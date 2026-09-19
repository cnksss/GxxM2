using System;

namespace GXX.Client.Scenes;

/// <summary>小地图图库标识（case g_nMiniMapIndex 五段）。</summary>
public enum MiniMapLibKind
{
    WBmpMap,   // g_WBmpMapImages（9900..9999 预留）
    MMap,      // g_WMMapImages（缺省段）
    MMap10,    // g_WMMapImages10（10000..14999）
    MMap11,    // g_WMMapImages11（15000..19999）
    MMap12,    // g_WMMapImages12（20000..24999）
}

/// <summary>雷达标记类别（Ex 版对应 g_WNewopUIImages 1650..1656；Big 版对应 btMinMapColor* 调色板色）。</summary>
public enum RadarKind
{
    Monster = 1650,
    Npc = 1651,
    Guard = 1652,
    OtherPlayer = 1653,
    Hero = 1654,
    Boss = 1655,
    Self = 1656,
}

/// <summary>雷达扫描的参与判定的角色快照（TActor headless 镜像）。</summary>
public sealed class RadarActor
{
    public int Race;
    public int RealRace;
    public bool Dead;
    public int X, Y;
    public bool IsNpcActor;    // Actor is TNpcActor（系统 NPC）
    public bool IsHumActor;    // Actor.ClassType = THumActor（其他玩家）
    public bool IsHeroActor;   // Actor.ClassType = THeroActor
    public bool IsMyHero;      // Actor = g_MyHero
    public string UserName = "";
}

/// <summary>小地图环境接缝：调色板（GetRGB）。</summary>
public static class MiniMapEnv
{
    public static Func<int, int> GetRgbFn = c => c;

    public static void Reset() => GetRgbFn = c => c;
}

/// <summary>
/// 小地图渲染纯逻辑 1:1（批次J55，SerialWindowsDlg.pas 30426/30701 + MShare.pas 4181-4200 + ClMain.pas 30048-30079）：
/// 图库选择五段、ActorXYToMapXY(×48/32,×32/32)/MapXYToActorXY(×32/48,×32/32)/MapToScreen/ScreenToMap（Delphi Round 银行家舍入）、
/// 雷达分类与四开关过滤、自身标记闪烁节拍（大窗 dwMinMapFlagFlash&lt;200 恒显 / 小窗置 True 粘滞）、
/// 地图描述文字定位钳制、大图取点反向换算、SM_READMINIMAP_OK/FAIL 消息状态机。
/// </summary>
public static class MiniMapRender
{
    // ---- 图库选择（DMinMapBigDlgPaint / DMinMapDlgExPaint 同款 case）----

    public static (MiniMapLibKind Lib, int Index)? SelectImage(int miniMapIndex)
    {
        if (miniMapIndex is >= 9900 and <= 9999)
            return (MiniMapLibKind.WBmpMap, miniMapIndex - 9900);
        if (miniMapIndex is >= 10000 and <= 14999)
            return (MiniMapLibKind.MMap10, miniMapIndex - 10000);
        if (miniMapIndex is >= 15000 and <= 19999)
            return (MiniMapLibKind.MMap11, miniMapIndex - 15000);
        if (miniMapIndex is >= 20000 and <= 24999)
            return (MiniMapLibKind.MMap12, miniMapIndex - 20000);
        return (MiniMapLibKind.MMap, miniMapIndex);
    }

    // ---- 坐标换算（MShare.pas 4181-4200）----

    public static (int X, int Y) ActorXYToMapXY(int currX, int currY)
        => (currX * 48 / 32, currY * 32 / 32);

    public static (int X, int Y) MapXYToActorXY(int mapX, int mapY)
        => (mapX * 32 / 48, mapY * 32 / 32);

    /// <summary>MapToScreen：Delphi Round 银行家舍入（64.5→64）。</summary>
    public static int MapToScreen(int mapWH, int screenWH, int mapXY)
        => (int)Math.Round((double)screenWH * mapXY / mapWH, MidpointRounding.ToEven);

    public static int ScreenToMap(int mapWH, int screenWH, int screenXY)
        => (int)Math.Round((double)mapWH * screenXY / screenWH, MidpointRounding.ToEven);

    // ---- 雷达（±12 扫描 + 分类 + 过滤）----

    public const int RadarScanRange = 12;

    /// <summary>扫描范围（g_MySelf.m_nCurrX−12 .. +12）。</summary>
    public static (int MinX, int MaxX, int MinY, int MaxY) RadarScanBounds(int selfX, int selfY)
        => (selfX - RadarScanRange, selfX + RadarScanRange, selfY - RadarScanRange, selfY + RadarScanRange);

    /// <summary>雷达显示四开关过滤（TMirConfigDlg 恒开；RC 常量 Grobal2 0/1/11/15/50/112，练功师 55）。</summary>
    public static bool RadarGate(RadarActor actor, bool showPlayer, bool showNpc, bool showAttackNpc, bool showActor, bool isMirConfigDlg)
    {
        if (actor.Race is 0 or 1) // RC_PLAYOBJECT, RC_HEROOBJECT → 玩家
            return showPlayer || isMirConfigDlg;
        if (actor.Race is 50 or 55) // RC_ANIMAL（和平NPC）, 练功师
            return showNpc || isMirConfigDlg;
        if (actor.Race == 15 || actor.RealRace is 11 or 112) // RC_PEACENPC / RC_GUARD 弓箭手, RC_ARCHERGUARD 大刀
            return showAttackNpc || isMirConfigDlg;
        return showActor || isMirConfigDlg; // 怪物
    }

    /// <summary>雷达分类（Ex 版 INDEX_* 逻辑：TNpcActor → THumActor → THeroActor → m_btRealRace switch + Boss 名单；己方英雄最后覆写）。</summary>
    public static RadarKind Classify(RadarActor actor, Func<string, bool> isBoss)
    {
        RadarKind kind;
        if (actor.IsNpcActor)
            kind = RadarKind.Npc; // 系统 NPC
        else if (actor.IsHumActor)
            kind = RadarKind.OtherPlayer; // 其他玩家
        else if (actor.IsHeroActor)
            kind = actor.IsMyHero ? RadarKind.Hero : RadarKind.OtherPlayer;
        else
        {
            switch (actor.RealRace)
            {
                case 0: // RC_PLAYOBJECT
                    kind = RadarKind.OtherPlayer;
                    break;
                case 55: // 练功师
                case 15: // RC_PEACENPC
                case 50: // RC_ANIMAL
                    kind = RadarKind.Npc;
                    break;
                case 11: // RC_GUARD 弓箭手
                case 112: // RC_ARCHERGUARD 大刀守卫
                    kind = RadarKind.Guard;
                    break;
                default:
                    kind = isBoss(actor.UserName) ? RadarKind.Boss : RadarKind.Monster;
                    break;
            }
        }
        if (actor.IsMyHero)
            kind = RadarKind.Hero; // g_MyHero 最后覆写
        return kind;
    }

    /// <summary>Big 版雷达色（GetRGB(btMinMapColor*)）。</summary>
    public static int RadarColor(RadarKind kind, int colorNpc, int colorOther, int colorHero, int colorGuard, int colorBoss, int colorMonster)
    {
        int raw = kind switch
        {
            RadarKind.Npc => colorNpc,
            RadarKind.OtherPlayer => colorOther,
            RadarKind.Hero => colorHero,
            RadarKind.Guard => colorGuard,
            RadarKind.Boss => colorBoss,
            RadarKind.Monster => colorMonster,
            _ => colorMonster,
        };
        return MiniMapEnv.GetRgbFn(raw);
    }

    /// <summary>Big 版自身标记闪烁（dwMinMapFlagFlash&lt;200 恒显；否则 tick_diff ≥ 间隔翻转；回绕安全减法）。</summary>
    public static bool BigSelfRadarVisible(uint now, uint flashInterval, ref bool isShow, ref uint lastTick)
    {
        if (flashInterval < 200)
        {
            isShow = true;
            return true;
        }
        uint diff = now >= lastTick ? now - lastTick : uint.MaxValue - lastTick + now; // tick_diff
        if (diff >= flashInterval)
        {
            isShow = !isShow;
            lastTick = now;
        }
        return isShow;
    }

    /// <summary>Ex 版自身图标闪烁（&lt;200 置 True 粘滞不主动复位；否则到间隔翻转——原版差异保留）。</summary>
    public static bool ExSelfRadarVisible(uint now, uint flashInterval, ref bool drawSelf, ref uint lastUpdate)
    {
        if (flashInterval < 200)
        {
            drawSelf = true;
        }
        else if (now - lastUpdate >= flashInterval)
        {
            drawSelf = !drawSelf;
            lastUpdate = now;
        }
        return drawSelf;
    }

    // ---- 版面 ----

    /// <summary>Ex 版视口：MapRect = Bounds(max(0, mapX − w div 2), max(0, mapY − h div 2), w, h)。</summary>
    public static (int Left, int Top, int Width, int Height) ExViewport(int selfMapX, int selfMapY, int width, int height)
        => (Math.Max(0, selfMapX - width / 2), Math.Max(0, selfMapY - height / 2), width, height);

    /// <summary>Big 版地图描述文字定位：居中于标记点后钳制到虚拟矩形内（nX/nY 取 _Max 下界，右/下越界收缩）。</summary>
    public static (int X, int Y) DescTextPos(int vtLeft, int vtTop, int vtRight, int vtBottom,
        int mapScreenX, int mapScreenY, int textWidth, int fontHeight)
    {
        int nX = Math.Max(vtLeft, vtLeft + mapScreenX - textWidth / 2);
        int nY = Math.Max(vtTop, vtTop + mapScreenY);
        if (nX + textWidth > vtRight)
            nX = vtRight - textWidth;
        if (nY + fontHeight > vtBottom)
            nY = vtBottom - fontHeight;
        return (nX, nY);
    }

    /// <summary>Big 版鼠标取点反向换算（ShowMiniBigMapXY）：Round((大图坐标−Left)×图宽/控件宽) → MapXYToActorXY。</summary>
    public static (int ActorX, int ActorY) BigMapPointToActor(int bigX, int bigY, int left, int top, int texW, int texH, int screenW, int screenH)
    {
        int nMapX = (int)Math.Round((double)(bigX - left) * texW / screenW, MidpointRounding.ToEven);
        int nMapY = (int)Math.Round((double)(bigY - top) * texH / screenH, MidpointRounding.ToEven);
        return MapXYToActorXY(nMapX, nMapY);
    }

    // ---- SM_READMINIMAP_OK / FAIL 消息状态（ClMain.pas 30048-30079）----

    public sealed class MiniMapMessageState
    {
        public int MiniMapIndex = -1;   // g_nMiniMapIndex
        public bool ViewMiniMap;        // g_boViewMiniMap
        public int ViewMinMapLv;        // g_nViewMinMapLv
        public uint QueryMsgTick;       // g_dwQueryMsgTick
    }

    public sealed class MiniMapMessageResult
    {
        public bool LevelChangeNotified; // FrmDlg.MinMapLevelChange
        public bool ShowDgjPoints;       // FrmDlg.ShowDGJPointsDlg（挂机图）
        public bool ChatBoardMessage;    // '没有可用的地图'（仅 FAIL）
    }

    /// <summary>ProcessMessageReadMiniMap 1:1：param≥1 才处理；Recog=1 挂机图（index=param−1 + ShowDGJPoints）；否则普通图（index=param−1，view=index≥0，Lv 负值归 0 + MinMapLevelChange）。</summary>
    public static MiniMapMessageResult HandleReadMiniMapOk(MiniMapMessageState state, int param, int recog)
    {
        if (param < 1)
            return new MiniMapMessageResult();
        bool isGjMap = recog == 1;
        if (isGjMap)
        {
            state.MiniMapIndex = param - 1;
            return new MiniMapMessageResult { ShowDgjPoints = true };
        }
        state.ViewMiniMap = true;
        state.MiniMapIndex = param - 1;
        state.ViewMiniMap = state.MiniMapIndex >= 0;
        if (state.ViewMinMapLv <= 0)
            state.ViewMinMapLv = 0;
        return new MiniMapMessageResult { LevelChangeNotified = true };
    }

    /// <summary>ProcessMessageReadMiniMapFail 1:1：查询时钟重置、index=-1、MinMapLevelChange、聊天板提示。</summary>
    public static MiniMapMessageResult HandleReadMiniMapFail(MiniMapMessageState state, uint now)
    {
        state.QueryMsgTick = now;
        state.MiniMapIndex = -1;
        return new MiniMapMessageResult { LevelChangeNotified = true, ChatBoardMessage = true };
    }
}

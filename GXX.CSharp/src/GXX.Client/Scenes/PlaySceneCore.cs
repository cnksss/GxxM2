using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// MShare.pas/SDK.pas 客户端屏幕常量（ScreenXYfromMCXY/CXYfromMouseXY 依赖）。
/// </summary>
public static class PlaySceneConsts
{
    public const int UNITX = 48;
    public const int UNITY = 32;
    public const int DR_UP = 0;
    public const int DR_UPRIGHT = 1;
    public const int DR_RIGHT = 2;
    public const int DR_DOWNRIGHT = 3;
    public const int DR_DOWN = 4;
    public const int DR_DOWNLEFT = 5;
    public const int DR_LEFT = 6;
    public const int DR_UPLEFT = 7;

    // Grobal2.pas Race 常量
    public const int RC_PLAYOBJECT = 0;
    public const int RC_HEROOBJECT = 1;
    public const int RC_MERCHANT = 50;
    public const int RC_GUARD = 11;
    public const int RC_ARCHERGUARD = 112;
    public const int RC_MOVE_ARCHERGUARD = 142;
}

/// <summary>地图查询接缝（MapUnit.pas TMap 的 headless 镜像）。</summary>
public interface IPlayMap
{
    int BlockLeft { get; }
    int BlockTop { get; }
    int ClientLeft { get; }
    int ClientTop { get; }
    int ClientRight { get; }
    int ClientBottom { get; }

    /// <summary>TMap.CanMove（940-974：块坐标范围 0..312 + EI/EN/普通三分支 + 门开合位）。</summary>
    bool CanMove(int mx, int my);

    /// <summary>TMap.NewCanMove（976-979：GetMapCellInfo = 0）。</summary>
    bool NewCanMove(int mx, int my);

    /// <summary>TMap.CanFly（981-990：仅前层掩码 $8000）。</summary>
    bool CanFly(int mx, int my);

    /// <summary>g_boMapMoving（换地图中禁止创建角色）。</summary>
    bool MapMoving { get; }

    // ---- 批次J73：SendMsg 换图族（PlayScn.pas 7779-7842） ----
    /// <summary>Map.LoadMap（换图装载；headless 仅记录当前地图名）。</summary>
    void LoadMap(string mapName, int x, int y);

    /// <summary>Map.m_sOldMap（旧地图名）。</summary>
    string OldMapName { get; set; }

    /// <summary>g_boViewMiniMap 的窗体侧条件（TSerialWindows.DMinMapDlgEx.Visible）。</summary>
    bool MiniMapVisible { get; }

    /// <summary>FrmDlg.CheckDMinMapBigDlgVisible（小地图放大窗可见）。</summary>
    bool CheckDMinMapBigDlgVisible { get; }

    /// <summary>FrmDlg.IsDGJPointsShow（打怪点显示）。</summary>
    bool IsDGJPointsShow { get; }
}

/// <summary>
/// 客户端全局态（MShare.pas 全局变量族的 headless 容器；PlayScene 全族依赖）。
/// </summary>
public sealed class PlaySceneGlobals
{
    public TActorCore? MySelf;
    public TActorCore? MyHero;

    /// <summary>g_nScreenCenterX/Y（SDK.pas，默认 1024×768 半屏）。</summary>
    public int ScreenCenterX = 512;
    public int ScreenCenterY = 384;

    /// <summary>g_ClientConfig（客户端配置子集：穿人/穿怪/穿 NPC/守卫/马三格/宠物无实体）。</summary>
    public bool boPetNoEntity;
    public bool boHorseRun3Grid;

    /// <summary>g_HumanRunConfig（穿行规则）。</summary>
    public bool boCanRunHuman;
    public bool boCanRunNpc;
    public bool boCanRunMon;
    public bool boCanRunGuard;

    /// <summary>g_ConfigDlg.ConfigCheckeds[ckSimpleShowActor / ckSimpleShowBB]。</summary>
    public bool ckSimpleShowActor;
    public bool ckSimpleShowBB;

    /// <summary>g_ClientConfig.boSimpleShowActor / boSimpleShowBB。</summary>
    public bool boSimpleShowActor;
    public bool boSimpleShowBB;

    /// <summary>g_ConfigClient 简装自定义（-1 表示未配置）。</summary>
    public bool boCustomActorSimpleShow;
    public int nSimpleActorRaceImg = -1;
    public int nSimpleActorAppr = -1;
    public int nSimpleActorRace = -1;
    public bool boCustomBBSimpleShow;
    public int nSimpleBBSimpleRaceImg = -1;
    public int nSimpleBBAppr = -1;
    public int nSimpleBBRace = -1;

    /// <summary>g_nMagicTargetRecogId / g_MagicLockActor（NewActor 尾部锁定修正）。</summary>
    public long g_nMagicTargetRecogId;
    public TActorCore? g_MagicLockActor;

    // ---- 批次J73：全局指向与锁定（CleanObjects/ClearActors/SendMsg 族） ----
    public TActorCore? TargetCret;
    public TActorCore? FocusCret;
    public uint FocusCretTick;
    public long MagicTarget;      // g_MagicTarget（FGameObject 指针；headless 以 RecogId 承载）
    public TActorCore? BrightActor;
    public TActorCore? LockTarget;  // g_LockTarget
    public uint dwLockTargetTick;
}

/// <summary>
/// PlayScn.pas 73-278 TPlayScene 第一批（批次J72）：
/// 场景生命周期（Initialize/Finalize/OpenScene/CloseScene/RefreshScene）、
/// 角色表（DoAddActor/DoDelActor/DoSearchActor/DoSearchSortYDrawActtor 二分）、
/// 坐标换算（ScreenXYfromMCXY/CXYfromMouseXY）、碰撞族（CanWalk/CanWalkEx/CrashMan 等 15 个）、
/// 鼠标选角（GetCharacter/GetAttackFocusCharacter/IsSelectMyself）、
/// 地面物品（GetDropItems×2/GetXYDropItems×2）、NewActor 主体与 ButchAnimal。
/// </summary>
public partial class TPlayScene : TScene
{
    public SceneDialogs FrmDlg;

    // ---- 场景状态（Initialize 471-481） ----
    public int m_nShiftX;
    public int m_nShiftY;
    public int m_nCurrX = -1;
    public int m_nCurrY = -1;
    public int m_nCurrentAction;
    public int m_nActionCount;
    public int m_nProcDrawSceneIdx;
    public int m_nProcDrawEffectIdx;
    public int m_nProcDrawItemsIdx;
    public int m_nProcDrawActorLabelIdx;
    public int m_nProcDropItemsIdx;
    public bool m_boCanDraw;
    public int m_nDefXX;
    public int m_nDefYY;

    // ---- 四张角色表（PlayScn 字段） ----
    public readonly List<TActorCore> ActorList = new();        // m_ActorList（按 m_nRecogId 有序）
    public readonly List<TActorCore> DrawActorList = new();    // m_DrawActorList
    public readonly List<TActorCore> SortYDrawActorList = new(); // m_SortYDrawActorList（按 Ry-DownDrawLevel 有序）
    public readonly List<TActorCore> FreeActorList = new();    // m_FreeActorList（AddFreeActorList）
    public readonly List<TChrMsg> MsgList = new();            // m_MsgList

    // ---- 震动 ----（CheckSceneShake/SceneShake 8273-8314）
    public readonly List<int> SceneShakeList = new();
    public bool m_boDelaySceneShake;
    public uint m_dwDelaySceneShakeTick;
    public uint m_dwDelaySceneShakeTime;
    public int m_dwDelaySceneShakeCount;

    /// <summary>ShakeX/ShakeY（PlayScn.pas 44-45 单元全局）。</summary>
    public static int ShakeX;
    public static int ShakeY;

    /// <summary>WalkShift（PlayScn.pas 47-56：8 方向 × 6 格像素偏移）。</summary>
    public static readonly (int X, int Y)[][] WalkShift =
    {
        // DR_UP
        new[] { (0, 5), (0, 6), (0, 5), (0, 5), (0, 6), (0, 5) },
        // DR_UPRIGHT
        new[] { (8, 5), (8, 6), (8, 5), (8, 5), (8, 6), (8, 5) },
        // DR_RIGHT
        new[] { (8, 0), (8, 0), (8, 0), (8, 0), (8, 0), (8, 0) },
        // DR_DOWNRIGHT
        new[] { (8, 5), (8, 6), (8, 5), (8, 5), (8, 6), (8, 5) },
        // DR_DOWN
        new[] { (0, 5), (0, 6), (0, 5), (0, 5), (0, 6), (0, 5) },
        // DR_DOWNLEFT
        new[] { (8, 5), (8, 6), (8, 5), (8, 5), (8, 6), (8, 5) },
        // DR_LEFT
        new[] { (8, 0), (8, 0), (8, 0), (8, 0), (8, 0), (8, 0) },
        // DR_UPLEFT
        new[] { (8, 5), (8, 6), (8, 5), (8, 5), (8, 6), (8, 5) },
    };

    // ---- 接缝 ----
    public PlaySceneGlobals G = new();
    public IPlayMap Map;
    public DropItemsStore? DropItems;

    /// <summary>g_WDnItemImages.Images[looks] 探测接缝（null = 图未就绪 → 跳过）。</summary>
    public Func<int, SurfaceSize?>? DropItemImageProbe;

    /// <summary>CheckTextureAlpha 接缝（掉落物命中）。</summary>
    public Func<SurfaceSize, int, int, bool>? CheckTextureAlpha;

    /// <summary>地面物品图库数量（g_WDnItemImages.Count）。</summary>
    public int DropItemImageCount = 2000;

    /// <summary>地图是否已加载（CanDrawTileMap 891-894）。</summary>
    public bool MapLoadOk = true;
    public bool boCanDrawTileMap = true;

    /// <summary>g_MySelf 访问器（Delphi 单元全局）。</summary>
    public TActorCore? MySelf => G.MySelf;
    public TActorCore? MyHero => G.MyHero;

    /// <summary>AddFreeActorList 接缝（Delphi 释放链；headless 入回收表）。</summary>
    public void AddFreeActorList(TActorCore actor) => FreeActorList.Add(actor);

    public TPlayScene(SceneDialogs dialogs, IPlayMap? map = null) : base(TSceneType.stPlayGame)
    {
        FrmDlg = dialogs;
        Map = map ?? new DefaultPlayMap();
    }

    // ================= 生命周期 =================

    /// <summary>PlayScn.pas 450-528 Initialize 1:1（状态清零 + 名条贴图构建后立即释放）。</summary>
    public override void Initialize()
    {
        m_nShiftX = 0;
        m_nShiftY = 0;
        m_nCurrX = -1;
        m_nCurrY = -1;
        m_nCurrentAction = 0;
        m_nActionCount = 0;
        m_nProcDrawSceneIdx = 0;
        m_nProcDrawEffectIdx = 0;
        m_nProcDrawItemsIdx = 0;
        m_nProcDrawActorLabelIdx = 0;
        m_nProcDropItemsIdx = 0;

        // HumLabel := MakeActorLabel(clRed, clRed) 等 5 张名条随后即 FreeAndNil（原文废弃物）
        MakeActorLabelCount += 5;
        m_boCanDraw = true;
    }

    /// <summary>MakeActorLabel 调用计数（原文构建 5 张名条后全部释放，保留语义位）。</summary>
    public int MakeActorLabelCount;

    /// <summary>PlayScn.pas 530-572 Finalize 1:1（停绘 + 全角色 Finalize + 事件清空 + 地图旧矩形清零 + 掉落物贴图引用清空）。</summary>
    public override void Finalize()
    {
        m_boCanDraw = false;
        for (int i = 0; i < ActorList.Count; i++)
            ActorList[i].OnFinalize?.Invoke();

        EventManFinalize?.Invoke();
        OldClientRectCleared = true;

        if (DropItems != null)
        {
            foreach (var point in DropItems.Points)
            {
                foreach (var item in point.Items)
                {
                    item.ItemTexture = null;
                    item.TextureWidth = 0;
                    item.TextureHeight = 0;
                    item.ClearNameImage();
                }
            }
        }
    }

    /// <summary>EventMan.Finalize 接缝。</summary>
    public Action? EventManFinalize;

    /// <summary>FillChar(Map.m_OldClientRect, 0) 语义位。</summary>
    public bool OldClientRectCleared;

    /// <summary>PlayScn.pas 574-579 OpenScene 1:1。</summary>
    public override void OpenScene()
    {
        FrmDlg.ViewBottomBox(true);
    }

    /// <summary>PlayScn.pas 581-588 CloseScene 1:1（SilenceSound + 隐藏聊天输入 + 收起底栏）。</summary>
    public override void CloseScene()
    {
        SilenceSoundCount++;
        FrmDlg.HideChatEdit();
        FrmDlg.ViewBottomBox(false);
    }

    public int SilenceSoundCount;

    public override void OpeningScene() { }

    public override void RefreshScene() { }

    /// <summary>PlayScn.pas 891-894 CanDrawTileMap 1:1（地图已加载且开关开启，且非换图中）。</summary>
    public bool CanDrawTileMap()
        => boCanDrawTileMap && MapLoadOk && !Map.MapMoving;

    // ================= 角色表（二分） =================

    /// <summary>PlayScn.pas 8339-8362 DoSearchActor 1:1（按 m_nRecogId 二分；命中返回 true，Index 为位置/插入位）。</summary>
    public bool DoSearchActor(long id, out int index)
    {
        bool result = false;
        int l = 0;
        int h = ActorList.Count - 1;
        while (l <= h)
        {
            int i = (l + h) >> 1;
            long c = ActorList[i].m_nRecogId - id;
            if (c < 0)
                l = i + 1;
            else
            {
                h = i - 1;
                if (c == 0)
                {
                    result = true;
                    l = i;
                }
            }
        }
        index = l;
        return result;
    }

    /// <summary>PlayScn.pas 8364-8386 DoSearchSortYDrawActtor 1:1（按 Ry-DownDrawLevel 二分）。</summary>
    public bool DoSearchSortYDrawActtor(int nRY, out int index)
    {
        bool result = false;
        int l = 0;
        int h = SortYDrawActorList.Count - 1;
        while (l <= h)
        {
            int i = (l + h) >> 1;
            var a = SortYDrawActorList[i];
            int c = (a.m_nRy - a.m_nDownDrawLevel) - nRY;
            if (c < 0)
                l = i + 1;
            else
            {
                h = i - 1;
                if (c == 0)
                    result = true;
            }
        }
        index = l;
        return result;
    }

    /// <summary>PlayScn.pas 8316-8324 DoAddActor 1:1（未存在 → 有序插入 ActorList + 尾附 DrawActorList）。</summary>
    public void DoAddActor(TActorCore actor)
    {
        if (!DoSearchActor(actor.m_nRecogId, out int index))
        {
            ActorList.Insert(index, actor);
            DrawActorList.Add(actor);
        }
    }

    /// <summary>PlayScn.pas 8326-8337 DoDelActor 1:1（命中删 ActorList；DrawActorList 恒 Remove）。</summary>
    public bool DoDelActor(TActorCore actor)
    {
        bool result = false;
        if (DoSearchActor(actor.m_nRecogId, out int index))
        {
            ActorList.RemoveAt(index);
            result = true;
        }
        DrawActorList.Remove(actor);
        return result;
    }

    /// <summary>PlayScn.pas 6792-6814 FindActorList 1:1（二分命中即返回，不看删除标记）。</summary>
    public TActorCore? FindActorList(long id)
        => DoSearchActor(id, out int index) ? ActorList[index] : null;

    /// <summary>PlayScn.pas 6816-6848 FindActor(id) 1:1（二分命中且未标记删除）。</summary>
    public TActorCore? FindActor(long id)
    {
        if (DoSearchActor(id, out int index))
        {
            var actor = ActorList[index];
            if (!actor.m_boDelActor)
                return actor;
        }
        return null;
    }

    /// <summary>PlayScn.pas 6850-6872 FindActor(name) 1:1（CompareText 线性查找）。</summary>
    public TActorCore? FindActor(string sname)
    {
        for (int i = 0; i < ActorList.Count; i++)
        {
            var actor = ActorList[i];
            if (!actor.m_boDelActor && string.Compare(actor.m_sUserName, sname, StringComparison.OrdinalIgnoreCase) == 0)
                return actor;
        }
        return null;
    }

    /// <summary>PlayScn.pas 6874-6897 FindActorXY 1:1（首个匹配；命中死亡/不可见/不占位者继续扫描）。</summary>
    public TActorCore? FindActorXY(int x, int y)
    {
        TActorCore? result = null;
        for (int i = 0; i < ActorList.Count; i++)
        {
            var actor = ActorList[i];
            if (!actor.m_boDelActor && actor.m_nCurrX == x && actor.m_nCurrY == y)
            {
                result = actor;
                if (!result.m_boDeath && result.m_boVisible && result.m_boHoldPlace)
                    break;
            }
        }
        return result;
    }

    /// <summary>PlayScn.pas 6899-6927 FindActorXY(X,Y,Actor) 1:1（指定对象匹配；未通过可见性判定则返回 null）。</summary>
    public TActorCore? FindActorXY(int x, int y, TActorCore actor)
    {
        TActorCore? result = null;
        for (int i = 0; i < ActorList.Count; i++)
        {
            var a = ActorList[i];
            if (!a.m_boDelActor && a.m_nCurrX == x && a.m_nCurrY == y && ReferenceEquals(actor, a))
            {
                result = a;
                if (!((!result.m_boDeath) && result.m_boVisible && result.m_boHoldPlace))
                    result = null;
                break;
            }
        }
        return result;
    }

    /// <summary>PlayScn.pas 6929-6952 IsValidActor 1:1（ActorList 中存在且未标记删除）。</summary>
    public bool IsValidActor(TActorCore actor)
    {
        for (int i = 0; i < ActorList.Count; i++)
        {
            var a = ActorList[i];
            if (!a.m_boDelActor && ReferenceEquals(a, actor))
                return true;
        }
        return false;
    }

    /// <summary>PlayScn.pas 6954-6967 IsValidActorEx 1:1（DrawActorList 中存在且未标记删除）。</summary>
    public bool IsValidActorEx(TActorCore actor)
    {
        for (int i = 0; i < DrawActorList.Count; i++)
        {
            var a = DrawActorList[i];
            if (!a.m_boDelActor && ReferenceEquals(a, actor))
                return true;
        }
        return false;
    }

    /// <summary>PlayScn.pas 7718-7754 ButchAnimal 1:1（先精确坐标、再 ±1 范围找已死亡非人物对象）。</summary>
    public TActorCore? ButchAnimal(int x, int y)
    {
        for (int i = 0; i < ActorList.Count; i++)
        {
            var a = ActorList[i];
            if (a.m_boDeath && a.m_btRace != 1
                && Math.Abs(a.m_nCurrX - x) == 0 && Math.Abs(a.m_nCurrY - y) == 0)
                return a;
        }
        for (int i = 0; i < ActorList.Count; i++)
        {
            var a = ActorList[i];
            if (a.m_boDeath && a.m_btRace != 1
                && Math.Abs(a.m_nCurrX - x) <= 1 && Math.Abs(a.m_nCurrY - y) <= 1)
                return a;
        }
        return null;
    }

    // ================= 坐标换算 =================

    /// <summary>PlayScn.pas 5942-5968 ScreenXYfromMCXY 1:1（g_MySelf 为空时不改动出参）。</summary>
    public void ScreenXYfromMCXY(int cx, int cy, ref int sx, ref int sy)
    {
        var self = G.MySelf;
        if (self == null)
            return;

        int nWidth = Math.Abs(self.m_nRx - cx);
        int nHeight = Math.Abs(self.m_nRy - cy);
        if (self.m_nRx > cx)
            sx = G.ScreenCenterX - nWidth * PlaySceneConsts.UNITX;
        else
            sx = G.ScreenCenterX + nWidth * PlaySceneConsts.UNITX;

        if (self.m_nRy > cy)
            sy = G.ScreenCenterY - nHeight * PlaySceneConsts.UNITY;
        else
            sy = G.ScreenCenterY + nHeight * PlaySceneConsts.UNITY;

        sx -= self.m_nShiftX;
        sy -= self.m_nShiftY;
    }

    /// <summary>PlayScn.pas 5972-6020 CXYfromMouseXY 1:1（整除 + 余数过半进位；g_MySelf 为空时不改动出参）。</summary>
    public void CXYfromMouseXY(int mx, int my, ref int ccx, ref int ccy)
    {
        var self = G.MySelf;
        if (self == null)
            return;

        int nWidth = Math.Abs(mx - G.ScreenCenterX + self.m_nShiftX) / PlaySceneConsts.UNITX;
        int nHeight = Math.Abs(my - G.ScreenCenterY + self.m_nShiftY) / PlaySceneConsts.UNITY;
        if (Math.Abs(G.ScreenCenterX - mx) % PlaySceneConsts.UNITX > PlaySceneConsts.UNITX / 2)
            nWidth++;
        if (Math.Abs(G.ScreenCenterY - my) % PlaySceneConsts.UNITY > PlaySceneConsts.UNITY / 2)
            nHeight++;

        if (G.ScreenCenterX > mx)
            ccx = self.m_nRx - nWidth;
        else
            ccx = self.m_nRx + nWidth;

        if (G.ScreenCenterY > my)
            ccy = self.m_nRy - nHeight;
        else
            ccy = self.m_nRy + nHeight;
    }

    // ================= 碰撞族 =================

    /// <summary>PlayScn.pas 6685-6690 CanWalk 1:1。</summary>
    public bool CanWalk(int mx, int my) => Map.CanMove(mx, my) && !CrashMan(mx, my);

    /// <summary>PlayScn.pas 6466-6471 UnLockCanWalkEx 1:1。</summary>
    public bool UnLockCanWalkEx(int mx, int my) => Map.CanMove(mx, my) && !UnLockCrashManEx(mx, my);

    /// <summary>PlayScn.pas 6473-6478 CanWalkEx 1:1。</summary>
    public bool CanWalkEx(int mx, int my) => Map.CanMove(mx, my) && !CrashManEx(mx, my);

    /// <summary>PlayScn.pas 6480-6485 NewCanWalkEx 1:1（NewCanMove 版）。</summary>
    public bool NewCanWalkEx(int mx, int my) => Map.NewCanMove(mx, my) && !CrashManEx(mx, my);

    /// <summary>PlayScn.pas 6487-6492 NewCanWalkEx_2 1:1（与 NewCanWalkEx 同体，原文重复实现保留）。</summary>
    public bool NewCanWalkEx_2(int mx, int my) => Map.NewCanMove(mx, my) && !CrashManEx(mx, my);

    /// <summary>PlayScn.pas 6451-6464 NewCanRun 1:1（一步 + 终点两次 NewCanWalkEx）。</summary>
    public bool NewCanRun(int sx, int sy, int ex, int ey)
    {
        int ndir = MapPath.GetNextDirection(sx, sy, ex, ey);
        int rx = sx, ry = sy;
        MapPath.GetNextPosXY(ndir, ref rx, ref ry);
        return NewCanWalkEx(rx, ry) && NewCanWalkEx(ex, ey);
    }

    /// <summary>g_MySelf.m_btHorse &lt;&gt; 0（原文骑马判定；显式比较避免 byte 常量模式误配）。</summary>
    public bool IsOnHorse => G.MySelf != null && G.MySelf.m_btHorse != 0;

    /// <summary>PlayScn.pas 6433-6449 CanRun 1:1（含骑马一步三格追加判定）。</summary>
    public bool CanRun(int sx, int sy, int ex, int ey)
    {
        int ndir = MapPath.GetNextDirection(sx, sy, ex, ey);
        int rx = sx, ry = sy;
        MapPath.GetNextPosXY(ndir, ref rx, ref ry);

        bool result = CanWalkEx(rx, ry) && CanWalkEx(ex, ey);

        if (result && IsOnHorse && G.boHorseRun3Grid)
        {
            MapPath.GetNextPosXY(ndir, ref rx, ref ry);
            result = CanWalkEx(rx, ry);
        }
        return result;
    }

    /// <summary>PlayScn.pas 6406-6429 UnLockCanRun 1:1（重写版：Map.CanMove 对 + UnLock 对 + 马三格追加）。</summary>
    public bool UnLockCanRun(int sx, int sy, int ex, int ey)
    {
        bool result = false;
        int ndir = MapPath.GetNextDirection(sx, sy, ex, ey);
        int rx = sx, ry = sy;
        MapPath.GetNextPosXY(ndir, ref rx, ref ry);

        if (Map.CanMove(rx, ry) && Map.CanMove(ex, ey))
        {
            if (UnLockCanWalkEx(rx, ry) && UnLockCanWalkEx(ex, ey))
            {
                if (IsOnHorse && G.boHorseRun3Grid)
                {
                    MapPath.GetNextPosXY(ndir, ref rx, ref ry);
                    if (Map.CanMove(rx, ry) && Map.CanMove(ex, ey))
                        result = UnLockCanWalkEx(rx, ry) && UnLockCanWalkEx(ex, ey);
                }
                else
                    result = true;
            }
        }
        return result;
    }

    /// <summary>PlayScn.pas 6334-6366 CanHorseRun 1:1（两格 + 终点，Map.CanMove 与 CanWalkEx 交替早退）。</summary>
    public bool CanHorseRun(int sx, int sy, int ex, int ey)
    {
        int ndir = MapPath.GetNextDirection(sx, sy, ex, ey);
        int rx = sx, ry = sy;
        MapPath.GetNextPosXY(ndir, ref rx, ref ry);

        if (!Map.CanMove(rx, ry)) return false;
        if (!CanWalkEx(rx, ry)) return false;

        MapPath.GetNextPosXY(ndir, ref rx, ref ry);
        if (!Map.CanMove(rx, ry)) return false;
        if (!CanWalkEx(rx, ry)) return false;

        if (!Map.CanMove(ex, ey)) return false;
        return CanWalkEx(ex, ey);
    }

    /// <summary>PlayScn.pas 6692-6697 UnLockCanWalk 1:1。</summary>
    public bool UnLockCanWalk(int mx, int my) => Map.CanMove(mx, my) && !UnLockCrashMan(mx, my);

    /// <summary>PlayScn.pas 6699-6722 UnLockCrashMan 1:1（无 12 格距离门；NPC 允许开关生效）。</summary>
    public bool UnLockCrashMan(int mx, int my)
    {
        for (int i = 0; i < ActorList.Count; i++)
        {
            var actor = ActorList[i];
            if (actor.m_boVisible && actor.m_boHoldPlace && !actor.m_boDeath
                && actor.m_nCurrX == mx && actor.m_nCurrY == my)
            {
                if (actor.m_HumsBBType == THumBBType.bbGamePet && G.boPetNoEntity)
                    continue;

                if (actor.m_btRace == PlaySceneConsts.RC_MERCHANT
                    && (G.boCanRunNpc || IsMerchantAppearance(actor.m_wAppearance)))
                    continue;

                if (actor.m_btHorse is 1 or 2)
                    continue;

                return true;
            }
        }
        return false;
    }

    /// <summary>PlayScn.pas 6724-6756 CrashMan 1:1（NPC 路只认外观 54..58/94..98，boCanRunNpc 被原文注释）。</summary>
    public bool CrashMan(int mx, int my)
    {
        for (int i = 0; i < ActorList.Count; i++)
        {
            var actor = ActorList[i];
            if (actor.m_boVisible && actor.m_boHoldPlace && !actor.m_boDeath
                && actor.m_nCurrX == mx && actor.m_nCurrY == my)
            {
                if (actor.m_HumsBBType == THumBBType.bbGamePet && G.boPetNoEntity)
                    continue;

                // 原文：{g_HumanRunConfig.boCanRunNpc or}(Actor.m_wAppearance in [54..58, 94..98])
                if (actor.m_btRace == PlaySceneConsts.RC_MERCHANT && IsMerchantAppearance(actor.m_wAppearance))
                    continue;

                if (actor.m_btHorse is 1 or 2)
                    continue;

                return true;
            }
        }
        return false;
    }

    /// <summary>PlayScn.pas 6553-6619 CrashManEx 1:1（12 格距离门 + 倒序遍历 + 五条穿越豁免）。</summary>
    public bool CrashManEx(int mx, int my) => CrashManExCore(mx, my, reverse: true);

    /// <summary>PlayScn.pas 6621-6683 CrashManEx_2 1:1（与 CrashManEx 同体，原文重复保留）。</summary>
    public bool CrashManEx_2(int mx, int my) => CrashManExCore(mx, my, reverse: true);

    /// <summary>PlayScn.pas 6496-6549 UnLockCrashManEx 1:1（正向遍历 + 宠物/人怪 Npc/怪/守卫/骑马五豁免）。</summary>
    public bool UnLockCrashManEx(int mx, int my) => CrashManExCore(mx, my, reverse: false);

    private bool CrashManExCore(int mx, int my, bool reverse)
    {
        var self = G.MySelf;
        if (self == null)
            return false;
        if (Math.Abs(self.m_nCurrX - mx) > 12 || Math.Abs(self.m_nCurrY - my) > 12)
            return false;

        for (int step = 0; step < ActorList.Count; step++)
        {
            var actor = ActorList[reverse ? ActorList.Count - 1 - step : step];
            if (!(actor.m_boVisible && actor.m_boHoldPlace && !actor.m_boDeath
                && actor.m_nCurrX == mx && actor.m_nCurrY == my))
                continue;

            if (actor.m_HumsBBType == THumBBType.bbGamePet && G.boPetNoEntity)
                continue;

            if (IsHumanRace(actor.m_btRace) && !actor.m_boPlayMoster && G.boCanRunHuman)
                continue;

            if (actor.m_btRace == PlaySceneConsts.RC_MERCHANT
                && (G.boCanRunNpc || IsMerchantAppearance(actor.m_wAppearance)))
                continue;

            if (IsMonsterLike(actor) && G.boCanRunMon)
                continue;

            if (G.boCanRunGuard && IsGuardRace(actor.m_btRealRace))
                continue;

            if (actor.m_btHorse is 1 or 2)
                continue;

            return true;
        }
        return false;
    }

    internal static bool IsMerchantAppearance(int appearance)
        => appearance is (>= 54 and <= 58) or (>= 94 and <= 98);

    internal static bool IsHumanRace(int race) => race is 0 or 1;

    internal static bool IsGuardRace(int realRace)
        => realRace is PlaySceneConsts.RC_GUARD or 12 or PlaySceneConsts.RC_ARCHERGUARD
            or PlaySceneConsts.RC_MOVE_ARCHERGUARD;

    /// <summary>原文怪物判定：(race &gt; RC_HEROOBJECT 且 ≠ RC_MERCHANT 且 realRace ≠ 55 且非守卫) 或 (race = 0 且人型怪)。</summary>
    internal static bool IsMonsterLike(TActorCore actor)
        => ((actor.m_btRace > PlaySceneConsts.RC_HEROOBJECT
             && actor.m_btRace != PlaySceneConsts.RC_MERCHANT
             && actor.m_btRealRace != 55
             && !IsGuardRace(actor.m_btRealRace))
            || (actor.m_btRace == 0 && actor.m_boPlayMoster));

    /// <summary>PlayScn.pas 6758-6761 CanFly 1:1。</summary>
    public bool CanFly(int mx, int my) => Map.CanFly(mx, my);

    /// <summary>PlayScn.pas 6763-6775 MapCanMove 1:1。</summary>
    public bool MapCanMove(int mx, int my) => Map.CanMove(mx, my);

    /// <summary>PlayScn.pas 6777-6789 MapCanFly 1:1。</summary>
    public bool MapCanFly(int mx, int my) => Map.CanFly(mx, my);

    // ================= 鼠标选角 =================

    /// <summary>
    /// PlayScn.pas 6022-6066 GetCharacter 1:1（按 k=ccy+8..ccy-1 行倒序扫描 CheckSelect）。
    /// 关键：Result 只在「已命中的第 wantsel 个」才落定（原文 Result:=a 位于 nowsel&gt;=wantsel 分支内）
    /// —— 未达 wantsel 的中间命中只推进 nowsel，不产生返回值。
    /// </summary>
    public TActorCore? GetCharacter(int x, int y, int wantsel, out int nowsel, bool liveonly)
    {
        TActorCore? result = null;
        nowsel = -1;
        int ccx = 0, ccy = 0;
        CXYfromMouseXY(x, y, ref ccx, ref ccy);

        for (int k = ccy + 8; k >= ccy - 1; k--)
        {
            bool boFind = false;
            for (int i = ActorList.Count - 1; i >= 0; i--)
            {
                var a = ActorList[i];
                if (ReferenceEquals(a, G.MySelf))
                    continue;
                if ((!liveonly || !a.m_boDeath) && a.m_boHoldPlace && a.m_boVisible
                    && (a.m_HumsBBType != THumBBType.bbGamePet || !G.boPetNoEntity))
                {
                    if (a.m_nCurrY == k)
                    {
                        int dx = (a.m_nRx - Map.ClientLeft) * PlaySceneConsts.UNITX + m_nDefXX + a.m_nPx + a.m_nShiftX;
                        int dy = (a.m_nRy - Map.ClientTop - 1) * PlaySceneConsts.UNITY + m_nDefYY + a.m_nPy + a.m_nShiftY;
                        if (a.CheckSelect(x - dx, y - dy))
                        {
                            nowsel++;
                            if (nowsel >= wantsel)
                            {
                                result = a;
                                boFind = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (boFind)
                break;
        }
        return result;
    }

    /// <summary>PlayScn.pas 6070-6163 GetAttackFocusCharacter 1:1（GetCharacter 未命中 → 按宽高包围盒放宽命中）。</summary>
    public TActorCore? GetAttackFocusCharacter(int x, int y, int wantsel, out int nowsel, bool liveonly)
    {
        var result = GetCharacter(x, y, wantsel, out nowsel, liveonly);
        if (result != null)
            return result;

        nowsel = -1;
        int ccx = 0, ccy = 0;
        CXYfromMouseXY(x, y, ref ccx, ref ccy);

        for (int k = ccy + 8; k >= ccy - 1; k--)
        {
            bool boFind = false;
            for (int i = ActorList.Count - 1; i >= 0; i--)
            {
                var a = ActorList[i];
                if (ReferenceEquals(a, G.MySelf))
                    continue;
                if (!((!liveonly || !a.m_boDeath) && a.m_boHoldPlace && a.m_boVisible
                    && (a.m_HumsBBType != THumBBType.bbGamePet || !G.boPetNoEntity)))
                    continue;
                if (a.m_nCurrY != k)
                    continue;

                int nPx = a.m_nPx;
                int nPy = a.m_nPy;

                // 微端资源不完整时点不上（原文 6099-6111）
                if (a.m_btHorse > 0)
                {
                    if (a.HorseSurface == null)
                    {
                        nPx = 0;
                        nPy = 0;
                    }
                }
                else if (a.BodySurface == null)
                {
                    nPx = 0;
                    nPy = 0;
                }

                int dx = (a.m_nRx - Map.ClientLeft) * PlaySceneConsts.UNITX + m_nDefXX + nPx + a.m_nShiftX;
                int dy = (a.m_nRy - Map.ClientTop - 1) * PlaySceneConsts.UNITY + m_nDefYY + nPy + a.m_nShiftY;

                int chrW = a.CharWidth;
                int chrH = a.CharHeight;
                int centx;
                if (chrW > 40)
                    centx = (chrW - 40) / 2;
                else
                {
                    centx = 0;
                    if (chrW < 10 && !IsMoonMonDeadSpecial(a))
                    {
                        chrW = 60;
                        centx = (chrW - 40) / 2;
                    }
                }

                int centy;
                if (chrH > 70)
                    centy = (chrH - 70) / 2;
                else
                {
                    centy = 0;
                    if (chrH < 10 && !IsMoonMonDeadSpecial(a))
                    {
                        chrH = 80;
                        centy = (chrH - 70) / 2;
                    }
                }

                if (x - dx >= centx && x - dx <= chrW - centx && y - dy >= centy && y - dy <= chrH - centy)
                {
                    result = a;
                    nowsel++;
                    if (nowsel >= wantsel)
                    {
                        boFind = true;
                        break;
                    }
                }
            }
            if (boFind)
                break;
        }
        return result;
    }

    /// <summary>原文 6125/6137：月灵（race 56）死亡或残影时不放宽命中框。</summary>
    private static bool IsMoonMonDeadSpecial(TActorCore a)
        => a.m_btRace == 56 && (a.m_boDeath || a.m_boGhost);

    /// <summary>PlayScn.pas 6165-6182 IsSelectMyself 1:1（k=ccy+2..ccy-1）。</summary>
    public bool IsSelectMyself(int x, int y)
    {
        var self = G.MySelf;
        if (self == null)
            return false;
        int ccx = 0, ccy = 0;
        CXYfromMouseXY(x, y, ref ccx, ref ccy);
        for (int k = ccy + 2; k >= ccy - 1; k--)
        {
            if (self.m_nCurrY == k)
            {
                int dx = (self.m_nRx - Map.ClientLeft) * PlaySceneConsts.UNITX + m_nDefXX + self.m_nPx + self.m_nShiftX;
                int dy = (self.m_nRy - Map.ClientTop - 1) * PlaySceneConsts.UNITY + m_nDefYY + self.m_nPy + self.m_nShiftY;
                if (self.CheckSelect(x - dx, y - dy))
                    return true;
            }
        }
        return false;
    }

    // ================= 地面物品 =================

    /// <summary>PlayScn.pas 6187-6236 GetDropItems(X,Y,inames) 1:1（同格逐项命中，名串以 '\' 连接）。</summary>
    public DropItem? GetDropItems(int x, int y, out string inames)
    {
        DropItem? result = null;
        int ccx = 0, ccy = 0, ssx = 0, ssy = 0;
        CXYfromMouseXY(x, y, ref ccx, ref ccy);
        ScreenXYfromMCXY(ccx, ccy, ref ssx, ref ssy);
        inames = "";

        var list = DropItems?.GetPointList(ccx, ccy);
        if (list == null)
            return null;

        for (int ii = 0; ii < list.Items.Count; ii++)
        {
            var dropItem = list.Items[ii];
            var s = DropItemImageProbe?.Invoke(dropItem.Looks);
            if (s == null)
                continue;

            int nW = s.Value.Width;
            int nH = s.Value.Height;

            int dx1 = (x - ssx) + (nW / 2) - 3;
            int dy1 = (y - ssy) + (nH / 2);

            if (nW > 16) nW = 16;
            if (nH > 16) nH = 16;
            int dx2 = (x - ssx) + (nW / 2) - 3;
            int dy2 = (y - ssy) + (nH / 2);

            if ((dx2 >= -10 && dx2 <= 10 && dy2 >= -10 && dy2 <= 10)
                || (CheckTextureAlpha?.Invoke(s.Value, dx1, dy1) ?? false))
            {
                result ??= dropItem;
                inames += dropItem.Name + "\\";
            }
        }
        return result;
    }

    /// <summary>PlayScn.pas 6238-6285 GetDropItems(X,Y,HintList) 1:1（全点表包围盒 + 五像素 alpha）。</summary>
    public DropItem? GetDropItems(int x, int y, List<string> hintList)
    {
        DropItem? result = null;
        int ccx = 0, ccy = 0, ssx = 0, ssy = 0;
        CXYfromMouseXY(x, y, ref ccx, ref ccy);
        ScreenXYfromMCXY(ccx, ccy, ref ssx, ref ssy);

        if (DropItems == null)
            return null;

        foreach (var list in DropItems.Points)
        {
            foreach (var dropItem in list.Items)
            {
                if (!dropItem.Visible)
                    continue;

                int ix = 0, iy = 0;
                ScreenXYfromMCXY(dropItem.X, dropItem.Y, ref ix, ref iy);

                if (x >= ix - dropItem.TextureWidth / 2 && y >= iy - dropItem.TextureHeight / 2
                    && x <= ix + dropItem.TextureWidth / 2 && y <= iy + dropItem.TextureHeight / 2)
                {
                    var s = DropItemImageProbe?.Invoke(dropItem.Looks);
                    if (s == null)
                        continue;

                    int dx = x - (ix - dropItem.TextureWidth / 2);
                    int dy = y - (iy - dropItem.TextureHeight / 2);
                    var probe = CheckTextureAlpha;
                    bool hit = probe != null && (probe(s.Value, dx, dy) || probe(s.Value, dx + 1, dy)
                        || probe(s.Value, dx - 1, dy) || probe(s.Value, dx, dy + 1) || probe(s.Value, dx, dy - 1));
                    if (hit)
                    {
                        result ??= dropItem;
                        hintList.Add(dropItem.Name);
                    }
                }
            }
        }
        return result;
    }

    /// <summary>PlayScn.pas 6287-6308 GetXYDropItemsList 1:1（同点首个可见且坐标相等者加入后立即返回）。</summary>
    public void GetXYDropItemsList(int nx, int ny, List<DropItem> itemList)
    {
        var list = DropItems?.GetPointList(nx, ny);
        if (list == null)
            return;
        for (int ii = 0; ii < list.Items.Count; ii++)
        {
            var dropItem = list.Items[ii];
            if (dropItem.Visible && dropItem.X == nx && dropItem.Y == ny)
            {
                itemList.Add(dropItem);
                return;
            }
        }
    }

    /// <summary>PlayScn.pas 6310-6332 GetXYDropItems 1:1。</summary>
    public DropItem? GetXYDropItems(int nx, int ny)
    {
        var list = DropItems?.GetPointList(nx, ny);
        if (list == null)
            return null;
        for (int ii = 0; ii < list.Items.Count; ii++)
        {
            var dropItem = list.Items[ii];
            if (dropItem.Visible && dropItem.X == nx && dropItem.Y == ny)
                return dropItem;
        }
        return null;
    }

    // ================= 震动 =================

    /// <summary>PlayScn.pas 8273-8281 CheckSceneShake 1:1（延时到点 → 关标记并执行震动）。</summary>
    public void CheckSceneShake()
    {
        if (m_boDelaySceneShake)
        {
            if (TickDiff(m_dwDelaySceneShakeTick, SceneTime.TickNow()) >= m_dwDelaySceneShakeTime)
            {
                m_boDelaySceneShake = false;
                SceneShake(m_dwDelaySceneShakeCount, 0);
            }
        }
    }

    /// <summary>PlayScn.pas 8283-8306 SceneShake 1:1（DelayTime=0 立即八段偏移 × Count；否则登记延时）。</summary>
    public void SceneShake(int count, int delayTime)
    {
        if (delayTime == 0)
        {
            for (int i = 0; i < count; i++)
            {
                AddSceneShakeOffset(0, -10);
                AddSceneShakeOffset(0, 0);
                AddSceneShakeOffset(0, -8);
                AddSceneShakeOffset(0, 0);
                AddSceneShakeOffset(0, -6);
                AddSceneShakeOffset(0, 0);
                AddSceneShakeOffset(0, -4);
                AddSceneShakeOffset(0, 0);
            }
        }
        else
        {
            m_boDelaySceneShake = true;
            m_dwDelaySceneShakeTick = SceneTime.TickNow();
            m_dwDelaySceneShakeTime = (uint)delayTime;
            m_dwDelaySceneShakeCount = count;
        }
    }

    /// <summary>PlayScn.pas 8308-8314 AddSceneShakeOffset 1:1（MakeLong(nX,nY) 打包入表：X 低位、Y 高位）。</summary>
    public void AddSceneShakeOffset(int nx, int ny)
    {
        int packed = unchecked((int)((uint)(ushort)(short)nx | ((uint)(ushort)(short)ny << 16)));
        SceneShakeList.Add(packed);
    }

    /// <summary>MakeLong 解包：取低 16 位为 X、高 16 位为 Y（有符号）。</summary>
    public static (int X, int Y) UnpackShake(int packed)
        => ((short)(packed & 0xFFFF), (short)((packed >> 16) & 0xFFFF));

    /// <summary>tick_diff（HUtil32：处理回绕的无符号差）。</summary>
    public static uint TickDiff(uint start, uint now) => unchecked(now - start);
}

/// <summary>GetNextDirection/GetNextPosXY（MShare.pas/HUtil32 客户端八方向步进）。</summary>
public static class MapPath
{
    /// <summary>八方向离散方向（Delphi GetNextDirection 客户端版：先 X 后 Y 的符号组合）。</summary>
    public static int GetNextDirection(int sx, int sy, int ex, int ey)
    {
        const int DR_UP = 0, DR_UPRIGHT = 1, DR_RIGHT = 2, DR_DOWNRIGHT = 3;
        const int DR_DOWN = 4, DR_DOWNLEFT = 5, DR_LEFT = 6, DR_UPLEFT = 7;

        int dx = ex - sx;
        int dy = ey - sy;
        if (dx == 0 && dy == 0)
            return DR_DOWN;
        if (dx > 0)
        {
            if (dy > 0) return DR_DOWNRIGHT;
            if (dy < 0) return DR_UPRIGHT;
            return DR_RIGHT;
        }
        if (dx < 0)
        {
            if (dy > 0) return DR_DOWNLEFT;
            if (dy < 0) return DR_UPLEFT;
            return DR_LEFT;
        }
        return dy > 0 ? DR_DOWN : DR_UP;
    }

    private static readonly (int X, int Y)[] Deltas =
    {
        (0, -1), // DR_UP
        (1, -1), // DR_UPRIGHT
        (1, 0),  // DR_RIGHT
        (1, 1),  // DR_DOWNRIGHT
        (0, 1),  // DR_DOWN
        (-1, 1), // DR_DOWNLEFT
        (-1, 0), // DR_LEFT
        (-1, -1) // DR_UPLEFT
    };

    /// <summary>GetNextPosXY（GetNextDirection 方向走一格）。</summary>
    public static void GetNextPosXY(int dir, ref int x, ref int y)
    {
        if (dir < 0 || dir >= Deltas.Length)
            return;
        x += Deltas[dir].X;
        y += Deltas[dir].Y;
    }
}

/// <summary>默认地图（无地图数据：全不可走，换图标记 false）。</summary>
public sealed class DefaultPlayMap : IPlayMap
{
    public int BlockLeft => 0;
    public int BlockTop => 0;
    public int ClientLeft => 0;
    public int ClientTop => 0;
    public int ClientRight => 0;
    public int ClientBottom => 0;
    public bool MapMoving => false;
    public bool CanMove(int mx, int my) => false;
    public bool NewCanMove(int mx, int my) => false;
    public bool CanFly(int mx, int my) => false;

    public string OldMapName { get; set; } = "";
    public bool MiniMapVisible => false;
    public bool CheckDMinMapBigDlgVisible => false;
    public bool IsDGJPointsShow => false;
    public void LoadMap(string mapName, int x, int y) { }
}

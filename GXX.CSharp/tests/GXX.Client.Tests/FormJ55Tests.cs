using System;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>小地图渲染纯逻辑（批次J55）：图库选择/坐标换算/雷达/闪烁/描述定位/消息状态机。</summary>
public sealed class MiniMapRenderTests : IDisposable
{
    public MiniMapRenderTests() => MiniMapEnv.Reset();

    public void Dispose() => MiniMapEnv.Reset();

    [Fact]
    public void SelectImage_FiveSegments()
    {
        Assert.Equal((MiniMapLibKind.WBmpMap, 5), MiniMapRender.SelectImage(9905));
        Assert.Equal((MiniMapLibKind.MMap, 3), MiniMapRender.SelectImage(3)); // 缺省段
        Assert.Equal((MiniMapLibKind.MMap10, 0), MiniMapRender.SelectImage(10000)); // mmap10 从 10001 帧起
        Assert.Equal((MiniMapLibKind.MMap11, 12), MiniMapRender.SelectImage(15012));
        Assert.Equal((MiniMapLibKind.MMap12, 24999 - 20000), MiniMapRender.SelectImage(24999));
    }

    [Fact]
    public void CoordinateConversions_DelphiDiv()
    {
        Assert.Equal((48, 32), MiniMapRender.ActorXYToMapXY(32, 32));   // ×48/32, ×32/32
        Assert.Equal((24, 16), MiniMapRender.ActorXYToMapXY(16, 16));   // 16×48/32=24
        Assert.Equal((16, 16), MiniMapRender.MapXYToActorXY(24, 16));   // 24×32/48=16
        Assert.Equal((31, 47), MiniMapRender.MapXYToActorXY(47, 47));   // 47×32=1504/48=31（整除截断）
    }

    [Fact]
    public void MapToScreen_BankerRounding()
    {
        Assert.Equal(64, MiniMapRender.MapToScreen(100, 128, 50));   // 128×50/100=64.0
        Assert.Equal(64, MiniMapRender.MapToScreen(100, 129, 50));   // 64.5 → 64（银行家）
        Assert.Equal(0, MiniMapRender.ScreenToMap(100, 129, 0));     // 反向
        Assert.Equal(47, MiniMapRender.ScreenToMap(100, 129, 61));   // 100×61/129=47.28…→47
    }

    [Fact]
    public void RadarScanBounds_PlusMinus12()
    {
        Assert.Equal((88, 112, 188, 212), MiniMapRender.RadarScanBounds(100, 200));
    }

    private static RadarActor Actor(int race, int realRace, bool isNpc = false, bool isHum = false, bool isHero = false,
        bool isMyHero = false, string name = "")
        => new() { Race = race, RealRace = realRace, IsNpcActor = isNpc, IsHumActor = isHum, IsHeroActor = isHero, IsMyHero = isMyHero, UserName = name };

    [Fact]
    public void Classify_NpcPlayerHeroPrecedence()
    {
        Assert.Equal(RadarKind.Npc, MiniMapRender.Classify(Actor(0, 0, isNpc: true), _ => false)); // TNpcActor 最优先
        Assert.Equal(RadarKind.OtherPlayer, MiniMapRender.Classify(Actor(0, 0, isHum: true), _ => false));
        Assert.Equal(RadarKind.Hero, MiniMapRender.Classify(Actor(1, 1, isHero: true, isMyHero: true), _ => false));
        Assert.Equal(RadarKind.OtherPlayer, MiniMapRender.Classify(Actor(1, 1, isHero: true), _ => false)); // 他人英雄
    }

    [Fact]
    public void Classify_RealRaceTable()
    {
        Assert.Equal(RadarKind.OtherPlayer, MiniMapRender.Classify(Actor(0, 0), _ => false));           // RC_PLAYOBJECT
        Assert.Equal(RadarKind.Npc, MiniMapRender.Classify(Actor(50, 50), _ => false));                 // RC_ANIMAL 和平NPC
        Assert.Equal(RadarKind.Npc, MiniMapRender.Classify(Actor(15, 15), _ => false));                 // RC_PEACENPC
        Assert.Equal(RadarKind.Npc, MiniMapRender.Classify(Actor(55, 55), _ => false));                 // 练功师
        Assert.Equal(RadarKind.Guard, MiniMapRender.Classify(Actor(11, 11), _ => false));               // RC_GUARD
        Assert.Equal(RadarKind.Guard, MiniMapRender.Classify(Actor(112, 112), _ => false));             // RC_ARCHERGUARD
        Assert.Equal(RadarKind.Boss, MiniMapRender.Classify(Actor(80, 80, name: "祖玛教主"), n => n == "祖玛教主"));
        Assert.Equal(RadarKind.Monster, MiniMapRender.Classify(Actor(80, 80, name: "鸡"), _ => false));
    }

    [Fact]
    public void Classify_MyHeroOverride()
    {
        // 系统NPC 分支命中但 g_MyHero 覆写为 Hero（Delphi 末尾 if 顺序）
        Assert.Equal(RadarKind.Hero, MiniMapRender.Classify(Actor(0, 0, isNpc: true, isMyHero: true), _ => false));
    }

    [Fact]
    public void RadarGate_FourSwitches()
    {
        var player = Actor(0, 0);
        var peaceNpc = Actor(15, 15);
        var animal = Actor(50, 50);
        var trainer = Actor(55, 55);
        var guard = Actor(11, 11);
        var monster = Actor(80, 80);

        // 全关：只有怪物通过 showActor
        Assert.False(MiniMapRender.RadarGate(player, false, false, false, false, false));
        Assert.True(MiniMapRender.RadarGate(monster, false, false, false, true, false));
        // 各开关
        Assert.True(MiniMapRender.RadarGate(player, showPlayer: true, false, false, false, false));
        Assert.True(MiniMapRender.RadarGate(animal, false, showNpc: true, false, false, false));
        Assert.True(MiniMapRender.RadarGate(trainer, false, showNpc: true, false, false, false));
        Assert.True(MiniMapRender.RadarGate(peaceNpc, false, false, showAttackNpc: true, false, false));
        Assert.True(MiniMapRender.RadarGate(guard, false, false, showAttackNpc: true, false, false)); // RealRace 11
        // TMirConfigDlg 恒开
        Assert.True(MiniMapRender.RadarGate(player, false, false, false, false, isMirConfigDlg: true));
        Assert.True(MiniMapRender.RadarGate(guard, false, false, false, false, isMirConfigDlg: true));
    }

    [Fact]
    public void RadarColor_PaletteSeam()
    {
        MiniMapEnv.GetRgbFn = c => c * 3;
        Assert.Equal(30, MiniMapRender.RadarColor(RadarKind.Npc, 10, 0, 0, 0, 0, 0));
        Assert.Equal(33, MiniMapRender.RadarColor(RadarKind.Boss, 0, 0, 0, 0, 11, 0));
        Assert.Equal(36, MiniMapRender.RadarColor(RadarKind.Guard, 0, 0, 0, 12, 0, 0));
    }

    [Fact]
    public void BigSelfRadar_FlashToggle()
    {
        bool isShow = false;
        uint lastTick = 0;
        // <200 恒显且不动时钟
        Assert.True(MiniMapRender.BigSelfRadarVisible(1000, 150, ref isShow, ref lastTick));
        Assert.True(isShow);
        Assert.Equal(0u, lastTick);

        // ≥200：间隔未到保持
        isShow = true;
        Assert.True(MiniMapRender.BigSelfRadarVisible(400, 500, ref isShow, ref lastTick));
        Assert.True(isShow);
        // 到间隔翻转并记时钟
        Assert.False(MiniMapRender.BigSelfRadarVisible(500, 500, ref isShow, ref lastTick));
        Assert.Equal(500u, lastTick);
        // 回绕（tick_diff）：lastTick=最大值附近（flash 须 ≥200 才走翻转分支）
        isShow = true;
        lastTick = uint.MaxValue - 10;
        Assert.True(MiniMapRender.BigSelfRadarVisible(20, 250, ref isShow, ref lastTick)); // tick_diff=30 < 250 → 保持
        Assert.True(isShow);
        Assert.False(MiniMapRender.BigSelfRadarVisible(250, 250, ref isShow, ref lastTick)); // tick_diff=260 ≥ 250 → 翻转
    }

    [Fact]
    public void ExSelfRadar_StickyTrueUnder200()
    {
        bool drawSelf = false;
        uint lastUpdate = 0;
        Assert.True(MiniMapRender.ExSelfRadarVisible(100, 150, ref drawSelf, ref lastUpdate));
        Assert.True(drawSelf);
        Assert.Equal(0u, lastUpdate);

        drawSelf = false;
        Assert.False(MiniMapRender.ExSelfRadarVisible(100, 500, ref drawSelf, ref lastUpdate));
        Assert.True(MiniMapRender.ExSelfRadarVisible(600, 500, ref drawSelf, ref lastUpdate)); // 翻转
        Assert.Equal(600u, lastUpdate);
        Assert.True(MiniMapRender.ExSelfRadarVisible(700, 500, ref drawSelf, ref lastUpdate)); // 未到间隔 → 600 翻转后的 True 粘滞
    }

    [Fact]
    public void ExViewport_ClampedOrigin()
    {
        Assert.Equal((34, 24, 100, 80), MiniMapRender.ExViewport(84, 64, 100, 80)); // 84−50, 64−40
        Assert.Equal((0, 0, 100, 80), MiniMapRender.ExViewport(10, 5, 100, 80));    // 负值钳 0
    }

    [Fact]
    public void DescTextPos_ClampedInside()
    {
        // 居中不越界
        Assert.Equal((100 + 50 - 20, 100 + 40), MiniMapRender.DescTextPos(100, 100, 400, 400, 50, 40, 40, 12));
        // 左越界钳 vtLeft；右越界收缩
        Assert.Equal((100, 100), MiniMapRender.DescTextPos(100, 100, 400, 400, 0, 0, 200, 12));
        Assert.Equal((400 - 200, 100), MiniMapRender.DescTextPos(100, 100, 400, 400, 380, 0, 200, 12));
        // 下越界收缩
        Assert.Equal((100, 400 - 12), MiniMapRender.DescTextPos(100, 100, 400, 400, 0, 395, 40, 12));
    }

    [Fact]
    public void BigMapPointToActor_RoundTrip()
    {
        // 大图坐标 → 地图坐标 → 角色坐标（×32/48 反推）
        var (ax, ay) = MiniMapRender.BigMapPointToActor(bigX: 116, bigY: 58, left: 16, top: 26, texW: 200, texH: 100, screenW: 400, screenH: 200);
        Assert.Equal(33, ax); // (116−16)×200/400=50 → 50×32/48=33（整除截断）
        Assert.Equal(16, ay); // (58−26)×100/200=16 → 16×32/32=16
    }

    [Fact]
    public void ReadMiniMapOk_GjMapBranch()
    {
        var st = new MiniMapRender.MiniMapMessageState();
        var r = MiniMapRender.HandleReadMiniMapOk(st, param: 5, recog: 1);
        Assert.Equal(4, st.MiniMapIndex); // param − 1
        Assert.True(r.ShowDgjPoints);
        Assert.False(r.LevelChangeNotified);
        // param=0 不处理
        var r0 = MiniMapRender.HandleReadMiniMapOk(st, param: 0, recog: 1);
        Assert.False(r0.ShowDgjPoints);
        Assert.False(r0.LevelChangeNotified);
    }

    [Fact]
    public void ReadMiniMapOk_NormalBranch()
    {
        var st = new MiniMapRender.MiniMapMessageState { ViewMinMapLv = -3 };
        var r = MiniMapRender.HandleReadMiniMapOk(st, param: 3, recog: 0);
        Assert.Equal(2, st.MiniMapIndex);
        Assert.True(st.ViewMiniMap);
        Assert.Equal(0, st.ViewMinMapLv); // 负值归 0
        Assert.True(r.LevelChangeNotified);

        // index<0 → 关闭视图
        var st2 = new MiniMapRender.MiniMapMessageState();
        MiniMapRender.HandleReadMiniMapOk(st2, param: 0, recog: 0); // 不处理
        var r3 = MiniMapRender.HandleReadMiniMapOk(st2, param: 1, recog: 0);
        Assert.Equal(0, st2.MiniMapIndex);
        Assert.True(st2.ViewMiniMap);

        var st3 = new MiniMapRender.MiniMapMessageState();
        MiniMapRender.HandleReadMiniMapOk(st3, param: -0, recog: 0); // param=0 → 不处理，保持 -1
        Assert.Equal(-1, st3.MiniMapIndex);
        Assert.False(st3.ViewMiniMap);
    }

    [Fact]
    public void ReadMiniMapFail_ResetsState()
    {
        var st = new MiniMapRender.MiniMapMessageState { MiniMapIndex = 7 };
        var r = MiniMapRender.HandleReadMiniMapFail(st, now: 12345);
        Assert.Equal(-1, st.MiniMapIndex);
        Assert.Equal(12345u, st.QueryMsgTick);
        Assert.True(r.LevelChangeNotified);
        Assert.True(r.ChatBoardMessage); // '没有可用的地图'
    }
}

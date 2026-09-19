using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>PlayScn.pas TPlayScene 第一批（批次J72）：角色表/坐标换算/碰撞族/选角/地面物品/NewActor。</summary>
public sealed class PlaySceneCoreTests : IDisposable
{
    // ---------------- 测试替身 ----------------

    private sealed class FakeMap : IPlayMap
    {
        public HashSet<(int, int)> Movable = new();
        public HashSet<(int, int)> NewMovable = new();
        public HashSet<(int, int)> Flyable = new();
        public bool Moving;
        public int BlockLeft, BlockTop, ClientLeft, ClientTop, ClientRight, ClientBottom;

        int IPlayMap.BlockLeft => BlockLeft;
        int IPlayMap.BlockTop => BlockTop;
        int IPlayMap.ClientLeft => ClientLeft;
        int IPlayMap.ClientTop => ClientTop;
        int IPlayMap.ClientRight => ClientRight;
        int IPlayMap.ClientBottom => ClientBottom;
        bool IPlayMap.MapMoving => Moving;

        public bool CanMove(int mx, int my) => Movable.Contains((mx, my));
        public bool NewCanMove(int mx, int my) => NewMovable.Contains((mx, my));
        public bool CanFly(int mx, int my) => Flyable.Contains((mx, my));

        // ---- 批次J73：SendMsg 换图族所需成员 ----
        public string LastLoadedMap = "";
        public string OldMapName { get; set; } = "";
        public bool MiniMapVisible { get; set; }
        public bool CheckDMinMapBigDlgVisible { get; set; }
        public bool IsDGJPointsShow { get; set; }
        public void LoadMap(string mapName, int x, int y) => LastLoadedMap = mapName;
    }

    private static TActorCore MakeActor(long id, int x, int y, string name = "", int race = 50)
    {
        var a = new TActor
        {
            m_nRecogId = id,
            m_nCurrX = x,
            m_nCurrY = y,
            m_nRx = x,
            m_nRy = y,
            m_sUserName = name,
        };
        a.m_btRace = (byte)race;
        return a;
    }

    private static uint _tick = 5000;

    public PlaySceneCoreTests()
    {
        _tick = 5000;
        SceneTime.TickNow = () => _tick;
        TPlayScene.ShakeX = 0;
        TPlayScene.ShakeY = 0;
    }

    public void Dispose()
    {
        SceneTime.TickNow = () => (uint)Environment.TickCount;
    }

    private static (TPlayScene scene, FakeMap map, SceneDialogs dlg) MakeScene()
    {
        var dlg = new SceneDialogs();
        var map = new FakeMap();
        var scene = new TPlayScene(dlg, map);
        return (scene, map, dlg);
    }

    // ---------------- 角色表 ----------------

    [Fact]
    public void DoAddActor_InsertsByRecogIdAndAppendsDrawList()
    {
        var (scene, _, _) = MakeScene();
        var c = MakeActor(300, 1, 1);
        var a = MakeActor(100, 1, 1);
        var b = MakeActor(200, 1, 1);

        scene.DoAddActor(c);
        scene.DoAddActor(a);
        scene.DoAddActor(b);

        Assert.Equal(new long[] { 100, 200, 300 }, scene.ActorList.ConvertAll(x => x.m_nRecogId));
        // DrawActorList 为尾附顺序（原文 m_DrawActorList.Add）
        Assert.Equal(new long[] { 300, 100, 200 }, scene.DrawActorList.ConvertAll(x => x.m_nRecogId));

        // 重复加入：DoSearchActor 命中 → 两表均不改变（原文 8320-8323）
        scene.DoAddActor(a);
        Assert.Equal(3, scene.ActorList.Count);
        Assert.Equal(3, scene.DrawActorList.Count);
    }

    [Fact]
    public void DoSearchActor_ReportsHitAndInsertIndex()
    {
        var (scene, _, _) = MakeScene();
        foreach (var id in new long[] { 10, 20, 30 })
            scene.DoAddActor(MakeActor(id, 0, 0));

        Assert.True(scene.DoSearchActor(20, out int i20));
        Assert.Equal(1, i20);

        Assert.False(scene.DoSearchActor(25, out int i25));
        Assert.Equal(2, i25);

        Assert.False(scene.DoSearchActor(5, out int i5));
        Assert.Equal(0, i5);

        Assert.False(scene.DoSearchActor(35, out int i35));
        Assert.Equal(3, i35);
    }

    [Fact]
    public void DoDelActor_RemovesBothListsAndReportsHit()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(1, 0, 0);
        var b = MakeActor(2, 0, 0);
        scene.DoAddActor(a);
        scene.DoAddActor(b);

        Assert.True(scene.DoDelActor(a));
        Assert.Single(scene.ActorList);
        Assert.Single(scene.DrawActorList);

        // 再次删除：ActorList 未命中 → false；DrawActorList 已无 → 仍 Remove 无副作用
        Assert.False(scene.DoDelActor(a));
        Assert.Single(scene.DrawActorList);
    }

    [Fact]
    public void FindActor_ByDeleteFlagNameAndXY()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(7, 10, 20, "Hero");
        var b = MakeActor(8, 11, 20, "Other");
        scene.DoAddActor(a);
        scene.DoAddActor(b);

        Assert.Same(a, scene.FindActor(7L));
        Assert.Same(a, scene.FindActorList(7L));
        Assert.Same(a, scene.FindActor("hero"));          // CompareText 忽略大小写
        Assert.Null(scene.FindActor("nobody"));

        // FindActorXY：首个匹配且非死亡/可见/占位 → 立即命中
        Assert.Same(a, scene.FindActorXY(10, 20));
        Assert.Null(scene.FindActorXY(10, 21));

        // FindActorXY(x,y,Actor)：未通过可见性判定 → null
        Assert.Same(a, scene.FindActorXY(10, 20, a));
        a.m_boVisible = false;
        Assert.Null(scene.FindActorXY(10, 20, a));

        // FindActorXY(X,Y)：命中不可见时 result 保留但继续扫描
        var c = MakeActor(9, 10, 20, "c");
        scene.DoAddActor(c);
        Assert.Same(c, scene.FindActorXY(10, 20));

        // FindActor(id) 看删除标记
        a.m_boVisible = true;
        a.m_boDelActor = true;
        Assert.Null(scene.FindActor(7L));
        Assert.Same(a, scene.FindActorList(7L));  // FindActorList 不看标记
    }

    [Fact]
    public void IsValidActor_ChecksMemberAndDeleteFlag()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(1, 0, 0);
        var outsider = MakeActor(2, 0, 0);
        scene.DoAddActor(a);

        Assert.True(scene.IsValidActor(a));
        Assert.False(scene.IsValidActor(outsider));
        Assert.True(scene.IsValidActorEx(a));   // DrawActorList 内

        a.m_boDelActor = true;
        Assert.False(scene.IsValidActor(a));
        Assert.False(scene.IsValidActorEx(a));
    }

    [Fact]
    public void ButchAnimal_PrefersExactThenAdjacent()
    {
        var (scene, _, _) = MakeScene();
        var far = MakeActor(1, 5, 5);
        far.m_boDeath = true;
        var human = MakeActor(2, 10, 10);
        human.m_boDeath = true;
        human.m_btRace = 1;                       // 人物（race 1）不参与屠宰
        scene.DoAddActor(far);
        scene.DoAddActor(human);

        // 无同点 → null（人物被排除）
        Assert.Null(scene.ButchAnimal(10, 10));

        var near = MakeActor(3, 20, 20);
        near.m_boDeath = true;
        scene.DoAddActor(near);
        Assert.Same(near, scene.ButchAnimal(21, 21));   // ±1 命中
        Assert.Null(scene.ButchAnimal(22, 22));

        var exact = MakeActor(4, 40, 40);
        exact.m_boDeath = true;
        var adjacent = MakeActor(5, 41, 41);
        adjacent.m_boDeath = true;
        scene.DoAddActor(exact);
        scene.DoAddActor(adjacent);
        Assert.Same(exact, scene.ButchAnimal(40, 40));  // 精确优先
    }

    // ---------------- 坐标换算 ----------------

    [Fact]
    public void ScreenXYfromMCXY_MatchesDelphiFormula()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 100, 100);
        self.m_nRx = 100;
        self.m_nRy = 100;
        self.m_nShiftX = 7;
        self.m_nShiftY = 9;
        scene.G.MySelf = self;
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;

        int sx = 0, sy = 0;
        scene.ScreenXYfromMCXY(103, 98, ref sx, ref sy);
        Assert.Equal(512 + 3 * 48 - 7, sx);
        Assert.Equal(384 - 2 * 32 - 9, sy);

        // 同点：nWidth/nHeight=0 → 落在中心（无 shift 时为屏幕中心）
        self.m_nShiftX = 0;
        self.m_nShiftY = 0;
        scene.ScreenXYfromMCXY(100, 100, ref sx, ref sy);
        Assert.Equal(512, sx);
        Assert.Equal(384, sy);
    }

    [Fact]
    public void ScreenXYfromMCXY_NoSelfKeepsOutParams()
    {
        var (scene, _, _) = MakeScene();
        int sx = 77, sy = 88;
        scene.ScreenXYfromMCXY(1, 1, ref sx, ref sy);   // g_MySelf = nil → Exit 不改出参
        Assert.Equal(77, sx);
        Assert.Equal(88, sy);
    }

    [Fact]
    public void CXYfromMouseXY_TruncatesAndRoundsUpPastHalf()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        scene.G.MySelf = self;
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;

        // 中心右侧 24px：|512-536| = 24, 24 div 48 = 0；余 24 > 24 为假 → ccx = 10 + 0
        int ccx = 0, ccy = 0;
        scene.CXYfromMouseXY(536, 384, ref ccx, ref ccy);
        Assert.Equal(10, ccx);
        Assert.Equal(10, ccy);

        // 中心右侧 25px：余数 25 > 24 → 进位 → ccx = 11
        scene.CXYfromMouseXY(537, 384, ref ccx, ref ccy);
        Assert.Equal(11, ccx);

        // 中心左侧 32px：32 div 48 = 0；余 32 > 24 → 进位 → ccx = 10 - 1
        scene.CXYfromMouseXY(480, 384, ref ccx, ref ccy);
        Assert.Equal(9, ccx);

        // 中心上方 96px：96 div 32 = 3；余 0 → ccy = 10 - 3
        scene.CXYfromMouseXY(512, 288, ref ccx, ref ccy);
        Assert.Equal(7, ccy);

        // shift 参与：shiftY = 88 → |288-384+88| = 8 div 32 = 0 → ccy = 10（原文加号方向）
        self.m_nShiftY = 88;
        scene.CXYfromMouseXY(512, 288, ref ccx, ref ccy);
        Assert.Equal(10, ccy);
        self.m_nShiftY = 0;
    }

    [Fact]
    public void CXYfromMouseXY_NoSelfKeepsOutParams()
    {
        var (scene, _, _) = MakeScene();
        int ccx = 5, ccy = 6;
        scene.CXYfromMouseXY(1, 1, ref ccx, ref ccy);
        Assert.Equal(5, ccx);
        Assert.Equal(6, ccy);
    }

    // ---------------- 碰撞族 ----------------

    [Fact]
    public void CanWalk_FollowsMapAndCrashMan()
    {
        var (scene, map, _) = MakeScene();
        map.Movable.Add((5, 5));
        map.Movable.Add((6, 5));
        Assert.True(scene.CanWalk(5, 5));
        Assert.False(scene.CanWalk(7, 7));   // 地图不可走

        var blocker = MakeActor(1, 6, 5);
        scene.DoAddActor(blocker);
        Assert.False(scene.CanWalk(6, 5));   // 占位对象阻挡

        // 死亡 / 不可见 / 不占位 → 不阻挡
        blocker.m_boDeath = true;
        Assert.True(scene.CanWalk(6, 5));
        blocker.m_boDeath = false;
        blocker.m_boHoldPlace = false;
        Assert.True(scene.CanWalk(6, 5));
    }

    [Fact]
    public void CanWalkEx_TwelveGridGateAndPetNoEntity()
    {
        var (scene, map, _) = MakeScene();
        map.Movable.Add((0, 0));
        map.Movable.Add((13, 0));
        var self = MakeActor(1, 0, 0);
        scene.G.MySelf = self;
        var blocker = MakeActor(2, 13, 0);
        scene.DoAddActor(blocker);

        // 距离 13 > 12 → 短路返回 false（不看角色）
        Assert.False(scene.CrashManEx(13, 0));
        // 距离 12 → 进入扫描
        map.Movable.Add((12, 0));
        var near = MakeActor(3, 12, 0);
        scene.DoAddActor(near);
        Assert.True(scene.CrashManEx(12, 0));

        // 宠物无实体 → 不阻挡
        near.m_HumsBBType = THumBBType.bbGamePet;
        scene.G.boPetNoEntity = true;
        Assert.False(scene.CrashManEx(12, 0));
        scene.G.boPetNoEntity = false;

        // 骑马 1/2 不阻挡
        near.m_btHorse = 1;
        Assert.False(scene.CrashManEx(12, 0));
        near.m_btHorse = 0;

        // 穿怪开关：race > 1 且非 NPC/守卫
        near.m_btRace = 80;
        scene.G.boCanRunMon = true;
        Assert.False(scene.CrashManEx(12, 0));
        scene.G.boCanRunMon = false;
        Assert.True(scene.CrashManEx(12, 0));

        // 穿守卫开关
        near.m_btRace = 80;
        near.m_btRealRace = PlaySceneConsts.RC_ARCHERGUARD;
        scene.G.boCanRunGuard = true;
        Assert.False(scene.CrashManEx(12, 0));

        // 无 g_MySelf → 恒 false
        scene.G.MySelf = null;
        Assert.False(scene.CrashManEx(0, 0));
    }

    [Fact]
    public void CrashMan_SkipsNpcByAppearanceOnly()
    {
        var (scene, map, _) = MakeScene();
        map.Movable.Add((3, 3));
        var npc = MakeActor(1, 3, 3);
        npc.m_btRace = PlaySceneConsts.RC_MERCHANT;
        npc.m_wAppearance = 55;               // 54..58 → 传送门可穿
        scene.DoAddActor(npc);
        Assert.True(scene.CanWalk(3, 3));

        // 其它外观：CrashMan 不认 boCanRunNpc（原文注释掉）
        npc.m_wAppearance = 100;
        scene.G.boCanRunNpc = true;
        Assert.False(scene.CanWalk(3, 3));

        // UnLockCrashMan 认开关
        Assert.True(scene.UnLockCanWalk(3, 3));
    }

    [Fact]
    public void CrashManEx_NpcUsesSwitchOrAppearance()
    {
        var (scene, map, _) = MakeScene();
        map.Movable.Add((2, 2));
        var self = MakeActor(1, 0, 0);
        scene.G.MySelf = self;
        var npc = MakeActor(2, 2, 2);
        npc.m_btRace = PlaySceneConsts.RC_MERCHANT;
        npc.m_wAppearance = 96;               // 94..98 外观
        scene.DoAddActor(npc);
        Assert.True(scene.CanWalkEx(2, 2));
        Assert.True(scene.UnLockCanWalkEx(2, 2));

        npc.m_wAppearance = 30;
        Assert.False(scene.CanWalkEx(2, 2));
        scene.G.boCanRunNpc = true;
        Assert.True(scene.CanWalkEx(2, 2));

        // 人物（race 0/1）且非人型怪 + boCanRunHuman
        npc.m_btRace = 0;
        scene.G.boCanRunNpc = false;
        Assert.False(scene.CanWalkEx(2, 2));
        scene.G.boCanRunHuman = true;
        Assert.True(scene.CanWalkEx(2, 2));

        // 人型怪（boPlayMoster）不享人物豁免
        npc.m_boPlayMoster = true;
        Assert.False(scene.CanWalkEx(2, 2));
    }

    [Fact]
    public void CanRun_UnLockCanRunAndNewCanRun()
    {
        var (scene, map, _) = MakeScene();
        var self = MakeActor(1, 0, 0);
        scene.G.MySelf = self;
        foreach (var p in new[] { (1, 0), (2, 0), (3, 0), (4, 0), (5, 0) })
        {
            map.Movable.Add(p);
            map.NewMovable.Add(p);
        }

        Assert.True(scene.CanRun(0, 0, 2, 0));
        Assert.True(scene.NewCanRun(0, 0, 2, 0));
        Assert.True(scene.NewCanWalkEx(1, 0));
        Assert.True(scene.NewCanWalkEx_2(1, 0));

        // 骑马一步三格：以终点 (3,0) 观察追加判定（起点 (0,0) → 首格 (1,0)）。
        // 原文两处追加 GetNextPosXY 均由「首格」再推一格 → (2,0)；故追加格恒为 (2,0)。
        self.m_btHorse = 1;
        scene.G.boHorseRun3Grid = true;
        Assert.True(scene.CanRun(0, 0, 3, 0));
        Assert.True(scene.UnLockCanRun(0, 0, 3, 0));

        // 移除 (3,0)：CanRun 追加格由 (2,0) 再推一格 → (3,0) 不可走 → 假；UnLockCanRun 同
        map.Movable.Remove((3, 0));
        Assert.False(scene.CanRun(0, 0, 3, 0));
        Assert.False(scene.UnLockCanRun(0, 0, 3, 0));

        // 关闭马三格开关 → CanRun 仅看 (1,0)/(3,0) → 终点不可走 → 假；UnLock 同
        scene.G.boHorseRun3Grid = false;
        Assert.False(scene.CanRun(0, 0, 3, 0));
        Assert.False(scene.UnLockCanRun(0, 0, 3, 0));

        // 终点不可走（(2,0) 移除）→ 两路皆假
        map.Movable.Remove((2, 0));
        map.NewMovable.Remove((2, 0));
        Assert.False(scene.CanRun(0, 0, 2, 0));
        Assert.False(scene.UnLockCanRun(0, 0, 2, 0));
        Assert.False(scene.NewCanRun(0, 0, 2, 0));
    }

    [Fact]
    public void CanHorseRun_ThreeStageEarlyExit()
    {
        var (scene, map, _) = MakeScene();
        foreach (var p in new[] { (1, 0), (2, 0), (3, 0), (4, 0), (5, 0) })
            map.Movable.Add(p);

        // 起点 (0,0) 不在可走格内：首格 (1,0) 可走 → 追加格 (2,0) 可走 → 终点 (3,0) 可走 → 真
        Assert.True(scene.CanHorseRun(0, 0, 3, 0));

        // 追加格 (2,0) 不可走 → 假
        map.Movable.Remove((2, 0));
        Assert.False(scene.CanHorseRun(0, 0, 3, 0));

        // 恢复追加格，终点 (3,0) 不可走 → 假
        map.Movable.Add((2, 0));
        map.Movable.Remove((3, 0));
        Assert.False(scene.CanHorseRun(0, 0, 3, 0));

        // 起点 (1,0) 可走、终点 (3,0) 可走、追加格 (2,0) 可走 → 真
        map.Movable.Add((3, 0));
        Assert.True(scene.CanHorseRun(1, 0, 3, 0));
    }

    [Fact]
    public void CanFly_MapCanMoveAndMapCanFlyDelegates()
    {
        var (scene, map, _) = MakeScene();
        map.Flyable.Add((8, 8));
        map.Movable.Add((9, 9));
        Assert.True(scene.CanFly(8, 8));
        Assert.False(scene.CanFly(9, 9));
        Assert.True(scene.MapCanMove(9, 9));
        Assert.False(scene.MapCanFly(9, 9));
    }

    // ---------------- 鼠标选角 ----------------

    [Fact]
    public void GetCharacter_ScansByRowAndAlphaHit()
    {
        var (scene, _, _) = MakeScene();
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        scene.G.MySelf = self;

        var target = MakeActor(2, 12, 10);
        target.m_nRx = 12;
        target.m_nRy = 10;
        target.BodySurface = new SurfaceSize(60, 80);
        target.m_nPx = 0;
        target.m_nPy = 0;
        target.CheckTextureAlpha = (_, _, _) => true;
        scene.DoAddActor(target);

        // 目标屏幕位置：dx=(12-0)*48+0+0+0=608, dy=(10-0-1)*32+0=288
        // 反查 CXYfromMouseXY(608,288) = (12,10) → k 域 9..16 覆盖 m_nCurrY=10 → 命中
        int nowsel;
        var hit = scene.GetCharacter(608, 288, 0, out nowsel, liveonly: true);
        Assert.Same(target, hit);
        Assert.Equal(0, nowsel);

        // wantSel=1 → 需要第二个命中（只有一个 → null 且 nowsel 停在 0）
        var none = scene.GetCharacter(608, 288, 1, out nowsel, liveonly: true);
        Assert.Null(none);
        Assert.Equal(0, nowsel);

        // liveonly 且目标死亡 → 不命中
        target.m_boDeath = true;
        Assert.Null(scene.GetCharacter(608, 288, 0, out nowsel, liveonly: true));
        // liveonly=false → 命中
        Assert.Same(target, scene.GetCharacter(608, 288, 0, out nowsel, liveonly: false));

        // 自身被排除（g_MySelf 不进扫描）
        target.m_boDeath = false;
        Assert.Same(target, scene.GetCharacter(608, 288, 0, out nowsel, liveonly: true));
    }

    [Fact]
    public void GetAttackFocusCharacter_FallsBackToBoxHit()
    {
        var (scene, _, _) = MakeScene();
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        scene.G.MySelf = self;

        var target = MakeActor(2, 12, 10);
        target.m_nRx = 12;
        target.m_nRy = 10;
        target.BodySurface = new SurfaceSize(30, 40);   // 宽 30 < 40 → centx=0；高 40 < 70 → centy=0
        target.CheckTextureAlpha = (_, _, _) => false;  // alpha 恒不命中 → 走包围盒回退
        scene.DoAddActor(target);

        // 屏幕 dx=576; dy=288；命中框 x-dx ∈ [0,30]、y-dy ∈ [0,40]
        int nowsel;
        var hit = scene.GetAttackFocusCharacter(576 + 15, 288 + 20, 0, out nowsel, liveonly: true);
        Assert.Same(target, hit);

        // 框外
        Assert.Null(scene.GetAttackFocusCharacter(576 + 100, 288 + 20, 0, out nowsel, liveonly: true));

        // ChrW < 10 放宽到 60：宽 8 → chrW=60, centx=(60-40)/2=10 → x-dx ∈ [10,50]
        target.BodySurface = new SurfaceSize(8, 40);
        Assert.Same(target, scene.GetAttackFocusCharacter(576 + 30, 288 + 20, 0, out nowsel, liveonly: true));
        Assert.Null(scene.GetAttackFocusCharacter(576 + 5, 288 + 20, 0, out nowsel, liveonly: true));

        // 月灵（race 56）死亡 → 不放宽
        target.m_btRace = 56;
        target.m_boDeath = true;
        Assert.Null(scene.GetAttackFocusCharacter(576 + 30, 288 + 20, 0, out nowsel, liveonly: false));
    }

    [Fact]
    public void IsSelectMyself_UsesCcyPlusTwoRange()
    {
        var (scene, _, _) = MakeScene();
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        self.BodySurface = new SurfaceSize(60, 80);
        self.CheckTextureAlpha = (_, _, _) => true;
        scene.G.MySelf = self;

        // 鼠标 (608,384) → ccx=12, ccy=10 → k 域 9..12 覆盖自身 m_nCurrY=10
        // 自身屏幕位置 dx=(10-0)*48=480, dy=(10-1)*32=288 → 偏移 (128,96)
        Assert.True(scene.IsSelectMyself(608, 384));

        scene.G.MySelf = null;
        Assert.False(scene.IsSelectMyself(608, 384));
    }

    // ---------------- 地面物品 ----------------

    private static DropItemsStore MakeDropStore()
    {
        var store = new DropItemsStore(_ => null, () => _tick);
        return store;
    }

    [Fact]
    public void GetDropItems_ByNameConcatenationAndProximity()
    {
        var (scene, _, _) = MakeScene();
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        scene.G.MySelf = self;
        scene.DropItemImageProbe = _ => new SurfaceSize(32, 32);
        scene.CheckTextureAlpha = (_, _, _) => false;
        scene.DropItems = MakeDropStore();

        var item = scene.DropItems.AddDropItem(1, 10, 10, "金创药(小)", 0);
        item.Name = "金创药(小)";
        item.Looks = 5;

        // 点 (10,10) 屏幕坐标 = (512, 384)
        var hit = scene.GetDropItems(512, 384, out string names);
        Assert.Same(item, hit);
        Assert.Equal("金创药(小)\\", names);

        // 偏离 30px 以上：dx2/dy2 超出 ±10 且 alpha 假 → 未命中
        var miss = scene.GetDropItems(512 + 60, 384 + 60, out string names2);
        Assert.Null(miss);
        Assert.Equal("", names2);

        // 图未就绪（probe 返回 null）→ 跳过
        scene.DropItemImageProbe = _ => null;
        Assert.Null(scene.GetDropItems(512, 384, out _));
    }

    [Fact]
    public void GetDropItems_HintListBoxAndAlphaProbe()
    {
        var (scene, _, _) = MakeScene();
        scene.G.ScreenCenterX = 512;
        scene.G.ScreenCenterY = 384;
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        scene.G.MySelf = self;
        scene.DropItemImageProbe = _ => new SurfaceSize(32, 32);
        scene.DropItems = MakeDropStore();

        var item = scene.DropItems.AddDropItem(1, 10, 10, "DB", 0);
        item.Name = "屠龙";
        item.Looks = 3;
        item.Visible = true;
        item.TextureWidth = 32;
        item.TextureHeight = 32;

        var hints = new List<string>();
        scene.CheckTextureAlpha = (_, _, _) => true;
        var hit = scene.GetDropItems(512, 384, hints);
        Assert.Same(item, hit);
        Assert.Equal(new[] { "屠龙" }, hints);

        // Visible=false → 无命中（外层遍历跳过）
        item.Visible = false;
        hints.Clear();
        Assert.Null(scene.GetDropItems(512, 384, hints));
        Assert.Empty(hints);
    }

    [Fact]
    public void GetXYDropItems_RequiresVisibleAndSamePoint()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_nRx = 10;
        self.m_nRy = 10;
        scene.G.MySelf = self;
        scene.DropItems = MakeDropStore();

        var item = scene.DropItems.AddDropItem(1, 10, 10, "x", 0);
        item.Name = "n";
        Assert.Same(item, scene.GetXYDropItems(10, 10));

        var list = new List<DropItem>();
        scene.GetXYDropItemsList(10, 10, list);
        Assert.Single(list);

        item.Visible = false;
        Assert.Null(scene.GetXYDropItems(10, 10));
        list.Clear();
        scene.GetXYDropItemsList(10, 10, list);
        Assert.Empty(list);

        Assert.Null(scene.GetXYDropItems(11, 11));
    }

    // ---------------- 震动 ----------------

    [Fact]
    public void SceneShake_ImmediateAddsEightOffsetsPerCount()
    {
        var (scene, _, _) = MakeScene();
        scene.SceneShake(2, 0);
        Assert.Equal(16, scene.SceneShakeList.Count);
        Assert.Equal((0, -10), TPlayScene.UnpackShake(scene.SceneShakeList[0]));
        Assert.Equal((0, 0), TPlayScene.UnpackShake(scene.SceneShakeList[1]));
        Assert.Equal((0, -8), TPlayScene.UnpackShake(scene.SceneShakeList[2]));
        Assert.Equal((0, -4), TPlayScene.UnpackShake(scene.SceneShakeList[6]));
        Assert.Equal((0, 0), TPlayScene.UnpackShake(scene.SceneShakeList[7]));
        Assert.False(scene.m_boDelaySceneShake);
    }

    [Fact]
    public void SceneShake_DelayedThenCheckFiresOnce()
    {
        var (scene, _, _) = MakeScene();
        scene.SceneShake(3, 200);
        Assert.True(scene.m_boDelaySceneShake);
        Assert.Equal(3, scene.m_dwDelaySceneShakeCount);
        Assert.Empty(scene.SceneShakeList);

        // 未到点 → 不触发
        _tick += 199;
        scene.CheckSceneShake();
        Assert.True(scene.m_boDelaySceneShake);
        Assert.Empty(scene.SceneShakeList);

        // 到点 → 关标记 + 立即震动 3×8
        _tick += 1;
        scene.CheckSceneShake();
        Assert.False(scene.m_boDelaySceneShake);
        Assert.Equal(24, scene.SceneShakeList.Count);
    }

    // ---------------- 生命周期 ----------------

    [Fact]
    public void Lifecycle_InitializeOpenCloseFinalize()
    {
        var (scene, _, dlg) = MakeScene();
        scene.Initialize();
        Assert.True(scene.m_boCanDraw);
        Assert.Equal(-1, scene.m_nCurrX);
        Assert.Equal(5, scene.MakeActorLabelCount);   // 5 张名条构建后释放

        scene.OpenScene();
        Assert.True(dlg.ViewBottomBoxVisible);

        scene.CloseScene();
        Assert.False(dlg.ViewBottomBoxVisible);
        Assert.Equal(1, scene.SilenceSoundCount);

        var actor = MakeActor(1, 0, 0);
        int finalized = 0;
        actor.OnFinalize = () => finalized++;
        scene.DoAddActor(actor);
        scene.Finalize();
        Assert.False(scene.m_boCanDraw);
        Assert.Equal(1, finalized);
        Assert.True(scene.OldClientRectCleared);
    }

    [Fact]
    public void CanDrawTileMap_RequiresFlags()
    {
        var (scene, map, _) = MakeScene();
        Assert.True(scene.CanDrawTileMap());

        map.Moving = true;
        Assert.False(scene.CanDrawTileMap());
        map.Moving = false;

        scene.boCanDrawTileMap = false;
        Assert.False(scene.CanDrawTileMap());
        scene.boCanDrawTileMap = true;

        scene.MapLoadOk = false;
        Assert.False(scene.CanDrawTileMap());
    }

    // ---------------- NewActor ----------------

    private static TFeature HumFeatureOf(THumFeature hum)
    {
        var f = new TFeature();
        f.SetHumFeature(hum);
        return f;
    }

    private static TFeature MonFeatureOf(TMonFeature mon)
    {
        var f = new TFeature();
        f.SetMonFeature(mon);
        return f;
    }

    [Fact]
    public void NewActor_RejectsWhileMapMoving()
    {
        var (scene, map, _) = MakeScene();
        map.Moving = true;
        var f = MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14 });
        Assert.Null(scene.NewActor(1, 1, 1, 0, f, 0));
        Assert.Empty(scene.ActorList);
    }

    [Fact]
    public void NewActor_ReusesExistingAndClearsDeleteFlag()
    {
        var (scene, _, _) = MakeScene();
        var existing = MakeActor(55, 1, 1);
        existing.m_boDelActor = true;
        scene.DoAddActor(existing);

        var f = MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14 });
        var result = scene.NewActor(55, 9, 9, 3, f, 0);
        Assert.Same(existing, result);
        Assert.False(existing.m_boDelActor);
        Assert.Equal(1, existing.m_nCurrX);      // 复用分支不改坐标（原文 6988-6993）
        Assert.Single(scene.ActorList);
    }

    [Fact]
    public void NewActor_DispatchesRaceImgToActorClass()
    {
        var (scene, _, _) = MakeScene();

        var human = scene.NewActor(1, 10, 10, 2,
            HumFeatureOf(new THumFeature { wDress = 5, btGender = 1, btHair = 3, nChangeAppr = -1 }), 0);
        Assert.IsType<THumActor>(human);
        Assert.Equal("THumActor", human!.ActorClass);
        Assert.Equal((ushort)0, human.m_wAppearance);   // nChangeAppr<0 → 0
        Assert.Equal((ushort)5, human.m_wDress);
        Assert.Equal(THumBBType.bbNo, human.m_HumsBBType);

        var skel = scene.NewActor(2, 20, 20, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14, wAppr = 30, MonLevel = 7 }), 0);
        Assert.IsType<TSkeletonOma>(skel);
        Assert.Equal(30, skel!.m_wAppearance);
        Assert.Equal(7, skel.m_nLevel);
        Assert.Equal(0, skel.m_btSex);

        var npc = scene.NewActor(3, 30, 30, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 50, btRace = 50, wAppr = 10 }), 0);
        Assert.IsType<TNpcActor>(npc);

        var statue = scene.NewActor(4, 31, 31, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 50, btRace = 50, wAppr = 273 }), 0);
        Assert.IsType<TStatuaryNpcActor>(statue);

        var arrow = scene.NewActor(5, 32, 32, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 210, btRace = 112 }), 0);
        Assert.IsType<TArcherMon>(arrow);

        var unknown = scene.NewActor(6, 33, 33, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 9999, btRace = 9 }), 0);
        Assert.IsType<TActor>(unknown);

        // 有序表按 id 升序
        Assert.Equal(new long[] { 1, 2, 3, 4, 5, 6 }, scene.ActorList.ConvertAll(a => a.m_nRecogId));
        Assert.Equal(6, scene.LoadSurfaceCount);
    }

    [Fact]
    public void NewActor_SimpleShowReplacesMonsterAppearance()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowActor = true;
        scene.G.ckSimpleShowActor = true;

        var actor = scene.NewActor(1, 1, 1, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14, wAppr = 99, HumBBType = THumBBType.bbNo }), 0);
        // 简装替换 → 稻草人（RaceImg 18 / Appr 27 / Race 83）
        Assert.IsType<THuSuABi>(actor);
        Assert.Equal(27, actor!.m_wAppearance);
        Assert.Equal(83, actor.m_btRealRace);

        // 守卫族豁免（Race 112 弓箭手 + RaceImg 1）：不换装，按 RaceImg=1 建 THeroActor；
        // 注意 RaceImg ∈ [0,1] 走人物分支，m_wAppearance 取自 thumFeature.nChangeAppr（默认 -1 → 0），
        // 原文 7321-7326 同样丢弃 monFeature.wAppr
        var guard = scene.NewActor(2, 2, 2, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 1, btRace = PlaySceneConsts.RC_ARCHERGUARD, wAppr = 7 }), 0);
        Assert.IsType<THeroActor>(guard);
        Assert.Equal(0, guard!.m_wAppearance);
        Assert.Equal(PlaySceneConsts.RC_ARCHERGUARD, guard.m_btRealRace);
        Assert.Equal(1, guard.m_btRace);

        // 禁止简装开关
        var disabled = scene.NewActor(3, 3, 3, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14, wAppr = 99, IsDisableSimpleActor = true }), 0);
        Assert.IsType<TSkeletonOma>(disabled);
    }

    [Fact]
    public void NewActor_HeroSlotReuse()
    {
        var (scene, _, _) = MakeScene();
        var hero = new THeroActor { m_nRecogId = 77 };
        scene.G.MyHero = hero;

        var result = scene.NewActor(77, 5, 6, 4,
            HumFeatureOf(new THumFeature { btGender = 1, btJob = 2, wWeapon = 12, nChangeAppr = 300, btHair = 4 }), 0);
        Assert.Same(hero, result);
        Assert.Equal(5, hero.m_nCurrX);
        Assert.Equal(6, hero.m_nRy);
        Assert.Equal((byte)4, hero.m_btDir);
        Assert.Equal(1, hero.m_btRace);
        Assert.Equal(300, hero.m_wAppearance);          // nChangeAppr ≥ 0 → 生效
        Assert.Equal((ushort)12, hero.m_wWeapon);
        Assert.Equal((byte)4, hero.m_btHair);
    }

    [Fact]
    public void NewActor_ChangingFaceBlocksCreate()
    {
        var (scene, _, _) = MakeScene();
        scene.IsChangingFaceFn = id => id == 42;
        var actor = scene.NewActor(42, 1, 1, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14 }), 0);
        // boCreate 关闭 → 走 TActor 默认分支
        Assert.IsType<TActor>(actor);
        Assert.IsNotType<TSkeletonOma>(actor);
    }

    [Fact]
    public void NewActor_CustomMonsterMissingConfig()
    {
        var (scene, _, _) = MakeScene();
        var missing = new List<int>();

        var actor = scene.NewActor(1, 1, 1, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 156, btRace = 155, wAppr = 400 }), 0);
        // CustomMonsterConfigResolver 为空 → cfg = nil → 自定义怪缺配置 → 退回 TActor
        Assert.IsType<TActor>(actor);
        Assert.IsNotType<TCustomActor>(actor);

        // 配置存在 → TCustomActor
        scene.CustomMonsterConfigResolver = _ => new object();
        var found = scene.NewActor(2, 2, 2, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 156, btRace = 155, wAppr = 400 }), 0);
        Assert.IsType<TCustomActor>(found);

        // 非 154..157 → TActor
        var plain = scene.NewActor(3, 3, 3, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 156, btRace = 20 }), 0);
        Assert.IsType<TActor>(plain);
    }

    [Fact]
    public void NewActor_MagicLockFix()
    {
        var (scene, _, _) = MakeScene();
        scene.G.g_nMagicTargetRecogId = 9;
        var actor = scene.NewActor(9, 1, 1, 0,
            MonFeatureOf(new TMonFeature { wRaceImg = 14, btRace = 14 }), 0);
        Assert.Equal(0, scene.G.g_nMagicTargetRecogId);
        Assert.Same(actor, scene.G.g_MagicLockActor);
    }

    [Fact]
    public void WalkShiftTable_LocksEightDirections()
    {
        Assert.Equal(8, TPlayScene.WalkShift.Length);
        Assert.Equal((0, 5), TPlayScene.WalkShift[PlaySceneConsts.DR_UP][0]);
        Assert.Equal((0, 6), TPlayScene.WalkShift[PlaySceneConsts.DR_UP][1]);
        Assert.Equal((8, 0), TPlayScene.WalkShift[PlaySceneConsts.DR_RIGHT][0]);
        Assert.Equal((8, 0), TPlayScene.WalkShift[PlaySceneConsts.DR_LEFT][5]);
        Assert.Equal((8, 6), TPlayScene.WalkShift[PlaySceneConsts.DR_UPLEFT][4]);
    }

    [Fact]
    public void CharWidthHeightAndCheckSelect()
    {
        var a = new TActor();
        // 步行无图 → 48/70
        Assert.Equal(48, a.CharWidth);
        Assert.Equal(70, a.CharHeight);

        a.BodySurface = new SurfaceSize(64, 90);
        Assert.Equal(64, a.CharWidth);
        Assert.Equal(90, a.CharHeight);

        // 骑马无马图 → 100/70；有马图 → 马图尺寸
        a.m_btHorse = 1;
        Assert.Equal(100, a.CharWidth);
        Assert.Equal(70, a.CharHeight);
        a.HorseSurface = new SurfaceSize(120, 88);
        Assert.Equal(120, a.CharWidth);
        Assert.Equal(88, a.CharHeight);

        // CheckSelect：十字五像素全通过
        var probe = new List<(int, int)>();
        a.CheckTextureAlpha = (_, x, y) =>
        {
            probe.Add((x, y));
            return x is >= -1 and <= 1 && y is >= -1 and <= 1;
        };
        Assert.True(a.CheckSelect(0, 0));
        Assert.Equal(new[] { (0, 0), (-1, 0), (1, 0), (0, -1), (0, 1) }, probe.ToArray());

        a.CheckTextureAlpha = (_, _, _) => false;
        Assert.False(a.CheckSelect(0, 0));

        // 无图 → false
        var b = new TActor { CheckTextureAlpha = (_, _, _) => true };
        Assert.False(b.CheckSelect(0, 0));
    }

    [Fact]
    public void MapPath_DirectionAndStep()
    {
        Assert.Equal(PlaySceneConsts.DR_RIGHT, MapPath.GetNextDirection(0, 0, 5, 0));
        Assert.Equal(PlaySceneConsts.DR_LEFT, MapPath.GetNextDirection(5, 0, 0, 0));
        Assert.Equal(PlaySceneConsts.DR_DOWN, MapPath.GetNextDirection(0, 0, 0, 5));
        Assert.Equal(PlaySceneConsts.DR_UP, MapPath.GetNextDirection(0, 5, 0, 0));
        Assert.Equal(PlaySceneConsts.DR_DOWNRIGHT, MapPath.GetNextDirection(0, 0, 1, 1));
        Assert.Equal(PlaySceneConsts.DR_UPLEFT, MapPath.GetNextDirection(0, 0, -1, -1));
        Assert.Equal(PlaySceneConsts.DR_UPRIGHT, MapPath.GetNextDirection(0, 0, 1, -1));
        Assert.Equal(PlaySceneConsts.DR_DOWNLEFT, MapPath.GetNextDirection(0, 0, -1, 1));
        Assert.Equal(PlaySceneConsts.DR_DOWN, MapPath.GetNextDirection(3, 3, 3, 3)); // 同点 → DR_DOWN

        int x = 10, y = 10;
        MapPath.GetNextPosXY(PlaySceneConsts.DR_UPLEFT, ref x, ref y);
        Assert.Equal(9, x);
        Assert.Equal(9, y);
        MapPath.GetNextPosXY(99, ref x, ref y);   // 越界方向不动
        Assert.Equal(9, x);
        Assert.Equal(9, y);
    }

    [Fact]
    public void DoSearchSortYDrawActtor_BinaryByRyMinusDownDrawLevel()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(1, 0, 10);
        a.m_nDownDrawLevel = 2;   // key = 8
        var b = MakeActor(2, 0, 20);
        b.m_nDownDrawLevel = 0;   // key = 20
        var c = MakeActor(3, 0, 30);
        c.m_nDownDrawLevel = 5;   // key = 25
        scene.SortYDrawActorList.AddRange(new[] { a, b, c });

        Assert.True(scene.DoSearchSortYDrawActtor(20, out int i20));
        Assert.Equal(1, i20);
        Assert.False(scene.DoSearchSortYDrawActtor(15, out int i15));
        Assert.Equal(1, i15);
        Assert.False(scene.DoSearchSortYDrawActtor(26, out int i26));
        Assert.Equal(3, i26);
    }

    [Fact]
    public void TickDiff_HandlesWrapAround()
    {
        Assert.Equal(10u, TPlayScene.TickDiff(1000, 1010));
        Assert.Equal(5u, TPlayScene.TickDiff(uint.MaxValue - 2, 2));   // 回绕安全
    }
}

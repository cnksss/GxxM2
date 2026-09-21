using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// PlayScn.pas 消息驱动与角色生命周期（批次J73）：
/// ClearDropItem（735-842）、CleanObjects（600-724）、ActorDied（7489-7525）、
/// SetActorDrawLevel（7527-7533）、ClearActors（7535-7659）、DeleteActor/DelActor（7661-7716）、
/// AddEffectList（4374-4387）、ProcessActors（906-1336）、SendMsg（7758-8271）。
/// </summary>
public sealed class PlaySceneMessagesTests : IDisposable
{
    // ---------------- 测试替身 ----------------

    private sealed class FakeMap : IPlayMap
    {
        public HashSet<(int, int)> Movable = new();
        public HashSet<(int, int)> NewMovable = new();
        public HashSet<(int, int)> Flyable = new();

        public int BlockLeft => 0;
        public int BlockTop => 0;
        public int ClientLeft => 0;
        public int ClientTop => 0;
        public int ClientRight => 0;
        public int ClientBottom => 0;
        public bool MapMoving => false;
        public bool CanMove(int mx, int my) => Movable.Contains((mx, my));
        public bool NewCanMove(int mx, int my) => NewMovable.Contains((mx, my));
        public bool CanFly(int mx, int my) => Flyable.Contains((mx, my));

        public string LastLoadedMap = "";
        public string OldMapName { get; set; } = "";
        public bool MiniMapVisible { get; set; }
        public bool CheckDMinMapBigDlgVisible { get; set; }
        public bool IsDGJPointsShow { get; set; }
        public void LoadMap(string mapName, int x, int y) => LastLoadedMap = mapName;
    }

    /// <summary>frmMain 接缝记录器。</summary>
    private sealed class FakeFrmMain : FrmMainSeam
    {
        public int ClearDropItemsCount;
        public readonly List<string> MiniMapRequests = new();
        public readonly List<string> ChatLines = new();
        public readonly List<(int X, int Y, int MagicId)> UsedMagics = new();
        public int ClearWaitingHeroUseItemCount;
        public int ClearHeroEatingItemCount;
        public int CloseDHeroStateDlgCount;
        public int CloseDHeroItemBagDlgCount;
        public int CloseDHeroStateWinDlgCount;
        public readonly HashSet<string> FriendMemo = new(StringComparer.Ordinal);
        public readonly HashSet<string> BlacklistMemo = new(StringComparer.Ordinal);

        public void ClearDropItems() => ClearDropItemsCount++;
        public void SendWantMiniMap(bool big) => MiniMapRequests.Add(big ? "big" : "small");
        public void AddChatBoardString(string text) => ChatLines.Add(text);
        public void ClearWaitingHeroUseItem() => ClearWaitingHeroUseItemCount++;
        public void ClearHeroEatingItem() => ClearHeroEatingItemCount++;
        public void UseMagic(int x, int y, (int MagicId, int Spell, int DefSpell) magic)
            => UsedMagics.Add((x, y, magic.MagicId));
        public bool IsInFriendMemo(string name) => FriendMemo.Contains(name);
        public bool IsInBlacklistMemo(string name) => BlacklistMemo.Contains(name);
        public void CloseDHeroStateDlg() => CloseDHeroStateDlgCount++;
        public void CloseDHeroItemBagDlg() => CloseDHeroItemBagDlgCount++;
        public void CloseDHeroStateWinDlg() => CloseDHeroStateWinDlgCount++;
    }

    private static TActorCore MakeActor(long id, int x, int y, string name = "", int race = 0)
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

    private static (TPlayScene Scene, FakeMap Map, FakeFrmMain Frm) MakeScene()
    {
        var map = new FakeMap();
        var scene = new TPlayScene(new SceneDialogs(), map);
        var frm = new FakeFrmMain();
        scene.FrmMain = frm;
        return (scene, map, frm);
    }

    public void Dispose() => SceneTime.TickNow = () => (uint)Environment.TickCount;

    // ================= AddEffectList / SetActorDrawLevel =================

    [Fact]
    public void AddEffectList_SplitsGroundEffectFromOthers()
    {
        var (scene, _, _) = MakeScene();

        var ground = new TBujaukGroundEffect(140, 5, 1, 1, 3, 3); // mtBujaukGroundEffect
        var normal = new TPlayEffect(100, 0, 0, 0, 1, 1, false, null);

        scene.AddEffectList(ground);
        scene.AddEffectList(normal);

        Assert.Single(scene.GroundEffectList);
        Assert.Same(ground, scene.GroundEffectList[0]);
        Assert.Single(scene.EffectList);
        Assert.Same(normal, scene.EffectList[0]);
    }

    [Fact]
    public void SetActorDrawLevel_ZeroMovesToFrontOthersIgnored()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(1, 1, 1);
        var b = MakeActor(2, 2, 2);
        var c = MakeActor(3, 3, 3);

        scene.DrawActorList.Add(a);
        scene.DrawActorList.Add(b);
        scene.DrawActorList.Add(c);

        scene.SetActorDrawLevel(c, 0);
        Assert.Equal(new[] { c, a, b }, scene.DrawActorList);

        // 非 0 等级原文无任何操作
        scene.SetActorDrawLevel(c, 3);
        Assert.Equal(new[] { c, a, b }, scene.DrawActorList);
    }

    // ================= ActorDied / DeleteActor / DelActor =================

    [Fact]
    public void ActorDied_IsNoOpAsInOriginal()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(7, 1, 1);
        scene.DoAddActor(a);
        a.m_dwDeleteTime = 123;

        scene.ActorDied(a);

        // 原文函数体为空（旧重排逻辑整段注释），因此不动任何标记
        Assert.Single(scene.ActorList);
        Assert.False(a.m_boDelActor);
        Assert.False(a.m_boFreeActor);
        Assert.Equal(123u, a.m_dwDeleteTime);
    }

    [Fact]
    public void DeleteActor_HeroKeepsActorNotFreeAndClearsMsgs()
    {
        SceneTime.TickNow = () => 9000;
        var (scene, _, _) = MakeScene();
        var hero = MakeActor(11, 1, 1);
        scene.G.MyHero = hero;
        scene.DoAddActor(hero);
        hero.SendMsg(new TChrMsg { Ident = 1 });

        scene.DeleteActor(11);

        Assert.True(hero.m_boDelActor);
        Assert.False(hero.m_boFreeActor);     // 英雄不回收
        Assert.Empty(hero.MsgList);
        Assert.Equal(9000u, hero.m_dwDeleteTime);
        Assert.Empty(scene.DrawActorList);
        Assert.Single(scene.ActorList);       // 尚未由 ProcessActors 移出
    }

    [Fact]
    public void DeleteActor_NormalActorFreesAndIgnoresMsgs()
    {
        SceneTime.TickNow = () => 9000;
        var (scene, _, _) = MakeScene();
        var a = MakeActor(12, 1, 1);
        scene.DoAddActor(a);
        a.SendMsg(new TChrMsg { Ident = 1 });

        scene.DeleteActor(12);

        Assert.True(a.m_boDelActor);
        Assert.True(a.m_boFreeActor);
        Assert.Single(a.MsgList);             // 非英雄不清消息
        Assert.Equal(9000u, a.m_dwDeleteTime);
    }

    [Fact]
    public void DeleteActor_MissingIdIsNoOp()
    {
        var (scene, _, _) = MakeScene();
        scene.DeleteActor(999);
        Assert.Empty(scene.ActorList);
    }

    [Fact]
    public void DelActor_OnlyRegistersWhenPresentInList()
    {
        SceneTime.TickNow = () => 7000;
        var (scene, _, _) = MakeScene();
        var a = MakeActor(21, 1, 1);
        var ghost = MakeActor(22, 1, 1);

        scene.DoAddActor(a);
        scene.DelActor(a);
        Assert.True(a.m_boDelActor);
        Assert.True(a.m_boFreeActor);
        Assert.Equal(7000u, a.m_dwDeleteTime);

        // 不在表中 → 原文不做任何事
        scene.DelActor(ghost);
        Assert.False(ghost.m_boDelActor);
        Assert.False(ghost.m_boFreeActor);
        Assert.Equal(0u, ghost.m_dwDeleteTime);
    }

    // ================= CleanObjects =================

    [Fact]
    public void CleanObjects_KeepsSelfAndHeroOnly()
    {
        SceneTime.TickNow = () => 5000;
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        var hero = MakeActor(2, 2, 2);
        var other = MakeActor(3, 3, 3);
        scene.G.MySelf = self;
        scene.G.MyHero = hero;
        scene.DoAddActor(self);
        scene.DoAddActor(hero);
        scene.DoAddActor(other);
        scene.DrawActorList.Add(other);
        scene.DrawActorList.Add(self);
        scene.MsgList.Add(new TChrMsg());

        scene.CleanObjects();

        Assert.Equal(new[] { self, hero }, scene.ActorList);
        Assert.Equal(new[] { self }, scene.DrawActorList);
        Assert.Contains(other, scene.FreeActorList);
        Assert.DoesNotContain(self, scene.FreeActorList);
        Assert.DoesNotContain(hero, scene.FreeActorList);
        Assert.True(other.m_boGhost);
        Assert.Empty(scene.MsgList);
    }

    [Fact]
    public void CleanObjects_RecyclesEffectsAndResetsGlobals()
    {
        SceneTime.TickNow = () => 4000;
        var (scene, _, frm) = MakeScene();
        scene.G.MySelf = MakeActor(1, 1, 1);

        var e1 = new TPlayEffect(100, 0, 0, 0, 1, 1, false, null);
        var e2 = new TBujaukGroundEffect(140, 5, 1, 1, 3, 3);
        var e3 = new TPlayEffect(101, 0, 0, 0, 1, 1, false, null);
        scene.EffectList.Add(e1);
        scene.GroundEffectList.Add(e2);
        scene.FlyList.Add(e3);

        scene.G.TargetCret = scene.G.MySelf;
        scene.G.FocusCret = scene.G.MySelf;
        scene.G.BrightActor = scene.G.MySelf;
        scene.G.MagicTarget = 55;
        scene.m_nProcDropItemsIdx = 12;

        scene.CleanObjects();

        Assert.Empty(scene.EffectList);
        Assert.Empty(scene.GroundEffectList);
        Assert.Empty(scene.FlyList);
        Assert.Equal(3, scene.FreeEffectList.Count);
        Assert.Equal(4000, e1.m_dwGhostTick);
        Assert.Null(scene.G.TargetCret);
        Assert.Null(scene.G.FocusCret);
        Assert.Null(scene.G.BrightActor);
        Assert.Equal(0, scene.G.MagicTarget);
        Assert.Equal(0, scene.m_nProcDropItemsIdx);
        Assert.Equal(1, frm.ClearDropItemsCount);
    }

    [Fact]
    public void CleanObjects_ProcessesSelfLastMsgAndClearsQueues()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        var hero = MakeActor(2, 2, 2);
        scene.G.MySelf = self;
        scene.G.MyHero = hero;
        scene.DoAddActor(self);
        scene.DoAddActor(hero);
        self.SendMsg(new TChrMsg { Ident = 1 });
        hero.SendMsg(new TChrMsg { Ident = 2 });

        int selfProc = 0;
        self.OnProcLastMsg = () => selfProc++;

        scene.CleanObjects();

        Assert.Equal(1, selfProc);
        Assert.Empty(self.MsgList);
        Assert.Empty(hero.MsgList);
    }

    // ================= ClearActors =================

    [Fact]
    public void ClearActors_RecyclesEverythingAndNullsAllGlobals()
    {
        SceneTime.TickNow = () => 6000;
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 1, 1);
        var hero = MakeActor(2, 2, 2);
        var other = MakeActor(3, 3, 3);
        scene.G.MySelf = self;
        scene.G.MyHero = hero;
        scene.DoAddActor(self);
        scene.DoAddActor(hero);
        scene.DoAddActor(other);
        scene.DrawActorList.Add(self);
        scene.SortYDrawActorList.Add(self);
        scene.MsgList.Add(new TChrMsg());

        var eff = new TPlayEffect(100, 0, 0, 0, 1, 1, false, null);
        scene.EffectList.Add(eff);

        scene.G.LockTarget = self;
        scene.G.BrightActor = self;
        scene.G.TargetCret = self;
        scene.G.FocusCret = self;
        scene.G.MagicTarget = 9;

        scene.ClearActors();

        Assert.Empty(scene.ActorList);
        Assert.Empty(scene.DrawActorList);
        Assert.Empty(scene.SortYDrawActorList);
        Assert.Empty(scene.MsgList);
        Assert.Empty(scene.EffectList);
        Assert.Equal(4, scene.FreeActorList.Count);   // self/hero/other + 重新登记的 hero
        Assert.True(other.m_boGhost);
        Assert.True(hero.m_boGhost);
        Assert.Single(scene.FreeEffectList);
        Assert.Equal(6000, eff.m_dwGhostTick);

        Assert.Null(scene.G.MySelf);
        Assert.Null(scene.G.MyHero);
        Assert.Null(scene.G.BrightActor);
        Assert.Null(scene.G.TargetCret);
        Assert.Null(scene.G.FocusCret);
        Assert.Null(scene.G.LockTarget);
        Assert.Equal(0, scene.G.MagicTarget);
        Assert.Equal(6000u, scene.G.FocusCretTick);
        Assert.Equal(6000u, scene.G.dwLockTargetTick);
        Assert.Equal(1, frm.ClearDropItemsCount);
    }

    [Fact]
    public void ClearActors_HeroIsRegisteredTwiceLikeOriginal()
    {
        var (scene, _, _) = MakeScene();
        var hero = MakeActor(5, 1, 1);
        scene.G.MyHero = hero;
        scene.DoAddActor(hero);

        scene.ClearActors();

        // 原文：英雄先在表内循环入回收表，随后又被单独 AddFreeActorList 一次
        Assert.Equal(2, scene.FreeActorList.Count);
        Assert.Same(hero, scene.FreeActorList[0]);
        Assert.Same(hero, scene.FreeActorList[1]);
    }

    // ================= ProcessActors =================

    [Fact]
    public void ProcessActors_NoSelfIsImmediateReturn()
    {
        var (scene, _, _) = MakeScene();
        scene.boCanDrawTileMap = true;

        scene.ProcessActors();

        Assert.False(scene.boCanDrawTileMap);
        Assert.Equal(0, scene.m_nRenderCode);
    }

    [Fact]
    public void ProcessActors_DeletedActorIsRemovedAndRecycled()
    {
        SceneTime.TickNow = () => 100;
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        var other = MakeActor(2, 2, 2);
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.DoAddActor(other);

        // ProcessActors 先清绘制位；movetick 由 tick 差触发
        other.m_boDelActor = true;
        other.m_boFreeActor = true;

        scene.ProcessActors();

        Assert.Single(scene.ActorList);
        Assert.Same(self, scene.ActorList[0]);
        Assert.Contains(other, scene.FreeActorList);
        Assert.True(other.m_boGhost);
    }

    [Fact]
    public void ProcessActors_MovetickAdvancesAndResetsStepCount()
    {
        SceneTime.TickNow = () => 100000;
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.m_dwMoveTime = 0;      // 保证首次必触发 movetick

        scene.ProcessActors();

        Assert.True(scene.m_nMoveStepCount is 0 or 1);
        Assert.Equal(100000u, scene.m_dwMoveTime);
    }

    [Fact]
    public void ProcessActors_AniTickRollsOverAt100000()
    {
        var now = 200u;
        SceneTime.TickNow = () => now;
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.m_nAniCount = 100000;
        scene.m_dwAniTime = 0;

        scene.ProcessActors();

        Assert.Equal(0, scene.m_nAniCount);   // >100000 回卷
        Assert.Equal(200u, scene.m_dwAniTime);
    }

    [Fact]
    public void ProcessActors_EffectRunFalseIsRecycledRunTrueEntersDrawList()
    {
        SceneTime.TickNow = () => 300;
        var (scene, _, _) = MakeScene();
        scene.G.MySelf = MakeActor(1, 1, 1);

        // keep：NextFrameTime 极大 → Shift 推进不动 → Run 返回 true → 入地面绘制表
        var keep = new TPlayEffect(100, 0, 0, 0, 1, 5, false, null);
        keep.m_boActive = true;
        keep.NextFrameTime = int.MaxValue;
        scene.GroundEffectList.Add(keep);

        // drop：TPlayEffect 是循环特效（Run 恒 true），改用 TLightingEffect——其 Run 恒返回 false
        var drop = new TLightingEffect(0, 0, 0, 0);
        drop.m_boActive = true;
        scene.FlyList.Add(drop);

        scene.ProcessActors();

        Assert.Single(scene.GroundEffectList);
        Assert.Same(keep, scene.GroundEffectList[0]);
        Assert.Single(scene.DrawGroundEffectList);
        Assert.Empty(scene.FlyList);
        Assert.Contains(drop, scene.FreeEffectList);
        Assert.Equal(300, drop.m_dwGhostTick);
    }

    [Fact]
    public void ProcessActors_InactiveEffectIsLeftUntouched()
    {
        SceneTime.TickNow = () => 300;
        var (scene, _, _) = MakeScene();
        scene.G.MySelf = MakeActor(1, 1, 1);

        // m_boActive = false → 原文只在 m_boActive 时处理，既不回收也不入绘制表
        var idle = new TPlayEffect(100, 0, 0, 0, 1, 5, false, null);
        scene.EffectList.Add(idle);

        scene.ProcessActors();

        Assert.Single(scene.EffectList);
        Assert.Empty(scene.FreeEffectList);
    }

    [Fact]
    public void ProcessActors_WaitForRecogIdReplacesActorWhenIdle()
    {
        SceneTime.TickNow = () => 800;
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        var waiting = MakeActor(2, 5, 5, "神兽");
        waiting.m_nWaitForRecogId = 77;
        waiting.OnIsIdle = () => true;
        waiting.m_btBodyColor = 9;
        waiting.m_nNameColor = 44;
        scene.DoAddActor(waiting);

        var f = new TFeature();
        f.SetMonFeature(new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3 });
        waiting.m_WaitForFeature = f;

        long delChanged = -1;
        scene.DelChangeFaceFn = id => delChanged = id;

        scene.ProcessActors();

        Assert.Equal(77, delChanged);
        Assert.Equal(0, waiting.m_nWaitForRecogId);
        Assert.True(waiting.m_boDelActor);
        Assert.True(waiting.m_boFreeActor);

        // 新角接管名字/体色/名色
        var waitActor = scene.FindActor(77);
        Assert.NotNull(waitActor);
        Assert.Equal("神兽", waitActor!.m_sUserName);
        Assert.Equal(9, waitActor.m_btBodyColor);
        Assert.Equal(44, waitActor.m_nNameColor);
    }

    // ================= SendMsg：换图族 =================

    [Fact]
    public void SendMsg_NewMapLoadsMapAndDelaysSelfRemoval()
    {
        var (scene, map, frm) = MakeScene();
        var self = MakeActor(1, 5, 6);
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        var result = scene.SendMsg(TPlayScene.SM_NEWMAP, 0, 10, 20, 2, null, 0, "0");

        Assert.Null(result);
        Assert.Equal("0", map.LastLoadedMap);
        Assert.Equal("", map.OldMapName);
        Assert.Equal(2, scene.g_nDarkLevel);
        Assert.True(scene.boViewFog);
        Assert.Equal(160, scene.g_nDarkValue);
        Assert.True(scene.boCanDrawTileMap);

        // 主角坐标回写后登记删除并置空（新图分支）
        Assert.Null(scene.G.MySelf);
        Assert.True(self.m_boDelActor);
    }

    [Fact]
    public void SendMsg_ChangeMapKeepsSelf()
    {
        var (scene, map, _) = MakeScene();
        var self = MakeActor(1, 5, 6);
        scene.G.MySelf = self;

        scene.SendMsg(TPlayScene.SM_CHANGEMAP, 0, 1, 2, 0, null, 0, "3");

        Assert.Equal("3", map.LastLoadedMap);
        Assert.Same(self, scene.G.MySelf);   // 换图（非新图）不清主角
        Assert.False(scene.boViewFog);       // darkLevel = 0
    }

    [Fact]
    public void SendMsg_NewMapMiniMapBranches()
    {
        // 小地图可见 → small 请求
        {
            var (scene, map, frm) = MakeScene();
            map.MiniMapVisible = true;
            scene.SendMsg(TPlayScene.SM_CHANGEMAP, 0, 1, 2, 0, null, 0, "0");
            Assert.Equal(new[] { "small" }, frm.MiniMapRequests);
            Assert.Equal(-1, scene.g_nMiniMapIndex);
        }

        // 未可见 + 寻路开启 + 放大小地图可见 → small
        {
            var (scene, map, frm) = MakeScene();
            scene.clientConfig_boUseFindPath = true;
            map.CheckDMinMapBigDlgVisible = true;
            scene.SendMsg(TPlayScene.SM_CHANGEMAP, 0, 1, 2, 0, null, 0, "0");
            Assert.Equal(new[] { "small" }, frm.MiniMapRequests);
        }

        // 未可见 + 打怪点显示 → big
        {
            var (scene, map, frm) = MakeScene();
            map.IsDGJPointsShow = true;
            scene.SendMsg(TPlayScene.SM_CHANGEMAP, 0, 1, 2, 0, null, 0, "0");
            Assert.Equal(new[] { "big" }, frm.MiniMapRequests);
        }

        // 三者皆否 → 不请求
        {
            var (scene, map, frm) = MakeScene();
            scene.SendMsg(TPlayScene.SM_CHANGEMAP, 0, 1, 2, 0, null, 0, "0");
            Assert.Empty(frm.MiniMapRequests);
        }
    }

    // ================= SendMsg：隐藏/英雄消失 =================

    [Fact]
    public void SendMsg_DisappearMyHeroClosesDialogsAndClearsHeroGlobal()
    {
        var (scene, _, frm) = MakeScene();
        var hero = MakeActor(31, 1, 1);
        scene.G.MyHero = hero;
        scene.DoAddActor(hero);

        var result = scene.SendMsg(TPlayScene.SM_DISAPPEARMYHERO, 31, 0, 0, 0, null, 0, "");

        Assert.Same(hero, result);
        Assert.Null(scene.G.MyHero);
        Assert.Equal(1, frm.CloseDHeroStateDlgCount);
        Assert.Equal(1, frm.CloseDHeroItemBagDlgCount);
        Assert.Equal(1, frm.CloseDHeroStateWinDlgCount);
        Assert.Equal(1, frm.ClearWaitingHeroUseItemCount);
        Assert.Equal(1, frm.ClearHeroEatingItemCount);
        Assert.True(hero.m_boDelActor);
    }

    [Fact]
    public void SendMsg_HideUnknownActorReturnsNull()
    {
        var (scene, _, _) = MakeScene();
        Assert.Null(scene.SendMsg(TPlayScene.SM_HIDE, 404, 0, 0, 0, null, 0, ""));
    }

    [Fact]
    public void SendMsg_HideOnDelActionAfterFinishedReturnsWithoutMarking()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(41, 1, 1);
        scene.DoAddActor(a);
        a.m_boDelActionAfterFinished = true;

        var result = scene.SendMsg(TPlayScene.SM_HIDE, 41, 0, 0, 0, null, 0, "");

        Assert.Same(a, result);
        Assert.False(a.m_boDelActor);
    }

    [Fact]
    public void SendMsg_HideOnWaitingActorReturnsWithoutMarking()
    {
        var (scene, _, _) = MakeScene();
        var a = MakeActor(42, 1, 1);
        scene.DoAddActor(a);
        a.m_nWaitForRecogId = 88;

        var result = scene.SendMsg(TPlayScene.SM_HIDE, 42, 0, 0, 0, null, 0, "");

        Assert.Same(a, result);
        Assert.False(a.m_boDelActor);
    }

    [Fact]
    public void SendMsg_HideMagicLockActorRebindsOrRemembersId()
    {
        var (scene, _, _) = MakeScene();
        var locked = MakeActor(51, 1, 1);
        scene.DoAddActor(locked);
        scene.G.g_MagicLockActor = locked;

        // X 低位 = 60、Y 高位 = 0 → newchrid = 60，且 60 不在表中 → 记入 g_nMagicTargetRecogId
        var result = scene.SendMsg(TPlayScene.SM_HIDE, 51, 60, 0, 0, null, 0, "");

        Assert.Same(locked, result);
        Assert.Null(scene.G.g_MagicLockActor);
        Assert.Equal(60, scene.G.g_nMagicTargetRecogId);
    }

    // ================= SendMsg：兜底分支建角与消息入队 =================

    [Fact]
    public void SendMsg_TurnCreatesMissingActorAndQueuesMessage()
    {
        var (scene, _, _) = MakeScene();
        var f = new TFeature();
        f.SetMonFeature(new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3 });

        var actor = scene.SendMsg(TActorCore.SM_TURN, 100, 4, 5, 6, f, 0, "名字");

        Assert.NotNull(actor);
        Assert.Equal(100, actor!.m_nRecogId);
        Assert.Equal(4, actor.m_nCurrX);
        Assert.Equal(5, actor.m_nCurrY);
        // SM_TURN 走照明/方向处理：HiByte(6)=0 写入 m_nChrLight，cdir 被压成 LoByte(6)=6
        Assert.Equal(0, actor.m_nChrLight);
        Assert.Single(actor.MsgList);
        var msg = actor.MsgList[0];
        Assert.Equal(TActorCore.SM_TURN, msg.Ident);
        Assert.Equal(4, msg.X);
        Assert.Equal(5, msg.Y);
        Assert.Equal(6, msg.Dir);
        // 原文：ident = SM_TURN 且 Str <> '' → 角色名取自 Str
        Assert.Equal("名字", actor.m_sUserName);
    }

    [Fact]
    public void SendMsg_DeadOnArrivalCreatesActorAndMarksDeath()
    {
        var (scene, _, _) = MakeScene();
        var f = new TFeature();
        f.SetMonFeature(new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3 });

        var actor = scene.SendMsg(TActorCore.SM_DEATH, 101, 1, 1, 0, f, 0, "");

        Assert.NotNull(actor);
        Assert.True(actor!.m_boDeath);
        Assert.False(actor.m_boStruckShowNumber);
        Assert.False(actor.m_boShowBigHPProgress);
    }

    [Fact]
    public void SendMsg_UnknownIdentOnMissingActorReturnsNull()
    {
        var (scene, _, _) = MakeScene();
        // SM_HIT(14) 不在「按需建角」集合内 → 不建角，直接 null
        Assert.Null(scene.SendMsg(TActorCore.SM_HIT, 999, 1, 1, 0, new TFeature(), 0, ""));
        Assert.Empty(scene.ActorList);
    }

    [Fact]
    public void SendMsg_BackstepDoesNotRelightAndKeepsPackedDir()
    {
        var (scene, _, _) = MakeScene();
        var actor = MakeActor(200, 1, 1);
        actor.m_nChrLight = 9;
        scene.DoAddActor(actor);

        scene.SendMsg(TActorCore.SM_BACKSTEP, 200, 1, 1, 0x0302, null, 0, "");

        // BACKSTEP：不写 m_nChrLight，仅旧值同步；cdir 保持打包值
        Assert.Equal(9, actor.m_nChrLight);
        // ★ 集成方修正（台账 §64.8 / D-P17-09）：原文三个写点**都带 `if Actor is TCustomActor` 守卫**
        //   （PlayScn.pas:7852-7853 / 8019-8020 / 8023-8024）⇒ **普通角色根本不会被写**。
        //   本用例原断言 `Assert.Equal(9, actor.m_nOldChrLight)`，那是按**丢了守卫的托管实现**写的
        //   （§19.2 的"锁定偏离现状"型断言）；补回原文守卫后改为断言真值 0。
        Assert.Equal(0, actor.m_nOldChrLight);
        Assert.Single(actor.MsgList);
        Assert.Equal(0x0302, actor.MsgList[0].Dir);
    }

    [Fact]
    public void SendMsg_MoveFamilyWritesChrLightAndLowByteDir()
    {
        var (scene, _, _) = MakeScene();
        var actor = MakeActor(201, 1, 1);
        scene.DoAddActor(actor);

        // cdir = 0x0504 → light = HiByte = 5，dir = LoByte = 4
        scene.SendMsg(TActorCore.SM_RUN, 201, 2, 3, 0x0504, null, 0, "");

        Assert.Equal(5, actor.m_nChrLight);
        // ★ 集成方修正（同上 D-P17-09）：普通角色不写 `m_nOldChrLight`（原文有 `is TCustomActor` 守卫）。
        Assert.Equal(0, actor.m_nOldChrLight);
        Assert.Equal(4, actor.MsgList[0].Dir);
    }

    [Fact]
    public void SendMsg_SkeletonMarksDeathAndDrawFlagForSelf()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.boCanDrawTileMap = false;

        scene.SendMsg(TActorCore.SM_SKELETON, 1, 1, 1, 0, null, 0, "");

        Assert.True(self.m_boDeath);
        Assert.True(self.m_boSkeleton);
        Assert.False(self.m_boStruckShowNumber);
        Assert.False(self.m_boShowBigHPProgress);
        Assert.False(self.m_boSendQueryBigHPProgress);
        Assert.True(scene.boCanDrawTileMap);
    }

    [Fact]
    public void SendMsg_CustomMagicMoveRangeIsAccepted()
    {
        var (scene, _, _) = MakeScene();
        var actor = MakeActor(300, 1, 1);
        scene.DoAddActor(actor);

        // SM_CUSTOM_MAGICMOVE001 + 5 落在 [11500, 11800)
        scene.SendMsg(TPlayScene.SM_CUSTOM_MAGICMOVE001 + 5, 300, 3, 4, 0x0100, null, 0, "");

        Assert.Equal(1, actor.m_nChrLight);
        Assert.Equal(0, actor.MsgList[0].Dir);
    }

    [Fact]
    public void SendMsg_NonMoveFamilyQueuesWithoutTouchingLight()
    {
        var (scene, _, _) = MakeScene();
        var actor = MakeActor(301, 1, 1);
        actor.m_nChrLight = 7;
        scene.DoAddActor(actor);

        scene.SendMsg(TActorCore.SM_HIT, 301, 2, 2, 0x0303, null, 0, "");

        Assert.Equal(7, actor.m_nChrLight);      // 未进入移动族处理
        Assert.Single(actor.MsgList);
        Assert.Equal(0x0303, actor.MsgList[0].Dir); // cdir 原样入队
    }

    // ================= SendMsg：SM_FEATURECHANGED 简装替换 =================

    [Fact]
    public void SendMsg_FeatureChangedAppliesSimpleShowScarecrow()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowActor = true;
        scene.G.ckSimpleShowActor = true;

        var actor = MakeActor(400, 1, 1);
        scene.DoAddActor(actor);
        int changed = 0;
        actor.OnFeatureChanged = () => changed++;

        var mon = new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3, HumBBType = THumBBType.bbNo };
        var f = new TFeature();
        f.SetMonFeature(mon);

        scene.SendMsg(TPlayScene.SM_FEATURECHANGED, 400, 1, 1, 0, f, 0, "");

        Assert.Same(f, actor.m_Feature);
        Assert.Equal(1, changed);
        // 默认（非自定义）换装稻草人 18/27/83
        Assert.Equal(18, mon.wRaceImg);
        Assert.Equal(27, mon.wAppr);
        Assert.Equal(83, mon.btRace);
    }

    [Fact]
    public void SendMsg_FeatureChangedGuardRaceIsExempt()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowActor = true;
        scene.G.ckSimpleShowActor = true;

        var actor = MakeActor(401, 1, 1);
        scene.DoAddActor(actor);
        int changed = 0;
        actor.OnFeatureChanged = () => changed++;

        // 大刀卫士（race 112）+ RaceImg 1 → 豁免
        var mon = new TMonFeature { wRaceImg = 1, btRace = PlaySceneConsts.RC_ARCHERGUARD, wAppr = 7 };
        var f = new TFeature();
        f.SetMonFeature(mon);

        scene.SendMsg(TPlayScene.SM_FEATURECHANGED, 401, 1, 1, 0, f, 0, "");

        Assert.Equal(1, changed);
        Assert.Equal(1, mon.wRaceImg);
        Assert.Equal(7, mon.wAppr);
        Assert.Equal(PlaySceneConsts.RC_ARCHERGUARD, mon.btRace);
    }

    [Fact]
    public void SendMsg_FeatureChangedUsesCustomSimpleShowConfig()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowActor = true;
        scene.G.ckSimpleShowActor = true;
        scene.G.boCustomActorSimpleShow = true;
        scene.G.nSimpleActorRaceImg = 9;
        scene.G.nSimpleActorAppr = 11;
        scene.G.nSimpleActorRace = 22;

        var actor = MakeActor(402, 1, 1);
        scene.DoAddActor(actor);

        var mon = new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3, HumBBType = THumBBType.bbNo };
        var f = new TFeature();
        f.SetMonFeature(mon);

        scene.SendMsg(TPlayScene.SM_FEATURECHANGED, 402, 1, 1, 0, f, 0, "");

        Assert.Equal(9, mon.wRaceImg);
        Assert.Equal(11, mon.wAppr);
        Assert.Equal(22, mon.btRace);
    }

    [Fact]
    public void SendMsg_FeatureChangedBbPathUsesSimpleShowBB()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowBB = true;
        scene.G.ckSimpleShowBB = true;
        scene.G.boCustomBBSimpleShow = true;
        scene.G.nSimpleBBSimpleRaceImg = 18;
        scene.G.nSimpleBBAppr = 27;
        scene.G.nSimpleBBRace = 83;

        var actor = MakeActor(403, 1, 1);
        scene.DoAddActor(actor);

        // HumBBType = bbSlave → 走 BB 分支
        var mon = new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3, HumBBType = THumBBType.bbSlave };
        var f = new TFeature();
        f.SetMonFeature(mon);

        scene.SendMsg(TPlayScene.SM_FEATURECHANGED, 403, 1, 1, 0, f, 0, "");

        Assert.Equal(18, mon.wRaceImg);
        Assert.Equal(27, mon.wAppr);
        Assert.Equal(83, mon.btRace);
    }

    [Fact]
    public void ApplySimpleShowSubstitution_DisableSimpleActorSkips()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowActor = true;
        scene.G.ckSimpleShowActor = true;

        var mon = new TMonFeature
        {
            wRaceImg = 50, btRace = 80, wAppr = 3,
            HumBBType = THumBBType.bbNo, IsDisableSimpleActor = true,
        };
        var f = new TFeature();
        f.SetMonFeature(mon);

        scene.ApplySimpleShowSubstitution(f);

        Assert.Equal(50, mon.wRaceImg);
        Assert.Equal(3, mon.wAppr);
    }

    [Fact]
    public void ApplySimpleShowSubstitution_DisabledConfigSkips()
    {
        var (scene, _, _) = MakeScene();
        scene.G.boSimpleShowActor = false;   // 开关关闭

        var mon = new TMonFeature { wRaceImg = 50, btRace = 80, wAppr = 3 };
        var f = new TFeature();
        f.SetMonFeature(mon);

        scene.ApplySimpleShowSubstitution(f);

        Assert.Equal(50, mon.wRaceImg);
    }

    // ================= SendMsg：SM_CHARSTATUSCHANGED =================

    [Fact]
    public void SendMsg_CharStatusChangedOnSelfWithStateBitCancelsWarMode()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        self.m_boWarMode = true;
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        int cancel = 0;
        self.OnCancelAction = () => cancel++;

        scene.SendMsg(TPlayScene.SM_CHARSTATUSCHANGED, 1, 1, 1, 0, null, 0x00080000, "");

        Assert.Equal(0x00080000, self.m_nState);
        Assert.Equal(1, cancel);
        Assert.False(self.m_boWarMode);
    }

    [Fact]
    public void SendMsg_CharStatusChangedWithoutBitKeepsWarMode()
    {
        var (scene, _, _) = MakeScene();
        var self = MakeActor(1, 1, 1);
        self.m_boWarMode = true;
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        int cancel = 0;
        self.OnCancelAction = () => cancel++;

        scene.SendMsg(TPlayScene.SM_CHARSTATUSCHANGED, 1, 1, 1, 0, null, 0x00000001, "");

        Assert.Equal(0, cancel);
        Assert.True(self.m_boWarMode);
        Assert.Empty(self.MsgList);   // 状态变更不入消息队列
    }

    [Fact]
    public void SendMsg_CharStatusChangedOnOtherActorOnlySetsState()
    {
        var (scene, _, _) = MakeScene();
        var other = MakeActor(500, 1, 1);
        other.m_boWarMode = true;
        scene.DoAddActor(other);

        scene.SendMsg(TPlayScene.SM_CHARSTATUSCHANGED, 500, 1, 1, 0, null, 0x04000000, "");

        Assert.Equal(0x04000000, other.m_nState);
        Assert.True(other.m_boWarMode);   // 非主角不处理
    }

    // ================= SendMsg：近身好友/敌人提示 =================

    [Fact]
    public void SendMsg_NearFriendHintAddedOnceAndRemovedWhenLeaving()
    {
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10, "自己");
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        var friend = MakeActor(600, 12, 12, "好友甲");   // race 0，≤4 格
        scene.DoAddActor(friend);
        frm.FriendMemo.Add("好友甲");
        scene.ckFriendHit = true;

        scene.SendMsg(TActorCore.SM_WALK, 600, 12, 12, 2, null, 0, "");
        Assert.Single(frm.ChatLines);
        Assert.Contains("好友甲", frm.ChatLines[0]);
        Assert.Contains("你的好友", frm.ChatLines[0]);

        // 第二次同位置：名单已有 → 不重复提示
        scene.SendMsg(TActorCore.SM_WALK, 600, 12, 12, 2, null, 0, "");
        Assert.Single(frm.ChatLines);

        // 出圈（>4 格）→ 移出名单
        friend.m_nCurrX = 20;
        friend.m_nCurrY = 20;
        scene.SendMsg(TActorCore.SM_WALK, 600, 20, 20, 2, null, 0, "");
        Assert.Empty(self.m_FriendHitList);

        // 回圈可再次提示
        friend.m_nCurrX = 12;
        friend.m_nCurrY = 12;
        scene.SendMsg(TActorCore.SM_WALK, 600, 12, 12, 2, null, 0, "");
        Assert.Equal(2, frm.ChatLines.Count);
    }

    [Fact]
    public void SendMsg_NearBlacklistHintUsesEnemyText()
    {
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10, "自己");
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        var enemy = MakeActor(601, 11, 11, "敌人乙");
        scene.DoAddActor(enemy);
        frm.BlacklistMemo.Add("敌人乙");
        scene.ckBlacklistHit = true;

        scene.SendMsg(TActorCore.SM_RUN, 601, 11, 11, 2, null, 0, "");

        Assert.Single(frm.ChatLines);
        Assert.Contains("你的敌人", frm.ChatLines[0]);
    }

    [Fact]
    public void SendMsg_NearNonHumanRaceIsNotTracked()
    {
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10, "自己");
        scene.G.MySelf = self;
        scene.DoAddActor(self);

        var mon = MakeActor(602, 11, 11, "怪物", race: 80);
        scene.DoAddActor(mon);
        scene.ckFriendHit = true;
        frm.FriendMemo.Add("怪物");

        scene.SendMsg(TActorCore.SM_WALK, 602, 11, 11, 2, null, 0, "");

        Assert.Empty(frm.ChatLines);       // 非 race 0 不提示
        Assert.Empty(self.m_FriendHitList);
    }

    // ================= SendMsg：近身开盾 =================

    [Fact]
    public void SendMsg_NearStruckShieldUsesJobSpecificMagicWithinThrottle()
    {
        SceneTime.TickNow = () => 1000;
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_btJob = 1;             // 法师 → 魔法盾 31
        self.m_nAbilMP = 100;
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.clientConfig_boHumStruckShield = true;
        scene.ckHumStruckShield = true;
        scene.MagicLookupFn = id => id == 31 ? (31, 5, 3) : null;

        scene.SendMsg(TActorCore.SM_WALK, 1, 12, 10, 2, null, 0, "");

        Assert.Single(frm.UsedMagics);
        Assert.Equal(31, frm.UsedMagics[0].MagicId);
        Assert.Equal(1000u, scene.g_dwHumStruckShieldTick);

        // 600ms 节流内再触发 → 不再施放
        scene.SendMsg(TActorCore.SM_WALK, 1, 12, 10, 2, null, 0, "");
        Assert.Single(frm.UsedMagics);
    }

    [Fact]
    public void SendMsg_NearStruckShieldSkipsWhenMpInsufficient()
    {
        SceneTime.TickNow = () => 1000;
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_btJob = 0;             // 战士 → 87
        self.m_nAbilMP = 5;
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.clientConfig_boHumStruckShield = true;
        scene.ckHumStruckShield = true;
        scene.MagicLookupFn = id => id == 87 ? (87, 10, 8) : null;

        scene.SendMsg(TActorCore.SM_WALK, 1, 11, 10, 2, null, 0, "");

        Assert.Empty(frm.UsedMagics);   // 10+8 > 5
    }

    [Fact]
    public void SendMsg_NearStruckShieldFallsBackToNewShieldMagic()
    {
        SceneTime.TickNow = () => 1000;
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_btJob = 2;             // 道士 → 73 无、回退 89
        self.m_nAbilMP = 500;
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.clientConfig_boHumStruckShield = true;
        scene.ckHumStruckShield = true;
        scene.MagicLookupFn = id => id == 89 ? (89, 5, 5) : null;

        scene.SendMsg(TActorCore.SM_WALK, 1, 12, 11, 2, null, 0, "");

        Assert.Single(frm.UsedMagics);
        Assert.Equal(89, frm.UsedMagics[0].MagicId);
    }

    [Fact]
    public void SendMsg_NearStruckShieldSkipsWhenStateBitAlreadySet()
    {
        SceneTime.TickNow = () => 1000;
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_btJob = 1;
        self.m_nAbilMP = 500;
        self.m_nState = 0x00100000;   // 已有盾
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.clientConfig_boHumStruckShield = true;
        scene.ckHumStruckShield = true;
        scene.MagicLookupFn = id => id == 31 ? (31, 5, 5) : null;

        scene.SendMsg(TActorCore.SM_WALK, 1, 12, 10, 2, null, 0, "");

        Assert.Empty(frm.UsedMagics);
    }

    [Fact]
    public void SendMsg_NearStruckShieldRequiresConfigBothSides()
    {
        SceneTime.TickNow = () => 1000;
        var (scene, _, frm) = MakeScene();
        var self = MakeActor(1, 10, 10);
        self.m_btJob = 1;
        self.m_nAbilMP = 500;
        scene.G.MySelf = self;
        scene.DoAddActor(self);
        scene.clientConfig_boHumStruckShield = true;
        scene.ckHumStruckShield = false;               // 配置复选未勾
        scene.MagicLookupFn = id => id == 31 ? (31, 5, 5) : null;

        scene.SendMsg(TActorCore.SM_WALK, 1, 12, 10, 2, null, 0, "");

        Assert.Empty(frm.UsedMagics);
    }

    // ================= ClearDropItem =================

    private static (TPlayScene Scene, DropItemsStore Store) MakeDropScene()
    {
        var map = new FakeMap();
        var scene = new TPlayScene(new SceneDialogs(), map);
        scene.FrmMain = new FakeFrmMain();
        scene.G.MySelf = MakeActor(1, 1, 1);
        var store = new DropItemsStore(_ => null);
        scene.DropItems = store;
        return (scene, store);
    }

    [Fact]
    public void ClearDropItem_NoSelfIsNoOp()
    {
        var (scene, store) = MakeDropScene();
        scene.G.MySelf = null;
        store.AddDropItem(1, 1, 1, "药水", 0);

        scene.ClearDropItem();

        Assert.Null(store.GetItemById(1)!.ItemTexture);
    }

    [Fact]
    public void ClearDropItem_FlashWindowOpensAndClosesAfterTenSteps()
    {
        var (scene, store) = MakeDropScene();
        var now = 100000u;
        SceneTime.TickNow = () => now;
        var item = store.AddDropItem(1, 1, 1, "药水", 0);

        // 首次调用：now - FlashTime > 5000（FlashTime 初值 0）→ 开闪并把 FlashStep 清零
        scene.ClearDropItem();
        Assert.True(item.ShowFlash);
        Assert.Equal(0, item.FlashStep);
        Assert.Equal(now, item.FlashTime);

        // 每 20ms 推进一步；10 步后关闪
        for (int i = 0; i < 10; i++)
        {
            now += 20;
            scene.ClearDropItem();
        }
        Assert.Equal(10, item.FlashStep);
        Assert.False(item.ShowFlash);

        // FlashTime 被刷新到开闪时刻，窗口内不再重开
        now += 100;
        scene.ClearDropItem();
        Assert.False(item.ShowFlash);
    }

    [Fact]
    public void ClearDropItem_SpecialItemUses300msFlashWindow()
    {
        var (scene, store) = MakeDropScene();
        var now = 100000u;
        SceneTime.TickNow = () => now;
        scene.ckSpecialQuickFlashing = true;

        var item = store.AddDropItem(1, 1, 1, "特戒", 0);
        item.ShowItem = new ShowItemInfo { ShowName = true, ShowSpecial = true };

        scene.ClearDropItem();
        Assert.True(item.ShowFlash);
        Assert.Equal(now, item.FlashTime);

        // 350ms 后（> 300ms 窗口）重开闪烁
        now += 350;
        scene.ClearDropItem();
        Assert.Equal(now, item.FlashTime);
    }

    [Fact]
    public void ClearDropItem_TextureSelectionFollowsDeathAndFocus()
    {
        var (scene, store) = MakeDropScene();
        var probes = new List<int>();
        scene.DropItemTextureFn = (_, kind) =>
        {
            probes.Add(kind);
            return new SurfaceSize(kind, kind);
        };

        var a = store.AddDropItem(1, 1, 1, "甲", 0);
        var b = store.AddDropItem(2, 2, 2, "乙", 0);

        scene.ClearDropItem();
        Assert.Equal(new[] { 0, 0 }, probes);            // 常态取 Images

        probes.Clear();
        scene.FocusItem = b;
        scene.ClearDropItem();
        Assert.Contains(1, probes);                     // 焦点物品取 Brights
        Assert.Same(b, scene.OldFocusItem);

        probes.Clear();
        scene.G.MySelf!.m_boDeath = true;
        scene.ClearDropItem();
        Assert.All(probes, k => Assert.Equal(2, k));    // 主角死亡全部取 Grays
        Assert.Same(a, a);
    }

    [Fact]
    public void ClearDropItem_OldFocusItemClearsWhenFocusLost()
    {
        var (scene, store) = MakeDropScene();
        var a = store.AddDropItem(1, 1, 1, "甲", 0);

        scene.FocusItem = a;
        scene.ClearDropItem();
        Assert.Same(a, scene.OldFocusItem);

        // g_FocusItem 清空且 OldFocusItem 指向本项 → OldFocusItem 归零
        scene.FocusItem = null;
        scene.ClearDropItem();
        Assert.Null(scene.OldFocusItem);
    }

    [Fact]
    public void ClearDropItem_NameImageGeneratedAndClearedByNameFlag()
    {
        var (scene, store) = MakeDropScene();
        scene.NameImageFn = _ => (60, 14);
        scene.PlugInEnabled = true;

        var item = store.AddDropItem(1, 1, 1, "屠龙", 0);
        item.ShowItem = new ShowItemInfo { ShowName = true };

        scene.ClearDropItem();
        Assert.True(item.ShowName);
        Assert.True(item.NameImageGenerated);
        Assert.Equal(60, item.NameImageWidth);
        Assert.Equal(14, item.NameImageHeight);

        // 关闭内挂 → ShowName 变 false → 名字图清除
        scene.PlugInEnabled = false;
        scene.ClearDropItem();
        Assert.False(item.ShowName);
        Assert.False(item.NameImageGenerated);
    }

    [Fact]
    public void ClearDropItem_OverlapCountFormatsOldOrNewStyle()
    {
        var (scene, store) = MakeDropScene();
        var captured = new List<string>();
        scene.NameImageFn = s =>
        {
            captured.Add(s);
            return (20, 10);
        };
        scene.PlugInEnabled = true;
        scene.ShowAllItem = true;

        var item = store.AddDropItem(1, 1, 1, "药水", 0);
        item.Name = "药水";
        item.OverlapCount = 3;

        scene.ClearDropItem();
        Assert.Equal(new[] { "药水x3" }, captured);

        // 旧格式：NameImageFn 只对尚未生成名字图的项调用一次，故换新项验证
        captured.Clear();
        scene.OverlapItemNumOldShow = true;
        var item2 = store.AddDropItem(2, 2, 2, "药水", 0);
        item2.Name = "药水";
        item2.OverlapCount = 3;
        scene.ClearDropItem();
        Assert.Contains("药水 (3)", captured);
    }

    [Fact]
    public void ClearDropItem_SkipsEmptyPointLists()
    {
        var (scene, store) = MakeDropScene();
        int calls = 0;
        scene.DropItemTextureFn = (_, _) =>
        {
            calls++;
            return null;
        };

        store.AddDropItem(1, 1, 1, "甲", 0);
        store.DelDropItem(1);           // 点表清空

        scene.ClearDropItem();
        Assert.Equal(0, calls);
    }

    // ================= 辅助函数 =================

    [Fact]
    public void HiByteLoByte_SplitPackedValue()
    {
        Assert.Equal(5, TPlayScene.HiByte(0x0504));
        Assert.Equal(4, TPlayScene.LoByte(0x0504));
        Assert.Equal(0, TPlayScene.HiByte(0x00FF));
        Assert.Equal(0xFF, TPlayScene.LoByte(0x00FF));
    }

    [Fact]
    public void DirsStr_HasEightDirections()
    {
        Assert.Equal(8, TPlayScene.DirsStr.Length);
        Assert.Equal("上", TPlayScene.DirsStr[0]);
        Assert.Equal("左上", TPlayScene.DirsStr[7]);
    }

    [Fact]
    public void ClearDropItemName_ResetsNameImage()
    {
        var (_, store) = MakeDropScene();
        var item = store.AddDropItem(1, 1, 1, "甲", 0);
        item.SetNameImage(60, 14);

        TPlayScene.ClearDropItemName(item);

        Assert.False(item.NameImageGenerated);
        Assert.Equal(0, item.NameImageWidth);
        Assert.True(item.NameImageCleared);
    }
}

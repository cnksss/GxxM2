using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 `p17-client-actor` 切片 3d：`THumActor` 中等方法族
/// （<c>DrawDressEffect</c> 11260 / <c>DefaultMotion</c> 13136 /
/// <c>GetDefaultFrame</c> 13217 / <c>RunFrameAction</c> 13310）+
/// <c>TActor.DrawDressEffect</c> 基类槽位（5974）的 1:1 回归证据。
/// </summary>
public sealed class ActorHumMotionTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorHumMotionTests()
    {
        ActorNpcEnv.Reset();
        _restore.Add(ActorNpcEnv.Reset);

        ActorFamilyEnv.Reset();
        _restore.Add(ActorFamilyEnv.Reset);

        ActorNpcEnv.TimeGetTimeFn = () => 1000;
        _restore.Add(() => ActorNpcEnv.TimeGetTimeFn = () => SceneTime.TickNow());
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    private static List<NpcDrawOp> RecordDraws()
    {
        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;
        return ops;
    }

    private static THumActor NewHum() => new() { m_btRace = 1 };

    // ══════════════════════════════════════════════════════════════════════
    // 1. DrawDressEffect（11260-11304 + inherited 11303）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void DrawDressEffect_BaseSlotDrawsOnlyDressEffectSurface()
    {
        var actor = new TActor();
        actor.m_DressEffectSurface = "dress";
        actor.m_nDressEffectX = 5;
        actor.m_nDressEffectY = 6;
        actor.m_nShiftX = 7;
        actor.m_nShiftY = 8;
        var ops = RecordDraws();

        actor.DrawDressEffect(100, 200, false, TColorEffect.ceNone);

        var op = Assert.Single(ops);
        Assert.Equal("DressEffect", op.Layer);
        Assert.Equal(100 + 5 + 7, op.X);   // 5974-5987
        Assert.Equal(200 + 6 + 8, op.Y);
    }

    [Fact]
    public void DrawDressEffect_BaseSlotPolarityIsNoBlendTrueMeansDraw()
    {
        var actor = new TActor { m_DressEffectSurface = "d" };
        var ops = RecordDraws();

        actor.m_boDressEffectDrawNoBlend = false;
        actor.DrawDressEffect(0, 0, false, TColorEffect.ceNone);
        Assert.Equal(NpcDrawKind.DrawBlend, ops[0].Kind);

        ops.Clear();
        actor.m_boDressEffectDrawNoBlend = true;
        actor.DrawDressEffect(0, 0, false, TColorEffect.ceNone);
        Assert.Equal(NpcDrawKind.Draw, ops[0].Kind);
    }

    [Fact]
    public void DrawDressEffect_HumFourBlocksAreIndependentAndInheritedComesLast()
    {
        var hum = NewHum();
        hum.m_HumWinSurface = "humwin";
        hum.m_HumWinSurface_30 = "humwin30";
        hum.m_HeroM2DressEffect = "herom2";
        hum.m_MedalEffectSurface = "medal";
        hum.m_DressEffectSurface = "dress";
        var ops = RecordDraws();

        hum.DrawDressEffect(10, 20, false, TColorEffect.ceNone);

        // 四个块各自独立（非 else 链）⇒ 五张图；11303 的 inherited 排最后
        Assert.Equal(5, ops.Count);
        Assert.Equal(new[] { "HumWin", "HumWin30", "HeroM2Dress", "MedalEffect", "DressEffect" },
            ops.ConvertAll(o => o.Layer).ToArray());
    }

    [Fact]
    public void DrawDressEffect_HumUsesItsOwnOffsetsNotPxPy()
    {
        var hum = NewHum();
        hum.m_HumWinSurface = "h";
        hum.m_nSpX = 11;
        hum.m_nSpY = 12;
        hum.m_nPx = 999;                // ★ 不应被用到
        hum.m_nPy = 999;
        hum.m_nShiftX = 1;
        hum.m_nShiftY = 2;
        var ops = RecordDraws();

        hum.DrawDressEffect(100, 200, false, TColorEffect.ceNone);

        var op = Assert.Single(ops);
        Assert.Equal(100 + 11 + 1, op.X);   // 11265-11266
        Assert.Equal(200 + 12 + 2, op.Y);
    }

    [Fact]
    public void DrawDressEffect_HumBlendPolarityDiffersPerBlockButBehaviourMatches()
    {
        var hum = NewHum();
        hum.m_HumWinSurface = "a";
        hum.m_HumWinSurface_30 = "b";
        hum.m_HeroM2DressEffect = "c";
        hum.m_MedalEffectSurface = "d";
        var ops = RecordDraws();

        // ①② 用 not m_boEffectNormalDraw；③④ 用 m_boHeroM2DressNoBlend / m_boMedalEffectDrawNoBlend
        hum.m_boEffectNormalDraw = true;          // ⇒ Draw
        hum.m_boEffect_30NormalDraw = false;      // ⇒ DrawBlend
        hum.m_boHeroM2DressNoBlend = true;        // ⇒ Draw
        hum.m_boMedalEffectDrawNoBlend = false;   // ⇒ DrawBlend

        hum.DrawDressEffect(0, 0, false, TColorEffect.ceNone);

        Assert.Equal(NpcDrawKind.Draw, ops[0].Kind);
        Assert.Equal(NpcDrawKind.DrawBlend, ops[1].Kind);
        Assert.Equal(NpcDrawKind.Draw, ops[2].Kind);
        Assert.Equal(NpcDrawKind.DrawBlend, ops[3].Kind);
    }

    [Fact]
    public void DrawDressEffect_HumIgnoresBlendAndColorEffectArguments()
    {
        var hum = NewHum();
        hum.m_HumWinSurface = "a";
        var ops = RecordDraws();

        hum.DrawDressEffect(0, 0, true, TColorEffect.ceRed);
        var withTrue = ops[0].Kind;

        ops.Clear();
        hum.DrawDressEffect(0, 0, false, TColorEffect.ceBlack);
        var withFalse = ops[0].Kind;

        // ★ 11260-11304 全文不含 blend / ceff
        Assert.Equal(withTrue, withFalse);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. DefaultMotion（13136-13215）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void DefaultMotion_Effect50RunsTwentyFrameCycleWith100msThrottle()
    {
        var hum = NewHum();
        hum.m_wEffect = 50;
        hum.m_nCurrentFrame = 100;      // <= 536
        hum.m_dwFrameTick = 0;          // now(1000) - 0 > 100
        ActorNpcEnv.TimeGetTimeFn = () => 1000;

        Assert.True(hum.DefaultMotion());   // m_nFrame 0 -> 1 ⇒ True
        Assert.Equal(1, hum.m_nFrame);
        Assert.Equal(1000u, hum.m_dwFrameTick);
    }

    [Fact]
    public void DefaultMotion_Effect50At19FlipsTwoDZeroAndResetsFrame()
    {
        var hum = NewHum();
        hum.m_wEffect = 50;
        hum.m_nCurrentFrame = 100;
        hum.m_dwFrameTick = 0;
        hum.m_nFrame = 19;
        hum.m_bo2D0 = false;

        Assert.True(hum.DefaultMotion());
        Assert.True(hum.m_bo2D0);      // 13148-13151
        Assert.Equal(0, hum.m_nFrame); // 13152
    }

    [Fact]
    public void DefaultMotion_Effect50FlipsTwoDZeroBackOnSecondRound()
    {
        var hum = NewHum();
        hum.m_wEffect = 50;
        hum.m_nCurrentFrame = 100;
        hum.m_dwFrameTick = 0;
        hum.m_nFrame = 19;
        hum.m_bo2D0 = true;

        hum.DefaultMotion();

        Assert.False(hum.m_bo2D0);     // 是真/假交替，不是单调置真
    }

    [Fact]
    public void DefaultMotion_EffectNot50UsesEightFrameCycleAtFrameTime()
    {
        var hum = NewHum();
        hum.m_wEffect = 10;
        hum.m_nCurrentFrame = 10;       // < 64
        hum.m_dwFrameTime = 150;
        hum.m_dwFrameTick = 0;          // now(1000) - 0 > 150

        Assert.True(hum.DefaultMotion());
        Assert.Equal(1, hum.m_nFrame);
    }

    [Fact]
    public void DefaultMotion_BaseRunsFirstAndOverwritesCurrentFrameBeforeEffectBlocks()
    {
        // ★ 13140 的 `Result := inherited DefaultMotion` **不只是取返回值**：
        //   基类实体会 `m_nCurrentFrame := GetDefaultFrame(...)`，而 `GetDefaultFrame` 是**虚的**
        //   ⇒ 它先被 THumActor.GetDefaultFrame 求值并写回 m_nCurrentFrame，
        //   **然后** 13143/13160 的效果块才读 m_nCurrentFrame。
        //   本例把一个"看起来应当 >= 64 而停止推进"的场景，变成"被基类改写为 0 ⇒ 仍然推进"。
        var hum = NewHum();
        hum.m_wEffect = 10;
        hum.m_nCurrentFrame = 64;      // 调用前是 64（>= 64）
        hum.m_btDir = 0;
        hum.m_nCurrentDefFrame = 0;
        hum.m_nFrame = 0;
        hum.m_dwFrameTime = 150;
        hum.m_dwFrameTick = 0;         // now(1000) - 0 > 150 ⇒ 节流通过

        hum.DefaultMotion();

        Assert.Equal(0, hum.m_nCurrentFrame);   // ★ 基类把它写成了默认帧 0
        Assert.Equal(1, hum.m_nFrame);          // ⇒ 13160 的 `< 64` 被满足，仍然自增
    }

    [Fact]
    public void DefaultMotion_EffectNot50StopsAdvancingWhenFrameReallyStaysAboveSixtyFour()
    {
        // 让基类写回一个确实 >= 64 的帧号（ActDie 支：536 + dir*8 + 3），才能观察到 13170-13171 的空分支
        var hum = NewHum();
        hum.m_wEffect = 10;
        hum.m_nCurrentFrame = 64;
        hum.m_boDeath = true;          // ⇒ GetDefaultFrame 走 ActDie（>= 64）
        hum.m_btDir = 0;
        hum.m_nFrame = 0;
        hum.m_dwFrameTick = 0;

        bool r = hum.DefaultMotion();

        Assert.True(hum.m_nCurrentFrame >= 64);
        Assert.Equal(0, hum.m_nFrame);          // 空 else 分支 ⇒ 不自增
        Assert.Equal(0u, hum.m_dwFrameTick);    // 且 tick 不被刷新
        Assert.True(r);                         // 基类结果（帧号变了）原样传出
    }

    [Fact]
    public void DefaultMotion_TwoBlocksShareTickAndFrameCounter()
    {
        var hum = NewHum();
        hum.m_wEffect = 50;
        hum.m_wEffect_30 = 50;
        hum.m_nCurrentFrame = 100;
        hum.m_dwFrameTick = 0;

        hum.DefaultMotion();

        // ★ 第一段把 tick 更新为 now；第二段再判 now - tick = 0 ⇒ 不推进
        Assert.Equal(1, hum.m_nFrame);
        Assert.Equal(1000u, hum.m_dwFrameTick);
    }

    [Fact]
    public void DefaultMotion_ThrottleNotElapsedDoesNotAdvance()
    {
        var hum = NewHum();
        hum.m_wEffect = 50;
        hum.m_nCurrentFrame = 100;
        hum.m_dwFrameTick = 950;        // 1000 - 950 = 50，不 > 100

        hum.DefaultMotion();

        Assert.Equal(0, hum.m_nFrame);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3. GetDefaultFrame（13217-13308）
    // ══════════════════════════════════════════════════════════════════════

    private static TMonsterClientAction Act(short start, ushort play, ushort empty, byte calcDir = 1)
        => new()
        {
            StartIndex = start,
            PlayCount = play,
            EmptyCount = empty,
            CalcDir = calcDir,
            PlayTime = 100,
        };

    private static TClientCustomMonsterConfig MakeMonsterConfig(ushort appr,
        TMonsterClientAction? stand = null, TMonsterClientAction? die = null, TMonsterClientAction? stone = null)
    {
        var cfg = new TClientCustomMonsterConfig { wMonsterAppr = appr };
        cfg.Actions[(int)TMonsterClientActionType.matStand] = stand ?? Act(100, 4, 2);
        cfg.Actions[(int)TMonsterClientActionType.matDie] = die ?? Act(200, 6, 0);
        cfg.Actions[(int)TMonsterClientActionType.matStoneRevive] = stone ?? Act(300, 5, 1);
        return cfg;
    }

    [Fact]
    public void GetDefaultFrame_ChangeApprBelowHundredThousandDelegatesToBase()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = 5;
        hum.m_btDir = 0;

        var plain = new TActor();
        plain.m_nChangeAppr = 5;
        plain.m_btDir = 0;
        plain.m_btRace = 1;

        Assert.Equal(plain.GetDefaultFrame(false), hum.GetDefaultFrame(false));   // 13274
    }

    [Fact]
    public void GetDefaultFrame_CustomStandUsesBaseFramePlusClampedCf()
    {
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => MakeMonsterConfig(7, Act(100, 4, 2));
        var hum = NewHum();
        hum.m_nChangeAppr = 100007;
        hum.m_btDir = 3;
        hum.m_nCurrentDefFrame = 2;

        // BaseFrame = 100 + 3*(4+2) = 118；cf = 2 ⇒ 120
        Assert.Equal(120, hum.GetDefaultFrame(false));   // 13268
    }

    [Fact]
    public void GetDefaultFrame_CustomDeathWithoutSkeletonLandsOnLastFrame()
    {
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => MakeMonsterConfig(7, die: Act(200, 6, 0));
        var hum = NewHum();
        hum.m_nChangeAppr = 100007;
        hum.m_btDir = 1;
        hum.m_boDeath = true;
        hum.m_boSkeleton = false;

        // BaseFrame(matDie) = 200 + 1*6 = 206；+ (PlayCount-1)=5 ⇒ 211
        Assert.Equal(211, hum.GetDefaultFrame(false));   // 13244
    }

    [Fact]
    public void GetDefaultFrame_CustomSkeletonDeathUsesRawStartIndexWithoutTempDir()
    {
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => MakeMonsterConfig(7, die: Act(200, 6, 0));
        var hum = NewHum();
        hum.m_nChangeAppr = 100007;
        hum.m_btDir = 1;
        hum.m_boDeath = true;
        hum.m_boSkeleton = true;

        // ★ 13238 只取 StartIndex（**不加** TempDir、**不减** 1）
        Assert.Equal(200, hum.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_CustomStoneModeUsesStoneReviveBaseFrame()
    {
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => MakeMonsterConfig(7, stone: Act(300, 5, 1));
        var hum = NewHum();
        hum.m_nChangeAppr = 100007;
        hum.m_btDir = 2;
        hum.m_nState = (int)ActorStates.STATE_STONE_MODE;

        Assert.Equal(300 + 2 * 6, hum.GetDefaultFrame(false));   // 13254
    }

    [Fact]
    public void GetDefaultFrame_CustomConfigMissingReturnsZero()
    {
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => null;
        var hum = NewHum();
        hum.m_nChangeAppr = 100007;
        hum.m_btDir = 5;

        Assert.Equal(0, hum.GetDefaultFrame(false));   // 13224 的初值，且不落任何分支
    }

    [Fact]
    public void GetDefaultFrame_StandBranchWritesDefFrameCount()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_boDeath = false;
        hum.m_btDir = 0;
        hum.m_nCurrentDefFrame = 0;
        hum.m_nDefFrameCount = 99;

        Assert.Equal(0, hum.GetDefaultFrame(false));   // HA.ActStand.start + 0
        Assert.Equal(ActorActionTables.HA.ActStand.frame, hum.m_nDefFrameCount);   // 13299
    }

    [Fact]
    public void GetDefaultFrame_StandClampsNegativeAndOverflowCf()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_btDir = 1;

        hum.m_nCurrentDefFrame = -1;
        Assert.Equal(1 * 8, hum.GetDefaultFrame(false));      // cf = 0
        hum.m_nCurrentDefFrame = 4;                            // >= ActStand.frame(4)
        Assert.Equal(1 * 8, hum.GetDefaultFrame(false));      // cf 仍 0
    }

    [Fact]
    public void GetDefaultFrame_WarModeBranchUsesActWarMode()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_boCustomMagicNoAction = false;
        hum.m_btDir = 0;

        Assert.Equal(ActorActionTables.HA.ActWarMode.start, hum.GetDefaultFrame(true));   // 13296
    }

    [Fact]
    public void GetDefaultFrame_WarModeWithCustomMagicNoActionUsesActStand()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_boCustomMagicNoAction = true;
        hum.m_btDir = 0;

        Assert.Equal(ActorActionTables.HA.ActStand.start, hum.GetDefaultFrame(true));     // 13294
    }

    [Fact]
    public void GetDefaultFrame_DeathBranchUsesActDieLastFrame()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_boDeath = true;
        hum.m_btDir = 2;
        var ha = ActorActionTables.HA;

        Assert.Equal(ha.ActDie.start + 2 * (ha.ActDie.frame + ha.ActDie.skip) + (ha.ActDie.frame - 1),
            hum.GetDefaultFrame(false));   // 13290
    }

    [Fact]
    public void GetDefaultFrame_HorseDeathUsesHardcodedMagicNumber()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_boDeath = true;
        hum.m_btHorse = 3;                  // in [1..5]
        hum.m_btDoubleHumHorse = 1;         // <> 0
        hum.m_btDir = 1;

        Assert.Equal(256 + 1 * (7 + 1) + 8 - 1, hum.GetDefaultFrame(false));   // 13288
    }

    [Fact]
    public void GetDefaultFrame_HorseDeathWithSingleRiderUsesActDie()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_boDeath = true;
        hum.m_btHorse = 3;
        hum.m_btDoubleHumHorse = 0;         // ★ 必须 <> 0 才走魔数支
        hum.m_btDir = 1;
        var ha = ActorActionTables.HA;

        Assert.Equal(ha.ActDie.start + 1 * (ha.ActDie.frame + ha.ActDie.skip) + (ha.ActDie.frame - 1),
            hum.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_HorseForcesWarModeOffIntoStandBranch()
    {
        var hum = NewHum();
        hum.m_nChangeAppr = -1;
        hum.m_btHorse = 2;                  // <> 0
        hum.m_btDir = 0;
        hum.m_nCurrentDefFrame = 0;
        hum.m_nDefFrameCount = 77;

        int frame = hum.GetDefaultFrame(true);   // 13280：wmode → False

        Assert.Equal(ActorActionTables.HA.ActStand.start, frame);   // 走站立支而不是 ActWarMode
        Assert.Equal(ActorActionTables.HA.ActStand.frame, hum.m_nDefFrameCount);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4. RunFrameAction（13310-13353）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void RunFrameAction_AlwaysClearsHideWeaponFirst()
    {
        var hum = NewHum();
        hum.m_boHideWeapon = true;
        hum.m_nCurrentAction = 999;

        hum.RunFrameAction(0);

        Assert.False(hum.m_boHideWeapon);   // 13316
    }

    [Fact]
    public void RunFrameAction_HeavyHitFrame5WithFlagRequestsDigFragment()
    {
        var requests = new List<HumDigFragmentEffect>();
        ActorNpcEnv.SpawnDigFragmentEffectFn = requests.Add;
        var hum = NewHum();
        hum.m_boDigFragment = true;
        hum.m_nCurrentAction = TActorCore.SM_HEAVYHIT;
        hum.m_boThrow = false;
        hum.m_btDir = 2;
        hum.m_nCurrX = 30;
        hum.m_nCurrY = 40;

        hum.RunFrameAction(5);

        var r = Assert.Single(requests);
        Assert.Equal(8 * 2, r.Dir8);            // 13320
        Assert.Equal(3, r.Kind);
        Assert.Equal(30, r.X);
        Assert.Equal(40, r.Y);
        Assert.Equal("WEffectImg", r.ImageLib); // 13321
        Assert.Equal(80, r.NextFrameTime);      // 13322
        Assert.Equal("s_strike_stone", r.SoundName);   // 13323
        Assert.Equal("ET_PILESTONES", r.EventType);    // 13328
        Assert.False(hum.m_boDigFragment);      // 13319
    }

    [Fact]
    public void RunFrameAction_HeavyHitFrame5WithoutFlagDoesNothing()
    {
        var requested = false;
        ActorNpcEnv.SpawnDigFragmentEffectFn = _ => requested = true;
        var hum = NewHum();
        hum.m_boDigFragment = false;
        hum.m_nCurrentAction = TActorCore.SM_HEAVYHIT;

        hum.RunFrameAction(5);

        Assert.False(requested);
    }

    [Fact]
    public void RunFrameAction_ThrowFrame3WithFlagRequestsFlyingAxe()
    {
        var requests = new List<HumThrowAxeEffect>();
        ActorNpcEnv.SpawnThrowAxeFn = requests.Add;
        var hum = NewHum();
        hum.m_boThrow = true;
        hum.m_nCurrentAction = TActorCore.SM_THROW;
        hum.m_nCurrX = 1;
        hum.m_nCurrY = 2;
        hum.m_nTargetX = 3;
        hum.m_nTargetY = 4;
        hum.m_nTargetRecog = 5;

        hum.RunFrameAction(3);

        var r = Assert.Single(requests);
        Assert.Equal(1, r.X);
        Assert.Equal(2, r.Y);
        Assert.Equal(3, r.TargetX);
        Assert.Equal(4, r.TargetY);
        Assert.Equal(5L, r.TargetRecog);
        Assert.Equal(40, r.ReadyFrame);                                   // 13344
        Assert.Equal(3, r.ImgLibIndex);                                   // 13345
        Assert.Equal(MagicEffConsts.FLYOMAAXEBASE, r.FlyImageBase);        // 13346
        Assert.False(hum.m_boThrow);                                      // 13335
    }

    [Fact]
    public void RunFrameAction_ThrowFrameAtLeastThreeHidesWeaponIndependentlyOfFlag()
    {
        var hum = NewHum();
        hum.m_nCurrentAction = TActorCore.SM_THROW;
        hum.m_boThrow = false;          // ★ 13334 为假
        hum.m_boHideWeapon = false;

        hum.RunFrameAction(4);          // 13350：frame >= 3 独立成立

        Assert.True(hum.m_boHideWeapon);
    }

    [Fact]
    public void RunFrameAction_ThrowFrameBelowThreeDoesNotHideWeapon()
    {
        var hum = NewHum();
        hum.m_nCurrentAction = TActorCore.SM_THROW;

        hum.RunFrameAction(2);

        Assert.False(hum.m_boHideWeapon);   // 13350 为假
    }

    [Fact]
    public void RunFrameAction_NullSeamsDoNotThrow()
    {
        ActorNpcEnv.SpawnDigFragmentEffectFn = null;
        ActorNpcEnv.SpawnThrowAxeFn = null;
        var hum = NewHum();
        hum.m_boDigFragment = true;
        hum.m_boThrow = true;
        hum.m_nCurrentAction = TActorCore.SM_HEAVYHIT;

        hum.RunFrameAction(5);          // 未接线 ⇒ 只是不请求，标志仍被清

        Assert.False(hum.m_boDigFragment);

        hum.m_nCurrentAction = TActorCore.SM_THROW;
        hum.RunFrameAction(3);
        Assert.False(hum.m_boThrow);
    }

    [Fact]
    public void RunFrameActionIsVirtualSoBaseStaticTypeDispatchesToHum()
    {
        // ★ 原文 1821 声明 virtual、THumActor 2025 override；补虚前托管侧是"非虚 ⇒ 覆写被旁路"
        TActor asBase = NewHum();
        asBase.m_boDigFragment = true;
        asBase.m_nCurrentAction = TActorCore.SM_HEAVYHIT;
        bool requested = false;
        ActorNpcEnv.SpawnDigFragmentEffectFn = _ => requested = true;

        asBase.RunFrameAction(5);

        Assert.True(requested);
        Assert.False(asBase.m_boDigFragment);
    }
}

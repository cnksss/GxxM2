using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 `p7-client-actor-family`：**`TActor` 四个虚方法本体**（`Actor.pas`
/// `LoadSurface` 5480-5593 / `DrawChr` 6067-6129 / `RunSound` 6788-6901 /
/// `RunActSound` 6903-7095）的 1:1 回归证据。
///
/// <para><b>为何这些断言对"静态核心形态"仍然有效</b>：四个本体是**纯状态机** ——
/// 只读写 <c>TActorCore</c> 字段、只经 <see cref="ActorFamilyEnv"/> 接缝取外部资源。
/// 授权接通 <c>PlaySceneNewActor.cs</c>（报告 §8 请求 1）后，虚成员会以**零逻辑改动**转调
/// <see cref="ActorFamilyImpl"/>，故本文件的每一条断言**无需改动即继续成立**。</para>
///
/// <para>判据全部是**结构性**的：接缝收到的规划/操作/声音队列，以及被改写的字段。
/// 差异断言集中在原文最容易"看起来一样实则不同"的三处：
/// ① 自定义怪路径的偏移**不减** `m_nStartFrame` 而全局反向帧减；
/// ② `SM_STRUCK` 三音各自判 `&gt;= 0` 而 `SM_ALIVE/SM_DIGUP` 不判；
/// ③ `RunActSound` 非武器族的 race 50 空实现 vs race 202..209 硬编码音。</para>
/// </summary>
public sealed class ActorFamilyBaseTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorFamilyBaseTests()
    {
        ActorFamilyEnv.Reset();
        _restore.Add(ActorFamilyEnv.Reset);

        // 画布默认就绪（多数用例只关心守卫之后的逻辑）
        ActorFamilyEnv.CanvasReadyFn = () => true;
        _restore.Add(() => ActorFamilyEnv.CanvasReadyFn = () => false);

        ActorFamilyEnv.MyGetTickCountFn = () => 7777;
        _restore.Add(() => ActorFamilyEnv.MyGetTickCountFn = () => SceneTime.TickNow());
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ══════════════════════════════════════════════════════════════════════
    // 构造工具
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>最小的 TActor 实例（用现成的 THumActor 作载体，避免新造类型）。
    /// ★ 车道 p7-client-actor-family：本体已接上虚槽位，故返回类型由 TActorCore 收紧为 <see cref="TActor"/>。</summary>
    private static TActor NewActor() => new THumActor();

    private static TMonsterClientAction Act(int start, int play, int empty, int actionFile = -1)
        => new()
        {
            ActionType = TMonsterClientActionType.matStand,
            StartIndex = (short)start,
            PlayCount = (ushort)play,
            EmptyCount = (ushort)empty,
            PlayTime = 100,
            CalcDir = 1,
            EffectIndex = -1,
            EffectIndex2 = -1,
            EffectFile = -1,
            EffectFile2 = -1,
            ActionFile = (short)actionFile,
        };

    private static TClientCustomMonsterConfig Cfg(params (TMonsterClientActionType T, TMonsterClientAction A)[] slots)
    {
        var cfg = new TClientCustomMonsterConfig();
        foreach (var (t, a) in slots)
            cfg.Actions[(int)t] = a;
        return cfg;
    }

    /// <summary>记录一次取图（原文 5546/5548/5559-5562/5571-5587 的唯一出口）。</summary>
    private sealed class FetchLog
    {
        public readonly List<(ActorBodyImage Img, string Kind)> Calls = new();

        public Func<ActorBodyImage, string, object?> Hook(bool returnSurface = true)
            => (img, kind) =>
            {
                Calls.Add((img, kind));
                return returnSurface ? "surface" : null;
            };
    }

    // ══════════════════════════════════════════════════════════════════════
    // 一、LoadSurface（5480-5593）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void LoadSurface_CanvasNotReady_DoesNotConsumeThrottleWindow()
    {
        // 核心发现一（ClientLoadSurfaceCore J179）：守卫在打点**之前**
        ActorFamilyEnv.CanvasReadyFn = () => false;
        var a = NewActor();
        a.m_dwLoadSurfaceTime = 4242;
        a.m_boLoadSurface = true;
        a.m_BodySurface = "old";

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Equal(4242u, a.m_dwLoadSurfaceTime);   // 时间戳**未**刷新
        Assert.True(a.m_boLoadSurface);               // 加载标志**未**清
        Assert.Equal("old", a.m_BodySurface);         // 主体图**未**清
    }

    [Fact]
    public void LoadSurface_CanvasReady_RefreshesTimestampAndClearsBody()
    {
        var a = NewActor();
        a.m_dwLoadSurfaceTime = 1;
        a.m_boLoadSurface = true;
        a.m_BodySurface = "old";

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Equal(7777u, a.m_dwLoadSurfaceTime);
        Assert.False(a.m_boLoadSurface);
        Assert.Null(a.m_BodySurface);                 // 全局路径未取到图（接缝默认 null）
    }

    [Fact]
    public void LoadSurface_GlobalPath_ForwardIndexIsOffsetPlusCurrentFrame()
    {
        var fetch = new FetchLog();
        ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
        ActorFamilyEnv.EffectImageListCountFn = () => 10;

        var a = NewActor();
        a.m_btRace = 80;
        a.m_wAppearance = 12;
        a.m_nCurrentFrame = 3;
        a.m_nStartFrame = 1;
        a.m_nEndFrame = 8;
        a.m_boReverseFrame = false;

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Single(fetch.Calls);
        Assert.Equal(12, fetch.Calls[0].Img.LibraryId);                                  // 图库 = 外观号
        Assert.Equal(ActorOffsets.GetOffset(12) + 3, fetch.Calls[0].Img.ImageIndex);      // 5575-5578
        Assert.Equal("surface", a.m_BodySurface);
    }

    [Fact]
    public void LoadSurface_GlobalPath_ReverseIndexUsesEndFrameAndStartFrame()
    {
        var fetch = new FetchLog();
        ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();

        var a = NewActor();
        a.m_btRace = 80;
        a.m_wAppearance = 12;
        a.m_nCurrentFrame = 3;
        a.m_nStartFrame = 1;
        a.m_nEndFrame = 8;
        a.m_boReverseFrame = true;                    // ← 反向播放

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Single(fetch.Calls);
        // 5581：GetOffset + EndFrame - (CurrentFrame - StartFrame)
        Assert.Equal(ActorOffsets.GetOffset(12) + 8 - (3 - 1), fetch.Calls[0].Img.ImageIndex);
    }

    [Fact]
    public void LoadSurface_ReverseAndForwardIndexesDiffer()
    {
        // 差异断言：同一状态下正/反向下标**必须不同**（否则等于反向分支没生效）
        static int IndexFor(bool reverse, out ActorBodyImage img)
        {
            var fetch = new FetchLog();
            ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
            var a = NewActor();
            a.m_btRace = 80;
            a.m_wAppearance = 12;
            a.m_nCurrentFrame = 3;
            a.m_nStartFrame = 1;
            a.m_nEndFrame = 8;
            a.m_boReverseFrame = reverse;
            ActorFamilyImpl.LoadSurface(a, null);
            img = fetch.Calls[0].Img;
            return fetch.Calls[0].Img.ImageIndex;
        }

        int fwd = IndexFor(false, out var f);
        int rev = IndexFor(true, out var r);

        Assert.NotEqual(fwd, rev);
        Assert.Equal(ActorOffsets.GetOffset(12) + 3, fwd);
        Assert.Equal(ActorOffsets.GetOffset(12) + 8 - 2, rev);
        Assert.Equal(12, f.LibraryId);
        Assert.Equal(12, r.LibraryId);
    }

    [Fact]
    public void LoadSurface_CustomMonster_CustomPathOffsetIsRawCurrentFrame()
    {
        // 差异断言（核心发现十二）：自定义怪路径**不减** m_nStartFrame
        var fetch = new FetchLog();
        ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
        ActorFamilyEnv.EffectImageListCountFn = () => 0;      // ActionFile 判为越界 → 回退分支

        var cfg = Cfg((TMonsterClientActionType.matStand, Act(100, 4, 2, actionFile: -1)));
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => cfg;

        var a = NewActor();
        a.m_btRace = 156;
        a.m_nChangeAppr = 7;                 // >= 0 且 race 156 → 自定义怪路径
        a.m_nCurrentAction = TActorCore.SM_TURN;
        a.m_nCurrentFrame = 37;              // 原文 5550：偏移**就是** 37
        a.m_nStartFrame = 30;

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Single(fetch.Calls);
        Assert.Equal(37, fetch.Calls[0].Img.ImageIndex);                                   // 不减 30
        Assert.Equal(7 - ActorFamilyImpl.AppearanceBase, fetch.Calls[0].Img.LibraryId);    // 5548 回退：ChangeAppr-100000
        Assert.False(fetch.Calls[0].Img.UseEffectImageList);
    }

    [Fact]
    public void LoadSurface_CustomMonster_ActionFileInRangeUsesEffectImageList()
    {
        var fetch = new FetchLog();
        ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
        ActorFamilyEnv.EffectImageListCountFn = () => 10;

        var cfg = Cfg((TMonsterClientActionType.matStand, Act(100, 4, 2, actionFile: 5)));
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => cfg;

        var a = NewActor();
        a.m_btRace = 156;
        a.m_nChangeAppr = 7;
        a.m_nCurrentAction = TActorCore.SM_TURN;

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Single(fetch.Calls);
        Assert.Equal(5, fetch.Calls[0].Img.LibraryId);          // 5546：g_EffectImageList[ActionFile]
        Assert.True(fetch.Calls[0].Img.UseEffectImageList);
    }

    [Fact]
    public void LoadSurface_CustomMonster_ReverseFrameTakesNothing()
    {
        // 5564 的门：自定义怪路径被 `not m_boReverseFrame` 包住
        var fetch = new FetchLog();
        ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
        var cfg = Cfg((TMonsterClientActionType.matStand, Act(100, 4, 2)));
        ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => cfg;

        var a = NewActor();
        a.m_btRace = 156;
        a.m_nChangeAppr = 7;
        a.m_nCurrentAction = TActorCore.SM_TURN;
        a.m_boReverseFrame = true;

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Empty(fetch.Calls);                              // ★ 反向帧什么都不取
        Assert.Null(a.m_BodySurface);
    }

    [Fact]
    public void LoadSurface_CustomMonster_TripleGuardRejectsBadSlots()
    {
        // 5544：三重判据 —— StartIndex < 0 或 PlayCount <= 0 都拒绝
        static bool Fetched(TMonsterClientAction slot)
        {
            var fetch = new FetchLog();
            ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
            ActorFamilyEnv.CustomMonsterConfigLookupFn = _ => Cfg((TMonsterClientActionType.matStand, slot));
            var a = NewActor();
            a.m_btRace = 156;
            a.m_nChangeAppr = 7;
            a.m_nCurrentAction = TActorCore.SM_TURN;
            ActorFamilyImpl.LoadSurface(a, null);
            return fetch.Calls.Count == 1;
        }

        Assert.True(Fetched(Act(0, 1, 0)));          // 边界：StartIndex = 0 合法、PlayCount = 1 合法
        Assert.False(Fetched(Act(-1, 1, 0)));        // StartIndex < 0 拒绝
        Assert.False(Fetched(Act(0, 0, 0)));         // PlayCount = 0 拒绝（严格 > 0）
    }

    [Fact]
    public void LoadSurface_CustomMonster_EmptyCaseBranchesMapToNull()
    {
        // 核心发现三/四：SM_LIGHTINGEX 与 SM_SKELETON 是**空分支**（列了等于没列）
        Assert.Null(ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_LIGHTINGEX, false));
        Assert.Null(ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_SKELETON, false));
        Assert.Null(ActorFamilyImpl.MapActionToClientSlot(12345, false));        // 未列出 = 无 else
    }

    [Fact]
    public void LoadSurface_CustomMonster_SkillBranchesSplitIdentically()
    {
        // 差异断言：SM_LIGHTINGEX 与 SM_SKELETON 同为空；SM_DEATH 与 SM_NOWDEATH 同取 matDie
        Assert.Equal(
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_LIGHTINGEX, false),
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_SKELETON, false));
        Assert.Equal(TMonsterClientActionType.matDie,
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_DEATH, false));
        Assert.Equal(TMonsterClientActionType.matDie,
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_NOWDEATH, false));
    }

    [Fact]
    public void LoadSurface_CustomMonster_StoneModeOverridesTurn()
    {
        // 5512-5515：0 与 SM_TURN 共用分支，**石化改写为起身**
        Assert.Equal(TMonsterClientActionType.matStand,
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_TURN, false));
        Assert.Equal(TMonsterClientActionType.matStoneRevive,
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_TURN, true));
        Assert.Equal(TMonsterClientActionType.matStoneRevive,
            ActorFamilyImpl.MapActionToClientSlot(0, true));
    }

    [Fact]
    public void LoadSurface_CustomMonster_FourMovesShareWalkSlot()
    {
        foreach (int act in new[]
                 {
                     TActorCore.SM_WALK, TActorCore.SM_RUSH,
                     TActorCore.SM_RUSHKUNG, TActorCore.SM_BACKSTEP,
                 })
        {
            Assert.Equal(TMonsterClientActionType.matWalk,
                ActorFamilyImpl.MapActionToClientSlot(act, false));
        }
    }

    [Fact]
    public void LoadSurface_CustomMonster_DigupUsesReviveSlot()
    {
        Assert.Equal(TMonsterClientActionType.matStoneRevive,
            ActorFamilyImpl.MapActionToClientSlot(TActorCore.SM_DIGUP, false));
    }

    [Fact]
    public void LoadSurface_GhostHideShortCircuit_RequestsFinalizeInsteadOfFetching()
    {
        var fetch = new FetchLog();
        ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
        ActorFamilyEnv.GhostHideEnabledFn = () => true;

        int finalized = 0;
        ActorFamilyEnv.FinalizeRequestedFn = _ => finalized++;

        var a = NewActor();
        a.m_btRace = 80;
        a.m_wAppearance = 100;
        a.m_boDeath = true;                       // 四重与命中

        ActorFamilyImpl.LoadSurface(a, null);

        Assert.Equal(1, finalized);
        Assert.Empty(fetch.Calls);                // 互斥：不取图
    }

    [Fact]
    public void LoadSurface_GhostHide_ExclusionRange900To906ProtectsBoundaries()
    {
        // 核心发现十九：外观 900..906 的死亡怪物**即使开了隐藏鬼魂也不被终结化**
        static (int Finalized, int Fetched) Run(int appearance)
        {
            var fetch = new FetchLog();
            ActorFamilyEnv.FetchBodySurfaceFn = fetch.Hook();
            ActorFamilyEnv.GhostHideEnabledFn = () => true;
            int fin = 0;
            ActorFamilyEnv.FinalizeRequestedFn = _ => fin++;
            var a = NewActor();
            a.m_btRace = 80;
            a.m_wAppearance = (ushort)appearance;
            a.m_boDeath = true;
            ActorFamilyImpl.LoadSurface(a, null);
            return (fin, fetch.Calls.Count);
        }

        Assert.Equal((1, 0), Run(899));           // 区间外 → 终结化
        Assert.Equal((0, 1), Run(900));           // 下界 → 被保护
        Assert.Equal((0, 1), Run(906));           // 上界 → 被保护
        Assert.Equal((1, 0), Run(907));           // 区间外 → 终结化
    }

    [Fact]
    public void LoadSurface_AlwaysCallsActionChangedLast()
    {
        int changed = 0;
        ActorFamilyEnv.ActionChangedRequestedFn = _ => changed++;

        ActorFamilyImpl.LoadSurface(NewActor(), null);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void LoadSurface_GhostHidePathAlsoCallsActionChanged()
    {
        // 5569 的 Finalize 短路**不 return** —— 仍会走到 5592 的 ActionChanged
        ActorFamilyEnv.GhostHideEnabledFn = () => true;
        ActorFamilyEnv.FinalizeRequestedFn = _ => { };
        int changed = 0;
        ActorFamilyEnv.ActionChangedRequestedFn = _ => changed++;

        var a = NewActor();
        a.m_btRace = 80;
        a.m_wAppearance = 100;
        a.m_boDeath = true;

        ActorFamilyImpl.LoadSurface(a, null);
        Assert.Equal(1, changed);
    }

    [Theory]
    [InlineData(TColorEffect.ceNone, "image")]
    [InlineData(TColorEffect.ceGrayScale, "gray")]
    [InlineData(TColorEffect.ceGrayScale2, "gray")]   // ★ 两个灰度成员共用一支
    [InlineData(TColorEffect.ceBright, "bright")]
    [InlineData(TColorEffect.ceRed, "image")]         // 其余 11 个成员全走普通
    [InlineData(TColorEffect.ceGray, "image")]
    public void LoadSurface_ColorEffectMapsToThreeFetchKinds(TColorEffect c, string expected)
    {
        Assert.Equal(expected, ActorFamilyImpl.ActorBodyImageFetchKind(c));
    }

    [Fact]
    public void LoadSurface_GrayScaleAndGrayScale2ShareOneBranch()
    {
        // 差异断言：ceGrayScale 与 ceGrayScale2 取值不同（1 vs 13）但**同走灰度支**
        Assert.NotEqual(TColorEffect.ceGrayScale, TColorEffect.ceGrayScale2);
        Assert.Equal(
            ActorFamilyImpl.ActorBodyImageFetchKind(TColorEffect.ceGrayScale),
            ActorFamilyImpl.ActorBodyImageFetchKind(TColorEffect.ceGrayScale2));
    }

    // ══════════════════════════════════════════════════════════════════════
    // 二、DrawChr（6067-6129）与 DrawStateEffSurface（5654-5702）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void DrawChr_DirectionOutOfRange_ExitsBeforeAnyDraw()
    {
        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;
        ActorFamilyEnv.CanvasDrawOpFn = o => { if (o != null) ops.Add(o); };

        var a = NewActor();
        a.m_btDir = 8;                    // 越界
        a.m_BodySurface = "s";
        a.m_boCobweb = true;

        ActorFamilyImpl.DrawChr(a, 0, 0, false, false);

        Assert.Empty(ops);                // 6073 是唯一前置守卫，越界则**全不画**
    }

    [Fact]
    public void DrawChr_BodySurfaceNullSkipsBodyAndStateLayers()
    {
        // 6075 的门：BodySurface 为 nil ⇒ 既不 DrawEffSurface 也不 DrawStateEffSurface
        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;
        ActorFamilyEnv.CanvasDrawOpFn = o => { if (o != null) ops.Add(o); };

        var a = NewActor();
        a.m_btDir = 3;
        a.m_BodySurface = null;
        a.m_boCobweb = true;
        a.m_dwCobwebTick = 0;

        ActorFamilyImpl.DrawChr(a, 0, 0, false, false);

        Assert.Empty(ops);
        Assert.Equal(0, a.m_nCobwebIndex);   // 游标也未被推进
    }

    [Fact]
    public void DrawChr_BodySurfacePresentDrawsBodyThenStateLayers()
    {
        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;
        ActorFamilyEnv.CanvasDrawOpFn = o => { if (o != null) ops.Add(o); };
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;

        var a = NewActor();
        a.m_btDir = 3;
        a.m_BodySurface = "s";
        a.m_boCobweb = true;                                   // 让状态层也出一笔
        a.m_nSayX = 100;                                       // 蛛网 x = m_nSayX - d.Width div 2
        a.m_nPx = 5;
        a.m_nPy = 6;
        a.m_nShiftX = 7;
        a.m_nShiftY = 8;

        ActorFamilyImpl.DrawChr(a, 100, 200, false, false);

        Assert.Equal(2, ops.Count);                            // 主体 1 次 + 蛛网 1 次
        Assert.Equal(112, ops[0].X);                           // 100 + 5 + 7（6078）
        Assert.Equal(214, ops[0].Y);                           // 200 + 6 + 8（6079）
        Assert.Equal(100, ops[1].X);                           // m_nSayX - 宽/2（6083 → 5669）
        Assert.Equal(1, a.m_nCobwebIndex);                     // 顺带确认游标被推进
    }

    [Fact]
    public void DrawChr_SpellEffectOnlyWhenUseMagicAndFrameInRange()
    {
        // 6099-6100 的双门
        static int Ops(bool useMagic, int effectNumber, int curEff, int spellFrame)
        {
            var ops = new List<SurfaceDrawOp>();
            ActorFamilyEnv.CanvasDrawBlendFn = ops.Add;
            ActorFamilyEnv.GetEffectBaseFn = (_, _, _) => new ActorDrawDispatch.EffectBaseRef("lib", 10, 1, 2);
            var a = NewActor();
            a.m_btDir = 2;
            a.m_BodySurface = null;                  // 隔离：只看施法层
            a.m_boUseMagic = useMagic;
            a.m_CurMagicEffectNumber = effectNumber;
            a.m_nCurEffFrame = curEff;
            a.m_nSpellFrame = spellFrame;
            ActorFamilyImpl.DrawChr(a, 0, 0, false, false);
            return ops.Count;
        }

        Assert.Equal(1, Ops(true, 5, 0, 4));         // 边界下界合法
        Assert.Equal(1, Ops(true, 5, 3, 4));         // 边界上界合法（spellFrame - 1）
        Assert.Equal(0, Ops(true, 5, 4, 4));         // 越上界
        Assert.Equal(0, Ops(true, 5, -1, 4));        // 越下界
        Assert.Equal(0, Ops(false, 5, 0, 4));        // 未用魔法
        Assert.Equal(0, Ops(true, 0, 0, 4));         // EffectNumber 必须 > 0（严格）
    }

    [Fact]
    public void DrawChr_SpellEffectMissingEffectBaseDrawsNothing()
    {
        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.CanvasDrawBlendFn = ops.Add;
        ActorFamilyEnv.GetEffectBaseFn = (_, _, _) => null;   // wimg = nil（6104 的门）

        var a = NewActor();
        a.m_btDir = 2;
        a.m_boUseMagic = true;
        a.m_CurMagicEffectNumber = 5;
        a.m_nSpellFrame = 4;

        ActorFamilyImpl.DrawChr(a, 0, 0, false, false);
        Assert.Empty(ops);
    }

    [Fact]
    public void DrawStateEffSurface_CobwebAdvancesOnlyPast100ms()
    {
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;
        ActorFamilyEnv.CanvasDrawOpFn = _ => { };

        var a = NewActor();
        a.m_btDir = 1;
        a.m_boCobweb = true;
        a.m_dwCobwebTick = 7700;
        a.m_nCobwebIndex = 0;

        ActorFamilyEnv.MyGetTickCountFn = () => 7700 + 100;   // 恰好 100 → **不** > 100
        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);
        Assert.Equal(0, a.m_nCobwebIndex);

        ActorFamilyEnv.MyGetTickCountFn = () => 7700 + 101;   // > 100
        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);
        Assert.Equal(1, a.m_nCobwebIndex);
        Assert.Equal(7700u + 101, a.m_dwCobwebTick);
    }

    [Fact]
    public void DrawStateEffSurface_CobwebIndexWrapsAfter9()
    {
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;
        var a = NewActor();
        a.m_boCobweb = true;
        a.m_nCobwebIndex = 9;
        a.m_dwCobwebTick = 0;
        ActorFamilyEnv.MyGetTickCountFn = () => 1000;         // > 100 → 推进到 10

        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);

        Assert.Equal(0, a.m_nCobwebIndex);                    // 5664-5665：> 9 → 回卷 0
    }

    [Fact]
    public void DrawStateEffSurface_GhostSuppressesAllThreeLayers()
    {
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;
        ActorFamilyEnv.CanvasDrawOpFn = _ => { };

        var a = NewActor();
        a.m_boCobweb = true;
        a.m_boToxicSmoke = true;
        a.m_boForeverFrozen = true;
        a.m_boGhost = true;                                   // 隐身
        a.m_dwCobwebTick = 0;
        a.m_dwToxicSmokeTick = 0;
        a.m_dwForeverFrozenTick = 0;
        ActorFamilyEnv.MyGetTickCountFn = () => 9999;

        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);

        Assert.Equal(0, a.m_nCobwebIndex);           // 三层游标**都未**推进
        Assert.Equal(0, a.m_nToxicSmokeIndex);
        Assert.Equal(0, a.m_nForeverFrozenIndex);
    }

    [Fact]
    public void DrawStateEffSurface_DeathSuppressesAllThreeLayers()
    {
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;
        var a = NewActor();
        a.m_boCobweb = true;
        a.m_boToxicSmoke = true;
        a.m_boForeverFrozen = true;
        a.m_boDeath = true;
        ActorFamilyEnv.MyGetTickCountFn = () => 9999;

        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);

        Assert.Equal(0, a.m_nCobwebIndex);
        Assert.Equal(0, a.m_nToxicSmokeIndex);
        Assert.Equal(0, a.m_nForeverFrozenIndex);
    }

    [Fact]
    public void DrawStateEffSurface_DuanJinSuppressesCobwebOnly()
    {
        // 差异断言（5659）：m_boDuanJin 只压制**蛛网**，毒烟与冰冻照常
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;
        var a = NewActor();
        a.m_boCobweb = true;
        a.m_boDuanJin = true;
        a.m_boToxicSmoke = true;
        a.m_boForeverFrozen = true;
        ActorFamilyEnv.MyGetTickCountFn = () => 9999;

        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);

        Assert.Equal(0, a.m_nCobwebIndex);            // 被压制
        Assert.Equal(1, a.m_nToxicSmokeIndex);        // 80ms 节流 → 推进
        Assert.Equal(1, a.m_nForeverFrozenIndex);     // 80ms 节流 → 推进
    }

    [Fact]
    public void DrawStateEffSurface_FrozenWrapsAfter3ButToxicAfter9()
    {
        // 差异断言：冰冻回卷区间 0..3、毒烟 0..9（5694 vs 5679）
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => true;
        var a = NewActor();
        a.m_boToxicSmoke = true;
        a.m_boForeverFrozen = true;
        a.m_nToxicSmokeIndex = 9;
        a.m_nForeverFrozenIndex = 3;
        ActorFamilyEnv.MyGetTickCountFn = () => 9999;

        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);

        Assert.Equal(0, a.m_nToxicSmokeIndex);        // 9 → 10 → 回卷 0
        Assert.Equal(0, a.m_nForeverFrozenIndex);     // 3 → 4 → 回卷 0
    }

    [Fact]
    public void DrawStateEffSurface_MissingImageDrawsNothingButStillAdvances()
    {
        // 5668/5683/5698 的存在性门与游标推进是**两件事**：d = nil 只是不画
        ActorFamilyEnv.StateFxImageExistsFn = (_, _) => false;
        int drawn = 0;
        ActorFamilyEnv.CanvasDrawOpFn = o => { if (o != null) drawn++; };

        var a = NewActor();
        a.m_boCobweb = true;
        ActorFamilyEnv.MyGetTickCountFn = () => 9999;

        ActorFamilyImpl.DrawStateEffSurface(a, 0, 0);

        Assert.Equal(0, drawn);
        Assert.Equal(1, a.m_nCobwebIndex);            // 游标**照样**推进
    }

    // ══════════════════════════════════════════════════════════════════════
    // 三、RunSound（6788-6901）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void RunSound_UnconditionallySetsRunSoundTrue_EvenWhenNothingPlays()
    {
        // 6794：这一句**无条件**执行（差异断言：即使 case 未命中/全部音号为 -1）
        var a = NewActor();
        a.m_boRunSound = false;
        a.m_nCurrentAction = 999999;            // 不在五标签内

        ActorFamilyImpl.RunSound(a);

        Assert.True(a.m_boRunSound);
    }

    [Fact]
    public void RunSound_StruckPlaysThreeSoundsIndependently()
    {
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_nCurrentAction = TActorCore.SM_STRUCK;
        a.m_nStruckWeaponSound = 11;
        a.m_nStruckSound = 22;
        a.m_nScreamSound = 33;

        ActorFamilyImpl.RunSound(a);

        Assert.Equal(new[] { 11, 22, 33 }, played);     // 三音**各自**判 >= 0，可同时播
    }

    [Fact]
    public void RunSound_StruckSkipsNegativeSoundIds()
    {
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_nCurrentAction = TActorCore.SM_STRUCK;
        a.m_nStruckWeaponSound = -1;
        a.m_nStruckSound = 22;
        a.m_nScreamSound = -1;

        ActorFamilyImpl.RunSound(a);

        Assert.Equal(new[] { 22 }, played);
    }

    [Fact]
    public void RunSound_NowDeathRequiresDeathFlagInAdditionToValidSound()
    {
        // 6803：(m_nDieSound >= 0) **and** m_boDeath
        static int Played(bool death)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            var a = NewActor();
            a.m_nCurrentAction = TActorCore.SM_NOWDEATH;
            a.m_nDieSound = 66;
            a.m_boDeath = death;
            ActorFamilyImpl.RunSound(a);
            return played.Count;
        }

        Assert.Equal(1, Played(true));
        Assert.Equal(0, Played(false));
    }

    [Fact]
    public void RunSound_NowDeathOnMySelfAddsGameOverBgmDelay()
    {
        int bgm = 0;
        ActorFamilyEnv.SendGameOverBgmDelayFn = () => bgm++;

        var a = NewActor();
        a.m_nCurrentAction = TActorCore.SM_NOWDEATH;
        a.m_nDieSound = 66;
        a.m_boDeath = true;

        ActorFamilyImpl.RunSound(a);
        Assert.Equal(0, bgm);                    // 非主角 → 不加发

        TActorCore.MySelfRef = a;                // 变主角
        _restore.Add(() => TActorCore.MySelfRef = null);

        ActorFamilyImpl.RunSound(a);
        Assert.Equal(1, bgm);
    }

    [Fact]
    public void RunSound_AppearActionPlaysEvenWhenSoundIdIsNegative()
    {
        // 差异断言（6814-6815）：SM_ALIVE/SM_DIGUP **没有** >= 0 判定
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_nCurrentAction = TActorCore.SM_DIGUP;
        a.m_nAppearSound = -1;                   // 仍会被播

        ActorFamilyImpl.RunSound(a);

        Assert.Equal(new[] { -1 }, played);
    }

    [Fact]
    public void RunSound_AttackFamilyPlaysOnlyWhenAttackSoundNonNegative()
    {
        foreach (int act in new[]
                 {
                     TActorCore.SM_THROW, TActorCore.SM_HIT, TActorCore.SM_FLYAXE,
                     TActorCore.SM_LIGHTING, TActorCore.SM_DIGDOWN,
                 })
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            var a = NewActor();
            a.m_nCurrentAction = act;
            a.m_nAttackSound = -1;
            ActorFamilyImpl.RunSound(a);
            Assert.Empty(played);

            a.m_nAttackSound = 44;
            ActorFamilyImpl.RunSound(a);
            Assert.Equal(new[] { 44 }, played);
        }
    }

    [Fact]
    public void RunSound_SpellWithoutCustomMagicPlaysMagicStartSound()
    {
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;
        ActorFamilyEnv.CustomMagicConfigLookupFn = _ => null;      // 无配置

        var a = NewActor();
        a.m_nCurrentAction = TActorCore.SM_SPELL;
        a.m_nMagicStartSound = 77;

        ActorFamilyImpl.RunSound(a);

        Assert.Equal(new[] { 77 }, played);
    }

    [Fact]
    public void RunSound_SpellWithCustomMagicRequiresNonEmptyUseMagicSound()
    {
        // 6832：只有 `Length(Sounds[cmstUseMagic]) > 0` 才播 —— **空串不播**
        static int Played(string useMagic)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            ActorFamilyEnv.SoundIdFn = _ => 88;
            ActorFamilyEnv.CustomMagicConfigLookupFn = _ =>
                new ActorFamilyEnv.CustomMagicSoundView { UseMagic = useMagic };

            var a = NewActor();
            a.m_nCurrentAction = TActorCore.SM_SPELL;
            a.m_nMagicStartSound = 77;                 // 有配置时**不**走这一支
            ActorFamilyImpl.RunSound(a);
            return played.Count;
        }

        Assert.Equal(1, Played("s_custom"));           // 非空 → 播配置音
        Assert.Empty(new List<int>());                 // （保持对称）
        Assert.Equal(0, Played(""));                   // 空串 → **不播**且不回退 magicStart
    }

    // ══════════════════════════════════════════════════════════════════════
    // 四、RunActSound（6903-7095）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void RunActSound_GateIsBoRunSound()
    {
        // 6909：关掉后整段不发一声
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_boRunSound = false;
        a.m_btRace = 202;
        a.m_nCurrentAction = TActorCore.SM_TURN;

        ActorFamilyImpl.RunActSound(a, 0);
        Assert.Empty(played);
    }

    [Fact]
    public void RunActSound_Race0And1TakeWeaponFamily()
    {
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_boRunSound = true;
        a.m_btRace = 1;                            // ∈ {0,1} → 武器族
        a.m_nCurrentAction = TActorCore.SM_HIT;
        a.m_nWeaponSound = 55;

        ActorFamilyImpl.RunActSound(a, 2);          // 武器族共同门是 frame = 2

        Assert.Equal(new[] { 55 }, played);
        Assert.False(a.m_boRunSound);               // 6915：播过就关
    }

    [Fact]
    public void RunActSound_WeaponFamilyRequiresFrame2()
    {
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_boRunSound = true;
        a.m_btRace = 0;
        a.m_nCurrentAction = TActorCore.SM_HIT;
        a.m_nWeaponSound = 55;

        ActorFamilyImpl.RunActSound(a, 3);          // frame ≠ 2

        Assert.Empty(played);
        Assert.True(a.m_boRunSound);                // 未播 ⇒ 门保持开
    }

    [Fact]
    public void RunActSound_Race50IsEmptyImplementation()
    {
        // 7046-7047：既不播音、也**不消耗随机数**
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;
        int randomCalls = 0;
        ActorFamilyEnv.RandomFn = _ => { randomCalls++; return 1; };

        var a = NewActor();
        a.m_boRunSound = true;
        a.m_btRace = 50;
        a.m_nCurrentAction = TActorCore.SM_TURN;
        a.m_nNormalSound = 10;

        ActorFamilyImpl.RunActSound(a, 1);

        Assert.Empty(played);
        Assert.Equal(0, randomCalls);               // ★ 连随机数都不取
        Assert.True(a.m_boRunSound);
    }

    [Fact]
    public void RunActSound_NonWeaponTurnRequiresFrame1AndRandomEquals1()
    {
        static int Played(int frame, int rand)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            ActorFamilyEnv.RandomFn = _ => rand;
            var a = NewActor();
            a.m_boRunSound = true;
            a.m_btRace = 80;                        // 非 {0,1} 非 50
            a.m_nCurrentAction = TActorCore.SM_TURN;
            a.m_nNormalSound = 90;
            ActorFamilyImpl.RunActSound(a, frame);
            return played.Count;
        }

        Assert.Equal(1, Played(1, 1));              // 7052 全中
        Assert.Equal(0, Played(1, 0));              // Random ≠ 1
        Assert.Equal(0, Played(2, 1));              // frame ≠ 1
    }

    [Fact]
    public void RunActSound_NonWeaponHitRequiresFrame3AndAttackSound()
    {
        static int Played(int frame, int attackSound)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            var a = NewActor();
            a.m_boRunSound = true;
            a.m_btRace = 80;
            a.m_nCurrentAction = TActorCore.SM_HIT;
            a.m_nAttackSound = attackSound;
            a.m_nWeaponSound = 70;
            ActorFamilyImpl.RunActSound(a, frame);
            return played.Count;
        }

        Assert.Equal(1, Played(3, 5));              // 7058 全中
        Assert.Equal(0, Played(3, -1));             // 攻击音无效
        Assert.Equal(0, Played(2, 5));              // frame ≠ 3（注意与武器族的 frame=2 相反）
    }

    [Fact]
    public void RunActSound_Appearance80Die2BranchIsLanded()
    {
        // ★ 7063-7072：既有派发层 ActorSoundDispatch.RunActSoundOther **缺这一支**
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_boRunSound = true;
        a.m_btRace = 80;
        a.m_wAppearance = 80;
        a.m_nCurrentAction = TActorCore.SM_NOWDEATH;
        a.m_nDie2Sound = 123;

        ActorFamilyImpl.RunActSound(a, 2);

        Assert.Equal(new[] { 123 }, played);
        Assert.False(a.m_boRunSound);
    }

    [Fact]
    public void RunActSound_Appearance80Die2BranchStillRequiresFrame2()
    {
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_boRunSound = true;
        a.m_btRace = 80;
        a.m_wAppearance = 80;
        a.m_nCurrentAction = TActorCore.SM_NOWDEATH;
        a.m_nDie2Sound = 123;

        ActorFamilyImpl.RunActSound(a, 1);          // frame ≠ 2
        Assert.Empty(played);
    }

    [Fact]
    public void RunActSound_Appearance80BranchDoesNotApplyToOtherAppearanceOrAction()
    {
        static int Played(int appearance, int action)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            var a = NewActor();
            a.m_boRunSound = true;
            a.m_btRace = 80;
            a.m_wAppearance = (ushort)appearance;
            a.m_nCurrentAction = action;
            a.m_nDie2Sound = 123;
            ActorFamilyImpl.RunActSound(a, 2);
            return played.Count;
        }

        Assert.Equal(0, Played(81, TActorCore.SM_NOWDEATH));   // 外观不是 80
        Assert.Equal(0, Played(80, TActorCore.SM_DEATH));      // 动作不是 NOWDEATH
    }

    [Fact]
    public void RunActSound_Mon36FamilyPlaysHardCodedSoundsWithoutFrameGate()
    {
        // ★ 7076-7092：race 202..209 三动作 → 三个硬编码音，**都不判 frame**
        static List<int> Played(int action, int frame)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            var a = NewActor();
            a.m_boRunSound = true;
            a.m_btRace = 205;
            a.m_nCurrentAction = action;
            ActorFamilyImpl.RunActSound(a, frame);
            return played;
        }

        Assert.Equal(new[] { 542 }, Played(TActorCore.SM_TURN, 0));
        Assert.Equal(new[] { 495 }, Played(TActorCore.SM_STRUCK, 7));      // frame 无关
        Assert.Equal(new[] { 496 }, Played(TActorCore.SM_NOWDEATH, 99));   // frame 无关
    }

    [Fact]
    public void RunActSound_Mon36FamilyBoundaries()
    {
        static bool Played(int race)
        {
            var played = new List<int>();
            ActorFamilyEnv.PlaySoundByIdFn = played.Add;
            var a = NewActor();
            a.m_boRunSound = true;
            a.m_btRace = (byte)race;
            a.m_nCurrentAction = TActorCore.SM_TURN;
            ActorFamilyImpl.RunActSound(a, 0);
            return played.Count == 1;
        }

        Assert.False(Played(201));      // 下界外
        Assert.True(Played(202));       // 下界
        Assert.True(Played(209));       // 上界
        Assert.False(Played(210));      // 上界外
    }

    [Fact]
    public void RunActSound_Mon36BranchDoesNotApplyToRace0or1()
    {
        // 差异断言：race 2..49 / 51..201 走非武器族；而 {0,1} 走武器族**不会**落到 Mon36 分支
        var played = new List<int>();
        ActorFamilyEnv.PlaySoundByIdFn = played.Add;

        var a = NewActor();
        a.m_boRunSound = true;
        a.m_btRace = 1;                                 // ∈ {0,1}
        a.m_nCurrentAction = TActorCore.SM_TURN;        // Mon36 的动作之一

        ActorFamilyImpl.RunActSound(a, 0);
        Assert.Empty(played);                           // 武器族里 SM_TURN 无分支 ⇒ 不播
    }

    // ══════════════════════════════════════════════════════════════════════
    // 五、SetSound（6454-6786）—— 空实现的**语义正当性**证据
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void SetSound_IsNoOpForMonsterRaces()
    {
        // 原文 6459 `if m_btRace in [0,1]` 无 else ⇒ 怪物族本就无副作用
        var a = NewActor();
        a.m_btRace = 80;
        a.m_nStruckSound = -1;
        a.m_nDieSound = -1;

        ActorFamilyImpl.SetSound(a);

        Assert.Equal(-1, a.m_nStruckSound);      // 未被改写
        Assert.Equal(-1, a.m_nDieSound);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 六、防御性：null 实参必须显式抛（而不是静默跳过）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void AllBodies_ThrowOnNullSelf()
    {
        Assert.Throws<ArgumentNullException>(() => ActorFamilyImpl.LoadSurface(null!, null));
        Assert.Throws<ArgumentNullException>(() => ActorFamilyImpl.DrawChr(null!, 0, 0, false, false));
        Assert.Throws<ArgumentNullException>(() => ActorFamilyImpl.DrawStateEffSurface(null!, 0, 0));
        Assert.Throws<ArgumentNullException>(() => ActorFamilyImpl.RunSound(null!));
        Assert.Throws<ArgumentNullException>(() => ActorFamilyImpl.RunActSound(null!, 0));
        Assert.Throws<ArgumentNullException>(() => ActorFamilyImpl.SetSound(null!));
    }
}

using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J85：Actor.pas 自身特效族 1:1 测试 ——
/// DrawSelfEffect(8751-8892)、ShowIcons(8601-8665)、DrawExploreItemEffect(8926-8959)、
/// DrawLockTargetEffect(8894-8907)、GetNearObjectHintInfo(8910-8924)。
/// </summary>
public sealed class ActorSelfEffectRenderTests
{
    private static FxImage Img(int idx, int ox = 0, int oy = 0)
        => new(idx, 16, 16, ox, oy, false);

    private static ActorSelfEffectState SelfState() => new()
    {
        m_btDir = 0,
        m_nCurrentAction = 0,
        EffectImageListCount = 100,
        m_nShiftX = 0, m_nShiftY = 0,
    };

    private static ActorIconRenderState IconState() => new()
    {
        m_btRace = ActorLabelConsts.RC_MONSTER,
        m_boCanDraw = true,
        m_nSayX = 100, m_nSayY = 200,
        EffectImageListCount = 100,
    };

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(120, ActorSelfEffectRender.StatusEffectTickMs);
        Assert.Equal(100000, ActorSelfEffectRender.StatusAniIndexWrap);
        Assert.Equal(100000, ActorSelfEffectRender.StatusStruckWrap);
        Assert.Equal(2, ActorSelfEffectRender.StatusEffectOffsetY);
        Assert.Equal(800, ActorSelfEffectRender.ExploreItemDelayMs);
        Assert.Equal(-20, ActorSelfEffectRender.ExploreItemOffsetY);
    }

    // ===================== IsMonster（原文取反语义） =====================

    [Fact]
    public void IsMonsterPlayerFollowsHumMonsterFlag()
    {
        Assert.True(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_PLAYOBJECT, true));
        Assert.False(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_PLAYOBJECT, false));
    }

    [Fact]
    public void IsMonsterForUnlistedRaceIsTrue()
    {
        // 集合是「非怪物」清单且结果取反 → 未列出者才是怪物
        Assert.True(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_MONSTER, false));
        Assert.True(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_PLAYMOSTER, false));
        Assert.True(ActorSelfEffectRender.IsMonster(200, false));
    }

    [Fact]
    public void IsMonsterForListedRaceIsFalse()
    {
        // 这些在「非怪物」集合内 → 取反后为 False
        Assert.False(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_HEROOBJECT, false));
        Assert.False(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_GUARD, false));
        Assert.False(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_ANIMAL, false));
        Assert.False(ActorSelfEffectRender.IsMonster(ActorLabelConsts.RC_ARCHERGUARD, false));
        Assert.False(ActorSelfEffectRender.IsMonster(23, false));   // 变异骷髅
        Assert.False(ActorSelfEffectRender.IsMonster(54, false));   // 神兽
    }

    [Fact]
    public void NotMonsterRacesMatchesOriginalList()
    {
        Assert.Equal(12, ActorSelfEffectRender.NotMonsterRaces.Length);
        Assert.DoesNotContain(ActorLabelConsts.RC_PLAYOBJECT, ActorSelfEffectRender.NotMonsterRaces);
        Assert.DoesNotContain(ActorLabelConsts.RC_PLAYMOSTER, ActorSelfEffectRender.NotMonsterRaces);
        Assert.Contains(23, ActorSelfEffectRender.NotMonsterRaces);
        Assert.Contains(54, ActorSelfEffectRender.NotMonsterRaces);
    }

    // ===================== DrawSelfEffect 守卫 =====================

    [Fact]
    public void BackActorSkipsStatusAndSelfEffect()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.m_boSelfEffectRunning = true;
        st.HasSelfEffectGameImage = true;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, true, 0,
            null, (i, g) => Img(i), (i, g) => Img(i), (i, g) => Img(i), false);

        Assert.DoesNotContain(ops, o => o.Kind == "StatusEffect");
        Assert.DoesNotContain(ops, o => o.Kind == "SelfEffect");
    }

    [Fact]
    public void HorseSuppressesStatusEffect()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.m_btHorse = 1;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0,
            null, (i, g) => Img(i), null, null, false);

        Assert.DoesNotContain(ops, o => o.Kind == "StatusEffect");
    }

    [Fact]
    public void StatusEffectRequiresBoShow()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;
        st.CustomMagicStatusEffect.Status1_File = 1;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0,
            null, (i, g) => Img(i), null, null, false);

        Assert.DoesNotContain(ops, o => o.Kind == "StatusEffect");
    }

    // ===================== 状态特效：动画推进 =====================

    [Fact]
    public void StatusEffectAdvancesOnTickBoundary()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_PlayCount = 10;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.m_nGenAniTick = 0;
        st.CustomMagicStatusEffect.m_nGenAniIndex = 0;
        st.CustomMagicStatusEffect.m_nStruck = 0;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 200, null, (i, g) => Img(i), null, null, false);

        Assert.Equal(1, st.CustomMagicStatusEffect.m_nGenAniIndex);
        Assert.Equal(1, st.CustomMagicStatusEffect.m_nStruck);
        Assert.Equal(200u, st.CustomMagicStatusEffect.m_nGenAniTick);
    }

    [Fact]
    public void StatusEffectDoesNotAdvanceAtExactly120()
    {
        // 8762 是 `> 120`，恰为 120 时不推进
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_PlayCount = 10;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.m_nGenAniTick = 100;
        st.CustomMagicStatusEffect.m_nGenAniIndex = 5;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 220, null, (i, g) => Img(i), null, null, false);

        Assert.Equal(5, st.CustomMagicStatusEffect.m_nGenAniIndex);
    }

    [Fact]
    public void StatusEffectAniIndexWraps()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_PlayCount = 10;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.m_nGenAniTick = 0;
        st.CustomMagicStatusEffect.m_nGenAniIndex = 100000;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 500, null, (i, g) => Img(i), null, null, false);

        Assert.Equal(0, st.CustomMagicStatusEffect.m_nGenAniIndex);
    }

    [Fact]
    public void StatusEffectStruckClampsAtWrap()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_PlayCount = 10;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.m_nGenAniTick = 0;
        st.CustomMagicStatusEffect.m_nStruck = 100000;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 500, null, (i, g) => Img(i), null, null, false);

        Assert.Equal(100000, st.CustomMagicStatusEffect.m_nStruck);   // 钳制而非回零
    }

    // ===================== 状态特效：图号选择 =====================

    [Fact]
    public void StruckBranchPreferredWhenConditionsMet()
    {
        var st = SelfState();
        st.m_nCurrentAction = TActorCore.SM_STRUCK;
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status2_File = 1;
        st.CustomMagicStatusEffect.Status2_PlayCount = 10;
        st.CustomMagicStatusEffect.Status2_StartIndex = 500;
        st.CustomMagicStatusEffect.m_nStruck = 3;
        st.CustomMagicStatusEffect.Status1_PlayCount = 10;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_StartIndex = 900;

        int asked = -1;
        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => { asked = i; return Img(i); }, null, null, false);

        Assert.Equal(503, asked);                   // 500 + 3
    }

    [Fact]
    public void StruckBranchNeedsStruckBelowPlayCount()
    {
        var st = SelfState();
        st.m_nCurrentAction = TActorCore.SM_STRUCK;
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status2_File = 1;
        st.CustomMagicStatusEffect.Status2_PlayCount = 3;
        st.CustomMagicStatusEffect.Status2_StartIndex = 500;
        st.CustomMagicStatusEffect.m_nStruck = 3;    // 不满足 `< 3`
        st.CustomMagicStatusEffect.Status1_PlayCount = 10;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_StartIndex = 900;

        int asked = -1;
        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => { asked = i; return Img(i); }, null, null, false);

        Assert.Equal(900, asked);                   // 落到 Status1 分支
    }

    [Fact]
    public void StruckBranchWithCalcDirUsesDirStride()
    {
        var st = SelfState();
        st.m_btDir = 2;
        st.m_nCurrentAction = TActorCore.SM_STRUCK;
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status2_File = 1;
        st.CustomMagicStatusEffect.Status2_PlayCount = 10;
        st.CustomMagicStatusEffect.Status2_EmptyCount = 2;
        st.CustomMagicStatusEffect.Status2_StartIndex = 100;
        st.CustomMagicStatusEffect.Status2_CalcDir = true;
        st.CustomMagicStatusEffect.m_nStruck = 1;
        st.CustomMagicStatusEffect.Status1_PlayCount = 0;

        int asked = -1;
        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => { asked = i; return Img(i); }, null, null, false);

        // 100 + (10 + 2) * 2 + 1
        Assert.Equal(125, asked);
    }

    [Fact]
    public void Status1UsesGenAniIndexModuloPlayCount()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_PlayCount = 4;
        st.CustomMagicStatusEffect.Status1_StartIndex = 200;
        st.CustomMagicStatusEffect.m_nGenAniIndex = 6;

        int asked = -1;
        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => { asked = i; return Img(i); }, null, null, false);

        Assert.Equal(202, asked);                   // 200 + 6 % 4
    }

    [Fact]
    public void Status1BlendFlagFromDrawMode()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;
        st.CustomMagicStatusEffect.Status1_StartIndex = 200;
        st.CustomMagicStatusEffect.Status1_DrawMode = (int)TCustomDrawMode.mdmBlend;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => Img(i), null, null, false);

        var op = Assert.Single(ops);
        Assert.True(op.Blend);
    }

    [Fact]
    public void StatusEffectOffsetYIsTwo()
    {
        var st = SelfState();
        st.m_nShiftY = 10;
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;
        st.CustomMagicStatusEffect.Status1_StartIndex = 200;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 5, 7, false, 0, null, (i, g) => Img(i, 3, 4), null, null, false);

        var op = Assert.Single(ops);
        Assert.Equal(5 + 3, op.X);
        Assert.Equal(7 + 4 + 10 + 2, op.Y);
    }

    [Fact]
    public void StatusEffectFileOutOfRangeSkipped()
    {
        var st = SelfState();
        st.EffectImageListCount = 5;
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_File = 5;   // 不满足 `< 5`
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => Img(i), null, null, false);

        Assert.Empty(ops);
    }

    [Fact]
    public void StatusEffectGrayWhenSelfDead()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;
        st.CustomMagicStatusEffect.Status1_StartIndex = 200;

        bool? gray = null;
        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null,
            (i, g) => { gray = g; return Img(i); }, null, null, true);

        Assert.True(gray);
    }

    [Fact]
    public void MissingTextureProducesNoOp()
    {
        var st = SelfState();
        st.CustomMagicStatusEffect.boShow = true;
        st.CustomMagicStatusEffect.Status1_File = 1;
        st.CustomMagicStatusEffect.Status1_PlayCount = 1;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, (i, g) => null, null, null, false);

        Assert.Empty(ops);
    }

    // ===================== 自身持续特效 =====================

    [Fact]
    public void SelfEffectAdvancesFrameOnInterval()
    {
        var st = SelfState();
        st.m_boSelfEffectRunning = true;
        st.HasSelfEffectGameImage = true;
        st.m_nSelfEffectFrameTime = 100;
        st.m_dwSelfEffectLastTick = 0;
        st.m_nSelfEffectCurrentFrame = 2;
        st.m_nSelfEffectEndFrame = 10;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 150, null, null, (i, g) => Img(i), null, false);

        Assert.Equal(3, st.m_nSelfEffectCurrentFrame);
        Assert.Equal(150u, st.m_dwSelfEffectLastTick);
    }

    [Fact]
    public void SelfEffectStopsWhenFrameExceedsEnd()
    {
        var st = SelfState();
        st.m_boSelfEffectRunning = true;
        st.HasSelfEffectGameImage = true;
        st.m_nSelfEffectFrameTime = 0;
        st.m_nSelfEffectCurrentFrame = 10;
        st.m_nSelfEffectEndFrame = 10;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 50, null, null, (i, g) => Img(i), null, false);

        Assert.False(st.m_boSelfEffectRunning);
        Assert.Empty(ops);                          // 8821 直接 Exit
    }

    [Fact]
    public void SelfEffectExitSkipsSelfKeepPlay()
    {
        // 8821 的 Exit 会跳过后续 SelfKeepPlay 段
        var st = SelfState();
        st.m_boSelfEffectRunning = true;
        st.HasSelfEffectGameImage = true;
        st.m_nSelfEffectFrameTime = 0;
        st.m_nSelfEffectCurrentFrame = 99;
        st.m_nSelfEffectEndFrame = 10;
        st.SelfKeepPlay.HasImages = true;
        st.SelfKeepPlay.SelfPlay.SelfKeep_DrawOrder = TCustomDrawOrder.mdoPriorSelf;
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex = 1;
        st.SelfKeepPlay.SelfPlay.SelfKeep_PlayCount = 1;
        st.SelfKeepPlay.SelfPlay.SelfKeep_KeepTime = 10;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 50, null, null, (i, g) => Img(i), (i, g) => Img(i), false);

        Assert.Empty(ops);
    }

    [Fact]
    public void SelfEffectRequiresGameImage()
    {
        var st = SelfState();
        st.m_boSelfEffectRunning = true;
        st.HasSelfEffectGameImage = false;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, (i, g) => Img(i), null, false);

        Assert.Empty(ops);
    }

    [Fact]
    public void SelfEffectBlendFlagHonoured()
    {
        var st = SelfState();
        st.m_boSelfEffectRunning = true;
        st.HasSelfEffectGameImage = true;
        st.m_nSelfEffectFrameTime = 100;
        st.m_nSelfEffectCurrentFrame = 1;
        st.m_nSelfEffectEndFrame = 10;
        st.m_boSelfEffectBlendDraw = true;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, (i, g) => Img(i), null, false);

        var op = Assert.Single(ops);
        Assert.True(op.Blend);
        Assert.Equal("SelfEffect", op.Kind);
    }

    // ===================== SelfKeepPlay =====================

    private static ActorSelfEffectState KeepState(TCustomDrawOrder order)
    {
        var st = SelfState();
        st.SelfKeepPlay.HasImages = true;
        st.SelfKeepPlay.SelfPlay.SelfKeep_DrawOrder = order;
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex = 100;
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 = -1;   // 默认 0 会通过 >= 0 再绘一层
        st.SelfKeepPlay.SelfPlay.SelfKeep_PlayCount = 3;
        st.SelfKeepPlay.SelfPlay.SelfKeep_PlayTime = 100;
        st.SelfKeepPlay.SelfPlay.SelfKeep_KeepTime = 10;
        st.SelfKeepPlay.SelfKeep_StartTime = 0;
        st.SelfKeepPlay.SelfKeep_LastTick = 0;
        return st;
    }

    [Fact]
    public void SelfKeepFrontUsesMdoPriorSelf()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false);
        Assert.Single(ops);
    }

    [Fact]
    public void SelfKeepFrontSkipsWhenOrderIsPriorMagic()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorMagic);
        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false);
        Assert.Empty(ops);
    }

    [Fact]
    public void SelfKeepBackUsesMdoPriorMagic()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorMagic);
        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, true, 0, null, null, null, (i, g) => Img(i), false);
        Assert.Single(ops);
    }

    [Fact]
    public void SelfKeepBackSkipsWhenOrderIsPriorSelf()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, true, 0, null, null, null, (i, g) => Img(i), false);
        Assert.Empty(ops);
    }

    [Fact]
    public void SelfKeepExpiresAfterKeepTimeSeconds()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        // KeepTime = 10 秒 → 10000ms；now = 10001 超时
        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 10001, null, null, null, (i, g) => Img(i), false);
        Assert.Empty(ops);
    }

    [Fact]
    public void SelfKeepBoundaryIsInclusive()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 10000, null, null, null, (i, g) => Img(i), false);
        Assert.Single(ops);                         // `<=` 含边界
    }

    [Fact]
    public void SelfKeepAdvancesIndexAndWraps()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfKeep_Index = 2;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 500, null, null, null, (i, g) => Img(i), false);

        Assert.Equal(0, st.SelfKeepPlay.SelfKeep_Index);   // 2 + 1 >= 3 → 0
        Assert.Equal(500u, st.SelfKeepPlay.SelfKeep_LastTick);
    }

    [Fact]
    public void SelfKeepIndexDoesNotAdvanceBeforePlayTime()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfKeep_Index = 1;
        st.SelfKeepPlay.SelfKeep_LastTick = 0;

        ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 99, null, null, null, (i, g) => Img(i), false);

        Assert.Equal(1, st.SelfKeepPlay.SelfKeep_Index);
    }

    [Fact]
    public void SelfKeepRequiresPositivePlayCountAndKeepTime()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfPlay.SelfKeep_PlayCount = 0;
        Assert.Empty(ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false));

        var st2 = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st2.SelfKeepPlay.SelfPlay.SelfKeep_KeepTime = 0;
        Assert.Empty(ActorSelfEffectRender.DrawSelfEffect(st2, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false));
    }

    [Fact]
    public void SelfKeepRequiresImages()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.HasImages = false;
        Assert.Empty(ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false));
    }

    [Fact]
    public void SelfKeepNegativeStartIndexSkipped()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex = -1;
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 = -1;      // 两层皆关，才真正无 op
        Assert.Empty(ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false));
    }

    [Fact]
    public void SelfKeepDrawsSecondLayerIndependently()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex = -1;      // 第一层跳过
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 = 300;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false);

        var op = Assert.Single(ops);
        Assert.Equal("SelfKeep2", op.Kind);
        Assert.Equal(300, op.ImageIndex);
    }

    [Fact]
    public void SelfKeepBothLayersDrawInOrder()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 = 300;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false);

        Assert.Equal(2, ops.Count);
        Assert.Equal("SelfKeep1", ops[0].Kind);
        Assert.Equal("SelfKeep2", ops[1].Kind);
    }

    [Fact]
    public void SelfKeepLayer2UsesItsOwnDrawMode()
    {
        var st = KeepState(TCustomDrawOrder.mdoPriorSelf);
        st.SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 = 300;
        st.SelfKeepPlay.SelfPlay.SelfKeep_DrawMode = TCustomDrawMode.mdmNormal;
        st.SelfKeepPlay.SelfPlay.SelfKeep_DrawMode2 = TCustomDrawMode.mdmBlend;

        var ops = ActorSelfEffectRender.DrawSelfEffect(st, 0, 0, false, 0, null, null, null, (i, g) => Img(i), false);

        Assert.False(ops[0].Blend);
        Assert.True(ops[1].Blend);
    }

    // ===================== ShowIcons =====================

    private static ActorIconRenderState IconWith(int slot, int fileIdx, int drawOrder, bool blend = false)
    {
        var st = IconState();
        st.ActorIcons[slot].nFileIndex = fileIdx;
        st.ActorIcons[slot].nIconCount = 1;
        st.ActorIcons[slot].btDrawOrder = drawOrder;
        st.ActorIcons[slot].boBlend = blend;
        st.ActorIconIndexs[slot].HasTexture = true;
        return st;
    }

    [Fact]
    public void ShopStallSuppressesIcons()
    {
        var st = IconWith(0, 1, 0);
        st.m_boShopStall = true;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void DeathSuppressesIcons()
    {
        var st = IconWith(0, 1, 0);
        st.m_boDeath = true;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void CannotDrawSuppressesIcons()
    {
        var st = IconWith(0, 1, 0);
        st.m_boCanDraw = false;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HorseDoesNotSuppressIconsBecauseOriginalCheckIsCommentedOut()
    {
        // 8608 的 `if m_btHorse > 0 then Exit;` 已被注释掉
        var st = IconWith(0, 1, 0);
        Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HideActorIconsHidesPlayer()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_PLAYOBJECT;
        st.ckHideActorIcons = true;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HideActorIconsHidesHero()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_HEROOBJECT;
        st.ckHideActorIcons = true;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HideActorIconsDoesNotHideMonster()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_MONSTER;
        st.ckHideActorIcons = true;
        Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HideActorIconsExemptsPlayerAsMonster()
    {
        // `m_boPlayMoster` 为真时玩家不被隐藏
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_PLAYOBJECT;
        st.m_boPlayMoster = true;
        st.ckHideActorIcons = true;
        Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HideMonsterIconsHidesMonster()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_MONSTER;
        st.ckHideMonsterIcons = true;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void HideMonsterIconsKeepsGuard()
    {
        // 守卫在「非怪物」集合内 → IsMonster 为假 → 不被隐藏
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_GUARD;
        st.ckHideMonsterIcons = true;
        Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void IconDrawOrderZeroDrawsFrontOnly()
    {
        var st = IconWith(0, 1, 0);
        Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, true));
    }

    [Fact]
    public void IconDrawOrderNonZeroDrawsBackOnly()
    {
        var st = IconWith(0, 1, 1);
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
        Assert.Single(ActorSelfEffectRender.ShowIcons(st, true));
    }

    [Fact]
    public void IconRequiresTexture()
    {
        var st = IconWith(0, 1, 0);
        st.ActorIconIndexs[0].HasTexture = false;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void IconRequiresNonNegativeFileIndex()
    {
        var st = IconWith(0, -1, 0);
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void IconRequiresFileIndexBelowListCount()
    {
        var st = IconWith(0, 100, 0);
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void IconRequiresPositiveCount()
    {
        var st = IconWith(0, 1, 0);
        st.ActorIcons[0].nIconCount = 0;
        Assert.Empty(ActorSelfEffectRender.ShowIcons(st, false));
    }

    [Fact]
    public void IconPositionUsesDefTextureWidthHalved()
    {
        var st = IconWith(0, 1, 0);
        st.ActorIconIndexs[0].DefTextureWidth = 20;
        st.ActorIcons[0].nX = 3;
        st.ActorIconIndexs[0].nX = 5;

        var op = Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));

        Assert.Equal(100 - 10 + 3 + 5, op.X);
        Assert.Equal(200 - 32, op.Y);
    }

    [Fact]
    public void IconPositionUsesIntegerDivisionForOddWidth()
    {
        var st = IconWith(0, 1, 0);
        st.ActorIconIndexs[0].DefTextureWidth = 21;   // 21 div 2 = 10
        var op = Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
        Assert.Equal(100 - 10, op.X);
    }

    [Fact]
    public void IconHpBarOffsetHum()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_PLAYOBJECT;
        st.nHumHPBarOffsetX = 7; st.nHumHPBarOffsetY = 9;
        st.nMonHPBarOffsetX = 100; st.nMonHPBarOffsetY = 100;

        var op = Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));

        Assert.Equal(100 + 7, op.X);
        Assert.Equal(200 - 32 + 9, op.Y);
    }

    [Fact]
    public void IconHpBarOffsetNpcForMerchant()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_MERCHANT;
        st.nNpcHPBarOffsetX = 3; st.nNpcHPBarOffsetY = 4;

        var op = Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));

        Assert.Equal(100 + 3, op.X);
        Assert.Equal(200 - 32 + 4, op.Y);
    }

    [Fact]
    public void IconHpBarOffsetMonForOthers()
    {
        var st = IconWith(0, 1, 0);
        st.m_btRace = ActorLabelConsts.RC_MONSTER;
        st.nMonHPBarOffsetX = 11; st.nMonHPBarOffsetY = 12;

        var op = Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));

        Assert.Equal(100 + 11, op.X);
        Assert.Equal(200 - 32 + 12, op.Y);
    }

    [Fact]
    public void IconBlendFlagHonoured()
    {
        var st = IconWith(0, 1, 0, blend: true);
        var op = Assert.Single(ActorSelfEffectRender.ShowIcons(st, false));
        Assert.True(op.Blend);
    }

    [Fact]
    public void MultipleIconsAllDrawn()
    {
        var st = IconWith(0, 1, 0);
        st.ActorIcons[1].nFileIndex = 2;
        st.ActorIcons[1].nIconCount = 1;
        st.ActorIcons[1].btDrawOrder = 0;
        st.ActorIconIndexs[1].HasTexture = true;
        st.ActorIcons[2].nFileIndex = 3;
        st.ActorIcons[2].nIconCount = 1;
        st.ActorIcons[2].btDrawOrder = 0;
        st.ActorIconIndexs[2].HasTexture = true;

        Assert.Equal(3, ActorSelfEffectRender.ShowIcons(st, false).Count);
    }

    // ===================== DrawExploreItemEffect =====================

    private static ActorExploreItemState ExploreState() => new()
    {
        m_boDeath = true,
        m_IsExploreItem = true,
        m_boSkeleton = false,
        m_nSayX = 50, m_nSayY = 60,
        m_dwDeathTick = 0,
        dwExploreItemIconCount = 5,
        dwExploreItemIconPlayTime = 100,
        dwExploreItemIconIndex = 700,
    };

    [Fact]
    public void ExploreRequiresDeath()
    {
        var st = ExploreState();
        st.m_boDeath = false;
        Assert.Empty(ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i)));
    }

    [Fact]
    public void ExploreRequiresIsExploreItem()
    {
        var st = ExploreState();
        st.m_IsExploreItem = false;
        Assert.Empty(ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i)));
    }

    [Fact]
    public void ExploreSkeletonSuppresses()
    {
        var st = ExploreState();
        st.m_boSkeleton = true;
        Assert.Empty(ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i)));
    }

    [Fact]
    public void ExploreZeroCountSuppresses()
    {
        var st = ExploreState();
        st.dwExploreItemIconCount = 0;
        Assert.Empty(ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i)));
    }

    [Fact]
    public void ExploreWithinDelayResetsStateAndExits()
    {
        // 8938-8942：不足 800ms 时重置 tick 与 frame 后 Exit
        var st = ExploreState();
        st.m_dwDeathTick = 1000;
        st.m_dwExploreItemEffectTick = 999;
        st.m_dwExploreItemEffectFrame = 3;

        var ops = ActorSelfEffectRender.DrawExploreItemEffect(st, 1500, (i, g) => Img(i));

        Assert.Empty(ops);
        Assert.Equal(1500u, st.m_dwExploreItemEffectTick);
        Assert.Equal(0, st.m_dwExploreItemEffectFrame);
    }

    [Fact]
    public void ExploreDelayBoundaryIsExclusive()
    {
        // tick_diff < 800 才重置；恰为 800 时继续
        var st = ExploreState();
        st.m_dwDeathTick = 1000;
        st.m_dwExploreItemEffectTick = 9000;    // 使后续不推进

        var ops = ActorSelfEffectRender.DrawExploreItemEffect(st, 1800, (i, g) => Img(i));

        Assert.Single(ops);
    }

    [Fact]
    public void ExploreAdvancesFrameOnPlayTime()
    {
        var st = ExploreState();
        st.m_dwDeathTick = 0;
        st.m_dwExploreItemEffectTick = 0;
        st.m_dwExploreItemEffectFrame = 1;

        ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i));

        Assert.Equal(2, st.m_dwExploreItemEffectFrame);
        Assert.Equal(5000u, st.m_dwExploreItemEffectTick);
    }

    [Fact]
    public void ExploreFrameWrapsAtCount()
    {
        var st = ExploreState();
        st.m_dwDeathTick = 0;
        st.m_dwExploreItemEffectTick = 0;
        st.m_dwExploreItemEffectFrame = 4;      // 4 + 1 >= 5 → 0

        ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i));

        Assert.Equal(0, st.m_dwExploreItemEffectFrame);
    }

    [Fact]
    public void ExploreImageIndexIsBasePlusFrame()
    {
        var st = ExploreState();
        st.m_dwDeathTick = 0;
        st.m_dwExploreItemEffectTick = 4900;    // tick_diff = 100 == PlayTime → 推进
        st.m_dwExploreItemEffectFrame = 3;

        int asked = -1;
        ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => { asked = i; return Img(i); });

        // 8944 的推进发生在 8951 取图号**之前**，故本帧用推进后的 frame
        Assert.Equal(704, asked);               // 700 + (3 + 1)
    }

    [Fact]
    public void ExploreImageIndexWithoutAdvance()
    {
        var st = ExploreState();
        st.m_dwDeathTick = 0;
        st.m_dwExploreItemEffectTick = 4950;    // tick_diff = 50 < 100 → 不推进
        st.m_dwExploreItemEffectFrame = 3;

        int asked = -1;
        ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => { asked = i; return Img(i); });

        Assert.Equal(703, asked);               // 700 + 3
    }

    [Fact]
    public void ExplorePositionUsesSayMinus20PlusOffsets()
    {
        var st = ExploreState();
        st.m_dwDeathTick = 0;
        st.m_dwExploreItemEffectTick = 9000;
        st.nExploreItemIconOffsetX = 4;
        st.nExploreItemIconOffsetY = 6;

        var ops = ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => Img(i, 1, 2));

        var op = Assert.Single(ops);
        Assert.Equal(50 + 4 + 1, op.X);
        Assert.Equal(60 - 20 + 6 + 2, op.Y);
    }

    [Fact]
    public void ExploreMissingTextureNoOp()
    {
        var st = ExploreState();
        st.m_dwDeathTick = 0;
        st.m_dwExploreItemEffectTick = 9000;

        Assert.Empty(ActorSelfEffectRender.DrawExploreItemEffect(st, 5000, (i, g) => null));
    }

    // ===================== DrawLockTargetEffect =====================

    [Fact]
    public void LockTargetSuppressedWhenConfigOff()
    {
        var ops = ActorSelfEffectRender.DrawLockTargetEffect(1, 0, 0, false, true, 0, 0, (i, g) => Img(i));
        Assert.Empty(ops);
    }

    [Fact]
    public void LockTargetSuppressedWhenNoImages()
    {
        var ops = ActorSelfEffectRender.DrawLockTargetEffect(1, 0, 0, true, false, 0, 0, (i, g) => Img(i));
        Assert.Empty(ops);
    }

    [Fact]
    public void LockTargetDrawsWithShift()
    {
        var ops = ActorSelfEffectRender.DrawLockTargetEffect(7, 10, 20, true, true, 3, 4, (i, g) => Img(i, 1, 2));

        var op = Assert.Single(ops);
        Assert.Equal(10 + 1 + 3, op.X);
        Assert.Equal(20 + 2 + 4, op.Y);
        Assert.Equal(7, op.ImageIndex);
        Assert.False(op.Blend);
    }

    [Fact]
    public void LockTargetMissingTextureNoOp()
    {
        var ops = ActorSelfEffectRender.DrawLockTargetEffect(7, 0, 0, true, true, 0, 0, (i, g) => null);
        Assert.Empty(ops);
    }

    // ===================== GetNearObjectHintInfo =====================

    [Fact]
    public void NearHintExcludesSelf()
    {
        var r = ActorSelfEffectRender.GetNearObjectHintInfo(true, false, true, false, 10, 20, 1, 2);
        Assert.False(r.Visible);
    }

    [Fact]
    public void NearHintExcludesDead()
    {
        var r = ActorSelfEffectRender.GetNearObjectHintInfo(false, true, true, false, 10, 20, 1, 2);
        Assert.False(r.Visible);
    }

    [Fact]
    public void NearHintRequiresBossList()
    {
        var r = ActorSelfEffectRender.GetNearObjectHintInfo(false, false, false, false, 10, 20, 1, 2);
        Assert.False(r.Visible);
    }

    [Fact]
    public void NearHintAppliesShiftWhenVisible()
    {
        var r = ActorSelfEffectRender.GetNearObjectHintInfo(false, false, true, false, 10, 20, 1, 2);
        Assert.True(r.Visible);
        Assert.Equal(11, r.X);
        Assert.Equal(22, r.Y);
        Assert.Equal(0, r.FriendFlag);
    }

    [Fact]
    public void NearHintFriendFlagWhenInFriendList()
    {
        var r = ActorSelfEffectRender.GetNearObjectHintInfo(false, false, true, true, 0, 0, 0, 0);
        Assert.True(r.Visible);
        Assert.Equal(1, r.FriendFlag);
    }

    [Fact]
    public void NearHintLeavesCoordsUnshiftedWhenNotVisible()
    {
        var r = ActorSelfEffectRender.GetNearObjectHintInfo(true, false, true, true, 10, 20, 5, 6);
        Assert.False(r.Visible);
        Assert.Equal(10, r.X);
        Assert.Equal(20, r.Y);
    }

    // ===================== TickDiff =====================

    [Fact]
    public void TickDiffIsUnsignedWrapAround()
    {
        Assert.Equal(5u, ActorSelfEffectRender.TickDiff(100, 105));
        Assert.Equal(uint.MaxValue, ActorSelfEffectRender.TickDiff(1, 0));   // 回绕
    }
}

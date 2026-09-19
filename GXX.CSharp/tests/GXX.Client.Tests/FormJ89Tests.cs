using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J89：Actor.pas THumActor.DrawChr 尾段护盾/状态特效族 1:1 测试 ——
/// 破碎盾(16853-16864)、3 级魔法盾新旧两版(16868-16903)、4 级魔法盾(16906-16924)、
/// 新武力盾(16929-16959)、新道力盾(16963-16999)。
/// </summary>
public sealed class ShieldStatusEffectRenderTests
{
    private const int SM_STRUCK = 31;      // TActorCore.SM_STRUCK

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(3890, ShieldStatusEffectRender.MAGBUBBLEBASE);
        Assert.Equal(3900, ShieldStatusEffectRender.MAGBUBBLESTRUCKBASE);
        Assert.Equal(3, ShieldStatusEffectRender.BubbleStruckMax);
        Assert.Equal(3, ShieldStatusEffectRender.BubbleAniMod);
        Assert.Equal(4100, ShieldStatusEffectRender.BrokenShieldBase);
        Assert.Equal(0x00100000, ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP);
        Assert.Equal(0x00008000, ShieldStatusEffectRender.STATE_16);
        Assert.Equal(0x00040000, ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP);
        Assert.Equal(0x00020000, ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2);
        Assert.Equal(494, ShieldStatusEffectRender.NewHitBubbleFixedIndex);
        Assert.Equal(-6, ShieldStatusEffectRender.NewBubbleOffsetY);
        Assert.Equal(2, ShieldStatusEffectRender.NewMagBubbleOffsetY);
        Assert.Equal(923, ShieldStatusEffectRender.NewBubbleStruckBase + 3);   // 920 + 3
        Assert.Equal(910, ShieldStatusEffectRender.NewBubbleBase);
        Assert.Equal(723, ShieldStatusEffectRender.Magic6StruckBase);
        Assert.Equal(720, ShieldStatusEffectRender.Magic6Base);
    }

    // ===================== 破碎盾 =====================

    [Fact]
    public void BrokenShieldRequiresFlagAndNoHorse()
    {
        Assert.NotNull(ShieldStatusEffectRender.PlanBrokenShield(true, 0, 0));
        Assert.Null(ShieldStatusEffectRender.PlanBrokenShield(false, 0, 0));
        Assert.Null(ShieldStatusEffectRender.PlanBrokenShield(true, 1, 0));
    }

    [Fact]
    public void BrokenShieldIndexIsBase4100PlusEffect()
    {
        var p = ShieldStatusEffectRender.PlanBrokenShield(true, 0, 5);
        Assert.NotNull(p);
        Assert.Equal(4105, p!.ImageIndex);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.CboEffect, p.ImageSet);
        Assert.Equal("BrokenShield", p.Kind);
    }

    [Fact]
    public void BrokenShieldSuppressesAllMagicShields()
    {
        // 16864 的 else：破碎盾时不走魔法盾族
        var plans = ShieldStatusEffectRender.Plan(
            boBrokenShield: true, nBrokenShieldEffect: 0,
            m_nState: ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP | ShieldStatusEffectRender.STATE_16,
            m_btHorse: 0, boSkill31UseNewEffect: false,
            m_nCurrentAction: 0, m_nCurBubbleStruck: 0, m_nGenAniCount: 0);

        var only = Assert.Single(plans);
        Assert.Equal("BrokenShield", only.Kind);
    }

    [Fact]
    public void HorseSuppressesBrokenShieldToo()
    {
        var plans = ShieldStatusEffectRender.Plan(
            boBrokenShield: true, nBrokenShieldEffect: 0,
            m_nState: ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP,
            m_btHorse: 1, boSkill31UseNewEffect: false,
            m_nCurrentAction: 0, m_nCurBubbleStruck: 0, m_nGenAniCount: 0);

        // 骑马时破碎盾不绘，且因 boBrokenShield 为真仍不进魔法盾分支
        Assert.Empty(plans);
    }

    // ===================== 3 级魔法盾：旧版 =====================

    [Fact]
    public void BubbleDefenceRequiresStateBit()
    {
        Assert.Null(ShieldStatusEffectRender.PlanBubbleDefence(0, 0, false, 0, 0, 0));
    }

    [Fact]
    public void BubbleDefenceRequiresNoHorse()
    {
        Assert.Null(ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 1, false, 0, 0, 0));
    }

    [Fact]
    public void BubbleDefenceOldAnimatedIndex()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, 0, 0, 7);

        Assert.NotNull(p);
        Assert.Equal(3890 + (7 % 3), p!.ImageIndex);      // 3891
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic, p.ImageSet);
        Assert.Equal(0, p.OffsetY);
    }

    [Fact]
    public void BubbleDefenceOldStruckIndex()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, SM_STRUCK, 2, 0);

        Assert.Equal(3900 + 2, p!.ImageIndex);
    }

    [Fact]
    public void BubbleDefenceStruckRequiresCountBelowThree()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, SM_STRUCK, 3, 6);

        Assert.Equal(3890 + (6 % 3), p!.ImageIndex);      // 落到动画分支
    }

    [Fact]
    public void BubbleDefenceStruckRequiresActionStruck()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, 0, 0, 4);

        Assert.Equal(3890 + (4 % 3), p!.ImageIndex);
    }

    // ===================== 3 级魔法盾：新版 =====================

    [Fact]
    public void BubbleDefenceNewAnimatedIndex()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, true, 0, 0, 5);

        Assert.NotNull(p);
        Assert.Equal(910 + (5 % 3), p!.ImageIndex);       // 912
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagicre, p.ImageSet);
    }

    [Fact]
    public void BubbleDefenceNewStruckIndex()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, true, SM_STRUCK, 1, 0);

        Assert.Equal(920 + 1, p!.ImageIndex);
    }

    [Fact]
    public void BubbleDefenceNewShiftsYByMinusSix()
    {
        // 16893：`ay := ay - 6`
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, true, 0, 0, 0);

        Assert.Equal(-6, p!.OffsetY);
    }

    [Fact]
    public void BubbleDefenceOldHasNoYShift()
    {
        var p = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, 0, 0, 0);

        Assert.Equal(0, p!.OffsetY);
    }

    [Fact]
    public void NewAndOldUseDifferentImageSets()
    {
        var oldP = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, 0, 0, 0);
        var newP = ShieldStatusEffectRender.PlanBubbleDefence(
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, true, 0, 0, 0);

        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic, oldP!.ImageSet);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagicre, newP!.ImageSet);
    }

    // ===================== 4 级魔法盾 =====================

    [Fact]
    public void Magic6RequiresState16()
    {
        Assert.Null(ShieldStatusEffectRender.PlanMagic6(0, 0, 0, 0, 0));
    }

    [Fact]
    public void Magic6RequiresNoHorse()
    {
        Assert.Null(ShieldStatusEffectRender.PlanMagic6(ShieldStatusEffectRender.STATE_16, 1, 0, 0, 0));
    }

    [Fact]
    public void Magic6AnimatedIndex()
    {
        var p = ShieldStatusEffectRender.PlanMagic6(ShieldStatusEffectRender.STATE_16, 0, 0, 0, 8);
        Assert.Equal(720 + (8 % 3), p!.ImageIndex);       // 722
    }

    [Fact]
    public void Magic6StruckIndex()
    {
        var p = ShieldStatusEffectRender.PlanMagic6(ShieldStatusEffectRender.STATE_16, 0, SM_STRUCK, 1, 0);
        Assert.Equal(723 + 1, p!.ImageIndex);
    }

    [Fact]
    public void Magic6HasNoOffset()
    {
        // 16906-16924 无坐标修正（与 3 级新版不同）
        var p = ShieldStatusEffectRender.PlanMagic6(ShieldStatusEffectRender.STATE_16, 0, 0, 0, 0);
        Assert.Equal(0, p!.OffsetY);
    }

    // ===================== 新武力盾 =====================

    [Fact]
    public void NewHitBubbleRequiresStateBit()
    {
        Assert.Null(ShieldStatusEffectRender.PlanNewHitBubble(0, 0));
    }

    [Fact]
    public void NewHitBubbleRequiresNoHorse()
    {
        Assert.Null(ShieldStatusEffectRender.PlanNewHitBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP, 1));
    }

    [Fact]
    public void NewHitBubbleIndexIsAlways494()
    {
        // 16930 是固定值；16944-16958 的动画版已被整段注释掉
        var p = ShieldStatusEffectRender.PlanNewHitBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP, 0);

        Assert.Equal(494, p!.ImageIndex);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic6, p.ImageSet);
    }

    // ===================== 新道力盾 =====================

    [Fact]
    public void NewMagBubbleRequiresItsOwnStateBit()
    {
        // 0x00020000，与 0x00040000 不同
        Assert.Null(ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP, 0, 0, 0, 0));
        Assert.NotNull(ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2, 0, 0, 0, 0));
    }

    [Fact]
    public void NewMagBubbleRequiresNoHorse()
    {
        Assert.Null(ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2, 1, 0, 0, 0));
    }

    [Fact]
    public void NewMagBubbleAnimatedIndex()
    {
        var p = ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2, 0, 0, 0, 4);

        Assert.Equal(720 + (4 % 3), p!.ImageIndex);       // 721
    }

    [Fact]
    public void NewMagBubbleStruckIndex()
    {
        var p = ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2, 0, SM_STRUCK, 2, 0);

        Assert.Equal(723 + 2, p!.ImageIndex);
    }

    [Fact]
    public void NewMagBubbleShiftsYByPlusTwo()
    {
        // 16996 的 `+ 2`
        var p = ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2, 0, 0, 0, 0);

        Assert.Equal(2, p!.OffsetY);
    }

    [Fact]
    public void NewMagBubbleUsesItsOwnCounters()
    {
        // 用的是 m_nCurNewMagBubbleStruck / m_nGenNewMagAniCount，与 4 级盾不同字段
        var p = ShieldStatusEffectRender.PlanNewMagBubble(
            ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2, 0, SM_STRUCK, 0, 0);

        Assert.Equal(723, p!.ImageIndex);                 // 用传入的 NewMag 计数 0
    }

    // ===================== 组合 =====================

    [Fact]
    public void NoStateNoPlans()
    {
        var plans = ShieldStatusEffectRender.Plan(false, 0, 0, 0, false, 0, 0, 0);
        Assert.Empty(plans);
    }

    [Fact]
    public void AllFourShieldsCanCoexist()
    {
        int state = ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP
            | ShieldStatusEffectRender.STATE_16
            | ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP
            | ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2;

        var plans = ShieldStatusEffectRender.Plan(false, 0, state, 0, false, 0, 0, 0);

        Assert.Equal(4, plans.Count);
        Assert.Equal("BubbleDefenceOld", plans[0].Kind);
        Assert.Equal("Magic6", plans[1].Kind);
        Assert.Equal("NewHitBubble", plans[2].Kind);
        Assert.Equal("NewMagBubble", plans[3].Kind);
    }

    [Fact]
    public void PlanOrderFollowsSourceOrder()
    {
        int state = ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP | ShieldStatusEffectRender.STATE_16;
        var plans = ShieldStatusEffectRender.Plan(false, 0, state, 0, false, 0, 0, 0);

        Assert.Equal("BubbleDefenceOld", plans[0].Kind);
        Assert.Equal("Magic6", plans[1].Kind);
    }

    [Fact]
    public void HorseSuppressesEntireFamily()
    {
        int state = ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP
            | ShieldStatusEffectRender.STATE_16
            | ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP
            | ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2;

        Assert.Empty(ShieldStatusEffectRender.Plan(false, 0, state, 1, false, 0, 0, 0));
    }

    // ===================== Draw =====================

    private static List<ShieldStatusEffectRender.ShieldPlan> OnePlan()
        => ShieldStatusEffectRender.Plan(false, 0,
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, false, 0, 0, 0);

    [Fact]
    public void DrawAppliesShiftAndOrigin()
    {
        var ops = ShieldStatusEffectRender.Draw(OnePlan(), 10, 20, 3, 4, false,
            (set, idx, gray) => new FxImage(idx, 16, 16, 1, 2, gray));

        var op = Assert.Single(ops);
        Assert.Equal(10 + 1 + 3, op.X);
        Assert.Equal(20 + 2 + 4, op.Y);
    }

    [Fact]
    public void DrawAppliesPlanOffsetY()
    {
        // 3 级新版有 -6 修正
        var plans = ShieldStatusEffectRender.Plan(false, 0,
            ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP, 0, true, 0, 0, 0);

        var ops = ShieldStatusEffectRender.Draw(plans, 0, 0, 0, 0, false,
            (set, idx, gray) => new FxImage(idx, 16, 16, 0, 0, gray));

        Assert.Equal(-6, Assert.Single(ops).Y);
    }

    [Fact]
    public void DrawPassesGrayWhenSelfDead()
    {
        var ops = ShieldStatusEffectRender.Draw(OnePlan(), 0, 0, 0, 0, true,
            (set, idx, gray) => new FxImage(idx, 16, 16, 0, 0, gray));

        Assert.True(Assert.Single(ops).Gray);
    }

    [Fact]
    public void DrawPassesColorWhenAlive()
    {
        var ops = ShieldStatusEffectRender.Draw(OnePlan(), 0, 0, 0, 0, false,
            (set, idx, gray) => new FxImage(idx, 16, 16, 0, 0, gray));

        Assert.False(Assert.Single(ops).Gray);
    }

    [Fact]
    public void DrawSkipsMissingTexture()
    {
        var ops = ShieldStatusEffectRender.Draw(OnePlan(), 0, 0, 0, 0, false,
            (set, idx, gray) => null);

        Assert.Empty(ops);
    }

    [Fact]
    public void DrawRoutesEachPlanToItsImageSet()
    {
        int state = ShieldStatusEffectRender.STATE_BUBBLEDEFENCEUP
            | ShieldStatusEffectRender.STATE_16
            | ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP
            | ShieldStatusEffectRender.STATE_NEWHITBUBBLEDEFENCEUP_2;
        var plans = ShieldStatusEffectRender.Plan(false, 0, state, 0, false, 0, 0, 0);

        var seen = new List<ShieldStatusEffectRender.ShieldImageSet>();
        ShieldStatusEffectRender.Draw(plans, 0, 0, 0, 0, false,
            (set, idx, gray) => { seen.Add(set); return new FxImage(idx, 16, 16, 0, 0, gray); });

        Assert.Equal(4, seen.Count);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic, seen[0]);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic6, seen[1]);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic6, seen[2]);
        Assert.Equal(ShieldStatusEffectRender.ShieldImageSet.WMagic6, seen[3]);
    }

    [Fact]
    public void EmptyPlansProduceNoOps()
    {
        Assert.Empty(ShieldStatusEffectRender.Draw(new List<ShieldStatusEffectRender.ShieldPlan>(),
            0, 0, 0, 0, false, (s, i, g) => new FxImage(i, 1, 1, 0, 0, g)));
    }
}

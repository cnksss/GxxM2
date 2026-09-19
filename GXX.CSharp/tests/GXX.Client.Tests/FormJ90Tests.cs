using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J90：Actor.pas THumActor.DrawChr 魔法/攻击特效派发族 1:1 测试 ——
/// 跟随动作效果(17002-17013)、施法起手(17017-17139)、魔法结束(17143-17161)、攻击特效(17171-17240)。
/// </summary>
public sealed class MagicEffectDispatchRenderTests
{
    private static EffectBaseImage Base(int idx, MagicImgLib lib = MagicImgLib.WMagicImages)
        => new(lib, idx);

    private static Func<int, int, EffectBaseImage> Stub(int idx = 500, MagicImgLib lib = MagicImgLib.WMagicImages)
        => (mag, level) => Base(idx, lib);

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(100, MagicEffectDispatchRender.EffectNumberExcludedLow);
        Assert.Equal(111, MagicEffectDispatchRender.EffectNumberExcludedHigh);
        Assert.Equal(29, MagicEffectDispatchRender.ShieldSpellEffectNumber);
        Assert.Equal(45, MagicEffectDispatchRender.FireOfHeavenSerial);
        Assert.Equal(80, MagicEffectDispatchRender.FireOfHeavenBaseIndex);
        Assert.Equal(900, MagicEffectDispatchRender.NewShieldEffectBase4Level);
        Assert.Equal(880, MagicEffectDispatchRender.NewShieldEffectBaseNormal);
        Assert.Equal(2, MagicEffectDispatchRender.NewShieldOffsetX);
        Assert.Equal(-6, MagicEffectDispatchRender.NewShieldOffsetY);
        Assert.Equal(68, MagicEffectDispatchRender.NewPhysicalShieldEffectNumber);
        Assert.Equal(1608, MagicEffectDispatchRender.NewPhysicalShieldBaseIndex);
        Assert.Equal(10, MagicEffectDispatchRender.SoulFireSymbolEffectNumber);
        Assert.Equal(120, MagicEffectDispatchRender.SoulFireSymbolBaseIndex);
        Assert.Equal(4, MagicEffectDispatchRender.NewPhysicalShieldFrameCap);
        Assert.Equal(4, MagicEffectDispatchRender.PoisonEffectNumber);
        Assert.Equal(210, MagicEffectDispatchRender.PoisonItemOffset);
        Assert.Equal(4, MagicEffectDispatchRender.FireHitEffectNumber);
        Assert.Equal(23, MagicEffectDispatchRender.ChasingHeartHitEffectNumber);
    }

    // ===================== ① 跟随动作效果 =====================

    [Fact]
    public void UseEffectGateRequiresBoth()
    {
        Assert.True(MagicEffectDispatchRender.UseEffectGate(true, true));
        Assert.False(MagicEffectDispatchRender.UseEffectGate(false, true));
        Assert.False(MagicEffectDispatchRender.UseEffectGate(true, false));
    }

    // ===================== ② 施法起手门禁 =====================

    [Fact]
    public void SpellGateRequiresUseMagic()
    {
        Assert.False(MagicEffectDispatchRender.SpellGate(false, 5));
    }

    [Fact]
    public void SpellGateRequiresPositiveEffectNumber()
    {
        Assert.False(MagicEffectDispatchRender.SpellGate(true, 0));
        Assert.False(MagicEffectDispatchRender.SpellGate(true, -1));
    }

    [Fact]
    public void SpellGateExcludes100To111()
    {
        Assert.False(MagicEffectDispatchRender.SpellGate(true, 100));
        Assert.False(MagicEffectDispatchRender.SpellGate(true, 111));
        Assert.True(MagicEffectDispatchRender.SpellGate(true, 99));
        Assert.True(MagicEffectDispatchRender.SpellGate(true, 112));
    }

    // ===================== 连击退出（唯一中断 DrawChr 的路径） =====================

    [Fact]
    public void ContinuousShieldExitsForSelf()
    {
        // 17024：EffectNumber = 29 且 self = g_MySelf 且 IsInContinuous
        Assert.True(MagicEffectDispatchRender.ShouldExitForContinuousShield(29, true, true));
    }

    [Fact]
    public void ContinuousShieldDoesNotExitForOthers()
    {
        Assert.False(MagicEffectDispatchRender.ShouldExitForContinuousShield(29, false, true));
    }

    [Fact]
    public void ContinuousShieldDoesNotExitWithoutContinuous()
    {
        Assert.False(MagicEffectDispatchRender.ShouldExitForContinuousShield(29, true, false));
    }

    [Fact]
    public void ContinuousShieldOnlyForEffect29()
    {
        Assert.False(MagicEffectDispatchRender.ShouldExitForContinuousShield(28, true, true));
        Assert.False(MagicEffectDispatchRender.ShouldExitForContinuousShield(30, true, true));
    }

    // ===================== 4 级灭天火特例 =====================

    [Fact]
    public void FireOfHeavenSpecialCondition()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            magicSerial: 45, effectNumber: 5, magicLevel: 4, newLevel: 0,
            btJob: 0, curEffFrame: 3, boSkill31UseNewEffect: false, magicItemType: false,
            getEffectBase: Stub());

        Assert.Equal("FireOfHeaven", p.Kind);
        Assert.Equal(MagicImgLib.WMagic6Images, p.Lib);
        Assert.Equal(83, p.Index);                    // 80 + 3
    }

    [Fact]
    public void FireOfHeavenNeedsSerial45()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            magicSerial: 44, effectNumber: 5, magicLevel: 4, newLevel: 0,
            btJob: 0, curEffFrame: 3, boSkill31UseNewEffect: false, magicItemType: false,
            getEffectBase: Stub());

        Assert.NotEqual("FireOfHeaven", p.Kind);
    }

    [Fact]
    public void FireOfHeavenNeedsMagicLevel4()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            45, 5, magicLevel: 3, newLevel: 0, btJob: 0, curEffFrame: 3,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.NotEqual("FireOfHeaven", p.Kind);
    }

    [Fact]
    public void FireOfHeavenNeedsNewLevelZero()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            45, 5, magicLevel: 4, newLevel: 1, btJob: 0, curEffFrame: 3,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.NotEqual("FireOfHeaven", p.Kind);
    }

    // ===================== 新魔法盾起手 =====================

    [Fact]
    public void NewShieldUses900ForEnhanced()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 29, magicLevel: 0, newLevel: 1, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: true, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(900, p.Index);
        Assert.Equal(MagicImgLib.WMagicreImages, p.Lib);
        Assert.Equal(2, p.OffsetX);
        Assert.Equal(-6, p.OffsetY);
    }

    [Fact]
    public void NewShieldUses900ForMagicLevel4()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 29, magicLevel: 4, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: true, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(900, p.Index);
    }

    [Fact]
    public void NewShieldUses880ForNormal()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 29, magicLevel: 0, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: true, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(880, p.Index);
    }

    [Fact]
    public void NewShieldBranchOnlyForEffect29()
    {
        // boSkill31UseNewEffect 为真但 EffectNumber ≠ 29 时走 else
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 5, magicLevel: 0, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: true, magicItemType: false, getEffectBase: Stub(700));

        Assert.Equal(700, p.Index);
        Assert.Equal(0, p.OffsetX);
        Assert.Equal(0, p.OffsetY);
    }

    // ===================== 4 级强化盾（旧路径，NewLevel 传 1） =====================

    [Fact]
    public void EnhancedShieldUsesNewLevel1Lookup()
    {
        // 直接比对查表结果，确认 NewLevel 固定为 1
        var viaBranch = MagicEffectDispatchRender.PlanSpellEffect(
            0, 29, magicLevel: 4, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(999));

        var expected = MagicEffectBaseLookup.Resolve(29 - 1, 0, 1, 0, 0);
        Assert.Equal(expected.BaseIndex + 0, viaBranch.Index);
        Assert.NotEqual(999, viaBranch.Index);
    }

    [Fact]
    public void EnhancedShieldDoesNotApplyForNormalLevel()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 29, magicLevel: 0, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(700));

        Assert.Equal(700, p.Index);                    // 走 else 接缝
    }

    // ===================== 4 级新武力盾起手 =====================

    [Fact]
    public void NewPhysicalShieldUses1608()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 68, magicLevel: 0, newLevel: 1, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(1608, p.Index);
        Assert.Equal(MagicImgLib.WMagic10Images, p.Lib);
    }

    [Fact]
    public void NewPhysicalShieldNeedsBoost()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 68, magicLevel: 0, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(700));

        Assert.Equal(700, p.Index);
    }

    // ===================== 4 级灵魂火符起手 =====================

    [Fact]
    public void SoulFireSymbolUses120()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 10, magicLevel: 4, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(120, p.Index);
        Assert.Equal(MagicImgLib.WMagic6Images, p.Lib);
    }

    [Fact]
    public void SoulFireSymbolNeedsMagicLevelAtLeast4()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 10, magicLevel: 3, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(700));

        Assert.Equal(700, p.Index);
    }

    [Fact]
    public void SoulFireSymbolUsesGeNotEquals()
    {
        // 原文是 `MagicLevel >= 4`
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 10, magicLevel: 5, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(120, p.Index);
    }

    // ===================== 职业相关偏移 =====================

    [Fact]
    public void Effect61OffsetsOnlyForJob2()
    {
        var job2 = MagicEffectDispatchRender.PlanSpellEffect(
            0, 61, 0, 0, btJob: 2, curEffFrame: 0, false, false, Stub(500));
        var job1 = MagicEffectDispatchRender.PlanSpellEffect(
            0, 61, 0, 0, btJob: 1, curEffFrame: 0, false, false, Stub(500));

        Assert.Equal(480, job2.Index);
        Assert.Equal(500, job1.Index);
    }

    [Fact]
    public void Effect62OffsetsByJobTimes20()
    {
        var j0 = MagicEffectDispatchRender.PlanSpellEffect(0, 62, 0, 0, 0, 0, false, false, Stub(500));
        var j1 = MagicEffectDispatchRender.PlanSpellEffect(0, 62, 0, 0, 1, 0, false, false, Stub(500));
        var j2 = MagicEffectDispatchRender.PlanSpellEffect(0, 62, 0, 0, 2, 0, false, false, Stub(500));

        Assert.Equal(500, j0.Index);
        Assert.Equal(480, j1.Index);
        Assert.Equal(460, j2.Index);
    }

    [Fact]
    public void Effect64OffsetsPositiveForJob2()
    {
        var job2 = MagicEffectDispatchRender.PlanSpellEffect(0, 64, 0, 0, 2, 0, false, false, Stub(500));
        var job0 = MagicEffectDispatchRender.PlanSpellEffect(0, 64, 0, 0, 0, 0, false, false, Stub(500));

        Assert.Equal(520, job2.Index);
        Assert.Equal(500, job0.Index);
    }

    // ===================== 帧叠加 =====================

    [Fact]
    public void NormalEffectAddsCurEffFrame()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 5, 0, 0, 0, curEffFrame: 7, false, false, Stub(500));

        Assert.Equal(507, p.Index);
    }

    [Fact]
    public void NewPhysicalShieldCapsFrameAt4()
    {
        // 17100：EffectNumber = 68 且 m_nCurEffFrame > 4 → 只加 4
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 68, magicLevel: 4, newLevel: 0, btJob: 0, curEffFrame: 9,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(1608 + 4, p.Index);
    }

    [Fact]
    public void NewPhysicalShieldAddsFrameWhenAtMost4()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 68, magicLevel: 4, newLevel: 0, btJob: 0, curEffFrame: 4,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(1608 + 4, p.Index);
    }

    [Fact]
    public void NewPhysicalShieldFrameCapIsExclusiveBoundary()
    {
        // curEffFrame = 5 > 4 → 加 4（而非 5）
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 68, magicLevel: 4, newLevel: 0, btJob: 0, curEffFrame: 5,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub());

        Assert.Equal(1608 + 4, p.Index);
    }

    // ===================== 施毒术 =====================

    [Fact]
    public void PoisonAdds210WhenNoItem()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 4, magicLevel: 0, newLevel: 1, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(500));

        Assert.Equal(710, p.Index);
    }

    [Fact]
    public void PoisonDoesNotAddWhenHoldingItem()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 4, magicLevel: 0, newLevel: 1, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: true, getEffectBase: Stub(500));

        Assert.Equal(500, p.Index);
    }

    [Fact]
    public void PoisonDoesNotAddWithoutNewLevel()
    {
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 4, magicLevel: 0, newLevel: 0, btJob: 0, curEffFrame: 0,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(500));

        Assert.Equal(500, p.Index);
    }

    [Fact]
    public void PoisonOffsetAppliesAfterFrameAdd()
    {
        // 顺序：先加帧再加 210
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 4, magicLevel: 0, newLevel: 1, btJob: 0, curEffFrame: 3,
            boSkill31UseNewEffect: false, magicItemType: false, getEffectBase: Stub(500));

        Assert.Equal(500 + 3 + 210, p.Index);
    }

    [Fact]
    public void NegativeBaseIndexSkipsAllAdjustments()
    {
        // 17081：idx < 0 时整段跳过
        var p = MagicEffectDispatchRender.PlanSpellEffect(
            0, 61, 0, 0, btJob: 2, curEffFrame: 5, false, false, Stub(-1));

        Assert.Equal(-1, p.Index);
    }

    // ===================== ③ 魔法结束特效 =====================

    [Fact]
    public void MagicEndRequiresFlagAndEffectNumber()
    {
        Assert.Null(MagicEffectDispatchRender.PlanMagicEndEffect(
            false, 5, 0, 0, 10, 0, 0, 0, Stub()));
        Assert.Null(MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 0, 0, 0, 10, 0, 0, 0, Stub()));
    }

    [Fact]
    public void MagicEndRequiresFrameInRange()
    {
        Assert.Null(MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 5, 0, curEffFrame: 10, spellFrame: 10, 0, 0, 0, Stub()));
        Assert.NotNull(MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 5, 0, curEffFrame: 9, spellFrame: 10, 0, 0, 0, Stub()));
    }

    [Fact]
    public void MagicEndIndexUsesThreeMagicFrameFields()
    {
        // 17147：+ (m_nCurrentMagicFrame - m_nStartMagicFrame + m_nStartPosMagicFrame)
        var p = MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 5, 0, curEffFrame: 0, spellFrame: 10,
            currentMagicFrame: 12, startMagicFrame: 4, startPosMagicFrame: 3, Stub(500));

        Assert.NotNull(p);
        Assert.Equal(500 + (12 - 4 + 3), p!.Index);      // 511
    }

    [Fact]
    public void MagicEndDoesNotUseCurEffFrame()
    {
        // 与本族其它成员不同：不加 m_nCurEffFrame
        var a = MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 5, 0, curEffFrame: 0, spellFrame: 10, 0, 0, 0, Stub(500));
        var b = MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 5, 0, curEffFrame: 7, spellFrame: 10, 0, 0, 0, Stub(500));

        Assert.Equal(a!.Index, b!.Index);
    }

    [Fact]
    public void MagicEndSkipsWhenBaseNegative()
    {
        Assert.Null(MagicEffectDispatchRender.PlanMagicEndEffect(
            true, 5, 0, 0, 10, 0, 0, 0, Stub(-1)));
    }

    // ===================== ④ 攻击特效 =====================

    [Fact]
    public void HitEffectStrides()
    {
        Assert.Equal(20, MagicEffectDispatchRender.HitEffectDirStride(7));
        Assert.Equal(20, MagicEffectDispatchRender.HitEffectDirStride(8));
        Assert.Equal(20, MagicEffectDispatchRender.HitEffectDirStride(9));
        Assert.Equal(20, MagicEffectDispatchRender.HitEffectDirStride(20));
        Assert.Equal(20, MagicEffectDispatchRender.HitEffectDirStride(26));
        Assert.Equal(10, MagicEffectDispatchRender.HitEffectDirStride(10));
        Assert.Equal(10, MagicEffectDispatchRender.HitEffectDirStride(12));
        Assert.Equal(10, MagicEffectDispatchRender.HitEffectDirStride(25));
        Assert.Equal(10, MagicEffectDispatchRender.HitEffectDirStride(14));
        Assert.Equal(10, MagicEffectDispatchRender.HitEffectDirStride(27));
        Assert.Equal(10, MagicEffectDispatchRender.HitEffectDirStride(23));
    }

    [Fact]
    public void UnknownHitEffectFallsToElseStride10()
    {
        Assert.Null(MagicEffectDispatchRender.HitEffectDirStride(99));

        var p = MagicEffectDispatchRender.PlanHitEffect(
            99, 0, 0, btDir: 2, currentFrame: 6, startFrame: 1, currentAction: 5,
            500, MagicImgLib.WMagicImages);

        Assert.Equal(500 + 2 * 10 + 5, p!.Index);
    }

    [Fact]
    public void HitEffectAppliesDirAndFrame()
    {
        var p = MagicEffectDispatchRender.PlanHitEffect(
            7, 0, 0, btDir: 3, currentFrame: 10, startFrame: 4, currentAction: 5,
            500, MagicImgLib.WMagicImages);

        Assert.Equal(500 + 3 * 20 + 6, p!.Index);
    }

    [Fact]
    public void ChasingHeartSkipsWhenNoAction()
    {
        // 17208-17209：m_nCurrentAction = 0 → idx := -1
        var p = MagicEffectDispatchRender.PlanHitEffect(
            23, 0, 0, btDir: 1, currentFrame: 5, startFrame: 0, currentAction: 0,
            500, MagicImgLib.WMagicImages);

        Assert.Null(p);
    }

    [Fact]
    public void ChasingHeartDrawsWhenActionNonZero()
    {
        var p = MagicEffectDispatchRender.PlanHitEffect(
            23, 0, 0, btDir: 1, currentFrame: 5, startFrame: 0, currentAction: 3,
            500, MagicImgLib.WMagicImages);

        Assert.NotNull(p);
        Assert.Equal(500 + 1 * 10 + 5, p!.Index);
    }

    [Fact]
    public void Fire4SpecialResetsIndexToZeroBase()
    {
        // 17202-17204：idx := 0 + dir*10 + 帧差，并换 WMagic6Images
        var p = MagicEffectDispatchRender.PlanHitEffect(
            4, hitEffectLevel: 0, hitEffectLevel2: 4, btDir: 2,
            currentFrame: 7, startFrame: 1, currentAction: 5,
            500, MagicImgLib.WMagicImages);

        Assert.Equal("HitFire4", p!.Kind);
        Assert.Equal(0 + 2 * 10 + 6, p.Index);
        Assert.Equal(MagicImgLib.WMagic6Images, p.Lib);
    }

    [Fact]
    public void Fire4SpecialNeedsLevel2Four()
    {
        var p = MagicEffectDispatchRender.PlanHitEffect(
            4, hitEffectLevel: 0, hitEffectLevel2: 3, btDir: 2,
            currentFrame: 7, startFrame: 1, currentAction: 5,
            500, MagicImgLib.WMagicImages);

        Assert.NotEqual("HitFire4", p!.Kind);
        Assert.Equal(500 + 2 * 10 + 6, p.Index);
    }

    [Fact]
    public void Fire4SpecialNeedsHitEffectLevelZero()
    {
        var p = MagicEffectDispatchRender.PlanHitEffect(
            4, hitEffectLevel: 1, hitEffectLevel2: 4, btDir: 2,
            currentFrame: 7, startFrame: 1, currentAction: 5,
            500, MagicImgLib.WMagicImages);

        Assert.NotEqual("HitFire4", p!.Kind);
    }

    // ===================== 绘制 =====================

    [Fact]
    public void DrawAppliesShiftOriginAndPlanOffsets()
    {
        var plan = new MagicEffectDispatchRender.SpellEffectPlan(
            MagicImgLib.WMagicImages, 100, 2, -6, "Spell");

        var op = MagicEffectDispatchRender.Draw(plan, dx: 10, dy: 20, shiftX: 3, shiftY: 4,
            selfDead: false, originX: 1, originY: 2);

        Assert.NotNull(op);
        Assert.Equal(10 + 1 + 3 + 2, op!.X);
        Assert.Equal(20 + 2 + 4 - 6, op.Y);
    }

    [Fact]
    public void DrawSkipsNegativeIndex()
    {
        var plan = new MagicEffectDispatchRender.SpellEffectPlan(
            MagicImgLib.WMagicImages, -1, 0, 0, "Spell");

        Assert.Null(MagicEffectDispatchRender.Draw(plan, 0, 0, 0, 0, false, 0, 0));
    }

    [Fact]
    public void DrawCarriesLibraryAndIndex()
    {
        var plan = new MagicEffectDispatchRender.SpellEffectPlan(
            MagicImgLib.WMagic10Images, 1608, 0, 0, "Spell");

        var op = MagicEffectDispatchRender.Draw(plan, 0, 0, 0, 0, false, 0, 0);

        Assert.Equal(MagicImgLib.WMagic10Images, op!.Lib);
        Assert.Equal(1608, op.ImageIndex);
    }

    [Fact]
    public void DrawWithoutOffsetsMatchesPlainPosition()
    {
        var plan = new MagicEffectDispatchRender.SpellEffectPlan(
            MagicImgLib.WMagicImages, 5, 0, 0, "Hit");

        var op = MagicEffectDispatchRender.Draw(plan, 7, 9, 1, 2, false, 3, 4);

        Assert.Equal(7 + 3 + 1, op!.X);
        Assert.Equal(9 + 4 + 2, op.Y);
    }
}

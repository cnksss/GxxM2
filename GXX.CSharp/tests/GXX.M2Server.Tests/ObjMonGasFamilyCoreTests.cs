using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J242：`ObjMon.pas` 中气/蛛网三族（`TGasAttackMonster`、`TGasMothMonster`、`TGasDungMonster`）
/// 十个方法的 1:1 测试（167 行）。
/// **本批最有价值的发现**：
/// ① `sub_4A9C78` 是本系列**第一次明确出现模板方法模式**（基类 `virtual` 做全套并把"打到了谁"
///    作为 `TBaseObject` **返回**；子类 `override` 后调 `inherited` 再追加"把隐身目标显形"）；
///    两处声明行**共用同一个 VMT 槽位号 `// FFEA`** —— 也是第一次把 VMT 标签写在**声明行**上；
/// ② 伤害是**内联掷骰**、并把**反编译原式（`HiWord`/`LoWord` 位运算）保留成注释**；
/// ③ `TGasMothMonster.Run` 的走位节流**只更新了一半**（清 `m_nWalkDelay` 却从不更新
///    `m_dwWalkTick`）⇒ 节流永远开着 —— 与 J238 **互为镜像**；
/// ④ `TGasDungMonster` 的全部区别就是视距 7（`Create` 与楔蛾逐字相同、`Destroy` 纯空壳、
///    既不覆写 `Run` 也不覆写 `sub_4A9C78`）。
/// </summary>
public sealed class ObjMonGasFamilyCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(1710, ObjMonGasFamilyCore.GasCreateStart);
        Assert.Equal(1715, ObjMonGasFamilyCore.GasCreateEnd);
        Assert.Equal(6, ObjMonGasFamilyCore.GasCreateLines);
        Assert.Equal(1717, ObjMonGasFamilyCore.GasDestroyStart);
        Assert.Equal(1720, ObjMonGasFamilyCore.GasDestroyEnd);
        Assert.Equal(4, ObjMonGasFamilyCore.GasDestroyLines);
        Assert.Equal(1722, ObjMonGasFamilyCore.SubStart);
        Assert.Equal(1807, ObjMonGasFamilyCore.SubEnd);
        Assert.Equal(86, ObjMonGasFamilyCore.SubLines);
        Assert.Equal(1809, ObjMonGasFamilyCore.AttackStart);
        Assert.Equal(1835, ObjMonGasFamilyCore.AttackEnd);
        Assert.Equal(27, ObjMonGasFamilyCore.AttackLines);
        Assert.Equal(2680, ObjMonGasFamilyCore.MothCreateStart);
        Assert.Equal(2684, ObjMonGasFamilyCore.MothCreateEnd);
        Assert.Equal(5, ObjMonGasFamilyCore.MothCreateLines);
        Assert.Equal(2686, ObjMonGasFamilyCore.MothDestroyStart);
        Assert.Equal(2689, ObjMonGasFamilyCore.MothDestroyEnd);
        Assert.Equal(4, ObjMonGasFamilyCore.MothDestroyLines);
        Assert.Equal(2691, ObjMonGasFamilyCore.MothSubStart);
        Assert.Equal(2701, ObjMonGasFamilyCore.MothSubEnd);
        Assert.Equal(11, ObjMonGasFamilyCore.MothSubLines);
        Assert.Equal(2703, ObjMonGasFamilyCore.MothRunStart);
        Assert.Equal(2717, ObjMonGasFamilyCore.MothRunEnd);
        Assert.Equal(15, ObjMonGasFamilyCore.MothRunLines);
        Assert.Equal(2720, ObjMonGasFamilyCore.DungCreateStart);
        Assert.Equal(2724, ObjMonGasFamilyCore.DungCreateEnd);
        Assert.Equal(5, ObjMonGasFamilyCore.DungCreateLines);
        Assert.Equal(2726, ObjMonGasFamilyCore.DungDestroyStart);
        Assert.Equal(2729, ObjMonGasFamilyCore.DungDestroyEnd);
        Assert.Equal(4, ObjMonGasFamilyCore.DungDestroyLines);
        Assert.Equal(167, ObjMonGasFamilyCore.TotalLines);
        Assert.Equal(10, ObjMonGasFamilyCore.MethodCount);
        Assert.Equal(3, ObjMonGasFamilyCore.ClassCount);

        Assert.Equal(2695, ObjMonGasFamilyCore.InheritedCallLine);
        Assert.Equal(2696, ObjMonGasFamilyCore.UnhideGateLine);
        Assert.Equal(2698, ObjMonGasFamilyCore.UnhideSetLine);
        Assert.Equal(8, ObjMonGasFamilyCore.STATE_TRANSPARENT);
        Assert.Equal(15, ObjMonGasFamilyCore.TransparentDeclLine);
        Assert.Equal("0x70", ObjMonGasFamilyCore.BraceOffset);
        Assert.Equal(332, ObjMonGasFamilyCore.BaseDeclLine);
        Assert.Equal(449, ObjMonGasFamilyCore.SubDeclLine);
        Assert.Equal("FFEA", ObjMonGasFamilyCore.VmtSlot);

        Assert.Equal(1732, ObjMonGasFamilyCore.AliasLine);
        Assert.Equal(1733, ObjMonGasFamilyCore.DiceBoundLine);
        Assert.Equal(1734, ObjMonGasFamilyCore.DiceGuardLine);
        Assert.Equal(1735, ObjMonGasFamilyCore.DiceRollLine);
        Assert.Equal(1736, ObjMonGasFamilyCore.DiceAddLine);
        Assert.Equal(1737, ObjMonGasFamilyCore.OriginalCommentLine);
        Assert.Equal(1747, ObjMonGasFamilyCore.AccuracyGateLine);
        Assert.Equal(165, ObjMonGasFamilyCore.HitPointDeclLine);
        Assert.Equal(176, ObjMonGasFamilyCore.SpeedPointDeclLine);
        Assert.Equal("Word", ObjMonGasFamilyCore.DeclaredType);
        Assert.Equal("Byte", ObjMonGasFamilyCore.CommentedType);
        Assert.Equal(1743, ObjMonGasFamilyCore.PoseCreateLine);
        Assert.Equal(3, ObjMonGasFamilyCore.PoseCreateOverloads);
        Assert.Equal(1788, ObjMonGasFamilyCore.StoneGateLine);
        Assert.Equal(20, ObjMonGasFamilyCore.CanStoneArg);
        Assert.Equal(5, ObjMonGasFamilyCore.StoneValue);
        Assert.Equal(1792, ObjMonGasFamilyCore.ParalysisLine);
        Assert.Equal("m_dwParalysisTime", ObjMonGasFamilyCore.ParalysisTimeField);
        Assert.Equal(796, ObjMonGasFamilyCore.CanStoneDeclLine);
        Assert.Equal(5, ObjMonGasFamilyCore.POISON_STONE);
        Assert.Equal(300, ObjMonGasFamilyCore.Delay);

        Assert.Equal(1814, ObjMonGasFamilyCore.NilGuardLine);
        Assert.Equal(1816, ObjMonGasFamilyCore.DirCheckLine);
        Assert.Equal(1818, ObjMonGasFamilyCore.CooldownLine);
        Assert.Equal(1823, ObjMonGasFamilyCore.SubCallLine);
        Assert.Equal(1824, ObjMonGasFamilyCore.BreakSeizeLine);
        Assert.Equal(1826, ObjMonGasFamilyCore.ResultTrueLine);
        Assert.Equal(1830, ObjMonGasFamilyCore.SameMapLine);
        Assert.Equal(11, ObjMonGasFamilyCore.TemplateConfirmations);
        Assert.Equal(5659, ObjMonGasFamilyCore.TemplateRangeLine);

        Assert.Equal(2705, ObjMonGasFamilyCore.WalkGateLine);
        Assert.Equal(2708, ObjMonGasFamilyCore.DelayClearLine);
        Assert.Equal(2709, ObjMonGasFamilyCore.ThrottleLine);
        Assert.Equal(2713, ObjMonGasFamilyCore.SubSearchCallLine);
        Assert.Equal(850, ObjMonGasFamilyCore.SubSearchDeclLine);
        Assert.Equal(2716, ObjMonGasFamilyCore.RunInheritedLine);
        Assert.Equal(8000, ObjMonGasFamilyCore.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonGasFamilyCore.SearchWithoutTargetMs);

        Assert.Equal(1713, ObjMonGasFamilyCore.SearchTimeLine);
        Assert.Equal(1500, ObjMonGasFamilyCore.SearchTimeBase);
        Assert.Equal(1500, ObjMonGasFamilyCore.SearchTimeBound);
        Assert.Equal(1714, ObjMonGasFamilyCore.AnimalFlagLine);
        Assert.Equal(2683, ObjMonGasFamilyCore.MothViewRangeLine);
        Assert.Equal(2723, ObjMonGasFamilyCore.DungViewRangeLine);
        Assert.Equal(7, ObjMonGasFamilyCore.ViewRangeValue);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonGasFamilyCore.SpanMatches());
        Assert.True(ObjMonGasFamilyCore.TotalLinesAddUp());
        Assert.True(ObjMonGasFamilyCore.BaseMethodsAscending());
        Assert.True(ObjMonGasFamilyCore.SubMethodsAscending());
        Assert.True(ObjMonGasFamilyCore.WithinUnit());
        Assert.True(ObjMonGasFamilyCore.NoInstrumentation());
    }

    // ===================== 一、模板方法 =====================

    [Fact]
    public void TemplateMethodFacts()
    {
        Assert.True(ObjMonGasFamilyCore.TemplateMethodPattern());
        Assert.True(ObjMonGasFamilyCore.BaseReturnsTheVictim());
        Assert.True(ObjMonGasFamilyCore.SubclassExtendsViaInherited());
        Assert.True(ObjMonGasFamilyCore.FirstTemplateMethod());
        Assert.True(ObjMonGasFamilyCore.MothUnhidesTarget());
    }

    [Fact]
    public void VmtLabelFacts()
    {
        Assert.True(ObjMonGasFamilyCore.VmtLabelOnDeclaration());
        Assert.True(ObjMonGasFamilyCore.SameSlotOnBothDecls());
        Assert.True(ObjMonGasFamilyCore.SlotProvesSameVirtual());
        Assert.True(ObjMonGasFamilyCore.FirstOnDeclaration());
    }

    [Fact]
    public void UnhideBoundaries()
    {
        Assert.True(ObjMonGasFamilyCore.AllThreeUnhide());
        Assert.True(ObjMonGasFamilyCore.NotHiddenSkips());
        Assert.True(ObjMonGasFamilyCore.MissedRollSkips());
        Assert.True(ObjMonGasFamilyCore.NullSkips());
        Assert.True(ObjMonGasFamilyCore.UnhideSetsOne());

        Assert.True(ObjMonGasFamilyCore.Unhides(true, 0, true));
        Assert.False(ObjMonGasFamilyCore.Unhides(true, 0, false));
        Assert.False(ObjMonGasFamilyCore.Unhides(true, 1, true));
        Assert.False(ObjMonGasFamilyCore.Unhides(false, 0, true));
    }

    [Fact]
    public void BraceAnnotationFacts()
    {
        Assert.True(ObjMonGasFamilyCore.BraceHasValueAndOffset());
        Assert.True(ObjMonGasFamilyCore.TwoPartAnnotation());
        Assert.Equal(8, ObjMonGasFamilyCore.STATE_TRANSPARENT);
        Assert.Equal("0x70", ObjMonGasFamilyCore.BraceOffset);
    }

    // ===================== 二、内联掷骰 =====================

    [Fact]
    public void InlineDiceFacts()
    {
        Assert.True(ObjMonGasFamilyCore.InlinedDiceRoll());
        Assert.True(ObjMonGasFamilyCore.DecompiledOriginalKeptAsComment());
        Assert.True(ObjMonGasFamilyCore.NoGetAttackPowerHere());
        Assert.True(ObjMonGasFamilyCore.BoundaryDiffersFromGetAttackPower());
        Assert.True(ObjMonGasFamilyCore.NegativeSkipsTheRoll());
        Assert.True(ObjMonGasFamilyCore.RollInRange());
        Assert.True(ObjMonGasFamilyCore.BoundIsInclusive());
        Assert.True(ObjMonGasFamilyCore.DiffersWhenNegative());
        Assert.True(ObjMonGasFamilyCore.OriginalHasTwoBitOps());
        Assert.True(ObjMonGasFamilyCore.AliasLineThrice());
        Assert.True(ObjMonGasFamilyCore.ThirdVariantOfTheLineage());
    }

    [Fact]
    public void InlineDiceBoundaries()
    {
        // **正常范围 [dc1, dc2]**
        Assert.Equal(10, ObjMonGasFamilyCore.InlineRoll(10, 20, 0));
        Assert.Equal(20, ObjMonGasFamilyCore.InlineRoll(10, 20, 10));

        // **差值 < 1 时退化为 `DC2 + 1`（不是 DC1、也不归零）**
        Assert.Equal(6, ObjMonGasFamilyCore.InlineRoll(10, 5, 0));
        Assert.Equal(11, ObjMonGasFamilyCore.GetAttackPowerForm(10, 5));
    }

    [Fact]
    public void NegativeBranchFacts()
    {
        Assert.True(ObjMonGasFamilyCore.NegativeGivesDc2PlusOne());
        Assert.True(ObjMonGasFamilyCore.NegativeResultIsDc2PlusOne());
        Assert.True(ObjMonGasFamilyCore.NegativeIsNotDc1());
        Assert.True(ObjMonGasFamilyCore.GuardOnlySkipsRoll());
    }

    [Fact]
    public void MaximumComparisonFacts()
    {
        Assert.True(ObjMonGasFamilyCore.SameMaximumWhenNormal());
        Assert.True(ObjMonGasFamilyCore.DivergeWhenInverted());

        // **正常时两者最大都是 dc2 = 20**
        Assert.Equal(20, ObjMonGasFamilyCore.InlineRoll(10, 20, 10));
        Assert.Equal(20, ObjMonGasFamilyCore.GetAttackPowerForm(10, 20));
    }

    [Fact]
    public void DiceOriginalOps()
    {
        Assert.Equal(new[] { "HiWord", "LoWord" }, ObjMonGasFamilyCore.OriginalBitOps);
    }

    // ---------- 命中判据 ----------

    [Fact]
    public void AccuracyGateFacts()
    {
        Assert.True(ObjMonGasFamilyCore.AccuracyVsAgilityGate());
        Assert.True(ObjMonGasFamilyCore.BothDeclaredWord());
        Assert.True(ObjMonGasFamilyCore.BothCommentedByte());
        Assert.True(ObjMonGasFamilyCore.PairOfTypeMismatches());
        Assert.True(ObjMonGasFamilyCore.HigherAgilityHarderToHit());
    }

    [Fact]
    public void HitBoundaries()
    {
        Assert.True(ObjMonGasFamilyCore.LowRollHits());
        Assert.True(ObjMonGasFamilyCore.HighRollMisses());
        Assert.True(ObjMonGasFamilyCore.ZeroAgilityAlwaysHits());

        Assert.True(ObjMonGasFamilyCore.Hits(20, 5, 0));
        Assert.True(ObjMonGasFamilyCore.Hits(20, 5, 4));
        Assert.False(ObjMonGasFamilyCore.Hits(20, 5, 5));
    }

    // ---------- GetPoseCreate / 双效果 / 延迟 ----------

    [Fact]
    public void PoseCreateFacts()
    {
        Assert.True(ObjMonGasFamilyCore.UsesGetPoseCreate());
        Assert.True(ObjMonGasFamilyCore.ThreeOverloads());
        Assert.True(ObjMonGasFamilyCore.ReturnsWhoWasHit());
    }

    [Fact]
    public void TwoEffectFacts()
    {
        Assert.True(ObjMonGasFamilyCore.TwoEffectsSameSlot());
        Assert.True(ObjMonGasFamilyCore.StoneDurationFive());
        Assert.True(ObjMonGasFamilyCore.ParalysisUsesConfiguredTime());
        Assert.True(ObjMonGasFamilyCore.SecondWriteWins());
        Assert.True(ObjMonGasFamilyCore.CompleteParalysisForm());
        Assert.True(ObjMonGasFamilyCore.ContrastWithJ235J236());
        Assert.True(ObjMonGasFamilyCore.CanStoneArgIsTwenty());
        Assert.True(ObjMonGasFamilyCore.CanStoneDeclChecked());
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        Assert.True(ObjMonGasFamilyCore.SwitchOnFires());
        Assert.True(ObjMonGasFamilyCore.FluteCanFire());
        Assert.True(ObjMonGasFamilyCore.ResistedBlocks());
        Assert.True(ObjMonGasFamilyCore.ImmuneBlocks());

        Assert.True(ObjMonGasFamilyCore.ParalysisFires(true, true, 0, 0, 0));
        Assert.True(ObjMonGasFamilyCore.ParalysisFires(true, false, 100, 0, 0));
        Assert.False(ObjMonGasFamilyCore.ParalysisFires(true, true, 0, 0, 1));
        Assert.False(ObjMonGasFamilyCore.ParalysisFires(false, true, 0, 0, 0));
    }

    [Fact]
    public void DelayFacts()
    {
        Assert.True(ObjMonGasFamilyCore.DelayThreeHundred());
        Assert.True(ObjMonGasFamilyCore.BothSendsUseIt());
        Assert.True(ObjMonGasFamilyCore.FourthDelayValueSeen());
        Assert.True(ObjMonGasFamilyCore.DelayTableExtracted());
        Assert.True(ObjMonGasFamilyCore.ThreeHundredInTable());

        Assert.Equal(new[] { 200, 300, 500, 2000 }, ObjMonGasFamilyCore.DelayValues);
    }

    // ===================== 三、AttackTarget 的砍门版 =====================

    [Fact]
    public void TemplateConfirmationFacts()
    {
        Assert.True(ObjMonGasFamilyCore.EleventhTemplateConfirmation());
        Assert.True(ObjMonGasFamilyCore.NoRangeGate());
        Assert.True(ObjMonGasFamilyCore.NoProbabilityGate());
        Assert.True(ObjMonGasFamilyCore.BothGatesCut());
        Assert.True(ObjMonGasFamilyCore.SecondTrimmedVariant());
        Assert.True(ObjMonGasFamilyCore.ElseWithoutBeginEnd());
        Assert.True(ObjMonGasFamilyCore.SameAsOthersSemantically());
        Assert.True(ObjMonGasFamilyCore.CallIsUnconditional());
    }

    [Fact]
    public void AttackBoundaries()
    {
        Assert.True(ObjMonGasFamilyCore.AllOkAttacks());
        Assert.True(ObjMonGasFamilyCore.NoTargetNoAttack());
        Assert.True(ObjMonGasFamilyCore.BadDirNoAttack());
        Assert.True(ObjMonGasFamilyCore.CooldownBlocks());

        Assert.True(ObjMonGasFamilyCore.Attacks(true, true, true));
        Assert.False(ObjMonGasFamilyCore.Attacks(false, true, true));
        Assert.False(ObjMonGasFamilyCore.Attacks(true, false, true));
        Assert.False(ObjMonGasFamilyCore.Attacks(true, true, false));
    }

    // ===================== 四、Moth.Run =====================

    [Fact]
    public void HalfThrottleFacts()
    {
        Assert.True(ObjMonGasFamilyCore.WalkTickNeverUpdated());
        Assert.True(ObjMonGasFamilyCore.HalfUpdatedThrottle());
        Assert.True(ObjMonGasFamilyCore.ThrottleStaysOpen());
        Assert.True(ObjMonGasFamilyCore.MirrorImageOfJ238());
    }

    [Fact]
    public void ComparisonOperatorFacts()
    {
        Assert.True(ObjMonGasFamilyCore.GreaterEqualHere());
        Assert.True(ObjMonGasFamilyCore.GreaterElsewhere());
        Assert.True(ObjMonGasFamilyCore.OffByOneTick());
        Assert.True(ObjMonGasFamilyCore.EqualOnlyPassesGreaterEqual());
        Assert.True(ObjMonGasFamilyCore.StaysPassingAfterPass());

        Assert.False(ObjMonGasFamilyCore.PassesWithGreater(5, 5));
        Assert.True(ObjMonGasFamilyCore.PassesWithGreaterEqual(5, 5));
        Assert.True(ObjMonGasFamilyCore.PassesWithGreater(6, 5));
    }

    [Fact]
    public void MothRunOtherFacts()
    {
        Assert.True(ObjMonGasFamilyCore.AddressNamedField());
        Assert.True(ObjMonGasFamilyCore.ContinuesTheFamily());
        Assert.True(ObjMonGasFamilyCore.UsesSub4C959C());
        Assert.True(ObjMonGasFamilyCore.InsteadOfSearchTarget());
        Assert.True(ObjMonGasFamilyCore.SubSearchDeclChecked());
        Assert.True(ObjMonGasFamilyCore.InheritedUnconditional());
        Assert.True(ObjMonGasFamilyCore.TwoTierThrottle());
    }

    [Fact]
    public void SearchThrottleBoundaries()
    {
        Assert.True(ObjMonGasFamilyCore.SearchAfterEight());
        Assert.True(ObjMonGasFamilyCore.SearchAfterOne());
        Assert.True(ObjMonGasFamilyCore.ExactlyOneBlocks());

        Assert.True(ObjMonGasFamilyCore.ShouldSearch(8001, true));
        Assert.False(ObjMonGasFamilyCore.ShouldSearch(8000, true));
        Assert.True(ObjMonGasFamilyCore.ShouldSearch(1001, false));
        Assert.False(ObjMonGasFamilyCore.ShouldSearch(1000, false));
    }

    // ===================== 五、Create / Destroy 与子类差异 =====================

    [Fact]
    public void CreateFacts()
    {
        Assert.True(ObjMonGasFamilyCore.TwoFieldsOnly());
        Assert.True(ObjMonGasFamilyCore.SearchTimeRandom1500To3000());
        Assert.True(ObjMonGasFamilyCore.IsAnAnimalOppositeOfJ231());
        Assert.True(ObjMonGasFamilyCore.NoCommentHere());
        Assert.True(ObjMonGasFamilyCore.SearchTimeRange());

        Assert.Equal(1500, ObjMonGasFamilyCore.SearchTime(0));
        Assert.Equal(2999, ObjMonGasFamilyCore.SearchTime(1499));
    }

    [Fact]
    public void SubclassDifferenceFacts()
    {
        Assert.True(ObjMonGasFamilyCore.DungOnlyDiffersByViewRange());
        Assert.True(ObjMonGasFamilyCore.CreateIdenticalToMoth());
        Assert.True(ObjMonGasFamilyCore.PureShellDestroy());
        Assert.True(ObjMonGasFamilyCore.NoRunNoAttackOverride());
        Assert.True(ObjMonGasFamilyCore.SubclassExistsForOneField());
        Assert.True(ObjMonGasFamilyCore.BothSubclassesSetSeven());
        Assert.True(ObjMonGasFamilyCore.BaseDoesNotSetIt());
    }

    [Fact]
    public void DestroyFacts()
    {
        Assert.True(ObjMonGasFamilyCore.ThreePureShellDestroys());
        Assert.True(ObjMonGasFamilyCore.TwentyFourTotal());
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonGasFamilyCore.GasFamilyClosed());
        Assert.True(ObjMonGasFamilyCore.ThreeRowsToFlip());
        Assert.True(ObjMonGasFamilyCore.DeclLinesChecked());
        Assert.True(ObjMonGasFamilyCore.BaseChainCorrect());

        Assert.Equal(new[] { 327, 444, 452 }, ObjMonGasFamilyCore.DeclLines);
    }
}

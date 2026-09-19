using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J192：`TCopyMon.Struck`（33 行，含内嵌函数 `CanSetTarget`）与
/// `TCopyMon.Operate`（28 行）1:1 测试。
/// **核心发现是"同一份代码在两个类里注释状态相反"**：
/// 父类 `THumMon.Struck` 的 `CanSetTarget` 后半段被 `{ }` 吞掉（150-160），
/// 子类同一段是活的 —— 后果是"父类永不换目标、子类会换"。
/// </summary>
public sealed class CopyMonStruckOperateCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(33, CopyMonStruckOperateCore.StruckLines);
        Assert.Equal(1727, CopyMonStruckOperateCore.StruckStart);
        Assert.Equal(1759, CopyMonStruckOperateCore.StruckEnd);
        Assert.Equal(28, CopyMonStruckOperateCore.OperateLines);
        Assert.Equal(1761, CopyMonStruckOperateCore.OperateStart);
        Assert.Equal(1788, CopyMonStruckOperateCore.OperateEnd);
        Assert.Equal(61, CopyMonStruckOperateCore.TotalLines);
        Assert.Equal(15, CopyMonStruckOperateCore.CanSetTargetLines);
        Assert.Equal(1729, CopyMonStruckOperateCore.CanSetTargetStart);
        Assert.Equal(1743, CopyMonStruckOperateCore.CanSetTargetEnd);
        Assert.Equal(20048, CopyMonStruckOperateCore.RM_STRUCK);
        Assert.Equal(1, CopyMonStruckOperateCore.UnderFireIndex);
        Assert.Equal(4, CopyMonStruckOperateCore.TMonStatusCount);
        Assert.Equal(300, CopyMonStruckOperateCore.MeatRandomBound);
        Assert.Equal(150, CopyMonStruckOperateCore.HitBase);
        Assert.Equal(130, CopyMonStruckOperateCore.HitAccelCap);
        Assert.Equal(4, CopyMonStruckOperateCore.HitAccelPerLevel);
        Assert.Equal(20, CopyMonStruckOperateCore.HitFloor);
        Assert.Equal(150, CopyMonStruckOperateCore.HitCeiling);
        Assert.Equal(33, CopyMonStruckOperateCore.HitBreakpointLevel);
        Assert.Equal(150, CopyMonStruckOperateCore.ParentCommentOpen);
        Assert.Equal(160, CopyMonStruckOperateCore.ParentCommentClose);
        Assert.Equal(4, CopyMonStruckOperateCore.Param3CastCount);
        Assert.Equal(6, CopyMonStruckOperateCore.StruckTickSites);
        Assert.Equal(6, CopyMonStruckOperateCore.MeatQualitySites);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(CopyMonStruckOperateCore.SpanMatches());
        Assert.True(CopyMonStruckOperateCore.CanSetTargetIsNested());
    }

    // ===================== 一、注释状态相反 =====================

    [Fact]
    public void CommentStateFacts()
    {
        Assert.True(CopyMonStruckOperateCore.ParentBlockCommented());
        Assert.True(CopyMonStruckOperateCore.ChildBlockLive());
        Assert.True(CopyMonStruckOperateCore.OppositeCommentState());
        Assert.True(CopyMonStruckOperateCore.CommentDelimitersAt150And160());
        Assert.True(CopyMonStruckOperateCore.CommentBlockElevenLines());
        Assert.True(CopyMonStruckOperateCore.CommentInsideParentStruck());
    }

    [Fact]
    public void SemanticsDiffer()
    {
        Assert.True(CopyMonStruckOperateCore.ParentReturnsOnlyProperTarget());
        Assert.True(CopyMonStruckOperateCore.ChildAlsoComparesDistance());
        Assert.True(CopyMonStruckOperateCore.SemanticsDiffer());
        Assert.True(CopyMonStruckOperateCore.ChildSwitchesParentDoesNot());
        Assert.True(CopyMonStruckOperateCore.ChildDegeneratesWhenNull());
    }

    [Fact]
    public void ParentOnlyChecksProperTarget()
    {
        // **父类：与距离完全无关**
        Assert.True(CopyMonStruckOperateCore.ParentCanSetTarget(true));
        Assert.False(CopyMonStruckOperateCore.ParentCanSetTarget(false));

        // **子类：合法目标 + 当前目标更远 → 换**
        Assert.True(CopyMonStruckOperateCore.ChildCanSetTarget(true, false, 10, 5, false, 0, 0));
        // **子类：合法目标 + 当前目标更近 → 不换（父类仍会换）**
        Assert.False(CopyMonStruckOperateCore.ChildCanSetTarget(true, false, 5, 10, false, 0, 0));
    }

    // ===================== 二、距离比较 =====================

    [Fact]
    public void DistanceComparison()
    {
        Assert.True(CopyMonStruckOperateCore.ManhattanDistance());
        Assert.True(CopyMonStruckOperateCore.StrictlyGreater());
        Assert.True(CopyMonStruckOperateCore.EqualDoesNotSwitch());
        Assert.True(CopyMonStruckOperateCore.ManhattanValues());
        Assert.True(CopyMonStruckOperateCore.StrictBoundary());
    }

    [Fact]
    public void ManhattanIsNotChebyshev()
    {
        // **曼哈顿：对角 (3,4) 得 7（切比雪夫会得 4）**
        Assert.Equal(7, CopyMonStruckOperateCore.Manhattan(0, 0, 3, 4));
        Assert.Equal(3, CopyMonStruckOperateCore.Manhattan(0, 0, 3, 0));
        Assert.Equal(0, CopyMonStruckOperateCore.Manhattan(5, 5, 5, 5));

        // **相等不换、远一格才换**
        Assert.False(CopyMonStruckOperateCore.ChildCanSetTarget(true, false, 5, 5, false, 0, 0));
        Assert.True(CopyMonStruckOperateCore.ChildCanSetTarget(true, false, 6, 5, false, 0, 0));
    }

    [Fact]
    public void TwoPerspectives()
    {
        Assert.True(CopyMonStruckOperateCore.TwoPerspectives());
        Assert.True(CopyMonStruckOperateCore.EitherTriggers());
        Assert.True(CopyMonStruckOperateCore.MasterBranchRequiresMaster());
        Assert.True(CopyMonStruckOperateCore.MasterAloneCanTrigger());
        Assert.True(CopyMonStruckOperateCore.NoMasterIgnoresMasterSide());
    }

    // ===================== 三、冗余判空一族 =====================

    [Fact]
    public void RedundantNilChecks()
    {
        Assert.True(CopyMonStruckOperateCore.RedundantNilRecheck());
        Assert.True(CopyMonStruckOperateCore.AlwaysTrueHere());
        Assert.True(CopyMonStruckOperateCore.SecondInstanceOfDeadGuard());
        Assert.True(CopyMonStruckOperateCore.MasterRelaxDisjunct());
        Assert.True(CopyMonStruckOperateCore.SecondConjunctRedundant());
        Assert.True(CopyMonStruckOperateCore.ThirdRedundantNilCheck());
        Assert.True(CopyMonStruckOperateCore.RedundantNilLinesExtracted());

        Assert.Equal(new[] { 1732, 1736, 1749 }, CopyMonStruckOperateCore.RedundantNilLines);
    }

    [Fact]
    public void ParenStyleIsEquivalent()
    {
        Assert.True(CopyMonStruckOperateCore.ParenStyleDiffers());
        Assert.True(CopyMonStruckOperateCore.EvaluatesIdentically());
        Assert.True(CopyMonStruckOperateCore.PrecedenceMakesThemEqual());

        // **八组合逐一比对一致**
        for (int h = 0; h <= 1; h++)
        {
            for (int r = 0; r <= 1; r++)
            {
                Assert.Equal(
                    CopyMonStruckOperateCore.ParentSlaveRelax(h == 1, r == 1),
                    CopyMonStruckOperateCore.ChildSlaveRelax(h == 1, r == 1));
            }
        }

        // **语义：无主人 → 真；有主人且松弛 → 假；有主人且不松弛 → 真**
        Assert.True(CopyMonStruckOperateCore.ChildSlaveRelax(false, false));
        Assert.False(CopyMonStruckOperateCore.ChildSlaveRelax(true, true));
        Assert.True(CopyMonStruckOperateCore.ChildSlaveRelax(true, false));
    }

    // ===================== 四、肉量衰减 =====================

    [Fact]
    public void MeatDecay()
    {
        Assert.True(CopyMonStruckOperateCore.MeatBlockIdentical());
        Assert.True(CopyMonStruckOperateCore.MeatRandomRange());
        Assert.True(CopyMonStruckOperateCore.ClampedToZero());
        Assert.True(CopyMonStruckOperateCore.RandomExclusiveUpper());
        Assert.True(CopyMonStruckOperateCore.MeatQualitySitesCount());
        Assert.True(CopyMonStruckOperateCore.DecayMeatValues());
        Assert.True(CopyMonStruckOperateCore.RandomBoundBoundary());

        // **钳到零**
        Assert.Equal(0, CopyMonStruckOperateCore.DecayMeat(100, 299));
        Assert.Equal(201, CopyMonStruckOperateCore.DecayMeat(500, 299));
    }

    [Fact]
    public void MeatLines()
    {
        Assert.Equal(new[] { 177, 178, 179, 1754, 1755, 1756 },
            CopyMonStruckOperateCore.MeatQualityLines);
    }

    // ===================== 五、命中间隔公式 =====================

    [Fact]
    public void HitTickFormulaFacts()
    {
        Assert.True(CopyMonStruckOperateCore.HitTickFormula());
        Assert.True(CopyMonStruckOperateCore.FloorIs20());
        Assert.True(CopyMonStruckOperateCore.CeilingIs150());
        Assert.True(CopyMonStruckOperateCore.BreakpointAtLevel33());
        Assert.True(CopyMonStruckOperateCore.FormulaAppearsTwice());
        Assert.True(CopyMonStruckOperateCore.BothIdentical());
        Assert.True(CopyMonStruckOperateCore.FormulaLinesExtracted());
        Assert.True(CopyMonStruckOperateCore.CastIsDefensive());
        Assert.True(CopyMonStruckOperateCore.CannotGoNegativeAsWritten());
    }

    [Fact]
    public void HitTickValues()
    {
        Assert.True(CopyMonStruckOperateCore.HitTickValues());

        // **等级零最大（150）**
        Assert.Equal(150, CopyMonStruckOperateCore.HitTickIncrement(0));
        // **每级减四**
        Assert.Equal(146, CopyMonStruckOperateCore.HitTickIncrement(1));
        Assert.Equal(110, CopyMonStruckOperateCore.HitTickIncrement(10));
        // **拐点前**
        Assert.Equal(22, CopyMonStruckOperateCore.HitTickIncrement(32));
        // **拐点处到下限 20**
        Assert.Equal(20, CopyMonStruckOperateCore.HitTickIncrement(33));
        Assert.Equal(20, CopyMonStruckOperateCore.HitTickIncrement(100));
    }

    [Fact]
    public void HitTickProperties()
    {
        Assert.True(CopyMonStruckOperateCore.LevelZeroIsCeiling());
        Assert.True(CopyMonStruckOperateCore.NeverBelowFloor());
        Assert.True(CopyMonStruckOperateCore.NeverAboveCeiling());
        Assert.True(CopyMonStruckOperateCore.MonotonicNonIncreasing());
        Assert.True(CopyMonStruckOperateCore.LinearBeforeBreakpoint());
        Assert.True(CopyMonStruckOperateCore.FlatAfterBreakpoint());
    }

    [Fact]
    public void FormulaLines()
    {
        Assert.Equal(new[] { 181, 1758 }, CopyMonStruckOperateCore.HitTickFormulaLines);
    }

    // ===================== 六、Operate =====================

    [Fact]
    public void OperateIntercept()
    {
        Assert.True(CopyMonStruckOperateCore.InterceptsOneMessage());
        Assert.True(CopyMonStruckOperateCore.MessageId20048());
        Assert.True(CopyMonStruckOperateCore.NarrowIntercept());
        Assert.True(CopyMonStruckOperateCore.CommentedResultFalse());
        Assert.True(CopyMonStruckOperateCore.BothBranchesAssign());
        Assert.True(CopyMonStruckOperateCore.CommentedResultVerbatim());

        Assert.Equal("// Result:=False;", CopyMonStruckOperateCore.CommentedResultText);
    }

    [Fact]
    public void InnerGuard()
    {
        Assert.True(CopyMonStruckOperateCore.TwoConjuncts());
        Assert.True(CopyMonStruckOperateCore.SelfIdentityCheck());
        Assert.True(CopyMonStruckOperateCore.AttackerMustBeNonNull());
        Assert.True(CopyMonStruckOperateCore.InnerGuardValues());

        Assert.True(CopyMonStruckOperateCore.InnerGuard(true, true));
        Assert.False(CopyMonStruckOperateCore.InnerGuard(false, true));
        Assert.False(CopyMonStruckOperateCore.InnerGuard(true, false));
    }

    [Fact]
    public void ResultTrueIsOutsideInnerBlock()
    {
        Assert.True(CopyMonStruckOperateCore.TrueOutsideInnerBlock());
        Assert.True(CopyMonStruckOperateCore.ConsumedEvenWhenInnerFails());
        Assert.True(CopyMonStruckOperateCore.EasyToMisplace());
        Assert.True(CopyMonStruckOperateCore.ConsumeSemanticsValues());

        // **内层失败仍返回真（消息被消费）**
        Assert.True(CopyMonStruckOperateCore.OperateResult(true, false));
        Assert.True(CopyMonStruckOperateCore.OperateResult(true, true));
        // **非 RM_STRUCK 走继承**
        Assert.False(CopyMonStruckOperateCore.OperateResult(false, false));
        Assert.True(CopyMonStruckOperateCore.OperateResult(false, true));
    }

    [Fact]
    public void FiveSteps()
    {
        Assert.True(CopyMonStruckOperateCore.FiveSteps());
        Assert.True(CopyMonStruckOperateCore.OrderIsSignificant());
        Assert.True(CopyMonStruckOperateCore.StruckStepsExtracted());

        Assert.Equal(5, CopyMonStruckOperateCore.StruckSteps.Length);
        Assert.Equal("SetLastHiter", CopyMonStruckOperateCore.StruckSteps[0]);
        Assert.Equal("Struck", CopyMonStruckOperateCore.StruckSteps[1]);
        Assert.Equal("BreakHolySeizeMode", CopyMonStruckOperateCore.StruckSteps[2]);
        Assert.Equal("SetPKFlag", CopyMonStruckOperateCore.StruckSteps[3]);
        Assert.Equal("MonsterSayMsg", CopyMonStruckOperateCore.StruckSteps[4]);
    }

    [Fact]
    public void PkFlag()
    {
        Assert.True(CopyMonStruckOperateCore.PkFlagThreeConjuncts());
        Assert.True(CopyMonStruckOperateCore.AttackerNotMaster());
        Assert.True(CopyMonStruckOperateCore.AttackerMustBePlayer());
        Assert.True(CopyMonStruckOperateCore.ShouldSetPkFlagValues());
        Assert.True(CopyMonStruckOperateCore.MasterHittingOwnSlaveNoPk());

        // **三条件全满足**
        Assert.True(CopyMonStruckOperateCore.ShouldSetPkFlag(true, false, 0));
        // **攻击者是主人 → 不加**
        Assert.False(CopyMonStruckOperateCore.ShouldSetPkFlag(true, true, 0));
        // **非玩家 → 不加**
        Assert.False(CopyMonStruckOperateCore.ShouldSetPkFlag(true, false, 80));
        // **无主人 → 不加**
        Assert.False(CopyMonStruckOperateCore.ShouldSetPkFlag(false, false, 0));
    }

    [Fact]
    public void SayMessage()
    {
        Assert.True(CopyMonStruckOperateCore.SayGatedByConfig());
        Assert.True(CopyMonStruckOperateCore.StateIsUnderFire());
        Assert.True(CopyMonStruckOperateCore.EnumOrderIndex1());
        Assert.True(CopyMonStruckOperateCore.EnumHasFourMembers());
        Assert.True(CopyMonStruckOperateCore.EnumOrderExtracted());
    }

    [Fact]
    public void RepeatedCastsAndComments()
    {
        Assert.True(CopyMonStruckOperateCore.RepeatedCasts());
        Assert.True(CopyMonStruckOperateCore.LocalVarUnderused());
        Assert.True(CopyMonStruckOperateCore.FourCastsTotal());
        Assert.True(CopyMonStruckOperateCore.InlineMeaningComments());
        Assert.True(CopyMonStruckOperateCore.OffsetMarker0FFEC());
        Assert.True(CopyMonStruckOperateCore.InlineCommentsVerbatim());

        Assert.Equal("{ AttackBaseObject }", CopyMonStruckOperateCore.AttackBaseObjectComment);
        Assert.Equal("{ 0FFEC }", CopyMonStruckOperateCore.OffsetMarkerComment);
    }

    // ===================== 七、m_dwStruckTick =====================

    [Fact]
    public void StruckTickWindows()
    {
        Assert.True(CopyMonStruckOperateCore.StruckTickSitesCount());
        Assert.True(CopyMonStruckOperateCore.MultipleTimeouts());
        Assert.True(CopyMonStruckOperateCore.SameFieldDifferentWindows());
        Assert.True(CopyMonStruckOperateCore.StruckTickLinesExtracted());
        Assert.True(CopyMonStruckOperateCore.ThreeDistinctWindows());
        Assert.True(CopyMonStruckOperateCore.WindowsDiffer());
        Assert.True(CopyMonStruckOperateCore.StruckRefreshesOwnTick());

        Assert.Equal(new[] { 164, 1033, 1746, 1988, 2010, 2021 },
            CopyMonStruckOperateCore.StruckTickLines);
        Assert.Equal(new[] { 1000, 5000, 30000 }, CopyMonStruckOperateCore.StruckTickWindows);
    }

    // ===================== 八、分工 =====================

    [Fact]
    public void DivisionOfLabour()
    {
        Assert.True(CopyMonStruckOperateCore.OperateIsEntry());
        Assert.True(CopyMonStruckOperateCore.StruckIsBody());
        Assert.True(CopyMonStruckOperateCore.StruckSelfContained());
        Assert.True(CopyMonStruckOperateCore.NoInstrumentation());
    }
}

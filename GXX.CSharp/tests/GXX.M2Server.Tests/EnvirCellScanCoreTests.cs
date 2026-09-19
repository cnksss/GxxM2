using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J169：`TEnvirnoment` 格子扫描族 1:1 测试。
/// **隐藏全局出参 `bo2C` 用写入分布清点固证、
/// 三种演员策略用判别性输入区分、范围遍历用格子数与负半径反例。**
/// </summary>
public sealed class EnvirCellScanCoreTests
{
    // ===================== bo2C 全局 =====================

    [Fact]
    public void Bo2CIsGlobal()
    {
        Assert.True(EnvirCellScanCore.Bo2CIsUnitGlobal());
        Assert.Equal(202, EnvirCellScanCore.Bo2CDeclLine);
        Assert.True(EnvirCellScanCore.HiddenOutParameter());
        Assert.True(EnvirCellScanCore.NotThreadLocal());
        Assert.True(EnvirCellScanCore.ConcurrencyHazard());
    }

    [Fact]
    public void WriteAndReadCounts()
    {
        Assert.Equal(17, EnvirCellScanCore.WriteCount());
        Assert.True(EnvirCellScanCore.SeventeenWrites());
        Assert.Equal(8, EnvirCellScanCore.ReadCount());
        Assert.True(EnvirCellScanCore.EightReads());
    }

    [Fact]
    public void WriteSites()
    {
        // **GetItem 五、Ex 四、Ex2 四、Ex3 三、GetEvent 一**
        Assert.True(EnvirCellScanCore.FiveWriteSites());
        Assert.True(EnvirCellScanCore.WritesAddUp());
        Assert.True(EnvirCellScanCore.ExWriteCountsDiffer());
        Assert.True(EnvirCellScanCore.Ex3HasOneFewerWrite());
        Assert.Equal(new[] { 5, 4, 4, 3, 1 }, new[]
        {
            EnvirCellScanCore.WriteSites[0].Count, EnvirCellScanCore.WriteSites[1].Count,
            EnvirCellScanCore.WriteSites[2].Count, EnvirCellScanCore.WriteSites[3].Count,
            EnvirCellScanCore.WriteSites[4].Count,
        });
    }

    [Fact]
    public void LastWriterWins()
    {
        // **一个线程的结果会被另一个覆盖**
        Assert.True(EnvirCellScanCore.LastWriterWins());
        Assert.Equal(1, EnvirCellScanCore.ConcurrentWrites(1));
        Assert.Equal(0, EnvirCellScanCore.ConcurrentWrites(2));
    }

    [Fact]
    public void ReadsConcentrated()
    {
        Assert.True(EnvirCellScanCore.ReadsConcentratedInDropPosition());
        Assert.True(EnvirCellScanCore.OneReadRange());
        Assert.Single(EnvirCellScanCore.ReadRange);
        Assert.Equal((1449, 1581), EnvirCellScanCore.ReadRange[0]);
    }

    [Fact]
    public void Semantics()
    {
        Assert.True(EnvirCellScanCore.MeaningIsPlaceable());
        Assert.True(EnvirCellScanCore.GateBlocks());
        Assert.True(EnvirCellScanCore.LivingActorBlocks());
    }

    [Fact]
    public void ObjGameConstants()
    {
        Assert.Equal(4, EnvirCellScanCore.ObjGate);
        Assert.Equal(2, EnvirCellScanCore.ObjItem);
        Assert.Equal(1, EnvirCellScanCore.ObjActor);
        Assert.Equal(0, EnvirCellScanCore.RcPlayObject);
    }

    // ===================== 一、三种演员策略 =====================

    [Fact]
    public void ThreePolicies()
    {
        Assert.True(EnvirCellScanCore.ThreeActorPolicies());
        Assert.True(EnvirCellScanCore.Ex1AnyActor());
        Assert.True(EnvirCellScanCore.Ex2PlayerOnly());
        Assert.True(EnvirCellScanCore.Ex3IgnoresActors());
    }

    [Fact]
    public void PoliciesDiffer()
    {
        // **活着的非玩家只有策略一挡、活着的玩家策略一二挡、死了都不挡**
        Assert.True(EnvirCellScanCore.ThreePoliciesDiffer());
    }

    [Fact]
    public void PolicyValues()
    {
        // 活着、非玩家
        Assert.True(EnvirCellScanCore.Ex1ActorBlocks(1, false));
        Assert.False(EnvirCellScanCore.Ex2ActorBlocks(1, 10, false));
        Assert.False(EnvirCellScanCore.Ex3ActorBlocks(1, false));

        // 活着、玩家
        Assert.True(EnvirCellScanCore.Ex2ActorBlocks(1, 0, false));

        // 死了
        Assert.False(EnvirCellScanCore.Ex1ActorBlocks(1, true));
        Assert.False(EnvirCellScanCore.Ex2ActorBlocks(1, 0, true));

        // 非演员
        Assert.False(EnvirCellScanCore.Ex1ActorBlocks(2, false));
    }

    [Fact]
    public void CommentedCondition()
    {
        Assert.True(EnvirCellScanCore.CommentedBroaderCondition());
        Assert.True(EnvirCellScanCore.NarrowedFromThreeRaces());
        Assert.True(EnvirCellScanCore.ThreeOriginalRaces());
        Assert.True(EnvirCellScanCore.NarrowedToOne());
        Assert.True(EnvirCellScanCore.CommentHasThreeRaces());
        Assert.Equal(new[] { 0, 1, 150 }, EnvirCellScanCore.OriginalThreeRaces);
    }

    // ===================== 二、chFlag 门 =====================

    [Fact]
    public void GateTable()
    {
        Assert.True(EnvirCellScanCore.NineFunctionsInTable());
        Assert.Equal(9, EnvirCellScanCore.GateTable.Length);
        Assert.True(EnvirCellScanCore.ThreeHaveGate());
        Assert.True(EnvirCellScanCore.SixHaveNoGate());
        Assert.True(EnvirCellScanCore.GatedCountIsThree());
        Assert.True(EnvirCellScanCore.UngatedCountIsSix());
        Assert.Equal(3, EnvirCellScanCore.GatedCount());
    }

    [Fact]
    public void GatedOnes()
    {
        // **只有三个 GetItemEx 有门**
        Assert.True(EnvirCellScanCore.GateTable[0].HasGate);
        Assert.True(EnvirCellScanCore.GateTable[1].HasGate);
        Assert.True(EnvirCellScanCore.GateTable[2].HasGate);

        for (int i = 3; i < 9; i++)
        {
            Assert.False(EnvirCellScanCore.GateTable[i].HasGate);
        }
    }

    [Fact]
    public void BlockedCells()
    {
        Assert.True(EnvirCellScanCore.ScansBlockedCells());
        Assert.True(EnvirCellScanCore.CellPlaceable(0, true));
        Assert.False(EnvirCellScanCore.CellPlaceable(1, true));
        Assert.True(EnvirCellScanCore.BlockedCellNotPlaceable());
    }

    // ===================== 三、范围遍历 =====================

    [Fact]
    public void SquareGeometry()
    {
        Assert.True(EnvirCellScanCore.ThreeIdenticalRangeLoops());
        Assert.True(EnvirCellScanCore.SquareSideValues());
        Assert.True(EnvirCellScanCore.ZeroRangeIsOneCell());
        Assert.Equal(1, EnvirCellScanCore.SquareSide(0));
        Assert.Equal(3, EnvirCellScanCore.SquareSide(1));
        Assert.Equal(7, EnvirCellScanCore.SquareSide(3));
    }

    [Fact]
    public void CellCounts()
    {
        Assert.True(EnvirCellScanCore.CellCountValues());
        Assert.Equal(1, EnvirCellScanCore.CellCount(0));
        Assert.Equal(9, EnvirCellScanCore.CellCount(1));
        Assert.Equal(25, EnvirCellScanCore.CellCount(2));
    }

    [Fact]
    public void NegativeRange()
    {
        // **Delphi for 起点大于终点时零次 —— 负半径返回零**
        Assert.True(EnvirCellScanCore.NegativeRangeYieldsZero());
        Assert.True(EnvirCellScanCore.NegativeRangeEmpty());
        Assert.True(EnvirCellScanCore.NegativeRangeReverses());
        Assert.True(EnvirCellScanCore.PositiveRangeRuns());
        Assert.Empty(EnvirCellScanCore.DelphiFor(5, 3));
        Assert.Equal(7, EnvirCellScanCore.DelphiFor(7, 13).Count);
    }

    [Fact]
    public void Locking()
    {
        Assert.True(EnvirCellScanCore.LockPerCell());
        Assert.True(EnvirCellScanCore.NoOuterLock());
        Assert.True(EnvirCellScanCore.TwentyFiveLockCycles());
        Assert.Equal(25, EnvirCellScanCore.LockCycles(2));
        Assert.Equal(9, EnvirCellScanCore.LockCycles(1));
    }

    [Fact]
    public void TornView()
    {
        // **逐格加锁但无外层锁 → 不能保证一致性快照**
        Assert.True(EnvirCellScanCore.NoSnapshotConsistency());
        Assert.True(EnvirCellScanCore.TornView());
        Assert.True(EnvirCellScanCore.TornReadPossible());
        Assert.True(EnvirCellScanCore.TornRead(25));
    }

    [Fact]
    public void CumulativeCount()
    {
        Assert.True(EnvirCellScanCore.ReturnsListCount());
        Assert.True(EnvirCellScanCore.ReturnsCumulativeCount());
        Assert.True(EnvirCellScanCore.ReuseAccumulates());
        Assert.Equal(5, EnvirCellScanCore.Cumulative(3, 2));
    }

    // ===================== 四、GetItemEx 细节 =====================

    [Fact]
    public void LastItemWins()
    {
        // **返回最后一个物品、而计数是累加的**
        Assert.True(EnvirCellScanCore.ResultIsLastItem());
        Assert.True(EnvirCellScanCore.CountIsCumulative());
        Assert.True(EnvirCellScanCore.ReturnsOneButCountsMany());
        Assert.True(EnvirCellScanCore.LastItemValues());
        Assert.True(EnvirCellScanCore.OneVersusMany());
        Assert.Equal(9, EnvirCellScanCore.LastItem(new List<int> { 7, 8, 9 }));
        Assert.Null(EnvirCellScanCore.LastItem(new List<int>()));
    }

    [Fact]
    public void Indistinguishable()
    {
        // **无效格子与"有效但被挡"都是假 —— 调用者分不开**
        Assert.True(EnvirCellScanCore.IndistinguishableStates());
        Assert.True(EnvirCellScanCore.BothGiveFalse());
        Assert.True(EnvirCellScanCore.CollapseToFalse());
        Assert.True(EnvirCellScanCore.CannotDistinguish());

        Assert.False(EnvirCellScanCore.Bo2C(false, false));
        Assert.False(EnvirCellScanCore.Bo2C(true, true));
        Assert.True(EnvirCellScanCore.Bo2C(true, false));
    }

    [Fact]
    public void ComparisonCounts()
    {
        // **Ex1/Ex2 每对象比三次、Ex3 只比两次**
        Assert.True(EnvirCellScanCore.ThreeComparisonsPerObject());
        Assert.Equal(3, EnvirCellScanCore.ComparisonCount());
        Assert.True(EnvirCellScanCore.ThreeComparisons());
        Assert.True(EnvirCellScanCore.NoElseIfChain());
        Assert.True(EnvirCellScanCore.Ex3HasTwoComparisons());
        Assert.Equal(2, EnvirCellScanCore.Ex3ComparisonCount());
        Assert.True(EnvirCellScanCore.Ex3TwoComparisons());
    }

    [Fact]
    public void GhostFiltering()
    {
        // **GetItemObjects 过滤幽灵、GetItemEx 不过滤**
        Assert.True(EnvirCellScanCore.GetItemObjectsFiltersGhost());
        Assert.True(EnvirCellScanCore.GetItemExDoesNotFilter());
        Assert.True(EnvirCellScanCore.SameCellDifferentCounts());
        Assert.True(EnvirCellScanCore.SameWhenNotGhost());
    }

    [Fact]
    public void GhostFilterValues()
    {
        // 幽灵物品：不过滤得 1、过滤得 0
        Assert.Single(EnvirCellScanCore.Collect(2, true, false));
        Assert.Empty(EnvirCellScanCore.Collect(2, true, true));

        // 非幽灵：两者一致
        Assert.Single(EnvirCellScanCore.Collect(2, false, false));
        Assert.Single(EnvirCellScanCore.Collect(2, false, true));

        // 非物品：都不收
        Assert.Empty(EnvirCellScanCore.Collect(1, false, false));
    }

    // ===================== 五、范围两兄弟 =====================

    [Fact]
    public void IdenticalButCallee()
    {
        Assert.True(EnvirCellScanCore.RangeBaseAndPlayIdenticalButCall());
        Assert.True(EnvirCellScanCore.TwoCallees());
        Assert.Equal(new[] { "GetBaseObjects", "GetPlayObjects" }, EnvirCellScanCore.RangeCallees);
    }

    [Fact]
    public void InvertedParameterName()
    {
        // **参数名说"包含"、真值表示"不包含"**
        Assert.True(EnvirCellScanCore.IncDeathObjectInverted());
        Assert.True(EnvirCellScanCore.NameContradictsSemantics());
        Assert.True(EnvirCellScanCore.ParamNameSaysInclude());
        Assert.Equal("IncDeathObject", EnvirCellScanCore.ParamName);
        Assert.True(EnvirCellScanCore.TrueMeansExclude());
    }

    [Fact]
    public void IncludeSemantics()
    {
        Assert.True(EnvirCellScanCore.IncludeModelValues());
        Assert.True(EnvirCellScanCore.ThreeCommentLines());
        Assert.Equal(3, EnvirCellScanCore.SemanticsComment.Length);

        // 真 → 排除死亡者
        Assert.False(EnvirCellScanCore.Include(true, true));
        Assert.True(EnvirCellScanCore.Include(true, false));

        // 假 → 包括全部
        Assert.True(EnvirCellScanCore.Include(false, true));
        Assert.True(EnvirCellScanCore.Include(false, false));
    }

    [Fact]
    public void NamePropagates()
    {
        Assert.True(EnvirCellScanCore.InvertedNamePropagates());
        Assert.True(EnvirCellScanCore.TwoChainSteps());
        Assert.Equal(2, EnvirCellScanCore.PropagationChain.Length);
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirCellScanCore.SevenMethods());
        Assert.Equal(7, EnvirCellScanCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 52, 52, 43, 33, 12, 12, 15 }, EnvirCellScanCore.MethodLineCounts);
    }

    [Fact]
    public void EqualLengths()
    {
        Assert.True(EnvirCellScanCore.Ex1Ex2SameLength());
        Assert.True(EnvirCellScanCore.TwoRangesSameLength());
        Assert.Equal(EnvirCellScanCore.MethodLineCounts[0], EnvirCellScanCore.MethodLineCounts[1]);
        Assert.Equal(EnvirCellScanCore.MethodLineCounts[4], EnvirCellScanCore.MethodLineCounts[5]);
    }

    [Fact]
    public void Ex3IsShorter()
    {
        Assert.True(EnvirCellScanCore.Ex3Shorter());
        Assert.Equal(9, EnvirCellScanCore.MethodLineCounts[0] - EnvirCellScanCore.MethodLineCounts[2]);
    }

    [Fact]
    public void Totals()
    {
        Assert.True(EnvirCellScanCore.TotalLinesValues());
        Assert.Equal(219, EnvirCellScanCore.TotalLines());
        Assert.Equal(180, EnvirCellScanCore.ScannerLines());
        Assert.True(EnvirCellScanCore.ScannerLinesIs180());
        Assert.True(EnvirCellScanCore.ScannerShareIs82());
    }
}

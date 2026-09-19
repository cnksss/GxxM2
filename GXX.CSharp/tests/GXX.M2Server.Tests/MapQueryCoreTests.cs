using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J132：地图取对象原语 —— 三个 `GetMovingObject` 重载、`GetRangeBaseObject`、
/// `GetBaseObjects`、`GetPlayObjects`、`GetXYHuman`、`sub_4B5FC8`
/// （Envir.pas 4643-4768、5150-5252、5327-5371）1:1 测试。
/// </summary>
public sealed class MapQueryCoreTests
{
    // ===================== 常量与锁索引 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MapQueryCore.ConstantsMatchSource());
        Assert.Equal(33, MapQueryCore.LockMovingList);
        Assert.Equal(34, MapQueryCore.LockMovingOne);
        Assert.Equal(36, MapQueryCore.LockMovingMatch);
        Assert.Equal(44, MapQueryCore.LockBaseObjects);
        Assert.Equal(45, MapQueryCore.LockPlayObjects);
        Assert.Equal(47, MapQueryCore.LockXYHuman);
    }

    [Fact]
    public void ThreeOverloadsDifferentLocks()
    {
        Assert.True(MapQueryCore.ThreeOverloadsDifferentLocks());
    }

    [Fact]
    public void LockIndex35Skipped()
    {
        // **33、34、36 —— 35 被跳过**
        Assert.True(MapQueryCore.LockIndex35Skipped());
        Assert.DoesNotContain(35, MapQueryCore.AllLockIndices);
    }

    [Fact]
    public void TwoSiblingDifferentLocks()
    {
        Assert.True(MapQueryCore.TwoSiblingDifferentLocks());
    }

    [Fact]
    public void ElevenDistinctLockIndices()
    {
        Assert.True(MapQueryCore.ElevenDistinctLockIndices());
        Assert.Equal(12, MapQueryCore.AllLockIndices.Length);
    }

    [Fact]
    public void NewLocksAreDistinct()
    {
        Assert.True(MapQueryCore.NewLocksAreDistinct());
        Assert.Equal(new[] { 4, 16, 17, 19, 28, 46 }, MapQueryCore.OtherLockIndices);
    }

    [Fact]
    public void LockIndex47()
    {
        Assert.True(MapQueryCore.LockIndex47());
    }

    // ===================== 一、死亡过滤与参数名 =====================

    [Fact]
    public void TrueExcludesDeath()
    {
        // **IncDeathObject = True 排除死亡对象**
        Assert.True(MapQueryCore.TrueExcludesDeath());
        Assert.False(MapQueryCore.PassesDeathFilter(true, true));
        Assert.True(MapQueryCore.PassesDeathFilter(true, false));
    }

    [Fact]
    public void FalseIncludesDeath()
    {
        // **False 包含死亡对象**
        Assert.True(MapQueryCore.FalseIncludesDeath());
        Assert.True(MapQueryCore.PassesDeathFilter(false, true));
        Assert.True(MapQueryCore.PassesDeathFilter(false, false));
    }

    [Fact]
    public void CommentAndNameAgreeButNameIsStale()
    {
        Assert.True(MapQueryCore.CommentAndNameAgree());
        Assert.True(MapQueryCore.ParamNameIsStaleBoFlag());
        Assert.True(MapQueryCore.CommentAndNameAgreeButNameIsStale());
    }

    [Fact]
    public void IncDeathCommentContent()
    {
        Assert.Equal(3, MapQueryCore.IncDeathComment.Length);
        Assert.Contains("是否包括死亡对象", MapQueryCore.IncDeathComment[0]);
        Assert.Contains("FALSE 包括", MapQueryCore.IncDeathComment[1]);
        Assert.Contains("TRUE  不包括", MapQueryCore.IncDeathComment[2]);
    }

    [Fact]
    public void AllCallersPassTrue()
    {
        Assert.True(MapQueryCore.AllCallersPassTrue());
    }

    [Fact]
    public void ThreeFunctionsShareSemantics()
    {
        Assert.True(MapQueryCore.ThreeFunctionsShareSemantics());
    }

    [Fact]
    public void FiveFunctionsShareFilter()
    {
        Assert.True(MapQueryCore.DeathFilterIdenticalAcrossFiveFunctions());
        Assert.True(MapQueryCore.FiveFunctionsShareFilter());
        Assert.Equal(5, MapQueryCore.FunctionsWithDeathFilter.Length);
    }

    [Fact]
    public void DeathFilterTruthTable()
    {
        // 两参数四组合
        Assert.False(MapQueryCore.PassesDeathFilter(true, true));
        Assert.True(MapQueryCore.PassesDeathFilter(true, false));
        Assert.True(MapQueryCore.PassesDeathFilter(false, true));
        Assert.True(MapQueryCore.PassesDeathFilter(false, false));
    }

    // ===================== 二、三个 GetMovingObject 重载 =====================

    [Fact]
    public void OverloadACollectsAll()
    {
        Assert.True(MapQueryCore.OverloadACollectsAll());
        Assert.True(MapQueryCore.OverloadAReturnsCount());
        Assert.True(MapQueryCore.OnlyOverloadACollectsAll());
    }

    [Fact]
    public void OverloadANilListStillCounts()
    {
        // **A 容忍 nil 列表：仍会正确计数**
        Assert.True(MapQueryCore.OverloadAToleratesNullList());
        Assert.True(MapQueryCore.OverloadANilListStillCounts());
    }

    [Fact]
    public void OverloadBTakesFirst()
    {
        Assert.True(MapQueryCore.OverloadBReturnsFirst());
        Assert.True(MapQueryCore.OverloadBTakesFirst());
        Assert.True(MapQueryCore.OverloadBNoMatch());
    }

    [Fact]
    public void OverloadCHitsTarget()
    {
        Assert.True(MapQueryCore.OverloadCReturnsMatch());
        Assert.True(MapQueryCore.OverloadCHitsTarget());
        Assert.True(MapQueryCore.OverloadCHasIdentityCheck());
    }

    [Fact]
    public void OverloadCMissOnBadTarget()
    {
        Assert.True(MapQueryCore.OverloadCMissOnBadTarget());
    }

    [Fact]
    public void OverloadCDiffersFromB()
    {
        var cell = new[] { MapQueryCore.Good(1), MapQueryCore.Good(2) };

        Assert.Equal(1, MapQueryCore.OverloadB(cell, false));
        Assert.Equal(2, MapQueryCore.OverloadC(cell, false, 2));
    }

    [Fact]
    public void ThreeOverloadsAgreeOnDeathFilter()
    {
        Assert.True(MapQueryCore.ThreeOverloadsAgreeOnDeathFilter());
    }

    [Fact]
    public void NullCheckAfterCastIsRedundant()
    {
        Assert.True(MapQueryCore.NullCheckAfterCastIsRedundant());
    }

    [Fact]
    public void PassesCommonFilterTruthTable()
    {
        Assert.True(MapQueryCore.PassesCommonFilter(true, false, true, false, false));
        Assert.False(MapQueryCore.PassesCommonFilter(false, false, true, false, false));
        Assert.False(MapQueryCore.PassesCommonFilter(true, true, true, false, false));
        Assert.False(MapQueryCore.PassesCommonFilter(true, false, false, false, false));
        Assert.False(MapQueryCore.PassesCommonFilter(true, false, true, true, true));
    }

    // ===================== 三、bo2B9 =====================

    [Fact]
    public void Bo2B9DefaultsTrue()
    {
        Assert.True(MapQueryCore.Bo2B9DefaultsTrue());
        Assert.True(MapQueryCore.DefaultBo2B9());
    }

    [Fact]
    public void OnlyCastleDoorChangesIt()
    {
        Assert.True(MapQueryCore.OnlyCastleDoorChangesIt());
    }

    [Fact]
    public void DoorOpenCloseFlags()
    {
        Assert.True(MapQueryCore.DoorOpenSetsFalse());
        Assert.True(MapQueryCore.DoorCloseSetsTrue());
        Assert.False(MapQueryCore.CastleDoorFlag(true));
        Assert.True(MapQueryCore.CastleDoorFlag(false));
    }

    [Fact]
    public void OpenDoorExcludedFromQueries()
    {
        // **开着的门被排除在取对象查询之外**
        Assert.True(MapQueryCore.OpenDoorExcludedFromQueries());
        Assert.True(MapQueryCore.ClosedDoorIncluded());
    }

    [Fact]
    public void Bo2B9IsQueryParticipationFlag()
    {
        Assert.True(MapQueryCore.Bo2B9IsQueryParticipationFlag());
    }

    [Fact]
    public void OffsetCommentPresent()
    {
        Assert.True(MapQueryCore.OffsetCommentPresent());
        Assert.Equal("// 0x2B9", MapQueryCore.Bo2B9OffsetComment);
    }

    [Fact]
    public void DeadDoorSetsBlockedFlag()
    {
        Assert.True(MapQueryCore.DeadDoorSetsBlockedFlag());
        Assert.Equal(2, MapQueryCore.DeadDoorFlag());
        Assert.True(MapQueryCore.EchoesMineOnBlockedCells());
    }

    // ===================== 四、GetRangeBaseObject =====================

    [Fact]
    public void RangeIsSquareNotCircle()
    {
        Assert.True(MapQueryCore.RangeIsSquareNotCircle());
    }

    [Fact]
    public void RangeCellCountValues()
    {
        Assert.True(MapQueryCore.RangeCellCountValues());
        Assert.Equal(1, MapQueryCore.RangeCellCount(0));
        Assert.Equal(9, MapQueryCore.RangeCellCount(1));
        Assert.Equal(25, MapQueryCore.RangeCellCount(2));
        Assert.Equal(49, MapQueryCore.RangeCellCount(3));
    }

    [Fact]
    public void RangeSideIsOdd()
    {
        Assert.Equal(1, MapQueryCore.RangeSide(0));
        Assert.Equal(3, MapQueryCore.RangeSide(1));
        Assert.Equal(5, MapQueryCore.RangeSide(2));
    }

    [Fact]
    public void ZeroRangeScansOneCell()
    {
        Assert.True(MapQueryCore.ZeroRangeScansOneCell());
    }

    [Fact]
    public void OneRangeScansNineCells()
    {
        Assert.True(MapQueryCore.OneRangeScansNineCells());
        Assert.True(MapQueryCore.OneRangeCoversCenterAndNeighbours());
    }

    [Fact]
    public void RangeCellCoordinates()
    {
        var cells = MapQueryCore.RangeCells(5, 5, 1);

        Assert.Equal(9, cells.Count);
        Assert.Contains((5, 5), cells);
        Assert.Contains((4, 5), cells);
        Assert.Contains((6, 6), cells);
    }

    [Fact]
    public void NegativeRangeYieldsZero()
    {
        // **负范围不执行循环 → 返回 0**
        Assert.True(MapQueryCore.NegativeRangeYieldsZero());
        Assert.Empty(MapQueryCore.RangeCells(5, 5, -2));
    }

    [Fact]
    public void DelegatesBoundsToInner()
    {
        Assert.True(MapQueryCore.DelegatesBoundsToInner());
        Assert.True(MapQueryCore.CornerRangeClipsToFour());
    }

    [Fact]
    public void ScanWithBoundsClipping()
    {
        Assert.Equal(4, MapQueryCore.ScanWithBounds(0, 0, 1, 10, 10));
        Assert.Equal(9, MapQueryCore.ScanWithBounds(5, 5, 1, 10, 10));
        Assert.Equal(1, MapQueryCore.ScanWithBounds(0, 0, 0, 10, 10));
    }

    [Fact]
    public void ReturnsCumulativeCountNotDelta()
    {
        // **返回列表总元素数而非本次加入数**
        Assert.True(MapQueryCore.ReturnsCumulativeCountNotDelta());
        Assert.True(MapQueryCore.CumulativeIncludesPreexisting());
        Assert.True(MapQueryCore.BothRangeFunctionsCumulative());
    }

    [Fact]
    public void CumulativeCountValues()
    {
        Assert.Equal(5, MapQueryCore.CumulativeCount(3, 2));
        Assert.Equal(2, MapQueryCore.CumulativeCount(0, 2));
    }

    [Fact]
    public void RangePlayMirrorsRangeBase()
    {
        Assert.True(MapQueryCore.RangePlayMirrorsRangeBase());
        Assert.True(MapQueryCore.OnlyDifferenceIsInnerCall());
        Assert.True(MapQueryCore.RangeFunctionsHaveNoLock());
    }

    // ===================== 五、GetBaseObjects / GetPlayObjects =====================

    [Fact]
    public void PlayObjectsAddsRaceFilter()
    {
        Assert.True(MapQueryCore.PlayObjectsAddsRaceFilter());
    }

    [Fact]
    public void MonsterInBaseNotInPlay()
    {
        Assert.True(MapQueryCore.MonsterInBaseNotInPlay());
    }

    [Fact]
    public void PlayerInBoth()
    {
        Assert.True(MapQueryCore.PlayerInBoth());
    }

    [Fact]
    public void SiblingsShareDeathFilter()
    {
        Assert.True(MapQueryCore.SiblingsShareDeathFilter());
    }

    [Fact]
    public void BothCollectAll()
    {
        Assert.True(MapQueryCore.BothCollectAll());
    }

    [Fact]
    public void BaseFilterTruthTable()
    {
        Assert.True(MapQueryCore.PassesBaseFilter(1, false, true, false, false));
        Assert.False(MapQueryCore.PassesBaseFilter(2, false, true, false, false));
        Assert.False(MapQueryCore.PassesBaseFilter(1, true, true, false, false));
        Assert.False(MapQueryCore.PassesBaseFilter(1, false, false, false, false));
        Assert.False(MapQueryCore.PassesBaseFilter(1, false, true, true, true));
    }

    [Fact]
    public void PlayFilterAddsRace()
    {
        Assert.True(MapQueryCore.PassesPlayFilter(1, false, true, false, false, 0));
        Assert.False(MapQueryCore.PassesPlayFilter(1, false, true, false, false, 80));
    }

    // ===================== 六、GetXYHuman =====================

    [Fact]
    public void GetXYHumanIgnoresGhost()
    {
        // **完全忽略 m_boGhost**
        Assert.True(MapQueryCore.GetXYHumanIgnoresGhost());
    }

    [Fact]
    public void GetXYHumanIgnoresBo2B9()
    {
        Assert.True(MapQueryCore.GetXYHumanIgnoresBo2B9());
    }

    [Fact]
    public void DivergesFromOtherFour()
    {
        // **幽灵玩家算"有人"、但被 GetMovingObject 跳过**
        Assert.True(MapQueryCore.DivergesFromOtherFour());
        Assert.True(MapQueryCore.GhostPlayerCountsAsPresent());
        Assert.True(MapQueryCore.GhostPlayerSkippedByMoving());
    }

    [Fact]
    public void XYHumanSemantics()
    {
        Assert.True(MapQueryCore.XYHumanGhostTrue());
        Assert.True(MapQueryCore.XYHumanMonsterFalse());
        Assert.True(MapQueryCore.XYHumanNonActorFalse());
    }

    [Fact]
    public void XYHumanFilterValues()
    {
        Assert.True(MapQueryCore.XYHumanFilter(1, 0));
        Assert.False(MapQueryCore.XYHumanFilter(1, 80));
        Assert.False(MapQueryCore.XYHumanFilter(2, 0));
    }

    [Fact]
    public void OpenDoorStillCountsAsHuman()
    {
        // 开门状态不影响 GetXYHuman（它不看 bo2B9）
        Assert.True(MapQueryCore.OpenDoorStillCountsAsHuman());
    }

    [Fact]
    public void ReturnsBoolWithBreak()
    {
        Assert.True(MapQueryCore.ReturnsBoolWithBreak());
    }

    // ===================== 七、sub_4B5FC8 =====================

    [Fact]
    public void Sub4B5FC8Semantics()
    {
        Assert.True(MapQueryCore.Sub4B5FC8Semantics());
    }

    [Fact]
    public void OnlyFlagTwoBlocks()
    {
        // **只有 chFlag = 2 才为假**
        Assert.True(MapQueryCore.OnlyFlagTwoBlocks());
        Assert.False(MapQueryCore.Sub4B5FC8(true, 2));
        Assert.True(MapQueryCore.Sub4B5FC8(true, 0));
        Assert.True(MapQueryCore.Sub4B5FC8(true, 1));
    }

    [Fact]
    public void OutOfRangeReturnsTrue()
    {
        // **越界返回真（可通行）—— 方向性陷阱**
        Assert.True(MapQueryCore.OutOfRangeReturnsTrue());
        Assert.True(MapQueryCore.OutOfRangeTrap());
    }

    [Fact]
    public void ConsistentWithJ131Passable()
    {
        Assert.True(MapQueryCore.ConsistentWithJ131Passable());
    }

    // ===================== 八、顶层仿真 =====================

    [Fact]
    public void RangeScanCountsAcrossCells()
    {
        // **注意顺序：计数为 4，但首个是 4 而非 1**（x 外层、y 内层）
        Assert.True(MapQueryCore.RangeScanCountsAcrossCells());
    }

    [Fact]
    public void ScanOrderIsXThenY()
    {
        // **(4,4) 最先被访问**
        Assert.True(MapQueryCore.ScanOrderIsXThenY());
        Assert.True(MapQueryCore.RangeCellOrderMatchesSourceLoop());
        Assert.True(MapQueryCore.SameXAdjacentYCellsAreContiguous());
        Assert.True(MapQueryCore.LastScannedIsMaxCorner());
    }

    [Fact]
    public void RangeCellsOrder()
    {
        var cells = MapQueryCore.RangeCells(5, 5, 1);

        Assert.Equal((4, 4), cells[0]);
        Assert.Equal((4, 5), cells[1]);
        Assert.Equal((4, 6), cells[2]);
        Assert.Equal((5, 4), cells[3]);
        Assert.Equal((6, 6), cells[8]);
    }

    [Fact]
    public void ListOrderFollowsScanOrder()
    {
        Assert.True(MapQueryCore.ListOrderFollowsScanOrder());
    }

    [Fact]
    public void RangeScanExcludesOutOfRange()
    {
        Assert.True(MapQueryCore.RangeScanExcludesOutOfRange());
    }

    [Fact]
    public void RangeScanClipsAtMapEdge()
    {
        Assert.True(MapQueryCore.RangeScanClipsAtMapEdge());
    }

    [Fact]
    public void RangeScanHonoursDeathFilter()
    {
        var dead = MapQueryCore.Good(1);
        dead.Death = true;

        var map = new Dictionary<(int, int), List<MapQueryCore.Candidate>>
        {
            [(5, 5)] = new List<MapQueryCore.Candidate> { dead, MapQueryCore.Good(2) },
        };

        var (count, first) = MapQueryCore.RangeScan(5, 5, 1, 10, 10, map, true);

        Assert.Equal(1, count);   // 死亡对象被排除
        Assert.Equal(2, first);
    }
}

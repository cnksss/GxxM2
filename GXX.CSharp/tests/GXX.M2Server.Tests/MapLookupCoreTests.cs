using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J131：地图格子查询基础原语 —— `GetMapCellInfo` / `SetMapXYFlag` / `GetEvent` /
/// `CanAddToMapPosition` / `AddHumBBCount`（Envir.pas 1612-1642、5254-5325）1:1 测试。
/// </summary>
public sealed class MapLookupCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MapLookupCore.ConstantsMatchSource());
        Assert.Equal(0, MapLookupCore.FlagPassable);
        Assert.Equal(2, MapLookupCore.FlagBlocked);
        Assert.Equal(46, MapLookupCore.GetEventLockIndex);
        Assert.Equal(10, MapLookupCore.CanFlyMaxIterations);
    }

    [Fact]
    public void FifthDistinctLockIndex()
    {
        Assert.True(MapLookupCore.FifthDistinctLockIndex());
        Assert.Equal(new[] { 16, 17, 28, 4, 19 }, MapLookupCore.OtherLockIndices);
        Assert.DoesNotContain(46, MapLookupCore.OtherLockIndices);
    }

    [Fact]
    public void LockIndex46SharedWithUsrEngn()
    {
        Assert.True(MapLookupCore.LockIndex46SharedWithUsrEngn());
    }

    // ===================== 一、GetMapCellInfo 边界 =====================

    [Fact]
    public void FourBoundsUseHalfOpenRange()
    {
        Assert.True(MapLookupCore.FourBoundsUseHalfOpenRange());
    }

    [Fact]
    public void InBoundsBoundaries()
    {
        Assert.True(MapLookupCore.InBoundsBoundaries());
    }

    [Fact]
    public void WidthBoundIsExclusive()
    {
        Assert.True(MapLookupCore.WidthBoundIsExclusive());
        Assert.False(MapLookupCore.InBounds(3, 0, 3, 5));
        Assert.True(MapLookupCore.InBounds(2, 0, 3, 5));
    }

    [Fact]
    public void HeightBoundIsExclusive()
    {
        Assert.True(MapLookupCore.HeightBoundIsExclusive());
        Assert.False(MapLookupCore.InBounds(0, 5, 3, 5));
        Assert.True(MapLookupCore.InBounds(0, 4, 3, 5));
    }

    [Fact]
    public void NegativeBoundsRejected()
    {
        Assert.False(MapLookupCore.InBounds(-1, 0, 3, 5));
        Assert.False(MapLookupCore.InBounds(0, -1, 3, 5));
        Assert.False(MapLookupCore.InBounds(-1, -1, 3, 5));
    }

    // ===================== 二、列优先索引（核心） =====================

    [Fact]
    public void IndexIsColumnMajor()
    {
        // **索引是 nX * m_nHeight + nY，列优先**
        Assert.True(MapLookupCore.IndexIsColumnMajor());
        Assert.Equal(7, MapLookupCore.IndexFormula(1, 2, 5));   // 1*5+2
    }

    [Fact]
    public void IndexFormulaValues()
    {
        Assert.Equal(0, MapLookupCore.IndexFormula(0, 0, 5));
        Assert.Equal(1, MapLookupCore.IndexFormula(0, 1, 5));
        Assert.Equal(5, MapLookupCore.IndexFormula(1, 0, 5));
        Assert.Equal(6, MapLookupCore.IndexFormula(1, 1, 5));
    }

    [Fact]
    public void TransposedIndexDiffersOnNonSquareMap()
    {
        // **按行优先理解会得到不同索引**
        Assert.True(MapLookupCore.TransposedIndexDiffersOnNonSquareMap());
        Assert.NotEqual(
            MapLookupCore.IndexFormula(1, 0, 5),
            MapLookupCore.TransposedIndex(1, 0, 3));
    }

    [Fact]
    public void TransposedIndexDiffersOnSquareMap()
    {
        // 正方形地图上沿对角线镜像
        Assert.True(MapLookupCore.TransposedIndexDiffersOnSquareMap());
        Assert.NotEqual(
            MapLookupCore.IndexFormula(1, 0, 3),
            MapLookupCore.TransposedIndex(1, 0, 3));
    }

    [Fact]
    public void IndicesAgreeOnDiagonal()
    {
        Assert.True(MapLookupCore.IndicesAgreeOnDiagonal());
    }

    [Fact]
    public void StridesReflectColumnMajor()
    {
        Assert.True(MapLookupCore.XStrideIsHeight());
        Assert.True(MapLookupCore.YStrideIsOne());
    }

    [Fact]
    public void AllInBoundsIndicesAreValid()
    {
        Assert.True(MapLookupCore.AllInBoundsIndicesAreValid());
    }

    [Fact]
    public void IndexWithinArrayBoundaries()
    {
        Assert.True(MapLookupCore.IndexWithinArray(0, 0, 3, 5));
        Assert.True(MapLookupCore.IndexWithinArray(2, 4, 3, 5));
        Assert.False(MapLookupCore.IndexWithinArray(3, 0, 3, 5));
    }

    // ===================== 三、查找仿真 =====================

    [Fact]
    public void OnlyAssignedOnSuccess()
    {
        Assert.True(MapLookupCore.OnlyAssignedOnSuccess());
        Assert.True(MapLookupCore.BothBranchesAssignResult());
        Assert.True(MapLookupCore.ShortCircuitProtectsCallers());
    }

    [Fact]
    public void LookupSucceedsInRange()
    {
        Assert.True(MapLookupCore.LookupSucceedsInRange());
    }

    [Fact]
    public void LookupFailsOutOfRange()
    {
        Assert.True(MapLookupCore.LookupFailsOutOfRange());
    }

    [Fact]
    public void LookupFailsWhenArrayNull()
    {
        Assert.True(MapLookupCore.LookupFailsWhenArrayNull());
    }

    [Fact]
    public void LookupCornerIndices()
    {
        Assert.Equal((true, 0), MapLookupCore.GetMapCellInfo(0, 0, 3, 5, false));
        Assert.Equal((true, 14), MapLookupCore.GetMapCellInfo(2, 4, 3, 5, false));
    }

    [Fact]
    public void DimensionsAreIntegers()
    {
        Assert.True(MapLookupCore.DimensionsAreIntegers());
    }

    // ===================== 四、chFlag 的两个取值 =====================

    [Fact]
    public void FlagValuesAreZeroAndTwo()
    {
        Assert.True(MapLookupCore.FlagValuesAreZeroAndTwo());
    }

    [Fact]
    public void FlagDirectionIsInvertedVsBool()
    {
        // **boFlag = True → chFlag = 0（反直觉）**
        Assert.True(MapLookupCore.FlagDirectionIsInvertedVsBool());
        Assert.True(MapLookupCore.FlagMapping());
    }

    [Fact]
    public void FlagForBoolValues()
    {
        Assert.Equal(0, MapLookupCore.FlagForBool(true));
        Assert.Equal(2, MapLookupCore.FlagForBool(false));
    }

    [Fact]
    public void ZeroMeansPassable()
    {
        Assert.True(MapLookupCore.ZeroMeansPassable());
        Assert.True(MapLookupCore.TwoMeansBlocked());
    }

    [Fact]
    public void ExplainsJ130GateInversion()
    {
        // **解开 J130 的门控反转之谜**
        Assert.True(MapLookupCore.ExplainsJ130GateInversion());
        Assert.True(MapLookupCore.MineOnlyOnBlockedCells());
        Assert.True(MapLookupCore.ObjectsOnlyOnPassableCells());
        Assert.True(MapLookupCore.GatesComplementaryOnBothFlags());
    }

    [Fact]
    public void GateRequirementsOnBothFlagValues()
    {
        Assert.True(MapLookupCore.AddRequiresZero(0));
        Assert.False(MapLookupCore.AddRequiresZero(2));
        Assert.False(MapLookupCore.MineRequiresNonZero(0));
        Assert.True(MapLookupCore.MineRequiresNonZero(2));
    }

    // ===================== 五、SetMapXYFlag =====================

    [Fact]
    public void SetFlagSilentOnOutOfRange()
    {
        Assert.True(MapLookupCore.SetFlagSilentOnOutOfRange());
        Assert.True(MapLookupCore.SetFlagOutOfRangeNoChange());
    }

    [Fact]
    public void SetFlagSemantics()
    {
        Assert.True(MapLookupCore.SetFlagInRangeChanges());
        Assert.Equal((true, 2), MapLookupCore.SetFlag(true, 0, false));
        Assert.Equal((true, 0), MapLookupCore.SetFlag(true, 2, true));
    }

    [Fact]
    public void DoorOpenCloseFlags()
    {
        Assert.True(MapLookupCore.OpenDoorSetsZero());
        Assert.True(MapLookupCore.CloseDoorSetsTwo());
    }

    [Fact]
    public void CastleDoorCells()
    {
        Assert.True(MapLookupCore.CastleDoorHasSixCells());
        Assert.Equal((0, -2), MapLookupCore.CastleDoorCells[0]);
    }

    [Fact]
    public void CastleDoorFlagSequence()
    {
        Assert.True(MapLookupCore.CastleDoorFlagSequence());
        Assert.Equal((0, 2), MapLookupCore.CastleDoorFlags());
    }

    [Fact]
    public void DoorStateDeterminesWhatCanBePlaced()
    {
        Assert.True(MapLookupCore.ClosedDoorAcceptsMineRejectsObjects());
        Assert.True(MapLookupCore.OpenDoorAcceptsObjectsRejectsMine());
    }

    // ===================== 六、CanAddToMapPosition =====================

    [Fact]
    public void CanAddToMapPositionMirrorsAddToMap()
    {
        Assert.True(MapLookupCore.CanAddToMapPositionMirrorsAddToMap());
        Assert.True(MapLookupCore.BothFormsAgree());
    }

    [Fact]
    public void CanAddHasExtraInvalidCheck()
    {
        Assert.True(MapLookupCore.CanAddHasExtraInvalidCheck());
    }

    [Fact]
    public void CanAddToMapPositionTruthTable()
    {
        Assert.True(MapLookupCore.CanAddToMapPosition(false, true, 0));
        Assert.False(MapLookupCore.CanAddToMapPosition(false, true, 2));
        Assert.False(MapLookupCore.CanAddToMapPosition(false, false, 0));
        Assert.False(MapLookupCore.CanAddToMapPosition(true, true, 0));
    }

    // ===================== 七、GetEvent =====================

    [Fact]
    public void ResultStartsNilAndBo2CIsDead()
    {
        Assert.True(MapLookupCore.ResultStartsNil());
        Assert.True(MapLookupCore.Bo2CIsDeadAssignment());
    }

    [Fact]
    public void GateTruthTable()
    {
        Assert.True(MapLookupCore.GateTruthTable());
        Assert.True(MapLookupCore.RequiresValidCellAndNonNullList(true, false));
        Assert.False(MapLookupCore.RequiresValidCellAndNonNullList(true, true));
        Assert.False(MapLookupCore.RequiresValidCellAndNonNullList(false, false));
    }

    [Fact]
    public void OnlyObjEventCounts()
    {
        Assert.True(MapLookupCore.OnlyObjEventCounts(3));
        Assert.True(MapLookupCore.OtherTypesRejected());
    }

    [Fact]
    public void NoBreakLastEventWins()
    {
        Assert.True(MapLookupCore.NoBreakLastEventWins());
        Assert.True(MapLookupCore.LastEventWinsVerified());
    }

    [Fact]
    public void FindEventSemantics()
    {
        Assert.Equal(1, MapLookupCore.FindEvent(new[] { (true, 3), (true, 3) }));
        Assert.Equal(2, MapLookupCore.FindEvent(new[] { (true, 3), (true, 2), (true, 3), (true, 1) }));
        Assert.Equal(-1, MapLookupCore.FindEvent(new[] { (true, 2), (true, 4) }));
        Assert.Equal(-1, MapLookupCore.FindEvent(Array.Empty<(bool, int)>()));
    }

    [Fact]
    public void LastEventWinsWithInterleaving()
    {
        Assert.True(MapLookupCore.LastEventWinsWithInterleaving());
    }

    [Fact]
    public void NoEventReturnsMinusOne()
    {
        Assert.True(MapLookupCore.NoEventReturnsMinusOne());
    }

    [Fact]
    public void NullObjectsSkipped()
    {
        Assert.True(MapLookupCore.NullObjectsSkipped());
    }

    [Fact]
    public void SameAsMiningCopies()
    {
        // 三处搜索循环都不 Break
        Assert.True(MapLookupCore.SameAsMiningCopies());
        Assert.True(MapLookupCore.ThreeSearchLoopsAllLackBreak());
        Assert.Equal(new[] { 12316, 18563 }, MapLookupCore.MiningSearchLoopLines);
    }

    [Fact]
    public void ReturnsTObjectNotTGameEvent()
    {
        Assert.True(MapLookupCore.ReturnsTObjectNotTGameEvent());
    }

    // ===================== 八、GetEvent 仿真 =====================

    [Fact]
    public void QueryOutOfRangeReturnsMinusOne()
    {
        Assert.True(MapLookupCore.QueryOutOfRangeReturnsMinusOne());
    }

    [Fact]
    public void QueryNullListReturnsMinusOne()
    {
        Assert.True(MapLookupCore.QueryNullListReturnsMinusOne());
    }

    [Fact]
    public void QueryFindsLastEvent()
    {
        Assert.True(MapLookupCore.QueryFindsLastEvent());
    }

    [Fact]
    public void QueryNoEventInCell()
    {
        int idx = MapLookupCore.QueryEvent(0, 0, 3, 5, false, new[] { (true, 2), (true, 1) });

        Assert.Equal(-1, idx);
    }

    // ===================== 九、AddHumBBCount =====================

    [Fact]
    public void PatchCompensatesOrderDependency()
    {
        Assert.True(MapLookupCore.PatchCompensatesOrderDependency());
        Assert.True(MapLookupCore.ExplainsJ130PetClassification());
    }

    [Fact]
    public void CommentExplainsOrder()
    {
        Assert.True(MapLookupCore.CommentHasDateAndAuthor());
        Assert.True(MapLookupCore.CommentExplainsOrder());
        Assert.Contains("先AddToMap", MapLookupCore.AddHumBBCountComment);
    }

    [Fact]
    public void MovesOneFromMonToBB()
    {
        Assert.True(MapLookupCore.MovesOneFromMonToBB());
        Assert.Equal((1, 4), MapLookupCore.AddHumBBCount(0, 5));
    }

    [Fact]
    public void DecGuardedHere()
    {
        // **Dec 带下界保护**
        Assert.True(MapLookupCore.DecGuardedHere());
        Assert.True(MapLookupCore.MonZeroNoDecrement());
        Assert.Equal((1, 0), MapLookupCore.AddHumBBCount(0, 0));
    }

    [Fact]
    public void UnconditionalIncrement()
    {
        Assert.True(MapLookupCore.UnconditionalIncrement());
        Assert.True(MapLookupCore.CanDriftCountsIfMisused());
    }

    [Fact]
    public void DecIsGuardedIncIsNot()
    {
        // 与 J130 加入侧无保护 Inc 的对照
        Assert.True(MapLookupCore.DecIsGuardedIncIsNot());
    }

    [Fact]
    public void AddHumBBCountSequence()
    {
        var (bb, mon) = MapLookupCore.AddHumBBCount(3, 7);

        Assert.Equal(4, bb);
        Assert.Equal(6, mon);
    }

    // ===================== 十、CanFly =====================

    [Fact]
    public void CanFlyTyposFixed()
    {
        Assert.True(MapLookupCore.CanFlyTyposFixed());
        Assert.True(MapLookupCore.TypoUsesDXInsteadOfDY());
    }

    [Fact]
    public void CanFlyTypoLineContent()
    {
        Assert.Contains("nDY - nDX", MapLookupCore.CanFlyTypoLine);
        Assert.Contains("nDY - nSY", MapLookupCore.CanFlyCorrectLine);
        Assert.True(MapLookupCore.FixCommentHasDate());
        Assert.True(MapLookupCore.DivisorIsTen());
    }

    [Fact]
    public void CanFlyLoopIsRedundant()
    {
        // **最多 10 次且每轮同一坐标**
        Assert.True(MapLookupCore.CanFlyLoopIsRedundant());
        Assert.True(MapLookupCore.TenIterationsWhenWalkable());
        Assert.True(MapLookupCore.OneIterationWhenBlocked());
        Assert.True(MapLookupCore.CoordinatesDoNotAdvance());
    }

    [Fact]
    public void CanFlyIterations()
    {
        Assert.Equal(10, MapLookupCore.CanFlyIterations(true));
        Assert.Equal(1, MapLookupCore.CanFlyIterations(false));
    }
}

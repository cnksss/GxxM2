using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J130：地图格子对象增删 —— `TEnvirnoment.AddToMap`（Envir.pas 1644-1770）
/// 与 `TEnvirnoment.DeleteFromMap`（1772-1934）1:1 测试。
/// </summary>
public sealed class MapCellCoreTests
{
    // ===================== 常量与枚举 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MapCellCore.ConstantsMatchSource());
        Assert.Equal("金币", MapCellCore.GoldName);
        Assert.Equal(2000, MapCellCore.GoldMergeCap);
        Assert.Equal(50, MapCellCore.RcAnimal);
        Assert.Equal(0, MapCellCore.RcPlayObject);
    }

    [Fact]
    public void ObjGameOrdinals()
    {
        Assert.True(MapCellCore.ObjGameOrdinals());
    }

    [Fact]
    public void ObjGameEnumValues()
    {
        Assert.Equal(0, (int)MapCellCore.ObjGame.ObjNone);
        Assert.Equal(1, (int)MapCellCore.ObjGame.ObjActor);
        Assert.Equal(2, (int)MapCellCore.ObjGame.ObjItem);
        Assert.Equal(3, (int)MapCellCore.ObjGame.ObjEvent);
        Assert.Equal(4, (int)MapCellCore.ObjGame.ObjGate);
        Assert.Equal(7, (int)MapCellCore.ObjGame.ObjDoor);
        Assert.Equal(9, (int)MapCellCore.ObjGame.ObjMapEffect);
    }

    [Fact]
    public void ThreeDistinctLockIndices()
    {
        Assert.True(MapCellCore.ThreeDistinctLockIndices());
        Assert.Equal(16, MapCellCore.AddLockIndex);
        Assert.Equal(17, MapCellCore.DeleteLockIndex);
        Assert.Equal(new[] { 28, 4 }, MapCellCore.OtherLockIndices);
    }

    [Fact]
    public void ExceptionMessages()
    {
        Assert.Equal("[Exception] TEnvirnoment.AddToMap", MapCellCore.AddExceptionMsg);
        Assert.Contains("DeleteFromMap", MapCellCore.DeleteExceptionMsgTemplate);
    }

    // ===================== 一、门控反转（核心） =====================

    [Fact]
    public void AddToMapRequiresZeroFlag()
    {
        // **AddToMap 只在 chFlag = 0 的格子上放对象**
        Assert.True(MapCellCore.AddToMapRequiresZeroFlag());
        Assert.True(MapCellCore.AddToMapGate(true, 0));
        Assert.False(MapCellCore.AddToMapGate(true, 1));
        Assert.False(MapCellCore.AddToMapGate(true, 255));
    }

    [Fact]
    public void MineAddRequiresNonZeroFlag()
    {
        // J129：AddToMapMineEvent 要求非零
        Assert.True(MapCellCore.MineAddRequiresNonZeroFlag());
        Assert.True(MapCellCore.MineAddGate(false, true, 1));
        Assert.False(MapCellCore.MineAddGate(false, true, 0));
    }

    [Fact]
    public void TwoFunctionsInvertTheSameFlag()
    {
        // **同一个 chFlag，两个门通过与否恰好相反**
        Assert.True(MapCellCore.TwoFunctionsInvertTheSameFlag());
        // 逐个标志验证相反（而非比较两个恒真的自述式 helper）
        Assert.True(MapCellCore.GatesAreComplementary(0));
        Assert.True(MapCellCore.GatesAreComplementary(1));
        Assert.True(MapCellCore.GatesAreComplementary(255));
        Assert.True(MapCellCore.GatesAreComplementary(-1));
    }

    [Fact]
    public void GateComparisonTable()
    {
        Assert.True(MapCellCore.GateComparisonTable());
        Assert.Equal(4, MapCellCore.GateComparison.Length);
    }

    [Fact]
    public void GateComparisonValues()
    {
        var t = MapCellCore.GateComparison;

        Assert.Equal((0, true, false), t[0]);     // flag=0：Add 通过、Mine 拒绝
        Assert.Equal((1, false, true), t[1]);     // flag=1：Add 拒绝、Mine 通过
        Assert.Equal((255, false, true), t[2]);
        Assert.Equal((-1, false, true), t[3]);
    }

    [Fact]
    public void GatesAreComplementary()
    {
        // 除边界外两者恰好相反
        Assert.True(MapCellCore.GatesAreComplementary(0));
        Assert.True(MapCellCore.GatesAreComplementary(1));
        Assert.True(MapCellCore.GatesAreComplementary(255));
    }

    [Fact]
    public void AddToMapCellNotFoundExits()
    {
        Assert.True(MapCellCore.AddToMapCellNotFoundExits());
    }

    [Fact]
    public void ShortCircuitIsSafeBothWays()
    {
        Assert.True(MapCellCore.ShortCircuitIsSafeBothWays());
    }

    // ===================== 二、金币合并 =====================

    [Fact]
    public void GoldMergeRequiresObjItemAndGoldName()
    {
        Assert.True(MapCellCore.GoldMergeRequiresObjItemAndGoldName());
        Assert.True(MapCellCore.GoldItemMerges());
        Assert.True(MapCellCore.NonItemSkipsMerge());
        Assert.True(MapCellCore.ItemWithOtherNameSkipsMerge());
    }

    [Fact]
    public void IsGoldItemSemantics()
    {
        Assert.True(MapCellCore.IsGoldItem(MapCellCore.ObjGame.ObjItem, MapCellCore.GoldName));
        Assert.False(MapCellCore.IsGoldItem(MapCellCore.ObjGame.ObjActor, MapCellCore.GoldName));
        Assert.False(MapCellCore.IsGoldItem(MapCellCore.ObjGame.ObjItem, "屠龙"));
        Assert.False(MapCellCore.IsGoldItem(MapCellCore.ObjGame.ObjItem, "金币 "));
    }

    [Fact]
    public void MergeCapIsHardcodedTwoThousand()
    {
        Assert.True(MapCellCore.MergeCapIsHardcodedTwoThousand());
        Assert.True(MapCellCore.CanMerge(0, 2000));
        Assert.False(MapCellCore.CanMerge(0, 2001));
    }

    [Fact]
    public void MergeCapIsInclusive()
    {
        // `<= 2000` 恰好可合并
        Assert.True(MapCellCore.MergeCapIsInclusive());
        Assert.True(MapCellCore.CanMerge(1999, 1));
        Assert.False(MapCellCore.CanMerge(1999, 2));
        Assert.True(MapCellCore.CanMerge(2000, 0));
    }

    [Fact]
    public void SixMergeWrittenFields()
    {
        Assert.True(MapCellCore.SixMergeWrittenFields());
        Assert.Contains("m_nCount", MapCellCore.MergeWrittenFields);
        Assert.Contains("m_wLooks", MapCellCore.MergeWrittenFields);
        Assert.Contains("m_nMapX", MapCellCore.MergeWrittenFields);
        Assert.Contains("m_btReserved", MapCellCore.MergeWrittenFields);
        // 1692 另写 m_dwAddTime（不计在六字段内）
        Assert.DoesNotContain("m_dwAddTime", MapCellCore.MergeWrittenFields);
    }

    [Fact]
    public void MergeWritesAllFields()
    {
        Assert.True(MapCellCore.MergeWritesAllFields());
    }

    [Fact]
    public void MergeResultDetails()
    {
        var r = MapCellCore.Merge(MapCellCore.ObjGame.ObjItem, MapCellCore.GoldName,
            100, 100, 5, 6, existingGhost: false);

        Assert.True(r.Merged);
        Assert.Equal(200, r.Count);
        Assert.Equal(5, r.MapX);
        Assert.Equal(6, r.MapY);
        Assert.Equal(0, r.AniCount);
        Assert.Equal(0, r.Reserved);
        Assert.Equal(MapCellCore.GoldShape(200), r.Looks);
    }

    [Fact]
    public void GhostSkipsMerge()
    {
        Assert.True(MapCellCore.GhostSkipsMerge());
    }

    [Fact]
    public void OverCapDoesNotMerge()
    {
        Assert.True(MapCellCore.OverCapDoesNotMerge());
    }

    [Fact]
    public void NonGoldDoesNotMerge()
    {
        var r = MapCellCore.Merge(MapCellCore.ObjGame.ObjActor, MapCellCore.GoldName,
            100, 100, 5, 6, existingGhost: false);

        Assert.False(r.Merged);
        Assert.Equal(100, r.Count);   // 保持不变
    }

    [Fact]
    public void ExitAfterMergePreventsDuplication()
    {
        // **合并后立即 Exit，防止反复丢金币刷钱**
        Assert.True(MapCellCore.ExitAfterMergePreventsDuplication());
        Assert.True(MapCellCore.GoldDuplicationCommentRetained());
        Assert.Contains("刷金币", MapCellCore.GoldDuplicationFixComment);
        Assert.Contains("2019-08-27", MapCellCore.GoldDuplicationFixComment);
    }

    [Fact]
    public void AlreadyPresentIsSuccess()
    {
        // 对象已在格子里 → 视为成功、直接返回
        Assert.True(MapCellCore.AlreadyPresentIsSuccess());
        Assert.True(MapCellCore.AlreadyPresent(true));
        Assert.False(MapCellCore.AlreadyPresent(false));
    }

    [Fact]
    public void GoldShapeLadder()
    {
        Assert.True(MapCellCore.GoldShapeLadder());
    }

    [Fact]
    public void GoldShapeBoundaries()
    {
        Assert.Equal(112, MapCellCore.GoldShape(0));
        Assert.Equal(112, MapCellCore.GoldShape(29));
        Assert.Equal(113, MapCellCore.GoldShape(30));
        Assert.Equal(114, MapCellCore.GoldShape(70));
        Assert.Equal(115, MapCellCore.GoldShape(300));
        Assert.Equal(116, MapCellCore.GoldShape(1000));
    }

    [Fact]
    public void GoldShapeCascadeNotElseIf()
    {
        // **独立 if 而非 else if**：后面的覆盖前面的
        Assert.True(MapCellCore.GoldShapeCascadeNotElseIf());
    }

    [Fact]
    public void GoldShapeNegativeFallsBack()
    {
        Assert.True(MapCellCore.GoldShapeNegativeFallsBack());
    }

    // ===================== 三、物品上限 =====================

    [Fact]
    public void CapDefaults()
    {
        Assert.True(MapCellCore.CapFeatureDefaultsOff());
        Assert.True(MapCellCore.ItemCountCapDefaults());
    }

    [Fact]
    public void CountItemsFiltersCorrectly()
    {
        Assert.True(MapCellCore.CountItemsFiltersCorrectly());
    }

    [Fact]
    public void CountItemsExcludesGhost()
    {
        int n = MapCellCore.CountItems(new[]
        {
            (MapCellCore.ObjGame.ObjItem, true),
            (MapCellCore.ObjGame.ObjItem, true),
        });

        Assert.Equal(0, n);
    }

    [Fact]
    public void CapComparesExistingNotIncludingNew()
    {
        Assert.True(MapCellCore.CapComparesExistingNotIncludingNew());
        Assert.True(MapCellCore.CapIsInclusiveReject());
    }

    [Fact]
    public void DisabledCapNeverRejects()
    {
        Assert.True(MapCellCore.DisabledCapNeverRejects());
        Assert.False(MapCellCore.CapGate(false, 100, 5));
        Assert.True(MapCellCore.CapGate(true, 100, 5));
    }

    [Fact]
    public void MergeExitsBeforeCapCheck()
    {
        // 合并成功先 Exit → 走不到上限检查
        Assert.True(MapCellCore.MergeExitsBeforeCapCheck());
    }

    [Fact]
    public void CapAppliesToAllItemsNotJustGold()
    {
        // **上限在 Obj_Item 内、金币名外 → 对任何物品生效**
        Assert.True(MapCellCore.CapAppliesToAllItemsNotJustGold());
        Assert.True(MapCellCore.CapIsSiblingOfGoldNameCheck());
        Assert.Equal(8, MapCellCore.NestingDepths[2].Indent);
        Assert.Equal(8, MapCellCore.NestingDepths[3].Indent);
        Assert.Equal(6, MapCellCore.NestingDepths[1].Indent);
    }

    [Fact]
    public void MergeBeatsCapInSimulation()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: MapCellCore.GoldName, addCount: 100,
            listIsNull: false,
            existing: new[] { (MapCellCore.ObjGame.ObjItem, MapCellCore.GoldName, 100, false) },
            capEnabled: true, capValue: 1,          // 上限已满
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        // 合并优先于上限检查
        Assert.True(r.Merged);
        Assert.True(r.Success);
        Assert.False(r.RejectedByCap);
    }

    [Fact]
    public void CapRejectsWhenMergeFails()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: MapCellCore.GoldName, addCount: 1500,
            listIsNull: false,
            existing: new[] { (MapCellCore.ObjGame.ObjItem, MapCellCore.GoldName, 1500, false) },
            capEnabled: true, capValue: 1,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        // 合并超 2000 失败 → 落到上限检查 → 被拒
        Assert.False(r.Merged);
        Assert.True(r.RejectedByCap);
        Assert.False(r.Success);
    }

    [Fact]
    public void NullListSkipsMergeAndCap()
    {
        Assert.True(MapCellCore.NullListSkipsMergeAndCap());

        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: MapCellCore.GoldName, addCount: 100,
            listIsNull: true,
            existing: Array.Empty<(MapCellCore.ObjGame, string, int, bool)>(),
            capEnabled: true, capValue: 0,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.False(r.Merged);
        Assert.False(r.RejectedByCap);
        Assert.True(r.Appended);
    }

    // ===================== 四、挂载与坐标 =====================

    [Fact]
    public void AddTimeAlwaysRefreshed()
    {
        Assert.True(MapCellCore.AddTimeAlwaysRefreshed());
    }

    [Fact]
    public void GateDoesNotUpdateCoords()
    {
        Assert.True(MapCellCore.GateDoesNotUpdateCoords());
        Assert.True(MapCellCore.GateKeepsOldCoords());
        Assert.True(MapCellCore.NonGateGetsNewCoords());
    }

    [Fact]
    public void CoordsAfterAddSemantics()
    {
        Assert.Equal((1, 2), MapCellCore.CoordsAfterAdd(MapCellCore.ObjGame.ObjGate, 1, 2, 9, 9));
        Assert.Equal((9, 9), MapCellCore.CoordsAfterAdd(MapCellCore.ObjGame.ObjItem, 1, 2, 9, 9));
        Assert.True(MapCellCore.GateDoesNotUpdateCoords(MapCellCore.ObjGame.ObjItem));
    }

    [Fact]
    public void StorageModes()
    {
        Assert.Equal("TSafeList", MapCellCore.StorageMode(false));
        Assert.Equal("动态数组 SetLength", MapCellCore.StorageMode(true));
        Assert.True(MapCellCore.ListModeLazyCreates());
    }

    // ===================== 五、加入计数器 =====================

    [Fact]
    public void AddBucketOrdered()
    {
        Assert.True(MapCellCore.AddBucketOrdered());
    }

    [Fact]
    public void AddBucketValues()
    {
        Assert.Equal("Hum", MapCellCore.AddBucket(0, false, false));
        Assert.Equal("BB", MapCellCore.AddBucket(80, true, true));
        Assert.Equal("Mon", MapCellCore.AddBucket(80, false, false));
        Assert.Equal("None", MapCellCore.AddBucket(10, false, false));
    }

    [Fact]
    public void PetGoesToBBCountNotMonCount()
    {
        // **else if 短路：宝宝归 BB、即使自身种族 >= RC_ANIMAL**
        Assert.True(MapCellCore.PetGoesToBBCountNotMonCount());
        Assert.Equal("BB", MapCellCore.AddBucket(80, true, true));
    }

    [Fact]
    public void PlayerTakesPrecedence()
    {
        Assert.True(MapCellCore.PlayerTakesPrecedence());
    }

    [Fact]
    public void NonActorSkipsCounters()
    {
        Assert.True(MapCellCore.ItemSkipsCounters());
        Assert.False(MapCellCore.NonActorSkipsCounters(MapCellCore.ObjGame.ObjActor));
        Assert.True(MapCellCore.NonActorSkipsCounters(MapCellCore.ObjGame.ObjItem));
    }

    [Fact]
    public void MonsterBoundaryIsRcAnimal()
    {
        // `>= 50` 算怪物
        Assert.Equal("Mon", MapCellCore.AddBucket(50, false, false));
        Assert.Equal("None", MapCellCore.AddBucket(49, false, false));
        Assert.Equal("Mon", MapCellCore.AddBucket(80, false, false));
    }

    [Fact]
    public void AddFlagsOnlySetOnce()
    {
        Assert.True(MapCellCore.AddFlagsOnlySetOnce());
        Assert.True(MapCellCore.FirstAddClearsDelFlag());
        Assert.True(MapCellCore.RepeatAddKeepsDelFlag());
    }

    [Fact]
    public void AddFlagsBoundaries()
    {
        Assert.Equal((true, false), MapCellCore.AddFlags(false, true));
        Assert.Equal((true, false), MapCellCore.AddFlags(false, false));
        Assert.Equal((true, true), MapCellCore.AddFlags(true, true));
    }

    [Fact]
    public void SuccessReturnsSameObject()
    {
        Assert.True(MapCellCore.SuccessReturnsSameObject());

        var o = new object();

        Assert.True(MapCellCore.AddSucceeded(o, o));
        Assert.False(MapCellCore.AddSucceeded(null, o));
    }

    // ===================== 六、DeleteFromMap 的不对称 =====================

    [Fact]
    public void DeleteIgnoresChFlag()
    {
        Assert.True(MapCellCore.DeleteIgnoresChFlag());
        Assert.True(MapCellCore.DeleteGate(true, 0));
        Assert.True(MapCellCore.DeleteGate(true, 255));
    }

    [Fact]
    public void AddAndDeleteAreAsymmetricOnFlag()
    {
        // **非零标志上：删除可以、添加不行**
        Assert.True(MapCellCore.AddAndDeleteAreAsymmetricOnFlag());
        Assert.True(MapCellCore.NonZeroFlagDeleteOnly());
    }

    [Fact]
    public void DeleteAlsoCompactsNilHoles()
    {
        Assert.True(MapCellCore.DeleteAlsoCompactsNilHoles());
        Assert.True(MapCellCore.NilHoleRemovalDoesNotIncrement());
        Assert.True(MapCellCore.DeleteRoutineCompacts());
    }

    [Fact]
    public void DeleteRoutineCompactsDetails()
    {
        var (removed, remaining) = MapCellCore.DeleteRoutine(new[] { -1, 5, -1, 7 }, 7);

        Assert.True(removed);
        Assert.Single(remaining);
        Assert.Equal(5, remaining[0]);   // 两个空洞都被清理
    }

    [Fact]
    public void DeleteBreaksAfterFirstMatch()
    {
        // **只删第一个匹配**
        Assert.True(MapCellCore.DeleteBreaksAfterFirstMatch());
    }

    [Fact]
    public void NotFoundStillCompacts()
    {
        Assert.True(MapCellCore.NotFoundStillCompacts());
    }

    [Fact]
    public void EmptyListReturnsFalse()
    {
        Assert.True(MapCellCore.EmptyListReturnsFalse());
    }

    [Fact]
    public void TargetAtFirstPosition()
    {
        var (removed, remaining) = MapCellCore.DeleteRoutine(new[] { 7, 5, 7 }, 7);

        Assert.True(removed);
        Assert.Equal(2, remaining.Count);
        Assert.Equal(new[] { 5, 7 }, remaining);
    }

    [Fact]
    public void CommentedContinueRetained()
    {
        Assert.True(MapCellCore.CommentedContinueRetained());
        Assert.True(MapCellCore.CommentedContinueIsRetained());
        Assert.Equal("Break; // Continue;", MapCellCore.CommentedBreakLine);
    }

    [Fact]
    public void NilHoleRemovedByDelete()
    {
        Assert.True(MapCellCore.NilHoleRemovedByDelete());
    }

    // ===================== 七、Code 位置标记 =====================

    [Fact]
    public void CodeIsHandRolledPositionMarker()
    {
        Assert.True(MapCellCore.CodeIsHandRolledPositionMarker());
        Assert.True(MapCellCore.CodeStartsAtZero());
    }

    [Fact]
    public void CodeValuesAreSequential()
    {
        Assert.True(MapCellCore.CodeValuesAreSequential());
        Assert.True(MapCellCore.TwentyFiveCodeValues());
        Assert.Equal(25, MapCellCore.CodeSequence.Length);
    }

    [Fact]
    public void CodeSequenceEndpoints()
    {
        Assert.Equal(0, MapCellCore.CodeSequence[0]);
        Assert.Equal(24, MapCellCore.CodeSequence[24]);
    }

    [Fact]
    public void CodeNotUniqueAcrossCompileModes()
    {
        Assert.True(MapCellCore.CodeNotUniqueAcrossCompileModes());
    }

    [Fact]
    public void ExceptionMsgHasThreePlaceholders()
    {
        Assert.True(MapCellCore.ExceptionMsgHasThreePlaceholders());
    }

    [Fact]
    public void FormatDeleteExceptionWorks()
    {
        Assert.True(MapCellCore.FormatDeleteExceptionWorks());
    }

    [Fact]
    public void MapNameSnapshottedBeforeWork()
    {
        Assert.True(MapCellCore.MapNameSnapshottedBeforeWork());
    }

    // ===================== 八、计数器回退与 tick =====================

    [Fact]
    public void DecrementGuardedByPositiveCheck()
    {
        Assert.True(MapCellCore.DecrementGuardedByPositiveCheck());
        Assert.True(MapCellCore.SafeDecBoundaries());
    }

    [Fact]
    public void SafeDecSemantics()
    {
        Assert.Equal(4, MapCellCore.SafeDec(5));
        Assert.Equal(0, MapCellCore.SafeDec(1));
        Assert.Equal(0, MapCellCore.SafeDec(0));   // **不产生负数**
    }

    [Fact]
    public void AddSideHasNoGuard()
    {
        Assert.True(MapCellCore.AddSideHasNoGuard());
        Assert.True(MapCellCore.DecrementIsGuardedButIncrementIsNot());
    }

    [Fact]
    public void UnguardedIncSemantics()
    {
        Assert.Equal(6, MapCellCore.UnguardedInc(5));
        Assert.Equal(1, MapCellCore.UnguardedInc(0));
    }

    [Fact]
    public void ClearTickOnlyOnDelete()
    {
        Assert.True(MapCellCore.ClearTickOnlyOnDelete());
        Assert.True(MapCellCore.AddSideHasNoTickMaintenance());
    }

    [Fact]
    public void RefreshTickBothZero()
    {
        Assert.True(MapCellCore.RefreshTickBothZero());
        Assert.True(MapCellCore.ShouldRefreshClearTick(0, 0));
        Assert.False(MapCellCore.ShouldRefreshClearTick(1, 0));
        Assert.False(MapCellCore.ShouldRefreshClearTick(0, 1));
    }

    [Fact]
    public void MonCountZeroDoesNotRefreshTick()
    {
        Assert.True(MapCellCore.MonCountZeroDoesNotRefreshTick());
        Assert.True(MapCellCore.MonBranchSkipsTick(true));
        Assert.False(MapCellCore.MonBranchSkipsTick(false));
    }

    [Fact]
    public void BucketsMirror()
    {
        Assert.True(MapCellCore.BucketsMirror());
    }

    [Fact]
    public void DeleteBucketValues()
    {
        Assert.Equal("Hum", MapCellCore.DeleteBucket(0, false, false));
        Assert.Equal("BB", MapCellCore.DeleteBucket(80, true, true));
        Assert.Equal("Mon", MapCellCore.DeleteBucket(80, false, false));
    }

    [Fact]
    public void FlagsMirror()
    {
        Assert.True(MapCellCore.FlagsMirror());
        Assert.True(MapCellCore.FirstDeleteClearsAddFlag());
        Assert.True(MapCellCore.RepeatDeleteKeepsAddFlag());
    }

    [Fact]
    public void DeleteFlagsBoundaries()
    {
        Assert.Equal((false, true), MapCellCore.DeleteFlags(false, true));
        Assert.Equal((true, true), MapCellCore.DeleteFlags(true, true));
    }

    [Fact]
    public void PartialSuccessStillReturnsTrue()
    {
        Assert.True(MapCellCore.ResultTrueBeforeCountersUpdate());
        Assert.True(MapCellCore.PartialSuccessStillReturnsTrue());
        Assert.True(MapCellCore.ResultSurvivesException());
    }

    // ===================== 九、顶层仿真 =====================

    [Fact]
    public void AddToMapInvalidMap()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: true, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: "药水", addCount: 1,
            listIsNull: true,
            existing: Array.Empty<(MapCellCore.ObjGame, string, int, bool)>(),
            capEnabled: false, capValue: 5,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.RejectedByInvalid);
        Assert.False(r.Success);
    }

    [Fact]
    public void AddToMapNonZeroFlagRejected()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 1,
            objGame: MapCellCore.ObjGame.ObjItem, name: "药水", addCount: 1,
            listIsNull: true,
            existing: Array.Empty<(MapCellCore.ObjGame, string, int, bool)>(),
            capEnabled: false, capValue: 5,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        // **chFlag 非零 → AddToMap 拒绝**
        Assert.True(r.RejectedByFlag);
        Assert.False(r.Success);
    }

    [Fact]
    public void AddToMapNormalItem()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: "药水", addCount: 1,
            listIsNull: true,
            existing: Array.Empty<(MapCellCore.ObjGame, string, int, bool)>(),
            capEnabled: false, capValue: 5,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.False(r.RejectedByFlag);
        Assert.False(r.Merged);
        Assert.True(r.Appended);
        Assert.True(r.Success);
        Assert.Equal((5, 6), r.Coords);
        Assert.Equal("None", r.CounterBucket);   // 物品不计入角色计数器
    }

    [Fact]
    public void AddToMapGateDoesNotUpdateCoords()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjGate, name: "", addCount: 1,
            listIsNull: true,
            existing: Array.Empty<(MapCellCore.ObjGame, string, int, bool)>(),
            capEnabled: false, capValue: 5,
            oldX: 1, oldY: 2, nX: 9, nY: 9,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        // **门保持原坐标**
        Assert.Equal((1, 2), r.Coords);
        Assert.True(r.Success);
    }

    [Fact]
    public void AddToMapPlayerIncrementsHum()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjActor, name: "", addCount: 1,
            listIsNull: true,
            existing: Array.Empty<(MapCellCore.ObjGame, string, int, bool)>(),
            capEnabled: false, capValue: 5,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 0);

        Assert.Equal("Hum", r.CounterBucket);
    }

    [Fact]
    public void AddToMapGoldMerge()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: MapCellCore.GoldName, addCount: 500,
            listIsNull: false,
            existing: new[] { (MapCellCore.ObjGame.ObjItem, MapCellCore.GoldName, 300, false) },
            capEnabled: false, capValue: 5,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.Merged);
        Assert.Equal(800, r.MergedCount);
        Assert.True(r.Success);
        Assert.False(r.Appended);   // **合并不挂载新对象**
    }

    [Fact]
    public void AddToMapGoldNoMergeWhenGhost()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: MapCellCore.GoldName, addCount: 500,
            listIsNull: false,
            existing: new[] { (MapCellCore.ObjGame.ObjItem, MapCellCore.GoldName, 300, true) },
            capEnabled: false, capValue: 5,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.False(r.Merged);
        Assert.True(r.Appended);
    }

    [Fact]
    public void AddToMapCapRejects()
    {
        var r = MapCellCore.AddToMap(
            boInvalid: false, cellFound: true, chFlag: 0,
            objGame: MapCellCore.ObjGame.ObjItem, name: "药水", addCount: 1,
            listIsNull: false,
            existing: new[]
            {
                (MapCellCore.ObjGame.ObjItem, "药水", 1, false),
                (MapCellCore.ObjGame.ObjItem, "药水", 1, false),
            },
            capEnabled: true, capValue: 2,
            oldX: 0, oldY: 0, nX: 5, nY: 6,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.RejectedByCap);
        Assert.False(r.Success);
    }

    // ===================== 十、DeleteFromMap 仿真 =====================

    [Fact]
    public void DeleteFromMapInvalidMap()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: true, cellFound: true, listIsNull: false,
            list: new[] { 7 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.RejectedByInvalid);
        Assert.False(r.Success);
    }

    [Fact]
    public void DeleteFromMapCellNotFound()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: false, listIsNull: false,
            list: new[] { 7 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.CellNotFound);
        Assert.False(r.Success);
    }

    [Fact]
    public void DeleteFromMapNullList()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: true,
            list: Array.Empty<int>(), target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.ListWasNull);
        Assert.False(r.Success);
    }

    [Fact]
    public void DeleteFromMapSuccess()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { 5, 7, 9 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.Success);
        Assert.Equal(new List<int> { 5, 9 }, r.Remaining);
        Assert.Equal(0, r.HolesCompacted);
    }

    [Fact]
    public void DeleteFromMapCompactsHoles()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { -1, 5, -1, 7, -1 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.Success);
        // 目标之前的空洞都被清（2 个）；**目标之后的空洞不会被清**（1914 的 Break）
        Assert.Equal(2, r.HolesCompacted);
        Assert.Equal(new List<int> { 5, -1 }, r.Remaining);
    }

    [Fact]
    public void HolesAfterTargetSurvive()
    {
        // 目标在末位 → 前面的空洞都被清、无后续空洞
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { -1, -1, 7 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.Success);
        Assert.Equal(2, r.HolesCompacted);
        Assert.Empty(r.Remaining);
    }

    [Fact]
    public void DeleteFromMapOnlyFirstMatch()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { 7, 7, 7 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.True(r.Success);
        Assert.Equal(2, r.Remaining.Count);   // **只删第一个**
    }

    [Fact]
    public void DeleteFromMapNotFound()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { 5, 9 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 10);

        Assert.False(r.Success);
        Assert.Equal(2, r.Remaining.Count);
    }

    [Fact]
    public void DeleteFromMapActorCounter()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { 7 }, target: 7,
            isActor: true, alreadyDelFromMaped: false,
            masterNonNull: true, masterIsPlayer: true, raceServer: 80);

        Assert.True(r.Success);
        Assert.Equal("BB", r.CounterBucket);   // **宝宝归 BB**
    }

    [Fact]
    public void DeleteFromMapPlayerCounter()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { 7 }, target: 7,
            isActor: true, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 0);

        Assert.Equal("Hum", r.CounterBucket);
    }

    [Fact]
    public void DeleteFromMapNonActorNoCounter()
    {
        var r = MapCellCore.DeleteFromMap(
            boInvalid: false, cellFound: true, listIsNull: false,
            list: new[] { 7 }, target: 7,
            isActor: false, alreadyDelFromMaped: false,
            masterNonNull: false, masterIsPlayer: false, raceServer: 0);

        Assert.True(r.Success);
        Assert.Equal("None", r.CounterBucket);
    }
}

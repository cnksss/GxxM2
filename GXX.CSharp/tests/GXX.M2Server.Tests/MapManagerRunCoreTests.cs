using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J157：`TMapManager` 其余五个方法 1:1 测试。
/// **矩形周长的"或"表达式、方向层级树、`Run` 的两个分片循环及其缺陷、
/// 正/倒序删除差异全部经探针实测。**
/// </summary>
public sealed class MapManagerRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.True(MapManagerRunCore.ConstantsMatchSource());
        Assert.True(MapManagerRunCore.DirectionValuesNotStartingAtUpLeft());
        Assert.Equal(7, MapManagerRunCore.DrUpLeft);
        Assert.Equal(0, MapManagerRunCore.DrUp);
        Assert.Equal(5, MapManagerRunCore.SliceLimitMs);
    }

    // ===================== 一、MakeSafePkZone 周长判定 =====================

    [Fact]
    public void PerimeterFormula()
    {
        Assert.True(MapManagerRunCore.PerimeterFormula());
        Assert.True(MapManagerRunCore.TopEdgeExcludesCorner());
        Assert.True(MapManagerRunCore.LeftEdgeExcludesCorner());
        Assert.True(MapManagerRunCore.RightEdgeIncludesBothCorners());
        Assert.True(MapManagerRunCore.BottomEdgeIncludesBothCorners());
    }

    [Fact]
    public void PerimeterSetIsExact()
    {
        // **探针实测：周长集合恰好 16 格、内部 9 格一个不选；
        // 但左上与右下两个对角被重复命中 2 次（14 格 1 次、2 格 2 次）**
        Assert.True(MapManagerRunCore.PerimeterCoversExactlyOnce());
        Assert.True(MapManagerRunCore.InteriorNotSelected());
    }

    [Fact]
    public void DoubleMatchedCorners()
    {
        // **左上与右下各命中 2 次**
        Assert.Equal(2, MapManagerRunCore.CountPerimeterMatches(0, 0, 0, 4, 0, 4));
        Assert.Equal(2, MapManagerRunCore.CountPerimeterMatches(4, 4, 0, 4, 0, 4));

        // 右上与左下只命中 1 次
        Assert.Equal(1, MapManagerRunCore.CountPerimeterMatches(4, 0, 0, 4, 0, 4));
        Assert.Equal(1, MapManagerRunCore.CountPerimeterMatches(0, 4, 0, 4, 0, 4));
    }

    [Fact]
    public void PerimeterCounts()
    {
        Assert.True(MapManagerRunCore.PerimeterCountFormula());
        Assert.Equal(16, MapManagerRunCore.PerimeterCount(0, 4, 0, 4));
        Assert.Equal(1, MapManagerRunCore.PerimeterCount(0, 0, 0, 0));
        Assert.Equal(4, MapManagerRunCore.PerimeterCount(0, 1, 0, 1));
    }

    // ===================== 方向层级树 =====================

    [Fact]
    public void DirectionHierarchy()
    {
        Assert.True(MapManagerRunCore.DirectionHierarchy());
        Assert.True(MapManagerRunCore.XBranchDecidesCorners());
        Assert.True(MapManagerRunCore.AllEightDirectionsUsed());
        Assert.True(MapManagerRunCore.DirectionAssignments());
    }

    [Fact]
    public void CornerDirections()
    {
        Assert.Equal(MapManagerRunCore.DrUpLeft, MapManagerRunCore.PerimeterDirection(0, 0, 0, 4, 0, 4));
        Assert.Equal(MapManagerRunCore.DrUpRight, MapManagerRunCore.PerimeterDirection(4, 0, 0, 4, 0, 4));
        Assert.Equal(MapManagerRunCore.DrDownLeft, MapManagerRunCore.PerimeterDirection(0, 4, 0, 4, 0, 4));
        Assert.Equal(MapManagerRunCore.DrDownRight, MapManagerRunCore.PerimeterDirection(4, 4, 0, 4, 0, 4));
    }

    [Fact]
    public void EdgeDirections()
    {
        Assert.Equal(MapManagerRunCore.DrUp, MapManagerRunCore.PerimeterDirection(2, 0, 0, 4, 0, 4));
        Assert.Equal(MapManagerRunCore.DrDown, MapManagerRunCore.PerimeterDirection(2, 4, 0, 4, 0, 4));
        Assert.Equal(MapManagerRunCore.DrLeft, MapManagerRunCore.PerimeterDirection(0, 2, 0, 4, 0, 4));
        Assert.Equal(MapManagerRunCore.DrRight, MapManagerRunCore.PerimeterDirection(4, 2, 0, 4, 0, 4));
    }

    // ===================== 两类区域的差异 =====================

    [Fact]
    public void TwoAreaKinds()
    {
        Assert.True(MapManagerRunCore.ShowTypeGateOnlyForRange());
        Assert.True(MapManagerRunCore.ShowTypeGateValues());
        Assert.True(MapManagerRunCore.AllotypeIgnoresShowType());
        Assert.True(MapManagerRunCore.NullMapSkips());
    }

    [Fact]
    public void EventSignatures()
    {
        Assert.True(MapManagerRunCore.EventSignaturesDiffer());
        Assert.True(MapManagerRunCore.EventSignaturesAreDifferent());
        Assert.Equal(2, MapManagerRunCore.EventSignatures.Length);
    }

    [Fact]
    public void YOnRowXOnPoint()
    {
        // **行管 Y、点管 X**
        Assert.True(MapManagerRunCore.YOnRowXOnPoint());
        Assert.True(MapManagerRunCore.AllotypePointValues());

        var (x, y) = MapManagerRunCore.AllotypePoint(7, 3);

        Assert.Equal(3, x);
        Assert.Equal(7, y);
    }

    [Fact]
    public void TraversalAndExpansion()
    {
        Assert.True(MapManagerRunCore.AreaTraversalsDiffer());
        Assert.True(MapManagerRunCore.RangeExpansion());
        Assert.Equal(5, MapManagerRunCore.RangeWidth(2));
    }

    // ===================== 二、MakeMapMagic =====================

    [Fact]
    public void MakeMapMagicIsHollow()
    {
        Assert.True(MapManagerRunCore.MakeMapMagicIsOneBigComment());
        Assert.True(MapManagerRunCore.ParenStarCommentStyle());
        Assert.True(MapManagerRunCore.CommentedCodeUsesContinue());
        Assert.True(MapManagerRunCore.ThreeCommentStyles());
    }

    [Fact]
    public void NinthEmptyShellForm()
    {
        // **第九种：整块注释 + 空过程体的叠加**
        Assert.True(MapManagerRunCore.NinthEmptyShellForm());
        Assert.True(MapManagerRunCore.NineEmptyShellForms());
        Assert.True(MapManagerRunCore.NinthIsCombination());
        Assert.Equal(9, MapManagerRunCore.EmptyShellForms.Length);
        Assert.Contains("叠加", MapManagerRunCore.EmptyShellForms[8]);
    }

    [Fact]
    public void DiffersFromProcessMapDoor()
    {
        Assert.True(MapManagerRunCore.DiffersFromProcessMapDoor());
        Assert.True(MapManagerRunCore.EmptyShellKindsDiffer());
        Assert.Equal("空过程体、没有注释", MapManagerRunCore.EmptyShellKind("ProcessMapDoor"));
    }

    [Fact]
    public void IntListCompare()
    {
        // **把指针当整数直接相减、不做三态归一**
        Assert.True(MapManagerRunCore.IntListComparePointerArithmetic());
        Assert.True(MapManagerRunCore.IntListCompareEqual());
        Assert.True(MapManagerRunCore.IntListCompareSign());
        Assert.True(MapManagerRunCore.IntListCompareNotNormalised());

        Assert.Equal(-5, MapManagerRunCore.IntListCompare(100, 105));
        Assert.Equal(0, MapManagerRunCore.IntListCompare(100, 100));
    }

    // ===================== 三、Create / Destroy =====================

    [Fact]
    public void Create()
    {
        Assert.True(MapManagerRunCore.CreateThreeSteps());
        Assert.True(MapManagerRunCore.CreateIndexesAreZero());
        Assert.True(MapManagerRunCore.CreateMakesNoMaps());

        var (m, g) = MapManagerRunCore.CreateIndexes();

        Assert.Equal(0, m);
        Assert.Equal(0, g);
    }

    [Fact]
    public void DestroyOrder()
    {
        // **先释放门、后释放地图（避免门里的地图指针悬空）**
        Assert.True(MapManagerRunCore.DestroyReleasesGatesBeforeMaps());
        Assert.True(MapManagerRunCore.DestroyOrderSteps());
        Assert.True(MapManagerRunCore.GatesFreedBeforeMaps());
        Assert.True(MapManagerRunCore.DestroyOrderAvoidsDangling());
        Assert.Equal(4, MapManagerRunCore.DestroyOrder.Length);
    }

    [Fact]
    public void OppositeIterationDirections()
    {
        // **Destroy 正序、Run 倒序**
        Assert.True(MapManagerRunCore.DestroyUsesForwardLoop());
        Assert.True(MapManagerRunCore.RunUsesReverseLoop());
        Assert.True(MapManagerRunCore.OppositeIterationDirections());
        Assert.True(MapManagerRunCore.LoopDirectionsDiffer());
    }

    // ===================== 四、Run 地图段 =====================

    [Fact]
    public void SlicePattern()
    {
        Assert.True(MapManagerRunCore.SliceProcessingPattern());
        Assert.True(MapManagerRunCore.SliceLimitIsFiveMs());
        Assert.True(MapManagerRunCore.SliceExpiryStrictGreater());
        Assert.False(MapManagerRunCore.SliceExpired(5, 0));
        Assert.True(MapManagerRunCore.SliceExpired(6, 0));
    }

    [Fact]
    public void MapSegmentClamp()
    {
        Assert.True(MapManagerRunCore.MapSegmentSilentClamp());
        Assert.True(MapManagerRunCore.ClampIndexValues());
        Assert.Equal(0, MapManagerRunCore.ClampIndex(99, 3));
    }

    [Fact]
    public void MapSegmentDefectOneNeverAdvances()
    {
        // **缺陷一：nIdx 在循环体内从不被赋值 —— 写回的是初始值本身**
        Assert.True(MapManagerRunCore.MapSegmentNeverAdvancesIdx());
        Assert.True(MapManagerRunCore.MapSegmentIdentityWriteback());
        Assert.True(MapManagerRunCore.WritebackIndependentOfProgress());

        // 处理 1 个与处理 9 个，写回值完全相同
        Assert.Equal(
            MapManagerRunCore.MapSegmentWriteback(2, 10, 1),
            MapManagerRunCore.MapSegmentWriteback(2, 10, 9));
    }

    [Fact]
    public void MapSegmentDefectTwoDirectionMismatch()
    {
        // **缺陷二：倒序遍历、正序索引**
        Assert.True(MapManagerRunCore.MapSegmentDirectionMismatch());
        Assert.True(MapManagerRunCore.MapSegmentDirectionsConflict());
    }

    [Fact]
    public void MapSegmentDefectThreeTwoVariables()
    {
        // **缺陷三：循环变量与断点变量互不相干**
        Assert.True(MapManagerRunCore.MapSegmentTwoSeparateVariables());
        Assert.True(MapManagerRunCore.MapSegmentLimitFiveMs());
        Assert.True(MapManagerRunCore.MapSegmentResetsWhenNotLimited());
        Assert.True(MapManagerRunCore.FinalResetValues());
    }

    // ===================== Run 门段 =====================

    [Fact]
    public void GateSegmentStructure()
    {
        Assert.True(MapManagerRunCore.GateSegmentWhileTrue());
        Assert.True(MapManagerRunCore.GateLoopBoundValues());
        Assert.Equal(2, MapManagerRunCore.MapSegmentVariables.Length);
    }

    [Fact]
    public void GateSegmentDeletesAndContinues()
    {
        // **删掉当前项后 Continue、不自增 —— 这是正确写法**
        Assert.True(MapManagerRunCore.GateSegmentDeletesAndContinues());
        Assert.True(MapManagerRunCore.GateSegmentDeletesConsecutive());
    }

    [Fact]
    public void IncrementAfterDeleteWouldSkip()
    {
        // **对照：若删后自增则会漏项**
        Assert.True(MapManagerRunCore.IncrementAfterDeleteSkips());
    }

    [Fact]
    public void GateSegmentIncrementsOnNormalPath()
    {
        Assert.True(MapManagerRunCore.GateSegmentIncrementsOnNormalPath());
    }

    [Fact]
    public void SegmentsDifferInGuards()
    {
        // **地图段有 Lock + try/finally，门段两者都没有**
        Assert.True(MapManagerRunCore.GateSegmentNoLock());
        Assert.True(MapManagerRunCore.GateSegmentNoTryFinally());
        Assert.True(MapManagerRunCore.SegmentsDifferInGuards());
        Assert.True(MapManagerRunCore.GuardProfilesDiffer());
        Assert.Equal("有 Lock + try/finally", MapManagerRunCore.GuardProfile("map"));
        Assert.Equal("无锁、无 try/finally", MapManagerRunCore.GuardProfile("gate"));
    }

    [Fact]
    public void GateExpiry()
    {
        // **严格大于、且运行时间必须为正**
        Assert.True(MapManagerRunCore.GateSegmentExpiryCondition());
        Assert.True(MapManagerRunCore.ExpiryStrictGreaterThan());
        Assert.True(MapManagerRunCore.ZeroRunTimeNeverExpires());
        Assert.True(MapManagerRunCore.NegativeRunTimeNeverExpires());

        Assert.False(MapManagerRunCore.GateExpired(1000, 500, 500));
        Assert.True(MapManagerRunCore.GateExpired(1000, 500, 501));
    }

    [Fact]
    public void OnlyDeleteOnSuccess()
    {
        Assert.True(MapManagerRunCore.OnlyDeleteOnSuccess());
        Assert.True(MapManagerRunCore.FailedCellDeleteKeepsGate());
        Assert.True(MapManagerRunCore.SuccessfulCellDeleteRemovesGate());
    }

    // ===================== 镜像地图空壳 =====================

    [Fact]
    public void MirrorBlockIsEmpty()
    {
        // **镜像到期逻辑已被掏空、只剩空壳**
        Assert.True(MapManagerRunCore.MirrorBlockIsEmpty());
        Assert.True(MapManagerRunCore.MirrorConditionValues());
        Assert.True(MapManagerRunCore.MirrorStillCallsMapRun());
    }

    [Fact]
    public void MirrorCommentRemnants()
    {
        Assert.True(MapManagerRunCore.CommentedTickDiffDecl());
        Assert.True(MapManagerRunCore.CommentedDeclPresent());
        Assert.True(MapManagerRunCore.MirrorExpiryLogicCommented());
        Assert.True(MapManagerRunCore.ThreeMirrorKeywords());
        Assert.Equal(3, MapManagerRunCore.MirrorCommentKeywords.Length);
    }

    // ===================== 行数与移植完整性 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(MapManagerRunCore.FiveMethods());
        Assert.Equal(new[] { 89, 28, 9, 13, 78 }, MapManagerRunCore.MethodLineCounts);
        Assert.True(MapManagerRunCore.MakeSafePkZoneIsLongest());
        Assert.True(MapManagerRunCore.CreateIsShortest());
        Assert.True(MapManagerRunCore.TotalLinesValues());
        Assert.Equal(217, MapManagerRunCore.TotalLines());
    }

    [Fact]
    public void AllMethodsMigrated()
    {
        // **TMapManager 的 21 个方法至此全部移植完毕**
        Assert.True(MapManagerRunCore.AllTwentyOneMigrated());
        Assert.True(MapManagerRunCore.MatchesClassDeclaration());
        Assert.Equal(21, MapManagerRunCore.MigratedMethods.Length);
    }

    [Fact]
    public void MigratedMethodList()
    {
        Assert.Contains("Run", MapManagerRunCore.MigratedMethods);
        Assert.Contains("Destroy", MapManagerRunCore.MigratedMethods);
        Assert.Contains("MakeSafePkZone", MapManagerRunCore.MigratedMethods);
        Assert.Contains("MakeMapMagic", MapManagerRunCore.MigratedMethods);
        Assert.Contains("AddMapRoute(8)", MapManagerRunCore.MigratedMethods);
    }
}

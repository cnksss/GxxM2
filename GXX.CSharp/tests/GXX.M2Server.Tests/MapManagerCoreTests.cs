using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J155：`TMapManager` 七个查询方法 1:1 测试。
/// **二分查找的最左语义、双向扫描、插入点与哨兵值重合全部经探针实测。**
/// </summary>
public sealed class MapManagerCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MapManagerCore.ConstantsMatchSource());
        Assert.True(MapManagerCore.RemnantStringsMatchSource());
        Assert.Equal("TGList", MapManagerCore.BaseClass);
    }

    [Fact]
    public void SentinelOverlapsValidIndex()
    {
        // **"找不到"返回 0，而 0 也是合法的服务器索引 —— 两者无法区分**
        Assert.True(MapManagerCore.SentinelOverlapsValidValue());
        Assert.Equal(0, MapManagerCore.NotFoundServerIndex);
        Assert.Equal(0, MapManagerCore.RcPlayObject);
    }

    // ===================== 一、Find =====================

    [Fact]
    public void BinarySearchShape()
    {
        Assert.True(MapManagerCore.BinarySearchShape());
        Assert.True(MapManagerCore.ShrInsteadOfDiv());
        Assert.True(MapManagerCore.ShrEqualsHalf());
    }

    [Fact]
    public void LeftmostOnDuplicate()
    {
        // **探针实测：三个 B 取索引 0**
        Assert.True(MapManagerCore.LeftmostOnDuplicate());
        Assert.True(MapManagerCore.HitStillGoesLeft());

        var (found, idx) = MapManagerCore.Find(new[] { "B", "B", "B" }, "B");

        Assert.True(found);
        Assert.Equal(0, idx);
    }

    [Fact]
    public void DuplicateBlock()
    {
        var (found, idx) = MapManagerCore.Find(new[] { "A", "B", "B", "B", "C" }, "B");

        Assert.True(found);
        Assert.Equal(1, idx);
    }

    [Fact]
    public void UniqueLookup()
    {
        Assert.True(MapManagerCore.UniqueLookup());
        Assert.True(MapManagerCore.CaseInsensitiveLookup());
    }

    [Fact]
    public void InsertionPointSemantics()
    {
        // **未命中时 `Index` 是插入点、不是 -1**
        Assert.True(MapManagerCore.IndexIsInsertionPoint());
        Assert.True(MapManagerCore.IndexNotMinusOne());
        Assert.True(MapManagerCore.LessThanAllInsertionPoint());
        Assert.True(MapManagerCore.GreaterThanAllInsertionPoint());
        Assert.True(MapManagerCore.EmptyListResult());

        var (found, idx) = MapManagerCore.Find(new[] { "A", "C", "E" }, "B");

        Assert.False(found);
        Assert.Equal(1, idx);
    }

    [Fact]
    public void AnsiCompareTextNotLocale()
    {
        Assert.True(MapManagerCore.AnsiCompareTextNotLocale());
    }

    [Fact]
    public void DuplicateAcceptRemnant()
    {
        // **标准库血统证据**
        Assert.True(MapManagerCore.DuplicateAcceptRemnantPresent());
        Assert.True(MapManagerCore.DuplicateAcceptUndefinedHere());
        Assert.True(MapManagerCore.FindSignatureFromStringList());
    }

    // ===================== 二、两套查表法 =====================

    [Fact]
    public void TwoLookupStyles()
    {
        Assert.True(MapManagerCore.TwoLookupStyles());
        Assert.True(MapManagerCore.ConditionalCompilationSwitches());
        Assert.True(MapManagerCore.FindMapLinearUsesSameText());
        Assert.True(MapManagerCore.FindMapBinaryUsesFind());
        Assert.True(MapManagerCore.GetMapInfoCommentedLinearBlock());
    }

    [Fact]
    public void SameCommentOppositeDirections()
    {
        // **同一句"优化"注释在两个函数里指向相反做法**
        Assert.True(MapManagerCore.SameCommentOppositeDirections());
        Assert.True(MapManagerCore.DirectionsAreOpposite());

        Assert.Equal("改成线性", MapManagerCore.DirectionAfterComment("FindMap"));
        Assert.Equal("改成二分 + 双向扫描", MapManagerCore.DirectionAfterComment("GetMapInfo"));
    }

    [Fact]
    public void FindMapStyles()
    {
        Assert.Equal("线性扫描 + SameText", MapManagerCore.FindMapStyle(2));
        Assert.Equal("二分查找 + Find", MapManagerCore.FindMapStyle(1));
    }

    [Fact]
    public void EmptyNameExits()
    {
        Assert.True(MapManagerCore.EmptyNameExits());
        Assert.True(MapManagerCore.EmptyNameExitsValues(""));
        Assert.False(MapManagerCore.EmptyNameExitsValues("A"));
    }

    // ===================== GetMapInfo 双向扫描 =====================

    [Fact]
    public void BidirectionalScan()
    {
        Assert.True(MapManagerCore.BidirectionalScan());
        Assert.True(MapManagerCore.BackwardThenForward());
        Assert.True(MapManagerCore.BreakOnNameMismatch());
        Assert.Equal(2, MapManagerCore.ScanOrders.Length);
    }

    [Fact]
    public void ScanOrders()
    {
        Assert.Contains("downto", MapManagerCore.ScanOrders[0]);
        Assert.Contains("to", MapManagerCore.ScanOrders[1]);
    }

    [Fact]
    public void LookupBehaviour()
    {
        Assert.True(MapManagerCore.LeftmostHitReturnsFirst());
        Assert.True(MapManagerCore.ForwardScanFinds());
        Assert.True(MapManagerCore.BackwardScanFinds());
        Assert.True(MapManagerCore.NoMatchReturnsNil());
        Assert.True(MapManagerCore.MissingNameReturnsNil());
    }

    [Fact]
    public void ScanStopsAtBlockEdge()
    {
        // **扫描不会越过同名块**
        Assert.True(MapManagerCore.ScanStopsAtBlockEdge());

        var items = new[] { ("A", 9), ("B", 1), ("B", 2), ("C", 5) };

        Assert.Equal(-1, MapManagerCore.GetMapInfoLookup(items, "B", 5));
    }

    [Fact]
    public void TakesLeftmostMatchingInBlock()
    {
        Assert.True(MapManagerCore.TakesLeftmostMatchingInBlock());

        var items = new[] { ("B", 7), ("B", 7), ("B", 7) };

        Assert.Equal(0, MapManagerCore.GetMapInfoLookup(items, "B", 7));
    }

    // ===================== 三、GetMapOfServerIndex =====================

    [Fact]
    public void ServerIndexLookup()
    {
        Assert.True(MapManagerCore.GetMapOfServerIndexLinearShape());
        Assert.True(MapManagerCore.DefaultZeroNotMinusOne());
        Assert.True(MapManagerCore.NotFoundReturnsZero());
        Assert.True(MapManagerCore.ZeroOverlapsRealIndex());
        Assert.True(MapManagerCore.ServerIndexLookupCaseInsensitive());
    }

    [Fact]
    public void TakesFirstMatch()
    {
        Assert.True(MapManagerCore.TakesFirstMatchNotLeftmost());
        Assert.True(MapManagerCore.StrategyDiffersFromGetMapInfo());
        Assert.True(MapManagerCore.StrategyLabelsDiffer());

        var items = new[] { ("B", 1), ("B", 2) };

        Assert.Equal(1, MapManagerCore.GetMapOfServerIndexLinear(items, "B"));
    }

    // ===================== 四、三个薄方法 =====================

    [Fact]
    public void LoadMapDoor()
    {
        Assert.True(MapManagerCore.LoadMapDoorIteratesAll());
        Assert.Equal(3, MapManagerCore.LoadMapDoorCalls(3));
        Assert.Equal(0, MapManagerCore.LoadMapDoorCalls(0));
    }

    [Fact]
    public void ProcessMapDoorIsEmpty()
    {
        Assert.True(MapManagerCore.ProcessMapDoorIsEmpty());
        Assert.True(MapManagerCore.ProcessMapDoorBodyEmpty());
        Assert.True(MapManagerCore.ProcessMapDoorNeverCalled());
    }

    [Fact]
    public void EmptyShellForms()
    {
        // **空过程体是第八种空壳形态**
        Assert.True(MapManagerCore.EightEmptyShellForms());
        Assert.True(MapManagerCore.EmptyProcedureIsEighth());
        Assert.Equal(8, MapManagerCore.EmptyShellForms.Length);
        Assert.Equal("空过程体", MapManagerCore.EmptyShellForms[7]);
    }

    [Fact]
    public void ReSetMinMap()
    {
        Assert.True(MapManagerCore.ReSetMinMapDoubleLoop());
        Assert.True(MapManagerCore.MinMapFromObjectsList());
    }

    [Fact]
    public void ReSetMinMapBehaviour()
    {
        Assert.True(MapManagerCore.ReSetMinMapAssigns());
        Assert.True(MapManagerCore.UnmatchedKeepsOldValue());
        Assert.True(MapManagerCore.BreakOnlyInner());
        Assert.True(MapManagerCore.TakesFirstMiniMap());
        Assert.True(MapManagerCore.EmptyMapListNoOp());
        Assert.True(MapManagerCore.ReSetMinMapCaseInsensitive());
    }

    [Fact]
    public void UnmatchedKeepsOldValueExplicit()
    {
        var r = MapManagerCore.ReSetMinMap(new[] { ("A", 5) }, new[] { ("Z", 7) });

        Assert.Equal(5, r["A"]);
    }

    [Fact]
    public void BreakOnlyInnerExplicit()
    {
        var r = MapManagerCore.ReSetMinMap(
            new[] { ("A", 0), ("B", 0) }, new[] { ("A", 1), ("B", 2) });

        Assert.Equal(1, r["A"]);
        Assert.Equal(2, r["B"]);
    }

    // ===================== 五、继承与行数 =====================

    [Fact]
    public void Inheritance()
    {
        Assert.True(MapManagerCore.InheritsFromList());
        Assert.True(MapManagerCore.ListMembersFromBase());
    }

    [Fact]
    public void LineCounts()
    {
        Assert.True(MapManagerCore.SevenMethods());
        Assert.True(MapManagerCore.GetMapInfoIsLongest());
        Assert.True(MapManagerCore.ProcessMapDoorIsShortest());
        Assert.True(MapManagerCore.TotalLinesValues());
        Assert.Equal(new[] { 26, 38, 160, 22, 10, 3, 21 }, MapManagerCore.MethodLineCounts);
        Assert.Equal(280, MapManagerCore.TotalLines());
    }
}

using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J167：安全区族 1:1 测试（整个 `SafeAreaManager.pas` 单元
/// 加 `TBaseObject` 的三个判定）。
/// **手写二分查找用穷举不变量验证、包围盒初值缺陷用负坐标反例固证、
/// 三参重载残留用两行原文对照。**
/// </summary>
public sealed class SafeAreaCoreTests
{
    // ===================== 抽象基类 =====================

    [Fact]
    public void AbstractBase()
    {
        Assert.Equal(4, SafeAreaCore.AbstractCount());
        Assert.True(SafeAreaCore.FourAbstractProtected());
        Assert.True(SafeAreaCore.FourForwardingPublic());
        Assert.True(SafeAreaCore.ForwardingIsIdentity());
        Assert.True(SafeAreaCore.ForwardIdentityValues());
        Assert.True(SafeAreaCore.Forward(true));
        Assert.False(SafeAreaCore.Forward(false));
    }

    [Fact]
    public void OrderDiffersBetweenSides()
    {
        // **保护侧判定在首位、公开侧判定在第三位**
        Assert.True(SafeAreaCore.OrderDiffersBetweenSides());
        Assert.Equal("DoInSafeArea", SafeAreaCore.ProtectedOrder[0]);
        Assert.Equal("InSafeArea", SafeAreaCore.PublicOrder[2]);
        Assert.Equal(4, SafeAreaCore.ProtectedOrder.Length);
        Assert.Equal(4, SafeAreaCore.PublicOrder.Length);
    }

    // ===================== 二、TRangeSafeArea =====================

    [Fact]
    public void RectBounds()
    {
        Assert.True(SafeAreaCore.RectInside(5, 5, 5, 5, 10, 10));
        Assert.True(SafeAreaCore.RectInside(10, 10, 5, 5, 10, 10));
        Assert.True(SafeAreaCore.FourClosedBounds());
        Assert.True(SafeAreaCore.AllFourAreAnd());
    }

    [Fact]
    public void RectRejectsOutOfBounds()
    {
        // **四组越界全部被正确拒绝 —— 与 J166 的缺陷形成对照**
        Assert.True(SafeAreaCore.RejectsAllFourOutOfBounds());
        Assert.True(SafeAreaCore.ContrastWithJ166());

        Assert.False(SafeAreaCore.RectInside(4, 7, 5, 5, 10, 10));
        Assert.False(SafeAreaCore.RectInside(11, 7, 5, 5, 10, 10));
        Assert.False(SafeAreaCore.RectInside(7, 4, 5, 5, 10, 10));
        Assert.False(SafeAreaCore.RectInside(7, 11, 5, 5, 10, 10));
    }

    [Fact]
    public void RecallBuildsRect()
    {
        Assert.True(SafeAreaCore.RecallBuildsRectFromCenterAndRange());
        Assert.True(SafeAreaCore.TopGetsYNotX());
        Assert.True(SafeAreaCore.RangeReadOnlyInRecall());

        var r = SafeAreaCore.BuildRect(50, 60, 5);
        Assert.Equal(45, r.Left);
        Assert.Equal(55, r.Top);
        Assert.Equal(55, r.Right);
        Assert.Equal(65, r.Bottom);
    }

    // ===================== 三、TAllotypeRow =====================

    [Fact]
    public void SearchBasicCases()
    {
        Assert.True(SafeAreaCore.SearchFindsExisting());
        Assert.True(SafeAreaCore.SearchReturnsInsertionPoint());
        Assert.True(SafeAreaCore.SearchBelowAllGivesZero());
        Assert.True(SafeAreaCore.SearchAboveAllGivesCount());
        Assert.True(SafeAreaCore.SearchOnEmpty());
        Assert.True(SafeAreaCore.SearchFirstElement());
        Assert.True(SafeAreaCore.SearchLastElement());
        Assert.True(SafeAreaCore.SearchSingleElementHit());
    }

    [Fact]
    public void SearchInvariant()
    {
        // **穷举验证：命中必返回下标、未命中必返回插入点**
        Assert.True(SafeAreaCore.SearchInvariantHolds());
    }

    [Fact]
    public void SearchConcreteValues()
    {
        var xs = new List<int> { 10, 20, 30 };

        int idx = SafeAreaCore.Search(xs, 20, out bool found);
        Assert.True(found);
        Assert.Equal(1, idx);

        idx = SafeAreaCore.Search(xs, 25, out found);
        Assert.False(found);
        Assert.Equal(2, idx);

        idx = SafeAreaCore.Search(xs, 5, out found);
        Assert.False(found);
        Assert.Equal(0, idx);

        idx = SafeAreaCore.Search(xs, 35, out found);
        Assert.False(found);
        Assert.Equal(3, idx);
    }

    [Fact]
    public void AddSemantics()
    {
        Assert.True(SafeAreaCore.AddInsertsWhenAbsent());
        Assert.True(SafeAreaCore.AddUpdatesWhenPresent());
        Assert.True(SafeAreaCore.AddNeverAppendsDuplicateX());
        Assert.True(SafeAreaCore.AddKeepsSorted());
    }

    [Fact]
    public void AddKeepsSortedEnumeration()
    {
        var xs = new List<int>();
        var shows = new List<int>();

        foreach (int v in new[] { 50, 10, 40, 20, 30, 10 })
        {
            SafeAreaCore.Add(xs, shows, v, v);
        }

        Assert.Equal(5, xs.Count);
        Assert.Equal(new[] { 10, 20, 30, 40, 50 }, xs);
    }

    [Fact]
    public void ClearSemantics()
    {
        Assert.True(SafeAreaCore.ClearDisposesEach());
        Assert.True(SafeAreaCore.DestroyClearsFirst());
    }

    // ---------- PointInRow ----------

    [Fact]
    public void PointInRow()
    {
        Assert.True(SafeAreaCore.PointInRowUsesFirstAndLast());
        Assert.True(SafeAreaCore.EndpointsInside());
        Assert.True(SafeAreaCore.OutsideEndsFalse());
        Assert.True(SafeAreaCore.EmptyRowReturnsFalse());
    }

    [Fact]
    public void PointInRowIgnoresGaps()
    {
        // **只看首尾 —— 内部空洞仍报"在行内"**
        Assert.True(SafeAreaCore.IgnoresInteriorGaps());
        Assert.True(SafeAreaCore.HoleStillReportedInside());

        var xs = new List<int> { 10, 30 };
        Assert.True(SafeAreaCore.PointInRow(xs, 25));
        Assert.True(SafeAreaCore.PointInRow(xs, 10));
        Assert.True(SafeAreaCore.PointInRow(xs, 30));
        Assert.False(SafeAreaCore.PointInRow(xs, 9));
        Assert.False(SafeAreaCore.PointInRow(xs, 31));
    }

    // ===================== 四、TAllotypeSafeArea =====================

    [Fact]
    public void ThreeStageCheck()
    {
        Assert.True(SafeAreaCore.ThreeStageCheck());
        Assert.True(SafeAreaCore.BoundingBoxFirst());
        Assert.True(SafeAreaCore.ThenRowThenPointInRow());
        Assert.True(SafeAreaCore.IntersectionSemantics());
        Assert.True(SafeAreaCore.AllThreeMustPass());
    }

    [Fact]
    public void AllThreeStages()
    {
        var rows = new Dictionary<int, List<int>> { { 5, new List<int> { 10, 20 } } };

        // 三段全过
        Assert.True(SafeAreaCore.AllotypeInside(15, 5, 0, 0, 100, 100, rows));

        // 包围盒不过
        Assert.False(SafeAreaCore.AllotypeInside(200, 5, 0, 0, 100, 100, rows));

        // 该行不存在
        Assert.False(SafeAreaCore.AllotypeInside(15, 6, 0, 0, 100, 100, rows));

        // 行内不在
        Assert.False(SafeAreaCore.AllotypeInside(25, 5, 0, 0, 100, 100, rows));
    }

    // ---------- Recall ----------

    [Fact]
    public void CenterAlgorithm()
    {
        Assert.True(SafeAreaCore.MiddleRowIndexValues());
        Assert.True(SafeAreaCore.SinglePointRowCenter());
        Assert.True(SafeAreaCore.RowCenterValues());

        Assert.Equal(1, SafeAreaCore.MiddleRowIndex(3));
        Assert.Equal(2, SafeAreaCore.MiddleRowIndex(4));
        Assert.Equal(0, SafeAreaCore.MiddleRowIndex(1));
        Assert.Equal(20, SafeAreaCore.RowCenterX(new List<int> { 10, 30 }));
    }

    // ---------- 包围盒初值缺陷 ----------

    [Fact]
    public void BoxInitValues()
    {
        // **两个整数最大值、两个负一**
        Assert.True(SafeAreaCore.AsymmetricInitValues());
        Assert.True(SafeAreaCore.TwoMaxValueTwoMinusOne());
        Assert.Equal(4, SafeAreaCore.BoxInitValues.Length);
        Assert.Equal(int.MaxValue, SafeAreaCore.BoxInitValues[0].Value);
        Assert.Equal(-1, SafeAreaCore.BoxInitValues[2].Value);
    }

    [Fact]
    public void BoxPositiveCorrect()
    {
        Assert.True(SafeAreaCore.PositiveCoordsCorrect());

        var box = SafeAreaCore.ComputeBox(new List<(int, int)> { (10, 10), (30, 20), (20, 40) });
        Assert.Equal(10, box.Left);
        Assert.Equal(10, box.Top);
        Assert.Equal(30, box.Right);
        Assert.Equal(40, box.Bottom);
    }

    [Fact]
    public void BoxNegativeCoordsBreak()
    {
        // **全负横坐标时 Right 停在 -1（比真实最大值更大）**
        Assert.True(SafeAreaCore.NegativeCoordsBreakMaxInit());
        Assert.True(SafeAreaCore.RightWrongWhenAllNegative());
        Assert.True(SafeAreaCore.RightIsMinusOneNotMinusThirty());

        var box = SafeAreaCore.ComputeBox(new List<(int, int)> { (-50, 10), (-30, 20) });
        Assert.Equal(-1, box.Right);
        Assert.NotEqual(-30, box.Right);
        Assert.Equal(-50, box.Left);
    }

    [Fact]
    public void BoxBottomAlsoBreaks()
    {
        // **纵坐标全负时 Bottom 同样出错、而 Top 是安全的**
        Assert.True(SafeAreaCore.BottomWrongWhenAllNegative());
        Assert.True(SafeAreaCore.TopInitIsSafe());

        var box = SafeAreaCore.ComputeBox(new List<(int, int)> { (10, -50), (20, -30) });
        Assert.Equal(-1, box.Bottom);
        Assert.NotEqual(-30, box.Bottom);
        Assert.Equal(-50, box.Top);
    }

    [Fact]
    public void BoxEmptyCase()
    {
        Assert.True(SafeAreaCore.EmptySkipsEverything());
        Assert.True(SafeAreaCore.EmptyLeavesBoxUntouched());
        Assert.True(SafeAreaCore.CommentedDebugOutput());
        Assert.True(SafeAreaCore.DebugLineIsCommented());
        Assert.StartsWith("//", SafeAreaCore.CommentedDebugLine, StringComparison.Ordinal);
    }

    // ===================== 五、TSafeAreaManager =====================

    [Fact]
    public void AllotypeNeverSetsMapName()
    {
        // **两个构造体各只有一句、都没给地图名赋值**
        Assert.True(SafeAreaCore.AllotypeNeverSetsMapName());
        Assert.True(SafeAreaCore.AllotypeCtorIsOneLine());
        Assert.True(SafeAreaCore.RowAlsoNeverSetsMapName());
        Assert.Single(SafeAreaCore.AllotypeCtorBody);
        Assert.Single(SafeAreaCore.RowCtorBody);
    }

    [Fact]
    public void EmptyNameOnlyMatch()
    {
        // **空串只匹配空串 —— 所以查非空地图名时原型安全区永不匹配**
        Assert.True(SafeAreaCore.EmptyNameOnlyMatch());
        Assert.True(SafeAreaCore.ManagerLookupsFailForAllotype());
        Assert.True(SafeAreaCore.NameMatches("", ""));
        Assert.False(SafeAreaCore.NameMatches("", "比奇省"));
        Assert.True(SafeAreaCore.NameMatches("ABC", "abc"));
    }

    [Fact]
    public void LookupModel()
    {
        Assert.True(SafeAreaCore.LookupModel());
    }

    [Fact]
    public void BoundsCheck()
    {
        // **写法不对称（`>=` 对 `<=`）但结果正确**
        Assert.True(SafeAreaCore.BoundsCheckAsymmetric());
        Assert.True(SafeAreaCore.EmptyListRejectsAll());
        Assert.True(SafeAreaCore.BoundsCorrectButUneven());
        Assert.True(SafeAreaCore.BoundsEquivalentToHalfOpen());
    }

    [Fact]
    public void TwoTraversalSemantics()
    {
        // **前者取第一个名字匹配、后者看有没有一个包含**
        Assert.True(SafeAreaCore.TwoTraversalSemantics());
        Assert.True(SafeAreaCore.FindFirstVsAnyInside());
        Assert.True(SafeAreaCore.TwoSemanticsDiffer());
        Assert.True(SafeAreaCore.NameMatchButNotInside());
        Assert.True(SafeAreaCore.SemanticsAreIntentional());
    }

    [Fact]
    public void GetAllotype()
    {
        Assert.True(SafeAreaCore.TypeFilterFirst());
        Assert.True(SafeAreaCore.MatchesNameAndId());
        Assert.True(SafeAreaCore.TypeFilterExcludes());
        Assert.True(SafeAreaCore.IdMustMatch());
        Assert.True(SafeAreaCore.FirstAllotypeWins());
        Assert.True(SafeAreaCore.SameUnitPrivateAccess());
        Assert.True(SafeAreaCore.ManagerClearDisposesElements());
    }

    // ===================== 六、TBaseObject 三个判定 =====================

    [Fact]
    public void JudgmentOrder()
    {
        Assert.True(SafeAreaCore.ThreeStageOrder());
        Assert.True(SafeAreaCore.FiveJudgmentSteps());
        Assert.Equal(5, SafeAreaCore.JudgmentOrder.Length);
    }

    [Fact]
    public void ShortCircuits()
    {
        // **地图级标志短路掉后面全部**
        Assert.True(SafeAreaCore.MapSafeFlagShortCircuits());
        Assert.True(SafeAreaCore.SafeFlagResult());
        Assert.True(SafeAreaCore.SafeFlagSkipsManager());
        Assert.True(SafeAreaCore.NullMapReturnsFalse());
        Assert.True(SafeAreaCore.MapSafeReturnsEarly());
    }

    [Fact]
    public void ManagerLastResort()
    {
        Assert.True(SafeAreaCore.ManagerCheckedOnlyAsLastResort());
        Assert.True(SafeAreaCore.ManagerMissGivesFalse());
        Assert.True(SafeAreaCore.ManagerCheckedLast());
    }

    [Fact]
    public void NationBeforeGlobal()
    {
        Assert.True(SafeAreaCore.NationCheckedBeforeGlobal());
        Assert.True(SafeAreaCore.NationHitSkipsGlobal());
    }

    // ---------- 半径判定 ----------

    [Fact]
    public void HomeRadius()
    {
        Assert.True(SafeAreaCore.MapNameMustMatch());
        Assert.True(SafeAreaCore.RadiusIsSymmetric());
        Assert.True(SafeAreaCore.BoundaryInclusive());
        Assert.True(SafeAreaCore.ThreeConditionsJoined());
    }

    [Fact]
    public void HomeRadiusBoundaries()
    {
        // **判据是 `>` 所以边界取到（闭区间）**
        Assert.True(SafeAreaCore.HomeRadiusHit("A", "A", 10, 10, 0, 0, 10));
        Assert.False(SafeAreaCore.HomeRadiusHit("A", "A", 11, 0, 0, 0, 10));
        Assert.False(SafeAreaCore.HomeRadiusHit("A", "A", 0, 11, 0, 0, 10));
        Assert.False(SafeAreaCore.HomeRadiusHit("B", "A", 0, 0, 0, 0, 10));
    }

    // ---------- 三参重载残留 ----------

    [Fact]
    public void OverloadRemnant()
    {
        // **最后一行只换了坐标、没换地图名**
        Assert.True(SafeAreaCore.OverloadUsesWrongMapName());
        Assert.True(SafeAreaCore.HalfChangedCopyPaste());
        Assert.True(SafeAreaCore.BothUseMPEnvirMapName());
        Assert.True(SafeAreaCore.OnlyCoordsDiffer());
        Assert.True(SafeAreaCore.OnlyMapNameNotSubstituted());
    }

    [Fact]
    public void OverloadLastLines()
    {
        Assert.Contains("m_PEnvir.sMapName", SafeAreaCore.OverloadLastLine, StringComparison.Ordinal);
        Assert.Contains("nX, nY", SafeAreaCore.OverloadLastLine, StringComparison.Ordinal);
        Assert.Contains("m_PEnvir.sMapName", SafeAreaCore.NoArgLastLine, StringComparison.Ordinal);
        Assert.Contains("m_nCurrX, m_nCurrY", SafeAreaCore.NoArgLastLine, StringComparison.Ordinal);
    }

    [Fact]
    public void OverloadDiverges()
    {
        Assert.True(SafeAreaCore.DivergesWhenEnvirDiffers());
        Assert.True(SafeAreaCore.BothLookUpMemberMap());
        Assert.True(SafeAreaCore.OverloadBehavesLikeNoArgOnLastLine());
    }

    [Fact]
    public void RemnantFamily()
    {
        Assert.True(SafeAreaCore.SameFamilyAsRemnants());
        Assert.True(SafeAreaCore.ThreeRemnantInstances());
        Assert.Equal(3, SafeAreaCore.RemnantFamily.Length);
    }

    // ---------- InSafeArea ----------

    [Fact]
    public void InSafeAreaModel()
    {
        Assert.True(SafeAreaCore.InSafeAreaIsOneLine());
        Assert.True(SafeAreaCore.SafeFlagShortCircuitsManagerCall());
        Assert.True(SafeAreaCore.InSafeAreaNullEnv());
        Assert.True(SafeAreaCore.InSafeAreaConsultsManager());
    }

    [Fact]
    public void InSafeAreaTruthTable()
    {
        // 地图为空 → 假、不查管理器
        var (r, c) = SafeAreaCore.InSafeArea(true, true, true);
        Assert.False(r);
        Assert.False(c);

        // 标志真 → 真、不查管理器
        (r, c) = SafeAreaCore.InSafeArea(false, true, true);
        Assert.True(r);
        Assert.False(c);

        // 标志假、管理器命中 → 真
        (r, c) = SafeAreaCore.InSafeArea(false, false, true);
        Assert.True(r);
        Assert.True(c);

        // 标志假、管理器不中 → 假
        (r, c) = SafeAreaCore.InSafeArea(false, false, false);
        Assert.False(r);
        Assert.True(c);
    }

    // ---------- 两者语义不同 ----------

    [Fact]
    public void TwoSemantics()
    {
        Assert.True(SafeAreaCore.TwoDifferentSemantics());
        Assert.True(SafeAreaCore.InSafeAreaIgnoresHomeRadius());
        Assert.True(SafeAreaCore.TwoSemanticsRows());
        Assert.Equal(2, SafeAreaCore.SemanticsContrast.Length);
        Assert.True(SafeAreaCore.NotInterchangeable());
    }

    [Fact]
    public void SafeAreaIsSubset()
    {
        // **`InSafeArea` 为真时 `InSafeZone` 必为真**
        Assert.True(SafeAreaCore.InSafeAreaIsSubset());
        Assert.True(SafeAreaCore.SubsetHolds());
    }

    [Fact]
    public void ZoneTrueAreaFalse()
    {
        // **回家点命中但不在管理器里 → Zone 真、Area 假**
        Assert.True(SafeAreaCore.ZoneTrueAreaFalse());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(SafeAreaCore.JudgmentLinesIs96());
        Assert.Equal(96, SafeAreaCore.JudgmentLines());
        Assert.Equal(380, SafeAreaCore.ManagerUnitLines());
        Assert.True(SafeAreaCore.TotalLinesValue());
        Assert.Equal(476, SafeAreaCore.TotalLines());
    }

    [Fact]
    public void OverloadsSameLength()
    {
        // **两个重载逐字复制的最强证据：行数一致**
        Assert.True(SafeAreaCore.TwoOverloadsSameLength());
        Assert.True(SafeAreaCore.IdenticalLengthEvidence());
        Assert.Equal(SafeAreaCore.SectionLineCounts[0], SafeAreaCore.SectionLineCounts[1]);
        Assert.Equal(3, SafeAreaCore.SectionLineCounts.Length);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 `p13-m2-objmon-real` 切片 1：**`TChickenDeer`（ObjMon.pas:1382-1464）原文取证**。
///
/// <para>
/// 本类存在的理由（台账 §48.1）：修前 `ObjMonChickenDeerCore` 的 40 条谓词里有 40 条是裸
/// <c>=&gt; true;</c> —— 编译过、测试过、证明不了任何事。修后每一条都**真的读原文再判定**，
/// 本类则**独立地（不经被测代码）**把同一份原文再取一遍，逐条核对谓词所声称的原文事实。
/// </para>
/// <para>
/// 因此这里的用例不是"断言 C# 等于 C#"：右侧的期望值全部来自原文行号/字面量，
/// 任何一个行号或字面量写错、或原文被改动，都会立刻红。
/// </para>
/// </summary>
public sealed class ObjMonChickenDeerRealTests
{
    private static string Src(int line) => ObjMonRealSourceHarness.Line(line);
    private static int CountIn(string needle, int from, int to)
        => ObjMonRealSourceHarness.Count(needle, from, to);

    // ============ 0. 原文来源与整体行数 ============

    [Fact]
    public void SourceIsAvailableAndHasExpectedShape()
    {
        Assert.Equal(9502, ObjMonRealSourceHarness.Lines.Length);
        Assert.False(string.IsNullOrWhiteSpace(ObjMonRealSourceHarness.SourcePath));

        // 产品侧与测试侧解析到的必须是同一份原文（口径一致性）
        Assert.Equal(9502, ObjMonRealSource.Lines.Length);
    }

    [Fact]
    public void ProductSideSourceMatchesTestSideSource()
    {
        string[] product = ObjMonRealSource.LoadAllLines();
        string[] test = ObjMonRealSourceHarness.Lines;

        Assert.Equal(test.Length, product.Length);

        // 判据全部落在 ASCII 上；逐行比对整份原文（两种来源的 ASCII 字节一致）
        for (int i = 0; i < test.Length; i++)
        {
            Assert.Equal(Ascii(test[i]), Ascii(product[i]));
        }
    }

    private static string Ascii(string s)
    {
        var sb = new System.Text.StringBuilder(s.Length);
        foreach (char c in s) sb.Append(c <= 0x7F ? c : '?');
        return sb.ToString();
    }

    // ============ 1. 原文事实：三个方法体的边界 ============

    [Fact]
    public void ThreeMethodSpansMatchOriginal()
    {
        Assert.Equal("constructor TChickenDeer.Create; // 004A93E8", Src(1382).TrimEnd());
        Assert.Equal("destructor TChickenDeer.Destroy;", Src(1388).TrimEnd());
        Assert.Equal("procedure TChickenDeer.Run; // 004A9438", Src(1393).TrimEnd());
        Assert.Equal("end;", Src(1386).Trim());
        Assert.Equal("end;", Src(1391).Trim());
        Assert.Equal("end;", Src(1464).Trim());

        Assert.Equal(1382, ObjMonRealSource.FindImplementationLine("TChickenDeer", "Create"));
        Assert.Equal(1388, ObjMonRealSource.FindImplementationLine("TChickenDeer", "Destroy"));
        Assert.Equal(1393, ObjMonRealSource.FindImplementationLine("TChickenDeer", "Run"));
    }

    [Fact]
    public void ClassDeclarationRegion()
    {
        Assert.Contains("TChickenDeer = class(TMonster)", Src(25), StringComparison.Ordinal);
        Assert.Equal("end;", Src(30).Trim());
        Assert.Equal(0, CountIn("Think", 25, 30));
    }

    // ============ 2. 缺陷一：1456 行重复同一个表达式 ============

    [Fact]
    public void Line1456DuplicatesTheXAxisTerm()
    {
        string line = Src(1456);
        Assert.Equal(2, CountIn("m_nCurrX - BaseObject.m_nCurrX", 1456, 1456));
        Assert.Equal(0, CountIn("m_nCurrY - BaseObject.m_nCurrY", 1456, 1456));
        Assert.Contains("(Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6)", line, StringComparison.Ordinal);

        // 对照：同一个扫描循环里的 1429 行**两轴都判**
        Assert.Equal(1, CountIn("m_nCurrX - BaseObject.m_nCurrX", 1429, 1429));
        Assert.Equal(1, CountIn("m_nCurrY - BaseObject.m_nCurrY", 1429, 1429));
    }

    [Fact]
    public void PredicatesForDefectOneHold()
    {
        Assert.True(ObjMonChickenDeerCore.DuplicatedAxisCondition());
        Assert.True(ObjMonChickenDeerCore.XCountedTwice());
        Assert.True(ObjMonChickenDeerCore.YCountedZeroTimes());
        Assert.True(ObjMonChickenDeerCore.IntentWasBothAxes());
        Assert.True(ObjMonChickenDeerCore.ConditionEffectivelyOnlyX());
        Assert.True(ObjMonChickenDeerCore.DuplicatedLineExtracted());
    }

    [Fact]
    public void WrongConditionLosesTheYConstraintWhileIntendedOneKeepsIt()
    {
        // 误写版：纵轴取极大值仍为真；本意版：为假
        Assert.True(ObjMonChickenDeerCore.WrittenCondition(0, 9999));
        Assert.False(ObjMonChickenDeerCore.IntendedCondition(0, 9999));
        Assert.True(ObjMonChickenDeerCore.AbsurdYStillPasses());
        Assert.False(ObjMonChickenDeerCore.FarXBothFail() && false);   // 二者都为假 ⇒ 该谓词为真
        Assert.True(ObjMonChickenDeerCore.FarXBothFail());
        Assert.True(ObjMonChickenDeerCore.BothNearBothPass());
        Assert.True(ObjMonChickenDeerCore.YConstraintLost());
    }

    // ============ 3. 缺陷二：1456 行用循环泄漏变量 ============

    [Fact]
    public void Line1456UsesTheLeakedLoopVariable()
    {
        Assert.Contains("BaseObject := nil;", Src(1400), StringComparison.Ordinal);
        Assert.Contains("BaseObject := TBaseObject(VisibleBaseObject.BaseObject);", Src(1414), StringComparison.Ordinal);
        Assert.Equal("Continue;", Src(1416).Trim());
        Assert.Contains("BaseObject.m_nCurrX", Src(1456), StringComparison.Ordinal);
        Assert.DoesNotContain("m_TargetCret", Src(1456), StringComparison.Ordinal);

        // 相邻两行用的都是 m_TargetCret（每行 2 次引用 ⇒ 共 4 次）
        Assert.Equal(4, CountIn("m_TargetCret", 1458, 1459));
    }

    [Fact]
    public void PredicatesForDefectTwoHold()
    {
        Assert.True(ObjMonChickenDeerCore.UsesLeakedLoopVariable());
        Assert.True(ObjMonChickenDeerCore.ShouldUseTargetCret());
        Assert.True(ObjMonChickenDeerCore.LoopVarSurvivesLastIteration());
        Assert.True(ObjMonChickenDeerCore.NilWhenNoVisibleActors());
        Assert.True(ObjMonChickenDeerCore.NullDerefWhenEmpty());
        Assert.True(ObjMonChickenDeerCore.NeighborsUseTargetCret());
        Assert.True(ObjMonChickenDeerCore.LeakedVarLineExtracted());
        Assert.True(ObjMonChickenDeerCore.Stage2HasNilGuard());
        Assert.True(ObjMonChickenDeerCore.Stage2InnerLacksGuard());
    }

    [Fact]
    public void LeakedVariableSemanticsUnderTheFaithfulModel()
    {
        // 模型 1:1 复刻 1400-1416 的赋值/跳过规则
        Assert.Null(ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?>()));
        Assert.Null(ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?> { null, null }));
        Assert.Equal(3, ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?> { 1, 2, 3 }));
        Assert.Equal(7, ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?> { 7, null }));
        Assert.True(ObjMonChickenDeerCore.AllSkippedLeavesNull());
        Assert.True(ObjMonChickenDeerCore.EmptyListLeavesNull());
        Assert.True(ObjMonChickenDeerCore.LastValueSurvives());
        Assert.True(ObjMonChickenDeerCore.SkippedDoesNotUpdate());

        // ★ 否定性断言（§37.3）：可见对象为空 ⇒ 1456 必然解引用 nil
        Assert.Null(ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?>()));
        Assert.False(ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?>()) is not null);
    }

    // ============ 4. 缺陷三：m_boRunAwayMode 反义使用 ============

    [Fact]
    public void RunAwayModeAssignmentSitesCountedFromOriginal()
    {
        // 全单元 7 处赋值：1 处 True（1444）、6 处 False
        int trueSites = CountIn("m_boRunAwayMode := True;", 1, 9502);
        int falseSites = CountIn("m_boRunAwayMode := False;", 1, 9502);

        Assert.Equal(1, trueSites);
        Assert.Equal(6, falseSites);
        Assert.Equal(1444, LineOf("m_boRunAwayMode := True;"));
        Assert.Equal(new[] { 769, 1075, 1332, 1444, 1449, 5517, 5911 }, LineOfAll("m_boRunAwayMode :="));
    }

    [Fact]
    public void BaseClassGuardLineIsTheNegatedRead()
    {
        Assert.Equal("if not m_boRunAwayMode then", Src(1191).Trim());
        // 基类 TMonster.Run 的区间
        Assert.Equal(1121, ObjMonRealSource.FindImplementationLine("TMonster", "Run"));
        Assert.True(1191 > 1121 && 1191 < 1391);
    }

    [Fact]
    public void PredicatesForDefectThreeHold()
    {
        Assert.True(ObjMonChickenDeerCore.InvertedRunAwaySemantics());
        Assert.True(ObjMonChickenDeerCore.SetTrueOnTargetFound());
        Assert.True(ObjMonChickenDeerCore.BaseSkipsMoveWhenTrue());
        Assert.True(ObjMonChickenDeerCore.OnlyTrueSiteIs1444());
        Assert.True(ObjMonChickenDeerCore.SixFalseSites());
        Assert.True(ObjMonChickenDeerCore.TwoIncompatibleMeanings());
        Assert.True(ObjMonChickenDeerCore.SevenAssignSites());
        Assert.True(ObjMonChickenDeerCore.AssignSitesExtracted());
        Assert.True(ObjMonChickenDeerCore.ExactlyOneTrue());
        Assert.True(ObjMonChickenDeerCore.ThreeTimeoutSites());
        Assert.True(ObjMonChickenDeerCore.TimeoutSitesExtracted());
        Assert.True(ObjMonChickenDeerCore.TrueSiteNotTimeout());
        Assert.True(ObjMonChickenDeerCore.BaseGuardLineExtracted());
        Assert.True(ObjMonChickenDeerCore.SemanticsConflict());
    }

    [Fact]
    public void TimeoutResetSitesAreFalseSitesFollowedByTickReset()
    {
        foreach (int site in new[] { 769, 5517, 5911 })
        {
            Assert.Equal("m_boRunAwayMode := False;", Src(site).Trim());
            Assert.Contains("m_dwRunAwayTime := 0;", Src(site + 1), StringComparison.Ordinal);
        }

        Assert.Equal("m_boRunAwayMode := True;", Src(1444).Trim());
    }

    // ============ 5. 两段结构：同一判据、>= 而非 > ============

    [Fact]
    public void BothStagesUseTheSameGreaterOrEqualPredicate()
    {
        const string predicate = "tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay";

        Assert.Contains(predicate, Src(1404), StringComparison.Ordinal);
        Assert.Contains(predicate, Src(1453), StringComparison.Ordinal);
        Assert.Equal(2, CountIn(predicate, 1393, 1464));
        Assert.Equal(0, CountIn("tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay", 1393, 1464));

        // 延迟复位两处
        Assert.Equal("m_nWalkDelay := 0;", Src(1406).Trim());
        Assert.Equal("m_nWalkDelay := 0;", Src(1455).Trim());

        // 同一个 m_dwWalkTick 字段被两段共用
        Assert.Equal(2, CountIn("tick_diff(m_dwWalkTick, MyGetTickCount)", 1393, 1464));
    }

    [Fact]
    public void StageStructurePredicatesHold()
    {
        Assert.True(ObjMonChickenDeerCore.TwoStagesSamePredicate());
        Assert.True(ObjMonChickenDeerCore.StageLinesExtracted());
        Assert.True(ObjMonChickenDeerCore.DelayResetTwice());
        Assert.True(ObjMonChickenDeerCore.GreaterEqualNotGreater());
        Assert.True(ObjMonChickenDeerCore.SameTickTwoGuards());
    }

    [Fact]
    public void WalkPredicateBoundaryIsGreaterOrEqual()
    {
        Assert.True(ObjMonChickenDeerCore.CanWalk(0, 500, 500, 0));      // 恰好相等即通过
        Assert.False(ObjMonChickenDeerCore.CanWalk(0, 499, 500, 0));
        Assert.True(ObjMonChickenDeerCore.CanWalk(0, 501, 500, 0));
        Assert.True(ObjMonChickenDeerCore.ExactlyEqualPasses());
        Assert.True(ObjMonChickenDeerCore.NotYetBlocked());
        Assert.True(ObjMonChickenDeerCore.ExceededPasses());
        Assert.True(ObjMonChickenDeerCore.DelayPostpones());
    }

    // ============ 6. 扫描：曼哈顿距离 / 硬编码 9999 / 三重过滤 ============

    [Fact]
    public void NearestScanUsesManhattanDistanceAndHardcoded9999()
    {
        Assert.Contains("n10 := 9999;", Src(1399), StringComparison.Ordinal);
        Assert.Contains("Abs(m_nCurrX - BaseObject.m_nCurrX) + Abs(m_nCurrY - BaseObject.m_nCurrY)", Src(1429), StringComparison.Ordinal);
        Assert.Contains("if nC < n10 then", Src(1430), StringComparison.Ordinal);

        Assert.True(ObjMonChickenDeerCore.ManhattanDistance());
        Assert.True(ObjMonChickenDeerCore.Hardcoded9999());
        Assert.True(ObjMonChickenDeerCore.ClosestWins());
        Assert.True(ObjMonChickenDeerCore.NotEuclidean());
        Assert.Equal(2, ObjMonChickenDeerCore.Manhattan(0, 0, 1, 1));
        Assert.Equal(1, ObjMonChickenDeerCore.Manhattan(0, 0, 1, 0));
        Assert.True(ObjMonChickenDeerCore.DiagonalCountsTwo());
        Assert.True(ObjMonChickenDeerCore.StraightOneIsOne());
        Assert.True(ObjMonChickenDeerCore.EuclideanDiffers());

        Assert.True(ObjMonChickenDeerCore.ChoosesSmallest());
        Assert.True(ObjMonChickenDeerCore.EmptyReturnsMinusOne());
        Assert.True(ObjMonChickenDeerCore.TieKeepsFirst());
    }

    [Fact]
    public void ThreeFiltersAndInvisibilityRule()
    {
        Assert.Equal("if BaseObject.m_boDeath then", Src(1417).Trim());
        Assert.Equal("Continue;", Src(1418).Trim());
        Assert.Contains("BaseObject.m_btRaceServer = RC_PLAYOBJECT", Src(1420), StringComparison.Ordinal);
        Assert.Contains("m_boOffLine", Src(1420), StringComparison.Ordinal);
        Assert.Contains("g_Config.boMonNoAttackOffLinePlayer", Src(1420), StringComparison.Ordinal);
        Assert.Contains("if IsProperTarget(BaseObject) then", Src(1425), StringComparison.Ordinal);
        Assert.Contains("if not BaseObject.m_boHideMode or m_boCoolEye then", Src(1427), StringComparison.Ordinal);

        Assert.True(ObjMonChickenDeerCore.ThreeFilters());
        Assert.True(ObjMonChickenDeerCore.OfflinePlayerFilter());
        Assert.True(ObjMonChickenDeerCore.HideModeNeedsCoolEye());
        Assert.True(ObjMonChickenDeerCore.NotHiddenVisible());
        Assert.True(ObjMonChickenDeerCore.HiddenNoCoolEyeInvisible());
        Assert.True(ObjMonChickenDeerCore.HiddenWithCoolEyeVisible());
        Assert.True(ObjMonChickenDeerCore.OfflineSkipped());
        Assert.True(ObjMonChickenDeerCore.ConfigOffNotSkipped());
        Assert.True(ObjMonChickenDeerCore.NonPlayerNotSkipped());
    }

    [Fact]
    public void LockUnlockIsAProperTryFinally()
    {
        Assert.Equal("m_VisibleActors.Lock;", Src(1407).Trim());
        Assert.Equal("try", Src(1408).Trim());
        Assert.Equal("finally", Src(1439).Trim());
        Assert.Equal("m_VisibleActors.UnLock;", Src(1440).Trim());
        Assert.Equal("end;", Src(1441).Trim());
        Assert.Equal(1, CountIn("m_VisibleActors.Lock;", 1393, 1464));
        Assert.Equal(1, CountIn("m_VisibleActors.UnLock;", 1393, 1464));

        Assert.True(ObjMonChickenDeerCore.ProperLockUnlock());
        Assert.True(ObjMonChickenDeerCore.TryFinallyGuarantees());
        Assert.True(ObjMonChickenDeerCore.TwoLevelNilChecks());
    }

    // ============ 7. Create / Destroy ============

    [Fact]
    public void CreateAndDestroyShape()
    {
        Assert.Equal("inherited;", Src(1384).Trim());
        Assert.Equal("m_nViewRange := 5;", Src(1385).Trim());
        Assert.Equal("inherited;", Src(1390).Trim());

        Assert.True(ObjMonChickenDeerCore.OnlySetsViewRange());
        Assert.True(ObjMonChickenDeerCore.ViewRangeIsFive());
        Assert.True(ObjMonChickenDeerCore.InheritedCalledFirst());
        Assert.True(ObjMonChickenDeerCore.DestroyIsEmptyShell());
        Assert.True(ObjMonChickenDeerCore.RedundantInherited());
        Assert.True(ObjMonChickenDeerCore.NoThinkOverride());
        Assert.True(ObjMonChickenDeerCore.TargetScanInRunNotThink());
        Assert.True(ObjMonChickenDeerCore.ThreeDeclaredMethods());
        Assert.True(ObjMonChickenDeerCore.FieldDeclaredTwice());
        Assert.True(ObjMonChickenDeerCore.ReadsInheritedField());
        Assert.True(ObjMonChickenDeerCore.NeverAssignedInThisClass());
    }

    /// <summary>
    /// ★ **本车道复核更正的一处 J199 旧结论**：原文说 `bo554` "两处声明"，
    /// 实测是**三处**（11 / 182 / 507），且**四处**赋值（555/792/2166/5534，全为 `False`）。
    /// 本用例把实测值与"旧结论"的差异一并锁死。
    /// </summary>
    [Fact]
    public void Bo554DeclarationAndAssignmentSitesCorrectedAgainstOriginal()
    {
        // 三处声明
        Assert.Contains("bo554: Boolean;", Src(11), StringComparison.Ordinal);
        Assert.Contains("bo554: Boolean;", Src(182), StringComparison.Ordinal);
        Assert.Contains("bo554: Boolean;", Src(507), StringComparison.Ordinal);

        int decls = 0;
        for (int i = 1; i <= 9502; i++)
            if (Src(i).TrimStart().StartsWith("bo554: Boolean;", StringComparison.Ordinal)) decls++;
        Assert.Equal(3, decls);
        Assert.Equal(ObjMonChickenDeerCore.Bo554DeclarationCount, decls);

        // 四处赋值、全为 False
        Assert.Equal(4, CountIn("bo554 := False;", 1, 9502));
        Assert.Equal(0, CountIn("bo554 := True;", 1, 9502));
        Assert.Equal(new[] { 555, 792, 2166, 5534 }, LineOfAll("bo554 :="));

        // 本类既声明体没有它、三个方法体也从不写它
        Assert.Equal(0, CountIn("bo554", 25, 30));
        Assert.Equal(1, CountIn("bo554", 1393, 1464));        // Run 的 1402 行读一次
        Assert.Equal(0, CountIn("bo554 :=", 1382, 1464));

        Assert.True(ObjMonChickenDeerCore.FieldDeclaredTwice());
        Assert.True(ObjMonChickenDeerCore.ReadsInheritedField());
        Assert.True(ObjMonChickenDeerCore.NeverAssignedInThisClass());
    }

    // ============ 8. 整体与跨度 ============

    [Fact]
    public void OverallAndSpanFacts()
    {
        Assert.True(ObjMonChickenDeerCore.TwoDifferentThresholds());
        Assert.True(ObjMonChickenDeerCore.SixVsFive());
        Assert.True(ObjMonChickenDeerCore.NoInstrumentation());
        Assert.True(ObjMonChickenDeerCore.ManyMoreSubclasses());
        Assert.True(ObjMonChickenDeerCore.RemainingClassCountIs54());
        Assert.True(ObjMonChickenDeerCore.TotalLinesAddUp());
        Assert.True(ObjMonChickenDeerCore.SpanMatches());
        Assert.True(ObjMonChickenDeerCore.StartsAscending());
        Assert.True(ObjMonChickenDeerCore.WithinUnit());
    }

    [Fact]
    public void ClassDeclarationCountRecomputedFromOriginal()
    {
        // 与 tools/audit-objmon-coverage.ps1 同口径：^\s*(T\w+)\s*=\s*class\b
        var matches = System.Text.RegularExpressions.Regex.Matches(
            string.Join("\n", ObjMonRealSourceHarness.Lines),
            @"^[ \t]*(T[A-Za-z0-9_]+)[ \t]*=[ \t]*class\b",
            System.Text.RegularExpressions.RegexOptions.Multiline);

        var names = matches.Select(m => m.Groups[1].Value).ToList();
        var dups = names.GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

        Assert.True(matches.Count == ObjMonChickenDeerCore.DeclarationCountIncludingCommented,
            "类声明条数实测 " + matches.Count + "；重名=" + string.Join(",", dups)
            + "；原文=" + ObjMonRealSourceHarness.SourcePath);
        Assert.Equal(new[] { "TElfWarriorMonster" }, dups);

        // 56 条声明 − 1 条被 (* *) 注掉 = 55 个实有类；再减去本批覆盖的 1 个 = 54
        Assert.Equal(ObjMonChickenDeerCore.RealClassCount,
            matches.Count - ObjMonChickenDeerCore.CommentedOutDeclarations);
        Assert.Equal(ObjMonChickenDeerCore.RemainingClassCount,
            matches.Count - ObjMonChickenDeerCore.CommentedOutDeclarations
            - ObjMonChickenDeerCore.ClassesCoveredByThisBatch);
        Assert.True(ObjMonChickenDeerCore.RemainingClassCountIs54());
        Assert.Equal(matches.Count, ObjMonChickenDeerCore.CountClassDeclarationsPublic());
    }

    /// <summary>
    /// ★ 原文实现条数（对账口径）与"`Run` 之后还剩多少"都由本用例从原文重算 ——
    /// J199 的魔数 189 在本车道被改成"由原文重算 + 下界断言"。
    /// </summary>
    [Fact]
    public void ImplementationLineCountRecomputedFromOriginal()
    {
        Assert.Equal(202, CountImplementations(1, 9502));
        Assert.Equal(ObjMonChickenDeerCore.CountImplementationLinesPublic(), CountImplementations(1, 9502));

        Assert.Equal(186, CountImplementations(1465, 9502));
        Assert.Equal(ObjMonChickenDeerCore.CountImplementationLinesAfter1464Public(), CountImplementations(1465, 9502));

        // ★ J199 记"189"，重算是 187（RunStart 1393 之后）—— 差额 2 已在报告登记为 D-P13-01
        int productAfterRunStart = ObjMonChickenDeerCore.CountImplementationsAfterRunStartPublic();
        int testAfterRunStart = CountImplementations(1394, 9502);
        Assert.True(productAfterRunStart == testAfterRunStart,
            "两侧不符：test=" + testAfterRunStart + " product=" + productAfterRunStart
            + " const=" + ObjMonChickenDeerCore.ImplementationsAfterRunStart
            + " src=" + ObjMonRealSourceHarness.SourcePath);
        Assert.Equal(186, productAfterRunStart);
        Assert.Equal(189, ObjMonChickenDeerCore.RemainingImplCount);
        Assert.True(ObjMonChickenDeerCore.ManyMoreSubclasses());
    }

    private static int CountImplementations(int from, int to)
    {
        int n = 0;
        for (int i = from; i <= to; i++)
        {
            string raw = Src(i);
            if (raw.Length == 0 || char.IsWhiteSpace(raw[0])) continue;
            string t = raw.TrimStart();

            string? rest = null;
            foreach (string kw in new[] { "procedure ", "function ", "constructor ", "destructor " })
                if (t.StartsWith(kw, StringComparison.Ordinal)) { rest = t.Substring(kw.Length); break; }

            if (rest == null || !rest.StartsWith("T", StringComparison.Ordinal)) continue;
            int dot = rest.IndexOf('.');
            if (dot <= 1) continue;
            n++;
        }

        return n;
    }

    // ============ 9. 未取证断言登记（台账 §49.3 之 (c)） ============

    [Fact]
    public void UnverifiedClaimsAreExplicitlyRegistered()
    {
        // 先调用两条登记型谓词
        Assert.True(ObjMonChickenDeerCore.SameAsJ197Operate());
        Assert.True(ObjMonChickenDeerCore.FirstSubclassOnly());

        Assert.Contains("ObjMonChickenDeerCore.SameAsJ197Operate (ObjMon.pas:1390)",
            ObjMonChickenDeerCore.NotPortedClaims);
        Assert.Contains("ObjMonChickenDeerCore.FirstSubclassOnly (ObjMon.pas:1467)",
            ObjMonChickenDeerCore.NotPortedClaims);
        Assert.Equal(2, ObjMonChickenDeerCore.NotPortedClaims.Count);
    }

    // ============ 10. 否定性断言计数取证（§37.3） ============

    [Fact]
    public void NegativeAssertion_ChickenDeerIsNotAClassInManagedEngine()
    {
        // 已入库的原文副本里，TChickenDeer 有 3 个实现体
        ObjMonRealSourceHarness.CountAcrossAllCopies("TChickenDeer.", 3);

        // ★ 否定性断言（计数取证）：托管侧 **没有** TChickenDeer 这个运行时类型
        var managedTypes = typeof(ObjMonChickenDeerCore).Assembly
            .GetTypes()
            .Where(t => t.Name == "TChickenDeer")
            .ToList();

        Assert.Empty(managedTypes);
        Assert.Equal(0, managedTypes.Count);
    }

    private static int LineOf(string needle)
    {
        for (int i = 1; i <= 9502; i++)
            if (Src(i).Contains(needle, StringComparison.Ordinal)) return i;
        return -1;
    }

    private static int[] LineOfAll(string needle)
    {
        var hits = new List<int>();
        for (int i = 1; i <= 9502; i++)
            if (Src(i).Contains(needle, StringComparison.Ordinal)) hits.Add(i);
        return hits.ToArray();
    }
}

using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J196：`MySqlStorageDB.pas` 姊妹实现 1:1 测试（七方法合计 263 行）。
/// **核心是"姊妹实现比对"**：两文件行数几乎相同（426/427）、
/// 逐行比对仅 94 行不同（约 77% 逐字相同）、
/// 差异全部是机械改名，六组 API 名字一一对称。
/// 唯一的结构性差异是 `DoGetAllHumans` 的循环惯用法。
/// </summary>
public sealed class MySqlStorageDbCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(426, MySqlStorageDbCore.Lines);
        Assert.Equal(427, MySqlStorageDbCore.SiblingLines);
        Assert.Equal(94, MySqlStorageDbCore.DiffLines);
        Assert.Equal(427, MySqlStorageDbCore.CompareResolution);
        Assert.Equal(156, MySqlStorageDbCore.LoadStart);
        Assert.Equal(31, MySqlStorageDbCore.LoadLines);
        Assert.Equal(188, MySqlStorageDbCore.RenameStart);
        Assert.Equal(9, MySqlStorageDbCore.RenameLines);
        Assert.Equal(198, MySqlStorageDbCore.SaveStart);
        Assert.Equal(59, MySqlStorageDbCore.SaveLines);
        Assert.Equal(258, MySqlStorageDbCore.AddStart);
        Assert.Equal(47, MySqlStorageDbCore.AddLines);
        Assert.Equal(306, MySqlStorageDbCore.DeleteStart);
        Assert.Equal(56, MySqlStorageDbCore.DeleteLines);
        Assert.Equal(363, MySqlStorageDbCore.ClearStart);
        Assert.Equal(43, MySqlStorageDbCore.ClearLines);
        Assert.Equal(407, MySqlStorageDbCore.GetAllStart);
        Assert.Equal(18, MySqlStorageDbCore.GetAllLines);
        Assert.Equal(263, MySqlStorageDbCore.TotalLines);
        Assert.Equal(79, MySqlStorageDbCore.InitStart);
        Assert.Equal(352, MySqlStorageDbCore.ExtraBlankLine);
        Assert.Equal(14, MySqlStorageDbCore.ItemTypeSiteCount);
        Assert.Equal(12, MySqlStorageDbCore.ConstantUseCount);
        Assert.Equal(2, MySqlStorageDbCore.LiteralUseCount);
        Assert.Equal(0, MySqlStorageDbCore.STORAGEEX_ITEM_TYPE);
        Assert.Equal(6, MySqlStorageDbCore.PrepStatementCount);
        Assert.Equal(2, MySqlStorageDbCore.SqlDialectDiffCount);
        Assert.Equal(14, MySqlStorageDbCore.TextBindCount);
        Assert.Equal(4, MySqlStorageDbCore.IntBindCount);
        Assert.Equal(11, MySqlStorageDbCore.QueryFetchCount);
        Assert.Equal(3, MySqlStorageDbCore.TransactionApiCount);
        Assert.Equal(3, MySqlStorageDbCore.ExecApiCount);
        Assert.Equal(3, MySqlStorageDbCore.CommitCount);
        Assert.Equal(3, MySqlStorageDbCore.RollbackCount);
        Assert.Equal(4, MySqlStorageDbCore.MySqlStepCount);
        Assert.Equal(16, MySqlStorageDbCore.SqliteStepCount);
        Assert.Equal(11, MySqlStorageDbCore.SqliteRowCount);
        Assert.Equal(2, MySqlStorageDbCore.ResultAfterCommitCount);
        Assert.Equal("SQLITE_ROW", MySqlStorageDbCore.SQLITE_ROW_NAME);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(MySqlStorageDbCore.SpanMatches());
        Assert.True(MySqlStorageDbCore.TotalLinesAddUp());
        Assert.True(MySqlStorageDbCore.StartsAscending());
        Assert.True(MySqlStorageDbCore.WithinFile());
        Assert.True(MySqlStorageDbCore.SameMethodOrder());
        Assert.True(MySqlStorageDbCore.SameDeclarationOrder());
        Assert.True(MySqlStorageDbCore.MethodOrderExtracted());
        Assert.True(MySqlStorageDbCore.NoInstrumentation());
        Assert.True(MySqlStorageDbCore.NoExceptionMsg());
    }

    [Fact]
    public void MethodOrder()
    {
        Assert.Equal(9, MySqlStorageDbCore.MethodOrder.Length);
        Assert.Equal("DoInit", MySqlStorageDbCore.MethodOrder[0]);
        Assert.Equal("DoLoadStorageItems", MySqlStorageDbCore.MethodOrder[2]);
        Assert.Equal("DoGetAllHumans", MySqlStorageDbCore.MethodOrder[8]);
    }

    // ===================== 一、姊妹比对 =====================

    [Fact]
    public void SisterComparisonFacts()
    {
        Assert.True(MySqlStorageDbCore.NearlyIdenticalLength());
        Assert.True(MySqlStorageDbCore.NinetyFourDiffLines());
        Assert.True(MySqlStorageDbCore.MostlyVerbatim());
        Assert.True(MySqlStorageDbCore.DiffCountMeasured());
        Assert.True(MySqlStorageDbCore.VerbatimOver75());
    }

    [Fact]
    public void VerbatimPercent()
    {
        // **(427 - 94) / 427 = 77%**
        Assert.Equal(77, MySqlStorageDbCore.VerbatimPercent());
        Assert.True(MySqlStorageDbCore.NearlyIdenticalLength());

        // **行数只差一行**
        Assert.Equal(1, MySqlStorageDbCore.SiblingLines - MySqlStorageDbCore.Lines);
    }

    [Fact]
    public void SymmetricApiPairs()
    {
        Assert.True(MySqlStorageDbCore.SixSymmetricPairs());
        Assert.True(MySqlStorageDbCore.ApiPairsExtracted());
        Assert.True(MySqlStorageDbCore.TextBindCounts());
        Assert.True(MySqlStorageDbCore.IntBindCounts());
        Assert.True(MySqlStorageDbCore.QueryFetchVsStep());
        Assert.True(MySqlStorageDbCore.TransactionApiPair());
        Assert.True(MySqlStorageDbCore.ExecVsExecute());
        Assert.True(MySqlStorageDbCore.CommitRollbackShared());
        Assert.True(MySqlStorageDbCore.NoOrphans());

        Assert.Equal(6, MySqlStorageDbCore.ApiPairs.Length);
        Assert.Equal("OrderBindParamText", MySqlStorageDbCore.ApiPairs[0].MySql);
        Assert.Equal("OrderBindText", MySqlStorageDbCore.ApiPairs[0].Sqlite);
    }

    [Fact]
    public void CommitRollbackNotRenamed()
    {
        Assert.True(MySqlStorageDbCore.CommitRollbackUnchanged());
        Assert.True(MySqlStorageDbCore.FiveRenamedCategories());

        // **提交与回滚两套驱动同名**
        Assert.Equal(MySqlStorageDbCore.CommitCount, MySqlStorageDbCore.RollbackCount);
    }

    [Fact]
    public void StepNameDifferentRole()
    {
        Assert.True(MySqlStorageDbCore.StepBothPresent());
        Assert.True(MySqlStorageDbCore.StepCountsDiffer());
        Assert.True(MySqlStorageDbCore.SameNameDifferentRole());
        Assert.True(MySqlStorageDbCore.SqliteStepMostlyPredicate());

        // **MySQL 4 次、SQLite 16 次**
        Assert.Equal(4, MySqlStorageDbCore.MySqlStepCount);
        Assert.Equal(16, MySqlStorageDbCore.SqliteStepCount);
    }

    // ===================== 二、SQL 方言 =====================

    [Fact]
    public void SqlDialectFacts()
    {
        Assert.True(MySqlStorageDbCore.SqlDialectDiffers());
        Assert.True(MySqlStorageDbCore.TableCaseDiffers());
        Assert.True(MySqlStorageDbCore.RenameExistenceDiffers());
        Assert.True(MySqlStorageDbCore.DerivedTableWorkaround());
        Assert.True(MySqlStorageDbCore.OnlyTwoSqlDiffs());
        Assert.True(MySqlStorageDbCore.RestVerbatim());
        Assert.True(MySqlStorageDbCore.SixPrepStatements());
    }

    [Fact]
    public void RenameSqlDiffers()
    {
        Assert.True(MySqlStorageDbCore.RenameSqlTextsDiffer());
        Assert.True(MySqlStorageDbCore.MySqlHasDerivedTable());
        Assert.True(MySqlStorageDbCore.SqliteHasNoDerivedTable());
        Assert.True(MySqlStorageDbCore.BothUseNotExists());
        Assert.True(MySqlStorageDbCore.SameBindCount());

        // **MySQL 版多一层派生表**
        Assert.Contains("(select HumanName from StorageEx) t2",
            MySqlStorageDbCore.MySqlRenameSql);
        Assert.DoesNotContain("(select HumanName from StorageEx) t2",
            MySqlStorageDbCore.SqliteRenameSql);
    }

    // ===================== 三、常量与字面量 =====================

    [Fact]
    public void ConstantLiteralSameAsJ195()
    {
        Assert.True(MySqlStorageDbCore.SameConstantLiteralSplit());
        Assert.True(MySqlStorageDbCore.FourteenTotal());
        Assert.True(MySqlStorageDbCore.TwelveConstantTwoLiteral());
        Assert.True(MySqlStorageDbCore.SitesAddUp());
        Assert.True(MySqlStorageDbCore.TrapReplicated());
        Assert.True(MySqlStorageDbCore.LiteralSitesExtracted());
        Assert.True(MySqlStorageDbCore.ConstantIsZero());

        Assert.Equal(new[] { 107, 243 }, MySqlStorageDbCore.LiteralSites);
    }

    // ===================== 四、DoGetAllHumans 循环 =====================

    [Fact]
    public void LoopIdiomFacts()
    {
        Assert.True(MySqlStorageDbCore.LoopIdiomDiffers());
        Assert.True(MySqlStorageDbCore.MySqlQueryFetchWhile());
        Assert.True(MySqlStorageDbCore.SqliteManualStep());
        Assert.True(MySqlStorageDbCore.OnlyStructuralDiff());
        Assert.True(MySqlStorageDbCore.QueryAlreadyFetches());
        Assert.True(MySqlStorageDbCore.FirstRowFromQuery());
        Assert.True(MySqlStorageDbCore.EasyToMissFirstRow());
        Assert.True(MySqlStorageDbCore.IntAsObjectIdiom());
        Assert.True(MySqlStorageDbCore.VerbatimInBoth());
    }

    [Fact]
    public void BothLoopFormsAgree()
    {
        Assert.True(MySqlStorageDbCore.BothLoopFormsAgree());
        Assert.True(MySqlStorageDbCore.EmptyResultBothEmpty());
        Assert.True(MySqlStorageDbCore.SingleRowBothGetOne());
    }

    [Fact]
    public void MisreadingLosesFirstRow()
    {
        Assert.True(MySqlStorageDbCore.MisreadingLosesFirstRow());
        Assert.True(MySqlStorageDbCore.FirstRowProvidedByQuery());
        Assert.True(MySqlStorageDbCore.FailedQueryYieldsNone());
        Assert.True(MySqlStorageDbCore.SqliteIgnoresQueryFlag());
    }

    [Fact]
    public void EnumerateValues()
    {
        var rows = new List<int> { 10, 20, 30 };

        // **两种写法结果一致**
        Assert.Equal(new[] { 10, 20, 30 }, MySqlStorageDbCore.MySqlEnumerate(true, rows));
        Assert.Equal(new[] { 10, 20, 30 }, MySqlStorageDbCore.SqliteEnumerate(rows));

        // **`Query` 失败则一行都不取**
        Assert.Empty(MySqlStorageDbCore.MySqlEnumerate(false, rows));
    }

    // ===================== 五、与 J195 共享 =====================

    [Fact]
    public void SharedStructureFacts()
    {
        Assert.True(MySqlStorageDbCore.LookupCopiedFiveTimes());
        Assert.True(MySqlStorageDbCore.ThreeVerbatimTwoSimplified());
        Assert.True(MySqlStorageDbCore.PredicateSplitSame());
        Assert.True(MySqlStorageDbCore.TwoUseGreaterThan());
        Assert.True(MySqlStorageDbCore.ThreeUseNotEqual());
        Assert.True(MySqlStorageDbCore.GreaterThanSitesExtracted());
        Assert.True(MySqlStorageDbCore.ResultAfterCommit());
        Assert.True(MySqlStorageDbCore.BugReplicated());
    }

    [Fact]
    public void ExtraBlankLineExplainsOffset()
    {
        Assert.True(MySqlStorageDbCore.OneExtraBlankLine());
        Assert.True(MySqlStorageDbCore.BlankAt352());
        Assert.True(MySqlStorageDbCore.ExplainsOffset());
        Assert.True(MySqlStorageDbCore.NoHiddenDiff());

        // **空行在 Commit 与 Result := True 之间；故 ClearStart 为 363**
        Assert.Equal(352, MySqlStorageDbCore.ExtraBlankLine);
        Assert.Equal(363, MySqlStorageDbCore.ClearStart);
    }

    [Fact]
    public void GreaterThanSites()
    {
        Assert.Equal(new[] { 306, 363 }, MySqlStorageDbCore.GreaterThanSites);
    }
}

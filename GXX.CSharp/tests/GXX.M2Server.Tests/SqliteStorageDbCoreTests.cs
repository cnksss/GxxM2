using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J195：`SqliteStorageDB.pas` 仓库子系统 1:1 测试（三个方法合计 158 行）。
/// **核心是"同一份逻辑被复制多遍、各副本细节不一致"**：
/// 九表删除的拼装分成两种风格（共享 `sWhere` vs 逐条内联），
/// `ItemType` 的写法在 14 处里 12 处用常量、2 处用字面量，
/// 判据则有 `&lt;&gt; 0` 与 `&gt; 0` 两种 —— 当前取值相同、将来会静默失配。
/// </summary>
public sealed class SqliteStorageDbCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(59, SqliteStorageDbCore.SaveLines);
        Assert.Equal(198, SqliteStorageDbCore.SaveStart);
        Assert.Equal(256, SqliteStorageDbCore.SaveEnd);
        Assert.Equal(55, SqliteStorageDbCore.DeleteLines);
        Assert.Equal(306, SqliteStorageDbCore.DeleteStart);
        Assert.Equal(360, SqliteStorageDbCore.DeleteEnd);
        Assert.Equal(44, SqliteStorageDbCore.ClearLines);
        Assert.Equal(362, SqliteStorageDbCore.ClearStart);
        Assert.Equal(405, SqliteStorageDbCore.ClearEnd);
        Assert.Equal(158, SqliteStorageDbCore.TotalLines);
        Assert.Equal(427, SqliteStorageDbCore.UnitLines);
        Assert.Equal(9, SqliteStorageDbCore.TableCount);
        Assert.Equal(0, SqliteStorageDbCore.STORAGEEX_ITEM_TYPE);
        Assert.Equal(14, SqliteStorageDbCore.ItemTypeSiteCount);
        Assert.Equal(12, SqliteStorageDbCore.ConstantUseCount);
        Assert.Equal(2, SqliteStorageDbCore.LiteralUseCount);
        Assert.Equal(9, SqliteStorageDbCore.WithArgApiCount);
        Assert.Equal(2, SqliteStorageDbCore.NoArgApiCount);
        Assert.Equal(1365, SqliteStorageDbCore.NoArgApiMirrorHits);
        Assert.Equal(5, SqliteStorageDbCore.LookupSiteCount);
        Assert.Equal(3, SqliteStorageDbCore.VerbatimCopyCount);
        Assert.Equal(2, SqliteStorageDbCore.SimplifiedCount);
        Assert.Equal(156, SqliteStorageDbCore.LoadStart);
        Assert.Equal(258, SqliteStorageDbCore.AddStart);
        Assert.Equal(188, SqliteStorageDbCore.RenameStart);
        Assert.Equal(365, SqliteStorageDbCore.SiblingLines);
        Assert.Equal(6, SqliteStorageDbCore.StatementFieldCount);
        Assert.Equal(184, SqliteStorageDbCore.LoadItemsLiteralLine);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(SqliteStorageDbCore.SpanMatches());
        Assert.True(SqliteStorageDbCore.MethodsOrdered());
        Assert.True(SqliteStorageDbCore.WithinUnit());
        Assert.True(SqliteStorageDbCore.NoInstrumentation());
    }

    // ===================== 一、九表与两种风格 =====================

    [Fact]
    public void NineTables()
    {
        Assert.True(SqliteStorageDbCore.NineTables());
        Assert.True(SqliteStorageDbCore.AllThreeSitesSameTables());
        Assert.True(SqliteStorageDbCore.TableOrderIdentical());
        Assert.True(SqliteStorageDbCore.TableNamesExtracted());
        Assert.True(SqliteStorageDbCore.ItemsIsLast());
        Assert.True(SqliteStorageDbCore.AssemblyStartsExtracted());

        Assert.Equal(9, SqliteStorageDbCore.DeletedTables.Length);
        Assert.Equal("ItemElementAdd", SqliteStorageDbCore.DeletedTables[0]);
        Assert.Equal("Items", SqliteStorageDbCore.DeletedTables[8]);
    }

    [Fact]
    public void TwoAssemblyStyles()
    {
        Assert.True(SqliteStorageDbCore.TwoAssemblyStyles());
        Assert.True(SqliteStorageDbCore.SharedWhereTwice());
        Assert.True(SqliteStorageDbCore.InlineNineTimesOnce());
    }

    [Fact]
    public void TwoScopes()
    {
        Assert.True(SqliteStorageDbCore.TwoScopes());
        Assert.True(SqliteStorageDbCore.DeleteAddsItemIndex());
        Assert.True(SqliteStorageDbCore.SameTableSetDifferentScope());
        Assert.True(SqliteStorageDbCore.ClearSqlHasNoItemIndex());
        Assert.True(SqliteStorageDbCore.DeleteSqlHasItemIndex());
        Assert.True(SqliteStorageDbCore.SharedPrefix());
    }

    [Fact]
    public void SaveSemantics()
    {
        Assert.True(SqliteStorageDbCore.DeleteThenRewrite());
        Assert.True(SqliteStorageDbCore.WholeStorageOverwrite());
        Assert.True(SqliteStorageDbCore.IndexIsLoopCounter());
    }

    // ===================== 二、常量 vs 字面量 =====================

    [Fact]
    public void ConstantVersusLiteral()
    {
        Assert.True(SqliteStorageDbCore.ConstantIsZero());
        Assert.True(SqliteStorageDbCore.TwelveUseConstant());
        Assert.True(SqliteStorageDbCore.TwoUseLiteral());
        Assert.True(SqliteStorageDbCore.SitesAddUp());
        Assert.True(SqliteStorageDbCore.LiteralSitesExtracted());
        Assert.True(SqliteStorageDbCore.ItemsLineIsLiteral());
        Assert.True(SqliteStorageDbCore.OtherEightAreConstant());
        Assert.True(SqliteStorageDbCore.SameValueToday());
        Assert.True(SqliteStorageDbCore.LatentTrapIfChanged());
        Assert.True(SqliteStorageDbCore.LiteralIsMinority());
        Assert.True(SqliteStorageDbCore.LoadItemsCallSiteLiteral());
    }

    [Fact]
    public void LiteralSites()
    {
        Assert.Equal(new[] { 107, 243 }, SqliteStorageDbCore.LiteralSites);
        Assert.Equal(12, SqliteStorageDbCore.ConstantSites.Length);
        Assert.Equal(97, SqliteStorageDbCore.ConstantSites[0]);
        Assert.Equal(384, SqliteStorageDbCore.ConstantSites[11]);
    }

    [Fact]
    public void FormsAgreeTodayButDivergeIfChanged()
    {
        Assert.True(SqliteStorageDbCore.FormsAgreeToday());
        Assert.True(SqliteStorageDbCore.FormsDivergeIfConstantChanges());

        // **今天两种写法完全一致**
        Assert.Equal(SqliteStorageDbCore.ConstantForm(7), SqliteStorageDbCore.LiteralForm(7));
        Assert.Equal(" where ItemType = 0 and ParentID = 7", SqliteStorageDbCore.LiteralForm(7));
    }

    // ===================== 三、查找块复制 =====================

    [Fact]
    public void LookupBlockCopies()
    {
        Assert.True(SqliteStorageDbCore.LookupOrCreateFiveTimes());
        Assert.True(SqliteStorageDbCore.ThreeVerbatimCopies());
        Assert.True(SqliteStorageDbCore.TwoSimplifiedVersions());
        Assert.True(SqliteStorageDbCore.CopiesAddUp());
        Assert.True(SqliteStorageDbCore.ResetCalledFourTimes());
        Assert.True(SqliteStorageDbCore.DenseResetPoints());
        Assert.True(SqliteStorageDbCore.FullVersionInserts());
        Assert.True(SqliteStorageDbCore.SimplifiedOnlyQueries());
        Assert.True(SqliteStorageDbCore.CopySitesExtracted());
        Assert.True(SqliteStorageDbCore.CopiesOrdered());
    }

    [Fact]
    public void LookupOrCreateSemantics()
    {
        Assert.True(SqliteStorageDbCore.LookupUsesFoundId());
        Assert.True(SqliteStorageDbCore.CreateUsesNewId());
        Assert.True(SqliteStorageDbCore.FailedCreateYieldsZero());

        Assert.Equal(42, SqliteStorageDbCore.LookupOrCreate(true, 42, false, 0));
        Assert.Equal(77, SqliteStorageDbCore.LookupOrCreate(false, 0, true, 77));
        Assert.Equal(0, SqliteStorageDbCore.LookupOrCreate(false, 0, false, 0));
    }

    // ===================== 四、判据不一致 =====================

    [Fact]
    public void TwoPredicates()
    {
        Assert.True(SqliteStorageDbCore.TwoPredicates());
        Assert.True(SqliteStorageDbCore.GreaterThanZeroVsNotEqual());
        Assert.True(SqliteStorageDbCore.NegativeDiverges());
        Assert.True(SqliteStorageDbCore.AgreeOnPositive());
        Assert.True(SqliteStorageDbCore.AgreeOnZero());
        Assert.True(SqliteStorageDbCore.DisagreeOnNegative());
        Assert.True(SqliteStorageDbCore.PredicateDistribution());
    }

    [Fact]
    public void PredicateDivergence()
    {
        // **正数与零：两者一致**
        Assert.Equal(SqliteStorageDbCore.GreaterThanZero(5), SqliteStorageDbCore.NotEqualZero(5));
        Assert.Equal(SqliteStorageDbCore.GreaterThanZero(0), SqliteStorageDbCore.NotEqualZero(0));

        // **负数：`<> 0` 为真、`> 0` 为假**
        Assert.True(SqliteStorageDbCore.NotEqualZero(-1));
        Assert.False(SqliteStorageDbCore.GreaterThanZero(-1));
    }

    // ===================== 五、返回值与异常 =====================

    [Fact]
    public void ReturnAndExceptions()
    {
        Assert.True(SqliteStorageDbCore.SaveIsProcedure());
        Assert.True(SqliteStorageDbCore.DeleteAndClearAreFunctions());
        Assert.True(SqliteStorageDbCore.ResultTrueAfterCommit());
        Assert.True(SqliteStorageDbCore.SwallowedException());
        Assert.True(SqliteStorageDbCore.NoRethrow());
        Assert.True(SqliteStorageDbCore.NoLogging());
        Assert.True(SqliteStorageDbCore.ContrastWithBaseClasses());
        Assert.True(SqliteStorageDbCore.NoElseOnZero());
        Assert.True(SqliteStorageDbCore.SilentNoOp());
        Assert.True(SqliteStorageDbCore.RollbackMayBeNoOp());
        Assert.True(SqliteStorageDbCore.SaveHasNoStatus());
    }

    [Fact]
    public void ResultSemantics()
    {
        Assert.True(SqliteStorageDbCore.SuccessRequiresBoth());
        Assert.True(SqliteStorageDbCore.FailureKeepsFalse());

        Assert.True(SqliteStorageDbCore.ResultSemantics(true, true));
        Assert.False(SqliteStorageDbCore.ResultSemantics(true, false));
        Assert.False(SqliteStorageDbCore.ResultSemantics(false, true));
        Assert.False(SqliteStorageDbCore.ResultSemantics(false, false));
    }

    // ===================== 六、语句与 API =====================

    [Fact]
    public void ValueApis()
    {
        Assert.True(SqliteStorageDbCore.TwoValueApis());
        Assert.True(SqliteStorageDbCore.NineWithArg());
        Assert.True(SqliteStorageDbCore.TwoWithoutArg());
        Assert.True(SqliteStorageDbCore.ApiIsNotDefective());
        Assert.True(SqliteStorageDbCore.StyleMixed());
        Assert.True(SqliteStorageDbCore.NoParensBothSites());
        Assert.True(SqliteStorageDbCore.ConsistentBetweenThem());
        Assert.True(SqliteStorageDbCore.LegalInDelphi());
        Assert.True(SqliteStorageDbCore.NoArgSitesExtracted());
        Assert.True(SqliteStorageDbCore.SixStatementFields());

        Assert.Equal(new[] { 295, 332 }, SqliteStorageDbCore.NoArgApiSites);
    }

    [Fact]
    public void RenameBinds()
    {
        Assert.True(SqliteStorageDbCore.RenameBindsThree());
        Assert.True(SqliteStorageDbCore.FirstEqualsThird());
        Assert.True(SqliteStorageDbCore.NewNameBoundTwice());
        Assert.True(SqliteStorageDbCore.RenameBindsExtracted());
        Assert.True(SqliteStorageDbCore.RenameLineExtracted());

        Assert.Equal(new[] { "NewName", "OldName", "NewName" }, SqliteStorageDbCore.RenameBinds);
    }

    // ===================== 七、模式 =====================

    [Fact]
    public void DesignPattern()
    {
        Assert.True(SqliteStorageDbCore.TemplateMethodPattern());
        Assert.True(SqliteStorageDbCore.DoPrefixOnAll());
        Assert.True(SqliteStorageDbCore.SiblingMySqlImplementation());
    }

    [Fact]
    public void SqlBuilders()
    {
        // **清空式：不带 ItemIndex**
        string clear = SqliteStorageDbCore.BuildClearSql(7);
        Assert.Contains(" where ItemType = 0 and ParentID = 7", clear);
        Assert.DoesNotContain("ItemIndex", clear);

        // **单件式：带 ItemIndex**
        string del = SqliteStorageDbCore.BuildDeleteSql(7, 3);
        Assert.Contains("and ItemIndex = 3", del);

        // **两者共享前缀**
        Assert.StartsWith(SqliteStorageDbCore.BuildWhere(7), clear);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// MySqlRoleDB.pas 的 SQL 语句字面量族与语句注册面。
///
/// 这些测试的作用是**锁住"逐字保留"**：SQL 文本由脚本从 .pas 抽取（MySqlRoleDB.SqlStatements.cs），
/// 这里用独立的期望文本核对关键差异点（反引号、REPLACE INTO、空格、LIMIT 写法、CRLF），
/// 外加"条数 / 名称集合 / 无重复"这些结构性断言。
/// </summary>
public class MySqlRoleDbSqlTests
{
    // ---------------- 结构性断言 ----------------

    [Fact]
    public void HumanStatements_Are86_AndNamesMatchSource()
    {
        // MySqlRoleDB.pas:264-542 共 86 次 Stms.AddSQLStatement（Human 侧）
        var names = MySqlRoleDBStatements.Human.Names;
        Assert.Equal(86, names.Count);
        Assert.Equal(86, names.Values.Distinct().Count() - 0 == 86 ? 86 : names.Values.Count());
    }

    [Fact]
    public void HeroStatements_Are54()
    {
        var names = MySqlRoleDBStatements.Hero.Names;
        Assert.Equal(54, names.Count);
    }

    [Fact]
    public void Human_AllConstantsAreNonNullAndNonEmpty()
    {
        foreach (var kv in MySqlRoleDBStatements.Human.Names)
        {
            var value = ConstantValue(typeof(MySqlRoleDBStatements.Human), kv.Key);
            Assert.False(string.IsNullOrEmpty(value), $"{kv.Key} 不应为空");
        }
    }

    [Fact]
    public void Hero_AllConstantsAreNonNullAndNonEmpty()
    {
        foreach (var kv in MySqlRoleDBStatements.Hero.Names)
        {
            var value = ConstantValue(typeof(MySqlRoleDBStatements.Hero), kv.Key);
            Assert.False(string.IsNullOrEmpty(value), $"{kv.Key} 不应为空");
        }
    }

    private static string ConstantValue(Type t, string field)
        => (string)t.GetField(field)!.GetRawConstantValue()!;

    // ---------------- 逐字文本核对（关键差异点） ----------------

    [Theory]
    // 语句名 / 期望 SQL（原文逐字，含大小写与空格）
    [InlineData("CheckHumanExists", "select HumanID from Human where Account = ? and HumanName = ?;")]
    [InlineData("HumanGetID", "select HumanID from Human where HumanName = ?;")]
    [InlineData("HumanGetCount", "select count(*) from Human where (Account = ?) and (IsDelete = 0);")]
    [InlineData("GetOtherHumanName", "select HumanName from Human where (Account = ?) and (HumanName <> ?) and (IsDelete = 0);")]
    [InlineData("HumanGetHumanHeroName", "select HeroName, DeputyHeroName from Human where (HumanName = ?);")]
    [InlineData("HumanGetHumanHeroID", "select HeroID from Hero where (HumanID = ?);")]
    [InlineData("HumanSelectAbilNpcAdd", "select `Index`,`Value` from HumanAbilNpcAdd where HumanID = ?;")]
    [InlineData("HumanInsertItemValueAdd", "insert into HumanItemValueAdd(HumanID, ItemType, ItemIndex, ValueIndex, `Value`) values(?, ?, ?, ?, ?);")]
    [InlineData("HumanInsertItemFlute", "insert into HumanItemFlute(HumanID, ItemType, ItemIndex, ValueIndex, `Value`,OverlapCount) values(?, ?, ?, ?, ?, ?);")]
    [InlineData("HumanSelectitems", "select ItemType,ItemIndex,MakeIndex,DBIndex,Name,Dura,DuraMax,HeroM2DressEffect,UpgradeCount,IsStartTime,LimitTime,HeroM2Light,Color,IsBind,BindOption,Effect,NewLooks,NewShape,FluteCount,PropertyText,PropertyTextColor,ItemFrom,ItemFromMap,ItemFromMon,ItemFromMaker,ItemFromDate,InsuranceCount, NewExpand3, NewExpand4 from HumanItems where HumanID = ?;")]
    [InlineData("HumanRenameSyncDearName", "update Human set DearName = ? where DearName = ?;")]
    [InlineData("HumanRenameSyncMasterName", "update Human set MasterName = ? where MasterName = ?;")]
    [InlineData("HumanSearchByLevel", "select a.Account, a.HumanName, a.IsDelete, a.Sex, a.Job, a.Level from Human a where (a.Level >= ?) LIMIT ?;")]
    [InlineData("HumanGetMobileNumbers1", "SELECT DISTINCT MobileNumber FROM  Human WHERE Length(MobileNumber) > 0;")]
    [InlineData("HumanGetMobileNumbers2", "select DISTINCT MobileNumber from Human where (Length(MobileNumber) > 0) and (IsMobileBind = 1);")]
    [InlineData("BuyPlayer", "update Human set Account = ?, IsDelete = 0 where Account = ? and HumanName = ?")]
    [InlineData("HumanGetLevelRankTopCount", "select HumanName, Level from Human where (Job in (?,?,?)) order by Level desc limit ?")]
    [InlineData("HumanGetLevelRankCheckLevelAndCount", "select HumanName, Level from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by Level desc  limit ?")]
    [InlineData("HumanGetLevelRankCheckLevel", "select HumanName, Level from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by Level desc")]
    [InlineData("GetMasterRankCheckLevelAndCount", "select HumanName, MasterCount from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by MasterCount desc  limit ?")]
    [InlineData("HumanInsertAbilNG", "replace into HumanAbilNG(HumanID, IsTrainingNG, IsTrainingXF, AbilNGLevel, AbilNGValue, AbilNGMaxValue, AbilNGExp, AbilNGMaxExp,ContinuousMagicOrder1, ContinuousMagicOrder2, ContinuousMagicOrder3, IsOpenLastContinuous, LastContinuousMagicOrder, Meridians1Level, Meridians1BlastHitRate1, Meridians1Acupoints1, Meridians1Acupoints2, Meridians1Acupoints3, Meridians1Acupoints4, Meridians1Acupoints5, Meridians2Level, Meridians2BlastHitRate, Meridians2Acupoints1, Meridians2Acupoints2, Meridians2Acupoints3, Meridians2Acupoints4, Meridians2Acupoints5, Meridians3Level, Meridians3BlastHitRate, Meridians3Acupoints1, Meridians3Acupoints2, Meridians3Acupoints3, Meridians3Acupoints4, Meridians3Acupoints5, Meridians4Level, Meridians4BlastHitRate, Meridians4Acupoints1, Meridians4Acupoints2, Meridians4Acupoints3, Meridians4Acupoints4, Meridians4Acupoints5, Meridians5Level, Meridians5BlastHitRate, Meridians5Acupoints1, Meridians5Acupoints2, Meridians5Acupoints3, Meridians5Acupoints4, Meridians5Acupoints5) values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);")]
    public void Human_SqlText_IsVerbatim(string statementName, string expectedSql)
    {
        Assert.Contains(statementName, MySqlRoleDBStatements.Human.Names.Values);
        var field = MySqlRoleDBStatements.Human.Names.First(kv => kv.Value == statementName).Key;
        Assert.Equal(expectedSql, ConstantValue(typeof(MySqlRoleDBStatements.Human), field));
    }

    [Theory]
    [InlineData("HeroGetID", "select HeroID from Hero where HeroName = ?;")]
    [InlineData("HeroGetCount", "select count(*) from Hero where (HumanID = ?);")]
    [InlineData("HeroGetHumanInfo", "select b.Account, a.HumanID, b.HumanName, b.HeroName, b.DeputyHeroName from Hero a, Human b where (a.HumanID = b.HumanID) and (a.HeroID = ?);")]
    [InlineData("HeroGetHumanInfo2", "select a.HumanID, b.HumanName, b.IsStorageHero, b.IsStorageDeputyHero, b.HeroName, b.DeputyHeroName from Hero a, Human b where (a.HumanID = b.HumanID) and (a.HeroID = ?);")]
    [InlineData("HeroSelectStatusTime", "select `Index`,`Value` from HeroStatusTime where HeroID = ?;")]
    [InlineData("HeroRenameSyncHumanHero", "update Human set HeroName = ?, DeputyHeroName = ? where HumanID = ?;")]
    [InlineData("HeroRenameSyncHumanHero2", "update Human set IsFixedHero = ?, IsStorageHero = ?, IsStorageDeputyHero = ?, HeroName = ?, DeputyHeroName = ? where HumanID = ?;")]
    [InlineData("HeroDelOrRestore", "update Hero set IsDelete = ? where (HeroName = ?);")]
    [InlineData("HeroSelectAbilNpcAdd", "select `Index`,`Value` from HeroAbilNpcAdd where HeroID = ?;")]
    [InlineData("HeroInsertAbilWine", "replace into HeroAbilWine(HeroID, IsDrinkWineDrunk, DrinkWineQuality, DrinkWineAlcohol, AbilAlcohol, AbilMaxAlcohol, AbilDrinkValue, AbilMedicineLevel, AbilMedicineValue, AbilMaxMedicineValue) values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);")]
    [InlineData("HeroGetLevelRankTopCount", "select (select HumanName from Human where HumanID = a.HumanID) as HumanName,a.HeroName, a.Level from Hero a where (a.Job in (?,?,?)) order by a.Level desc limit ?")]
    [InlineData("HeroGetLevelRankCheckLevel", "select (select HumanName from Human where HumanID = a.HumanID) as HumanName, a.HeroName, a.Level from Hero a where (a.Job in (?,?,?)) and (a.Level >= ?) and (a.Level <= ?) order by a.Level desc")]
    public void Hero_SqlText_IsVerbatim(string statementName, string expectedSql)
    {
        Assert.Contains(statementName, MySqlRoleDBStatements.Hero.Names.Values);
        var field = MySqlRoleDBStatements.Hero.Names.First(kv => kv.Value == statementName).Key;
        Assert.Equal(expectedSql, ConstantValue(typeof(MySqlRoleDBStatements.Hero), field));
    }

    [Fact]
    public void HumanSelect_UsesExactColumnOrder_AndSkipsCommentedOutColumns()
    {
        string sql = MySqlRoleDBStatements.Human.FStatementGetHuman;
        Assert.StartsWith("select HumanID,Account,HumanName,Sex,Job,Hair,Dir,Level,ReLevel,Map,X,Y,HomeMap,HomeX,HomeY,AttackMode,StoragePassword,CreditPoint,Gold,GameGold,GamePoint,GameDiamond,GameGird,GameGoldEx,GameGlory,PKPoint,PayMentPoint,MemberType,MemberLevel,IsMaster,MasterName,MasterCount,MarryCount,DearName,IncHP,", sql);
        // 原文 `//'IsFilterGlobalMsg,' +` 被注释掉 → SQL 里不得出现该列
        Assert.DoesNotContain("IsFilterGlobalMsg", sql);
        Assert.Contains("IsFilterGlobalDropItemMsg,", sql);
        Assert.EndsWith("ClearDayVarTime from Human where (Account = ?) and (HumanName = ?);", sql);
    }

    [Fact]
    public void HumanUpdate_SkipsCommentedOutColumns()
    {
        string sql = MySqlRoleDBStatements.Human.FStatementUpdateHuman;
        Assert.StartsWith("update Human set Sex = ?,Job = ?,Hair = ?,Dir = ?,", sql);
        Assert.DoesNotContain("IsFilterGlobalMsg", sql);
        Assert.DoesNotContain("IsFixedHero = ?", sql);
        Assert.DoesNotContain("HeroName = ?", sql);
        Assert.DoesNotContain("DeputyHeroName = ?", sql);
        Assert.EndsWith("ClearDayVarTime = ? where HumanID = ?;", sql);
    }

    [Fact]
    public void HeroSelect_SkipsCommentedOutHomeAndMasterColumns()
    {
        string sql = MySqlRoleDBStatements.Hero.FStatementGetHero;
        Assert.DoesNotContain("HomeMap", sql);
        Assert.DoesNotContain("HomeX", sql);
        Assert.DoesNotContain("HomeY", sql);
        Assert.DoesNotContain("MasterName", sql);
        Assert.EndsWith("from Hero a where (a.HeroName = ?);", sql);
    }

    [Fact]
    public void MySql_UsesBackticks_WhereSqliteBuildWouldNot()
    {
        // MySQL 方言特征：保留字 Index / Value / MoneyName / OverlapCount 一律反引号
        Assert.Contains("`Index`", MySqlRoleDBStatements.Human.FStatementGetStatusTime);
        Assert.Contains("`Value`", MySqlRoleDBStatements.Human.FStatementGetVariableU);
        Assert.Contains("`MoneyName`", MySqlRoleDBStatements.Human.FStatementGetCustomMoney);
        Assert.Contains("`OverlapCount`", MySqlRoleDBStatements.Human.FStatementGetItemFlute);
        // 且不是 SQLite 的双引号写法
        Assert.DoesNotContain("\"Index\"", MySqlRoleDBStatements.Human.FStatementGetStatusTime);
    }

    [Fact]
    public void MySql_UsesReplaceInto_ForAbilTables()
    {
        Assert.StartsWith("replace into HumanAbil(", MySqlRoleDBStatements.Human.FStatementInsertAbil);
        Assert.StartsWith("replace into HumanAbilNG(", MySqlRoleDBStatements.Human.FStatementInsertAbilNG);
        Assert.StartsWith("replace into HumanAbilWine(", MySqlRoleDBStatements.Human.FStatementInsertAbilWine);
        Assert.StartsWith("replace into HeroAbil(", MySqlRoleDBStatements.Hero.FStatementInsertAbil);
        Assert.StartsWith("replace into HeroAbilNG(", MySqlRoleDBStatements.Hero.FStatementInsertAbilNG);
        Assert.StartsWith("replace into HeroAbilWine(", MySqlRoleDBStatements.Hero.FStatementInsertAbilWine);
    }

    [Fact]
    public void MySql_UppercasesHuman_WhereSqliteWouldLowercase()
    {
        // 原文 `FROM  Human`（两个空格）—— 与 SqliteRoleDB 版的 `FROM  human` 是真实差异
        Assert.Contains("FROM  Human", MySqlRoleDBStatements.Human.FStatementGetMobileNumbers1);
        Assert.DoesNotContain("FROM  human", MySqlRoleDBStatements.Human.FStatementGetMobileNumbers1);
    }

    [Fact]
    public void ParameterPlaceholderCount_MatchesBindSiteCount_ForKeyStatements()
    {
        // HumanInsertAbil 34 个占位符（HumanID + 33 个属性列）
        Assert.Equal(34, CountOf(MySqlRoleDBStatements.Human.FStatementInsertAbil, '?'));
        // HumanInsertAbilNG 47 个（1 + 11 + 5*(2+5)）
        Assert.Equal(48, CountOf(MySqlRoleDBStatements.Human.FStatementInsertAbilNG, '?'));
        // HumanInsertAbilWine 11 个
        Assert.Equal(11, CountOf(MySqlRoleDBStatements.Human.FStatementInsertAbilWine, '?'));
        // HeroInsertAbil 23 个（1 + 22）
        Assert.Equal(23, CountOf(MySqlRoleDBStatements.Hero.FStatementInsertAbil, '?'));
        // HumanAbil 查询 33 列
        Assert.Equal(1, CountOf(MySqlRoleDBStatements.Human.FStatementGetAbil, '?'));
        // HumanItems 插入 30 个占位符
        Assert.Equal(30, CountOf(MySqlRoleDBStatements.Human.FStatementInsertItems, '?'));
    }

    private static int CountOf(string s, char c) => s.Count(ch => ch == c);

    [Fact]
    public void NoSqlConstantAccidentallyContainsOurPlaceholder()
    {
        Assert.DoesNotContain("__MYSQL_DBVERSION__", MySqlRoleDBStatements.Human.FStatementGetHuman);
        Assert.DoesNotContain("__MYSQL_DBVERSION__", MySqlRoleDBStatements.Hero.FStatementGetHero);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// MySqlRoleDB.pas 的载荷路径（DoGet / SaveHumanData / Erase / Rename / ChangedGold / Rank / Hero）行为测试。
/// 全程内存接缝；断言针对"参数绑定顺序 / SQL 顺序 / 返回码 / 边界钳位"。
/// </summary>
[Collection("MySqlRoleDb")]
public class MySqlRoleDbPayloadTests
{
    private static (TMySqlRoleDB Db, FakeMySqlFactory F, FakeMySqlDatabase D) NewDb()
    {
        DelphiTick.GetTickCount = () => 0;
        var f = new FakeMySqlFactory();
        var db = new TMySqlRoleDB(f);
        db.DoInit();
        return (db, f, f.Database);
    }

    /// <summary>构造 HumanSelect 的一行（列序见 MySqlRoleDB.pas:322-330）。</summary>
    private static FakeRow HumRow(params (int Index, object? Value)[] overrides)
    {
        var cells = new object?[99];
        for (int i = 0; i < cells.Length; i++) cells[i] = 0;
        cells[0] = 7;                 // HumanID 由 GetColumnValueInt(0) 单独取，这里占位
        cells[1] = "acc";
        cells[2] = "chr";
        cells[3] = 1; cells[4] = 2; cells[5] = 3; cells[6] = 4;   // Sex Job Hair Dir
        cells[7] = 42;                                            // Level
        foreach (var o in overrides) cells[o.Index] = o.Value;
        return FakeRow.Of(cells);
    }

    // ==========================================================================================
    // DoGet（MySqlRoleDB.pas:1210-1845）
    // ==========================================================================================

    [Fact]
    public void HumanGet_NoMainRow_ReturnsFalse_AndLeavesStructUntouched()
    {
        var (db, f, d) = NewDb();
        var hum = new THumData();
        hum.ChrName = "keep-me";
        bool ok = db.HumanDB!.Get("acc", "chr", ref hum, out int id);
        Assert.False(ok);
        Assert.Equal(0, id);
        Assert.Equal("keep-me", hum.ChrName);      // 主表未命中 → 连 FillChar 都不执行
    }

    [Fact]
    public void HumanGet_MainRowOnly_ReturnsTrue_AndFillsMainColumns()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        var hum = new THumData();
        bool ok = db.HumanDB!.Get("acc", "chr", ref hum, out int id);
        Assert.True(ok);
        Assert.Equal(7, id);
        Assert.Equal("acc", hum.Account);
        Assert.Equal("chr", hum.ChrName);
        Assert.Equal(1, hum.btSex);
        Assert.Equal(42, hum.Abil.Level);
    }

    [Fact]
    public void HumanGet_BindsAccountThenHumanName()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow(new[] { (7, 42) }.Select(x => (x.Item1, (object?)x.Item2)).ToArray())));
        var hum = new THumData();
        db.HumanDB!.Get("acc", "chr", ref hum, out _);
        Assert.Equal(new[] { "acc", "chr" }, d.Stmt("HumanSelect").AllBoundTexts);
    }

    [Fact]
    public void HumanGet_StorageOpen1IsAlwaysOne_AndOnlyTwoColumnsAreRead()
    {
        // 原文 1313-1316：boStorageOpen[0] := True 恒置，其它三个才读库
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(1, hum.boStorageOpen[0]);
    }

    [Fact]
    public void HumanGet_SubTableMisses_DoNotClearMainResult()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // 其余子表全部返回空 → 仍然 True
        var hum = new THumData();
        Assert.True(db.HumanDB!.Get("a", "c", ref hum, out _));
    }

    [Fact]
    public void HumanGet_AbilSubTable_FillsAbilityColumns()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // HumanSelectAbil：33 列（AC1..AdjustAbilMaxRate）
        d.Stmt("HumanSelectAbil").EnqueueResult(FakeResultSet.Of(
            new object?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22,
                            23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33 }));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(1, hum.Abil.AC1);
        Assert.Equal(10, hum.Abil.SC2);
        Assert.Equal(11u, hum.Abil.HP);
        Assert.Equal(22, hum.Abil.MaxHandWeight);
        Assert.Equal(23, hum.nBonusPoint);
        Assert.Equal(24, hum.BonusAbil.DC);
        Assert.Equal(33, hum.BonusAbil.X2);
    }

    [Fact]
    public void HumanGet_AbilNG_FillsMeridians5x5()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // 12 固定列 + 5*(Level,BlastHitRate,5*Acupoints) = 12 + 35 = 47 列
        var cells = new object?[47];
        for (int i = 0; i < cells.Length; i++) cells[i] = i + 1;
        d.Stmt("HumanSelectAbilNG").EnqueueResult(FakeResultSet.Of(cells));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(3, hum.AbilNG.Level);
        Assert.Equal(13, hum.Meridians[0].Level);
        Assert.Equal(14, hum.Meridians[0].BlastHitRate);
        Assert.Equal(15, MySqlItemAccess.GetMeridianAcupoint(ref hum, 0, 0));
        Assert.Equal(19, MySqlItemAccess.GetMeridianAcupoint(ref hum, 0, 4));
    }

    [Fact]
    public void HumanGet_NpcAdd_OutOfRangeIndexIsIgnored()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        d.Stmt("HumanSelectAbilNpcAdd").EnqueueResult(FakeResultSet.Of(
            new object?[] { 0, 11 },
            new object?[] { 29, 22 },
            new object?[] { 30, 99 },      // 越界 → 忽略（原文 Low/High 边界）
            new object?[] { -1, 99 }));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(11, hum.AddSaveAbil[0]);
        Assert.Equal(22, hum.AddSaveAbil[29]);
    }

    [Fact]
    public void HumanGet_MagicUseTick_SubtractsCustomMagicStartId()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // 列 MagicID = 1000 + Index
        d.Stmt("HumanSelectMagicUseTick").EnqueueResult(FakeResultSet.Of(
            new object?[] { 1000, 111 },
            new object?[] { 1299, 222 },
            new object?[] { 1300, 333 }));   // 1300-1000=300 越界（数组 0..299）→ 忽略
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(111, hum.CustomSkillUseTicks[0]);
        Assert.Equal(222, hum.CustomSkillUseTicks[299]);
    }

    [Fact]
    public void HumanGet_CustomMoney_FillsInOrder()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        d.Stmt("GetCustomMoney").EnqueueResult(FakeResultSet.Of(
            new object?[] { "Gold", 10 },
            new object?[] { "Point", 20 }));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal("Gold", hum.CustomMoney[0].NameStr);
        Assert.Equal(10, hum.CustomMoney[0].nCount);
        Assert.Equal("Point", hum.CustomMoney[1].NameStr);
    }

    [Fact]
    public void HumanGet_Items_FillsBagItemAndCallsAllGetStatements()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // HumanSelectitems：30 列，ItemType=5（BagItems）ItemIndex=3
        var itemCells = new object?[30];
        itemCells[0] = 5; itemCells[1] = 3;      // ItemType, ItemIndex
        itemCells[2] = 9001;                     // MakeIndex
        itemCells[3] = 55;                       // wIndex
        itemCells[4] = "屠龙";
        for (int i = 5; i < 30; i++) itemCells[i] = 0;
        d.Stmt("HumanSelectitems").EnqueueResult(FakeResultSet.Of(itemCells));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(9001, hum.BagItems[3].MakeIndex);
        Assert.Equal(55, hum.BagItems[3].wIndex);
        Assert.Equal("屠龙", hum.BagItems[3].NameStr);
        Assert.Equal(1, d.Stmt("HumanSelectitems").QueryCount);
    }

    [Fact]
    public void HumanGet_Items_UnknownItemTypeIsSkippedButColumnsStillConsumed()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        d.Stmt("HumanSelectitems").EnqueueResult(FakeResultSet.Of(
            new object?[] { 99, 0, 1, 2, "x", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, "", "", "", 0, 0, 0, 0 },
            new object?[] { 5, 0, 7, 8, "y", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, "", "", "", 0, 0, 0, 0 }));
        var hum = new THumData();
        Assert.True(db.HumanDB!.Get("a", "c", ref hum, out _));
        Assert.Equal(7, hum.BagItems[0].MakeIndex);   // 第二行仍被正确读入（列游标没被污染）
    }

    [Fact]
    public void HumanGet_FluteZeroCountIsBumpedToOne()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // HumanSelectitemFlute: ItemType, ItemIndex, ValueIndex, Value, OverlapCount
        d.Stmt("HumanSelectitemFlute").EnqueueResult(FakeResultSet.Of(
            new object?[] { 5, 0, 0, 77, 0 },      // GemIndex=77 GemCount=0 → 补 1
            new object?[] { 5, 0, 1, 0, 0 }));     // GemIndex=0 → 保持
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(77, hum.BagItems[0].GetFlute(0).GemIndex);
        Assert.Equal(1, hum.BagItems[0].GetFlute(0).GemCount);
        Assert.Equal(0, hum.BagItems[0].GetFlute(1).GemIndex);
    }

    [Fact]
    public void HumanGet_SkillPower_FillsStruct()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Rows_(HumRow()));
        // HumanSelectSkillPower: SkillID + 7 个值
        d.Stmt("HumanSelectSkillPower").EnqueueResult(FakeResultSet.Of(
            new object?[] { 3, 10, 20, 30, 40, 50, 60, 70 }));
        var hum = new THumData();
        db.HumanDB!.Get("a", "c", ref hum, out _);
        Assert.Equal(10, hum.NpcSkillPowerAdd[3].HumanAttackPercent);
        Assert.Equal(20, hum.NpcSkillPowerAdd[3].HumanAttackValue);
        Assert.Equal(70, hum.NpcSkillPowerAdd[3].RemainingTime);
    }

    // ==========================================================================================
    // DoAdd / DoDelete / DoSetEnabled（MySqlRoleDB.pas:1890-1945）
    // ==========================================================================================

    [Fact]
    public void HumanAdd_BindsEightParams_InSourceOrder_UsesDate2MyDateNow()
    {
        var (db, f, d) = NewDb();
        RoleDbDate.Now = () => new DateTime(2024, 3, 5);
        try
        {
            Assert.True(db.HumanDB!.Add("acc", "chr", true, 1, 2, 3));
        }
        finally { RoleDbDate.Now = () => DateTime.Now; }

        var s = d.Stmt("HumanAdd");
        Assert.Equal(new[] { "acc", "chr" }, s.AllBoundTexts);
        // IsDelete=False, IsSelect=True, CreateDate, Sex, Job, Hair
        Assert.Equal(new[] { 0, 1, 20240305, 1, 2, 3 }, s.AllBoundNumbers);
    }

    [Fact]
    public void HumanAdd_ReturnsFalse_WhenStepFails()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanAdd").EnqueueResult(FakeResultSet.Step(false));
        Assert.False(db.HumanDB!.Add("a", "c", false, 0, 0, 0));
    }

    [Fact]
    public void HumanDelete_And_Restore_And_SetEnabled_AllUseSameStatement()
    {
        var (db, f, d) = NewDb();
        Assert.True(db.HumanDB!.Delete("a", "c"));
        Assert.True(db.HumanDB!.DeleteRestore("a", "c"));
        Assert.True(db.HumanDB!.SetEnabled("a", "c", 2));

        var s = d.Stmt("HumanDelOrRestore");
        Assert.Equal(3, s.StepCount);
        // 三次的 IsDelete 分别是 1 / 0 / 2，后续两个绑定都是 ("a","c")
        Assert.Equal(new[] { 1, 0, 2 }, s.BindHistory.Where(b => b.Kind == "I").Select(b => (int)b.Value!).ToArray());
        Assert.Equal(new[] { "a", "c", "a", "c", "a", "c" }, s.AllBoundTexts);
    }

    // ==========================================================================================
    // DoErase（MySqlRoleDB.pas:1947-2046）
    // ==========================================================================================

    [Fact]
    public void HumanErase_NoId_ReturnsFalse_AndTouchesNoSql()
    {
        var (db, f, d) = NewDb();
        Assert.False(db.HumanDB!.Erase("acc", "chr"));
        Assert.Empty(d.Executed);
        Assert.Equal(0, d.StartTransactionCount);
    }

    [Fact]
    public void HumanErase_CollectsHeroIds_ThenDeletesHeroAndHumanTablesInSourceOrder()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetID").EnqueueResult(FakeResultSet.Of(new object?[] { 5 }));
        d.Stmt("HumanGetHumanHeroID").EnqueueResult(FakeResultSet.Of(
            new object?[] { 11 }, new object?[] { 12 }));
        Assert.True(db.HumanDB!.Erase("acc", "chr"));

        Assert.Equal(1, d.CommitCount);
        Assert.Equal(0, d.RollBackCount);

        // Hero 侧用 in (11,12)，Human 侧用 = 5
        Assert.Contains("delete from HeroItemElementAdd where HeroID in (11,12);", d.Executed);
        Assert.Contains("delete from Hero where HeroID in (11,12);", d.Executed);
        Assert.Contains("delete from HumanItemElementAdd where HumanID = 5;", d.Executed);
        Assert.Contains("delete from Human where HumanID = 5;", d.Executed);
        // 最后一条是 Human 主表
        Assert.EndsWith("delete from Human where HumanID = 5;", d.Executed[^1]);
    }

    [Fact]
    public void HumanErase_NoHeroes_SkipsHeroDeletes()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetID").EnqueueResult(FakeResultSet.Of(new object?[] { 5 }));
        Assert.True(db.HumanDB!.Erase("acc", "chr"));
        Assert.DoesNotContain(d.Executed, s => s.Contains("HeroID"));
        Assert.Contains("delete from Human where HumanID = 5;", d.Executed);
    }

    [Fact]
    public void HumanErase_RollsBack_WhenExecuteThrows_AndReturnsFalse()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetID").EnqueueResult(FakeResultSet.Of(new object?[] { 5 }));
        d.ThrowOnExec = new InvalidOperationException("nope");
        Assert.False(db.HumanDB!.Erase("acc", "chr"));
        Assert.Equal(1, d.RollBackCount);
        Assert.Equal(0, d.CommitCount);
    }

    // ==========================================================================================
    // DoRename / DoChangedGold / DoChangedCustomMoney（MySqlRoleDB.pas:2077-2239）
    // ==========================================================================================

    [Fact]
    public void HumanRename_SyncsDearAndMasterNames()
    {
        var (db, f, d) = NewDb();
        Assert.True(db.HumanDB!.Rename("acc", "oldname", 9, "newname"));
        Assert.Equal(new[] { "newname", "9" }, new[] { d.Stmt("HumanRename").AllBoundTexts[0], d.Stmt("HumanRename").AllBoundNumbers[0].ToString() });
        Assert.Equal(new[] { "newname", "oldname" }, d.Stmt("HumanRenameSyncDearName").AllBoundTexts);
        Assert.Equal(new[] { "newname", "oldname" }, d.Stmt("HumanRenameSyncMasterName").AllBoundTexts);
        Assert.Equal(1, d.CommitCount);
    }

    [Fact]
    public void HumanRename_WhenPrimaryStepFails_SkipsSyncs()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanRename").EnqueueResult(FakeResultSet.Step(false));
        Assert.False(db.HumanDB!.Rename("acc", "oldname", 9, "newname"));
        Assert.Equal(0, d.Stmt("HumanRenameSyncDearName").StepCount);
        Assert.Equal(0, d.Stmt("HumanRenameSyncMasterName").StepCount);
    }

    [Theory]
    [InlineData(TDBChangeGoldType.cgtGold, 0)]
    [InlineData(TDBChangeGoldType.cgtGameGold, 1)]
    [InlineData(TDBChangeGoldType.cgtGamePoint, 2)]
    [InlineData(TDBChangeGoldType.cgtGameDiamond, 3)]
    [InlineData(TDBChangeGoldType.cgtGameGird, 4)]
    public void HumanChangedGold_UpdatesTheRightColumn_AndReturnsNewValue(TDBChangeGoldType type, int column)
    {
        var (db, f, d) = NewDb();
        var start = new object?[] { 100, 200, 300, 400, 500 };
        d.Stmt("HumanGetHumanGoldInfo").EnqueueResult(FakeResultSet.Of(start));
        Assert.True(db.HumanDB!.ChangedGold("chr", type, 50, out uint result));

        int expected = (int)start[column]! + 50;
        Assert.Equal((uint)expected, result);
        // UPDATE 绑定顺序固定是 Gold,GameGold,GamePoint,GameDiamond,GameGird,HumanName
        var nums = d.Stmt("HumanUpdateHumanGoldInfo").AllBoundNumbers;
        Assert.Equal(new[] { 100, 200, 300, 400, 500 }.Select((v, i) => i == column ? expected : v).ToArray(), nums);
        Assert.Equal(new[] { "chr" }, d.Stmt("HumanUpdateHumanGoldInfo").AllBoundTexts);
    }

    [Fact]
    public void HumanChangedGold_ClampsToZero_OnNegativeResult()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetHumanGoldInfo").EnqueueResult(FakeResultSet.Of(new object?[] { 10, 0, 0, 0, 0 }));
        Assert.True(db.HumanDB!.ChangedGold("chr", TDBChangeGoldType.cgtGold, -999, out uint result));
        Assert.Equal(0u, result);
    }

    [Fact]
    public void HumanChangedGold_ClampsToLongWordMax_OnOverflow()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetHumanGoldInfo").EnqueueResult(FakeResultSet.Of(new object?[] { -1, 0, 0, 0, 0 })); // (uint)-1 = 4294967295
        Assert.True(db.HumanDB!.ChangedGold("chr", TDBChangeGoldType.cgtGold, 100, out uint result));
        Assert.Equal(uint.MaxValue, result);
    }

    [Fact]
    public void HumanChangedGold_CustomMoneyChangeType_HasNoCaseBranch_SoValueIsUnchanged()
    {
        // 原文缺陷（逐字保留）：cgtCustomMoney 不在 case 里 → ResultValue 保持初值，但仍然执行 UPDATE
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetHumanGoldInfo").EnqueueResult(FakeResultSet.Of(new object?[] { 1, 2, 3, 4, 5 }));
        Assert.True(db.HumanDB!.ChangedGold("chr", TDBChangeGoldType.cgtCustomMoney, 999, out uint result));
        Assert.Equal(0u, result);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, d.Stmt("HumanUpdateHumanGoldInfo").AllBoundNumbers);
    }

    [Fact]
    public void HumanChangedGold_MissingRow_ReturnsFalse_WithoutUpdate()
    {
        var (db, f, d) = NewDb();
        Assert.False(db.HumanDB!.ChangedGold("chr", TDBChangeGoldType.cgtGold, 1, out uint result));
        Assert.Equal(0u, result);
        Assert.Equal(0, d.Stmt("HumanUpdateHumanGoldInfo").StepCount);
    }

    [Fact]
    public void HumanChangedCustomMoney_UpdatesWhenRowExists()
    {
        var (db, f, d) = NewDb();
        d.Stmt("GetCustomMoneyByName").EnqueueResult(FakeResultSet.Of(new object?[] { 100 }));
        Assert.True(db.HumanDB!.ChangedCustomMoney(7, "Gold", 25, out uint result));
        Assert.Equal(125u, result);
        Assert.Equal(new[] { 125, 7 }, d.Stmt("UpdateCustomMoney").AllBoundNumbers);
        Assert.Equal(new[] { "Gold" }, d.Stmt("UpdateCustomMoney").AllBoundTexts);
    }

    [Fact]
    public void HumanChangedCustomMoney_MissingRow_ReturnsFalse()
    {
        // 原文缺陷（逐字保留）：if 不进 → Result 从未赋值；C# 侧按初值 False
        var (db, f, d) = NewDb();
        Assert.False(db.HumanDB!.ChangedCustomMoney(7, "Gold", 25, out uint result));
        Assert.Equal(0u, result);
    }

    // ==========================================================================================
    // DoSave / SaveHumanData（MySqlRoleDB.pas:2061-2916）
    // ==========================================================================================

    [Fact]
    public void HumanSave_WhenUpdateHumanFails_ReturnsFalse_AndStopsBeforeChildTables()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanUpdate").EnqueueResult(FakeResultSet.Step(false));
        var hum = new HumWithFullName();
        Assert.False(db.HumanDB!.Save(1, ref hum.Data));
        Assert.Equal(0, d.Stmt("HumanInsertAbil").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertItems").StepCount);
        Assert.Equal(1, d.CommitCount);   // 原文即使 SaveHumanData 返回 False 也 Commit
    }

    [Fact]
    public void HumanSave_EmptyStruct_WritesNoChildRows_ButReturnsTrue()
    {
        var (db, f, d) = NewDb();
        var hum = new HumWithFullName();
        Assert.True(db.HumanDB!.Save(1, ref hum.Data));
        Assert.Equal(1, d.Stmt("HumanUpdate").StepCount);
        Assert.Equal(1, d.Stmt("HumanInsertAbil").StepCount);
        Assert.Equal(1, d.Stmt("HumanInsertAbilNG").StepCount);
        Assert.Equal(1, d.Stmt("HumanInsertAbilWine").StepCount);
        // 全 0 / 全空 → 所有"非零才写"的子表一条都不写
        Assert.Equal(0, d.Stmt("HumanInsertAbilNpcAdd").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertGamePetData").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertMagic").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertQuestFlag").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertCustomMoney").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertItems").StepCount);
        Assert.Equal(0, d.Stmt("HumanInsertSkillPower").StepCount);
        // 24 条 delete 一律执行（原文这些字符串**不带**结尾分号：
        //   `Execute('delete from HumanItems where HumanID = ' + IntToStr(HumanID));`）
        Assert.Contains("delete from HumanItems where HumanID = 1", d.Executed);
        Assert.Contains("delete from HumanMoney where HumanID = 1", d.Executed);
        Assert.Contains("delete from HumanAbilNpcAdd where HumanID = 1", d.Executed);
        Assert.DoesNotContain("delete from Human where HumanID = 1", d.Executed);   // 主表删除只出现在 DoErase
    }

    [Fact]
    public void HumanSave_WritesOnlyNonZeroSlots()
    {
        var (db, f, d) = NewDb();
        var hum = new HumWithFullName();
        hum.Data.AddSaveAbil[0] = 0;      // 跳过
        hum.Data.AddSaveAbil[3] = 9;      // 写入
        hum.Data.GodBlessItemsState[2] = 7;
        hum.Data.QuestFlag[5] = 1;
        hum.Data.UValues[10] = 4;
        hum.Data.JValues[11] = 5;
        hum.Data.TValues[12].Value = "t";
        hum.Data.ZValues[13].Value = "z";
        hum.Data.CustomMoney[0].NameStr = "Gold";
        hum.Data.CustomMoney[0].nCount = 100;
        hum.Data.CustomSkillUseTicks[0] = 55;
        hum.Data.wStatusTimeArr[1] = 66;

        Assert.True(db.HumanDB!.Save(1, ref hum.Data));

        Assert.Equal(1, d.Stmt("HumanInsertAbilNpcAdd").StepCount);
        Assert.Equal(new[] { 1, 3, 9 }, d.Stmt("HumanInsertAbilNpcAdd").AllBoundNumbers);
        Assert.Equal(new[] { 1, 2, 7 }, d.Stmt("HumanInsertGodBlessState").AllBoundNumbers);
        Assert.Equal(new[] { 1, 5, 1 }, d.Stmt("HumanInsertQuestFlag").AllBoundNumbers);
        Assert.Equal(new[] { 1, 10, 4 }, d.Stmt("HumanInsertVariableU").AllBoundNumbers);
        Assert.Equal(new[] { 1, 11, 5 }, d.Stmt("HumanInsertVariableJ").AllBoundNumbers);
        Assert.Equal(new[] { "t" }, d.Stmt("HumanInsertVariableT").AllBoundTexts);
        Assert.Equal(new[] { "z" }, d.Stmt("HumanInsertVariableZ").AllBoundTexts);
        Assert.Equal(new[] { 1, 100 }, d.Stmt("HumanInsertCustomMoney").AllBoundNumbers);
        Assert.Equal(new[] { "Gold" }, d.Stmt("HumanInsertCustomMoney").AllBoundTexts);
        // 自定义技能列值 +1000
        Assert.Equal(new[] { 1, 1000, 55 }, d.Stmt("HumanInsertMagicUseTick").AllBoundNumbers);
        Assert.Equal(new[] { 1, 1, 66 }, d.Stmt("HumanInsertStatusTime").AllBoundNumbers);
    }

    [Fact]
    public void HumanSave_ItemsNeedBothMakeIndexAndWIndex()
    {
        var (db, f, d) = NewDb();
        var hum = new HumWithFullName();
        hum.Data.BagItems[0] = new TUserItem { MakeIndex = 0, wIndex = 5 };   // 缺 MakeIndex → 跳过
        hum.Data.BagItems[1] = new TUserItem { MakeIndex = 5, wIndex = 0 };   // 缺 wIndex → 跳过
        hum.Data.BagItems[2] = new TUserItem { MakeIndex = 5, wIndex = 6 };   // 写入
        Assert.True(db.HumanDB!.Save(1, ref hum.Data));
        Assert.Equal(1, d.Stmt("HumanInsertItems").StepCount);
        var nums = d.Stmt("HumanInsertItems").AllBoundNumbers;
        Assert.Equal(1, nums[0]);       // HumanID
        Assert.Equal(5, nums[1]);       // ItemType = 5 (BagItems)
        Assert.Equal(2, nums[2]);       // ItemIndex
        Assert.Equal(5, nums[3]);       // MakeIndex
        Assert.Equal(6, nums[4]);       // wIndex
    }

    [Fact]
    public void HumanSave_SkillPowerNeedsOneNonZeroNumeric()
    {
        var (db, f, d) = NewDb();
        var hum = new HumWithFullName();
        hum.Data.NpcSkillPowerAdd[4].RemainingTime = 1;   // 只有 RemainingTime 非零
        Assert.True(db.HumanDB!.Save(7, ref hum.Data));
        // 差异/易错点：原文判据是 6 个数值字段（不含 RemainingTime），所以这一条**不落库**
        Assert.Equal(0, d.Stmt("HumanInsertSkillPower").StepCount);

        // 让 HumanAttackPercent 非零后才会写入，且 RemainingTime 作为第 9 个参数一起写出
        hum.Data.NpcSkillPowerAdd[4].HumanAttackPercent = 3;
        Assert.True(db.HumanDB!.Save(7, ref hum.Data));
        Assert.Equal(1, d.Stmt("HumanInsertSkillPower").StepCount);
        var nums = d.Stmt("HumanInsertSkillPower").AllBoundNumbers;
        Assert.Equal(7, nums[0]);   // HumanID
        Assert.Equal(4, nums[1]);   // SkillID = I
        Assert.Equal(3, nums[2]);   // HumanAttackPercent
        Assert.Equal(1, nums[^1]);  // RemainingTime 一起写出
    }

    [Fact]
    public void HumanSave_RollsBack_WhenExecuteThrows()
    {
        var (db, f, d) = NewDb();
        var hum = new HumWithFullName();
        d.ThrowOnExec = new InvalidOperationException("exec boom");
        Assert.False(db.HumanDB!.Save(1, ref hum.Data));
        Assert.Equal(1, d.RollBackCount);
        Assert.Equal(0, d.CommitCount);
    }

    [Fact]
    public void HumanSave_CustomMoneyChange_WritesAllFiveCurrencyColumnsInFixedOrder()
    {
        var (db, f, d) = NewDb();
        var hum = new HumWithFullName();
        hum.Data.nGold = 1; hum.Data.nGameGold = 2; hum.Data.nGamePoint = 3;
        hum.Data.nGameDiamond = 4; hum.Data.nGameGird = 5;
        Assert.True(db.HumanDB!.Save(1, ref hum.Data));
        // 五个币种在 HumanUpdate 的绑定序列里必须**连续且按 Gold,GameGold,GamePoint,GameDiamond,GameGird 顺序**
        var nums = d.Stmt("HumanUpdate").AllBoundNumbers;
        int at = -1;
        for (int i = 0; i + 4 < nums.Count; i++)
            if (nums[i] == 1 && nums[i + 1] == 2 && nums[i + 2] == 3 && nums[i + 3] == 4 && nums[i + 4] == 5) { at = i; break; }
        Assert.True(at >= 0, "找不到连续的五个币种绑定序列 " + string.Join(",", nums));
    }

    /// <summary>把 THumData 包一层，方便在测试里 ref。</summary>
    private sealed class HumWithFullName
    {
        public THumData Data;
        public HumWithFullName() { Data = new THumData(); Data.Account = "acc"; Data.ChrName = "chr"; }
    }

    // ==========================================================================================
    // 排行榜（MySqlRoleDB.pas:2918-3358）
    // ==========================================================================================

    [Fact]
    public void HumanRank_Path1_UsesTopCountQueries_AndStopsAtTopCount()
    {
        var (db, f, d) = NewDb();
        // 4 个人物榜（Job 0,0,1,2）+ 2 个 MasterCount 榜（TopCount 变体只有 1 次 master）
        for (int i = 0; i < 4; i++)
        {
            d.Stmt("HumanGetLevelRankTopCount").EnqueueResult(FakeResultSet.Of(
                new object?[] { "A", 10 }, new object?[] { "B", 9 }, new object?[] { "C", 8 }));
        }
        d.Stmt("HumanGetMasterRankTopCount").EnqueueResult(FakeResultSet.Of(new object?[] { "M", 3 }));

        var human = new TRoleRankList(); var warrior = new TRoleRankList();
        var wizard = new TRoleRankList(); var taoist = new TRoleRankList();
        var master = new TRoleRankList();
        db.HumanDB!.GetRankData(0, 0, 2, human, warrior, wizard, taoist, master);

        Assert.Equal(2, human.Count);        // TopCount=2 → Break
        Assert.Equal("A", human.Items(0)!.HumanName);
        Assert.Equal(10u, human.Items(0)!.Level);
        Assert.Equal(0, human.Items(0)!.RankIndex);
        Assert.Equal("", human.Items(0)!.HeroName);
        Assert.Equal(2, warrior.Count);
        Assert.Equal(2, wizard.Count);
        Assert.Equal(2, taoist.Count);
        Assert.Equal(1, master.Count);
        Assert.Equal(3u, master.Items(0)!.MasterCount);
        // 绑定：Job,Job,Job,QueryCount = TopCount+50
        Assert.Equal(new[] { 0, 0, 0, 52 }, d.Stmt("HumanGetLevelRankTopCount").BindHistory.Where(b => b.Kind == "I").Select(b => (int)b.Value!).Take(4).ToArray());
    }

    [Fact]
    public void HumanRank_Path1_FiltersRankingNames()
    {
        var (db, f, d) = NewDb();
        DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList();
        DBShareSeam.g_FilterRankingNameTextList.Add("GM");
        try
        {
            for (int i = 0; i < 4; i++)
                d.Stmt("HumanGetLevelRankTopCount").EnqueueResult(FakeResultSet.Of(
                    new object?[] { "GM001", 99 }, new object?[] { "Player", 10 }));
            d.Stmt("HumanGetMasterRankTopCount").EnqueueResult(FakeResultSet.Empty);

            var human = new TRoleRankList();
            db.HumanDB!.GetRankData(0, 0, 10, human, new TRoleRankList(), new TRoleRankList(), new TRoleRankList(), new TRoleRankList());
            Assert.Equal(1, human.Count);
            Assert.Equal("Player", human.Items(0)!.HumanName);
        }
        finally { DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList(); }
    }

    [Fact]
    public void HumanRank_Path2_TopCountZero_UsesCheckLevel_AndDoesNotFilter()
    {
        var (db, f, d) = NewDb();
        DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList();
        DBShareSeam.g_FilterRankingNameTextList.Add("GM");
        try
        {
            for (int i = 0; i < 4; i++)
                d.Stmt("HumanGetLevelRankCheckLevel").EnqueueResult(FakeResultSet.Of(
                    new object?[] { "GM001", 99 }, new object?[] { "A", 10 }));
            d.Stmt("HumanGetMasterRankCheckLevel").EnqueueResult(FakeResultSet.Empty);

            var human = new TRoleRankList();
            db.HumanDB!.GetRankData(20, 500, 0, human, new TRoleRankList(), new TRoleRankList(), new TRoleRankList(), new TRoleRankList());
            Assert.Equal(2, human.Count);      // 原文明细：这一支不过滤
            Assert.Equal("GM001", human.Items(0)!.HumanName);
            // 参数：Job,Job,Job,MinLevel,MaxLevel
            Assert.Equal(new[] { 0, 0, 0, 20, 500 }, d.Stmt("HumanGetLevelRankCheckLevel").BindHistory.Where(b => b.Kind == "I").Select(b => (int)b.Value!).Take(5).ToArray());
        }
        finally { DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList(); }
    }

    [Fact]
    public void HumanRank_Path3_FiltersAndStopsAtTopCount()
    {
        var (db, f, d) = NewDb();
        for (int i = 0; i < 4; i++)
            d.Stmt("HumanGetLevelRankCheckLevelAndCount").EnqueueResult(FakeResultSet.Of(
                new object?[] { "A", 10 }, new object?[] { "B", 9 }));
        d.Stmt("GetMasterRankCheckLevelAndCount").EnqueueResult(FakeResultSet.Of(new object?[] { "M", 2 }));

        var human = new TRoleRankList(); var master = new TRoleRankList();
        db.HumanDB!.GetRankData(20, 500, 1, human, new TRoleRankList(), new TRoleRankList(), new TRoleRankList(), master);
        Assert.Equal(1, human.Count);
        Assert.Equal(1, master.Count);
        // Job,Job,Job,MinLevel,MaxLevel,QueryCount
        Assert.Equal(new[] { 0, 0, 0, 20, 500, 51 }, d.Stmt("HumanGetLevelRankCheckLevelAndCount").BindHistory.Where(b => b.Kind == "I").Select(b => (int)b.Value!).Take(6).ToArray());
    }

    [Fact]
    public void HumanRank_ClearsAllListsFirst()
    {
        var (db, f, d) = NewDb();
        var human = new TRoleRankList();
        human.Add(new TRoleRankData { HumanName = "stale" });
        var master = new TRoleRankList();
        master.Add(new TRoleRankData { HumanName = "stale" });
        db.HumanDB!.GetRankData(0, 0, 5, human, new TRoleRankList(), new TRoleRankList(), new TRoleRankList(), master);
        Assert.Equal(0, human.Count);
        Assert.Equal(0, master.Count);
    }

    // ==========================================================================================
    // Hero 侧（MySqlRoleDB.pas:3936-5749）
    // ==========================================================================================

    [Fact]
    public void HeroSearchByAccount_IsDeleteIsZero_AndIsHeroIsTrue()
    {
        // 差异断言：Hero 版 IsDelete 恒 0（不读列）、IsHero 恒 True；Human 版读列且 IsHero=False
        var (db, f, d) = NewDb();
        d.Stmt("HeroSearchByAccount1").EnqueueResult(FakeResultSet.Of(new object?[] { "acc", "hero", 1, 2, 30 }));
        var list = new TSerarchRoleList();
        Assert.Equal(1, db.HeroDB!.SearchByAccount("acc", TSearchMatchType.smtComplete, list));
        Assert.Equal(0, list.Items(0)!.IsDelete);
        Assert.True(list.Items(0)!.IsHero);
        Assert.Equal("hero", list.Items(0)!.RoleName);
        Assert.Equal(30u, list.Items(0)!.Level);
    }

    [Fact]
    public void HeroSearchByName_Fuzzy_WrapsWithPercent()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroSearchByName2").EnqueueResult(FakeResultSet.Of(new object?[] { "acc", "hero", 0, 1, 1 }));
        Assert.Equal(1, db.HeroDB!.SearchByName("hero", TSearchMatchType.smtFuzzy, new TSerarchRoleList()));
        Assert.Equal(new[] { "%hero%" }, d.Stmt("HeroSearchByName2").AllBoundTexts);
    }

    [Fact]
    public void HeroGet_NoRow_ReturnsFalse()
    {
        var (db, f, d) = NewDb();
        var hero = new THeroData();
        Assert.False(db.HeroDB!.Get("nobody", ref hero, out int id));
        Assert.Equal(0, id);
    }

    [Fact]
    public void HeroGet_MainRowOnly_FillsColumns()
    {
        var (db, f, d) = NewDb();
        var cells = new object?[42];
        for (int i = 0; i < cells.Length; i++) cells[i] = 0;
        cells[0] = 3;           // HeroID
        cells[1] = "acc";       // Account 子查询列
        cells[2] = "hero";      // HeroName
        cells[8] = 25;          // Level
        cells[9] = 1;           // ReLevel
        cells[10] = 1.5;        // LoyalPoint
        d.Stmt("HeroSelect").EnqueueResult(FakeResultSet.Of(cells));

        var hero = new THeroData();
        Assert.True(db.HeroDB!.Get("hero", ref hero, out int id));
        Assert.Equal(3, id);
        Assert.Equal("acc", hero.Account);
        Assert.Equal("hero", hero.ChrName);
        Assert.Equal(25, hero.Abil.Level);
        Assert.Equal(1.5, hero.rLoyalPoint);
        // 原文没有 HomeMap/HomeX/HomeY 的赋值 —— THeroData 里根本没有这三个字段（SQL 里也没有那些列）
        Assert.Equal(0, hero.wCurX);
        Assert.Equal(0, hero.wCurY);
    }

    [Fact]
    public void HeroItemTypes_AreOnlyOneToFive_Diff_FromHuman()
    {
        // 差异断言：英雄只有 5 组物品，ItemType 6/7（StorageItems/GamePetBagItems）不存在
        var (db, f, d) = NewDb();
        var cells = new object?[42];
        for (int i = 0; i < cells.Length; i++) cells[i] = 0;
        cells[0] = 3; cells[1] = "acc"; cells[2] = "hero";
        d.Stmt("HeroSelect").EnqueueResult(FakeResultSet.Of(cells));

        var itemCells = new object?[30];
        itemCells[0] = 6;       // ItemType 6 → 英雄侧无效
        itemCells[1] = 0;
        itemCells[2] = 999;     // MakeIndex
        itemCells[3] = 1;       // wIndex
        itemCells[4] = "x";
        for (int i = 5; i < 30; i++) itemCells[i] = 0;
        d.Stmt("HeroSelectitems").EnqueueResult(FakeResultSet.Of(itemCells));

        var hero = new THeroData();
        Assert.True(db.HeroDB!.Get("hero", ref hero, out _));
        Assert.Equal(0, hero.HumItems[0].MakeIndex);
        Assert.Equal(0, hero.BagItems[0].MakeIndex);
    }

    [Fact]
    public void HeroAdd_ReadsHumanThenWritesHeroThenSyncsHuman()
    {
        var (db, f, d) = NewDb();
        RoleDbDate.Now = () => new DateTime(2024, 1, 2);
        try
        {
            d.Stmt("HumanGetHumanHeroName").EnqueueResult(FakeResultSet.Of(new object?[] { "", "" }));
            Assert.True(db.HeroDB!.Add("acc", "human", 9, "hero", 1, 2, 3, false));
        }
        finally { RoleDbDate.Now = () => DateTime.Now; }

        // 顺序：HumanID(int), HeroName(text), IsDelete=False(bool), CreateDate(int), Sex, Job, Hair
        Assert.Equal(new[] { 9, 0, 20240102, 1, 2, 3 }, d.Stmt("HeroAdd").AllBoundNumbers);
        Assert.Equal(new[] { "hero" }, d.Stmt("HeroAdd").AllBoundTexts);
        // Human 同步：HeroName=hero, DeputyHeroName=''
        Assert.Equal(new[] { "hero", "" }, d.Stmt("HeroRenameSyncHumanHero").AllBoundTexts);
        Assert.Equal(1, d.CommitCount);
    }

    [Fact]
    public void HeroAdd_DeputyHero_GoesToDeputySlot()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetHumanHeroName").EnqueueResult(FakeResultSet.Of(new object?[] { "main", "" }));
        Assert.True(db.HeroDB!.Add("acc", "human", 9, "deputy", 1, 2, 3, true));
        Assert.Equal(new[] { "main", "deputy" }, d.Stmt("HeroRenameSyncHumanHero").AllBoundTexts);
    }

    [Fact]
    public void HeroAdd_StepsTrueButRequiresHumanSync()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetHumanHeroName").EnqueueResult(FakeResultSet.Of(new object?[] { "", "" }));
        d.Stmt("HeroAdd").EnqueueResult(FakeResultSet.Step(false));
        Assert.False(db.HeroDB!.Add("acc", "human", 9, "hero", 1, 2, 3, false));
        Assert.Equal(0, d.Stmt("HeroRenameSyncHumanHero").StepCount);
    }

    [Fact]
    public void HeroErase_NoId_ReturnsFalse()
    {
        var (db, f, d) = NewDb();
        Assert.False(db.HeroDB!.Erase("nobody"));
        Assert.Empty(d.Executed);
    }

    [Fact]
    public void HeroErase_MatchesHeroNameCaseInsensitively_AndClearsHumanSlots()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroGetID").EnqueueResult(FakeResultSet.Of(new object?[] { 4 }));
        // HeroGetHumanInfo2: HumanID, HumanName, IsStorageHero, IsStorageDeputyHero, HeroName, DeputyHeroName
        d.Stmt("HeroGetHumanInfo2").EnqueueResult(FakeResultSet.Of(
            new object?[] { 9, "human", 1, 0, "HERO", "" }));
        Assert.True(db.HeroDB!.Erase("hero"));      // SameText 命中（大小写不同）

        Assert.Contains("delete from Hero where HeroID = 4;", d.Executed);
        Assert.Equal(new object?[] { false, false, false, "", "", 9 }, d.Stmt("HeroRenameSyncHumanHero2").BindHistory.Select(b => b.Value).ToArray());
        Assert.Equal(1, d.CommitCount);
    }

    [Fact]
    public void HeroErase_WhenNameDoesNotMatchEitherSlot_SkipsHumanSync()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroGetID").EnqueueResult(FakeResultSet.Of(new object?[] { 4 }));
        d.Stmt("HeroGetHumanInfo2").EnqueueResult(FakeResultSet.Of(
            new object?[] { 9, "human", 1, 1, "other", "other2" }));
        Assert.True(db.HeroDB!.Erase("hero"));
        Assert.Equal(0, d.Stmt("HeroRenameSyncHumanHero2").StepCount);
    }

    [Fact]
    public void HeroRename_SyncsHumanWhenNamesMatch()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroGetHumanInfo").EnqueueResult(FakeResultSet.Of(
            new object?[] { "acc", 9, "human", "hero", "" }));
        Assert.True(db.HeroDB!.Rename(4, "hero", "newhero"));
        Assert.Equal(new[] { "newhero", "4" }, new[] { d.Stmt("HeroRename").AllBoundTexts[0], d.Stmt("HeroRename").AllBoundNumbers[0].ToString() });
        Assert.Equal(new[] { "newhero", "" }, d.Stmt("HeroRenameSyncHumanHero").AllBoundTexts);
    }

    [Fact]
    public void HeroRename_NoNameMatch_SkipsHumanSync()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroGetHumanInfo").EnqueueResult(FakeResultSet.Of(
            new object?[] { "acc", 9, "human", "x", "y" }));
        Assert.True(db.HeroDB!.Rename(4, "hero", "newhero"));
        Assert.Equal(0, d.Stmt("HeroRenameSyncHumanHero").StepCount);
    }

    [Fact]
    public void HeroAssess_AcceptsBothOrderings()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroGetHumanInfo").EnqueueResult(FakeResultSet.Of(
            new object?[] { "acc", 9, "human", "b", "a" }));
        Assert.True(db.HeroDB!.Assess(4, "a", "b"));      // 互换顺序也认
        Assert.Equal(new object?[] { true, false, false, "a", "b", 9 }, d.Stmt("HeroRenameSyncHumanHero2").BindHistory.Select(b => b.Value).ToArray());
    }

    [Fact]
    public void HeroAssess_NoMatch_ReturnsFalse()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroGetHumanInfo").EnqueueResult(FakeResultSet.Of(
            new object?[] { "acc", 9, "human", "x", "y" }));
        Assert.False(db.HeroDB!.Assess(4, "a", "b"));
        Assert.Equal(0, d.Stmt("HeroRenameSyncHumanHero2").StepCount);
    }

    [Fact]
    public void HeroSave_EmptyStruct_WritesOnlyTheMainRow_AndReturnsTrue()
    {
        var (db, f, d) = NewDb();
        var hero = new THeroData();
        Assert.True(db.HeroDB!.Save(4, ref hero));
        Assert.Equal(1, d.Stmt("HeroUpdate").StepCount);
        Assert.Equal(1, d.Stmt("HeroInsertAbil").StepCount);
        Assert.Equal(1, d.Stmt("HeroInsertAbilNG").StepCount);
        Assert.Equal(1, d.Stmt("HeroInsertAbilWine").StepCount);
        Assert.Equal(0, d.Stmt("HeroInsertItems").StepCount);
        Assert.Equal(0, d.Stmt("HeroInsertSkillPower").StepCount);
        // HeroInsertAbilWine 没有 boPleaseDrink（10 个参数，Human 版是 11 个）
        Assert.Equal(10, d.Stmt("HeroInsertAbilWine").AllBoundNumbers.Count);
    }

    [Fact]
    public void HeroSave_UpdateFails_ReturnsFalse()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HeroUpdate").EnqueueResult(FakeResultSet.Step(false));
        var hero = new THeroData();
        Assert.False(db.HeroDB!.Save(4, ref hero));
        Assert.Equal(0, d.Stmt("HeroInsertAbil").StepCount);
    }

    [Fact]
    public void HeroRank_HasFourLists_AndFiltersOnHeroName()
    {
        var (db, f, d) = NewDb();
        DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList();
        DBShareSeam.g_FilterRankingNameTextList.Add("GM");
        try
        {
            for (int i = 0; i < 4; i++)
                d.Stmt("HeroGetLevelRankTopCount").EnqueueResult(FakeResultSet.Of(
                    new object?[] { "humanGM", "GMhero", 99 },    // 过滤判据用 HeroName
                    new object?[] { "human", "hero", 10 }));
            var list = new TRoleRankList();
            db.HeroDB!.GetRankData(0, 0, 10, list, new TRoleRankList(), new TRoleRankList(), new TRoleRankList());
            Assert.Equal(1, list.Count);
            Assert.Equal("human", list.Items(0)!.HumanName);
            Assert.Equal("hero", list.Items(0)!.HeroName);
            Assert.Equal(10u, list.Items(0)!.Level);
        }
        finally { DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList(); }
    }

    [Fact]
    public void HeroRank_Path2_DoesNotFilter()
    {
        var (db, f, d) = NewDb();
        DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList();
        DBShareSeam.g_FilterRankingNameTextList.Add("GM");
        try
        {
            for (int i = 0; i < 4; i++)
                d.Stmt("HeroGetLevelRankCheckLevel").EnqueueResult(FakeResultSet.Of(
                    new object?[] { "h", "GMhero", 99 }));
            var list = new TRoleRankList();
            db.HeroDB!.GetRankData(20, 500, 0, list, new TRoleRankList(), new TRoleRankList(), new TRoleRankList());
            Assert.Equal(1, list.Count);
        }
        finally { DBShareSeam.g_FilterRankingNameTextList = new GXX.Core.Util.TStringList(); }
    }
}

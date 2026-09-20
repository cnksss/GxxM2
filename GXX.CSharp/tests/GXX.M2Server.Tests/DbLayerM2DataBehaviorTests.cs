// 源单元（文件头证据）：
//   Source/M2Engine/SqliteM2DataDB.pas
//   Source/M2Engine/MySqlM2DataDB.pas
//   Source/M2Engine/M2DataCommon.pas（TM2DataDB 的 public 面与 TItemInfo 记录）
//
// 测试组：M2DataDB 的行为（**不连真库**，全部注入内存接缝）。
//   * 每个公开方法 ≥3 个用例（空/0/负/超界/异常/SQL 注入字符）。
//   * 姊妹实现（SQLite vs MySQL）"看起来一样实则不同"的地方写**差异断言**：
//       - Flute 读取：DoLoadItemsFromDB 绑 0，DoLoadItemFromDB 绑 ItemType（原文不一致）；
//       - DoFinal 都不 Finalize SelectItems / SelectItems_Sort（原文缺陷）；
//       - 方言 API：OrderBindInt/Step vs OrderBindParamInt/Query+Fetch。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.Core.Protocol;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerM2DataBehaviorTests
{
    private static void IsolateSqliteFiles(bool fileExists)
    {
        TSqliteM2DataDB.ExtractFilePath = () => @"C:\M2Test\";
        TSqliteM2DataDB.DirectoryExists = _ => true;
        TSqliteM2DataDB.ForceDirectories = _ => { };
        TSqliteM2DataDB.FileExists = _ => fileExists;
        TSqliteM2DataDB.DeleteFile = _ => { };
        TSqliteM2DataDB.SaveResourceToFile = (_, _) => { };
        TSqliteM2DataDB.Sqlite3ConfigMultithread = () => { };
    }

    /// <summary>建一个已完成 DoInit 的 SQLite 实现（库文件"不存在"分支 → 只登记 20 条语句）。</summary>
    private static (TSqliteM2DataDB Unit, FakeSqliteDatabase Db, FakeDbLayerHost Host) NewSqlite()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteM2DataDB(host, db);
        unit.DoInit();
        return (unit, db, host);
    }

    /// <summary>SelectItems 的 28 列顺序（SqliteM2DataDB.pas:436-467 的 select 列表，去掉 ParentID/ItemType 两个条件列）。</summary>
    private static object?[] ItemRow(int itemIndex, int makeIndex, int dbIndex, string name, int dura, int duraMax)
    {
        var row = new object?[28];
        row[0] = itemIndex;
        row[1] = makeIndex;
        row[2] = dbIndex;
        row[3] = name;
        row[4] = dura;
        row[5] = duraMax;
        for (int i = 6; i < row.Length; i++) row[i] = 0;
        row[21] = "";    // ItemFrom.MapName（第 22 列）
        row[22] = "";    // ItemFrom.MonName（第 23 列）
        row[23] = "";    // ItemFrom.MakerName（第 24 列）
        row[24] = 0.0;   // ItemFrom.DateTime（第 25 列，Double）
        return row;
    }

    // ==================================================================
    // 构造 / Destroy / DataBase / IsInitOK / 子库属性
    // ==================================================================

    [Fact]
    public void Ctor_WithInjectedDatabase_ExposesItThroughDataBase()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);

        Assert.Same(db, unit.DataBase);
        Assert.Same(db, unit.Database);
        Assert.False(unit.IsInitOK);
    }

    [Fact]
    public void Ctor_WithoutDatabase_UsesTheUnavailableSeamAndDoesNotInit()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost());

        // 原文 Create 里 FDB := TSQLite3Database.Create；托管侧缺省用"不可用"实现（装配方注入真库）。
        Assert.IsType<UnavailableSqliteDatabase>(unit.DataBase);
        Assert.False(unit.IsInitOK);
    }

    [Fact]
    public void Destroy_ReleasesTheDatabaseReference()
    {
        var (unit, _, _) = NewSqlite();
        Assert.NotNull(unit.DataBase);

        unit.Destroy();

        Assert.Null(unit.DataBase);
        Assert.Null(unit.Database);
        // Destroy 之后 DoInit 直接返回（原文 Free 之后不再使用）；IsInitOK 不被改写。
        unit.DoInit();
        Assert.False(unit.IsInitOK);
    }

    [Fact]
    public void IsInitOK_IsSetByInitNotByDoInit()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);

        Assert.False(unit.IsInitOK);
        // ★ 原文：FIsInitOK 由 TM2DataDB.Init（M2DataCommon.pas:1638）设置，DoInit 自己不设。
        unit.DoInit();
        Assert.False(unit.IsInitOK);
        unit.Init();
        Assert.True(unit.IsInitOK);
        // 原文 TM2DataDB.Final 不改 IsInitOK。
        unit.Final();
        Assert.True(unit.IsInitOK);
    }

    [Fact]
    public void AuctionDB_And_UserShopDB_AreAssignableSlots()
    {
        var (unit, _, _) = NewSqlite();
        Assert.Null(unit.AuctionDB);
        Assert.Null(unit.UserShopDB);

        var shop = new TSqliteUserShopDB(new FakeDbLayerHost());
        unit.UserShopDB = shop;

        Assert.Same(shop, unit.UserShopDB);
    }

    [Fact]
    public void Init_And_Final_DelegateToDoInitAndDoFinal()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);

        unit.Init();
        Assert.Equal(20, db.Created.Count);
        Assert.True(unit.IsInitOK);

        unit.Final();
        Assert.Equal(18, db.Created.Count(s => s.FinalizeCount == 1));
    }

    /// <summary>原文 TM2DataDB.Init / Final 还会驱动三个子库（本车道只装配 AuctionDB / UserShopDB）。</summary>
    [Fact]
    public void Init_And_Final_AlsoDriveTheSubDatabases()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);

        var auctionDb = new FakeSqliteDatabase();
        var auction = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = auctionDb }, auctionDb);
        unit.AuctionDB = auction;

        unit.Init();
        // 子库被 Init（原文 FAuctionDB.DoInit）：21 条语句登记完成并 Prepare。
        Assert.Equal(21, auctionDb.Created.Count);

        unit.Final();
        Assert.All(auctionDb.Created, s => Assert.Equal(1, s.FinalizeCount));
    }

    [Fact]
    public void Run_IsAnEmptyMethodInTheOriginal()
    {
        var (unit, db, host) = NewSqlite();
        int created = db.Created.Count;
        int steps = db.Created.Sum(s => s.StepCount);

        unit.Run();

        // M2DataCommon.pas:1699 procedure TM2DataDB.Run; begin end;
        Assert.Equal(created, db.Created.Count);
        Assert.Equal(steps, db.Created.Sum(s => s.StepCount));
        Assert.Equal(0, host.LockCount);
    }

    // ==================================================================
    // LoadItemsFromDB（原文 1075-1292）
    // ==================================================================

    [Fact]
    public void LoadItemsFromDB_EmptyResultSet_YieldsNoItems()
    {
        var (unit, db, _) = NewSqlite();
        var list = new List<TUserItem>();

        unit.LoadItemsFromDB(100, 1, false, list);

        Assert.Empty(list);
        // 仍然绑了 (ParentID, ItemType) 并走了一次 Step。
        Assert.Equal(new[] { "100", "1" }, db.For("SelectItems").LastBinds.Select(b => b.Text));
        Assert.Equal(1, db.For("SelectItems").StepCount);
    }

    [Fact]
    public void LoadItemsFromDB_TwoRows_MapsTheOriginal27Columns()
    {
        var (unit, db, _) = NewSqlite();
        var stmt = db.For("SelectItems");
        stmt.AddRow(ItemRow(7, 4242, 9001, "屠龙", 33, 40));
        stmt.AddRow(ItemRow(8, 4243, 9002, "开天", 10, 12));
        var list = new List<TUserItem>();

        unit.LoadItemsFromDB(100, 1, false, list);

        Assert.Equal(2, list.Count);
        // 第 1 列 ItemIndex(7) 被读掉丢弃（原文如此，它只用于主键定位）；MakeIndex 是第 2 列。
        Assert.Equal(4242, list[0].MakeIndex);
        Assert.Equal("屠龙", list[0].NameStr);
        Assert.Equal((ushort)33, list[0].Dura);
        Assert.Equal((ushort)40, list[0].DuraMax);
        Assert.Equal(4243, list[1].MakeIndex);
        Assert.Equal("开天", list[1].NameStr);
    }

    [Fact]
    public void LoadItemsFromDB_IsSortTrue_UsesTheSortStatement()
    {
        var (unit, db, _) = NewSqlite();

        unit.LoadItemsFromDB(100, 1, true, new List<TUserItem>());

        Assert.Equal(0, db.For("SelectItems").StepCount);
        Assert.Equal(1, db.For("SelectItems_Sort").StepCount);
        Assert.Equal(new[] { "100", "1" }, db.For("SelectItems_Sort").LastBinds.Select(b => b.Text));
    }

    /// <summary>
    /// ★ 原文不一致（SqliteM2DataDB.pas:1219 vs 1294+）：<c>DoLoadItemsFromDB</c> 在 Flute 查询上
    /// 绑的是字面量 <c>0</c>，而 <c>DoLoadItemFromDB</c> 同位置绑 <c>ItemType</c>。本测试把差异钉住。
    /// </summary>
    [Fact]
    public void FluteBinding_DiffersBetweenLoadItemsAndLoadItem()
    {
        var (unit, db, _) = NewSqlite();
        db.For("SelectItems").AddRow(ItemRow(1, 1, 1, "x", 0, 0));

        unit.LoadItemsFromDB(100, 5, false, new List<TUserItem>());
        var fromItems = db.For("SelectitemFlute").LastBinds.Select(b => b.Text).ToArray();

        var item = new TUserItem();
        unit.LoadItemFromDB(ref item, 100, 5, 1);
        var fromItem = db.For("SelectitemFlute").LastBinds.Select(b => b.Text).ToArray();

        Assert.Equal(new[] { "100", "0", "1" }, fromItems);
        Assert.Equal(new[] { "100", "5", "1" }, fromItem);
    }

    [Fact]
    public void LoadItemsFromDB_SubTableRows_FillValueAddAndIgnoreOutOfRangeIndexes()
    {
        var (unit, db, _) = NewSqlite();
        db.For("SelectItems").AddRow(ItemRow(3, 30, 300, "n", 0, 0));
        // 正常下标 + 越界下标（负 / 超上界）——原文只写 Low..High 区间内的，越界行**读取后丢弃**。
        var valueAdd = db.For("SelectitemValueAdd");
        valueAdd.AddRow(0, 11);
        valueAdd.AddRow(13, 99);
        valueAdd.AddRow(-1, 7);
        valueAdd.AddRow(14, 8);
        var list = new List<TUserItem>();

        unit.LoadItemsFromDB(100, 1, false, list);

        TUserItem item = list[0];
        var copy = item;
        Assert.Equal(11, M2ItemDbAccess.GetValue(ref copy, 0));
        Assert.Equal(99, M2ItemDbAccess.GetValue(ref copy, 13));
        // 4 行结果集 → 4 次 SQLITE_ROW + 1 次 SQLITE_DONE。
        Assert.Equal(5, valueAdd.StepCount);
    }

    [Fact]
    public void LoadItemsFromDB_SqlInjectionCharacters_AreBoundAsText()
    {
        var (unit, db, _) = NewSqlite();
        string evil = "'; drop table Items; --";
        db.For("SelectItems").AddRow(ItemRow(1, 1, 1, evil, 0, 0));
        var list = new List<TUserItem>();

        unit.LoadItemsFromDB(100, 1, false, list);

        // 注入串作为**列值**原样进入 NameStr，从不进入 SQL 文本。
        Assert.Equal(evil, list[0].NameStr);
        Assert.DoesNotContain(evil, db.For("SelectItems").Sql, StringComparison.Ordinal);
    }

    // ==================================================================
    // LoadItemFromDB（原文 1294-1496）
    // ==================================================================

    [Fact]
    public void LoadItemFromDB_NoRow_LeavesTheItemZeroed()
    {
        var (unit, db, _) = NewSqlite();
        var item = new TUserItem { MakeIndex = 12345, Dura = 77 };

        unit.LoadItemFromDB(ref item, 9, 1, 2);

        // 原文 1300：FillChar(UserItem^, SizeOf(TUserItem), 0) —— 先清零再查；
        // 原文传 PTUserItem（指针），故清零结果对调用方可见。
        Assert.Equal(0, item.MakeIndex);
        Assert.Equal(0, item.Dura);
        Assert.Equal(1, db.For("SelectItem").StepCount);
    }

    [Fact]
    public void LoadItemFromDB_WithRow_MapsTheColumns()
    {
        var (unit, db, _) = NewSqlite();
        var row = new object?[27];
        row[0] = 555;          // MakeIndex
        row[1] = 601;          // DBIndex(wIndex)
        row[2] = "复活戒指";
        row[3] = 5;            // Dura
        row[4] = 6;            // DuraMax
        for (int i = 5; i < row.Length; i++) row[i] = 0;
        row[20] = "比奇省";
        row[21] = "";
        row[22] = "";
        row[23] = 0.0;
        db.For("SelectItem").AddRow(row);

        var item = new TUserItem();
        unit.LoadItemFromDB(ref item, 9, 1, 2);

        Assert.Equal(555, item.MakeIndex);
        Assert.Equal((ushort)601, item.wIndex);
        Assert.Equal("复活戒指", item.NameStr);
        Assert.Equal((ushort)5, item.Dura);
        Assert.Equal((ushort)6, item.DuraMax);
        Assert.Equal("比奇省", item.ItemFrom.MapName);
    }

    [Fact]
    public void LoadItemFromDB_NegativeAndZeroIdentifiers_AreBoundAsIs()
    {
        var (unit, db, _) = NewSqlite();
        var item = new TUserItem();

        unit.LoadItemFromDB(ref item, -1, 0, -99);

        // 原文不做任何范围校验，负值原样绑到语句上。
        Assert.Equal(new[] { "-1", "0", "-99" }, db.For("SelectItem").LastBinds.Select(b => b.Text));
    }

    [Fact]
    public void LoadItemFromDB_PropertyIndexOutOfRange_IsIgnored()
    {
        var (unit, db, _) = NewSqlite();
        var prop = db.For("SelectItemProperty");
        prop.AddRow(0, 1, 2, 3, 4, 5, 6, 7, 8);
        prop.AddRow(19, 1, 1, 1, 1, 1, 1, 1, 1);
        prop.AddRow(20, 9, 9, 9, 9, 9, 9, 9, 9);   // 越界：读掉但丢弃

        var item = new TUserItem();
        unit.LoadItemFromDB(ref item, 1, 1, 1);

        Assert.Equal(6, M2ItemDbAccess.GetProperty(ref item, 0).nValues0());
        Assert.Equal(1, M2ItemDbAccess.GetProperty(ref item, 19).nValues0());
    }

    /// <summary>语句抛异常时 <c>TM2DataDB.LoadItemFromDB</c> 的 try/except 吞掉并写日志（原文 1657-1661）；
    /// 这里用 <c>list = null</c> 触发同一条 except 路径（DoLoadItemsFromDB 内部的 NRE）。</summary>
    [Fact]
    public void LoadItemsFromDB_Exception_IsSwallowedAndLoggedWithTheOriginalMessage()
    {
        DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var (_, log) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();
        db.For("SelectItems").AddRow(ItemRow(1, 1, 1, "x", 0, 0));

        unit.LoadItemsFromDB(1, 1, false, null!);   // 不得抛出

        Assert.Contains(log.Messages, m => m.StartsWith("[Exception] TM2DataDB:LoadItemsFromDB;", StringComparison.Ordinal));
    }

    // ==================================================================
    // SaveItemToDB（原文 1498-1701）
    // ==================================================================

    [Fact]
    public void SaveItemToDB_ZeroItem_InsertsOnlyTheItemsRow()
    {
        var (unit, db, _) = NewSqlite();

        unit.SaveItemToDB(new TUserItem(), 10, 1, 20);

        Assert.Equal(1, db.For("InsertItems").StepCount);
        Assert.Equal(30, db.For("InsertItems").LastBinds.Count);   // Items 表 30 个占位符
        Assert.Equal(new[] { "10", "1", "20" }, db.For("InsertItems").LastBinds.Take(3).Select(b => b.Text));
        // 定长数组全 0 → 8 个附表一条都不插。
        foreach (string label in new[]
                 {
                     "InsertItemValueAdd", "InsertItemElementAdd", "InsertItemAddDataByte", "InsertItemAddDataInt",
                     "InsertItemAddDataText", "InsertItemFlute", "InsertItemProgress", "InsertItemProperty",
                 })
        {
            Assert.Equal(0, db.For(label).StepCount);
            Assert.Equal(1, db.For(label).ResetCount);   // finally 里仍然 Reset 一次
        }
    }

    [Fact]
    public void SaveItemToDB_NonZeroArraySlots_InsertOnlyThoseRows()
    {
        var (unit, db, _) = NewSqlite();
        var item = new TUserItem();
        var copy = item;
        M2ItemDbAccess.SetValue(ref copy, 0, 5);
        M2ItemDbAccess.SetValue(ref copy, 13, 7);
        M2ItemDbAccess.SetNewValue(ref copy, 29, 4);
        M2ItemDbAccess.SetAddDataByte(ref copy, 19, 3);
        M2ItemDbAccess.SetAddDataInt(ref copy, 9, 2);
        M2ItemDbAccess.SetAddDataText(ref copy, 1, "abc");
        item = copy;

        unit.SaveItemToDB(item, 1, 1, 1);

        var v = db.For("InsertItemValueAdd");
        // btValue 有 2 个非零槽（0 与 13）⇒ 插 2 行；LastBinds 是最后一次 Step（j=13）的快照。
        Assert.Equal(2, v.StepCount);
        Assert.Equal(new[] { "1", "1", "1", "13", "7" }, v.LastBinds.Select(b => b.Text));

        var e = db.For("InsertItemElementAdd");
        Assert.Equal(1, e.StepCount);
        Assert.Equal(new[] { "1", "1", "1", "29", "4" }, e.LastBinds.Select(b => b.Text));

        Assert.Equal(1, db.For("InsertItemAddDataByte").StepCount);
        Assert.Equal(1, db.For("InsertItemAddDataInt").StepCount);

        var t = db.For("InsertItemAddDataText");
        Assert.Equal(1, t.StepCount);
        Assert.Equal("abc", t.LastBinds[4].Text);

        // SaveItemToDB 每个附表在 finally 里 Reset；2 个非零槽 ⇒ 循环内 2 次 + finally 1 次。
        Assert.Equal(3, v.ResetCount);
    }

    [Fact]
    public void SaveItemToDB_ProgressIsInsertedOnlyWhenBoOpenIsSet()
    {
        var (unit, db, _) = NewSqlite();
        var item = new TUserItem();
        var copy = item;

        // 槽 0：boOpen = 0，但其它字段非 0 → 原文判据是 boOpen，**不插**。
        var p0 = M2ItemDbAccess.GetProgress(ref copy, 0);
        p0.boOpen = 0;
        p0.wValue = 123;
        p0.NameStr = "不应插入";
        M2ItemDbAccess.SetProgress(ref copy, 0, p0);

        // 槽 1：boOpen = 1 → 插。
        var p1 = M2ItemDbAccess.GetProgress(ref copy, 1);
        p1.boOpen = 1;
        p1.btNameColor = 2;
        p1.btCount = 3;
        p1.btShowType = 4;
        p1.wMax = 5;
        p1.wValue = 6;
        p1.wLevel = 7;
        p1.NameStr = "进度";
        M2ItemDbAccess.SetProgress(ref copy, 1, p1);
        item = copy;

        unit.SaveItemToDB(item, 2, 1, 3);

        var stmt = db.For("InsertItemProgress");
        Assert.Equal(1, stmt.StepCount);
        Assert.Equal(new[] { "2", "1", "3", "1", "True", "2", "3", "4", "5", "6", "7", "进度" },
            stmt.LastBinds.Select(b => b.Text));
    }

    [Fact]
    public void SaveItemToDB_PropertyIsInsertedWhenAnyValueIsPositive()
    {
        var (unit, db, _) = NewSqlite();
        var item = new TUserItem();
        var copy = item;

        // 槽 0：三个 nValues 全 0（即使颜色等非 0）→ 不插。
        var q0 = M2ItemDbAccess.GetProperty(ref copy, 0);
        q0.btColor = 9;
        q0.SetValues(new[] { 0, 0, 0 });
        M2ItemDbAccess.SetProperty(ref copy, 0, q0);

        // 槽 1：nValues[2] = 1 → 插。
        var q1 = M2ItemDbAccess.GetProperty(ref copy, 1);
        q1.btColor = 1;
        q1.btBindType = 2;
        q1.btShowFlag = 3;
        q1.btPercent = 4;
        q1.btHintModule = 5;
        q1.SetValues(new[] { 10, 20, 30 });
        M2ItemDbAccess.SetProperty(ref copy, 1, q1);
        item = copy;

        unit.SaveItemToDB(item, 4, 1, 5);

        var stmt = db.For("InsertItemProperty");
        Assert.Equal(1, stmt.StepCount);
        Assert.Equal(new[] { "4", "1", "5", "1", "1", "2", "3", "4", "5", "10", "20", "30" },
            stmt.LastBinds.Select(b => b.Text));
    }

    [Fact]
    public void SaveItemToDB_FluteIsInsertedOnlyWhenGemIndexIsNonZero()
    {
        var (unit, db, _) = NewSqlite();
        var item = new TUserItem();
        var copy = item;

        var f0 = copy.GetFlute(0);
        f0.GemIndex = 0;
        f0.GemCount = 9;          // GemIndex = 0 → 不插
        copy.SetFlute(0, f0);

        var f7 = copy.GetFlute(7);
        f7.GemIndex = 88;
        f7.GemCount = 2;
        copy.SetFlute(7, f7);
        item = copy;

        unit.SaveItemToDB(item, 6, 1, 7);

        var stmt = db.For("InsertItemFlute");
        Assert.Equal(1, stmt.StepCount);
        Assert.Equal(new[] { "6", "1", "7", "7", "88", "2" }, stmt.LastBinds.Select(b => b.Text));
    }

    [Fact]
    public void SaveItemToDB_SqlInjectionInTextFields_IsBoundAsParameter()
    {
        var (unit, db, _) = NewSqlite();
        string evil = "x'); DELETE FROM Items; --";
        var item = new TUserItem { NameStr = evil };
        var copy = item;
        M2ItemDbAccess.SetAddDataText(ref copy, 0, evil);
        item = copy;

        unit.SaveItemToDB(item, 1, 1, 1);

        // NameStr 是 string[60]，装得下；sAddDataText 是 string[20]（原文），按 GBK 字节截断。
        Assert.Equal(evil, db.For("InsertItems").LastBinds[5].Text);
        string truncated = M2ItemDbAccess.GetAddDataText(ref copy, 0);
        Assert.NotEqual(evil, truncated);
        Assert.Equal(truncated, db.For("InsertItemAddDataText").LastBinds[4].Text);
        // 注入串始终作为**参数值**，从不进入 SQL 文本。
        Assert.DoesNotContain("DELETE FROM Items", db.For("InsertItems").Sql, StringComparison.Ordinal);
        Assert.DoesNotContain("DELETE FROM Items", db.For("InsertItemAddDataText").Sql, StringComparison.Ordinal);
    }

    [Fact]
    public void SaveItemToDB_AllArraySlotsFilled_InsertsEveryArrayRow()
    {
        var (unit, db, _) = NewSqlite();
        var copy = new TUserItem();
        for (int j = 0; j <= 13; j++) M2ItemDbAccess.SetValue(ref copy, j, 1);
        for (int j = 0; j <= 29; j++) M2ItemDbAccess.SetNewValue(ref copy, j, 1);
        for (int j = 0; j <= 19; j++) M2ItemDbAccess.SetAddDataByte(ref copy, j, 1);
        for (int j = 0; j <= 9; j++) M2ItemDbAccess.SetAddDataInt(ref copy, j, 1);
        for (int j = 0; j <= 1; j++) M2ItemDbAccess.SetAddDataText(ref copy, j, "t");

        unit.SaveItemToDB(copy, 1, 1, 1);

        Assert.Equal(14, db.For("InsertItemValueAdd").StepCount);
        Assert.Equal(30, db.For("InsertItemElementAdd").StepCount);
        Assert.Equal(20, db.For("InsertItemAddDataByte").StepCount);
        Assert.Equal(10, db.For("InsertItemAddDataInt").StepCount);
        Assert.Equal(2, db.For("InsertItemAddDataText").StepCount);
    }

    // ==================================================================
    // MySQL 姊妹实现（只写"看起来一样实则不同"的差异断言 + 原文缺陷）
    // ==================================================================

    [Fact]
    public void MySql_DoFinal_HasTheSameMissingFinalizeDefect()
    {
        DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlM2DataDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.DoFinal();

        Assert.Equal(0, db.For("SelectItems").FinalizeCount);
        Assert.Equal(0, db.For("SelectItems_Sort").FinalizeCount);
    }

    [Fact]
    public void MySql_DoInit_RegistersTheSameTwentyLabelsAsSqlite()
    {
        DbLayerTestKit.Isolate();
        var sqliteDb = new FakeSqliteDatabase();
        var mySqlDb = new FakeMySqlDatabase();
        IsolateSqliteFiles(fileExists: false);
        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = sqliteDb }, sqliteDb).DoInit();
        new TMySqlM2DataDB(new FakeDbLayerHost { DataBase = mySqlDb }, mySqlDb).DoInit();

        Assert.Equal(sqliteDb.AddedNames, mySqlDb.AddedNames);
    }
}

/// <summary>测试用的小扩展：读 <c>TCustomProperty.nValues[0]</c>（托管侧 <c>nValues</c> 是 fixed buffer）。</summary>
internal static class CustomPropertyTestExtensions
{
    public static int nValues0(this TCustomProperty p) => p.GetValues()[0];
}

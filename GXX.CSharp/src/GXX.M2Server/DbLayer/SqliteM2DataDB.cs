// 源单元：Source/M2Engine/SqliteM2DataDB.pas（1-1708 行，1:1 移植）
//   TSqliteM2DataDB = class(TM2DataDB)：Create/Destroy、GetAuctionDBClass/GetStorageDBClass/
//   GetUserShopDBClass、DoInit、UpdateDB_1..6、DoUpdate、DoFinal、DoLoadItemsFromDB、
//   DoLoadItemFromDB、DoSaveItemToDB、GetDataBase。
//
// SQL 逐字保真：20 条 <c>AddSQLStatement</c> 语句在 SqlStatements.SqliteM2DataDB.cs，
// 方法级脚本（7 段迁移 DDL + 5 条 DoUpdate 动态语句）在 SqlStatements.SqliteM2DataDB.Scripts.cs
// 与 SqlStatements.SqliteM2DataDB.Scripts2.cs —— 全部由 _recon/p3-sql.mjs 从 GBK 原文机械抽取，
// **零手工转录**。测试 DbLayerM2DataSqlFidelityTests 把实现真正绑上去的 SQL 取 SHA-256 与
// DbLayerFingerprints.SqliteM2DataDB.cs 的指纹表比对。
//
// 方言要点（对照 MySqlM2DataDB）：
//   * 事务 BeginTransaction/Commit/RollBack；批量脚本 Execute(sql)；
//   * 结果集 `Ret := Step; while Ret = SQLITE_ROW do ... Ret := Step;`，单行 `if Step = SQLITE_ROW`；
//   * 无结果集成功判据（原文这里未判）—— DoAddAuctionItem 等用的是 `Step in [SQLITE_OK, SQLITE_DONE]`；
//   * 迁移是"版本号分支 + UpdateDB_N 逐步执行"；`S` 用 sLineBreak（Windows = #13#10）拼接；
//   * DoUpdate 末尾用同名 `temp` 语句两轮（原文如此：AddSQLStatement('temp') 同名复用）。
//
// 原文缺陷/易错点（逐字保留，测试锁定；详见 docs/并行报告-p3-m2-dbdata.md §7）：
//   * DoFinal **不 Finalize** FStatementGetItems / FStatementGetItems_Sort（原文如此，
//     SqliteM2DataDB.pas:963-1073 里没有它们的 Finalize 分支）；
//   * DoUpdate 的两条 `temp` 语句用同一个 label，SQLite3DataBase 同名复用返回同一实例；
//   * Flute 读取在 DoLoadItemsFromDB 用 `OrderBindInt(0)` 绑 ItemType，
//     而 DoLoadItemFromDB 同位置绑 `ItemType`（Sqlite 1708 行版，原文不一致）；
//   * DoSaveItemToDB 的 Items 行插入在 `finally` 里 Reset（异常路径也会 Reset）。

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.DbLayer;

/// <summary>SqliteM2DataDB.pas:24-74 <c>TSqliteM2DataDB</c>。</summary>
public sealed class TSqliteM2DataDB : IM2DataDb
{
    private ISqliteDatabase? _fdb;

    private ISqliteStatement? _fStatementInsertItems;
    private ISqliteStatement? _fStatementInsertItemValueAdd;
    private ISqliteStatement? _fStatementInsertItemElementAdd;
    private ISqliteStatement? _fStatementInsertItemAddDataByte;
    private ISqliteStatement? _fStatementInsertItemAddDataInt;
    private ISqliteStatement? _fStatementInsertItemAddDataText;
    private ISqliteStatement? _fStatementInsertItemFlute;
    private ISqliteStatement? _fStatementInsertItemProgress;
    private ISqliteStatement? _fStatementInsertItemProperty;

    private ISqliteStatement? _fStatementGetItems;
    private ISqliteStatement? _fStatementGetItems_Sort;

    private ISqliteStatement? _fStatementGetItem;
    private ISqliteStatement? _fStatementGetItemValueAdd;
    private ISqliteStatement? _fStatementGetItemElementAdd;
    private ISqliteStatement? _fStatementGetItemAddDataByte;
    private ISqliteStatement? _fStatementGetItemAddDataInt;
    private ISqliteStatement? _fStatementGetItemAddDataText;
    private ISqliteStatement? _fStatementGetItemFlute;
    private ISqliteStatement? _fStatementGetItemProgress;
    private ISqliteStatement? _fStatementGetItemProperty;

    private readonly IDbLayerHost _owner;

    /// <summary>SqliteM2DataDB.pas:15 <c>SQLITE_M2DBVERSION = '20200916'</c>（在 SqliteCreateTableSql.pas）。</summary>
    public const string SQLITE_M2DBVERSION = "20200916";

    /// <summary>SqliteM2DataDB.pas:144 库文件名（M2Data\M2Data.DB）。</summary>
    public const string M2DataFileName = "M2Data.DB";

    /// <summary>SqliteM2DataDB.pas:150 资源名（TResourceStream.Create(HInstance, 'M2Data', PChar('SQLITEDB'))）。</summary>
    public const string M2DataResourceName = "M2Data";

    /// <summary>SqliteM2DataDB.pas:166 SQLite 错误码 11（SQLITE_CORRUPT）→ 删 shm/wal 重连。</summary>
    public const int SQLITE_ERROR_CODE_CORRUPT = 11;

    /// <summary>SqliteM2DataDB.pas:83-110 <c>constructor Create</c>：FDB 新建，21 个语句字段置 nil。</summary>
    public TSqliteM2DataDB(IDbLayerHost owner, ISqliteDatabase? db = null)
    {
        _owner = owner;
        _fdb = db ?? UnavailableSqliteDatabase.Instance;

        _fStatementInsertItems = null;
        _fStatementInsertItemValueAdd = null;
        _fStatementInsertItemElementAdd = null;
        _fStatementInsertItemAddDataByte = null;
        _fStatementInsertItemAddDataInt = null;
        _fStatementInsertItemAddDataText = null;
        _fStatementInsertItemFlute = null;
        _fStatementInsertItemProgress = null;
        _fStatementInsertItemProperty = null;

        _fStatementGetItems = null;
        _fStatementGetItems_Sort = null;
        _fStatementGetItem = null;
        _fStatementGetItemValueAdd = null;
        _fStatementGetItemElementAdd = null;
        _fStatementGetItemAddDataByte = null;
        _fStatementGetItemAddDataInt = null;
        _fStatementGetItemAddDataText = null;
        _fStatementGetItemFlute = null;
        _fStatementGetItemProgress = null;
        _fStatementGetItemProperty = null;
    }

    /// <summary>SqliteM2DataDB.pas:112-116 <c>destructor Destroy</c>（原文 FDB.Free；托管侧驱动器由装配方持有）。</summary>
    public void Destroy()
    {
        _fdb = null;
    }

    /// <summary>SqliteM2DataDB.pas:118-121 <c>GetAuctionDBClass</c> → <c>TSqliteAuctionDB</c>。</summary>
    protected Type GetAuctionDBClass() => typeof(TSqliteAuctionDB);

    /// <summary>SqliteM2DataDB.pas:123-126 <c>GetStorageDBClass</c> → <c>TSqliteStorageDB</c>（未移植，见报告 §8）。</summary>
    protected Type GetStorageDBClass() => typeof(object);

    /// <summary>SqliteM2DataDB.pas:128-131 <c>GetUserShopDBClass</c> → <c>TSqliteUserShopDB</c>。</summary>
    protected Type GetUserShopDBClass() => typeof(TSqliteUserShopDB);

    /// <summary>M2DataCommon.pas:480 <c>property Owner: TM2DataDB read FOwner;</c>。</summary>
    public IDbLayerHost Owner => _owner;

    /// <summary>SqliteM2DataDB.pas:1703-1706 <c>GetDataBase: TObject</c>。</summary>
    public object? DataBase => _fdb;

    /// <summary>M2DataCommon.pas:487-488 的三个子库属性（本车道只装配 AuctionDB/UserShopDB 的类型）。</summary>
    public IAuctionDb? AuctionDB { get; set; }

    /// <summary>M2DataCommon.pas:488 <c>property UserShopDB: TUserShopDB</c>。</summary>
    public IUserShopDb? UserShopDB { get; set; }

    /// <summary>M2DataCommon.pas:501 <c>property IsInitOK: Boolean</c>。</summary>
    public bool IsInitOK { get; private set; }

    /// <summary>底层数据库（测试/装配用）。</summary>
    public ISqliteDatabase? Database => _fdb;

    // ------------------------------------------------------------------
    // DoInit（原文 133-336）
    // ------------------------------------------------------------------

    /// <summary>
    /// SqliteM2DataDB.pas:133-336 <c>DoInit</c>。
    /// 目录/文件准备 → 连接（错误码 11 时删 shm/wal 重连）→ PRAGMA → DoUpdate / 建表 → 20 条语句注册+Prepare。
    /// </summary>
    public void DoInit()
    {
        if (_fdb is null) return;

        // 140-142：Dir := ExtractFilePath(ParamStr(0)) + 'M2Data\'; if not DirectoryExists(Dir) then ForceDirectories(Dir);
        string dir = ExtractFilePath() + "M2Data\\";
        if (!DirectoryExists(dir)) ForceDirectories(dir);

        // 144：FileName := Dir + 'M2Data.DB';
        string fileName = dir + M2DataFileName;

        // 146-152：多线程模式；库文件不存在时从资源流出释放
        Sqlite3ConfigMultithread();
        if (!FileExists(fileName)) SaveResourceToFile(M2DataResourceName, fileName);

        if (FileExists(fileName))
        {
            // 156-158
            _fdb.MustExist = true;
            _fdb.Database = fileName;
            _fdb.UseThreadMode = true;

            // 160-175
            bool isTryAgain = false;
            try
            {
                _fdb.Connected = true;
            }
            catch (Exception e)
            {
                if (_fdb.ErrorCode == SQLITE_ERROR_CODE_CORRUPT)
                {
                    isTryAgain = true;
                }
                else
                {
                    // 172：raise Exception.Create(E.Message);
                    throw new Exception(e.Message);
                }
            }

            // 177-192
            if (isTryAgain)
            {
                fileName = dir + M2DataFileName + "-shm";
                if (FileExists(fileName)) DeleteFile(fileName);

                fileName = dir + M2DataFileName + "-wal";
                if (FileExists(fileName)) DeleteFile(fileName);

                _fdb.Connected = true;
            }

            // 194-199（194 与 199 两条被原文注释掉）
            // FDB.Execute('PRAGMA synchronous = NORMAL;');
            _fdb.Execute("PRAGMA cache_size = 32768;");
            _fdb.Execute("PRAGMA mmap_size = 32768");
            _fdb.Execute("PRAGMA locking_mode = EXCLUSIVE;");
            _fdb.Execute("PRAGMA journal_mode = WAL;");
            // FDB.Execute('PRAGMA temp_store = MEMORY;');

            // 201
            DoUpdate();
        }
        else
        {
            // 205-218
            _fdb.MustExist = false;
            _fdb.Database = fileName;
            _fdb.Connected = true;
            _fdb.BeginTransaction();
            try
            {
                _fdb.Execute(SqliteCreateTableSql.SQLITE_CREATE_M2DATA_TABLES);
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }

        // 222-335：物品表语句
        _fStatementInsertItems = _fdb.AddSQLStatement("InsertItems");
        _fStatementInsertItems.Sql = SqliteM2DataDbStatements.InsertItems;
        _fStatementInsertItems.Prepare();

        _fStatementInsertItemValueAdd = _fdb.AddSQLStatement("InsertItemValueAdd");
        _fStatementInsertItemValueAdd.Sql = SqliteM2DataDbStatements.InsertItemValueAdd;
        _fStatementInsertItemValueAdd.Prepare();

        _fStatementInsertItemElementAdd = _fdb.AddSQLStatement("InsertItemElementAdd");
        _fStatementInsertItemElementAdd.Sql = SqliteM2DataDbStatements.InsertItemElementAdd;
        _fStatementInsertItemElementAdd.Prepare();

        _fStatementInsertItemAddDataByte = _fdb.AddSQLStatement("InsertItemAddDataByte");
        _fStatementInsertItemAddDataByte.Sql = SqliteM2DataDbStatements.InsertItemAddDataByte;
        _fStatementInsertItemAddDataByte.Prepare();

        _fStatementInsertItemAddDataInt = _fdb.AddSQLStatement("InsertItemAddDataInt");
        _fStatementInsertItemAddDataInt.Sql = SqliteM2DataDbStatements.InsertItemAddDataInt;
        _fStatementInsertItemAddDataInt.Prepare();

        _fStatementInsertItemAddDataText = _fdb.AddSQLStatement("InsertItemAddDataText");
        _fStatementInsertItemAddDataText.Sql = SqliteM2DataDbStatements.InsertItemAddDataText;
        _fStatementInsertItemAddDataText.Prepare();

        _fStatementInsertItemFlute = _fdb.AddSQLStatement("InsertItemFlute");
        _fStatementInsertItemFlute.Sql = SqliteM2DataDbStatements.InsertItemFlute;
        _fStatementInsertItemFlute.Prepare();

        _fStatementInsertItemProgress = _fdb.AddSQLStatement("InsertItemProgress");
        _fStatementInsertItemProgress.Sql = SqliteM2DataDbStatements.InsertItemProgress;
        _fStatementInsertItemProgress.Prepare();

        _fStatementInsertItemProperty = _fdb.AddSQLStatement("InsertItemProperty");
        _fStatementInsertItemProperty.Sql = SqliteM2DataDbStatements.InsertItemProperty;
        _fStatementInsertItemProperty.Prepare();

        _fStatementGetItems = _fdb.AddSQLStatement("SelectItems");
        _fStatementGetItems.Sql = SqliteM2DataDbStatements.SelectItems;
        _fStatementGetItems.Prepare();

        _fStatementGetItems_Sort = _fdb.AddSQLStatement("SelectItems_Sort");
        _fStatementGetItems_Sort.Sql = SqliteM2DataDbStatements.SelectItems_Sort;
        _fStatementGetItems_Sort.Prepare();

        _fStatementGetItem = _fdb.AddSQLStatement("SelectItem");
        _fStatementGetItem.Sql = SqliteM2DataDbStatements.SelectItem;
        _fStatementGetItem.Prepare();

        _fStatementGetItemValueAdd = _fdb.AddSQLStatement("SelectitemValueAdd");
        _fStatementGetItemValueAdd.Sql = SqliteM2DataDbStatements.SelectitemValueAdd;
        _fStatementGetItemValueAdd.Prepare();

        _fStatementGetItemElementAdd = _fdb.AddSQLStatement("SelectitemElementAdd");
        _fStatementGetItemElementAdd.Sql = SqliteM2DataDbStatements.SelectitemElementAdd;
        _fStatementGetItemElementAdd.Prepare();

        _fStatementGetItemAddDataByte = _fdb.AddSQLStatement("SelectitemAddDataByte");
        _fStatementGetItemAddDataByte.Sql = SqliteM2DataDbStatements.SelectitemAddDataByte;
        _fStatementGetItemAddDataByte.Prepare();

        _fStatementGetItemAddDataInt = _fdb.AddSQLStatement("SelectitemAddDataInt");
        _fStatementGetItemAddDataInt.Sql = SqliteM2DataDbStatements.SelectitemAddDataInt;
        _fStatementGetItemAddDataInt.Prepare();

        _fStatementGetItemAddDataText = _fdb.AddSQLStatement("SelectitemAddDataText");
        _fStatementGetItemAddDataText.Sql = SqliteM2DataDbStatements.SelectitemAddDataText;
        _fStatementGetItemAddDataText.Prepare();

        _fStatementGetItemFlute = _fdb.AddSQLStatement("SelectitemFlute");
        _fStatementGetItemFlute.Sql = SqliteM2DataDbStatements.SelectitemFlute;
        _fStatementGetItemFlute.Prepare();

        _fStatementGetItemProgress = _fdb.AddSQLStatement("SelectItemProgress");
        _fStatementGetItemProgress.Sql = SqliteM2DataDbStatements.SelectItemProgress;
        _fStatementGetItemProgress.Prepare();

        _fStatementGetItemProperty = _fdb.AddSQLStatement("SelectItemProperty");
        _fStatementGetItemProperty.Sql = SqliteM2DataDbStatements.SelectItemProperty;
        _fStatementGetItemProperty.Prepare();

        // ★ 原文 SqliteM2DataDB.pas:133-336 的 DoInit **不设** FIsInitOK；
        //   FIsInitOK 由 TM2DataDB.Init（M2DataCommon.pas:1638）设置，见本文件 Init()。
    }

    // ------------------------------------------------------------------
    // UpdateDB_1 .. UpdateDB_6（原文 338-473）
    // ------------------------------------------------------------------

    /// <summary>SqliteM2DataDB.pas:338-348 <c>UpdateDB_1</c>。</summary>
    private void UpdateDB_1()
    {
        string s =
            "ALTER TABLE ItemProperty ADD COLUMN \"Value2\" INTEGER DEFAULT 0;" + SLB +
            "ALTER TABLE ItemProperty ADD COLUMN \"Value3\" INTEGER DEFAULT 0;" + SLB +
            "UPDATE db_constant set ConstValue = \"" + SQLITE_M2DBVERSION + "\" where ConstName = \"version\"";

        _fdb!.Execute(s);
    }

    /// <summary>SqliteM2DataDB.pas:350-410 <c>UpdateDB_2</c>。</summary>
    private void UpdateDB_2()
    {
        string s = SqliteM2DataDbScripts.UpdateDB_2_L354_S;

        _fdb!.Execute(s);
    }

    /// <summary>SqliteM2DataDB.pas:412-421 <c>UpdateDB_3</c>。</summary>
    private void UpdateDB_3()
    {
        string s =
            "ALTER TABLE ItemFlute ADD COLUMN \"OverlapCount\" INTEGER DEFAULT 0;" + SLB +
            "UPDATE db_constant set ConstValue = \"" + SQLITE_M2DBVERSION + "\" where ConstName = \"version\"";

        _fdb!.Execute(s);
    }

    /// <summary>SqliteM2DataDB.pas:423-433 <c>UpdateDB_4</c>。</summary>
    private void UpdateDB_4()
    {
        string s =
            "ALTER TABLE Items ADD COLUMN \"NewExpand3\" INTEGER DEFAULT 0;" + SLB +
            "ALTER TABLE Items ADD COLUMN \"NewExpand4\" INTEGER DEFAULT 0;" + SLB +
            "UPDATE db_constant set ConstValue = \"" + SQLITE_M2DBVERSION + "\" where ConstName = \"version\"";

        _fdb!.Execute(s);
    }

    /// <summary>SqliteM2DataDB.pas:435-443 <c>UpdateDB_5</c>。</summary>
    private void UpdateDB_5()
    {
        string s =
            "ALTER TABLE ItemProperty ADD COLUMN \"HintModule\" INTEGER DEFAULT 0;" + SLB +
            "UPDATE db_constant set ConstValue = \"" + SQLITE_M2DBVERSION + "\" where ConstName = \"version\"";

        _fdb!.Execute(s);
    }

    /// <summary>SqliteM2DataDB.pas:445-473 <c>UpdateDB_6</c>。</summary>
    private void UpdateDB_6()
    {
        string s = SqliteM2DataDbScripts.UpdateDB_6_L449_S;

        _fdb!.Execute(s);
    }

    /// <summary>原文 <c>sLineBreak</c>（SysUtils，Windows = #13#10）。</summary>
    private const string SLB = "\r\n";

    // ------------------------------------------------------------------
    // DoUpdate（原文 475-961）
    // ------------------------------------------------------------------

    /// <summary>SqliteM2DataDB.pas:475-961 <c>DoUpdate</c>：按 db_constant.version 分支做迁移，
    /// 再回填 UserShopItem / AuctionData 的 ItemDBName/ItemName。</summary>
    private void DoUpdate()
    {
        // 493-501
        ISqliteStatement sm = _fdb!.AddSQLStatement("get_db_constant_value");
        sm.Sql = SqliteM2DataDbScripts.DoUpdate_L494_sm_Sql;
        sm.Prepare();
        int dbVersion = 0;
        if (sm.Step() == SqliteCodes.SQLITE_ROW)
        {
            dbVersion = sm.OrderGetColumnValueInt;
        }
        sm.StatementFinalize();

        // 503-869：版本分支（逐字保留原文的 if/else if 顺序与 UpdateDB_N 调用集合）
        if (dbVersion == 20170506)
        {
            _fdb.BeginTransaction();
            try
            {
                string s = MigrateScript_20170506();

                _fdb.Execute(s);

                UpdateDB_1();
                UpdateDB_3();
                UpdateDB_5();
                UpdateDB_6();

                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion == 20170603)
        {
            _fdb.BeginTransaction();
            try
            {
                string s = MigrateScript_20170603();

                _fdb.Execute(s);

                UpdateDB_1();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();

                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion == 20170610)
        {
            _fdb.BeginTransaction();
            try
            {
                string s = MigrateScript_20170610();

                _fdb.Execute(s);

                UpdateDB_1();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();

                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion == 20170701)
        {
            _fdb.BeginTransaction();
            try
            {
                string s = MigrateScript_20170701();

                _fdb.Execute(s);

                UpdateDB_1();
                UpdateDB_2();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();

                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion == 20180512)
        {
            // 本版本所有拍卖货币只支持一种类型CurrencyType均填0，2018-06-13要支持多种类型，所以要修改数据
            _fdb.BeginTransaction();
            try
            {
                string s = MigrateScript_20180512();

                _fdb.Execute(s);

                UpdateDB_1();
                UpdateDB_2();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();

                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }

        // 原文 755 行有一个多余空行（原文如此），语义不变。
        else if (dbVersion == 20180613)
        {
            // 本版本所有拍卖货币只支持一种类型CurrencyType均填0，2018-06-13要支持多种类型，所以要修改数据
            _fdb.BeginTransaction();
            try
            {
                string s = MigrateScript_20180613();

                _fdb.Execute(s);

                UpdateDB_1();
                UpdateDB_2();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();

                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion == 20180615)
        {
            _fdb.BeginTransaction();
            try
            {
                UpdateDB_1();
                UpdateDB_2();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion <= 20190318)
        {
            _fdb.BeginTransaction();
            try
            {
                UpdateDB_2();
                UpdateDB_3();
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion <= 20190606)
        {
            _fdb.BeginTransaction();
            try
            {
                UpdateDB_4();
                UpdateDB_5();
                UpdateDB_6();
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion <= 20190928)
        {
            _fdb.BeginTransaction();
            try
            {
                UpdateDB_5();
                UpdateDB_6();
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }
        else if (dbVersion <= 20200813)
        {
            _fdb.BeginTransaction();
            try
            {
                UpdateDB_6();
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }
        }

        // 871-961：回填 ItemDBName / ItemName
        var list = new List<ItemInfo>();
        try
        {
            // 873-888
            sm = _fdb.AddSQLStatement("temp");
            sm.Sql = SqliteM2DataDbScripts.DoUpdate_L874_sm_Sql;
            sm.Prepare();
            int ret = sm.Step();
            while (ret == SqliteCodes.SQLITE_ROW)
            {
                var itemInfo = new ItemInfo
                {
                    ParentID = sm.OrderGetColumnValueInt,
                };
                itemInfo.ItemID = sm.OrderGetColumnValueInt;
                itemInfo.DBIndex = sm.OrderGetColumnValueInt;
                itemInfo.Name = sm.OrderGetColumnValueText;
                list.Add(itemInfo);
                ret = sm.Step();
            }
            sm.Reset();
            sm.StatementFinalize();

            // 890-915
            sm.Sql = SqliteM2DataDbScripts.DoUpdate_L890_sm_Sql;
            sm.Prepare();
            _fdb.BeginTransaction();
            try
            {
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    ItemInfo itemInfo = list[i];
                    sm.Reset();
                    sm.OrderBindText(ItemNameOf(itemInfo.DBIndex));
                    sm.OrderBindText(ProcessItemName(itemInfo.Name));
                    sm.OrderBindInt(itemInfo.ParentID);
                    sm.OrderBindInt(itemInfo.ItemID);
                    sm.Step();
                }
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }

            sm.StatementFinalize();
            list.Clear();

            // 917-932（原文再次 AddSQLStatement('temp')，同名语句复用）
            sm = _fdb.AddSQLStatement("temp");
            sm.Sql = SqliteM2DataDbScripts.DoUpdate_L918_sm_Sql;
            sm.Prepare();
            ret = sm.Step();
            while (ret == SqliteCodes.SQLITE_ROW)
            {
                var itemInfo = new ItemInfo
                {
                    ParentID = sm.OrderGetColumnValueInt,
                };
                itemInfo.ItemID = sm.OrderGetColumnValueInt;
                itemInfo.DBIndex = sm.OrderGetColumnValueInt;
                itemInfo.Name = sm.OrderGetColumnValueText;
                list.Add(itemInfo);
                ret = sm.Step();
            }
            sm.Reset();
            sm.StatementFinalize();

            // 934-955
            sm.Sql = SqliteM2DataDbScripts.DoUpdate_L934_sm_Sql;
            sm.Prepare();
            _fdb.BeginTransaction();
            try
            {
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    ItemInfo itemInfo = list[i];
                    sm.Reset();
                    sm.OrderBindText(ItemNameOf(itemInfo.DBIndex));
                    sm.OrderBindText(ProcessItemName(itemInfo.Name));
                    sm.OrderBindInt(itemInfo.ParentID);
                    sm.Step();
                }
                _fdb.Commit();
            }
            catch (Exception e)
            {
                _fdb.RollBack();
                DbLayerGlobals.MainOutMessage(e.Message);
            }

            sm.StatementFinalize();
        }
        finally
        {
            // 959：List.Free（托管侧交给 GC；Dispose(ItemInfo) 语义即释放引用）
        }
    }

    /// <summary>SqliteM2DataDB.pas:479-484 <c>TItemInfo</c>（局部记录类型）。</summary>
    private sealed class ItemInfo
    {
        public int ParentID;
        public int ItemID;
        public int DBIndex;
        public string Name = "";
    }

    /// <summary>SqliteM2DataDB.pas:544/604/648/691/734 的 <c>IntToStr(g_Config.nAuctionCurrencyType)</c>。</summary>
    private static string IntToStr(int value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>SqliteM2DataDB.pas:898/942 <c>UserEngine.GetStdItemName(DBIndex)</c>。接缝：待 UsrEngn 移植后接入。</summary>
    private static string ItemNameOf(int dbIndex) => DbLayerRunSeam.GetStdItemName(dbIndex);

    /// <summary>SqliteM2DataDB.pas:899/943 <c>ProcessItemName(Name)</c>。</summary>
    private static string ProcessItemName(string name) => DbLayerRunSeam.ProcessItemName(name);

    // 507-547 / 569-607 / 630-651 / 674-694 / 719-735 / 761-768 的 6 段迁移脚本。
    // ★ 全部由 _recon/p3-sql.mjs 从 GBK 原文机械抽取（SqlStatements.SqliteM2DataDB.Scripts.cs），
    //   这里只做"原文里拼进去的运行时值"的拼接，拼接点与原文字面一一对应。

    /// <summary>SqliteM2DataDB.pas:507-547（DB_Version = 20170506）。</summary>
    private string MigrateScript_20170506()
        => SqliteM2DataDbScripts.DoUpdate_L507_S_P0
           + IntToStr(DbLayerGlobals.Environment.nAuctionCurrencyType)
           + SqliteM2DataDbScripts.DoUpdate_L507_S_P2;

    /// <summary>SqliteM2DataDB.pas:569-607（DB_Version = 20170603）。</summary>
    private string MigrateScript_20170603()
        => SqliteM2DataDbScripts.DoUpdate_L569_S_P0
           + IntToStr(DbLayerGlobals.Environment.nAuctionCurrencyType)
           + SqliteM2DataDbScripts.DoUpdate_L569_S_P2;

    /// <summary>SqliteM2DataDB.pas:630-651（DB_Version = 20170610）。</summary>
    private string MigrateScript_20170610()
        => SqliteM2DataDbScripts.DoUpdate_L630_S_P0
           + IntToStr(DbLayerGlobals.Environment.nAuctionCurrencyType)
           + SqliteM2DataDbScripts.DoUpdate_L630_S_P2;

    /// <summary>SqliteM2DataDB.pas:674-694（DB_Version = 20170701）。</summary>
    private string MigrateScript_20170701()
        => SqliteM2DataDbScripts.DoUpdate_L674_S_P0
           + IntToStr(DbLayerGlobals.Environment.nAuctionCurrencyType)
           + SqliteM2DataDbScripts.DoUpdate_L674_S_P2;

    /// <summary>SqliteM2DataDB.pas:719-735（DB_Version = 20180512）。</summary>
    private string MigrateScript_20180512()
        => SqliteM2DataDbScripts.DoUpdate_L719_S_P0
           + IntToStr(DbLayerGlobals.Environment.nAuctionCurrencyType)
           + SqliteM2DataDbScripts.DoUpdate_L719_S_P2;

    /// <summary>SqliteM2DataDB.pas:761-768（DB_Version = 20180613）。</summary>
    private string MigrateScript_20180613() => SqliteM2DataDbScripts.DoUpdate_L761_S;

    // ------------------------------------------------------------------
    // DoFinal（原文 963-1073）
    // ------------------------------------------------------------------

    /// <summary>
    /// SqliteM2DataDB.pas:963-1073 <c>DoFinal</c>。
    /// ★ 原文缺陷（逐字保留）：**没有** FStatementGetItems / FStatementGetItems_Sort 的
    /// Finalize 分支（其它 18 条都有）。测试 <c>DoFinal_DoesNotFinalizeGetItemsStatements</c> 锁定。
    /// </summary>
    public void DoFinal()
    {
        if (_fStatementInsertItems != null) { _fStatementInsertItems.StatementFinalize(); _fStatementInsertItems = null; }
        if (_fStatementInsertItemValueAdd != null) { _fStatementInsertItemValueAdd.StatementFinalize(); _fStatementInsertItemValueAdd = null; }
        if (_fStatementInsertItemElementAdd != null) { _fStatementInsertItemElementAdd.StatementFinalize(); _fStatementInsertItemElementAdd = null; }
        if (_fStatementInsertItemAddDataByte != null) { _fStatementInsertItemAddDataByte.StatementFinalize(); _fStatementInsertItemAddDataByte = null; }
        if (_fStatementInsertItemAddDataInt != null) { _fStatementInsertItemAddDataInt.StatementFinalize(); _fStatementInsertItemAddDataInt = null; }
        if (_fStatementInsertItemAddDataText != null) { _fStatementInsertItemAddDataText.StatementFinalize(); _fStatementInsertItemAddDataText = null; }
        if (_fStatementInsertItemFlute != null) { _fStatementInsertItemFlute.StatementFinalize(); _fStatementInsertItemFlute = null; }
        if (_fStatementInsertItemProgress != null) { _fStatementInsertItemProgress.StatementFinalize(); _fStatementInsertItemProgress = null; }
        if (_fStatementInsertItemProperty != null) { _fStatementInsertItemProperty.StatementFinalize(); _fStatementInsertItemProperty = null; }

        // 原文 1020-1067：只有 GetItem*，**没有** GetItems / GetItems_Sort（原文如此）。
        if (_fStatementGetItem != null) { _fStatementGetItem.StatementFinalize(); _fStatementGetItem = null; }
        if (_fStatementGetItemValueAdd != null) { _fStatementGetItemValueAdd.StatementFinalize(); _fStatementGetItemValueAdd = null; }
        if (_fStatementGetItemElementAdd != null) { _fStatementGetItemElementAdd.StatementFinalize(); _fStatementGetItemElementAdd = null; }
        if (_fStatementGetItemAddDataByte != null) { _fStatementGetItemAddDataByte.StatementFinalize(); _fStatementGetItemAddDataByte = null; }
        if (_fStatementGetItemAddDataInt != null) { _fStatementGetItemAddDataInt.StatementFinalize(); _fStatementGetItemAddDataInt = null; }
        if (_fStatementGetItemAddDataText != null) { _fStatementGetItemAddDataText.StatementFinalize(); _fStatementGetItemAddDataText = null; }
        if (_fStatementGetItemFlute != null) { _fStatementGetItemFlute.StatementFinalize(); _fStatementGetItemFlute = null; }
        if (_fStatementGetItemProgress != null) { _fStatementGetItemProgress.StatementFinalize(); _fStatementGetItemProgress = null; }
        if (_fStatementGetItemProperty != null) { _fStatementGetItemProperty.StatementFinalize(); _fStatementGetItemProperty = null; }
    }

    /// <summary>M2DataCommon.pas:1677-1687 <c>TM2DataDB.LoadItemsFromDB</c>（外层 try/except + MainOutMessage，**不加锁**）。</summary>
    public void LoadItemsFromDB(int parentId, int itemType, bool isSort, List<TUserItem> list)
    {
        try
        {
            DoLoadItemsFromDB(parentId, itemType, isSort, list);
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage("[Exception] TM2DataDB:LoadItemsFromDB;" + e.Message);
        }
    }

    /// <summary>M2DataCommon.pas:1653-1663 <c>TM2DataDB.LoadItemFromDB</c>。
    /// ★ 原文传 <c>PTUserItem</c>（指针，<c>@ShopItem.UserItem</c>），故托管侧用 <c>ref</c> 表达，
    /// 否则读到的物品会随栈拷贝被丢弃。</summary>
    public void LoadItemFromDB(ref TUserItem userItem, int parentId, int itemType, int itemIndex)
    {
        try
        {
            DoLoadItemFromDB(ref userItem, parentId, itemType, itemIndex);
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage("[Exception] TM2DataDB:LoadItemFromDB;" + e.Message);
        }
    }

    /// <summary>M2DataCommon.pas:1665-1675 <c>TM2DataDB.SaveItemToDB</c>（外层 try/except + MainOutMessage）。</summary>
    public void SaveItemToDB(TUserItem userItem, int parentId, int itemType, int itemIndex)
    {
        try
        {
            DoSaveItemToDB(userItem, parentId, itemType, itemIndex);
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage("[Exception] TM2DataDB:SaveItemToDB;" + e.Message);
        }
    }

    // 接缝：DoLoadItemsFromDB / DoLoadItemFromDB 的输出形态。
    //   原文签名是 `List: TList` + `UserItem: PTUserItem`（增加引用计数的指针）；
    //   托管侧 TUserItem 是 struct（值类型），"输出"必须显式回传，
    //   故以 ref/List<TUserItem> 表达同一语义（调用点顺序与原文一致）。

    /// <summary>SqliteM2DataDB.pas:1075-1292 <c>DoLoadItemsFromDB</c>。</summary>
    private void DoLoadItemsFromDB(int parentID, int itemType, bool isSort, List<TUserItem> list)
    {
        ISqliteStatement statement = isSort ? _fStatementGetItems_Sort! : _fStatementGetItems!;

        statement.Reset();
        statement.OrderBindInt(parentID);
        statement.OrderBindInt(itemType);

        int ret = statement.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            TUserItem userItem = M2ItemDbAccess.Zero();

            int itemIndex = statement.OrderGetColumnValueInt;
            userItem.MakeIndex = statement.OrderGetColumnValueInt;
            userItem.wIndex = (ushort)statement.OrderGetColumnValueInt;
            userItem.NameStr = statement.OrderGetColumnValueText;
            userItem.Dura = (ushort)statement.OrderGetColumnValueInt;
            userItem.DuraMax = (ushort)statement.OrderGetColumnValueInt;
            userItem.dwHeroM2DressEffect = (uint)statement.OrderGetColumnValueInt;
            userItem.btUpgradeCount = (byte)statement.OrderGetColumnValueInt;
            userItem.boStartTime = statement.OrderGetColumnValueBool ? (byte)1 : (byte)0;
            userItem.nLimitTime = statement.OrderGetColumnValueInt;
            userItem.btHeroM2Light = (byte)statement.OrderGetColumnValueInt;
            userItem.btColor = (byte)statement.OrderGetColumnValueInt;
            userItem.boIsBind = statement.OrderGetColumnValueBool ? (byte)1 : (byte)0;
            userItem.btBindOption = (byte)statement.OrderGetColumnValueInt;
            userItem.wEffect = (ushort)statement.OrderGetColumnValueInt;
            userItem.wNewLooks = (ushort)statement.OrderGetColumnValueInt;
            userItem.wNewShape = (ushort)statement.OrderGetColumnValueInt;
            userItem.btFluteCount = (byte)statement.OrderGetColumnValueInt;
            userItem.CustomProperty.TextStr = statement.OrderGetColumnValueText;
            userItem.CustomProperty.btTextColor = (byte)statement.OrderGetColumnValueInt;
            userItem.ItemFrom.ItemForm = (TItemFormType)statement.OrderGetColumnValueInt;
            userItem.ItemFrom.MapName = statement.OrderGetColumnValueText;
            userItem.ItemFrom.MonName = statement.OrderGetColumnValueText;
            userItem.ItemFrom.MakerName = statement.OrderGetColumnValueText;
            userItem.ItemFrom.DateTime = statement.OrderGetColumnValueDouble;
            userItem.wInsuranceCount = (ushort)statement.OrderGetColumnValueInt;
            userItem.wNewExpand3 = (ushort)statement.OrderGetColumnValueInt;
            userItem.wNewExpand4 = (ushort)statement.OrderGetColumnValueInt;

            // ---- FStatementGetItemValueAdd ----
            _fStatementGetItemValueAdd!.Reset();
            _fStatementGetItemValueAdd.OrderBindInt(parentID);
            _fStatementGetItemValueAdd.OrderBindInt(itemType);
            _fStatementGetItemValueAdd.OrderBindInt(itemIndex);
            int ret2 = _fStatementGetItemValueAdd.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemValueAdd.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.ValueLow && index2 <= M2ItemDbAccess.ValueHigh)
                {
                    int value = _fStatementGetItemValueAdd.OrderGetColumnValueInt;
                    M2ItemDbAccess.SetValue(ref userItem, index2, value);
                }

                ret2 = _fStatementGetItemValueAdd.Step();
            }
            _fStatementGetItemValueAdd.Reset();

            // ---- FStatementGetItemElementAdd ----
            _fStatementGetItemElementAdd!.Reset();
            _fStatementGetItemElementAdd.OrderBindInt(parentID);
            _fStatementGetItemElementAdd.OrderBindInt(itemType);
            _fStatementGetItemElementAdd.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemElementAdd.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemElementAdd.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.NewValueLow && index2 <= M2ItemDbAccess.NewValueHigh)
                {
                    int value = _fStatementGetItemElementAdd.OrderGetColumnValueInt;
                    M2ItemDbAccess.SetNewValue(ref userItem, index2, value);
                }

                ret2 = _fStatementGetItemElementAdd.Step();
            }
            _fStatementGetItemElementAdd.Reset();

            // ---- FStatementGetItemAddDataByte ----
            _fStatementGetItemAddDataByte!.Reset();
            _fStatementGetItemAddDataByte.OrderBindInt(parentID);
            _fStatementGetItemAddDataByte.OrderBindInt(itemType);
            _fStatementGetItemAddDataByte.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemAddDataByte.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.AddDataByteLow && index2 <= M2ItemDbAccess.AddDataByteHigh)
                {
                    int value = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    M2ItemDbAccess.SetAddDataByte(ref userItem, index2, value);
                }

                ret2 = _fStatementGetItemAddDataByte.Step();
            }
            _fStatementGetItemAddDataByte.Reset();

            // ---- FStatementGetItemAddDataInt ----
            _fStatementGetItemAddDataInt!.Reset();
            _fStatementGetItemAddDataInt.OrderBindInt(parentID);
            _fStatementGetItemAddDataInt.OrderBindInt(itemType);
            _fStatementGetItemAddDataInt.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemAddDataInt.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.AddDataIntLow && index2 <= M2ItemDbAccess.AddDataIntHigh)
                {
                    int value = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    M2ItemDbAccess.SetAddDataInt(ref userItem, index2, value);
                }

                ret2 = _fStatementGetItemAddDataInt.Step();
            }
            _fStatementGetItemAddDataInt.Reset();

            // ---- FStatementGetItemAddDataText ----
            _fStatementGetItemAddDataText!.Reset();
            _fStatementGetItemAddDataText.OrderBindInt(parentID);
            _fStatementGetItemAddDataText.OrderBindInt(itemType);
            _fStatementGetItemAddDataText.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemAddDataText.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemAddDataText.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.AddDataTextLow && index2 <= M2ItemDbAccess.AddDataTextHigh)
                {
                    string sTemp = _fStatementGetItemAddDataText.OrderGetColumnValueText;
                    M2ItemDbAccess.SetAddDataText(ref userItem, index2, sTemp);
                }

                ret2 = _fStatementGetItemAddDataText.Step();
            }
            _fStatementGetItemAddDataText.Reset();

            // ---- FStatementGetItemFlute（★ 原文 1219 绑 0 而不是 ItemType） ----
            _fStatementGetItemFlute!.Reset();
            _fStatementGetItemFlute.OrderBindInt(parentID);
            _fStatementGetItemFlute.OrderBindInt(0);
            _fStatementGetItemFlute.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemFlute.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemFlute.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.FluteLow && index2 <= M2ItemDbAccess.FluteHigh)
                {
                    var flute = userItem.GetFlute(index2);
                    flute.GemIndex = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;
                    flute.GemCount = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;

                    if (flute.GemIndex > 0 && flute.GemCount == 0) flute.GemCount = 1;
                    userItem.SetFlute(index2, flute);
                }

                ret2 = _fStatementGetItemFlute.Step();
            }
            _fStatementGetItemFlute.Reset();

            // ---- FStatementGetItemProgress ----
            _fStatementGetItemProgress!.Reset();
            _fStatementGetItemProgress.OrderBindInt(parentID);
            _fStatementGetItemProgress.OrderBindInt(itemType);
            _fStatementGetItemProgress.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemProgress.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemProgress.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.ProgressLow && index2 <= M2ItemDbAccess.ProgressHigh)
                {
                    TUserItemProgress progress = M2ItemDbAccess.GetProgress(ref userItem, index2);
                    progress.boOpen = _fStatementGetItemProgress.OrderGetColumnValueBool ? (byte)1 : (byte)0;
                    progress.btNameColor = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;
                    progress.btCount = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;
                    progress.btShowType = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;
                    progress.wMax = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;
                    progress.wValue = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;
                    progress.wLevel = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;
                    progress.NameStr = _fStatementGetItemProgress.OrderGetColumnValueText;
                    M2ItemDbAccess.SetProgress(ref userItem, index2, progress);
                }

                ret2 = _fStatementGetItemProgress.Step();
            }
            _fStatementGetItemProgress.Reset();

            // ---- FStatementGetItemProperty ----
            _fStatementGetItemProperty!.Reset();
            _fStatementGetItemProperty.OrderBindInt(parentID);
            _fStatementGetItemProperty.OrderBindInt(itemType);
            _fStatementGetItemProperty.OrderBindInt(itemIndex);
            ret2 = _fStatementGetItemProperty.Step();
            while (ret2 == SqliteCodes.SQLITE_ROW)
            {
                int index2 = _fStatementGetItemProperty.OrderGetColumnValueInt;
                if (index2 >= M2ItemDbAccess.PropertyLow && index2 <= M2ItemDbAccess.PropertyHigh)
                {
                    TCustomProperty property = M2ItemDbAccess.GetProperty(ref userItem, index2);
                    property.btColor = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                    property.btBindType = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                    property.btShowFlag = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                    property.btPercent = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                    property.btHintModule = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                    property.SetValues(new[]
                    {
                        _fStatementGetItemProperty.OrderGetColumnValueInt,
                        _fStatementGetItemProperty.OrderGetColumnValueInt,
                        _fStatementGetItemProperty.OrderGetColumnValueInt,
                    });
                    M2ItemDbAccess.SetProperty(ref userItem, index2, property);
                }

                ret2 = _fStatementGetItemProperty.Step();
            }
            _fStatementGetItemProperty.Reset();

            ret = statement.Step();

            list.Add(userItem);
        }
    }

    /// <summary>SqliteM2DataDB.pas:1294-1496 <c>DoLoadItemFromDB</c>。</summary>
    private void DoLoadItemFromDB(ref TUserItem userItem, int parentID, int itemType, int itemIndex)
    {
        // 1300：FillChar(UserItem^, SizeOf(TUserItem), 0)
        userItem = M2ItemDbAccess.Zero();

        _fStatementGetItem!.Reset();
        _fStatementGetItem.OrderBindInt(parentID);
        _fStatementGetItem.OrderBindInt(itemType);
        _fStatementGetItem.OrderBindInt(itemIndex);
        int ret = _fStatementGetItem.Step();
        if (ret == SqliteCodes.SQLITE_ROW)
        {
            userItem.MakeIndex = _fStatementGetItem.OrderGetColumnValueInt;
            userItem.wIndex = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.NameStr = _fStatementGetItem.OrderGetColumnValueText;
            userItem.Dura = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.DuraMax = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.dwHeroM2DressEffect = (uint)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.btUpgradeCount = (byte)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.boStartTime = _fStatementGetItem.OrderGetColumnValueBool ? (byte)1 : (byte)0;
            userItem.nLimitTime = _fStatementGetItem.OrderGetColumnValueInt;
            userItem.btHeroM2Light = (byte)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.btColor = (byte)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.boIsBind = _fStatementGetItem.OrderGetColumnValueBool ? (byte)1 : (byte)0;
            userItem.btBindOption = (byte)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.wEffect = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.wNewLooks = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.wNewShape = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.btFluteCount = (byte)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.CustomProperty.TextStr = _fStatementGetItem.OrderGetColumnValueText;
            userItem.CustomProperty.btTextColor = (byte)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.ItemFrom.ItemForm = (TItemFormType)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.ItemFrom.MapName = _fStatementGetItem.OrderGetColumnValueText;
            userItem.ItemFrom.MonName = _fStatementGetItem.OrderGetColumnValueText;
            userItem.ItemFrom.MakerName = _fStatementGetItem.OrderGetColumnValueText;
            userItem.ItemFrom.DateTime = _fStatementGetItem.OrderGetColumnValueDouble;
            userItem.wInsuranceCount = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.wNewExpand3 = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
            userItem.wNewExpand4 = (ushort)_fStatementGetItem.OrderGetColumnValueInt;
        }
        _fStatementGetItem.Reset();

        _fStatementGetItemValueAdd!.Reset();
        _fStatementGetItemValueAdd.OrderBindInt(parentID);
        _fStatementGetItemValueAdd.OrderBindInt(itemType);
        _fStatementGetItemValueAdd.OrderBindInt(itemIndex);
        ret = _fStatementGetItemValueAdd.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemValueAdd.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.ValueLow && index2 <= M2ItemDbAccess.ValueHigh)
            {
                int value = _fStatementGetItemValueAdd.OrderGetColumnValueInt;
                M2ItemDbAccess.SetValue(ref userItem, index2, value);
            }

            ret = _fStatementGetItemValueAdd.Step();
        }
        _fStatementGetItemValueAdd.Reset();

        _fStatementGetItemElementAdd!.Reset();
        _fStatementGetItemElementAdd.OrderBindInt(parentID);
        _fStatementGetItemElementAdd.OrderBindInt(itemType);
        _fStatementGetItemElementAdd.OrderBindInt(itemIndex);
        ret = _fStatementGetItemElementAdd.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemElementAdd.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.NewValueLow && index2 <= M2ItemDbAccess.NewValueHigh)
            {
                int value = _fStatementGetItemElementAdd.OrderGetColumnValueInt;
                M2ItemDbAccess.SetNewValue(ref userItem, index2, value);
            }

            ret = _fStatementGetItemElementAdd.Step();
        }
        _fStatementGetItemElementAdd.Reset();

        _fStatementGetItemAddDataByte!.Reset();
        _fStatementGetItemAddDataByte.OrderBindInt(parentID);
        _fStatementGetItemAddDataByte.OrderBindInt(itemType);
        _fStatementGetItemAddDataByte.OrderBindInt(itemIndex);
        ret = _fStatementGetItemAddDataByte.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.AddDataByteLow && index2 <= M2ItemDbAccess.AddDataByteHigh)
            {
                int value = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;
                M2ItemDbAccess.SetAddDataByte(ref userItem, index2, value);
            }

            ret = _fStatementGetItemAddDataByte.Step();
        }
        _fStatementGetItemAddDataByte.Reset();

        _fStatementGetItemAddDataInt!.Reset();
        _fStatementGetItemAddDataInt.OrderBindInt(parentID);
        _fStatementGetItemAddDataInt.OrderBindInt(itemType);
        _fStatementGetItemAddDataInt.OrderBindInt(itemIndex);
        ret = _fStatementGetItemAddDataInt.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.AddDataIntLow && index2 <= M2ItemDbAccess.AddDataIntHigh)
            {
                int value = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;
                M2ItemDbAccess.SetAddDataInt(ref userItem, index2, value);
            }

            ret = _fStatementGetItemAddDataInt.Step();
        }
        _fStatementGetItemAddDataInt.Reset();

        _fStatementGetItemAddDataText!.Reset();
        _fStatementGetItemAddDataText.OrderBindInt(parentID);
        _fStatementGetItemAddDataText.OrderBindInt(itemType);
        _fStatementGetItemAddDataText.OrderBindInt(itemIndex);
        ret = _fStatementGetItemAddDataText.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemAddDataText.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.AddDataTextLow && index2 <= M2ItemDbAccess.AddDataTextHigh)
            {
                string sTemp = _fStatementGetItemAddDataText.OrderGetColumnValueText;
                M2ItemDbAccess.SetAddDataText(ref userItem, index2, sTemp);
            }

            ret = _fStatementGetItemAddDataText.Step();
        }
        _fStatementGetItemAddDataText.Reset();

        // ★ 这里原文绑的是 ItemType（与 DoLoadItemsFromDB 的 OrderBindInt(0) 不一致，原文如此）
        _fStatementGetItemFlute!.Reset();
        _fStatementGetItemFlute.OrderBindInt(parentID);
        _fStatementGetItemFlute.OrderBindInt(itemType);
        _fStatementGetItemFlute.OrderBindInt(itemIndex);
        ret = _fStatementGetItemFlute.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemFlute.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.FluteLow && index2 <= M2ItemDbAccess.FluteHigh)
            {
                var flute = userItem.GetFlute(index2);
                flute.GemIndex = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;
                flute.GemCount = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;

                if (flute.GemIndex > 0 && flute.GemCount == 0) flute.GemCount = 1;
                userItem.SetFlute(index2, flute);
            }

            ret = _fStatementGetItemFlute.Step();
        }
        _fStatementGetItemFlute.Reset();

        _fStatementGetItemProgress!.Reset();
        _fStatementGetItemProgress.OrderBindInt(parentID);
        _fStatementGetItemProgress.OrderBindInt(itemType);
        _fStatementGetItemProgress.OrderBindInt(itemIndex);
        ret = _fStatementGetItemProgress.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemProgress.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.ProgressLow && index2 <= M2ItemDbAccess.ProgressHigh)
            {
                TUserItemProgress progress = M2ItemDbAccess.GetProgress(ref userItem, index2);
                progress.boOpen = _fStatementGetItemProgress.OrderGetColumnValueBool ? (byte)1 : (byte)0;
                progress.btNameColor = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;
                progress.btCount = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;
                progress.btShowType = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;
                progress.wMax = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;
                progress.wValue = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;
                progress.wLevel = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;
                progress.NameStr = _fStatementGetItemProgress.OrderGetColumnValueText;
                M2ItemDbAccess.SetProgress(ref userItem, index2, progress);
            }

            ret = _fStatementGetItemProgress.Step();
        }
        _fStatementGetItemProgress.Reset();

        _fStatementGetItemProperty!.Reset();
        _fStatementGetItemProperty.OrderBindInt(parentID);
        _fStatementGetItemProperty.OrderBindInt(itemType);
        _fStatementGetItemProperty.OrderBindInt(itemIndex);
        ret = _fStatementGetItemProperty.Step();
        while (ret == SqliteCodes.SQLITE_ROW)
        {
            int index2 = _fStatementGetItemProperty.OrderGetColumnValueInt;
            if (index2 >= M2ItemDbAccess.PropertyLow && index2 <= M2ItemDbAccess.PropertyHigh)
            {
                TCustomProperty property = M2ItemDbAccess.GetProperty(ref userItem, index2);
                property.btColor = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                property.btBindType = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                property.btShowFlag = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                property.btPercent = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                property.btHintModule = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                property.SetValues(new[]
                {
                    _fStatementGetItemProperty.OrderGetColumnValueInt,
                    _fStatementGetItemProperty.OrderGetColumnValueInt,
                    _fStatementGetItemProperty.OrderGetColumnValueInt,
                });
                M2ItemDbAccess.SetProperty(ref userItem, index2, property);
            }

            ret = _fStatementGetItemProperty.Step();
        }
        _fStatementGetItemProperty.Reset();
    }

    /// <summary>SqliteM2DataDB.pas:1498-1701 <c>DoSaveItemToDB</c>。</summary>
    private void DoSaveItemToDB(TUserItem userItem, int parentID, int itemType, int itemIndex)
    {
        // 1502-1537
        _fStatementInsertItems!.Reset();
        try
        {
            _fStatementInsertItems.OrderBindInt(parentID);
            _fStatementInsertItems.OrderBindInt(itemType);
            _fStatementInsertItems.OrderBindInt(itemIndex);
            _fStatementInsertItems.OrderBindInt(userItem.MakeIndex);
            _fStatementInsertItems.OrderBindInt(userItem.wIndex);
            _fStatementInsertItems.OrderBindText(userItem.NameStr);
            _fStatementInsertItems.OrderBindInt(userItem.Dura);
            _fStatementInsertItems.OrderBindInt(userItem.DuraMax);
            _fStatementInsertItems.OrderBindInt((int)userItem.dwHeroM2DressEffect);
            _fStatementInsertItems.OrderBindInt(userItem.btUpgradeCount);
            _fStatementInsertItems.OrderBindBool(userItem.boStartTime != 0);
            _fStatementInsertItems.OrderBindInt(userItem.nLimitTime);
            _fStatementInsertItems.OrderBindInt(userItem.btHeroM2Light);
            _fStatementInsertItems.OrderBindInt(userItem.btColor);
            _fStatementInsertItems.OrderBindBool(userItem.boIsBind != 0);
            _fStatementInsertItems.OrderBindInt(userItem.btBindOption);
            _fStatementInsertItems.OrderBindInt(userItem.wEffect);
            _fStatementInsertItems.OrderBindInt(userItem.wNewLooks);
            _fStatementInsertItems.OrderBindInt(userItem.wNewShape);
            _fStatementInsertItems.OrderBindInt(userItem.btFluteCount);
            _fStatementInsertItems.OrderBindText(userItem.CustomProperty.TextStr);
            _fStatementInsertItems.OrderBindInt(userItem.CustomProperty.btTextColor);
            _fStatementInsertItems.OrderBindInt((int)userItem.ItemFrom.ItemForm);
            _fStatementInsertItems.OrderBindText(userItem.ItemFrom.MapName);
            _fStatementInsertItems.OrderBindText(userItem.ItemFrom.MonName);
            _fStatementInsertItems.OrderBindText(userItem.ItemFrom.MakerName);
            _fStatementInsertItems.OrderBindDouble(userItem.ItemFrom.DateTime);
            _fStatementInsertItems.OrderBindInt(userItem.wInsuranceCount);
            _fStatementInsertItems.OrderBindInt(userItem.wNewExpand3);
            _fStatementInsertItems.OrderBindInt(userItem.wNewExpand4);
            _fStatementInsertItems.Step();
        }
        finally
        {
            _fStatementInsertItems.Reset();
        }

        // ---- ItemValueAdd ----
        try
        {
            for (int j = M2ItemDbAccess.ValueLow; j <= M2ItemDbAccess.ValueHigh; j++)
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetValue(ref copy, j) != 0)
                {
                    _fStatementInsertItemValueAdd!.Reset();
                    _fStatementInsertItemValueAdd.OrderBindInt(parentID);
                    _fStatementInsertItemValueAdd.OrderBindInt(itemType);
                    _fStatementInsertItemValueAdd.OrderBindInt(itemIndex);
                    _fStatementInsertItemValueAdd.OrderBindInt(j);
                    _fStatementInsertItemValueAdd.OrderBindInt(M2ItemDbAccess.GetValue(ref copy, j));
                    _fStatementInsertItemValueAdd.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemValueAdd!.Reset();
        }

        // ---- ItemElementAdd ----
        try
        {
            for (int j = M2ItemDbAccess.NewValueLow; j <= M2ItemDbAccess.NewValueHigh; j++)
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetNewValue(ref copy, j) != 0)
                {
                    _fStatementInsertItemElementAdd!.Reset();
                    _fStatementInsertItemElementAdd.OrderBindInt(parentID);
                    _fStatementInsertItemElementAdd.OrderBindInt(itemType);
                    _fStatementInsertItemElementAdd.OrderBindInt(itemIndex);
                    _fStatementInsertItemElementAdd.OrderBindInt(j);
                    _fStatementInsertItemElementAdd.OrderBindInt(M2ItemDbAccess.GetNewValue(ref copy, j));
                    _fStatementInsertItemElementAdd.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemElementAdd!.Reset();
        }

        // ---- ItemAddDataByte ----
        try
        {
            for (int j = M2ItemDbAccess.AddDataByteLow; j <= M2ItemDbAccess.AddDataByteHigh; j++)
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetAddDataByte(ref copy, j) != 0)
                {
                    _fStatementInsertItemAddDataByte!.Reset();
                    _fStatementInsertItemAddDataByte.OrderBindInt(parentID);
                    _fStatementInsertItemAddDataByte.OrderBindInt(itemType);
                    _fStatementInsertItemAddDataByte.OrderBindInt(itemIndex);
                    _fStatementInsertItemAddDataByte.OrderBindInt(j);
                    _fStatementInsertItemAddDataByte.OrderBindInt(M2ItemDbAccess.GetAddDataByte(ref copy, j));
                    _fStatementInsertItemAddDataByte.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemAddDataByte!.Reset();
        }

        // ---- ItemAddDataInt ----
        try
        {
            for (int j = M2ItemDbAccess.AddDataIntLow; j <= M2ItemDbAccess.AddDataIntHigh; j++)
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetAddDataInt(ref copy, j) != 0)
                {
                    _fStatementInsertItemAddDataInt!.Reset();
                    _fStatementInsertItemAddDataInt.OrderBindInt(parentID);
                    _fStatementInsertItemAddDataInt.OrderBindInt(itemType);
                    _fStatementInsertItemAddDataInt.OrderBindInt(itemIndex);
                    _fStatementInsertItemAddDataInt.OrderBindInt(j);
                    _fStatementInsertItemAddDataInt.OrderBindInt(M2ItemDbAccess.GetAddDataInt(ref copy, j));
                    _fStatementInsertItemAddDataInt.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemAddDataInt!.Reset();
        }

        // ---- ItemAddDataText ----
        try
        {
            for (int j = M2ItemDbAccess.AddDataTextLow; j <= M2ItemDbAccess.AddDataTextHigh; j++)
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetAddDataText(ref copy, j) != "")
                {
                    _fStatementInsertItemAddDataText!.Reset();
                    _fStatementInsertItemAddDataText.OrderBindInt(parentID);
                    _fStatementInsertItemAddDataText.OrderBindInt(itemType);
                    _fStatementInsertItemAddDataText.OrderBindInt(itemIndex);
                    _fStatementInsertItemAddDataText.OrderBindInt(j);
                    _fStatementInsertItemAddDataText.OrderBindText(M2ItemDbAccess.GetAddDataText(ref copy, j));
                    _fStatementInsertItemAddDataText.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemAddDataText!.Reset();
        }

        // ---- ItemFlute ----
        try
        {
            for (int j = M2ItemDbAccess.FluteLow; j <= M2ItemDbAccess.FluteHigh; j++)
            {
                var copy = userItem;
                TFluteInfo flute = copy.GetFlute(j);
                if (flute.GemIndex != 0)
                {
                    _fStatementInsertItemFlute!.Reset();
                    _fStatementInsertItemFlute.OrderBindInt(parentID);
                    _fStatementInsertItemFlute.OrderBindInt(itemType);
                    _fStatementInsertItemFlute.OrderBindInt(itemIndex);
                    _fStatementInsertItemFlute.OrderBindInt(j);
                    _fStatementInsertItemFlute.OrderBindInt(copy.GetFlute(j).GemIndex);
                    _fStatementInsertItemFlute.OrderBindInt(copy.GetFlute(j).GemCount);
                    _fStatementInsertItemFlute.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemFlute!.Reset();
        }

        // ---- ItemProgress（★ 原文判据是 boOpen，不是"整槽非空"） ----
        try
        {
            for (int j = M2ItemDbAccess.ProgressLow; j <= M2ItemDbAccess.ProgressHigh; j++)
            {
                var copy = userItem;
                TUserItemProgress progress = M2ItemDbAccess.GetProgress(ref copy, j);
                if (progress.boOpen != 0)
                {
                    _fStatementInsertItemProgress!.Reset();
                    _fStatementInsertItemProgress.OrderBindInt(parentID);
                    _fStatementInsertItemProgress.OrderBindInt(itemType);
                    _fStatementInsertItemProgress.OrderBindInt(itemIndex);
                    _fStatementInsertItemProgress.OrderBindInt(j);
                    _fStatementInsertItemProgress.OrderBindBool(progress.boOpen != 0);
                    _fStatementInsertItemProgress.OrderBindInt(progress.btNameColor);
                    _fStatementInsertItemProgress.OrderBindInt(progress.btCount);
                    _fStatementInsertItemProgress.OrderBindInt(progress.btShowType);
                    _fStatementInsertItemProgress.OrderBindInt(progress.wMax);
                    _fStatementInsertItemProgress.OrderBindInt(progress.wValue);
                    _fStatementInsertItemProgress.OrderBindInt(progress.wLevel);
                    _fStatementInsertItemProgress.OrderBindText(progress.NameStr);
                    _fStatementInsertItemProgress.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemProgress!.Reset();
        }

        // ---- ItemProperty（★ 原文判据：三个 nValues 任一 > 0） ----
        try
        {
            for (int j = M2ItemDbAccess.PropertyLow; j <= M2ItemDbAccess.PropertyHigh; j++)
            {
                var copy = userItem;
                TCustomProperty property = M2ItemDbAccess.GetProperty(ref copy, j);
                int[] nValues = property.GetValues();
                if (nValues[0] > 0 || nValues[1] > 0 || nValues[2] > 0)
                {
                    _fStatementInsertItemProperty!.Reset();
                    _fStatementInsertItemProperty.OrderBindInt(parentID);
                    _fStatementInsertItemProperty.OrderBindInt(itemType);
                    _fStatementInsertItemProperty.OrderBindInt(itemIndex);
                    _fStatementInsertItemProperty.OrderBindInt(j);
                    _fStatementInsertItemProperty.OrderBindInt(property.btColor);
                    _fStatementInsertItemProperty.OrderBindInt(property.btBindType);
                    _fStatementInsertItemProperty.OrderBindInt(property.btShowFlag);
                    _fStatementInsertItemProperty.OrderBindInt(property.btPercent);
                    _fStatementInsertItemProperty.OrderBindInt(property.btHintModule);
                    _fStatementInsertItemProperty.OrderBindInt(nValues[0]);
                    _fStatementInsertItemProperty.OrderBindInt(nValues[1]);
                    _fStatementInsertItemProperty.OrderBindInt(nValues[2]);
                    _fStatementInsertItemProperty.Step();
                }
            }
        }
        finally
        {
            _fStatementInsertItemProperty!.Reset();
        }
    }

    /// <summary>M2DataCommon.pas:1634-1643 <c>TM2DataDB.Init</c>：
    /// <c>DoInit; FIsInitOK := True; FAuctionDB.DoInit; FUserShopDB.DoInit; FStorageDB.DoInit;</c>。
    /// ★ 原文 <c>IsInitOK</c> 是 <b>Init</b> 设的，不是 <c>DoInit</c> 设的（本移植照此）。</summary>
    public void Init()
    {
        DoInit();
        IsInitOK = true;
        AuctionDB?.Init();
        UserShopDB?.Init();
        // FStorageDB.DoInit —— StorageDB 未移植（见报告 §接缝清单）。
    }

    /// <summary>M2DataCommon.pas:1645-1651 <c>TM2DataDB.Final</c>：
    /// <c>DoFinal; FAuctionDB.DoFinal; FUserShopDB.DoFinal; FStorageDB.DoFinal;</c>（不改 IsInitOK）。</summary>
    public void Final()
    {
        DoFinal();
        AuctionDB?.Final();
        UserShopDB?.Final();
        // FStorageDB.DoFinal —— StorageDB 未移植。
    }

    /// <summary>M2DataCommon.pas <c>TM2DataDB.Run</c>（本单元无 DoRun 覆写，原文如此）。</summary>
    public void Run()
    {
    }

    // ------------------------------------------------------------------
    // 文件系统 / SQLite 全局接缝（原文直接调 SysUtils、SQLite3DataBase、sqlite3_config）
    // ------------------------------------------------------------------

    /// <summary>接缝：<c>ExtractFilePath(ParamStr(0))</c>。默认取当前目录（测试可替换）。</summary>
    public static Func<string> ExtractFilePath { get; set; } = () => AppContext.BaseDirectory;

    /// <summary>接缝：<c>DirectoryExists</c>。</summary>
    public static Func<string, bool> DirectoryExists { get; set; } = System.IO.Directory.Exists;

    /// <summary>接缝：<c>ForceDirectories</c>。</summary>
    public static Action<string> ForceDirectories { get; set; } = dir => System.IO.Directory.CreateDirectory(dir);

    /// <summary>接缝：<c>FileExists</c>。</summary>
    public static Func<string, bool> FileExists { get; set; } = System.IO.File.Exists;

    /// <summary>接缝：<c>DeleteFile</c>（失败静默，与原文 DeleteFile 返回 Boolean 被忽略一致）。</summary>
    public static Action<string> DeleteFile { get; set; } = path =>
    {
        try { System.IO.File.Delete(path); } catch { /* 原文忽略返回值 */ }
    };

    /// <summary>接缝：<c>TResourceStream.Create(HInstance, 'M2Data', ...).SaveToFile(FileName)</c>。</summary>
    public static Action<string, string> SaveResourceToFile { get; set; } = (_, _) => { };

    /// <summary>接缝：<c>sqlite3_config(SQLITE_CONFIG_MULTITHREAD)</c>。</summary>
    public static Action Sqlite3ConfigMultithread { get; set; } = () => { };

    /// <summary>恢复文件系统/驱动接缝默认实现（测试用）。</summary>
    public static void ResetFileSeams()
    {
        ExtractFilePath = () => AppContext.BaseDirectory;
        DirectoryExists = System.IO.Directory.Exists;
        ForceDirectories = dir => System.IO.Directory.CreateDirectory(dir);
        FileExists = System.IO.File.Exists;
        DeleteFile = path =>
        {
            try { System.IO.File.Delete(path); } catch { }
        };
        SaveResourceToFile = (_, _) => { };
        Sqlite3ConfigMultithread = () => { };
    }
}

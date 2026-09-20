// 源单元：Source/M2Engine/MySqlM2DataDB.pas（1-1398 行，1:1 移植）
//   TMySqlM2DataDB = class(TM2DataDB)：Create/Destroy、GetAuctionDBClass/GetStorageDBClass/
//   GetUserShopDBClass、UpdaeDB_1..5（原文拼写 Updae）、DoInit、DoFinal、DoLoadItemsFromDB、
//   DoLoadItemFromDB、DoSaveItemToDB、GetDataBase、OnRequest、Run。
//
// SQL 逐字保真：20 条 <c>AddSQLStatement</c> 语句在 SqlStatements.MySqlM2DataDB.cs（常量名 = 原文
// label、顺序 = 原文 DoInit 顺序，每条带 MySqlM2DataDB.pas 行号注释）；方法级脚本
// <c>UpdaeDB_5_L198_S</c>（192-235）与 DoInit 版本探针 <c>DoInit_L250_sm_Sql</c>（250）在
// SqlStatements.MySqlM2DataDB.Scripts.cs —— 二者均由 _recon/p3-sql.mjs 从 GBK 原文机械抽取，
// **零手工转录**。测试 DbLayerM2DataSqlFidelityTests 把实现真正绑上去的 SQL 取 SHA-256 与
// tests/.../DbLayerFingerprints.MySqlM2DataDB.cs 的指纹表比对。
//   ⚠ UpdaeDB_1..4（136-190）在原文里是**硬编码 Exec 字符串**（无 label、无脚本常量），
//     本文件按原文逐字手写这 6 条 ALTER + 4 条 REPLACE；UpdaeDB_5（192-235）的 DDL 有脚本常量
//     <c>UpdaeDB_5_L198_S</c>，它的收尾 REPLACE 同样是硬编码（合计 5 条 REPLACE，见各方法注释）。
//   ⚠ MySQL 版**不建表**：建表脚本在 Source/Common/MySqlCreateTableSql.pas 的
//     MYSQL_CREATE_M2DATA_TABLES（C# 侧 GXX.Core.Data.MySqlCreateTableSql），由装配方执行；
//     DoInit 只做 Connect → 版本探针 → UpdaeDB_N 升级。这与 SQLite 版 DoInit（内建
//     SQLITE_CREATE_M2DATA_TABLES + DoUpdate 七段迁移 + ItemDBName/ItemName 回填）**完全不同**，
//     不要照抄 SQLite 的迁移做法。
//
// 方言要点（对照姊妹实现 SqliteM2DataDB.cs；逐条带原文行号）：
//   * 连接：DoInit 第一件事是 Connect(..., CLIENT_MULTI_STATEMENTS)（243）+ CharacterSetName := 'utf8'
//     （244）；另有 OnRequest（1384）刷新 FLastRequestTick 与 Run（1389）的 10 分钟保活 Exec('select 1')。
//     SQLite 版没有这些（它走库文件 + ErrorCode=11 时删 shm/wal 重连）。
//   * 事务 StartTransaction/Commit/RollBack（138/145/147、153/159/161 …），批量脚本 Exec(S)（141/229）；
//   * 结果集必须 `if X.Query then while (X.Fetch) do`（759-761、798-800 …），
//     单行是 `if X.Query and X.Fetch`（973、252）；
//   * 无结果集语句直接 `Step`（Boolean，1208、1227 …）；
//   * 探针语句 `sm.Reset`（256）+ finally（258）；语句结束一律 `Reset`（810/1023 …），**从不 Finalize**；
//   * 绑定 OrderBindParamInt/Bool/Text（757/1188/1183 …）与 OrderBindParamDouble（1204）
//     —— 接缝 IMySqlStatement 的成员名与原文逐字一致；
//     读列 OrderGetColumnValueInt/Bool/Text/Double（765/773/768/789 …）；
//   * 时间列 ItemFromDate 两方言都用 Double（1204/998 与 SQLite 1302/873 一致），不是 DateTime；
//   * 版本分支是**精确相等 =**（261/269/276/282/287），版本号集合（20180615/20190314/20190606/
//     20190929/20200813）与 SQLite 版（20170506…20200813 的 <= 区间）毫无交集；
//   * UpdaeDB_1..5 的 except 是**裸 `except FDB.RollBack`**（146/160/175/187/232），
//     **不打日志**；SQLite 版同位置是 `on E: Exception do MainOutMessage(E.Message)`。
//
// 原文缺陷/易错点（逐字保留，不修）：
//   * DoFinal（628-738）**不 Finalize** FStatementGetItems / FStatementGetItems_Sort（与 SQLite 版同病）；
//   * DoLoadItemsFromDB 的 Flute 语句绑 `OrderBindParamInt(0)`（887），而 DoLoadItemFromDB 同位置
//     绑 `ItemType`（1099）——同一个 SQL 在两处的绑定参数不一致（原文如此）；
//   * DoLoadItemsFromDB 外层语句循环结束后**没有** Reset（955-960），内层 7 条语句都有；
//   * DoSaveItemToDB 的 8 个 try/finally 在**异常路径也会 Reset**（1209-1211、1230 …）；
//   * UpdaeDB_5 用 sLineBreak 拼 DDL 且小写 `drop table IF EXISTS`（198-227）——与
//     SqliteM2DataDB.UpdateDB_6 的大写 `DROP TABLE` + `-- ---` 注释行不同形；
//   * Run 的保活 SQL 是 `'select 1'`（1394，无分号）；
//   * DoInit 结尾 `FDB.Statements.Clear`（258）会把探针语句一起清掉（且**不** Finalize 它）；
//     接缝 `IMySqlDatabase.ClearStatements()`（DbSeam.cs）逐字对应这一句。

using System;
using System.Collections.Generic;
using GXX.Core.Data;
using GXX.Core.Protocol;

namespace GXX.M2Server.DbLayer;

/// <summary>MySqlM2DataDB.pas:10-65 <c>TMySqlM2DataDB</c>。</summary>
public sealed class TMySqlM2DataDB : IM2DataDb
{
    private uint _fLastRequestTick;
    private IMySqlDatabase? _fdb;

    private IMySqlStatement? _fStatementInsertItems;
    private IMySqlStatement? _fStatementInsertItemValueAdd;
    private IMySqlStatement? _fStatementInsertItemElementAdd;
    private IMySqlStatement? _fStatementInsertItemAddDataByte;
    private IMySqlStatement? _fStatementInsertItemAddDataInt;
    private IMySqlStatement? _fStatementInsertItemAddDataText;
    private IMySqlStatement? _fStatementInsertItemFlute;
    private IMySqlStatement? _fStatementInsertItemProgress;
    private IMySqlStatement? _fStatementInsertItemProperty;

    private IMySqlStatement? _fStatementGetItems;
    private IMySqlStatement? _fStatementGetItems_Sort;

    private IMySqlStatement? _fStatementGetItem;
    private IMySqlStatement? _fStatementGetItemValueAdd;
    private IMySqlStatement? _fStatementGetItemElementAdd;
    private IMySqlStatement? _fStatementGetItemAddDataByte;
    private IMySqlStatement? _fStatementGetItemAddDataInt;
    private IMySqlStatement? _fStatementGetItemAddDataText;
    private IMySqlStatement? _fStatementGetItemFlute;
    private IMySqlStatement? _fStatementGetItemProgress;
    private IMySqlStatement? _fStatementGetItemProperty;

    private readonly IDbLayerHost _owner;

    /// <summary>MySqlM2DataDB.pas:243 <c>CLIENT_MULTI_STATEMENTS</c>（MySQLCli.pas:186 = 65536，Connect 末参）。</summary>
    public const uint CLIENT_MULTI_STATEMENTS = 65536;

    /// <summary>MySqlM2DataDB.pas:111/245/1386/1392 的保活周期（<c>10 * 60000</c> 毫秒）。</summary>
    public const uint RunKeepAliveTick = 10 * 60000;

    /// <summary>
    /// MySqlM2DataDB.pas:74-112 <c>constructor Create</c>：原文 FMySqlLib.Create/Load('libmysql-32.dll')
    /// → FDB.Create(FMySqlLib) → FDB.Init → FDB.OnRequest := OnRequest → 21 个语句字段置 nil →
    /// FLastRequestTick := MyGetTickCount。
    /// </summary>
    /// <remarks>
    /// 接缝：<c>TMySQLLib</c> / <c>TMySqlDataBase</c>（MySqlDataBase.pas + MySqlWrap.pas，源码树内不存在、
    /// 原文直连 libmysql-32.dll）未移植 —— 驱动器由装配方注入 <see cref="IMySqlDatabase"/>：
    /// 优先用构造参数 <paramref name="db"/>，其次用 <c>Owner.DataBase</c>（与 DbLayerSqlFidelityTests
    /// 里 TMySqlUserShopDB 的注入方式一致），都没有时退回 <see cref="UnavailableMySqlDatabase"/>。
    /// 原文 85/87 的 <c>FDB.Init</c> / <c>FDB.OnRequest := OnRequest</c> 仅在真正注入了驱动时执行
    /// （与姊妹实现 SqliteM2DataDB.cs 的构造函数一样：不碰未注入的默认驱动器，避免构造即抛）。
    /// <c>FMySqlLib</c>（dll 句柄）在托管侧无对应物，见 <see cref="Destroy"/>。
    /// </remarks>
    public TMySqlM2DataDB(IDbLayerHost owner, IMySqlDatabase? db = null)
    {
        _owner = owner;

        IMySqlDatabase? injected = db ?? owner.DataBase as IMySqlDatabase;
        _fdb = injected ?? UnavailableMySqlDatabase.Instance;

        if (injected is not null)
        {
            _fdb.Init();                        // 85
            _fdb.OnRequest += OnRequest;        // 87
        }

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

        _fLastRequestTick = DbLayerGlobals.GetTickCount();   // 111
    }

    /// <summary>MySqlM2DataDB.pas:114-119 <c>destructor Destroy</c>（原文 FDB.Free; FMySqlLib.Free）。
    /// 托管侧：驱动器实例（含 libmysql 句柄）由装配方持有，这里只断开引用。</summary>
    public void Destroy()
    {
        _fdb = null;
    }

    /// <summary>MySqlM2DataDB.pas:121-124 <c>GetAuctionDBClass</c> → <c>TMySqlAuctionDB</c>。</summary>
    protected Type GetAuctionDBClass() => typeof(TMySqlAuctionDB);

    /// <summary>MySqlM2DataDB.pas:126-129 <c>GetStorageDBClass</c> → <c>TMySqlStorageDB</c>（未移植，见报告）。</summary>
    protected Type GetStorageDBClass() => typeof(object);

    /// <summary>MySqlM2DataDB.pas:131-134 <c>GetUserShopDBClass</c> → <c>TMySqlUserShopDB</c>。</summary>
    protected Type GetUserShopDBClass() => typeof(TMySqlUserShopDB);

    /// <summary>M2DataCommon.pas:480 <c>property Owner: TM2DataDB read FOwner;</c>。</summary>
    public IDbLayerHost Owner => _owner;

    /// <summary>MySqlM2DataDB.pas:1379-1382 <c>GetDataBase: TObject</c>。</summary>
    public object? DataBase => _fdb;

    /// <summary>M2DataCommon.pas:487-488 的两个子库属性（本车道只装配 AuctionDB/UserShopDB 的类型）。</summary>
    public IAuctionDb? AuctionDB { get; set; }

    /// <summary>M2DataCommon.pas:488 <c>property UserShopDB: TUserShopDB</c>。</summary>
    public IUserShopDb? UserShopDB { get; set; }

    /// <summary>M2DataCommon.pas:501 <c>property IsInitOK: Boolean</c>。</summary>
    public bool IsInitOK { get; private set; }

    /// <summary>底层数据库（测试/装配用）。</summary>
    public IMySqlDatabase? Database => _fdb;

    // ------------------------------------------------------------------
    // UpdaeDB_1 .. UpdaeDB_5（原文 136-235；原文方法名就是拼错的 Updae）
    // ------------------------------------------------------------------

    /// <summary>MySqlM2DataDB.pas:136-149 <c>UpdaeDB_1</c>（原文如此：方法名拼作 Updae）。
    /// 硬编码 SQL：两条 ItemProperty 的 ALTER + 一条 REPLACE 版本号，裸 except 只 RollBack。</summary>
    private void UpdaeDB_1()
    {
        _fdb!.StartTransaction();                                          // 138
        try
        {
            // 141
            _fdb.Exec("ALTER TABLE ItemProperty ADD COLUMN `Value2` INTEGER DEFAULT 0;");
            // 142
            _fdb.Exec("ALTER TABLE ItemProperty ADD COLUMN `Value3` INTEGER DEFAULT 0;");
            // 144（原文 MYSQL_M2DBVERSION = '20200916'，MySqlCreateTableSql.pas:2803）
            _fdb.Exec("REPLACE INTO db_constant(ConstName, ConstValue) values (\"m2data_version\", "
                      + MySqlCreateTableSql.MYSQL_M2DBVERSION + ");");
            _fdb.Commit();                                                 // 145
        }
        catch
        {
            _fdb.RollBack();                                               // 147（原文裸 except，无 MainOutMessage）
        }
    }

    /// <summary>MySqlM2DataDB.pas:151-163 <c>UpdaeDB_2</c>（原文如此）。硬编码 SQL：ItemFlute 加 OverlapCount。</summary>
    private void UpdaeDB_2()
    {
        _fdb!.StartTransaction();                                          // 153
        try
        {
            // 156
            _fdb.Exec("ALTER TABLE ItemFlute ADD COLUMN `OverlapCount` INTEGER DEFAULT 0;");
            // 158
            _fdb.Exec("REPLACE INTO db_constant(ConstName, ConstValue) values (\"m2data_version\", "
                      + MySqlCreateTableSql.MYSQL_M2DBVERSION + ");");
            _fdb.Commit();                                                 // 159
        }
        catch
        {
            _fdb.RollBack();                                               // 161
        }
    }

    /// <summary>MySqlM2DataDB.pas:165-178 <c>UpdaeDB_3</c>（原文如此）。硬编码 SQL：Items 加 NewExpand3/NewExpand4。</summary>
    private void UpdaeDB_3()
    {
        _fdb!.StartTransaction();                                          // 167
        try
        {
            // 170
            _fdb.Exec("ALTER TABLE Items ADD COLUMN `NewExpand3` INTEGER DEFAULT 0;");
            // 171
            _fdb.Exec("ALTER TABLE Items ADD COLUMN `NewExpand4` INTEGER DEFAULT 0;");
            // 173
            _fdb.Exec("REPLACE INTO db_constant(ConstName, ConstValue) values (\"m2data_version\", "
                      + MySqlCreateTableSql.MYSQL_M2DBVERSION + ");");
            _fdb.Commit();                                                 // 174
        }
        catch
        {
            _fdb.RollBack();                                               // 176
        }
    }

    /// <summary>MySqlM2DataDB.pas:180-190 <c>UpdaeDB_4</c>（原文如此）。硬编码 SQL：ItemProperty 加 HintModule。</summary>
    private void UpdaeDB_4()
    {
        _fdb!.StartTransaction();                                          // 182
        try
        {
            // 184
            _fdb.Exec("ALTER TABLE ItemProperty ADD COLUMN `HintModule` INTEGER DEFAULT 0;");
            // 185
            _fdb.Exec("REPLACE INTO db_constant(ConstName, ConstValue) values (\"m2data_version\", "
                      + MySqlCreateTableSql.MYSQL_M2DBVERSION + ");");
            _fdb.Commit();                                                 // 186
        }
        catch
        {
            _fdb.RollBack();                                               // 188
        }
    }

    /// <summary>MySqlM2DataDB.pas:192-235 <c>UpdaeDB_5</c>（原文如此）：重建三张 AddData* 表
    /// （SQL 在 <see cref="MySqlM2DataDbScripts.UpdaeDB_5_L198_S"/>，含 sLineBreak = \r\n）+ REPLACE 版本号。</summary>
    private void UpdaeDB_5()
    {
        string s = MySqlM2DataDbScripts.UpdaeDB_5_L198_S;                   // 198-227

        _fdb!.StartTransaction();                                          // 196
        try
        {
            _fdb.Exec(s);                                                  // 229
            // 230
            _fdb.Exec("REPLACE INTO db_constant(ConstName, ConstValue) values (\"m2data_version\", "
                      + MySqlCreateTableSql.MYSQL_M2DBVERSION + ");");
            _fdb.Commit();                                                 // 231
        }
        catch
        {
            _fdb.RollBack();                                               // 233
        }
    }

    // ------------------------------------------------------------------
    // DoInit（原文 238-626）
    // ------------------------------------------------------------------

    /// <summary>
    /// MySqlM2DataDB.pas:238-626 <c>DoInit</c>。
    /// Connect（CLIENT_MULTI_STATEMENTS）→ CharacterSetName := 'utf8' → 版本探针 → UpdaeDB_N 升级
    /// → 20 条语句注册 + Prepare。<b>没有建表</b>（建表脚本由装配方用 MySqlCreateTableSql 执行）。
    /// </summary>
    public void DoInit()
    {
        // 243：FDB.Connect(g_sDataSaveDBServer, g_sDataSaveDBUser, g_sDataSaveDBPassword,
        //                  g_sDataSaveDataBase, g_wDataSaveDBPort, CLIENT_MULTI_STATEMENTS)
        _fdb!.Connect(MySqlM2DataDbSeam.g_sDataSaveDBServer,
                      MySqlM2DataDbSeam.g_sDataSaveDBUser,
                      MySqlM2DataDbSeam.g_sDataSaveDBPassword,
                      MySqlM2DataDbSeam.g_sDataSaveDataBase,
                      MySqlM2DataDbSeam.g_wDataSaveDBPort,
                      CLIENT_MULTI_STATEMENTS);

        _fdb.CharacterSetName = "utf8";                                    // 244

        _fLastRequestTick = DbLayerGlobals.GetTickCount();                 // 245

        // 247
        int dbVersion = 0;

        // 248-259：select ConstValue from db_constant where ConstName = 'm2data_version';
        IMySqlStatement sm = _fdb.AddSQLStatement("get_db_constant_value");
        try
        {
            sm.Sql = MySqlM2DataDbScripts.DoInit_L250_sm_Sql;              // 250
            sm.Prepare();                                                  // 251
            if (sm.Query() && sm.Fetch())                                  // 252
            {
                dbVersion = sm.OrderGetColumnValueInt;                     // 254
            }
            sm.Reset();                                                    // 256
        }
        finally
        {
            // MySqlM2DataDB.pas:258 FDB.Statements.Clear
            // 接缝：IMySqlDatabase.ClearStatements()（DbSeam.cs，语义 = 清空本次登记的语句清单，
            // 已返回的语句实例仍有效 —— 原文 Clear 只清 TList，不释放 TSQLStatement）。
            // 效果：DoInit 结束时语句表恰好剩 20 条物品语句（探针语句随之消失，且**不** Finalize）。
            _fdb.ClearStatements();
        }

        // 261-290：版本分支（精确 =；版本号集合与 SQLite 版完全不同）
        if (dbVersion == 20180615)
        {
            UpdaeDB_1();
            UpdaeDB_2();
            UpdaeDB_3();
            UpdaeDB_4();
            UpdaeDB_5();
        }
        else if (dbVersion == 20190314)
        {
            UpdaeDB_2();
            UpdaeDB_3();
            UpdaeDB_4();
            UpdaeDB_5();
        }
        else if (dbVersion == 20190606)
        {
            UpdaeDB_3();
            UpdaeDB_4();
            UpdaeDB_5();
        }
        else if (dbVersion == 20190929)
        {
            UpdaeDB_4();
            UpdaeDB_5();
        }
        else if (dbVersion == 20200813)
        {
            UpdaeDB_5();
        }

        // 292-625：物品表语句（顺序 = 原文 DoInit 顺序；SQL 逐字来自 SqlStatements.MySqlM2DataDB.cs）
        _fStatementInsertItems = _fdb.AddSQLStatement("InsertItems");      // 293
        _fStatementInsertItems.Sql = MySqlM2DataDbStatements.InsertItems;
        _fStatementInsertItems.Prepare();                                  // 329

        _fStatementInsertItemValueAdd = _fdb.AddSQLStatement("InsertItemValueAdd");   // 332
        _fStatementInsertItemValueAdd.Sql = MySqlM2DataDbStatements.InsertItemValueAdd;
        _fStatementInsertItemValueAdd.Prepare();                           // 341

        _fStatementInsertItemElementAdd = _fdb.AddSQLStatement("InsertItemElementAdd");   // 343
        _fStatementInsertItemElementAdd.Sql = MySqlM2DataDbStatements.InsertItemElementAdd;
        _fStatementInsertItemElementAdd.Prepare();                         // 352

        _fStatementInsertItemAddDataByte = _fdb.AddSQLStatement("InsertItemAddDataByte");   // 354
        _fStatementInsertItemAddDataByte.Sql = MySqlM2DataDbStatements.InsertItemAddDataByte;
        _fStatementInsertItemAddDataByte.Prepare();                        // 363

        _fStatementInsertItemAddDataInt = _fdb.AddSQLStatement("InsertItemAddDataInt");   // 365
        _fStatementInsertItemAddDataInt.Sql = MySqlM2DataDbStatements.InsertItemAddDataInt;
        _fStatementInsertItemAddDataInt.Prepare();                         // 374

        _fStatementInsertItemAddDataText = _fdb.AddSQLStatement("InsertItemAddDataText");   // 376
        _fStatementInsertItemAddDataText.Sql = MySqlM2DataDbStatements.InsertItemAddDataText;
        _fStatementInsertItemAddDataText.Prepare();                        // 385

        _fStatementInsertItemFlute = _fdb.AddSQLStatement("InsertItemFlute");   // 387
        _fStatementInsertItemFlute.Sql = MySqlM2DataDbStatements.InsertItemFlute;
        _fStatementInsertItemFlute.Prepare();                              // 397

        _fStatementInsertItemProgress = _fdb.AddSQLStatement("InsertItemProgress");   // 399
        _fStatementInsertItemProgress.Sql = MySqlM2DataDbStatements.InsertItemProgress;
        _fStatementInsertItemProgress.Prepare();                           // 415

        _fStatementInsertItemProperty = _fdb.AddSQLStatement("InsertItemProperty");   // 417
        _fStatementInsertItemProperty.Sql = MySqlM2DataDbStatements.InsertItemProperty;
        _fStatementInsertItemProperty.Prepare();                           // 433

        _fStatementGetItems = _fdb.AddSQLStatement("SelectItems");         // 435
        _fStatementGetItems.Sql = MySqlM2DataDbStatements.SelectItems;
        _fStatementGetItems.Prepare();                                     // 468

        _fStatementGetItems_Sort = _fdb.AddSQLStatement("SelectItems_Sort");   // 470
        _fStatementGetItems_Sort.Sql = MySqlM2DataDbStatements.SelectItems_Sort;
        _fStatementGetItems_Sort.Prepare();                                // 503

        _fStatementGetItem = _fdb.AddSQLStatement("SelectItem");           // 506
        _fStatementGetItem.Sql = MySqlM2DataDbStatements.SelectItem;
        _fStatementGetItem.Prepare();                                      // 538

        // 原文 540 的 label 就是 'SelectitemValueAdd'（小写 i），逐字保留。
        _fStatementGetItemValueAdd = _fdb.AddSQLStatement("SelectitemValueAdd");
        _fStatementGetItemValueAdd.Sql = MySqlM2DataDbStatements.SelectitemValueAdd;
        _fStatementGetItemValueAdd.Prepare();                              // 547

        // 原文 549 的 label 就是 'SelectitemElementAdd'（小写 i），逐字保留。
        _fStatementGetItemElementAdd = _fdb.AddSQLStatement("SelectitemElementAdd");
        _fStatementGetItemElementAdd.Sql = MySqlM2DataDbStatements.SelectitemElementAdd;
        _fStatementGetItemElementAdd.Prepare();                            // 556

        // 原文 558 的 label 就是 'SelectitemAddDataByte'（小写 i），逐字保留。
        _fStatementGetItemAddDataByte = _fdb.AddSQLStatement("SelectitemAddDataByte");
        _fStatementGetItemAddDataByte.Sql = MySqlM2DataDbStatements.SelectitemAddDataByte;
        _fStatementGetItemAddDataByte.Prepare();                           // 565

        // 原文 567 的 label 就是 'SelectitemAddDataInt'（小写 i），逐字保留。
        _fStatementGetItemAddDataInt = _fdb.AddSQLStatement("SelectitemAddDataInt");
        _fStatementGetItemAddDataInt.Sql = MySqlM2DataDbStatements.SelectitemAddDataInt;
        _fStatementGetItemAddDataInt.Prepare();                            // 574

        // 原文 576 的 label 就是 'SelectitemAddDataText'（小写 i），逐字保留。
        _fStatementGetItemAddDataText = _fdb.AddSQLStatement("SelectitemAddDataText");
        _fStatementGetItemAddDataText.Sql = MySqlM2DataDbStatements.SelectitemAddDataText;
        _fStatementGetItemAddDataText.Prepare();                           // 583

        // 原文 585 的 label 就是 'SelectitemFlute'（小写 i），逐字保留。
        _fStatementGetItemFlute = _fdb.AddSQLStatement("SelectitemFlute");
        _fStatementGetItemFlute.Sql = MySqlM2DataDbStatements.SelectitemFlute;
        _fStatementGetItemFlute.Prepare();                                 // 593

        _fStatementGetItemProgress = _fdb.AddSQLStatement("SelectItemProgress");   // 595
        _fStatementGetItemProgress.Sql = MySqlM2DataDbStatements.SelectItemProgress;
        _fStatementGetItemProgress.Prepare();                              // 609

        _fStatementGetItemProperty = _fdb.AddSQLStatement("SelectItemProperty");   // 611
        _fStatementGetItemProperty.Sql = MySqlM2DataDbStatements.SelectItemProperty;
        _fStatementGetItemProperty.Prepare();                              // 625

        // ★ 原文 MySqlM2DataDB.pas:238-626 的 DoInit **不设** FIsInitOK；
        //   由 TM2DataDB.Init（M2DataCommon.pas:1638）设置，见本文件 Init()。
    }

    // ------------------------------------------------------------------
    // DoFinal（原文 628-738）
    // ------------------------------------------------------------------

    /// <summary>
    /// MySqlM2DataDB.pas:628-738 <c>DoFinal</c>。
    /// ★ 原文缺陷（逐字保留）：**没有** FStatementGetItems / FStatementGetItems_Sort 的
    /// Finalize 分支（其它 18 条都有），与 SQLite 版同病。
    /// </summary>
    public void DoFinal()
    {
        // 原文 630：inherited（TM2DataDB.DoFinal 是 abstract，无实现体）

        if (_fStatementInsertItems != null) { _fStatementInsertItems.StatementFinalize(); _fStatementInsertItems = null; }
        if (_fStatementInsertItemValueAdd != null) { _fStatementInsertItemValueAdd.StatementFinalize(); _fStatementInsertItemValueAdd = null; }
        if (_fStatementInsertItemElementAdd != null) { _fStatementInsertItemElementAdd.StatementFinalize(); _fStatementInsertItemElementAdd = null; }
        if (_fStatementInsertItemAddDataByte != null) { _fStatementInsertItemAddDataByte.StatementFinalize(); _fStatementInsertItemAddDataByte = null; }
        if (_fStatementInsertItemAddDataInt != null) { _fStatementInsertItemAddDataInt.StatementFinalize(); _fStatementInsertItemAddDataInt = null; }
        if (_fStatementInsertItemAddDataText != null) { _fStatementInsertItemAddDataText.StatementFinalize(); _fStatementInsertItemAddDataText = null; }
        if (_fStatementInsertItemFlute != null) { _fStatementInsertItemFlute.StatementFinalize(); _fStatementInsertItemFlute = null; }
        if (_fStatementInsertItemProgress != null) { _fStatementInsertItemProgress.StatementFinalize(); _fStatementInsertItemProgress = null; }
        if (_fStatementInsertItemProperty != null) { _fStatementInsertItemProperty.StatementFinalize(); _fStatementInsertItemProperty = null; }

        // 原文 685-737：只有 GetItem*，**没有** GetItems / GetItems_Sort（原文如此）。
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

    /// <summary>MySqlM2DataDB.pas:740-960 <c>DoLoadItemsFromDB</c>。</summary>
    private void DoLoadItemsFromDB(int parentID, int itemType, bool isSort, List<TUserItem> list)
    {
        // 751-754
        IMySqlStatement statement = isSort ? _fStatementGetItems_Sort! : _fStatementGetItems!;

        statement.Reset();                                                 // 756
        statement.OrderBindParamInt(parentID);                             // 757
        statement.OrderBindParamInt(itemType);                             // 758
        if (statement.Query())                                             // 759
        {
            while (statement.Fetch())                                      // 761
            {
                // 763：FillChar(UserItem, SizeOf(UserItem), 0)
                TUserItem userItem = M2ItemDbAccess.Zero();

                int itemIndex = statement.OrderGetColumnValueInt;          // 765
                userItem.MakeIndex = statement.OrderGetColumnValueInt;     // 766
                userItem.wIndex = (ushort)statement.OrderGetColumnValueInt;          // 767
                userItem.NameStr = statement.OrderGetColumnValueText;      // 768
                userItem.Dura = (ushort)statement.OrderGetColumnValueInt;            // 769
                userItem.DuraMax = (ushort)statement.OrderGetColumnValueInt;         // 770
                userItem.dwHeroM2DressEffect = (uint)statement.OrderGetColumnValueInt;   // 771
                userItem.btUpgradeCount = (byte)statement.OrderGetColumnValueInt;    // 772
                userItem.boStartTime = statement.OrderGetColumnValueBool ? (byte)1 : (byte)0;   // 773
                userItem.nLimitTime = statement.OrderGetColumnValueInt;    // 774
                userItem.btHeroM2Light = (byte)statement.OrderGetColumnValueInt;     // 775
                userItem.btColor = (byte)statement.OrderGetColumnValueInt;           // 776
                userItem.boIsBind = statement.OrderGetColumnValueBool ? (byte)1 : (byte)0;      // 777
                userItem.btBindOption = (byte)statement.OrderGetColumnValueInt;      // 778
                userItem.wEffect = (ushort)statement.OrderGetColumnValueInt;         // 779
                userItem.wNewLooks = (ushort)statement.OrderGetColumnValueInt;       // 780
                userItem.wNewShape = (ushort)statement.OrderGetColumnValueInt;       // 781
                userItem.btFluteCount = (byte)statement.OrderGetColumnValueInt;      // 782
                userItem.CustomProperty.TextStr = statement.OrderGetColumnValueText; // 783
                userItem.CustomProperty.btTextColor = (byte)statement.OrderGetColumnValueInt;   // 784
                userItem.ItemFrom.ItemForm = (TItemFormType)statement.OrderGetColumnValueInt;   // 785
                userItem.ItemFrom.MapName = statement.OrderGetColumnValueText;      // 786
                userItem.ItemFrom.MonName = statement.OrderGetColumnValueText;      // 787
                userItem.ItemFrom.MakerName = statement.OrderGetColumnValueText;    // 788
                userItem.ItemFrom.DateTime = statement.OrderGetColumnValueDouble;   // 789
                userItem.wInsuranceCount = (ushort)statement.OrderGetColumnValueInt; // 790
                userItem.wNewExpand3 = (ushort)statement.OrderGetColumnValueInt;     // 791
                userItem.wNewExpand4 = (ushort)statement.OrderGetColumnValueInt;     // 792

                // ---- FStatementGetItemValueAdd（794-810） ----
                _fStatementGetItemValueAdd!.Reset();                       // 794
                _fStatementGetItemValueAdd.OrderBindParamInt(parentID);    // 795
                _fStatementGetItemValueAdd.OrderBindParamInt(itemType);    // 796
                _fStatementGetItemValueAdd.OrderBindParamInt(itemIndex);   // 797
                if (_fStatementGetItemValueAdd.Query())                    // 798
                {
                    while (_fStatementGetItemValueAdd.Fetch())             // 800
                    {
                        int index2 = _fStatementGetItemValueAdd.OrderGetColumnValueInt;   // 802
                        if (index2 >= M2ItemDbAccess.ValueLow && index2 <= M2ItemDbAccess.ValueHigh)
                        {
                            int value = _fStatementGetItemValueAdd.OrderGetColumnValueInt;   // 805
                            M2ItemDbAccess.SetValue(ref userItem, index2, value);            // 806
                        }
                    }
                }
                _fStatementGetItemValueAdd.Reset();                        // 810

                // ---- FStatementGetItemElementAdd（812-828） ----
                _fStatementGetItemElementAdd!.Reset();                     // 812
                _fStatementGetItemElementAdd.OrderBindParamInt(parentID);  // 813
                _fStatementGetItemElementAdd.OrderBindParamInt(itemType);  // 814
                _fStatementGetItemElementAdd.OrderBindParamInt(itemIndex); // 815
                if (_fStatementGetItemElementAdd.Query())                  // 816
                {
                    while (_fStatementGetItemElementAdd.Fetch())           // 818
                    {
                        int index2 = _fStatementGetItemElementAdd.OrderGetColumnValueInt;   // 820
                        if (index2 >= M2ItemDbAccess.NewValueLow && index2 <= M2ItemDbAccess.NewValueHigh)
                        {
                            int value = _fStatementGetItemElementAdd.OrderGetColumnValueInt;   // 823
                            M2ItemDbAccess.SetNewValue(ref userItem, index2, value);           // 824
                        }
                    }
                }
                _fStatementGetItemElementAdd.Reset();                      // 828

                // ---- FStatementGetItemAddDataByte（830-846） ----
                _fStatementGetItemAddDataByte!.Reset();                    // 830
                _fStatementGetItemAddDataByte.OrderBindParamInt(parentID); // 831
                _fStatementGetItemAddDataByte.OrderBindParamInt(itemType); // 832
                _fStatementGetItemAddDataByte.OrderBindParamInt(itemIndex);   // 833
                if (_fStatementGetItemAddDataByte.Query())                 // 834
                {
                    while (_fStatementGetItemAddDataByte.Fetch())          // 836
                    {
                        int index2 = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;   // 838
                        if (index2 >= M2ItemDbAccess.AddDataByteLow && index2 <= M2ItemDbAccess.AddDataByteHigh)
                        {
                            int value = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;   // 841
                            M2ItemDbAccess.SetAddDataByte(ref userItem, index2, value);         // 842
                        }
                    }
                }
                _fStatementGetItemAddDataByte.Reset();                     // 846

                // 原文 848 是一个多余空行，语义不变。

                // ---- FStatementGetItemAddDataInt（849-865） ----
                _fStatementGetItemAddDataInt!.Reset();                     // 849
                _fStatementGetItemAddDataInt.OrderBindParamInt(parentID);  // 850
                _fStatementGetItemAddDataInt.OrderBindParamInt(itemType);  // 851
                _fStatementGetItemAddDataInt.OrderBindParamInt(itemIndex); // 852
                if (_fStatementGetItemAddDataInt.Query())                  // 853
                {
                    while (_fStatementGetItemAddDataInt.Fetch())           // 855
                    {
                        int index2 = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;   // 857
                        if (index2 >= M2ItemDbAccess.AddDataIntLow && index2 <= M2ItemDbAccess.AddDataIntHigh)
                        {
                            int value = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;   // 860
                            M2ItemDbAccess.SetAddDataInt(ref userItem, index2, value);         // 861
                        }
                    }
                }
                _fStatementGetItemAddDataInt.Reset();                      // 865

                // ---- FStatementGetItemAddDataText（867-883） ----
                _fStatementGetItemAddDataText!.Reset();                    // 867
                _fStatementGetItemAddDataText.OrderBindParamInt(parentID); // 868
                _fStatementGetItemAddDataText.OrderBindParamInt(itemType); // 869
                _fStatementGetItemAddDataText.OrderBindParamInt(itemIndex);   // 870
                if (_fStatementGetItemAddDataText.Query())                 // 871
                {
                    while (_fStatementGetItemAddDataText.Fetch())          // 873
                    {
                        int index2 = _fStatementGetItemAddDataText.OrderGetColumnValueInt;   // 875
                        if (index2 >= M2ItemDbAccess.AddDataTextLow && index2 <= M2ItemDbAccess.AddDataTextHigh)
                        {
                            string sTemp = _fStatementGetItemAddDataText.OrderGetColumnValueText;   // 878
                            M2ItemDbAccess.SetAddDataText(ref userItem, index2, sTemp);             // 879
                        }
                    }
                }
                _fStatementGetItemAddDataText.Reset();                     // 883

                // ---- FStatementGetItemFlute（885-904）
                //      ★ 原文 887 绑的是常量 0 而不是 ItemType（与 DoLoadItemFromDB:1099 不一致） ----
                _fStatementGetItemFlute!.Reset();                          // 885
                _fStatementGetItemFlute.OrderBindParamInt(parentID);       // 886
                _fStatementGetItemFlute.OrderBindParamInt(0);              // 887
                _fStatementGetItemFlute.OrderBindParamInt(itemIndex);      // 888
                if (_fStatementGetItemFlute.Query())                       // 889
                {
                    while (_fStatementGetItemFlute.Fetch())                // 891
                    {
                        int index2 = _fStatementGetItemFlute.OrderGetColumnValueInt;   // 893
                        if (index2 >= M2ItemDbAccess.FluteLow && index2 <= M2ItemDbAccess.FluteHigh)
                        {
                            var flute = userItem.GetFlute(index2);
                            flute.GemIndex = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;   // 896
                            flute.GemCount = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;   // 897

                            if (flute.GemIndex > 0 && flute.GemCount == 0) flute.GemCount = 1;          // 899-900
                            userItem.SetFlute(index2, flute);
                        }
                    }
                }
                _fStatementGetItemFlute.Reset();                           // 904

                // ---- FStatementGetItemProgress（906-929） ----
                _fStatementGetItemProgress!.Reset();                       // 906
                _fStatementGetItemProgress.OrderBindParamInt(parentID);    // 907
                _fStatementGetItemProgress.OrderBindParamInt(itemType);    // 908
                _fStatementGetItemProgress.OrderBindParamInt(itemIndex);   // 909
                if (_fStatementGetItemProgress.Query())                    // 910
                {
                    while (_fStatementGetItemProgress.Fetch())             // 912
                    {
                        int index2 = _fStatementGetItemProgress.OrderGetColumnValueInt;   // 914
                        if (index2 >= M2ItemDbAccess.ProgressLow && index2 <= M2ItemDbAccess.ProgressHigh)
                        {
                            TUserItemProgress progress = M2ItemDbAccess.GetProgress(ref userItem, index2);
                            progress.boOpen = _fStatementGetItemProgress.OrderGetColumnValueBool ? (byte)1 : (byte)0;   // 917
                            progress.btNameColor = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;   // 918
                            progress.btCount = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;       // 919
                            progress.btShowType = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;    // 920
                            progress.wMax = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;        // 921
                            progress.wValue = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;      // 922
                            progress.wLevel = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;      // 923
                            progress.NameStr = _fStatementGetItemProgress.OrderGetColumnValueText;            // 924
                            M2ItemDbAccess.SetProgress(ref userItem, index2, progress);
                        }
                    }
                }
                _fStatementGetItemProgress.Reset();                        // 929

                // ---- FStatementGetItemProperty（931-953） ----
                _fStatementGetItemProperty!.Reset();                       // 931
                _fStatementGetItemProperty.OrderBindParamInt(parentID);    // 932
                _fStatementGetItemProperty.OrderBindParamInt(itemType);    // 933
                _fStatementGetItemProperty.OrderBindParamInt(itemIndex);   // 934
                if (_fStatementGetItemProperty.Query())                    // 935
                {
                    while (_fStatementGetItemProperty.Fetch())             // 937
                    {
                        int index2 = _fStatementGetItemProperty.OrderGetColumnValueInt;   // 939
                        if (index2 >= M2ItemDbAccess.PropertyLow && index2 <= M2ItemDbAccess.PropertyHigh)
                        {
                            TCustomProperty property = M2ItemDbAccess.GetProperty(ref userItem, index2);
                            property.btColor = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;       // 942
                            property.btBindType = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;    // 943
                            property.btShowFlag = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;    // 944
                            property.btPercent = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;     // 945
                            // 原文 946 是 btHintmodule（小写 m）；托管侧字段名是 btHintModule。
                            property.btHintModule = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                            // 947-949：nValues[0..2]
                            property.SetValues(new[]
                            {
                                _fStatementGetItemProperty.OrderGetColumnValueInt,     // 947
                                _fStatementGetItemProperty.OrderGetColumnValueInt,     // 948
                                _fStatementGetItemProperty.OrderGetColumnValueInt,     // 949
                            });
                            M2ItemDbAccess.SetProperty(ref userItem, index2, property);
                        }
                    }
                }
                _fStatementGetItemProperty.Reset();                        // 953

                list.Add(userItem);                                        // 955-957（原文 New(PUserItem) + 入 List）
            }
        }
        // 原文 958-960：外层 while/if 之后**没有** Statement.Reset（原文如此，逐字保留）。
    }

    /// <summary>MySqlM2DataDB.pas:962-1169 <c>DoLoadItemFromDB</c>。</summary>
    private void DoLoadItemFromDB(ref TUserItem userItem, int parentID, int itemType, int itemIndex)
    {
        // 968：FillChar(UserItem^, SizeOf(TUserItem), 0)
        userItem = M2ItemDbAccess.Zero();

        _fStatementGetItem!.Reset();                                       // 969
        _fStatementGetItem.OrderBindParamInt(parentID);                    // 970
        _fStatementGetItem.OrderBindParamInt(itemType);                    // 971
        _fStatementGetItem.OrderBindParamInt(itemIndex);                   // 972
        if (_fStatementGetItem.Query() && _fStatementGetItem.Fetch())      // 973
        {
            // 注意：本语句的列从 MakeIndex 开始（没有 ItemIndex 列）
            userItem.MakeIndex = _fStatementGetItem.OrderGetColumnValueInt;             // 975
            userItem.wIndex = (ushort)_fStatementGetItem.OrderGetColumnValueInt;        // 976
            userItem.NameStr = _fStatementGetItem.OrderGetColumnValueText;              // 977
            userItem.Dura = (ushort)_fStatementGetItem.OrderGetColumnValueInt;          // 978
            userItem.DuraMax = (ushort)_fStatementGetItem.OrderGetColumnValueInt;       // 979
            userItem.dwHeroM2DressEffect = (uint)_fStatementGetItem.OrderGetColumnValueInt;   // 980
            userItem.btUpgradeCount = (byte)_fStatementGetItem.OrderGetColumnValueInt;  // 981
            userItem.boStartTime = _fStatementGetItem.OrderGetColumnValueBool ? (byte)1 : (byte)0;   // 982
            userItem.nLimitTime = _fStatementGetItem.OrderGetColumnValueInt;            // 983
            userItem.btHeroM2Light = (byte)_fStatementGetItem.OrderGetColumnValueInt;   // 984
            userItem.btColor = (byte)_fStatementGetItem.OrderGetColumnValueInt;         // 985
            userItem.boIsBind = _fStatementGetItem.OrderGetColumnValueBool ? (byte)1 : (byte)0;      // 986
            userItem.btBindOption = (byte)_fStatementGetItem.OrderGetColumnValueInt;    // 987
            userItem.wEffect = (ushort)_fStatementGetItem.OrderGetColumnValueInt;       // 988
            userItem.wNewLooks = (ushort)_fStatementGetItem.OrderGetColumnValueInt;     // 989
            userItem.wNewShape = (ushort)_fStatementGetItem.OrderGetColumnValueInt;     // 990
            userItem.btFluteCount = (byte)_fStatementGetItem.OrderGetColumnValueInt;    // 991
            userItem.CustomProperty.TextStr = _fStatementGetItem.OrderGetColumnValueText;   // 992
            userItem.CustomProperty.btTextColor = (byte)_fStatementGetItem.OrderGetColumnValueInt;   // 993
            userItem.ItemFrom.ItemForm = (TItemFormType)_fStatementGetItem.OrderGetColumnValueInt;   // 994
            userItem.ItemFrom.MapName = _fStatementGetItem.OrderGetColumnValueText;     // 995
            userItem.ItemFrom.MonName = _fStatementGetItem.OrderGetColumnValueText;     // 996
            userItem.ItemFrom.MakerName = _fStatementGetItem.OrderGetColumnValueText;   // 997
            userItem.ItemFrom.DateTime = _fStatementGetItem.OrderGetColumnValueDouble;  // 998
            userItem.wInsuranceCount = (ushort)_fStatementGetItem.OrderGetColumnValueInt;   // 999
            userItem.wNewExpand3 = (ushort)_fStatementGetItem.OrderGetColumnValueInt;   // 1000
            userItem.wNewExpand4 = (ushort)_fStatementGetItem.OrderGetColumnValueInt;   // 1001
        }
        _fStatementGetItem.Reset();                                        // 1003

        // ---- FStatementGetItemValueAdd（1005-1023） ----
        _fStatementGetItemValueAdd!.Reset();                               // 1005
        _fStatementGetItemValueAdd.OrderBindParamInt(parentID);            // 1006
        _fStatementGetItemValueAdd.OrderBindParamInt(itemType);            // 1007
        _fStatementGetItemValueAdd.OrderBindParamInt(itemIndex);           // 1008
        if (_fStatementGetItemValueAdd.Query())                            // 1010
        {
            while (_fStatementGetItemValueAdd.Fetch())                     // 1012
            {
                int index2 = _fStatementGetItemValueAdd.OrderGetColumnValueInt;   // 1014
                if (index2 >= M2ItemDbAccess.ValueLow && index2 <= M2ItemDbAccess.ValueHigh)
                {
                    int value = _fStatementGetItemValueAdd.OrderGetColumnValueInt;   // 1017
                    M2ItemDbAccess.SetValue(ref userItem, index2, value);            // 1018
                }
            }
        }
        _fStatementGetItemValueAdd.Reset();                                // 1023

        // ---- FStatementGetItemElementAdd（1025-1041） ----
        _fStatementGetItemElementAdd!.Reset();                             // 1025
        _fStatementGetItemElementAdd.OrderBindParamInt(parentID);          // 1026
        _fStatementGetItemElementAdd.OrderBindParamInt(itemType);          // 1027
        _fStatementGetItemElementAdd.OrderBindParamInt(itemIndex);         // 1028
        if (_fStatementGetItemElementAdd.Query())                          // 1029
        {
            while (_fStatementGetItemElementAdd.Fetch())                   // 1031
            {
                int index2 = _fStatementGetItemElementAdd.OrderGetColumnValueInt;   // 1033
                if (index2 >= M2ItemDbAccess.NewValueLow && index2 <= M2ItemDbAccess.NewValueHigh)
                {
                    int value = _fStatementGetItemElementAdd.OrderGetColumnValueInt;   // 1036
                    M2ItemDbAccess.SetNewValue(ref userItem, index2, value);           // 1037
                }
            }
        }
        _fStatementGetItemElementAdd.Reset();                              // 1041

        // ---- FStatementGetItemAddDataByte（1043-1059） ----
        _fStatementGetItemAddDataByte!.Reset();                            // 1043
        _fStatementGetItemAddDataByte.OrderBindParamInt(parentID);         // 1044
        _fStatementGetItemAddDataByte.OrderBindParamInt(itemType);         // 1045
        _fStatementGetItemAddDataByte.OrderBindParamInt(itemIndex);        // 1046
        if (_fStatementGetItemAddDataByte.Query())                         // 1047
        {
            while (_fStatementGetItemAddDataByte.Fetch())                  // 1049
            {
                int index2 = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;   // 1051
                if (index2 >= M2ItemDbAccess.AddDataByteLow && index2 <= M2ItemDbAccess.AddDataByteHigh)
                {
                    int value = _fStatementGetItemAddDataByte.OrderGetColumnValueInt;   // 1054
                    M2ItemDbAccess.SetAddDataByte(ref userItem, index2, value);         // 1055
                }
            }
        }
        _fStatementGetItemAddDataByte.Reset();                             // 1059

        // ---- FStatementGetItemAddDataInt（1061-1077） ----
        _fStatementGetItemAddDataInt!.Reset();                             // 1061
        _fStatementGetItemAddDataInt.OrderBindParamInt(parentID);          // 1062
        _fStatementGetItemAddDataInt.OrderBindParamInt(itemType);          // 1063
        _fStatementGetItemAddDataInt.OrderBindParamInt(itemIndex);         // 1064
        if (_fStatementGetItemAddDataInt.Query())                          // 1065
        {
            while (_fStatementGetItemAddDataInt.Fetch())                   // 1067
            {
                int index2 = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;   // 1069
                if (index2 >= M2ItemDbAccess.AddDataIntLow && index2 <= M2ItemDbAccess.AddDataIntHigh)
                {
                    int value = _fStatementGetItemAddDataInt.OrderGetColumnValueInt;   // 1072
                    M2ItemDbAccess.SetAddDataInt(ref userItem, index2, value);         // 1073
                }
            }
        }
        _fStatementGetItemAddDataInt.Reset();                              // 1077

        // ---- FStatementGetItemAddDataText（1079-1095） ----
        _fStatementGetItemAddDataText!.Reset();                            // 1079
        _fStatementGetItemAddDataText.OrderBindParamInt(parentID);         // 1080
        _fStatementGetItemAddDataText.OrderBindParamInt(itemType);         // 1081
        _fStatementGetItemAddDataText.OrderBindParamInt(itemIndex);        // 1082
        if (_fStatementGetItemAddDataText.Query())                         // 1083
        {
            while (_fStatementGetItemAddDataText.Fetch())                  // 1085
            {
                int index2 = _fStatementGetItemAddDataText.OrderGetColumnValueInt;   // 1087
                if (index2 >= M2ItemDbAccess.AddDataTextLow && index2 <= M2ItemDbAccess.AddDataTextHigh)
                {
                    string sTemp = _fStatementGetItemAddDataText.OrderGetColumnValueText;   // 1090
                    M2ItemDbAccess.SetAddDataText(ref userItem, index2, sTemp);             // 1091
                }
            }
        }
        _fStatementGetItemAddDataText.Reset();                             // 1095

        // ---- FStatementGetItemFlute（1097-1117）
        //      ★ 这里绑的是 ItemType（1099），与 DoLoadItemsFromDB:887 的常量 0 不一致（原文如此） ----
        _fStatementGetItemFlute!.Reset();                                  // 1097
        _fStatementGetItemFlute.OrderBindParamInt(parentID);               // 1098
        _fStatementGetItemFlute.OrderBindParamInt(itemType);               // 1099
        _fStatementGetItemFlute.OrderBindParamInt(itemIndex);              // 1100
        if (_fStatementGetItemFlute.Query())                               // 1102
        {
            while (_fStatementGetItemFlute.Fetch())                        // 1104
            {
                int index2 = _fStatementGetItemFlute.OrderGetColumnValueInt;   // 1106
                if (index2 >= M2ItemDbAccess.FluteLow && index2 <= M2ItemDbAccess.FluteHigh)
                {
                    var flute = userItem.GetFlute(index2);
                    flute.GemIndex = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;   // 1109
                    flute.GemCount = (ushort)_fStatementGetItemFlute.OrderGetColumnValueInt;   // 1110

                    if (flute.GemIndex > 0 && flute.GemCount == 0) flute.GemCount = 1;          // 1112-1113
                    userItem.SetFlute(index2, flute);
                }
            }
        }
        _fStatementGetItemFlute.Reset();                                   // 1117

        // ---- FStatementGetItemProgress（1119-1142） ----
        _fStatementGetItemProgress!.Reset();                               // 1119
        _fStatementGetItemProgress.OrderBindParamInt(parentID);            // 1120
        _fStatementGetItemProgress.OrderBindParamInt(itemType);            // 1121
        _fStatementGetItemProgress.OrderBindParamInt(itemIndex);           // 1122
        if (_fStatementGetItemProgress.Query())                            // 1124
        {
            while (_fStatementGetItemProgress.Fetch())                     // 1126
            {
                int index2 = _fStatementGetItemProgress.OrderGetColumnValueInt;   // 1128
                if (index2 >= M2ItemDbAccess.ProgressLow && index2 <= M2ItemDbAccess.ProgressHigh)
                {
                    TUserItemProgress progress = M2ItemDbAccess.GetProgress(ref userItem, index2);
                    progress.boOpen = _fStatementGetItemProgress.OrderGetColumnValueBool ? (byte)1 : (byte)0;   // 1131
                    progress.btNameColor = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;   // 1132
                    progress.btCount = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;       // 1133
                    progress.btShowType = (byte)_fStatementGetItemProgress.OrderGetColumnValueInt;    // 1134
                    progress.wMax = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;        // 1135
                    progress.wValue = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;      // 1136
                    progress.wLevel = (ushort)_fStatementGetItemProgress.OrderGetColumnValueInt;      // 1137
                    progress.NameStr = _fStatementGetItemProgress.OrderGetColumnValueText;            // 1138
                    M2ItemDbAccess.SetProgress(ref userItem, index2, progress);
                }
            }
        }
        _fStatementGetItemProgress.Reset();                                // 1142

        // ---- FStatementGetItemProperty（1144-1168） ----
        _fStatementGetItemProperty!.Reset();                               // 1144
        _fStatementGetItemProperty.OrderBindParamInt(parentID);            // 1145
        _fStatementGetItemProperty.OrderBindParamInt(itemType);            // 1146
        _fStatementGetItemProperty.OrderBindParamInt(itemIndex);           // 1147
        if (_fStatementGetItemProperty.Query())                            // 1149
        {
            while (_fStatementGetItemProperty.Fetch())                     // 1151
            {
                int index2 = _fStatementGetItemProperty.OrderGetColumnValueInt;   // 1153
                if (index2 >= M2ItemDbAccess.PropertyLow && index2 <= M2ItemDbAccess.PropertyHigh)
                {
                    TCustomProperty property = M2ItemDbAccess.GetProperty(ref userItem, index2);
                    property.btColor = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;       // 1156
                    property.btBindType = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;    // 1157
                    property.btShowFlag = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;    // 1158
                    property.btPercent = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;     // 1159
                    // 原文 1160 是 btHintmodule（小写 m）；托管侧字段名是 btHintModule。
                    property.btHintModule = (byte)_fStatementGetItemProperty.OrderGetColumnValueInt;
                    // 1161-1163：nValues[0..2]
                    property.SetValues(new[]
                    {
                        _fStatementGetItemProperty.OrderGetColumnValueInt,     // 1161
                        _fStatementGetItemProperty.OrderGetColumnValueInt,     // 1162
                        _fStatementGetItemProperty.OrderGetColumnValueInt,     // 1163
                    });
                    M2ItemDbAccess.SetProperty(ref userItem, index2, property);
                }
            }
        }
        _fStatementGetItemProperty.Reset();                                // 1168
    }

    /// <summary>MySqlM2DataDB.pas:1171-1377 <c>DoSaveItemToDB</c>。
    /// 8 段 try/finally（★ 原文在**异常路径也会 Reset**，逐字保留）。</summary>
    private void DoSaveItemToDB(TUserItem userItem, int parentID, int itemType, int itemIndex)
    {
        // 1176-1211
        _fStatementInsertItems!.Reset();
        try
        {
            _fStatementInsertItems.OrderBindParamInt(parentID);                        // 1178
            _fStatementInsertItems.OrderBindParamInt(itemType);                        // 1179
            _fStatementInsertItems.OrderBindParamInt(itemIndex);                       // 1180
            _fStatementInsertItems.OrderBindParamInt(userItem.MakeIndex);              // 1181
            _fStatementInsertItems.OrderBindParamInt(userItem.wIndex);                 // 1182
            _fStatementInsertItems.OrderBindParamText(userItem.NameStr);               // 1183
            _fStatementInsertItems.OrderBindParamInt(userItem.Dura);                   // 1184
            _fStatementInsertItems.OrderBindParamInt(userItem.DuraMax);                // 1185
            _fStatementInsertItems.OrderBindParamInt((int)userItem.dwHeroM2DressEffect);   // 1186
            _fStatementInsertItems.OrderBindParamInt(userItem.btUpgradeCount);         // 1187
            _fStatementInsertItems.OrderBindParamBool(userItem.boStartTime != 0);      // 1188
            _fStatementInsertItems.OrderBindParamInt(userItem.nLimitTime);             // 1189
            _fStatementInsertItems.OrderBindParamInt(userItem.btHeroM2Light);          // 1190
            _fStatementInsertItems.OrderBindParamInt(userItem.btColor);                // 1191
            _fStatementInsertItems.OrderBindParamBool(userItem.boIsBind != 0);         // 1192
            _fStatementInsertItems.OrderBindParamInt(userItem.btBindOption);           // 1193
            _fStatementInsertItems.OrderBindParamInt(userItem.wEffect);                // 1194
            _fStatementInsertItems.OrderBindParamInt(userItem.wNewLooks);              // 1195
            _fStatementInsertItems.OrderBindParamInt(userItem.wNewShape);              // 1196
            _fStatementInsertItems.OrderBindParamInt(userItem.btFluteCount);           // 1197
            _fStatementInsertItems.OrderBindParamText(userItem.CustomProperty.TextStr);   // 1198
            _fStatementInsertItems.OrderBindParamInt(userItem.CustomProperty.btTextColor);   // 1199
            _fStatementInsertItems.OrderBindParamInt((int)userItem.ItemFrom.ItemForm);   // 1200
            _fStatementInsertItems.OrderBindParamText(userItem.ItemFrom.MapName);      // 1201
            _fStatementInsertItems.OrderBindParamText(userItem.ItemFrom.MonName);      // 1202
            _fStatementInsertItems.OrderBindParamText(userItem.ItemFrom.MakerName);    // 1203
            // 1204：原文就是 OrderBindParamDouble（与 SQLite 侧的 OrderBindDouble 名字不同，两个接缝各保留原文名）。
            _fStatementInsertItems.OrderBindParamDouble(userItem.ItemFrom.DateTime);
            _fStatementInsertItems.OrderBindParamInt(userItem.wInsuranceCount);        // 1205
            _fStatementInsertItems.OrderBindParamInt(userItem.wNewExpand3);            // 1206
            _fStatementInsertItems.OrderBindParamInt(userItem.wNewExpand4);            // 1207
            _fStatementInsertItems.Step();                                            // 1208
        }
        finally
        {
            _fStatementInsertItems.Reset();                                           // 1210
        }

        // ---- ItemValueAdd（1216-1232）：btValue[J] <> 0 ----
        try
        {
            for (int j = M2ItemDbAccess.ValueLow; j <= M2ItemDbAccess.ValueHigh; j++)   // 1217
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetValue(ref copy, j) != 0)                          // 1219
                {
                    _fStatementInsertItemValueAdd!.Reset();                            // 1221
                    _fStatementInsertItemValueAdd.OrderBindParamInt(parentID);          // 1222
                    _fStatementInsertItemValueAdd.OrderBindParamInt(itemType);          // 1223
                    _fStatementInsertItemValueAdd.OrderBindParamInt(itemIndex);         // 1224
                    _fStatementInsertItemValueAdd.OrderBindParamInt(j);                 // 1225
                    _fStatementInsertItemValueAdd.OrderBindParamInt(M2ItemDbAccess.GetValue(ref copy, j));   // 1226
                    _fStatementInsertItemValueAdd.Step();                              // 1227
                }
            }
        }
        finally
        {
            _fStatementInsertItemValueAdd!.Reset();                                    // 1231
        }

        // ---- ItemElementAdd（1234-1250）：btNewValue[J] <> 0 ----
        try
        {
            for (int j = M2ItemDbAccess.NewValueLow; j <= M2ItemDbAccess.NewValueHigh; j++)   // 1235
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetNewValue(ref copy, j) != 0)                       // 1237
                {
                    _fStatementInsertItemElementAdd!.Reset();                          // 1239
                    _fStatementInsertItemElementAdd.OrderBindParamInt(parentID);        // 1240
                    _fStatementInsertItemElementAdd.OrderBindParamInt(itemType);        // 1241
                    _fStatementInsertItemElementAdd.OrderBindParamInt(itemIndex);       // 1242
                    _fStatementInsertItemElementAdd.OrderBindParamInt(j);               // 1243
                    _fStatementInsertItemElementAdd.OrderBindParamInt(M2ItemDbAccess.GetNewValue(ref copy, j));   // 1244
                    _fStatementInsertItemElementAdd.Step();                            // 1245
                }
            }
        }
        finally
        {
            _fStatementInsertItemElementAdd!.Reset();                                  // 1249
        }

        // ---- ItemAddDataByte（1252-1268）：btAddDataByte[J] <> 0 ----
        try
        {
            for (int j = M2ItemDbAccess.AddDataByteLow; j <= M2ItemDbAccess.AddDataByteHigh; j++)   // 1253
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetAddDataByte(ref copy, j) != 0)                    // 1255
                {
                    _fStatementInsertItemAddDataByte!.Reset();                         // 1257
                    _fStatementInsertItemAddDataByte.OrderBindParamInt(parentID);       // 1258
                    _fStatementInsertItemAddDataByte.OrderBindParamInt(itemType);       // 1259
                    _fStatementInsertItemAddDataByte.OrderBindParamInt(itemIndex);      // 1260
                    _fStatementInsertItemAddDataByte.OrderBindParamInt(j);              // 1261
                    _fStatementInsertItemAddDataByte.OrderBindParamInt(M2ItemDbAccess.GetAddDataByte(ref copy, j));   // 1262
                    _fStatementInsertItemAddDataByte.Step();                           // 1263
                }
            }
        }
        finally
        {
            _fStatementInsertItemAddDataByte!.Reset();                                 // 1267
        }

        // ---- ItemAddDataInt（1270-1286）：nAddDataInt[J] <> 0 ----
        try
        {
            for (int j = M2ItemDbAccess.AddDataIntLow; j <= M2ItemDbAccess.AddDataIntHigh; j++)   // 1271
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetAddDataInt(ref copy, j) != 0)                     // 1273
                {
                    _fStatementInsertItemAddDataInt!.Reset();                          // 1275
                    _fStatementInsertItemAddDataInt.OrderBindParamInt(parentID);        // 1276
                    _fStatementInsertItemAddDataInt.OrderBindParamInt(itemType);        // 1277
                    _fStatementInsertItemAddDataInt.OrderBindParamInt(itemIndex);       // 1278
                    _fStatementInsertItemAddDataInt.OrderBindParamInt(j);               // 1279
                    _fStatementInsertItemAddDataInt.OrderBindParamInt(M2ItemDbAccess.GetAddDataInt(ref copy, j));   // 1280
                    _fStatementInsertItemAddDataInt.Step();                            // 1281
                }
            }
        }
        finally
        {
            _fStatementInsertItemAddDataInt!.Reset();                                  // 1285
        }

        // ---- ItemAddDataText（1288-1304）：sAddDataText[J] <> '' ----
        try
        {
            for (int j = M2ItemDbAccess.AddDataTextLow; j <= M2ItemDbAccess.AddDataTextHigh; j++)   // 1289
            {
                var copy = userItem;
                if (M2ItemDbAccess.GetAddDataText(ref copy, j) != "")                   // 1291
                {
                    _fStatementInsertItemAddDataText!.Reset();                         // 1293
                    _fStatementInsertItemAddDataText.OrderBindParamInt(parentID);       // 1294
                    _fStatementInsertItemAddDataText.OrderBindParamInt(itemType);       // 1295
                    _fStatementInsertItemAddDataText.OrderBindParamInt(itemIndex);      // 1296
                    _fStatementInsertItemAddDataText.OrderBindParamInt(j);              // 1297
                    _fStatementInsertItemAddDataText.OrderBindParamText(M2ItemDbAccess.GetAddDataText(ref copy, j));   // 1298
                    _fStatementInsertItemAddDataText.Step();                           // 1299
                }
            }
        }
        finally
        {
            _fStatementInsertItemAddDataText!.Reset();                                 // 1303
        }

        // ---- ItemFlute（1308-1325）：Flutes[J].GemIndex <> 0 ----
        try
        {
            for (int j = M2ItemDbAccess.FluteLow; j <= M2ItemDbAccess.FluteHigh; j++)   // 1309
            {
                var copy = userItem;
                TFluteInfo flute = copy.GetFlute(j);
                if (flute.GemIndex != 0)                                                // 1311
                {
                    _fStatementInsertItemFlute!.Reset();                               // 1313
                    _fStatementInsertItemFlute.OrderBindParamInt(parentID);            // 1314
                    _fStatementInsertItemFlute.OrderBindParamInt(itemType);            // 1315
                    _fStatementInsertItemFlute.OrderBindParamInt(itemIndex);           // 1316
                    _fStatementInsertItemFlute.OrderBindParamInt(j);                   // 1317
                    _fStatementInsertItemFlute.OrderBindParamInt(copy.GetFlute(j).GemIndex);   // 1318
                    _fStatementInsertItemFlute.OrderBindParamInt(copy.GetFlute(j).GemCount);   // 1319
                    _fStatementInsertItemFlute.Step();                                 // 1320
                }
            }
        }
        finally
        {
            _fStatementInsertItemFlute!.Reset();                                       // 1324
        }

        // ---- ItemProgress（1327-1350）：★ 原文判据是 Progress[J].boOpen ----
        try
        {
            for (int j = M2ItemDbAccess.ProgressLow; j <= M2ItemDbAccess.ProgressHigh; j++)   // 1328
            {
                var copy = userItem;
                TUserItemProgress progress = M2ItemDbAccess.GetProgress(ref copy, j);
                if (progress.boOpen != 0)                                               // 1330
                {
                    _fStatementInsertItemProgress!.Reset();                            // 1332
                    _fStatementInsertItemProgress.OrderBindParamInt(parentID);          // 1333
                    _fStatementInsertItemProgress.OrderBindParamInt(itemType);          // 1334
                    _fStatementInsertItemProgress.OrderBindParamInt(itemIndex);         // 1335
                    _fStatementInsertItemProgress.OrderBindParamInt(j);                 // 1336
                    _fStatementInsertItemProgress.OrderBindParamBool(progress.boOpen != 0);   // 1337
                    _fStatementInsertItemProgress.OrderBindParamInt(progress.btNameColor);    // 1338
                    _fStatementInsertItemProgress.OrderBindParamInt(progress.btCount);        // 1339
                    _fStatementInsertItemProgress.OrderBindParamInt(progress.btShowType);     // 1340
                    _fStatementInsertItemProgress.OrderBindParamInt(progress.wMax);           // 1341
                    _fStatementInsertItemProgress.OrderBindParamInt(progress.wValue);         // 1342
                    _fStatementInsertItemProgress.OrderBindParamInt(progress.wLevel);         // 1343
                    _fStatementInsertItemProgress.OrderBindParamText(progress.NameStr);       // 1344
                    _fStatementInsertItemProgress.Step();                              // 1345
                }
            }
        }
        finally
        {
            _fStatementInsertItemProgress!.Reset();                                    // 1349
        }

        // ---- ItemProperty（1352-1376）：★ 原文判据：三个 nValues 任一 > 0 ----
        try
        {
            for (int j = M2ItemDbAccess.PropertyLow; j <= M2ItemDbAccess.PropertyHigh; j++)   // 1353
            {
                var copy = userItem;
                TCustomProperty property = M2ItemDbAccess.GetProperty(ref copy, j);
                int[] nValues = property.GetValues();
                if (nValues[0] > 0 || nValues[1] > 0 || nValues[2] > 0)                 // 1355-1356
                {
                    _fStatementInsertItemProperty!.Reset();                            // 1358
                    _fStatementInsertItemProperty.OrderBindParamInt(parentID);          // 1359
                    _fStatementInsertItemProperty.OrderBindParamInt(itemType);          // 1360
                    _fStatementInsertItemProperty.OrderBindParamInt(itemIndex);         // 1361
                    _fStatementInsertItemProperty.OrderBindParamInt(j);                 // 1362
                    _fStatementInsertItemProperty.OrderBindParamInt(property.btColor);      // 1363
                    _fStatementInsertItemProperty.OrderBindParamInt(property.btBindType);   // 1364
                    _fStatementInsertItemProperty.OrderBindParamInt(property.btShowFlag);   // 1365
                    _fStatementInsertItemProperty.OrderBindParamInt(property.btPercent);    // 1366
                    // 原文 1367 是 btHintmodule（小写 m）；托管侧字段名是 btHintModule。
                    _fStatementInsertItemProperty.OrderBindParamInt(property.btHintModule);
                    _fStatementInsertItemProperty.OrderBindParamInt(nValues[0]);         // 1368
                    _fStatementInsertItemProperty.OrderBindParamInt(nValues[1]);         // 1369
                    _fStatementInsertItemProperty.OrderBindParamInt(nValues[2]);         // 1370
                    _fStatementInsertItemProperty.Step();                              // 1371
                }
            }
        }
        finally
        {
            _fStatementInsertItemProperty!.Reset();                                    // 1375
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

    /// <summary>MySqlM2DataDB.pas:1384-1387 <c>OnRequest</c>：每次 DB 请求把 FLastRequestTick 刷新为当前 tick。</summary>
    private void OnRequest()
    {
        _fLastRequestTick = DbLayerGlobals.GetTickCount();                 // 1386
    }

    /// <summary>
    /// MySqlM2DataDB.pas:1389-1396 <c>Run</c>：<c>inherited</c>（TM2DataDB.Run 是空实现，
    /// M2DataCommon.pas:1699-1702）+ 距上次请求 ≥ 10 分钟时 <c>Exec('select 1')</c> 保活。
    /// </summary>
    public void Run()
    {
        // 1391：inherited（基类 Run 空实现，托管侧无基类）

        // 1392：if (FDB <> nil) and (FDB.MySQL <> nil) and (MyGetTickCount - FLastRequestTick >= 10 * 60000)
        if (_fdb is not null
            && _fdb.MySql is not null
            && unchecked(DbLayerGlobals.GetTickCount() - _fLastRequestTick) >= RunKeepAliveTick)
        {
            _fdb.Exec("select 1");                                         // 1394（原文无分号）
        }
    }
}

/// <summary>
/// MySqlM2DataDB.pas:243 的 5 个连接参数全局（M2Share.pas:5600-5604 <c>g_sDataSaveDBServer</c> /
/// <c>g_wDataSaveDBPort</c> / <c>g_sDataSaveDBUser</c> / <c>g_sDataSaveDBPassword</c> /
/// <c>g_sDataSaveDataBase</c>）。
/// </summary>
/// <remarks>
/// 接缝：M2Server 侧 M2Share 尚未移植这 5 个成员（既有 <c>M2Config</c> 里没有），
/// 按 DbSeam.cs 的惯例以可写静态字段承载；待 M2Share/M2Config 落地后改回转调
/// （本车道不顺手改 src/GXX.M2Server/Engine/**，也不改 DbSeam.cs）。
/// 默认值与 M2Share.pas:5600-5604 / GShare.pas:84-88 声明一致（端口 3306）。
/// </remarks>
public static class MySqlM2DataDbSeam
{
    /// <summary>M2Share.pas:5601 <c>g_sDataSaveDBServer: string = ''</c>。</summary>
    public static string g_sDataSaveDBServer = "";

    /// <summary>M2Share.pas:5603 <c>g_sDataSaveDBUser: string = ''</c>。</summary>
    public static string g_sDataSaveDBUser = "";

    /// <summary>M2Share.pas:5604 <c>g_sDataSaveDBPassword: string = ''</c>。</summary>
    public static string g_sDataSaveDBPassword = "";

    /// <summary>M2Share.pas:5605 <c>g_sDataSaveDataBase: string = ''</c>（DBShare.pas:877 同名全局）。</summary>
    public static string g_sDataSaveDataBase = "";

    /// <summary>M2Share.pas:5602 <c>g_wDataSaveDBPort: Word = 3306</c>。</summary>
    public static ushort g_wDataSaveDBPort = 3306;

    /// <summary>恢复默认（测试用）。</summary>
    public static void ResetDefaults()
    {
        g_sDataSaveDBServer = "";
        g_sDataSaveDBUser = "";
        g_sDataSaveDBPassword = "";
        g_sDataSaveDataBase = "";
        g_wDataSaveDBPort = 3306;
    }
}

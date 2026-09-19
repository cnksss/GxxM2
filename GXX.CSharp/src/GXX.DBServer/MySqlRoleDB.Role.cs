using System;
using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// MySqlRoleDB.pas:221-250 + 5819-6203 `TMySqlRoleDB`（1:1）。
/// 公开面（Create/Destroy/Run/Init/Fainal/Lock/UnLock）沿用 RoleDB.pas:266-350 的 TRoleDB 形状，
/// 但**不继承** GXX.LoginSrv 的接缝基类（本车道自带一套 THumanDBBase/THeroDBBase 骨架，
/// GetHumanDBClass/GetHeroDBClass 的"虚构造"用工厂委托还原）。
/// </summary>
public sealed class TMySqlRoleDB
{
    /// <summary>MySqlRoleDB.pas:224 `FMySqlLib: TMySQLLib`。</summary>
    private IRoleMySqlLib? FMySqlLib;

    /// <summary>MySqlRoleDB.pas:225 `FDB: TMySqlDataBase`。</summary>
    private IRoleMySqlDatabase? FDB;

    /// <summary>MySqlRoleDB.pas:223 `FLastRequestTick: LongWord`。</summary>
    private uint FLastRequestTick;

    private readonly object FLock = new object();

    /// <summary>MySqlRoleDB.pas:226-237 的 OnRequest 回调与 UpdateDB_1..10。</summary>
    private readonly IRoleMySqlDatabaseFactory FFactory;

    /// <summary>RoleDB.pas:1263 `FHumanDB := GetHumanDBClass.Create(Self)`。</summary>
    public TMySqlHumanDB? HumanDB { get; private set; }

    /// <summary>RoleDB.pas:1264 `FHeroDB := GetHeroDBClass.Create(Self)`。</summary>
    public TMySqlHeroDB? HeroDB { get; private set; }

    /// <summary>内部接缝用：原文的 `TMySqlRoleDB(Owner).FDB`。</summary>
    internal IRoleMySqlDatabase DB => FDB ?? throw new InvalidOperationException("TMySqlRoleDB.DB not create");

    /// <summary>未接入工厂时用原生接缝桩（CreateLib/CreateDatabase 会抛出并指明接入点）。</summary>
    public TMySqlRoleDB() : this(RoleMySqlNativeSeamFactory.Instance)
    {
    }

    public TMySqlRoleDB(IRoleMySqlDatabaseFactory factory)
    {
        FFactory = factory;
        // MySqlRoleDB.pas:5821-5826 `Create`：FDB := nil; FMySqlLib := nil;
        FDB = null;
        FMySqlLib = null;
    }

    /// <summary>MySqlRoleDB.pas:5838-5841 `GetHumanDBClass`（原文 `Result := TMySqlHumanDB`）。</summary>
    private THumanDBBase GetHumanDBClass() => new TMySqlHumanDB(this);

    /// <summary>MySqlRoleDB.pas:5843-5846 `GetHeroDBClass`（原文 `Result := TMySqlHeroDB`）。</summary>
    private THeroDBBase GetHeroDBClass() => new TMySqlHeroDB(this);

    // ==========================================================================================
    // 事务（MySqlRoleDB.pas:6175-6188）
    // ==========================================================================================

    /// <summary>MySqlRoleDB.pas:6175-6178 `TMySqlRoleDB.Commit`。</summary>
    public void Commit() => FDB!.Commit();

    /// <summary>MySqlRoleDB.pas:6180-6183 `TMySqlRoleDB.RollBack`（注意原文这里是大写 B 的 RollBack）。</summary>
    public void RollBack() => FDB!.RollBack();

    /// <summary>MySqlRoleDB.pas:6185-6188 `TMySqlRoleDB.BeginTransaction`。</summary>
    public void BeginTransaction() => FDB!.StartTransaction();

    // ==========================================================================================
    // 锁（RoleDB.pas:1297-1305 `TRoleDB.Lock` / `UnLock`）
    // ==========================================================================================

    public void Lock() => System.Threading.Monitor.Enter(FLock);

    public void UnLock() => System.Threading.Monitor.Exit(FLock);

    // ==========================================================================================
    // 生命周期
    // ==========================================================================================

    /// <summary>RoleDB.pas:1275-1278 `TRoleDB.Init` → `DoInit`。</summary>
    public void Init() => DoInit();

    /// <summary>RoleDB.pas:1280-1283 `TRoleDB.Fainal` → `DoFainal`。</summary>
    public void Fainal() => DoFainal();

    /// <summary>
    /// MySqlRoleDB.pas:6035-6155 `TMySqlRoleDB.DoInit`。
    /// FDB 已存在时整段 if 直接跳过（原文 `if not Assigned(FDB) then`），一个语句都不准备。
    /// </summary>
    public void DoInit()
    {
        if (FDB is null)
        {
            FMySqlLib = FFactory.CreateLib();                    // TMySQLLib.Create(nil) + Load('', 'libmysql-32.dll')

            FDB = FFactory.CreateDatabase(FMySqlLib);            // TMySQLDataBase.Create(FMySqlLib)
            FDB.Init();
            FDB.OnRequest += OnRequest;

            // 原文 Connect(Server, User, Password, DataBase, Port, CLIENT_MULTI_STATEMENTS)
            FDB.Connect(MySqlRoleDbGlobals.g_sDataSaveDBServer, MySqlRoleDbGlobals.g_sDataSaveDBUser,
                        MySqlRoleDbGlobals.g_sDataSaveDBPassword, MySqlRoleDbGlobals.g_sDataSaveDataBase,
                        MySqlRoleDbGlobals.g_wDataSaveDBPort, MySqlClientFlags.CLIENT_MULTI_STATEMENTS);
            FDB.CharacterSetName = "utf8";

            int DB_Version = 0;
            IRoleMySqlStatement sm = FDB.AddSQLStatement("get_db_constant_value");
            try
            {
                sm.Sql = "select ConstValue from db_constant where ConstName = 'role_version';";
                sm.Prepare();
                if (sm.Query() && sm.Fetch())
                {
                    DB_Version = sm.OrderGetColumnValueInt;
                }
                sm.Reset();
            }
            finally
            {
                FDB.ClearStatements();
            }

            foreach (int Step in MySqlRoleDbMigration.StepsFor(DB_Version))
            {
                RunUpdateDB(Step);
            }

            FLastRequestTick = DelphiTick.GetTickCount();
        }

        // 原文 `inherited` → TRoleDB.DoInit：构造 FHumanDB / FHeroDB 并逐个 Init
        HumanDB = (TMySqlHumanDB)GetHumanDBClass();
        HeroDB = (TMySqlHeroDB)GetHeroDBClass();
        HumanDB.DoInit();
        HeroDB.DoInit();
    }

    /// <summary>
    /// MySqlRoleDB.pas:5848-6033 的 UpdateDB_n 分发（按步号调对应 SQL 序列，每步自带事务）。
    /// </summary>
    public void RunUpdateDB(int Step)
    {
        switch (Step)
        {
            case 1: UpdateDB_1(); break;
            case 2: UpdateDB_2(); break;
            case 3: UpdateDB_3(); break;
            case 4: UpdateDB_4(); break;
            case 5: UpdateDB_5(); break;
            case 6: UpdateDB_6(); break;
            case 7: UpdateDB_7(); break;
            case 8: UpdateDB_8(); break;
            case 9: UpdateDB_9(); break;
            case 10: UpdateDB_10(); break;
        }
    }

    /// <summary>每步的公共外壳：`FDB.StartTransaction; try Exec...; Commit; except RollBack; end`。</summary>
    private void RunStep(string[] Sqls)
    {
        FDB!.StartTransaction();
        try
        {
            foreach (string S in Sqls) FDB.Exec(S);
            FDB.Commit();
        }
        catch
        {
            FDB.RollBack();
        }
    }

    /// <summary>MySqlRoleDB.pas:5848-5863 `UpdateDB_1`。</summary>
    public void UpdateDB_1() => RunStep(MySqlRoleDbMigration.UpdateDB_1());

    /// <summary>MySqlRoleDB.pas:5865-5906 `UpdateDB_2`（原文有一大段被 (* *) 注释掉的 SQL，此处同删）。</summary>
    public void UpdateDB_2() => RunStep(MySqlRoleDbMigration.UpdateDB_2());

    /// <summary>MySqlRoleDB.pas:5908-5924 `UpdateDB_3`。</summary>
    public void UpdateDB_3() => RunStep(MySqlRoleDbMigration.UpdateDB_3());

    /// <summary>MySqlRoleDB.pas:5926-5939 `UpdateDB_4`。</summary>
    public void UpdateDB_4() => RunStep(MySqlRoleDbMigration.UpdateDB_4());

    /// <summary>MySqlRoleDB.pas:5941-5954 `UpdateDB_5`。</summary>
    public void UpdateDB_5() => RunStep(MySqlRoleDbMigration.UpdateDB_5());

    /// <summary>MySqlRoleDB.pas:5956-5967 `UpdateDB_6`。</summary>
    public void UpdateDB_6() => RunStep(MySqlRoleDbMigration.UpdateDB_6());

    /// <summary>MySqlRoleDB.pas:5969-5984 `UpdateDB_7`。</summary>
    public void UpdateDB_7() => RunStep(MySqlRoleDbMigration.UpdateDB_7());

    /// <summary>MySqlRoleDB.pas:5986-5998 `UpdateDB_8`。</summary>
    public void UpdateDB_8() => RunStep(MySqlRoleDbMigration.UpdateDB_8());

    /// <summary>MySqlRoleDB.pas:6000-6015 `UpdateDB_9`。</summary>
    public void UpdateDB_9() => RunStep(MySqlRoleDbMigration.UpdateDB_9());

    /// <summary>MySqlRoleDB.pas:6017-6033 `UpdateDB_10`。</summary>
    public void UpdateDB_10() => RunStep(MySqlRoleDbMigration.UpdateDB_10());

    /// <summary>
    /// MySqlRoleDB.pas:6157-6173 `TMySqlRoleDB.DoFainal`。
    /// Disconnect + Free FDB，再 Free FMySqlLib；两者都置 nil。
    /// </summary>
    public void DoFainal()
    {
        if (FDB is not null)
        {
            FDB.Disconnect();
            FDB = null;
        }

        if (FMySqlLib is not null)
        {
            FMySqlLib = null;
        }

        HumanDB = null;
        HeroDB = null;
    }

    /// <summary>
    /// MySqlRoleDB.pas:6190-6197 `TMySqlRoleDB.Run`。
    /// 只在「FDB 非空 且 FDB.MySQL 非空 且 GetTickCount - FLastRequestTick &gt;= 10*60000」时发一次 `select 1` 保活。
    /// 注意原文判的是 `&gt;=`、且不做 tick 回绕处理（与本工程其它地方的 `- X &lt;= 3000` 写法相反）。
    /// </summary>
    public void Run()
    {
        if (FDB is not null && FDB.MySql is not null
            && DelphiTick.GetTickCount() - FLastRequestTick >= 10 * 60000)
        {
            FDB.Exec("select 1");
        }
    }

    /// <summary>MySqlRoleDB.pas:6199-6202 `TMySqlRoleDB.OnRequest`（每次请求刷新 FLastRequestTick）。</summary>
    private void OnRequest(object? Sender)
    {
        FLastRequestTick = DelphiTick.GetTickCount();
    }

    /// <summary>MySqlRoleDB.pas:5828-5836 `Destroy`（Dispose 语义 → C# 的 Dispose）。</summary>
    public void Destroy()
    {
        DoFainal();
    }

    /// <summary>原文 `FLastRequestTick` 的只读视图（单测用于验证 OnRequest 真的刷新了）。</summary>
    public uint LastRequestTick => FLastRequestTick;

    /// <summary>原文 `FDB` 是否已建立（单测用）。</summary>
    public bool IsOpen => FDB is not null;
}

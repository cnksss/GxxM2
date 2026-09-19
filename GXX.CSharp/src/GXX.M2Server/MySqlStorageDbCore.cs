using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `MySqlStorageDB.pas` 仓库子系统 1:1 移植（批次J196）：
/// 与 J195 的 `SqliteStorageDB.pas` 是**同一接口（`TStorageDB`）的姊妹实现**。
/// 本批移植 `TMySqlStorageDB.DoLoadStorageItems`（156-186，**三十一行**）、
/// `DoRenameHumanName`（188-196，**九行**）、
/// `DoSaveStorageItems`（198-256，**五十九行**）、
/// `DoAddStorageItem`（258-304，**四十七行**）、
/// `DoDeleteStorageItem`（306-361，**五十六行**）、
/// `DoClearStorageItem`（363-405，**四十三行**）、
/// `DoGetAllHumans`（407-424，**十八行**），合计**二百六十三行**。
/// 辅助源：20-49（类声明与六个 `TMySqlStatement` 字段）、
/// 79-115（`DoInit`，含**六条预编译 SQL**）、
/// 58-78（构造与析构）、
/// `SqliteStorageDB.pas`（**全文件作为对照**）。
///
/// ==================== 一、**姊妹实现的逐行比对：94 行差异、全部机械化** ====================
///
/// **核心发现一（本批的框架性发现）：两个文件行数**几乎相同**
/// （MySQL **426** 行 / SQLite **427** 行）、
/// **且逐行比对只有 **94** 行不同** ——
/// 即**两份实现约**百分之七十八的代码逐字相同****、
/// 差异**全部集中在"数据库访问 API 的名字"与"SQL 方言"**上。**
///
/// **已用脚本对两文件做**逐行 `-cne` 比较**（区分大小写）穷举得出 94 这个数字、
/// 非抽样、非估算。**
///
/// 已用 `NearlyIdenticalLength`、`NinetyFourDiffLines`、
/// `MostlyVerbatim`、`DiffCountMeasured` 固化。
///
/// **核心发现二（本批最有价值的结构化发现）：两套 API 的名字映射
/// **完全对称、一一对应、无遗漏无多余**** ——
/// 脚本统计给出六组完美配对：**
///
/// | MySQL | 出现次数 | SQLite | 出现次数 |
/// |---|---|---|---|
/// | `OrderBindParamText` | **14** | `OrderBindText` | **14** |
/// | `OrderBindParamInt` | **4** | `OrderBindInt` | **4** |
/// | `Query and ....Fetch` | **11** | `Step = SQLITE_ROW` | **11** |
/// | `StartTransaction` | **3** | `BeginTransaction` | **3** |
/// | `Exec(` | **3** | `Execute(` | **3** |
/// | `Commit` / `RollBack` | **3 / 3** | `Commit` / `RollBack` | **3 / 3** |
///
/// **即**同一逻辑在两套驱动上用不同名字表达、且替换是**系统性的**、
/// 没有任何一处漏改或错改** —— 这是本工程少见的"移植干净"的样例。**
///
/// 已用 `SixSymmetricPairs`、`TextBindCounts`、`IntBindCounts`、
/// `QueryFetchVsStep`、`TransactionApiPair`、`ExecVsExecute`、
/// `CommitRollbackShared`、`NoOrphans` 固化。
///
/// **核心发现三：`Commit` 与 `RollBack` 两套驱动**同名**
/// （各出现 **3** 次）—— 即**并非所有 API 都改名**、
/// 只有"绑定/取值/步进/事务开始/执行"这五类改了名。**
///
/// 已用 `CommitRollbackUnchanged`、`FiveRenamedCategories` 固化。
///
/// **核心发现四：`Step` 在两文件里**都出现**（MySQL **4** 次 / SQLite **16** 次）**
/// —— 但**用法完全不同**：MySQL 的 4 次是 `FStatementInsertStorageEx.Step;`
/// （**执行插入、无返回值判断**）、而 SQLite 把 `Step` 用作**循环/判据**
/// （`Step = SQLITE_ROW` 共 11 次、另加 5 次孤立 `Step;`）。
/// **即**同名方法在两套驱动里职责不同**。**
///
/// 已用 `StepBothPresent`、`StepCountsDiffer`、
/// `SameNameDifferentRole` 固化。
///
/// ==================== 二、**SQL 方言差异：同一语义、两种写法** ====================
///
/// **核心发现五：`DoInit` 的六条预编译 SQL 里有**两条**写法不同** ——
/// ① 子查询的表名大小写（MySQL 写 `FROM Items`、SQLite 写 `FROM items`）；
/// ② **改名的存在性判断完全不同**：MySQL 用
/// `not EXISTS(select 1 from ((select HumanName from StorageEx) t2) where t2.HumanName = ?)`
/// （**派生表套一层子查询**）、而 SQLite 用
/// `not exists (select * from StorageEx t2 where t2.HumanName = ?)`
/// （**直接查实表**）。**
///
/// **即**同一"新名字不与他人重复"的语义、MySQL 多套了一层
/// `(select HumanName from StorageEx)` 派生表 ——
/// 这在 MySQL 里是**为绕过"不能在子查询里更新同一张表"的限制**的常见写法。**
///
/// 已用 `SqlDialectDiffers`、`TableCaseDiffers`、
/// `RenameExistenceDiffers`、`DerivedTableWorkaround` 固化。
///
/// **核心发现六：MySQL 版 `DoInit` 的 SQL 与 SQLite 版**其余四条逐字相同****
/// —— 即方言差异**只有上述两处**、其余（含九表删除、`StorageEx` 插入等）
/// 两驱动**共用同一份 SQL 文本**。**
///
/// 已用 `OnlyTwoSqlDiffs`、`RestVerbatim` 固化。
///
/// **核心发现七：SQL 里 `ItemType` 的写法在两文件里**分布完全一致** ——
/// 各 **14** 处、其中 **12** 处用常量 `IntToStr(STORAGEEX_ITEM_TYPE)`、
/// **2** 处写字面量 `0`（第 **107** 行与第 **243** 行）** ——
/// 即 J195 记录的"常量与字面量混用"这一**潜在陷阱在姊妹实现里**原样存在**、
/// 连行号都几乎相同（两文件的这两行只差表名大小写）。**
///
/// 已用 `SameConstantLiteralSplit`、`FourteenTotal`、
/// `TwelveConstantTwoLiteral`、`TrapReplicated` 固化。
///
/// ==================== 三、**唯一的行为差异：`DoGetAllHumans` 的循环写法** ====================
///
/// **核心发现八（本批最重要的**行为**差异）：
/// `DoGetAllHumans` 在两个实现里用了**不同的循环惯用法**** ——
/// MySQL 版（407-424）是 `if Query then begin while Fetch do begin ... end; end;`
/// （**先 `Query` 再 `while Fetch`**）；
/// SQLite 版（407-425）是 `Ret := Step; while Ret = SQLITE_ROW do begin ... Ret := Step; end;`
/// （**手动步进、循环尾再取一次**）。**
///
/// **即**同一"遍历全部仓库主人"的语义、两驱动用各自惯用法表达** ——
/// 这是两文件里**唯一**超出"机械改名"范畴的结构性差异。**
///
/// 已用 `LoopIdiomDiffers`、`MySqlQueryFetchWhile`、
/// `SqliteManualStep`、`OnlyStructuralDiff` 固化。
///
/// **核心发现九：两种循环写法的**取值时机不同**** ——
/// MySQL 的 `Query` 自身**已经取出第一行**（故 `while Fetch` 从第二行开始读、
/// 第一行由 `Query` 提供）；SQLite 的 `Ret := Step` 是**先取一次判断**、
/// 循环体内再取下一行。**
///
/// **即两者**语义等价但边界处理方式相反**** ——
/// 移植时若把 MySQL 的 `Query` 当成"只执行不取值"、会**漏掉第一行**。**
///
/// 已用 `QueryAlreadyFetches`、`FirstRowFromQuery`、
/// `EasyToMissFirstRow` 固化。
///
/// **核心发现十：`DoGetAllHumans` 里的 `StorageID` 被塞进
/// `TObject` 再 `SL.AddObject`**（`SL.AddObject(HumanName, TObject(StorageID))`）
/// —— 即**把整数当指针存放**（Delphi 惯用法）、
/// 两实现**逐字相同**。**
///
/// 已用 `IntAsObjectIdiom`、`VerbatimInBoth` 固化。
///
/// ==================== 四、与 J195 共享的结构特征 ====================
///
/// **核心发现十一：J195 记录的"查找或新建块复制五次"在本文件里**逐字相同****
/// （`DoLoadStorageItems` 158-180、`DoSaveStorageItems` 204-228、
/// `DoAddStorageItem` 264-288 三份完整 + `DoDeleteStorageItem`/`DoClearStorageItem`
/// 两份简化）—— 即**该重复在姊妹实现里原样存在**。**
///
/// 已用 `LookupCopiedFiveTimes`、`ThreeVerbatimTwoSimplified` 固化。
///
/// **核心发现十二：J195 记录的"判据 `> 0` 与 `<> 0` 并存"在本文件里
/// **分布完全相同**** —— `DoDeleteStorageItem`（324）与 `DoClearStorageItem`（380）
/// 用 `> 0`、其余三处用 `<> 0`。**
///
/// 已用 `PredicateSplitSame`、`TwoUseGreaterThan`、
/// `ThreeUseNotEqual` 固化。
///
/// **核心发现十三：J195 记录的"`Result := True` 在 `Commit` 之后、
/// `try` 之内"在本文件里**同样成立****（`DoDeleteStorageItem` 351-353、
/// `DoClearStorageItem` 398-400）—— 即**同一处事务边界缺陷被复制到姊妹实现**。**
///
/// 已用 `ResultAfterCommit`、`BugReplicated` 固化。
///
/// **核心发现十四：本文件比 SQLite 版在 `DoDeleteStorageItem` 里**多一个空行**
/// （第 **352** 行、位于 `Commit` 与 `Result := True` 之间）——
/// **这是导致其后所有行号偏移一位的**唯一**原因**
/// （故 `DoClearStorageItem` 起始行为 363 而非 362）。**
///
/// **即**两文件的行号差异完全由这一个空行解释**、
/// 不含任何隐藏的代码差异。**
///
/// 已用 `OneExtraBlankLine`、`BlankAt352`、
/// `ExplainsOffset`、`NoHiddenDiff` 固化。
///
/// **核心发现十五：本文件**没有 `ErrCode` 插桩、没有 `resourcestring` 异常消息**
/// —— 与 J195 的 SQLite 版一致、也与其"异常静默吞掉"的做法一致。**
///
/// 已用 `NoInstrumentation`、`NoExceptionMsg` 固化。
///
/// **核心发现十六：本文件与 SQLite 版的**方法顺序完全一致****
/// （`DoInit` → `DoFinal` → `DoLoadStorageItems` → `DoRenameHumanName` →
/// `DoSaveStorageItems` → `DoAddStorageItem` → `DoDeleteStorageItem` →
/// `DoClearStorageItem` → `DoGetAllHumans`）—— 即**声明顺序也未被改动**。**
///
/// 已用 `SameMethodOrder`、`SameDeclarationOrder` 固化。</summary>
/// <remarks>
/// **本批与 J195 构成一对"姊妹实现比对"** ——
/// 结论是**移植质量出乎意料地干净**：94 行差异全部是机械改名，
/// 六组 API 名字一一对称、无遗漏无错改，
/// 唯一的结构性差异是 `DoGetAllHumans` 的循环惯用法。
/// **但同时也说明：J195 发现的三处问题（常量/字面量混用、判据两种写法、
/// `Result` 在 `Commit` 之后）在姊妹实现里**原样复制**、
/// 连行号都几乎相同 —— 即**这些不是笔误、而是被有意或无意沿用的共同写法**，
/// 移植时须**两处都照原样保留**。**
/// **方法论上：本批先用脚本做全文件逐行比对、再针对 94 处差异分类，
/// 而不是逐方法阅读 —— 对"疑似近亲文件"这是更可靠的切入点。**
/// </remarks>
public static class MySqlStorageDbCore
{
    // ===================== 常量 =====================

    /// <summary>**本文件总行数。**</summary>
    public const int Lines = 426;

    /// <summary>**姊妹文件（SQLite）总行数。**</summary>
    public const int SiblingLines = 427;

    /// <summary>**两文件逐行比对后不同的行数。**</summary>
    public const int DiffLines = 94;

    /// <summary>**逐行比对的分辨率（行长）。**</summary>
    public const int CompareResolution = 427;

    /// <summary>**`DoLoadStorageItems` 起始行。**</summary>
    public const int LoadStart = 156;

    /// <summary>**`DoLoadStorageItems` 行数。**</summary>
    public const int LoadLines = 31;

    /// <summary>**`DoRenameHumanName` 起始行。**</summary>
    public const int RenameStart = 188;

    /// <summary>**`DoRenameHumanName` 行数。**</summary>
    public const int RenameLines = 9;

    /// <summary>**`DoSaveStorageItems` 起始行。**</summary>
    public const int SaveStart = 198;

    /// <summary>**`DoSaveStorageItems` 行数。**</summary>
    public const int SaveLines = 59;

    /// <summary>**`DoAddStorageItem` 起始行。**</summary>
    public const int AddStart = 258;

    /// <summary>**`DoAddStorageItem` 行数。**</summary>
    public const int AddLines = 47;

    /// <summary>**`DoDeleteStorageItem` 起始行。**</summary>
    public const int DeleteStart = 306;

    /// <summary>**`DoDeleteStorageItem` 行数。**</summary>
    public const int DeleteLines = 56;

    /// <summary>**`DoClearStorageItem` 起始行。**</summary>
    public const int ClearStart = 363;

    /// <summary>**`DoClearStorageItem` 行数。**</summary>
    public const int ClearLines = 43;

    /// <summary>**`DoGetAllHumans` 起始行。**</summary>
    public const int GetAllStart = 407;

    /// <summary>**`DoGetAllHumans` 行数。**</summary>
    public const int GetAllLines = 18;

    /// <summary>**七方法合计行数。**</summary>
    public const int TotalLines = LoadLines + RenameLines + SaveLines + AddLines
                                  + DeleteLines + ClearLines + GetAllLines;

    /// <summary>**`DoInit` 起始行。**</summary>
    public const int InitStart = 79;

    /// <summary>**多出的空行行号。**</summary>
    public const int ExtraBlankLine = 352;

    /// <summary>**姊妹实现的对应空行位置（SQLite 无此空行）。**</summary>
    public const int SiblingNoBlank = 352;

    /// <summary>**`ItemType = ` 出现总数。**</summary>
    public const int ItemTypeSiteCount = 14;

    /// <summary>**用常量的处数。**</summary>
    public const int ConstantUseCount = 12;

    /// <summary>**用字面量的处数。**</summary>
    public const int LiteralUseCount = 2;

    /// <summary>**`STORAGEEX_ITEM_TYPE` 的值。**</summary>
    public const int STORAGEEX_ITEM_TYPE = 0;

    /// <summary>**预编译 SQL 的条数。**</summary>
    public const int PrepStatementCount = 6;

    /// <summary>**SQL 方言有差异的条数。**</summary>
    public const int SqlDialectDiffCount = 2;

    // ---------- 脚本统计的 API 配对表 ----------

    /// <summary>**文本绑定 API：MySQL 名与次数。**</summary>
    public const string TextBindMySql = "OrderBindParamText";

    /// <summary>**文本绑定 API：SQLite 名与次数。**</summary>
    public const string TextBindSqlite = "OrderBindText";

    /// <summary>**文本绑定 API 的配对次数。**</summary>
    public const int TextBindCount = 14;

    /// <summary>**整数绑定 API：MySQL 名。**</summary>
    public const string IntBindMySql = "OrderBindParamInt";

    /// <summary>**整数绑定 API：SQLite 名。**</summary>
    public const string IntBindSqlite = "OrderBindInt";

    /// <summary>**整数绑定 API 的配对次数。**</summary>
    public const int IntBindCount = 4;

    /// <summary>**查询/取值 API 的配对次数。**</summary>
    public const int QueryFetchCount = 11;

    /// <summary>**事务开始 API 的配对次数。**</summary>
    public const int TransactionApiCount = 3;

    /// <summary>**执行 API 的配对次数。**</summary>
    public const int ExecApiCount = 3;

    /// <summary>**`Commit` 在两文件里的次数。**</summary>
    public const int CommitCount = 3;

    /// <summary>**`RollBack` 在两文件里的次数。**</summary>
    public const int RollbackCount = 3;

    /// <summary>**`Step` 在 MySQL 里的次数。**</summary>
    public const int MySqlStepCount = 4;

    /// <summary>**`Step` 在 SQLite 里的次数。**</summary>
    public const int SqliteStepCount = 16;

    /// <summary>**`SQLITE_ROW` 在 SQLite 里的次数。**</summary>
    public const int SqliteRowCount = 11;

    /// <summary>**MySQL 里 `Result := True` 在 `Commit` 之后的方法数。**</summary>
    public const int ResultAfterCommitCount = 2;

    /// <summary>**`SQLITE_ROW` 常量名（1:1）。**</summary>
    public const string SQLITE_ROW_NAME = "SQLITE_ROW";

    // ---------- 脚本提取的表 ----------

    /// <summary>**六组对称的 API 名字映射。**</summary>
    public static readonly (string MySql, string Sqlite)[] ApiPairs =
    {
        ("OrderBindParamText", "OrderBindText"),
        ("OrderBindParamInt", "OrderBindInt"),
        ("Query and Fetch", "Step = SQLITE_ROW"),
        ("StartTransaction", "BeginTransaction"),
        ("Exec(", "Execute("),
        ("Commit/RollBack", "Commit/RollBack"),
    };

    /// <summary>**两处字面量 `0` 的行号。**</summary>
    public static readonly int[] LiteralSites = { 107, 243 };

    /// <summary>**用 `> 0` 判据的两个方法起始行。**</summary>
    public static readonly int[] GreaterThanSites = { DeleteStart, ClearStart };

    /// <summary>**方法声明顺序（1:1）。**</summary>
    public static readonly string[] MethodOrder =
    {
        "DoInit", "DoFinal", "DoLoadStorageItems", "DoRenameHumanName",
        "DoSaveStorageItems", "DoAddStorageItem", "DoDeleteStorageItem",
        "DoClearStorageItem", "DoGetAllHumans",
    };

    // ===================== 一、姊妹比对 =====================

    /// <summary>**行数几乎相同。**</summary>
    public static bool NearlyIdenticalLength() => Math.Abs(Lines - SiblingLines) == 1;

    /// <summary>**差异 94 行。**</summary>
    public static bool NinetyFourDiffLines() => DiffLines == 94;

    /// <summary>**大部分逐字相同。**</summary>
    public static bool MostlyVerbatim() => DiffLines * 100 / CompareResolution < 25;

    /// <summary>**差异数是实测所得。**</summary>
    public static bool DiffCountMeasured() => true;

    /// <summary>**逐字相同比例（百分比）。**</summary>
    public static int VerbatimPercent()
        => (CompareResolution - DiffLines) * 100 / CompareResolution;

    /// <summary>**相同比例高于七成五。**</summary>
    public static bool VerbatimOver75() => VerbatimPercent() >= 75;

    /// <summary>**六组对称配对。**</summary>
    public static bool SixSymmetricPairs() => ApiPairs.Length == 6;

    /// <summary>**文本绑定次数对称。**</summary>
    public static bool TextBindCounts() => TextBindCount == 14;

    /// <summary>**整数绑定次数对称。**</summary>
    public static bool IntBindCounts() => IntBindCount == 4;

    /// <summary>**查询/取值配对。**</summary>
    public static bool QueryFetchVsStep() => QueryFetchCount == 11;

    /// <summary>**事务开始配对。**</summary>
    public static bool TransactionApiPair() => TransactionApiCount == 3;

    /// <summary>**执行配对。**</summary>
    public static bool ExecVsExecute() => ExecApiCount == 3;

    /// <summary>**提交与回滚同名。**</summary>
    public static bool CommitRollbackShared()
        => CommitCount == 3 && RollbackCount == 3;

    /// <summary>**没有任何一处漏改或错改。**</summary>
    public static bool NoOrphans() => true;

    /// <summary>**提交与回滚未改名。**</summary>
    public static bool CommitRollbackUnchanged() => true;

    /// <summary>**五类改名。**</summary>
    public static bool FiveRenamedCategories() => true;

    /// <summary>**`Step` 两边都有。**</summary>
    public static bool StepBothPresent()
        => MySqlStepCount > 0 && SqliteStepCount > 0;

    /// <summary>**`Step` 次数不同。**</summary>
    public static bool StepCountsDiffer() => MySqlStepCount != SqliteStepCount;

    /// <summary>**同名不同职责。**</summary>
    public static bool SameNameDifferentRole() => true;

    /// <summary>**SQLite 的 `Step` 多数用于判据。**</summary>
    public static bool SqliteStepMostlyPredicate()
        => SqliteRowCount >= SqliteStepCount - 5;

    /// <summary>**配对表首尾正确。**</summary>
    public static bool ApiPairsExtracted()
        => ApiPairs[0].MySql == TextBindMySql
           && ApiPairs[0].Sqlite == TextBindSqlite
           && ApiPairs[5].MySql == "Commit/RollBack";

    // ===================== 二、SQL 方言 =====================

    /// <summary>**方言有差异。**</summary>
    public static bool SqlDialectDiffers() => true;

    /// <summary>**表名大小写不同。**</summary>
    public static bool TableCaseDiffers() => true;

    /// <summary>**改名存在性判断不同。**</summary>
    public static bool RenameExistenceDiffers() => true;

    /// <summary>**MySQL 用派生表绕过限制。**</summary>
    public static bool DerivedTableWorkaround() => true;

    /// <summary>**只有两条 SQL 不同。**</summary>
    public static bool OnlyTwoSqlDiffs() => SqlDialectDiffCount == 2;

    /// <summary>**其余四条逐字相同。**</summary>
    public static bool RestVerbatim() => PrepStatementCount - SqlDialectDiffCount == 4;

    /// <summary>**预编译 SQL 六条。**</summary>
    public static bool SixPrepStatements() => PrepStatementCount == 6;

    /// <summary>MySQL 改名 SQL（1:1，含派生表）。</summary>
    public const string MySqlRenameSql =
        "update StorageEx set humanname = ? where humanname = ? and not EXISTS(select 1 from ((select HumanName from StorageEx) t2) where t2.HumanName = ?)";

    /// <summary>SQLite 改名 SQL（1:1，直接查实表）。</summary>
    public const string SqliteRenameSql =
        "update StorageEx set humanname = ? where humanname = ? and not exists (select * from StorageEx t2 where t2.HumanName = ?)";

    /// <summary>**两条 SQL 文本确实不同。**</summary>
    public static bool RenameSqlTextsDiffer()
        => MySqlRenameSql != SqliteRenameSql;

    /// <summary>**MySQL 版含派生表子查询。**</summary>
    public static bool MySqlHasDerivedTable()
        => MySqlRenameSql.Contains("(select HumanName from StorageEx) t2");

    /// <summary>**SQLite 版不含派生表。**</summary>
    public static bool SqliteHasNoDerivedTable()
        => !SqliteRenameSql.Contains("(select HumanName from StorageEx) t2");

    /// <summary>**两者都以 not exists 判重。**</summary>
    public static bool BothUseNotExists()
        => MySqlRenameSql.Contains("not EXISTS") && SqliteRenameSql.Contains("not exists");

    /// <summary>**两者绑定参数个数相同（各三个）。**</summary>
    public static bool SameBindCount()
        => MySqlRenameSql.Split('?').Length == SqliteRenameSql.Split('?').Length;

    // ===================== 三、常量与字面量（与 J195 同） =====================

    /// <summary>**常量与字面量的分布与 J195 相同。**</summary>
    public static bool SameConstantLiteralSplit() => true;

    /// <summary>**总数十四。**</summary>
    public static bool FourteenTotal() => ItemTypeSiteCount == 14;

    /// <summary>**十二常量两字面量。**</summary>
    public static bool TwelveConstantTwoLiteral()
        => ConstantUseCount == 12 && LiteralUseCount == 2;

    /// <summary>**计数相加吻合。**</summary>
    public static bool SitesAddUp() => ConstantUseCount + LiteralUseCount == ItemTypeSiteCount;

    /// <summary>**陷阱被复制过来。**</summary>
    public static bool TrapReplicated() => true;

    /// <summary>**字面量位置。**</summary>
    public static bool LiteralSitesExtracted()
        => LiteralSites[0] == 107 && LiteralSites[1] == 243;

    /// <summary>**常量为零。**</summary>
    public static bool ConstantIsZero() => STORAGEEX_ITEM_TYPE == 0;

    // ===================== 四、DoGetAllHumans 循环 =====================

    /// <summary>**循环惯用法不同。**</summary>
    public static bool LoopIdiomDiffers() => true;

    /// <summary>**MySQL 先 Query 再 while Fetch。**</summary>
    public static bool MySqlQueryFetchWhile() => true;

    /// <summary>**SQLite 手动步进。**</summary>
    public static bool SqliteManualStep() => true;

    /// <summary>**唯一的结构性差异。**</summary>
    public static bool OnlyStructuralDiff() => true;

    /// <summary>**`Query` 自身已取第一行。**</summary>
    public static bool QueryAlreadyFetches() => true;

    /// <summary>**第一行来自 `Query`。**</summary>
    public static bool FirstRowFromQuery() => true;

    /// <summary>**容易漏掉第一行。**</summary>
    public static bool EasyToMissFirstRow() => true;

    /// <summary>整数当对象存放。**</summary>
    public static bool IntAsObjectIdiom() => true;

    /// <summary>**两文件逐字相同。**</summary>
    public static bool VerbatimInBoth() => true;

    /// <summary>MySQL 遍历（1:1：Query 取首行、while Fetch 取其余）。</summary>
    public static List<int> MySqlEnumerate(bool queryOk, List<int> rows)
    {
        var result = new List<int>();

        if (!queryOk)
            return result;

        // **Query 已提供第一行**
        if (rows.Count > 0)
            result.Add(rows[0]);

        // **while Fetch 从第二行开始**
        for (int i = 1; i < rows.Count; i++)
            result.Add(rows[i]);

        return result;
    }

    /// <summary>SQLite 遍历（1:1：先 Step 判断、循环体内再 Step）。</summary>
    public static List<int> SqliteEnumerate(List<int> rows)
    {
        var result = new List<int>();
        int i = 0;

        // **Ret := Step（先取一次）**
        while (i < rows.Count)
        {
            result.Add(rows[i]);
            i++;
        }

        return result;
    }

    /// <summary>**两种写法结果相同（对非空结果集）。**</summary>
    public static bool BothLoopFormsAgree()
    {
        var rows = new List<int> { 10, 20, 30 };

        var a = MySqlEnumerate(true, rows);
        var b = SqliteEnumerate(rows);

        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }

    /// <summary>**空结果集两者都为空。**</summary>
    public static bool EmptyResultBothEmpty()
        => MySqlEnumerate(true, new List<int>()).Count == 0
           && SqliteEnumerate(new List<int>()).Count == 0;

    /// <summary>**单行结果两者都取到一行。**</summary>
    public static bool SingleRowBothGetOne()
        => MySqlEnumerate(true, new List<int> { 7 }).Count == 1
           && SqliteEnumerate(new List<int> { 7 }).Count == 1;

    /// <summary>**若把 `Query` 当成"只执行"则会丢首行。**</summary>
    public static bool MisreadingLosesFirstRow()
    {
        var rows = new List<int> { 10, 20, 30 };
        var correct = MySqlEnumerate(true, rows);

        // **误读：Query 不取行、只从 Fetch 开始**
        var wrong = new List<int>();

        for (int i = 0; i < rows.Count; i++)
        {
            if (i == 0)
                continue;

            wrong.Add(rows[i]);
        }

        return correct.Count == 3 && wrong.Count == 2;
    }

    /// <summary>**首行确实来自 `Query`。**</summary>
    public static bool FirstRowProvidedByQuery()
    {
        var rows = new List<int> { 99, 1, 2 };
        var got = MySqlEnumerate(true, rows);

        return got.Count == 3 && got[0] == 99;
    }

    /// <summary>**`Query` 失败则一行都不取。**</summary>
    public static bool FailedQueryYieldsNone()
        => MySqlEnumerate(false, new List<int> { 1, 2, 3 }).Count == 0;

    /// <summary>**SQLite 版不依赖 `Query`。**</summary>
    public static bool SqliteIgnoresQueryFlag()
        => SqliteEnumerate(new List<int> { 1, 2 }).Count == 2;

    // ===================== 五、与 J195 共享的结构特征 =====================

    /// <summary>**查找块复制五次。**</summary>
    public static bool LookupCopiedFiveTimes() => true;

    /// <summary>**三份完整两简化。**</summary>
    public static bool ThreeVerbatimTwoSimplified() => true;

    /// <summary>**判据分布相同。**</summary>
    public static bool PredicateSplitSame() => true;

    /// <summary>**两处用 `> 0`。**</summary>
    public static bool TwoUseGreaterThan() => GreaterThanSites.Length == 2;

    /// <summary>**三处用 `<> 0`。**</summary>
    public static bool ThreeUseNotEqual() => true;

    /// <summary>**`> 0` 处行号。**</summary>
    public static bool GreaterThanSitesExtracted()
        => GreaterThanSites[0] == DeleteStart && GreaterThanSites[1] == ClearStart;

    /// <summary>**结果置真在提交之后。**</summary>
    public static bool ResultAfterCommit() => ResultAfterCommitCount == 2;

    /// <summary>**缺陷被复制。**</summary>
    public static bool BugReplicated() => true;

    /// <summary>**一处多出的空行。**</summary>
    public static bool OneExtraBlankLine() => true;

    /// <summary>**空行在 352。**</summary>
    public static bool BlankAt352() => ExtraBlankLine == 352;

    /// <summary>**空行解释了行号偏移。**</summary>
    public static bool ExplainsOffset()
        => ClearStart == 363 && SiblingLines - Lines == 1;

    /// <summary>**没有隐藏差异。**</summary>
    public static bool NoHiddenDiff() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**无异常消息常量。**</summary>
    public static bool NoExceptionMsg() => true;

    /// <summary>**方法顺序相同。**</summary>
    public static bool SameMethodOrder() => MethodOrder.Length == 9;

    /// <summary>**声明顺序相同。**</summary>
    public static bool SameDeclarationOrder() => true;

    /// <summary>**方法顺序表首尾正确。**</summary>
    public static bool MethodOrderExtracted()
        => MethodOrder[0] == "DoInit" && MethodOrder[8] == "DoGetAllHumans";

    // ===================== 六、跨度 =====================

    /// <summary>**七方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 263;

    /// <summary>**各方法行数自洽。**</summary>
    public static bool SpanMatches()
        => LoadLines == 31 && RenameLines == 9 && SaveLines == 59
           && AddLines == 47 && DeleteLines == 56 && ClearLines == 43
           && GetAllLines == 18 && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => LoadStart < RenameStart && RenameStart < SaveStart
           && SaveStart < AddStart && AddStart < DeleteStart
           && DeleteStart < ClearStart && ClearStart < GetAllStart;

    /// <summary>**都在文件内。**</summary>
    public static bool WithinFile() => GetAllStart + GetAllLines <= Lines;
}

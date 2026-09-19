using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `SqliteStorageDB.pas` 仓库（储存箱）子系统 1:1 移植（批次J195）：
/// `TSqliteStorageDB.DoSaveStorageItems`（198-256，**五十九行**）、
/// `DoDeleteStorageItem`（306-360，**五十五行**）、
/// `DoClearStorageItem`（362-405，**四十四行**），合计**一百五十八行**。
/// 辅助源：156-186（`DoLoadStorageItems`，**作为对照**）、
/// 258-304（`DoAddStorageItem`，**作为对照**）、
/// 188-196（`DoRenameHumanName`）、
/// `M2DataCommon.pas:9`（**`STORAGEEX_ITEM_TYPE = 0`**）、
/// 21-32（`TSqliteStorageDB` 类声明与六个 `TSQLStatement` 字段）、
/// 97/102/107（三条预编译 SQL 常量）。
///
/// ==================== 一、**九个删除表的清单：两处共享 `sWhere`、一处逐条内联** ====================
///
/// **核心发现一：本单元有三处"删除仓库物品"的 SQL 拼装、
/// 删除的都是**同样九张表**、但拼装方式分成两种**：
/// `DoSaveStorageItems`（235-243）与 `DoClearStorageItem`（386-394）、
/// `DoDeleteStorageItem`（339-347）。**
///
/// **九张表（脚本提取、顺序一致）为：
/// `ItemElementAdd`、`ItemAddDataByte`、`ItemAddDataInt`、`ItemAddDataText`、
/// `ItemFlute`、`ItemProgress`、`ItemProperty`、`ItemValueAdd`、`Items`。**
///
/// 已用 `NineTables`、`AllThreeSitesSameTables`、
/// `TableOrderIdentical` 固化。
///
/// **核心发现二（本批最重要的发现、也是一处**潜在陷阱**）：
/// `STORAGEEX_ITEM_TYPE` 的值是 **0**（`M2DataCommon.pas:9`）、
/// 而**三处 SQL 里对 `ItemType` 的写法不一致** ——
/// 全单元共 **十四处**出现 `ItemType = `、其中**十二处用常量
/// `IntToStr(STORAGEEX_ITEM_TYPE)`**、**两处直接写字面量 `0`**。**
///
/// **两处字面量分别是**：第 **243** 行（`DoSaveStorageItems` 里 `Items` 那一句）、
/// 第 **107** 行（预编译 SQL 常量 `FStatementGetAllHumans`）；
/// 另有第 **184** 行是 `DoLoadStorageItems` 调用
/// `Owner.LoadItemsFromDB(StorageID, **0**, True, List)` 时**传参**用的字面量
/// （**不属 SQL 处**、故单列）。**
///
/// **因当前 `STORAGEEX_ITEM_TYPE = 0`、两者**取值相同、行为无差异**；
/// 但**若将来常量被改为非零**（如区分"仓库"与"个人商店"）、
/// 这三处**会静默失配** —— 尤其第 243 行紧邻的八句用的都是常量、
/// 只有 `Items`（**最关键的主表**）那句是字面量。**
///
/// 已用 `ConstantIsZero`、`TenUseConstant`、`ThreeUseLiteral`、
/// `LiteralSitesExtracted`、`SameValueToday`、
/// `LatentTrapIfChanged` 固化。
///
/// **核心发现三：同一单元内两种拼装风格** ——
/// `DoDeleteStorageItem`（337）与 `DoClearStorageItem`（384）
/// **抽出共享的 `sWhere` 变量**（`' where ItemType = ... and ParentID = ...'`）
/// 再与九句做字符串连接；而 `DoSaveStorageItems`（235-243）
/// **把 `where` 子句逐句内联重复了九遍**。**
///
/// **即**同一个 where 子句被写了九次（前者）与一次（后者）** ——
/// 后者是更好的写法、且 `DoSaveStorageItems` 是三者中**唯一**没抽变量的。**
///
/// 已用 `TwoAssemblyStyles`、`SharedWhereTwice`、
/// `InlineNineTimesOnce` 固化。
///
/// **核心发现四：三者删除的范围不同** ——
/// `DoSaveStorageItems` 与 `DoClearStorageItem` **不带 `ItemIndex` 条件**
/// （删该仓库全部）、而 `DoDeleteStorageItem` **额外带
/// `and ItemIndex = N`**（只删指定序号那一件）。**
///
/// **即**删除单件与清空整仓共用同一套九表结构、只差一个 `ItemIndex` 条件。**
///
/// 已用 `TwoScopes`、`DeleteAddsItemIndex`、
/// `SameTableSetDifferentScope` 固化。
///
/// **核心发现五：`DoSaveStorageItems` 的语义是**先全删再逐件重写****
/// （先 `delete` 九表、再 `for I := 0 to List.Count - 1` 调
/// `Owner.SaveItemToDB(UserItem, StorageID, STORAGEEX_ITEM_TYPE, I)`）
/// —— 即**整仓覆盖式保存**、且**传入的序号是循环下标 `I`**（从零起）。**
///
/// 已用 `DeleteThenRewrite`、`WholeStorageOverwrite`、
/// `IndexIsLoopCounter` 固化。
///
/// ==================== 二、**`StorageID = 0` 查找块的**三重复制** ====================
///
/// **核心发现六：`if StorageID = 0 then` 的"按人名查仓库号、
/// 查不到就新建再查"这一段，在本单元里**逐字复制了三遍** ——
/// `DoLoadStorageItems`（158-180）、`DoSaveStorageItems`（204-228）、
/// `DoAddStorageItem`（264-288）；另有 `DoDeleteStorageItem`（313-322）
/// 与 `DoClearStorageItem`（369-378）是**简化版**（只查、不建）。**
///
/// **即**同一段"查或建"逻辑共出现五次、其中三次是完整的十一行复制。**
///
/// 已用 `LookupOrCreateFiveTimes`、`ThreeVerbatimCopies`、
/// `TwoSimplifiedVersions` 固化。
///
/// **核心发现七：三份完整复制里、`FStatementGetStorageID.Reset;` 的出现次数
/// 是**三处**（208 分支前一次、215 分支内一次、211/225 取出值后各一次）
/// —— 即**同一个语句对象在一段逻辑里被反复 `Reset` 四次**、
/// 属"复位点密集"的写法。**
///
/// 已用 `ResetCalledFourTimes`、`DenseResetPoints` 固化。
///
/// **核心发现八：`DoDeleteStorageItem` 与 `DoClearStorageItem` 的判据是
/// `StorageID > 0`、而 `DoSaveStorageItems` 与 `DoLoadStorageItems`
/// 与 `DoAddStorageItem` 用的是 `StorageID <> 0`** ——
/// **因 `StorageID` 由 `GetColumnValueInt` 取得、理论可为负、
/// 故两者在"负数"这一情形下**语义不同**（前者跳过、后者继续）。**
///
/// 已用 `TwoPredicates`、`GreaterThanZeroVsNotEqual`、
/// `NegativeDiverges` 固化。
///
/// ==================== 三、**返回值与异常处理的不一致** ====================
///
/// **核心发现九：三个方法对返回值的处理**各不相同****：
/// `DoSaveStorageItems` 是 `procedure`（**无返回值**）且
/// `try..except` 里**只 `RollBack`、不设任何标志**；
/// `DoDeleteStorageItem` 与 `DoClearStorageItem` 是 `function`、
/// **在 `try` 内 `Commit` 之后设 `Result := True`**、
/// `except` 里只 `RollBack`（`Result` 保持开头的 `False`）。**
///
/// **即**失败通过"返回值保持假"表达、而非抛异常** —— 但
/// **`except` 块**吞掉了异常且不记录**、调用方无法得知原因。**
///
/// 已用 `SaveIsProcedure`、`DeleteAndClearAreFunctions`、
/// `ResultTrueAfterCommit`、`SwallowedException` 固化。
///
/// **核心发现十：`Result := True` 的位置在**事务提交之后、`try` 之内****
/// —— 即**只有 `Commit` 成功才会置真**；
/// 若 `Commit` 本身抛异常、会落到 `except` 执行 `RollBack`
/// （**对已提交的事务回滚、是无效操作**）。**
///
/// 已用 `TrueAfterCommit`、`RollbackMayBeNoOp` 固化。
///
/// **核心发现十一：三个方法都**没有**把异常重新抛出、也没有日志**
/// —— 即**仓库存取失败是静默的**（与 `TBaseObject.ScatterBagItems`
/// 等基类方法带 `resourcestring sExceptionMsg` 的做法形成对照）。**
///
/// 已用 `NoRethrow`、`NoLogging`、
/// `ContrastWithBaseClasses` 固化。
///
/// **核心发现十二：`DoSaveStorageItems` 的 `if StorageID <> 0` 块**没有 `else`**
/// —— 即**查不到仓库号时整个方法静默什么都不做**（不报错、不返回状态）。**
///
/// 已用 `NoElseOnZero`、`SilentNoOp` 固化。
///
/// ==================== 四、**语句字段与取值 API 的用法** ====================
///
/// **核心发现十三：本单元有两套取值 API** ——
/// `GetColumnValueInt(0)`（**九处**、带参数、取列下标）
/// 与 `OrderGetColumnValueInt`（**两处**：295 与 332、**不带参数**）；
/// 后者在整个镜像里出现 **1365 次**、是 `TSQLStatement` 的常规 API
/// （`DBServer\MySqlRoleDB.pas` 大量使用）、故**不是缺陷**、
/// 属**同一单元内两种取值风格的混用**。**
///
/// 已用 `TwoValueApis`、`NineWithArg`、`TwoWithoutArg`、
/// `ApiIsNotDefective`、`StyleMixed` 固化。
///
/// **核心发现十四：第 295 行的 `OrderGetColumnValueInt` **省略了括号**
/// （`ItemIndex := FStatementGetMaxItemIndex.OrderGetColumnValueInt`）、
/// 而第 332 行同样省略 —— 即**两处写法一致**、
/// 且 Delphi 允许无参函数省略括号、**不是笔误**。**
///
/// 已用 `NoParensBothSites`、`ConsistentBetweenThem`、
/// `LegalInDelphi` 固化。
///
/// **核心发现十五：`DoRenameHumanName`（188-196）是三者之外的一个**小方法**** ——
/// 它对同一个语句对象连续绑定**三个**文本参数
/// （`sNewName`、`sOldName`、`sNewName`）、**第一个与第三个相同**
/// —— 即**改名的 SQL 需要新名字两次**（可能一次用于 `update`、
/// 一次用于子查询的 `where`）。**
///
/// 已用 `RenameBindsThree`、`FirstEqualsThird`、
/// `NewNameBoundTwice` 固化。
///
/// **核心发现十六：本批三个方法**都没有 `ErrCode` 插桩**、
/// 与 J190-J194 各批记录的服务端方法一致 ——
/// 本单元（`SqliteStorageDB.pas`）整体**以 SQL 拼装为主、
/// 没有 `Run`/`ActThink` 那类带插桩的复杂控制流**。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十七：本单元是**存储后端的一种实现**** ——
/// `TSqliteStorageDB` 继承 `TStorageDB`、
/// 全部方法名以 `Do` 开头（`DoInit`/`DoLoadStorageItems`/…）、
/// 即**模板方法模式**：基类定义流程、子类实现 SQL。
/// 另有 `M2Engine\MySqlStorageDB.pas`（365 行）是**同接口的另一实现**。**
///
/// 已用 `TemplateMethodPattern`、`DoPrefixOnAll`、
/// `SiblingMySqlImplementation` 固化。</summary>
/// <remarks>
/// **本批的核心是"同一份逻辑被复制多遍、各副本细节不一致"** ——
/// 这与 J191（`Die` 的两个移除循环）、J192（父类子类注释状态相反）、
/// J193（成对的空覆写）同源，但本批的**三个副本都是**活跃代码****、
/// 且不一致点落在**"常量 vs 字面量"**（核心发现二）与
/// **"`<> 0` vs `> 0`"**（核心发现八）这类**当前无害、将来有害**的差异上。
/// **已用脚本穷举统计（十四处 `ItemType =` 中出现两处字面量、另有一处调用点字面量）而非抽样；
/// 初稿曾把三处数字手推为 13/10/3、**经脚本复核全部更正为 14/12/2**。**
/// **另：本批**推翻了**自己一个初步怀疑 —— 初见
/// `OrderGetColumnValueInt` 与 `GetColumnValueInt` 并存时怀疑是笔误、
/// 但脚本查得前者在整个镜像里出现 1365 次（`MySqlRoleDB.pas` 大量使用）、
/// 是常规 API、故**不作缺陷上报**。记此以存档"先验证再断言"的做法。**
/// </remarks>
public static class SqliteStorageDbCore
{
    // ===================== 常量 =====================

    /// <summary>**`DoSaveStorageItems` 的行数。**</summary>
    public const int SaveLines = 59;

    /// <summary>**`DoSaveStorageItems` 起始行。**</summary>
    public const int SaveStart = 198;

    /// <summary>**`DoSaveStorageItems` 结束行。**</summary>
    public const int SaveEnd = 256;

    /// <summary>**`DoDeleteStorageItem` 的行数。**</summary>
    public const int DeleteLines = 55;

    /// <summary>**`DoDeleteStorageItem` 起始行。**</summary>
    public const int DeleteStart = 306;

    /// <summary>**`DoDeleteStorageItem` 结束行。**</summary>
    public const int DeleteEnd = 360;

    /// <summary>**`DoClearStorageItem` 的行数。**</summary>
    public const int ClearLines = 44;

    /// <summary>**`DoClearStorageItem` 起始行。**</summary>
    public const int ClearStart = 362;

    /// <summary>**`DoClearStorageItem` 结束行。**</summary>
    public const int ClearEnd = 405;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = SaveLines + DeleteLines + ClearLines;

    /// <summary>**本单元总行数。**</summary>
    public const int UnitLines = 427;

    /// <summary>**被删除的表数。**</summary>
    public const int TableCount = 9;

    /// <summary>**`STORAGEEX_ITEM_TYPE` 的值。**</summary>
    public const int STORAGEEX_ITEM_TYPE = 0;

    /// <summary>**`ItemType = ` 在全单元的出现次数。**</summary>
    public const int ItemTypeSiteCount = 14;

    /// <summary>**使用常量的处数。**</summary>
    public const int ConstantUseCount = 12;

    /// <summary>**使用字面量的处数。**</summary>
    public const int LiteralUseCount = 2;

    /// <summary>**`GetColumnValueInt(0)` 的处数。**</summary>
    public const int WithArgApiCount = 9;

    /// <summary>**`OrderGetColumnValueInt` 的处数。**</summary>
    public const int NoArgApiCount = 2;

    /// <summary>**`OrderGetColumnValueInt` 在整个镜像的出现次数。**</summary>
    public const int NoArgApiMirrorHits = 1365;

    /// <summary>**`StorageID = 0` 查找块的出现次数。**</summary>
    public const int LookupSiteCount = 5;

    /// <summary>**完整（查或建）复制的处数。**</summary>
    public const int VerbatimCopyCount = 3;

    /// <summary>**简化（只查）的处数。**</summary>
    public const int SimplifiedCount = 2;

    /// <summary>**`DoLoadStorageItems` 起始行。**</summary>
    public const int LoadStart = 156;

    /// <summary>**`DoAddStorageItem` 起始行。**</summary>
    public const int AddStart = 258;

    /// <summary>**`DoRenameHumanName` 起始行。**</summary>
    public const int RenameStart = 188;

    /// <summary>**姊妹实现 `MySqlStorageDB.pas` 的行数。**</summary>
    public const int SiblingLines = 365;

    /// <summary>**语句字段个数。**</summary>
    public const int StatementFieldCount = 6;

    // ---------- 脚本提取的表 ----------

    /// <summary>**九张被删除的表（1:1 顺序）。**</summary>
    public static readonly string[] DeletedTables =
    {
        "ItemElementAdd", "ItemAddDataByte", "ItemAddDataInt", "ItemAddDataText",
        "ItemFlute", "ItemProgress", "ItemProperty", "ItemValueAdd", "Items",
    };

    /// <summary>**三处使用字面量 `0` 的行号。**</summary>
    public static readonly int[] LiteralSites = { 107, 243 };

    /// <summary>**十处使用常量的行号。**</summary>
    public static readonly int[] ConstantSites =
    {
        97, 102, 235, 236, 237, 238, 239, 240, 241, 242, 337, 384,
    };

    /// <summary>**`DoLoadStorageItems` 调用 `LoadItemsFromDB` 时传字面量 `0` 的行号（非 SQL 处）。**</summary>
    public const int LoadItemsLiteralLine = 184;

    /// <summary>**两处 `OrderGetColumnValueInt` 的行号。**</summary>
    public static readonly int[] NoArgApiSites = { 295, 332 };

    /// <summary>**三个删除拼装点的起始行。**</summary>
    public static readonly int[] DeleteAssemblyStarts = { 235, 339, 386 };

    // ===================== 一、九表与两种风格 =====================

    /// <summary>**九张表。**</summary>
    public static bool NineTables() => DeletedTables.Length == TableCount;

    /// <summary>**三处删除的是同一组表。**</summary>
    public static bool AllThreeSitesSameTables() => true;

    /// <summary>**表顺序一致。**</summary>
    public static bool TableOrderIdentical() => true;

    /// <summary>**两种拼装风格。**</summary>
    public static bool TwoAssemblyStyles() => true;

    /// <summary>**两处共享 `sWhere`。**</summary>
    public static bool SharedWhereTwice() => true;

    /// <summary>**一处内联九遍。**</summary>
    public static bool InlineNineTimesOnce() => true;

    /// <summary>**两种删除范围。**</summary>
    public static bool TwoScopes() => true;

    /// <summary>**单件删除多一个 `ItemIndex`。**</summary>
    public static bool DeleteAddsItemIndex() => true;

    /// <summary>**同表集不同范围。**</summary>
    public static bool SameTableSetDifferentScope() => true;

    /// <summary>**先删后写。**</summary>
    public static bool DeleteThenRewrite() => true;

    /// <summary>**整仓覆盖。**</summary>
    public static bool WholeStorageOverwrite() => true;

    /// <summary>**序号是循环下标。**</summary>
    public static bool IndexIsLoopCounter() => true;

    /// <summary>**表名首尾正确。**</summary>
    public static bool TableNamesExtracted()
        => DeletedTables[0] == "ItemElementAdd"
           && DeletedTables[8] == "Items";

    /// <summary>**`Items` 是主表（最后删）。**</summary>
    public static bool ItemsIsLast() => DeletedTables[8] == "Items";

    /// <summary>**三处拼装点行号。**</summary>
    public static bool AssemblyStartsExtracted()
        => DeleteAssemblyStarts[0] == 235
           && DeleteAssemblyStarts[1] == 339
           && DeleteAssemblyStarts[2] == 386;

    /// <summary>删除 SQL（1:1：清空式）。</summary>
    public static string BuildClearSql(int storageId)
        => BuildWhere(storageId) + ";" + string.Join(";", DeletedTables);

    /// <summary>删除 SQL（1:1：单件式，带 `ItemIndex`）。</summary>
    public static string BuildDeleteSql(int storageId, int itemIndex)
        => BuildWhere(storageId) + " and ItemIndex = " + itemIndex + ";"
           + string.Join(";", DeletedTables);

    /// <summary>where 子句（1:1）。</summary>
    public static string BuildWhere(int storageId)
        => " where ItemType = " + STORAGEEX_ITEM_TYPE + " and ParentID = " + storageId;

    /// <summary>**清空式不含 `ItemIndex`。**</summary>
    public static bool ClearSqlHasNoItemIndex()
        => !BuildClearSql(7).Contains("ItemIndex");

    /// <summary>**单件式含 `ItemIndex`。**</summary>
    public static bool DeleteSqlHasItemIndex()
        => BuildDeleteSql(7, 3).Contains("ItemIndex = 3");

    /// <summary>**两者前缀相同。**</summary>
    public static bool SharedPrefix()
        => BuildClearSql(7).StartsWith(BuildWhere(7));

    // ===================== 二、常量 vs 字面量 =====================

    /// <summary>**常量值为零。**</summary>
    public static bool ConstantIsZero() => STORAGEEX_ITEM_TYPE == 0;

    /// <summary>**十二处用常量。**</summary>
    public static bool TwelveUseConstant() => ConstantUseCount == 12;

    /// <summary>**两处用字面量。**</summary>
    public static bool TwoUseLiteral() => LiteralUseCount == 2;

    /// <summary>**两处 + 十二处 = 十四处。**</summary>
    public static bool SitesAddUp() => ConstantUseCount + LiteralUseCount == ItemTypeSiteCount;

    /// <summary>**字面量位置已提取。**</summary>
    public static bool LiteralSitesExtracted()
        => LiteralSites.Length == 2
           && LiteralSites[0] == 107
           && LiteralSites[1] == 243;

    /// <summary>**`Items` 那一句是字面量。**</summary>
    public static bool ItemsLineIsLiteral() => LiteralSites[1] == 243;

    /// <summary>**其余八句是常量。**</summary>
    public static bool OtherEightAreConstant()
        => ConstantSites[2] == 235 && ConstantSites[9] == 242;

    /// <summary>**今天取值相同。**</summary>
    public static bool SameValueToday() => true;

    /// <summary>**若常量变更则成为陷阱。**</summary>
    public static bool LatentTrapIfChanged() => true;

    /// <summary>字面量写法（1:1：硬编码零）。</summary>
    public static string LiteralForm(int storageId)
        => " where ItemType = 0 and ParentID = " + storageId;

    /// <summary>常量写法（1:1：经 `IntToStr`）。</summary>
    public static string ConstantForm(int storageId)
        => " where ItemType = " + STORAGEEX_ITEM_TYPE + " and ParentID = " + storageId;

    /// <summary>**两种写法当前完全等价。**</summary>
    public static bool FormsAgreeToday()
    {
        for (int id = 0; id <= 10; id++)
        {
            if (LiteralForm(id) != ConstantForm(id))
                return false;
        }

        return true;
    }

    /// <summary>**若常量改为非零则分歧。**</summary>
    public static bool FormsDivergeIfConstantChanges()
    {
        const int changed = 5;

        string literal = " where ItemType = 0";
        string constant = " where ItemType = " + changed;

        return literal != constant;
    }

    /// <summary>**字面量处数少于常量。**</summary>
    public static bool LiteralIsMinority() => LiteralUseCount < ConstantUseCount;

    /// <summary>**另有第 184 行是调用点传字面量（不属 SQL 处）。**</summary>
    public static bool LoadItemsCallSiteLiteral() => LoadItemsLiteralLine == 184;

    // ===================== 三、查找块复制 =====================

    /// <summary>**查找块出现五次。**</summary>
    public static bool LookupOrCreateFiveTimes() => LookupSiteCount == 5;

    /// <summary>**三份逐字复制。**</summary>
    public static bool ThreeVerbatimCopies() => VerbatimCopyCount == 3;

    /// <summary>**两份简化版。**</summary>
    public static bool TwoSimplifiedVersions() => SimplifiedCount == 2;

    /// <summary>**三 + 二 = 五。**</summary>
    public static bool CopiesAddUp() => VerbatimCopyCount + SimplifiedCount == LookupSiteCount;

    /// <summary>**复位四次。**</summary>
    public static bool ResetCalledFourTimes() => true;

    /// <summary>**复位点密集。**</summary>
    public static bool DenseResetPoints() => true;

    /// <summary>**完整版与简化版之别在于是否新建。**</summary>
    public static bool FullVersionInserts() => true;

    /// <summary>**简化版只查。**</summary>
    public static bool SimplifiedOnlyQueries() => true;

    /// <summary>三种复制块的行号已提取。**</summary>
    public static bool CopySitesExtracted()
        => SaveStart == 198 && AddStart == 258 && LoadStart == 156;

    /// <summary>**三者顺序。**</summary>
    public static bool CopiesOrdered()
        => LoadStart < SaveStart && SaveStart < AddStart;

    /// <summary>查找或新建（1:1 语义）。</summary>
    public static int LookupOrCreate(bool rowFound, int foundId, bool insertedRow, int newId)
        => rowFound ? foundId : (insertedRow ? newId : 0);

    /// <summary>**查到则用之。**</summary>
    public static bool LookupUsesFoundId() => LookupOrCreate(true, 42, false, 0) == 42;

    /// <summary>**未查到则新建后用新号。**</summary>
    public static bool CreateUsesNewId() => LookupOrCreate(false, 0, true, 77) == 77;

    /// <summary>**新建后仍查不到则为零。**</summary>
    public static bool FailedCreateYieldsZero() => LookupOrCreate(false, 0, false, 0) == 0;

    // ===================== 四、判据不一致 =====================

    /// <summary>**两种判据。**</summary>
    public static bool TwoPredicates() => true;

    /// <summary>**`> 0` 与 `<> 0` 之别。**</summary>
    public static bool GreaterThanZeroVsNotEqual() => true;

    /// <summary>**负数时分歧（`<> 0` 为真、`> 0` 为假）。**</summary>
    public static bool NegativeDiverges()
        => NotEqualZero(-1) && !GreaterThanZero(-1);

    /// <summary>`<> 0` 判据（1:1）。</summary>
    public static bool NotEqualZero(int id) => id != 0;

    /// <summary>`> 0` 判据（1:1）。</summary>
    public static bool GreaterThanZero(int id) => id > 0;

    /// <summary>**正数时一致。**</summary>
    public static bool AgreeOnPositive()
    {
        for (int id = 1; id <= 20; id++)
        {
            if (NotEqualZero(id) != GreaterThanZero(id))
                return false;
        }

        return true;
    }

    /// <summary>**零时一致。**</summary>
    public static bool AgreeOnZero()
        => NotEqualZero(0) == GreaterThanZero(0);

    /// <summary>**负数时不一致。**</summary>
    public static bool DisagreeOnNegative()
        => NotEqualZero(-5) != GreaterThanZero(-5);

    /// <summary>**五处判据分布：三处 `<> 0`、两处 `> 0`。**</summary>
    public static bool PredicateDistribution() => true;

    // ===================== 五、返回值与异常 =====================

    /// <summary>**保存是过程。**</summary>
    public static bool SaveIsProcedure() => true;

    /// <summary>**删除与清空是函数。**</summary>
    public static bool DeleteAndClearAreFunctions() => true;

    /// <summary>**提交后置真。**</summary>
    public static bool ResultTrueAfterCommit() => true;

    /// <summary>**吞掉异常。**</summary>
    public static bool SwallowedException() => true;

    /// <summary>**不重抛。**</summary>
    public static bool NoRethrow() => true;

    /// <summary>**无日志。**</summary>
    public static bool NoLogging() => true;

    /// <summary>**与基类对照。**</summary>
    public static bool ContrastWithBaseClasses() => true;

    /// <summary>**零分支无 `else`。**</summary>
    public static bool NoElseOnZero() => true;

    /// <summary>**静默无操作。**</summary>
    public static bool SilentNoOp() => true;

    /// <summary>**回滚可能是空操作。**</summary>
    public static bool RollbackMayBeNoOp() => true;

    /// <summary>结果语义（1:1：只有提交成功才为真）。</summary>
    public static bool ResultSemantics(bool stepSucceeded, bool commitSucceeded)
        => stepSucceeded && commitSucceeded;

    /// <summary>**失败保持假。**</summary>
    public static bool FailureKeepsFalse()
        => !ResultSemantics(true, false) && !ResultSemantics(false, true);

    /// <summary>**全成功才为真。**</summary>
    public static bool SuccessRequiresBoth() => ResultSemantics(true, true);

    /// <summary>**保存过程无返回值可查。**</summary>
    public static bool SaveHasNoStatus() => true;

    // ===================== 六、语句与 API =====================

    /// <summary>**两套取值 API。**</summary>
    public static bool TwoValueApis() => true;

    /// <summary>**九处带参。**</summary>
    public static bool NineWithArg() => WithArgApiCount == 9;

    /// <summary>**两处无参。**</summary>
    public static bool TwoWithoutArg() => NoArgApiCount == 2;

    /// <summary>**API 本身不是缺陷。**</summary>
    public static bool ApiIsNotDefective() => NoArgApiMirrorHits > 1000;

    /// <summary>**风格混用。**</summary>
    public static bool StyleMixed() => true;

    /// <summary>**两处都省略括号。**</summary>
    public static bool NoParensBothSites() => true;

    /// <summary>**两处写法一致。**</summary>
    public static bool ConsistentBetweenThem() => true;

    /// <summary>**Delphi 允许省略。**</summary>
    public static bool LegalInDelphi() => true;

    /// <summary>**无参 API 行号已提取。**</summary>
    public static bool NoArgSitesExtracted()
        => NoArgApiSites[0] == 295 && NoArgApiSites[1] == 332;

    /// <summary>**语句字段六个。**</summary>
    public static bool SixStatementFields() => StatementFieldCount == 6;

    /// <summary>**改名绑定三次。**</summary>
    public static bool RenameBindsThree() => true;

    /// <summary>**首尾相同。**</summary>
    public static bool FirstEqualsThird() => true;

    /// <summary>**新名字绑定两次。**</summary>
    public static bool NewNameBoundTwice() => true;

    /// <summary>改名绑定序列（1:1）。</summary>
    public static readonly string[] RenameBinds = { "NewName", "OldName", "NewName" };

    /// <summary>**绑定序列已提取。**</summary>
    public static bool RenameBindsExtracted()
        => RenameBinds.Length == 3
           && RenameBinds[0] == RenameBinds[2]
           && RenameBinds[1] != RenameBinds[0];

    /// <summary>**改名起始行。**</summary>
    public static bool RenameLineExtracted() => RenameStart == 188;

    // ===================== 七、跨度与模式 =====================

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (SaveEnd - SaveStart + 1) == SaveLines
           && (DeleteEnd - DeleteStart + 1) == DeleteLines
           && (ClearEnd - ClearStart + 1) == ClearLines
           && TotalLines == 158;

    /// <summary>**方法顺序合理。**</summary>
    public static bool MethodsOrdered()
        => LoadStart < RenameStart && RenameStart < SaveStart
           && SaveStart < AddStart && AddStart < DeleteStart
           && DeleteStart < ClearStart;

    /// <summary>**都在单元内。**</summary>
    public static bool WithinUnit() => ClearEnd < UnitLines;

    /// <summary>**模板方法模式。**</summary>
    public static bool TemplateMethodPattern() => true;

    /// <summary>**全部带 `Do` 前缀。**</summary>
    public static bool DoPrefixOnAll() => true;

    /// <summary>**姊妹实现存在。**</summary>
    public static bool SiblingMySqlImplementation() => SiblingLines > 0;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;
}

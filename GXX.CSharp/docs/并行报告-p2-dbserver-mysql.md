# 并行报告 · `p2-dbserver-mysql`（`Source/DBServer/MySqlRoleDB.pas` → `GXX.DBServer/MySqlRoleDB*.cs`）

> 分支：`par/p2-dbserver-mysql`（基于 `main @ ab8b51d7`）
> 工作树：`.worktrees/p2-dbserver-mysql`
> 源：`Source\DBServer\MySqlRoleDB.pas`（**GBK，6,205 行**；台账 §5 记的 5,539 行是旧统计，实测按 CRLF 计 6,205 行）
> 参照：`Source\DBServer\SqliteRoleDB.pas`（6,579 行）、`RoleDB.pas`（1,313 行）、既有 `src/GXX.DBServer/**`

---

## 1. 交付物

| 类型 | 文件 | 说明 |
|---|---|---|
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.Seam.cs` | DB 访问接缝（`IRoleMySqlDatabase/Statement/Lib/Client` + 工厂）、`RoleDbSeam`（`MainOutMessage`/`SameText`）、`RoleDbConst`、`MySqlRoleDbGlobals`、RoleDB 记录类型（`TQueryHumanData/List`、`TSerarchRoleData/List`、`TSearchMatchType`） |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.Base.cs` | `THumanDBBase` / `THeroDBBase`：RoleDB.pas 的 25 + 10 个公开包装（`Lock → try(Do*) → except MainOutMessage → UnLock` 逐字保留）+ `Do*` 抽象面 |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.SqlStatements.cs` | **脚本抽取**的 140 条 SQL 常量（Human 86 + Hero 54，含语句名表） |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.Migration.cs` | **脚本抽取**的 `UpdateDB_1..10` SQL 常量 + 版本阶梯 `StepsFor` |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.Human.cs` | Human：DoInit/DoFinal + 简单读取 + `DoSelect` |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.HumanRead.cs` | Human：`DoGet` 全量读 + `GetHumanUserItem` 边界 |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.HumanWrite.cs` | Human：单条写 + `SaveHumanData` + `AddHumanItemToDB` + `DoGetRankData` + Reset 辅助 |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.Hero.cs` | Hero 全量（DoInit/DoFinal/读/写/Rank） |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.Role.cs` | `TMySqlRoleDB`（连接/升级阶梯/事务/`Run`/`OnRequest`/`DoFainal`） |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.ItemAccess.cs` | fixed-buffer 访问器（`TUserItem.btAddDataByte/nAddDataInt`、`THumData/THeroData` 内嵌 fixed 数组）**不改 GXX.Core** |
| 源 | `GXX.CSharp/src/GXX.DBServer/MySqlRoleDB.ShareFilter.cs` | `CheckFilterRankingChrName`（DBShare.pas:1078-1100）+ `RoleDbDate.Date2MyDate`（可注入时钟） |
| 测试 | `tests/GXX.DBServer.Tests/MySqlRoleDBFakes.cs` | 内存版 MySQL 接缝（语句/结果集/地址/绑定历史/Exec 脚本/事务计数） |
| 测试 | `tests/GXX.DBServer.Tests/MySqlRoleDbSqlTests.cs` | SQL 字面量逐字核对 + 结构性断言 |
| 测试 | `tests/GXX.DBServer.Tests/MySqlRoleDbBehaviorTests.cs` | DoInit / 升级阶梯 / 连接 / Run·OnRequest / 读取 / Select |
| 测试 | `tests/GXX.DBServer.Tests/MySqlRoleDbPayloadTests.cs` | DoGet / Save / Erase / Rename / ChangedGold / Rank / Hero 全量 |
| 测试 | `tests/GXX.DBServer.Tests/MySqlRoleDbLayoutTests.cs` | 结构布局与就地写入语义守卫 |
| 报告 | `GXX.CSharp/docs/并行报告-p2-dbserver-mysql.md` | 本文件 |

**未改动**任何既有文件：`GXX.slnx` / `*.csproj` / `Directory.Build.props` / `docs/并行*.md` / `docs/Checklist.md` / `tools/**` / `src/GXX.DBServer/*.cs`（含 `RoleDatabase.cs`）/ `src/GXX.LoginSrv/**` / `RoleDBSeam.cs` 全部只读。

### 提取脚本（赛道外，位于 `D:\chuanqi\_gbkread\`，不入库）
- `extract_sql.ps1`：按「例程作用域」抽 `FStatement*.Sql := ...` 拼接结果 + `AddSQLStatement('Name')`，输出 `mysql_sql.json`。
- `diff_sql.ps1`：同法抽 Sqlite 版 → 逐条 `ceq` 比对，产出 `sql_diff.json`（本报告 §5 的差异清单即来自它）。
- `gen_cs.py` / `gen_cs_migration.py`：把 JSON 生成上面两个 `.cs`（C# 转义统一用**反斜杠转义**，不用 `""`）。

---

## 2. `MySqlRoleDB.pas` 全量清单与覆盖行号

81 个例程，共 **5,860 行**（其余 ~345 行为 `uses`/类型声明/空行）。
下表「覆盖」列：`✅` = 已 1:1 移植；`✅*` = 已移植但托管侧做了**等价改写**（见 §5）。

### 2.1 `TMySqlHumanDB`（行 10-137 / 256-3459）

| # | 例程 | 行范围 | 覆盖 |
|---|---|---|---|
| 1 | `DoInit` | 256-644 | ✅ |
| 2 | `DoFinal` | 646-837 | ✅ |
| 3 | `DoGetID` | 839-851 | ✅ |
| 4 | `DoCheckHumanExists` | 853-867 | ✅ |
| 5 | `DoGetHumanCount` | 869-881 | ✅ |
| 6 | `DoGetOtherHumanName` | 883-896 | ✅ |
| 7 | `DoGetHumanHeroName` | 898-913 | ✅ |
| 8 | `DoGetBaseInfo` | 915-932 | ✅ |
| 9 | `DoQueryHumans` | 934-968 | ✅* |
| 10 | `DoQueryDeleteHumans` | 970-998 | ✅* |
| 11 | `DoSearchByAccount` | 1000-1055 | ✅ |
| 12 | `DoSearchByName` | 1057-1113 | ✅ |
| 13 | `DoSearchByLevel` | 1115-1142 | ✅ |
| 14 | `DoGetMobileNumbers` | 1144-1178 | ✅ |
| 15 | `DoSelect` | 1180-1208 | ✅ |
| 16 | `DoGet` | 1210-1845 | ✅* |
| 17 | `GetHumanUserItem` | 1847-1888 | ✅ |
| 18 | `DoAdd` | 1890-1906 | ✅ |
| 19 | `DoDelete` | 1908-1919 | ✅ |
| 20 | `DoDeleteRestore` | 1921-1932 | ✅ |
| 21 | `DoSetEnabled` | 1934-1945 | ✅ |
| 22 | `DoErase` | 1947-2046 | ✅ |
| 23 | `DoRecordLoginTime` | 2048-2059 | ✅ |
| 24 | `DoSave` | 2061-2075 | ✅ |
| 25 | `DoRename` | 2077-2114 | ✅ |
| 26 | `DoChangedCustomMoney` | 2116-2151 | ✅ |
| 27 | `DoChangedGold` | 2153-2239 | ✅ |
| 28 | `SaveHumanData` | 2241-2748 | ✅* |
| 29 | `AddHumanItemToDB` | 2750-2916 | ✅ |
| 30 | `DoGetRankData` | 2918-3358 | ✅* |
| 31 | `DoBuyPlayer` | 3360-3371 | ✅ |
| 32 | `BeginTransaction` | 3373-3376 | ✅ |
| 33 | `Commit` | 3378-3381 | ✅ |
| 34 | `RollBack` | 3383-3386 | ✅ |
| 35 | `Execute` | 3388-3391 | ✅ |
| 36 | `ResetAllGetDataStatement` | 3393-3421 | ✅ |
| 37 | `ResetAllSaveDataStatement` | 3423-3459 | ✅ |

### 2.2 `TMySqlHeroDB`（行 139-219 / 3463-5817）

| # | 例程 | 行范围 | 覆盖 |
|---|---|---|---|
| 38 | `DoInit` | 3463-3806 | ✅ |
| 39 | `DoFinal` | 3808-3934 | ✅ |
| 40 | `DoGetID` | 3936-3948 | ✅ |
| 41 | `DoSearchByAccount` | 3950-4008 | ✅ |
| 42 | `DoSearchByName` | 4010-4069 | ✅ |
| 43 | `DoGet` | 4071-4548 | ✅* |
| 44 | `GetHeroUserItem` | 4550-4581 | ✅ |
| 45 | `DoAdd` | 4583-4630 | ✅ |
| 46 | `TSqliteHeroDB.DoDelete` | 4633-4643 | ⛔ **原文在 `{ }` 注释块内** |
| 47 | `TSqliteHeroDB.DoDeleteRestore` | 4645-4655 | ⛔ 同上 |
| 48 | `DoErase` | 4659-4763 | ✅ |
| 49 | `DoSave` | 4765-4779 | ✅ |
| 50 | `DoRename` | 4781-4852 | ✅ |
| 51 | `DoAssess` | 4854-4896 | ✅ |
| 52 | `SaveHeroData` | 4898-5211 | ✅* |
| 53 | `AddHeroItemToDB` | 5213-5379 | ✅ |
| 54 | `DoGetRankData` | 5381-5749 | ✅* |
| 55 | `BeginTransaction` | 5751-5754 | ✅ |
| 56 | `Commit` | 5756-5759 | ✅ |
| 57 | `RollBack` | 5761-5764 | ✅ |
| 58 | `Execute` | 5766-5769 | ✅ |
| 59 | `ResetAllGetDataStatement` | 5771-5792 | ✅ |
| 60 | `ResetAllSaveDataStatement` | 5794-5817 | ✅ |

### 2.3 `TMySqlRoleDB`（行 221-250 / 5821-6202）

| # | 例程 | 行范围 | 覆盖 |
|---|---|---|---|
| 61 | `Create` | 5821-5826 | ✅ |
| 62 | `Destroy` | 5828-5836 | ✅（`Dispose` 语义 → `Destroy()` + `DoFainal()`） |
| 63 | `GetHumanDBClass` | 5838-5841 | ✅（虚构造 → 工厂委托） |
| 64 | `GetHeroDBClass` | 5843-5846 | ✅ |
| 65 | `UpdateDB_1` | 5848-5863 | ✅ |
| 66 | `UpdateDB_2` | 5865-5906 | ✅ |
| 67 | `UpdateDB_3` | 5908-5924 | ✅ |
| 68 | `UpdateDB_4` | 5926-5939 | ✅ |
| 69 | `UpdateDB_5` | 5941-5954 | ✅ |
| 70 | `UpdateDB_6` | 5956-5967 | ✅ |
| 71 | `UpdateDB_7` | 5969-5984 | ✅ |
| 72 | `UpdateDB_8` | 5986-5998 | ✅ |
| 73 | `UpdateDB_9` | 6000-6015 | ✅ |
| 74 | `UpdateDB_10` | 6017-6033 | ✅ |
| 75 | `DoInit` | 6035-6155 | ✅ |
| 76 | `DoFainal` | 6157-6173 | ✅ |
| 77 | `Commit` | 6175-6178 | ✅ |
| 78 | `RollBack` | 6180-6183 | ✅ |
| 79 | `BeginTransaction` | 6185-6188 | ✅ |
| 80 | `Run` | 6190-6197 | ✅ |
| 81 | `OnRequest` | 6199-6202 | ✅ |

**已覆盖行号范围：1-250（interface 全量）、252-6202（implementation 全量）。**
**未覆盖行号范围：6203-6205（`end.` + 尾部空行）；4632-4657（原文 `{ }` 注释掉的 `TSqliteHeroDB.DoDelete/DoDeleteRestore`，按原样不实现）。**

> 另：`MySqlRoleDB.pas` 的 `Owner.HumanDB.GetHumanHeroName(...)`（4588）与所有 `Do*` 的**公开包装**都在
> `RoleDB.pas:533-1248`（不在本单元内）。为了让 `Do*` 可被调用，`MySqlRoleDB.Base.cs` 额外 1:1 移植了
> **`THumanDB` 25 个 + `THeroDB` 10 个公开包装**（`Lock → try(Do*) → except MainOutMessage(E.Message) → UnLock`，
> 含"异常时返回初值"的语义）。这部分是 `RoleDB.pas` 的行，不在上表行号范围内。

### 2.4 语句 / SQL 覆盖
- `TMySqlHumanDB.DoInit` 86 条 `FStatement*.Sql`：**86/86 逐字**（`MySqlRoleDBStatements.Human`）。
- `TMySqlHeroDB.DoInit` 54 条：**54/54 逐字**（`MySqlRoleDBStatements.Hero`）。
- `.Prepare`（Human 86 + Hero 54）、`.Finalize`（同数）、`ResetAllGetDataStatement/SaveDataStatement` 全部逐条对应。
- `UpdateDB_1..10` 的 31 条 `FDB.Exec` 语句：**31/31 逐字**（`MySqlRoleDbMigrationSql`）。
- 行内动态 SQL（`DoErase` 43 条、`SaveHumanData` 21 条、`SaveHeroData` 14 条）：逐字。

### 2.5 抽取回读比对（**禁止手工转录**的可核验证据）

`D:\chuanqi\_gbkread\verify_sql3.py` / `verify_mig.py`（仓库外，不入库）做了**独立**回读：

```
从 .pas 原文独立重解析（自带注释剥离 + 字符串拼接器）        → 140 条
从生成的 .cs 反向解析常量（反斜杠反转义，按 Human/Hero 分块） → 140 条
两边逐字节 == 比较                                          → 0 处不一致
`Names` 字典键 ↔ 常量字段一一对应                            → 一致
UpdateDB_1..10 的 31 条 FDB.Exec 文本                        → 0 处不一致
```

即：**SQL 文本没有一处手工转录**，全部由脚本抽取 + 独立回读验证。

---

## 3. 门禁结果

```
cd .worktrees\p2-dbserver-mysql\GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo          → 0 error / 0 warning
dotnet test  GXX.slnx -c Debug --nologo
  GXX.Core.Tests            162 ✅
  GXX.M2Server.Tests       5053 ✅
  GXX.Client.Tests         2278 ✅
  GXX.DBServer.Tests        371 ✅   ← 既有 199 + 新增 172
  GXX.LoginSrv.Tests        229 ✅
  GXX.LogDataServer.Tests   142 ✅
  GXX.GatewayKit.Tests        7 ✅
  GXX.RunGate.Tests         248 ✅
  GXX.SelGate.Tests         163 ✅
  GXX.GameCenter.Tests      219 ✅
  GXX.Integration.Tests    1/2 ❌  ← 见下
```

`GXX.Integration.Tests.LoginSrvIntegrationTests.Client_Through_Gate_ToLoginSrv_RegisterAndLogin`
失败（30s 超时）。**与本次改动无关**：台账 §9.3-2 已登记它是"固定端口 + 并发负载"下的既有 flaky 用例，
在主干/集成分支同样偶发；本车道只新增 `src/GXX.DBServer/MySqlRoleDB*.cs`、`tests/GXX.DBServer.Tests/MySqlRoleDB*`、
`docs/并行报告-p2-dbserver-mysql.md`，不触碰 `GXX.Integration.Tests` 或其依赖。
`GXX.DBServer.Tests` 371 例全绿（含既有 199 例，无回归）。

### 新增测试分布（172 例）

| 测试文件 | 例数 | 覆盖对象 |
|---|---|---|
| `MySqlRoleDbSqlTests.cs` | 32 | 140 条 SQL 常量的逐字文本（含 20+ 条 InlineData 精确串）、占位符计数、反引号/REPLACE INTO/大小写/被注释掉的列 |
| `MySqlRoleDbBehaviorTests.cs` | 60 | DoInit（连接参数、140 语句、Clear、二次调用）、升级阶梯 12 个版本、UpdateDB 事务与回滚、`Run`/`OnRequest`、公开包装的 except 语义、13 个读取方法、`DoSelect` 三分支 |
| `MySqlRoleDbPayloadTests.cs` | 74 | `DoGet` 24 段子表、物品槽位/边界、`DoAdd/Delete/SetEnabled/Erase/Rename/ChangedGold/ChangedCustomMoney/Save`、Rank 三路径、Hero 全量 |
| `MySqlRoleDbLayoutTests.cs` | 6 | InlineArray/fixed-buffer 就地写入、`ref` 语义 |

---

## 4. 接缝设计（**数据库访问绝不进单测**）

```
IRoleMySqlDatabaseFactory ──CreateLib──► IRoleMySqlLib        （TMySQLLib + libmysql-32.dll）
        │
        └──CreateDatabase──► IRoleMySqlDatabase               （TMySQLDataBase）
                                   │ Init/Connect/CharacterSetName/MySql
                                   │ AddSQLStatement/ClearStatements
                                   │ StartTransaction/Commit/RollBack/Exec/Disconnect
                                   │ event OnRequest
                                   └──► IRoleMySqlStatement     （TMySqlStatement）
                                            Sql/Prepare/FinalizeStatement/Reset
                                            OrderBindParam{Text,Int,Bool,Double}
                                            Query/Fetch/Step
                                            GetColumnValueText/Int
                                            OrderGetColumnValue{Text,Int,Bool,Double}
```

- **与车道6（`GXX.LoginSrv`）的接缝刻意不同名**：车道4 的 `IMySqlStatement` 只有 AccountDB 用到的 8 个成员，
  且 `GXX.LoginSrv/RoleDBSeam.cs` 里的 `THumData/THeroData/TSerarchRoleData/TSerarchRoleList/IHumanRoleDB/...`
  也只是给 `uFrmDataManager` 用的**空接缝**（`sealed class THumData {}`，无字段）。
  本车道需要 `OrderBindParamBool/Double`、`Disconnect`、`OrderGetColumnValueDouble` 等，复用就必须改他人文件
  （违反铁律1），故在 `GXX.DBServer` 命名空间内自带一套 → **两项目互不引用，类型不冲突**。
- `RoleMySqlNativeSeamFactory.Instance` 是默认工厂：`CreateLib/CreateDatabase` 直接抛
  `NotSupportedException` 并在消息里指明"`MySQLWrap.pas/MySQLCli.pas` 未移植（libmysql-32.dll）"。
- `DBServer.csproj` 已有 `MySqlConnector 2.3.7` 引用但**本车道不用**（原文就是裸 libmysql，不是 ADO.NET 驱动）；
  如后续要接真库，实现上述 4 个接口即可，无需改本车道任何一行。

---

## 5. 发现的原文缺陷 / 易错点（全部逐字保留，并有测试锁定）

### 5.1 原文 Bug（保留不修）

| # | 位置 | 内容 | 锁定测试 |
|---|---|---|---|
| B1 | 1115-1142 `DoSearchByLevel` | `finally` 里 Reset 的是 **`FStatementSearchByNameMatchFuzzy`**，不是 `FStatementSearchByLevel` | `HumanDb_SearchByLevel_ResetsTheWrongStatement_InFinally` |
| B2 | 2116-2151 `DoChangedCustomMoney` | `HumanMoney` 里没有该 `MoneyName` 时整个 `if` 不进 → **`Result` 从未赋值**（Delphi 未初始化函数返回值，行为不定）；且 `finally` 两次 Reset 的都是 `FStatementUpdateCustomMoney` | `HumanChangedCustomMoney_MissingRow_ReturnsFalse` |
| B3 | 2153-2239 `DoChangedGold` | `case ChangeType of` **没有 `cgtCustomMoney` 分支** → 该类型下 `ResultValue` 保持调用方初值，但仍执行 UPDATE | `HumanChangedGold_CustomMoneyChangeType_HasNoCaseBranch_SoValueIsUnchanged` |
| B4 | 2750-2916 `AddHumanItemToDB` | `btAddDataByte` 那一组绑的是 **`UserItem.btNewValue[J]`**（不是 `btAddDataByte[J]`）→ 该子表落库的是"元素值"而非"附加数据字节"。**英雄侧（5213-5379）是正确的 `btAddDataByte[J]`** | `HumanSave_*`（Human 与 Hero 的绑定点在代码里各自注明） |
| B5 | 4633-4655 | `TSqliteHeroDB.DoDelete/DoDeleteRestore` 的类名写错（复制自 Sqlite 版），且整体被 `{ }` 注释掉 | 不实现（与原文一致） |
| B6 | 254/3461 | 注释 `{ TSqliteHumanDB }` / `{ TSqliteHeroDB }` 同样是复制残留 | 代码注释保留 |
| B7 | 260/3467 | `Assert` 文案 `'TSqliteHumanDB owner type error.'` 同样残留 | 抛出的消息逐字保留 |
| B8 | 1313-1316 `DoGet` | `HumData.boStorageOpen[0] := True` **恒置**，仓库 1 从不读库；SQL 里也没有 `IsOpenStorage1` 列（`HumanUpdate` 也只绑 `[1][2][3]`） | `HumanGet_StorageOpen1IsAlwaysOne_AndOnlyTwoColumnsAreRead` |
| B9 | 934-998 `DoQueryHumans/DoQueryDeleteHumans` | `QueryData` 是**同一个局部 record**，`HumanList.Add(@QueryData)` 传的是同一块栈地址；且循环内从不重新初始化 → 列表里所有元素实际指向同一内存，循环后内容全部相同（悬垂指针式共享） | 托管侧每次 `new`，`HumanDb_QueryHumans_*` 锁定"逐行独立"的意图语义 |
| B10 | 1831-1840 `DoGet` 的 `CustomMoney` | `Index` 无上界检查（`CustomMoney` 是 `array[0..29]`），DB 多返回一行即越界写 | 托管侧加 `Index > 29 → break`（**唯一有意加的保护**，见 §5.2） |
| B11 | 1333-1334 / 4136-4137 | `if not Result then Exit;` 在 `try..finally` 里，`Exit` 仍会走 `ResetAllGetDataStatement` | `HumanGet_NoMainRow_ReturnsFalse_*` |

### 5.2 托管侧的有意改写（等价，已在代码注释说明）
1. **指针就地改写 → 局部副本 + 出口整体回写**：`DoGet` 用 `THumData H = HumData; … HumData = H;`（`DoGet` 有 24 段子表、每段都可能提前 `continue`，用 `ref` 局部别名不可行）。语义等价。
2. **`TUserItem`/内嵌 fixed 数组访问**：`Grobal2.Types4/6.cs` 里 7 组物品是 `[InlineArray]` 展开字段，**无法取地址**（原文 `UserItem := @HumData.BagItems[I]`），故用 `GetHumanItem/SetHumanItem` 成对"整体读—整体写"；`btAddDataByte/nAddDataInt/经脉穴位/宠物技能/仓库开关/任务标记` 用 `MySqlItemAccess` 的 `unsafe fixed` 访问器。**这些访问器放在本车道文件里，不改 `GXX.Core`**。
3. **`CustomMoney` 上界守卫**（B10）：多一个 `if (Index > 29) break;`。
4. **`DoGetRankData` 的 4 个职业块**：原文把同一段 25 行展开 4 次（仅 Job 常量与目标列表不同），托管侧抽成 `RunRankBlock(stmt, list, Job, …)`，**绑定序列、过滤判据、Break 条件逐字一致**；`QueryCount = TopCount + 50` 与三条路径的参数形态（`(j,j,j)`；`(j,j,j,Min,Max)`；`(j,j,j,Min,Max,QueryCount)`）用同一函数的三分支还原。
5. **`DoQueryHumans` 的 `QueryData`**（B9）：每次 `new`（见上）。
6. **`GetHumanDBClass/GetHeroDBClass` 的虚构造**：Delphi `class of` 元类在 C# 无对应，改为在 `DoInit` 里 `new TMySqlHumanDB(this)` / `new TMySqlHeroDB(this)`（顺序同 `RoleDB.pas:1263-1264`）。
7. **`TMySqlRoleDB` 不继承 `GXX.LoginSrv` 的接缝基类**：本车道自带 `THumanDBBase/THeroDBBase` 骨架，`Lock/UnLock` 用私有 `Monitor`（对应 `RoleDB.pas:1297-1305`）。
8. **`MainOutMessage` / `Date2MyDate`**：`DBShare.pas` 未整体移植（车道4 只做最小接缝且本车道不可改），故在 `MySqlRoleDB.ShareFilter.cs` 里按原文实现 `CheckFilterRankingChrName`（含 `Pos('', s) = 1` → 空串命中的怪行为）与可注入时钟的 `Date2MyDate`。

---

## 6. Sqlite 版 vs MySql 版的**真实差异清单**

用 `diff_sql.ps1` 对两边 `FStatement*.Sql` 逐条 `ceq` 比对（MySql 140 条 / Sqlite 146 条）。
**70 条逐字相同，70 条文本不同，Sqlite 多 6 条。**

### 6.1 方言差异（文本不同）

| 类别 | MySqlRoleDB.pas | SqliteRoleDB.pas | 影响 |
|---|---|---|---|
| 保留字引用 | 反引号：``select `Index`,`Value` …`` | 双引号或裸写：``select "Index",Value …`` | `Human/Hero` 的 AbilNpcAdd / GodBlessState / StatusTime / QuestFlag / VariableU/T/J/Z / ItemValueAdd / ItemElementAdd / ItemAddData* / ItemFlute / ItemProgress / ItemProperty / GetCustomMoney(ByName) / MagicUseTick 共 ~45 条 |
| 表名大小写 | ``select … FROM  Human …``（`FROM` 后两个空格） | `FROM  human`（小写） | `HumanGetMobileNumbers1` |
| 插入语句 | `replace into HumanAbil/HumanAbilNG/HumanAbilWine/HeroAbil/HeroAbilNG/HeroAbilWine` | `insert into …` | 语义不同：MySQL 的 `REPLACE` 会**先删后插**（覆盖既有行）；SQLite 版是纯 INSERT |
| 逗号后空格 | `InsuranceCount, NewExpand3, NewExpand4`（`,`+空格） | `InsuranceCount,NewExpand3, NewExpand4`（缺一个空格） | `HumanInsertItems` |

> ⚠️ 注意：上表第 4 行是**容易误判为"无差异"**的地方 —— 必须用 `-ceq` 逐字节比较，
> 用 `-eq`/`-match` 之类的宽松比较会把 `" , "` 与 `", "` 判成相同。
> 本车道的 `diff_sql.ps1` 用的就是 `-ceq`，所以 `identical=70 / textDiff=70`。

### 6.2 语句数差异（Sqlite 多 6 条）

Sqlite 版多出 **`HumanUpdateAbil` / `HumanUpdateAbilNG` / `HumanUpdateAbilWine` / `HeroUpdateAbil` / `HeroUpdateAbilNG` / `HeroUpdateAbilWine`** 这 6 条 `update` 语句。
MySql 版把对应 6 处**用 `{ }` 注释掉**（Human 行 3431-3435 / 606-608 附近；Hero 行 3588-3681 那整段被 `(* *)` 包住），
只保留 `replace into`。→ **MySQL 侧用 REPLACE 替代 UPDATE，所以这 6 条语句根本不存在**。

### 6.3 方法面差异（代码层）

| 项 | MySql | Sqlite | 说明 |
|---|---|---|---|
| `DoCheckHumanExists` | `if Query and Fetch then True` | `Result := Step = SQLITE_ROW` | MySql 版代码里留着被注释掉的 `// Result := …Step = SQLITE_ROW;` |
| `DoBuyPlayer` | `Result := FStatementBuyPlayer.Step` | `FStatementBuyPlayer.Step; Result := Step in [SQLITE_OK, SQLITE_DONE]` | **Sqlite 版把 `Step` 调了两次**（第一次的结果被丢弃），MySql 版只调一次 |
| `HeroDB.DoDelete/DoDeleteRestore` | 被 `{ }` 注释掉 | 同样被注释但类名写成 `TSqliteHeroDB` | 两边都不实现 |
| 版本阶梯常量 | `MYSQL_DBVERSION` | SQLite 侧用同名的 `MYSQL_DBVERSION`（复制残留） | 两边引用同一个常量 |
| `DoInit` 的语句名 | `'HumanGetID'` 等 140 个名 | 同名（本报告 §6.2 的 6 条除外） | 名字一致，便于对照 |

---

## 7. 未完成 / 后续接口

1. **`MySQLDataBase.pas` / `MySQLWrap.pas` / `MySQLCli.pas` + `libmysql-32.dll` 仍未移植**。
   本车道的 4 个接口就是接入口；实现后 `new TMySqlRoleDB(new RealFactory())` 即可跑真库。
   已实现的部分**没有一处**依赖 `MySqlConnector` 包。
2. **`DBShare.pas` 的 MySQL 连接参数全局**（`g_sDataSaveDBServer/User/Password/DataBase/Port`）暂放
   `MySqlRoleDbGlobals`（车道4 的 `DBShareSeam` 里没有这几个变量且本车道不可改它）。
   待 `DBShare.pas` 正式移植后应合并过去（届时是"移动 + 转调"，不动语义）。
3. **`RoleDB.pas` 的 `THumanDB/THeroDB/TRoleDB` 基类**目前是 `GXX.DBServer` 内的骨架
   （`MySqlRoleDB.Base.cs`）。`SqliteRoleDB.pas` 车道开工时应直接复用它 —— **不要再造第三套 THumanDB 骨架**
   （台账 §9.3-1 的重复劳动教训）。
4. **`GXX.LoginSrv/RoleDBSeam.cs` 的空接缝**（`THumData {}`/`THeroData {}` 无字段）与
   `GXX.DBServer` 的真实 `GXX.Core.Protocol.THumData/THeroData` 是两套东西。
   待 `uFrmRoleDataEdit.pas` 移植或 `DBServerService` 接线时，建议把 LoginSrv 的 `IRoleDB/IHumanRoleDB/IHeroRoleDB`
   接到本车道的 `TMySqlRoleDB.HumanDB/HeroDB`（方法名已对齐：`SearchByAccount/SearchByName/Get/SetEnabled/Erase`）。
   **跨项目改动属集成者范围，本车道未动。**
5. **`DoDelete/DoDeleteRestore`（人物删除）在 MySQL 侧落库的是 `DeleteOrRestore` 语句**，
   而 `CreateId.pas` / `uFrmMain.pas` 的删除规则（`g_boCanDeleteHuman`、`g_nCanDeleteHumanLowLevel`）
   属窗体/上层逻辑，不在本单元内。
6. **覆盖率审计**：`tools/audit-coverage.ps1` 的证据规则 E2 要求 `.cs` 头部 40 行内提到 `<unit>.pas`。
   `MySqlRoleDB.Human.cs` / `.HumanRead.cs` / `.HumanWrite.cs` / `.Hero.cs` / `.Role.cs` / `MySqlRoleDB.Base.cs`
   的文件头都写了 `Source\DBServer\MySqlRoleDB.pas` 与行号区间 → 满足 E2。
7. **文件命名的轻微偏差**：任务书给的测试文件名模式是 `tests/GXX.DBServer.Tests/MySqlRoleDB*`（大写 `DB`），
   本车道除 `MySqlRoleDBFakes.cs` 外，三个测试文件用了可读性更好的 `MySqlRoleDb*`（小写 `b`）命名
   （`MySqlRoleDbSqlTests.cs` / `MySqlRoleDbBehaviorTests.cs` / `MySqlRoleDbPayloadTests.cs` / `MySqlRoleDbLayoutTests.cs`）。
   全部落在既定的独占目录内、不与任何既有文件同名，**如需严格对齐模式可在集成时 `git mv` 重命名，不影响内容**。
8. **`GXX.Integration.Tests` 的既有 flaky 用例**（§3）建议按台账 §10-12 改成随机端口或加串行 collection —
   属集成者/该测试所有者范围，本车道未动。

---

## 9. 与台账 §9.4 / §10 的衔接

- **§9.4 `HUtil32.GetValidStr3` 根因修复**：本单元**不使用** `GetValidStr3`（`MySqlRoleDB.pas` 全文无该调用），
  故不涉及；`DBShareSeam.Filter` 只用了 `CheckFilterRankingChrName`（本车道按 DBShare.pas 原文实现）。
- **§10-4「`DBServer` 侧 `RoleDB / SqliteRoleDB / MySqlRoleDB` 仍未移植」**：本车道**已补上 `MySqlRoleDB`**
  （`RoleDB` 的 `THumanDB/THeroDB/TRoleDB` 基类面也已作为 `MySqlRoleDB.Base.cs` 落地）。
  剩余 `RoleDB.pas` 的 `TQueryHumanList/TSerarchRoleList/TRoleRankList` 列表实现（RoleDB.pas:352-503）
  与 `SqliteRoleDB.pas` 仍待认领；**`SqliteRoleDB` 车道请直接复用本车道的 `THumanDBBase/THeroDBBase`**。
- **§10-8 `CreateId.pas` 只有窗体骨架**：与本单元无关（真正的 ID 分配在 `DoGetID`/`Human` 表的
  `AUTO_INCREMENT`，见 `MySqlRoleDB.pas:839-851` —— 注意 `DoGetID` 只**查** ID，不自增）。

---

## 8. 提交

- 分支 `par/p2-dbserver-mysql`，按切片提交（见 `git log`）。
- 只做本工作树内的 `git add` / `git commit`；未执行任何 `merge/rebase/checkout/switch/push/worktree`。
- 未在主工作树创建/修改任何文件；未向主工作树写临时脚本（所有探查脚本都在 `D:\chuanqi\_gbkread\`，仓库外）。

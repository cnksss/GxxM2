# 并行报告 — `p2b-m2-dblayer`（M2Engine 的 6 个 DB 访问层单元）

> 分支：`par/p2b-m2-dblayer` ｜ 工作树：`.worktrees/p2b-m2-dblayer`
> 基线：`main @ 5bd12133`（integrate par/p2-resources-uib）
> 交付形态：**切片 1 完成**（接缝层 + UserShop 镜像对，门禁全绿）；M2DataDB / AuctionDB 两对**未开始**（见 §7）

---

## 1. 分支与提交

| # | commit | 内容 |
|---|---|---|
| 1 | `（见 §1 末尾实际 hash）` | 并行批次P2b：DbLayer 接缝层 + `Sqlite/MySqlUserShopDB` 1:1 移植（35 例测试） |

工作树内只有这一个提交；未做任何 `merge/rebase/checkout/switch/push/worktree`。

---

## 2. 新增文件清单

### 生产代码（`GXX.CSharp/src/GXX.M2Server/DbLayer/**`，全部为新建）

| 文件 | 行数 | 对应源单元 / 职责 |
|---|---|---|
| `DbSeam.cs` | ~560 | **接缝层**：`IDbLayerHost` / `IDbLayerEnvironment` / `IDbLayerLog` / `ISqliteStatement` / `ISqliteDatabase` / `IMySqlStatement` / `IMySqlDatabase` / `IUserShopDb` / `IAuctionDb` / `IM2DataDb` + 默认"未接入"实现 |
| `DbBases.cs` | ~880 | `M2DataCommon.pas` 的 **`TUserShopDB` / `TAuctionDB` 基类**（public 加锁 + 吞异常 + `DoXxx` 虚方法模板，逐方法 1:1） |
| `DataCommon.cs` | ~430 | `M2DataCommon.pas` 的类型面：4 个 `*_ITEM_TYPE` 常量、`TShopItemType`、`TUserShop`、`TUserShopItem`、`TSimpleUserShopItem`、`TSelledAndNoGetMoneyTotal`、`TUserShopList`、`TUserShopItemList`、`TAuctionRecord/Info`、`TAuctionItemList`、`ShortString13` |
| `DbLayerSeams.cs` | ~110 | `DelphiDateUtil`（`UnixToDateTime`/`DateTimeToUnix`）+ `DbLayerRunSeam`（`GetStdItem`/`ProcessItemName`/`GetUserItemBindValue`/`GetItemRule`/`UpdateShopItem`/`AddGameDataLogItemMove`） |
| `SqlStatements.SqliteUserShopDB.cs` | 生成物 | `SqliteUserShopDB.pas` 的 **41 条语句 SQL 逐字**（+5 条方法级 const） |
| `SqlStatements.MySqlUserShopDB.cs` | 生成物 | `MySqlUserShopDB.pas` 的 **43 条语句 SQL 逐字**（+5 条方法级 const） |
| `SqliteUserShopDB.cs` | ~1630 | `SqliteUserShopDB.pas` 1:1 移植（`TSqliteUserShopDB`） |
| `MySqlUserShopDB.cs` | ~1645 | `MySqlUserShopDB.pas` 1:1 移植（`TMySqlUserShopDB`） |

### 测试（`GXX.CSharp/tests/GXX.M2Server.Tests/`）

| 文件 | 内容 |
|---|---|
| `DbLayerTestKit.cs` | 内存接缝实现：`FakeDbLayerHost` / `FakeSqliteDatabase` / `FakeMySqlDatabase` / `ScriptedSqliteStatement` / `ScriptedMySqlStatement` / `FakeDbLayerEnvironment` / `CapturingDbLayerLog` / `DbLayerTestKit`（SHA-256 口径与生成器一致） |
| `DbLayerFingerprints.SqliteUserShopDB.cs` | 生成物：41 条语句的 `label → SHA-256(SQL)` |
| `DbLayerFingerprints.MySqlUserShopDB.cs` | 生成物：43 条 |
| `DbLayerSqlFidelityTests.cs` | 9 例：SQL 指纹、语句数量/顺序、方言差异面、Prepare/Finalize 生命周期、基类加锁模板 |
| `DbLayerUserShopBehaviorTests.cs` | 26 例：空结果集、负数/边界绑定、超长字符串截断、时间戳格式（方言差异断言）、重复插入/名称占用、事务边界、列表容器语义 |

### 机械抽取/生成脚本（工作树内临时目录 `_recon/`，**不提交**）

`delphi-sql-extract.mjs`（GBK 词法 + Delphi `'+'` 拼接合并 + `const` 展开）、`labels.mjs`（语句字段→`AddSQLStatement` label 映射，来自原文）、`gen-statements.mjs`、`gen-fingerprints.mjs`、`diff-sql.mjs`（方言对账）、`fix-stmt-refs2.mjs`、`grep-ref.mjs` / `grep-file.mjs` / `dump-sql-ref.mjs` / `dump-pas.mjs`（侦察辅助）。

---

## 3. 侦察结果：每单元方法/字段/常量清单（含行号）

### 3.1 `SqliteUserShopDB.pas`（2161 行）／`MySqlUserShopDB.pas`（2190 行）

| 项 | Sqlite | MySql |
|---|---|---|
| `interface` / `implementation` / `end.` | 3 / 127 / 2160 | 3 / 130 / 2189 |
| 类 | `TSqliteUserShopDB = class(TUserShopDB)` 16–125 | `TMySqlUserShopDB = class(TUserShopDB)` 16–128 |
| `FDB` 字段 | 18 `TSQLite3Database` | 18 `TMySqlDatabase` |
| 语句字段 | 20–77（42 个） | 20–80（44 个：多 `FStatementSetTimeHasArrivedSellItems` 40、`FStatementGetItemBindOption` 80） |
| `Create` / `Destroy` | 134–196 / 198–201 | 137–202 / 204–207 |
| `DoInit` / `DoFinal` | 203–473 / 475–748 | 209–492 / 494–779 |
| `DoShopAdd` … `DoRun`（20 个 `Do*`） | 750 / 764 / 772 / 780 / 952 / 1048 / 1084 / 1361 / **1671** / 1775 / 1801 / 1809 / 1867 / 1887 / 1924 / 1938 / 1951 / 1978 / 2012 / 2024 / 2057 / 2069 | 781 / 795 / 803 / 811 / 984 / 1080 / 1116 / 1394 / **1704** / 1808 / 1834 / 1842 / 1900 / 1920 / 1958 / 1972 / 1985 / 2011 / 2044 / 2056 / 2088 / 2100 |
| 方法级 `const` | 205 `SGetShopItemQueryField`、208 `..._MakeIndex`、213 `SGetShopItemQueryCount`、214 `SGetShopItemWhere`、218 `SGetAllShopQueryField` | 211 / 214 / 219 / 220 / 224（同名，行号 +6 / +5） |
| 注释掉的整段方法 | `DoGetHumanItems` 1564–1669（花括号） | 1597–1702（`(* *)`） |
| 注释掉的 4 条语句 | 417–450 | 427–460 |
| 无 `Run` 覆写（基类 `M2DataCommon.pas:1157-1168`） | ✔ | ✔ |
| 方言（同名方法内的真实差异）见 §5 | | |

### 3.2 `SqliteM2DataDB.pas`（1708 行）／`MySqlM2DataDB.pas`（1398 行）

| 项 | Sqlite | MySql |
|---|---|---|
| `interface` / `end.` | 3–75 / 1708 | 3–66 / 1398 |
| 类 | `TSqliteM2DataDB = class(TM2DataDB)` 24 | `TMySqlM2DataDB = class(TM2DataDB)` 10 |
| 私有新增方法 | `UpdateDB_1..6` 338/350/412/423/435/445、`DoUpdate` 475–961 | **`UpdaeDB_1..5` 136/151/165/180/192**（原文拼写 `Updae`）、`OnRequest` 1384、`Run` 1389 |
| `Create`/`Destroy` | 83–110 / 112–116 | 74–112 / 114–119 |
| `DoInit` | 133–336 | 238–626 |
| `DoFinal` | 963–1073（**不 Finalize `GetItems`/`GetItems_Sort`**） | 628–738（同） |
| `DoLoadItemsFromDB` / `DoLoadItemFromDB` / `DoSaveItemToDB` | 1075–1292 / 1294–1496 / 1498–1701 | 740–960 / 962–1169 / 1171–1377 |
| `GetDataBase` | 1703–1706 | 1379–1382 |
| `const`（方法内） | **无** | **无** |
| 语句名（20 条相同） | `InsertItems`222 … `SelectItemProperty`332 | 293 … 611 |
| 独有语句名 | `get_db_constant_value`493、`temp`873、`temp`917（**同名复用**） | `get_db_constant_value`248 |
| 独有 SQL | `PRAGMA …` / `SQLITE_CREATE_M2DATA_TABLES` / `strftime` / `COLLATE NOCASE` / `AUTOINCREMENT` / 7 段内联迁移 DDL | `REPLACE INTO db_constant(ConstName, ConstValue) values ("m2data_version", …)` / 反引号 DDL / `VARCHAR(30)` |
| 版本常量 | `SQLITE_M2DBVERSION='20200916'` | `MYSQL_M2DB_VERSION='20200916'` |

### 3.3 `SqliteAuctionDB.pas`（1604 行）／`MySqlAuctionDB.pas`（1584 行）

| 项 | Sqlite | MySql |
|---|---|---|
| `interface` / `end.` | 3–118 / 1603 | 3–111 / 1583 |
| 类 | `TSqliteAuctionDB = class(TAuctionDB)` 17 | `TMySqlAuctionDB = class(TAuctionDB)` 10 |
| 私有字段 | 19–48（22 个：21 条语句 + `FDB`） | 12–41（同，22 个） |
| 方法 | **22 个 `override` + `Create`/`Destroy` override，0 个新方法** | 同 |
| `Create`/`Destroy` | 142–191 / 193–196 | 134–166 / 168–171 |
| `DoInit` / `DoFinal` | 198–335 / 337–465 | 173–308 / 310–439 |
| `DoAddAuctionItem` … `DoHumanRename` | 467 / 595 / 611 / 626 / 655 / 689 / 723 / 761 / 917 / 964 / 1019 / 1054 / 1092 / 1185 / 1200 / 1222 / 1245 / 1268 / 1290（`DoRun`，内含嵌套过程 `IncPlayerGameMoney` 1292–1411）/ 1564 | 441 / 569 / 585 / 600 / 631 / 667 / 703 / 741 / 896 / 943 / 998 / 1033 / 1071 / 1164 / 1179 / 1201 / 1224 / 1247 / 1269（嵌套过程 1271–1390）/ 1544 |
| 方法级 `const` | **两个文件都没有** | 同 |
| 语句名（23 个，两文件完全一致） | 205…298 + 动态 773 / 1100 | 180…271 + 752 / 1079 |
| 差异面 | **5 个 SQL 簇**：超时判定（`strftime` vs `TIMESTAMPDIFF`）、`TimeLeft` 表达式、`LastBidTime`（`strftime("%s","now")` vs `CURRENT_TIMESTAMP`）、流拍计数多一层括号、`SortField=3` 的 `ORDER BY`（SQLite 带 `A.` 前缀，MySQL 不带）；MySQL 独有 `FDB.ClearResult` 与 `RowCount <> 0` 存在性判据；SQLite 独有 `Step in [SQLITE_OK, SQLITE_DONE]` 与动态语句 `sm.Finalize`；**唯一非方言语义差异**：`AddDateTime` 在 SQLite 用 `OrderGetColumnValueInt`（Unix 秒直接赋给 `TDateTime`），MySQL 用 `OrderGetColumnValueDateTime` | |
| `MinPrices/MaxPrices` 交换 | **只在基类**（`M2DataCommon.pas:1246-1251` 与 `1311-1316`），派生类不重复 | 同 |

---

## 4. 本次交付覆盖情况（切片 1）

| 单元 | 已覆盖行号范围 | 未覆盖部分（方法名清单） |
|---|---|---|
| `SqliteUserShopDB.pas` | **1–2161 全覆盖**（含 1564–1669 的注释段以"原文无实现"注释保留、417–450 的 4 条注释语句以注释保留） | 无 |
| `MySqlUserShopDB.pas` | **1–2190 全覆盖**（含 1597–1702、427–460 注释段） | 无 |
| `SqliteM2DataDB.pas` | 0 | **全部**：`Create`/`Destroy`/`GetAuctionDBClass`/`GetStorageDBClass`/`GetUserShopDBClass`/`DoInit`/`UpdateDB_1..6`/`DoUpdate`/`DoFinal`/`DoLoadItemsFromDB`/`DoLoadItemFromDB`/`DoSaveItemToDB`/`GetDataBase` |
| `MySqlM2DataDB.pas` | 0 | **全部**：`Create`/`Destroy`/`Get*Auction/Storage/UserShop DBClass`/`UpdaeDB_1..5`/`DoInit`/`DoFinal`/`DoLoadItemsFromDB`/`DoLoadItemFromDB`/`DoSaveItemToDB`/`GetDataBase`/`OnRequest`/`Run` |
| `SqliteAuctionDB.pas` | 0 | **全部 22 个 `override` + `Create`/`Destroy`**（`DoQueryAllItems`/`DoQueryMyItems`/`DoQueryMyAttentionItems`/`DoGetAllItemsPageCount`/`DoGetMyItemsPageCount`/`DoGetMyAttentionPageCount`/`DoGetMyAuctioningItemsCount`/`DoGetMySellFailItemsCount`/`DoGetMyBuyOKItemsCount`/`DoAddAuctionItem`/`DoCancelAuctionItem`/`DoRetrieveAuctionItem`/`DoDeleteAuctionItem`/`DoAddAttentionItem`/`DoDeleteAttentionItem`/`DoJoinItemBid`/`DoGetAuctionInfo`/`DoGetAuctionRecord`/`DoHumanRename`/`DoRun`/`DoInit`/`DoFinal`） |
| `MySqlAuctionDB.pas` | 0 | 同上 |
| `M2DataCommon.pas`（依赖，非任务单元） | 只搬了六个单元用到的公开面：常量 8–12、`TShopItemType` 54、`TUserShop` 56–67、`TUserShopItem` 71–84、`TSimpleUserShopItem` 88–114、`TSelledAndNoGetMoneyTotal` 118–122、`TUserShopList` 126–140、`TUserShopItemList` 142–157、`TAuctionRecord/Info` 262–296、`TAuctionItemList` 299–314、`TUserShopDB` 159–257（基类模板）、`TAuctionDB` 317–454（基类模板） | `TStorageDB` 17–50 及其实现、`TM2DataDB` 实现 1615–1702（仅以 `IDbLayerHost`/`IM2DataDb` 接缝表达） |

---

## 5. SQLite 版 vs MySQL 版的**真实差异清单**（UserShop 对，全量对账）

用 `_recon/diff-sql.mjs` 对 41 条共有语句逐条比对：**31 条逐字相同、10 条不同**。

| # | 语句 | SQLite | MySQL | 性质 |
|---|---|---|---|---|
| 1 | `FStatementUpdateUserShopItem` | `CreateDate = (strftime(''%s'', ''now''))` | `CreateDate = CURRENT_TIMESTAMP` | 方言 |
| 2 | `FStatementBuyUserShopItem` | `CreateDate = (strftime(''%s'', ''now''))` | `CreateDate = CURRENT_TIMESTAMP` | 方言 |
| 3 | `FStatementGetTimeHasArrivedSellItems` | `(strftime("%s", "now")) - A.createdate > ?` | `TIMESTAMPDIFF(SECOND, A.createdate, CURRENT_TIMESTAMP) > ?` | 方言 |
| 4 | `FStatementGetUserShopInfo` | 子查询 `where shopid = a.shopid` | 子查询 `where ShopID = A.ShopID` | **非方言**（原文如此） |
| 5–10 | `FStatementGetAllShop_Sort0..5` | 同上（小写相关子查询） | 同上（大写） | **非方言** |

**MySQL 独有 2 条语句**：
- `UserShop_SetTimeHasArrivedSellItems`（原文 324–327）= `UPDATE UserShopItem set IsAllowSell = 0 WHERE (IsAllowSell = 1) and length(ifnull(BuyerName, '''')) = 0 and TIMESTAMPDIFF(SECOND, CreateDate, CURRENT_TIMESTAMP) > ?`
- `UserShop_GetItemBindOption`（原文 488–491）= `select MakeIndex,IsBind,BindOption from Items where ParentID = ? and ItemType = ? and ItemIndex = ?;`

**API 方言对照**（逐方法保留）：

| 能力 | SQLite | MySQL |
|---|---|---|
| 事务 | `BeginTransaction` / `Commit` / `RollBack` | `StartTransaction` / `Commit` / `RollBack` |
| 批量脚本 | `Execute(sql)` | `Exec(sql)` + `ClearResult` |
| 绑定 | `OrderBindInt/Int64/Bool/Text` | `OrderBindParamInt/Bool/Text/DateTime` |
| 读列 | `OrderGetColumnValueInt/Int64/Bool/Text/Double` | + `OrderGetColumnValueDateTime` |
| 无结果集成功判据 | `Step in [SQLITE_OK, SQLITE_DONE]` | `Step`（Boolean） |
| 结果集循环 | `Ret := Step; while Ret = SQLITE_ROW do … Ret := Step` | `if Query then while Fetch do …` |
| 单行查询 | `if Step = SQLITE_ROW` | `if Query and Fetch` |
| 元素存在性 | `Step = SQLITE_ROW` | `Query; RowCount <> 0` |
| 动态语句收尾 | `sm.Reset; sm.Finalize` | `sm.Reset` |
| 时间写法 | Unix 秒（`UnixToDateTime(x + 8*60*60)`） | `OrderGetColumnValueDateTime`（**不加 8h**） |

**原文缺陷/易错点（逐字保留，已在代码与测试中登记）**：

1. `DoGetSellItemsCount` 的 `IsMyShop` **两分支都绑 (1, 2)**（Sqlite 1443–1452 / MySql 1476–1485），而 `DoGetSellItems` 是 `(1,1)` / `(1,2)` —— 两文件一致的不对称。测试 `*_GetSellItemsCount_BindsOneTwoForBothBranches_MirroringTheOriginalBug` 锁死。
2. `FStatementGetAllShop_Sort5` 在**两个文件里都没有 `Prepare`**（Sqlite 404 / MySql 414）。测试锁死。
3. `DoShopAdd(ShopName, HumanName)` 形参顺序与绑定顺序**相反**（绑 `HumanName` 再 `ShopName`，Sqlite 757–758 / MySql 788–789）。测试锁死。
4. `DoDeleteItem` 的多语句脚本里 `'-------------------------------'` 分隔行：**SQLite 版有**（1908）、**MySQL 版被注释掉**（1941）。测试分别锁死两方言。
5. MySQL `DoRun` 读 `dCreateDate` 仍用 `OrderGetColumnValueInt64 + UnixToDateTime(+8h)`（2130），而 SQLite 同位置也是这套写法 —— 与 `DoGetSellItems` 用 `OrderGetColumnValueDateTime` 不一致，属原文混用；**逐字保留**。
6. MySql `DoRun` 的日志字符串含原始乱码（`'店铺->店铺仓库[到时物品]'` 在 GBK→UTF-8 双跳后为 `'搴楅摵->...'`），SQLite 版是正常中文；原文两文件不一致，本车道按**语义等价**统一为可读中文（属日志文本，不影响协议/DB）。
7. SQLite `DoGetAllShop` 的 `SM := nil` 后 `if SM = nil then Exit;`（872–873）在 `case` 已覆盖 0..5 时是死分支；MySQL 同构（903–904）。逐字保留。

---

## 6. 测试与门禁

```
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p2b-m2-dblayer\GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo          → 0 个错误
dotnet test tests\GXX.M2Server.Tests\... -c Debug → 已通过 5088 / 失败 0（基线 5053 + 新增 35）
```

- 新增 35 例：`DbLayerSqlFidelityTests` 9 例 + `DbLayerUserShopBehaviorTests` 26 例。
- **数据库访问绝不进单测**：全部经 `FakeSqliteDatabase` / `FakeMySqlDatabase` 内存实现。
- **SQL 逐字保真的可验证做法**：
  1. `_recon/delphi-sql-extract.mjs` 用 Node 以 GBK 解码原文，做 Delphi 词法（含 `''` 双写单引号的累积式词法、`{…}`/`(*…*)`/`//` 注释跳过）与 `'+'` 拼接合并、方法级 `const` 展开；
  2. `gen-statements.mjs` 把结果写成 `SqlStatements.*.cs` 的 `public const string`（**零人工转录**）；
  3. `gen-fingerprints.mjs` 为 41/43 条语句生成 `label → SHA-256(SQL)`；
  4. 测试 `Sqlite_DoInit_BindsExactlyTheExtractedSql` / `MySql_...` 让实现跑一遍 `DoInit`，逐个语句取"实现真正绑上去的 SQL"算 SHA-256 与指纹表比对 —— 任一手抄字符漂移都会失败。

---

## 7. 接缝 / 未完成

### 7.1 已定义的接缝（注释均含 `// 接缝：待 <单元名> 移植后接入`）

| 接缝 | 原文出处 | 现状 |
|---|---|---|
| `IDbLayerHost` | `M2DataCommon.pas:460-504 TM2DataDB`（`Lock`/`UnLock`/`DataBase`/`LoadItemFromDB`/`SaveItemToDB`） | 接缝；`DbSeam.cs` |
| `ISqliteStatement` / `ISqliteDatabase` | `SQLite3DataBase.pas` + `SQLiteCli.pas`（**源码树中不存在**，外部依赖） | 接缝；默认实现抛 `NotSupportedException` |
| `IMySqlStatement` / `IMySqlDatabase` | `MySqlDataBase.pas` + `MySqlWrap.pas`（**源码树中不存在**，直连 `libmysql-32.dll`） | 接缝；默认实现抛 `NotSupportedException` |
| `IDbLayerEnvironment` | M2Share `g_Config` 的 16 个成员 | 能从既有 `M2Config` 读的 7 个直接转调；其余 9 个以 `Seam*` 可写静态字段承载 |
| `IDbLayerLog` | M2Share `MainOutMessage` | 接缝；默认写 `Console.Error` |
| `DelphiDateUtil` | DateUtils `UnixToDateTime`/`DateTimeToUnix` | 本层自实现（本地时区），互逆 |
| `DbLayerRunSeam.ProcessItemName` | Grobal2.pas:6380 | 接缝；默认原样返回 |
| `DbLayerRunSeam.GetUserItemBindValue` | ObjBase/M2Share | 接缝；默认按位判定 |
| `DbLayerRunSeam.GetStdItem` / `GetItemRule` / `UpdateShopItem` / `AddGameDataLogItemMove` | UsrEngn / ItemRules / TM2DataDB / M2Share | 接缝；默认"无宿主"（`DoRun` 因此不产生副作用） |
| `IM2DataDb` | `M2DataCommon.pas:460-504` 公开面 | 接缝，本切片未装配 |

### 7.2 未完成（本车道剩余工作）

1. **`SqliteM2DataDB` + `MySqlM2DataDB`**：0 行移植。两个单元共 3,106 行；含 20 条 `AddSQLStatement`（SQL canonical 两方言 21/22 条逐字相同）、`DoLoadItemsFromDB`/`DoLoadItemFromDB`/`DoSaveItemToDB` 的完整字段读写序列、SQLite 侧 7 段版本迁移 DDL（`UpdateDB_1..6` + `DoUpdate`）、MySQL 侧 `UpdaeDB_1..5` + `Run` 保活。侦察报告已完整（见 §3.2），可直接开工。
2. **`SqliteAuctionDB` + `MySqlAuctionDB`**：0 行移植。两个单元共 3,188 行；22 个 `override` 方法，其中 `DoQueryAllItems`/`DoGetAllItemsPageCount` 的 ItemGroup 归类（28 个 `StdMode` 分支）与 `DoRun` 的嵌套过程 `IncPlayerGameMoney`（含 5 类货币税后结算 + `g_FunctionNPC.GotoLable` 两个 NPC 页）是最大块。侦察报告已完整（见 §3.3）。
3. **`TM2DataDB` 完整移植**：本切片只做了 `TUserShopDB`/`TAuctionDB` 基类模板 + `IDbLayerHost`/`IM2DataDb` 接缝，`TM2DataDB.Init/Final/Run/Lock/UnLock` 与三个子库的装配尚未落地（属 `M2DataCommon.pas`，不在本车道 6 单元任务书内）。
4. **`TStorageDB`**（`M2DataCommon.pas:17-50`，被 `TStorageDBClass` 引用）：未移植。
5. **`ShortString` 家族**：本层自建 `ShortString13`（`string[ACTOR_NAME_LEN]` = 13 字节）；`GXX.Core` 已有 `ShortStringBuf`/`ShortStr` 但语义不同（不截断），未强行统一。建议后续在 `GXX.Core` 收口一套 `string[N]` 值类型。
6. **`DoRun` 的宿主能力**：`UserEngine.GetStdItem` / `g_ItemRules.Get` / `g_M2DataDB.UserShopDB.UpdateItem` / `AddGameDataLog` 全部是接缝，`DoRun` 在默认装配下不产生任何副作用（仅消费语句与列表）。接入宿主后需补"到时物品落库 + 日志"的端到端用例。

---

## 8. 工具与可复现性

生成物（`SqlStatements.*.cs` / `DbLayerFingerprints.*.cs`）**一律由脚本从 GBK 原文生成**，脚本在工作树 `_recon/`（**不提交**）。复现步骤：

```powershell
# 1) 抽取（Node，GBK 解码 + Delphi 词法 + '+' 合并 + const 展开）
node _recon/delphi-sql-extract.mjs <src>/SqliteUserShopDB.pas _recon/ref-SqliteUserShopDB.json
node _recon/delphi-sql-extract.mjs <src>/MySqlUserShopDB.pas  _recon/ref-MySqlUserShopDB.json
# 2) 生成常量表（语句 label 映射见 _recon/labels.mjs，来自原文 DoInit）
node _recon/gen-statements.mjs   _recon/ref-<unit>.json SqliteUserShopStatements SqliteUserShopDbSql <unit> <out.cs>
# 3) 生成指纹表
node _recon/gen-fingerprints.mjs _recon/ref-<unit>.json SqliteUserShopDbFingerprints <unit> <out.cs>
# 4) 方言对账
node _recon/diff-sql.mjs _recon/ref-SqliteUserShopDB.json _recon/ref-MySqlUserShopDB.json
```

抽取口径的两个坑（已修，记录备查）：
- Delphi 用 `''` 表示字符串内的单引号 —— 朴素词法会把 `''` 当成"空字符串字面量"从而**截断整条 SQL**（本次踩到，SQL 长度 541 被截成 185）。
- powershell `.ps1` 以 ANSI 解析 → 脚本一律纯 ASCII；复杂词法改用 Node（见 `docs/并行派发台账.md` §8.1）。

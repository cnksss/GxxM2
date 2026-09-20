# p3-m2-dbdata 车道报告（M2DataDB / AuctionDB 四份实现）

- 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p3-m2-dbdata`（分支 `par/p3-m2-dbdata`）
- 目标单元：`Source/M2Engine/` 的 `MySqlM2DataDB.pas` / `SqliteM2DataDB.pas` / `MySqlAuctionDB.pas` / `SqliteAuctionDB.pas`
- 独占区：`GXX.CSharp/src/GXX.M2Server/DbLayer/**`、`tests/GXX.M2Server.Tests/DbLayerM2Data*`、`DbLayerAuction*`、本文件

---

## 0. 全部 commit（时间序）

| # | commit | 内容 |
|---|---|---|
| 0 | `f6c7eb74` | WIP：把上一轮遗留的 10 个文件**原样固化**（未验证，先保命） |
| 1 | `0152c551` | 修 `p3-gen.mjs` 重复 `RuntimeValue` 常量（CS0102）+ 补 `SqliteCreateTableSql.cs` / 接缝，整树可编译 |
| 2 | `2503ed86` | 四单元实现全部落地 + 4 张 SQL 指纹表 + 保真/行为测试（src 0 error） |
| 3 | `e5cf302d` | 修 Delphi `''` 解码（SQL 逐字保真核心缺陷）+ `LoadItemFromDB` ref 化 + `AuctionAddDateTime` 崩溃修复 |
| 4 | `70fc83f2` | `DbLayerAuctionBehaviorSqliteTests.cs`（227 例）+ 报告，门禁全绿 |
| 5 | `0afb497a` | 清理 `_recon` 派生中间产物（2.17MB→67KB，仅留 `.mjs` 流水线）+ 报告收尾 |
| 6 | `642d930c` | **跨车道修复**：UserShop SQL 的 Delphi `''` 引号缺陷（**已合入 main 的活性缺陷**） |
| 7 | `534945b3` | P2 核实 `TStorageDB` 真实状态（报告登记）+ P3 `IMySqlStatement.OrderBindParamDouble` 按原文名 |
| 8 | `209a4e4c` | P4：`TMySqlAuctionDB` 行为测试 261 例 + 双方言对账 + **修掉 6 个动态 SQL 拼接缺陷** + 逐字尾部守卫 |

> 说明：`0afb497a`（切片5）已被调度方并入 `main`；此后 main 的 tip 为 `0e68afe7`。
> 本分支在 `par/p3-m2-dbdata` 上继续叠加切片 6-8。

---

## 0.5 ★ 第二轮（调度方追加指令）的三项结论

### 0.5.1 P1 跨车道修复：UserShop SQL 的引号缺陷（`642d930c`）

**这是已合入 main 的活性缺陷，不只是形态问题**：Delphi 字符串字面量里的两个连续单引号是**一个**
单引号的转义、编译期即折叠，运行期 SQL 是 `ifnull(BuyerName, '')`（空串，`length = 0`）；
而托管常量保留了源码形态（四个连续单引号），SQL 解析成**一个单字符的串**，`length` 恒为 1
⇒ 上架 / 购买 / 取货 / 上架超时**整族判定恒假**。

**处置（全程脚本，零手工改字符串）**：

1. **可复现性校验**：先用**未修改**的 `_recon/delphi-sql-extract.mjs` + `gen-statements.mjs` 重跑，
   输出与已提交的 `SqlStatements.{Sqlite,MySql}UserShopDB.cs` **逐字节相同**
   （22,636 / 23,151 字节，`-ceq` True）⇒ 证明"流水线可复现已提交产物"。
2. 再把 `delphi-sql-extract.mjs` 的引号解码改为折叠成单个 `'`，重跑同一流水线。
3. **机器验证"只改了引号层数"**：去掉所有单引号后，新旧常量 `==` True。
4. 修正后：`ifnull(BuyerName, '')` 出现 27 / 28 次，`strftime('%s', 'now')` 2 次（仅 SQLite；
   MySQL 用 `CURRENT_TIMESTAMP`，为 0 是正确的）。
5. **额外发现并一并修掉**：`Sqlite/MySqlUserShopDB.cs` 里还有 **35 处手写内联 SQL**
   （不是引用常量类），同样带错引号 —— 含 `UpdateShopItem` / `BuyShopItem` / `GetMoneyShopItem` /
   `GetSelledAndNoGetMoneyTotal` / `GetUserShopInfo` 的子查询，以及 **20 处 `sm.Sql = sm.Sql + " and …"`
   动态拼接片段**。同属"禁止手工转录 SQL"的问题，已机械订正。
6. `DbLayerSqlFidelityTests.cs`：把锁定旧错文本的 3 处期望改为正确形态，并**新增回归守卫**：
   任何语句都不得含四个连续单引号或 `''%s''`；正确形态 `ifnull(BuyerName, '')` 必须出现。
7. **保留的原文差异**：`SqliteUserShopDB.pas:315` 原文写的是 `strftime("%s", "now")`（**双引号**，
   不是单引号）—— 该断言本来就正确，**没有改**。

> 另一条可复用的教训：**"某文件是从脚本生成的"不等于"用脚本重生成就一定安全"**。
> 我第一次尝试用本车道的 `p3-sql.mjs` 重生成 UserShop，结果留下了
> `@@SGetShopItemQueryField@@` / `@@SGetShopItemWhere@@` 模板占位（`p3-sql.mjs` 不展开方法级
> `const`，而 p2b 的 `delphi-sql-extract.mjs` 会展开）。**跨车道重生成必须先把原流水线跑成
> "与已提交产物逐字节相同"，再施加改动** —— 这就是第 1 步存在的理由。

### 0.5.2 P2 `GetStorageDBClass()` **无法接通**（证据如下）

调度方的指示是"`TStorageDB` 家族在 main 上、你的基线落后、吸收后接通"。实测**前提不成立**：

| 核查项 | 命令 | 结果 |
|---|---|---|
| J195/J196 的产物是否在本分支 | `git cat-file -e HEAD:GXX.CSharp/src/GXX.M2Server/SqliteStorageDbCore.cs` | **True**（早就在） |
| 是否来自 J195 | `git log HEAD -- .../SqliteStorageDbCore.cs` | `5c0b6c6e`（批次J195） |
| 与 main 是否一致 | `git checkout main -- <4 文件>` 后 `git status` / `git diff HEAD main` | **干净 / 为空**（逐字节相同，无需吸收） |
| 是不是 `TStorageDB` 派生类 | `git grep -E "class T\w*StorageDB"` | **0 命中** |
| main 上有没有该类型 | `git grep -E "\bT(StorageDB\|SqliteStorageDB\|MySqlStorageDB)\b" main -- GXX.CSharp/src` | 只有 **7 处注释**文字 |

J195/J196 的实际形状是 **`public static class SqliteStorageDbCore` / `MySqlStorageDbCore`**
（namespace `GXX.M2Server`，**不在 `DbLayer/`**），即**静态辅助类**（`Lines` / `SaveStart` / `DiffLines`
这类行号/差异常量 + 静态方法），**不是** `TStorageDB` 的单元类。
`DbBases.cs` 里只有 `TUserShopDB` / `TAuctionDB` 两个基类，**没有 `TStorageDB`**。

**结论**：`GetStorageDBClass(): Type` 的语义是 `TStorageDBClass = class of TStorageDB`（要能 `Create`
出对象并调 `DoInit/DoFinal/DoLoadStorageItems/...`），当前**没有真实类型可接**；
`FStorageDB.Init/Final` 也没有对象可调。按台账 §12.8「只有在正式归属文件缺失时才允许造接缝、
且禁止造第二份」，**本车道没有自造 `TSqliteStorageDB`**，留给已登记的
「`TM2DataDB` 基类 + `TStorageDB` 单元类」批次。
J195/J196 的 37 条测试在本分支**全绿**，未改动。

### 0.5.3 P3 命名统一：两个方言的名字**本来就不同**

调度方指示"按原文名统一为 `OrderBindParamDouble`"。核原文后发现需要细分：

| 方言 | 原文调用 | 出处 |
|---|---|---|
| MySQL | `OrderBindParamDouble` | `MySqlM2DataDB.pas:1204` |
| SQLite | `OrderBindDouble` | `SqliteM2DataDB.pas:1530` |

故只把 **`IMySqlStatement`** 的成员改名为 `OrderBindParamDouble`
（`DbSeam.cs` + `ScriptedMySqlStatement` + `MySqlM2DataDB.cs` 调用点与注释），
**`ISqliteStatement.OrderBindDouble` 保持原文名不动** —— 强行统一反而偏离原文，
这正是本车道一贯的"方言差异不抹平"原则。未留别名。

### 0.5.4 P4 发现的 **6 个动态 SQL 拼接缺陷**（已全部修复 + 补守卫测试，`209a4e4c`）

**根因（值得写进台账的机制性教训）**：`p3-gen` 把 `sm.Sql := sm.Sql + '...'` 渲染成
**累积快照**常量 —— `_L<行>_sm_Sql_P<n>` 的字面段**既含本步新增的文本，也含上一状态尾部的文本**
（例如基串尾的 `") "`、或上一子句的收尾 `")"`）。因此"追加式"实现必须去掉**与上一状态重叠的那一段**；
直接用 `Substring(1)` 也可能错（重叠长度是 2 时要 `Substring(2)`）。

| # | 位置 | 现象 | 原文证据 | 影响面 |
|---|---|---|---|---|
| **D1** | `MySqlAuctionDB.cs:832`（MySQL 独有） | itemGroup 分支把**拼接点占位常量** `"@@IntToStr(ItemGroup)@@"` 当字面量拼进 SQL | `MySqlAuctionDB.pas:765` 是 `IntToStr(Integer(ItemGroup))` | itemGroup ≠ igAll 时 SQL **非法** |
| **D2** | `Sqlite/MySqlAuctionDB.cs` itemColors 分支 | 用 `_P4`（以 `)` 开头）未去重叠 ⇒ **多一个右括号** | `SqliteAuctionDB.pas:802` / `MySql:802` 只追加 `' and ItemColor in ('` | 两方言 |
| **D3** | 同上 moneyType 分支 | 用 `_P6`（以 `)` 开头）⇒ 多一个右括号 | `:808` 只追加 `' and A.CurrencyType = '` | 两方言 |
| **D4** | `DoGetAllItemsPageCount` moneyType 分支 | 用 `_P4`（以 `)` 开头）⇒ 多一个右括号 | `Sqlite:1130` / `MySql:1130` 只追加 `' and CurrencyType = '` | 两方言 |
| **D5** | `MySqlAuctionDB.cs:1190`（MySQL 独有，父 agent 追加发现） | itemGroup 分支直接拼**整条基串快照** `L1087_sm_Sql_P0` ⇒ **select 基串被拼两次** | `MySqlAuctionDB.pas:1087` 只追加 `' and ItemGroup = '` | itemGroup ≠ igAll 时 SQL 非法 |
| **D6** | `SqliteAuctionDB.cs:1137` | 同处**手写**了 SQL 字面量（违反"禁止手工转录 SQL"） | 同上 | 改为从快照去重叠段取 |

**修法统一**：`P<n>.Substring(<上一状态尾部常量的长度>)`，并在每处注释里写明重叠来源。
D1 的第一次修复曾用 `Substring(1)`，但 itemGroup 站点的重叠是**基串尾 `") "`（2 字符）**，
`Substring(1)` 会多留一个空格 —— 已在第二轮修正为 `Substring(L775_sm_Sql_P2.Length)`。

**新增守卫测试**（此前只做 `Contains` 弱断言，**多一个 `)` 照样通过** —— 这是本车道补的最后一课）：

1. 4 条**手写期望串**测试：两方言 ×（`DoQueryAllItems` / `DoGetAllItemsPageCount`）在全过滤条件下，
   断言"基串之后的整段尾部"**逐字相等（含空格数量）**；期望串字面量直接抄自原文 `.pas` 各行
   （`SqliteAuctionDB.pas:781/785/802/808/813/818/843`、`MySqlAuctionDB.pas:754/765/802/808/813/818/823`）。
2. 1 条兜底守卫：动态拼出的 SQL 不得含 `@@`，也不得含 `") )"`。
3. 1 条 itemGroup-only 用例：四种组合下**基串只出现一次**、子句只追加一次。
4. 修正 1 条**锁定了错误文本**的旧断言（`") and ItemColor in ("` → `")  and ItemColor in ("`）。
5. 恢复子 agent 因缺陷停用的 5 条用例，并新增 D5 的回归位。

### 0.5.5 P4 其余结论

- **`High(LongWord)` 钳位分支两方言都不可达**（有价值的"死代码"证明）：
  `nValue = (long)m_n* + Prices`，两侧都是 `Integer`，最大 `2 × 2147483647 = 4294967294`，
  恰好比 `High(LongWord) = 4294967295` **小 1** ⇒ 钳位永不命中；真正发生的是 `Int64 → Integer` 截断
  （结果 `-2`）。已用 SQLite/MySQL 各 1 例锁死。
- **`DoQueryMyAttentionItems` 多行交错**：Fake 没有跨语句的全局调用日志，故用
  「计数 + `Reads` 列号序列 + `LastBinds`」三者联合锁死"内层 `Reset` 在每次外层 `Step` 之前"的顺序。
- **双方言对账测试组**（14 例）：同一输入分别驱动两方言，断言 13/13 记录字段、绑定序列
  （`Kind[]`/`Text[]`）、错误路径返回值与日志前缀一致；`AddDateTime` 单列断言"来源不同、结果相等"。

---

## 1. ★ 遗留产出甄别结论（10 个文件各自处置）

上一轮被宿主杀死时留在磁盘上的 10 个文件，**先原样提交（`f6c7eb74`）再迭代**。逐个判定：

| 文件 | 甄别结论 | 处置 |
|---|---|---|
| `M2DataDbSupport.cs` | **保留+补齐**。359 行的三块接缝（`M2ItemDbAccess` / `IAuctionPlayer` / `AuctionDbRunSeam`）设计正确、注释齐全 | 只补 `IAuctionStdItem.Name` / `.NeedIdentify`（原文实际用到，接缝缺） |
| `SqliteM2DataDB.cs` | **保留+局部重写**。1570 行，`DoInit`/`UpdateDB_1..6`/`DoUpdate`/`DoLoadItemsFromDB`/`DoLoadItemFromDB`/`DoSaveItemToDB` 已 1:1 且质量高 | 改 4 处：`_Prefix/_Suffix`→生成名 `_P0/_P2`；`LoadItemFromDB` 按值→`ref`；三个包装层补 `try/except MainOutMessage`；`IsInitOK` 从 `DoInit` 移到 `Init`；补 `Owner` 属性 |
| `SqlStatements.MySqlM2DataDB.cs` | **保留**（20 条语句，机械抽取，零手工） | 仅因 `''` 解码修复而重生成（实际无变化） |
| `SqlStatements.MySqlM2DataDB.Scripts.cs` | **保留**（2 段脚本） | 重生成（探针 `''m2data_version''`→`'m2data_version'`） |
| `SqlStatements.SqliteM2DataDB.cs` | **保留** | 无变化 |
| `SqlStatements.SqliteM2DataDB.Scripts.cs` | **保留**（17 段） | 重生成（3 段 `strftime` 引号） |
| `SqlStatements.MySqlAuctionDB.cs` | **保留**（21 条） | 重生成（3 条 `ifnull(LastBidder, ''''）`） |
| `SqlStatements.MySqlAuctionDB.Scripts.cs` | **保留**（16 段） | 修重复常量 + 重生成 |
| `SqlStatements.SqliteAuctionDB.cs` | **保留**（21 条） | 重生成（同上 3 条） |
| `SqlStatements.SqliteAuctionDB.Scripts.cs` | **保留**（16 段） | 修重复常量 + 重生成 |

**关键缺口（已补）**：这批产出里**一个测试文件都没有**，且**缺三份实现**（`MySqlM2DataDB.cs` / `SqliteAuctionDB.cs` / `MySqlAuctionDB.cs`）与 `SqliteCreateTableSql.cs`；`SqliteM2DataDB.cs` 还引用了 5 个不存在的成员（`SqliteCreateTableSql`、`DbLayerRunSeam.GetStdItemName`、`ISqliteStatement.OrderBindDouble`、`TSqliteAuctionDB`、`SqliteM2DataDbScripts.DoUpdate_L*_S_Prefix/_Suffix`）⇒ 上一轮的产出**根本编译不过**。

**结论**：**在其基础上继续，未推倒重来**。骨架质量高，缺的是"最后一公里"（三份姊妹实现 + 接缝补齐 + 测试）。

---

## 2. 交付物清单

### 2.1 新增源码

| 文件 | 行数 | 说明 |
|---|---|---|
| `DbLayer/MySqlM2DataDB.cs` | 1327 | `TMySqlM2DataDB`：Create/Destroy/Get*DBClass/UpdaeDB_1..5/DoInit/DoFinal/DoLoadItemsFromDB/DoLoadItemFromDB/DoSaveItemToDB/GetDataBase/OnRequest/Run + 基类三包装 + Init/Final |
| `DbLayer/SqliteAuctionDB.cs` | 1752 | `TSqliteAuctionDB`：24 个成员 |
| `DbLayer/MySqlAuctionDB.cs` | 1736 | `TMySqlAuctionDB`：22 个成员 |
| `DbLayer/SqliteCreateTableSql.cs` | 23 | `SqliteCreateTableSql.pas` 本车道用到的 3 个常量（`SQLITE_DBVERSION` / `SQLITE_M2DBVERSION` / `SQLITE_CREATE_M2DATA_TABLES`） |

### 2.2 新增测试

| 文件 | 行数 | 内容 |
|---|---|---|
| `tests/.../DbLayerFingerprints.SqliteM2DataDB.cs` | 36 | 20 条语句 SHA-256 |
| `tests/.../DbLayerFingerprints.MySqlM2DataDB.cs` | 37 | 20 条 |
| `tests/.../DbLayerFingerprints.SqliteAuctionDB.cs` | 37 | 21 条 |
| `tests/.../DbLayerFingerprints.MySqlAuctionDB.cs` | 37 | 21 条 |
| `tests/.../DbLayerM2DataSqlFidelityTests.cs` | 449 | 指纹 + 语句顺序 + 方言差异面 + 迁移脚本回读 + 版本分支 + DoFinal 缺陷 |
| `tests/.../DbLayerM2DataBehaviorTests.cs` | 622 | M2DataDB 行为（每公开方法 ≥3 例） |
| `tests/.../DbLayerAuctionSqlFidelityTests.cs` | 488 | AuctionDB 指纹 + 方言差异面 + 公开包装层 |
| `tests/.../DbLayerAuctionBehaviorSqliteTests.cs` | 2577 | `TSqliteAuctionDB` 行为（22 个 public 方法 × ≥3 例） |

### 2.3 修改的既有文件（都在 `DbLayer/**` 独占区内）

- `DbSeam.cs`：`ISqliteStatement` 补 `OrderBindDouble`；`IMySqlDatabase` 补 `ClearStatements`；`IM2DataDb.LoadItemFromDB` 与 `IDbLayerHost.LoadItemFromDB` 改 `ref TUserItem`
- `DbLayerSeams.cs`：`DbLayerRunSeam` 补 `GetStdItemName`
- `SqliteM2DataDB.cs`：见 §1
- `MySqlUserShopDB.cs` / `SqliteUserShopDB.cs`：`Owner.LoadItemFromDB(ref …, …)`（各 2 处，配合 ref 化）
- `tests/.../DbLayerTestKit.cs`：`ScriptedSqliteStatement.OrderBindDouble`、`FakeMySqlDatabase.ClearStatements`、`FakeDbLayerHost.LoadItemFromDB(ref …)`

---

## 3. ★ SQL 抽取 / 回读比对（本车道最重要的保真要求）

**流水线（全程脚本，零手工转录）**：

```
Source/M2Engine/*.pas  (GBK)
  └─ _recon/gbk2utf8.mjs        → _recon/<unit>.utf8.txt        （行号与原 .pas 一致）
      └─ _recon/p3-sql.mjs      → _recon/p3-<unit>.json         （逐字 SQL + 拼接点 + 运行时值名）
          ├─ _recon/p3-gen.mjs          → SqlStatements.<unit>.cs / .Scripts.cs
          └─ _recon/p3-fingerprints.mjs → tests/DbLayerFingerprints.<Unit>.cs   （SHA-256）
      _recon/p3-consts.mjs      → SqliteCreateTableSql.cs（3 个 const）
```

**两条独立路径**：指纹的 SHA-256 直接取自 `p3-<unit>.json`（GBK 原文抽取结果），**不经过 C# 常量生成步骤**；测试断言"实现真正绑到 `FDB` 的 SQL"的 SHA-256 等于该值。任何一侧被手改都会红。

**结果（全绿）**：

| 单元 | 语句数 | 指纹条数 | 结果 |
|---|---|---|---|
| SqliteM2DataDB | 20 | 20 | SHA-256 全等 |
| MySqlM2DataDB | 20 | 20 | SHA-256 全等 |
| SqliteAuctionDB | 21 | 21 | SHA-256 全等 |
| MySqlAuctionDB | 21 | 21 | SHA-256 全等 |

**回读比对（方法级脚本）**：`*_Scripts.cs` 的分片常量 + 运行时值在实现里被逐点接回，测试用"实现真正 `Execute` 出去的字符串 == 分片拼接结果"来锁：
- SqliteM2DataDB：6 段动态迁移脚本（`DoUpdate_L507/L569/L630/L674/L719` 的 `P0 + IntToStr(g_Config.nAuctionCurrencyType) + P2`，以及 `L761` 静态脚本）逐版本断言；
- MySqlM2DataDB：`UpdaeDB_5_L198_S`（DDL）、`DoInit_L250_sm_Sql`（探针）；
- AuctionDB：`DoDeleteAuctionItem` 的 23 项拼接（`sWhere`×9 + `IntToStr(AuctionID)`×2 = `RuntimeValueCount 11`）、`DoQueryAllItems` 7 个动态站点、`DoGetAllItemsPageCount` 7 个站点。

**方言差异面（"看起来一样实则不同"的差异断言）**：
- M2DataDB：两方言 20 条**恰好 2 条**不同（`InsertItemProperty` / `SelectItemProperty` 的 `HintModule` vs `Hintmodule`——原文大小写笔误），其余 18 条逐字相同。
- AuctionDB：两方言 21 条**恰好 8 条**不同，全部是时间写法：
  SQLite `strftime("%s","now")` / `(AddDateTime + AuctionTime * 3600)`
  vs MySQL `CURRENT_TIMESTAMP` / `TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(…, interval AuctionTime hour))`；
  其余 13 条逐字相同。

---

## 4. ★ 发现并修复的**原文/抽取/移植**缺陷

### 4.1 抽取器把 Delphi `''` 保留成了源码形态（**语义级**，影响 SQL 正确性）

`_recon/p3-sql.mjs`（及 `p3-consts.mjs`）旧版在字符串字面量里遇到 `''` 时 `buf += "''"`，即**保留源码形态**。
但 Delphi 的 `''` 是**一个单引号的转义**，编译期就折叠成 `'`。后果：送给 driver 的 SQL **多一层引号**，语义改变。

```
原文 MySqlM2DataDB.pas:250  'select ConstValue from db_constant where ConstName = ''m2data_version'';'
  真·运行期 SQL             select ConstValue from db_constant where ConstName = 'm2data_version';
  旧生成常量（错）          select ConstValue from db_constant where ConstName = ''m2data_version'';

原文 SqliteAuctionDB.pas:206  ... (length(ifnull(LastBidder, '''')) = 0) ...
  真·运行期 SQL               ... (length(ifnull(LastBidder, '')) = 0) ...
  旧生成常量（错）            ... (length(ifnull(LastBidder, '''')) = 0) ...
      → SQL 里 '''''''' 解析出来是**一个单字符的串**，length 恒为 1，条件恒假 ⇒ 到期结算路径完全失效
```

**修复**：`p3-sql.mjs` / `p3-consts.mjs` 改为 `buf += "'"`；重生成 4 个单元的 statements/scripts + 指纹 + `SqliteCreateTableSql.cs`。
受影响并已订正的值共 **10 处**：`MySqlM2DataDB` 探针 1、`SqliteM2DataDB` 脚本 3、`SqliteAuctionDB` 语句 3、`MySqlAuctionDB` 语句 3，另加 `SQLITE_CREATE_M2DATA_TABLES` 内的 `strftime`。
（工作树内**零手工编辑**：全部由脚本重生成。）

> ⚠ **跨车道未闭合（需调度方裁定）**：p2b 车道的 `SqlStatements.SqliteUserShopDB.cs` / `.MySqlUserShopDB.cs`
> **仍带同一缺陷（各 29 / 30 行含 `''`）**，例如
> `UserShop_UpdateShopItem = "update UserShopItem set CreateDate = (strftime(''%s'', ''now'')), … where length(ifnull(BuyerName, '''')) = 0 …"`
> —— 应为 `strftime('%s', 'now')` / `ifnull(BuyerName, '')`。这是**语义级**缺陷（该 UPDATE 的 WHERE 会恒假）。
> 修它需要同时改 `DbLayerSqlFidelityTests.cs`（该车道文件，**不在本车道独占区**，且其断言 `DbLayerSqlFidelityTests.cs:129-134` 把错形态写成了期望），
> 故本车道**只报告、不越区修改**，请调度方按台账 §12.8 的流程授权原车道订正。

### 4.2 `DateTime.FromOADate(Unix 秒)` 必抛 ⇒ `DoQueryAllItems` 对任何真实数据返回 0 条

`SqliteAuctionDB.cs` 初版把 `AuctionAddDateTime(int)` 落为 `DateTime.FromOADate(raw)`。
但 `AuctionData.AddDateTime` 在 SQLite 建表里是 `INTEGER DEFAULT (strftime('%s','now'))`（Unix 秒，如 1600000000），
而 `FromOADate` 的上限约 2,958,465 ⇒ **必抛 `ArgumentOutOfRangeException`**，被外层 `catch { MainOutMessage }` 吞掉 → 结果集一行都读不到。
**修复**：改 `DelphiDateUtil.UnixToDateTime(raw)`，并在 XML 注释里写明这是**必要的托管侧偏离**（原文 `TDateTime := Integer` 把 Unix 秒当 Delphi 日期序列号，托管 `DateTime` 无法表示；按该列真实语义还原后，SQLite/MySQL 两方言对同一逻辑字段给出一致的值）。

### 4.3 接缝按值传 struct ⇒ 宿主读回的物品被丢弃

原文 `TM2DataDB.LoadItemFromDB(UserItem: PTUserItem; …)` 是**指针**，调用点写 `@ShopItem.UserItem` / `@AuctionRecord.ActionItem`。
初版托管接缝按值传 `TUserItem`（struct）⇒ `FakeDbLayerHost.LoadedByParent` 之类的写回**随栈拷贝丢失**，UserShop/AuctionDB 读到的物品恒为空。
**修复**：`IDbLayerHost.LoadItemFromDB` 与 `IM2DataDb.LoadItemFromDB` 均改 `ref TUserItem`，同步 12 处调用点。
`SaveItemToDB` 保持按值（原文指针只为避免大记录拷贝，`DoSaveItemToDB` 只读不改，语义等价——已在 `DbSeam.cs` 注释说明）。

### 4.4 生成器：同一拼接表达式重复出现时发出重名常量

`p3-gen.mjs` 对"每个拼接点发一个 `_RuntimeValue_<表达式>` 常量"，而 `DoDeleteAuctionItem` 里 `sWhere` 出现 9 次 ⇒ **CS0102 重复定义**（22 error，上一轮遗留产出的编译失败根因之一）。
**修复**：按 `sanitize(表达式)` 去重，每名只发一次（`RuntimeValueCount` 仍为 11，拼接时同名常量重复引用）。

### 4.5 原文自身缺陷（逐字保留 + 测试锁定，带 `文件:行`）

| # | 缺陷 | 位置 | 我们的处理 |
|---|---|---|---|
| 1 | `DoFinal` **不** Finalize `FStatementGetItems` / `_Sort`（其余 18 条都 Finalize） | `SqliteM2DataDB.pas:963-1073`、`MySqlM2DataDB.pas:628-739` | 保留；`*_DoFinal_DoesNotFinalizeTheTwoGetItemsStatements` 锁定 |
| 2 | Flute 读取绑定不一致：`DoLoadItemsFromDB` 绑字面量 `0`，`DoLoadItemFromDB` 绑 `ItemType` | Sqlite `1219` vs `1294+`；MySql `887` vs `1099` | 保留；`FluteBinding_DiffersBetweenLoadItemsAndLoadItem` 锁定差异 |
| 3 | `DoInit` **不设** `FIsInitOK`（由 `TM2DataDB.Init` 设） | `M2DataCommon.pas:1638` | 已照此：`IsInitOK` 只在 `Init()` 里置 true |
| 4 | 9 处 SQL **末尾缺分号** | `SqliteAuctionDB.pas:207/255/260/274/279/290/293/296/299` | 逐字保留（常量即原文） |
| 5 | `DoDeleteAuctionItem` 的 `Items` 段**多一个 `;`**（sWhere 自带 `;` 又拼 `sLineBreak`） | `SqliteAuctionDB.pas:638-639` | 保留 |
| 6 | `StdMode = 28` 已被第 3 分支（照明物）吃掉 ⇒ "马牌"分支是**死代码** | Sqlite `490-491` / `534-535`；MySql `508-509` | 保留并就地标注 |
| 7 | 用 `UserItem.btValue[13]`（**不是** `[0]`）判定改名物品 | Sqlite `568`；MySql `542` | 保留 |
| 8 | `if AuctionID > 0` **无 else** ⇒ `GetMaxAuctionID` 返回 0 时静默返回 0 | Sqlite `545-591`；MySql `519-565` | 保留 |
| 9 | `DoGetMyItemsPageCount` **无 except、只有 finally** | Sqlite `1185-1198`；MySql `1164-1177` | 保留 |
| 10 | `DoHumanRename` 的 3 个 `Reset` 在 `try` **之外**（异常时不复位） | Sqlite `1567-1569`；MySql `1547-1549` | 保留 |
| 11 | `DoRun` 里 `except end;` **空吞** NPC `GotoLable` 段全部异常 | Sqlite `1543-1544`；MySql `1524-1525` | 保留 |
| 12 | 玩家离线时 `GoldType` 无 else 却仍调 `HumanChangeGold` | Sqlite `1351-1364` | 保留（C# 用 `= default` 承载） |
| 13 | `DoSaveItemToDB` 8 个 `try/finally` 的**异常路径也 Reset** | Sqlite `1498-1701`；MySql `1171-1377` | 保留 |
| 14 | `DoLoadItemsFromDB` 外层语句循环**没有 Reset**（内层 7 条都有） | MySql `955-960` | 保留 |
| 15 | `UpdaeDB_1..5` 是**裸 `except RollBack`**、不打日志（SQLite 同位置打 `MainOutMessage`） | MySql `146/160/175/187/232` | 保留 |
| 16 | `Format(' WHERE ParentID = %d and ItemType = ' + …, [AuctionID])` 格式串含未转义 `%d` | MySql `606` | 按语义落为等价拼接（注释标明） |
| 17 | 表名 `ItemProperty.HintModule`(SQLite) vs `Hintmodule`(MySQL) 大小写不一致 | Sqlite `268`/`333`；MySql `418`/`612` | 逐字保留（方言差异断言锁定） |
| 18 | 5 个版本分支的迁移脚本集合不同、且 MySQL 与 SQLite 的版本号**毫无交集** | `SqliteM2DataDB.pas:503-869` vs `MySqlM2DataDB.pas:261-287` | 逐分支保留 + 参数化测试 |

---

## 5. 四单元逐方法判定表

### 5.1 `TSqliteM2DataDB`（源 1-1708）

| 方法 | 原文行 | 状态 | 接缝/备注 |
|---|---|---|---|
| `Create` / `Destroy` | 83-116 | 完成 | 驱动器由装配方注入 |
| `GetAuctionDBClass` / `GetUserShopDBClass` | 118-131 | 完成 | `typeof(TSqliteAuctionDB)` / `typeof(TSqliteUserShopDB)`（已从临时 `object` 改回） |
| `GetStorageDBClass` | 123-126 | **部分** | `typeof(object)`：`TSqliteStorageDB` 未移植（见 §7） |
| `DoInit` | 133-336 | 完成 | 文件/目录接缝 + 4 条 PRAGMA + DoUpdate + 20 条语句 |
| `UpdateDB_1..6` | 338-473 | 完成 | `_P0/_P2` 分片拼接 |
| `DoUpdate` | 475-961 | 完成 | 11 个版本分支 + 2 段 ItemDBName/ItemName 回填（`temp` 同名复用） |
| `DoFinal` | 963-1073 | 完成 | 含原文缺陷 #1 |
| `DoLoadItemsFromDB` | 1075-1292 | 完成 | 28 列 + 7 张子表 |
| `DoLoadItemFromDB` | 1294-1496 | 完成 | 27 列 + 8 张子表 |
| `DoSaveItemToDB` | 1498-1701 | 完成 | 8 段 try/finally |
| `GetDataBase` | 1703-1706 | 完成 | |
| 基类三包装 + `Init`/`Final` | `M2DataCommon.pas:1653/1665/1677/1634/1645` | 完成 | `ref` + try/except `[Exception] TM2DataDB:*` |
| `Run` | `M2DataCommon.pas:1699` | 完成 | 原文空体 |

### 5.2 `TMySqlM2DataDB`（源 ~1398）

同上一一对应（14 个成员 + 6 个 `UpdaeDB_N` 含硬编码 ALTER/REPLACE），另加原文独有的 `OnRequest`（1384）与 `Run`（1389，10 分钟保活 `Exec("select 1")`）。
`GetStorageDBClass` 同样是 `typeof(object)`（未移植）。

### 5.3 `TSqliteAuctionDB`（源 1-1603）/ `TMySqlAuctionDB`（源 1-1583）

22-24 个成员**全部完成**：`Create`/`Destroy`、`DoInit`（21 条语句 + Prepare）、`DoFinal`、`DoAddAuctionItem`、`DoCancelAuctionItem`、`DoRetrieveAuctionItem`、`DoDeleteAuctionItem`、`DoAddAttentionItem`、`DoDeleteAttentionItem`、`DoJoinItemBid`、`DoQueryAllItems`、`DoQueryMyItems`、`DoQueryMyAttentionItems`、`DoGetAuctionInfo`、`DoGetAuctionRecord`、`DoGetAllItemsPageCount`、`DoGetMyItemsPageCount`、`DoGetMyAttentionPageCount`、`DoGetMyAuctioningItemsCount`、`DoGetMySellFailItemsCount`、`DoGetMyBuyOKItemsCount`、`DoRun`（含内嵌 `IncPlayerGameMoney`）、`DoHumanRename`。
public 包装层（`Lock`/`try/except`/`finally UnLock` + `RunTick2` 节流，`M2DataCommon.pas:317-454`）在既有 `DbBases.cs:640-1039` 的 `abstract class TAuctionDB` 里，**未重写**。

### 5.4 `TSqliteAuctionDB` 逐 public 方法用例数（`DbLayerAuctionBehaviorSqliteTests`，227 例）

| 方法 | 用例 | 方法 | 用例 | 方法 | 用例 |
|---|---|---|---|---|---|
| `Init` | 6 | `GetAllItemsPageCount` | 13 | `AddAttentionItem` | 9 |
| `Final` | 4 | `GetMyItemsPageCount` | 8 | `DeleteAttentionItem` | 6 |
| `QueryAllItems` | 22 | `GetMyAttentionPageCount` | 6 | `JoinItemBid` | 9 |
| `QueryMyItems` | 8 | `GetMyAuctioningItemsCount` | 6 | `GetAuctionInfo` | 6 |
| `QueryMyAttentionItems` | 7 | `GetMySellFailItemsCount` | 5 | `GetAuctionRecord` | 7 |
| `AddAuctionItem` | 50 | `GetMyBuyOKItemsCount` | 5 | `HumanRename` | 7 |
| `CancelAuctionItem` | 7 | `RetrieveAuctionItem` | 5 | `Run` | 23 |
| `DeleteAuctionItem` | 8 | | | **合计** | **227** |

每个 public 方法 ≥4 例（最少 `Final` 4 例）；**无删除、无跳过**（无 `[Fact(Skip=…)]`）。
`[Fact]` 147 + `[Theory]` 15（80 条 `[InlineData]`）。

### 5.5 `TMySqlM2DataDB` 逐方法（源 ~1398 行）

| 方法 | 原文行 | 状态 |
|---|---|---|
| `Create` / `Destroy` | 74-119 | 完成（`FMySqlLib`/dll 句柄归装配方） |
| `GetAuctionDBClass` / `GetUserShopDBClass` | 121-134 | 完成 |
| `GetStorageDBClass` | 126-129 | **部分**（`typeof(object)`，`TMySqlStorageDB` 未移植） |
| `UpdaeDB_1..4` | 136-190 | 完成（**硬编码** SQL：6 条 ALTER + 4 条 REPLACE） |
| `UpdaeDB_5` | 192-235 | 完成（DDL 走 `UpdaeDB_5_L198_S`） |
| `DoInit` | 238-626 | 完成（Connect + `CLIENT_MULTI_STATEMENTS` → utf8 → 探针 + `ClearStatements` → 5 个版本分支 → 20 条注册+Prepare；**不建表、不设 IsInitOK**） |
| `DoFinal` | 628-738 | 完成（含原文缺陷 #1） |
| `DoLoadItemsFromDB` | 740-960 | 完成 |
| `DoLoadItemFromDB` | 962-1169 | 完成 |
| `DoSaveItemToDB` | 1171-1377 | 完成 |
| `GetDataBase` / `OnRequest` / `Run` | 1379/1384/1389 | 完成（`Run` = 10 分钟保活 `Exec("select 1")`，**无分号**，原文如此） |
| 基类三包装 + `Init`/`Final` | `M2DataCommon.pas:1653/1665/1677/1634/1645` | 完成 |

### 5.6 测试侧"期望写错"的主要修正（非实现缺陷，登记备查）

1. **动态语句必须在 `DoInit()` 之前预登记**：`Auction_QueryAllItems` / `Auction_GetAllItemsCount` 是方法内 `AddSQLStatement` 现建的；只有先建，Fake 的同名复用才会返回**同一实例**。
2. **`Auction_QueryOneItem` 没有 `AuctionID` 列**（首列是 `HumanName`，11 列）；`Auction_QueryAuctionItems` 才是 12 列、首列 `AuctionID`。
3. **`ScriptedSqliteStatement.Step()` 无预设行时直接返回 `DONE`**：要造 `SQLITE_ROW(100)` 必须**塞一行结果集**（只设 `StepCode` 无效）。
4. `MySqlM2DataDB` 的版本探针 `get_db_constant_value` 也必须**预登记**（`AddSQLStatement` 后 `AddRow`）。
5. SqliteM2DataDB 的 `DoInit` 会先 `Execute` 4 条 PRAGMA，故 `ExecutedSql[0..3]` 是 PRAGMA、迁移脚本是 `ExecutedSql[4]`。
6. `temp` 语句在 `DoUpdate` 871-957 里被 **Prepare+Finalize 各 4 次**（873-888 / 890-915 / 917-932 / 934-955）。
7. `DoDeleteAuctionItem` 的 Items 段**没有**双分号（`sWhere` 自带尾分号 + `sLineBreak` → 单分号 + CRLF；MySQL 同形）—— 子 agent 的初版判断有误，已按原文订正。
8. `AddDateTime`：SQLite 的 `AuctionData.AddDateTime` 是 Unix 秒，故结果集里要填**真实 Unix 秒**（如 `1600000000`）。

---

## 6. 接缝清单

| 接缝 | 承载原文 | 状态 |
|---|---|---|
| `IDbLayerHost`（Lock/UnLock/DataBase/LoadItemFromDB/SaveItemToDB） | `TM2DataDB` | 已有；本轮 `LoadItemFromDB` 改 `ref` |
| `ISqliteDatabase` / `ISqliteStatement` | `TSQLite3Database` / `TSQLStatement`（**源码树外**） | 已有；本轮补 `OrderBindDouble` |
| `IMySqlDatabase` / `IMySqlStatement` | `TMySqlDataBase` / `TMySqlStatement`（直连 `libmysql-32.dll`） | 已有；本轮补 `ClearStatements`（原文 `FDB.Statements.Clear`） |
| `IDbLayerEnvironment` / `DbLayerGlobals` | `g_Config.*` / `MainOutMessage` / `MyGetTickCount` | 已有 |
| `DbLayerRunSeam.GetStdItemName` | `UserEngine.GetStdItemName` | 本轮新增 |
| `AuctionDbRunSeam`（`GetPlayObject`/`GetStdItem`/`HumanChangeGold`/`GotoLable`/`AddGameDataLog`/`g_boGameLog*`） | `ObjPlayer` / `UsrEngn` / `DataEngn` / `M2Share` / `g_FunctionNPC` | 已有（`M2DataDbSupport.cs`） |
| `IAuctionPlayer` / `IAuctionStdItem` | `TPlayObject` / `TStdItem` 的最小面 | 已有；本轮为 `IAuctionStdItem` 补 `Name` / `NeedIdentify` |
| `M2ItemDbAccess` | `TUserItem` 的 fixed-buffer 下标访问 | 已有 |
| `DelphiDateUtil` | `DateUtils.UnixToDateTime/DateTimeToUnix` | 已有 |
| `SqliteM2DataDB` 文件系统接缝（`ExtractFilePath`/`DirectoryExists`/`ForceDirectories`/`FileExists`/`DeleteFile`/`SaveResourceToFile`/`Sqlite3ConfigMultithread`） | `SysUtils` / `TResourceStream` / `sqlite3_config` | 已有 |
| `MySqlM2DataDbSeam`（5 个连接参数全局 + `ResetDefaults`） | `g_sDataSaveDBServer/User/Password/DataBase` / `g_wDataSaveDBPort`（`M2Share.pas:5600-5604`） | 本轮新增（在 `MySqlM2DataDB.cs` 内，未另造第二套） |

### 需要调度方协调的事项

1. **★ p2b 车道的 `SqlStatements.{Sqlite,MySql}UserShopDB.cs` 仍带 §4.1 的引号缺陷**（各 29/30 行）。修它要同时改该车道的 `DbLayerSqlFidelityTests.cs`（不在本车道独占区）⇒ 请授权原车道订正。
2. **`TStorageDB` 家族未移植**（`SqliteStorageDB.pas` / `MySqlStorageDB.pas`）：4 处 `GetStorageDBClass()` 与 `Init`/`Final` 里的 `FStorageDB.Init/Final` 只能留注释（`SqliteM2DataDB`/`MySqlM2DataDB` 的 `GetStorageDBClass` 返回 `typeof(object)`）。
   注：任务书说 `MySqlStorageDB.pas`/`SqliteStorageDB.pas` 已由顺序会话移植完成 —— 但本工作树里**没有** `TSqliteStorageDB`/`TMySqlStorageDB` 类型（`grep` 全树无命中），故仍按未移植处理。
3. **`TM2DataDB` 基类未移植**：4 个 DB 单元与 `IM2DataDb` 目前是"一个类同时扮演 TM2DataDB 的 public 面 + 具体单元的 Do* 实现"的折叠形态。原文的 `AuctionDB`/`UserShopDB`/`StorageDB` 属性在原文里是**只读且由基类 Create 里 new 出来的**，托管侧退化为可赋值槽位。建议后续单独排一个"TM2DataDB 基类"批次统一。
4. **`IMySqlStatement.OrderBindParamDouble`**：原文 `MySqlM2DataDB.pas:1204` 调 `OrderBindParamDouble`，接缝里叫 `OrderBindDouble`。当前是本单元唯一的"命名不一致"（语义相同）。若要逐字，需给 `IMySqlStatement` 加别名。

---

## 7. 门禁与结果

```
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p3-m2-dbdata\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
```

### 7.1 build

- `dotnet build src\GXX.M2Server\GXX.M2Server.csproj -c Debug --nologo` → **Build succeeded. 0 Error(s)**（本车道 4 个单元 + 所有接缝改动）
- `dotnet build tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo` → **0 error CS**

### 7.2 test（基线对账）

| 口径 | 结果 |
|---|---|
| 基线（任务书）：`GXX.M2Server.Tests` | **5591 全绿** |
| 本车道改完后，**排除**本车道新增的 `DbLayerAuctionBehaviorSqliteTests` 跑全量 | **Passed: 5678 / Failed: 0**（1m03s） |
| 差值 | 5678 − 5591 = **87** = 本车道新增的 `DbLayerM2DataSqlFidelityTests`(26) + `DbLayerM2DataBehaviorTests`(30) + `DbLayerAuctionSqlFidelityTests`(31) |
| 新增 `DbLayerAuctionBehaviorSqliteTests` | **229 例**（22 个 public 方法 ≥3 例 + 2 条边角，见 §8） |
| 新增 `DbLayerAuctionBehaviorMySqlTests` | **261 例**（22 个 public 方法 ≥4 例 + 14 例双方言对账） |
| 全量合计（最终） | **6179 例**（`Failed: 0`） |

> **无基线漂移、无基线回归**：`5678 − 87 = 5591`，与任务书给的基线**逐例相符**。
> 特别地，p2b 车道的 41 条 `DbLayer*UserShop*` 用例在 `LoadItemFromDB` ref 化之后**仍然全绿**（已在 §4.3 说明）。

### 7.3 本车道各测试文件用例数（最终）

| 测试文件 | 用例数 |
|---|---|
| `DbLayerM2DataSqlFidelityTests` | 26 |
| `DbLayerM2DataBehaviorTests` | 30 |
| `DbLayerAuctionSqlFidelityTests` | 35（含 §0.5.4 的逐字尾部守卫） |
| `DbLayerAuctionBehaviorSqliteTests` | 229（227 + §0.5.5 的 2 条边角） |
| `DbLayerAuctionBehaviorMySqlTests` | **261**（22/22 public 方法 ≥4 例，含 14 例双方言对账） |
| 本车道合计 | **581** |

> p2b 车道的 `DbLayer*UserShop*` 用例（本车道修了引号缺陷）也在 `DbLayer` 过滤器内，一并全绿。

---

## 8. 实测记录（最终）

```
dotnet build GXX.slnx -c Debug --nologo
  → Build succeeded.  0 Error(s)

dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
  → Passed!  - Failed: 0, Passed: 6179, Skipped: 0, Total: 6179     (43 s)

dotnet test ... --filter "FullyQualifiedName~DbLayer"
  → Passed!  - Failed: 0, Passed: 623, Skipped: 0, Total: 623
```

> 环境备注：本轮 4 条 agent 并发跑 `dotnet build/test`，偶发 `MSB4166`/obj 文件锁；按台账 §12.9 单独复跑即全绿（本车道实测未出现 MSB4166）。

---

## 9. 未完成 / 剩余量（诚实）

1. **`TStorageDB` 家族**（`SqliteStorageDB.pas` / `MySqlStorageDB.pas`）的**单元类**未移植：
   4 个 `GetStorageDBClass()` 无类可返回（`typeof(object)`），`Init`/`Final` 里的 `FStorageDB.Init/Final`
   只有注释。**不在本车道四个目标单元内**。
   **状态 = 顺延（不是"未移植"）**：J195/J196 已把该子系统的**方法**以静态辅助类
   (`SqliteStorageDbCore` / `MySqlStorageDbCore`) 落地（本分支含其全部 37 条测试，全绿），
   缺的是 `TStorageDB` 单元类本身。调度方已裁定（§0.5.2）：**保持现状 + 登记**，
   与「`TM2DataDB` 基类家族」合并为一个批次，**本车道不自造第二份**。
2. **`TM2DataDB` 基类未移植**，故 `AuctionDB`/`UserShopDB`/`StorageDB` 三个属性是"可赋值槽位"而非原文的"基类 Create 里 new"。语义差异已在上文与代码注释中标明。调度方已登记为独立批次。
3. **`TMySqlAuctionDB` 的逐方法行为测试**（本车道曾自认的最大剩余量）：已在切片 8 补齐
   （`DbLayerAuctionBehaviorMySqlTests.cs`），并含**双方言双向对账**测试组 —— 见 §8 的最终计数。
4. **`TSqliteStorageDB` / `TMySqlStorageDB` 的 SQL 常量**未抽取（随 §9.1 的批次一起做）。
5. 抽取流水线脚本在 `_recon/`（`.git/info/exclude` 忽略，**未入库**）。按交接纪律已清理派生中间产物
   （`*.utf8.txt` / `p3-*.json` / `ref-*.json` 等，从 2.17 MB 降到 67 KB），**只保留可复用的 `.mjs` 流水线脚本**：
   `gbk2utf8.mjs` → `p3-sql.mjs` → `p3-gen.mjs` / `p3-fingerprints.mjs` / `p3-regen-all.mjs`，
   以及 `p3-consts.mjs` + `p3-gen-createtable.mjs`，再加**第二轮新增的跨车道修复工具**
   `delphi-sql-extract.mjs`（已修引号解码）+ `gen-statements.mjs` / `gen-fingerprints.mjs` + `labels.mjs`。
   中间产物全部可由 repo 内的 GBK 原文重新生成，故不影响可复核性；报告 §3 与 §0.5.1 记录了每步的输入/输出。
   注意：清理后为跨车道修复又临时重建了 `SqliteUserShopDB.utf8.txt` / `MySqlUserShopDB.utf8.txt` /
   `MySqlM2DataDB.utf8.txt` / `SqliteM2DataDB.utf8.txt` 与对应的 `p3-*.json` / `ref-*.json`；
   这些是**可再生成的中间产物**，会在最终清理时一并删除。

---

## 10. 建议推广的做法（调度方点名）

1. **SQL 常量必须脚本抽取 + 双路径指纹对账**：指纹的 SHA-256 直接取自"原文抽取结果"，
   **不经过 C# 常量生成步骤** ⇒ 抽取与实现是两条独立路径，任何一侧被手改都会红。
   本车道用这条链抓出了 4 个真实缺陷（重复常量 / 引号解码 / 35 处手写内联 SQL / 方言差异面漂移）。
2. **跨车道重生成前，先证明工具链能逐字节复现已提交产物**（§0.5.1 第 1 步）。
   否则"用脚本重生成"会把别的车道依赖的模板占位（如 `@@SGetShopItemQueryField@@`）打进去。
3. **"原文如此"要连方言 API 名一起照抄**：`OrderBindDouble`(SQLite) vs `OrderBindParamDouble`(MySQL)
   是原文驱动 API 的真实差异，强行统一反而偏离 1:1（§0.5.3）。
4. **★ 动态 SQL（`X := X + '...'`）的"累积快照常量"是陷阱**：生成器的 `_L<行>_P<n>` 字面段含
   **上一状态尾部**，追加时必须按**重叠长度**裁剪，不能一律 `Substring(1)`；
   更不能用含 `@@` 的占位分片、也不能用含整条基串的 `_P0` 快照（§0.5.4 的 D1/D5）。
   **守卫方式**：对"基串之后的整段尾部"做**逐字**断言（含空格数），而不是 `Contains`。
5. **不可达分支要证明它不可达**，而不是"构造不出就不测"：`2 × int.MaxValue = 4294967294 < 4294967295`
   ⇒ `> High(LongWord)` 钳位分支数学上不可达，真正发生的是 `Int64 → Integer` 截断（§0.5.5）。
6. **弱断言（`Contains`）会与错误实现"共谋"**：本车道的 `QueryAllItems` 旧断言在 SQL 多一个 `)` 时
   依然通过；一旦改成"逐字尾部对账"就立刻抓出 6 个缺陷。

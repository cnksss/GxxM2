# 并行报告 — `p9-m2-datalayer`

> 车道：`p9-m2-datalayer` ｜ 分支：`par/p9-m2-datalayer` ｜ 工作树：`.worktrees/p9-m2-datalayer`
> 基座：`main @ b48e4c43` ｜ 目标目录：`GXX.CSharp/src/GXX.M2Server/Sweep9/DataLayer/**`
> 独占分区（`tools/lane-zones.tsv:58`）：`Sweep9/DataLayer/**` · `tests/.../Sweep9DataLayer*` · 本文件

---

## 0. 一句话结论

| 单元 | 行数 | 已移植方法数 / 总方法数 | 状态 |
|---|---|---|---|
| `ItemEvent.pas` | 389 | **13 / 13** | ✅ 100% |
| `DataManage.pas` | 278 | **24 / 24** | ✅ 100% |
| `UserShopDB_Old.pas` | 1073 | **0 / 33** | ⛔ **不移植 + 证据**（死代码，见 §2） |

**合计**：37 / 70 方法已移植；另 33 个方法按派发任务书的明确授权登记为「不移植 + 证据」。
**新增测试 130 例全绿**（`dotnet test --filter FullyQualifiedName~Sweep9DataLayer` → 0 failed）。

---

## 1. 交付物清单

### 1.1 实现（`src/GXX.M2Server/Sweep9/DataLayer/`）

| 文件 | 内容 | 对应原文 |
|---|---|---|
| `Sweep9DataLayerSeams.cs` | ItemEvent 链的接缝层（`TGameObject` 的宿主能力、`g_Config` 两字段、`GetStdItem`、副本标志、`AddGameDataLog`、日志） | ItemEvent.pas 的 6 个 `uses` |
| `ItemEvent.cs` | `TGameObject`（ObjGame.pas 基类）+ `TItemObject`（19 字段 + 4 方法） | ItemEvent.pas:9-164 |
| `ItemEventManager.cs` | `TItemManager`（3 字段 + 7 方法 + `ItemCount` 属性） | ItemEvent.pas:38-55 / 166-386 |
| `DataManage.cs` | `DataManageConst` / `DataManageGlobals` / `DataManageAccessSeam` / `TAccessEngine` / `TAccessTable` | DataManage.pas 全单元 |

### 1.2 测试（`tests/GXX.M2Server.Tests/`）

| 文件 | 用例数 | 覆盖 |
|---|---|---|
| `Sweep9DataLayerTestKit.cs` | — | 内存替身：`FakeAdoConnection` / `FakeAdoQuery` / `FakeItemGameEnvir` / `FakeBaseObject` / `Sweep9FakeClock` |
| `Sweep9DataLayerItemEventTests.cs` | 37 | `TGameObject` 构造、`TItemObject` 构造/`Destroy`/`Run`/`MakeGhost` |
| `Sweep9DataLayerItemManagerTests.cs` | 43 | `TItemManager` 全部 8 项过程函数 + 4 个 `FindItem` 重载 |
| `Sweep9DataLayerDataManageTests.cs` | 50 | 4 个常量、`TAccessEngine` 11 方法、`TAccessTable` 14 方法/属性 |
| `Sweep9DataLayerMethodParityTests.cs` | 5 | **反射计数对账**（台账 §37.3）：逐类断言"原文过程函数数 == 托管公开成员数"；另一条反证 `UserShopDB_Old` 的 3 个类名在托管侧**零命中** |
| **合计** | **135** | — |

**门禁输出（实际，2026 本车道收尾运行）**

```
$ dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false
    169 个警告
    0 个错误
已用时间 00:00:14.88

$ dotnet test GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj -c Debug --nologo -m:1 -p:BuildInParallel=false
已通过! - 失败:     0，通过:   9617，已跳过:     0，总计:   9617，持续时间: 32 s - GXX.M2Server.Tests.dll (net8.0)
```

本车道专项过滤（135 例）：

```
$ dotnet test ... --filter "FullyQualifiedName~Sweep9DataLayer"
已通过! - 失败:     0，通过:    135，已跳过:     0，总计:    135，持续时间: 139 ms
```

> `M2Server.Tests` 单工程 **9,617** 例。台账 §38.1 记的 `18,576` 是**全解决方案**口径
> （含 `GXX.Core.Tests`/`GXX.Client.Tests`/`GXX.LoginSrv.Tests` 等）。
> 本车道按派发纪律**只跑自己的测试工程**。
> 本车道在 `M2Server.Tests` 里**新增 135 例**（130 + 5 条方法数对账），
> 全部在同一工程内，未改动任何既有用例。

---

## 2. `UserShopDB_Old.pas` 死代码判定与证据

### 2.1 判定：**死代码 ⇒ 不移植**

派发任务书对本单元给出**明确授权**：

> 「先判定它是不是死代码……若确证无人引用、且与新版 UserShopDB 同源，**登记为"不移植+证据"**（写进报告），不要硬搬」

下面 6 条独立判据全部成立，**无一例外**。所有 `git grep` 均在**主工作树只读**执行（未做任何 git 写操作）。

### 2.2 证据（计数取证，台账 §37.3）

| # | 判据 | 命令 | 结果 |
|---|---|---|---|
| E-1 | **全仓零引用**该单元名 | `git grep -c "UserShopDB_Old" -- "*.pas" "*.dpr" "*.dpk"` | **零命中**（exit 1） |
| E-2 | **未被任何 `.dpr` 编译** | `git grep -n "UserShopDB_Old in" -- "*.dpr"` | **零命中**（exit 1）。`M2Server.dpr:81/86` 用的是 `MySqlUserShopDB` / `SqliteUserShopDB` |
| E-3 | 四个自声明类型**全仓只在本文件出现** | `git grep -l <T> -- "*.pas"` | `TUserShopFile` 1 文件/14 行 · `TUserShopItemFile` 1/17 · `TQuickNameList` 1/15 · `TRecordCount` 1/9 —— **全部就是 `UserShopDB_Old.pas` 自己** |
| E-4 | **依赖的 `ACCOUNTLEN` / `ACTORNAMELEN` 常量在全仓无定义** | `git grep -n "ACCOUNTLEN\s*=\|ACTORNAMELEN\s*=" -- "Source/**/*.pas"` | **零命中**（exit 1）⇒ 该单元**在任何配置下都无法编译**（Delphi 会把 `string[ACCOUNTLEN]` 解析为过程类型 ⇒ E2029） |
| E-5 | 引用的全局 `g_UserShopDB` **只有本文件的两处读取、无声明** | `git grep -n "g_UserShopDB" -- "Source/**/*.pas"` | 只有 `UserShopDB_Old.pas:759/:810` 两处**读**，全仓**无声明** ⇒ 即便被引用也链接不过 |
| E-6 | 文件的 `unit` 名与新版**撞名** | 读原文头（`_analysis/utf8_mirror/M2Engine/UserShopDB_Old.pas:1`） | 写的是 `unit UserShopDB;` —— 而 `MySqlUserShopDB.pas:1` / `SqliteUserShopDB.pas:1` 各自声明 `unit MySqlUserShopDB` / `unit SqliteUserShopDB`，**同一个 `TUserShopDB` 家族的新旧两版**。文件名带 `_Old`，`unit` 名却还是 `UserShopDB` ⇒ 一旦加入 `uses` 就是**重名单元**，工程本身也不允许 |

### 2.3 「与新版同源」判定

原文（`UserShopDB_Old.pas`）声明 `TUserShopFile` / `TUserShopItemFile`，方法集为
`Add / DeleteShop / ResetUserShopName / UpDate / QueryCharName / QueryShopName / GetShop /
LoadShopList / AddSell / ShopToStorage / StorageToShop / SellItemCount / SelledItemCount /
StorageItemCount / DeleteStorage`（共 33 个过程函数）。
新版 `SqliteUserShopDB.pas:17` / `MySqlUserShopDB.pas:17` 是 `TSqliteUserShopDB = class(TUserShopDB)` /
`TMySqlUserShopDB = class(TUserShopDB)` —— 基类 `TUserShopDB` 提供同一族语义
（`HumanNameExists`/`ShopNameExists`/`InsertUserShop`/`UserShopRename`/`GetUserShopInfo`/
`InsertUserShopItem`/`UpdateUserShopItem`/`BuyUserShopItem`/`GetMoneyShopItem`/… **同一批 SQL 语句名**），
且 `TUserShopItem` 的字段（`boAllowSell`/`boGetMoney`/`btItemType`/`btMoneyType`/`nPrice`/
`dCreateDate`/`UserItem`/`sAccount`/`sShopName`/`sMasterName`/`sBuyName`/`UserShop`）
与新版建表语句（`SqliteCreateTableSql.cs`）逐字段对应。
⇒ **同源**：旧版是「本地 Jet/文件流」实现，新版是「SQLite/MySQL」实现。

### 2.4 本车道的处置

- **不移植、不建同名 `.cs`**（不写 `UserShopDB_Old.cs`，避免制造"已移植"的假证据）。
- 新版 `TUserShopDB` 面**已在 `main`**：`src/GXX.M2Server/DbLayer/{MySqlUserShopDB,SqliteUserShopDB}.cs`
  （由 `p3-m2-dbdata` 移植），本车道**不重复**。
- `tools/unit-map.tsv:84` 那行 `UserShopDB_Old	par/p9-m2-datalayer` 是**在飞归属**而非覆盖证据；
  本单元完工后该行应删除，删除动作属集成方（`tools/**` 在本车道禁改区）⇒ 见 §6.1 请求 #1。

---

## 3. 逐单元移植明细

### 3.1 `ItemEvent.pas`（13 / 13 方法，100%）

| 原文行 | 原文签名 | 托管落点 |
|---|---|---|
| 9-36 | `TItemObject = class(TGameObject)` | `ItemEvent.cs::TItemObject`（19 字段，名字逐字保留，含 `m_` 前缀） |
| — | **基类 `TGameObject`**（ObjGame.pas:9-17/51-62） | `ItemEvent.cs::TGameObject`（4 字段 + 构造）← 见 §4.1 D-P9-01 |
| 62-83 | `constructor TItemObject.Create(); override` | `TItemObject()`（18 条赋值，**顺序逐行保留**：`m_btColor:=255` 在 `m_dwRunTick` 之后） |
| 85-93 | `destructor TItemObject.Destroy; override` | `TItemObject.Destroy()`（显式方法；托管侧无确定性析构，测试直接调用） |
| 95-157 | `procedure TItemObject.Run();` | `TItemObject.Run()`（三段结构逐行） |
| 159-164 | `procedure TItemObject.MakeGhost;` | `TItemObject.MakeGhost()` |
| 38-55 | `TItemManager = class` | `ItemEventManager.cs::TItemManager`（3 字段） |
| 166-171 | `constructor TItemManager.Create();` | `TItemManager()`（原文**无** `inherited`，托管侧同样不写 `: base()`） |
| 173-189 | `destructor TItemManager.Destroy;` | `TItemManager.Destroy()` |
| 191-199 | `function GetItemCount: Integer` | `ItemCount` 属性（原文 `property ItemCount: Integer read GetItemCount`，:54） |
| 201-209 | `procedure AddItem` | `AddItem` |
| 211-230 | `function FindItem(Envir, ItemObject)` | `FindItem(object, TItemObject)` |
| 232-253 | `function FindItem(Envir, nX, nY)` | `FindItem(object, int, int)` |
| 255-276 | `function FindItem(Envir, nX, nY, ItemObject)` | `FindItem(object, int, int, TItemObject)` |
| 278-301 | `function FindItem(Envir, nX, nY, nRange, List): Integer` | `FindItem(object, int, int, int, TGList?)` |
| 303-386 | `procedure TItemManager.Run();` | `Run()`（三段结构 + 被注释掉的旧版循环逐字保留为注释） |

**虚分派（台账 §18.8）**：原文 `TGameObject.Create` 是 `virtual`（ObjGame.pas:15）⇒ 托管侧保留 `virtual`
（C# 构造不是虚成员，故以「基类构造 + 派生类构造链」表达同一语义，并由
`ItemObject_IsDerivedFromGameObject` 断言类层次真实存在）。
`TItemObject.Create`/`Destroy` 是 `override`；`Run`/`MakeGhost` 原文**不是** virtual ⇒ 托管侧**不加** `virtual`。

**`TGList` / `TList` 复用**：`m_ItemList`/`m_FreeItemList` 用既有
`GXX.Core.Protocol.SDK.TGList`（`SDK.cs:55`）；`FindItem` 的 `List: TList` 形参用同一个 `TGList`
（`TGList` 本就是 `TList` 派生，属**收窄到既有等价类型**，不另造）。

**原文把 `Lock`/`UnLock` 全部注释掉**（:193-198、:203-208、:216-229、:238-252、:261-275、:284-300、
:316-317、:345-347、:354-355、:371-372、:383-385）⇒ 托管侧**同样不加锁**（不"顺手补 Lock"）。

### 3.2 `DataManage.pas`（24 / 24 方法，100%）

| 原文行 | 原文签名 | 托管落点 |
|---|---|---|
| 8-30 | `TAccessEngine = class` | `DataManage.cs::TAccessEngine`（4 字段） |
| 74-113 | `constructor Create(const FileName: string)` | `TAccessEngine(string FileName)` |
| 115-128 | `destructor Destroy; override` | `Destroy()` |
| 130-133 | `procedure Lock` | `Lock()`（`Monitor.Enter`） |
| 135-138 | `procedure UnLock` | `UnLock()` |
| 140-147 | `function LoadTable(Table: string): Boolean` | `LoadTable` |
| 149-153 | `procedure CreateTable(Table, SQL: string)` | `CreateTable` |
| 155-172 | `function Connect: Boolean` | `Connect()` |
| 174-179 | `procedure DisConnect` | `DisConnect()` |
| 181-192 | `function GetTable(Table: string): TAccessTable` | `GetTable` |
| 194-205 | `procedure SetTable(Table: string; Value: TAccessTable)` | `SetTable` |
| 29 | `property Tables[Table: string]` | `this[string]` 索引器 |
| 28 | `// property Count: Integer read GetCount;` | **原文被注释掉 ⇒ 不实现**（逐字保留为注释） |
| 32-54 | `TAccessTable = class` | `DataManage.cs::TAccessTable`（1 字段） |
| 208-212 | `constructor Create(const Table: string)` | `TAccessTable(string Table)` |
| 214-217 | `destructor Destroy; override` | `Destroy()` |
| 220-222 | `function GetCount: Integer` | `Count` 属性 |
| 224-227 | `function GetADOQuery:TADOQuery` | `ADOQuery` 属性 |
| 228-231 | `procedure Lock` | `Lock()` |
| 233-236 | `procedure UnLock` | `UnLock()` |
| 238-241 | `procedure ClearSQL` | `ClearSQL()` |
| 243-246 | `procedure AddSQL(SQL: string)` | `AddSQL` |
| 248-251 | `procedure OpenSQL` | `OpenSQL()` |
| 253-256 | `procedure NextSQL` | `NextSQL()` |
| 258-261 | `procedure CloseSQL` | `CloseSQL()` |
| 263-266 | `procedure ExecSQL` | `ExecSQL()` |
| 268-271 | `function GetField(Field: string): TField` | `GetField` + `this[string]` 索引器 |
| 272-275 | `function GetParameters: TParameters` | `GetParameters` + `Parameters` 属性 |
| 55-73 | 4 个单元级 `const` | `DataManageConst`（`g_sShopItemTable` / `g_sSQLString` / `g_sShopItemField` **逐字符**保留，含原文的 `\t` 混排） |
| 59-61 | 2 个 unit-level `var` | `DataManageGlobals`（`DBQry` / `ADOConnection` **静态**；另加 `AccessEngine`） |

**ADO/Access 走接缝的理由（任务书第 4 条）**：本单元 100% 的行为都落在两个外部依赖里 ——
`ADODB` 的 `TADOConnection`/`TADOQuery`/`TField`/`TParameters`，以及 `Access` 的 5 个自由函数。
实测托管侧对二者**零命中**（`git grep -n "TADOQuery" -- 'GXX.CSharp/src/**'` 与
`TADOConnection`/`TParameters`/`CreateAccessDB`/`TableExists`/`AccessCreateTable`/`CreateAccessIndex`/
`GetTableList` 全部 0 命中），且 Delphi 侧的 `Access.pas` **本仓根本没有**
（`git ls-files | Select-String "Access.pas"` 零命中）⇒ 二者都是 COM/Windows 专有、**无可转调对象**。
故按「不臆造替身」原则落成 `interface` + `Func`/`Action` 委托，**不写一个假 ADO 去冒充能连库的东西**。

---

## 4. 偏离登记（D-P9-xx）

| 编号 | 偏离点 | 原文依据 | 托管处置 | 影响 |
|---|---|---|---|---|
| **D-P9-01** | 本车道**额外移植了 `ObjGame.pas` 的 `TGameObject`**（4 字段 + 构造，ObjGame.pas:9-17/51-62） | `ItemEvent.pas:9` 写的是 `TItemObject = class(TGameObject)`；`ObjGame.pas:9` 定义该类 | 在 `ItemEvent.cs` 内 1:1 落该类（**同单元不重复实现 `TGateObject`/`TDoorObject`**） | 属**跨单元依赖**而非新增第三份实现：实测 `git grep -n "class TGameObject" -- GXX.CSharp/src` **零命中**；既有的 `GXX.M2Server.IGameObject`（`MapObjectScanCore.cs:24`）只是 `m_ObjGame { get; }` 只读接口，**无任何实现类**，也不构成层次。若把基类字段平铺进 `TItemObject` 会破坏 §18.8 的层次真实性 |
| **D-P9-02** | `TItemObject.Destroy` 落成**普通公开方法**而非 `IDisposable.Dispose` | `ItemEvent.pas:85 destructor TItemObject.Destroy; override` | 同名同序的 `public void Destroy()`，由测试直接调用 | 托管侧无确定性析构；实现 `IDisposable` 会把"释放钩子"升格为契约，偏离原文 |
| **D-P9-03** | `TItemObject.m_PEnvir` 是 `object?` 而原文是 `TObject`（不是 `TEnvirnoment`） | `ItemEvent.pas:23 m_PEnvir: TObject;` | 逐字保留为 `object?`；`DeleteFromMap` 走 `IItemGameEnvir` 接口，回退既有的 `Engine.TEnvirnoment` | 原文类型就是 `TObject`，`TEnvirnoment(...)` 是**未检查硬转换**；托管侧保留该"不安全"性质（非两者之一 ⇒ `InvalidCastException`） |
| **D-P9-04** | 接缝为 null 时的**空保护**：`DataManageGlobals.DBQry`/`ADOConnection` 为 `null` 时，`TAccessTable` 的 12 个转发方法与 `TAccessEngine.Connect/DisConnect` 静默无操作 | 原文在无 ADO 对象时是 `nil` 解引用 ⇒ AV | `if (x != null)` 守护 | **唯一一处有意的行为收窄**。理由：接缝默认工厂返回 `null`（无 Jet/ADO 运行时），若保留 AV 则**构造就崩**，130 例中 50 例无法运行。已由 `AllForwarders_WhenGlobalQueryIsNull_AreSilentNoOps` 与 `Lock_WhenAccessEngineUnassigned_Throws_OriginalFlaw`（**该处保留抛异常**，因为原文的 `AccessEngine` 恒 nil 是原文缺陷本身）分别锁定 |
| **D-P9-05** | `TGameObject.m_ObjGame` 复用既有 `GXX.M2Server.TObjGame`（`MapObjectScanCore.cs:9`） | `ObjGame.pas:10 m_ObjGame: TObjGame;`（`M2Definition.pas`） | 不另造第二个 `TObjGame` 枚举 | 台账 §14.2「不造第三份实现」 |
| **D-P9-06** | `ItemEvent.pas` 的 `TList` 形参落成 `TGList?` | `ItemEvent.pas:278 FindItem(..., List: TList)` | 用既有的 `GXX.Core.Protocol.SDK.TGList` | `TGList` 是 `TList` 的派生等价物（`SDK.cs:55`）；挂 `?` 以表达原文的 `List <> nil` |
| **D-P9-07** | `DataManage.pas` 的 `m_UserCriticalSection: TRTLCriticalSection` 落成 `readonly object` + `System.Threading.Monitor` | `:9/:81/:125/:132/:137` | Win32 临界区 → `Monitor`（**同为可重入**） | 无独立 `InitializeCriticalSection`/`DeleteCriticalSection` 对应物（字段初始化即完成）；`Lock_ThenUnLock_IsReentrantAndBalanced` 断言可重入性 |

---

## 5. 原文缺陷清单（全部**照抄** + 差异断言锁定）

> 工程方法论第 2 条：原文的写法就是标准；可疑处**不顺手修**，而是注释 + 差异断言。

### 5.1 `ItemEvent.pas`

| # | 位置 | 缺陷 | 差异断言用例 |
|---|---|---|---|
| F-1 | :102 vs :105；:110 vs :113 | **同一表达式块里 `MyGetTickCount` 被调用两次**（判据用先读到的值、赋值用后读到的值） | `Run_Expiry_CallsTickCountTwice_TickStampIsOneAheadOfJudgement_OriginalFlaw` |
| F-2 | :102 | 注释 `{60 * 60 * 1000}` 与**实际读的** `g_Config.dwClearDropOnFloorItemTime` 不一致（注释是过期常量） | 代码按实际读 `g_Config`；`Run_Expiry_UsesStrictGreaterThan` 用任意阈值证明"读的是 g_Config" |
| F-3 | :109 | `TEnvirnoment(m_PEnvir)` 是**无保护硬转换**：`m_PEnvir = nil` 时 :110 读 `m_boFB` 即 AV | `Run_NilEnvir_DereferencesAnyway_OriginalFlaw` |
| F-4 | :143-145 | `if not DeleteFromMap(...) then begin end;` —— **空 then 块，返回值被完全丢弃**（删除失败照样写日志、照样 `m_PEnvir := nil`） | `Run_Ghost_EmptyThenBlock_DeleteFailureStillLogsAndClearsEnvir_OriginalFlaw` |
| F-5 | :266-267 | 第三个 `FindItem` 的判据**混用两个变量**：`(AItemObject = ItemObject)` 比列表元素，而坐标 `(ItemObject.m_nMapX = nX)` 读**形参** | `FindItem_ByCoordsAndObject_ReadsCoordsFromParameterNotListElement_OriginalFlaw` |
| F-6 | :25-27 | `m_boDieDrop`/`m_boHumDrop`/`m_boNpcThrowItem` **构造不初始化且整个单元零引用** | `Ctor_LeavesThreeDropFlagsAtFalse_OriginalDoesNotAssignThem` |
| F-7 | :350-351 | 异常路径 `except MainOutMessage(...)` **不重置 `m_nProcItemIDx`**（正常路径 :348-349 会重置）—— 不对称 | `Run_SwallowsExceptionLogsAndKeepsCursor_OriginalFlaw` |
| F-8 | :380 | 空闲表清理里的 `// break;` **被注释掉** ⇒ 一趟可删多个 | `Run_FreeListCleanup_RemovesAllExpiredInOnePass_BreakIsCommentedOut_OriginalFlaw` |
| F-9 | :353-369 | 整段旧版倒序循环**被 `{ }` 注释掉**（不执行） | 逐字保留为注释；`Run_ContinueAfterDelete_ProcessesNextElementAtSameIndexInSameFrame` 锁定的是**新**循环的 `Continue` 语义 |
| F-10 | :163 | `// m_PEnvir := nil;` **被注释掉** ⇒ `MakeGhost` 不清地图 | `MakeGhost_DoesNotClearEnvir_CommentedOutInOriginal` |
| F-11 | :220-221 | 多行布尔表达式在断行处无运算符（Delphi 隐式续行） | `FindItem_ByObject_*` 5 例锁定三条件与 |
| F-12 | :173-189 | `TItemManager.Destroy` 逐个 `Free` 后**不置 nil**，随后的 `m_ItemList.Free` 也不拥有元素 | `Destroy_WithItemsAndFreeItems_DoesNotThrow`（托管侧 GC 承担释放，顺序保持） |

### 5.2 `DataManage.pas`

| # | 位置 | 缺陷 | 差异断言用例 |
|---|---|---|---|
| G-1 | :87-89 | `if CreateAccessDB(FileName, False) then begin end;` —— **空 then 块**，建库失败被静默吞掉 | `Ctor_CreateAccessDbResultIsDiscarded_EmptyThenBlock_OriginalFlaw` |
| G-2 | :84-85 | **无条件覆写单元级全局** `ADOConnection`/`DBQry`，**不 Free 旧值** ⇒ 多实例互相踩 + 内存泄漏 | `Ctor_SecondInstanceOverwritesGlobalsAndNeverAssignsAccessEngine_OriginalFlaw` |
| G-3 | 全文 | **没有** `AccessEngine := Self`（:182/:194/:230/:234 四处**只读**）⇒ 单元级 `AccessEngine` 恒为 nil ⇒ `TAccessTable.Lock`/`UnLock` 必 AV | `Lock_WhenAccessEngineUnassigned_Throws_OriginalFlaw` + `Lock_WhenAccessEngineAssigned_Forwards` |
| G-4 | :108-112 vs :101-106 | `try/except` **只包住** `Connected := True`；"赋连接串/接查询"在**保护圈之外** ⇒ 那里抛异常直接冒泡 | `Ctor_ConnectionStringThrows_PropagatesBecauseItIsOutsideTry_OriginalFlaw` vs `Ctor_ConnectThrows_LogsExactMessageAndDoesNotRethrow` |
| G-5 | :169-170 | `Connect` 的 `except` 是**空块** ⇒ 连接失败时 `Result` 保持 :157 的旧值；**且不写日志**（与构造函数 :111 不同） | `Connect_WhenConnectThrows_ReturnsFalseAndLogsNothing_OriginalFlaw` |
| G-6 | :13 | `AccessTableList: array of TAccessTable` 在**全文零引用**（无 `SetLength`、无下标） | `AccessTableList_IsNeverPopulated_ZeroReferencesInOriginal`（计数取证） |
| G-7 | :222 | `Count` 读的是**全局 `DBQry`** 的 `RecordCount`，**与 `m_sTable` 无关** | `Count_ReadsGlobalQueryRecordCount_NotTheNamedTable` |
| G-8 | :211 | `m_sTable` 被赋值后**从不参与任何数据库操作**（连 SQL 都没拼过表名） | 同 G-7；另 `Ctor_StoresFileNameAndOverwritesUnitGlobals` |
| G-9 | :198-204 | `SetTable` **未命中则静默无操作**（不追加新行）；`Value = nil` 时也照样写入 | `SetTable_MissingName_IsSilentlyIgnored_OriginalFlaw` + `SetTable_NullValue_WritesNullSlot_OriginalDoesNotGuard` |
| G-10 | :186-191 | `GetTable` 名字比较**大小写敏感**（Delphi `=`） | `GetTable_IsCaseSensitive_OriginalSemantics` |
| G-11 | :123-124 | 析构 `Free` 两个全局后**不清空指针**（悬垂引用） | `Destroy_CoUnInitializesAndLeavesGlobalsPointingAtOldObjects_OriginalFlaw` |
| G-12 | :270 | `FieldByName` 结果**不判 nil** | `GetField_MissingField_ReturnsNull` |
| G-13 | :245 | `DBQry.SQL.Add(SQL)` **末尾无分号**（排版差异，无行为影响） | 逐字保留说明 |
| G-14 | :265 | `ExecSQL` **无 try/except** ⇒ 语句出错直接冒泡 | `OpenNextCloseExecSql_ForwardToGlobalQueryInOrder` 锁定调用序列 |

---

## 6. 未完成 / 阻塞项（如实登记）

### 6.1 请求集成方执行（`tools/**` 与其它车道分区，本车道**禁改**）

1. **删除 `tools/unit-map.tsv:84` 的 `UserShopDB_Old	par/p9-m2-datalayer` 行。**
   理由：该单元经 §2 判定为**死代码 + 不移植**，行留着会变成台账 §38.2 所说的"永久占位"。
   同时**不要**为本单元建 `UserShopDB_Old.cs`（见 §2.4）。
2. **`ItemEvent` / `DataManage` 两行**（`unit-map.tsv:85`/`:86`）：本车道已 100% 移植，完工后可删行
   （E1 证据已就位：`ItemEvent.cs`（文件名同单元名）+ `DataManage.cs`（同），
   且三份 `.cs` 的文件头都写了 `Source/M2Engine/<unit>.pas` ⇒ **E2** 亦成立）。
3. **`TGameObject` 的最终归位**（承 D-P9-01）：本车道在 `ItemEvent.cs` 内落了 17 行的 ObjGame 基类。
   若后续有车道移植 `ObjGame.pas` 全单元（`TGateObject`/`TDoorObject` 至今无人移植），
   应把它移到 `ObjGame.cs` 并让本文件改为引用 —— **本车道不越区**，仅登记。
4. **接缝接入点**（各一行，待依赖单元移植后替换）：
   - `Sweep9DataLayerSeam.GetStdItem` ← 待 `UsrEngn.pas`（`UserEngine.GetStdItem`）移植
   - `Sweep9DataLayerSeam.IsBaseObjectGhost` ← 待 `ObjBase.pas` 的 `TBaseObject.m_boGhost` 归位
   - `Sweep9DataLayerSeam.{EnvirBoFB,EnvirBoFBCreate}` ← 待 `Envir.pas` 的
     `TEnvirnoment.m_boFB`/`m_boFBCreate` 落地（**当前 `Engine/Envir.cs` 对这两个字段零命中**）
   - `Sweep9DataLayerSeam.AddGameDataLog` / `MainOutMessage` ← 待 `M2Share.pas` 移植
   - `DataManageAccessSeam.*` ← 待 ADO/Access 方案裁定（COM 互操作或整体改换数据访问层）
5. **`svMain.pas:1037/1083/1949/2124` 的 `g_ItemManager.Run/Create/Free`** 是 `TItemManager` 的
   **唯一生产调用点**（`main` 侧 `g_ItemManager` 零命中）。接线上只需在 `M2ShareGlobals` 加
   `public static TItemManager g_ItemManager;` 并在启动/收尾处调 `Run()`/`Create`/`Destroy`。
   **本车道未做**（`M2ShareGlobals` 不在分区内）。

### 6.2 本车道内的已知限制

| 项 | 说明 |
|---|---|
| `TItemManager.Destroy` / `TItemObject.Destroy` 的**释放语义** | 托管侧由 GC 承担（原文是显式 `Free`/`Dispose`）。元素遍历与顺序逐行保留，但"立即释放"不可表达 |
| `TGameObject` 的 `virtual` 构造 | C# 构造不能是 `virtual`；以"派生类构造链"表达，`ItemObject_IsDerivedFromGameObject` 断言层次存在 |
| `DataManage` 的 **ADO/Access 端到端行为** | 接缝默认实现是"无宿主"，故本单元的**文件流/建表/建索引/连库**行为**无法在本车道端到端验证**（需要 Jet OLEDB Provider）。已覆盖的是本单元**自己的 24 个方法的逻辑与调用序列** |
| `ItemEvent` 的**地图侧副作用** | `DeleteFromMap` 走替身；未验证真实 `Engine.TEnvirnoment.DeleteFromMap`（`Envir.cs:250`）的内部行为（那属 `Envir.pas` 的移植范围） |

### 6.3 没有做的事

- **没有**修改任何分区外文件（`git status` 只新增本车道的 7 个文件）。
- **没有**在主工作树执行任何 git 写命令；所有 `git grep`/`git ls-files`/`git log` 均为只读查询。
- **没有**改 `GXX.slnx` / 任何 `*.csproj` / `Directory.Build.props` / `docs/Checklist.md` /
  `docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**`
  （两个工程都是 SDK 式默认 glob，新文件自动纳入编译，**无需改 csproj**）。
- **没有**保留任何临时探查目录。

---

## 7. 提交序列

| 提交 | 内容 |
|---|---|
| `6b024c69` | 并行批次P1 切片1：ItemEvent.pas（`TItemObject`/`TItemManager`）+ DataManage.pas（`TAccessEngine`/`TAccessTable`）1:1 初稿 + 接缝层 |
| `f56684e2` | 并行批次P1 切片2：Sweep9DataLayer 测试 130 例（ItemEvent 90 + DataManage 40）全绿 |
| （本条之后）| 并行批次P1 切片3：接缝隔离反转（去掉 `Sweep9DataLayerSeam` ↔ `DataManageAccessSeam` 的 ResetDefaults 成环）+ 135 例（新增方法数对账 5 例）+ 本报告 |

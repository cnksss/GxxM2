# 并行报告：p10-db-login-forms（DBServer / LoginSrv / GameCenter / LogDataServer 窗体 + 线程池 7 单元）

> 分支：`par/p10-db-login-forms`　基线：`main @ d8674c8e`
> 独占分区：`GXX.CSharp/src/GXX.DBServer/Forms/**`、`src/GXX.LoginSrv/Forms/**`、
> `src/GXX.GameCenter/Forms/**`、`src/GXX.LogDataServer/Pool/**`、
> `tests/GXX.{DBServer,LoginSrv,GameCenter,LogDataServer}.Tests/P10*`、本文件
> 本报告**滚动更新**：每完成一个切片即 commit。

---

## 0. 开工侦察结论（先落盘）

### 0.1 源文件可用性 + DFM 格式判定（实测）

| 单元 | 行数 | UTF-8 镜像 `.pas` | `.dfm` 格式 | DFM 来源 |
|---|---|---|---|---|
| `DBServer/uFrmRoleDataEdit.pas` | 974 | ✅ | **文本**（20,861 B，GBK） | `Source/DBServer/` |
| `LoginSrv/MasSock.pas` | 1,017 | ✅ | ★ **二进制**（567 B） | `Source/LoginSrv/` |
| `LoginSrv/GrobalSession.pas` | 93 | ✅ | 文本（1,190 B，GBK） | `Source/LoginSrv/` |
| `GameCenter/GHeroDBConfig.pas` | 565 | ✅ | 文本（6,335 B，GBK） | `Source/GameCenter/` |
| `GameCenter/GLoginServerRouteSet.pas` | 35 | ✅ | 文本（600 B，GBK） | `Source/GameCenter/` |
| `LogDataServer/ThreadPool.pas` | 445 | ✅ | 无 DFM（非窗体） | — |
| `LogDataServer/FileSearchPool.pas` | 558 | ✅ | 无 DFM（非窗体） | — |

**二进制 DFM 处置**：`_analysis/utf8_mirror/**/*.dfm` 镜像是坏的（二进制 DFM 被当文本转码），
按 §41.3 一律**回读 `Source/` 原始文件**。`MasSock.dfm` 已手工解码（见 §0.5）。

### 0.2 ★★ `GLoginServerRouteSet.pas` = **死代码**，且**早已被移植**（重命名类）

**裁定：不新建任何文件**（既不移植、也不登记为"未移植缺口"）。两组独立取证：

**(a) 死代码取证（原始构建里根本不编译该单元）— 6 条计数**

| # | 取证项 | 命中数 | 说明 |
|---|---|---|---|
| 1 | `Source/**/*.dpr` 含 `GLoginServerRouteSet` | **0** | `GameCenter.dpr` 的 uses 里没有它 |
| 2 | `Source/**/*.dproj` 含 `GLoginServerRouteSet` | **0** | `GameCenter.dproj` 的 13 条 `<DCCReference>` 里没有它 |
| 3 | `Source/**/*.dpk` 含 `GLoginServerRouteSet` | **0** | — |
| 4 | 任何 `.pas` 的 `uses` 命中 | **0** | 全树唯一命中的 `.pas` 是它**自己**（unit 声明 / class 声明 / 全局 var / 注释 / 方法头，5 处自引用） |
| 5 | 调用点（`frmLoginServerRouteSet.` / `.Open(`） | **0** | 全树 `LoginServerRouteSet` 子串命中总数 = **4**，全部在 `GLoginServerRouteSet.pas`(4) + `.dfm`(1) 自身 |
| 6 | `Application.CreateForm` 列表 | 只有 3 个（`TfrmMain`/`TfrmLoginServerConfig`/`TFrmBDEToSqlite`） | 无 `TfrmLoginServerRouteSet` |

**(b) 已被移植的取证（审计的 WEAK 判定是"类名改写"造成的假缺口）**

| 原文 | 托管现有实现 | 证据 |
|---|---|---|
| `TFrmLoginServerRouteSet`（1 个方法 `Open`） | `LoginServerRouteSetForm` | `src/GXX.GameCenter/GLoginServer.cs:85-130`，头注释明写 `GLoginServerRouteSet.pas（26 行）1:1 移植` |
| DFM 1 个控件 `GroupBox1: TGroupBox`、Caption=`'路由设置'` | `public GroupBox GroupBox1` | `GLoginServer.cs:92`、`:114` |
| `m_boNewRouteMode := boNewRouteMode; ShowModal;` | `Open(bool)` → `m_boNewRouteMode = ...; ShowModalEquivalent();` | `GLoginServer.cs:119-123` |
| 已有用例 | 2 例（`FormSmallUnitsTests.cs:341/359`） | — |

⇒ 审计报表把 `GLoginServerRouteSet` 报成 `WEAK`（"`TfrmLoginServerRouteSet` 无声明"），根因是
**类名改写**（`Tfrm…` → `…Form`），不是真缺口。**处置**：本车道**不建同名 `.cs`**（那会制造
"第 2 份实现"，违反 §14.2），登记为"死代码 + 已移植（重命名）"。

> ⚠ 顺带发现 2 处该既有移植的**小偏离**（在分区外，未改，登记为 B-P10-06）：
> ① `GroupBox1.Text` 写成 `"路由"`，而 DFM 的 `Caption` 是字面量 `'GroupBox1'`；
> ② 窗体尺寸 `420×260` vs DFM `ClientWidth=482 / ClientHeight=357`；
> ③ 行数注释写"26 行"（那是 **DFM** 的行数；`.pas` 是 35 行）。

### 0.3 ★ 两处"接缝同名冲突"与命名空间处置（D-P10-01 / D-P10-15）

本车道的两个分区里都存在**根命名空间中已有一个同名但更窄的接缝类型**，而接缝所在文件
**在本分区之外（禁止修改）**：

| 分区 | 冲突类型 | 既有接缝（分区外） | 本车道真实现 | 后果 |
|---|---|---|---|---|
| `GXX.LogDataServer/Pool/**` | `TPoolTask` / `TSearchTask` | `src/GXX.LogDataServer/LogDataShare.cs:152/158`（注释明写"待 FileSearchPool/ThreadPool 移植后接入"） | `Pool/ThreadPool.cs` / `Pool/FileSearchPool.cs` | 同命名空间会 **CS0101** 重名 |
| `GXX.LoginSrv/Forms/**` | `TMsgServerInfo` | `src/GXX.LoginSrv/LoginSrvShare.cs:287`（**4 字段**，而 `MasSock.pas:9-17` 是 **7 字段**） | `Forms/MasSock.cs` | 同命名空间会 **CS0101** 重名 |

**处置**：本车道的实现一律落在**与分区目录同名的子命名空间**——
`GXX.LogDataServer.Pool`、`GXX.LoginSrv.Forms`、`GXX.GameCenter.Forms`、`GXX.DBServer.Forms`；
测试同理（`….Pool.Tests` / `….Forms.Tests`）。

> **这不是风格偏好，是硬约束**（实测过一个坑）：C# 简单名查找在**外层命名空间成员**与
> compilation-unit 的 `using` 之间，**外层命名空间成员优先**。所以只要根命名空间里有一个同名类型，
> 即使写了 `using GXX.LogDataServer.Pool;`，简单名 `TPoolTask` 仍会解析到**接缝那一份**
> （本车道实测报 **CS0115「没有找到适合的方法来重写」**）。落在子命名空间后，
> 外层命名空间 = 子命名空间本身 ⇒ 结构上不可能再撞名。
> 该约定与既有 `GXX.M2Server.Forms`（`GXX.CSharp/src/GXX.M2Server/Forms/**`）一致。

### 0.4 依赖缺口（先登记，动手时按接缝处理）

| 原文依赖 | 托管现状（实测 grep） | 处置 |
|---|---|---|
| `LSShare.TConnInfo` / `TConfig.SessionList: TGList` | `TConnInfo` **0 命中**；`LoginSrvShare.TConfig`（`LoginSrvShare.cs:133-275`）**无 `SessionList` 字段** | `GrobalSession` 自带最小面 `TConnInfo` + 静态接缝宿主（未接线即抛）；`TGList` 复用 `GXX.Core.Protocol.SDK.TGList`（`SDK.cs:55`） |
| `LSShare.TMsgServerInfo` | `LoginSrvShare.cs:287` 只有 **4 字段**（真身 7 字段，且全树只定义在 `MasSock.pas`） | MasSock 自带 7 字段版本于子命名空间；接缝退役列为 B-P10-05 |
| `GHeroDB.pas` 的 `THeroDB` | **0 命中**（`GXX.DBServer` 的 `THeroDBBase` 是 `RoleDB.pas` 的另一回事） | `GHeroDBConfig` 自带显式接缝（默认抛"未接线"） |
| `DBShare.pas` 的 `THumData` / `THeroData` | `GXX.LoginSrv/RoleDBSeam.cs` 有**同名但不同源**的两个类（**不可复用**） | `uFrmRoleDataEdit` 自带显式接缝 |
| `TSpinEditLongWord` | `GXX.DBServer/SpinControls.cs` 只有 `TSpinEdit`/`TSpinEditEx` | `uFrmRoleDataEdit` 自带 |
| `Classes.TMemoryStream` 系 | `GXX.RunGate/IniFilesEx.cs:637`（`TMemoryStreamEx` 接缝）、`GXX.Core/Paradox/ParadoxDataSet.Seams.cs:129`（`TMemoryStream` 接缝）——**都各自注明"待 MemoryStreamEx.pas 移植后接入"** | `FileSearchPool` 的 `TCustomMemoryStreamEx`/`TMemoryStreamEx` 是 **FileSearchPool.pas 单元内自带的私有副本**（不是 `MemoryStreamEx.pas`）⇒ 属于本单元，按 1:1 自带；三份接缝的统一列为 B-P10-07 |
| 线程池设施（`GatewayKit`） | GatewayKit 里 **0 个** `TPoolManager`/`TPoolThread`/`TSearchManager` 声明（实测） | 无同族可复用 ⇒ `LogDataServer/ThreadPool.pas` 按 1:1 真移植（**已先读再判**） |
| `SelGate/ThreadPool.pas` 与 `LoginGate/ThreadPool.pas` | 两份 **逐字节相同**（sha256 `702113A1F52B0FC1`，26,839 B），是 Windows APC 线程池封装；`LogDataServer/ThreadPool.pas` **不同**（9,736 B，`F19C2EA4579DEEDC`） | 逐副本裁定：网关两份 = 已登记 VENDOR（被 GatewayKit 取代）；LogDataServer 份 = 本车道真移植（文件名就叫 `ThreadPool.cs`，无歧义） |

### 0.5 ★ `MasSock.dfm` 二进制解码结果（供逐控件/逐事件对账）

- 头部结构实测（**与 §41.3 记录的 `ViewHeroRcd.dfm` 同族，但头部长度随窗体名长度变化**）：
  `FF 0A 00` + `ShortString(窗体类名大写)` + `30 10` + `Int32(流长度)`；
  `TPF0` 流起始偏移 = `3 + (1+len(名)) + 2 + 4` = `10 + len(名)`。
  本例名 = `TFRMMASSOC`(10) ⇒ 流起于 **20**，流长 **547**，20+547 = **567** = 文件长度（自洽）。
  > 复核 §41.3 的 `ViewHeroRcd.dfm`：名 `TFRMHEROFDBVIEWER`(17) ⇒ 10+17 = 27 ✓ 与记录一致。
  > **即"27 字节头"是特例，"10 + len(类名)" 才是通式**（这条修正对后续二进制 DFM 车道有用）。
- 解码结果：**2 个 object / 6 条事件绑定**
  - `TFrmMasSoc` / `FrmMasSoc`：`Left=780 Top=172 Caption='FrmMasSoc' ClientHeight=107 ClientWidth=137
    Color=clBtnFace Font.Name='MS Sans Serif' Font.Height=-11 Font.Style=[] PixelsPerInch=96 TextHeight=13`；
    `OnCreate=FormCreate`、`OnDestroy=FormDestroy`
  - `TServerSocket` / `MSocket`：`Active=False Address='0.0.0.0' Port=0 ServerType=stNonBlocking Left=40 Top=32`；
    `OnClientConnect=MSocketServerConnect→MSocketClientConnect`、`OnClientDisconnect=MSocketClientDisconnect`、
    `OnClientRead=MSocketClientRead`、`OnClientError=MSocketClientError`
  - ⇒ **DFM 绑定数 6 必须等于托管 `+=` 数 6**；控件/组件实例化数 2 必须等于 DFM object 数 2。

### 0.6 ★ DFM 事件绑定地图（逐控件，独立于实现抽取，用于对账复核）

`uFrmRoleDataEdit.dfm`：**35 条**绑定（此前 WEAK 报表只给了"1 例程"的类计数，从未对过账）

| 目标处理器 | 控件数 | 控件（事件） |
|---|---|---|
| `FormCreate` | 1 | `FrmRoleDataEdit`(OnCreate) |
| `edtPasswordChange` | **31** | `edtPassword, edtDearName, edtMasterName, edtCurMap, seCurX, seCurY, seHomeX, seHomeY, seLevel, seGold, seGameGold, seGamePoint, seCreditPoint, sePayPoint, sePKPoint, seContribution, EditDC, EditMC, EditSC, EditAC, EditMAC, EditHP, EditMP, EditHit, EditSpeed, EditX2, seBonusPoint, seGameDiamond, seGameGird`（29×OnChange）+ `edtHomeMap, chkIsMaster`（2×**OnClick**） |
| `ButtonSaveDataClick` | 1 | `ButtonSaveData`(OnClick) |
| `ButtonExportDataClick` | **2** | `ButtonExportData`(OnClick) **与 `ButtonImportData`(OnClick)** ← ★ "导入"按钮绑的是**导出**处理器 |
| 合计 | **35** | 另有 **4 个无任何绑定**的控件（含 `lvMagic/lvUserItem/lvFenghaoItem/lvStorage/strGridVarU/strGridVarT` 等） |

`GHeroDBConfig.dfm`：**6 条**绑定，**窗体根无 `OnCreate`**：
`EditHeroDBPath`(OnButtonClick→`EditHeroDBPathButtonClick`)、`ButtonSaveHeroDBConfig`、`ButtonCreateStdItemsField`、
`ButtonMonsterField`、`ButtonMagicField`、`ButtonClose`（均 OnClick→同名 `…Click`）。

`GrobalSession.dfm`：**4 个 object / 1 条绑定**（`OnCreate=FormCreate`）；
`ButtonRefGrid`（`'刷新(&R)'`）**没有任何 `OnClick`** ⇒ §2 缺陷清单第 1 条。

---

## 1. 逐单元进度与对账（滚动）

| 单元 | 行数 | 已移植方法数/总方法数 | DFM/类 对账 | 状态 |
|---|---|---|---|---|
| `LogDataServer/ThreadPool.pas` | 445 | **32/32**（`Pool/ThreadPool.cs`） | 3 类 + 1 类引用类型 + 1 事件类型，全部 1:1 | ✅ 完成 |
| `LogDataServer/FileSearchPool.pas` | 558 | **28/28**（`Pool/FileSearchPool.cs`） | 7 类型 + 1 自由函数，全部 1:1 | ✅ 完成 |
| `LoginSrv/GrobalSession.pas` | 93 | **3/3**（`Forms/GrobalSession.cs`） | DFM 4 object → 实例化 4 ✅；绑定 1 → `+=` 1 ✅ | ✅ 完成 |
| `GameCenter/GLoginServerRouteSet.pas` | 35 | 0（死代码 + 已移植，见 §0.2） | DFM 1 控件（既有实现里已有） | ✅ 判定完成（**不建文件**） |
| `GameCenter/GHeroDBConfig.pas` | 565 | 进行中（子车道） | 目标：DFM 25 object / 6 绑定 | ⏳ |
| `LoginSrv/MasSock.pas` | 1,017 | 进行中（子车道） | 目标：DFM 2 object / 6 绑定 | ⏳ |
| `DBServer/uFrmRoleDataEdit.pas` | 974 | 进行中（子车道） | 目标：DFM 88 object / 35 绑定 | ⏳ |

**计数口径（可复跑，§37.3）**：在 UTF-8 镜像的 `.pas` 上，只取 `interface` 段，
逐行维护"当前类型"（`^\s*(T\w+)\s*=\s*class`），统计
`^\s*(class\s+)?(constructor|destructor|procedure|function)\s+(\w+)` ——
即**类型声明里的例程声明数**（含 private/protected 段的声明、含属性的读写 accessor、含 `override`/`abstract`）。
实现段（`implementation` 之后）里的函数体与方法头**不重复计数**；但仍**单独列出**实现段里新增的
单元级自由函数（`FileSearchPool.GetSearch`、`GHeroDBConfig.SelectDirCB/SelectDirectory`）。
本次实测：`ThreadPool` 32、`FileSearchPool` 27(+1 自由函数)、`GrobalSession` 3、
`MasSock` 18、`GHeroDBConfig` 8(+2 自由函数)、`uFrmRoleDataEdit` 14(+1 单元级过程)。

### 1.1 `ThreadPool.pas` 逐例程（**32/32**）

| 类 | 原文例程数 | 已移植 | 例程清单（原文名，1:1 保留） |
|---|---|---|---|
| `TPoolTask` | 6 | **6** | `Create` / `Cancel` / `CompareTask` / `Assign` / `AssignTo` / `AssignError` |
| `TPoolThread` | 11 | **11** | `GetSleeping` / `Execute` / `TriggerEvent` / `GetTerminated` / `DoExecInitialize` / `DoExecFinalize` / `DoExecuteLoop` / `DoTaskFinished` / `Create` / `Destroy`(→`Dispose`) / `Terminate` |
| `TPoolManager` | 15 | **15** | `GetSleepingThread` / `GetTaskCount` / `GetThreadCount` / `GetTasks` / `GetThreads` / `GetPoolThreadClass` / `DoTaskFinished` / `Create` / `Destroy`(→`Dispose`) / `AddTask` / `ClearTasks` / `CancelAndClearAllTask` / `FindTask` / `LockTaskList` / `UnlockTaskList` |
| 合计 | **32** | **32** | 另有 11 个属性/字段（`Canceled`·`Finished`·`Sleeping`·`Owner`·`ContextTask`·`TaskCount`·`Tasks`·`ThreadCount`·`Threads`·`OnTaskFinished`·`IsWantDestroy`）与 2 个类型（`TPoolThreadClass` 类引用、`TTaskFinishedEvent` 事件）1:1 |

### 1.2 `FileSearchPool.pas` 逐例程（**28/28**，含 1 个实现段自由函数）

| 类/单位 | 原文例程数 | 已移植 | 例程清单 |
|---|---|---|---|
| `TCustomMemoryStreamEx` | 5 | **5** | `SetPointer` / `Read` / `Seek` / `SaveToStream` / `SaveToFile` |
| `TMemoryStreamEx` | 8 | **8** | `SetCapacity` / `Realloc` / `Destroy`(→`Dispose`) / `Clear` / `LoadFromStream` / `LoadFromFile` / `SetSize` / `Write` |
| `TSearchTask` | 4 | **4** | `AssignTo` / `Create` / `Destroy`(→`Dispose`) / `CompareTask` |
| `TSearchThread` | 6 | **6** | `RefreshLabel` / `DoTaskComplete` / `DoSearch` / `DoExecuteLoop` / `Create` / `Destroy`(→`Dispose`) |
| `TSearchManager` | 4 | **4** | `GetActionChecked` / `GetPoolThreadClass` / `DoTaskComplete` / `AddTask` |
| 单元级自由函数（实现段） | 1 | **1** | `GetSearch`（寄居 `static class FileSearchPool`，同项目 `LogDataShare` 的既有约定） |
| 合计 | **28** | **28** | 另有 `Memory`·`Size`·`Position`·`Capacity`·`TaskID`·`ShowPanel`·`FileName`·`OnTaskComplete` 与 8 个 `TSearchManager` public 字段、1 个 `TTaskCompleteEvent` 事件，全部 1:1 |

### 1.3 `GrobalSession.pas` 逐例程（3/3）

`FormCreate` / `Open` / `RefGridSession`（private）——3 条，全部 1:1。

---

## 2. 原文缺陷清单（照抄 + `// 原文如此` + 差异断言锁死）

| # | 位置 | 缺陷 | 后果 | 锁死用例 |
|---|---|---|---|---|
| 1 | `GrobalSession.dfm:20-27` + `GrobalSession.pas`（全单元仅 3 条过程：`FormCreate`/`RefGridSession`/`Open`） | `ButtonRefGrid`（`'刷新(&R)'`）在 DFM 里**没有 `OnClick`**，`RefGridSession` 也**没有任何按钮绑定** | **"刷新"按钮永久失效**，点它什么都不会发生 | `P10GrobalSessionTests.Dfm_ButtonRefGrid_HasNoClickHandler_OriginalFlaw`（计数取证：`IsBound(btn,"Click")==false`、该控件绑定数 == 0、全窗体绑定数 == 1） |
| 2 | `GrobalSession.pas:57` | `PanelStatus.Caption := '正在取得数据...'` **只设不复位** | 取完数据后状态栏**永远停在"正在取得数据..."** | `P10GrobalSessionTests.RefGridSession_PanelStatusCaptionIsNeverReset_OriginalFlaw` |
| 3 | `GrobalSession.pas:69-75` | `GridSession.FixedRows := 1` **只在"会话数为 0"分支**里写；非空分支不写（靠 VCL 默认值 1） | 非空路径下 `FixedRows` 依赖 DFM/控件默认值 | `RefGridSession_EmptyList_LeavesTwoRowsAndFixedRowsOne` + `RefGridSession_NonEmptyList_FillsColumns`（两条路径都断言） |
| 4 | `GrobalSession.pas:59-64` | `GridSession.Cells[x, 1] := ''` 发生在 `RowCount` 被设置**之前**；DFM 里**没有** `RowCount`（VCL 默认 1 行）⇒ 首次调用时第 1 行**不存在** | 首次调用的"清空第 1 行"实际是**空操作**（VCL `TStringGrid.GetCells/SetCells` 对越界下标静默忽略） | `Cells_OutOfRange_IsSilentNoOp_VclStringGridSemantics`、`RefGridSession_ReusesGrid_RowCountShrinks` |
| 5 | `ThreadPool.pas:332-351` `TPoolManager.AddTask` | ① 任务进队列后**只唤醒 1 个**空闲线程；② `Sleep(5)` 那行**被注释掉** ⇒ 空闲线程可能尚未把 `FInExecuteLoop` 置位，紧接着的 `GetSleepingThread` 会把同一线程再算一次空闲 | 多任务突发时可能出现"加了任务但没人立刻取"的时序窗口（原文自己注释承认） | `P10ThreadPoolTests.TPoolManager_AddTask_EnqueuesAndSignalsOneSleepingThread` |
| 6 | `ThreadPool.pas:276-281` `TPoolManager.Destroy` | `//Thread.WaitFor;` 被注释掉（等待实际发生在 `TThread.Destroy` 内部）；且销毁前用 `while not Thread.Sleeping do Sleep(1) + Application.ProcessMessages` **忙等** | 忙等 + 隐式等待 | `TPoolManager_Dispose_SetsIsWantDestroyAndStopsThreads`、`TPoolManager_Dispose_DisposesRunningContextTask` |
| 7 | `ThreadPool.pas:186-189` `TPoolThread.DoTaskFinished` | **全单元零调用点**（`FileSearchPool` 的 `TSearchThread` 走的是自己的 `DoTaskComplete`） | 该方法在本单元对里是**死代码**（仅作为派生类可调用的 protected 钩子存在） | `TPoolThread_DoTaskFinished_InvokesManagerOnTaskFinished`（正向）+ `..._WithoutHandler_IsNoOp`；零调用点由本报告计数取证 |
| 8 | `FileSearchPool.pas:521-522` `TSearchThread.DoSearch` | 整个函数体包在 `try ... except end` 里 ⇒ **静默吞掉一切异常**（含 `LoadFromFile` 打不开文件） | 打不开/损坏的日志文件**没有任何提示**，且照常回调"完成" | `TSearchManager_Search_MissingFile_IsSilentlySwallowed` |
| 9 | `FileSearchPool.pas:421-423`（读 `nServerNumber`/`nServerIndex`）vs `:489-501`（写 `LogData`） | 两个字段**读出来后从未写进 `LogData`**（局部变量读了就丢） | 结果集里 `nServerNumber`/`nServerIndex` **恒为 0** | `TSearchManager_Search_HappyPath_PushesMatchingRecord`（`Assert.Equal(0, rec.nServerNumber/nServerIndex)`） |
| 10 | `FileSearchPool.pas:295-300` `TSearchTask.CompareTask` | 函数体只有 `Result := False`（比较逻辑的局部变量声明被注释掉，从未写） | `TSearchTask` 的 `FindTask` **永远找不到**任何搜索任务 | `TSearchTask_CompareTask_AlwaysFalse` |
| 11 | `FileSearchPool.pas:284-293` `TSearchTask.AssignTo` | 只搬 `FPanel`，`TaskID`/`FileName` **不搬** | `Assign` 后目标的 Id/文件名仍是旧值 | `TSearchTask_Assign_SearchToSearch_CopiesPanelOnly` |
| 12 | `FileSearchPool.pas:539-550` `GetActionChecked` | `Result := SearchActions[LoByte(W1)] or SearchActions[HiByte(W1)]` —— **高位字节（`HiByte`）也参与"或"** | 例如 `nAction=5` 时 `SearchActions[0]` 单独为真也会放行 ⇒ 关掉的动作可能仍被搜索到 | `TSearchManager_GetActionChecked_HighByteOfLowWordAlsoGates` |
| 13 | `FileSearchPool.pas:364-370` `GetSearch` | `Result := True; if nWhere <= 0 then Exit;` ⇒ `nWhere<=0` 恒返回 **True**（"不筛选"） | 调用方若误传 0/负数 ⇒ 过滤器**全部通过** | `GetSearch_WhereZero_ReturnsTrue` |
| 14 | `FileSearchPool.pas:130-138` `Seek` | `case Origin of` **无 `else`** ⇒ 未知 Origin 时 `FPosition` 不变（不报错） | 错误 Origin 被静默忽略 | `TCustomMemoryStreamEx_Seek_UnknownOrigin_LeavesPositionUnchanged` |
| 15 | `FileSearchPool.pas:330-337` `TSearchThread.DoExecuteLoop` | `if Tasks.Count = 0 then Exit;` 位于 `try` 内 ⇒ `finally` 里的 `UnlockTaskList` **仍会执行**（正确），但 `Exit` 与 `FreeAndNil(FContextTask)` 的顺序使"上一轮任务"总是被释放 | 任务对象生命周期与调用方预期可能不符 | `TSearchManager_Search_*` 端到端 6 例 |
| 16 | `uFrmRoleDataEdit.dfm` | `ButtonImportData.OnClick` 绑的是 **`ButtonExportDataClick`**（"导入"按钮触发"导出"处理器） | 见 §0.6；由子车道判定是否 `Sender` 分流 | 见 §1（uFrmRoleDataEdit 行） |
| 17 | `FileSearchPool.pas:284-293` + `ThreadPool.pas:168-174` | `TSearchThread.Destroy` 先 `FMemoryStream.Free` 再 `inherited`（后者才 `Terminate+WaitFor`）⇒ **先释放缓冲区、后等线程退出** | 后台线程可能在缓冲已释放后继续用（原文靠时序侥幸） | D-P10-05（托管侧把"等待退出"提前，见 §3） |

---

## 3. 偏离登记（D-P10-xx）

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P10-01 | `ThreadPool.pas` / `FileSearchPool.pas` | 单元内 `TPoolTask` 等 | 落在子命名空间 `GXX.LogDataServer.Pool` | 分区外接缝 `LogDataShare.cs` 已声明同名类型 ⇒ 同命名空间必 CS0101；不能在分区外改（见 §0.3） |
| D-P10-02 | `ThreadPool.pas:138` `EConvertError` | `SysUtils.EConvertError`（RTL） | 在 `Pool/ThreadPool.cs` 内声明接缝类（含 `SAssignError` 的 `%s` 格式化） | `GXX.Core.Rtl` 实测**无**该异常类型；归位 `GXX.Core` 属跨区（B-P10-02） |
| D-P10-03 | `ThreadPool.pas:35/60` `virtual; abstract` | `TPoolTask.CompareTask`、`TPoolThread.DoExecuteLoop` 为 `abstract`，而 `TPoolManager.GetPoolThreadClass` 默认返回**非抽象**的 `TPoolThread`（Delphi 允许，运行期抛 `EAbstractError`） | 方法保持 `virtual`，方法体抛 `EAbstractError` | C# 的 `abstract` 成员要求类也 `abstract`，而抽象类无法被 `Activator.CreateInstance` 实例化 ⇒ 无法表达"可实例化但方法抽象"。托管运行期行为与 Delphi 一致 |
| D-P10-04 | `ThreadPool.pas:39/64` | 基类 `TThread`；`constructor Create; virtual` | 自带 `System.Threading.Thread` 的包装类 + `IsBackground = true`；虚构造由 `GetPoolThreadClass(): Type` + `Activator` 还原 | 托管无"虚构造"语法；`IsBackground` 防止异常路径下测试宿主挂死（管理器的 `Destroy` 一定会 `Terminate+Join`） |
| D-P10-05 | `ThreadPool.pas:168-174` | `FEvent.Free` **先于** `inherited`（= `Terminate`+`WaitFor`） | 把"等待线程退出"提到释放 `AutoResetEvent` **之前** | 照抄原文会在后台线程仍等事件时释放事件 ⇒ `ObjectDisposedException`；净语义相同（原文也必然等待退出） |
| D-P10-06 | `ThreadPool.pas:73` `TPoolThreadClass = class of TPoolThread` | 类引用类型 | `Type` + `Activator.CreateInstance` | 托管无 `class of`；虚分派点（`GetPoolThreadClass`）保留为 `virtual`/`override` |
| D-P10-07 | `FileSearchPool.pas:10` `TCustomMemoryStreamEx = class(TStream)` | 继承 `TStream` | 独立类（**不**继承 `System.IO.Stream`） | `Stream` 的契约（短读必须抛）与本类相反（短读返回 0）⇒ 继承会让 1:1 语义无法成立 |
| D-P10-08 | `FileSearchPool.pas:52` `ShowPanel: TStatusPanel` | ComCtrls `TStatusPanel` | `System.Windows.Forms.ToolStripStatusLabel` | 托管无 `TStatusPanel`；沿用既有接缝 `LogDataShare.cs:161` 的同款选择 |
| D-P10-09 | `FileSearchPool.pas:489-511` | 局部 `LogData: TLogData` 记录 → `New(PLogData); PLogData^ := LogData`（堆上复制快照） | 每轮 `new TLogData()` 并把该实例入表 | 语义等价（`nServerNumber`/`nServerIndex` 从未赋值 ⇒ 无跨轮残留），且避免"同一实例入表 N 次"的假实现 |
| D-P10-10 | `FileSearchPool.pas:346/350` `Synchronize` | `TThread.Synchronize`（阻塞式投递到主线程） | `SynchronizeHandler` 接缝，默认**就地调用** | 无头/测试环境没有 VCL 消息泵；宿主可注入真正的 UI 线程投递 |
| D-P10-11 | `ThreadPool.cs`/`FileSearchPool.cs` 的测试 | — | 测试命名空间 `GXX.LogDataServer.Pool.Tests`（而非工程根 `…Tests`） | 见 §0.3 的 C# 名字查找硬约束；实测在根命名空间会 CS0115 |
| D-P10-12 | `ThreadPool.pas:191-208` 线程体 | Delphi 线程体异常**不终止进程**（`ThreadProc` 交顶层异常处理，线程结束） | `RunThread` 捕获并存入 `public Exception? ExecException` | .NET 默认"任一线程未处理异常 ⇒ 进程立即终止"，对服务端是语义放大（实测：一个 `NullReferenceException` 直接把 xunit testhost 打崩）。**不静默吞**：异常可从外部读取/断言 |
| D-P10-13 | `GrobalSession.pas:58/70` `GridSession.Visible` / `FixedRows` | VCL `TControl.Visible` 返回**写入值**；`TStringGrid.FixedRows` 是控件属性 | 额外提供 `GridSessionVisible`（属性值承载）与 `FixedRows` 字段；写入时**同时**设置 `GridSession.Visible` | WinForms `Control.Visible` getter 返回**有效可见性**（窗体未 `Show` 时恒 false）⇒ 无头环境无法复刻原文读回语义；`DataGridView` 无 `FixedRows` |
| D-P10-14 | `GrobalSession.pas:59-64` | `GridSession.Cells[x,1] := ''`（`RowCount=1` 时第 1 行不存在） | `Cells`/`SetCells` **越界静默忽略/返回空串** | VCL `TStringGrid` 的稀疏行实现对越界下标静默忽略；1:1 直译成 `Rows[Row]` 会抛 `ArgumentOutOfRangeException`（首次 `Open()` 即崩） |
| D-P10-15 | 本车道全部窗体 | — | 实现落 `GXX.<Proj>.Forms`，测试落 `GXX.<Proj>.Forms.Tests` | 同 §0.3；并与既有 `GXX.M2Server.Forms` 约定一致 |

---

## 4. 未完成 / 阻塞项

### 4.1 跨区事项（分区外，本车道**未改**）

| 编号 | 事项 | 影响 |
|---|---|---|
| B-P10-01 | `src/GXX.LogDataServer/LogDataShare.cs` 的接缝退役：`TPoolTask`(152)/`TSearchTask`(158)/`TSearchManagerSeam`(173)/`TSearchManagerHost`(201) 应改为 `GXX.LogDataServer.Pool` 的真实现；`LogDataShare.ResetForTests():59` 也要跟着改；`LogManage.cs:223/581` 的 `TSearchManagerSeam` 引用同理 | 不退役则接缝与真实现**长期并存**（两套 `TSearchTask`），`LogManage` 用不到真搜索池 |
| B-P10-02 | `EConvertError` / `EAbstractError` / `EStreamError`（本区私有接缝）应归位 `GXX.Core.Rtl` | 三处同类接缝可能在他处重复声明 ⇒ CS0101 |
| B-P10-03 | `src/GXX.LoginSrv/LoginSrvShare.cs`：`TMasSocSeam`(278) / `FrmMasSoc`(53) / 4 字段 `TMsgServerInfo`(287) 应在 MasSock 落地后退役并改指真 `TFrmMasSoc` | 不退役则 `LMain` 侧永远拿到空 `m_ServerList`（**静默**） |
| B-P10-04 | `src/GXX.LoginSrv/LoginSrvShare.cs` 的 `TConfig` 缺 `SessionList: TGList`；`GrobalSession` 已用静态接缝 `GrobalSessionHost.SessionList` 承载（未接线即抛） | LSShare 整单元落地时必须把接缝换成 `TConfig.SessionList` 并补 `TConnInfo`（本区已声明最小面 5 字段版本，需去重） |
| B-P10-05 | `src/GXX.GameCenter/GLoginServer.cs:85-130` 的既有 `LoginServerRouteSetForm` 三处小偏离（`GroupBox1.Text` 应为字面量 `'GroupBox1'`、窗体尺寸、行数注释） | 界面一致性；见 §0.2 |
| B-P10-06 | `TMemoryStream`/`TMemoryStreamEx` 全仓现有 **3 份接缝**（`GXX.RunGate/IniFilesEx.cs:637`、`GXX.Core/Paradox/ParadoxDataSet.Seams.cs:129`、本车道 `Pool/FileSearchPool.cs`）+ 待移植的 `Common/MemoryStreamEx.pas` | 建议下一波统一到 `MemoryStreamEx.pas` 的真移植上（§14.2 家族） |

### 4.2 本车道未完成项

（滚动登记：三个在飞单元——`MasSock` / `GHeroDBConfig` / `uFrmRoleDataEdit`——完成后在此收口）

---

## 5. 门禁记录（滚动）

| 切片 | 命令 | 结果 |
|---|---|---|
| 1（Pool 2 单元） | `dotnet test tests/GXX.LogDataServer.Tests/…csproj -c Debug --nologo -m:1 -p:BuildInParallel=false` | `失败: 0，通过: 222，总计: 222`（其中本车道新增 **80** 例） |
| 2（GrobalSession） | `dotnet test tests/GXX.LoginSrv.Tests/…csproj …` | `失败: 0，通过: 250，总计: 250`（其中本车道新增 **19** 例） |

---

## 6. 复现命令

```powershell
# 二进制 DFM 解码（MasSock.dfm）——成例，供后续二进制 DFM 车道复用
$path = "D:\chuanqi\daima\GXX原版_Delphi7\Source\LoginSrv\MasSock.dfm"
$b = [System.IO.File]::ReadAllBytes($path)      # ★ 必须 ReadAllBytes；Get-Content 会读坏二进制
# 头部：FF 0A 00 + ShortString(窗体类名大写) + 30 10 + Int32(流长度)
#       流起始偏移 = 10 + len(类名)（本例 10+10=20），流长 547 ⇒ 20+547 = 567 = 文件长度
# 流签名 TPF0 起：对象 = ShortString类名 + ShortString名 + 属性表(0x00结尾) + 子对象表(0x00结尾)
# 值类型：01=vaList 02=int8 03=int16 04=int32 06=vaString 07=vaIdent 08=False 09=True
#         0B=vaSet(ShortString表,0x00结尾) 12=vaWString(Int32字符数+UTF-16LE)

# DFM 逐控件事件地图（文本 DFM）
$txt=[System.Text.Encoding]::GetEncoding(936).GetString([System.IO.File]::ReadAllBytes($dfm))
# 逐行维护 object 缩进栈，遇到 `^\s*(On\w+)\s*=\s*(\w+)$` 即输出 (当前 object, 事件, 处理器)
```

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

> ⚠ 顺带发现 2 处该既有移植的**小偏离**（在分区外，未改，登记为 B-P10-05）：
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
| `DBShare.pas` 的 `THumData` / `THeroData` | ★ **已存在**：`GXX.Core.Protocol.THumData`（`Grobal2.Types4.cs:128`，`unsafe struct` + `Pack=1` + `fixed byte` 短串）与 `THeroData`（同文件 `:275`）；`GXX.DBServer/MySqlRoleDB.Base.cs:43/50/448/451` 与 `DBServerService.cs:259-280` 已在用它们 | **复用**（不造替身）。⚠ `GXX.LoginSrv/RoleDBSeam.cs:36/41` 另有**同名但不同源**的两个类（RoleDB.pas 侧接缝），**不可混用** |
| `SizeOf(THumData)` 字节级记录 I/O | ★ **已存在**：`GXX.Core.Protocol.StructBytes`（`ShortStr.cs:77`）：`SizeOf<T>` / `BytesOf<T>` / `FromBytes<T>` / `ToBytes<T>` | **复用**（`uFrmRoleDataEdit` 的 `FileWrite/FileRead(... SizeOf(...))` 直接落在这上面） |
| `g_RoleDB.HumanDB/HeroDB` | ★ **已存在注入点**：`GXX.DBServer.SelectClientRoleDbSeam.HumanDB/HeroDB`（`SelectClient.Seams.cs:141-181`，含显式抛错的 `RequireHuman/RequireHero`）；`THumanDBBase.Save(int, ref THumData)`（`MySqlRoleDB.Base.cs:352`）、`THeroDBBase.Save(int, ref THeroData)`（`:547`） | **复用**。⚠ 适配器 `SelectClientHumanDb/SelectClientHeroDb` 只接线了 `SelectClient.pas` 用到的 `Do*`，其余抛 `NotSupportedException` ⇒ 若窗体走 `Save`，宿主需换用完整适配器（跨区事项） |
| `TSpinEditLongWord` | `GXX.DBServer/SpinControls.cs` 只有 `TSpinEdit`/`TSpinEditEx`（`TSpinEditEx` 可直接复用） | `uFrmRoleDataEdit` 自带 `TSpinEditLongWord` |
| `TListView` 列表面 | ★ **已存在**：`GXX.DBServer/ListViewSink.cs` 的 `IListViewSink`/`ListViewSink`（`Ranking.cs` 已有 9 处在用） | **复用** |
| `Classes.TMemoryStream` 系 | `GXX.RunGate/IniFilesEx.cs:637`（`TMemoryStreamEx` 接缝）、`GXX.Core/Paradox/ParadoxDataSet.Seams.cs:129`（`TMemoryStream` 接缝）——**都各自注明"待 MemoryStreamEx.pas 移植后接入"** | `FileSearchPool` 的 `TCustomMemoryStreamEx`/`TMemoryStreamEx` 是 **FileSearchPool.pas 单元内自带的私有副本**（不是 `MemoryStreamEx.pas`）⇒ 属于本单元，按 1:1 自带；三份接缝的统一列为 B-P10-06 |
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
| `ButtonExportDataClick` | **2** | `ButtonExportData`(OnClick) **与 `ButtonImportData`(OnClick)** ← 同一处理器被两个按钮共用（**不是缺陷**：`:722-733` 用 `Sender` 分流；但其中 `Sender = ButtonSaveData` 那支是**空体死代码**，因为 `ButtonSaveData` 绑的是另一个处理器） |
| 合计 | **35** | 另有 **4 个无任何绑定**的控件（含 `lvMagic/lvUserItem/lvFenghaoItem/lvStorage/strGridVarU/strGridVarT` 等） |

`GHeroDBConfig.dfm`：**6 条**绑定，**窗体根无 `OnCreate`**：
`EditHeroDBPath`(OnButtonClick→`EditHeroDBPathButtonClick`)、`ButtonSaveHeroDBConfig`、`ButtonCreateStdItemsField`、
`ButtonMonsterField`、`ButtonMagicField`、`ButtonClose`（均 OnClick→同名 `…Click`）。

`GrobalSession.dfm`：**4 个 object / 1 条绑定**（`OnCreate=FormCreate`）；
`ButtonRefGrid`（`'刷新(&R)'`）**没有任何 `OnClick`** ⇒ §2 缺陷清单第 1 条。

---

### 0.7 活代码取证：本车道另外 4 个单元**都是活代码**（与 `GLoginServerRouteSet` 形成对照）

用与 §0.2 同一套计数口径复核，避免"死代码"这一成因只抓到一个：

| 单元 | `.dpr` 命中 | `.dproj` 命中 | 其他单元的引用（不含自身） |
|---|---|---|---|
| `LoginSrv/MasSock` | **1**（`LoginSrv.dpr:6`，`{FrmMasSoc}`） | **1**（`LoginSrv.dproj:78`） | **3**：`LMain.pas:223`（uses）、`MonSoc.pas:28`（uses）、`FrmFindId.pas:37`（uses） |
| `LoginSrv/GrobalSession` | **1**（`LoginSrv.dpr:17`，`{frmGrobalSession}`） | **1**（`LoginSrv.dproj:71`） | **1**：`LMain.pas:223`（uses） |
| `DBServer/uFrmRoleDataEdit` | **1**（`DBServer.dpr:15`，`{FrmRoleDataEdit}`） | **1**（`DBServer.dproj:91`） | **1**：★ `LoginSrv/uFrmDataManager.pas:64`（**跨模块** uses，见 B-P10-08） |
| `GameCenter/GHeroDBConfig` | **1**（`GameCenter.dpr:15`，`{FrmHeroDB}`） | **1**（`GameCenter.dproj:62`） | **1**：`GMain.pas:574`（uses） |

⇒ 4 个单元全部**在构建里、被别处引用**，必须真移植；而 `GLoginServerRouteSet` 三项全 0（§0.2）。

---

## 1. 逐单元进度与对账（滚动）

| 单元 | 行数 | 已移植方法数/总方法数 | DFM/类 对账 | 状态 |
|---|---|---|---|---|
| `LogDataServer/ThreadPool.pas` | 445 | **32/32**（`Pool/ThreadPool.cs`） | 3 类 + 1 类引用类型 + 1 事件类型，全部 1:1 | ✅ 完成 |
| `LogDataServer/FileSearchPool.pas` | 558 | **28/28**（`Pool/FileSearchPool.cs`） | 7 类型 + 1 自由函数，全部 1:1 | ✅ 完成 |
| `LoginSrv/GrobalSession.pas` | 93 | **3/3**（`Forms/GrobalSession.cs`） | DFM 4 object → 实例化 4 ✅；绑定 1 → `+=` 1 ✅ | ✅ 完成 |
| `GameCenter/GLoginServerRouteSet.pas` | 35 | 0（死代码 + 已移植，见 §0.2） | DFM 1 控件（既有实现里已有） | ✅ 判定完成（**不建文件**） |
| `GameCenter/GHeroDBConfig.pas` | 565 | **10/10**（`Forms/GHeroDBConfig.cs`，1193 行） | DFM object 25 → 实例化 25 ✅；绑定 6 → `+=` 6（拆"有名控件 5 + 无名 Raize 内嵌按钮 1"）✅ | ✅ 完成 |
| `LoginSrv/MasSock.pas` | 1,017 | **21/22**（`Forms/MasSock.cs`，1553 行） | DFM object 2 → 实例化 2 ✅；绑定 6 → `+=` 6 ✅ | ✅ 完成 |
| `DBServer/uFrmRoleDataEdit.pas` | 974 | **15/15**（`Forms/uFrmRoleDataEdit.cs`，1794 行） | DFM object 88 → 88（86 控件树 + 2 非可视组件）✅；绑定 35 → `+=` 35 ✅ | ✅ 完成 |

**收口口径**：7 个单元**全部收口**，其中 **6 个按"完成"**、**1 个（`GLoginServerRouteSet`）按"死代码 + 已移植（重命名类）"裁定且不建文件**（§0.2）。
`uFrmRoleDataEdit` 的**能力面**有如实登记的未覆盖项（§4.2 PARTIAL），**例程面 15/15 无缺口**。

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

### 1.4 ★ DFM object 数 ↔ `.pas` 控件字段声明数 双向对账（车道方独立取证，§37.3）

对三个窗体单元做了**双向集合差**核对（`DFM 子对象集` vs `.pas` class 块内的 `Name: TType;` 字段集）：

| 单元 | `.pas` 控件字段 | DFM object（含窗体根） | DFM 子对象 | 仅 DFM 有 | 仅 .pas 有 |
|---|---|---|---|---|---|
| `DBServer/uFrmRoleDataEdit` | **87** | **88** | **87** | **（空）** | **（空）** |
| `GameCenter/GHeroDBConfig` | **24** | **25** | **24** | **（空）** | **（空）** |
| `LoginSrv/GrobalSession` | **3** | **4** | **3** | **（空）** | **（空）** |

⇒ 三者的"DFM 声明数 = `.pas` 字段数 = 待实例化数"三者相等；`MasSock.dfm` 为二进制，
其 2 个 object（窗体根 + `TServerSocket MSocket`）与 `.pas:26-27` 的 1 个字段声明一致（+1 窗体根）。
这是一条**独立于实现**的对账基线：托管侧 `CountDfmObjects()` 必须等于上表第 3 列 + 1。

### 1.5 `MasSock.pas` 逐例程（**21/22**，`Forms/MasSock.cs`）

| 类/单位 | 原文例程数 | 已移植 | 说明 |
|---|---|---|---|
| `TFrmMasSoc`（interface 段 18 条） | 18 | **18** | `FormCreate` / `FormDestroy` / `MSocketClientConnect` / `MSocketClientDisconnect` / `MSocketClientError` / `MSocketClientRead` / `SortServerList` / `RefServerLimit` / `LimitName` / `LoadUserLimit` / `LoadServerAddr` / `CheckReadyServers` / `SendServerMsg` / `SendServerMsgA` / `IsNotUserFull` / `ServerStatus` / `GetOnlineHumCount` / `StartService` |
| 实现段自由函数 | 3 | **3** | `CheckAccountValid` / `CheckStringValid` / `CheckStringValid2` |
| `{$IFDEF LOG_SESSION}` 块（`:67-130` + `:770-775` + `:788-793` + `:1009-1015`） | 1 过程 + 1 类型 + init/final | **0（有意不移植）** | ★ **计数取证**：全 `Source` 树（`*.pas/*.dpr/*.inc`）grep `LOG_SESSION` = **14 处**，全部是 `{$IFDEF LOG_SESSION}` 指令或被注释掉的 `{.$DEFINE LOG_SESSION}`（`MasSock.pas:67`、`M2Engine/Forms/IdSrvClient.pas:73` 均为注释态），**生效 `{$DEFINE}` = 0 处** ⇒ 从未参与编译 |
| `CheckAccountValid` 的调用点 | — | — | ★ 唯一调用点在 `:389-398` 的 `{ }` 注释块内 ⇒ **死代码**（本车道仍按"逐条移植"保留该函数本体） |
| 合计 | 22 | **21** | 唯一 ❌ = `LogSession`（条件编译，见上） |

DFM 对账：**object 2 / 绑定 6**，托管 `CountDfmObjects()==2`、`CountChildrenOf()==1`、`+=` 6
（窗体 `Load`=OnCreate + `FormClosed`=OnDestroy，`MSocket` 4 条）。子车道另有
`Dfm_Bindings_ActuallyInvokeFormHandlers`（经接缝 `Raise*` 驱动 6 条绑定，证明**真接上**而不只是计数）。

### 1.6 `GHeroDBConfig.pas` 逐例程（**10/10**，`Forms/GHeroDBConfig.cs`）

| 单位 | 原文例程数 | 已移植 | 例程清单 |
|---|---|---|---|
| `TFrmHeroDB`（interface 段 8 条） | 8 | **8** | `EditHeroDBPathButtonClick` / `ButtonSaveHeroDBConfigClick` / `ButtonCloseClick` / `ButtonCreateStdItemsFieldClick` / `ButtonMagicFieldClick` / `ButtonMonsterFieldClick` / `Open` / `CheckHeroDB` |
| 实现段自由函数 | 2 | **2** | `SelectDirCB`(:52-57) / `SelectDirectory`(:59-110) |
| 合计 | **10** | **10** | 无 ❌、无 PARTIAL |

DFM 对账（计数取证）：
- **object 25 = DFM 25 vs 实例化 25** —— `DfmReconcile_25Objects_FormPlus24Children` + `DfmReconcile_ControlNamesMatchDfmOrder`（**逐名逐序 24 项**）。
- **绑定 6 = DFM 6 vs 托管 6** —— `DfmReconcile_SixOnClickBindings_PlusClosed`（拆为"有名控件 5 条 + 无名 Raize 内嵌按钮 1 条"，逐控件 `IsBound`）+ `DfmReconcile_RzButtonEditInnerButtonIsUnnamed_NotCountedAsObject`（`Name == ""` ⇒ 不污染 object 计数，但仍断言其 `Click` 真挂上）。
- **否定性断言**：`DfmReconcile_FormRootHasNoOnCreateOrOnDestroy` 用计数钉住窗体根**确无** DFM 事件（`Load`/`FormClosed`/`FormClosing`/`Shown` 全 False）⇒ **证实本报告 §0.6 的提取结论无需修正**。
- 3 条 WinForms 行为前提固化：`TabPage_VisibleOnlyForSelectedPage`、`TabPage_VisibleIsNotTheTabVisibleProperty`、`SetTabVisible_DoesNotTouchTabPageVisibility`。

### 1.7 `uFrmRoleDataEdit.pas` 逐例程（**15/15**，`Forms/uFrmRoleDataEdit.cs`）

| 单位 | 原文例程数 | 已移植 | 例程清单 |
|---|---|---|---|
| `TFrmRoleDataEdit`（interface 段 14 条） | 14 | **14** | `ButtonExportDataClick`:720 / `edtPasswordChange`:842 / `FormCreate`:936 / `ButtonSaveDataClick`:941 / `DoOpen`:189 / `RefreshShow`:325 / `RefreshBaseInfo`:227 / `RefreshMagicInfo`:339 / `RefreshUserItems`:380 / `RefreshFenghaoItems`:584 / `RefreshStorages`:661 / `RefreshUserVar`:703 / `ProcessSaveDataToFile`:736 / `ProcessLoadDataformFile`:769 |
| 单元级过程（实现段） | 1 | **1** | `ShowFrmRoleDataEdit`:164 |
| 单元级常量 | 1 | **1** | `TItemWhereNames`(:131-162) —— ★ **实测 30 项**（`array[Low(THumanUseItems)..High(THumanUseItems)]` = `0..29`）。**本报告 §0 派发时的"31 项"是错的，以此处 30 为准**（子车道以源文件计数纠正） |
| 合计 | **15** | **15** | 无 ❌ |

DFM 对账（计数取证）：
- **object 88 = DFM 88**：`DFM对账_88个object节点与35条事件绑定` —— `CountDfmObjects(form)==86`（窗体 + 85 具名控件）+ `SaveDialog`/`OpenDialog` 两个 `TComponent` = **88**；**双向差集为空**；`DFM对账_全部控件名与字段名逐字同名`（85 具名控件 + 窗体，反射取证"DFM 名 == public 字段名"）。
- **绑定 35 = DFM 35**：`CountEventBindings(form)==35`（修复 B-P10-21 后工具口径直接相等；修复前为 31，见下）；`DFM对账_31个控件挂到edtPasswordChange且事件类型与DFM一致`（表 31 行逐控件 1 条）+ `DFM对账_三个按钮各一条Click绑定_导入按钮绑的也是ButtonExportDataClick`。
- ★★ **工具盲区（B-P10-21，本车道已修）**：`P10FormReconcile` 的 `TryKeyed` **不查** `KnownBackingFieldAliases`，而 .NET 8 里 `Control.TextChanged` 的 `EventHandlerList` 键名是 **`s_textEvent`**（归一化得 `text` ≠ `TextChanged`）⇒ 该事件**恒计 0（假绿）**，把 DFM 的 35 条数成 31 条。子车道**没有**擅自改共享工具（守纪律），只在测试侧补独立第二口径 `TextChangedBindingCount` 并同时断言"工具计 0、真实 1"；**收口时我已把三份副本的 `TryKeyed` 都补查别名表并登记 `[(Control,"TextChanged")] = ["s_textEvent","EventText"]`**（切片 `e90f1c47`），工具口径随即由 31 → 35 与 DFM 一致。
- **`ButtonImportData.OnClick = ButtonExportDataClick` 判定**：**不是缺陷** —— `:722/:726` 明确按 `Sender` 分支，导入按钮是**有意复用**同一处理器；已用 `RaiseClick(ButtonImportData)` 行为锁定走的是导入路径。

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
| 16 | `uFrmRoleDataEdit.pas:876-883` `edtPasswordChange` | ★★ `seHomeX` 分支之后写的是 **`else if Sender = seCurY then FHumData.wHomeY := seHomeY.Value;`** —— 判据用的是 **`seCurY`**（`:868` 已经判过一次 ⇒ 该支**永不可达**），而 `seHomeY` **根本没有分支** | **改 `seHomeY` 什么都不会发生**（`wHomeY` 永不更新）；`seCurY` 只更新 `wCurY`。典型复制粘贴缺陷 | 子车道用例（断言 `Server=seHomeY` 时 `wHomeY` 不变 + `seCurY`/`seHomeY` 两个 sender 的实际效果） |
| 17 | `uFrmRoleDataEdit.pas:821-827` `ProcessLoadDataformFile` | 英雄分支里 `GetMem(ReadBuf, SizeOf(**THumData**))`，却 `FileRead(..., SizeOf(**THeroData**))` —— **按 THumData 分配、按 THeroData 读取** | 若 `SizeOf(THeroData) > SizeOf(THumData)` ⇒ **堆越界写**（内存破坏）；反之只是多分配 | 子车道用例（用 `StructBytes.SizeOf<THumData/THeroData>()` 把两者大小关系钉死；越界本身无法在托管侧复现 ⇒ 用"分配尺寸取自哪个类型"的结构断言锁死） |
| 18 | `uFrmRoleDataEdit.pas:802-806 / 823-827` | 读文件失败时 `Exit` —— `finally` 只 `FreeMem(ReadBuf)`，**`FileClose(nFileHandle)` 在 `Exit` 之后（`:837`）⇒ 文件句柄泄漏** | 每次导入失败泄漏一个文件句柄 | 子车道用例（失败路径后句柄计数/可再次打开同一文件） |
| 19 | `uFrmRoleDataEdit.pas:749-752` `ProcessSaveDataToFile` | 目标文件已存在时用 `FileOpen(..., fmOpenReadWrite)`（**不截断**）后从 0 写 `SizeOf(THumData)` 字节 | 新记录比旧文件短时**尾部残留旧字节** ⇒ 导出的文件比记录长、再导入会被多读 | 子车道用例（先写长文件再导短记录，断言长度 == SizeOf） |
| 20 | `uFrmRoleDataEdit.pas:730-733` | `ButtonExportDataClick` 里 `Sender = ButtonSaveData` 那一支是**空体**，而 `ButtonSaveData` 的 `OnClick` 绑定的是 `ButtonSaveDataClick` ⇒ 该支**死代码** | 无（仅证明"共用处理器"的写法不完整） | 子车道计数用例（35 条绑定地图 + 该处理器只被 2 个按钮触发） |

> `MasSock.pas` 的原文缺陷单列于 §2.1（由子车道逐条编号 F1–F19，含本报告 §2 早期登记的 5 条同源项）。

### 2.1 `MasSock.pas` 缺陷全表（子车道编号 F1–F19；每条的"锁死用例"均已在 `P10MasSockTests.cs` 落地并全绿）

| # | 位置 | 缺陷 | 后果 |
|---|---|---|---|
| F1 | `:628` | `sReceiveMsg := sReviceMsg` 写在 `if MsgServer.Socket = Socket` **之外**（在 `for` 循环体内） | 命中项**之后**的所有服务器条目的未完成报文缓冲被同一份 `sReviceMsg` 覆盖（首次未命中时是 `""`）⇒ 多服务器并发**串包/丢包**。**本单元最严重** |
| F2 | `:383` `:622` | `Exit` 退出**整个过程**（不是跳出 case/for） | 后续服务器条目的 `:628` 回写被跳过（与 F1 组合出更乱的缓冲状态） |
| F3 | `:820` | `sLineText[I]` —— 用**文件行号**当 1-based **字符**下标判 `;` 注释行（本意应为 `sLineText[1]`） | 只有第 1 行判得对；其余行的注释判断落在随机位置 |
| F4 | `:826` `:830` | `g_ServerAddrCount := nServerIdx` 写在 `for` 体内 | ① 文件为 0 行时**不赋值**（保留旧计数）；② 满 100 `break` 时计数停在 **99**（差一） |
| F5 | `:939` | `UserLimit[nC]` 无上界检查（`array[0..99]`） | 超过 100 行 ⇒ **写穿静态数组**（Delphi 内存破坏） |
| F6 | `:917` | 重载 `!UserLimit.txt` 前**不清空** `UserLimit` | 旧条目残留，`LimitName` 仍可能命中已删除的限流项 |
| F7 | `:949` | 文件缺失只 `ShowMessage`，`nUserLimit`/`UserLimit` **都不重置** | 启动路径上的**阻塞模态框** + 状态不一致 |
| F8 | `:642` | `m_ServerList.Free` 之后**不置 nil** | 悬垂字段；再访问即 AV |
| F9 | `:150` `:669` `:752` `:797` `:854` `:887` `:911` `:1004` | **空 `except`** 吞掉一切异常（只留一行日志） | 真正的故障被静默 |
| F10 | `:979-999` | `ServerStatus` 用整数 `div` 分级；`Max=0, Min=0` ⇒ **恒报"1 空闲"** | 限流为 0 的服务器被显示为空闲 |
| F11 | `:541-556` | 手机号"非数字"标志**取反**，且与上面的禁字符检查**共用错误码 -13** | 非数字手机号被**当成合法**；两种不同错误无法区分 |
| F12 | `:389-398` | 帐号名校验整块**被注释掉** | 非法帐号名（长度/字符集）照样通过 |
| F13 | `:782` `:885` | `MsgServer.Socket` 未判空直接解引用 | `nil` 项让**整轮循环中断**（异常被 F9 吞掉） |
| F14 | `:784` vs `:658/:684/:967/:977` | 同一次匹配里 `:784` 用 `CompareText`（忽略大小写），其余四处用 `=`（区分大小写） | 服务器名匹配语义不一致 |
| F15 | `:165` | 白名单循环上界 `g_ServerAddrCount - 1` 未夹紧到 100 | 计数越界时读越界内存 |
| F16 | `:686` | `nLimitCountMin > nLimitCountMax` 才判"满" | **正好满员时仍报"未满"** |
| F17 | `:934-937` | 全分隔符行（`GetValidStr3` 返回空）→ 生成 `sServerName = "   "` 的垃圾条目 | 空白条目污染限流表 |
| F18 | `:199` | `MsgServer.Socket = Socket` 判等：`nil = nil` 成立 | 传 `nil` 的 Disconnect 会**删掉第一个 nil 项** |
| F19 | `:300-303` | 未命中 `(` 时 `ArrestStringEx` 返回原串（原文 `Result := Source`） | 缓冲区**永不丢弃**无 `(` 的垃圾 ⇒ 无限增长 |

**F19 连带发现（跨区，见 B-P10-17 —— ✅ 已关闭）**：托管 `GXX.Core.Util.HUtil32.ArrestStringEx`
原先把 `result` 初值写成 `""`，且两条"未找到"路径都返回 `""`；而原文 `HUtil32.pas:1761-1805`
是 `Result := Source`（`:1766`）+ 未找到 `SearchEnd` 时**不动 `Result`**、`ArrestStr := ''`。
**两条路径语义都不同** ⇒ 子车道当时在本区逐字复刻了一份 `MasSockFns.ArrestStringExAnsi`（D-P10-17）。
**收口状态**：集成方已按原文修 Core（`Result := Source` 起手 + 删掉 `else` 改写，台账 §48.2），
并把 `main` 并入本车道（`b671a662`）⇒ 本车道**已删除该复刻、改回直接转调
`HUtil32.ArrestStringEx_Ansi`**（切片 `76f2f0cc`，D-P10-17 随之退役）；F19 的差异断言改为
锁"Core 现在与原文一致"（`ArrestStringEx_NotFoundPath_KeepsSource_BP10_17Closed`）。

### 2.2 `GHeroDBConfig.pas` 缺陷表（子车道逐条；每条的锁死用例均已全绿）

| # | 位置 | 缺陷 | 锁死要点 |
|---|---|---|---|
| G1 | `:158/:387/:489/:526/:557` | `g_boHeroDBOK := not CheckHeroDB`（OK = **无**缺失字段）之后才弹成功框并 `Close`（双重否定） | 4 个 `*Click_*` 用例（成功/失败两侧都断言） |
| G2 | ★★ **`:338`** | 唯一一处用 **`TabSheet3.Visible`**（`Control.Visible`）而非 `TabVisible`；而 `ActivePage` 恒为 `TabSheet2` ⇒ 该条件**恒成立** ⇒ `:340-371` 的 Magic 字段检查**无条件执行**（即使刚判定 Monster 缺字段、本该短路） | `CheckHeroDB_TabSheet3VisibleGuardIsDead_MagicChecksRunAnyway`（断言 `TabVisible=True` 而同刻 `Visible=False`，且 `ListBoxMagic=[NeedL1]`） |
| G3 | `:511` | `sFieldName[1]` Delphi 1-based 索引 + **无空串守卫** ⇒ `EStringIndex` | `MagicFieldValue_EmptyFieldName_ThrowsLikeDelphiEStringIndex`、`ButtonMagicFieldClick_EmptyItemString_ThrowsLikeDelphiEStringIndex`（托管抛 `IndexOutOfRangeException` 对应） |
| G4 | `:340` vs `:290` | Magic 循环 `for I := 0 to 14` + `I+1` ⇒ `NeedL1..15`/`L1Train..15Train`；StdItems 循环 `for I := 1 to 24` ⇒ `Element1..24`（两处风格不一致） | `CheckHeroDB_MagicLoopGeneratesNeedL1To15AndTrain1To15`（33 项，且断言**无** `NeedL0`/`NeedL16`）、`CheckHeroDB_StdItemsCheckCoversExactly38Fields`（断言**无** `Element25`） |
| G5 | `:464/:469` vs `:474` | `ButtonCreateStdItemsFieldClick` 里 **Element 循环写在 InsuranceCurrency/InsuranceGold 之后**（日志行序 = 14 具名 → 24 元素），而 `CheckHeroDB` 的 Element 循环在 Insurance **之前** ⇒ 两个方法的字段顺序**不一致** | `ButtonCreateStdItemsFieldClick_AllFail_Logs38FailuresAndReenablesButton`（38 行逐字快照 + 38 次 `CreateField` 的"字段:值:长度"快照） |
| G6 | `:114` | `var sFilePath: string;` 未初始化（Delphi 受管串初值 `''`；托管 `string?` 默认 null 会 NRE） | `EditHeroDBPathButtonClick_*` |
| G7 | `:548` | `nValue := 0` 写在**循环体内** | `ButtonMonsterFieldClick_ZeroValueLen4AndLogLines` |
| G8 | `:104` | `SelectDirectory` 内部**不**剥尾反斜杠（剥除在调用方 `:117-118`/`:141-142`） | `SelectDirectory_Chosen_CopiesVerbatimWithTrailingBackslash` |
| G9 | `:71-72` | 传入目录不存在则先 `Directory := ''`（**清掉调用方传来的值**） | `SelectDirectory_NonExistentInput_ClearsDirectoryBeforePicking` |
| G10 | `:509-516` | `SameText(...)` 大小写不敏感，但首字符判据 `= 'M'`/`= 'N'` **区分大小写** ⇒ `'m'`/`'n'` 落到 `else → 200` | `MagicFieldValue_RulesAreLocked('m'→200, 'n'→200)` |
| G11 | `:394` vs `:489` | `MemoLog1` 被 `Clear` **两次**（结尾 `CheckHeroDB` 再清一次）⇒ 方法返回后日志**为空** | 三个 Memo 一律用"过程中**最长**快照"断言（否则会得到假绿）；测试内已注释原因 |

### 2.3 `uFrmRoleDataEdit.pas` 缺陷表（子车道逐条 + 本报告独立登记项的交叉核对）

| # | 位置 | 缺陷 | 锁死要点 |
|---|---|---|---|
| R1 | ★★ `:876-883` | `seHomeY` 已绑定却**无分支**；`:880` 第二个 `else if Sender = seCurY`（`:868` 已判过）是**死分支** ⇒ `wHomeY` 在任何 UI 路径下都写不进去 | `原始缺陷_seHomeY已绑定却没有分支_改动不写回wHomeY`、`原始缺陷_31个已绑定Sender逐一试过都写不到wHomeY`（31 条绑定逐一当 Sender 试，计数取证） |
| R2 | `:842-934` | 11 个已绑定控件**无分支**（10 个 `Edit*` 属性点 + `seHomeY`） | `原始缺陷_十个属性点控件与seHomeY共11个绑定控件无分支` |
| R3 | `:891-932` | 9 个分支**无 `if FIsHuman` 守卫** ⇒ 英雄模式下仍写 `FHumData` | `原始缺陷_英雄模式下九个无守卫分支仍写FHumData` |
| R4 | ★★ `:802` `:823` | `if not FileRead(...) = SizeOf(...)` —— Delphi 一元 `not`（优先级 1）**高于** `=`（4）⇒ 实为 `(~nRead) = Size`，**恒 False、错误分支不可达** ⇒ 短读也照样走到 `:814` 赋值 + `:839` 提示"导入成功" | `原始缺陷_短读的错误分支不可达_照样提示导入成功`、`ProcessLoadDataformFile_零字节文件同样走成功分支`（同时断言 `OpenHandleCount==0`：正因不可达，`:805` 的 `Exit` 才没跳过 `:837 FileClose`）。⚠ **该优先级结论未经 Delphi 编译器实测**（本车道无 Delphi 工具链，仅有语言规范 + FPC 手册 `Not 1 = -2` 佐证）⇒ 若被推翻，只需改 `ProcessLoadDataformFile` 的两处 `(~nRead) == size` 与两条用例 |
| R5 | `:730-733` | `Sender = ButtonSaveData` 分支是**空体死代码**（`ButtonSaveData` 绑的是另一个处理器） | `原始缺陷_ButtonExportDataClick里ButtonSaveData分支是死代码` |
| R6 | `:661-701` | `RefreshStorages` **无 `FIsHuman` 分支** ⇒ 英雄模式也读 `FHumData.StorageItems` | `RefreshStorages_没有FIsHuman分支_英雄模式也读FHumData` |
| R7 | `:749-752` | 导出时目标文件已存在走 `FileOpen`（**不截断**）⇒ 新记录比旧文件短时残留旧尾 | `ProcessSaveDataToFile_目标文件已存在时走FileOpen覆盖前段` |
| R8 | `:206` `:212` | `Length(静态数组)` **恒 500**（不是"已用个数"） | `DoOpen_人类_标题栅格与TabVisible`、`RefreshUserVar_未扩RowCount时越界静默丢弃` |
| R9 | `:395` | `wIndex = 0` **或** `MakeIndex = 0` 都跳过 | `RefreshUserItems_wIndex或MakeIndex为零一律跳过` |
| R10 | `:938` | `FormCreate` 只是重复写 DFM 已有的 `MaxValue=65535`（无效果） | `FormCreate_把seLevel上限设为HighWord` |
| R11 | `:314` | `//edtAccount.Text := FHeroData.sAccount;` 被**注释掉** ⇒ 英雄模式不填账号 | `RefreshBaseInfo_英雄_只填英雄字段且人类字段清零` |
| R12 | `:821` | 英雄分支按 `SizeOf(THumData)` 申请读缓冲（**只浪费、无越界**：645KB ≥ 316KB） | 由 `ProcessLoadDataformFile_英雄_只保留两个字段其余全取文件` 覆盖 |

> **与本报告 §2 早期独立登记的交叉核对**：我在 §2 登记的 #16（`seHomeY` 分支不可达）与 #20（空体死分支）**与 R1/R5 独立互证**；
> #17（`GetMem(SizeOf(THumData))` 却按 `SizeOf(THeroData)` 读）经子车道实测**只浪费不越界**（645KB > 316KB），**降级为 R12**；
> #18（读失败 `Exit` 泄漏文件句柄）**被 R4 推翻并深化**——因为 `not ... = ...` 恒 False，那条 `Exit` **根本不可达**，
> 所以句柄其实**不会**泄漏（子车道用 `OpenHandleCount==0` 反向取证）。★ 这是本车道**"我自己的初判被实现否掉"**的一例，按 §34.4 先取证再断言处置。
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

### 3.1 `MasSock.cs` 的偏离（D-P10-16 … D-P10-23；编号已落在代码注释里，报告与代码一致）

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P10-16 | `MasSock.pas:6-7` `JSocket` | `TServerSocket` / `TCustomWinSocket` / `TErrorEvent` / `TServerType` | 在 `MasSock.cs` 内声明接缝：`TServerSocket` **派生自 `System.Windows.Forms.Control`**（DFM 本就给了 `Left=40/Top=32`，且只有组件化才能被 `CountDfmObjects` 数到 2/6）、`TCustomWinSocket`（`RemoteAddress`/`Connected`/`ReceiveText`/`SendText×2`/`Close`）、`TErrorEvent`、`TServerType`、`TClientSocketEventArgs`、`Raise*` 事件驱动面、`TFrmMasSoc.SocketFactory` | 托管无 JSocket 等价物；默认接缝在 `Active := True` 时**显式抛"未接线"**（`False` 允许 = DFM 初值），符合 §25.2 |
| D-P10-17 | `MasSock.pas:302` 调用的 `ArrestStringEx` | `HUtil32.pas:1761-1805`（`Result := Source` 起手） | **已退役**：Core 修正后本区删除逐字复刻，直接转调 `HUtil32.ArrestStringEx_Ansi`（`76f2f0cc`） | 托管 `HUtil32.ArrestStringEx` 原先两条"未找到"路径都返回 `""`，与原文语义不同（F19）；B-P10-17 已由集成方修 Core 并合并（`b671a662`）⇒ 复刻失去存在理由（§37.7 的"正式归属落地后去掉替身"闭环） |
| D-P10-18 | `MasSock.pas:376/378` `SizeOf(TAccountInfo2)` | `SizeOf` | 显式常量 `TAccountInfo2PackedSize = 218` | 避免 `unsafe sizeof`；**实测托管 `sizeof(TAccountInfo2)==218` 且字段偏移完全重合**（原以为需补 `Pack=1`，实测不需要）⇒ 无需跨区改动 |
| D-P10-19 | `:810` `'.\!ServerAddr.txt'`、`:926` `'.\!UserLimit.txt'` | 硬编码相对路径 | 接缝 `ServerAddrFileName` / `UserLimitFileName`，默认值即原文字面量 | 测试可指向 `Path.GetTempPath()`，不碰真实数据目录 |
| D-P10-20 | `:784` `CompareText` / `:410` `SameText` | Delphi 大小写不敏感比较 | `StringComparison.OrdinalIgnoreCase` | `GXX.Core` 无托管 `CompareText`；沿用工程既有处置 |
| D-P10-21 | `:642` `m_ServerList.Free`（不置 nil） | 悬垂字段 | `m_ServerList = null` 表达悬垂态 | 托管无"已释放但仍非 nil"的对象态；用 null 精确表达"悬垂" |
| D-P10-22 | `:820` `sLineText[I]` | Delphi `AnsiString` 的 1-based 越界读（0 号与超长位置行为未定义） | `MasSockFns.AnsiStringCharAt(s, index)` 复刻（越界 → `'\0'`） | 让 F3 的原文语义在托管侧可稳定断言 |
| D-P10-23 | `:939` `UserLimit[nC]`、`:165` `g_ServerAddr[I]` | 内存越界/垃圾读 | `IndexOutOfRangeException` | 托管数组必做边界检查；原文是内存破坏（登记为 F5/F15，用异常类型差异断言） |

> ⚠ **偏离编号冲突的处置（已执行）**：三个单元由并行子车道各自"从 D-P10-15 续编"，因此 `GHeroDBConfig`
> 也用了 `D-P10-17…20`（与 MasSock 撞号）。收口时已把 **`GHeroDBConfig` 的偏离重编号到 `D-P10-24…28`
> 并同步改了它的代码/测试注释**（本报告与代码始终一致）。最终编号分区见 §3.2。

### 3.2 编号分区总表（避免撞号；本表为唯一真源）与 `GHeroDBConfig` 的偏离

| 编号范围 | 归属单元 |
|---|---|
| `D-P10-01 … D-P10-15` | 车道级 + `Pool/*` + `GrobalSession`（§3） |
| `D-P10-16 … D-P10-23` | `MasSock`（§3.1，已落在其代码注释里） |
| `D-P10-24 … D-P10-28` | `GHeroDBConfig`（下表，已落在其代码/测试注释里） |
| `D-P10-29 …` | `uFrmRoleDataEdit`（子车道；其代码注释里已按 `D-P10-29…37` 落盘，见 §3.3） |
| `B-P10-01 … B-P10-15` | 车道级跨区事项（§4.1） |
| `B-P10-16 … B-P10-19` | `MasSock` 跨区事项（B-P10-17 已关闭） |
| `B-P10-20 … B-P10-29` | `uFrmRoleDataEdit` 跨区事项 |
| `B-P10-30 …` | `GHeroDBConfig` 的报备项（下表/§4.1） |

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P10-24 | `GHeroDBConfig.pas:20` `TRzButtonEdit EditHeroDBPath` | Raize 复合控件（`OnButtonClick`） | `TextBox` + 新字段 `EditHeroDBPathButton`（**刻意不设 `Name`**），`OnButtonClick` → 内嵌按钮 `Click` | 托管无 Raize；沿用 `GMainForm.Fields.g.cs:387-390` 既有惯例。★ 不设 `Name` 是为了让 `CountDfmObjects()` 仍等于 DFM 的 25（DFM 里没有该内嵌按钮的 `object` 节点），并有专门用例钉住"无名但仍真挂事件" |
| D-P10-25 | `:52-110` `SelectDirCB` / `SelectDirectory` | `ShBrowseForFolder` + `IMalloc` + `BFFM_SETSELECTION` 预选回调 + `DisableTaskWindows/EnableTaskWindows` | `SelectDirCB` 只保留**回调契约**（恒返回 0、仅 `BFFM_INITIALIZED` 分支、`lpData=0` 不发消息）；预选意图改由 `FolderPickerProvider(initialDirectory)` 承载；`Root`(pidlRoot)/`Owner`(hwndOwner) 仅签名保真；`DisableTaskWindows` 为 VCL 专有**不复制** | 托管没有"向 Shell 对话框回调发消息"的等价通道；默认实现是真实 `FolderBrowserDialog`，**测试全部注入替身**（不弹窗） |
| D-P10-26 | `:186-189` `TabSheetN.TabVisible` | `TTabSheet.TabVisible` | 窗体自有状态 `_tabVisibleState`，与 `Control.Visible` **完全解耦**；**代价：页签行始终显示 4 个页签（纯视觉）** | ★ 实测三次：WinForms 写 `TabPage.Visible` **不摘页签**，而"摘/追加页签"会让 `TabPage.Visible` 与选中页**脱钩**（容器只剩孤页时翻 True）⇒ 会**破坏原文缺陷 G2（`:338`）的条件语义、误跳过 `:340` 的 Magic 检查**。所有分支判定/弹窗/落盘行为与原文一致 |
| D-P10-27 | `GHeroDBConfig.dfm` 窗体根 | DFM **无** `OnCreate/OnDestroy` | 仍挂 `Closed += (s,e) => CloseCalled = true`（**非 DFM 事件**的纯观测绑定，不计入 DFM 绑定数） | 用于观察原文 `:161/:388/:492/:530/:562` 的 `Close` 是否被调用；"窗体根无 DFM 事件"另由计数用例独立钉死。断言 `CloseCalled` 的用例会先 `f.Show()`（WinForms 对未显示窗体的 `Close()` 是 no-op） |
| D-P10-28 | `GHeroDBConfig.dfm:72/115/149/192`（4 个 `TMemo`） | VCL `TMemo` 无字数上限 | 4 个 Memo 统一 `MaxLength = 0` | WinForms `TextBox` 默认 `MaxLength=32767` 会**静默截断**（本窗体日志可达 38 行/1.6k+ 字符）⇒ 不置会让"日志行快照"断言出现假绿/假红 |

### 3.3 `uFrmRoleDataEdit` 的偏离（D-P10-29 … D-P10-37；编号已落在其代码/测试注释里）

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P10-29 | `:287` `:320` `RefreshBaseInfo` 喂给 `TSpinEditEx` 的记录值 | `TSpinEditEx.CheckValue` **静默裁剪**到 [Min,Max] | `ClampSeLevel` 显式裁剪到 `[0, 65535]` 后再赋值 | ★ 托管 `TSpinEdit.Value`（`SpinControls.cs:29-33`）把 Min/Max 当**硬边界**，越界赋值**抛** `ArgumentOutOfRangeException`（实测 `Maximum=65535` 后赋 70000 即抛），而原文是**静默裁剪**；Level 来自 DB 记录且 GXX 是"21 亿"改版 ⇒ 不可不处理。**这是还原原文语义，不是修正原文**。用例同时锁住"壳会抛"与"本单元裁剪"。根治见 B-P10-20 |
| D-P10-30 | `:183` `ShowModal` | 阻塞式模态 | `ShowModalHandler` 接缝 + `ShowModalEquivalent()`（默认 null ⇒ 返回 false、**不阻塞**） | 派发约定；无头测试不能挂死 |
| D-P10-31 | `:21-22` `TSaveDialog`/`TOpenDialog` | `Execute` 返回 Boolean | `RoleDataEditFileDialogSeam.SaveDialogExecute/OpenDialogExecute`（返回 **bool** = 原文 `Execute` 的返回值） | 让窗体侧 `if not X.Execute then Exit; s := X.FileName;` **可逐字保留**；默认弹真对话框，单测注入替身 |
| D-P10-32 | 多处 | 托管侧缺失的类型 | 就地声明：`TSpinEditLongWord`（`NumericUpDown` 壳，`[0, High(LongWord)]`）、`TStringGrid`（`DataGridView` + `Cells/SetCells` **越界静默**，VCL `GetEditText/SetEditText` 语义）、`TTabSheet`（单独承载 `TabVisible`）、`DelphiFileIo`（`System.pas` 的 `FileOpen/FileCreate/FileRead/FileWrite/FileClose` 句柄表） | 原文依赖的 VCL/System 类型在托管侧无等价物；均按分区内自带 |
| D-P10-33 | `RefreshMagicInfo`/`RefreshUserItems` | `GetMagicName`/`GetStdItemName` | `RoleDataEditDbShareSeam.GetMagicName/GetStdItemName`，默认**抛"未接线"** | 台账 §25.2：接缝不得静默返回空串（否则"名字查不到"会被伪装成"字段为空"） |
| D-P10-34 | `:800` `:821` `GetMem(ReadBuf, …)` | 未初始化堆内存 | 零填充 `byte[]` | 托管无法复刻"未初始化内存"；登记以保证"读短了"的行为差异可追溯 |
| D-P10-35 | DFM 对话框组件属性 | `InitialDir` / `ofHideReadOnly` / `ofEnableSizing` / `ShowReadOnly` | `InitialDirectory`；WinForms 无对应项（`Options` 只映射了 `ofOverwritePrompt`） | 托管对话框能力面差异 |
| D-P10-36 | 单测基础设施 | — | 全部单测跑在 **16 MB 栈线程**（`Run(...)` / `BigStack`） | ★ 实测 `SizeOf<THumData>() = 660,377`（645KB）、`SizeOf<THeroData>() = 323,962`（316KB），而 xunit 默认线程栈 **1MB** ⇒ "按值返回/传参一个 `THumData`"就会打穿栈、`Stack overflow` **杀进程**（本车道实测复现并定位，见 §5 警示 W2）。**测试侧限定，产线窗体逻辑未改** |
| D-P10-37 | 全单元 | `TListView` / `OnChange` / `OnClick` / `Caption` / `ClientHeight·Width` / `bsSingle` / `poDesktopCenter` | 复用既有 `IListViewSink/ListViewSink`；`TextChanged·ValueChanged` / `Click` / `Text` / `ClientSize` / `FixedSingle` / `CenterScreen`（沿用 `uFrmDataManager.cs:65` 既有映射） | 不新造适配器（§14.2）；沿用本工程既有控件映射约定 |

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
| B-P10-07 | ★ **`unit-map.tsv` 的车道行收口**（按 §38.2/§41.10，车道一经合并必须回头清理）：<br>① `:123 GLoginServerRouteSet par/p10-db-login-forms` —— **应删行**：本单元是**死代码**（见 §0.2 六条取证）+ 已有重命名实现，不属于"在飞"，留着就是"永久占位行"；<br>② `:119-122`、`:124-125`（`uFrmRoleDataEdit`/`MasSock`/`GrobalSession`/`GHeroDBConfig`/`LogDataServer/ThreadPool`/`FileSearchPool`）—— 合并后**删行**（E1 同名 `.cs` 会接管）；<br>③ `FileSearchPool` 目前是**裸 basename** 行，全树只有一份副本（实测），可安全删；`LogDataServer/ThreadPool` 必须保留**逐副本键**（`SelGate/ThreadPool`、`LoginGate/ThreadPool` 仍是 VENDOR/not-ported，见 `tools/audit-coverage.ps1:90-100`） | 不清理则报表同时"假装在飞"与"不是缺口"（§41.10 的第 3 批系统性缺陷） |
| B-P10-08 | ★★ **`uFrmRoleDataEdit` 的跨模块接线需要架构裁定**（本车道**没有**擅自接线）：<br>原文里 `uFrmRoleDataEdit.pas` 是 **DBServer** 单元，却被 **LoginSrv** 的 `uFrmDataManager.pas:64` `uses`，并在 `:199/:207/:454/:462` 调用 `ShowFrmRoleDataEdit`。托管侧现状：`src/GXX.LoginSrv/RoleDBSeam.cs:80` 已有接缝 `public static Action<int, THumData?, THeroData?> ShowFrmRoleDataEdit`（`uFrmDataManager.cs:203/213/427/437` 已在调，`DataManagerFormTests.cs` 有 6 处在替换），**但那里的 `THumData`/`THeroData` 是两个空类**（`RoleDBSeam.cs:36/41`，注释："仅作为不透明句柄在窗体间传递"），而 `GXX.Core.Protocol.THumData/THeroData` 是 `unsafe struct`；且 `GXX.LoginSrv` **不引用** `GXX.DBServer`（两个独立 exe）。<br>⇒ 需要集成方二选一：**(a)** 把 LoginSrv 的接缝统一到 `GXX.Core.Protocol.THumData`（并把空句柄类退役）；或 **(b)** 由 LoginSrv 引用 DBServer 程序集（跨 exe 依赖，需评估）。**本车道只登记、不擅改** | 不裁定则 `uFrmDataManager` 的"编辑角色数据"按钮**永远打不开窗体**（且是静默空实现） |
| B-P10-16 | ★ `src/GXX.LoginSrv/LoginSrvShare.cs` 的三处接缝必须在 MasSock 落地后**退役**：`TMsgServerInfo`（`:287`，**只有 4 字段**）、`TMasSocSeam`（`:278`）、`LoginSrvShare.FrmMasSoc`（`:53`）。真源现在是 `GXX.LoginSrv.Forms.TMsgServerInfo`（**7 字段**：`sReceiveMsg/Socket/sServerName/nServerIndex/nOnlineCount/dwKeepAliveTick/sIPaddr`）+ `TFrmMasSoc` + `MasSockGlobals`。★ **`src/GXX.LoginSrv/MonSoc.cs:64-67` 现在读的是 4 字段接缝**，退役时需改指 `MasSockGlobals.FrmMasSoc.m_ServerList` | 不退役则 MasSock 与 MonSoc **长期各持一套服务器列表类型**（§14.2 家族，且 MonSoc 读到的是空实现） |
| B-P10-16b | `LoginSrvShare` 还应补 `g_ServerAddr` / `g_ServerAddrCount` / `nOnlineCountMin` / `nOnlineCountMax` / `GetSessionID()`（本区以 `MasSockGlobals` 接缝承载；`GetSessionID` 与 `CloseUser` 未接线时**显式抛"未接线"**） | 同上；这两处是真源码里的 `LMain.pas` 全局 |
| B-P10-17 | ~~`src/GXX.Core/Util/HUtil32.cs` 的 `ArrestStringEx` 初值应为 `Result := Source`~~ **✅ 已关闭**：集成方已按原文修正 Core（台账 §48.2）并把 `main` 并入本车道（`b671a662`）；本车道已删除 D-P10-17 的本地复刻、改回转调（`76f2f0cc`，LoginSrv 404 例全绿） | — |
| B-P10-18 | `TAccountInfo2` 的托管尺寸/偏移经子车道实测与原文一致（218 字节、偏移重合），**无需**加 `Pack=1` —— 登记以免后人误改 | 防误改 |
| B-P10-19 | `JSocket`（`TServerSocket`）**没有任何托管等价物** ⇒ `StartService` 的真实监听能力待接线（当前 `Active := True` 按 §25.2 显式抛） | MasSock 窗体可移植、可测试，但**还不能真的监听**；需集成方裁定用 `GatewayKit` 的 socket 设施还是新写 |
| B-P10-30 | `GHeroDBConfig` 的两条**报备项**（本车道未改任何区外文件）：<br>① `GameCenterDialogs` **缺 `MB_ICONWARNING`（0x30）常量** —— 本区以 `MB_OK + 0x30` 并在注释里说明；建议后续在公共文件补常量；<br>② 见 D-P10-28 的 `MaxLength` 处置（已在本区实现，仅报备"WinForms TextBox 默认截断"这一全局陷阱，其他窗体若用 `TextBox`/`RichTextBox` 当 `TMemo` 也应同样处置） | ① 影响所有需要"警告图标"消息框的窗体；② 影响面是**所有把 `TMemo` 译成 `TextBox` 的窗体**（静默截断会让长日志断言假绿/假红） |
| B-P10-20 | ★ `src/GXX.DBServer/SpinControls.cs`：`TSpinEdit.Value` 的 setter 应改回 Delphi 的**裁剪**语义（或让 `MaxValue` 只走 `DfmMaxValue` 而不成为 `NumericUpDown` 的硬边界） | 否则**任何"把 DB 记录喂进 SpinEdit"的窗体**都可能被记录里的越界值**抛 `ArgumentOutOfRangeException`** 打断（原文是静默裁剪）。本车道的 D-P10-29 只在 `uFrmRoleDataEdit` 内绕过，**根因未修** |
| B-P10-21 | ~~`P10FormTestKit.cs` 三份副本的 `TryKeyed` 应查 `KnownBackingFieldAliases` 并登记 `[(Control,"TextChanged")]=["s_textEvent"]`~~ **✅ 已关闭**：收口时已修三份副本（`e90f1c47`），工具口径由 31 → 35 与 DFM 一致；DBServer 871 例仍全绿 | 关掉了一类**假绿**（任何绑 `TextChanged` 的窗体对账此前都会被少数 4 条/控件） |
| B-P10-22 | `ShowFrmRoleDataEdit` 的形参用真身 `GXX.Core.Protocol.THumData?/THeroData?`，与 `src/GXX.LoginSrv/RoleDBSeam.cs:80` 的 `Action<int,THumData?,THeroData?>`（那里是**两个空类**句柄）**不是同一类型** | 与 B-P10-08 同源，需集成方一并裁定"统一到 `GXX.Core.Protocol.THumData`"还是"LoginSrv 引用 DBServer" |
| B-P10-23 | （风险提示）`THumData` 645KB / `THeroData` 316KB **值类型**：`ProcessLoadDataformFile` 的 `THumData ReadData` **局部量在 UI 线程栈上占 ~645KB**（原文 `GetMem` 在**堆**上）；本车道**未改**（保持 1:1） | 生产 UI 线程若同时有多个此类局部量，有**真实栈溢出**风险（不只是测试问题）；建议后续统一改 `ref`/堆暂存。见 §5 警示 W2 |

### 4.2 本车道未完成 / 未覆盖面（**如实登记**；供集成方按 PARTIAL 第三态处置）

**例程面**：7 单元 **0 缺口**（`MasSock` 的 1 条 `LogSession` 属条件编译块，计数取证后**有意不移植**；`GLoginServerRouteSet` 属死代码+已移植，**不建文件**）。

**能力/验证面**（`uFrmRoleDataEdit`，子车道如实申报）：

| 未覆盖面 | 说明 |
|---|---|
| 真实对话框路径 | `provider = null` 时的真实 `SaveFileDialog/OpenFileDialog.Execute` 无测试覆盖 |
| 真实模态/显示行为 | `ShowModalEquivalent()` 默认不显示窗体 ⇒ 真实 `ShowModal` 行为未验证 |
| 真实数据库 | `THumanDBBase.Save` → MySQL 未接库，用内存替身；"异常被包装层吞掉⇒返回 False"已用替身锁死 |
| UI 渲染 | `ListView`/`DataGridView` 实际绘制、`TabVisible` 的视觉隐藏未在真实消息循环下验证（`TabVisible` 用"摘除/插回 TabControl + 独立属性值"模拟） |
| `GetMagicName`/`GetStdItemName` | 未移植 ⇒ 技能名/物品名列在宿主接线前**抛异常**（按 §25.2，不静默） |
| 未初始化内存 | 短读场景的 `GetMem` 未初始化内存无法复刻（零填充，D-P10-34） |
| DFM 对话框 `Options` | 三项只映射了 `ofOverwritePrompt`（D-P10-35） |
| ★ R4 的语言规范结论 | `not … = …` 的优先级判定**未经 Delphi 编译器实测**（无工具链）；若被推翻，只需改 2 处表达式 + 2 条用例 |

**能力面（其余 6 单元）**：`MasSock` 的真实监听不可用（JSocket 无托管等价物，`Active := True` 显式抛，B-P10-19）；
`GrobalSession` 的会话列表来源为接缝（未接线即抛，B-P10-04）；`GHeroDBConfig` 的 `THeroDB` 为接缝（默认抛，故真实 HeroDB 读写不可用）；
`Pool/*` 的两个单元**无未覆盖面**（含端到端搜索用例，跑临时目录）。

---

## 5. 门禁记录（滚动；★ 判据含 `$LASTEXITCODE`）

| 切片 | 命令 | 结果 |
|---|---|---|
| 1（Pool 2 单元） | `dotnet test tests/GXX.LogDataServer.Tests/…csproj -c Debug --nologo -m:1 -p:BuildInParallel=false` | `失败: 0，通过: 222，总计: 222`（其中本车道新增 **80** 例） |
| 2（GrobalSession） | `dotnet test tests/GXX.LoginSrv.Tests/…csproj …` | `失败: 0，通过: 250，总计: 250`（其中本车道新增 **19** 例） |
| 3（MasSock） | 同上（LoginSrv.Tests） | `失败: 0，通过: 404，总计: 404`（本单元新增 **154** 例；250+154=404 ✓ 与子车道自报逐例相符） |
| 4（吸收 main + 退役复刻） | `main` 由**集成方**并入本车道（`b671a662`，无冲突；车道被硬禁 merge/rebase）→ 删 D-P10-17 复刻、改转调 Core | 同上（LoginSrv.Tests）仍 `失败: 0，通过: 404，总计: 404` |
| 5（GHeroDBConfig） | `dotnet test tests/GXX.GameCenter.Tests/…csproj …`（**判据含 `$LASTEXITCODE==0`**） | `失败: 0，通过: 278，总计: 278`（本单元新增 **59** 例；基线 219 ⇒ 219+59=278 ✓ 由 `--filter FullyQualifiedName~GXX.GameCenter.Forms.Tests` 实测 59 例） |
| 6（uFrmRoleDataEdit） | `dotnet test tests/GXX.DBServer.Tests/…csproj …` | 曾出现 `已通过! 失败: 0，通过: 675/693` **但 `$LASTEXITCODE=1`** —— 真因：`P10RoleDataEditTests.ButtonSaveDataClick_…_HumanDB_Save` 触发 `Stack overflow` ⇒ testhost 崩溃 ⇒ run 中止。★ **登记为方法论警示 W1**（见下） |
| 7（uFrmRoleDataEdit 修好后） | 同上 | **`失败: 0，通过: 871，总计: 871`；`$LASTEXITCODE=0`**；无 `Stack overflow`/`测试主机进程崩溃`/`测试运行已中止`/`error CS` |
| 8（修 B-P10-21 工具盲区后） | 同上 | 仍 **871/871，`$LASTEXITCODE=0`**（工具口径 31→35 后测试断言同步更新） |

### 5.1 ★★ 最终四工程门禁（全绿，判据含 `$LASTEXITCODE`）

```
dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false
  → 已成功生成。0 个错误                                    EXIT=0

dotnet test GXX.CSharp/tests/GXX.DBServer.Tests/GXX.DBServer.Tests.csproj       -c Debug --nologo -m:1 -p:BuildInParallel=false
  → 已通过! - 失败: 0，通过: 871，总计: 871                  EXIT=0  无崩溃/错误字样

dotnet test GXX.CSharp/tests/GXX.LoginSrv.Tests/GXX.LoginSrv.Tests.csproj       -c Debug --nologo -m:1 -p:BuildInParallel=false
  → 已通过! - 失败: 0，通过: 404，总计: 404                  EXIT=0  无崩溃/错误字样

dotnet test GXX.CSharp/tests/GXX.GameCenter.Tests/GXX.GameCenter.Tests.csproj   -c Debug --nologo -m:1 -p:BuildInParallel=false
  → 已通过! - 失败: 0，通过: 278，总计: 278                  EXIT=0  无崩溃/错误字样

dotnet test GXX.CSharp/tests/GXX.LogDataServer.Tests/GXX.LogDataServer.Tests.csproj -c Debug --nologo -m:1 -p:BuildInParallel=false
  → 已通过! - 失败: 0，通过: 222，总计: 222                  EXIT=0  无崩溃/错误字样
```

**本车道新增用例合计 = 80(Pool) + 19(GrobalSession) + 154(MasSock) + 59(GHeroDBConfig) + 62(uFrmRoleDataEdit) = 374**
（四工程合计 1,775 例全通过；逐项增量与各子车道自报逐例相符：250→404=+154、219→278=+59、809→871=+62。）

### 5.2 ★★ 两条方法论警示（建议入台账）

**W1 — `dotnet test` 的"假绿"：宿主崩溃前会把部分结果打印成 `已通过!`。**
同一命令连跑三次得到 `已通过! - 失败: 0，通过: 675`、`通过: 693`（**数字每次不同**），而 `$LASTEXITCODE` **全是 1**，
stderr 为 `Stack overflow` + `测试主机进程中止`。⇒ **只看摘要行会得出"全绿"的错误结论，且排在崩溃点之后的用例根本没跑**。
> **规程**：门禁判据必须是 **`$LASTEXITCODE == 0`**，且输出中**不得出现** `Stack overflow` / `测试主机进程崩溃` / `测试运行已中止`；
> 与 §37.3"否定性断言必须计数取证"同源 —— **"跑过了"也要取证**，不能只看"通过了"。

**W2 — `THumData`/`THeroData` 是巨型值类型，按值传递即打穿 1MB 线程栈。**
实测 `SizeOf<THumData>() = 660,377`（645KB）、`SizeOf<THeroData>() = 323,962`（316KB）。
夹逼证据：`new THumData()` + **内联**字段赋值（1/2/3/4 条）**全绿**；同样赋值放进**"返回 `THumData` 的辅助方法"**即 **`Stack overflow`**；
只构造窗体、对象初始化器赋 `FHumData`、网格读写、`ButtonSaveDataClick` 均正常（窗体把 `THumData` 存为**字段**，在堆上）。
⇒ **风险面**：任何车道的测试只要写 `static THumData Human() => …` 这类"按值返回/传参"的辅助方法（或同时存活 2~3 个副本）就会崩 testhost；
生产侧同理（`ProcessLoadDataformFile` 的 `THumData` 局部量占 ~645KB 栈，原文 `GetMem` 在堆上）——见 B-P10-23。
> **规程建议**：禁止按值返回/传参 `THumData`/`THeroData`（改 `out`/`ref`，或跑 `new Thread(action, maxStackSize: 64 << 20)`）；
> 或由集成方统一给测试宿主加大栈。

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

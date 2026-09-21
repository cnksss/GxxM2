# 并行报告 — 车道 `p9-m2-pathfind`

> 分支：`par/p9-m2-pathfind` ｜ 基线：`main @ b48e4c43` ｜ 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p9-m2-pathfind`
> 分区：`GXX.CSharp/src/GXX.M2Server/Sweep9/PathFind/**`、`GXX.CSharp/tests/GXX.M2Server.Tests/Sweep9PathFind*.cs`、本文件
> 源：`_analysis/utf8_mirror/M2Engine/{PathFindClient,PathFind_Hero,ClientPickItemsCfg,StruckDamageAbsorbUtils}.pas`（UTF-8 镜像；`Source/**` 是 GBK 原文，未直接读）

---

## 0. 门禁（两条命令的实际输出摘要）

```
$ dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false
    165 个警告
    0 个错误
    === BUILD EXIT 0 ===

$ dotnet test GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj -c Debug --nologo -m:1 -p:BuildInParallel=false
    已通过! - 失败:     0，通过:  9641，已跳过:     0，总计:  9641，持续时间: 38 s
    === TEST EXIT 0 ===
```

* **0 error / 0 failed**。
* 全量警告数在 `157/163` 区间已知会波动（台账 §34.5：同一条警告被多项目重复计入），故按 §34.5 规程改用**更强的口径**：
  `dotnet build …M2Server.Tests.csproj -t:Rebuild` 的完整输出中，匹配 `Sweep9\PathFind|Sweep9PathFind` 的警告行数 = **0** ⇒ **本车道改动的 8 个文件贡献 0 条警告**。
* 未跑整解决方案测试（仅本车道测试工程）。为压低峰值内存，所有 build/test 均带 `-m:1 -p:BuildInParallel=false`（调度方提醒）。

---

## 1. 交付物

| 文件 | 行数 | 内容 |
|---|---|---|
| `src/GXX.M2Server/Sweep9/PathFind/PathFindClient.cs` | 1109 | `PathFindClient.pas` 1:1（`TPathMap`/`TLegendMap`/`TFindPathThread`/`TTerrainTypes`/`TTerrainParam`/`TMapHeader`/`TMapInfo`/`TCellParams`/`TGetCostFunc`/`TerrainParams`/接缝） |
| `src/GXX.M2Server/Sweep9/PathFind/PathFind_Hero.cs` | 849 | `PathFind_Hero.pas` 1:1（`TPathMap`/`TFindPath`/`TWave`/`TGetCostFunc`(UnitPath 版)/接缝） |
| `src/GXX.M2Server/Sweep9/PathFind/ClientPickItemsCfg.cs` | 265 | `ClientPickItemsCfg.pas` 1:1（`TClientPickItems`） |
| `src/GXX.M2Server/Sweep9/PathFind/StruckDamageAbsorbUtils.cs` | 311 | `StruckDamageAbsorbUtils.pas` 1:1（`TMonterStruckDamageAbsorb`/`TStruckDamageAbsorbMgr`/接缝） |
| `tests/GXX.M2Server.Tests/Sweep9PathFindClientTests.cs` | 1081 | 47 个测试方法 / 51 个用例 |
| `tests/GXX.M2Server.Tests/Sweep9PathFindHeroTests.cs` | 1025 | 46 个测试方法 / 53 个用例 |
| `tests/GXX.M2Server.Tests/Sweep9PathFindAbsorbTests.cs` | 480 | 21 个测试方法 / 34 个用例 |
| `tests/GXX.M2Server.Tests/Sweep9PathFindPickItemsTests.cs` | 381 | 21 个测试方法 / 21 个用例 |
| `docs/并行报告-p9-m2-pathfind.md` | — | 本文件 |

命名空间（因 `PathFindClient.pas` 与 `PathFind_Hero.pas` **声明了同名不同形的 `TPathMap`/`TWave`**，必须分开，否则 CS0101）：

* `GXX.M2Server.Sweep9.PathFind.Client`（PathFindClient.pas）
* `GXX.M2Server.Sweep9.PathFind.Hero`（PathFind_Hero.pas）
* `GXX.M2Server.Sweep9.PathFind.Items`（ClientPickItemsCfg.pas）
* `GXX.M2Server.Sweep9.PathFind.Damage`（StruckDamageAbsorbUtils.pas）

**未改任何分区外文件**（`GXX.slnx` / `*.csproj` / `Directory.Build.props` / `docs/Checklist.md` / `docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**` 一字未动；SDK 风格 csproj 自动 glob，新文件无需登记）。

---

## 2. 逐单元方法对账（已移植 / 总数）

计数口径（脚本 `Select-String` 扫 `^\s*(procedure|function|constructor|destructor)\s+Ident`，分别统计 interface 段与 implementation 段）——**不是肉眼扫**：

| 单元 | interface 段声明 | implementation 段实现 | 其中嵌套过程/函数 | 托管侧已移植 | 结论 |
|---|---|---|---|---|---|
| `PathFindClient.pas` | **31** | 35 | 4（`GetNextDirection`/`PreparePathMap`/`TestNeighbours`/`ExchangeWaves`） | **31/31 + 4/4** | 100% |
| `PathFind_Hero.pas` | **27** | 31 | 4（同名四个） | **27/27 + 4/4** | 100% |
| `ClientPickItemsCfg.pas` | **11** | 11 | 0 | **11/11** | 100% |
| `StruckDamageAbsorbUtils.pas` | **9** | 9 | 0 | **9/9** | 100% |
| **合计** | **78** | 86 | **8** | **78/78 + 8/8 = 86/86** | 100% |

* 嵌套过程在 C# 里落为**私有方法**（`ExchangeWaves` 用 `ref` 参数保留"交换两个波对象"的语义）或**局部函数**（`GetNextDirection`，与原文逐字对应），并逐一在注释里标出原文行号。
* 新增用例 **159 个**（21 + 34 + 51 + 53）。每个公开成员都有用例；每个原文缺陷都有差异断言（见 §4）。
* 逐条清点取证（否定性断言，台账 §37.3）：`PublicSurfaceMatchesOriginalInterfaceSection` 把原文 interface 段的成员名**逐条枚举后计数**（`TPathMap` 17 项、`TLegendMap` 10 字段 + `FindPath` 5 重载 + `Find` 2 重载、`TFindPath` 3 重载、`TWave` 7 成员），断言"枚举项数"与"反射实际存在"两两相符。

---

## 3. 与既有 `src/GXX.Core/Util/PathFind*.cs` 的对照结论

### 3.1 先确认"谁是谁"（用 sha256 + 行数取证，不靠肉眼）

```
Source/Common/PathFind.pas          509 行  3690E569CF132CF5   ← 既有 GXX.Core/Util/PathFind.cs 的来源
Source/Client-HGE/PathFind.pas      817 行  981A0BADF8EC41FD
Source/M2Engine/PathFind.pas        732 行  53127E8E9F5A088F
Source/M2Engine/PathFindClient.pas  811 行  4C5E3F24167B2771   ← 本车道切片 3
Source/M2Engine/PathFind_Hero.pas   693 行  31212A4294A7BC1D   ← 本车道切片 4
```

结论：**三个同名单元互不相同**，既有 Core 产物只对应 `Common/PathFind.pas`。本车道的两个单元与它**同源同算法、但不同单元、不同容器类型**。
另有 `_analysis/utf8_mirror/M2Engine/PathFind.pas`（732 行）**不在本车道任务书内**，未动。

### 3.2 「只移植差异部分」的落地方式

按任务书"不要重复实现同一算法"，本车道**不重写波扩散算法**，而是**复用**既有 `GXX.Core.Util` 的四个数据容器：

| 复用的既有类型 | 依据 |
|---|---|
| `TWave`（`PathFind.cs:162`） | 其 `Clear` 与 `PathFindClient.pas` 的 **逐字相同**（`FPos/FCount/FMinCost`，无 `FData := nil`）⇒ 直接复用 |
| `TWaveCell`（`PathFind.cs:153`） | 两单元的记录声明逐字相同、无差异 |
| `TPathMapCell`（`UnitPath.cs:36`） | 同上 |
| `PathPoint`（`UnitPath.cs:26`，= `TPath` 的元素） | 与 `UnitPath.pas` 的 `array of TPoint` 对应 |
| `PathFindConst.TerrainMoveCost` | 用作 `TerrainParams` 的 **交叉验证**（见下） |

`PathFind_Hero.pas` 的 `TWave` **不能**复用：它的 `Clear` 多一句 `FData := nil`（三份 `Clear` 各不相同），故按原文单独声明（登记 D-P9-12）。

**交叉验证用例** `TerrainParams_MatchesOriginalTableAndExistingCore`：把 `PathFindTerrainParams` 的 6 个 `MoveCost` 与既有 `PathFindConst.TerrainMoveCost` **逐项比对**，锁死"两份产物不分叉"。

### 3.3 逐函数对照（PathFindClient.pas ↔ Common/PathFind.pas ↔ 既有 Core 产物）

| PathFindClient.pas | Common/PathFind.pas（既有 Core 产物） | 本车道处置 |
|---|---|---|
| `TPathMap.DirToDX/DirToDY`（308/318） | 同名同体（278/288） | **照抄**（private，用"记录回调"用例反推 8 方向表） |
| `TWave.Create/Destroy/GetItem/Add/Clear/Start/Next` | 同名同体（174-221） | **复用既有 Core 的 `TWave`**（Clear 逐字相同） |
| `TPathMap.FillPathMap`（566-679） | 同名但**边界由 `MapWidth/MapHeight` 决定、无 `StartFind`**（302-382） | **移植差异版**：边界改由 `ClientRect` 决定 + 全程查 `StartFind` |
| `TPathMap.GetCost`（681-690） | 同名同体（384-392） | **照抄**（`virtual` 保留） |
| `TPathMap.FindPathOnMap`（332-373） | 是 **function**、返回 `TPath`（243-261） | **移植差异版**：本版是 **procedure**、写 `FPath` 后再 `WalkToRun()` |
| `TPathMap.WalkToRun`（375-522） | **无** | 新增移植（含骑马一步三格支） |
| `TPathMap.MapX/MapY/LoaclX/LoaclY`（530-548） | **无** | 新增移植 |
| `TPathMap.GetClientRect`（551-564） | **无**（尺寸由 `FindPath` 入参吸收） | 新增移植（**忽略 4 个入参**，那段按 `ScopeValue` 收窄的算法原文整段被 `{ }` 注释掉 ⇒ 逐字保留为注释） |
| `TPathMap.FWidth/FHeight/FRealSize/SetWidth/SetHeight/Path/RunPath/Width/Height`（97-99/283-299/124-127） | **无** | 新增移植（`FRealSize` 是**死字段**：只写不读，照抄） |
| `TPathMap.FFillPathMap/FFindPathOnMap/StartFind`（103-104/115） | **无** | 新增移植（两个标志位是 `TLegendMap.FindPath` 自旋等待的依据） |
| `TLegendMap.Create/Stop/Find/FindPath×2/SetStartPos`（692-777） | 只有 `Create/LoadMap/FindPath×2/SetStartPos/XYToCurrXY`（394-468） | **移植差异版**（本版无 `LoadMap`，多 `Stop`/`Find` 与 7 个公开计数/坐标字段） |
| `TLegendMap.GetCost`（779-809） | 用 `TCellParams` 地形表 + `CanWalkEx`（470-…） | **移植差异版**：用 `TMapInfo` 表 + `PlayScene.NewCanWalkEx/_2`（接缝） |
| `TFindPathThread`（176-220） | **无** | 新增移植 |
| `TerrainParams` 常量表（187-194） | 只有 `PathFindConst.TerrainMoveCost` | 新增移植（含 `TColor`/`CellLabel`）+ 与既有 MoveCost 交叉验证 |
| `TMapHeader`/`TMapInfo`/`TMapInfoArray`/`TCellParams`/`TTerrainTypes`/`TTerrainParam`/`TGetCostFunc`（45-93） | 部分同名（`TTerrainTypes`/`TCellParams`/`TGetCostFunc`） | 按本单元原文**独立声明**；`TGetCostFunc` 与既有的（3 参）**签名不同**（本版 4 参带默认 `PathWidth`） |

### 3.4 逐函数对照（PathFind_Hero.pas ↔ Common/PathFind.pas ↔ 既有 Core 产物）

| PathFind_Hero.pas | Common（既有 Core 产物） | 本车道处置 |
|---|---|---|
| `DirToDX/DirToDY`（194/204） | 同名同体 | 照抄 |
| `TWave`（101-175） | 同名，`Clear` **少一句 `FData := nil`** | **单独声明**（D-P9-12） |
| `TPathMap.GetClientRect`（408-411，**无参**、读公开字段 `Width/Height`） | 无 | 照抄 |
| `TPathMap.FindPathOnMap(X,Y,Run): TPath`（218-267） | `FindPathOnMap(X,Y): TPath` | 移植差异版（多 `Run`、用完释放 `PathMapArray`、多 `nCount >= Length(Result)*2` 上限） |
| `TPathMap.WalkToRun(Path): TPath`（269-380） | **无** | 新增移植（**纯函数**，只有 2 格合并支） |
| `TPathMap.FillPathMap(...,boFlag)`（413-544） | `FillPathMap(X1,Y1,X2,Y2)` | 移植差异版（多 `boFlag`、多两道 2000 次上限、多两道**维数错位**的守卫、无 `StartFind` 初始化检查） |
| `TPathMap.GetCost(...,boFlag)`（546-549） | `GetCost(X,Y,Dir)` | 差异版：**基类返回 0**（不是 -1） |
| `TFindPath`（73-93/551-691） | `TLegendMap` | 全新移植（`FEnvir`/`FBaseObject`/`TM2CriticalSection` + 三重重载 + 锁编号 1/2/3/5） |
| `UnitPath.TGetCostFunc`（4 参 `procedure of object` + `var Result`） | 既有 `GXX.Core.Util.TGetCostFunc` 是 3 参 function | **独立声明**（D-P9-10）——工程内**共有三个**不同形状的 `TGetCostFunc`，本单元用的是 UnitPath 那个 |

---

## 4. 原文缺陷清单（全部照抄 + 差异断言锁定）

> 处置原则：原文的写法就是标准；可疑处不顺手修，只加 `// 原文如此` 注释 + 一条差异断言。

### 4.1 `ClientPickItemsCfg.pas`

| # | 缺陷 | 锁定用例 |
|---|---|---|
| ① | `Sort` 是**不稳定**快排：`CompareItem` 只看低 14 位，而分区里 `if I <> J then` **无条件互换** ⇒ 同键两条的相对顺序由分区轨迹决定（实测：2 元素输入时**必然对调**，故 `SearchItem` 命中的是"后输入的那条"） | `Sort_SwapsEqualKeyEntries_SoDuplicateLookupHitsReversedInput`、`Sort_CanBeCalledAgainOnLoadedData` |
| ② | `SearchItem` 命中后先 `H := I - 1` 再 `L := I` ⇒ 循环立即结束，**不会继续向左找重复键**；返回的是对半收敛落到的那个下标 | `SearchItem_…`（见 ①，同一组） |
| ③ | `I := L + (H - L) shr 1` 依赖 Delphi 优先级（`shr` 高于 `+`）；C# 的 `>>` **低于** `+`，照抄会变成 `(L+H-L) >> 1` —— 台账 §35.3 同坑 | 源码显式加括号 + 覆盖 1..40 规模的 `SortAndSearch_AllKeysResolvable_ForSizesOneToForty` |
| ④ | `SetData` 只判 `BufLen mod 2`，**不校验** `BufLen` 是否超出缓冲区 ⇒ 越界读（原文 AV） | `SetData_BufLenBeyondBuffer_Throws` |
| ⑤ | `destructor Destroy` 的 `FItems := nil` 与 `Clear` 同效 | `ClearAndDispose_EmptyTheStore` |
| ⑥ | 三个 `Check*` 的位掩码是 `$8000`/`$4000`/`$C000` ⇒ `CheckCanPickItem` 是"**或**"而不是"与" | `CheckCanPickItem_IsOrOfBothFlags_NotAnd` |
| ⑦ | 金币"Idx = 0 特殊处理"意味着存储值的低 14 位就是 0（而非 `0+1`） | `ItemIdxZero_MatchesOnlyKeyZero` |

### 4.2 `StruckDamageAbsorbUtils.pas`

| # | 缺陷 | 锁定用例 |
|---|---|---|
| ① | `Search` 命中即终止，不向左找同名项 | `AddAndSearch_SortedAndFindable_ForFortyNames`（含"重复名不产生重复项"的计数取证） |
| ② | `Add` 把"率与值都 > 0"当唯一有效条件、**不裁下界** ⇒ `Rate=0` 或 `Value=0` 时**删除已有项 / 不新增** ⇒ **无法表示"吸收 0%"**，且"率 0 ⇒ 永不触发"这条分支在公开 API 下**不可达** | `Add_ZeroRateOrValue_OnNewName_AddsNothing`、`Add_ZeroOnExistingName_RemovesEntry`、`ZeroRateEntryIsUnreachableThroughPublicApi` |
| ③ | 已存在项不删除时**原地改写**同一对象（引用恒等 + `StartTime` 被刷新） | `Add_OnExistingName_UpdatesInPlaceAndRefreshesStartTime` |
| ④ | `EffectiveTime * 1000` 在 Delphi 是 `Cardinal * Integer → Int64`（不溢出） | `Run_ExpiryGateIsGreaterOrEqual` + `Run_WrapAround_InheritsTickDiffOffByOne`（同时锁住既有 `tick_diff` 的 `High(Cardinal)` 少 1 偏差） |
| ⑤ | `Run` 在循环体内**逐条**调 `MyGetTickCount` | `Run_RemovesMultipleExpiredEntries` |
| ⑥ | 通配名 `'*'` 只是普通字符串，必须真的在表里 | `GetStruckDamage_FallsBackToWildcardNameOnlyIfPresent`、`GetStruckDamage_ExactNameBeatsWildcard` |
| ⑦ | `AbsorbDamageValue` 是"吸收百分比"而非固定点数；`Round` 是**银行家舍入** | `GetStruckDamage_FormulaUsesFloatingDivideAndBankersRounding`（三个 .5 算例：0.5→0、1.5→2、2.5→2） |
| ⑧ | 触发门是 `Random(100) < Rate`（**严格小于**） | `GetStruckDamage_TriggerGateIsStrictlyLess` |
| ⑨ | 结果下限钳 0 只在触发分支内 | `GetStruckDamage_ClampsNegativeResultToZero` |

### 4.3 `PathFindClient.pas`

| # | 缺陷 | 锁定用例 |
|---|---|---|
| ① | `TLegendMap.Find` **先起线程、后**写 `FExcludeMonster`（`with` 里的名字落到外层 `Self`）⇒ 与线程体存在竞态 | `LegendMap_FindSetsExcludeMonster`（顺序逐字保留；用 `GetCost` 支路取证字段被写入） |
| ② | `FindPathOnMap` 中途 `Break` 后**仍然**执行 `FPath[0] := …` 与整表回填 ⇒ `FPath[1..]` 是 `null` ⇒ 崩溃（原文 AV） | `FindPathOnMap_BreakMidwayLeavesNullsAndCrashes` |
| ③ | 收集条件 `(x <> -1) and (y <> -1)`：任一坐标为 -1 即丢弃（当 `ClientRect.Left/Top` 为负时，合法坐标也可能被静默丢） | `WalkToRun_CollectionConditionIsAndOnBothCoordinates` |
| ④ | 收集**从下标 1 开始** ⇒ 起点永不进入 `FRunPath` | `WalkToRun_TwoPointPathDropsStartPoint`、`FindPathOnMap_BacktracksAndMergesStraightLine` |
| ⑤ | `FRealSize` **只写不读**（死字段） | 见 `Create_…`/`WidthHeight_PropertiesRoundTrip`（字段存在但无可观察效果，已在报告登记） |
| ⑥ | 两个 `FindPath` 重载的**前几步顺序不同**（`Inc(FindCount)` 与 `FExcludeMonster := …` 互换） | `LegendMap_BothFindPathVariantsIncrementFindCountOnce` |
| ⑦ | `GetClientRect(X1,Y1,X2,Y2)` **四个入参全部不使用**；按 `ScopeValue` 收窄的算法整段被注释 | `GetClientRect_IgnoresArguments` |
| ⑧ | **`SetStartPos` 不设 `StartFind`**，而 `FillPathMap` 全程查它 ⇒ 调用前 `StartFind = False` 时**必崩**（原文 AV） | `LegendMap_SetStartPosRequiresStartFindAlreadyTrue` |
| ⑨ | **值语义陷阱**：原文 `WalkPath[I] := Path[I]` 是 TPoint **记录拷贝**；托管侧 `PathPoint` 是 class，直接赋值会连带改写 `FPath` | `WalkToRun_DoesNotAliasInputPath`（含 `NotSame` 断言） |
| ⑩ | `GetNextDirection` 的两条**改写规则**（`abs(sy-dy)>2 ⇒ flagx:=0`、`abs(sx-dx)>2 ⇒ flagy:=0`，且后者低端用 `>`、高端用 `<=`） | `GetNextDirection_FlagxOverrideAllowsMerge`、`GetNextDirection_FlagyOverrideAllowsMerge`（各给"若无此改写就不合并"的对照推理） |
| ⑪ | 骑马支一次合并 3 格，普通支 2 格；需 `m_btHorse <> 0` **且** `boHorseRun3Grid`（两个条件缺一不可） | `WalkToRun_HorseThreeGridBranchMergesThreeSteps`、`WalkToRun_HorseBranchNeedsBothConditions` |

### 4.4 `PathFind_Hero.pas`

| # | 缺陷 | 锁定用例 |
|---|---|---|
| ① | `FillPathMap` 的两道越界守卫**维数互相错位**（`nX1 >= Length(Result)` 拿列号比行数、`nY1 >= Length(Result[0])` 拿行号比列数） | `FillPathMap_TransposedGuardsRejectLegalStartAndAcceptIllegalOne`（实测：`2×10` 图里**合法**起点 `(0,2)` 被判非法 → 空图；**非法**起点 `(5,0)` 被放行 → 越界） |
| ② | 那两条 `Exit` 在 `TWave.Create` **之后** ⇒ 原文漏 `Free`（泄漏） | 顺序逐字保留；托管侧由 GC 承担（无可观察后果，已在报告登记） |
| ③ | `FindPathOnMap` 中途 `Break` 后 `Result[1..]` 为 `null` ⇒ 崩溃（原文 AV） | `FindPathOnMap_BreakMidwayCrashes` |
| ④ | 临界区使用**完全由 `g_MultiThreadRun` 门控**（假则根本不进锁） | `FindPath_StopClearsState`（真/假两条路径各一例） |
| ⑤ | `FindPath(…, boFlag): Boolean` 这一重载**自己不拿锁**，转调 6 参版（那一层拿锁编号 2） | `FindPath_BooleanOverloadReturnsWhetherPathExists` |
| ⑥ | 收集条件同样是 `(x<>-1) and (y<>-1)`；起点永不进入结果 | `WalkToRun_CollectionConditionIsAnd`、`WalkToRun_IsPureAndDropsStartPoint` |
| ⑦ | 基类 `TPathMap.GetCost` 返回 **0**（不是 -1）⇒ 直接用基类则 `C >= 0` 恒真 | `Map_BaseGetCostReturnsZeroNotMinusOne`（含 `(99,99)` 都不判界） |
| ⑧ | `SetStartPos` 不设 `StartFind` ⇒ 调用前为假时主循环第一轮就 `Break`、图里只剩起点格 | `FindPath_SetStartPosWithoutStartFindLeavesOnlyStartCell` |
| ⑨ | `TFindPath.GetCost` 在 `FEnvir = nil` 时**无条件 -1**，而 `SetStartPos` **不设 `FEnvir`** ⇒ 此时接缝一次都不会被问到、图里除起点外全 -1 | `FindPath_SetStartPosWithoutEnvirConsultsNothing`（计数取证：`calls == 0`） |
| ⑩ | 值语义陷阱（同 4.3 ⑨） | `WalkToRun_IsPureAndDropsStartPoint` |
| ⑪ | `TWave.Destroy` 只清 `FData`、**不清 `FCount`** ⇒ `Start()` 仍为真而 `Item` 越界 | `Wave_DestroyEmptiesDataButLeavesCount` |
| ⑫ | 两道 **2000 次循环上限**（外层 `nCount`、内层 `nLoopCount`），且检查位置在内层 `StartFind` 之前 | `FillPathMap_*` 系列（正常规模不触发；上限值已逐字保留） |

---

## 5. 偏离登记（D-P9-xx）

| 编号 | 偏离 | 理由 / 范围 |
|---|---|---|
| **D-P9-01** | `SetData(Buf: PByte; BufLen: Integer)` → `SetData(byte[] Buf, int BufLen)`；`Move` → 逐字节小端还原 | 工程统一做法（`PByte` → `byte[]`）；语义等价（原文是整块内存拷贝） |
| **D-P9-02** | `destructor Destroy`（`FItems := nil`）→ `Dispose()` | 托管侧无析构时机；与 `Clear` 同效（原文缺陷 ⑤） |
| **D-P9-03** | 越界 `Move` 的 AV → `IndexOutOfRangeException` | 行为等价（都是"越界即失败"），异常类型是托管的 |
| **D-P9-04** | `TMonterStruckDamageAbsorb`（record + `^` 指针 + `New/Dispose`）→ **class**（`new` / GC） | 列表里存的就是指针、`Add` 返回的指针会被长期持有 ⇒ 必须保留引用语义；Delphi 记录的值语义在此毫无用处 |
| **D-P9-05** | `TLegendMap.Find`/`FindPath` 的**默认参数** → **显式重载**（`Find`×2、`FindPath`×5） | C# 里 `(int,int,bool=false)` 与 `(int,int,int=0,bool=false)` 对 `FindPath(1,2)` 会 **CS0121 歧义**。显式重载后的**合法调用面与原文完全一致**（原文也不允许 3 个 int 实参），并有 `LegendMap_FindPathOverloadsMatchDefaultArguments` 锁定等价性 |
| **D-P9-06** | `string[16]`（`CellLabel`）→ `string` | 不强制 16 字节截断；6 个标签都远短于 16 字节 |
| **D-P9-07** | `TMapInfoArr = array[0..MaxListSize] of TMapInfo`、`pTMapInfoArr`、`pTPathMapArray` **不移植** | 巨型定长数组 + 指针类型，本单元**从不使用**，且 `MaxListSize` 是 Delphi 私有常量 |
| **D-P9-08** | `TPathMap` 的 `private FPath/FRunPath/FFillPathMap/FFindPathOnMap` → `protected` | Delphi 的 `private` 在**同一单元内**对 `TLegendMap` 可见（它确实读写这四个成员）；`protected` 保留该可见性 |
| **D-P9-09** | `TFindPathThread` 不继承 `System.Threading.Thread`，改**组合** + `protected virtual Execute()`，并补回原文从 `TThread` 继承的 `WaitFor` | `Thread.Run` 非虚，继承就无法保留 `Execute` 的 `override` 语义（台账 §18.8 禁止把虚分派降级）；`FreeOnTerminate := True` 由 GC 承担 |
| **D-P9-10** | `UnitPath.pas` 的 `TGetCostFunc`（`procedure(Sender; X,Y,Direction; var Result) of object`）在本命名空间**独立声明** | 工程内**共有三个形状不同**的 `TGetCostFunc`（UnitPath 版 / Common\PathFind 三参版 / PathFindClient 四参版），不能互相顶替；本单元用的是 UnitPath 版 |
| **D-P9-11** | `FEnvir.m_nWidth/m_nHeight` → 既有 `TEnvirnoment.nWidth/nHeight` | 既有 `Engine/Envir.cs` 把字段命名为 `nWidth/nHeight`（偏离原文 `m_n*`）。本车道**不越区**改 `Envir.cs`，故按既有名读取并登记 |
| **D-P9-12** | `TWave.Clear` 的 `FData := nil` → `FData = Array.Empty<TWaveCell>()` | 语义等价（都是清空），差异仅在容量复用；**不可行为观察**，故只能以源码对照取证（三份 `Clear` 已在 §3.2 引用） |
| **D-P9-13** | `TBaseObject` → **`TCreature`**；`TBaseObject(FBaseObject).m_btPermission` → 接缝 `GetPermission`（默认"是 `TPlayObject` 就取 `m_btPermission`、否则 0"） | 托管侧把原 `TGameObject→TBaseObject→TAnimalObject→TSmartObject` 几层折叠成 `TCreature`；`m_btPermission` 在原文属 `TBaseObject`（ObjBase.pas:117，`Byte`），而托管侧该字段落在 `TPlayObject` 上。**不在 `TCreature` 上新增同名字段**是为了避免台账 §34.2 的"同名字段隐藏 ⇒ 同一状态两份存储" |
| **D-P9-14** | `FCriticalSection.Free` → `Dispose()`；`TFindPath` 实现 `IDisposable` | 托管侧释放约定 |
| **D-P9-15** | `TRect` → `System.Drawing.Rectangle`；`Bounds(0,0,W,H)` → `new Rectangle(0,0,W,H)`；`TColor` → `int` | Windows/Types/Graphics 单元的外部类型，取 BCL 对应物（`Rectangle` 是值类型，与 Delphi 记录语义一致） |
| **D-P9-16** | `PathFindClientSeam.LegendMap` 默认给**非 null 的 0×0 空实例**，且接缝层各默认值取"空状态" | 原文的全局 `LegendMap` 由客户端在载图时配置；M2Server 侧无该对象。**风险见 B-P9-05** |

---

## 6. 未完成 / 阻塞项 / 风险（如实登记）

### 6.1 阻塞项（依赖缺失，已按最小接缝处置，**未臆造替身**）

| 编号 | 内容 | 影响 |
|---|---|---|
| **B-P9-01** | `TEnvirnoment.CanWalkEx(nX,nY,boFlag)` 与 `CanWalkEx(BaseObject,nX,nY,boFlag)`（`Envir.pas:2296-2735`，440 行）**本体未移植**（既有 `CanWalkCore.cs` 是"证据型核心"，只有元数据断言，没有可调用实现） | `PathFind_Hero.GetCost` 的两支可走判定默认恒假 ⇒ 未接线前**寻路必然失败**。接缝：待 Envir 本体落地后把 `PathFindHeroSeam.CanWalkEx/CanWalkExObject` 指过去 |
| **B-P9-02** | `TBaseObject.InSafeZone()`（`ObjBase.pas:633`，两个重载）**完全未移植** | `boSafeAreaLimited = True` 时会误判"不在安全区" ⇒ `GetCost` 少一条可走来源 |
| **B-P9-03** | `m_btPermission` 在托管侧落在 `TPlayObject`、`TCreature` 基类没有 | 非人物对象恒按 0 处理（原文读的是 `TBaseObject` 字段）。待分层归位后改默认转发 |
| **B-P9-04** | `PlayScene.NewCanWalkEx/NewCanWalkEx_2`、`MShare.g_MySelf.m_btHorse`、`g_ClientConfig.boHorseRun3Grid`、全局 `LegendMap` 全部是接缝（M2Server 无客户端对象） | 未接线时 `TLegendMap.GetCost` 恒 -1、`WalkToRun` 恒走 2 格支（骑马一步三格支不可达） |
| **B-P9-05** | ⚠ **接缝默认值不安全**：`PathFindClientSeam.LegendMap` 是 0×0 实例，而 `TFindPathThread` 在**后台线程**里对它调 `FindPath` ⇒ `FillPathMap` 越界（原文 AV），托管侧未捕获异常会**直接终止进程** | 宿主接线时**必须**先给 `LegendMap` 设 `Width/Height`。已在接缝 XML 注释里显著标注；本车道用例一律先换成带尺寸实例 |
| **B-P9-06** | `tools/unit-map.tsv` 第 59/60/74/76 行把本车道的 4 个单元都登记在 **`par/p3-m2-sweep`** 名下，与本车道 `par/p9-m2-pathfind` **重叠**；`docs/并行报告-p3-m2-sweep.md:194` 的"零散小单元族"一行也把 `StruckDamageAbsorbUtils`/`ClientPickItemsCfg` 记为**未开工** | 本车道**不越区**改 `tools/**`。请集成方裁定归属并把 `unit-map.tsv` 改到 `par/p9-m2-pathfind`，或明确 p3-m2-sweep 不得再动这 4 个文件 |
| **B-P9-07** | `TStruckDamageAbsorbMgr.GetStruckDamage` 的**唯一调用点**（`TBaseObject._Attack` 里按攻击者名查吸收表）在 `AttackCore.cs`/`ExplosionMonsterCore.cs` 里被登记为"尚未移植" | 本单元目前是"**已交付实现但未接线**"（与台账 §33.4/§35.4 同源）。接线只需在 `AttackCore` 的对应分支调用 `m_StruckDamageAbsorbMgr.GetStruckDamage(name, power)` |
| **B-P9-08** | `TClientPickItems` 在 M2Engine 侧的处理链（客户端内挂上传的捡取配置消息 → `SetData`）未接线 | 同上：实现已就绪，缺调用点。另：本单元的两个**调用方**（客户端 `PlayScn`/内挂）属客户端产物，不在本车道范围 |

### 6.2 风险（已保留最小改法，供将来裁定）

| 编号 | 内容 |
|---|---|
| **R-P9-01** | **B-P9-05** 的进程终止风险（0×0 接缝 LegendMap + 后台线程）。最小改法：把 `PathFindClientSeam.ResetDefaults()` 里的 `LegendMap` 改成 `new TLegendMap { Width = 1, Height = 1 }`（一处开关）。本车道**未擅自改**，因为那会给接缝默认值安一个凭空的尺寸 |
| **R-P9-02** | `AnsiCompareText` 用 **`OrdinalIgnoreCase`** 近似（`MonGenLoadCore.AnsiCompareText`，工程既有唯一处置）。对 GBK 中文怪物名，Delphi 的 `AnsiCompareText` 是**区域敏感**比较 ⇒ 排序可能与原文不同（会连带影响 `Search` 的命中）。这是**工程级既有权衡**，不是本单元引入 |
| **R-P9-03** | `StruckDamageAbsorbUtils` 的 `Random` 用自建接缝 `PathFindDamageSeam.Random`（M2Server 侧没有全局 Random 接缝；`Npc.ObjNpcSeams.Random` 属 Npc 命名空间，不跨用）。将来若出现全局 Random 接缝，应改指向它 |
| **R-P9-04** | `PathFindHeroSeam.MultiThreadRun` 默认**转发**既有 `Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun`（不另造第二份存储）。待 `M2Threads.pas` 正式移植后应改指向它 |

### 6.3 未做（明确不在本车道范围）

* `Source/M2Engine/PathFind.pas`（732 行）与 `Source/Client-HGE/PathFind.pas`（817 行）**不在任务书内**，未动。
* `PathFind_Hero.pas` 依赖的 `Envir.pas` / `ObjBase.pas` 本体、`M2Threads.pas`：只做接缝，未顺手移植（任务书第 4 条）。
* 未把新单元登记进 `docs/Checklist.md` / `docs/并行覆盖审计.md` / `tools/unit-map.tsv`（分区外）。

### 6.4 给集成方的动作项

1. **裁定 `unit-map.tsv` 归属**（B-P9-06）：4 个单元应记到 `par/p9-m2-pathfind`。
2. **接线 `TStruckDamageAbsorbMgr.GetStruckDamage`**（B-P9-07）到 `AttackCore.cs` 的人类分支之前那一步。
3. **接线 `TClientPickItems`**（B-P9-08）。
4. **接线三个接缝**（B-P9-01/02/04）到 Envir/PlayScene/MShare 落地后的真实实现；接线前注意 B-P9-05 的尺寸前置条件。
5. `TLegendMap`/`TFindPath` 现存**两套**（`GXX.Core.Util.TLegendMap` 与 `...PathFind.Client.TLegendMap`），集成时若要"只留一套"需先裁定 Common/M2Engine/Client 三份 `PathFind.pas` 的关系 —— 本车道按任务书"只移植差异部分"保留了两套，并已在 §3 给出逐函数对照。

---

## 7. 过程事故与可复用教训

**I-P9-01 ★ 用 PowerShell 做文本往返，把 UTF-8 中文的 `.cs` 按 GBK 读回再写，损坏文件。**
本车道在修 7 处 xUnit 分析器告警时用了
`Get-Content $p -Raw` → `Replace` → `Set-Content -Encoding utf8`；该 shell 的 `Get-Content` 默认按 **ANSI(GBK)** 解码，于是
① 全部中文注释变成乱码、② 部分多字节序列**吞掉了行尾 CRLF**（两行被并成一行）。
损害经 `read` 工具发现（源码结构被破坏），因该文件**尚未提交**（切片 3 未 commit）⇒ 按上下文**完整重写**并继续。
**规程（建议写进台账）**：本工程**任何含中文的 `.cs`/`.md` 一律只用 `read`/`edit`/`write` 工具**，
禁止用 `Get-Content`/`Set-Content`/`Out-File` 做"读—改—写"往返；如需脚本化改写，必须先 `git commit` 或复制备份。
这与台账 §8.1（PS 5.1 按 ANSI 解析 `.ps1`）与 §37.4（GBK 文本被误判为 UTF-8）**同族**，方向相反（本次是"UTF-8 被按 ANSI 读"）。

**I-P9-02 3 例测试失败全部是"我写错、源码没错"。**
`FindPath_TwoArgOverloadReusesExistingMap`（漏设 `FEnvir` ⇒ `GetCost` 恒 -1）、
`Wave_DestroyEmptiesData`（`Destroy` 只清 `FData`、不清 `FCount`）、
`FindPath_SetStartPosFillsMapWithBoFlagFalse`（同上漏设 `FEnv`）。
三次都按台账 §34.4 先回读原文判定哪侧错：**实现一行未改**，只改期望，并把新查清的三条机制
（`SetStartPos` 不设 `FEnvir`、`Destroy` 不清 `FCount`、`GetCost` 的 `FEnvir = nil` 早退）
各自补成正式的差异断言用例（`FindPath_SetStartPosWithoutEnvirConsultsNothing`、
`Wave_DestroyEmptiesDataButLeavesCount`）。

**I-P9-03 第一次 `dotnet test | Select-Object -First N` 报 exit 1 —— 是断管假失败。**
`Select-Object -First` 提前关闭管道会让 `dotnet test` 拿到 broken pipe 而返回 1。
改为 `Out-File` 落盘后再筛，得到真实的 `exit=0`。与台账 §12.5（门禁假绿）同族但方向相反（**假红**）。

---

## 8. 一句话结论

4 个单元 **78/78 个声明例程 + 8/8 个嵌套过程**全部 1:1 落地，**159 个用例**
（§4 的 **39 条原文缺陷**逐条有差异断言），
两条门禁 **0 error / 0 failed**（我改动的 8 个文件贡献 **0 条警告**）；
与既有 Core 寻路产物**同源不同单元**，已按任务书只移植差异部分并给出逐函数对照；
8 条阻塞项（依赖未移植/未接线/台账归属重叠）与 4 条风险已如实登记。

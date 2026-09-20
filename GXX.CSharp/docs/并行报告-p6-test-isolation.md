# 并行报告 · 车道 `p6-test-isolation`

（GXX Delphi7 → C# ｜ 分支 `par/p6-test-isolation` ｜ 工作树 `D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p6-test-isolation`）

> 独占区：`GXX.CSharp/tests/GXX.M2Server.Tests/M2ConfigIsolation*.cs`、`.../TestConfig.cs`、本报告。
> **没有修改任何既有测试文件，也没有碰任何在飞车道的独占区**：本车道相对开工基线只动 7 个路径
> （5 个新增 `M2ConfigIsolation*.cs` + `TestConfig.cs` + 本报告），其中前 6 个已被集成方合入 `main`（见 §1）。

---

## 0. 交付摘要（TL;DR）

| 项 | 值 |
|---|---|
| 机制 | **程序集级自定义 xUnit `TestFramework`**（执行期包一层 `IXunitTestCase`），每个测试用例前后反射快照/逐项还原静态全局 |
| 改动既有文件 | **仅 `TestConfig.cs` 加 1 行程序集特性**；既有测试文件 **0 改动** |
| 覆盖 | **26 个静态全局类型、1180 个成员槽位**（1155 可赋值 + 25 只读数组内容位；M2Config 占 1048），反射枚举、**非手写清单**；含 3 个 src 侧静态接缝（`NpcSeams`/`AuctionDbRunSeam`/`DbLayerRunSeam`，共 73 槽位，见 §1 第 6 次提交） |
| 复现 | 两类两顺序：修复前「污染→受害」1 失败 / 1 通过；「受害→污染」2 通过 → 顺序相关 |
| 修复后 | 两种顺序 **全绿**；整工程 **7298 passed / 0 failed**（基线 7286/0） |
| 整工程门禁 | `dotnet build GXX.slnx -c Debug` → 0 error ｜ `dotnet test` → 7298/0，27 s（与基线 27 s 持平） |
| 机制开销 | 实测 `Capture+Restore` = **0.164 ms / 用例**（7300 例 ≈ 1.2 s），墙钟无可测差异（基线与修复后均为 27 s） |

---

## 1. 全部 commit hash

| # | hash | 内容 |
|---|---|---|
| 1 | `60ae723a` | **WIP-不可合并**（预期红）复现：`M2ConfigIsolationReproTests.cs` 两个顺序的污染复现用例 |
| 2 | `b65ab679` | 修复：`M2ConfigIsolationFramework/State/Coverage/EngineTests.cs` + `TestConfig.cs` 程序集特性；整工程 7298/0 |
| 3 | `2576a38b` | 本报告 `GXX.CSharp/docs/并行报告-p6-test-isolation.md` |
| 4 | `429b5db1` | 报告补记 commit hash 与时延抖动说明 |
| 5 | `e9bd6758` | 报告登记「集成方已合入」这一事实并校正改动路径口径 |
| 6 | `HEAD`（`git log -1` 可查） | **裁定落实**：把 3 个在飞车道的 src 侧静态接缝纳入隔离覆盖（23→26 类型、1107→1180 槽位）+ 报告同步；整工程复测仍 7298/0，开销无可测变化（本文件自身的 hash 无法自指，故记作 HEAD） |
| — | `bbd64f3b` | **集成方提交**：`integrate par/p6-test-isolation` —— 已把步骤 1+2（6 文件 / 892 行）合入 `main`；此后本车道只剩本报告的若干次提交 |

> 步骤 1 的提交故意是红的（复现证据），已按纪律在 subject 标注 `WIP-不可合并`；**本车道 HEAD 不含 WIP**（`tools/Check-LaneReady.ps1` 对本车道显示无 WIP 标记）。
> 集成方在我写报告期间就完成了 `integrate par/p6-test-isolation`（合入的是步骤 2 之后的 HEAD，其时 subject 无 WIP），所以修复已在 `main` 上，本报告是**事后补充的说明材料**；第 6 次提交是**上级裁定「纳入 3 个 src 侧接缝」后新增的代码切片**，需要再合一次。

---

## 2. 机制选择与理由

**选程序集级自定义 `TestFramework`**（`XunitTestFramework.CreateExecutor` → 自定义 `XunitTestFrameworkExecutor.RunTestCases` → 包装 `IXunitTestCase.RunAsync`），理由：

1. **任务书首选方案且确实可行、稳定**。`CreateExecutor` / `RunTestCases`（`protected virtual`）是 xUnit 2.9.2 的公开扩展点，实测在本工程（VSTest + `xunit.runner.visualstudio` 2.8.2）工作正常，`RunAll` 与 `RunTests` 两条入口都汇到 `RunTestCases`，所以一个测试用例都逃不掉。
2. **零改既有文件、未来自动受保护**：不需要给 48 个污染文件逐类加标注；新写的测试类只要存在就自动被隔离 —— 而本缺陷的根因恰恰是「没人记得还原」。
3. 没有退而用 `BeforeAfterTestAttribute`：它无法程序集级生效（xUnit 2 只在**方法/类**上收集），逐类标注必然随新测试漂移，正是要根治的病。

实现要点：

* 快照点在内层用例 `RunAsync` **之前**（此时类构造器、`BeforeAfter` 特性都还没跑），还原在 `finally`（用例抛异常/取消也还原）。
* 还原目标是「**本用例开始前的状态**」而非「进程首次抓取的默认值」：既治好跨用例污染，又不会误伤 xUnit 允许的、在用例之外建立的上下文（例如将来引入 `IClassFixture`/`ICollectionFixture` 时 fixture 构造器里设的全局量）。这是刻意的语义选择，报告里明确记录。
* `Capture`/`Restore` 是纯函数 + 不可变快照，**幂等、可重入**（重复 `Restore` 返回 0 写入）；逐成员独立容错，读写异常记入运行期诊断而不是让整套挂掉。
* 数组做**元素级**快照（浅克隆 + 原实例引用），所以 `static readonly bool[]/uint[]` 这类「引用不可变但内容可变」的成员也能复位；`static readonly` 的非数组引用只做引用级（见 §8）。

---

## 3. 覆盖范围（反射口径）

### 3.1 纳入的类型（23 个，`M2ConfigIsolationCoverage.cs`）

| 类型 | 槽位 | 说明 |
|---|---|---|
| `Engine.M2Config` | **1048** | 主目标（Delphi `g_Config`），含 `MagicEx.cs`/`MagicConst.cs`/`ExpTables.g.cs`/`MagicBatchH.cs` 等 partial |
| `Engine.M2ShareState` | 12 | 含 4 个 **private** 静态字段 `_config/_expConfig/_commandConf/_stringConf` |
| `Engine.InterServerState` | 9 | 含只读槽位数组 `SrvArray` 的元素级保护 |
| `Engine.M2ShareLimits` | 6 | |
| `Forms.ViewList2State` | 5 | |
| `Forms.M2Forms` | 5 | `MessageBoxHandler/NextAnswer/LastMessage/HintColor` 等注入与捕获位 |
| `Engine.CustomHeroMagicState` | 4 | |
| `Forms.ViewListGroups` | 3 | |
| `Engine.ClientModuleState` | 3 | |
| `Engine.DbLayerGlobals` | 3 | 三个静态**自动属性** `Environment/Log/GetTickCount`（测试注入重灾区） |
| `GameCenter.GHeroDBConfig` | 3 | |
| `Engine.GameConfigState` | 2 | |
| `Engine.DropLimitGlobals` | 2 | |
| `Engine.MissionPageState` | 1 | |
| `Engine.SndaShopEnv` | 1 | |
| `Npc.NpcSeams` | **58** | src 侧静态接缝（静态委托持有者 + 全局标量）；测试里 200 处引用 |
| `DbLayer.AuctionDbRunSeam` | 8 | 同上（拍卖/UserShop 侧）；测试里 50 处引用 |
| `DbLayer.DbLayerRunSeam` | 7 | 同上（DbLayer 运行期接缝）；测试里 7 处引用 |
| `Engine.M2ShareGlobals` / `M2ShareAbilConfig` / `M2ShareFuncs` / `ViewListState3` / `CastleState` / `IdSocState` / `PluginManagerState` / `M2ServerLog` | 0 | 纳入清单但**只有 const / static readonly 非数组**，按口径无槽位（保留在清单里以便将来加字段自动受保护） |
| **合计** | **1180**（1155 可赋值） | 26 个类型 |

枚举规则（**不是手写成员清单**）：

* **字段**：`BindingFlags.Public|NonPublic|Static|DeclaredOnly` 全收 —— 排除 `const`（不可写，13 个）、排除「非数组的 `static readonly`」（37 个，见 §8）、排除自动属性后备字段（由属性槽接管，避免同一存储快照两次）。
* **属性**：只收**编译器生成 getter 的静态自动属性**（= 真有存储的）。手写 getter 的属性一律不碰，因为它们可能有副作用：`M2ShareState.ConfigIni` 会 `??=` 建对象、`M2Config.ModuleListPath` 会拼路径 —— 读它们本身就等于制造污染。自动属性的存储仍由字段口径覆盖，不会漏。
* **只有可赋值属性的 setter 才算可赋值**（识别 `IsExternalInit`，排除 `init`）。

### 3.2 明确排除的类型及理由

* **纯常量/查找表类**（`DropLimitConsts`/`ObjNpcConst`/`NpcCmdCodes`/`DataItemTypes`/`SqliteCodes`/`Sqlite*Sql`/`MySql*Sql`/`GameCommandTables`/`ItemSetAddValueTables`/`CombatPowerUtils`/`UseSlots`/`MagicACUtils` …）：只有 `const`/`static readonly`，没有可赋值静态状态，纳入是空转。
* **测试工程自己定义的接缝**（`CustomMagicFormGlobals` / `CustomMagicMessageBoxSeam` / `TVtEditorFactory` / `DbLayerTestKit` …）：不属于 `GXX.M2Server` 程序集，不归本机制管（上级裁定：测试侧接缝不纳入）。
* **src 侧静态接缝已按上级裁定纳入**（第 6 次提交）：`NpcSeams` / `AuctionDbRunSeam` / `DbLayerRunSeam` 三代「静态委托持有者」共 73 槽位。理由：各车道虽然都写了 `ResetDefaults()`，但那是**靠自觉**，而本机制的立论正是「不能靠自觉」；它们定义在 src 里，引用类型不修改任何在飞车道的文件。纳入后整工程仍是 7298/0、开销无可测变化。

---

## 4. 修复前 vs 修复后（对照证据）

### 4.1 复现方式

`.worktree` 内新建 `M2ConfigIsolationReproTests.cs`，**两类各一污染 + 一受害**：

* 顺序 A：`M2ConfigIsolationPolluterThenVictimTests` → `Step01_Polluter`（照抄 `FormGeneralConfigTests.cs:488`：`M2Config.sEnvirDir = "D:\\Mir\\Envir\\"` 且不还原）→ `Step02_Victim`（照抄 p3-m2-sweep 的 `SweepNationsTests`：用 `M2Config.sEnvirDir + "Nations\\Nations.txt"` 拼路径，并断言看到默认值 `.\Envir\`）。
* 顺序 B：`M2ConfigIsolationVictimThenPolluterTests` → 受害先跑。

> 顺序由**类级 `[TestCaseOrderer]`** 显式钉死。原因：实测 xUnit 2 的**同类内方法执行顺序不是声明顺序**（默认是 UniqueID 哈希序），第一次写复现时两个类都恰好先跑了污染方，靠「声明顺序」假设会写出不可靠的复现。

### 4.2 数字对照

| 场景 | 修复前（`60ae723a`） | 修复后（`b65ab679`） |
|---|---|---|
| 顺序 A（污染→受害） | **1 failed / 1 passed**（`Expected: ".\Envir\"  Actual: "D:\Mir\Envir\"`） | **2 passed** |
| 顺序 B（受害→污染） | 2 passed | **2 passed** |
| 两顺序一起跑（`--filter ~M2ConfigIsolation`） | **2 failed / 2 passed** | **12 passed / 0 failed** |
| 整工程 `GXX.M2Server.Tests` | 7286 passed / 0 failed / 27 s（基线 `6eb34f18`） | **7298 passed / 0 failed / 27 s** |
| 整工程 build | 0 error | **0 error** |

结论：修复前**顺序相关**（相同用例换个先后就红），修复后**顺序无关**（两顺序都绿）。

### 4.3 受损半径量测（临时实验，已回滚、未提交）

在 `TestConfig.cs` 里临时塞一个 `[ModuleInitializer]`，把 `FormGeneralConfigTests:481-491` 与 `:417-421` 写过的全局量（`sGateAddr/nGatePort/sServerName/nServerNumber/boTestServer/nTestLevel/nUserFull/sEnvirDir/g_sDBName/g_sSqliteDBName/g_boUseSqliteDB/sGuildDir/sGuildFile/sBoxsDir/sBoxsFile`）在装配加载时一次性污染，然后跑整工程：

* 结果：**7288 passed / 2 failed** —— 失败的**只有本车道的两个复现受害例**，既有 7286 例**全部存活**。

诚实结论：`sEnvirDir`/`sBoxsDir` 这类污染在**本工作树当前基线里没有现成受害者**，因为凡是用到它们的既有测试类都**自己把路径钉回临时目录**（最典型的是 `FormJ57Tests.cs:23` 的注释「sBoxsDir/sBoxsFile 是跨测试类共享的静态配置（FormGeneralConfigTests 会改写），此处显式钉住」）。真正的受害者是任务书里那批 **p3-m2-sweep 的 `SweepNationsTests`（尚未落进本工作树）**；本车道的复现用例就是它的替身。这些「各自打补丁」正是本车道要根治的现象。

---

## 5. 改动文件清单

**新增（独占区）**

| 文件 | 作用 |
|---|---|
| `tests/GXX.M2Server.Tests/M2ConfigIsolationFramework.cs` | 程序集级 `TestFramework` / `Executor` / `IXunitTestCase` 隔离壳 |
| `tests/GXX.M2Server.Tests/M2ConfigIsolationState.cs` | 反射快照/逐项还原引擎（含元素级数组保护） |
| `tests/GXX.M2Server.Tests/M2ConfigIsolationCoverage.cs` | 覆盖的 23 个静态全局类型 + 选取/排除依据 |
| `tests/GXX.M2Server.Tests/M2ConfigIsolationReproTests.cs` | 复现证据（两顺序）+ 顺序钉死用的 `ITestCaseOrderer` |
| `tests/GXX.M2Server.Tests/M2ConfigIsolationEngineTests.cs` | 8 条机制自检/回归守卫（覆盖度、幂等、null、数组、属性、诊断为空） |

**修改（独占区，仅 +3 行）**

| 文件 | 改动 |
|---|---|
| `tests/GXX.M2Server.Tests/TestConfig.cs` | 追加 `[assembly: TestFramework("GXX.M2Server.Tests.M2ConfigIsolationFramework", "GXX.M2Server.Tests")]` 及注释 |

**想改但无权改 → 现在不需要改（留给你知晓）**

本机制**不需要**改任何既有测试文件，因此下列「污染源」文件我一行未动（全部保持原样、原语义）：

`FormGeneralConfigTests.cs:481-491`（`sEnvirDir="D:\Mir\Envir\"`）、`CombatPowerTests.cs:21`、`CombatPowerSettingTests.cs:22`、`CustomHeroMagicTests.cs:21`、`CustomNpcFormTests.cs:23`、`CustomNpcUtilsTests.cs:22`、`CastleFormTests.cs:22`、`FormJ56Tests.cs:19`、`FormJ57Tests.cs:22-25`、`HeroMagicFormsTests.cs:22`、`FormJ69/J22/J23/J67/J26/J68/J57` 等按文件统计的 48/31/27/23/19/19/14 处 `M2Config.*` 赋值点。

> 在飞车道独占区文件（`Npc*` / `PlayerSurface*` / `GamePets*` / `Sweep*` / `CustomMagic*` / `DbLayer*` / `MirReturn*`）**一律未改**。

---

## 6. 整工程门禁结果

```
dotnet build GXX.slnx -c Debug --nologo          →  Build succeeded, 0 Error(s)
dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
                                                 →  Passed! Failed: 0, Passed: 7298, Skipped: 0, Total: 7298, Duration: 27 s
```

* 基线（开工前，同工作树）：`7286 passed / 0 failed / 27 s`。
* 增量 = 本车道新增 12 例（2 复现 + 2 顺序 + 8 自检）。
* **未出现基线漂移**：本车道全程未遇到「我没碰过的文件」的编译错误。
* 机制开销实测（临时探针，测完删除）：`Capture` 0.066 ms、`Capture+Restore` **0.164 ms / 用例** → 全量约 1.2 s，被噪声淹没（27 s ↔ 27 s）。期间观察到 43 s / 49 s / 51 s 的读数，事后确认是**同机其它车道并发跑 dotnet** 造成的抖动（同一二进制重复跑读数即 27↔51 波动）。

---

## 7. 发现的其它污染源与隐患

1. **`M2Forms.NextAnswer` / `MessageBoxHandler` / `LastMessage` / `HintColor`**：已有测试注入后不还原（本机制已覆盖）。这类 UI 接缝泄漏比路径泄漏更危险 —— 泄漏的 `MessageBoxHandler` 会让后续窗体测试走错分支，泄漏的模态开关会**挂死 testhost**（见 `CustomMagicTestBase.cs` 的注释）。
2. **`DbLayerGlobals.Environment/Log/GetTickCount`（静态自动属性）**：DbLayer 测试用它注入假环境/假时钟；`GetTickCount` 若泄漏成常量，重试/超时循环可能永远不结束。本机制已覆盖（这也是我把「自动属性」专门纳入口径的原因）。
3. **`M2ShareState` 的 4 个 private 静态缓存**（`_config/_expConfig/_commandConf/_stringConf`）：一旦按被污染的 `g_sSelfFilePath` 建过 `TFastIniFile`，就会一直用错路径。已纳入（引擎覆盖非公有静态字段）。`TFastIniFile` 是内存态（不持有文件句柄），复位引用不会泄漏句柄。
4. **`M2ShareAbilConfig.g_BaseAbilConfig` 这类「只读引用 + 深改内部」**：测试直接改 `g_BaseAbilConfig.UseDefault` / `HumAbil[0].Base[41]`（`AbilRecalcTests.cs:100-101`、`BaseAbilConfigTests.cs:13`），引用级快照抓不到（见 §8）。目前靠各测试自己 `ResetDefaults()` 兜住。
5. **`[assembly: CollectionBehavior(DisableTestParallelization = true)]` 是承重墙**：本次操作中我误用 PowerShell `Set-Content` 重写 `TestConfig.cs` 导致编码损坏、把该特性注释掉了 —— 套件随即**并行跑并挂死 10 分钟以上**。教训：**不要用 `Set-Content`/`-replace` 改这些 `.cs` 文件**（GBK/UTF-8 语义会翻车），请用编辑器工具。
6. **xUnit 2 的同类内方法顺序不是声明顺序**（UniqueID 哈希序）。任何「靠 A 先跑、B 后跑」来构造的测试或复现都不可靠；需要顺序时请用类级 `ITestCaseOrderer`。
7. **src 侧静态接缝是「静态委托持有者」，是跨测试污染的高危载体**：`NpcSeams`（测试里 200 处引用，只 13 处 Reset）、`AuctionDbRunSeam`（50/13）、`DbLayerRunSeam`（7/4）。各车道写了 `ResetDefaults()`，但那是**靠自觉**；按上级裁定已纳入隔离覆盖（73 槽位），从此不依赖自觉。

---

## 8. 未覆盖的部分（诚实说明）

1. **`static readonly` 的非数组引用成员：只做引用比对，不做内容深拷贝**（37 个，全部列在下面）。理由：没有通用安全的深拷贝（循环引用/委托/句柄/大表），强做会拖垮 7,300 例并引入新风险。这些成员的**内容**若被测试改动，仍会跨用例残留；目前靠各自测试的 `Reset*()` 兜住（`M2ShareAbilConfig.ResetDefaults()`、`ViewList2State.ResetForTests()`、`SndaShopEnv.Reset()`、`M2ShareGlobals.ResetNames()` 等）。
   清单：`M2Config.BonusAbilofTaos/Warr/Wizard`、`NakedAbilofTaos/Warr/Wizard`、`CustomMagicConfigs`、`ItemSetAddValues`、`ItemSetUnknowValues`、`PlayMonsterConfigs`、`SavedPlayMonsterNames`、`M2ShareGlobals.g_SellPlayerList`、`M2ShareAbilConfig.g_BaseAbilConfig`、`DropLimitGlobals.g_DropLimitMgr`、`InterServerState.GlobaSessionList`、`ViewList2State.g_BoxsList/g_EffectImageList/g_EffectItemList/g_GroupItems/g_ItemDescList/g_ItemDescTopList/g_ItemEffects/g_ItemRules/g_SkillPowerItemList/g_TzItemDescList/g_UserCmds`、`ViewListGroups.GroupLockObj`、`CastleState.g_CastleManager/g_GuildManager`、`MissionPageState.g_MissionPageCaptionList`、`ClientModuleState.g_BlackModuleList/g_ModuleList`、`IdSocState.FrmIDSoc`、`CustomHeroMagicState.CustomHeroMagicMgr`、`PluginManagerState.PlugList`、`M2ServerLog.Messages`、`NpcSeams._Rnd`（最后一个随第 6 次提交新增，是 `NpcSeams` 里唯一的只读非数组成员）。
2. **交错数组（`int[][]`）只回填外层引用**，内层数组被改元素抓不到（`M2Config.NewLevelMagicPowerRatesSpecific` 属此类）。
3. **手写 getter 的静态属性一律不接管**（怕副作用，见 §3.1）。若将来有人把状态放进手写属性且带副作用，需要单独处理。
4. **单个数组成员超过 65536 元素时不克隆内容**（只做引用比对），防御性上限，当前无人触发。
5. **`const` 字段不接管**（不可写，13 个）—— 无需接管。
6. **只覆盖 `GXX.M2Server` 程序集内的静态全局**。测试工程自己定义的静态接缝（其它车道的 `CustomMagicMessageBoxSeam` / `TVtEditorFactory` 等）不归本机制管。
7. **`M2ConfigIsolationEngineTests.Coverage_HasExpectedScale`** 用的是**下限守卫**（`types>=20`、`M2Config>=900`、`total>=1000`、`assignable>=900`）而不是精确等值：精确等值会在别人合法增删字段时变成假红。覆盖度真正靠 `Coverage_IncludesEveryAssignableStaticMember` 这条「逐成员必须命中」的守门用例。

# 并行报告 — 车道 `p8-m2-dummysetting`

> 单元：`Source/M2Engine/Forms/uFrmDummySetting.pas`（实测 **1,081** 行，GBK）+ 同源
> `.dfm`（34,189 字节，**122** 个类型声明控件 / **62** 个事件绑定）
> 工作树：`.worktrees/p8-m2-dummysetting` ｜ 分支：`par/p8-m2-dummysetting`
> 独占区：`GXX.CSharp/src/GXX.M2Server/Forms/DummySetting/**`、
> `GXX.CSharp/tests/GXX.M2Server.Tests/DummySetting*`

## 1. 全部 commit

| hash | 内容 |
|---|---|
| `48142041` | 切片 1-4：源侧 1:1 移植（7 个新文件，无测试） |
| `2eed010f` | **`WIP-不可合并`** —— 调度方按台账 §13.3 代为固化的抢救提交（我的产出，非我提交） |
| `9af61d4d` | 切片 5：测试套件落地（342 例）+ 诊断残留清零，全绿 |
| `d4c3a32f` | 切片 6：DFM 绑定勘误 + 全量核对，全绿（**末次提交**） |

> 门禁实测（`dotnet build GXX.slnx -c Debug` = **0 error**；
> `dotnet test GXX.M2Server.Tests` = **Failed 0 / Passed 8,705 / Total 8,705**）。
> 基线为本工作树实跑 **8,362**；新增 **343** 例。

## 2. 逐方法族判定表（66/66 已落地）

| 族 | 原文行号 | 判定 |
|---|---|---|
| `ShowFrmDummySetting` | :208 / :217-227 | ✅ 已完成（`ShowModal` 走 `DummySettingMessageBoxSeam`） |
| `FormCreate` | :229-325 | ✅ 已完成 |
| `lstDummyListClick` | :327-343 | ✅ 已完成 |
| `btnDummyLogonClick` | :345-374 | ✅ 已完成（4 个接缝） |
| `btnDummyAddClick` / `btnDummyDelClick` | :376-406 | ✅ 已完成 |
| `edtDummyHomeMapChange` | :408-411 | ✅ 已完成（**空体**，DFM :162 已绑定） |
| `seDummyHomeX/Y/LogonTimeChange` | :413-429 | ✅ 已完成 |
| `ButtonDummySaveClick` | :431-550 | ✅ 已完成（52 次 `Config.Write*` 全部逐字） |
| `btn1Click` | :552-614 | ✅ 已完成 |
| `chkDisDummyRunClick` | :616-663 | ✅ 已完成（规则抽成 `ApplyDisableDummyRun` 纯逻辑） |
| 21 个 Spin 处理器 | :665-789 / :803-819 / :885-895 | ✅ 已完成 |
| 4 个自动加血蓝 + 2 个修装/召英雄 | :671-801 | ✅ 已完成 |
| 9 个勾选处理器 | :821-883 / :897-901 | ✅ 已完成 |
| `ModValue` / `uModValue` | :903-911 | ✅ 已完成 |
| `lstDisableMoveMapClick` 等 ts4 六件 | :913-995 | ✅ 已完成 |
| `lstNoAttackMonListClick` 等 ts5 六件 | :997-1078 | ✅ 已完成 |
| 4 个单元私有的纯逻辑抽取 | — (托管侧新增) | ✅ 已完成（`ClampSpinValue`/`ApplyDisableDummyRun`/`AddSelectedMaps`/`AddSelectedMons`） |

**未覆盖：0 个方法。**

## 3. 新增文件与覆盖行号

| 文件 | 覆盖原文行号 |
|---|---|
| `src/GXX.M2Server/Forms/DummySetting/TFrmDummySetting.cs` | :10-227（类型面 + 接缝 + `ShowFrmDummySetting`） |
| `.../TFrmDummySetting.Components.cs` | DFM :1-1131（控件树 + 62 处事件绑定） |
| `.../TFrmDummySetting.Handlers.cs` | :229-1078（66 个过程全体） |
| `.../DummySettingControls.cs` | DFM 122 个控件的类型面声明 |
| `.../DummySettingConfig.cs` | M2Share.pas :1143-1153 / :2248-2262 / :2930-2952（43 字段） |
| `.../DummySettingGlobals.cs` | M2Share.pas :3708-3709 / :3914 / :16640-16678 / :12922-12934 / :12968-12980 |
| `.../DummySettingSeams.cs` | 接缝层（无头 UI 开关 + 控件最小面） |
| `tests/.../DummySettingTestBase.cs` | 测试夹具（343 例共用） |
| `tests/.../DummySettingFormCreateTests.cs` | :229-325 |
| `tests/.../DummySettingHandlerTests.cs` | :413-429 / :665-901 / :903-911 |
| `tests/.../DummySettingListTests.cs` | :327-343 / :552-614 / :913-1078 |
| `tests/.../DummySettingSaveTests.cs` | :431-550 |
| `tests/.../DummySettingGlobalsTests.cs` | :208 / :217-227 / :345-374 + M2Share 假人函数 |

### 外部审计证据（可复跑）
- 原文过程 66 个 → 实现 66 个（`grep -c '^procedure'` 去重）。
- DFM 类型声明控件 122 个 → `DummySettingControls` 122 字段 → `Components` 122 处实例化。
- DFM 唯一事件绑定 62 个 → `Components` 62 处 `+=` 绑定，一一对应。

## 4. 无头 UI 处置

| 类别 | 处置 |
|---|---|
| `Application.MessageBox`（:352/:437/:445） | **接缝** `TFrmDummySetting.MessageBoxHandler`，默认转调既有 `M2Forms.MessageBox`；测试统一注入 ⇒ 不弹窗 |
| `ShowModal`（:223） | **接缝** `DummySettingMessageBoxSeam.UiEnabled`（默认 `true`=生产）+ `ShowModalHandler`；测试全程 `false` |
| `SetFocus`（:353/:438/:446） | **决策镜像** `SetFocusProbe`（无头无焦点概念） |
| UI 取值/写回规则 | **抽成纯逻辑**：`ClampSpinValue` / `ApplyDisableDummyRun` / `AddSelectedMaps` / `AddSelectedMons`（各 ≥3 例） |
| 事件处理器 | 全部 `public` ⇒ **直调**（不需要消息泵/窗体句柄），343 例中绝大多数走这条路径 |
| `g_MapManager` / `UserEngine` / `Config` / `g_MultiThreadRun` | **接缝**（`FindMapHandler`/`MapListHandler`/`MonsterListHandler`/`GetPlayObjectHandler`/`FindDummyLogonHandler`/`AddDummyLogonHandler`/`WriteBool|Int|StringHandler`/`g_MultiThreadRun`+`MonsterListLockR|UnLockR`） |

### 确实无法断言的（如实标注，**未假装覆盖**）
1. **真实模态渲染**：`form.ShowDialog()` 的真实窗体验证 —— 无头环境跑不了（`UiEnabled=false`
   时按设计**不**调用 `ShowDialog`）。已覆盖的是"零工厂路径 / 工厂路径 / 工厂抛异常 /
   模态抛异常 / 计数"5 条。
2. **`TSpinEditEx.Value` 的即时重绘与 `OnChange` 触发时序**：WinForms `NumericUpDown.ValueChanged`
   已实测**同步**触发（有机制用例锁定），但"用户键入 → 控件内部先钳制再触发"的交互时序
   属控件内部，未覆盖。
3. **`.dfm` 里的字体/配色/位置像素级一致性**：只按 DFM 数值 1:1 落位，无渲染比对（无显示环境）。

## 5. 测试用例数 + build/test

- 本车道新增 **343** 例（`DummySetting*`）。
- `dotnet build GXX.slnx -c Debug` → **0 error**（91 warning，全部为既有 analyzer 提示）。
- `dotnet test GXX.M2Server.Tests` → **Failed 0 / Passed 8,705 / Total 8,705**。
- 基线本工作树实跑 8,362 ⇒ **零新增失败**。

## 6. 发现的原文缺陷 / 易错点（带 `文件:行`）

| # | 位置 | 性质 | 处置 |
|---|---|---|---|
| ① | `uFrmDummySetting.pas:320-321` | `lstXxx.Items.Assign(g_XxxList)` —— Delphi `TListBox.Items` **没有 `Assign` 方法**（`TStrings.Assign` 才有），这两行**无法按字面编译** | 1:1 保留语义（清空后逐项添加），注释说明；差异断言 `FormCreate_ListAssign_UsesClearThenAppend_AndPreservesOrder` |
| ② | `:924` / `:1008` | `if <List>.Items.Count >= 0 then` —— **恒真**（Count 不可能 < 0），外层 `if` 是死代码；但仍带 :937/:1023 的置脏副作用 | 1:1 保留；差异断言 `DisableMoveMapAdd_NoSelection_...` / `NoAttackMonAdd_NoSelection_...`（**一个都没选也会置脏**） |
| ③ | `:441` 在 `:443` 之前 | 保存时**先写** `g_Config.sDummyHomeMap`，**再**校验地图存在；校验失败走 `Exit` **不回滚** ⇒ 配置被污染 | 1:1 保留；差异断言 `Save_UnknownMap_StillWritesHomeMapIntoConfig_NoRollback_OriginalDefect` |
| ④ | `:500` | 配置键名写成 **`DiableDummyRun`**（少一个 `b`），与字段名 `boDiableDummyRun` 的拼写一致但**与直觉的 `DisableDummyRun` 不同** | 1:1 保留；差异断言 `Save_BoolKey_IsDiableDummyRun_OriginalSpellingDefectPreserved` |
| ⑤ | `:514-530` / `:532-547` | 保存按钮里的两个脏标志块**不清标志**（只有 `:993`/`:1077` 的清脏按钮才清）⇒ 第二次点保存**仍会再写盘** | 1:1 保留；`Save_TwiceInARow_SecondRunWritesMuchLess` + `Save_AfterDisableMoveMapSaveButton_DirtyFlagCleared_...` |
| ⑥ | `:856` | `boDummyWarHreoRun := chkDummyWarHreoRun.Enabled and chkDummyWarHreoRun.Checked` —— 取 **`Enabled` 与 `Checked` 的逻辑与**（不是直接取 `Checked`）⇒ 父勾选取消时子配置被**强制置 False**（即使子仍勾着） | 1:1 保留；差异断言 `WarDisHumRun_Uncheck_ForcesChildConfigFalse_EvenIfChildStillChecked` |
| ⑦ | DFM :162 + `:408-411` | `edtDummyHomeMap` **确实绑定了** `OnChange`，而处理器本体是**空体** | ⚠ **我在切片 1-4 里曾错误声称"未绑定"**，切片 6 已勘误（见 §7） |
| ⑧ | `:285` + `:616-620` | `chkDisDummyRun.Checked := not boDiableDummyRun` 的赋值**只在产生状态变化时**才触发 `OnClick` ⇒ `boDiableDummyRun` 的两种取值导致**完全不同的从属终态**（True 时级联不跑、从属不被清空；False 时触发级联并走 else 分支） | 两条路径各一条正式用例锁定 |
| ⑨ | `:291` 触发 `:958-963` | 父勾选回填会**连带**用 `子.Enabled and 子.Checked` 覆写 `g_Config.boDummyWarHreoRun`，而此刻子框**尚未回填** ⇒ 该式得 False ⇒ `:292` 回填子框得到 False | 1:1 保留；`FormCreate_WarDisHumRunTrue_WarHreoRunEndsDisabled_BecauseOfHandlerCascade` |
| ⑩ | `:336` / `:521` / `:985` / `:410` | 多处语句**省略行尾分号**（Pascal 允许 `end` 前最后一条语句省分号），而 `:539`/`:1069` 同型位置**有分号** —— 原文两种写法混用 | 1:1 保留并在注释里点明 |

## 7. ★ 我自己的错误（勘误，已修正）

切片 1-4 我在 3 个文件的注释里写下：
> 「`edtDummyHomeMapChange` 在 DFM 里**没有**绑定 `OnChange` ⇒ **永不触发**」

**该结论是错的。** 切片 6 的"全量核对"（把 DFM 的 `OnChange` 行数统计出来跟托管绑定数对账）抓出：
DFM 有 **62** 个唯一绑定，而我当时只绑了 **61** 个。
回读 `uFrmDummySetting.dfm:154-163` 确认 **:162 确有** `OnChange = edtDummyHomeMapChange`。

**修正内容**：
- `TFrmDummySetting.Components.cs`：补上 `TextChanged` 绑定，并把 `edtDummyHomeMap` 的
  DFM 属性补全（`Top=16`/`Width=150`/`Text='3'` —— 此前误写成 `Top=14`/`Width=70`）。
- 3 个文件的错误注释 → 改为勘误说明（原文处理器本体是空体，所以**行为不受影响**，
  但**绑定事实**必须照 DFM 保留）。
- 测试：删掉基于错误结论的用例，改为 3 条正确用例。

**教训（建议进台账规程）**：
> 「DFM 未绑定某事件」这类**否定性断言**必须用**计数对账**取证
> （DFM 绑定总数 vs 托管 `+=` 总数），不能靠"逐行扫一眼似乎没看到"。

## 8. 接缝清单 + 精确签名 + 越区请求

### 接缝（全部落在本车道独占区内，**无越区**）
```csharp
// TFrmDummySetting（Forms/DummySetting/TFrmDummySetting.cs）
public Func<string, string, int, int>? MessageBoxHandler;                      // Application.MessageBox
public Action<string>? SetFocusProbe;                                          // SetFocus 决策镜像
public Func<string, GXX.M2Server.Engine.TEnvirnoment?>? FindMapHandler;        // g_MapManager.FindMap
public Func<IReadOnlyList<GXX.M2Server.Engine.TEnvirnoment>?>? MapListHandler; // g_MapManager.Items
public Func<IReadOnlyList<(string sName, byte btRace)>?>? MonsterListHandler;  // UserEngine.MonsterList
public bool g_MultiThreadRun;                                                  // {$IF MULTI_THREAD = 1}
public Action<int>? MonsterListLockR;                                          // LockR(6)
public Action? MonsterListUnLockR;                                             // UnLockR
public Func<string, object?>? GetPlayObjectHandler;                            // UserEngine.GetPlayObject
public Func<string, bool>? FindDummyLogonHandler;                              // UserEngine.FindDummyLogon
public Action<TDummyLogon>? AddDummyLogonHandler;                              // UserEngine.AddDummyLogon
public Action<string, bool>? WriteBoolHandler;                                 // Config.WriteBool
public Action<string, int>? WriteIntegerHandler;                               // Config.WriteInteger
public Action<string, string>? WriteStringHandler;                             // Config.WriteString
public static void ShowFrmDummySetting(Func<TFrmDummySetting>? formFactory = null);

// DummySettingState（Forms/DummySetting/DummySettingGlobals.cs）
public static Action<TStringList, string>? SaveListToFile;                     // TGStringList.SaveToFile

// DummySettingMessageBoxSeam（Forms/DummySetting/DummySettingSeams.cs）
public static bool UiEnabled;                                                  // 无头开关
public static Func<int>? ShowModalHandler;
public static int ShowModalCount / LastModalResult;
```

### 越区请求：**无**
本车道**未修改任何独占区之外的文件**。特别地：
- **未改** `csproj` / `slnx` / `Directory.Build.props` / `tools/**` / `Checklist.md`。
- **未改** `src/GXX.M2Server/**` 其余部分（含 `M2ShareState` / `M2Config` 既有文件）。
- `Tests` 侧只新增 `DummySetting*` 文件，**未改** `M2ConfigIsolationCoverage.cs` / `StaRunner` 等既有文件
  （因此 `StaRunner`（在 `FormGeneralConfigTests.cs:590`）与 `M2Forms` 直接复用，未复制）。

### 需要集成方后续处理的（跨区，我无权动）
1. **`DummySettingConfig.cs` 的 43 个 `M2Config` 假人字段**：`M2ShareGlobals`（隔离清单
   `M2ConfigIsolationCoverage.cs:48` 提到的类型）**当前在托管侧并不存在**（已 `git grep` 确认 0 命中
   `class M2ShareGlobals`）。待 `M2Share.pas` 全量批次落地后**必须删掉本文件的重复声明**，否则 CS0102。
2. **`DummySettingGlobals.cs` 的三个 `TGStringList`** 同理，待 `M2Share.pas` 移植后合并。
3. **三个 `Load*` 未移植**（`LoadDummyNameList` / `LoadDummyDisableMoveMap` /
   `LoadDummyNoActiveAttackMonList`，M2Share.pas :16607 / :12890 / :12936）——
   它们**不在 `uFrmDummySetting.pas` 里**（原文窗体只有 `Save*`），属 M2Share 初始化段。
   窗体侧 `FormCreate` :320-321 只**读**这两个列表 ⇒ 读空列表与"Load 未跑"在窗体分支上不可区分。
   **待 M2Share.pas 批次接入。**
4. **`svMain.pas:3241` 的 `ShowFrmDummySetting` 调用点**：托管侧对应位置**未接**
   （`svMain.pas` 属只读区，且托管侧尚无该窗体宿主）。
   接入时只需一行：`GXX.M2Server.Forms.DummySetting.TFrmDummySetting.ShowFrmDummySetting();`
   （生产需先确保 `DummySettingMessageBoxSeam.UiEnabled == true`，即默认值 —— **无需改动**）。
   **这是本车道唯一请求集成方代为接线的一处，且我未自行修改任何只读文件。**
   （`DummySettingMessageBoxSeam.UiEnabled` 默认即 `true` ⇒ 生产接入**无需任何额外改动**。）

## 9. 三条正式偏离（按台账 §26/§30.3 编号登记）

### D-p8-01：`TGStringList` → `GXX.Core.Util.TStringList`
- **偏离点**：原文三个全局的类型是 `SDK.pas:80 TGStringList = class(TStringList)`（只加临界区）。
- **原文行为**：`g_DummyDisableMoveMapList.Sorted := True`（:524/:988）与
  `g_DummyNoActiveAttackMonList.Sorted := True`（:542/:1072）**真的会排序**。
- **托管行为**：`GXX.Core.Protocol.SDK.TGStringList` 是 **SDK.cs 的嵌套类**（`SDK.TGStringList`）
  且**只有 get-only 索引器、没有 `Sorted` 属性** ⇒ 照抄类型**写不出** `Sorted`。
  故改用 `GXX.Core.Util.TStringList`（该类型有真 `Sorted` 语义）。
- **为何必须偏离**：照抄类型会让原文 4 处排序**静默失效**（正是台账 §25.2 禁止的"接缝静默返回中性值"）。
- **恢复途径**：若日后 `SDK.TGStringList` 补上 `Sorted`（转发到底层 `TStringList`），可改回该类型，行为不变。

### D-p8-02：临界区改为单元级单把锁
- **偏离点**：原文 `TGStringList` 的临界区是**每实例**；托管侧三个列表是 `static readonly` 单例 ⇒
  用一把 `static readonly object` 串行化。
- **原文行为**：每实例一把临界区。
- **托管行为**：三个列表共用一把锁。
- **为何必须偏离**：托管侧只有一个静态实例，无"每实例"可言。
- **影响**：仅并发粒度；本窗体全部写入点都在单线程 UI 路径（`ButtonDummySaveClick` / `*SaveClick`）
  ⇒ **无可观测差异**。

### D-p8-03：`seDummy*HP/MP*` 族的下界
- **偏离点**：DFM 里 16 个 `seDummyHP*/MP*Time/Base_*` 是 `MinValue = 0 MaxValue = 0`
  （原文 `TSpinEditEx` 语义 = **完全不钳制**，可输负数）。
- **原文行为**：可设负数。
- **托管行为**：`ClampSpinValue(v, 0, 0)`（下界 0 / 上界不钳）；且 WinForms
  `NumericUpDown.Minimum` 在类型上就不允许负数。
- **为何必须偏离**：WinForms `NumericUpDown` 无法表达"无下界"。
- **影响面**：只有"配置里本来就是负数"才可见差异；对全部**正数**往返**完全等价**。
  已用 `ClampSpinValue_NegativeOnUnboundedFamily_ClampsToZero_DrivesFormCreatePath` 锁定。

## 10. 诚实说明：未完成部分与剩余量

**本单元已 100% 完成**（66/66 过程、122/122 控件、62/62 事件绑定、1,081 行全部覆盖）。

剩余量**不在本单元内**，只有上面 §8「需要集成方后续处理的」那 4 条跨区事项：
待 `M2Share.pas` 批次落地后合并重复声明 + 接入三个 `Load*` + 在 `svMain.pas` 对应位置接一行调用。

**我未做的事（明确列出，避免"假完成"）**：
1. 未做任何**.dfm 像素级渲染比对**（无显示环境）。
2. 未验证**真实模态窗体**的用户交互（无头环境按设计跳过）。
3. 未移植三个 `Load*`（按任务书硬性要求 2「不要顺手移植依赖」，且它们不在本单元内）。
4. 未把 `DummySettingState` 加进 `M2ConfigIsolationCoverage.cs` 的快照清单
   （该文件在测试工程根目录、不在我的独占区内 ⇒ 未越区）。
   缓解措施：`DummySettingTestBase.ResetStatics()` 每例前后显式 `Clear()` 三个列表，
   且 `DummySettingSerial` 集合 `DisableParallelization = true`。
   **建议集成方后续把该类型加入隔离清单**（一行），以获得双保险。

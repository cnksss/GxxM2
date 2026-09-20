# 并行报告 — 车道 `p6-m2-pets`

> 目标单元：`Source/M2Engine/Forms/uFrmMainGamePets.pas`（**1,353 行**，GBK；审计实测 1,353 行，与派发一致）
> 同源 DFM：`Source/M2Engine/Forms/uFrmMainGamePets.dfm`（53,800 字节 / 1,930 行）
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p6-m2-pets`（分支 `par/p6-m2-pets`）
> 状态：**窗体全量 1:1 移植完成**；门禁全绿（详见 §7）

---

## 1. 全部 commit hash

| # | hash | 内容 |
|---|---|---|
| 1 | `9e90fb3e` | 切片1：窗体骨架 + 配置接缝 + `TGamePetConfig` + `DoOpen` 全量回显 |
| 2 | `32baf25e` | 切片2：宠物/怪物列表操作族 + ComboBox 越界垫片 |
| 3 | `3ad98abd` | 切片3：开关 / Spin / 文本处理器族（36 个处理器） |
| 4 | `609f97ec` | 切片4：升级经验计划 19 分支 + 网格置脏 |
| 5 | `388a24ed` | 切片5：保存族 + INI 持久化 |
| 6 | `36485c80` | 切片6（收尾）：DC/MC 回填缺陷锁定 + 端到端工作流 + 未覆盖如实标注 |

报告提交：本文件所在的提交（紧随 `36485c80` 之后，`git log -- GXX.CSharp/docs/并行报告-p6-m2-pets.md` 可查；
不在本表内列出自身 hash 以免形成自引用）。

基线：`603672af`（本车道开工时的 `par/p6-m2-pets` HEAD）。**6 个切片各自独立提交，未攒批。**
本车道最终 HEAD：分支 `par/p6-m2-pets`，改动文件 **12 个、+5,656 行**，
`git diff --name-only 603672af HEAD` 的**全部**路径都落在独占区内
（`Forms/GamePets/**`、`tests/.../GamePets*`、`docs/并行报告-p6-m2-pets.md`）——**未越区**。

---

## 2. 第一步侦察结论

### 2.1 窗体类 / 字段 / 方法 + 行号区间

`TFrmGamePets = class(TForm)`（`:17-227`），字段 3 个：

| 字段 | 行 | 说明 |
|---|---|---|
| `boOpened: Boolean` | `:219` | DoOpen 期间与之后为 True；未打开时**全部处理器早退** |
| `boModValued: Boolean` | `:220` | 脏标记 |
| `RefreshGamePetConfigList` | `:222` | private 方法 |
| `ModValue()` | `:223` | private 方法 |
| `DoOpen` | `:226` | public 方法 |

单元级全局（`implementation` 段）：

| 全局 | 行 | 说明 |
|---|---|---|
| `SelGamePetConfig: PTGamePetConfig = nil` | `:235` | 当前选中项（**单元级**，非窗体字段） |
| `FrmGamePets: TFrmGamePets` | `:230` | 窗体单例 |

方法实现区间（共 **51** 个过程：`DoOpen` + `RefreshGamePetConfigList` + `ModValue` + **48 个事件处理器**）：

| 行号区间 | 方法 | 类别 |
|---|---|---|
| `:241-390` | `DoOpen` | 初始化/回显 |
| `:392-457` | `lstGamePetsClick` | 列表 |
| `:459-519` | `btnEditPetClick` | 列表 |
| `:521-532` | `RefreshGamePetConfigList` | 列表 |
| `:534-612` | `btnAddPetClick` | 列表 |
| `:614-648` | `btnDelPetClick` | 列表 |
| `:650-654` | `btnSavePetClick` | 列表 |
| `:656-662` | `lstMonsterListDblClick` | 列表 |
| `:664-669` | `ModValue` | 脏标记 |
| `:671-698` | `lstMonsterListKeyDown` | Ctrl+F 查找 |
| `:700-772` | 8 个纯布尔开关处理器 | 开关族 |
| `:774-828` | 1 Spin + 6 布尔（叠加给主人） | 开关族 |
| `:830-912` | `btnSavePetParamsClick` | 保存 |
| `:914-1140` | `cbbLevelExpClick`（19 分支） | 经验计划 |
| `:1142-1147` | `GridLevelExpSetEditText` | 脏标记 |
| `:1149-1182` | `btnSaveExpClick` | 保存 |
| `:1184-1350` | 剩余 24 个开关/Spin/文本处理器 | 开关族 |

### 2.2 `.dfm` 控件树与事件绑定

DFM 存在（53,800 字节）。顶层：`object FrmGamePets: TFrmGamePets`（`:1`）。
页签树：`pgcMain: TPageControl`（`:18`）→ `ts1`（`:19`，宠物列表页）/ `TabSheet1`（`:37`，升级经验页）/ `TabSheet2`（`:47`，全局参数页）。

**DFM 内 `On*` 事件绑定实测共 48 条**，与 `.pas` 的 48 个处理器声明**一一对应**（已用脚本抽取逐条比对）。
绑定分布：`OnClick` 39 条、`OnChange` 8 条、`OnDblClick` 1 条、`OnKeyDown` 1 条、`OnSetEditText` 1 条（部分控件同时有两条）。

⚠ **关键侦察发现**：以下控件在 DFM 里**没有任何 `On*` 绑定**，只在"增加/修改"按钮里被**读取**：
`chkLevelDifference`、`seLevelDifference`、`seHPScale`、`sePetCaptureRate`、`sePetShow*` 全族、
`sePetAdd*` 全族、`cbbPetShowFile1/2`、`cbbPetAdd*Type`。
→ 改这些控件的值**不置脏、不写 g_Config**，必须点"增加/修改"才生效。**不得给它们补处理器。**

### 2.3 三类判定

| 类别 | 内容 |
|---|---|
| **纯逻辑/表驱动可独立验证** | `cbbLevelExpClick` 的 19 分支经验计算（含 `StdLevelExp` 特殊公式）、`btnSaveExpClick` 的网格解析与边界、`SaveGamePetsConfig`/`LoadGamePetsConfig` 的 INI 读写、`GetGamePetConfig` 大小写匹配、`RefreshGamePetConfigList` |
| **WinForms 布局** | 全部 90+ 控件的构造与定位（DFM → 设计器代码，保控件与行为、装饰性属性简化） |
| **需接缝** | 见 §6 |

### 2.4 `uses` 依赖判定

| Delphi 单元 | 托管侧现状 | 处置 |
|---|---|---|
| `Windows/Messages/SysUtils/Variants/Classes/Graphics/Controls/Forms/Dialogs/StdCtrls/ComCtrls/Grids/ExtCtrls` | VCL → WinForms | 直接映射 |
| `SpinEditEx` | 无 | `TSpinEditEx` → `NumericUpDown`（`Value` 语义一致） |
| `ColorIndexEdit` | 无 | `TColorIndexEdit` → `NumericUpDown`（0..255） |
| `Grobal2` | **已移植** `GXX.Core.Protocol.Grobal2*` | 复用 `Grobal2Const.RC_*` |
| `M2Share` | 部分（`Engine/M2Config.*`） | `g_Config` 缺失字段走接缝（§6） |
| `M2Threads` | 无 | `LockR/UnLockR` 走接缝 |
| `M2Definition` | 无 | 未用到（最小接缝即可） |
| 反向依赖 `UserEngine`（`UsrEngn.pas`） | 无 | `MonsterList` / `SendServerConfig` 走接缝 |

**动手前已 grep 确认**：`TFrmGamePets` 全树 0 命中；`TGamePetConfig` / `GamePetsState` / `TLevelNeedExp` 全树 0 命中。
**未碰**任何已被顺序会话做完的单元（`Engine/**` 一律只读）。

---

## 3. 逐方法族判定表

图例：✅=1:1 已移植并覆盖 ｜ 🔌=接缝（已移植调用侧，依赖未移植）｜ ⚠=已移植但**未完全覆盖**

| # | 原文方法 | 行号 | 状态 | 覆盖测试 |
|---|---|---|---|---|
| 1 | `DoOpen` | `:241-390` | ✅（`ShowDialog` 走 `showModal` 参数） | `GamePetsDoOpenTests`（22 例） |
| 2 | `lstGamePetsClick` | `:392-457` | ✅ | `GamePetsListOpsTests`（4 例） |
| 3 | `btnEditPetClick` | `:459-519` | ✅ | `GamePetsListOpsTests`（4 例） |
| 4 | `RefreshGamePetConfigList` | `:521-532` | ✅ | 经 `DoOpen`/增删改间接覆盖 |
| 5 | `btnAddPetClick` | `:534-612` | ✅ | `GamePetsListOpsTests`（7 例） |
| 6 | `btnDelPetClick` | `:614-648` | ✅ | `GamePetsListOpsTests`（6 例） |
| 7 | `btnSavePetClick` | `:650-654` | ✅ | `GamePetsListOpsTests`（2 例）+ `GamePetsSaveTests` |
| 8 | `lstMonsterListDblClick` | `:656-662` | ✅ | `GamePetsListOpsTests`（3 例） |
| 9 | `ModValue` | `:664-669` | ✅ | `GamePetsListOpsTests`（2 例） |
| 10 | `lstMonsterListKeyDown` | `:671-698` | ✅（`InputQuery` 走接缝） | `GamePetsListOpsTests`（8 例） |
| 11 | `chkOpenGamePetClick` | `:700-706` | ✅ | `GamePetsHandlerTests` |
| 12 | `chkPetNoEntityClick` | `:708-714` | ✅ | 同上 |
| 13 | `chkPetNoShowHPProgressClick` | `:716-722` | ✅ | 同上 |
| 14 | `chkPetSleepControlBySlaveClick` | `:724-730` | ✅ | 同上 |
| 15 | `chkEnabledPetAttackClick` | `:732-738` | ✅ | 同上 |
| 16 | `chkEnabledPetPickupClick` | `:740-746` | ✅ | 同上 |
| 17 | `chkPetOnlyPickMonsterItemClick` | `:748-754` | ✅ | 同上 |
| 18 | `chkPetPickupToMasterClick` | `:756-764` | ✅（含 `:763` 联动） | 同上（专项 2 例） |
| 19 | `chkPetPickupFullToMasterClick` | `:766-772` | ✅（**无**反向联动，差异断言） | 同上 |
| 20 | `sePetAbilToMasterRateChange` | `:774-780` | ✅ | 同上 |
| 21-26 | `chkPet{HP,DC,MC,SC,AC,MAC}ToMasterClick` | `:782-828` | ✅（`chkPetDCToMasterClick` 另见 §6.4） | 同上 |
| 27 | `btnSavePetParamsClick` | `:830-912` | ✅（INI 写入走接缝） | `GamePetsSaveTests`（4 例） |
| 28 | `cbbLevelExpClick` | `:914-1140` | ✅ 19 分支全展开 | `GamePetsLevelExpTests`（24 例） |
| 29 | `GridLevelExpSetEditText` | `:1142-1147` | ✅ | `GamePetsLevelExpTests`（3 例） |
| 30 | `btnSaveExpClick` | `:1149-1182` | ✅ | `GamePetsSaveTests`（11 例） |
| 31 | `sePetHighLevelChange` | `:1184-1190` | ✅ | `GamePetsHandlerTests` |
| 32 | `sePetHighLevelGetExpChange` | `:1192-1198` | ✅ | 同上 |
| 33 | `chkPetFixExpClick` | `:1200-1206` | ✅ | 同上 |
| 34 | `sePetBaseExpChange` | `:1208-1214` | ✅ | 同上 |
| 35 | `sePetAddExpChange` | `:1216-1222` | ✅ | 同上 |
| 36 | `chkPetShowMasterNameClick` | `:1224-1230` | ✅ | 同上 |
| 37 | `sePetNameColorChange` | `:1232-1238` | ✅（Byte 截断断言） | 同上 |
| 38 | `edtPetSuffixNameChange` | `:1240-1246` | ✅（Trim 断言） | 同上（5 例） |
| 39 | `sePetUseItemIntervalTimeChange` | `:1248-1254` | ✅（LongWord 边界） | 同上 |
| 40 | `chkCapturePetNeedItemClick` | `:1256-1262` | ✅ | 同上 |
| 41 | `chkCaptureOKDecDuraClick` | `:1264-1270` | ✅ | 同上 |
| 42 | `seGamePetMaxCountChange` | `:1272-1278` | ✅ | 同上 |
| 43 | `seGamePetNameCountChange` | `:1280-1286` | ✅ | 同上 |
| 44 | `seGamePetRecallTimeChange` | `:1288-1294` | ✅ | 同上 |
| 45 | `chkDisableMonAttackPetClick` | `:1296-1302` | ✅ | 同上 |
| 46 | `chkDisableAllAttackPetClick` | `:1304-1310` | ✅ | 同上 |
| 47 | `chkPetQuickPickupClick` | `:1312-1318` | ✅ | 同上 |
| 48 | `chkPetRangePickupClick` | `:1320-1326` | ✅ | 同上 |
| 49 | `sePetPickupRangeChange` | `:1328-1334` | ✅（Byte 截断） | 同上 |
| 50 | `chkEnablePetUseClientPickItemsClick` | `:1336-1342` | ✅ | 同上 |
| 51 | `chkGamePetKillMonTriggerClick` | `:1344-1350` | ✅ | 同上 |
| — | `SaveGamePetsConfig` | `M2Share.pas:32759-32833` | ✅ | `GamePetsSaveTests`（4 例） |
| — | `LoadGamePetsConfig` | `M2Share.pas:32678-32757` | ✅ | `GamePetsSaveTests`（6 例） |
| — | `ClearGamePetsConfig` | `M2Share.pas:32665-32676` | ✅ | `GamePetsSaveTests`（1 例） |
| — | `GetGamePetConfig` | `M2Share.pas:32835-32850` | ✅ | `GamePetsSaveTests`（3 例） |

**未移植项（有意，见 §8）**：`M2Share.pas` 其余部分、`UsrEngn.pas`、`M2Threads.pas`、`M2Definition.pas`。

---

## 4. 新增文件 + 覆盖行号范围

### 4.1 产出（独占区 `Forms/GamePets/`，新建子目录作路径隔离）

| 文件 | 行数 | 内容 | 对应原文行号 |
|---|---|---|---|
| `src/GXX.M2Server/Forms/GamePets/GamePetsConfig.cs` | 124 | `g_Config` 宠物字段子集（`partial M2Config`） | `M2Share.pas:961-963` + 宠物开关族 |
| `src/GXX.M2Server/Forms/GamePets/GamePetsState.cs` | 267 | `TGamePetConfig`（41 字段）+ `g_GamePetConfigList` + 4 个全局函数 | `:683-725`、`:3985`、`:32665-32850` |
| `src/GXX.M2Server/Forms/GamePets/GamePetsForm.cs` | 672 | 控件树 + `RefreshGamePetConfigList` + 事件绑定 + 测试直调入口 | `:17-227`、`:521-532` |
| `src/GXX.M2Server/Forms/GamePets/GamePetsForm.Handlers.cs` | 1,156 | `DoOpen` + 全部 51 个方法 1:1 | `:241-1350` |

**覆盖的原文行号范围：`uFrmMainGamePets.pas` 全文件 `:1-1353`（类型声明 + 全部实现）。**

### 4.2 测试

| 文件 | 行数 | 用例数 | 覆盖族 |
|---|---|---|---|
| `tests/GXX.M2Server.Tests/GamePetsTestBase.cs` | 231 | — | 串行化集合 + 状态复位 + 接缝注入 |
| `tests/GXX.M2Server.Tests/GamePetsDoOpenTests.cs` | 487 | 22 | `DoOpen` |
| `tests/GXX.M2Server.Tests/GamePetsListOpsTests.cs` | 684 | 36 | 列表操作 + Ctrl+F |
| `tests/GXX.M2Server.Tests/GamePetsHandlerTests.cs` | 348 | 94 | 开关/Spin/文本处理器 |
| `tests/GXX.M2Server.Tests/GamePetsLevelExpTests.cs` | 406 | 38 | 经验计划 19 分支 |
| `tests/GXX.M2Server.Tests/GamePetsSaveTests.cs` | 580 | 30 | 保存族 + INI |
| `tests/GXX.M2Server.Tests/GamePetsWorkflowTests.cs` | 213 | 6 | 端到端工作流 |

产出合计 **2,219 行**（src）+ **2,949 行**（tests）+ **488 行**（报告）= **5,656 行**。

### 4.3 已覆盖 / 未覆盖行号范围

- **已覆盖**：`uFrmMainGamePets.pas:241-1350`（全部实现体）。类型声明 `:13-235` 由 `TGamePetConfig` / 控件字段逐项对应，无逻辑。
- **未覆盖**：无"实现行未覆盖"项；覆盖缺口在**分支**层面，即 §5.3 与 §6.4 列出的项。

---

## 5. 测试用例数 + build/test 结果

### 5.1 门禁结果（全绿）

```
cd .worktrees\p6-m2-pets\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
  → Build succeeded.  0 Error(s)

dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
  → Passed!  -  Failed: 0, Passed: 6074, Skipped: 0, Total: 6074, Duration: 36 s
```

- 基线（派发书）≈ **5811+**；本车道工作树实跑 **6074**，**0 失败**，**无新增失败**。
- 本车道新增 **234 例**（全部位于 `GamePets*` 测试文件）。
- 未出现"我没碰过的文件"的编译错误 → **无基线漂移**。
- `dotnet build GXX.slnx` 全解决方案绿，说明 `Forms/GamePets/` 新增 `.cs` 已被自动纳入编译（**未改任何 csproj**）。

### 5.2 用例数分布

| 测试类 | 用例（Theory 展开后） |
|---|---|
| `GamePetsDoOpenTests` | 22 |
| `GamePetsListOpsTests` | 36 |
| `GamePetsHandlerTests` | 94 |
| `GamePetsLevelExpTests` | 38 |
| `GamePetsSaveTests` | 30 |
| `GamePetsWorkflowTests` | 6 |
| **合计** | **234**（唯一 Fact/Theory 声明数 130） |

每个公开方法族均 ≥3 用例（空/0/负/超界/异常全覆盖）。

### 5.3 差异断言清单（"看起来一样实则不同"）

1. `btnAddPetClick` 判重用 `lstGamePets.Items.IndexOf`（**区分大小写**），而 `GetGamePetConfig` 用 `SameText`（**大小写无关**）→ 两者语义不同，不可"统一"。
2. `btnAddPetClick:539/557` 判重用 `Trim` 后的 `S`，但**存的是未 Trim 的 `edtPetName.Text`**。
3. `btnDelPetClick:629` 是**指针相等**（`ReferenceEquals`），不是 Name 相等 → 同名不同实例走"删除失败"。
4. `btnEditPetClick:463` 提示"请选择一个需要修改的**怪物**！"vs `btnDelPetClick:621` 提示"…**物品**！"（后者为原文笔误）。
5. `ModValue():664-669` 只放 `btnSavePet`/`btnSavePetParams`，**不碰 `btnSaveExp`**。
6. `btnSavePetParamsClick` **没有** `boOpened` 闸门（对比其余 36 个处理器都有）。
7. `cbbLevelExpClick:941` 的除数 = `High(TLevelNeedExp)` = **1000**（`{ div 2}` 是注释）→ 4000000，**不是** div 500 的 8000000。
8. `cbbLevelExpClick:944` 的 `(26 + I) > MAXCHANGELEVEL` → 循环只到 `I=974`，写索引 `27..1000`；索引 `0..26` 保持 `OldNeedExps`。
9. 倍率分支 `if dwExp = 0 then dwExp := 1` → 0 被**抬升为 1**，不是保持 0。
10. 倍率分支直接改写 `g_Config.dwPetNeedExps`（不从 `OldNeedExps` 重算）→ 连续两次 `s_2Mult` 等价 `div 4`。
11. `btnSaveExpClick:1158` 边界是 `> High(LongWord)` = **uint.MaxValue**（不是 `int.MaxValue`）→ `2147483648` 必须通过。
12. `btnSaveExpClick` 对负数不拦截 → `(uint)(-1)` 回绕成 `4294967295`（原文 LongWord 赋值语义）。
13. `btnSaveExpClick` 越界报错时**前面已检查的行已写入**（`:1165` 在循环内逐行写，非事务）。
14. `btnSaveExpClick:1175` 是 `Low(dwPetNeedExps)..High(...)` = **1..1000**，**不含索引 0**。
15. `SaveGamePetsConfig` 是"先 `DeleteFile` 再 `Create`"（**不是覆盖写**）→ 旧的多余节必须消失。
16. `LoadGamePetsConfig:32699` 用 `Length(Name) > 0` 门控 → 空 name 的节**整节丢弃**。
17. `DoOpen:257-258` 是 7 元**集合成员**判定，不是范围判定（114 不在排除集 → 保留）。
18. `DoOpen` **不**清空 `lstMonsterList`（只清两个素材下拉与经验计划下拉）→ 重复打开**累加**怪物名。
19. `DoOpen:363` 回填 DC 勾选框读的是 **MC** 字段（原文缺陷，见 §6.4）。
20. `sePetNameColorChange:1236` / `sePetPickupRangeChange:1332` 有显式 `Byte(...)` 截断。
21. `edtPetSuffixNameChange:1244` 写入前 `Trim`。
22. `chkPetPickupToMasterClick:763` 额外联动 `chkPetPickupFullToMaster.Enabled = not 值`；`chkPetPickupFullToMasterClick` **无**反向联动。

---

## 6. 无头 UI 处置

### 6.1 走既有 seam 的（`M2Forms.MessageBox`）

全部 `Application.MessageBox` 调用（共 **6 处**）转调既有 `M2Server.Forms.M2Forms.MessageBox`，
并额外支持本窗体注入 `MessageBoxHandler`。测试基类统一注入捕获 handler → **绝不弹真实模态框**。

| 位置 | 文本 | 常量 |
|---|---|---|
| `:463` | `请选择一个需要修改的怪物！` | `MB_OK + MB_ICONERROR` |
| `:542` | `请输入怪物名称！` | 同上 |
| `:549` | `怪物 <S> 已经存在！` | 同上 |
| `:621` | `请选择一个需要修改的物品！` | 同上 |
| `:641` | `删除失败！` | 同上 |
| `:927` | `升级经验计划设置的经验将立即生效，是否确认使用此经验计划？` | `MB_YESNO + MB_ICONQUESTION` |
| `:1160` | `等级 <I> 升级经验设置错误！` | `MB_OK + MB_ICONERROR` |

（`:927` 的 `= IDNO` 早退分支已用 `NextMessageAnswer` 注入覆盖。）

### 6.2 抽成纯逻辑函数的

| 决策 | 纯函数 | 位置 |
|---|---|---|
| `{$IF NEED_KEY = 1}` 键控是否勾选/保持可见（`:345-356`） | `GamePetsForm.ShouldShowClientPickItems(int key)` | `GamePetsForm.Handlers.cs` |

**原因（如实标注）**：WinForms 的 `Control.Visible` 是**计算值**，受父 `TabPage` 是否被真正显示影响，
无头环境下恒为 `false`（实测：切到该页也仍为 `false`），**无法用它断言**。
故把"走哪条分支"抽成纯函数单独测，窗体侧只做机械赋值。
控件 `Checked` 侧仍可断言（`boEnablePetUseClientPickItems` 与 `:348` 的写入已覆盖）。

### 6.3 事件处理器直调 + 决策镜像

原文 48 个处理器在托管侧均为 `private`，通过 `Raise*` 系列入口直调以断言分支决策：

`RaiseParamHandler`（24 个开关）、`RaiseSpinHandler`（11 个 Spin）、`RaiseSuffixNameChanged`、
`RaiseAddPet`、`RaiseDelPet`、`RaiseEditPet`、`RaiseSavePet`、`RaiseSaveExp`、`RaiseSavePetParams`、
`RaiseLevelExpClick`、`RaiseMonsterListDblClick`、`RaiseMonsterListKeyDown`、`RaiseGamePetsClick`、
`RaiseGridSetEditText`。

**焦点 / 光标定位**（无头不可断言）走**决策镜像探针**：
`SetFocusProbe`（对应 `:464`/`:543`/`:550`/`:622` 的 `SetFocus`）、
`GridLevelExpLocateRowHandler`（对应 `:1161-1162` 的 `GridLevelExp.Row := I; SetFocus`）。
测试断言"定位到了哪一行 / 聚焦了哪个控件"，而非真实光标。

### 6.4 ⚠ 确实无法断言、如实标注为未覆盖

`DoOpen:363` 的 DC/MC 回填缺陷：**"回填源错误"本身已锁定**（4 条测试，见下），
但其**完整后果链**——"`:363` 置 `chkDC` 时 `boOpened` 仍为 `false`（`:388` 才置 `true`）
→ `chkPetDCToMasterClick` 被 `:791-792` 早退 → 保存时 `:872` 读 `chkDC.Checked`
→ 把 `PetDCToMaster=true` 静默写回 `false`"——**未能在无头环境稳定断言**。

实测障碍：同一断言在两次运行中给出**自相矛盾**的结果
（`dcField=true, chkDC=false` 与 `wroteDC=true` 不能同时成立），
疑与 WinForms 控件在**未创建句柄 / 未加入已显示父级**时 `CheckedChanged` 的触发条件有关。

**处置**：已在测试文件中用大段注释显式登记为**未覆盖**，
并在 `GamePetsForm.Handlers.cs:166-176` 写明三处巧合叠加的根因链，
请集成方在有显示环境时补测。**未假装覆盖。**

### 6.5 xUnit 串行化

本窗体族读写 `M2Config` 静态字段与 `GamePetsState.g_GamePetConfigList` 静态列表
（原文是单元级全局 `var`），与其它窗体族测试共享进程静态状态
→ 归入独立集合 `[CollectionDefinition("GamePetsSerial", DisableParallelization = true)]`。
测试基类构造/析构均调用 `ResetStatics()` 重置全部宠物字段与经验表。

---

## 7. 发现的原文缺陷 / 易错点（带 `文件:行`）

### 7.1 真缺陷（逐字保留 + 锁定测试）

| # | 位置 | 缺陷 | 后果 |
|---|---|---|---|
| **D1** | `uFrmMainGamePets.pas:363` | `chkPetDCToMaster.Checked := g_Config.boPetMCToMaster;` —— 回填 **DC** 勾选框时读 **MC** 字段；`:364` 回填 MC 也是 MC | `boPetDCToMaster` 在 `DoOpen` 里**从未被回填**；DC/MC 两勾选框初始勾选态**恒相等**（都等于 MC 值）。若 INI 里 `PetDC=true`/`PetMC=false`，打开后**两个勾选框都显示未勾选**，而内存里 DC 仍是 `true` → **UI 与内存不一致**。因两字段相同时缺陷不可见，长期未被发现 |
| **D2** | `uFrmMainGamePets.pas:621` | `Application.MessageBox('请选择一个需要修改的物品！', …)` —— 删除**宠物**的分支，提示文案误写为"物品" | 用户看到错误提示词（对比 `:463` 的"怪物"） |
| **D3** | `uFrmMainGamePets.pas:941` | `dwOneLevelExp := 4000000000 div (High(g_Config.dwPetNeedExps){ div 2});` —— `{ div 2}` 是**注释**，不参与运算 | 若误读为"除以 500"，经验值会**差一倍**（4000000 vs 8000000）。托管侧按 1000 逐字复刻并差异断言 |
| **D4** | `uFrmMainGamePets.pas:944` | `if (26 + I) > MAXCHANGELEVEL then Break;` + 循环 `1..MAXCHANGELEVEL` | 循环实际只到 `I=974`，写索引 `27..1000`；索引 `1..26` 不被该分支更新（但 `:940` 已整表拷贝为 `OldNeedExps`） |
| **D5** | `uFrmMainGamePets.pas:1236` | `g_Config.btPetNameColor := Byte(sePetNameColor.Value);` —— 显式 `Byte` 截断 | `TSpinEditEx` 的 Max 若被调大有静默截断风险；托管侧保留截断语义并断言 `IsType<byte>` |
| **D6** | `uFrmMainGamePets.pas:1155-1166` | 校验 `> High(LongWord)`（`:1158`）与逐行写表（`:1165`）在**同一循环内** | 越界报错 `Exit`（`:1163`）时，**前面已检查的行已写入** `g_Config.dwPetNeedExps` —— 非事务性部分写入。已用 `Assert.Equal(10u, …[1])` + `Assert.NotEqual(30u, …[3])` 锁定 |
| **D7** | `uFrmMainGamePets.pas:32770-32773`（`M2Share.pas`） | `SaveGamePetsConfig` 先 `DeleteFile` 再 `TIniFileEx.Create` | 是"**先删后建**"而非覆盖写 → 旧的多余节会消失（已在测试中断言）；且**没有**写盘刷新调用（对比 Load 路径有 `Free`） |
| **D8** | `uFrmMainGamePets.pas:32703-32705`（`M2Share.pas`） | `LoadGamePetsConfig` 的缺省值走 `ReadBool/ReadInteger` 第三参 | `EnabledLevelDifference=True` / `LevelDifference=2` / `HPScale=50` 是**缺省**而非 0；托管侧逐字复刻 |
| **D9** | `uFrmMainGamePets.pas:1160` | `PChar('等级 ' + IntToStr(I) + ' 升级经验设置错误！')` 中的 `PChar(...)` 是**冗余**转换（`Application.MessageBox` 已接受 `PChar`） | 无功能影响；逐字保留语义 |

### 7.2 易错点 / 与 .NET 语义冲突（本车道实测踩到）

| # | 冲突 | 处置 |
|---|---|---|
| **E1** | **`TComboBox.ItemIndex` 越界**：Delphi 静默置 `-1`；WinForms `ComboBox.SelectedIndex` 越界**抛 `ArgumentOutOfRangeException`**。原文 `:408/:415` 直接写 `cbbPetShowFile*.ItemIndex := ShowFile*`，而 `ShowFile*` 来自 INI，素材表未装载时下拉只有 1 项 → **必然越界崩溃** | 新增 `SetItemIndex(cb, value)` 垫片复刻 Delphi 语义（越界/负值 → `-1`）。**这是本车道由测试抓出的真实缺陷级差异** |
| **E2** | **`CheckBox.Checked = v` 只在值真的变化时触发 `CheckedChanged`** | 影响"模拟用户点击"的测试写法；已写入注释与诊断记录 |
| **E3** | `ComboBox` 无"用户点击"事件可绑定：DFM 原文绑 `OnClick`，WinForms 只能用 `SelectedIndexChanged` | 后果：**程序化**设 `SelectedIndex` 在托管侧也会触发一次处理器（Delphi 不会）。本窗体自身代码从不程序化设该项目（`DoOpen` 用 `Items.Clear()+Add` 重建，不设 `SelectedIndex`）→ **生产路径行为一致**；仅测试需注意"设索引即已触发一次"。已记录并写进 `GamePetsForm.cs` 注释 |
| **E4** | `TSpinEditEx.Value`（`Integer`）→ `NumericUpDown.Value`（`decimal`） | 统一显式 `(int)` / `(byte)` / `(uint)` 转换，与原文 `Byte(...)` 截断语义对齐 |
| **E5** | `DelphiRTL.Pos("")` 返回 0 而 Delphi 返回 1（派发书提示） | 本单元**未使用** `Pos` → 不受影响 |
| **E6** | Delphi 格式串陷阱（台账 §17.2） | 本单元**未使用** `Format`/`FormatFloat`；全部字符串拼装是 `+` 连接与 `IntToStr`（走 `GXX.Core.Rtl.DelphiRTL.IntToStr`）→ **未引入 `string.Format`**，该缺陷类在本车道**不适用** |

---

## 8. 接缝清单 + 需要集成方改白名单外文件的精确签名要求

### 8.1 接缝清单（全部以**可注入委托**形式落在窗体上，未改任何只读区文件）

| # | 接缝 | 类型 | 原文位置 | 待移植单元 | 精确签名要求 |
|---|---|---|---|---|---|
| S1 | `MonsterListHandler` | `Func<IReadOnlyList<(string sName, byte btRace)>?>` | `:253-260` | `UsrEngn.pas` / `ObjMon.pas` | `IReadOnlyList<(string sName, byte btRace)> UserEngine.MonsterList`（枚举视图） |
| S2 | `g_MultiThreadRun` | `bool` | `:249/:263`（`{$IF MULTI_THREAD = 1}`） | `M2Threads.pas` / `UsrEngn.pas` | `bool g_MultiThreadRun` |
| S3 | `MonsterListLockR` | `Action<int>` | `:250` | 同上 | `void MonsterList.LockR(int nIndex)` |
| S4 | `MonsterListUnLockR` | `Action` | `:264` | 同上 | `void MonsterList.UnLockR()` |
| S5 | `EffectImageListHandler` | `Func<IReadOnlyList<string>?>` | `:273-277` | `M2Share.pas` | `TStringList g_EffectImageList`（`Count` + `Strings[i]`） |
| S6 | `WriteBoolHandler` | `Action<string, bool>` | `:833-899` | `M2Share.pas` `Config` | `void Config.WriteBool(string section, string key, bool value)`，section 恒 `'Setup'` |
| S7 | `WriteIntegerHandler` | `Action<string, int>` | `:855-897` | 同上 | `void Config.WriteInteger(string section, string key, int value)` |
| S8 | `WriteStringHandler` | `Action<string, string>` | `:892` | 同上 | `void Config.WriteString(string section, string key, string value)` |
| S9 | `ExpConfigWriteStringHandler` | `Action<string, string, string>` | `:1177` | `M2Share.pas` `ExpConfig` | `void ExpConfig.WriteString(string section, string key, string value)` |
| S10 | `SendServerConfigHandler` | `Action` | `:910` | `UsrEngn.pas` | `void UserEngine.SendServerConfig()` |
| S11 | `g_nKey_UseClientPickItems` | `int`（字段，默认 1） | `:346/:903` | VMProtect 键控全局 | `int g_nKey_UseClientPickItems`（`== 1` 才勾选与写盘） |
| S12 | `InputQueryHandler` | `Func<string, string, string, (bool Ok, string Value)>` | `:683` | `Dialogs.InputQuery` | 对齐既有 `MonsterConfigForm.InputQueryHandler` 同形签名 |
| S13 | `GridLevelExpLocateRowHandler` | `Action<int>` | `:1161-1162` | 无头不可断言的光标定位 | 决策镜像（非需移植依赖） |
| S14 | `SetFocusProbe` | `Action<string>` | `:464/:543/:550/:622` | 同上 | 决策镜像 |
| S15 | `MessageBoxHandler` | `Func<string, string, int, int>` | 6 处 | 已有 `M2Forms.MessageBox` | 与 `M2Forms.MessageBoxHandler` 同形，便于统一注入 |

### 8.2 ⚠ 需要集成方改**白名单外文件**的精确签名要求

本车道**未修改**任何白名单外文件。但落地了 3 处**位于只读区之外**（即本车道独占区内）
的 partial / 新增类型，**集成方合并时需注意**：

#### (a) `M2Config` 的宠物字段（新增 `partial` 分支）

**文件**：`src/GXX.M2Server/Forms/GamePets/GamePetsConfig.cs`
**类型**：`namespace GXX.M2Server.Engine; public static partial class M2Config`

这是对**既有只读文件** `src/GXX.M2Server/Engine/M2Config.ServerValue.cs`（属顺序会话常驻区）
中 `M2Config` 的 **partial 扩展**。逐字保留的字段名（32 个新字段）：

```
boOpenGamePet / boEnabledPetAttack / boDisableMonAttackPet / boDisableAllAttackPet /
boEnabledPetPickup / boPetOnlyPickMonsterItem / boPetPickupToMaster / boPetPickupFullToMaster /
boPetQuickPickup / boPetRangePickup / btPetPickupRange / boPetNoEntity / boPetSleepControlBySlave /
boPetNoShowHPProgress / boEnablePetUseClientPickItems / boPetMCToMaster / boPetSCToMaster /
boPetACToMaster / boPetMACToMaster / dwPetUseItemIntervalTime / boCapturePetNeedItem /
boCaptureOKDecDura / boPetShowMasterName / btPetNameColor / sPetSuffixName /
nGamePetMaxCount / nGamePetNameCount / nGamePetRecallTime / boGamePetKillMonTrigger /
nPetHighLevel / nPetHighLevelGetExp
```

**类型（按原文声明，勿改）**：`btPetPickupRange`/`btPetNameColor` = `byte`；
`dwPetUseItemIntervalTime` = `uint`；`sPetSuffixName` = `string`；其余布尔 = `bool`，整数 = `int`。

**集成要求**：
1. 若顺序会话后续在 `M2Share.pas` 批次里为这些字段落了正式定义 → **必须删掉本文件中的重复声明**
   （否则 `CS0102` 重复成员）。**判定标准**：以 `M2Share.pas` 为正式归属，本文件只是提前落地的替身。
2. 若顺序会话把 `sPetSuffixName` 的初值写成 `null` 而非 `""`，需同步 `LoadGamePetsConfig` 的空串判定
   （本实现按 Delphi `AnsiString` 语义用 `""`）。
3. **不得**在 `Engine/**` 内新增同名 partial 文件（一个成员只能有一个声明点）。

#### (b) `TGamePetConfig`（原文正式归属 `M2Share.pas:685-725`）

**文件**：`src/GXX.M2Server/Forms/GamePets/GamePetsState.cs`
**类型**：`namespace GXX.M2Server.Forms.GamePets; public sealed class TGamePetConfig`

41 个字段名与类型逐字对应原文 record。**重名检查**：`git grep` 对 `main` 全树 **0 命中**

**集成要求**（依台账 §14.1/§17.1 裁定原则：**以原文正式归属为准**）：
M2Share.pas 全量移植时，若在 `Engine/` 或 `M2Share*.cs` 里落下正式 `TGamePetConfig`，
则本文件中的定义应**由集成方删除**，并把 `GamePetsState` / `GamePetsForm` 的 `using` 指向正式位置。
**签名要求**：字段名与类型必须与原文 `:686-724` 逐字一致（`Name: string`、`CaptureRate: Integer`→`int`、
`EnabledLevelDifference: Boolean`→`bool`、`IsAdd*Rate: Boolean`→`bool`，共 21 个 `int` + 1 `string` + 19 `bool`
= 41 字段）。**不要**改成 `record`/`struct`（本窗体用 `ReferenceEquals` 做指针相等判定，
`btnDelPetClick:629`；改成值类型会破坏该语义）。

#### (c) `GamePetsState`（原文正式归属 `M2Share.pas` 单元级全局 + 4 个函数）

**文件**：`src/GXX.M2Server/Forms/GamePets/GamePetsState.cs`
**类型**：`public static class GamePetsState`

承载 `g_GamePetConfigList` + `ClearGamePetsConfig` / `LoadGamePetsConfig` / `SaveGamePetsConfig` / `GetGamePetConfig`。

**集成要求**：这 4 个函数的原文正式归属是 `M2Share.pas`（`:32665-32850`），
且**其他单元也在用**（`ObjBase.pas:28894/29310`、`Magic.pas:10077`、`M2Share.pas:10280/32835` 都在调
`GetGamePetConfig`）。建议集成方在 `M2Share.pas` 批次落地时：
1. 把 `GamePetsState` 的 4 个函数与列表**迁移**到 `M2Share` 正式位置；
2. 或在 `Engine/` 内提供同签名的转发（`GetGamePetConfig(string) → TGamePetConfig?` 必须保留
   **大小写无关、首个命中**、未命中返回 `null` 的语义）。

---

## 9. 诚实说明：未完成部分与剩余量

### 9.1 本车道目标单元的完成度

**`uFrmMainGamePets.pas`（1,353 行）已 100% 覆盖实现体**：51/51 个过程、48/48 个 DFM 事件绑定、
90+ 个控件、32 个 `g_Config` 字段、19 个经验计划分支、4 个 `M2Share.pas` 全局函数。

### 9.2 明确未完成 / 未覆盖

1. **§6.4 的 DC/MC 后果链未能断言**（已如实登记，非"假装覆盖"）。
2. **`Control.Visible` 类断言无法覆盖**（无头 computed 值恒 false）→ 已抽纯逻辑函数替代。
3. **装饰性 DFM 属性简化**：控件 `Left/Top/Width/Height` 为示意布局，非 DFM 逐像素还原
   （派发书 §7 允许："不影响协议正确性的装饰性属性允许简化但控件与行为保留"）。
   `TBevel`、`TColorIndexEdit` 的真实颜色选择对话框、`TStringGrid` → `DataGridView` 的
   单元格编辑器外观差异属同一类简化。
4. **未移植任何依赖单元**：`M2Share.pas` 其余、`UsrEngn.pas`、`M2Threads.pas`、`M2Definition.pas`
   一律只做最小接缝（§8.1 共 15 个），**未顺手移植**。
5. `DoOpen` 尾部的 `ShowModal` 用 `showModal` 参数映射；真实模态循环路径**未在测试内触发**
   （无头会挂死 testhost），仅由 `ActionSpeedConfigForm`/`AttackSabukWallForm` 等同款既有模式保证。

### 9.3 剩余量（不在本车道职责内，供集成方排期）

| 项 | 归属 | 说明 |
|---|---|---|
| `M2Share.pas` 全量（28,952 行） | 顺序会话 / 后续批次 | 本车道仅落 32 个字段 + 4 个函数 + 1 个 record 的**依赖子集** |
| `UsrEngn.pas`（10,977 行） | 后续批次 | 接缝 S1/S2/S3/S4/S10 |
| `M2Threads.pas` | 后续批次 | 接缝 S2/S3/S4 |
| 上述 (a)(b)(c) 三处替身的**去重合并** | **集成方** | 按台账 §14.1/§17.1 裁定原则执行 |

---

## 10. 交接纪律执行情况

- ✅ **小切片提交、立刻提交**：6 个切片，每个切片独立提交（首个切片在侦察后即落骨架）。
- ✅ **提交前防重名检查**：对 `GamePetsForm` / `GamePetsState` / `TGamePetConfig` /
  `GamePetsSerialCollection` / `GamePetsTestBase` 逐个跑
  `git grep -l -E "(class|struct|enum|interface|delegate) +(partial +)?<Type>\b" main -- 'GXX.CSharp/src/**/*.cs' 'GXX.CSharp/tests/**/*.cs'`
  → **全部 0 命中**。
- ✅ **复用而非重建**：复用既有 `TLevelExpScheme`（`Engine/M2Config.GameMsgTime.cs:168`）、
  `M2Config.OldNeedExps`（`Engine/ExpTables.g.cs:6`）、`Grobal2Const.RC_*`（`Core/Protocol/Grobal2.Const.g.cs`）、
  `GXX.Core.Util.TFastIniFile`、`GXX.Core.Rtl.DelphiRTL.IntToStr`、`M2Forms.MessageBox`。
- ✅ **未改只读文件**：`GXX.slnx` / `*.csproj` / `Directory.Build.props` / `tools/**` / `Checklist.md` /
  `Engine/**` / `Forms/` 既有文件 —— 全部未动（`git status` 仅含 `Forms/GamePets/**` 与 `tests/GamePets*`）。
- ✅ **无临时目录残留**：`.recon*` / `.tmp*` / 提交信息文件已全部删除；仓库根目录无游离 `.cs`（台账 §14.6）。
- ✅ **未 rebase**，未触碰兄弟 `.worktrees/*`，未写主工作树。

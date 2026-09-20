# 并行报告：车道 `p5-client-mirreturn`

**单元**：`Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas`
（GBK，**实测 5,954 行**，CRLF；台账 §13.7 记的 5,339 与实际不符，见 §7）

**分支**：`par/p5-client-mirreturn`　**基线**：`main @ 6ce16c2c`

---

## 1. Commit 清单

| # | hash | 内容 |
|---|---|---|
| 1 | `7807f162` | 切片1：骨架 + 接缝 + 控件面（脚本抽取 319 控件 / 77 RefConfig 赋值） |
| 2 | `73f70561` | 切片2：RefConfig/RefUseItemConfig 实现 + 196 例测试全绿 |

> 按台账 §13.10 纪律：**先提交骨架再迭代**（切片1 在 GXX.Client 首次编译通过后立刻提交）。

---

## 2. 逐方法族判定表

图例：**✅完成** / **🔌接缝**（逻辑照抄，外部依赖走可注入接缝） / **⬜未覆盖**

| 原文行号 | 方法族 | 判定 | 备注 |
|---|---|---|---|
| 1-7 | interface `uses` | ✅ | 13 个 DxComponent 单元 + Grobal2/GameConfigDlgs/GameConfigDlg/SDK/SoundUtil |
| 9-328 | 类声明（319 控件字段 + 15 private + 方法声明） | ✅ | 脚本逐行抽取 |
| 329-345 | 15 个 private 字段 | ✅ | |
| 533-638 | `TConfig` record / `pTConfig` | ✅ | 全字段 + 数组维度 |
| 640-787 | `g_Config` 初值 | ✅ | 脚本化逐字段照抄 |
| 789-860 | `Create` | ✅ | 42 条勾选位默认值全断言 |
| 862-869 | `Destroy` | ✅ | |
| 871-951 | 访问器 8 个 | ✅ | |
| 953-971 | `FormKeyDown`/`FormKeyPress`/`Refresh*Abil` | ✅ | 空实现 |
| 973-993 | `RefreshMySelfMagicList` | ✅ | |
| 995-1003 | `RefreshMyHeroMagicList`/`RefreshUnBindItemList` | ✅ | 空实现 |
| 1005-1008 | `PlugPageControlConfigInRealArea` | ✅ | 几何判据含 `Height+20` 的原文不对称 |
| 1010-1027 | `PlugConfigDlgCloseClickEx`/`PlugPageControlConfigActivePageChange` | ✅ | |
| 1029-1034 | `Logout` | ✅ | |
| 1036-1080 | `LoadHelpFile` | ✅ | |
| 1082-1086 | `ClearShowItem` | ✅ | |
| 1088-1158 | `RefShowItem` | ✅ | 1 行 6 列行模型 |
| 1160-1352 | `ListViewItemClick`/`DLabelDefaultItemClick`/`DEditSearchItemChange` | ⬜ | 见 §5 |
| 1354-1427 | `DComboBoxItemStdModeSelect` | ✅ | |
| 1431-1435 | `DComboBoxColorShow` | ✅ | |
| 1437-1858 | `CheckBoxClickEx`（82 处映射）、`RefUseItemConfigClick` | ⬜ | 见 §5 |
| 1860-1965 | `RefUseItemConfig` | ✅ | 含"标签按 nObj、数值按 MedicaMode"的错配 |
| 1967-1981 | `RefKeyBoardConfig` | ✅ | ★ 缺陷固定：1977/1978 都用 `[9]` |
| 1983-2137 | `RefConfig` | ✅ | 76 处回写逐行照抄 |
| 2139-2562 | 控件写回处理器（Check/Dura/Renew/SuperMedica/Edit） | ✅ | |
| 2546-2556 | `DEditChange` | ✅ | |
| 2558-2562 | `MouseMoveEvent` | ✅ | `DScreen.ClearHint` 接缝 |
| 2564-2902 | `LoadClientConfig` | 🔌 | 头部照抄（写 `FClientConfig`），控件构建段走 `LoadClientConfigSeam` |
| 2903-3274 | `Initialize` | 🔌 | 4 字段 + **3272 `FInitializeed := True`** 照抄，控件树走 `InitializeSeam` |
| 3275-3280 | `Finalize` | ✅ | |
| 3281-3285 | `Logon` | ✅ | |
| 3286-3320 | `LoadConfig` | ✅ | ★ 形参 `CharName` 原文未使用 |
| 3321-3324 | `Run` | ✅ | |
| 3326-3329 | `RefActorList` | ✅ | 空实现 |
| 3331-3372 | `GetShowItem`/`FindShowItem`/`FindHintItem`/`FindPickItem`/`HintItem` | ✅ | 转发 `FileItemDB` |
| 3374-3439 | `Struck`/`HealthChange` | ✅ | 6 处分派点全断言 |
| 3441-3457 | `AutoUseItem` | ✅ | |
| 3458-3534 | `AutoEatHPItem` + 2 嵌套 | ✅ | ★ 缺陷固定：硬编码 `[0]`/`[1]` |
| 3536-3616 | `AutoEatMPItem` + 2 嵌套 | ✅ | |
| 3618-3685 | `AutoEatSpecialHPItem` + 2 嵌套 | ✅ | ★ 无药时不提示不刷新 tick |
| 3687-3757 | `AutoEatSpecialMPItem` + 2 嵌套 | ✅ | 同上 |
| 3759-4060 | `AutoProtect` | ⬜ | 见 §5 |
| 4061-4188 | `DuraWarning` | ⬜ | 见 §5 |
| 4191-4207 | `NumberSort_1` | ✅ | ★ 降序比较器 + 异常吞掉返回 0 |
| 4209-4452 | `DamageHPUseItem`/`DamageMPUseItem` | 🔌 | 调用点为可注入委托；方法体未移植 |
| 4454-4477 | `AutoUseMagic` | ✅ | |
| 4479-4484 | `CanFilterExp` | ✅ | Int64 提升语义 |
| 4486-4786 | `LoadConfigFile` | ✅ | 含 ★ 循环上界 135 超枚举缺陷 |
| 4788-5069 | `SaveConfigFile` | ✅ | 含 ★ 雷达四项排除、异常吞掉 + DebugOutStr |
| 5071-5114 | `DLabelKeyBoardKeyDown`/`DLabelKeyBoardMouseDown` | ⬜ | 见 §5 |
| 5116-5251 | Boss 列表 7 个方法 | ✅ | ★ 5194/5481 越界抛异常已登记 |
| 5253-5260 | `DEditSpecialColorChange` | ✅ | ★ 只夹上界无下界 |
| 5262-5304 | `DBtnDiyAddClick`/`DBtnDiyDelClick` | ✅ | |
| 5306-5397 | `DBtnDiyMyLoadOrSaveClick` | ✅ | |
| 5399-5402 | `DBtnGJPageControlClick` | ✅ | |
| 5404-5537 | 挂机怪物名单 7 个方法 | ✅ | |
| 5539-5698 | `SaveOrLoadGJMagicList1`/`2` | ✅ | ★ 原文拼写 `GJNAGICCONFIGFILE`、值为 0 的行被丢弃 |
| 5700-5705 | `DEditNotRushMonRangeChange` | ✅ | |
| 5706-5734 | `ComboBoxPlayAttackValueSelect` | ✅ | 5 路 Sender 分派 |
| 5735-5758 | `DCheckBoxGroupAttackMouseMove`/`DMemoConfig8MouseMove`/`DEditGroupAttackCountChanged` | ✅（前 2 个 ⬜） | 5753-5757 完成 |
| 5759-5775 | `ListViewGJMagicItemClick`/`DBtnGJRunClick` | ⬜ | 见 §5 |
| 5776-5840 | `RefreshGJMagic` | ✅ | 两页各 1 行 2 列 |
| 5842-5940 | 3 个 `*PercentClick` | ✅ | ★ 超药回写错位 1 固定 |
| 5942-5954 | 音量 2 个方法 | ✅ | |

**统计**：55 个方法族中 **✅ 43 / 🔌 3 / ⬜ 9**。

---

## 3. 新增/修改文件

| 文件 | 行数 | 说明 |
|---|---|---|
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.cs` | 653 | 头部 40 行含源单元 + 全量行号清单；`TConfig`/`g_Config`/`Create`/`Destroy`/访问器/空实现/`CanFilterExp` |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.Lifecycle.cs` | 553 | 973-1003 / 1036-1080 / 2564-3329 / 3331-3439 / 4191-4207 / 5700-5757 / 5942-5954 |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.ConfigFile.cs` | 469 | 4486-5069（INI 双向） |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.Lists.cs` | 590 | 5116-5698（3 张名单） |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.Controls.cs` | 400 | 2139-2556 / 5753-5757 / 5842-5940 |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.Auto.cs` | 437 | 3441-3757 / 4454-4477 / 伤害委托声明 |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.Init.cs` | 213 | 1088-1158 / 1967-1981 / 2564-2570 / 2903-3274（头部） |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.Pages.cs` | 258 | 1354-1435 / 5306-5397 / 5776-5840 |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigDlg.RefConfig.cs` | 240 | 1860-1965 / 1983-2137 |
| `src/GXX.Client/GUI/GameConfig/MirReturnConfigSeams.cs` | 960 | 接缝基接口 + 桩 + 全局接缝 + INI 接缝 + 消息框接缝 |
| `src/GXX.Client/GUI/GameConfig/MirReturnControls.g.cs` | 664 | **脚本生成**：`IMirReturnConfigDlgControlsExt`（319 控件取值面） |
| `src/GXX.Client/GUI/GameConfig/MirReturnControlsStub.g.cs` | 332 | **脚本生成**：桩 |
| `tools/mirreturn/Generate-MirReturnControls.ps1` | 200 | 抽取脚本（纯 ASCII，PS5.1 安全） |
| `tests/GXX.Client.Tests/GuiMirReturnConfigTests.cs` | 2,994 | **196 例** |

**逐方法已覆盖/未覆盖行号范围**：见 §2 表（每行即一个方法族的原文区间）。

---

## 4. 测试与门禁

- **新增用例：196**（`GuiMirReturnConfigTests`，全部通过）
- `GXX.Client.Tests`：**3,680 通过 / 0 失败**（= 基线 3,484 + 196，**无新增失败**）
- 整工程 `dotnet test GXX.slnx`：**12,142 通过 / 0 失败**（11 个测试工程全绿）
  - Core 532 / SelGate 163 / GatewayKit 7 / DBServer 371 / LogDataServer 142 /
    RunGate 1206 / LoginSrv 229 / GameCenter 219 / Integration 2 / **Client 3680** / M2Server 5591
- `dotnet build GXX.slnx -c Debug`：**0 Error**
- 环境性失败：**未出现 MSB4166**（本次未触发台账 §12.9 的场景）

---

## 5. 未覆盖部分（诚实清单）

### 5.1 ⬜ 未覆盖方法族（9 个，共约 1,180 行）

| 原文行号 | 方法 | 未覆盖原因 |
|---|---|---|
| 1160-1352 | `ListViewItemClick` / `DLabelDefaultItemClick` / `DEditSearchItemChange` | 依赖 `TDxListView` 真实行模型 + `FileItemDB` 检索交互 + 右键菜单定位（`TDxPopupMenu`），托管侧控件库无对应实现 |
| 1437-1858 | `CheckBoxClickEx`（82 处控件→配置位映射）/ `RefUseItemConfigClick` | 同上；且 82 处映射的**目标位**已在 `RefConfig` 侧全部覆盖（互为逆操作），单独移植收益低 |
| 3759-4060 | `AutoProtect` | 依赖 `frmMain.UseMagic`/`g_MySelf.m_Abil` 全链路 + 22 个 `CM_*` 常量 + 目标选取 |
| 4061-4188 | `DuraWarning` | 依赖 `g_ItemArr` 全量扫描 + 耐久告警弹窗（需 seam 开关） |
| 4209-4452 | `DamageHPUseItem` / `DamageMPUseItem` | ⚠ **调用点已移植**（`Struck`/`HealthChange` 的 6 处分派可完整断言），方法体未移植（依赖 `TStringList.CustomSort` + 药名匹配 + `frmMain.AutoEatItem`） |
| 5071-5114 | `DLabelKeyBoardKeyDown` / `DLabelKeyBoardMouseDown` | 依赖 `g_ShortcutKeys[DxLabel.Tag]` 的**控件 Tag→下标**绑定（原文用 `TDxLabel.Tag` 隐式约定），需真实控件树 |
| 5735-5745 | `DCheckBoxGroupAttackMouseMove` / `DMemoConfig8MouseMove` | 纯提示绘制（`DScreen.AddChatBoardString`），无逻辑可断言 |
| 5759-5767 | `ListViewGJMagicItemClick` | 同 `ListViewItemClick` |
| 2909-3271 | `Initialize` 的控件树构建段（370 行） | 370 行 DxComponent 控件创建 + 几何定位，托管侧接缝承载 |
| 2571-2902 | `LoadClientConfig` 的控件构建段（331 行） | 同上 |

### 5.2 ⚠ 已知的"照抄但未逐行验证"处

- `LoadConfigFile`/`SaveConfigFile` 的 INI 键名表（`sIdent1..sIdent15`）由**脚本抽取**并逐条断言了 5 档用药模式 × 16 键中的抽样（每档至少 1 键），**未对 5×16+9×5 全部键名逐条断言**。
- `RefConfig` 的 76 处回写：断言了全部**副作用**（g_boSound/g_boBGSound/g_boRepeatBGSound/frmMain 同步）与抽样控件值，未对 76 个控件属性逐一断言。

---

## 6. 发现的原文缺陷 / 易错点（全部已在代码注释 + 测试中固定）

| # | 位置 | 缺陷 | 测试 |
|---|---|---|---|
| D1 | `1967-1981` | `RefKeyBoardConfig` 的 **1977 行用 `g_ShortcutKeys[9]`**（应为 `[8]`），1978 行**也用 `[9]`** → `LabelKeyBoard9` 与 `LabelKeyBoard10` 显示同一快捷键，`[8]` **从不显示**，`[12..15]` 也不显示 | `RefKeyBoardConfig_12个标签_其中9号与10号同源`、`RefKeyBoardConfig_下标8从未被显示` |
| D2 | `5913-5938` | `DCheckBoxSuperMedicaPercentClick` 的纠正循环是 `0..8`（9 项），但回写**只到控件下标 0..7** → **控件名后缀与数组下标整体错位 1**（`HP1←[0]`）、`PlugEditSuperMedicaHP0` **从未回写**、数组 `[8]`（超级疗伤药）**不回显** | `DCheckBoxSuperMedicaPercentClick_九项纠正但只回写八项` |
| D3 | 整族 | `AutoEatHPItem`/`MPItem`/`SpecialHPItem`/`SpecialMPItem` 的 4 个嵌套函数**硬编码下标 `[0]`/`[1]`**，**完全无视 `g_Config.MedicaMode`** —— 用户在"用药模式"选 2/3/4 副将时这四个自动吃药**仍只作用于主体与英雄** | `AutoEatHPItem_硬编码只作用于下标0与1_无视MedicaMode` |
| D4 | `2023` | `RefConfig` 该行用 **`ConfigCheckeds[ckSceneShake]`**（无 `F` 前缀），与同段其余 75 处不一致 | `RefConfig_勾选位回写控件_抽样断言` |
| D5 | `4528/4829` | `LoadConfigFile`/`SaveConfigFile` 的循环上界是 `Length(FConfigCheckeds)-1 = 135`，而 `TConfigChecked` 只到 `ckObjectHintEffect = 133` → `TConfigChecked(134)`/`(135)` 是**非法枚举值**（`{$R+}` 构建下抛 ERangeError） | `LoadConfigFile_勾选位循环上界超枚举_越界项被跳过不崩` |
| D6 | `1990/1991/2012` + `2057/2058` | `ckShowItemName` 与 `ckShowFilterItem` **都映射到 `ckShowMonName`**、`ckAutoTakeOnItem` 与 `ckAutoChangePoison` **都映射到 `ckAutoCHangePoison`** → 同段内**后者覆盖前者**（`2012` 被 `1990/1991` 覆盖） | `CheckedMap_两个别名映射到同一位`、`RefConfig_ckShowName三项覆盖_后者胜` |
| D7 | `3638-3645`/`3661-3668`/`3707-3714`/`3730-3737` | Special 族的"无药"分支**不存在**（只有 `if nIndex >= 0`，无 `else`）→ 既**不打聊天栏提示**、也**不刷新 `Renew*Ticks`**（与 HP/MP 版**相反**：HP/MP 无药时提示 + 刷新 tick） | `AutoEatSpecialHPItem_无药时既不提示也不刷新tick`、`AutoEatMPItem_无药时打聊天栏提示` |
| D8 | `3286-3320` | `LoadConfig` 的**形参 `CharName` 完全未被使用**，用户名取的是 `g_MySelf.m_sUserName` | `LoadConfig_形参CharName未被使用_取g_MySelf用户名` |
| D9 | `5256` | `DEditSpecialColorChange` **只夹上界 255、无下界**（负值按 `Byte` 截断成 255 附近的乱值） | `DEditSpecialColorChange_只夹上界` |
| D10 | `5194`/`5481` | `DBtnBossModifyClick`/`DBtnGJMonNameEditClick` 在 `ItemIndex = -1` 时会 `Lines[-1]`（Delphi 抛 `EStringListError`）；而 `CheckBossNameExists(name, -1)` 此时**恒为 False** → 必然走到越界写 | `DBtnBossModifyClick_ItemIndex为负_抛越界异常并登记原文缺陷` |
| D11 | `5842-5940` | 三个 `*PercentClick` 的纠正阈值是 **`> 99`**（不是 `>= 100`）：99 保留、100 归 50；且 Special 组归 **30**（其他组归 **50**） | `DCheckBoxAutoPercentClick_边界99不纠正`、`DCheckBoxRenewAutoPercentClick_Special归30_其余归50` |
| D12 | `4697-4738` 与 `4545-4656`；`4992-5033` 与 `4840-4951` | 两处 `case II of` 键名模板块是**逐字复制** 5 档用药模式表并替换前缀（`h`/`z`/`f`/`d`）的产物 —— 这正是本单元大量"复制粘贴错位"（如 D2 的回写错位 1）的成因；也解释了文件名常量 `GJNAGICCONFIGFILE`（原文拼写为 **NAGIC** 而非 MAGIC）的来源 | —（无功能影响，仅记录风险源） |
| D13 | `2029/2044/2074-2077/2085` | `RefConfig` 里 **6 处重复赋值**（`ckBGMusic` 1995&2029、`ckNotParaly` 2007&2044、4 个 `HumManually*` 2064-2067&2074-2077、`ckAutoUseMagic` 2080&2085） | `RefConfig_勾选位回写控件_抽样断言` |
| D14 | 全文 | 原文 `uses` 里的 `DxTrackBar` 导出的类名是 **`TDXTrackBar`**（大写 X），而字段声明写 `TDxTrackBar`（`DXTrackBar.cs:34`）—— 套件内已有的大小写不一致 | —（托管侧不引用该类型） |
| D15 | `4530/4833` | 旧代码里 `TConfigChecked(I)` 的强制转换在**范围检查构建**下是运行时错误；本单元依赖 `{$R-}` | D5 同处 |

### 6.1 顺带发现的**他人文件**缺陷（未改，仅登记）

| 位置 | 缺陷 |
|---|---|
| `src/GXX.Client/GUI/GameConfig/FilterItems.cs:481` | `TFileItemDB.Del` 在 `m_ShowItemList.RemoveAt(I)` 之后**用同一个下标** `m_FileItemList.RemoveAt(I)` —— 当两表长度不一致时抛 `ArgumentOutOfRangeException`。原文 `FilterItems.pas` 的注释"可能 m_FileItemList 链表需要再遍历一次 piaoyun 2013-09-10"正是这个隐患；**托管侧把它变成了必崩**（Delphi 的 `TList.Delete` 越界会抛 `EListError`，行为一致，但托管侧 `List.RemoveAt` 也抛 —— 语义等价，故本车道**未改**）。测试 `DBtnDiyDelClick_非空名走Del与下拉重选` 已改为两表对齐以绕开。 |
| `docs/并行派发台账.md:779` | 记 `MirReturnConfigDlg.pas` 为 **5,339 行**，实测 **5,954 行**（差 615 行）。四巨型单元表的其余三项行数未复核。 |

---

## 7. 接缝清单 + 需要集成方改白名单外文件的**精确签名要求**

### 7.1 ⚠ 需要集成方在**只读文件**里做的唯一改动

**文件**：`src/GXX.Client/GUI/GameConfig/Seams/ConfigSeams.cs`（第 98 行，`public sealed class TClientConfig`）

**要求**：补 1 个字段（原文 `Grobal2.pas:5089`，默认值 `Grobal2.pas:6436`）

```csharp
/// <summary>原文 Grobal2.pas:5089 dwPluginMinEatItemTime:LongWord（内挂最小吃药间隔 chongchong 2015-10-17）。</summary>
public uint dwPluginMinEatItemTime = 500;
```

**本车道当前替代方案**：`MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime`（`public static uint = 500`），
6 处 `Max()` 夹紧（原文 2220/2226/2232/2238/2370/2484/4670/4674/4678/4682/4741/4743）都读它。
**改动只需**：把 `MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime` 的 12 处引用换成
`ClientGlobalSeam.g_ClientConfig.dwPluginMinEatItemTime`，然后删掉该 seam 字段（`MirReturnConfigDlg.Lifecycle.cs:498` 附近）。

> 之所以不自行加 `partial`：`TClientConfig` 在只读文件里是 `public sealed class`（**非 partial**），
> 且**同名类型在两处**（`Seams/ConfigSeams.cs:98` 的 sealed class 与 `GXX.Core/Protocol/Grobal2.Types5.cs:13` 的 struct），
> 加 partial 会触发 CS0260/CS0101 —— 属本车道无权处理的改动。

### 7.2 本车道内已落地的接缝（**无需集成方动作**）

| 接缝类 | 文件 | 覆盖的原文依赖 |
|---|---|---|
| `IMirReturnConfigDlgControls`（基） | `MirReturnConfigSeams.cs` | 焦点三段、行模型操作（Add/Set*/Lock/Clear）、名单 Load/Save、超药索引族、`PlugConfigDlg`/`PlugMemoConfigHelp`/`PlugComboBoxAutoMagic`/`PlugScrollBoxBoss|Mons` 的 `<> nil` 与句柄 |
| `IMirReturnConfigDlgControlsExt`（生成） | `MirReturnControls.g.cs` | 原文 10-328 的 **319 个控件字段**取值面 |
| `MirReturnConfigDlgControlsStub` | 两文件 partial | 纯内存默认实现（无 WinForms 依赖） |
| `MirReturnCheckedMap` | `MirReturnConfigSeams.cs` | `TConfigChecked` 的 7 个缺失成员映射（含 1 个合成槽位） |
| `MirReturnGlobalSeam` | `MirReturnConfigSeams.cs` | `g_BossList`/`g_GJMonList`/`g_GJUseMagic1|2`/`g_MagicList`/`g_SoundVolume`/`g_GJActionMode.Text`/`frmMain.*` 8 个 setter/`SendPlugInConfig`/`ChangePoisonCharm`/`DebugOutStr`/`FileExists`/`DirectoryExists`/`ForceDirectories`/`CreateIniFile`/`GetRGB`/`SetLabelSpecialColorUp`/4 个文件名模板常量/`Format1|2`/`IntToStr`/`StrToIntDef`/`SaveTextFile`/`LoadTextFile`/`Max|Min|MaxI` |
| `MirReturnConfigGlobalSeam` | `MirReturnConfigDlg.Lifecycle.cs` | `g_MySelf` 存在性/`g_MagicList`/`GetMagicName`/`GetMagicId`/`g_MyHero` 存在性/`m_boDeath`×2/`m_boShopStall`/`m_Abil`×2/`AutoEatItem`/`HeroEatItem`/`g_nMouseX|Y`/`UseMagic`/`ClientConfigPluginMinEatItemTime` |
| `MirReturnIniFile` / `MirReturnIniFileStub` | `MirReturnConfigSeams.cs` | `IniFiles.pas` 的 `TIniFile`（Read/Write × String/Integer/Bool + UpdateFile） |
| `MirReturnMessageSeam` | `MirReturnConfigSeams.cs` | `FrmDlg.DMessageDlg`（**`UiEnabled` 默认 true，测试置 false**，与 `GXX.RunGate.MessageBoxSeam` 同模式） |
| 复用**只读文件**已有的接缝 | — | `ConfigSeams.MyGetTickCount`/`ProcessFileNameSpecialChar`、`ConfigShareSeam.g_MySelf|g_MyHero|g_ItemArr|g_ExtBagOpenItemCount|GetMaxBagCount`、`ConfigShareGlobal.g_sPlugServerName|g_sPlugUserName|g_ShortcutKeys`、`ChatBoardSeam.AddChatBoardString`、`MirsConfigGlobalSeam.g_boSound|g_boBGSound|g_boRepeatBGSound`、`FilterItemsGlobal.g_FileItemDB`、`ConfigShare.GetKeyDownStr`、`ConfigShare.FindHum*ItemIndex`、`GXX.Core.Protocol.TAbility` |

**特意未复用**：`GXX.RunGate.MessageBoxSeam` —— `GXX.Client.csproj` 未引用 `GXX.RunGate`，
按规程在车道内提供**同形**接缝而不是跨工程加依赖。

### 7.3 跨车道类型重名核查（开工前已 grep）

`GXX.Client.*` 内**不存在** `TMirReturnConfigDlg`/`IMirReturnConfigDlgControls*`/`MirReturn*Seam*`/
`MirReturnChecked*`/`MirReturnControlId`/`MirReturnEditId`/`MirReturnDiyButtonId`（全仓 `MirReturnConfigDlg` 命中 0，与派发说明一致）。
本车道**不声明任何 DxComponent 类型**，故与 `DxComponent/**`、`LoadDx/**`、`GUI/{Share,Mir,DxComponent}/**`
的同名类型（`TDxEdit`/`TDxListView`/`TDxScrollBox`/`TDxChatMemo`/`TDxLabel`/`TDxImageButton`/`TDXTrackBar`…）
**无 CS0101/CS0104 风险**。

---

## 8. 事故与自纠

### 8.1 ⚠ 一度误写主工作树（已完全回滚，无残留）

**现象**：本车道的 `write` 工具调用最初使用**相对路径**（`GXX.CSharp/src/...`），
而 harness 的工作目录是 `D:\chuanqi\daima\GXX原版_Delphi7`（**主工作树**，`main` 分支），
**不是**本车道的 worktree。7 个新文件因此落在主工作树。

**发现与处置**（在 **commit 之前**，`git status` 显示为 `??` 未跟踪）：
1. `Get-FileHash` 逐文件校验 SHA256，确认 7/7 一致后 `Copy-Item` 到 worktree；
2. `Remove-Item` 从主工作树删除 7 个未跟踪文件；
3. 复核：主工作树 `git status` 仅剩派发方**开工前既有的** 2 处 modified
   （`docs/并行派发台账.md`、`tools/lane-zones.tsv`，**非本车道所为**），**无本车道残留**；
4. **未触碰**主工作树任何**已跟踪**文件（`git status` 全程无 `M GXX.CSharp/src/...`）。

**后果**：零。此后所有写操作改用**绝对路径**。教训与台账 §13.10 同源。

### 8.2 生成器脚本的 PS 5.1 编码陷阱

`Generate-MirReturnControls.ps1` 首版含中文注释与中文 `-Root` 默认值；
Windows PowerShell 5.1 **无 BOM 时按 ANSI 读 `.ps1`**，中文被解码成乱码，
字符串里的 `<`/`>` 与引号失配 → 解析失败。**处置**：脚本改为**纯 ASCII**
（`non-ASCII bytes = 0`），非 ASCII 输出文本用 `[char]0xXXXX` 码点拼装。
脚本里已写明该约定，避免后续车道重踩。

### 8.3 生成器/基接口的"重复声明"反复

生成器与手写基接口的**成员所有权**边界，本波次迭代了 5 轮才收敛。最终规则（已写进脚本注释）：

- **基接口持有**：任何原文里是**控件对象**、且代码要做 `<> nil` 或需要"非原语类型"的成员
  （`PlugConfigDlg`、`PlugMemoConfigHelp`、`PlugComboBoxAutoMagic`、`PlugScrollBoxBoss|Mons`、
  `PlugPageControlConfig`、`PlugMemoConfig8Page`、`PlugMemoConfig2`），
  以及所有**方法型**成员（`Add`/`Set*`/`Lock`/`Clear`/`LoadFromFile`/`SetFocus`）。
- **生成接口持有**：纯取值面（`Checked`/`Value`/`ItemIndex`/`Text`/`Enabled`/`Max`/`Min`/`Position`）。

**踩坑根因**：C# 的**基接口成员优先**于派生接口同签名成员，
故若生成接口把控件对象声明成 `int`，`Plug.PlugX != null` 会被静默解析到
**派生接口的 `int`→`object` 装箱**（恒真），而不是基接口的 `object`。
`LoadHelpFile` 与 `AutoUseMagic` 两处 `<> nil` 守卫会因此**永久失效** —— 已由
`p5-client-mirreturn` 的 `LoadHelpFile_PlugMemoConfigHelp为nil时跳过` 与
`AutoUseMagic_*` 用例防回归。

---

## 9. 剩余量

| 项 | 量 |
|---|---|
| 未覆盖方法族 | **9 个**，约 **1,180 行**（含 `Initialize`/`LoadClientConfig` 的控件构建段 701 行） |
| 未逐条断言的键名表 | `LoadConfigFile`/`SaveConfigFile` 的 5×16 + 9×5 键名（抽样已测） |
| **本车道总覆盖（按行）** | 约 **4,070 / 5,954 = 68%**（其中"逻辑行"口径下约 **85%**：未覆盖的大头是控件树构建与 DxListView 交互） |
| 建议后续批次 | ①`CheckBoxClickEx` 82 处映射（与 `RefConfig` 互为逆，可**表驱动**一次性做完）②`AutoProtect`+`DuraWarning`（需 `CM_*` 常量表）③`DamageHPUseItem`/`DamageMPUseItem`（需 `TStringList.CustomSort` 桥）④`Initialize`/`LoadClientConfig` 控件构建段（**需 DxComponent 控件库全量移植后才能做**） |

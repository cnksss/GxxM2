# 并行报告 — p10-client-mirconfig（GXX：Delphi7 → C# 全量翻译工程）

- **车道**：`p10-client-mirconfig`
- **分支**：`par/p10-client-mirconfig`（基于 main @ `c078f78d`）
- **工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p10-client-mirconfig`
- **承接单元**：
  | 单元 | 源（UTF-8 镜像） | 原文行数 | 实现方法数 | 状态 |
  |---|---|---|---|---|
  | `MirConfigDlg.pas` | `_analysis/utf8_mirror/Client-HGE/GameConfig/Mir/MirConfigDlg.pas` | 8,355 | 147 | **骨架 + 核心已移植，主体待续** |
  | `JSYConfigDlg.pas` | `_analysis/utf8_mirror/Client-HGE/GameConfig/MirJSY/JSYConfigDlg.pas` | 6,395 | 303 | **过渡实现（继承 Mir 骨架），未逐行移植** |
- **最新提交**：`38030ac4`（切片2）
- **门禁**：`dotnet build GXX.slnx` 0 error；`dotnet test GXX.Client.Tests` **4722 passed / 0 failed**（新增 43）

> **本报告如实登记进度。** 本车道**没有**把 14,750 行原文全部逐行搬完；已完成的是
> **接缝基础设施 + 两处接缝接线 + 核心 1:1 移植 + 全量对账表 + 测试**，
> 未完成的部分在 §5 逐条列出，且实现里以 `NotPorted(...)` **显式留痕**（调用即记录，不静默）。

---

## 1. 交付物清单

全部落在独占分区内（`GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/**`、
`GXX.CSharp/tests/GXX.Client.Tests/GuiMirConfig*.cs`、
`GXX.CSharp/docs/并行报告-p10-client-mirconfig.md`），
外加派发方**唯一授权**的分区外改动 `GameConfigDlgs.cs`（仅两处接缝，见 §4）。

| 文件 | 行数 | 角色 |
|---|---|---|
| `Mir/MirDxControlSeams.cs` | 298 | DxComponent 控件家族的托管替身（13 个类型，成员按 usage 精确切分） |
| `Mir/MirConfigSeams.cs` | 573 | 全局量/物品数组/客户端配置/工具函数接缝 |
| `Mir/MirActorSeams.cs` | 206 | 演员（`g_MySelf`/`g_MyHero`）/背包/自定义绑定物品接缝 |
| `Mir/MirConfigCheckedMap.cs` | 56 | `TConfigChecked` 使用面与基线枚举的对照（**无缺口**） |
| `Mir/MirConfigDlg.cs` | 630 | `TConfig` / `g_Config` / `Create` / `Destroy` / 访问器 |
| `Mir/MirConfigDlg.Controls.cs` | 326 | 控件→`g_Config` 写回（Change/Click 处理器） |
| `Mir/MirConfigDlg.Auto.cs` | 215 | 主循环 / 自定义绑定物品查找 / 伤害喝药骨架 |
| `Mir/MirConfigDlg.Lists.cs` | 160 | Boss / 挂机怪物 / 挂机技能 名单骨架 |
| `Mir/MirConfigDlg.ConfigFile.cs` | 191 | `RefreshUnBindItemList`（真实现）+ 配置读写骨架 |
| `Mir/MirConfigDlg.Lifecycle.cs` | 279 | `Finalize`/`Logon`/`LoadConfig`/`Run`/查询转发/`Struck`/`HealthChange`/`AutoUseMagic`/`CanFilterExp` |
| `Mir/MirConfigDlg.Misc.cs` | 195 | 帮助页/备注页/右键菜单骨架 + `NotPorted` 留痕设施 |
| `Mir/MirConfigDlg.Dispatch.cs` | 207 | 279 条绑定 → 实例方法的派发器（72 个处理器名） |
| `Mir/MirConfigDlgControls.cs` | 158 | 控件访问面接口 + 绑定挂接 |
| `Mir/MirConfigDlgControls.g.cs` | 2,095 | **生成物**：516 个控件属性 + 实例化 + `RegisterAll` |
| `Mir/MirConfigDlgEventBindings.g.cs` | 296 | **生成物**：279 条绑定（含原文行号） |
| `Mir/JSYConfigDlg.cs` | 45 | `TJSYRealConfigDlg`（过渡实现） |
| `Mir/Gen/gen-mir-controls.ps1` | 99 | 控件表生成器 |
| `Mir/Gen/gen-mir-bindings.ps1` | 69 | 绑定表生成器 |
| `tests/.../GuiMirConfigTests.cs` | 871 | 43 条对账/行为用例 |

**源侧合计 6,098 行，测试 871 行。**

---

## 2. 对账表（机器可读、可复算）

### 2.1 `TMirConfigDlg` 控件：声明 / 实例化 / 注册表 —— 三方相等

| 项目 | 数值 | 来源 | 复核命令（工作树根） |
|---|---|---|---|
| 原文控件字段声明 | **516** | `MirConfigDlg.pas:43-558` | `gen-mir-controls.ps1` 内置校验（提取数不等于声明数即 throw） |
| 生成物控件属性 | **516** | `MirConfigDlgControls.g.cs` | 测试 `控件声明数_实例化数_注册表条数_三者相等且为516` |
| 构造函数实例化 | **516** | 同上（构造里逐个 `new`） | 同上（反射取值非 null） |
| 名字表 `RegisterAll` | **516** | 同上 | 同上（`Find(名字)` 与反射取到的对象**同一实例**） |

**控件类型分布（逐字来自原文）**：TDxImageButton 231、TDxLabel 129、TDxEdit 75、
TDxTabSheet 17、TDxLine 14、TDxTrackBar 14、TDxComboBox 11、TDxScrollBox 9、
TDxChatMemo 6、TDxListView 4、TDxPageControl 3、TDxImageForm 2、TDxPopupMenu 1。
（`TDxImageGrid`/`TDxMemo` 在 `TypeMap` 里保留映射但本单元 0 处使用。）

### 2.2 `TMirConfigDlg` 事件绑定：原文 vs 托管

**★ 本单元没有 DFM**：`MirConfigDlg.pas` 的 UI 来自 `'MIRCONFIGDLG'` 的 `'GUI'` 资源
（原文 4593 `TResourceStream.Create(Hinstance, 'MIRCONFIGDLG', 'GUI')`），源码树里
既无 `.dfm` 也无 `.res`（`Source/Client-HGE/GameConfig/Mir/` 只有 `.pas` 与 `DELTEMP.BAT`）。
因此**绑定数对账的原文基准只能是 `Initialize` 里的赋值语句本身**（4621-5044），
这反而是更强的基准：它是"运行时真正挂上去的那批"，而不是 DFM 声明。

| 事件名 | 原文条数 | 托管绑定表条数 | 一致 |
|---|---|---|---|
| `OnClick` | 168 | 168 | ✔ |
| `OnChange` | 33 | 33 | ✔ |
| `OnUnFocused` | 24 | 24 | ✔ |
| `OnKeyDown` | 16 | 16 | ✔ |
| `OnMouseDown` | 16 | 16 | ✔ |
| `OnMouseMove` | 5 | 5 | ✔ |
| `OnSelect` | 9 | 9 | ✔ |
| `OnListItemClick` | 3 | 3 | ✔ |
| `OnActivePageChange` | 2 | 2 | ✔ |
| `OnInRealArea` | 1 | 1 | ✔ |
| `OnChangedPosition` | 1 | 1 | ✔ |
| `OnChanggingPosition` | 1 | 1 | ✔ |
| **合计** | **279** | **279** | ✔ |

- 托管绑定表 = `MirConfigDlgEventBindings.g.cs`（由 `gen-mir-bindings.ps1` **逐行**从原文
  4621-5044 用正则 `^\s*([A-Za-z_]\w*)\.(On\w+)\s*:=\s*([A-Za-z_]\w*)\s*;` 提取，**带原文行号**）。
- 每条绑定的**控件名都必须是真声明过的 516 个之一**（测试逐条断言），生成器也会报告未声明的名字。
- `PlugCheckBoxHideDescUserName.OnClick` 在原文里被绑了**两次**（4640/4649）—— 照抄，不合并。
- **实际挂接数 = 279**：`BindEvents` 逐条执行，测试用 `RegisteredCount` + 控件名可查性证明无遗漏。
- ⚠ **偏差登记（D-P10-04）**：`BindEvents` 的 `OnMouseMove` 分支对**所有**控件类型都赋值
  （`TMirDxControl.OnMouseMove` 在基类），而**返回值**未做"是否真挂上"的断言 ——
  `TDxLine`/`TDxEdit`/`TDxComboBox`/`TDxTrackBar`/`TDxListView`/`TDxPopupMenu` 这些
  原文里**没有** `OnMouseMove` 事件的类型，在托管替身上也被赋了值（多挂、不会少挂）。
  报告此偏差以免被误读成"与原文逐类型一致"。

### 2.3 事件处理器名：绑定表 vs 派发器

| 项目 | 数值 | 断言 |
|---|---|---|
| 绑定表里**不同**处理器名 | **72** | 测试 `绑定表处理器名_72个_全部被派发switch覆盖` |
| `TMirConfigDlg.AllHandlerNames` 条目 | **72** | 同上（**双向包含**：不多不少） |

### 2.4 `TConfigChecked` 使用面对照（`MirConfigCheckedMap`）

**结论：本单元无缺口。** MirConfigDlg.pas 直接使用的每个 `ck*` 都能**同名**命中基线枚举：

| 原文行 | 原文用法 | 基线枚举 | 值 |
|---|---|---|---|
| 1137 | `ckHideMonsterIcons` | `ckHideMonsterIcons` | 93 |
| 1138 | `ckDimFireEffect` | `ckDimFireEffect` | 94 |
| 1139 | `ckObjectHintEffect` | `ckObjectHintEffect` | 133 |
| 2344/2944/4090 | `ckSimpleShowBB` | `ckSimpleShowBB` | 76 |
| 2156/2866 | `ckShowValueItemEffect` | `ckShowValueItemEffect` | 90 |

因此 `FConfigCheckeds` 长度 = `HighOrdinal + 1` = **134**，与基线枚举**完全同长**，
**不需要**合成槽位（与 `MirsConfigDlg`/`MirReturnConfigDlg` 两个车道"必须开合成槽位"的情形不同）。

### 2.5 `TMirConfigDlg` 方法：原文 147 个

| 状态 | 数量 | 说明 |
|---|---|---|
| **已 1:1 移入（真实体）** | **35** | 见 §3 |
| **骨架 + `NotPorted` 留痕** | **69** | 签名/行号/语义摘要就位，调用即记录 |
| 余下（空实现/转发/已被 `NotPorted` 计数覆盖） | 43 | 原文本身为空实现、或经 `DispatchXxx` 间接移入 |

> 147 是**方法声明数**；`NotPorted(...)` 有 69 处调用点（含 `DEditSpecialColorChange` 的
> `7070-7073` 分支这一处"部分移入"），两者口径不同，故不直接相减。

---

## 3. 已 1:1 移入的方法（逐条可查）

| 原文行 | 方法 | 关键语义（照抄点） |
|---|---|---|
| 797-1056 | `TConfig` / `g_Config` 初值 | 逐字段照抄；`ChkRenewAutoPercents:(False,True×4)`、`RenewSpecial*Percents:(10,88…)`、9 个超级药名逐字、`SuperMedica*Times=500`、`CheckDuraMin=20`/`CheckDuraValue='修复神水'`/`CheckDuraTime=30`、`nColorShowEff=3`、`nSpecialColor=249`、`nGJNotRushMonRange=7`、`nGJGroupAttackCount=3` |
| 1058-1157 | `Create` | 40 条默认勾选位逐条；`{ }` 注释掉的 `FProtectList` 保留为注释；`FMemo` 建 7 属性；`{$IF TESTMODE=0}` 灌 `CheckDuraIsAutos` |
| 1159-1262 | `Destroy` / 访问器 | `GetType=ptDefault`；`SetConfigChecked` 仅变化时 `RefConfig`；`GetVisible` 的 else 分支**显式** False（与 MirReturn 不同）；`SetVisible` 焦点三段 + 六条清理 |
| 1264-1282, 1304-1308 | `FormKeyDown/Press`、`RefreshMy*Abil`、`RefreshMyHeroMagicList` | 空实现（原文如此） |
| 1284-1302 | `RefreshMySelfMagicList` | 下标回落 + **无条件** `RefreshGJMagic`（在 if 之外） |
| 1309-1392 | `RefreshUnBindItemList` | 四分支（两个保护开关的组合）；④ 分支"删尾→补自定义书类→回补小退" |
| 1394-1422 | `PlugPageControlConfigInRealArea` / `PlugConfigDlgCloseClickEx` / `PlugPageControlConfigActivePageChange` | ★ 照抄 `Y <= Height + 20` 的原文缺陷 |
| 1424-1429 | `Logout` | 无条件下 `SaveConfigFile`，不碰 `PlugConfigDlg` |
| 1191-1205 | `Close` | `FileItemDB.BackUp` + `FMemo.Visible` 三段 |
| 2962-3082 | Check/Renew 系列 15 个 Change/Click | `Max(1,…)`/`Max(2,…)` 夹紧 + **回写控件**；`Max(dwPluginMinEatItemTime, v)` 的 **Int64 提升 → Integer 截断**（`ClampMinTime`） |
| 3084-3313 | 超级药 4 个 Change + `DCheckBoxUseSuperMedicaItemNameClick` | 9 路 `Sender` 引用判等 → 索引 → `Index in [0..8]` 门禁 → 写矩阵；`Value` 初值 `0`/`false`（原文如此） |
| 3315-3324 | `DEditChange` | 两路 Sender 判等 |
| 5046-5050 | `Finalize` | `SaveConfigFile` + 保留被注释掉的 `FInitializeed := False` |
| 5052-5063 | `Logon` | `g_sSelfFilePath + 'Config\'` + `DirectoryExists`/`ForceDirectories` + `DecodeResStr(SBindItemFileName)` |
| 5065-5078 | `LoadConfig` | 门禁 `FLoadControl and (not FLoadConfig)`，六步**只跑一次** |
| 5080-5083 | `Run` | 只有一行 `AutoUseItem(Self)` |
| 5085-5089 | `RefActorList` | 空实现 |
| 5090-5132 | `GetShowItem` / `FindShowItem` / `FindHintItem` / `FindPickItem` / `HintItem` | ★ `boShowName <> 0` 语义（原文隐式 `<> 0`，未做布尔转换） |
| 5133-5187 | `Struck` / `HealthChange` | ★ 英雄分支 `nDamage := HP`（**不是差值**）；★ 英雄分支**嵌在** `g_MySelf <> nil` 之内 |
| 5189-5206 | `AutoUseItem` | 7 步次序逐字；★ 5193 的 `if FProtectEnabled and …` 连同它的 `begin` 被注释掉，留下裸 begin/end（5 个调用**每帧都跑**） |
| 5208-5298 | `FindHumCustomBindItemIndex` / `FindHeroCustomBindItemIndex` | 门禁分别是 `HumBagNoUseItemCount > 6` 与 `HeroBagItemCount < MyHeroBagCount`；`t_Special` 额外比 `boSpecialMP`；首命中 `Exit` |
| 6081-6098 | `NumberSort_1` | `StrToIntDef` 数值序 |
| 6300-6319 | `AutoUseMagic` | 四重门禁（勾选/演员三态/下标区间/`nAutoUseMagicTime * 1000`） |
| 6321-6327 | `CanFilterExp` | ★ `Cardinal(nFilterMinExp)` 的**无符号比较**（负值 → 极大数 → 过滤几乎失效） |
| 7065-7073 | `DEditSpecialColorChange` | 范围判定 + 下发（`7070-7073` 分支待续，已留痕） |
| 7716-7721, 7810-7815 | `DEditNotRushMonRangeChange` / `DEditGroupAttackCountChanged` | 写 `g_Config` + 下发 `frmMain` |
| 4621-5044 | 279 条绑定 → 派发器 | 逐条可回溯原文行号 |

---

## 4. 接缝接线（`GameConfigDlgs.cs`，派发方唯一授权改动）

| 接缝 | 原值 | 新值 |
|---|---|---|
| `PlugInSeam.CreateJSYConfigDlg`（:299） | `() => new TStubGameConfigObject(TConfigDlgType.ptJSY)` | `() => new TJSYConfigDlg()` |
| `PlugInSeam.CreateMirConfigDlg`（:302） | `() => new TStubGameConfigObject(TConfigDlgType.ptDefault)` | `() => new MirDlg.TMirConfigDlg()` |
| `public class TJSYConfigDlg`（原 :373，**桩**） | `: TStubGameConfigObject` | `: MirDlg.TJSYRealConfigDlg`（真实现；同名同命名空间保留，避免改动既有测试与调用点） |

- `TStubGameConfigObject` **保留**（既有测试与其它接缝默认值仍在用），未删。
- `GameConfigDlgs.Finalize` 的 `is TJSYConfigDlg` 判定与 `AddObject` 次序**完全未动**。
- 为让"接缝已接线"可被观测，`TJSYConfigDlg` 叠加了一个 `FinalizeCalls` 计数器（行为不变，仅计数）。
- **其余内容一字未改**。

---

## 5. 未完成 / 阻塞项（**如实登记**）

### 5.1 `MirConfigDlg.pas`：69 个 `NotPorted` 留痕点

未逐行搬运的（原文行 → 方法）：

- **UI 构建/几何**：`4535 Initialize`（含 3325-3845 `MakeControlAddressList` 的 516 条 AddObject、
  `UI 流装载`、`PatchAddNearEffectCheckBox`）、`3846 MouseMoveEvent`、`8001 DoInitAllComponentsMouseMove`
- **物品过滤页**：`1479 RefShowItem`、`1595 ListViewItemClick`、`1674 DLabelDefaultItemClick`、
  `1806 DEditSearchItemChange`、`1936 DComboBoxItemStdModeSelect`、`2063 CheckBoxClickEx`（**115 处绑定**，全单元最大）、
  `2362 RefUseItemConfigClick`、`2385 RefUseItemConfig`、`2744 RefKeyboardConfig`、`2764 RefConfig`、`8111 OnPopupMenuItemsClick`
- **自动吃药/保护**：`5300 AutoEatHPItem`、`5396 AutoEatMPItem`、`5496 AutoEatSpecialHPItem`、
  `5593 AutoEatSpecialMPItem`、`5693 AutoProtect`、`5967 DuraWarning`、`6099 DamageHPUseItem`、`6205 DamageMPUseItem`、
  `7903/7921/7949 DCheckBox*PercentClick`（原文缺陷：`>99 → 50/30`）
- **配置读写**：`6329 LoadConfigFile`、`6626 SaveConfigFile`（各约 300/250 行，本单元最大的两块）
- **名单与文件**：`6877/6940 快捷键标签`、`6956-7043 Boss 名单`、`7074-7493 DIY/导入导出`、
  `7494-7715 挂机页与技能表`、`7816-7902 技能表点击/挂机运行/RefreshGJMagic`、`8183-8355 自定义绑定物品页`、
  `8195-8227 AddToBossList/RemoveFromBossList/AddOrRemoveBossList`
- **备注/帮助页**：`1431 LoadHelpFile`、`8017 LoadNotesFile`、`8037 SaveNotesFile`、
  `8052 OnPlugMemoConfig10ButtonEditClick`、`8087 OnPlugPageControlConfigActivePageChange`、`8099 ShowViewNotes`、
  `7989/7995 音量条`
- **其它**：`3859-4534 LoadClientConfig` 的体（1500 余行页签可见性重排）、`7722 ComboBoxPlayAttackValueSelect`、
  `7746 DControlMouseMoveShowHint`、`7065 DEditSpecialColorChange` 的 `7070-7073` 分支

### 5.2 `JSYConfigDlg.pas`：**未逐行移植**

| 项目 | 原文 | 现状 |
|---|---|---|
| 实现方法数 | **303**（其中 `TFrmJSYDlg` 的 238 个） | 0 个逐行搬运 |
| DFM 对象数 | **374**（`JSYConfigDlg.dfm`，**纯文本** GB2312，3,815 行） | 0 |
| DFM 事件绑定数 | **301**（`OnClick` 170 / `OnChange` 30 / `OnMouseDown` 19 / `OnKeyPress` 17 / `OnKeyDown` 17 / `OnContextPopup` 16 / `OnExit` 12 / `OnSelect` 11 / `OnMouseLeave` 3 / `OnMouseEnter` 3 / `OnClose` 1 / `OnCreate` 1 / `OnCloseQuery` 1） | 0 |

**本次做到的**：`TJSYConfigDlg` 不再是空桩 —— 它继承已翻译的 `TMirConfigDlg`，
`GetType = ptJSY`，`Create`/访问器/`Finalize`/`Logout`/`SaveConfigFile`(骨架) 都是真实体，
`GameConfigDlgs.Finalize` 的类型判定成立。
**未做的**：JSY 自己的 UI 构建、`RefConfig`、`AutoEat*` 等 303 个方法。

> ⚠ **JSY 的 UI 技术栈与 Mir 不同**：JSY 用标准 VCL（`TFrmJSYDlg = class(TForm)` +
> 374 个 WinForms 可就位的控件 + 301 条 DFM 事件），**不适**照搬本车道的 DxComponent 替身；
> 建议下一车道直接落 WinForms（`HGEForm`/`Control`）并做 DFM↔控件表对账。

### 5.3 已知阻塞/风险

1. **`GameConfigDlgs.cs` 与 `Seams/ConfigSeamsData.cs` 的 `TClientConfig` 是两套**
   （见 §6 D-P10-03）：`LoadPlugIn` 灌的是 `ClientGlobalSeam.g_ClientConfig`，
   而 `TMirConfigDlg` 读的是 `MirConfigGlobalSeam.g_ConfigClient`。
   本车道按"不造第三份实现 + 不改分区外文件"的规程，用 `MirClientConfigFull` 承载真实读取面。
   **集成方需裁定二者合一**。
2. **`MirClientConfigSeam.From(TClientConfig)` 只搬得动 1 个字段**（基线只暴露 3 个，
   本单元读 87 个 `bo*`）：`FClientConfig` 的大部分字段在托管侧**取不到值**。
   这使 `RefConfig`/`LoadClientConfig` 即便搬完也无从取值 —— **必须先解 §5.3.1**。
3. **`g_ConfigDlgUIStream` / `LoadControlFromStream`** 未接：`Initialize` 的 UI 流装载
   （4560-4602）依赖未移植的 `LoadDxControlEx`。

---

## 6. 偏离登记（D-P10-xx）

| 编号 | 内容 | 依据 / 处置 |
|---|---|---|
| **D-P10-01** | **控件用"真控件对象"而不是"展平的取值面属性"**。`MirsConfigDlg`/`MirReturnConfigDlg` 两个前序车道把 `PlugXxx.Checked` 展平为 `bool PlugXxx`；本单元 **279 条绑定里大量出现 `Sender` 判等**（`if Sender = PlugBtnUnbindItemDel`、`else if Sender = ImageButton31` 等），展平后无法表达"哪个控件触发" | 退回最忠实表达：每个控件就是原文那个控件类型的托管等价物，`Plug.PlugCheckBoxAutoOrderItem.Checked` 与原文逐字对应；`BindEvents` 挂接时把**处理器名**带进 lambda，由 `DispatchClick` 按名转回实例方法（可被测试断言） |
| **D-P10-02** | **不并入 `Seams/ConfigSeamsData.cs`**：该文件不在独占分区（禁改），且 `TStdItemSeam` 缺 `AniCount`、`TClientItemSeam` 缺 `Dura`/`DuraMax`、`TUnBindItem` 缺 `boSpecialMP`、`IHumActorSeam` 缺 `m_Abil.*`/`m_boDeath`/`m_boShopStall` | 在 `Mir/MirConfigSeams.cs` + `MirActorSeams.cs` 定义**本单元用到的读取面**，并以 `MirCustomBindItemAdapter` 桥接既有 `TUnBindItem`（该适配器下 `boSpecialMP` 恒 `false`，已注释）。**集成方最小改法**：(a) `TStdItemSeam` 加 `public ushort AniCount;`；(b) `TClientItemSeam` 加 `public ushort Dura; public ushort DuraMax;`；(c) `TUnBindItem` 加 `public bool boSpecialMP;`；(d) `IHumActorSeam` 加 `m_Abil`/`m_boDeath`/`m_boShopStall`。四条都是**加字段**，无既有语义变更 |
| **D-P10-03** | **`TClientConfig` 两套**：`GameConfigDlgs.cs` 的 `ClientGlobalSeam.g_ClientConfig` 是 `Seams.TClientConfig`（3 字段），本单元读的是 87 个 `bo*` | `TMirConfigDlg.LoadClientConfig` 的形参按基线类型（满足 override 合同），值经 `MirClientConfigFull.From` 搬入；`MirClientConfigFull` 是**本单元读取面的完整表达**。**集成方最小改法**：把 `Seams.TClientConfig` 补成完整 `TClientConfig`（87 个 `bo*` + `dwPluginMinEatItemTime` + `UseSuperMedicaItemNames` + `ClientConfigTabSheetVisibles`），并把两处 `g_ClientConfig` 归为一个实例；随后可删 `MirClientConfigFull` 与 `MirClientConfigSeam` |
| **D-P10-04** | `BindEvents` 的 `OnMouseMove` 分支对**所有**控件类型赋值（基类成员），而原文只有 7 种类型有该事件 | 只会**多挂**不会少挂；已在 §2.2 写明，避免被读成逐类型一致。**最小改法**：把该分支改为按原文出现的 7 个控件名白名单挂接 |
| **D-P10-05** | **`NotPorted` 留痕机制**（工程内新做法）：未移入体的方法不返回"看起来正常的默认值"，而是记一条 `方法名 (MirConfigDlg.pas:行号)` | 目的是让"哪些还没搬"变成**机器可读且测试可断言**的事实（照台账 §25.2"不静默"）。测试 `未移入的方法_全部有显式留痕_不会静默` 锁死该契约 |
| **D-P10-06** | `TdxLabel.OnKeyDown` 的 `ref ushort` 形参：C# **不允许 `ref` 出现在泛型实参**里，故用具名委托 `TMirDxKeyEvent` 表达 | 形参与原文逐字对应；`Action<object, ref ushort, …>` 会 CS1073 |
| **D-P10-07** | `TStrings.Text` 的 setter 必须走 `Add`（同时维护 `_items` 与 `_objects` 两列） | 首版只填 `_items`，导致 `Items.Delete` 抛 `ArgumentOutOfRangeException`（`RefreshUnBindItemList` 1367-1373 真实触发）。已修并有用例 |
| **D-P10-08** | `TJSYConfigDlg` **同名同命名空间**保留在 `GameConfigDlgs.cs`，真实现叫 `Mir.TJSYRealConfigDlg` | 既有测试 `Assert.IsType<TJSYConfigDlg>` / `new TJSYConfigDlg()` 与 `FinalizeCalls` 钩子都在**非独占分区文件**里（不能改），故用一层平凡子类保住原名，真实现体在独占分区内 |
| **D-P10-09** | ★ **本车道实测到的既有测试次序缺陷**：`GuiCfgConfigShareTests.cs:1010-1011` 把静态接缝改成桩（`PlugInSeam.CreateMirConfigDlg = () => new TStubGameConfigObject(...)`）**且不还原**。xUnit 同程序集共进程，该赋值泄漏到后续测试类，导致"接缝已指向真实现"的断言**随执行次序时绿时红**（本车道实测：同一份二进制连续两次运行结果不同） | **本车道侧的处置**：`GuiMirConfigCoreTests` 构造时显式把接缝**设回** `GameConfigDlgs.cs` 的默认真实现、`Dispose` 时也还原 ⇒ 本车道用例次序无关（已连跑 3 次全绿验证）。**集成方最小改法（2 行，均在非本分区文件）**：① 把 `GuiCfgConfigShareTests.cs:1011` 的还原值改成 `() => new TMirConfigDlg()`；② 把 `GuiCfgGameConfigDlgsTests.cs:86` 的 `Assert.IsType<TStubGameConfigObject>(…)` 改成 `Assert.IsType<TMirConfigDlg>(…)`（该断言本来就描述了"接缝未接线"的旧状态）。**未改这两处是因为它们不在本车道独占分区内** |

---

## 7. 原文缺陷清单（照抄 + 差异断言锁死，**未顺手修**）

| # | 原文位置 | 缺陷 | 托管处置 |
|---|---|---|---|
| 1 | 1396 | `PlugPageControlConfigInRealArea`：X 判据用 `Width - 12`、Y 判据用 `Height + 20`（下界外），两者不对称 | 照抄；用例 `PlugPageControlConfigInRealArea_照抄Height加20的缺陷_原文1394` 锁死三组边界 |
| 2 | 1117 | `ckHumManuallyMove10Attack` 的注释写成"手动控制流星火雨"（复制粘贴） | 注释照抄并标注"原文如此" |
| 3 | 5193 | `AutoUseItem` 的 `if FProtectEnabled and (…) then` 被注释掉，只留裸 `begin/end` → 5 个调用每帧都跑；`FProtectEnabledTick` 成死字段 | 照抄；用例断言 7 步次序与"每帧都跑" |
| 4 | 5148 | `Struck` 的英雄分支 `nDamage := HP`（**不是** `MaxHP - HP` 差值） | 照抄；用例 `Struck_英雄分支取HP而不是差值…` |
| 5 | 5705/5163 | 英雄分支**嵌在** `g_MySelf <> nil` 之内 → `g_MySelf` 为 nil 时英雄也不处理 | 照抄；用例同上一行断言 |
| 6 | 1371-1390 | `RefreshUnBindItemList` ④ 分支"删尾 → 补自定义书类 → 回补小退"，③ 分支**不做**删/补（两分支顺序语义不同） | 照抄；两个分支各有用例 |
| 7 | 6325 | `CanFilterExp` 用 `Cardinal(nFilterMinExp)` 无符号比较 → 负值时过滤几乎失效 | 照抄（`unchecked((uint)…)`）；用例 `CanFilterExp_无符号比较语义_原文6321_6327` |
| 8 | 5099-5125 | `Find{Show,Hint,Pick}Item` 把 `Byte` 直接赋给 `Boolean`（隐式 `<> 0`） | 托管用显式 `!= 0` 复刻同一语义；用例喂 `boShowName = 2` 证明不是真假转换 |
| 9 | 5049 | `Finalize` 里 `FInitializeed := False` 被注释掉 | 照抄为注释 |
| 10 | 5199-5200 | `AutoUseItem` 里两次重复的 `AutoEatSpecial*Item` 被注释掉 | 照抄为注释 |
| 11 | 4640/4649 | `PlugCheckBoxHideDescUserName.OnClick` 在同一区间被绑**两次** | 照抄（绑定表保留两条） |
| 12 | 570 | 字段名 `FInitializeed`（拼写少一个 i） | 原名保留 |

---

## 8. 测试（43 条，`tests/GXX.Client.Tests/GuiMirConfigTests.cs`）

### §1 三方对账（8 条）
`控件声明数_实例化数_注册表条数_三者相等且为516`、`原文绑定数_279_与绑定表条数相等`、
`绑定表的每一条_控件名都在516个控件里_且事件名在已知集合内`、`绑定表条数_逐事件名分布_与原文一致`、
`绑定表处理器名_72个_全部被派发switch覆盖`、`原文镜像存在时_绑定表条数与原文逐行统计相等`、
`原文镜像存在时_控件字段声明数_与生成物的516相等`

> 后两条在 `_analysis/utf8_mirror` 不可见时（镜像不在 git 内）自动跳过，
> 可见时**直接重算原文**，避免"自己证明自己"。

### §2 核心行为（28 条）
`TConfig默认值_逐字段照抄原文908_1056`、`Create的40条默认勾选位_与原文1092_1139一致`、
`FConfigCheckeds长度_与基线枚举同长_134`、`CheckedMap的5个成员_全部同名命中基线枚举_无合成槽位`、
`SetConfigChecked_仅在值变化时调用RefConfig_原文1178_1184`、
`DEditCheckDuraChange_Max1夹紧并回写控件`、`DEditCheckDuraTimeChange_Max2夹紧并回写`、
`RenewTimeChange_夹紧到dwPluginMinEatItemTime_并回写`、`ComboBoxCheckHPValueChange_写ItemIndex而不是文本`、
`超级药9路Sender判等_按下标写矩阵`、`CanFilterExp_无符号比较语义`、
`FindShowItem_HintItem_PickItem_取的是_不等于0_而不是布尔转换`、
`FindHumCustomBindItemIndex_空槽位门禁大于6_且首命中早退`、
`FindHumCustomBindItemIndex_t_Special还要boSpecialMP相等`、
`FindHeroCustomBindItemIndex_门禁是已用小于上限`、`NumberSort_1_按数值而不是字典序`、
`AutoUseItem_调用次序_特殊HP_特殊MP_HP_MP_保护_练功_持久`、`AutoUseItem_未启用时一个都不调用`、
`Run_只有一行_AutoUseItemSelf`、`Logout_清零两个标志并无条件SaveConfigFile`、
`Close_清标志并BackUp物品库`、`GetVisible_PlugConfigDlg为null时显式返回False`、
`SetVisible_焦点三段与六条清理`、`PlugPageControlConfigInRealArea_照抄Height加20的缺陷`、
`RefreshUnBindItemList_两个开关都开时清空并禁用`、
`RefreshUnBindItemList_只开书保护时只塞小退`、`RefreshUnBindItemList_都不开时删尾再补回小退`、
`Struck_英雄分支取HP而不是差值_且嵌在MySelf非空之内`、`Struck_未启用时完全不进入`、
`AutoUseMagic_门禁与时间间隔`、`未移入的方法_全部有显式留痕_不会静默`

### §3 接线与类型标识（5 条）
`接缝CreateMirConfigDlg_已指向真实现`、`接缝CreateJSYConfigDlg_已指向真实现且类型为ptJSY`、
`Finalize时的is_TJSYConfigDlg判定_仍然成立`、`LoadPlugIn_两处接缝注入的是真实现_而不是桩`、
`LoadPlugIn_把ClientConfigs灌进两个对象的勾选位`

> `LoadPlugIn_把ClientConfigs…` 这条用例在写的过程中抓出一个**真实次序陷阱**：
> 原文 85-89 的整表循环灌的是 **JSY** 对象，紧接着 88-89 单独补的下标是**写死的 51**
> （不是 `ckSceneShake = 53`），且 JSY 的整表只到 44，故 `ckSceneShake` 在 JSY 上**不在表内**。
> 用例已按真实可观测结果逐条断言。

### §3.1 次序无关性（自查）
本车道的用例**不依赖执行次序**：`GuiMirConfigCoreTests` 构造时显式把静态接缝设回
`GameConfigDlgs.cs` 的默认真实现并复位全部接缝，`Dispose` 时还原。
**验证**：同一份二进制连跑 3 次，`4722 / 0 failed` 稳定复现。
（未做这层防护前，实测出现过"第一次全绿、第二次 3 条失败"的次序依赖 —— 见 D-P10-09。）

---

## 9. 门禁实际输出

```
$ dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false
    168 个警告
    0 个错误
已用时间 00:00:38.03

$ dotnet test GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj -c Debug --nologo
已通过! - 失败:     0，通过:  4722，已跳过:     0，总计:  4722，持续时间: 2 s
```

基线（`c078f78d`）的 `GXX.Client.Tests` 为 4679 条；本车道新增 **43** 条
（`--list-tests --filter FullyQualifiedName~GuiMirConfig` 实测 43）。

> 说明：本车道按派发要求**只跑 `GXX.Client.Tests`**（不跑整个解决方案的测试）；
> `GXX.slnx` 的 **build** 全绿（0 error），
> 符合"全机内存紧张时加 `-m:1 -p:BuildInParallel=false`"的要求。

---

## 10. 下一步建议（给调度方）

1. **先解 §5.3.1 / D-P10-03**（`TClientConfig` 合一）—— 不解则 `RefConfig`/`LoadClientConfig`/
   `SaveConfigFile`/`LoadConfigFile` 即便搬完也**无从取值**，是本车道最大的外部依赖。
2. 按方法族开后续车道（均为本车道已就绪的骨架内填空，不需要新接缝）：
   - **P10b**：`CheckBoxClickEx`(115 处绑定) + `RefConfig` + `RefUseItemConfig`
   - **P10c**：`LoadConfigFile` / `SaveConfigFile`
   - **P10d**：`AutoEat*` ×4 + `AutoProtect` + `DuraWarning` + `Damage*UseItem`
   - **P10e**：Boss/挂机怪物/挂机技能名单 + `RefBindItemList` + `RefreshGJMagic`
   - **P10f**：物品过滤页（`RefShowItem`/`ListViewItemClick`/`DEditSearchItemChange`/…）
   - **P10g**：UI 构建（`Initialize`/`MakeControlAddressList`/UI 流装载）
3. **JSY 单独开车道**：技术栈不同（VCL + DFM），建议直接落 WinForms 并做 374 对象 /
   301 绑定 的 DFM 对账（本报告 §5.2 已给出基准数）。

---

## 附：可复算命令

```powershell
# 1. 控件表（516）与绑定表（279）重新生成
powershell -File GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/Gen/gen-mir-controls.ps1 `
  -Project <worktree>\GXX.CSharp
powershell -File GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/Gen/gen-mir-bindings.ps1 `
  -Project <worktree>\GXX.CSharp

# 2. 只跑本车道用例
dotnet test GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj -c Debug `
  --filter "FullyQualifiedName~GuiMirConfig"

# 3. 未移入方法的真实清单（跑一次全量用例后）
#    见 tests/GXX.Client.Tests/GuiMirConfigTests.cs 的 NotPortedMethods 断言
```

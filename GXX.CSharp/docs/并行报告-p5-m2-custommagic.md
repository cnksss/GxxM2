# 并行报告 · 车道 `p5-m2-custommagic`
（GXX Delphi7 → C# ｜ 单元 `Source\M2Engine\Forms\uFrmCustomMagic.pas`，4,800 行，GBK ｜ 分支 `par/p5-m2-custommagic`）

> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p5-m2-custommagic`
> 独占区：`GXX.CSharp/src/GXX.M2Server/Forms/CustomMagic/**`、`GXX.CSharp/tests/GXX.M2Server.Tests/CustomMagic*`、本报告。
> **未修改任何区外文件**（`git diff --name-only main...HEAD` 的 18 个路径全部落在上述两处；根目录无 `.cs`）。

---

## 0. 交付摘要（TL;DR）

| 项 | 值 |
|---|---|
| 交付切片 | 3 次提交（骨架 / 测试 / 方法名门禁） |
| 新增源文件 | 9 个，**6,765 行** |
| 新增测试文件 | 9 个（本车道），**3,866 行** |
| 本车道新增测试 | **903 例**（全量 `GXX.M2Server.Tests` 5,619 → **6,522**） |
| 门禁 | `dotnet build GXX.slnx -c Debug` → **0 error**；`dotnet test` → **6,522 passed / 0 failed** |
| 方法名 1:1 覆盖率 | 原文实现段 **232/232** 方法全部有同名托管成员（脚本抽取 + 逐条反射断言） |
| 组件字段 1:1 | 原文 `:17-616` 的 **600** 个 published 字段，字段名/顺序/类型一一对应 |
| `VirtualTrees.pas` 处置 | 取值/校验/写回**全部抽成纯逻辑**（`PrepareEditSpec`/`ApplyEditorResult`/树列文本/编辑许可/勾选/点击决策）；**编辑器创建·销毁·消息泵 + 真实绘制留接缝** |

---

## 1. 全部 commit hash

| # | hash | 内容 |
|---|---|---|
| 1 | `a991257a` | 切片1：骨架（数据模型 / 接缝层 / 600 组件字段 / 151 表驱动处理器 / EditLink 纯逻辑 / 树纯逻辑 / 窗体主体） |
| 2 | `909c03df` | 切片2：测试 735 例全绿（EditLink 纯逻辑+接缝壳 / 五棵树 / 151 处理器 × 4 分支 / 窗体主体 / 组包） |
| 3 | `fd3246df` | 切片3：方法名 1:1 覆盖率门禁（232 条）+ 组件字段 600 计数；全量 6,522 例 0 失败 |

基线确认（开工前，同工作树）：`dotnet build GXX.slnx -c Debug` → 0 error；`GXX.M2Server.Tests` → **5,619 passed / 0 failed**。
**未出现基线漂移**：本车道的全部提交期间主干未引入编译错误；无"我没碰过的文件报错"。

---

## 2. 第一步侦察结论

### 2.1 单元结构（原文物理行号；该文件 4,800 行、**全 CRLF、无裸 LF**，故行号稳定）

| 区间 | 内容 |
|---|---|
| `1-14` | `unit` 头 + 4 个 `WM_STARTEDITING_*` 常量（`WM_USER+300..303`） |
| `5-7` | 第一段 `uses`（Windows/Forms/VirtualTrees/StdCtrls/Spin/**SpinEditEx**/Grobal2/**uCustomMagicUtils**/uFrmCustomMagicCopySetting/M2Threads/CheckUnit/…） |
| `16-616` | `TFrmCustomMagic = class(TForm)` 的 **600 个 published 组件字段** |
| `617-830` | 204 个事件处理器声明 |
| `831-841` | private：4 个状态字段 + 4 个 `WM_` 消息处理器 |
| `842-847` | public：`DoOpen` / `SetConfigChanged(IsChanged = True)` |
| `849` | `function ShowCustomMagic: Boolean;`（**接口段声明**） |
| `851-853` | `implementation` + `{$R *.dfm}` |
| `855-856` | 第二段 `uses`：**M2Share, UsrEngn, EDCode** |
| `858-885` | 本单元自有的 4 个 node-data 记录（`TMagicConfigNodeData` / `TAttackDecAttribData` / `TProtectAddAttribData` / `TMagicElementData`） |
| `887-928` | `TDecAttribPropertyEditLink`（`888`）、`TElementPropertyEditLink`（`909`）两个 `IVTEditLink` 实现类声明 |
| `932-1604` | 两个 EditLink 的实现（各 10 个方法，逐字同构） |
| `1605-1623` | 单元级 `SetControlEnabled`（递归进 TabSheet/Panel/GroupBox） |
| `1625-1636` | 单元级 `ShowCustomMagic` |
| `1638-1666` | `DoOpen` |
| `1668-1861` | `FormCreate`（150 行：名称表填充 + `SetControlEnabled(pgcMain, False)`） |
| `1863-1875` | `SetConfigChanged` |
| `1877-1908` | `vstCustomMagic` 三事件（DrawText/GetNodeDataSize/GetText） |
| `1910-2376` | **`vstCustomMagicNodeClick`（467 行，全窗体最长的装载逻辑）** |
| `2378-3531` | 特殊处理器 + 151 个"单字段写回"处理器混排 |
| `3532-3600` | `RebuildCustomMagicListText`（组包 + zLib + CRC） |
| `3601-3665` | `btnSaveClick` / `btnMakeConfigDataClick` |
| `3666-4799` | 其余处理器 + 三棵树的事件处理器 + 5 个 `WM_` 消息处理器 |

### 2.2 「纯逻辑/表驱动可独立验证」 vs 「需接缝」

**可独立验证（已 100% 抽出并单测）**

| 家族 | 原文行号 | 托管实现 |
|---|---|---|
| 151 个"单字段写回"处理器（守卫/目标路径/初值/`SetConfigChanged`） | 见 `CustomMagicHandlerTable.g.cs` 逐条行号 | `CustomMagicForm.Handlers.cs`（**脚本生成，非手工转录**） |
| EditLink 列→编辑器类型/范围/初值/下拉项 | `1156-1263`、`1486-1587` | `DecAttribPropertyEditLinkLogic.PrepareEditSpec` / `ElementPropertyEditLinkLogic.PrepareEditSpec` |
| EditLink 编辑器取值→字段写回/`IsChanged` | `1007-1145`、`1361-1475` | `…Logic.ApplyEditorResult` |
| EditLink 键盘决策（ESC/RETURN/UP/DOWN） | `942-986`、`1296-1340` | `TVtEditLinkKeys.EditKeyDown/EditKeyUp` |
| 三棵属性/元素树的列文本 | `4347-4399`、`4608-4690`、`4489-4520` | `DecAttribTreeLogic.GetAttackDecAttribText` / `GetProtectedAddAttribText` / `ElementTreeLogic.GetText` |
| 编辑许可矩阵（含三段互斥禁编辑规则） | `4400-4413`、`4691-4708`、`4521-4526` | `…IsAttackDecAttribEditingAllowed` / `IsProtectedAddAttribEditingAllowed` / `IsEditingAllowed` |
| 勾选回写 / 节点点击决策 / 提示图标布局 | `4290-4346`、`4429-4488`、`4551-4607` | `ApplyChecked` / `ResolveNodeClick` / `TryGetHintIconLayout` |
| 主列表红字判定（`IsChanged` 优先于高亮） | `1877-1893` | `CustomMagicMainTreeLogic.ResolveFontColor` |
| `SetControlEnabled` 递归语义 | `1605-1623` | `TFrmCustomMagic.SetControlEnabled` |
| 组包（packed 记录 + CRC + 压缩） | `3532-3600` | `RebuildCustomMagicListText` + `SizeOfClientConfig` |

**需接缝（已留，未覆盖）**

| 家族 | 原文行号 | 接缝 |
|---|---|---|
| `IVTEditLink` 的实现面（编辑器 new/free、`Show/SetFocus/Hide/Visible`、`WindowProc`、`Header.Columns.GetColumnBounds`） | `990-1003`、`1149-1155`、`1267-1282`、`1344-1357`、`1479-1485`、`1588-1603` | `IVTEditLinkSeam` / `TVtEditLinkSeamBase` / `TVtEditorFactory.Create` / `IVirtualTreeHost.GetColumnBounds` |
| `TVirtualStringTree` 本体（`AddChild`/`Clear`/`GetFirst`/`GetNext`/`EditNode`/`EndEditNode`/`Invalidate`/`CheckState`/`Selected`/`Focused`/`Font`） | 全单元 | `IVirtualTreeHost` + `CustomMagicTreeHost`（内存替身） |
| 真实绘制（`ilCheck.Draw`、`TargetCanvas.Font.Color`、`Brush.Style`） | `4326-4346`、`4468-4488`、`4587-4607` | `TImageListSeam.Draw` 记录 + `TargetCanvasFontColorMirror` / `BrushStyleClearMirror` 决策镜像 |
| 模态框 `ShowModal` / `Showmessage` | `1632`、`3664` | `CustomMagicMessageBoxSeam`（`UiEnabled` 默认 true，测试强制 false） |
| `TSpinEditEx` / `TSpinEdit` | `1178`、`1508`、`4741` | `TSpinEditExSeam` / `TSpinEditSeam` |
| `M2Share`/`UsrEngn`/`EDCode`/`M2Threads`/`CheckUnit` 全局量 | `855-856`、`5-7` | `CustomMagicFormGlobals`（`g_Config` 两项、`g_MultiThreadRun`、`LockR/UnLockR`、`g_CustomMagicListText*`、`BufferCrc`、`zLibCompressBuffer`、`g_EffectImageList`、`UserEngine.*`） |
| `uCustomMagicUtils.pas` 数据层（`TCustomMagicConfig` 默认值 / INI 读写 / `SaveCustomMagicClientConfigs`） | 该单元 `:247-317`、`:349-751`、`:758+` | `CustomMagicConfigDefaults.Apply` / `.SaveToIniFile`、`CustomMagicFormGlobals.SaveCustomMagicClientConfigs` |

### 2.3 `uses` 逐项处置

| 单元 | 是否已移植 | 处置 |
|---|---|---|
| `VirtualTrees.pas` | **否**（§2.3 不移植项，全仓无实现） | 最小接缝 + 纯逻辑替代（见 §4） |
| `SpinEditEx.pas` | **否** | 接缝 `TSpinEditExSeam`（`// 接缝：待 SpinEditEx.pas 移植后接入`） |
| `uCustomMagicUtils.pas` | **部分**（仅枚举/名称表，`Engine\CustomMagicUtils.cs` 批次J61） | 记录/类按原文逐字落在本车道命名空间；INI 读写留接缝 |
| `uFrmCustomMagicCopySetting.pas` | **是**（`Forms\CustomMagicCopySettingForm.cs`） | `ShowCustomMagicCopySettingHandler` 静态接缝（默认返回 false） |
| `Grobal2.pas` | **是**（`GXX.Core.Protocol`） | `TMagicClientBaseConfig`/`TMagicClientConfig`/`TClientCustomMagicConfig`/枚举全部直接复用；**`MagicNeedItemNames`（`Grobal2.pas:5497`）漏移植** → 本地常量 + 登记（§7） |
| `M2Share.pas` | 部分（无 `g_Config.boSendCustomMagicConfig` / `sCustomMagicClientConfigFileName`） | 接缝 |
| `UsrEngn.pas` | 部分（无 `m_CustomMagicList`/`ResetMagicCDList`） | 接缝 |
| `M2Threads.pas` | **是**（`p3-m2-sweep` 车道，`Sweep/M2Threads.cs`） | 不直接引用其内部；`g_MultiThreadRun` + `LockR/UnLockR` 走接缝（原文只是"加锁 + 计数"语义） |
| `CheckUnit.pas` | **是**（`GXX.Core.Util.CheckUnit`） | `BufferCrc` 默认转调 `CheckUnit.BufferCrc` |
| `EDCode.pas` | **是**（`GXX.Core.Protocol.EDcode`） | `zLibCompressBuffer` 默认转调 `EDcode.zLibCompressBuffer` |

### 2.4 大段常量/属性表的"脚本抽取 + 回读比对"

全部**脚本从 GBK 原文抽取**，无手工转录：

1. **600 个组件字段**：`uFrmCustomMagic.pas:17-616` 按行号抽取 `name: TType;` → `CustomMagicForm.Components.cs`（每行带原文行号注释），测试 `ComponentFields_Are600` 回读断言为 600。
2. **151 个"单字段写回"处理器**：按"去空行/去 `//` 注释后的规范体"匹配 7 行形状
   `begin / if <G> <> nil then / begin / <G>.<Path> := <Rhs>; / SetConfigChanged(); / end; / end;`
   → 生成代码 + 生成测试夹具 `CustomMagicHandlerTable.g.cs`（151 行，带原文行号）
   并**由表驱动测试反向执行**（4 个 Theory × 151 = 604 例）。
3. **232 条方法声明**：`procedure|function|destructor|constructor <Owner>.<Name>` 全量登记 → `CustomMagicMethodTable.g.cs`，逐条反射断言同名成员存在。
4. **名称表**：`M2Share.pas:384-391` 的 7 张表与 `Engine\CustomMonsterState.cs:35-43` 逐项比对一致后直接复用（`CustomDrawModeNames`＝`透明绘制/普通绘制`，**不是** `CustomNpcForm.cs:129` 的 `混合/普通`——两处同名表内容不同，见 §6 易错点）。

---

## 3. 逐方法族判定表

图例：**✔ 已移植+已测** ｜ **seam 已留接缝（壳已实现、真实控件未接线）** ｜ **✖ 未覆盖**

| Delphi 行号 | 方法族 | 判定 |
|---|---|---|
| `1605-1623` | `SetControlEnabled`（递归） | ✔ |
| `1625-1636` | `ShowCustomMagic` | ✔（`ShowModal` 走 seam；`UiEnabled=false` 时返回 false 不挂死） |
| `1638-1666` | `DoOpen` | ✔ |
| `1668-1861` | `FormCreate` | ✔（DFM 父子层级未复刻 → §8） |
| `1863-1875` | `SetConfigChanged` | ✔ |
| `1877-1893` | `vstCustomMagicDrawText` | ✔（颜色判定纯逻辑；画布=镜像） |
| `1895-1898` | `vstCustomMagicGetNodeDataSize` | ✔ |
| `1900-1908` | `vstCustomMagicGetText` | ✔ |
| `1910-2376` | `vstCustomMagicNodeClick` | ✔（全部 467 行的判定与赋值；11 个附加伤害槽改为"索引化逐槽同语句"） |
| `2378-2509` | `cbbClientLevelChange` | ✔（含 `:2502-2506` 双重还原） |
| `2519-2532` | `cbbClientActionTypeChange` | ✔ |
| `2542-2550` | `chkClientNotRaiseHandClick` | ✔（表驱动） |
| `3082-3090` / `3145-3153` | `seTargetStatus1/2_PlayTimeChange` | ✔（**空体**，原文如此） |
| `3172-3195` | `cbbOperateModeChange` | ✔ |
| `3196-3242` | `seUseIntervalChange` / `edtFail/Succeed/CloseMsgChange` / `chkFailNoShowEffClick` | ✔（`chkFailNoShowEffClick` 为 `{}` 死代码，保留空方法） |
| `3243-3259` | `cbbAttackTargetChange` | ✔ |
| `3260-3330` | 攻击范围/威力/需物品族 | ✔ |
| `3331-3531` | 附加伤害/目标状态/传送族 | ✔ |
| `3376-3459` | 附加伤害 6 个 Tag 驱动处理器 | ✔（Tag 越界跳写但仍 `SetConfigChanged()`） |
| `3532-3600` | `RebuildCustomMagicListText` | ✔ |
| `3601-3627` | `btnSaveClick` | ✔ |
| `3628-3665` | `btnMakeConfigDataClick` | ✔（落盘走 seam） |
| `3666-3671` | `chkSendCustomMagicConfigClick` | ✔ |
| `3672-3700` | 召唤怪物 2 个 Tag 驱动处理器 | ✔ |
| `3917-3934` | `edtSound1Change` | ✔ |
| `4007-4056` | `cbbMagicSwitchModeChange` / `btnCopyConfigClick` | ✔ |
| `4183-4208` | `chkCheckVarValueClick` | ✔ |
| `4724-4799` | 破防 5 个 Tag 驱动处理器 | ✔ |
| `932-1604` | 两个 EditLink 全 20 个方法 | ✔ 逻辑；**编辑器 new/free/消息泵 = seam** |
| `4290-4714` | 三棵树 × 6 事件 + `CreateEditor` | ✔（`CreateEditor` 返回接缝 EditLink 实例） |
| `4420-4428`、`4533-4550`、`4715-4723` | 4 个 `WM_STARTEDITING_*` | ✔（`PostMessage` 走 seam，`WPARAM` 由节点地址退化为节点序号） |
| `17-616` | 600 组件字段 | ✔ 字段面；**设计期属性值/父子层级 ✖** |
| `853` | `{$R *.dfm}` | ✖（DFM 未解析；`uFrmCustomMagic.dfm` 6,000+ 行） |
| `1-14`、`831-849` | 常量/状态字段/public 声明 | ✔ |

---

## 4. `VirtualTrees` 依赖的具体处置

> 依赖：`TVirtualStringTree` / `TVirtualDrawTree` / `TBaseVirtualTree` / `IVTEditLink` / `THitInfo` / `TVirtualNode` / `TVirtualNodeStates` / `TCheckState` / `TVSTTextType` / `TColumnIndex`。全仓无实现（`§2.3 不移植项`），且 `M2Engine` 另有 8 个单元用它（`GameCommand`/`MonsterConfig`/`uFrmCombatPowerSetting`/`uFrmCustomNpc`/`uFrmHeroMagicSetting`/`uFrmItemDropLog`/`uFrmPlugManager`/`ViewList2`），故本车道的接缝层是**后续共用资产**。

### 4.1 用纯逻辑替代（可 100% 断言）

| VirtualTrees 能力 | 原文用法 | 纯逻辑替代 |
|---|---|---|
| `OnGetText` | 三棵树 14/11 列文本 | `GetAttackDecAttribText` / `GetProtectedAddAttribText` / `ElementTreeLogic.GetText` / `CustomMagicMainTreeLogic.GetText` |
| `OnDrawText` 改字体色 | `Config.IsChanged → clRed`，否则选中&&聚焦 → `clHighlightText` | `CustomMagicMainTreeLogic.ResolveFontColor`（含分支优先级断言） |
| `OnAfterCellPaint` 画勾/叉图标 | 提示列 `ImageIndex := Integer(ShowHint)`，坐标居中 | `TryGetHintIconLayout`（含 `div` 向零截断断言） |
| `OnChecked` | `CheckState[Node] = csCheckedNormal` → `IsChecked` | `ApplyChecked` |
| `OnEditing`（`var Allowed`） | 列 + 属性类型的禁编辑矩阵 | `IsAttackDecAttribEditingAllowed` / `IsProtectedAddAttribEditingAllowed` / `IsEditingAllowed` |
| `OnNodeClick`（`THitInfo`） | 提示列取反 / 其它正列发起编辑 | `ResolveNodeClick` + `ElementTreeLogic.StartEditingMessage` |
| `Node.CheckType/CheckState` | 建节点时按 `IsChecked` 置勾 | `CustomMagicTreeHost.SetCheckState` + 断言（`csCheckedNormal`/`csUnCheckedNormal`） |
| `NodeDataSize` + `GetMem` 清零 | 每节点一块数据 | `CustomMagicTreeHost.NodeDataFactory`（`AddChild` 时造对象；`GetNodeData` 永不为 nil，与原文一致） |
| `Header.Columns.GetColumnBounds` | `SetBounds` 取列右边界 | `IVirtualTreeHost.GetColumnBounds` + `…Logic.SetBoundsSpec` |
| `Selected[Node]` / `Focused` / `Font.Color` | 红字/高亮判定 | `IsSelected` / `Focused` / `FontColor` |

### 4.2 留接缝（未覆盖）

| 项 | 接缝 | 状态 |
|---|---|---|
| `IVTEditLink` 的**创建/销毁** | `TVtEditorFactory.Create`（默认真造接缝控件；可替换） | 壳已实现，真实 `TSpinEditEx`/`TComboBox`/`TEdit` 未接 |
| **消息泵** | `IVTEditLinkSeam.ProcessMessage` → 只记 `LastProcessedMessage`（原文 `FEdit.WindowProc`） | ✖ 未接真实窗口过程 |
| 编辑器的显示/隐藏/聚焦 | `FEdit.Visible` / `FocusedEditor` 镜像（原文 `Show`/`SetFocus`/`Hide`） | 半覆盖 |
| **真实绘制** | `TImageListSeam.Draw` 记录 `(x,y,index)`；`TargetCanvasFontColorMirror` / `BrushStyleClearMirror` | ✖ 无像素级断言 |
| **真实 `TreeView(OwnerDraw)` 适配器** | 原计划照 `p2-rungate-impl` 的 `uFrmGameSpeed` 做法实现 `TreeView(DrawMode=OwnerDrawText)` 适配 `IVirtualTreeHost` | **✖ 本切片未实现**（见 §8 剩余量第 1 条） |
| 列级命中测试（`HitColumn`） | `THitInfo.HitColumn`（WinForms `TreeView` 无列概念） | ✖（需 `ListView` 或自绘网格，登记为未覆盖） |
| 滚动 / 表头拖拽 / 键盘导航 | — | ✖ |

### 4.3 未覆盖清单（VirtualTrees 相关）

1. 真实 `TreeView(OwnerDraw)` 适配器（含 `DrawNode` 事件到 `ResolveFontColor` 的接线）。
2. `TreeView` 的列文本渲染（多列树必须自绘或换控件）。
3. `EditNode` 的真实就地编辑（`IVTEditLink.ProcessMessage` 的 `WindowProc`）。
4. `ilCheck` 图标的真实 `Bitmap` 绘制（DFM `:6058+` 的内嵌位图未搬运）。
5. `Node.CheckType = ctCheckBox` 的真实勾选框（WinForms `TreeView.CheckBoxes` 可用，但未接线）。

---

## 5. 新增文件 + 覆盖行号

### 5.1 源码（`src/GXX.M2Server/Forms/CustomMagic/`，命名空间 `GXX.M2Server.Forms.CustomMagic`）

| 文件 | 行数 | 覆盖的 Delphi 行号 | 未覆盖 |
|---|---|---|---|
| `CustomMagicModel.cs` | 363 | `uFrmCustomMagic.pas:9-13、858-885`；`uCustomMagicUtils.pas:83-238、753-756` | `uCustomMagicUtils.pas:247-317、349-751、758+`（INI/默认值/落盘，另单元） |
| `CustomMagicSeams.cs` | 515 | `:5-13、855-856、1605-1623`（接缝面）、`:1632/3664` 的模态与提示 | 真实 WinForms 控件接线 |
| `CustomMagicEditLinkLogic.cs` | 489 | `:942-986、1007-1145、1156-1263、1274-1282、1296-1340、1361-1475、1486-1587、1595-1603` | — |
| `CustomMagicEditLinkSeam.cs` | 494 | `:888-928、932-938、990-1003、1149-1152、1267-1270、1286-1292、1344-1357、1479-1482、1588-1591` | `WindowProc` 真实消息泵；`Header.Columns` 真实列宽 |
| `CustomMagicTreeLogic.cs` | 311 | `:1877-1908、4290-4413、4429-4532、4551-4708` | 真实绘制 |
| `CustomMagicForm.Components.cs` | 1,220 | `:17-616`（600 字段） | 设计期属性值、父子层级 |
| `CustomMagicForm.Handlers.cs` | 1,534 | 151 个处理器（行号见每方法注释） | — |
| `CustomMagicForm.cs` | 911 | `:1605-1623、1625-1636、1638-1666、1668-1861、1863-1875、1877-1908、1910-2376` | DFM 层级 |
| `CustomMagicForm.Actions.cs` | 928 | `:2378-2509、2519-2532、3082-3090、3145-3153、3172-3671、3672-3700、3917-3934、4007-4056、4183-4208、4290-4723、4724-4799` | `SaveCustomMagicClientConfigs` 真实落盘（seam） |
| **合计** | **6,765** | | |

### 5.2 测试（`tests/GXX.M2Server.Tests/`）

| 文件 | 行数 | 内容 |
|---|---|---|
| `CustomMagicTestBase.cs` | 90 | 串行集合 `CustomMagicFormLane`（`DisableParallelization`）+ 接缝复位 + 无头开关 |
| `CustomMagicHandlerTable.g.cs` | 198 | **脚本生成的 151 行来源表** |
| `CustomMagicMethodTable.g.cs` | 275 | **脚本生成的 232 行方法声明表** |
| `CustomMagicFormHandlerTests.cs` | 177 | 151 处理器 × 4 分支（守卫 nil / 正常写回 / 边界值 / RHS 差异断言）+ 表结构自检 |
| `CustomMagicMethodParityTests.cs` | 150 | 232 条逐条反射断言 + 差异断言 + 字段名断言 + 组件计数 |
| `CustomMagicEditLinkLogicTests.cs` | 435 | PrepareEditSpec / ApplyEditorResult 全列覆盖 + 差异断言 + 键决策 |
| `CustomMagicEditLinkSeamTests.cs` | 472 | 创建/销毁/A-B-C/边界/消息副作用/Owner 通知 |
| `CustomMagicTreeLogicTests.cs` | 406 | 列文本全列 / 三段禁显差异 / 编辑矩阵 / 点击决策 / 图标布局 |
| `CustomMagicFormCoreTests.cs` | 1,663 | 窗体主体（见下） |
| **合计** | **3,866** | |

`CustomMagicFormCoreTests.cs` 覆盖：`SetControlEnabled`（递归/空容器）、`SetConfigChanged`（nil/真/假/聚焦节点）、`ShowCustomMagic`（OK/Cancel/无头/副作用）、`DoOpen`（0/2 项 + 多线程锁）、`FormCreate`（10 张名称表 + 文件下拉 + 标签挪位 + 状态清零）、`CbbClientLevelChange`（-1/0/3/4/`int.MinValue`）、`VstCustomMagicNodeClick`（nil 焦点/nil 节点数据/普通技能全量装载/战士技能强制近攻 + 禁用飞行）、`VstCustomMagicDrawText/GetText`、`BtnSaveClick`（只落盘 Changed）、`RebuildCustomMagicListText`（空/2 项/战士近攻夹取/`IsCheckVarValue` 清零/真实 zLib+CRC）、`BtnMakeConfigDataClick`（取消/成功/多线程/兜底序列化）、`ChkSendCustomMagicConfigClick`、`BtnCopyConfigClick`、6 组 Tag 驱动族（界内/越界/错误 Sender 类型）、树事件处理器与 4 个 `WM_`、短字符串截断。

---

## 6. 发现的原文缺陷 / 易错点

### 6.1 原文缺陷（带 `文件:行`）

| # | 位置 | 说明 |
|---|---|---|
| **1** | `uFrmCustomMagic.pas:1161` + `:1175-1262`（元素链接 `:1486` + `:1505-1583`） | `PrepareEdit` 先 `Result := True`，`case FColumn of` **无 `else`**：不认识的列不建编辑器但**仍返回 True**，`FEdit` 保持 `nil`。`Editing`（`:4402` 只禁 `Column=0/12`；`:4523` 只禁 `9`）**不拦列 14+** → 随后 `BeginEdit`（`:993 FEdit.Show`）与 `EndEdit`（`:1135 FEdit.Visible`）必 AV。托管侧保留 `Result=True` 与 `FEdit=null`，并让 `BeginEdit/CancelEdit/EndEdit/GetBounds/ProcessMessage/SetBounds` 抛 `NullReferenceException`（测试锁定）。 |
| **2** | `:1938` | `FCurrentClientConfig := @FCurrentCustomConfig.ClientBaseConfig;` —— `ClientBaseConfig` 是 `TMagicClientBaseConfig`，而 `FCurrentClientConfig` 是 `PMagicClientConfig`（**不同记录类型**）。在默认 `{$T-}` 下 `@` 返回**无类型** `Pointer`，故编译通过：一个类型双关的野指针。它之所以没炸，是因为 `:1943-1944` 立刻调用 `cbbClientLevelChange`，由 `:2394` 把它改成 `ClientConfigs[0]`；**期间从未解引用**。任何调整 `:1943-1944` 顺序的改动都会变成静默内存错乱。托管侧无法复刻双关指针，改为 `null` + 注释（行为等价，已登记偏差）。 |
| **3** | `:3278-3286` | `seAttackLineWidthChange` 的 **guard 与 target 不一致**：`if FCurrentServerConfig <> nil then FCurrentCustomConfig.ServerConfig.AttackLineWidth := …`。两者恒同时非 nil 故未暴露。**照抄原文**，并用"两个不同配置对象"的测试锁定该行为。 |
| **4** | `:4709-4714` + `:1156-1263` | `vstProtectedAddAttrCreateEditor` **复用** `TDecAttribPropertyEditLink`，而保护属性树的节点数据是 `TProtectAddAttribData`，被该链接当 `PAttackDecAttribData` 使用。能工作是因为两个记录**布局完全相同**（1 字节枚举 + 3 填充 + 4 字节指针），且两条 EditLink 路径**都不读 `AttribType`**。托管侧显式做"布局等价重解释"适配并共享同一 `Data` 引用。 |
| **5** | `:963-967`（`:1317-1321` 同） | `VK_UP/VK_DOWN` 分支先 `CanAdvance := Shift = []`，紧接着 `else if FEdit is TSpinEditEx then CanAdvance := True;` **无条件覆盖**——按住 Shift 时 `TSpinEditEx` 仍会上/下移，而 `TEdit`（及未建编辑器）受 Shift 约束。两条路径行为不同（差异断言已写）。 |
| **6** | `:1897` | `vstCustomMagicGetNodeDataSize` 填的是 `SizeOf(TCustomMagicConfig)`（**类引用大小 4 字节**），而节点数据实际是 `TMagicConfigNodeData` 记录（指针字段，8 字节：1 字节枚举对齐后 + 4）。数据块大小与 `GetNodeData` 的解引用宽度不符；当前侥幸成立，若 `TMagicConfigNodeData` 增字段即越界。 |
| **7** | `:3206-3214` | `chkFailNoShowEffClick` **整段处于 `{ }` 注释内 → 死代码**（DFM 里的 `chkFailNoShowEff` 仍存在，但点击无任何效果）。 |
| **8** | `:2502-2506`（`:2372-2375` 同型） | `SetConfigChanged(OldChanged); FIsConfigChanged := OldIsConfigCanSave; if not FIsConfigChanged then btnSave.Enabled := False;` —— `SetConfigChanged` 刚把 `btnSave.Enabled := True`（`:1872-1873`），随后可能立刻被改回 `False`，产生"配置已 Changed 但保存按钮被禁用"的中间态。装载期为有意为之（避免误标记）。 |
| **9** | `:2397` 与 `:2555` | 文件下拉是 `ItemIndex ± 1` 的偏移编码，**无下限保护**：`Icon_File = -1` 时再次 `-1` 会写回 `-2`（Delphi 下静默）。托管侧照抄（`(short)(ItemIndex - 1)`），测试覆盖 `ItemIndex = 0 / -1 / int.MinValue`。 |
| **10** | `:3633` 与 `:3635` | `SetCurrentDirectory` 在**取消分支也调用一次**，成功分支再调一次；原文 `:3632-3635` 的顺序是"Execute 失败 → SetCurrentDirectory → Exit"。托管侧 1:1 保留（测试断言取消分支 1 次、成功分支 1 次）。 |
| **11** | `:4462-4464` | `vstDecElement` / `vstAddElement` **共用同一组事件处理器**，靠 `Sender = vstDecElement`（**指针比较**）选 `WM_STARTEDITING_{DEC,INC}_ELEMENT`。托管侧必须传对宿主对象；测试用两个宿主各发一次并断言消息号不同。 |
| **12** | `:1995-2002`、`:1946-1947`、`:1972`、`:2050`、`:2101`、`:2187`、`:2395`、`:2428`、`:2490`、`:2498`、`:1860` | 被注释掉的语句/整段（`chkClientLevelEnabled`、`chkClientWarr`、`lblMagicWarrNGOption.Visible`、`chkFailNoShowEff.Checked`、`seProtectSelfRate`、`grpClientAttackConfigs.Caption`、`cbbClientSelfPlayMode`、`seTargetStatus1/2_PlayTime`、`// DoOpen`）。**逐条以 `// 原文 :NNNN 注释掉` 保留在对应位置**。 |
| **13** | `:3082-3090`、`:3145-3153` | 两个事件处理器**体为空**（只有 `if FCurrentClientConfig <> nil then begin end;`），却仍挂在 DFM 上。托管侧保留空体方法。 |

### 6.2 跨语言 / 跨库易错点（本车道实测踩到）

1. **packed struct 的值语义（踩到过）**：`TMagicClientConfig` 是 `unsafe struct`。`var c = holder.Value;` 得到**副本**，改副本不写回配置。必须 `holder.Value.Field = ...` 就地赋值。测试首轮因此失败 11 例。
2. **Delphi `Boolean` → C# `byte`**（GXX.Core 生成约定：`MagicLock: Boolean` → `public byte MagicLock;`）：赋值 `(byte)(b ? 1 : 0)`，读取 `!= 0`。直接写 `bool` 编译不过。
3. **C# 枚举强转必须 `(TEnum)(expr)`**：Delphi 写 `TCustomDrawMode(cbb.ItemIndex)`，直译到 C# 会得到 `CS1955: Non-invocable member cannot be used like a method`（生成器已做替换）。
4. **`div` 与 `/` 都向零截断**：`(4-13)/2 = -4`，与 Delphi `div` 一致（本车道有断言）。
5. **`SameText` → `StringComparison.OrdinalIgnoreCase`**：`HintText` 的大小写不敏感比较（`:1125`、`:1455`）。
6. **同名不同内容的名称表**：`M2Share.pas:386 CustomDrawModeNames = ('透明绘制','普通绘制')`，而 `Forms/CustomNpcForm.cs:129` 里也叫 `CustomDrawModeNames` 但内容是 `('混合','普通')`（来源不同）。**跨窗体复用名称表前必须回原文核对**。
7. **`g_EffectImageList` 的正式归属**是 `Forms/ViewList2State.g_EffectImageList`（已有车道产出），本车道不另造。
8. `Assert.Equal` 的**期望/实际**两侧要分清：本轮 17 例失败里 **6 例是测试期望写错**（`aaHide` 也 `>= aaHitPoint`；`IsChecked` 未设却断言 `Checked`），其余是真实的实现/接缝缺口。

---

## 7. 接缝清单 + 需要改白名单外文件的精确签名要求

> 以下 4 项**必须由集成方/对应车道在区外文件上落地**；本车道一律不越区。

### 7.1 `GXX.Core.Protocol`：补 `MagicNeedItemNames`（**漏移植**）

- 原文：`Source\Common\Grobal2.pas:5497`
  ```pascal
  MagicNeedItemNames: array [TMagicNeedItem] of string = ('无', '红毒', '绿毒', '符', '自定义物品');
  ```
- 要求（精确签名，放 `src/GXX.Core/Protocol/Grobal2.Const.g.cs` 或同目录名称表静态类即可）：
  ```csharp
  /// <summary>Grobal2.pas:5497 MagicNeedItemNames（下标 = TMagicNeedItem）。</summary>
  public static readonly string[] MagicNeedItemNames = { "无", "红毒", "绿毒", "符", "自定义物品" };
  ```
- 现状：本车道在 `CustomMagicForm.cs` 用私有 `CustomMagicMagicNeedItemNames` 暂存（带接缝注释与逐字值）。
- **落地后请通知本车道改为引用**（同时删掉私有副本，避免第 7 份重复）。

### 7.2 `GXX.M2Server.Engine.TCustomMagicConfig` 重名（**必须裁定，否则持续摩擦**）

- **正式归属**：`uCustomMagicUtils.pas:215`（M2Engine 根目录单元） → 未来应落在 `src/GXX.M2Server/Engine/`（与 `CustomMagicUtils.cs` 同目录）。
- **现状 A**：`src/GXX.M2Server/Engine/MagicBatchH.cs:62` 已有一个**子集**同名类
  `public class TCustomMagicConfig`（ctor `TCustomMagicConfig(ushort magicId)`，仅 `DisableInSafeZone`/`TargetPattern` 等服务端运行字段）。
- **现状 B**：本车道在 `GXX.M2Server.Forms.CustomMagic` 按原文落了**完整面**
  `TCustomMagicConfig(string aMagicName, ushort aMagicID, bool aIsMagicWarr)` + `SetChanged(bool value = true)` + `IsChanged` + `MagicName`/`MagicID`/`IsMagicWarr` + `ClientBaseConfig`/`ClientConfigs`/`ServerConfig`。
- **裁定建议（任一，需集成方执行）**：
  1. `MagicBatchH.cs:62` 的子集改名为 `TCustomMagicSubsetConfig`（改动最小，且它是"服务端运行时子集"，语义上确实不是同一物）；**或**
  2. 把完整面搬到 `Engine/CustomMagicUtils.cs` 旁，`MagicBatchH.cs` 的子集改为 `: TCustomMagicConfig`。
- **在裁定前**：任何同时 `using GXX.M2Server.Engine;` 与 `using GXX.M2Server.Forms.CustomMagic;` 的**测试文件**都会 `CS0104`。本车道已全部规避（测试里用
  `using TCheckVarType = GXX.M2Server.Engine.TCheckVarType;` 等 5 个别名，不 import `Engine` 命名空间）。
- 同族另两个类型**无冲突**：`TMagicServerConfig`（本车道）、`TMagicClientConfig`/`TMagicClientBaseConfig`/`TClientCustomMagicConfig`（`GXX.Core.Protocol`，直接复用）。

### 7.3 `uCustomMagicUtils.pas` 数据层（未移植，非本单元）

需要（**签名按原文**）：

```csharp
// 1) 默认值：uCustomMagicUtils.pas:349-751（约 400 行 Create 体）
//    托管接缝：CustomMagicConfigDefaults.Apply: Action<TCustomMagicConfig>?
public TCustomMagicConfig(string aMagicName, ushort aMagicID, bool aIsMagicWarr);

// 2) INI 读写：uCustomMagicUtils.pas:758+ LoadFromIniFile / SaveToIniFile
//    托管接缝：CustomMagicConfigDefaults.SaveToIniFile: Action<TCustomMagicConfig>?
public void LoadFromIniFile();
public void SaveToIniFile();          // btnSaveClick（:3601-3627）对 IsChanged 节点调用

// 3) 落盘：uCustomMagicUtils.pas:247-317
//    procedure SaveCustomMagicClientConfigs(MagicConfigs: TList; FileName: string);
//    托管接缝：CustomMagicFormGlobals.SaveCustomMagicClientConfigs: Action<List<TCustomMagicConfig>, string>?
public static void SaveCustomMagicClientConfigs(List<TCustomMagicConfig> magicConfigs, string fileName);

// 4) 查找：uCustomMagicUtils.pas:319-346
//    function GetCustomMagicConfig(MagicID: Word): TCustomMagicConfig;
public static TCustomMagicConfig? GetCustomMagicConfig(ushort magicId);
```
原文落盘格式（供接管者核对）：`MS.Write(ClientCustomMagicConfigFlag)` → `Count: Integer` → `SizeOf(TClientCustomMagicConfig): Integer` → `CRC: Cardinal` → N × `TClientCustomMagicConfig`，再回填 CRC。
本车道 `RebuildCustomMagicListText` 已按同一布局打包（`Marshal.SizeOf<TClientCustomMagicConfig>()` 与原文 `SizeOf` 一致），可由接管者直接复用。
当前兜底：`BtnMakeConfigDataClick` 在未接线时调用 `SaveCustomMagicClientConfigsFallback`，只序列化到 `TFrmCustomMagic.LastSavedConfigBytes`（**不落盘**）。

### 7.4 `M2Config`（`g_Config`）与 `UserEngine`

```csharp
// src/GXX.M2Server/Engine/M2Config.*.cs（或同族 partial）
public static bool   boSendCustomMagicConfig;              // 原文 :1645 / :3667
public static string sCustomMagicClientConfigFileName;     // 原文 :3630 / :3636
public static string sCustomMagicDir;                      // uCustomMagicUtils.pas:775

// UserEngine 接缝（C# 现有惯例见 Engine/M2Config.GameSpeed.cs:110-121 GameConfigState）
public static readonly List<TCustomMagicConfig> m_CustomMagicList;   // 原文 :1652 / :3538
public static void SendServerConfig();                               // 原文 :3626
public static void ResetMagicCDList();                               // 原文 :3625
```
现状：`CustomMagicFormGlobals` 暂存（`SendServerConfig` 已转调既有 `Engine.GameConfigState.SendServerConfig()`）。

### 7.5 长期接缝（无需他人改动，供后续车道复用）

| 接缝 | 位置 | 说明 |
|---|---|---|
| `IVirtualTreeHost` / `CustomMagicTreeHost` / `IVTEditLinkSeam` / `TVtEditorFactory` | `Forms/CustomMagic/CustomMagicEditLinkSeam.cs`、`CustomMagicSeams.cs` | **`M2Engine` 另外 8 个用 VirtualTrees 的窗体（`GameCommand`/`MonsterConfig`/`uFrmCombatPowerSetting`/`uFrmCustomNpc`/`uFrmHeroMagicSetting`/`uFrmItemDropLog`/`uFrmPlugManager`/`ViewList2`）可直接复用，勿另造** |
| `CustomMagicMessageBoxSeam` | `CustomMagicSeams.cs` | 对应 `GXX.RunGate.uFrmGameSpeedLogic.MessageBoxSeam`；**`UiEnabled` 默认 true，测试必须置 false，否则挂死 testhost** |
| `CustomMagicFormLane`（`CollectionDefinition`，`DisableParallelization`） | `tests/…/CustomMagicTestBase.cs` | 新增窗体测试类必须挂 `[Collection("CustomMagicFormLane")]` |
| 表驱动探针 `public` 化 | `CustomMagicForm.*`、`CustomMagicEditLinkSeam.cs` | 测试工程是独立程序集，`internal` 不可见；`FCurrent*`/`Mirror`/`Calls` 等探针均为 `public` |

---

## 8. 诚实说明未完成部分与剩余量

**已完成并全绿**：本单元 `implementation` 段 `:858-4799` 的**全部 232 个方法**都有同名托管成员（门禁断言），其中逻辑密集部分（151 处理器、两个 EditLink 的取值/写回、五棵树的列文本/许可/勾选/点击、`SetControlEnabled`、`SetConfigChanged`、`DoOpen`、`FormCreate`、`cbbClientLevelChange`、`vstCustomMagicNodeClick`、`RebuildCustomMagicListText`、`btnSaveClick`、`btnMakeConfigDataClick`）**逐行移植并单测**。

**未完成（按优先级）**：

| 优先 | 项 | 规模 | 说明 |
|---|---|---|---|
| 高 | **真实 `TreeView(OwnerDraw)` 适配器** | ~150-250 行 | 本切片只把"列文本/红字/勾叉/编辑许可"抽成纯逻辑并用**内存宿主**验证；**没有**把它们接到真实 WinForms 控件上。`IVirtualTreeHost` 的接口面已按此设计（`AddChild`/`Clear`/`GetFirst`/`GetNext`/`EditNode`/`GetColumnBounds`/`Selected`/`Focused`/`FontColor`），照 `p2-rungate-impl` 的 `uFrmGameSpeed` 做法补一个适配器即可，**不建议另造接口**。 |
| 高 | **DFM（6,000+ 行）的设计期属性与父子层级** | 大 | 600 个字段名/顺序/类型已 1:1；但 `Left/Top/Width/Height/Caption/Anchors/Items/Enabled` 等设计期值只落地了被逻辑读到的那几项（`btnSave.Enabled=False`、`ilCheck 13×13`、`cbbClient*` 的 Items 由 `FormCreate` 填）。`Controls` 父子层级未复刻 ⇒ `SetControlEnabled(pgcMain, False)` 在托管侧是"空操作"（其递归语义已单独测试）。 |
| 中 | 两个 EditLink 的**消息泵**（`FEdit.WindowProc`） | ~20 行 | `ProcessMessage` 目前只记 `LastProcessedMessage`。 |
| 中 | **列级命中测试**（`THitInfo.HitColumn`）与多列树渲染 | 中 | WinForms `TreeView` 无列概念；要在真实 UI 上复刻 `HitColumn=12/9` 的提示列与"点其它列开始编辑"，需 `ListView`（Detail）或自绘。 |
| 中 | `ilCheck` 内嵌位图 + 真实 `Draw` | 小 | DFM `:6058+` 的位图未搬运；当前记 `(x,y,index)`。 |
| 低 | `uCustomMagicUtils.pas` 数据层（另一单元） | 1,754 行 | 默认值 400 行 + INI 读写 + 落盘 + 查找；本车道只留接缝（§7.3）。**建议单开一条车道**，因为它同时被 `uFrmCustomMagic` / `uFrmHeroMagicSetting` / `ObjBase.pas:26451+` 使用。 |
| 低 | `MagicNeedItemNames`（`Grobal2.pas:5497`） | 1 行 | §7.1。 |
| 低 | `TCustomMagicConfig` 重名裁定 | 裁定 | §7.2，**需集成方执行**。 |

**未覆盖行号汇总**：`uFrmCustomMagic.pas:853`（`{$R *.dfm}`）、`:1-14`/`:17-616` 中"方法签名以外的设计期属性值"、`:855-856`（uses，接缝）、`:4326-4346`/`:4468-4488`/`:4587-4607` 的真实绘制、`:1267-1270`/`:1588-1591` 的真实窗口过程。其余实现段均有托管对应物。

**风险提示（给集成方）**：
1. 合并前请确认 `main` 未移动（本车道基于 `35c09a67`）。
2. 本车道**只新增文件**，与任何既有文件零冲突；唯一需要他人动作的是 §7.2 的重名裁定。
3. 若后续有人把完整 `TCustomMagicConfig` 放进 `GXX.M2Server.Engine` 而**未**处理 `MagicBatchH.cs:62`，会立刻 `CS0101` —— 这正是台账 §12.8/§14 的复现模式，请按「一个文件只有一个写者」处理。

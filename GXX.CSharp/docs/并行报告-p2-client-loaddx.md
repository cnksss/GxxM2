# 并行报告 · 车道 `p2-client-loaddx`（控件布局资源栈 1:1 移植）

> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p2-client-loaddx`
> 分支：`par/p2-client-loaddx`
> 源单元：`Source/Client-HGE/LoadDxControl.pas`（实测 **1,771** 行）、`Source/Client-HGE/LoadDxControlEx.pas`（实测 **2,469** 行）
> 依赖记录定义：`Source/Client-HGE/DxComponent/DxComponents.pas`（接口段 13-899 行）
> 日期：2026-09-20

---

## 1. 交付速览

| 项 | 内容 |
|---|---|
| 分支 | `par/p2-client-loaddx` |
| commit | `819379e8`（并行批次P2-1：代码+测试）、`d6c4b5f9`（并行批次P2-2：本报告）、并行批次P2-fix（去重，见 **§10**） |
| 门禁 | `dotnet build GXX.slnx -c Debug` → **0 error**（71 个警告全部来自既有文件，**本车道新增文件 0 警告**）；`dotnet test tests\GXX.Client.Tests` → **2425 passed / 0 failed** |
| 新增测试 | **147** 个（`--list-tests` 差分：全量 2425，前缀 `GXX.Client.Tests.LoadDx` 147；本车道开工前基线 2278） |
| 新增源文件 | 8 个（含 1 个生成器 + 1 个生成文件） |
| 行号口径 | 任务书写的 1,550 / 2,125 是旧统计；本报告一律用**实测行数**（`Get-Content -Encoding Default` 计数） |

### 1.1 新增文件清单

| 路径 | 行数级 | 说明 |
|---|---|---|
| `GXX.CSharp/src/GXX.Client/LoadDx/GuiRecords.g.cs` | 175 KB | **脚本生成**：52 条 `TGui*` 记录 + 8 个 VCL 枚举，带 `SizeOf`/`AlignOf`/`DelphiField[]` 偏移表 + `ReadAt`/`WriteAt`/`ToBytes`/`FromBytes` |
| `GXX.CSharp/src/GXX.Client/LoadDx/Tools/GenGuiRecords.ps1` | 27 KB | **出处脚本**（纯 ASCII）：从 `DxComponents.pas` 抽取声明、按 Delphi `{$A8}` 规则算布局、生成 C#，并把 `AUDIT;类型;SizeOf;对齐;字段@偏移+宽度` 表打到 stdout |
| `GXX.CSharp/src/GXX.Client/LoadDx/GuiComponentLoader.cs` | 83 KB | `LoadComponent`（两单元 1:1，23 个 case 分支全量）+ `NewDxControl` 公共部分 + `DxFontAssign`/`GuiFontAssign` |
| `GXX.CSharp/src/GXX.Client/LoadDx/DxControlSeams.cs` | 22 KB | 18 种未移植控件的**最小接缝** + `TDxControlExtra`/`TDxControlExtras` + `THashedStringList`/`TDxControlRef`/`TDxControlAddressList` |
| `GXX.CSharp/src/GXX.Client/LoadDx/GuiReaders.cs` | 8 KB | `IGuiReader` + `TDxMemoryReader`（内存版）+ `TDxStreamReader`（流版）+ 读取原语扩展 |
| `GXX.CSharp/src/GXX.Client/LoadDx/GuiResource.cs` | 6 KB | `LoadCompressedUIData`（ZDAT + zlib）+ `IDxGuiResourceProvider` + `DxGuiCrypt`（DES 头密钥） |
| `GXX.CSharp/src/GXX.Client/LoadDx/LoadDxControl.cs` | 11 KB | 单元 `LoadDxControl`：`LoadControlFromStream`/`LoadControlFromMemory`/`LoadSubComponent`/`NewDxControl` |
| `GXX.CSharp/src/GXX.Client/LoadDx/LoadDxControlEx.cs` | 19 KB | 单元 `LoadDxControlEx`：正式版 4 方法 + Patch 版 2 方法 |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxTestHelpers.cs` | — | 合成 `.GUI` 字节构造器（无仓库外资源依赖） |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxRecordLayoutTests.cs` | 20 用例 | 布局表回读比对（整表 52 条）+ 关键偏移 + 往返 |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxReaderTests.cs` | 22+9 用例 | 两个 `ReadMemory` 的边界 + 字体赋值（含 `LoadDxFontTests`） |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxControlLoaderTests.cs` | 58 用例 | 基础版：23 个分支 + 遍历 + 递归 + 地址线性登记 |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxControlExLoaderTests.cs` | 25 用例 | Ex 版：名字表 + Free 语义 + Patch 版 + **两单元差异断言** |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxResourceTests.cs` | 13 用例 | ZDAT→zlib→控件树端到端 + 异常路径 + DES 接缝 |
| `GXX.CSharp/tests/GXX.Client.Tests/LoadDxNamespaceCollisionTests.cs` | 5 用例 | **命名空间撞车守卫**（`GXX.Client.LoadDx` × `GXX.Client.DxComponent` 公开类型简单名交集必须为空），见 §10 |

---

## 2. 侦察：两个源单元全量清单（含行号）

### 2.1 `LoadDxControl.pas`（1,771 行）——"内存 + 指针数组"版

| 行号 | 种类 | 名称 / 内容 |
|---|---|---|
| 1 | unit | `LoadDxControl` |
| 4 | 编译开关 | `{.$DEFINE OUTPUT_GUI_READORDER}`（**关闭**） |
| 6-36 | uses | Windows/Messages/SysUtils/StrUtils/Classes/Graphics/Controls/StdCtrls/Grids + 20 个 Dx* 单元 + `UnitDes` |
| 38-39 | 接口过程 | `DxFontAssign`、`GuiFontAssign` |
| 42 | 接口函数 | `LoadControlFromStream(MemoryStream:TMemoryStream; Background:TDxControl; AControlAddress:Pointer; sUiName:string):Integer` |
| 43 | 接口函数 | `LoadControlFromMemory(Address, Memory:Pointer; Size:Integer; Background:TDxControl; sUiName:string):Integer` |
| 45 | — | `implementation` |
| 47-50 | 编译开关 | `{$IFDEF OUTPUT_GUI_READORDER} uses LogHelper {$ENDIF}` |
| 52-59 | **unit 级变量** | `PControlAddress:Pointer=nil`；`MemoryData:Pointer=nil`；`MemorySize:Integer=0`；`MemoryPosition:Integer=0`（58-59 是注释掉的 `ComponentIndex`/`ComponentList`） |
| 61-83 | 函数 | `ReadMemory(var Buffer; Count):Longint`（裸指针 `Move`，短读返回 nRem） |
| 85-92 | 过程 | `DxFontAssign(DxFont, GuiFont)`（5 字段：Color/BColor/Size/Bold/Style） |
| 94-102 | 过程 | `GuiFontAssign(var GuiFont, DxFont)`（同上 + `NameLen := Length(Name)`） |
| 104-114 | 函数 | `ReadGuiFontName(AFont):string`（仅 `NameLen>0` 时消费） |
| 116-1558 | 过程 | **`LoadComponent(Gui, DxControl, GuiVersion)`**（唯一入口：1437 行 case 分派） |
| 1560-1619 | 函数 | `NewDxControl(GuiHeader, AOwner):TDxControl`（case 建控件 + 12 项基础属性 + **写 `PControlAddress^` 并前移**） |
| 1621-1676 | 函数 | `LoadSubComponent(AOwner, GuiVersion, sUiName):Integer`（递归） |
| 1678-1681 | 函数 | `LoadControlFromStream`（转发到 Memory 版） |
| 1683-1769 | 函数 | `LoadControlFromMemory`（顶层遍历） |
| 1771 | — | `end.` |

**常量**：本单元**不声明任何常量**，全部版本界都是字面量（20160409 / 20160430 / 20160508 / 20160514 / 20160818 / 20170226 / 20171106 / 20180619 / 20190724 / 20190729 / 20211120）。
**枚举**：`TGuiType`（24 值，`t_None`..`t_SwitchButton`）来自 `DxComponents.pas:33`。

### 2.2 `LoadDxControlEx.pas`（2,469 行）——"TStream + 名字表"版（**运行时真正被调用的一支**）

| 行号 | 种类 | 名称 / 内容 |
|---|---|---|
| 1 | unit | `LoadDxControlEx` |
| 4 | 编译开关 | `{.$DEFINE OUTPUT_GUI_READORDER}`（关闭） |
| 6-39 | uses | 同基础版 + `IniFiles`、`SDK`、`ZlibEx` |
| 41-42 | 接口过程 | `DxFontAssign`、`GuiFontAssign` |
| 45 | 接口函数 | `LoadControlFromStream(streamUI:TStream; Background:TDxControl; ControlAddrList:THashedStringList; sUiName:string):Integer` |
| 46 | 接口过程 | `PatchLoadControlFromStream(streamUI; Background; ControlAddrList; sUiName)` |
| 47 | — | 注释掉的 `PatchControlFromSteam` |
| 48 | 接口函数 | **`LoadCompressedUIData(sResName:string; psResType:PChar):TMemoryStream`** |
| 50 | — | `implementation` |
| 52-55 | 编译开关 | `{$IFDEF OUTPUT_GUI_READORDER} uses LogHelper {$ENDIF}` |
| 57-71 | 函数 | `LoadCompressedUIData` 实现（`TResourceStream` + `Zlibex.ZDecompressStream` + `Position:=0`，except → `Result := nil`） |
| 74-81 | 函数 | `ReadMemory(streamUI, var Buffer; Count)`（转发 `TStream.Read`） |
| 83-90 / 92-100 | 过程 | `DxFontAssign` / `GuiFontAssign` |
| 102-112 | 函数 | `ReadGuiFontName(streamUI, AFont)` |
| 114-1550 | 过程 | `LoadComponent(streamUI, Gui, DxControl, GuiVersion)` |
| 1551-1610 | 函数 | `NewDxControl(GuiHeader, AOwner)`（**不**登记指针） |
| 1612-1686 | 函数 | `LoadSubComponent(streamUI, AOwner, ControlAddrList, GuiVersion, sUiName):Integer` |
| 1688-1793 | 函数 | `LoadControlFromStream` |
| **1795-2306** | **注释块** | `(* ... *)` 包住的 `ApplyControlData(DestCtrl, SrcCtrl)`（**未编译**，见 §7 缺陷 2） |
| 2308-2374 | 过程 | `PatchLoadSubComponent` |
| 2376-2467 | 过程 | `PatchLoadControlFromStream` |
| 2469 | — | `end.` |

**常量**：同基础版，无常量声明。

### 2.3 记录定义真源：`DxComponents.pas`（接口段）

`TGui*` 记录在整个仓库里**只有一处声明**（`Source/Client-HGE/DxComponent/DxComponents.pas`，全树只此一份，已用脚本核过）。其接口段类型与行号：

`TSaveUIColor`18 · `TClientVersion`27 · `TReferenceX`28 · `TButtonAnimationShowType`30 · `TGuiType`33 · `TImageType`39 · `TMouseEvents`44 · `TMagicBallType`46 · `TMagicBallValueAlignment`47 · `TProgressValueType`48 · `TDrawAligment`49 · `TLineStyle`51 · `TInValue`52 · `TButtonStyle`53 · `TClickSound`54 · `TAlignEx`88 · `TSelection`90 · `TGuiFileHeader`94 · `TGuiFont`107 · `TGuiCaptionColor`116 · `TGuiImageIndex`123 · `TGuiImageIndex_Form`131 · `TGuiImageIndex_Button`138 · `TGuiHeader`147 · `TGuiHeaderAdd`169 · `TGuiImageForm`180 · `TGuiImageForm_New`187 · `TGuiAnimation`197 · `TGuiImageForm_New2`216 · `TGuiImageForm_New3`233 · `TGuiFormShapeInfo`247 · `TGuiFormShapeInfoArray`259 · `TGuiImageFormShape`261 · `TGuiImageButton`270 · `TGuiImageButton_New2`285 · `TGuiButtonAnimation`304 · `TGuiImageButton_New3`325 · `TGuiTrackBar`346 · `TGuiMainBottomCenter`355 · `TGuiMainBottomForm`388 · `TGuiMainBottomFormAnimation`398 · `TGuiMainBottomForm_New`423 · `TGuiMainBottomForm_New2`437 · `TGuiMagicBall`453 · `TGuiMagicBall2`475 · `TGuiSexPanel`510 · `TGuiGroupAttackProgressSetting`525 · `TGuiGroupAttackProgress`540 · `TGuiImageProgress`546 · `TGuiSwitchButtonSetting`572 · `TGuiSwitchButton`592 · `TGuiLabel`600 · `TGuiLabel_New`614 · `TGuiEdit`629 · `TGuiImageEdit`650 · `TBackgroundImage`679 · `TGuiImageEdit_New`689 · `TGuiImageCheckBox`727 · `TGuiImageGrid`733 · `TGuiMemo`742 · `TGuiMemo_New`772 · `TGuiViewField`809 · `TGuiPopupMenu`816 · `TGuiTabSheet`828 · `TGuiPageControl`840 · `TGuiPageControl_New`856 · `TGuiComboBox`882 · `TGuiLine`894

### 2.4 已覆盖 / 未覆盖行号范围

| 单元 | 已覆盖（1:1 移植） | 未覆盖 | 未覆盖原因 |
|---|---|---|---|
| `LoadDxControl.pas` | **1-1771 的全部可执行语句** | 4、47-50、1641-1643、1729-1731 | `OUTPUT_GUI_READORDER` 开关默认关闭（`{.$DEFINE}`），这些行只在该开关打开时编译；已按"日志钩子"形式保留注释 |
| `LoadDxControl.pas` | 同上 | 41、58-59、181、335、355、393、408、448、465、486、500、522、534、581、716、722、728、912、929、983、1001-1002、1008、1022、1047、1055、1060、1086、1595、1598-1599、1613-1614、1710、1768 | 注释行（无对应代码） |
| `LoadDxControlEx.pas` | **1-1793 与 2308-2469 的全部可执行语句** | **1795-2306** | 整段被 `(* ... *)` 注释掉（`ApplyControlData`），**不参与编译**，故不移植；已在报告 §7 登记 |
| `LoadDxControlEx.pas` | 同上 | 4、52-55、1633-1635、1736-1738、2328-2330、2414-2416、1717-1723 | 编译开关关闭的日志块 / 调试代码块（1717-1723 是 `{ }` 包住的 OutputDebugString 调试段） |

> 覆盖率口径说明：本车道的"已覆盖"= 全部**参与编译**的语句逐行对应到 C#（分支顺序、版本界、赋值顺序、字节消费顺序一致），不是"文件里出现过同名符号"。

---

## 3. 压缩 UI 数据二进制格式（逐字段）

### 3.0 全链路

```
Client.exe 资源段
  RT_RCDATA 类型 'ZDAT'，名字 = 资源名（如 MIR_CONFIG_DLG_UI / MIR_UI / STATE_WIN_UI）
        │  LoadDxControlEx.LoadCompressedUIData(name, 'ZDAT')
        ▼
  zlib 流（2 字节头 + deflate + Adler32；Delphi 侧 Zlibex.ZDecompressStream）
        │
        ▼
  .GUI 字节序列 = TGuiFileHeader + N 个顶层控件（每个 = TGuiHeader + [名称] + [TGuiHeaderAdd] + payload + 尾随串）
        │  LoadDxControlEx.LoadControlFromStream(streamUI, Background, ControlAddrList, sUiName)
        ▼
  TDx* 控件树（按 TGuiHeader.Name 与 ControlAddrList 对号入座）
```

`.GUI` 由 GUI 编辑器 `DxComponent/Main.pas` 的 `SaveToFile` 用 `FileStream.Write(记录, SizeOf(记录))` 直接落盘，即**Delphi x86 内存像**（默认 `{$A8}` 对齐、`{$MINENUMSIZE 1}`）。

**已核实的调用点**（三个 GUI 窗口族全部走本栈，佐证"布局真源"）：

| 调用点 | 资源名 |
|---|---|
| `Client-HGE/GameConfig/GameConfigDlgs.pas:55` | `MIR_CONFIG_DLG_UI` |
| `Client-HGE/GUI/Mir/SerialWindowsDlg.pas:17343` | `MIR_UI` |
| `Client-HGE/GUI/NewStateWin/StateWindows.pas:2138` | `STATE_WIN_UI` |

### 3.1 文件头 `TGuiFileHeader`（**packed**，固定 56 字节）

| 偏移 | 宽度 | 字段 | 类型 | 说明 |
|---|---|---|---|---|
| 0 | 36 | `sDesc` | `string[$23]` | 1 字节长度 + 最多 35 字节 GBK |
| 36 | 1 | `ClientVersion` | `TClientVersion` | 枚举 0..6（cv176..cvMirNewUI205） |
| 37 | 1 | `Reserve` | `Byte` | 编辑器 `FillChar` 置 0 |
| 38 | 2 | `GroupCount` | `Word` | >0 且 `nGuiVersion>=20160409` 时，头后面有 GroupCount 段"组名" |
| 40 | 4 | `nGuiVersion` | `Integer` | **决定 record 布局与是否 DES 加密的唯一版本号** |
| 44 | 4 | `nCount` | `Integer` | 顶层控件数；`LoadControlFromStream` 的返回值 |
| 48 | 8 | `dCreateDate` | `TDateTime` | Double，天数 |

> `packed` ⇒ 无任何对齐填充（已断言 `Align=1`）。

### 3.2 控件头 `TGuiHeader`（固定 40 字节）

| 偏移 | 宽度 | 字段 | 说明 |
|---|---|---|---|
| 0 | 1 | `Gui` | `TGuiType`，见 §4 映射表；`t_None`(0) 与越界值**不建控件** |
| 4 | 4 | `Left` | |
| 8 | 4 | `Top` | |
| 12 | 4 | `Width` | |
| 16 | 4 | `Height` | |
| 20 | 1 | `Enabled` | |
| 21 | 1 | `Visible` | |
| 22 | 1 | `Transparent` | |
| 23 | 1 | `EnableFocus` | |
| 24 | 1 | `Floating` | |
| 25 | 1 | `OwnerMove` | |
| 26 | 1 | `MouseEvents` | `set of TMouseButton` → 1 字节位域（mbLeft=1/mbRight=2/mbMiddle=4） |
| 28 | 4 | `NameLen` | 紧随头之后的**名字字节数**（GBK） |
| 32 | 4 | `Background` | 编辑器里存的是所有者指针，仅编辑期有意义 |
| 36 | 4 | `Count` | 直接子控件数（`LoadSubComponent` 递归次数） |

**`nGuiVersion >= 20170226` 时这 40 字节是 DES-CBC 密文**（`UnitDes`，块大小 BS=20，40 = 2 块，无余数），密钥固定为 12 个字符 `#2#1#6#14#20#3#4#1#6#5#10#9`（`LoadDxControl.pas:1719` / `Ex:1727`）。**只加密头，名称/扩展头/payload 均明文。**

### 3.3 控件头扩展 `TGuiHeaderAdd`（48 字节，仅 `nGuiVersion >= 20160409` 存在）

| 偏移 | 宽度 | 字段 | 说明 |
|---|---|---|---|
| 0 | 4 | `ShowNameLen` | 后面"显示名"的字节数 |
| 4 | 1 | `ReferenceX` | `TReferenceX`（rxLeft/rxCenter/rxRight） |
| 5 | 1 | `AdjustYByHeight` | |
| 6 | 1 | `TopAlignment` | `nGuiVersion <= 20171106` 时会被强制改回 False |
| 7 | 1 | `Reserverd` | **原文笔误**（应为 Reserved），真占 1 字节 |
| 8 | 2 | `HintTextLen` | 提示文本字节数 |
| 10 | 2 | `Reserverd2` | **原文笔误**，真占 2 字节 |
| 12 | 36 | `Reserve` | `array[0..8] of Integer` |
| 48 | — | 之后 | `ShowNameLen` 字节显示名 + `HintTextLen` 字节提示文本 |

### 3.4 记录级字段语义（所有 `TGui*` 共用）

* 全部为 **Delphi 默认对齐**（`{$A8}`）：每个字段对齐到 `min(自身对齐, 8)`，记录整体再对齐到字段最大对齐；`packed` 记录（仅 `TGuiFileHeader`）无填充。
* 枚举 = **1 字节**（`{$MINENUMSIZE 1}`，全部枚举值数 ≤ 256）。
* `set of X`（≤8 元素）= **1 字节**位域：`TFontStyles`（fsBold=1/fsItalic=2/fsUnderline=4/fsStrikeOut=8）、`TMouseEvents`。
* `string[N]`（ShortString）= **1 字节长度 + N 字节 GBK**；长度是**字节**数，超长静默截断（已测：`string[20]` 只能装 10 个汉字）。
* `Boolean` = 1 字节（非 0 即 True）；`Char` = AnsiChar = 1 字节；`TColor` = 4 字节；`TRect` = 4×int = 16 字节；`TDateTime` = Double = 8 字节。
* 记录内嵌记录按**值**内联（如 `TGuiCaptionColor` = 4 × 24 = 96 字节）；固定数组内联（如 `TGuiImageFormShape.ImageIndexs` = 8 × 52 = 416 字节）。

### 3.5 版本 → 记录布局分派表（逐 GUI 标签）

| GUI 标签 | 条件（`nGuiVersion`） | 记录 | SizeOf | 之后追加的串 |
|---|---|---|---|---|
| `t_Form` | `< 20160409` | `TGuiImageForm` | 28 | — |
| | `< 20160514` | `TGuiImageForm_New` | 48 | — |
| | `< 20171106` | `TGuiImageForm_New2` | 208 | — |
| | else | `TGuiImageForm_New3` | 204 | — |
| `t_FormShape` | — | `TGuiImageFormShape` | 444 | — |
| `t_Button` | `< 20171106` | `TGuiImageButton` | 140 | 4 字体名 + `CaptionLen` |
| | `< 20180619` | `TGuiImageButton_New2` | 152 | 4 字体名 + `CaptionLen` |
| | else | `TGuiImageButton_New3` | 188 | 4 字体名 + `CaptionLen` |
| `t_Edit` | — | `TGuiEdit` | 160 | 1 字体名 + `TextLen` |
| `t_ImageEdit` | `< 20160430` | **`TGuiEdit`（复用编辑框布局）** | 160 | 1 字体名 + `TextLen` |
| | `< 20190724` | `TGuiImageEdit` | 220 | 2 字体名 + `TextLen` + `HintTextLen` |
| | else | `TGuiImageEdit_New` | 280 | 2 字体名 + `TextLen` + `HintTextLen` |
| `t_Label` | `< 20160409` | `TGuiLabel` | 216 | 4 字体名 + `CaptionLen` |
| | else | `TGuiLabel_New` | 216 | 4 字体名 + `CaptionLen` |
| `t_Grid` | — | `TGuiImageGrid` | 20 | — |
| `t_ScrollBox` `t_ChatMemo` `t_ListView` `t_TreeView` | `< 20160508` | `TGuiMemo` | 168 | ListView 追加 `ColCount`×TRect + `ColCount`×`TGuiViewField`(+4 字体名+标题) |
| | else | `TGuiMemo_New` | 224 | ChatMemo 追加 `FontLen`；ListView 追加同上 |
| `t_PopupMenu` | — | `TGuiPopupMenu` | 216 | 4 字体名 + `ItemTextLen` |
| `t_PageControl` | `< 20160514` | `TGuiPageControl` | 48 | — |
| | else | `TGuiPageControl_New` | 144 | — |
| `t_ComboBox` | — | `TGuiComboBox` | 428 | **8** 字体名（内嵌 PopupMenu 4 + 自身 TextColor 4）+ `TextLen` + `ItemLen` |
| `t_TabSheet` | — | `TGuiTabSheet` | 136 | 4 字体名 + `CaptionLen` |
| `t_Line` | — | `TGuiLine` | 100 | — |
| `t_TrackBar` | — | `TGuiTrackBar` | 56 | — |
| `t_MainBottomForm` | `< 20190729` | `TGuiMainBottomForm` | 132 | — |
| | `< 20211120` | `TGuiMainBottomForm_New` | 340 | — |
| | else | `TGuiMainBottomForm_New2` | 348 | — |
| `t_MagicBall` | `< 20160818` | `TGuiMagicBall` | 80 | — |
| | else | `TGuiMagicBall2` | 116 | — |
| `t_SexPanel` | — | `TGuiSexPanel` | 56 | — |
| `t_GroupAttackProgress` | — | `TGuiGroupAttackProgress` | 204 | — |
| `t_ImageProgress` | — | `TGuiImageProgress` | 236 | 1 字体名 |
| `t_SwitchButton` | — | `TGuiSwitchButton` | 404 | `CloseSetting.CaptionLen` + `OpenSetting.CaptionLen` |
| `t_None` / 越界 | — | **无** | 0 | 不消费任何字节（见 §7 缺陷 6） |

> `TImageType`（图库编号，1 字节）与 `TClientVersion` 的取值顺序见 `DxComponents.pas:27,39`；`TGuiImageIndex.Image` 就是 `TImageType`。

### 3.6 完整偏移表（52 条，**由 `GenGuiRecords.ps1` 从原文抽取并计算，禁止手工转录**）

格式：`类型;SizeOf;对齐;字段@偏移+宽度 ...`。此表与
`LoadDxRecordLayoutTests.Full_Offset_Table_Matches_Source_Derived_Audit` 逐行锁定。

```
TBackgroundImage;16;4;ImageType@0+1 BlendMode@1+1 OutsideAreaDraw@2+1 Reserve@3+1 ImageIndex@4+4 OffsetX@8+4 OffsetY@12+4
TGuiAnimation;32;4;ImageType@0+1 DrawBeforeDef@1+1 Reserve@2+2 StartIndex@4+4 EndIndex@8+4 FrameTime@12+4 PlayCount@16+4 OffsetX@20+4 OffsetY@24+4 UseImageOffset@28+1 OutsideAreaDraw@29+1 Draw@30+1 BlendDraw@31+1
TGuiButtonAnimation;36;4;ImageType@0+1 DrawBeforeDef@1+1 Reserve@2+2 ShowType@4+1 StartIndex@8+4 EndIndex@12+4 FrameTime@16+4 PlayCount@20+4 OffsetX@24+4 OffsetY@28+4 UseImageOffset@32+1 OutsideAreaDraw@33+1 Draw@34+1 BlendDraw@35+1
TGuiCaptionColor;96;4;Up@0+24 Hot@24+24 Down@48+24 Disabled@72+24
TGuiComboBox;428;4;GuiPopupMenu@0+216 DrawBorder@216+1 ButtonColor@220+4 BackgroundColor@224+4 TextColor@228+96 BorderColor@324+96 TextLen@420+4 ItemLen@424+4
TGuiEdit;160;4;DrawBorder@0+1 SelectedColor@4+4 SelBackColor@8+4 SelFontColor@12+4 BackgroundColor@16+4 FontColor@20+24 BorderColor@44+96 ReadOnly@140+1 MaxLength@144+4 InValue@148+1 PasswordChar@149+1 AllowSelect@150+1 AllowPaste@151+1 TabOrder@152+4 TextLen@156+4
TGuiFileHeader;56;1;sDesc@0+36 ClientVersion@36+1 Reserve@37+1 GroupCount@38+2 nGuiVersion@40+4 nCount@44+4 dCreateDate@48+8
TGuiFont;24;4;Color@0+4 BColor@4+4 Style@8+1 Size@12+4 Bold@16+1 NameLen@20+4
TGuiFormShapeInfo;52;4;ImageType@0+1 ImageIndex@4+4 Draw@8+1 Stretch@9+1 Center@10+1 BlendMode@12+4 Align@16+1 SrcRect@20+16 DestRect@36+16
TGuiGroupAttackProgress;204;4;ProgressAlignment@0+1 Settings@4+144 Reserve@148+56
TGuiGroupAttackProgressSetting;48;4;ImageType@0+1 Background@4+4 Progress@8+4 FlashStart@12+4 FlashEnd@16+4 FlashInterval@20+4 OffsetX1@24+4 OffsetY1@28+4 OffsetX2@32+4 OffsetY2@36+4 OffsetX3@40+4 OffsetY3@44+4
TGuiHeader;40;4;Gui@0+1 Left@4+4 Top@8+4 Width@12+4 Height@16+4 Enabled@20+1 Visible@21+1 Transparent@22+1 EnableFocus@23+1 Floating@24+1 OwnerMove@25+1 MouseEvents@26+1 NameLen@28+4 Background@32+4 Count@36+4
TGuiHeaderAdd;48;4;ShowNameLen@0+4 ReferenceX@4+1 AdjustYByHeight@5+1 TopAlignment@6+1 Reserverd@7+1 HintTextLen@8+2 Reserverd2@10+2 Reserve@12+36
TGuiImageButton;140;4;Alignment@0+1 ImageIndex@4+20 AutoSize@24+1 CaptionColor@28+96 Checked@124+1 ClickCount@125+1 Style@126+1 CaptionDownOffsetX@128+4 CaptionDownOffsetY@132+4 CaptionLen@136+4
TGuiImageButton_New2;152;4;Alignment@0+1 ImageIndex@4+24 AutoSize@28+1 CaptionColor@32+96 Checked@128+1 ClickCount@129+1 Style@130+1 CaptionDownOffsetX@132+4 CaptionDownOffsetY@136+4 CaptionOffsetX@140+4 CaptionOffsetY@144+4 CaptionLen@148+4
TGuiImageButton_New3;188;4;Alignment@0+1 ImageIndex@4+24 AutoSize@28+1 CaptionColor@32+96 Checked@128+1 ClickCount@129+1 Style@130+1 CaptionDownOffsetX@132+4 CaptionDownOffsetY@136+4 CaptionOffsetX@140+4 CaptionOffsetY@144+4 Animation@148+36 CaptionLen@184+4
TGuiImageCheckBox;144;4;Button@0+140 Checked@140+1
TGuiImageEdit;220;4;DrawBorder@0+1 SelectedColor@4+4 SelBackColor@8+4 SelFontColor@12+4 BackgroundColor@16+4 DisableBackgroundColor@20+4 HintTextFont@24+24 HintTextAlignment@48+1 FontColor@52+24 BorderColor@76+96 ReadOnly@172+1 MaxLength@176+4 InValue@180+1 PasswordChar@181+1 AllowSelect@182+1 AllowPaste@183+1 TabOrder@184+4 TextLen@188+4 HintTextLen@192+4 Reserve@196+24
TGuiImageEdit_New;280;4;DrawBorder@0+1 SelectedColor@4+4 SelBackColor@8+4 SelFontColor@12+4 BackgroundColor@16+4 BackgroundColorAlpha@20+1 BackgroundImage@24+16 DisableHideCtrl@40+1 DisableBackgroundTransparent@41+1 DisableBackgroundColor@44+4 DisableBackgroundAlpha@48+1 DisableBackgroundImage@52+16 HintTextFont@68+24 HintTextAlignment@92+1 FontColor@96+24 BorderColor@120+96 ReadOnly@216+1 MaxLength@220+4 InValue@224+1 PasswordChar@225+1 AllowSelect@226+1 AllowPaste@227+1 TabOrder@228+4 TextLen@232+4 HintTextLen@236+4 Reserve@240+40
TGuiImageForm;28;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1
TGuiImageForm_New;48;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1 BackgroundAlpha@25+1 BackgroundColor@28+4 Reserve@32+16
TGuiImageForm_New2;208;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1 BackgroundAlpha@25+1 BackgroundColor@28+4 Animation1@32+32 Animation2@64+32 Animation3@96+32 ImageOffsetX@128+4 ImageOffsetY@132+4 Reserve@136+72
TGuiImageForm_New3;204;4;AutoSize@0+1 ImageIndex@4+16 Center@20+1 BackgroundAlpha@21+1 BackgroundColor@24+4 Animation1@28+32 Animation2@60+32 Animation3@92+32 Reserve@124+80
TGuiImageFormShape;444;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1 ImageIndexs@28+416
TGuiImageGrid;20;4;ColCount@0+4 RowCount@4+4 ColWidth@8+4 RowHeight@12+4 ViewTopLine@16+4
TGuiImageIndex;20;4;Image@0+1 Up@4+4 Hot@8+4 Down@12+4 Disabled@16+4
TGuiImageIndex_Button;24;4;Image@0+1 Up@4+4 Hot@8+4 Down@12+4 Disabled@16+4 Checked@20+4
TGuiImageIndex_Form;16;4;Image@0+1 Up@4+4 OffsetX@8+4 OffsetY@12+4
TGuiImageProgress;236;4;AutoSize@0+1 ImageType@1+1 ImageBG@4+4 ImageProgress@8+4 ImageProgressX@12+4 ImageProgressY@16+4 ValueType@20+1 ValueSplite@21+21 ValueAlignment@42+1 ValuePrefix@43+61 ValueSuffix@104+61 Max@168+4 Min@172+4 Value@176+4 Font@180+24 Reserve@204+32
TGuiLabel;216;4;AutoSize@0+1 DrawBorder@1+1 BackgroundColor@4+4 BorderColor@8+96 CaptionColor@104+96 ClickCount@200+1 Style@201+1 CaptionDownOffsetX@204+4 CaptionDownOffsetY@208+4 CaptionLen@212+4
TGuiLabel_New;216;4;AutoSize@0+1 Alignment@1+1 DrawBorder@2+1 BackgroundColor@4+4 BorderColor@8+96 CaptionColor@104+96 ClickCount@200+1 Style@201+1 CaptionDownOffsetX@204+4 CaptionDownOffsetY@208+4 CaptionLen@212+4
TGuiLine;100;4;LineColor@0+96 LineStyle@96+1
TGuiMagicBall;80;4;BallType@0+1 ValueAlignment@1+1 Overall_ImageType@2+1 Overall_EmptyHPMP@4+4 Overall_FullHPMP@8+4 Overall_EmptyHP@12+4 Overall_FullHP@16+4 Overall_Splite@20+4 Overall_MiddleZoneWidth@24+4 Alone_ImageType@28+1 Alone_Empty@32+4 Alone_Full@36+4 Reserve@40+40
TGuiMagicBall2;116;4;BallType@0+1 ValueAlignment@1+1 Overall_ImageType@2+1 Overall_EmptyHPMP@4+4 Overall_FullHPMP@8+4 Overall_EmptyHP@12+4 Overall_FullHP@16+4 Overall_Splite@20+4 Overall_MiddleZoneWidth@24+4 Overall_EffectDrawBlend@28+1 Overall_EffectImageType@29+1 Overall_EffectHPMPStart@32+4 Overall_EffectHPStart@36+4 Overall_EffectImageCount@40+4 Overall_EffectPlayInterval@44+4 Alone_ImageType@48+1 Alone_Empty@52+4 Alone_Full@56+4 Alone_EffectDrawBlend@60+1 Alone_EffectImageType@61+1 Alone_EffectStart@64+4 Alone_EffectImageCount@68+4 Alone_EffectPlayInterval@72+4 Reserve@76+40
TGuiMainBottomCenter;100;4;Height@0+4 MinHeight@4+4 MaxHeight@8+4 OffsetLeft@12+4 OffsetRight@16+4 DragHeightOffsetY@20+4 DragHeightSize@24+4 AutoStretchSize@28+1 StretchImageFillCenterAlpha@29+1 StretchImageFillCenterColor@32+4 StretchImageFillCenterExpandHorz@36+4 StretchImageFillCenterExpandVert@40+4 StretchImageType@44+1 StretchImageUpLeft@48+4 StretchImageUp@52+4 StretchImageUpRight@56+4 StretchImageLeft@60+4 StretchImageRight@64+4 StretchImageDownLeft@68+4 StretchImageDown@72+4 StretchImageDownRight@76+4 FillImageType@80+1 FillImageIndex@84+4 Reserve@88+12
TGuiMainBottomForm;132;4;LeftImageType@0+1 LeftImageIndex@4+4 RightImageType@8+1 RightImageIndex@12+4 Center@16+100 Reserve@116+16
TGuiMainBottomFormAnimation;52;4;ImageType@0+1 DrawBeforeDef@1+1 Reserve@2+2 StartIndex@4+4 EndIndex@8+4 FrameTime@12+4 PlayCount@16+4 OffsetX@20+4 OffsetY@24+4 UseImageOffset@28+1 OutsideAreaDraw@29+1 Draw@30+1 BlendDraw@31+1 HorzAlignment@32+1 VertAlignment@33+1 AdjustYByHeight@34+1 Reseved@35+17
TGuiMainBottomForm_New;340;4;LeftImageType@0+1 LeftImageIndex@4+4 RightImageType@8+1 RightImageIndex@12+4 Center@16+100 Animation1@116+52 Animation2@168+52 Animation3@220+52 Animation4@272+52 Reserve@324+16
TGuiMainBottomForm_New2;348;4;LeftImageType@0+1 LeftImageIndex@4+4 RightImageType@8+1 RightImageIndex@12+4 BottomImageType@16+1 BottomImageIndex@20+4 Center@24+100 Animation1@124+52 Animation2@176+52 Animation3@228+52 Animation4@280+52 Reserve@332+16
TGuiMemo;168;4;ImageIndex@0+20 ScrollImageIndex@20+20 PrevImageIndex@40+20 NextImageIndex@60+20 BarImageIndex@80+20 ShowScroll@100+1 ItemHeight@104+4 ItemIndex@108+4 ScrollBars@112+1 ScrollSize@116+4 ExpandSize@120+4 Position@124+4 VisibleItemCount@128+4 ShowButton@132+1 OffSetX@136+4 OffSetY@140+4 ColCount@144+4 ShowItemCount@148+4 ShowGridLine@152+1 GridLineColor@156+4 CheckItemControlSize@160+1 Reserve@164+4
TGuiMemo_New;224;4;ImageIndex@0+20 ScrollImageIndex@20+20 PrevImageIndex@40+20 NextImageIndex@60+20 BarImageIndex@80+20 ShowScroll@100+1 ItemHeight@104+4 ItemIndex@108+4 ScrollBars@112+1 ScrollSize@116+4 ExpandSize@120+4 Position@124+4 VisibleItemCount@128+4 ShowButton@132+1 OffSetX@136+4 OffSetY@140+4 ColCount@144+4 ShowItemCount@148+4 ShowGridLine@152+1 GridLineColor@156+4 CheckItemControlSize@160+1 BackGroupColor@164+4 FontBackTransparent@168+1 FontLen@172+4 FontSize@176+4 FontStroke@180+1 Reserve@184+40
TGuiPageControl;48;4;ShowButton@0+1 ClientLeft@4+4 ClientTop@8+4 ClientWidth@12+4 ClientHeight@16+4 TabPosition@20+1 PageCount@24+4 ActivePageIndex@28+4 ButtonWidth@32+4 ButtonHeight@36+4 OffSetX@40+4 OffSetY@44+4
TGuiPageControl_New;144;4;ShowButton@0+1 ClientLeft@4+4 ClientTop@8+4 ClientWidth@12+4 ClientHeight@16+4 TabPosition@20+1 PageCount@24+4 ActivePageIndex@28+4 ButtonWidth@32+4 ButtonHeight@36+4 OffSetX@40+4 OffSetY@44+4 CaptionOffsetX@48+4 CaptionOffsetY@52+4 DownCaptionOffsetX@56+4 DownCaptionOffsetY@60+4 ReverseDrawButton@64+1 Reserve1@65+3 Reserve2@68+76
TGuiPopupMenu;216;4;DrawBorder@0+1 SelectColor@4+4 BackgroundColor@8+4 ItemColor@12+96 BorderColor@108+96 ItemHeight@204+4 ItemIndex@208+4 ItemTextLen@212+4
TGuiSexPanel;56;4;IsMale@0+1 UseSettign2@1+1 ImageType@2+1 Male@4+4 Female@8+4 ImageType2@12+1 Male2@16+4 Female2@20+4 Reserve@24+32
TGuiSwitchButton;404;4;AutoSize@0+1 CloseSetting@4+184 OpenSetting@188+184 Reserve@372+32
TGuiSwitchButtonSetting;184;4;ImageIndex@0+20 CaptionColor@20+96 ClickSound@116+1 Alignment@117+1 CaptionOffsetX@120+4 CaptionOffsetY@124+4 CaptionDownOffsetX@128+4 CaptionDownOffsetY@132+4 ButtonDownOffsetX@136+4 ButtonDownOffsetY@140+4 DrawAligment@144+1 CaptionLen@148+4 Reserve@152+32
TGuiTabSheet;136;4;OffSetX@0+4 OffSetY@4+4 HideTable@8+1 Reserve1@9+3 Reserve2@12+4 CaptionColor@16+96 ImageIndex@112+20 CaptionLen@132+4
TGuiTrackBar;56;4;ImageIndex@0+20 SliderIndex@20+20 AutoSize@40+1 Min@44+4 Max@48+4 Position@52+4
TGuiViewField;104;4;Color@0+96 Alignment@96+1 CaptionLen@100+4
TSaveUIColor;16;4;Up@0+4 Hot@4+4 Down@8+4 Disabled@12+4
TSelection;8;4;StartPos@0+4 EndPos@4+4
```

### 3.7 字体名与标题串的追加规则（顺序不可换）

记录里只放 `NameLen`；**名字字节在整条记录之后按固定次序连续排列**：

| GUI | 字体名次序 |
|---|---|
| `t_Button` / `t_Label` | `CaptionColor.Up` → `Hot` → `Down` → `Disabled`，随后 `CaptionLen` 字节标题 |
| `t_Edit`（含 `t_ImageEdit` 的 `<20160430` 分支） | `FontColor` 一个（**BorderColor 的 4 个名字不写也不读**），随后 `TextLen` |
| `t_ImageEdit`（≥20160430） | `FontColor` → `HintTextFont`，随后 `TextLen` + `HintTextLen` |
| `t_PopupMenu` | `ItemColor.Up/Hot/Down/Disabled`，随后 `ItemTextLen` |
| `t_ComboBox` | `PopupMenu.ItemColor.Up/Hot/Down/Disabled` → `TextColor.Up/Hot/Down/Disabled`，随后 `TextLen` + `ItemLen` |
| `t_TabSheet` | `CaptionColor.Up/Hot/Down/Disabled`，随后 `CaptionLen` |
| `t_ListView` 的每个列 | `Fields[i].Color.Up/Hot/Down/Disabled`，随后该列 `CaptionLen` |
| `t_ImageProgress` | `Font` 一个 |
| `t_SwitchButton` | 两个设置的标题在**两组设置字段全部赋完之后**才读：先 `CloseSetting.CaptionLen`，再 `OpenSetting.CaptionLen` |
| 其余 | 无尾随串 |

---

## 4. `TGui*` → 控件映射表（后续窗口批次的基础设施）

| GUI 标签 | 值 | 记录（版本分派见 §3.5） | 原文控件类（`NewDxControl`） | C# 落点 | 接缝状态 |
|---|---|---|---|---|---|
| `t_None` | 0 | — | （nil） | — | 不建控件 |
| `t_Form` | 1 | `TGuiImageForm*` | `TDxImageForm` | `GXX.Client.LoadDx.TDxImageForm` | **接缝**（待 `DxImageForm.pas`） |
| `t_Button` | 2 | `TGuiImageButton*` | `TDxImageButton` | `GXX.Client.DxComponent.TDxImageButton` | **既有实现** |
| `t_Edit` | 3 | `TGuiEdit` | `TDxEdit` | `LoadDx.TDxEdit` | **接缝**（待 `DxEdit.pas`） |
| `t_Label` | 4 | `TGuiLabel` / `_New` | `TDxLabel` | `GXX.Client.DxComponent.TDxLabel` | **既有实现** |
| `t_Grid` | 5 | `TGuiImageGrid` | `TDxImageGrid` | `LoadDx.TDxImageGrid` | **接缝**（待 `DxImageGrid.pas`） |
| `t_ScrollBox` | 6 | `TGuiMemo` / `_New` | `TDxScrollBox` | `LoadDx.TDxScrollBox` | **接缝**（待 `DxMemo.pas`） |
| `t_ChatMemo` | 7 | 同上 | `TDxChatMemo` | `LoadDx.TDxChatMemo` | **接缝** |
| `t_PopupMenu` | 8 | `TGuiPopupMenu` | `TDxPopupMenu` | `LoadDx.TDxPopupMenu` | **接缝**（待 `DxPopupMenu.pas`） |
| `t_ComboBox` | 9 | `TGuiComboBox` | `TDxComboBox` | `LoadDx.TDxComboBox` | **接缝**（待 `DxComboBox.pas`） |
| `t_PageControl` | 10 | `TGuiPageControl` / `_New` | `TDxPageControl` | `LoadDx.TDxPageControl` | **接缝**（待 `DxPageControl.pas`） |
| `t_TabSheet` | 11 | `TGuiTabSheet` | `TDxTabSheet` | `LoadDx.TDxTabSheet` | **接缝** |
| `t_TreeView` | 12 | `TGuiMemo` / `_New` | `TDxTreeView` | `LoadDx.TDxTreeView` | **接缝** |
| `t_ListView` | 13 | `TGuiMemo` / `_New` | `TDxListView` | `LoadDx.TDxListView` | **接缝** |
| `t_Line` | 14 | `TGuiLine` | `TDxLine` | `GXX.Client.DxComponent.TDxLine` | **既有实现** |
| `t_FormShape` | 15 | `TGuiImageFormShape` | `TDxImageFormShape` | `LoadDx.TDxImageFormShape` | **接缝** |
| `t_ImageEdit` | 16 | `TGuiEdit`/`TGuiImageEdit`/`_New` | `TDxImageEdit` | `LoadDx.TDxImageEdit` | **接缝**（待 `DxImageEdit.pas`） |
| `t_TrackBar` | 17 | `TGuiTrackBar` | `TDXTrackBar` | `GXX.Client.DxComponent.TDXTrackBar` | **既有实现** |
| `t_MainBottomForm` | 18 | `TGuiMainBottomForm*` | `TDxMainBottomForm` | `LoadDx.TDxMainBottomForm` | **接缝** |
| `t_MagicBall` | 19 | `TGuiMagicBall` / `2` | `TDxMagicBall` | `LoadDx.TDxMagicBall` | **接缝** |
| `t_SexPanel` | 20 | `TGuiSexPanel` | `TDxSexPanel` | `LoadDx.TDxSexPanel` | **接缝** |
| `t_GroupAttackProgress` | 21 | `TGuiGroupAttackProgress` | `TDxGroupAttackProgress` | `LoadDx.TDxGroupAttackProgress` | **接缝** |
| `t_ImageProgress` | 22 | `TGuiImageProgress` | `TDxImageProgress` | `GXX.Client.DxComponent.TDxImageProgress` | **既有实现** |
| `t_SwitchButton` | 23 | `TGuiSwitchButton` | `TDxSwitchButton` | `LoadDx.TDxSwitchButton` | **接缝** |

**既有实现（5 种）** 引用 P1 车道5 的 `src/GXX.Client/DxComponent/**`，本车道**不重新定义**（台账 §9.3 教训）；
**接缝（18 种）** 全部继承 `GXX.Client.DxComponent.TDxControl`，只声明 `LoadComponent` 真会写的成员，每处都标了
`// 接缝：待 <单元名> 移植后接入`。

### 4.1 属性名映射（原文 → 既有接缝）

| 原文（`DxControls.pas`） | 既有 C# | 备注 |
|---|---|---|
| `TDxImageButton.ClickCount:TClickSound` | `TDxImageButton.ClickSound` | 类型名同名 `TClickSound` 但它是 `TDxImageButton` 的**嵌套枚举** |
| `TDxImageButton.ButtonChecked` | `TDxControl.Checked` | |
| `TDxImageButton.Animation` | `TDxControlExtras.Of(c).Animation` | 既有类缺该成员 → 挂附带对象 |
| `TDxControl.GuiType` | `TDxControlExtras.Get/SetGuiType` | 基类未定义；`TDxImageButton` 上同步 |
| `TDxControl.Hint` | `TDxControl.HintText` | WinForms `Control.Hint` 语义不同，不混用 |
| `TDxControl.Owner`（TabSheet 用） | `TDxControl.DxOwner` | 既有接缝的父链字段 |
| `TAlignment` | `TDxAlignment` | 值序相同 |
| `TFontStyles` | `TDxFont.StyleSet`（字符串集合） | 位序 fsBold=1/fsItalic=2/fsUnderline=4/fsStrikeOut=8 |

---

## 5. 两个单元的行为差异（全部写成差异断言）

| # | 主题 | `LoadDxControl.pas`（内存版） | `LoadDxControlEx.pas`（流版） | 测试 |
|---|---|---|---|---|
| 1 | 读字节 | 单元级 `MemoryData/Size/Position` + 裸指针 `Move`（短读返回 nRem） | `TStream.Read` 转发 | `LoadDxReaderTests` 两套各有 4-8 例 |
| 2 | 控件登记 | `PControlAddress^` 线性数组 + 指针前移（`:1594-1596`），**新建即登记** | `THashedStringList` 按名字查表（`:1740-1749`），**读完才 Free 未命中的控件** | `NewDxControl_Registers_Linearly_Through_PControlAddress` / `LoadControlFromStream_Unregistered_Name_Is_Disposed_...` |
| 3 | `LoadSubComponent` 入口守卫 | `if DxControl <> nil then`（`:1636`）→ **`NameLen = 0` 也读 GuiHeaderAdd + payload** | `if (DxControl <> nil) and (GuiHeader.NameLen > 0)`（`:1629`）→ **一个字节都不多消费** | `LoadSubComponent_With_NameLen_Zero_Still_Reads_Add_And_Component` ↔ `NameLen_Zero_Skips_GuiHeaderAdd_And_Payload_Entirely` |
| 4 | `HintText` 版本守卫 | 只判 `>= 20160409`（`:1745-1749`） | 追加 `>= 20180619`（`:1763`，注释"只有20180619之后的版本才有HintText"） | `HintText_Is_Read_From_20160409_Onwards_In_This_Unit` ↔ `HintText_Is_Not_Read_At_20160409` |
| 5 | 返回值 | `Result := GuiHeader.Count + Σ 子控件返回值` | 顶层 `Result := FileHeader.nCount`（**不累加**子控件）；子控件返回 `Count + Σ` | `LoadSubComponent_Returns_...` / `Ex_SubComponent_Returns_Count_Plus_Children` |
| 6 | Patch 版 | 无 | `Patch*`：**先递归后登记**，槽位非空**不覆盖**，未命中 Free | `Normal_Version_Overwrites_An_Occupied_Slot_While_Patch_Does_Not` |

> `LoadComponent` 主体（1437 行）**逐行等价**：把两边的 `streamUI,` 参数抹掉后做 A/B diff，差异只有 8 处
> `end else` 换行方式 + 1 处多余空行，**无语义差异**。故本车道只保留一份实现（`GuiComponentLoader`），
> 分支顺序、版本界、字段赋值顺序与原文一致。

---

## 6. 测试与门禁

### 6.1 门禁结果（本工作树）

```
dotnet build GXX.slnx -c Debug --nologo                 → 0 error（71 warning，全为既有文件；本车道文件 0 warning）
dotnet test tests\GXX.Client.Tests\... -c Debug         → 2425 passed / 0 failed / 0 skipped
```

* 本车道开工前基线：**2278**（`--list-tests` 全量差分；任务书写的 2308 是更早快照）
* 本车道新增：**147** 个测试方法（`GXX.Client.Tests.LoadDx*`）
* 分类：记录布局 20 · 读取原语与字体 22+9 · 基础版加载器 58 · Ex 版加载器 25 · 资源与加密 13

### 6.2 测试策略

* **全部用合成字节**：仓库里没有随客户端发布的那批 `ZDAT` `.GUI` 资源（它们编译在 `Client.exe` 资源段），
  `GuiTest` 用 `GuiRecords` 的 `WriteAt` 拼出与读取端**互为镜像**的字节序列，因此"格式"这一层是自洽闭环。
* **整表回读比对**：`Full_Offset_Table_Matches_Source_Derived_Audit` 把 52 条记录的
  `SizeOf/对齐/每个字段的偏移与宽度`（= `GenGuiRecords.ps1` 的 AUDIT 输出）逐条与运行时反射出的
  `DelphiRecordLayout` 比对 —— 任何人改生成器、改记录声明或手改 `GuiRecords.g.cs` 都会立刻红灯。
* **异常路径**：标签未知 / `t_None` / 记录数不足（短读零填充）/ 头长度不足（break）/ 资源缺失 /
  zlib 损坏 / 资源类型不匹配 / DES 密钥不符 / 名字表为空 / 越界地址槽。
* **每个分支都断言"消耗的字节数"**：`OneLength(name, version, payloadLen)` 算出期望，避免"字段读对了但错位"这类假绿。

---

## 7. 发现的原文缺陷 / 易错点

### 缺陷

1. **`LoadDxControlEx.pas:2329` 引用了不存在的参数**：`PatchLoadSubComponent` 的签名里没有 `sUiName`，
   但 `{$IFDEF OUTPUT_GUI_READORDER}` 块里写了 `Format('[%s] %s', [sUiName, sText])`。
   默认开关关闭所以能编译；**一旦打开该开关，本单元编译不过**。（`LoadDxControl.pas` 侧同一块有参数，正常。）
2. **`LoadDxControlEx.pas:1795-2306` 的 `ApplyControlData` 整段被 `(* *)` 注释**（512 行）：
   既不参与编译，内容也与 `LoadComponent` 不一致（例如 `TDxPopupMenu.Items.Text` 的拷贝在 2021-06-24 被注释掉，
   注释写着"修复升级菜单后文字还是之前的"）。本车道**不移植**，仅登记。
3. **GUI 编辑器 `DxComponent/Main.pas` 与 `DxComponents.pas` 版本不一致，不能作为格式参照**：
   `Main.pas:1733-1734` 给 `TGuiTabSheet` 赋 `CaptionDownOffsetX/Y`，而 `TGuiTabSheet`（`DxComponents.pas:828-837`）
   **没有这两个字段**（对应位置是 `Reserve1:array[0..2] of Byte` + `Reserve2:Integer`）。⇒ 该编辑器对当前
   `DxComponents.pas` **无法编译**，它与发布版 `.GUI` 资源的格式不一定是同一版。**格式真源只能取
   `LoadDxControl*.pas` + `DxComponents.pas` 这一对**（本车道即按此实现）。
4. **编辑器不加密头，而加载器无条件解密**：`Main.pas` 的 `SaveToFile` 直接 `Write(GuiHeader, SizeOf)`，
   没有 `EncryptDes`；但 `nGuiVersion >= 20170226` 时加载器**一定**执行 `DecryptDes`。
   今天不出问题只因为 `DxComponent/Share.pas:8` 的 `PROVERSION = 20100101`（< 20170226）；
   一旦把该常量提到 20170226 以上，编辑器存出的文件客户端读不了。⇒ 说明随客户端发布的 .GUI 资源
   不是这个编辑器版本产出的（或产出后另有加密步骤，仓库内未见）。
5. **`LoadComponent` 内所有记录读取都不检查返回值**（`ReadMemory(x, SizeOf(TXxx))` 裸调用）：
   短读时 Delphi 局部记录变量尾部是**未初始化栈残留**，行为不可预测。托管侧统一以 0 填充并在
   `GuiReaderExtensions.ReadRecord` 注明（`// 原文如此`）。
6. **未知标签 / `t_None` 会让顶层循环静默错位**：`case Gui of` 无 `else` ⇒ 不消费 payload；
   而顶层 `while` 只按 `MemoryPosition >= MemorySize` 或"读满 40 字节失败"退出，于是剩余字节被当作
   下一个控件头重解析。真实 `.GUI` 不含 `t_None` 所以没暴露；托管侧保留同样行为并加了显式测试。
7. **`PControlAddress` 线性写不判越界**（`:1594-1596`）：数组不够长就直接写出界（原生内存越界）。
   托管侧 `TDxControlAddressList.Store` 越界返回 `false` 并继续推进游标（差异，已注释+测试）。
8. **`TDxImageFormShape.ImageCount` 无法从两个源单元确定**：记录数组是 `array[0..8 - 1]`（8 项），
   本车道据此取 `ImageCount = 8`；真实值在未移植的 `DxImageForm.pas` 里。若该值为 9，则两者都会越界读 ——
   属**待核对项**（已在接缝注释中登记）。
9. **`TGuiHeader.MouseEvents` 是 1 字节的 `set of TMouseButton`**：这是格式的隐性假设，
   若 VCL 侧 `TMouseButton` 将来超过 8 个成员，该字段会变成 2 字节并**整体错位**（潜在脆弱点）。
10. **`LoadCompressedUIData` 的调用方不检查 nil**：`GameConfigDlgs.pas:55` 等处注释写着"必须存在，否则报错"，
    但函数在 except 里返回 nil 且调用方直接 `LoadControlFromStream(msDefaultUI, ...)`。
    托管侧保持同样的宽松，异常表现为 `ArgumentNullException`（构造 `TDxStreamReader` 时）。

### 易错点（"看起来一样实则不同"）

1. **版本界的 `<` / `<=` 不统一**：`TopAlignment := False` 用 `<= 20171106`（`:1665/1753`），
   而按钮/表单布局分派用 `< 20171106`；`HintText` 在 Ex 用 `>= 20180619`、在基础版**没有**该条件。
2. **`t_ImageEdit` 在 `< 20160430` 复用 `TGuiEdit` 布局**（`LoadDxControl.pas:497`），不是 `TGuiImageEdit`。
3. **`t_TrackBar` 是唯一写成 `SizeOf(GuiTrackBar)`（变量）而非 `SizeOf(TGuiTrackBar)` 的地方**（`:1084`），
   结果相同但容易被"精确搬运"改错。
4. **`TGuiLabel` 与 `TGuiLabel_New` `SizeOf` 都是 216**：`_New` 多出的 `Alignment` 恰好吃掉填充；
   只比对 `SizeOf` 的测试会漏掉它（已用具体偏移断言锁住）。
5. **`TGuiImageFormShape` 的 `array[0..8 - 1]` 是 8 项**（原文写成算式），不是九宫格的 9 项。
6. **原文笔误字段名照抄**：`TGuiHeaderAdd.Reserverd` / `Reserverd2`、`TGuiSexPanel.UseSettign2`、
   `TGuiSwitchButtonSetting.DrawAligment`（少一个 n）、`TGuiMemo_New.BackGroupColor`（少一个 d）。
7. **`TGuiMemo_New.BackGroupColor` 是 `Integer` 不是 `TColor`**（后者也是 4 字节，但语义不同）。
8. **ShortString 长度按字节**：`string[20]` 只能装 10 个汉字；`GuiFontAssign` 的 `NameLen` 也要按 GBK 字节数算。
9. **字体名为空（`NameLen = 0`）时一个字节都不消费** —— 没写名字的记录后面直接跟标题串。
10. **`t_SwitchButton` 的两个标题在两组设置字段之后才读**，顺序 Close → Open。
11. **`t_ComboBox` 要读 8 个字体名**（内嵌 PopupMenu 4 + 自身 TextColor 4），少读 4 个会整体错位。
12. **DES 只加密 40 字节的头**，`TGuiHeaderAdd` 与 payload 是明文；且 40 = 2 × BS(20) 恰好整块，
   没有余数分支（若 `TGuiHeader` 尺寸变成非 20 的倍数，`UnitDes.DecryptCBC` 会走余数路径）。

---

## 8. 接缝与未完成

### 8.1 接缝（本车道定义的"最小可用面"）

| 接缝 | 待接入 | 位置 |
|---|---|---|
| 18 种 `TDx*` 控件（Form/FormShape/Edit/ImageEdit/ImageGrid/ScrollControl+4 子类/PopupMenu/ComboBox/PageControl/TabSheet/MainBottomForm/MagicBall/SexPanel/GroupAttackProgress/SwitchButton） | 对应 `DxComponent/*.pas` 各自的完整移植（P2 待认领池第 4 项） | `LoadDx/DxControlSeams.cs` |
| `TDxControlExtra.Animation` | `DxImageButton.pas` 的 `TDxImageButton.Animation` | 同上 |
| `TDxControlExtra.GuiType` | `DxControls.pas` 的 `TDxControl.GuiType`（既有基类未定义） | 同上 |
| `THashedStringList` / `TDxControlRef` | `Common/HashList.pas`（`THashedStringList`）；本车道只用了 `IndexOf`/`Objects[]` | 同上 |
| `IDxGuiResourceProvider` | 资源装载层（`TResourceStream(HInstance, Name, Type)` 的托管等价；`ReadResources` 族移植后接入） | `LoadDx/GuiResource.cs` |
| `TDxFont.Style`（字符串集合） | 车道5 的 `TFontStyles` 接缝形态；本车道做了位↔名双向映射 | `LoadDx/GuiComponentLoader.cs` `DxGuiFonts` |

### 8.2 未完成 / 待办（交回调度会话）

| # | 待办 | 影响 |
|---|---|---|
| 1 | **UI 控件真类**：本车道只给 18 种控件的最小面（无绘制/命中/输入）；等 `DxComponent` 全量后把接缝替换为真类（改动集中在 `DxControlSeams.cs` 一个文件） | 大；P2 待认领池第 4/7 项 |
| 2 | **`TDxImageFormShape.ImageCount` 待核对**（本车道取 8） | 中；影响 `t_FormShape` 的 payload 解析长度 |
| 3 | **真实 `ZDAT` 资源回归**：本车道全部用合成字节；拿到客户端 exe 后应做一次"真资源 → 控件树"的对拍（用 `TDxGuiMemoryResourceProvider` 注入 exe 资源段即可，无需改代码） | 中；最终验收 |
| 4 | **`ApplyControlData`（Ex:1795-2306）**：若后续确认需要"补丁式复制控件数据"，需从注释块恢复并单独移植（512 行） | 小；当前是死代码 |
| 5 | **DES 密钥与 GUIVersion 的配套关系**：编辑部（`Main.pas`）不加密头，与本车道加载端的 `>=20170226 必解密` 冲突（见 §7 缺陷 4），需要权威版编辑器/打包工具确认 | 中；格式风险登记 |
| 6 | **`LoadCompressedUIData` 的 null 返回未被调用方检查**（3 处调用点）——建议后续窗口批次统一加守卫 | 小 |
| 7 | 消费方接入：`GameConfigDlgs.pas:55`、`SerialWindowsDlg.pas:17343`、`StateWindows.pas:2138` 对应的窗口批次应改为 `DxGuiResource.LoadCompressedUIData(...)` + `TDxGuiLoaderControlEx.LoadControlFromStream(...)`（本车道已备好 API 与端到端测试） | 中；属车道1/7/8 的后续工作 |

### 8.3 给后续窗口批次的调用示例

```csharp
// 1) 注入资源提供者（真实运行时由资源装载层实现 IDxGuiResourceProvider）
DxGuiResource.Provider = myResourceProvider;          // GetResource(name, "ZDAT") → 原始压缩字节

// 2) 解压 + 建控件树（对应各窗口单元的 initialization）
using var ui = DxGuiResource.LoadCompressedUIData("STATE_WIN_UI", "ZDAT");
var names = new THashedStringList();
var refMain      = names.Register("MainForm");         // 名字取自 .GUI 记录里的 Name
var refCloseBtn  = names.Register("btnClose");
var loader = new TDxGuiLoaderControlEx(ui);
int topLevelCount = loader.LoadControlFromStream(background: null, names, "STATE_WIN_UI");

// 3) 取回控件（未登记的名字会按原文语义被 Free）
TDxControl main = refMain.Value;
```

---

## 9. 结论

* 两个源单元（4,240 行）**参与编译的全部语句**已 1:1 移植；`LoadComponent` 的 23 个分支、
  版本分派、字段赋值顺序与字节消费顺序与原文一致，分歧点（6 处结构性差异）全部写成差异断言。
* 记录布局表（52 条 × 逐字段偏移）由脚本从原文抽取并用整表回读测试锁定，**无手工转录**。
* 门禁：解决方案 build 0 error；`GXX.Client.Tests` 2425 通过 / 0 失败（新增 147）。
* 关键结论给后续批次：**这些窗口的布局真源就是本单元 + `DxComponents.pas` 的记录布局**，
  `.dfm` 对齐不适用；消费方只需实现 `IDxGuiResourceProvider` 注入 `ZDAT` 资源即可复用整条栈。

---

## 10. 去重（并行批次P2-fix，2026-09-20）

### 10.1 事故与裁决

车道 `p2-dxcontrols-rest` 把 `DxControls.pas` / `DxImageForm.pas` 的全量移植并入
`GXX.Client.DxComponent` 后，与本车道 `GXX.Client.LoadDx` 里早先自造的接缝类型**同名**。
凡是同时 `using` 两个命名空间的文件（本项目全部测试）都报 `CS0104`。

**根因**：C# 的"当前命名空间声明优先于 `using` 导入"规则掩盖了源码侧的撞车 ——
`src/GXX.Client`（我自己的命名空间）编译**通过**，而 `GXX.Client.Tests` 里两个命名空间都在作用域，
于是 82 处 `CS0104` 全部落在测试工程。

**裁决（并行调度会话）**：`GXX.Client.DxComponent` 是这些类型的正式归属；本车道删除自造声明、改引用。

### 10.2 全量重复清单（用脚本对两个命名空间的公开类型求交集得出，非人工搜）

| # | 类型名 | 本车道原声明位置 | 正式归属（`GXX.Client.DxComponent`） | 处理 |
|---|---|---|---|---|
| 1 | `TAlignEx` | `GuiRecords.g.cs`（生成） | `DxImageForm.cs` | 生成器加入"已有归属"清单，不再生成；值与顺序逐个核对**完全一致**（`alxNone..alxBottomRight`） |
| 2 | `TDrawAligment` | `GuiRecords.g.cs`（生成） | `DxControls.cs` | 同上；`daFill/daBottom` 一致（正式那份无 `: byte` 底层类型，读写用显式转换，字节宽度不变） |
| 3 | `TDxControlRef` | `DxControlSeams.cs` | `DxControls.cs` | 删除自造类；正式类**无无参构造** → `THashedStringList.Register` 改 `new TDxControlRef(null)`；`Value` 字段同名 |
| 4 | `TDxImageForm` | `DxControlSeams.cs` | `DxImageForm.cs` | 删除自造类；`LoadComponent` 的 `t_Form` 分支直接用正式类 |
| 5 | `TDxImageFormShape` | `DxControlSeams.cs` | `DxImageForm.cs` | 删除自造类与 `TDxFormShapeItem`（由正式 `TDxFormShapeImage` 取代，成员名逐个一致） |
| 6 | `TDxScrollControl` | `DxControlSeams.cs` | `DxControls.cs` | 本车道承载面改名 `TDxScrollControlSeam` 并**派生自**正式抽象基类；4 个子类改挂到承载面 |

### 10.3 引用侧同步改动（只改引用，未改语义）

| 文件 | 改动 |
|---|---|
| `Tools/GenGuiRecords.ps1` | `$existingEnums` 增加 `TAlignEx`/`TDrawAligment` → 不再生成重复枚举 |
| `GuiComponentLoader.cs` | `AssignAnimation` → `AssignImageFormAnimation`，目标类型改为正式 `TDxImageFormAnimation`；**唯一字段名映射**：原文 `DrawBeforeDef` → 正式类 `PaintBeforeDefault`。`dxFormShape.ImageCount` → `ShapeImageCount`。滚动族转换目标改为 `TDxScrollControlSeam` |
| `DxControlSeams.cs` | 删除上文第 3-6 项的自造类；`TDxGuiAnimation` 保留（现在只被 `TDxMainBottomForm` 接缝使用，正式类没有对应类型） |
| `LoadDxControlExLoaderTests.cs` | `new TDxControlRef()` → `new TDxControlRef(null)`（正式类无无参构造） |
| `LoadDxControlLoaderTests.cs` | `Assert.Equal(200, BackgroundAlpha)` → `(byte)200`（正式 `TDxImageForm.BackgroundAlpha` 是 `byte`，原接缝误用 `int`） |
| `LoadDxNamespaceCollisionTests.cs`（新增） | **回归守卫**：断言两个命名空间公开类型简单名交集为空；断言 6 个去重名只属于 `DxComponent`；断言滚动族确实挂在正式基类之下；反向断言本车道自有接缝仍在（防止"顺手删干净"） |

### 10.4 验证（用对方的**真实**文件，不是替身）

| 步骤 | 结果 |
|---|---|
| ① 复现（未修复 + 按对方真实声明做的最小替身） | `GXX.Client.Tests` **82 处 CS0104**（`TDxControlRef`×64 / `TDxImageForm`×8 / `TDrawAligment`×4 / `TAlignEx`×4 / `TDxImageFormShape`×2） |
| ② 修复后 + 替身 | `GXX.Client` 0 error；`GXX.Client.Tests` **2425 通过 / 0 失败** |
| ③ 修复后 + **`par/p2-dxcontrols-rest` 的两个真实文件**（`git show` 取出后临时放在本车道目录 `LoadDx/_verify_ref/`，命名空间不变；`GXX.slnx` 全量 build + 测试） | **0 error**；`GXX.Client.Tests` **2430 通过 / 0 失败**（2425 + 5 条守卫）；验证后临时目录已删除 |
| ④ 仅本车道（对方车道**未**合并） | 12 处 `CS0246`（`TAlignEx`/`TDrawAligment`/`TDxControlRef`/`TDxImageFormAnimation`/`TDxScrollControl` 等）—— **这是本车道分支的已知依赖，见 §10.5** |

### 10.5 未完成 / 依赖（需调度会话决策）

1. **本车道分支无法独立构建**：去重后本车道引用了 `p2-dxcontrols-rest` 的
   `src/GXX.Client/DxComponent/{DxControls.cs,DxImageForm.cs}`，而本工作树还没有这两个文件。
   解决方式（任一）：
   * 把 `par/p2-dxcontrols-rest` 合并进 `par/p2-client-loaddx`（推荐；合并后本车道即可独立绿），或
   * 集成时把这两个车道**一起**合入（本报告 §10.4 ③ 已证明该状态全绿），或
   * 授权本车道在自己的工作树执行一次 `git merge par/p2-dxcontrols-rest`（车道规程默认禁止 merge）。
2. **`GXX.Client.DxComponent` 内部仍有一处同名重复（不是本车道的问题，供统一）**：
   | 类型 | 顶层声明 | 嵌套声明 | 影响 |
   |---|---|---|---|
   | `TClickSound` | `DxControls.cs`（`DxComponents.pas:54` 的正式归属） | `DxLabel.cs:22` 的 `TDxImageButton.TClickSound` | 本车道 `GuiRecords.g.cs` 的按钮/开关记录字段**只能**用嵌套那份，因为 `TDxImageButton.ClickSound` 的属性类型就是嵌套枚举（`dxImageButton.ClickSound = g.ClickCount` 直接赋值）。建议在 `DxLabel.cs` 侧改用顶层 `TClickSound` 并删掉嵌套声明，本车道随后可去掉 `$typeNameRemap` |
3. **将来会撞车的接缝名（建议提前登记归属）**：本车道仍有 14 个类型的名字与
   `DxComponent` 未来必然移植的单元同名，一旦那些单元落地就会再次 `CS0104`：
   `TDxEdit`(DxEdit.pas)、`TDxImageEdit`(DxImageEdit.pas)、`TDxImageGrid`(DxImageGrid.pas)、
   `TDxPopupMenu`(DxPopupMenu.pas)、`TDxComboBox`(DxComboBox.pas)、`TDxPageControl` + `TDxTabSheet`(DxPageControl.pas)、
   `TDxScrollBox` + `TDxChatMemo` + `TDxListView` + `TDxTreeView`(DxMemo.pas / DxListView.pas)、
   `TDxMainBottomForm`(DxMainBottomForm.pas)、`TDxMagicBall`(DxMagicBall.pas)、`TDxSexPanel`(DxSexPanel.pas)、
   `TDxGroupAttackProgress`(DxGroupAttackProgress.pas)、`TDxSwitchButton`(DxSwitchButton.pas)。
   派发这些单元时请指定"移植者拥有该名字"，本车道届时按本次同样的方式交接（删除接缝 + 改引用 + 更新守卫测试）。
   `LoadDxNamespaceCollisionTests` 会在撞车发生时立刻变红并点名。
4. **已关闭的旧待办**：§8.2 第 2 条"`TDxImageFormShape.ImageCount` 待核对"已确认 ——
   正式类 `ShapeImageCount => Items.Length`，`Items = new TDxFormShapeImage[8]`，与本车道取的 **8** 一致。

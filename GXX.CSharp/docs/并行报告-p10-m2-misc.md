# 并行报告：p10-m2-misc（最后一批"无主的产界面单元"7 单元 / 4 个源目录）

> 分支：`par/p10-m2-misc`　基线：`main @ b879503e`
> 独占分区：`src/GXX.M2Server/Sweep9b/Forms/**`、`src/GXX.Core/MemoryIni/**`、`src/GXX.DBServer/Forms2/**`、
> `src/GXX.LoginSrv/Forms2/**`、`tests/GXX.M2Server.Tests/Sweep9bForms*`、`tests/GXX.Core.Tests/MemoryIni*`、
> `tests/GXX.DBServer.Tests/P10b*`、`tests/GXX.LoginSrv.Tests/P10b*`、本文件
> 本报告**滚动更新**：每完成一个切片即 commit，结论不留在脑子里。

---

## 0. 开工侦察结论（先落盘）

### 0.1 ★★ 首要发现：3 个单元在 `main` 上**已经有实现**（台账判据是假阳性）

台账 §38.6(b)、§39.2 把下面 3 个单元记为「**真未移植**」，判据是
「同名 Delphi 类 `T…` 在全部 `src`+`tests` 的 `.cs` 里**找不到 `class` 声明**」。
本车道逐文件复核后确认：**三处都有实现**，判据的失效原因有两条（与 §38.6 自己写下的教训同源）：

| 单元 | 托管实现位置 | 为什么审计工具没看见 |
|---|---|---|
| `uFrmGlobalVarEdit.pas`（233） | `src/GXX.M2Server/Forms/InterServerForms.cs:151-303`（`GlobalVarEditForm`，批次 J14 产物） | ① 类名不同（`TFrmGlobalVarEdit` → `GlobalVarEditForm`）；② 该文件同名 `.cs` 不存在（E1 假）；③ `InterServerForms.cs:151` 超出工具 E2 的「头 40 行」窗口 |
| `dlgReplaceText.pas`（121） | `src/GXX.M2Server/Forms/TextSearchReplaceDialogs.cs:149-210`（`TextReplaceDialog`，批次 J3 产物） | 同上（实现落在该文件第 **149** 行，E2 窗口外） |
| `dlgConfirmReplace.pas`（104） | 同文件 `:215-278`（`ConfirmReplaceDialog`） | 同上（第 **215** 行） |

> **方法论（本车道复核口径，可复用）**：
> 「同名 Delphi 类无声明 ⇒ 未移植」**不是充分条件**——托管类名与 Delphi 类名**本来就会不同**
> （本工程既有惯例：`TfrmXxx` → `XxxForm`）。判「真未移植」必须再做一步：
> 用**该单元独有的公开成员名**（而不是类名）在全树计数，或直接按 `unit-map` 的源路径搜索实现。
> 本次只要搜 `strngrdVar`/`btnRefreshVarClick`/`PrepareShow`/`ReplaceTextHistory` 四个独有名字，
> 三处实现立刻现身（本车道开工侦察第一件事）。

### 0.2 源文件可用性

| 单元 | UTF-8 镜像 `.pas` | `.dfm` | 类型 |
|---|---|---|---|
| `M2Engine/Forms/uFrmGlobalVarEdit.pas`（233） | ✅ | `Source/M2Engine/Forms/uFrmGlobalVarEdit.dfm`（1,548 B） | **文本**（`#nnn` 转义） |
| `M2Engine/Forms/dlgReplaceText.pas`（121） | ✅ | 同目录 `.dfm`（982 B，首行 `inherited`） | **文本 / 继承窗体** |
| `M2Engine/Forms/dlgConfirmReplace.pas`（104） | ✅ | 同目录 `.dfm`（1,367 B） | **文本** |
| `Common/MemoryIniFiles.pas`（888） | ✅ | 无 DFM | — |
| `DBServer/uFrmHumanExport.pas`（151） | ✅ | `Source/DBServer/uFrmHumanExport.dfm`（2,542 B） | **文本** |
| `DBServer/CreateChr.pas`（69） | ✅ | `Source/DBServer/CreateChr.dfm`（944 B） | **二进制**（`FF 0A 00` 头） |
| `LoginSrv/GateSet.pas`（302） | ✅ | `Source/LoginSrv/GateSet.dfm`（6,155 B） | **二进制**（`FF 0A 00` 头） |

⚠ `_analysis/utf8_mirror/**` 只存在于**主工作树**（不在 worktree 内），且其中**二进制 `.dfm` 已损坏**
（§41.3-1）⇒ 两个二进制 DFM 一律回读 `Source/**` 原始字节手工解码（见 §7 复现命令）。

**Basename 唯一性检查（§39.3/§41.10 规程）**：本车道 7 个单元在 `Source/` 全树各只有**一份**
（实测：`CreateChr.pas` / `GateSet.pas` / `uFrmHumanExport.pas` / `uFrmGlobalVarEdit.pas` /
`dlgReplaceText.pas` / `dlgConfirmReplace.pas` / `MemoryIniFiles.pas` 各 1 命中）
⇒ **可以在 `.cs` 头写 `<unit>.pas` 字面量闭合 E2**（无"关闭一份静默闭合另一份"的风险）。

---

## 1. 逐单元进度（滚动）

| # | 单元 | 行数 | 本车道动作 | 已移植方法数/总方法数 | DFM 控件数（对账） | DFM 绑定数（对账） | 状态 |
|---|---|---|---|---|---|---|---|
| 1 | `Common/MemoryIniFiles.pas` | 888 | **1:1 移植** → `src/GXX.Core/MemoryIni/MemoryIniFiles.cs` | **41 / 41** | 无窗体 | 无窗体 | ✅ 完成（+62 例） |
| 2 | `DBServer/uFrmHumanExport.pas` | 151 | **1:1 移植** → `src/GXX.DBServer/Forms2/HumanExportForm.cs` | **3 / 3** | 10 / 10 ✅ | 2 / 2 ✅ | ✅ 完成 |
| 3 | `DBServer/CreateChr.pas` | 69 | **1:1 移植** → `src/GXX.DBServer/Forms2/CreateChrForm.cs` | **3 / 3** | 8 / 8 ✅ | 1 / 1 ✅ | ✅ 完成 |
| 4 | `LoginSrv/GateSet.pas` | 302 | **1:1 移植** → `src/GXX.LoginSrv/Forms2/GateSetForm.cs` | **8 / 8** | 49 / 49 ✅ | 6 / 6 ✅ | ✅ 完成 |
| 5 | `M2Engine/Forms/uFrmGlobalVarEdit.pas` | 233 | **复核（已在 main）** + 对账 + 差异断言 | 6 / 7（缺 `ShowFrmGlobalVarEdit`） | **3 / 5**（缺 `btnClearVar`/`btnRefreshVar`） | **2 / 5** | ⚠ 形态偏离，缺口已登记 X-P10-03 |
| 6 | `M2Engine/Forms/dlgReplaceText.pas` | 121 | **复核（已在 main）** + 对账 + 差异断言 | 5 / 5 | 8 / 8 ✅ | 0 / 0 ✅ | ⚠ 虚分派丢失，缺口已登记 X-P10-04 |
| 7 | `M2Engine/Forms/dlgConfirmReplace.pas` | 104 | **复核（已在 main）** + 对账 + 差异断言 | 1 / 3 | **5 / 6**（缺 `Image1`） | **0 / 2** | ⚠ 缺口已登记 X-P10-05 |

**新增用例数：62（Core）+ 29（DBServer）+ 36（LoginSrv）+ 32（M2Server 复核）= 159 例。**

### 1.1 三向控件对账（§37.3 计数取证）

对每个窗体同时核 **DFM 节点数**、**托管 public 控件字段数**、**实例化数**、**挂到 `Controls` 树上的数**：

| 窗体 | DFM 节点 | 声明字段 | 实例化 | 挂树 | 绑定（DFM / 托管 `+=`） |
|---|---|---|---|---|---|
| `TFrmHumanExport` | 10 | 10 | 10 | 10 | 2 / 2 |
| `TFrmCreateChr` | 8 | 8 | 8 | 8 | 1 / 1 |
| `TFrmGateSetting` | 49 | 49 | 49 | 49 | 6 / 6 |
| `GlobalVarEditForm` | 5 | 3 | 3 | 3 | 5 / 2 |
| `TextReplaceDialog` | 8（本单元）+7（基类 `dlgSearchText.dfm`） | 15 | 15 | 15 | 0 / 0 |
| `ConfirmReplaceDialog` | 6 | 5 | 5 | 5 | 2 / 0 |

### 1.2 ★ 事件计数器的**两处实现陷阱**（本车道实测，供后续窗体车道复用）

§41.3-2 已确认「.NET 8 WinForms 事件不是 field-like event ⇒ 反射数委托字段一律得 0（假绿）」。
本车道在实现该对账时又踩出**两个会让计数"假红"的坑**，已在三个测试工程的工具类里固定：

1. **静态键命名有 3 种形态**：`EventXxx`（.NET Framework）/ `s_xxxEvent`（.NET 8）/
   **`EVENT_XXX`（全大写）**。只按大小写敏感的 `Contains("Event")` 匹配会**漏掉整批 `EVENT_*`**
   —— 实测：`Form.Shown` 的键是 `EVENT_SHOWN`，漏掉之后 `TFrmCreateChr` 的绑定数被数成 **0**。
   ⇒ 必须 `IndexOf("event", OrdinalIgnoreCase)`。
2. **WinForms 会给部分控件接"自己的"内部处理器**：实测 `RadioButton` 自带 **2** 条、
   `ComboBox` 自带 **1** 条。若把它们算进来，`TFrmHumanExport` 的 2 条 DFM 绑定会被数成 **4**。
   ⇒ 计数时按 **`Delegate.Method.DeclaringType` 是否属于窗体类**过滤（结构性判据，
   比"按控件名列例外表"更抗框架改名）。
3. **复合控件自带匿名内部子控件**：`NumericUpDown`（= `TSpinEdit`）内部有 `UpDownEdit` +
   `UpDownButtons` ⇒ 「递归数 `Controls`」会把 10 个 DFM 控件数成 **14**。
   ⇒ 只数"**属于窗体声明字段**的控件是否可达"。

---

## 2. `MemoryIniFiles.pas` 的三选一裁决（派发要求 #2）

### 2.1 裁决：**② 仍有引用 + 有独立语义 ⇒ 1:1 移植**（41/41 例程）

**计数取证**（全部为本车道实测，命令见 §7）：

| 取证项 | 结果 |
|---|---|
| `.dpr/.dproj/.dpk` 命中数 | **0**（`inDpr=False` 属实） |
| 任何 `uses` 命中数 | **1**：`Source/M2Engine/NpcActionCmd.pas:8` `MemoryIniFiles, …`（**活代码**） |
| 调用点命中数 | **3 处构造**：`NpcActionCmd.pas:19215/:19255`、`:19339/:19374`、`:19467/:19502`（`TMemoryIniFile.Create(LoadList)`） |
| C# 侧 `TMemoryIniFile`/`TIniValueList`/`TQuickSortList` **声明**数 | **0**（移植前） |
| 与 `FastIniFile` 的类/方法对照 | 见 §2.2 —— **不是同一单元、也不重叠** |

⇒ 它**不是死代码**（有真实 `uses` + 3 个构造点），且**不是** `FastIniFile` 的第二份实现。

### 2.2 为什么它不是"与 `FastIniFile` 重复的第二份设施"（§14.2 判据）

| 维度 | `FastIniFile.pas`（86,353 B，已有 `GXX.Core/Util/FastIniFile.cs`） | `MemoryIniFiles.pas`（23,937 B，本单元） |
|---|---|---|
| 定位 | **文件后端**：`TFastIniFile = class(TCustomIniFile)`，构造即 `Load`，`Save/UpdateFile` 落盘 | **内存后端**：`TMemoryIniFile` 从 `TStrings` / 文本 / **裸字节缓冲**构造，**没有文件句柄** |
| 构造面 | `Create(FileName)` | `Create()` / `Create(TStrings)` / `Create(Text)` / `Create(Data, Size)` —— 原文 `Move(Data^, Text[1], Size)` 的裸内存入口 |
| 节存储 | `Dictionary` + 首次出现序 | `TQuickSortList`（`TStringList` 子类）**手工快排 + 二分查找**，值表是 `TValueList`/`TIniValueList` |
| 节名查找 | 大小写不敏感（`OrdinalIgnoreCase`） | `CompareText` 二分（节）+ **`CompareStr` 线性**（键，解析路径） |
| 通知 | 无 | `OnChange`（节表 + 值表两层） |
| 额外 API | `ReadFixedDateTime` / `WriteFixedDateTime` / `EraseSection` / `DeleteKey` / `ReadSections` | `SaveToList` / `ReadSection` / `Get` / 4 个日期时间读（**无写**）/ `TIniValueList.GetIndex` |
| 公开面交集 | 仅 `Read/Write(String|Integer|Bool)` 的**语义约定**（`WriteBool='1'/'0'`、`ReadInteger` 的 `0x→$` 改写） | 同上 |

⇒ 交集只有"TCustomIniFile 的约定"，**能力不重叠**：本单元独有的"从内存构造 + 手工快排节表 + `SaveToList`"
在 `FastIniFile` 里**一律没有**。故按 §14.2 判 **不是** 重复设施，**予以移植**。

### 2.3 移植形态与两处"不新建第二份"的处置

- 原文有**两条**值表实现同时被用到：`TIniValueList`（`MemoryIniFiles.pas:24`，**重写** `GetIndex`
  为线性 + `CompareStr`）与 `TValueList`（`SDK.pas:113`，`GetIndex` 为**二分**，`Sorted` 决定
  `CompareStr`/`CompareText`）——`WriteString` 在"节不存在"时建的是后者（`:649`）。
- 托管侧 `GXX.Core.Protocol.TValueList`（`SDK.pas` 的既有权宜移植）**没有值写入口**
  （`Strings[i] := …` 无从表达）、`GetIndex` **非虚**，且该文件在**分区外不可改**。
- ⇒ 本单元用一个 `TIniValueList` + `TIniValueLookup` 枚举还原两条查找路径，
  **没有**新建第二个 `TValueList`（`AddRecord` 的二分插入算法只有一份，两条路径共用）。
  两套查找在"键名大小写不同"时结论相反，已用差异断言锁死。

---

## 3. 原文缺陷清单（**照抄 + 差异断言锁定**，不"修正"）

| # | 位置 | 缺陷 | 后果 | 锁定用例 |
|---|---|---|---|---|
| F1 | `MemoryIniFiles.pas:625-655` + `:223-377` | 往**解析出来的节**写一个与已有键**仅大小写不同**的键时：查找走 `TIniValueList.GetIndex`（`CompareStr`，认作不存在），插入走 `TValueList.AddRecord` 的 **`Count = 1` 分支**（用 `CompareText`，相等则"什么都不做"）⇒ **既没插入也没覆盖** | 节内只有 1 个键时，`WriteString(sec,'name',…)` 在已有 `'Name'` 的情况下**永久失效且静默返回成功** | `WriteString_ExistingParsedSection_CaseDifferentKeyIsSilentlyDropped_OriginalFlaw` + 多键对照组 |
| F2 | 同上 | `AddRecord` 的 `Count = 1` 分支在同名时**既不插入也不置 `Result := False`** | 返回 True 但列表未变（`Count > 1` 的重复路径才返回 False） | `QuickSortList_AddRecord_CountOneDuplicate_…` / `IniValueList_AddRecord_CountOneDuplicate_…` |
| F3 | `MemoryIniFiles.pas:223-377` | `Count = 2` 且重复项落在 **`nHigh`** 时：`nMed` 先被置 0、又被下一句覆写 ⇒ 走成 `InsertObject(nLow+1)` | **插入重复项**且返回 True（`[a,c]` + `AddRecord('c')` ⇒ `[a,c,c]`） | `QuickSortList_AddRecord_CountTwoDuplicate_InsertsDuplicate_OriginalFlaw` |
| F4 | `MemoryIniFiles.pas:101-189` | `GetIndex` 的分支判据是 `Self.Sorted`，**不是** `CaseSensitive`（`boCaseSensitive` 属性对查找**毫无影响**） | 暴露出来的 `boCaseSensitive` 是"看起来能配、实际无效" | `QuickSortList_boCaseSensitive_DoesNotAffectGetIndex_OriginalFlaw` |
| F5 | `MemoryIniFiles.pas:513-516 / :542-545` | `SaveToList/SaveToFile` 的 `else` 分支**原样写节名行** | 空节名 `[]` ⇒ 写出**空行**；`[;x]` ⇒ 写出 `;x`（**丢方括号**）；两者再读回都**不是节** | `SaveToList_EmptyOrSemicolonSectionName_WritesRawLine_OriginalFlaw` |
| F6 | `MemoryIniFiles.pas:569-575` | 节名取 `Pos(']')` 的**第一个** `]`，且**不要求它在行尾** | `[a]b]` / `[a] 尾注` 的节名都是 `a`，其后内容被丢弃 | `Get_SectionNameEndsAtFirstBracket_OriginalFlaw` |
| F7 | `MemoryIniFiles.pas:572` | 节的登记走 `TQuickSortList.AddObject`（**不去重**） | 同名节会有**两份**，`GetIndex` 只命中其一；随后 `SortString` 的 `Exchange` 还会把两份的顺序连同 Objects 翻过来 | `Get_DuplicateSectionNames_AreKeptTwice_OriginalFlaw` |
| F8 | `MemoryIniFiles.pas:703-721` | 整数节序号的 `WriteString` **不置** `FChanged`，越界**静默不做事** | 越界写既不报错也不建节；"少写一句"只在越界时才是可观察差异（在范围内仍会经值表 `OnChange` 置 True） | `IntegerSection_WriteString_OutOfRange_…` + `…_InRange_SetsChangedOnlyViaValueListOnChange_…` |
| F9 | `MemoryIniFiles.pas:527 / :553` | `FChanged`/`FLoadOK` 的守卫被注释掉 ⇒ `FChanged` **只写不读**；`LoadFromFile` 先清标志、紧接着 `Sections.Clear` 的通知又把它置回 True | 标志对行为无影响，但在"非空表"上 `LoadFromFile` 之后它是 True | `LoadFromFile_NonEmptyTable_ClearNotificationResetsFlagToTrue_OriginalFlaw` |
| F10 | `GateSet.pas:177-191` | 网关校验循环**不逐轮重置** `sIPaddr`/`sPort`：空槽跳过 `GetValidStr3`，于是沿用上一轮取值 | 「第 0 槽有效、第 1..9 槽留空」**通过校验**（空槽被当作有效）；只有第 0 槽为空才 `Beep; exit` | `BtnOkClick_EmptyLaterSlots_PassValidation_OriginalFlaw` |
| F11 | `GateSet.pas:203` | 路由查找循环硬编码 `if nGateIdx >= 59 then break;`（`High(GateRoute) = 59`） | **59 号槽永远匹配不到** ⇒ 标题命中 59 号路由时**静默不保存** | `BtnOkClick_RouteIndex59_IsNeverMatched_OriginalFlaw` + 58 号对照组 |
| F12 | `GateSet.pas:244` | `Config.GateRoute[nTitleIdx].sTitle := sTitle` 用的是**过滤后的下拉框序号**当 `GateRoute` 下标 | 选中的服务器不是第 0 个时，**改名写到另一条路由上**：下拉框显示改了、磁盘配置改错了对象 | `BtnChangeTitleClick_WritesToWrongRoute_OriginalFlaw` |
| F13 | `GateSet.pas:281-286` | 去重局部变量名 `boAdded` 与语义**相反**（初值 True、命中相同项置 False、为 True 才 `Add`）；且内层循环**无 `break`** | 名实不符（行为正确）；每轮多扫一遍 | `RefRouteList_CollectsDistinctServerNamesAndSelectsFirst`（正向锁定） |
| F14 | `GateSet.dfm` + `GateSet.pas:27-46` | 第 1 个复选框的 `Caption` 是控件名 `'CkGate1'`，其余 9 个是 `'CheckBox1'`（从未改过） | 界面上 10 个复选框全是无意义文字 | `GateSet_CheckBoxCaptions_CopiedVerbatimFromDfm_OriginalFlaw` |
| F15 | `GateSet.dfm` | 关闭按钮 `BtnClose` 的 `Caption='确定(&O)'`（不是"取消"），`ModalResult=1` | 界面语义混淆 | `GateSet_KeyDfmProperties_PortFaithfully` |
| F16 | `CreateChr.pas:44-49 → :54-55` | `IncputChrInfo` **先清空** `sUserId`/`sChrName`，`GetInputInfo` 再用这两个字段回填编辑框 | "预置初值"这条设计意图**已经失效**：回填进去的永远是空串 | `CreateChr_PrefillsEditsFromPublicFields_ThenClearsThem_OriginalFlaw` |
| F17 | `CreateChr.pas:61-65` | 选择 ID 非法时只弹一次框就 `Exit`（**不重新弹对话框**），返回 False | 调用方拿到 False 后必须自己重来 | `CreateChr_InvalidSelectId_ShowsBoxAndReturnsFalse` |
| F18 | `uFrmHumanExport.pas:85` | `ExtractFileExt(FileName) <> '.txt'`（**大小写敏感**） | 用户选 `X.TXT` 会被改写成 `X.txt`（大小写敏感文件系统上是另一个文件） | `HumanExport_UppercaseTxtExtension_IsRewrittenToLowercase_OriginalFlaw` |
| F19 | `uFrmHumanExport.pas:80` | `Filter := 'AutoLoadOffline\|*.txt'` —— 描述段写成了产品名、`FileName` 也是硬编码英文 | 对话框过滤项显示 `AutoLoadOffline` | `HumanExport_WritesTsvOfRoleNameAndAccount`（原样锁定） |
| F20 | `uFrmGlobalVarEdit.pas:96-123` | `btnSave.Enabled := True;` 写在两个 `MessageBox` 判断**之外** | 用户在"是否清除"上点**否**，保存按钮照样被点亮 | `GlobalVarEdit_NotConfirmed_StillEnablesSave_OriginalFlaw` |
| F21 | `dlgConfirmReplace.pas:96` | 原文用 `MulDiv(nH, 2, 3)`（Win32，**四舍五入**） | 托管写 `nH * 2 / 3`（截断）：`nH=100` 时 67 vs 66，**恰好在边界上给出相反分支** | `ConfirmReplace_PrepareShow_MulDivRoundingIsNotReproduced_Deviation` |

---

## 4. 偏离登记（D-P10-xx）

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P10-01 | `MemoryIniFiles.pas` 8 个 `Read*` 的 `except on EConvertError` | `SysUtils.EConvertError` + `StrToDate/StrToDateTime/StrToFloat/StrToTime` | 本单元自带 `GXX.Core.MemoryIni.EConvertError` + `MemoryIniRtl` 四个转换 | `GXX.Core.Rtl`（分区外）无 `EConvertError`、无这四个函数；`else raise` 的"非 EConvertError 继续上抛"语义用 C# 的 `catch (EConvertError)` 天然等价 |
| D-P10-02 | 同上 | 按**当前区域设置**（`ShortDateFormat` 等）解析 | 固定 `InvariantCulture` | 原文结果随机器变化、不可复现；与 `TIniFixedDateTime` 的 D-P8-4 同一口径 |
| D-P10-03 | `MemoryIniFiles.pas:657-666/:723-732` | `StrToIntDef` **认 `'$'` 十六进制**（内部走 `Val`） | 本单元自带 `MemoryIniRtl.StrToIntDef`（`$` / `0x` / 十进制 / 失败回退） | `DelphiRTL.StrToIntDef` 用 `NumberStyles.Integer` **不认 `$`**，会让 `'0x1F' → '$1F'` 的改写恒回退默认值（真实保真缺口） |
| D-P10-04 | `TQuickSortList` 的 `OnChange` | 由 `TStrings.Changed` 在基类内部触发 | 用 `new` **遮蔽** `Add/AddObject/Insert/InsertObject/Delete/Exchange/Clear`，转调基类后自行触发 | 托管 `GXX.Core.Util.TStringList`（分区外）方法非虚、无 `OnChange`；触发集合按 `TStrings` 公开语义对齐（`Sort` 不触发，原文亦然） |
| D-P10-05 | `MemoryIniFiles.pas:430-441/:443-457` | 在体内二次调用 `Create(FileList)`（同对象再跑一次构造器） | 私有 `ConstructFrom(FileList)` | C# 不能对已构造对象重跑构造器；字段初值顺序与原文一致。原文临时 `TStringList.OnChange := Changed` 的连接**无观察者**（对象尚未构造完），未复制 |
| D-P10-06 | `SDK.pas:559/:588/:605` | `Put/InsertObject/PutName` 在 `Sorted=True` 时先抛 `SSortedListError` | 未复刻该守卫 | 本单元从不在 `Sorted=True` 下写入（`Sorted` 恒为 False） |
| D-P10-07 | `uFrmHumanExport.pas:77/:121` | `TSaveDialog.Create(nil)` + `Execute` | 可注入替身 `TSaveDialogSeam`（默认弹真实 `SaveFileDialog`） | 无头单测不能弹模态框；替身保留 `Title/Filter/FileName/Execute` 四项语义 |
| D-P10-08 | `uFrmHumanExport.pas:85/:128` | `ExtractFileExt`/`ChangeFileExt`（SysUtils） | `P10bFileUtils`（`ChangeFileExt` **转调**既有 `GXX.Core.Paradox.PxFileUtils`） | 不新建第二份；`ExtractFileExt` 按同一口径补齐（Core 侧无公开同名函数） |
| D-P10-09 | `uFrmHumanExport.pas:119` | `GetMobileNumbers(Boolean, TStrings)` | 托管签名收 `List<string>`（既定接口），本方法用临时列表承接后逐条 `SL.Add` | 落地字节一致（每行一条，CRLF） |
| D-P10-10 | `CreateChr.pas:56` | `Self.ShowModal = mrOK` | 可注入 `ShowModalHandler`（默认 `ShowDialog()`） | 同上；`DialogResult.OK=1=mrOk`、`Cancel=2=mrCancel` 数值一致 |
| D-P10-11 | `uFrmHumanExport.pas:66/:119` | `g_RoleDB.HumanDB` 为 `nil` 时**AV** | `SelectClientRoleDbSeam.RequireHuman` 抛 `NotSupportedException`（带接线指引） | Delphi 裸指针解引用在托管侧无从表达；改为可读的可捕获异常 |
| D-P10-12 | `GateSet.pas` 的 `g_Config.GateRoute/nRouteCount` | `LSShare.pas:226/:240` | `GateSetConfigSeam.GateRoute/nRouteCount`（**本车道分区内**的注入点） | 托管 `LoginSrvShare.TConfig` **没有**这两个成员（其类注释明写"GateRoute … 待 LSShare 整单元移植后接入"），且在分区外不可改 ⇒ 见 X-P10-02 |
| D-P10-13 | `GateSet.pas:227` → `LSShare.pas:507-535` | `SaveGateConfig()` | 同文件 `GateSetConfigSeam.DefaultSaveGateConfig` **1:1 落地**（含 `GenSpaceString` 列宽、`'*'` 前缀、空槽截断、`.\!addrtable.txt`） | 该例程属 `LSShare.pas`（依赖缺失）。按本工程既有惯例（`LoginSrvShare` 已承载 `MainOutMessage/GenSpaceString/…`）就近落地并登记：**LSShare 整单元移植时整体搬走**，只保留转调 |
| D-P10-14 | `GateSet.pas:266/:289` | `TComboBox.ItemIndex := 0`（空列表上 `CB_SETCURSEL` 失败 ⇒ **静默保持 -1**） | `P10cComboBox.SetItemIndex`（越界**不改动**、`< -1` 钳到 -1） | WinForms `SelectedIndex = 0` 在空列表上**抛异常**（§21.3 已知陷阱）⇒ 空路由配置打开窗体即崩，必须还原 Delphi 语义 |
| D-P10-15 | `GateSet.pas:186` | `Beep`（Windows `MessageBeep(0)`） | 可注入 `GateSetConfigSeam.Beep`（默认 `SystemSounds.Beep.Play()`） | 测试需要断言"响没响"；托管无同语义 API |
| D-P10-16 | `GateSet.pas:261`（`RefRouteList`） | `GateRoute[I]` 在 `nRouteCount > 60` 时是**无检查的越界读写**（静默内存破坏） | 托管数组抛 `IndexOutOfRangeException` | 托管数组自带边界检查；不刻意复刻内存破坏 |
| D-P10-17 | `InterServerForms.cs`（**既有实现**，非本车道所写） | DFM `Caption='全局G变量编辑'/'全局A变量编辑'` | `'G变量编辑'/'A变量编辑'`（`TextSearchReplaceDialogs.cs` 同类简化） | 复核登记；形态偏离，非功能偏离（见 X-P10-03） |
| D-P10-18 | `TextSearchReplaceDialogs.cs:272`（既有实现） | `MulDiv(nH, 2, 3)`（四舍五入） | `nH * 2 / 3`（截断） | 复核登记；边界用例给出相反分支（F21） |
| D-P10-19 | 装饰性属性 | `Font.Charset/Name/Height`、`PixelsPerInch`、`TextHeight`、`TBitBtn.Glyph`（二进制字形资源） | 未复刻 | §7「不影响协议正确性的装饰性属性允许简化，但控件与行为保留」 |

---

## 5. 跨区请求（X-P10-xx，本车道**不可改**，请集成方处置）

| 编号 | 事项 | 影响 | 建议 |
|---|---|---|---|
| **X-P10-01** | `CreateChr.pas:31 var FrmCreateChr`（`DBServer.dpr:37 Application.CreateForm`）的托管接线点在 `src/GXX.DBServer/Program.cs`（分区外）⇒ 本车道只留了 `TFrmCreateChr.FrmCreateChr` 静态字段 | 该窗体目前**被实例化但无调用点**（唯一调用点 `LoginSrv/uFrmDataManager.pas:139-176` 在原文里被 `(* *)` 整段注释） | 可暂不接线；若接线，在 `Program.cs` 里 `TFrmCreateChr.FrmCreateChr = new TFrmCreateChr();` |
| **X-P10-02** | `LoginSrvShare.TConfig`（`src/GXX.LoginSrv/LoginSrvShare.cs`，分区外）缺 `nRouteCount` 与 `GateRoute[0..59]`（`LSShare.pas:226/:240`） | `GateSetForm` 现在读 `GateSetConfigSeam.GateRoute/nRouteCount`（同名同型注入点） | LSShare 整单元移植时给 `TConfig` 补这两个成员，并把 `GateSetConfigSeam` 的两个静态成员改为**转调 `g_Config`**（只改 getter/setter，**不要**保留两份），并把 `DefaultSaveGateConfig` 搬进 LSShare |
| **X-P10-03** | `GlobalVarEditForm`（`src/GXX.M2Server/Forms/InterServerForms.cs:151-303`）与 `uFrmGlobalVarEdit.pas` 的**形态偏离**：① 缺 `btnClearVar`/`btnRefreshVar` 两个控件（**3/5**）；② 缺绑定 3 条（**2/5**，`OnSetEditText` 未接线）；③ 缺单元级 `ShowFrmGlobalVarEdit(VarType)`（**6/7** 方法，全程序集计数 0 命中）；④ Caption 少"全局"前缀；⑤ 两个保存按钮**初始即可点**（原文由缺失的 `ShowFrmGlobalVarEdit` 先行禁用）；⑥ `DataGridView` 列头是"序号/值/说明"，原文是 `'变量名'/'变量值'/'变量备注'` 且第 0 列填 `'G'+i`/`'A'+i` | 该单元**可用但不完整**：`ShowFrmGlobalVarEdit` 的调用点（`svMain.pas` 等）无法接线 | 由 M2Forms 归属方决定补做 or 登记为既有偏离；`btnClearVar`/`btnRefreshVar` 的处理器 `btnClearVarClick`/`btnRefreshVarClick` **已存在且已测**，缺的只是控件与绑定 |
| **X-P10-04** | `TextReplaceDialog`（`TextSearchReplaceDialogs.cs:188`）用 **`new` 隐藏**而非 `override`：手工调用（或经 `TextSearchDialog` 引用调用）时**虚分派丢失**，替换词不会置顶 | 原文 `TTextReplaceDialog.FormCloseQuery` 是 `override`（基类为 `virtual`）⇒ 通过基类引用也会派发到派生版。调用方若把它当 `TextSearchDialog` 用（`fTxtEditor` 那条路）行为不同 | 把 `TextSearchDialog.FormCloseQuery(out bool)` 改为 `virtual`，派生版改 `override`（两行）。已用 `ReplaceDialog_VirtualDispatchIsLost_Gap` 锁死现状 |
| **X-P10-05** | `ConfirmReplaceDialog`（`TextSearchReplaceDialogs.cs:215-278`）缺 `Image1: TImage`（**5/6** 控件）、缺 `FormCreate`/`FormDestroy` 两个方法与对应绑定（**0/2**）⇒ `Image1.Picture.Icon.Handle := LoadIcon(0, IDI_QUESTION)` 的问号图标未复刻 | 界面少一个图标；`FormDestroy` 的"全局变量置 nil"已由 `Dispose(bool)` 覆盖 | 补 `Image1` + 两条绑定即可；`lblConfirmation` 的 `AutoSize=False/WordWrap=True`（DFM）与托管 `AutoSize=true` 也不一致 |
| **X-P10-06** | `MemoryIniFiles` 的 `TIniValueList` 借用权宜：托管 `GXX.Core.Protocol.TValueList`（`SDK.cs:187`）**没有值写入口**（`Strings[i] := v` 无从表达）、`GetIndex` 非虚 | 本单元用一个类 + `TIniValueLookup` 枚举还原两条查找路径（未新建第二个 `TValueList`） | 若日后给 `GXX.Core.Protocol.TValueList` 补 `SetValue(int,string)` 与 `virtual GetIndex`，可把 `TIniValueList` 改为其子类；**在那之前不要新增第三份值表实现**（§14.2） |

---

## 6. 未完成 / 阻塞项（如实登记）

1. **X-P10-02 是 `GateSetForm` 的长期接缝**：它现在读 `GateSetConfigSeam`，而不是 `g_Config`。
   在 LSShare 落地前，**登录服务器主流程改的 `g_Config.GateRoute` 不会被这个窗体看到**（反之亦然）。
   ⚠ 这是"接缝未接线"而不是"实现错误"，但**接线前不要把它当成已接线的配置编辑器**（§33.4 同源）。
2. **X-P10-03/04/05 三个单元的缺口本车道无法修**（文件在 `src/GXX.M2Server/Forms/**`，分区外）。
   现状已用差异断言锁死（`Sweep9bFormsReconTests`），修好后这些用例会**变红**并提示同步更新。
3. `dlgSearchText.pas`（SynEdit 搜索对话框族第 1 个）**不在本车道单元表内**，
   本次只借用其托管基类做对账；它的自身对账（基类 7 个额外控件、基类 DFM 的绑定）**未做**。
4. `MemoryIniFiles` 的**消费者未接线**：唯一 `uses` 方 `NpcActionCmd.pas`（47,018 行）**整单元未移植**
   （`ActionOfSortVarToList` 等 3 个脚本动作 0 命中）⇒ 本单元目前"已移植但零生产调用方"
   （§35.4 同源）。**这不是本单元的问题**，登记以便接线时按名查找。
5. `LoginSrv` 侧 `LMain.pas:228-233 OpenRouteConfig` 的托管接线点（`LoginSrvService`/`Program`）
   **未接线**（分区外）；`FrmGateSetting` 静态字段已备好。

---

## 7. 复现命令

### 7.1 二进制 DFM 解码（`Source/**` 原始字节，不要用镜像）

```powershell
# CreateChr.dfm：头 23 字节 = FF 0A 00 + 'TFRMCREATECHR' + 00 + 30 10 + Int32(0x399=921)
#                流自偏移 23 的 'TPF0' 起，解到 944 = 文件长度（零残留）
# GateSet.dfm  ：头 25 字节 = FF 0A 00 + 'TFRMGATESETTING' + 00 + 30 10 + Int32(0x17F2=6130)
#                流自偏移 25 起，解到 6155 = 文件长度
$b = [System.IO.File]::ReadAllBytes("D:\chuanqi\daima\GXX原版_Delphi7\Source\LoginSrv\GateSet.dfm")
# 值类型：01=vaList(0x00 结尾) 02=int8 03=int16 04=int32 06=vaString(1 字节长度+GBK)
#         07=vaIdent 08=False 09=True 0A=vaBinary 0B=vaSet 12=vaWString(Int32 字符数+UTF-16LE)
#         ★ 14=vaUTF8String 用的是 **Int32 字节数 + UTF-8**（不是 1 字节长度！）
# 对象 = [前缀字节?]ShortString 类名 + ShortString 名 + 属性表(0x00 结尾) + 子对象表(0x00 结尾)
# 校验：解完的游标必须 == 文件长度；控件名集合必须与 .pas 的字段声明**逐名一致**。
```

**实测坑**：`vaUTF8String`(0x14) 我第一版按"1 字节长度"读，导致**偏移 625 处错位**并抛
`unknown value type 0x68`。正确形态是 `Int32 字节数 + UTF-8 字节`
（`CreateChr.dfm:540` 的 `Label3.Caption='选择ID:'` 即 9 字节 UTF-8）。

### 7.2 `MemoryIniFiles` 的引用取证（裁决 ② 的依据）

```powershell
Select-String -Path "Source\**\*.pas","Source\**\*.dpr" -Pattern "MemoryIniFiles|TMemoryIniFile" -List | Select-Object Path
Get-ChildItem -Path Source -Recurse -Include *.dpr,*.dproj,*.dpk | Select-String -Pattern "MemoryIniFiles"   # 0 命中
```

### 7.3 「同名 Delphi 类无声明」判据的**反例**搜索（供审计工具改进）

```powershell
# 只要搜单元独有的成员名，三处既有实现立刻现身：
Select-String -Path "GXX.CSharp\src\**\*.cs","GXX.CSharp\tests\**\*.cs" -Pattern `
  "strngrdVar|btnRefreshVarClick|PrepareShow|ReplaceTextHistory" -List | Select-Object Path,LineNumber
```

---

## 8. 门禁记录

| 项 | 结果 |
|---|---|
| `dotnet build GXX.CSharp/GXX.slnx -c Debug -m:1 -p:BuildInParallel=false` | **0 error** |
| `dotnet test tests/GXX.M2Server.Tests` | **10,079 通过 / 0 失败** |
| `dotnet test tests/GXX.Core.Tests` | **1,015 通过 / 0 失败**（含本车道 62 例） |
| `dotnet test tests/GXX.DBServer.Tests` | **838 通过 / 0 失败**（含本车道 29 例） |
| `dotnet test tests/GXX.LoginSrv.Tests` | **265 通过 / 0 失败**（含本车道 36 例） |
| 合计 | **12,197 通过 / 0 失败** |

> 全部在本 worktree 内执行；构建带 `-m:1 -p:BuildInParallel=false`（内存协调，§39.1）。
> 本车道**未改** `GXX.slnx` / 任何 `*.csproj` / `Directory.Build.props` / `docs/Checklist.md` /
> `docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**`（全部只读引用）。


# 并行报告：p9-m2-forms（M2Engine 窗体/杂项族 7 单元）

> 分支：`par/p9-m2-forms`　基线：`main @ b48e4c43`
> 独占分区：`GXX.CSharp/src/GXX.M2Server/Sweep9/Forms/**`、`GXX.CSharp/tests/GXX.M2Server.Tests/Sweep9Forms*.cs`、本文件
> 本报告**滚动更新**：每完成一个切片即 commit，结论不留在脑子里（派发方要求）。

---

## 0. 开工侦察结论（先落盘）

### 0.1 源文件可用性

| 单元 | UTF-8 镜像 `.pas` | `.dfm` | 备注 |
|---|---|---|---|
| `ConfigMerchant.pas`（654） | ✅ `_analysis/utf8_mirror/M2Engine/Forms/` | ✅ 文本 DFM 466 行 | 直接可读 |
| `ConfigMonGen.pas`（60） | ✅ 同上 | ✅ 文本 DFM 37 行 | |
| `ViewHeroRcd.pas`（453） | ✅ 同上 | ⚠ **镜像里的 `.dfm` 已损坏** | 见 §0.2 |
| `ViewKernelInfo.pas`（198） | ✅ 同上 | ✅ 文本 DFM 500 行 | |
| `uFrmClientPlugManager.pas`（151） | ✅ 同上 | ✅ 文本 DFM 33 行 | |
| `NoticeM.pas`（119） | ✅ `_analysis/utf8_mirror/M2Engine/` | 无 DFM（非窗体） | |
| `uAliyunSendSMSThread.pas`（320） | ✅ 同上 | 无 DFM（非窗体） | |

### 0.2 ⚠ `ViewHeroRcd.dfm`：镜像副本**不可用**（必须回读 `Source/`）

- `_analysis/utf8_mirror/M2Engine/Forms/ViewHeroRcd.dfm` 是**二进制 DFM 被当作文本转码**后的产物：
  首字节 `FF` 变成 `EF A3 B5`（3 字节），长度字段 `83 11 00 00` 变成 `3F 00 00`（**丢字节、长度错**）。
  长度：镜像 4,541 字节 vs 原始 4,510 字节。
- 因此本车道**回读原始二进制** `Source/M2Engine/Forms/ViewHeroRcd.dfm` 并**手工解码**（DFM 是二进制格式，
  本身不是 GBK 文本 ⇒ 不存在乱码风险；文件内字符串按 `vaWString`(0x12, UTF-16LE) / `vaString`(0x06, GBK) 解码）。
- **二进制格式实测结论（与常见资料不同，按实测）**：
  - 文件头 27 字节：`FF 0A 00` + 全大写窗体名（NUL 结尾）+ `30 10` + `Int32`(DFM 流长度 = 4483)，
    紧接着偏移 27 起是标准 `TPF0` 流；`27 + 4483 = 4510` = 文件长度（**自洽**）。
  - 对象节点 = `ShortString 类名` + `ShortString 名` + 属性表（`0x00` 结尾）+ 子对象表（`0x00` 结尾）；
    **本变体没有"前缀字节"**（既不是 inherited 也不是 inline）。
  - `vaList`(0x01) 在本文件里是**无名值列表**（`ColWidths`/`RowHeights`），不是 name=value 表。
  - 解码校验：`p` 收尾 = 4510 = 文件长度（**全覆盖、零残留**），29 个控件与 `.pas:11-39` 声明**逐名一致**。
- 复现：见本报告 §5「二进制 DFM 解码复现命令」。

### 0.3 ★★ 高价值原文缺陷（`ViewKernelInfo.pas:116/:122/:128`）：`@Config.XxxThread` 取的是**指针字段的地址**

```pascal
Config := @g_Config;                          // :99
ThreadInfo := @Config.UserEngineThread;       // :116  ← UserEngineThread: pTThreadInfo（M2Share.pas:1808）
ThreadInfo.hThreadHandle := 0;                // :117  ← 写进"指针字段本身"所在的内存
ThreadInfo.dwRunTick   := 0;                  // :118
ThreadInfo.nRunTime    := 0;                  // :119
ThreadInfo.nMaxRunTime := 0;                  // :120
ThreadInfo.nRunFlag    := 0;                  // :121
```

`UserEngineThread` / `IDSocketThread` / `DBSOcketThread` 在 `g_Config` 里是**指针字段**（`pTThreadInfo`，4 字节）。
`@Config.UserEngineThread` 取的是**该指针字段的地址**（作者显然想写 `Config.UserEngineThread` —— 但那也没 `New`，
仍是 nil 解引用），于是 `ThreadInfo.xxx := 0` 把 `TThreadInfo`（M2Share.pas:558-568，8 字段 × 4B）**覆盖到 `g_Config`
的后续字段上**。按字段偏移精确换算（`TThreadInfo` 字段序：`dwRunTick`+0 / `boActived`+4 / `nRunFlag`+8 /
`boTerminaled`+12 / `nRunTime`+16 / `nMaxRunTime`+20 / `hThreadHandle`+24 / `dwThreadID`+28；
`g_Config` 该段布局：`+0 UserEngineThread`、`+4 IDSocketThread`、`+8 DBSOcketThread`、`+12 nUserSellOffCount`、
`+16 nUserSellOffTax`、`+20 nSkill69CD`、`+24 nSkill69AddTime`、`+28 nSkill69AddRange`、`+32 boSkill69SameLevel`、
`+36 nSkill70CD`），**`FormCreate` 跑完一次的净效果**是：

| 被写坏的 `g_Config` 字段 | 变成 | 来源 |
|---|---|---|
| `UserEngineThread` | `nil` | `:116` 基址 +0 ← `dwRunTick = 0` |
| `DBSOcketThread` | `nil` | `@IDSocketThread` 基址 +4 ← `nRunTime`、或 `@UserEngineThread` +8 ← `nRunFlag` |
| `IDSocketThread` | `nil` | `@IDSocketThread` 基址 +0 ← `dwRunTick = 0` |
| `nUserSellOffCount` | `0` | `@IDSocketThread` +8 ← `nRunFlag` |
| `nUserSellOffTax` | `0` | `@UserEngineThread` +16 ← `nRunTime` |
| `nSkill69CD` | `0` | `@UserEngineThread` +20 ← `nMaxRunTime` |
| `nSkill69AddTime` | `0` | `@UserEngineThread` +24 ← `hThreadHandle` |
| `nSkill69AddRange` | `0` | `@DBSOcketThread` +20 ← `nMaxRunTime` |
| `boSkill69SameLevel` | `False` | `@DBSOcketThread` +24 ← `hThreadHandle`（4 字节写入的低字节） |

⇒ **打开"内核数据查看"窗口会把三个线程指针置空、并清零 5 个 `Skill69`/寄售配置字段。**
处置：**逐字保留**（用显式内存视图模型 `TConfigThreadRegion` 复刻 4 字节重叠写），
并在测试里用**逐字段差异断言**锁死这张表（D-P9-01 登记，见 §2）。

### 0.4 依赖缺口（先登记，动手时按接缝处理）

托管侧**不存在**下列原文依赖 ⇒ 按 §25.2「接缝默认显式抛/显式注入，不臆造中性替身」处理：

| 原文依赖 | 现状 | 处置 |
|---|---|---|
| `UserEngine`（`UsrEngn.pas` 全局对象） | 全树 0 命中 | 每个窗体自带**实例级** `Func/Action` 接缝（照 `TFrmDummySetting` 的既有形态） |
| `UserEngine.m_MerchantList` / `.m_MonGenList` | 0 命中 | 同上（列表面：`LockR/UnLockR/Count/Items`） |
| `g_M2RunThreadMgr` / `TM2RunThread`（`M2Threads.pas`） | 0 命中 | 同上（`ThreadDesc/Handle/ThreadID/RunTick/MinRunTick/MaxRunTick/ThreadCPUUsage/MaxThreadCPUUsage`） |
| `g_PlugClientList` / `g_PlugFileMD5ListText{,Len,CRC}`（`M2Share.pas:3971/3804-3806`） | 0 命中 | 接缝静态类 + `pTPlugClientInfo` 托管等价（原文是 `New/Dispose` 的**指针**记录） |
| `TSendSmsRequest` / `g_AcsUtil`（`SendSmsRequest.pas` / `acsUtils.pas`） | 0 命中 | 接缝 |
| `TEvent`（`SyncObjs`） | 0 命中 | 接缝（`TSafeList` 已存在：`GXX.M2Server.Sweep.TSafeList`） |
| `TPlayObject.m_sMobileNumber` / `m_boMobileBind` / `m_sMobileVerifyCode` / `m_dwMobileVerifyTick` | 0 命中 | 接缝（待 `ObjPlayer.pas` 批次落地） |
| `g_FunctionNPC` | 仅注释命中 | 接缝 |
| `TBaseObject` | 托管侧为 `TCreature`（全树 0 处 `class TBaseObject`） | 接缝参数用 `TCreature`，登记 D-P9-02 |

**复用（已存在，不新增替身）**：`GXX.M2Server.Npc.TMerchant`（含 `m_sScript/m_boCastle/...` 全部字段）、
`GXX.M2Server.Npc.NpcProcessCommand.sNF_*`、`GXX.Core.Protocol.TUserItem`、
`GXX.M2Server.Sweep.TSafeList`、`GXX.M2Server.Sweep.SweepSeam.MainOutMessage / MyGetTickCount`、
`GXX.Core.Util.CheckUnit.BufferCrc`、`GXX.Core.Protocol.EDcode.zEncodeString`、
`GXX.M2Server.Forms.M2Forms`（MessageBox 无头闸门）、`M2ShareState.ConfigIni`、`M2Config.*`。

### 0.5 无头 UI 规程（照 `DummySetting` 车道已验证形态）

- 窗体一律 `System.Windows.Forms.Form` 子类；控件树在构造期实例化（**不需要窗口句柄**即可断言 `Items/Text/Checked/Value`）。
- `ShowModal` → `Sweep9FormsMessageBoxSeam.UiEnabled`（默认 true=生产；测试置 false，否则挂死 testhost）。
- `Application.MessageBox` → 实例级 `MessageBoxHandler`，默认转调 `M2Forms.MessageBox`。
- 所有事件处理器 `public`，测试**直调**，不用消息泵。

---

## 1. 逐单元进度（滚动）

| 单元 | 行数 | 已移植方法数/总方法数 | DFM 控件数（对账） | DFM 绑定数（对账） | 状态 |
|---|---|---|---|---|---|
| `NoticeM.pas` | 119 | 4/4（ctor/dtor/LoadingNotice/GetNoticeMsg） | 无窗体 | 无窗体 | ✅ 完成 |
| `ConfigMonGen.pas` | 60 | 2/2（ListBoxMonGenDblClick/Open） | 2 / 2 ✅ | 1 / 1 ✅ | ✅ 完成 |
| `uFrmClientPlugManager.pas` | 151 | 3/3（LoadPlugClientFiles/Open/ButtonRefClick） | 2 / 2 ✅ | 1 / 1 ✅ | ✅ 完成 |
| `ViewHeroRcd.pas` | 453 | 16/16（含 4 个**空体**：整段被原文注释掉） | 29 / 29 ✅ | 1 / 1 ✅ | ✅ 完成 |
| `ViewKernelInfo.pas` | 198 | 3/3（FormCreate/Open/TimerTimer） | 63 控件 + 1 组件 = 64 / 64 ✅ | 2 / 2 ✅ | ✅ 完成 |
| `ConfigMerchant.pas` | 654 | 39/39（32 个 DFM 处理器 + 6 个 private 辅助 + Open） | 50 / 50 ✅ | **33 / 33** ✅ | ✅ 完成 |
| `uAliyunSendSMSThread.pas` | 320 | 10/10（构造函数 + 9 个具名方法） | 无窗体 | 无窗体 | ✅ 完成 |

**7 个单元全部完成**。用例数：62（切片1）→ 93（切片2 +ViewHeroRcd）→ 120（切片3 +ViewKernelInfo）
→ 174（切片4 +ConfigMerchant）→ **204**（切片5 +uAliyunSendSMSThread），全绿；
`GXX.M2Server.Tests` 全量 **9,686 通过 / 0 失败**。

### ★ 对账方法论的一处**实测修正**（重要，供后续窗体车道复用）

`§37.3` 要求"DFM 绑定数 vs 托管 `+=` 数"对账。本车道实现该对账时踩出**两个会直接造成假绿/假红的坑**，
已写进 `tests/GXX.M2Server.Tests/Sweep9FormsTestKit.cs`：

1. **.NET 8 WinForms 的事件不是 field-like event**
   —— `Control`/`Form` 的事件由 `Component.Events`（`EventHandlerList`）+ 声明类型上的
   **静态键对象**承载（`.NET Framework` 叫 `EventXxx`、`.NET 8` 叫 `s_xxxEvent`、
   另一些是 `EVENT_XXX`）。**只有**"按同名私有委托字段"找 ⇒ 实测一律数成 **0**（假绿）。
   正确做法：静态键（多种命名归一化后比较）+ field-like 字段两条路都走。
2. **复合控件自带匿名内部子控件**
   —— 每张 `DataGridView` 内部有 2 个匿名 `ScrollBar`、`TabControl` 有内部 `UpDown`。
   若按 `Controls` 递归计数，ViewHeroRcd 的 29 个 DFM 控件会被数成 **51**（假红）。
   正确做法：只数**有名字**的控件（DFM 的 `object` 节点全部有名字）。
3. **`System.Windows.Forms.Timer.Tick` 的承载字段叫 `onTimer`**（与事件名不同源，
   实测于 .NET Framework；`OnTick` 是另一个 protected 虚方法）⇒ 名字机械对齐会漏计，
   已在测试工具里以**仅含实测条目的例外表**登记（不猜）。

---

## 2. 偏离登记（D-P9-xx）

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P9-01 | `ViewKernelInfo.pas:116-133` | `@Config.UserEngineThread` 的 4 字节重叠写（§0.3） | 用 `TConfigThreadRegion` 的**显式内存视图**复刻同一组 4 字节写 | 托管侧无裸指针；用显式 dword 视图**精确复刻**原文缺陷，而不是"修正"它 |
| D-P9-02 | `uAliyunSendSMSThread.pas:12/:37` | `Player, Npc: TBaseObject` | 形参类型用托管基类 `TCreature` | 托管侧无 `TBaseObject` 类型（`class TBaseObject` 全树 0 命中）；`TBaseObject` 在原文即 `TCreature` 的别名层 |
| D-P9-03 | `ConfigMonGen.dfm:32` `pnl.Caption` | `TPanel.Caption = '双击条目复制到剪贴板'` | 文本存 `pnlCaption` 字段，**不新增子控件、不订阅 `Paint`** | WinForms `Panel` 无 `Caption`；两种"补显"做法会分别破坏**控件数**与**事件绑定数**的 DFM 对账 |
| D-P9-04 | `ConfigMerchant.dfm` 的 15 条 `Hint` + 窗体 `ShowHint = True` | `ToolTip` 语义 | Hint 原文存 `DfmHints` 字典，**不挂 `ToolTip`** | 实测 `ToolTip.SetToolTip` 会给目标控件追加 `MouseEnter/MouseLeave/…` 一批事件 ⇒ 绑定数 33→**59** |
| D-P9-05 | 全车道 | VCL `TControl.Enabled` / `Visible` 是**控件自身**标志 | WinForms 版本与**父链求与**（`Form` 未 `Show` 前子控件 `Visible` 恒 false） | 无头测试里读不到"控件自身"的值；`Sweep9Memo.DfmVisibleStored` 为留证镜像；对账用例已按父链语义改写并单独锁定该差异 |
| D-P9-06 | `uAliyunSendSMSThread.pas:156/:182-207/:232` | `Player2: TPlayObject` 的 6 个成员 + `SendMsg(BaseObject, …)` 7 参重载 | `ISweep9FormsSmsPlayer` 接缝接口（成员名逐字照抄） | 这 6 个成员 + 1 个方法重载在托管侧**全树 0 命中**；**不**用 §19.6 的 `partial` 补 `TPlayObject`（成员多、且与未来 `ObjPlayer.pas` 批次 CS0102 风险高） |

---

## 3. 原文缺陷清单（逐条照抄 + 差异断言锁定）

| # | 单元:行 | 缺陷 | 锁定用例 |
|---|---|---|---|
| 1 | `NoticeM.pas:85-96` | 命中循环**不 Break** ⇒ 同名登记两份时**两份都追加** | `GetNoticeMsg_DuplicateRegistrations_AppendsAll_NoBreak` |
| 2 | `NoticeM.pas:89-97` | 命中项 `sList = nil` ⇒ `Result=False` 且 `bo15=False` ⇒ **直接 Exit**，"已登记名字但列表为 nil"会把该名字**永久卡死** | `GetNoticeMsg_RegisteredButListNil_ReturnsFalse_AndSkipsNewRegistration` |
| 3 | `NoticeM.pas:105-113` | `try/except` 只包住加载；**登记与 `Result:=True` 在 except 之外** ⇒ 读失败也照样登记 + 返回 True | `GetNoticeMsg_LoadThrowsOutsideExcept_StillRegistersAndReturnsTrue` |
| 4 | `NoticeM.pas:12/:40` | `bo0C` 是**死字段**（构造置 True 后全单元不再触及） | `Bo0C_IsDeadField_NeverReadOrWrittenByTheUnit` |
| 5 | `ConfigMonGen.pas:38-57` | `Open` **不清空** `ListBoxMonGen` ⇒ 反复 `Open` 累积条目 | `Open_DoesNotClearList_AccumulatesAcrossCalls_OriginalBehaviour` |
| 6 | `uFrmClientPlugManager.pas:38-42` | 嵌套函数 `IsDir` **声明了但 0 处调用**（死代码） | `Load_IsDir_ImplementationExistsButIsDeadCode` |
| 7 | `uFrmClientPlugManager.pas:111` | 行尾**两个分号** `sLineBreak;;`（Pascal 空语句） | 源码注释留证 + 拼接语义用例 |
| 8 | `uFrmClientPlugManager.pas:116 vs :118` | `g_PlugFileMD5ListTextLen` 用**编码前**长度、CRC 用**编码后**字节长度（不是同一个量） | `Load_SingleFile_JoinsMd5WithoutLineBreak_AndEncodes` |
| 9 | `ViewHeroRcd.pas:239-275` | `ShowBagItem` **两个分支都写第 0 列**，`ShowUserItem` **从不写第 0 列**（同单元内不对称） | `ShowUserItem_NeverWritesColumn0_AsymmetricWithShowBagItem` |
| 10 | `ViewHeroRcd.pas:202` + DFM `RowCount=14` | `InitUserItemGrid` 写 `Cells[0,15]` ⇒ 靠 `TStringGrid` **自动扩容**把 14 涨到 16 | `FormCreate_InitUserItemGrid_AutoGrowsRows14To16_OriginalBehaviour` |
| 11 | `ViewHeroRcd.pas:230` + DFM `ColCount=4` | `sub_49AB10` 写 `Cells[4,0]` ⇒ 列数 4 涨到 5 | `FormCreate_Sub49AB10_WritesMagicHeaders_AndGrowsCols4To5` |
| 12 | `ViewHeroRcd.pas:80-90` | `ShowHumData` **没有 `ShowModal`**（对比同族窗体） | `ShowHumData_DoesNotShowModal_OriginalBehaviour` |
| 13 | DFM `ActivePage` 四处 | `PageControlHero=TabSheet1`（**最后一页**）、`Job0=TabSheet9`、`Job1=TabSheet10`、`Job2=TabSheet15`（不对称） | `DfmReconcile_ActivePages_MatchDfm_Asymmetric` |
| 14 | **`ViewKernelInfo.pas:116-133`** | ★★ `@Config.XxxThread` 取**指针字段地址** ⇒ `FormCreate` 写坏 `g_Config` 9 个字段（见 §0.3） | `FormCreate_ClobbersConfigRegion_AllNineFields` + `FormCreate_FirstBlockAlone_ClobbersOnlyFiveFields` |
| 15 | `ViewKernelInfo.pas:192-194` | 只清 `GridMemory` 第 1 列第 2/3/4 行（不动第 0 列、不动第 1 行） | `TimerTimer_ClearsOnlyGridMemoryColumn1Rows2To4` |
| 16 | `ViewKernelInfo.pas:181` | 线程表第 0 列写 `IntToStr(I)`（**序号从 0 起**），行号才是 `I+1` | `TimerTimer_PopulatesThreadGrid_WithZeroBasedIndex` |
| 17 | `ViewKernelInfo.dfm` Timer | DFM **未写 `Interval`** ⇒ Delphi 默认 1000（WinForms 默认只有 100） | `DfmReconcile_TimerProperties_MatchDfm` |
| 18 | `ConfigMerchant.pas:215-234` | `RefListBoxMerChant` **不清空** ⇒ 反复 `Open` 累积 NPC 条目 | `RefListBoxMerChant_DoesNotClear_AccumulatesAcrossCalls_OriginalBehaviour` |
| 19 | `ConfigMerchant.pas:394-415 vs :477-480` | `LoadScriptFile` 只写 **11** 个开关（**不含** CreateHero/CreateDeputy），`ChangeScriptAllowAction` 写 **13** 个，且英雄两项插在 `SendMsg` 与 `ArmRemoveStone` **之间** | `LoadScriptFile_Writes11FlagsHeader_WithoutHeroFlags_OriginalBehaviour` + `ChangeScriptAllowAction_Writes13FlagsWithHeroInMiddle` |
| 20 | `ConfigMerchant.pas:583` | `EditPriceRateChange` 直接写 `Lines[1]`，**不判空** ⇒ 行数 < 2 时抛 | `EditPriceRateChange_WithoutEnoughLines_Throws_OriginalBehaviour` |
| 21 | `ConfigMerchant.pas:591` | `ButtonScriptSaveClick` **不判 `SelMerchant = nil`**（对比 `ButtonReLoadNpcClick:599` 判了） | `ButtonScriptSaveClick_NullMerchant_Throws_OriginalBehaviour` |
| 22 | `ConfigMerchant.pas:643-651` | `btnSearchClick` 命中后**不 Break** ⇒ 多个匹配停在**最后一个** | `BtnSearchClick_MultipleMatches_StopsAtLast_NoBreak` |
| 23 | `ConfigMerchant.pas:293-299` | `CheckBoxDenyRefStatusClick` 是唯一**不判 `boOpened`、不调 `ModValue`** 的处理器 | `CheckBoxDenyRefStatusClick_DoesNotDirtyButtons_NorCheckBoOpened` |
| 24 | `ConfigMerchant.dfm:457-464` | `btnSearchNext`（"下一个"）**没有 `OnClick` 绑定** ⇒ 点了没反应 | `DfmReconcile_BtnSearchNext_HasNoBinding_OriginalBehaviour` |
| 25 | `ConfigMerchant.dfm:416` | `ButtonViewData.OnClick` **复用** `ButtonClearTempDataClick`（同一处理器的第二次绑定） | `DfmReconcile_ButtonViewData_SharesHandlerWithClearTempData` |
| 26 | `uAliyunSendSMSThread.pas:191-194` | 模板选择与名字**相反**：`not m_boMobileBind` ⇒ 用"绑定"模板 | `DoExecuteLoop_MobileBindTrue_UsesCheckTemplate_OriginalNaming` |
| 27 | `uAliyunSendSMSThread.pas:209` | `else if Npc <> nil` ⇒ **不带 NPC 的发送失败被完全静默吞掉**（无日志、无跳转） | `DoExecuteLoop_Failure_WithoutNpc_IsSilentlySwallowed` |
| 28 | `uAliyunSendSMSThread.pas:230-234` | 失败跳转用的是 **`g_FunctionNPC`**（全局功能 NPC）而非本次任务的 `Npc` ⇒ 任务里的 `Npc` 仅用于 :209 判空 | `DoExecuteLoop_Failure_WithNpc_LogsAndGotosLabel` + `DoExecuteLoop_Failure_NullFunctionNpc_NoGoto` |
| 29 | `uAliyunSendSMSThread.pas:130` | `Execute` 循环条件只在**顶部**判 `Terminated` ⇒ `Terminate` 后还会再跑一轮 | `Execute_LoopTopChecksTerminated_RunsOneMoreRoundAfterTerminate` |
| 30 | `uAliyunSendSMSThread.pas:177 vs :237` | `Player = nil` 的 `Exit` 在 `try` **之前** ⇒ 空表时**不** `TriggerEvent`（不会自旋） | `DoExecuteLoop_EmptyList_ReturnsWithoutTriggeringEvent` |
| 31 | `uAliyunSendSMSThread.pas:268-269` | `LoadSendSMSConfig` 每次**无条件**删掉 `Endpoint`/`Topic` 两个历史键；整数键用 `ReadInteger`（与字符串键成对但不同型） | `LoadSendSMSConfig_CleansLegacyKeys_AndWritesDefaultsWhenMissing` + `LoadSendSMSConfig_ReadsExistingValues` |

---

## 4. 未完成 / 阻塞项（如实登记）

### 4.1 待接线（不是本单元的内容；本单元已提供接缝）

| 接缝 | 原文位置 | 接线方 |
|---|---|---|
| `MerchantListHandler` / `MerchantListLockR` / `MerchantListUnLockR` | `UsrEngn.pas UserEngine.m_MerchantList` | `UsrEngn.pas` 批次 |
| `MonGenListHandler` / `MonGenListLockR` / `MonGenListUnLockR` | 同上 `m_MonGenList` | `UsrEngn.pas` / `Envir.pas` 刷怪表批次 |
| `g_MultiThreadRun` | `M2Threads.pas:59` | `M2Threads.pas` 批次 |
| `RunThreadMgrHandler` / `Sweep9FormsRunThreadInfo.ThreadCPUUsageFn` | `M2Threads.pas:11-58` | `M2Threads.pas` 批次 |
| `Sweep9FormsKernelConfig`（`KernelConfig`） | `M2Share.pas` 的 `g_Config` 计数/彩票/`GlobalVal[0..999]` | `M2Share.pas` 批次 |
| `GetStdItemNameHandler` | `HUtil32`/`DBShare` 的 `GetStdItemName` | 已默认转调既有 `DbLayerRunSeam.GetStdItemName`（同一未接线项，未新增替身） |
| `Sweep9FormsPlugClientGlobals.*` + `SendPlugClientListHandler` | `M2Share.pas:3971/3804-3806` + `UsrEngn.pas` | `M2Share.pas` / `UsrEngn.pas` 批次 |
| `Sweep9FormsNoticeGlobals.NoticeManager` | `M2Share.pas:3685`（`svMain.pas:1945` 创建） | `M2Share.pas` / `svMain` 批次 |
| `ISweep9FormsSmsPlayer` 的实现类（在 `TPlayObject` 上接出） | `ObjPlayer.pas` 的 6 个手机字段 + `SendMsg` 7 参重载 | `ObjPlayer.pas` 批次（见 D-P9-06） |
| `ISweep9FormsSendSmsRequest` 的实现类 | `SendSmsRequest.pas` | `SendSmsRequest.pas` 批次 |
| `ISweep9FormsFunctionNpc` 的实现类 | `M2Share.pas g_FunctionNPC` | `M2Share.pas` 批次 |
| `Sweep9FormsSmsSeams.CreateIniFile` 的生产实现已可用（薄包 `TFastIniFile`） | — | ✅ 无需接线 |
| `g_AcsUtil` / `g_RequestStr` | `acsUtils.pas` / `acsParams.pas` | 阿里云 SDK 相关批次 |

### 4.2 跨区事项（**需要集成方/顺序会话处理**）

1. ★ **`Sweep9/Forms/ConfigMerchant/Sweep9FormsTCreatureGapMember.cs`**：按 §19.6 用 `partial` 给
   `GXX.M2Server.Engine.TCreature` 补了缺口成员 **`m_boDenyRefStatus`**（原文 `ObjBase.pas:340`，
   全树 0 命中；`ConfigMerchant.pas:297` 是它的写入点）。
   **若日后 `ObjBase.pas` 批次正式落地该字段，必须删掉本文件**（否则 **CS0102**）。
2. ⚠ **既有 `GXX.M2Server.Engine.TUserEngine`（`Engine/UsrEngn.cs`，181 行）不是 `UsrEngn.pas` 的 1:1 移植**
   —— 它只有 `MapList/PlayObjects/Monsters/MagicDefs` 等骨架字段，**没有** `m_MerchantList`/`m_MonGenList`/
   `GetPlayObject`/`SendPlugClientList`。本车道因此**没有**复用它做接缝载体（否则会掩盖"依赖未移植"）。
   请集成方注意：`Forms/GroupItemSkillPowerForm.cs` 等**旧车道**窗体依赖这个骨架类，与本车道的接缝形态不同。
3. `GXX.Core.Util.TStringList` **没有** `AddStrings`（本车道以扩展方法 `Sweep9FormsStringListExtensions.AddStrings`
   补齐，调用形态与原文一致）。若集成方决定把它收进 `GXX.Core`，本车道的扩展方法会被实例方法**自然遮蔽**（无需改动）。
4. `GXX.Core.Util.TFastIniFile` 有 `ValueExists`/`DeleteKey`，但既有 `Sweep.ISweepIniFile` 接缝**没有**这两个成员
   ⇒ 本车道为 SMS 单元自带 `ISweep9FormsSmsIniFile`（薄包 `TFastIniFile`）。建议后续统一 `ISweepIniFile` 的面。

### 4.3 无（本车道 7 单元无"未完成"项）

7 个单元的全部过程/方法/DFM 控件/DFM 事件绑定均已 1:1 落地并有测试锁定；
"未接线"项全部是**原文依赖的其它单元**（见 4.1），非本分区可在不越区的前提下完成的工作。

---

## 5. 门禁记录（最终）

```text
$ dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false
已成功生成。  0 个错误（15 个既有 warning，全部来自他区文件）

$ dotnet test GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj -c Debug --nologo -m:1 -p:BuildInParallel=false
已通过! - 失败: 0，通过: 9686，已跳过: 0，总计: 9686，持续时间: 46 s

本车道用例（--filter FullyQualifiedName~Sweep9Forms）：204 / 204 通过（实测逐类计数）
  · Sweep9FormsNoticeMTests            21
  · Sweep9FormsConfigMonGenTests       16
  · Sweep9FormsClientPlugManagerTests  25
  · Sweep9FormsHeroRcdTests            31
  · Sweep9FormsKernelInfoTests         27
  · Sweep9FormsConfigMerchantTests     54
  · Sweep9FormsAliyunSmsTests          30
  · 合计 204
```

### 5.1 ⚠ 分区事故（自查并已修正，记录备查）

编写切片 5 时，一次 PowerShell 批改脚本把输出路径写成了
`GXX.CSharp/tests/Sweep9FormsAliyunSmsTests.cs`（**漏了 `GXX.M2Server.Tests/` 一层**），
`Get-Content` 失败后 `Set-Content` 仍创建了一个 3 字节（仅 BOM）的**空文件**。
后果：该文件**不属于任何工程** ⇒ 不会被编译（用例会静默丢失），且 `git add -A` 会把它提交进 main。
已删除；`GXX.CSharp/tests/` 根目录现在 **0 个文件**；`git status --short` 只剩本分区 3 个未跟踪项。
**规程提醒**：批改脚本的 `Set-Content` 目标路径必须显式校验（先 `Test-Path` 或断言目录层级）。

---

## 6. 二进制 DFM 解码复现命令

```powershell
$path = "D:\chuanqi\daima\GXX原版_Delphi7\Source\M2Engine\Forms\ViewHeroRcd.dfm"
$b = [System.IO.File]::ReadAllBytes($path)     # ★ 必须 ReadAllBytes；Get-Content 会读坏二进制
# 头部 27 字节：FF 0A 00 + 'TFRMHEROFDBVIEWER' + 00 + 30 10 + Int32(0x1183=4483)
# 流从偏移 27 开始（'TPF0'），到 4510 结束。
# 对象 = ShortString类名 + ShortString名 + 属性表(0x00结尾) + 子对象表(0x00结尾)；无前缀字节。
# 值类型：01=vaList(无名值表,0x00结尾) 02=int8 03=int16 04=int32 06=vaString 07=vaIdent
#         08=False 09=True 0B=vaSet(ShortString表,0x00结尾) 12=vaWString(Int32字符数+UTF-16LE)
```

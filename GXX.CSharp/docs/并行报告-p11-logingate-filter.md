# 并行报告 p11-logingate-filter：LoginGate 执法面缺口移植

> 车道：`p11-logingate-filter`（写车道）
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p11-logingate-filter`（分支 `par/p11-logingate-filter`）
> 承接：`docs/并行报告-p11-logingate-review.md` §2/§3/§6/§7 与 §5.4 的 `p11-logingate-filter` 车道建议
> 基线：`main @ d8e871e3` 的父级（本工作树切片0 提交 `7b14e03a`）
> 三条门禁：**全绿**（见 §8）

---

## 0. 结论摘要（先给数字）

| 项 | 数字 |
|---|---|
| 新增源码文件 | **7**（GatewayKit/Rest11 **6** + LoginGate/Rest11 **1**） |
| 修改既有文件 | **1**（`src/GXX.LoginGate/LoginGateService.cs`，仅**追加注释**，0 行逻辑改动） |
| 新增测试文件 | **6**（`tests/GXX.GatewayKit.Tests/Rest11*.cs`） |
| 新增测试用例 | **[Fact] 93 + [InlineData] 22 = 115 条声明 → 运行时 **122** 个用例** |
| 新增代码行 | 2,679（src）+ 1,911（tests）= **4,590**（切片1 提交实测） |
| 已移植源单元 | `Misc.pas`(321) / `FuncForComm.pas`(590) / `IPAddrFilter.pas`(400) / `ConfigManager.pas`(269) / `ClientSession.pas`(820) = **2,400 行，25 单元中的 5 单元** |
| 对既有行为的影响 | **0**（全部设施 opt-in 且默认不构造；`SelGate.Tests` 163/163 全绿） |

**一句话**：LoginGate 的 5 个 C 类/残部单元（合计约 2,400 行）已按 1:1 忠实移植为**可选并存设施**，
落在 `GXX.GatewayKit.Rest11` + `GXX.LoginGate.Rest11` 两个新命名空间，**不碰** `GateService`、
**不碰** `SelGate`、**不改任何 csproj/slnx/tools/docs**。

---

## 1. 落点与命名空间（为什么"通用设施"放在 GatewayKit）

| 落点 | 命名空间 | 内容 |
|---|---|---|
| `src/GXX.GatewayKit/Rest11/Rest11Seams.cs` | `GXX.GatewayKit.Rest11` | 选项开关、枚举/记录投影、`IRest11SessionObj`/`IRest11EnforcementChannel`/`IRest11GameCenterChannel` 接缝、`ReverseIP`/`inet_addr`/`inet_ntoa`/`GetValidStr3` |
| `src/GXX.GatewayKit/Rest11/Rest11LoginGateIpFilter.cs` | 同上 | `IPAddrFilter.pas` 全量 1:1（12 个例程 + 5 张全局表） |
| `src/GXX.GatewayKit/Rest11/Rest11LoginGateConfig.cs` | 同上 | `ConfigManager.pas` 全量 1:1（19 字段 + 4 个 `Read*` + `LoadConfig` + `SaveConfig`）+ `Rest11LoginGateIniFile` |
| `src/GXX.GatewayKit/Rest11/Rest11LoginGateMisc.cs` | 同上 | `Misc.pas` 8 个执法例程 + 常量/枚举 |
| `src/GXX.GatewayKit/Rest11/Rest11LoginGateProcMsg.cs` | 同上 | `FuncForComm.pas`：`TProcMsgThread` / `TAddressInfo` / `TAddressListEx` / `ShowThreadInfo` / `OnTimerProc` |
| `src/GXX.GatewayKit/Rest11/Rest11LoginGateSession.cs` | 同上 | `ClientSession.pas` 残部：`RotateBits` / `DelayClose` / `ProcessSvrData` / `UserEnter` / `UserLeave` / `FillUserList` + `SendDefMessage` 组帧 |
| `src/GXX.LoginGate/Rest11/Rest11LoginGateKernel.cs` | `GXX.LoginGate.Rest11` | LoginGate 侧接线：`StartService`/`StopService` 残部、会话进出、计时器入口、8 个执法例程的 kernel 出口、`Rest11GateSessionAdapter`（复用既有 `GateSession`） |

**为什么通用设施必须落在 GatewayKit**：本车道被指派的测试落点是
`tests/GXX.GatewayKit.Tests/Rest11*.cs`，而 `GXX.GatewayKit.Tests.csproj` 只引用
`GXX.GatewayKit` + `GXX.RunGate`（**禁止改 csproj**）⇒ 只有**放在 GatewayKit 里**的代码
才能被该工程覆盖。LoginGate 侧只剩"装配/接线"，无独立逻辑。

---

## 2. 逐单元已移植清单（1:1 对照）

### 2.1 `LoginGate/Misc.pas`（321 行）→ `Rest11LoginGateMisc.cs`（274 行）

| 源例程 | 源行 | 托管成员 | 状态 |
|---|---|---|---|
| `CloseIPConnect(nRemoteIP)` | :155-179 | `CloseIPConnect` | 1:1（含 `g_fServiceStarted` 早退、`LastGameSvrActive` 复合判据、`m_fHandleLogin>=2` 分派） |
| `KickUser(nRemoteIP): Boolean` | :181-205 | `KickUser(int, cfg, list, ch)` | 1:1（三分支；`mDisconnect` 分支**不封禁**只返回 False） |
| `KickUser(const UserObj)` | :207-224 | `KickUser(session, cfg, ch)` | 1:1（先 `FreeSocket` 后置 `KickFlag`；`case` 只列 `mBlock/mBlockList`） |
| `BlockUser(const UserObj)` | :226-242 | `BlockUser` | 1:1（不 `FreeSocket`） |
| `ReverseIP(dwIP)` | :244-250 | `Rest11LoginGateNet.ReverseIP` | 1:1（4 个 `LOBYTE/HIBYTE` 项） |
| `AnsiStrToVal(nPtr, nPos)` | :252-273 | `AnsiStrToVal(string?, out int)` | 1:1（`nPos` 出参语义保留） |
| `SendGameCenterMsg(wIdent, sMsg)` | :275-286 | `SendGameCenterMsg` + `IRest11GameCenterChannel` | 1:1（`nParam := MakeLong(Word(tLoginGate), wIdent)`，**无** SelGate 的 `g_boNetComGate` 分支） |
| `CheckAccountName(sName)` | :288-318 | `CheckAccountName` | 1:1（宽松范围判据 + GBK 双字节回退，见 §5 D-P11-02） |
| 常量 `VER_TYPE`/`VER_VERSION`/`PROGRAM_NAME`/`SG_*`/`GS_QUIT`/`TProgamType`/`g_nProtocolKey2/3` | :8-146 | 同名字段/常量 | 1:1 |

### 2.2 `LoginGate/FuncForComm.pas`（590 行）→ `Rest11LoginGateProcMsg.cs`（507 行）

| 源例程 | 源行 | 托管成员 | 状态 |
|---|---|---|---|
| `TProcMsgThread.Create/Destroy` | :99-117 | 构造 / `Destroy()` | 1:1（`Interval=1`、`Enabled=True`、两个 TList） |
| `Lock/Unlock/AddSession/DelSession/GetSession` | :119-178 | 同名方法 | 1:1（`GetSession` 的 `INVALID_SOCKET` 早退；`DelSession` 按**引用相等**） |
| `TProcMsgThread.Run(Sender)` | :180-257 | `Run(channel, cfg)` | 1:1（快照复制 → 三条踢线路径 + `DelayClose` 到期 + `HandleLogin<3` 门 + `CheckLevel(5)` 日志门） |
| `TAddressInfo`(14 字段) | :29-49 | `TAddressInfo` | 1:1（全 14 字段逐名保留） |
| `TAddressListEx`（7 个例程） | :51-70 + :261-356 | `TAddressListEx` | 1:1（`Add` 走 ZeroMemory ⇒ `nCount=0`；`Find` 按 `inet_addr`；`Delete` 按引用；**只有 `Clear` 加锁**） |
| `StartService`（残部） | :358-427 | `Rest11LoginGateKernel.StartService` | 残部（`g_fServiceStarted` / `Enabled` / `LoadConfig` / `ClearConnectOfIP` / 计时器登记）；UI 与 `VER_TYPE=1` 授权块属 `p11-logingate-ui` |
| `StopService`（残部） | :429-465 | `Rest11LoginGateKernel.StopService` | 残部（清连接表 + `SaveBlockIPList` + `SaveBlockIPAreaList` + 停线程）；窗体网格清理属 UI 车道 |
| `KeepAlive` | :467-505 | 由 `Rest11LoginGateKernel.OnKeepAliveTimer` → `Run` 承接 | 部分（原文遍历 `m_xGameServerList` 的 5 秒心跳属 `ClientThread`/UI 面，见 §7） |
| `ShowThreadInfo` | :507-561 | `Rest11LoginGateProcMsgFormat.ShowThreadInfo` + `IRest11ThreadInfoSource` | 1:1（行号从 1 起、`↑%fM/↓%fK/%dB` 三档、读后清零、`连接: %d/%d`） |
| `OnTimerProc` | :563-587 | `Rest11LoginGateProcMsgFormat.OnTimerProc` | 1:1（4 个 `_IDM_TIMER_*` 分派） |

### 2.3 `LoginGate/IPAddrFilter.pas`（400 行）→ `Rest11LoginGateIpFilter.cs`（483 行）

全量 1:1（复核报告 §4.1 列的 4 项残部缺口**全部**落地）：

| 源例程 | 源行 | 托管成员 | 备注 |
|---|---|---|---|
| `LoadBlockIPList` / `SaveBlockIPList` | :40-77 | 同名 | 缺失即建空文件 |
| `AddToBlockIPList(szIP)` / `(nIP)` | :79-112 | 两个重载 | **两条重载去重键不同**（字符串 vs `Objects` 整数），照抄 |
| `AddToTempBlockIPList(szIP)` / `(nIP)` | :114-147 | 两个重载 | 同上 |
| `IsBlockIP` | :149-176 | 同名 | 永久表 `Exit`、临时表 `Break`（原文写法不同） |
| `OverConnectOfIP` | :178-207 | 同名 | ★ `Count + 1 > Max`，超限**不自增** |
| `DeleteConnectOfIP` / `ClearConnectOfIP` | :209-252 | 同名 | `m_fCheckNullSession` 为假直接 `Exit` |
| `LoadBlockIPAreaList` / `SaveBlockIPAreaList` | :254-313 | 同名 | IP 段表 |
| `IsBlockIPArea` | :315-335 | 同名 | `ReverseIP` 后闭区间比较 |
| `CheckNewIDOfIP` | :337-380 | 同名 | 4 秒窗口 + 阈值 + 窗口过期 `Dec`/删表 |
| 5 个全局表 + 2 个临界区 | :8-15 | 同名实例字段 | 独立实例（**不与 SelGate 共享**） |

### 2.4 `LoginGate/ConfigManager.pas`（269 行）→ `Rest11LoginGateConfig.cs`（455 行）

| 项 | 源行 | 托管成员 | 状态 |
|---|---|---|---|
| `TGameGateList` | :10-15 | `TGameGateList` | 1:1 |
| 19 个字段 | :18-39 | 19 个字段逐名保留 | **全 19 个**（复核报告 §4.1 指"只接了 5 键"） |
| `Create` 默认值 | :56-88 | 构造函数 | 1:1（`ServerPort=5500`、`GatePort=7000+i-1`，**LoginGate 副本值**） |
| `ReadString/ReadInteger/ReadBool/ReadFloat` | :96-141 | 同名 | 1:1（缺失即**回写**；`-1` 探测语义照抄） |
| `LoadConfig` | :143-210 | 同名 | 1:1（段名 `[LoginGate]/[Integer]/[Switch]/[Method]` + `Count<=0` 单网关分支 + `ClientTimeOutTime3` 的 10 秒下限） |
| `SaveConfig(nType)` | :212-267 | 同名 | 1:1（三分支 + `:231 if I = 1` 镜像 idx1） |
| `m_xIni` | :18 | `Rest11LoginGateIniFile` | 保序 GBK INI（与 `SelIniFile`/`TFastIniFile` **并存**，差异见 §3） |

### 2.5 `LoginGate/ClientSession.pas`（820 行）→ `Rest11LoginGateSession.cs`（376 行，**残部**）

| 项 | 源行 | 托管成员 | 状态 |
|---|---|---|---|
| `m_dwProtocolPassword` | :27/:74/:95/:716 | 同名字段 + `UserEnter` 随机化 | ★ LoginGate 独有，**SelGate 版没有** |
| `m_IsCanSetL2Password` / `m_IsCanCheckL2Password` | :28-29/:75-76/:97/:718-719 | 同名字段 | ★ LoginGate 独有（`ProcessCltData` 门控见 §7） |
| `m_IsDelayClose` / `m_dwDelayCloseTick` | :31-32/:78-79/:99-100 | 同名字段 | ★ |
| `DelayClose(DelayTick)` | :785-789 | `DelayClose` | 1:1 |
| `RotateBits(C, Bits)` | :103-121 | `RotateBits` | 1:1（16 位截断语义，见 §5 D-P11-05） |
| `ReCreate` | :87-101 | `ReCreate` | 1:1 |
| `UserEnter` | :706-737 | `UserEnter`（注入 `Rand`/发送回调） | 1:1 |
| `UserLeave` | :739-783 | `UserLeave`（注入 `DrainSendQueue`） | 1:1（`DrainSendQueue` 是接缝，原文依赖 `SendQueue.pas` 的 `DynSendList`） |
| `ProcessSvrData` | :690-704 | `ProcessSvrData` | 1:1 |
| `SendDefMessage` 组帧 | :123-155 | `BuildDefMessageFrame` | 1:1（**含 `:136 Cmd.param := nSeries` 的覆盖缺陷**） |
| `FillUserList` / `CleanupUserList` | :791-806 | `FillUserList` | 1:1（`USER_ARRAY_COUNT = 1048`，且**全表指向同一对象**） |
| `ProcessCltData`（协议密码校验块 :259-401） | :157-401 | **未移植** | 依赖 `DecodeMessage/DecodeString` 与 `m_pOverlapRecv.ABuffer`（`AcceptExWorkedThread`，本仓 0 托管声明）⇒ 见 §7 |
| `ProcessCltData`（`CM_*` 分派 :462-687） | :462-687 | **未移植** | 见 §7（原文该块体量最大，且 :582-678 大段被注释掉） |

---

## 3. 与既有设施的关系（**并存**，不合并、不"统一"）

| 既有设施 | 位置 | 本车道如何对待 | 理由 |
|---|---|---|---|
| `GateService.CheckIP` / `_blockList` / `_perIP` / `AddBlockIP` | `GateService.cs:203-227` | **一字未改**，Rest11 不替换它 | 判据不同：托管 `rec.Count > Max`（先自增再比）vs 原文 `Count + 1 > Max`（超限不自增） |
| `GateService.LoadConfig`（5 键） | `GateService.cs:67-74` | **一字未改**；Rest11 另起 19 字段 + 原文段名 | 段名不兼容（`Gateway/Server/PacketRule` vs `[LoginGate]/[Integer]/[Switch]/[Method]`） |
| `GateSession`（`GatewayProtocol.cs:84-130`） | 同上 | **复用**：`Rest11GateSessionAdapter` 包装它，只补 LoginGate 独有 4 字段 | §14.2"不造第三份实现" |
| `GXX.SelGate.CSelGateIPFilter` | `SelGateIPAddrFilter.cs` | **并存**：本命名单向不引用 SelGate，SelGate 不引用本命名空间 | `SelGateIPAddrFilter.cs:17-25` 自陈"差异必须保留"；两份绑不同 `CConfigMgr`/`_STR_*` |
| `GXX.SelGate.CSelGateMisc` | `SelGateMisc.cs` | **并存**（同上） | `SelGateMisc.cs:11-16`；LoginGate 副本无 `g_boNetComGate`/`tSelGate` 分支 |
| `GXX.SelGate.SelIniFile` | `SelGateConfig.cs:264-409` | **并存**（`Rest11LoginGateIniFile` 同构但独立） | LoginGate 不得依赖 SelGate（反之亦然） |
| `GXX.Core.Util.TFastIniFile` | `FastIniFile.cs` | **不替换**；Rest11 用自带 INI | `ReadBool` 默认值语义不同（`TFastIniFile` 默认 "1"/"0"；`TIniFile` 是 `-1`/`0`） |
| `GXX.Core.Rtl.DelphiRTL.MakeWord` | `DelphiRTL.cs:18-19` | **复用**（具名实参调用） | 语义正确（`(hi shl 8) or lo`）；重载陷阱见 §5 D-P11-04 |
| `DelphiRTL.MakeLong` | `DelphiRTL.cs:16` | **复用**（照抄 SelGate 用法） | 与 `SelGateMisc.cs:151-153` 同一约定 |
| `EDcode.EncodeBuffer` | `GXX.Core/Protocol/EDcode.cs:354` | **复用**（E1 真移植的既有设施） | 复核报告 §4.1：`EDcode` 是唯一 E1 真命中 |

**"绝不统一掉 SelGate 语义"的执行证据**：本车道 7 个新文件对 `GXX.SelGate` 的引用数为 **0**；
`GXX.SelGate` 对 `Rest11` 的引用数亦为 **0**；`SelGate.Tests` 163/163 全绿。

---

## 4. 原文缺陷（照抄 + 断言锁死）

| # | 缺陷 | 源位置 | 托管落点 | 锁死断言 |
|---|---|---|---|---|
| 1 | `KickUser(nRemoteIP)` 的 `mDisconnect` 分支**只返回 False、不执行任何封禁** | `Misc.pas:186-189` | `KickUser(int,...)` | `KickUser_ByIP_mDisconnect_ReturnsFalseWithoutBlocking` |
| 2 | `KickUser(UserObj)` **先 `FreeSocket` 再置 `KickFlag`**（与 `BlockUser` 顺序相反），且 `case` 只列 `mBlock/mBlockList` | `Misc.pas:211-213` | `KickUser(session,...)` | `KickUser_ByObject_FreeSocketThenFlag_ThenBlockByMethod`、`..._mDisconnect_OnlyFreesAndFlags` |
| 3 | `CheckAccountName` 范围 `(sName[I] < '0') or (sName[I] > 'z')` **极宽松**（大写、`':;<=>?@'` 全放行） | `Misc.pas:303` | `CheckAccountName` | `CheckAccountName_RangeIsLoose`（6 组 InlineData） |
| 4 | `OverConnectOfIP` 判据 `Count + 1 > Max`，**超限时不自增** | `IPAddrFilter.pas:193-196` | `OverConnectOfIP` | `OverConnectOfIP_UsesCountPlusOne_AndDoesNotIncrementWhenOver` |
| 5 | `AddToBlockIPList` 两条重载**去重键不同**（字符串 `IndexOf` vs `Objects` 整数） | `IPAddrFilter.pas:83` / `:100` | 两个重载 | `AddToBlockIPList_DedupesPerOverload_StringVsInteger` |
| 6 | `IsBlockIP` 永久表用 `Exit`、临时表用 `Break`（写法不一致） | `IPAddrFilter.pas:161` / `:172` | `IsBlockIP` | `IsBlockIP_ChecksBothPermanentAndTempTables` |
| 7 | `ReadString/ReadInteger/ReadBool/ReadFloat` **缺失即回写默认值**；`ReadFloat` 声明 `szLoadDW` 却不用、且**两次读同一键** | `ConfigManager.pas:96-141` | 4 个 `Read*` | `ReadString_WritesBackDefault_WhenMissing`、`ReadFloat_WritesBackWhenBelowThreshold` |
| 8 | ★ `ReadBool` 用 `< 0` 探测缺失，而 `WriteBool(True)` 落盘为 `-1` ⇒ **显式写 -1 的 True 会被当成缺失并回写成默认值** | `ConfigManager.pas:114` / `:126` + `:127` | `ReadBool` | `ReadInteger_And_ReadBool_TreatNegativeAsMissing`（§5 D-P11-03） |
| 9 | `LoadConfig` 用 `ReadInteger('LoginGate','Count',0)`，`Count=0` 被当有效值读回后仍进入"≤0"分支 | `ConfigManager.pas:179-180` | `LoadConfig` | `LoadConfig_ReadsAllOriginalSections`（`Count=0` 用例） |
| 10 | `SaveConfig` 的 `:231 if I = 1`（`I` 与循环变量 `i` 在 Delphi 下同物） | `ConfigManager.pas:231` | `SaveConfig` | `SaveConfig_Type0_WritesLoginGateSectionAndMirrorsIndex1` |
| 11 | `TAddressListEx.Add` 走 `ZeroMemory` ⇒ **`nCount` 等全字段为 0 而非 1** | `FuncForComm.pas:318` | `TAddressListEx.Add` | `TAddressListEx_AddZeroesAllFields_AndFindMatchesByInetAddr` |
| 12 | `TAddressListEx` 的 `Add/Find/Delete` **不加锁**（只有 `Clear` 加锁） | `FuncForComm.pas:276-356` | 同名方法 | 注释登记（行为断言见 `TAddressListEx_DeleteByReference_AndClear`） |
| 13 | `TProcMsgThread.Run` 末尾 `m_xTempUserList.Clear` 被注释掉；超时分支**先刷新 tick 再踢线** | `FuncForComm.pas:253` / `:227` | `Run` | `Run_ClientTimeOut_KicksBlocksAndRefreshesTick` |
| 14 | `Run` 的 `DelayClose` 到期分支置 `KickFlag`/清 `IsDelayClose`/`FreeSocket`，**不** `BlockUser` | `FuncForComm.pas:215-217` | `Run` | `Run_DelayCloseExpired_FlagsAndFreesWithoutBlocking` |
| 15 | `ShowThreadInfo` 阈值为 `> 1024`（非 `>=`）与 `> 1024*1000` | `FuncForComm.pas:537-542` | `FormatSendBytes/FormatRecvBytes` | `FormatSendBytes_MatchesOriginalThresholds`（5 组 InlineData） |
| 16 | ★ `SendDefMessage` 把 `nSeries` 赋给 `Cmd.param`（而非 `Series`）⇒ **`nParam` 被覆盖、`Series` 恒为 0** | `ClientSession.pas:134` + `:136` | `BuildDefMessageFrame` | `BuildDefMessageFrame_SeriesOverwritesParam_OriginalDefectPreserved` |
| 17 | `ProcessSvrData` 在 `m_fKickFlag` 为真时把标志**反转回 False** 再关连接 | `ClientSession.pas:694` | `ProcessSvrData` | `ProcessSvrData_KickedSessionClearsFlagAndInvalidatesSocket` |
| 18 | `Create`/`ReCreate` 对 `m_dwDelayCloseTick` 写 `GetTickCount`（**无括号**），而 `m_dwClientTimeOutTick` 写 `GetTickCount()`（写法不一致，Delphi 下语义等价） | `ClientSession.pas:79` / `:100` | 构造 / `ReCreate` | 注释登记 + `Constructor_SetsLoginGateOnlyFieldsToZero` |
| 19 | `FillUserList` 把**同一个** `g_pFillUserObj` 填满整个 1048 长数组 | `ClientSession.pas:791-800` | `FillUserList` | `FillUserList_FillsSameInstanceAcrossAllSlots` |

---

## 5. 偏离登记（D-P11-xx）

| 编号 | 性质 | 内容 | 依据 / 影响 |
|---|---|---|---|
| **D-P11-01** | 交付形态偏离 | 5 个单元**不是**"原地改写 `src/GXX.LoginGate/**` 既有文件"，而是**新建并存设施**（`Rest11` 命名空间）。`LoginGateService` **不**默认使用它。 | 车道硬性禁止修改 `src/GXX.LoginGate/**` 与 `src/GXX.GatewayKit/**` 既有文件；且"必须 opt-in、默认不改变现有行为"。**启用路径写在 `LoginGateService.cs` 的追加注释里**（配置段名 `[LoginGate]` 与现有 `Config.ini` 不兼容，直接切换会改变行为） |
| **D-P11-02** | 语义偏差（未修） | `CheckAccountName` 原文按 **AnsiString 字节**逐字节判定；托管按 **UTF-16 代码单元**判定 ⇒ `#$B0..#$C8`/`#$A1..#$FE` 的双字节回退分支在托管侧几乎不可达（GBK 汉字解成单代码单元且 > `'z'`） | 断言：`CheckAccountName_GbkDoubleByteCharsAreRejectedBecauseUtf16CodePointExceedsZ`、`CheckAccountName_Latin1FallbackRangeAcceptsSecondByteInA1Fe`。修复需引入 byte[] 重载，属接口变更 |
| **D-P11-03** | 原文缺陷照抄 | `ReadBool` 把 `-1` 当"缺失" ⇒ `WriteBool(True)` 的落盘值读回为 `Default` | 断言：`ReadInteger_And_ReadBool_TreatNegativeAsMissing`。**未顺手修** |
| **D-P11-04** | 调用陷阱（非缺陷） | `DelphiRTL.MakeWord` 有 `(byte,byte)` 与 `(int,int)` 两个重载；**字面量常量会自动绑到 `(byte,byte)`**，容易被误读为"实参顺序反了"。两者语义均正确（`(hi shl 8) or lo`） | 断言：`CoreMakeWord_ArgumentOrderIsLoHi_AndOverloadAmbiguityTrapIsPinned`。Rest11 改用**具名实参** `MakeWord(lo:, hi:)` |
| **D-P11-05** | 托管语言差异（已在实现内处理） | Delphi 的 `shl`/`Swap` 在 `Word` 上**截断到 16 位**；C# 的 `ushort` 参与 `<<` 会提升为 `int` ⇒ 必须显式 `& 0xFFFF` | 断言：`SwapWord_RequiresExplicit16BitTruncation`（含 0..255 × 2 个位移的全枚举边界） |
| **D-P11-06** | 接缝替代（已登记） | `UserLeave` 第 3 步原依赖 `PSendQueueNode(m_pOverlapSend).DynSendList`（`SendQueue.pas`）⇒ 以 `DrainSendQueue` 委托表达（默认空操作） | `SendQueue` 由复核报告判 B(c3)，已由 `GateSession._sendQueue` 取代 |
| **D-P11-07** | 接缝替代（已登记） | `ShowThreadInfo` 原依赖 `FormMain.GridSocketInfo` + `TIOCPAccepter`（`AcceptExWorkedThread.pas`，全库 **0 托管声明**）⇒ 以 `IRest11ThreadInfoSource`/`IRest11ServerInfo` 表达，**格式化逻辑逐行保留** | 复核报告 §4.2：`AcceptExWorkedThread` 由 `IocpManager` 取代 |
| **D-P11-08** | 容量语义偏离 | `g_UserList` 原文是 `array[0..1047]`（`FillUserList` 把同一占位对象填满）；`Rest11LoginGateKernel` 用 `List<IRest11SessionObj?>`（默认 1048 槽，按 `SocketId % Count` 落槽、初值 `null`） | 原文"全表同一对象"这一特征在 `Rest11LoginGateSession.FillUserList` 中**已 1:1 保留**；kernel 侧是运行期索引，故用 `null` 槽 |
| **D-P11-09** | 容量语义偏离 | `TSessionObj.m_fHandleLogin: Byte` / `m_nSvrObject: Integer` 等在 `Rest11GateSessionAdapter` 上是属性；`HandleLogin` 只读（原文 `ProcessCltData` 才自增，属未移植块） | 见 §7 |

---

## 6. 新增用例明细（运行时 122 个）

| 测试文件 | [Fact] | [Theory] | [InlineData] |
|---|---|---|---|
| `Rest11LoginGateIpFilterTests.cs` | 18 | 0 | 0 |
| `Rest11LoginGateConfigTests.cs` | 17 | 0 | 0 |
| `Rest11LoginGateMiscTests.cs` | 17 | 1 | 6 |
| `Rest11LoginGateProcMsgTests.cs` | 18 | 3 | 11 |
| `Rest11LoginGateSessionTests.cs` | 19 | 1 | 5 |
| `Rest11LoginGateKernelContractTests.cs` | 4 | 0 | 0 |
| **合计** | **93** | **5** | **22** |

- **不依赖真实网络端口 / 真实计时器**：执法副作用走 `FakeEnforcementChannel`，时钟走注入的
  `TickCount`/`Rand` 委托，INI/黑名单文件落在 `Path.GetTempPath()` 下的临时目录并在 `Dispose` 清理。
- **每个公开成员至少一个用例**：`CSelGateIPFilter` 类 12 个例程全覆盖；`Rest11LoginGateConfig`
  19 字段 + 4 `Read*` + `LoadConfig` + `SaveConfig` + INI 类全覆盖；`Rest11LoginGateMisc` 8 例程 + 常量全覆盖；
  `Rest11ProcMsgThread` 7 方法 + `TAddressListEx` 7 方法 + 3 个格式化方法 + `OnTimerProc` 全覆盖；
  `Rest11LoginGateSession` 9 个移植方法 + `IRest11SessionObj` 接缝全覆盖。

---

## 7. 未完成 / 阻塞（如实登记）

1. **`ClientSession.ProcessCltData` 未移植**（原文 :157-401 的协议密码校验块与 :462-687 的 `CM_*` 分派）。
   理由：
   - 该块依赖 `m_pOverlapRecv.ABuffer` + `PAnsiChar(Addr)` 原地内存布局（`IPAddr` 指针算术 +
     `PByte(Addr + Len)^ := 0` 写终止符）与 `m_tLastGameSvr.SendBuffer`；这两者分别属于
     `AcceptExWorkedThread.pas`（全库 0 托管声明）与 `ClientThread.pas`（已由 `TcpLink` 取代），
     1:1 移植需要先决定"原始缓冲指针"在托管侧的表示，会引入跨车道接口约定；
   - `:582-678` 的 `CM_IDPASSWORD`/`CM_ADDNEWUSER` 大段原文**已被注释掉**，其中唯一使用
     `RotateBits` 的 :600/:602 也在注释内 ⇒ `RotateBits` 当前无活跃调用点；
   - 该块的**行为面**（协议密码/二级密码门控的字段与状态机）已随 `Rest11LoginGateSession` 落地。
   ⇒ 建议作为 `p11-logingate-protocol`（或并入 `p11-logingate-cfg-residual`）的独立切片。
2. **`KeepAlive` 的上游心跳部分未移植**（`FuncForComm.pas:467-505` 遍历
   `FormMain.m_xGameServerList` 的 `SendBuffer('%--$')` + 25 秒超时判定）。
   该部分依赖 `TIOCPAccepter`/`TGameServerManager`（`AcceptExWorkedThread`/`IOCPManager` 的 UI 侧），
   属 `p11-logingate-ui` 车道；`LoginGateService.SendKeepAlive()` 已提供等价出口。
3. **UI 面未动**（按任务边界）：`AppMain` 的 socket 状态网格 / 菜单 / `WndProc` / 异常对话框、
   `GeneralConfig`、`PacketRuleConfig` ⇒ 属 `p11-logingate-ui`。
4. **`Rest11LoginGateKernel` 未被单测直接覆盖**：`GXX.GatewayKit.Tests.csproj` 不引用
   `GXX.LoginGate`（本车道禁止改 csproj）⇒ 无法在该工程引用 LoginGate 类型。
   已改为在 `Rest11LoginGateKernelContractTests.cs` 覆盖 kernel **实际使用的**通用入口契约 +
   适配器语义契约。**建议调度方**：给 `GXX.GatewayKit.Tests` 增加 `GXX.LoginGate` 项目引用
   （或新建 `tests/GXX.LoginGate.Tests`，复核报告 §5.4 亦建议如此），届时可把 kernel 的
   8 个执法出口逐条转为直接单测。
5. **`OverSpeed`/`DefenceCCPacket`/`KickOverSpeed` 三类开关只到配置层**：`Misc.pas` 只用
   `m_fKickOverPacketSize` 作为总开关（原文如此）；其余开关的**使用点**在未移植的
   `ProcessCltData`/`ClientThread` 里。已按原文保留字段与落盘。
6. **`m_fOverSpeedSendBack`** 同上：原文在 `ClientThread`/`ProcessCltData` 使用，本次只移植字段。

---

## 8. 门禁输出（三条，全绿）

```powershell
$W='D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p11-logingate-filter'

# 门禁1：整解构建（0 error）
dotnet build "$W\GXX.CSharp\GXX.slnx" -c Debug --nologo -m:1 -p:BuildInParallel=false
# → 已成功生成。  149 个警告  0 个错误

# 门禁2：GatewayKit.Tests（含全部 Rest11 用例）
dotnet test "$W\GXX.CSharp\tests\GXX.GatewayKit.Tests\GXX.GatewayKit.Tests.csproj" -c Debug --nologo -m:1
# → 已通过! - 失败: 0，通过: 122，已跳过: 0，总计: 122

# 门禁3：SelGate.Tests（证明没碰坏 SelGate）
dotnet test "$W\GXX.CSharp\tests\GXX.SelGate.Tests\GXX.SelGate.Tests.csproj" -c Debug --nologo -m:1
# → 已通过! - 失败: 0，通过: 163，已跳过: 0，总计: 163
```

---

## 9. 否定性断言的计数取证（§37.3，可复跑）

复核报告 §4.3 声称 `Misc`/`FuncForComm`/`IPAddrFilter`/`ConfigManager` 的执法面在
`src/GXX.LoginGate` + `src/GXX.GatewayKit` 中 **0 命中**。本车道复跑该断言（排除本车道新增的
`Rest11` 目录），结果一致：

```powershell
$W='D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p11-logingate-filter'
$files = Get-ChildItem "$W\GXX.CSharp\src\GXX.LoginGate","$W\GXX.CSharp\src\GXX.GatewayKit" `
           -Recurse -File -Filter *.cs |
         Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
foreach($n in @('KickUser','BlockUser','CloseIPConnect','SendGameCenterMsg','CheckAccountName',
                'TProcMsgThread','TAddressInfo','CheckNewIDOfIP','m_fCheckNullSession')){
  $hits = (Select-String -Path $files.FullName -Pattern "\b$n\b" |
           Where-Object { $_.Path -notmatch '\\Rest11\\' }).Count
  '{0,-22} 非Rest11命中={1}' -f $n,$hits
}
# → 全部 9 个名字均为 0 命中（移植前）
```

移植后这些名字**只**出现在 `\Rest11\` 目录下 ⇒ 缺口已被本车道封闭，且**没有**触碰到既有文件。

---

## 10. 边界声明

- 未修改 `GXX.slnx`、任何 `*.csproj`、`Directory.Build.props`、`docs/Checklist.md`、
  `docs/并行派发台账.md`、`docs/并行覆盖审计.md`、`tools/**`；
- `src/GXX.GatewayKit/**` 与 `src/GXX.LoginGate/**` 下的既有文件**只改动 1 个**：
  `src/GXX.LoginGate/LoginGateService.cs`，且**仅在文件末尾追加注释块**（0 行可执行代码改动）；
  `src/GXX.SelGate/**` **零改动**；
- 未在主工作树执行任何 git 写命令；未执行 `merge/rebase/checkout/switch/push/worktree/reset`；
- 所有写文件动作用**绝对路径**；含中文文件一律走 read/edit/write 工具。

---

*报告生成：车道 `p11-logingate-filter`（写车道）；第三条门禁数字与 §6 用例数均可在本工作树复跑。*

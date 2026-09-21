# 并行报告 p14-logingate-wire：LoginGate 执法面**真接线**

> 车道：`p14-logingate-wire`（写车道）
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p14-logingate-wire`（分支 `par/p14-logingate-wire`）
> 基线：`main @ 5a396fce`；本车道切片 0 = `b356d5e1`（切片 1）、`a0001834`（切片 2）
> 承接：`docs/并行报告-p11-logingate-filter.md`（把执法面移植为"可接线"的并存设施）+ `docs/并行报告-p11-logingate-review.md`
> 四道门禁：**全绿**（见 §8）

---

## 0. 结论摘要（先给数字）

| 项 | 数字 |
|---|---|
| 已接线设施数 / 总设施数 | **15 / 17**（§1 的对照表逐项；未接线 2 项见 §7） |
| 修改既有文件 | **4**（`GateService.cs`、`LoginGateService.cs`、`Rest11LoginGateKernel.cs`、`Rest11LoginGateSession.cs`、`Rest11Seams.cs` — 共 5 个，其中 3 个只在 p11 基础上补接口成员） |
| 新增源码文件 | **2**（`GXX.GatewayKit/Rest11/Rest11LoginGatePacketGate.cs`、`GXX.LoginGate/Rest11/Rest11LoginGateEnforcement.cs`） |
| 新增测试文件 | **1** + 1 归档（`Rest14LoginGateWireTests.cs`；`Rest14LoginGateWiringE2E.cs.txt` 归档） |
| 新增用例 | **72 条声明 → 运行时 72 例**（[Fact] 52 + [Theory] 6 → [InlineData] 20） |
| GatewayKit.Tests 总数 | 122 → **194**（全绿） |
| 对既有行为的影响 | **0**（默认 `Rest11Options == null` ⇒ 设施不构造；`SelGate` 163 / `GatewayKit` 既有 122 全绿） |
| 端到端接线验证 | 真实 `LoginGateService` 3 例**实测通过**（默认关闭 / opt-in 装配 / StartService 残部），因 csproj 限制归档为 `.txt`（§7.1） |

**一句话**：p11 交付的 17 项 LoginGate 执法面设施里 **15 项已真正接上**运行路径
（`GateService.CheckIP` 判定链、`LoadConfig` 判定链、客户端帧门控与 `CM_*` 分派、会话进出、计时器巡检），
剩下 2 项因依赖**未移植的 `AcceptExWorkedThread` 原地缓冲**而**显式留痕未接线**（§7）；
全部接线 **opt-in 且默认 OFF**，默认路径行为与接线前逐字节一致。

---

## 1. 接线对照表（设施 → 接线点 → 打开后的行为 → 默认关闭时的等价性证明）

> 口径：本工程的"**可接线 ≠ 已接线**"（台账 §33.4/§50.3）。
> "接线点"一列是**本轮新增/改动的代码位置**；"关闭等价性"一列是**机器可读的证明**
> （测试用例或结构性论证）。

| # | 设施（p11 移植落点） | 接线点（本轮） | 打开后的行为（原文） | 默认关闭时的等价性证明 |
|---|---|---|---|---|
| 1 | 16 字段黑名单**两张表**（`Rest11LoginGateIpFilter.g_BlockIPList` / `g_TempBlockIPList`） | `GateService.CheckIP:218-224`（`:576`）+ `Rest11LoginGateEnforcement.IsBlockIP(string)` | 命中永久表或临时表 ⇒ `return false` ⇒ `ClientIocp.CloseSession` | `Rest11GateHook is { Enabled: true }` 在第一行短路；`DefaultPath_Rest11BranchesAreNeverEntered` + `Rest11Off_SameInputsNeverEnterRest11Checks` |
| 2 | IP 段过滤（`g_BlockIPAreaList` + `IsBlockIPArea`） | `GateService.CheckIP:225-229`（`:587`） | `ReverseIP` 后落在闭区间 ⇒ `return false` | 同上；`Rest11_CheckIp_BlocksIpAreaInclusiveBounds` 断言界内命中/界外不命中 |
| 3 | 每 IP 连接数（`g_ConnectOfIPList` + `OverConnectOfIP`，判据 `Count+1 > Max`） | `GateService.CheckIP:230-234`（`:598`） | `Count+1 > Max` ⇒ `return false`（**超限不自增**） | 同上；`Rest11_CheckIp_OverConnectUsesCountPlusOneAndDoesNotIncrementWhenOver` + `..._NullSessionSwitchOffDisablesConnectionLimit` |
| 4 | 换 ID 频率限制（`CheckNewIDOfIP`，4 秒窗口） | `Rest11LoginGatePacketGate.ProcessClientFrame` 的**调用点已按原文落地**，但原文调用点 `:659` 在 `(* *)` 注释块内 ⇒ **不可达**（见 §7.2） | 4 秒窗口内 `Count > m_nCheckNewIDOfIP` ⇒ true ⇒ 调用点踢线 | `CheckNewIDOfIP_ExceedsThresholdWithinWindow` / `..._WindowExpiryDecaysCount` / `..._SwitchOffAlwaysFalse` / `..._IsUnreachableThroughDispatchBecauseItsDetailBlockIsCommentedOut` |
| 5 | `Misc.pas` 8 个执法例程（`KickUser`×2 / `BlockUser` / `CloseIPConnect` / `ReverseIP` / `AnsiStrToVal` / `SendGameCenterMsg` / `CheckAccountName`） | `Rest11LoginGateEnforcement` 实现 `IRest11EnforcementChannel`，四个副作用出口落到既有设施（`FreeSocket→IocpManager.CloseSession`、`SendOutOfConnection→SendRawToClient`、`AddToBlockIPList/AddToTempBlockIPList→IpFilter`、`AddLog→GateService.SendLog`） | 与原文同（含"`mDisconnect` 分支不封禁"等缺陷照抄） | p11 的 `Rest11LoginGateMiscTests` 17 例 + 本轮 `ProcessClientFrames_KickStopsRemainingFrames` 等 |
| 6 | `ConfigManager.pas` 19 字段 + `[LoginGate]/[Integer]/[Switch]/[Method]` 原文段名 | `GateService.LoadConfig:75-80`（**在既有 5 键之后**）+ `Rest11LoginGateEnforcement.LoadLoginGateConfigSections` | 读/回写 19 字段；与既有 5 键**并存**（不同段名/键名，互不覆盖） | `Rest11GateHook is { Enabled: true }` 短路；`[LoginGate]` 段名与既有 `[Gateway]/[Server]/[PacketRule]` 不重叠（D-P14-05） |
| 7 | `TProcMsgThread.Run` 客户端超时踢线（`FuncForComm.pas:225-233`） | `LoginGateService.StartRest11TickLoop`（后台线程，20ms）→ `Rest11LoginGateEnforcement.OnKeepAliveTick` → `g_ProcMsgThread.Run` | 超时 ⇒ 刷新 tick + `SM_OUTOFCONNECTION` + `BlockUser` + 日志 | `_rest11Wire == null` ⇒ 线程不启动；`Run_ClientTimeoutSendsOutOfConnectionBlocksAndLogs` |
| 8 | `TProcMsgThread.Run` 的 `DelayClose` 到期关闭（`:213-217`） | 同上；`DelayClose` 由 `Rest11GateSessionAdapter.DelayClose` / `PacketGate` | 到期 ⇒ `KickFlag` + 清 `IsDelayClose` + `FreeSocket`（**不封禁**） | 同上；`Run_DelayCloseExpiredFlagsAndFreesWithoutBlocking` + `DelayClose_FromServerSelectServerOk_ThenExpiredByRun` |
| 9 | `TProcMsgThread.Run` 的 `KickFlag` 二次超时分支（`:240-248`） | 同上 | 只日志 + 刷新 + `FreeSocket` | `Run_AlreadyKickedSessionTimesOutWithoutBlocking` + `Run_HandleLoginAtLeastThreeSkipsTimeoutCheck` |
| 10 | `m_fCheckNullSession` 开关（`:184/:214/:242`） | `Rest11LoginGateEnforcement.OnServiceStarted/OnServiceStopping` + `IpFilter` | 关 ⇒ `OverConnectOfIP`/`DeleteConnectOfIP`/`ClearConnectOfIP` 直接 Exit | `Rest11_CheckIp_NullSessionSwitchOffDisablesConnectionLimit` |
| 11 | 会话进出（`g_UserList` 落槽 + `g_ProcMsgThread.AddSession/DelSession`） | `LoginGateService.OnClientAccept/OnClientDisconnect` → `OnClientAccepted/OnClientClosed` | 落槽到 `USER_ARRAY_COUNT(1048)` 定长数组 + 进出处理线程表 | `_rest11Wire?.` 空条件调用；`OnClientAccepted` 首行 `if (!Enabled) return` |
| 12 | `ProcessCltData` 包头门控（超长 / `HTTP/` / `$` / `gDeny` / `Len < 22`） | `LoginGateService.OnClientReceive`（`#`…`!` 切帧）→ `PacketGate.ProcessClientFrame` + `ProcessClientFrames` | 五条门控逐条与原文同（含 `$` **无条件**、超长**严格大于**） | `IsRest11FrameGateActive` 为假 ⇒ 直接走 `base.OnClientReceive`（既有透传）；`ProcessClientFrame_*` 7 例 |
| 13 | `ProcessCltData` 的 `CM_*` 分派（21 个白名单标识 + `else` 踢线） | `PacketGate.DispatchCommand`（宿主四出口经 `IRest11PacketGateHost`） | 白名单转发 / `CM_MACHINEID` 只处理不转发 / `else` 踢线 | `DispatchCommand_WhitelistedIdentsForward`(7) + `..._UnknownIdentKicks` + `..._MachineIdIsHandledWithoutForwarding` |
| 14 | 二级密码（L2）门控（`m_IsCanSetL2Password` / `m_IsCanCheckL2Password`） | `PacketGate.DispatchCommand:489-520` + **上游** `PacketGate.OnServerMessage`（`AppMain.pas:747-752`）+ `LoginGateService.DeliverToClient → NotifyRest11ServerMessage` | 无许可 ⇒ 踢线；有许可 ⇒ 消费并转发（两许可**互不消费**，原文缺陷照抄） | `DispatchCommand_SetL2PasswordWithoutPermissionKicks` / `..._CheckL2Password...` / `..._L2PermissionsDoNotCrossConsume` / `L2Gate_EndToEnd_PermissionFromServerEnablesClientPacket` |
| 15 | `SendGameCenterMsg`（`WM_COPYDATA`，`nParam := MakeLong(Word(tLoginGate), wIdent)`） | `Rest11LoginGateEnforcement.GameCenterChannel`（宿主注入点；未注入时空操作） | 原文 `:280` 无分支（LoginGate 副本） | `Rest11LoginGateMiscTests.SendGameCenterMsg_UsesLoginGateTypeAndIdent`（p11） |
| 16 | `ClientSession.pas:259-401` 协议密码校验块 | ❌ **未接线**（依赖 `m_pOverlapRecv.ABuffer` 原地内存布局） | 原文：校验失败 ⇒ `KickUser` | **显式留痕**：`ProtocolPasswordGateSkipped` + `NotPortedMethods`（`ProcessClientFrame_ProtocolPasswordGateIsNotPortedButTraced`） |
| 17 | `FuncForComm.pas:467-505 KeepAlive` 的**上游心跳** | ❌ **未接线**（依赖 `TIOCPAccepter`/`m_xGameServerList`，属 UI 面） | 原文：遍历游戏服列表发 `'%--$'` + 25 秒超时 | `LoginGateService.SendKeepAlive()` 已提供等价出口（p11 即如此）；登记 §7.3 |

**已接线 15 / 总 17**（未接线 2 项均为"依赖未移植的 `AcceptExWorkedThread` 原地缓冲 / UI 侧"）。

### 1.1 "默认关闭时逐字节一致"的结构性论证

三条硬约束与它们的执行证据：

1. **默认 OFF**：`LoginGateService` 的**默认构造函数**把 `Rest11Options` 置为 `null`
   ⇒ `AttachRest11Kernel()` 首行 `if (Rest11Options == null) return;` 直接返回
   ⇒ `_rest11Wire` 恒为 `null`。
2. **零新分支**：`GateService.Rest11Options` / `Rest11Kernel` 默认返回 `null`，
   `CheckIP`（`GateService.cs:218`）与 `LoadConfig`（`:79`）里的
   `Rest11GateHook is { Enabled: true }` 在第一行短路；`OnClientAccept` / `OnClientDisconnect` /
   `DeliverToClient` 里对 `_rest11Wire` 一律用 `?.` 空条件调用（`null` 即不执行）。
3. **用户可见的证明**：
   - `DefaultPath_Rest11BranchesAreNeverEntered` / `DefaultPath_MaxConnZeroDisablesLimit` /
     `DefaultPath_ExistingBlockListStillWorks`：未接线时既有判据/既有黑名单照旧，Rest11 日志出口零调用；
   - `Rest11Off_SameInputsNeverEnterRest11Checks`：**同一输入**在关闭时不进入任何 Rest11 判定（反例对照）；
   - `SelGate.Tests` **163 全绿**（`GateService.cs` 是**三网关共用**，见 §5）；
   - GatewayKit.Tests 既有 **122 全绿**（原样保留，未改一条断言）。

---

## 2. 落点与依赖方向（为什么分成两半）

| 落点 | 命名空间 | 内容 | 为什么放这里 |
|---|---|---|---|
| `src/GXX.GatewayKit/Rest11/Rest11LoginGatePacketGate.cs` | `GXX.GatewayKit.Rest11` | `ProcessCltData` 的包头门控 + `CM_*` 分派 + L2 门控 + 三态结果枚举 + `IRest11PacketGateHost` / `IRest11GateLogger` 接缝 | `tests/GXX.GatewayKit.Tests` **不引用** `GXX.LoginGate`（csproj 未含该引用且本车道禁止改 csproj）⇒ 只有落在 GatewayKit 才能**直接单测**（本轮 B 项任务的硬要求） |
| `src/GXX.LoginGate/Rest11/Rest11LoginGateEnforcement.cs` | `GXX.LoginGate.Rest11` | 装配层：实现 `IRest11EnforcementChannel` + `GateService.IRest11GateEnforcement` + `IRest11PacketGateHost` + `IRest11GateLogger`，四个副作用出口全部落到既有设施 | 依赖方向保持 **GatewayKit ← LoginGate**（`GateService.Rest11Kernel` 声明为 `object?`，GatewayKit 不引用 LoginGate） |
| `src/GXX.GatewayKit/GateService.cs` | `GXX.GatewayKit` | opt-in 判定链接缝（`Rest11Options` / `Rest11Kernel` / `IRest11GateEnforcement` / `Rest11GateHook`）+ `CheckIP`/`LoadConfig` 的插入点 | 任务指定的接线点；**三网关共用**故必须"关闭时零新分支" |
| `src/GXX.LoginGate/LoginGateService.cs` | `GXX.LoginGate` | 按选项装配 kernel、`StartService`/`StopService` 挂钩、后台巡检线程、`#`…`!` 切帧、服务器侧 `SM_*` 钩子 | 唯一有权决定"是否启用 LoginGate 原文语义"的宿主 |

**"不造第三份实现"（§14.2）**：会话复用 `GateSession`（`Rest11GateSessionAdapter` 只补 LoginGate 独有 4 字段）、
编解码复用 `EDcode`、踢线封禁复用 `Rest11LoginGateMisc.KickUser`、装帧复用
`Rest11LoginGateSession.BuildDefMessageFrame`、黑名单/段表/每 IP 计数复用 `Rest11LoginGateIpFilter`。

---

## 3. 原文语义对齐：三处**必须按原文**的地方

### 3.1 `AcceptExWorkedThread.pas:576/587/598` 的三连判定

原文在 accept 回调里依次 `IsBlockIP` → `IsBlockIPArea` → `OverConnectOfIP`，
命中即 `bClose := True; Exit`（不继续判后面的）。托管映射到 `GateService.CheckIP` 的
**前置提前返回**（既有判据其后；原文 accept 路径里没有既有那套 `_blockIPList`/`_perIP`）。

### 3.2 `ClientSession.pas:462` 的 `m_fHandleLogin := 2` 无条件赋值

```pascal
462:   m_fHandleLogin := 2;
463:
464:   if m_fHandleLogin = 2 then
```
`CltCmd` 在 `:249` 已解出，而 `m_fHandleLogin := 2` 在**分派之前、且不在任何 `if` 内**
⇒ 托管照抄（`DispatchCommand` 的第一条语句）。这条安排让 `FuncForComm.Run` 的超时巡检
（判据 `m_fHandleLogin < 3`）对"已连接但未登录"的会话依然生效。

### 3.3 `ClientSession.pas:219-229` 的 `'$'` **无条件**检查

```pascal
219:  if (Len >= 1) then
220:  begin
221:    if (StrPos(PChar(Addr), '$') <> nil) then
```
与 `:207` 的 `'HTTP/'`（受 `m_fDefenceCCPacket` 约束、且要求 `Len >= 5`）**不同**：
`'$'` 没有任何开关。托管照抄，并有用例把"开关关闭时 `'$'` 仍然踢"锁死
（`ProcessClientFrame_DollarAttackIsKickedEvenWhenDefenceSwitchOff`）。

---

## 4. 原文缺陷（接线层新增的**照抄**清单）

p11 报告的 19 条缺陷全部保留；本轮**在接线层新增锁定**的 7 条：

| # | 缺陷 | 源位置 | 托管落点 | 锁死断言 |
|---|---|---|---|---|
| W1 | `Len > m_nNomClientPacketSize` 是**严格大于**（等于不踢） | `ClientSession.pas:196` | `PacketGate.ProcessClientFrame` | `ProcessClientFrame_OversizeIsStrictlyGreater`（100/101/22 三组 InlineData） |
| W2 | `Len < DEF_BLOCK_SIZE(22)` 直接踢线 | `:242-247` | 同上 | `ProcessClientFrame_ShorterThanDefBlockSizeIsKicked`（21/22） |
| W3 | `'HTTP/'` 要求 `Len >= 5` 且受 `m_fDefenceCCPacket` 约束 | `:207-217` | 同上 | `ProcessClientFrame_HttpAttackDependsOnDefenceCCPacketSwitch` + `..._HttpCheckRequiresLengthAtLeastFive` |
| W4 | `m_fKickFlag` 早退时把标志**反转回 `False`** | `:189-194` | 同上 | `ProcessClientFrame_KickFlagEarlyExit_ReversesFlag` + `ProcessClientFrames_KickFlagEarlyExitCloses` |
| W5 | L2 门控是 `if/else if` 链 ⇒ 两个许可**互不消费** | `:489-520` | `PacketGate.DispatchCommand` | `DispatchCommand_L2PermissionsDoNotCrossConsume` |
| W6 | `CM_CHECKL2PASSWORD` 的日志是**双空格** `'非法检测二级密码:  '` | `:510` | 同上 | `DispatchCommand_CheckL2PasswordWithoutPermissionKicks`（`Assert.Contains("非法检测二级密码:  ")`） |
| W7 | `:582-678` 整块被 `(* *)` 注释 ⇒ `CM_PROTOCOL` **不在**白名单、`CM_ADDNEWUSER`/`CM_IDPASSWORD` 的详细处理不可达 | `:582-678` | 同上 | `DispatchCommand_ProtocolIdentIsNotWhitelistedBecauseItsBranchIsCommentedOut` + `CheckNewIDOfIP_IsUnreachableThroughDispatchBecauseItsDetailBlockIsCommentedOut` |

**照抄而非"顺手修"的声明**：以上 7 条全部**不修**。特别是 W7 —— 本车道**没有**
把 `:582-678` 解注释"恢复功能"，因为那会引入原文**当前不生效**的行为。

---

## 5. 绝不改变 SelGate / RunGate（`GateService.cs` 是**三网关共用**）

`GateService.cs` 的改动**只**包含三类：

1. `Rest11Options` / `Rest11Kernel` / `IRest11GateEnforcement` / `Rest11GateHook`
   —— 新增成员，默认 `null`/`false`，**SelGate/RunGate 不覆写 ⇒ 死代码**；
2. `CheckIP` / `LoadConfig` 里的**提前返回分支**（在第一行短路）；
3. `StartService` / `StopService` 加 `virtual`（**只加修饰符，方法体一字未改**）。

SelGate 侧三处头注要求的差异**未被触碰**：
`SelGateIPAddrFilter.cs:17-25`、`SelGateMisc.cs:11-16`、`SelGateSession.cs:17-24`。
执行证据：

- 本车道对 `GXX.SelGate` 的引用数 = **0**；`GXX.SelGate` 对 `Rest11` 的引用数 = **0**；
- **门禁 3：`GXX.SelGate.Tests` 163/163 全绿**（§8）。

---

## 6. 偏离登记（D-P14-xx）

| 编号 | 性质 | 内容 | 依据 / 影响 |
|---|---|---|---|
| **D-P14-01** | 定时器周期偏离 | `Rest11LoginGateEnforcement.KeepAliveIntervalMs` 默认 **20ms**，原文 `FuncForComm.pas:106 Self.Interval := 1`（1ms） | 1ms 周期在真实服务器上持续占用一个核心；20ms 对 `m_nClientTimeOutTime`（下限 10 秒）与 `DelayClose` 判定等价（误差 <0.2%）。需要逐字对齐时置 1 |
| **D-P14-02** | 切帧落点偏离 | 原文 `'#'…'!'` 切帧在 `AppMain.pas:1070-1127 UserReadBuffer`（属未移植的 AppMain/UI 面）；托管实现为 `LoginGateService.ExtractFrames`（跨 TCP 读保留残片） | 语义与原文一致（找 `#` → 找 `!` → 取两者**之间**、`:1105-1106`）；`UserReadBuffer` 末尾的 `m_nMaxClientPacketCount` 计数踢线 **未接线**（§7.4） |
| **D-P14-03** | 生命周期落点偏离 | 原文 `FuncForComm.pas:358-427 StartService` 是**独立过程**；托管挂在 `LoginGateService.StartService()` **成功之后** | 既有 `GateService.StartService` 先建监听与上游线程，Rest11 残部（`g_fServiceStarted`/`Enabled`/`LoadConfig`/`ClearConnectOfIP`/载入黑名单）在其后执行，**不改基类任何一步** |
| **D-P14-04** | 容量语义偏离 | `g_UserList` 原文 `array[0..1047]`，由 AcceptEx 的 socket 序号决定下标；托管用定长数组按 `SocketId % 1048` 落槽 | 原文"全表同一对象"的特征已在 `Rest11LoginGateSession.FillUserList` 1:1 保留；kernel 侧是运行期索引 |
| **D-P14-05** | 配置落点偏离 | Rest11 的 19 字段用**同一份** `.\Config.ini`（不同段名 `[LoginGate]/[Integer]/[Switch]/[Method]`），并在**既有 5 键之后**加载 | 段名/键名与既有 `[Gateway]/[Server]/[PacketRule]` **零重叠** ⇒ 两者并存、互不覆盖（不需要迁移配置）。`LoginGateService.cs:26` 的既有 `base(@".\Config.ini")` 未改，故相对路径语义与接线前一致 |
| **D-P14-06** | 定时器机制偏离 | 原文 `SetTimer(g_hMainWnd, id, ms, @OnTimerProc)`（Win32 消息定时器）；托管用**后台线程** `LoginGateService.StartRest11TickLoop` | 既有 `GateMainForm` 的 5 秒 `OnKeepAliveTimer` **不参与**本设施（否则 UI 不在时设施失效）；`GateService.OnKeepAliveTimer()` 既有实现一字未改 |
| **D-P14-07** | 观测口径偏离（**仅测试**） | `GateService.CheckIP` 是 `private`，其返回值在单测里不可直接观测 | 探针断言"**判定链的可观测副作用**"（`KernelProbe.LogKinds` 记录命中种类与入参）+ `Rest11LoginGateIpFilter` 的直测。"命中即拦"由上表 #1/#2/#3 的 filter 直测保证 |
| **D-P14-08** | ★ **地址口径（接线正确性的关键）** | 既有 `Share.MakeIPToInt("1.2.3.4")` = **67305985**（0x04030201）与 WinSock `inet_addr`（`Rest11LoginGateNet.InetAddr`）= **16909060**（0x01020304）**逐字节相反** | 原文 `:576/587/598` 传的是 `in_addr.S_addr`（与 `inet_addr` 同口径），而既有 `_perIP`/`_blockList` 用 `MakeIPToInt` ⇒ **Rest11 判定一律以点分字符串入参**，由 `Rest11LoginGateEnforcement` 统一按 `inet_addr` 换算；**绝不**把 `MakeIPToInt` 结果直接喂给 `IPAddrFilter`（否则黑名单与段表全部失效）。`IpAddressConventions_InetAddrAndMakeIpToIntAreByteReversed` 把这个反相关锁死 |
| **D-P14-09** | 日志接缝双轨 | `IRest11EnforcementChannel.AddLog/CheckLevel`（p11 执法面用）与 `IRest11GateLogger`（本轮包头门控用）并存 | 两者都指向同一既有出口（`GateService.SendLog` + `m_nShowLogLevel` 门）；`PacketGate.Log` **只**走 `Logger`，避免双写与门控失配 |
| **D-P14-10** | ★ 版本号门控的部分移植 | `AppMain.pas:524` 的 `GetValidStr3(sRecv, sMachineID, ['/'])` 依赖 `:523 DecodeString(sRecv)`，而 `DecodeString` 的**入参口径由未移植的 `:259-401` 决定** | 采用**双路探测 + 只认可打印 ASCII**：两条路都取不到版本号时返回 `""` ⇒ 上层按"版本不符"走 `SM_CHECKCLIENTVERSION_FAIL`（**不静默放行**）。实测粒度是"第一个 `/` 之前"⇒ `"2025/04/28/machine"` 的版本号是 **`"2025"`**（`ExtractSoftVersion_TakesFirstSlashSegment` 锁死） |
| **D-P14-11** | 接口成员加宽（p11 文件的必要改动） | `IRest11SessionObj` 新增可写的 `HandleLogin`/`SvrObject`/`m_IsCanSetL2Password`/`m_IsCanCheckL2Password` 与 `DelayClose(uint)` | `ClientSession.pas:462` 的无条件赋值与 `:502/:518` 的许可消费**必须**能写；`Rest11GateSessionAdapter` / `Rest11LoginGateSession` 同步实现。因此 `tests/GXX.GatewayKit.Tests/Rest11LoginGateIpFilterTests.cs` 的 `FakeSession` 也补了这几个成员（**该文件属 p11 分区**，此处登记为不得不动的跨分区编辑） |
| **D-P14-12** | p11 伪造调用点删除 | `Rest11LoginGateKernel.OnClientAccepted` 原有一行 `g_CurrIPaddrList.Add(session.IPText)` 并注释成 `AppMain.pas:762`，但原文该表属 **AppMain.pas:760-980 的服务器消息记账块**（未移植），**不由客户端接入填充** | 删除该行以免制造不存在的语义；`g_CurrIPaddrList` 字段保留（原文 `FuncForComm.pas:78` 的全局量），待 AppMain 记账块移植后由它使用 |

---

## 7. 未完成 / 阻塞（如实登记）

### 7.1 ★ 阻塞：`tests/GXX.GatewayKit.Tests.csproj` **没有** `GXX.LoginGate` 引用

任务书 §"已解除的阻塞"说集成方本轮已给
`tests/GXX.GatewayKit.Tests/GXX.GatewayKit.Tests.csproj` 加入 `GXX.LoginGate` 项目引用，
但**实测该 csproj 在本车道基线上只有 `GXX.GatewayKit` + `GXX.RunGate` 两条引用**：

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\GXX.GatewayKit\GXX.GatewayKit.csproj" />
  <ProjectReference Include="..\..\src\GXX.RunGate\GXX.RunGate.csproj" />
</ItemGroup>
```

因此本车道**无法**在测试工程里直接引用 `GXX.LoginGate` 类型（任务 B 项要求"对 kernel 写直接单测"
只能通过**把 kernel 逻辑落到 GatewayKit** 来满足，见 §2）。

**本车道已做的绕行**：把 `ProcessCltData` 包头门控 + `CM_*` 分派 + L2 门控整体落到
`GXX.GatewayKit.Rest11.Rest11LoginGatePacketGate`，从而在**不改 csproj** 的前提下拿到
71 例直接单测。

**已实测但归档的端到端用例**：本车道**临时**在 csproj 加入该引用并跑通
**真实 `LoginGateService` 的 3 个端到端接线用例**（GWK 总数 194 → 197，**全绿**），
随后**按要求撤回 csproj 改动**。用例源码与实测结论归档在
`tests/GXX.GatewayKit.Tests/Rest14LoginGateWiringE2E.cs.txt`（不参与编译），
集成方补上引用后去掉 `.txt` 后缀即可启用。实测结论：

| 用例 | 结论 |
|---|---|
| `DefaultConstructor_DoesNotWireRest11` | `new LoginGateService(null, ini)` ⇒ `Rest11Options == null` **且** `Rest11Wire == null`（设施根本不存在） |
| `OptInConstructor_WiresKernelAndLoadsOriginalSections` | `new LoginGateService(Rest11LoginGateOptions.All, ini)` ⇒ kernel 装配、`Enabled == true`；`[LoginGate]`/`[Integer]` 原文段名已加载（`ShowLogLevel=6`/`MaxConnectOfIP=3`/`CheckNewIDOfIP=2`/`ClientTimeOutTime3=20000`）；接缝三条判定可调用 |
| `OptIn_StartService_RunsResidualStartup` | `StartService()` ⇒ `g_fServiceStarted == true`、`g_ProcMsgThread.Enabled == true`、两张黑名单文件路径可用 |

**建议**：集成方在 `GXX.GatewayKit.Tests.csproj` 加一行
`<ProjectReference Include="..\..\src\GXX.LoginGate\GXX.LoginGate.csproj" />`，
并把归档文件去后缀入树（本车道已实测该改动无副作用：SelGate 163 / LoginSrv 229 不受影响）。

### 7.2 换 ID 频率限制（`CheckNewIDOfIP`）**不可达**（原文如此）

设施本身已 1:1 移植且已接线（`PacketGate.ProcessClientFrame` 的调用口径与原文同），
但原文的**唯一调用点** `ClientSession.pas:659 if CheckNewIDOfIP(...)` 位于
`:582-678` 的 `(* *)` 注释块内 ⇒ 当前**没有任何路径**能触发它。
`CheckNewIDOfIP_IsUnreachableThroughDispatchBecauseItsDetailBlockIsCommentedOut` 锁定该事实。
想要真正生效必须先解注释 `:582-678`（那属于"恢复原文未启用代码"，**不应**由本车道顺手做）。

### 7.3 `FuncForComm.pas:467-505 KeepAlive` 的上游心跳**未接线**

依赖 `FormMain.m_xGameServerList` + `TIOCPAccepter`（`AcceptExWorkedThread`/`IOCPManager` 的 UI 侧），
属 `p11-logingate-ui` 车道。`LoginGateService.SendKeepAlive()`（既有）已提供等价出口。

### 7.4 `UserReadBuffer` 的 `m_nMaxClientPacketCount` 计数踢线**未接线**

原文 `AppMain.pas:1116-1120`：一次读缓冲里处理的消息数达到
`g_pConfig.m_nMaxClientPacketCount` 即 `KickUser` 并 `Break`。
托管 `LoginGateService.ExtractFrames` 已按 `#`…`!` 切出**全部**帧并逐帧分派（`:1086-1124` 的 `LOOP:`），
但**没有**在计数达到上限时中止并踢线 ⇒ 登记为 D-P14-02 的未接线部分。
（`m_nMaxClientPacketCount` 字段本身已移植并在 `[Integer]` 段落盘。）

### 7.5 `ClientSession.pas:157-401` 的协议密码校验块（任务 C）**未完成**

逐字节依赖 `m_pOverlapRecv.ABuffer` 的原地内存布局（`PAnsiChar(Addr)` 指针算术、
`PByte(Addr + Len)^ := 0` 写终止符、`DecryptDes(sRecv[1], ...)` 原地解密），
而 `AcceptExWorkedThread.pas` 在本仓 **0 托管声明**（p11 复核报告 §4.2）。
缺它的后果已**显式留痕**（`ProtocolPasswordGateSkipped` 计数 + `NotPortedMethods`），
**不**裸放行、**不**误杀：

- 原文只有 `IsPackError`（`:394-401`）才因校验失败踢线 ⇒ 未移植时"不踢"与上游未移植时的既有透传行为一致；
- `:394-401` 分支**当前不可达**；
- `ProcessClientFrame_ProtocolPasswordGateIsNotPortedButTraced` 锁定留痕。

### 7.6 其余未接线项

- `Misc.pas` 的 `OverSpeed`/`DefenceCCPacket`/`KickOverSpeed` 三类开关的使用点在
  未移植的 `ProcessCltData`/`ClientThread` 里（p11 报告 §7.5，本轮不变）；
- `ClientSession.FillUserList`（`g_UserList` 全表同一对象）与 `CleanupUserList` 未接线
  （kernel 侧用 `SocketId % 1048` 落槽，见 D-P14-04）；
- `LogManager.pas`（`g_pLogMgr`）本体未移植，日志经 `IRest11GateLogger` 落到既有 `SendLog`。

---

## 8. 门禁输出（四条，全绿）

```powershell
$W='D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p14-logingate-wire'

# 门禁1：整解构建（0 error）
dotnet build "$W\GXX.CSharp\GXX.slnx" -c Debug --nologo -m:1 -p:BuildInParallel=false
# → 已成功生成。  182 个警告  0 个错误

# 门禁2：GatewayKit.Tests（含 Rest11* 122 例 + 本轮 Rest14* 72 例）
dotnet test "$W\GXX.CSharp\tests\GXX.GatewayKit.Tests\GXX.GatewayKit.Tests.csproj" -c Debug --nologo
# → 已通过! - 失败: 0，通过: 194，已跳过: 0，总计: 194

# 门禁3：SelGate.Tests（证明没碰坏 SelGate）
dotnet test "$W\GXX.CSharp\tests\GXX.SelGate.Tests\GXX.SelGate.Tests.csproj" -c Debug --nologo
# → 已通过! - 失败: 0，通过: 163，已跳过: 0，总计: 163

# 门禁4：LoginSrv.Tests（LoginGate 邻居）
dotnet test "$W\GXX.CSharp\tests\GXX.LoginSrv.Tests\GXX.LoginSrv.Tests.csproj" -c Debug --nologo
# → 已通过! - 失败: 0，通过: 229，已跳过: 0，总计: 229
```

**额外（临时加 csproj 引用时实测，随后撤回）**：
`GatewayKit.Tests` → **197** 例全绿（含真实 `LoginGateService` 端到端 3 例，见 §7.1）。

---

## 9. 新增用例明细（72 例）

单个文件 `tests/GXX.GatewayKit.Tests/Rest14LoginGateWireTests.cs`：

| 分组 | 内容 | 用例数 |
|---|---|---|
| A-1 默认 OFF 等价性 | `DefaultPath_Rest11BranchesAreNeverEntered` / `..._MaxConnZeroDisablesLimit` / `..._ExistingBlockListStillWorks` / `Rest11Off_SameInputsNeverEnterRest11Checks` | 4 |
| A-2 `CheckIP` 判定链 | 永久/临时黑名单、IP 段闭区间、`Count+1>Max` 不自增、`m_fCheckNullSession` 开关 | 5 |
| B-1 `ProcessCltData` 包头门控 | `KickFlag` 反转、超长（3 组）、`Len<22`（2 组）、`'$'` 无条件、`'HTTP/'` 两态、`Len>=5` 前提、`gDeny`、协议密码留痕 | 12 |
| B-2 `CM_*` 分派 | 无条件 `HandleLogin=2`、L2 两向门控、互不消费、服务器侧置位、端到端两态、`else` 踢线、白名单（7 组）、`CM_PROTOCOL` 不在白名单、`CM_MACHINEID` 只处理、版本不符/相符、版本号粒度（3 组）、日志门控 | 24 |
| B-3 逐帧分派 | 白名单逐帧转发、`CM_MACHINEID` 不转发不关、踢线即停、`KickFlag` 早退 | 4 |
| B-4 换 ID 频率 | 阈值、窗口过期 `Dec`、开关关闭、不可达 | 4 |
| B-5 超时 / `DelayClose` | 超时踢线、`DelayClose` 到期、端到端、二次超时、`HandleLogin>=3` 跳过、`g_fServiceStarted` 早退、`LastGameSvrActive=false` 跳过 | 7 |
| B-6 黑名单增删 | 两重载去重键、两模式去重、两张表并存、落盘往返、IP 段落盘/非法跳过、非法行跳过 | 8 |
| C 地址口径 | `InetAddr` ↔ `MakeIPToInt` 逐字节相反（3 组）、反转不动点（2 组） | 5 |
| 脚手架自检 | `EncodedBody` ↔ `DecodeString` 往返 | 1 |
| 结构性契约 | `UnoverriddenGatewayBase_ExposesNoRest11Hooks`（**SelGate/RunGate 死代码**证明） | 1 |
| 合计 | `[Fact] 52 + [Theory] 6 → [InlineData] 20` | **72** |

**每条设施都有反例**：所有"A-2 打开后命中"的用例都有对应的"A-1 关闭时同一输入不命中"；
所有"原文缺陷照抄"的用例都写明 `// 原文如此` / `:行号`。

---

## 10. 边界声明

- 未修改 `GXX.slnx`、`Directory.Build.props`、`docs/Checklist.md`、`docs/并行派发台账.md`、
  `docs/并行覆盖审计.md`、`tools/**`；
- **未修改任何 `*.csproj`**（§7.1 的临时验证已撤回，`git diff` 为空）；
- `src/GXX.SelGate/**` **零改动**；
- 未在主工作树执行任何 git 写命令；未执行 `merge/rebase/checkout/switch/push/worktree/reset`；
- 所有写文件动作用**绝对路径**；含中文文件一律走 read/edit/write 工具；
- 仓库根目录无探查残留（临时工具 `.p14-splice*` 已在切片 1 前清理，`git status --porcelain` 复核）。

---

*报告生成：车道 `p14-logingate-wire`（写车道）；§8 的四条门禁数字与 §9 的用例数均可在本工作树复跑。*

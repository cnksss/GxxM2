# 并行报告 · 车道 `p9-rungate-rest`（GXX Delphi7→C# · RunGate IOCP 余部四单元）

> 分支：`par/p9-rungate-rest` ｜ 工作树：`.worktrees/p9-rungate-rest`
> 基线：`main @ b48e4c43`（车道开工时的 HEAD）
> 规程：`docs/转换开发文档.md`、`docs/并行派发台账.md` §3/§18.5/§37.3（另参考 §14.2/§35.4）、`docs/并行报告-p2-rungate-impl.md`
> 独占分区：`src/GXX.RunGate/Rest9/**`、`tests/GXX.RunGate.Tests/Rest9*.cs`、本文件

---

## 0. 交付摘要（TL;DR）

| 项 | 结果 |
|---|---|
| 裁定 | **4 个单元全部「不移植 + 证据」**，0 个 1:1 移植 |
| 新建源码 | `GXX.CSharp/src/GXX.RunGate/Rest9/Rest9NotPortedEvidence.cs`（证据数据 + 3 个纯判定函数，**无运行时算法**） |
| 新建测试 | `GXX.CSharp/tests/GXX.RunGate.Tests/Rest9NotPortedEvidenceTests.cs`（**31 例，全绿**） |
| 新增测试用例 | **31** |
| 改动文件数 | **3**（2 新建 + 本报告） |
| `dotnet build GXX.slnx -c Debug` | **0 error**（165 warnings，均来自既有文件） |
| `dotnet test tests\GXX.RunGate.Tests\...` | **1922 通过 / 0 失败 / 0 跳过** |
| 与 GatewayKit 的逐函数对照 | §2 |
| 原文缺陷/易错点 | §3（7 条） |
| 偏离登记 D-P9-xx | §4（6 条） |
| 未完成/阻塞项 | §6 |

**行数口径**：一律**实测物理 LF**（`File.ReadAllLines(...).Length`），并有用例
`AllFourUnits_PhysicalLineCounts_MatchMeasuredReality` 逐单元锁定：

| 单元 | 任务书 | 实测 LF | 一致？ |
|---|---|---|---|
| `Common/IODataPool.pas` | 586 | **586** | ✅ |
| `Common/Qos.pas` | 293 | **293** | ✅ |
| `Common/IocpTcpClient.pas` | 340 | **340** | ✅ |
| `DllUpdateCommon.pas` | 10 | **10** | ✅ |
| 合计 | 1,229 | **1,229** | ✅ |

---

## 1. 编译期开关：先钉死"哪一半是活代码"

`Source/RunGate/Common/iocp.inc` 全文只有两条指令：

```pascal
{$DEFINE SHARE_POOL_MODE}      // ← 生效
{.$DEFINE USE_SPINLOCK}        // ← 注释掉了，不生效
```

因此本车道四个单元的取值一律按「`SHARE_POOL_MODE` 开、`USE_SPINLOCK` 关」判定：

| 开关 | 值 | 对四单元的影响 |
|---|---|---|
| `SHARE_POOL_MODE` | **开**（`iocp.inc:2`） | `TIODataPool.Instance` 单例（`IODataPool.pas:37/63/566-584`）**是活代码**；`iocpTcpClient.pas:214-218` 走 `TIODataPool.Instance` 分支 |
| `USE_SPINLOCK` | 关（`iocp.inc:4` 是 `{.$DEFINE}`） | `TIocpCriticalSection.Create` **不传**名字参数（`IODataPool.pas:88` 的 `{$IFDEF USE_SPINLOCK}'TIODataPoolLocker'{$ENDIF}` 为空） |
| `UseIocpClient` | **1**（`IocpCommon.pas:14`） | `IODataPool.pas` 里 4 处 `{$IF UseIocpClient <> 0}` **全部生效**（`:23-26/127-150/227-242/276-318/390-425`）；`IocpTcpClient` 是活单元（`RunGateUtils.pas:9` 的 `{$IF UseIocpClient <> 0} IocpTcpClient, {$IFEND}`） |
| `MAX_IOCP_CLIENT_RECV_BUFFER_SIZE` | `1 shl 20` = 1,048,576（`IocpCommon.pas:17`） | 第二档池（60 块 × 1MB）存在 |

> 结论：四单元**没有一个**是因条件编译而整体死掉的"僵尸单元"。
> 它们"不移植"的理由**不是**"编译不进去"，而是"**其消费者本身已被 .NET 取代**"——见 §2。

---

## 2. 逐单元处置 + 与 GatewayKit 的逐函数对照

### 2.0 为什么本车道不做 1:1 移植（判定链，可核查）

```
Qos.pas ──────────────► IocpWinsock2.pas（唯一引用者）
IODataPool.pas ───────► IocpUtils.pas / IocpTcpServer.pas / IocpTcpClient.pas / MirClientContext.pas
                                      └─ 全部经 ─► TIocpCore / TIocpContext（IocpUtils.pas:34/136）
IocpTcpClient.pas ────► TIocpCore + TIocpContext（同上，未移植）
DllUpdateCommon.pas ──► ∅（无引用者）
```

**链条的根**是 `TIocpCore`/`TIocpContext`（`IocpUtils.pas` 的 IOCP 管道本体）。
该单元已被**前一条同区车道 `p2-rungate-impl` 裁定为「管道本体不移植」**
（`docs/并行报告-p2-rungate-impl.md` **:80**：`Common/IocpUtils.pas` → "管道本体不移植 + 新建纯策略"，
理由是 `GatewayKit/GatewayProtocol.cs` 的 `IocpManager`（`SocketAsyncEventArgs`，底层即 Windows IOCP）等价覆盖）。
该报告同时也对下游三单元给出了初判：
**`:102`**（`IocpTcpClient` → `GatewayKit/TcpLink.cs` 等价覆盖）、
**`:103`**（`IODataPool` → 不移植，托管侧无 `OVERLAPPEDEx` 池需求）、
**`:104`**（`Qos` → 不移植，仅被 `IocpWinsock2.pas:79` 引用）。
但 **`:102`–`:104` 三条都没有给出取证**（台账 §18.5 第 6 条要求"证据"），
`DllUpdateCommon` 更只有一句"仅在 `RunGate_LEG.dproj` 出现"（`:114`，见 §3-7 实测反证）。
**本车道的职责正是给这四条补上可复现的证据，并复核初判是否站得住。**

**本车道遵守该裁定**：不在其下游另造一份池/连接器，否则就是台账 §14.2「不造第三份实现」所禁的
"两份正确但语义不同的实现"（§35.4「已移植但零生产调用方 = 没接上」同样适用）。

### 2.1 `Common/IODataPool.pas`（586 LF）—— **不移植**（被 GatewayKit 取代 + 唯一非管道用处是 UI 计数）

| 项 | 值 |
|---|---|
| 已移植方法数 / 总方法数 | **0 / 11** |
| 其中原文已注释掉（`{ }` 内，非活代码） | 2（`GetNewSendBufer:489`、`GiveBackSendBuffer:536`）⇒ 活方法 **9** |
| 原文**未**注释但恒无调用者的辅助过程 | `DivMod:68`（见 §3-1） |

**逐函数对照**（证据：`Rest9NotPortedEvidence.IODataPoolMembers`，用例逐行号核对）

| # | `TIODataPool` 成员 | 原文行 | 对应的既有 .NET/GatewayKit 设施 | 覆盖判定 |
|---|---|---|---|---|
| 1 | `Create`（建 41 档 × 1024 块预分配） | 82 | `IocpManager.ProcessAccept`：每连接 `new byte[DATA_BUFSIZE]`（`GatewayProtocol.cs:218`） | 取代（托管无池化需求） |
| 2 | `Destroy` | 174 | `GateSession`/`SocketAsyncEventArgs` 由 GC 回收；`IocpManager.Dispose` | 取代 |
| 3 | `Clear` | 200 | `IocpManager.Stop`（清会话表） | 取代 |
| 4 | `GetNewIOData`（尺寸档取块，档 = `ceil(MaxSize/128)`） | 266 | 无对应；`SocketAsyncEventArgs.SetBuffer` 每次分配 | **无等价物**（登记见 §4 D-P9-01） |
| 5 | `GetCount` | 368 | 无对应 | 无 |
| 6 | `GetUseCount` | 373 | 无对应 | 无 |
| 7 | `GetNoUseCount` | 378 | 无对应 | 无 |
| 8 | `GiveBackIOData`（归还；≥500MB 时随机释放一整档） | 383 | 无对应（GC 自动） | 无 |
| 9 | `GetNewSendBufer` | 489 | — | **原文已在 `{ }` 内（死代码）** |
| 10 | `GiveBackSendBuffer` | 536 | — | **原文已在 `{ }` 内（死代码）** |
| 11 | `Instance`（`SHARE_POOL_MODE` 单例） | 567 | 无对应 | 无 |

**"没有可复用等价物"的定量证据**（用例 `IODataPool_ManagedReplacement_HasNoPoolApi_SoThereIsNothingToReuse`）：
`GatewayProtocol.cs + TcpLink.cs` 的公开 API 共 23 项（清单见证据类的 `ManagedGatewayKitPublicApi`），
其中 **0 项**含 `GetNewIOData`/`GiveBackIOData`/`AllocMemSize`/`MaxUseMemSize`/`NoUseCount`/`POVERLAPPEDEx`。

**调用点证据**（用例 `IODataPool_AllCallers_AreTheFourUnits_AndEveryOneOfThemExists`，全树 `.pas` 扫描，**6** 个文件命中）

| 引用文件 | 用法 | 是否管道内部 |
|---|---|---|
| `Common/IocpUtils.pas` | `uses:9`；`TIODataPool.Instance` / `FIODataPool`（`:370/389/447/466`） | 是（`TIocpCore` 本体） |
| `Common/IocpTcpServer.pas` | `uses:9`；`FIODataPool: TIODataPool`（`:142/609/649`） | 是（服务端） |
| `Common/IocpTcpClient.pas` | `uses:9`；`TIODataPool.Instance.GetNewIOData(0)`（`:215/217`） | 是（客户端） |
| `MirClientContext.pas` | `uses:9` | 是（M2 链路上下文） |
| `uFrmMain.pas` | `uses:18`；**6 个统计标签**（`:2240/2241/2243/2244/2245`） | **否** ← 唯一非管道用处 |
| `Common/IODataPool.pas` | 单元自身 | — |

> ★ `uFrmMain.pas:2240-2245` 把 `UseCount/NoUseCount/MaxUseCount/MaxAllocMemSize/AllocMemSize`
> 显示到标签上。这条**不是**"被 .NET 取代"，而是"**数据源消失**"——
> 已作为缺口登记（§4 D-P9-02），并给出 GatewayKit 侧的替代计数器清单，供主窗体车道决定是否复刻。

**原文缺陷**：见 §3-1 / §3-3 / §3-4。

### 2.2 `Common/IocpTcpClient.pas`（340 LF）—— **不移植**（被 `GatewayKit/TcpLink` 取代）

| 项 | 值 |
|---|---|
| 已移植方法数 / 总方法数 | **0 / 19**（`TIocpRemoteContext` 7 + `TIocpTcpClient` 12） |

**逐函数对照**（证据：`Rest9NotPortedEvidence.IocpTcpClientMembers`）

| # | 原文成员 | 原文行 | `TcpLink` 对应物 | 覆盖判定 |
|---|---|---|---|---|
| 1 | `TIocpRemoteContext.Create` | 88 | `TcpLink(string host, int port)` / `TcpLink(Socket)` | 取代 |
| 2 | `.Destroy` | 94 | `TcpLink.Dispose`（内部 `Close()`） | 取代 |
| 3 | `.CloseContextSocket` | 100 | `TcpLink.Close` | 取代 |
| 4 | `.DoConnect` | 105 | `TcpLink.Connect()` 成功后触发 `OnConnected` | 取代 |
| 5 | `.DoDisconnect` | 111 | `ProcessReceive` 里 `OnDisconnected` | 取代 |
| 6 | `.DoReset` | 117 | `TcpLink.Close`（重置 `_accumLen`） | 取代 |
| 7 | `.SetActive`（**含整套 Win32 机械**） | 123 | `TcpLink.Connect`（**阻塞 connect**，非 `ConnectEx`） | 功能等价、机制不同（§4 D-P9-03） |
| 8 | `TIocpTcpClient.Create` | 241 | 无（`TcpLink` 不需要 `TIocpCore` 聚合） | 取代（结构不同） |
| 9 | `.Destroy` | 254 | `Dispose` | 取代 |
| 10 | `.ClearContexts` | 265 | 无（每连接一个 `TcpLink` 实例） | 取代 |
| 11 | `.Add` | 276 | `new TcpLink(host, port)` | 取代 |
| 12 | `.GetCount` | 291 | 无（调用方自持集合） | 取代 |
| 13 | `.GetItems`（越界返回 `nil`） | 296 | — | 取代（集合由调用方管） |
| 14-18 | `SetOnContextConnect` / `SetOnContextDisconnect` / `SetOnError` / `SetOnRecvDataBuffer` / `SetOnSendDataBuffer`（每个都是"存字段 + 下推给 `FIocpCore`"） | 304-332 | `TcpLink.OnConnected` / `OnDisconnected` / `OnReceive`（**事件**，无 `OnError`） | 取代（`OnSendDataBuffer` 无对应） |
| 19 | `.RegisterContextClass` | 334 | 无（`TcpLink` 非 `class of` 工厂模式） | 取代（结构不同） |

**调用点证据**（用例 `IocpTcpClient_AllCallers_AreRunGateUtilsAlone`）：
`TIocpTcpClient`/`TIocpRemoteContext` 在 RunGate **只被 `RunGateUtils.pas` 一个文件**引用（对 6 个候选文件逐一扫描，命中数 = 1）；
且 `uses` 在**活分支**里（`RunGateUtils.pas:9` = `{$IF UseIocpClient <> 0} IocpTcpClient, {$IFEND}`，`UseIocpClient = 1`）。
6 条调用点原文逐行核对通过（用例 `IocpTcpClient_EveryRegisteredCallSiteInEvidenceExistsVerbatim`）：

```
RunGateUtils.pas:3509  FIocpClient := TIocpTcpClient.Create(nil);
RunGateUtils.pas:3510  FIocpClient.RegisterContextClass(TMirRemoteContext);
RunGateUtils.pas:1422  FTcpClient := TMirRemoteContext(AOnwer.FIocpClient.Add);
RunGateUtils.pas:3621  for I := 0 to FIocpClient.Count - 1 do
RunGateUtils.pas:3623  TIocpRemoteContext(FIocpClient.Items[I]).Active := False;
RunGateUtils.pas:1502  FTcpClient.Active := True;      // → SetActive
```

**`SetActive` 为什么不能"只移植这一段"**（用例 `IocpTcpClient_SetActive_...`，逐条计数取证）：

| Win32 调用 | 全单元出现次数 |
|---|---|
| `WSASocket` | 2 |
| `Bind` | 2 |
| `CreateIoCompletionPort` | 1 |
| `SIO_GET_EXTENSION_FUNCTION_POINTER` | 1 |
| `WSAID_CONNECTEX` | 3 |
| `inet_addr` | 1 |
| `htons` / `htonl` | 2 / 2 |
| `IocpConnectEx` | 6 |
| **合计** | **20** |

这 20 处全部落在 `SetActive`（`:123-237`）一个方法体内 ⇒ 它是"整条自研 IOCP 客户端栈"的入口，
抽出任何真子集都会得到一份**半截实现**。`TcpLink` 是**活设施**（用例断言 ≥3 个生产调用方，
含 `src/GXX.DBServer/IDSocCli.Adapter.cs`）⇒ 不需要第二份。

### 2.3 `Common/Qos.pas`（293 LF）—— **不移植**（第三方头文件翻译，宿主已不移植）

| 项 | 值 |
|---|---|
| 已移植方法数 / 总方法数 | **0 / 0**（可执行方法数为 0） |
| `const` 常量 | **27** |
| `type` 声明/别名 | **22**（5 个 `type` 段、4 个 `const` 段） |
| `{$EXTERNALSYM}` | **40** = 27 常量 + 13 类型/记录（另 9 个纯别名无 `EXTERNALSYM`） |
| `implementation` 段 | **空**（用例断言段内无 `procedure`/`function`/`class`） |

> 注意：任务书把 `Qos.pas` 描述为"流量整形/限速"。
> **实测该描述只对了一半**：整形（shaping / `QOS_OBJECT_SHAPING_RATE` / `TC_NONCONF_*`）**只有常量和记录声明**，
> 没有任何可执行代码。真正会"限速"的是 `IocpUtils.pas` 的发送分块与 `GateShare.pas` 的节流逻辑，不在本单元。

**真实性核查（★ 本车道自我纠错 1）**：初版我按"`Qos` 只在 `IocpWinsock2.pas` 出现 ⇒ 引用方不用它"登记，
被取证用例当场否掉——`IocpWinsock2.pas` **确实在用** Qos 的类型：

| Qos 符号 | 在 `IocpWinsock2.pas` 的出现次数 | 用处 |
|---|---|---|
| `FLOWSPEC` | **7** | `:1603` 注释；`:1620/:1621` `_QualityOfService.SendingFlowspec/ReceivingFlowspec` 两个字段；另 3 处（`WSA_QOS_EFLOWSPEC` 等子串 2 处） |
| `SERVICETYPE` | **2** | `WSA_QOS_ESERVICETYPE` 子串 |

其余整族（`QOS_OBJECT_HDR` / `QOS_SD_MODE` / `QOS_SHAPING_RATE` / `TC_NONCONF_*` / `QOS_GENERAL_ID_BASE` /
`QOS_NOT_SPECIFIED` / `POSITIVE_INFINITY_RATE` / `TFlowSpec` / `TServiceType` / `PServiceType`）
在 `IocpWinsock2.pas` 里 **0 命中**（用例 `Qos_OnlyFlowSpecAndServiceTypeReachIocpWinsock2_AndNothingElse`）。

**裁定仍然成立**，理由链更硬了：`Qos.pas` 的**唯一**消费者 `IocpWinsock2.pas` 已裁定不移植
（约 2,950 行 WinSock2 声明 → `System.Net.Sockets` 覆盖），而 `_QualityOfService`/`WSAIoctl(...,QOS)` 这条
"给套接字设 QOS"的路在托管侧没有对应语义 ⇒ 移植 `Qos.pas` 会产出 **0 调用方的死代码**（§35.4）。

**C# 侧存量核查**（用例 `Qos_AlreadyPortedPart_...` / `Qos_FortyTwoSymbols_HaveLiteralZeroCSharpHits`，扫描 1,358 个 `.cs`）：

| 项 | 数 | 说明 |
|---|---|---|
| Qos.pas 名字总数 | **49** | 27 常量 + 22 类型 |
| C# 侧**已有**（同名落地） | **7** | 全部是 `SERVICETYPE_*`，来自**另一份同源翻译** `Source/Client-HGE/WinSock2.pas:724-730` → `src/GXX.Client/Tail/WinSock2Constants.cs`（各出现 2 处：常量本体 + `TailWinSock2Golden.g.cs` 金值表） |
| C# 侧**0 命中** | **42** | 含全部 `QOS_OBJECT_*`/`TC_NONCONF_*`/记录族/`SERVICE_*`/`QOS_NOT_SPECIFIED` 等 |
| C# 侧记录/结构体 | **0** | `QOS_OBJECT_HDR`/`QOS_SD_MODE`/`QOS_SHAPING_RATE`/`TQOSObjectHdr`/`TQOSSDMode`/`TFlowSpec`/`PFLOWSPEC`/`LPFLOWSPEC` 均 0 命中 |
| `FLOWSPEC` 的 C# 命中 | 仅子串 | 全是 `WSA_QOS_EFLOWSPEC`/`WSA_QOS_EPSFLOWSPEC`（**另一个**常量），用例逐条断言 `StartsWith("WSA_QOS_E")` |

> ★ 自我纠错 2/3：初版把"const 段 5 处"和"C# 0 命中 32 条"写进证据，实测分别是 **4** 和 **42**。
> 两处都由取证用例算出来并已改正（这正是"否定性断言必须计数取证"的价值）。

### 2.4 `DllUpdateCommon.pas`（10 LF）—— **不移植**（空壳孤儿单元）

原文全文 10 行：`unit DllUpdateCommon;` / `interface` / `uses` / `Windows;` / `implementation` / `end.`。

| 项 | 值 | 取证 |
|---|---|---|
| 声明数（`type`/`const`/`var`/`procedure`/`function`/`class`/`record`/`property`） | **0** | 用例正则计数 = 0 |
| `interface`..`implementation` 段内容 | **只有 `uses\n  Windows;`** | 用例断言去空行后恰为这两行 |
| `implementation`..`end.` 段内容 | **空** | 用例断言剥注释后为空白 |
| 被 `*.dpr`/`*.dproj`/`*.dpk` 引用 | **0**（扫过 ≥20 个工程文件） | 用例 `DllUpdateCommon_IsNotReferencedByAnyProjectFile` |
| 被任何 `.pas` 的 `uses` 引用 | **0**（扫过 RunGate ≥40 个 `.pas`） | 用例 `DllUpdateCommon_IsNotInAnyPascalUsesClause`（用**精确 token** 匹配，`UsesClauseContainsUnit`） |
| 除原文自身外出现在何处 | **只在文档/映射表**（`.md`/`.tsv`） | 用例 `DllUpdateCommon_AppearsOnlyInDocumentation_NotInCode` |
| `RunGate.dproj` 的 22 条 `DCCReference` | 不含它 | 见 §2.5 |

**判定**：`uses Windows;` 后面没有任何 `interface` 段内容，因此这条 `uses` **既不导出也不使用任何符号**
⇒ 该单元是**编译期空转的空壳**。前一条车道 `p2-rungate-impl` 报告 §2.3 第 27 行称它
"仅在 `RunGate_LEG.dproj` 出现，活工程未引用"——本车道复核：**"仅在 `RunGate_LEG.dproj` 出现"这半句无法证实**，
`.dpr/.dproj/.dpk` 全树 grep **0 命中**（§3-7）。**主结论"不移植"一致**，处置保留。

### 2.5 工程成员资格（`.dpr` / `.dproj`）

`RunGate.dpr` 的 `uses`（24 项）与 `RunGate.dproj` 的 22 条 `DCCReference` **都不含**本车道四单元中任何一个；
四单元是经 `DCC_UnitSearchPath` 的 `Common\`（= `Source/RunGate/Common/`）**隐式编译**进来的：

```
DCC_UnitSearchPath = $(DELPHI)\Lib\Debug;Common\;..\Common\;..\Common\Compress;...
```

这与 `docs/并行报告-p2-rungate-impl.md` §1 的结论一致：`IocpWinsock2/IocpUtils/uBuffer/IocpTcpServer/IocpTcpClient/IODataPool/Qos`
是一整套 WinSock2+IOCP 自研栈，**不是**"过时遗留"，而是被**整体**判为"管道本体不移植"。

---

## 3. 发现的原文缺陷 / 易错点（7 条，均未"顺手修正"）

### 3-1 【汇编 `DivMod` 的实际语义与签名不符】而它**是活代码**（被调用 2 次）

- `IODataPool.pas:68-80`：

  ```pascal
  procedure DivMod(Dividend: Integer; Divisor: Word; var Result, Remainder: Word);
  asm
    PUSH EBX / MOV EBX,EDX / MOV EDX,EAX
    SHR EDX,16          // ← EDX = Dividend 的高 16 位（被 IN 参数覆盖）
    DIV BX              // ← (DX:AX) / BX，即"高16位:低16位" 拼成的 32 位数
    MOV EBX,Remainder / MOV [ECX],AX / MOV [EBX],DX
  ```

- **调用点**：`:319`（`GetNewIOData`）与 `:427`（`GiveBackIOData`），
  两处都是 `DivMod(MaxSize, 1 shl 7, w1, w2); if w2 > 0 then Inc(w1);`。
- **缺陷**：`MOV EDX,EAX / SHR EDX,16` **先**把 `EDX` 刷成 `EAX` 的高 16 位，
  所以 `DIV BX` 的被除数是 `高16位:低16位`（= 完整 32 位 `Dividend`）——
  这与"首个 `MOV EBX,EDX` 保存的是 `Divisor`"**合起来恰好自洽**，
  但当 `Dividend` 为**负**（bit31=1）时 `EDX != 0` ⇒ 商溢出 `AX` ⇒ x86 触发 **#DE 除零/溢出异常**。
- 现网调用点传入的是 `MaxSize`（请求缓冲大小）与 `AllocSize`，恒非负、且 `< 2^20` ⇒ **不会触发**。
  ⇒ 这是一条**潜伏缺陷**（不是"死代码"）。
- 处置：不移植（宿主 `TIODataPool` 不移植）；C# 侧用 `(a / b, a % b)` 即可，且天然对负数安全。

> ★ 本条保留了一次**本车道自己的判断纠错**：第一版写成"`DivMod` 从未被调用（死代码）"，
> 复查调用点后更正为"被调用 2 次，缺陷是负数路径会 #DE"。
> 教训：断言"某函数是死代码"必须 grep **调用点**，不能只看函数名形态。

### 3-2 【池的第二档只有 60 块且阈值硬编码】`UseIocpClient` 分支

- `:131` `for J := 0 to 60 - 1 do`（预分配 60 块 × 1MB = 60MB）；
- `:397` `if FNoUseIODataLists_2.Count >= 60 then`（归还时同样用裸字面量 `60`）。
- 两处 `60` **没有**写成常量，而主档用的是 `MAX_PREALLOCATED_MEMORY_SIZE`（1024）。
  ⇒ "预分配多少"与"超过多少就地释放"用同一个魔数，改一处忘一处会静默失衡。
- 处置：不移植，登记。

### 3-3 【`AllocMem` 结果未判空，随后立即解引用】

`:107-123`（及 `:133-148`、`:291-310`、`:336-355`）：
`IoData := AllocMem(SizeOf(OVERLAPPEDEx));` 之后**立刻** `IoData.AllocSize := ...`。
`AllocMem` 失败返回 `nil` 时是裸访问违例（Delphi 默认不检查）。共 **4 处**同型写法。

### 3-4 【随机释放只用 `High()`，且会随机到"当前档"】

`:437` `Index := Random(High(FNoUseIODataLists));`
- `Random(n)` 取值域 `[0, n)`，而 `High()` = 40 ⇒ **档 40 永远不会被随机释放**（少 1 档）。
- 注释写"随机性的释放内存空间 chongchong 2016-08-24"，但阈值 `500 shl 20`（500MB）与
  `FAllocMemSize` 的核算口径（只在分配/释放时累加）耦合，边界不可测。
- 处置：不移植，登记。

### 3-5 【归还路径不校验归属】`GiveBackIOData` 可把外部指针塞进池

`:383-486`：`Result := False;` 开头，但 `w1 <= High(FNoUseIODataLists)` 成立时**无条件**把入参挂回
`FNoUseIODataLists[w1]`；既没有"该指针确实来自本池"的校验，也没有"已在链上"的查重。
重复归还同一个 `IOData` ⇒ 同一块被挂两次 ⇒ 后续两次 `GetNewIOData` 返回同一块。

### 3-6 【`GetNewIOData` 的越界分支是 `raise`，但异常类型过窄】

`:362` `raise Exception.Create('GetNewIOData Size error');`
在 `FCS` 已 `Lock` 的情况下抛出，由 `finally FCS.UnLock` 保护 ⇒ 锁没问题，
但异常字符串**不含** `MaxSize` 与 `w1`，线上无法定位是哪次请求超限。

### 3-7 【跨车道报告的事实不符】`DllUpdateCommon.pas` 的"仅在 `RunGate_LEG.dproj` 出现"

`docs/并行报告-p2-rungate-impl.md:114` 称该单元"仅在 `RunGate_LEG.dproj` 出现"。
实测全树 `*.dpr`/`*.dproj`/`*.dpk` grep `DllUpdateCommon` = **0 命中**（含 `RunGate_LEG.dproj` 亦无）。
**主结论（不移植）不变**，但该半句证据应删除或改为"任何工程文件均未引用"。
本车道无权改前一车道的报告，仅在此登记。

---

## 4. 偏离登记（D-P9-xx）

本车道**没有做 1:1 移植**，因此不存在"移植语义偏离"；下表登记的是**裁定层面的偏离/缺口**。

| 编号 | 内容 | 原因 | 恢复途径 |
|---|---|---|---|
| **D-P9-01** | `IODataPool` 的整体语义（41 档 × 1024 预分配 + 500MB 随机修剪 + 60 块 1MB 第二档）在托管侧**不存在等价物** | 托管 IOCP 用 `SocketAsyncEventArgs.SetBuffer` 每次分配、GC 回收；无 `OVERLAPPEDEx` 池需求 | 若将来出现"高频分配导致 GC 压力"的实测数据，可在 `GXX.GatewayKit` 内新增**唯一一份**缓冲池（届时本登记作废） |
| **D-P9-02** | `uFrmMain.pas:2240-2245` 的 6 个池统计标签**数据源消失** | `TIODataPool` 不移植 | 主窗体车道可改用 `IocpManager.SessionCount` / `GateSession.PendingSendCount` 复刻"用量"含义的标签；"内存量"无对应值，建议标签隐藏并登记 |
| **D-P9-03** | `TIocpRemoteContext.SetActive` 是 **`ConnectEx` 异步连接**，`TcpLink.Connect()` 是**阻塞 connect** | `TcpLink` 是既有活设施（`GXX.DBServer` 在用），本车道无权改 | 若 RunGate↔M2 链路需要"连接不阻塞 UI 线程"，由 `GatewayKit` 归属方给 `TcpLink` 加异步连接（**不在本分区**） |
| **D-P9-04** | `TIocpTcpClient` 的 5 个事件（`OnContextConnect`/`OnContextDisconnect`/`OnError`/`OnRecvDataBuffer`/`OnSendDataBuffer`）里，`TcpLink` 只提供 3 个（无 `OnError`、无 `OnSendDataBuffer`） | 同 D-P9-03 | `TcpLink.ProcessReceive` 的 `SocketError` 分支即 `OnError` 语义；如需可加事件（`GatewayKit` 归属方） |
| **D-P9-05** | `Qos.pas` 的 42 个名字在 C# 侧**零落点**；另 7 个 `SERVICETYPE_*` 已由 Client 车道在 `WinSock2Constants.cs` 落地 | Qos 的宿主 `IocpWinsock2.pas` 不移植 | 若将来要接 `WSAIoctl(SIO_SET_QOS)`，直接**引用** `GXX.Client.Tail.WinSock2Constants` 的既有同名常量，**不要**从 `Qos.pas` 重译 |
| **D-P9-06** | `DivMod`（`IODataPool.pas:68`）的 16 位除法语义未在 C# 侧保留 | 宿主不移植；C# 用 `(a/b, a%b)` 更安全 | 无（不移植项） |

---

## 5. 门禁结果（本车道独占工程，未跑整解决方案）

```
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p9-rungate-rest

# 门禁 1
dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false

# 门禁 2（只跑本车道相关测试工程；正式门禁跑全工程）
dotnet test GXX.CSharp/tests/GXX.RunGate.Tests/GXX.RunGate.Tests.csproj -c Debug --nologo -m:1 -p:BuildInParallel=false
```

**结果（最终门禁，实测输出）**

| 门禁 | 命令 | 结果 |
|---|---|---|
| 门禁 1 | `dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false` | `已成功生成。` / **0 个错误** / 165 个警告（全部来自其它车道的既有文件） |
| 门禁 2 | `dotnet test GXX.CSharp/tests/GXX.RunGate.Tests/GXX.RunGate.Tests.csproj -c Debug --nologo -m:1 -p:BuildInParallel=false` | `已通过! - 失败: 0，通过: 1922，已跳过: 0，总计: 1922`（10 s） |
| 本车道切片 | 同上 + `--filter FullyQualifiedName~Rest9NotPortedEvidenceTests` | `已通过! - 失败: 0，通过: 31，已跳过: 0，总计: 31` |

> 内存受限提醒：全机仅约 1.1GB 空闲、5 条车道并行编译时，本车道的所有 `dotnet` 命令
> **一律带 `-m:1 -p:BuildInParallel=false`**（调度方提醒），已实测可稳定通过。
> 门禁 2 的 1922 例含本车道新增的 **31** 例（其余为本区既有车道 `p2-rungate-impl` / `p4-rungate-mirclient` 的用例，
> 本车道**未改动任何一个既有测试文件**，因此它们全绿说明本车道没有引入回归）。

### 5.1 本车道的自我纠错记录（4 处，全部由取证用例抓出）

| # | 初版（错） | 实测（对） | 抓出它的用例 |
|---|---|---|---|
| 1 | `Qos` 的符号"引用方一个都没用到" | `IocpWinsock2.pas` 用了 `FLOWSPEC`(7) 与 `SERVICETYPE`(2) | `Qos_OnlyFlowSpecAndServiceTypeReachIocpWinsock2_AndNothingElse` |
| 2 | `Qos` 有 5 个 `const` 段 | **4** 个（`:81/175/215/261`） | `Qos_HasZeroExecutableCode_OnlyTypesAndConstants` |
| 3 | C# 侧 0 命中 **32** 条 | **42** 条（49 名字 − 7 已落地） | `Qos_FortyTwoSymbols_HaveLiteralZeroCSharpHits` |
| 4 | `DivMod` "从未被调用" | 被调用 **2** 次（`:319`、`:427`） | §3-1 复查 |

**方法论副产物（值得进下一波派发规程）**：
**不要用 `Regex.Matches(s, @"^type\s*$", RegexOptions.Multiline)` 数"整行标记"** ——
`\s` 会跨行吞掉空行，使 `$` 永远无法定位，**恒返回 0**（本车道实测）。
改用 `text.Split('\n').Count(l => l.Trim() == token)`（见测试里的 `CountExactLines`）。
同源陷阱：**剥注释时若连换行一起删掉，前后两行会被粘成一行**，行首标记随之丢失
（本车道第一版 `StripPascalComments` 正是这样把 5 个 `type` 段数成 4 个）。

---

## 6. 未完成 / 阻塞项（如实登记）

### 6.1 阻塞项（需调度方/集成方处置，**本车道无权执行**）

| # | 阻塞项 | 影响 | 需要的动作 |
|---|---|---|---|
| B1 | 台账 §18.5 第 6 条要求"在这些 `.cs` 文件头 40 行内注明源单元以闭合 E2 证据" | 本车道四单元的证据**无法写进** `src/GXX.GatewayKit/GatewayProtocol.cs` / `TcpLink.cs`（**文件分区外，禁止修改**）⇒ 覆盖审计报表仍会把 `IODataPool`/`IocpTcpClient`/`Qos` 显示为缺口 | 由 `GatewayKit` 归属方在 `GatewayProtocol.cs` / `TcpLink.cs` 头部各加一段源单元注释（内容可直接抄本报告 §2.1/§2.2/§2.3 的对照表首行） |
| B2 | `Qos.pas` 的 7 个 `SERVICETYPE_*` 已在 `src/GXX.Client/Tail/WinSock2Constants.cs` 落地，与 RunGate 份**同源但不同单元** | 报表无法自动把 `RunGate/Common/Qos` 判为"部分已移植" | 同上，需归属方在文件头注明"另见 `Source/RunGate/Common/Qos.pas`" |
| B3 | `uFrmMain.pas` 的 6 个池统计标签（D-P9-02） | 主窗体车道若复刻这 6 个标签，**没有数据源** | 主窗体车道决策：改语义 or 隐藏标签 |

> 以上三项都**不阻塞**本车道的门禁与提交，仅阻塞"覆盖审计报表闭合"。

### 6.2 明确不做（有意留下的）

| 项 | 说明 |
|---|---|
| 不建 `Rest9QosCompat` / `Rest9IODataPool` 之类的"证据外壳类" | 会产生**零调用方**的托管类型（§35.4），且与 `GatewayKit` 构成第二份实现（§14.2）。证据一律以**数据 + 用例**形式留在 `Rest9NotPortedEvidence` 与测试里 |
| 不做真实 socket 端到端 | 四单元全部不移植，无运行时可测对象；测试只读文件系统（不依赖网络端口，符合"测试不得依赖真实网络端口"） |
| 不改任何既有文件 | 四单元的全部依赖（`IocpUtils`/`IocpWinsock2`/`GatewayKit`）都在分区外 |

### 6.3 交付物清单
| 文件 | 大小 | 说明 |
|---|---|---|
| `GXX.CSharp/src/GXX.RunGate/Rest9/Rest9NotPortedEvidence.cs` | 约 20 KB | 四单元的证据数据（成员清单/行号/计数/调用点）+ 3 个纯判定函数（`UsesClauseContainsUnit`、`StripPascalComments`），**无运行时算法** |
| `GXX.CSharp/tests/GXX.RunGate.Tests/Rest9NotPortedEvidenceTests.cs` | 约 40 KB | **31 例**，全部**直接读仓库里的 Delphi 原文 / C# 既有源码**重新抽取计数（不是 C# 自证），否定性断言全部先数总数再断言（§37.3） |
| `GXX.CSharp/docs/并行报告-p9-rungate-rest.md` | 本文件 | — |

**未改动的文件数 = 0**（`GXX.slnx` / `*.csproj` / `Directory.Build.props` / `docs/Checklist.md` /
`docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**` / `src/GXX.RunGate/**` 既有文件 **均未触碰**）。

---

## 7. 给集成方的一句话结论

本车道的四单元**不需要再派车道移植**：前一条车道 `p2-rungate-impl` 给出的初判（:102–:104）经本车道取证后**全部成立**，
`DllUpdateCommon` 的"仅在 `RunGate_LEG.dproj` 出现"这半句被实测反证（§3-7，主结论不变）；
四单元的不移植现在都有可复现的证据链
（原文逐行号核对 + 全树调用点计数 + C# 侧 0 命中计数 + GatewayKit 公开 API 差异）。
集成方只需做 **§6.1 的 B1/B2**（在 `GatewayKit` 的两个文件头补源单元注释）即可让覆盖审计报表闭合。

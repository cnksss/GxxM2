# 并行报告 · 车道 `p2-rungate-impl`（GXX Delphi7→C# · RunGate 余部）

> 分支：`par/p2-rungate-impl` ｜ 工作树：`.worktrees/p2-rungate-impl`
> 基线：`main`（车道开工时的 HEAD）
> 写入者：本车道 agent（父）+ 2 个受控子代理（`uBuffer.pas` / `uFrm*.pas`）
> 规程：`docs/转换开发文档.md`、`docs/并行派发台账.md` §9.4/§10/§11、`docs/并行覆盖审计.md`

---

## 0. 交付摘要（TL;DR）

| 项 | 结果 |
|---|---|
| 新建源码文件 | 见 §3 |
| 新建测试文件 | 见 §3 |
| 本车道新增测试用例 | 见 §4 |
| `dotnet build GXX.slnx -c Debug` | 见 §4 |
| `dotnet test tests\GXX.RunGate.Tests\...` | 见 §4 |
| RunGate 单元映射 | §2（43 个单元全部有处置结论） |
| 发现原文缺陷/易错点 | §5（20 条，每条带 `文件:行号`） |
| 未完成 | §6 |

**行数口径警告**：任务书与 `docs/并行覆盖审计.md` 给出的行数与实际文件不符（审计脚本的行数计数器与物理 LF 数不同）。
本报告一律用**实测 LF 行数**（`[System.IO.File]::ReadAllBytes(...)` 数 `\n`）：

| 单元 | 任务书/审计 | 实测 LF | 倍差 |
|---|---|---|---|
| `RunGateUtils.pas` | 4,393 | **4,925** | 1.12× |
| `Common/IocpWinsock2.pas` | 3,101 | **3,450** | 1.11× |
| `Common/uBuffer.pas` | 2,271 | **2,365** | 1.04× |
| `Common/IocpUtils.pas` | 1,383 | **1,646** | 1.19× |
| `uFrmGameSpeed.pas` | 1,172 | **1,333** | 1.14× |
| `uFrmSafeFilter.pas` | 1,307 | **1,497** | 1.15× |

> 后续派发请以实测行数为准（`Get-Content -Encoding Default -Raw` 后数 `\n`）。

---

## 1. 编译期开关：先钉死"哪一半是活代码"

`RunGate` 的条件编译开关决定了哪些 `{$IF}` 分支是**死代码**。审计时必须先定死，否则会把死分支当活逻辑移植
（或反过来）。实测生效组合：

| 开关 | 定义处 | 值 | 后果 |
|---|---|---|---|
| `UseIocpClient` | `Common/IocpCommon.pas:14` | **1** | `RunGateUtils.pas:1538-1786`（`TClientSocket` 版）与 `:4233-4270` 全为**死代码**；活的是 `TMirRemoteContext`（`IocpTcpClient`） |
| `MultiThreadRunContext` | `Grobal2_Ex.pas:24` | **1** | `OnTimerRunContext`（`:4331-4476`）为**死代码**；活的是 `TFullServiceMsgProcessThread.Execute`（`:3256-3381`） |
| `NEED_REGISTER` | `Grobal2_Ex.pas:18` | **1** | `GM_DATA/GM_COMPDATA/...` 在源码里是 `var` 而非 `const`（见 §5-1）；`DoConnnectUser` 活 |
| `REGISTER_TEST` | `Grobal2_Ex.pas:21` | 0 | 暗桩走 `Register` 版分支 |
| `CLIENT_ANTIPLUG` | `Grobal2_Ex.pas:10` | 1 | `SM_BLACKMODULEMD5` 分支里的反外挂重发逻辑活 |
| `RungateLEG_IOCP` | `Grobal2_Ex.pas:32` | 6 | 验证服务器的 `dwCode` 常量（`RunGateUtils.pas:4610-4620` 死分支） |

**关键结论**：`IocpWinsock2/IocpUtils/uBuffer/IocpTcpServer/IocpTcpClient/IODataPool/Qos` 这一整套
**WinSock2 + IOCP 自研栈**是 RunGate 与 M2 之间的真实链路；它**不是**"过时遗留"。

---

## 2. RunGate 单元映射表（43 个单元全部处置）

`Source/RunGate/**` 下 `.pas` 共 43 个（含 `Common/` 与 `Demo/`）。
列口径：**新建** = 本车道新文件；**已覆盖** = 既有 `.cs` 等价实现；**不移植** = 有意不移植 + 理由。

### 2.1 本车道新建（优先 1/3/4/5）

| # | 单元 | 实测 LF | 处置 | 产物 | 覆盖行号 |
|---|---|---|---|---|---|
| 1 | `RunGateUtils.pas` | 4925 | **新建**（纯逻辑切片） | `RunGateUtilsProtocol.cs` | 228-301, 314-357, 402-437, 1867-1979（常量部分 667-694） |
| | | | | `RunGateUtilsFrame.cs` | 439-529, 747-924, 1367-1378 |
| | | | | `RunGateUtilsHash.cs` | 1867-1979 |
| | | | | `RunGateUtilsClientStat.cs` | 3816-3829, 3831-3899（死孪生）, 3901-3967 |
| | | | | `RunGateUtilsCache.cs` | 1094-1231 |
| | | | | `RunGateUtilsFullServiceMsg.cs` | 4341-4417（死分支语义）, 4478-4500, 4502-4524 |
| | | | | `RunGateUtilsHeartbeat.cs` | 377, 1828-1981, 4259-4311 |
| | | | | `RunGateUtilsForward.cs` | 926-1365 |
| | | | | `RunGateUtilsThreads.cs` | 3791-3814, 4526-4532 |
| | | | **接缝（未 1:1）** | `RunGateService.cs`（既有）, `GatewayKit/*` | `TMirRemoteContext`/`TRunGate`/`TRunGateManager` 的 socket/线程壳 |
| 2 | `Common/IocpWinsock2.pas` | 3450 | **大部分不移植 + 新建纯宏** | `IocpWinsock2Compat.cs` | 174-184, 206-212, 241-267, 174-1210（常量）, 3131-3287 |
| | | | **不移植** | — | 约 2,950 行是 WinSock2 类型/常量/`external ws2_32` 声明 → 由 `System.Net.Sockets` 覆盖 |
| 3 | `Common/uBuffer.pas` | 2365 | **新建**（子代理 A，1:1） | `uBufferMemoryPool.cs` / `uBufferStream.cs` / `uBufferLink.cs` / `uBufferRing.cs` / `uBufferObjectPool.cs` | 全文（见 §4 子代理 A 报告） |
| 4 | `Common/IocpUtils.pas` | 1646 | **管道本体不移植 + 新建纯策略** | `IocpUtilsPolicy.cs` | 336-412, 414-486, 600-625, 699-742, 883-943, 1519-1533 |
| | | | **不移植** | — | `CreateIoCompletionPort`/`GetQueuedCompletionStatus`/`WSARecv`/`WSASend`/`PostQueuedCompletionStatus`/工作线程池/`OVERLAPPEDEx` 池 → `GatewayKit/GatewayProtocol.cs` 的 `IocpManager`（`SocketAsyncEventArgs`，底层即 Windows IOCP） |
| 5 | `uFrm*.pas`（12 个，合计 5381 LF） | — | **新建**（子代理 B，WinForms + 可测逻辑层） | `uFrm*.cs` | 见 §4 子代理 B 报告 |

### 2.2 已由既有产物覆盖

| # | 单元 | 实测 LF | 现住在哪里 | 证据 |
|---|---|---|---|---|
| 6 | `EDcode.pas` | 1291 | `GXX.Core/Protocol/EDcode.cs` + `EDcode.Tables.g.cs` | 同名 `.cs`（E1）；RunGate 份与 M2/Client 版同源 |
| 7 | `DesNew2.pas` | 909 | `src/GXX.RunGate/DesNew2.cs` | E1 |
| 8 | `IniFilesEx.pas` | 810 | `src/GXX.RunGate/IniFilesEx.cs`（车道3 P1） | E1 |
| 9 | `ParadoxConv.pas` | 1106 | `src/GXX.RunGate/ParadoxConv.cs` + `ParadoxConv.Tables.g.cs`（车道3 P1） | E1 |
| 10 | `uIPDownThread.pas` | 469 | `src/GXX.RunGate/IpDownThread.cs`（车道3 P1） | E1 |
| 11 | `RunGatePluginInterface.pas` | 440 | `src/GXX.RunGate/RunGatePluginInterface.cs`（车道3 P1） | E1 |
| 12 | `VerifyCodeUtils.pas` | 328 | `src/GXX.RunGate/VerifyCodeUtils.cs`（车道3 P1） | E1 |
| 13 | `EncryptUnit_LF.pas` | 187 | `src/GXX.RunGate/EncryptUnitLf.cs`（车道3 P1） | E1 |
| 14 | `BagItemList.pas` | 202 | `src/GXX.RunGate/BagItemList.cs` | E1 |
| 15 | `DesUtils.pas` | 1369 | `GXX.Core/Crypto/**`（DES 系列） | `GXX.Client/.../LauncherParam.cs` 提及 + Core/Crypto |
| 16 | `WinHttp.pas` | 983 | `src/GXX.RunGate/IpDownThread.cs`（HTTP 下载语义） | `IpDownThread.cs` 文件头提及 `WinHttp.pas`/`uIPDownThread.pas` |
| 17 | `Grobal2_Ex.pas` | 788 | **常量**在 `GXX.Core/Protocol/Grobal2.Const.g.cs`（`RUNGATECODE` 等）与 `GXX.Core/CommonConst.g.cs`（`GM_*`）；**RunGate 私有结构** `TRungateVerifyHeader/TRungateVerifyData/TRungateVerifyData_New` 无既有 `.cs` → 本车道新建于 `RunGateUtilsProtocol.cs` | 同名常量逐一比对通过（`RunGateUtilsProtocolTests`） |
| 18 | `Common/IocpCommon.pas` | 298 | `TSafeList`/`TIocpCriticalSection` → .NET `lock`（`GatewayKit/GateSession`）；`MAX_OVERLAPPEDEX_BUFFER_SIZE`/`IOCP_QUEUED_SHUTDOWN`/`SIO_KEEPALIVE_VALS`/keepalive → 本车道 `IocpUtilsPolicy.cs` + `IocpWinsock2Compat.cs`；`UseIocpClient`/`MAX_IOCP_CLIENT_RECV_BUFFER_SIZE` 在 `RunGateUtilsProtocol.cs` 头部注释登记 | 文件头提及 |
| 19 | `Common/IocpTcpServer.pas` | 884 | `GatewayKit/GatewayProtocol.cs` 的 `IocpManager`（Accept 池 + 会话表）等价覆盖 | — |
| 20 | `Common/IocpTcpClient.pas` | 341 | `GatewayKit/TcpLink.cs` 等价覆盖 | — |
| 21 | `Common/IODataPool.pas` | 587 | 不移植（托管侧无 `OVERLAPPEDEx` 池需求，由 GC + SAEA 池替代） | — |
| 22 | `Common/Qos.pas` | 294 | 不移植：仅被 `IocpWinsock2.pas:79` 引用（QOS 结构体来源），.NET 无对应语义 | `Qos` 只在 `IocpWinsock2.pas` 出现 |
| 23 | `MagicIntervalUtils.pas` | 199 | **接缝**：`uFrmGameSpeedLogic.cs` 提及；1:1 未见（见 §6） | 文件提及 |

### 2.3 有意不移植（理由 + 证据）

| # | 单元 | 实测 LF | 理由 | 证据 |
|---|---|---|---|---|
| 24 | `Demo/RungatePlug/RungateCommon.pas` | 97 | 不是 `RunGate.exe` 的组成：`RunGate.dpr` 的 `uses` 与 `RunGate.dproj` 的 22 条 `DCCReference` 都没有它；全树无 `uses RungateCommon` | `RunGate.dpr:3-27`、`RunGate.dproj`（0 处 `Demo`）、grep 结果为空 |
| 25 | `Demo/ClientPlug/ClientPlugCommon.pas` | 38 | 同上（插件示例工程自带单元） | 同上 |
| 26 | `EHookLIB.pas` | 974 | **孤儿单元**：不在 `.dpr`、不在 `.dproj`、无任何 `uses`。且属 `SetWindowsHookEx` 全局钩子（反外挂件，DoD §2.3 不移植项） | `RunGate.dpr` 无；`RunGate.dproj` 无；grep `\bEHookLIB\b` 无命中 |
| 27 | `DllUpdateCommon.pas` | 11 | **孤儿单元**（11 行，仅在 `RunGate_LEG.dproj` 出现，活工程未引用） | grep 无命中 |
| 28 | `uFrmHitInterval.pas` | 85 | **孤儿单元**：不在 `RunGate.dpr`/`RunGate.dproj`，也无任何 `uses` 引用 | `RunGate.dpr` 24 个单元列表里没有；grep 无命中 |
| 29 | `AsyncCalls.pas` | 3440 | **本轮不移植（跨车道裁定）**：`M2Engine`/`Client-HGE`/`RunGate` 三份同源（各 2,959 行），台账 §8.5 裁定 P2 上移到 `src/GXX.Core/Async/` 三处共用；本车道文件分区不含 `src/GXX.Core/**` | `docs/并行派发台账.md` §8.5、§5 第 10 行 |
| 30 | `ParadoxDataSet.pas` | 1363 | **本轮不移植（跨车道裁定）**：与 `ParadoxConv.pas` 同族，台账 §5 第 15 行裁定 RunGate/GameCenter 两份副本应上移 `GXX.Core` 共用；本车道只做了 `ParadoxConv` | `docs/并行派发台账.md` §5:151 |

### 2.4 接缝（本轮只做薄壳，未 1:1）

| # | 单元 | 实测 LF | 现状 |
|---|---|---|---|
| 31 | `MirClientContext.pas` | 11126 | `RunGateService.cs` + `GatewayKit/GateSession` 覆盖了会话状态/节流/转发壳；单元特有的包裹/药品/魔法/防外挂处理未移植 |
| 32 | `uFrmMain.pas` | 4217 | `Program.cs`（483 B）+ `RunGateService.cs` + `GatewayKit/GateMainForm.cs`（2845 B） |
| 33 | `GateShare.pas` | 3596 | 已覆盖：节流配置、`RUNGATECODE`/`RUN_GATE_MSG_CODE`、`EncodeRunGateMsg`（B）、`tick_diff`、配置默认值（`:1204/1264/1272-1275`）。未覆盖：过滤词表/`g_TempIPList`/IP-MAC 黑名单与落盘、`InputPassword`、`g_VerifyFailUserList`、暗桩与验证服务器全流程 |

---

## 3. 新增文件清单

### 3.1 父 agent（RunGateUtils / Iocp 族）

| 文件 | 大小 | 对应源 |
|---|---|---|
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsProtocol.cs` | 10,492 B | `Grobal2_Ex.pas:228-301, 667-694`；`GateShare.pas:3475-3494`；`RunGateUtils.pas:314-357, 402-437` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsFrame.cs` | 9,597 B | `RunGateUtils.pas:439-529, 747-924, 1367-1378` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsHash.cs` | 5,018 B | `RunGateUtils.pas:1867-1979` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsClientStat.cs` | 4,481 B | `RunGateUtils.pas:3816-3967` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsCache.cs` | 7,350 B | `RunGateUtils.pas:1094-1231` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsFullServiceMsg.cs` | 4,408 B | `RunGateUtils.pas:4341-4417, 4478-4524` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsHeartbeat.cs` | 6,761 B | `RunGateUtils.pas:1828-1981, 4259-4311`；`GateShare.pas:377, 1204, 1264, 1272-1275, 1458-1464` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsForward.cs` | 10,761 B | `RunGateUtils.pas:926-1365` |
| `GXX.CSharp/src/GXX.RunGate/RunGateUtilsThreads.cs` | 3,728 B | `RunGateUtils.pas:3791-3814, 4526-4532`；`MirClientContext.pas:300` |
| `GXX.CSharp/src/GXX.RunGate/IocpWinsock2Compat.cs` | 10,683 B | `Common/IocpWinsock2.pas:174-184, 206-212, 241-267, 174-1210, 3131-3287`；`Common/IocpCommon.pas:94-103, 122-151` |
| `GXX.CSharp/src/GXX.RunGate/IocpUtilsPolicy.cs` | 7,355 B | `Common/IocpUtils.pas:336-412, 414-486, 600-625, 699-742, 883-943, 1519-1533` |

### 3.2 测试

| 文件 | 覆盖 |
|---|---|
| `tests/GXX.RunGate.Tests/RunGateUtilsProtocolTests.cs` | 布局 `SizeOf` / 常量 / 帧构造 / `EncodeRunGateMsg` / 跨文件契约缺陷 |
| `tests/GXX.RunGate.Tests/RunGateUtilsFrameTests.cs` | 严格 vs 宽松扫描差异 / 全服消息切分边界 |
| `tests/GXX.RunGate.Tests/RunGateUtilsHashTests.cs` | Jenkins 变体黄金向量（独立复算） |
| `tests/GXX.RunGate.Tests/RunGateUtilsClientStatTests.cs` | 多数表决（含"首个达半数≠最大值"差异） |
| `tests/GXX.RunGate.Tests/RunGateUtilsCacheTests.cs` | 14 槽映射 / 连续性不变量 / 死分支槽位不足 |
| `tests/GXX.RunGate.Tests/RunGateUtilsFullServiceMsgTests.cs` | 有损排空（只发 10 条但全出队）/ `-1` 去尾 |
| `tests/GXX.RunGate.Tests/RunGateUtilsHeartbeatTests.cs` | `tick_diff` 回绕 / 心跳与超时判据差异 / 挑战状态机 |
| `tests/GXX.RunGate.Tests/RunGateUtilsForwardTests.cs` | else-if 链分类 / CRLF 裁剪 / `SM_CHECK_RUNGATE1` 应答 |
| `tests/GXX.RunGate.Tests/RunGateUtilsThreadsTests.cs` | 线程数账目 / IOCP 日志格式 |
| `tests/GXX.RunGate.Tests/IocpWinsock2Tests.cs` | `fd_set`/`timeval`/ioctl 编码/地址分类/异步消息打包/keepalive |
| `tests/GXX.RunGate.Tests/IocpUtilsTests.cs` | 发送分块与回收 / 良性错误白名单 / 接收护栏 / 线程数策略 |

### 3.3 子代理产物（uBuffer.pas）

| 文件 | 覆盖 `uBuffer.pas` 行 |
|---|---|
| `src/GXX.RunGate/uBufferMemoryPool.cs` | :14-25（`TMemoryBlock`/`TDxMemBlockType`）、:27-52、:180-193、:195-253、:255-534、:2352-2362 |
| `src/GXX.RunGate/uBufferStream.cs` | :54-94（声明）、:535-1316（`TDxMemoryStream` 全部） |
| `src/GXX.RunGate/uBufferLink.cs` | :96-124（声明）、:1317-1686（`TBufferLink` 全部） |
| `src/GXX.RunGate/uBufferRing.cs` | :138-178（声明）、:1773-2350（`TDxRingStream` 全部） |
| `src/GXX.RunGate/uBufferObjectPool.cs` | :125-137（声明）、:1687-1756（`TDxObjectPool`）、:1758-1772（`FreeObjPool`） |
| `tests/GXX.RunGate.Tests/BufferMemoryPoolTests.cs` / `BufferStreamTests.cs` / `BufferLinkTests.cs` / `BufferRingTests.cs` / `BufferObjectPoolTests.cs` | 测试 |

**六个全局池的 `BlockSize` 用脚本从 GBK 原文抽取 + 回读比对**（非手工转录）：
`Small=128 / Normal=640 / Big=1024 / SpBig=2048 / Large=4096 / SPLarge=16384`，
`InitCount=30`、`MaxFreeBlocks=30`（原 `uBuffer.pas:217/224/231/238/245/252`），
抽取命令与回读结果写在 `uBufferMemoryPool.cs` 文件头，并由
`GlobalPools_BlockSize_MatchExtractionFromSource` 断言。

公开 API 一览（供映射/接缝使用）：
- `enum TDxMemBlockType`（`MB_Small=0 … MB_SPLarge=5` 顺序保留）
- `sealed class TMemoryBlock`（`RecordSize=32`、`byte[] Memory`、`Next/Prev/NextEx/PrevEx`、`BlockType`、`ushort DataLen`）
- `class TDxMemoryPool`（`GetMemory(bool Zero)→byte[]`、`GetMemoryBlock()`、`FreeMemory(byte[])`、`FreeMemoryBlock`、`Clear`、`Lock/Unlock`、`UseCount/FreeCount`；测试探针 `FindUseBlock/FindFreeBlock/CountUseChain/CountFreeChain`、`FUseCount/FFreeCount`）
- `static class MemoryPoolGlobal`（六个池 + `GetPool(TDxMemBlockType)` + `GetTotalMemBytes` + `AddMemory/DelMemory` + `FreeObjPool`）
- `class TDxMemoryStream : Stream`（`Read/Write/Seek/SetLength`、`Head/Last`、`ReadStream/WriteStream`、`SaveToStream/SaveToFile`、`LoadFromStream/LoadFromFile/LoadFromBufferList`、`LinkToBufferList`、`SwapStreamLink`、`SaveCurBlock/RestoreCurBlock`；探针 `PositionInternal/CapacityInternal/CurBlockPosInternal/CurBlockInternal/MarkBlockInternal/MarkBlokPosInternal`）
- `class TBufferLink`（`AddBuffer`、`AddMemBlockLink`、`ReadBuffer`、`ReadBufferWhileFindChar`、`static InnerReadBuf`、`MarkReaderIndex/RestoreReaderIndex`、`ClearBuffer/ClearHaveReadBuffer`、`ValidCount`；探针 `HeadBlock/LastBlock/ReadBlock/MarkBlock/ReadPositionInternal/MarkPositionInternal`）
- `class TDxRingStream : Stream`（`ReadBuffer/WriteBuffer`、两个 mark/restore、`SeekReadWrite`、`ReadPosition/WritePosition/CanWriteSize/DataSize`；探针 `*Internal`）
- `class TDxObjectPool`（`GetObject/FreeObject`、`UsesCount/UnUsesCount/MaxObjCount`）

### 3.4 子代理产物（uFrm*.pas → WinForms）

| 文件 | 对应 `.pas` / 实测 LF | `.dfm` |
|---|---|---|
| `src/GXX.RunGate/uFrmGameSpeedLogic.cs` | **共享接缝层**（`GateShare.pas:217-1365` 被用子集：枚举 / `TAntiPlugConfig` / 默认配置 / 字符串表 / `TSpinEditEx` / `TColorIndexEdit` / `TProcessBlackList` / `TMagicIntervalList` / `MessageBoxSeam` / `FormGlobals`） | — |
| `src/GXX.RunGate/uFrmGameSpeed.cs` | `uFrmGameSpeed.pas` 1332 | ✅ |
| `src/GXX.RunGate/uFrmSafeFilterLogic.cs` + `uFrmSafeFilter.cs` | `uFrmSafeFilter.pas` 1496 | ✅ |
| `src/GXX.RunGate/uFrmMagicCD.cs` | 582 | ✅ |
| `src/GXX.RunGate/uFrmItemEatCD.cs` | 264 | ✅ |
| `src/GXX.RunGate/uFrmInterval.cs` | 278 | ✅ |
| `src/GXX.RunGate/uFrmMessageFilter.cs` | 232 | ✅ |
| `src/GXX.RunGate/uFrmProcessBlacklist.cs` | 201 | ✅ |
| `src/GXX.RunGate/uFrmReadFileIP.cs` | 152 | ✅ |
| `src/GXX.RunGate/uFrmLogClientPacketSetting.cs` | 145 | ✅ |
| `src/GXX.RunGate/uFrmAddProcessBlack.cs` | 106 | ✅ |
| `src/GXX.RunGate/uFrmHitInterval.cs` | 84 | ✅ |
| `src/GXX.RunGate/uFrmAntiPlugUpdateSetting.cs` | 72 | ✅ |
| `tests/GXX.RunGate.Tests/RunGateUtilsForm*Tests.cs`（12 个） | — | — |

**重要更正**：`Source\RunGate\` 下这 12 个 `uFrm*.pas` **全部有同名 `.dfm`**
（含 `uFrmLogClientPacketSetting` / `uFrmHitInterval` / `uFrmAddProcessBlack` / `uFrmAntiPlugUpdateSetting`）。
**台账 §10 第 11 条「无 .dfm 偏差」在本车道不适用**，不必登记。
唯一例外：`uFrmGameSpeed.dfm` / `uFrmMagicCD.dfm` 里的 `TVirtualStringTree`（第三方 `VirtualTrees.pas`）未移植。

---

## 4. 门禁结果

```
cd .worktrees\p2-rungate-impl\GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo
dotnet test  tests\GXX.RunGate.Tests\GXX.RunGate.Tests.csproj -c Debug --nologo
```

**结果**：见下方"最终门禁"小节（本节由集成时回填）。

### 4.1 父 agent 切片的自验方式（隔离验证，避免与子代理抢 `obj/`）

由于 3 个写入者共用同一 `obj/`，父 agent 在自己的切片上采用了**隔离验证工程**：
把 `src/GXX.RunGate/RunGateUtils*.cs` + `Iocp*.cs` 与 `tests/GXX.RunGate.Tests/{RunGateUtils*,Iocp*}Tests.cs`
用 `EnableDefaultCompileItems=false` 显式列入 TEMP 下的一个临时 csproj，并 `Reference` 已构建好的 `GXX.Core.dll`。
结果：

```
已通过! - 失败: 0，通过: 206，已跳过: 0，总计: 206
```

> 隔离工程的 glob 需排除子代理 B 的 `RunGateUtilsForm*Tests.cs`（它们依赖窗体源文件，
> 而隔离工程只编父 agent 的切片）。

### 4.2 分切片结果（回炉前 → 回炉后）

| 切片 | 回炉前 | 处置 |
|---|---|---|
| 父 agent（`RunGateUtils*` + `Iocp*`） | 206 通过 / 0 失败（隔离验证） | 无 |
| 子代理 A（`uBuffer*`） | 257 通过 / **24 Skip** / 0 失败 | 已回炉要求消掉全部 `Skip`（通过或删除，禁止保留 `Skip`） |
| 子代理 B（`uFrm*`） | 232 通过 / **11 失败** | 已回炉，逐条给出 11 条失败明细 |

**回炉前的整轮快照**（用于对照）：
```
dotnet build GXX.slnx -c Debug → 0 错误
dotnet test tests\GXX.RunGate.Tests\... → 失败: 11，通过: 1155，已跳过: 24，总计: 1190（7 秒）
```
11 条失败**全部**在子代理 B 的窗体测试里；子代理 A 的「24 Skip」是"字节级期望值尚未核对"的边界用例。

> 踩坑（已写进 §4.1）：**子代理 B 一度报告"489/489 全绿"**，但那是在它自己的 scratch 镜像里跑的结果，
> 而它随后改了测试期望却没重跑 —— 权威门禁上仍有 11 条红。
> **教训：子代理的自述必须由父 agent 在共享工作树上复跑一次才算数**（与台账 §8.3 同一类问题）。

> **踩坑记录（务必写进下一波派发规程）**：**不要**用
> `-p:BaseIntermediateOutputPath=obj_x\ -p:BaseOutputPath=bin_x\` 来做"多 agent 并行构建隔离"。
> SDK 风格的 csproj 用 `EnableDefaultItems`，其 `DefaultItemExcludes` 只排除
> `$(BaseIntermediateOutputPath)`（默认 `obj\`）。一旦把它改名，`obj\` 与其它 `obj_*` 目录
> **不再被排除**，`obj\**\*.AssemblyInfo.cs`、`obj_other\**\...` 会被当作源码编入 →
> 满屏 `CS0579: 特性重复`，整个解决方案必炸（本车道实测）。
> 正确做法：要么串行构建，要么像上面那样用独立的隔离验证工程（引用 DLL，不引用 csproj）。

---

## 5. 发现的原文缺陷 / 易错点

> 每条都给 `文件:行号`。**未做任何"顺手修正"**；语义一律按原文照搬，差异用测试钉死。

### 5.1 【跨文件契约缺陷】`GatewayKit.RUNGATECODEX` 用错了常量

- `src/GXX.GatewayKit/GatewayProtocol.cs:22`：
  `public const uint RUNGATECODEX = Grobal2Const.RUN_GATE_MSG_CODE;` → **0xAABBCCDD**
- Delphi 真值：`Source/RunGate/Grobal2_Ex.pas:694` → `RUNGATECODEX = $AA9AAA9A`；
  而 `RUN_GATE_MSG_CODE = $AABBCCDD` 是**另一个**常量（`Grobal2_Ex.pas:37`，用作 `TRungateMsgHeader.Code`，见 `GateShare.pas:3479`）。
- 影响：`RunGateService.OnServerData` 与任何用 `RUNGATECODEX` 做 `GM_COMPDATA` 校验的地方都会判错
  （Delphi 侧 `RunGateUtils.pas:494 / 765` 用 `LongWord(MsgHeader.nSocket) = RUNGATECODEX` 校验压缩包）。
- 处置：本车道**无权修改** `src/GXX.GatewayKit/**`（文件分区外）。已在
  `RunGateUtilsConst.RUNGATECODEX` 给出正确值，并用差异断言
  `RunGateUtilsProtocolTests.Differential_GatewayKit_RUNGATECODEX_IsWrongConstant` 把差异固定，
  供集成者修 `GatewayProtocol.cs:22` 后删除该守卫。

### 5.2 【越界写】死分支 `FCacheDatas` 槽位少一个

- 活分支 `RunGateUtils.pas:38` → `FCacheDatas: array[0..13] of string`（14 槽）
- 死分支 `RunGateUtils.pas:107` → `FCacheDatas: array[0..12] of string`（**13 槽**）
- 而索引映射恰好用到 13（`RunGateUtils.pas:1159-1162` 的 `SM_SENDDROPITEMEFFECTLIST → 13`，`:1167` 写入）。
- 若把 `IocpCommon.pas:14` 的 `UseIocpClient` 切回 0，写槽 13 就是数组越界；Delphi 默认关范围检查 → 静默内存破坏。
- 处置：`RunGateCacheTable.DeadBranchSlotCount = 13` 显式登记，测试 `DeadBranch_FCacheDatas_IsTooSmall` 防"顺手补全"。

### 5.3 【`Recog` 长度字段与负载不符】`SM_ITEMEAT_CDTIME` 全服消息

```pascal
// RunGateUtils.pas:1255-1256
TheDefMsg := MakeDefaultMsg(SM_ITEMEAT_CDTIME, Length(sSendMsg), 0, 0, 0);
Context.AddServerMsg(@TheDefMsg, @g_EatItemCDConfig, SizeOf(g_EatItemCDConfig));
```
- `MakeDefaultMsg` 的第 2 个形参是 `nRecog: Int64`，此单元里的约定是「负载长度」：
  紧邻的 `:1250` 用 `Length(g_ProcessBlacklistStr)`、`:1258` 用 `Length(g_SendToClientSpeedIntervalsText)`。
- 但 `:1255` 这里传的是 `Length(sSendMsg)`，而 `sSendMsg` 在**本分支里恒为空串**
  （它只在 `:955` 的 `SM_CHECK_RUNGATE1` 分支被赋值）→ `Recog = 0`，而实际负载是
  `SizeOf(g_EatItemCDConfig)` 字节。极可能是从别处复制粘贴时漏改。
- 精确边界（避免误判）：**wire 上的长度字段没错** —— `AddServerMsg`（`MirClientContext.pas:9764`）
  把 `DataAddLen` 写进 `TRungateMsgHeader.DataLen`，而 `EncodeRunGateMsg` 用的是形参 `DataAddLen`
  而不是 `DefMsg.Recog`（`GateShare.pas:3482-3485`）。所以坏掉的只是**协议头里的 `Recog` 字段**，
  凡是按 `Recog` 解析该消息负载长度的客户端逻辑都会读到 0。

> 附带观察：`:1258` 把同一个长度写了两遍（`Recog = Length(文本)`，
> 同时 `Param = LoWord(字节长度)`、`Tag = HiWord(字节长度)`），
> 即"字符数"与"字节数"两种口径并存 —— GBK 下二者不等，移植时**不能合并**。

### 5.4 【死代码】`DoForwardToClientData` 的 `BufferLen <= 0` 永不成立

`:944` 已经 `if BufferLen < SizeOf(TDefaultMessage) then Exit`（< 16 即退出），
随后 `:979` 又判 `if BufferLen <= 0 then Exit` —— 永远为假。已按"不可达"处理并加注释，测试断言 `bufferLen <= 0` 归入 `TooShort`。

### 5.5 【空分支不 Exit】`SM_SENDNOTICE` 仍然会被转发

`RunGateUtils.pas:1267-1270` 内是空的 `begin end;`，**没有 Exit**，于是控制流落到 `:1358` 的
`Context.AddServerMsg(pDefMsg, DataAdd, DataAddLen)`。分类器把 `SendNotice` 单列成一种 Kind，
就是为了让调用方不要误判为"已处理"。

### 5.6 【启发式误判风险】`DoCheckRecvBuffer` 在"帧头布局未知"时读 `wIdent`

`RunGateUtils.pas:456` 发现 `dwCode <> RUNGATECODE` 后，`:458` 又用 `MsgHeader.wIdent`（**偏移 10**）
去判断是否 `GM_RUN_GATE_VER` 以区分"版本不配套"和"一般错误"。此刻对端帧头布局未知，
该字段只是启发式；布局不同就会误报/漏报。已按原样实现，并把两种错误分成不同枚举值。

### 5.7 【判定顺序导致尾部坏帧滞留】`Len <= 20` 先于坏帧检查

`RunGateUtils.pas:514-525`：消费完最后一帧后若剩余恰好 ≤ 20 字节，直接"留尾巴"返回，
**不会**再进下一轮去检查其中的坏魔术。于是坏帧被当成"未处理尾巴"永久滞留（若对端不再发数据就无法自愈）。
测试 `ScanM2_BadMagicInTailExactly20Bytes_IsNotDetected_LeftoverWinsFirst` 固化这一顺序语义。

### 5.8 【跨语言运算符优先级陷阱】Delphi 的 `shl/shr` 比 `+` 紧，C# 的 `<< >>` 比 `+` 松

- Delphi：`RunGateUtils.pas:421` / `:1762` → `(nLen shr 10 + 1) shl 10`
  按 Delphi 优先级 = `((nLen shr 10) + 1) shl 10`。
- C#：`>>`/`<<` 的优先级**低于** `+`，直译成 `nLen >> 10 + 1 << 10` 会变成 `nLen >> (10 + 1) << 10` → 静默错。
- 同样的形态还出现在 `RunGateUtils.pas:1708`、`uFrm*.pas` 与其它单元。本车道所有相关表达式都**显式加括号**。

### 5.9 【注释与代码不符】keepalive 参数

`Common/IocpCommon.pas:137-141`：
注释「设置30秒钟时间间隔」但代码是 `InKeepAlive.KeepAliveTime := 5000`（**5 秒**）；
注释「设置每30秒中发送１次的心跳」但代码是 `KeepAliveInterval := 1`（**1 秒**）。
已按**代码**移植（`IocpTcpKeepAlive`），并用测试把"注释不是真值"钉死。

### 5.10 【散列常量与注释不符】`TRunGate.Run` 的应答散列

`RunGateUtils.pas:1893-1918 / 1941-1966` 的注释逐条写着移位量，但代码实际移位量有 7 处不同，例如：
- `:1899-1900` 注释 `c ^= b >> 13`，代码 `c xor (b shr 15)`
- `:1902-1903` 注释 `a ^= c >> 12`，代码 `a xor (c shr 9)`
- `:1950-1951` 注释 `a ^= c >> 12`，代码 `a xor (c shr 11)`
- `:1956-1957` 注释 `c ^= b >> 5`，代码 `c xor (b shl 12)` ← **方向都反了**

照抄注释会得到完全不同的散列 → 与 M2 的合法性校验直接失配。已按代码移植并用**独立复算的黄金向量**锁定
（见 §7 复算命令）。

### 5.11 【不可达的异常分支】`IocpUtils.DoRecvBuffer` 的 1000 次护栏

`Common/IocpUtils.pas:611-621`：
```pascal
while (FSocket <> INVALID_SOCKET) do
begin
  if not DoCheckRecvBuffer(FRecvBuffers) then Break;
  ...
  Inc(I);
  if I >= 1000 then raise Exception.Create('DoCheckRecvBuffer no result False');
end;
```
而基类 `DoCheckRecvBuffer`（`:595-598`）**恒返回 False** → 第一轮就 `Break`，
`I >= 1000` 的异常**永远不可达**。只有子类覆写（如 `TMirRemoteContext.DoCheckRecvBuffer`）后才可能触发。

### 5.12 【注释与代码不符】`PostWSASend` "尝试10次"其实是 2 次

`Common/IocpUtils.pas:954-955`：注释 `//尝试10次,如果还不成功就返回false`，代码 `while I <= 2 do`。

### 5.13 【检测到短写但什么都不做】`ProcessIOQueued` 的发送分支

`Common/IocpUtils.pas:1360-1363`：
```pascal
if lvIOData.DataBuf.len <> lvBytesTransferred then
begin
end;
```
空 `begin end` —— 检测到发送字节数不符却既不报错也不补偿。属未完成代码。

### 5.14 【活分支少了就绪门】`OnTimerCheckConnect` 的 `FIsReady` 条件

- 死分支（`UseIocpClient=0`，`:4259/:4265`）：`RunGate.FIsReady and RunGate.FClientSocket.Active and ...`
- 活分支（`:4299/:4305`）：只有 `Context.Active and ...`（**没有** `FIsReady`）

即 IOCP 客户端"已连接但尚未就绪"时也会发心跳、也会判超时。已按**活分支**语义实现并注释。

### 5.15 【多数表决不是取最大值】`GetClientDate`

`RunGateUtils.pas:3948-3956`：遍历桶按**首次出现顺序**，返回**第一个** `Count >= AllCount div 2` 的桶，
不是票数最大者。`AllCount div 2` 还向下取整（21 个有效值时阈值是 10 而不是 11）。
测试 `Differential_ReturnsFirstBucketReachingHalf_NotTheMaximum` 固化。

### 5.16 【双重门限】`GetClientDate` 的样本数 vs 有效值数

`:3910` 要求 `List.Count >= 20`（样本数），`:3946` 又要求 `AllCount >= 20`（字段非 0 的个数）。
因此 25 个样本里只要有 6 个 `Date = 0`，即使其余 19 个完全一致也返回 0。

### 5.17 【死分支引用了未声明变量 → 该分支根本编不过】

`RunGateUtils.pas:4339` 的 var 块只声明了 `FullServiceMsgText_New, S: string;`
（`FullServiceMsgText_Old` 被注释在行尾），但 `:4342` 有 **未被注释** 的 `FullServiceMsgText_Old := '';`
（该行位于 `{$IF MultiThreadRunContext = 0}` 死分支的 `OnTimerRunContext` 内；
`:4398` 与 `:4433` 的同类引用都在 `{}` / `//` 注释内）。
在 `MultiThreadRunContext = 0` 下该单元**无法编译**（Undeclared identifier）——
反证该分支长期未编译/未维护。
> 对照：**活分支** `TFullServiceMsgProcessThread.Execute`（`:3256-3378`）里
> `:3263` 同样只声明 `FullServiceMsgText_New, S`，但 `FullServiceMsgText_Old` 的引用（`:3326`、`:3348`）
> **全部在 `{ }` 注释内**，所以活分支可以正常编译。

### 5.18 【有损排空】全服消息队列"只发 10 条"实际是"全部出队、10 条以外全丢"

活分支 `RunGateUtils.pas:3268-3343`（`TFullServiceMsgProcessThread.Execute`，
死分支 `:4345-4414` 结构相同）：`while FFullServiceMsgList.Count > 0` 把**所有**条目出队并 `FreeMem`，
只有 `Count < 10` 且带负载的会被 `EncodeRunGateMsg` 编进 `FullServiceMsgText_New`，其余**直接丢弃**。
且"10 条"的计数（`:3342 Inc(Count)`）对**每一条**都自增，包括 `pBuffer = nil` 的条目 ——
它们占了配额又被丢。
测试 `Drain_PayloadLessEntriesConsumeTheTenSlotWindow` 固化（12 条里第 0 条无负载 → 编码 9、丢弃 3）。

> 另一个活分支细节（本车道**未**移植，登记在 §6.1）：`:3348` 的
> `if Length(FullServiceMsgText_New) > 0 then ...` 意味着"本轮全被丢弃"时**一条都不发**；
> `:3362` 只对 `not MirContext.boIsOldClient` 的会话调用 `AddServerText`。

### 5.19 【不检查就回发】缓存命中分支不看槽是否已装载

`RunGateUtils.pas:1227-1228`：`sTemp := FCacheDatas[Ident - SM_MODULEMD5_CACHE]; Context.AddServerMsg(@TheDefMsg, PChar(sTemp), Length(sTemp));`
没有"该槽为空则回落到向 M2 请求"的逻辑 → 客户端在 M2 首次推送前请求会拿到**空负载**。
照原样保留，测试 `DataCache_MissReturnsEmptyArray_ButStillCountsAsAReply` 固化。

### 5.20 【静默丢弃】`_FD_SET` 满 64 个直接丢

`Common/IocpWinsock2.pas:3163-3170`：`if fdset.fd_count < FD_SETSIZE` 不成立时**无日志、无返回**，调用方无法感知。
与 Windows `FD_SET` 宏行为一致，故保留；但已测试固化 `Set_OverflowBeyond64_IsSilentlyDropped`。

### 5.21 【口径失真】审计文档行数与实际不符

见 §0 表格（最多差 19%）。建议修正 `tools/audit-coverage.ps1` 的行数统计口径，或在下游报告里标注"非物理行数"。

### 5.22 【潜在越界】`GM_CLOSE` 的会话下标无范围校验，字段为 0 时得 -1

三处"按帧头字段找会话"用了**两种不同**的下标派生：

| 分支 | 位置 | 写法 |
|---|---|---|
| `GM_SERVERUSERINDEX` | `RunGateUtils.pas:582`（另见 `:798`） | `Contexts[MsgHeader.wGSocketIdx]`（**直接用**） |
| `GM_KICK` | `RunGateUtils.pas:658`（另见 `:867`） | `Contexts[MsgHeader.wGSocketIdx]`（**直接用**） |
| `GM_CLOSE` | `RunGateUtils.pas:678`（另见 `:884`） | `Contexts[MsgHeader.wUserListIndex - 1]`（**减 1**） |

`GM_CLOSE` 分支对 `wUserListIndex` **没有任何范围校验**。若该字段为 0（GM_* 帧常不填用户序号），
表达式在 `LongWord` 域里算出 `$FFFFFFFF`，再当 `Integer` 下标用 → **-1**；
Delphi 默认关范围检查时 `List[-1]` 读的是数组首元素**之前**的内存并强转成 `TMirClientContext`
→ 紧接着 `Context.Socket` 解引用 → 访问违例或野指针。
另外 `wUserListIndex >= $80000001` 时会回绕成一个大正数（同样越界）。

处置：不擅自"修正"（否则与 `GM_SERVERUSERINDEX`/`GM_KICK` 的 1-based 约定不一致、
且 M2 侧发送端才是真源）；在 `RunGateContextLookup` 里把两种派生方式显式并列，
并由 `ContextLookup_GmCloseWithZeroUserIndex_YieldsNegativeIndex` /
`ContextLookup_GmCloseHugeValue_WrapsInUnsignedDomain` 固化。

### 5.23 子代理 A（`uBuffer.pas`）发现的原文缺陷

> 全部照抄原文语义并加断言固定；括号内为处置。

1. **阈值 off-by-one（真有差异）**：`:358` 是 `FFreeCount + 1 < FMaxFreeBlocks`（严格小于），
   而 `:415` 是 `FFreeCount + 1 <= FMaxFreeBlocks`。**同一状态**下 `FreeMemory` 会真释放、
   `FreeMemoryBlock` 会挂回链 —— 两个入口行为不同。（照抄 + 差异断言）
2. **`:343-346` 无 nil 保护 → AV**：`FreeMemory` 里 `pBlock := FUseHead` 后直接进 `while true` 解引用；
   **重复释放最后一个块**时已使用链为空 → 原文访问违例，"重复释放是 no-op" 的直觉不成立。（托管侧加 `null` 守卫）
3. **`:281-284` `Clear` 不重置 `FUseCount`/`FFreeCount`**：链已清空但计数保留旧值。
4. **`:720-721` Read 的 `Count := FSize - FPosition` 可能为负** → `:766/:774` 解引用 nil。
5. **`:780-784` 多跨一次块导致游标停在 nil**：从块首一次性读**正好整数个块**后 `FCurBlock = nil`、
   `FPosition` 只记到 256（实际消耗 300）→ 后续 Read 返回 0。根因是 `:757-761` 与 `:780-784` 的重复跨块判定。
6. **`:1136` + `:1150` `SwapStreamLink` 不交换 `FSize`**：接收方拿到的是**发送方旧值**，
   而发送方在对方为空时 `FSize := 0` → "块链易主但两边 FSize 都是 0"，`FCapacity/FMemBlockCount` 却已更新
   → 后续 `:898` 算出**负数**长度（`128 - 384 + 0 = -256`）。
7. **`:1237-1241` 的 nil 游标复位只在 `Write` 里有、`WriteStream` 没有**：写满恰好一整块后紧接 `WriteStream`
   → `:1268` 解引用 nil。（托管侧加守卫）
8. **`:24` `DataLen: Word` 截断**：`:1377` 把 `Cardinal` 直接赋给 `Word`，`len >= 65536` 静默截断；
   `:1395` 还把 SuperLarge 块的 `DataLen` 硬编码成 `SuperMemoryPool.FBlockSize`（2048，不是本池的 16384）。
9. **`:1417` `AddMemBlockLink` 第一行覆盖入参** → 传进来的块被丢弃，等价于"追加一个空 Small 块"（明显笔误）。
10. **`:1599` 的 `for I := lvPosition to lvBuf.DataLen - 1`**：`DataLen = 0` 时上界**下溢成 `$FFFFFFFF`** → 越界读。
    （`:1622` 的 `raise` 在良构数据下不可达；`:1615` 的 `Break` 只保护"当前块为空"。）
11. **`:1599-1608` / `:1611` 未找到分隔符时返回 0，但中间块已被拷进 buf、游标已推进**。
12. **`:1669-1686` `ValidCount` 用 Cardinal 表达式赋给 `int Result`** → 下溢时结果巨大/负数。
13. **`:1784-1794` vs `:1796-1806` 两个"相等"分支判据字段不同**（一个判 `FReadPosition = 0`、
    一个判 `FWritePosition = 0`）。"两位置相等即视为**满**"是原文刻意行为（子代理最初按"空"写的 4 条测试已按原文改正）。
14. **`:1932-1935` `TDxRingStream.Seek` 是空实现**（恒返回 0、不动游标）；
    `:2033` 写侧用的是 `FWRiteBlock`（Delphi 大小写不敏感所以能编译）。
15. **`:1711-1715` `TDxObjectPool.FDestroy` 第二循环 `FUses.Delete(FUnUses.Count - 1)` 是笔误**
    （在 `FUses` 上按 `FUnUses` 的下标删）。**实测可达**：`MaxObjCount=1`、`Get×2`、`Free×2` 后
    `FUses.Count(0) < FUnUses.Count(1)` → `FUses.Delete(0)` 越界抛 `EListError`。
16. **`:605-614` `LinkToBufferList` 清空列表不含 `FMemBlockType`**；`:1074` 后新块的 `DataLen` 未初始化（池复用残留旧值）。
17. **`GetMemory` / `GetMemoryBlock` / `InnerCreateBlock` 三份完全相同的抽块代码**，
    其中 `InnerCreateBlock`（`:518-524`）**从未被调用**（死代码）。
18. **`uBuffer` 在 Delphi 侧只有一个引用点**：`Common/IocpTcpClient.pas:9`，且被 `{$IFDEF USE_BUFFER_LINK}` 包住
    ⇒ 当前**没有活跃调用方**，这批类型是"备用设施"。接缝风险低，但也意味着没有端到端回归可依赖。

### 5.24 子代理 B（`uFrm*.pas`）发现的原文缺陷（节选，25 条全量见其交接）

1. `uFrmSafeFilter.pas:544` — `mniTempAddAllToBlockClick` 循环体用 `lstTemp.Items[lstTemp.ItemIndex]`
   而非 `Items[I]` → "全部加入永久过滤"把**当前选中项重复加 N 次**（未选中则加 N 个空串）。同族 `:614` 是正确的。
2. `uFrmSafeFilter.pas:802` + `:337` — `btnOKClick` 末尾是 `Close` 而**不是** `ModalResult := mrOK`，
   但 `ShowFrmSafeFilter` 判 `ShowModal = mrOk` → **确定按钮永远返回 False**。
3. `uFrmSafeFilter.pas:601-605` — `mniBlockClearClick` **没有 Lock/UnLock**（同族清空都有）；`:1424` 同型。
4. `uFrmSafeFilter.pas:1326` — `g_TempMacList.Delete(lstTempMac.ItemIndex)` 用 **UI 下标删列表**
   （若 `Sorted=True` 会删错项）；同族 `:1294-1296` 用 `IndexOf(Items[i])`。`:1439` 同型。
5. `uFrmMessageFilter.pas:137/139/151` — `btnEditClick` 中 `sInputText` 在"未选中项"路径**未初始化**
   → 走 `:145` 判空分支；若它意外非空，`:151` 会以 `ItemIndex = -1` 越界。
6. `uFrmMessageFilter.pas:84-90` — `case g_FilterSayMsgMode of` **无 else**：越界值时 5 个单选**都不勾选**。
7. `uFrmInterval.pas:178` — `if I >= HALF_SPEED_INTERVALS_COUNT` → I=200 走**正号分支**，
   `'+' + IntToStr(200-200)` = **`"+0"`**（不是 `"0"`）。
8. `uFrmGameSpeed.pas:658` vs `:790` — `FormCreate` 只建 `Low..amCutMeatToMove`（**24 行**），
   而 `btnSaveClick` 遍历 `Low..High` 写 **27 节** → 3 个并发模式**没有行可编辑却会被写盘**。
9. `uFrmGameSpeed.pas:769-781` — 两个 TrackBar 值**先夹到下限 3 再比较** →
   "超速次数=0、总记录数=4" 会**通过**校验并落盘 `SpeedValue=3`（**用户填 0 变成 3**）。
10. `uFrmGameSpeed.pas:720 / :836 / :1220` — 3 处被注释掉的赋值；`:1164-1172` `btnDefaultClick`
    **整个函数体是空的**。
11. `uFrmLogClientPacketSetting.pas:64-70` + `.dfm` — `chkLogOther` 在 DFM 里 `Checked=True Enabled=False`
    但代码**从不读它** → 掩码恒为 7 位。
12. `uFrmReadFileIP.pas:147` — `ModalResult := mrOK;;`（双分号，无害笔误）；
    `:104-146` `btnOKClick` **无 try..finally** → `TIniFile.Create` 后抛异常不释放（`:123-130` 同型）。
13. `uFrmProcessBlacklist.pas:52` vs `:151/:184` — `g_ProcessBlacklist`（小写 l）与 `g_ProcessBlackList`
    （大写 L）**大小写不一致**（Delphi 不敏感故等价；托管侧合并为一个字段）。
14. `uFrmReadFileIP.pas` 4 个 `TSpinEditEx` 的 `Left` 有 441 与 442 **差 1** 的原文不一致（照抄并断言）。
15. 键名"不可顺手统一"清单：`uFrmAntiPlugUpdateSetting.pas:64` 的 `AntiPlugUpdateConfigUrl5`（**末尾数字 5**）、
    `uFrmItemEatCD.pas:188` 的 `IntToStr(I+1) + 'NormalHP'` → `'1NormalHP'`（无分隔符、前缀从 1 起）、
    `uFrmInterval.pas:226` 的 `'Speed' + (I-200)` → 含负号的 `Speed-200..Speed200`、
    `uFrmHitInterval.pas:75-76` 的 `Speed0..SpeedN`（从 0 起）。

### 5.25 跨语言/跨库易错点（子代理 B 报告，值得全局复用）

1. **`GXX.Core.Rtl.DelphiRTL.Pos("")` 返回 0，而 Delphi 的 `Pos('', S)` 返回 1** ——
   两者语义不同。受影响处已显式补偿（`ProcessBlacklistLogic.FindIndex`、`MagicCDLogic.FindText`）。
   **建议在 `GXX.Core` 侧修正或改名**（属其它分区，本车道只登记）。
2. **`TSpinEditEx` 语义**：**编程赋值不裁剪**，`.dfm` 的 `Min/Max` 只影响上下按钮
   （依据：`.dfm` 里大量 `MaxValue=0 MinValue=0` 却有非 0 `Value`，且代码把 `MaxValue := High(Integer)` 后直接赋 1200）。
3. **xUnit 共享静态全局量**：12 个窗体测试类共享 `FormGlobals`（对应 Delphi 单元级全局变量），
   并行执行会互相清空/污染 → 已用 `[CollectionDefinition(..., DisableParallelization = true)]` +
   `[Collection("RunGateFormLane")]` 串行化（**不改 csproj、不加 AssemblyInfo**，符合车道约束）。
4. **`MessageBoxSeam.UiEnabled`**：新增的接缝开关（默认 `true`，生产行为不变），
   测试夹具置 `false` —— 否则真实 `MessageBox.Show` 会启动模态消息循环**挂死 testhost**
   （这就是本车道观测到的 `dotnet test` 10 分钟超时的根因）。
   **主窗体车道若要复用 `MessageBoxSeam`，请保留该开关。**
5. **`ListView.SelectedIndices` 需要已创建句柄**，无消息循环的单测里恒为空 →
   已加"索引镜像"（`SelectedRowMirror` 等）与决策镜像（`DeleteMenuItemShouldBeVisible` 等）。
   **生产运行时镜像恒等于真实值**，不改变语义。

---

## 6. 接缝与未完成（如实报告）

### 6.1 本轮**未覆盖**（按优先级）

| 优先 | 单元 | 规模 | 说明 |
|---|---|---|---|
| — | `MirClientContext.pas` | 11,126 LF | 仅薄壳。`TMirClientContext` 的包裹/药品/魔法 CD/防外挂/截图/进程上报等处理链未移植。它 `uses VerifyCodeUtils / BagItemList / MagicIntervalUtils / IocpTcpServer / IocpUtils / IocpWinsock2 / IocpCommon / EncryptUnit_LF / EDcode / Grobal2_Ex / GateShare`，是 RunGate 的"业务大脑"，建议单独一条 P3 车道 |
| — | `uFrmMain.pas` | 4,217 LF | 仅 `Program.cs` + `RunGateService.cs` + `GatewayKit/GateMainForm.cs` 的启动/主窗体壳 |
| — | `GateShare.pas` | 3,596 LF | 只覆盖了协议常量、`EncodeRunGateMsg`、`tick_diff`、几个配置默认值、节流配置。**未覆盖**：词表过滤/`LoadFilterSayMsgFile`、`g_TempIPList`/`g_BlockIPList`/MAC 黑名单的加载与落盘、`IsConnLimited`/`GetAttackCountOfIP`、`InputPassword(Ex)`、`SendGameCenterMsg`、`g_VerifyFailUserList`、验证服务器/暗桩全流程、`RebuildProcessBlacklist` |
| — | `AsyncCalls.pas` | 3,440 LF | 跨车道裁定上移 `GXX.Core/Async/`（台账 §8.5），本分区不含 `src/GXX.Core/**` |
| — | `ParadoxDataSet.pas` | 1,363 LF | 同族上移裁定（台账 §5:151） |
| — | `MagicIntervalUtils.pas` | 199 LF | 只有 `uFrmGameSpeedLogic.cs` 的接缝 |
| 高 | `RunGateUtils.pas` 的 socket/线程壳 | — | `TMirRemoteContext` / `TRunGate` / `TRunGateManager` / `TProcessServerReceiveThread` / `TFullServiceMsgProcessThread` / `TClientContextRunThread` 的**线程与生命周期**未 1:1 移植（本车道只移植了其中的纯逻辑切片）。`RunGateService.cs` 是功能等价但结构不同的实现 |
| 中 | `RunGateUtils.pas:3256-3381`（`TFullServiceMsgProcessThread.Execute`） | 126 LF | 只移植了其中的队列排空语义；`MirContext.Run(...)` + 反外挂重发时间窗（`tick_diff(FLastSendAntiPlugTick, now) >= 500`）未移植 |
| 中 | `RunGateUtils.pas:2073-2144`（`DoConnnectUser`） | 72 LF | Win32 提权 + `NtSetInformationProcess(ProcessBreakOntermination)` 反调试，属 DoD §2.3 不移植项；如需协议兼容，建议 `Stub` 返回 `true` |
| 低 | `RunGateUtils.pas:1983-2069`（`CheckCRC1`/`CheckCRC2`） | 87 LF | 原文**整段处于 `{ }` 注释内**（`:1983` 开、`:2069` 闭）→ 死代码，不移植 |
| 低 | `RunGateUtils.pas:3816-3899`（`GetClientRunGateIP`） | 84 LF | 原文**整段处于 `(* *)` 注释内** → 死代码。已在 `RunGateClientStat.GetClientRunGateIP` 保留对照实现，并由 `TIPCheckField` 差异断言说明"孪生但只有一个活着" |

**窗体族未覆盖（子代理 B 自报）**

| 项 | 说明 |
|---|---|
| `TVirtualStringTree` 就地编辑 | `uFrmGameSpeed.dfm` / `uFrmMagicCD.dfm` 用的第三方 `VirtualTrees.pas` 未移植。改为 `TreeView(OwnerDraw)` 复刻列文本/红字/勾叉，**编辑器的"创建/销毁/消息泵"未覆盖**；但全部取值与写回规则已抽成 `GameSpeedLogic.PrepareEditSpec` / `ApplyEditorResult` 并单测（原 `:385-572` 与 `:258-361` 的全部判定） |
| `uFrmMagicCD.DoOpen` 的文件版 | `TParadoxDataSet` 未移植（`ParadoxConv.cs` 只覆盖编码转换）→ 抛 `NotSupportedException`；数据读取改由 `IMagicDbReader` 接缝注入（`DoOpenWithReader`），过滤/去重/CD 回填逻辑已 100% 移植并单测 |
| `TParadoxDataSet` / 真实 `Magic.DB` 解析 | 同上（属 §2.3 第 30 行的跨车道裁定） |
| 像素级绘制断言 | `OnDrawText/OnAfterCellPaint` 的图标绘制在无头环境无法截图，**未做像素级断言** |
| `uFrmSafeFilter` 的 `TIOCPClientContextPool`/`TMirClientContext` | 以 `ISafeFilterClientPool`/`ISafeFilterClient` 注入；**真实 IOCP 上下文池未接线**（`MirClientContext.pas` 属其它车道） |
| `GateShare.pas` 的落盘类函数 | `SaveBlockIPList / SaveIPSectionList / SaveBlockMacList / AddBlockIP(Ex) / AddBlockMac / SaveProcessBlacklist / RebuildProcessBlacklist` 以 `ISafeFilterHost` / `IProcessBlacklistSink` 注入，默认内存实现只**计数**，未落盘（原文会 zLib+RSA+MD5） |
| `uFrmSafeFilter` 的自建对话框 `InputQueryEx` | 原 `:230-325`（比 `Dialogs.InputQuery` 多一个蓝色 Hint 标签、`FORM_WIDTH=280`、`Edit.MaxLength=255`、默认全选）：只保留三参数接缝，**未复刻自绘布局** |
| `g_Config.LoadConfig` 读取侧 | `GateShare.pas:632-1066` 只做了默认值 + 写盘侧；读取侧属主窗体车道 |
| UI 交互断言 | 鼠标点树节点/右键菜单弹出/TrackBar 拖动/模态结果显示/焦点/滚动位置 —— 受无头环境限制，改用"事件处理器直调 + 决策镜像"覆盖 |
| `uBuffer.pas` 的 24 条边界用例 | 见 §4.2（子代理 A 首轮以 `Skip` 登记；回炉后处置结果见 §4.2 的最终数字） |

### 6.2 有意留下的接缝（供下一步接管）

1. **`RunGateFrameScanner`** 是纯函数：以 `byte[] + len` 入参、返回帧切片列表 + 尾巴 + 错误枚举。
   接入真实 socket 时只需在外层把 `GateSession.Buffer` 传进来、把 `Leftover` 写回。
2. **`RunGateDataCache` / `RunGateFullServiceMsgQueue`** 已做线程安全，可直接被 `MirClientContext` 的托管对应物使用。
3. **`RunGateTiming` + `RunGateChallengeState`** 是定时器/状态机的纯内核，外层只需喂 `MyGetTickCount` 与 `Now`。
4. **`IocpWinsock2Compat`** 保留了 `TFdSet` 的 `fd_array` 语义；若未来要接 `select()` 才需要真实 P/Invoke。
5. **`IocpUtilsPolicy`** 是发送/接收策略内核，`GatewayKit.IocpManager` 若要补"发送分块上限 5120"与"良性错误白名单"
   可直接调用（当前 `GatewayKit` 未做这两件事，见 §6.3）。
6. **窗体族的共享接缝层 `uFrmGameSpeedLogic.cs`**（`GateShare.pas` 的替身：枚举 / `TAntiPlugConfig` /
   `TEatItemCDConfig` / `TProcessBlackList` / `TMagicIntervalList` / `TSafeHashStringList` / `RunGateConst` /
   `FormGlobals` / `MessageBoxSeam` / `ISafeFilterHost` / `IProcessBlacklistSink` / `IMagicDbReader` /
   `TSpinEditEx` / `TColorIndexEdit`）。
   **若后续 `GateShare.pas` 车道正式移植，必须做一次名字/语义对齐**（尤其 `TSumActionProcessMode` 的二级成员
   原文拼写是 `sampOffline`（`samp` 而非 `sapm`）—— 子代理 B 照抄了）。
   `MessageBoxSeam.UiEnabled` 开关（默认 `true`）**必须保留**，否则任何窗体测试都会挂死 testhost。
7. **xUnit 串行化集合**：`[CollectionDefinition("RunGateFormLane", DisableParallelization = true)]` 定义在
   `RunGateUtilsFormAntiPlugUpdateSettingTests.cs` 内（**不改 csproj、不加 AssemblyInfo**）。
   新增窗体测试类必须挂 `[Collection("RunGateFormLane")]`，否则会因共享 `FormGlobals` 静态全局量互相污染。
8. **`uBuffer` 的测试探针**：`uBuffer*.cs` 里所有 `*Internal` / `F*` / `*Block` 探针都是 `public` 只读
   （测试工程是独立程序集，`internal` 不可见）。若后续要收紧可见性，需要同时改测试或不使用这些断言。

### 6.3 给集成者的 `GatewayKit` 差异清单（本车道无权改）

| 差异 | 位置 | 建议 |
|---|---|---|
| `RUNGATECODEX` 常量错（见 §5.1） | `GatewayKit/GatewayProtocol.cs:22` | 改为 `Grobal2Const.RUNGATECODEX` |
| 发送无 5120 字节/次分块上限 | `GatewayKit/GatewayProtocol.cs:251-274`（`Send`） | 需要时改用 `IocpSendCachePolicy.MaxChunkBytes` |
| 无良性 Winsock 错误白名单 | 同上 | 需要时改用 `IocpRecvPolicy.IsBenignWsaError` |
| `GateSession.AppendBuffer` 溢出时**整段清零**（丢数据） | `GatewayProtocol.cs:117-127` | 原文是 `ReallocMem` 扩容（`RunGateUtils.pas:1706-1710`），语义不同 —— 已在映射表登记为"结构不同" |

---

## 7. 大段常量/散列的"脚本抽取 + 回读比对"

### 7.1 常量表（`GM_*` / `SM_*` / 魔数）

全部用 `Select-String` 从 Delphi 源与 `GXX.Core/*.g.cs` 双向抽取后逐项比对（非手工转录）：

```powershell
# Delphi 侧（Grobal2_Ex.pas:667-694）
Select-String -Path 'Source\RunGate\Grobal2_Ex.pas' -Pattern 'GM_[A-Z_0-9]+\s*=' -Encoding Default
# C# 侧（GXX.Core）
Select-String -Path 'src\GXX.Core\CommonConst.g.cs'   -Pattern 'public const .* (GM_|SS_)'
Select-String -Path 'src\GXX.Core\Protocol\Grobal2.Const.g.cs' -Pattern 'RUNGATECODE|RUN_GATE_MSG_CODE|SM_.*_CACHE ='
```
比对结论：`GM_*` 16 个值、`SM_*_CACHE` 14 个值（10104..10117 连续）与 Delphi 侧**完全一致**；
唯一不一致的就是 §5.1 的 `RUNGATECODEX`。

### 7.2 散列黄金向量（PowerShell 独立复算，非 C# 自证）

用 Int64 + 显式 `mod 2^32` 算术（与 C# 的 `uint` 回绕是两条不同实现路径）复算：

```
0,0                        -> 0x724DCB78
1,0                        -> 0xD623D13C
305419896, 2596069104      -> 0x74B38D26
4294967295, 4294967295     -> 0xA944C93D
3735928559, 16909060       -> 0xEA554FFF
```
（脚本见本次会话记录；5 个向量已写入 `RunGateUtilsHashTests.ComputeChallenge_GoldenVectors` 的 `[InlineData]`。）

### 7.3 `SM_*_CACHE` 连续性校验

`RunGateCacheTable.VerifyContiguity()` 是这条不变量在代码里的可执行形式（原文 `:1227` 用
`Ident - SM_MODULEMD5_CACHE` 直接做下标算术，一旦出现空洞就读错槽位）。

---

## 8. 与台账 §9.4 / §10 / §11 的呼应

- **§9.4（`GetValidStr3` 根因修复）**：本车道未新增本地复刻，直接使用 `GXX.Core.HUtil32`。
- **§10 第 11 条（"无同名 .dfm"偏差）**：**不适用于 RunGate 窗体** —— 子代理 B 已核实
  `Source\RunGate\` 下 12 个 `uFrm*.pas` **全部有同名 `.dfm`**（含 `uFrmLogClientPacketSetting` /
  `uFrmHitInterval` / `uFrmAddProcessBlack` / `uFrmAntiPlugUpdateSetting`）。台账该条应在此车道标注"已排除"。
- **§11.3（陈旧构建产物）**：本车道再次踩到"多 agent 共用 `obj/`"的变体（见 §4.1 的 `obj_x` 踩坑），
  处置是隔离验证工程 + 提交前一刀清 `obj/`。
- **§11.4（不要用 `!= -1` 当成功判据）**：本车道的 `LiveIdentToIndex` 返回 `-1` 表示未命中，
  但测试一律断言**具体槽位下标**或**精确的枚举 Kind**，不依赖 `!= -1`。
- **§11.5（生成物冲突）**：本车道**未**运行 `tools/audit-coverage.ps1`，未生成/修改
  `docs/并行覆盖审计.md`；映射表为本报告 §2 手工整理（证据取自 grep）。

---

# 第二轮（宿主重启后的续跑）：GateShare.pas 收口 + uFrmMain.pas 纯逻辑切片

> 基线：上一轮 HEAD `e06d90c4`（`GXX.RunGate.Tests` = **1206 例全绿**）
> 本轮结束：`GXX.RunGate.Tests` = **1535 例全绿**（+329），`dotnet build GXX.slnx -c Debug` = 0 错误

## 9. 本轮 commit

| # | hash | 内容 | 新增用例 |
|---|---|---|---|
| 1 | `7fa84159` | `MagicIntervalUtils.pas`（198 LF）**全文件 1:1** + 合并 `uFrmGameSpeedLogic.cs` 的 `TMagicIntervalList` 接缝 | 43 |
| 2 | `f1aae9cc` | GateShare 容器/地址/全局量族（`TAddressList/Ex`、`TSafeStringList`、`TSafeMemoryStream`、`TProcessBlacklist`、`inet_addr/ntoa`、`IP2Long/Long2IP`、hex 助手） | 78 |
| 3 | `36696ceb` | GateShare 名单加载/落盘 + 日志 + 进程黑名单重建 | 71 |
| 4 | `d6aef672` | GateShare 判定谓词/定时/口令/版本/GameCenter + 客户端反外挂加载（暗桩侧） | 85 |
| 5 | `15bea762` | `uFrmMain.pas` 纯逻辑切片 | 52 |

## 10. 条件编译开关：**复核结论（含一处对上一轮的重要补充）**

本轮逐行复核了 `{$IF}` 用到**全部**开关，结论与上一轮 §1 一致，并新增一条**上一轮遗漏的开关**：

| 开关 | 定义处（**都是 `const`，不是 `{$DEFINE}`**） | 值 | 后果 |
|---|---|---|---|
| `CLIENT_ANTIPLUG` | `Grobal2_Ex.pas:10` | 1 | `CheckClientAntiPlugDllChanged`/`LoadClientAntiPlugDll` **是活代码** |
| **`VERSION_TYPE`** | **`Grobal2_Ex.pas:15`** | **2** | ★ **新增结论**：`GateShare.pas` 三处 `{$IF VERSION_TYPE = 1}` **全为假** → `:436-438`（`CheckInWhiteList` 声明）、`:1479-1536`（`AddToDefFilterSayMsgList` 本体 + `CheckInWhiteList` 实现）、`:1549-1551`（`LoadFilterSayMsgFile` 里的调用）全是**死代码** |
| `NEED_REGISTER` | `Grobal2_Ex.pas:18` | 1 | 但所有 `WL*` 注册校验体被 `//` 整段注释、替换成 `IsKeyOK := True;` → `IsKeyOK` **恒真** |
| `REGISTER_TEST` | `Grobal2_Ex.pas:21` | 0 | — |
| `MultiThreadRunContext` | `Grobal2_Ex.pas:24` | 1 | — |
| `LOG_PLUG_DATA` | `Grobal2_Ex.pas:26-30` | 0（`NEED_REGISTER <> 0`） | — |
| `RungateLEG_IOCP` | `Grobal2_Ex.pas:32` | 6 | `FileFlag = $0533BF07`、`Key[8] = A7 45 32 BB 3D 6A 7F 90`、aks1024 私钥 = 脚本抽取的 256-hex |
| `UseIocpClient` | `Common/IocpCommon.pas:14` | 1 | 上一轮结论成立；并**新增**其在 `uFrmMain.RecallPreAllocatedSize` 的作用：`+ 1MiB × 60` |
| `USE_SPINLOCK` | `Common/iocp.inc` | **未定义**（`{.$DEFINE USE_SPINLOCK}` 被注释） | 所有 `{$IFDEF USE_SPINLOCK}` 的"带名字参数"被编译掉；活的永远是 `TRTLCriticalSection` 分支 |
| `SHARE_POOL_MODE` | `Common/iocp.inc` | **已定义** | 共享 `IODataPool`/`TIOCPClientContextPool` |

> **给后续车道的警告**：本轮有一个只做静态扫描的子代理得出过
> "这些开关在 worktree 里 UNDEFINED → Delphi 按 0 处理"的结论。**那是错的**：
> Delphi 的 `{$IF}` 表达式可以引用 **`uses` 子句里单元**的 `const`（本仓库所有开关都是这种写法），
> 不需要 `{$DEFINE}`。判断时必须直接读 `Grobal2_Ex.pas` / `IocpCommon.pas` / `iocp.inc`。

## 11. GateShare.pas（3595 LF）逐段覆盖表

| 行号 | 内容 | 处置 |
|---|---|---|
| `:13-26` | `tRunGate`/`GATEMAXSESSION`/`MSGMAXLENGTH`/`SENDCHECKSIZE(Max)`/`sSTATUS_*`/`HALF_SPEED_INTERVALS_COUNT`/`SPEED_INTERVALS_COUNT` | 上一轮已覆盖（`RunGateUtilsProtocol`/`RunGateUtilsHeartbeat`） |
| `:29-35, 45-115, 117-199, 211-215` | `TSockaddr`/`TAddressList`/`TAddressInfo`/`TAddressListEx`/`TSafeHashStringList`/`TSafeStringList`/`TSafeMemoryStream`/`TProcessInfo`/`TProcessBlacklist`/`TIPSection` 声明 | **本轮 `GateShareContainers.cs`** |
| `:217-255` | `TBlockIPMethod`/`TFilterSayMsgMode`/`TActionProcessMode`/`TSumActionProcessMode`/`TAntiPlugAction`/`TSpeedIntervals`/`TAntiPlugActionMode` | 上一轮 `uFrmGameSpeedLogic.cs`（`sampOffline` 拼写已核对为照抄） |
| `:257-418` | `TBaseAction`/`TAntiPlugConfig`/`TGameSpeed`/`TAntiPlugAddData`/`TItemCDTime`/`TEatItemCDConfig`/`TClientAntiPlugIdents` + 接口声明 | 上一轮 + 本轮 `TAntiPlugAddData` |
| `:447-532` | `GateClass`/`GateName`/`ActionProcessModeNames(2)`/`SumActionProcessModeNames`/`AntiPlugActionModeNames(_2/_3)`/`Sections` 表 | 上一轮 `RunGateConst` |
| `:535-623` | `TRunGatePlugClientInfo`/`TPlugInitRecord`/插件函数指针 + 反外挂全局量 | 上一轮 `RunGatePluginInterface.cs` + 本轮全局量 |
| `:625-1066` | `g_DefaultConfig`/`g_Config` 默认值 | 上一轮 `FormGlobals.CreateDefaultConfig` |
| `:1067-1345` | 其余 `g_*` 全局量 | 上一轮 `FormGlobals` 子集 + **本轮 `GateShareGlobals.cs`**（补齐 60+ 个） |
| `:1351-1365` | `ActionModeUseSpeedIntervals` | 上一轮 `FormGlobals` |
| `:1367-1390` | `RebuildSendToClientSpeedIntervalsText` | 上一轮 |
| `:1392-1432` | `AddTempBlockIP`/`AddBlockIP`/`AddTempBlockMac`/`AddBlockMac` | **本轮 `GateShareLists.cs`** |
| `:1434-1477` | `AddMainLogMsg`/`AddIOCPLogMsg` | **本轮** |
| `:1458-1464` | `tick_diff` | 上一轮 + 本轮 `GateShareRuntime.tick_diff` |
| `:1481-1504` | `AddToDefFilterSayMsgList`（**死代码**，见 §10） | **本轮**（按 1:1 保留 + 不接入活路径） |
| `:1506-1535` | `CheckInWhiteList`（**死代码**） | **不移植**（登记：需 `DecryString_LF` + 10 条加密串，且 `VERSION_TYPE = 2` 永不可达） |
| `:1538-1553` | `LoadFilterSayMsgFile` | **本轮** |
| `:1555-1634` | `ReadFYDenyIPListFile`/`ReadFYPassIPListFile`/`ReadFYDenyMACListFile` | **本轮** |
| `:1636-1693` | `LoadBlockIPFile`/`SaveBlockIPList`/`LoadBlockMacFile`/`SaveBlockMacList` | **本轮** |
| `:1695-1775` | `LoadIPSectionList`/`SaveIPSectionList` | **本轮** |
| `:1777-1803` | `LoadDBAddressTable` | **本轮** |
| `:1805-1864` | `IsHexString`/`StrToHexEx`/`HexToStrEx` | **本轮 `GateShareAddressUtils.cs`** |
| `:1866-1979` | `LoadProcessBlacklist`/`RebuildProcessBlacklist`/`SaveProcessBlacklist` | **本轮**（RSA 走接缝） |
| `:1981-2014` | `LoadNoVerifyChrList` | **本轮** |
| `:2016-2519` | `CheckClientAntiPlugDllChanged`/`LoadClientAntiPlugDll` | **本轮 `GateShareAntiPlug.cs`**（RSA/DES 走接缝） |
| `:2523-2534` | `SendGameCenterMsg` | **本轮 `GateShareRuntime.cs`** |
| `:2538-2704` | `TAddressList` 实现 | **本轮 `GateShareContainers.cs`** |
| `:2708-2834` | `TAddressListEx` 实现 | **本轮** |
| `:2838-2969` | `TSafeHashStringList`（复用接缝）/`TSafeStringList`/`TSafeMemoryStream` 实现 | **本轮**（前者的 `IndexOf` 语义已改正，见 §12-6） |
| `:2971-2990` | `ReverseBytes`/`IP2Long`/`Long2IP` | **本轮 `GateShareAddressUtils.cs`** |
| `:2992-3198` | `CheckInFYDeny/PassIPList`/`IsBlockIP`/`IsBlockMac`/`IsConnLimited`/`GetAttackCountOfIP`/`GetConnectCountOfIP` | **本轮 `GateShareRuntime.cs`** |
| `:3200-3224` | `InitIntervals` | **本轮** |
| `:3261-3374` | `TProcessBlacklist` 实现 | **本轮 `GateShareContainers.cs`** |
| `:3376-3387` | `InitActionIntervalsFileNames` | **本轮 `GateShareLists.cs`** |
| `:3389-3473` | `InputPassword`/`InputPasswordEx` | **本轮 `GateShareRuntime.cs`**（自绘对话框走 `MessageBoxSeam` 接缝） |
| `:3475-3494` | `EncodeRunGateMsg` | 上一轮 `RunGateUtilsProtocol.cs:157-` |
| `:3496-3502` | `MyGetTickCount` | **本轮** |
| `:3505-3543` | `GetFileVersionNumber`/`GetFileVersionStr` | **本轮** |
| `:3545-3592` | `initialization`/`finalization` | **本轮**（`GateShareGlobals.ResetForTest` 承载等价初值；用 exe 目录拼路径的项走 `GateSharePaths` 接缝） |

**结论：`GateShare.pas` 除 `CheckInWhiteList`（死代码，有意不移植）外，已**全部有处置结论**。**

## 12. 本轮新发现的原文缺陷 / 易错点（带 `文件:行`）

1. **`GateShare.pas:1873` + `:1963` 复制粘贴缺陷**：`LoadProcessBlacklist` 与 `SaveProcessBlacklist` 的**第一行都是 `g_DBAddressList.Clear;`** —— 清的是 **DB 地址表**，不是进程黑名单（从 `LoadDBAddressTable:1783` 抄来）。加载/保存进程黑名单会把 DB 地址表清空，进而破坏 `uFrmMain.ServerSocketDBClientConnect` 的地址授权。
2. **`GateShare.pas:1609-1620` 三份"读名单"的 Clear/FileExists 顺序不一致**：`ReadFYDenyIPListFile:1564-1565` 与 `ReadFYPassIPListFile:1591-1592` 是"先判存在再 Clear"（文件缺失时保留旧数据），而 `ReadFYDenyMACListFile:1617/1620` 是"**先 Clear 再判存在**"（文件缺失也会清空）。
3. **`GateShare.pas:3382` 条件恒真**：`if Length(g_sActionIntervalsFileNames) > 0 then` 判的是**静态数组长度（恒 27）**而不是元素字符串 → 原本为 `''` 的 7 个槽位会被拼成 **exe 目录路径**（`"C:\...\RunGate\"`）；且该函数**不幂等**（重复调用会叠加前缀）。
4. **`GateShare.pas:2619/2620` 与 `:2789/2790` 的 `INADDR_NONE` 守卫是死代码**：`nIP: Integer` 收下 `inet_addr` 的 `$FFFFFFFF` 后是 **-1**，而 `INADDR_NONE` 是无类型常量 `$FFFFFFFF`（Delphi 定型为 `Cardinal`）；`Integer = Cardinal` 按 **Int64 提升**比较 → 恒不相等。后果：**非法 IP 字符串也会入名单**，`nIPaddr` 存成 **-1**（即 `255.255.255.255`）→ 落盘时写成 `255.255.255.255`（`SaveBlockIPList`），并且"255.255.255.255"会被判为**重复**。（正是交接纪律里"不要用 -1 当哨兵"的那个坑。）
5. **`GateShare.pas:1825/1832` `StrToHexEx` 的 `I: Byte` 计数器**：`for I := 1 to Length(S)` 在 `Length(S) > 255` 时 Byte 回绕 → Delphi 侧**死循环**。托管侧用 `int` 计数器（已登记差异 + 差异断言）。
6. **`uFrmGameSpeedLogic.cs` 接缝的三处语义偏差（本轮已改正）**：
   - `TSafeHashStringList.IndexOf` 原本用 `List<string>.IndexOf`（**大小写敏感**），而原文类型是 `THashedStringList`（继承 `TStringList`，`CaseSensitive` 默认 **False**）→ `LoadNoVerifyChrList`/`AddBlockMac` 的去重语义会偏离；
   - `TMagicIntervalList.SaveToFile` 原本用 `TIniFileEx` 写 `[Interval]` 节（**接缝臆造**），原文是 `TStringList` 直写明文 `MagicId=Interval`（`uFrmMain.pas:478` 的文件名也是 `MagicCD.txt` 而非 `.ini`）；
   - `TProcessBlackList.Add` 漏了原文 `:3301` 的 `UpperCase(ProcessMD5)`，且 `MaxCount` 被做成可写（原文 `:189` 只读）。
7. **`GateShare.pas:3202-3214` `InitActionIntervals` 只有 25 个初始值、枚举有 27 个成员**：`amSpellConcurrent`(25)/`amMoveConcurrent`(26) 落回 **0**；且 25 行的**行尾注释与下标系统性错位**（例：`:3207` 注释"走路到魔法, 魔法到走路"，下标 8/9 实为 `amRunToHit/amHitToRun`；`:3214` 注释"三个并发"，下标 22/23/24 实为 `amMoveToCutMeat/amCutMeatToMove/amHitConcurrent`）。本实现**只认数组序**。
8. **`GateShare.pas:2476` vs `:342-348` 反外挂模块"永远加载不上"的推断**：`TAntiPlugAddData` 无 packed、无 `{$A}` 指令 → `SizeOf = 20`；而同函数的 `RSA.KeySize := aks128` 配的模数是 32 hex = **16 字节**，`aks1024` 配的是 256 hex = **128 字节**。若 `DecryptBuffer` 返回与模数同宽的字节数，则 `OutSize = SizeOf(AddData) = 20` **永假** → 两个函数恒返回 False。**标 UNVERIFIED**（`LbRSA.pas` 全树缺失，无法实跑复核）。
9. **`GateShare.pas:2502` 除零**：`(len + g_ClientAntiPlugDllBlockSize - 1) div g_ClientAntiPlugDllBlockSize`，而 `g_ClientAntiPlugDllBlockSize` 初值 **0**（`:597`）且本函数不校验 → `EDivByZero`。托管侧保留除零语义（`DivideByZeroException`）。
10. **`GateShare.pas:1552/1655/1687/1747` 的日志等级是 4，而 `g_btShowLogLevel` 默认 3**：`4 <= 3` 为假 → 这 4 条"加载完成"日志**一条都不会进** `g_MainLogStrings`。
11. **`GateShare.pas:2529` `cbData := Length(sSendMsg) + 1` 是字符数，而负载是字节数**：GBK 下中文 2 字节/字 → `cbData` 小于实际缓冲长度（接收方 `MyMessage` 用 `StrPas` 读到 NUL 为止，功能上无碍，但协议字段口径不一致）。
12. **`uFrmMain.pas:3449` off-by-one**：`if FSearchIndex >= lvContextProcessListInfo.Items.Count - 1 then FSearchIndex := 0;` 应为 `>= Count` → `FSearchIndex = Count-1`（合法）被重置为 0，"下一个"永远搜不到最后一行。
13. **`uFrmMain.pas:3429/3458` 空语句**：`ListItem.Selected;` 是**属性读取**（无副作用），应为 `Selected := True`。
14. **`uFrmMain.pas:3782-3785` 越界读**：早退分支读 `ListItem.SubItems[0]`，当 `SubItems.Count = 0` 且 `ListItem <> nil` 时 Delphi 抛 `EListError`。托管侧安全返回 `""`（已登记差异）。
15. **`uFrmMain.pas:3630-3633` 模糊匹配用 `Pos(...) > 0`**：大小写**敏感** + 空关键字命中一切（Delphi `Pos('', S) = 1`）；而 `GXX.Core.Rtl.DelphiRTL.Pos("")` 返回 **0** → 本轮的 `RunGateMainLogic.PosDelphi` 显式补偿了这一跨库差异（与前任 §5.25-1 同一问题，第二次踩到）。
16. **`GateShare.pas:2953(嵌套 UnixDateToDateTime)` 硬编码 UTC+8**（`IncHour(Result, 8)`），PE 时间戳换算无时区接缝。
17. **`GateShare.pas:1873` 之外还有**：`LoadBlockIPFile:1651`/`LoadBlockMacFile:1682` 都**不先 Clear**（`LoadBlockMacFile` 靠 `LoadFromFile` 内部 `SetTextStr` 的 `Clear` 才等价替换，`LoadBlockIPFile` 则是纯追加）；`LoadBlockIPFile` 还**没有 `IsIpaddr` 过滤**（与同族 `ReadFYDenyIPListFile` 不一致）。
18. **`uFrmMain.pas:3845` 的 `SizeOf(OVERLAPPEDEx)` = 36**（`OVERLAPPED`20 + `TWSABUF`8 + `TIoType`4 + `AllocSize`4，32 位对齐）—— **推断值**，托管侧登记为 `RunGateMainLogic.OVERLAPPED_EX_SIZE`。

## 13. 本轮留下的接缝（供集成者接管）

| 接缝 | 位置 | 默认行为 | 生产接线需要 |
|---|---|---|---|
| `GateSharePaths.ExeDir` / `ParamStr0` | `GateShareLists.cs` | 真实 exe 目录（带尾随分隔符） | 无需接线；测试用 `SetExeDirForTest` |
| `GateShareLists.IProcessBlacklistCipher` | `GateShareLists.cs` | **恒等 + 计数**（原文 `LbRSA` aks128 公钥加密） | 移植 `LbRSA.pas` 或提供等价 RSA；参数已登记（`597A…`/`CF2C…`） |
| `GateShareLists.ZlibCompressBuffer` | 同上 | `EDcode.zLibCompressBuffer` | 无需 |
| `GateShareLists.NowProvider` | 同上 | `DateTime.Now` | 无需 |
| `GateShareRuntime.TickProvider` | `GateShareRuntime.cs` | `Environment.TickCount` | 若要 `timeGetTime` 的 1ms 精度需另接 |
| `GateShareRuntime.InputPasswordQuery` | 同上 | `MessageBoxSeam.InputQueryWithValue`（**自绘对话框未复刻**） | 需要"密码掩码 + MaxLength=255 + 默认全选"的对话框 |
| `GateShareRuntime.FileVersionProvider` | 同上 | `FileVersionInfo` | 无需 |
| `GateShareRuntime.SendCopyData` | 同上 | 真 `user32!SendMessageW(WM_COPYDATA)` | 无需 |
| `GateShareAntiPlug.Cipher` | `GateShareAntiPlug.cs` | **RSA 返回 null（安全失败）+ DES 走 `UnitDes.DecryptDes`（Latin-1 key）** | ① 移植 `LbRSA.pas`；② 给 `GXX.Core.Crypto.UnitDes` 加 **`byte[] key` 重载**（现在只有 `string key`，内部按 **GBK** 编码 → 8 字节原始 key 会失真） |
| `RunGateMainLogic.OVERLAPPED_EX_SIZE` | `uFrmMainLogic.cs` | 36（推断） | 接线真实 Win32 结构尺寸 |
| `IProcessBlacklistSink`（既有） | `uFrmProcessBlacklist.cs` | 空实现 | **本轮已提供真实实现 `GateShareProcessBlacklistSink`**（见 §17），把它赋给 `ProcessBlacklistUnit.Sink` 即可落盘 |
| `ISafeFilterHost`（既有） | `uFrmSafeFilter.cs` | `InMemorySafeFilterHost` 只计数 | **本轮未接线**：接口把 `TempIPList`/`BlockIPList` 声明为 `TSafeHashStringList`（字符串表，窗体直接读写并遍历），而原文这两者是 `TAddressList`（只存 `nIPaddr` 数值、不存字符串）。要做成真实实现必须加一层"字符串视图 ↔ TAddressList"的双向同步，属于**会改动 12 个窗体测试**的改动，本轮不做（**已登记为后续事项**，见 §17） |

## 14. 未完成（如实）

### 14.1 `uFrmMain.pas`（4216 LF）**只完成了纯逻辑切片**

本轮把"不依赖缺失接缝"的部分做完了（`uFrmMainLogic.cs`，52 例）：
`GetSizeString:426-439`、运行时长格式化 `:2188-2207`、`UnixDateToDateTime/ExtractSelfTimeDateStamp:2942-2965`、
`RecallPreAllocatedSize:3837-3877`、`lblRecommendPreAllocatedCountClick:3885-3899`、
进程搜索判定 `:3419-3466`、在线搜索判定 `:3612-3766`、`pmProcessListPopup:3777-3794`。

**其余 ~90 个例程未移植**，核心障碍是**三类基础类型在 C# 侧完全不存在**（`GXX.GatewayKit` 是另一套简化设计，不能直接顶替）：

| 缺失的 Delphi 类型 | 声明处 | uFrmMain 里的用法 |
|---|---|---|
| `TMirClientContext` / `TIocpClientContextPool` | `MirClientContext.pas`（其它车道）/ `IocpTcpServer.pas` | `Contexts[ID]`、`FSelectContext`（所有右键动作的载体）、`RefreshContextProcessList/StatusText` 的**反向回调** |
| `TRunGateManager` / `TRunGate` | `RunGateUtils.pas:64/198` | `Create(Handle)`/`Add(port,idx)`/`StartRunGates`/`StopRunGates`/`WorkerThreadCount`/`Send|RecvBlockCount`/`GetOnlineUser` |
| `TIODataPool` | `IODataPool.pas:11` | 内存统计 5 个计数（上一轮 §2.3 判为"不移植"）。**另外**：DFM 有 **222 个对象 / 221 个已发布字段**（本轮已统计），主窗体 UI 本身也没做 |

→ 建议按**子代理给出的切片计划**做 S1→S8（见 §15），S8（`RunGateServicesLifecycle`）必须最后做，
且要先裁定 `CLIENT_ANTIPLUG` 构建目标（本轮已钉死为 **1**）。

### 14.2 其它未覆盖

| 项 | 说明 |
|---|---|
| `CheckInWhiteList`（`GateShare.pas:1506-1535`） | **死代码**（`VERSION_TYPE = 2`），有意不移植 |
| `GateShare.pas` 的 `g_Config.LoadConfig` **读取侧** | 属主窗体车道（见 §15 S1） |
| `GateShare.pas:535-623` 的插件函数指针表 `g_rgp*` | 上一轮已在 `RunGatePluginInterface.cs` 覆盖，本轮只补了全局量 |
| `uFrmMain.pas` 的 13 个 `_*` stdcall 插件回调（`:854-1073`，全在 `CLIENT_ANTIPLUG` 内） | 未移植（需 `IocpClientContextPool`） |
| `uFrmMain.pas` 的 `tmrRefreshLogTimer:3012-3130` / `WriteLog:3588-3611` | 未移植（需求 `ILogSink` 接缝） |

## 15. 给 uFrmMain.pas 后续车道的切片计划（子代理产出，已按物理 LF 行号复核）

> 行号一律为**物理 LF 行号**；`Get-Content`/`Select-String` 对该文件会漂移到 **+17**，不要混用。
> 103 个例程（88 个 `TFrmMain` 方法 + 15 个独立函数）；按可移植性分为 **a=15 纯逻辑 / b=58 壳层 / c=30 纯 UI**。

| # | C# 文件 | Delphi 行号 | 规模 | 接缝需求 |
|---|---|---|---|---|
| S1 | `RunGateConfigLoader.cs` | `1076-1507` + `2766-2806` + `2874-2929` | ~440 | `IIniFile`（`IniFilesEx.cs` 已有） |
| S2 | `RunGateConstsAndGlobals.cs` | §5 缺口（identity/port、`MAX_*`、验证码、退出延时、8 个 tick stamp、20 个名单对象、插件句柄） | ~250 | 无 |
| S3 | `RunGateDiagnostics.cs` | `426-439`、`3837-3877`、`3885-3899`、`2188-2207`、`2942-2965` | ~180 | `IProcessMemoryInfo`（`2214-2219`）—— **其中前三段本轮已完成** |
| S4 | `RunGateLogPipeline.cs` | `3012-3130`、`3588-3611`、`1029-1041`、`3131-3158` | ~170 | `ILogSink`/`ILogFileWriter` |
| S5 | `RunGateOnlineQuery.cs` | `3612-3687`、`3688-3767`、`3413-3438`、`3439-3467`、`3777-3795` | ~230 | 无 —— **本轮已完成** |
| S6 | `RunGateUserActions.cs` | `3168-3267`、`3268-3295`、`4111-4124`、`3796-3826`、`3159-3167` | ~260 | `IBlockListAdmin`（可直连本轮产物）、`IClipboard`、`IMessageBox`、`IContextHandle` |
| S7 | `RunGateAdminAccess.cs` | `3993-4047`、`4125-4180`、`4048-4085`、`4181-4215`、`3468-3536` | ~250 | `MD5Util.RivestStr`（**需先移植 `MD5Util.pas:359`**）、`IInputBox`、`IContextFileRequest` |
| S8 | `RunGateServicesLifecycle.cs` | `440-575`、`576-665`、`666-690`、`691-751`、`752-853`、`1932-2119`、`2986-3011`、`3967-3992`、`2151-2182` | ~400 | `IRunGateManager`/`IRunGate`/`IContextPool`/`IDownloadThreadFactory`/`ITcpListener`/`IGameCenterChannel`/`IWindowsHosting` |

**跨切片公共接缝（≥3 个切片要用，建议先定义）**：`IRunGateUi`（`RefreshContextProcessList`/`RefreshContextStatusText` —— `MirClientContext.pas:2351/2379/2432/2518/2535/2597` 反向调用）、
`IContextPool`、`IRunGateManager`、`ILogSink`、`IClipboard`、`IShellExecute`、`IWindowsHosting`、`IProcessMemoryInfo`、`IDownloadThread`、`IPlugDllLoader`。

**已统计但未使用的关键数据**：`uFrmMain.dfm` = 2030 LF / **222 个对象**，与 `uFrmMain.pas:41-261` 的 **221 个已发布字段**一一对应；
按类：`TLabel 91、TMenuItem 33、TCheckBox 17、TSpinEdit 16、TEdit 13、TButton 12、TGroupBox 7、TTabSheet 5、TBevel 4、TTimer 3、TComboBox 3、TPanel 3、TListView 3、TPopupMenu 2、TMemo 2、TSplitter 2、TServerSocket 1、TMainMenu 1、TStatusBar 1、TPageControl 1、TListBox 1`。
**运行期动态创建（不在 DFM 里）**：`mmMain` 插件菜单 + 6 个子项（`1606-1640`/`1730-1764`）、Gxx 插件菜单（`2443-2457`）、`FTimerSendADText`（`2436`，`VERSION_TYPE = 1`）。

## 16. 本轮发现的两处**跨车道/跨分区**待协调事项

1. **`GXX.Core.Crypto.UnitDes` 需要 `byte[] key` 重载**：`UnitDes.GetKeyData`/`Hash` 用 `EncodingInit.GBK.GetBytes(key)` 把 key 字符串编码成字节，
   而 `GateShare.pas:2491` 的 `sKey` 是 `Move(Key[0], sKey[1], 8)` 得到的 **8 个原始字节** → 现有 `string key` API 无法无损表达。
   本车道的 `GateShareAntiPlug` 用 Latin-1 字符串近似并已登记，**生产接线前必须修**（`GXX.Core` 不属本车道分区）。
2. **`GXX.Core.Rtl.DelphiRTL.Pos("")` 返回 0，而 Delphi 的 `Pos('', S)` 返回 1**（前任 §5.25-1 已登记，本轮在 `uFrmMain.pas:3630-3633` **第二次**踩到）。
   建议在 `GXX.Core` 侧改名或修正，否则每个新区道都要各自补偿一次。
## 17. 接缝接线：`GateShareProcessBlacklistSink`（本轮补的最后一块）

上一轮报告 §6.1 登记过"`GateShare.pas` 的落盘类函数以 `ISafeFilterHost`/`IProcessBlacklistSink` 注入，
默认内存实现只**计数**、未落盘"。本轮提供了 `GateShareSeamAdapters.cs` 的
**`GateShareProcessBlacklistSink : IProcessBlacklistSink`**，把
`uFrmProcessBlacklist.pas:155-156 / :189-190` 的两个触发器接到真实实现：

```csharp
ProcessBlacklistUnit.Sink = new GateShareProcessBlacklistSink();   // 启动时赋值一次
// 之后 SaveProcessBlacklist → GateShareLists.SaveProcessBlacklist（真写 ExeDir\ProcessBlacklist.txt）
//      RebuildProcessBlacklist → GateShareLists.RebuildProcessBlacklist（真算 zLib + MD5；RSA 走接缝）
```

**`ISafeFilterHost` 有意未接线**（原因见 §13 表格）：接口把 `TempIPList`/`BlockIPList` 声明为
`TSafeHashStringList`（字符串表，窗体直接读写并遍历），而原文这两者是 `TAddressList`
（只存 `nIPaddr` 数值、不存字符串）→ 真实实现需要一层"字符串视图 ↔ `TAddressList`"双向同步，
会改动 12 个窗体测试类。**登记为后续事项**，本轮不动。
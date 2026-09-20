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

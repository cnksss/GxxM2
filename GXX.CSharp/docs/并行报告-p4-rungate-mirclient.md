# 并行报告 · 车道 `p4-rungate-mirclient`（GXX Delphi7→C# · `MirClientContext.pas`）

> 分支：`par/p4-rungate-mirclient` ｜ 工作树：`.worktrees/p4-rungate-mirclient`
> 目标单元：`Source/RunGate/MirClientContext.pas`（GBK，**实测 11,125 LF / 11,126 物理行**）
> 规程：`docs/转换开发文档.md`、`docs/并行派发台账.md` §9.4/§10/§11/§12、`docs/并行报告-p2-rungate-impl.md`
> 状态：**build 0 error / RunGate.Tests 1299 例 0 失败**；`CheckUsePlugin` 分派体未覆盖（见 §5）

---

## 1. 全部 commit hash

| # | hash | 内容 | 门禁 |
|---|---|---|---|
| 1 | `865932d1` | 接缝层 + 骨架/生命周期/消息队列/发送族（**该提交编译不过**，存档性质） | ❌ 被集成门禁挡下 |
| 2 | `760c85f3` | `Run` / `DoCheckRecvBuffer` / 消息族 1:1 移植 + 93 例测试（**修复 #1 的编译错误**） | ✅ build 0 error，1299/1299 |

**HEAD = `760c85f3`**。集成时请以 `760c85f3` 为准（`865932d1` 之后没有任何"已知坏状态"残留）。

---

## 2. 逐方法族判定表（含行号）

口径：**已完成** = 1:1 逐字移植；**接缝** = 保留签名 + 空实现，注释 `// 接缝：待 … 移植后接入`；
**未覆盖** = 未移植（含有意按 §2.3 不移植）。

| # | 方法 / 族 | 原文行号 | 判定 | 落点 |
|---|---|---|---|---|
| 1 | `uses` / `const` / `type` / `TClientMsg` / `TProcessMsg` / `TRecordActionInfo` / 类字段与属性 | 1-301 | **已完成** | `MirClientContext.cs` |
| 2 | `UpdateLockUserList` | 308-351 | **已完成** | `MirClientContext.cs`（`MirClientContextUnit`） |
| 3 | `GetUserLockTime` | 353-387 | **已完成** | 同上 |
| 4 | `SubStringOccurences` | 389-412 | **已完成**（+ `…Bounded` 供测试观察死循环） | 同上 |
| 5 | `Create` | 416-443 | **已完成** | `MirClientContext.cs` |
| 6 | `Destroy` | 445-470 | **已完成**（非托管释放 → GC，逐条注释原行号） | 同上 |
| 7 | `DoReset` | 473-679 | **已完成** | 同上 |
| 8 | **`Run`** | **681-1886** | **已完成** | `MirClientContext.Run.cs` |
| 9 | `GetExVersionNO` | 1888-1901 | **已完成** | `MirClientContext.cs` |
| 10 | `CheckRecvPacketSize` | 1904-1918 | **已完成** | 同上 |
| 11 | **`DoCheckRecvBuffer`** | **1920-2945** | **已完成**（含 3 处被注释段落的占位注释） | `MirClientContext.Messages.cs` |
| 12 | `ProcessClientMessage` | 2947-3020 | **已完成** | `MirClientContext.cs` |
| 13 | `DelayClientMessage` | 3022-3054 | **已完成** | 同上 |
| 14 | `ClearClientMsgList` | 3056-3078 | **已完成** | 同上 |
| 15 | `GetClientMessage` | 3080-3120 | **已完成** | 同上 |
| 16 | `SendMessaggeToClient` | 3123-3143 | **已完成**（原文三 g 拼写保留） | 同上 |
| 17 | `SendMessageToServer` ×3 | 3146-3228 | **已完成** | 同上 |
| 18 | `SendActionRet` | 3230-3238 | **已完成** | 同上 |
| 19 | `LockUser` / `UnLockUser` | 3241-3279 | **已完成** | 同上 |
| 20 | `FilterSayMsg` | 3281-3408 | **已完成** | 同上 |
| 21 | `GetSpeedText` | 3410-3416 | **已完成** | 同上 |
| 22 | `ContinuousSpeed` | 3418-3455 | **已完成** | 同上 |
| 23 | `ProcessAssasinate` | 3457-3463 | **已完成** | 同上 |
| 24 | **`CheckUsePlugin`** | **3465-9688** | **未覆盖**（仅前导段 3465-3520 + 兜底） | `MirClientContext.CheckUsePlugin.cs` |
| 25 | `GetConcurrentPacketCount` | 9689-9708 | **已完成** | `MirClientContext.cs` |
| 26 | `ClearConcurrentPacket` | 9710-9735 | **已完成** | 同上 |
| 27 | `SendWarnMsg` | 9737-9745 | **已完成** | 同上 |
| 28 | `GetRunGate` | 9747-9753 | **已完成** | 同上 |
| 29 | `AddServerMsg` | 9755-9800 | **已完成** | 同上 |
| 30 | `AddServerText` | 9802-9844 | **已完成** | 同上 |
| 31 | `CloseContextSocket` | 9846-9881 | **已完成** | 同上 |
| 32 | `DoConnect` | 9883-9965 | **已完成** | 同上 |
| 33 | `Dissconnect`（反调试暗桩） | 9967-10021 | **未覆盖（§2.3 不移植）** | — |
| 34 | `DoDisconnect` | 10023-10249 | **已完成** | `MirClientContext.cs` |
| 35 | `ProcesssSendToClientSendMyMagic` | 10251-10303 | **接缝** | `MirClientContext.Messages.cs` |
| 36 | `ProcesssSendToClientSendAddMagic` | 10305-10340 | **接缝** | 同上 |
| 37 | `ProcesssSendToClientBagItems` | 10342-10407 | **接缝** | 同上 |
| 38 | `ProcesssSendToClientAddItem` | 10409-10451 | **接缝** | 同上 |
| 39 | `ProcesssSendToClientDelItem` | 10453-10481 | **已完成** | 同上 |
| 40 | `ProcesssSendToClientDelItems` | 10483-10520 | **已完成** | 同上 |
| 41 | `ProcesssSendToClientDropItem` | 10522-10543 | **已完成** | 同上 |
| 42 | `ProcesssSendToClientEatItemOK` | 10545-10566 | **已完成** | 同上 |
| 43 | `ProcesssSendToClientMasterBagToHeroBagOK` | 10568-10606 | **已完成** | 同上 |
| 44 | `ProcesssSendToClientHeroBagToMasterBagOK` | 10608-10646 | **已完成** | 同上 |
| 45 | `DoLogClientPacket` | 10648-10797 | **已完成** | 同上 |
| 46 | `GenerateVerifyCode` | 10799-10890 | **已完成** | 同上 |
| 47 | `DelayClose` | 10892-10896 | **已完成** | `MirClientContext.cs` |
| 48 | `SendAntiPlugStreamInfo` | 10899-10948 | **已完成** | `MirClientContext.Messages.cs` |
| 49 | `SendAntiPlugStreamUnload` | 10950-10989 | **已完成** | 同上 |
| 50 | `SendAntiPlugStreamLoadCache` | 10991-11021 | **已完成** | 同上 |
| 51 | `SendAntiPlugStream` | 11024-11064 | **已完成** | 同上 |
| 52 | `LogPluginData` | 11069-11120 | **已完成** | 同上 |

### 2.1 原文**整段被注释**、本车道按"不可达"处理的区段

| 原文行号 | 内容 | 处置 |
|---|---|---|
| 905-912 | `GetNoSendCacheSize` 堆积检测（`(* *)`） | 保留占位注释 |
| 9918-9925 | 测试插件限定 10 人 | 保留占位注释 |
| 2143-2173 | VMProtect + 登录器 MD5 授权（`//`） | 保留占位注释 |
| 2254-2272 | `g_rgpStartContext` 回调（`(* *)`，另一处 2911 是活的，已实现） | 保留占位注释 |
| 2662-2740 | `CM_RUNGATEDOOR` 的 RSA1024 解密 + `RtlAdjustPrivilege`/`NtSetInformationProcess` 提权 + `ExitProcess` | **§2.3 不移植**；仅保留 2741 的 `Exit`（该行在 `{$IF}` 之外，是活代码） |
| 2756-2788 | `g_DisableChrLoginList` / `CM_SENDCHECKPLUGIN`（`(* *)`） | 保留占位注释 |
| 10102-10248 | `g_boAntiUseException` + `g_rgpEndContext` 插件回调（`(* *)`） | 保留占位注释 |
| 2054-2058 | `REGISTER_TEST = 1` 暗桩日志（`{$IF}` 死分支） | 保留占位注释 |
| 1340 / 1493 / 10598 / 10638 | 单行赋值/调用被注释 | 原文如此，逐行标 `// 原文如此（…:行）` |

---

## 3. 新增文件 + 每个方法的已覆盖/未覆盖行号范围

| 文件 | 行数 | 对应原文行号范围 |
|---|---|---|
| `src/GXX.RunGate/MirClientContext.cs` | 1,723 | 1-301（声明/字段/记录）、308-412（单元级三函数）、416-470、473-679、1888-1918、2947-3120、3123-3463、9689-9965、10023-10249、10892-10896 |
| `src/GXX.RunGate/MirClientContext.Run.cs` | 1,322 | 681-1886（`Run` 全文） |
| `src/GXX.RunGate/MirClientContext.Messages.cs` | 1,789 | 1920-2945（`DoCheckRecvBuffer` 全文）、10453-10797、10799-10890、10899-11064、11069-11120；**10251-10451 为接缝空实现** |
| `src/GXX.RunGate/MirClientContext.CheckUsePlugin.cs` | 122 | 3465-3520（忠实前导段 + 兜底）；**3521-9681 未覆盖** |
| `src/GXX.RunGate/MirClientContextSeams.cs` | 919 | 接缝层（见 §6） |
| `tests/GXX.RunGate.Tests/MirClientContextTests.cs` | 1,230 | 93 例测试 |

### 3.1 阅读覆盖率（按物理 LF）

| 口径 | 行数 | 占比 |
|---|---|---|
| 原文总 LF | 11,125 | 100% |
| **已 1:1 覆盖** | **≈ 5,442** | **≈ 48.9%** |
| 接缝空实现（4 个方法） | 201 | 1.8% |
| **未覆盖 `CheckUsePlugin` 分派体** | **6,161** | **55.4%** |
| 有意不移植（`Dissconnect` 9967-10021 + `CM_RUNGATEDOOR` 2662-2740） | 134 | 1.2% |
| 原文整段被注释（§2.1） | 231 | 2.1% |
| 其余（条件编译死分支、`SE_*` 常量声明等） | ≈ 0 | — |

> 覆盖 5,442 + 未覆盖 6,161 + 接缝 201 + 不移植 134 + 注释段 231 ≈ 12,169 > 11,125，因为 §2.1 的注释段与接缝/未覆盖在两侧都计入了；净口径以"未覆盖 6,161 行（55%）+ 有意不移植 134 行"为准。

---

## 4. 测试用例数 + build/test 结果

```
cd .worktrees\p4-rungate-mirclient\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test  tests\GXX.RunGate.Tests\GXX.RunGate.Tests.csproj -c Debug --nologo
```

| 项 | 结果 |
|---|---|
| `dotnet build GXX.slnx -c Debug` | **0 Error(s)**（112 warning，均为既有 xUnit 分析器告警） |
| `dotnet test GXX.RunGate.Tests` | **Failed: 0 / Passed: 1299 / Skipped: 0 / Total: 1299** |
| 本车道新增用例 | **93** |
| 基线 1206 例 | **全部保留通过**（0 新增失败） |

测试类（全部挂 `[Collection("RunGateFormLane")]`，与窗体族共享静态全局量，必须串行）：

| 测试类 | 用例数 | 覆盖 |
|---|---|---|
| `MirClientContextUnitTests` | 17 | `SubStringOccurences`（含"空子串在 Delphi 死循环、托管侧不死"的差异断言）、`UpdateLockUserList`/`GetUserLockTime`（保存开关、余秒、夹到 `nLockTime`、0/负值） |
| `MirClientContextVersionTests` | 6 | `GetExVersionNO` 的 0/负/阈值/多步/退一步/`int.MaxValue`（**溢出分支不可达**） |
| `MirClientContextCoreTests` | 70 | Create/DoReset、`CheckRecvPacketSize` 4 种 `g_BlockMethod`、消息队列 7 例、并发包计数（含恒返回 0 差异）、发送族 8 例、`FilterSayMsg` 全部 5 种模式 + 边界、`ContinuousSpeed`/`ProcessAssasinate`、`AddServerMsg/AddServerText` 超限、`DoConnect`/`DoDisconnect`/`CloseContextSocket`、包堆积拦截 |

**测试替身**（测试文件内定义，不改 csproj、不加 AssemblyInfo）：
`CapturingTransport : IIocpTransportSeam`（捕获 `PostSendText`/`PostSendBuffer`/`Close`）、
`FakeTcpClient : ITcpClientSeam`（捕获 `SendServerMsg` 的 6 个形参）、`FakeFrmMain : IFrmMainSeam`。

---

## 5. 发现的原文缺陷 / 易错点（带 `文件:行`）

> 全部**照抄不改**；每条都有对应测试或注释。**未做任何"顺手修正"。**

### 5.1 `ClearConcurrentPacket` 恒返回 0（`MirClientContext.pas:9710-9735`）

`:9715 Result := 0;` 之后**从未对 `Result` 赋值**（与 `GetConcurrentPacketCount` 的 `:9702 Inc(Result)` 不对称）。
测试 `ClearConcurrentPacket_AlwaysReturnsZero_UnlikeGetConcurrentPacketCount` 在同一状态下断言
`Get == 4` 而 `Clear == 0`。
附带两处同族不一致：`:9721` 对 `ClientMsg` **没有 nil 检查**（`:9700` 有）；`:9725` 释放长度用
`nBufferLen + 1`，而 `:2970` 分配的是 `nBufferLen`（**多算 1 字节**）。

### 5.2 `ProcessClientMessage` 的堆积检测在"延时窗口内"整体失效（`:2957` / `:3006`）

`ClientPacketCount` 只在 `MyGetTickCount > FDelayTick + 50`（`:2957`）成立时才被计算；
否则它保持 `0`，于是 `:3006` 的 `ClientPacketCount >= g_nMaxClientPacketCount` 恒假 —— 堆积检测被跳过而不是报错。

### 5.3 `nPacketIndex := IntValue` 读到未初始化值（`:2025`）

`IntValue` 的唯一赋值点 `:2013` 位于被注释的 `{ }` 块内 → 该处是**未定义行为**（读到栈残留）。
托管侧确定性置 0（测试无断言，仅注释 `// 原文缺陷 M1`）。

### 5.4 `GetExVersionNO` 的"退一步"语义与"溢出不可达"（`:1894` / `:1897`）

- `:1894 while (nVersionDate > 100000000)` 是**严格大于** → `300000000` 只走 2 步，
  `Result = 200000000`、余数停在 `100000000`（不是 0）。测试 `…_ExactMultiple_StopsOneStepEarly` 钉死。
- `:1897 Inc(Result, 100000000)` 看似会整数溢出，实际**不可能**：循环条件保证
  `Result <= 2.1e9 < int.MaxValue`。测试 `…_MaxInt_CannotOverflow` 断言 `int.MaxValue` 得 `2100000000`（正数）。

### 5.5 `CheckRecvPacketSize` 的 `case` 无 `else`（`:1910-1913`）

`g_BlockMethod = bmDisconnect` 时，`bmTempBlock`/`bmBlockList` 两个分支都不执行 →
**既不拉黑也不临时拉黑**，但仍然 `DelayClose(100)`。
测试 `CheckRecvPacketSize_DisconnectMethod_MatchesNoCaseSoNoBlockCall` 固化。

### 5.6 三处"开关技能列表"互不相同（`:1067` / `:1513` / `:1605`）

| 位置 | 列表 |
|---|---|
| `:1067`（延时关闭时禁技能） | 7, 12, 25, **26**, 40, **42, 43, 56, 66** |
| `:1513`（技能 CD 不关心开关时间） | 7, 12, 25, 40 |
| `:1605`（开关技能不记录技能时间） | 7, 12, 25, 26, 40, 42, 43, 56, 66 |

**三处列表不能统一**，已逐处照抄。

### 5.7 `TimeInterval <= MagicCDTime - 60` 在 `MagicCDTime < 60` 时下溢（`:1530`）

按 `LongWord` 无符号下溢 → 巨大值 → 该分支**恒成立**（`MagicCDTime = 1` 也判超速）。
注释标记 `原文缺陷（R4）`。

### 5.8 `MakeWord(g_btMagicCDFColor, g_btMagicCDBColor)` 的参数顺序是 (F, B)（`:1575`）

与同族 `SendMessaggeToClient(..., btFColor, btBColor)`（`:3132` 也是 `MakeWord(F, B)`）一致，
但与"颜色常量通常 BGRA"的直觉相反 —— 照抄。

### 5.9 `FilterSayMsg` 的发言长度是**字节口径**（`:3333-3334`）

判据 `Length(sMsg) > g_dwSayMaxLen` 是 AnsiString **字节数**，截断 `Copy(sMsg, 1, g_dwSayMaxLen)` 也是**字节**。
托管侧若用 `string.Substring` 会变成"字符数"口径（GBK 下差 2 倍）。
本车道为此新增 `AnsiStrSeam.AnsiCopyPrefix`（按 GBK 字节取前缀）并加注释说明截断点落在双字节字符中间时
会产生替换字符（与 Delphi 截断出半个汉字等价）。测试 `FilterSayMsg_MaxLenTruncatesByAnsiBytes`。

### 5.10 `AddServerText` 的异常日志文案是 `AddServerMsg`（`:9841`）

从 `AddServerMsg`（`:9797`）复制粘贴遗留；照抄并注释。

### 5.11 小退/大退延时提示的连发次数不同（`:2624-2631` vs `:2648-2655`）

小退提示连发 **3** 次 + 1 次进度提示；大退提示连发 **4** 次 + 1 次。照抄。

### 5.12 `RequestClientFileRootPath` 为空时前缀校验恒真（`:2489`）

`Copy(FClientResponseFileName, 1, Length(RequestClientFileRootPath))` 在根路径为空串时返回 `''`，
而 `SameText('', '')` 为 True → **任意文件名都通过前缀校验**。照抄，注释标记 `原文缺陷 M5`。

### 5.13 `ClientList.SaveToFile(Format('.\log\%s-%s.txt', …))`（`:3014`）

包堆积日志直接写相对路径 `.\log\`，**不创建目录** → 目录不存在时静默失败（原文无 try）。
托管侧未落盘（保留注释与计数语义）。

### 5.14 跨库易错点（复用 p2 报告 §5.25-1，本车道实测确认）

`GXX.Core.Rtl.DelphiRTL.Pos("")` **返回 0**，而 Delphi 的 `Pos('', S)` 返回 **1**。
后果：`SubStringOccurences`（`:410`）在子串为空串时，Delphi 会**死循环**，托管侧**不会**
（`PosEx` 直接返回 0）。这是"缺陷被无意修好"，本车道用
`SubStringOccurences_EmptySubString_DoesNotReproduceTheDelphiInfiniteLoop` 把这个**行为差异**钉死，
防止后人误以为这里需要死循环保护。

### 5.15 `Run` 的解锁/丢弃边界不对称（`:916` vs `:997`）

`:916 if boLocked and (MyGetTickCount >= dwUnLockTick) then UnLockUser;`（**>=**）
`:997 if boLocked and (MyGetTickCount <= dwUnLockTick) then` 丢弃（**<=**）
同一毫秒两者同时成立 —— 丢弃优先，`UnLockUser` 的结果要到下一轮才生效。照抄，注释标记 `原文缺陷 R1`。

### 5.16 `if LockTime >= 0` 恒真（`:1166`）

`GetUserLockTime`（`:357 Result := 0`）恒返回 `>= 0`，故该条件永真；
实际无害是因为 `LockUser(0)` 在 `:3246` 直接 `Exit`。注释标记 `原文缺陷 R2`。

### 5.17 `TObject(nTime)` 把秒数当指针存（`:336` / `:346`）

Delphi 把 `Integer` 强转成 `TObject` 存进 `TStringList.Objects`；`nTime = 0` 时 `TObject(0)` 就是 `nil`。
托管侧存 boxed `int`（0 仍是 0 而不是 null）。`GetUserLockTime` 的 `Integer(Objects[Index]) > 0` 判据结果一致，
但"nil vs boxed-0"的可观测差异已在 `MirClientContextUnit` 的文档注释里登记（偏差 D-补充）。

### 5.18 `Dissconnect` 的 PE 头校验（`:10011`）

`:10010 if CRC2 <> l_nt_header.FileHeader.NumberOfSymbols` —— 拿自算 CRC 与 PE 可选头里的
"符号数"字段比较，**两者语义无关**，是一个恒真/恒假的反调试暗桩。按 §2.3 不移植。

### 5.19 `doConnect` 的 `RunGate.OnlineUser.Unlock`（`:9915`）

Delphi 大小写不敏感所以能编译；托管侧是 `UnLock`。已按实现在 `:9915` 注释说明。

### 5.20 `DoDisconnect` 的 `nSessionID` 当 `wSocketIndex` 用（`:10091`/`:10093`/`:10095`）

`SendServerMsg`（`RunGateUtils.pas:60`）的形参是 `(nIdent: Integer; wSocketIndex: Word; nSocket, nUserListIndex: Integer; …)`，
而 `:10091` 传的是 `nSessionID`（SessionID 当 socket 索引）—— 与 `:9928` 传 `ContextID` 的约定**不同**。
照抄，注释保留。

---

## 6. 接缝清单 + 需要调度方改白名单外文件的精确签名要求

### 6.1 本车道建立的接缝（全在 `src/GXX.RunGate/MirClientContextSeams.cs`，属本车道独占区）

| # | 源单元 | 接缝类型 / 成员 | 为什么需要 |
|---|---|---|---|
| S1 | `Common/IocpUtils.pas:12` | `enum TCloseFrom { cfOther, cfPostWSASendCache1, cfPostWSASendCache2, cfProcessIOQueued }` | main 上没有；`CloseContextSocket` 的形参 |
| S2 | `Grobal2_Ex.pas:262`（= `GateShare.pas:258`） | `enum TBaseAction { baOther..baCutMeat }` | main 上没有；`RecordActionArr[].Action` 用 |
| S3 | `Common/IocpCommon.pas:73` | `class TIocpCriticalSection`（`Lock/UnLock/TryLock`） | `USE_SPINLOCK` 未定义 → 无字符串形参（偏差 D1） |
| S4 | `Common/IocpCommon.pas:56` | `class TSafeList`（`Count/this[]/Add/Insert/Delete/Clear/IndexOf/Remove/Lock/UnLock`） | 存 `PClientMsg` → 托管侧 `object` |
| S5 | `GateShare.pas:133` | `class TSafeStringList`（含 `Text`、`Strings`） | `ProcessList` |
| S6 | `GateShare.pas:117` | `class TSafeHashStringListEx : TSafeStringList`（`Objects[]/AddObject`） | `uFrmGameSpeedLogic.cs:834` 的 `TSafeHashStringList` **没有** `Objects`，且该文件只读 → 加 `Ex` 后缀派生（偏差 D2） |
| S7 | `GateShare.pas:149` | `class TSafeMemoryStream : MemoryStream`（`Size`/`Clear`/`Lock`/`UnLock`） | 截图流 / 客户端文件流 |
| S8 | `GateShare.pas:308` | `class TGameSpeed`（含 `Clear()`） | 原文 record + `FillChar(…,0)` → 托管侧引用类型 + `Clear()`（偏差 D5） |
| S9 | `GateShare.pas:82/93` | `class TAddressInfo` / `TAddressListEx` | `g_CurrIPList` |
| S10 | `Common/IocpTcpServer.pas:33` | `abstract class TIocpClientContext`（`ContextID/RemoteAddr/Socket/IocpCore/IsPostedCloseQuest/IsWaitingGiveBack/LastRecvDataTick/PostSendText/PostSendBuffer/Close/CloseContextSocket/DoConnect/DoDisconnect/DoReset/DoCheckRecvBuffer`） | socket/线程本体按 §2.3 不移植 |
| S11 | `Common/IocpTcpServer.pas:119` | `class TIocpTcpServer { object BindObject }` | `GetRunGate`（`:9749-9752`）用 |
| S12 | `RunGateUtils.pas:60/151` | `interface ITcpClientSeam { bool Active; void SendServerMsg(int nIdent, ushort wSocketIndex, int nSocket, int nUserListIndex, byte[] buffer, int bufferLen); }` + `class TRunGate { TcpClient; OnlineUser; nMaxOnlineUserCount; dwCurDefenseLevel; nTotalAttackCount; dwClearTempTick; dwResotreDefenseTick; SendServerMsg(...) }` | `TRunGate` 的 socket 壳未 1:1（p2 报告 §6.1） |
| S13 | `GateShare.pas` 全局量 | `g_LockUserList`、`g_VerifyFailUserList`、`g_CurrIPList`、`g_LoginMACPlayerList`、`g_VerifyCodeMapList`、`g_LoadNoVerifyChrList`、`g_dwClientAccumulateMaxSize`、`g_nClientCloseDelay`、`g_nClientLogoutDelay`、`g_boOpenVerifyCode`、`g_nVerifyCodeErrCount`、`g_nVerifyCodeRefreshCount`、`g_nVerifyCodeWaitTime`、`g_dwVerifyCodeInterval1/2`、`g_dwVerifySuccessAddInterval`、`g_boVerifyFailTriggerScript`、`g_boVerifyFailLoginVerify`、`g_boVerifyCodeExcludeMap`、`g_boLogoutNoResendAntiplugStream`、`g_boOneMACLimitePlayer`、`g_nOneMACLimitePlayerCount`、`g_boDelayCloseDisableMove/Spell/Attack/UseItem`、`g_boBreakClientCloseHint`、`g_sBreakClientCloseHint`、`g_boBreakClientLogoutHint`、`g_sBreakClientLogoutHint`、`g_boCheckClientPassword`、`g_sClientPassWord`、`g_sLogClientPacketDir`、`g_sReplaceWord`、`g_sDisableSayMsg`、`g_sDisableSayMsgBegin`、`g_ScreenshotPath`、`g_ClientAntiPlugVersion`、`g_ClientAntiPlugDllStringCRC`、`g_ClientAntiPlugDllSize`、`g_ClientAntiPlugDllString`、`g_ClientAntiPlugDllBlockSize`、`g_ClientAntiPlugDllBlockCount`、`g_boAntiplugAllLog`、`g_RunGatePlugDllHandle`、`g_rgpRecvPacket`、`g_rgpStartContext`、`g_rgpEndContext`、`g_CSRunGatePlug` | 这些在 `uFrmGameSpeedLogic.cs` 的 `FormGlobals` 里**没有**，而该文件只读 → 落在接缝层 |
| S14 | `IocpCommon.pas:112` + SysUtils | `MyGetTickCount()`（可注入 `MyGetTickCountProvider`）、`Randomize()`、`Random(n)`（可注入 `RandomSink`） | `timeGetTime` 与 `GetTickCount` **不是同一个 API**（精度 1ms vs 15.6ms），故独立提供 |
| S15 | `GateShare.pas` 回调 | `AddMainLogMsg(msg, nLevel)`、`AddBlockIP(ip)`、`AddTempBlockIP(ip)`、`IsBlockMac(mac)`、`CloseAllUser()`、`AppendTextLine(file, line)`（可注入 sink） | GateShare/uFrmMain 未覆盖 |
| S16 | `MD5Util.pas:29/346` | `MD5Match(byte[] d1, byte[] d2)` | main 上没有 |
| S17 | `uFrmMain.pas` | `interface IFrmMainSeam { RefreshContextProcessList(TMirClientContext, bool); RefreshContextStatusText(string) }` + `FrmMainSeam.FrmMain` | `DoCheckRecvBuffer` 的 4 个调用点 |
| S18 | `StrUtils/SysUtils` | `AnsiContainsText`、`AnsiReplaceText`、`StringReplace`、`StringOfChar`、`SameText`、`FormatDateTime`、**`AnsiCopyPrefix`（按字节截断）** | 原文 `uses StrUtils` |
| S19 | `ZlibEx.pas` | `zLibDecodeString(string)`、`zLibDecompressBuffer`、`zLibCompressBuffer`、`ZDecompressStream(byte[])` | 薄适配 `EDcode` 真实现 |
| S20 | `SysUtils`/IO | `HostExeDirectory`、`ParamStr0`、`FileExists`、`DirectoryExists`、`ForceDirectories`、`ExtractFilePath`、`sLineBreak` | 原文文件 IO |

> **注意**：S19/S20/S16/S18 是包 **main 上已有真实现**（`EDcode`）的**表示法适配**（AnsiString ↔ `byte[]`/`string`），
> 不是"未移植接缝"。按台账 §12.8 的要求我已删除 `MagicIntervalUtilsSeam`，
> 并把 `MagicIntervalUtils.pas` 的调用点改为**直接调真实现**：
> `MagicUseTickList.Find(unchecked((ushort)MagicID))` / `g_MagicCDList.Find(unchecked((ushort)MagicID))`。
> 用 `unchecked((ushort)…)` 是为了对 `Find(int)`（本工作树 `uFrmGameSpeedLogic.cs:963`）与
> `Find(ushort)`（integration 上 `GateShareMagicIntervalUtils.cs`）**两版都编译且语义一致**
> （原文 `function Find(MagicID: Word)` 本就是 Word）。

### 6.2 需要调度方改**白名单外文件**的精确签名要求

| # | 目标文件 | 需要的精确签名 | 用途 | 优先级 |
|---|---|---|---|---|
| R1 | `src/GXX.RunGate/uFrmGameSpeedLogic.cs`（**只读**） | 把 `public class TSafeHashStringList` 增加 `public object[] Objects { get; }` + `public int AddObject(string s, object o)` + `public object GetObject(int i)` + `public void SetObject(int i, object v)` | 让 `g_*` 全局量能用原文类型名 `TSafeHashStringList` 而不是我的 `TSafeHashStringListEx`（消除偏差 D2） | 低（现方案可用） |
| R2 | `src/GXX.RunGate/uFrmGameSpeedLogic.cs`（**只读**） | 把 `FormGlobals` 里的 `g_dwClientAccumulateMaxSize` / `g_nClientCloseDelay` / `g_nClientLogoutDelay` / `g_LockUserList` 等 §6.1-S13 的全局量**搬进 `FormGlobals`** | 与原文声明位置（`GateShare.pas`）对齐，消掉本车道接缝层里最大的一块 | 中 |
| R3 | `src/GXX.RunGate/GateShareMagicIntervalUtils.cs`（**integration 上新增**） | 确认 `TMagicIntervalList.Find` 的最终形参是 `ushort`；若是 `int` 请告知，我改回无 cast 形式 | 我当前用 `unchecked((ushort))` 对两版都安全，但需要一次权威确认 | **高（阻塞性确认）** |
| R4 | 后续 `Grobal2_Ex.pas` 车道 | `TClientMagic`（含 `TMagic_C` 与 `string[ITEM_NAME_LEN]`）、`TClientItem`（含 `TStdItem`）的 packed 布局 | 让 §2 的 #35-#38 四个接缝方法能 1:1 落地 | 中 |
| R5 | 后续 `uFrmMain.pas` 车道 | 把 `IFrmMainSeam` 的实现接到真实主窗体（`FrmMainSeam.FrmMain = …`） | 让 `DoCheckRecvBuffer` 的进程列表/状态文本真的刷新 | 低 |
| R6 | 后续 `GateShare.pas` 车道 | 把 `GateShareSeam` 的 51 个全局量与 6 个函数回调**改名为不冲突**并接管（或直接把 `GateShareSeam` 整体并入 `GateShare.cs`） | 消掉接缝层 | 低 |

---

## 7. 诚实说明：未完成部分与剩余量

### 7.1 完全未完成（**6,161 行，占原文 55%**）

**`CheckUsePlugin`（3465-9688）的分派体 3521-9681 未移植。**
`MirClientContext.CheckUsePlugin.cs` 只有：
- 原文 `:3465-3472` 的 5 个局部常量（`DEBUG_LEVEL` / `MAX_REPAIR_TIME` / `TIME_INACCURACY` /
  `DELAY_TIME_ADD` / `DROP_CONCURRENT_RATE`）；
- 忠实前导段 `:3505-3519`（`Result := False` … `AntiPlugAction := nil`）；
- 异常兜底 `:9682-9686` 的形状。

**当前行为**：对所有 `Ident` 都返回 `false` = "未检测到外挂" = 放行（与原文"无匹配 `Ident` 时 `Result := False`"一致，
但**未覆盖的 `Ident` 本该被检测/限速/锁定却不会**）。
**这是本车道最大的功能缺口**，必须由后续车道补。文件头已写清接管者需要的：
- 分派键 `DefMsg.Ident`（`NEED_REGISTER = 1` 的活分支走 `if DefMsg.Ident = CM_WALK then …` 链，不是 `case`）；
- 用到的本类状态（`dwCollectIntervalArr` / `nCollectIntervalIndexArr` / `RecordActionArr` / `nRecordActionIndex` /
  `SumSpeedProcessArr` / `nCompensationArr` / `LastLockAntiPlugActionMode` / `FLastAction` / `GameSpeed` /
  `nMoveSpeed` / `nAttackSpeed` / `nSpellSpeed` / `MagicUseTickList`）；
- 用到的全局量（`g_Config.ActionList[*]` / `g_wActionSpeedIntervals` / `g_MagicCDList` / `g_sMagicCDMsgText` /
  `LastEatingItemTick` / `LastHeroEatingItemTick` / `HumBagItems` / `HeroBagItems`）；
- **可直接复用的既有纯判定内核**：`RunGateUtilsClientStat`（多数表决）、`RunGateForwardClassifier`、
  `RunGateTiming.TickDiff`、`uFrmGameSpeedLogic.cs` 的 `TAntiPlugActionMode`/`TAntiPlugAction`/`TAntiPlugConfig`/
  `RunGateConstants.AntiPlugActionModeNames*`/`FormGlobals.g_wActionSpeedIntervals`/`ActionModeUseSpeedIntervals`。

**跨语言易错点（已核实，写入文件头）**：调用点 `:1820` 写的是 `CheckUsePlugin(@ProcessMsg.DefMessage)`，
而形参声明是 `Msg: PProcessMsg` —— **类型不匹配的指针双关**。它能工作是因为 `TProcessMsg` 的前两个字段
恰好是 `DefMessage`（偏移 0，16 字节）与 `dwTimeTick`（偏移 16）。C# 侧 `TProcessMsg` 是引用类型，
**直接传整个 `ProcessMsg` 与原文语义完全等价**，故 `Run` 里写 `CheckUsePlugin(ProcessMsg)`。

### 7.2 部分完成 / 接缝（201 行）

`ProcesssSendToClientSendMyMagic` / `SendAddMagic` / `BagItems` / `AddItem` 四个方法**空实现**，
原因：依赖 `Grobal2_Ex.pas` 的 packed record `TClientMagic`（含 `TMagic_C` 与 `string[ITEM_NAME_LEN]` 短串）
与 `TClientItem`（含 `TStdItem`）。本车道**不定义**这些类型 —— 跨车道类型重名事故本工程已发生 4 次，
且这些类型属 `Grobal2_Ex.pas` 车道的类型面。
四个方法的原文语义已在文件注释里写清（压缩流长度校验 → `g_MagicCDList.Find` 覆写 `dwInterval`/`dwRealInterval`
→ 回写 `AddServerMsg`；背包按 `s.StdMode in [0,2,3]` 填 `MakeIndex/StdMode/Shape/AC1/MAC1`）。

### 7.3 有意不移植（134 行，§2.3）

`Dissconnect`（9967-10021，PE 头反调试）与 `CM_RUNGATEDOOR` 的 RSA/提权分支（2662-2740）。

### 7.4 未做的验证

- **无端到端回归**：`Run` / `DoCheckRecvBuffer` 的 socket 侧走接缝，没有真实客户端↔网关↔M2 的回环测试。
- **`Run` 只做了分支级单测**，没有覆盖全部 30+ 个 `Ident` 分派分支（例如 `CM_SAY` 的
  `GetValidStr3` + `FilterSayMsg` 串联、`CM_QUERYBAGITEMS` 的 `LockUser(GetUserLockTime)` 链路、
  魔法 CD 超速放行的 `MagicCDSpeed[10]` 环形窗口 —— 这几处已在实现里逐行落地，但缺测试）。
- **`DoCheckRecvBuffer` 的进程列表/截图/客户端文件落盘三条路径**只做了代码级落地，未做 IO 级测试
  （`FrmMainSeam` 与文件落地都是可注入接缝，未接线真实实现）。
- **GBK 承载的保真风险**：`sData`/`sDataMsg`（原始二进制 AnsiString）在托管侧用 `string` 承载，
  经 GBK 解码/编码往返。6-bit 编码体是 ASCII 安全的，但 `sDataMsg` 的附加负载含任意字节时
  GBK 往返**可能不无损**（非法序列 → 替换字符）。已在 `MirClientContext.Messages.cs` 文件头
  以偏差 D3 登记；若要 100% 字节保真，需把这两个局部量改为 `byte[]` 并把
  `ArrestStringEx_Ansi` 换成字节版（属 `HUtil32` 车道）。

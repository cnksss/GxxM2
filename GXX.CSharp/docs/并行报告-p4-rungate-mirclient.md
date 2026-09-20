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
| 3 | `63430f46` | 本报告（`docs/并行报告-p4-rungate-mirclient.md`） | ✅ |
| 4 | `cd287320` | **修跨车道重名（第 8 次）+ 吸收 main 增量文件 + `CheckUsePlugin` 切片 1** | ✅ build 0 error，1459/1459 |

**HEAD = `cd287320`**（除 #1 外每个提交都编译+测试全绿；#1 的编译错误由 #2 修复覆盖）。

### 1.1 吸收 main 增量的记录（`cd287320`）

我的开工基线 `76eb613d` 早于 main 后续提升的若干提交，导致两条分支各自可编译、**合并才炸**（CS0101/CS0111）。
`cd287320` 里按「先落地并已提升者为准」（台账 §12.8）做了两件事：

**(a) 删除本接缝层里 main 已有的重复定义**（第 8 次跨车道重名事故）：

| 类型 / 全局量 | main 上的权威定义 | 本车道处置 |
|---|---|---|
| `TAddressInfo` | `GateShareContainers.cs:71` | 删除自持副本，直接用 main |
| `TAddressListEx` | `GateShareContainers.cs:227` | 同上 |
| `TSafeStringList` | `GateShareContainers.cs:316` | 同上 |
| `TSafeMemoryStream` | `GateShareContainers.cs:407` | 同上（缺 `Clear`/`Size` → 用扩展方法补齐，见 §6.2-R7） |
| 26 个 `g_*` 全局量 | `GateShareGlobals.cs` | 删除自持副本，改为**只转发**（`=> GateShareGlobals.g_X`），存储唯一 |
| `TMagicInterval` / `TMagicIntervalList` | `GateShareMagicIntervalUtils.cs:62/:139` | 删除 `MagicIntervalUtilsSeam`，调用点直调真实现（`Find(unchecked((ushort)…))`） |

**(b) 吸收 main 的增量文件**（内容与 main **逐字节相同** → 合并时该路径为 no-op）：

- `src/GXX.RunGate/`：`GateShareAddressUtils.cs`、`GateShareContainers.cs`、`GateShareGlobals.cs`、`GateShareMagicIntervalUtils.cs`（新增）；
  `uFrmGameSpeedLogic.cs`、`uFrmMagicCD.cs`、`uFrmProcessBlacklist.cs`（改动）
- `tests/GXX.RunGate.Tests/`：`GateShareContainerTests.cs`、`GateShareMagicIntervalTests.cs`（新增）；
  `RunGateUtilsFormAddProcessBlackTests.cs`、`RunGateUtilsFormProcessBlacklistTests.cs`、`RunGateUtilsFormMagicCDTests.cs`（改动）

> 不吸收这 3 个 `uFrm*.cs` 就会与新搬入的 `GateShareMagicIntervalUtils.cs` 重复定义 `TMagicInterval`；
> 不吸收 3 个测试文件则测试工程编不过（main 已把 `TProcessBlackList` → `TProcessBlacklist`、
> `MaxCount` → `MaxCountForTest`）。这是本分支上已有先例的 "absorb main" 操作（见 `767ab38d`），
> **不是** rebase / merge。

**防复发（本车道实际执行的检查）** —— 凡新增一个类型作为接缝/替身，先查 main：

```powershell
git grep -l -E "(class|struct|enum|interface|delegate) +(partial +)?<TypeName>\b" main -- 'GXX.CSharp/src/GXX.RunGate/*.cs'
```

`cd287320` 提交前对**全部 27 个**本车道声明的类型跑过该命令 → ✅ 无重复定义。

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
| 24 | **`CheckUsePlugin`** | **3465-9688** | **部分覆盖**（见 §8 分派表） | `MirClientContext.CheckUsePlugin.cs` |
| 24a | ├ 局部常量 + 前导段 | 3467-3472、3505-3519 | **已完成** | 同上 |
| 24b | ├ `CM_DROPITEM` 分支 | 9474-9487 | **已完成** | 同上 |
| 24c | ├ `CM_PICKUP` 分支 | 9492-9505 | **已完成** | 同上 |
| 24d | ├ `else` 分支 | 9506-9516 | **已完成** | 同上 |
| 24e | ├ **公共收尾**（所有分支共用） | **9521-9681** | **已完成**（提取为 `CheckUsePluginPostlude`，逐行等价） | 同上 |
| 24f | ├ 异常兜底 | 9682-9686 | **已完成** | 同上 |
| 24g | └ **6 个 ident 族分派体** | **3528-9473** | **未覆盖**（显式早退 `UnportedIdentFamilies`，见 §8.2） | 同上 |
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
| `src/GXX.RunGate/MirClientContext.cs` | 1,724 | 1-301（声明/字段/记录）、308-412（单元级三函数）、416-470、473-679、1888-1918、2947-3120、3123-3463、9689-9965、10023-10249、10892-10896 |
| `src/GXX.RunGate/MirClientContext.Run.cs` | 1,322 | 681-1886（`Run` 全文） |
| `src/GXX.RunGate/MirClientContext.Messages.cs` | 1,805 | 1920-2945（`DoCheckRecvBuffer` 全文）、10453-10797、10799-10890、10899-11064、11069-11120；**10251-10451 为接缝空实现** |
| `src/GXX.RunGate/MirClientContext.CheckUsePlugin.cs` | 395 | 3467-3472、3505-3519、**9474-9516**、**9521-9681**、9682-9686；**3528-9473 未覆盖** |
| `src/GXX.RunGate/MirClientContextSeams.cs` | 880 | 接缝层（见 §6） |
| `tests/GXX.RunGate.Tests/MirClientContextTests.cs` | 1,230 | 93 例 |
| `tests/GXX.RunGate.Tests/MirClientContextCheckUsePluginTests.cs` | 460 | 39 例 |

另**吸收** main 的 4 个源文件 + 3 个改动源文件 + 5 个测试文件（见 §1.1，非本车道产出）。

### 3.1 阅读覆盖率（按物理 LF）

| 口径 | 行数 | 占比 |
|---|---|---|
| 原文总 LF | 11,125 | 100% |
| **已 1:1 覆盖** | **≈ 5,603** | **≈ 50.4%** |
| 接缝空实现（4 个方法） | 201 | 1.8% |
| **未覆盖 `CheckUsePlugin` 的 6 个 ident 族** | **5,472** | **49.2%** |
| 有意不移植（`Dissconnect` 9967-10021 + `CM_RUNGATEDOOR` 2662-2740） | 134 | 1.2% |
| 原文整段被注释（§2.1） | 231 | 2.1% |

> 已覆盖包含 `CheckUsePlugin` 的公共收尾 161 行（原以为它随分支未覆盖，切片 1 已落地）。
> 净口径：**唯一的功能缺口是 5,472 行的 6 个 ident 族分派体**（占原文 49.2%）。

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
| `dotnet test GXX.RunGate.Tests` | **Failed: 0 / Passed: 1459 / Skipped: 0 / Total: 1459** |
| 本车道新增用例 | **132**（93 + 39） |
| 基线 | main 侧 1366 例**全部保留通过**（0 新增失败） |

测试类（全部挂 `[Collection("RunGateFormLane")]`，与窗体族共享静态全局量，必须串行）：

| 测试类 | 用例数 | 覆盖 |
|---|---|---|
| `MirClientContextUnitTests` | 17 | `SubStringOccurences`（含"空子串在 Delphi 死循环、托管侧不死"的差异断言）、`UpdateLockUserList`/`GetUserLockTime`（保存开关、余秒、夹到 `nLockTime`、0/负值） |
| `MirClientContextVersionTests` | 6 | `GetExVersionNO` 的 0/负/阈值/多步/退一步/`int.MaxValue`（**溢出分支不可达**） |
| `MirClientContextCoreTests` | 70 | Create/DoReset、`CheckRecvPacketSize` 4 种 `g_BlockMethod`、消息队列 7 例、并发包计数（含恒返回 0 差异）、发送族 8 例、`FilterSayMsg` 全部 5 种模式 + 边界、`ContinuousSpeed`/`ProcessAssasinate`、`AddServerMsg/AddServerText` 超限、`DoConnect`/`DoDisconnect`/`CloseContextSocket`、包堆积拦截 |
| `MirClientContextCheckUsePluginTests` | 39 | 未覆盖族的**差异断言**（10 个 ident 参数化）、自定义技能区间边界、`CM_DROPITEM`/`CM_PICKUP`/`else`、环形缓冲回绕、公共收尾的 `ProcessMode` 六态、`SumProcessMode` 锁定/掉线、`boSpeedClearData`、`ClearConcurrentPacket` 恒 0 的连锁影响 |

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
| S5 | `GateShare.pas:133` | `TSafeStringList` | **已删**：main 的 `GateShareContainers.cs:316` 是权威定义（`Count/this[]/Add/Clear/IndexOf/Delete/Text/Lines/SaveToFile/LoadFromFile/Lock/UnLock`），本车道直接用 |
| S6 | `GateShare.pas:117` | `class TSafeHashStringListEx`（`Count/this[]/Add/AddObject/GetObject/SetObject/Objects/Delete/Clear/IndexOf/Strings/Lock/UnLock`） | 需要 `Objects[]` 的 `g_LockUserList`/`g_LoginMACPlayerList` 用；**自包含**（不继承任何既有类型）。main 的 `uFrmGameSpeedLogic.cs:834` `TSafeHashStringList` 无 Objects → 见 §6.2-R7 |
| S7 | `GateShare.pas:149` | `TSafeMemoryStream` + 扩展方法 `ClearStream()`/`StreamSize()` | **类型已删**（用 main 的 `GateShareContainers.cs:407`）；它缺 `Clear`/`Size` → 用扩展方法补，不改非本分区文件 |
| S8 | `GateShare.pas:308` | `class TGameSpeed`（含 `Clear()`） | 原文 record + `FillChar(…,0)` → 托管侧引用类型 + `Clear()`（偏差 D5） |
| S9 | `GateShare.pas:82/93` | `TAddressInfo` / `TAddressListEx` | **已删**：main 的 `GateShareContainers.cs:71/:227` 是权威定义，字段/成员完全够用 |
| S10 | `Common/IocpTcpServer.pas:33` | `abstract class TIocpClientContext`（`ContextID/RemoteAddr/Socket/IocpCore/IsPostedCloseQuest/IsWaitingGiveBack/LastRecvDataTick/PostSendText/PostSendBuffer/Close/CloseContextSocket/DoConnect/DoDisconnect/DoReset/DoCheckRecvBuffer`） | socket/线程本体按 §2.3 不移植 |
| S11 | `Common/IocpTcpServer.pas:119` | `class TIocpTcpServer { object BindObject }` | `GetRunGate`（`:9749-9752`）用 |
| S12 | `RunGateUtils.pas:60/151` | `interface ITcpClientSeam { bool Active; void SendServerMsg(int nIdent, ushort wSocketIndex, int nSocket, int nUserListIndex, byte[] buffer, int bufferLen); }` + `class TRunGate { TcpClient; OnlineUser; nMaxOnlineUserCount; dwCurDefenseLevel; nTotalAttackCount; dwClearTempTick; dwResotreDefenseTick; SendServerMsg(...) }` | `TRunGate` 的 socket 壳未 1:1（p2 报告 §6.1） |
| S13 | `GateShare.pas` 全局量 | **26 个只转发到 `GateShareGlobals`**（`g_CurrIPList`、`g_ScreenshotPath`、`g_sLogClientPacketDir`、`g_sReplaceWord`、`g_ClientAntiPlug*`、`g_RunGatePlugDllHandle`、`g_boOpenVerifyCode`、`g_nVerifyCode*`、`g_dwVerifyCode*`、`g_dwVerifySuccessAddInterval`、`g_boVerifyFail*`、`g_boVerifyCodeExcludeMap`、`g_VerifyCodeMapList`、`g_LoadNoVerifyChrList`、`g_VerifyFailUserList`、`g_boOneMACLimitePlayer`、`g_nOneMACLimitePlayerCount`）<br>**22 个自持**（main 上确实没有）：`g_dwClientAccumulateMaxSize`、`g_nClientCloseDelay`、`g_nClientLogoutDelay`、`g_boDelayCloseDisable*`(4)、`g_boBreakClient*`/`g_sBreakClient*`(4)、`g_boCheckClientPassword`、`g_sClientPassWord`、`g_sDisableSayMsg`、`g_sDisableSayMsgBegin`、`g_boLogoutNoResendAntiplugStream`、`g_boAntiplugAllLog`、`g_rgpRecvPacket`、`g_rgpStartContext`、`g_rgpEndContext`、`g_CSRunGatePlug`<br>**2 个自持且带 Objects**：`g_LockUserList`、`g_LoginMACPlayerList`（§6.2-R7） | 单一真源：转发后存储唯一，且调用点仍可用原文裸名 |
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
| R7 | `src/GXX.RunGate/uFrmGameSpeedLogic.cs` 的 `public class TSafeHashStringList`（**只读**） | 增加 `public object[] Objects { get; }`、`public int AddObject(string s, object o)`、`public object GetObject(int index)`、`public void SetObject(int index, object value)`（并让 `Add/Delete/Clear` 同步维护 Objects 槽） | 消掉本车道最后 2 个「同名但自持」的全局量：`g_LockUserList`、`g_LoginMACPlayerList`。原文 `MirClientContext.pas:336/:346/:377`（锁定时长秒数）与 `:2195/:2181/:10070/:10079`（单机登录计数）都需要 `Objects[Index]` 存取 **int**。**替代方案**：把 `GateShareGlobals.g_LockUserList`/`g_LoginMACPlayerList` 的类型换成带 Objects 的版本亦可。补齐后我会删掉 `TSafeHashStringListEx` 并把这两个量改成转发 | **中（不阻塞合并；阻塞"完全消掉接缝"）** |
| R8 | `src/GXX.RunGate/GateShareGlobals.cs` 的 `g_ClientAntiPlugDllString`（**只读**） | 由 `string` 改为 `byte[]`（原文该量承载反外挂模块**二进制**，`PChar` + `Length` 是字节口径；`GateShare.pas` 的赋值来自 `EncryptUnit` 加密后的缓冲） | 现在按 GBK 取字节（`GbkBytes`/`AnsiLen`）。GBK 是非满射映射，模块二进制含非法序列时往返会失真 → `SendAntiPlugStream` 分包内容可能被改写 | 低 |
| R1 | `src/GXX.RunGate/uFrmGameSpeedLogic.cs`（**只读**） | 见 R7（本条与 R7 合并） | — | — |
| R2 | `src/GXX.RunGate/GateShareGlobals.cs`（**只读**） | 把 §6.1-S13 里"自持的 22 个"搬进 `GateShareGlobals`（与原文声明位置 `GateShare.pas` 对齐） | 消掉接缝层里最后一块自持全局量 | 低 |
| R3 | `src/GXX.RunGate/GateShareMagicIntervalUtils.cs`（main 新增） | **已确认**：`TMagicIntervalList.Find` 形参为 `ushort`；本车道统一写 `Find(unchecked((ushort)MagicID))`，对 `Find(int)`/`Find(ushort)` 两版都安全 | — | ✅ 已关闭 |
| R4 | 后续 `Grobal2_Ex.pas` 车道 | `TClientMagic`（含 `TMagic_C` 与 `string[ITEM_NAME_LEN]`）、`TClientItem`（含 `TStdItem`）的 packed 布局 | 让 §2 的 #35-#38 四个接缝方法能 1:1 落地。**精确布局需求见 §9** | 中 |
| R5 | 后续 `uFrmMain.pas` 车道 | 把 `IFrmMainSeam` 的实现接到真实主窗体（`FrmMainSeam.FrmMain = …`） | 让 `DoCheckRecvBuffer` 的进程列表/状态文本真的刷新 | 低 |
| R6 | 后续 `GateShare.pas` 车道 | 把 `GateShareSeam` 的 22 个自持全局量与 6 个函数回调**改名为不冲突**并接管（或直接把 `GateShareSeam` 整体并入 `GateShare.cs`） | 消掉接缝层 | 低 |

---

## 7. 诚实说明：未完成部分与剩余量

### 7.1 唯一的功能缺口：`CheckUsePlugin` 的 6 个 ident 族（**5,472 行，占原文 49.2%**）

原文 **3528-9473** 未移植。当前行为：这些 ident 由 `UnportedIdentFamilies(ident)` 命中后
**显式早退 `return false`**（= 不判定 = 放行），**不写入 `RecordActionArr`、不进入公共收尾**。

> **为什么不干脆让它们落进 `else`**：原文 `else`（9506-9516）会把
> `RecordActionArr[nRecordActionIndex].Action` 写成 `baOther`，而走路/跑步/转向/攻击/魔法各自要写
> `baWalk`/`baRun`/`baTurn`/`baHit`/`baSpell`；更严重的是公共收尾（9543-9547、9680）会对**每个**走路/攻击包
> 执行 `GameSpeed.boContinueSpeed` 状态迁移，直接破坏"连续超速"状态机。
> 因此"整族不处理（零副作用）"严格优于"错写状态"。
> **删除条件**：某族移植完成后，把 `UnportedIdentFamilies` 里对应的一行删掉即可（其它族不受影响）。

### 7.2 部分完成 / 接缝（201 行）

`ProcesssSendToClientSendMyMagic` / `SendAddMagic` / `BagItems` / `AddItem` 四个方法**空实现**，
原因：依赖 `Grobal2_Ex.pas` 的 packed record `TClientMagic`（含 `TMagic_C` 与 `string[ITEM_NAME_LEN]` 短串）
与 `TClientItem`（含 `TStdItem`）。本车道**不定义**这些类型 —— 跨车道类型重名事故本工程已发生 **8 次**，
且这些类型属 `Grobal2_Ex.pas` 车道的类型面。
四个方法的原文语义已在文件注释里写清（压缩流长度校验 → `g_MagicCDList.Find` 覆写 `dwInterval`/`dwRealInterval`
→ 回写 `AddServerMsg`；背包按 `s.StdMode in [0,2,3]` 填 `MakeIndex/StdMode/Shape/AC1/MAC1`）。
**精确布局需求见 §9。**

### 7.3 有意不移植（134 行，§2.3）

`Dissconnect`（9967-10021，PE 头反调试）与 `CM_RUNGATEDOOR` 的 RSA/提权分支（2662-2740）。

### 7.4 未做的验证

- **无端到端回归**：`Run` / `DoCheckRecvBuffer` 的 socket 侧走接缝，没有真实客户端↔网关↔M2 的回环测试。
- **`Run` 只做了分支级单测**，未覆盖全部 30+ 个 `Ident` 分派分支（`CM_SAY` 的 `GetValidStr3`+`FilterSayMsg` 串联、
  `CM_QUERYBAGITEMS` 的 `LockUser(GetUserLockTime)` 链路、魔法 CD 超速放行的 `MagicCDSpeed[10]` 环形窗口
  已在实现里逐行落地，但缺测试）。
- **`DoCheckRecvBuffer` 的进程列表/截图/客户端文件落盘三条路径**只做了代码级落地，未做 IO 级测试。
- **`CheckUsePlugin` 的 `FLastAction = baSpell` 补帧分支**：`FLastAction` 是 private，公开探针只读，
  因此测试只能反向固定"baOther 时不补帧"（帧数 1 而不是 2）。若要正向覆盖，需在接缝层加 `LastActionForTest` 可写探针。
- **GBK 承载的保真风险**：`sData`/`sDataMsg`（原始二进制 AnsiString）在托管侧用 `string` 承载，经 GBK 往返。
  6-bit 编码体是 ASCII 安全的，但 `sDataMsg` 的附加负载含任意字节时可能不无损。已在
  `MirClientContext.Messages.cs` 文件头以偏差 D3 登记。

---

## 8. `CheckUsePlugin` 分派表（原文 3465-9688）

### 8.1 结构

原文 `:3521-3529` 是：

```pascal
{$IF NEED_REGISTER = 0}
  case DefMsg.Ident of
{$IFEND}
{$IF NEED_REGISTER = 0}
    CM_WALK:
{$ELSE}
    if DefMsg.Ident = CM_WALK then
{$IFEND}
```

活分支 **`NEED_REGISTER = 1`**（`Grobal2_Ex.pas:18`）→ **顶层是 if / else-if 链，不是 `case`**。
顶层只有 **9 个分支**（实测）：

| # | 分派键 | 起始行 | 结束行 | 行数 | 语义一句话 | 本车道 |
|---|---|---|---|---|---|---|
| 1 | `CM_WALK` (3011) | 3528 | 4696 | 1,169 | 走路：记录 `baWalk`；走路并发/走路限速 + 转向→走路 等模式判定 | **未覆盖** |
| 2 | `CM_RUN` (3013) | 4697 | 5857 | 1,161 | 跑步：与走路同构（`baRun`、跑步限速、走路→跑步） | **未覆盖** |
| 3 | `CM_TURN` (3010) | 5858 | 6689 | 832 | 转向：记录 `baTurn`；转向并发/转向到移动/移动到转向 | **未覆盖** |
| 4 | 攻击族（`CM_HIT`=3014 … `CM_115HIT` + `CM_CUSTOM_HIT001..+300`） | 6690 | 7888 | 1,199 | 记录 `baHit`；攻击并发 + 20 种攻击动作各自的前后摇间隔判定（7809-7847 是"动作名→日志文本"的二级 if 链） | **未覆盖** |
| 5 | `CM_SPELL` (3017) | 7889 | 8999 | 1,111 | 记录 `baSpell`；魔法并发 + `SkillId` 维度判定 + 技能 CD 超速 | **未覆盖** |
| 6 | `CM_SITDOWN` (3012) | 9000 | 9473 | 474 | 记录 `baCutMeat`（注释写"挖肉"）；**暗杀检测**（环形缓冲 `RecordActionArr` 回溯，`nAssasinate >= 3` → `ProcessAssasinate`）+ "移动到挖肉"限速（采集池 `dwCollectIntervalArr`） | **未覆盖** |
| 7 | `CM_DROPITEM` (1000) | 9474 | 9487 | 14 | 记录 `baOther`、`FLastAction := baOther` | ✅ 已完成 |
| 8 | `CM_PICKUP` (1001) | 9492 | 9505 | 14 | 同 7 | ✅ 已完成 |
| 9 | `else` | 9506 | 9516 | 11 | 其它全部 ident：记录 `baOther` | ✅ 已完成 |
| — | **公共收尾（所有分支共用）** | **9521** | **9681** | **161** | 发超速提示 → `AntiPlugAction` 非空则：调脚本(`CM_SENDUSERSPEEDING`)、置 `boContinueSpeed`、累计超速门限(`nSumSpeedMaxCount`)→`LockUser`/`DelayClose`、`boSpeedClearData` 清队列、按 `ProcessMode` 六态决定 `Result` 与副作用 | ✅ 已完成 |
| — | 异常兜底 | 9682 | 9686 | 5 | `AddMainLogMsg('…CheckUsePlugin Error, Code = …')` | ✅ 已完成 |

> 注：`:9471-9475` / `:9489-9493` / `:9517-9519` 是 `{$IF NEED_REGISTER = 0}` 的 case 标签 / `end; // end case`
> 包裹行（死分支），活分支下等价展开。

### 8.2 未覆盖族的显式早退（本车道唯一结构性偏差）

`MirClientContext.CheckUsePlugin.cs` 的 `UnportedIdentFamilies(ushort ident)` 命中上表 #1-#6 的 ident →
`return false`。**删除条件**：移植完某族后删掉对应行。

### 8.3 接管 #1-#6 需要的既有资产（**不必重写**）

- 纯判定内核：`RunGateUtilsClientStat`（多数表决）、`RunGateForwardClassifier`、`RunGateTiming.TickDiff`、
  `IocpUtilsPolicy`。
- 配置/枚举：`uFrmGameSpeedLogic.cs` 的 `TAntiPlugActionMode` / `TAntiPlugAction` / `TAntiPlugConfig` /
  `TActionProcessMode` / `TSumActionProcessMode` / `RunGateConst.AntiPlugActionModeNames(2/3)` /
  `FormGlobals.g_wActionSpeedIntervals` / `ActionModeUseSpeedIntervals`。
- 本类已就绪的状态：`dwCollectIntervalArr` / `nCollectIntervalIndexArr` / `RecordActionArr` /
  `nRecordActionIndex` / `SumSpeedProcessArr` / `nCompensationArr` / `LastLockAntiPlugActionMode` /
  `FLastAction` / `GameSpeed` / `nMoveSpeed` / `nAttackSpeed` / `nSpellSpeed` / `MagicUseTickList` /
  `LastEatingItemTick` / `LastHeroEatingItemTick` / `HumBagItems` / `HeroBagItems`。
- 本类已就绪的方法：`GetConcurrentPacketCount` / `ClearConcurrentPacket` / `ContinuousSpeed` /
  `ProcessAssasinate` / `SendMessaggeToClient` / `DelayClose` / `SendActionRet` / `AddServerMsg` /
  `CheckUsePluginPostlude`（公共收尾已提取，各族**只需**产出 `AntiPlugAction`/`sSendMsg`/`nDelayTime`/
  `IsDropConcurrent` 后调用它）。

---

## 9. 给 `Grobal2_Ex.pas` 车道的 packed record 布局需求（R4，精确）

`ProcesssSendToClient*` 四个接缝方法需要下列类型；本车道**不越区定义**，只登记需求。
以下行号来自 `Source/RunGate/Grobal2_Ex.pas`（只读抽取；`ITEM_NAME_LEN` / `MAX_FLUTE_COUNT` /
`ITEM_PROP_COUNT` / `ITEM_PROP_VALUES_COUNT` 需在该单元确认后再定偏移）：

| 类型 | 原文行号 | 本车道用到的字段 | 备注 |
|---|---|---|---|
| `TMagic_C` | `Grobal2_Ex.pas:307-321` | `wMagicId: Word` | `packed record`，含 `sMagicName: string[ITEM_NAME_LEN]` |
| `TClientMagic` | `Grobal2_Ex.pas:323-333` | `Def: TMagic_C`（用 `Def.wMagicId`）、`dwInterval: LongWord`、`dwRealInterval: LongWord` | 大小即 `SizeOf(TClientMagic)`（`10267/10315` 用它做长度校验） |
| `TStdItem` | `Grobal2_Ex.pas:350-399` | `StdMode: Byte`、`Shape: Word`、`AC1: Integer`、`MAC1: Integer` | `packed record`，含 `Name/DBName: string[ITEM_NAME_LEN]` |
| `TClientItem` | `Grobal2_Ex.pas:441-464` | `s: TStdItem`（用 `s.StdMode/Shape/AC1/MAC1`）、`MakeIndex: Integer` | `SizeOf(TClientItem)` 用于 `10355/10417` 的长度校验 |

**用到的语义**（照抄原文，无需改设计）：
- `ProcesssSendToClientSendMyMagic`(10251-10303)：`zLibDecompressBuffer` 后长度 = `SizeOf(TClientMagic) * DefMsg.Series`
  → 逐条 `g_MagicCDList.Find(Def.wMagicId)` 覆写 `dwInterval`+`dwRealInterval`，有改动则 `zLibCompressBuffer` 回写 +
  `DefMsg.Param := 压缩后长度` + `AddServerMsg`。
- `ProcesssSendToClientSendAddMagic`(10305-10340)：`MsgLen = SizeOf(TClientMagic)` 时对单条做同样覆写 + `AddServerMsg`。
- `ProcesssSendToClientBagItems`(10342-10407)：`MsgLen = DefMsg.Param` 且解压后长度 = `DefMsg.Series * SizeOf(TClientItem)`
  → 先 `Clear` 再按 `s.StdMode in [0,2,3]` 填 `MakeIndex/StdMode/Shape/AC1/MAC1`（`Inc(ClientItem)` 步进）。
- `ProcesssSendToClientAddItem`(10409-10451)：`MsgLen = SizeOf(TClientItem)` 时同上，**不 Clear**。

> 本车道刻意**不**移植 `string[ITEM_NAME_LEN]` 的 `ShortString` 处理 —— `GXX.Core.Protocol.ShortStr` 已有
> 1:1 实现（`ShortStr.cs:11-75`），接管者应直接复用，不要另写。


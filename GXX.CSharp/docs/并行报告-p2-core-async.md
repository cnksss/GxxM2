# 并行报告 · 车道 `p2-core-async`（`par/p2-core-async`）

> 任务：`AsyncCalls.pas` **三份同源副本**移植一次，三处共用
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p2-core-async`（唯一写入目录）
> 完成日期：2026-09-19 ｜ 状态：**门禁全绿，已提交**

---

## 1. 分支与提交

| 项 | 值 |
|---|---|
| 分支 | `par/p2-core-async` |
| 移植提交 | `3016ccee3b41d72694a56fb08ea2a109fe384b1d`（并行批次P2：AsyncCalls.pas 三份同源副本移植 + 266 项测试） |
| 报告提交 | 见本节末尾（本文件所在提交） |

改动范围**严格限于**任务书授权的独占区，未触碰 `GXX.slnx` / 任何 `*.csproj` / `Directory.Build.props` /
`docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `docs/Checklist.md` / `tools/**`，也未读写任何兄弟
`.worktrees\p2-*` / `.worktrees\integration`。提交前已删除临时探查目录 `_tmp_src/`（`git status` 仅剩本次新增文件）。

---

## 2. 三份副本的同源验证：**结论：逐字节完全相同，移植一次**

### 2.1 证据（可复现）

```
D:\chuanqi\daima\GXX原版_Delphi7\Source\M2Engine\AsyncCalls.pas
D:\chuanqi\daima\GXX原版_Delphi7\Source\Client-HGE\AsyncCalls.pas
D:\chuanqi\daima\GXX原版_Delphi7\Source\RunGate\AsyncCalls.pas
```

| 校验项 | M2Engine | Client-HGE | RunGate |
|---|---|---|---|
| 文件字节数 | 103,449 | 103,449 | 103,449 |
| SHA-256 | `82262F0AC03D848D7ABEC5A4559914EE1574C1CA2CEFAEA4BC5C1C8F49D0C048` | 同左 | 同左 |
| 物理行数 | 3,439 | 3,439 | 3,439 |
| 非空行数 | 2,959 | 2,959 | 2,959 |
| CRLF 数 | 0 | 0 | 0 |

- `[System.Linq.Enumerable]::SequenceEqual(byte[], byte[])`：**A==B = True，A==C = True**（全字节序列相等）。
- 按 GBK(CP936) 解码后逐行 `-cne` 比较：**三份之间差异行数 = 0**。
- 非空行数 2,959 与 `docs/并行覆盖审计.md:41-43` 记录的 “2959” 完全吻合（该列即“非空行数”）。

**结论**：三份**逐字节完全相同**，不存在需要抽成参数/接缝的差异点，故按 M2Engine 版为基准**移植一次**，
产物置于 `src/GXX.Core/Async/`，由 M2Engine / Client-HGE / RunGate 三处共用；每个源文件头部均注明
“对应三份同源 .pas 副本”。

### 2.2 条件编译基线（决定哪些分支是"活的"）

原树为 Delphi 7（CompilerVersion 15.0，`VER140` 未定义），据此：

| 符号 | 状态 | 影响 |
|---|---|---|
| `SUPPORT_LOCAL_FUNCTIONS` | **生效**（`MSWINDOWS and not CPUX64`，30-34 行） | 局部函数/`array of const` 相关代码段（92-99、347-381、965-997、1083-1096、1302-1350、1379-1407、2228-2410、2626-2758、3419-3422）**均参与编译**，必须移植 |
| `DELPHI7_UP` | **生效**（45-50 行） | `{$IFNDEF DELPHI7_UP}` 段（Delphi5/6 的 `SyncEvent` 声明、`TThread` 补丁、`HookWakeMainThread`）**不编译**；`SyncEvent` 实际解析为 `Classes.pas` 的全局事件 |
| `DELPHI2009_UP` | 不生效 | `TMultiArgProcCall<...>`（576-606）、`TAsyncCalls` 类（608-717）及其实现（3049-3326）**不编译** |
| `DELPHI5` / `DELPHI6` | 不生效 | 783-807、809-830 段不编译 |
| `UNICODE` | 不生效 | 所有 `vtUnicodeString` 分支（2269-2271、2288、2300、2388-2390）不编译 |
| `DEBUG_ASYNCCALLS*` / `DEBUG_THREADSTATS` | 未定义 | 调试/计时分支不编译 |

---

## 3. 新增文件清单与源单元行号范围

### 3.1 `src/GXX.Core/Async/`（新建 10 个文件，共 3,589 行）

| 文件 | 行数 | 对应 `AsyncCalls.pas` 行号范围 | 内容 |
|---|---|---|---|
| `AsyncCallsConst.cs` | 151 | 129、385-444、726-736、866、1102-1113、1665-1677 | resourcestring 6 条、`WM_VCLSYNC/WM_RAISEEXCEPTION`、`MAXIMUM_ASYNC_WAIT_OBJECTS`、`WAIT_*`/`QS_*`、线程数组长度、`NotFinishedError`/`UnknownVarRecType` |
| `IAsyncCall.cs` | 174 | 90-127、129-175 | `TAsyncCallError`、`IAsyncCall`/`IAsyncCallEx`/`IAsyncRunnable`、22 个委托族 |
| `TVarRec.cs` | 517 | 1084-1096、2231-2414 | `TVarRecType`（vt0-17）、`TVarRec` 托管替身、`CopyVarRec`/`ReleaseVarRec`/压栈分类与字节记账 |
| `AsyncCallSeams.cs` | 284 | 接缝（无原文对等代码；对应 495、1411-1520、1694-1746、1756-1760、1976-1986、2324-2408 的 Win32/asm 调用点） | `IAsyncWaitObject`/`IAsyncWaitService`/`IAsyncThreadLauncher`/`IMainThreadDispatch`/`IVarRecInvoker` + 真实实现 |
| `TInternalAsyncCall.cs` | 394 | 489-533、1411-1434、2032-2225 | 内部调用基类：事件、引用计数、状态机、异常暂存与重抛 |
| `TAsyncCall.cs` | 187 | 536-574、1991-2027、3332-3415 | `TAsyncCall` 外壳（`CheckForget`/`Forget` 断链）+ `TSyncCall` |
| `TThreadPool.cs` | 452 | 841-899、1688-1990、1751-1807、1976-1986 | `TThreadPriority`、池化线程、FIFO 队列、扩容策略、`AllocThread` CAS、销毁排空 |
| `TAsyncCallArgs.cs` | 421 | 903-1096、2415-2685 | 20 个具体 `TInternalAsyncCall` 派生类（按参数类型） |
| `TAsyncCallMultiSync.cs` | 432 | 385-444、1411-1663 | `TAsyncMultiSyncPlan`（映射/换算）、`WaitForSingleObjectMainThread`、`WaitForMultipleObjectsMainThread`、`InternalAsyncMultiSync`、对外 4 函数 |
| `AsyncCalls.cs` | 577 | 576-717、1100、1102-1113、1117-1406、1641-1663、3079-3091、326-3326 | 单元级全局 `ThreadPool`、`Set/GetMaxAsyncCallThreads`、`AsyncCall` 全家族、`AsyncCallEx`、`AsyncExec`、`TAsyncCallsFacade`（`MsgExec`/`VCLInvoke`/`VCLSync`） |

命名空间：`GXX.Core.Async`（与 `GXX.Core.Util` / `GXX.Core.Protocol` / `GXX.Core.Rtl` 惯例一致）。
`GXX.Core.csproj` / `GXX.Core.Tests.csproj` 均为默认通配包含，**无需改动 csproj**。

### 3.2 `tests/GXX.Core.Tests/`（新建 6 个文件，共 3,311 行）

| 文件 | 行数 | 用例数 | 覆盖的原文范围 |
|---|---|---|---|
| `AsyncTestDoubles.cs` | 321 | （替身，0 用例） | 脚本化等待服务 / 假事件 / 线程启动记录器 / 主线程派发记录器 / 变参调用接缝替身 |
| `AsyncVarRecTests.cs` | 406 | 76 | 2231-2414（`CopyVarRec`、压栈分类、字节记账、Self 插入） |
| `AsyncCallStateTests.cs` | 692 | 60 | 1991-2225、3332-3415（状态机、引用计数、异常重抛、`TAsyncCall`/`TSyncCall`） |
| `AsyncThreadPoolTests.cs` | 577 | 48 | 1102-1113、1688-1990（队列、扩容策略、CAS、销毁、线程主循环、窗口过程） |
| `AsyncMultiSyncTests.cs` | 669 | 40 | 1411-1663（映射、句柄压缩、结果换算、4 个对外函数） |
| `AsyncCallFacadeTests.cs` | 646 | 42 | 1102-1406、1287-1299、326-3326（全重载、`AsyncExec`、`MsgExec`、`VCLInvoke`、全局量） |
| **合计** | **3,311** | **266** | |

---

## 4. 测试与门禁结果

```
cd .worktrees\p2-core-async\GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo          → 0 个错误，88 个警告（全部来自既有文件；本次新增 10 个源文件 0 警告）
dotnet test tests\GXX.Core.Tests\...csproj -c Debug → 已通过! 失败: 0，通过: 428，总计: 428
```

- 新增用例 **266** 项；`GXX.Core.Tests` 总量由 162 → **428**，全绿。
- 等待全部经 `IAsyncWaitService` 脚本化替身返回，**断言中不含 `Thread.Sleep` / `Stopwatch` / 真实时间**；
  线程经 `IAsyncThreadLauncher` 替身记录，测试过程**不起真实线程**。
- 每个公开方法 ≥3 用例，并覆盖空/0/负/超时/回绕/越界边界。

### 4.1 “看起来一样实则不同”的差异断言（逐条已落成用例）

| # | 分支 | 断言要点 |
|---|---|---|
| 1 | `AsyncCall(IntegerProc)` vs `AsyncCall(ObjectProc)`（1126-1129、1187-1190） | Integer 重载**自己不做** `MaxThreads = 0` 判定，而是 `TObject(Arg)` 转调 Object 重载；入队类型是 `TAsyncCallArgObject`（原文**没有** `TAsyncCallArgInteger` 类） |
| 2 | `CopyVarRec` 深拷贝分支 vs `else Result := Data` | 深拷贝分支**只**赋 `VType` + 指针槽（对齐填充保持默认）；`else` 分支整体复制（填充一并搬运）。指针为 `nil` 的深拷贝类型走 `else` |
| 3 | `vtUnicodeString(17)` | D7 基线**不受支持**：`GetPushKind(17)` 抛 `UnknownVarRecType`，且不在深拷贝集合内（D2009+ 才生效） |
| 4 | `InternalAsyncMultiSync` vs `InternalWait` 的 `Count = 0` | 前者返回 `WAIT_FAILED`，后者返回 `WAIT_OBJECT_0`（两处语义相反） |
| 5 | `WaitAll = True` 时底层 `bWaitAll` | 原文在 `MsgWait` 分支与普通分支**都硬编码 False**（1482、1491），改为“逐个等待 + 压缩句柄数组” |
| 6 | `WaitForMultipleObjectsMainThread` 返回值 | `WaitAll=True` 时返回**第一个**完成的句柄下标（`FirstFinished`），不是最后一个 |
| 7 | 主线程路径句柄数 | = `Count + 2`（追加 RTL `SyncEvent` 与池 `MainThreadSyncEvent`）；非主线程 = `Count` |
| 8 | `WaitForSingleObjectMainThread` 超时 | 被同步事件反复唤醒时**不递减** `Timeout`，总等待可远超给定值 |
| 9 | `TInternalAsyncCall.Forget` | 内部对象上**就是** `ForceDifferentThread()`，无其它动作 |
| 10 | `SyncInThisThreadIfPossible` | 池销毁态 `or` 压过 `FForceDifferentThread`（`not Force or Destroying`） |
| 11 | `WAIT_FAILED == INFINITE == 0xFFFFFFFF` | 数值不可区分（同台账 §11.4 的 `MakeIPToInt` 一类），已登记**不得修正** |
| 12 | `ComputeDefaultMaxThreads(0)` | = **-2**（原文只钳上界，无下界） |
| 13 | `SetMaxAsyncCallThreads` 负数 | **被忽略**（保持原值）；`>= 256` 钳制为 256 |
| 14 | 取消后取值 | `CancelInvocation` 后 `Finished()` 立即为 True、`ReturnValue()` 返回 0 **且不抛**；`Canceled()` 此时仍为 False |
| 15 | 异常重抛 | `Sync`/`ReturnValue` 重抛暂存异常一次后置 `nil`，**第二次取值不再抛** |
| 16 | `array of const` 的同步降级 | `MaxThreads = 0` 时返回**真正的 `TAsyncCall`**（而非 `TSyncCall`），因为原文走 `InternExecuteSyncCall + TAsyncCall.Create` |
| 17 | `AllocThread` 超限 | `Index = FMaxThreads` 时循环退出且**静默不建线程**；`SetMaxThreads` 可把上限降到当前线程数之下而不回收已建线程 |
| 18 | 取消在 `Finished` 中的优先级 | `FCanceled or (FCancelInvocation and not FExecuted)` **先于**事件判定 |

---

## 5. 发现的原文缺陷 / 易错点

1. **`WAIT_FAILED` 与 `INFINITE` 同值**（均 `$FFFFFFFF`）：`InternalAsyncMultiSync`（1638 行）在对象数超限时返回
   `WAIT_FAILED`，而 `Milliseconds` 默认值正是 `INFINITE` —— 调用方无法区分“失败”与“无限等待”常量。原设计固有，已登记不修。
2. **`Count = 0` 的两处相反语义**（1604 vs 1638）：`InternalWait` 认为“全部已完成”，而外层门限判定把它当“失败”。
3. **超时不递减**（1422-1428、1456-1474）：主线程等待循环复用同一个 `Timeout`，被同步事件反复唤醒时总耗时无上界。
4. **`WaitAll=True` 分支的句柄压缩**（1498-1513）：1498 行把下界判断 `{(Result >= WAIT_OBJECT_0) and}` **注释掉了**，
   仅靠 uint 比较 `Result <= WAIT_OBJECT_0 + Count`；`Move` 长度为 `((Count + 2) - Index)` 而非新数组长度，
   压缩后数组尾部存在失效槽 —— 托管侧必须按 `Count + 2` 切片传递，否则会多等一个失效句柄（本次移植时已修正并加守卫用例）。
5. **`CopyVarRec` 的 `nil` 语义**（2286-2312）：只要指针为 `nil` 就**不**做深拷贝，转而整体复制（含对齐填充）。
6. **`EAsyncCallError.CreateFmt` 使用 Delphi `%d` 占位符**：移植时若用 `string.Format` 会原样输出 `%d`
   （首轮测试真实踩到），必须走 `GXX.Core.Rtl.DelphiRTL.Format`。
7. **`vtUnicodeString` 在本树不可用**：D7 下传入 17 会抛 `UnknownVarRecType`，且不在深拷贝集合内。
8. **`AsyncCall(TCdeclFunc, array of const)` 的同步降级与其它重载不一致**（1380-1392）：返回 `TAsyncCall` 而非 `TSyncCall`。
9. **`AsyncExec`/`MsgExec` 的循环常量 `= 1`**（1296、1336、3318）：`1` 即“单个等待对象的列表被消息唤醒”时的
   `WAIT_OBJECT_0 + Count`；字面量含义隐晦，改写它等于改行为。
10. **`{$IFNDEF DELPHi7_UP}`（3423 行）小写 `i`**：Delphi 条件符号大小写不敏感，故该“笔误”无实际影响 —— 已注释留档。
11. **`FThreads: array[0..255]`**（866 行）使线程上限恒为 256；`SetMaxAsyncCallThreads` 用 `>= Length` 判定，
    因此“正好 256”会走到赋值分支而非钳制分支（结果相同，路径不同）。
12. **源文件行尾是 LF（CRLF=0）**，与本工程其它文件的 CRLF 不一致：`git add` 时提示 “LF will be replaced by CRLF”。
    比较“同源副本”时应注意这一格式特征，避免用行尾敏感的工具误判。
13. **`TAsyncCall.Destroy` 会阻塞等待**（3339-3350）：接口释放时若异步调用未完成，`FCall.Sync` 会阻塞调用方线程
    并把异步异常在调用方线程重抛 —— 这是 `IAsyncCall` 引用释放路径上的隐式同步点。

---

## 6. 接缝（未移植依赖）与未完成项

### 6.1 已定义的最小接缝

| 接缝 | 位置 | 承接的原文内容 | 备注 |
|---|---|---|---|
| `IAsyncWaitObject` / `IAsyncWaitService` | `AsyncCallSeams.cs` | `CreateEvent` / `WaitForSingleObject` / `WaitForMultipleObjects` / `MsgWaitForMultipleObjects`（1411-1520、2035、2075） | **时间可注入点**；测试用脚本化替身，零真实耗时 |
| `IMainThreadDispatch` | 同上 | `AllocateHWnd` / `PostMessage(WM_VCLSYNC)` / `PeekMessage`+`DispatchMessage`（1756、1930、1981-1985、1959-1974）、RTL `CheckSynchronize` 与 `Classes.SyncEvent`（1416、1448、1425、1471） | `// 接缝：待 VCL 消息泵（Forms.pas / Classes.pas）移植后接入` |
| `IAsyncThreadLauncher` | 同上 | `TAsyncCallThread.Create(False)` 与线程优先级 `tpHigher`/`tpNormal`（1691、1711、1923） | 替身可记录“应启动第几号线程” |
| `IVarRecInvoker` | 同上 | `TAsyncCallArrayOfConst.ExecuteAsyncCall` 的 x86 cdecl 压栈与 `jmp`（2324-2408 内联 asm） | 可移植部分（分类/深拷贝/字节记账）已实现并单测；调用本体待接 |
| `TAsyncCallsFacade.VCLSync` 非主线程分支 | `AsyncCalls.cs` | `StaticSynchronize(TThreadMethod(M))`（3288） | 当前抛 `NotSupportedException`（`// 接缝：…待 Classes.pas 移植后接入`） |
| COM 套间 | `TThreadPool.ThreadExecute` | `CoInitialize`/`CoUninitialize`（1703-1705、1743-1744） | 托管侧由宿主决定，注释留档 |
| `ApplicationHandleException` + `WM_RAISEEXCEPTION` | `TInternalAsyncCall.Destroy` / `TThreadPool.MainThreadWndProc` | 未取回异常的主线程重抛（2050-2052、1965-1966） | 经 `IMainThreadDispatch.PostVclSync` 承接 |

### 6.2 明确未移植（附理由）

1. **`EnterMainThread` / `LeaveMainThread` / `ExecuteInMainThread` / `InitStackBuffer` / `TMainThreadContext`（2762-3050）**
   —— 纯 32 位汇编栈指针切换（`esp`/`ebp` 备份恢复、`System.@HandleFinally` 地址比较、栈缓冲搬运）。
   托管运行时无可对应语义，按总规程 §2.3 处理，不做空实现以免误导调用方。
2. **局部函数入口 asm 桩**：`LocalAsyncCall`（1309-1313）、`LocalAsyncCallEx`（1321-1325）、
   `LocalAsyncExec`（1343-1347）、`LocalVclCall`（2727-2730）、`LocalAsyncVclCall`（2754-2758）—— 依赖 `EBP` 注入。
   其**目标类型**（`TAsyncCallLocalProc` / `Ex` / `TAsyncVclCallLocalProc`，2628-2685）已移植，
   `FBasePointer` 在托管侧由闭包捕获取代（差异已在注释登记）。
3. **`{$IFDEF DELPHI2009_UP}` 泛型家族**（576-717、3049-3326）：`TMultiArgProcCall<TProc,T1..T4>`、
   `TAsyncCalls.TAsyncCallArg<T…>` / `TAsyncCallArgMethod<T…>`（8 个 `ExecuteAsyncCall` 重写）、
   `TAsyncCalls.Invoke<T>`（8 个重载）、`Invoke(TIntFunc)` / `Invoke(TProc)`、
   `TAsyncCallAnonymProc` / `TAsyncCallAnonymFunc`。
   **理由**：Delphi 7 下整段不参与编译（见 §2.2），属 D2009+ 才存在的入口。
   其中与编译器版本无关的**决策语义**已按语义移植：`MaxThreads = 0 ⇒ 就地同步执行`、
   `VCLInvoke` 的主线程短路、`MsgExec`/`AsyncExec` 的空闲消息循环（`TAsyncCallsFacade`）。
4. **`TAsyncCallArrayOfConst` 的真实调用**（接缝占位，见 6.1）。
5. **`InternLocalVclCall` / `LocalVclCallProc`（2697-2724）的 VCL 局部函数派发**：依赖 `TLocalVclCallRec` 栈上记录与 asm 取参。

### 6.3 给集成者 / 覆盖审计的提示

- 本次改动把**三份** `AsyncCalls.pas` 一次性消解到 `src/GXX.Core/Async/` 一处。
  `tools/audit-coverage.ps1` 的 E2 证据规则（`.cs` 文件头 40 行内提及 `<unit>.pas`）对
  **全部 10 个**新增源文件均已满足（每个文件头 40 行内都出现 `AsyncCalls.pas`，命中 1~6 次），
  故 `M2Engine/Client-HGE/RunGate` 三行应**同时**由 `unmapped` 转为 `mapped`。
- 未运行 `tools/audit-coverage.ps1`（它会重写 `docs/并行覆盖审计.md`，属共享文件，车道不得触碰），
  上述结论以“文件头 grep”作为可核验证据给出。
- 本车道**只新增文件**，未修改任何共享文件；与其它车道的独占区交集为空。

---

## 7. 与原文字面保真的取舍（逐条登记）

| 原文 | 本移植 | 理由 |
|---|---|---|
| `Extended`（80 位） | `double`（64 位） | .NET 无 80 位浮点；已注释登记。`vtExtended` 的**压栈字节数仍按 10 字节（3 DWORD、+8）**记账，保持 wire/栈布局语义 |
| `AnsiString` / `WideString` | `string` | 总规程 §3.1；协议层字节语义不涉及本单元 |
| `TList` / 链表 | 手工 `FNext` 链（保留原文结构） | `TThreadPool` 的入队/摘除/尾部修正逻辑是原文语义的一部分，未替换为 `Queue<T>` |
| `TRTLCriticalSection` + `InitializeCriticalSectionAndSpinCount(..., 4000)` | `lock (object)` | 托管侧不可设自旋计数；语义等价（已注释） |
| `TThread.Terminate` / `Terminated` | `Terminate()` / `Terminated`（池级标志） | 线程外壳替身化 |
| `raise E at FFatalErrorAddr` | `ExceptionDispatchInfo.Capture(E).Throw()` | 托管侧无“指定机器地址重抛”；保留原始托管栈，信息更完整 |
| `ExceptAddr` | `IntPtr.Zero`（占位） | 托管侧无抛出点机器地址概念 |
| 单元 `initialization` 里**急切**创建全局线程池（3418） | **惰性**创建（`AsyncCallsGlobals.ThreadPool`） | 避免无异步需求的进程/测试产生 OS 资源；行为等价，差异已注释 |
| 单元级全局 `ThreadPool` 的隐式依赖 | 每个门面入口追加**可选** `pool` / `runtime` 参数（省略即取全局） | 可测性注入钩子；省略时与原文一致 |
| 原文私有成员（`FMaxThreads`、`FAsyncCallHead`、`FNext` 等） | 增补只读访问器（`MaxThreads`、`AsyncCallHead`、`Next`、`RefCount`、`Executed` 等） | 仅为可测性暴露，注释标明“不改变语义” |

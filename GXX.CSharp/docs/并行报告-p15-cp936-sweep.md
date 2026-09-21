# 并行报告 — p15-cp936-sweep（直取 CP936 全量清扫 + 回归守卫）

- **车道**：`p15-cp936-sweep`
- **工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p15-cp936-sweep`
- **分支**：`par/p15-cp936-sweep`（base = main `96af2e29`）
- **切片提交**：
  - `ca775d80` 切片1：17 文件 31 处代码直取 936 → `GXX.Core.EncodingInit.GBK`（+3 处 RunGate 抽取命令行注释去字面量）
  - `522a0b06` 切片2：守卫测试 `Cp936DirectLookupGuardTests`
  - `HEAD`（切片3：本报告）

---

## 0. 结论（一句话）

`GXX.CSharp/src` 下**直取 CP936 的处数已从 35 降到 1**，唯一保留的一处就是定义本身
`src/GXX.Core/EncodingInit.cs:36`（`_gbk ??= Encoding.GetEncoding(936);`）——
即"注册者自己造实例"这条唯一合法的路径。其余 34 处全部改为 `GXX.Core.EncodingInit.GBK`
（其 getter 先 `Ensure()` 注册 `CodePagesEncodingProvider`，再返回同一个 CP936 编码实例），
**语义完全等价**，且彻底消除"调用方先于任何 GXX.Core 类型被触碰"导致的
`No data is available for encoding 936`（台账 §52.1 已实测过一次红门禁：`GatewayKit.Tests` 2 条 + `RunGate.Tests` 13 条）。

> **另有一条与本次改动无关、但会挡住所有车道的环境问题**：`tools/run-gate.ps1` 的**已提交**版本在本机
> （ACP=936 + 无 PowerShell 7）用 `powershell -File` **无法解析**；主工作树里已有未提交的 ASCII 化修复待集成。
> 详见 §5.1，本车道只报不改（禁改 `tools/**`）。

---

## 1. 为什么必须做（本工程实测过的雷）

`EncodingInit` 的注册写在 `[ModuleInitializer]` 里 ⇒ **只在 GXX.Core 模块被触碰时才跑**。
直接 `Encoding.GetEncoding(936)` 的调用点如果先跑，就绕过了注册：

| 事实 | 取证 |
| --- | --- |
| 注册在 GXX.Core 模块初始化 | `src/GXX.Core/EncodingInit.cs:14-15` `[ModuleInitializer] internal static void ModuleInit() => Ensure();` |
| `GBK` getter 每次先 `Ensure()` | `src/GXX.Core/EncodingInit.cs:33-37` |
| 已发生过的红门禁 | 台账 §52.1（`GatewayKit.Tests` 2 条 + `RunGate.Tests` 13 条，根因即加载顺序） |
| 今天能跑只因"恰好先触碰过 GXX.Core" | 进程内模块初始化顺序不可保证 ⇒ 是"迟早爆"的雷，不是"现在没事" |

---

## 2. 逐文件替换清单（前 → 后）

统一替换规则（**只换实例来源，用法一字不动**）：

| 前 | 后 |
| --- | --- |
| `Encoding.GetEncoding(936)` | `GXX.Core.EncodingInit.GBK` |
| `System.Text.Encoding.GetEncoding(936)` | `GXX.Core.EncodingInit.GBK` |

> 全部 14 个代码文件一律写**全名** `GXX.Core.EncodingInit.GBK`：这些文件多数没有
> `using GXX.Core;`（其中 `ClientModuleList.cs` / `Items.cs` / `ViewFormsData.cs` 连
> `System.Text` 都是全名写法），全名可零风险地对齐两种现状，不需要新增/调整 using。

行号列为「改动后行号」（每个文件的首处替换同时插入了 1 行说明注释，故其后行号 +1）。

### 2.1 代码处（31 处 / 14 个文件）

| # | 文件 | 处数 | 处（改动后行号） | 前 → 后（片段） |
| --- | --- | --- | --- | --- |
| 1 | `src/GXX.Core/Launcher/LauncherSettings.cs` | 2 | 187, 328 | `var gbk = Encoding.GetEncoding(936);` → `var gbk = GXX.Core.EncodingInit.GBK;`（L327 原样：`File.WriteAllText(path, sb.ToString(), …)`） |
| 2 | `src/GXX.M2Server/Engine/Boxs.cs` | 4 | 185, 198, 229, 358 | `File.WriteAllLines(M2Config.sBoxsFile, tempList, …)` / `File.ReadLines(sFileName, …)` / `File.ReadAllLines(sFileName, …)` / `File.WriteAllLines(sFileName, saveList, …)` |
| 3 | `src/GXX.M2Server/Engine/ClientModuleList.cs` | 6 | 58, 66, 81, 97, 117, 130 | `.GetBytes(text)` ×2（`var raw` / `var rawBytes`）、`File.WriteAllLines(…ModuleList.txt…)`、`File.WriteAllLines(path, saveList, …)`、`File.ReadAllLines(fileName, …)`、`File.ReadAllLines(path, …)` |
| 4 | `src/GXX.M2Server/Engine/CombatPower.cs` | 2 | 196, 313 | `File.WriteAllText(fileName, sb.ToString(), …)` / `File.WriteAllText(fileName, sb.ToString() + sbOthers, …)` |
| 5 | `src/GXX.M2Server/Engine/CustomHeroMagic.cs` | 1 | 437 | `File.WriteAllText(fileName, sb.ToString(), …)` |
| 6 | `src/GXX.M2Server/Engine/CustomNpcUtils.cs` | 1 | 272 | `File.WriteAllText(fileName, sb.ToString(), …)` |
| 7 | `src/GXX.M2Server/Engine/FilterTexts.cs` | 2 | 137, 162 | `File.ReadLines(sFileName, …)` / `File.WriteAllLines(sFileName, saveList, …)` |
| 8 | `src/GXX.M2Server/Engine/GroupItems.cs` | 4 | 139, 247, 279, 320 | `File.ReadLines(sFileName, …)` / `File.WriteAllLines(sFileName, saveList, …)` / `File.WriteAllLines(sFileName, lines, …)` / `File.ReadLines(path, …)` |
| 9 | `src/GXX.M2Server/Engine/ItemEffects.cs` | 2 | 135, 321 | `File.ReadLines(sFileName, …)` / `File.WriteAllLines(sFileName, saveList, …)` |
| 10 | `src/GXX.M2Server/Engine/Items.cs` | 1 | 253 | `File.WriteAllLines(System.IO.Path.Combine(…"ItemRuleList.txt"), saveList, System.Text.Encoding.GetEncoding(936));` |
| 11 | `src/GXX.M2Server/Engine/SkillPowerItemList.cs` | 1 | 120 | `File.WriteAllText(sFileName, sb.ToString(), …)` |
| 12 | `src/GXX.M2Server/Engine/SndaShop.cs` | 2 | 131, 283 | `File.WriteAllLines(Path.Combine(…"ShopItemList.txt"), saveList, …)` / `File.ReadLines(path, …)` |
| 13 | `src/GXX.M2Server/Engine/UserCmds.cs` | 2 | 40, 68 | `File.ReadLines(sFileName, …)` / `File.WriteAllLines(sFileName, saveList, …)` |
| 14 | `src/GXX.M2Server/Engine/ViewFormsData.cs` | 1 | 51 | `File.ReadAllLines(fileName, System.Text.Encoding.GetEncoding(936))` |

每个文件都已按派发要求插入 **1 行**同文说明注释（位于该文件首处替换之上）：

```csharp
// GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
```

### 2.2 RunGate 抽取命令行注释（3 处 / 3 个文件）—— **需集成方注意的一处判断**

这 3 个文件里 `GetEncoding(936)` 出现在**文件头"抽取/回读流程"溯源注释**里（记录当初用哪条 PowerShell 命令把
GBK 原文读出来），**不在任何可执行代码路径上**：

| # | 文件 | 改动后行 | 前 | 后 |
| --- | --- | --- | --- | --- |
| 15 | `src/GXX.RunGate/GateShareAddressUtils.cs` | 16（+17 说明） | `[Text.Encoding]::GetEncoding(936).GetString($b)` | `[Text.Encoding]::GetEncoding('GBK').GetString($b)` |
| 16 | `src/GXX.RunGate/GateShareMagicIntervalUtils.cs` | 8（+9 说明） | 同上 | 同上 |
| 17 | `src/GXX.RunGate/RunGateConfigLoader.cs` | 11（+12 说明） | `[Text.Encoding]::ReadAllText(…,[Text.Encoding]::GetEncoding(936))` | `…::GetEncoding('GBK'))` |

**处理理由（明确记录，便于集成方复核）**：派发书把"17 文件 35 处"列全，其中这 3 处按逐行 grep 计数；
而验收判据写的是"**剩余直取 936 的处数应为 1**"。若保留注释字面量，全仓字符串计数就是 4，与判据冲突，
且守卫测试（要求"除 `EncodingInit.cs` 外没有任何文件再出现 `GetEncoding(936)`"）必须为注释开豁免口子 —— 那会削弱守卫。
故此处把**等价写法** `GetEncoding('GBK')`（Windows PowerShell 与 .NET 8 + provider 均可解析，返回同一 CP936）写回注释，
并各自补 1 行说明"原写作数字代码页形式"。**溯源信息只降级不丢失**：注释内的说明行明确记录了这件事，抽取命令仍可直接执行。

---

## 3. 守卫测试（防回归）

**文件**：`GXX.CSharp/tests/GXX.Core.Tests/Cp936DirectLookupGuardTests.cs`（新增，命名满足派发要求的 `Cp936*.cs`）

### 3.1 判据

| 项 | 内容 |
| --- | --- |
| 扫描面 | 从 `AppContext.BaseDirectory` 逐级**上溯**找到含 `GXX.CSharp/src` 的那一级（**不写死盘符**，与 `GXX.RunGate.Tests/Rest9NotPortedEvidenceTests.cs`、`GXX.Client.Tests` 的既有写法一致），再扫 `*.cs` 全递归，排除路径段为 `bin`/`obj` 的文件 |
| 命中模式 | `GetEncoding\s*\(\s*936\s*\)`（比字面量更严：`GetEncoding( 936 )` 也抓） |
| 允许清单 | 仅 `EncodingInit.cs`（按文件名） |
| 断言 A | **违规清单必须为空**；失败时逐条打印 `GXX.CSharp/src/<相对路径>:<行号>: <行内容(Trim)>` |
| 断言 B | **扫描面非空取证**：`files.Count > 100`（否则路径定位失效时用例会"恒真通过"） |
| 断言 C（反向） | **全仓命中总数恰为 1** —— 防止 `EncodingInit.cs` 被改名/删除后守卫退化成空转 |
| 附加用例 | `EncodingInitGbkIsCp936`：`EncodingInit.GBK.CodePage == 936`（替代物本身不能是别的东西） |

### 3.2 失败信息样例（**实测**，非手写）

取证方法：临时在 `src/GXX.Core/Probe/P15Cp936ProbeTmp.cs` 放一个**可编译**的违规探针
（`internal static System.Text.Encoding Raw() => System.Text.Encoding.GetEncoding(936);`），
跑
`dotnet test GXX.CSharp/tests/GXX.Core.Tests/GXX.Core.Tests.csproj -c Debug --filter "FullyQualifiedName~Cp936DirectLookupGuardTests"`，
取走真实输出后**立即删除探针**（`git status --porcelain` 已复核：工作树内只剩报告这一项未跟踪文件；`GetEncoding(936)` 复核计数回到 1）。

```text
[xUnit.net 00:00:00.85]     GXX.Core.Tests.Cp936DirectLookupGuardTests.NoDirectCp936LookupOutsideEncodingInit [FAIL]
  失败 GXX.Core.Tests.Cp936DirectLookupGuardTests.NoDirectCp936LookupOutsideEncodingInit [522 ms]
  错误消息:
   GXX.CSharp/src 下除 EncodingInit.cs 外不得再直取 CP936（加载顺序依赖雷）：
GXX.CSharp/src/GXX.Core/Probe/P15Cp936ProbeTmp.cs:6: internal static System.Text.Encoding Raw() => System.Text.Encoding.GetEncoding(936);
     at GXX.Core.Tests.Cp936DirectLookupGuardTests.NoDirectCp936LookupOutsideEncodingInit() in ...\Cp936DirectLookupGuardTests.cs:line 82
失败!  - 失败:     1，通过:     1，已跳过:     0，总计:     2，持续时间: 526 ms - GXX.Core.Tests.dll (net8.0)
```

⇒ 违规**文件（仓库相对路径）+ 行号 + 整行内容**三要素齐全；探针删除后同一用例转绿
（见 §5：`GXX.Core.Tests` 1017/1017 通过，其中含本车道这 2 条）。

---

## 4. `EncodingInit.GBK` 线程安全核对（派发要求 #5）

现状（未改动，`src/GXX.Core/EncodingInit.cs`）：

```csharp
public static void Ensure()
{
    if (_done) return;                                      // _done 非 volatile
    try { Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); } catch { }
    _done = true;
}
public static Encoding GBK { get { Ensure(); return _gbk ??= Encoding.GetEncoding(936); } }
```

**结论：并发下可接受，无需改 Core 语义。** 逐条给出理由：

1. **不会出现"未注册就取 936"**（这才是唯一致命的失败模式）：`GBK` 的 getter 在 `GetEncoding(936)` **之前**先 `Ensure()`，
   所以**每个调用线程都会自己走一遍注册**（或观察到别人已注册完）。抢输的线程不会跳过注册 ⇒ 原事故的
   `No data is available for encoding 936` 在当前形状下不可能出现。
2. `Ensure()` 的竞争最坏后果是**重复注册同一个 provider**：`Encoding.RegisterProvider` 内部有锁且对同一实例幂等，
   重复登记只是往 provider 表里追加一次，无异常、无语义变化。
3. `_gbk ??=` 的竞争最坏后果是**建出两个 CP936 实例**：`Encoding` 实例构造后即不可变、文档保证成员线程安全，
   且 `Encoding.GetEncoding` 本身对同一代码页通常返回**同一个缓存实例**，故"丢一次更新"等价于没发生。
4. 可见性：`RegisterProvider` 与 `GetEncoding` 内部都走锁（含 acquire/release 屏障），
   `_done` 的读取即使是被"提前"看到为 `true`，后续 `GetEncoding(936)` 的加锁也必然同步到 provider 登记结果。
5. **若将来要更严**（派发要求：只报告、不改）：最小改动是把 `_done` 换 `volatile`、或把 `_gbk` 改成
   `Lazy<Encoding>(LazyThreadSafetyMode.ExecutionAndPublication)`；收益仅是"最多省一次冗余注册/一个冗余实例"，
   **不改变任何可观测语义**，因此本轮不做。

---

## 5. 门禁（`run-gate.ps1`，三项同时成立才算绿）

命令：

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p15-cp936-sweep
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1
```

### 5.1 ⚠ 先决问题：派发书给的那条命令在本机**连解析都过不去**（非本车道引入，但会挡住所有车道）

实测（在 worktree 根原样执行）：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1
```

```text
At ...\GXX.CSharp\tools\run-gate.ps1:46 char:6
+     'The active test run was aborted',
+      ~~~
Unexpected token 'The' in expression or statement.
At ...\run-gate.ps1:87 char:74 ... The string is missing the terminator: '.
At ...\run-gate.ps1:92 char:2 ... Missing closing ')' in subexpression.
[exit code: 1]
```

**根因（逐项定量取证，不是猜测）**：

| 事实 | 取证 |
| --- | --- |
| 脚本**无 UTF-8 BOM** | `[IO.File]::ReadAllBytes` 首 3 字节 = `23 20 3D`（`# =`），非 `EF BB BF` |
| 脚本含 **96 个非 ASCII 字节** | 第 44/45 行的中文崩溃标记 `测试主机进程` / `测试运行已中止` |
| 本机 ANSI 代码页 = **936** | `[Text.Encoding]::Default.WebName` = `gb2312` |
| 本会话**没有 PowerShell 7** | `Get-Command pwsh` = False；宿主 = `Windows PowerShell 5.1.19041.6693` |
| 崩溃机理 | 第 45 行 UTF-8 共 21 字节（奇数），末字节 `A2` 与结束引号 `'`(0x27) 组成合法 GBK 字 `A227` ⇒ **吞掉字符串结束引号** ⇒ 后续崩 |
| 与脚本自身声明冲突 | 脚本第 2 行写着 `(ASCII-only: Windows PowerShell 5.1 reads .ps1 as ANSI)` —— 它**本应纯 ASCII**，是两个中文标记破坏了前提 |
| **不是本车道引入** | `git status --porcelain` 对 `tools/run-gate.ps1` 为空 ⇒ 我的副本与 main **已提交**内容逐字节相同（SHA256 `A2892672BB53C281…`）；且本车道**被禁止改 `tools/**`**，故未修 |

**主工作树里其实已有未提交的修复**（只读核对，本车道未触碰 main 任何文件）：
`git -C <主工作树> status --porcelain` → ` M GXX.CSharp/tools/run-gate.ps1`；该工作副本 **0 个非 ASCII 字节**
（中文标记改成运行时按码点拼装：`[char]0x6D4B …`），GBK 解码下 `parseErrors = 0`（SHA256 `6FEFADBBFE26DCC1…`）。
⇒ **请集成方把这份修复提交**（或给 `run-gate.ps1` 加 UTF-8 BOM）：否则任何人在 ACP=936 的机器上照派发书执行 `powershell -File`
都会拿到**与代码无关的红**。

### 5.2 本车道实际执行的等价方式（只改解码，脚本正文逐字节未改）

把 `tools/run-gate.ps1` **原文**（UTF-8 解码）写到 `%TEMP%\p15-run-gate-bom.ps1` 并加上 UTF-8 BOM，
再用 PS 5.1 以 `-File` 运行，并显式传 `-Repo`（脚本默认的 `$PSScriptRoot\..\..` 指向 temp 目录）：

```powershell
$src = '<worktree>\GXX.CSharp\tools\run-gate.ps1'
$txt = [IO.File]::ReadAllText($src, [Text.Encoding]::UTF8)
[IO.File]::WriteAllText("$env:TEMP\p15-run-gate-bom.ps1", $txt, [System.Text.UTF8Encoding]::new($true))
powershell -NoProfile -ExecutionPolicy Bypass -File "$env:TEMP\p15-run-gate-bom.ps1" -Repo '<worktree>'
```

> 该 temp 副本是**逐字节复制 + BOM**（`Get-FileHash` 前后一致性已核）；三条判据、`$CRASH_MARKERS`、
> `exit` 码传递全部原样生效。temp 文件落在 `%TEMP%`，**不在仓库内**。

### 5.3 `gate evidence`（脚本原样输出）

```text
== build: GXX.CSharp/GXX.slnx ==
    182 个警告
    0 个错误
已用时间 00:00:15.90
== test: GXX.CSharp/GXX.slnx ==
已通过! - 失败:     0，通过:   122 ... GXX.GatewayKit.Tests.dll
已通过! - 失败:     0，通过:   163 ... GXX.SelGate.Tests.dll
已通过! - 失败:     0，通过:  1017 ... GXX.Core.Tests.dll
已通过! - 失败:     0，通过:   219 ... GXX.GameCenter.Tests.dll
已通过! - 失败:     0，通过:   142 ... GXX.LogDataServer.Tests.dll
已通过! - 失败:     0，通过:   265 ... GXX.LoginSrv.Tests.dll
已通过! - 失败:     0，通过:   838 ... GXX.DBServer.Tests.dll
已通过! - 失败:     0，通过:  4904 ... GXX.Client.Tests.dll
已通过! - 失败:     0，通过:  1922 ... GXX.RunGate.Tests.dll
已通过! - 失败:     0，通过:     2 ... GXX.Integration.Tests.dll
已通过! - 失败:     0，通过: 10283 ... GXX.M2Server.Tests.dll

== gate evidence ==
dotnet test exit code : 0
crash markers found   : none

GATE: PASS (build 0 error, test exit 0, no crash markers)
```

```text
EXITCODE=0
```

三行判据逐条对上：**build 0 错误**（182 警告全部为既有 xUnit 分析器警告）、**dotnet test exit code 0**、
**crash markers = none**（无 `Stack overflow` / `testhost` / `测试主机进程` / `The active test run was aborted` 等）。
11 个测试程序集合计 **19,877 通过 / 0 失败 / 0 跳过**（其中 `GXX.Core.Tests` 1017 通过 = 本车道新增 2 条已计入），
并且**只有 `GATE: PASS` 与 `exit 0` 同时出现才算绿**（台账 §52.3 的假绿坑：摘要行 `已通过!` 单独出现不算）。

> 本报告定稿前共跑 **2 次**完整门禁（§3.2 探针实验**之前**一次、探针清理**之后**再一次），
> 两次结论完全一致：`exit 0` / `crash markers none` / `GATE: PASS` / 19,877 通过 0 失败。
> 上表为**清理后（= 交付态）**那一次的输出。

---

## 6. 剩余直取 936 的处数

| 范围 | 改动前 | 改动后 |
| --- | --- | --- |
| `GXX.CSharp/src/**/*.cs`（含注释） | **35** | **1** |
| 其中代码路径 | 31 | 0 |
| 其中 RunGate 溯源注释 | 3 | 0（改为等价 `GetEncoding('GBK')`） |
| 唯一保留 | — | `src/GXX.Core/EncodingInit.cs:36` `_gbk ??= Encoding.GetEncoding(936);`（**定义本身，唯一合法**） |
| `GXX.CSharp/tests/**` | 0 | 0（本车道守卫测试用正则写法 `GetEncoding\s*\(\s*936\s*\)`，不含该字面量） |

复核命令（只读）：

```powershell
Select-String -Path <worktree>\GXX.CSharp\src\**\*.cs -Pattern 'GetEncoding\(936\)'
# ⇒ 恰好 1 条：src\GXX.Core\EncodingInit.cs:36
```

---

## 7. 未完成 / 阻塞 / 遗留判断

- **未完成**：无。17 个文件全部改完，守卫测试已加，门禁见 §5。
- **⚠ 需集成方立刻处理（影响所有车道，非本车道范围）**：§5.1 —— `tools/run-gate.ps1` 的**已提交**版本无 BOM 且含 96 个非 ASCII 字节，
  在 ACP=936 的机器上用 `powershell -File` **连解析都过不去**（本车道实测 3 个 parse error）；主工作树里已存在**未提交**的
  ASCII 化修复，**需要 commit**（或给文件加 UTF-8 BOM）。本车道禁止改 `tools/**`，故只报不改。
- **需集成方裁决的 1 项**：§2.2 的 3 处**溯源注释**改写（`GetEncoding(936)` → `GetEncoding('GBK')`）。
  若集成方认为"注释里的历史命令行必须原样保留"，可把守卫改为"忽略整行注释"，并把本报告 §6 的
  "剩余 1 处"改述为"代码路径 1 处 + 注释 3 处"；本车道选择的是"全仓字符串计数严格为 1"这一更可机检的口径。
- **未触碰**：`EncodingInit.cs`（定义）、`GXX.slnx`、所有 `*.csproj`、`Directory.Build.props`、
  `docs/Checklist.md`、`docs/并行派发台账.md`、`docs/并行覆盖审计.md`、`tools/**` —— 一行未动。
- **不在本车道范围但同源的风险**（提示，不代改）：`PakSeams.cs` / `UnitDes.cs` 里还有
  `Encoding.GetEncoding(28591)`（Latin1）。**28591 是 .NET 内置代码页，不需要 provider 注册**，
  故**没有**同类的加载顺序风险，本轮不动。

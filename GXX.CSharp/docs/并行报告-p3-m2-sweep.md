# 并行报告 · 车道 `p3-m2-sweep`（M2Engine 零散小单元清扫）

> 工作树：`.worktrees/p3-m2-sweep`，分支 `par/p3-m2-sweep`
> 报告时基线：已 **rebase 到当前 main `3c7a8ab7`（批次 J217）**，`main...HEAD = 0 / 4`
> 独占区：`src/GXX.M2Server/Sweep/**`、`tests/GXX.M2Server.Tests/Sweep*`、本报告

---

## 1. 全部 commit hash

rebase 到 main 后 hash **已改写**（旧 hash 作废）：

| # | commit | 内容 |
|---|---|---|
| 1 | `1a46cfff` | 固化上一轮遗留产出 `Nations`/`M2Locker`/`SweepSeams` + 测试（含修正 `m_btNation` 类型） |
| 2 | `d469e380` | 移植 `PowerBase64.pas`（可变码表 Base64 + XOR 混淆） |
| 3 | `528c6be9` | 修正 `SweepNations` 测试的环境依赖（Envir 路径硬编码 → 从 `sEnvirDir` 派生） |
| 4 | `419e658b` | `MemoryModuleEx.pas` 按 §2.3 只做 Stub |

**工作树干净（`git status --porcelain` 为空），无 `WIP-不可合并` 存档提交，HEAD 即全绿状态。**

---

## 2. ★ 遗留产出甄别结论（5 个文件各自）

上一轮本车道被宿主重启杀死、产出未提交。逐文件通读后判定如下 —— **全部「保留 + 补齐」，无一重写**：

| 文件 | 判定 | 理由（逐条核对原文后的结论） |
|---|---|---|
| `Sweep/Nations.cs` | **保留 + 修正 1 处** | 通读并与 `Nations.pas` 逐段比对：结构、方法名、分支顺序、缺陷注释**均正确**，可直接续用。<br>**唯一实质缺陷**：`INationsPlayObject.m_btNation` 被建模成 `byte`，而原文是 **`Word`**（`ObjBase.pas:408`）。此错会让 1000 号国家的成员被静默塞进 1 号列表、`SendNationMsg(1000,…)` 永远发不出去且**无任何报错**。已改为 `ushort` 并加入差异断言用例 `MaxNationSlot_MemberRouting_WordNotByte` 钉死。 |
| `Sweep/M2Locker.cs` | **保留不动** | 逐段比对 `M2Locker.pas`：`USE_SPINLOCK` 生效分支、三份不对称的 `UnLockR`、写反的 `BeginWrite` 守卫（:354）、两处被注释的 `TryLock`、非原子 `EndWrite` —— 全部 1:1 且注释到位。未改一行。 |
| `Sweep/SweepSeams.cs` | **保留不动** | 接缝设计正确（复用既有 `TFastIniFile`/`DelphiRTL`，未复制第二份实现）；`RenameFile` 显式判存以对齐 Win32 `MoveFile` 不覆盖语义、`TIniFile` 写穿 → `Dispose` 落盘（仅脏时）—— 两处细节都对。 |
| `tests/SweepNationsTests.cs` | **补齐（3 处编译/断言错误 + 2 处新增）** | ① `(ushort)70000` / `(byte)300` 常量转换 **编译不过**（CS0221），已改 `unchecked(...)`；② 一处**恒真**占位断言 `Assert.Equal(33, x == null ? 0 : 33)` 已换成真断言；③ `SendNationMsg` 的 MAX 槽用例**期望写错**（断言某玩家收到消息，但该玩家从未 `AddMember`）；④ 新增 `MaxNationSlot_MemberRouting_WordNotByte`（Word/Byte 差异）；⑤ 新增 `Env_SurvivesPollutedEnvirDir_LeftByOtherTestClasses`（环境残留回归，见 §6）。<br>另：`CompareText` 差异断言的**理由写错了** —— 原注释称 `.NET OrdinalIgnoreCase` 会把 Kelvin 记号 `U+212A` 折叠成 `k`，**实测为 false**；真正会误折叠的是**区域敏感**比较（`CurrentCulture`/`InvariantCulture`）。已订正注释与交叉验证断言。 |
| `tests/SweepTestKit.cs` | **保留 + 修正 2 处** | ① `FakeNationPlayer.m_btNation` 同步改 `ushort`；② 可控时钟默认值由「恒 0」改为「每次自增 1」—— **恒定时钟会让自旋锁测试永久挂死**（见 §6 事故记录），自增让超时分支确定性触发。 |

**结论：上一轮产出质量高（尤其 `M2Locker.cs` 与接缝层几乎零改动），确实「能续作」；本轮以补齐为主，未推倒重来。**

---

## 3. 逐单元判定表

| 优先 | 源单元 | 行数(实测) | 判定 | 证据 |
|---|---|---|---|---|
| 1 | `Nations.pas` | 400 | **已完成** 1:1 | `Sweep/Nations.cs:1-495`；`TNationManage` 全 14 个成员 + `TNationInfo` |
| 2 | `PowerBase64.pas` | 397 | **已完成** 1:1（与既有 Base64 比对后**判定不可互替** → 单独移植） | `Sweep/PowerBase64.cs:1-586` |
| 4 | `MemoryModuleEx.pas` | 815 | **Stub（§2.3）** | `Sweep/MemoryModuleEx.cs:1-109`；接口段 3 函数签名全保留，全部成员抛 `NotSupportedException` |
| 8 | `M2Locker.pas` | 470 | **已完成** 1:1 | `Sweep/M2Locker.cs:1-459`；`TSafeList`/`TSafeStringList`/`TM2CriticalSection`/6 个自由函数 |
| 3 | `ItemEvent.pas` | 389 | **未开工** | 见 §8 未完成说明 |
| 5 | `GameGoldDealDB.pas` | 494 | **未开工** | 同上 |
| 6 | `HTTPService.pas` | 278 | **未开工** | 同上 |
| 7 | `PathFindClient.pas`/`PathFind_Hero.pas` | — | **未开工** | 同上 |
| 9 | `ObjDummy`/`ObjRobot`/`ObjGuard`/`ObjFireDragon`/`UserShopDB_Old` | ≈3,440 | **未开工** | 同上 |
| — | 窗体系 / 零散小单元族 | ≈2,400 | **未开工** | 同上 |

**未碰已被顺序会话做完的单元**：动手前已 grep `src/` 确认 `MySqlStorageDB`/`SqliteStorageDB` 等已有实现，未重复派工。

---

## 4. 新增文件 + 每单元已覆盖/未覆盖行号范围

| 文件 | 行数 | 覆盖的原文行号 | 未覆盖 |
|---|---|---|---|
| `Sweep/Nations.cs` | 495 | `Nations.pas:1-397`（**全单元**：声明 8-33、Create 40-65、Destroy 67-76、GetNationInfo 78-94、GetNationIndex 96-112、GetNationName 114-123、AddMember 125-140、DeleteMember 142-157、IsMember 159-175、SendNationMsg 177-213、Get 215-221、SaveConfig(btNation) 223-262、SaveConfig() 264-303、LoadConfig 305-353、RenameNationName 355-397） | **无**（`TNationInfo` record 定义在 `M2Definition.pas:456-473`，已一并移植） |
| `Sweep/M2Locker.cs` | 459 | `M2Locker.pas:1-470`（声明 8-83、`TSafeList` 95-171、`TSafeStringList` 173-265、自由函数 267-374、`TM2CriticalSection` 376-468） | **`:1-7`/`:84-94` 条件编译与外部导入声明**（`{$DEFINE USE_SPINLOCK}` 分支已移植；非 `USE_SPINLOCK` 的 `TCriticalSection` 分支与 `InterlockedCompareExchange` 32 位导入按托管平台无意义，已在文件头登记）。**`:199-209`/`:402-412` 两处 `TryLock` 原文即被 `(* *)` 注释**，以注释形式保留 |
| `Sweep/PowerBase64.cs` | 586 | `PowerBase64.pas:1-397`（**全单元**：接口 13-27、`EncodeTable` 32-34、`DEF_FILL_CHAR` 35、`DecodeTable` 38、`XOR_encrypt` 40-59、`XOR_decrypt` 61-64、`EncodeBase64` 66-151、`DecodeBase64` 153-275、`EncryptAndEncodeBase64` 277-295、`DecryptAndiDecodeBase64` 297-309、`EncryptAndEncodeBase64String` 311-325、`DecryptAndDecodeBase64String` 327-348、`EncodeBase64String` 350-356、`DecodeBase64String` 358-368、`InitDecodeTable` 370-387、`initialization` 389-390） | **无** |
| `Sweep/MemoryModuleEx.cs` | 109 | `MemoryModuleEx.pas:1-72`（**接口段全部**：`TMemoryModule` 61、`MemoryLoadLibrary` 68、`MemoryGetProcAddress` 70、`MemoryFreeLibrary` 72） | `:74-815` **implementation 段原生 PE 加载实现** —— 按 §2.3 显式登记为不移植项（理由见文件头 40 行内） |
| `Sweep/SweepSeams.cs` | 178 | 接缝层（非单元移植）：服务 `Nations.pas`/`M2Locker.pas`/`GameGoldDealDB.pas`/`HTTPService.pas` 的 INI/文件系统/日志/计时依赖 | — |
| `tests/SweepTestKit.cs` | 242 | 测试基础设施（内存 FS + 内存 INI + 假 `TPlayObject` + 接缝安装/还原） | — |
| `tests/SweepNationsTests.cs` | 597 | `Nations.cs` 全部公开成员 | — |
| `tests/SweepM2LockerTests.cs` | 522 | `M2Locker.cs` 全部公开成员 | — |
| `tests/SweepPowerBase64Tests.cs` | 497 | `PowerBase64.cs` 全部公开成员 | — |
| `tests/SweepMemoryModuleExTests.cs` | 107 | Stub 的签名保留 + 显式失败语义 | — |

**大段常量/表全部脚本从原文抽取 + 回读比对，无手工转录**：
```powershell
# EncodeTable：从 PowerBase64.pas:32-34 抽取 -> 64 项
$raw  = ((Get-Content Source\M2Engine\PowerBase64.pas -Encoding Default)[31..33] -join ' ')
$tbl  = -join ([regex]::Matches($raw, "'(.)'") | % { $_.Groups[1].Value })
# => COUNT=64 / LEN=64 / TABLE=[456789#/opqrstuvQRSTUVWXYZabcdefghijklmnABCDEFGHIJKLMNOPwxyz0123]
```
`CIPHER_INDEX`/`BASE_CIPHER`（各 16 项）同样抽取后回读比对，并在测试侧**独立复算**一次（`SweepPowerBase64Tests` 的 `CipherIndex`/`BaseCipher` 常量）。

---

## 5. 测试用例数 + build / test 结果

| 测试类 | 用例数 |
|---|---|
| `SweepNationsTests` | 24 |
| `SweepM2LockerTests` | 34 |
| `SweepPowerBase64Tests` | 25 |
| `SweepMemoryModuleExTests` | 8 |
| **合计（本车道新增）** | **91** |

**门禁（rebase 到 main `3c7a8ab7` 之后整工程重跑）：**
```
cd .worktrees/p3-m2-sweep/GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
  -> Build succeeded.  0 Warning(s) / 0 Error(s)
dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
  -> Passed!  - Failed: 0, Passed: 7316, Skipped: 0, Total: 7316
```
**0 失败 / 7316 通过。**（注：派发单给的基线「5591 例」是旧口径；rebase 后 main 已长到 7316，说明基线在持续漂移 —— 本车道**未新增任何失败**。）

> 本报告只跑了 `GXX.M2Server.Tests`（派发单指定）。全量 11 个测试工程的只读验证由调度方的波次门禁完成。

---

## 6. 发现的原文缺陷 / 易错点（带 `文件:行`）

### 6.1 原文缺陷（按原文逐字保留 + 注释 + 差异断言）

| # | 位置 | 现象 | 处置 |
|---|---|---|---|
| 1 | `Nations.pas:198` | `if PlayObject.m_boBanNationChat then` —— 只有**被禁言者**才收到国家消息，语义可疑（多半应为 `not`） | 逐字保留；`SendNationMsg_OnlyBanNationChatReceivers_OriginalInvertedLogic` 差异断言 |
| 2 | `Nations.pas:328` | `Inc(FCount)` 只在文件存在时递增且**不重置** → 重复 `LoadConfig` 累加 | 逐字保留；`LoadConfig_RepeatedCall_AccumulatesCount_OriginalDefect` |
| 3 | `Nations.pas:365` | `if GetNationIndex(NewName) > 0 then Exit` → 改名为**同名**也直接 Exit | 逐字保留；`RenameNationName_RejectsExistingOrSameName` |
| 4 | `Nations.pas:238/280` | 键名不对称：写入 `'nRedHomeX'`/`'RedHomeY'`，读取 `'nRedHomeX'`/`'RedHomeY'` | 逐字保留 |
| 5 | `M2Locker.pas:354` | `if not Result then` —— `BeginWrite`「等待所有读取」守卫**写反**：第一段自旋成功时 `Result=True`，第二段被整段跳过 → **写者不等读者** | 逐字保留；`BeginWrite_DoesNotWaitForActiveReader_OriginalInvertedGuard` |
| 6 | `M2Locker.pas:138-146` | `TSafeList.UnLockR` **不清 `FLockerID`**，而 `TSafeStringList:228-237`、`TM2CriticalSection:431-440` 的**同名方法都清** → 三份实现不对称 | 逐字保留；两个用例分别钉死两侧 |
| 7 | `M2Locker.pas:131` | `TSafeList.LockR` 死锁分支的 `MainOutMessage` **整行被注释掉**（静默失败） | 以注释形式保留；`SafeList_LockR_DeadlockPath_LogsNothing_OriginalCommentedOut` |
| 8 | `M2Locker.pas:369-372` | `EndWrite` 是**非原子**直接赋值 `Target := 0`（不是 `InterlockedExchange`） | 逐字保留 |
| 9 | `PowerBase64.pas:314/341` | `CurrentReference := Target and $FFFFFFFC`，而 CAS 比较的是**未掩码**的 `Target` ⇒ 只要已有读者持锁（`Target=2`），第二位读者的 `CAS(Target,2,0)` **永不成功** → **读锁实际不可重入**（自旋到 1000ms 超时） | 逐字保留；`BeginRead_SecondConcurrentReader_TimesOut_ReadLockNotReentrant`。**这是本次最有价值的发现**：同一根因也作用于 `TSafeList`/`TSafeStringList`/`TM2CriticalSection` 三处 |
| 10 | `PowerBase64.pas:180` | `else if strBase64[nStrSize] = DEF_FILL_CHAR` —— Delphi 1-based 下标下 `nStrSize` 与 `:176` 的 `nStrSize-1` 是**同一个字符** ⇒ `nFillLen := 1` 分支**永远不可达**（死分支） | **以注释形式保留**（写成可达形式会改变行为）；`DecodeBase64_FillLenOneBranchIsDead_OriginalDefect` 证明其不可达 |
| 11 | `PowerBase64.pas:191-193` | `nCycle := (nStrSize - nFillLen) div 4; nBufLen := nCycle*3 - nFillLen` —— `nFillLen` 被**减了两次** | 逐字保留；`DecodeBase64_ProbeLength_...` 把实际数值逐一钉死 |
| 12 | `PowerBase64.pas:308` | `XOR_decrypt(pData, nDataSize)` 用**传入的缓冲区长度**（而非实际解码长度）做异或 ⇒ 缓冲区比解码长度长时尾部被**多异或一次**，原文无法还原 | 逐字保留；`DecryptAndiDecodeBase64_XorLengthUsesBufferSize_TailOverXored_OriginalDefect` |
| 13 | `PowerBase64.pas:95/117/135` | `nData = PUINT(pData)^` 在不足 4 字节时**越界读**（读进相邻内存），随后用 `$00FFFFFF`/`$FF`/`$FFFF` 掩码丢掉高位 ⇒ 越界字节**不影响结果** | 托管侧用「读不足补 0」等价实现（**不复制越界行为**），语义一致 |
| 14 | `PowerBase64.pas:88` | **初读时以为**「`SetLength` 多算了 `nFillLen` 字节、尾部留 `#0`」—— **实测证伪**：长度公式与实际写入量**始终一致** | 已在文件头登记为「已核查、无缺陷」，并用 7 个长度的**逐字符期望串**钉死 |

### 6.2 原文缺陷的**端到端后果**（原文字节语义陷阱）

`PowerBase64` 的**编码器与解码器对 len ≡ 2 (mod 3) 的输入无法往返**：
`EncodeBase64` 对 2/5/8… 字节输入在**末位写单个 `'+'`**，而 `DecodeBase64` 因 §6.1-10 恒判 `nFillLen=2`
→ `(n-2) % 4 ≠ 0` → 直接返回 0。即**该单元解不开自己产出的三分之一长度类**。
（`EncryptAndEncodeBase64_RoundTrip_FailsForLenMod3Equals2_OriginalDefect` 用 5 字节输入钉死，并对照 4/6 字节可正常往返。）

### 6.3 本轮踩到的**环境/工具**易错点（非原文缺陷，但会打挂别人）

| # | 现象 | 根因 | 处置 |
|---|---|---|---|
| A | **测试进程永久挂死**（需 `--blame-hang` 才能定位） | `M2Locker` 的 4 个自旋函数都是「CAS 失败 → `Sleep(0)` → 查超时」的**无界**循环；测试用「恒定时钟」（`MyGetTickCount` 恒 0）时超时永不推进 → 死循环。<br>触发点：`BeginRead_PreservesUpperBits` 里 `t = 4 \| 1 = 5` 时 `(5 & 0xFFFFFFFC) = 4 ≠ 5`，CAS 永不成功 | ①`SweepTestEnv` 时钟默认改为**每次自增 1**（超时 1000/3000 次后确定性触发）；②新增 `FreezeClock()` 仅用于「CAS 一次成功」的常规路径；③改为 `--blame-hang` 定位 |
| B | **集成分支上 4 例失败**（`LoadConfig_MissingKeys`/`LoadConfig_RepeatedCall`/`Ctor_CountZero`/`MaxNationSlot`） | `M2Config.sEnvirDir` 是**进程级静态全局**；`FormGeneralConfigTests.cs:488` 在**用例体内**把它改成 `"D:\Mir\Envir\"` 且**不还原**。本工程并行已关闭、测试按类顺序执行 → `SweepNationsTests` 在其后运行时，**我 Seed 的硬编码路径**与**实现算出的路径**错位 → `LoadConfig` 找不到文件 → 连锁 NRE/断言失败。<br>**实现侧（`Nations.pas:342/346` 用 `g_Config.sEnvirDir` 拼路径）无误。** | **纯测试侧修复**：①`SweepTestEnv` 构造时把 `sEnvirDir` 钉回出厂默认 `".\Envir\"`、`Dispose` 还原；②`NationFile`/`NationsIni`/`NationsDir` 改为**从 `M2Config.sEnvirDir` 派生**；③新增回归用例 `Env_SurvivesPollutedEnvirDir_LeftByOtherTestClasses` |
| C | 用 PowerShell `-replace` + `Set-Content -Encoding UTF8` 改 `.cs` 文件会**按 ANSI 读入**，把中文注释写成乱码并吞掉换行 | PS 5.1 的 `Get-Content` 不按 UTF-8 读 | **禁止用 PS 管道改含中文的源文件**；一律用 `edit`/`write` 工具。本轮一次踩中即回滚重写 |
| D | `(ushort)70000` / `(byte)300` 编译不过（CS0221） | C# 常量转换在编译期做溢出检查 | 用 `unchecked(...)`，并**保留**其语义（原文 `(Word)ReadInteger` 截断） |

---

## 7. 接缝清单 + 需调度方协调的事项

### 7.1 本车道建立的接缝（后续车道**复用，不得另造**）

| 接缝 | 位置 | 服务的原文依赖 | 默认实现 / 状态 |
|---|---|---|---|
| `ISweepIniFile` + `SweepFastIniFile` | `SweepSeams.cs:34-92` | `IniFiles.TIniFile`（`Create/ReadString/ReadInteger/WriteString/WriteInteger/Free`） | 包装既有 `GXX.Core.Util.TFastIniFile`；「析构即落盘」仅脏时落盘，对齐 `TIniFile` 写穿语义 |
| `SweepSeam.CreateIniFile` | `SweepSeams.cs:101` | `TIniFile.Create(FileName)` | **接缝：待 `TIniFile` 移植后接入** |
| `SweepSeam.FileExists` / `DirectoryExists` / `ForceDirectories` / `RenameFile` | `SweepSeams.cs:104-133` | `SysUtils` 同名函数 | BCL `File`/`Directory`；`RenameFile` **显式判存**以对齐 Win32 `MoveFile` 不覆盖语义 |
| `SweepSeam.MainOutMessage` | `SweepSeams.cs:139` | `M2Share.MainOutMessage` | 落入 `LoggedMessages` 观察列表；**接缝：待 `M2Share.pas` 日志移植后接入** |
| `SweepSeam.MyGetTickCount` | `SweepSeams.cs:148` | `M2Share.MyGetTickCount`（`M2Share.pas:3589`，`timeGetTime`） | `DelphiRTL.GetTickCount`；**接缝：待 `M2Share.pas` 移植后接入** |
| `INationsPlayObject` | `Nations.cs:65-83` | `TPlayObject` 的国战成员（`m_btNation`/`m_sNationaName`/`m_boBanNationChat`/`SendMsg`） | **接缝：待 `ObjPlayer.pas` 的 `TPlayObject` 补齐这三个字段后，让其实现本接口（或写 5 行适配器）** |
| `VmpStub` | `PowerBase64.cs:76-92` | `VMProtectSDK`（`{$IFDEF USE_VMP}`） | 空实现（无数据语义）；按 §2.3 属不移植项 |
| `MemoryModuleEx`（整体） | `MemoryModuleEx.cs:65-109` | 原生 PE 内存加载器 | **Stub**：签名保留、全部抛 `NotSupportedException`、`IsSupported=false` |

⚠ **`SweepSeams.cs` 只有一份，未在任何地方复制第二份。** 本车道所有实现**复用** `GXX.Core`（`HUtil32`/`TStringList`/`DelphiRTL`/`TFastIniFile`）与 `GXX.M2Server` 既有类型（`M2Config`、`Grobal2Const`、`Engine.TCreature.SendMsg`），未新造并行版本。

### 7.2 需调度方协调的事项

1. **🔴 `FormGeneralConfigTests.cs:488` 是跨测试类污染源，建议单独派一条小车道修。**
   它在 `[Fact]` 体内把 `M2Config.sEnvirDir` 设成 `"D:\Mir\Envir\"` 且不还原，会随机打挂**任何**「按 `sEnvirDir` 拼路径」的车道测试（本车道即为受害者，集成分支上真实失败 4 例）。
   同类未还原赋值共 **9 处**：`CombatPowerTests:21`、`CombatPowerSettingTests:22`、`CustomHeroMagicTests:21`、`CustomNpcUtilsTests:22`、`CustomNpcFormTests:23`、`HeroMagicFormsTests:22`、`FormJ56Tests:19`、`FormJ57Tests:22`、`FormGeneralConfigTests:488`。
   建议统一改为「保存 → `try/finally` 还原」（或提供 `TempEnvirScope` 之类的共享测试工具）。**本车道只把自己解耦了，未改别人的文件。**
2. `MemoryModuleEx` 归属确认：本车道按 §2.3 只做 Stub。若后续有车道要真正实现「从内存加载插件」，应走 `AssemblyLoadContext`（见 `MemoryModuleEx.cs` 文件头登记的托管替代路径），**不要复用本 Stub**。
3. `PowerBase64` 的调用方（若将来移植到用它的单元）需知悉 §6.2 的**编解码不可往返**限制（len ≡ 2 mod 3），否则会误判为移植 bug。
4. **注意 `TItemObject`/`TItemManager`（`ItemEvent.pas`）与既有 `TVisibleMapItem`（`VisibleItemLifecycleCore.cs`）不是同一批类型**（前者属 `ItemEvent.pas`，后者属 `ObjBase`/`ObjPlayer`），后续派工时不要合并或误认为重复。

---

## 8. 诚实说明：未完成部分与剩余量

### 8.1 本轮**已完成**（4 个单元，全部落库 + 门禁全绿）

`Nations.pas`(400) 、`M2Locker.pas`(470) 、`PowerBase64.pas`(397) 、`MemoryModuleEx.pas`(Stub) —— 合计覆盖原文约 **1,267 行实现 + 815 行按 §2.3 显式 Stub**，新增 **91 个测试用例**。

### 8.2 本轮**未完成**（如实登记，未留半成品在工作树里）

| 优先 | 单元 | 行数 | 状态 | 说明 |
|---|---|---|---|---|
| 3 | `ItemEvent.pas` | 389 | **未开工** | 已完成依赖勘察（见 §8.3），实现与测试均未写 |
| 5 | `GameGoldDealDB.pas` | 494 | **未开工** | — |
| 6 | `HTTPService.pas` | 278 | **未开工** | — |
| 7 | `PathFindClient.pas` / `PathFind_Hero.pas` | 711 / 609 | **未开工** | 含「先与 `GXX.Core/Util/PathFind.cs` 比对」的前置步骤，未做 |
| 9 | `ObjDummy`/`ObjRobot`/`ObjGuard`/`ObjFireDragon`/`UserShopDB_Old` | ≈3,440 | **未开工** | — |
| — | 窗体系（`ConfigMerchant`/`ViewHeroRcd`/`uAliyunSendSMSThread`） | ≈1,280 | **未开工** | — |
| — | 零散小单元族（`DataManage`/`M2Threads`/`NoticeM`/`StruckDamageAbsorbUtils`/`StringListHelper`/`ClientPickItemsCfg`/`ViewKernelInfo`/`ConfigMonGen`） | ≈1,130 | **未开工** | — |

**剩余量合计约 8,331 行**（含 `PowerBase64` 之外的 P2–P9 全部目标）。按本轮 4 个单元的节奏（约 1,267 行实现 + 91 例测试），**剩余工作量约为本轮的 6.5 倍**，一条车道一轮做不完。

### 8.3 已完成的勘察（供后续接手者直接续作，不浪费）

`ItemEvent.pas` 依赖勘察结论（已 grep 确认）：
- ✅ **已有、可直接复用**：`Engine/Envir.cs` 的 `TEnvirnoment`、`Engine/AddAbility.cs:64` 的 `TUserItem`、`M2Config.dwClearDropOnFloorItemTime`/`M2Config.dwFloorItemCanPickUpTime`（`Engine/M2Config.GameMsgTime.cs:73-74`）。
- ❌ **需建接缝**：`TGList`（无移植）、`TGameObject`（无移植，`TItemObject` 的基类）、`UserEngine.GetStdItem`、`AddGameDataLog`（已有 `DbLayer/DbLayerSeams.cs` 与 `DbLayer/M2DataDbSupport.cs:19` 可参照）。
- ✅ **判定**：`TItemObject.Run` 的**决策/状态部分**（幽灵计时、副本地图清理、拾取窗口过期、`m_dwRunTick` 250ms 节流）与 `TItemManager` 的全部列表管理（含 `m_nProcItemIDx` 游标式分片 `Run`、5 分钟延迟释放 `m_FreeItemList`）**都可以独立验证**，值得按「接缝 + 可测决策部分」移植；只有 `DeleteFromMap`/`GetStdItem`/`AddGameDataLog` 三处需要接缝。

### 8.4 交接提醒（给下一条接管本车道的会话）

1. **先跑门禁再动手**，并且**整工程**跑（不是只跑 `Sweep*`）—— 本轮 4 例失败只在整工程跑时暴露（见 §6.3-B）。
2. 工作树里**没有**未提交产出，也没有越区文件；`Sweep/*` 的既有接缝**请复用，不要另造**。
3. 新增 `.cs` 会自动纳入编译，**不要改 `csproj`**（本轮未改）。
4. 若继续做自旋/锁相关单元，**务必遵守 §6.3-A 的时钟约定**（默认自增、仅在单次 CAS 成功路径用 `FreezeClock`），否则会再次挂死测试进程。
5. `Sweep/` 只是**路径隔离手段**，不是「另造一套」——新单元必须继续复用 `GXX.Core` 与 `GXX.M2Server` 既有类型。

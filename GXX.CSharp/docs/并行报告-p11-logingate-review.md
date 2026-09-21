# 并行报告 p11-logingate-review：`Source/LoginGate` 25 单元逐单元复核

> 车道：`p11-logingate-review`（**只读复核车道**，唯一产物 = 本文件）
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p11-logingate-review`（分支 `par/p11-logingate-review`）
> 基线：`main @ d8674c8e`（本工作树 HEAD = `c2bd3fbd` = `d8674c8e` 的父提交，仅差"第 54 轮调度提交"这一份 docs；源码/工具侧与 main 一致）
> 授权边界：本车道**未改**任何 `src/**`、`tests/**`、`tools/**`、`*.csproj`、`GXX.slnx`、`docs/Checklist.md`、`docs/并行派发台账.md`、`docs/并行覆盖审计.md`；**未执行** `dotnet build` / `dotnet test`。

---

## 0. 结论摘要（先给数字）

| 判定 | 单元数 | 行数合计 | 含义 |
|---|---|---|---|
| **A 真移植**（存在托管实现且确实服务于 LoginGate；含"部分移植 + 残留缺口"） | **8** | **5,140** | AppMain / ClientSession / ClientThread / ConfigManager / EDcode / IPAddrFilter / LogManager / Protocol |
| **B 共享框架等价覆盖**（被 GatewayKit / .NET 通用设施取代，或死代码 / 逐字节重复，属"按设计不移植"） | **13** | **9,207** | AcceptExWorkedThread / DesUtils / FixedMemoryPool / IOCPManager / IOCPTypeDef / MemPool / SHSocket / SimpleClass / SendQueue / SyncObj / ThreadPool / uDep / WinSock2 |
| **C 未移植真缺口**（无实现、无替代） | **4** | **1,977** | FuncForComm / GeneralConfig / Misc / PacketRuleConfig |
| 合计 | 25 | 16,324 | 与审计的 LoginGate 单元数/行数逐条相符 |

**对 §41.2 那个问题的直接回答**：
`mapped=15 / checklist-only=1 / not-ported=9` 这个口径**高估了完成度**。15 条 MAPPED 里：
- **只有 3 条**（`IPAddrFilter` / `LogManager` / `WinSock2`）的 E2 证据是**有效的**——因为它们的 LoginGate 副本与提供 E2 证据的兄弟副本**逐字节相同**（SHA256 相同，同一份源码）；
- **其余 12 条**的 E2 是**借名**：LoginGate 副本与"被移植的那份"**是不同文件**（多则相差 552 行归一化正文），E2 来自别的模块的 `.cs` 头部声明；
- **真正"代码已翻译"的只有 1 条**：`EDcode`（E1 真命中 `src/GXX.Core/Protocol/EDcode.cs`）。
- 结论：LoginGate 的 **15 条 MAPPED 中 12 条必须逐副本重判**（3 条 E2 有效的不必重判）。重判后的分布是：
  - 12 条借名里 → **A 6 条**（`AppMain`/`ClientSession`/`ClientThread`/`ConfigManager`/`EDcode`/`Protocol`，其中 5 条只是**部分**到位）、**B 2 条**（`AcceptExWorkedThread`/`DesUtils`）、**C 4 条**（`FuncForComm`/`GeneralConfig`/`Misc`/`PacketRuleConfig`）；
  - 其中 **`AcceptExWorkedThread` 与 `LoginGate/DesUtils` 是当前 not-ported 注册表完全没覆盖的两条**；
  - **4 条真缺口（C）合计 1,977 行 + 约 1,500 行残部**需要派车道。

**新增/修正的注册表建议（详见 §5）**：
- **新增**：`AcceptExWorkedThread`（可裸键）、`uDep`（可裸键）、**`LoginGate/DesUtils`（必须逐副本，禁止裸键）**。
- **修正**：`SendQueue` / `IOCPManager` 目前是**裸键**，而它们的 basename **并非唯一**（各有 LoginGate + SelGate 两份）——§39.5 里"basename 唯一 ⇒ 安全闭合"这个理由**在事实上不成立**（当时 `LoginGate` 还不在 `$Dir` 里，见 §41.2）。结论仍然成立（两份副本同判"不移植"），但**理由必须改写**。
- **不要动**：`IPAddrFilter` / `LogManager` / `WinSock2` 的 MAPPED（E2 有效）。

---

## 1. 判定方法与可复跑命令

### 1.1 口径定义（本报告使用的三条判据）

- **(A) 真移植**：该副本的**功能**在 LoginGate 运行时路径上有托管实现（或声明面 1:1 落地），并且**确实被 LoginGate 用到**（`Program.cs → FrmMain → LoginGateService → GateService/IocpManager/TcpLink/GatewayProtocol`，或 `GXX.Core` 共享设施）。允许"部分移植"，但**必须逐条列出残留缺口**。
- **(B) 共享框架等价覆盖**：该副本的**角色**由 GatewayKit / .NET 通用设施承担，且**没有任何为该单元声明的托管对应物**。三个子类：
  - `c3 取代`：由 .NET 原生设施取代（GC / lock / `SocketAsyncEventArgs` / `Socket` / 集合）；
  - `c4 重复`：与另一个模块的副本**逐字节（或仅换行差异）相同**，那份已被移植或被裁定"不移植"；
  - `c1 死代码/不编译`：在本工程当前的编译配置下**没有活调用点**。
- **(C) 未移植真缺口**：既无托管实现、也无替代设施，且给出**计数取证**（0 命中 / 大量缺项）。

> 说明：`Dead code` 不给第 4 个字母，按审计自己的分类法（`$VENDOR_UNITS` 把 c1/c3/c4 混装于同一"not-ported"桶）归入 **B**，但在"一句话证据"里**写明子类**，避免被读成"有设施替代"。

### 1.2 三条硬规则（本轮复核的关键，建议固化）

1. **禁用子串出现判定，必须用声明判定**。上一轮实测子串法会把 `AxeMon` 的 52 个类全部"命中"（全是注释引用）。本报告一律用
   `\b(class|record|struct|interface|enum)\s+<类名>\b` 抓**声明**。
2. **★ E2 借名真伪按 SHA256 分流**（本报告最重要的方法论产出）：
   - LoginGate 副本与"提供 E2 证据的那份源码"**逐字节相同** ⇒ **E2 有效**（同一份源码，兄弟模块的移植成品确实覆盖它）；
   - **不同** ⇒ **E2 是借名**（可能连算法都不同），该副本的 MAPPED 判决**无效**，必须逐副本重判。
   > 这条把 §39.3 的"重复 basename 盲区"从"要人记住"升级成"可机械判定"：**同名 + 字节相同 = 同一份；同名 + 字节不同 = 两份，必须分开裁定。**
3. **同名 ≠ 同单元**（本轮实测到 3 例假阳性，全部在 §6 列出）：`TList`（`SimpleClass.TList` vs Delphi RTL `Classes.TList`）、`TAddressListEx`（`LoginGate\FuncForComm.pas:51` vs `RunGate\GateShare.pas:90`）、`TThreadPool`（`GXX.Core\Async\TThreadPool.cs` 的 source 不是本副本）。**声明命中之后必须回看 source 行号引用。**

### 1.3 枚举与类/例程抽取（命令 1）

```powershell
$W='D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p11-logingate-review'
$M='D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\LoginGate'   # Source/** 是 GBK，读文本走 utf8 镜像
Get-ChildItem "$W\Source\LoginGate" -File -Filter *.pas | Sort-Object Name | ForEach-Object {
  $u=$_.BaseName
  $t=[IO.File]::ReadAllText((Join-Path $M "$u.pas"),[Text.Encoding]::UTF8)
  $cls=@([regex]::Matches($t,'(?m)^\s*(T\w+)\s*=\s*class')|%{$_.Groups[1].Value}|Select-Object -Unique)
  $rts=@([regex]::Matches($t,'(?m)^\s*(?:function|procedure|constructor|destructor)\s+(\w+)')|%{$_.Groups[1].Value}|Select-Object -Unique)
  '{0,-22} classes={1,-3} routines={2}' -f $u,$cls.Count,$rts.Count
}
```
> **执行陷阱**（§33.5 / §41.6 同源）：Windows PowerShell 5.1 按 ANSI 读 `.ps1`，**含中文的临时脚本必须写成纯 ASCII 并把路径当参数传进来**（`-File x.ps1 -W $W -M $M`），否则路径里的 `GXX原版` 会被解码成乱码、`Get-ChildItem` 直接 PathNotFound。本车道的 5 个探针脚本全部按此写法（脚本本体零非 ASCII 字符）。

### 1.4 托管声明检索（命令 2）

```powershell
$decl=@{}
Get-ChildItem "$W\GXX.CSharp" -Recurse -File -Filter *.cs |
  Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object {
    $rel=$_.FullName.Substring($W.Length+1); $i=0
    foreach($l in [IO.File]::ReadAllLines($_.FullName,[Text.Encoding]::GetEncoding(28591))){
      $i++
      foreach($m in [regex]::Matches($l,'\b(?:class|record|struct|interface|enum)\s+([A-Za-z_]\w*)\b')){
        $decl[$m.Groups[1].Value] += ,"$rel`:$i" } } }
# 索引规模：1,316 个 .cs（排除 bin/obj）→ 3,535 个声明名
```
结果：**LoginGate 25 个单元的全部 33 个类声明名，在全部 1,316 个 `.cs` 里只有 3 个命中，且 3 个全是"同名异单元"假阳性**（`TList` / `TAddressListEx` / `TThreadPool`，见 §6）。

### 1.5 例程级取证（命令 3）

对每个单元取 `.pas` **interface 段**（`implementation` 之前）的 `function|procedure|constructor|destructor` 名字，统计
`anyHitAnywhere`（1,316 个 `.cs` 里出现）与 `hitInLoginGateScope`（仅 `src/GXX.LoginGate/**` + `src/GXX.GatewayKit/**` 这 7 个文件里出现）。
后者才是"接线与否"的判据——**名字在别处出现不算接线**。

### 1.6 同源度（命令 4）

```powershell
Get-ChildItem "$W\Source\LoginGate" -File -Filter *.pas | ForEach-Object {
  $mine=(Get-FileHash $_.FullName -Algorithm SHA256).Hash
  Get-ChildItem "$W\Source" -Recurse -File -Filter "$($_.BaseName).pas" |
    Where-Object { $_.Directory.Name -ne 'LoginGate' } | ForEach-Object {
      $h=(Get-FileHash $_.FullName -Algorithm SHA256).Hash
      '{0,-22} {1}  LoginGate vs {2,-14} {3}' -f $_.BaseName,$(if($h -eq $mine){'IDENTICAL'}else{'differs'}),$_.Directory.Name,$h.Substring(0,16) } }
```

---

## 2. 25 行总表

> `类声明数` = `^\s*(T\w+)\s*=\s*class` 唯一命中数（命令 1）。
> `单元级例程数` = `^\s*(function|procedure|constructor|destructor)\s+(\w+)` 唯一命中数（命令 1，含方法实现）。
> `托管声明命中` = 命令 2 的结果 / 若无则注明**提供 E2 的文件**。
> `桶` 列的取值：`MAPPED`（保持）/ `not-ported`（登记）/ `需派车道`（进 `unit-map.tsv` 为 ASSIGNED 或留在 UNMAPPED 待派）。

| # | 单元 | 行数 | 类声明数 | 单元级例程数 | 托管声明命中？（哪个文件） | 判定 | 归入哪个桶 | 一句话证据 |
|---|---|---|---|---|---|---|---|---|
| 1 | `AcceptExWorkedThread` | 1,398 | 5 | 50 | **无**（E2 借名：`tests/GXX.SelGate.Tests/SelGateConfigTests.cs`、`src/GXX.SelGate/SelGateConfig.cs` 头注） | **B(c3)** | **not-ported（新增）** | 5 个类（`TIOCPWriter/TIOCPReader/TUserManager/TIOCPAccepter/TAcceptExWorkedThread`）全库 **0 托管声明**；55 个 interface 例程 **41 缺**；Accept 池由 `GatewayProtocol.cs:136-230` 的 `IocpManager`（`SocketAsyncEventArgs`）取代 |
| 2 | `AppMain` | 1,417 | 1 | 32 | `src/GXX.LoginGate/FrmMain.cs:13`（头注 `AppMain.pas TFormMain → FrmMain`） | **A（部分）** | MAPPED + **需补切片** | `FrmMain.cs` + `Program.cs:17-19` 接线成立；但 30 个 interface 例程 **26 缺**：socket 状态网格 / 5 个菜单项 / 日志级别 / `WMSysCommand` / `OnProgramException` |
| 3 | `ClientSession` | 820 | 1 | 13 | 无同名类；`GatewayProtocol.cs:84` `GateSession` + `src/GXX.LoginGate/LoginGateService.cs:21` | **A（部分）** | MAPPED + **需补切片** | `LoginGateService.cs:16` 头注直陈对应；会话表/双向转发/`GM_OPEN/CLOSE/DATA/KICK` 已实现；**但 LoginGate 副本比 SelGate 副本多 344 行归一化正文**（协议密码 / 二级密码 / `DelayClose`），`SelGateSession.cs` 只覆盖 SelGate 版 |
| 4 | `ClientThread` | 646 | 1 | 25 | 无同名类；`src/GXX.GatewayKit/GateService.cs:126-166`（`ServerLinkLoop`）+ `TcpLink.cs` 全文 | **A** | MAPPED（**口径敏感**，见 §7.1） | `GateService.cs:32` 头注直陈 `ClientThread.pas` 公共语义；连接/断线重连/收发/缓冲/关闭全部落地；17/24 缺项均为 Win32 内部机制（`InitClientSocket`/`LockBuffer`/`ReaderDone`…）无需 1:1 |
| 5 | `ConfigManager` | 269 | 1 | 9 | 无同名类；`GateService.cs:37-74`（`TFastIniFile` + `LoadConfig`） | **A（部分）** | MAPPED + **需补切片** | `TConfigMgr` **19 个字段** ↔ `LoadConfig` 只读 **5 个键**（`GateAddr/GatePort/ServerAddr/ServerPort/MaxConnOfIPaddr`），且 INI 段名不同（原文是 `[LoginGate]`，托管读 `Gateway/Server/PacketRule`） |
| 6 | `DesUtils` | 1,368 | 0 | 5 | **无**（E2 借名：`src/GXX.Client/ReadResources/PakCrypto.cs`、`Pak.cs`，对应的是 **ReadResources 副本**） | **B(c1)** | **not-ported（新增，必须逐副本 `LoginGate/DesUtils`）** | LoginGate 的全部调用点都在 `{$IF VER_TYPE=1}` 内（`AppMain.pas:113-119` 的 uses、`:251-460` 的两处 `DecryptDes_New`），而 `Misc.pas:9` 定 `VER_TYPE = 0` ⇒ **编译期出局**；`.dproj` 中 `DesUtils` 出现 **0 次**；4 份副本互不相同，**裸键会误伤 Client 的 PakCrypto** |
| 7 | `EDcode` | 1,415 | 0 | 31 | **`src/GXX.Core/Protocol/EDcode.cs`（E1 真命中）** | **A** | MAPPED（**唯一 E1 真移植**） | 31 个 interface 例程 **30 个在 `EDcode.cs` 里命中**，仅 `MakeDefaultMsg` 不在 Core（它在 `RunGate/MirClientContextSeams.cs:627`、`DBServer/SelectClient.cs:1128` 各自私有）；LoginGate 副本是 `PChar/string` 变体、Common 副本是 `AnsiString` 变体，同算法不同签名 |
| 8 | `FixedMemoryPool` | 486 | 1 | 20 | 无 | **B(c3+c4)** | not-ported（**已有裸键**） | 与 SelGate 副本 **SHA256 完全相同**；15/19 例程缺；`.NET` GC / `ConcurrentQueue` 取代 |
| 9 | `FuncForComm` | 590 | 2 | 22 | 无。`TAddressListEx` 的唯一命中 `src/GXX.RunGate/GateShareContainers.cs:227` 是 **RunGate\GateShare.pas:90** 的同名异单元 | **C** | **需派车道** | `TProcMsgThread` / `ShowThreadInfo` / `OnTimerProc` 在全部 1,316 个 `.cs` 里 **0 命中**；`TAddressInfo`（14 字段：IP 计数 / 拒绝 / 账号错 / 密码错 / 超时）未移植；`KeepAlive` 仅由 `FrmMain.cs:63-64` 的 5 秒计时器部分顶替 |
| 10 | `GeneralConfig` | 157 | 1 | 10 | **无**（E2 借名：`src/GXX.M2Server/Forms/GeneralConfigForm.cs`，对应 `M2Engine/Forms` 副本） | **C** | **需派车道** | `TfrmGeneralConfig` 9 个 interface 例程 **7 个全库缺失**；`src/GXX.LoginGate/**` 只有 3 个 `.cs`（`Program/FrmMain/LoginGateService`），无任何配置窗体；LoginGate 副本与 `Forms`/`SelGate` 两份**都不同**（三份互不相同） |
| 11 | `IOCPManager` | 285 | 3 | 15 | 无 | **B(c3+c4)** | not-ported（**已有裸键**；建议改逐副本，见 §5） | 与 SelGate 副本 **SHA256 完全相同**；8/16 例程缺（`CompPortInit`/`InitServer`/`InitGameServer`…）；`IocpManager` 取代 |
| 12 | `IOCPTypeDef` | 98 | 0 | 2 | 无 | **B(c3)** | not-ported（**已有裸键**） | 2/2 例程（`PostIOCPRecv`/`PostIOCPSend`）全库 0 命中；typedef 由 `SocketAsyncEventArgs` 取代；与 SelGate 副本仅换行/签名差异 |
| 13 | `IPAddrFilter` | 400 | 0 | 12 | 无同名类；`GateService.cs:49-53/203-227`（`_blockList`/`_perIP`/`CheckIP`/`TPerIPAddr`/`TBlockIPMethod`）；`src/GXX.SelGate/SelGateIPAddrFilter.cs:31`（1:1） | **A（部分）** | MAPPED（**E2 有效**：副本与 SelGate **SHA256 相同**）+ **需补切片** | 实况核心（黑名单 + 每 IP 连接数）由 GatewayKit 提供；**IP 段过滤 / 临时黑名单表 / `CheckNewIDOfIP` 换 ID 频率 / `m_fCheckNullSession` 开关 四项 `GatewayKit` 完全没有**——这是 `SelGateIPAddrFilter.cs:17-25` 自己写下的"必须保留的差异" |
| 14 | `LogManager` | 55 | 1 | 4 | 无同名类；`src/GXX.SelGate/SelGateLogManager.cs:12` `CLogMgr`（**1:1 逐字移植**） | **A** | MAPPED（**E2 有效**：副本与 SelGate **SHA256 相同**） | `TLogMgr` 的 4 个方法在 `CLogMgr` 里 1:1（`:9-16`、`:26-53` 含 `FormatStr`）；LoginGate 侧的等价出口是 `GateService.SendLog`→`OnLogMsg`→`FrmMain.cs:59` 的 ListBox；**`CheckLevel`/`m_nShowLogLevel` 在 LoginGate 路径上未接** |
| 15 | `MemPool` | 296 | 1 | 17 | 无 | **B(c3+c4)** | not-ported（**已有裸键**） | 与 SelGate 副本仅换行差异（归一化后 3/6 行）；12/16 例程缺；GC 取代 |
| 16 | `Misc` | 321 | 0 | 7 | **无**（E2 借名：`src/GXX.SelGate/SelGateMisc.cs:18`，对应 SelGate 副本） | **C** | **需派车道** | 8 个 interface 例程（`CloseIPConnect`/`KickUser`/`BlockUser`/`ReverseIP`/`AnsiStrToVal`/`SendGameCenterMsg`/`CheckAccountName`）在 `src/GXX.LoginGate/**`+`src/GXX.GatewayKit/**` 中 **0 命中**；`SelGateMisc.cs:11-16` 自陈"与 GatewayKit 的差异必须保留"；LoginGate 副本 **88 行独有** |
| 17 | `PacketRuleConfig` | 909 | 1 | 39 | **无**（E2 借名：`src/GXX.SelGate/SelGatePacketRule.cs`、`SelGatePacketRuleActive.cs`） | **C** | **需派车道** | 38 个 interface 例程 **31 个全库缺失**；`src/GXX.LoginGate/**` 无包过滤窗体（原文是 `.dpr` 第 3 个 `CreateForm`）；LoginGate 副本多出 `cbCheckNewIDOfIP` / `TrackBarIDLimitLevel` / `TabSheet3` / `Bevel1` 两个控件与两个 handler |
| 18 | `Protocol` | 118 | 0 | 0 | `GatewayProtocol.cs:50` `TSvrCmdPack`（1:1 packed 20B）、`GXX.Core/Protocol` 的 `TDefaultMessage`、`GateService.cs:239-250` `TPerIPAddr`/`TBlockIPMethod` | **A** | MAPPED（**E2 有效**） | 纯声明单元（0 类 0 例程）；`_STR_*`/`_IDM_TIMER_*` 由 `FrmMain` 计时器与 `LoginGateService` 基类 `@".\Config.ini"` 承载；**残留：`TNewIDAddr`/`TIPArea` 只在 `SelGateProtocol.cs`**；与 SelGate 副本仅 2/3 行差异（中文串+`g_boNetComGate`） |
| 19 | `SendQueue` | 561 | 1 | 13 | 无 | **B(c3)** | not-ported（已有裸键；**建议改逐副本**） | 7/12 例程缺；`GateSession._sendQueue`（`GatewayProtocol.cs:107-114`）+ `SocketAsyncEventArgs` 取代；与 SelGate 副本**不同**（13/18 行） |
| 20 | `SHSocket` | 594 | 0 | 23 | 无 | **B(c3)** | not-ported（已有裸键；**建议改逐副本**） | 18/20 例程缺（`TransmitFile`/`AcceptEx`/`GetAcceptExSockaddrs`…）；`.NET Socket` 取代；与 SelGate 副本仅注释差异（3/0） |
| 21 | `SimpleClass` | 1,236 | 4 | 46 | `TList` 命中 `src/GXX.Client/GUI/Share/FStateSeams.cs:134` —— **假阳性**：那是 Delphi RTL `Classes.TList`，不是 `SimpleClass.TList` | **B(c3+c4)** | not-ported（**已有裸键**） | 与 SelGate 副本 **SHA256 完全相同**；`TQueue/TStack/TList/TVector` 由 `System.Collections.Generic` 取代 |
| 22 | `SyncObj` | 41 | 1 | 5 | 无 | **B(c3+c4)** | not-ported（**已有裸键**） | 与 SelGate 副本 **SHA256 完全相同**；`TCriticalSection` 包装由 `lock`/`Monitor` 取代 |
| 23 | `ThreadPool` | 1,127 | 7 | 50 | `TThreadPool` 命中 `src/GXX.Core/Async/TThreadPool.cs:32` —— source 是**另一单元**，非本副本 | **B(c3+c4)** | not-ported（**已有 `LoginGate/ThreadPool` 逐副本键**） | 与 SelGate 副本 **SHA256 完全相同**；39/68 例程缺；真缺口在 **LogDataServer 副本**（445 行，已由 `p10-db-login-forms` 认领） |
| 24 | `uDep` | 103 | 0 | 1 | 无 | **B(c3)** | **not-ported（新增，可裸键）** | `SetCurrentProcessDEP` 全库 0 命中；原文在 `initialization` 里执行 **`DEP_DISABLED`**（`uDep.pas:99-100`）——托管侧**不应**复刻关闭 DEP 的行为；basename 全仓唯一（无兄弟副本） |
| 25 | `WinSock2` | 1,614 | 0 | 120 | `src/GXX.Client/Tail/WinSock2Seam.cs`、`WinSock2Structs.cs`、`WinSock2Constants.cs`、`tests/GXX.Client.Tests/TailWinSock2*.cs`（E2，来自 **Client-HGE 副本**） | **B(c4+c3)** | **建议不改**（保持 MAPPED；**E2 有效**：三副本 `Client-HGE`/`LoginGate`/`SelGate` **SHA256 完全相同**） | 120 个例程 61 个全库缺；LoginGate 运行时走 .NET `Socket`/`SocketAsyncEventArgs`，**不需要裸 winsock**；但三副本字节相同 ⇒ MAPPED 不是假阳性，登记 `LoginGate/WinSock2` 只会制造噪音与 §39.3 撞键风险 |

---

## 3. 与 SelGate 的同源度表（SHA256）

`Source/LoginGate` 25 个 `.pas` 与全仓同名副本逐字节比对（命令 4）。**只有"逐字节相同"才允许引用兄弟模块的移植成品作为本副本的证据。**

### 3.1 逐字节相同（`IDENTICAL`）——共 **8 个 basename / 9 组比对**

| 单元 | LoginGate SHA256（前 16） | 相同副本 |
|---|---|---|
| `FixedMemoryPool` | `45D4F2E5DD070D9B` | SelGate |
| `IOCPManager` | `E27F77E59456D3D6` | SelGate |
| `IPAddrFilter` | `9EBC5173E385AB72` | SelGate |
| `LogManager` | `A6FCD308D550FC31` | SelGate |
| `SimpleClass` | `BE52D46FF828B9E2` | SelGate |
| `SyncObj` | `A89A51AE7153FB85` | SelGate |
| `ThreadPool` | `702113A1F52B0FC1` | SelGate（**不含** LogDataServer 副本，见下） |
| `WinSock2` | `74C679F061C3A2F8` | **Client-HGE 与 SelGate 两份都相同**（三份互相逐字节相同） |

> 这 8 个里面 **3 个的 E2 证据因此有效**（`IPAddrFilter` / `LogManager` / `WinSock2`），另 5 个虽然字节相同，但**两份副本同判"不移植"**（c3/c4），所以登记为 not-ported 不冲突。
> `ThreadPool` 的 **LogDataServer 副本不同**（`F19C2EA4579DEEDC`）——这正是 §39.3 那条"同名不同裁定"的原始案例，现行 `LoginGate/ThreadPool` + `SelGate/ThreadPool` 逐副本键处理正确。

### 3.2 不相同（`differs`）——共 17 个 basename

| 单元 | LoginGate 独有归一化行 / 兄弟独有 | 兄弟目录 | 差异性质（要点） |
|---|---|---|---|
| `AppMain` | **552 / 59** | SelGate | LoginGate 副本 1,250 行 vs SelGate 566 行；LG 多 `TFormMain`+`tmrVerify`+`VER_TYPE` 验证服务器整块 |
| `ClientSession` | **344 / 83** | SelGate | LG 多 `m_dwProtocolPassword` / `m_IsCanSetL2Password` / `m_IsCanCheckL2Password` / `m_IsDelayClose`，并 `uses UnitDes`（SelGate 版无） |
| `EDcode` | **359 / 154** | Common | LG 是 `PChar/string` 变体 + `uses Protocol`；Common 是 `PAnsiChar/AnsiString` + `EDCODEBEIJING` + `uses Grobal2` |
| `Misc` | **88 / 7** | SelGate | LG 多 `VER_TYPE/VER_VERSION/PROGRAM_NAME`；SelGate 多 `{$DEFINE SIGN3D}` / `g_boNetComGate` / `tSelGate` 参数 |
| `AcceptExWorkedThread` | 101 / 94 | SelGate | 大量成员顺序/换行差异（`ACCEPTEX_POST_COUNT`、`TOnUserEnter/Leave`、`FreeAcceptEx`） |
| `ConfigManager` | 35 / 22 | SelGate | LG 多 `m_boCheckVersion`/`m_sClientSoftVer`；默认端口 5500/7000 vs 5100/7100；INI 段名 `[LoginGate]` vs `Strings/Integer` |
| `PacketRuleConfig` | 19 / 1 | SelGate | LG 多 `cbCheckNewIDOfIP` / `TrackBarIDLimitLevel` / `TabSheet3` / `Bevel1` + 2 handler |
| `SendQueue` | 13 / 18 | SelGate | `AddBuffer`/`AddBuffer2` 签名与 `PostIOCPSend(ErrCode)` 调用形式不同 |
| `IOCPTypeDef` | 4 / 2 | SelGate | 仅 `PostIOCPSend` 的 `var ErrCode` 参数差异 + 换行 |
| `MemPool` | 3 / 6 | SelGate | 仅换行（`TFreeMemEvent` 声明被折行） |
| `Protocol` | 2 / 3 | SelGate | 仅中文串（`登陆网关` vs `角色网关`）+ SelGate 多 `g_boNetComGate` |
| `SHSocket` | 3 / 0 | SelGate | **仅注释差异**（LG 多一段被注释掉的 `SafeSend`） |
| `ClientThread` | **0 / 0** | SelGate | **归一化后完全相同**（仅行尾/换行差异） |
| `FuncForComm` | 154 / 31 | SelGate | LG 多 `TAddressInfo`（14 字段 IP 计数记录）；SelGate 多 `g_LogLock`/`g_AppLogPath`/`g_LoginData` + `with FrmMain do` |
| `GeneralConfig` | — | Forms / SelGate | 三份**互不相同**（LG / `M2Engine/Forms` / SelGate） |
| `DesUtils` | — | M2Engine / Client-HGE\ReadResources / RunGate | **四份互不相同** |
| `uDep` | — | 无兄弟副本 | 全仓唯一（`5F2648434C116E6B`） |

### 3.3 副本目录一览（用于"是否可用裸 basename"的判定）

| 副本数 | 单元 |
|---|---|
| 1 份（唯一） | `uDep` |
| 2 份 | `AcceptExWorkedThread` `AppMain` `ClientSession` `ClientThread` `ConfigManager` `FixedMemoryPool` `FuncForComm` `IOCPManager` `IOCPTypeDef` `IPAddrFilter` `LogManager` `MemPool` `Misc` `PacketRuleConfig` `Protocol` `SendQueue` `SHSocket` `SimpleClass` `SyncObj`（= SelGate + LoginGate） |
| 3 份 | `EDcode`（Common/LoginGate/RunGate）、`GeneralConfig`（Forms/LoginGate/SelGate）、`ThreadPool`（LogDataServer/LoginGate/SelGate）、`WinSock2`（Client-HGE/LoginGate/SelGate） |
| 4 份 | `DesUtils`（Client-HGE\ReadResources / LoginGate / M2Engine / RunGate） |

---

## 4. 逐单元证据（按 A / B / C 分组）

### 4.1 (A) 真移植 —— 8 个单元 / 5,140 行

**接线链（唯一入口，可复跑核对）**：
`GXX.LoginGate/Program.cs:17` `new LoginGateService()` → `:18` `new FrmMain(service)` → `LoginGateService.cs:18` `: GateService` → `GateService.cs:60` `new IocpManager()` → `GatewayProtocol.cs` 的 `IocpManager`/`GateSession`/`GatewayProtocol`；上游链路 = `GateService.cs:131` `new TcpLink(ServerAddr, ServerPort)`。
项目引用：`GXX.LoginGate.csproj:9-10` → `GXX.Core` + `GXX.GatewayKit`；`GXX.slnx` 的 `/src/` 第 8 项 = `src/GXX.LoginGate/GXX.LoginGate.csproj`。
测试：`tests/GXX.Integration.Tests/GXX.LoginGate` 全链路用例 **1 个**（`LoginGateIntegrationTests.cs`，114 行，`GM_OPEN/GM_DATA/GM_CLOSE` + 6-Bit 帧往返）。**LoginGate 没有自己的单测工程**（`tests/` 下无 `GXX.LoginGate.Tests`）——这是本模块 16,324 行源码的验证面现状，见 §5.4。

| 单元 | (A) 的具体实现与接线证据 | 残留缺口（必须补） |
|---|---|---|
| `AppMain` | `FrmMain.cs:13` `public class FrmMain : Form`（头注 `AppMain.pas TFormMain → FrmMain`）；`Program.cs:17-19` 实例化并 `Application.Run`；启动/停止/日志/心跳计时器 4 件事齐备 | ① socket 状态网格（原文 `GridSocketInfoDrawCell` + `_STR_GRID_*` 5 个列头）；② 5 个菜单项（HELP_ABOUT / STARTClick / STOPClick / EXITClick / OPTION_GENERALClick / OPTION_IPFILTERClick / CLEAELOG / RELOADCONFIG）；③ `MemoLogDblClick`；④ 日志级别过滤；⑤ `WMSysCommand`/`WMSetParentWindow`（GameCenter 父窗口停靠）；⑥ `OnProgramException`/`OnAppModalBegin/End`；⑦ `{$IF VER_TYPE=0}` 分支（`AppMain.pas:625-632`）未被检视 |
| `ClientSession` | `GatewayProtocol.cs:84-130` `GateSession`（`SocketId/RecogId/IsVerified/LastRecv/LastSendTick/Buffer/BufferLen/PacketCount/PacketSpeedTick/SpeedLimited` + `EnqueueSend/TryDequeueSend/AppendBuffer/ResetBuffer`）= `TSessionObj` 的字段与缓冲语义；`LoginGateService.cs:21-128` 会话表 + `OnClientAccept/OnClientDisconnect/OnClientReceive/OnServerData/DeliverToClient` | ① `m_dwProtocolPassword` 通讯协议密码校验（LG 副本独有，SelGate 版没有）；② `m_IsCanSetL2Password`/`m_IsCanCheckL2Password` 二级密码门控；③ `DelayClose`/`m_dwDelayCloseTick`；④ `RotateBits`（`ClientSession.pas:103-120`）；⑤ `g_UserList` 固定数组语义（托管改 `ConcurrentDictionary`，容量上限语义丢失） |
| `ClientThread` | `GateService.cs:126-166` `_serverLink` + `ServerLinkLoop`（`OnConnected/OnDisconnected/OnReceive`、5 秒重连）;`TcpLink.cs:65-90` `Connect`（`NoDelay=true` + 异步收）、`:92-112` 收循环、`:114-129` `Send`、`:132-151` 累计缓冲、`:153-162` `Close` | 无功能缺口（`SafeSend`/`SendText` 的"重试到发完"语义：`TcpLink.Send` 一次 `Socket.Send`，**未循环发送**，大包可能截断 —— 建议核对，属可观测缺陷候选） |
| `ConfigManager` | `GateService.cs:37` `TFastIniFile Config`、`:67-74` `LoadConfig()` 读 5 键；`GXX.Core/Util/FastIniFile.cs` 提供 `ReadString/ReadInteger/ReadBool` | **19 个字段 / 19 个键未接**：`m_szTitle`、`m_nShowLogLevel`、`m_nGateCount`、`m_boCheckVersion`、`m_sClientSoftVer`、`m_xGameGateList[1..32]`（`ServerAdress/ServerPort/GatePort`，原文默认 5500/7000+i-1）、`m_fCheckNewIDOfIP`、`m_fCheckNullSession`、`m_fOverSpeedSendBack`、`m_fDefenceCCPacket`、`m_fKickOverSpeed`、`m_fKickOverPacketSize`、`m_nCheckNewIDOfIP`、`m_nMaxConnectOfIP`、`m_nClientTimeOutTime`(180000)、`m_nNomClientPacketSize`(700)、`m_nMaxClientPacketCount`(20)、`m_tBlockIPMethod`；且**原文 INI 段名是 `[LoginGate]`**，托管读 `Gateway/Server/PacketRule` ⇒ **现有 `Config.ini` 不兼容** |
| `EDcode` | `src/GXX.Core/Protocol/EDcode.cs:11` `public static unsafe partial class EDcode`（+ `EDcode.Tables.g.cs`）：31 个 LG 副本例程 **30 命中**；被 `LoginSrv`/`Client`/`DBServer`/`RunGate` 与集成测试使用（`EDcode.EncodeMessage/DecodeMessage`） | `MakeDefaultMsg` 不在 Core（分散在 `RunGate/MirClientContextSeams.cs:627` 与 `DBServer/SelectClient.cs:1128` 各一份私有实现）——建议提到 Core 单一实现 |
| `IPAddrFilter` | `GateService.cs:49-55`（`_blockList`/`_perIP`/`BlockIPMethod`/`MaxConnOfIPaddr`）、`:203-222` `CheckIP`、`:224-227` `AddBlockIP`、`:239-250` `TPerIPAddr`/`TBlockIPMethod`；`src/GXX.SelGate/SelGateIPAddrFilter.cs:31` `CSelGateIPFilter` 是**字节相同副本**的 1:1 移植（`:13` "1:1 逐字移植（:40-380）"） | ① IP 段过滤 `TIPArea` + `Load/SaveBlockIPAreaList` + `IsBlockIPArea`；② 临时黑名单 `g_TempBlockIPList` + `AddToTempBlockIPList`（`mBlock` 分支要用）；③ `CheckNewIDOfIP` 换 ID 频率限制 + `TNewIDAddr`；④ `m_fCheckNullSession` 总开关；⑤ `OverConnectOfIP` 判据差异（原 `Count+1 > Max` 超限**不**自增 vs 托管 `rec.Count > Max` 先自增再比）——**这四条差异是 `SelGateIPAddrFilter.cs:17-25` 自己写下的** |
| `LogManager` | `src/GXX.SelGate/SelGateLogManager.cs:12` `CLogMgr`：`m_hWnd`(:16)、`OnAppend`(:18)、`CheckLevel`(:26-29)、`Add`(:35-39)、`FormatStr`(:42)——对应 `LogManager.pas:9-16/26-53` 全部 4 个方法；LoginGate 路径出口 = `GateService.SendLog`(:83) → `OnLogMsg` → `FrmMain.cs:59` | `CheckLevel`/`m_nShowLogLevel` **在 LoginGate 路径上未接**（`FrmMain.cs:59` 无条件 append，不按级别过滤）；`CLogMgr` 在 `GXX.SelGate` 命名空间下，LoginGate 未引用 |
| `Protocol` | `GatewayProtocol.cs:50-58` `TSvrCmdPack`（`[StructLayout(Sequential, Pack=1)]`，6 字段 20B，与原文 `Protocol.pas:55-62` 逐字段一致）、`:21-46` `RUNGATECODE`/`GM_*`/`SS_*`；`GXX.Core/Protocol` 的 `TDefaultMessage`（48 个 `.cs` 命中）；`GateService.cs:239-250` `TPerIPAddr`/`TBlockIPMethod`（与 `Protocol.pas:83/86-90` 同名同形） | ① `TNewIDAddr`（`Protocol.pas:92-97`）、`TIPArea`（`:99-103`）只在 `SelGateProtocol.cs`；② `_tagCmdHeader`/`TEnDeInfo`（`:65-81`）、`LPDYNCODE`/`LPGETDYNCODE`（`:40-41`）、`MAX_FUNC_COUNT`/`MAX_SERVER_FUNC_SIZE`（`:34-37`）、`FIRST_PAKCET_MAX_LEN`（`:32`）、`TSockThreadStutas`（`:84`）未见托管对应物 —— 需逐名核对（本轮未逐条取证，见 §7.2） |

### 4.2 (B) 共享框架等价覆盖 —— 13 个单元 / 9,207 行

| 单元 | 子类 | 替代设施（具体到类型/方法） | 计数取证 | 桶动作 |
|---|---|---|---|---|
| `AcceptExWorkedThread` | c3 | `GatewayProtocol.cs:136-287` `IocpManager`：`StartAccept/ProcessAccept/ProcessReceive/Send/CloseSession` + `SocketAsyncEventArgs`（`.NET` 底层即 Windows IOCP）；`:134` 头注自陈"对应原 `IOCPManager.pas` + `AcceptExWorkedThread.pas`" | 5 个类全库 **0 声明**；55 个 interface 例程 **41 缺**、LoginGate 作用域 **1/55** | **新增 not-ported**（裸键可，理由见 §5.1） |
| `DesUtils` | c1 | 无替代——**本副本在 `VER_TYPE=0` 下不参与编译**（调用点全在 `{$IF VER_TYPE=1}`） | 5 个例程；LoginGate 活跃调用 **0**；`.dproj` 出现 **0** 次；4 副本互不相同 | **新增逐副本 `LoginGate/DesUtils`** |
| `FixedMemoryPool` | c3+c4 | `.NET` GC + `ConcurrentQueue`；`MemoryPool<T>` | 15/19 缺；与 SelGate **SHA256 相同** | 已是裸键，无需动 |
| `IOCPManager` | c3+c4 | 同上 `IocpManager` | 8/16 缺（`CompPortInit`/`CompPortcleanup`/`InitServer`/`InitGameServer`/`FindGameServer`/`CloseAllGameServer`…）；与 SelGate **SHA256 相同** | 已有裸键；**建议改 2 条逐副本键**（§5.2） |
| `IOCPTypeDef` | c3 | `SocketAsyncEventArgs` | 2/2 缺 | 已是裸键 |
| `MemPool` | c3+c4 | `.NET` GC | 12/16 缺 | 已是裸键 |
| `SHSocket` | c3 | `System.Net.Sockets.Socket` / `SocketAsyncEventArgs` | 18/20 缺；LG 与 SelGate 仅注释差异 | 已有裸键；建议改逐副本 |
| `SimpleClass` | c3+c4 | `System.Collections.Generic` | 与 SelGate **SHA256 相同**；`TList` 命中是假阳性 | 已是裸键 |
| `SendQueue` | c3 | `GatewayProtocol.cs:107-114` `GateSession._sendQueue`（`ConcurrentQueue<byte[]>`）+ `IocpManager.Send` | 7/12 缺（`GetSendQueue`/`AddBuffer2`/`GetDynPacket`…）；LG 与 SelGate **不同** | 已有裸键；建议改逐副本 |
| `SyncObj` | c3+c4 | `lock` / `Monitor` | 与 SelGate **SHA256 相同** | 已是裸键 |
| `ThreadPool` | c3+c4 | `System.Threading.ThreadPool` / `Task`；`GXX.Core/Async/TThreadPool.cs` 的 source 是另一单元 | 39/68 缺；与 SelGate **SHA256 相同**；LogDataServer 副本 **不同**（真缺口） | 已有 `LoginGate/ThreadPool` 逐副本键 |
| `uDep` | c3 | `.NET` 运行时自行管理 DEP；原文行为是**关闭** DEP（`uDep.pas:99-100` `SetCurrentProcessDEP(DEP_DISABLED)`） | 1/1 例程缺；全库 0 命中；basename 唯一 | **新增 not-ported** |
| `WinSock2` | c4+c3 | `System.Net.Sockets`；Client 侧 `WinSock2Seam.cs`/`WinSock2Structs.cs`/`WinSock2Constants.cs` 提供声明面 | 61/119 缺；三副本 **SHA256 完全相同** | **建议不改**（保持 MAPPED，见 §5.3） |

### 4.3 (C) 未移植真缺口 —— 4 个单元 / 1,977 行

| 单元 | 计数取证 | 缺口内容 | 规模 |
|---|---|---|---|
| `FuncForComm` | `TProcMsgThread` **0 命中 / 1,316 个 .cs**；`ShowThreadInfo`、`OnTimerProc` **0 命中**；`TAddressInfo` 未移植（`TAddressListEx` 的唯一命中是 `RunGate\GateShare.pas` 的同名异单元） | ① `TProcMsgThread`（`TTimer` 1ms 心跳：会话轮转 `Run`、客户端超时踢线 `m_nClientTimeOutTime`、`DelayClose` 到期关闭、`KeepAlive`、线程信息展示）；② `TAddressInfo`（14 字段：IP 计数/拒绝/账号密码错误计数）；③ `StartService`/`StopService`/`OnTimerProc` | 590 |
| `GeneralConfig` | 9 个 interface 例程 **7 个全库缺失**（只剩 `FormCreate`/`btnSaveClick` 的通用名）；`src/GXX.LoginGate/**` 无配置窗体 | `TfrmGeneralConfig`：`speGateCount`/`speGateIdx`/`EditServerIPaddr`/`EditTitle`/`TrackBarLogLevel`/`chkClientSoftVer`/`edtClientSoftVer` + 保存 `SaveConfig` | 157（+5,360B 文本 DFM） |
| `Misc` | 8/8 例程在 `src/GXX.LoginGate/**`+`src/GXX.GatewayKit/**` **0 命中**（只在 `SelGateMisc.cs`）；LG 副本 **88 行独有** | `CloseIPConnect`、`KickUser`(2 重载)、`BlockUser`、`ReverseIP`、`AnsiStrToVal`、`SendGameCenterMsg`、`CheckAccountName`——即"超包踢线 + 三条封禁路径（`mDisconnect/mBlock/mBlockList`）+ GameCenter 消息 + 换 ID 统计"整套反 CC 执法面 | 321 |
| `PacketRuleConfig` | 38 个 interface 例程 **31 个全库缺失**；LoginGate 作用域 **0/38** | `TfrmPacketRule`：永久/临时/活动三张 IP 列表 + 各 5~7 个右键菜单项 + IP 段过滤子页 + `MemoCmdFilter` + `rdDisConnect` + `cbAllowGetBackChr` + `TrackBarIDLimitLevel` | 909（+17,347B 文本 DFM） |

---

## 5. 给调度方的可执行建议

### 5.1 该进 not-ported 注册表的（新增 3 条）

| 建议键 | 形式 | 理由 |
|---|---|---|
| `AcceptExWorkedThread` | **裸键可用** | basename 有 LoginGate + SelGate 两份，但**两份裁定相同**（均被 `IocpManager` 取代）；实测 5 个类在全部 1,316 个 `.cs` 里 **0 托管声明**，无"另一份是真移植"的风险。若严格按 §39.3，也可写 `LoginGate/AcceptExWorkedThread` + `SelGate/AcceptExWorkedThread` 两条 |
| `LoginGate/DesUtils` | **必须逐副本；禁止裸键** | `DesUtils` 有 **4 份互不相同**的副本，其中 `Client-HGE\ReadResources` 那份**正在被移植**（`src/GXX.Client/ReadResources/PakCrypto.cs` 头注 + `Pak.cs:38`）。裸键 `DesUtils` 会把 Client 那份的 MAPPED 一并翻成 VENDOR，**正好是 §39.3 禁止的形态** |
| `uDep` | **裸键可用** | basename 全仓唯一（无兄弟副本），符合 §39.4/§39.5 的"唯一 ⇒ 安全闭合"条件 |

### 5.2 该**修正**的既有注册（不是新增，是理由改写 + 可选硬化）

`SendQueue` 与 `IOCPManager` 目前是**裸键**，而实测**它们各有 2 份副本**（LoginGate + SelGate）。§39.5 写下的理由是"basename 唯一（如 `imm`/`SendQueue`/`IOCPManager`）⇒ 可以在 `.cs` 头注明，安全闭合"——**这个理由在事实上不成立**：当时 `$Dir` 还没有 `LoginGate`（§41.2 才补入），所以那两份"唯一 basename"只是在**被审计的子集里**唯一。

- **结论不变**（两份副本同判"不移植 + 被 GatewayKit 取代"），所以裸键**当前不产生错误裁定**；
- **但理由必须改写**，并建议**硬化成逐副本**（4 条）：
  `LoginGate/SendQueue`、`SelGate/SendQueue`、`LoginGate/IOCPManager`、`SelGate/IOCPManager`。
  依据：`SendQueue` 的两份副本**并不相同**（13/18 行归一化差异），`IOCPManager` 的两份**逐字节相同**；逐副本键能让"同名不同裁定"在未来有第三份副本时**自动**表达出来。
- 同理建议核对 `MemPool`（两份仅换行差异）、`SHSocket`（仅注释差异）——**当前同判**，可保持裸键，但请在注册表注释里记下"已核对：两份副本同判"。

### 5.3 建议**不动**的（保持 MAPPED）

| 单元 | 理由 |
|---|---|
| `WinSock2` | 三份副本（Client-HGE / LoginGate / SelGate）**SHA256 完全相同** ⇒ E2 来自 Client 的 `WinSock2Seam.cs` 等文件是**同一份源码**的证据，**不是借名假阳性**。登记 `LoginGate/WinSock2` 不会省下任何工作量，却会在裸键误用时撞掉 Client 那份（§39.3）。→ **保持现状** |
| `IPAddrFilter` / `LogManager` | 同上：副本与 SelGate **逐字节相同**，SelGate 侧已有 1:1 移植（`SelGateIPAddrFilter.cs` / `SelGateLogManager.cs`）⇒ E2 有效，MAPPED 判决成立。**残留缺口走 §5.4 的派车道，而不是改桶** |

### 5.4 建议派的车道（3 条）

| 车道建议名 | 覆盖单元 | 规模（源行） | 落点 | 关键约束 |
|---|---|---|---|---|
| **`p11-logingate-filter`**（优先） | `Misc`(321) + `FuncForComm`(590) + `IPAddrFilter` 残部(400 中的 IP 段/临时表/CheckNewIDOfIP) + `ConfigManager` 残部(269 中的 14 个键 + INI 段名兼容) | **≈1,200 行有效** | `src/GXX.GatewayKit/**`（新增通用设施）+ `src/GXX.LoginGate/**` | ★ **绝不能把 SelGate 的差异"统一"掉**：`SelGateIPAddrFilter.cs:17-25`、`SelGateMisc.cs:11-16`、`SelGateSession.cs:17-24` 三处头部**明确写着"与 GatewayKit 的差异必须保留"**。新增的通用设施必须**可选启用**，SelGate/RunGate 行为不变 |
| **`p11-logingate-ui`** | `AppMain` UI 残部（网格/7 个菜单项/日志级别/`WndProc`/异常对话框）+ `GeneralConfig`(157+5.4KB DFM) + `PacketRuleConfig`(909+17.3KB DFM) | **≈1,300 行 .pas + 22.7KB DFM** | `src/GXX.LoginGate/**` + 建议新建 `tests/GXX.LoginGate.Tests/**` | DFM 侧好消息：LoginGate 的 3 个 `.dfm` 都是**文本 DFM**（首字节 `object f...`，不是 §41.3#1 那种二进制 DFM），可直接读 `Source/**` 原文；`FrmMain.cs` 与 `GatewayKit/GateMainForm.cs` **结构几乎逐行相同**（`GateMainForm` 还多显示 `OnlineCount` 并调用 `OnKeepAliveTimer`）⇒ 建议**删掉 `FrmMain.cs` 改用 `GateMainForm`**，避免第 3 份主窗体 |
| **`p11-logingate-cfg-residual`**（可并入第一条） | `ClientSession` 残部（协议密码 / 二级密码 / `DelayClose` / `RotateBits`）+ `ClientThread` 的 `Send` 非循环发送核对 | ≈250 行 | `src/GXX.LoginGate/**` / `src/GXX.GatewayKit/TcpLink.cs` | `RotateBits`/`m_dwProtocolPassword` 是 **LoginGate 副本独有**（SelGate 版没有），`SelGateSession.cs` 无法复用；`TcpLink.Send` 单次 `Socket.Send` 对大包可能截断，建议作为可观测缺陷单独核对 |

### 5.5 工具硬化建议（本车道**无权改** `tools/`，仅提案）

1. **审计报表的 `DUPLICATE-BASENAME` 分节增加"字节同一性"标注**。现状：该分节只列 `dir | unit | lines | verdict`，读者无法区分"重复 basename + 同一份源码（E2 有效）"与"重复 basename + 两份不同源码（E2 借名）"。
   建议：对每个重复组，额外输出该组各副本的 `SHA256(前8)` 与"组内不同哈希数"。**零成本实现**：`Get-FileHash`，只在重复组上算（实测全树重复组 ≈30 组，成本可忽略）。这样 §1.2 规则 2 就从"靠人记得"变成"报表直接标出"。
2. **`E2` 证据增加"来源副本"限定**（可选，成本略高）：`$csHead` 命中时同时记录命中文件所属模块，与 `<unit>.pas` 的副本目录做一次比对；不一致则降级为 `WEAK`。**但这会改变现有统计口径**（`mapped` 会掉），建议先只在报表的 `WEAK` 分节以**新列**呈现，不改判据。
3. **`ThreadPool` 那条注释里的"basename 唯一"表述**（`audit-coverage.ps1:100-103` 与 `:90-94`）建议在 `SendQueue`/`IOCPManager` 行补一句"本 basename 有 LoginGate+SelGate 两份，已核对两份同判"——把 §5.2 的核对结果落到代码注释里。

---

## 6. 本轮实测到的"同名 ≠ 同单元"假阳性（3 例，全部如实登记）

| 名字 | 命中位置 | 真实身份 | 影响 |
|---|---|---|---|
| `TList` | `src/GXX.Client/GUI/Share/FStateSeams.cs:134` | 头注明写 `Delphi Classes.TList（无泛型、Object 元素、0-based）` = **Delphi RTL `TList`**，不是 `SimpleClass.pas` 的 `TList` | 若按子串/类名直接采信，会给 `SimpleClass` 制造"已移植"假证据（本报告判 B，不受影响） |
| `TAddressListEx` | `src/GXX.RunGate/GateShareContainers.cs:227`（注释引原文 `:90-115` / `:2708-2834`） | `TAddressListEx` 在**两个单元**里声明：`LoginGate\FuncForComm.pas:51` 与 `RunGate\GateShare.pas:90`；行号引用证明命中来自 **RunGate 的 GateShare.pas** | 若采信，`FuncForComm` 会被误判 A；正因为发现这点才判 **C** |
| `TThreadPool` | `src/GXX.Core/Async/TThreadPool.cs:32` | 该文件的 source 与 `LoginGate\ThreadPool.pas` 不是同一单元（本副本另有 LogDataServer/SelGate 同名副本） | `ThreadPool` 本就已有逐副本 not-ported 键，不受影响；但说明**必须核对 source 行号引用** |

> 规程建议：**"声明命中"之后必须再问一句"这个声明的 source 注释指向哪份 `.pas` 的哪几行？"** 行号引用是本工程既有的强证据（大量 `.cs` 里写着 `// 原 :1234`），应当被用到判据里，而不是只看类名。

---

## 7. 如实登记：无法判定的部分与口径敏感性

### 7.1 口径敏感性（3 个单元有两种合理读法，本报告已选其一并给出另一读法的数字）

本报告对 A 的定义是"**功能**在 LoginGate 路径上落地（允许由 GatewayKit 的通用类型承担，且该类型头部自陈对应本单元）"。若调度方改用**更严的"代码已翻译"标准**（即 §39.5 处理 `SendQueue`/`IOCPManager` 时用的标准：按 interface 例程缺项数判定），则以下 3 条会从 A 落到 B：

| 单元 | 本报告判定 | 严标准的判定 | 依据 | 数字影响 |
|---|---|---|---|---|
| `ClientThread` | A | B | 24 个 interface 例程 **17 缺**（`InitClientSocket`/`LockBuffer`/`ReaderDone`… 全是 Win32 内部机制） | A 8/5,140 → A 7/4,494；B 13/9,207 → B 14/9,853 |
| `LogManager` | A | B | 4 个方法在 `CLogMgr` 里 1:1，但那份在 `GXX.SelGate` 命名空间、LoginGate 未引用 | A 7/4,494 → A 6/4,439；B 14/9,853 → B 15/9,908 |
| `IPAddrFilter` | A | B | 12 个 interface 例程里实况覆盖 ≈5，另 4 项差异是 SelGate 自己写下的"必须保留" | A 6/4,439 → A 5/4,039；B 15/9,908 → B 16/10,308 |

**建议**：采用本报告的 A 口径（"功能落地"），因为 LoginGate 是一个**可运行的服务**（`LoginGateIntegrationTests` 实证端到端 `GM_OPEN/GM_DATA/GM_CLOSE`），把提供该服务的主链路类型一律判"未移植"会让报表失去"服务是否可用"的信息；而**残留缺口用"需补切片"列单独表达**（本报告已这么做）。请调度方明确写进规程。

### 7.2 本轮**未**逐条取证、因此不作为判决依据的项（不猜）

1. **`Protocol.pas` 剩余声明名的逐名覆盖**：`_tagCmdHeader`/`TEnDeInfo`/`LPDYNCODE`/`LPGETDYNCODE`/`MAX_FUNC_COUNT`/`MAX_SERVER_FUNC_SIZE`/`MAX_CLIENT_FUNC_SIZE`/`FIRST_PAKCET_MAX_LEN`/`TSockThreadStutas`/`TNewIDAddr`/`TIPArea` 共 11 项**未逐名比对**。已确证的是 `TSvrCmdPack`/`RPCK` 常量/`TPerIPAddr`/`TBlockIPMethod`/`TDefaultMessage` 有对应物，故判 A；上表列出的 11 项需在补切片时逐名核对。
2. **`AppMain.pas` 的 `{$IF VER_TYPE=0}` 活跃分支逐行覆盖**：只确证了 `VER_TYPE=1` 块（`:58-83`、`:113-119`、`:172-197`、`:251-460`、`:518-525`、`:1192-1320`、`:1337-1340`、`:1342-1413`）**不编译**，以及 `:625-632` 是 `VER_TYPE=0` 分支；该分支的**具体内容未逐行读**。
3. **`AcceptExWorkedThread` 的 101/94 行差异是否含实质逻辑差异**：只确证了"两边都 0 托管声明"，未逐行判定差异性质。**不影响判决**（两边都是 B）。
4. **`RunGate/GateShare.pas` 的 `TAddressListEx` 是否与 LoginGate 的 `FuncForComm.TAddressListEx` 语义相同**：未比对。**不影响判决**（那是 RunGate 的单元归属）。
5. **`TcpLink.Send` 的单次 `Socket.Send` 是否真会截断**：这是**怀疑**，未构造用例验证，故只登记为"建议核对"，未计入缺口。

### 7.3 本车道的只读边界声明

- 未修改任何 `src/**`、`tests/**`、`tools/**`、`*.csproj`、`GXX.slnx`、`docs/Checklist.md`、`docs/并行派发台账.md`、`docs/并行覆盖审计.md`；
- 未执行 `dotnet build` / `dotnet test`；
- 未在主工作树执行任何 git 写命令；未执行 `merge/rebase/checkout/switch/push/worktree/reset`；
- 未运行 `audit-coverage.ps1 -Report`（该开关会重写 `docs/并行覆盖审计.md`）。只跑了 `-Dir LoginGate -ShowMapped` 的**只读**形式，用于复现 §41.2 的 `mapped=15 / checklist-only=1 / not-ported=9` 基线数字：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File "$W\GXX.CSharp\tools\audit-coverage.ps1" -Dir LoginGate -ShowMapped
# → explicit map rows: 58 / cs files indexed: 1316 / src text loaded: 760
# → LoginGate  Units=25  Mapped=15  Weak=0  Assigned=0  ChecklistOn=1  Unmapped=0  Vendor=9  NonUnit=0
# → 15 条 MAPPED 中 14 条 E1=False E2=True，仅 EDcode 为 E1=True
```

- 所有写文件动作用**绝对路径**；本报告是唯一产物。

---

## 8. 附录：本报告的完整数字表（可直接抄进台账）

### 8.1 单元清单（按判定分组，行数取自审计口径 = 物理行数）

| # | 单元 | 行数 | 类 | 例程 | interface 例程 | 判定 | 子类 |
|---|---|---|---|---|---|---|---|
| 1 | `AppMain` | 1,417 | 1 | 32 | 30 | A | 部分（UI 面 26/30 缺） |
| 2 | `ClientSession` | 820 | 1 | 13 | 9 | A | 部分（LG 独有 344 行未覆盖） |
| 3 | `ClientThread` | 646 | 1 | 25 | 24 | A | 完整（17 缺为 Win32 内部） |
| 4 | `ConfigManager` | 269 | 1 | 9 | 8 | A | 部分（19 字段 vs 5 键） |
| 5 | `EDcode` | 1,415 | 0 | 31 | 31 | A | 完整（30/31 命中 Core） |
| 6 | `IPAddrFilter` | 400 | 0 | 12 | 14 | A | 部分（4 项差异未覆盖） |
| 7 | `LogManager` | 55 | 1 | 5 | 4 | A | 部分（CheckLevel 未接） |
| 8 | `Protocol` | 118 | 0 | 0 | 0 | A | 部分（11 个声明名待核） |
| | **A 小计** | **5,140** | | | | | |
| 9 | `AcceptExWorkedThread` | 1,398 | 5 | 50 | 55 | B | c3 |
| 10 | `DesUtils` | 1,368 | 0 | 5 | 2 | B | c1（VER_TYPE=1 专用） |
| 11 | `FixedMemoryPool` | 486 | 1 | 20 | 19 | B | c3+c4 |
| 12 | `IOCPManager` | 285 | 3 | 15 | 16 | B | c3+c4 |
| 13 | `IOCPTypeDef` | 98 | 0 | 2 | 2 | B | c3 |
| 14 | `MemPool` | 296 | 1 | 17 | 16 | B | c3+c4 |
| 15 | `SHSocket` | 594 | 0 | 23 | 20 | B | c3 |
| 16 | `SimpleClass` | 1,236 | 4 | 46 | 55 | B | c3+c4 |
| 17 | `SendQueue` | 561 | 1 | 13 | 12 | B | c3 |
| 18 | `SyncObj` | 41 | 1 | 5 | 4 | B | c3+c4 |
| 19 | `ThreadPool` | 1,127 | 7 | 50 | 68 | B | c3+c4 |
| 20 | `uDep` | 103 | 0 | 1 | 1 | B | c3 |
| 21 | `WinSock2` | 1,614 | 0 | 120 | 119 | B | c4+c3 |
| | **B 小计** | **9,207** | | | | | |
| 22 | `FuncForComm` | 590 | 2 | 22 | 21 | C | — |
| 23 | `GeneralConfig` | 157 | 1 | 10 | 9 | C | — |
| 24 | `Misc` | 321 | 0 | 7 | 8 | C | — |
| 25 | `PacketRuleConfig` | 909 | 1 | 39 | 38 | C | — |
| | **C 小计** | **1,977** | | | | | |
| | **合计** | **16,324** | | | | | |

### 8.2 与现行审计口径的对照

| 口径 | mapped | not-ported | checklist-only | unmapped | 真缺口（需派车道） |
|---|---|---|---|---|---|
| 现行审计（`main @ d8674c8e`，`-Dir LoginGate`） | **15** | **9** | 1（`uDep`） | 0 | 0 |
| 本报告**判定** | **8**（A） | **13**（B） | 0（`uDep` → B） | 0 | **4**（C） |
| 本报告**建议的桶动作** | **9**（A 8 + `WinSock2` 保持 MAPPED，E2 有效） | **12**（B 13 − `WinSock2` 不登记） | 0 | 0 | **4** |

> 逐项对账：现行 9 条 not-ported（`SimpleClass`/`SHSocket`/`FixedMemoryPool`/`MemPool`/`SyncObj`/`IOCPTypeDef`/`SendQueue`/`IOCPManager`/`LoginGate/ThreadPool`）**全部保留**；从 MAPPED 移入 not-ported 的是 `AcceptExWorkedThread` 与 `LoginGate/DesUtils`（逐副本）；`uDep` 从 checklist-only 移入；`WinSock2` 判定为 B 但**建议不登记**（三副本字节相同，E2 有效）。
> 用一句话概括：**LoginGate 的"15 条已完成"实质是"8 条真到位（其中 5 条只是部分到位）+ 13 条按设计不移植 + 4 条真缺口"，真缺口 1,977 行 + 残部约 1,500 行。**

---

*报告生成：车道 `p11-logingate-review`（只读复核）；全部数字均可由 §1.3–§1.6 四条命令在本工作树复跑。*

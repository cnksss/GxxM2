# 并行报告 — 车道 `p7-db-selectclient`

> 单元：`Source\DBServer\SelectClient.pas`（**1,248 行**，GBK）
> 工作树：`.worktrees\p7-db-selectclient`，分支 `par/p7-db-selectclient`
> 基线：`main` = `ca026e6d`；`GXX.DBServer.Tests` 基线 **371 例**（实跑确认）
> 收尾：**542 例全绿**（371 + **171 新增**），`dotnet build GXX.slnx` = **0 error / 157 warning**（与基线同数，无新增警告）

---

## 1. 提交清单

| # | commit | 内容 |
|---|---|---|
| 1 | `e8615f65` | 切片1：`SelectClient.pas` 1:1 移植（接缝 + 1000 槽 `TSelectChar` + `TSelectClient` 全体方法） |
| 2 | `ddb42d8b` | 切片2：`TSelectChar` 1000 槽会话表测试（**27 例**，含 `Initialize`/`Finalize` 字段集差异断言） |
| 3 | `f55e8ee2` | 切片3：收包/解帧/分发内核测试（`ExecGateBuffers`/`ProcessUserMsg`/`DeCodeUserMsg`，**71 例**；tick 源统一到可注入的 `DelphiTick`） |
| 4 | `8058b0e8` | 切片4：角色族测试（**71 例**，含两处"恒返回 False"的原文缺陷锁定） |
| 5 | `f834d80f` | 切片5：`TSelectClient` 构造初值与 `Destroy` 用例（**2 例**） |

工作树 `git status --porcelain` = **空**。

### ★ 一次自己的操作事故（已修复，逐字记录）

切片2 中我用 `Get-Content -replace` + `Set-Content` 改了 `SelectClient.CharTable.cs`（**违反"`.cs` 一律用编辑工具"**），
把 UTF-8 中文注释按 ANSI 读入再写回 ⇒ 注释全部乱码、换行被吞（`git diff` 显示 −79/+38 行）。
按台账 **§29.4 规程 2「恢复只按路径白名单」** 用 `git checkout HEAD -- GXX.CSharp/src/GXX.DBServer/SelectClient.CharTable.cs` 恢复，
再用编辑工具重做（最终该文件相对切片1 只剩 4 行 `DelphiRTL.GetTickCount` → `DelphiTick.GetTickCount` 的改动）。
**该规程有效，未波及其它文件。**

---

## 2. 逐方法判定表

### 2.1 `TSelectChar`（SelectClient.pas:30-258）—— 已完成，全覆盖

| 原文 | 行 | 托管成员 | 判定 |
|---|---|---|---|
| `UserArray: TUserArray` | :32 | `TSelectChar.USER_ARRAY_LENGTH` + `TUserInfo[]` | ✅ 已完成 |
| `GetItem` | :116-122 | `Items(int)` | ✅ 3+ 例（−1/0/999/1000/`int.MinValue`/`int.MaxValue`） |
| `GetCount` | :124-127 | `Count` | ✅ |
| `GetOnLineItem` | :129-135 | `OnLineItems(int)` | ✅ 越界 nil |
| `GetOnLineCount` | :137-140 | `OnLineCount` | ✅ |
| `Add` | :142-168 | `Add()` | ✅ 9 例（回收表优先 / FIFO / 表满 −1 / **不写 Socket 故可重复返回同一下标**） |
| `Initialize`（无参） | :170-195 | `Initialize()` | ✅ 14 字段 + 清两表 |
| `Finalize`（无参） | :197-214 | `Finalize()` | ✅ **7 字段**（差异断言） |
| `Initialize(Index)` | :216-238 | `Initialize(int)` | ✅ 越界不抛 / 不清表 |
| `Finalize(Index)` | :240-258 | `Finalize(int)` | ✅ 摘 `OnLineList` + 追加 `DeleteList` / 越界不动表 / 重复调用致重复下标 |
| `Create`/`Destroy` | :102-114 | 构造函数 / `Destroy()` | ✅ |

### 2.2 `TSelectClient`（SelectClient.pas:55-1237）

| 原文 | 行 | 托管成员 | 判定 |
|---|---|---|---|
| `Create(ASocket)`（线程分支） | :265-278 | — | ⛔ **未编译**（`{$IF DBSUSETHREAD = 1}`；DBShare.pas:19 = **0**） |
| `Create(Socket, ServerWinSocket)` | :281-294 | `TSelectClient()` | ✅ 已完成（**偏差 D-p7-1**）|
| `Destroy` | :297-301 | `Destroy()` | ✅ |
| `Close` / `ClientExecute`（线程分支） | :305-348 | — | ⛔ **未编译**（同 DBSUSETHREAD=0）；任务书列出的 :84/:85 即此二处 |
| `SendKeepAlivePacket` | :350-382 | `SendKeepAlivePacket()` | ✅ 4 例（`%++$` 逐字节 / 三个 tick / 最大值不压低 / `m_Module` 接缝） |
| `SendKickUser` | :384-412 | `SendKickUser(string,int)` | ✅ 6 例（0/1/2 + **非 0/1/2 什么都不发**） |
| `SendUserSocket` | :414-434 | `SendUserSocket(string, byte[])` | ✅ 逐字节（**偏差 D-p7-2**）|
| `OutOfConnect` | :436-442 | `OutOfConnect(string)` | ✅ 逐字节 |
| `ExecGateBuffers(string)` | :444-538 | `ExecGateBuffers(string)` | ✅ 13 例（**本配置下真正生效的收包入口**，uFrmMain.pas:352） |
| `ExecGateBuffers(PChar,BufLen)` | :540-636 | `ExecGateBuffers(byte[],int)` | ✅ 2 例（**原文唯一调用点在未编译的 :330 ⇒ 不可达**；偏差 D-p7-4） |
| `ProcessUserMsg` | :638-667 | `ProcessUserMsg` | ✅ 3 例（<22 字节不派发 / 双帧 / 空段两次清缓冲） |
| `OpenUser` | :669-702 | `OpenUser` | ✅ 4 例（两级 IP / 重复 ConnID Exit / 多槽 / `Socket := Self`） |
| `CloseUser` | :704-723 | `CloseUser` | ✅ 3 例（会话活跃不动 IDSoc / 失效发 SS_SOFTOUTSESSION / 未知 ConnID 无副作用） |
| `DeCodeUserMsg` | :725-885 | `DeCodeUserMsg` | ✅ 27 例（见 §3 分派表） |
| `RandomName` | :889-901 | `RandomName` | ✅ 5 例（**`Random(Count-1)` 最后一项取不到** / 空表抛异常 / 不刷 tick） |
| `NewChr` | :903-998 | `NewChr` | ✅ 24 例（nCode 0/1/2/3/5/6/7/8 全覆盖 + **字节语义**） |
| `QueryDelChr` | :1000-1041 | `QueryDelChr` | ✅ 7 例（10 条上限 / 21 字节结构回读 / 空入参不查库） |
| `GetBackDelChr` | :1043-1081 | `GetBackDelChr` | ✅ 9 例（−5/−1/1/−2/0 + **恒返回 False**） |
| `DelChr` | :1083-1118 | `DelChr` | ✅ 6 例（−2/−3/0/1，边界 45） |
| `SelectChr` | :1120-1180 | `SelectChr` | ✅ 9 例（两条路由 + 动态 IP + 接缝未接线抛异常） |
| `QueryChr` | :1182-1237 | `QueryChr` | ✅ 11 例（串接格式 / 上限截断 / CloseUser / **恒返回 False**） |
| `initialization`/`finalization` | :1239-1248 | — | ✅ 原文为**空块**，照抄为注释 |

**覆盖率小结：`SelectClient.pas` 编辑器可见的全部实现行（1-258 / 260-301 / 350-1237）均已移植；未覆盖行仅 :303-348 与 :263-278（`DBSUSETHREAD = 0` 下编译器根本不读）。**

---

## 3. `DeCodeUserMsg` 分派表（命令 → 行号 → 语义）

| 命令（值） | case 行 | 语义 | 前置校验 | 节流窗口 |
|---|---|---|---|---|
| `CM_QUERYCHR` (100) | :735-752 | `QueryChr` → `SM_QUERYCHR`（+`EncodeString` 角色串） | **无 CheckSession** | `not boChrQueryed or (tick−dwChrTick) > 200`；否则 `[Hacker Attack] _QUERYCHR` |
| `CM_RANDOMNAME` (106) | :753-776 | `RandomName` → `SM_RANDOMNAME` + `EncodeString(名)` | `sAccount <> ''` **且** `CheckSession` | **无**（原文把 tick 判定整段注释掉了 :756-758 / :770-775） |
| `CM_NEWCHR` (101) | :777-800 | `NewChr` → `SM_NEWCHR_SUCCESS/FAIL`；无论成败都 `boChrQueryed := False` | `sAccount <> ''` **且** `CheckSession` | `(tick−dwChrTick) > 1000` |
| `CM_DELCHR` (102) | :801-823 | `DelChr` → `SM_DELCHR_SUCCESS/FAIL`；`boChrQueryed := False` | 同上 | `> 1000` |
| `CM_SELCHR` (103) | :824-847 | `SelectChr` → `SM_STARTPLAY`(+路由串)/`SM_STARTFAIL`；成功置 `boChrSelected` | 同上 | `not boChrQueryed`；否则 `Double send _SELCHR` |
| `CM_QUERYDELCHR` (105) | :849-862 | `QueryDelChr`（**返回值被丢弃**）→ `SM_QUERYDELCHR` | **无 CheckSession** | `> 200` |
| `CM_GETBACKDELCHR` (3006) | :863-876 | `GetBackDelChr`（**返回值被丢弃**）→ `SM_GETBAKCHAR_SUCCESS/FAIL` | **无 CheckSession** | `> 200` |
| 其它 | :877-883 | 回 `SM_CHECKISMYSELFSERVER`(8889)（原文注释："检测是否为我们自己的服务端"） | — | — |

不通过前置校验的三个分支统一：`OutOfConnect(sConnID)` + `MainOutMessage("[ERROR] _XXX …")`。

---

## 4. 新增文件与覆盖行号

| 文件 | 对应原文行 | 说明 |
|---|---|---|
| `src/GXX.DBServer/SelectClient.Seams.cs` | 依赖面（非 SelectClient.pas 本体） | 接缝：`TCustomWinSocket`/`TServerClientWinSocket`、`ITFrmIDSoc`+`IDSocCliSeam`、`SelectClientRoleDbSeam`、`SelectClientGlobals`、`SelectClientDbShareSeam`、`SelectClientRandom`、`SelectClientModuleSeam`、`SelectClientAnsi`（AnsiString 字节串映射） |
| `src/GXX.DBServer/SelectClient.CharTable.cs` | :10-258 | `TUserInfo` / `TSelectChar` |
| `src/GXX.DBServer/SelectClient.cs` | :55-1237 | `TSelectClient` 全体 |
| `tests/…/SelectClientTestDoubles.cs` | — | `FakeFrmIDSoc` / `FakeSelectHumanDB` / `FakeSelectHeroDB` / `SentFrame` / `SelectClientTestBase` |
| `tests/…/SelectClientCharTableTests.cs` | :10-258 | 27 例 |
| `tests/…/SelectClientFrameTests.cs` | :350-885 | 73 例 |
| `tests/…/SelectClientRoleTests.cs` | :889-1237 | 71 例 |

### ★ 字节语义（本车道最要紧的一处实现决定）

`SelectClient.pas` 的整条解析链（`m_sReceiveText` / `s10` / `s18` / `sChrName` …）是 Delphi `string = AnsiString`，
即 **1 字符 = 1 字节**。原文对它们做的是**按字节**判定：

* `NewChr:932` `Length(sChrName) < MIN_CHAR_NAME_LEN(4)` —— 两个汉字 = **4 字节** ⇒ 通过；按 UTF-16 计长只有 2 ⇒ 会误判"名字过短"。
* `NewChr:935-938` `if not (sChrName[I] in TextChars) then Delete(...)`，`TextChars = [#32..#255]` —— 按字节过滤；按 UTF-16 过滤会把**每个汉字整字删掉**。
* `CheckChrName`(DBShare.pas:1204-1249) 体里是 `Chr := sChrName[I]` 与 `#$81..#$FE`（GBK 首字节区间）比较。

因此托管侧对这两条链路统一采用 **latin-1 逐字节映射**（`SelectClientAnsi.StrOf/BytesOf`），
只在调用 DB / `FrmIDSoc` 等托管 API 的边界转成 GBK 文本（`AnsiTextOf`）。
接缝类里的名校验函数**一律收字节串**，唯一例外是 `CheckSpecialChar(WideString)`（收 GBK 文本）——三处都有专项用例锁定。

---

## 5. 测试与门禁

```
cd .worktrees\p7-db-selectclient\GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo
  → 0 Error(s) / 157 Warning(s)      （基线 157，无新增）
dotnet test tests\GXX.DBServer.Tests\GXX.DBServer.Tests.csproj -c Debug --nologo
  → Passed!  Failed: 0, Passed: 542, Skipped: 0, Total: 542
```

* 新增 **171 例**（27 + 71 + 71 + 2）；`[Fact]/[Theory]` 声明数：CharTable 21、Frame 62、Role 68。
* 无偶发失败：连跑 3 次全绿。
* 新增文件产生的编译警告 = **0**（`Finalize` 的 CS0465/CS0114 用 `#pragma` 就地抑制，见 D-p7-3）。
* 单测**不连真库、不开真 socket、不弹窗体**；`TempDirTest` + 本车道接缝复位保证隔离。

---

## 6. 集成点说明（**我没有改这些文件，只给接线位置**）

### 6.1 `src/GXX.DBServer/DBServerService.cs`（SelGate 端）

现状：`:114-181 ProcessGateData` 里 `while (link.AccumLength >= 22)` + 逐帧 `DecodeMessage` + 4 路 `switch(100/101/102/103)`。
**这正是 `Checklist.md:298` 过度声明的那一条**（任务书写的是 :287，实测该行是 ThreadPool 行；`SelectClient` 只在 :298 出现）——
它缺 105/106/3006、缺 else 分支、缺 1000 槽会话表。

| 位置 | 现状 | 需要接管为 |
|---|---|---|
| `:102-108` `link.OnReceive` | 直接 `ProcessGateData` | 建 `TSelectClient`（对应 `uFrmMain.pas:303-306 SelectSocketGetSocket`），把 `buf/len` 喂给 `client.ExecGateBuffers(buf, len)`，并把 `TSelectClient.SendTextSink = (c, b) => link.Send(b)`、`RemoteAddressSink = _ => 对端地址` |
| `:114-181 ProcessGateData` | 自造 4 路 switch | **删除**，由 `DeCodeUserMsg` 分派（这是 `uFrmMain.pas:340-353 SelectSocketClientRead` 的原形状） |
| `:130-137` case 100 | `Make(520, msg.Recog, n>0?1:0, 0, min(n,16))` | ★ **字段位置与原文不一致**：原文 `MakeDefaultMsg(SM_QUERYCHR, nChrCount, 0, 1, 0)` ⇒ 角色数在 **`Recog`**、`Tag=1`；这里是 `Param=n>0`、`Tag=0`。另：原文按 `g_nCreateChrNameCount` 截断并串接 `[*]名/职业/发型/等级/性别/` 文本，这里发的是 `TDeleteHumanInfo` 二进制 |
| `:139-147` case 101 | `Split('/')` 取 3 段 | 原文载荷是 `账号/名/发型/职业/性别`（5 段，`SelectClient.pas:923-927`），且有 8 种 nCode |
| `:148-156` case 102 | `parts[0]/parts[1]` = account/chr | 原文 `DelChr` 入参**只有角色名**，账号取**槽位的 `UserInfo.sAccount`**（:1101） |
| `:157-173` case 103 | 直接返回角色数据 | 原文 `SelectChr` 只回 `SM_STARTPLAY + EncodeString(routeIP + "/" + port)` |
| `:174-178` default | `UNKNOWMSG` | 原文回 `SM_CHECKISMYSELFSERVER`(8889) |

### 6.2 `src/GXX.DBServer/RoleDatabase.cs`

| 位置 | 现状 | 建议 |
|---|---|---|
| `:46 QueryChr(account, byte[] outBuf)` | 用 `TDeleteHumanInfo` 二进制序列化角色表（`:61-70`） | 原文走 `THumanDBBase.QueryHumans` + **文本**串接；建议命令层改调 `TMySqlHumanDB.QueryHumans`，本方法仅保留 M2 数据端用途或删除 |
| `:75 ChrExists` / `:88 ChrNameUsed` / `:100 NewChr` | 只查 `Roles` 表、只 `IsHero=0` | 原文查重是 `HumanDB.GetID(name) <> NO_ID` **or** `HeroDB.GetID(name) <> NO_ID`（:964），跨 Human+Hero |
| `:125 DelChr(account, chrName)` | 无等级限制 | 原文先 `GetBaseInfo` 判 `Level > g_nCanDeleteHumanLowLevel(45)` ⇒ nCode −3 |
| `:138 LoadHum` / `:157 SaveHum` / `:172 LoadHero` / `:189 SaveHero` | M2 数据端 | **保持**，这部分由 `ProcessM2Data:232-306` 负责，与本单元无关 |

### 6.3 需要在宿主里赋值的接缝（**全部默认抛 `NotSupportedException`，不静默**，台账 §25.2）

| 接缝 | 精确签名 | 接什么 |
|---|---|---|
| `TSelectClient.SendTextSink` | `Action<TSelectClient, byte[]>` | `TcpLink.Send` |
| `TSelectClient.RemoteAddressSink` | `Func<TSelectClient, string>` | 对端 IP（`uFrmMain.pas:516 Self.RemoteAddress`） |
| `IDSocCliSeam.FrmIDSoc` | `ITFrmIDSoc?` | IDSocCli.pas 的 `TFrmIDSoc`（未移植单元） |
| `SelectClientRoleDbSeam.HumanDB` | `THumanDBBase?` | `TMySqlRoleDB.HumanDB`（或 RoleDatabase 的实现） |
| `SelectClientRoleDbSeam.HeroDB` | `THeroDBBase?` | `TMySqlRoleDB.HeroDB` |
| `SelectClientDbShareSeam.CheckChrName` | `Func<string,bool>` | DBShare.pas:1204（**入参：字节串**） |
| `SelectClientDbShareSeam.CheckSpecialChar` | `Func<string,bool>` | DBShare.pas:1251（**入参：GBK 文本**） |
| `SelectClientDbShareSeam.CheckDenyChrName` | `Func<string,bool>` | DBShare.pas:1043（字节串） |
| `SelectClientDbShareSeam.CheckNumberName` | `Func<string,bool>` | DBShare.pas:1103（字节串） |
| `SelectClientDbShareSeam.CheckLetterName` | `Func<string,bool>` | DBShare.pas:1124（字节串） |
| `SelectClientDbShareSeam.CheckFilterNewHumanChrName` | `Func<string,bool>` | DBShare.pas:1058（字节串） |
| `SelectClientDbShareSeam.GateActiveRouteIP` | `delegate string (string, out int)` | DBShare.pas:751-870（**约 120 行，未移植**） |
| `SelectClientDbShareSeam.CheckActiveRunGate` | `Func<string,int,bool>` | DBShare.pas:731-749 |
| `SelectClientModuleSeam.UpdateModuleBuffer` | `Action<IntPtr,string>` | DBShare.pas:98-100 `AddModule/UpdateModule`（本配置下 `m_Module` 恒 nil） |
| `SelectClientGlobals.g_FirstName/g_LastName` | `TStringList` | DBShare.pas:244-245 |
| `SelectClientGlobals.g_boDynamicIPMode/g_boShowQuryChrLog/g_nCreateHumCount` | `byte/byte/int` | DBShare.pas:152 / :174 / :203 —— **建议并入只读的 `DBShareSeam.cs`** |

---

## 7. 发现的原文缺陷 / 易错点

| # | 位置 | 内容 | 影响 | 用例 |
|---|---|---|---|---|
| **B1** | `SelectClient.pas:1182-1237` | **`QueryChr` 恒返回 `False`**：`Result` 只在 :1190 赋 `False`，成功路径从不置真 | `DeCodeUserMsg:742-745` 的 `boChrQueryed := True` **永不执行** ⇒ ① CM_QUERYCHR 的 `not boChrQueryed` 门永远为真（200ms 节流一直是唯一屏障）；② CM_SELCHR 的 `if not boChrQueryed` **永远进得去**，"Double send _SELCHR" 分支**不可达** | `QueryChr_恒返回False_…`、`QueryChr_恒返回False的连带后果_…` |
| **B2** | `:1043-1081` | **`GetBackDelChr` 恒返回 `False`**：`Result` 只在 :1048 赋 `False`；`:1064 nCode := 1` 成功也不改它 | 当前调用点 :869 丢弃返回值 ⇒ **暂无行为差异**；一旦有人使用返回值即出错 | `GetBackDelChr_恒返回False_即使成功也是如此` |
| **B3** | `:350-382` vs `:444-538` | **本端发出的 keepalive `%++$` 在收包侧是"未知命令"**（首字符 `'+'` 不在 :466 的 case 里，`'-'` 才是心跳） | 对端若回 `%++$`，本端会累积未知命令计数，两次后**清空整个接收缓冲** | `ExecGateBuffers_收到的百分号加号加号美元是未知命令`、`…未知命令两次之后停止解析后续帧` |
| **B4** | `:484` | `if Pos('!', s10) < 1 then Continue;` —— `Continue` 绑定的是 **`for`**（不是"结束本条帧"） | 同一 ConnID 在表里出现多次时，**没有 `'!'` 的载荷会被追加到每一个匹配槽**；有 `'!'` 才 `Break` | `帧A_载荷没有感叹号_会继续把载荷追加到后续同ConnID槽` / `帧A_载荷带有感叹号_命中第一个槽后立即停止` |
| **B5** | `:142-168` | `Add` **不检查 `DeleteList` 里的下标是否已在 `OnLineList`**，也**不写 `Socket`** | ① 直接连调 `Add` 恒返回同一个下标并重复入表；② 重复 `Finalize(同一 Index)` 后 `Add` 会产生重复项 | `Add_不写Socket_故连续调用恒返回0并重复入表`、`Finalize_Index_重复调用同一槽会让回收表出现重复下标` |
| **B6** | `:248` | `Finalize(Index)` **无条件** `DeleteList.Add(Pointer(Index))`（不查重） | 与 B5 复合 | 同上 |
| **B7** | `:197-214` vs `:170-195` | **`Finalize`（无参）与 `Initialize`（无参）看起来一样、实则不同**：`Finalize` 只清 7 个字段与**不清两张表**；`Initialize` 清 14 个字段 + 清表 | 若把二者归一，会把 `nSessionID`/`boChrQueryed`/`dwChrTick`/`nSelGateID` 一并清掉 ⇒ `CloseUser` 后再接入的槽会带错误状态 | `Finalize_只清7个字段_…`、`Initialize_与_Finalize_字段集差异_逐字段锁定` |
| **B8** | `:895/:897` | `Random(g_FirstName.Count - 1)` —— 参数是 **`Count - 1`**（不是 `Count`） | 名单 ≥2 项时**最后一项永远取不到**；`Count = 0` ⇒ `Random(-1) = 0` ⇒ 索引空表**抛异常** | `RandomName_最后一项永远取不到_…`、`RandomName_名单为空时抛异常_…` |
| **B9** | `:756-758` / `:770-775` | **`CM_RANDOMNAME` 的 tick 节流整段被注释掉** | 与 NEWCHR/DELCHR 的 1000ms、QUERYCHR 的 200ms **不对称**：随机名可被无限刷 | `随机名_即使tick没变化也照常处理_原文没有节流` |
| **B10** | `:1152` vs `:58` | `GateActiveRouteIP(m_sGateAddr, …)` 用的是 **`m_sGateAddr`**（大写 A），字段声明是 `m_sGateaddr` | Delphi 大小写不敏感 ⇒ 同一字段；**移植到 C# 时若照抄会编译失败或误建第二个字段**（本车道已合并为同一个 `m_sGateaddr`） | `SelectChr_主动网关模式_走GateActiveRouteIP接缝` |
| **B11** | `:24` + `:699` | 字段 `nSelGateID: ShortInt`（有符号字节），从 `m_nGateID: Integer` 赋值 | Delphi 默认 `{$R-}` ⇒ **静默取低 8 位**（本车道用 `unchecked((sbyte)…)` 保留）；且全工程 `m_nGateID` **只被赋 0**（uFrmMain 里那段赋值被注释掉了） | `帧O_接入用户_解析两级IP并占槽`（断言 `nSelGateID == 0`） |
| **B12** | `:1014` | `QueryDelChr` 的 `if Length(sData) > 0` 判的是**编码后的入参**，不是解码后的账号 | 语义上"入参为空就不查库"，与"账号为空"不是一回事 | `QueryDelChr_入参为空时不查库直接回包` |
| **B13** | `:849-876` | `CM_QUERYDELCHR` / `CM_GETBACKDELCHR` 的**函数返回值被丢弃**；且这两个分支**不做 `CheckSession`** | 与 NEWCHR/DELCHR/SELCHR/RANDOMNAME 的安全模型不一致 | `分派表…` 组 |
| **B14** | `:854/:868` 与 `:741/:782/:805` | `dwChrTick` 是**每槽一个**、被每一条进入节流分支的命令刷新 | 同一批收到两条节流命令时，**第二条必然被判为 Hacker** | `ProcessUserMsg_同一槽里两条连续帧_第二条被第一条刷新的dwChrTick挡住` |
| **B15** | `:1030-1032` | `QueryDelChr` 每条记录都从**未初始化的栈上 `TDeleteHumanInfo`** 取 `SizeOf` 字节（只赋 4 个字段） | 依赖 `sChrName` 的短串写满 15 字节；托管侧 `ShortStr.Set` 会零填充，故用 `default` 等价（已在代码注释与本报告登记） | `QueryDelChr_每条记录按21字节结构编码` |
| **B16** | `:690` | `OpenUser` 在 `Initialize(nIndex)` **之后**把 `nIndex := nIndex` 写回 | 说明 `Initialize` 会把 `nIndex` 置 −1，必须回填；`CloseUser` 依赖它调 `Finalize(UserInfo.nIndex)` | `帧O_接入用户_解析两级IP并占槽` |

---

## 8. 与原文的偏离登记（建议由集成方转入 `并行派发台账.md`，本车道无权写该文件）

| 编号 | 偏离点 | 原文行为 | 托管行为 | 为什么必须偏离 | 恢复途径 |
|---|---|---|---|---|---|
| **D-p7-1** | `TSelectClient` 构造签名 | `Create(Socket: TSocket; ServerWinSocket: TServerWinSocket)` | `TSelectClient()` 无参 | 首参是 WinSock 句柄、次参是 VCL `TServerWinSocket`，两者都不存在；且本单元**自身即 socket 对象**（`OnGetSocket` 造它），托管侧用接缝基类承接 | ServerClient.pas 移植后按原签名重载 |
| **D-p7-2** | `SendUserSocket(sSessionID, sSendMsg)` 第二参 | `string`（承载协议字节的 AnsiString） | `byte[]` | 转换开发文档 §3.1「协议层一律 byte[]」；这样单测能**逐字节**锁定组包结果（拼接顺序/分隔符一字未改） | 无需恢复（表示层映射） |
| **D-p7-3** | `TSelectChar.Finalize` / `Finalize(int)` 命名 | `Finalize` | **保留原名** + `#pragma warning disable CS0465, CS0114` | 与 C# `object.Finalize` 同名；本仓既有先例是改名（`FinalizeStatement`），本车道选择**保名 + 抑制警告**以便 1:1 对照（两者无行为差异、零新增警告） | 若统一到改名惯例，全套 `Finalize(` → `FinalizeItems(` |
| **D-p7-4** | `ExecGateBuffers(PChar, BufLen)` 的循环体 | :458-537 与 :555-635 **两段近乎逐字重复**的解帧循环 | 字节重载**转调** string 重载 | 行为完全等价（首句 `m_sReceiveText := m_sReceiveText + sReceiveText` 同型），不复制 80 行；且该重载在 `DBSUSETHREAD = 0` 下**不可达** | 若把 `DBSUSETHREAD` 打开做线程化，可原样复制回两段 |
| **D-p7-5** | `TUserInfo` 表示 | `record` + `pTUserInfo` 指针 | `sealed class` | 原文**从不按值复制**，只经指针就地改写；`struct` 会丢别名语义（与 `MySqlRoleDB.Seam.cs` 的 `TQueryHumanData` 同一处置） | 无需恢复 |
| **D-p7-6** | `OnLineList` / `DeleteList` | `TList`（存 `Pointer(下标)`） | `List<int>` | 托管侧无 `Pointer`；语义逐条对应（含 `Remove` 只删首个、`Delete(0)` = `RemoveAt(0)`） | 无需恢复 |
| **D-p7-7** | 计时源 | Windows `GetTickCount` | **可注入的 `DelphiTick.GetTickCount`**（默认即 `DelphiRTL.GetTickCount()` → `Environment.TickCount`） | 与 `MySqlRoleDB.Role.cs` 等既有产物的口径一致，单测可钉死时钟（本车道的节流用例全部依赖它） | 无需恢复 |
| **D-p7-8** | `QueryDelChr` 的局部 `TDeleteHumanInfo` | **未初始化**的栈上记录 | `default`（零填充） | C# 的明确赋值规则不允许未初始化结构体；`ShortStr.Set` 本身就会把名字缓冲补零，故二者等价（且原文那 21 字节里唯一可能泄漏的是已被覆盖的 4 个字段 + 零填充的名字） | 无需恢复 |
| **D-p7-9** | `AnsiString` 表示 | 字节串 | **latin-1 逐字节字符串**（`SelectClientAnsi`），在 DB/IDSoc 边界转 GBK | 见 §4「字节语义」；这是本单元唯一能把"按字节判定"整条链路忠实搬过来的方式 | 无需恢复 |

---

## 9. 越区请求 / 需要集成方决定的事

1. **`DBShareSeam.cs` 增补**（该文件是车道4 产物，我只读）：
   `g_boDynamicIPMode` / `g_boShowQuryChrLog` / `g_nCreateHumCount` / `g_FirstName` / `g_LastName`
   目前放在本车道的 `SelectClientGlobals` 里。若后续有其它车道也要用，请授权把它们搬进 `DBShareSeam.cs`（届时删掉 `SelectClientGlobals` 对应项）。
2. **`GateActiveRouteIP` / `CheckActiveRunGate` 的正确归属**：它们是 `DBShare.pas:731-870`（约 120 行，依赖 `TRouteInfo.RunGate2List` / `TRunGateInfo`）。
   本车道只做接缝。**建议由 DBShare 车道补齐**，届时删掉 `SelectClientDbShareSeam` 的这两个委托、改为直接调用。
3. **`Checklist.md:298` 的 ✅ 应改为 ⚠（过度声明）**：实测 C# 侧原来只落了 100/101/102/103 四条命令，且字段位置与原文不符（见 §6.1）。
   （任务书给的 `:287` 实际是 ThreadPool 行；`SelectClient` 在本仓只出现于 `:298`。）
4. **`DBServerService.ProcessGateData` 的协议字段位置修正**（`:135`）若与既有集成测试期望冲突，请由集成方裁定改哪一侧。
5. **`SelectClient.pas` 的 `{$IF DBSUSETHREAD = 1}` 分支**（:263-278 / :303-348）我**没有移植**（当前根本不编译）。
   若将来要打开该开关做线程化，需要 `TServerClientThread` / `TWinSocketStream` 的接缝——那时再派车。

---

## 10. 诚实的未完成部分与剩余量

**已完成（可合并）**：`SelectClient.pas` 在 `DBSUSETHREAD = 0` 配置下的**全部实现行**，含 1000 槽会话表、收包解帧、消息分发内核、角色族七个方法；
171 条新用例、build/test 全绿、`git status` 为空。

**未完成 / 未覆盖**：

1. **:263-278 与 :303-348（`{$IF DBSUSETHREAD = 1}` 分支）—— 未移植**。理由：`DBShare.pas:19 DBSUSETHREAD = 0`，编译器根本不读这两段；且它们依赖 `TServerClientThread` / `TWinSocketStream` / `Application.Terminated` / `ClientSocket`。
   任务书里点名的 `ClientExecute`(:84)、`Close`(:85) 与 PChar 重载的**唯一调用点**(:330) 都在这两段里 ⇒ **在本配置下不可达**，这一点已在报告与代码注释里显式登记。
2. **`TSelectClient` 与真实 socket 的端到端联调 —— 未做**（无头不可验证）。`SendTextSink` / `RemoteAddressSink` 默认**抛异常**，所以一旦有人按本文 §6.3 接线就能立刻发现漏接；在此之前**任何真实收包都会抛 `NotSupportedException`**（这是刻意的，符合台账 §25.2）。
3. **`DBShare.pas` 的 6 个名校验函数、`GateActiveRouteIP`、`CheckActiveRunGate`、`IDSocCli.pas` 的 `TFrmIDSoc` —— 未移植，只有接缝**。因此：
   * `NewChr` 的**真实**校验结论无法在本车道验证（单测用桩替代，验的是**分支顺序与 nCode 语义**，不是校验算法本身）；
   * `CM_SELCHR` 的主动网关路线（`g_boUseActiveRunGage = 1`，默认 False）无真实现；
   * `CheckSession` / 会话状态机的真实行为未验证。
4. **`RoleDatabase.cs` / `DBServerService.cs` 的接线本身 —— 我没动**（只读区）。所以**当前跑起来的 DBServer 仍然只有那 4 条命令**，本车道的成果要等集成方按 §6 接线后才生效（或者说：本车道交付的是"可接线的正确实现"，不是"已接线的服务"）。
5. **`SelectClientAnsi` 是 `internal`**，测试只能间接覆盖（通过公开入口）；其正/负边界由 `NewChr` 的两个字节语义用例与 `帧A` 的载荷用例间接锁定。

---

## 11. 接线执行（切片7，`5b3bc2e0`）与**待授权的越区路径**

### 11.1 已完成（分区内，可合并）

| 产物 | 内容 |
|---|---|
| `src/GXX.DBServer/SelectClient.GateWiring.cs` | `SelectClientGateWiring`：一条 SelGate 连接 ↔ 一个 `TSelectClient`（1000 槽会话表）的绑定、出站按实例路由、收包喂入。对应 `uFrmMain.pas:303-306`(OnGetSocket) / `:335-338`(OnClientDisconnect) / `:340-353`(OnClientRead) |
| `tests/…/SelectClientGateWiringTests.cs` | **20 例**：接线/断线、两连接互不串扰、RemoteAddress、半包、带偏移切片、未接线抛异常，以及 **SelGate 命令路径的第一批端到端用例** |

`DBServerService` 侧届时只需 3 行：
```csharp
int idx = _gateLinks.Count + 1;
link.OnDisconnected += () => _gateLinks.TryRemove(idx, out _);
_gateLinks[idx] = link;
string remote = (client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "";
SelectClientGateWiring.AttachTcpLink(link, remote);     // ← 新增的唯一一行
```
`StopService()` 里加一行 `SelectClientGateWiring.DetachAll();`。

### 11.2 ★ 授权状态（台账 §31.4：授权必须可由仓库状态自查）

```
git show main:GXX.CSharp/tools/lane-zones.tsv | Select-String p7-db-selectclient
→ p7-db-selectclient  !GXX.CSharp/src/GXX.DBServer/SelectClient*.cs;!GXX.CSharp/tests/GXX.DBServer.Tests/SelectClient*;!GXX.CSharp/docs/并行报告-p7-db-selectclient.md
```

**`DBServerService.cs` 与 `RoleDatabase.cs` 不在本车道分区**（三条 `!` 里没有它们）；
集成方的任务书写的是"若不在你现有分区，请先告诉我需要哪几个路径" ⇒ 本报告 §11.4 即该清单。

### 11.3 §28.3 双根侦察（`src` + `tests` 同时搜）——结论：`:135` 的修正**没有测试冲突**

| 搜索项 | `src` | `tests` |
|---|---|---|
| `DBServerService` | 自身 + `Program.cs:26/30`（仅构造） | **0** |
| `RoleDatabase` | 自身 + `DBServerService.cs:22/44/47` | **0** |
| `SM_QUERYCHR`/`SM_NEWCHR`/`SM_DELCHR`/`SM_STARTPLAY`/… 字面量 | `DBServerService.cs:135/144/153/164/169` | **0**（除本车道 `SelectClient*` 自己的用例） |
| 数字常量 520–527 | `DBServerService.cs:135-169` | **0** |

`GXX.Integration.Tests` 只有 2 例，均在 LoginGate / LoginSrv（`LoginGateIntegrationTests.cs`、`LoginSrvIntegrationTests.cs`），**不碰 DBServer**。

* ⇒ 集成方给的停止条件（"若 `:135` 与既有集成测试冲突"）**未触发**：没有任何既有断言依赖现行的 `Param=n>0 / Tag=0`。
* ⇒ 反向也成立：**现行的 4 条命令一条测试都没有**；删除 `ProcessGateData` 不损失任何覆盖，而本切片的 20 例正是接管它的验收网
  （`端到端_CM_QUERYCHR_角色数在Recog且Tag为1`、`端到端_CM_QUERYCHR_角色数落在Recog上`、`端到端_未知命令回SM_CHECKISMYSELFSERVER`）。

### 11.4 需要的越区路径（精确两项）

1. `GXX.CSharp/src/GXX.DBServer/DBServerService.cs`
2. `GXX.CSharp/src/GXX.DBServer/RoleDatabase.cs`

### 11.5 已核实的连带死代码（同文件内，删除 `ProcessGateData` 后）

`DBServerService.cs` 的 `ToByte`(:183)、`ExtractBodyText`(:185-191)、`SendReply`(:193-206)
**只被 `ProcessGateData` 的 4 个 case 调用**；`src` + `tests` 全仓无其它调用点（§28.3 双根已搜）。
⇒ 接线时应一并删除（`SendM2Reply` 是 M2 数据端专用，**保留**）。

### 11.6 ★★ 需要裁定：两个依赖**根本没有本体**，只有接缝

* `IDSocCliSeam.FrmIDSoc` ← `IDSocCli.pas`（会话状态机 / LoginSrv 客户端）**未移植**，默认抛 `NotSupportedException`。
* `SelectClientDbShareSeam.CheckChrName / CheckSpecialChar / CheckDenyChrName / CheckNumberName / CheckLetterName / CheckFilterNewHumanChrName` ← `DBShare.pas:1043-1280` **未移植**，默认抛 `NotSupportedException`。

⇒ 若严格按台账 §25.2「接缝不得静默返回中性值」只接**已有**依赖，**服务一收到 `CM_QUERYCHR` / `CM_NEWCHR` 就会抛异常**。
三条路，请裁一条：

| 方案 | 行为 | 代价 |
|---|---|---|
| **(a) 保持抛异常** | 接线完成、服务可起，但 `CM_QUERYCHR`/`CM_NEWCHR` 抛 `NotSupportedException`（= 显式"未实现"） | 要等 IDSocCli / DBShare 车道补齐才真正可用 |
| **(b) 显式命名的放行桩**（如 `SelectClientSeamPolicy.UnwiredPermissive`，每次调用 `MainOutMessage` 留痕） | 服务立即可用，行为对齐**现行** C#（不校验会话、名校验全放行） | **正是 §25.2 点名的"中性值"形态** ⇒ 必须登记为带编号的正式偏差 + 调用留痕，不能裸给 |
| **(c) 一并移植** `IDSocCli.pas` 会话判定 + `DBShare.pas` 名校验族 | 最忠实 | 工作量远超本车道，建议另开 1~2 条车道 |

**我没有自行选 (b)**（它是 §25.2 点名的形态，需要编号裁定）；也**没有改任何分区外文件**。

---

## 12. 接线执行完毕（切片9/10，`2f605d53` / `0dde7dad`）

### 12.1 授权与裁定（按集成方 §2 条执行）

* 授权自查：`main` 的 `a6a413cf` 已把 `!GXX.CSharp/src/GXX.DBServer/DBServerService.cs` 与
  `!GXX.CSharp/src/GXX.DBServer/RoleDatabase.cs` 加进本车道分区（`git show main:GXX.CSharp/tools/lane-zones.tsv` 可自查）。
* **裁定：方案 (a) —— 保持抛异常，不做放行桩。** 理由（集成方原文）：校验与过滤规则不是展示逻辑，
  (b) 会把"拒绝"变成"接受"；**安静的错 > 响亮的缺**；可审计 ≠ 正确。
  ⇒ 本车道**没有**为任何未移植依赖提供中性值。

### 12.2 任务书 5 项的落地情况

| 任务书 | 落地 | 位置 |
|---|---|---|
| 1. 为每个 TcpLink 建 TSelectClient / 接 SendText 与 RemoteAddress / 收包喂 ExecGateBuffers | ✅ | `DBServerService.GateAcceptLoop`（3 行）+ `SelectClient.GateWiring.cs`（`AttachTcpLink`） |
| 2. 删除 `ProcessGateData`（整体） | ✅ 已删 | 连同 `ToByte` / `ExtractBodyText` / `SendReply` 三个**只被它调用**的死方法（§28.3 双根已核实） |
| 3. 修 `:135` 协议字段位置 | ✅ 该行随 `ProcessGateData` 一并消失；正确目标由 `TSelectClient.QueryChr`（:1228 `MakeDefaultMsg(SM_QUERYCHR, nChrCount, 0, 1, 0)`）承担 | 验收基准：`SelectClientGateWiringTests.端到端_CM_QUERYCHR_角色数在Recog且Tag为1` / `…角色数落在Recog上` |
| 4. `RoleDatabase.cs:46/75/88/100/125` 对齐原文 | ✅ 见 §12.3 逐条映射 | `RoleDatabase.cs` |
| 5. 接缝赋值（能接的接、接不上的显式登记） | ✅ 接上 `HumanDB`/`HeroDB`/`SendText`/`RemoteAddress`；**未接** `FrmIDSoc` / DBShare 校验族 / 主动网关（显式抛，见 §12.4） | `DBServerService` ctor + `SelectClientRoleDbSeam.AttachRoleDatabase` |

**门禁**：`GXX.DBServer.Tests` **609/609 全绿**（接线前 562 → **+47**）；
`GXX.Integration.Tests` **2/2**；`dotnet build GXX.slnx --no-incremental` **0 error / 162 warning**
（合并 main 后基线 163，**少 1 条**：删掉了 `DBServerService.cs` 里重复的 `using GXX.GatewayKit;`，即原有 CS0105）。
`SelectClient*.cs` / `RoleDatabase.cs` / `DBServerService.cs` 新增警告 **0**。

### 12.3 `RoleDatabase.cs` 的逐条对齐（任务书第 4 项）

| 原位置 | 原文语义（`MySqlRoleDB.pas` 原样 SQL） | 处置 |
|---|---|---|
| `:46 QueryChr(account, byte[] outBuf)` | SM_QUERYCHR 包体是**文本** `[*]名/职业/发型/等级/性别/`（SelectClient.pas:1217-1222）；原实现发的是 `TDeleteHumanInfo` 6-bit 二进制 | **删除**（格式错 + 接线后零调用方）。正确包体由 `TSelectClient.QueryChr` + `HumanDB.QueryHumans` 产生 |
| `:75 ChrExists(account,name,isHero)` | 原文查重判据是 `HumanDB.GetID(name) <> NO_ID **or** HeroDB.GetID(name) <> NO_ID`（:964） | **删除**；由 `GetHumanId`+`GetHeroId` 合成（`IsHero=0` / `IsHero=1` 两次查，等价于原 `ChrNameUsed` 的 `WHERE ChrName=@c`） |
| `:88 ChrNameUsed(chrName)` | 同上 | **删除**（`GetID` 在两个适配器上合成，能力不丢） |
| `:100 NewChr(account,name,job,gender)` | `DoAdd` = `insert into Human(Account, HumanName, IsDelete, IsSelect, CreateDate, Sex, Job, Hair) values(?,?,?,?,?,?,?,?)`；查重与等级策略**不在这一层** | **收敛为 `AddHuman`**（参数顺序/取值照原文；`CreateDate = Date2MyDate(Now())`）；查重移到 `TSelectClient.NewChr`（:964）；THumData 初始化保留 |
| `:125 DelChr(account,name)` | `DoDelete` = `update Human set IsDelete = 1 where (Account = ?) and (HumanName = ?)`（**软删**） | **拆成 `DeleteHuman`/`DeleteRestoreHuman`**（`IsDelete=1/0`）；`Level > 45 → nCode -3` 在 `TSelectClient.DelChr`（:1097，已移植） |

新增的 10 个数据操作（SQL 逐条抄自 `MySqlRoleDB.SqlStatements.cs`，注释在 `RoleDatabase.cs` 里）：
`GetHumanId` / `GetHeroId` / `GetHumanCount` / `GetBaseInfo` / `QueryHumans` / `QueryDeleteHumans` /
`SelectHuman`（两条 update + 事务）/ `AddHuman` / `DeleteHuman` / `DeleteRestoreHuman`；
并加 `EnsureSchema` 的**增量列迁移**（`IsDelete` / `IsSelect` / `Hair` / `CreateDate` / `LoginDate`，
用 `PRAGMA table_info` 判缺再 `ALTER TABLE`，旧库可直接打开）。
**忠实保留的两处原文细节**：① `GetHumanId`/`GetBaseInfo` **不过滤 `IsDelete`**（删掉的名字仍被占用）；
② `QueryHumans` 做 Sex/Job 钳位、`QueryDeleteHumans` **不做**（测试锁定该差异）。

### 12.4 ★★ 未移植依赖的**精确清单**（集成方要求：哪个单元、多少行）

接线后，**以下命令会抛 `NotSupportedException`**（不是静默放行）——原因与规模如下：

| 命令 / 帧 | 抛在哪（原文行） | 缺失单元 | 规模 |
|---|---|---|---|
| `CM_QUERYCHR`(100) | `QueryChr` → `FrmIDSoc.CheckSession`（:1198） | **`IDSocCli.pas`** | **431 行**（13,763 字节，整单元未移植） |
| `CM_RANDOMNAME`(106) | `DeCodeUserMsg` → `CheckSession`（:760） | 同上 | 同上 |
| `CM_NEWCHR`(101) | `DeCodeUserMsg` → `CheckSession`（:784）；若会话通过还会撞 **`DBShare.pas` 名校验族** | `IDSocCli.pas` + `DBShare.pas` 名校验族 | 431 行 + **170 行** |
| `CM_DELCHR`(102) | `DeCodeUserMsg` → `CheckSession`（:807） | `IDSocCli.pas` | 431 行 |
| `CM_SELCHR`(103) | `DeCodeUserMsg` → `CheckSession`（:829） | `IDSocCli.pas` | 431 行 |
| **任何 `%X` 帧（用户离开）** | `CloseUser` → `FrmIDSoc.GetGlobaSessionStatus`（:714） | `IDSocCli.pas` | 431 行 |
| `g_boUseActiveRunGage = True` 时的 `CM_SELCHR` | `SelectChr` → `GateActiveRouteIP`（:1152）/ `CheckActiveRunGate`（:1158） | `DBShare.pas:731-848` | **117 行** |

**`DBShare.pas` 名校验族的精确行号**（`SelectClient.pas` 实际调用的 6 个 + 1 个依赖）：

| 函数 | 行号 | 行数 |
|---|---|---|
| `CheckDenyChrName` | :1043-1056 | 14 |
| `CheckFilterNewHumanChrName` | :1058-1077 | 20 |
| `CheckNumberName` | :1103-1122 | 20 |
| `CheckLetterName` | :1124-1143 | 20 |
| `CheckCanCaseChar`（被 `CheckChrName` 调用） | :1177-1202 | 26 |
| `CheckChrName` | :1204-1249 | 46 |
| `CheckSpecialChar` | :1251-1274 | 24 |
| **合计** | | **170 行** |
| 另需 `LoadChrNameList`（装载 `g_DenyChrNameList` / `g_FilterNewHumanNameTextList`） | :403-425 | 23 行 |
| 另需 `GateActiveRouteIP` :751-848（98）+ `CheckActiveRunGate` :731-749（19） | | 117 行 |

⇒ **要让它"全命令可用"，后续车道需要移植约 `431 + 170 + 23 + 117 ≈ 741 行`**（`IDSocCli.pas` 整单元优先，它一条就挡住 6 项）。
（`DBShare.pas:705-729 GateRouteIP` 已在 `DBShareSeam.cs` 移植 ✅，所以**默认路由模式**的 `CM_SELCHR` 只差 `IDSocCli`。）

**已经能真正跑通的**（已用端到端用例锁定）：`%-` 心跳、`%O` 接入、`%X`（**不命中槽位**时）、
`CM_QUERYDELCHR`(105)、`CM_GETBACKDELCHR`(3006)（原文这两条本就不做 `CheckSession`），
以及 else 分支的 `SM_CHECKISMYSELF SERVER`。

### 12.5 新增偏差登记

| 编号 | 偏离点 | 原文/旧行为 | 新行为 | 登记理由 |
|---|---|---|---|---|
| **D-p7-10** | `RoleDatabase` 删掉 `QueryChr`/`ChrExists`/`ChrNameUsed`/`NewChr`/`DelChr` 五个方法 | 为旧 `ProcessGateData` 的 4 条命令而写，语义与原文不一致（包体格式、查重范围、无等级门） | 换成 §12.3 的 10 个操作 | 接线后这 5 个**零调用方**（§28.3 双根已核实）；"零调用方的错实现"比"缺实现"更危险 |
| **D-p7-11** | `SelectClientGateWiring.AttachTcpLink` 的 `OnReceive` 里加了宿主**异常边界** | 原文 `ExecGateBuffers` 无 try/except，但它的调用者是 **VCL 事件回调**，异常由 `Application.HandleException` 兜住（不静默、也不炸进程） | `FeedSafe` 捕获 → 写 `MainOutMessage("[ERROR] SelectClient 命令处理抛异常：…")` → **返回异常给宿主** → 宿主 `Detach` + `link.Close()` | 托管侧 `TcpLink.ProcessReceive` 的 `OnReceive?.Invoke` **没有** try，异常会升级为 IOCP 回调线程上的**进程级未处理异常**。**不是"吞"**：日志留全 + 主动断连（fail closed）。薄直通版 `Feed` 仍在，测试锁定"不加边界就原样抛" |
| **D-p7-12** | `SelectClientHumanDb`/`SelectClientHeroDb` **只**实现 `SelectClient.pas` 用到的 Do*（Human 9 个 / Hero 1 个） | `THumanDB`/`THeroDB` 各有 27/11 个 Do* | 其余抛 `NotSupportedException` | 刻意收窄，避免把半个 HumanDB 伪装成通用 g_RoleDB。**注意**：`THumanDBBase` 的公开包装会 `catch` 并把异常降级成"日志 + 初值"（`MySqlRoleDB.Base.cs:17-27`，本车道无权改）—— 该行为已被 `适配器_未接线的Do成员被THumanDBBase包装吞成日志与初值` **锁死**，以免后来人误以为它会抛 |

### 12.6 ★ 第 17 条发现（本次接线时被它绊倒）：`THumData` 是巨型结构，值复制会**栈溢出**

* `THumData`（`Grobal2.Types4.cs:128`，`Pack=1`）含 `TUserItemArray206 BagItems`、
  `TUserItemArray196 StorageItems`、`ShortStr100Array500 TValues/ZValues`（各 500×101 字节）、
  `IntArray500 UValues/JValues`、`TSaveNpcSkillPowerAddArray550` … ⇒ **`StructBytes.SizeOf<THumData>() > 10000`**。
* 我在适配器测试里写了 `db.LoadHum(...)` 返回 `THumData?`，然后连续访问 `hum.Value.X` 8 次
  —— **每次 `.Value` 都是一整块值复制** ⇒ **测试宿主 "Stack overflow" / `Test Run Aborted`**
  （477 例通过后中止，无失败列表，只在 stderr 留一行 `Stack overflow.`）。
* 修法：按本仓既有约定（`GXX.Core.Tests/CoreTests.cs:194-198` 的注释已经点明"THumData 为巨型结构，
  CLR 不允许创建其数组元素"）改走 `Unsafe.As<byte, THumData>(ref wire[0])` 别名。
* **对生产代码的影响**：`RoleDatabase.LoadHum/SaveHum` 与 `ProcessM2Data` 各只做**一次**复制，安全；
  但**任何"在栈上多复制几次 THumData"的写法都要先想想**。已把这条固化成断言
  `SelectClientRoleDbAdapterTests.THumData是巨型结构_值复制是真实的栈开销`。
* **规程建议**：**"测试宿主的 `Test Run Aborted` + stderr 的 `Stack overflow.`"要当成一等失败处理**
  ——它不会出现在失败列表里，`Passed!` 数字还会变小（本次 609 → 477），只看汇总行会误判成"全绿"。

### 12.7 仍未做（诚实）

1. **真实 socket 端到端联调未做**：`GateAcceptLoop` 的 `Accept`→`AttachTcpLink` 路径没有自动化用例
   （既有 `GXX.Integration.Tests` 有 TOCTOU 端口竞态的历史，不引入新的网络测试）。
   覆盖到的是**同一条路径的等价层**：`SelectClientGateWiringTests`（20+4 例）+ `SelectClientHostWiringTests`（17 例）。
2. `%X` 会断整条 SelGate 连接（§12.4 末行）——这是裁定 (a) 的**已知代价**，若集成方认为
   "`CloseUser` 里的 IDSoc 只是清理通知"可以单独放宽，请给编号；**本车道没有自行放宽**。
3. `IDSocCli.pas` / `DBShare.pas` 名校验族 / 主动网关路由**仍未移植**（规模见 §12.4）。

---

## 13. 第 2 轮：`%X` 窄口子（D-p7-13）落地 + `IDSocCli.pas` 移植侦察

### 13.1 ✅ 裁定执行：**只对 `CloseUser` 的 `GetGlobaSessionStatus` 放宽**（偏差 D-p7-13）

集成方**细化**了上一轮的 (a)：`%X` 的这一步是**清理通知**（"会话已失效 → 叫 LoginSrv 关掉它"），
**不是校验判定**；而它抛出去的代价是**断开整条 SelGate 连接**（一条连接上通常挂着多个无关玩家）——
**一个缺失的清理动作，代价是一批玩家被踢**。⇒ 只此一条路径允许"记日志 + 跳过清理 + 继续"。

| 要求 | 落实 |
|---|---|
| ① **命名清晰的窄口子**，不要通用"放行开关" | `IDSocCliSeam.CloseUser_ShouldCloseSession(int nSessionID)` —— 名字直接点明**调用方**（CloseUser）与**唯一用途**（该不该关会话）；没有 `bool Permissive` 之类的开关，也**没有默认值可配**。调用点只有一个：`SelectClient.CloseUser`（:419 附近） |
| ② **注释写明为什么这里可以跳过而别处不行** | 方法 XML 注释里逐条列出集成方裁定的 3 条理由（非校验判定 / 后果不成比例 / 其余接缝一条都不放宽），并注明这是**偏差 D-p7-13** |
| ③ **每次触发都留痕** | 未接线时每次都写 `MainOutMessage("[WARN] 接缝未接线（D-p7-13）：… nSessionID=" + id)` —— 带**偏差编号**便于审计计数 |
| ④ **登记为带编号的正式偏差** | 见下表 |
| ⑤ **两条用例改成锁定新行为 + 保留"不命中槽位"那条** | `帧X_命中槽位时不抛_跳过清理_留痕_且不断连接`（新）· `帧X_命中槽位但FrmIDSoc已接线时_回到原文分支`（新，对照组）· `帧X_会话仍活跃时不做清理`（新）· `帧X_不命中槽位时不抛`（保留） |

**偏差登记 D-p7-13**

| 项 | 内容 |
|---|---|
| **偏离点** | `SelectClient.CloseUser` 里"要不要给 LoginSrv 发会话清理通知"这一步 |
| **原文行为** | `if not FrmIDSoc.GetGlobaSessionStatus(nSessionID) then begin SendSocketMsg(SS_SOFTOUTSESSION,…); CloseSession(…); end;`（SelectClient.pas:714-718）——`FrmIDSoc` 恒存在，两步必定按状态执行 |
| **托管行为** | `FrmIDSoc` **已接线**时与原文**完全一致**（用例 `帧X_命中槽位但FrmIDSoc已接线时_回到原文分支` / `帧X_会话仍活跃时不做清理` 锁定）；**未接线**时 `CloseUser_ShouldCloseSession` 返回 `false` ⇒ **跳过清理**并写一条 `[WARN] …（D-p7-13）` 日志 |
| **为什么必须偏离** | 未接线让它抛 ⇒ 异常经 `SelectClientGateWiring` 的宿主边界被接住 ⇒ **断开整条 SelGate 连接**（多玩家受影响）。而这一步只是**清理通知**，不是校验判定："少做一次清理"与"接受本应拒绝的输入"后果不成比例 |
| **边界（明确不放宽的部分）** | `CheckSession`（`CM_QUERYCHR`/`CM_RANDOMNAME`/`CM_NEWCHR`/`CM_DELCHR`/`CM_SELCHR`）、`DBShare.pas` 名校验族、`GateActiveRouteIP`/`CheckActiveRunGate` —— **一条都不放宽**，仍然是 `NotSupportedException`；守卫用例：`依赖IDSocCli的五条命令_目前抛NotSupportedException`、`DBShare名校验接缝_未接线_访问即抛NotSupportedException`、`主动网关路由接缝_未接线_访问即抛NotSupportedException` |
| **恢复途径** | `IDSocCli.pas` 移植并接线（`IDSocCliSeam.FrmIDSoc` 非 nil）后，本口子**自动失效**（方法内 `if (FrmIDSoc != null) return !FrmIDSoc.GetGlobaSessionStatus(...)`）；随后**删除** `CloseUser_ShouldCloseSession`、`CloseUser` 改回 `if (!IDSocCliSeam.Require.GetGlobaSessionStatus(...))`，并**注销 D-p7-13** |

### 13.2 ⚠ 我的行数口径错误（自我更正）

上一轮 §12.4 我写 **`IDSocCli.pas` 431 行** —— **错了**。实测：

| 口径 | 值 |
|---|---|
| **总行数**（`[regex]::Matches($t,"\n").Count`） | **464 行** |
| 非空行（`Get-Content \| Where-Object { $_.Trim() -ne '' }`） | 431 行 |
| `Get-Content \| Measure-Object -Line` | 431 ← **我当时用的就是这个，它计的是非空行** |

⇒ **正确数字是 464 行**（`DBShare.pas` 的**函数行区间**是 `read` 工具按含空行给的，那些是准的，不用改）。
**规程**：报"某单元多少行"必须用**总行数**，且用 `read` 工具或 `[regex]::Matches($t,"\n").Count` 复核；
`Measure-Object -Line` **不报空行**，不能当总行数用。

### 13.3 `IDSocCli.pas`（464 行）移植侦察 —— ✅ 结论：**依赖比预想的少得多**

**已经具备、不用碰的**（这是好消息，说明它不是"要带一串依赖"的单元）：

| 依赖 | 现状 |
|---|---|
| `TGlobaSessionInfo` / `pTGlobaSessionInfo`（Grobal2.pas:4048-4061） | ✅ **已移植**：`GXX.Core.Protocol.TGlobaSessionInfo`（`Grobal2.Types5.cs:795`），字段逐一对应（含 `n24` / `bo28` / `boHeroLoadRcd` / `dwAddTick` / `dAddDate`） |
| `SS_OPENSESSION` / `SS_CLOSESESSION` / `SS_KEEPALIVE` / `SS_SERVERINFO` | ✅ **已移植**：`GXX.Core.CommonConst` = **1000 / 1010 / 1040 / 1030** |
| `ArrestStringEx` / `GetValidStr3` / `StrToIntDef` | ✅ `GXX.Core.Util.HUtil32` + `GXX.Core.Rtl.DelphiRTL` |
| `SameText` | ✅ `DelphiStrUtils.SameText` |
| `Format('%d/%s')` / `Format('%s:%d → %s:%d')` | ✅ `GXX.Core.Rtl.DelphiFormat.Format`（§17.2 规程） |
| `Now`（TDateTime） / `GetTickCount` | ✅ `DelphiDate.Now()` / 可注入的 `DelphiTick.GetTickCount()` |
| `MainOutMessage` | ✅ `RoleDbSeam.MainOutMessage` |

★ **一处原文陷阱**：`case nIdent of SS_OPENSESSION {100}` 里的 **`{100}` 注释是陈旧的**（真值是 **1000**，
`SS_CLOSESESSION {101}`→**1010**、`SS_KEEPALIVE {104}`→**1040**）。⇒ **1:1 照抄 case 标签，不要照抄注释里的十进制值**。

**需要的新接缝（精确签名）**——共 4 组，都是"宿主面"而不是"未移植算法"：

```csharp
// 1) JSocket/TClientSocket（未移植）：本单元只用到 8 个成员
public interface IIDSocClientSocket            // 对应 IDSocket: TClientSocket
{
    bool Active { get; set; }                  // OpenConnect/CloseConnect/Timer1Timer
    string Address { get; set; }               // OpenConnect/Timer1Timer（g_sIDServerAddr）
    int Port { get; set; }                     // OpenConnect/Timer1Timer（g_nIDServerPort）
    bool Connected { get; }                    // IDSocket.Socket.Connected
    void SendText(string sMsg);                // IDSocket.Socket.SendText
    string ReceiveText { get; }                // IDSocketRead → Socket.ReceiveText
    string RemoteAddress { get; }              // IDSocketConnect
    int LocalPort { get; }                     // IDSocketConnect（模块地址串）
    int RemotePort { get; }                    // IDSocketConnect
    void Close();                              // IDSocketError → Socket.Close
}

// 2) TTimer（未移植）：只用 Enabled
public static Action<bool> Timer1Enabled;          // Timer1.Enabled
public static Action<bool> KeepAliveTimerEnabled;  // KeepAliveTimer.Enabled

// 3) uFrmMain.pas:434-453 GetSelectCharCount（DBSUSETHREAD=0 分支 = SelectSocket.Socket.ActiveConnections）
public static Func<int> GetSelectCharCount;

// 4) DBShare.pas 全局 + 模块表（均未移植）
public static string g_sIDServerAddr = "127.0.0.1";   // DBShare.pas:129
public static int    g_nIDServerPort  = 5600;         // DBShare.pas:128
public static string g_sServerName    = "GeeM2";      // DBShare.pas:131
public static IntPtr AddModule(string moduleName, string address, string buffer);  // DBShare.pas:98
public static void   RemoveModule(IntPtr module);                                  // DBShare.pas:99
```

**纯逻辑占比很高**（可高覆盖测试）：`GlobaSessionList` 的增删查（`ProcessAddSession` / `ProcessDelSession`）、
9 个会话查询/变更（`CheckSession` / `CheckSessionLoadRcd` / `CheckSessionHeroLoadRcd` / `SetSessionSaveRcd` /
`SetGlobaSessionNoPlay` / `SetGlobaSessionPlay` / `GetGlobaSessionStatus` / `CloseSession` / `GetSession`）、
解包循环（`ProcessSocketMsg`）、组包（`SendSocketMsg`）。
真正只剩接缝的是：`FormCreate/FormDestroy`、`Timer1Timer`、`IDSocketRead/Error/Connect/Disconnect`、
`SendKeepAlivePacket`、`OpenConnect`/`CloseConnect`。

### 13.4 ★ 需要的分区（**待授权，本车道未动**）

自查：`main` 的 `lane-zones.tsv` 里本车道是
`!…/SelectClient*.cs;!…/DBServerService.cs;!…/RoleDatabase.cs;!…/tests/…/SelectClient*;!…/docs/并行报告-p7-db-selectclient.md` —— **不含 `IDSocCli*`**。

| 需要授权的路径 | 用途 |
|---|---|
| `GXX.CSharp/src/GXX.DBServer/IDSocCli*.cs` | 按转换开发文档 §3.3（文件名与原 .pas 单元名一致）新建 `IDSocCli.cs` —— **不把别人的单元塞进 `SelectClient*.cs` 里凑路径** |
| `GXX.CSharp/tests/GXX.DBServer.Tests/IDSocCli*` | 对应测试 |

（若集成方更希望**直接扩成 `GXX.DBServer/**`**，请一并告知；否则上面两条即可。）

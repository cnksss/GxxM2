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

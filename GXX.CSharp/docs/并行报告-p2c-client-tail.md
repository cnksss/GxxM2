# 并行报告 · 车道 `par/p2c-client-tail`（Client-HGE「小单元尾巴」）

> 分支：`par/p2c-client-tail` ｜ 工作树：`.worktrees/p2c-client-tail` ｜ 日期：2026-09-20
> 任务：`Source/Client-HGE/**` 约 20 个"小单元尾巴"（≈8,000 行）的 Delphi 7 → C# 1:1 移植
> 命名空间：`GXX.Client.Tail`（跟随 `GXX.Client` 既有惯例：一级子命名空间 + 目录同名）

---

## 1. 分支与提交

分支从 `main @ 5bd12133` 切出。**`P2c-1`/`P2c-2` 已被集成会话并入 `main`**
（`main` 上可见提交 `4acf4376 integrate par/p2c-client-tail`，其父为 `b9a2303e`），
其余 5 个提交仍待集成。

| # | commit | 说明 | 测试属性数 |
|---|---|---|---|
| P2c-1 | `7f1b3ce1` | GlobalString / uWeatherEffectDef / ClientBuff / uAntiPlug / DepUtils / Mpeg / LanguagesDEPfix / uExceptionStruct（8 单元） | 96 |
| P2c-2 | `b9a2303e` | WinSock2.pas（常量 463 条 + packed 结构 + socket 接缝） | 54 |
| P2c-3 | `4db0f6b3` | StringHashMap / uDropItemEffectList / LogHelper / IECache | 76 |
| P2c-4 | `e5f82a14` | CheckProcessModules（含 60 条加密版权表）/ uFrmNGItemEdit | 40 |
| P2c-5 | `0c763cca` | NPCFormDeBug（脚本生成逻辑 + 接缝 + 文本 DFM 对齐） | 52 |
| P2c-6 | `d1409c2a` | UpdateEngine（协议/校验码/三级队列/消息解析 + 传输接缝） | 74 |
| P2c-7 | `339855cb` | HerbActor（**部分移植** + 覆盖登记） | 45 |

> `P2c-1`/`P2c-2` 的 SHA 已进入 `main`，因此当前 `git merge-base HEAD main = b9a2303e`；
> **`main` 尚未收到** `4db0f6b3`、`e5f82a14`、`0c763cca`、`d1409c2a`、`339855cb`。

### 1.1 一次分区违规及其修正（重要）

`P2c-1` 曾为把 3 份金标 TSV 挂成 `EmbeddedResource` 而修改了**共享文件**
`GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj`。集成会话体检时判定为越区。
已在 `P2c-2` 修正：

1. `git checkout main -- <该 csproj>`，使其与 `main` **逐字节一致**（已核
   `git diff main HEAD -- <该 csproj>` 为空）；
2. **删除全部 `.golden.tsv`**，改为由 `_scratch` 的生成器把金标写成**普通 `.cs` 源文件**
   （`TailGlobalStringGolden.g.cs` / `TailWeatherEffectGolden.g.cs` / `TailWinSock2Golden.g.cs`），
   随 SDK 风格工程自动编译 —— 不再需要改动任何共享文件。
3. 本车道全部提交的净改动 = **纯新增文件**（读取 `_scratch` 的产物一律不入库）。
4. `_scratch/` 通过**主仓库** `.git/info/exclude`（已含 `_scratch/`）排除，工作树内不留临时目录。

---

## 2. 逐单元完成表

`行号` 一律指 **GBK 原文**的行号；括号内给出 CRLF 计入后的行数。

| 优先 | 源单元（`Source/Client-HGE/`） | 行数 | 已覆盖 | 未覆盖 | 状态 |
|---|---|---|---|---|---|
| 1 | `WinSock2.pas` | 1405(1614) | 常量 :19-:1101 **463 条**（脚本抽取+回读）；`type` 段 packed 结构 :88-:424/:668-:759；纯逻辑实现 :1215-:1221、:1519-:1589 | `external 'ws2_32.dll'` 的 **121** 条绑定（:1391-:1520）与全部 WSA* 扩展声明 → 接缝 | ✅ 1:1（调用收敛） |
| 2 | `HerbActor.pas` | 1223(1341) | 常量 :19-:23、枚举 :26；`TKillingHerb` :157-:299、`TBeeQueen` :303-:425、`TCentipedeKingMon` :430-:514、`TMineMon` :1166-:1226、`TBigHeartMon` :1230-:1234、`TSpiderHouseMon` :1238-:1242、`TCastleDoor.ApplyDoorState` :533-:560 + Create :519-:525、`TWallStructure` 常量 | :583-:745、:763-:1165、:1177-:1216、:1244-:1341（绘制/贴图状态机，依赖 `TTexture` 与图库接缝）；**且基类 `TActor` 的虚方法是硬阻塞**（见 §6.1） | ⚠️ **部分**（已登记） |
| 3 | `UpdateEngine.pas` | 1558(1786) | 常量 :19-:36、枚举 :40、两个 packed 头 :56-:75、`CheckIP` :191-:216、`TSafeList` :78-:86/:220-:240、校验码哈希 :657-:675、三级优先级取件 :479-:568、请求表 :163-:178、`ClearRequests` :242-:311、消息解析 :604-:639 | 线程主循环 :362-…、`TClientSocket` 收发 :461/:593/:684、图库更新 :575/:1189-:1198、:765-:1165 的 WM_DATA 落盘 | ✅ 逻辑 1:1 / 传输接缝 |
| 4 | `GlobalString.pas`<br>`GlobalString-加密前备份.pas` | 449/447 | **414 条** resourcestring 全部（脚本抽取+回读逐字节）+ `DecodeResStr` :540/:544-547 | 无 | ✅ 1:1（含两份差异结论，见 §5.1） |
| 5 | `IECache.pas` | 723(1297) | 常量 :41-:77、`TFilterOption` :161-:177 + 位值表 :1271-:1273、`TSearchPattern` 模式表 :1121、记录 :111-:159、`UpdateFilterOptionValue` :1269-:1282、`ClearEntryValues` :1027-:1069、`GetEntryValues` 字段映射 :1073-:1112、`Create` :431-:449 | :299-:331 的 wininet 动态绑定与 20 个方法（:431-:1265）→ 接缝 | ✅ 逻辑 1:1 / WinINet 接缝 |
| 5 | `StringHashMap.pas` | 565(638) | 全部（哈希 hashlittle :149-:315、桶/链/负载因子 :317-:568、枚举器 :572-:635） | 无 | ✅ 1:1 |
| 5 | `LogHelper.pas` | 183(217) | 级别过滤 :65-:70、`WriteLog` 两种重载 :128-:161、时间戳/行格式 :134-:137、`GetUniqueMutexName` :204-:208、`TLogMode` :30 | `QueueUserAPC` + 命名互斥体 :73-:124 → 后台线程 + 阻塞队列（语义等价） | ✅ 1:1（线程接缝） |
| 6 | `NPCFormDeBug.pas` | 451(497) | `CalcEuclidDistance` :89-:97、`ColorTo256` :99-:134、`GetFontStr` :141-:168、`DxControlToString` :170-:475、`Start` :477-:488、`Open` :51-:62、`Button1Click` 预处理 :70-:71、**文本 DFM 101 行逐条** | `FrmDlg`/`g_EffectImageList` 的实际绘制与图库 | ✅ 逻辑 1:1 + 接缝 |
| 6 | `CheckProcessModules.pas` | 362(445) | 56 个 `_CSIDL_*` :26-:153；`ReadRegKey` :229-:255、`WriteRegKey` :258-:283、`GetSpecialFolderDir` :285-:312、`GetFileLegalCopyright` :314-:340、`CompareLStr` :372-:384、`CheckProcessModule` :386-:430、60 条加密版权表 :166-:227 + 解密 :362-:363 | tlHelp32 的模块枚举（原文其实**未调用**，见 §5.4） | ✅ 1:1 |
| 6 | `uFrmNGItemEdit.pas` | 41(49) | 全部 + **二进制 DFM 528 字节手工解码**（:18-:25 的控件树、`ShowFrmNGItemEdit` :33-:47） | 无 | ✅ 1:1 |
| 7 | `uDropItemEffectList.pas` | 160(182) | 全部（Add/IndexOf/Get/Items/Sort/QuickSort/CompareItem/CompareItem 差值） | 无 | ✅ 1:1 |
| 7 | `uWeatherEffectDef.pas` | 43(49) | 全部（22 项常量表 + `TWeateherEffect` packed 记录 + `string[50]`） | 无 | ✅ 1:1 |
| 7 | `uAntiPlug.pas` | 27(33) | `EnableDebugPrivilege` 按 §2.3 做 **Stub**（保留签名/返回值，注释保留 Win32 序列） | 实际提权实现 | ⛔ Stub（§2.3） |
| 7 | `uExceptionStruct.pas` | 134(153) | 全部常量 :138-:149 + 4 个 SEH/VEH 结构 :21-:48 / :109-:120 + 枚举 :124-:129（布局保真） | 任何注册/派发函数（原文也没有） | ⛔ Stub（§2.3） |
| 7 | `DepUtils.pas` | 84(101) | 6 个常量 :26-:31 + `DepEnforcement` :13-:20 + `ComputeDepFlags` 两条分支 :57-:63/:75-:85 | `SetProcessDEPPolicy`/`NtSetInformationProcess` 调用 | ⛔ Stub（§2.3） |
| 7 | `Mpeg.pas` | 98(113) | `TMPEG` 五公开成员 + `Close`/`Init` + 状态机取值（Create/Play/Pause/Stop/Close 的字段迁移） | DirectShow COM 互操作 | ⛔ Stub（§2.3） |
| 7 | `LanguagesDEPfix.pas` | 130(153) | `ParseLocaleIdHex` :98 的纯逻辑、跳转补丁常量 :140/:145、`LCID_SUPPORTED`、`TLangRec` | `ApplyLanguagesDEPfix` 的代码改写 :130-:148 | ⛔ Stub（§2.3） |
| 7 | `ClientBuff.pas` | 17 | 全部（两个 public 字段 `m_nButtonTop`/`m_nClientBuffTop`） | 无 | ✅ 1:1 |

**统计**：任务表 27 个单元中完成 **19 个**（其中 4 个是 §2.3 Stub）；**8 个未做**（见 §6）。

---

## 3. 新增文件清单

### 3.1 生产代码（`GXX.CSharp/src/GXX.Client/Tail/`，19 个）

| 文件 | 对应源单元 | 备注 |
|---|---|---|
| `GlobalString.cs` | `GlobalString.pas` | 414 条常量 + `BTMemoryMoudleErrTexts_TestMode` 数组 + `DecodeResStr` |
| `UWeatherEffectDef.cs` | `uWeatherEffectDef.pas` | `TWeateherEffect`/`ShortString50`/`WeatherEffectDef` |
| `ClientBuff.cs` | `ClientBuff.pas` | |
| `UAntiPlug.cs` | `uAntiPlug.pas` | §2.3 Stub |
| `DepUtils.cs` | `DepUtils.pas` | §2.3 Stub + `ComputeDepFlags` 纯逻辑 |
| `Mpeg.cs` | `Mpeg.pas` | §2.3 Stub |
| `LanguagesDEPfix.cs` | `LanguagesDEPfix.pas` | §2.3 Stub + `ParseLocaleIdHex` 纯逻辑 |
| `UExceptionStruct.cs` | `uExceptionStruct.pas` | §2.3 Stub（类型/常量全量） |
| `WinSock2Constants.cs` | `WinSock2.pas` const 段 | **脚本生成**（463 条，回读比对） |
| `WinSock2Structs.cs` | `WinSock2.pas` type 段 + `FD_*` | 手工（packed 布局断言） |
| `WinSock2Seam.cs` | `WinSock2.pas` implementation 段 | 纯逻辑 1:1 + `FunctionMap` 121 条映射 |
| `StringHashMap.cs` | `StringHashMap.pas` | |
| `UDropItemEffectList.cs` | `uDropItemEffectList.pas` | |
| `LogHelper.cs` | `LogHelper.pas` | |
| `IECache.cs` | `IECache.pas` | |
| `CheckProcessModules.cs` | `CheckProcessModules.pas` | |
| `UFrmNGItemEdit.cs` | `uFrmNGItemEdit.pas` | WinForms + 二进制 DFM 解码 |
| `NPCFormDeBug.cs` | `NPCFormDeBug.pas` | WinForms + `INpcControlView` 接缝 |
| `UpdateEngine.cs` | `UpdateEngine.pas` | 引擎逻辑 + `IUpdateTransport` 接缝 |
| `HerbActor.cs` | `HerbActor.pas` | **部分**：`HerbActorFramework` + `HerbActorCoverage` |

### 3.2 测试（`GXX.CSharp/tests/GXX.Client.Tests/`，12 个）

`TailGlobalStringTests.cs`（9）、`TailWeatherEffectTests.cs`（14）、`TailSmallUnitsTests.cs`（37）、
`TailWinSock2Tests.cs`（36）、`TailUtilUnitsTests.cs`（66）、`TailCheckModulesTests.cs`（34）、
`TailNpcFormDebugTests.cs`（47）、`TailUpdateEngineTests.cs`（51）、`TailHerbActorTests.cs`（40）
+ 3 份**生成**的金标表 `TailGlobalStringGolden.g.cs`、`TailWeatherEffectGolden.g.cs`、`TailWinSock2Golden.g.cs`。

> 金标表的生成器在 `_scratch/`（`gen_globalstring.py` / `gen_weathereffect.py` / `gen_winsock2.py`
> + `verify_winsock2.py`），**不入库**；产物入库以便测试可重复且不依赖外部文件。

---

## 4. 测试与门禁

| 项 | 结果 |
|---|---|
| 新增测试属性（`[Fact]`/`[Theory]`） | **334** |
| 实际新增用例数 | **437**（`filter FullyQualifiedName~Tail` 测得；`[Theory]` 多组数据展开后） |
| `dotnet build GXX.slnx -c Debug --nologo` | **0 error / 0 warning** |
| `dotnet test tests\GXX.Client.Tests`（全量） | **2758 passed / 0 failed** |
| 其中非本车道的用例（`filter FullyQualifiedName!~Tail`） | **2321 passed / 0 failed** ⇒ 本车道净增 **437** |
| 其它测试工程 | 未改动、未受影响（本车道只新增文件） |
| 分区体检 | 本车道 vs `merge-base` 的改动 = **纯新增**；`csproj` 与 `main` 一致 |

---

## 5. 发现的原文缺陷 / 易错点

### 5.1 `GlobalString.pas` vs `GlobalString-加密前备份.pas` 的差异结论（任务明确要求）

脚本对两份文件做了**全量键值比对**（`GlobalString.pas` 414 条有效常量 / 备份 412 条）：

| 类别 | 条数 | 明细 |
|---|---|---|
| 本次源**新增**（备份中不存在） | **2** | `SMyShopChangeItemFail12 = '[修改物品失败]：店铺物品禁止修改价格！'`（:342）、`SGuildRequestAllyRet7 = '不能和敌对行会或者联盟行会的敌对行会结盟。'`（:451） |
| 备份中**存在**、本次源已删除 | **0** | — |
| 同名但**取值不同** | **2** | ① `SCannotExitGame2`：本次源 `'战斗状态不能退出游戏！'`（:304） vs 备份 `'攻击状态不能退出游戏！'`（:307）<br>② `SGuildDelMemberFail4`：本次源 `'退出行会失败'`（:407） vs 备份 `'不能使用命令Z！'`（:409） |
| 仅排版差异（缩进/对齐空格） | **410** | 不影响取值 |

**另一处非"值"差异**（同样登记）：原文 `{$IF TESTMODE = 1}`（:31）分支下 19 条
`SBTMemoryMoudleErr01..19` 是真正的错误文本，而 `{$ELSE}`（:51，默认构建）**全部是单个空格**。
本移植默认值取**默认构建的实际行为**（空格），并把真文本另存为
`GlobalString.BTMemoryMoudleErrTexts_TestMode`（数组，19 条，测试锁住两者不同）。

### 5.2 `WinSock2.pas` 的解析陷阱（三条，都会静默丢数据）

1. **混合行尾**：该文件同时含 CRLF 与**裸 LF**。若按 `\n` 切分，行号会从第一次裸 LF 起持续漂移
   （实测偏移 2 行），进而让 `// 原文如此（<文件>:<行>）` 注释全部指错位置。
   生成器已改为先 `\r\n`→`\n`、`\r`→`\n` 再切分。
2. **`{$ENDIF}` 之后仍在同一 `const` 段**：`:306` 的 `{$ENDIF}` 之后 `:307`(PVD_CONFIG)…`:327`(SO_*)
   仍属该 const 段。把 `{$ENDIF}` 当块结束会**静默丢掉 59 条常量**（359 vs 463）。
3. **`type` 段里的无类型常量**：`:314` `AF_UNSPEC = 0;` 这类"旧式常量"位于 `type` 段内，
   但它们是常量面的一部分；若只扫 `const` 段会漏掉 `AF_UNSPEC/AF_INET/…` 一整族。

生成器现已做三重校验：① 每条表达式在 Python 里独立求值；② 写出的 `.cs` **回读**逐条比对；
③ 一个独立的"dumb scan"复核"所有形如 `NAME = <含数字表达式>;` 的行都在产物里"（`verify_winsock2.py`，missing=0）。

### 5.3 `StringHashMap.pas`：`SetValue` 漏写 `HashCode` ⇒ 首次扩容后哈希表退化成链表

原文 `SetValue`（:355-359）建节点时**没有**写 `HashCode`（只有 `TryAdd` :477 写了），
而 `Rehash`（:558）用 `Bucket.HashCode mod Length(NewItems)` 决定新桶。
后果：**第一次**扩容（旧桶数 4）时 `HashCode mod 4` 恒为 0 ⇒ 全部节点挤进 0 号桶。
已用测试 `Rehash_SetValueNodes_CollapseIntoBucketZero_OriginalBugFaithfullyPreserved` 锁死该退化
（并对照 `TryAdd` 路径不退化）。**本移植逐行保留，未"顺手修好"。**

### 5.4 `CheckProcessModules.pas`：`GetFileLegalCopyright` 恒返回空串

原文 :325 `InfoSize := GetFileVersionInfoSize(PChar(sFileName), InfoSize);` 把 `InfoSize`
同时当**出参**传入 —— 该 API 会用 `GetLastError` 覆写它（成功时为 0），函数的**返回值**（真实大小）
被丢弃 ⇒ `InfoSize` 恒为 0，`AllocMem(0)` 得零长缓冲、`GetFileVersionInfo` 必然失败。
**该函数在原文里实际上永远返回空串**。本移植用 `FileVersionInfo` 得到"原文注释所意图的结果"，
差异已登记；测试对真实系统 DLL 断言"非空且已 Trim"。

### 5.5 `NPCFormDeBug.pas`：`ColorTo256` 忘记更新 `nMinEd`

原文 :127-129 只写 `Result := I;`，**没有** `nMinEd := nCurED;`。
于是非精确命中时返回的是"自索引 1 起第一个比 d(0) 更近的索引"（若无则 0），
而**不是**真正最近的索引。测试用 4000 次随机采样证明"缺陷返回值 ≠ 真正最近索引"存在，并锁住该行为。

### 5.6 `UpdateEngine.pas`：接收缓冲必须是 `byte[]`

原文 `FRecvText: AnsiString` 是**字节容器**（用 `pTUpdateSrvMsgHeader(@FRecvText[1])^` 直接解读）。
移植中若把二进制放进 C# `string` 再取 GBK 字节，未定义字节序列会被解码器替换成 `'?'`(0x3F)，
协议头当场损坏（实测 9 个用例失败）。已改为 `byte[]` 承载并在文档注释里点明。

### 5.7 其它易错点

- `WinSock2.pas`：`INVALID_SOCKET`/`SOCKET_ERROR` 数值都是 **−1**，而 `INADDR_NONE`/`INADDR_BROADCAST`
  都是 `$FFFFFFFF` —— 正是"不能用 `!= -1` 当成功判据"的经典来源；已单独断言。
- `WinSock2.pas`：`FD_CLR` 是**左移保序**（:1560-1564 的 while 循环），不是某些 BSD 的
  "末元素覆盖"；已单独断言。
- `UpdateEngine.CheckIP`（:209-211）：`255.255.255.255` 四段都在 [0,255]，
  但 `inet_addr` 对它正好返回 `INADDR_NONE` ⇒ 原文的**第二重校验**把它判为非法。已单独断言。
- `StringHashMap.pas`（:532）：`Remove` 里 `if Prev <> nil then` 被**注释掉**且原文自带注释
  "Prev始终落在桶内，不可能为空" —— 已保真。
- `IECache.pas`（:1104）：`FSize := (info^.dwSizeHigh shl 32) + info^.dwSizeLow;`
  而 `dwSizeHigh` 是 32 位 `DWORD` ⇒ `shl 32` 结果恒 0，`FSize` 只等于 `dwSizeLow`。已保真并登记。
- `LogHelper.pas`（:176）：`if DirectoryExists(ExtractFileDir(sLogFile)) then ForceDirectories(...)`
  —— 条件写反（存在才建，而 `ForceDirectories` 对已存在目录是 no-op）。本移植按"能落盘"的实际
  意图实现（不存在才建），差异已登记。
- `CheckProcessModules.pas`（:272）：`WriteRegKey` 模式 3 写的是**未初始化的局部 `bData:Byte`**
  且长度恒 1 —— 形如"写入 1 个未定义字节"。已保真（值参数化以便测试）。
- `uWeatherEffectDef.pas`（:44）：被 `//` 注释掉的第 23 项 `dwEndOffset: 209 < dwStartOffset: 600`
  （区间反向）—— 已按原文"不采纳"，并由测试断言该区间不会出现在表里。
- `uDropItemEffectList.pas`：`Items[Index]` 越界在原文返回 nil（而 `FList.Items[Index]` 会抛）
  —— 本移植按原文语义返回 `null`，差异已登记。

---

## 6. 接缝 / 未完成

### 6.1 **硬阻塞**：`HerbActor.pas` 的基类虚方法

`HerbActor.pas` 的 14 个类全部 `override` 了 `TActor.CalcActorFrame` / `GetDefaultFrame`。
但既有 `GXX.Client.Scenes.ActorCore` 里这两个方法是**非虚**的
（`public void CalcActorFrame()` / `public int GetDefaultFrame(...)`），
且 `GetRaceByPM` 读的是 `ActorMonsterAction` 的 `TMonsterClientAction` 而非原版 `pTMonsterAction`。

要 1:1 落成"派生类里 override"**必须修改 `src/GXX.Client/Scenes/**`** ——
那是会话 A 的独占区，本车道无权修改。
因此本单元按"把每个子类的判定逻辑抽成可测纯函数"落地为 `HerbActorFramework`，
并由 `HerbActorCoverage` 逐条登记落地程度与未覆盖行区间；
**接入方（`Scenes` 车道）把这两个方法改虚后可直接转调这些函数。**

未覆盖行区间（程序化登记于 `HerbActorCoverage.UncoveredRanges`）：
`:583-:745`（`TCastleDoor` 绘制/贴图）、`:763-:1165`（`TWallStructure`+`TNewWallStructure`）、
`:1177-:1216`（`TCentipedeKingMon` 特效）、`:1244-:1341`（`TCentipedeKingMon.Run` + `TDragonBody`）。

### 6.2 本次**未做**的单元（8 个，含原因）

| 源单元 | 行数 | 未做原因 |
|---|---|---|
| `DxImageButtonEx.pas` | 769 | 依赖未移植的 `TDxImageButton`/`TDxImageIndex`/`TDxCaptionColor` 控件树；另一车道（DxComponent）负责 |
| `DxComponent/DxSwitchButton.pas` | 382 | 同上（需要 `TDxControl` 虚方法族） |
| `DxComponent/DxControlClpbrd.pas` | 158 | 同上 |
| `DxComponent/DxGroupAttackProgress.pas` | 462 | 同上 |
| `DxComponent/AsphyreTimer.pas` | 278 | 同上 |
| `DxComponent/LoginDlg.pas` | 162 | 同上（其 `.dfm` 已读并可随时对齐：`TRzDialogButtons`/`TRzButtonEdit`/`TRzRadioGroup` 是第三方 Raize 控件，托管侧无对应） |
| `DxComponent/StreamClipbrd.pas` | 204 | 同上 |
| `DxComponent/GuiManage.pas` | 417 | 同上 |

> 这 8 个单元全部依赖 `DxComponent/DxControls.pas`(3,516) + `DxComponents.pas` 的控件族，
> 而后者在台账 §10 第 7 条里明确是"P1 未认领的主力、待 DxComponent 全量后补"。
> 在控件族只做了接缝的当前基线下，硬做只能产出又一套接缝面，与既有
> `GXX.Client.DxComponent/DxComponentCommon.cs`、`GXX.Client.GUI.DxComponent/*` **重复造类型**，
> 正是台账 §9.3 记录过的 `CS0101` 事故。故本次如实不做。

### 6.3 已建立的接缝清单（供接入方对接）

| 接缝 | 位置 | 对接物 |
|---|---|---|
| `WinSock2Seam.FunctionMap` | `WinSock2Seam.cs` | 原文 121 条 `ws2_32.dll` 绑定 → `System.Net.Sockets` |
| `IECacheSeam.FunctionMap` | `IECache.cs` | 22 个 wininet 入口 → `HttpClient` |
| `IUpdateTransport` | `UpdateEngine.cs` | `TClientSocket` 收发 |
| `IUpdateImageLibrary` | `UpdateEngine.cs` | `TGameImages` 的 `m_ImgArr[*].boUpdate*` 标记 |
| `INpcControlView` / `INpcControlNode` | `NPCFormDeBug.cs` | `TDxControl` 家族 |
| `IHerbActorView` / `IHerbMonsterAction` | `HerbActor.cs` | `TActor` / `pTMonsterAction` |
| `TLogFile.DrainForTest` / `GlobalLogFileHolder.ResetForTest` | `LogHelper.cs` | 测试隔离 |
| `TStringHashMap.BucketAt` | `StringHashMap.cs` | 桶结构只读检视 |
| `NPCFormDeBug.ResetColorTableForTest` | `NPCFormDeBug.cs` | `g_DefColorTable` 注入 |

### 6.4 交给下一波的待办

1. **`HerbActor` 的剩余 5 个区间**需先由 `Scenes` 车道把 `TActorCore.CalcActorFrame` /
   `GetDefaultFrame` 改为 `virtual`（并把 `GetRaceByPM` 的返回类型统一），
   然后本文件的 `HerbActorFramework` 函数即可直接转调。
2. **DxComponent 尾巴 8 控件**在 `DxControls.pas`/`DxComponents.pas` 全量移植后按同一批次补做；
   `LoginDlg.dfm` 的三个 Raize 控件（`TRzDialogButtons`/`TRzButtonEdit`/`TRzRadioGroup`）
   需要逐条映射到 WinForms（`FlowLayoutPanel` + `Button` / `TextBox` + `Button` / `GroupBox`+`RadioButton`）。
3. `UpdateEngine` 的线程主循环 `:362-…` 与 WM_DATA 落盘 `:765-:1165` 待 `Pak`/`Wzl`/`SoundUtil`
   移植到位后接入 `IUpdateTransport`。
4. `uAntiPlug.EnableDebugPrivilege`、`DepUtils.SetCurrentProcessDEP`、`LanguagesDEPfix.ApplyLanguagesDEPfix`、
   `Mpeg.TMPEG` 的 Stub 若要落地为真实实现，需按 §2.3 重新评估（当前判定为"托管下无意义/不可移植"）。

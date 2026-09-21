# 并行报告 — 车道 `p8-m2-itemprop-misc`

> 目标：清扫三个剩余整单元（派发优先级 1→3）+ **第二轮：调度方批准的四条请求（#1–#4）落地**
>
> | 优先 | 源 | 实测行数 | 结论 |
> |---|---|---|---|
> | 1 | `Source/M2Engine/Forms/uFrmCustomItemProperty.pas` | 498 | **全树 1:1 完成** |
> | 2 | `Source/M2Engine/SellPlayer.pas` | 307 | **全类 1:1 完成** |
> | 3 | `Source/Common/EncodingHelper.pas` | 206 | **全单元 1:1 完成** |
> | 追加 | `Source/M2Engine/StringListHelper.pas`（请求 #4 授权） | 53 | **全单元 1:1 完成**（`EncodingHelper` 的首个生产调用方） |
>
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p8-m2-itemprop-misc`（分支 `par/p8-m2-itemprop-misc`）
> 基线：`64e4e1d1`（本车道开工时的 HEAD）
> 状态：**四单元全部完成 + 四条请求全部落地；门禁全绿**
> （build 0 error；`GXX.M2Server.Tests` **8560** 例、`GXX.Core.Tests` **953** 例，失败 0）
> 第二轮细节见 **§11**（含一条**新发现的原文数据相关缺陷**：GBK『值』被误判为无 BOM UTF-8）。

---

## 1. 全部 commit hash

| # | hash | 内容 |
|---|---|---|
| 1 | `c8ce509e` | 并行批次P8-1：`uFrmCustomItemProperty.pas` 窗体全树 1:1（接缝层 + 纯逻辑 + 窗体；91 例测试） |
| 2 | `066936ef` | 并行批次P8-2：`SellPlayer.pas` `TSellPlayerList` 全类 1:1（含 ShortStr 字节截断 / 二分插入位 / INI 往返 / AutoLoad；78 例测试） |
| 3 | `e6b06b1b` | 并行批次P8-3：`EncodingHelper.pas` 1:1（BOM/UTF-16 嗅探逐分支；53 例含差异断言） |
| 4 | `9873dd99` | 并行批次P8-4：本报告（第一轮） |
| 5 | `3f71fdf9` | **P8-5（请求 #1）**：`g_SellPlayerList` 由 `List<string>+SearchSellPlayer` 改为 `TSellPlayerList`（原文 `M2Share.pas:8416`），同步 `ViewOnlineHumanForm` 与两处测试 —— 由**调度方**在车道被宿主杀死后代提交；如实登记"车道被杀时工作树有 4 个未提交文件"这一流程事实 |
| 6 | `38d5b6af` | **P8-6（请求 #2）**：`FastIniFile` 固定日期时间整体回收进 `GXX.Core`（`ReadFixedDateTime/WriteFixedDateTime` + `TIniFixedDateTime`），`SellPlayerIni` 降级为纯转调 |
| 7 | `032a3e44` | **P8-7（请求 #3）**：`TStringList` 补 `Text`（`TStrings.GetTextStr/SetTextStr`）+ `DefaultEncoding/SetEncoding/GetEncoding/LineBreak` + `LoadFrom/SaveTo` 编码嗅探；`CustomItemPropertyLogic` 降级为纯转调、窗体改回 1:1 的 `.Text` |
| 8 | `ae414926` | **P8-8（请求 #4）**：`StringListHelper.pas`（53 行）1:1 移植 + `TFastIniFile.Load` 接入 `GetBufferEncoding`；锁定新发现的原文缺陷（GBK『值』`D6 B5` 被误判为无 BOM UTF-8） |
| 9 | 本报告第二轮提交 | `docs/并行报告-p8-m2-itemprop-misc.md` §11 追加（不含自身 hash 以免自引用） |

**切片粒度**：第一轮 3 个切片 + 第二轮 4 个切片，**每片独立提交、未攒批**；每片提交前都跑过 build + 两个测试工程全量。

`git diff --stat 64e4e1d1`（第二轮结束时）：**20 个文件；新增 15 个、修改 5 个（+6,542 / −26 行）**。
修改的 5 个既有文件**全部是调度方明确授权**的（`M2ShareFuncs.cs` / `ViewOnlineHumanForm.cs` / `FormViewOnlineHumanTests.cs`（请求 #1）、
`GXX.Core/Util/FastIniFile.cs` / `GXX.Core/Util/TStringList.cs`（请求 #2/#3））；其余 15 个全为新建（含本报告）。

---

## 2. 逐单元判定表

### 2.1 `uFrmCustomItemProperty.pas`（498 行）—— **已完成（1:1）**

| 行号区间 | 内容 | 状态 |
|---|---|---|
| `1-7` | 单元头 + `interface uses`（Windows/Messages/SysUtils/Variants/Classes/Graphics/Controls/Forms/Dialogs/StdCtrls/M2Share/ExtCtrls/ComCtrls） | 已覆盖（VCL 单元→接缝控件类型；`M2Share`→`CustomItemPropertyGlobals`） |
| `9-157` | `TFrmCustomItemProperty = class(TForm)`：120 个控件字段 + 8 个方法声明 | 已覆盖（字段名逐字照抄；`chk01..60`/`edtShowName01..60` 落为 `chk[1..60]`/`edtShowName[1..60]` + 只读别名属性） |
| `159` | `procedure ShowFrmCustomItemProperty;` 前置声明 | 已覆盖 |
| `163` | `{$R *.dfm}` | **未覆盖（有意）**：DFM 资源不移植；其**事件布线**由 `WireDfmEvents()` 1:1 复刻（见 §3） |
| `165-176` | `ShowFrmCustomItemProperty`：`if not boStartReady then Exit` → Create → ShowModal → Free | 已覆盖（`ShowFrmCustomItemProperty`） |
| `178-306` | `FormCreate`：60 个 chk 回填 + 60 个 edt 回填 + `btnOK.Enabled := False` + `mmoVar.Text := g_...TextVarList.Text` + `btnOK2.Enabled := False` | 已覆盖（**逐条展开照抄**） |
| `308-450` | `btnOKClick`：60 写回 + 60 写回 + Config 60×2 键 + `RebuildCustomItemPropertyConfig` + CRC 比较 + Send + `btnOK.Enabled := False` | 已覆盖（**逐条展开照抄**） |
| `452-461` | `chk01Click` / `edtShowName01Change` | 已覆盖 |
| `463-466` | `mmoVarChange` | 已覆盖 |
| `468-484` | `btnOK2Click` | 已覆盖 |
| `486-496` | `mmoVarKeyUp` / `mmoVarMouseDown` | 已覆盖（`CaretPos.Y + 1` 的行号文本抽为 `CustomItemPropertyLogic.LineNumCaption`） |

未覆盖汇总：仅 `:163`（`{$R *.dfm}`，DFM 不提；其事件布线已另行 1:1 复刻）。
**无"接缝待补"的方法**——8 个处理器 + 1 个单元级过程全部落地。

### 2.2 `SellPlayer.pas`（307 行）—— **已完成（1:1）**

| 行号区间 | 内容 | 状态 |
|---|---|---|
| `1-6` | 单元头 + uses（SysUtils/Classes/Grobal2/FastIniFile） | 已覆盖 |
| `9-21` | `PSellPlayerInfo` / `TSellPlayerInfo`（9 字段，4 个 `string[N]`） | 已覆盖（record→class；`string[N]` 按 **GBK 字节**截断） |
| `23-47` | `TSellPlayerList` 声明 | 已覆盖 |
| `49-52` | implementation uses（`M2Share, UsrEngn{$IF MULTI_THREAD=1}, M2Threads{$IFEND}`） | 已覆盖（→ `SellPlayerGlobals`） |
| `54-59` | `Create`：`FList.Capacity := 300`、`FIniFileName := g_Config.sEnvirDir + 'SellPlayer.ini'` | 已覆盖 |
| `61-66` | `Destroy`：`Clear; FList.Free; inherited` | 已覆盖（GC 语义等价；显式 `Clear` 的可观测行为保留） |
| `68-79` | `Clear` | 已覆盖 |
| `81-89` | `GetCount` / `GetItems` | 已覆盖（`Count` 属性 / 索引器 + `GetItems`） |
| `91-116` | `AddSellPlayer` | 已覆盖 |
| `118-145` | `Search`（二分 + 插入位） | 已覆盖（**`:129` 的 Delphi 优先级已加括号还原**） |
| `147-154` | `DeleteByIndex` | 已覆盖 |
| `156-174` | `DeletePlayer`（`SameText(Delegater)`） | 已覆盖 |
| `176-191` | `DeletePlayerEx` | 已覆盖 |
| `193-231` | `LoadConfig`（含 5 项校验 + `SetUserName=''`→`SetUser:=False`） | 已覆盖 |
| `233-264` | `SaveConfig`（含被注释的 `IniFile.Clear` + 裸 `except`） | 已覆盖 |
| `266-305` | `AutoLoadSellPlayer`（含 `MULTI_THREAD` 加锁/解锁） | 已覆盖 |

**该单元 1-307 行全覆盖，无未覆盖行。**

### 2.3 `EncodingHelper.pas`（206 行）—— **已完成（1:1）**

| 行号区间 | 内容 | 状态 |
|---|---|---|
| `1-25` | 单元头 + `TUTF8NoBomEncoding` / `TEncodingHelper` 声明 | 已覆盖（class helper→静态类，见 D-P8-5 形式偏差） |
| `29-34` | `TUTF8NoBomEncoding.GetPreamble` → 空 | 已覆盖 |
| `38-47` | UTF-8 字节形态注释表 | 已覆盖（逐条搬进 `IsBufferUTF8` 注释） |
| `48-131` | `IsBufferUTF8`（7 个形态分支 + ASCII 跳过 + 全 ASCII 返回 False） | 已覆盖（分支顺序、`P+n < EndPtr` 判据、`$80..$BF` 区间逐字照抄） |
| `133-136` | 两参 `GetBufferEncoding`（`Default` 属性 getter） | 已覆盖（`Default` = GBK/936，见 D-P8-6） |
| `139-188` | 三参 `GetBufferEncoding` + 嵌套 `ContainsPreamble` | 已覆盖（5 条嗅探分支 + 2 条已给定编码分支 + `Exit` 早退） |
| `190-204` | `GetNoBomUTF8`（`AtomicCmpExchange` 单例） | 已覆盖（`Interlocked.CompareExchange`） |

**该单元 1-206 行全覆盖，无未覆盖行。**

---

## 3. 新增文件 + 已覆盖/未覆盖行号范围

| 文件 | 行数 | 对应原文（覆盖行号） |
|---|---|---|
| `src/GXX.M2Server/Forms/ItemProperty/CustomItemPropertySeams.cs` | 269 | `uFrmCustomItemProperty.pas:5-7`（uses）、`:9-142`（120 控件字段的最小成员面）、DFM 事件字段（`OnClick/OnChange/OnKeyUp/OnMouseDown/OnCreate`）；接缝：`M2Share.pas`/`UsrEngn.pas` 未移植全局 |
| `src/GXX.M2Server/Forms/ItemProperty/CustomItemPropertyLogic.cs` | 83 | `uFrmCustomItemProperty.pas:304/472`（`TStrings.Text` 往返，Delphi `TStrings.GetTextStr/SetTextStr` 语义）、`:435`（`Low/High` = 1/60）、`:489/495`（行号文本） |
| `src/GXX.M2Server/Forms/ItemProperty/CustomItemPropertyForm.cs` | 698 | **`:10-157`、`:159`、`:165-176`、`:178-306`、`:308-450`、`:452-461`、`:463-466`、`:468-484`、`:486-496`**（唯一未覆盖 `:163` `{$R *.dfm}`） |
| `src/GXX.M2Server/Misc/SellPlayer.cs` | 388 | **`SellPlayer.pas:1-307` 全部** |
| `src/GXX.M2Server/Misc/SellPlayerSeams.cs` | 185 | 接缝：`UsrEngn.pas:14-21`（`TOffLineData`）、`UsrEngn.pas:174/176`、`M2Threads.pas`（`g_MultiThreadRun`）、`M2Share.pas:3589`（`MyGetTickCount`）；缺口方法：`FastIniFile.pas:189-194、988-1024、2953-2971、2985-2989` |
| `src/GXX.Core/Encoding/EncodingHelper.cs` | 259 | **`EncodingHelper.pas:1-206` 全部** |
| `tests/GXX.M2Server.Tests/ItemPropertyLogicTests.cs` | 365 | 纯逻辑 + 接缝（33 例） |
| `tests/GXX.M2Server.Tests/ItemPropertyTestBase.cs` | 87 | 测试基类（集合 `ItemPropertyLane`，`DisableParallelization`） |
| `tests/GXX.M2Server.Tests/ItemPropertyFormTests.cs` | 615 | 窗体全树（58 例） |
| `tests/GXX.M2Server.Tests/SellPlayerTests.cs` | 847 | `SellPlayer.pas` + `SellPlayerIni`（78 例） |
| `tests/GXX.Core.Tests/EncodingHelperTests.cs` | 433 | `EncodingHelper.pas`（53 例，含 7 组差异断言 Theory） |

---

## 4. 测试用例数 + build/test 结果

### 4.1 最终（第二轮结束时，HEAD = `ae414926` + 本报告）

| 门禁 | 结果 |
|---|---|
| `dotnet build GXX.slnx -c Debug` | **0 Error**（158 既有 warning，均为他方文件既有告警） |
| `dotnet test tests/GXX.M2Server.Tests` | **Passed! Failed: 0, Passed: 8560, Total: 8560** |
| `dotnet test tests/GXX.Core.Tests` | **Passed! Failed: 0, Passed: 953, Total: 953** |

- 基线实测（本工作树开工时先跑）：`GXX.M2Server.Tests` = **8,362**（派发单写 8,344，以实测为准）、`GXX.Core.Tests` = **837**。
- 用例增量逐笔：
  - 第一轮 **+169**：`ItemProperty*` 91（33 逻辑/接缝 + 58 窗体）+ `SellPlayer*` 78 → 8,531；
  - 请求 #1 **+3**（`g_SellPlayerList` 类型锁）→ 8,534（= 调度方代提交时的实测数）；
  - 请求 #2 **+7**（`SellPlayerIni` 纯转调核验）→ 8,541；Core `+24`（`TIniFixedDateTime` / `FastIniFile` 固定日期时间）→ 914；
  - 请求 #3 **+0**（M2Server 侧无新增；`CustomItemPropertyLogic` 语义由既有 10 例继续锁定）+ Core `+30`（`TStringList.Text` / 编码状态 / `LoadFromFile`/`SaveToFile` 嗅探）→ 944（**注**：请求 #3 的 M2Server 侧是"降级为转调 + 窗体改 `.Text`"，**复用既有断言**，未新增用例）；
  - 请求 #4 **+19**（`StringListHelper*`）+ Core `+9`（`TFastIniFile.Load` 嗅探 + 原文缺陷锁定）→ **8,560 / 953** ✅。
- **无新增失败、无跳过、无"偶发豁免"**；每个切片提交前都跑过全量。

### 4.2 第一轮基线快照（保留备查）

| 门禁 | 结果 |
|---|---|
| `dotnet test tests/GXX.M2Server.Tests` | Passed! Failed: 0, Passed: 8531, Total: 8531 |
| `dotnet test tests/GXX.Core.Tests` | Passed! Failed: 0, Passed: 890, Total: 890 |

每个公开方法的用例数（最少 3 例）抽样：
`ShowFrmCustomItemProperty` 5、`FormCreate` 6、`btnOKClick` 10、`btnOK2Click` 8、`chk01Click/edtShowName01Change/mmoVarChange` 各 2–3、
`mmoVarKeyUp/mmoVarMouseDown` 各 4（Theory 3 + 参数忽略 1）、`Search` 7、`AddSellPlayer` 4、`DeletePlayer(Ex)` 6、
`LoadConfig` 9、`SaveConfig` 5、`AutoLoadSellPlayer` 7、`GetBufferEncoding` 20、`IsBufferUTF8` 16、`NoBomUTF8` 7、
`ReadFixedDateTime` 7、`WriteFixedDateTime/FormatFixedDateTime` 4、`StrToDateDef/StrToTimeDef` 6、
`TStringList.Text` 14、`LoadFromFile/LoadFromStream` 18（helper 12 + RTL 版 6）。

---

## 5. 发现的原文缺陷 / 易错点（带 `文件:行`）

### 5.1 语义缺陷（已逐字保留 + 单测锁定）

| # | 位置 | 现象 | 后果 | 锁定用例 |
|---|---|---|---|---|
| 1 | `EncodingHelper.pas:68-69` | 注释写 "If all character is US-ASCII, done."，但 `Result` 自 `:53` 起恒为 False，该分支**直接 Exit 返回 False** | **纯 ASCII 缓冲不被判为 UTF-8**，一律回落到系统 ANSI（GBK）编码 | `IsBufferUTF8_PureAscii_IsFalse_OriginalQuirk` |
| 2 | `EncodingHelper.pas:147-157` | `ContainsPreamble` 在 `Length(Signature)=0` 时 `Length(Buffer)>=0` 恒真且 `for I:=1 to 0` 不执行 → **返回 True**（对任何缓冲，含空缓冲） | 目前"无害"（空 preamble 的长度就是 0，与不命中同值）；但若 `TEncoding.UTF8.GetPreamble` 变为空，**一切缓冲都会被判为 UTF-8** | `GetBufferEncoding_PreSetNoBomUtf8_OnEmptyPreamble_ReturnsZero`（不可区分，如实标注） |
| 3 | `EncodingHelper.pas:102-121` | 接受**已废弃的 5/6 字节形态** `$F8..$FB`/`$FC..$FD` | 非法 UTF-8 被误判为 UTF-8 | `IsBufferUTF8_DeprecatedFiveByteForm_IsAccepted_OriginalQuirk` / `...SixByteForm...` |
| 4 | `EncodingHelper.pas:78-83` | 无"最短编码"校验，`$C0/$C1` 过长编码照收 | 同上 | `IsBufferUTF8_OverlongTwoByteEncoding_IsAccepted_OriginalQuirk` |
| 5 | `EncodingHelper.pas:176-179` | 四条嗅探全不中 → `AEncoding := ADefaultEncoding; Exit;` —— **不读默认编码的 preamble** | 即使默认编码带 BOM，返回值也恒为 0（调用方不会跳过任何字节） | `GetBufferEncoding_ExplicitDefaultWithPreamble_StillReturnsZero_OriginalQuirk` |
| 6 | `SellPlayer.pas:243` | `// IniFile.Clear;` 被注释掉 | **陈旧节不删除**：列表收缩后 `SellPlayer.ini` 里残留无用节（原文如此，未"顺手修"） | `SaveConfig_DoesNotClearOldSections_OriginalFlawPreserved` |
| 7 | `SellPlayer.pas:262-263` | 外层**裸 `except`（空处理器）** 吞掉一切异常（含 `IniFile.Free` 抛出的） | 写盘失败**完全静默** | `SaveConfig_SwallowsWriteErrors` |
| 8 | `FastIniFile.pas:2967-2968` | `T := StrToTimeDef(Copy(S, I+1, 8), 0)` 的默认值是 **0**，而判据是 `T <> -1` | **时间解析失败仍被接受**（0 ≠ -1）→ 结果 = 当天 00:00:00，坏时间被静默归零 | `ReadFixedDateTime_BadTime_IsSilentlyAcceptedAsMidnight_OriginalFlaw` |
| 9 | `FastIniFile.pas:2963-2964` | `I := Pos(' ', S); if I > 0 then` | 值里**没有空格时连日期都不解析**，直接返回 Default | `ReadFixedDateTime_NoSpace_ReturnsDefaultWithoutParsingDate` |
| 10 | `SellPlayer.pas:58` | `g_Config.sEnvirDir + 'SellPlayer.ini'` 是**裸字符串相加**（非 `Path.Combine`） | `sEnvirDir` 少一个尾分隔符时，文件名会拼到**目录名后面**（`...\sub` + `SellPlayer.ini` = `...\subSellPlayer.ini`） | `Ctor_EnvirDirWithoutTrailingSeparator_ConcatenatesLiterally_OriginalTrap` |
| 11 | `SellPlayer.pas:86-89`、`:151`、`:184` | `FList.Items[Index]`／`FList.Items[I]` **无范围检查**（Delphi 默认 `$R-`） | Delphi 越界读的是垃圾指针（不抛）；托管必抛 `ArgumentOutOfRangeException` —— **无法复刻** | `Indexer_OutOfRange_ThrowsManagedException`（登记偏离 D-P8-2） |
| 12 | `uFrmCustomItemProperty.pas:435` | `for I := Low(...) to High(...)` 的下界是 **1**（不是 0） | Config 键名是 `CustomItemPropertyCheck1..60`；写 0 或 61 即为错 | `BtnOKClick_ConfigKeysUseOneBasedIndicesOnly` |
| 13 | `uFrmCustomItemProperty.pas:304` / `:472` | `mmoVar.Text := ...Text` / `...Text := mmoVar.Text` 用的是 `TStrings.Text`（**每行后追加 sLineBreak，含最后一行**） | `Count>0` 时 `Text` **必以 CRLF 结尾**；`''` → 0 行、`"\r\n"` → 1 个空行 | `SetTextStr_*` 5 例 + `GetTextStr_*` 5 例 + 往返 |
| 14 | `uFrmCustomItemProperty.pas:489/495` | `mmoVar.CaretPos.Y` 是 **0-based**（`+1` 才显示） | 行号显示差 1 的经典错 | `MmoVarKeyUp_WritesOneBasedLineNumber` |

### 5.2 移植陷阱（**已被单测抓出的真实错误**）

| # | 位置 | 陷阱 | 处置 |
|---|---|---|---|
| A | `uFrmCustomItemProperty.pas:169` | `if not boStartReady then Exit;` —— **`Exit` 是"返回"**。首版把它译成私有空方法 `Exit()`，于是**门控完全失效**（未启动也弹窗） | 单测 `ShowFrm_WhenNotStartReady_DoesNothing` 把它抓出 → 改为 `return;`。**这是本车道唯一一次"首版写错被门禁抓住"的地方，已如实记录** |
| B | `SellPlayer.pas:129` | `I := L + (H - L) shr 1` —— Delphi 里 `shr`（二级）**优先于** `+`（三级），等价 `L + ((H-L) shr 1)`；而 C# 的 `>>` **后于** `+`，直译会变成 `(L + (H-L)) >> 1` | 显式加括号 `L + ((H - L) >> 1)` + 注释 + 偶数长度用例 `Search_EvenCountMidpointFormula_MatchesDelphiPrecedence` |
| C | `SellPlayer.pas:102` | `FillChar(Info^, SizeOf(TSellPlayerInfo), 0)` | 托管侧靠字段初值（`""`/`0`/`false`）等价；`boPassWordSuccess` 在 `AutoLoadSellPlayer` 的 `New` 路径上**未清零**，故托管默认 False 与原文一致 → 用例 `AutoLoad_PacksEntriesIntoAutoLoadList` 断言 |
| D | `EncodingHelper.pas` 默认编码 | Delphi `TEncoding.Default` = **系统 ANSI（936）**；.NET Core 的 `Encoding.Default` = **UTF-8** | 接线到 `EncodingInit.GBK`，并用 `GetBufferEncoding_DefaultIsSystemAnsiNotDotNetUtf8` 做**差异断言** |
| E | `EncodingHelper.pas:9-12` vs `TEncoding.UTF8` | `TUTF8NoBomEncoding` 之所以存在，正说明 `TUTF8Encoding.GetPreamble` **非空**（3 字节）；禁用 `Encoding.UTF8`→`IsBufferUTF8` 的假设 | `Utf8Preamble_IsThreeBytes_DifferenceFromNoBomVariant` |
| F | `TEncoding.UTF8.GetPreamble()` 与 BOM 嗅探的**顺序** | `EF BB BF` 本身也是合法 3 字节 UTF-8，两条路径都"命中"；原文 BOM 嗅探在 `IsBufferUTF8` **之前** | `GetBufferEncoding_BomWinsOverIsBufferUTF8` |
| G | `FastIniFile.pas:2077-2085` | Delphi `TFastIniFile.Destroy` → `FlushBuffers`（自动落盘）；而 `GXX.Core.Util.TFastIniFile.Dispose()` 是**空实现** | `SaveConfig` 里显式 `UpdateFile()`（偏离 D-P8-3），并向 GXX.Core 提越区请求（§6） |
| H | `uFrmCustomItemProperty.pas:437-438` | `Config.WriteBool` 写 `'1'/'0'`（`TCustomIniFile` 约定），**不是** `BoolToStr` 的 `'-1'/'0'` | 用 `TFastIniFile.WriteBool`（既有实现已是 `'1'/'0'`）+ 用例断言 `ReadInteger(...)==1` |

### 5.3 跨模块不一致（**第一轮：发现但未擅改** —— 第二轮已全部获授权落地，状态见行尾）

1. **`M2ShareFuncs.cs:55-61` 的 `g_SellPlayerList` 与原文类型不符（重要）**
   原文 `M2Share.pas:8416`：`g_SellPlayerList: TSellPlayerList;` —— **它就是本切片刚移植的 `TSellPlayerList`**。
   而托管侧现存的是 `public static readonly List<string> g_SellPlayerList` + `SearchSellPlayer(name, out index)`（`IndexOf` 语义）。
   Delphi 侧调用点是**二分查找 + 记录访问**：`UsrEngn.pas:1446/1523/1525/2328/2330-2332/2338/2340/2355/2837/2840-2841/2894/2896/2938`、
   `ViewOnlineHuman.pas:481`、`svMain.pas:663-667/1414/1636-1637/3180`（`LoadConfig`/`AutoLoadSellPlayer`/`SaveConfig`/`DeleteByIndex`/`AddSellPlayer`）。
   **`List<string>` 无法表达 `Items[I]` 记录访问与 `DeleteByIndex`，属"接缝臆造"型偏差**
   → 📌 **第二轮已修正**（请求 #1，commit `3f71fdf9`，见 §11.1）。

2. **`GXX.Core.Util.TFastIniFile` 缺 `ReadFixedDateTime` / `WriteFixedDateTime`**（`FastIniFile.pas:2953-2971` / `:2985-2989`），
   而 `SellPlayer.pas:216/:255` 必须用 → 本车道先在 `SellPlayerSeams.SellPlayerIni` 里落地
   → 📌 **第二轮已回收进 `GXX.Core`**（请求 #2，commit `38d5b6af`，见 §11.2）。

3. **`GXX.Core.Util.TStringList` 缺 `Text`（`TStrings.GetTextStr/SetTextStr`）**，而 `uFrmCustomItemProperty.pas:304/:472` 必须用 →
   本车道先在 `CustomItemPropertyLogic.GetTextStr/SetTextStr` 里落地
   → 📌 **第二轮已补齐并降级为纯转调**（请求 #3，commit `032a3e44`，见 §11.3）。

4. **`string[N]` 截断语义两车道不一致**：本车道按原文 **GBK 字节**截断（`SellPlayerShortStr.Trunc`）；
   p5-m2-custommagic 车道的 `CustomMagicShortStr.Trunc` 是**按字符**截断（`value[..maxLen]`），中文字符串下与 Delphi 不同。
   未擅改他方文件，仅登记（`SellPlayer.cs` 文件头已注明）。**第二轮未处理（不在批准范围）。**

5. **`EncodingHelper` 曾**零** C# 调用点**：其真实消费者是 `StringListHelper.pas:45`（**53 行，当时尚未移植**）
   与 `FastIniFile.pas:1771`（托管侧 `TFastIniFile.Load()` 用固定 GBK 读，**没有**无 BOM UTF-8 嗅探）。
   即"无 BOM 的 UTF-8 文件"在 Delphi 下会被正确识别，在托管侧会被当 GBK 读
   → 📌 **第二轮已接线 + 已移植消费者**（请求 #4，commit `ae414926`，见 §11.4）；
   ⚠ 同时暴露了**反方向**的原文缺陷（GBK 被误判为 UTF-8）→ 见 **§11.5**。

---

## 6. 接缝清单 + 精确签名 + 越区请求

### 6.1 接缝清单（本车道**新增**，全部在独占区内）

`GXX.M2Server.Forms.ItemProperty.CustomItemPropertyGlobals`
（★ 按《并行派发台账》§25.2，**未接线即抛 `InvalidOperationException`，绝不返回中性值**）：

| 成员 | 精确签名 | 默认 | 接线目标（原文） |
|---|---|---|---|
| `g_CustomItemPropertyChecks` | `static bool[] g_CustomItemPropertyChecks { get; set; }` | **get 抛异常** | `M2Share.pas:4054`（`array[1..60] of Boolean`，typed const 默认全 True） |
| `g_CustomItemPropertyBindNames` | `static string[] g_CustomItemPropertyBindNames { get; set; }` | **get 抛异常** | `M2Share.pas:3992` |
| `g_CustomItemPropertyTextVarList` | `static TStringList g_CustomItemPropertyTextVarList { get; set; }` | **get 抛异常** | `M2Share.pas:3765`（`svMain.pas:2010` 创建） |
| `g_CustomItemPropertyCRC` | `static uint g_CustomItemPropertyCRC` | `0`（数据面） | `M2Share.pas`（`Rebuild…` 写入） |
| `g_CustomItemPropertyTextVarListTextCRC` | `static uint g_CustomItemPropertyTextVarListTextCRC` | `0`（数据面） | `M2Share.pas:3821` |
| `boStartReady` | `static bool boStartReady` | `false` | `M2Share.pas`；**False = 原文全局初值**（非臆造中性值），文件内已注明 |
| `RebuildCustomItemPropertyConfig` | `static Action? RebuildCustomItemPropertyConfig` | `null` | `M2Share.pas:22227-22243` |
| `SaveCustomItemPropertyTextVarList` | `static Action? SaveCustomItemPropertyTextVarList` | `null` | `M2Share.pas:9343-9359` |
| `SendCustomItemPropertyConfig` | `static Action? SendCustomItemPropertyConfig` | `null` | `UsrEngn.pas UserEngine.SendCustomItemPropertyConfig` |
| `SendCustomItemPropertyTextVarList` | `static Action? SendCustomItemPropertyTextVarList` | `null` | `UsrEngn.pas UserEngine.SendCustomItemPropertyTextVarList` |
| 调用门 | `static void InvokeRebuildCustomItemPropertyConfig()` / `InvokeSaveCustomItemPropertyTextVarList()` / `InvokeSendCustomItemPropertyConfig()` / `InvokeSendCustomItemPropertyTextVarList()` | 未接线抛异常 | 窗体 `:442 / :476 / :446 / :480` 的唯一入口 |

`GXX.M2Server.Misc.SellPlayerGlobals`：

| 成员 | 精确签名 | 默认 | 说明 |
|---|---|---|---|
| `m_AutoLoadSellPlayerList` | `static readonly List<TOffLineData> m_AutoLoadSellPlayerList` | 空列表 | `UsrEngn.pas:176`（构造于 `:475`）；**空列表 = 原文构造期状态** |
| `m_boStartAutoLoadSellPlayer` | `static bool m_boStartAutoLoadSellPlayer` | `false` | `UsrEngn.pas:174`（`:473`） |
| `g_MultiThreadRun` | `static bool g_MultiThreadRun` | `false` | `M2Threads.pas` |
| `MyGetTickCount` | `static Func<uint> MyGetTickCount` | `DelphiRTL.GetTickCount`（**真实现**） | `M2Share.pas:3589` `external mmsyst name 'timeGetTime'`（uint 回绕） |
| `LockWAutoLoadSellPlayerList(int)` / `UnLockWAutoLoadSellPlayerList()` | `static void …` | 记录到 `ListLockCalls` | `UsrEngn.pas:278/297`（`LockW(2)`） |

`GXX.M2Server.Misc.TOffLineData`（**接缝替身，正式归属 UsrEngn.pas**）：
`class TOffLineData { string sAccount=""; string sCharName=""; bool boStartLogin; uint dwStartLoginTick; bool boPassWordSuccess; object? SessInfo; }`

`GXX.M2Server.Forms.ItemProperty` 的无头 UI 接缝：
`CustomItemPropertyMessageBoxSeam`（`UiEnabled` 默认 true；`ShowModalCalls`；`static bool ShowModal()`）、
控件接缝 `CiControlSeam / CiWinControlSeam / CiFormSeam / CiLabelSeam / CiCheckBoxSeam / CiEditSeam / CiMemoSeam / CiPanelSeam / CiButtonSeam / CiTabSheetSeam / CiPageControlSeam`
（★ 一律加 `Ci` 前缀：`GXX.M2Server.Forms.CustomMagic` 命名空间下已有同名 `TControlSeam` 等类型，按派发要求"新增类型前 `git grep` 必须为空"改用前缀；`CiPageControlSeam.SetActivePageIndex` 复刻 §21.3 的 Delphi 越界静默 `-1`）。

**接线到真实现（非接缝）**：`Config` → `M2ShareState.ConfigIni`（`!Setup.txt` 的 TFastIniFile 镜像）；
`MainOutMessage` → `GXX.M2Server.Engine.M2ServerLog.MainOutMessage`；`MyGetTickCount` → `DelphiRTL.GetTickCount`。

### 6.2 越区请求（**第一轮原始形态，保留为历史记录**）

> ⚠ **本节四条请求已于第二轮全部获批并执行完毕**（授权：调度方 2026-09-20 追加分区）。
> 执行结果、落地 commit 与新增偏离见 **§11**。下面保留第一轮的**原始请求文本与精确 diff**，
> 作为"请求 → 批准 → 落地"链路的存档；**未执行者不要再照此改动**。

**#1（高优先，语义级）`src/GXX.M2Server/Engine/M2ShareFuncs.cs`：`g_SellPlayerList` 类型错了**

```diff
--- a/GXX.CSharp/src/GXX.M2Server/Engine/M2ShareFuncs.cs
+++ b/GXX.CSharp/src/GXX.M2Server/Engine/M2ShareFuncs.cs
@@ class M2ShareGlobals
-    /// <summary>g_SellPlayerList：寄售离线玩家名单（Search 语义：找到返回 true + 索引）。</summary>
-    public static readonly List<string> g_SellPlayerList = new();
-
-    /// <summary>TStringList.Search 等效。</summary>
-    public static bool SearchSellPlayer(string name, out int index)
-    {
-        index = g_SellPlayerList.IndexOf(name);
-        return index >= 0;
-    }
+    /// <summary>
+    /// g_SellPlayerList（M2Share.pas:8416 `g_SellPlayerList: TSellPlayerList;`）——
+    /// 出售/寄售角色列表（**不是** string 列表）。构造于 svMain.pas:1636
+    /// `g_SellPlayerList := TSellPlayerList.Create;`，随后 `LoadConfig`。
+    /// </summary>
+    public static GXX.M2Server.Misc.TSellPlayerList g_SellPlayerList = new();
```
配套改动（同一持有方）：
- `src/GXX.M2Server/Forms/ViewOnlineHumanForm.cs:402`
  `!M2ShareGlobals.SearchSellPlayer(player.m_sCharName, out int _)`
  → `!M2ShareGlobals.g_SellPlayerList.Search(player.m_sCharName, out int _)`（原文 `ViewOnlineHuman.pas:481`）。
- `tests/GXX.M2Server.Tests/FormViewOnlineHumanTests.cs:18/26/206`：`g_SellPlayerList.Clear()` 保留（新类型有 `Clear()`），
  但 `:206` 的 `g_SellPlayerList.Add("寄售乙")` 必须改为
  `g_SellPlayerList.AddSellPlayer("acc", "寄售乙", "deleg", 0, 1, 0, false, "")`。
- 后续 `svMain` / `UsrEngn` 批次应以 `M2ShareGlobals.g_SellPlayerList` 承接
  `svMain.pas:1636-1637/1414/663-667/3180` 与 `UsrEngn.pas` 的 12 处调用。

**#2 `src/GXX.Core/Util/FastIniFile.cs`：补 `ReadFixedDateTime` / `WriteFixedDateTime`**

```diff
--- a/GXX.CSharp/src/GXX.Core/Util/FastIniFile.cs
+++ b/GXX.CSharp/src/GXX.Core/Util/FastIniFile.cs
@@ public class TFastIniFile : IDisposable
+    /// <summary>FastIniFile.pas:191 <c>FIXED_DATE</c>。</summary>
+    public const string FIXED_DATE = "dd-mm-yyyy";
+    /// <summary>FastIniFile.pas:193 <c>FIXED_TIME</c>。</summary>
+    public const string FIXED_TIME = "hh:nn:ss";
+    /// <summary>FastIniFile.pas:194 <c>FIXED_DATETIME</c>。</summary>
+    public const string FIXED_DATETIME = FIXED_DATE + " " + FIXED_TIME;
+
+    /// <summary>FastIniFile.pas:2953-2971（逐分支 1:1；实现见 SellPlayerIni.ReadFixedDateTime）。</summary>
+    public double ReadFixedDateTime(string section, string ident, double defaultValue)
+        => GXX.M2Server... // ← 反例：GXX.Core 不能依赖 GXX.M2Server
```
> ⚠ 上面最后一行是**有意标出的依赖方向问题**：`GXX.Core` **不能**引用 `GXX.M2Server`。
> 正确做法是把 `SellPlayerIni.ReadFixedDateTime/WriteFixedDateTime/StrToDateDef/StrToTimeDef/FormatFixedDateTime`
> （本车道已 1:1 落地，含 `FIXED_*` 常量与三条原文边界）**整体搬进** `GXX.Core.Util.TFastIniFile`（或 `GXX.Core.Util.FixedDateTimeIni`），
> 然后删除 `GXX.M2Server/Misc/SellPlayerSeams.cs` 里的 `SellPlayerIni` 与 `SellPlayer.cs` 里的调用点改为直接调用。
> 源码可直接照抄（含 D-P8-4 的两条精确格式偏离说明）。**请勿由本车道执行**（`src/GXX.Core/Util/**` 是他方常驻区）。

**#3 `src/GXX.Core/Util/TStringList.cs`：补 `Text`（`TStrings.GetTextStr` / `SetTextStr`）**

```diff
--- a/GXX.CSharp/src/GXX.Core/Util/TStringList.cs
+++ b/GXX.CSharp/src/GXX.Core/Util/TStringList.cs
@@ public class TStringList
+    /// <summary>Delphi TStrings.Text 读（GetTextStr）：每行后追加 sLineBreak(#13#10)，含最后一行。</summary>
+    public string Text
+    {
+        get { var sb = new StringBuilder(); for (int i = 0; i < Count; i++) sb.Append(_strings[i]).Append("\r\n"); return sb.ToString(); }
+        set { /* SetTextStr：按 #13/#10/#13#10 切行，先 Clear 再逐行 Add（见 GXX.M2Server.Forms.ItemProperty.CustomItemPropertyLogic.SetTextStr） */ }
+    }
```
> 落地后请删除 `CustomItemPropertyLogic.GetTextStr/SetTextStr` 并把窗体 `:304/:472` 改为 `list.Text`。
> （`GetTextStr/SetTextStr` 的 5+5 条边界用例已在 `ItemPropertyLogicTests.cs`，可直接搬。）

**#4 `src/GXX.Core/Util/TFastIniFile.cs` / `TStringList.LoadFromFile`：接入 `TEncodingHelper.GetBufferEncoding`**

```diff
--- a/GXX.CSharp/src/GXX.Core/Util/TFastIniFile.cs
+++ b/GXX.CSharp/src/GXX.Core/Util/TFastIniFile.cs
@@ private void Load()
-        foreach (string raw in File.ReadAllLines(_fileName, EncodingInit.GBK))
+        // 原文 FastIniFile.pas:1762-1773 TIniItems.LoadFromStream：
+        //   Size := TEncoding.GetBufferEncoding(Buffer, Encoding);   // ← BOM/UTF-16 嗅探 + 无 BOM UTF-8 识别
+        //   SetTextStr(Encoding.GetString(Buffer, Size, Length(Buffer) - Size));
+        // 托管现状用固定 GBK 读 → **无 BOM 的 UTF-8 文件会被当 GBK 读**（乱码）。
+        foreach (string raw in EncodingHelperDecode(_fileName))
```
> 同一请求覆盖 `TStringList.LoadFromFile`（`Classes.TStrings.LoadFromFile` 的 BOM 检测语义）。
> 另：**`Source/M2Engine/StringListHelper.pas`（53 行）尚未移植**，它是 `TEncodingHelper.GetBufferEncoding(Buffer, Encoding, DefaultEncoding)`
> 的**主要消费者**（`:45`）；建议后续批次认领（本车道未越区实现）。

---

## 7. 诚实的完成度说明与剩余量

- **三单元全部完成**，无"做不完"的部分：`uFrmCustomItemProperty.pas`（498 行）、`SellPlayer.pas`（307 行）、
  `EncodingHelper.pas`（206 行）的**全部可移植行**均已 1:1 落地并单测。三单元合计 1,011 行原文 → 11 个新文件、4,517 行（含注释与测试）。
- **未覆盖（有意，已逐条说明）**：
  1. `uFrmCustomItemProperty.pas:163` `{$R *.dfm}`（DFM 资源不提；事件布线已另以 `WireDfmEvents()` 1:1 复刻并断言 60+60）；
  2. VCL 框架自身行为（`TForm.ShowModal` 的真窗口生命周期、`Application` 消息泵）→ §2.3 不移植项，走 `CustomItemPropertyMessageBoxSeam`；
  3. `EncodingHelper` 的 `{$IFDEF AUTOREFCOUNT}` 段（`EncodingHelper.pas:199-201`，移动编译器 ARC 专用，托管侧不适用）。
- **无法断言的项（如实标注，不假装覆盖）**：
  1. `ContainsPreamble` 的"空签名恒真"分支（`EncodingHelper.pas:147-157`）在**当前编码集合下不可观测**（返回值都是 0），
     已用注释 + 用例说明，**没有**伪造可区分断言；
  2. `SellPlayer.pas:86-89` 的越界读垃圾指针语义（D-P8-2）在托管侧**原理上不可复刻**，只锁定了"必抛"这一侧；
  3. `AutoLoadSellPlayer` 的多线程锁**真实并发时序**（原文 `TSafeList.LockW` 是临界区）未断言，只断言了调用序列与参数；
  4. 真实 `!Setup.txt` 落盘的**并发**行为（D-P8-1）未断言。
- **剩余量（本车道之外，见 §6 越区请求）**：4 条请求，其中 **#1 是语义级**（`g_SellPlayerList` 类型错，影响 `ViewOnlineHuman` 与后续 `svMain`/`UsrEngn` 批次），
  #2/#3 是把本车道已落地的"缺口方法"回收进 `GXX.Core`，#4 是把 `EncodingHelper` 真正接到文件读取路径上（否则本单元虽已移植但**零调用点**）。
- **不移植登记**：本车道**没有**任何"不移植"单元（三单元都是派发指定的整单元，全部落地）。

---

## 8. 偏离登记（带编号，承 §26/§30.3）

| 编号 | 位置 | 偏离内容 | 理由 / 恢复途径 |
|---|---|---|---|
| **D-P8-1** | `CustomItemPropertyForm.btnOKClick`（`:437-438` 之后） | 补一次 `Config.UpdateFile()` | Delphi `TIniFile.WriteXxx` **立即落盘**，`TFastIniFile` 是内存缓存；不补则"保存后进程崩溃 = 什么都没存"。恢复途径：若判定应与其他窗体（`ItemSetForm` 等，均未落盘）保持一致，删掉该行即可 |
| **D-P8-2** | `TSellPlayerList` 索引器 / `DeleteByIndex` | 越界**抛** `ArgumentOutOfRangeException`（原文 Delphi `$R-` 读垃圾指针） | 无法复刻未定义行为；仅保留"失败必可见"的一侧 |
| **D-P8-3** | `TSellPlayerList.SaveConfig` | 在 `finally` 里显式 `IniFile.UpdateFile()` | Delphi `TFastIniFile.Destroy → FlushBuffers`（`FastIniFile.pas:2077-2085`）自动落盘，托管 `Dispose()` 是空实现；见越区请求 #2 |
| **D-P8-4** | `SellPlayerIni.StrToDateDef/StrToTimeDef` | 用 `TryParseExact("dd-MM-yyyy")` / `("HH:mm:ss")` | Delphi `StrToDate/StrToTime` 是按 `ShortDateFormat`/`TimeSeparator`（此处已被临时改为固定值）解析的宽松解析器，另有 AM/PM、单数字月日等形态；本复刻覆盖固定格式 + 全部原文边界，未覆盖其全部宽松形态 |
| **D-P8-5** | `TEncodingHelper` | Delphi `class helper for TEncoding` → 托管**静态类**（C# 12 无 class helper） | 成员名/重载/语义不变；调用点写作 `TEncodingHelper.GetBufferEncoding(...)` |
| **D-P8-6** | `TEncodingHelper.Default` | 用 `EncodingInit.GBK`（CP936）而非 .NET `Encoding.Default`（.NET Core = UTF-8） | 对齐 Delphi `TEncoding.Default` = 系统 ANSI；已写差异断言 |
| **D-P8-7** | `GXX.Core/Encoding/` 目录 | 命名空间取 `GXX.Core.EncodingHelper`（**不是** `GXX.Core.Encoding`） | 若取 `GXX.Core.Encoding`，`GXX.Core` 下既有文件（`EncodingInit.cs:22/28/31/36`、`Launcher/*.cs`、`Paradox/ParadoxDataSet.cs`）里所有 `Encoding.Xxx` 简单名会解析到**命名空间**而非 `System.Text.Encoding` → 一片 CS0118；那些文件在他方常驻区不可改 |
| **D-P8-8** | `TFrmCustomItemProperty` 控件字段 | `chk01..chk60`/`edtShowName01..60` 落为 `chk[1..60]`/`edtShowName[1..60]` + 60 个只读别名属性 | 保留 1:1 字段名与 1-based 下标；FormCreate/btnOKClick 的 4 组赋值**逐条展开**未合并为循环（已用脚本回读校验 6 组索引序列均为完整有序的 1..60） |
| **D-P8-9** | `TOffLineData`（`source/M2Engine/UsrEngn.pas:14-21`） | 在本车道 `Misc/SellPlayerSeams.cs` 里**临时定义** | 正式归属 = `UsrEngn.pas`；`UsrEngn` 批次落地后**必须删除本处定义**并改为引用正式类型（§14 去重规程） |

---

## 9. 防复发检查（派发要求 6）

新增每个**类型**前均执行：

```
git grep -l -E "(class|struct|enum|interface|delegate) +(partial +)?<TypeName>\b" main -- \
  'GXX.CSharp/src/**/*.cs' 'GXX.CSharp/tests/**/*.cs'
```

结果：`TFrmCustomItemProperty`、`CustomItemPropertyGlobals`、`CustomItemPropertyMessageBoxSeam`、`CustomItemPropertyLogic`、
`Ci{Control,WinControl,Form,Label,CheckBox,Edit,Memo,Panel,Button,TabSheet,PageControl}Seam`、
`TSellPlayerInfo`、`TSellPlayerList`、`SellPlayerGlobals`、`SellPlayerIni`、`SellPlayerShortStr`、
`TOffLineData`、`TUTF8NoBomEncoding`、`TEncodingHelper` —— **全部为空结果**（可新增）。

**第二轮追加检查**（同样全部为空结果）：`TIniFixedDateTime`、`TStringListHelper`、`StringListHelper`、
`EncodingHelperCoreWiringTests`、`EncodingHelperWiringTests`。
（`TStringListHelper` 是 Delphi 原地类型名，检查确认 main 上无同名托管类型 —— 可新增。）

其中 `TControlSeam`/`TCheckBoxSeam`/`TEditSeam`… 在 `GXX.M2Server.Forms.CustomMagic.CustomMagicSeams.cs` 中**已存在**，
故本车道的控件接缝**统一改用 `Ci` 前缀**（避免同文件 `using` 两个命名空间时的 CS0104）。

## 10. 门禁复现命令（本报告所有数字由此产生）

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p8-m2-itemprop-misc\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
dotnet test tests\GXX.Core.Tests\GXX.Core.Tests.csproj -c Debug --nologo
```

---

## 11. 第二轮交付 —— 调度方批准的四条请求（#1–#4）落地

> 授权来源：调度方消息（2026-09-20）。四条请求**全部批准**，并把
> `src/GXX.M2Server/Engine/M2ShareFuncs.cs`、`src/GXX.M2Server/Forms/ViewOnlineHumanForm.cs`、
> `tests/GXX.M2Server.Tests/FormViewOnlineHumanTests.cs`、`src/GXX.Core/Util/FastIniFile.cs`、
> `src/GXX.Core/Util/TStringList.cs`、`src/GXX.M2Server/Misc/StringListHelper*.cs` 及
> 测试 `StringListHelper*` 追加进本车道分区。**均已在第二轮执行完毕并逐片提交。**

### 11.1 请求 #1 —— `g_SellPlayerList` 类型错（接缝臆造修正）✅ 已落地

**依据**：原文 `M2Share.pas:8416` <c>g_SellPlayerList: TSellPlayerList;</c>（生命周期 `svMain.pas:1636` Create /
`:1637` LoadConfig / `:1414` AutoLoadSellPlayer / `:3180` Free）。原托管 `List<string> + SearchSellPlayer`（`IndexOf` 语义）
**从类型上无法表达** `Items[I]` 记录访问 / `DeleteByIndex` / `AddSellPlayer` / `SaveConfig`。

| 文件 | 改动 |
|---|---|
| `src/GXX.M2Server/Engine/M2ShareFuncs.cs` | `public static GXX.M2Server.Misc.TSellPlayerList g_SellPlayerList = new();`；**删除** `SearchSellPlayer`；注释里写明"接缝臆造修正"、依据行号与全部调用点 |
| `src/GXX.M2Server/Forms/ViewOnlineHumanForm.cs:402` | `SearchSellPlayer(name, out _)` → `g_SellPlayerList.Search(name, out _)`（原文 `ViewOnlineHuman.pas:481`） |
| `tests/GXX.M2Server.Tests/FormViewOnlineHumanTests.cs:206` | `g_SellPlayerList.Add("寄售乙")` → `AddSellPlayer("acc","寄售乙","deleg",0,1,0,false,"")`；`:18/:26` 的 `Clear()` 不变（新类型同样有） |
| `tests/GXX.M2Server.Tests/SellPlayerTests.cs` | **+3 例**类型锁：`IsType<TSellPlayerList>`、记录访问+`DeleteByIndex`、用反射锁死"`SearchSellPlayer` 不存在 / 字段类型必须是 `TSellPlayerList`"（防复发） |

> 说明：本车道在落地该请求时被宿主杀死，工作树中 4 个文件未提交；**由调度方按台账 §13.3 验证后代提交为 `3f71fdf9`**
> （`build 0 error` / `M2Server.Tests 8534`）。**调度方未改动本车道任何一行产出。**

### 11.2 请求 #2 —— `TFastIniFile` 补固定日期时间（实现整体回收进 `GXX.Core`）✅ 已落地

- `src/GXX.Core/Util/FastIniFile.cs`（**新增**）：
  - `public double ReadFixedDateTime(string section, string ident, double defaultValue)`（`FastIniFile.pas:2953-2971` 1:1）
  - `public void WriteFixedDateTime(string section, string ident, double value)`（`:2985-2989` 1:1）
  - 同文件**新增静态类** `TIniFixedDateTime`：`FIXED_DS/FIXED_DATE/FIXED_TS/FIXED_TIME/FIXED_DATETIME`
    （`:189-194`）+ `FormatFixedDateTime` + `StrToDateDef` + `StrToTimeDef`（`:988-1024`）
  - 注释里写明原文缺陷：**`FastIniFile.pas:2967-2968` 坏时间被静默归零**
    （`StrToTimeDef(..., 0)` 的 Default 是 0，而判据 `T <> -1` ⇒ 恒真）。
- `src/GXX.M2Server/Misc/SellPlayerSeams.cs`：`SellPlayerIni` **降级为纯转调**（保留 Delphi 单元级函数名与常量），
  符合方向约束"`GXX.Core` 不得反向依赖 `GXX.M2Server`"。
- 用例：Core **+24**（`EncodingHelperCoreWiringTests.cs`）；M2Server **+7**（`SellPlayerIniTests` 的转调等价核验，
  含 `Theory` 5 组覆盖三条原文分支）。

### 11.3 请求 #3 —— `TStringList` 补 `Text`（+ 编码状态）✅ 已落地

- `src/GXX.Core/Util/TStringList.cs`（**新增**）：
  - `public string Text { get; set; }` = `GetTextStr()` / `SetTextStr()`（`TStrings.GetTextStr`/`SetTextStr` 1:1）；
  - `public string GetTextStr()`：**每行后追加 `LineBreak`（含最后一行）** ⇒ `Count > 0` 时必以 CRLF 结尾；
  - `public void SetTextStr(string value)`：先 `Clear`，再按 `#13`/`#10`/`#13#10` 切行（**不使用 `LineBreak`**，与原文一致）；
  - `DefaultEncoding`（懒取 GBK/936 = Delphi `TEncoding.Default`）、`GetEncoding()`（缓存）、`SetEncoding(v)`；
  - `LineBreak`（默认 `"\r\n"`，置空抛 `ArgumentException`，对齐 `TStrings.SetLineBreak`）。
- `src/GXX.M2Server/Forms/ItemProperty/CustomItemPropertyLogic.cs`：`GetTextStr/SetTextStr` **降级为纯转调**。
- `src/GXX.M2Server/Forms/ItemProperty/CustomItemPropertyForm.cs`：两处调用点改回**逐字 1:1** 形态
  （`:304` `mmoVar.Text = g_...TextVarList.Text`；`:472` `g_...TextVarList.Text = mmoVar.Text`）。
- 用例：Core **+30**（`Text` 14 + 编码状态 3 + `LineBreak` 2 + `LoadFromFile`/`SaveToFile` 嗅探 11）。

### 11.4 请求 #4 —— 把 `TEncodingHelper` 接到文件读取 + 移植其真实消费者 ✅ 已落地

**① 移植 `StringListHelper.pas`（53 行）→ `src/GXX.M2Server/Misc/StringListHelper.cs`**

| 行号区间（原文） | 内容 | 状态 |
|---|---|---|
| `1-3` | 单元注释「修复 StringList 读取无 bom 表的 Utf8 文件时乱码」 | 已覆盖（搬进文件头） |
| `5-16` | 单元头 + `TStringListHelper = class helper for TStringList` 声明（两个 `overload; virtual`） | 已覆盖（→ 静态类 + 显式首参，形式偏差 D-P8-14） |
| `22-32` | `LoadFromFile(const FileName: string)`：`TFileStream.Create(..., fmOpenRead or fmShareDenyWrite)` → `LoadFromStream(Stream, nil)` | 已覆盖 |
| `34-51` | `LoadFromStream(Stream, Encoding)`：`GetBufferEncoding(Buffer, Encoding, DefaultEncoding)` → `SetEncoding` → `SetTextStr(Encoding.GetString(...))` | 已覆盖 |

**② 接线**：`TFastIniFile.Load`（`FastIniFile.cs`）与 `TStringList.LoadFromFile`/`SaveToFile`（`TStringList.cs`）
现均走 `TEncodingHelper.GetBufferEncoding` 嗅探链。
`TFastIniFile` 的对应关系已核实到原文行号：`FastIniFile.pas:2432-2445 TFastIniFile.LoadValues`
→ `:1743-1753 TIniItems.LoadFromFile(FileName, FEncoding)`（`Create(AFileName)` ⇒ `FEncoding = nil`）
→ `:1757-1760 LoadFromStream(Stream)` → `:1762-1773 LoadFromStream(Stream, nil)` → `EncodingHelper.pas:167-179`。

**③ 其它"该用嗅探却直接假设编码"的读取点清单（★ 只列清单，未改任何一处）**

| # | 位置 | 现状 |
|---|---|---|
| 1 | `src/GXX.Core/Launcher/LauncherSettings.cs:188` | `File.ReadAllLines(path, gbk)` |
| 2 | `src/GXX.DBServer/IniFiles.cs:50` | `File.ReadAllLines(_fileName, EncodingInit.GBK)`（`TIniFileEx` 家族，与 `GXX.Core` 的 TFastIniFile 是两份实现） |
| 3 | `src/GXX.LogDataServer/LogDataService.cs:105` | `File.ReadAllLines(file, EncodingInit.GBK)` |
| 4 | `src/GXX.M2Server/Engine/Boxs.cs:228` | `File.ReadAllLines(sFileName, Encoding.GetEncoding(936))` |
| 5-6 | `src/GXX.M2Server/Engine/ClientModuleList.cs:96/129` | 固定 936（模块名清单） |
| 7 | `src/GXX.M2Server/Engine/MissionPageState.cs:48` | `EncodingInit.GBK` |
| 8 | `src/GXX.M2Server/Engine/ViewFormsData.cs:50` | 固定 936 |
| 9-10 | `src/GXX.M2Server/Forms/TxtEditorForm.cs:86/171` | `EncodingInit.GBK`（**文本编辑器**——最典型的"用户可能给 UTF-8 文件"场景） |
| 11 | `src/GXX.M2Server/Plugins/PluginAssemblyLoader.cs:140` | `EncodingInit.GBK` |
| 12-13 | `src/GXX.M2Server/Plugins/PluginHostRuntime.cs:101/174` | `EncodingInit.GBK` |
| 14 | `src/GXX.SelGate/SelGateConfig.cs:297` | `EncodingInit.GBK` |

> 其中 **#9/#10 `TxtEditorForm`** 与新接线的 `TStringList.LoadFromFile` 语义最相关（同一个"打开文本文件"场景），
> 建议后续由该文件所属车道评估统一走 `TEncodingHelper`。**本车道未擅自修改**（分区纪律）。
> 其余 `GBK.GetString(...)`（`Pak.cs`/`Wis.cs`/`Uib.cs`/`GuiRecords.g.cs`/`DesUnit.cs` 等）是**二进制格式内的定长文本字段**，
> GBK 是格式契约而非"猜测"，**不属于**本清单。

### 11.5 ★★ 新发现的原文缺陷：**GBK 文本被误判为「无 BOM 的 UTF-8」**（请求 #3/#4 过程中被单测抓出）

**这是本车道第二轮最有价值的发现，且它由"忠实移植"直接暴露**——我最初写的用例用「值」做 GBK 样本，
结果 `TFastIniFile.Load` 读出来是乱码，追查后确认**原文也如此**（不是我移植引入的）。

**成因**：`EncodingHelper.pas:167-179` 的嗅探第 4 条是"`IsBufferUTF8` 为真 → `NoBomUTF8`"，
而 `IsBufferUTF8`（`:78-83`）按 **UTF-8 字节形态**判断，**没有 GBK 排他性**：

| 事实 | 证据 |
|---|---|
| 「值」的 GBK 字节是 `D6 B5` | `EncodingInit.GBK.GetBytes("值") == {0xD6,0xB5}`（用例断言） |
| `D6 B5` 恰好是**合法**的 UTF-8 两字节序列 | `D6 ∈ $C0..$DF` 且 `B5 ∈ $80..$BF` ⇒ `IsBufferUTF8` 返回 **True**（用例断言） |
| 于是整份 GBK 文件被当 UTF-8 解码 | 只要**第一个非 ASCII 字节起、到文件尾**全部构成合法 UTF-8 序列即触发 |

**触发面**：GBK 首字节落在 `$C0..$DF` 且**次字节落在 `$80..$BF`**（GBK 次字节范围 `$40..$FE` 的子集），
其后**全是 ASCII 或同样"看起来合法"的序列**。典型场景：`Key=值`、`名字=值`（单个/少量汉字 + 其余全 ASCII）。
对照组：「测试」= `B2 E2 CA D4`，`B2` 不是合法 UTF-8 首字节 ⇒ 不会触发（用例 `..._GbkFileWithUnsafeLeadByte_DecodesAsGbk`）。

**完整调用链（原文，逐层已核实行号）**：

```
FastIniFile.pas:2432-2445  TFastIniFile.LoadValues
  → :1743-1753             TIniItems.LoadFromFile(FileName, FEncoding)   // Create(AFileName) ⇒ FEncoding = nil
  → :1757-1760             LoadFromStream(Stream) → LoadFromStream(Stream, nil)
  → :1762-1773             Size := TEncoding.GetBufferEncoding(Buffer, Encoding)
  → EncodingHelper.pas:167-179   BOM 三条不中 → IsBufferUTF8 为真 ⇒ AEncoding := TEncoding.NoBomUTF8
```
`StringListHelper.pas:45` 是**同一条链**（它正是"修 UTF-8 乱码"的补丁），因此**反方向**的 GBK 误判对它同样成立。

**本车道的处置（忠实优先 + 显著登记 + 用例锁定）**：
1. **不擅自加"GBK 安全阈值"**——那会偏离 1:1 语义（本工程的核心纪律）；
2. 把该行为**用两支用例锁死**：`..._IsMisdetected_OriginalFlaw`（证明缺陷存在且复刻了原文行为）
   与 `..._GbkFileWithUnsafeleadByte_DecodesAsGbk`（对照组）；
3. 在此**显著登记**，并给出"若调度方决定加保险"的最小改法（**不由本车道执行**）：
   `TEncodingHelper.GetBufferEncoding` 增加可选参数 `bool preferAnsiWhenAmbiguous`，
   当 `IsBufferUTF8` 命中但**同时**能按默认 ANSI 解出更少替换符时回落 ANSI；
   **注意这属于语义增强，会改变 `EncodingHelper.pas` 的 1:1 契约，须走正式偏离登记。**

> **对集成方的提示（重要）**：本请求把 `TFastIniFile.Load` 接到了嗅探链（忠实原文），
> 因此**任何"GBK 内容恰好构成合法 UTF-8 序列"的 INI 都会被误读**。若 main 上已有此类真实配置
> （如 `SellPlayer.ini` 的 `Player=值` 这种单汉字值），请在并入前用真实配置做一次冒烟。

### 11.6 ★ 为什么命名空间**不能**叫 `GXX.Core.Encoding`（独立条目，防后人"顺手改名"）

**结论**：本车道 `src/GXX.Core/Encoding/EncodingHelper.cs` 的命名空间是 **`GXX.Core.EncodingHelper`**，
**不是** `GXX.Core.Encoding`。**后者会导致编译失败，且失败点在本车道无权修改的他方文件里。**

**机理（C# 名字解析）**：在命名空间 `N` 内使用简单名 `X` 时，编译器**逐层向外**在"命名空间成员"里查找
`X`——**命名空间与类型共用同一声明空间**。若存在命名空间 `GXX.Core.Encoding`，那么：

- 文件 `GXX.CSharp/src/GXX.Core/EncodingInit.cs`（`namespace GXX.Core;`，第 22/28/31/36 行使用
  `Encoding.RegisterProvider(...)`、`Encoding?`、`Encoding.GetEncoding(936)`）中的 `Encoding`
  会解析到**命名空间 `GXX.Core.Encoding`**，而不是 `System.Text.Encoding`
  ⇒ **CS0118：`'Encoding' is a namespace but is used like a type`**；
- 同样受影响的还有 `GXX.Core.Launcher/LauncherSettings.cs:186/327`（`Encoding.GetEncoding(936)`）
  与 `GXX.Core.Paradox/ParadoxDataSet.cs:288-296`（`ParadoxConvSeam.Encoding(...)` 是方法名，另有 `TEncodingKind`）。

**为何不能"顺手把那几个文件加上限定名"**：它们在 **`src/GXX.Core/**` 的他方常驻区**（除本车道获批的
`Encoding/**`、`Util/FastIniFile.cs`、`Util/TStringList.cs`），按分区纪律**一个文件只能有一个写者**。

**因此**：目录名 `Encoding/` 与命名空间名 `GXX.Core.EncodingHelper` **有意不一致**（C# 不要求一致），
登记为偏离 **D-P8-7**。**后人不要"顺手改名"。**

### 11.7 第二轮新增偏离登记（承 §8）

| 编号 | 位置 | 偏离内容 | 理由 / 恢复途径 |
|---|---|---|---|
| **D-P8-10** | `TStringList.SaveToFile` | 由"固定 GBK"改为 `GetEncoding()` + `GetPreamble()` | Delphi `TStrings.SaveToStream(Stream, GetEncoding)` 先写 BOM 再写 `GetBytes(GetTextStr)`。**对从未 LoadFrom* 过的列表 / GBK 载入的列表字节完全一致**（GBK preamble 为空），只有 UTF-8/UTF-16 载入过的列表才改变 |
| **D-P8-11** | `TIniFixedDateTime.FormatFixedDateTime` | 超出 OLE 日期范围时**返回空串**（Delphi `FormatDateTime` 会抛 `EConvertError`） | 避免"写 INI 时因一个坏值整文件写失败"；恢复途径：改成 `throw` |
| **D-P8-12** | `TStringList.LoadFromFile` / `TEncodingHelper` 相关读取 | 文件不存在时**不抛**（保持既有托管行为） | Delphi `TFileStream.Create` 会抛 `EFOpenError`；既有各车道调用点依赖"缺失即空表"，本次**只改编码路径**、不动缺文件语义 |
| **D-P8-13** | `TStringList.LoadFromFile` | 由"固定 GBK"改为嗅探（BOM / UTF-16 / 无 BOM UTF-8 / 回落 GBK） | 对齐 Delphi RTL `TStrings.LoadFromStream` 的 `GetBufferEncoding` 语义；同时**继承 §11.5 的误判风险** |
| **D-P8-14** | `TStringListHelper` | Delphi `class helper` → C# **静态类**（实例作首参）；原文的 `virtual` 无对应物 | C# 12 无 class helper；`virtual` 在 helper 里意为"后代 helper 可覆盖"，静态方法无虚分派 |
| **D-P8-15** | `TFastIniFile.Load` / `Save` 不对称 | `Load` 已按嗅探解码，`Save` **仍固定 GBK** | Delphi 的 `TIniItems` 会把读入时的 `FEncoding` 用于回写。本次范围只批了 `Load`；**后果**：从 UTF-8 文件载入的 INI 再落盘会变回 GBK。恢复途径：`Save` 改用读到的编码 + preamble（与 D-P8-10 同形） |
| **D-P8-16** | `TEncodingHelper.GetBufferEncoding` 的嗅探 | **未加**"GBK 排他性"保险，逐字复刻原文（含 §11.5 的误判） | 1:1 优先；增强方案与影响已写在 §11.5，须由调度方裁定 |

### 11.8 第二轮新增文件 / 测试

| 文件 | 行数 | 说明 |
|---|---|---|
| `src/GXX.M2Server/Misc/StringListHelper.cs` | 89 | `StringListHelper.pas:1-53` 全单元 1:1；**`EncodingHelper` 的首个生产调用方** |
| `tests/GXX.M2Server.Tests/StringListHelperTests.cs` | 292 | 19 例：4 种编码差异断言、`LoadFromStream` 语义、`Stream.Position`、`DefaultEncoding` 回落、原文误判锁定、往返稳定 |
| `tests/GXX.Core.Tests/EncodingHelperCoreWiringTests.cs` | 625 | **63 例**（实跑）：`TIniFixedDateTime`/`TFastIniFile.ReadFixedDateTime` 24 + `TStringList.Text` 与编码状态 30 + `TFastIniFile.Load` 嗅探与原文缺陷锁定 9 ※ 三项分属请求 #2/#3/#4，在各自切片中追加进同一文件 |

> 文件名说明：`tests/GXX.Core.Tests/EncodingHelperCoreWiringTests.cs` 沿用本车道在该工程的授权前缀
> `EncodingHelper*`（**不越出文件分区**），内容却覆盖请求 #2/#3/#4 三项 Core 侧落地 —— 名称与内容不完全对应，
> 在此如实登记。

### 11.9 纪律遵守与流程事实（如实记录）

1. **`.cs` 一律用编辑工具**（`edit`/`write`），**全程未使用脚本替换或 `Set-Content`**；写入均为**绝对路径**。
2. **小切片、立刻提交**：第二轮 4 个切片各自提交（`3f71fdf9`（调度方代提交）、`38d5b6af`、`032a3e44`、`ae414926`）。
3. **⚠ 流程事实（需登记）**：车道在完成请求 #1 的 4 个文件改动、**尚未提交**时被宿主杀死；
   调度方按台账 §13.3 验证后代提交为 `3f71fdf9`。**此后每个切片均即时提交**，未再出现未提交产出。
4. **最后一次提交必须全绿**：本轮最终门禁 = `build 0 error` / `M2Server.Tests 8560` / `Core.Tests 953`（见 §4.1），
   且该结果在报告提交**之前**的代码 HEAD（`ae414926`）上实跑；报告为纯 `.md`、不参与编译。
5. **不存在"偶发豁免"**：第二轮出现过 **3 次红**（`LoadFromFile_FourEncodings...` 的断言写错 2 次；
   `FastIniFile_Load_SniffsEncoding...` 的 GBK 样本触发 §11.5 缺陷 1 次），**全部定位根因后修复**，
   其中第 3 次直接催生了 §11.5 的原文缺陷登记与两支锁定用例。

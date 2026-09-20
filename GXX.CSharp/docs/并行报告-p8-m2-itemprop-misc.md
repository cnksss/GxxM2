# 并行报告 — 车道 `p8-m2-itemprop-misc`

> 目标：清扫三个剩余整单元（派发优先级 1→3）
>
> | 优先 | 源 | 实测行数 | 结论 |
> |---|---|---|---|
> | 1 | `Source/M2Engine/Forms/uFrmCustomItemProperty.pas` | 498 | **全树 1:1 完成** |
> | 2 | `Source/M2Engine/SellPlayer.pas` | 307 | **全类 1:1 完成** |
> | 3 | `Source/Common/EncodingHelper.pas` | 206 | **全单元 1:1 完成** |
>
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p8-m2-itemprop-misc`（分支 `par/p8-m2-itemprop-misc`）
> 基线：`64e4e1d1`（本车道开工时的 HEAD）
> 状态：**三单元全部完成；门禁全绿**（build 0 error；`GXX.M2Server.Tests` 8531 例、`GXX.Core.Tests` 890 例，失败 0）

---

## 1. 全部 commit hash

| # | hash | 内容 |
|---|---|---|
| 1 | `c8ce509e` | 并行批次P8-1：`uFrmCustomItemProperty.pas` 窗体全树 1:1（接缝层 + 纯逻辑 + 窗体；91 例测试） |
| 2 | `066936ef` | 并行批次P8-2：`SellPlayer.pas` `TSellPlayerList` 全类 1:1（含 ShortStr 字节截断 / 二分插入位 / INI 往返 / AutoLoad；78 例测试） |
| 3 | `e6b06b1b` | 并行批次P8-3：`EncodingHelper.pas` 1:1（BOM/UTF-16 嗅探逐分支；53 例含差异断言） |
| 4 | 本报告提交 | `docs/并行报告-p8-m2-itemprop-misc.md`（紧随 `e6b06b1b`；不含自身 hash 以免自引用） |

**切片粒度**：3 个切片各自独立提交，**未攒批**；每个切片提交前都跑过 `GXX.M2Server.Tests` 全量（P8-3 跑的是 `GXX.Core.Tests` 全量）。
最终 HEAD 的全部改动路径都落在独占区内（`Forms/ItemProperty/**`、`Misc/SellPlayer*.cs`、`Core/Encoding/**`、
`tests/.../{ItemProperty*,SellPlayer*}`、`tests/GXX.Core.Tests/EncodingHelper*`、本报告）——**未越区**。

`git diff --stat 64e4e1d1 HEAD`：**11 个文件、+4,517 行（全部为新增，无任何既有文件被修改）**。

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

| 门禁 | 结果 |
|---|---|
| `dotnet build GXX.slnx -c Debug` | **0 Error**（158 既有 warning，均为他方文件既有告警） |
| `dotnet test tests/GXX.M2Server.Tests` | **Passed! Failed: 0, Passed: 8531, Total: 8531** |
| `dotnet test tests/GXX.Core.Tests` | **Passed! Failed: 0, Passed: 890, Total: 890** |

- 基线实测（本工作树开工时先跑）：`GXX.M2Server.Tests` = **8,362**（派发单写 8,344，以实测为准）、`GXX.Core.Tests` = **837**。
- 新增：`+91`（`ItemProperty*`：33 + 58）+ `+78`（`SellPlayer*`）= **+169** → 8,362 + 169 = **8,531** ✅；
  `+53`（`EncodingHelper*`）→ 837 + 53 = **890** ✅。
- **无新增失败、无跳过、无"偶发豁免"**（全量各跑一次全绿）。

每个公开方法的用例数（最少 3 例）抽样：
`ShowFrmCustomItemProperty` 5、`FormCreate` 6、`btnOKClick` 10、`btnOK2Click` 8、`chk01Click/edtShowName01Change/mmoVarChange` 各 2–3、
`mmoVarKeyUp/mmoVarMouseDown` 各 4（Theory 3 + 参数忽略 1）、`Search` 7、`AddSellPlayer` 4、`DeletePlayer(Ex)` 6、
`LoadConfig` 9、`SaveConfig` 5、`AutoLoadSellPlayer` 7、`GetBufferEncoding` 20、`IsBufferUTF8` 16、`NoBomUTF8` 7、
`ReadFixedDateTime` 7、`WriteFixedDateTime/FormatFixedDateTime` 4、`StrToDateDef/StrToTimeDef` 6。

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

### 5.3 跨模块不一致（**发现但未擅改**）

1. **`M2ShareFuncs.cs:55-61` 的 `g_SellPlayerList` 与原文类型不符（重要）**
   原文 `M2Share.pas:8416`：`g_SellPlayerList: TSellPlayerList;` —— **它就是本切片刚移植的 `TSellPlayerList`**。
   而托管侧现存的是 `public static readonly List<string> g_SellPlayerList` + `SearchSellPlayer(name, out index)`（`IndexOf` 语义）。
   Delphi 侧调用点是**二分查找 + 记录访问**：`UsrEngn.pas:1446/1523/1525/2328/2330-2332/2338/2340/2355/2837/2840-2841/2894/2896/2938`、
   `ViewOnlineHuman.pas:481`、`svMain.pas:663-667/1414/1636-1637/3180`（`LoadConfig`/`AutoLoadSellPlayer`/`SaveConfig`/`DeleteByIndex`/`AddSellPlayer`）。
   **`List<string>` 无法表达 `Items[I]` 记录访问与 `DeleteByIndex`，属"接缝臆造"型偏差** → 见 §6 越区请求 #1。

2. **`GXX.Core.Util.TFastIniFile` 缺 `ReadFixedDateTime` / `WriteFixedDateTime`**（`FastIniFile.pas:2953-2971` / `:2985-2989`），
   而 `SellPlayer.pas:216/:255` 必须用 → 本车道在 `SellPlayerSeams.SellPlayerIni` 里落地并登记越区请求 #2。

3. **`GXX.Core.Util.TStringList` 缺 `Text`（`TStrings.GetTextStr/SetTextStr`）**，而 `uFrmCustomItemProperty.pas:304/:472` 必须用 →
   本车道在 `CustomItemPropertyLogic.GetTextStr/SetTextStr` 里落地并登记越区请求 #3。

4. **`string[N]` 截断语义两车道不一致**：本车道按原文 **GBK 字节**截断（`SellPlayerShortStr.Trunc`）；
   p5-m2-custommagic 车道的 `CustomMagicShortStr.Trunc` 是**按字符**截断（`value[..maxLen]`），中文字符串下与 Delphi 不同。
   未擅改他方文件，仅登记（`SellPlayer.cs` 文件头已注明）。

5. **`EncodingHelper` 目前**零** C# 调用点**：其真实消费者是 `StringListHelper.pas:45`（**53 行，尚未移植**）
   与 `FastIniFile.pas:1771`（托管侧 `TFastIniFile.Load()` 用固定 GBK 读，**没有**无 BOM UTF-8 嗅探）。
   即"无 BOM 的 UTF-8 文件"在 Delphi 下会被正确识别，在托管侧会被当 GBK 读 → 见 §6 越区请求 #4。

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

### 6.2 越区请求（**精确 diff，请勿由本车道执行**）

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

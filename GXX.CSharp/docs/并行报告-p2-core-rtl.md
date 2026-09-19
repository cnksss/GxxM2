# 并行报告 · 车道 `p2-core-rtl`（Delphi 布尔字符串化收口 + 三处本地绕过去重）

> 分支：`par/p2-core-rtl`　｜　工作树：`.worktrees/p2-core-rtl`　｜　日期：本轮 P2
> 门禁：（见 §5）全绿。本车道是唯一被授权**修改既有文件**的车道，实际只改了任务书白名单里的 5 个既有文件。

## 0. 结论先行：任务书的三处前提与原文不符（已按原文执行，未凭直觉）

| 任务书说法 | 回读原文的实况（文件:行） | 采取的动作 |
|---|---|---|
| `HUtil32.pas` 的 `BoolToStr` → `"-1"`/`"0"` | **不成立**。`HUtil32.pas:2884-2890` 返回**大写** `'TRUE'`/`'FALSE'`。`-1`/`0` 是 **SysUtils.BoolToStr**（Delphi 7 默认重载 `UseBoolStrs=False`）的语义 | 不把 -1/0 倒灌进 HUtil32；新增忠实成员 `BoolToStrDelphi`（`'TRUE'/'FALSE'`），并在 `DelphiRTL` 新增 SysUtils 版 `BoolToStr`（`'-1'/'0'`）作为 -1/0 的唯一收口点 |
| `HUtil32.pas` 里有 `StrToBoolDef` | **不存在**。全树只有 `StrToBool`（声明 174 / 实现 449-452）= `Boolean(Str_ToInt(Str, 0))` | 只核 `StrToBool`，补行号注释 + 不互逆差异断言 |
| `FastIniFile.pas` 的 `WriteBool` 是自研实现（可能是 1/0 也可能是 -1/0） | `FastIniFile.pas:370` `TFastIniFile = class(TCustomIniFile)`；`:428`「And the rest of the Readers/Writers are inherited from TCustomIniFile」→ 本单元**没有** WriteBool，走 RTL `TCustomIniFile.WriteBool` = **`'1'/'0'`**（同源镜像 `MemoryIniFiles.pas:678-683`） | **保留 1/0**，并在代码里注明「原文如此（FastIniFile.pas:370/428 + MemoryIniFiles.pas:678-683）」，另加与 SysUtils `-1/0` 的差异断言 |
| `FormatOne` 布尔实参按 `Convert.ToInt64` 渲染成 1/0 | 属实 | 已修为 `-1`/`0`（见 §2.1） |

> 关键判定依据（为什么 GameCenter/M2 的 `BoolToStr` 是 SysUtils 版）：它们所在单元的 uses 里**没有** HUtil32 ——
> `GameCenter/GMain.pas:5-11`、`M2Engine/Forms/GameConfig.pas:5-7`、`M2Engine/Forms/GeneralConfig.pas:5-7`。
> 所以 `GMain.pas:1991-1993`、`GameConfig.pas:2146-2147/3340-3346` 的 `WriteString(..., BoolToStr(...))` 落盘是 `'-1'/'0'`，
> 项目内既有的三处桩（`GShareTypes.cs:145`、`GeneralConfigForm.cs:475`、`SelGateConfig.cs:363`）与之一致，**它们本来就是对的**。

## 1. 提交（3 个切片，每片门禁全绿后提交）

| 序 | commit | 内容 |
|---|---|---|
| 1 | `f5fc6c28` | `并行批次P2-core-rtl-1：SysUtils 布尔字符串化收口（Format %d 布尔 -> -1/0、DelphiRTL.BoolToStr）` |
| 2 | `22c0f999` | `并行批次P2-core-rtl-2：HUtil32 布尔族按原文对齐（TRUE/FALSE、是/否）+ TFastIniFile ReadBool 按原文非零即真` |
| 3 | `0efc9681` | `并行批次P2-core-rtl-3：HUtil32Seam/FilterItems 的本地 GetValidStr3 复刻改转调（去重）` |

## 2. 既有文件改动（原文依据 + 修复前/后行为）

### 2.1 `src/GXX.Core/Rtl/DelphiRTL.cs`

| 改动 | 原文依据 | 修复前 → 修复后 |
|---|---|---|
| `DelphiFormat.FormatOne` 的 `'d'` 分支识别 `bool` 实参 | 本仓库**不含** Delphi RTL 源码（`Source/` 下无 `SysUtils.pas`），无行号可引；依据 = 该语义在项目内已落地的三处桩 + 任务书明确要求 `Format('%d',[True]) → '-1'` | `Format("%d", true)` → `"1"` ⇒ **`"-1"`**；`false` → `"0"`（不变）。宽度/精度沿用整数路径规则（`"%3d"`→`" -1"`、`"%.3d"`→`"-001"`） |
| 新增 `DelphiRTL.BoolToStr(bool value, bool useBoolStrs = false)` | SysUtils.BoolToStr 默认重载 → `'-1'/'0'`；`useBoolStrs:true` → `'True'/'False'`。互证桩：`GShareTypes.cs:137-149`、`GeneralConfigForm.cs:474-475`、`SelGateConfig.cs:363`；调用点：`GMain.pas:1991-1993`、`GameConfig.pas:2146-2147` | 新增（GXX.Core 内 `-1`/`0` 的唯一收口点，供各模块去重转调） |

### 2.2 `src/GXX.Core/Util/HUtil32.cs`（`布尔转换` 区）

| 成员 | 原文（HUtil32.pas） | 修复前 → 修复后 |
|---|---|---|
| `BoolToStr` | 2884-2890 → `'TRUE'`/`'FALSE'` | **保留** `"True"/"False"`（不静默改：`CoreTests.cs:448` 与 `GShareDeclTests.cs:256` 依赖它，且两个文件都不在白名单）＋登记注释 |
| `BoolToStrDelphi` | 2884-2890 | **新增** → `'TRUE'`/`'FALSE'`（忠实复刻） |
| `BooleanToStr` | 2900-2906 → `'是'/'否'` | `"True"/"False"` ⇒ **`'是'/'否'`**（旧实现与原文完全不符；全仓无调用点，grep 仅定义处） |
| `BoolToStr2` / `BoolToIntStr` / `BoolToCStr` / `BoolToInt` / `StrToBool` | 2892-2898 / 503-506 / 508-516 / 495-501 / 449-452 | 核对后**与原文一致，未改**，仅补行号与差异注释 |

原文四套语义（**禁止"统一"**）：`'TRUE'/'FALSE'`、`'1'/'0'`（`BoolToStr2` 与 `BoolToIntStr` 同形）、`'是'/'否'`（`BooleanToStr` 与 `BoolToCStr` 同形）、`StrToBool` = 整数非 0 即真。另有非本单元两套：SysUtils `'-1'/'0'`、`TCustomIniFile.WriteBool` `'1'/'0'`。

### 2.3 `src/GXX.Core/Util/FastIniFile.cs`

| 方法 | 原文依据 | 修复前 → 修复后 |
|---|---|---|
| `WriteBool` | `FastIniFile.pas:370/428`（继承 `TCustomIniFile`）＋ `MemoryIniFiles.pas:678-683`（`Values: array[Boolean] of string = ('0','1')`） | `'1'/'0'` **保持不变**（原文如此），补「原文如此（文件:行）」注释 + 与 SysUtils `-1/0` 的差异说明 |
| `ReadBool` | `TCustomIniFile.ReadBool` = `ReadInteger(Section, Ident, Ord(Default)) <> 0`（镜像 `MemoryIniFiles.pas:673-676`；GXX 内同一原文的镜像 `GXX.DBServer/IniFiles.cs:141-143`） | `s == "1" \|\| s.Equals("True", OrdinalIgnoreCase)` ⇒ **`ReadInteger(...) != 0`**。后果：① `'-1'`（LoginSrv `BasicSet.cs:741` 的落盘形态）由**假**变**真**；② `'2'`/`'$1'` 等非 0 整数为真；③ 文本 `'True'` 不再被无条件当真，而是回退 `Default`（原文 `StrToIntDef` 行为） |
| `ReadInteger` | `TCustomIniFile.ReadInteger`：先把 `'0x'/'0X'` 改写成 `'$'` 再 `StrToIntDef`（镜像 `MemoryIniFiles.pas:657-666`） | 无十六进制支持 ⇒ 支持 `$1F`/`0x1f`/`0X1F`；非数字/空值仍回退默认值（`ReadBool` 的依赖项） |
| `WriteInteger` | `MemoryIniFiles.pas:668-671`（`IntToStr(Value)`） | 逻辑不变，仅固定 `InvariantCulture`（原文与区域设置无关） |

### 2.4 `src/GXX.DBServer/HUtil32Seam.cs`（去重）

本地逐字复刻（82 行）⇒ 两个方法均**转调** `GXX.Core.Util.HUtil32.GetValidStr3 / GetValidStr3_Ex`（根因修复见 `HUtil32.cs:241-301`、台账 §9.4）。
调用点零改动：`AddrEdit.cs:183-192`、`DBShareSeam.cs:371-425`（均用 `[' ', '\t']`）。

### 2.5 `src/GXX.Client/GUI/GameConfig/FilterItems.cs`（去重）

`TFileItemDB.GetValidStr3`（本地复刻 63 行）⇒ **转调** `HUtil32.GetValidStr3`；`FilterItems.pas:186-193` 的 7 次连续调用形状不变（`FilterItems.cs:220-226`）。
注：该本地复刻的**文档注释**原先声称"返回值含该分隔符"，与其自身代码（`Copy(str, I+1, …)`，分隔符被吃掉）矛盾 —— 已随去重一并删除该错注，语义按原文（分隔符被吃掉）。

## 3. 新增文件

| 文件 | 例数 | 锁定内容 |
|---|---|---|
| `tests/GXX.Core.Tests/CoreRtlBooleanStrTests.cs` | 9 | `Format("%d", bool)` = `-1/0`（含宽度/精度、整数回归、与 `1/0`、`True/False` 的差异断言）+ `DelphiRTL.BoolToStr` 两种重载 |
| `tests/GXX.Core.Tests/BoolStrSemanticsTests.cs` | 11 | HUtil32 布尔族四套语义逐条 + `StrToBool` + "`StrToBool`/`BoolToStrDelphi` 不互逆"差异断言 + 全家族真值对照表 |
| `tests/GXX.Core.Tests/BoolStrFastIniFileTests.cs` | 7 | `WriteBool` 写 `1/0`（原文如此）+ 与 SysUtils `-1/0` 的差异断言 + `ReadBool` 非 0 即真（含 `-1`、`2`、`True` 回退默认值、缺键）+ 读写往返 + 十六进制 `ReadInteger` |
| `tests/GXX.Core.Tests/CoreRtlGetValidStr3ContractTests.cs` | 9 | 去重后**唯一实现真源**的契约：前导 `\t`/分隔符、链式切割必须前进（旧缺陷"原地打转"守卫）、全分隔符/无分隔符边界、单分隔符重载等价 |
| `docs/并行报告-p2-core-rtl.md` | — | 本报告 |

## 4. 五条门禁（实测）

```
dotnet build GXX.slnx -c Debug --nologo                                   → 0 error / 86 warning
dotnet test tests\GXX.Core.Tests\GXX.Core.Tests.csproj        → 198 passed / 0 failed   （基线 162，+36）
dotnet test tests\GXX.DBServer.Tests\GXX.DBServer.Tests.csproj → 199 passed / 0 failed   （基线 199，持平）
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj     → 2278 passed / 0 failed  （基线 2278，持平）
dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj → 5053 passed / 0 failed  （基线 5053，持平）
```
额外回归（非本车道门禁，但一并复跑确认无外溢）：
`LoginSrv 229 ✅　LogDataServer 142 ✅　GameCenter 219 ✅　SelGate 163 ✅　GatewayKit 7 ✅　RunGate 248 ✅`
（`GXX.Integration.Tests` 未跑：台账 §9.3/§10-12 已登记其固定端口 flaky 用例。）

## 5. 发现的其它不一致 —— **列出但未擅自统一**（多数不在本车道白名单）

1. **`HUtil32.Str_ToInt` 会吞掉 `Def`**：`HUtil32.cs:429-439` 的 `int.TryParse(str, out result)` 在解析失败时把 `result` 覆写成 `0`；原文 `HUtil32.pas:798-810` 是 `Result := Def` 起步、`StrToInt64` 抛异常被 `except` 吞掉 → **保留 Def**。故 `Str_ToInt("12abc", 5500)` 原文 = 5500，C# 给 0。
   **未修**：`tests/GXX.LoginSrv.Tests/BasicSetTests.cs:436-453` 用 `"12x"` 把该偏差钉成"差异断言"（断言 0），修它需要同时改车道6 的用例 —— 属集成分支裁决范围。`StrToBool` 因 `Def=0` 暂不受影响。
2. **`HUtil32.BoolToStr` 大小写偏差**（`"True"/"False"` vs 原文 `'TRUE'/'FALSE'`）：忠实成员已就位，翻转本体需同步改 `tests/GXX.Core.Tests/CoreTests.cs:448` 与 `tests/GXX.GameCenter.Tests/GShareDeclTests.cs:254-257`（两个文件均不在白名单）。
3. **三处 `SysUtils.BoolToStr` 本地副本**：`GShareTypes.cs:145`、`GeneralConfigForm.cs:475`、`SelGateConfig.cs:363` —— 现在可直接转调 `GXX.Core.Rtl.DelphiRTL.BoolToStr`（本车道未动，不在白名单）。
4. **同一 Delphi 原文的两份 C# INI 实现**：`GXX.Core.Util.TFastIniFile` 与 `GXX.DBServer/IniFiles.cs`（`TIniFile`）。后者 ReadBool/ReadInteger 早已按原文，前者本次才对齐；建议集成期收敛为一份（会牵动 DBServer.Tests 的键序/落盘断言，需谨慎）。
5. **`GameCenterIniFile.ReadBool` 只认 `'1'/'True'`**（`GameCenterIniFileTests.cs:102-111` 明确锁定，含"`-1` 读回 False"的差异断言），而 `GMain` 用 `WriteString(BoolToStr(...))` 落的正是 `'-1'` —— 即**原文缺陷：写入 -1、读回 False**（属车道9 的类，未动）。
6. **"看起来该统一、其实原文就是多套"**：HUtil32 内 4 套 + SysUtils 1 套 + `TCustomIniFile` 1 套 = 6 套布尔字符串化；其中 `BoolToStr2`≡`BoolToIntStr`（`'1'/'0'`）、`BooleanToStr`≡`BoolToCStr`（`'是'/'否'`）是**原文重复定义**，不是实现事故，**不要合并**；False 侧有四套都落成 `'0'`，差异只出现在 True 侧。
7. `HUtil32.StrToBool` 与 `HUtil32.BoolToStrDelphi` **不互逆**（`'TRUE'` → False、`'-1'` → True），原文如此。
8. 台账 §8.4 / §10-2 悬案**已结案**：`TFastIniFile.WriteBool` 写 `1/0` 与"原版 `TIniFile.WriteBool` 写 -1/0"的"矛盾"不成立 —— 原文 `TCustomIniFile.WriteBool` 就是 `'1'/'0'`，`-1/0` 属于 `SysUtils.BoolToStr`（`WriteString` 路径），两套语义并存且都正确。台账 §10-1（两处 `GetValidStr3` 复刻去重）**已完成**。

## 6. 未完成 / 存疑

1. **`Format` 的 `%u`/`%x`/`%p` 收到 Boolean 实参时未跟随 `%d`**。Delphi 的 RTL `FormatBuf` 很可能把 `'d','u'` 放在同一 `case` 分支（即 `%u` 也应为 `'-1'`），但本仓库没有 RTL 源码、联网也未取得可引用的 RTL 原文，按"不凭直觉"保留现状（`%u`→`1`、`%x`→`1`）。拿到 RTL 原文后可一次性补齐。
2. **`Format` 的 `0` 填充标志本就缺失**（`"%03d"` → `" -1"` 而非 Delphi 的 `"-01"`）：属既有偏差，与布尔无关，未动。
3. 未在 `GXX.DBServer.Tests` / `GXX.Client.Tests` 里新增去重用例（白名单只允许在 `tests/GXX.Core.Tests/` 新建 `CoreRtl*.cs`、`BoolStr*.cs`）；两处转调的正确性由这两个工程的**原有用例**守护，被转调方的契约由 `CoreRtlGetValidStr3ContractTests.cs` 钉死。
4. 未改任何 `*.csproj` / `GXX.slnx` / `Directory.Build.props` / `docs/并行*.md` / `tools/**`；新增测试靠 SDK glob 自动纳入（已由 build 验证）。
5. **环境教训（建议补进台账 §8）**：Windows PowerShell 5.1 下用 `Get-Content -Raw` + `Set-Content` 往返 UTF-8 中文源文件会**直接损坏文件**（本轮已实际踩到一次，靠 `write` 工具重写才恢复）。改文件一律用编辑器/文件工具，不要用 PowerShell 做文本往返。

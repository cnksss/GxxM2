# 并行报告 · p17-client-mshare（`MShare.pas`）

| 项 | 值 |
|---|---|
| 车道 | `p17-client-mshare` |
| 分支 | `par/p17-client-mshare` |
| 工作树 | `D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare` |
| 源单元 | `Client-HGE/MShare.pas`（UTF-8 镜像 `_analysis/utf8_mirror/Client-HGE/MShare.pas`，**13,522 行**） |
| 复核车道裁决 | `p12-e2only-review` 判 **C（未移植/仅借名）**，已登记 **REFUTED（真缺口）** |
| 本车道生命周期 | `REFUTED` → **`PARTIAL`（带实测覆盖率）** |
| 最新提交 | `50063f9e`（切片3） |

> **行号约定**：本报告所有「原文行」= `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号，**不是** `Source/`（GBK）的行号。

---

## 1. 结论摘要

- **MShare 的全局面（`g_*`）已从零开始落地**：累计交付 **231 个 `g_*` 声明真身**，其中 **24 个是上游车道 `p14-client-fstate` 在其 §11.2 B-6 里点名「最密集阻塞类」的实体**（含它已用 `FStateMShareSeam` 承载的 7 条）。
- **32 条单元级例程**以 1:1 忠实口径落地并配真实断言（含 3 条「原文如此」）。
- **第一个类落地**：`TWarrContinueHitManager` 4 条方法（其中 `CanOpenMagic` 因原文方法体被整段注释而**恒为 True**，属「原文如此」P17-ASIS-02）。
- **其余 21 个类（181 条方法）仍未移植**——其中 **129 条**是**图像/资源加载族**（`TImageList` 家族 17 个类、`TImageEvent`、`TMapDesc`），依赖 `HGE`/`GameImages`/`Wzl`/`Wil`/`Pak`；另 **52 条**是 HTTP/用户中心族，已按 CR-5 裁定列为**架构待决项**（§9.5）。
- **`MShare` 仍是 `PARTIAL`，不是 `MAPPED`**。剩余工作量与架构待决项见 §9。

### 1.1 实测覆盖率

| 口径 | 总数 | 真实体 | NotPorted | 原文如此 | 覆盖率 |
|---|---:|---:|---:|---:|---:|
| **类方法**（22 个类，全量口径） | 185 | **4** | 181 | 0 | **2.16 %** |
| **类方法**（可推进口径 = 185 − CR-5 的 52） | 133 | **4** | 129 | 0 | **3.01 %** |
| **单元级例程**（implementation 段顶层） | 168 | **39** | 126 | 3 | **23.21 %** |
| **例程合计** | 353 | **43** | 307 | 3 | **12.18 %** |
| **单元级全局 `g_*`**（interface 1271-3061） | 约 460 | **231** | 约 229 | 0 | **约 50.2 %** |
| **原文缺陷 / 原文如此（已锁死）** | — | — | — | **5**（P17-ASIS-01 / P17-ASIS-02 / P17-DEF-02 / P17-DEF-03 / P17-DEF-04） | — |
| **偏差点** | — | **4**（D-P17-01…04） | — | — | — |
| **已退役 seam（不计分子，单列）** | — | **1 个类 / 4 个成员** | — | — | §5.3 / §5.4 |

> **CR-5 裁定后的分母口径（本轮起生效）**：`THttpThread`(6) + `THttpClient`(7) + `TUserCenterManager`(39) = **52 条类方法**属
> **架构待决项**，本轮不做架构决定、**保持未移植且可见**。按调度方授权，这 52 条**从进度分母中排除**，在 §9.5 单列。
> 报告**同时给出全量与可推进两个口径**，避免"换分母美化进度"。

**「真实体 + NotPorted + 原文如此 = 成员数」对账**（§10 有逐文件明细）：
- 类方法：`4 + 181 + 0 = 185` ✅（其中 `CanOpenMagic` 计入真实体，其「原文如此」性质记在 P17-ASIS-02）
- 单元级例程：`39 + 126 + 3 = 168` ✅
- 全局：`231 + 约229 + 0 = 约460` ✅
- 已退役 seam：**1 个类 / 4 个成员**（`MShareWarrConfigSeam`，**不计入覆盖率分子**，见 §5.3 / §5.4）

**5 条「原文如此 / 原文缺陷」**（都在已交付的真实体内，不重复计入 NotPorted）：
`ActorXYToMapXY`/`MapXYToActorXY` 的 Y 轴 `* 32 div 32` 恒等式（P17-ASIS-01）、
`TWarrContinueHitManager.CanOpenMagic` 方法体被原文整段注释 ⇒ 恒 `True`（P17-ASIS-02）、
`IntToHexN` 的 `Digits` 当进制 + `>10` 早退（P17-DEF-02）、
`GetInputBoxInFilterList` 对 `nil` 过滤表无 `Assigned()` 判断（P17-DEF-03）、
`g_ClientConfig` 全文无声明（P17-DEF-04）。

### 1.2 可复跑命令

```powershell
# 覆盖率（按原文逐条清点，不依赖任何手工登记）
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare
# ① 22 个类 / 185 条类方法 / 168 条单元级例程
$f="_analysis\utf8_mirror\Client-HGE\MShare.pas"
(Select-String -Path $f -Pattern '=\s*[Cc]lass' | Where-Object { $_.LineNumber -lt 3062 }).Count   # => 22
(Select-String -Path $f -Pattern '^\s{0,2}(procedure|function|constructor|destructor)\s+[A-Za-z_]\w*\.' |
  Where-Object { $_.LineNumber -gt 3062 -and $_.LineNumber -lt 13356 }).Count                     # => 185
# ② 本车道交付的真身（含注入点/常量字段，脚本给的是"public static 声明总数"，不是覆盖率分子）
(Select-String -Path 'GXX.CSharp\src\GXX.Client\GUI\Mir\MShare\MShareFunctions.cs' -Pattern '^\s{4}public static').Count  # => 45
(Select-String -Path 'GXX.CSharp\src\GXX.Client\GUI\Mir\MShare\MShareGlobals.Core.cs' -Pattern '^\s{4}public static').Count # => 232（230 个 g_* + 2 私有 helper 行不计）
(Select-String -Path 'GXX.CSharp\src\GXX.Client\GUI\Mir\MShare\TWarrContinueHitManager.cs' -Pattern '^\s{4}public ').Count  # => 6
# ③ 门禁三行（官方脚本，带 -Repo，不要 --no-build）
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 `
  -Repo "D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare" `
  -Project GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj
```

---

## 2. 本车道交付物（切片 1 / 3 / A / B / C）

| 文件 | 提交 | 状态 | 内容 |
|---|---|---|---|
| `GXX.CSharp/src/GXX.Client/GUI/Mir/ClientGlobals.cs` | 1 / 3 / A | 改 | `MShareGlobals` 改 `partial`；补 24 条 fstate B-6 全局真身；补 `g_MyBlacklist`/`g_boContinuous`；`ResetForTests` 覆盖 seam/注入点 + **切片A 的 10 个新配置字段** |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareGlobals.Core.cs` | 1 / 3 | 新 | 232 条 `g_*` 全局（202 主流程 + `g_DefColorTable` + `g_InputBoxFilterList` + `g_MyBlacklist` + `g_boContinuous`） |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareFunctions.cs` | 1 / 3 / C | 新 | **39 条真身**（切片1 的 21 + 切片3 的 11 平台族 + 切片C 的 7 hint 字体族）+ 7 个 `ShiftState_*` 常量 + 4 条私有 helper + 4 个测试注入点 |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareTypes.cs` | 1 | 新 | `TRGBQuad`（`RGBTRIPLE`，`Pack=1`） |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/TWarrContinueHitManager.cs` | 3 / B | 新/改 | 客户端 `TWarrContinueHitManager`（4 条方法，1 条原文如此）；切片B 改指 `g_ConfigClient` 真身 |
| ~~`GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareWarrConfigSeam.cs`~~ | 3 → **A/B 删除** | **已退役** | 切片3 的临时跨区 seam；切片A 补字段后删除（见 §5.3 R-1） |
| **`GXX.CSharp/src/GXX.Client/GUI/Mir/MirForms.cs`** | A | 改 | **调度方 CR-6 授权**：`TConfigClient` 补 10 字段（连击三件套 + hint 字体族 7 个）+ `HintFontNameBuffer` 包装类型（见 §4.6） |
| `GXX.CSharp/tests/GXX.Client.Tests/MShareP17Slice1Tests.cs` | 1 | 新 | 33 个 `[Fact]/[Theory]` ⇒ **110 条用例** |
| `GXX.CSharp/tests/GXX.Client.Tests/MShareP17Slice3Tests.cs` | 3 | 新 | 44 个 `[Fact]/[Theory]` ⇒ **61 条用例** |
| `GXX.CSharp/docs/并行报告-p17-client-mshare.md` | 2 / 3 | 新 | 本报告 |

**门禁证据（切片3，官方脚本三行）**：
```
dotnet test exit code : 0
crash markers found   : none
GATE: PASS (build 0 error, test exit 0, no crash markers)
```
客户端测试项目全量：`已通过! - 失败: 0，通过: 5199，已跳过: 0，总计: 5199`
（车道基线 4,999 ⇒ 本车道累计新增 **201 条**用例：切片1 110 + 切片3 61 + 切片C 30；其余为并道带入）

---

## 3. 优先级 ① 已完成：fstate 7 条 seam 的**真身**

上游 `p14-client-fstate` §11.2 **B-6** 报告：MShare 全局缺失是它剩余 `PENDING` 里最密集的一类。它已用 `GUI/Share/FStateSeams.cs::FStateMShareSeam` 承载 7 条。**这 7 条的真身已在 `MShareGlobals` 落地，语义与接缝逐位一致**：

| # | 全局 | 原文行 | 原文声明 | 真身（本切片） | 消费点（原文） |
|---:|---|---:|---|---|---|
| 1 | `g_dwQueryMsgTick` | 1825 | `g_dwQueryMsgTick:longword` | `MShareGlobals.g_dwQueryMsgTick`（`uint`） | 17876/17885/18906/18914，判据 `MyGetTickCount > g_dwQueryMsgTick`，命中后 `+3000` |
| 2 | `g_dwDealActionTick` | 1824 | `g_dwDealActionTick:longword` | `MShareGlobals.g_dwDealActionTick`（`uint`） | 17535（严格 `>`）、17749（`+4000`） |
| 3 | `g_dwChallengeActionTick` | 2076 | `g_dwChallengeActionTick:Longword = 0` | `MShareGlobals.g_dwChallengeActionTick`（`uint`） | 20815（严格 `>`）、20792（`+4000`） |
| 4 | `g_boDealEnd` | 2040 | `g_boDealEnd:Boolean` | `MShareGlobals.g_boDealEnd`（`bool`） | 17748 —— **先判它** |
| 5 | `g_nDealGold` | 2038 | `g_nDealGold:Integer` | `MShareGlobals.g_nDealGold`（`int`） | 17748 |
| 6 | `g_boChallengeEnd` | 2074 | `g_boChallengeEnd:Boolean = False` | `MShareGlobals.g_boChallengeEnd`（`bool`） | 20791 |
| 7 | `g_nChallengeGold` | 2068 | `g_nChallengeGold:Integer = 0` | `MShareGlobals.g_nChallengeGold`（`int`） | 20791 |

**另补 B-6 剩余项（原文同段，一并落地）**：
`g_dwChangeGroupModeTick`(1823)、`g_boAllowGroup`(1827)、`g_dwLatestStruckTick`(1740)、`g_dwLatestMagicTick`(1749)、`g_dwLatestHitTick`(1748)、`g_boMagicMoving`(2089)、`g_MovingMagic`(2090)、`g_GameGoldDeal`(2054)、`g_nDealGameDiamond`(2055)、`g_boGameGoldDealing`(2056)、`g_dwGameGoldDealTick`(2057)、`g_nRankingsTablePage`(2280)、`g_nRankingsTableType`(2281)、`g_nRankingsPage`(2282，**初值 -1**)、`g_nRankingsPageCount`(2283)。

**另外 3 条接缝对应的真身**（p14 也登记在 `FStateMShareSeam`）：

| 全局 | 原文行 | 真身 | 备注 |
|---|---:|---|---|
| `g_SellDlgItem` | 2008 | `MShareGlobals.g_SellDlgItem`（`TClientItem`） | `TFrmDlg.Core.cs:94` 走 `FStateMShareSeam.g_SellDlgItem.s.Name` |
| `g_ExtBagOpenItemCount` | 1879 | `MShareGlobals.g_ExtBagOpenItemCount`（`ushort`） | `GetMaxBagCount()` 已直接吃真身 |
| `g_SelDeleteHumanInfo.sChrName` | 20594 只读 | **未落地**（见 §8 跨区请求 CR-1） | 依赖 `TUserCharacterInfo`，不在本车道切口 |

---

## 4. 对账表

### 4.1 22 个类（逐类）

状态码：`REAL`=真实体 / `NP`=NotPorted / `ASIS`=原文如此

| # | Class | 声明行 | 实现条数 | 实现行域 | 状态 | 阻塞原因（本车道判定） |
|---:|---|---:|---:|---|---|---|
| 1 | `THttpThread` | 761 | 6 | 4465-4671 | NP | 依赖 `WinINet`（`InternetOpen/HttpSendRequest` 等）；`TMshare` 侧无可复用托管层 |
| 2 | `TImageList` | 779 | 13 | 4710-4801 | NP | 依赖 `TGameImages`(`GameImages.pas`)、`TTexture`，该单元未移植 |
| 3 | `TTilesList` | 802 | 6 | 4812-4923 | NP | 同上（`Tiles.Wil/.Wzl` 解码） |
| 4 | `TSmTilesList` | 814 | 6 | 4945-5055 | NP | 同上 |
| 5 | `TMonImageList` | 826 | 4 | 5077-5198 | NP | 同上 + `Monster.pas` 动作表 |
| 6 | `THumImageList` | 837 | 6 | 5212-5460 | NP | 同上 + 人物/Wil 图像族 |
| 7 | `THumEffectList` | 849 | 7 | 10455-10763 | NP | 同上 |
| 8 | `TWeaponImageList` | 865 | 10 | 5488-5801 | NP | 同上 |
| 9 | `TWeaponEffectList` | 882 | 7 | 10794-10931 | NP | 同上 |
| 10 | `TCboWeaponEffectList` | 899 | 3 | 10949-10986 | NP | 同上 |
| 11 | `TCboWeaponList` | 909 | 3 | 11004-11041 | NP | 同上 |
| 12 | `TCboHumList` | 919 | 3 | 11059-11096 | NP | 同上 |
| 13 | `TCboHumEffect` | 929 | 3 | 11114-11151 | NP | 同上 |
| 14 | `TStateItemImages` | 938 | 11 | 5823-5974 | NP | 同上（状态栏物品图像缓存） |
| 15 | `TDnItemImages` | 960 | 13 | 6005-6209 | NP | 同上（内观物品图像缓存） |
| 16 | `TBagItemImages` | 986 | 11 | 6240-6388 | NP | 同上（包裹物品图像缓存） |
| 17 | `TNpcImageList` | 1008 | 3 | 6419-6462 | NP | 同上 |
| 18 | `TImageEvent` | 1029 | 16 | 7240-8171 | NP | 同上 + `TImageEvent` 全局事件表；`LoadGameImages`(7347-7914) 单个方法 567 行 |
| 19 | `TMapDesc` | 1057 | 4 | 10323-10435 | NP | 依赖 `MemIni`/地图描述文件解析（`TMapDescList` 记录族） |
| 20 | `TWarrContinueHitManager` | 1067 | 4 | 11987-12105 | **REAL 4** | ✅ **已落地（切片3）**。`CanOpenMagic` 主体被原文 `{...}` 整段注释 ⇒ 恒 True（P17-ASIS-02） |
| 21 | `THttpClient` | 1081 | 7 | 12210-12433 | NP | 依赖 `WinINet`；`Post`(12286-12419) 为 WinINet 包装 |
| 22 | `TUserCenterManager` | 1125 | 39 | 12490-13338 | NP | 依赖 `IdHTTP`/`SuperObject`(`ISuperObject`)/`THttpClient`；整族**用户中心（微信/手机登录）**，与 `MShare` 主线解耦 |
| | | | **185** | | `REAL 4 / NP 181 / ASIS 0` | |

> ⚠ `TWarrContinueHitManager` 与 `M2Engine/ObjPlayer.pas:1398` **同名**——`p12` 已登记这条同名风险（`并行报告-p12-e2only-review.md:839-840`）。两个是**不同实现**，逐副本，不得合并。

### 4.2 185 条类方法逐例程对账表

| # | Class | Routine | Src line | Kind | Status |
|---:|---|---|---:|---|---|
| 1 | THttpThread | WebPagePost | 4465 | function | NP |
| 2 | THttpThread | Post | 4548 | procedure | NP |
| 3 | THttpThread | Create | 4613 | constructor | NP |
| 4 | THttpThread | Execute | 4624 | procedure | NP |
| 5 | THttpThread | CallPayMentURL | 4637 | procedure | NP |
| 6 | THttpThread | GetPayMentURL | 4671 | procedure | NP |
| 7 | TImageList | Create | 4710 | constructor | NP |
| 8 | TImageList | Destroy | 4717 | destructor | NP |
| 9 | TImageList | ImageOf | 4731 | function | NP |
| 10 | TImageList | IndexOf | 4736 | function | NP |
| 11 | TImageList | Initialize | 4741 | procedure | NP |
| 12 | TImageList | Finalize | 4745 | procedure | NP |
| 13 | TImageList | GetCount | 4749 | function | NP |
| 14 | TImageList | SetIndex | 4754 | procedure | NP |
| 15 | TImageList | GetCachedGrayImage | 4761 | function | NP |
| 16 | TImageList | GetCachedBrightImage | 4771 | function | NP |
| 17 | TImageList | GetCachedImage | 4781 | function | NP |
| 18 | TImageList | FreeOldMemorys | 4791 | procedure | NP |
| 19 | TImageList | ClearCache | 4801 | procedure | NP |
| 20 | TTilesList | ClearCache | 4812 | procedure | NP |
| 21 | TTilesList | Destroy | 4823 | destructor | NP |
| 22 | TTilesList | FreeOldMemorys | 4838 | procedure | NP |
| 23 | TTilesList | GetGameImages | 4850 | function | NP |
| 24 | TTilesList | Initialize | 4895 | procedure | NP |
| 25 | TTilesList | Finalize | 4923 | procedure | NP |
| 26 | TSmTilesList | ClearCache | 4945 | procedure | NP |
| 27 | TSmTilesList | Destroy | 4956 | destructor | NP |
| 28 | TSmTilesList | FreeOldMemorys | 4971 | procedure | NP |
| 29 | TSmTilesList | GetGameImages | 4983 | function | NP |
| 30 | TSmTilesList | Initialize | 5027 | procedure | NP |
| 31 | TSmTilesList | Finalize | 5055 | procedure | NP |
| 32 | TMonImageList | IndexOf | 5077 | function | NP |
| 33 | TMonImageList | ImageOf | 5108 | function | NP |
| 34 | TMonImageList | Initialize | 5184 | procedure | NP |
| 35 | TMonImageList | Finalize | 5198 | procedure | NP |
| 36 | THumImageList | IndexOf | 5212 | function | NP |
| 37 | THumImageList | GetWHumGrayImg | 5244 | function | NP |
| 38 | THumImageList | GetWHumBrightImg | 5299 | function | NP |
| 39 | THumImageList | GetWHumImg | 5356 | function | NP |
| 40 | THumImageList | Initialize | 5434 | procedure | NP |
| 41 | THumImageList | Finalize | 5460 | procedure | NP |
| 42 | TWeaponImageList | IndexOf | 5488 | function | NP |
| 43 | TWeaponImageList | GetWWeaponGrayImg | 5523 | function | NP |
| 44 | TWeaponImageList | GetWWeaponBrightImg | 5590 | function | NP |
| 45 | TWeaponImageList | GetWWeaponImg | 5655 | function | NP |
| 46 | TWeaponImageList | Initialize | 5724 | procedure | NP |
| 47 | TWeaponImageList | Create | 5757 | constructor | NP |
| 48 | TWeaponImageList | Destroy | 5763 | destructor | NP |
| 49 | TWeaponImageList | ClearCache | 5779 | procedure | NP |
| 50 | TWeaponImageList | FreeOldMemorys | 5790 | procedure | NP |
| 51 | TWeaponImageList | Finalize | 5801 | procedure | NP |
| 52 | TStateItemImages | Create | 5823 | constructor | NP |
| 53 | TStateItemImages | Destroy | 5828 | destructor | NP |
| 54 | TStateItemImages | GetCount | 5842 | function | NP |
| 55 | TStateItemImages | ClearCache | 5847 | procedure | NP |
| 56 | TStateItemImages | ImageOf | 5857 | function | NP |
| 57 | TStateItemImages | LooksOf | 5887 | function | NP |
| 58 | TStateItemImages | IndexOf | 5909 | function | NP |
| 59 | TStateItemImages | Initialize | 5931 | procedure | NP |
| 60 | TStateItemImages | Finalize | 5950 | procedure | NP |
| 61 | TStateItemImages | FreeOldMemorys | 5963 | procedure | NP |
| 62 | TStateItemImages | GetCachedImage | 5974 | function | NP |
| 63 | TDnItemImages | Create | 6005 | constructor | NP |
| 64 | TDnItemImages | Destroy | 6010 | destructor | NP |
| 65 | TDnItemImages | GetCount | 6025 | function | NP |
| 66 | TDnItemImages | ClearCache | 6030 | procedure | NP |
| 67 | TDnItemImages | GetCachedGray | 6040 | function | NP |
| 68 | TDnItemImages | GetCachedBright | 6067 | function | NP |
| 69 | TDnItemImages | ImageOf | 6094 | function | NP |
| 70 | TDnItemImages | LooksOf | 6122 | function | NP |
| 71 | TDnItemImages | IndexOf | 6144 | function | NP |
| 72 | TDnItemImages | Initialize | 6166 | procedure | NP |
| 73 | TDnItemImages | Finalize | 6185 | procedure | NP |
| 74 | TDnItemImages | FreeOldMemorys | 6198 | procedure | NP |
| 75 | TDnItemImages | GetCachedImage | 6209 | function | NP |
| 76 | TBagItemImages | Create | 6240 | constructor | NP |
| 77 | TBagItemImages | Destroy | 6245 | destructor | NP |
| 78 | TBagItemImages | ClearCache | 6260 | procedure | NP |
| 79 | TBagItemImages | GetCount | 6270 | function | NP |
| 80 | TBagItemImages | ImageOf | 6275 | function | NP |
| 81 | TBagItemImages | LooksOf | 6302 | function | NP |
| 82 | TBagItemImages | IndexOf | 6324 | function | NP |
| 83 | TBagItemImages | Initialize | 6345 | procedure | NP |
| 84 | TBagItemImages | Finalize | 6364 | procedure | NP |
| 85 | TBagItemImages | FreeOldMemorys | 6377 | procedure | NP |
| 86 | TBagItemImages | GetCachedImage | 6388 | function | NP |
| 87 | TNpcImageList | IndexOf | 6419 | function | NP |
| 88 | TNpcImageList | Initialize | 6444 | procedure | NP |
| 89 | TNpcImageList | Finalize | 6462 | procedure | NP |
| 90 | TImageEvent | Create | 7240 | constructor | NP |
| 91 | TImageEvent | Destroy | 7246 | destructor | NP |
| 92 | TImageEvent | UnLoadGameImages | 7253 | procedure | NP |
| 93 | TImageEvent | LoadGameImages | 7347 | procedure | NP |
| 94 | TImageEvent | Add | 7914 | procedure | NP |
| 95 | TImageEvent | AddDynamic | 7919 | procedure | NP |
| 96 | TImageEvent | AddImageList | 7924 | procedure | NP |
| 97 | TImageEvent | GetCount | 7929 | function | NP |
| 98 | TImageEvent | GetImages | 7934 | function | NP |
| 99 | TImageEvent | GetDynamicCount | 7939 | function | NP |
| 100 | TImageEvent | GetDynamicGameImages | 7944 | function | NP |
| 101 | TImageEvent | Initialize | 7949 | procedure | NP |
| 102 | TImageEvent | Finalize | 7989 | procedure | NP |
| 103 | TImageEvent | ClearCache | 8037 | procedure | NP |
| 104 | TImageEvent | FreeOldMemorys | 8133 | procedure | NP |
| 105 | TImageEvent | Clear | 8171 | procedure | NP |
| 106 | TMapDesc | Create | 10323 | constructor | NP |
| 107 | TMapDesc | Destroy | 10328 | destructor | NP |
| 108 | TMapDesc | LoadFromFile | 10349 | procedure | NP |
| 109 | TMapDesc | Get | 10435 | function | NP |
| 110 | THumEffectList | Create | 10455 | constructor | NP |
| 111 | THumEffectList | Finalize | 10462 | procedure | NP |
| 112 | THumEffectList | GetWHumEffectBrightImg | 10476 | function | NP |
| 113 | THumEffectList | GetWHumEffectGrayImg | 10560 | function | NP |
| 114 | THumEffectList | GetWHumEffectImg | 10643 | function | NP |
| 115 | THumEffectList | IndexOf | 10728 | function | NP |
| 116 | THumEffectList | Initialize | 10763 | procedure | NP |
| 117 | TWeaponEffectList | Create | 10794 | constructor | NP |
| 118 | TWeaponEffectList | Finalize | 10800 | procedure | NP |
| 119 | TWeaponEffectList | GetWWeaponEffectBrightImg | 10813 | function | NP |
| 120 | TWeaponEffectList | GetWWeaponEffectGrayImg | 10845 | function | NP |
| 121 | TWeaponEffectList | GetWWeaponEffectImg | 10876 | function | NP |
| 122 | TWeaponEffectList | IndexOf | 10907 | function | NP |
| 123 | TWeaponEffectList | Initialize | 10931 | procedure | NP |
| 124 | TCboWeaponEffectList | Finalize | 10949 | procedure | NP |
| 125 | TCboWeaponEffectList | IndexOf | 10962 | function | NP |
| 126 | TCboWeaponEffectList | Initialize | 10986 | procedure | NP |
| 127 | TCboWeaponList | Finalize | 11004 | procedure | NP |
| 128 | TCboWeaponList | IndexOf | 11017 | function | NP |
| 129 | TCboWeaponList | Initialize | 11041 | procedure | NP |
| 130 | TCboHumList | Finalize | 11059 | procedure | NP |
| 131 | TCboHumList | IndexOf | 11072 | function | NP |
| 132 | TCboHumList | Initialize | 11096 | procedure | NP |
| 133 | TCboHumEffect | Finalize | 11114 | procedure | NP |
| 134 | TCboHumEffect | IndexOf | 11127 | function | NP |
| 135 | TCboHumEffect | Initialize | 11151 | procedure | NP |
| 136 | TWarrContinueHitManager | Create | 11987 | constructor | **REAL** |
| 137 | TWarrContinueHitManager | CanOpenMagic | 11993 | function | **REAL**（原文如此：方法体整段注释 ⇒ 恒 True） |
| 138 | TWarrContinueHitManager | CanUseMagic | 12071 | function | **REAL** |
| 139 | TWarrContinueHitManager | UseMagic | 12105 | procedure | **REAL** |
| 140 | THttpClient | Create | 12210 | constructor | NP |
| 141 | THttpClient | Destroy | 12217 | destructor | NP |
| 142 | THttpClient | GetInternetStatusCode | 12223 | function | NP |
| 143 | THttpClient | Get | 12235 | function | NP |
| 144 | THttpClient | Post | 12286 | function | NP |
| 145 | THttpClient | SetInternetSecurityOption | 12420 | function | NP |
| 146 | THttpClient | SetInternetTimeout | 12433 | function | NP |
| 147 | TUserCenterManager | Create | 12490 | constructor | NP |
| 148 | TUserCenterManager | Destroy | 12497 | destructor | NP |
| 149 | TUserCenterManager | GetSubAccountCount | 12514 | function | NP |
| 150 | TUserCenterManager | GetSubAccountItem | 12519 | function | NP |
| 151 | TUserCenterManager | GetRoleString | 12528 | function | NP |
| 152 | TUserCenterManager | GetSubAccountItemString | 12539 | function | NP |
| 153 | TUserCenterManager | GetSubAccountString | 12567 | function | NP |
| 154 | TUserCenterManager | GetCanRequestWechatQrCode | 12575 | function | NP |
| 155 | TUserCenterManager | DoUserCenterEvent | 12580 | procedure | NP |
| 156 | TUserCenterManager | GetWechatQrCodeBusyStatus | 12585 | function | NP |
| 157 | TUserCenterManager | GetPhoneCodeBusyStatus | 12590 | function | NP |
| 158 | TUserCenterManager | GetLoginBusyStatus | 12595 | function | NP |
| 159 | TUserCenterManager | IsPhoneNumber | 12600 | function | NP |
| 160 | TUserCenterManager | OnThreadFished | 12618 | procedure | NP |
| 161 | TUserCenterManager | PostJsonData | 12629 | function | NP |
| 162 | TUserCenterManager | PostJsonData | 12661 | function | NP |
| 163 | TUserCenterManager | RequestWechatQrcode | 12699 | procedure | NP |
| 164 | TUserCenterManager | RequestWechatQrcodeProc | 12725 | procedure | NP |
| 165 | TUserCenterManager | ResetWechatQrCode | 12791 | procedure | NP |
| 166 | TUserCenterManager | RequestPhoneVerifyCode | 12807 | procedure | NP |
| 167 | TUserCenterManager | RequestPhoneLogin | 12834 | procedure | NP |
| 168 | TUserCenterManager | RequestPhoneVerifyCodeProc | 12867 | procedure | NP |
| 169 | TUserCenterManager | RequestPhoneLoginProc | 12917 | procedure | NP |
| 170 | TUserCenterManager | RequestAccountLoginProc | 12969 | procedure | NP |
| 171 | TUserCenterManager | RequestAccountLogin | 13019 | procedure | NP |
| 172 | TUserCenterManager | ReadSubAccountFromJson | 13051 | procedure | NP |
| 173 | TUserCenterManager | RequestSubAccountProc | 13081 | procedure | NP |
| 174 | TUserCenterManager | RequestSubAccountList | 13131 | procedure | NP |
| 175 | TUserCenterManager | SetHostInfo | 13159 | procedure | NP |
| 176 | TUserCenterManager | GetLoginMode | 13164 | function | NP |
| 177 | TUserCenterManager | SetLoginMode | 13169 | procedure | NP |
| 178 | TUserCenterManager | SetSubAccountTestData | 13175 | procedure | NP |
| 179 | TUserCenterManager | WechatPollingProc | 13220 | procedure | NP |
| 180 | TUserCenterManager | StartWechatPolling | 13298 | procedure | NP |
| 181 | TUserCenterManager | StopWechatPolling | 13313 | procedure | NP |
| 182 | TUserCenterManager | TryLock | 13323 | function | NP |
| 183 | TUserCenterManager | Lock | 13328 | procedure | NP |
| 184 | TUserCenterManager | Unlock | 13333 | procedure | NP |
| 185 | TUserCenterManager | UpdateWechatQrCode | 13338 | procedure | NP |

**小计**：`真实体 0 + NotPorted 185 + 原文如此 0 = 185` ✅

> 两条同名重载已分列：#161/#162 `TUserCenterManager.PostJsonData`（原文 12629 用 `TIdHTTP`，12661 用 `THttpClient`）。

### 4.3 单元级例程（168 条）——已落地的 32 条

**切片1（21 条）**：

| # | 例程 | 原文行 | 托管位置 | 状态 | 关键判据（原文） |
|---:|---|---:|---|---|---|
| 1 | `IsOverLapItem(Item)` | 4084 | `MShareFunctions.IsOverLapItem(TClientItem*)` | REAL | `Name<>'' && StdMode in [0,2,3,31,40,41,42,46,47] && OverLap>0` |
| 2 | `IsOverLapItem(Item1,Item2)` | 4090 | 同名重载 | REAL | 上列 3 项 **两件物品各判一遍** + `Name` 相等 + `StdMode` 相等 |
| 3 | `IsUnOverLapItem(Item)` | 4096 | 同名 | REAL | 第 3 项换成 `Item.Dura > 0` |
| 4 | `GetFeatureLen(nLen)` | 4102 | `GetFeatureLen` | REAL | `nLen = SizeOf(THumFeature)` → `g_nHumFeature`；`elif = TMonFeature` → `g_nMonFeature` |
| 5 | `GetRGB(c256)` | 4111 | `GetRGB` | REAL | `RGB(g_DefColorTable[c].rgbRed, .rgbGreen, .rgbBlue)` |
| 6 | `RGB32(C,BitCount)` | 4116 | `RGB32` | REAL | `BitCount=16` → `RGB(C and $F8 shr 8, C and $FC shr 3, C and $F8 shl 3)`，else 原样 |
| 7 | `GetJobName(nJob)` | 8957 | `GetJobName` | REAL | `case 0/1/2` → `g_sWarriorName/g_sWizardName/g_sTaoistName`，else `g_sUnKnowName` |
| 8 | `GetSexName(nSex)` | 8973 | `GetSexName` | REAL | `case 0/1` → `'男'/'女'`，else `''` |
| 9 | `ActorXYToMapXY` | 4181 | 同名 | REAL | `X*48 div 32` / `Y*32 div 32`（**Y 轴恒等，照抄**） |
| 10 | `MapXYToActorXY` | 4187 | 同名 | REAL | `X*32 div 48` / `Y*32 div 32` |
| 11 | `MapToScreen` | 4193 | 同名 | REAL | `Round(nScreenWH*nMapXY/nMapWH)` ⇒ `ToEven` |
| 12 | `ScreenToMap` | 4198 | 同名 | REAL | `Round(nMapWH*nScreenXY/nScreenWH)` |
| 13 | `DeleteNumber` | 8536（副本 8619） | `DeleteNumber` | REAL | 尾数连数位 → `StrToIntDef(sNum,0)`；`nC=0` 原样；全数字 → `''` |
| 14 | `DeleteFileExt` | 8557（副本 8640） | `DeleteFileExt` | REAL | **`Pos('.')` = 第一个点**（非最后一个） |
| 15 | `tick_diff` | 11659 | `tick_diff` | REAL | `end>=start` 相减，else `High(Cardinal)-start+end`（**uint** 语义） |
| 16 | `GetTickCount_Ex` | 11667 | `GetTickCount_Ex` | REAL | `TimeGetTime()` ⇒ `(uint)Environment.TickCount` |
| 17 | `HpAddUnit(V)` | 11672 | `HpAddUnit` | REAL | `>=1e8` → `%.2fE`（**除 1e8，不是 1e9**）；`>=1e5` → `V div 10000`+`W` |
| 18 | `IntToHexN(V,Digits)` | 11484 | `IntToHexN` | REAL（**带缺陷**，见 P17-DEF-02） | `Digits` 当**进制**；`Digits>10` 或 `<2` → `''` |
| 19 | `GetInputBoxInFilterList` | 11959 | 同名 | REAL | 逐字符查 `@ < > $`；否则整串转小写做 `Pos(词,串)>0` |
| 20 | `IsAttackAction(Action)` | 11781 | 同名 | REAL | 18 个具名动作码 + `SM_CUSTOM_HIT001..+CUSTOM_MAGIC_COUNT-1` |
| 21 | `GetMaxBagCount` | 11762 | 同名 | REAL | `DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount` |

**切片3（11 条）**：

| # | 例程 | 原文行 | 托管位置 | 状态 | 关键判据（原文） |
|---:|---|---:|---|---|---|
| 22 | `IsInContinuous` | 3267 | `MShareFunctions.IsInContinuous` | REAL | `Result := g_boContinuous;`（同行 `InterlockedCompareExchange` 是**注释**） |
| 23 | `ProcessFileNameSpecialChar` | 11565 | 同名 | REAL | 9 个非法字符逐字符替换：`/→{  \→}  :→;  *→@  ?→!  "→~  <→(  >→)  |→-` |
| 24 | `GetTempDir` | 11682 | 同名 | REAL | `GetTempPath(...)` ⇒ `Path.GetTempPath()`（**带结尾反斜杠**，已与 Win32 逐字节比对） |
| 25 | `MakeTempFileName` | 11690 | 同名 | REAL | `QueryPerformanceCounter` 成功 ⇒ `%x`（无前导零）；失败 ⇒ `%.8x%.4x`；`Length(FileExt)>0` 才加点 |
| 26 | `_FileSize` | 8984 | `FileSize` | REAL | `FindFirst` 失败 ⇒ `0`，否则 `SearchRec.Size`（`uint`） |
| 27 | `GetAbsolutePathEx` | 6542 | 同名 | REAL | `PathCombine(Dest, BasePath, RelativePath)`（**不解析 `..`**） |
| 28 | `ShiftStateToPlugShiftState` | 11786 | 同名 | REAL | 七个**独立 if**（非 else-if）累加：`Shift=1 Alt=2 Ctrl=4 Left=8 Right=16 Middle=32 Double=64` |
| 29 | `CheckBlockListSys` | 11454 | 同名 | REAL | `SM_HEAR/GROUP/GUILD` 切 `':'`；`SM_CRY` 切 `':'` 后 `RightStr(名, Len-3)`；`SM_WHISPER` 切 `'='`；再切 `' '`；黑名单**大小写不敏感**；**异常 ⇒ False** |
| 30 | `ShiftState_Shift` … `ShiftState_Double` | 2582-2588 | 同名常量 | REAL | 7 个常量 = 1/2/4/8/16/32/64 |
| 31 | `g_MyBlacklist` 承载 | 2462 | `MShareGlobals.g_MyBlacklist` | REAL | 原文唯一读点 11475 的 `THashedStringList.IndexOf` 语义（不敏感 + 首个命中） |
| 32 | `g_boContinuous` 承载 | 2442 | `MShareGlobals.g_boContinuous` | REAL | 原文 3267 的唯一读点 |

| # | 例程 | 原文行 | 托管位置 | 状态 | 关键判据（原文） |
|---:|---|---:|---|---|---|
| 33 | `GetHintNameFontName` | 11703 | `MShareFunctions.GetHintNameFontName` | REAL | `Result := g_ClientConfig.sShowHintFontName;`（短串长度 20） |
| 34 | `GetHintNameFontSize` | 11708 | 同名 | REAL | `Result := btShowHintNameFontSize;`（`Byte`→`Integer` 提升） |
| 35 | `GetHintNameFontStyle` | 11713 | 同名 | REAL | `case btShowHintNameFontBold of 0:入参 / 1:[] / 2:[fsBold]` —— **无 `else`**（越界 ⇒ 空集，非入参） |
| 36 | `GetHintNameFontStroke` | 11725 | 同名 | REAL | `case btShowHintNameFontStroke of 0:入参 / 1:False / 2:True else False` |
| 37 | `GetHintFontSize` | 11735 | 同名 | REAL | `Result := btShowHintOtherFontSize;` |
| 38 | `GetHintFontStyle` | 11740 | 同名 | REAL | `case btShowHintOtherFontBold of …` —— 同 11713，**无 `else`** |
| 39 | `GetHintFontStroke` | 11752 | 同名 | REAL | `case btShowHintOtherFontStroke of …` —— 同 11725 |

> 这 7 条的读取源已在切片A 补进 `MirForms.TConfigClient`（§4.6 #4-#10）；
> 与既有 `GUI/Share/FStateSeams.cs::MShareHintFont` seam 的**退役细则与兜底值差异**见 §5.3（R-2）。

**小计**：`真实体 39 + NotPorted 126 + 原文如此 3 = 168` ✅

**未移植的 133 条**主要落在**资源族**（`THttpThread` 6、`CreateGameImages`/`GetObjs`/`GetMonImg`/`GetObjInfo` 图像族、`LoadSkillDescList`/`GetItemDesc`/`GetTzItemDesc`/`GetGodBlessItem`/`GetFengHaoItem` 等文本表族、`TImageList` 家族相关、`EncryptImageFileListPassword`/`SetMachineID`/`DebugOutStr` 等平台族）。
其中**文本表族（8 条）本轮未落地**，原因见 §9.4（原文那 8 条**读的全局从未被声明** + **没有任何调用点**）。

### 4.4 单元级全局（`g_*`）——已落地的 231 条

| 分组 | 条数 | 原文行域 | 位置 |
|---|---:|---|---|
| fstate B-6 优先级 ①（7 条 seam + 15 条同段 + 2 条接缝真身） | 24 | 1740-2283 | `ClientGlobals.cs` |
| 主流程优先级 ②（移动/鼠标、攻击/动作节流、小地图、地图尺寸、属性、人物/目标、名称/文本、声音、连接/服务器、渲染、动作/计数、模块 CRC、更新/机器码、目录/路径、开关/测试） | 202 | 1281-2328 | `MShareGlobals.Core.cs` |
| 调色板 / 过滤表（`g_DefColorTable` / `g_InputBoxFilterList`） | 2 | 2243 / HGE | `MShareGlobals.Core.cs` |
| 切片3：`g_MyBlacklist`(2462) / `g_boContinuous`(2442) | 2 | 2442 / 2462 | `MShareGlobals.Core.cs` |
| **合计** | **231** | | |

### 4.5 切片3 新增：`TWarrContinueHitManager` 与承载类

| 交付物 | 位置 | 计数对账 | 备注 |
|---|---|---|---|
| `TWarrContinueHitManager`（客户端侧） | `MShare/**/TWarrContinueHitManager.cs` | 真实体 4 / NotPorted 0 / 原文如此 1（`CanOpenMagic`） | 与 `GXX.M2Server.Engine.TWarrContinueHitManager` **同名不同实现**，已用反射用例锁死"签名不同、不可改指" |
| ~~`MShareWarrConfigSeam`~~ | **切片B 已删除** | — | 切片3 的临时跨区 seam；切片A 补字段后**已退役**（见 §5.3）。属"退役不计覆盖率分子"单列项 |
| `MShareFunctions` 新增平台族 | 同文件 | 真实体 11 | 见 §4.3 切片3 表 |

> **原文全文无调用点**（本轮实测）：`GetTempDir`(11682) 与 `MakeTempFileName`(11690) 在 `MShare.pas` 里**只有声明与实现，没有任何调用**。
> 因此它们目前是"可移植但无人消费"的例程——本切片仍按 1:1 落地并配用例，**不因此虚增进度**（它们确实在 168 条口径内）。

### 4.6 切片A：`MirForms.TConfigClient` 补 10 字段 —— 「原文声明 → 我补的字段 → 两侧一致性」逐行表

**为什么补在这个类**：调度方 CR-6 裁定"准"，并把 `!GXX.CSharp/src/GXX.Client/GUI/Mir/MirForms.cs` 加进本车道分区。
原文类型就是本类所对的 `TConfigClient`（`MShare.pas:493-703`）。

**⚠ 先说清一处原文缺陷（它决定了"读谁"）**：原文那 11 处引用写的是 **`g_ClientConfig`**，
而 `MShare.pas` 全文**没有**这个标识符的声明 —— 2180 行真正的声明是
`g_ConfigClient:TConfigClient;`（`g_ConfigClient` vs `g_ClientConfig`，多一个 `i`）。
⇒ 语义上就是 `g_ConfigClient`；原文这 11 处**原本编译不过**（与 `boNextTime43Hit` 同类）。登记为 **P17-DEF-04**。

**类型口径选择：沿用 Core 口径**（`Grobal2.Types5.cs` 的同名字段类型）。理由：
① 这些字段在 Core 的 `TClientConfig` 里**已有同名同类型的真身**，将来把 `MShareGlobals.g_ConfigClient`
   收敛为 `GXX.Core.Protocol.TClientConfig` 是**机械替换**（正是调度方希望的方向）；
② 本类既有约定就是 Delphi `Boolean` → `byte`（见类内 `boShow1024` 等 13 个既有字段）。

| # | 原文声明 | 原文行 | 我补的字段（`MirForms.TConfigClient`） | Core 对应（`Grobal2.Types5.cs`） | 两侧一致性 |
|---:|---|---:|---|---|---|
| 1 | `g_ClientConfig.boDisableWarrContinueHit:Boolean`（被读，无声明） | 12004 / 12079 | `public byte boDisableWarrContinueHit;` | `:341 byte boDisableWarrContinueHit;` | ✅ 名称逐字同、类型逐字同 |
| 2 | `g_ClientConfig.nWarrContinueHitMinInterval`（被读，无声明；原文别处写作 `Integer`） | 12101 | `public uint nWarrContinueHitMinInterval;` | `:342 uint nWarrContinueHitMinInterval;` | ✅ 名称逐字同、类型随 Core；**理由见注①** |
| 3 | `g_ClientConfig.ArrDisableWarrContinueHitIDs`（被读，无声明；`array[0..9] of Word`） | 12010 / 12088 / 12112 | `public WordArray10 ArrDisableWarrContinueHitIDs;` | `:343 WordArray10 ArrDisableWarrContinueHitIDs;` | ✅ 名称逐字同、类型逐字同 |
| 4 | `g_ClientConfig.sShowHintFontName`（`array[0..20] of AnsiChar` ⇒ `string[20]`，被读，无声明） | 11705 | `public HintFontNameBuffer sShowHintFontName;` + `ShowHintFontName` 访问器（长度 20） | `:49 fixed byte sShowHintFontName[21];` + `:372 ShowHintFontName`（`ShortStr.Get(p, 20)`） | ✅ **语义与长度逐字同**；承载形态因 C# 限制不同（**注②**） |
| 5 | `g_ClientConfig.btShowHintNameFontSize:Byte` | 11710 | `public byte btShowHintNameFontSize;` | `:50 byte btShowHintNameFontSize;` | ✅ 逐字同 |
| 6 | `g_ClientConfig.btShowHintNameFontBold:Byte` | 11715 | `public byte btShowHintNameFontBold;` | `:51` 同名同类型 | ✅ 逐字同 |
| 7 | `g_ClientConfig.btShowHintNameFontStroke:Byte` | 11727 | `public byte btShowHintNameFontStroke;` | `:52` 同名同类型 | ✅ 逐字同 |
| 8 | `g_ClientConfig.btShowHintOtherFontSize:Byte` | 11737 | `public byte btShowHintOtherFontSize;` | `:53` 同名同类型 | ✅ 逐字同 |
| 9 | `g_ClientConfig.btShowHintOtherFontBold:Byte` | 11742 | `public byte btShowHintOtherFontBold;` | `:54` 同名同类型 | ✅ 逐字同 |
| 10 | `g_ClientConfig.btShowHintOtherFontStroke:Byte` | 11754 | `public byte btShowHintOtherFontStroke;` | `:55` 同名同类型 | ✅ 逐字同 |

**注① 为什么 `nWarrContinueHitMinInterval` 选 `uint` 而不是原文别处的 `Integer`**：
它只在一个表达式里被读（原文 12101）：
`Result := tick_diff(FLastUseMagicTick, MyGetTickCount) >= g_ClientConfig.nWarrContinueHitMinInterval + 100;`
`tick_diff` 的返回是 `Cardinal`（**无符号、带回绕**）。若用 `Integer`，`nWarrContinueHitMinInterval + 100`
会先按有符号算，再与 `Cardinal` 比较时被隐式转成无符号 —— 当配置值为负或接近 `High(Integer)` 时
两侧行为分叉。用 `uint` 让 `>=` 的运算符语义与 `tick_diff` **完全一致**，且与 Core 真身逐字对齐。
已用边界用例锁死：`CanUseMagic_IntervalZero_StillRequiresTheHardcoded100ms`、
`CanUseMagic_TickDiffWraparound_IsHandledByCardinalSemantics`。

**注② 为什么 `sShowHintFontName` 用 `HintFontNameBuffer`（`[InlineArray(21)]`）而不是 `fixed byte[21]`**：
`TConfigClient` 是 **class**，C# 不允许 `fixed` 定长缓冲区做 class 成员（**CS1642**）。
改用嵌套 `[InlineArray(21)] struct HintFontNameBuffer`，并在访问器里用 `Unsafe.As` 取首元素引用后取地址
—— 与 Core 在 **struct** 上用 `fixed` 的是**同一份 21 字节布局**，访问器长度同为 20。
已用 `GetHintNameFontName_ShortStringIsLengthLimitedTo20Bytes` 锁死截断行为。

**注③ 一句话说明这批字段的性质**：这 10 个字段**都没写在 `MShare.pas` 的 `TConfigClient` 里**
（它们是"被引用但未声明"），所以此处不是"抄原文记录体"，而是**按引用点反推字段名与语义**、
再把类型对齐到 Core 的**已有真身**。因此**没有创造第三份定义**——Core 里那 10 个字段就是定义。

**注④ 调度方点名的两个 fstate 字段（单列回报）**：

| fstate 诉求 | 现状 | 结论 |
|---|---|---|
| `boNPCGuiCanMove` | **`MirForms.TConfigClient` 里早就有了**（`MirForms.cs:35`，`public byte boNPCGuiCanMove;`） | ✅ **不需要我补**（原文行 `MShare.pas` 亦无该字段的正式声明；它由 `ConfigShare`/`MirForms` 既有承载） |
| `DMerchantDlgHelp` | 它是 **UI 控件**，不是配置字段：`MirForms.cs` 的 `class TFrmDlg` 里已有 `public TDxImageButton DMerchantDlgHelp;`（原文 `FState.pas TFrmDlg`） | ⚠️ **不是"补 `g_ClientConfig` 字段"能解决的**。若 fstate 需要的是布局/别名，应走 `TFrmDlg` 侧（见 §8.3 CR-9） |
| （fstate 实际被挡的字段）`sHomePage` | `MirForms.TConfigClient` **没有**；`FStateClMainSeam.sHomePage` 是它的临时承载（p14 D-P14-12） | 🔶 **我没有落**（见 §8.3 CR-10）—— 原文对它有**两个互相冲突的声明**，尺寸未定，先请裁定 |


---

## 5. 可直接退役 / 改指的 seam 清单（供集成方执行）

> **本车道不动 `GUI/Share/**`**（那是 `p14-client-fstate` 的在飞分区）。以下动作**全部由集成方或 p14 执行**。

### 5.1 可直接退役（真身已等价，改指后语义不变）

| # | 接缝成员 | 接缝位置 | 真身 | 退役动作 | 风险 |
|---:|---|---|---|---|---|
| 1 | `FStateMShareSeam.g_dwQueryMsgTick` | `GUI/Share/FStateSeams.cs:593` | `MShareGlobals.g_dwQueryMsgTick` (1825) | 33 处读点批量改指，删除接缝字段 | 无。类型同为 `uint`，初值同为 `0`，判据严格 `>` 不动 |
| 2 | `FStateMShareSeam.g_dwDealActionTick` | 同 596 | `MShareGlobals.g_dwDealActionTick` (1824) | 同上 | 无 |
| 3 | `FStateMShareSeam.g_dwChallengeActionTick` | 同 599 | `MShareGlobals.g_dwChallengeActionTick` (2076) | 同上 | 无 |
| 4 | `FStateMShareSeam.g_boDealEnd` | 同 602 | `MShareGlobals.g_boDealEnd` (2040) | 同上 | 无 |
| 5 | `FStateMShareSeam.g_nDealGold` | 同 605 | `MShareGlobals.g_nDealGold` (2038) | 同上 | 无 |
| 6 | `FStateMShareSeam.g_boChallengeEnd` | 同 608 | `MShareGlobals.g_boChallengeEnd` (2074) | 同上 | 无 |
| 7 | `FStateMShareSeam.g_nChallengeGold` | 同 611 | `MShareGlobals.g_nChallengeGold` (2068) | 同上 | 无 |
| 8 | `FStateMShareSeam.g_SellDlgItem` | 同 572 | `MShareGlobals.g_SellDlgItem` (2008) | 同上 | 无（同为 `TClientItem` 值承载） |
| 9 | `FStateMShareSeam.g_ExtBagOpenItemCount` | 同 575 | `MShareGlobals.g_ExtBagOpenItemCount` (1879) | 同上 | 无（同为 `ushort`） |
| 10 | `FStateMShareSeam.ResetForTests()` | 同 614 | 并入 `MShareGlobalsReset.ResetForTests()` | 删除接缝 Reset，改调 `MShareGlobalsReset.ResetForTests()` | 低。两个 Reset 的字段集**不重叠**，先合并再删（顺序：先让 `MShareGlobalsReset` 覆盖接缝那 10 个字段——**已覆盖**，再删接缝 Reset） |

**退役前置条件（已满足）**：`MShareGlobalsReset.ResetForTests()` **已经**复位上表全部 10 个字段（含 `g_SellDlgItem` / `g_ExtBagOpenItemCount`），所以把 `FStateSeams.cs:981` 的 `FStateMShareSeam.ResetForTests();` 换成 `MShareGlobalsReset.ResetForTests();` 是**无损**的。

### 5.2 可改指但**不能退役**（真身缺，或跨分区）

| # | 接缝 | 原因 | 建议 |
|---:|---|---|---|
| 11 | `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName` | 真身是 `g_SelDeleteHumanInfo : TUserCharacterInfo` 的 `sChrName` 字段，**该记录类型在 `Grobal2`/`Core` 是定长缓冲版且无短串访问器**；本车道切口不含 `FState.pas` 读写该记录的那段 | 保持现状。见 §8 CR-1（需 `Core.Protocol` 侧补 `TUserCharacterInfo.sChrName` 短串属性） |
| 12 | `Scenes/MiniMapRender.cs:225` `MiniMapMessageState.QueryMsgTick` | **跨分区**（Scenes 车道），且它是**另一份承载** | 集成方决定合并方向：建议改为引用 `MShareGlobals.g_dwQueryMsgTick`（原文只有一份） |
| 13 | `ConfigShareSeam.HumBagNoUseItemCount()` / `.GetMaxBagCount()` | **跨分区**（`GUI/GameConfig` 车道）。本车道已交付真身 `MShareFunctions.GetMaxBagCount()`；`HumBagNoUseItemCount` 真身在原文 9049（本切片未落地） | `GetMaxBagCount` 可改指真身；`HumBagNoUseItemCount` 待本车道后续切片 |
| 14 | `FStateClMainSeam.sHomePage` / `FStateClMainSeam` 6 成员 | `frmMain`、`g_ClientConfig` 属 `ClMain.pas`/配置单元，**不在 `MShare.pas`** | 不属本车道，保持现状 |
| 15 | `FStateScreenSeam`（`DScreen.ClearHint` 计次留痕） | `DScreen:TDrawScreen` 真身在 **MShare.pas:1456（声明）/ DrawScrn.pas（实现）**——**声明在 MShare，实现在 DrawScrn** | 本切片**未**落地 `DScreen` 变量（`TDrawScreen` 类型不存在）。见 §8 CR-2 |

### 5.3 切片B/C 的 seam 变动（**退役不计覆盖率分子**，按调度方要求单列）

| # | seam | 位置 | 本次动作 | 条目数 | 说明 |
|---:|---|---|---|---:|---|
| R-1 | ~~`MShareWarrConfigSeam`~~（整类） | `MShare/**/MShareWarrConfigSeam.cs`（**已删除**） | ✅ **退役（本车道自己执行）** | **1 个类 / 4 个成员** | 切片3 的临时跨区 seam（3 字段 + Reset）。切片A 把字段补进 `MirForms.TConfigClient` 后，`TWarrContinueHitManager` 已**改指真身** `MShareGlobals.g_ConfigClient.<字段>`；Reset 逻辑也已并入 `MShareGlobalsReset.ResetForTests()`。**`git status` 可见 `delete mode`** |
| R-2 | `MShareHintFont`（3 条 + 3 个 Handler + Reset） | `GUI/Share/FStateSeams.cs:455-478`（**跨分区，本车道不动**） | 🔶 **真身已就绪，退役待集成方执行** | **3 条函数 / 8 个成员** | 真身 = `MShareFunctions.GetHintFontSize/GetHintFontStyle/GetHintFontStroke`（切片C）。**退役细则与语义差异见下表** |

**R-2 的退役细则（供集成方执行）**：

| seam 成员 | 真身 | 目标调用点（`file:line`） | ⚠ 语义差异（改指前必须知道） |
|---|---|---|---|
| `MShareHintFont.GetHintFontSize()` | `MShareFunctions.GetHintFontSize()` | `DrawScrnEnv.cs:597`；`FStatePure.cs:93/118/130` | **兜底值不同**：seam 在 handler 为 null 时返回 **9**；真身读 `g_ConfigClient.btShowHintOtherFontSize`，**未装载时是 0**。⇒ 直接改指会让"未注入 handler 的测试/未装载配置的启动期"从 `9` 变成 `0`（字号变小）。**必须先确认 9 是不是原文的默认值** —— 原文 11735 无默认值语义（直接读字段），所以 `9` 是**旧车道自己定的兜底**，不是原文。 |
| `MShareHintFont.GetHintFontStyle(FontStyles)` | `MShareFunctions.GetHintFontStyle(...)` | `DrawScrnEnv.cs:600`；`FStatePure.cs:94/119/131` | 兜底不同：seam 返回**入参原样**；真身在字段为 `0` 时也返回入参原样，但字段**不在 0/1/2** 时返回 `fsNone`（原文无 `else` 的照抄行为）。⇒ 行为更忠实，但"字段脏值"下会与旧兜底分叉。 |
| `MShareHintFont.GetHintFontStroke(IsStroke)` | `MShareFunctions.GetHintFontStroke(...)` | `DrawScrnEnv.cs:603`；`FStatePure.cs:94/119/131` | 兜底不同：seam 返回**入参原样**；真身字段 `0` 时返回入参原样、`1/2` 时强制、其它返回 `false`（原文 `else`）。 |
| `GetHintFontSizeHandler` / `GetHintFontStyleHandler` / `GetHintFontStrokeHandler` | —— | 测试注入点：`GuiSharePureTests.cs:243-245 / 260` | 改指后这 3 个注入点无对象；那 4 处测试需改为**直接设置 `MShareGlobals.g_ConfigClient.btShowHintOtherFont*` 字段**（真身没有可注入 handler）。 |
| `MShareHintFont.ResetForTests()` | —— | `GuiShareHandlersTests.cs:60`；`GuiSharePureTests.cs:54` | 改指为 `MShareGlobalsReset.ResetForTests()`（已包含 7 个 hint 字段的复位）。 |

> **本车道为什么不动 R-2**：`FStateSeams.cs` 不在本车道分区（`!` 清单里没有它），
> 且那 3 处注入点被 GUI/Share 自己的测试依赖。**改指是集成方动作**，本车道只提供真身 + 差异说明。

### 5.4 退役计数（收口用）

| 类别 | 数量 |
|---|---:|
| **本车道本轮自行退役**（整类删除） | **1 个类**（`MShareWarrConfigSeam`，含 4 个成员） |
| **本车道提供真身、待集成方退役** | **3 条函数**（`MShareHintFont` 的 3 条，含 3 个 Handler + Reset 共 8 个成员） |
| 截至上一轮的待集成方退役项（§5.1 的 10 项 + §5.2 的 3 项可改指） | 10 + 3 |

---

## 6. 原文缺陷（`原文如此` + 差异断言锁死）

> 分类：`P17-ASIS-xx` = 原文奇怪的写法但**不是缺陷**，照抄即可；`P17-DEF-xx` = 原文**真缺陷**，照抄 + 锁死。

### P17-DEF-01 · `g_FontArr` 字面量在 UTF-8 镜像里被截断（**转写产物，非原文缺陷**）

- **原文**：`MShare.pas:1690`
  ```
  g_FontArr:array[0..MAXFONT - 1] of string = ('宋体', '新宋体', '仿宋', '楷体',
                                               'Courier New', 'Arial', 'MS Sans Serif', 'Microsoft Sans Serif');
  ```
- **缺陷**：镜像文件里第 2/3 项被转写成 `'新宋体?` / `'仿宋?`（**没有闭合引号**），这是 `Source/`(GBK) → UTF-8 转写的字节截断产物，**不是原文缺陷**。
- **处理**：值取自同一字面量在 `HGE.pas` 的副本（`MAXFONT = 8`）。
- **锁死用例**：`MShareGlobalsP17Tests.FontArr_MatchesOriginalEightEntries`（断言长度 8 且 `[0]='宋体'`、`[3]='楷体'`、`[7]='Microsoft Sans Serif'`）。

### P17-ASIS-01 · `ActorXYToMapXY` / `MapXYToActorXY` 的 Y 轴是恒等式

- **原文**：`MShare.pas:4183-4184` / `4189-4190`
  ```pascal
  nX := nCurrX * 48 div 32;
  nY := nCurrY * 32 div 32;   // ← 恒等于 nCurrY
  ```
- **原文如此**：Y 轴的 `* 32 div 32` 与 Actor↔Map 换算无关（显然是从 X 轴复制后忘了改），实际是恒等映射。
- **处理**：照抄，不"修正"。
- **锁死用例**：`ActorXYToMapXY_And_Back_UseIntegerDivisionLikeOriginal`（断言 `nY` 恒等、`nX` 走 `48/32` 与 `32/48` 的整数截断）。

### P17-ASIS-01a · `g_FontArr` 的镜像字面量被转写截断（**转写产物，非原文缺陷**）

- **原文**：`MShare.pas:1690`
  ```
  g_FontArr:array[0..MAXFONT - 1] of string = ('宋体', '新宋体', '仿宋', '楷体',
                                               'Courier New', 'Arial', 'MS Sans Serif', 'Microsoft Sans Serif');
  ```
- **缺陷**：镜像文件里第 2/3 项被转写成 `'新宋体?` / `'仿宋?`（**没有闭合引号**），这是 `Source/`(GBK) → UTF-8 转写的字节截断产物，**不是原文缺陷**。
- **处理**：值取自同一字面量在 `HGE.pas` 的副本（`MAXFONT = 8`）。
- **锁死用例**：`MShareGlobalsP17Tests.FontArr_MatchesOriginalEightEntries`（断言长度 8 且 `[0]='宋体'`、`[3]='楷体'`、`[7]='Microsoft Sans Serif'`）。

### P17-DEF-02 · `IntToHexN` **名不副实**（真·原文缺陷）

- **原文**：`MShare.pas:11484-11519`
  ```pascal
  function IntToHexN(const V, Digits:Integer):string;
  const
    CSTR = '00000000000000000000000000000000';
    Convert2:array[0..9] of Char = ('0', '1', ..., '9');
  begin
    Result := '';
    if Digits > 10 then Exit;
    if Digits < 2 then Exit;
    ...
      P1^ := Convert2[I mod Digits];
      I := I div Digits;
    ...
  end;
  ```
- **缺陷**：函数名叫 `IntToHexN`（"转 N 位十六进制"），但实现把 `Digits` 当**进制**用（`mod`/`div`），并且 `Convert2` 只有 10 个字符，于是：
  1. **`Digits = 16` 恒返回 `''`**（被 `if Digits > 10 then Exit` 挡住）——**没有任何十六进制形态**；
  2. `Digits ∈ [2,10]` 时才工作，且对非 2 的幂进制（如 3/5/9）会输出**该进制的数字串**，不是十六进制。
- **处理**：**1:1 照抄**（含 `Digits` 当进制、含 `>10` 早退）。
- **锁死用例**：
  - `IntToHexN_UsesDigitsAsRadix`（`(255,10)="255"`、`(255,2)="11111111"`、`(255,3)="100110"`、`(255,4)="3333"`、`(255,5)="2010"`、`(255,8)="377"`、`(255,9)="313"`、`(31,8)="37"`）
  - `IntToHexN_OutOfRangeDigitsReturnsEmpty`（含 **`(255,16)=""`**、`(255,1)=""`、`(255,11)=""`）

### P17-DEF-03 · `GetInputBoxInFilterList` 对 `g_InputBoxFilterList = nil` 会崩

- **原文**：`MShare.pas:2243` `g_InputBoxFilterList:TStringList = nil;`（**初值 nil**）
- **原文缺陷**：`11977` 直接 `for I := 0 to g_InputBoxFilterList.Count - 1`，**没有 `Assigned()` 判断**。若在加载过滤词表之前调用，Delphi 会访问空指针。
- **处理**：托管侧对 `null` **返回 `false`**（不抛 NRE），并在用例 `GetInputBoxInFilterList_SubstringListIsCaseInsensitiveOnInput` 末段显式锁死该行为。
- **差异断言**：这是**唯一一处托管侧比原文更安全**的地方，已登记为 P17-DEF-03（属「原文如此 + 安全化」，不是偏离）。

---

### P17-DEF-04 · `g_ClientConfig` 在 `MShare.pas` **从未声明**（切片A 新增）

- **原文**：`MShare.pas:2180` 真正的声明是
  ```pascal
  g_ConfigClient:TConfigClient;
  ```
  但 `TWarrContinueHitManager`(12004/12079/12101) 与 hint 字体族(11705/11710/11715/11727/11737/11742/11754)
  引用的是 **`g_ClientConfig`**（多一个 `i`）。
- **原文缺陷**：`g_ClientConfig` 这个标识符在 `MShare.pas` **全文没有声明**（`git grep` 式全文扫描：11 处**全是读点**，0 处声明）。
  ⇒ 原文这 11 处**原本编译不过**，与 `boNextTime43Hit`（见 P17-ASIS-02）**同类悬空引用**。
- **处理**：按语义认定它就是 `g_ConfigClient`；10 个字段补进本类所对的 `TConfigClient`
  （`MirForms.TConfigClient`），并让实现从 `MShareGlobals.g_ConfigClient` 读。
- **锁死用例**：`ConfigureWarr_ActuallyLandsOnTheConfigClient`、`NewConfigFields_UseCoreAlignedTypes`、
  `ResetForTests_ZeroesAllSevenHintFields`，以及切片C 的全部 7 条函数用例。
- **附带发现**：`CanOpenMagic` 里被注释掉的 `boNextTime43Hit`（P17-ASIS-02）也是同一类"引用了不存在的标识符"。
  两处合起来说明：**原文这一版是在删改过程中留下的中间态**，不能用"原文能编译"作为推断前提。

### P17-ASIS-02 · `TWarrContinueHitManager.CanOpenMagic` 方法体被原文**整段注释**（切片3 新增）

- **原文**：`MShare.pas:11993-12069`
  ```pascal
  function TWarrContinueHitManager.CanOpenMagic(MagicID:Word):Boolean;
  {
  var
    I: Integer; TempID: Integer; IsFound: Boolean;
  }
  begin
    Result := True;
  {
    if not g_ClientConfig.boDisableWarrContinueHit then Exit;
    ...
      43:  Result := not boNextTime43Hit;      // ← boNextTime43Hit 在 MShare.pas **从未声明**
    ...
  }
  end;
  ```
- **原文如此**：变量声明块（11994-11999）与整个逻辑体（12003-12068）都被 `{ }` 注释掉，
  方法**实际只剩 12001 的 `Result := True;`** —— **永远返回 True，完全无视入参**。
  旁证：被注释的代码里引用了 `boNextTime43Hit`（无 `g_` 前缀），该标识符在 `MShare.pas` 全文
  **没有任何声明** ⇒ 那段代码**根本编译不过**，这正是它被整段注释的原因。
- **处理**：**照抄这一事实**（`CanOpenMagic` 只 `return true`），**不"恢复"**被注释的逻辑。
  恢复会引入一段原文从未生效的行为，属虚构。
- **锁死用例**：
  - `CanOpenMagic_AlwaysTrue_BodyIsEntirelyCommentedOutInTheOriginal`（4 组入参，含 `0` / `65535`）
  - `CanOpenMagic_IgnoresTheDisableSwitchAndTheManagedTable`（把注释块里所有会导向 `False` 的前置条件都摆好，仍恒 `True`）

---

## 7. 偏离（D-P17-xx）

| 编号 | 偏离 | 决定 | 影响面 | 偿还方式 |
|---|---|---|---|---|
| **D-P17-01** | `ClientGlobals.cs` 里 `MShareGlobals` 由 `static class` 改为 `static partial class` | 必须改（否则「不另起第二套 MShareGlobals」与「新子目录放其余实现」两条硬约束无法同时满足） | 零。`partial` 不改变类型标识、反射结果、可访问性 | 无须偿还。已用用例 `FStateSeamCounterparts_AreIndependentStatics_NoSecondGlobalsClass` 反射锁死「程序集内 `MShareGlobals` 唯一」 |
| **D-P17-02** | `g_DefColorTable` 用同一份托管 `MShareGlobals.Palette256` **逆向反解**出 `TRGBQuad` 通道，而不是从 `HGE.pas` 读真实 `T256ColorTable` | `HGE.pas` 未移植，`g_DefColorTable` 无真实数据源。为保证 `GetRGB(c)` 与既有 `Palette256[c]` **严格一致**，按 `r \| (g<<8) \| (b<<16)` 的反变换填通道字节 | `GetRGB` 的 256 项；其余读 `g_DefColorTable` 的代码目前不存在 | `HGE.pas` 移植后改为读真实表，并用「256 项逐项相等」用例（**已写**）守住回归 |
| **D-P17-03** | `g_boOpenMerchantBigDlg` 在 `ClientGlobals.cs` 是 `byte`（不是 `bool`） | **沿用既有声明**，不改类型（该字段在切片1 之前就由车道1 以 `byte` 承载，且可能有既有读点依赖） | 读点须写 `!= 0` 而非直接当 `bool` | 本车道后续切片统一为 `bool`，或由集成方一次性收敛 |
| **D-P17-04** | `TWarrContinueHitManager` 需要的 3 个 `g_ClientConfig` 字段，改用**本车道分区内**的 `MShareWarrConfigSeam` 承载（而非直接给车道1 的 `MirForms.TConfigClient` 加字段） | `MirForms.cs` **不在本车道分区**，越区写入会触发 MODIFIED-EXISTING 告警；该文件也不在本车道的 `!` 授权清单里 | `TWarrContinueHitManager` 读 seam 而非 `MShareGlobals.g_ClientConfig`；字段名/类型与 `Grobal2.Types5.cs:341-343` 的 `TClientConfig` **逐字一致**，将来收敛为机械替换 | 见 CR-6：`MirForms.TConfigClient` 补上这 3 个字段（或 `g_ClientConfig` 收敛为 `Core.Protocol.TClientConfig`）后，把 `MShareWarrConfigSeam.X` 改成 `MShareGlobals.g_ClientConfig.X`、删除 seam。**本切片已把这一替换点写在 seam 的类注释里** |

> **D-P17-04 的越区自纠**：本切片最初把 3 个字段直接加进了 `MirForms.cs`，随后判定该文件不在我的分区，
> **已回滚**该编辑（`git diff` 对 `MirForms.cs` 为空），改为有界 seam。工作树最终状态**不含任何分区外改动**。

---

## 8. 跨区请求 / 裁定记录（需其它车道/集成方配合，本车道不越界）

### 8.1 调度方裁定（本轮收到，已执行）

| # | 裁定 | 本车道执行情况 |
|---:|---|---|
| **CR-1** | `TUserCharacterInfo.sChrName` 短串访问器**由调度方补**（`src/GXX.Client/Scenes/Scenes.cs:36`），做完通知；在此之前 `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName` **不要动** | ✅ 未动该字段。§5.1 #11 仍记「不可退役」，待通知 || **CR-2** | `DScreen` 归属**随"定义该类型的单元"** ⇒ 归 `DrawScrn.pas` 一侧；`MShare` 里的只是前向声明 | ✅ `MShare.pas:1456` 的 `DScreen` 全局**不在本车道移植范围**，不落地。`FStateScreenSeam` 的退役改由 DrawScrn 车道承接 |
| **CR-4** | `GetKeyDownStr`/`GetKey` **一份实现、归声明它的单元** ⇒ 保留 **MShare 超集版（含 `var nKey` 出参）**，让 `ConfigShare` 改指；改指由集成方执行 | 🔶 **本轮未落地**（见 §9.2）：目标调用点清单已备好（§8.2），待本车道落 `GetKeyDownStr` 后由集成方改指 |
| **CR-5** | HTTP/用户中心 52 条**本轮不做架构决定**，登记为**架构待决项**、保持未移植且可见；可从进度分母排除但须单列 | ✅ 已排除出可推进分母并在 §9.5 单列；**未造任何空壳** |

### 8.2 CR-4 的**目标调用点清单**（供集成方执行；期望签名如下）

裁定为「MShare 版超集」，故 **MShare 侧必须先落地**，然后集成方把 `ConfigShare` 侧改指过来。

**期望签名（本车道将落地的形态）**：

```csharp
// GXX.Client.GUI.Mir.MShareFunctions
/// <summary>MShare.pas:4338 nested function GetKey(Key:Word):string（含 var nKey 出参）。</summary>
public static string GetKey(ushort Key, out int nKey);

/// <summary>MShare.pas:4336 function GetKeyDownStr(Key:Word; Shift:TShiftState; var nKey:Integer):string。</summary>
public static string GetKeyDownStr(ushort Key, GXX.Client.GUI.Share.TShiftState Shift, out int nKey);
```

**需改指的调用点（`ConfigShare` 侧）**：

| 文件:行 | 现签名 | 期望签名 | 备注 |
|---|---|---|---|
| `src/GXX.Client/GUI/GameConfig/ConfigShare.cs:73` | `ConfigShare.GetKey(ushort Key)` → `string`（**无 `nKey` 出参**） | `MShareFunctions.GetKey(ushort Key, out int nKey)` | 原文 `MShare.pas:4341-4342` 同时写 `nKey := Key`；ConfigShare 版丢了该出参 |
| `src/GXX.Client/GUI/GameConfig/ConfigShare.cs:154` | `ConfigShare.GetKeyDownStr(ushort Key, DelphiShiftState Shift, bool IncludeFN = false)` | `MShareFunctions.GetKeyDownStr(ushort Key, TShiftState Shift, out int nKey)` | ⚠ **两个差异必须一起处理**：① `ConfigShare` 版多一个 `IncludeFN` 形参（MShare 版没有 F 键特判）；② `ConfigShare` 用的是 `DelphiShiftState`，MShare 版用 `Share.TShiftState`。改指前需先裁决"以哪一版的语义为准" |

> ⚠ **改指前必须先解决的语义分歧**：`ConfigShare.cs:159-165` 有 `IncludeFN` 形参与 F 键特判，
> 而 `MShare.pas:4444` 的 `GetKeyDownStr` **没有**该形参（F 键在 MShare 版里靠 `(Key >= 186) and (Key <= 222)` 之外的另一条路）。
> 换言之两者**不是严格超集关系**，而是各有分支。裁定为"MShare 版为超集"与代码事实有出入，
> **建议集成方按调用点逐个核对后再改指**（本车道已在报告指出该风险，未擅自合并）。

### 8.3 跨区请求

| # | 请求 | 目标分区 | 阻塞了什么 |
|---:|---|---|---|
| **CR-1** | （已由调度方承接）`TUserCharacterInfo` 补 `sChrName` 短串访问器 | `src/GXX.Client/Scenes/Scenes.cs:36` | `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName` 的退役（§5.1 #11） |
| **CR-2** | （已裁定）`DScreen` 归 `DrawScrn.pas` | DrawScrn 车道 | `FStateScreenSeam` 的退役 |
| **CR-3** | `GUI/Share/FStateSeams.cs` 执行 §5.1 的 10 项退役（改指 `MShareGlobals`） | `p14-client-fstate` / 集成方 | 无阻塞，仅清理接缝 |
| **CR-5** | `Scenes/MiniMapRender.cs:225` 的 `QueryMsgTick` 合并为单一全局 | `p10-client-scrn` / 集成方 | 无阻塞，仅一致性 |
| **CR-6** | **【已裁定"准" + 授权】给 `MirForms.cs:23 TConfigClient` 补字段**，或把 `MShareGlobals.g_ConfigClient` 收敛为 `GXX.Core.Protocol.TClientConfig` | ✅ **本轮已完成**：`MirForms.cs` 已加进本车道分区，补了 10 字段 + 1 包装类型（§4.6）。**注意**：调度方明令**不要**改 `ClientGlobals.g_ConfigClient` 的**类型** ⇒ 收敛到 `Core.Protocol.TClientConfig` 只作**建议**登记（§8.4），未执行 |
| **CR-7** | **【新增】`GetKeyDownStr`/`GetKey` 落地后的改指**（CR-4 的第二步） | `p10-client-mirconfig`（`GUI/GameConfig/ConfigShare.cs`） | §8.2 的语义分歧需先裁决；本车道**尚未落地**这 2 条（§9.2），故不构成第三份实现 |
| **CR-9** | **【新增】`DMerchantDlgHelp` 不是 `g_ClientConfig` 字段** —— 它是 `FState.pas TFrmDlg` 上的 UI 控件，托管侧已在 `GUI/Mir/MirForms.cs::TFrmDlg` 落地（`public TDxImageButton DMerchantDlgHelp;`） | 调度方 → 转告 `p14-client-fstate` | fstate 的 D-P14-16 ② 若指"控件别名/布局"，应走 `TFrmDlg` 侧而非配置字段（§4.6 注④） |
| **CR-10** | **【新增】`sHomePage` 的尺寸需先裁定** —— 原文对它有两个**互相冲突**的声明：`TClientParam` 分支 `array[0..251] of Char`(252) vs `sHomePage:string[255]`(256)；Core 的 `TClientConfig` 用的是 `fixed byte sHomePage[200]`(200) | 调度方 / `Core.Protocol` | fstate 的 D-P14-12（`FStateClMainSeam.sHomePage` 退役）**卡在这里**。三处尺寸三种值 ⇒ 需裁定以哪份为准，再补进 `MirForms.TConfigClient`（与 Core 对齐 or 与原文对齐）。**本车道未擅自落**（§4.6 注④） |
| **CR-11** | **【新增，仅建议，未执行】把 `MShareGlobals.g_ConfigClient` 收敛为 `GXX.Core.Protocol.TClientConfig`** | 调度方裁决 | 切片A 已让 10 个新字段的类型与 Core **逐字一致** ⇒ 收敛时是机械替换。但调度方明令**不要**在本轮改该全局的类型（会影响多条已合并车道），故**只登记建议、不执行**（§8.4） |

### 8.4 建议（未执行）：`g_ConfigClient` 类型的收敛路径

调度方明令"不要顺手改 `ClientGlobals.cs` 里 `g_ClientConfig` 的**类型**"，故此处**只登记建议**。

现状：`MShareGlobals.g_ConfigClient` 的类型是 `GXX.Client.GUI.Mir.TConfigClient`（**class**），
而 `GXX.Core.Protocol.TClientConfig`（**struct**）已经承载了本次新增的全部字段（且远多于本类）。
两个类型**并存**，风险是同一个"配置"有两份定义 —— 正是本工程反复登记的事故类型。

**收敛可行性（本切片已把地基铺平）**：
- 本次新增的 10 个字段的**名称与类型与 Core 逐字一致**（§4.6 逐行表的"两侧一致性"列全 ✅）；
- 唯一形态差异是 `sShowHintFontName`（本类用 `[InlineArray(21)]` 包装，Core 用 `fixed` + struct 字段）
  —— 因为 **class 不能有 `fixed` 成员**（CS1642）；收敛成 struct 后这个差异**自然消失**。
- 因此收敛动作 = 把 `g_ConfigClient` 的类型换成 `Core.Protocol.TClientConfig` + 删除 `MirForms.TConfigClient`
  + 把既有 13 个字段的读写点名字对齐。**这是一次跨多条车道的重构**，须调度方统一排期。**本车道不做。**

**注意**：本次补字段**没有**加重收敛难度（正相反：把它往 Core 口径靠了一步）。
`MirForms.TConfigClient` 仍是既有 13 个字段的承载者，本车道只是**追加**了 10 个同名同类型字段。

---

## 9. 未完成 / 阻塞（如实登记，**没做的不填 0**）

### 9.1 本轮（切片3）未做

| 项 | 数量 | 原因 | 下一步 |
|---|---:|---|---|
| 类方法 | **181** | 其中 **129** 条依赖未移植的 `GameImages`/`Wzl`/`Wil`/`Pak`；另 **52** 条为 CR-5 架构待决项 | 见 9.2；图像族需等宿主单元 |
| 单元级例程 | **133** | 见 9.4（文本表族本轮实测**不可落地**）与 §9.3 | 见 9.2 |
| 全局 | **约 229** | 多为图像/网络目录句柄（`g_WMainImages` 族、`g_Wh*Images` 族、`g_AntiPlugDll*` 族）——**有类型但类型本身未移植**（`TGameImages`/`TUibImages`/`TUpdateEngine`/`TMemoryStreamEx`/`TCriticalSection`…）。强行落地只能造出无人消费的壳 | 随宿主单元分批落地 |

### 9.2 切片3 的四个切口：实际结果

调度方要求按我 §9.2 的顺序①②③④推进。**实际结果**：

| 切口 | 计划 | 实际 | 说明 |
|---|---|---|---|
| ① `TWarrContinueHitManager` 4 条 | 落地 | ✅ **4/4 落地**（含 2 条原文如此事实的照抄） | 见 §4.5；`CanOpenMagic` 恒 True（P17-ASIS-02） |
| ② 文本表族 8 条 | 落地 | ❌ **0/8，本轮不可落地** | 实测阻塞，见 §9.4（**新增实测结论**） |
| ③ 平台族 12 条 | 落地 | ✅ **11/12 落地** + **切片C 补齐 hint 字体族 7 条** | 落：`IsInContinuous`/`ProcessFileNameSpecialChar`/`GetTempDir`/`MakeTempFileName`/`_FileSize`/`GetAbsolutePathEx`/`ShiftStateToPlugShiftState`(+7 常量)/`CheckBlockListSys`；**切片C 又落 7 条 hint 字体族**（原估 4 条，实为 7 条）⇒ 该族**已全部落地** |
| ④ `TMapDesc` 4 条 | 落地 | ❌ **0/4，本轮未落地** | 依赖 `TMapDescList`（原文 258-265 记录）与 `MemIni` 文本解析；本轮预算用于 ①②③ + 切片A/B/C，如实登记未做 |
| **切片A**（调度方 CR-6 追加） | 补字段 | ✅ **10 字段 + 1 包装类型** | 见 §4.6 逐行表；`MirForms.cs` 由调度方加进本车道分区 |
| **切片B**（调度方 CR-6 追加） | 退役 seam | ✅ **1 个类 / 4 个成员退役** | `MShareWarrConfigSeam.cs` 删除，改指 `g_ConfigClient` 真身；见 §5.3 R-1 |
| **切片C**（调度方 CR-6 追加） | hint 字体族 7 条 | ✅ **7/7 落地** | 见 §4.3 切片C 表；真身就绪 ⇒ `MShareHintFont` seam 可退役（§5.3 R-2） |

> **注**：hint 字体族实为 **7 条**（`GetHintNameFontName`(11703) / `GetHintNameFontSize`(11708) / `GetHintNameFontStyle`(11713) /
> `GetHintNameFontStroke`(11725) / `GetHintFontSize`(11735) / `GetHintFontStyle`(11740) / `GetHintFontStroke`(11752)），
> 比我上一轮估的 4 条多 3 条。它们全部只读 `g_ClientConfig` 的 7 个字段，切片A 补字段后即纯 BCL 逻辑，**已一次落完**。

### 9.3 明确阻塞（需外部条件）

- **`THttpThread` / `THttpClient` / `TUserCenterManager`（52 条）**：CR-5 已裁定为**架构待决项**，本轮**不做架构决定、不造空壳**，见 §9.5。
- **`TImageEvent`（16 条）**：`LoadGameImages`(7347-7914) 单方法 567 行，是 `MShare.pas` 最长的图像加载器，依赖 `GetGameImageFiles`/`CreateGameImages`/`GetPakFile` 等 6 条资源定位函数，全部依赖 `Wzl`/`Wil`/`Pak` 单元。
- **hint 字体族 7 条**：依赖 `TConfigClient` 的 7 个 hint 字段（CR-6）。

### 9.4 🔴 **文本表族（切口②）本轮不可落地的实测结论**（新增，纠正我自己的原判断）

我在上一轮报告 §9.2 把文本表族列为「性价比第 2」并说它「依赖 `TGHashStringList`/`TGStringList`（`Core.Util` 已有）」。**本轮实测证明该判断不成立**，原文这 8 条**读的全局在 `MShare.pas` 里从未被声明**，且**全文没有任何调用点**：

| 例程 | 原文行 | 它读的全局 | 该全局的声明 | 调用点 |
|---|---:|---|---|---|
| `LoadSkillDescList` | 9073 | `g_SkillDescList` | ❌ **`MShare.pas` 全文无声明** | ❌ 无 |
| `UnLoadSkillDescList` | 9127 | `g_SkillDescList` | ❌ 同上 | ❌ 无 |
| `GetSkillDesc` | 9140 | `g_SkillDescList` | ❌ 同上 | ❌ 无 |
| `LoadSkillUpgradeDescList` | 9156 | `g_SkillUpgradeDescList` | ❌ 无声明（2219 声明的是同名的**另一条**，且该段被注释） | ❌ 无 |
| `LoadItemDescList` | 9252 | `g_ItemDescList` | ❌ 无声明 | ❌ 无 |
| `GetItemDesc` | 9761 | `g_ItemDescList` + `g_CustomItemPropertyTextVarList` | 前者 ❌ / 后者 ✅(2521) | ❌ 无 |
| `GetTzItemDesc` | 10139 | `g_TzItemDescList` | ❌ 无声明 | ❌ 无 |
| `GetGodBlessItem` | 9876 | `g_GodBlessItemList` | ❌ 无声明 | ❌ 无 |
| `GetFengHaoItem` | 9965 | `g_FengHaoItemList` | ❌ 无声明 | ❌ 无 |

> **为什么这很重要**：`MShare.pas` 里 `g_ItemDescList`/`g_SkillDescList` 等名字**只出现在 `implementation` 段的函数体内**（读/写），
> **没有一处 `var` 声明**。这说明它们**本来就属于别的单元**（`ClMain.pas` 或 `ClFunc.pas`），
> 或者原文这个单元在移除某段代码时把声明一起删了、留下了悬空引用。
> **⇒ 正确做法不是在本车道凭空补一个全局**（那会造出第三份实现，违反 §14.2），
> 而是**先把这些全局的归属裁定清楚**（新增 **CR-8**）。
> 这 8 条因此在本轮**保持 NotPorted 且可见**，不填 0、不造假体。

### 9.5 🔶 架构待决项：HTTP / 用户中心族（CR-5 单列，**已移出进度分母**）

| 类 | 方法数 | 依赖 | 状态 |
|---|---:|---|---|
| `THttpThread` | 6 | `WinINet`（`InternetOpen`/`InternetConnect`/`HttpOpenRequest`/`HttpSendRequest`/`HttpQueryInfo`/`InternetReadFile`） | **架构待决**（未移植，可见） |
| `THttpClient` | 7 | `WinINet` + `TUri`/`URLEncode` | **架构待决**（未移植，可见） |
| `TUserCenterManager` | 39 | 上面两个 + `IdHTTP` + `SuperObject`(`ISuperObject`) + `TThread` 同步 | **架构待决**（未移植，可见） |
| **合计** | **52** | — | 需调度方裁定：自研 `HttpClient` 适配层 / 引第三方包 / 放弃该族 |

**待决点（供裁定）**：
1. `WinINet` 的三族句柄 + `INTERNET_FLAG_*` 语义，用 `System.Net.Http.HttpClient` 能否等价（`HttpQueryInfo` 的裸头读取在 `HttpClient` 里需要改形）？
2. `SuperObject`(`ISuperObject`) 是 `MShare.pas` 里**大量 JSON 读写的实际载体**（`JsonLoad`/`JsonSave`/`TUserCenterManager.*Proc` 都用它），
   而它**不是 MShare 的类**——托管侧目前**没有对应物**。这不只是"HTTP 适配层"问题，而是**一个类型系统的缺口**。
3. `TUserCenterManager` 的 39 条里有 12 条是 `*Proc(sParam:string)` 形态的线程回调，依赖 `TThread.Synchronize` 语义 ⇒ 需先定"托管侧线程模型"。

### 9.6 生命周期判定

`MShare` 保持 **`PARTIAL`**：
- 全量口径：例程 **36/353 = 10.20 %**，全局 **231/约460 ≈ 50.2 %**，类 **1/22**。
- **可推进口径（排除 CR-5 的 52 条）**：类方法 **4/133 = 3.01 %**。
**不满足 `MAPPED`**。升级 `MAPPED` 的判据（建议，供调度方校准）：
① `GameImages`/`Wzl`/`Wil`/`Pak` 四单元落地；② 文本表族的全局归属裁定（CR-8）；③ CR-6 收敛后落 hint 字体族 7 条；
④ 22 个类至少各有 1 条真实体。

---

## 10. 计数对账

| 口径 | 成员数 | 真实体 | NotPorted | 原文如此 | 等式 |
|---|---:|---:|---:|---:|---|
| 类方法（22 类） | 185 | **4** | 181 | 0 | `4+181+0=185` ✅ |
| 单元级例程 | 168 | **39** | 126 | **3** | `39+126+3=168` ✅ |
| 例程合计 | 353 | **43** | 307 | **3** | `43+307+3=353` ✅ |
| 全局 `g_*` | 约 460 | **231** | 约 229 | 0 | `231+约229+0=约460` ✅ |
| 类（`= class`） | 22 | **1** | 21 | 0 | `1+21+0=22` ✅ |
| 原文缺陷 / 原文如此（锁死） | — | — | — | **5**（P17-ASIS-01 / P17-ASIS-02 / P17-DEF-02 / P17-DEF-03 / P17-DEF-04） | — |
| 偏离 | — | — | — | **4**（D-P17-01/02/03/04） | — |
| **已退役 seam（不计覆盖率分子）** | — | **1 个类 / 4 个成员** | — | — | §5.3 R-1 ✅ |
| **待集成方退役（真身已就绪，亦不计分子）** | — | **3 条函数 / 8 个成员** | — | — | §5.3 R-2 🔶 |
| 🔶 架构待决（CR-5，已移出可推进分母） | 52 条方法 / 3 个类 | — | — | — | 见 §9.5 |

**逐文件交付明细**：

| 提交 | 文件 | 行数 | 内容计数 |
|---|---|---:|---|
| `54f3e9af` | `src/GXX.Client/GUI/Mir/ClientGlobals.cs` | +123/-1 | 24 条 `g_*` + `Reset` 同步 + `partial` |
| `54f3e9af` | `src/GXX.Client/GUI/Mir/MShare/MShareGlobals.Core.cs` | +765（新） | 202 条 `g_*` + `g_DefColorTable` + `g_InputBoxFilterList` + 私有 `BuildDefColorTable` |
| `54f3e9af` | `src/GXX.Client/GUI/Mir/MShare/MShareFunctions.cs` | +349（新） | 21 条真身 + 4 条私有 helper |
| `54f3e9af` | `src/GXX.Client/GUI/Mir/MShare/MShareTypes.cs` | +28（新） | `TRGBQuad` |
| `54f3e9af` | `tests/GXX.Client.Tests/MShareP17Slice1Tests.cs` | +641（新） | 33 个 `[Fact]/[Theory]` ⇒ 110 条用例 |
| `5227d242` | `docs/并行报告-p17-client-mshare.md` | +571（新） | 本报告（22 类表 + 185 条逐例程表 + 覆盖率 + seam 清单） |
| `50063f9e` | `src/GXX.Client/GUI/Mir/MShare/TWarrContinueHitManager.cs` | +139（新） | 类方法 4（1 条原文如此） |
| `50063f9e` | `src/GXX.Client/GUI/Mir/MShare/MShareWarrConfigSeam.cs` | +66（新） | 3 字段 + Reset（有界跨区 seam） |
| `50063f9e` | `src/GXX.Client/GUI/Mir/MShare/MShareFunctions.cs` | +约 230 | 11 条平台族真身 + 3 个注入点 + 7 个 `ShiftState_*` 常量 |
| `50063f9e` | `src/GXX.Client/GUI/Mir/MShare/MShareGlobals.Core.cs` | +约 20 | `g_MyBlacklist` + `g_boContinuous` |
| `50063f9e` | `src/GXX.Client/GUI/Mir/ClientGlobals.cs` | +约 10 | `Reset` 补 2 全局 + seam/注入点复位 |
| `50063f9e` | `tests/GXX.Client.Tests/MShareP17Slice3Tests.cs` | +约 660（新） | 44 个 `[Fact]/[Theory]` ⇒ 61 条用例 |
| `f1af8e57` | `src/GXX.Client/GUI/Mir/MirForms.cs` | +约 120 | **切片A**：`TConfigClient` 补 10 字段 + `HintFontNameBuffer`（`[InlineArray(21)]`）+ `ShowHintFontName` 访问器 |
| `f1af8e57` | `src/GXX.Client/GUI/Mir/MShare/MShareWarrConfigSeam.cs` | **−64（删除）** | **切片B 退役**：整类删除（4 个成员） |
| `f1af8e57` | `src/GXX.Client/GUI/Mir/MShare/TWarrContinueHitManager.cs` | 改 | 切片B：3 处读点改指 `MShareGlobals.g_ConfigClient.*` |
| `f1af8e57` | `src/GXX.Client/GUI/Mir/MShare/MShareFunctions.cs` | +约 110 | **切片C**：hint 字体族 7 条真身 + `TFontStyles` 别名 |
| `f1af8e57` | `src/GXX.Client/GUI/Mir/ClientGlobals.cs` | 改 | 切片A：10 个新字段的 `ResetForTests` 复位（含 `ShowHintFontName=""`） |
| `f1af8e57` | `tests/GXX.Client.Tests/MShareP17SliceCTests.cs` | +约 290（新） | 30 个 `[Fact]/[Theory]` ⇒ **30 条用例** |
| `f1af8e57` | `tests/GXX.Client.Tests/MShareP17Slice3Tests.cs` | 改 | 切片B：改指 `g_ConfigClient`，helper 由 seam 改为真身 |

---

## 11. 提交与门禁

| 提交 | 内容 | 门禁证据 |
|---|---|---|
| `54f3e9af` | 切片1：MShare 全局真身（fstate B-6）+ 首批 21 条纯函数 | **PASS**；`dotnet test exit code: 0` / `crash markers found: none` / `GATE: PASS`；5108 通过 / 0 失败 |
| `5227d242` | 切片2：185 条逐例程对账表 + 22 类表 + 实测覆盖率 + seam 退役清单 | **PASS**；同上；5108 通过 / 0 失败 |
| `50063f9e` | 切片3：`TWarrContinueHitManager`(4) + 平台族(11) + 黑名单/连击配置 | **PASS**；5169 通过 / 0 失败 |
| `416449f0` | 切片4：报告收口（CR-1…CR-5 裁定执行 + 文本表族实测结论 + 三段计数对账） | **PASS**；5169 通过 / 0 失败 |
| **`f1af8e57`** | **切片A/B/C**：`MirForms.TConfigClient` 补 10 字段 + 退役 `MShareWarrConfigSeam` + hint 字体族 7 条 | **PASS**；`dotnet test exit code: 0` / `crash markers found: none` / `GATE: PASS (build 0 error, test exit 0, no crash markers)`；**5199 通过 / 0 失败** |

**最新门禁命令（官方脚本，带 `-Repo`，不用 `--no-build`）**：
```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 `
  -Repo "D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare" `
  -Project GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj
```

---

## 12. 给集成方的动作清单

**可立即执行（无阻塞）**：
1. `FStateSeams.cs:981` 把 `FStateMShareSeam.ResetForTests();` 换成 `MShareGlobalsReset.ResetForTests();`（无损，§5.1 #10）。
2. 按 §5.1 的 10 项把 `FStateMShareSeam.*` 批量改指 `MShareGlobals.*`，然后删除那 10 个字段。
3. `GUI/GameConfig/ConfigShare.cs` 的 `GetMaxBagCount` 改指 `MShareFunctions.GetMaxBagCount()`（真身已交付）。

**需先裁定再执行**：
4. **CR-6**（已裁定"准"并授权本车道）✅ **本轮已完成**：`MirForms.TConfigClient` 补 10 字段 —— 连击三件套退役 `MShareWarrConfigSeam`（§5.3 R-1），hint 字体族 7 字段解锁 7 条例程（§4.3 切片C）。
   剩余由集成方执行的：**R-2**（`MShareHintFont` seam 退役，含 3 个测试注入点的改造，细则见 §5.3）。
5. **CR-7 / §8.2**：`GetKeyDownStr`/`GetKey` 的改指 —— ⚠ **先裁决 `IncludeFN` 形参与 `TShiftState` 类型的两处分歧**（§8.2 已列出目标调用点的文件:行与期望签名）。
6. **CR-8（新增）**：裁定 `g_SkillDescList` / `g_ItemDescList` / `g_TzItemDescList` / `g_GodBlessItemList` / `g_FengHaoItemList` / `g_SkillUpgradeDescList` 这些**在 `MShare.pas` 无声明却在其中被读写**的全局的**归属单元**（§9.4）⇒ 解锁文本表族 8 条。
7. **CR-5 / §9.5**：HTTP + 用户中心 52 条的架构决策（含 `SuperObject` 类型缺口与线程模型）。
8. **CR-9（新增，转告 fstate）**：`DMerchantDlgHelp` 是 `TFrmDlg` 上的 UI 控件、**不是**配置字段（§4.6 注④）。
9. **CR-10（新增）**：`sHomePage` 的三处尺寸冲突（252 / 256 / 200）需裁定 ⇒ fstate D-P14-12 卡在这里（§8.3）。
10. **CR-11（仅建议）**：`g_ConfigClient` 类型收敛到 `Core.Protocol.TClientConfig` —— 地基已铺平，**调度方明令本轮不做**（§8.4）。

**不可退役（等 CR-1）**：
11. `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName` —— 等调度方补 `TUserCharacterInfo.sChrName`。

---

## 13. 附：本轮（切片A/B/C）踩到的两个工程坑（建议记入规程）

1. **`TConfigClient` 是 class ⇒ 不能有 `fixed` 定长缓冲区成员**（CS1642）。
   原文 `sShowHintFontName:array[0..20] of AnsiChar` 不能直译成 `public unsafe fixed byte sShowHintFontName[21];`。
   解法：嵌套 `[InlineArray(21)] struct`（与 `Grobal2.Types4.cs:40 WordArray10` 同构），
   访问器里用 `Unsafe.As` 取首元素引用后取地址（`fixed (byte* p = someInlineArrayField)` 会 **CS8385**）。
   **凡把 Delphi 定长数组加进 class 都会遇到这一条。**

2. **`[InlineArray(N)]` 的越界是编译期错误（CS9166），不能写进 `Assert.Throws`。**
   我最初把"长度必须为 10"写成 `Assert.Throws<IndexOutOfRangeException>(() => arr[10] = 99)`
   —— **编译不过**。这类长度约束应改为"0..N-1 全可写"的**正向**用例（越界由编译器守，比运行时断言更强）。

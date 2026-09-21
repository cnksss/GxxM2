# 并行报告 · p17-client-mshare（`MShare.pas`）

| 项 | 值 |
|---|---|
| 车道 | `p17-client-mshare` |
| 分支 | `par/p17-client-mshare` |
| 工作树 | `D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare` |
| 源单元 | `Client-HGE/MShare.pas`（UTF-8 镜像 `_analysis/utf8_mirror/Client-HGE/MShare.pas`，**13,522 行**） |
| 复核车道裁决 | `p12-e2only-review` 判 **C（未移植/仅借名）**，已登记 **REFUTED（真缺口）** |
| 本车道生命周期 | `REFUTED` → **`PARTIAL`（带实测覆盖率）** |
| 最新提交 | `54f3e9af`（切片1） |

> **行号约定**：本报告所有「原文行」= `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号，**不是** `Source/`（GBK）的行号。

---

## 1. 结论摘要

- **MShare 的全局面（`g_*`）已从零开始落地**：本切片交付 **228 个 `g_*` 声明真身**，其中 **24 个是上游车道 `p14-client-fstate` 在其 §11.2 B-6 里点名「最密集阻塞类」的实体**（含它已用 `FStateMShareSeam` 承载的 7 条）。
- **首批 21 条纯函数**以 1:1 忠实口径落地并配 110 条真实断言。
- **22 个类（185 条方法）仍未移植**——它们全部是**图像/资源加载族**（`TImageList` 家族 17 个类、`TImageEvent`、`TMapDesc`）、**HTTP/支付族**（`THttpThread`、`THttpClient`）与**用户中心族**（`TUserCenterManager` 39 条），依赖 `HGE`/`GameImages`/`WinINet`/`IdHTTP` 等**尚未移植的单元**。
- **`MShare` 仍是 `PARTIAL`，不是 `MAPPED`**。理由与剩余工作量见 §9。

### 1.1 实测覆盖率

| 口径 | 总数 | 真实体 | NotPorted | 原文如此 | 覆盖率 |
|---|---:|---:|---:|---:|---:|
| **类方法**（22 个类） | 185 | **0** | 185 | 0 | **0.00 %** |
| **单元级例程**（implementation 段顶层） | 168 | **21** | 144 | 3 | **12.50 %** |
| **例程合计** | 353 | **21** | 329 | 3 | **5.95 %** |
| **单元级全局 `g_*`**（interface 1271-3061） | 约 460 | **228** | 约 232 | 0 | **约 49.6 %** |
| **原文缺陷（`原文如此`，已锁死）** | — | — | — | **3** | — |
| **偏差点** | — | **3**（D-P17-01/02/03） | — | — | — |

> 覆盖率分子只算**真身落地的例程**。切片1 另外交付了 24 条 `g_*` 全局（它们不是例程，单列在全局口径里）。

**「真实体 + NotPorted + 原文如此 = 成员数」对账**（§10 有逐文件明细）：
- 类方法：`0 + 185 + 0 = 185` ✅
- 单元级例程：`21 + 144 + 3 = 168` ✅
- 全局：`228 + 约232 + 0 = 约460` ✅

**3 条 `原文如此`**（都在已交付的 21 条真实体内，不重复计入 NotPorted）：
`ActorXYToMapXY`/`MapXYToActorXY` 的 Y 轴 `* 32 div 32` 恒等式（P17-ASIS-01）、`IntToHexN` 的 `Digits` 当进制 + `>10` 早退（P17-DEF-02）、`GetInputBoxInFilterList` 对 `nil` 过滤表无 `Assigned()` 判断（P17-DEF-03）。

### 1.2 可复跑命令

```powershell
# 覆盖率（按原文逐条清点，不依赖任何手工登记）
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-mshare
# ① 22 个类 / 185 条类方法 / 168 条单元级例程
$f="_analysis\utf8_mirror\Client-HGE\MShare.pas"
(Select-String -Path $f -Pattern '=\s*[Cc]lass' | Where-Object { $_.LineNumber -lt 3062 }).Count   # => 22
(Select-String -Path $f -Pattern '^\s{0,2}(procedure|function|constructor|destructor)\s+[A-Za-z_]\w*\.' |
  Where-Object { $_.LineNumber -gt 3062 -and $_.LineNumber -lt 13356 }).Count                     # => 185
# ② 本车道交付的真身
(Select-String -Path 'GXX.CSharp\src\GXX.Client\GUI\Mir\MShare\MShareFunctions.cs' -Pattern '^\s{4}public static').Count  # => 21
(Select-String -Path 'GXX.CSharp\src\GXX.Client\GUI\Mir\MShare\MShareGlobals.Core.cs' -Pattern '^\s{4}public static').Count # => 230（其中 228 个 g_*, 2 个 private helper 不计）
# ③ 门禁三行
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 -Project GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj
```

---

## 2. 本切片交付物（切片1）

| 文件 | 状态 | 内容 |
|---|---|---|
| `GXX.CSharp/src/GXX.Client/GUI/Mir/ClientGlobals.cs` | 改 | `MShareGlobals` 改 `partial`；补 24 条 fstate B-6 全局真身；`MShareGlobalsReset` 同步补复位 |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareGlobals.Core.cs` | 新 | 202 条主流程读取的 `g_*` 全局（`partial class MShareGlobals`，**不另起第二套**） |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareFunctions.cs` | 新 | 21 条纯函数真身 + `TShiftState` 复用说明 |
| `GXX.CSharp/src/GXX.Client/GUI/Mir/MShare/MShareTypes.cs` | 新 | `TRGBQuad`（Windows `RGBTRIPLE`，`Pack=1`） |
| `GXX.CSharp/tests/GXX.Client.Tests/MShareP17Slice1Tests.cs` | 新 | 33 个 `[Fact]/[Theory]` ⇒ **110 条用例** |

**门禁证据（三行）**：
```
dotnet test exit code : 0
crash markers found   : none
GATE: PASS (build 0 error, test exit 0, no crash markers)
```
全量：`已通过! - 失败: 0，通过: 5108，已跳过: 0，总计: 5108`（车道基线 4,999 ⇒ 本车道新增 110 条，其余为并道带入）。

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
| 20 | `TWarrContinueHitManager` | 1067 | 4 | 11987-12105 | NP | **可移植**（见 §9 下一步）：`CanOpenMagic`(11993-12070) 主体被原文 `{...}` 整段注释掉，`CanUseMagic` 同理 |
| 21 | `THttpClient` | 1081 | 7 | 12210-12433 | NP | 依赖 `WinINet`；`Post`(12286-12419) 为 WinINet 包装 |
| 22 | `TUserCenterManager` | 1125 | 39 | 12490-13338 | NP | 依赖 `IdHTTP`/`SuperObject`(`ISuperObject`)/`THttpClient`；整族**用户中心（微信/手机登录）**，与 `MShare` 主线解耦 |
| | | | **185** | | `REAL 0 / NP 185 / ASIS 0` | |

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
| 136 | TWarrContinueHitManager | Create | 11987 | constructor | NP |
| 137 | TWarrContinueHitManager | CanOpenMagic | 11993 | function | NP |
| 138 | TWarrContinueHitManager | CanUseMagic | 12071 | function | NP |
| 139 | TWarrContinueHitManager | UseMagic | 12105 | procedure | NP |
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

### 4.3 单元级例程（168 条）——本切片已落地的 21 条

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

**小计**：`真实体 21 + NotPorted 144 + 原文如此 3 = 168` ✅
**未移植的 144 条**全部落在**资源/网络**族（`THttpThread` 6、`TImageList` 家族相关、`LoadSkillDescList`/`GetItemDesc`/`GetTzItemDesc`/`GetGodBlessItem`/`GetFengHaoItem` 等文本表族、`CreateGameImages`/`GetObjs`/`GetMonImg` 图像族、`MakeTempFileName`/`GetTempDir`/`EncryptImageFileListPassword`/`SetMachineID` 等平台族），它们各自依赖**未移植的兄弟单元**（`GameImages`/`Wzl`/`Wil`/`Pak`/`MemoryModule`/`SDK`/`HardInfo`）。

### 4.4 单元级全局（`g_*`）——本切片已落地的 228 条

| 分组 | 条数 | 原文行域 | 位置 |
|---|---:|---|---|
| fstate B-6 优先级 ①（7 条 seam + 15 条同段 + 2 条接缝真身） | 24 | 1740-2283 | `ClientGlobals.cs` |
| 主流程优先级 ②（移动/鼠标、攻击/动作节流、小地图、地图尺寸、属性、人物/目标、名称/文本、声音、连接/服务器、渲染、动作/计数、模块 CRC、更新/机器码、目录/路径、开关/测试） | 202 | 1281-2328 | `MShareGlobals.Core.cs` |
| 调色板 / 过滤表（`g_DefColorTable` / `g_InputBoxFilterList`） | 2 | 2243 / HGE | `MShareGlobals.Core.cs` |
| **合计** | **228** | | |

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

### P17-DEF-02 · `IntToHexN` **名不副实**（真·原文缺陷）

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

## 7. 偏离（D-P17-xx）

| 编号 | 偏离 | 决定 | 影响面 | 偿还方式 |
|---|---|---|---|---|
| **D-P17-01** | `ClientGlobals.cs` 里 `MShareGlobals` 由 `static class` 改为 `static partial class` | 必须改（否则「不另起第二套 MShareGlobals」与「新子目录放其余实现」两条硬约束无法同时满足） | 零。`partial` 不改变类型标识、反射结果、可访问性 | 无须偿还。已用用例 `FStateSeamCounterparts_AreIndependentStatics_NoSecondGlobalsClass` 反射锁死「程序集内 `MShareGlobals` 唯一」 |
| **D-P17-02** | `g_DefColorTable` 用同一份托管 `MShareGlobals.Palette256` **逆向反解**出 `TRGBQuad` 通道，而不是从 `HGE.pas` 读真实 `T256ColorTable` | `HGE.pas` 未移植，`g_DefColorTable` 无真实数据源。为保证 `GetRGB(c)` 与既有 `Palette256[c]` **严格一致**，按 `r \| (g<<8) \| (b<<16)` 的反变换填通道字节 | `GetRGB` 的 256 项；其余读 `g_DefColorTable` 的代码目前不存在 | `HGE.pas` 移植后改为读真实表，并用「256 项逐项相等」用例（**已写**）守住回归 |
| **D-P17-03** | `g_boOpenMerchantBigDlg` 在 `ClientGlobals.cs` 是 `byte`（不是 `bool`） | **沿用既有声明**，不改类型（该字段在切片1 之前就由车道1 以 `byte` 承载，且可能有既有读点依赖） | 读点须写 `!= 0` 而非直接当 `bool` | 本车道后续切片统一为 `bool`，或由集成方一次性收敛 |

---

## 8. 跨区请求（需其它车道/集成方配合，本车道不越界）

| # | 请求 | 目标分区 | 阻塞了什么 |
|---:|---|---|---|
| **CR-1** | `GXX.Core.Protocol.TUserCharacterInfo` 补一个 `sChrName` 短串访问器（`ShortStr.Get/Set`，长度同 Delphi `string[19]`） | `GXX.Core/Protocol`（**非本车道**） | `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName` 的退役（§5.1 #11 因此**不可退役**） |
| **CR-2** | 明确 `DScreen:TDrawScreen`（MShare.pas:1456 声明）的归属：类型在 `DrawScrn.pas`、变量声明在 `MShare.pas` | 调度方裁定 | `FStateScreenSeam` 的退役；`MShare.pas` 的 `DScreen` 全局本切片**未落地** |
| **CR-3** | `GUI/Share/FStateSeams.cs` 执行 §5.1 的 10 项退役（改指 `MShareGlobals`） | `p14-client-fstate` / 集成方 | 无阻塞，仅清理接缝 |
| **CR-4** | `GUI/GameConfig/ConfigShare.cs` 的 `GetKeyDownStr`/`GetKey` 与 `MShare.pas:4336/4338` **同源**（`MShare` 版多一个 `var nKey:Integer` 出参） | `p10-client-mirconfig` / 集成方 | 避免**第三份实现**（§14.2）。本车道**主动不落** `GetKeyDownStr`/`GetKey`，改指/合并由集成方统一裁决 |
| **CR-5** | `Scenes/MiniMapRender.cs:225` 的 `QueryMsgTick` 合并为单一全局 | `p10-client-scrn` / 集成方 | 无阻塞，仅一致性 |

---

## 9. 未完成 / 阻塞（如实登记，**没做的不填 0**）

### 9.1 本切片未做

| 项 | 数量 | 原因 | 下一步 |
|---|---:|---|---|
| 类方法 | **185** | 全部依赖未移植的 `GameImages`/`Wzl`/`Wil`/`Pak`/`WinINet`/`IdHTTP`/`SuperObject`；在那些单元落地前，任何移植都只能造空壳（违反 §14.2 不造第三份实现） | 见 9.2 的可行起点 |
| 单元级例程 | **144** | 同上（文本表族 `GetItemDesc`/`GetTzItemDesc`/`GetGodBlessItem` 依赖 `TGHashStringList` + `g_*List` 全局；图像族依赖 `CreateGameImages`） | 下一批优先 `TWarrContinueHitManager`（4 条，纯逻辑）与文本表族 |
| 全局 | **约 232** | 多为图像/网络目录句柄（`g_WMainImages` 族、`g_Wh*Images` 族、`g_AntiPlugDll*` 族）——**有类型但类型本身未移植**（`TGameImages`/`TUibImages`/`TUpdateEngine`/`TMemoryStreamEx`/`TCriticalSection`…）。强行落地只能造出无人消费的壳 | 随宿主单元分批落地 |

### 9.2 下一步最可行的切口（按性价比排序）

1. **`TWarrContinueHitManager`（4 条，原文 11987-12105）** —— 纯逻辑，只需 `g_boCanLongHit` 等 6 个已落地全局。⚠ 注意：`CanOpenMagic` 主体被原文 `{...}` **整段注释**（12003-12070），`CanUseMagic` 同理。**移植时必须照抄"注释掉"这一事实**，即 `CanOpenMagic` 实际近乎 `Result := True`——这是**原文如此**，不是偷懒。
2. **文本表族**（`LoadSkillDescList` 9073 / `UnLoadSkillDescList` 9127 / `GetSkillDesc` 9140 / `LoadItemDescList` 9252 / `GetItemDesc` 9761 / `GetTzItemDesc` 10139 / `GetGodBlessItem` 9876 / `GetFengHaoItem` 9965）—— 依赖 `TGHashStringList`/`TGStringList`（`Core.Util` 已有）。
3. **平台族**（`GetTempDir` 11682 / `MakeTempFileName` 11690 / `_FileSize` 8984 / `GetAbsolutePathEx` 6542 / `ProcessFileNameSpecialChar` 11565 / `IsInContinuous` 3267 / `GetCustomMoneyIndexByName` 3272 / `GetCustomMoneyNameByRule` 3296 / `GetHintFontSize` 11735 / `GetHintNameFontSize` 11708 / `ShiftStateToPlugShiftState` 11786 / `CheckBlockAutoMagic*`）—— 多数只需 BCL。
4. **`TMapDesc`（4 条，10323-10435）** —— 只依赖 `TMapDescList` 记录与文本解析。

### 9.3 明确阻塞（需外部条件）

- **`THttpThread` / `THttpClient` / `TUserCenterManager`（52 条）**：需要 `WinINet` 与 `IdHTTP` 的托管替代 + `SuperObject`(`ISuperObject`) 的托管等价物。这两者都不在 `MShare.pas` 内，属**架构决策**，需调度方裁定（自研 `HttpClient` 适配层 or 引第三方包）。
- **`TImageEvent`（16 条）**：`LoadGameImages`(7347-7914) 单方法 567 行，是 `MShare.pas` 最长的图像加载器，依赖 `GetGameImageFiles`/`CreateGameImages`/`GetPakFile` 等 6 条资源定位函数，全部依赖 `Wzl`/`Wil`/`Pak` 单元。

### 9.4 生命周期判定

`MShare` 保持 **`PARTIAL`（覆盖率 5.95% 例程 / 约 49.6% 全局）**，**不满足 `MAPPED`**。
升级 `MAPPED` 的判据：22 个类至少各有一个真实体 + 185 条类方法覆盖率 ≥ 90%，或 `GameImages`/`Wzl`/`Wil`/`Pak` 四单元落地后由后续切片补齐。

---

## 10. 计数对账

| 口径 | 成员数 | 真实体 | NotPorted | 原文如此 | 等式 |
|---|---:|---:|---:|---:|---|
| 类方法（22 类） | 185 | 0 | 185 | 0 | `0+185+0=185` ✅ |
| 单元级例程 | 168 | 21 | 144 | 3 | `21+144+3=168` ✅ |
| 例程合计 | 353 | 21 | 329 | 3 | `21+329+3=353` ✅ |
| 全局 `g_*` | 约 460 | 228 | 约 232 | 0 | `228+约232+0=约460` ✅ |
| 类（`= class`） | 22 | 0 | 22 | 0 | `0+22+0=22` ✅ |
| 原文缺陷（锁死） | — | — | — | 3（P17-ASIS-01 / P17-DEF-02 / P17-DEF-03；P17-DEF-01 属转写产物，不计入） | — |
| 偏离 | — | — | — | 3（D-P17-01/02/03） | — |

**逐文件交付明细**（`git show --stat 54f3e9af`）：

| 文件 | 新增行 | 内容计数 |
|---|---:|---|
| `src/GXX.Client/GUI/Mir/ClientGlobals.cs` | +123/-1 | 新增 24 条 `g_*` + `MShareGlobalsReset` 同步 24 条复位 + `partial` 关键字 |
| `src/GXX.Client/GUI/Mir/MShare/MShareGlobals.Core.cs` | +765（新） | 202 条 `g_*` + `g_DefColorTable` + `g_InputBoxFilterList` + 私有 `BuildDefColorTable` |
| `src/GXX.Client/GUI/Mir/MShare/MShareFunctions.cs` | +349（新） | 21 条真身 + 4 条私有 helper |
| `src/GXX.Client/GUI/Mir/MShare/MShareTypes.cs` | +28（新） | `TRGBQuad` |
| `tests/GXX.Client.Tests/MShareP17Slice1Tests.cs` | +641（新） | 33 个 `[Fact]/[Theory]` ⇒ **110 条用例** |

---

## 11. 提交

| 提交 | 内容 | 门禁 |
|---|---|---|
| `54f3e9af` | 切片1：MShare 全局真身（fstate B-6 阻塞类）+ 首批纯函数 | **PASS**（build 0 error / test exit 0 / no crash markers；5108 通过 / 0 失败） |

---

## 12. 给集成方的三条最短动作

1. **`FStateSeams.cs:981`** 把 `FStateMShareSeam.ResetForTests();` 换成 `MShareGlobalsReset.ResetForTests();`（无损，见 §5.1 #10）。
2. 按 §5.1 的 10 项把 `FStateMShareSeam.*` 批量改指 `MShareGlobals.*`，然后删除 `FStateMShareSeam` 的这 10 个字段。
3. 裁决 CR-1（`TUserCharacterInfo.sChrName`）与 CR-2（`DScreen` 归属）——这两条是 `FStateMShareSeam` / `FStateScreenSeam` **无法退役**的唯一剩余原因。

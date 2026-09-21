# 并行报告 · 车道 `p14-client-fstate`（FState.pas 声明面 → 1:1 实现）

> 车道：`par/p14-client-fstate` ｜ 工作树：`.worktrees/p14-client-fstate`
> 源单元：`Source/Client-HGE/GUI/Share/FState.pas`（25,165 物理行；`_analysis/utf8_mirror` 为 UTF-8 镜像，本报告全部行号以镜像为准）
> 承接状态：**REFUTED（真缺口）** —— 复核车道 `p12-e2only-review` 裁决 `FState` = **C（未移植/仅借名）**，
> 理由为"名面命中 330 条中 **291 条（79%）来自 `TFrmDlg.Decl.g.cs` 的 `throw` 壳**"。

---

## 0. 一句话结论（本轮）

把"515 个 `throw` 壳"这件事从**传闻**变成**逐条实测的对账表**（533 行，见附录 A），
并按原文 1:1 落地了**八个切片共 79 条**成员，同时把 D-P10-06 的三段接缝按既定条件**核对并删除**。

| 指标 | 本轮实测值 | 取证方式 |
|---|---|---|
| `TFrmDlg.Decl.g.cs` 内 `throw new NotSupportedException` | **436**（本轮起始 **515**，净减 79） | `Select-String ... \| Measure-Object` |
| TFrmDlg 声明面成员（去重名字） | **533**（`TFrmDlgMethodTable` 538 条声明，含 5 组重载同名） | `FStateDeclManifest.g.cs` |
| `REAL`（已有 1:1 真实现） | **100**（其中本车道 **79**：切片 1..8 = 25/25/6/5/6/2/4/6） | 生成器 `$Handwritten` + `TFrmDlg.Handlers.cs` |
| `PENDING`（原文有体、托管仍是 throw 壳） | **161** | `gen-recon-table.py` |
| `ORIGINAL_EMPTY`（原文空体/仅注释） | **40** | 同上 |
| `ABSTRACT_NO_BODY`（原文无实现体） | **232** | 同上 |
| **当前真实覆盖率** | **100/533 = 18.76%** | 同上 |
| 可移植面完成率（分母 `REAL+PENDING+ORIGINAL_EMPTY` = 301） | **100/301 = 33.22%** | 同上 |
| 本车道落地的成员 | **79** | `TFrmDlgPortLedger.LaneCount` |
| 本车道新增用例 | **约 136**（实测 4904 → 5015） | `GXX.Client.Tests` |

> 与 p12 复核的差异说明：p12 的"约 31/367 真移植"是**以 `implementation` 段例程为分母**；
> 本表以**声明面成员**为分母（更严：把 232 条原文根本没有实现体的声明也算进分母）。
> 两个口径都保留，报告里各自标注来源，避免再次出现"同一数字两个含义"。

---

## 1. 分支与提交

| 提交 | 内容 |
|---|---|
| `3738fa27` | 并行批次P1：切片 1 —— 无依赖/空体/纯字段读写面 **25 条** + D-P10-06 接缝删除（真实体 25 / NotPorted 0 / 剩余 throw 490） |
| `747f5064` | 并行批次P2：对账表生成器 `gen-recon-table.py` + 本报告（533 成员逐条表） |
| `39252e1a` | 并行批次P3：切片 2 关闭/打开转发面 **25 条**（真实体 25 / NotPorted 0 / 剩余 throw 465） |
| `26b12e23` | 并行批次P4：切片 3 B-2 授权解锁 **6 条**（`DWebClick`/`DActionLogClick`/`DGetBackDeleteHumanClick`/`DCustomButtonClick` + 骑马两条）（剩余 throw 459） |
| `02759641` | 并行批次P5：切片 4 tick 守卫族 **2 条** + 商铺/排行/好友入口 **3 条**（剩余 throw 454） |
| `e4d55ede` | 并行批次P6：报告同步（切片 3/4、D-P14-11..15、B-5..B-7）+ 对账表刷新（真实体 82） |
| `92b6d7d1` | 并行批次P7：切片 5 交易/挑战守卫族 **6 条**（剩余 throw 448） |
| `831618e7` | 并行批次P8：切片 6 帮助按钮差判据节流 **1 条** + 更新状态框重连 **1 条**（剩余 throw 446） |
| `4b757d38` | 并行批次P10：切片 7 组队模式开关对 **2 条** + 交易物品回包 **1 条** + 元宝交易菜单清场 **1 条**（剩余 throw 442） |
| `6985a73a` | 并行批次P11：切片 8 提示清理族/关闭转发/小地图坐标/原文 Exit 短路 **6 条**（剩余 throw 436） |

工作树基线：`main @ 838ad9ae`。

---

## 2. 改动文件清单

**源码（全部在 `GXX.Client/src/GXX.Client/GUI/Share/**` + 测试分区内）**

| 文件 | 变更 |
|---|---|
| `GUI/Share/TFrmDlg.Handlers.cs` | **新增**：切片 1..8 共 **79** 条 1:1 真实现 + `TFrmDlgPortLedger` 机器可读台账 |
| `GUI/Share/FStateScreenSeam.cs` | **新增**：`DScreen.ClearHint` 的显式留痕接缝（真实对象未落地，调一次记一次） |
| `GUI/Share/FStateResStrSeam.cs` | **新增**：`DecodeResStr` + 3 条 `S*` resourcestring 注入接缝（切片 2 用；默认值=常量名，不静默成空串） |
| `GUI/Share/FStateDeclGen.ps1` | `$Handwritten` 由 20 条扩到 100 条（切片 1..8 = 25/25/6/5/6/2/4/6，另 21 条为前任车道）；新增 `$CsTypeOverrides`（`FSayItemHintWin`）；源码改显式 UTF-8 读取 |
| `GUI/Share/TFrmDlg.Decl.g.cs` | 生成器重跑：69 个成员不再声明（`throw` 515→446）；`FSayItemHintWin` 类型 `object` → `THintWindows` |
| `GUI/Share/FStateDeclManifest.g.cs` | 生成器重跑（字段表 `FSayItemHintWin` 类型同步；西文注释乱码修复为正确中文） |
| `GUI/Share/FStatePure.cs` | `GetHitLines` 的 `THintLines` 形参改指 `GXX.Client.Scenes.THintLines`（D-P10-06） |
| `GUI/Share/FStateSeams.cs` | **删除** `THintLines`（旧 407-452）、`THintWindows` + `DrawScrn` 静态接缝（旧 599-612） |
| `GUI/Share/TFrmDlg.Core.cs` | `FSayItemHintWin := new THintWindows()`（原 `DrawScrn.CreateHintWindows()`）+ 补 `using GXX.Client.Scenes;` |
| `GUI/Share/gen-recon-table.py` | **新增**：对账表生成器（Python 3，UTF-8 I/O 无歧义；见 §7 D-P14-04） |
| `tests/GXX.Client.Tests/GuiShareHandlersTests.cs` | **新增**：约 114 条用例（切片 1..4，含"壳行为回归闸门"、IL 级判壳、转发证据、tick 落点断言） |
| `tests/GXX.Client.Tests/GuiSharePureTests.cs` | 按原文改正壳行为断言（`THintLines` 接缝 → 正式实现的对象形态） |

**未改动**（硬性禁区）：`GXX.slnx`、任何 `*.csproj`、`Directory.Build.props`、`docs/Checklist.md`、`docs/并行派发台账.md`、`docs/并行覆盖审计.md`、`tools/**`。

---

## 3. 切片 1 的 25 条成员（真实体 25 / NotPorted 0 / 原文如此 1）

| # | 成员 | 原文行 | 说明 | 计数归类 |
|---:|---|---|---|---|
| 1 | `HideAllControls` | 1891-1895 | 快照 `Memo.Visible` → `GuildMemoVisible`，再隐藏 | 真实体 |
| 2 | `RestoreHideControls` | 1897-1900 | 只按快照恢复，不写 `GuildMemoVisible` | 真实体 |
| 3 | `DStateWinClick` | 2316-2319 | 原文空体 | 原文如此（空体） |
| 4 | `DBottomInRealArea` | 2883-2886 | **无条件** `IsRealArea := True` | 真实体 |
| 5 | `DBotPlusAbilDirectPaint` | 2888-2891 | 原文空体 | 原文如此（空体） |
| 6 | `MerchantDlgPaint` | 12457-12460 | 原文空体 | 原文如此（空体） |
| 7 | `DUserState1MouseDown` | 17795-17798 | 原文空体 | 原文如此（空体） |
| 8 | `DChgGamePwdCloseClick` | 18018-18021 | **原文体内只有一行被注释掉的 `// CloseDChgGamePwd;`** | 原文如此（缺陷照抄） |
| 9 | `DChgGamePwdDirectPaint` | 18023-18026 | 原文空体 | 原文如此（空体） |
| 10 | `DscSelect1InRealArea` | 18028-18031 | **无条件** `IsRealArea := True` | 真实体 |
| 11 | `DItemBagMouseMove` | 18038-18042 | `HintWindows.Clear` → `DScreen.ClearHint` → `g_boShowBagInfo := False` | 真实体 |
| 12 | `DMinMapDlgShow` | 18197-18200 | 原文空体 | 原文如此（空体） |
| 13 | `DMinMapDlgHide` | 18202-18205 | 原文空体 | 原文如此（空体） |
| 14 | `DMinMapDlgResize` | 18207-18210 | 原文空体 | 原文如此（空体） |
| 15 | `DGameGoldDealCancelClick` | 18655-18658 | 原文空体 | 原文如此（空体） |
| 16 | `AttactkModeChange` | 20367-20370 | 原文空体 | 原文如此（空体） |
| 17 | `OpenDUpgradeDlg` | 20741-20744 | 原文空体 | 原文如此（空体） |
| 18 | `CloseDUpgradeDlg` | 20746-20749 | 原文空体 | 原文如此（空体） |
| 19 | `OpenDRandomCodeDlg` | 21062-21065 | 原文空体 | 原文如此（空体） |
| 20 | `CloseDRandomCodeDlg` | 21067-21070 | 原文空体 | 原文如此（空体） |
| 21 | `DMouseMoveClearHints` | 24133-24137 | `DScreen.ClearHint` → `HintWindows.Clear`（**顺序**保留） | 真实体 |
| 22 | `DSayItemDlgCloseClick` | 24356-24359 | `DSayItemDlg.Visible := False` | 真实体 |
| 23 | `DSayItemDlgMouseDown` | 24361-24365 | **只在右键**关闭 | 真实体 |
| 24 | `DSayItemDlgMouseMove` | 24367-24372 | 置移出标志 → 清提示窗 → 清提示 | 真实体 |
| 25 | `DUpdateStatusDlgMouseLeave` | 24464-24467 | 只清 `HintWindows`（**不**调 `DScreen.ClearHint`） | 真实体 |

**切片计数对账**：真实体 **13** + 原文如此（空体/仅注释）**12** + NotPorted **0** = **25**。
（`REAL` 在全局对账表里是 71，因为其中还含前任车道的 21 条与本车道切片 2 的 25 条；
本表只统计本切片。）

**壳内剩余 `throw` 数**：`Select-String 'throw new NotSupportedException'` = **490**（起始 515）。

---

## 3b. 切片 2 的 25 条成员（真实体 25 / NotPorted 0 / 原文如此 0）

| # | 成员 | 原文行 | 说明 | 归类 |
|---:|---|---|---|---|
| 1 | `DSellDlgCloseClick` | 17362-17365 | 转 `CloseDSellDlg` | 真实体（转发） |
| 2 | `DMenuCloseClick` | 17446-17449 | 转 `CloseDMenuDlg` | 真实体（转发） |
| 3 | `DKsOkClick` | 17506-17509 | 转 `CloseDKeySelDlg` | 真实体（转发） |
| 4 | `DCloseUS1Click` | 17806-17809 | 转 `CloseDUserState1Dlg` | 真实体（转发） |
| 5 | `DGDUpClick` | 17854-17860 | `GuildTopLine -= 3`（>0 才减）+ 夹回 0 | 真实体（可完整断言） |
| 6 | `DGDDownClick` | 17862-17866 | `GuildTopLine + 12 < GuildStrs.Count` 才 `+= 3` | 真实体（可完整断言） |
| 7 | `DGDCloseClick` | 17868-17872 | 转 `CloseDGuildDlg` + `BoGuildChat := False` | 真实体（半转发） |
| 8 | `DGDEditNoticeClick` | 17906-17910 | `GuildEditHint := DecodeResStr(SGuildEditNotice)` → 转 `OpenDGuildEditNoticeDlg` | 真实体（半转发） |
| 9 | `DNewGuildDlgCloseClick` | 17912-17915 | 转 `CloseDGuildDlg_New` | 真实体（转发） |
| 10 | `DNewGuildNoticeClick` | 17917-17920 | 转 `OpenDGuildEditNoticeDlg_New` | 真实体（转发） |
| 11 | `DGDEditGradeClick` | 17922-17926 | `GuildEditHint := DecodeResStr(SGuildEditGradeHint)` → 转 `OpenGuildEditGradeDlg` | 真实体（半转发） |
| 12 | `DCloseStateClick` | 18822-18825 | 转 `CloseDStateWinDlg` | 真实体（转发） |
| 13 | `DCloseBagClick` | 18827-18830 | 转 `CloseDItemBagDlg` | 真实体（转发） |
| 14 | `DBotRankClick` | 18832-18835 | 转 `OpenDRankingDlg` | 真实体（转发） |
| 15 | `DBotWhisperClick` | 18837-18840 | 转 `OpenDWhisperDlg` | 真实体（转发） |
| 16 | `DMissionDlgClick` | 18868-18871 | 转 `OpenDMissionDlg` | 真实体（转发） |
| 17 | `DMissionDlgCloseClick` | 18873-18876 | 转 `CloseDMissionDlg` | 真实体（转发） |
| 18 | `DOpenShopClick` | 18878-18881 | 转 `OpenDShopDlg` | 真实体（转发） |
| 19 | `DBotRankingCloseClick` | 18888-18891 | 转 `CloseDRankingDlg` | 真实体（转发） |
| 20 | `DFrdCloseClick` | 18898-18901 | 转 `CloseDFriendDlg`（原文带空括号，同族唯一） | 真实体（转发） |
| 21 | `DGrpDlgCloseClick` | 18935-18938 | 转 `CloseDGroupDlg` | 真实体（转发） |
| 22 | `DMyHeroStateCloseClick` | 18996-18999 | 转 `CloseDHeroStateWinDlg` | 真实体（转发） |
| 23 | `DMyHeroBagCloseClick` | 19006-19009 | 转 `CloseDHeroItemBagDlg` | 真实体（转发） |
| 24 | `DLieDragonCloseClick` | 24206-24209 | `DLieDragon.Visible := False` | 真实体（可完整断言） |
| 25 | `DLieDragonNpcCloseClick` | 24311-24314 | `DLieDragonNpc.Visible := False` | 真实体（可完整断言） |

**切片 2 计数对账**：真实体 **25** + 原文如此 **0** + NotPorted **0** = **25**。
累计：切片 1 **25** + 切片 2 **25** = 本车道 **50**。

**★ 关于"转发形态"的诚实声明**：切片 2 里有 18 条是"一行转发"，其中 15 条的被转发方法
**本身仍是 `throw` 壳**。因此这些成员**调用时仍会抛 `NotSupportedException`**
（抛点在被转发方，消息里带的是被转发方法的原文行号）。
本报告与台账只声称**转发体已 1:1 落地**，**不**声称整条调用链已通。
测试 `ForwardersDelegateToTheOriginalTarget` 用"异常消息里必须出现 `TFrmDlg.<目标方法>:`"
把"真的转发了"这一事实锁死（而不是靠注释自证）。

**壳内剩余 `throw` 数**：**436**（切片 2 起点 490 → 3: 459 → 4: 454 → 5: 448 → 6: 446 → 7: 442 → 8: 436；起始 515）。

---

## 3g. 切片 7 的 4 条成员（真实体 4 / NotPorted 0 / 原文如此 0）

| # | 成员 | 原文行 | 说明 |
|---:|---|---|---|
| 1 | `DealItemReturnBag` | 17617-17624 | 只判 `not g_boDealEnd`；缓存 → `SendDelDealItem` → 重装交易 tick `+4000` |
| 2 | `DGameGoldDealMenuDlgCloseClick` | 18660-18665 | 关菜单 → 清 9 个远端物品槽（`Array.Clear`）→ `g_GameGoldDeal = default` |
| 3 | `DBotGroupMouseDown` | 18920-18928 | **先判右键**，再走与 #4 相同的主体 |
| 4 | `DGrpAllowGroupClick` | 18940-18947 | 主体：守卫 → **取反** `g_boAllowGroup` → 重装 `+5000` → 上报**取反后**的值 |

**原文重复体合并登记**：原文 18923-18927 与 18942-18946 是**两份逐字相同**的主体 ——
托管侧合并为一个私有 `ToggleGroupMode()`（两条公开成员各自转发），
测试用"右键点过之后按钮点不动"（共享同一 tick）锁死二者确实同体。

---

## 3h. 切片 8 的 6 条成员（真实体 6 / NotPorted 0 / 原文如此 1）

| # | 成员 | 原文行 | 说明 |
|---:|---|---|---|
| 1 | `DSSrvCloseClick` | 2294-2298 | 关选服对话框 → 关主窗体（顺序保留） |
| 2 | `DUserState1MouseMove` | 17800-17804 | 清物品名 → 清提示窗 → 清提示 |
| 3 | `DMinMapDlgMouseMove` | 18222-18226 | 只记**原始**事件坐标（不换算），负数照存 |
| 4 | `DGameGoldDealDlgMouseMove` | 18521-18525 | 只清两处提示 |
| 5 | `DGoToLieDragonClick` | 24287-24291 | 发 `'@HeroMap'` 商人命令 → 隐藏卧龙对话框 |
| 6 | `CloseSayItemDlg` | 24374-24383 | ★ **原文缺陷**：函数体首句是**无条件 `Exit`** |

**★ 新登记的原文缺陷（第 8 条）**：`CloseSayItemDlg`（原文 24374-24383）第一句就是
**无条件 `Exit;`** ⇒ 其后的 `if DSayItemDlg.Visible and boSayItemDlgMoveOutClose then ...`
（24378-24382）**永远不可达** —— 原文的"鼠标移出即关闭 SayItem 对话框"**功能失效**。
托管侧逐字保留（先 `return`，不可达代码以注释形态留存并标 `// 原文如此`），
用例 `CloseSayItemDlgShortCircuitsAtTheOriginalUnconditionalExit`
把两个前置条件都摆成成立、坐标摆在矩形外，断言对话框**仍然可见**。

---

## 3f. 切片 6 的 2 条成员（真实体 2 / NotPorted 0 / 原文如此 0）

| # | 成员 | 原文行 | 判据 | 说明 |
|---:|---|---|---|---|
| 1 | `DControlHelpClick` | 21054-21060 | `Now - dwControlHelpCickTick > 1000` | **差**判据（不是序判据）；命中后 `:= Now`（**不是 +1000**）；时间戳是**本单元字段**（原文 486）⇒ 无接缝依赖 |
| 2 | `DUpdateStatusDlgDblClick` | 24469-24472 | — | 转发 `FrmMain.ReConnectClientSocketGate`（双击更新状态框 = 重连网关） |

**与切片 4/5 的差异断言**：`DControlHelpClick` 用的是**无符号差**而非"当前 tick > 计数器"；
`> 1000` 严格（差 == 1000 不触发）；重装是**赋值当前 tick**（不是 `+N`）。
（`dwControlHelpCickTick` 在原文是 protected ⇒ 测试经反射读写。）

---

## 3e. 切片 5 的 6 条成员（真实体 6 / NotPorted 0 / 原文如此 0）

交易 / 挑战的**镜像两族**——守卫用**各自**的动作时间戳，`*ZeroGold` 两条多一个 `not *End` 前置判据。

| # | 成员 | 原文行 | 前置/守卫 | 重装 | 转发 |
|---:|---|---|---|---|---|
| 1 | `DDealCloseClick` | 17533-17539 | `Now > g_dwDealActionTick` | **不重装** | 关 `DDealDlg` + `SendCancelDeal` |
| 2 | `DealZeroGold` | 17746-17752 | `not g_boDealEnd and g_nDealGold > 0` | `g_dwDealActionTick := +4000` | `SendChangeDealGold(0)` |
| 3 | `BotChallengeClick` | 18904-18910 | `Now > g_dwQueryMsgTick` | `+3000` | `SendChallengeTry` |
| 4 | `DBotTradeClick` | 18912-18918 | `Now > g_dwQueryMsgTick`（**共享**） | `+3000` | `SendDealTry` |
| 5 | `ChallengeZeroGold` | 20789-20795 | `not g_boChallengeEnd and g_nChallengeGold > 0` | `g_dwChallengeActionTick := +4000` | `SendChangeChallengeGold(0)` |
| 6 | `DChallengeCloseClick` | 20813-20819 | `Now > g_dwChallengeActionTick` | **不重装** | 关 `DChallengeDlg` + `SendCancelChallenge` |

**差异断言（切片 5 用例）**：`DDealCloseClick`/`DChallengeCloseClick` **不重装**自己的时间戳
（原文守卫体内没有赋值 —— 重装只发生在 `*ZeroGold`）；
`DealZeroGold`/`ChallengeZeroGold` 的 `> 0` 与 `not *End` **两个都成立才**触发；
交易与挑战用**各自**的时间戳（互不串台）。

---

## 3c. 切片 3 的 6 条成员（真实体 6 / NotPorted 0 / 原文如此 0）

调度方下放 B-2（`frmMain` 接缝）增量补充权后解锁。**逐条计数（新补 6 个 frmMain/Actor 成员）**：
`Navigate` / `SendDActionLogClick` / `SendGetBackDeleteChr` / `SendClientMessage` /
`sHomePage`（字段）/ `TakeHorse`；**仍缺**约 12 条（`Close`/`ReConnectClientSocketGate`/`SendSay`/
`SendGuildHome`（切片 4 已补）/`SendGuildAddMem`/`SendGuildDelMem`/`SendAdjustBonus`/
`SendCancelGameGoldDealItem`/`SendGetShopItems` …），清单写在 `FStateSeams.cs` 的 `FStateClMainSeam` 注释里。

| # | 成员 | 原文行 | 说明 |
|---:|---|---|---|
| 1 | `DBotHorseClick` | 18842-18845 | 无条件 `g_MySelf.TakeHorse`（**未判 nil**） |
| 2 | `DDownHorseClick` | 20570-20575 | `m_btHorse in [1,2] and m_btDoubleHumHorse = 0`（**未判 nil**） |
| 3 | `DWebClick` | 20577-20580 | `frmMain.Navigate(g_ClientConfig.sHomePage)` |
| 4 | `DActionLogClick` | 20582-20585 | `frmMain.SendDActionLogClick`（原文无括号） |
| 5 | `DGetBackDeleteHumanClick` | 20592-20596 | 先判选中名**非空**才发找回请求 |
| 6 | `DCustomButtonClick` | 24917-24922 | 只在 `Sender is TDxImageButton` 时发 `CM_CUSTOM_BUTTON_CLICK` + `Tag` |

---

## 3d. 切片 4 的 5 条成员（真实体 5 / NotPorted 0 / 原文如此 0）

**tick 守卫族**（调度方建议优先吃的一族）——原文形态统一，且**可完整断言**：
时钟经 `FStateSeamClock.NowHandler` 注入，`>` 与 `>=`、以及 `+3000` 重装窗口都能精确落点。

| # | 成员 | 原文行 | 守卫判据 | 重装 | 转发 |
|---:|---|---|---|---|---|
| 1 | `DGDHomeClick` | 17874-17881 | `Now > g_dwQueryMsgTick` | `+3000` | `SendGuildHome` + `BoGuildChat := False` |
| 2 | `DGDListClick` | 17883-17890 | 同上（**共享同一计数器**） | `+3000` | `SendGuildMemberList` + `BoGuildChat := False` |
| 3 | `DBotRankingClick` | 18883-18886 | — | — | `OpenDRankingDlg` |
| 4 | `DBotFriendClick` | 18893-18896 | — | — | `OpenDFriendDlg()`（原文带空括号） |
| 5 | `DBotUserShopClick` | 20565-20568 | — | — | `OpenDGameShopDlg`（默认参 `IsCheckTime = True`） |

**tick 守卫族的断言**（切片 4 用例）：`>` 是**严格大于**（tick == 计数器时不触发）；
重装在**守卫体内**（不触发时计数器不变）；`BoGuildChat` 也只在守卫体内改；
两条共享计数器（先 Home 后 List，第二条被挡住）。

---

## 4. D-P10-06 的核对与删除（本轮结清）

**台账 §47.5 D-P10-06 原文要求**：DrawScrn 车道已把 `THintLines`/`THintWindows` 的正式归属
落在 `GXX.Client.Scenes`，并要求删掉 `FStateSeams.cs:407-452 / 599-612` 三段；
**附条件**：需核对 `FStatePure.cs:75 GetHitLines` 是否用了接缝 `Add` 的 **int 返回值**。

### 4.1 条件核对（实测）

`FStatePure.cs` 的 `GetHitLines` 体内共 3 处 `HintLines.Add(...)`：

| 原文行 | 调用形态 | 是否使用返回值 |
|---|---|---|
| 原文 3658 | `HintLines.Add(S, DefColor, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke(True))` | **否**（Delphi `Add` 是 `procedure`） |
| 原文 3674 | `HintLines.Add(Copy(S, 1, I), Color, ...)` | **否** |
| 原文 3682 | `HintLines.Add(S, Color, ...)` | **否** |

全树复核：接缝 `THintLines.Add(...)` 的返回值**没有任何调用点消费**
（`Grep 'HintLines.Add' GXX.CSharp/src` 仅 `FStatePure.cs` 这 3 处；`TStateWindowsText.cs`
另有一套自己的 `HintLine` 结构，不依赖接缝）。

**⇒ 条件成立：接缝的 `int` 返回值是死值，删除不改变任何行为。**
（顺带确认语义等价：正式实现 `THintLines.Add` 与原文同为 `void`，签名
`Add(string, TColor, int fontSize = 9, TFontStyles = fsNone, bool isStroke = false, string fontName = "")`
与原文 5 参调用点完全对得上。）

### 4.2 删除动作

| 删除项 | 位置（删除前） | 替换为 |
|---|---|---|
| `class THintLines` + `struct HintLine` | `FStateSeams.cs` 旧 407-452 | `using GXX.Client.Scenes;`（`FStatePure.cs`），形参改用 `GXX.Client.Scenes.THintLines` |
| `class THintWindows` | `FStateSeams.cs` 旧 599-602 | `GXX.Client.Scenes.THintWindows` |
| `static class DrawScrn` + `CreateHintWindows()` | `FStateSeams.cs` 旧 604-612 | `TFrmDlg.Create` 1555 行改 `new THintWindows()` |

删除后 `GXX.Client.GUI.Share` 命名空间下**不再存在** `THintLines`/`THintWindows`/`DrawScrn`
—— 由用例 `GuiShareHandlersTests.DeletedSeamTypesAreGoneFromTheGuiShareNamespace` 反射断言锁死。

### 4.3 测试断言的"壳行为 → 正确断言"改写（方法论第 5 条）

`GuiSharePureTests.cs` 的 10 条 `GetHitLines` 用例原先断言的是**接缝的数据形态**
（`h.Count` / `h.Lines[i].Text` / `h.Lines[i].Color`）。改用正式实现后按原文改成：

| 旧（接缝壳形态） | 新（正式实现 / 原文形态） |
|---|---|
| `h.Count` | `h.FList.Count` |
| `h.Lines[i].Text` | `((THintText)h.FList[i]).FCaption` |
| `h.Lines[i].Color.Value` | `((THintText)h.FList[i]).FColor.Value` |
| `h.Lines[i].Size / Style / Stroke` | `((THintText)h.FList[i]).Size / .Style / .IsStroke` |

**行为差异实测**：无。10 条用例全部保持原断言值通过 —— 证明接缝与正式实现在
纯文本输入下逐字等价（`ProcessHintText` 的 `<Tag:...>` 分支未被这些用例触发）。

---

## 5. 关键工程发现（**后续车道必须知道**）：生成壳的"声明 / 实现"契约

这是本车道**踩过并已固化**的一条硬事实，直接影响所有后续 FState 切片的工作方式。

### 5.1 现象（两条对照实验）

把 25 条真实现放进**另一个** partial 文件（`TFrmDlg.Handlers.cs` 的 `partial class TFrmDlg`）：

| 写法 | 实测结果 |
|---|---|
| `public override void XXX(...)` | `error CS0115: "TFrmDlg.XXX(...)": 没有找到适合的方法来重写` |
| `public virtual void XXX(...)`（同签名） | `error CS0111: 类型"TFrmDlg"已定义了一个名为"XXX"的具有相同参数类型的成员` |

**对照实验（定位用）**：把同一份 `override` 体写进 `TFrmDlg.Decl.g.cs` **自身**
→ **同样 CS0115**；把 `HideAllControls` 的整段壳**替换**成真体 → **编译通过**。

### 5.2 结论

**`TFrmDlg.Decl.g.cs` 声明的成员不能由其它 partial 文件实现**（既不能 `override`，
也不能同签名重声明）。实现面必须落在"生成壳那个 partial 组"里。

⇒ 正确落地方式 = **生成器 `$Handwritten` 跳过 + 手写 partial 供给真体**。
这不是绕过生成器，而正是生成器的设计意图（原本 20 条 `$Handwritten` 走的就是这条路，
见 `TFrmDlg.Core.cs`）。

### 5.3 本车道的做法（后续切片照抄）

1. 在 `FStateDeclGen.ps1` 的 `$Handwritten` 加入要落地的成员名；
2. 重跑生成器（`& FStateDeclGen.ps1 -Src <utf8镜像> -OutDir <Share目录>`）
   → `throw` 数下降、这些成员不再出现在生成壳；
3. 在 `TFrmDlg.Handlers.cs` 写 1:1 真体 + 在 `TFrmDlgPortLedger` 登记原文行号；
4. 用 `gen-recon-table.py` 重生成对账表。

**回归闸门**：`GuiShareHandlersTests.LedgerMembersAreNoLongerShellsInTheGeneratedFile`
对台账里的**每一条**成员做 **IL 级**检查 —— 若方法体里还有 `newobj NotSupportedException(...)`，
就说明它仍是生成壳，测试立刻红。

> 为什么用 IL 而不是"调用一下看抛不抛"：切片 2 的 18 条**转发**成员在目标仍未移植时**确实会抛**，
> 但抛点在被转发方。IL 检查只看"有没有**自己构造**那个异常"，因此能精确区分
> **"真实现（含转发）"** 与 **"还是 throw 壳"** —— 这正是本工程反复出现的"假完成"陷阱的机器判据。

---

## 6. 原文缺陷清单（照抄 + 断言锁死，**不顺手修**）

| # | 位置（原文行） | 缺陷 | 托管侧处理 | 锁死用例 |
|---|---|---|---|---|
| 1 | 3658 / 3674 / 3682 | `GetHitLines` 三处 `Add` 丢弃返回值（接缝曾伪造为 `int`） | 改指 `Scenes.THintLines`（`void`，与原文一致） | `GuiSharePureTests` 10 条 |
| 2 | 3667-3668 | `if I < nPos - 4 then I := nPos - 4` 把颜色码窗口夹到 3 位，导致 `"12345/abc"` 的前两位 `12` 变成正文 | 逐字保留 | `GetHitLinesClampsTheColorCodeWindowToThreeDigits` |
| 3 | 3675 | 颜色码非数字时 `StrToIntDef(..., 255)` 回退到调色板 255 | 逐字保留 | `GetHitLinesDefaultsToPalette255WhenTheCodeIsNotNumeric` |
| 4 | 18018-18021 | `DChgGamePwdCloseClick` 体内**只有一行注释** `// CloseDChgGamePwd;`（改密码对话框关不掉） | 照抄空体 + `// 原文如此` | `EmptyBodyMembersDoNothingAndDoNotThrow` |
| 5 | 2883-2886 / 18028-18031 | 两个 `*InRealArea` 回调**无条件**置 `True`，忽略 Sender/X/Y | 逐字保留 | `DBottomInRealAreaAlwaysSetsTrue` / `DscSelect1InRealAreaAlwaysSetsTrue` |
| 6 | 24120-24123 | `OpenGuildViewMemeberInfo` 拼写 `Memeber`（前任车道登记，本轮沿用） | 逐字保留 | 前任车道用例 |
| 7 | `FStateDeclGen.ps1` 生成的 481/484 行 `TDXLabel`/`TDXListView` | 原文拼写大小写错（`TDX` vs `TDx`） | 生成器 `Map-Type` 显式映射，注释标"original typo" | `GuiShareDeclTests` |
| 8 | 24374-24383 | `CloseSayItemDlg` 首句是**无条件 `Exit;`** ⇒ 其后的"鼠标移出即关闭"（24378-24382）**永远不可达**，功能失效 | 照抄 `return` + 不可达代码以注释留存 + `// 原文如此` | `CloseSayItemDlgShortCircuitsAtTheOriginalUnconditionalExit` |
| 9 | 20570-20575 / 18842-18845 | 两条骑马按钮**未判 `g_MySelf` 为 nil**（未进场景时点会 AV） | 逐字保留（**不加** nil 保护） | `DDownHorseClickKeepsTheOriginalMissingNilCheck` |
| 10 | 18663-18664 | `SafeFillChar` 整体清零 9 格 `TClientItem` + 整份 `TGameGoldDeal`（Delphi 的字节级 memset） | 等价改为 `Array.Clear` / `= default`（与前任车道 `FillChar` → `Array.Clear`/`default` 处理一致） | `GameGoldDealSeamHasTheOriginalNineRemoteSlots` |
| 11 | 17535-17538 / 20815-20818 | 两条 `*CloseClick` 的守卫体内**不重装**自己的时间戳（重装只在 `*ZeroGold`） | 逐字保留 | `DDealCloseClickForwardsAndDoesNotRearmTheDealTick` |

---

## 7. 偏离登记（D-P14-xx）

| 编号 | 偏离 | 依据 / 影响 |
|---|---|---|
| **D-P14-01** | 切片 1 的实现体放在 `TFrmDlg.Handlers.cs`（手写 partial），而**不是**直接编辑生成壳 | §5 实测：生成壳成员无法由其它 partial 实现。做法与既有 `TFrmDlg.Core.cs` 同构，且生成器 `$Handwritten` 是官方交界。 |
| **D-P14-02** | `TFrmDlg.Decl.g.cs` 的 `FSayItemHintWin` 字段类型由 `object` 改为 `THintWindows` | 原文 503 是 `FSayItemHintWin:TObject`，但 1555 行只`:= DrawScrn.THintWindows.Create`。D-P10-06 把正式归属定在 `GXX.Client.Scenes`，故字段类型落到该类型。已写入生成器 `$CsTypeOverrides`，**重跑生成器可复现**，不是手工漂移。 |
| **D-P14-03** | 25 条成员用 `public virtual`（new-slot）而非 C# `override` | 与 `TFrmDlg.Core.cs` 既有 20 条**完全同构**（§14.2 不造第二份实现）。虚分派仍成立：新槽可被后续子类 override，且对 `TFrmDlg` 实例的直接调用命中真体而非 throw 壳。 |
| **D-P14-04** | 对账表生成器用 **Python 3** 而非 PowerShell | 实测：`FStateExtract.ps1`/`FStateDeclGen.ps1` 依赖 `-Encoding Default`（PS 5.1 = ANSI，PS 7 = UTF-8），本车道在 PS 7 下把中文注释写成乱码，且**在 .ps1 里写中文标点会被解析器误判**（实测 `'—'` 触发了 syntax error）。Python 的 UTF-8 I/O 无歧义。生成器只读文件、只写 Markdown，不参与构建。 |
| **D-P14-05** | `FStateDeclGen.ps1` 的源码读取由 `Get-Content -Encoding Default` 改为显式 UTF-8 | 同上。副作用：生成文件里的中文注释由**乱码修复为正确中文**（`FStateDeclManifest.g.cs` 有 4 行西文/中文注释变化）。表格/指纹（`Count`/`NamesSha256`）**未变**：`CONSTS=9 TYPES=24 FIELDS=201 METHODS=538` 与重跑前完全一致。 |
| **D-P14-06** | `DScreen.ClearHint` 用 `FStateScreenSeam`（带计数留痕）承载 | `MShare.pas` 的 `DScreen:TDrawScreen` 托管侧尚未落地；按 §25.2**显式留痕、不静默返回**。每次调用记一条 `NotPortedCalls`，测试可断言"确实走到了这一步"。**不计入 FState 的未移植缺口**（源在 MShare/DrawScrn）。 |
| **D-P14-07** | 对账表把 538 条声明按**名字去重**为 533 行（5 组重载同名合并一行） | 表以"版本/成员"为单位才可读；重载的逐条签名仍完整保留在 `FStateDeclManifest.g.cs` 的 `Decls` 数组里（含原文行号）。 |
| **D-P14-08** | 切片 2 里 18 条"一行转发"被登记为**真实体**，即使被转发方仍是 `throw` 壳 | 原文这一族就是纯转发（`CloseDSellDlg;` 一行）。按 1:1 必须保留转发形态：内联被调者会**造第三份实现**（§14.2），加"目标未移植则跳过"会把缺口**静默**掉（§25.2）。台账只声称转发体已落地，并把"抛点在被转发方"写进代码注释与报告 §3b。 |
| **D-P14-09** | 3 条 `S*` resourcestring（`SGuildDelMem` / `SGuildEditNotice` / `SGuildEditGradeHint`）用 `FStateResStrSeam` 注入，**默认值 = 常量名本身** | 这 3 个常量在 ClFunc/MShare.pas（不在本单元）。默认值取常量名而非空串：忘记注入时表现为**可见占位**，不会静默变成空提示。真实文本待 resourcestring 落地后替换；**不计入 FState 缺口**。 |
| **D-P14-10** | 切片 2 的 25 条用 `public virtual`（与切片 1 同） | 同 D-P14-03。 |
| **D-P14-11** | B-2 的增量补充**没有**新建授权给我的 `Scenes/FStateClMainSeam.cs`，而是扩了既有的 `GUI/Share/FStateSeams.cs::FStateClMainSeam` | 该授权文件与既有类**同名**。同一程序集里再造同名类型会让所有同时 `using` 两命名空间的文件 CS0104 —— 正是本工程多次登记的"同一单元两条车道各造一套接缝"事故。⇒ 沿用既有类（授权未使用，也不产生二义性）。 |
| **D-P14-12** | `g_ClientConfig.sHomePage` 的最小承载放在 `FStateClMainSeam.sHomePage` | 车道1 的 `GXX.Client.GUI.Mir.TConfigClient`（`MirForms.cs:23`）**没有**该字段且不在本车道分区。默认值与 M2 端 `M2Config.sHomePage` 一致（`M2Config.ClientConf.cs:128`）。 |
| **D-P14-13** | `g_SelDeleteHumanInfo` 以 `g_SelDeleteHumanInfo_sChrName`（string）承载 | 原文 20594 只读它的 `sChrName`；Core 的 `TUserCharacterInfo` 是定长缓冲版、无可直读短串访问器。**字段名带后缀**以免被误当成整个记录；待 MShare 落地后替换。 |
| **D-P14-14** | `g_dwQueryMsgTick` 在 `FStateMShareSeam` 独立承载 | Scenes 车道的 `MiniMapMessageState.QueryMsgTick`（`MiniMapRender.cs:225`）是同一全局的**另一份**承载（跨分区）。已在注释里登记，待 MShare 落地后合并为单一全局。 |
| **D-P14-15** | tick 守卫族用**注入时钟**而非真实 `MyGetTickCount` | 原文判据全为严格 `>`，边界（tick == 计数器）与 `+3000` 重装窗口若用真实时钟无法确定性命中。接缝 `FStateSeamClock` 由前任车道建立，本切片沿用。 |

---

## 8. 测试与门禁

**门禁实跑（官方脚本 `GXX.CSharp/tools/run-gate.ps1`，本工程新规程的判据）**

```
powershell -NoProfile -ExecutionPolicy Bypass -File <main>\GXX.CSharp\tools\run-gate.ps1 `
    -Repo    D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p14-client-fstate `
    -Project GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj `
    -Log     <temp>\p14_gate.log

== gate evidence ==
dotnet build : 0 个错误 (182 个警告)
dotnet test exit code : 0
crash markers found   : none
GATE: PASS (build 0 error, test exit 0, no crash markers)
```

> 说明：该脚本被调度方修好后提交在 **main**（本车道工作树建立时尚无此文件），
> 故以 `-Repo <本车道工作树>` 指过来运行；`-Log` 指向临时目录以避免在仓库内留日志。
> 三个判据（build exit 0 / test exit 0 / 无崩溃标记）全部为脚本自身打印的实测值。

（基线：本轮开工前 `GXX.Client.Tests` 为 4904 例全绿；切片 1..8 累计新增 → **5015**。）

**新增用例分布（`GuiShareHandlersTests.cs`，约 136 例）**

| 组 | 例数 | 覆盖 |
|---|---:|---|
| A. 台账 / 生成壳一致性（含 IL 级"已不是壳"闸门） | 12 | 台账 25/25/6/5/6/2/4/6 条、行号区间合法、名字唯一且存在、**成员体不得再 `newobj NotSupportedException`** |
| C. 原文空体成员（`[Theory]` 11 行数据） | 11 | 调用即"什么都不发生"且行号登记一致 |
| D. RealArea 恒真（两个 `[Theory]` × 2） | 4 | `initial=false/true` 两向 |
| 切片 2：翻行 + 资源串 + 卧龙关闭 | 9 | `DGDUp/DGDDown` 边界、`DecodeResStr` 顺序、`Visible` 只动各自控件 |
| 切片 2：转发族（`[Theory]` 18 行数据） | 18 | **异常消息必须带被转发方法名** = 转发证据 |
| 切片 3：B-2 解锁四条 + 骑马两条 | 11 | `Navigate` 取 sHomePage、空选中名不发、`is TDxImageButton` 分支、骑马两条件 `[Theory]` 5 行 + 缺 nil 保护 |
| 切片 4：tick 守卫族 + 三个入口 | 7 | **严格 `>`** 边界、`+3000` 重装、`BoGuildChat` 在守卫体内、两条共享计数器 |
| 切片 5：交易/挑战守卫族 | 6 | **不重装**的两条、`not *End and > 0` 两条件、交易与挑战各自时间戳 |
| 切片 6：帮助按钮 + 重连 | 4 | **差判据** `> 1000` 边界、重装赋当前 tick、反射读写 protected 字段 |
| 切片 7：组队开关对 + 交易回包 + 清场 | 8 | 取反后上报、`+5000`、共享计数器、`not *End` 跳过、9 槽常量 |
| 切片 8：提示清理 + 关闭转发 + 原文缺陷 | 6 | 清理顺序、原始坐标不换算、**原文 `Exit` 短路锁死** |
| B/E/F/G/H 其余 | 11 | Hide/Restore 往返、SayItem 三态、HintWindows 清理顺序、D-P10-06 类型归属与接缝消失 |

---

## 9. 逐条对账表（533 个成员）

**见文末附录 A**（533 行，逐条给出 `原文行 / 原文实现体行号 / 体行数 / State`）。
该表由 `gen-recon-table.py` 生成后粘贴，**不是手抄**；重跑命令与期望输出见 §10。

`State` 的判定规则（与生成器实现一致）：

| State | 判定 | 条数 |
|---|---|---:|
| `REAL` | 该成员名在 `FStateDeclGen.ps1` 的 `$Handwritten` 里（⇒ 生成壳不再声明它，真体在手写 partial） | 100 |
| `PENDING` | 原文 `implementation` 段有该成员体、且体行数 > 3，托管侧仍是 `throw` 壳 | 161 |
| `ORIGINAL_EMPTY` | 原文有体但体行数 ≤ 3（空体/仅注释） | 41 |
| `ABSTRACT_NO_BODY` | 原文**声明了但整单元没有实现体** | 232 |
| 合计（去重名字） | | **533** |

> 538 条声明里有 5 组重载同名（`DrawBodyItemEffect` / `ClearMerchantSay` / `ShowMouseItemInfo` /
> `DelScreenMagicButton` / `GetMouseItemInfo`），逐条签名仍完整保留在
> `FStateDeclManifest.g.cs` 的 `Decls` 数组里（**那个数组就是原文 538 行声明的原文**）。

---

## 10. 复现方式（任何后继者都能一键重跑）

```powershell
$share = "D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p14-client-fstate\GXX.CSharp\src\GXX.Client\GUI\Share"

# 1) 例程清单（原文实现体行号）
& "$share\FStateExtract.ps1" `
    -src "D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\Client-HGE\GUI\Share\FState.pas" `
    -out $env:TEMP

# 2) 对账表
python "$share\gen-recon-table.py" `
    --src      "D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\Client-HGE\GUI\Share\FState.pas" `
    --routines "$env:TEMP\routines.csv" `
    --decl     "$share\TFrmDlg.Decl.g.cs" `
    --manifest "$share\FStateDeclManifest.g.cs" `
    --gen      "$share\FStateDeclGen.ps1" `
    --out      $env:TEMP\recon.md
# 期望输出：
# MANIFEST=538 DECLARED_THROW_NAMES=450 HANDWRITTEN=82 DISTINCT=533
# REAL=100 PENDING=161 ORIGINAL_EMPTY=40 ABSTRACT=232 THROW_NOW=436

# 3) 重新生成声明面（改了 $Handwritten 之后）
& "$share\FStateDeclGen.ps1" `
    -Src "D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\Client-HGE\GUI\Share\FState.pas" `
    -OutDir $share
```

---

## 11. 未完成 / 阻塞 / 后续切片建议

### 11.1 未完成（本车道明确的待办主体）

- **`PENDING` 161 条**：原文有实现体、托管侧仍是 `throw` 壳。这是本车道的**主战场**。
  建议按"依赖半径"从小到大推进：
  1. **无依赖 / 单跳转发** —— 切片 1/2/8 已把这一层基本扫完；
  2. **tick / 动作守卫族** —— 切片 4/5/6/7 已吃掉 **11 条，该族已清零**；
  3. **公会/组队转发族**（`DGDHome/DGDList` 已落）—— 余下 `DGDAddMemClick`/`DGDDelMemClick`/
     `DGDAllyClick`/`DGDBreakAllyClick` 还差 **`DMessageDlg`**（本单元自己的方法，声明在原文 863，
     托管侧是 throw 壳）+ `frmMain.SendGuildAddMem/SendGuildDelMem/SendSay`。
     ⇒ **下一刀（性价比最高）**：先落 `DMessageDlg` 一族（它本身就是一个可移植的成员），
     再补那 3 个 `Send*` 接缝，即可一次拿 4~5 条且**每条都能完整断言**（弹窗调用 + 文本 + 按钮集）。
  4. **提示窗族**（`DMessageDlg*` / `HintWindows` 交互）—— 与上一项合并做；
  5. **绘制族**（`*DirectPaint`，依赖 `GameCanvas`/纹理，接缝最厚，放最后）。
- **`ORIGINAL_EMPTY` 40 条**：原文空体，**零风险**，可一次性批量照抄（工作量 ≈ 0，
  但必须先按 §5.3 的四步走，否则编译不过）。

### 11.2 阻塞（如实登记）

| 阻塞 | 内容 | 影响面 |
|---|---|---|
| B-1 | `DScreen:TDrawScreen`（MShare/DrawScrn）未见托管实体 | 约十余个鼠标/绘制处理器的**第一步**只能走 `FStateScreenSeam` 留痕（D-P14-06）。**不计入 FState 未移植缺口**。 |
| B-2 | `frmMain`（ClMain.pas）在车道1 的 `GXX.Client.GUI.Mir.frmMain` 里**只有** `boNpcDlgCanMove` 一条 | 切片 3 已按调度方授权补进 6 个成员（见 §3c），由此**解锁 4 条**并把 `DWebClick`/`DActionLogClick`/`DGetBackDeleteHumanClick`/`DCustomButtonClick` 从"主动放弃"改为**已 1:1**。**仍缺约 12 条**（`Close`/`ReConnectClientSocketGate`/`SendSay`/`SendGuildAddMem`/`SendGuildDelMem`/`SendAdjustBonus`/`SendCancelGameGoldDealItem`/`SendGetShopItems`/`SendDealTry`/`SendChallengeTry`/`SendGroupMode`/`AppLogout` …）—— 清单写在 `FStateSeams.cs` 的 `FStateClMainSeam` 注释里，每条都挡着一个 `PENDING`。 |
| B-3 | `GXX.Client.GUI.Mir.TFrmDlg`（车道1 早期接缝）与本车道 `GXX.Client.GUI.Share.TFrmDlg` **同名不同类型** | 每个引用点都要 `using TFrmDlg = ...` 消歧（`GuiSharePureTests.cs`/`GuiShareHandlersTests.cs` 已如此）。**建议后续合并**，但跨分区，本车道不动。 |
| B-4 | `S*` resourcestring（`SGuildDelMem` 等）与 `DecodeResStr` 无正式归属 | 已用 `FStateResStrSeam`（默认值=常量名）承载（D-P14-09）。凡原文提示文本走 resourcestring 的处理器都受此影响。**不计入 FState 缺口**。 |
| B-5 | `g_ClientConfig`（`GXX.Client.GUI.Mir.TConfigClient`）**缺 `sHomePage`** 等 FState 用到的字段 | 已用 `FStateClMainSeam.sHomePage` 承载（D-P14-12）。同类字段后续还会挡人（该类型只有约 60 个字段，FState 引用面更宽）。 |
| B-6 | `MShare` 全局大面积缺失（切片 4/5 已补 `g_dwQueryMsgTick`/`g_dwDealActionTick`/`g_dwChallengeActionTick`/`g_boDealEnd`/`g_nDealGold`/`g_boChallengeEnd`/`g_nChallengeGold`；仍缺 `g_dwChangeGroupModeTick`/`g_boAllowGroup`/`g_dwLatestStruckTick`/`g_dwLatestMagicTick`/`g_dwLatestHitTick`/`g_nRankingsPage*`/`g_boMagicMoving`/`g_MovingMagic`/`g_GameGoldDeal*` 等） | 这些是本车道 `PENDING` 里**最密集的一类**阻塞。**建议把 `FStateMShareSeam` 的增量补充权明确化**（本轮已在用，未越界到车道1 的 `ClientGlobals.cs`）。 |
| B-8 | `g_dwLatest*Tick` 三兄弟（`DBotExitClick` 的"强行退出"判据） | 挡着 `DBotExitClick`（原文 18969-18982）。它还额外要 `frmMain.Close`（B-2 余项）与 `DScreen.AddChatBoardString`（B-1 同类）。 |
| B-7 | `TDxScrollBox`（`DxComponent/DxControls.cs:216`）**只有构造**，缺 `ControlCount`/`Control[]`/`Position` | 挡着 `ClearDMissionMemo`（原文 24125-24131）。同类还可能挡 `DNpcScrollBox*` 族。 |

### 11.3 给调度方的状态建议

- 本车道证据已足以把 `FState` 从 **REFUTED** 推进到 **PARTIAL（部分，**18.76%**）**：
  分母与算法在 §0 与 §9 全部给出，可独立复算。
- **建议继续加宽本车道分区**：`PENDING` 里相当一部分成员只差**一个接缝**
  （`g_dwQueryMsgTick` / `g_boMagicMoving` / `g_nMinMapX` 一类 `MShare` 全局）。
  目前这些全局散落在 `GUI/Mir/ClientGlobals.cs`（车道1 分区）与本车道的
  `FStateSeams.cs`。若能把 `MShareGlobals` 的**增量补充权**下放给本车道，
  可显著降低每条的接缝成本（否则每条都要在报告里登记一个新接缝）。

---

## 附录 A：533 成员逐条对账表

### TFrmDlg 声明成员逐条对账表

本表由 `src/GXX.Client/GUI/Share/gen-recon-table.py` 从**原文镜像 + 当前生成壳 + 生成器 `$Handwritten`** 实测生成（非手抄；随时可重跑复现）。

- **TFrmDlg 声明面成员**（`FStateDeclManifest.g.cs` 的 `TFrmDlgMethodTable`：538 条声明，其中 5 组重载同名 ⇒ 去重后 **533** 个名字）：**538**
  - 生成壳 `TFrmDlg.Decl.g.cs` **仍声明并 `throw`** 的名字：**432**（`throw` 语句实测 **436** 条）
  - `FStateDeclGen.ps1` 的 `$Handwritten` 跳过、由手写 partial 供给真体的名字：**100**
- 生成壳内 `throw new NotSupportedException` 实测条数：**436**
- `REAL`（托管侧已有 1:1 真实现）：**100**
- `PENDING`（原文有实现体、托管侧仍是 throw 壳 ⇒ **本车道待办主体**）：**161**
- `ORIGINAL_EMPTY`（原文自带空体/仅注释 ⇒ 可零风险照抄为 空体）：**40**
- `ABSTRACT_NO_BODY`（原文声明但本单元无实现体 ⇒ 保持 throw 壳）：**232**

**当前真实覆盖率（分母 = 全部声明成员 533）= 100/533 = 18.76%**
**可移植面完成率（分母 = REAL+PENDING+ORIGINAL_EMPTY = 301）= 100/301 = 33.22%**

`State` 取值：`REAL` = 真实现已落；`PENDING` = 待办（原文有体）；`ORIGINAL_EMPTY` = 原文空体；`ABSTRACT_NO_BODY` = 原文无实现体。

> `$Handwritten` 的成员**不出现在下表**：生成壳已不再声明它们，真体在 `TFrmDlg.Core.cs`（前任车道 20 条）与 `TFrmDlg.Handlers.cs`（本车道切片 1 的 25 条）。逐条清单见本报告“已落地成员”一节，生成器侧清单见 `FStateDeclGen.ps1` 的 `$Handwritten`。

| # | 原文行 | 成员 | 原文体 | 体行数 | State |
|---:|---:|---|---|---:|---|
| 1 | 493 | `OnMagicButtonClick` | `24609-24614` | 6 | REAL |
| 2 | 494 | `OnMagicButtonDblClick` | `24619-24622` | 4 | REAL |
| 3 | 495 | `OnMagicButtonMouseMove` | `24637-24678` | 42 | PENDING |
| 4 | 496 | `OnMagicButtonMove` | `24625-24627` | 3 | REAL |
| 5 | 499 | `CreateNpcQRButtonFromText` | `1610-1642` | 33 | PENDING |
| 6 | 533 | `Create` | `1418-1604` | 187 | REAL |
| 7 | 534 | `Destroy` | `1647-1712` | 66 | REAL |
| 8 | 535 | `UpDate` | `1783-1862` | 80 | PENDING |
| 9 | 536 | `DBottomInRealArea` | `2884-2886` | 3 | REAL |
| 10 | 537 | `DItemGridGridSelect` | n/a | n/a | ABSTRACT_NO_BODY |
| 11 | 538 | `DItemGridGridPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 12 | 539 | `DItemGridDblClick` | `11653-12396` | 744 | PENDING |
| 13 | 540 | `DBackgroundBackgroundClick` | `2158-2225` | 68 | PENDING |
| 14 | 541 | `DItemGridGridMouseMove` | n/a | n/a | ABSTRACT_NO_BODY |
| 15 | 542 | `DBelt1DirectPaint` | `2899-2913` | 15 | PENDING |
| 16 | 543 | `DBelt1DblClick` | `3038-3062` | 25 | PENDING |
| 17 | 544 | `DLoginCloseClick` | `2247-2249` | 3 | ORIGINAL_EMPTY |
| 18 | 545 | `DLoginOkClick` | `2242-2244` | 3 | ORIGINAL_EMPTY |
| 19 | 546 | `DLoginNewClick` | `2237-2239` | 3 | ORIGINAL_EMPTY |
| 20 | 547 | `DLoginChgPwClick` | `2252-2254` | 3 | ORIGINAL_EMPTY |
| 21 | 548 | `DNewAccountOkClick` | `2271-2273` | 3 | ORIGINAL_EMPTY |
| 22 | 549 | `DNewAccountCloseClick` | `2276-2278` | 3 | ORIGINAL_EMPTY |
| 23 | 550 | `DChgpwOkClick` | `2283-2285` | 3 | ORIGINAL_EMPTY |
| 24 | 551 | `DChgpwCancelClick` | `2290-2292` | 3 | ORIGINAL_EMPTY |
| 25 | 552 | `DSWWeaponClick` | `2327-2606` | 280 | PENDING |
| 26 | 553 | `DCloseBagClick` | `18828-18830` | 3 | REAL |
| 27 | 554 | `DBelt1Click` | `2976-3033` | 58 | PENDING |
| 28 | 555 | `DStateWinClick` | `2317-2319` | 3 | REAL |
| 29 | 556 | `DBelt1MouseMove` | `2919-2939` | 21 | PENDING |
| 30 | 557 | `DBelt1MouseDown` | `2944-2970` | 27 | PENDING |
| 31 | 558 | `DMerchantDlgCloseClick` | `16522-16632` | 111 | PENDING |
| 32 | 559 | `DMerchantDlgClick` | `17226-17360` | 135 | PENDING |
| 33 | 560 | `DMenuCloseClick` | `17447-17449` | 3 | REAL |
| 34 | 561 | `DMenuDlgDirectPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 35 | 562 | `DMenuDlgClick` | n/a | n/a | ABSTRACT_NO_BODY |
| 36 | 563 | `DMenuDlgMouseMove` | n/a | n/a | ABSTRACT_NO_BODY |
| 37 | 564 | `DSellDlgCloseClick` | `17363-17365` | 3 | REAL |
| 38 | 565 | `DSellDlgSpotClick` | `17370-17404` | 35 | PENDING |
| 39 | 566 | `DSellDlgSpotDirectPaint` | `17415-17444` | 30 | PENDING |
| 40 | 567 | `DSellDlgSpotMouseMove` | `17454-17466` | 13 | PENDING |
| 41 | 568 | `DSellDlgOkClick` | `17471-17504` | 34 | PENDING |
| 42 | 569 | `DMenuBuyClick` | `16757-16810` | 54 | PENDING |
| 43 | 570 | `DMenuPrevClick` | `16813-16826` | 14 | PENDING |
| 44 | 571 | `DMenuNextClick` | `16829-16841` | 13 | PENDING |
| 45 | 572 | `DGoldClick` | `12399-12421` | 23 | PENDING |
| 46 | 573 | `DSWLightDirectPaint` | `2305-2314` | 10 | PENDING |
| 47 | 574 | `DBackgroundMouseDown` | `2228-2232` | 5 | PENDING |
| 48 | 575 | `DStateWinMouseMove` | `2609-2615` | 7 | PENDING |
| 49 | 576 | `DLoginNewClickSound` | `2257-2266` | 10 | PENDING |
| 50 | 577 | `DStMag1Click` | `2753-2844` | 92 | PENDING |
| 51 | 578 | `DStMag1MouseDown` | `2850-2872` | 23 | PENDING |
| 52 | 579 | `DStMag1MouseUp` | `2875-2879` | 5 | PENDING |
| 53 | 582 | `DKsOkClick` | `17507-17509` | 3 | REAL |
| 54 | 583 | `DDealOkClick` | `17516-17531` | 16 | PENDING |
| 55 | 584 | `DDealCloseClick` | `17534-17539` | 6 | REAL |
| 56 | 585 | `DBotTradeClick` | `18913-18918` | 6 | REAL |
| 57 | 586 | `BotChallengeClick` | `18905-18910` | 6 | REAL |
| 58 | 587 | `DDealRemoteDlgDirectPaint` | `17547-17565` | 19 | PENDING |
| 59 | 588 | `DDealDlgDirectPaint` | `17570-17615` | 46 | PENDING |
| 60 | 589 | `DDGridGridSelect` | `17630-17667` | 38 | PENDING |
| 61 | 590 | `DDGridGridPaint` | `17673-17690` | 18 | PENDING |
| 62 | 591 | `DDGridGridMouseMove` | `17697-17711` | 15 | PENDING |
| 63 | 592 | `DDRGridGridPaint` | `17717-17723` | 7 | PENDING |
| 64 | 593 | `DDRGridGridMouseMove` | `17730-17744` | 15 | PENDING |
| 65 | 594 | `DDGoldClick` | `17758-17791` | 34 | PENDING |
| 66 | 595 | `DUserState1MouseMove` | `17801-17804` | 4 | REAL |
| 67 | 596 | `DCloseUS1Click` | `17807-17809` | 3 | REAL |
| 68 | 597 | `DNecklaceUS1DirectPaint` | `17814-17822` | 9 | PENDING |
| 69 | 598 | `DGuildDlgDirectPaint` | `17829-17852` | 24 | PENDING |
| 70 | 599 | `DGDUpClick` | `17855-17860` | 6 | REAL |
| 71 | 600 | `DGDDownClick` | `17863-17866` | 4 | REAL |
| 72 | 601 | `DGDCloseClick` | `17869-17872` | 4 | REAL |
| 73 | 602 | `DGDHomeClick` | `17875-17881` | 7 | REAL |
| 74 | 603 | `DGDListClick` | `17884-17890` | 7 | REAL |
| 75 | 604 | `DGDAddMemClick` | `17893-17897` | 5 | PENDING |
| 76 | 605 | `DGDDelMemClick` | `17900-17904` | 5 | PENDING |
| 77 | 606 | `DGDEditNoticeClick` | `17907-17910` | 4 | REAL |
| 78 | 607 | `DGDEditGradeClick` | `17923-17926` | 4 | REAL |
| 79 | 608 | `DNewGuildDlgCloseClick` | `17913-17915` | 3 | REAL |
| 80 | 609 | `DNewGuildNoticeClick` | `17918-17920` | 3 | REAL |
| 81 | 610 | `DGuildEditNoticeDirectPaint` | `17944-17951` | 8 | PENDING |
| 82 | 611 | `DGDChatClick` | `17967-17975` | 9 | PENDING |
| 83 | 612 | `DAdjustAbilCloseClick` | `17980-17983` | 4 | PENDING |
| 84 | 613 | `DBotPlusAbilClick` | `18985-18987` | 3 | ORIGINAL_EMPTY |
| 85 | 614 | `DAdjustAbilOkClick` | `17986-17989` | 4 | PENDING |
| 86 | 615 | `DBotPlusAbilDirectPaint` | `2889-2891` | 3 | REAL |
| 87 | 616 | `DAdjustAbilityMouseMove` | `17997-18016` | 20 | PENDING |
| 88 | 617 | `DUserState1MouseDown` | `17796-17798` | 3 | REAL |
| 89 | 618 | `DChgGamePwdDirectPaint` | `18024-18026` | 3 | REAL |
| 90 | 619 | `DscSelect1InRealArea` | `18029-18031` | 3 | REAL |
| 91 | 620 | `DCreateChrDirectPaint` | `18034-18036` | 3 | ORIGINAL_EMPTY |
| 92 | 621 | `DItemBagMouseMove` | `18039-18042` | 4 | REAL |
| 93 | 622 | `DBottomMouseMove` | `18045-18047` | 3 | ORIGINAL_EMPTY |
| 94 | 623 | `DNewAccountCancelClick` | `18055-18057` | 3 | ORIGINAL_EMPTY |
| 95 | 624 | `DHeroM2ShopDlgShowOnClick` | `23119-23124` | 6 | PENDING |
| 96 | 625 | `DShopDlgDirectPaint` | `18064-18089` | 26 | PENDING |
| 97 | 626 | `DButtonShopPrevClick` | `18092-18095` | 4 | PENDING |
| 98 | 627 | `DButtonShopNextClick` | `18098-18102` | 5 | PENDING |
| 99 | 628 | `DBotRankingHomeClick` | `18167-18170` | 4 | PENDING |
| 100 | 629 | `DBotRankingUpClick` | `18173-18176` | 4 | PENDING |
| 101 | 630 | `DBotRankingDownClick` | `18179-18183` | 5 | PENDING |
| 102 | 631 | `DBotRankingLastClick` | `18186-18195` | 10 | PENDING |
| 103 | 632 | `DButtonShopBuyClick` | `18108-18132` | 25 | PENDING |
| 104 | 633 | `DButtonShopBuyGiveClick` | `18138-18162` | 25 | PENDING |
| 105 | 634 | `MinMapLevelChange` | n/a | n/a | ABSTRACT_NO_BODY |
| 106 | 635 | `DMinMapDlgShow` | `18198-18200` | 3 | REAL |
| 107 | 636 | `DMinMapDlgHide` | `18203-18205` | 3 | REAL |
| 108 | 637 | `DMinMapDlgResize` | `18208-18210` | 3 | REAL |
| 109 | 638 | `DMinMapDlgMouseEnter` | `18213-18215` | 3 | ORIGINAL_EMPTY |
| 110 | 639 | `DMinMapDlgMouseLeave` | `18218-18220` | 3 | ORIGINAL_EMPTY |
| 111 | 640 | `DMinMapDlgMouseMove` | `18223-18226` | 4 | REAL |
| 112 | 641 | `DMinMapDlgMouseUP` | n/a | n/a | ABSTRACT_NO_BODY |
| 113 | 642 | `DMinMapDlgClick` | n/a | n/a | ABSTRACT_NO_BODY |
| 114 | 643 | `DscStartDirectPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 115 | 644 | `DDiceDlgDirectPaint` | `20341-20354` | 14 | PENDING |
| 116 | 645 | `OnGetImage` | `1715-1775` | 61 | PENDING |
| 117 | 646 | `HideAllControls` | `1892-1895` | 4 | REAL |
| 118 | 647 | `RestoreHideControls` | `1898-1900` | 3 | REAL |
| 119 | 648 | `DealItemReturnBag` | `17618-17624` | 7 | REAL |
| 120 | 649 | `DealZeroGold` | `17747-17752` | 6 | REAL |
| 121 | 650 | `OpenSoundOption` | `1908-1948` | 41 | PENDING |
| 122 | 653 | `FindActiveControl` | `13153-13165` | 13 | PENDING |
| 123 | 654 | `AddNpcMemo` | `15251-15506` | 256 | PENDING |
| 124 | 655 | `LoadJsonControl` | `25131-25134` | 4 | PENDING |
| 125 | 656 | `DBotRankClick` | `18833-18835` | 3 | REAL |
| 126 | 657 | `DBotWhisperClick` | `18838-18840` | 3 | REAL |
| 127 | 658 | `DBotHorseClick` | `18843-18845` | 3 | REAL |
| 128 | 659 | `DOptionClick` | `18848-18851` | 4 | PENDING |
| 129 | 660 | `DWhisperDlgCloseClick` | `18854-18856` | 3 | ORIGINAL_EMPTY |
| 130 | 661 | `DRankingDlgCloseClick` | `18859-18861` | 3 | ORIGINAL_EMPTY |
| 131 | 662 | `DShopDlgCloseClick` | `18864-18866` | 3 | ORIGINAL_EMPTY |
| 132 | 663 | `DMissionDlgClick` | `18869-18871` | 3 | REAL |
| 133 | 664 | `DMissionDlgCloseClick` | `18874-18876` | 3 | REAL |
| 134 | 665 | `DGrpDlgCloseClick` | `18936-18938` | 3 | REAL |
| 135 | 666 | `DBotGroupClick` | `18931-18933` | 3 | ORIGINAL_EMPTY |
| 136 | 667 | `DGrpAllowGroupClick` | `18941-18947` | 7 | REAL |
| 137 | 668 | `DBotGroupMouseDown` | `18921-18928` | 8 | REAL |
| 138 | 671 | `DBotExitClick` | `18970-18982` | 13 | PENDING |
| 139 | 672 | `DGDAllyClick` | `17929-17932` | 4 | PENDING |
| 140 | 673 | `DGDBreakAllyClick` | `17935-17939` | 5 | PENDING |
| 141 | 674 | `DButtonFriendClick` | `18050-18052` | 3 | ORIGINAL_EMPTY |
| 142 | 675 | `DBotRankingClick` | `18884-18886` | 3 | REAL |
| 143 | 676 | `DBotRankingCloseClick` | `18889-18891` | 3 | REAL |
| 144 | 677 | `DBotFriendClick` | `18894-18896` | 3 | REAL |
| 145 | 678 | `DFrdCloseClick` | `18899-18901` | 3 | REAL |
| 146 | 679 | `DChgGamePwdCloseClick` | `18019-18021` | 3 | REAL |
| 147 | 680 | `DGameGoldDealDlgMouseMove` | `18522-18525` | 4 | REAL |
| 148 | 681 | `DGameGoldDealMenuDlgPaint` | `18423-18519` | 97 | PENDING |
| 149 | 682 | `DGameGoldDealMenuDlgMouseMove` | `18376-18410` | 35 | PENDING |
| 150 | 683 | `DGameGoldDealGridGridPaint` | `18558-18580` | 23 | PENDING |
| 151 | 684 | `DGameGoldDealGridGridSelect` | `18588-18637` | 50 | PENDING |
| 152 | 685 | `DGameGoldDealGridGridMouseMove` | `18532-18552` | 21 | PENDING |
| 153 | 686 | `DGameGoldDealDlgCloseClick` | `18642-18653` | 12 | PENDING |
| 154 | 687 | `DGameGoldDealCancelClick` | `18656-18658` | 3 | REAL |
| 155 | 688 | `DGameGoldDealMenuDlgCloseClick` | `18661-18665` | 5 | REAL |
| 156 | 689 | `DBuyGameGoldDealItemOKClick` | `18672-18698` | 27 | PENDING |
| 157 | 690 | `DBuyGameGoldDealItemCancelClick` | `18701-18703` | 3 | ORIGINAL_EMPTY |
| 158 | 691 | `DGameGoldDealItemCancelClick` | `18706-18708` | 3 | ORIGINAL_EMPTY |
| 159 | 692 | `DBotFuncClick` | `18715-18779` | 65 | PENDING |
| 160 | 693 | `DBotFuncMouseMove` | `18787-18820` | 34 | PENDING |
| 161 | 694 | `DStMagMouseMove` | `2629-2738` | 110 | PENDING |
| 162 | 695 | `DMissionMerchantDlgClick` | `15596-15616` | 21 | PENDING |
| 163 | 696 | `DNPC_IMG_PAINT` | `23478-23571` | 94 | PENDING |
| 164 | 697 | `DNPC_IMGEX_PAINT` | `23580-23616` | 37 | PENDING |
| 165 | 698 | `DNPC_IMGNUM_PAINT` | `23625-23655` | 31 | PENDING |
| 166 | 699 | `DNPC_PLAYIMG_PAINT` | `23782-23842` | 61 | PENDING |
| 167 | 700 | `DNPC_IMGPAY_PAINT` | `23664-23697` | 34 | PENDING |
| 168 | 701 | `Close` | `20396-20413` | 18 | PENDING |
| 169 | 702 | `MakeShareControlAddrList` | `20416-20429` | 14 | PENDING |
| 170 | 703 | `LoadShareFromStream` | `20434-20563` | 130 | PENDING |
| 171 | 704 | `OpenDSelectChrDlg` | `20357-20365` | 9 | PENDING |
| 172 | 705 | `Initialize` | n/a | n/a | ABSTRACT_NO_BODY |
| 173 | 706 | `LoadFromStream` | n/a | n/a | ABSTRACT_NO_BODY |
| 174 | 707 | `MakeControlAddrList` | n/a | n/a | ABSTRACT_NO_BODY |
| 175 | 708 | `GetMissionTreeView` | n/a | n/a | ABSTRACT_NO_BODY |
| 176 | 709 | `GetMissionActivePageIndex` | n/a | n/a | ABSTRACT_NO_BODY |
| 177 | 710 | `SetMissionActivePageIndex` | n/a | n/a | ABSTRACT_NO_BODY |
| 178 | 711 | `MissionPageClear` | n/a | n/a | ABSTRACT_NO_BODY |
| 179 | 712 | `LoginPasswdOK` | n/a | n/a | ABSTRACT_NO_BODY |
| 180 | 713 | `OpenDLoginDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 181 | 714 | `CloseDLoginDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 182 | 715 | `OpenDDoorDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 183 | 716 | `CloseDDoorDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 184 | 718 | `OpenDRealNameDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 185 | 719 | `CloseDRealNameDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 186 | 721 | `OpenDGetPwdBackDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 187 | 722 | `CloseDGetPwdBackDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 188 | 724 | `OpenDRegAccount` | n/a | n/a | ABSTRACT_NO_BODY |
| 189 | 725 | `CloseDRegAccount` | n/a | n/a | ABSTRACT_NO_BODY |
| 190 | 727 | `OpenDBindPhone` | n/a | n/a | ABSTRACT_NO_BODY |
| 191 | 728 | `CloseDBindPhone` | n/a | n/a | ABSTRACT_NO_BODY |
| 192 | 729 | `CheckUserEntrys` | n/a | n/a | ABSTRACT_NO_BODY |
| 193 | 730 | `NewIdRetry` | n/a | n/a | ABSTRACT_NO_BODY |
| 194 | 731 | `NewAccountOk` | n/a | n/a | ABSTRACT_NO_BODY |
| 195 | 732 | `OpenDNewAccountDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 196 | 733 | `CloseDNewAccountDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 197 | 734 | `ChangePassWordOK` | n/a | n/a | ABSTRACT_NO_BODY |
| 198 | 735 | `OpenDChgPwDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 199 | 736 | `CloseDChgPwDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 200 | 737 | `OpenDSelServerDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 201 | 738 | `CloseDSelServerDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 202 | 739 | `CloseDSelectChrDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 203 | 740 | `OpenDCreateChrDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 204 | 741 | `CloseDCreateChrDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 205 | 742 | `MakeNewChar` | n/a | n/a | ABSTRACT_NO_BODY |
| 206 | 743 | `OpenDMerchantDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 207 | 744 | `CloseDMerchantDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 208 | 745 | `RestoreDMerchantDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 209 | 746 | `OpenDMenuDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 210 | 747 | `CloseDMenuDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 211 | 748 | `OpenTradingMarketItemsDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 212 | 749 | `RecallTradingSellPrices` | n/a | n/a | ABSTRACT_NO_BODY |
| 213 | 750 | `CheckDStorageViewDlgShow` | n/a | n/a | ABSTRACT_NO_BODY |
| 214 | 751 | `OpenDStorageViewDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 215 | 752 | `RefrshDStorageViewDlgText` | `23845-23850` | 6 | REAL |
| 216 | 753 | `RefrshDStorageOpen` | n/a | n/a | ABSTRACT_NO_BODY |
| 217 | 754 | `CheckDTradingMarketDlgVisible` | n/a | n/a | ABSTRACT_NO_BODY |
| 218 | 755 | `OpenDItemBagDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 219 | 756 | `CloseDItemBagDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 220 | 757 | `OpenDAuctionDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 221 | 758 | `CloseDAuctionDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 222 | 759 | `CloseDAuctionBuyDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 223 | 760 | `CloseDAuctionSellDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 224 | 761 | `OpenDAuctionBroadcastDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 225 | 762 | `CloseDAuctionBroadcastDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 226 | 763 | `OpenSellPlayDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 227 | 764 | `OpenDelSellPlayDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 228 | 765 | `OpenSellPlayerShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 229 | 766 | `SetSellPlayerShopItemPageInfo` | n/a | n/a | ABSTRACT_NO_BODY |
| 230 | 767 | `OpenViewSellPlayerInfo` | n/a | n/a | ABSTRACT_NO_BODY |
| 231 | 768 | `OpenViewSellPlayerBagItems` | n/a | n/a | ABSTRACT_NO_BODY |
| 232 | 769 | `OpenViewSellPlayerStorage` | n/a | n/a | ABSTRACT_NO_BODY |
| 233 | 770 | `SellPlayerAbilChange` | n/a | n/a | ABSTRACT_NO_BODY |
| 234 | 771 | `ShowSellPlayerOtherInfo` | n/a | n/a | ABSTRACT_NO_BODY |
| 235 | 772 | `ClearSellPlayerOtherInfo` | n/a | n/a | ABSTRACT_NO_BODY |
| 236 | 773 | `OpenDSellDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 237 | 774 | `CloseDSellDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 238 | 775 | `OpenDDealDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 239 | 776 | `CloseDDealDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 240 | 777 | `OpenDDealRemoteDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 241 | 778 | `CloseDDealRemoteDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 242 | 779 | `OpenDStateWinDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 243 | 780 | `CloseDStateWinDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 244 | 781 | `OpenDShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 245 | 782 | `CloseDShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 246 | 783 | `OpenDUserState1Dlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 247 | 784 | `CloseDUserState1Dlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 248 | 785 | `OpenDFriendDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 249 | 786 | `CloseDFriendDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 250 | 787 | `OpenDGroupDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 251 | 788 | `CloseDGroupDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 252 | 789 | `OpenDWhisperDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 253 | 790 | `CloseDWhisperDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 254 | 791 | `OpenDRankingDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 255 | 792 | `CloseDRankingDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 256 | 793 | `OpenDGameGoldDealDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 257 | 794 | `CloseDGameGoldDealDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 258 | 795 | `OpenDGameGoldDealMenuDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 259 | 796 | `CloseDGameGoldDealMenuDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 260 | 797 | `OpenDGuildDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 261 | 798 | `CloseDGuildDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 262 | 799 | `DGuildChangeRankNo` | n/a | n/a | ABSTRACT_NO_BODY |
| 263 | 800 | `UpdateGuildJoinCondition` | `23853-23857` | 5 | REAL |
| 264 | 801 | `RefreshDGuildManageListViewWJ` | n/a | n/a | ABSTRACT_NO_BODY |
| 265 | 802 | `OpenGuildViewMemeberInfo` | `24121-24123` | 3 | REAL |
| 266 | 803 | `OpenGuildEditGradeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 267 | 804 | `OpenDGuildEditNoticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 268 | 805 | `CloseDGuildEditNoticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 269 | 806 | `RefreshGuildMembers` | n/a | n/a | ABSTRACT_NO_BODY |
| 270 | 807 | `CloseDGuildDlg_New` | n/a | n/a | ABSTRACT_NO_BODY |
| 271 | 808 | `OpenDGuildEditNoticeDlg_New` | n/a | n/a | ABSTRACT_NO_BODY |
| 272 | 809 | `RefreshGuildJoinUserList` | n/a | n/a | ABSTRACT_NO_BODY |
| 273 | 810 | `DoSetGuildWantAddGuildName` | n/a | n/a | ABSTRACT_NO_BODY |
| 274 | 811 | `OpenDGuildApplyDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 275 | 812 | `CloseDGuildApplyDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 276 | 813 | `OpenDGuildNoticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 277 | 814 | `CloseDGuildNoticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 278 | 815 | `OpenDMissionDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 279 | 816 | `CloseDMissionDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 280 | 817 | `OpenDAdjustAbilityDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 281 | 818 | `CloseDAdjustAbilityDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 282 | 819 | `IsDAdjustAbilityDlgShow` | n/a | n/a | ABSTRACT_NO_BODY |
| 283 | 820 | `OpenDHeroStateDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 284 | 821 | `CloseDHeroStateDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 285 | 822 | `OpenDHeroItemBagDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 286 | 823 | `CloseDHeroItemBagDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 287 | 824 | `OpenDHeroStateWinDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 288 | 825 | `CloseDHeroStateWinDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 289 | 826 | `SetHeroBagCount` | n/a | n/a | ABSTRACT_NO_BODY |
| 290 | 827 | `CloseDHeroJewelryBoxDlg` | `23881-23883` | 3 | REAL |
| 291 | 828 | `CloseDHeroGodBlessDlg` | `23876-23878` | 3 | REAL |
| 292 | 829 | `OpenDDeleteHumanDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 293 | 830 | `CloseDDeleteHumanDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 294 | 831 | `OpenMyStatus` | n/a | n/a | ABSTRACT_NO_BODY |
| 295 | 832 | `OpenMyMagic` | n/a | n/a | ABSTRACT_NO_BODY |
| 296 | 833 | `AddDDrinkWineDlgSay` | n/a | n/a | ABSTRACT_NO_BODY |
| 297 | 834 | `OpenDPleaseDrinkWineDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 298 | 835 | `CloseDPleaseDrinkWineDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 299 | 836 | `OpenDGuessfingerDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 300 | 837 | `CloseDGuessfingerDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 301 | 838 | `OpenDMakeWineDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 302 | 839 | `CloseDMakeWineDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 303 | 840 | `OpenDMakeMedicinalWineDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 304 | 841 | `CloseDMakeMedicinalWineDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 305 | 842 | `OpenDStorageHeroInfoDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 306 | 843 | `CloseDStorageHeroInfoDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 307 | 846 | `OpenDHeroAppraisalInfoDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 308 | 847 | `CloseDHeroAppraisalInfoDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 309 | 848 | `OpenDHeroAppraisalDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 310 | 849 | `CloseDHeroAppraisalDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 311 | 851 | `OpenDHeroAutoPracticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 312 | 852 | `CloseDHeroAutoPracticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 313 | 853 | `ShowCollectProgressBarDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 314 | 854 | `CloseCollectProgressBarDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 315 | 855 | `BringToFrontCollectProgressBarDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 316 | 856 | `ShowProgressBarDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 317 | 857 | `CloseProgressBarDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 318 | 858 | `ShowSayItemDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 319 | 859 | `CloseSayItemDlg` | `24375-24383` | 9 | REAL |
| 320 | 860 | `ShowDGJPointsDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 321 | 861 | `IsDGJPointsShow` | n/a | n/a | ABSTRACT_NO_BODY |
| 322 | 862 | `GetUserStateShieldRect` | n/a | n/a | ABSTRACT_NO_BODY |
| 323 | 863 | `DMessageDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 324 | 864 | `DMessageDiceDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 325 | 865 | `DMessageLoadDataDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 326 | 866 | `DMessageNoticeDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 327 | 867 | `ShowMDlg` | `1865-1889` | 25 | PENDING |
| 328 | 868 | `ShowGorupJoinDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 329 | 869 | `ResetMenuDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 330 | 870 | `CloseMDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 331 | 871 | `ToggleShowGroupDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 332 | 872 | `ViewBottomBox` | n/a | n/a | ABSTRACT_NO_BODY |
| 333 | 873 | `MySelfAbilChange` | n/a | n/a | ABSTRACT_NO_BODY |
| 334 | 874 | `MyHeroAbilChange` | n/a | n/a | ABSTRACT_NO_BODY |
| 335 | 875 | `SetMagicKeyDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 336 | 876 | `CloseDKeySelDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 337 | 877 | `RefLoadConfig` | n/a | n/a | ABSTRACT_NO_BODY |
| 338 | 878 | `OpenUserState` | n/a | n/a | ABSTRACT_NO_BODY |
| 339 | 879 | `AttactkModeChange` | `20368-20370` | 3 | REAL |
| 340 | 880 | `OpenDChallengeDlg` | `20752-20754` | 3 | ORIGINAL_EMPTY |
| 341 | 881 | `CloseDChallengeDlg` | `20757-20773` | 17 | PENDING |
| 342 | 882 | `RefreshChallengeDlg` | `20776-20778` | 3 | ORIGINAL_EMPTY |
| 343 | 883 | `OpenDUpgradeDlg` | `20742-20744` | 3 | REAL |
| 344 | 884 | `CloseDUpgradeDlg` | `20747-20749` | 3 | REAL |
| 345 | 885 | `OpenDRandomCodeDlg` | `21063-21065` | 3 | REAL |
| 346 | 886 | `CloseDRandomCodeDlg` | `21068-21070` | 3 | REAL |
| 347 | 887 | `ReInputDRandomCode` | `21073-21075` | 3 | ORIGINAL_EMPTY |
| 348 | 888 | `NpcItemButtonDirectPaint` | `12629-12793` | 165 | PENDING |
| 349 | 889 | `NpcItemButtonMouseMove` | `12801-12824` | 24 | PENDING |
| 350 | 890 | `NpcUserItemButtonDirectPaint` | `12835-13008` | 174 | PENDING |
| 351 | 891 | `NpcUserItemButtonMouseMove` | `13014-13030` | 17 | PENDING |
| 352 | 894 | `NpcLabelMouseMove` | `13045-13088` | 44 | PENDING |
| 353 | 897 | `NpcButtonMouseMove` | `13103-13146` | 44 | PENDING |
| 354 | 900 | `ItemBoxButtonClick` | `23135-23198` | 64 | PENDING |
| 355 | 901 | `ItemBoxButtonMouseMove` | `23207-23249` | 43 | PENDING |
| 356 | 902 | `ItemBoxButtonStartSubDirectPaint` | `23255-23263` | 9 | PENDING |
| 357 | 903 | `ProgressButtonStartSubDirectPaint` | `23275-23364` | 90 | PENDING |
| 358 | 904 | `DCloseStateClick` | `18823-18825` | 3 | REAL |
| 359 | 905 | `DSSrvCloseClick` | `2295-2298` | 4 | REAL |
| 360 | 906 | `DOpenShopClick` | `18879-18881` | 3 | REAL |
| 361 | 907 | `DMyHeroStateClick` | `18990-18994` | 5 | PENDING |
| 362 | 908 | `DMyHeroStateCloseClick` | `18997-18999` | 3 | REAL |
| 363 | 909 | `DMyHeroBagClick` | `19002-19004` | 3 | ORIGINAL_EMPTY |
| 364 | 910 | `DMyHeroBagCloseClick` | `19007-19009` | 3 | REAL |
| 365 | 911 | `LabelDStateWinCharNameClick` | `20386-20393` | 8 | PENDING |
| 366 | 912 | `CancelItemMoving` | `1953-2123` | 171 | PENDING |
| 367 | 913 | `CancelMagicMoving` | `2126-2131` | 6 | PENDING |
| 368 | 914 | `DropMovingItem` | `2134-2151` | 18 | PENDING |
| 369 | 915 | `SoldOutGoods` | `16849-17036` | 188 | PENDING |
| 370 | 916 | `DelStorageItem` | `17042-17065` | 24 | PENDING |
| 371 | 917 | `GetMouseItemInfo` | `4502-11642` | 7141 | PENDING |
| 372 | 919 | `ShowMouseItemInfo` | `3710-4254` | 480 | PENDING |
| 373 | 921 | `GetMouseItemInfoWindow` | `3638-3646` | 9 | PENDING |
| 374 | 922 | `GetMouseFengHaoItemInfoWindow` | `3687-3692` | 6 | PENDING |
| 375 | 923 | `GetTzItemHintWindow` | `3431-3633` | 203 | PENDING |
| 376 | 924 | `GetTzItemHintWindowEx` | `3134-3363` | 230 | PENDING |
| 377 | 925 | `ClearMerchantSay` | `12427-12455` | 16 | PENDING |
| 378 | 927 | `AddGuildChat` | `17956-17964` | 9 | PENDING |
| 379 | 928 | `AddMissionMemo1` | `15537-15591` | 55 | PENDING |
| 380 | 929 | `AddMissionMemo2` | `16225-16519` | 295 | PENDING |
| 381 | 932 | `DrawGridItem` | `19022-19262` | 241 | PENDING |
| 382 | 935 | `DrawGridItemEx` | `19268-19290` | 23 | PENDING |
| 383 | 936 | `DrawGridStdItem` | `19301-19510` | 210 | PENDING |
| 384 | 939 | `DrawBodyItem` | `-1-19868` | 19870 | PENDING |
| 385 | 942 | `DrawBodyItemBelowEffect` | `19518-19563` | 46 | PENDING |
| 386 | 943 | `DrawBodyItemEffect` | `19876-20333` | 401 | PENDING |
| 387 | 945 | `CheckDGameShopDlgVisible` | n/a | n/a | ABSTRACT_NO_BODY |
| 388 | 946 | `OpenDGameShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 389 | 947 | `CloseDGameShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 390 | 948 | `OpenDMyShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 391 | 949 | `CloseDMyShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 392 | 950 | `OpenDUserShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 393 | 951 | `CloseDUserShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 394 | 952 | `CheckDHeroM2ShopDlgVisible` | n/a | n/a | ABSTRACT_NO_BODY |
| 395 | 953 | `OpenDHeroM2ShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 396 | 954 | `CloseDHeroM2ShopDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 397 | 957 | `CloseDHeroM2InputPriceDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 398 | 958 | `OpenDHeroM2ShopRemoteDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 399 | 959 | `CloseDHeroM2ShopRemoteDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 400 | 960 | `DBotUserShopClick` | `20566-20568` | 3 | REAL |
| 401 | 961 | `DDownHorseClick` | `20571-20575` | 5 | REAL |
| 402 | 962 | `DWebClick` | `20578-20580` | 3 | REAL |
| 403 | 963 | `DActionLogClick` | `20583-20585` | 3 | REAL |
| 404 | 964 | `SetMyShopPageCount` | n/a | n/a | ABSTRACT_NO_BODY |
| 405 | 965 | `SetGameShopPageCount` | n/a | n/a | ABSTRACT_NO_BODY |
| 406 | 966 | `SetUserShopItemPageCount` | n/a | n/a | ABSTRACT_NO_BODY |
| 407 | 967 | `DDeleteHumanDlgCloseClick` | `20588-20590` | 3 | ORIGINAL_EMPTY |
| 408 | 968 | `DGetBackDeleteHumanClick` | `20593-20596` | 4 | REAL |
| 409 | 969 | `DListViewDeleteHumanListItemClick` | `20604-20634` | 31 | PENDING |
| 410 | 970 | `DSpecialCmdMenuClick` | `20641-20656` | 16 | PENDING |
| 411 | 971 | `MerchantDlgPaint` | `12458-12460` | 3 | REAL |
| 412 | 972 | `DDiceDlgProcess` | `20727-20737` | 11 | PENDING |
| 413 | 973 | `DChallengeItemReturnBag` | `20781-20787` | 7 | PENDING |
| 414 | 974 | `ChallengeZeroGold` | `20790-20795` | 6 | REAL |
| 415 | 975 | `DChallengeOkClick` | `20798-20811` | 14 | PENDING |
| 416 | 976 | `DChallengeCloseClick` | `20814-20819` | 6 | REAL |
| 417 | 977 | `DDChallengeGoldClick` | `20825-20859` | 35 | PENDING |
| 418 | 978 | `DChallengeGridGridPaint` | `20866-20888` | 23 | PENDING |
| 419 | 979 | `DChallengeGridGridSelect` | `20897-20982` | 86 | PENDING |
| 420 | 980 | `DChallengeGridGridMouseMove` | `20989-21003` | 15 | PENDING |
| 421 | 981 | `DChallengeRemoteGridGridPaint` | `21010-21031` | 22 | PENDING |
| 422 | 982 | `DChallengeRemoteGridGridMouseMove` | `21038-21052` | 15 | PENDING |
| 423 | 983 | `DControlHelpClick` | `21055-21060` | 6 | REAL |
| 424 | 984 | `DRandomCodeDlgDirectPaint` | `21080-21086` | 7 | PENDING |
| 425 | 985 | `DItemBagDirectPaint` | `21091-21093` | 3 | ORIGINAL_EMPTY |
| 426 | 986 | `DStPageUpClick` | `21096-21098` | 3 | ORIGINAL_EMPTY |
| 427 | 987 | `DMainBottomCenterHeightChangeQuery` | `21101-21103` | 3 | ORIGINAL_EMPTY |
| 428 | 988 | `DMainBottomCenterHeightChanged` | `21106-21108` | 3 | ORIGINAL_EMPTY |
| 429 | 989 | `DMainBottomDlgStartSubPaint` | `21111-21113` | 3 | ORIGINAL_EMPTY |
| 430 | 990 | `DMagicBallStopPaint` | `21116-21118` | 3 | ORIGINAL_EMPTY |
| 431 | 991 | `DMagicBallGetHumAbility` | `21121-21137` | 17 | PENDING |
| 432 | 992 | `OnGetGroupAttackProgress` | `21140-21156` | 17 | PENDING |
| 433 | 993 | `DStateMemo4DirectPaint` | `21159-21161` | 3 | ORIGINAL_EMPTY |
| 434 | 994 | `DHeroStateWinDirectPaint` | `21164-21166` | 3 | ORIGINAL_EMPTY |
| 435 | 995 | `DBotMiniMapClick` | `21169-21171` | 3 | ORIGINAL_EMPTY |
| 436 | 996 | `DUserState1DirectPaint` | `21174-21176` | 3 | ORIGINAL_EMPTY |
| 437 | 997 | `DWeaponUS1MouseMove` | `21179-21181` | 3 | ORIGINAL_EMPTY |
| 438 | 998 | `DMinMapDlgDirectPaint` | `21184-21186` | 3 | ORIGINAL_EMPTY |
| 439 | 999 | `DSelectChrWindowsPaint` | `21189-21191` | 3 | ORIGINAL_EMPTY |
| 440 | 1000 | `DSWWeaponMouseMove` | `21194-21196` | 3 | ORIGINAL_EMPTY |
| 441 | 1001 | `ItemBagDirectPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 442 | 1002 | `StPageUpClick` | n/a | n/a | ABSTRACT_NO_BODY |
| 443 | 1003 | `MainBottomCenterHeightChangeQuery` | n/a | n/a | ABSTRACT_NO_BODY |
| 444 | 1004 | `MainBottomCenterHeightChanged` | n/a | n/a | ABSTRACT_NO_BODY |
| 445 | 1005 | `MainBottomDlgStartSubPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 446 | 1006 | `MagicBallStopPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 447 | 1007 | `StateMemo4DirectPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 448 | 1008 | `HeroStateWinDirectPaint` | n/a | n/a | ABSTRACT_NO_BODY |
| 449 | 1009 | `BotMiniMapClick` | `21200-21405` | 206 | PENDING |
| 450 | 1010 | `UserState1DirectPaint` | `21418-21591` | 174 | PENDING |
| 451 | 1011 | `WeaponUS1MouseMove` | `21607-21961` | 355 | PENDING |
| 452 | 1012 | `MinMapDlgDirectPaint` | `21986-22265` | 280 | PENDING |
| 453 | 1013 | `SelectChrWindowsPaint` | `22607-23116` | 510 | PENDING |
| 454 | 1014 | `SWWeaponMouseMove` | n/a | n/a | ABSTRACT_NO_BODY |
| 455 | 1015 | `OpenDBoxDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 456 | 1016 | `CloseDBoxDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 457 | 1017 | `OpenDItemBoxDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 458 | 1018 | `CloseDItemBoxDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 459 | 1019 | `SetGuessfinger` | n/a | n/a | ABSTRACT_NO_BODY |
| 460 | 1020 | `SetDrinkDrunkValue` | n/a | n/a | ABSTRACT_NO_BODY |
| 461 | 1022 | `CheckShowRemoveStoneForm` | n/a | n/a | ABSTRACT_NO_BODY |
| 462 | 1023 | `ShowRemoveStoneForm` | n/a | n/a | ABSTRACT_NO_BODY |
| 463 | 1024 | `CheckDMinMapBigDlgMouseDown` | n/a | n/a | ABSTRACT_NO_BODY |
| 464 | 1025 | `CheckDMinMapBigDlgVisible` | n/a | n/a | ABSTRACT_NO_BODY |
| 465 | 1026 | `OpenDMinMapBigDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 466 | 1027 | `CloseDMinMapBigDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 467 | 1028 | `CheckDMinMapExDlgVisible` | n/a | n/a | ABSTRACT_NO_BODY |
| 468 | 1029 | `CheckDMinMapExShowButton` | n/a | n/a | ABSTRACT_NO_BODY |
| 469 | 1030 | `OpenDMinMapExDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 470 | 1031 | `CloseDMinMapExDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 471 | 1032 | `SelectChrWindowsPaint205` | `22355-22599` | 245 | PENDING |
| 472 | 1033 | `DStUpgradeMagicButtonClick` | `23379-23384` | 6 | PENDING |
| 473 | 1034 | `DStHeroUpgradeMagicButtonClick` | `23369-23374` | 6 | PENDING |
| 474 | 1035 | `DStUpgradeMagicMouseMove` | `23395-23423` | 29 | PENDING |
| 475 | 1036 | `DHeroStUpgradeMagicMouseMove` | `23434-23462` | 29 | PENDING |
| 476 | 1037 | `DStateRefurbishLabelClick` | `23465-23469` | 5 | PENDING |
| 477 | 1038 | `GuildGroupIndex` | n/a | n/a | ABSTRACT_NO_BODY |
| 478 | 1041 | `ClearDMissionMemo` | `24126-24131` | 6 | PENDING |
| 479 | 1042 | `DMouseMoveClearHints` | `24134-24137` | 4 | REAL |
| 480 | 1043 | `DNpcScrollBoxMove` | `24142-24153` | 12 | PENDING |
| 481 | 1044 | `DNpcScrollBoxDown` | `24158-24167` | 10 | PENDING |
| 482 | 1045 | `DNpcScrollBoxUp` | `24172-24181` | 10 | PENDING |
| 483 | 1048 | `DLieDragonPaint` | `24189-24204` | 16 | PENDING |
| 484 | 1050 | `DLieDragonClosePaint` | `24215-24227` | 13 | PENDING |
| 485 | 1052 | `DLieDragonCloseClick` | `24207-24209` | 3 | REAL |
| 486 | 1054 | `DLieDragonNextPagePaint` | `24251-24263` | 13 | PENDING |
| 487 | 1056 | `DLieDragonPrevPagePaint` | `24233-24245` | 13 | PENDING |
| 488 | 1058 | `DLieDragonNextPageClick` | `24266-24285` | 20 | PENDING |
| 489 | 1060 | `DGoToLieDragontPaint` | `24297-24309` | 13 | PENDING |
| 490 | 1062 | `DGoToLieDragonClick` | `24288-24291` | 4 | REAL |
| 491 | 1064 | `DLieDragonNpcPaint` | `24320-24329` | 10 | PENDING |
| 492 | 1066 | `DLieDragonNpcCloseClick` | `24312-24314` | 3 | REAL |
| 493 | 1069 | `DSayItemDlgPaint` | `24336-24354` | 19 | PENDING |
| 494 | 1070 | `DSayItemDlgCloseClick` | `24357-24359` | 3 | REAL |
| 495 | 1071 | `DSayItemDlgMouseDown` | `24362-24365` | 4 | REAL |
| 496 | 1072 | `DSayItemDlgMouseMove` | `24368-24372` | 5 | REAL |
| 497 | 1073 | `DUpdateStatusDlgPaint` | `24389-24398` | 10 | PENDING |
| 498 | 1074 | `DUpdateStatusDlgMouseEnter` | `24407-24462` | 56 | PENDING |
| 499 | 1075 | `DUpdateStatusDlgMouseLeave` | `24465-24467` | 3 | REAL |
| 500 | 1076 | `DUpdateStatusDlgDblClick` | `24470-24472` | 3 | REAL |
| 501 | 1079 | `UpdateBusinessStatusText` | n/a | n/a | ABSTRACT_NO_BODY |
| 502 | 1080 | `UpdateShopMoneyInfo` | n/a | n/a | ABSTRACT_NO_BODY |
| 503 | 1082 | `GetLastHistroySendSay` | n/a | n/a | REAL |
| 504 | 1083 | `GetPreHistroySendSay` | n/a | n/a | REAL |
| 505 | 1084 | `GetNextHistroySendSay` | n/a | n/a | REAL |
| 506 | 1085 | `IsInputChatEdit` | n/a | n/a | REAL |
| 507 | 1086 | `ShowChatEdit` | `24495-24500` | 6 | REAL |
| 508 | 1087 | `HideChatEdit` | `24503-24508` | 6 | REAL |
| 509 | 1088 | `FindMagicButton` | `24595-24604` | 10 | REAL |
| 510 | 1089 | `ClearScreenMagicButtons` | `24514-24520` | 7 | REAL |
| 511 | 1090 | `AddScreenMagicButton` | `24525-24555` | 31 | REAL |
| 512 | 1091 | `DelScreenMagicButton` | `24561-24589` | 12 | REAL |
| 513 | 1093 | `SaveMagicButtons` | `24687-24734` | 48 | REAL |
| 514 | 1094 | `LoadMagicButtons` | `24743-24799` | 57 | PENDING |
| 515 | 1095 | `SetAuctionPageCount` | n/a | n/a | ABSTRACT_NO_BODY |
| 516 | 1096 | `RequestAuctionItems` | n/a | n/a | ABSTRACT_NO_BODY |
| 517 | 1097 | `OpenGameLevelDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 518 | 1098 | `CloseGameLevelDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 519 | 1099 | `RefreshGuardianLevelStatueMonHP` | n/a | n/a | ABSTRACT_NO_BODY |
| 520 | 1100 | `SetGuardianLevelBatchNo` | n/a | n/a | ABSTRACT_NO_BODY |
| 521 | 1101 | `ShowGuardianLevelResult` | n/a | n/a | ABSTRACT_NO_BODY |
| 522 | 1102 | `RefreshAdjustAbilityValue` | n/a | n/a | ABSTRACT_NO_BODY |
| 523 | 1103 | `DMessageDlgDeleteUser` | n/a | n/a | ABSTRACT_NO_BODY |
| 524 | 1104 | `DCustomButtonClick` | `24918-24922` | 5 | REAL |
| 525 | 1105 | `RestoreButton_DItemBagArrange` | n/a | n/a | ABSTRACT_NO_BODY |
| 526 | 1106 | `ResetBagPageButton` | n/a | n/a | ABSTRACT_NO_BODY |
| 527 | 1107 | `ClearNewGroupMember` | n/a | n/a | ABSTRACT_NO_BODY |
| 528 | 1108 | `RefreshNewGroupMember` | n/a | n/a | ABSTRACT_NO_BODY |
| 529 | 1109 | `ReshowNewGroupMember` | n/a | n/a | ABSTRACT_NO_BODY |
| 530 | 1110 | `CheckDMinMapDlgExVisible` | n/a | n/a | ABSTRACT_NO_BODY |
| 531 | 1111 | `ReSetControl` | n/a | n/a | ABSTRACT_NO_BODY |
| 532 | 1112 | `OpenDBetterItemDlg` | n/a | n/a | ABSTRACT_NO_BODY |
| 533 | 1113 | `CloseDBetterItemDlg` | n/a | n/a | ABSTRACT_NO_BODY |


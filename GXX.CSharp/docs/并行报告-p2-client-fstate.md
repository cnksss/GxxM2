# 并行报告 · 车道 `p2-client-fstate`（FState.pas 分片移植）

> 车道：`par/p2-client-fstate` ｜ 工作树：`.worktrees/p2-client-fstate`
> 源单元：`Source/Client-HGE/GUI/Share/FState.pas`
> **实测规模：25,165 物理行 / 22,503 非空行 / 378 个例程声明（含 3 个被块注释掉的死代码）**
> 类型：`interface` 段 1-1118；`implementation` 段 1119-25165
> 交付形态：**分片交付**（本波次完成切片 1/2/3，其余如实列出待接清单）

---

## 1. 分支与提交

| 提交 | 内容 |
|---|---|
| `578c7444` | 并行批次P1-1：声明段 100%（脚本抽取 + 反射/原文双重回读） |
| `9a7c75e6` | 并行批次P1-2：纯函数与无控件树依赖的行为 |
| `b5376414` | 并行批次P1-3：收尾补齐（OpenGuildViewMemeberInfo / TNpcGraphicButton） |

分支基线：`main @ ab8b51d7`（车道创建时的工作树 HEAD）。

---

## 2. 新增文件清单

**源码（全部新建，未改任何既有文件、未改 `GXX.slnx` / `*.csproj` / `Directory.Build.props` / 台账 / 审计 / Checklist / tools）**

| 文件 | 说明 |
|---|---|
| `GXX.CSharp/src/GXX.Client/GUI/Share/FStateDeclGen.ps1` | **生成器**：从 FState.pas 抽取声明面 → 两张 `.g.cs`。纯 ASCII（规避台账 §8.1 的 PS 5.1 ANSI 坑），可重复运行 |
| `GXX.CSharp/src/GXX.Client/GUI/Share/FStateDeclManifest.g.cs` | 常量 9 / 类型 24 / TFrmDlg 字段 201 / 方法 538 四张清单 + SHA-256 指纹（脚本生成） |
| `GXX.CSharp/src/GXX.Client/GUI/Share/TFrmDlg.Decl.g.cs` | TFrmDlg 完整声明面（201 字段 + 538 成员），未移植成员一律 `throw`（脚本生成） |
| `GXX.CSharp/src/GXX.Client/GUI/Share/FStateTypes.cs` | 24 个类型全量；4 个容器类与 13 个 NPC 控件类的 1:1 实现 |
| `GXX.CSharp/src/GXX.Client/GUI/Share/TFrmDlg.Core.cs` | 手写部分：`Create` 1:1 + 22 个可移植成员 + `FStateGlobal` |
| `GXX.CSharp/src/GXX.Client/GUI/Share/FStateSeams.cs` | 接缝层（复用车道1 的 DxComponent / MShareGlobals，只补缺口） |
| `GXX.CSharp/src/GXX.Client/GUI/Share/FStatePure.cs` | 单元级纯函数（GetHitLines / GetJobText(Ex) / WriteOutStr）+ Delphi 串原语 |

**测试（新建）**

| 文件 | 例数 |
|---|---|
| `GXX.CSharp/tests/GXX.Client.Tests/GuiShareDeclTests.cs` | 33 |
| `GXX.CSharp/tests/GXX.Client.Tests/GuiSharePureTests.cs` | 95 |

**文档**

| 文件 | 说明 |
|---|---|
| `GXX.CSharp/docs/并行报告-p2-client-fstate.md` | 本报告 |

> 抽取/校验脚本已随车道提交：`src/GXX.Client/GUI/Share/FStateDeclGen.ps1`（声明面生成）与
> `src/GXX.Client/GUI/Share/FStateExtract.ps1`（例程清单抽取，含 `(* *)`/`{ }` 块注释识别，
> 本报告 §3.5 即其产物）。两者均为**纯 ASCII**（规避台账 §8.1 的 PS 5.1 ANSI 解析坑）、可重复运行；
> 工作树内**未留任何临时探查目录**。

---

## 3. FState.pas 全量清单

### 3.0 先说三个**既往车道事实的更正**（本车道实测）

任务书与既往车道报告把以下符号记作"FState.pas 的公共头内容"，**实测均不成立**：

| 被记在 FState 名下的符号 | 实测归属 | 证据 |
|---|---|---|
| `THintLines`（类） | **`DrawScrn.pas:318`**（前向声明 `:99`） | `Select-String 'THintLines\s*=\s*class'` 全树只命中 DrawScrn.pas |
| `GetHintFontSize` / `GetHintFontStyle` / `GetHintFontStroke` | **`MShare.pas:2999/3001/3003`（声明）, `11735/11740/11752`（实现）** | 同上，FState.pas 内 55 处全是**调用点** |
| `GetRGB` / 调色板 | **`MShare.pas:2897`（声明）, `4111`（实现）** | FState.pas 内 114 处全是调用点，**无声明** |
| `g_DefColorTable` | **`MShare.pas`**（另有 `GameImages/Wzl/Wis/Pak/DxCanvas/NPCFormDeBug` 引用） | FState.pas 内 **0 命中** |
| `g_AcupointLevels` | **`MShare.pas` / `ClMain.pas` / `StateWindows.pas` / SerialWindowsDlg.pas** | FState.pas 内 **0 命中** |
| `GetGodBlessItem` | 实际名为 **`GetGodBlessItems`（复数）**，且是 **TFrmDlg 方法内的嵌套函数**（原文 3093 / 3391），不是单元级函数 | `GetGodBlessItem` 在 FState.pas 只有 4 处、全是嵌套定义/调用 |
| `g_MySelf` / `g_MyHero` / `g_UserState1` / `g_MagicList` | **全是 `MShare.pas` 的单元级全局**；FState.pas 只有调用点 | 本车道实测 |

**关键结论**：FState.pas **只有一个单元级全局变量** ——

```
原文 1116-1117:  var
                   FrmDlg:TFrmDlg = nil;
原文 1132-1133:  const
                   ConditionOKHitColor = clWhite;   { implementation 段单元级常量 }
```

因此：
- 车道8 在 `TStateWindowsText.cs` 里登记的接缝"`THintLines / GetHintFontSize / GetHintFontStyle / GetHintFontStroke`（**FState.pas**）未移植"**归属错误**，应由 `MShare.pas` / `DrawScrn.pas` 的车道负责；本车道已按原文调用形态提供 `MShareHintFont` / `THintLines` 注入接缝，但**不应**被视为 FState 的缺口。
- 任务书切片优先级里的"颜色表、提示行排版（`THintLines`/`GetHintFont*`/`GetRGB`/`g_DefColorTable`）"**不属于本单元**；本车道改为按 FState.pas 的真实内容重排切片（见 §3.5 覆盖表）。

### 3.1 单元级常量（9 条，原文 49-57 行）

| # | line | constant |
|---|---|---|
| 1 | 49 | `BOTTOMBOARD800` |
| 2 | 50 | `BOTTOMBOARD1024` |
| 3 | 51 | `VIEWCHATLINE` |
| 4 | 52 | `MAXSTATEPAGE` |
| 5 | 53 | `LISTLINEHEIGHT` |
| 6 | 54 | `MAXMENU` |
| 7 | 55 | `MINMAP_RECT_SIZE` |
| 8 | 56 | `MINMAP_RECT_SIZE2` |
| 9 | 57 | `AdjustAbilHints:array[0..8] of string` |


外加 `implementation` 段单元级常量 1 条：

| 行 | 常量 | 值 |
|---|---|---|
| 1133 | `ConditionOKHitColor` | `clWhite` |

### 3.2 类型（24 个，原文 60-1114 行）

原文 `type` 段还包含 `TFrmDlg` 本身（原文 311-1114，见 §3.3/§3.4）。

| # | line | type | source kind | managed form |
|---|---|---|---|---|
| 1 | 60 | `TArrHintWindows` | array | array |
| 2 | 62 | `PTArrHintWindows` | pointer | pointer |
| 3 | 64 | `TSpotDlgMode` | enum | enum |
| 4 | 66 | `TDiceInfo` | record | record |
| 5 | 76 | `pTDiceInfo` | pointer | pointer |
| 6 | 78 | `TGuildGroup` | class:TObject | class:TObject |
| 7 | 91 | `TGuildGroupList` | class:TObject | class:TObject |
| 8 | 109 | `PShowGuildInfo` | pointer | pointer |
| 9 | 111 | `TShowGuildInfo` | record | record |
| 10 | 117 | `TShowGuildList` | class:TObject | class:TObject |
| 11 | 132 | `TGuildJoinUserList` | class:TObject | class:TObject |
| 12 | 146 | `TNpcButton` | class:TDxImageButton | class:TDxImageButton |
| 13 | 165 | `TNpcGraphicButton` | class:TDxImageButton | class:TDxImageButton |
| 14 | 178 | `TNpcItemButton` | class:TDxImageButton | class:TDxImageButton |
| 15 | 193 | `TNpcUserItemButton` | class:TNpcItemButton | class:TNpcItemButton |
| 16 | 198 | `TNpcItemBoxButton` | class:TDxImageButton | class:TDxImageButton |
| 17 | 204 | `TNpcProgressBoxButton` | class:TDxImageButton | class:TDxImageButton |
| 18 | 225 | `TNpcLabel` | class:TDxLabel | class:TDxLabel |
| 19 | 237 | `TCountDownLabel` | class:TDxLabel | class:TDxLabel |
| 20 | 253 | `TImgCountDownButton` | class:TDxImageButton | class:TDxImageButton |
| 21 | 270 | `TNpcInputEdit` | class:TDxEdit | class:TDxEdit |
| 22 | 284 | `TNpcScrollBox` | class:TDxScrollBox | class:TDxScrollBox |
| 23 | 300 | `TMissionLabel` | class:TDxTreeNode | class:TDxTreeNode |
| 24 | 305 | `TMagicButton` | class:TDxImageButton | class:TDxImageButton |


### 3.3 TFrmDlg 字段（201 条，原文 311-1114 行）

> 多名字一行（原文 459 行的 4 个 `FDStorageViewDlg*`）按名字展开，故条数大于源码行数。

| # | line | field | type |
|---|---|---|---|
| 1 | 312 | `DBackground` | TDxControlEngine |
| 2 | 313 | `DEdId` | TDxEdit |
| 3 | 314 | `DEdPasswd` | TDxEdit |
| 4 | 315 | `DEdNewId` | TDxEdit |
| 5 | 316 | `DEdConfirm` | TDxEdit |
| 6 | 317 | `DEdChgId` | TDxEdit |
| 7 | 318 | `DEdChgL2Pw` | TDxEdit |
| 8 | 319 | `DEdChrName` | TDxEdit |
| 9 | 320 | `DscStart` | TDxImageButton |
| 10 | 321 | `DscUpChr` | TDxImageButton |
| 11 | 322 | `DscDownChr` | TDxImageButton |
| 12 | 323 | `DEdChat` | TDxImageEdit |
| 13 | 324 | `DGuildDlg` | TDxImageForm |
| 14 | 325 | `DChatMemo` | TDxChatMemo |
| 15 | 326 | `DWhisperMemo` | TDxChatMemo |
| 16 | 327 | `DBotPlusAbil` | TDxImageButton |
| 17 | 328 | `DGrpAllowGroup` | TDxImageButton |
| 18 | 329 | `DMemoGroupMembers` | TDxListView |
| 19 | 330 | `DLabelGroupMembersOwner` | TDxLabel |
| 20 | 331 | `DMerchantDlg` | TDxImageForm |
| 21 | 332 | `DMerchantDlgClose` | TDxImageButton |
| 22 | 333 | `DMerchantDlgHelp` | TDxImageButton |
| 23 | 334 | `DMissionMemo` | TDxScrollBox |
| 24 | 335 | `DCheckBoxAutoAnswersWhisper` | TDxImageButton |
| 25 | 336 | `DEditAutoAnswersWhisper` | TDxEdit |
| 26 | 337 | `DSpecialCmdMenu` | TDxPopupMenu |
| 27 | 338 | `DListViewDeleteHuman` | TDxListView |
| 28 | 339 | `LabelDSelectChrServerName` | TDxLabel |
| 29 | 340 | `LabelSelectChrDlgCharName1` | TDxLabel |
| 30 | 341 | `LabelSelectChrDlgJob1` | TDxLabel |
| 31 | 342 | `LabelSelectChrDlgLevel1` | TDxLabel |
| 32 | 343 | `LabelSelectChrDlgCharName2` | TDxLabel |
| 33 | 344 | `LabelSelectChrDlgJob2` | TDxLabel |
| 34 | 345 | `LabelSelectChrDlgLevel2` | TDxLabel |
| 35 | 346 | `LabelSelectChrDlgCharName3` | TDxLabel |
| 36 | 347 | `LabelSelectChrDlgJob3` | TDxLabel |
| 37 | 348 | `LabelSelectChrDlgLevel3` | TDxLabel |
| 38 | 349 | `DHelmetUS1_` | TDxImageButton |
| 39 | 350 | `DMainMenu` | TDxPopupMenu |
| 40 | 353 | `DMinMapDlg` | TDxImageForm |
| 41 | 354 | `DSayItemDlg` | TDxImageForm |
| 42 | 355 | `DSayItemDlgClose` | TDxImageButton |
| 43 | 356 | `DUpdateStatusDlg` | TDxImageForm |
| 44 | 357 | `DLieDragon` | TDxImageForm |
| 45 | 358 | `DGoToLieDragon` | TDxImageButton |
| 46 | 359 | `DLieDragonClose` | TDxImageButton |
| 47 | 360 | `DLieDragonNextPage` | TDxImageButton |
| 48 | 361 | `DLieDragonPrevPage` | TDxImageButton |
| 49 | 362 | `DLieDragonNpc` | TDxImageForm |
| 50 | 363 | `DLieDragonNpcClose` | TDxImageButton |
| 51 | 366 | `MsgText` | string |
| 52 | 367 | `m_nDiceCount` | int |
| 53 | 368 | `m_boPlayDice` | bool |
| 54 | 369 | `m_Dice` | TDiceInfo[10] |
| 55 | 370 | `MerchantCMD` | string |
| 56 | 371 | `MerchantName` | string |
| 57 | 372 | `MerchantFace` | int |
| 58 | 373 | `MDlgStr` | string |
| 59 | 374 | `RequireAddPoints` | bool |
| 60 | 375 | `SelectMenuStr` | string |
| 61 | 376 | `LastestClickTime` | uint |
| 62 | 377 | `SpotDlgMode` | TSpotDlgMode |
| 63 | 378 | `MenuList` | TGList |
| 64 | 379 | `menuindex` | int |
| 65 | 380 | `CurDetailItem` | string |
| 66 | 381 | `MenuTopLine` | int |
| 67 | 382 | `TradingItemTopLine` | int |
| 68 | 383 | `TradingItemSelIndex` | int |
| 69 | 384 | `BoDetailMenu` | bool |
| 70 | 385 | `BoStorageMenu` | bool |
| 71 | 386 | `BoNoDisplayMaxDura` | bool |
| 72 | 387 | `BoMakeDrugMenu` | bool |
| 73 | 389 | `NewAccountTitle` | string |
| 74 | 390 | `DlgEditText` | string |
| 75 | 391 | `GuildGroupListPage` | int |
| 76 | 392 | `GuildGroupList` | TGuildGroupList |
| 77 | 393 | `Guild` | string |
| 78 | 394 | `GuildFlag` | string |
| 79 | 395 | `GuildCommanderMode` | int |
| 80 | 396 | `GuildStrs` | TStringList |
| 81 | 397 | `GuildStrs2` | TStringList |
| 82 | 398 | `GuildNotice` | TStringList |
| 83 | 399 | `GuildWJ` | TStringList |
| 84 | 400 | `GuildMembers` | TStringList |
| 85 | 401 | `GuildTopLine` | int |
| 86 | 402 | `GuildEditHint` | string |
| 87 | 403 | `GuildChats` | TStringList |
| 88 | 404 | `WantAddGuildName` | string |
| 89 | 405 | `ShowGuildListPage` | int |
| 90 | 406 | `ShowGuildList` | TShowGuildList |
| 91 | 407 | `GuildJoinUserList` | TGuildJoinUserList |
| 92 | 408 | `GuildJoinUserListPage` | int |
| 93 | 409 | `GuildOnlineCount` | int |
| 94 | 410 | `GuildMemberCount` | int |
| 95 | 411 | `GuildMemberMaxLimit` | int |
| 96 | 412 | `BoGuildChat` | bool |
| 97 | 413 | `HeroMagicIndex` | int |
| 98 | 414 | `MagicIndex` | int |
| 99 | 415 | `RankingPage` | int |
| 100 | 416 | `Initialized` | bool |
| 101 | 417 | `ShopTabPage` | int |
| 102 | 418 | `RankingSelectLine` | int |
| 103 | 419 | `SelDeleteCharName` | string |
| 104 | 420 | `Memo` | TMemo |
| 105 | 421 | `ViewDlgEdit` | bool |
| 106 | 422 | `MenuTop` | int |
| 107 | 423 | `BlinkTime` | uint |
| 108 | 424 | `BlinkCount` | int |
| 109 | 425 | `BoxDlgWideScreen` | bool |
| 110 | 426 | `m_sLoginId` | string |
| 111 | 427 | `m_sLoginPasswd` | string |
| 112 | 428 | `FAbilHPTick` | uint |
| 113 | 429 | `FAbilMPTick` | uint |
| 114 | 430 | `FAbilHPIndex` | int |
| 115 | 431 | `FAbilMPIndex` | int |
| 116 | 432 | `GameGoldDealMenuIndex` | int |
| 117 | 433 | `GuildMemoVisible` | bool |
| 118 | 434 | `StorageHeroInfos` | TStorageHeroInfo[2] |
| 119 | 436 | `ClientVersion` | TClientVersion |
| 120 | 437 | `ShowMiniBigMapXY` | bool |
| 121 | 438 | `BigMinMapX` | int |
| 122 | 439 | `BigMinMapY` | int |
| 123 | 440 | `MapTextureWidth` | int |
| 124 | 441 | `MapTextureHeight` | int |
| 125 | 442 | `FRemoveStoneLock` | bool |
| 126 | 443 | `FRemoveStoneShowEffect` | bool |
| 127 | 444 | `FRemoveStoneShowEffectTick` | uint |
| 128 | 445 | `FRemoveStoneShowEffectIdx` | int |
| 129 | 446 | `FRemoveStoneIdx` | int |
| 130 | 447 | `FRemoveStoneIndex` | int |
| 131 | 448 | `FOverlapStoneItemMakeIndex` | int |
| 132 | 449 | `FRemoveStoneHideStone` | bool |
| 133 | 450 | `FRemoveStoneHideStoneMsg` | string |
| 134 | 451 | `FRemoveStoneHideStoneEffect` | bool |
| 135 | 452 | `FRemoveStoneHideStoneEffectTick` | uint |
| 136 | 453 | `FRemoveStoneHideStoneEffectIdx` | int |
| 137 | 454 | `FShowProgressBarTime` | uint |
| 138 | 455 | `FShowProgressBarTick` | uint |
| 139 | 456 | `FShowProgressBarCmd` | int |
| 140 | 457 | `FShowProgressBarParam1` | int |
| 141 | 458 | `FShowProgressBarParam2` | int |
| 142 | 459 | `FDStorageViewDlgCount` | int |
| 143 | 459 | `FDStorageViewDlgMaxCount` | int |
| 144 | 459 | `FDStorageViewDlgPage` | int |
| 145 | 459 | `FDStorageViewDlgMaxPage` | int |
| 146 | 460 | `FNpcStoragePage` | int |
| 147 | 461 | `FGameShopPageControlIndex` | int |
| 148 | 462 | `FGameShopPageIndex` | int |
| 149 | 463 | `FGroupChatBottomSpace` | int |
| 150 | 464 | `FChatMemoBottomSpace` | int |
| 151 | 465 | `FEditChatBottomSpace` | int |
| 152 | 466 | `FChangeChatHeightBottomSpace` | int |
| 153 | 467 | `FDayBrightIconStartIndex` | int |
| 154 | 468 | `FMagicBallStartIndex` | int |
| 155 | 470 | `FHeroState185IconStartIndex` | int |
| 156 | 471 | `FHeroStateIconStartIndex` | int |
| 157 | 473 | `FHeroBagItem40ImageIndex` | int |
| 158 | 475 | `FBotPlusAbilFlashStartIndex` | int |
| 159 | 477 | `FJewelryBoxUpImageIndex` | int |
| 160 | 478 | `FJewelryBoxDownImageIndex` | int |
| 161 | 479 | `FHeroJewelryBoxUpImageIndex` | int |
| 162 | 480 | `FHeroJewelryBoxDownImageIndex` | int |
| 163 | 481 | `_DLableMenuDlgC1` | TDxLabel |
| 164 | 482 | `_DLableMenuDlgC2` | TDxLabel |
| 165 | 483 | `_DLableMenuDlgC3` | TDxLabel |
| 166 | 484 | `_ListViewMenuDlg` | TDxListView |
| 167 | 486 | `dwControlHelpCickTick` | uint |
| 168 | 487 | `FIsRingLeft` | bool |
| 169 | 488 | `FIsArmRingLeft` | bool |
| 170 | 489 | `FIsFashionRingLeft` | bool |
| 171 | 490 | `FIsFashionArmRingLeft` | bool |
| 172 | 501 | `boSayItemDlgMoveOutClose` | bool |
| 173 | 502 | `nSayItemMakeIndex` | int |
| 174 | 503 | `FSayItemHintWin` | object |
| 175 | 504 | `FGuildGroupIndex` | int |
| 176 | 505 | `FGuildJoinJob` | int |
| 177 | 506 | `FGuildJoinLevel` | int |
| 178 | 507 | `FGuildJoinMsg` | string |
| 179 | 508 | `FGuildViewMemberInfo` | TGuildMemeberInfo |
| 180 | 509 | `FScreenMagicBtnList` | TList |
| 181 | 510 | `FStorageViewDlgMerchant` | long |
| 182 | 511 | `FMiniMapLoadIndex` | int |
| 183 | 512 | `FMiniMapLoadSurfaceTime` | uint |
| 184 | 513 | `FMiniMapSurface` | TTexture |
| 185 | 515 | `m_boRandomCodeClick` | bool |
| 186 | 516 | `FAuctionAllItemsSortField` | int |
| 187 | 517 | `FAuctionAllItemsSortASC` | bool |
| 188 | 518 | `FAuctionAllItemsPage` | int |
| 189 | 519 | `FAuctionMyItemsPage` | int |
| 190 | 520 | `FAuctionMyAuctioningItemsPage` | int |
| 191 | 521 | `FAuctionMyItemsBagPage` | int |
| 192 | 522 | `FAuctionMyItemsBagSelectIndex` | int |
| 193 | 523 | `FAuctionMyItemsPageCount` | int |
| 194 | 524 | `FAuctionAllItemsPageCount` | int |
| 195 | 525 | `FAuctionMyAuctioningItemsPageCount` | int |
| 196 | 526 | `FAuctionMyItemsBagPageCount` | int |
| 197 | 527 | `FGuardianLevelRewardItems` | TClientItem[4] |
| 198 | 528 | `FGuardianLevelItemCounts` | TGuardianLevelItemCounts |
| 199 | 529 | `FGuardianLevelStatueMonRecogId` | int |
| 200 | 530 | `FCurrentBagPage` | byte |
| 201 | 531 | `FExtBagPageCount` | byte |


### 3.4 TFrmDlg 方法/属性（538 条，原文 311-1114 行）

> `DONE` = 本波次已有 1:1 实现（或原文空体）；`PART` = 守卫/收集已实现、INI 主体为接缝；
> `TODO` = 已声明但方法体未移植（调用即 `throw NotSupportedException`，不会静默返回默认值）。
> 注意第 1 条不是 `Create` —— 原文 486-496 行是 `private` 段，先声明 4 个 `OnMagicButton*`，
> `Create` 在原文 533 行。

| # | line | member | kind | state |
|---|---|---|---|---|
| 1 | 493 | `OnMagicButtonClick` | procedure private | DONE |
| 2 | 494 | `OnMagicButtonDblClick` | procedure private | DONE |
| 3 | 495 | `OnMagicButtonMouseMove` | procedure private | TODO |
| 4 | 496 | `OnMagicButtonMove` | procedure private | DONE |
| 5 | 499 | `CreateNpcQRButtonFromText` | function private | TODO |
| 6 | 533 | `Create` | constructor public | DONE |
| 7 | 534 | `Destroy` | destructor public | DONE |
| 8 | 535 | `UpDate` | procedure public | TODO |
| 9 | 536 | `DBottomInRealArea` | procedure public | TODO |
| 10 | 537 | `DItemGridGridSelect` | procedure public | TODO |
| 11 | 538 | `DItemGridGridPaint` | procedure public | TODO |
| 12 | 539 | `DItemGridDblClick` | procedure public | TODO |
| 13 | 540 | `DBackgroundBackgroundClick` | procedure public | TODO |
| 14 | 541 | `DItemGridGridMouseMove` | procedure public | TODO |
| 15 | 542 | `DBelt1DirectPaint` | procedure public | TODO |
| 16 | 543 | `DBelt1DblClick` | procedure public | TODO |
| 17 | 544 | `DLoginCloseClick` | procedure public | TODO |
| 18 | 545 | `DLoginOkClick` | procedure public | TODO |
| 19 | 546 | `DLoginNewClick` | procedure public | TODO |
| 20 | 547 | `DLoginChgPwClick` | procedure public | TODO |
| 21 | 548 | `DNewAccountOkClick` | procedure public | TODO |
| 22 | 549 | `DNewAccountCloseClick` | procedure public | TODO |
| 23 | 550 | `DChgpwOkClick` | procedure public | TODO |
| 24 | 551 | `DChgpwCancelClick` | procedure public | TODO |
| 25 | 552 | `DSWWeaponClick` | procedure public | TODO |
| 26 | 553 | `DCloseBagClick` | procedure public | TODO |
| 27 | 554 | `DBelt1Click` | procedure public | TODO |
| 28 | 555 | `DStateWinClick` | procedure public | TODO |
| 29 | 556 | `DBelt1MouseMove` | procedure public | TODO |
| 30 | 557 | `DBelt1MouseDown` | procedure public | TODO |
| 31 | 558 | `DMerchantDlgCloseClick` | procedure public | TODO |
| 32 | 559 | `DMerchantDlgClick` | procedure public | TODO |
| 33 | 560 | `DMenuCloseClick` | procedure public | TODO |
| 34 | 561 | `DMenuDlgDirectPaint` | procedure public | TODO |
| 35 | 562 | `DMenuDlgClick` | procedure public | TODO |
| 36 | 563 | `DMenuDlgMouseMove` | procedure public | TODO |
| 37 | 564 | `DSellDlgCloseClick` | procedure public | TODO |
| 38 | 565 | `DSellDlgSpotClick` | procedure public | TODO |
| 39 | 566 | `DSellDlgSpotDirectPaint` | procedure public | TODO |
| 40 | 567 | `DSellDlgSpotMouseMove` | procedure public | TODO |
| 41 | 568 | `DSellDlgOkClick` | procedure public | TODO |
| 42 | 569 | `DMenuBuyClick` | procedure public | TODO |
| 43 | 570 | `DMenuPrevClick` | procedure public | TODO |
| 44 | 571 | `DMenuNextClick` | procedure public | TODO |
| 45 | 572 | `DGoldClick` | procedure public | TODO |
| 46 | 573 | `DSWLightDirectPaint` | procedure public | TODO |
| 47 | 574 | `DBackgroundMouseDown` | procedure public | TODO |
| 48 | 575 | `DStateWinMouseMove` | procedure public | TODO |
| 49 | 576 | `DLoginNewClickSound` | procedure public | TODO |
| 50 | 577 | `DStMag1Click` | procedure public | TODO |
| 51 | 578 | `DStMag1MouseDown` | procedure public | TODO |
| 52 | 579 | `DStMag1MouseUp` | procedure public | TODO |
| 53 | 582 | `DKsOkClick` | procedure public | TODO |
| 54 | 583 | `DDealOkClick` | procedure public | TODO |
| 55 | 584 | `DDealCloseClick` | procedure public | TODO |
| 56 | 585 | `DBotTradeClick` | procedure public | TODO |
| 57 | 586 | `BotChallengeClick` | procedure public | TODO |
| 58 | 587 | `DDealRemoteDlgDirectPaint` | procedure public | TODO |
| 59 | 588 | `DDealDlgDirectPaint` | procedure public | TODO |
| 60 | 589 | `DDGridGridSelect` | procedure public | TODO |
| 61 | 590 | `DDGridGridPaint` | procedure public | TODO |
| 62 | 591 | `DDGridGridMouseMove` | procedure public | TODO |
| 63 | 592 | `DDRGridGridPaint` | procedure public | TODO |
| 64 | 593 | `DDRGridGridMouseMove` | procedure public | TODO |
| 65 | 594 | `DDGoldClick` | procedure public | TODO |
| 66 | 595 | `DUserState1MouseMove` | procedure public | TODO |
| 67 | 596 | `DCloseUS1Click` | procedure public | TODO |
| 68 | 597 | `DNecklaceUS1DirectPaint` | procedure public | TODO |
| 69 | 598 | `DGuildDlgDirectPaint` | procedure public | TODO |
| 70 | 599 | `DGDUpClick` | procedure public | TODO |
| 71 | 600 | `DGDDownClick` | procedure public | TODO |
| 72 | 601 | `DGDCloseClick` | procedure public | TODO |
| 73 | 602 | `DGDHomeClick` | procedure public | TODO |
| 74 | 603 | `DGDListClick` | procedure public | TODO |
| 75 | 604 | `DGDAddMemClick` | procedure public | TODO |
| 76 | 605 | `DGDDelMemClick` | procedure public | TODO |
| 77 | 606 | `DGDEditNoticeClick` | procedure public | TODO |
| 78 | 607 | `DGDEditGradeClick` | procedure public | TODO |
| 79 | 608 | `DNewGuildDlgCloseClick` | procedure public | TODO |
| 80 | 609 | `DNewGuildNoticeClick` | procedure public | TODO |
| 81 | 610 | `DGuildEditNoticeDirectPaint` | procedure public | TODO |
| 82 | 611 | `DGDChatClick` | procedure public | TODO |
| 83 | 612 | `DAdjustAbilCloseClick` | procedure public | TODO |
| 84 | 613 | `DBotPlusAbilClick` | procedure public | TODO |
| 85 | 614 | `DAdjustAbilOkClick` | procedure public | TODO |
| 86 | 615 | `DBotPlusAbilDirectPaint` | procedure public | TODO |
| 87 | 616 | `DAdjustAbilityMouseMove` | procedure public | TODO |
| 88 | 617 | `DUserState1MouseDown` | procedure public | TODO |
| 89 | 618 | `DChgGamePwdDirectPaint` | procedure public | TODO |
| 90 | 619 | `DscSelect1InRealArea` | procedure public | TODO |
| 91 | 620 | `DCreateChrDirectPaint` | procedure public | TODO |
| 92 | 621 | `DItemBagMouseMove` | procedure public | TODO |
| 93 | 622 | `DBottomMouseMove` | procedure public | TODO |
| 94 | 623 | `DNewAccountCancelClick` | procedure public | TODO |
| 95 | 624 | `DHeroM2ShopDlgShowOnClick` | procedure public | TODO |
| 96 | 625 | `DShopDlgDirectPaint` | procedure public | TODO |
| 97 | 626 | `DButtonShopPrevClick` | procedure public | TODO |
| 98 | 627 | `DButtonShopNextClick` | procedure public | TODO |
| 99 | 628 | `DBotRankingHomeClick` | procedure public | TODO |
| 100 | 629 | `DBotRankingUpClick` | procedure public | TODO |
| 101 | 630 | `DBotRankingDownClick` | procedure public | TODO |
| 102 | 631 | `DBotRankingLastClick` | procedure public | TODO |
| 103 | 632 | `DButtonShopBuyClick` | procedure public | TODO |
| 104 | 633 | `DButtonShopBuyGiveClick` | procedure public | TODO |
| 105 | 634 | `MinMapLevelChange` | procedure public | TODO |
| 106 | 635 | `DMinMapDlgShow` | procedure public | TODO |
| 107 | 636 | `DMinMapDlgHide` | procedure public | TODO |
| 108 | 637 | `DMinMapDlgResize` | procedure public | TODO |
| 109 | 638 | `DMinMapDlgMouseEnter` | procedure public | TODO |
| 110 | 639 | `DMinMapDlgMouseLeave` | procedure public | TODO |
| 111 | 640 | `DMinMapDlgMouseMove` | procedure public | TODO |
| 112 | 641 | `DMinMapDlgMouseUP` | procedure public | TODO |
| 113 | 642 | `DMinMapDlgClick` | procedure public | TODO |
| 114 | 643 | `DscStartDirectPaint` | procedure public | TODO |
| 115 | 644 | `DDiceDlgDirectPaint` | procedure public | TODO |
| 116 | 645 | `OnGetImage` | procedure public | TODO |
| 117 | 646 | `HideAllControls` | procedure public | TODO |
| 118 | 647 | `RestoreHideControls` | procedure public | TODO |
| 119 | 648 | `DealItemReturnBag` | procedure public | TODO |
| 120 | 649 | `DealZeroGold` | procedure public | TODO |
| 121 | 650 | `OpenSoundOption` | procedure public | TODO |
| 122 | 653 | `FindActiveControl` | function public | TODO |
| 123 | 654 | `AddNpcMemo` | procedure public | TODO |
| 124 | 655 | `LoadJsonControl` | procedure public | TODO |
| 125 | 656 | `DBotRankClick` | procedure public | TODO |
| 126 | 657 | `DBotWhisperClick` | procedure public | TODO |
| 127 | 658 | `DBotHorseClick` | procedure public | TODO |
| 128 | 659 | `DOptionClick` | procedure public | TODO |
| 129 | 660 | `DWhisperDlgCloseClick` | procedure public | TODO |
| 130 | 661 | `DRankingDlgCloseClick` | procedure public | TODO |
| 131 | 662 | `DShopDlgCloseClick` | procedure public | TODO |
| 132 | 663 | `DMissionDlgClick` | procedure public | TODO |
| 133 | 664 | `DMissionDlgCloseClick` | procedure public | TODO |
| 134 | 665 | `DGrpDlgCloseClick` | procedure public | TODO |
| 135 | 666 | `DBotGroupClick` | procedure public | TODO |
| 136 | 667 | `DGrpAllowGroupClick` | procedure public | TODO |
| 137 | 668 | `DBotGroupMouseDown` | procedure public | TODO |
| 138 | 671 | `DBotExitClick` | procedure public | TODO |
| 139 | 672 | `DGDAllyClick` | procedure public | TODO |
| 140 | 673 | `DGDBreakAllyClick` | procedure public | TODO |
| 141 | 674 | `DButtonFriendClick` | procedure public | TODO |
| 142 | 675 | `DBotRankingClick` | procedure public | TODO |
| 143 | 676 | `DBotRankingCloseClick` | procedure public | TODO |
| 144 | 677 | `DBotFriendClick` | procedure public | TODO |
| 145 | 678 | `DFrdCloseClick` | procedure public | TODO |
| 146 | 679 | `DChgGamePwdCloseClick` | procedure public | TODO |
| 147 | 680 | `DGameGoldDealDlgMouseMove` | procedure public | TODO |
| 148 | 681 | `DGameGoldDealMenuDlgPaint` | procedure public | TODO |
| 149 | 682 | `DGameGoldDealMenuDlgMouseMove` | procedure public | TODO |
| 150 | 683 | `DGameGoldDealGridGridPaint` | procedure public | TODO |
| 151 | 684 | `DGameGoldDealGridGridSelect` | procedure public | TODO |
| 152 | 685 | `DGameGoldDealGridGridMouseMove` | procedure public | TODO |
| 153 | 686 | `DGameGoldDealDlgCloseClick` | procedure public | TODO |
| 154 | 687 | `DGameGoldDealCancelClick` | procedure public | TODO |
| 155 | 688 | `DGameGoldDealMenuDlgCloseClick` | procedure public | TODO |
| 156 | 689 | `DBuyGameGoldDealItemOKClick` | procedure public | TODO |
| 157 | 690 | `DBuyGameGoldDealItemCancelClick` | procedure public | TODO |
| 158 | 691 | `DGameGoldDealItemCancelClick` | procedure public | TODO |
| 159 | 692 | `DBotFuncClick` | procedure public | TODO |
| 160 | 693 | `DBotFuncMouseMove` | procedure public | TODO |
| 161 | 694 | `DStMagMouseMove` | procedure public | TODO |
| 162 | 695 | `DMissionMerchantDlgClick` | procedure public | TODO |
| 163 | 696 | `DNPC_IMG_PAINT` | procedure public | TODO |
| 164 | 697 | `DNPC_IMGEX_PAINT` | procedure public | TODO |
| 165 | 698 | `DNPC_IMGNUM_PAINT` | procedure public | TODO |
| 166 | 699 | `DNPC_PLAYIMG_PAINT` | procedure public | TODO |
| 167 | 700 | `DNPC_IMGPAY_PAINT` | procedure public | TODO |
| 168 | 701 | `Close` | procedure public | TODO |
| 169 | 702 | `MakeShareControlAddrList` | function public | TODO |
| 170 | 703 | `LoadShareFromStream` | procedure public | TODO |
| 171 | 704 | `OpenDSelectChrDlg` | procedure public | TODO |
| 172 | 705 | `Initialize` | procedure public | TODO |
| 173 | 706 | `LoadFromStream` | procedure public | TODO |
| 174 | 707 | `MakeControlAddrList` | function public | TODO |
| 175 | 708 | `GetMissionTreeView` | function public | TODO |
| 176 | 709 | `GetMissionActivePageIndex` | function public | TODO |
| 177 | 710 | `SetMissionActivePageIndex` | procedure public | TODO |
| 178 | 711 | `MissionPageClear` | procedure public | TODO |
| 179 | 712 | `LoginPasswdOK` | procedure public | TODO |
| 180 | 713 | `OpenDLoginDlg` | procedure public | TODO |
| 181 | 714 | `CloseDLoginDlg` | procedure public | TODO |
| 182 | 715 | `OpenDDoorDlg` | procedure public | TODO |
| 183 | 716 | `CloseDDoorDlg` | procedure public | TODO |
| 184 | 718 | `OpenDRealNameDlg` | procedure public | TODO |
| 185 | 719 | `CloseDRealNameDlg` | procedure public | TODO |
| 186 | 721 | `OpenDGetPwdBackDlg` | procedure public | TODO |
| 187 | 722 | `CloseDGetPwdBackDlg` | procedure public | TODO |
| 188 | 724 | `OpenDRegAccount` | procedure public | TODO |
| 189 | 725 | `CloseDRegAccount` | procedure public | TODO |
| 190 | 727 | `OpenDBindPhone` | procedure public | TODO |
| 191 | 728 | `CloseDBindPhone` | procedure public | TODO |
| 192 | 729 | `CheckUserEntrys` | function public | TODO |
| 193 | 730 | `NewIdRetry` | procedure public | TODO |
| 194 | 731 | `NewAccountOk` | procedure public | TODO |
| 195 | 732 | `OpenDNewAccountDlg` | procedure public | TODO |
| 196 | 733 | `CloseDNewAccountDlg` | procedure public | TODO |
| 197 | 734 | `ChangePassWordOK` | procedure public | TODO |
| 198 | 735 | `OpenDChgPwDlg` | procedure public | TODO |
| 199 | 736 | `CloseDChgPwDlg` | procedure public | TODO |
| 200 | 737 | `OpenDSelServerDlg` | procedure public | TODO |
| 201 | 738 | `CloseDSelServerDlg` | procedure public | TODO |
| 202 | 739 | `CloseDSelectChrDlg` | procedure public | TODO |
| 203 | 740 | `OpenDCreateChrDlg` | procedure public | TODO |
| 204 | 741 | `CloseDCreateChrDlg` | procedure public | TODO |
| 205 | 742 | `MakeNewChar` | procedure public | TODO |
| 206 | 743 | `OpenDMerchantDlg` | procedure public | TODO |
| 207 | 744 | `CloseDMerchantDlg` | procedure public | TODO |
| 208 | 745 | `RestoreDMerchantDlg` | procedure public | TODO |
| 209 | 746 | `OpenDMenuDlg` | procedure public | TODO |
| 210 | 747 | `CloseDMenuDlg` | procedure public | TODO |
| 211 | 748 | `OpenTradingMarketItemsDlg` | procedure public | TODO |
| 212 | 749 | `RecallTradingSellPrices` | procedure public | TODO |
| 213 | 750 | `CheckDStorageViewDlgShow` | function public | TODO |
| 214 | 751 | `OpenDStorageViewDlg` | procedure public | TODO |
| 215 | 752 | `RefrshDStorageViewDlgText` | procedure public | DONE |
| 216 | 753 | `RefrshDStorageOpen` | procedure public | TODO |
| 217 | 754 | `CheckDTradingMarketDlgVisible` | function public | TODO |
| 218 | 755 | `OpenDItemBagDlg` | procedure public | TODO |
| 219 | 756 | `CloseDItemBagDlg` | procedure public | TODO |
| 220 | 757 | `OpenDAuctionDlg` | procedure public | TODO |
| 221 | 758 | `CloseDAuctionDlg` | procedure public | TODO |
| 222 | 759 | `CloseDAuctionBuyDlg` | procedure public | TODO |
| 223 | 760 | `CloseDAuctionSellDlg` | procedure public | TODO |
| 224 | 761 | `OpenDAuctionBroadcastDlg` | procedure public | TODO |
| 225 | 762 | `CloseDAuctionBroadcastDlg` | procedure public | TODO |
| 226 | 763 | `OpenSellPlayDlg` | procedure public | TODO |
| 227 | 764 | `OpenDelSellPlayDlg` | procedure public | TODO |
| 228 | 765 | `OpenSellPlayerShopDlg` | procedure public | TODO |
| 229 | 766 | `SetSellPlayerShopItemPageInfo` | procedure public | TODO |
| 230 | 767 | `OpenViewSellPlayerInfo` | procedure public | TODO |
| 231 | 768 | `OpenViewSellPlayerBagItems` | procedure public | TODO |
| 232 | 769 | `OpenViewSellPlayerStorage` | procedure public | TODO |
| 233 | 770 | `SellPlayerAbilChange` | procedure public | TODO |
| 234 | 771 | `ShowSellPlayerOtherInfo` | procedure public | TODO |
| 235 | 772 | `ClearSellPlayerOtherInfo` | procedure public | TODO |
| 236 | 773 | `OpenDSellDlg` | procedure public | TODO |
| 237 | 774 | `CloseDSellDlg` | procedure public | TODO |
| 238 | 775 | `OpenDDealDlg` | procedure public | TODO |
| 239 | 776 | `CloseDDealDlg` | procedure public | TODO |
| 240 | 777 | `OpenDDealRemoteDlg` | procedure public | TODO |
| 241 | 778 | `CloseDDealRemoteDlg` | procedure public | TODO |
| 242 | 779 | `OpenDStateWinDlg` | procedure public | TODO |
| 243 | 780 | `CloseDStateWinDlg` | procedure public | TODO |
| 244 | 781 | `OpenDShopDlg` | procedure public | TODO |
| 245 | 782 | `CloseDShopDlg` | procedure public | TODO |
| 246 | 783 | `OpenDUserState1Dlg` | procedure public | TODO |
| 247 | 784 | `CloseDUserState1Dlg` | procedure public | TODO |
| 248 | 785 | `OpenDFriendDlg` | procedure public | TODO |
| 249 | 786 | `CloseDFriendDlg` | procedure public | TODO |
| 250 | 787 | `OpenDGroupDlg` | procedure public | TODO |
| 251 | 788 | `CloseDGroupDlg` | procedure public | TODO |
| 252 | 789 | `OpenDWhisperDlg` | procedure public | TODO |
| 253 | 790 | `CloseDWhisperDlg` | procedure public | TODO |
| 254 | 791 | `OpenDRankingDlg` | procedure public | TODO |
| 255 | 792 | `CloseDRankingDlg` | procedure public | TODO |
| 256 | 793 | `OpenDGameGoldDealDlg` | procedure public | TODO |
| 257 | 794 | `CloseDGameGoldDealDlg` | procedure public | TODO |
| 258 | 795 | `OpenDGameGoldDealMenuDlg` | procedure public | TODO |
| 259 | 796 | `CloseDGameGoldDealMenuDlg` | procedure public | TODO |
| 260 | 797 | `OpenDGuildDlg` | procedure public | TODO |
| 261 | 798 | `CloseDGuildDlg` | procedure public | TODO |
| 262 | 799 | `DGuildChangeRankNo` | procedure public | TODO |
| 263 | 800 | `UpdateGuildJoinCondition` | procedure public | DONE |
| 264 | 801 | `RefreshDGuildManageListViewWJ` | procedure public | TODO |
| 265 | 802 | `OpenGuildViewMemeberInfo` | procedure public | DONE |
| 266 | 803 | `OpenGuildEditGradeDlg` | procedure public | TODO |
| 267 | 804 | `OpenDGuildEditNoticeDlg` | procedure public | TODO |
| 268 | 805 | `CloseDGuildEditNoticeDlg` | procedure public | TODO |
| 269 | 806 | `RefreshGuildMembers` | procedure public | TODO |
| 270 | 807 | `CloseDGuildDlg_New` | procedure public | TODO |
| 271 | 808 | `OpenDGuildEditNoticeDlg_New` | procedure public | TODO |
| 272 | 809 | `RefreshGuildJoinUserList` | procedure public | TODO |
| 273 | 810 | `DoSetGuildWantAddGuildName` | procedure public | TODO |
| 274 | 811 | `OpenDGuildApplyDlg` | procedure public | TODO |
| 275 | 812 | `CloseDGuildApplyDlg` | procedure public | TODO |
| 276 | 813 | `OpenDGuildNoticeDlg` | procedure public | TODO |
| 277 | 814 | `CloseDGuildNoticeDlg` | procedure public | TODO |
| 278 | 815 | `OpenDMissionDlg` | procedure public | TODO |
| 279 | 816 | `CloseDMissionDlg` | procedure public | TODO |
| 280 | 817 | `OpenDAdjustAbilityDlg` | procedure public | TODO |
| 281 | 818 | `CloseDAdjustAbilityDlg` | procedure public | TODO |
| 282 | 819 | `IsDAdjustAbilityDlgShow` | function public | TODO |
| 283 | 820 | `OpenDHeroStateDlg` | procedure public | TODO |
| 284 | 821 | `CloseDHeroStateDlg` | procedure public | TODO |
| 285 | 822 | `OpenDHeroItemBagDlg` | procedure public | TODO |
| 286 | 823 | `CloseDHeroItemBagDlg` | procedure public | TODO |
| 287 | 824 | `OpenDHeroStateWinDlg` | procedure public | TODO |
| 288 | 825 | `CloseDHeroStateWinDlg` | procedure public | TODO |
| 289 | 826 | `SetHeroBagCount` | procedure public | TODO |
| 290 | 827 | `CloseDHeroJewelryBoxDlg` | procedure public | DONE |
| 291 | 828 | `CloseDHeroGodBlessDlg` | procedure public | DONE |
| 292 | 829 | `OpenDDeleteHumanDlg` | procedure public | TODO |
| 293 | 830 | `CloseDDeleteHumanDlg` | procedure public | TODO |
| 294 | 831 | `OpenMyStatus` | procedure public | TODO |
| 295 | 832 | `OpenMyMagic` | procedure public | TODO |
| 296 | 833 | `AddDDrinkWineDlgSay` | procedure public | TODO |
| 297 | 834 | `OpenDPleaseDrinkWineDlg` | procedure public | TODO |
| 298 | 835 | `CloseDPleaseDrinkWineDlg` | procedure public | TODO |
| 299 | 836 | `OpenDGuessfingerDlg` | procedure public | TODO |
| 300 | 837 | `CloseDGuessfingerDlg` | procedure public | TODO |
| 301 | 838 | `OpenDMakeWineDlg` | procedure public | TODO |
| 302 | 839 | `CloseDMakeWineDlg` | procedure public | TODO |
| 303 | 840 | `OpenDMakeMedicinalWineDlg` | procedure public | TODO |
| 304 | 841 | `CloseDMakeMedicinalWineDlg` | procedure public | TODO |
| 305 | 842 | `OpenDStorageHeroInfoDlg` | procedure public | TODO |
| 306 | 843 | `CloseDStorageHeroInfoDlg` | procedure public | TODO |
| 307 | 846 | `OpenDHeroAppraisalInfoDlg` | procedure public | TODO |
| 308 | 847 | `CloseDHeroAppraisalInfoDlg` | procedure public | TODO |
| 309 | 848 | `OpenDHeroAppraisalDlg` | procedure public | TODO |
| 310 | 849 | `CloseDHeroAppraisalDlg` | procedure public | TODO |
| 311 | 851 | `OpenDHeroAutoPracticeDlg` | procedure public | TODO |
| 312 | 852 | `CloseDHeroAutoPracticeDlg` | procedure public | TODO |
| 313 | 853 | `ShowCollectProgressBarDlg` | procedure public | TODO |
| 314 | 854 | `CloseCollectProgressBarDlg` | procedure public | TODO |
| 315 | 855 | `BringToFrontCollectProgressBarDlg` | procedure public | TODO |
| 316 | 856 | `ShowProgressBarDlg` | procedure public | TODO |
| 317 | 857 | `CloseProgressBarDlg` | procedure public | TODO |
| 318 | 858 | `ShowSayItemDlg` | procedure public | TODO |
| 319 | 859 | `CloseSayItemDlg` | procedure public | TODO |
| 320 | 860 | `ShowDGJPointsDlg` | procedure public | TODO |
| 321 | 861 | `IsDGJPointsShow` | function public | TODO |
| 322 | 862 | `GetUserStateShieldRect` | function public | TODO |
| 323 | 863 | `DMessageDlg` | function public | TODO |
| 324 | 864 | `DMessageDiceDlg` | function public | TODO |
| 325 | 865 | `DMessageLoadDataDlg` | function public | TODO |
| 326 | 866 | `DMessageNoticeDlg` | function public | TODO |
| 327 | 867 | `ShowMDlg` | procedure public | TODO |
| 328 | 868 | `ShowGorupJoinDlg` | procedure public | TODO |
| 329 | 869 | `ResetMenuDlg` | procedure public | TODO |
| 330 | 870 | `CloseMDlg` | procedure public | TODO |
| 331 | 871 | `ToggleShowGroupDlg` | procedure public | TODO |
| 332 | 872 | `ViewBottomBox` | procedure public | TODO |
| 333 | 873 | `MySelfAbilChange` | procedure public | TODO |
| 334 | 874 | `MyHeroAbilChange` | procedure public | TODO |
| 335 | 875 | `SetMagicKeyDlg` | function public | TODO |
| 336 | 876 | `CloseDKeySelDlg` | procedure public | TODO |
| 337 | 877 | `RefLoadConfig` | procedure public | TODO |
| 338 | 878 | `OpenUserState` | procedure public | TODO |
| 339 | 879 | `AttactkModeChange` | procedure public | TODO |
| 340 | 880 | `OpenDChallengeDlg` | procedure public | TODO |
| 341 | 881 | `CloseDChallengeDlg` | procedure public | TODO |
| 342 | 882 | `RefreshChallengeDlg` | procedure public | TODO |
| 343 | 883 | `OpenDUpgradeDlg` | procedure public | TODO |
| 344 | 884 | `CloseDUpgradeDlg` | procedure public | TODO |
| 345 | 885 | `OpenDRandomCodeDlg` | procedure public | TODO |
| 346 | 886 | `CloseDRandomCodeDlg` | procedure public | TODO |
| 347 | 887 | `ReInputDRandomCode` | procedure public | TODO |
| 348 | 888 | `NpcItemButtonDirectPaint` | procedure public | TODO |
| 349 | 889 | `NpcItemButtonMouseMove` | procedure public | TODO |
| 350 | 890 | `NpcUserItemButtonDirectPaint` | procedure public | TODO |
| 351 | 891 | `NpcUserItemButtonMouseMove` | procedure public | TODO |
| 352 | 894 | `NpcLabelMouseMove` | procedure public | TODO |
| 353 | 897 | `NpcButtonMouseMove` | procedure public | TODO |
| 354 | 900 | `ItemBoxButtonClick` | procedure public | TODO |
| 355 | 901 | `ItemBoxButtonMouseMove` | procedure public | TODO |
| 356 | 902 | `ItemBoxButtonStartSubDirectPaint` | procedure public | TODO |
| 357 | 903 | `ProgressButtonStartSubDirectPaint` | procedure public | TODO |
| 358 | 904 | `DCloseStateClick` | procedure public | TODO |
| 359 | 905 | `DSSrvCloseClick` | procedure public | TODO |
| 360 | 906 | `DOpenShopClick` | procedure public | TODO |
| 361 | 907 | `DMyHeroStateClick` | procedure public | TODO |
| 362 | 908 | `DMyHeroStateCloseClick` | procedure public | TODO |
| 363 | 909 | `DMyHeroBagClick` | procedure public | TODO |
| 364 | 910 | `DMyHeroBagCloseClick` | procedure public | TODO |
| 365 | 911 | `LabelDStateWinCharNameClick` | procedure public | TODO |
| 366 | 912 | `CancelItemMoving` | procedure public | TODO |
| 367 | 913 | `CancelMagicMoving` | procedure public | TODO |
| 368 | 914 | `DropMovingItem` | procedure public | TODO |
| 369 | 915 | `SoldOutGoods` | procedure public | TODO |
| 370 | 916 | `DelStorageItem` | procedure public | TODO |
| 371 | 917 | `GetMouseItemInfo` | procedure public | TODO |
| 372 | 918 | `GetMouseItemInfo` | procedure public | TODO |
| 373 | 919 | `ShowMouseItemInfo` | procedure public | TODO |
| 374 | 920 | `ShowMouseItemInfo` | procedure public | TODO |
| 375 | 921 | `GetMouseItemInfoWindow` | procedure public | TODO |
| 376 | 922 | `GetMouseFengHaoItemInfoWindow` | procedure public | TODO |
| 377 | 923 | `GetTzItemHintWindow` | function public | TODO |
| 378 | 924 | `GetTzItemHintWindowEx` | function public | TODO |
| 379 | 925 | `ClearMerchantSay` | procedure public | TODO |
| 380 | 926 | `ClearMerchantSay` | procedure public | TODO |
| 381 | 927 | `AddGuildChat` | procedure public | TODO |
| 382 | 928 | `AddMissionMemo1` | procedure public | TODO |
| 383 | 929 | `AddMissionMemo2` | procedure public | TODO |
| 384 | 932 | `DrawGridItem` | procedure public | TODO |
| 385 | 935 | `DrawGridItemEx` | procedure public | TODO |
| 386 | 936 | `DrawGridStdItem` | procedure public | TODO |
| 387 | 939 | `DrawBodyItem` | procedure public | TODO |
| 388 | 942 | `DrawBodyItemBelowEffect` | procedure public | TODO |
| 389 | 943 | `DrawBodyItemEffect` | procedure public | TODO |
| 390 | 944 | `DrawBodyItemEffect` | procedure public | TODO |
| 391 | 945 | `CheckDGameShopDlgVisible` | function public | TODO |
| 392 | 946 | `OpenDGameShopDlg` | procedure public | TODO |
| 393 | 947 | `CloseDGameShopDlg` | procedure public | TODO |
| 394 | 948 | `OpenDMyShopDlg` | procedure public | TODO |
| 395 | 949 | `CloseDMyShopDlg` | procedure public | TODO |
| 396 | 950 | `OpenDUserShopDlg` | procedure public | TODO |
| 397 | 951 | `CloseDUserShopDlg` | procedure public | TODO |
| 398 | 952 | `CheckDHeroM2ShopDlgVisible` | function public | TODO |
| 399 | 953 | `OpenDHeroM2ShopDlg` | procedure public | TODO |
| 400 | 954 | `CloseDHeroM2ShopDlg` | procedure public | TODO |
| 401 | 957 | `CloseDHeroM2InputPriceDlg` | procedure public | TODO |
| 402 | 958 | `OpenDHeroM2ShopRemoteDlg` | procedure public | TODO |
| 403 | 959 | `CloseDHeroM2ShopRemoteDlg` | procedure public | TODO |
| 404 | 960 | `DBotUserShopClick` | procedure public | TODO |
| 405 | 961 | `DDownHorseClick` | procedure public | TODO |
| 406 | 962 | `DWebClick` | procedure public | TODO |
| 407 | 963 | `DActionLogClick` | procedure public | TODO |
| 408 | 964 | `SetMyShopPageCount` | procedure public | TODO |
| 409 | 965 | `SetGameShopPageCount` | procedure public | TODO |
| 410 | 966 | `SetUserShopItemPageCount` | procedure public | TODO |
| 411 | 967 | `DDeleteHumanDlgCloseClick` | procedure public | TODO |
| 412 | 968 | `DGetBackDeleteHumanClick` | procedure public | TODO |
| 413 | 969 | `DListViewDeleteHumanListItemClick` | procedure public | TODO |
| 414 | 970 | `DSpecialCmdMenuClick` | procedure public | TODO |
| 415 | 971 | `MerchantDlgPaint` | procedure public | TODO |
| 416 | 972 | `DDiceDlgProcess` | procedure public | TODO |
| 417 | 973 | `DChallengeItemReturnBag` | procedure public | TODO |
| 418 | 974 | `ChallengeZeroGold` | procedure public | TODO |
| 419 | 975 | `DChallengeOkClick` | procedure public | TODO |
| 420 | 976 | `DChallengeCloseClick` | procedure public | TODO |
| 421 | 977 | `DDChallengeGoldClick` | procedure public | TODO |
| 422 | 978 | `DChallengeGridGridPaint` | procedure public | TODO |
| 423 | 979 | `DChallengeGridGridSelect` | procedure public | TODO |
| 424 | 980 | `DChallengeGridGridMouseMove` | procedure public | TODO |
| 425 | 981 | `DChallengeRemoteGridGridPaint` | procedure public | TODO |
| 426 | 982 | `DChallengeRemoteGridGridMouseMove` | procedure public | TODO |
| 427 | 983 | `DControlHelpClick` | procedure public | TODO |
| 428 | 984 | `DRandomCodeDlgDirectPaint` | procedure public | TODO |
| 429 | 985 | `DItemBagDirectPaint` | procedure public | TODO |
| 430 | 986 | `DStPageUpClick` | procedure public | TODO |
| 431 | 987 | `DMainBottomCenterHeightChangeQuery` | procedure public | TODO |
| 432 | 988 | `DMainBottomCenterHeightChanged` | procedure public | TODO |
| 433 | 989 | `DMainBottomDlgStartSubPaint` | procedure public | TODO |
| 434 | 990 | `DMagicBallStopPaint` | procedure public | TODO |
| 435 | 991 | `DMagicBallGetHumAbility` | procedure public | TODO |
| 436 | 992 | `OnGetGroupAttackProgress` | procedure public | TODO |
| 437 | 993 | `DStateMemo4DirectPaint` | procedure public | TODO |
| 438 | 994 | `DHeroStateWinDirectPaint` | procedure public | TODO |
| 439 | 995 | `DBotMiniMapClick` | procedure public | TODO |
| 440 | 996 | `DUserState1DirectPaint` | procedure public | TODO |
| 441 | 997 | `DWeaponUS1MouseMove` | procedure public | TODO |
| 442 | 998 | `DMinMapDlgDirectPaint` | procedure public | TODO |
| 443 | 999 | `DSelectChrWindowsPaint` | procedure public | TODO |
| 444 | 1000 | `DSWWeaponMouseMove` | procedure public | TODO |
| 445 | 1001 | `ItemBagDirectPaint` | procedure public | TODO |
| 446 | 1002 | `StPageUpClick` | procedure public | TODO |
| 447 | 1003 | `MainBottomCenterHeightChangeQuery` | procedure public | TODO |
| 448 | 1004 | `MainBottomCenterHeightChanged` | procedure public | TODO |
| 449 | 1005 | `MainBottomDlgStartSubPaint` | procedure public | TODO |
| 450 | 1006 | `MagicBallStopPaint` | procedure public | TODO |
| 451 | 1007 | `StateMemo4DirectPaint` | procedure public | TODO |
| 452 | 1008 | `HeroStateWinDirectPaint` | procedure public | TODO |
| 453 | 1009 | `BotMiniMapClick` | procedure public | TODO |
| 454 | 1010 | `UserState1DirectPaint` | procedure public | TODO |
| 455 | 1011 | `WeaponUS1MouseMove` | procedure public | TODO |
| 456 | 1012 | `MinMapDlgDirectPaint` | procedure public | TODO |
| 457 | 1013 | `SelectChrWindowsPaint` | procedure public | TODO |
| 458 | 1014 | `SWWeaponMouseMove` | procedure public | TODO |
| 459 | 1015 | `OpenDBoxDlg` | procedure public | TODO |
| 460 | 1016 | `CloseDBoxDlg` | procedure public | TODO |
| 461 | 1017 | `OpenDItemBoxDlg` | procedure public | TODO |
| 462 | 1018 | `CloseDItemBoxDlg` | procedure public | TODO |
| 463 | 1019 | `SetGuessfinger` | procedure public | TODO |
| 464 | 1020 | `SetDrinkDrunkValue` | procedure public | TODO |
| 465 | 1022 | `CheckShowRemoveStoneForm` | function public | TODO |
| 466 | 1023 | `ShowRemoveStoneForm` | procedure public | TODO |
| 467 | 1024 | `CheckDMinMapBigDlgMouseDown` | function public | TODO |
| 468 | 1025 | `CheckDMinMapBigDlgVisible` | function public | TODO |
| 469 | 1026 | `OpenDMinMapBigDlg` | procedure public | TODO |
| 470 | 1027 | `CloseDMinMapBigDlg` | procedure public | TODO |
| 471 | 1028 | `CheckDMinMapExDlgVisible` | function public | TODO |
| 472 | 1029 | `CheckDMinMapExShowButton` | function public | TODO |
| 473 | 1030 | `OpenDMinMapExDlg` | procedure public | TODO |
| 474 | 1031 | `CloseDMinMapExDlg` | procedure public | TODO |
| 475 | 1032 | `SelectChrWindowsPaint205` | procedure public | TODO |
| 476 | 1033 | `DStUpgradeMagicButtonClick` | procedure public | TODO |
| 477 | 1034 | `DStHeroUpgradeMagicButtonClick` | procedure public | TODO |
| 478 | 1035 | `DStUpgradeMagicMouseMove` | procedure public | TODO |
| 479 | 1036 | `DHeroStUpgradeMagicMouseMove` | procedure public | TODO |
| 480 | 1037 | `DStateRefurbishLabelClick` | procedure public | TODO |
| 481 | 1038 | `GuildGroupIndex` | property public | TODO |
| 482 | 1041 | `ClearDMissionMemo` | procedure public | TODO |
| 483 | 1042 | `DMouseMoveClearHints` | procedure public | TODO |
| 484 | 1043 | `DNpcScrollBoxMove` | procedure public | TODO |
| 485 | 1044 | `DNpcScrollBoxDown` | procedure public | TODO |
| 486 | 1045 | `DNpcScrollBoxUp` | procedure public | TODO |
| 487 | 1048 | `DLieDragonPaint` | procedure public | TODO |
| 488 | 1050 | `DLieDragonClosePaint` | procedure public | TODO |
| 489 | 1052 | `DLieDragonCloseClick` | procedure public | TODO |
| 490 | 1054 | `DLieDragonNextPagePaint` | procedure public | TODO |
| 491 | 1056 | `DLieDragonPrevPagePaint` | procedure public | TODO |
| 492 | 1058 | `DLieDragonNextPageClick` | procedure public | TODO |
| 493 | 1060 | `DGoToLieDragontPaint` | procedure public | TODO |
| 494 | 1062 | `DGoToLieDragonClick` | procedure public | TODO |
| 495 | 1064 | `DLieDragonNpcPaint` | procedure public | TODO |
| 496 | 1066 | `DLieDragonNpcCloseClick` | procedure public | TODO |
| 497 | 1069 | `DSayItemDlgPaint` | procedure public | TODO |
| 498 | 1070 | `DSayItemDlgCloseClick` | procedure public | TODO |
| 499 | 1071 | `DSayItemDlgMouseDown` | procedure public | TODO |
| 500 | 1072 | `DSayItemDlgMouseMove` | procedure public | TODO |
| 501 | 1073 | `DUpdateStatusDlgPaint` | procedure public | TODO |
| 502 | 1074 | `DUpdateStatusDlgMouseEnter` | procedure public | TODO |
| 503 | 1075 | `DUpdateStatusDlgMouseLeave` | procedure public | TODO |
| 504 | 1076 | `DUpdateStatusDlgDblClick` | procedure public | TODO |
| 505 | 1079 | `UpdateBusinessStatusText` | procedure public | TODO |
| 506 | 1080 | `UpdateShopMoneyInfo` | procedure public | TODO |
| 507 | 1082 | `GetLastHistroySendSay` | function public | DONE |
| 508 | 1083 | `GetPreHistroySendSay` | function public | DONE |
| 509 | 1084 | `GetNextHistroySendSay` | function public | DONE |
| 510 | 1085 | `IsInputChatEdit` | function public | DONE |
| 511 | 1086 | `ShowChatEdit` | procedure public | DONE |
| 512 | 1087 | `HideChatEdit` | procedure public | DONE |
| 513 | 1088 | `FindMagicButton` | function public | DONE |
| 514 | 1089 | `ClearScreenMagicButtons` | procedure public | DONE |
| 515 | 1090 | `AddScreenMagicButton` | procedure public | DONE |
| 516 | 1091 | `DelScreenMagicButton` | procedure public | DONE |
| 517 | 1092 | `DelScreenMagicButton` | procedure public | DONE |
| 518 | 1093 | `SaveMagicButtons` | procedure public | PART |
| 519 | 1094 | `LoadMagicButtons` | procedure public | TODO |
| 520 | 1095 | `SetAuctionPageCount` | procedure public | TODO |
| 521 | 1096 | `RequestAuctionItems` | procedure public | TODO |
| 522 | 1097 | `OpenGameLevelDlg` | procedure public | TODO |
| 523 | 1098 | `CloseGameLevelDlg` | procedure public | TODO |
| 524 | 1099 | `RefreshGuardianLevelStatueMonHP` | procedure public | TODO |
| 525 | 1100 | `SetGuardianLevelBatchNo` | procedure public | TODO |
| 526 | 1101 | `ShowGuardianLevelResult` | procedure public | TODO |
| 527 | 1102 | `RefreshAdjustAbilityValue` | procedure public | TODO |
| 528 | 1103 | `DMessageDlgDeleteUser` | function public | TODO |
| 529 | 1104 | `DCustomButtonClick` | procedure public | TODO |
| 530 | 1105 | `RestoreButton_DItemBagArrange` | procedure public | TODO |
| 531 | 1106 | `ResetBagPageButton` | procedure public | TODO |
| 532 | 1107 | `ClearNewGroupMember` | procedure public | TODO |
| 533 | 1108 | `RefreshNewGroupMember` | procedure public | TODO |
| 534 | 1109 | `ReshowNewGroupMember` | procedure public | TODO |
| 535 | 1110 | `CheckDMinMapDlgExVisible` | function public | TODO |
| 536 | 1111 | `ReSetControl` | procedure public | TODO |
| 537 | 1112 | `OpenDBetterItemDlg` | procedure public | TODO |
| 538 | 1113 | `CloseDBetterItemDlg` | procedure public | TODO |


### 3.5 实现段例程全量清单（含行号与覆盖状态）

> 由 `src/GXX.Client/GUI/Share/FStateExtract.ps1` 抽取；脚本已识别 `(* *)` 与 `{ }` **跨行**块注释，
> 故被注释掉的 `TFrmDlg.DNPC_PLAYIMG_PAINT`（23699-23773）**不计入**例程。

| # | from | to | lines | routine | state |
|---|---|---|---|---|---|
| 1 | 1135 | 1152 | 18 | `WriteOutStr` | DONE |
| 2 | 1154 | 1171 | 18 | `TNpcButton.Create` | DONE |
| 3 | 1173 | 1176 | 4 | `TNpcButton.Destroy` | DONE |
| 4 | 1178 | 1199 | 22 | `TNpcButton.Update` | DONE |
| 5 | 1201 | 1209 | 9 | `TNpcLabel.Create` | DONE |
| 6 | 1211 | 1215 | 5 | `TNpcLabel.Destroy` | DONE |
| 7 | 1217 | 1230 | 14 | `TNpcLabel.Update` | DONE |
| 8 | 1232 | 1240 | 9 | `TCountDownLabel.Create` | DONE |
| 9 | 1242 | 1246 | 5 | `TCountDownLabel.Destroy` | DONE |
| 10 | 1248 | 1271 | 24 | `TCountDownLabel.UpdateCaption` | DONE |
| 11 | 1273 | 1309 | 37 | `TCountDownLabel.Update` | DONE |
| 12 | 1311 | 1322 | 12 | `TImgCountDownButton.Create` | DONE |
| 13 | 1324 | 1328 | 5 | `TImgCountDownButton.Destroy` | DONE |
| 14 | 1330 | 1342 | 13 | `TImgCountDownButton.UpdateCaption` | DONE |
| 15 | 1344 | 1369 | 26 | `TImgCountDownButton.Update` | DONE |
| 16 | 1373 | 1380 | 8 | `TNpcInputEdit.Create` | DONE |
| 17 | 1382 | 1392 | 11 | `TNpcInputEdit.KeyPress` | DONE |
| 18 | 1394 | 1398 | 5 | `TNpcInputEdit.Destroy` | DONE |
| 19 | 1400 | 1408 | 9 | `TNpcScrollBox.Create` | DONE |
| 20 | 1410 | 1414 | 5 | `TNpcScrollBox.Destroy` | DONE |
| 21 | 1417 | 1604 | 188 | `TFrmDlg.Create` | DONE |
| 22 | 1606 | 1642 | 37 | `TFrmDlg.CreateNpcQRButtonFromText` | TODO |
| 23 | 1644 | 1712 | 69 | `TFrmDlg.Destroy` | TODO |
| 24 | 1714 | 1775 | 62 | `TFrmDlg.OnGetImage` | TODO |
| 25 | 1777 | 1862 | 86 | `TFrmDlg.UpDate` | TODO |
| 26 | 1864 | 1889 | 26 | `TFrmDlg.ShowMDlg` | TODO |
| 27 | 1891 | 1895 | 5 | `TFrmDlg.HideAllControls` | TODO |
| 28 | 1897 | 1900 | 4 | `TFrmDlg.RestoreHideControls` | TODO |
| 29 | 1904 | 1948 | 45 | `TFrmDlg.OpenSoundOption` | TODO |
| 30 | 1950 | 2123 | 174 | `TFrmDlg.CancelItemMoving` | TODO |
| 31 | 2125 | 2131 | 7 | `TFrmDlg.CancelMagicMoving` | TODO |
| 32 | 2133 | 2151 | 19 | `TFrmDlg.DropMovingItem` | TODO |
| 33 | 2153 | 2225 | 73 | `TFrmDlg.DBackgroundBackgroundClick` | TODO |
| 34 | 2227 | 2232 | 6 | `TFrmDlg.DBackgroundMouseDown` | TODO |
| 35 | 2236 | 2239 | 4 | `TFrmDlg.DLoginNewClick` | TODO |
| 36 | 2241 | 2244 | 4 | `TFrmDlg.DLoginOkClick` | TODO |
| 37 | 2246 | 2249 | 4 | `TFrmDlg.DLoginCloseClick` | TODO |
| 38 | 2251 | 2254 | 4 | `TFrmDlg.DLoginChgPwClick` | TODO |
| 39 | 2256 | 2266 | 11 | `TFrmDlg.DLoginNewClickSound` | TODO |
| 40 | 2270 | 2273 | 4 | `TFrmDlg.DNewAccountOkClick` | TODO |
| 41 | 2275 | 2278 | 4 | `TFrmDlg.DNewAccountCloseClick` | TODO |
| 42 | 2282 | 2285 | 4 | `TFrmDlg.DChgpwOkClick` | TODO |
| 43 | 2289 | 2292 | 4 | `TFrmDlg.DChgpwCancelClick` | TODO |
| 44 | 2294 | 2298 | 5 | `TFrmDlg.DSSrvCloseClick` | TODO |
| 45 | 2302 | 2314 | 13 | `TFrmDlg.DSWLightDirectPaint` | TODO |
| 46 | 2316 | 2319 | 4 | `TFrmDlg.DStateWinClick` | TODO |
| 47 | 2321 | 2606 | 286 | `TFrmDlg.DSWWeaponClick` | TODO |
| 48 | 2608 | 2615 | 8 | `TFrmDlg.DStateWinMouseMove` | TODO |
| 49 | 2617 | 2738 | 122 | `TFrmDlg.DStMagMouseMove` | TODO |
| 50 | 2740 | 2844 | 105 | `TFrmDlg.DStMag1Click` | TODO |
| 51 | 2846 | 2872 | 27 | `TFrmDlg.DStMag1MouseDown` | TODO |
| 52 | 2874 | 2879 | 6 | `TFrmDlg.DStMag1MouseUp` | TODO |
| 53 | 2883 | 2886 | 4 | `TFrmDlg.DBottomInRealArea` | TODO |
| 54 | 2888 | 2891 | 4 | `TFrmDlg.DBotPlusAbilDirectPaint` | TODO |
| 55 | 2895 | 2913 | 19 | `TFrmDlg.DBelt1DirectPaint` | TODO |
| 56 | 2915 | 2939 | 25 | `TFrmDlg.DBelt1MouseMove` | TODO |
| 57 | 2941 | 2970 | 30 | `TFrmDlg.DBelt1MouseDown` | TODO |
| 58 | 2972 | 3033 | 62 | `TFrmDlg.DBelt1Click` | TODO |
| 59 | 3035 | 3062 | 28 | `TFrmDlg.DBelt1DblClick` | TODO |
| 60 | 3067 | 3363 | 297 | `TFrmDlg.GetTzItemHintWindowEx` | TODO |
| 61 | 3365 | 3633 | 269 | `TFrmDlg.GetTzItemHintWindow` | TODO |
| 62 | 3635 | 3646 | 12 | `TFrmDlg.GetMouseItemInfoWindow` | TODO |
| 63 | 3650 | 3684 | 35 | `GetHitLines` | DONE |
| 64 | 3686 | 3692 | 7 | `TFrmDlg.GetMouseFengHaoItemInfoWindow` | TODO |
| 65 | 3694 | 4189 | 496 | `TFrmDlg.ShowMouseItemInfo` | TODO |
| 66 | 4191 | 4254 | 64 | `TFrmDlg.ShowMouseItemInfo` | TODO |
| 67 | 4257 | 4269 | 13 | `GetJobText` | DONE |
| 68 | 4271 | 4283 | 13 | `GetJobTextEx` | DONE |
| 69 | 4285 | 11022 | 6738 | `TFrmDlg.GetMouseItemInfo` | TODO |
| 70 | 11024 | 11067 | 44 | `TFrmDlg.DItemGridGridMouseMove` | TODO |
| 71 | 11069 | 11642 | 574 | `TFrmDlg.DItemGridGridSelect` | TODO |
| 72 | 11644 | 12172 | 529 | `TFrmDlg.DItemGridDblClick` | TODO |
| 73 | 12174 | 12396 | 223 | `TFrmDlg.DItemGridGridPaint` | TODO |
| 74 | 12398 | 12421 | 24 | `TFrmDlg.DGoldClick` | TODO |
| 75 | 12423 | 12434 | 12 | `TFrmDlg.ClearMerchantSay` | TODO |
| 76 | 12436 | 12455 | 20 | `TFrmDlg.ClearMerchantSay` | TODO |
| 77 | 12457 | 12460 | 4 | `TFrmDlg.MerchantDlgPaint` | TODO |
| 78 | 12464 | 12619 | 156 | `NpcItemButtonPaintLight` | TODO |
| 79 | 12621 | 12793 | 173 | `TFrmDlg.NpcItemButtonDirectPaint` | TODO |
| 80 | 12795 | 12824 | 30 | `TFrmDlg.NpcItemButtonMouseMove` | TODO |
| 81 | 12826 | 13008 | 183 | `TFrmDlg.NpcUserItemButtonDirectPaint` | TODO |
| 82 | 13010 | 13030 | 21 | `TFrmDlg.NpcUserItemButtonMouseMove` | TODO |
| 83 | 13034 | 13088 | 55 | `TFrmDlg.NpcLabelMouseMove` | TODO |
| 84 | 13092 | 13146 | 55 | `TFrmDlg.NpcButtonMouseMove` | TODO |
| 85 | 13148 | 13165 | 18 | `TFrmDlg.FindActiveControl` | TODO |
| 86 | 13167 | 15506 | 2340 | `TFrmDlg.AddNpcMemo` | TODO |
| 87 | 15508 | 15591 | 84 | `TFrmDlg.AddMissionMemo1` | TODO |
| 88 | 15595 | 15616 | 22 | `TFrmDlg.DMissionMerchantDlgClick` | TODO |
| 89 | 15618 | 16519 | 902 | `TFrmDlg.AddMissionMemo2` | TODO |
| 90 | 16521 | 16524 | 4 | `TFrmDlg.DMerchantDlgCloseClick` | TODO |
| 91 | 16528 | 16632 | 105 | `TFrmDlg.DMenuDlgDirectPaint` | TODO |
| 92 | 16636 | 16739 | 104 | `TFrmDlg.DMenuDlgClick` | TODO |
| 93 | 16741 | 16749 | 9 | `TFrmDlg.DMenuDlgMouseMove` | TODO |
| 94 | 16753 | 16810 | 58 | `TFrmDlg.DMenuBuyClick` | TODO |
| 95 | 16812 | 16826 | 15 | `TFrmDlg.DMenuPrevClick` | TODO |
| 96 | 16828 | 16841 | 14 | `TFrmDlg.DMenuNextClick` | TODO |
| 97 | 16843 | 17036 | 194 | `TFrmDlg.SoldOutGoods` | TODO |
| 98 | 17038 | 17065 | 28 | `TFrmDlg.DelStorageItem` | TODO |
| 99 | 17067 | 17144 | 78 | `GetPostTextLabel` | TODO |
| 100 | 17146 | 17218 | 73 | `GetPostTextButton` | TODO |
| 101 | 17220 | 17360 | 141 | `TFrmDlg.DMerchantDlgClick` | TODO |
| 102 | 17362 | 17365 | 4 | `TFrmDlg.DSellDlgCloseClick` | TODO |
| 103 | 17367 | 17404 | 38 | `TFrmDlg.DSellDlgSpotClick` | TODO |
| 104 | 17406 | 17444 | 39 | `TFrmDlg.DSellDlgSpotDirectPaint` | TODO |
| 105 | 17446 | 17449 | 4 | `TFrmDlg.DMenuCloseClick` | TODO |
| 106 | 17451 | 17466 | 16 | `TFrmDlg.DSellDlgSpotMouseMove` | TODO |
| 107 | 17468 | 17504 | 37 | `TFrmDlg.DSellDlgOkClick` | TODO |
| 108 | 17506 | 17509 | 4 | `TFrmDlg.DKsOkClick` | TODO |
| 109 | 17513 | 17531 | 19 | `TFrmDlg.DDealOkClick` | TODO |
| 110 | 17533 | 17539 | 7 | `TFrmDlg.DDealCloseClick` | TODO |
| 111 | 17541 | 17565 | 25 | `TFrmDlg.DDealRemoteDlgDirectPaint` | TODO |
| 112 | 17567 | 17615 | 49 | `TFrmDlg.DDealDlgDirectPaint` | TODO |
| 113 | 17617 | 17624 | 8 | `TFrmDlg.DealItemReturnBag` | TODO |
| 114 | 17626 | 17667 | 42 | `TFrmDlg.DDGridGridSelect` | TODO |
| 115 | 17669 | 17690 | 22 | `TFrmDlg.DDGridGridPaint` | TODO |
| 116 | 17692 | 17711 | 20 | `TFrmDlg.DDGridGridMouseMove` | TODO |
| 117 | 17713 | 17723 | 11 | `TFrmDlg.DDRGridGridPaint` | TODO |
| 118 | 17725 | 17744 | 20 | `TFrmDlg.DDRGridGridMouseMove` | TODO |
| 119 | 17746 | 17752 | 7 | `TFrmDlg.DealZeroGold` | TODO |
| 120 | 17754 | 17791 | 38 | `TFrmDlg.DDGoldClick` | TODO |
| 121 | 17795 | 17798 | 4 | `TFrmDlg.DUserState1MouseDown` | TODO |
| 122 | 17800 | 17804 | 5 | `TFrmDlg.DUserState1MouseMove` | TODO |
| 123 | 17806 | 17809 | 4 | `TFrmDlg.DCloseUS1Click` | TODO |
| 124 | 17811 | 17822 | 12 | `TFrmDlg.DNecklaceUS1DirectPaint` | TODO |
| 125 | 17824 | 17852 | 29 | `TFrmDlg.DGuildDlgDirectPaint` | TODO |
| 126 | 17854 | 17860 | 7 | `TFrmDlg.DGDUpClick` | TODO |
| 127 | 17862 | 17866 | 5 | `TFrmDlg.DGDDownClick` | TODO |
| 128 | 17868 | 17872 | 5 | `TFrmDlg.DGDCloseClick` | TODO |
| 129 | 17874 | 17881 | 8 | `TFrmDlg.DGDHomeClick` | TODO |
| 130 | 17883 | 17890 | 8 | `TFrmDlg.DGDListClick` | TODO |
| 131 | 17892 | 17897 | 6 | `TFrmDlg.DGDAddMemClick` | TODO |
| 132 | 17899 | 17904 | 6 | `TFrmDlg.DGDDelMemClick` | TODO |
| 133 | 17906 | 17910 | 5 | `TFrmDlg.DGDEditNoticeClick` | TODO |
| 134 | 17912 | 17915 | 4 | `TFrmDlg.DNewGuildDlgCloseClick` | TODO |
| 135 | 17917 | 17920 | 4 | `TFrmDlg.DNewGuildNoticeClick` | TODO |
| 136 | 17922 | 17926 | 5 | `TFrmDlg.DGDEditGradeClick` | TODO |
| 137 | 17928 | 17932 | 5 | `TFrmDlg.DGDAllyClick` | TODO |
| 138 | 17934 | 17939 | 6 | `TFrmDlg.DGDBreakAllyClick` | TODO |
| 139 | 17941 | 17951 | 11 | `TFrmDlg.DGuildEditNoticeDirectPaint` | TODO |
| 140 | 17953 | 17964 | 12 | `TFrmDlg.AddGuildChat` | TODO |
| 141 | 17966 | 17975 | 10 | `TFrmDlg.DGDChatClick` | TODO |
| 142 | 17979 | 17983 | 5 | `TFrmDlg.DAdjustAbilCloseClick` | TODO |
| 143 | 17985 | 17989 | 5 | `TFrmDlg.DAdjustAbilOkClick` | TODO |
| 144 | 17991 | 18016 | 26 | `TFrmDlg.DAdjustAbilityMouseMove` | TODO |
| 145 | 18018 | 18021 | 4 | `TFrmDlg.DChgGamePwdCloseClick` | TODO |
| 146 | 18023 | 18026 | 4 | `TFrmDlg.DChgGamePwdDirectPaint` | TODO |
| 147 | 18028 | 18031 | 4 | `TFrmDlg.DscSelect1InRealArea` | TODO |
| 148 | 18033 | 18036 | 4 | `TFrmDlg.DCreateChrDirectPaint` | TODO |
| 149 | 18038 | 18042 | 5 | `TFrmDlg.DItemBagMouseMove` | TODO |
| 150 | 18044 | 18047 | 4 | `TFrmDlg.DBottomMouseMove` | TODO |
| 151 | 18049 | 18052 | 4 | `TFrmDlg.DButtonFriendClick` | TODO |
| 152 | 18054 | 18057 | 4 | `TFrmDlg.DNewAccountCancelClick` | TODO |
| 153 | 18059 | 18089 | 31 | `TFrmDlg.DShopDlgDirectPaint` | TODO |
| 154 | 18091 | 18095 | 5 | `TFrmDlg.DButtonShopPrevClick` | TODO |
| 155 | 18097 | 18102 | 6 | `TFrmDlg.DButtonShopNextClick` | TODO |
| 156 | 18104 | 18132 | 29 | `TFrmDlg.DButtonShopBuyClick` | TODO |
| 157 | 18134 | 18162 | 29 | `TFrmDlg.DButtonShopBuyGiveClick` | TODO |
| 158 | 18166 | 18170 | 5 | `TFrmDlg.DBotRankingHomeClick` | TODO |
| 159 | 18172 | 18176 | 5 | `TFrmDlg.DBotRankingUpClick` | TODO |
| 160 | 18178 | 18183 | 6 | `TFrmDlg.DBotRankingDownClick` | TODO |
| 161 | 18185 | 18195 | 11 | `TFrmDlg.DBotRankingLastClick` | TODO |
| 162 | 18197 | 18200 | 4 | `TFrmDlg.DMinMapDlgShow` | TODO |
| 163 | 18202 | 18205 | 4 | `TFrmDlg.DMinMapDlgHide` | TODO |
| 164 | 18207 | 18210 | 4 | `TFrmDlg.DMinMapDlgResize` | TODO |
| 165 | 18212 | 18215 | 4 | `TFrmDlg.DMinMapDlgMouseEnter` | TODO |
| 166 | 18217 | 18220 | 4 | `TFrmDlg.DMinMapDlgMouseLeave` | TODO |
| 167 | 18222 | 18226 | 5 | `TFrmDlg.DMinMapDlgMouseMove` | TODO |
| 168 | 18228 | 18316 | 89 | `TFrmDlg.DMinMapDlgMouseUp` | TODO |
| 169 | 18318 | 18335 | 18 | `TFrmDlg.MinMapLevelChange` | TODO |
| 170 | 18337 | 18340 | 4 | `TFrmDlg.DMinMapDlgClick` | TODO |
| 171 | 18342 | 18369 | 28 | `TFrmDlg.DscStartDirectPaint` | TODO |
| 172 | 18373 | 18410 | 38 | `TFrmDlg.DGameGoldDealMenuDlgMouseMove` | TODO |
| 173 | 18412 | 18519 | 108 | `TFrmDlg.DGameGoldDealMenuDlgPaint` | TODO |
| 174 | 18521 | 18525 | 5 | `TFrmDlg.DGameGoldDealDlgMouseMove` | TODO |
| 175 | 18527 | 18552 | 26 | `TFrmDlg.DGameGoldDealGridGridMouseMove` | TODO |
| 176 | 18554 | 18580 | 27 | `TFrmDlg.DGameGoldDealGridGridPaint` | TODO |
| 177 | 18582 | 18637 | 56 | `TFrmDlg.DGameGoldDealGridGridSelect` | TODO |
| 178 | 18639 | 18653 | 15 | `TFrmDlg.DGameGoldDealDlgCloseClick` | TODO |
| 179 | 18655 | 18658 | 4 | `TFrmDlg.DGameGoldDealCancelClick` | TODO |
| 180 | 18660 | 18665 | 6 | `TFrmDlg.DGameGoldDealMenuDlgCloseClick` | TODO |
| 181 | 18667 | 18698 | 32 | `TFrmDlg.DBuyGameGoldDealItemOKClick` | TODO |
| 182 | 18700 | 18703 | 4 | `TFrmDlg.DBuyGameGoldDealItemCancelClick` | TODO |
| 183 | 18705 | 18708 | 4 | `TFrmDlg.DGameGoldDealItemCancelClick` | TODO |
| 184 | 18710 | 18779 | 70 | `TFrmDlg.DBotFuncClick` | TODO |
| 185 | 18781 | 18820 | 40 | `TFrmDlg.DBotFuncMouseMove` | TODO |
| 186 | 18822 | 18825 | 4 | `TFrmDlg.DCloseStateClick` | TODO |
| 187 | 18827 | 18830 | 4 | `TFrmDlg.DCloseBagClick` | TODO |
| 188 | 18832 | 18835 | 4 | `TFrmDlg.DBotRankClick` | TODO |
| 189 | 18837 | 18840 | 4 | `TFrmDlg.DBotWhisperClick` | TODO |
| 190 | 18842 | 18845 | 4 | `TFrmDlg.DBotHorseClick` | TODO |
| 191 | 18847 | 18851 | 5 | `TFrmDlg.DOptionClick` | TODO |
| 192 | 18853 | 18856 | 4 | `TFrmDlg.DWhisperDlgCloseClick` | TODO |
| 193 | 18858 | 18861 | 4 | `TFrmDlg.DRankingDlgCloseClick` | TODO |
| 194 | 18863 | 18866 | 4 | `TFrmDlg.DShopDlgCloseClick` | TODO |
| 195 | 18868 | 18871 | 4 | `TFrmDlg.DMissionDlgClick` | TODO |
| 196 | 18873 | 18876 | 4 | `TFrmDlg.DMissionDlgCloseClick` | TODO |
| 197 | 18878 | 18881 | 4 | `TFrmDlg.DOpenShopClick` | TODO |
| 198 | 18883 | 18886 | 4 | `TFrmDlg.DBotRankingClick` | TODO |
| 199 | 18888 | 18891 | 4 | `TFrmDlg.DBotRankingCloseClick` | TODO |
| 200 | 18893 | 18896 | 4 | `TFrmDlg.DBotFriendClick` | TODO |
| 201 | 18898 | 18901 | 4 | `TFrmDlg.DFrdCloseClick` | TODO |
| 202 | 18904 | 18910 | 7 | `TFrmDlg.BotChallengeClick` | TODO |
| 203 | 18912 | 18918 | 7 | `TFrmDlg.DBotTradeClick` | TODO |
| 204 | 18920 | 18928 | 9 | `TFrmDlg.DBotGroupMouseDown` | TODO |
| 205 | 18930 | 18933 | 4 | `TFrmDlg.DBotGroupClick` | TODO |
| 206 | 18935 | 18938 | 4 | `TFrmDlg.DGrpDlgCloseClick` | TODO |
| 207 | 18940 | 18947 | 8 | `TFrmDlg.DGrpAllowGroupClick` | TODO |
| 208 | 18969 | 18982 | 14 | `TFrmDlg.DBotExitClick` | TODO |
| 209 | 18984 | 18987 | 4 | `TFrmDlg.DBotPlusAbilClick` | TODO |
| 210 | 18989 | 18994 | 6 | `TFrmDlg.DMyHeroStateClick` | TODO |
| 211 | 18996 | 18999 | 4 | `TFrmDlg.DMyHeroStateCloseClick` | TODO |
| 212 | 19001 | 19004 | 4 | `TFrmDlg.DMyHeroBagClick` | TODO |
| 213 | 19006 | 19009 | 4 | `TFrmDlg.DMyHeroBagCloseClick` | TODO |
| 214 | 19011 | 19262 | 252 | `TFrmDlg.DrawGridItem` | TODO |
| 215 | 19264 | 19290 | 27 | `TFrmDlg.DrawGridItemEx` | TODO |
| 216 | 19292 | 19510 | 219 | `TFrmDlg.DrawGridStdItem` | TODO |
| 217 | 19512 | 19563 | 52 | `TFrmDlg.DrawBodyItemBelowEffect` | TODO |
| 218 | 19565 | 19868 | 304 | `TFrmDlg.DrawBodyItem` | TODO |
| 219 | 19870 | 19922 | 53 | `TFrmDlg.DrawBodyItemEffect` | TODO |
| 220 | 19924 | 20333 | 410 | `TFrmDlg.DrawBodyItemEffect` | TODO |
| 221 | 20335 | 20354 | 20 | `TFrmDlg.DDiceDlgDirectPaint` | TODO |
| 222 | 20356 | 20365 | 10 | `TFrmDlg.OpenDSelectChrDlg` | TODO |
| 223 | 20367 | 20370 | 4 | `TFrmDlg.AttactkModeChange` | TODO |
| 224 | 20372 | 20393 | 22 | `TFrmDlg.LabelDStateWinCharNameClick` | TODO |
| 225 | 20395 | 20413 | 19 | `TFrmDlg.Close` | TODO |
| 226 | 20415 | 20429 | 15 | `TFrmDlg.MakeShareControlAddrList` | TODO |
| 227 | 20431 | 20563 | 133 | `TFrmDlg.LoadShareFromStream` | TODO |
| 228 | 20565 | 20568 | 4 | `TFrmDlg.DBotUserShopClick` | TODO |
| 229 | 20570 | 20575 | 6 | `TFrmDlg.DDownHorseClick` | TODO |
| 230 | 20577 | 20580 | 4 | `TFrmDlg.DWebClick` | TODO |
| 231 | 20582 | 20585 | 4 | `TFrmDlg.DActionLogClick` | TODO |
| 232 | 20587 | 20590 | 4 | `TFrmDlg.DDeleteHumanDlgCloseClick` | TODO |
| 233 | 20592 | 20596 | 5 | `TFrmDlg.DGetBackDeleteHumanClick` | TODO |
| 234 | 20598 | 20634 | 37 | `TFrmDlg.DListViewDeleteHumanListItemClick` | TODO |
| 235 | 20636 | 20656 | 21 | `TFrmDlg.DSpecialCmdMenuClick` | TODO |
| 236 | 20658 | 20737 | 80 | `TFrmDlg.DDiceDlgProcess` | TODO |
| 237 | 20741 | 20744 | 4 | `TFrmDlg.OpenDUpgradeDlg` | TODO |
| 238 | 20746 | 20749 | 4 | `TFrmDlg.CloseDUpgradeDlg` | TODO |
| 239 | 20751 | 20754 | 4 | `TFrmDlg.OpenDChallengeDlg` | TODO |
| 240 | 20756 | 20773 | 18 | `TFrmDlg.CloseDChallengeDlg` | TODO |
| 241 | 20775 | 20778 | 4 | `TFrmDlg.RefreshChallengeDlg` | TODO |
| 242 | 20780 | 20787 | 8 | `TFrmDlg.DChallengeItemReturnBag` | TODO |
| 243 | 20789 | 20795 | 7 | `TFrmDlg.ChallengeZeroGold` | TODO |
| 244 | 20797 | 20811 | 15 | `TFrmDlg.DChallengeOkClick` | TODO |
| 245 | 20813 | 20819 | 7 | `TFrmDlg.DChallengeCloseClick` | TODO |
| 246 | 20821 | 20859 | 39 | `TFrmDlg.DDChallengeGoldClick` | TODO |
| 247 | 20861 | 20888 | 28 | `TFrmDlg.DChallengeGridGridPaint` | TODO |
| 248 | 20890 | 20982 | 93 | `TFrmDlg.DChallengeGridGridSelect` | TODO |
| 249 | 20984 | 21003 | 20 | `TFrmDlg.DChallengeGridGridMouseMove` | TODO |
| 250 | 21005 | 21031 | 27 | `TFrmDlg.DChallengeRemoteGridGridPaint` | TODO |
| 251 | 21033 | 21052 | 20 | `TFrmDlg.DChallengeRemoteGridGridMouseMove` | TODO |
| 252 | 21054 | 21060 | 7 | `TFrmDlg.DControlHelpClick` | TODO |
| 253 | 21062 | 21065 | 4 | `TFrmDlg.OpenDRandomCodeDlg` | TODO |
| 254 | 21067 | 21070 | 4 | `TFrmDlg.CloseDRandomCodeDlg` | TODO |
| 255 | 21072 | 21075 | 4 | `TFrmDlg.ReInputDRandomCode` | TODO |
| 256 | 21077 | 21086 | 10 | `TFrmDlg.DRandomCodeDlgDirectPaint` | TODO |
| 257 | 21090 | 21093 | 4 | `TFrmDlg.DItemBagDirectPaint` | TODO |
| 258 | 21095 | 21098 | 4 | `TFrmDlg.DStPageUpClick` | TODO |
| 259 | 21100 | 21103 | 4 | `TFrmDlg.DMainBottomCenterHeightChangeQuery` | TODO |
| 260 | 21105 | 21108 | 4 | `TFrmDlg.DMainBottomCenterHeightChanged` | TODO |
| 261 | 21110 | 21113 | 4 | `TFrmDlg.DMainBottomDlgStartSubPaint` | TODO |
| 262 | 21115 | 21118 | 4 | `TFrmDlg.DMagicBallStopPaint` | TODO |
| 263 | 21120 | 21137 | 18 | `TFrmDlg.DMagicBallGetHumAbility` | TODO |
| 264 | 21139 | 21156 | 18 | `TFrmDlg.OnGetGroupAttackProgress` | TODO |
| 265 | 21158 | 21161 | 4 | `TFrmDlg.DStateMemo4DirectPaint` | TODO |
| 266 | 21163 | 21166 | 4 | `TFrmDlg.DHeroStateWinDirectPaint` | TODO |
| 267 | 21168 | 21171 | 4 | `TFrmDlg.DBotMiniMapClick` | TODO |
| 268 | 21173 | 21176 | 4 | `TFrmDlg.DUserState1DirectPaint` | TODO |
| 269 | 21178 | 21181 | 4 | `TFrmDlg.DWeaponUS1MouseMove` | TODO |
| 270 | 21183 | 21186 | 4 | `TFrmDlg.DMinMapDlgDirectPaint` | TODO |
| 271 | 21188 | 21191 | 4 | `TFrmDlg.DSelectChrWindowsPaint` | TODO |
| 272 | 21193 | 21196 | 4 | `TFrmDlg.DSWWeaponMouseMove` | TODO |
| 273 | 21199 | 21405 | 207 | `TFrmDlg.BotMiniMapClick` | TODO |
| 274 | 21407 | 21591 | 185 | `TFrmDlg.UserState1DirectPaint` | TODO |
| 275 | 21593 | 21961 | 369 | `TFrmDlg.WeaponUS1MouseMove` | TODO |
| 276 | 21963 | 22265 | 303 | `TFrmDlg.MinMapDlgDirectPaint` | TODO |
| 277 | 22267 | 22599 | 333 | `TFrmDlg.SelectChrWindowsPaint205` | TODO |
| 278 | 22601 | 22736 | 136 | `TFrmDlg.SelectChrWindowsPaint` | TODO |
| 279 | 22740 | 23116 | 377 | `TFrmDlg.SWWeaponMouseMove` | TODO |
| 280 | 23118 | 23124 | 7 | `TFrmDlg.DHeroM2ShopDlgShowOnClick` | TODO |
| 281 | 23128 | 23198 | 71 | `TFrmDlg.ItemBoxButtonClick` | TODO |
| 282 | 23200 | 23249 | 50 | `TFrmDlg.ItemBoxButtonMouseMove` | TODO |
| 283 | 23251 | 23263 | 13 | `TFrmDlg.ItemBoxButtonStartSubDirectPaint` | TODO |
| 284 | 23265 | 23364 | 100 | `TFrmDlg.ProgressButtonStartSubDirectPaint` | TODO |
| 285 | 23366 | 23374 | 9 | `TFrmDlg.DStHeroUpgradeMagicButtonClick` | TODO |
| 286 | 23376 | 23384 | 9 | `TFrmDlg.DStUpgradeMagicButtonClick` | TODO |
| 287 | 23386 | 23423 | 38 | `TFrmDlg.DStUpgradeMagicMouseMove` | TODO |
| 288 | 23425 | 23462 | 38 | `TFrmDlg.DHeroStUpgradeMagicMouseMove` | TODO |
| 289 | 23464 | 23469 | 6 | `TFrmDlg.DStateRefurbishLabelClick` | TODO |
| 290 | 23471 | 23571 | 101 | `TFrmDlg.DNPC_IMG_PAINT` | TODO |
| 291 | 23573 | 23616 | 44 | `TFrmDlg.DNPC_IMGEX_PAINT` | TODO |
| 292 | 23618 | 23655 | 38 | `TFrmDlg.DNPC_IMGNUM_PAINT` | TODO |
| 293 | 23657 | 23697 | 41 | `TFrmDlg.DNPC_IMGPAY_PAINT` | TODO |
| 294 | 23775 | 23842 | 68 | `TFrmDlg.DNPC_PLAYIMG_PAINT` | TODO |
| 295 | 23844 | 23850 | 7 | `TFrmDlg.RefrshDStorageViewDlgText` | DONE |
| 296 | 23852 | 23857 | 6 | `TFrmDlg.UpdateGuildJoinCondition` | DONE |
| 297 | 23861 | 23868 | 8 | `TNpcItemButton.Create` | DONE |
| 298 | 23870 | 23873 | 4 | `TNpcUserItemButton.Create` | DONE |
| 299 | 23875 | 23878 | 4 | `TFrmDlg.CloseDHeroGodBlessDlg` | DONE |
| 300 | 23880 | 23883 | 4 | `TFrmDlg.CloseDHeroJewelryBoxDlg` | DONE |
| 301 | 23887 | 23893 | 7 | `TGuildGroup.Create` | DONE |
| 302 | 23895 | 23900 | 6 | `TGuildGroup.Destroy` | DONE |
| 303 | 23904 | 23908 | 5 | `TGuildGroupList.Create` | DONE |
| 304 | 23910 | 23915 | 6 | `TGuildGroupList.Destroy` | DONE |
| 305 | 23917 | 23921 | 5 | `TGuildGroupList.Add` | DONE |
| 306 | 23923 | 23931 | 9 | `TGuildGroupList.Clear` | DONE |
| 307 | 23933 | 23944 | 12 | `TGuildGroupList.DeleteGroupByName` | DONE |
| 308 | 23946 | 23955 | 10 | `TGuildGroupList.DeleteGroupByIndex` | DONE |
| 309 | 23957 | 23966 | 10 | `TGuildGroupList.FindGroup` | DONE |
| 310 | 23968 | 23979 | 12 | `TGuildGroupList.FindGroupIndex` | DONE |
| 311 | 23981 | 23984 | 4 | `TGuildGroupList.GetCount:Integer` | DONE |
| 312 | 23986 | 23991 | 6 | `TGuildGroupList.GetGroups` | DONE |
| 313 | 23995 | 23999 | 5 | `TShowGuildList.Create` | DONE |
| 314 | 24001 | 24006 | 6 | `TShowGuildList.Destroy` | DONE |
| 315 | 24008 | 24015 | 8 | `TShowGuildList.Add` | DONE |
| 316 | 24017 | 24025 | 9 | `TShowGuildList.Clear` | DONE |
| 317 | 24027 | 24030 | 4 | `TShowGuildList.GetCount:Integer` | DONE |
| 318 | 24032 | 24037 | 6 | `TShowGuildList.GetShowGuilds` | DONE |
| 319 | 24039 | 24074 | 36 | `TShowGuildList.DoSort` | DONE |
| 320 | 24078 | 24082 | 5 | `TGuildJoinUserList.Create` | DONE |
| 321 | 24084 | 24089 | 6 | `TGuildJoinUserList.Destroy` | DONE |
| 322 | 24091 | 24096 | 6 | `TGuildJoinUserList.Add` | DONE |
| 323 | 24098 | 24106 | 9 | `TGuildJoinUserList.Clear` | DONE |
| 324 | 24108 | 24111 | 4 | `TGuildJoinUserList.GetCount:Integer` | DONE |
| 325 | 24113 | 24118 | 6 | `TGuildJoinUserList.GetJoinUsers` | DONE |
| 326 | 24120 | 24123 | 4 | `TFrmDlg.OpenGuildViewMemeberInfo` | DONE |
| 327 | 24125 | 24131 | 7 | `TFrmDlg.ClearDMissionMemo` | TODO |
| 328 | 24133 | 24137 | 5 | `TFrmDlg.DMouseMoveClearHints` | TODO |
| 329 | 24139 | 24153 | 15 | `TFrmDlg.DNpcScrollBoxMove` | TODO |
| 330 | 24155 | 24167 | 13 | `TFrmDlg.DNpcScrollBoxDown` | TODO |
| 331 | 24169 | 24181 | 13 | `TFrmDlg.DNpcScrollBoxUp` | TODO |
| 332 | 24185 | 24204 | 20 | `TFrmDlg.DLieDragonPaint` | TODO |
| 333 | 24206 | 24209 | 4 | `TFrmDlg.DLieDragonCloseClick` | TODO |
| 334 | 24211 | 24227 | 17 | `TFrmDlg.DLieDragonClosePaint` | TODO |
| 335 | 24229 | 24245 | 17 | `TFrmDlg.DLieDragonPrevPagePaint` | TODO |
| 336 | 24247 | 24263 | 17 | `TFrmDlg.DLieDragonNextPagePaint` | TODO |
| 337 | 24265 | 24285 | 21 | `TFrmDlg.DLieDragonNextPageClick` | TODO |
| 338 | 24287 | 24291 | 5 | `TFrmDlg.DGoToLieDragonClick` | TODO |
| 339 | 24293 | 24309 | 17 | `TFrmDlg.DGoToLieDragontPaint` | TODO |
| 340 | 24311 | 24314 | 4 | `TFrmDlg.DLieDragonNpcCloseClick` | TODO |
| 341 | 24316 | 24329 | 14 | `TFrmDlg.DLieDragonNpcPaint` | TODO |
| 342 | 24331 | 24354 | 24 | `TFrmDlg.DSayItemDlgPaint` | TODO |
| 343 | 24356 | 24359 | 4 | `TFrmDlg.DSayItemDlgCloseClick` | TODO |
| 344 | 24361 | 24365 | 5 | `TFrmDlg.DSayItemDlgMouseDown` | TODO |
| 345 | 24367 | 24372 | 6 | `TFrmDlg.DSayItemDlgMouseMove` | TODO |
| 346 | 24374 | 24383 | 10 | `TFrmDlg.CloseSayItemDlg` | TODO |
| 347 | 24385 | 24398 | 14 | `TFrmDlg.DUpdateStatusDlgPaint` | TODO |
| 348 | 24400 | 24462 | 63 | `TFrmDlg.DUpdateStatusDlgMouseEnter` | TODO |
| 349 | 24464 | 24467 | 4 | `TFrmDlg.DUpdateStatusDlgMouseLeave` | TODO |
| 350 | 24469 | 24472 | 4 | `TFrmDlg.DUpdateStatusDlgDblClick` | TODO |
| 351 | 24474 | 24477 | 4 | `TFrmDlg.GetLastHistroySendSay:string` | DONE |
| 352 | 24479 | 24482 | 4 | `TFrmDlg.GetNextHistroySendSay:string` | DONE |
| 353 | 24484 | 24487 | 4 | `TFrmDlg.GetPreHistroySendSay:string` | DONE |
| 354 | 24489 | 24492 | 4 | `TFrmDlg.IsInputChatEdit:Boolean` | DONE |
| 355 | 24494 | 24500 | 7 | `TFrmDlg.ShowChatEdit` | DONE |
| 356 | 24502 | 24508 | 7 | `TFrmDlg.HideChatEdit` | DONE |
| 357 | 24510 | 24520 | 11 | `TFrmDlg.ClearScreenMagicButtons` | DONE |
| 358 | 24522 | 24555 | 34 | `TFrmDlg.AddScreenMagicButton` | DONE |
| 359 | 24557 | 24572 | 16 | `TFrmDlg.DelScreenMagicButton` | DONE |
| 360 | 24574 | 24589 | 16 | `TFrmDlg.DelScreenMagicButton` | DONE |
| 361 | 24591 | 24604 | 14 | `TFrmDlg.FindMagicButton` | DONE |
| 362 | 24606 | 24614 | 9 | `TFrmDlg.OnMagicButtonClick` | DONE |
| 363 | 24616 | 24622 | 7 | `TFrmDlg.OnMagicButtonDblClick` | DONE |
| 364 | 24624 | 24627 | 4 | `TFrmDlg.OnMagicButtonMove` | DONE |
| 365 | 24629 | 24678 | 50 | `TFrmDlg.OnMagicButtonMouseMove` | TODO |
| 366 | 24680 | 24734 | 55 | `TFrmDlg.SaveMagicButtons` | PART |
| 367 | 24736 | 24799 | 64 | `TFrmDlg.LoadMagicButtons` | TODO |
| 368 | 24803 | 24915 | 113 | `TMagicButton.Paint` | TODO |
| 369 | 24917 | 24922 | 6 | `TFrmDlg.DCustomButtonClick` | TODO |
| 370 | 24924 | 24967 | 44 | `TImgCountDownButton.Paint` | TODO |
| 371 | 24969 | 25134 | 166 | `TFrmDlg.LoadJsonControl` | TODO |
| 372 | 25138 | 25143 | 6 | `TNpcGraphicButton.Create` | DONE |
| 373 | 25145 | 25151 | 7 | `TNpcGraphicButton.Destroy` | DONE |
| 374 | 25153 | 25156 | 4 | `TNpcGraphicButton.SetGraphic` | DONE |
| 375 | 25158 | 25165 | 8 | `TNpcGraphicButton.Paint` | DONE |

- routines: **375** / **23486** lines
- DONE+PART: **75** routines / **968** lines
- TODO: **300** routines / **22518** lines


---

## 4. 覆盖统计（实测）

### 4.1 声明段：**100%**

| 维度 | 原文 | 已覆盖 | 校验方式 |
|---|---|---|---|
| 单元级常量 | 9 | 9 | 清单 SHA-256 + 原文逐条回读 |
| 实现段单元级常量 | 1 | 1 | `FStateGlobal.ConditionOKHitColor` |
| 单元级全局变量 | **1**（`FrmDlg`） | 1 | `FStateGlobal.FrmDlg` |
| 类型 | 24 | 24 | 反射存在性 + 种类 + 基类名 + 原文行号回读 |
| `TFrmDlg` 字段 | 201 | 201 | 反射逐名存在性 |
| `TFrmDlg` 方法/属性 | 538 | 538（**声明面 100%**） | 反射逐名 + 重载计数 + 原文声明文本逐字回读 |

声明面"完整"的定义：**每个成员都真实存在于 C# 类型上**；未移植方法体的成员会 `throw NotSupportedException`（并在消息里带原文行号），不会静默返回默认值 —— 测试 `UnportedMembersThrowInsteadOfSilentlyReturningDefaults` 锁定该性质。

### 4.2 实现段方法体

| 类别 | 例程数 | 行数 |
|---|---|---|
| **DONE**（1:1 已实现，或有测试锁定行为） | 74 | 968 −（PART 计入） |
| **PART**（守卫/收集已实现，INI 主体为接缝） | 1（`SaveMagicButtons`） | 含在上面 |
| **TODO**（声明已在，方法体未移植，调用即抛异常） | 300 | 22,518 |
| 实现段例程合计 | **375** | **23,486** |

> 说明：DONE 例程数多而行数少 —— 本波次优先拿下"短小但被全客户端共用"的公共面
> （4 个容器类 42 个例程、13 个 NPC 控件类、`TFrmDlg.Create` 单块 188 行、
> 聊天/技能图标/纯函数），把 6,700 行的 `GetMouseItemInfo`、2,340 行的 `AddNpcMemo`
> 这类巨型块留给依赖就绪后的下一轮。

**已覆盖行号区间（实现段，方法体级别）**

```
1135-1152   WriteOutStr                      （单元级）
1154-1415   TNpcButton / TNpcLabel / TCountDownLabel / TImgCountDownButton /
            TNpcInputEdit / TNpcScrollBox  的 Create/Destroy/Update/KeyPress/UpdateCaption
1417-1604   TFrmDlg.Create                   （188 行，逐行带原文行号注释）
23844-23859 RefrshDStorageViewDlgText / UpdateGuildJoinCondition
23861-23873 TNpcItemButton.Create / TNpcUserItemButton.Create
23875-23893 CloseDHeroGodBlessDlg / CloseDHeroJewelryBoxDlg（原文空体）
23887-24118 TGuildGroup / TGuildGroupList / TShowGuildList / TGuildJoinUserList 全部方法
24120-24123 OpenGuildViewMemeberInfo
24474-24508 GetLast/GetNext/GetPreHistroySendSay、IsInputChatEdit、ShowChatEdit、HideChatEdit
24510-24627 Clear/Add/Del/Find 屏幕技能图标 + OnMagicButtonClick/DblClick/Move
24680-24734 SaveMagicButtons（守卫与逐项收集；INI 写盘外派接缝）
25138-25165 TNpcGraphicButton.Create/Destroy/SetGraphic/Paint
3650-3684   GetHitLines（单元级）
4257-4283   GetJobText / GetJobTextEx（单元级）
```

**未覆盖行号区间（实现段）**

```
1606-25088 之间除上表以外的全部区间，主要是：
1644-1712   TFrmDlg.Destroy(69)
1714-1775   OnGetImage(62)
1777-1862   UpDate(86)
1864-1902   ShowMDlg / HideAllControls / RestoreHideControls(38)
1904-1948   OpenSoundOption(45)
1950-2225   CancelItemMoving(174) / CancelMagicMoving(7) / DropMovingItem(19) /
            DBackgroundBackgroundClick(73) / DBackgroundMouseDown(8)
2236-2881   一批 DLogin*/DNewAccount*/DStateWin*/DStMag* 事件处理(~330)
2883-3065   DBelt1* 系列(96)
3067-3633   GetTzItemHintWindowEx(297) / GetTzItemHintWindow(269)
3635-3692   GetMouseItemInfoWindow / GetMouseFengHaoItemInfoWindow(21)
3694-4255   ShowMouseItemInfo 两个重载(496 + 65)
4285-11022  GetMouseItemInfo 两个重载(4215 + 2522)   ← 全单元最大两块
11024-12421 DItemGrid* 系列(1332)
12464-13146 NpcItemButtonPaintLight(156) / NpcItemButtonDirectPaint(173) /
            NpcUserItemButtonDirectPaint(183) / NpcLabelMouseMove(57) / NpcButtonMouseMove(55)
13148-13165 FindActiveControl(18)
13167-15506 AddNpcMemo(2340)                          ← 第三大块
15508-16519 AddMissionMemo1(86) / AddMissionMemo2(902)
16528-17360 DMenu*/DMenuBuy/SoldOutGoods/DelStorageItem/DMerchantDlgClick(~700)
17067-17218 GetPostTextLabel(78) / GetPostTextButton(73)
17362-18999 出售/交易/行会/挑战/组队等一批事件处理(~900)
19011-20563 DrawGridItem(252) / DrawGridStdItem(219) / DrawBodyItem(304) /
            DrawBodyItemEffect(53+410) / 绘制与控件地址表(~800)
20565-23842 迷你地图/英雄包裹/任务/NPC 图片绘制等(~2000)
24736-24801 LoadMagicButtons(66)
24803-24917 TMagicButton.Paint(113)
24924-24967 TImgCountDownButton.Paint(44)
24969-25136 LoadJsonControl(168)
```

**未完成方法名清单（下一轮直接接手用）**

按"依赖就绪度"分档，先做 A 档：

*A 档 —— 依赖已就绪，可直接 1:1 落地（无需新接缝）*

```
GetPostTextLabel(17067-17144)  GetPostTextButton(17146-17218)
FindActiveControl(13148-13165)  ClearDMissionMemo(24125-24131)
DMouseMoveClearHints(24133-24137)  DNpcScrollBox* 三支(24139-24183)
MerchantDlgPaint(12457-12462)  DGuildDlgDirectPaint(17824-17852)  AddGuildChat(17953-17964)
DAdjustAbilityMouseMove(17991-18016)  DMagicBallGetHumAbility(21120-21137)
OnGetGroupAttackProgress(21139-21156)  DDiceDlgDirectPaint(20335-20354)
RefrshDStorageViewDlgText 已完成；UpdateGuildJoinCondition 已完成
```

*B 档 —— 需先补 DxControls.pas 的绘制面（`TDxControl` 的 Paint/ControlCount/Control[]/Position）*

```
ClearDMissionMemo(24125)  NpcItemButtonDirectPaint(12621-12793)
NpcUserItemButtonDirectPaint(12826-13008)  NpcLabelMouseMove(13034-13090)
NpcButtonMouseMove(13092-13146)  ItemBoxButton* 三支(23128-23263)
ProgressButtonStartSubDirectPaint(23265-23364)  DNPC_IMG* 五支(23471-23773)
TMagicButton.Paint(24803-24915)  TImgCountDownButton.Paint(24924-24967)
```

*C 档 —— 需先补 MShare.pas 的大面积全局 + TIniFile*

```
SaveMagicButtons 的 INI 主体(24720-24733)  LoadMagicButtons(24736-24801)
GetTzItemHintWindow(Ex)(3067-3633)  GetMouseItemInfo ×2(4285-11022)
ShowMouseItemInfo ×2(3694-4255)  AddNpcMemo(13167-15506)
AddMissionMemo2(15618-16519)  LoadJsonControl(24969-25136)
DrawGridItem/DrawGridStdItem/DrawBodyItem*/DrawBodyItemEffect(19011-20333)
CancelItemMoving/DropMovingItem(1950-2151)  DBelt1* / DItemGrid* 系列
```

*D 档 —— 纯 UI 事件壳（逻辑在 MShare/ClMain 侧，本单元只是解包）*

```
2236-2881 的 DLogin*/DNewAccount*/DChgpw*/DStateWin* 小组件
16528-18999 的 DMenu*/DShop*/DBot*/DGD*/DGrp* 等 ~60 个 4-6 行事件转发
```

---

## 5. 门禁结果

```
dotnet build GXX.slnx -c Debug --nologo
  → 0 error

dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo
  → 已通过! - 失败: 0，通过: 2404，已跳过: 0，总计: 2404
```

新增测试 **128 例**（`GuiShareDeclTests` 33 + `GuiSharePureTests` 95），全部通过。
> 说明：任务书给出的"基线 2308 例"与本工作树实测不符 —— 本车道开工时 `GXX.Client.Tests`
> 实际为 2276 例（清 `bin/obj` 重建后复测一致），加本车道 128 例后为 2404 例。以实测为准。

---

## 6. 发现的原文缺陷与易错点

1. **被块注释掉的整份重复实现（原文 23699-23773）**
   `procedure TFrmDlg.DNPC_PLAYIMG_PAINT` 在原文里**存在两份**：23699 行以 `(*` 开头的
   块注释把第一份（23702-23772）整段注释掉了，活的是 23775-23842 那份。
   **影响**：任何按行号扫描例程的脚本（含本车道第一版）都会把死代码当成活例程、导致行号清单错位。
   本车道已让抽取脚本识别 `(* *)` 与 `{ }` **跨行**块注释并加了守卫测试
   `CommentedOutDuplicateIsNotCountedAsALiveRoutine`。

2. **`GetGodBlessItem` 名字记错**
   原文是 `GetGodBlessItems`（复数），且是 **TFrmDlg 方法内的嵌套函数**（3093 / 3391 两处各自定义），
   **不是**单元级函数。按单元级函数去找会一无所获。

3. **`TBorderStyle` 等 WinForms 概念被硬编码整数**
   `TFrmDlg.Create` 原文用 `BorderStyle := bsSingle`；托管接缝里 `bsSingle = 1`
   已按 Delphi `Forms.TBorderStyle` 的枚举序号保留（`Memo.BorderStyle = 1`）。

4. **`FillChar(X, SizeOf(X), 0)` 用在托管数组字段上**
   原文 1599 / 1600：
   - `FillChar(FGuardianLevelRewardItems, SizeOf, 0)` → `Array.Clear(...)`；
   - `FillChar(FGuardianLevelItemCounts, SizeOf, 0)` → `FGuardianLevelItemCounts = default`。
   原文对**静态数组**做 `FillChar` 是 Delphi 常见写法，托管侧没有对应语义；已按"清零"等价改写并注释。

5. **`TShowGuildList.GetShowGuilds` 与 `TGuildGroupList.GetGroups` 边界写法不同但等价**
   前者 `Index < FList.Count`，后者 `Index <= FList.Count - 1`。**原文两处写法不同**，
   本车道逐字保留（并写了断言锁定两者在 `Index == Count` 时都返回 `nil`）。

6. **`TImgCountDownButton` 与 `TCountDownLabel` 三处"看似一样实则不同"**
   - 推进判据：前者 `> 1000`（严格），后者 `>= 1000`；
   - 前者**没有** `m_CountDownValue <= 0` 前置分支（0 会被继续减成负数），后者有；
   - 前者结束时调 `frmMain.SendMerchantDlgSelect`，后者调私有 `FClickEvent`。
   三处都有差异断言（见 `GuiSharePureTests`）。

7. **`OnMagicButtonClick` 用的是全局鼠标坐标，不是事件参数**
   原文 `FrmMain.UseMagic(g_nMouseX, g_nMouseY, Ctrl.m_Magic)` —— 形参 `X, Y` 被**完全忽略**。
   已写差异断言（传 (1,2) 断言实际用的是 (640,480)）。

8. **`FindMagicButton` / `DelScreenMagicButton(Magic)` 是**指针相等**而非 MagicID 相等**
   同 `wMagicId` 的两个不同 `PTClientMagic` 实例查不到彼此。`DelScreenMagicButton(MagicID:Word)`
   才是按 `Def.wMagicId` 匹配，且**不判 nil**（列表里若有 `m_Magic = nil` 的按钮会 AV）—— 原文缺陷，已保留并注释。

9. **`TGuildGroup.Create` 里 `inherited Create` 写在最后**
   原文先给字段赋值、最后才 `inherited Create`（通常写法相反）。托管侧构造语义无差异，但已注释保留顺序。

10. **`TNpcScrollBox.Destroy` 与 `TNpcLabel.Destroy` 的 inherited/Free 顺序相反**
    前者 `inherited` 在前再 `Free m_List`，后者先 `Free m_AutoColors` 再 `inherited`。原文如此。

11. **`TFrmDlg.Create` 只初始化了 201 个字段中的一部分**
    `ClientVersion`、`_DLableMenuDlgC1..3`、`_ListViewMenuDlg`、`DBackground` 之外的多数字段
    在 `Create` 里**未显式赋值**（靠 Delphi 的零初始化）；托管侧同样靠默认值，未额外"补初始化"。

12. **`RefrshDStorageViewDlgText` 的 `IsStorageViewDlgIsExt` 参数被完全忽略**
    原文只做 4 个字段赋值。已写差异断言。

13. **移植期自测抓到的一个真实缺陷（本车道自己的代码）**
    Delphi `Copy(S, Index)` 省略 Count 时等价于到串尾；C# 侧最初用
    `if (start + len > s.Length)` 夹取长度，而 `len` 默认 `int.MaxValue` 会**整型溢出**成负数，
    绕过夹取并让 `Substring` 抛 `ArgumentOutOfRangeException`。
    改为 `if (len > s.Length - start)`。由 6 个 `GetHitLines` 用例暴露。

14. **`SFrmDlg.FScreenMagicBtnList` 等 protected 字段的可见性**
    原文 `protected` 段声明了大量 FState 内部状态；托管侧逐字保留可见性，
    测试通过最小派生类 `TestFrmDlg` 访问（这也顺带验证了 TFrmDlg 可被继承 —— 与原文一致）。

---

## 7. 接缝清单

### 7.1 本车道新增的接缝（全部在 `FStateSeams.cs`，均带【接缝：待 …】注释）

| 接缝 | 对应的未移植单元 | 说明 |
|---|---|---|
| `TDxControlEngine` / `TDxChatMemo` / `TDxListView` / `TDxImageEdit` / `TDxTreeNode` / `TDxTreeView` | `DxComponent/DxControls.pas` 等 | 车道1 接缝未覆盖的控件类型 |
| `DxControlExt`（ConditionalWeakTable 外挂） | `DxComponent/DxControls.pas` | `AddData1/AddData2/DisableHideCtrl/OnMouseMove/OnMove/SetFocus` |
| `DxPopupMenuExt`（外挂） | `DxComponent/DxControls.pas` | `GuiType/Alpha/ItemColor/BorderColor/BackgroundColor/DrawBorder` |
| `TMemo` | `ClMain.pas frmMain` + WinForms 宿主 | `Create` 里的 Memo 初始化 |
| `TColor/TPoint/TShiftState/TMouseButton/TGridDrawState/TMsgDlgButtons/TModalResult/TFontStyles/TGraphic/TList/TMemoryStream/THashedStringList/ISuperObject` | Delphi RTL / VCL / superobject | 声明面所需的最小类型 |
| `PTClientItem/PTClientMagic/pTStdItem/PClientUserShop/PGuildJoinUser/pTUserEntry(Add)/pTUserCharacterInfo/PSellPlayerMoney/PTSellPlayerStorage*Header` | `Grobal2.pas` 指针类型 | 以持有记录的引用类型承载 |
| `THintLines` / `THintWindows` / `DrawScrn` | **`DrawScrn.pas`**（不是 FState.pas） | 提示行/提示窗 |
| `MShareHintFont` | **`MShare.pas:11735/11740/11752`** | 提示字体三函数 |
| `ConfigClientExt` | `MShare.pas TConfigClient` | `ItemHintTextConfig` / `boDisableDrogMagicIcon` / `boSaveMagicIconPosition` |
| `MagicButtonIniSeam` | `MShare.pas` 路径全局 + `FastIniFile` | `SaveMagicButtons` 的 INI 主体 |
| `FStateMShareSeam` | `MShare.pas` | `g_SellDlgItem` / `g_ExtBagOpenItemCount` |
| `FStateClMainSeam` | `ClMain.pas frmMain` | `SendMerchantDlgSelect` / `UseMagic` / `g_nMouseX/Y` / `g_nTargetX/Y` / `Owner` |
| `FStateSeamClock` | 无（**本车道为可验证性引入**） | 见 §7.3 |
| `TNpcGraphicButton.NewTextureFromGraphicHandler` | `DxCanvas/HGE` | `TGraphic → TTexture` |

### 7.2 复用而非复刻的既有接缝（避免台账 §9.3 的重复造接缝）

- `GXX.Client.GUI.DxComponent`（车道1）：`TDxControl/TDxImageButton/TDxLabel/TDxEdit/TDxImageForm/TDxImageGrid/TDxScrollBox/TDxPopupMenu/TDxPageControl/TDxMemo/TDxFont/TDxCaptionColor/TDxImageIndex/TRect/TTexture/TGameImages/TGuiImageIndex/TClientVersion/TImageType/TClickSound/TOnClickEx/TOnClickSound/TOnGetImage/TSaveUIColor`
- `GXX.Client.GUI.Mir`（车道1）：`MShareGlobals`（`g_MySelf/g_UserState1/g_MagicList/GetRGB/MyGetTickCount/GameCanvas/TGameCanvas` 等）、`ScreenSize`、`TColor`
- `GXX.Client.Scenes`：`TActor`
- `GXX.Core.Protocol`：`TClientItem/TStdItem/TClientMagic/TGuildJoinUser/TUserEntry(Add)/TUserCharacterInfo/TStorageHeroInfo/TGuildMemeberInfo/TGuardianLevelItemCounts/TClientUserShop/Grobal2Const`
- `GXX.Core.Protocol.SDK.TGList`
- `GXX.Core.Util.TStringList`

> **注意**：`MyGetTickCount` 的取值在 `FStateTypes.cs` / `TFrmDlg.Core.cs` 里走
> `FStateSeamClock.Now`（默认直通 `(uint)Environment.TickCount`，与原文 `GetTickCount` 同语义）；
> 见 §7.3 的理由。

### 7.3 唯一一处"为可验证性"引入的偏离

`FStateSeamClock` 把原文散落的 `MyGetTickCount` 收敛到一个可注入点。
理由：`TCountDownLabel.Update` 的 `>= 1000` 与 `TImgCountDownButton.Update` 的 `> 1000`
**只差一个等号**，用真实时钟无法确定性地命中"恰好 1000"这一格，也就写不出可靠断言。
默认实现与 `GetTickCount` 完全一致（含 uint 回绕），仅测试注入假时钟。
这是本波次唯一为测试可控性做的结构改动，**已在此显式登记**。

---

## 8. 建议改回真实现的既有接缝清单（交给集成者/下一轮）

> 本车道**没有改任何别人的文件**；下面只给建议，按优先级排列。

### 8.1 必须处理：同名类型重复（台账 §9.3 同款问题，已实测触发 CS0104）

| 位置 | 问题 | 建议 |
|---|---|---|
| `src/GXX.Client/GUI/Mir/MirForms.cs:96` `GXX.Client.GUI.Mir.TFrmDlg` | 与正式归属 `GXX.Client.GUI.Share.TFrmDlg` **同名同 Delphi 源**，任何同时 `using` 两个命名空间的文件都会 `CS0104` 歧义（本车道的测试已实际撞上，只能加 `using TFrmDlg = ...` 别名绕开） | 把 `GUI/Mir` 5 个窗口的基类改为 `GXX.Client.GUI.Share.TFrmDlg`，删除 `MirForms.cs` 里的 `TFrmDlg` 与 `TStateWindows` 接缝；`MirForms.cs:14/18/93/94/111/116/121/124/128/134/136/142/149/152/169/172` 的注释同步更新 |

### 8.2 建议合并：MShare.pas 的多套接缝

| 位置 | 建议 |
|---|---|
| 车道1 `GXX.Client.GUI.Mir.MShareGlobals` + 本车道 `FStateMShareSeam` + `ConfigClientExt` + `MagicButtonIniSeam` | MShare.pas 移植后统一为一个 `MShareGlobals`（本车道的 `FStateMShareSeam.g_SellDlgItem/g_ExtBagOpenItemCount`、`ConfigClientExt.*` 并入）。届时 `TFrmDlg.Core.cs:88` 的 `FStateMShareSeam.g_SellDlgItem` 与 `SaveMagicButtons` 的 `ConfigClientExt` 调用点按原名改回 |
| 车道1 `GXX.Client.GUI.Mir.frmMain` + 本车道 `FStateClMainSeam` | ClMain.pas 移植后合并为单一 `frmMain`；本车道的 `UseMagic/SendMerchantDlgSelect/g_nMouseX/g_nMouseY/g_nTargetX/g_nTargetY/Owner` 全部迁入 |

### 8.3 建议改为转调本车道真实现

| 位置 | 现在 | 建议 |
|---|---|---|
| 车道8 `GUI/NewStateWin/TStateWindowsText.cs:10` 的注释与接缝 | 登记为"`THintLines / GetHintFontSize / GetHintFontStyle / GetHintFontStroke`（**FState.pas**）未移植" | **归属更正**：这四个属于 `DrawScrn.pas` / `MShare.pas`。本车道已提供 `GXX.Client.GUI.Share.THintLines` 与 `MShareHintFont`，车道8 可直接转调；注释里的 `FState.pas` 应改为 `DrawScrn.pas` / `MShare.pas` |
| 车道8 `GUI/NewStateWin/TStateWindowsMeridians.cs:20` | "`g_MySelf / g_MyHero`（FState.pas，另一条车道）未移植" | **归属更正**：二者在 `MShare.pas`；且 `g_MyHero` 在 FState.pas 只有 20 处调用点、**无声明** |
| 车道8 `GUI/NewStateWin/TStateWindowsMagic.cs:18` | "`g_MagicList / g_MagicNGList / g_HeroMagicList / g_HeroMagicNGList`（MShare.pas + FState.pas）" | 四者全在 `MShare.pas`（FState.pas 无声明） |
| 车道1 `GUI/Mir/MirForms.cs:121` 的 `ShowMDlg` 接缝 | 自造几何/避让逻辑 | 待 `TFrmDlg.ShowMDlg`(原文 1864-1889) 移植后转调；该方法是 B 档依赖（需 `g_boOpenMerchantBigDlg`，已在车道1 `MShareGlobals`） |
| 车道1 `GUI/Mir/MirForms.cs:134` 的 `AddNpcMemo` 接缝 | 只记录调用次数与文本 | `AddNpcMemo`(13167-15506，2340 行) 属 C 档（需 MShare 大面积全局 + `TDxControl` 控件生成面），建议下一轮以"分片"方式单独排期 |
| 车道5 `src/GXX.Client/DxComponent/**` 与本车道 | 本车道的 Dx* 类型**全部**来自车道1 的 `GUI/DxComponent` 命名空间 | 待车道5 的 `DxComponent` 全量移植（`DxControls.pas` 3516 行等）完成后，`GUI/DxComponent` 应整体迁到 `src/GXX.Client/DxComponent`，届时本车道的 `using` 换一次即可 |

---

## 9. 本波次未做（如实声明）

1. **`GetMouseItemInfo` 两个重载（4285-11022，6737 行）未移植** —— 全单元最大两块，依赖
   `MShare.pas` 的物品提示/描述体系（`GetItemDesc`、`THintLines`、`StdItem` 属性表）与本单元的
   `THintWindows` 控件树；本波次只做到"声明面完整 + 调用即抛异常"。
2. **`AddNpcMemo`（2340 行）与 `AddMissionMemo2`（902 行）未移植** —— 需要
   `TDxControl` 的子控件生成/布局面（`ControlCount/Control[]/AddNpcMemo` 的 FCOLOR:/IMG:/ITEMSHOW: 解析）。
3. **`TFrmDlg.Destroy`（1644-1712）未移植** —— 不是"托管不需要"，而是原文会**逐个子控件 Free 并解绑事件**；
   简单留空会造成事件订阅泄漏，必须等控件树移植后按 1:1 结构改写为显式解绑。
4. **`SaveMagicButtons` 的 INI 读段/清段/写盘主体（24720-24733）与外派接缝** —— 需要
   `MShare.pas` 的 `g_sSelfFilePath/g_sPlugServerName/g_sPlugUserName/MAGIC_ICONS_INI_FILE`
   与 `FastIniFile`。守卫（两个 `Exit`）与"计数 + 逐项 (MagicID,X,Y) 收集"已 1:1 实现并有测试。
   `LoadMagicButtons`（66 行）同因未做。
5. **所有 `Paint`/绘制类方法未移植** —— 需要 `DxComponent` 的绘制面与 HGE 后端。
6. **`ProcessFileNameSpecialChar`（MShare.pas）在本车道是接缝式最小实现**（把 `\/:*?"<>|` 替换为 `_`），
   不是 1:1 移植 —— 已注释登记，待 MShare.pas 移植后替换。

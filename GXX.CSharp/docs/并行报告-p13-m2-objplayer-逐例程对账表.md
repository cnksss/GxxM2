# 附：`ObjPlayer.pas` 815 条类例程逐例程四态对账表

> 配套报告：`docs/并行报告-p13-m2-objplayer.md`（§9 引用本表）
> 抽取命令与四态口径见报告 §1 / §3。**本表由脚本机械生成**，与报告同源（同一份 `_p13_recon2.tsv`）。
>
> | 状态 | 含义 | 基线数 |
> |---|---|---|
> | **已覆盖** | 1:1 真实体（`p6-m2-playersurface` 及更早 `RecalcBonus.cs`） | 17 |
> | **★本车道新增** | 本车道 p13 新落的 1:1 真实体 | 1（`Operate`；另有 2 条**原文类内嵌套**例程 `ProcessPlayObjectMessage`/`CanFilter` 不在本表） |
> | **近似物/同名** | 托管侧有同名成员但是别的类/生成壳/简化克隆重 —— **不算覆盖** | 46 |
> | **未移植** | 托管侧无对应物（含 `NotPorted` 显式留痕） | 747 |
>
> 合计 **811**（本表只含 `TPlayObject` 的 811 条；`TWarrContinueHitManager` 的 4 条已 4/4 落地，见报告 §5 切片 3）。

| 状态 | 例程数 | 占 811 比例 |
|---|---|---|
| 已覆盖 | 17 | 2.10% |
| ★本车道新增 | 1 | 0.12% |
| 近似物/同名 | 46 | 5.67% |
| 未移植 | 747 | 92.11% |

## 实现段 L0 – L1999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 1486 | `Create` | 1022 | 近似物/同名 | GXX.Client\DxComponent\DIB.SharedImage.cs:156 GXX.Client\DxComponent\DxCanvas.cs:241 GXX.Client\DxComponent\DxImageForm.cs:422 GXX.Client\GUI\NewStateWin\TStateWindowsControlAddressList.cs:22 |

## 实现段 L2000 – L3999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 2508 | `DealCancel` | 16 | 未移植 | — |
| 2524 | `DealCancelA` | 6 | 未移植 | — |
| 2530 | `GoldChanged` | 5 | 已覆盖 | GXX.M2Server\DbLayer\M2DataDbSupport.cs:210 GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:183 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:548 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:613 |
| 2535 | `GameGoldChanged` | 5 | 已覆盖 | GXX.M2Server\DbLayer\M2DataDbSupport.cs:207 GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:226 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:553 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:618 |
| 2540 | `NewGamePointChanged` | 5 | 已覆盖 | GXX.M2Server\DbLayer\M2DataDbSupport.cs:213 GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:236 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:562 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:627 |
| 2545 | `GameGloryChanged` | 5 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:245 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:573 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:638 |
| 2550 | `RunNotice` | 85 | 未移植 | — |
| 2635 | `WinExp` | 40 | 未移植 | — |
| 2675 | `GetExp` | 163 | 未移植 | — |
| 2838 | `IncExp` | 87 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:582 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:718 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:647 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:787 |
| 2925 | `WinExpNG` | 41 | 未移植 | — |
| 2966 | `GetExpNG` | 110 | 未移植 | — |
| 3076 | `IncExpNG` | 62 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:584 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:719 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:649 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:788 |
| 3138 | `IncBeadExp` | 94 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:586 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:651 |
| 3232 | `IncGold` | 20 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:132 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:546 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:611 |
| 3252 | `IncGameGold` | 11 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:198 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:551 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:616 |
| 3263 | `IncGamePoint` | 11 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:556 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:621 |
| 3274 | `IncGameGird` | 11 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:565 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:630 |
| 3285 | `DecGold` | 11 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:163 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:547 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:612 |
| 3296 | `DecGameGold` | 10 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.Gold.cs:213 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:552 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:617 |
| 3306 | `DecGameGird` | 8 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:566 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:631 |
| 3314 | `IncGameDiamond` | 11 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:560 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:625 |
| 3325 | `DecGameDiamond` | 8 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:561 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:626 |
| 3333 | `IncGameGlory` | 8 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:571 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:636 |
| 3341 | `DecGameGlory` | 8 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:572 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:637 |
| 3349 | `SetSoftVersionDateEx` | 5 | 未移植 | — |
| 3354 | `SendAcupointLevels` | 7 | 未移植 | — |
| 3361 | `SendAddItem` | 36 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Items.cs:450 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:654 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:706 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:719 |
| 3397 | `IsGroupMember` | 32 | 未移植 | — |
| 3429 | `Whisper` | 83 | 未移植 | — |
| 3512 | `IsBlockWhisper` | 15 | 未移植 | — |
| 3527 | `SendSocket` | 30 | 近似物/同名 | GXX.M2Server\Engine\IdSrvClient.cs:41 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:646 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:711 GXX.M2Server\Sweep9\Monsters\ObjRobotCore.cs:625 |
| 3557 | `SendSocketEx` | 31 | 未移植 | — |
| 3588 | `SendOpenMagic` | 12 | 未移植 | — |
| 3600 | `SendDefMessage` | 15 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:647 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:712 GXX.SelGate\SelGateSession.cs:113 |
| 3615 | `ClientQueryAssessHero` | 39 | 未移植 | — |
| 3654 | `RefUserState` | 14 | 未移植 | — |
| 3668 | `GetHearMsgFColor` | 8 | 未移植 | — |
| 3676 | `RefHearMsgColor` | 5 | 未移植 | — |
| 3681 | `RefMyStatus` | 7 | 未移植 | — |
| 3688 | `CanSaveToStorage` | 5 | 未移植 | — |
| 3693 | `CanSaveToBigStorage` | 6 | 未移植 | — |
| 3699 | `Operate` | 73 | ★本车道新增 | `PlayerSurface/TPlayObject.PlayerSurface.Core2.cs`（1:1，含 2 条原文内嵌例程） |
| 3772 | `Run` | 1835 | 近似物/同名 | GXX.Client\GUI\GameConfig\GameConfigDlg.cs:296 GXX.Client\GUI\GameConfig\GameConfigDlgs.cs:365 GXX.Client\GUI\GameConfig\MirReturnConfigDlg.Lifecycle.cs:115 GXX.Client\GUI\GameConfig\MirsConfigDlg.cs:873 |

## 实现段 L4000 – L5999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 5607 | `ProcessSpiritSuite` | 33 | 未移植 | — |
| 5640 | `LogonTimcCost` | 19 | 未移植 | — |
| 5659 | `ProcessSayMsg` | 235 | 未移植 | — |
| 5894 | `HorseRunTo` | 256 | 未移植 | — |

## 实现段 L6000 – L7999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 6150 | `GetRangeHumanCount` | 5 | 未移植 | — |
| 6155 | `GetStartPoint` | 48 | 未移植 | — |
| 6203 | `GetQuestFlagStatus` | 20 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.NpcSession.cs:166 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:602 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:698 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:667 |
| 6223 | `SetQuestFlagStatus` | 22 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.NpcSession.cs:215 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:603 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:699 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:668 |
| 6245 | `ChallengeCancel` | 18 | 未移植 | — |
| 6263 | `ChallengeCancelA` | 6 | 未移植 | — |
| 6269 | `CheckMoney` | 21 | 未移植 | — |
| 6290 | `CheckMoney` | 21 | 未移植 | — |
| 6311 | `DecMoney` | 26 | 未移植 | — |
| 6337 | `DecMoney` | 24 | 未移植 | — |
| 6361 | `IncMoney` | 22 | 未移植 | — |
| 6383 | `IncMoney` | 24 | 未移植 | — |
| 6407 | `GetMoney` | 18 | 未移植 | — |
| 6425 | `GetMoney` | 18 | 未移植 | — |
| 6443 | `DecGamePoint` | 8 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:557 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:622 |
| 6451 | `Destroy` | 137 | 近似物/同名 | GXX.Client\DxComponent\DIB.Core.cs:72 GXX.Client\DxComponent\DIB.Core.cs:171 GXX.Client\DxComponent\DIB.cs:561 GXX.Client\DxComponent\DIB.cs:588 |
| 6588 | `Disappear` | 19 | 未移植 | — |
| 6607 | `DisappearB` | 11 | 未移植 | — |
| 6618 | `DropUseItems` | 325 | 未移植 | — |
| 6943 | `DropJewelryBoxItems` | 298 | 未移植 | — |
| 7241 | `DropGodBlessItems` | 303 | 未移植 | — |
| 7544 | `GainExp` | 117 | 未移植 | — |
| 7661 | `GainExpNG` | 103 | 未移植 | — |
| 7764 | `GameTimeChanged` | 9 | 未移植 | — |
| 7773 | `WeatherChanged` | 5 | 近似物/同名 | GXX.M2Server\EnvirMapCore.cs:854 |
| 7778 | `GetBackDealItems` | 17 | 未移植 | — |
| 7795 | `GetBackChallengeItems` | 43 | 未移植 | — |
| 7838 | `GetBagUseItems` | 142 | 未移植 | — |
| 7980 | `GeTBaseObjectInfo` | 19 | 未移植 | — |
| 7999 | `GetDigUpMsgCount` | 14 | 未移植 | — |

## 实现段 L8000 – L9999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 8013 | `SendUpgradeItem` | 22 | 未移植 | — |
| 8035 | `DoQueryBagItems` | 59 | 未移植 | — |
| 8094 | `ClearStatusTime` | 8 | 未移植 | — |
| 8102 | `ClearTimeLabel` | 13 | 未移植 | — |
| 8115 | `ClearAllDelayLabel` | 14 | 未移植 | — |
| 8129 | `SendMapDescription` | 16 | 未移植 | — |
| 8145 | `GetMapCanRun` | 50 | 未移植 | — |
| 8195 | `SendMapCanRun` | 8 | 近似物/同名 | GXX.M2Server\Engine\M2Config.GameSpeed.cs:120 |
| 8203 | `SendNotice` | 105 | 未移植 | — |
| 8308 | `UserLogon` | 825 | 未移植 | — |
| 9133 | `GetBoxItems` | 81 | 未移植 | — |
| 9214 | `SendGoldInfo` | 15 | 未移植 | — |
| 9229 | `SendNewGamePointInfo` | 12 | 未移植 | — |
| 9241 | `SendGameGlory` | 5 | 未移植 | — |
| 9246 | `SendSpecialCmdList` | 46 | 未移植 | — |
| 9292 | `SendEffectImageList` | 46 | 未移植 | — |
| 9338 | `SendMissionNPC` | 12 | 未移植 | — |
| 9350 | `SendClientModules` | 50 | 未移植 | — |
| 9400 | `SendFilterItemList` | 32 | 未移植 | — |
| 9432 | `SendItemDescList` | 32 | 未移植 | — |
| 9464 | `SendItemDescTopList` | 32 | 未移植 | — |
| 9496 | `SendTzItemDescList` | 31 | 未移植 | — |
| 9527 | `SendCustomMonsterConfig` | 50 | 未移植 | — |
| 9577 | `SendCustomMagicConfig` | 50 | 未移植 | — |
| 9627 | `SendCustomNpcConfig` | 50 | 未移植 | — |
| 9677 | `SendDropItemEffectList` | 50 | 未移植 | — |
| 9727 | `SendEnabledAuctionItemList` | 30 | 未移植 | — |
| 9757 | `SendUnbindList` | 12 | 未移植 | — |
| 9769 | `SendInputBoxFilterList` | 15 | 未移植 | — |
| 9784 | `SendStdItemList` | 52 | 未移植 | — |
| 9836 | `SendPlugClientList` | 43 | 未移植 | — |
| 9879 | `SendCustomMoney` | 35 | 未移植 | — |
| 9914 | `SendClientBlackModules` | 25 | 未移植 | — |
| 9939 | `SendCustomItemPropertyConfig` | 18 | 近似物/同名 | GXX.M2Server\Forms\ItemProperty\CustomItemPropertySeams.cs:262 |
| 9957 | `SendCustomItemPropertyTextVarList` | 18 | 近似物/同名 | GXX.M2Server\Forms\ItemProperty\CustomItemPropertySeams.cs:265 |
| 9975 | `SendArrButtonConfig` | 17 | 未移植 | — |
| 9992 | `SendLogon` | 22 | 近似物/同名 | GXX.M2Server\Engine\IdSrvClient.cs:150 |

## 实现段 L10000 – L11999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 10014 | `SendServerConfig` | 21 | 近似物/同名 | GXX.M2Server\Engine\M2Config.GameSpeed.cs:119 GXX.M2Server\Forms\CustomMagic\CustomMagicSeams.cs:452 |
| 10035 | `SendEnableClientUploadPickItems` | 9 | 未移植 | — |
| 10044 | `SendUseItems` | 41 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:653 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:700 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:718 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:769 |
| 10085 | `SendUseIcons` | 48 | 未移植 | — |
| 10133 | `SendUseEffects` | 48 | 未移植 | — |
| 10181 | `SendUseMagic` | 59 | 未移植 | — |
| 10240 | `ClientTakeOnItemsEx` | 237 | 未移植 | — |
| 10477 | `ClientTakeOffItemsEx` | 136 | 未移植 | — |
| 10613 | `UseStdmodeFunItem` | 11 | 未移植 | — |
| 10624 | `HeroUseStdmodeFunItem` | 11 | 未移植 | — |
| 10635 | `UseStdmodeFunItemEx` | 11 | 未移植 | — |
| 10646 | `HeroUseStdmodeFunItemEx` | 12 | 未移植 | — |
| 10658 | `DieFunc` | 12 | 未移植 | — |
| 10670 | `LevelUpFunc` | 12 | 未移植 | — |
| 10682 | `LevelUpFuncNG` | 12 | 未移植 | — |
| 10694 | `KillPlayFunc` | 11 | 未移植 | — |
| 10705 | `HeroKillPlayFunc` | 12 | 未移植 | — |
| 10717 | `ChallengeVictory` | 141 | 未移植 | — |
| 10858 | `ClientGuildAlly` | 57 | 未移植 | — |
| 10915 | `ClientGuildBreakAlly` | 33 | 未移植 | — |
| 10948 | `RecalcAdjusBonus` | 144 | 已覆盖 | GXX.M2Server\Engine\RecalcBonus.cs:183 |
| 11092 | `GetMyStatus` | 7 | 未移植 | — |
| 11099 | `SendAdjustBonus` | 34 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:532 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:597 |
| 11133 | `SendGroupText` | 30 | 未移植 | — |
| 11163 | `LeaveGroup` | 20 | 未移植 | — |
| 11183 | `CancelGroup` | 25 | 未移植 | — |
| 11208 | `DelGroupMember` | 123 | 未移植 | — |
| 11331 | `HideItem` | 14 | 近似物/同名 | GXX.M2Server\VisibleItemLifecycleCore.cs:292 |
| 11345 | `UpdateVisibleEvent` | 22 | 近似物/同名 | GXX.M2Server\VisibleItemLifecycleCore.cs:314 |
| 11367 | `ClearObject` | 24 | 未移植 | — |
| 11391 | `KillMonsterFunc` | 19 | 未移植 | — |
| 11410 | `PKDie` | 161 | 未移植 | — |
| 11571 | `DieGotoLable` | 19 | 未移植 | — |
| 11590 | `SendGroupMembers` | 32 | 未移植 | — |
| 11622 | `SendGroupMembers` | 77 | 未移植 | — |
| 11699 | `GetMagicInfo` | 39 | 未移植 | — |
| 11738 | `GetMagicInfo` | 23 | 未移植 | — |
| 11761 | `DoMotaebo` | 263 | 未移植 | — |

## 实现段 L12000 – L13999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 12024 | `DoMotaebo100` | 267 | 未移植 | — |
| 12291 | `PileStones` | 79 | 未移植 | — |
| 12370 | `SendSaveItemList` | 44 | 未移植 | — |
| 12414 | `SendSaveBigStorageItemList` | 65 | 未移植 | — |
| 12479 | `SendStorageViewItemList` | 139 | 未移植 | — |
| 12618 | `SendChangeGuildName` | 11 | 未移植 | — |
| 12629 | `SendDelItemList` | 12 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:655 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:720 |
| 12641 | `SendDelItem` | 19 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Items.cs:516 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:656 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:707 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:721 |
| 12660 | `GetUpdateItem` | 29 | 未移植 | — |
| 12689 | `SendUpdateItem` | 22 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:657 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:708 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:722 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:777 |
| 12711 | `SendUpdateItemName` | 9 | 未移植 | — |
| 12720 | `SendUpdateItemColor` | 9 | 未移植 | — |
| 12729 | `SendUpdateItemDura` | 9 | 未移植 | — |
| 12738 | `SendUpdateItemDuraMax` | 9 | 未移植 | — |
| 12747 | `SendUpdateItemUpgradeCount` | 10 | 未移植 | — |
| 12757 | `SendUpdateItemNewLook` | 9 | 未移植 | — |
| 12766 | `SendUpdateItemNewShape` | 9 | 未移植 | — |
| 12775 | `SendUpdateItemHeroM2Light` | 10 | 未移植 | — |
| 12785 | `SendUpdateItemInsuranceCount` | 11 | 未移植 | — |
| 12796 | `SendUpdateItemBind` | 9 | 未移植 | — |
| 12805 | `SendUpdateItemLimitTime` | 6 | 未移植 | — |
| 12811 | `SendUpdateItemNewValue` | 7 | 未移植 | — |
| 12818 | `SendUpdateItemFlute` | 10 | 未移植 | — |
| 12828 | `SendUpdateItemProgress` | 10 | 未移植 | — |
| 12838 | `SendUpdateItemPropertyText` | 9 | 未移植 | — |
| 12847 | `SendUpdateItemPropertyColor` | 9 | 未移植 | — |
| 12856 | `SendUpdateItemPropertyValue` | 12 | 未移植 | — |
| 12868 | `SysMsg` | 48 | 近似物/同名 | GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:62 GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:80 GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:172 GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:195 |
| 12916 | `SysMsg` | 34 | 近似物/同名 | GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:62 GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:80 GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:172 GXX.Client\Scenes\DrawScrn\SysMsgFamily.cs:195 |
| 12950 | `SysMsgEx` | 28 | 未移植 | — |
| 12978 | `SysMsgEx` | 14 | 未移植 | — |
| 12992 | `CheckTakeOnItems` | 384 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:651 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:716 |
| 13376 | `GetUserItemWeitht` | 41 | 未移植 | — |
| 13417 | `GetUserItemHandWeight` | 20 | 未移植 | — |
| 13437 | `EatItems` | 164 | 未移植 | — |
| 13601 | `ReadBook` | 53 | 未移植 | — |
| 13654 | `SendAddMagic` | 24 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:665 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:711 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:730 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:780 |
| 13678 | `GetMaxBagCount` | 6 | 已覆盖 | GXX.Client\GUI\GameConfig\Mir\MirActorSeams.cs:115 GXX.Client\GUI\GameConfig\Seams\ConfigSeamsData.cs:133 GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Items.cs:250 GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Items.cs:419 |
| 13684 | `SendDelMagic` | 90 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:666 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:712 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:731 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:781 |
| 13774 | `EatUseItems` | 102 | 未移植 | — |
| 13876 | `EatAttackItem` | 87 | 未移植 | — |
| 13963 | `MoveToHome` | 10 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:644 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:709 |
| 13973 | `MoveRandomToHome` | 6 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:645 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:710 |
| 13979 | `InitSpeed` | 25 | 未移植 | — |

## 实现段 L14000 – L15999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 14004 | `BaseObjectMove` | 29 | 未移植 | — |
| 14033 | `WeaptonMakeLuck` | 63 | 未移植 | — |
| 14096 | `RepairWeapon` | 41 | 未移植 | — |
| 14137 | `SuperRepairWeapon` | 33 | 未移植 | — |
| 14170 | `WinLottery` | 93 | 未移植 | — |
| 14263 | `SendDelDealItem` | 27 | 未移植 | — |
| 14290 | `SendAddDealItem` | 20 | 未移植 | — |
| 14310 | `OpenDealDlg` | 10 | 未移植 | — |
| 14320 | `OpenChallengeDlg` | 11 | 未移植 | — |
| 14331 | `SendDelChallengeItem` | 23 | 未移植 | — |
| 14354 | `SendAddChallengeItem` | 20 | 未移植 | — |
| 14374 | `JoinGroup` | 7 | 未移植 | — |
| 14381 | `MakeMine` | 123 | 未移植 | — |
| 14504 | `QuestTakeCheckItem` | 41 | 未移植 | — |
| 14545 | `MakeSaveRcd` | 354 | 未移植 | — |
| 14899 | `RefRankInfo` | 7 | 未移植 | — |
| 14906 | `GetHitMsgCount` | 27 | 近似物/同名 | GXX.Core\Protocol\HitActionMsgSet.cs:85 |
| 14933 | `GetSpellMsgCount` | 23 | 未移植 | — |
| 14956 | `GetRunMsgCount` | 23 | 未移植 | — |
| 14979 | `GetWalkMsgCount` | 23 | 未移植 | — |
| 15002 | `GetTurnMsgCount` | 23 | 未移植 | — |
| 15025 | `GetSiteDownMsgCount` | 23 | 未移植 | — |
| 15048 | `CheckActionStatus` | 125 | 未移植 | — |
| 15173 | `SetScriptLabel` | 7 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.NpcSession.cs:252 |
| 15180 | `GetScriptLabel` | 55 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TPlayObject.PlayerSurface.NpcSession.cs:281 |
| 15235 | `LableIsCanJmp` | 74 | 未移植 | — |
| 15309 | `RecalcAbilitys` | 311 | 近似物/同名 | GXX.M2Server\Engine\RecalcChain.cs:32 GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Base.cs:113 GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Base.cs:341 |
| 15620 | `SearchViewRange` | 534 | 未移植 | — |

## 实现段 L16000 – L17999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 16154 | `GetShowName` | 145 | 未移植 | — |
| 16299 | `CheckItemsNeed` | 53 | 已覆盖 | GXX.M2Server\Engine\PlayerSurface\TCreature.PlayerSurface.Items.cs:557 |
| 16352 | `CheckMarry` | 93 | 未移植 | — |
| 16445 | `CheckMaster` | 315 | 未移植 | — |
| 16760 | `MakeGhost` | 144 | 近似物/同名 | GXX.M2Server\Engine\ObjBase.OnlineMsg.cs:55 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:242 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:303 GXX.M2Server\Sweep9\DataLayer\ItemEvent.cs:341 |
| 16904 | `GetMyInfo` | 29 | 未移植 | — |
| 16933 | `CheckItemBindUse` | 76 | 未移植 | — |
| 17009 | `ScatterBagItems` | 154 | 未移植 | — |
| 17163 | `RecallHuman` | 24 | 未移植 | — |
| 17187 | `ReQuestGuildWar` | 41 | 未移植 | — |
| 17228 | `CheckDenyLogon` | 29 | 未移植 | — |
| 17257 | `ChangeMyShopType` | 8 | 未移植 | — |
| 17265 | `ClientTurn` | 200 | 未移植 | — |
| 17465 | `ClientWalk` | 310 | 未移植 | — |
| 17775 | `ClientSitDown` | 214 | 未移植 | — |
| 17989 | `ClientRun` | 392 | 未移植 | — |

## 实现段 L18000 – L19999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 18381 | `ClientHorseRun` | 156 | 未移植 | — |
| 18537 | `ClientHit` | 467 | 未移植 | — |
| 19004 | `ClientSpell` | 1186 | 未移植 | — |

## 实现段 L20000 – L21999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 20190 | `ClientQueryUserName` | 22 | 未移植 | — |
| 20212 | `ClientDropItem` | 206 | 未移植 | — |
| 20418 | `ClientPickUpItem` | 262 | 未移植 | — |
| 20680 | `ClientTakeOnItems` | 468 | 未移植 | — |
| 21148 | `ClientTakeOffItems` | 147 | 未移植 | — |
| 21295 | `ClientUseItems` | 1178 | 未移植 | — |

## 实现段 L22000 – L23999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 22473 | `ClientMagicKeyChange` | 29 | 未移植 | — |
| 22502 | `ClientClickNpc` | 32 | 未移植 | — |
| 22534 | `ClientMerchantDlgSelect` | 73 | 未移植 | — |
| 22607 | `ClientMerchantQuerySellPrice` | 47 | 未移植 | — |
| 22654 | `ClientUserSellItem` | 41 | 未移植 | — |
| 22695 | `ClientTradingSellItems` | 53 | 未移植 | — |
| 22748 | `ClientUserBuyItem` | 26 | 未移植 | — |
| 22774 | `ClientUserGetDetailItem` | 25 | 未移植 | — |
| 22799 | `ClientDropGold` | 38 | 未移植 | — |
| 22837 | `ClientLoginNoticeOK` | 4 | 未移植 | — |
| 22841 | `ClientGroupMode` | 35 | 未移植 | — |
| 22876 | `ClientCreateGroup` | 92 | 未移植 | — |
| 22968 | `ClientAddGroupMember` | 90 | 未移植 | — |
| 23058 | `ClientDelGroupMember` | 32 | 未移植 | — |
| 23090 | `ClientUserRepairItem` | 35 | 未移植 | — |
| 23125 | `ClientMerchantQueryRepairCost` | 39 | 未移植 | — |
| 23164 | `ClientDealTry` | 64 | 未移植 | — |
| 23228 | `ClientDealAddItem` | 55 | 未移植 | — |
| 23283 | `ClientDealDelItem` | 56 | 未移植 | — |
| 23339 | `ClientDealCancel` | 5 | 未移植 | — |
| 23344 | `ClientDealChgGold` | 41 | 未移植 | — |
| 23385 | `ClientDealEnd` | 151 | 未移植 | — |
| 23536 | `ClientUserStorageItem` | 180 | 未移植 | — |
| 23716 | `ClientUserTakebackStorageItem` | 237 | 未移植 | — |
| 23953 | `ClientTakeBackStorageViewItem` | 221 | 未移植 | — |

## 实现段 L24000 – L25999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 24174 | `ClientGetBackStorageViewItem` | 31 | 未移植 | — |
| 24205 | `ClientUserStorageViewItem` | 179 | 未移植 | — |
| 24384 | `ClientWantMinMap` | 22 | 未移植 | — |
| 24406 | `ClientUserMakeDrugItem` | 12 | 未移植 | — |
| 24418 | `OpenGuildDlg` | 90 | 未移植 | — |
| 24508 | `ClientOpenGuildDlg` | 16 | 未移植 | — |
| 24524 | `ClientGuildHome` | 5 | 未移植 | — |
| 24529 | `ClientGuildMemberList` | 58 | 未移植 | — |
| 24587 | `ClientGuildAddMember` | 85 | 未移植 | — |
| 24672 | `ClientGuildAddMemberEx` | 82 | 未移植 | — |
| 24754 | `ClientGuildDelMember` | 66 | 未移植 | — |
| 24820 | `ClientGuildDelSelf` | 34 | 未移植 | — |
| 24854 | `ClientGuildUnMasterSelf` | 48 | 未移植 | — |
| 24902 | `ClientGuildUpdateNotice` | 18 | 未移植 | — |
| 24920 | `ClientGuildUpdateRankInfo` | 27 | 未移植 | — |
| 24947 | `ClientGuildUpdateJoinCondition` | 8 | 未移植 | — |
| 24955 | `ClientGuildJoinTo` | 60 | 未移植 | — |
| 25015 | `ClientGuildCancelJoinTo` | 24 | 未移植 | — |
| 25039 | `ClientGuildViewJoinCondition` | 12 | 未移植 | — |
| 25051 | `ClientGuildGetJoinUserList` | 68 | 未移植 | — |
| 25119 | `ClientGuildAcceptUserJoin` | 121 | 未移植 | — |
| 25240 | `ClientGuildRefusalUserJoin` | 44 | 未移植 | — |
| 25284 | `ClientGuildDelMemberEx` | 56 | 未移植 | — |
| 25340 | `ClientGuildMemberMoveToRank` | 127 | 未移植 | — |
| 25467 | `ClientGuildAddAttention` | 24 | 未移植 | — |
| 25491 | `ClientGuildDelAttention` | 20 | 未移植 | — |
| 25511 | `ClientGuildRequestAlly` | 30 | 未移植 | — |
| 25541 | `ClientGuildAcceptAlly` | 29 | 未移植 | — |
| 25570 | `ClientGuildRefusalAlly` | 22 | 未移植 | — |
| 25592 | `ClientGuildUnAlly` | 28 | 未移植 | — |
| 25620 | `ClientGuildViewMemberInfo` | 49 | 未移植 | — |
| 25669 | `ClientGuildSetMaster1` | 53 | 未移植 | — |
| 25722 | `ClientGuildChangeAlly` | 11 | 未移植 | — |
| 25733 | `ClientGuildAddGuildWar` | 41 | 未移植 | — |
| 25774 | `ClientAdjustBonus` | 53 | 未移植 | — |
| 25827 | `ClientPassWord` | 122 | 未移植 | — |
| 25949 | `ClientSay` | 13 | 未移植 | — |
| 25962 | `ClientQueryUserState` | 199 | 未移植 | — |

## 实现段 L26000 – L27999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 26161 | `ClientQueryBagItems` | 68 | 未移植 | — |
| 26229 | `ClientQueryHeroBagItems` | 20 | 未移植 | — |
| 26249 | `ClientOpenDoor` | 16 | 未移植 | — |
| 26265 | `ClientSoftClose` | 13 | 未移植 | — |
| 26278 | `ClientGuildAllyDo` | 5 | 未移植 | — |
| 26283 | `ClientGuildBreakAllyDo` | 6 | 未移植 | — |
| 26289 | `ClientGetShopItems` | 89 | 未移植 | — |
| 26378 | `ClientBuyShopItem` | 348 | 未移植 | — |
| 26726 | `ClientBuyShopItemGive` | 378 | 未移植 | — |
| 27104 | `ClientQueryRanking` | 21 | 未移植 | — |
| 27125 | `ClientGetBackBox` | 26 | 未移植 | — |
| 27151 | `ClientOpenBox` | 46 | 未移植 | — |
| 27197 | `ClientRotationBox` | 72 | 未移植 | — |
| 27269 | `ClientGetBoxItem` | 252 | 未移植 | — |
| 27521 | `ClientGameGoldDealSell` | 195 | 未移植 | — |
| 27716 | `ClientGameGoldDealBuy` | 104 | 未移植 | — |
| 27820 | `ClientGameGoldDealCancel` | 95 | 未移植 | — |
| 27915 | `ClientOverLapItem` | 121 | 未移植 | — |

## 实现段 L28000 – L29999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 28036 | `ClientHeroOverLapItem` | 126 | 未移植 | — |
| 28162 | `ClientPackageItem` | 83 | 未移植 | — |
| 28245 | `ClientHeroPackageItem` | 83 | 未移植 | — |
| 28328 | `ClientQueryUserShops` | 69 | 未移植 | — |
| 28397 | `SendQuerySelectShopInfo` | 54 | 未移植 | — |
| 28451 | `ClientQuerySelectShopInfo` | 33 | 未移植 | — |
| 28484 | `ClientGetUserShops` | 73 | 未移植 | — |
| 28557 | `SendQueryUserShopItem` | 90 | 未移植 | — |
| 28647 | `ClientQueryUserShopItem` | 34 | 未移植 | — |
| 28681 | `ClientGetUserShopItem` | 75 | 未移植 | — |
| 28756 | `ClientQuerySearchShopItem` | 101 | 未移植 | — |
| 28857 | `ClientGetSearchShopItem` | 86 | 未移植 | — |
| 28943 | `SendQueryMyShopItem` | 100 | 未移植 | — |
| 29043 | `SendMyShopItems` | 96 | 未移植 | — |
| 29139 | `ClientQueryMyShopItem` | 28 | 未移植 | — |
| 29167 | `ClientGetMyShopItem` | 26 | 未移植 | — |
| 29193 | `ClientAddItemToMyShop` | 526 | 未移植 | — |
| 29719 | `ClientChgItemToMyShop` | 358 | 未移植 | — |

## 实现段 L30000 – L31999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 30077 | `ClientMoveMyShopItem` | 439 | 未移植 | — |
| 30516 | `ClientShopStallStatus` | 17 | 未移植 | — |
| 30533 | `ClientBuyUserShopItem` | 435 | 未移植 | — |
| 30968 | `ClientDeleteMyShopSelledItem` | 187 | 未移植 | — |
| 31155 | `ClientUsePlugin` | 17 | 未移植 | — |
| 31172 | `ClientCheckPlugin` | 17 | 未移植 | — |
| 31189 | `ClientRungateSendFilterMsg` | 22 | 未移植 | — |
| 31211 | `ClientGamePetToBag` | 8 | 未移植 | — |
| 31219 | `ClientGamePetRecall` | 93 | 未移植 | — |
| 31312 | `DoGamePetRetake` | 44 | 未移植 | — |
| 31356 | `ClientGamePetRetake` | 8 | 未移植 | — |
| 31364 | `ClientGamePetFree` | 54 | 未移植 | — |
| 31418 | `ClientPetUseItems` | 348 | 未移植 | — |
| 31766 | `ClientBagItemToPetBag` | 58 | 未移植 | — |
| 31824 | `ClientPetBagItemToBag` | 48 | 未移植 | — |
| 31872 | `ClientPetOverLapItem` | 125 | 未移植 | — |
| 31997 | `ClientPetDropItem` | 189 | 未移植 | — |

## 实现段 L32000 – L33999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 32186 | `ClientUpgrageDlg` | 102 | 未移植 | — |
| 32288 | `ClientCancelUpgrageDlg` | 14 | 未移植 | — |
| 32302 | `ClientChallengeTry` | 83 | 未移植 | — |
| 32385 | `ClientAddChallengeItem` | 60 | 未移植 | — |
| 32445 | `ClientDelChallengeItem` | 56 | 未移植 | — |
| 32501 | `ClientCancelChallenge` | 5 | 未移植 | — |
| 32506 | `ClientChangeChallengeGold` | 42 | 未移植 | — |
| 32548 | `ClientChangeChallengeGameDiamond` | 89 | 未移植 | — |
| 32637 | `ClientChallengeStart` | 75 | 未移植 | — |
| 32712 | `ClientUpgradeDialog` | 1082 | 未移植 | — |
| 33794 | `ClientHelpButtonClick` | 9 | 未移植 | — |
| 33803 | `ClientPleaseDrink` | 77 | 未移植 | — |
| 33880 | `ClientGiveNpcWine` | 46 | 未移植 | — |
| 33926 | `ClientGuessfinger` | 65 | 未移植 | — |
| 33991 | `ClientDrinkUpdateValue` | 88 | 未移植 | — |

## 实现段 L34000 – L35999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 34079 | `ClientAssessHero` | 14 | 未移植 | — |
| 34093 | `ClientGetBackHero` | 21 | 未移植 | — |
| 34114 | `ClientHeroAutoPractice` | 5 | 未移植 | — |
| 34119 | `ClientAcupointClick` | 36 | 未移植 | — |
| 34155 | `ClientTriningMeridianClick` | 33 | 未移植 | — |
| 34188 | `ClientStartContinuous` | 209 | 未移植 | — |
| 34397 | `ClientStopContinuous` | 6 | 未移植 | — |
| 34403 | `ClientChangeContinuousMagicOrder` | 74 | 未移植 | — |
| 34477 | `ClientAddModuleMD5` | 8 | 未移植 | — |
| 34485 | `ClientSendShopName` | 20 | 未移植 | — |
| 34505 | `ClientHeroLogon` | 119 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:538 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:603 |
| 34624 | `ClientMasterBagToHeroBag` | 83 | 未移植 | — |
| 34707 | `ClientHeroBagToMasterBag` | 52 | 未移植 | — |
| 34759 | `ClientHeroTakeOnItems` | 384 | 未移植 | — |
| 35143 | `ClientHeroTakeOffItems` | 138 | 未移植 | — |
| 35281 | `ClientHeroUseItems` | 645 | 未移植 | — |
| 35926 | `ClientHeroTarget` | 255 | 未移植 | — |

## 实现段 L36000 – L37999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 36181 | `ClientHeroDropItem` | 140 | 未移植 | — |
| 36321 | `ClientHeroGroupAttack` | 199 | 未移植 | — |
| 36520 | `ClientHeroChangeMagicKey` | 49 | 未移植 | — |
| 36569 | `ClientHeroProTect` | 60 | 未移植 | — |
| 36629 | `ClientSlaveTarget` | 226 | 未移植 | — |
| 36855 | `ServerSendTurn` | 33 | 未移植 | — |
| 36888 | `ServerSendTurnEx` | 7 | 未移植 | — |
| 36895 | `ServerSendRush` | 72 | 未移植 | — |
| 36967 | `ServerSendWalk` | 10 | 未移植 | — |
| 36977 | `ServerSendRun` | 11 | 未移植 | — |
| 36988 | `ServerSendHorseRun` | 10 | 未移植 | — |
| 36998 | `ServerSendSitDown` | 4 | 未移植 | — |
| 37002 | `ServerSendHit` | 16 | 未移植 | — |
| 37018 | `ServerSendHeavyHit` | 16 | 未移植 | — |
| 37034 | `ServerSendBigHit` | 16 | 未移植 | — |
| 37050 | `ServerSendPowerHit` | 16 | 未移植 | — |
| 37066 | `ServerSendLongHit` | 16 | 未移植 | — |
| 37082 | `ServerSendWideHit` | 16 | 未移植 | — |
| 37098 | `ServerSendFireHit` | 16 | 未移植 | — |
| 37114 | `ServerSendCrsHit` | 16 | 未移植 | — |
| 37130 | `ServerSendSWordHit` | 16 | 未移植 | — |
| 37146 | `ServerSendTwnHit` | 16 | 未移植 | — |
| 37162 | `ServerSend43Hit` | 16 | 未移植 | — |
| 37178 | `ServerSend60Hit` | 16 | 未移植 | — |
| 37194 | `ServerSend61Hit` | 16 | 未移植 | — |
| 37210 | `ServerSend62Hit` | 16 | 未移植 | — |
| 37226 | `ServerSend66Hit` | 17 | 未移植 | — |
| 37243 | `ServerSend66Hit1` | 16 | 未移植 | — |
| 37259 | `ServerSend101Hit` | 10 | 未移植 | — |
| 37269 | `ServerSend102Hit` | 10 | 未移植 | — |
| 37279 | `ServerSend103Hit` | 10 | 未移植 | — |
| 37289 | `ServerSend113Hit` | 10 | 未移植 | — |
| 37299 | `ServerSend115Hit` | 10 | 未移植 | — |
| 37309 | `ServerSend115HitTargetEffect` | 7 | 未移植 | — |
| 37316 | `ServerSendCustomHit` | 10 | 未移植 | — |
| 37326 | `ServerSendCustomHitTargetEff` | 7 | 未移植 | — |
| 37333 | `ServerSendCustomMagicSelfKeepPlay` | 7 | 未移植 | — |
| 37340 | `ServerSendMonMove` | 10 | 未移植 | — |
| 37350 | `ServerSendHealthSpellChangedStruck` | 44 | 未移植 | — |
| 37394 | `ServerSendStruck` | 184 | 未移植 | — |
| 37578 | `ServerSendMagicshieldStruck` | 116 | 未移植 | — |
| 37694 | `ServerSendHear` | 6 | 未移植 | — |
| 37700 | `ServerSendWhisper` | 7 | 未移植 | — |
| 37707 | `ServerSendCry` | 6 | 未移植 | — |
| 37713 | `ServerSendSysMessage` | 8 | 未移植 | — |
| 37721 | `ServerSendSysMessageEx` | 8 | 未移植 | — |
| 37729 | `ServerSendGroupMessage` | 7 | 未移植 | — |
| 37736 | `ServerSendGuildMessage` | 7 | 未移植 | — |
| 37743 | `ServerSendMerchantSay` | 7 | 未移植 | — |
| 37750 | `ServerSendMoveMessage` | 6 | 未移植 | — |
| 37756 | `ServerSendMoveMessageEx` | 6 | 未移植 | — |
| 37762 | `ServerSendNewMoveMessage` | 6 | 未移植 | — |
| 37768 | `ServerSendDelayMessage` | 6 | 未移植 | — |
| 37774 | `ServerSendMoveHintMsg` | 6 | 未移植 | — |
| 37780 | `ServerSendCenterMessage` | 6 | 未移植 | — |
| 37786 | `ServerSendCenterMessageEx` | 6 | 未移植 | — |
| 37792 | `ServerSendTopChatBoardMessage` | 7 | 未移植 | — |
| 37799 | `ServerSendTopChatBoardMessageEx` | 7 | 未移植 | — |
| 37806 | `ServerSendAuctionBroadcastMsg` | 7 | 未移植 | — |
| 37813 | `ServerSendPlayDrinkSay` | 6 | 未移植 | — |
| 37819 | `ServerSendWinExp` | 21 | 未移植 | — |
| 37840 | `ServerSendUserName` | 14 | 未移植 | — |
| 37854 | `ServerSendLevelUp` | 58 | 未移植 | — |
| 37912 | `ServerSendChangeNameColor` | 6 | 未移植 | — |
| 37918 | `ServerSendSpell` | 13 | 未移植 | — |
| 37931 | `ServerSendSpell2` | 10 | 未移植 | — |
| 37941 | `ServerSendSpell3` | 7 | 未移植 | — |
| 37948 | `ServerSendMoveFail` | 6 | 未移植 | — |
| 37954 | `ServerSendDeath` | 32 | 未移植 | — |
| 37986 | `ServerSendDisppear` | 16 | 未移植 | — |

## 实现段 L38000 – L39999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 38002 | `ServerSendLogon` | 206 | 未移植 | — |
| 38208 | `ServerSendAbility` | 9 | 未移植 | — |
| 38217 | `ServerSendHealthSpellChanged` | 56 | 未移植 | — |
| 38273 | `ServerSendHPMPChangedFormStone` | 38 | 未移植 | — |
| 38311 | `ServerSendDayChangeing` | 27 | 未移植 | — |
| 38338 | `ServerSendItemShow` | 5 | 未移植 | — |
| 38343 | `ServerSendItemHide` | 5 | 未移植 | — |
| 38348 | `ServerSendDoorOpen` | 7 | 未移植 | — |
| 38355 | `ServerSendDoorClose` | 5 | 未移植 | — |
| 38360 | `ServerSendUseItems` | 5 | 未移植 | — |
| 38365 | `ServerSendWeightChanged` | 5 | 未移植 | — |
| 38370 | `ServerSendFeatureChanged` | 7 | 未移植 | — |
| 38377 | `ServerSendNationMessage` | 7 | 未移植 | — |
| 38384 | `ServerSendSetClientBuff` | 6 | 未移植 | — |
| 38390 | `ServerSendCloseClientBuff` | 6 | 未移植 | — |
| 38396 | `ServerSendShowClientBuff` | 6 | 未移植 | — |
| 38402 | `ServerSendSetArrBuff` | 6 | 未移植 | — |
| 38408 | `ServerSendCloseArrBuff` | 6 | 未移植 | — |
| 38414 | `ServerSendShowArrBuff` | 7 | 未移植 | — |
| 38421 | `ServerSendOpenBooks` | 7 | 未移植 | — |
| 38428 | `ServerSendSceneShake` | 6 | 未移植 | — |
| 38434 | `ServerSendEffectStep` | 7 | 未移植 | — |
| 38441 | `ServerSendAddButton` | 11 | 未移植 | — |
| 38452 | `ServerSendDelButton` | 6 | 未移植 | — |
| 38458 | `ServerSendAddArrButton` | 11 | 未移植 | — |
| 38469 | `ServerSendDelArrButton` | 6 | 未移植 | — |
| 38475 | `ServerSendAddNumberButton` | 6 | 未移植 | — |
| 38481 | `ServerSendDelNumberButton` | 6 | 未移植 | — |
| 38487 | `ServerSendShowPhantom` | 6 | 未移植 | — |
| 38493 | `ServerSendClosePhantom` | 6 | 未移植 | — |
| 38499 | `ServerSendClearObjects` | 5 | 未移植 | — |
| 38504 | `ServerSendChangeMap` | 88 | 未移植 | — |
| 38592 | `ServerSendButch` | 10 | 未移植 | — |
| 38602 | `ServerSendMagicFire` | 16 | 未移植 | — |
| 38618 | `ServerSendMagicFireEx` | 11 | 未移植 | — |
| 38629 | `ServerSendMagicFireEx2` | 11 | 未移植 | — |
| 38640 | `ServerSendMyMagic` | 5 | 未移植 | — |
| 38645 | `ServerSendMagicLVEXP` | 6 | 未移植 | — |
| 38651 | `ServerSendSkeleton` | 16 | 未移植 | — |
| 38667 | `ServerSendDuraChange` | 6 | 未移植 | — |
| 38673 | `ServerSendGoldChanged` | 5 | 未移植 | — |
| 38678 | `ServerSendChangeLight` | 5 | 未移植 | — |
| 38683 | `ServerSendCharStatusChanged` | 8 | 未移植 | — |
| 38691 | `ServerSendDigUp` | 18 | 未移植 | — |
| 38709 | `ServerSendDigDown` | 7 | 未移植 | — |
| 38716 | `ServerSendFlyAxe` | 16 | 未移植 | — |
| 38732 | `ServerSendLighting` | 16 | 未移植 | — |
| 38748 | `ServerSendSubAbility` | 8 | 未移植 | — |
| 38756 | `ServerSendSpaceMoveShow` | 44 | 未移植 | — |
| 38800 | `ServerSendReconnection` | 6 | 未移植 | — |
| 38806 | `ServerSendHideEvent` | 7 | 未移植 | — |
| 38813 | `ServerSendShowEvent` | 48 | 未移植 | — |
| 38861 | `ServerSendOpenHealth` | 23 | 未移植 | — |
| 38884 | `ServerSendCloseHealth` | 5 | 未移植 | — |
| 38889 | `ServerSendChangeFace` | 21 | 未移植 | — |
| 38910 | `ServerSend10205` | 7 | 未移植 | — |
| 38917 | `ServerSendAlive` | 16 | 未移植 | — |
| 38933 | `ServerSendChangeGuildName` | 5 | 未移植 | — |
| 38938 | `ServerSend10414` | 7 | 未移植 | — |
| 38945 | `ServerSendMenuOK` | 7 | 未移植 | — |
| 38952 | `ServerSendMerchantDlgClose` | 5 | 未移植 | — |
| 38957 | `ServerSendDelItemList` | 5 | 未移植 | — |
| 38962 | `ServerSendUsersRepair` | 5 | 未移植 | — |
| 38967 | `ServerSendGoodsList` | 5 | 未移植 | — |
| 38972 | `ServerSendUserSell` | 5 | 未移植 | — |
| 38977 | `ServerSendUserMakeDrugItemList` | 5 | 未移植 | — |
| 38982 | `ServerSendUserStorageItem` | 5 | 未移植 | — |
| 38987 | `ServerSendUserGetBackItem` | 5 | 未移植 | — |
| 38992 | `ServerSendSpaceMoveFire` | 13 | 未移植 | — |
| 39005 | `ServerSendBuyItemOK` | 14 | 未移植 | — |
| 39019 | `ServerSendBuyItemFail` | 5 | 未移植 | — |
| 39024 | `ServerSendDetailGoodsList` | 7 | 未移植 | — |
| 39031 | `ServerSendBuyPrice` | 9 | 未移植 | — |
| 39040 | `ServerSendUserSellItemOK` | 5 | 未移植 | — |
| 39045 | `ServerSendUserSellItemFail` | 5 | 未移植 | — |
| 39050 | `ServerSendMakeDrugOK` | 5 | 未移植 | — |
| 39055 | `ServerSendMakeDrugFail` | 5 | 未移植 | — |
| 39060 | `ServerSendRepairCost` | 5 | 未移植 | — |
| 39065 | `ServerSendUserRepairOK` | 5 | 未移植 | — |
| 39070 | `ServerSendUserRepairFail` | 5 | 未移植 | — |
| 39075 | `ServerSendPlayDice` | 11 | 未移植 | — |
| 39086 | `ServerSendAdjustBonus` | 5 | 未移植 | — |
| 39091 | `ServerSendBuildGuildOK` | 5 | 未移植 | — |
| 39096 | `ServerSendBuildGuildFail` | 5 | 未移植 | — |
| 39101 | `ServerSendDonateOK` | 5 | 未移植 | — |
| 39106 | `ServerSendGameGoldChanged` | 5 | 未移植 | — |
| 39111 | `ServerSendGamePointChanged` | 5 | 未移植 | — |
| 39116 | `ServerSendGameGlory` | 5 | 未移植 | — |
| 39121 | `ServerSendMyStaus` | 5 | 未移植 | — |
| 39126 | `ServerSendMagicFireFail` | 5 | 未移植 | — |
| 39131 | `ServerSendLampChangeDura` | 5 | 未移植 | — |
| 39136 | `ServerSendGroupCancel` | 5 | 未移植 | — |
| 39141 | `ServerSendDonateFail` | 5 | 未移植 | — |
| 39146 | `ServerSendBreakWeapon` | 5 | 未移植 | — |
| 39151 | `ServerSendPassword` | 5 | 未移植 | — |
| 39156 | `ServerSendPasswordStatus` | 7 | 未移植 | — |
| 39163 | `ServerSendClickNpcLabel` | 6 | 未移植 | — |
| 39169 | `ServerSendQueryBagItems` | 4 | 未移植 | — |
| 39173 | `ServerSendTakeOnItem` | 5 | 未移植 | — |
| 39178 | `ServerSendTakeOffItem` | 5 | 未移植 | — |
| 39183 | `ServerSendDeleteDelayMessage` | 5 | 未移植 | — |
| 39188 | `ServerSendHeroLogout` | 6 | 未移植 | — |
| 39194 | `ServerSendHeroLogon` | 18 | 未移植 | — |
| 39212 | `ServerSendGetRegInfo` | 6 | 未移植 | — |
| 39218 | `ServerSendGameGoldDalItem` | 6 | 未移植 | — |
| 39224 | `ServerSendQueryDealFail` | 6 | 未移植 | — |
| 39230 | `ServerSendPlaySound` | 5 | 未移植 | — |
| 39235 | `ServerSendStopSound` | 5 | 未移植 | — |
| 39240 | `ServerSendPlaySoundEx` | 5 | 未移植 | — |
| 39245 | `ServerSendPlaySoundExt` | 5 | 未移植 | — |
| 39250 | `ServerSendPlayEffect` | 5 | 未移植 | — |
| 39255 | `ServerSendScreenEffect` | 10 | 未移植 | — |
| 39265 | `ServerSendStopScreenEffect` | 10 | 未移植 | — |
| 39275 | `ServerSendClearScreenEffect` | 10 | 未移植 | — |
| 39285 | `ServerSendChangeSpeed` | 5 | 未移植 | — |
| 39290 | `ServerSendServerConfig` | 5 | 未移植 | — |
| 39295 | `ServerSendOpenUpgradeDlg` | 5 | 未移植 | — |
| 39300 | `ServerSendUserIcon` | 5 | 未移植 | — |
| 39305 | `ServerSendWebBrowser` | 5 | 未移植 | — |
| 39310 | `ServerSendUserEffect` | 5 | 未移植 | — |
| 39315 | `ServerSendSuperShiledEffect` | 7 | 未移植 | — |
| 39322 | `ServerSendBlastHit` | 7 | 未移植 | — |
| 39329 | `ServerSendContinuousBLASTHIT` | 7 | 未移植 | — |
| 39336 | `ServerSendNewHitBubbleDefence` | 7 | 未移植 | — |
| 39343 | `ServerSendOpenhumDlg` | 7 | 未移植 | — |
| 39350 | `ServerSendOpenHeroDlg` | 7 | 未移植 | — |
| 39357 | `ServerSendAttackMiss` | 7 | 未移植 | — |
| 39364 | `ServerSendInputMObileVerifyCode` | 6 | 未移植 | — |
| 39370 | `ServerSendVerifyCode` | 36 | 未移植 | — |
| 39406 | `ServerSendOpenUrl` | 9 | 未移植 | — |
| 39415 | `ServerSendOpenGuardianLevelDlg` | 7 | 未移植 | — |
| 39422 | `ServerSendGuardianLevelBatchInfo` | 7 | 未移植 | — |
| 39429 | `ServerSendGuardianLevelResult` | 7 | 未移植 | — |
| 39436 | `ServerSendBrokenShield` | 7 | 未移植 | — |
| 39443 | `ServerSendPoisonStruckHum` | 6 | 未移植 | — |
| 39449 | `ServerSendHumsBBChange` | 6 | 未移植 | — |
| 39455 | `ServerSendShowCustomButton` | 7 | 未移植 | — |
| 39462 | `ServerSendMagicHintMsg` | 8 | 未移植 | — |
| 39470 | `ServerSendStruckEffect` | 7 | 未移植 | — |
| 39477 | `ServerSendAddDlg` | 11 | 未移植 | — |
| 39488 | `ServerSendDelDlg` | 6 | 未移植 | — |
| 39494 | `ServerSendAddEffectPlay` | 11 | 未移植 | — |
| 39505 | `ServerSendProviewMonItem` | 5 | 未移植 | — |
| 39510 | `ServerSendWeather` | 31 | 未移植 | — |
| 39541 | `ServerSendHearColor` | 5 | 未移植 | — |
| 39546 | `ServerSendOpenPlayDrink` | 5 | 未移植 | — |
| 39551 | `ServerSendCloseDrink` | 5 | 未移植 | — |
| 39556 | `ServerSendDrinkUpdateValue` | 5 | 未移植 | — |
| 39561 | `ServerSendPlayDrinkToDrink` | 5 | 未移植 | — |
| 39566 | `ServerSendUserPlayDrink` | 5 | 未移植 | — |
| 39571 | `ServerSendStorageHeroInfo` | 6 | 未移植 | — |
| 39577 | `ServerSendStorageHeroInfoEx` | 5 | 未移植 | — |
| 39582 | `ServerSendShowHeroAutoPracticeDlg` | 5 | 未移植 | — |
| 39587 | `ServerSendRefAbilityNG` | 5 | 未移植 | — |
| 39592 | `ServerSendAbilityNG` | 15 | 未移植 | — |
| 39607 | `ServerSendAbilityAlcohol` | 6 | 未移植 | — |
| 39613 | `ServerSendAbilityMeridians` | 6 | 未移植 | — |
| 39619 | `ServerSendWinExpNG` | 4 | 未移植 | — |
| 39623 | `ServerSendLevelUpNG` | 7 | 未移植 | — |
| 39630 | `ServerSendOpenCobWebWinding` | 5 | 未移植 | — |
| 39635 | `ServerSendCloseCobWebWinding` | 5 | 未移植 | — |
| 39640 | `ServerSendOpenToxicsMoke` | 5 | 未移植 | — |
| 39645 | `ServerSendCloseToxicsMoke` | 5 | 未移植 | — |
| 39650 | `ServerSendItemDescList` | 13 | 未移植 | — |
| 39663 | `ServerSendItemDescTopList` | 13 | 未移植 | — |
| 39676 | `ServerSendTzItemDescList` | 10 | 未移植 | — |
| 39686 | `ServerSendFilterItemList` | 10 | 未移植 | — |
| 39696 | `ServerSendPlayMagicBallEffect` | 10 | 未移植 | — |
| 39706 | `ServerSendLightingEx` | 23 | 未移植 | — |
| 39729 | `ServerSendContinuousMagicOrder` | 7 | 未移植 | — |
| 39736 | `ServerSendContinuousMagicOK` | 6 | 未移植 | — |
| 39742 | `ServerSendContinuousMagicFail` | 5 | 未移植 | — |
| 39747 | `ServerSendTraingNG` | 5 | 未移植 | — |
| 39752 | `ServerSendStopContinuousMagic` | 7 | 未移植 | — |
| 39759 | `ServerSendShopName` | 5 | 未移植 | — |
| 39764 | `ServerSendHeroM2DressEffect` | 6 | 未移植 | — |
| 39770 | `ServerSendIncHealth` | 11 | 未移植 | — |
| 39781 | `ServerSendAttack01` | 6 | 未移植 | — |
| 39787 | `ServerSendAttack02` | 6 | 未移植 | — |
| 39793 | `ServerSendAttack03` | 6 | 未移植 | — |
| 39799 | `ServerSendAttack04` | 6 | 未移植 | — |
| 39805 | `ServerSendAttack05` | 6 | 未移植 | — |
| 39811 | `ServerSendAttack06` | 6 | 未移植 | — |
| 39817 | `ServerSendOpenGameShop` | 7 | 未移植 | — |
| 39824 | `ServerSendArmRemoveStone` | 6 | 未移植 | — |
| 39830 | `ServerSendSetNpcImage` | 6 | 未移植 | — |
| 39836 | `ServerSendUserBigGetBackItem` | 5 | 未移植 | — |
| 39841 | `ServerSendThunderPalsyEff` | 7 | 未移植 | — |
| 39848 | `ClientAutoGJ` | 24 | 未移植 | — |
| 39872 | `ClientGJCallMonMagic` | 9 | 未移植 | — |
| 39881 | `ClientQuerySelectHeroM2ShopInfo` | 72 | 未移植 | — |
| 39953 | `ServerSendHeroM2StartShopStall` | 83 | 未移植 | — |

## 实现段 L40000 – L41999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 40036 | `ServerSendHeroM2StopShopStall` | 35 | 未移植 | — |
| 40071 | `ServerSendHeroM2BuyUserItem` | 340 | 未移植 | — |
| 40411 | `ServerSendHeroM2AddUserItem` | 256 | 未移植 | — |
| 40667 | `ServerSendHeroM2DelUserItem` | 51 | 未移植 | — |
| 40718 | `ServerSendHeroM2CloseShop` | 16 | 未移植 | — |
| 40734 | `ClientButtonClick` | 9 | 未移植 | — |
| 40743 | `ClientNumberButtonClick` | 9 | 未移植 | — |
| 40752 | `ClientArrButtonClick` | 9 | 未移植 | — |
| 40761 | `ClientClientBuffClick` | 9 | 未移植 | — |
| 40770 | `ClientArrBuffClick` | 9 | 未移植 | — |
| 40779 | `ClientAutoFindPath` | 16 | 未移植 | — |
| 40795 | `ClientPlugInConfig` | 11 | 未移植 | — |
| 40806 | `ClientTakeHorse` | 98 | 未移植 | — |
| 40904 | `ClientInviteHorse` | 92 | 未移植 | — |
| 40996 | `ClientResponseInviteHorse` | 64 | 未移植 | — |
| 41060 | `ClientOpenJewelryBox` | 9 | 未移植 | — |
| 41069 | `ClientHeroOpenJewelryBox` | 11 | 未移植 | — |
| 41080 | `ClientTakeOnJewelry` | 190 | 未移植 | — |
| 41270 | `ClientTakeOffJewelry` | 73 | 未移植 | — |
| 41343 | `ClientSwapJewelryItem` | 17 | 未移植 | — |
| 41360 | `ClientHeroTakeOnJewelry` | 184 | 未移植 | — |
| 41544 | `ClientHeroTakeOffJewelry` | 100 | 未移植 | — |
| 41644 | `ClientHeroSwapJewelryItem` | 18 | 未移植 | — |
| 41662 | `ClientSetShowFashion` | 40 | 未移植 | — |
| 41702 | `ClientMoveToItemBox` | 90 | 未移植 | — |
| 41792 | `ClientMoveItemBoxToBag` | 55 | 未移植 | — |
| 41847 | `ClientClearItemBox` | 8 | 未移植 | — |
| 41855 | `ClientGodBlessItemClick` | 18 | 未移植 | — |
| 41873 | `ClientTakeOnGodBless` | 431 | 未移植 | — |

## 实现段 L42000 – L43999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 42304 | `ClientTakeOffGodBless` | 180 | 未移植 | — |
| 42484 | `ClientGodBlessUpgradeClick` | 15 | 未移植 | — |
| 42499 | `ClientDActionLogClick` | 8 | 未移植 | — |
| 42507 | `ClientUpdateActiveFengHao` | 79 | 未移植 | — |
| 42586 | `ClientUpgradeMagic` | 7 | 未移植 | — |
| 42593 | `ClientUpgradeHeroMagic` | 7 | 未移植 | — |
| 42600 | `DoItemFluteStone` | 179 | 未移植 | — |
| 42779 | `ClientBagUseItem` | 48 | 未移植 | — |
| 42827 | `ClientRemoveStone` | 169 | 未移植 | — |
| 42996 | `ClientClickBox` | 49 | 未移植 | — |
| 43045 | `ClientRequestStdItem` | 18 | 未移植 | — |
| 43063 | `ClientGetSayItem` | 24 | 未移植 | — |
| 43087 | `ClientSendUserVerifyFail` | 17 | 未移植 | — |
| 43104 | `ClientGetM2VerifyCode` | 18 | 未移植 | — |
| 43122 | `ClientCheckM2VerifyCode` | 36 | 未移植 | — |
| 43158 | `ClientGetMobileVerifyCode` | 30 | 未移植 | — |
| 43188 | `ClientCheckMobileVerifyCode` | 45 | 未移植 | — |
| 43233 | `ClientCloseM2VerifyCode` | 10 | 未移植 | — |
| 43243 | `ClientQueryAllAuctionItems` | 84 | 未移植 | — |
| 43327 | `ClientQueryMyAuctionItems` | 51 | 未移植 | — |
| 43378 | `ClientQueryMyAttentionItems` | 54 | 未移植 | — |
| 43432 | `ClientAddAuctionItem` | 262 | 未移植 | — |
| 43694 | `ClientCancelMyAuctionItem` | 42 | 未移植 | — |
| 43736 | `ClientDeleteMyAuctionItem` | 36 | 未移植 | — |
| 43772 | `ClientRetrieveMyAuctionItem` | 136 | 未移植 | — |
| 43908 | `ClientAuctionAttentionItem` | 45 | 未移植 | — |
| 43953 | `ClientAuctionItemBid` | 587 | 未移植 | — |

## 实现段 L44000 – L45999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 44540 | `ClientRequestAuctionItemsByIndex` | 63 | 未移植 | — |
| 44603 | `ClientAddItemToJar` | 93 | 未移植 | — |
| 44696 | `ClientGameLevelItemGet` | 160 | 未移植 | — |
| 44856 | `ClientGameLevelButtonClick` | 20 | 未移植 | — |
| 44876 | `ClientResponseGorupJoin` | 101 | 未移植 | — |
| 44977 | `ClientCustomButtonClick` | 10 | 未移植 | — |
| 44987 | `ClientDlgButtonClick` | 6 | 未移植 | — |
| 44993 | `ClientCloseBagItemClick` | 6 | 未移植 | — |
| 44999 | `ClientMinMapCustomButtonClick` | 6 | 未移植 | — |
| 45005 | `DoAddSellPlayer` | 91 | 未移植 | — |
| 45096 | `ClientAddSellPlayer` | 180 | 未移植 | — |
| 45276 | `ClientAddSellPlayerAskRet` | 52 | 未移植 | — |
| 45328 | `ClientAddSellPlayerAskConfirmRet` | 20 | 未移植 | — |
| 45348 | `ClientDelSellPlayer` | 21 | 未移植 | — |
| 45369 | `SendQuerySellPlayerShopItem` | 213 | 未移植 | — |
| 45582 | `ClientQuerySellPlayerShopItem` | 12 | 未移植 | — |
| 45594 | `ClientGetSellPlayerShopItem` | 9 | 未移植 | — |
| 45603 | `ClientViewSellPlayerInfo` | 58 | 未移植 | — |
| 45661 | `ClientViewSellPlayerStorage` | 34 | 未移植 | — |
| 45695 | `SendViewSellPlayerFengHao` | 52 | 未移植 | — |
| 45747 | `SendViewSellPlayerMagicList` | 38 | 未移植 | — |
| 45785 | `SendViewSellPlayerGamePetList` | 40 | 未移植 | — |
| 45825 | `SendViewSellPlayerGamePetBagItemList` | 35 | 未移植 | — |
| 45860 | `SendViewSellPlayerInfo` | 153 | 未移植 | — |

## 实现段 L46000 – L47999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 46013 | `SendViewSellPlayerBagItems` | 47 | 未移植 | — |
| 46060 | `SendViewSellPlayerStorageItems` | 135 | 未移植 | — |
| 46195 | `SendViewSellPlayerHeroInfo` | 135 | 未移植 | — |
| 46330 | `ClientBuySellPlayer` | 170 | 未移植 | — |
| 46500 | `ClientUploadPickItmes` | 55 | 未移植 | — |
| 46555 | `ClientGamePetSelectChange` | 16 | 未移植 | — |
| 46571 | `ClientValueCrcError` | 7 | 未移植 | — |
| 46578 | `ClientGuide` | 9 | 未移植 | — |
| 46587 | `ClientUpdateTask` | 10 | 未移植 | — |
| 46597 | `ClientBagUseMode47Item` | 148 | 未移植 | — |
| 46745 | `ServerInviteHorse` | 6 | 未移植 | — |
| 46751 | `ServerTakeHorse` | 6 | 未移植 | — |
| 46757 | `ServerSyncScreen` | 7 | 未移植 | — |
| 46764 | `ServerUpdateCollect` | 12 | 未移植 | — |
| 46776 | `ServerSendScreenMessage` | 7 | 未移植 | — |
| 46783 | `ServerSendSuperMoveMessage` | 6 | 未移植 | — |
| 46789 | `ServerSendSuperMoveMessageEx` | 7 | 未移植 | — |
| 46796 | `ServerSendNewLineMessage` | 6 | 未移植 | — |
| 46802 | `ServerSendNewLineMessageEx` | 6 | 未移植 | — |
| 46808 | `DoClientClose` | 183 | 未移植 | — |
| 46991 | `ProcessUseItemSkill` | 274 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:652 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:717 |
| 47265 | `DoClientAction` | 9 | 未移植 | — |
| 47274 | `ProcessSafeZoneHint` | 39 | 未移植 | — |
| 47313 | `SendJewelryBox` | 49 | 未移植 | — |
| 47362 | `SendUpdateGodBless` | 51 | 未移植 | — |
| 47413 | `SendOpenGodBlessItem` | 36 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:662 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:704 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:727 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:773 |
| 47449 | `SendCloseGodBlessItem` | 6 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:663 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:705 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:728 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:774 |
| 47455 | `SendAddFengHaoItem` | 21 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:668 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:716 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:733 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:785 |
| 47476 | `SendDelFengHaoItem` | 67 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:669 GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:717 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:734 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:786 |
| 47543 | `SendUpdateFengHao` | 44 | 未移植 | — |
| 47587 | `SendGamePetList` | 45 | 未移植 | — |
| 47632 | `SendAddGamePet` | 23 | 未移植 | — |
| 47655 | `SendDelGamePet` | 6 | 未移植 | — |
| 47661 | `SendCurrentRecalGamePetIndex` | 6 | 未移植 | — |
| 47667 | `GetGamePetMagicCount` | 17 | 未移植 | — |
| 47684 | `GamePetAddMagic` | 46 | 未移植 | — |
| 47730 | `GamePetToBag` | 73 | 未移植 | — |
| 47803 | `SendGamePetBagItemList` | 35 | 未移植 | — |
| 47838 | `SendUpdatePetItem` | 16 | 未移植 | — |
| 47854 | `SendAddPetItem` | 16 | 未移植 | — |
| 47870 | `SendUpdatePetAbility` | 28 | 未移植 | — |
| 47898 | `SendUpdatePetAbilityEx` | 39 | 未移植 | — |
| 47937 | `SendDeletePetItems` | 14 | 未移植 | — |
| 47951 | `SendSetPetMagic` | 6 | 未移植 | — |
| 47957 | `SendClearPetMagic` | 6 | 未移植 | — |
| 47963 | `SendStorageOpenStatus` | 7 | 未移植 | — |
| 47970 | `StartCollect` | 35 | 未移植 | — |

## 实现段 L48000 – L49999

| 原文行 | 例程 | 行数 | 状态 | 托管落点 / 同名命中出处 |
|---|---|---|---|---|
| 48005 | `StopCollect` | 31 | 未移植 | — |
| 48036 | `StopCollectEx` | 36 | 未移植 | — |
| 48072 | `DelBoxItem` | 76 | 未移植 | — |
| 48148 | `ReturnBoxItem` | 58 | 未移植 | — |
| 48206 | `UpdateBoxItem` | 36 | 未移植 | — |
| 48242 | `GetMasterNoList` | 80 | 未移植 | — |
| 48322 | `SaveMasterNoList` | 33 | 未移植 | — |
| 48355 | `AddMaster` | 12 | 未移植 | — |
| 48367 | `DelMaster` | 23 | 未移植 | — |
| 48390 | `MasterNoListQuickSort` | 63 | 未移植 | — |
| 48453 | `SendSocketStatusFail` | 8 | 近似物/同名 | GXX.M2Server\Plugins\PluginInterfaceManaged.g.cs:670 GXX.M2Server\Plugins\PluginInterfaceTables.g.cs:735 |
| 48461 | `GetFeature_New` | 217 | 未移植 | — |
| 48678 | `CanUseCallMonMagic` | 83 | 未移植 | — |
| 48761 | `SendClientDataFile` | 77 | 未移植 | — |
| 48838 | `GetItemInfo` | 60 | 未移植 | — |
| 48898 | `IsEnoughPetBag` | 7 | 未移植 | — |
| 48905 | `AddItemToPetBag` | 10 | 未移植 | — |
| 48915 | `ClearNpcInputText` | 10 | 未移植 | — |
| 48925 | `ClearSendJoinUserList` | 10 | 未移植 | — |
| 48935 | `SendJoinUserListToClient` | 29 | 未移植 | — |
| 48964 | `ClientUseItmeToWealthAnimalMon` | 139 | 未移植 | — |
| 49103 | `DelayClose` | 7 | 近似物/同名 | GXX.GatewayKit\Rest11\Rest11LoginGateSession.cs:153 GXX.LoginGate\Rest11\Rest11LoginGateKernel.cs:223 GXX.RunGate\MirClientContext.cs:1567 |
| 49110 | `SendPreviewMonItem` | 122 | 未移植 | — |


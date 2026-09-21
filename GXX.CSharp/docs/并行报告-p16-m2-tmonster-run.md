# 并行报告 · 车道 `p16-m2-tmonster-run`

> 任务：把 `TMonster.Run` 从 main 上的 **18 行近似物**改写为 `ObjMon.pas:1121-1379`（**259 行**）的 1:1 移植，
> 并补回前序只读复核车道标定的**四条缺失分支**。
>
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p16-m2-tmonster-run`
> 分支：`par/p16-m2-tmonster-run`（main @ `4a331324`）

---

## 1. 交付摘要

| 项 | 值 |
|---|---|
| 改动文件 | **3**（`Engine/ObjBase.cs` 改写 + 2 个新建测试文件） |
| 新增用例 | **80**（74 个 `[Fact]`/`[Theory]` 方法 ⇒ `MonsterRunTests` 78 例 + `MonsterRunHarness` 2 例；连跑 5 次稳定） |
| `Run` 体量 | 旧 18 行 → 新 **259 行原文行**（托管方法体 **约 250 行**） |
| 四条缺失分支 | **4/4 落地**（① 天关宝宝 `MakeGhost` ② 镜像 `SpaceMove` ③ `m_boWalkWaitLocked` ④ 宠物拴物 + `TSmartObject` 范围拾取） |
| `NotPorted` 留痕 | **5 条**（`Think` / `AttackTarget` / `SpaceMove` / `PickRangeItem` / `StartPickUpItem`） |
| 偏离登记 | **D-P16-01 … D-P16-07**（见 §7） |
| 未完成/阻塞 | 见 §8（**无编译/门禁阻塞**；5 条接缝待各自切片接线） |

---

## 2. 逐段对账表（原文档位区间 → 托管实现位置 → 真实体/NotPorted/原文如此）

托管实现位置一律指 `GXX.CSharp/src/GXX.M2Server/Engine/ObjBase.cs`（本次改动后）。行号为**本次交付后的实际行号**。

| # | 原文区间（`ObjMon.pas`） | 原文内容 | 托管落点 | 定性 |
|---|---|---|---|---|
| 1 | 1120 | 版次注释 `修正宝宝的移动速度影响其攻击速度 2019-09-26 00:26:01` | 方法 `<summary>` 引用 | 原文如此（注释） |
| 2 | 1122-1126 | 四个局部变量（`nX,nY,nMinRange,IsCanMove,boEnabledPetPickup,SmartObject`） | 方法体首 4 行声明 | 真实体 |
| 3 | 1128 | 五重守卫 `not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove` | `if (!m_boGhost && !m_boDeath && !m_boFixedHideMode && !m_boStoneMode && CanMoveMode())` | 真实体（条件组合逐字一致；`CanMove` 走接缝，D-P16-01） |
| 4 | 1130-1137 | 天关宝宝：跨图 + `m_PEnvir.m_boGuardianLevel` → `MakeGhost; Exit` | 同序 `if` + `EnvirGuardianLevel` + `MakeGhost(); return;` | **真实体（补回分支①）** |
| 5 | 1138-1142 | 镜像：跨图 + `m_PEnvir.m_boMirror` → `SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1); Exit` | 同序 `else if` + `EnvirMirror` + `SpaceMove(...)` | **真实体（补回分支②）** |
| 6 | 1144-1148 | `if Think then begin inherited; Exit; end` | `if (Think()) { base.Run(); return; }` | 真实体（`Think` 走接缝，D-P16-01） |
| 7 | 1149-1155 | `m_boWalkWaitLocked` 解锁（**裸减法**、严格 `>`） | 同序 `if` / `(GetTickCount() - m_dwWalkWaitTick) > m_dwWalkWait` | **真实体（补回分支③）** |
| 8 | 1156 | 注释 `不确定此处的修改会不会让系统的怪物攻击速度变得更快 2019-09-26 00:31:41` | 同行注释保留 | 原文如此 |
| 9 | 1157 | `IsCanMove := (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay)` | `IsCanMove = TickDiff(...) > (uint)(m_nWalkSpeed + m_nWalkDelay)` | 真实体（**严格 `>`**；`TickDiff` 1:1 照 `M2Share.pas:32953-32959`） |
| 10 | 1158 | 注释 `提高宠物拴物速度 2019-12-19 11:20:40` | 保留 | 原文如此 |
| 11 | 1159-1171 | 宠物快速拾取（四条件 + `boEnabledPetPickup` + `boPetRangePickup`） | 同序 `if` 块 | **真实体（补回分支④前半）** |
| 12 | 1172 | `if not m_boWalkWaitLocked then` | `if (!m_boWalkWaitLocked)` | 真实体 |
| 13 | 1174-1185 | `IsCanMove` → 刷新 `m_dwWalkTick`/清零 `m_nWalkDelay`/`Inc(m_nWalkCount)`/超 `m_nWalkStep` 上锁 | 同序逐句 | 真实体 |
| 14 | 1186-1190 | 主人休息 ⇒ `DelTargetCreat; m_boTarget := False` | 同序 | 真实体（`m_boSlaveRelax` 走接缝，D-P16-04） |
| 15 | 1191-1276 | `not m_boRunAwayMode` → `not m_boNoAttackMode` → 攻击支 / `IsCanMove` 支（任务点 1207-1220、宠物拴物 1223-1251、`TSmartObject` 1253-1273） | 同序同层 | **真实体（含补回分支④后半）** |
| 16 | 1277-1326 | `IsCanMove and (m_Master <> nil)` → 目标超距删目标 / 无目标取主人回位点 / 155·156 与 `moNoMove` 豁免 / 1305 占位回退 / 1314-1325 `SpaceMove` | 同序同层 | 真实体 |
| 17 | 1327-1335 | `else` → `m_boRunAwayMode` 超时解除（**裸减法**） | 同序 | 真实体（**原文缺陷**，见 §6-②） |
| 18 | 1336-1340 | 主人休息 ⇒ `inherited; Exit` | 同序 `base.Run(); return;` | 真实体 |
| 19 | 1341-1374 | `IsCanMove` → `m_nTargetX <> -1` → 自定义怪最小距离三分支 / `GotoTargetXY` / `Wondering` | 同序 | 真实体（自定义怪判据走接缝，D-P16-01） |
| 20 | 1376 | 块尾注释 `// 004A93D8 if not bo510 and ...` | 保留 | 原文如此 |
| 21 | 1378 | 无条件 `inherited;` | 方法末 `base.Run();` | 真实体（**在守卫之外** —— 原文如此，见 §6-①） |
| — | 845-886 | `TMonster.Think`（`Run` 在 1144 调用） | `IMonsterRunWorld.Think` 接缝 | **NotPorted**（`Think@ObjMon.pas:1144`） |
| — | 888-932 | `TMonster.AttackTarget`（`Run` 在 1198 调用） | `IMonsterRunWorld.AttackTarget` 接缝 | **NotPorted**（`AttackTarget@ObjMon.pas:1198`） |
| — | `ObjBase.pas:22448` | `TBaseObject.SpaceMove`（`Run` 在 1140/1235/1323 调用） | `IMonsterRunWorld.SpaceMove` 接缝 | **NotPorted**（`SpaceMove@ObjMon.pas:1140`） |
| — | `ObjBase.pas` 范围拾取族 | `PickRangeItem` / `StartPickUpItem` | `IMonsterRunWorld` 两个接缝 | **NotPorted**（行号 1166 / 1169） |
| — | `ObjBase.pas:2599-2657` | `TBaseObject.GetBackPosition(out nX,out nY)`（`Run` 在 1232/1296 调用） | `MonsterRunBackPosition.GetBackPosition`（**本文件内 1:1**） | 真实体（另两条重载未落地，D-P16-03） |
| — | `ObjBase.pas:854` | `GotoTargetXY`（`Run` 五处调用） | `TMonster.GotoTargetXY()`（最小 1:1 语义） | 真实体（D-P16-02） |
| — | `ObjBase.pas:856/7607` | `Wondering`（`Run` 在 1372 调用） | `TMonster.Wondering()` | 真实体（D-P16-02） |
| — | `M2Share.pas:32953-32959` | `tick_diff` | `TMonster.TickDiff` | 真实体（1:1，含回绕） |
| — | `Grobal2.pas` | `DR_UP`..`DR_UPLEFT` / `RC_PLAYOBJECT` / `RC_HEROOBJECT` | `Grobal2Const.*`（既有） | 真实体（复用，未新造） |
| — | `M2Share.pas:3065` | 四参 `GetNextDirection` | `TCreature.GetNextDirection`（`Engine/MagicModel.cs:179`） | 真实体（**复用既有**，D-P16-05） |
| — | `M2Share.pas:10818-10839` | 一参 `GetNextDirection`（旋转） | 未用到（`Run` 不调用） | — |

### 2.1 四条补回分支的落实证据

| 分支 | 原文 | 托管落点 | 主用例 |
|---|---|---|---|
| ① `m_Master` 天关宝宝 `MakeGhost` | 1130-1137 | `ObjBase.cs` 天关分支 | `GuardianLevelCrossMapMakesGhost`、`GuardianLevelSameMapDoesNotMakeGhost`、`GuardianLevelFalseCrossMapDoesNotMakeGhost`、`GuardianLevelOnlyGateOfItsBranch` |
| ② 镜像地图 `SpaceMove` | 1138-1142 | `ObjBase.cs` 镜像分支 | `MirrorCrossMapSpaceMovesToMastersMap`、`MirrorFalseCrossMapDoesNotSpaceMove`、`MirrorSameMapDoesNotSpaceMove` |
| ③ `m_boWalkWaitLocked` 走步等待锁 | 1149-1155 / 1172 / 1174 | `ObjBase.cs` 解锁段 + `!m_boWalkWaitLocked` 闸门 | `WalkWaitLockBlocksTickRefreshEvenWhenIsCanMoveIsTrue`、`WalkWaitLockExpiresByTime`、`WalkWaitExpiryIsStrictGreaterThan`、`WalkCountExceedingStepLocksAndResets`、`WalkCountNotExceedingStepDoesNotLock` |
| ④ 宠物拴物 + `TSmartObject` 范围拾取 | 1159-1171 / 1223-1273 | `ObjBase.cs` 快速拾取段 + 宠物拴物段 + `TSmartObject` 段 | `PetQuickPickupRangePickupReturnsExit`、`PetRangePickupFalseSkipsPickRangeItemOnly`、`PetQuickPickupFalseSkipsWholeBlock`、`PetQuickPickupRequiresGamePetFlag`、`PetQuickPickupRequiresMasterBePlayerObject`、`PetEnablePickThreeStateField`(4 例)、`StartPickUpItemReturnValueDiscardedAt1169`、`GamePetFarFromMasterSpaceMovesToBackPosition`、`GamePetNearMasterDoesNotSpaceMove`、`GamePetCrossMapSpaceMovesEvenWhenNear`、`GamePetRangePickupFalseUsesThreeArgStartPickUp`、`SmartObjectSlaveAutoPickReadsMastersOwnPickFields`、`MapNoAutoRangePickItemBlocksSmartObjectPickup`、`SlaveAutoPickItemOffBlocksSmartObjectPickup`、`VmProtectKeyOffBlocksSmartObjectPickup`、`SlaveAutoPickItemRangeZeroSkipsOnlyRangePickup`、`SmartObjectBranchAcceptsHeroMasterToo` |

### 2.2 tick 边界（`>` vs `>=` 各一条 —— 任务硬要求）

| 口径 | 原文位置 | 用例 | 断言 |
|---|---|---|---|
| **`>`**（严格大于）| 1157 `tick_diff(m_dwWalkTick, now) > m_nWalkSpeed + m_nWalkDelay` | `IsCanMoveUsesStrictGreaterThanNotGreaterOrEqual` | 差值**恰好等于**阈值 ⇒ `IsCanMove` 为假 ⇒ `m_nWalkCount` 不变 |
| **`>`** 对照 | 同上 | `IsCanMoveOneMillisecondOverThresholdPasses` | 差值 = 阈值 + 1 ⇒ `m_nWalkCount` 自增 |
| **`>`**（时间锁）| 1151 `(now - m_dwWalkWaitTick) > m_dwWalkWait` | `WalkWaitExpiryIsStrictGreaterThan` | 差值 == 阈值 ⇒ **不解锁** |
| **`>=`**（同族对照）| `ObjMon.pas:1393+`（`TChickenDeer.Run`）与 `ObjMon.pas:2062`（`TIcicleMonster`）用 `>=` | `ObjMonChickenDeerRealTests`（既有，`CountIn("... >= ...")`）| 原文两处口径不同，本车道**只**把 `TMonster.Run` 一侧锁成 `>`；已用 `TickDiffIsWrapSafeUnlikeRawSubtraction` + `TickDiffMatchesOriginalDefinition` 把 `tick_diff` 自身锁死 |

> 说明：本任务要求"`>` 与 `>=` 各有一条"。**`TMonster.Run`（1121-1379）体内不存在任何 `>=`**
> （逐行核对：比较只有 1128/1144/1149/1157/1172/1179/1186/1191/1193/1195/1207/1215/1223/1228/
> 1253/1256/1258/1260/1277/1280/1282/1288/1290/1297/1301/1303/1305/1314/1318/1321/1330/1336/
> 1338/1341/1343/1346/1349/1353/1355/1357/1361/1371，全部是 `=`/`<>`/`>`/`<=`/`<`）。
> 故"`>=`"这一侧由**同族原文**（`TChickenDeer.Run`/`TIcicleMonster.Run`，均用 `>=`）承担，
> 并与 `TMonster.Run` 的 `>` 形成**差异断言**：既有的 `ObjMonChickenDeerRealTests:238`
> 已断言 `TChickenDeer.Run` 段内 `>` 形态为 **0 处**、`>=` 形态为 **1 处**；
> 本车道新增用例 `IsCanMoveUsesStrictGreaterThanNotGreaterOrEqual` 把 `TMonster.Run` 一侧
> 锁成"恰好等于阈值时不动作"。两侧合起来即"`>` 与 `>=` 各有其锁"。

### 2.3 "其它全开只关它"的防漏断言（每加一个守卫字段都要证明它真的在起作用）

| 守卫 | 用例 | "只关它"的做法 |
|---|---|---|
| 1128 `m_boGhost` | `EachGuardFieldAloneBlocksTheBody("ghost")` | 其余全开，只置 `m_boGhost` |
| 1128 `m_boDeath` | 同上 `("death")` | 只置 `m_boDeath` |
| 1128 `m_boFixedHideMode` | 同上 `("fixedHide")` | 只置 `m_boFixedHideMode` |
| 1128 `m_boStoneMode` | 同上 `("stone")` | 只置 `m_boStoneMode` |
| 1128 `CanMove` | 同上 `("canMove")` | 只让接缝返回 `false` |
| 1133 `m_boGuardianLevel` | `GuardianLevelFalseCrossMapDoesNotMakeGhost`、`GuardianLevelOnlyGateOfItsBranch` | 只关地图视图的天关开关 |
| 1133 跨图条件 | `GuardianLevelSameMapDoesNotMakeGhost` | 只把主人放回同图 |
| 1138 `m_boMirror` | `MirrorFalseCrossMapDoesNotSpaceMove` | 只关镜像开关 |
| 1138 跨图条件 | `MirrorSameMapDoesNotSpaceMove` | 只把主人放回同图 |
| 1144 `Think` | `ThinkFalseContinues` | 只让接缝返回 `false` |
| 1151 `m_dwWalkWait` | `WalkWaitExpiryIsStrictGreaterThan` | 只让差值恰好等于阈值 |
| 1159 `m_boGamePet` | `PetQuickPickupRequiresGamePetFlag` | 只清 `m_boGamePet` |
| 1159 `boPetQuickPickup` | `PetQuickPickupFalseSkipsWholeBlock` | 只关该配置 |
| 1159 主人种族 | `PetQuickPickupRequiresMasterBePlayerObject` | 只把主人改成英雄 |
| 1159 三态字段 | `PetEnablePickThreeStateField`(4 例) | 只改 `m_btGamePetEnablePick` |
| 1164 `boPetRangePickup` | `PetRangePickupFalseSkipsPickRangeItemOnly` | 只关该配置 |
| 1193 `m_boNoAttackMode` | `NoAttackModeSkipsAttackBranchEntirely` | 只置该字段 |
| 1195 站稳阈值 | `NotStationedLongEnoughSkipsAttack` | 只把 `m_nWalkSpeed` 调成巨值 |
| 1195 目标存在 | `NoTargetSkipsAttackBranch` | 只清 `m_TargetCret` |
| 1207 `m_boMission` | `MissionOffFallsIntoPickupBranch` | 只关任务模式（点还在） |
| 1207 点数 > 0 | `MissionEmptyPointsFallsIntoPickupBranch` | 只清空点表 |
| 1228 距离/跨图 | `GamePetNearMasterDoesNotSpaceMove`（关距离）、`GamePetCrossMapSpaceMovesEvenWhenNear`（只让跨图成立） | 分别只动一个析取项 |
| 1240 `boPetRangePickup` | `GamePetRangePickupFalseUsesThreeArgStartPickUp` | 只关该配置 |
| 1256 `m_boNoAutoRangePickItem` | `MapNoAutoRangePickItemBlocksSmartObjectPickup` | 只开地图禁止位 |
| 1258 `m_boSlaveAutoPickItem` | `SlaveAutoPickItemOffBlocksSmartObjectPickup` | 只清该字段 |
| 1258 `g_nKey_UseClientPickItems` | `VmProtectKeyOffBlocksSmartObjectPickup` | 只把键置 0 |
| 1260 `m_btSlaveAutoPickItemRange` | `SlaveAutoPickItemRangeZeroSkipsOnlyRangePickup` | 只把范围置 0 |
| 1282 目标超距/跨图 | `NearbyTargetNotDeleted` | 只把目标放到主人身边 |
| 1290 `155 and 156` | `Guardian155_156SkipsBackPosition`、`RaceServer155NeedsRaceImg156` | 分别只改其中一个字面量 |
| 1290 `moNoMove` 自定义怪 | `NoMoveCustomMonsterSkipsBackPosition` | 只让接缝返回真 |
| 1305 占位回退 | `OccupiedBackPositionRevertsToSelf` / `FreeBackPositionKeepsMasterBackPosition` | 只切换占用集合 |
| 1321 两个分量 | `MinusOneTargetBlocksFinalSpaceMove`（X、Y 均 -1）、`FinalSpaceMoveRequiresBothTargetComponents`（只 Y 为 -1） | 分别只动一个分量 |
| 1330 `m_dwRunAwayTime > 0` | `RunAwayTimeZeroNeverExpires` | 只把时间置 0（超时点古老） |
| 1330 超时 | `RunAwayTimeoutClearsBothFields` | 只把起点推早 |
| 1191 `m_boRunAwayMode` | `RunAwayModeSkipsWholeMovementBlock` | 只置该字段（并打断 1330 的超时） |
| 1346 自定义怪 | `CustomMonsterEqualAxesInsideMinRangeDoesNotGoto` | 只让"两轴相等"成立 |
| 1355 距离 > `nMinRange` | `CustomMonsterNilTargetStillGoto` | 只清 `m_TargetCret` |
| 1371 `m_TargetCret = nil` | `NoTargetXAndNoTargetWonders` | 只清 `m_TargetCret`（目标点也 -1） |
| 1348 `nMinRange <= 1` | `CustomMonsterMinRangeOneShortcut` | 只把最小距离置 1 |

---

## 3. 新增接缝清单（为什么不能直连）

| # | 接缝 | 被接缝的成员 | 为什么不能直连（已实测） | 未接线时的后果 |
|---|---|---|---|---|
| S1 | `IMonsterRunWorld.CanMove` | 原文 1128 的 `CanMove` | 托管全树**没有** `TCreature/TMonster.CanMove`（`src/**` 非注释命中 = 0）；原文它在 `TBaseObject` 上（`ObjBase.pas`），属 `ObjBase.CanMove` 自己的切片 | `TMonster.DefaultCanMove`（初值 `true`，与原文"可行动"同语义） |
| S2 | `IMonsterRunWorld.Think` | `TMonster.Think`（845-886，42 行） | 运行时侧不存在；在 `Run` 里重写 = **第二份实现**，本工程明令禁止 | `Think()` 恒 `false` ⇒ 不早退（原文"Think 返回假"的后果） |
| S3 | `IMonsterRunWorld.AttackTarget` | `TMonster.AttackTarget`（888-932，45 行） | 同上 | 恒 `false` ⇒ 不早退 |
| S4 | `IMonsterRunWorld.SpaceMove` | `TBaseObject.SpaceMove`（`ObjBase.pas:22448`） | 跨图移动要改 `m_PEnvir` 归属 + 地图对象表 + 广播；`Engine/Envir.cs` 不在本车道分区 | 空操作（**登记为 NotPorted**） |
| S5 | `IMonsterRunWorld.PickRangeItem` / `StartPickUpItem` | `TBaseObject` 范围拾取族 | 托管全树**没有**运行时定义（`PickRangeItem`/`StartPickUpItem` 非注释命中 = 0） | 恒 `false` ⇒ 不早退（**登记为 NotPorted**） |
| S6 | `IMonsterRunEnvirView` | `m_PEnvir.m_boGuardianLevel` / `m_boMirror` / `m_boNoAutoRangePickItem` / `GetMovingObject` | `Engine.TEnvirnoment`（`Envir.cs:80`）**四项全无**，而该文件**不在本车道分区** | 三个开关 `false`、`GetMovingObject` 返回 `null`（= 原文这些开关关闭时的取值） |
| S7 | `IMonsterRunConfig` | `g_Config.boPetQuickPickup` / `boEnabledPetPickup` / `boPetRangePickup` / `boPetSleepControlBySlave` / `g_nKey_UseClientPickItems` / `Self is TCustomMonster` 一族 | `M2Config` 是**静态类**、且不在本车道分区；`TCustomMonster` 类型在托管运行时侧**不存在**（全树 0 处） | **生产默认走 `M2ConfigMonsterRunConfig`**（直读真配置）；`g_nKey_UseClientPickItems` 返回 **1**（= 原文 `M2Share.pas:3887` 的正式版默认值）；自定义怪三项返回"非自定义怪"默认值（与原文该对象不是自定义怪同构） |
| S8 | `TCreature.SlaveRelaxSeam` | 原文四处 `m_Master.m_boSlaveRelax`（1186/1228/1318/1336） | `m_Master` 静态类型是 `TCreature`，而托管侧 `m_boSlaveRelax` **只声明在 `TScriptPlayer`**（`Engine/NpcScriptState.cs:23`）—— `TPlayObject` 上根本没有该字段（全树仅注释里出现） | `false`（= 原文"主人没有让宝宝休息"） |
| S9 | `TCreature.MakeGhost()`（**不是接缝，是实现上移**） | 原文 `TBaseObject.MakeGhost`（`ObjBase.pas:2558`） | 托管侧它**只在 `TPlayObject` 上**（`ObjBase.OnlineMsg.cs:55`），`TMonster` 够不到（编译期 CS0103） | 已按**逐字相同**的实现上移到 `TCreature`（派生类同名方法隐藏它，行为不变） |

> **登记纪律**：S2/S3/S4/S5 同时写入 `TMonster.NotPortedClaims`（`NotPorted(名, 原文行号)` 格式），
> 并由 `NotPortedClaimsAreExplicitAndFormatted` 断言格式与条目数（5 条）。
> `NullRunDepsIsSafe` 断言接缝全未接线时**不崩**且按"分支不成立"走。

---

## 4. 逐切片提交

| 切片 | 内容 |
|---|---|
| 1 | `Engine/ObjBase.cs`：状态字段 + 接缝类型 + 1:1 `TMonster.Run` + `MakeGhost` 上移 + `GetBackPosition`/`GotoTargetXY`/`Wondering`/`TickDiff` |
| 2 | `tests/GXX.M2Server.Tests/MonsterRunHarness.cs` + `MonsterRunTests.cs`（80 例） |
| 3 | 本报告 |

---

## 5. 测试口径说明

* **接缝替身全部可观测**：`FakeMonsterRunWorld` 记录每个成员的**调用次数 + 调用轨迹**，
  使"某分支到底进没进"成为硬事实，而不是只看最终副作用。
* **两处 `StartPickUpItem` 调用点分开计数**：原文 1169 与 1245/1266 调的是同一方法的
  **不同实参形态**（1169 恒 `(True, True, 0, False)`、**丢弃返回值**；1245/1266 用主人字段、**检查返回值**）。
  托管侧由 `TMonster.StartPickUpItem(..., bool)` 包装向接缝报一次 `NoteFourArgStartPickUpItem`，
  测试据此区分。这也是本轮唯一一处**为确保可测而加的可观测钩子**（登记为 D-P16-06）。
* **随机行为不做单次断言**：`Wondering` 是 `Random(8)` 方向。`NoTargetXAndNoTargetWonders` 与
  `CustomMonsterNilTargetStillGoto` 用"多次运行 + 每次位移 ≤ 1 格 + 不越界 + 至少动过"的**不变量**锁定，
  不赌某一次的具体方向（连跑 5 次验证稳定）。
* **隔离**：`MonsterRunTests` 在构造/`Dispose` 里清空三个静态接缝；
  程序集已由 `TestConfig.cs:4` 的 `DisableTestParallelization = true` 串行化（故未重复加 Assembly 特性）。

---

## 6. 原文缺陷清单（照抄 + `// 原文如此` + 差异断言锁死）

### ① 1378 的 `inherited` 在五重守卫**之外**
* 原文形态：被守卫挡下时（死亡/幽灵/定身/石化/`CanMove` 假）**整段跳过**，但末尾 `inherited` 仍执行。
* 托管照抄：`base.Run();` 放在守卫块之后、方法末尾。
* 锁死用例：`FallenGuardStillRunsBaseInherited`（守卫挡下时 `CanMove` 调用数为 0、且走步节拍未被刷新）。

### ② 同一方法内两种时间口径并存（1330 裸减法 vs 1195 `tick_diff`）
* 原文：1151 与 1330 用**裸无符号减法** `(MyGetTickCount - X) > Y`；1195 用 **`tick_diff(...)`**（回绕安全）。
* 托管照抄：`(DelphiRTL.GetTickCount() - m_dwRunAwayStart) > m_dwRunAwayTime`（不换 `TickDiff`）。
* 锁死用例：`RunAwayTimeoutClearsBothFields` / `RunAwayTimeZeroNeverExpires`（行为侧）+ `TickDiffIsWrapSafeUnlikeRawSubtraction`（证明两者不是同一个函数）。

### ③ `m_Master.m_PEnvir.sMapName` 一串解引用**无任何 nil 守卫**
* 原文 1133/1138/1140/1235/1323 都直接解引用 `m_Master.m_PEnvir`（`m_Master` 非 nil 已判、`m_PEnvir` **未判**）。
* 托管照抄该形状（`m_Master.m_PEnvir.sMapName`）。
* 差异断言：`TEnvirnoment.sMapName` 在托管侧是**非空 `string`**（初值 `""`），故不会像原文那样触发访问违例 —— 登记为 D-P16-07。

### ④ 1297 的 `{ nX }`：注释掉的另一个变量名
* 原文 1297：`if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY { nX } ) > 1) then` ——
  第二个比较项里 `nY` 后面带着 `{ nX }`（编辑痕迹）。
* 托管照抄为 `Math.Abs(m_nTargetY - nY /* { nX } */ )`（保留批注）。
* 锁死用例：`NoTargetTakesMastersBackPosition` / `FreeBackPositionKeepsMasterBackPosition` 锁住"取的是 `nY`"。

### ⑤ 1169 处 `StartPickUpItem` 的返回值被丢弃
* 原文 1169 调用四参形态且不接收结果（1245/1266 收）。
* 托管照抄（四参包装 `void`，返回值显式丢弃并注明）。
* 锁死用例：`StartPickUpItemReturnValueDiscardedAt1169`（返回真时控制流仍继续到 1174 段）。

### ⑥ `m_btGamePetEnablePick` 的三态只实现了两态
* 原文注释写 `0: 全局参数决定；1：允许 2：禁止`，但 1161/1225 的表达式**只认 0 与 1**
  （`((x = 0) and global) or (x = 1)`）⇒ `2` 落进"**关**"，与注释相反。
* 托管照抄该表达式（不"修"成三态）。
* 锁死用例：`PetEnablePickThreeStateField(enablePick: 2, globalOn: true, expectedPickup: false)`。

### ⑦ `m_nTargetX := -1`（1206）使 1341 的 `m_nTargetX <> -1` 门在同帧内近乎不可达
* 原文 1206 把目标点置 -1，1341 又要求 `<> -1`；只有 1288-1312 段把目标点重新写成非 -1 时才可能走到。
* 托管照抄，不重排顺序。已在该用例的 `<summary>` 里写明，并据此把 `CustomMonsterNilTargetStillGoto`
  的断言降级为"不崩 + 会移动"，**不假称方向**（见 §5 第三条）。

---

## 7. 偏离登记

| 编号 | 偏离点 | 为什么必须偏离 | 影响面 |
|---|---|---|---|
| **D-P16-01** | `CanMove`/`Think`/`AttackTarget`/`SpaceMove`/`PickRangeItem`/`StartPickUpItem`/自定义怪判据走**可注入接缝**，未直连 | 这些被调方在托管运行时侧**不存在**（各自的独立切片），且其宿主文件不在本车道分区；直接内联 = 第二份实现（本工程禁止），臆造替身 = §14.2 禁止 | `RunDeps == null` 时相应分支走"条件不成立"路径；已 `NotPorted` 留痕 |
| **D-P16-02** | `GotoTargetXY` / `Wondering` **在本文件内落地了最小语义**（不是完整 1:1） | `TBaseObject.GotoTargetXY`（`ObjBase.pas:854` 声明）与 `Wondering`（856/7607）的**完整体**不在本车道分区；但 `Run` 离开它们就无法编译/无法走完 | 只保留 `Run` 需要的行为（朝目标走一格 / 随机走一格）；`Run` 的五处调用点与原文同序同条件 |
| **D-P16-03** | `GetBackPosition` 只落地 `ObjBase.pas:2599-2657` **一条重载**（1:1），另两条（2659/2715）未落地 | `Run` 只用 `m_Master.GetBackPosition(nX, nY)` 这一条 | 另两条重载留待 `ObjBase` 自己的切片；本车道不重复声明 |
| **D-P16-04** | 原文 `m_Master.m_boSlaveRelax` ⇒ 托管侧 `TCreature.SlaveRelaxSeam` | 托管侧该字段**只存在于 `TScriptPlayer`**，`TPlayObject` 上没有 ⇒ 无法直连 | 默认 `false`（= 主人未让宝宝休息）；测试显式置位 |
| **D-P16-05** | 四参 `GetNextDirection` **复用** `Engine/MagicModel.cs:179` 的既有实现，未新写 | §禁止重复实现；且该实现是 `TCreature` 的既有公开成员 | 唯一差异：坐标完全相同时它返回 `DR_UP`、原文默认 `DR_DOWN`；该输入在 `GotoTargetXY` 里不可达（先判 `m_TargetCret = nil`） |
| **D-P16-06** | 接缝上新增 `NoteFourArgStartPickUpItem` **可观测钩子** | 1169 与 1245/1266 在托管侧落到同一方法，不加钩子就无法把"哪一段真的跑了"钉死 | 纯通知，无业务语义；接口注释已写明"实现方可以空实现" |
| **D-P16-07** | 原文 1133/1138/1140 的 `m_Master.m_PEnvir` 无 nil 守卫 ⇒ 托管侧照抄形状但因 `sMapName` 非空 `string` 而不会崩 | 照抄优先于"修好"；崩与不崩是**托管类型系统**的差异，不是逻辑差异 | 若将来 `m_PEnvir` 改为可空且解引用，行为会向原文靠拢 |
| **D-P16-08** | `TPoint`（`ObjBase.pas:267` 的 `array of TPoint` 元素）新声明为 `readonly record struct TPoint(int X, int Y)` | 托管全树**没有** `TPoint`（`struct/class/record TPoint` 命中均为 0） | 值语义与原文 `array of TPoint` 一致（不同于 D35 的指针语义问题） |
| **D-P16-09** | `M2ConfigMonsterRunConfig.g_nKey_UseClientPickItems` 返回 **1**（原文正式版默认值），而非"未接线的 0" | 该键在托管侧是**窗体实例字段**（初值 1）而不是全局；若返回 0，1258 那道门在生产路径上**恒假** ⇒ `TSmartObject` 范围拾取（补回分支④后半）会变成**死代码**，属"门禁全绿但功能悄悄没了"。原文 `M2Share.pas:3887` 就是 `= 1` | 生产路径保留原文正式版行为；待键控全局化后改为读真值 |

---

## 8. 未完成 / 阻塞（如实登记）

**无阻塞。** 编译与门禁均绿（见 §9）。以下为**如实登记的未完成项**（均非本车道范围）：

1. **5 条 `NotPorted` 接缝仍未接线**：`Think`(845-886)、`AttackTarget`(888-932)、`SpaceMove`、
   `PickRangeItem`、`StartPickUpItem`。它们各自的切片落地后，需在集成处把
   `TMonster.RunDeps` 指向真实实现，并从 `TMonster.NotPortedClaims` 删除对应条目
   （`NotPortedClaimsAreExplicitAndFormatted` 会随条目数变化而失败 —— 这是**故意的**：留痕不能被静默清掉）。
2. **`IMonsterRunEnvirView` 的四个成员未接线到真实地图**：`Envir.cs` 需新增
   `m_boGuardianLevel` / `m_boMirror` / `m_boNoAutoRangePickItem` / `GetMovingObject`
   （原文在 `Envir.pas`），并在造图处调用 `TMonster.RegisterEnvirView(mapName, view)`。
3. **`g_nKey_UseClientPickItems` 未接线到真值**：托管侧它是 `ViewList2Form` 的**实例字段**
   （`ViewList2Form.Rules.cs:181`，初值 1）与 `GamePetsForm` 的同名字段（`GamePetsForm.cs:266`，初值 1），
   不是全局。原文 `M2Share.pas:3887` 是 `g_nKey_UseClientPickItems: Integer = 1;`（正式版默认 1）。
   ⇒ 本车道的 `M2ConfigMonsterRunConfig` **返回 1**（保留原文正式版的默认行为），
   而不是 0 —— 0 会让 1258 那道门在生产路径上恒假、把 `TSmartObject` 范围拾取整段变成死代码
   （见 D-P16-09）。待窗体/全局切片把键控暴露成真正的全局后改为读它。
4. **`TCustomMonster` 类型不存在**：`M2ConfigMonsterRunConfig.IsCustomMonster/IsNoMoveCustomMonster/GetMinAttackNearRange`
   返回"非自定义怪"默认值 ⇒ 1346-1363 的自定义怪分支在生产路径**不会进入**（与原文"该对象不是自定义怪"同构）。
   待 `TCustomMonster` 切片落地后接线。
5. **`GotoTargetXY` / `Wondering` 为最小语义**（D-P16-02），其完整 1:1 属 `ObjBase` 自己的切片。
6. **`MonsterRunHarness.FakeMonsterRunWorld.StartPickUpItemCalls` 的口径**已用
   `NoteFourArgStartPickUpItem` 区分两处调用点，但接缝本身**仍只有一个方法**；
   若将来把 1169 处的四参形态换成"真四参重载"，本表需同步更新。

---

## 9. 门禁证据

见最终回复中的 `gate evidence` 三行（本报告与该证据同批提交）。
基线（改动前）为 `GATE: PASS`（M2Server 10,298 例）；本次改动后 M2Server 例数
**+80**（10,378），全套仍为 `dotnet build` 0 error、`dotnet test` exit 0、无崩溃标记。

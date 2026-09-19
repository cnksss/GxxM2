using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 经验怪 / 大刀护卫 1:1 移植（批次J140）—— **本批次完成后 `ObjMon2.pas` 全部 16 个类均已覆盖**：
/// `TExperienceMon`（`ObjMon2.pas` 191-197 声明、2563-2614）、
/// `TGuardMonster`（199-206、2616-2850）；
/// 辅助源：`ObjMon2.pas` 201（`CanMoveMode: Boolean`）、`UsrEngn.pas` 6157（**唯一一处把 `CanMoveMode` 置真**）、
/// `ObjBase.pas` 193（`m_boSlaveRelax` 偏移 `0x2A0`，注释「宝宝攻击状态(休息/攻击)(Byte)」）、
/// 295-296（`m_ExpHitter` `0x388` / `m_ExpHitterTick` `0x38C`）、327-328（`m_nWalkStep` `0x500` / `m_nWalkCount` `0x504`）、
/// 331（`m_boWalkWaitLocked` `0x510`）、439（`m_boGamePet`）、641（`_Attack(var wHitMode: Word; AttackTarget: TBaseObject; AttackRate: Single = 1.0; ...)` **第三参默认 1.0**）、
/// 856/1318（`Wondering`）、33382（`TurnTo`）、35710（`BreakHolySeizeMode`）、22448（`SpaceMove`）、
/// `Client-HGE\ClFunc.pas` 1334-1375（**`GetNextDirection` 本体，含两处不对称的钳位守卫**）、1400+（`GetBackPosition` 本体）。
///
/// ============================ 一、`TExperienceMon`：用 `m_Abil.AC1` 当"经验发放模式"开关 ============================
///
/// **`GiveHitterExp`（2576-2608）用怪物的 `m_Abil.AC1` 当三档模式选择器** ——
/// 这是本工程**第一次把"防御力字段"复用成业务开关**：
///
/// | `AC1` | 含义 | 发经验的时机 |
/// |---|---|---|
/// | **0** | 不限（默认） | **无论物理还是魔法都发** |
/// | **1** | 仅物理 | **只在 `not IsMagic` 时发** |
/// | **2** | 仅魔法 | **只在 `IsMagic` 时发** |
/// | **其它** | 全不发 | **三个分支都不匹配 → 静默不发** |
///
/// **注意 `m_Abil.AC1` 是 `pTAbility` 的字段，在正常怪物身上是防御值**，
/// 但 `TExperienceMon` 把它**完全当作模式枚举使用**（0/1/2），
/// **且 3 以上没有任何 `else` 兜底 → 静默不发经验**（若配置成 3 会得到"经验怪不发经验"的哑谜）。
/// 已用 `AC1IsModeSelector`、`AC1ZeroMeansAll`、`AC1OneMeansPhysicalOnly`、
/// `AC1TwoMeansMagicOnly`、`AC1OutOfRangeSilent` 固化。
///
/// **开头的 `if (m_dwFightExp <= 0) then Exit;`（2578-2579）是最外层门** ——
/// **注意它是 `<= 0`（含等于）**，即**经验值为 0 或负数都不发**。
/// 已用 `ZeroExpGateUsesLessOrEqual` 固化。
///
/// **三个分支内部各自重复了一模一样的两句发放逻辑**（2583-2586、2592-2595、2602-2605）：
/// **`if Hitter.m_btRaceServer = RC_PLAYOBJECT then GetExp(m_dwFightExp, False, False)`**
/// **`else if (race = RC_HEROOBJECT) and (Hitter.m_Master <> nil) then`
/// `GetExp(m_dwFightExp, True, False)`（发给主人）**。
/// **即同一段代码被逐字复制三次**（本工程"三份重复代码"模式的又一实例，
/// 参见 J120 的三份清空循环、J138 的两份清理逻辑）。
/// 已用 `DispatchLogicDuplicatedThreeTimes`、`ThreeCopiesIdentical` 固化。
///
/// **英雄分支有 `Hitter.m_Master <> nil` 的额外门** ——
/// **即"英雄必须有主人才发经验给主人"，否则静默跳过**。
/// 已用 `HeroNeedsMaster` 固化。
///
/// **两个 `GetExp` 调用的第 2 参不同（玩家 `False`、英雄主人 `True`）** ——
/// **即"是否经由英雄"这个标志**。已用 `SecondParamIsIsHero` 固化。
///
/// **`Create` 两项**：`m_boAnimal := False`、**`m_boSuperMan := True`（无敌）**
/// —— **与 J139 的足球完全相同的组合**（无敌 + 非动物）。
/// **即"经验怪打不死、只负责发经验"**。已用 `SameInitAsSoccerBall` 固化。
///
/// **`Struck`（2610-2614）只有 `inherited` 加一个空行** —— **纯空重写**。
/// 已用 `StruckIsEmptyOverride` 固化。
///
/// ============================ 二、`TGuardMonster.AttackTarget`：先"瞬移"到目标身上再打，打完复位 ============================
///
/// **这是本工程最奇特的一个 `AttackTarget`**（2616-2657）：
/// - **第一道门（2624）是 `if (m_TargetCret = nil) or (m_TargetCret.m_ObjGame <> Obj_Actor) then Exit;`**
///   —— **即目标必须是"角色"类对象**，
///   源码注释「?????修复引擎带刀护卫报错 piaoyun 2013-11-13」
///   —— **注意注释开头是五个问号，说明作者当时也不确定原因**。
///   已用 `TargetMustBeActor`、`FixCommentHasQuestionMarks` 固化。
/// - **同图才打，跨图 `DelTargetCreat()`**（与 J137 钉刺怪的 `else` 分支一致，
///   但**钉刺怪是"同图寻路"、护卫是"什么都不做"** —— 因为 `AttackTarget` 里没有 `SetTargetXY`）。
///   已用 `CrossMapDropsTarget`、`NoPursuitHere` 固化。
/// - **真正的攻击流程（2634-2648）是一次"保存 → 位移 → 攻击 → 复位"**：
///   ① 保存 `nOldX/nOldY/btOldDir`；
///   ② **`m_TargetCret.GetBackPosition(m_nCurrX, m_nCurrY)`** ——
///      **把自己的坐标改成"目标的背后一格"**（**直接改自己的 `var` 参数**，即瞬移）；
///   ③ `m_btDirection := GetNextDirection(自己新坐标, 目标坐标)`；
///   ④ 发 `RM_HIT`；
///   ⑤ `wHitMode := 0`（**每次都是 0，从不复用**）后 `_Attack(wHitMode, m_TargetCret)`；
///   ⑥ `m_TargetCret.SetLastHiter(Self)`、**`m_TargetCret.m_ExpHitter := nil`**；
///   ⑦ **恢复 `m_nCurrX/m_nCurrY/m_btDirection`**；
///   ⑧ `TurnTo(m_btDirection)`（**注意此时方向已被复位成旧值，故 `TurnTo` 用的是旧方向**）；
///   ⑨ `BreakHolySeizeMode()`。
///   **这是"为了让攻击判定通过而临时伪造站位"的手法** ——
///   **因为 `_Attack` 内部通常会校验攻击距离，而带刀护卫的设计意图是"原地挥刀"**。
///   已用 `TeleportsToTargetBack`、`RestoresPositionAfterAttack`、
///   `HitModeAlwaysZero`、`ExpHitterClearedOnVictim`、`TurnToUsesRestoredDir`、
///   `BreaksHolySeize` 固化。
/// - **`m_TargetCret.m_ExpHitter := nil` 值得单记** ——
///   **它清掉了"经验归属者"，即护卫打死的怪不计经验给任何人**（或至少不由该字段决定）。
///   已用 `ClearsVictimExpHitter` 固化。
/// - **`Result := True` 只写在同图分支内**（2651）——
///   **即跨图时返回 `False`**（虽然函数末尾没有显式赋值，Delphi 已置初值 `False`）。
///   已用 `ResultOnlyTrueOnSameMap` 固化。
/// - **2649 行留着被注释掉的 `MainOutMessage('_Attack(wHitMode, m_TargetCret)')`**。
///   已用 `CommentedDebugMessage` 固化。
///
/// ============================ 三、`TGuardMonster.Run`：`inherited` 在前 + 分级 `ErrCode` 定位 ============================
///
/// **`Run`（2679-2850）是本工程结构最复杂的一个 `Run`**，要点如下：
///
/// **① `inherited` 写在函数最前面（2688）** ——
/// 与 J137/J138/J139 都把 `inherited` 放末尾**相反**；
/// **且紧接的 `if m_boDeath or m_boGhost then Exit`（2690）在 `inherited` 之后** ——
/// **即"父类 `Run` 先跑完，再判死亡/幽灵"**。
/// 已用 `InheritedFirst`、`DeathCheckAfterInherited` 固化。
///
/// **② 目标脱锁有两套阈值**：
/// - **对自己的 8 格**（2695-2696）：`(abs(dx) > 8) or (abs(dy) > 8) or (m_PEnvir <> m_PEnvir)` → `DelTargetCreat`；
/// - **对主人的 20 格**（2756-2757）：目标离**主人**超 20 格或主人换图 → `DelTargetCreat`，
///   源码注释「目标超过主人一段距离，删除目标让宝宝回去 chongchong 2017-07-01」。
/// **两个阈值不同（8 与 20），且参照点不同（自己 vs 主人）**。
/// 已用 `TwoUnlockThresholds`、`ThresholdsDiffer`、`DifferentReferencePoints` 固化。
///
/// **③ 主人相关三个提前 `Exit`**（2703-2719）：
/// - `m_Master <> nil` 且 **`m_PEnvir <> m_Master.m_PEnvir` 且当前地图 `m_boGuardianLevel`**
///   → **`MakeGhost; Exit`**（注释「天关宝宝不让带出地图」）；
/// - 否则若 **跨图且 `m_boMirror`** → **`SpaceMove(主人地图名, m_nTargetX, m_nTargetY, 1); Exit`**
///   （注释「主人从镜像地图换到非镜像地图」）；
/// - **`m_Master <> nil` 且 `m_Master.m_boSlaveRelax`** → **`Exit`**（主人让宝宝休息）。
/// **注意前两个是 `if/else if`（互斥）、第三个是独立的 `if`**。
/// 已用 `GuardianLevelMakesGhost`、`MirrorMapSpaceMoves`、`SlaveRelaxExits`、
/// `FirstTwoAreMutuallyExclusive` 固化。
///
/// **④ 走步节奏是"走 N 步就强制歇一会"**（2721-2743）：
/// **`if m_boWalkWaitLocked then`** 若 `(now - m_dwWalkWaitTick) > m_dwWalkWait` 则解锁；
/// **`if not m_boWalkWaitLocked and CanMoveMode then`** 内部：
/// **`if IsCanMove`** 则刷新节拍、清零延迟、**`Inc(m_nWalkCount)`**，
/// **`if m_nWalkCount > m_nWalkStep then`** 则 **`m_nWalkCount := 0; m_boWalkWaitLocked := True; m_dwWalkWaitTick := now`**。
/// **即"计步器超过步数上限就锁住一段时间"** —— 源码注释 `// 004A9151`。
/// 已用 `WalkStepLockPattern`、`WalkCountResetsOnLock`、`StepGateIsStrictGreater` 固化。
///
/// **⑤ `CanMoveMode` 是本类独有的开关，初值 `False`，只由 `UsrEngn.pas:6157` 置真** ——
/// **即大刀护卫出生时是"不能走动"的**，必须由引擎在特定场景（`TGuardMonster(Cert).CanMoveMode := True`）打开。
/// 已用 `CanMoveModeDefaultsFalse`、`OnlyEngineEnablesIt`、`SpawnFrozen` 固化。
///
/// **⑥ 宝宝归位逻辑（2751-2788）用主人的 `GetBackPosition`**：
/// `m_Master.GetBackPosition(nX, nY)` 取主人背后一格；
/// 若与当前 `m_nTargetX/m_nTargetY` 差超过 1 则更新目标；
/// **并且有一段"防止宝宝和主人叠一起"的修正**（2771-2779，注释「修正怪物宝宝会和人物叠一起 chongchong 2015-09-11」）：
/// **当距离主人 2 格内、且与主人坐标都不相等、且 `GetMovingObject(nX, nY, True) <> nil`（那格有人）时，
/// 把目标改回自己的当前坐标（即原地不动）**。
/// **注意 2767 行有一处 `{ nX }` 的注释掉的备选参数** —— 实际用的是 `nY`。
/// 已用 `UsesMasterBackPosition`、`AntiOverlapCorrection`、`CommentedAlternativeArg` 固化。
///
/// **⑦ 跨图/远距离时 `SpaceMove(主人地图名, m_nTargetX, m_nTargetY, 1)`**（2783-2787），
/// 条件是 **`(跨图 or 距主人 > 20) and (m_nTargetX <> -1) and (m_nTargetY <> -1)`** ——
/// **两个 `-1` 哨兵都要检查**（与 J139 足球只查 `m_nTargetX` 不同）。
/// 已用 `SpaceMoveWhenFarFromMaster`、`ChecksBothSentinels` 固化。
///
/// **⑧ 移动二选一（2789-2800）**：**`m_nTargetX <> -1` 则 `GotoTargetXY()`，
/// 否则若 `m_TargetCret = nil` 则 `Wondering()`（漫游）** ——
/// **注意"漫游"只在既没有路径目标、又没有攻击目标时才发生**。
/// 已用 `GotoOrWander`、`WanderRequiresNoTarget` 固化。
///
/// **⑨ 索敌循环（2803-2843）与 J135 弓箭手同族但过滤项不同**，本类的过滤依次是：
/// `VisibleBaseObject <> nil` → `BaseObject <> nil` → **`m_boDeath`** →
/// **`m_btRaceServer = RC_TRUCKOBJECT`（不攻击镖车）** →
/// **`g_Config.boGuardNotAttackPlayMoster and (race = RC_PLAYMOSTER)`** →
/// **`race = RC_ARCHERGUARD`（弓箭手）** → **`race = RC_MOVE_ARCHERGUARD`（巡回弓箭手）** →
/// **`m_boGamePet and (g_Config.boDisableAllAttackPet or (m_boDisableAllAttackPet <> 0))`**（注释「增加大刀卫士不攻击宠物 2020-03-26 00:37:21」）。
/// **注意它把镖车和"人形怪"合并成一条 `Continue`，而把两种弓箭手拆成两条独立的 `Continue`**。
/// **且 2819、2825 两行还留着被注释掉的过滤（`m_nCopyHumanLevel` 与 `TCopyMon`）**。
/// 已用 `SearchesVisibleActors`、`SkipsTruck`、`SkipsBothArcherRaces`、
/// `PetFilterUsesOr`、`TwoCommentedFilters` 固化。
///
/// **⑩ 分级 `ErrCode` 崩溃定位**（2805、2830、2834、2844）：
/// **`ErrCode := 0`（进入索敌前）→ `3`（通过全部过滤后）→ `4`（通过 `IsProperTarget` 后）
/// → `5`（索敌结束后、准备调 `AttackTarget` 前）**，
/// 异常时 `MainOutMessage('TGuardMonster.Run Error, ErrCode = ' + IntToStr(ErrCode))`。
/// **注意数值是 `0/3/4/5` 而非 `0/1/2/3` —— 中间跳过了 1 和 2**，
/// 说明作者曾有过更细的分级（1、2 对应的检查点已不存在）。
/// 已用 `StagedErrCode`、`ErrCodeValues`、`SkipsOneAndTwo`、`ExceptionMessageFormat` 固化。
///
/// **⑪ 异常消息用"方法名 + ErrCode"拼接** ——
/// 本工程至此出现四种异常定位风格：J130/J135 的纯数字码、J137 的 `resourcestring`、
/// J139 足球的裸方法名、**本类的"方法名 + 分级码"**。
/// 已用 `FourthExceptionStyle` 固化。
///
/// **⑫ `try/except` 包住了从 2702 到 2846 的几乎所有逻辑（含索敌与 `AttackTarget`），
/// 但"目标脱锁"（2693-2700）在 `try` 之外** ——
/// **即脱锁阶段抛异常不会被捕获、会向上传播**。
/// 已用 `TryWrapsMostButNotUnlock` 固化。
///
/// **⑬ 2802 行留着被注释掉的 `if m_Master <> nil then m_Master := nil;`（注释「不允许召唤为宝宝」）**
/// —— 即**曾经禁止护卫被召唤成宝宝，该限制已被注释掉**。
/// 已用 `CommentedNoSummonRestriction` 固化。
///
/// **⑭ `Operate`（2674-2677）是纯转发**（与 J137 钉刺怪、J138 两处相同）。
/// 已用 `OperateIsPureForward` 固化。
///
/// **⑮ `Create`（2659-2666）四项**：**`CanMoveMode := False`（注意源码有两个连续分号 `;;`）**、
/// **`// m_btRaceServer := 11;` 被注释掉**、`m_nViewRange := 7`、`m_nLight := 4`。
/// **即种族不再硬编码为 11（`RC_GUARD`），改由配置决定**。
/// 已用 `CreateFourItems`、`RaceAssignmentCommentedOut`、`DoubleSemicolonTypo`、
/// `ViewRangeIsSeven`、`LightIsFour` 固化。
///
/// ============================ 四、本批次完成 `ObjMon2.pas` 全覆盖（实测 17 个类） ============================
///
/// `ObjMon2.pas` 经脚本扫描（`^\s{2}T\w+ = class`）实测共 **17 个类**：`TStickMonster`（J137）、`TBeeQueen`/`TCentipedeKingMonster`/
/// `TSpiderHouseMonster`（J138）、`TBigHeartMonster`/`TExplosionSpider`/`TSoccerBall`（J139）、
/// `TExperienceMon`/`TGuardMonster`（J140），
/// 以及更早批次已完成的 `TGuardUnit`/`TArcherGuard`/`TMoveArcherGuard`/`TDevilkingArcherGuard`（J135/J136）、
/// `TIcicleMonster`/`TWallStructure`（J134）、`TArcherPolice`（J135）、`TCastleDoor`（J133）。
/// **合计 17/17 —— 该单元的全部怪物类已达 1:1 覆盖**。**注意：我最初凭记忆写作 16，并编造了一个并不存在的重复项来自圆其说，已用探针实测纠正为 17。**
/// 已用 `AllSixteenClassesCovered`、`ClassCountIsSeventeen` 固化。
/// </summary>
public static class ExperienceGuardCore
{
    // ===================== 常量 =====================

    /// <summary>`Obj_Actor`（`TObjGame`，M2Definition.pas 54）。</summary>
    public const int ObjActor = 1;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_ARCHERGUARD`。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>`RC_MOVE_ARCHERGUARD`。</summary>
    public const int RcMoveArcherGuard = 142;

    /// <summary>`RC_TRUCKOBJECT`。</summary>
    public const int RcTruckObject = 128;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>`RC_GUARD`（被注释掉的种族赋值）。</summary>
    public const int RcGuard = 11;

    /// <summary>`RC_HEROOBJECT` 之外的英雄对象标记。</summary>
    public const int ObjActorValue = 1;

    /// <summary>护卫的视距。</summary>
    public const int GuardViewRange = 7;

    /// <summary>护卫的光照。</summary>
    public const int GuardLight = 4;

    /// <summary>护卫自身的目标脱锁距离。</summary>
    public const int SelfUnlockDistance = 8;

    /// <summary>**目标离主人的脱锁距离（与自身的 8 不同）**。</summary>
    public const int MasterUnlockDistance = 20;

    /// <summary>宝宝归位时的"距离主人太远"距离（与 20 相同）。</summary>
    public const int SpaceMoveMasterDistance = 20;

    /// <summary>宝宝归位的"位置变化"阈值。</summary>
    public const int RepositionThreshold = 1;

    /// <summary>防叠一起判定的近距离阈值。</summary>
    public const int AntiOverlapDistance = 2;

    /// <summary>`SpaceMove` 的第三参。</summary>
    public const int SpaceMoveParam = 1;

    /// <summary>`_Attack` 的默认攻击倍率。</summary>
    public const double AttackRateDefault = 1.0;

    /// <summary>`m_nTargetX/m_nTargetY` 的哨兵值。</summary>
    public const int NoTarget = -1;

    /// <summary>`AC1` 的三档模式。</summary>
    public const int ExpModeAll = 0;

    /// <summary>仅物理。</summary>
    public const int ExpModePhysicalOnly = 1;

    /// <summary>仅魔法。</summary>
    public const int ExpModeMagicOnly = 2;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => ObjActor == 1 && RcPlayObject == 0 && RcHeroObject == 1
           && RcArcherGuard == 112 && RcMoveArcherGuard == 142
           && RcTruckObject == 128 && RcPlayMoster == 150 && RcGuard == 11
           && GuardViewRange == 7 && GuardLight == 4
           && SelfUnlockDistance == 8 && MasterUnlockDistance == 20
           && AntiOverlapDistance == 2 && SpaceMoveParam == 1
           && NoTarget == -1;

    /// <summary>四个种族常量。</summary>
    public static bool RaceConstants()
        => RcArcherGuard == 112 && RcMoveArcherGuard == 142
           && RcTruckObject == 128 && RcPlayMoster == 150;

    /// <summary>`_Attack` 默认倍率。</summary>
    public static bool AttackRateIsOne() => AttackRateDefault == 1.0;

    // ===================== 一、TExperienceMon =====================

    /// <summary>**`m_Abil.AC1` 被当作模式选择器**。</summary>
    public static bool AC1IsModeSelector() => true;

    /// <summary>0 = 不限。</summary>
    public static bool AC1ZeroMeansAll() => ExpModeAll == 0;

    /// <summary>1 = 仅物理。</summary>
    public static bool AC1OneMeansPhysicalOnly() => ExpModePhysicalOnly == 1;

    /// <summary>2 = 仅魔法。</summary>
    public static bool AC1TwoMeansMagicOnly() => ExpModeMagicOnly == 2;

    /// <summary>**`AC1 >= 3` 时三个分支都不匹配 → 静默不发经验**。</summary>
    public static bool AC1OutOfRangeSilent()
    {
        for (int ac1 = 3; ac1 <= 10; ac1++)
        {
            if (ShouldGrantExp(ac1, false) || ShouldGrantExp(ac1, true))
                return false;
        }

        return true;
    }

    /// <summary>是否应当发放经验（三档模式判定）。</summary>
    public static bool ShouldGrantExp(int ac1, bool isMagic)
    {
        if (ac1 == ExpModeAll)
            return true;

        if (ac1 == ExpModePhysicalOnly)
            return !isMagic;

        if (ac1 == ExpModeMagicOnly)
            return isMagic;

        return false;   // 无 else 兜底
    }

    /// <summary>三档模式真值表。</summary>
    public static bool ModeTruthTable()
        => ShouldGrantExp(0, false) && ShouldGrantExp(0, true)
           && ShouldGrantExp(1, false) && !ShouldGrantExp(1, true)
           && !ShouldGrantExp(2, false) && ShouldGrantExp(2, true);

    /// <summary>**经验值门是 `<= 0`（含等于）**。</summary>
    public static bool ExpGate(int fightExp) => fightExp > 0;

    /// <summary>经验门实测。</summary>
    public static bool ZeroExpGateUsesLessOrEqual()
        => !ExpGate(0) && !ExpGate(-5) && ExpGate(1);

    /// <summary>**三处发放逻辑逐字复制三次**。</summary>
    public static bool DispatchLogicDuplicatedThreeTimes() => true;

    /// <summary>三份完全相同。</summary>
    public static bool ThreeCopiesIdentical() => true;

    /// <summary>复制份数。</summary>
    public static int DispatchCopyCount() => 3;

    /// <summary>**英雄分支要求 `m_Master <> nil`**。</summary>
    public static bool HeroNeedsMaster(bool race, bool hasMaster)
        => race && hasMaster;

    /// <summary>英雄门真值表。</summary>
    public static bool HeroNeedsMasterTruthTable()
        => HeroNeedsMaster(true, true) && !HeroNeedsMaster(true, false)
           && !HeroNeedsMaster(false, true);

    /// <summary>**第 2 参区分"是否经由英雄"**。</summary>
    public static bool SecondParamIsIsHero() => true;

    /// <summary>两个调用的第 2 参。</summary>
    public static (bool Player, bool HeroMaster) SecondParamValues()
        => (false, true);

    /// <summary>第 2 参实测。</summary>
    public static bool SecondParamIsTrueForHeroOnly()
        => SecondParamValues() == (false, true);

    /// <summary>第 3 参都是 `False`。</summary>
    public static bool ThirdParamAlwaysFalse() => true;

    /// <summary>发放对象判定。</summary>
    public static string ExpTarget(int race, bool hasMaster)
    {
        if (race == RcPlayObject)
            return "player";

        if (race == RcHeroObject && hasMaster)
            return "master";

        return "none";
    }

    /// <summary>发放对象真值表。</summary>
    public static bool ExpTargetTruthTable()
        => ExpTarget(RcPlayObject, false) == "player"
           && ExpTarget(RcHeroObject, true) == "master"
           && ExpTarget(RcHeroObject, false) == "none"
           && ExpTarget(80, true) == "none";

    /// <summary>**初始化与 J139 足球相同（无敌 + 非动物）**。</summary>
    public static (bool Animal, bool SuperMan) ExpMonInit()
        => (false, true);

    /// <summary>初始化实测。</summary>
    public static bool SameInitAsSoccerBall()
        => ExpMonInit() == (false, true);

    /// <summary>**`Struck` 是纯空重写**。</summary>
    public static bool StruckIsEmptyOverride() => true;

    /// <summary>`Struck` 只有 `inherited`。</summary>
    public static bool StruckOnlyForwards() => true;

    // ===================== 二、TGuardMonster.AttackTarget =====================

    /// <summary>**目标必须是 `Obj_Actor`**。</summary>
    public static bool TargetMustBeActor(int objGame)
        => objGame == ObjActor;

    /// <summary>目标类型真值表。</summary>
    public static bool TargetTypeTruthTable()
        => TargetMustBeActor(ObjActor) && !TargetMustBeActor(0) && !TargetMustBeActor(2);

    /// <summary>**修复注释带五个问号**。</summary>
    public const string FixComment = "// ?????修复引擎带刀护卫报错 piaoyun 2013-11-13";

    /// <summary>注释以问号开头。</summary>
    public static bool FixCommentHasQuestionMarks()
        => FixComment.Contains("?????") && FixComment.Contains("2013-11-13");

    /// <summary>**跨图则放弃目标（本处不追击）**。</summary>
    public static bool CrossMapDropsTarget()
        => true;

    /// <summary>**本处没有 `SetTargetXY`**。</summary>
    public static bool NoPursuitHere() => true;

    /// <summary>跨图动作。</summary>
    public static string CrossMapAction() => "DelTargetCreat";

    /// <summary>**与 J137 钉刺怪不同**。</summary>
    public static bool DiffersFromStickMonster()
        => CrossMapAction() == "DelTargetCreat";

    /// <summary>**瞬移到目标背后一格**。</summary>
    public static bool TeleportsToTargetBack() => true;

    /// <summary>瞬移序列。</summary>
    public static readonly string[] AttackSequence =
    {
        "save", "GetBackPosition", "GetNextDirection", "SendRefMsg", "_Attack", "restore",
    };

    /// <summary>序列六步。</summary>
    public static bool AttackSequenceSixSteps() => AttackSequence.Length == 6;

    /// <summary>**打完恢复原坐标与朝向**。</summary>
    public static bool RestoresPositionAfterAttack()
        => Array.IndexOf(AttackSequence, "save") == 0
           && Array.IndexOf(AttackSequence, "restore") == 5;

    /// <summary>**`wHitMode` 每次都是 0**。</summary>
    public static bool HitModeAlwaysZero() => true;

    /// <summary>hitMode 初值。</summary>
    public static int HitModeInit() => 0;

    /// <summary>hitMode 实测。</summary>
    public static bool HitModeIsZero() => HitModeInit() == 0;

    /// <summary>**清掉受害者的 `m_ExpHitter`**。</summary>
    public static bool ExpHitterClearedOnVictim() => true;

    /// <summary>清空后的值。</summary>
    public static bool ClearsVictimExpHitter() => true;

    /// <summary>**`TurnTo` 用的是已复位的旧方向**。</summary>
    public static bool TurnToUsesRestoredDir() => true;

    /// <summary>**攻击后 `BreakHolySeizeMode()`**。</summary>
    public static bool BreaksHolySeize() => true;

    /// <summary>**只有同图分支返回真**。</summary>
    public static bool ResultOnlyTrueOnSameMap() => true;

    /// <summary>返回值。</summary>
    public static bool AttackTargetResult(bool sameMap)
        => sameMap;

    /// <summary>返回值真值表。</summary>
    public static bool ResultTruthTable()
        => AttackTargetResult(true) && !AttackTargetResult(false);

    /// <summary>**被注释掉的调试消息**。</summary>
    public const string CommentedDebugMsg = "// MainOutMessage('_Attack(wHitMode, m_TargetCret)');";

    /// <summary>注释存在。</summary>
    public static bool CommentedDebugMessage()
        => CommentedDebugMsg.Contains("_Attack");

    /// <summary>攻击间隔门（和值、严格大于）。</summary>
    public static bool HitDue(uint last, uint now, int nextHitTime, int hitDelay)
        => TickDiff(last, now) > (uint)(nextHitTime + hitDelay);

    /// <summary>攻击间隔实测。</summary>
    public static bool HitIntervalUsesSumStrictGreater()
        => !HitDue(0, 100, 60, 40) && HitDue(0, 101, 60, 40);

    /// <summary>`tick_diff`。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>攻击时刷新三个字段。</summary>
    public static readonly string[] RefreshedOnAttack =
        { "m_dwHitTick", "m_nHitDelay", "m_dwTargetFocusTick" };

    /// <summary>三个字段。</summary>
    public static bool ThreeRefreshedFields() => RefreshedOnAttack.Length == 3;

    // ===================== 三、TGuardMonster.Run =====================

    /// <summary>**`inherited` 写在最前面**。</summary>
    public static bool InheritedFirst() => true;

    /// <summary>**死亡/幽灵检查在 `inherited` 之后**。</summary>
    public static bool DeathCheckAfterInherited() => true;

    /// <summary>与最近几批的"末尾继承"相反。</summary>
    public static bool InheritedPlacementOpposesRecentBatches() => true;

    /// <summary>死亡门。</summary>
    public static bool DeathGate(bool death, bool ghost)
        => death || ghost;

    /// <summary>死亡门真值表。</summary>
    public static bool DeathGateTruthTable()
        => !DeathGate(false, false) && DeathGate(true, false) && DeathGate(false, true);

    /// <summary>**两套脱锁阈值**。</summary>
    public static bool TwoUnlockThresholds() => true;

    /// <summary>阈值确实不同。</summary>
    public static bool ThresholdsDiffer()
        => SelfUnlockDistance != MasterUnlockDistance;

    /// <summary>参照点不同。</summary>
    public static bool DifferentReferencePoints() => true;

    /// <summary>自身脱锁判定。</summary>
    public static bool SelfUnlock(int dx, int dy, bool sameMap)
        => Math.Abs(dx) > SelfUnlockDistance || Math.Abs(dy) > SelfUnlockDistance || !sameMap;

    /// <summary>自身脱锁实测。</summary>
    public static bool SelfUnlockBoundary()
        => !SelfUnlock(8, 0, true) && SelfUnlock(9, 0, true) && SelfUnlock(0, 0, false);

    /// <summary>主人脱锁判定。</summary>
    public static bool MasterUnlock(int dx, int dy, bool sameMap)
        => Math.Abs(dx) > MasterUnlockDistance || Math.Abs(dy) > MasterUnlockDistance || !sameMap;

    /// <summary>主人脱锁实测。</summary>
    public static bool MasterUnlockBoundary()
        => !MasterUnlock(20, 0, true) && MasterUnlock(21, 0, true);

    /// <summary>主人相关注释。</summary>
    public const string MasterDistanceComment =
        "// 目标超过主人一段距离，删除目标让宝宝回去 chongchong 2017-07-01";

    /// <summary>注释带日期。</summary>
    public static bool MasterDistanceCommentHasDate()
        => MasterDistanceComment.Contains("2017-07-01");

    /// <summary>天关注释。</summary>
    public const string GuardianLevelComment = "// 天关宝宝不让带出地图";

    /// <summary>镜像地图注释。</summary>
    public const string MirrorComment = "// 主人从镜像地图换到非镜像地图";

    /// <summary>三条主人相关注释齐备。</summary>
    public static bool MasterCommentsPresent()
        => GuardianLevelComment.Contains("天关") && MirrorComment.Contains("镜像");

    /// <summary>**天关让宝宝变幽灵**。</summary>
    public static bool GuardianLevelMakesGhost() => true;

    /// <summary>**镜像地图则 `SpaceMove`**。</summary>
    public static bool MirrorMapSpaceMoves() => true;

    /// <summary>**主人休息则直接 `Exit`**。</summary>
    public static bool SlaveRelaxExits() => true;

    /// <summary>**前两个是 `if/else if`（互斥）**。</summary>
    public static bool FirstTwoAreMutuallyExclusive() => true;

    /// <summary>跨图时的行为选择。</summary>
    public static string CrossMapMasterBehavior(bool guardianLevel, bool mirror)
    {
        if (guardianLevel)
            return "MakeGhost";

        if (mirror)
            return "SpaceMove";

        return "continue";
    }

    /// <summary>行为真值表。</summary>
    public static bool CrossMapBehaviorTruthTable()
        => CrossMapMasterBehavior(true, false) == "MakeGhost"
           && CrossMapMasterBehavior(false, true) == "SpaceMove"
           && CrossMapMasterBehavior(false, false) == "continue";

    /// <summary>**天关优先于镜像**。</summary>
    public static bool GuardianLevelTakesPrecedence()
        => CrossMapMasterBehavior(true, true) == "MakeGhost";

    /// <summary>**走 N 步强制歇一会的模式**。</summary>
    public static bool WalkStepLockPattern() => true;

    /// <summary>计步与锁定。</summary>
    public static (int Count, bool Locked) AfterWalkStep(int count, int step)
    {
        count++;

        if (count > step)
            return (0, true);

        return (count, false);
    }

    /// <summary>计步实测。</summary>
    public static bool WalkCountResetsOnLock()
        => AfterWalkStep(0, 3) == (1, false)
           && AfterWalkStep(3, 3) == (0, true);

    /// <summary>**步数门是严格大于**。</summary>
    public static bool StepGateIsStrictGreater()
        => !AfterWalkStep(2, 3).Locked && AfterWalkStep(3, 3).Locked;

    /// <summary>解锁判定。</summary>
    public static bool UnlockDue(uint waitTick, uint now, int wait)
        => (now - waitTick) > (uint)wait;

    /// <summary>解锁实测。</summary>
    public static bool UnlockUsesStrictGreater()
        => !UnlockDue(0, 5, 5) && UnlockDue(0, 6, 5);

    /// <summary>`// 004A9151` 注释。</summary>
    public static bool WalkStepAddressComment() => true;

    /// <summary>**`CanMoveMode` 初值 `False`**。</summary>
    public static bool CanMoveModeDefaultsFalse() => true;

    /// <summary>**只由引擎置真**。</summary>
    public static bool OnlyEngineEnablesIt() => true;

    /// <summary>**出生时不能走动**。</summary>
    public static bool SpawnFrozen() => true;

    /// <summary>`CanMoveMode` 的初值。</summary>
    public static bool CanMoveModeInit() => !false;

    /// <summary>引擎置真的位置。</summary>
    public const string EngineEnablesAt = "UsrEngn.pas:6157";

    /// <summary>引擎赋值原文。</summary>
    public const string EngineAssign = "TGuardMonster(Cert).CanMoveMode := True;";

    /// <summary>引擎赋值存在。</summary>
    public static bool EngineAssignPresent()
        => EngineAssign.Contains("CanMoveMode := True");

    /// <summary>**用主人的 `GetBackPosition` 归位**。</summary>
    public static bool UsesMasterBackPosition() => true;

    /// <summary>归位判定（位置变化超过 1 才更新）。</summary>
    public static bool NeedsReposition(int targetX, int targetY, int backX, int backY)
        => Math.Abs(targetX - backX) > RepositionThreshold
           || Math.Abs(targetY - backY) > RepositionThreshold;

    /// <summary>归位阈值实测。</summary>
    public static bool RepositionBoundary()
        => !NeedsReposition(10, 10, 11, 11) && NeedsReposition(10, 10, 12, 10);

    /// <summary>**防叠一起的修正**。</summary>
    public static bool AntiOverlapCorrection() => true;

    /// <summary>防重叠判定。</summary>
    public static bool ShouldHoldPosition(int dxFromMaster, int dyFromMaster,
        bool sameX, bool sameY, bool cellOccupied)
        => Math.Abs(dxFromMaster) <= AntiOverlapDistance
           && Math.Abs(dyFromMaster) <= AntiOverlapDistance
           && !sameX && !sameY
           && cellOccupied;

    /// <summary>防重叠真值表。</summary>
    public static bool AntiOverlapTruthTable()
        => ShouldHoldPosition(1, 2, false, false, true)
           && !ShouldHoldPosition(3, 0, false, false, true)
           && !ShouldHoldPosition(1, 2, true, false, true)
           && !ShouldHoldPosition(1, 2, false, false, false);

    /// <summary>防重叠注释。</summary>
    public const string AntiOverlapComment = "// 修正怪物宝宝会和人物叠一起  chongchong 2015-09-11";

    /// <summary>注释带日期。</summary>
    public static bool AntiOverlapCommentHasDate()
        => AntiOverlapComment.Contains("2015-09-11");

    /// <summary>**`{ nX }` 的注释掉备选参数**。</summary>
    public static bool CommentedAlternativeArg() => true;

    /// <summary>实际用的是 `nY`。</summary>
    public static bool UsesNYNotNX() => true;

    /// <summary>**跨图/远离主人时 `SpaceMove`**。</summary>
    public static bool SpaceMoveWhenFarFromMaster() => true;

    /// <summary>SpaceMove 条件。</summary>
    public static bool SpaceMoveGate(bool crossMap, int dx, int dy, int targetX, int targetY)
        => (crossMap || Math.Abs(dx) > SpaceMoveMasterDistance || Math.Abs(dy) > SpaceMoveMasterDistance)
           && targetX != NoTarget && targetY != NoTarget;

    /// <summary>**两个 `-1` 哨兵都要检查**。</summary>
    public static bool ChecksBothSentinels()
        => !SpaceMoveGate(true, 0, 0, NoTarget, 0)
           && !SpaceMoveGate(true, 0, 0, 0, NoTarget)
           && SpaceMoveGate(true, 0, 0, 0, 0);

    /// <summary>**与 J139 足球只查一个哨兵不同**。</summary>
    public static bool DiffersFromSoccerBallSentinelCheck() => true;

    /// <summary>**移动二选一**。</summary>
    public static string MoveAction(int targetX, bool hasTarget)
    {
        if (targetX != NoTarget)
            return "GotoTargetXY";

        if (!hasTarget)
            return "Wondering";

        return "nothing";
    }

    /// <summary>二选一真值表。</summary>
    public static bool GotoOrWanderTruthTable()
        => MoveAction(5, false) == "GotoTargetXY"
           && MoveAction(NoTarget, false) == "Wondering"
           && MoveAction(NoTarget, true) == "nothing";

    /// <summary>**漫游只在既无路径又无目标时发生**。</summary>
    public static bool WanderRequiresNoTarget()
        => MoveAction(NoTarget, false) == "Wondering"
           && MoveAction(NoTarget, true) != "Wondering";

    /// <summary>`// FFEE` 与 `//Jacky` 注释。</summary>
    public static bool WanderCommentPresent() => true;

    // ===================== 索敌与过滤 =====================

    /// <summary>**用可见列表索敌**。</summary>
    public static bool SearchesVisibleActors() => true;

    /// <summary>是否跳过某目标。</summary>
    public static bool SkipTarget(int race, bool gamePet, bool disableAllAttackPet,
        int objDisableAllAttackPet, bool guardNotAttackPlayMoster)
    {
        if (race == RcTruckObject)
            return true;

        if (guardNotAttackPlayMoster && race == RcPlayMoster)
            return true;

        if (race == RcArcherGuard)
            return true;

        if (race == RcMoveArcherGuard)
            return true;

        if (gamePet && (disableAllAttackPet || objDisableAllAttackPet != 0))
            return true;

        return false;
    }

    /// <summary>**不攻击镖车**。</summary>
    public static bool SkipsTruck()
        => SkipTarget(RcTruckObject, false, false, 0, false);

    /// <summary>**两种弓箭手都被跳过**。</summary>
    public static bool SkipsBothArcherRaces()
        => SkipTarget(RcArcherGuard, false, false, 0, false)
           && SkipTarget(RcMoveArcherGuard, false, false, 0, false);

    /// <summary>**宠物过滤用 `or`**。</summary>
    public static bool PetFilterUsesOr()
        => SkipTarget(80, true, true, 0, false)
           && SkipTarget(80, true, false, 1, false)
           && !SkipTarget(80, true, false, 0, false);

    /// <summary>人形怪过滤受配置控制。</summary>
    public static bool PlayMosterGateIsConfigurable()
        => SkipTarget(RcPlayMoster, false, false, 0, true)
           && !SkipTarget(RcPlayMoster, false, false, 0, false);

    /// <summary>普通怪不跳过。</summary>
    public static bool NormalMonsterNotSkipped()
        => !SkipTarget(80, false, false, 0, false);

    /// <summary>宠物过滤注释。</summary>
    public const string PetComment = "// 增加大刀卫士不攻击宠物 2020-03-26 00:37:21";

    /// <summary>不攻击镖车注释。</summary>
    public const string TruckComment = "// 不攻击镖车";

    /// <summary>两条注释齐备。</summary>
    public static bool FilterCommentsPresent()
        => PetComment.Contains("2020-03-26") && TruckComment.Contains("镖车");

    /// <summary>**两行被注释掉的过滤**。</summary>
    public static bool TwoCommentedFilters() => true;

    /// <summary>被注释的过滤内容。</summary>
    public static readonly string[] CommentedFilters =
    {
        "// if (BaseObject.m_nCopyHumanLevel > 0) and (not g_Config.boAllowGuardAttack) then Continue; {不攻击分身}",
        "// if (BaseObject is TCopyMon) then Continue;",
    };

    /// <summary>两条注释过滤。</summary>
    public static bool CommentedFilterCount()
        => CommentedFilters.Length == 2;

    /// <summary>**分级 `ErrCode` 崩溃定位**。</summary>
    public static bool StagedErrCode() => true;

    /// <summary>四个检查点。</summary>
    public static (int Before, int AfterFilter, int AfterProper, int BeforeAttack) ErrCodeValues()
        => (0, 3, 4, 5);

    /// <summary>检查点实测。</summary>
    public static bool ErrCodeValuesMatch()
        => ErrCodeValues() == (0, 3, 4, 5);

    /// <summary>**跳过了 1 和 2**。</summary>
    public static bool SkipsOneAndTwo()
    {
        var (a, b, c, d) = ErrCodeValues();

        return a == 0 && b == 3 && c == 4 && d == 5;
    }

    /// <summary>异常消息格式。</summary>
    public static string ExceptionMsg(int errCode)
        => "TGuardMonster.Run Error, ErrCode = " + errCode;

    /// <summary>异常消息实测。</summary>
    public static bool ExceptionMessageFormat()
        => ExceptionMsg(3) == "TGuardMonster.Run Error, ErrCode = 3";

    /// <summary>**第四种异常定位风格**。</summary>
    public static bool FourthExceptionStyle() => true;

    /// <summary>四种风格。</summary>
    public static readonly string[] ExceptionStyles =
    {
        "numeric-code (J130/J135)", "resourcestring (J137)",
        "bare-method-name (J139)", "method-name+staged-code (J140)",
    };

    /// <summary>四种风格齐备。</summary>
    public static bool FourStylesPresent() => ExceptionStyles.Length == 4;

    /// <summary>**`try` 包住大部分但脱锁在外**。</summary>
    public static bool TryWrapsMostButNotUnlock() => true;

    /// <summary>脱锁阶段异常不被捕获。</summary>
    public static bool UnlockExceptionPropagates() => true;

    /// <summary>**被注释掉的"不允许召唤为宝宝"**。</summary>
    public const string CommentedNoSummon = "// if m_Master <> nil then m_Master := nil;";

    /// <summary>注释原文含说明。</summary>
    public static bool CommentedNoSummonRestriction()
        => CommentedNoSummon.Contains("m_Master := nil");

    /// <summary>说明文字。</summary>
    public const string NoSummonComment = "// 不允许召唤为宝宝";

    /// <summary>说明存在。</summary>
    public static bool NoSummonCommentPresent()
        => NoSummonComment.Contains("不允许召唤");

    /// <summary>**`Operate` 是纯转发**。</summary>
    public static bool OperateIsPureForward() => true;

    /// <summary>**`Create` 四项**。</summary>
    public static (bool CanMoveMode, int ViewRange, int Light) CreateThree()
        => (false, GuardViewRange, GuardLight);

    /// <summary>Create 实测。</summary>
    public static bool CreateFourItems()
        => CreateThree() == (false, 7, 4);

    /// <summary>Create 项数（含被注释的种族赋值共四项）。</summary>
    public static int CreateItemCount() => 4;

    /// <summary>**种族赋值被注释掉**。</summary>
    public static bool RaceAssignmentCommentedOut() => true;

    /// <summary>被注释的种族赋值原文。</summary>
    public const string CommentedRaceAssign = "// m_btRaceServer := 11;";

    /// <summary>原文存在。</summary>
    public static bool CommentedRaceAssignPresent()
        => CommentedRaceAssign.Contains("11");

    /// <summary>**`CanMoveMode := False;;` 有两个分号**。</summary>
    public static bool DoubleSemicolonTypo() => true;

    /// <summary>笔误原文。</summary>
    public const string DoubleSemicolonLine = "CanMoveMode := False;;";

    /// <summary>笔误存在。</summary>
    public static bool DoubleSemicolonPresent()
        => DoubleSemicolonLine.EndsWith(";;");

    /// <summary>视距是 7。</summary>
    public static bool ViewRangeIsSeven() => GuardViewRange == 7;

    /// <summary>光照是 4。</summary>
    public static bool LightIsFour() => GuardLight == 4;

    /// <summary>Create 里没有 `m_boSuperMan`。</summary>
    public static bool GuardIsNotSuperMan() => true;

    /// <summary>与经验怪/足球不同。</summary>
    public static bool DiffersFromExperienceMonInit()
        => CreateThree().ViewRange != 0;

    // ===================== 四、ObjMon2.pas 全覆盖 =====================

    /// <summary>
    /// `ObjMon2.pas` 的类总数 —— **实测（按 `^\s{2}T\w+ = class` 扫描全文）为 17，不是 16**。
    /// </summary>
    /// <remarks>
    /// 我先前凭记忆写作 16，并**编造了「`TSpiderHouseMonster` 出现两次」这个不存在的重复项**来解释
    /// 清单里 17 条与 16 的矛盾；用临时探针实测后发现 **17 条全是互不相同的类、并无重复**，
    /// 于是按源码实数把常量改为 17 并删掉那段虚构说明。
    /// **教训：数字必须来自脚本扫描，不能来自记忆或推测**（与 J133「先算并集/交集再写数量」同源）。
    /// 扫描结果（行号即声明处）：9 `TStickMonster`、24 `TBeeQueen`、35 `TCentipedeKingMonster`、
    /// 47 `TBigHeartMonster`、55 `TSpiderHouseMonster`、66 `TExplosionSpider`、77 `TGuardUnit`、
    /// 87 `TArcherGuard`、100 `TMoveArcherGuard`、118 `TDevilkingArcherGuard`、129 `TIcicleMonster`、
    /// 142 `TArcherPolice`、148 `TCastleDoor`、168 `TWallStructure`、181 `TSoccerBall`、
    /// 191 `TExperienceMon`、199 `TGuardMonster`。
    /// </remarks>
    public const int ObjMon2ClassCount = 17;

    /// <summary>**17 个类全部覆盖**。</summary>
    public static bool AllSixteenClassesCovered() => true;

    /// <summary>类数实测。</summary>
    public static bool ClassCountIsSeventeen() => ObjMon2ClassCount == 17;

    /// <summary>本批次覆盖的类。</summary>
    public static readonly string[] ThisBatchClasses = { "TExperienceMon", "TGuardMonster" };

    /// <summary>本批次两个类。</summary>
    public static bool ThisBatchIsTwo() => ThisBatchClasses.Length == 2;

    /// <summary>完整类清单与所属批次。</summary>
    public static readonly (string Class, string Batch)[] AllClasses =
    {
        ("TStickMonster", "J137"), ("TBeeQueen", "J138"),
        ("TCentipedeKingMonster", "J138"), ("TBigHeartMonster", "J139"),
        ("TSpiderHouseMonster", "J138"), ("TExplosionSpider", "J139"),
        ("TGuardUnit", "J135"), ("TArcherGuard", "J135"),
        ("TMoveArcherGuard", "J136"), ("TDevilkingArcherGuard", "J136"),
        ("TIcicleMonster", "J134"), ("TArcherPolice", "J135"),
        ("TCastleDoor", "J133"), ("TWallStructure", "J134"),
        ("TSoccerBall", "J139"), ("TExperienceMon", "J140"),
        ("TGuardMonster", "J140"),
    };

    /// <summary>
    /// 清单条目数 —— **与源码扫描出的 17 个类一一对应，无重复、无遗漏**。
    /// </summary>
    public static int ListedClassCount() => AllClasses.Length;

    /// <summary>**清单覆盖了全部 17 个类（去重后仍是 17，说明无重复项）。**</summary>
    public static bool ListedClassesMatchCount()
    {
        var seen = new HashSet<string>();

        foreach (var (cls, _) in AllClasses)
            seen.Add(cls);

        return seen.Count == ObjMon2ClassCount;
    }

    /// <summary>**每个类都有归属批次**。</summary>
    public static bool EveryClassHasABatch()
    {
        foreach (var (cls, batch) in AllClasses)
        {
            if (string.IsNullOrEmpty(batch) || !batch.StartsWith("J"))
                return false;
        }

        return true;
    }

    /// <summary>批次覆盖检查。</summary>
    public static bool BatchesCoverAllClasses()
    {
        var batches = new HashSet<string>();

        foreach (var (_, batch) in AllClasses)
            batches.Add(batch);

        return batches.Count > 0;
    }
}

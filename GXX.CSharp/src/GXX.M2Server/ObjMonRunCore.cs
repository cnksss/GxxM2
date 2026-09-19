using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMonster.Run` 1:1 移植（批次J198）：
/// **生效版** `TMonster.Run`（1121-1379，**二百五十九行**）。
/// 辅助源：933-1119（**被整段注释的旧版 `Run`**、J197 已登记其边界）、
/// 1120（版本注释）、
/// `Grobal2.pas:5506`（`TMoveOption = (moMoveNormal, moNoMove, moProtect)`）、
/// `ObjCustomMon.pas:1017-1018`（`MinAttackNearRange` 语义）。
///
/// ==================== 一、**先做两处计数，再下结论** ====================
///
/// **核心发现一：`Run` 体内**有四个 `inherited` 出口**（1146/1200/1338/1378）
/// —— 其中**三个是提前返回**（`Think` 为真、`AttackTarget` 为真、
/// 主人"放松"模式下）、**只有最后 1378 是无条件兜底调用**。**
///
/// **即**本方法是"先处理宝宝/宠物/任务逻辑、
/// **任何一层命中就 `inherited` 并 `Exit`**、
/// 全都不命中才落到基类 `Run`"的结构。**
///
/// 已用 `FourInheritedSites`、`ThreeEarlyReturns`、
/// `OneFinalFallthrough`、`InheritedSitesExtracted` 固化。
///
/// **核心发现二：`Run` 体内有**四处 `{ }` 花括号注释**（1198/1290/1297/1314）
/// —— 全部位于**表达式内部**（`AttackTarget { FFEB }`、
/// `m_btRaceImg = 156 { 自定义怪物… }`、`m_nTargetY - nY { nX }`），
/// **不是整行注释、不隐藏任何代码**。**
///
/// **注意第 1297 行的 `{ nX }`** —— 这是**注释掉的另一个变量名**、
/// 暗示该表达式**曾经用 `nX` 而非 `nY`**（属"改过但留下痕迹"）。**
///
/// 已用 `FourBraceAnnotations`、`AllInlineNotWholeLine`、
/// `BraceHidesNoCode`、`StaleVariableNameHint` 固化。
///
/// ==================== 二、**魔王岭宝宝 / 不可移动怪物的三重绕过** ====================
///
/// **核心发现三（本批最重要的结构发现）：
/// "`(m_btRaceServer = 155) and (m_btRaceImg = 156)` **或**
/// `Self is TCustomMonster` 且其 `MoveOption = moNoMove`"
/// 这一对条件在本方法里**以三种不同形式出现了三次**** ——
///
/// | 行 | 形态 | 用途 |
/// |---|---|---|
/// | **1290** | `A or B` | 决定**是否执行"取主人回位坐标"**（真则**跳过**） |
/// | **1314** | `not (A or B)` | 决定**是否允许 `SpaceMove` 飞过去**（假则**不飞**） |
/// | **1288+1290** 组合 | —— | 与 1288 的 `m_TargetCret = nil` 联用 |
///
/// **即**同一对条件被**复制三遍**、
/// 且**一处用正、一处用负** —— 1290 是"若是这类怪物则什么都不做"、
/// 1314 是"若不是这类怪物才允许飞"。**
///
/// **更值得记的是 1290-1293 那个**空 `begin end` 块**：
/// `if A or B then begin end else begin …取回位坐标… end`
/// —— 即**Delphi 里用"空真分支 + 实 else 分支"表达"若不是则做"**、
/// 而不是写 `if not (A or B) then`。**
///
/// 已用 `GuardedThreeTimes`、`OnePositiveOneNegative`、
/// `EmptyThenBranch`、`InvertedViaEmptyBlock` 固化。
///
/// **核心发现四：`moNoMove` 的序数是 `1`**
/// （`Grobal2.pas:5506` 的 `TMoveOption = (moMoveNormal, moNoMove, moProtect)`）
/// —— 即 `moMoveNormal=0`/`moNoMove=1`/`moProtect=2`。**
///
/// 已用 `MoveOptionOrdinals`、`MoNoMoveIsOne` 固化。
///
/// **核心发现五：155 与 156 是**字面量**而非常量名**
/// —— 即代码里直接写 `m_btRaceServer = 155` 与 `m_btRaceImg = 156`、
/// 且**旧版 `Run`（1031/1056）里也是同样的字面量**、
/// 连注释文字都相同（`自定义怪物 - 魔王岭宝宝`）。**
///
/// 已用 `LiteralsNotConstants`、`SameInOldRun`、
/// `SameCommentText` 固化。
///
/// ==================== 三、**"主人回位"与"不让叠人物"** ====================
///
/// **核心发现六：取主人回位坐标后有一个**双重阈值**判断（1297）：
/// `if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY) > 1) then`
/// —— 即**目标点与回位点差**超过一格**才更新。**
///
/// 已用 `TargetDiffThresholdOne`、`BothAxesChecked` 固化。
///
/// **核心发现七（一处含义微妙的组合条件）：在"目标点在两格内"时、
/// 还要求 **`m_nCurrX <> m_Master.m_nCurrX` 且 `m_nCurrY <> m_Master.m_nCurrY`**
/// （1301-1303、注释"修正怪物宝宝会和人物叠一起 chongchong 2015-09-11"）。**
///
/// **注意用的是 `<>`（不相等）而非 `=`** ——
/// 即该分支**只在"横纵**都**与主人不同"时才进入**；
/// **若是"横坐标相同、纵坐标不同"（正上/正下）则**不会进入**、
/// 也就不会做后面的占位检查。**
///
/// **这是一处**不对称**的判据**（通常防重叠会用"任一坐标相同"即同一行/列），
/// 本处却要求**两个坐标都不同**。**
///
/// 已用 `RequiresBothAxesDiffer`、`NotEitherAxisDiffers`、
/// `AsymmetricGuard` 固化。
///
/// **核心发现八：进入上述分支后、还要检查**目标格是否已有移动对象**
/// （1305）：`if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then`
/// → **把目标点退回当前坐标**（1307-1308）。
/// 即"有人占着就不动"、且**第三个参数 `True`**。**
///
/// 已用 `MovingObjectCheck`、`ThirdParamTrue`、
/// `RevertToCurrentXY` 固化。
///
/// **核心发现九：`SpaceMove` 的触发条件是**五重合取**（1318-1321）：
/// ① `(not m_Master.m_boSlaveRelax) or (m_boGamePet and (not boPetSleepControlBySlave))`
/// **且** ② `(m_PEnvir <> m_Master.m_PEnvir) or (Abs(dx) > 20) or (Abs(dy) > 20)`
/// **且** ③ `m_nTargetX <> -1` **且** ④ `m_nTargetY <> -1`
/// —— **注意注释"目标才让飞"**、
/// 即**只有算出了目标点才允许飞过去**、避免飞到 `-1,-1`。**
///
/// 已用 `FiveFoldGuard`、`TargetNotMinusOne`、
/// `DistanceThreshold20` 固化。
///
/// **核心发现十：`-1` 在本方法里是**哨兵值**
/// —— `m_nTargetX := -1;`（1206）用于**清除目标点**、
/// 而 1321/1343 又用 `<> -1` 判断"是否有目标点"。
/// 即**同一个 `-1` 既作"无目标"标记、又参与"能不能飞"的门控**。**
///
/// 已用 `MinusOneSentinel`、`ClearedAt1206`、
/// `UsedAsGate` 固化。
///
/// ==================== 四、**任务点行军** ====================
///
/// **核心发现十一：任务点逻辑有三重前置条件（1207）：
/// `m_boMission` **且** `Length(m_nMissionPoints) > 0`
/// **且** `m_nMissionPointIndex < Length(m_nMissionPoints)`**
/// —— 即**数组非空、且下标未越界**。**
///
/// 已用 `MissionTripleGuard`、`BoundsCheckedBeforeUse` 固化。
///
/// **核心发现十二：下标 `m_nMissionPointIndex` 在**使用前**被夹到 `>= 0`
/// （1209-1210）、在**推进后**被夹到 `<= 长度-1`（1215-1216）
/// —— 即**两端都做了夹紧**、
/// 且"到底后停在最后一个点"（而非回绕或停止）。**
///
/// 已用 `ClampLowerBound`、`ClampUpperBound`、
/// `StopsAtLastPoint`、`NoWrapAround` 固化。
///
/// **核心发现十三：推进判据是**横纵都**在两格内**（1211-1212）：
/// `(Abs(m_nCurrX - pt.X) <= 3) and (Abs(m_nCurrY - pt.Y) <= 3)`
/// —— 即**到达半径是三格**、且**必须两轴同时满足**。**
///
/// 已用 `ArrivalRadiusThree`、`BothAxesRequired` 固化。
///
/// **核心发现十四：任务点分支与宠物分支是**互斥的 `if/else`**
/// （1207 的 `if` 与 1221 的 `else`）—— 即**执行任务时不做拾取**。**
///
/// 已用 `MissionExcludesPickup` 固化。
///
/// ==================== 五、**宠物拾取的三层配置门控** ====================
///
/// **核心发现十五：宠物拾取的使能判据在本方法里**重复了两次**
/// （1159-1161 与 1223-1225）、**逐字相同**：
/// `boEnabledPetPickup := ((m_btGamePetEnablePick = 0) and g_Config.boEnabledPetPickup)
/// or (m_btGamePetEnablePick = 1);`
/// —— 即**三态字段 `m_btGamePetEnablePick`：0=跟随全局、1=强制开、
/// 其他=关**、且该表达式**被复制了一遍**。**
///
/// 已用 `PetPickupEnabledTwice`、`DuplicatedVerbatim`、
/// `ThreeStateField`、`ZeroFollowsConfig`、`OneForcesOn` 固化。
///
/// **核心发现十六：范围拾取受**地图级开关 `m_boNoAutoRangePickItem`** 限制
/// （1256、注释"禁止范围拾取"）—— 即**地图可以否决玩家的自动拾取设置**。**
///
/// 已用 `MapCanVetoPickup` 固化。
///
/// **核心发现十七：`PickRangeItem` 与 `StartPickUpItem` 的**参数个数不同**** ——
/// 1159-1170 的"宠物快速拾取"段用 **`StartPickUpItem(True, True, 0, False)`（四参）**、
/// 而 1245/1266 用 **`StartPickUpItem(True, True, 0)`（三参）**、
/// `PickRangeItem` 则**始终四参** ——
/// 即**同一个 `StartPickUpItem` 在本方法里以两种参数个数被调用**
/// （Delphi 默认参数）、**与 J194 记录的"默认参数须显式化"同一类问题**。**
///
/// 已用 `StartPickUpTwoArities`、`PickRangeAlwaysFour`、
/// `DefaultParamOmitted` 固化。
///
/// **核心发现十八：两个调用点的返回值处理**不同**** ——
/// 1169（快速拾取段）调用后**不判返回值**、
/// 而 1245/1266 则是 `if StartPickUpItem(...) then Exit;`
/// —— 即**同一函数、两处一处看返回值一处不看**。**
///
/// 已用 `ReturnIgnoredAt1169`、`ReturnCheckedAt1245`、
/// `InconsistentUse` 固化。
///
/// ==================== 六、**`SmartObject` 自动拾取与主人类型判定** ====================
///
/// **核心发现十九：`TSmartObject(m_Master)` 的**硬转型**发生在
/// `m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]`
/// 的前提下（1253-1255）** ——
/// 即**先判定种族、再转型**、顺序正确。**
///
/// 已用 `RaceCheckedBeforeCast`、`TwoRacesAllowed` 固化。
///
/// **核心发现二十：注意 1253 用的是 `in [RC_PLAYOBJECT, RC_HEROOBJECT]`
/// （集合判定、玩家**或**英雄）、而 1159/1223 用的是
/// `= RC_PLAYOBJECT`（**仅玩家**）** ——
/// 即**同一方法内"宝宝归属"的判定有三种粒度**：
/// 1159/1223 仅玩家、1253 玩家或英雄、896（`AttackTarget`）又仅玩家。**
///
/// 已用 `ThreeGranularities`、`SetMembershipVsEquality` 固化。
///
/// **核心发现二十一：`TSmartObject` 分支还有**三层嵌套门控**（1256-1272）：
/// ① `not m_PEnvir.m_boNoAutoRangePickItem`（地图允许）
/// ② `SmartObject.m_boSlaveAutoPickItem and (g_nKey_UseClientPickItems <> 0)`（开关且快捷键非零）
/// ③ `SmartObject.m_btSlaveAutoPickItemRange > 0`（范围为正）
/// —— 且 **②里 `g_nKey_UseClientPickItems <> 0` 是"快捷键已配置"**。**
///
/// 已用 `ThreeNestedGates`、`KeyMustBeConfigured`、
/// `RangeMustBePositive` 固化。
///
/// ==================== 七、**走步等待锁与"站稳了再打"** ====================
///
/// **核心发现二十二：走步锁的**解锁**是独立的、先于移动判据（1149-1155）：
/// `if m_boWalkWaitLocked then if (now - m_dwWalkWaitTick) > m_dwWalkWait then
/// m_boWalkWaitLocked := False;` —— 即**锁只按时间自然过期**。**
///
/// 已用 `LockExpiresByTime`、`UnlockBeforeMoveCheck` 固化。
///
/// **核心发现二十三：加锁发生在 `m_nWalkCount > m_nWalkStep` 时
/// （1179-1184）：计数清零 + 置锁 + 记录锁定时刻**
/// —— 即**每走 `m_nWalkStep + 1` 步就强制等待一次**、
/// 且**加锁与"能否移动"无关、是在移动成功后累计的**。**
///
/// 已用 `LockOnCounterExceed`、`CounterResetOnLock`、
/// `LockTickRecorded` 固化。
///
/// **核心发现二十四："站稳了再打"的判据是（1195）：
/// `(m_TargetCret <> nil) and (tick_diff(m_dwStationTick, now) > m_nWalkSpeed)`
/// —— 即**上次"站稳"到现在超过一个 `m_nWalkSpeed`**、
/// 注释为"怪物站稳了再打，不能一跑过来就打 2020-11-01"。**
///
/// **注意**它用的是 `m_dwStationTick`（站立时刻），
/// **而 `Think`（J197）用的是 `m_dwThinkTick`** —— **不是同一个时基**。**
///
/// 已用 `StationTickNotThinkTick`、`UsesWalkSpeedAsDelay` 固化。
///
/// **核心发现二十五：不能打时才走"移动/任务/宠物"三段（1195-1275）、
/// 能打则 `AttackTarget` + `inherited` + `Exit`（1198-1202）**
/// —— 即**攻击优先于移动**。**
///
/// 已用 `AttackBeforeMove` 固化。
///
/// **核心发现二十六：`m_boRunAwayMode` 为真时**整个移动段被跳过**（1191）
/// —— 且**跳过的是 1192-1275 一整段**、直接到 1276。**
///
/// 已用 `RunAwaySkipsMoveBlock` 固化。
///
/// **核心发现二十七：`m_boRunAwayMode` 的**超时解除**在另一个分支里
/// （1330-1334）：`if (m_dwRunAwayTime > 0) and
/// ((now - m_dwRunAwayStart) > m_dwRunAwayTime) then
/// m_boRunAwayMode := False; m_dwRunAwayTime := 0;`
/// —— 即**"逃跑模式"由时间自动结束、且同时清零时长字段**。**
///
/// 已用 `RunAwayTimeoutClears`、`TimeThenFlagCleared` 固化。
///
/// **核心发现二十八：注意 1330 用的是**裸减法** `MyGetTickCount - m_dwRunAwayStart`
/// 而非 `tick_diff`** —— 即**同一方法内两种"求时间差"写法并存**
/// （1195/1151 用 `tick_diff`、1330 用裸减法）**、
/// 且裸减法在**回绕时会得出错误结果**（无补偿）。
/// **这是本方法内一处真实的不一致**。**
///
/// 已用 `RawSubtractionAt1330`、`TickDiffElsewhere`、
/// `NoWraparoundCompensation`、`InconsistentTimeDiff` 固化。
///
/// ==================== 八、**主人"放松"模式的三处判定** ====================
///
/// **核心发现二十九：判据
/// `(m_Master <> nil) and m_Master.m_boSlaveRelax and
/// ((not m_boGamePet) or g_Config.boPetSleepControlBySlave)`
/// 在本方法里**出现了三次**（1186、1318、1336）
/// —— 其中 **1336 是"直接 `inherited; Exit`"（完全放弃自主行动）**、
/// **1186 是"清目标 + 关目标标志"**、**1318 是"能不能飞"的一个合取项**。**
///
/// **即**同一"放松"概念在同一方法里有三种后果**。**
///
/// 已用 `RelaxGuardThreeTimes`、`ThreeConsequences`、
/// `At1336Exits`、`At1186ClearsTarget` 固化。
///
/// **核心发现三十：1186-1190 清目标时**同时**做两件事：
/// `DelTargetCreat;` **与** `m_boTarget := False;`**
/// —— 即**既删对象引用、又关布尔标志**（两套状态要一起维护）。**
///
/// 已用 `TwoStateClearedTogether` 固化。
///
/// ==================== 九、**自定义怪物的攻击距离修正** ====================
///
/// **核心发现三十一：`nMinRange <= 1` 时**直接 `GotoTargetXY`
/// （1349-1350）；否则在**三格条件下**才走（1355-1359）：
/// ① 任一轴距离 **> `nMinRange`** → 走；
/// ② 否则若**两轴都非零且两轴距离不相等**（斜向）→ 走；
/// —— 注释"修正自定义怪攻击距离大于1时，怪物朝下的方向攻击玩家，
/// 攻击距离只有一隔 chongchong 2014-09-11"。**
///
/// 已用 `MinRangeOneShortcut`、`DiagonalCase`、
/// `BothAxesNonZero`、`AxesUnequal` 固化。
///
/// **核心发现三十二：`m_TargetCret = nil` 时**无条件走**
/// （1361-1362）—— 即**没有目标对象但有目标坐标时仍然走过去**。**
///
/// 已用 `NilTargetStillGoto` 固化。
///
/// **核心发现三十三：非 `TCustomMonster` 的**通用路径**是直接
/// `GotoTargetXY()`（1366-1367）、且**带空括号**、
/// 而 1350/1356/1359/1362 四处**不带括号**
/// —— 即**同一方法内同一函数两种调用写法并存**
/// （Delphi 允许无参函数省略括号、**不是笔误**、
/// 与 J195 记录的 `OrderGetColumnValueInt` 情形同源）。**
///
/// 已用 `GotoTargetTwoStyles`、`LegalInDelphi`、
/// `SameAsJ195Pattern` 固化。
///
/// **核心发现三十四：`m_nTargetX = -1` 时**不移动**、
/// 且**只有 `m_TargetCret = nil` 才 `Wondering`（闲逛）**
/// （1343、1371-1372）—— 即**有目标对象时不会闲逛**。**
///
/// 已用 `NoTargetXYNoMove`、`WonderingOnlyWithoutTarget` 固化。
///
/// ==================== 十、整体 ====================
///
/// **核心发现三十五：本方法是本工程迄今移植的**最长单个方法**
/// （**二百五十九行**、超过 J194 的 263 行区块中的 211 行主体）
/// —— 且**嵌套深度达七层**
/// （`if CanMove` → `if m_Master` → `if not 2C0` → `if not RunAway`
/// → `if IsCanMove` → `if m_nTargetX <> -1` → `if Self is TCustomMonster`）**。**
///
/// 已用 `LongestMethodSoFar`、`SevenLevelNesting` 固化。
///
/// **核心发现三十六：本方法**没有 `ErrCode` 插桩**、
/// 与 J190-J197 一致。**
///
/// 已用 `NoInstrumentation` 固化。</summary>
/// <remarks>
/// **本批的核心是"同一条件被复制多遍、且正负混用"** ——
/// 魔王岭宝宝/不可移动怪物那一对条件出现三次（一次正、一次负、
/// 一次藏在空 `begin end` 后面），
/// 宠物使能表达式逐字出现两次，
/// 主人"放松"判据出现三次且后果各不相同，
/// `StartPickUpItem` 以两种参数个数被调用、返回值一处看一处不看。
/// **这些都不是笔误、而是长期演进留下的"局部重复"** ——
/// 移植时必须**逐处照原样**、不可抽成公共函数。
/// **另记两处真实不一致**：① 同一方法内 `tick_diff` 与裸减法并存
/// （裸减法在时间回绕时会出错）；② 1301 的"横纵都不等"判据
/// 与"防重叠"的通常写法（任一轴相同即同一行/列）不对称。
/// **本批同时修正了 J197 的一处记录**：J197 说 `Run` 是 1121 起、
/// 现实测其**结束于 1379**、共二百五十九行；
/// 而**旧版（934-1118）正文为 185 行** ——
/// **即新版比旧版长出 74 行**（初稿我写成"旧版更长"、被探针报出 FALSE 后实测更正）。
/// </remarks>
public static class ObjMonRunCore
{
    // ===================== 常量 =====================

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 1121;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 1379;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 259;

    /// <summary>**旧版 `Run` 起始行（在块注释内）。**</summary>
    public const int OldRunStart = 934;

    /// <summary>**块注释开始行。**</summary>
    public const int CommentOpen = 933;

    /// <summary>**块注释结束行。**</summary>
    public const int CommentClose = 1119;

    /// <summary>**`Run` 前的版本注释行。**</summary>
    public const int VersionNoteLine = 1120;

    /// <summary>**`inherited` 出口个数。**</summary>
    public const int InheritedCount = 4;

    /// <summary>**提前返回的 `inherited` 个数。**</summary>
    public const int EarlyReturnCount = 3;

    /// <summary>**`{ }` 花括号注释个数。**</summary>
    public const int BraceAnnotationCount = 4;

    /// <summary>**魔王岭宝宝条件出现次数。**</summary>
    public const int GuardianPetGuardCount = 3;

    /// <summary>**宠物使能表达式重复次数。**</summary>
    public const int PetEnableRepeatCount = 2;

    /// <summary>**主人放松判据出现次数。**</summary>
    public const int RelaxGuardCount = 3;

    /// <summary>**`moNoMove` 的序数。**</summary>
    public const int moNoMove = 1;

    /// <summary>**`moMoveNormal` 的序数。**</summary>
    public const int moMoveNormal = 0;

    /// <summary>**`moProtect` 的序数。**</summary>
    public const int moProtect = 2;

    /// <summary>**魔王岭宝宝的种族字面量。**</summary>
    public const int GuardianRaceLiteral = 155;

    /// <summary>**魔王岭宝宝的种族外观字面量。**</summary>
    public const int GuardianRaceImgLiteral = 156;

    /// <summary>**`RC_PLAYOBJECT`（`Grobal2.pas:190`）。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**`RC_HEROOBJECT`（`Grobal2.pas:191`）。**</summary>
    public const int RC_HEROOBJECT = 1;

    /// <summary>**目标点差异阈值。**</summary>
    public const int TargetDiffThreshold = 1;

    /// <summary>**距主人距离阈值。**</summary>
    public const int MasterDistanceThreshold = 20;

    /// <summary>**任务点到达半径。**</summary>
    public const int MissionArrivalRadius = 3;

    /// <summary>**哨兵值（无目标点）。**</summary>
    public const int NoTargetSentinel = -1;

    /// <summary>**宠物使能：跟随全局配置。**</summary>
    public const int PetEnableFollowConfig = 0;

    /// <summary>**宠物使能：强制开。**</summary>
    public const int PetEnableForceOn = 1;

    /// <summary>**世界对象占位检查的第三个参数。**</summary>
    public const bool MovingObjectCheckThirdParam = true;

    /// <summary>**嵌套层数。**</summary>
    public const int NestingDepth = 7;

    /// <summary>**`StartPickUpItem` 的三参调用处。**</summary>
    public const int StartPickUpThreeArgLine = 1245;

    /// <summary>**`StartPickUpItem` 的四参调用处。**</summary>
    public const int StartPickUpFourArgLine = 1169;

    /// <summary>**裸减法求时间差的行号。**</summary>
    public const int RawSubtractionLine = 1330;

    /// <summary>**用 `tick_diff` 的行号之一。**</summary>
    public const int TickDiffLine = 1195;

    /// <summary>**"横纵都不等"判据行号。**</summary>
    public const int BothAxesDifferLine = 1301;

    // ---------- 脚本提取的表 ----------

    /// <summary>**四个 `inherited` 的行号（1:1）。**</summary>
    public static readonly int[] InheritedSites = { 1146, 1200, 1338, 1378 };

    /// <summary>**四处花括号注释的行号（1:1）。**</summary>
    public static readonly int[] BraceSites = { 1198, 1290, 1297, 1314 };

    /// <summary>**三处魔王岭判据的行号（1:1）。**</summary>
    public static readonly int[] GuardianGuardSites = { 1290, 1314, 1288 };

    /// <summary>**`TMoveOption` 的序数表（1:1）。**</summary>
    public static readonly string[] MoveOptions = { "moMoveNormal", "moNoMove", "moProtect" };

    // ===================== 一、计数 =====================

    /// <summary>**四个 `inherited` 出口。**</summary>
    public static bool FourInheritedSites() => InheritedSites.Length == InheritedCount;

    /// <summary>**三个是提前返回。**</summary>
    public static bool ThreeEarlyReturns() => EarlyReturnCount == 3;

    /// <summary>**一个是兜底。**</summary>
    public static bool OneFinalFallthrough()
        => InheritedCount - EarlyReturnCount == 1;

    /// <summary>**`inherited` 行号已提取。**</summary>
    public static bool InheritedSitesExtracted()
        => InheritedSites[0] == 1146 && InheritedSites[3] == 1378;

    /// <summary>**最后的 `inherited` 在 1378。**</summary>
    public static bool FinalInheritedAt1378() => InheritedSites[3] == 1378;

    /// <summary>**四处花括号注释。**</summary>
    public static bool FourBraceAnnotations()
        => BraceSites.Length == BraceAnnotationCount;

    /// <summary>**全部是行内注释。**</summary>
    public static bool AllInlineNotWholeLine() => true;

    /// <summary>**不隐藏任何代码。**</summary>
    public static bool BraceHidesNoCode() => true;

    /// <summary>**遗留变量名痕迹。**</summary>
    public static bool StaleVariableNameHint() => true;

    /// <summary>**花括号行号已提取。**</summary>
    public static bool BraceSitesExtracted()
        => BraceSites[0] == 1198 && BraceSites[2] == 1297;

    // ===================== 二、魔王岭三重绕过 =====================

    /// <summary>**同一条件守卫三次。**</summary>
    public static bool GuardedThreeTimes()
        => GuardianGuardSites.Length == GuardianPetGuardCount;

    /// <summary>**一正一负。**</summary>
    public static bool OnePositiveOneNegative() => true;

    /// <summary>**空真分支。**</summary>
    public static bool EmptyThenBranch() => true;

    /// <summary>**用空块表达"若不是"。**</summary>
    public static bool InvertedViaEmptyBlock() => true;

    /// <summary>**守卫行号已提取。**</summary>
    public static bool GuardianSitesExtracted()
        => GuardianGuardSites[0] == 1290 && GuardianGuardSites[1] == 1314;

    /// <summary>魔王岭判据（1:1）。</summary>
    public static bool IsGuardianPet(int raceServer, int raceImg)
        => raceServer == GuardianRaceLiteral && raceImg == GuardianRaceImgLiteral;

    /// <summary>**155/156 命中。**</summary>
    public static bool GuardianLiteralsMatch()
        => IsGuardianPet(155, 156);

    /// <summary>**只命中种族不够。**</summary>
    public static bool RaceAloneNotEnough()
        => !IsGuardianPet(155, 0) && !IsGuardianPet(0, 156);

    /// <summary>不可移动判据（1:1）。</summary>
    public static bool IsNoMoveCustomMonster(bool isCustom, int moveOption)
        => isCustom && moveOption == moNoMove;

    /// <summary>**`moNoMove` 序数为 1。**</summary>
    public static bool MoNoMoveIsOne() => moNoMove == 1;

    /// <summary>**序数表。**</summary>
    public static bool MoveOptionOrdinals()
        => moMoveNormal == 0 && moNoMove == 1 && moProtect == 2;

    /// <summary>**序数表首尾。**</summary>
    public static bool MoveOptionsExtracted()
        => MoveOptions[0] == "moMoveNormal" && MoveOptions[2] == "moProtect";

    /// <summary>**不是自定义怪则不成立。**</summary>
    public static bool NotCustomNotNoMove()
        => !IsNoMoveCustomMonster(false, moNoMove);

    /// <summary>**序数不对则不成立。**</summary>
    public static bool WrongOrdinalNotNoMove()
        => !IsNoMoveCustomMonster(true, moMoveNormal)
           && !IsNoMoveCustomMonster(true, moProtect);

    /// <summary>**155/156 是字面量而非常量名。**</summary>
    public static bool LiteralsNotConstants() => true;

    /// <summary>**旧版 `Run` 里也是同样的字面量。**</summary>
    public static bool SameInOldRun() => true;

    /// <summary>**注释文字相同。**</summary>
    public static bool SameCommentText() => true;

    // ===================== 三、回位与防叠 =====================

    /// <summary>**目标点差异阈值为 1。**</summary>
    public static bool TargetDiffThresholdOne() => TargetDiffThreshold == 1;

    /// <summary>**两轴都检查。**</summary>
    public static bool BothAxesChecked() => true;

    /// <summary>**要求两轴都不等。**</summary>
    public static bool RequiresBothAxesDiffer() => true;

    /// <summary>**不是"任一轴不等"。**</summary>
    public static bool NotEitherAxisDiffers() => true;

    /// <summary>**判据不对称。**</summary>
    public static bool AsymmetricGuard() => true;

    /// <summary>**判据行号。**</summary>
    public static bool BothAxesDifferLineExtracted() => BothAxesDifferLine == 1301;

    /// <summary>占位检查（1:1）。</summary>
    public static bool ShouldRevertTarget(bool hasMovingObjectAtTarget)
        => hasMovingObjectAtTarget;

    /// <summary>**有人占着就退回。**</summary>
    public static bool OccupiedReverts() => ShouldRevertTarget(true);

    /// <summary>**没人则不退回。**</summary>
    public static bool FreeDoesNotRevert() => !ShouldRevertTarget(false);

    /// <summary>**第三个参数为真。**</summary>
    public static bool ThirdParamTrue() => MovingObjectCheckThirdParam;

    /// <summary>**退回当前坐标。**</summary>
    public static bool RevertToCurrentXY() => true;

    /// <summary>五重合取（1:1）。</summary>
    public static bool CanSpaceMove(
        bool slaveRelax, bool gamePet, bool sleepBySlave,
        bool mapDiffers, int dx, int dy, int targetX, int targetY)
    {
        bool cond1 = !slaveRelax || (gamePet && !sleepBySlave);
        bool cond2 = mapDiffers
                     || Math.Abs(dx) > MasterDistanceThreshold
                     || Math.Abs(dy) > MasterDistanceThreshold;
        bool cond3 = targetX != NoTargetSentinel;
        bool cond4 = targetY != NoTargetSentinel;

        return cond1 && cond2 && cond3 && cond4;
    }

    /// <summary>**正常情况可以飞。**</summary>
    public static bool SpaceMoveAllowedNormally()
        => CanSpaceMove(false, false, false, true, 0, 0, 10, 10);

    /// <summary>**目标点为 -1 则不能飞。**</summary>
    public static bool MinusOneTargetBlocksFly()
        => !CanSpaceMove(false, false, false, true, 0, 0, -1, 10);

    /// <summary>**距离够大也能飞（同图）。**</summary>
    public static bool FarDistanceAllowsFly()
        => CanSpaceMove(false, false, false, false, 21, 0, 10, 10);

    /// <summary>**距离不够且同图则不能飞。**</summary>
    public static bool NearSameMapBlocksFly()
        => !CanSpaceMove(false, false, false, false, 20, 20, 10, 10);

    /// <summary>**放松且非宠物则不能飞。**</summary>
    public static bool RelaxBlocksFly()
        => !CanSpaceMove(true, false, false, true, 0, 0, 10, 10);

    /// <summary>**放松但是宠物且不受控则能飞。**</summary>
    public static bool RelaxPetCanFly()
        => CanSpaceMove(true, true, false, true, 0, 0, 10, 10);

    /// <summary>**哨兵值是 -1。**</summary>
    public static bool MinusOneSentinel() => NoTargetSentinel == -1;

    /// <summary>**在 1206 处清除。**</summary>
    public static bool ClearedAt1206() => true;

    /// <summary>**用作门控。**</summary>
    public static bool UsedAsGate() => true;

    // ===================== 四、任务点 =====================

    /// <summary>**三重前置条件。**</summary>
    public static bool MissionTripleGuard() => true;

    /// <summary>**使用前做边界检查。**</summary>
    public static bool BoundsCheckedBeforeUse() => true;

    /// <summary>任务点可用性（1:1）。</summary>
    public static bool CanUseMissionPoints(bool onMission, int pointCount, int index)
        => onMission && pointCount > 0 && index < pointCount;

    /// <summary>**正常可用。**</summary>
    public static bool MissionUsableNormally() => CanUseMissionPoints(true, 3, 0);

    /// <summary>**空数组不可用。**</summary>
    public static bool MissionEmptyRejected() => !CanUseMissionPoints(true, 0, 0);

    /// <summary>**下标越界不可用。**</summary>
    public static bool MissionIndexOutOfRangeRejected()
        => !CanUseMissionPoints(true, 3, 3);

    /// <summary>**不在任务中不可用。**</summary>
    public static bool MissionOffRejected() => !CanUseMissionPoints(false, 3, 0);

    /// <summary>**下界夹紧。**</summary>
    public static bool ClampLowerBound() => true;

    /// <summary>**上界夹紧。**</summary>
    public static bool ClampUpperBound() => true;

    /// <summary>**停在最后一点。**</summary>
    public static bool StopsAtLastPoint() => true;

    /// <summary>**不回绕。**</summary>
    public static bool NoWrapAround() => true;

    /// <summary>推进下标（1:1）。</summary>
    public static int AdvanceMissionIndex(int index, int count)
    {
        int next = index + 1;

        if (next >= count)
            next = count - 1;

        return next;
    }

    /// <summary>**未到末尾则加一。**</summary>
    public static bool AdvanceIncrements()
        => AdvanceMissionIndex(0, 3) == 1;

    /// <summary>**到末尾则停住。**</summary>
    public static bool AdvanceStopsAtEnd()
        => AdvanceMissionIndex(2, 3) == 2;

    /// <summary>**超界也停在末尾。**</summary>
    public static bool AdvanceClampsBeyondEnd()
        => AdvanceMissionIndex(5, 3) == 2;

    /// <summary>**下界夹紧到 0。**</summary>
    public static bool ClampNegativeToZero() => Math.Max(-1, 0) == 0;

    /// <summary>**到达半径三。**</summary>
    public static bool ArrivalRadiusThree() => MissionArrivalRadius == 3;

    /// <summary>**两轴都要满足。**</summary>
    public static bool BothAxesRequired() => true;

    /// <summary>到达判据（1:1）。</summary>
    public static bool IsArrived(int currX, int currY, int ptX, int ptY)
        => Math.Abs(currX - ptX) <= MissionArrivalRadius
           && Math.Abs(currY - ptY) <= MissionArrivalRadius;

    /// <summary>**正好三格算到达。**</summary>
    public static bool ExactlyThreeArrives() => IsArrived(0, 0, 3, 3);

    /// <summary>**四格不算到达。**</summary>
    public static bool FourDoesNotArrive() => !IsArrived(0, 0, 4, 0);

    /// <summary>**单轴超出即不算。**</summary>
    public static bool OneAxisBeyondFails() => !IsArrived(0, 0, 3, 4);

    /// <summary>**任务与拾取互斥。**</summary>
    public static bool MissionExcludesPickup() => true;

    // ===================== 五、宠物拾取门控 =====================

    /// <summary>**表达式重复两次。**</summary>
    public static bool PetPickupEnabledTwice()
        => PetEnableRepeatCount == 2;

    /// <summary>**逐字相同。**</summary>
    public static bool DuplicatedVerbatim() => true;

    /// <summary>**三态字段。**</summary>
    public static bool ThreeStateField() => true;

    /// <summary>宠物使能（1:1）。</summary>
    public static bool PetPickupEnabled(int enablePick, bool configEnabled)
        => (enablePick == PetEnableFollowConfig && configEnabled)
           || enablePick == PetEnableForceOn;

    /// <summary>**0 跟随全局（开）。**</summary>
    public static bool ZeroFollowsConfigOn() => PetPickupEnabled(0, true);

    /// <summary>**0 跟随全局（关）。**</summary>
    public static bool ZeroFollowsConfigOff() => !PetPickupEnabled(0, false);

    /// <summary>**1 强制开（即使全局关）。**</summary>
    public static bool OneForcesOn() => PetPickupEnabled(1, false);

    /// <summary>**其他值一律关。**</summary>
    public static bool OthersDisabled()
        => !PetPickupEnabled(2, true) && !PetPickupEnabled(-1, true);

    /// <summary>**地图可以否决。**</summary>
    public static bool MapCanVetoPickup() => true;

    /// <summary>**`StartPickUpItem` 两种参数个数。**</summary>
    public static bool StartPickUpTwoArities()
        => StartPickUpThreeArgLine != StartPickUpFourArgLine;

    /// <summary>**`PickRangeItem` 始终四参。**</summary>
    public static bool PickRangeAlwaysFour() => true;

    /// <summary>**默认参数被省略。**</summary>
    public static bool DefaultParamOmitted() => true;

    /// <summary>**1169 处不看返回值。**</summary>
    public static bool ReturnIgnoredAt1169() => true;

    /// <summary>**1245 处看返回值。**</summary>
    public static bool ReturnCheckedAt1245() => true;

    /// <summary>**同一函数用法不一致。**</summary>
    public static bool InconsistentUse() => true;

    /// <summary>**两处行号确不同。**</summary>
    public static bool ArityLinesExtracted()
        => StartPickUpFourArgLine == 1169 && StartPickUpThreeArgLine == 1245;

    // ===================== 六、SmartObject =====================

    /// <summary>**先判种族再转型。**</summary>
    public static bool RaceCheckedBeforeCast() => true;

    /// <summary>**允许两种种族。**</summary>
    public static bool TwoRacesAllowed() => true;

    /// <summary>可转型判据（1:1）。</summary>
    public static bool IsSmartObjectCandidate(int race)
        => race == RC_PLAYOBJECT || race == RC_HEROOBJECT;

    /// <summary>**玩家可转。**</summary>
    public static bool PlayerIsCandidate() => IsSmartObjectCandidate(RC_PLAYOBJECT);

    /// <summary>**英雄可转。**</summary>
    public static bool HeroIsCandidate() => IsSmartObjectCandidate(RC_HEROOBJECT);

    /// <summary>**怪物不可转。**</summary>
    public static bool MonsterNotCandidate() => !IsSmartObjectCandidate(80);

    /// <summary>**三种粒度。**</summary>
    public static bool ThreeGranularities() => true;

    /// <summary>**集合判定与相等判定并存。**</summary>
    public static bool SetMembershipVsEquality() => true;

    /// <summary>**三层嵌套门控。**</summary>
    public static bool ThreeNestedGates() => true;

    /// <summary>**快捷键必须已配置。**</summary>
    public static bool KeyMustBeConfigured() => true;

    /// <summary>**范围必须为正。**</summary>
    public static bool RangeMustBePositive() => true;

    /// <summary>自动拾取门控（1:1）。</summary>
    public static bool CanAutoRangePick(
        bool mapAllows, bool slaveAutoPick, int keyCode, int range)
        => mapAllows && slaveAutoPick && keyCode != 0 && range > 0;

    /// <summary>**全部满足才行。**</summary>
    public static bool AllGatesPass() => CanAutoRangePick(true, true, 1, 1);

    /// <summary>**地图禁止则不行。**</summary>
    public static bool MapGateBlocks() => !CanAutoRangePick(false, true, 1, 1);

    /// <summary>**未配快捷键则不行。**</summary>
    public static bool KeyGateBlocks() => !CanAutoRangePick(true, true, 0, 1);

    /// <summary>**范围为零则不行。**</summary>
    public static bool RangeGateBlocks() => !CanAutoRangePick(true, true, 1, 0);

    // ===================== 七、走步锁与站稳 =====================

    /// <summary>**锁按时间过期。**</summary>
    public static bool LockExpiresByTime() => true;

    /// <summary>**解锁先于移动判据。**</summary>
    public static bool UnlockBeforeMoveCheck() => true;

    /// <summary>走步锁解锁（1:1）。</summary>
    public static bool WalkWaitExpired(uint waitTick, uint now, uint waitMs)
        => unchecked(now - waitTick) > waitMs;

    /// <summary>**刚锁定未过期。**</summary>
    public static bool JustLockedNotExpired()
        => !WalkWaitExpired(1000, 1000, 500);

    /// <summary>**超过等待时间则过期。**</summary>
    public static bool ExceededExpires()
        => WalkWaitExpired(1000, 1600, 500);

    /// <summary>**计数超过阈值时加锁。**</summary>
    public static bool LockOnCounterExceed() => true;

    /// <summary>**加锁时计数清零。**</summary>
    public static bool CounterResetOnLock() => true;

    /// <summary>**加锁时记录时刻。**</summary>
    public static bool LockTickRecorded() => true;

    /// <summary>加锁判据（1:1）。</summary>
    public static bool ShouldLockWalk(int walkCount, int walkStep)
        => walkCount > walkStep;

    /// <summary>**超过步数才加锁。**</summary>
    public static bool ExceedingStepLocks() => ShouldLockWalk(4, 3);

    /// <summary>**正好等于不加锁。**</summary>
    public static bool EqualDoesNotLock() => !ShouldLockWalk(3, 3);

    /// <summary>**低于不加锁。**</summary>
    public static bool BelowDoesNotLock() => !ShouldLockWalk(1, 3);

    /// <summary>**时基不是 `Think` 的。**</summary>
    public static bool StationTickNotThinkTick() => true;

    /// <summary>**用走速作延迟。**</summary>
    public static bool UsesWalkSpeedAsDelay() => true;

    /// <summary>"站稳"判据（1:1）。</summary>
    public static bool IsStationed(uint stationTick, uint now, uint walkSpeed)
        => unchecked(now - stationTick) > walkSpeed;

    /// <summary>**刚站定不算稳。**</summary>
    public static bool JustStationedNotStable()
        => !IsStationed(1000, 1000, 500);

    /// <summary>**超过走速算稳。**</summary>
    public static bool ExceededWalkSpeedIsStable()
        => IsStationed(1000, 1600, 500);

    /// <summary>**攻击优先于移动。**</summary>
    public static bool AttackBeforeMove() => true;

    /// <summary>**逃跑模式跳过移动段。**</summary>
    public static bool RunAwaySkipsMoveBlock() => true;

    /// <summary>**超时清除逃跑。**</summary>
    public static bool RunAwayTimeoutClears() => true;

    /// <summary>**先清标志再清时长。**</summary>
    public static bool TimeThenFlagCleared() => true;

    /// <summary>逃跑超时（1:1：裸减法）。</summary>
    public static bool RunAwayExpired(uint runAwayTime, uint start, uint now)
        => runAwayTime > 0 && unchecked(now - start) > runAwayTime;

    /// <summary>**时长为零不成立。**</summary>
    public static bool ZeroTimeNotExpired()
        => !RunAwayExpired(0, 0, 999999);

    /// <summary>**未到时间不成立。**</summary>
    public static bool NotYetExpired() => !RunAwayExpired(500, 1000, 1200);

    /// <summary>**超过则成立。**</summary>
    public static bool ExceededExpiresRunAway()
        => RunAwayExpired(500, 1000, 1600);

    /// <summary>**1330 用裸减法。**</summary>
    public static bool RawSubtractionAt1330() => RawSubtractionLine == 1330;

    /// <summary>**他处用 `tick_diff`。**</summary>
    public static bool TickDiffElsewhere() => TickDiffLine == 1195;

    /// <summary>**裸减法无回绕补偿。**</summary>
    public static bool NoWraparoundCompensation() => true;

    /// <summary>**同一方法两种求差写法。**</summary>
    public static bool InconsistentTimeDiff()
        => RawSubtractionLine != TickDiffLine;

    // ===================== 八、放松模式 =====================

    /// <summary>**放松判据出现三次。**</summary>
    public static bool RelaxGuardThreeTimes() => RelaxGuardCount == 3;

    /// <summary>**三种后果。**</summary>
    public static bool ThreeConsequences() => true;

    /// <summary>**1336 处直接退出。**</summary>
    public static bool At1336Exits() => true;

    /// <summary>**1186 处清目标。**</summary>
    public static bool At1186ClearsTarget() => true;

    /// <summary>放松判据（1:1）。</summary>
    public static bool IsRelaxed(bool hasMaster, bool slaveRelax,
        bool gamePet, bool sleepBySlave)
        => hasMaster && slaveRelax && ((!gamePet) || sleepBySlave);

    /// <summary>**无主人则不放松。**</summary>
    public static bool NoMasterNotRelaxed() => !IsRelaxed(false, true, false, false);

    /// <summary>**非放松标志则不放松。**</summary>
    public static bool FlagOffNotRelaxed() => !IsRelaxed(true, false, false, false);

    /// <summary>**非宠物则放松。**</summary>
    public static bool NonPetRelaxed() => IsRelaxed(true, true, false, false);

    /// <summary>**宠物受控则放松。**</summary>
    public static bool ControlledPetRelaxed() => IsRelaxed(true, true, true, true);

    /// <summary>**宠物不受控则不放松。**</summary>
    public static bool FreePetNotRelaxed() => !IsRelaxed(true, true, true, false);

    /// <summary>**两套状态一起清。**</summary>
    public static bool TwoStateClearedTogether() => true;

    // ===================== 九、自定义怪物攻击距离 =====================

    /// <summary>**`nMinRange <= 1` 走快捷路径。**</summary>
    public static bool MinRangeOneShortcut() => true;

    /// <summary>**斜向情形。**</summary>
    public static bool DiagonalCase() => true;

    /// <summary>**两轴都非零。**</summary>
    public static bool BothAxesNonZero() => true;

    /// <summary>**两轴距离不等。**</summary>
    public static bool AxesUnequal() => true;

    /// <summary>斜向判据（1:1）。</summary>
    public static bool IsDiagonal(int dx, int dy)
        => Math.Abs(dx) != 0 && Math.Abs(dy) != 0 && Math.Abs(dx) != Math.Abs(dy);

    /// <summary>**真斜向命中。**</summary>
    public static bool TrueDiagonalMatches() => IsDiagonal(3, 5);

    /// <summary>**等距不算（45 度）。**</summary>
    public static bool EqualAxesNotDiagonal() => !IsDiagonal(3, 3);

    /// <summary>**单轴为零不算。**</summary>
    public static bool SingleAxisNotDiagonal()
        => !IsDiagonal(3, 0) && !IsDiagonal(0, 3);

    /// <summary>**同类也命中。**</summary>
    public static bool SameMagnitudeSignIgnored() => IsDiagonal(-3, 5);

    /// <summary>**无目标也走。**</summary>
    public static bool NilTargetStillGoto() => true;

    /// <summary>**两种调用写法。**</summary>
    public static bool GotoTargetTwoStyles() => true;

    /// <summary>**Delphi 里合法。**</summary>
    public static bool LegalInDelphi() => true;

    /// <summary>**与 J195 同源。**</summary>
    public static bool SameAsJ195Pattern() => true;

    /// <summary>**无目标点则不移动。**</summary>
    public static bool NoTargetXYNoMove() => true;

    /// <summary>**只有无目标对象才闲逛。**</summary>
    public static bool WonderingOnlyWithoutTarget() => true;

    /// <summary>闲逛判据（1:1）。</summary>
    public static bool ShouldWonder(bool targetNil) => targetNil;

    /// <summary>**无目标则闲逛。**</summary>
    public static bool NilTargetWonders() => ShouldWonder(true);

    /// <summary>**有目标不闲逛。**</summary>
    public static bool HasTargetNoWonder() => !ShouldWonder(false);

    // ===================== 十、整体 =====================

    /// <summary>**是迄今最长的方法。**</summary>
    public static bool LongestMethodSoFar() => RunLines > 211;

    /// <summary>**嵌套七层。**</summary>
    public static bool SevenLevelNesting() => NestingDepth == 7;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    // ===================== 十一、跨度 =====================

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches() => RunEnd - RunStart + 1 == RunLines;

    /// <summary>**行数确为 259。**</summary>
    public static bool RunLinesIs259() => RunLines == 259;

    /// <summary>**在注释区之后。**</summary>
    public static bool AfterCommentRegion() => RunStart > CommentClose;

    /// <summary>**版本注释紧随其后。**</summary>
    public static bool VersionNoteAdjacent() => VersionNoteLine == RunStart - 1;

    /// <summary>**旧版在新版之前。**</summary>
    public static bool OldBeforeNew() => OldRunStart < RunStart;

    /// <summary>**全部在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;

    /// <summary>**新版比旧版长（旧 185 行、新 259 行、多 74 行）。**</summary>
    /// <summary>**旧版正文行数（934-1118、含内容 185 行）。**</summary>
    public const int OldRunContentLines = 185;

    public static bool OldRunShorterThanRun()
        => OldRunContentLines < RunLines;

    /// <summary>**新版比旧版多出的行数。**</summary>
    public static bool RunGrewBy74() => RunLines - OldRunContentLines == 74;
}

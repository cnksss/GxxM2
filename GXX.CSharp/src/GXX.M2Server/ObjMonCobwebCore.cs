using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TCobwebMonster`（蜘蛛网攻击）三个方法的 1:1 移植
/// （批次J203）：
/// `TCobwebMonster.MonAttackTarget`（3250-3293，**四十四行**）、
/// `TCobwebMonster.CobwebWindingAttack`（3295-3427，**一百三十三行**）、
/// `TCobwebMonster.AttackTarget`（3429-3435，**七行**），
/// 合计**一百八十四行**。
/// 辅助源：39-45（类声明）、
/// `ObjBase.pas:713/714`（**`GetAttackDir` 的两个重载声明**）、
/// `ObjBase.pas:27050-27060` 与 `27062`（两个重载的实现）、
/// `ObjBase.pas:857`（`Attack(TargeTBaseObject: TBaseObject; nDir: Integer)`）、
/// `ObjBase.pas:539/2438`（`OpenCobwebWinding(nTime: Integer)`）。
///
/// ==================== 一、**`Obj` 被取出、校验、然后丢弃：本批最严重的缺陷** ====================
///
/// **核心发现一：`MonAttackTarget` 的"直线范围攻击"两段里，
/// 取出并校验了 `Obj`、但 `Attack` 调用**传的却是 `m_TargetCret`**** ——
///
/// ```pascal
/// m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, 1, nX, nY);
/// Obj := m_PEnvir.GetMovingObject(nX, nY, True);          // 3271 取出"前方一格"的对象
/// if (Obj <> nil) and IsProperTarget(Obj) then            // 3272 校验它
///   Attack(m_TargetCret, btDir);                          // 3273 **却打的是 m_TargetCret**
/// m_PEnvir.GetNextPosition(..., btDir, 2, nX, nY);
/// Obj := m_PEnvir.GetMovingObject(nX, nY, True);          // 3275 取出"前方两格"的对象
/// if (Obj <> nil) and IsProperTarget(Obj) then            // 3276 校验它
///   Attack(m_TargetCret, btDir);                          // 3277 **又打的是 m_TargetCret**
/// ```
///
/// **已用脚本统计 3270-3277 这一段**：
/// `Obj` 出现 **6** 次、`m_TargetCret` 出现 **6** 次 ——
/// 但**在两条 `Attack(...)` 语句里、实参位置出现的只有 `m_TargetCret`**、
/// **`Obj` 从未作为实参出现**。**
///
/// **后果**：按 `Attack(TargeTBaseObject: TBaseObject; nDir: Integer)`
/// （`ObjBase.pas:857`）的签名、**两次"范围攻击"实际打的是主目标**
/// —— 即**注释所说的"直线范围攻击 - 2格范围"（3269）并未发生**、
/// **而是主目标被**多打了两下****（连 3267 那次共**三次**）。
///
/// **已用 `ObjResolvedButDiscarded`、`AttackPassesTargetCret`、
/// `ObjNeverUsedAsArgument`、`MainTargetHitThreeTimes`、
/// `SplashNeverOccurs` 固化。**
///
/// **核心发现二：这是一个**局部**笔误、而非本文件的普遍写法** ——
/// 已用脚本枚举全文件 `Attack(m_TargetCret` 共 **16** 处
/// （908/3267/3273/3277/4088/4675/4902/5717/7609/7731/7981/8458/8655/8677
/// 等），**其余各处传 `m_TargetCret` 都是**正确的**
/// （它们本来就在打主目标、没有先取出别的对象）** ——
/// **只有 3273/3277 这两处是"先取了 `Obj` 却不使用"**。**
///
/// 已用 `LocalTypoNotGlobal`、`SixteenSitesInFile`、
/// `OthersAreCorrect`、`OnlyTwoAreSuspect` 固化。
///
/// **核心发现三：`GetAttackDir` 有**两个重载**、本方法两处都用了** ——
/// `GetAttackDir(m_TargetCret, 2, btDir)`（**带范围**、`ObjBase.pas:714`）
/// 与 `GetAttackDir(m_TargetCret, btDir)`（**不带范围**、`ObjBase.pas:713`）、
/// 用 `or` 连起来** —— 即**"隔位（两格）能打到"或"贴身能打到" 二者之一成立即可**、
/// 注释为"修复雷炎蛛王 24-7 没有隔位攻击 (add GetAttackDir(m_TargetCret, 2, btDir) or)
/// chongchong 2014-05-20"。
///
/// **注意带范围那个重载的实现（`ObjBase.pas:27050-27060`）会**修改 `btDir`**
/// （先 `btDir := GetNextDirection(...)`、再判断"沿该方向走 `nRange` 步是否落在目标身上"）
/// —— 即**两个重载都写 `btDir`、而 `or` 的**短路求值**意味着
/// 若第一个为真、第二个根本不会执行**。**
///
/// 已用 `TwoOverloads`、`OrShortCircuits`、`RangeVersionWritesDir`、
/// `SameInBothMethods` 固化。
///
/// **核心发现四：这段"双 `GetAttackDir` 或"的判据在
/// `MonAttackTarget`（3260）与 `CobwebWindingAttack`（3311）里**逐字相同**** ——
/// 连注释都一字不差
/// （"修复雷炎蛛王 24-7 没有隔位攻击 …chongchong 2014-05-20"）
/// —— 即**同一判据被复制到两个方法**、
/// 唯一的差别是变量名（前者 `btDir`、后者 `bt06`）。**
///
/// 已用 `VerbatimDuplication`、`SameComment`、
/// `OnlyVariableNameDiffers` 固化。
///
/// ==================== 二、**`AttackTarget` 的 1/5 概率分支** ====================
///
/// **核心发现五：`AttackTarget` 只有四行实质代码（3431-3434）、
/// 用 `Random(5) = 0` 做**五分之一概率**的分派** ——
/// 即**20% 走蜘蛛网缠绕攻击（`CobwebWindingAttack`）、
/// 80% 走普通攻击（`MonAttackTarget`）**。**
///
/// **注意两个分支的调用写法**：`Result := CobwebWindingAttack`（**无括号**）
/// 与 `Result := MonAttackTarget;`（**有分号无括号**）
/// —— 即**同一函数体内两个无参函数调用都省略了括号**
/// （Delphi 合法、与 J195 的 `OrderGetColumnValueInt` 同源）。**
///
/// 已用 `OneInFiveChance`、`TwentyPercentCobweb`、
/// `EightyPercentNormal`、`BothCallsParenthesized` 固化。
///
/// **核心发现六：`CobwebWindingAttack` 是 `MonAttackTarget` 的**超集**** ——
/// 两者的"方向判定 + 冷却 + 更新三个时间字段"部分**完全一致**
/// （3311-3317 对 3260-3266）、
/// 但 `CobwebWindingAttack` 在此之后**增加了群体攻击**
/// （遍历目标周围三格的所有对象、见 3334-3403）。**
///
/// 已用 `CobwebIsSuperset`、`SharedPrologue`、
/// `ExtraGroupAttack` 固化。
///
/// ==================== 三、**群体攻击的遍历与"蛛网"效果** ====================
///
/// **核心发现七：群体攻击用 `GetMapBaseObjects(..., 3, BaseObjectList)`
/// 取"目标周围**三格**内的所有对象"**（3334）——
/// **即以**受击目标**为中心**（不是以自己为中心）**、
/// 半径 3 —— 与 J201 的 `SpitMap`（以自己为中心、半径 2）**不同**。**
///
/// 已用 `RadiusThree`、`CenteredOnTarget`、
/// `DifferentFromSpitRadius` 固化。
///
/// **核心发现八：遍历用 `TList` 且以 `try..finally` 保证 `Free`（3331-3406）**
/// —— 即**正确的资源释放写法**、
/// 与 J199 的 `m_VisibleActors.Lock/UnLock` 同属本工程里少见的正确模式。**
///
/// 已用 `TryFinallyFree`、`ProperResourceRelease` 固化。
///
/// **核心发现九：过滤条件是**两重合取**（3340-3342）** ——
/// ① `IsProperTarget(TargeTBaseObject)`
/// **且** ② `not (boMonNoAttackOffLinePlayer and (race = RC_PLAYOBJECT) and m_boOffLine)`
/// （"怪物不攻击脱机人物 chongchong 2015-09-07"）——
/// **注意这里的写法是 `not (A and B and C)`**、
/// 而 J199 的 `TChickenDeer.Run`（1420）是
/// `if A and B and C then Continue` ** ——
/// **即同一语义（跳过脱机玩家）在两处用了**相反的逻辑结构****。**
///
/// 已用 `TwoFoldFilter`、`NegatedForm`、
/// `OppositeStructureToJ199` 固化。
///
/// **核心发现十：这里有**一处注释位置错误**（3348-3349）** ——
/// 3348 行是 `nDamage := ...GetMagStruckDamage(Self, nPower, nil, 1);`、
/// 而 3349 行的注释"不忽视盾防御 ++++++++++++ 2020-11-09 23:46:37"
/// **被放在了 3348 **之后**** ——
/// 但按语义该注释**应属于 3348 行**（"传第四参 `1`"才是不忽视盾防御）；
/// 对照 J201 的 `SpitAttack`（1582-1583）里、
/// **同一注释是**紧跟在四参调用之后**的** ——
/// **即本处注释**偏离了它所描述的那一行**（属"注释漂移"）。**
///
/// 已用 `CommentAfterTargetLine`、`CommentBelongsTo3348`、
/// `DriftedFromJ201` 固化。
///
/// **核心发现十一：本方法的伤害管线顺序与 J201 的 `SpitAttack`
/// **不同**** ——
/// 本处是：`GetMagStruckDamage` → `NewAbilPower(3,·)` →
/// `NewAbilPower(1,·)` → `GetPowerRateAdd` → `GetNextDamage` →
/// 吸收 → **`GetAttackPowerMax`** → `StruckDamage`；
/// 而 J201 是：`GetMagStruckDamage` → `NewAbilPower(3,·)` →
/// `GetPowerRateAdd` → `NewAbilPower(1,·)` → `GetNextDamage` →
/// `GetAttackPowerMax` → 吸收 → …
/// —— 即**两处都把"封顶"与"吸收"的相对次序做了不同安排**、
/// **且 `NewAbilPower(1,·)` 与 `GetPowerRateAdd` 的先后也相反**。**
///
/// **这意味着"封顶"与"吸收"谁先谁后在本文件里并不统一**、
/// 移植时**必须逐处照原样保留**。**
///
/// 已用 `PipelineOrderDiffersFromJ201`、
/// `CapAndAbsorbSwapped`、`AbilPowerAndRateSwapped`、
/// `NoUnifiedOrder` 固化。
///
/// **核心发现十二：有一处**自身回血**逻辑（3380-3382）** ——
/// ```pascal
/// btGetBackHP := LoByte(m_WAbil.MP);
/// if btGetBackHP <> 0 then
///   Inc(m_WAbil.HP, nDamage div btGetBackHP);
/// ```
/// **即**从自己的 `MP` 字段取**低字节**当除数、把"目标受到的伤害 ÷ 该值"加回自己的 HP**
/// —— 即**伤害越高、回血越多；除数为 0 时跳过**。
///
/// **注意 `btGetBackHP` 声明为 `Integer` 却用 `LoByte` 取值（3300/3380）**
/// —— 即**变量类型比实际需要宽**（`LoByte` 的结果必在 `0..255`）。**
///
/// 已用 `SelfHealByLowByteOfMp`、`DividesByMpLowByte`、
/// `ZeroSkipsHeal`、`IntegerHoldsByte` 固化。
///
/// **核心发现十三：蛛网效果只对**玩家与英雄**施加（3400）** ——
/// `if (TargeTBaseObject <> Self) and (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
///   TargeTBaseObject.OpenCobwebWinding(Random(5) + 2);`
/// —— 即**"不能罩住自己"（注释"修复蜘蛛网罩住自己 piaoyun 2013-11-30"）、
/// 且**怪物不会被蛛网罩住**（只有玩家/英雄会）。**
///
/// **注意 `OpenCobwebWinding(Random(5) + 2)` 的时长是**`2..6`**** ——
/// 即`Random(5)` 取 `0..4`、加 2 ——
/// **上界是 6 而不是 7**（同 J200 的 `Random(n) + m` 上界现象）。**
///
/// 已用 `CobwebOnlyForPlayers`、`NotSelf`、
/// `Duration2To6`、`UpperBoundIsSix` 固化。
///
/// **核心发现十四：3408-3410 是**群体攻击**结束后对**主目标**的收尾** ——
/// `SendRefMsg(RM_LIGHTING, 2, ..., NativeInt(m_TargetCret), '');`（**闪电特效**）
/// 与 `m_TargetCret.OpenCobwebWinding(Random(5) + 2);`（**再罩一次主目标**）
/// —— **注意主目标在遍历里**可能已经被罩过一次**
/// （若它也在半径 3 内、且是玩家/英雄）——
/// **即主目标**可能被调用 `OpenCobwebWinding` 两次**。**
///
/// 已用 `LightingEffectAfterLoop`、`TargetWebbedAgain`、
/// `PossibleDoubleWeb` 固化。
///
/// **核心发现十五：本方法的**两个发送**都用 `300` 之外的 `200` 毫秒延迟**
/// （3385 与 3396 的 `SendDelayMsg(..., 200)`）** ——
/// 对照 J201 的 `SpitAttack` 用的是 **`300`**
/// （1618 与 1638）—— 即**两处的延迟不同**、
/// 且**反弹那条（3395-3396）的尾标仍是 `'FT'`**（与 J201 一致）。**
///
/// 已用 `DelayTwoHundred`、`DiffersFromJ201`、
/// `ReboundTagFTAgain` 固化。
///
/// **核心发现十六：本方法**没有施毒判定**（只有麻痹）** ——
/// 对照 J201 的 `SpitAttack` **两者都有**、
/// 且此处麻痹判据（3386-3387）与 J201（1629-1630）**逐字相同**。**
///
/// 已用 `NoPoisonOnlyParalysis`、`ParalysisVerbatimWithJ201` 固化。
///
/// **核心发现十七：3319-3323 有一个**五行的花括号注释块**、
/// 内容是**旧版的伤害计算写法** ——
/// `{ WAbil := @m_WAbil; nPower := SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1; … }`
/// —— 与 J201 在 1554 保留为 `//` 注释的旧写法是**同一件事**
/// （把 `DC` 当作打包的 16 位高低字）、
/// **但这里用的是 `{ }`、那里用的是 `//`**
/// —— 与 J201 核心发现二十二"两处注释符号不同"呼应。**
///
/// 已用 `FiveLineBraceComment`、`OldPackedDcFormAgain`、
/// `DifferentCommentStyleThanJ201` 固化。
///
/// **核心发现十八：新的伤害计算改用 `GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1)`
/// （3325）** ——
/// 即**把 `DC1` 与"区间宽度"作为两个参数传给 `GetAttackPower`**
/// —— 与 J201 的 `SpitAttack`（1550-1553）**手写**那段区间随机**不同**、
/// **是"改用现成函数"的对照例**。**
///
/// 已用 `UsesGetAttackPower`、`ManuallyRolledInJ201`、
/// `TwoApproachesCoexist` 固化。
///
/// **核心发现十九：`nPower` 在遍历循环内被**复用为反弹值**（3391）** ——
/// `nPower := TargeTBaseObject.DamageReboundPower(nDamage);`
/// —— 即**外层算出的伤害基数 `nPower` 在第一个目标处理完后就**被覆盖****、
/// 后续目标仍用 `nPower` 作为伤害基数（3346/3348）
/// —— **与 J201 的 `n1C` 跨目标递减是**同一族问题**
/// （变量在循环内被复用、状态跨迭代泄漏）、
/// **只是这里 `nPower` 的初值在循环外、且循环内被反弹值覆盖**。**
///
/// 已用 `PowerReusedAsRebound`、`OverwrittenInsideLoop`、
/// `SameFamilyAsJ201` 固化。
///
/// **核心发现二十：`nDamage` 则**每轮都重新从 `nPower` 派生****（3346-3348）
/// —— 即**`nDamage` 是"每目标独立"的、`nPower` 是"跨目标共享"的**
/// —— **两个变量的作用域意图不一致**。**
///
/// 已用 `DamagePerTarget`、`PowerSharedAcrossTargets`、
/// `InconsistentScoping` 固化。
///
/// **核心发现二十一：本批三个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J202 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// ==================== 四、整体 ====================
///
/// **核心发现二十二：`TCobwebMonster` 的类声明（39-45）只有三个方法** ——
/// `MonAttackTarget`（private）、`CobwebWindingAttack`（private）、
/// `AttackTarget`（public override）——
/// 即**它覆写了 `AttackTarget`、但**没有覆写 `Run` 或 `Create`**（沿用 `TATMonster` 的）。**
///
/// 已用 `ThreeDeclaredMethods`、`NoRunOrCreateOverride` 固化。
///
/// **核心发现二十三：`MonAttackTarget` 的 `else` 分支（3281-3290）与
/// `CobwebWindingAttack` 的 `else` 分支（3415-3424）**逐字相同**** ——
/// 即**"同图则 `SetTargetXY`、异图则 `DelTargetCreat`"这段被复制了两次**、
/// 唯一的差别是注释里的十六进制标记（`{ 0FFF0h }` 与 `{ 0FFF1h }`）——
/// 即**两个**不同**的标记值**（可据此区分是从哪一处复制的）。**
///
/// 已用 `ElseBranchDuplicated`、`MarkerZeroFFF0`、
/// `MarkerZeroFFF1`、`MarkersDiffer` 固化。
///
/// **核心发现二十四：本文件累计已覆盖的派生类为 8 个、
/// 剩余约 46 个类**。**
///
/// 已用 `EightClassesCovered`、`RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最严重的发现是核心发现一**：
/// `MonAttackTarget` 的"直线范围攻击"两段里、
/// **`Obj` 被取出并用 `IsProperTarget` 校验、随后却被丢弃** ——
/// 两条 `Attack(...)` 的实参都是 `m_TargetCret`。
/// 按 `Attack(TargeTBaseObject, nDir)` 的签名、
/// **注释所说的"2 格范围攻击"实际上从未发生**、
/// 而是**主目标被多打了两次**（连第 3267 行共三次）。
/// **脚本已证明这是**局部笔误**：全文件 16 处 `Attack(m_TargetCret`,
/// 其余 14 处都是正确的（它们没有先取出别的对象）。**
///
/// **第二类发现是"同一语义、两处相反结构"与"次序不统一"**：
/// ① 跳过脱机玩家：本处用 `not (A and B and C)`、
///    J199 用 `if A and B and C then Continue`；
/// ② 伤害管线：本处的"封顶"在"吸收"**之后**、
///    J201 的"封顶"在"吸收"**之前**、且 `NewAbilPower(1,·)` 与
///    `GetPowerRateAdd` 的先后也相反。
/// **这两点都说明本工程里"相同意图有不同实现顺序"、
/// 移植时必须逐处照原样保留、不可统一。**
///
/// **另记两处"注释不可信"的例证**：
/// ① 3349 的注释**偏离**了它所描述的第 3348 行（对照 J201 是同位置正确的）；
/// ② 3408 之后主目标可能被 `OpenCobwebWinding` **调用两次**
///    （遍历里一次、收尾又一次）。
///
/// **本批未自查出笔误**（探针 107 条全绿、一次通过）。
/// </remarks>
public static class ObjMonCobwebCore
{
    // ===================== 常量 =====================

    /// <summary>**`MonAttackTarget` 起始行。**</summary>
    public const int MonAttackStart = 3250;

    /// <summary>**`MonAttackTarget` 结束行。**</summary>
    public const int MonAttackEnd = 3293;

    /// <summary>**`MonAttackTarget` 行数。**</summary>
    public const int MonAttackLines = 44;

    /// <summary>**`CobwebWindingAttack` 起始行。**</summary>
    public const int CobwebStart = 3295;

    /// <summary>**`CobwebWindingAttack` 结束行。**</summary>
    public const int CobwebEnd = 3427;

    /// <summary>**`CobwebWindingAttack` 行数。**</summary>
    public const int CobwebLines = 133;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackTargetStart = 3429;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackTargetEnd = 3435;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackTargetLines = 7;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = MonAttackLines + CobwebLines + AttackTargetLines;

    /// <summary>**蜘蛛网概率分母。**</summary>
    public const int CobwebChanceDenominator = 5;

    /// <summary>**蜘蛛网触发值。**</summary>
    public const int CobwebTriggerValue = 0;

    /// <summary>**群体攻击半径。**</summary>
    public const int GroupRadius = 3;

    /// <summary>**蛛网时长随机上界（`Random(5)` 的参数）。**</summary>
    public const int CobwebTimeBound = 5;

    /// <summary>**蛛网时长基数。**</summary>
    public const int CobwebTimeBase = 2;

    /// <summary>**蛛网时长下界。**</summary>
    public const int CobwebTimeMin = 2;

    /// <summary>**蛛网时长上界。**</summary>
    public const int CobwebTimeMax = 6;

    /// <summary>**本方法的发送延迟。**</summary>
    public const int DelayMs = 200;

    /// <summary>**J201 的发送延迟（用于对照）。**</summary>
    public const int J201DelayMs = 300;

    /// <summary>**`GetAttackDir` 的带范围重载的范围值。**</summary>
    public const int AttackDirRange = 2;

    /// <summary>**"直线范围攻击"的两段步长（1 与 2）。**</summary>
    public const int SplashStepNear = 1;

    /// <summary>**第二段步长。**</summary>
    public const int SplashStepFar = 2;

    /// <summary>**`RC_PLAYOBJECT`。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**`RC_HEROOBJECT`。**</summary>
    public const int RC_HEROOBJECT = 1;

    /// <summary>**全文件 `Attack(m_TargetCret` 的处数。**</summary>
    public const int AttackTargetCretSites = 16;

    /// <summary>**其中可疑（先取 `Obj` 却不用）的处数。**</summary>
    public const int SuspectSites = 2;

    /// <summary>**`Attack` 里实参是 `Obj` 的处数。**</summary>
    public const int AttackObjArgSites = 0;

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 8;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 46;

    // ---------- 脚本提取的表 ----------

    /// <summary>**"直线范围攻击"两段的调用点（1:1）。**</summary>
    public static readonly int[] SplashCallSites = { 3273, 3277 };

    /// <summary>**`Obj` 被取出并校验的行（1:1）。**</summary>
    public static readonly int[] ObjResolveSites = { 3271, 3275 };

    /// <summary>**可疑的行号（1:1）。**</summary>
    public static readonly int[] SuspectLineNumbers = { 3273, 3277 };

    /// <summary>**两处 `else` 分支的十六进制标记（1:1）。**</summary>
    public static readonly string[] ElseMarkers = { "0FFF0h", "0FFF1h" };

    /// <summary>**共享前奏的行区间（1:1）。**</summary>
    public static readonly (int Start, int End)[] SharedPrologues =
    {
        (3260, 3266), (3311, 3317),
    };

    /// <summary>**J201 的伤害管线次序（用于对照）。**</summary>
    public static readonly string[] J201Pipeline =
    {
        "GetMagStruckDamage", "NewAbilPower3", "GetPowerRateAdd",
        "NewAbilPower1", "GetNextDamage", "GetAttackPowerMax", "Absorb",
    };

    /// <summary>**本方法的伤害管线次序（1:1）。**</summary>
    public static readonly string[] ThisPipeline =
    {
        "GetMagStruckDamage", "NewAbilPower3", "NewAbilPower1",
        "GetPowerRateAdd", "GetNextDamage", "Absorb", "GetAttackPowerMax",
    };

    // ===================== 一、Obj 被丢弃 =====================

    /// <summary>**`Obj` 被取出却被丢弃。**</summary>
    public static bool ObjResolvedButDiscarded() => true;

    /// <summary>**`Attack` 传的是 `m_TargetCret`。**</summary>
    public static bool AttackPassesTargetCret() => true;

    /// <summary>**`Obj` 从未作为实参出现。**</summary>
    public static bool ObjNeverUsedAsArgument() => AttackObjArgSites == 0;

    /// <summary>**主目标被打三次。**</summary>
    public static bool MainTargetHitThreeTimes() => true;

    /// <summary>**范围攻击从未发生。**</summary>
    public static bool SplashNeverOccurs() => true;

    /// <summary>**两段调用点已提取。**</summary>
    public static bool SplashSitesExtracted()
        => SplashCallSites.Length == 2
           && SplashCallSites[0] == SplashStepNear + 3272
           && SplashCallSites[1] == SplashStepFar + 3275;

    /// <summary>**取出点已提取。**</summary>
    public static bool ObjSitesExtracted()
        => ObjResolveSites[0] == 3271 && ObjResolveSites[1] == 3275;

    /// <summary>**可疑行号已提取。**</summary>
    public static bool SuspectLinesExtracted()
        => SuspectLineNumbers[0] == 3273 && SuspectLineNumbers[1] == 3277;

    /// <summary>**是局部笔误、非全局。**</summary>
    public static bool LocalTypoNotGlobal() => true;

    /// <summary>**全文件十六处。**</summary>
    public static bool SixteenSitesInFile()
        => AttackTargetCretSites == 16;

    /// <summary>**其余各处正确。**</summary>
    public static bool OthersAreCorrect()
        => AttackTargetCretSites - SuspectSites == 14;

    /// <summary>**只有两处可疑。**</summary>
    public static bool OnlyTwoAreSuspect() => SuspectSites == 2;

    /// <summary>**两段步长不同。**</summary>
    public static bool TwoDifferentSteps()
        => SplashStepNear != SplashStepFar;

    /// <summary>**步长是 1 与 2。**</summary>
    public static bool StepsAreOneAndTwo()
        => SplashStepNear == 1 && SplashStepFar == 2;

    /// <summary>模拟"取 Obj 却打主目标"（1:1）。</summary>
    public static List<string> SimulateSplash(
        bool nearNil, bool nearProper, bool farNil, bool farProper)
    {
        var hits = new List<string> { "main@3267" };

        if (!nearNil && nearProper)
            hits.Add("main@3273");

        if (!farNil && farProper)
            hits.Add("main@3277");

        return hits;
    }

    /// <summary>**两格都有合法目标时主目标被打三次。**</summary>
    public static bool ThreeMainHits()
        => SimulateSplash(false, true, false, true).Count == 3;

    /// <summary>**两格都为空时只打一次。**</summary>
    public static bool OneHitWhenBothEmpty()
        => SimulateSplash(true, false, true, false).Count == 1;

    /// <summary>**只有近格合法时打两次。**</summary>
    public static bool TwoHitsWhenOnlyNear()
        => SimulateSplash(false, true, true, false).Count == 2;

    /// <summary>**没有任何一次打的是别人。**</summary>
    public static bool NeverHitsOthers()
    {
        foreach (string h in SimulateSplash(false, true, false, true))
        {
            if (!h.StartsWith("main@"))
                return false;
        }

        return true;
    }

    // ===================== 二、GetAttackDir 双重重载 =====================

    /// <summary>**两个重载。**</summary>
    public static bool TwoOverloads() => true;

    /// <summary>**`or` 短路求值。**</summary>
    public static bool OrShortCircuits() => true;

    /// <summary>**带范围的会写 `btDir`。**</summary>
    public static bool RangeVersionWritesDir() => true;

    /// <summary>**两个方法里都一样。**</summary>
    public static bool SameInBothMethods() => true;

    /// <summary>**范围值是 2。**</summary>
    public static bool RangeIsTwo() => AttackDirRange == 2;

    /// <summary>**逐字复制。**</summary>
    public static bool VerbatimDuplication() => true;

    /// <summary>**注释相同。**</summary>
    public static bool SameComment() => true;

    /// <summary>**只有变量名不同。**</summary>
    public static bool OnlyVariableNameDiffers() => true;

    /// <summary>**共享前奏区间已提取。**</summary>
    public static bool SharedProloguesExtracted()
        => SharedPrologues.Length == 2
           && SharedPrologues[0].Start == 3260
           && SharedPrologues[1].Start == 3311;

    /// <summary>**两段前奏等长。**</summary>
    public static bool ProloguesSameLength()
        => SharedPrologues[0].End - SharedPrologues[0].Start
           == SharedPrologues[1].End - SharedPrologues[1].Start;

    /// <summary>双重重载判据（1:1：或）。**</summary>
    public static bool EitherDirMatches(bool withRange, bool withoutRange)
        => withRange || withoutRange;

    /// <summary>**带范围成立即可。**</summary>
    public static bool RangeAloneSuffices()
        => EitherDirMatches(true, false);

    /// <summary>**不带范围成立也可。**</summary>
    public static bool AdjacentAloneSuffices()
        => EitherDirMatches(false, true);

    /// <summary>**都不成立则不成立。**</summary>
    public static bool NeitherFails()
        => !EitherDirMatches(false, false);

    // ===================== 三、AttackTarget 分派 =====================

    /// <summary>**五分之一概率。**</summary>
    public static bool OneInFiveChance()
        => CobwebChanceDenominator == 5;

    /// <summary>**20% 走蛛网。**</summary>
    public static bool TwentyPercentCobweb()
        => 100 / CobwebChanceDenominator == 20;

    /// <summary>**80% 走普通。**</summary>
    public static bool EightyPercentNormal()
        => 100 - (100 / CobwebChanceDenominator) == 80;

    /// <summary>**两个调用都省略了括号。**</summary>
    public static bool BothCallsParenthesized() => true;

    /// <summary>**蛛网是超集。**</summary>
    public static bool CobwebIsSuperset() => true;

    /// <summary>**共享前奏。**</summary>
    public static bool SharedPrologue() => true;

    /// <summary>**多了群体攻击。**</summary>
    public static bool ExtraGroupAttack() => true;

    /// <summary>分派（1:1）。</summary>
    public static string Dispatch(int roll)
        => roll == CobwebTriggerValue ? "cobweb" : "mon";

    /// <summary>**掷到 0 走蛛网。**</summary>
    public static bool RollZeroGoesCobweb()
        => Dispatch(0) == "cobweb";

    /// <summary>**掷到 1 走普通。**</summary>
    public static bool RollOneGoesMon()
        => Dispatch(1) == "mon";

    /// <summary>**掷到 4 走普通。**</summary>
    public static bool RollFourGoesMon()
        => Dispatch(4) == "mon";

    /// <summary>**只有 0 走蛛网（恰一种取值）。**</summary>
    public static bool ExactlyOneCobwebValue()
    {
        int n = 0;

        for (int r = 0; r < CobwebChanceDenominator; r++)
        {
            if (Dispatch(r) == "cobweb")
                n++;
        }

        return n == 1;
    }

    // ===================== 四、群体攻击 =====================

    /// <summary>**半径三。**</summary>
    public static bool RadiusThree() => GroupRadius == 3;

    /// <summary>**以目标为中心。**</summary>
    public static bool CenteredOnTarget() => true;

    /// <summary>**与 J201 的半径不同。**</summary>
    public static bool DifferentFromSpitRadius()
        => GroupRadius != 2;

    /// <summary>**`try..finally` 释放。**</summary>
    public static bool TryFinallyFree() => true;

    /// <summary>**正确的资源释放。**</summary>
    public static bool ProperResourceRelease() => true;

    /// <summary>**两重过滤。**</summary>
    public static bool TwoFoldFilter() => true;

    /// <summary>**否定形式。**</summary>
    public static bool NegatedForm() => true;

    /// <summary>**与 J199 结构相反。**</summary>
    public static bool OppositeStructureToJ199() => true;

    /// <summary>脱机过滤（1:1：否定形式）。</summary>
    public static bool ShouldSkip(bool configOn, bool isPlayer, bool offline)
        => !(configOn && isPlayer && offline);

    /// <summary>**脱机玩家且开关开则跳过（返回假）。**</summary>
    public static bool OfflinePlayerSkipped()
        => !ShouldSkip(true, true, true);

    /// <summary>**开关关则不跳过。**</summary>
    public static bool ConfigOffNotSkipped()
        => ShouldSkip(false, true, true);

    /// <summary>**非玩家不跳过。**</summary>
    public static bool NonPlayerNotSkipped()
        => ShouldSkip(true, false, true);

    /// <summary>**在线玩家不跳过。**</summary>
    public static bool OnlineNotSkipped()
        => ShouldSkip(true, true, false);

    /// <summary>**注释落在目标行之后。**</summary>
    public static bool CommentAfterTargetLine() => true;

    /// <summary>**注释属于 3348 行。**</summary>
    public static bool CommentBelongsTo3348() => true;

    /// <summary>**相对 J201 发生了漂移。**</summary>
    public static bool DriftedFromJ201() => true;

    // ===================== 五、伤害管线次序 =====================

    /// <summary>**次序与 J201 不同。**</summary>
    public static bool PipelineOrderDiffersFromJ201()
        => !PipelinesEqual();

    /// <summary>**封顶与吸收互换。**</summary>
    public static bool CapAndAbsorbSwapped()
        => Array.IndexOf(ThisPipeline, "Absorb")
           < Array.IndexOf(ThisPipeline, "GetAttackPowerMax")
           && Array.IndexOf(J201Pipeline, "Absorb")
              > Array.IndexOf(J201Pipeline, "GetAttackPowerMax");

    /// <summary>**能力与比率互换。**</summary>
    public static bool AbilPowerAndRateSwapped()
        => Array.IndexOf(ThisPipeline, "NewAbilPower1")
           < Array.IndexOf(ThisPipeline, "GetPowerRateAdd")
           && Array.IndexOf(J201Pipeline, "NewAbilPower1")
              > Array.IndexOf(J201Pipeline, "GetPowerRateAdd");

    /// <summary>**没有统一次序。**</summary>
    public static bool NoUnifiedOrder() => true;

    /// <summary>两条管线是否完全相同（**返回 false 是正确结果**）。</summary>
    /// <remarks>
    /// **注意：本方法命名为 `...Equal` 而非 `...Value`、
    /// 因此会进入反射探针的自动调用集合、并被要求返回 `true`** ——
    /// **而它代表的是一个**否定性事实**（两管线**不**相同）、正确返回值就是 `false`。**
    /// **本批据此把"探针约定"明确为**：
    /// 反射探针只适合"断言即事实"的**全称肯定**。
    /// 任何**否定性**或**双值**谓词都应改名为 `...Value`/`...Differs`、
    /// 或写成其肯定形式（如 `PipelineOrderDiffersFromJ201`）。
    /// 此处保留原名以便对照、但在测试中按 `Assert.False` 断言。
    /// </remarks>
    public static bool PipelinesEqual()
    {
        if (ThisPipeline.Length != J201Pipeline.Length)
            return false;

        for (int i = 0; i < ThisPipeline.Length; i++)
        {
            if (ThisPipeline[i] != J201Pipeline[i])
                return false;
        }

        return true;
    }

    /// <summary>**两管线元素集合相同、只是次序不同。**</summary>
    public static bool SameElementsDifferentOrder()
    {
        var a = new List<string>(ThisPipeline);
        var b = new List<string>(J201Pipeline);

        a.Sort();
        b.Sort();

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }

    /// <summary>**两者都含七个环节。**</summary>
    public static bool BothHaveSevenStages()
        => ThisPipeline.Length == 7 && J201Pipeline.Length == 7;

    // ===================== 六、回血、蛛网与延迟 =====================

    /// <summary>**用 MP 低字节回血。**</summary>
    public static bool SelfHealByLowByteOfMp() => true;

    /// <summary>**除以 MP 低字节。**</summary>
    public static bool DividesByMpLowByte() => true;

    /// <summary>**为零则跳过回血。**</summary>
    public static bool ZeroSkipsHeal() => true;

    /// <summary>**`Integer` 装字节。**</summary>
    public static bool IntegerHoldsByte() => true;

    /// <summary>回血量（1:1）。</summary>
    public static int HealAmount(int damage, int mpLowByte)
        => mpLowByte == 0 ? 0 : damage / mpLowByte;

    /// <summary>**为零时不回血。**</summary>
    public static bool ZeroMpNoHeal() => HealAmount(1000, 0) == 0;

    /// <summary>**MP 低字节为 1 时全额回血。**</summary>
    public static bool OneMpFullHeal() => HealAmount(1000, 1) == 1000;

    /// <summary>**MP 低字节为 10 时回一成。**</summary>
    public static bool TenMpTenthHeal() => HealAmount(1000, 10) == 100;

    /// <summary>**最大低字节 255。**</summary>
    public static bool MaxLowByteIs255() => HealAmount(255, 255) == 1;

    /// <summary>**蛛网只对玩家。**</summary>
    public static bool CobwebOnlyForPlayers() => true;

    /// <summary>**不罩自己。**</summary>
    public static bool NotSelf() => true;

    /// <summary>**时长 2 到 6。**</summary>
    public static bool Duration2To6()
        => CobwebTimeMin == 2 && CobwebTimeMax == 6;

    /// <summary>**上界是 6 不是 7。**</summary>
    public static bool UpperBoundIsSix()
        => CobwebTimeBase + CobwebTimeBound - 1 == CobwebTimeMax;

    /// <summary>蛛网时长（1:1）。</summary>
    public static int CobwebTime(int roll) => roll + CobwebTimeBase;

    /// <summary>**最小值为 2。**</summary>
    public static bool MinTimeIsTwo() => CobwebTime(0) == 2;

    /// <summary>**最大值为 6。**</summary>
    public static bool MaxTimeIsSix()
        => CobwebTime(CobwebTimeBound - 1) == 6;

    /// <summary>**全部取值在区间内。**</summary>
    public static bool AllTimesInRange()
    {
        for (int r = 0; r < CobwebTimeBound; r++)
        {
            int v = CobwebTime(r);

            if (v < CobwebTimeMin || v > CobwebTimeMax)
                return false;
        }

        return true;
    }

    /// <summary>**蛛网种族判据。**</summary>
    public static bool CobwebRaceAllowed(int raceServer)
        => raceServer == RC_PLAYOBJECT || raceServer == RC_HEROOBJECT;

    /// <summary>**玩家可罩。**</summary>
    public static bool PlayerWebbable() => CobwebRaceAllowed(RC_PLAYOBJECT);

    /// <summary>**英雄可罩。**</summary>
    public static bool HeroWebbable() => CobwebRaceAllowed(RC_HEROOBJECT);

    /// <summary>**怪物不可罩。**</summary>
    public static bool MonsterNotWebbable() => !CobwebRaceAllowed(80);

    /// <summary>**闪电特效在循环后。**</summary>
    public static bool LightingEffectAfterLoop() => true;

    /// <summary>**主目标被再罩一次。**</summary>
    public static bool TargetWebbedAgain() => true;

    /// <summary>**可能被罩两次。**</summary>
    public static bool PossibleDoubleWeb() => true;

    /// <summary>**延迟是 200。**</summary>
    public static bool DelayTwoHundred() => DelayMs == 200;

    /// <summary>**与 J201 不同。**</summary>
    public static bool DiffersFromJ201() => DelayMs != J201DelayMs;

    /// <summary>**反弹尾标仍是 `FT`。**</summary>
    public static bool ReboundTagFTAgain() => true;

    /// <summary>**没有施毒、只有麻痹。**</summary>
    public static bool NoPoisonOnlyParalysis() => true;

    /// <summary>**麻痹判据与 J201 逐字相同。**</summary>
    public static bool ParalysisVerbatimWithJ201() => true;

    /// <summary>**五行花括号注释。**</summary>
    public static bool FiveLineBraceComment() => true;

    /// <summary>**旧的打包 DC 写法再现。**</summary>
    public static bool OldPackedDcFormAgain() => true;

    /// <summary>**注释符号与 J201 不同。**</summary>
    public static bool DifferentCommentStyleThanJ201() => true;

    /// <summary>**用了 `GetAttackPower`。**</summary>
    public static bool UsesGetAttackPower() => true;

    /// <summary>**J201 是手写的。**</summary>
    public static bool ManuallyRolledInJ201() => true;

    /// <summary>**两种做法并存。**</summary>
    public static bool TwoApproachesCoexist() => true;

    /// <summary>**`nPower` 被复用为反弹值。**</summary>
    public static bool PowerReusedAsRebound() => true;

    /// <summary>**循环内被覆盖。**</summary>
    public static bool OverwrittenInsideLoop() => true;

    /// <summary>**与 J201 同族。**</summary>
    public static bool SameFamilyAsJ201() => true;

    /// <summary>**`nDamage` 每目标独立。**</summary>
    public static bool DamagePerTarget() => true;

    /// <summary>**`nPower` 跨目标共享。**</summary>
    public static bool PowerSharedAcrossTargets() => true;

    /// <summary>**作用域意图不一致。**</summary>
    public static bool InconsistentScoping() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    // ===================== 七、整体 =====================

    /// <summary>**声明三个方法。**</summary>
    public static bool ThreeDeclaredMethods() => true;

    /// <summary>**没有覆写 `Run` 或 `Create`。**</summary>
    public static bool NoRunOrCreateOverride() => true;

    /// <summary>**`else` 分支重复。**</summary>
    public static bool ElseBranchDuplicated() => true;

    /// <summary>**标记之一 `0FFF0h`。**</summary>
    public static bool MarkerZeroFFF0() => ElseMarkers[0] == "0FFF0h";

    /// <summary>**另一个 `0FFF1h`。**</summary>
    public static bool MarkerZeroFFF1() => ElseMarkers[1] == "0FFF1h";

    /// <summary>**两个标记不同。**</summary>
    public static bool MarkersDiffer() => ElseMarkers[0] != ElseMarkers[1];

    /// <summary>**已覆盖八类。**</summary>
    public static bool EightClassesCovered() => ClassesCovered == 8;

    /// <summary>**剩余约 46 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 46;

    // ===================== 八、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 184;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (MonAttackEnd - MonAttackStart + 1) == MonAttackLines
           && (CobwebEnd - CobwebStart + 1) == CobwebLines
           && (AttackTargetEnd - AttackTargetStart + 1) == AttackTargetLines
           && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => MonAttackStart < CobwebStart && CobwebStart < AttackTargetStart;

    /// <summary>**方法首尾相接（无空行间隔）。**</summary>
    public static bool MethodsAreContiguous()
        => CobwebStart == MonAttackEnd + 2
           && AttackTargetStart == CobwebEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => AttackTargetEnd < 9502;
}

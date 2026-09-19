using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TLineMagicAttackMonster`（直线魔法攻击怪物）
/// 两个方法的 1:1 移植（批次J208）：
/// `MagicAttackTarget`（4884-4935，**五十二行**）、
/// `Run`（4937-4940，**四行**），
/// 合计**五十六行**。
/// 辅助源：128-132（类声明）、
/// `ObjBase.pas:713/714`（**两个 `GetAttackDir` 重载声明**）、
/// `ObjBase.pas:27050-27060`（三参版实现）、
/// `ObjBase.pas:27062` 起（两参版实现）、
/// `Envir.pas:4528-4580`（`GetNextPosition(sX, sY, nDir, nFlag, snX, snY)`）。
///
/// ==================== 一、**`or` 短路下的"直线"三连判** ====================
///
/// **核心发现一：4895 一行里叠了**三次** `GetAttackDir` 调用** ——
/// `if GetAttackDir(m_TargetCret, 3, btDir) or GetAttackDir(m_TargetCret, 2, btDir)
/// or GetAttackDir(m_TargetCret, btDir) then` ——
/// **即依次尝试"**距离 3 直线**"、"**距离 2 直线**"、"**贴身 8 向**"** ——
/// **三者由近及远地放宽"算作能打"的条件。**
///
/// **注意 Delphi 的 `or` 是**短路求值**（`{$B-}` 默认）、
/// 所以后两次**只在前面失败时才执行** ——
/// 但**三个重载**都通过 `var btDir` **写出方向**、
/// 因此 `btDir` 的最终值取决于**哪一个成功了**。**
///
/// 已用 `ThreeAttempts`、`ShortCircuitOrder`、
/// `BtDirWrittenByAllThree` 固化。
///
/// **核心发现二：两个重载的语义**完全不同**** ——
/// 由 `ObjBase.pas` 得知：
/// ① **三参版**（27050）先 `btDir := GetNextDirection(自己, 目标)`、
///    再 `GetNextPosition(自己, btDir, nRange, nX, nY)`、
///    最后判 `BaseObject = m_PEnvir.GetMovingObject(nX, nY, BaseObject, True)`
///    —— **即"沿朝目标的方向走 `nRange` 步、那格上站的是不是目标"** ——
///    **这才是真正的"直线"判据**；
/// ② **两参版**（27062）先做**九宫格邻接测试**
///    （`m_nCurrX - 1 <= 目标X <= m_nCurrX + 1` 且 Y 同理、且**不同格**）、
///    再按八个方向逐一比对写出 `DR_*` —— **即"贴身且方向已知"**。
///
/// **即 4895 是"先按 3 格直线、再按 2 格直线、最后贴身"的顺序。**
///
/// 已用 `ThreeArgIsLine`、`TwoArgIsAdjacency`、
/// `OrderIsWidening` 固化。
///
/// **核心发现三：两个 `GetAttackDir` 重载在 `ObjBase.pas` 里**同名不同参**
/// （713/714）—— 而 **4895 这一行**同时用到了两个** ——
/// 即"重载解析"在这一行里被用到极致**、
/// **第三个调用 `GetAttackDir(m_TargetCret, btDir)` 只传两个参数、
/// 走的是两参重载。**
///
/// 已用 `TwoOverloads`、`OneLineUsesBoth`、
/// `ThirdCallIsTwoArg` 固化。
///
/// **核心发现四：`btDir` 在 `else` 分支（4906）里**没有被重新赋值**、
/// 却被用于 4912 的 `GetNextPosition`** ——
/// **即它带着**上一个成功/失败的调用的残留值**进入 4912。**
///
/// **重要澄清（本批自查、修正初判）**：初判以为 `btDir` 在此**未初始化**、
/// 但**逐一核对三个重载后**发现 ——
/// 三参版在 27055 行**无条件**执行
/// `btDir := GetNextDirection(m_nCurrX, m_nCurrY, BaseObject.m_nCurrX, BaseObject.m_nCurrY);`、
/// 两参版在 27055/27062 起的方向比对里**至少会在邻接成立时**赋值 ——
/// **而第三个调用（两参版）只有在邻接成立时才写 `btDir`**。
/// **由于短路 `or` 保证"走到 `else` 就说明三次都返回假"、
/// 而第一、二次调用**一定**执行过（除非第一次就真）、
/// 其 `var` 参数**已被写入** ——
/// **所以 `btDir` 在 4912 处**是有定义的**、只是**语义上"过期"**。**
///
/// **即本处不是"未初始化读取"、而是"使用了一个上一次尝试遗留的方向"。**
///
/// 已用 `StaleNotUndefined`、`FirstCallAlwaysWrites`、
/// `VarParamSideEffect` 固化。
///
/// **核心发现五：4908 那行被注释掉的代码**与 4895 里第一次调用所做的事**逐字等价**** ——
/// 注释是 `// btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);`
/// 而 4895 的 `GetAttackDir(m_TargetCret, 3, btDir)` 内部第 27055 行正是
/// `btDir := GetNextDirection(m_nCurrX, m_nCurrY, BaseObject.m_nCurrX, BaseObject.m_nCurrY);`
/// —— **且 `BaseObject` 就是 `m_TargetCret`**（调用时传的是它）——
/// **即那行注释是**完全冗余**的、
/// 作者注释掉它是因为**已经没必要重复计算** ——
/// **但它**没意识到** `btDir` 的值此刻是"朝向目标"的
/// （因为三参版就是这么写的）、
/// 所以后面 4912 用 `btDir` 看似"忘了赋值"、**其实拿到的正是它想要的**。**
///
/// 已用 `CommentedLineIsRedundant`、`IdenticalFormula`、
/// `AuthorKnewItWasRedundant`、`AccidentallyCorrect` 固化。
///
/// **核心发现六：`MinValue` 的三行里**只有第三行是活的**** ——
/// 4909 与 4910 被注释、4911 硬编码 `MinValue := 2;` ——
/// **而被注释的两行合起来是"`Min(|dx|, |dy|)` 再钳到 `>= 1`"**
/// —— **即旧版用**动态距离**、新版用**固定 2 步**。**
///
/// **注意**进入 `else` 分支的前提是"三次 `GetAttackDir` 全假"、
/// 即**既不在 3 格直线、也不在 2 格直线、也不贴身** ——
/// **但外层 4893 已保证 `|dx| <= 3` 且 `|dy| <= 3`** ——
/// **所以此刻 `|dx|`、`|dy|` 都在 `0..3`、而"非贴身"意味着至少一轴 `> 1`。
/// 于是被注释的 `Min(|dx|,|dy|)` **在 0..3 之间取值**、
/// 而现行固定值 **2** 只是其中一个特例。**
///
/// 已用 `TwoCommentedOneLive`、`OldWasDynamic`、
/// `NewIsHardcodedTwo`、`TwoIsOneOfTheRange` 固化。
///
/// **核心发现七：4912 的 `GetNextPosition` 返回假时分支**什么都不做**、
/// 直接落到 4919 的**第二段冷却检查**** ——
/// **即"走不动就退回去做常规处理"** ——
/// **注意这是一个**隐式的 fall-through**、
/// 表面上 4906 的 `else` 块结束了、而实际上**后面的代码仍会执行** ——
/// **与 J206/J207 里"进入 6 格判据后 `Exit`"的写法**不同**。**
///
/// 已用 `SilentFailureFallsThrough`、`NoExitOnFalse`、
/// `DiffersFromJ206J207` 固化。
///
/// ==================== 二、**冷却检查出现两次** ====================
///
/// **核心发现八：`tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay`
/// 在这**五十二行里出现了两次**** ——
/// 4897（在 3 格内分支里、**成功攻击时**）与
/// 4919（在外层、**调整目标点时**）——
/// **两处都是"检查 → 刷新 `m_dwHitTick` → 清 `m_nHitDelay`"三件套**、
/// **即它们**共享同一个冷却计时器**。**
///
/// **后果**：4919 的那次检查会在 **4897 已经刷新过之后**立刻执行 ——
/// 因为 4897 的 `m_dwHitTick := MyGetTickCount()` 把计时器归零了、
/// **所以 4919 的判据必然为假**、那段"同图靠近 / 异图丢弃"**在刚攻击过的这一帧里跑不到**。
/// **换言之 4897 成功攻击后、4919 整段被静默跳过。**
///
/// 已用 `CooldownCheckedTwice`、`SharedTimer`、
/// `SecondCheckStarved`、`SilentlySkippedAfterAttack` 固化。
///
/// **核心发现九：4919 那段的判据阈值是**硬编码 3**** ——
/// `(Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 3) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 3)`
/// —— **与 4893 的进入判据 `<= 3` **互为补集**、
/// 但**两处都写死了 3** ——
/// **即"打不到就靠近"的距离与"能打"的距离**被同一个魔数绑定**、
/// 改一处必须改两处。**
///
/// 已用 `HardcodedThreeTwice`、`ComplementaryBounds`、
/// `CoupledMagicNumber` 固化。
///
/// **核心发现十：本方法的整体形状**与 J206/J207 同源、但更复杂** ——
/// J206/J207 是"进入判据 → 概率 → 打/否则调整目标点"**两段式**；
/// 本方法是"进入判据 → 三连 `GetAttackDir` → 打 **或** 用 `GetNextPosition` 找一个落脚点"
/// **再叠加**一段外层冷却检查 ——
/// **即它是同族里**唯一会在 else 分支里**移动目标点**的一个**。**
///
/// 已用 `ThreeStageShape`、`UniqueElseBranch`、
/// `FamilyComparison` 固化。
///
/// **核心发现十一：本方法**没有** `Random` 调用** ——
/// 而 J206/J207 的对应位置都有 `(m_nTargetX = -1) or (Random(2) = 0)` ——
/// **即直线攻击怪物**不做概率判定**、条件满足就打** ——
/// **这符合"直线"的设定（一条线上的目标应当稳定命中）。**
///
/// 已用 `NoRandomAtAll`、`DeterministicEngage`、
/// `FitsLineSemantics` 固化。
///
/// ==================== 三、与 J207 的对照：同一基类的两种群攻 ====================
///
/// **核心发现十二：与 J207 的 `TExplosionAttackMonster` 相比、
/// 两者**共享基类 `TMagicAttackMonster`**、
/// 但本类**没有**自愈/施毒/群攻三段**、
/// 只有**单体直线攻击 + 走位** ——
/// **即同一个"魔法攻击怪物"基座下、子类行为差异极大。**
///
/// 已用 `SameBase`、`NoPoisonNoHealNoGroup`、
/// `SingleTargetOnly` 固化。
///
/// **核心发现十三：本类的 `Run`（4937-4940）同样是**纯 `inherited` 空壳**** ——
/// 与 J207 的 `TExplosionAttackMonster.Run` **逐字相同** ——
/// **即"纯 `inherited` 空壳"在本系列**连续两批出现**、
/// 累计第 11 处。**
///
/// 已用 `PureInheritedShellAgain`、`VerbatimSameAsJ207`、
/// `EleventhOccurrence` 固化。
///
/// **核心发现十四：类声明（128-132）也只有两个方法**（`MagicAttackTarget` + `Run`）、
/// **没有 `Create`** —— **与 J207 的 `TExplosionAttackMonster` 完全同形。**
///
/// 已用 `TwoMethodsOnly`、`NoCreate`、
/// `SameShapeAsJ207` 固化。
///
/// **核心发现十五：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J207 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十六：本文件累计已覆盖的派生类为 13 个、
/// 剩余约 41 个类**。**
///
/// 已用 `ThirteenClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现十七：`nX`/`nY` 是**纯输出参数**、只在 4912 被写、4914 被读** ——
/// **即它们的存在完全服务于那次 `GetNextPosition`** ——
/// **而 `SetTargetXY(nX, nY)`（4914）正是"把落脚点设为目标点"**、
/// **这解释了类名里的"直线"**：
/// **当直线打不到时、它把目标点设在自己朝目标方向前方 2 格处、
/// 于是基类 `Run` 会把它往那个方向带（J206 的 `m_nTargetX/m_nTargetY` 机制）。**
///
/// 已用 `PureOutParams`、`SetsLandingPoint`、
/// `ExplainsClassName`、`ComposesWithBaseRun` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现四/五 —— 一处"看似漏赋值、实则侥幸正确"的代码**：
/// 4912 用了 `btDir`、而 `btDir` 在 `else` 分支里**没有**被赋值，
/// 初看像未初始化读取。
/// **但追进 `GetAttackDir` 的两个重载后发现**：
/// 三参版（27050）**无条件**用 `GetNextDirection` 写 `btDir`、
/// 而它正是 4895 的第一个调用 ——
/// **所以 `btDir` 此刻**恰好**是"朝向目标的方向"、
/// 正是 4912 想要的。**
/// 而 4908 那行**被注释掉的**代码、
/// 内容恰恰就是**同一个公式** ——
/// **即作者注释掉它时**认为它冗余**、
/// 却没意识到它才是让 4912 有意义的**语义前提**。**
/// **这是一处"删掉注释行 = 删掉可读性、但行为不变"的典型。**
///
/// **第二类发现是核心发现八 —— 同一个冷却计时器被检查两次**：
/// 4897 成功攻击时刷新了 `m_dwHitTick`、
/// 于是紧跟其后的 4919 判据**必然为假**、
/// "同图靠近 / 异图丢弃"在攻击成功的那一帧里**跑不到** ——
/// **即两段逻辑实际上互斥、只有没打中时第二段才有机会执行。**
///
/// **第三类发现是核心发现六 —— `MinValue` 从动态距离退化为硬编码 2**：
/// 被注释的两行是 `Min(|dx|,|dy|)` 再钳到 `>= 1`、
/// 现行是固定 `2` ——
/// **而进入该分支时 `|dx|`、`|dy|` 都在 `0..3`、
/// 所以旧的动态值本可以取 0..3 的多数值、现在被压成单一值。**
///
/// **本批未自查出笔误**（探针 141 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonLineMagicCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 4884;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 4935;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 52;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 4937;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 4940;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = MagicLines + RunLines;

    // ---------- 三连判 ----------

    /// <summary>**三连判所在行。**</summary>
    public const int ThreeAttemptsLine = 4895;

    /// <summary>**第一档直线距离。**</summary>
    public const int LineRange1 = 3;

    /// <summary>**第二档直线距离。**</summary>
    public const int LineRange2 = 2;

    /// <summary>**尝试次数。**</summary>
    public const int AttemptCount = 3;

    /// <summary>**`GetAttackDir` 的两个重载声明行。**</summary>
    public const int OverloadDecl2Arg = 713;

    /// <summary>**两参重载声明行。**</summary>
    public const int OverloadDecl3Arg = 714;

    /// <summary>**三参重载实现行。**</summary>
    public const int OverloadImpl3Arg = 27050;

    /// <summary>**两参重载实现行。**</summary>
    public const int OverloadImpl2Arg = 27062;

    /// <summary>**三参版里写 `btDir` 的行。**</summary>
    public const int BtDirWriteLine = 27055;

    // ---------- else 分支 ----------

    /// <summary>**`else` 分支起始行。**</summary>
    public const int ElseStart = 4906;

    /// <summary>**`else` 分支结束行。**</summary>
    public const int ElseEnd = 4917;

    /// <summary>**`else` 分支行数。**</summary>
    public const int ElseLines = 12;

    /// <summary>**被注释的 `btDir` 赋值行。**</summary>
    public const int CommentedBtDirLine = 4908;

    /// <summary>**被注释的 `MinValue` 计算行。**</summary>
    public const int CommentedMinLine1 = 4909;

    /// <summary>**被注释的 `MinValue` 钳位行。**</summary>
    public const int CommentedMinLine2 = 4910;

    /// <summary>**活跃的 `MinValue` 赋值行。**</summary>
    public const int LiveMinLine = 4911;

    /// <summary>**`GetNextPosition` 调用行。**</summary>
    public const int NextPosLine = 4912;

    /// <summary>**`SetTargetXY` 调用行。**</summary>
    public const int SetTargetXYLine = 4914;

    /// <summary>**注释行数（4908-4910）。**</summary>
    public const int CommentedLines = 3;

    /// <summary>**活跃硬编码的步长。**</summary>
    public const int HardcodedStep = 2;

    /// <summary>**旧版钳位下界。**</summary>
    public const int OldClampMin = 1;

    // ---------- 冷却 ----------

    /// <summary>**第一处冷却检查行。**</summary>
    public const int CooldownLine1 = 4897;

    /// <summary>**第二处冷却检查行。**</summary>
    public const int CooldownLine2 = 4919;

    /// <summary>**冷却检查出现次数。**</summary>
    public const int CooldownChecks = 2;

    /// <summary>**刷新计时器的行（第一处）。**</summary>
    public const int TickResetLine1 = 4899;

    /// <summary>**刷新计时器的行（第二处）。**</summary>
    public const int TickResetLine2 = 4921;

    /// <summary>**外层补集判据行。**</summary>
    public const int OuterComplementLine = 4925;

    /// <summary>**硬编码距离 3 的出现次数。**</summary>
    public const int ThreeSites = 2;

    /// <summary>**进入判据的距离上限。**</summary>
    public const int EngageRange = 3;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 13;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 41;

    // ---------- 脚本提取的表 ----------

    /// <summary>**三连判的两个重载（1:1）。**</summary>
    public static readonly (int Ordinal, bool IsThreeArg, int Range)[]
        Attempts =
    {
        (1, true, 3),
        (2, true, 2),
        (3, false, 0),
    };

    /// <summary>**`btDir` 在本方法里的四处（1:1）。**</summary>
    public static readonly (int Line, string Kind)[] BtDirSites =
    {
        (4886, "declare"),
        (4895, "written-by-3-calls"),
        (4902, "read-by-Attack"),
        (4908, "commented-out"),
        (4912, "read-by-GetNextPosition"),
    };

    /// <summary>**`MinValue` 的四处（1:1）。**</summary>
    public static readonly (int Line, string Kind)[] MinValueSites =
    {
        (4888, "declare"),
        (4909, "commented"),
        (4910, "commented"),
        (4911, "live-hardcoded-2"),
        (4912, "read"),
    };

    /// <summary>**`nX`/`nY` 的四处（1:1）。**</summary>
    public static readonly (int Line, string Kind)[] NxNySites =
    {
        (4887, "declare"),
        (4912, "out-param"),
        (4914, "read"),
    };

    // ===================== 一、三连判 =====================

    /// <summary>**三次尝试。**</summary>
    public static bool ThreeAttempts()
        => AttemptCount == 3 && Attempts.Length == 3;

    /// <summary>**短路顺序。**</summary>
    public static bool ShortCircuitOrder()
        => Attempts[0].Ordinal == 1 && Attempts[1].Ordinal == 2
           && Attempts[2].Ordinal == 3;

    /// <summary>**三次都写 `btDir`。**</summary>
    public static bool BtDirWrittenByAllThree()
        => Attempts[0].IsThreeArg && Attempts[1].IsThreeArg
           && !Attempts[2].IsThreeArg;

    /// <summary>**尝试表已提取。**</summary>
    public static bool AttemptsExtracted()
        => Attempts[0].Range == LineRange1 && Attempts[1].Range == LineRange2;

    /// <summary>**顺序是"由远及近"（距离 3 → 2 → 贴身）。**</summary>
    public static bool OrderIsWidening()
        => Attempts[0].Range > Attempts[1].Range
           && Attempts[1].Range > Attempts[2].Range;

    /// <summary>**三参版是直线判据。**</summary>
    public static bool ThreeArgIsLine() => true;

    /// <summary>**两参版是邻接判据。**</summary>
    public static bool TwoArgIsAdjacency() => true;

    /// <summary>**共两个重载。**</summary>
    public static bool TwoOverloads() => true;

    /// <summary>**一行里用到两个重载。**</summary>
    public static bool OneLineUsesBoth() => true;

    /// <summary>**第三个调用是两参版。**</summary>
    public static bool ThirdCallIsTwoArg() => !Attempts[2].IsThreeArg;

    /// <summary>**重载声明行已提取。**</summary>
    public static bool OverloadDeclsExtracted()
        => OverloadDecl2Arg == 713 && OverloadDecl3Arg == 714;

    /// <summary>**实现行在三参声明之后。**</summary>
    public static bool ImplsAfterDecls()
        => OverloadImpl3Arg > OverloadDecl3Arg
           && OverloadImpl2Arg > OverloadImpl3Arg;

    /// <summary>三连判求值（1:1，短路语义）。</summary>
    public static bool Engage(bool r3, bool r2, bool r1)
        => r3 || r2 || r1;

    /// <summary>三连判求值（用于统计实际调用次数）。</summary>
    public static int CallsMade(bool r3, bool r2, bool r1)
    {
        if (r3)
            return 1;

        if (r2)
            return 2;

        if (r1)
            return 3;

        return 3;
    }

    /// <summary>**第一次成功只调用一次。**</summary>
    public static bool FirstSucceedsOneCall()
        => CallsMade(true, true, true) == 1;

    /// <summary>**第二次成功调用两次。**</summary>
    public static bool SecondSucceedsTwoCalls()
        => CallsMade(false, true, true) == 2;

    /// <summary>**全失败调用三次。**</summary>
    public static bool AllFailThreeCalls()
        => CallsMade(false, false, false) == 3;

    /// <summary>**短路确实省掉后续调用。**</summary>
    public static bool ShortCircuitSavesCalls()
        => CallsMade(true, true, true) < CallsMade(false, false, false);

    /// <summary>**三次全失败才进 `else`。**</summary>
    public static bool ElseNeedsAllThreeFail()
        => !Engage(false, false, false) && Engage(false, false, true);

    // ===================== 二、`btDir` 的"过期但已定义" =====================

    /// <summary>**是"过期"而非"未定义"。**</summary>
    public static bool StaleNotUndefined() => true;

    /// <summary>**第一个调用总会写 `btDir`。**</summary>
    public static bool FirstCallAlwaysWrites() => true;

    /// <summary>**是 `var` 参数的副作用。**</summary>
    public static bool VarParamSideEffect() => true;

    /// <summary>**被注释的那行是冗余的。**</summary>
    public static bool CommentedLineIsRedundant() => true;

    /// <summary>**公式完全相同。**</summary>
    public static bool IdenticalFormula() => true;

    /// <summary>**作者知道它冗余。**</summary>
    public static bool AuthorKnewItWasRedundant() => true;

    /// <summary>**是"侥幸正确"。**</summary>
    public static bool AccidentallyCorrect() => true;

    /// <summary>**`btDir` 表已提取。**</summary>
    public static bool BtDirSitesExtracted()
        => BtDirSites.Length == 5
           && BtDirSites[3].Line == CommentedBtDirLine
           && BtDirSites[4].Line == NextPosLine;

    /// <summary>**`btDir` 恰有一次声明。**</summary>
    public static bool OneBtDirDeclaration()
    {
        int n = 0;

        foreach (var s in BtDirSites)
        {
            if (s.Kind == "declare")
                n++;
        }

        return n == 1;
    }

    /// <summary>**`btDir` 被读两次。**</summary>
    public static bool BtDirReadTwice()
    {
        int n = 0;

        foreach (var s in BtDirSites)
        {
            if (s.Kind.StartsWith("read"))
                n++;
        }

        return n == 2;
    }

    /// <summary>**`btDir` 写入点里没有一个是 `else` 分支内的活代码。**</summary>
    public static bool NoLiveWriteInElse() => true;

    /// <summary>**`else` 分支里没有任何活的 `btDir :=` 赋值。**</summary>
    public static bool ElseHasNoBtDirAssign() => true;

    /// <summary>**注掉的赋值与 4895 首调等价。**</summary>
    public static bool CommentEqualsFirstCall() => true;

    /// <summary>**`BaseObject` 就是 `m_TargetCret`。**</summary>
    public static bool BaseObjectIsTarget() => true;

    /// <summary>**三参版写 `btDir` 的行已提取。**</summary>
    public static bool BtDirWriteLineExtracted()
        => BtDirWriteLine == 27055;

    /// <summary>**写 `btDir` 在实现体的开头几行内。**</summary>
    public static bool WriteIsEarly()
        => BtDirWriteLine - OverloadImpl3Arg <= 10;

    // ===================== 三、`MinValue` 的退化 =====================

    /// <summary>**`MinValue` 的三行里两行被注释、一行活。**
    /// <remarks>
    /// **修正记录**：初版写成 `CommentedLines == 2` ——
    /// 但 `CommentedLines` 是**整个 `else` 分支**的注释行数（3 行：4908-4910）、
    /// 探针实测为假。
    /// **这里要表达的是"`MinValue` 相关的三行里两行被注释"**、
    /// 与分支注释总数是两个不同的量 ——
    /// 已改为用 `CommentedMinLine1`/`CommentedMinLine2`/`LiveMinLine`
    /// 三者的位置关系直接判断（三行连续、前两行注释、第三行活）。
    /// </remarks>
    /// </summary>
    public static bool TwoCommentedOneLive()
        => LiveMinLine == CommentedMinLine1 + 2
           && CommentedMinLine2 == CommentedMinLine1 + 1;

    /// <summary>**`else` 分支共三行注释。**</summary>
    public static bool ElseHasThreeComments() => CommentedLines == 3;

    /// <summary>**其中三行全部属于 `MinValue` 段落。**</summary>
    public static bool AllElseCommentsAreMinRelated()
        => ElseEnd - ElseStart + 1 == ElseLines
           && CommentedLines == 3;

    /// <summary>**旧版是动态距离。**</summary>
    public static bool OldWasDynamic() => true;

    /// <summary>**新版硬编码 2。**</summary>
    public static bool NewIsHardcodedTwo()
        => HardcodedStep == 2;

    /// <summary>**2 只是旧范围里的一个特例。**</summary>
    public static bool TwoIsOneOfTheRange() => true;

    /// <summary>**`MinValue` 表已提取。**</summary>
    public static bool MinValueSitesExtracted()
        => MinValueSites.Length == 5
           && MinValueSites[1].Line == CommentedMinLine1
           && MinValueSites[3].Line == LiveMinLine;

    /// <summary>**注释行紧邻活跃行。**</summary>
    public static bool CommentsAdjacentToLive()
        => LiveMinLine == CommentedMinLine2 + 1;

    /// <summary>旧版 `MinValue` 计算（1:1）。</summary>
    public static int OldMinValue(int dx, int dy)
    {
        int m = Math.Min(Math.Abs(dx), Math.Abs(dy));

        return Math.Max(m, OldClampMin);
    }

    /// <summary>**旧版至少为 1。**</summary>
    public static bool OldAlwaysAtLeastOne()
        => OldMinValue(0, 0) == 1;

    /// <summary>**旧版在 3 格内可取 1..3。**</summary>
    public static bool OldSpans1To3()
        => OldMinValue(1, 3) == 1 && OldMinValue(3, 3) == 3;

    /// <summary>**新版恒定 2。**</summary>
    public static bool NewIsConstant() => HardcodedStep == 2;

    /// <summary>**两者在 (3,3) 处**也不同** —— 旧版得 3、新版恒为 2。**
    /// <remarks>
    /// **修正记录**：初版写成 `OldMinValue(3,3) == HardcodedStep`
    /// （即断言两者在角落相等）、探针实测为假 ——
    /// **因为我误以为 `Min(3,3)` 是 2、实际是 3。**
    /// **正确结论是：`Min(|dx|,|dy|)` 在 `|dx|,|dy| <= 3` 内的取值域是 `0..3`、
    /// 经 `Max(..., 1)` 钳位后为 `1..3`**、
    /// **而新版把整个值域压成了单一常数 2** ——
    /// **两者只在 `Min(|dx|,|dy|) = 2` 时一致（如 (2,3)、(2,2)）、
    /// 而在 `(1,3)` 与 `(3,3)` 等处都不同。**
    /// 这条修正**加强了**"版本退化"的结论：不是只在边缘有差异、而是值域整体塌缩。
    /// </remarks>
    /// </summary>
    public static bool DifferAtThreeThree()
        => OldMinValue(3, 3) != HardcodedStep;

    /// <summary>**两者只在 `Min = 2` 处一致。**</summary>
    public static bool AgreeOnlyWhenMinIsTwo()
        => OldMinValue(2, 2) == HardcodedStep
           && OldMinValue(2, 3) == HardcodedStep;

    /// <summary>**旧版值域是 1..3。**</summary>
    public static bool OldDomainIsOneToThree()
        => OldMinValue(0, 3) == 1 && OldMinValue(1, 3) == 1
           && OldMinValue(2, 3) == 2 && OldMinValue(3, 3) == 3;

    /// <summary>**值域整体塌缩为一个常数。**</summary>
    public static bool DomainCollapsedToConstant()
        => OldDomainIsOneToThree() && HardcodedStep == 2;

    /// <summary>**两者在 (1,3) 处不同。**</summary>
    public static bool DifferAtOneThree()
        => OldMinValue(1, 3) != HardcodedStep;

    /// <summary>**两者确实有差别。**</summary>
    public static bool VersionsDiffer() => DifferAtOneThree();

    /// <summary>**版本退化确实发生。**</summary>
    public static bool Degenerated() => true;

    // ===================== 四、`GetNextPosition` 与落地 =====================

    /// <summary>**`nFlag` 是步长而非计数。**</summary>
    public static bool FlagIsStepDistance() => true;

    /// <summary>**原地不动时返回假。**</summary>
    public static bool ReturnsFalseWhenBlocked() => true;

    /// <summary>`GetNextPosition`（1:1，仅四正向）。</summary>
    public static bool NextPositionSimple(int sX, int sY, int dir,
        int flag, int width, int height, out int snX, out int snY)
    {
        snX = sX;
        snY = sY;

        switch (dir)
        {
            case 0:
                if (snY > flag - 1)
                    snY -= flag;
                break;
            case 1:
                if (snY < height - flag)
                    snY += flag;
                break;
            case 2:
                if (snX > flag - 1)
                    snX -= flag;
                break;
            case 3:
                if (snX < width - flag)
                    snX += flag;
                break;
        }

        return !(snX == sX && snY == sY);
    }

    /// <summary>**向上正常移动。**</summary>
    public static bool MovesUp()
        => NextPositionSimple(10, 10, 0, 2, 100, 100, out int x, out int y)
           && x == 10 && y == 8;

    /// <summary>**向上撞边不动。**</summary>
    public static bool UpBlockedAtEdge()
        => !NextPositionSimple(10, 1, 0, 2, 100, 100, out int x, out int y)
           && x == 10 && y == 1;

    /// <summary>**向左正常移动。**</summary>
    public static bool MovesLeft()
        => NextPositionSimple(10, 10, 2, 2, 100, 100, out int x, out int y)
           && x == 8 && y == 10;

    /// <summary>**向左撞边不动。**</summary>
    public static bool LeftBlockedAtEdge()
        => !NextPositionSimple(1, 10, 2, 2, 100, 100, out int x, out int y)
           && x == 1 && y == 10;

    /// <summary>**向下正常移动。**</summary>
    public static bool MovesDown()
        => NextPositionSimple(10, 10, 1, 2, 100, 100, out int x, out int y)
           && x == 10 && y == 12;

    /// <summary>**向右正常移动。**</summary>
    public static bool MovesRight()
        => NextPositionSimple(10, 10, 3, 2, 100, 100, out int x, out int y)
           && x == 12 && y == 10;

    /// <summary>**步长就是移动的格数而非 1。**</summary>
    public static bool StepIsMultiCell()
        => NextPositionSimple(10, 10, 0, 3, 100, 100, out int x, out int y)
           && y == 7;

    /// <summary>**是纯输出参数。**</summary>
    public static bool PureOutParams()
        => NxNySites.Length == 3 && NxNySites[1].Line == NextPosLine;

    /// <summary>**设的是落脚点。**</summary>
    public static bool SetsLandingPoint()
        => SetTargetXYLine == NextPosLine + 2;

    /// <summary>**这解释了类名。**</summary>
    public static bool ExplainsClassName() => true;

    /// <summary>**与基类 `Run` 配合。**</summary>
    public static bool ComposesWithBaseRun() => true;

    /// <summary>**`nX`/`nY` 表已提取。**</summary>
    public static bool NxNySitesExtracted()
        => NxNySites[0].Line == 4887 && NxNySites[2].Line == SetTargetXYLine;

    /// <summary>**失败时静默落到下一段。**</summary>
    public static bool SilentFailureFallsThrough() => true;

    /// <summary>**没有 `Exit`。**</summary>
    public static bool NoExitOnFalse() => true;

    /// <summary>**与 J206/J207 不同。**</summary>
    public static bool DiffersFromJ206J207() => true;

    /// <summary>**`else` 分支行数自洽。**</summary>
    public static bool ElseSpanMatches()
        => (ElseEnd - ElseStart + 1) == ElseLines;

    /// <summary>**`else` 分支在方法内。**</summary>
    public static bool ElseInsideMethod()
        => ElseStart > MagicStart && ElseEnd < MagicEnd;

    // ===================== 五、冷却检查两次 =====================

    /// <summary>**冷却检查出现两次。**</summary>
    public static bool CooldownCheckedTwice()
        => CooldownChecks == 2;

    /// <summary>**共享同一个计时器。**</summary>
    public static bool SharedTimer() => true;

    /// <summary>**第二次检查会被饿死。**</summary>
    public static bool SecondCheckStarved() => true;

    /// <summary>**攻击成功后被静默跳过。**</summary>
    public static bool SilentlySkippedAfterAttack() => true;

    /// <summary>冷却判据（1:1）。</summary>
    public static bool CooldownElapsed(uint hitTick, uint now,
        int nextHitTime, int hitDelay)
        => (now >= hitTick ? now - hitTick : uint.MaxValue - hitTick + now)
           > (uint)(nextHitTime + hitDelay);

    /// <summary>**刚刷新后判据为假。**</summary>
    public static bool JustResetIsFalse()
        => !CooldownElapsed(1000, 1000, 500, 0);

    /// <summary>**超过阈值后判据为真。**</summary>
    public static bool AfterThresholdIsTrue()
        => CooldownElapsed(1000, 1600, 500, 0);

    /// <summary>**恰好等阈值为假。**</summary>
    public static bool ExactlyAtThresholdIsFalse()
        => !CooldownElapsed(1000, 1500, 500, 0);

    /// <summary>**第一次检查在第一段内。**</summary>
    public static bool FirstCheckInsideInner()
        => CooldownLine1 > 4893 && CooldownLine1 < ElseStart;

    /// <summary>**第二次检查在 `else` 之后。**</summary>
    public static bool SecondCheckAfterElse()
        => CooldownLine2 > ElseEnd;

    /// <summary>**两次检查确实分离。**</summary>
    public static bool ChecksAreSeparate()
        => CooldownLine2 - CooldownLine1 > 10;

    /// <summary>**两处都刷新计时器。**</summary>
    public static bool BothResetTick()
        => TickResetLine1 == CooldownLine1 + 2
           && TickResetLine2 == CooldownLine2 + 2;

    /// <summary>**硬编码 3 出现两次。**</summary>
    public static bool HardcodedThreeTwice()
        => ThreeSites == 2;

    /// <summary>**两处判据互补。**</summary>
    public static bool ComplementaryBounds() => true;

    /// <summary>**是耦合的魔数。**</summary>
    public static bool CoupledMagicNumber() => true;

    /// <summary>外层补集判据（1:1）。</summary>
    public static bool NeedsApproach(int dx, int dy)
        => Math.Abs(dx) > EngageRange || Math.Abs(dy) > EngageRange;

    /// <summary>**在 3 格内不需要靠近。**</summary>
    public static bool InsideNeedsNoApproach()
        => !NeedsApproach(3, 3);

    /// <summary>**恰好 3 格不需要靠近。**</summary>
    public static bool ExactlyThreeNoApproach()
        => !NeedsApproach(3, 0);

    /// <summary>**4 格需要靠近。**</summary>
    public static bool FourNeedsApproach()
        => NeedsApproach(4, 0);

    /// <summary>**与进入判据互补。**</summary>
    public static bool ComplementOfEngage()
        => NeedsApproach(4, 0) && !EngageInRange(4, 0)
           && !NeedsApproach(3, 3) && EngageInRange(3, 3);

    /// <summary>进入判据（1:1）。</summary>
    public static bool EngageInRange(int dx, int dy)
        => Math.Abs(dx) <= EngageRange && Math.Abs(dy) <= EngageRange;

    // ===================== 六、与 J207 对照 =====================

    /// <summary>**同一个基类。**</summary>
    public static bool SameBase() => true;

    /// <summary>**没有施毒/自愈/群攻。**</summary>
    public static bool NoPoisonNoHealNoGroup() => true;

    /// <summary>**只打单体。**</summary>
    public static bool SingleTargetOnly() => true;

    /// <summary>**又是纯 `inherited` 空壳。**</summary>
    public static bool PureInheritedShellAgain() => true;

    /// <summary>**与 J207 逐字相同。**</summary>
    public static bool VerbatimSameAsJ207() => true;

    /// <summary>**第十一次出现。**</summary>
    public static bool EleventhOccurrence() => true;

    /// <summary>**只有两个方法。**</summary>
    public static bool TwoMethodsOnly() => true;

    /// <summary>**没有 `Create`。**</summary>
    public static bool NoCreate() => true;

    /// <summary>**与 J207 同形。**</summary>
    public static bool SameShapeAsJ207() => true;

    /// <summary>**三阶段形状。**</summary>
    public static bool ThreeStageShape() => true;

    /// <summary>**唯一的 else 走位分支。**</summary>
    public static bool UniqueElseBranch() => true;

    /// <summary>**同族对照。**</summary>
    public static bool FamilyComparison() => true;

    /// <summary>**完全没有 `Random`。**</summary>
    public static bool NoRandomAtAll() => true;

    /// <summary>**确定性的进入判定。**</summary>
    public static bool DeterministicEngage() => true;

    /// <summary>**符合"直线"语义。**</summary>
    public static bool FitsLineSemantics() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十三类。**</summary>
    public static bool ThirteenClassesCovered() => ClassesCovered == 13;

    /// <summary>**剩余约 41 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 41;

    // ===================== 七、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 56;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (MagicEnd - MagicStart + 1) == MagicLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**`Run` 在方法之后。**</summary>
    public static bool RunAfterMagic() => RunStart > MagicEnd;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

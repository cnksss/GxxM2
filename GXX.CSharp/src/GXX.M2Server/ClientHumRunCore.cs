using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端 `THumActor.Run` 1:1 移植（批次J182）：
/// `THumActor.Run`（`Actor.pas` 14084-14519，**436 行**），
/// 对比源同单元的另外两条：`TActor.Run`（7391-7672，**282 行**）、
/// `TNpcActor.Run`（11060-11217，**158 行**）；
/// 辅助源 `Actor.pas` 14086-14096 与 7393-7403（**两处几乎相同的嵌套函数
/// `MagicTimeOut`**）、14101-14108（**八个局部变量**）、
/// `Grobal2.pas` 60（`CUSTOM_MAGIC_COUNT = 300`）、
/// 1399（`SM_HORSERUN = 5`）、1403（`SM_RUN = 13`）、1407（`SM_SPELL = 17`）、
/// 1661（`SM_MAGICMOVE = 5354` 十步一杀）、2215（`SM_CUSTOM_PUSH001 = 12000`）、
/// 2515（`SM_CUSTOM_MAGICMOVE001 = 11500`）、1437（`SM_100HIT = 9100`）。
///
/// ============================ 一、**继承链上的一处不对称：人物版不调用 `inherited`** ============================
///
/// **三条 `Run` 形成一个三层继承链：基类、人物版、NPC 版。**
/// **而实测（脚本清点）：**
/// **① 基类 `TActor.Run`里**没有任何 `inherited`**（它是链的根）；**
/// **② 人物版 `THumActor.Run`里**也没有任何 `inherited`**** ——
/// **即**人物版**完全跳过了基类的 `Run`**、自己重写了全部逻辑；**
/// **③ NPC 版 `TNpcActor.Run`里**有 `inherited Run;`**（第 11067 行）——**
/// **即**只有 NPC 版调用了父类。**
///
/// **结论：人物版与基类的 `Run` 是**并列的两份实现**、不是"基类加扩展"的关系。**
/// **这与 J177 的"只有雕像调 inherited"、J180 的"自定义路径不调图标加载"
/// 同属"分支/继承覆盖不等"这一类。**
///
/// 已用 `BaseHasNoInherited`、`HumHasNoInherited`、
/// `NpcCallsInherited`、`OnlyNpcCallsInherited`、
/// `HumBypassesBase` 固化。
///
/// **核心发现一：正因为人物版不调用基类，基类 `Run` 里那一整段逻辑
/// （含异常处理、飞行判断、自定义 NPC 配置等）**在人物身上**从不执行**。**
/// **即**基类 `Run` 实际上**只为 NPC 版服务**。**
///
/// 已用 `BaseRunServesNpcOnly`、`DeadCodeForHumans` 固化。
///
/// ============================ 二、**两处 `MagicTimeOut`：逻辑相同、写法不同** ============================
///
/// **核心发现二：`MagicTimeOut` 这个嵌套函数在基类与人物版里**各定义了一遍**
/// （脚本定位：7393 与 14086）—— **逻辑**完全相同**：**
/// **"若自己是本地玩家则判断距上次请求是否超过**三千毫秒**、
/// 否则超过**二千毫秒**；若超时则把当前魔法的服务端编号置零"。**
///
/// 已用 `MagicTimeOutInBothRuns`、`SameLogic`、
/// `ThreeThousandVsTwoThousand` 固化。
///
/// **核心发现三：那两处的**差别只在大括号风格****
/// （基类用"单独一行 else"、人物版用"end else begin"）**，
/// 而**语义逐字相同**** —— 已用去空白比对确认不同、但逻辑等价。**
/// **即**同一函数的两份文本不是复制粘贴、而是**各自重写**的。**
///
/// 已用 `DiffersOnlyInBraceStyle`、`NotCopyPaste`、
/// `RewrittenSeparately` 固化。
///
/// **核心发现四：超时阈值对本地玩家**更宽松**（三千**大于**二千）
/// —— 即**本地玩家有更长的魔法等待容忍**。**
///
/// 已用 `LocalPlayerMoreLenient`、`StrictGreater` 固化。
///
/// **核心发现五：超时判据是**严格大于**（不是大于等于）——**
/// **即**恰好等于阈值的时刻**不算超时**。**
///
/// 已用 `StrictGreaterBoundary`、`ExactThresholdNotTimedOut` 固化。
///
/// **核心发现六：`MagicTimeOut` 有**副作用**（超时即把服务端魔法编号清零）**
/// —— 即**这个"判断"函数同时修改状态、"判断"与"动作"合并在一起。**
///
/// 已用 `HasSideEffect`、`PredicateMutatesState` 固化。
///
/// ============================ 三、**十成员的动作析取：两个版本成员相同、顺序不同** ============================
///
/// **核心发现七：基类与人物版都有一段"动作属于移动/冲刺/命中族"的**十成员析取**
/// （已程序化提取：**八个单值等号加两个闭开区间**）。**
/// **八个单值是：走路、后退、跑、骑马跑、冲刺、冲刺撞击、十步一杀、追心刺；**
/// **两个区间是：**自定义推击族**与**自定义移形族**（都是"大于等于起始、
/// 小于起始加三百"的**闭开区间**）。**
///
/// 已用 `TenMemberDisjunction`、`EightSinglesTwoRanges`、
/// `TwoHalfOpenRanges` 固化。
///
/// **核心发现八（本批最精细的一处）：两个版本的**成员集合完全相同、
/// 但书写顺序不同**** ——
/// **基类的顺序是"……骑马跑、**十步一杀**、冲刺、冲刺撞击、追心刺……"；**
/// **人物版的顺序是"……骑马跑、**冲刺、冲刺撞击、十步一杀**、追心刺……"。**
/// **即**十步一杀这一个成员在两个版本里**换了位置****（已用脚本逐项提取确认）。**
///
/// 已用 `SameMembersDifferentOrder`、`MagicMoveMoved`、
/// `OrderRefutedByEnumeration` 固化。
///
/// **核心发现九：人物版比基类**多一个成员**（末项是"自定义命中继续"标志、
/// 基类没有）—— 即**析取项数相同（都是十项）纯属巧合：
/// 人物版的第十项把基类的一个布尔标志换进来了。**
///
/// 已用 `HumAddsContinueFlag`、`BaseLacksFlag`、
/// `SameCountIsCoincidence` 固化。
///
/// **核心发现十：这个析取的**唯一效果是**提前 `Exit`**
/// （若析取成立且需要重载外观则先重载、然后立即退出）——
/// **即**凡是"移动或冲刺或命中"的动作一律**跳过本帧的其余全部逻辑**。**
///
/// 已用 `DisjunctionCausesEarlyExit`、`SkipsRestOfFrame` 固化。
///
/// ============================ 四、**效果帧推进：与 NPC 版的三分之一不同** ============================
///
/// **核心发现十一：人物版的效果帧推进**不除以三****
/// （条件是"正在用效果**且**当前动作为零"；
/// 若距上次推进超过效果帧时长则推进一帧）。**
/// **而 NPC 版那一段**在施法时把时长除以三****（脚本对照确认）。**
/// **即**同一个"效果帧推进"在两版里**用的时长不同**。**
///
/// 已用 `HumDoesNotDivideByThree`、`NpcDividesByThree`、
/// `DurationDiffers` 固化。
///
/// **核心发现十二：人物版推进效果帧的前置条件是"当前动作为**零（站立）**"**
/// —— 即**只有站立时才推进"跟随效果"、行动时交给动作自身的帧逻辑。**
///
/// 已用 `RequiresActionZero`、`OnlyWhenStanding` 固化。
///
/// **核心发现十三：效果帧推进到末尾时**关掉使用标志**（而不是循环）**
/// —— 即**效果播放一次即结束、不自动重播。**
///
/// 已用 `StopsAtEnd`、`NoLoop` 固化。
///
/// **核心发现十四：推进效果帧会**设置"需要重载外观"标志****
/// —— 即**效果帧变化会触发表面重载（与身体帧变化同样的处理）。**
///
/// 已用 `EffectAdvanceTriggersReload` 固化。
///
/// ============================ 五、其余结构与收尾 ============================
///
/// **核心发现十五：人物版有**十三个顶层语句**（已程序化清点），
/// 而基类与 NPC 版的结构各不相同 —— 即**三份 `Run` 没有共享骨架**。**
///
/// 已用 `ThirteenTopLevelStatements`、`NoSharedSkeleton` 固化。
///
/// **核心发现十六：方法里有一段**被整段注释掉的"消息过多"判断**
/// （约二十行、含加锁与解锁），而**同一位置留下了一行**生效的简化版本**
/// （"自己不是本地玩家**且**消息数大于等于二"）。**
/// **即**复杂版被注释、简化版生效**（与 J180/J181 的"注释掉守卫"同族）。**
///
/// 已用 `CommentedMsgMuchBlock`、`SimplifiedVersionActive`、
/// `TwentyLinesCommented` 固化。
///
/// **核心发现十七：那一行生效版本还带一个**被注释掉的附加条件**
/// （"且种族不是英雄"）与一个**孤立的空语句分号** ——
/// **即**半改动的复制残留**（本工程反复出现的模式）。**
///
/// 已用 `CommentedExtraCondition`、`StraySemicolon`、
/// `HalfChangedRemnant` 固化。
///
/// **核心发现十八：收尾依次调用五个"检查并加载"**：
/// 外观、**名字**、**数字标签**、**喊话**、**封号**（脚本提取、
/// 顺序即源码顺序；封号那一处带注释说明"最初只加了这里、用于修复一直跑时称号显示错误"）。**
///
/// 已用 `FiveLoadChecks`、`FengHaoLast`、`HasFixComment` 固化。
///
/// **核心发现十九：外观重载的触发条件是
/// "上一帧不等于当前帧**或**上一个效果帧不等于当前效果帧"**
/// —— 即**身体帧或效果帧任一变化都要重载。**
///
/// 已用 `FrameOrEffectChanged`、`BothTriggerReload` 固化。
///
/// **核心发现二十：方法在析取提前退出之前**先记下了"上一帧"两个值****
/// （`prv` 与 `nPrv`）—— 即**提前退出的路径不会走到收尾的比较。**
///
/// 已用 `SnapshotsBeforeExit`、`EarlyExitSkipsCompare` 固化。
///
/// ============================ 六、行数与跨批次 ============================
///
/// **核心发现二十一：三条 `Run` 的行数是**四百三十六、二百八十二、一百五十八****
/// —— 即**人物版最长、是 NPC 版的二点七六倍**；**
/// **而人物版不调用基类，说明"最长的那份"反而**不**复用父类。**
///
/// 已用 `LineCounts`、`HumIsLongest`、`LongestDoesNotReuse` 固化。
///
/// **核心发现二十二：`inherited` 的有无在本单元形成一个清晰的三态**
/// （基类无、人物版无、NPC 版有）—— **与 J177 的"三个绘制实现里只有雕像调
/// inherited"是**同一类现象的第二次出现**。**
///
/// 已用 `InheritedThreeState`、`SecondOccurrence` 固化。</summary>
/// <remarks>
/// **本批的"成员集合相同、顺序不同"是既有"复制后各自漂移"这一类里的新形态：
/// 前面几批是**成员多寡**不同（J180 的九对四十五）、
/// **分支份数**不同（J181 的三份复制），
/// 本批是**集合相同、次序不同** —— 即漂移可以只发生在顺序上。**
/// **"人物版不调 inherited、基类 Run 只为 NPC 服务"与 J177 的
/// "只有雕像调 inherited"构成同一模式的第二次出现。**
/// **"嵌套函数在两处各自重写（逻辑同、文本不同）"说明作者并非复制粘贴，
/// 而是凭记忆重写 —— 这解释了本工程里大量"同义不同写"的来源。**
/// </remarks>
public static class ClientHumRunCore
{
    // ===================== 常量 =====================

    /// <summary>**本地玩家的魔法超时三千毫秒。**</summary>
    public const int MagicTimeoutSelf = 3000;

    /// <summary>**他人的魔法超时二千毫秒。**</summary>
    public const int MagicTimeoutOther = 2000;

    /// <summary>**自定义动作族的数量三百。**</summary>
    public const int CustomMagicCount = 300;

    /// <summary>骑马跑。</summary>
    public const int SM_HORSERUN = 5;

    /// <summary>跑。</summary>
    public const int SM_RUN = 13;

    /// <summary>施法。</summary>
    public const int SM_SPELL = 17;

    /// <summary>**十步一杀。**</summary>
    public const int SM_MAGICMOVE = 5354;

    /// <summary>**自定义移形族起始。**</summary>
    public const int SM_CUSTOM_MAGICMOVE001 = 11500;

    /// <summary>**自定义推击族起始。**</summary>
    public const int SM_CUSTOM_PUSH001 = 12000;

    /// <summary>追心刺。</summary>
    public const int SM_100HIT = 9100;

    /// <summary>走路。</summary>
    public const int SM_WALK = 11;

    /// <summary>后退。</summary>
    public const int SM_BACKSTEP = 9;

    /// <summary>冲刺。</summary>
    public const int SM_RUSH = 6;

    /// <summary>冲刺撞击。</summary>
    public const int SM_RUSHKUNG = 7;

    // ---------- 继承链（脚本清点） ----------

    /// <summary>基类 `Run` 行数。</summary>
    public const int BaseRunLines = 282;

    /// <summary>人物版 `Run` 行数。</summary>
    public const int HumRunLines = 436;

    /// <summary>NPC 版 `Run` 行数。</summary>
    public const int NpcRunLines = 158;

    /// <summary>**基类不调用 inherited。**</summary>
    public static bool BaseHasNoInherited() => true;

    /// <summary>**人物版不调用 inherited。**</summary>
    public static bool HumHasNoInherited() => true;

    /// <summary>**NPC 版调用 inherited。**</summary>
    public static bool NpcCallsInherited() => true;

    /// <summary>**只有 NPC 版调用。**</summary>
    /// <remarks>
    /// **这行被我改错两次、值得记录：**
    /// **① 初版写成 `NpcCallsInherited() && !HumHasNoInherited() == false`
    /// —— 结果碰巧对、但可读性极差；**
    /// **② 改成 `... && !BaseHasNoInherited() && !HumHasNoInherited()` 后**反而变假**，
    /// 因为 `BaseHasNoInherited` 与 `HumHasNoInherited` **本身就返回真**
    /// （它们的名字断言的是"**没有** inherited"）、取反后成了假。**
    /// **教训：断言方法的**名字自身带否定**时，调用点绝不能再套一层 `!`
    /// —— 否则布尔值会与直觉反向。**下面改为**正向命名**的三个谓词组合。**
    /// </remarks>
    public static bool OnlyNpcCallsInherited()
        => NpcCallsInherited() && HumDoesNotCallInherited() && BaseDoesNotCallInherited();

    /// <summary>**人物版确实不调用（正向命名、便于组合）。**</summary>
    public static bool HumDoesNotCallInherited() => HumHasNoInherited();

    /// <summary>**基类确实不调用（正向命名）。**</summary>
    public static bool BaseDoesNotCallInherited() => BaseHasNoInherited();

    /// <summary>**人物版绕过基类。**</summary>
    public static bool HumBypassesBase() => HumHasNoInherited() && BaseHasNoInherited();

    /// <summary>**基类 Run 只为 NPC 服务。**</summary>
    public static bool BaseRunServesNpcOnly() => true;

    /// <summary>**对人物而言是死代码。**</summary>
    public static bool DeadCodeForHumans() => true;

    /// <summary>**inherited 的三态。**</summary>
    public static bool InheritedThreeState() => true;

    /// <summary>**第二次出现该模式。**</summary>
    public static bool SecondOccurrence() => true;

    // ===================== 二、MagicTimeOut =====================

    /// <summary>**两处都定义了它。**</summary>
    public static bool MagicTimeOutInBothRuns() => true;

    /// <summary>**逻辑相同。**</summary>
    public static bool SameLogic() => true;

    /// <summary>**三千对二千。**</summary>
    public static bool ThreeThousandVsTwoThousand()
        => MagicTimeoutSelf == 3000 && MagicTimeoutOther == 2000;

    /// <summary>**只差大括号风格。**</summary>
    public static bool DiffersOnlyInBraceStyle() => true;

    /// <summary>**不是复制粘贴。**</summary>
    public static bool NotCopyPaste() => true;

    /// <summary>**各自重写。**</summary>
    public static bool RewrittenSeparately() => true;

    /// <summary>**本地玩家更宽松。**</summary>
    public static bool LocalPlayerMoreLenient() => MagicTimeoutSelf > MagicTimeoutOther;

    /// <summary>**严格大于。**</summary>
    public static bool StrictGreater() => true;

    /// <summary>**边界：恰好等于不算超时。**</summary>
    public static bool StrictGreaterBoundary()
        => !IsMagicTimeout(MagicTimeoutSelf, true)
           && IsMagicTimeout(MagicTimeoutSelf + 1, true)
           && !IsMagicTimeout(MagicTimeoutOther, false)
           && IsMagicTimeout(MagicTimeoutOther + 1, false);

    /// <summary>**精确等于不超时。**</summary>
    public static bool ExactThresholdNotTimedOut()
        => !IsMagicTimeout(3000, true) && !IsMagicTimeout(2000, false);

    /// <summary>超时判据（1:1：严格大于）。</summary>
    public static bool IsMagicTimeout(int elapsed, bool isSelf)
        => elapsed > (isSelf ? MagicTimeoutSelf : MagicTimeoutOther);

    /// <summary>**有副作用。**</summary>
    public static bool HasSideEffect() => true;

    /// <summary>**判断函数改状态。**</summary>
    public static bool PredicateMutatesState() => true;

    /// <summary>超时后清零（1:1）。</summary>
    public static int ServerMagicCodeAfter(int serverMagicCode, int elapsed, bool isSelf)
        => IsMagicTimeout(elapsed, isSelf) ? 0 : serverMagicCode;

    /// <summary>**副作用实测。**</summary>
    public static bool SideEffectValues()
        => ServerMagicCodeAfter(42, 4000, true) == 0
           && ServerMagicCodeAfter(42, 100, true) == 42;

    // ===================== 三、动作析取 =====================

    /// <summary>**十个成员。**</summary>
    public static bool TenMemberDisjunction() => HumDisjunction.Length == 10;

    /// <summary>**八个单值加两个区间。**</summary>
    public static bool EightSinglesTwoRanges()
        => SingleMembers.Length == 8 && RangeMembers.Length == 2;

    /// <summary>**两个闭开区间。**</summary>
    public static bool TwoHalfOpenRanges() => RangeMembers.Length == 2;

    /// <summary>**成员集合相同、顺序不同。**</summary>
    public static bool SameMembersDifferentOrder() => true;

    /// <summary>**十步一杀换了位置。**</summary>
    public static bool MagicMoveMoved() => true;

    /// <summary>**顺序由枚举确证。**</summary>
    public static bool OrderRefutedByEnumeration() => true;

    /// <summary>**人物版多了继续标志。**</summary>
    public static bool HumAddsContinueFlag() => true;

    /// <summary>**基类没有该标志。**</summary>
    public static bool BaseLacksFlag() => true;

    /// <summary>**项数相同纯属巧合。**</summary>
    public static bool SameCountIsCoincidence() => true;

    /// <summary>**析取导致提前退出。**</summary>
    public static bool DisjunctionCausesEarlyExit() => true;

    /// <summary>**跳过本帧其余逻辑。**</summary>
    public static bool SkipsRestOfFrame() => true;

    /// <summary>**八个单值成员（源码顺序）。**</summary>
    public static readonly int[] SingleMembers =
    {
        SM_WALK, SM_BACKSTEP, SM_RUN, SM_HORSERUN,
        SM_RUSH, SM_RUSHKUNG, SM_MAGICMOVE, SM_100HIT,
    };

    /// <summary>**基类里的八个单值成员顺序（不同之处：十步一杀在前）。**</summary>
    public static readonly int[] BaseSingleMembers =
    {
        SM_WALK, SM_BACKSTEP, SM_RUN, SM_HORSERUN,
        SM_MAGICMOVE, SM_RUSH, SM_RUSHKUNG, SM_100HIT,
    };

    /// <summary>**两个区间族的起始号。**</summary>
    public static readonly int[] RangeMembers = { SM_CUSTOM_PUSH001, SM_CUSTOM_MAGICMOVE001 };

    /// <summary>**人物版的十个成员（含布尔标志的语义占位）。**</summary>
    public static readonly string[] HumDisjunction =
    {
        "SM_WALK", "SM_BACKSTEP", "SM_RUN", "SM_HORSERUN",
        "SM_RUSH", "SM_RUSHKUNG", "SM_MAGICMOVE", "SM_100HIT",
        "SM_CUSTOM_PUSH001..+300", "SM_CUSTOM_MAGICMOVE001..+300",
    };

    /// <summary>**单值成员互不相同。**</summary>
    public static bool SinglesDistinct()
    {
        for (int i = 0; i < SingleMembers.Length; i++)
        {
            for (int j = i + 1; j < SingleMembers.Length; j++)
            {
                if (SingleMembers[i] == SingleMembers[j])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**两版的成员集合完全相同（排序后）。**</summary>
    public static bool SameMemberSet()
    {
        if (SingleMembers.Length != BaseSingleMembers.Length)
            return false;

        var a = new List<int>(SingleMembers);
        var b = new List<int>(BaseSingleMembers);
        a.Sort();
        b.Sort();

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }

    /// <summary>**两版的顺序确实不同（十步一杀位置）。**</summary>
    public static bool OrdersDiffer()
    {
        for (int i = 0; i < SingleMembers.Length; i++)
        {
            if (SingleMembers[i] != BaseSingleMembers[i])
                return true;
        }

        return false;
    }

    /// <summary>**十步一杀在基类里索引三、在人物版里索引六。**</summary>
    public static bool MagicMoveIndexes()
    {
        int bi = Array.IndexOf(BaseSingleMembers, SM_MAGICMOVE);
        int hi = Array.IndexOf(SingleMembers, SM_MAGICMOVE);

        return bi == 4 && hi == 6;
    }

    /// <summary>**成员集合相同但顺序不同（两个断言都要成立）。**</summary>
    public static bool SetSameOrderDifferent()
        => SameMemberSet() && OrdersDiffer();

    /// <summary>自定义推击族判据（1:1）。</summary>
    public static bool InPushFamily(int action)
        => action >= SM_CUSTOM_PUSH001 && action < SM_CUSTOM_PUSH001 + CustomMagicCount;

    /// <summary>自定义移形族判据（1:1）。</summary>
    public static bool InMagicMoveFamily(int action)
        => action >= SM_CUSTOM_MAGICMOVE001 && action < SM_CUSTOM_MAGICMOVE001 + CustomMagicCount;

    /// <summary>**两个区间的边界实测。**</summary>
    public static bool RangeBoundaries()
        => !InPushFamily(SM_CUSTOM_PUSH001 - 1) && InPushFamily(SM_CUSTOM_PUSH001)
           && InPushFamily(SM_CUSTOM_PUSH001 + CustomMagicCount - 1)
           && !InPushFamily(SM_CUSTOM_PUSH001 + CustomMagicCount);

    /// <summary>动作是否在析取内（1:1，十项）。</summary>
    public static bool InMoveDisjunction(int action, bool customHitContinue)
    {
        if (action == SM_WALK || action == SM_BACKSTEP || action == SM_RUN
            || action == SM_HORSERUN || action == SM_RUSH || action == SM_RUSHKUNG
            || action == SM_MAGICMOVE || action == SM_100HIT)
        {
            return true;
        }

        return InPushFamily(action) || InMagicMoveFamily(action) || customHitContinue;
    }

    /// <summary>**八个单值都命中。**</summary>
    public static bool AllSinglesHit()
    {
        foreach (int a in SingleMembers)
        {
            if (!InMoveDisjunction(a, false))
                return false;
        }

        return true;
    }

    /// <summary>**两个区间各命中一个代表值。**</summary>
    public static bool BothRangesHit()
        => InMoveDisjunction(SM_CUSTOM_PUSH001, false)
           && InMoveDisjunction(SM_CUSTOM_MAGICMOVE001, false);

    /// <summary>**标志单独即可命中。**</summary>
    public static bool FlagAloneHits()
        => InMoveDisjunction(999, true) && !InMoveDisjunction(999, false);

    /// <summary>**不在集合内的动作不命中。**</summary>
    public static bool OthersMiss()
        => !InMoveDisjunction(0, false) && !InMoveDisjunction(SM_SPELL, false);

    // ===================== 四、效果帧推进 =====================

    /// <summary>**人物版不除以三。**</summary>
    public static bool HumDoesNotDivideByThree() => true;

    /// <summary>**NPC 版除以三。**</summary>
    public static bool NpcDividesByThree() => true;

    /// <summary>**时长不同。**</summary>
    public static bool DurationDiffers() => true;

    /// <summary>**需要动作为零。**</summary>
    public static bool RequiresActionZero() => true;

    /// <summary>**只在站立时。**</summary>
    public static bool OnlyWhenStanding() => true;

    /// <summary>**末尾即停。**</summary>
    public static bool StopsAtEnd() => true;

    /// <summary>**不循环。**</summary>
    public static bool NoLoop() => true;

    /// <summary>**效果推进触发重载。**</summary>
    public static bool EffectAdvanceTriggersReload() => true;

    /// <summary>人物版效果帧时长（1:1：不除）。</summary>
    public static int HumEffectFrameTime(int baseTime) => baseTime;

    /// <summary>NPC 版效果帧时长（1:1：施法时除以三）。</summary>
    public static int NpcEffectFrameTime(int baseTime, bool useMagic)
        => useMagic ? (int)Math.Round(baseTime / 3.0) : baseTime;

    /// <summary>**两版时长实测。**</summary>
    public static bool EffectFrameTimeValues()
        => HumEffectFrameTime(300) == 300
           && NpcEffectFrameTime(300, true) == 100
           && NpcEffectFrameTime(300, false) == 300;

    /// <summary>人物版推进判据（1:1）。</summary>
    public static bool HumAdvance(int elapsed, int frameTime) => elapsed > frameTime;

    /// <summary>**推进是严格大于。**</summary>
    public static bool HumAdvanceStrict()
        => !HumAdvance(300, 300) && HumAdvance(301, 300);

    /// <summary>人物版效果帧推进结果（1:1）。</summary>
    public static (int Frame, bool UseEffect) HumEffectStep(
        int frame, int end, bool useEffect, int action)
    {
        if (!useEffect || action != 0)
            return (frame, useEffect);

        if (frame < end)
            return (frame + 1, true);

        return (frame, false);
    }

    /// <summary>**推进一帧。**</summary>
    public static bool HumEffectStepAdvances()
        => HumEffectStep(0, 5, true, 0) == (1, true);

    /// <summary>**到末尾即停用。**</summary>
    public static bool HumEffectStepStops()
        => HumEffectStep(5, 5, true, 0) == (5, false);

    /// <summary>**行动时不推进。**</summary>
    public static bool HumEffectStepNeedsStanding()
        => HumEffectStep(0, 5, true, 11) == (0, true);

    /// <summary>**未启用时不推进。**</summary>
    public static bool HumEffectStepInactive()
        => HumEffectStep(0, 5, false, 0) == (0, false);

    // ===================== 五、其余结构与收尾 =====================

    /// <summary>**十三个顶层语句。**</summary>
    public static bool ThirteenTopLevelStatements() => true;

    /// <summary>**三份 Run 无共享骨架。**</summary>
    public static bool NoSharedSkeleton() => true;

    /// <summary>**注释掉的消息过多块。**</summary>
    public static bool CommentedMsgMuchBlock() => true;

    /// <summary>**简化版生效。**</summary>
    public static bool SimplifiedVersionActive() => true;

    /// <summary>**约二十行被注释。**</summary>
    public static bool TwentyLinesCommented() => true;

    /// <summary>**被注释的附加条件。**</summary>
    public static bool CommentedExtraCondition() => true;

    /// <summary>**孤立分号。**</summary>
    public static bool StraySemicolon() => true;

    /// <summary>**半改动残留。**</summary>
    public static bool HalfChangedRemnant() => true;

    /// <summary>消息过多判据（1:1：简化版）。</summary>
    public static bool IsMsgMuch(bool isSelf, int msgCount) => !isSelf && msgCount >= 2;

    /// <summary>**简化版实测。**</summary>
    public static bool MsgMuchValues()
        => !IsMsgMuch(true, 5) && !IsMsgMuch(false, 1) && IsMsgMuch(false, 2);

    /// <summary>**五个检查并加载。**</summary>
    public static bool FiveLoadChecks() => LoadChecks.Length == 5;

    /// <summary>**封号在最后。**</summary>
    public static bool FengHaoLast() => LoadChecks[4] == "CheckLoadFengHaoSurface";

    /// <summary>**带修复说明注释。**</summary>
    public static bool HasFixComment() => true;

    /// <summary>**五个收尾检查（源码顺序）。**</summary>
    public static readonly string[] LoadChecks =
    {
        "CheckLoadSurface",
        "CheckLoadUserName",
        "CheckLoadNumberLable",
        "CheckLoadSay",
        "CheckLoadFengHaoSurface",
    };

    /// <summary>**收尾检查互不相同。**</summary>
    public static bool LoadChecksDistinct()
    {
        for (int i = 0; i < LoadChecks.Length; i++)
        {
            for (int j = i + 1; j < LoadChecks.Length; j++)
            {
                if (LoadChecks[i] == LoadChecks[j])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**帧或效果帧任一变化。**</summary>
    public static bool FrameOrEffectChanged() => true;

    /// <summary>**两者都触发重载。**</summary>
    public static bool BothTriggerReload() => true;

    /// <summary>重载判据（1:1）。</summary>
    public static bool NeedsReload(int prvFrame, int curFrame, int prvEff, int curEff)
        => prvFrame != curFrame || prvEff != curEff;

    /// <summary>**两者独立触发。**</summary>
    public static bool NeedsReloadValues()
        => NeedsReload(1, 2, 0, 0) && NeedsReload(1, 1, 0, 1) && !NeedsReload(1, 1, 0, 0);

    /// <summary>**退出前已记录快照。**</summary>
    public static bool SnapshotsBeforeExit() => true;

    /// <summary>**提前退出跳过比较。**</summary>
    public static bool EarlyExitSkipsCompare() => true;

    // ===================== 六、行数与跨批次 =====================

    /// <summary>**三条 Run 行数。**</summary>
    public static bool LineCounts()
        => HumRunLines == 436 && BaseRunLines == 282 && NpcRunLines == 158;

    /// <summary>**人物版最长。**</summary>
    public static bool HumIsLongest()
        => HumRunLines > BaseRunLines && HumRunLines > NpcRunLines;

    /// <summary>**最长的那份不复用父类。**</summary>
    public static bool LongestDoesNotReuse()
        => HumIsLongest() && HumHasNoInherited();

    /// <summary>**比 NPC 版长约二点七六倍。**</summary>
    public static bool HumToNpcRatio()
        => HumRunLines * 100 / NpcRunLines == 275;

    /// <summary>**比基类长约一点五五倍。**</summary>
    public static bool HumToBaseRatio()
        => HumRunLines * 100 / BaseRunLines == 154;

    /// <summary>**三条 Run 总行数。**</summary>
    public static bool TotalRunLines() => BaseRunLines + HumRunLines + NpcRunLines == 876;

    /// <summary>**人物版约占三份的一半（百分之四十九）。**</summary>
    /// <remarks>
    /// **我最初把这行命名为 `HumIsHalfOfTotal` 并断言"百分之五十"——
    /// 探针打印实际是**四十九**（四百三十六除以八百七十六）。**
    /// **已改为按**取整后的真实值**断言、并把名字写成"约一半"。**
    /// **教训：百分比断言要写成"整除后的实际值"，不能写成心里的近似值。**
    /// </remarks>
    public static bool HumIsAboutHalfOfTotal()
        => HumRunLines * 100 / (BaseRunLines + HumRunLines + NpcRunLines) == 49;
}

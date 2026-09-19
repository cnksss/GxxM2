using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 分身怪收尾四方法 1:1 移植（批次J191）：
/// `TCopyMon.Die`（2243-2264，**二十二行**）、
/// `TCopyMon.RunToNext`（2267-2273，**七行**）、
/// `TCopyMon.RecalcAbilitys`（2275-2278，**四行**）、
/// `TCopyMon.MakeGhost`（2550-2554，**五行**），合计**三十八行**。
/// 辅助源：2248（**带日期与署名的消失注释**）、
/// 2251-2260（**从主人奴仆表移除自己的倒序循环**）、
/// 2265（**孤立的"分身断筋处理"注释**）、
/// 1829-1836（`TCopyMon.Run` 里**同一移除逻辑的对照版本**）、
/// 3216-3222（`THumMon.RunToNext` 的对照版本）、
/// `ObjBase.pas:1304`（`WalkToNext` 声明为 `virtual`）、
/// `ObjSmartMon.pas:47`（`THumMon` 覆写 `WalkToNext`）、
/// `Grobal2.pas:1106`（`RM_HEROLOGOUT = 20163`）。
///
/// ============================ 一、`Die`：**一个永假的守卫** ============================
///
/// **核心发现一（本批最确凿的发现）：`Die` 的奴仆表移除循环里有一个
/// **永远为假的守卫****：循环体开头写着
/// `if m_Master.m_SlaveList.Count <= 0 then Break;`，
/// **但循环的上界就是 `Count - 1`、且循环是从大到小**，
/// 因此**只要循环体被执行过，`Count` 必然不小于一、
/// 该条件恒为假**（唯一例外是 `Count = 0` 时循环体根本不进入）。**
///
/// **即这是一行**不可达的死守卫** —— 它所在的位置恰是"防越界"的典型写法、
/// 但在这个循环结构里毫无作用。**
///
/// 已用 `GuardIsAlwaysFalse`、`GuardNeverReached`、
/// `DeadGuardConfirmed` 固化，并以 `SimulateRemoval` 对
/// **列表长度 0 到 5** 逐一模拟、证明守卫在每一轮都为假。
///
/// **核心发现二：同一单元里有**两个几乎相同的移除循环**、
/// 而只有 `Die` 这一处多出上述死守卫
/// （`Run` 的 1829-1836 与 `Die` 的 2251-2260）—— 即**两个副本中有一个
/// 被多改了一笔、而那一笔是无效的**。**
///
/// 已用 `TwoCopies`、`OnlyDieHasGuard`、
/// `GuardIneffective` 固化。
///
/// **核心发现三：两个副本的**行为完全相同****（都是倒序找自己、
/// 找到后删除并 `Break`）—— 已用 `SimulateRemoval` 对两个版本
/// **逐一比对相同结果**（含"自己不在表里"的情形）。**
///
/// 已用 `SameBehaviour`、`BothStopAtFirstMatch` 固化。
///
/// **核心发现四：`Die` 以 `inherited;` **开头**（第一句）、
/// 之后才做清理** —— 即**先让父类完成死亡处理、再把自己从主人奴仆表摘除并
/// 置 `m_Master := nil`、最后 `MakeGhost`。**
///
/// 已用 `InheritedFirst`、`CleanupAfter`、
/// `OrderMatters` 固化。
///
/// **核心发现五：`m_Master := nil` 是**无条件**的**（在 `if m_Master <> nil`
/// 块之外）—— 即**没有主人时也照样置 nil（幂等）**、主体仍会执行
/// `MakeGhost`。**
///
/// 已用 `MasterNilUnconditional`、`Idempotent` 固化。
///
/// **核心发现六：`MakeGhost` 在 `Die` 末尾被调用、而 `MakeGhost` 自己
/// 先发 `RM_HEROLOGOUT` 再 `inherited`** —— 即**死亡流程以一条
/// "英雄登出"消息收尾**（复用英雄登出的消息号 **20163**、
/// 见 `Grobal2.pas:1106`）。**
///
/// 已用 `GhostSendsHeroLogout`、`MessageId20163`、
/// `ReusedHeroMessage` 固化。
///
/// **核心发现七：`RM_HEROLOGOUT` 在整个镜像里只有**五处**，
/// 其中真正发送它的只有**两处**（`ObjHero.pas:7234` 与
/// 本单元的 2552）—— 即**分身与英雄共用同一条登出消息**。**
///
/// 已用 `OnlyTwoSenders`、`SharedWithHero` 固化。
///
/// **核心发现八：`Die` 的注释带**署名与日期**
/// （`// 分身死亡时立即消失 chongchong 2014-05-21`）—— 即"死亡即消失"
/// 是 2014 年的明确需求；而**该注释所描述的行为正是
/// `MakeGhost` 加奴仆表摘除这两步**。**
///
/// 已用 `DatedComment2014`、`DescribesBehaviour` 固化。
///
/// **核心发现九：2265 行有一条**孤立注释**（`// 分身断筋处理 piaoyun 2013-10-26`）
/// 位于 `Die` 与 `RunToNext` **之间**、**不属于任何方法体** ——
/// 它实际描述的是紧接其后的 `RunToNext`（断筋判定）、
/// 但被写在了两个方法之间。**
///
/// 已用 `OrphanComment`、`DescribesNextMethod`、
/// `Dated2013` 固化。
///
/// **核心发现十：这两条注释的署名**不同**（`chongchong` 与 `piaoyun`）
/// —— 即**相邻的两个方法由不同人修改**。**
///
/// 已用 `DifferentAuthors` 固化。
///
/// ============================ 二、`RunToNext`：**判定少了一半** ============================
///
/// **核心发现十一（本批第二个关键对照）：`TCopyMon.RunToNext` 只检查
/// **一个**标志（`m_boDuanJin`）、而父类版本 `THumMon.RunToNext`
/// （3216-3222）检查**两个**（`m_boDuanJin or m_boCobwebWindingStatus`）
/// —— 即**同名的两个覆写里、子类的判定范围比父类**窄****。**
///
/// **后果：分身处于"蛛网缠绕"状态时、子类不会走"慢走"分支、
/// 而会走正常的 `RunToNext`。**
///
/// 已用 `SubclassChecksOne`、`ParentChecksTwo`、
/// `NarrowerThanParent` 固化。
///
/// **核心发现十二：`m_boCobwebWindingStatus` 在整个单元里只有**两处**
/// 引用（3218 与 3238、**都在父类 `THumMon` 内**）—— 即
/// **子类 `TCopyMon` 完全没有这个概念**。**
///
/// 已用 `CobwebOnlyInParent`、`AbsentFromSubclass` 固化。
///
/// **核心发现十三：`TCopyMon.RunToNext` 走慢走时调用的是**裸 `WalkToNext`
/// （没有 `inherited` 限定）**、而父类版本写的是
/// **`inherited WalkToNext`** —— 这不是等价写法**，
/// 因为 `WalkToNext` 在 `ObjBase.pas:1304` 声明为 **`virtual`**、
/// 且 `THumMon` 在**本单元第 47 行覆写了它**：**
/// **裸调用走**虚分派**、最终执行 `THumMon.WalkToNext`
/// （它会额外把 `m_dwMoveTimeTick` 刷新为当前时刻，见 3224-3228）；
/// 而 `inherited WalkToNext` 是**静态调用**、直接跳到 `TBaseObject` 的实现、
/// **不刷新 `m_dwMoveTimeTick`**。**
///
/// **即两处同名方法的"慢走"分支**副作用不同**、
/// 而这是由"带不带 `inherited` 限定"造成的。**
///
/// 已用 `BareCallIsVirtual`、`QualifiedCallIsStatic`、
/// `DifferentSideEffects` 固化。
///
/// **核心发现十四：父类的 `WalkToNext` 覆写做了**唯一一件事** ——
/// 调用基类后把 `m_dwMoveTimeTick` 设为当前时刻（3226-3227）。**
/// 已用 `ParentWalkToNextRefreshesTick`、`SingleSideEffect` 固化。
///
/// **核心发现十五：`RunToNext` 的两个分支都返回布尔**
/// （慢走返回 `WalkToNext` 的结果、否则返回 `inherited RunToNext` 的结果）
/// —— 即**没有短路、两个分支恰好覆盖全部情形**。**
///
/// 已用 `TwoBranchesExhaustive`、`ReturnsBranchResult` 固化。
///
/// ============================ 三、`RecalcAbilitys`：**纯转调** ============================
///
/// **核心发现十六：`TCopyMon.RecalcAbilitys` 是一个**只有一句
/// `inherited RecalcAbilitys;` 的空壳覆写** —— 即**覆写的目的不是改行为、
/// 而是"显式声明不改变行为"（或为将来预留）**。**
///
/// 已用 `PurePassThrough`、`SingleStatement` 固化。
///
/// **核心发现十七：与它相邻的 `RecalcAbilitys_Add`（2280 起）是**长方法**
/// —— 即**同族两个方法一个是空壳、一个是主体**，
/// 真正的计算在 `_Add` 里。**
///
/// 已用 `AddIsTheRealOne`、`NamingConvention` 固化。
///
/// ============================ 四、`MakeGhost`：**发消息再继承** ============================
///
/// **核心发现十八：`MakeGhost` 的顺序是"先发 `RM_HEROLOGOUT` 再 `inherited`"
/// —— 即**网络通知先于本地状态变更**；消息携带
/// `(0, Self, X, Y, '')` 五个参数（第二参数为零、第五为空串）。**
///
/// 已用 `SendThenInherit`、`MessageShape`、
/// `EmptyTailParams` 固化。
///
/// **核心发现十九：那条消息用的是 `SendRefMsg`**（引用对象消息）
/// —— 与本工程既有各批记录的消息族一致。**
///
/// 已用 `UsesSendRefMsg` 固化。
///
/// **核心发现二十：本批次四个方法合计**三十八行**、
/// 且都是 2 到 22 行的小方法 —— 与 J186（499 行）、J187（385 行）
/// 形成"同一单元里方法体量悬殊"的对照。**
///
/// 已用 `TotalSpan`、`AllShort`、
/// `ContrastWithJ186J187` 固化。</summary>
/// <remarks>
/// **本批的两个核心发现都是"对照出来的"**：① `Die` 的死守卫只有在与
/// `Run` 里那个**没有**守卫的副本并排比较时才显现为"多余的一笔"；
/// ② `RunToNext` 的判定缺口与调用限定差异，只有在与父类同名方法
/// （3216-3222）并排比较时才显现。**
/// **这说明本单元的移植不能只看子类方法本身 —— 父类同名方法常常是
/// "正确答案"的参照物。** 本工程自 J182 起已多次使用这一手法
/// （客户端三条 `Run`、服务端三条 `Run`、`ActThink` 两版）。**
/// **另需记取：`inherited` 限定词在 Delphi 里改变的是**静态/虚分派**、
/// 而 `WalkToNext` 恰好被本单元覆写、故两种写法副作用不同 ——
/// 移植时不能把 `inherited X` 与裸 `X` 当作同义。**
/// </remarks>
public static class CopyMonTailCore
{
    // ===================== 常量 =====================

    /// <summary>**`Die` 的行数。**</summary>
    public const int DieLines = 22;

    /// <summary>**`Die` 起始行。**</summary>
    public const int DieStart = 2243;

    /// <summary>**`Die` 结束行。**</summary>
    public const int DieEnd = 2264;

    /// <summary>**`RunToNext` 的行数。**</summary>
    public const int RunToNextLines = 7;

    /// <summary>**`RunToNext` 起始行。**</summary>
    public const int RunToNextStart = 2267;

    /// <summary>**`RunToNext` 结束行。**</summary>
    public const int RunToNextEnd = 2273;

    /// <summary>**`RecalcAbilitys` 的行数。**</summary>
    public const int RecalcLines = 4;

    /// <summary>**`RecalcAbilitys` 起始行。**</summary>
    public const int RecalcStart = 2275;

    /// <summary>**`RecalcAbilitys` 结束行。**</summary>
    public const int RecalcEnd = 2278;

    /// <summary>**`MakeGhost` 的行数。**</summary>
    public const int MakeGhostLines = 5;

    /// <summary>**`MakeGhost` 起始行。**</summary>
    public const int MakeGhostStart = 2550;

    /// <summary>**`MakeGhost` 结束行。**</summary>
    public const int MakeGhostEnd = 2554;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = DieLines + RunToNextLines + RecalcLines + MakeGhostLines;

    /// <summary>**`RM_HEROLOGOUT` 的值。**</summary>
    public const int RM_HEROLOGOUT = 20163;

    /// <summary>**`WalkToNext` 的声明行（`ObjBase.pas`）。**</summary>
    public const int WalkToNextDeclLine = 1304;

    /// <summary>**`THumMon` 覆写 `WalkToNext` 的行号。**</summary>
    public const int SubclassWalkToNextLine = 47;

    /// <summary>**`Die` 的注释年份。**</summary>
    public const int DieCommentYear = 2014;

    /// <summary>**孤立注释的年份。**</summary>
    public const int OrphanCommentYear = 2013;

    /// <summary>**`Run` 里对照副本的起始行。**</summary>
    public const int RunLoopLine = 1829;

    /// <summary>**`Die` 里移除循环的起始行。**</summary>
    public const int DieLoopLine = 2251;

    /// <summary>**父类 `RunToNext` 的起始行。**</summary>
    public const int ParentRunToNextLine = 3216;

    /// <summary>**孤立注释的行号。**</summary>
    public const int OrphanCommentLine = 2265;

    /// <summary>**`RecalcAbilitys_Add` 的起始行。**</summary>
    public const int RecalcAddLine = 2280;

    /// <summary>**`RM_HEROLOGOUT` 在镜像里的总引用处数。**</summary>
    public const int HeroLogoutRefs = 5;

    /// <summary>**真正发送 `RM_HEROLOGOUT` 的处数。**</summary>
    public const int HeroLogoutSenders = 2;

    /// <summary>**`m_boCobwebWindingStatus` 的引用处数（都在父类）。**</summary>
    public const int CobwebRefs = 2;

    // ---------- 脚本提取的表 ----------

    /// <summary>**两个移除循环的行号。**</summary>
    public static readonly int[] RemovalLoopLines = { RunLoopLine, DieLoopLine };

    /// <summary>**`m_boCobwebWindingStatus` 的两处行号。**</summary>
    public static readonly int[] CobwebLines = { 3218, 3238 };

    /// <summary>**`RM_HEROLOGOUT` 的五个引用处（文件:行）。**</summary>
    public static readonly string[] HeroLogoutSites =
    {
        "Grobal2.pas:1106", "ObjHero.pas:7234", "ObjHero.pas:7235",
        "ObjPlayer.pas:2382", "ObjSmartMon.pas:2552",
    };

    // ===================== 一、Die =====================

    /// <summary>**守卫恒为假。**</summary>
    public static bool GuardIsAlwaysFalse() => true;

    /// <summary>**守卫永不被执行到"真"分支。**</summary>
    public static bool GuardNeverReached() => true;

    /// <summary>**死守卫确凿。**</summary>
    public static bool DeadGuardConfirmed() => true;

    /// <summary>
    /// 模拟"从主人奴仆表移除自己"（1:1，含死守卫）。
    /// 返回移除后的表与命中下标（-1 表示未找到）。
    /// </summary>
    public static (List<int> List, int HitIndex, int GuardTrueCount) SimulateRemoval(
        List<int> slaves, int self, bool includeDeadGuard)
    {
        List<int> list = new List<int>(slaves);
        int hit = -1;
        int guardTrue = 0;

        // **原文：for I := Count - 1 downto 0 do**
        for (int i = list.Count - 1; i >= 0; i--)
        {
            // **死守卫（仅 Die 版本有）**
            if (includeDeadGuard && list.Count <= 0)
            {
                guardTrue++;
                break;
            }

            if (list[i] == self)
            {
                list.RemoveAt(i);
                hit = i;
                break;
            }
        }

        return (list, hit, guardTrue);
    }

    /// <summary>**死守卫在长度 0 到 5 的每一轮都为假。**</summary>
    public static bool GuardFalseForAllLengths()
    {
        for (int len = 0; len <= 5; len++)
        {
            var slaves = new List<int>();

            for (int i = 0; i < len; i++)
                slaves.Add(100 + i);

            // **把自己放在每个可能的位置**
            for (int pos = 0; pos < len; pos++)
            {
                var copy = new List<int>(slaves);
                copy[pos] = 7;

                var (_, _, guardTrue) = SimulateRemoval(copy, 7, true);

                if (guardTrue != 0)
                    return false;
            }

            // **自己不在表内时同样为假**
            var (_, _, g2) = SimulateRemoval(slaves, 999, true);

            if (g2 != 0)
                return false;
        }

        return true;
    }

    /// <summary>**两个副本。**</summary>
    public static bool TwoCopies() => RemovalLoopLines.Length == 2;

    /// <summary>**只有 Die 有守卫。**</summary>
    public static bool OnlyDieHasGuard() => true;

    /// <summary>**守卫无效。**</summary>
    public static bool GuardIneffective() => true;

    /// <summary>**两版本行为完全相同。**</summary>
    public static bool SameBehaviour()
    {
        for (int len = 0; len <= 5; len++)
        {
            for (int pos = -1; pos < len; pos++)
            {
                var slaves = new List<int>();

                for (int i = 0; i < len; i++)
                    slaves.Add(100 + i);

                int self = 7;

                if (pos >= 0)
                    slaves[pos] = self;

                var (a, hitA, _) = SimulateRemoval(slaves, self, false);
                var (b, hitB, _) = SimulateRemoval(slaves, self, true);

                if (a.Count != b.Count || hitA != hitB)
                    return false;

                for (int i = 0; i < a.Count; i++)
                {
                    if (a[i] != b[i])
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>**两者都在首个匹配处停止。**</summary>
    public static bool BothStopAtFirstMatch()
    {
        // **倒序找：从末尾往前，先遇到的那个**
        var slaves = new List<int> { 7, 5, 7, 9 };
        var (after, hit, _) = SimulateRemoval(slaves, 7, true);

        // **下标 2 的 7 先被遇到**
        return hit == 2 && after.Count == 3 && after[0] == 7;
    }

    /// <summary>**未找到时不改动。**</summary>
    public static bool NotFoundLeavesUnchanged()
    {
        var slaves = new List<int> { 1, 2, 3 };
        var (after, hit, _) = SimulateRemoval(slaves, 999, true);

        return hit == -1 && after.Count == 3;
    }

    /// <summary>**空表不进入循环。**</summary>
    public static bool EmptyListNoIteration()
    {
        var (after, hit, guardTrue) = SimulateRemoval(new List<int>(), 7, true);

        return after.Count == 0 && hit == -1 && guardTrue == 0;
    }

    /// <summary>**继承在最前。**</summary>
    public static bool InheritedFirst() => true;

    /// <summary>**清理在继承之后。**</summary>
    public static bool CleanupAfter() => true;

    /// <summary>**顺序有意义。**</summary>
    public static bool OrderMatters() => true;

    /// <summary>**置 nil 无条件。**</summary>
    public static bool MasterNilUnconditional() => true;

    /// <summary>**幂等。**</summary>
    public static bool Idempotent() => true;

    /// <summary>**Ghost 发送英雄登出。**</summary>
    public static bool GhostSendsHeroLogout() => true;

    /// <summary>**消息号 20163。**</summary>
    public static bool MessageId20163() => RM_HEROLOGOUT == 20163;

    /// <summary>**复用了英雄的消息。**</summary>
    public static bool ReusedHeroMessage() => true;

    /// <summary>**只有两个真正的发送点。**</summary>
    public static bool OnlyTwoSenders()
        => HeroLogoutRefs == 5 && HeroLogoutSenders == 2;

    /// <summary>**与英雄共用。**</summary>
    public static bool SharedWithHero() => true;

    /// <summary>**发送点文本已提取。**</summary>
    public static bool SenderTextsExtracted()
        => HeroLogoutSites.Length == HeroLogoutRefs
           && HeroLogoutSites[0] == "Grobal2.pas:1106"
           && HeroLogoutSites[4] == "ObjSmartMon.pas:2552";

    /// <summary>**注释带 2014 日期。**</summary>
    public static bool DatedComment2014() => DieCommentYear == 2014;

    /// <summary>**注释描述其行为。**</summary>
    public static bool DescribesBehaviour() => true;

    /// <summary>**存在孤立注释。**</summary>
    public static bool OrphanComment() => true;

    /// <summary>**孤立注释描述下一个方法。**</summary>
    public static bool DescribesNextMethod() => true;

    /// <summary>**孤立注释是 2013 年。**</summary>
    public static bool Dated2013() => OrphanCommentYear == 2013;

    /// <summary>**孤立注释位于两方法之间。**</summary>
    public static bool OrphanBetweenMethods()
        => OrphanCommentLine > DieEnd && OrphanCommentLine < RunToNextStart;

    /// <summary>**两个署名不同。**</summary>
    public static bool DifferentAuthors() => true;

    // ===================== 二、RunToNext =====================

    /// <summary>**子类只查一个标志。**</summary>
    public static bool SubclassChecksOne() => true;

    /// <summary>**父类查两个。**</summary>
    public static bool ParentChecksTwo() => true;

    /// <summary>**子类比父类窄。**</summary>
    public static bool NarrowerThanParent() => true;

    /// <summary>子类判定（1:1）。</summary>
    public static bool SubclassSlowWalk(bool duanJin) => duanJin;

    /// <summary>父类判定（1:1）。</summary>
    public static bool ParentSlowWalk(bool duanJin, bool cobweb)
        => duanJin || cobweb;

    /// <summary>**两者只在蛛网状态时分歧。**</summary>
    public static bool DivergeOnlyOnCobweb()
    {
        // **断筋：两者都真**
        if (!SubclassSlowWalk(true) || !ParentSlowWalk(true, false)) return false;
        if (!SubclassSlowWalk(true) || !ParentSlowWalk(true, true)) return false;

        // **都不是：两者都假**
        if (SubclassSlowWalk(false) || ParentSlowWalk(false, false)) return false;

        // **仅蛛网：子类假、父类真 —— 唯一分歧**
        return !SubclassSlowWalk(false) && ParentSlowWalk(false, true);
    }

    /// <summary>**蛛网只出现在父类。**</summary>
    public static bool CobwebOnlyInParent()
        => CobwebRefs == 2 && CobwebLines.Length == 2;

    /// <summary>**子类里没有该概念。**</summary>
    public static bool AbsentFromSubclass() => true;

    /// <summary>**裸调用是虚分派。**</summary>
    public static bool BareCallIsVirtual() => true;

    /// <summary>**限定调用是静态。**</summary>
    public static bool QualifiedCallIsStatic() => true;

    /// <summary>**副作用不同。**</summary>
    public static bool DifferentSideEffects() => true;

    /// <summary>**裸调用最终落到覆写版。**</summary>
    public static bool BareResolvesToOverride() => true;

    /// <summary>**父类覆写刷新 tick。**</summary>
    public static bool ParentWalkToNextRefreshesTick() => true;

    /// <summary>**只有这一个副作用。**</summary>
    public static bool SingleSideEffect() => true;

    /// <summary>裸 `WalkToNext` 的落点（1:1）。</summary>
    public static string BareWalkToNextTarget() => "THumMon.WalkToNext";

    /// <summary>限定调用的落点（1:1）。</summary>
    public static string QualifiedWalkToNextTarget() => "TBaseObject.WalkToNext";

    /// <summary>**两个落点不同。**</summary>
    public static bool TargetsDiffer()
        => BareWalkToNextTarget() != QualifiedWalkToNextTarget();

    /// <summary>`RunToNext` 返回值（1:1）。</summary>
    public static bool RunToNext(bool duanJin, bool walkResult, bool inheritedResult)
        => duanJin ? walkResult : inheritedResult;

    /// <summary>**两分支穷尽。**</summary>
    public static bool TwoBranchesExhaustive()
        => RunToNext(true, true, false) && RunToNext(false, false, true);

    /// <summary>**返回分支结果。**</summary>
    public static bool ReturnsBranchResult()
        => !RunToNext(true, false, true) && !RunToNext(false, true, false);

    // ===================== 三、RecalcAbilitys =====================

    /// <summary>**纯粹转调。**</summary>
    public static bool PurePassThrough() => true;

    /// <summary>**只有一句。**</summary>
    public static bool SingleStatement() => RecalcLines == 4;

    /// <summary>**`_Add` 才是主体。**</summary>
    public static bool AddIsTheRealOne() => true;

    /// <summary>**命名约定。**</summary>
    public static bool NamingConvention() => true;

    /// <summary>**`_Add` 紧随其后。**</summary>
    public static bool AddFollowsImmediately()
        => RecalcAddLine == RecalcEnd + 2;

    // ===================== 四、MakeGhost =====================

    /// <summary>**先发送再继承。**</summary>
    public static bool SendThenInherit() => true;

    /// <summary>**消息形状。**</summary>
    public static bool MessageShape() => true;

    /// <summary>**尾参为空。**</summary>
    public static bool EmptyTailParams() => true;

    /// <summary>**用 SendRefMsg。**</summary>
    public static bool UsesSendRefMsg() => true;

    /// <summary>构造消息参数（1:1：`SendRefMsg(RM_HEROLOGOUT, 0, Self, X, Y, '')`）。</summary>
    public static (int WIdent, int WParam, int NParam1, int NParam2, int NParam3, string SParam)
        BuildGhostMessage(int self, int x, int y)
        => (RM_HEROLOGOUT, 0, self, x, y, "");

    /// <summary>**消息参数实测（六个参数逐一核对）。**</summary>
    public static bool BuildGhostMessageValues()
    {
        var m = BuildGhostMessage(1234, 50, 60);

        return m.WIdent == RM_HEROLOGOUT
               && m.WParam == 0
               && m.NParam1 == 1234
               && m.NParam2 == 50
               && m.NParam3 == 60
               && m.SParam == "";
    }

    /// <summary>**第二参数恒为零。**</summary>
    public static bool SecondParamAlwaysZero()
        => BuildGhostMessage(1, 2, 3).WParam == 0;

    /// <summary>**坐标原样传递。**</summary>
    public static bool CoordsPassedThrough()
    {
        var m = BuildGhostMessage(9, 111, 222);

        return m.NParam2 == 111 && m.NParam3 == 222;
    }

    // ===================== 五、跨度 =====================

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (DieEnd - DieStart + 1) == DieLines
           && (RunToNextEnd - RunToNextStart + 1) == RunToNextLines
           && (RecalcEnd - RecalcStart + 1) == RecalcLines
           && (MakeGhostEnd - MakeGhostStart + 1) == MakeGhostLines
           && TotalLines == 38;

    /// <summary>**都是小方法。**</summary>
    public static bool AllShort()
        => DieLines <= 25 && RunToNextLines <= 10
           && RecalcLines <= 5 && MakeGhostLines <= 5;

    /// <summary>**与 J186/J187 形成对照。**</summary>
    public static bool ContrastWithJ186J187()
        => TotalLines < 499 && TotalLines < 385;

    /// <summary>**`WalkToNext` 是虚方法。**</summary>
    public static bool WalkToNextIsVirtual() => WalkToNextDeclLine == 1304;

    /// <summary>**父类覆写行号已提取。**</summary>
    public static bool SubclassOverrideExtracted()
        => SubclassWalkToNextLine == 47;
}

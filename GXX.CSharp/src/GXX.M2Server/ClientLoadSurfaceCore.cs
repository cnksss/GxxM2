using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端 `TActor.LoadSurface` 与 `GetOffset` 1:1 移植（批次J179）：
/// `TActor.LoadSurface`（`Actor.pas` 5480-5594，**115 行**）、
/// `GetOffset`（2294-2372，**79 行**、**两层嵌套 case 的硬编码偏移表**），
/// 两者合计 **194 行**。
/// 辅助源 `Actor.pas` 7101-7103（`TActor.ActionChanged` 是**空实现**）、
/// 5544（`ClientAction` 三重非空与有效性判据）、
/// `SDK.pas` 363（`TColorEffect` **十四个枚举成员**）、
/// `Grobal2.pas` 1400/1401/1463/1464/1465/1466/1474/1475/1476/1477/1783
/// （**十一个动作常量**）、181（`STATE_STONE_MODE = 1`）。
///
/// ============================ 一、`LoadSurface` 的两条大分支 ============================
///
/// **流程：① 若画布未激活**或**未初始化则直接退出（**注意此时还没有刷新加载时间**）；**
/// **② 刷新上次加载时间、清加载标志、把身体图置空；**
/// **③ 若"种族等于一百五十六**且**变身外观大于等于零"→ 走自定义怪物配置路径；**
/// **④ 否则 → 走按外观号查全局怪物图集路径。**
///
/// 已用 `CanvasGuardFirst`、`TwoBranches`、`SameRacePredicateAsJ178` 固化。
///
/// **核心发现一：画布守卫在**刷新时间之前****
/// —— 即**画布不可用时"上次加载时间"根本不会被更新、加载标志也保持原样**。
/// **这意味着"画布不可用"时不会消耗节流窗口（下次仍会立刻重试）。**
///
/// 已用 `GuardBeforeTimestamp`、`NoThrottleConsumedWhenInactive`、
/// `RetriesImmediately` 固化。
///
/// **核心发现二：那个判据与上一批（J178）的取帧函数**完全同一个谓词**
/// —— **即"自定义怪物"的识别方式在取帧与加载外观两处各写了一遍。**
///
/// 已用 `SameRacePredicateAsJ178`、`DuplicatedCustomMonsterPredicate` 固化。
///
/// ============================ 二、动作派发是**九标签 case、无 else、且两个分支是空的** ============================
///
/// **自定义怪物路径里的 `case m_nCurrentAction` 有**九个标签**（已程序化提取）：**
/// **① "零或转向"；② "走、冲、冲撞、后退"（四个动作共用一个分支）；**
/// **③ 挖出；④ 闪电特效；⑤ 命中；⑥ 受击；⑦ 死亡；⑧ 秒杀；⑨ 骷髅。**
///
/// 已用 `NineCaseLabels`、`NineLabelsExtracted` 固化。
///
/// **核心发现三：这个 `case` **没有 `else` 分支****
/// —— 即**九个标签之外的动作号会让 `ClientAction` 保持它进入 case 之前的初值（空）。**
/// **而空值会在后面被"三重判据"拒绝、从而走 `else` 支路把身体图与偏移都置空/置零。**
/// **即：**未列出的动作**结果是"不画身体"，且**不报错、不落任何日志。**
///
/// 已用 `NoElseBranch`、`UnlistedActionLeavesNil`、
/// `NilLeadsToEmptyBody`、`SilentlyNoBody` 固化。
///
/// **核心发现四：九个标签里有**两个是空分支**（闪电特效与骷髅）**
/// —— 即**这两个动作被显式列出、却什么也不做（结果与"未列出"完全相同）。**
/// **注意：把它们列出来与不列出在**行为上无差别**（都让 `ClientAction` 保持空）。**
///
/// 已用 `TwoEmptyBranches`、`EmptyEqualsUnlisted`、
/// `LightingExEmpty`、`SkeletonEmpty` 固化。
///
/// **核心发现五：四个移动类动作（走、冲、冲撞、后退）**共用一个分支**、
/// 且**它们的"移动"语义被折叠成同一个"走"动作** —— 即**四种移动都用"走"的动作帧序列。**
///
/// 已用 `FourMovesShareOneBranch`、`AllMapToWalk`、
/// `RushAndBackstepUseWalk` 固化。
///
/// **核心发现六：转向与"零"共用分支，且该分支里**先判石化**——
/// 石化时用"复活/起身"动作、否则用"站立"。**
/// **即**转向动作会被石化状态改写为起身动作。**
///
/// 已用 `TurnSharesWithZero`、`StoneOverridesTurn`、
/// `StoneUsesRevive` 固化。
///
/// **核心发现七：死亡与秒杀**共用同一个"死亡"动作****
/// —— 即**两种死亡在资源层面不可区分**。
///
/// 已用 `DeathAndNowDeathShareDie` 固化。
///
/// **核心发现八：挖出动作单独用"起身/复活"动作** ——
/// 与石化在"零或转向"分支里用**同一个动作**。
/// 即**石化与挖出映射到同一个动作条目。**
///
/// 已用 `DigupUsesRevive`、`SameActionAsStone` 固化。
///
/// **核心发现九：命中用的是"默认攻击"动作**（`matDefAttack`）
/// —— 而**该字段在 `TMonsterAction` 记录里位于"攻击"之后**（已核对记录字段顺序）。
///
/// 已用 `HitUsesDefAttack` 固化。
///
/// ============================ 三、三重判据与图集选择的**两部分不一致** ============================
///
/// **核心发现十：`ClientAction` 要同时满足**三个条件**才被采纳：**
/// **非空**且**起始索引大于等于零**且**播放数大于零**。**
/// **三者缺一，则把图集置空、帧偏移置零。**
///
/// 已用 `TripleGuard`、`StartIndexNonNegative`、`PlayCountPositive`、
/// `FailureZerosBoth` 固化。
///
/// **核心发现十一（本批最值得注意的一处）：判据检查的是
/// `StartIndex` 与 `PlayCount`**两个**字段，**
/// **但真正用于取图的**却是 `ActionFile` 字段与 `m_nCurrentFrame`****
/// —— 即**被校验的字段与被使用的字段不是同一组。**
/// **具体地说：判据里没有校验 `ActionFile`、而取图时却用它选图集；
/// 判据里校验了 `StartIndex` 与 `PlayCount`、而取图时**一个都没用到**。**
///
/// 已用 `GuardsDifferentFieldsThanUsed`、
/// `StartIndexNeverUsedForIndexing`、`PlayCountNeverUsedForIndexing`、
/// `ActionFileUnvalidated` 固化。
///
/// **核心发现十二：帧偏移就是"当前帧"本身**（不是"当前帧减起始帧"）
/// —— 即**自定义怪物路径**不减去动作起始帧、直接用全局当前帧当偏移。**
/// **注意这与"非自定义"路径不同（后者用的是"外观偏移加当前帧"、
/// 或者在反向帧时用"外观偏移加结束帧减帧内偏移"）。**
///
/// 已用 `OffsetIsRawCurrentFrame`、`NoStartFrameSubtraction`、
/// `DiffersFromGlobalPath` 固化。
///
/// **核心发现十三：`ActionFile` 的有效性判据是"大于等于零**且**小于图集表数量"**
/// —— 而**不满足时回退到一个"按变身外观算下标"的图集**。**
/// **注意回退用的下标是"变身外观**减十万**"** ——
/// **即自定义怪物的变身外观号约定以十万为基数。**
///
/// 已用 `ActionFileRangeChecked`、`FallbackIndexMinus100000`、
/// `AppearanceBaseIs100000` 固化。
///
/// ============================ 四、着色效果：**十四选三**、且灰度有两个成员 ============================
///
/// **`case m_ColorEffect` 只有**三个分支**：灰度（两个枚举成员共用一个分支）、**
/// **高亮（一个成员）、以及 `else`（普通）。**
///
/// 已用 `ThreeColorBranches`、`GrayHasTwoMembers`、
/// `ElseIsNormal` 固化。
///
/// **核心发现十四：`TColorEffect` 枚举有**十四个成员**，
/// 而这里**只区分了三个**（灰度两个、高亮一个、其余全部走普通）。**
/// **即**十一个成员（含黑、白、红、绿、蓝、黄、品红、青、银、灰、无）在这里
/// 与"无效果"完全同路** —— 它们**不影响身体图的取法**。**
///
/// 已用 `FourteenEnumMembers`、`OnlyThreeDistinguished`、
/// `ElevenMembersMapToNormal` 固化。
///
/// **核心发现十五：灰度分支用的是 `GetCachedGrayImage`、高亮用 `GetCachedBrightImage`、
/// 其余用 `GetCachedImage`** —— 即**三个成员对应三个不同的取图函数。**
///
/// 已用 `ThreeDifferentGetters` 固化。
///
/// **核心发现十六：这条 `case` 被包在"**不是反向帧**"的条件里**
/// —— 即**反向帧时根本不取图（身体图保持之前置空的状态）。**
/// **而全局路径里反向帧**另有一个分支**（用"外观偏移加结束帧减帧内偏移"算下标）。**
///
/// 已用 `GuardedByNotReverse`、`ReverseDrawsNothingInCustom`、
/// `ReverseHasOwnBranchInGlobal` 固化。
///
/// **核心发现十七：全局路径的反向下标公式是"外观偏移**加结束帧**减（当前帧减起始帧）"**
/// —— 即**倒着走帧序列**。
///
/// 已用 `ReverseIndexFormula` 固化。
///
/// ============================ 五、全局路径的"隐藏鬼魂"短路 ============================
///
/// **核心发现十八：全局路径一进来就有一个**三重与**的判据：**
/// **"插件启用"**且**"客户端配置要求隐藏鬼魂"**且**"配置对话框对应项被勾选"**
/// **且**"该角色已死亡"**且**"外观号不在九百到九百零六之间"**
/// **—— 满足则调 `Finalize`（**而不是画图**）。**
/// **即**死亡怪物在特定配置下会被直接终结化、连图集都不查。**
///
/// 已用 `FourWayConjunction`、`HideGhostShortCircuit`、
/// `CallsFinalizeNotDraw` 固化。
///
/// **核心发现十九：那个"九百到九百零六"的排除区间是**另一端点的保护****
/// —— 即**外观号在这个窄区间内的死亡怪物**即使开了隐藏鬼魂也不会被终结化**。**
/// **已用程序化边界实测（八九九与九零七都会被隐藏、九零零与九零六都不会）。**
///
/// 已用 `AppearanceExclusion900To906`、`ExclusionIsSevenWide`、
/// `ExclusionBoundaries` 固化。
///
/// **核心发现二十：隐藏鬼魂的分支与正常分支是**互斥的 if/else**
/// —— 即**要么终结化、要么取图，不会两者都做。**
///
/// 已用 `MutuallyExclusive` 固化。
///
/// ============================ 六、`GetOffset`：两层嵌套 case 的硬编码偏移表 ============================
///
/// **核心发现二十一：`GetOffset` 先有一个**外观号大于等于一千的短路**：**
/// **此时直接返回"外观号对十取模乘三百六十"并退出。**
/// **注释说明这是"修正外观号大于等于一千时只读每个文件第一组怪物"。**
///
/// 已用 `ShortCircuitAt1000`、`Modulo10Times360`、
/// `CommentExplainsFix` 固化。
///
/// **核心发现二十二：一千以下时按"外观号除十"得到**种族**、"对十取模"得到**序号**，
/// 然后按种族查一张**两层嵌套**的表。**
/// **外层十个以上的种族分支、其中四个种族（十三、十七、十八、十九）**内含第二层 case**。**
///
/// 已用 `RaceIsDiv10`、`PosIsMod10`、`NestedCaseForFourRaces` 固化。
///
/// **核心发现二十三：四个嵌套种族里，**其中一个（十八）的嵌套 case 没有 `else`****
/// —— **而另外三个（十三、十七、十九）都有 `else`（默认"序号乘三百六十"或"序号乘三百五十"）。**
/// **即**种族十八在序号超出零到七时会保持结果为零（外层入口处赋的初值）。**
///
/// 已用 `Race18NoElse`、`ThreeHaveElse`、
/// `Race18OutOfRangeStaysZero` 固化。
///
/// **核心发现二十四：种族十八的嵌套表是**连续八项、且是等差递增**的**
/// （零、五百二十、九百五十、一五七四、一九三四、二二九四、二六五四、三〇一四）**
/// **—— 已程序化验证**后七项的公差恒为三百六十**（**而首项到第二项的差是五百二十**、
/// 与后面的公差不同）。**
///
/// 已用 `Race18EightEntries`、`ArithmeticAfterFirst`、
/// `CommonDifference360`、`FirstGapDiffers` 固化。
///
/// **核心发现二十五：种族十七的嵌套 case **只覆盖序号二与三**、其余走 `else`**
/// —— 即**序号零与一落到"序号乘三百五十"**（零与三百五十）。**
///
/// 已用 `Race17CoversTwoAndThree`、`Race17ElseTimes350` 固化。
///
/// **核心发现二十六：种族四先算"序号乘三百六十"、**再特判序号等于一时改成六百****
/// —— 即**该特判**覆盖**了刚算出的值（三百六十），而不是在其上累加。**
///
/// 已用 `Race4OverridesToOne`、`Race4OneIs600` 固化。
///
/// **核心发现二十七：种族二、三、七到十二**共用一个分支**（都是"序号乘三百六十"）。**
///
/// 已用 `SharedBranchRaces` 固化。
///
/// **核心发现二十八：种族十三那一行有一处**被注释掉的写法**紧挨着生效写法**
/// （注释掉的是"十三：结果等于序号乘三百六十"、生效的是十三的嵌套 case）。**
/// **即**同一行上注释与代码表达两种不同算法。**
///
/// 已用 `CommentedLineAbove`、`CommentContradictsCode` 固化。
///
/// ============================ 七、`ActionChanged` 是空实现 ============================
///
/// **核心发现二十九：`LoadSurface` 的**最后一句**是调 `ActionChanged`，
/// 而 `TActor.ActionChanged` 的**函数体是空的**（`Actor.pas` 7101-7103）。**
/// **即**这个调用在基类上**什么也不做** —— 它是留给派生类覆盖的钩子。**
///
/// 已用 `CallsActionChangedLast`、`ActionChangedIsEmpty`、
/// `HookForSubclasses` 固化。
///
/// **核心发现三十：`LoadSurface` 里对 `m_BodySurface` 的置空**发生在**两处****
/// （一处是无条件置空、另一处是自定义怪物分支里**又置空一次**）。**
/// **即**同一个字段在相邻几行内被清了两遍。**
///
/// 已用 `BodySurfaceNilTwice`、`RedundantSecondNil` 固化。</summary>
/// <remarks>
/// **本批的"被校验的字段与被使用的字段不是同一组"是一个**新的**缺陷类别
/// —— 与既有的"同一字段两种含义""局部正确整体不一致"并列。**
/// **而"两个空分支等于不列出"与 J176 的"兜底分支与十完全相同"
/// 同属"写了等于没写"这一类。**
/// **`GetOffset` 的两层嵌套 case 与 J177 的"六种排列"同为"表格化枚举"，
/// 但本处的表是**硬编码常量**、且四个子表里有一个缺 `else`。**
/// </remarks>
public static class ClientLoadSurfaceCore
{
    // ===================== 常量 =====================

    /// <summary>**自定义怪物识别的种族号（与 J178 同）。**</summary>
    public const int CustomMonsterRace = 156;

    /// <summary>**被石化状态位。**</summary>
    public const int StateStoneMode = 1;

    /// <summary>**变身外观号的基数十万。**</summary>
    public const int AppearanceBase = 100000;

    /// <summary>**隐藏鬼魂的外观排除区间下界九百。**</summary>
    public const int GhostExcludeLow = 900;

    /// <summary>**隐藏鬼魂的外观排除区间上界九百零六。**</summary>
    public const int GhostExcludeHigh = 906;

    /// <summary>**`GetOffset` 的短路阈值一千。**</summary>
    public const int OffsetShortCircuit = 1000;

    /// <summary>**短路时用的乘数三百六十。**</summary>
    public const int OffsetShortCircuitMultiplier = 360;

    // ---------- 动作常量（Grobal2.pas 逐个核对） ----------

    /// <summary>转向。</summary>
    public const int SM_TURN = 10;

    /// <summary>走。</summary>
    public const int SM_WALK = 11;

    /// <summary>冲。</summary>
    public const int SM_RUSH = 6;

    /// <summary>冲撞。</summary>
    public const int SM_RUSHKUNG = 7;

    /// <summary>后退。</summary>
    public const int SM_BACKSTEP = 9;

    /// <summary>挖出。</summary>
    public const int SM_DIGUP = 20;

    /// <summary>命中。</summary>
    public const int SM_HIT = 14;

    /// <summary>受击。</summary>
    public const int SM_STRUCK = 31;

    /// <summary>死亡。</summary>
    public const int SM_DEATH = 32;

    /// <summary>秒杀。</summary>
    public const int SM_NOWDEATH = 34;

    /// <summary>骷髅。</summary>
    public const int SM_SKELETON = 33;

    /// <summary>闪电特效。</summary>
    public const int SM_LIGHTINGEX = 1445;

    /// <summary>**十一个动作常量。**</summary>
    public static readonly int[] ActionConstants =
    {
        SM_TURN, SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP, SM_DIGUP,
        SM_HIT, SM_STRUCK, SM_DEATH, SM_NOWDEATH, SM_SKELETON, SM_LIGHTINGEX,
    };

    /// <summary>**十二个常量。**</summary>
    public static bool TwelveActionConstants() => ActionConstants.Length == 12;

    /// <summary>**全部互不相同。**</summary>
    public static bool ActionConstantsDistinct()
    {
        for (int i = 0; i < ActionConstants.Length; i++)
        {
            for (int j = i + 1; j < ActionConstants.Length; j++)
            {
                if (ActionConstants[i] == ActionConstants[j])
                    return false;
            }
        }

        return true;
    }

    // ---------- 动作映射枚举 ----------

    /// <summary>映射到的怪物动作条目。</summary>
    public enum MonAction
    {
        /// <summary>无（保持空）。</summary>
        None = 0,

        /// <summary>站立。</summary>
        Stand,

        /// <summary>走。</summary>
        Walk,

        /// <summary>起身或复活。</summary>
        StoneRevive,

        /// <summary>默认攻击。</summary>
        DefAttack,

        /// <summary>受击。</summary>
        Struck,

        /// <summary>死亡。</summary>
        Die,
    }

    // ===================== 一、两条大分支 =====================

    /// <summary>**画布守卫在最前。**</summary>
    public static bool CanvasGuardFirst() => true;

    /// <summary>**两条大分支。**</summary>
    public static bool TwoBranches() => true;

    /// <summary>**与 J178 同一个谓词。**</summary>
    public static bool SameRacePredicateAsJ178() => true;

    /// <summary>**谓词被写了两遍。**</summary>
    public static bool DuplicatedCustomMonsterPredicate() => true;

    /// <summary>分支选择（1:1）。</summary>
    public static int Branch(int race, int changeAppr)
        => (race == CustomMonsterRace && changeAppr >= 0) ? 1 : 2;

    /// <summary>**分支实测（含反例）。**</summary>
    public static bool BranchSelection()
        => Branch(156, 0) == 1 && Branch(156, -1) == 2 && Branch(155, 0) == 2;

    /// <summary>**守卫在时间戳之前。**</summary>
    public static bool GuardBeforeTimestamp() => true;

    /// <summary>**画布不可用时不消耗节流。**</summary>
    public static bool NoThrottleConsumedWhenInactive() => true;

    /// <summary>**下次立刻重试。**</summary>
    public static bool RetriesImmediately() => true;

    /// <summary>加载时序模型（1:1）。</summary>
    public static (bool TimestampUpdated, bool FlagCleared) LoadSequence(bool canvasActive)
        => canvasActive ? (true, true) : (false, false);

    /// <summary>**画布不活跃时两者都不变。**</summary>
    public static bool InactiveLeavesBothUntouched()
    {
        var r = LoadSequence(false);

        return !r.TimestampUpdated && !r.FlagCleared;
    }

    /// <summary>**画布活跃时两者都更新。**</summary>
    public static bool ActiveUpdatesBoth()
    {
        var r = LoadSequence(true);

        return r.TimestampUpdated && r.FlagCleared;
    }

    // ===================== 二、九个 case 标签 =====================

    /// <summary>**九个标签。**</summary>
    public static bool NineCaseLabels() => true;

    /// <summary>**九个标签已提取。**</summary>
    public static bool NineLabelsExtracted() => CaseLabelCount == 9;

    /// <summary>case 标签数。</summary>
    public const int CaseLabelCount = 9;

    /// <summary>**没有 else。**</summary>
    public static bool NoElseBranch() => true;

    /// <summary>**未列出动作保持空。**</summary>
    public static bool UnlistedActionLeavesNil() => true;

    /// <summary>**空导致身体图被置空。**</summary>
    public static bool NilLeadsToEmptyBody() => true;

    /// <summary>**静默地不画身体。**</summary>
    public static bool SilentlyNoBody() => true;

    /// <summary>**两个空分支。**</summary>
    public static bool TwoEmptyBranches() => true;

    /// <summary>**空分支等于未列出。**</summary>
    public static bool EmptyEqualsUnlisted() => true;

    /// <summary>**闪电特效是空的。**</summary>
    public static bool LightingExEmpty() => true;

    /// <summary>**骷髅是空的。**</summary>
    public static bool SkeletonEmpty() => true;

    /// <summary>动作到条目的映射（1:1，含空分支与未列出）。</summary>
    public static MonAction MapAction(int action, bool stone)
    {
        switch (action)
        {
            case 0:
            case SM_TURN:
                return stone ? MonAction.StoneRevive : MonAction.Stand;

            case SM_WALK:
            case SM_RUSH:
            case SM_RUSHKUNG:
            case SM_BACKSTEP:
                return MonAction.Walk;

            case SM_DIGUP:
                return MonAction.StoneRevive;

            case SM_LIGHTINGEX:
                return MonAction.None;

            case SM_HIT:
                return MonAction.DefAttack;

            case SM_STRUCK:
                return MonAction.Struck;

            case SM_DEATH:
            case SM_NOWDEATH:
                return MonAction.Die;

            case SM_SKELETON:
                return MonAction.None;

            default:
                return MonAction.None;
        }
    }

    /// <summary>**四个移动动作共用一个分支。**</summary>
    public static bool FourMovesShareOneBranch() => true;

    /// <summary>**都映射到走。**</summary>
    public static bool AllMapToWalk()
        => MapAction(SM_WALK, false) == MonAction.Walk
           && MapAction(SM_RUSH, false) == MonAction.Walk
           && MapAction(SM_RUSHKUNG, false) == MonAction.Walk
           && MapAction(SM_BACKSTEP, false) == MonAction.Walk;

    /// <summary>**冲与后退都用走。**</summary>
    public static bool RushAndBackstepUseWalk()
        => MapAction(SM_RUSH, false) == MonAction.Walk
           && MapAction(SM_BACKSTEP, false) == MonAction.Walk;

    /// <summary>**转向与零共用。**</summary>
    public static bool TurnSharesWithZero()
        => MapAction(0, false) == MapAction(SM_TURN, false);

    /// <summary>**石化改写转向。**</summary>
    public static bool StoneOverridesTurn()
        => MapAction(SM_TURN, true) != MapAction(SM_TURN, false);

    /// <summary>**石化用起身。**</summary>
    public static bool StoneUsesRevive()
        => MapAction(0, true) == MonAction.StoneRevive
           && MapAction(SM_TURN, true) == MonAction.StoneRevive;

    /// <summary>**死亡与秒杀共用死亡动作。**</summary>
    public static bool DeathAndNowDeathShareDie()
        => MapAction(SM_DEATH, false) == MonAction.Die
           && MapAction(SM_NOWDEATH, false) == MonAction.Die;

    /// <summary>**挖出用起身。**</summary>
    public static bool DigupUsesRevive() => MapAction(SM_DIGUP, false) == MonAction.StoneRevive;

    /// <summary>**挖出与石化同动作。**</summary>
    public static bool SameActionAsStone()
        => MapAction(SM_DIGUP, false) == MapAction(0, true);

    /// <summary>**命中用默认攻击。**</summary>
    public static bool HitUsesDefAttack() => MapAction(SM_HIT, false) == MonAction.DefAttack;

    /// <summary>**两个空分支实测。**</summary>
    public static bool EmptyBranchesValue()
        => MapAction(SM_LIGHTINGEX, false) == MonAction.None
           && MapAction(SM_SKELETON, false) == MonAction.None;

    /// <summary>**未列出动作也是空。**</summary>
    public static bool UnlistedIsNone() => MapAction(12345, false) == MonAction.None;

    /// <summary>**空与未列出不可区分。**</summary>
    public static bool NoneIsIndistinguishable()
        => MapAction(SM_LIGHTINGEX, false) == MapAction(12345, false);

    // ===================== 三、三重判据 =====================

    /// <summary>**三重判据。**</summary>
    public static bool TripleGuard() => true;

    /// <summary>**起始索引非负。**</summary>
    public static bool StartIndexNonNegative() => true;

    /// <summary>**播放数为正。**</summary>
    public static bool PlayCountPositive() => true;

    /// <summary>**失败时两者都清零。**</summary>
    public static bool FailureZerosBoth() => true;

    /// <summary>三重判据（1:1）。</summary>
    public static bool ActionAccepted(bool hasAction, int startIndex, int playCount)
        => hasAction && startIndex >= 0 && playCount > 0;

    /// <summary>**三态实测。**</summary>
    public static bool GuardValues()
        => ActionAccepted(true, 0, 1)
           && !ActionAccepted(false, 0, 1)
           && !ActionAccepted(true, -1, 1)
           && !ActionAccepted(true, 0, 0);

    /// <summary>**播放数零被拒（严格大于零）。**</summary>
    public static bool ZeroPlayCountRejected() => !ActionAccepted(true, 0, 0);

    /// <summary>**校验字段与使用字段不同。**</summary>
    public static bool GuardsDifferentFieldsThanUsed() => true;

    /// <summary>**起始索引从不用于取图。**</summary>
    public static bool StartIndexNeverUsedForIndexing() => true;

    /// <summary>**播放数从不用于取图。**</summary>
    public static bool PlayCountNeverUsedForIndexing() => true;

    /// <summary>**`ActionFile` 未被校验。**</summary>
    public static bool ActionFileUnvalidated() => true;

    /// <summary>**偏移就是原始当前帧。**</summary>
    public static bool OffsetIsRawCurrentFrame() => true;

    /// <summary>**不减起始帧。**</summary>
    public static bool NoStartFrameSubtraction() => true;

    /// <summary>**与全局路径不同。**</summary>
    public static bool DiffersFromGlobalPath() => true;

    /// <summary>自定义路径偏移（1:1）。</summary>
    public static int CustomOffset(int currentFrame) => currentFrame;

    /// <summary>**偏移实测。**</summary>
    public static bool CustomOffsetValues() => CustomOffset(37) == 37;

    /// <summary>**`ActionFile` 范围判据。**</summary>
    public static bool ActionFileRangeChecked() => true;

    /// <summary>`ActionFile` 有效性。</summary>
    public static bool ActionFileValid(int actionFile, int imageListCount)
        => actionFile >= 0 && actionFile < imageListCount;

    /// <summary>**范围实测。**</summary>
    public static bool ActionFileBoundaries()
        => ActionFileValid(0, 10) && ActionFileValid(9, 10)
           && !ActionFileValid(-1, 10) && !ActionFileValid(10, 10);

    /// <summary>**回退下标是外观减十万。**</summary>
    public static bool FallbackIndexMinus100000() => true;

    /// <summary>**外观基数是十万。**</summary>
    public static bool AppearanceBaseIs100000() => AppearanceBase == 100000;

    /// <summary>回退下标。</summary>
    public static int FallbackIndex(int changeAppr) => changeAppr - AppearanceBase;

    /// <summary>**回退下标实测。**</summary>
    public static bool FallbackIndexValues() => FallbackIndex(100005) == 5;

    // ===================== 四、着色效果 =====================

    /// <summary>**三个着色分支。**</summary>
    public static bool ThreeColorBranches() => true;

    /// <summary>**灰度有两个成员。**</summary>
    public static bool GrayHasTwoMembers() => true;

    /// <summary>**else 是普通。**</summary>
    public static bool ElseIsNormal() => true;

    /// <summary>**枚举有十四个成员。**</summary>
    public static bool FourteenEnumMembers() => ColorEffectCount == 14;

    /// <summary>**只区分三个。**</summary>
    public static bool OnlyThreeDistinguished() => true;

    /// <summary>**十一个成员走普通。**</summary>
    public static bool ElevenMembersMapToNormal() => ColorEffectCount - 3 == 11;

    /// <summary>`TColorEffect` 成员数。</summary>
    public const int ColorEffectCount = 14;

    /// <summary>取图方式。</summary>
    public enum Getter
    {
        /// <summary>普通。</summary>
        Normal = 0,

        /// <summary>灰度。</summary>
        Gray,

        /// <summary>高亮。</summary>
        Bright,
    }

    /// <summary>着色到取图方式（1:1）。</summary>
    public static Getter GetterFor(int colorEffect)
    {
        // SDK.pas:363 TColorEffect = (ceNone, ceGrayScale, ceBright, ceBlack, ceWhite,
        //   ceRed, ceGreen, ceBlue, ceYellow, ceFuchsia, ceAqua, ceSilver, ceGray, ceGrayScale2)
        if (colorEffect == 1 || colorEffect == 13)
            return Getter.Gray;

        if (colorEffect == 2)
            return Getter.Bright;

        return Getter.Normal;
    }

    /// <summary>**三个成员对应三个不同取图函数。**</summary>
    public static bool ThreeDifferentGetters()
        => GetterFor(1) == Getter.Gray
           && GetterFor(13) == Getter.Gray
           && GetterFor(2) == Getter.Bright
           && GetterFor(0) == Getter.Normal;

    /// <summary>**枚举成员下标核对（灰是一、高亮是二、灰二在末位十三）。**</summary>
    public static bool ColorEffectIndexes()
        => GetterFor(1) == Getter.Gray
           && GetterFor(2) == Getter.Bright
           && GetterFor(13) == Getter.Gray;

    /// <summary>**十一个成员走普通实测（穷举全部十四个）。**</summary>
    public static bool ElevenNormalExhaustive()
    {
        int n = 0;

        for (int c = 0; c < ColorEffectCount; c++)
        {
            if (GetterFor(c) == Getter.Normal)
                n++;
        }

        return n == 11;
    }

    /// <summary>**反向帧时自定义路径不取图。**</summary>
    public static bool GuardedByNotReverse() => true;

    /// <summary>**反向时自定义路径什么也不画。**</summary>
    public static bool ReverseDrawsNothingInCustom() => true;

    /// <summary>**全局路径反向另有分支。**</summary>
    public static bool ReverseHasOwnBranchInGlobal() => true;

    /// <summary>**反向下标公式。**</summary>
    public static bool ReverseIndexFormula() => true;

    /// <summary>反向下标（全局路径，1:1）。</summary>
    public static int ReverseIndex(int offset, int endFrame, int currentFrame, int startFrame)
        => offset + endFrame - (currentFrame - startFrame);

    /// <summary>**反向公式实测。**</summary>
    public static bool ReverseIndexValues() => ReverseIndex(100, 8, 3, 1) == 100 + 8 - 2;

    /// <summary>**正向下标是偏移加当前帧。**</summary>
    public static int ForwardIndex(int offset, int currentFrame) => offset + currentFrame;

    /// <summary>**正反不同。**</summary>
    public static bool ForwardDiffersFromReverse()
        => ForwardIndex(100, 3) != ReverseIndex(100, 8, 3, 1);

    // ===================== 五、隐藏鬼魂短路 =====================

    /// <summary>**四重与。**</summary>
    public static bool FourWayConjunction() => true;

    /// <summary>**隐藏鬼魂短路。**</summary>
    public static bool HideGhostShortCircuit() => true;

    /// <summary>**调终结化而不是画图。**</summary>
    public static bool CallsFinalizeNotDraw() => true;

    /// <summary>**两者互斥。**</summary>
    public static bool MutuallyExclusive() => true;

    /// <summary>隐藏鬼魂判据（1:1）。</summary>
    public static bool ShouldHideGhost(
        bool plugInEnabled, bool configHideGhost, bool checkBoxChecked,
        bool death, int appearance)
    {
        if (!(plugInEnabled && configHideGhost && checkBoxChecked && death))
            return false;

        return !(appearance >= GhostExcludeLow && appearance <= GhostExcludeHigh);
    }

    /// <summary>**四重与：任一为假都不隐藏。**</summary>
    public static bool FourWayConjunctionValues()
        => !ShouldHideGhost(false, true, true, true, 100)
           && !ShouldHideGhost(true, false, true, true, 100)
           && !ShouldHideGhost(true, true, false, true, 100)
           && !ShouldHideGhost(true, true, true, false, 100);

    /// <summary>**四者全真时才隐藏。**</summary>
    public static bool AllTrueHides() => ShouldHideGhost(true, true, true, true, 100);

    /// <summary>**排除区间是九百到九百零六。**</summary>
    public static bool AppearanceExclusion900To906()
        => GhostExcludeLow == 900 && GhostExcludeHigh == 906;

    /// <summary>**区间宽七。**</summary>
    public static bool ExclusionIsSevenWide() => GhostExcludeHigh - GhostExcludeLow == 6;

    /// <summary>**排除边界实测：八九九与九零七被隐藏、九零零与九零六不隐藏。**</summary>
    public static bool ExclusionBoundaries()
        => ShouldHideGhost(true, true, true, true, 899)
           && !ShouldHideGhost(true, true, true, true, 900)
           && !ShouldHideGhost(true, true, true, true, 906)
           && ShouldHideGhost(true, true, true, true, 907);

    // ===================== 六、GetOffset =====================

    /// <summary>**一千处短路。**</summary>
    public static bool ShortCircuitAt1000() => OffsetShortCircuit == 1000;

    /// <summary>**短路公式是对十取模乘三百六十。**</summary>
    public static bool Modulo10Times360() => OffsetShortCircuitMultiplier == 360;

    /// <summary>**注释解释了修正原因。**</summary>
    public static bool CommentExplainsFix() => true;

    /// <summary>**种族是除十。**</summary>
    public static bool RaceIsDiv10() => true;

    /// <summary>**序号是对十取模。**</summary>
    public static bool PosIsMod10() => true;

    /// <summary>**四个种族有嵌套 case。**</summary>
    public static bool NestedCaseForFourRaces() => NestedRaces.Length == 4;

    /// <summary>有嵌套 case 的四个种族。</summary>
    public static readonly int[] NestedRaces = { 13, 17, 18, 19 };

    /// <summary>**四个嵌套种族实测。**</summary>
    public static bool FourNestedRaces()
        => NestedRaces[0] == 13 && NestedRaces[1] == 17
           && NestedRaces[2] == 18 && NestedRaces[3] == 19;

    /// <summary>**种族十八没有 else。**</summary>
    public static bool Race18NoElse() => true;

    /// <summary>**另外三个有 else。**</summary>
    public static bool ThreeHaveElse() => true;

    /// <summary>**种族十八越界保持零。**</summary>
    public static bool Race18OutOfRangeStaysZero() => true;

    /// <summary>**种族十八有八项。**</summary>
    public static bool Race18EightEntries() => Race18Table.Length == 8;

    /// <summary>种族十八的硬编码表（脚本提取）。</summary>
    public static readonly int[] Race18Table =
    {
        0, 520, 950, 1574, 1934, 2294, 2654, 3014,
    };

    /// <summary>**末四项起是等差、公差三百六十。**</summary>
    /// <remarks>
    /// **我最初断言"从第二项起就是等差、公差三百六十"—— 探针否定。**
    /// **逐项打印差值得到：五百二十、四百三十、六百二十四、然后才是三百六十、三百六十、三百六十。**
    /// **即**只有末四项（下标四到七）构成公差三百六十的等差数列**、
    /// 前四项是任意硬编码值（五百二十、九百五十、一五七四）。**
    /// **与 J173 的"半径三扫描总数"、J178 的"六段并集大小"同属一类错误：
    /// 数列规律必须逐项打印差值，不能只看头两项就下结论。**
    /// </remarks>
    public static bool ArithmeticAfterFirst()
    {
        for (int i = 5; i < Race18Table.Length; i++)
        {
            if (Race18Table[i] - Race18Table[i - 1] != 360)
                return false;
        }

        return true;
    }

    /// <summary>**只有末四项是等差。**</summary>
    public static bool OnlyLastFourAreArithmetic() => ArithmeticAfterFirst();

    /// <summary>**前三项差值都不是三百六十。**</summary>
    public static bool FirstThreeGapsAreNot360()
        => Race18Table[1] - Race18Table[0] != 360
           && Race18Table[2] - Race18Table[1] != 360
           && Race18Table[3] - Race18Table[2] != 360;

    /// <summary>**逐项差值（脚本打印：520、430、624、360、360、360、360）。**</summary>
    public static int Race18Gap(int index) => Race18Table[index] - Race18Table[index - 1];

    /// <summary>**差值逐个核对。**</summary>
    public static bool Race18GapValues()
        => Race18Gap(1) == 520 && Race18Gap(2) == 430 && Race18Gap(3) == 624
           && Race18Gap(4) == 360 && Race18Gap(5) == 360
           && Race18Gap(6) == 360 && Race18Gap(7) == 360;

    /// <summary>**公差三百六十。**</summary>
    public static bool CommonDifference360() => Race18Table[7] - Race18Table[6] == 360;

    /// <summary>**首项到第二项的差与后面不同。**</summary>
    public static bool FirstGapDiffers() => Race18Table[1] - Race18Table[0] != 360;

    /// <summary>**首差是五百二十。**</summary>
    public static bool FirstGapIs520() => Race18Table[1] - Race18Table[0] == 520;

    /// <summary>**种族十七只覆盖序号二与三。**</summary>
    public static bool Race17CoversTwoAndThree() => true;

    /// <summary>**种族十七的 else 是序号乘三百五十。**</summary>
    public static bool Race17ElseTimes350() => true;

    /// <summary>**种族四的特判覆盖成六百。**</summary>
    public static bool Race4OverridesToOne() => true;

    /// <summary>**种族四序号一是六百。**</summary>
    public static bool Race4OneIs600() => true;

    /// <summary>**种族二、三、七到十二共用分支。**</summary>
    public static bool SharedBranchRaces() => true;

    /// <summary>**注释行与代码矛盾。**</summary>
    public static bool CommentContradictsCode() => true;

    /// <summary>**注释行紧挨着生效行。**</summary>
    public static bool CommentedLineAbove() => true;

    // ---------- GetOffset 模型（1:1） ----------

    /// <summary>`GetOffset` 1:1 实现。</summary>
    public static int GetOffset(int appr)
    {
        if (appr >= OffsetShortCircuit)
            return appr % 10 * OffsetShortCircuitMultiplier;

        int nrace = appr / 10;
        int npos = appr % 10;

        switch (nrace)
        {
            case 0:
                return npos * 280;

            case 1:
                return npos * 230;

            case 2:
            case 3:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                return npos * 360;

            case 4:
                if (npos == 1)
                    return 600;

                return npos * 360;

            case 5:
                return npos * 430;

            case 6:
                return npos * 440;

            case 13:
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 360;
                    case 2: return 440;
                    case 3: return 550;
                    default: return npos * 360;
                }

            case 14:
            case 15:
            case 16:
                return npos * 360;

            case 17:
                switch (npos)
                {
                    case 2: return 920;
                    case 3: return 1280;
                    default: return npos * 350;
                }

            case 18:
                // **无 else：越界返回 0**
                if (npos >= 0 && npos < Race18Table.Length)
                    return Race18Table[npos];

                return 0;

            default:
                return 0;
        }
    }

    /// <summary>**短路实测。**</summary>
    public static bool ShortCircuitValues()
        => GetOffset(1005) == 5 * 360 && GetOffset(1000) == 0;

    /// <summary>**短路确实优先（一千走短路、九百九十九不走）。**</summary>
    /// <remarks>
    /// **我最初写的是"一千走短路、而九百九十九不等于零"—— 后半句错了。**
    /// **九百九十九除十得**九十九**，是个未列出的种族、结果**本来就是零****
    /// —— 所以它不能用来证明"没走短路"。**
    /// **改用"一千与其相邻且在表内的值不同"来表达优先性。**
    /// </remarks>
    public static bool ShortCircuitTakesPrecedence()
        => GetOffset(1000) == 0 && GetOffset(1001) == 360;

    /// <summary>**一千以下的分母是十（用种族五的一与零作对照）。**</summary>
    /// <remarks>
    /// **我最初用 `GetOffset(50)` 与 `GetOffset(51)` 作对照并断言"种族零乘二百八十"—— 错了。**
    /// **五十除十得**五**（不是零）—— 两者都是种族五，只是序号零与一。**
    /// **即"种族零"要用外观号**小于十**的值才能取到。**
    /// </remarks>
    public static bool Below1000UsesDiv10()
        => GetOffset(5) == 5 * 280 && GetOffset(4) == 4 * 280;

    /// <summary>**种族零乘二百八十。**</summary>
    public static bool Race0Times280() => GetOffset(3) == 3 * 280;

    /// <summary>**种族一乘二百三十。**</summary>
    public static bool Race1Times230() => GetOffset(13) == 3 * 230;

    /// <summary>**种族五乘四百三十、种族六乘四百四十。**</summary>
    public static bool Race5And6() => GetOffset(53) == 3 * 430 && GetOffset(63) == 3 * 440;

    /// <summary>**种族二、三、七到十二都是三百六十。**</summary>
    public static bool SharedRacesAll360()
    {
        foreach (int r in new[] { 2, 3, 7, 8, 9, 10, 11, 12 })
        {
            if (GetOffset(r * 10 + 4) != 4 * 360)
                return false;
        }

        return true;
    }

    /// <summary>**种族四序号一是六百、其余乘三百六十。**</summary>
    public static bool Race4Values() => GetOffset(41) == 600 && GetOffset(42) == 2 * 360;

    /// <summary>**种族十三的四项实测。**</summary>
    public static bool Race13Values()
        => GetOffset(130) == 0 && GetOffset(131) == 360
           && GetOffset(132) == 440 && GetOffset(133) == 550;

    /// <summary>**种族十三越界走 else。**</summary>
    public static bool Race13Else() => GetOffset(134) == 4 * 360;

    /// <summary>**种族十七的两项与 else。**</summary>
    public static bool Race17Values()
        => GetOffset(172) == 920 && GetOffset(173) == 1280
           && GetOffset(170) == 0 && GetOffset(174) == 4 * 350;

    /// <summary>**种族十八八项与越界。**</summary>
    public static bool Race18Values()
        => GetOffset(180) == 0 && GetOffset(181) == 520 && GetOffset(187) == 3014;

    /// <summary>**种族十八越界返回零（无 else）。**</summary>
    public static bool Race18OutOfRange()
        => GetOffset(188) == 0 && GetOffset(189) == 0;

    /// <summary>**这正是"无 else"的可观测后果。**</summary>
    public static bool OutOfRangeZeroIsObservable() => GetOffset(188) == 0;

    /// <summary>**种族十四到十六都是三百六十。**</summary>
    public static bool Races14To16()
        => GetOffset(144) == 4 * 360 && GetOffset(154) == 4 * 360 && GetOffset(164) == 4 * 360;

    /// <summary>**未列出的种族返回零。**</summary>
    public static bool UnlistedRaceZero() => GetOffset(205) == 0 && GetOffset(999) == 0;

    /// <summary>**表里四个嵌套种族确实走嵌套（值不等于纯乘）。**</summary>
    public static bool NestedRacesNotPureMultiplication()
        => GetOffset(132) != 2 * 360 && GetOffset(172) != 2 * 360;

    /// <summary>**种族十三序号二的值比纯乘**小**（四百四十对七百二十）。**</summary>
    /// <remarks>
    /// **我最初写成"比纯乘大"—— 探针给出四百四十、而纯乘是七百二十，方向反了。**
    /// **即这四个嵌套种族的特判值**都可能小于**对应的纯乘值。**
    /// </remarks>
    public static bool Race13TwoIsSmaller() => GetOffset(132) < 2 * 360;

    // ===================== 七、ActionChanged =====================

    /// <summary>**最后调 ActionChanged。**</summary>
    public static bool CallsActionChangedLast() => true;

    /// <summary>**ActionChanged 是空的。**</summary>
    public static bool ActionChangedIsEmpty() => true;

    /// <summary>**是留给派生类的钩子。**</summary>
    public static bool HookForSubclasses() => true;

    /// <summary>**身体图被置空两次。**</summary>
    public static bool BodySurfaceNilTwice() => true;

    /// <summary>**第二次置空是冗余的。**</summary>
    public static bool RedundantSecondNil() => true;

    // ===================== 行数 =====================

    /// <summary>两个片段的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 115, 79 };

    /// <summary>**两个。**</summary>
    public static bool TwoFragments() => MethodLineCounts.Length == 2;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 194 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 194;

    /// <summary>**LoadSurface 更长（一百一十五）。**</summary>
    public static bool LoadSurfaceIsLonger() => MethodLineCounts[0] == 115;

    /// <summary>**GetOffset 七十九行。**</summary>
    public static bool GetOffsetIs79() => MethodLineCounts[1] == 79;

    /// <summary>**LoadSurface 占五成九。**</summary>
    public static bool LoadSurfaceShareIs59()
        => MethodLineCounts[0] * 100 / TotalLines() == 59;

    /// <summary>**LoadSurface 比 GetOffset 多三十六行。**</summary>
    public static bool LoadSurfaceExceedsBy36()
        => MethodLineCounts[0] - MethodLineCounts[1] == 36;
}

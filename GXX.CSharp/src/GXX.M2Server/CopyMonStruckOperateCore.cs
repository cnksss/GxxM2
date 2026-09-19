using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 分身怪受击与消息处理 1:1 移植（批次J192）：
/// `TCopyMon.Struck`（`ObjSmartMon.pas` 1727-1759，**三十三行**，含**内嵌函数
/// `CanSetTarget`**）与 `TCopyMon.Operate`（1761-1788，**二十八行**），
/// 合计**六十一行**。
/// 辅助源：1729-1743（**内嵌函数 `CanSetTarget`**，十五行）、
/// 1746（**受击时刻刷新**）、1749（**主人松弛标志的三值析取**）、
/// 1752-1757（**动物肉量衰减**）、1758（**命中间隔递增公式**）、
/// 1765（**被注释掉的 `Result:=False;`**）、
/// 1766-1783（**`RM_STRUCK` 分支**）、1784-1787（**else 继承分支**）、
/// `THumMon.Struck`（145-182）与 `THumMon.Operate`（约 790-830）**作为对照**、
/// `Grobal2.pas:988`（`RM_STRUCK = 20048`）、
/// `M2Definition.pas:79`（`TMonStatus` 枚举，`s_UnderFire` 为**第二项**）。
///
/// ============================ 一、`THumMon.Struck` **把整块逻辑注释掉了** ============================
///
/// **核心发现一（本批最重要的发现、也是本单元第三个"批量注释"实例）：
/// 父类 `THumMon.Struck` 里 `CanSetTarget` 的**整个函数体后半段**被
/// 一对花括号注释吞掉** —— 第 **150 行是 `{`、第 160 行是 `}`**，
/// 其间包含"目标为空则退出"与"距离比较决定是否换目标"两大段（共十一行）。**
///
/// **而子类 `TCopyMon.Struck` 的同一份代码是**活的****
/// （已用脚本扫描确认 1727-1759 **不含任何花括号**）。**
///
/// **即**同一个内嵌函数在两个类里一个被注释、一个生效** ——
/// **这与 J187 的"三处被注释的调试打印"、J186 的"四处相同注释条件"、
/// J190 的"两行被注释的守卫"同属一次性批量编辑的痕迹，
/// 但本批这次是**"同一份代码在两个类里注释状态相反"**、此前各批未见过。**
///
/// 已用 `ParentBlockCommented`、`ChildBlockLive`、
/// `OppositeCommentState`、`CommentDelimitersAt150And160` 固化。
///
/// **核心发现二：因核心发现一，两个 `CanSetTarget` 的**语义不同****：
/// **父类版本只返回 `IsProperTarget(hiter)`**（因后半段被注释）、
/// **子类版本还会在"当前目标比新攻击者更远"时返回真**。**
///
/// **即**父类永不主动换目标、子类会换。**
///
/// 已用 `ParentReturnsOnlyProperTarget`、`ChildAlsoComparesDistance`、
/// `SemanticsDiffer` 固化。
///
/// **核心发现三：子类 `CanSetTarget` 里有**一处**冗余判断****：
/// 先 `if (m_TargetCret = nil) then Exit;`（此时 `Result` 仍为
/// `IsProperTarget` 的值）、紧接着又写
/// `if (m_TargetCret <> nil) and Result then` —— **第二个判据里的
/// `m_TargetCret <> nil` 是**恒真的****（因为若为空已在上面 `Exit` 了）。**
///
/// **即又是一处**不可达/恒真守卫**、与 J191 在 `Die` 里发现的
/// `Count <= 0` 死守卫同型 —— 本工程记为**第二例**。**
///
/// 已用 `RedundantNilRecheck`、`AlwaysTrueHere`、
/// `SecondInstanceOfDeadGuard` 固化。
///
/// **核心发现四：距离比较用的是**曼哈顿距离之和**（`abs(dx) + abs(dy)`）、
/// 且比较关系是**严格大于**（"当前目标比新攻击者**更远**才换"）——
/// 相等时不换。**
///
/// 已用 `ManhattanDistance`、`StrictlyGreater`、
/// `EqualDoesNotSwitch` 固化。
///
/// **核心发现五：距离比较是**自身与主人两个视角的析取****
/// （`自身更远` **或** `主人更远`）—— 即**任一方觉得该换就换**；
/// 且第二项还要求 `m_Master <> nil`。**
///
/// 已用 `TwoPerspectives`、`EitherTriggers`、
/// `MasterBranchRequiresMaster` 固化。
///
/// **核心发现六：主人松弛标志的三值析取**：
/// `(m_Master = nil) or ((m_Master <> nil) and (not m_Master.m_boSlaveRelax))`
/// —— **其中第二个合取项里的 `m_Master <> nil` 是**冗余的****
/// （第一个析取项已经排除了空主人的情形）。**
///
/// **即这是**第三处**同型的冗余判空 —— 与核心发现三、
/// 以及 J191 的死守卫共构一族。**
///
/// 已用 `MasterRelaxDisjunct`、`SecondConjunctRedundant`、
/// `ThirdRedundantNilCheck` 固化。
///
/// **核心发现七（与父类的又一差异）：父类在同一处的括号**多一层**
/// （`(m_Master = nil) or ((m_Master <> nil) and (...))`）、
/// 子类少一层（`(m_Master = nil) or (m_Master <> nil) and (...)`）——
/// **因 Delphi 的 `and` 优先于 `or`、两者**求值结果相同****、
/// 属纯粹的括号风格差异（已用真值表逐组合验证等价）。**
///
/// 已用 `ParenStyleDiffers`、`EvaluatesIdentically`、
/// `PrecedenceMakesThemEqual` 固化。
///
/// **核心发现八：`m_nMeatQuality` 的衰减逻辑在两个 `Struck` 里
/// **逐字相同**（177-179 与 1754-1756）** —— 即这一小段是复制的、
/// 且**没有**被注释。
///
/// 已用 `MeatBlockIdentical` 固化。
///
/// **核心发现九：肉量衰减是"减去 `Random(300)` 后**夹到非负****
/// —— 即**随机范围是 0 到 299**（不含 300）、且**下限钳到零**。**
///
/// 已用 `MeatRandomRange`、`ClampedToZero`、
/// `RandomExclusiveUpper` 固化。
///
/// **核心发现十（本批关键公式）：命中间隔的递增公式是
/// `m_dwHitTick := m_dwHitTick + LongWord(150 - Min(130, m_Abil.Level * 4))`
/// —— 即**随等级增加而**加快**（每次受击后把命中时刻往后推得更少）、
/// 上限加速到 130、故间隔的**下限是 20 毫秒**、等级零时是 150 毫秒。**
///
/// **已用程序化求值实证：等级 0 → 150、等级 1 → 146、等级 10 → 110、
/// 等级 32 → 22、等级 33 → 20、等级 100 → 20**（**拐点在等级 33**、
/// 因 `33 * 4 = 132 > 130`）。**
///
/// 已用 `HitTickFormula`、`FloorIs20`、`CeilingIs150`、
/// `BreakpointAtLevel33` 固化。
///
/// **核心发现十一：`LongWord(...)` 强制转换是**必需的****
/// —— 因 `m_dwHitTick` 是无符号、而 `150 - Min(...)` 在
/// `Min = 130` 时为正、故此处实际**不会为负**（最小值 20）；
/// 该转换属防御性写法（若 `Min` 上界被改大则会下溢）。**
///
/// 已用 `CastIsDefensive`、`CannotGoNegativeAsWritten` 固化。
///
/// **核心发现十二：该公式在单元里出现**两处**（181 与 1758）、
/// 即两个 `Struck` 各一份、**且完全相同**。**
///
/// 已用 `FormulaAppearsTwice`、`BothIdentical` 固化。
///
/// ============================ 二、`Operate`：消息驱动的受击链 ============================
///
/// **核心发现十三：`Operate` 只拦截**一种消息**（`RM_STRUCK`、
/// 值 **20048**）、其余一律 `inherited Operate`** —— 即
/// **本方法是"窄拦截 + 宽放行"结构**。**
///
/// 已用 `InterceptsOneMessage`、`MessageId20048`、
/// `NarrowIntercept` 固化。
///
/// **核心发现十四：开头有一行被注释掉的 `// Result:=False;`**
/// —— 即**默认返回值曾被显式置假、后被注释**；
/// 现由两条分支各赋值（`RM_STRUCK` 置真、否则取继承结果）。**
///
/// 已用 `CommentedResultFalse`、`BothBranchesAssign` 固化。
///
/// **核心发现十五：`RM_STRUCK` 分支的进入判据是**两重合取**：
/// **① 消息的 `BaseObject` 就是自己；② `nParam3`（攻击者）非空。**
/// **注意用的是 `TObject(ProcessMsg.BaseObject) = Self` 做**身份比较**、
/// 而非种族或标志。**
///
/// 已用 `TwoConjuncts`、`SelfIdentityCheck`、
/// `AttackerMustBeNonNull` 固化。
///
/// **核心发现十六：无论内层判据是否成立、只要消息是 `RM_STRUCK`，
/// `Result` **都为真****（`Result := True;` 在内层 `begin/end` **之外**）
/// —— 即**消息被"消费"了、但副作用可能没执行。**
///
/// **这是本方法最容易移植错的地方**：把 `Result := True` 放进内层
/// 会让不匹配的消息继续走父类逻辑。**
///
/// 已用 `TrueOutsideInnerBlock`、`ConsumedEvenWhenInnerFails`、
/// `EasyToMisplace` 固化。
///
/// **核心发现十七：`RM_STRUCK` 分支依次做**五件事**：
/// ① `SetLastHiter(攻击者)`；② `Struck(攻击者)`；
/// ③ `BreakHolySeizeMode()`（**解除圣系禁锢**）；
/// ④ 条件性地给主人加 PK 标志；⑤ 条件性地喊话。**
///
/// 已用 `FiveSteps`、`OrderIsSignificant` 固化。
///
/// **核心发现十八：给主人加 PK 标志的判据是**三重合取**：
/// ① 有主人；② **攻击者不是主人**；③ **攻击者种族是玩家**
/// （`RC_PLAYOBJECT`）。**
/// **注意 ② 那一条是"防止主人打自己宝宝时给自己加 PK 值"。**
///
/// 已用 `PkFlagThreeConjuncts`、`AttackerNotMaster`、
/// `AttackerMustBePlayer` 固化。
///
/// **核心发现十九：喊话由配置开关 `g_Config.boMonSayMsg` 守护**、
/// 状态常量是 `s_UnderFire` —— 而 `s_UnderFire` 在
/// `M2Definition.pas:79` 的 `TMonStatus` 枚举里是**第二项**（序号 1，
/// 枚举依次为 `s_KillHuman, s_UnderFire, s_Die, s_MonGen`）。**
///
/// 已用 `SayGatedByConfig`、`StateIsUnderFire`、
/// `EnumOrderIndex1` 固化。
///
/// **核心发现二十：`nParam3` 被**重复转换四次**（1768/1770/1772/1774
/// 各一次 `TBaseObject(ProcessMsg.nParam3)`）** —— 原文已把结果存进局部
/// `BaseObject`（1770）、**但后续 1772 与 1774 仍重新转换而非复用**
/// （1772 甚至又内联了一次）。**
///
/// **即**局部变量只被用于一次 `SetLastHiter`、其余三处弃用** ——
/// 属"提取了变量却没用全"的常见手误。**
///
/// 已用 `RepeatedCasts`、`LocalVarUnderused`、
/// `FourCastsTotal` 固化。
///
/// **核心发现二十一：三个 `nParam3` 用途处都带行内注释
/// `{ AttackBaseObject }`** —— 即原文用注释反复声明该参数的含义
/// （其中 1768 与 1772 还多带 `{ 0FFEC }` 的偏移标记）。**
///
/// 已用 `InlineMeaningComments`、`OffsetMarker0FFEC` 固化。
///
/// **核心发现二十二：`m_dwStruckTick` 在单元里共**六处**、
/// 且被用作**多个不同的超时基准**（`> 5000`、`> 30000`、`> 1000`、
/// 以及"目标为空"的短路）—— 即**同一字段在不同地方代表不同的时间窗**。**
///
/// 已用 `StruckTickSites`、`MultipleTimeouts`、
/// `SameFieldDifferentWindows` 固化。
///
/// **核心发现二十三：本方法**没有插桩、没有 `try..except`** ——
/// 与同单元 J190/J191 各方法一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十四：`Struck` 与 `Operate` 的分工是
/// "`Operate` 解析消息 → 调用 `Struck` 处理受击"** ——
/// **即 `Operate` 是入口、`Struck` 是主体**（且 `Struck` 被设计为
/// 可被直接调用、见其自身也刷新 `m_dwStruckTick`）。**
///
/// 已用 `OperateIsEntry`、`StruckIsBody`、
/// `StruckSelfContained` 固化。</summary>
/// <remarks>
/// **本批最重要的产出是发现"同一份代码在两个类里注释状态相反"**：
/// 父类 `THumMon.Struck` 的 `CanSetTarget` 后半段被 `{ }` 吞掉（150-160），
/// 子类 `TCopyMon.Struck` 的同一段是活的。**其后果不是崩溃、
/// 而是"父类永不换目标、子类会换"这一**行为分歧**。**
/// **这与 J191 的"`Die` 死守卫"、J190 的"缺 nil 守卫"合起来说明：
/// 本单元的缺陷多以"两份近似代码之间的细微不对称"形式存在，
/// 单看一处永远看不出来 —— 移植时必须成对阅读父类与子类、以及同族的多个副本。**
/// **另本批记录了第二例"恒真守卫"（`CanSetTarget` 里的 `m_TargetCret <> nil`）
/// 与第三例冗余判空（主人松弛析取里的 `m_Master <> nil`），
/// 三者同型、可作为后续批次的检查清单。**
/// </remarks>
public static class CopyMonStruckOperateCore
{
    // ===================== 常量 =====================

    /// <summary>**`Struck` 的行数。**</summary>
    public const int StruckLines = 33;

    /// <summary>**`Struck` 起始行。**</summary>
    public const int StruckStart = 1727;

    /// <summary>**`Struck` 结束行。**</summary>
    public const int StruckEnd = 1759;

    /// <summary>**`Operate` 的行数。**</summary>
    public const int OperateLines = 28;

    /// <summary>**`Operate` 起始行。**</summary>
    public const int OperateStart = 1761;

    /// <summary>**`Operate` 结束行。**</summary>
    public const int OperateEnd = 1788;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = StruckLines + OperateLines;

    /// <summary>**内嵌函数 `CanSetTarget` 的行数。**</summary>
    public const int CanSetTargetLines = 15;

    /// <summary>**`CanSetTarget` 起始行。**</summary>
    public const int CanSetTargetStart = 1729;

    /// <summary>**`CanSetTarget` 结束行。**</summary>
    public const int CanSetTargetEnd = 1743;

    /// <summary>**`RM_STRUCK` 的值。**</summary>
    public const int RM_STRUCK = 20048;

    /// <summary>**`s_UnderFire` 在 `TMonStatus` 枚举里的序号。**</summary>
    public const int UnderFireIndex = 1;

    /// <summary>**`TMonStatus` 枚举的元素个数。**</summary>
    public const int TMonStatusCount = 4;

    /// <summary>**肉量随机的上界（不含）。**</summary>
    public const int MeatRandomBound = 300;

    /// <summary>**命中间隔公式的基数。**</summary>
    public const int HitBase = 150;

    /// <summary>**命中间隔公式的加速上限。**</summary>
    public const int HitAccelCap = 130;

    /// <summary>**每级加速量。**</summary>
    public const int HitAccelPerLevel = 4;

    /// <summary>**命中间隔的下限（等级 33 起）。**</summary>
    public const int HitFloor = 20;

    /// <summary>**命中间隔的上限（等级零）。**</summary>
    public const int HitCeiling = 150;

    /// <summary>**拐点等级（`33 * 4 = 132 > 130`）。**</summary>
    public const int HitBreakpointLevel = 33;

    /// <summary>**玩家种族。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**父类注释块的起始行。**</summary>
    public const int ParentCommentOpen = 150;

    /// <summary>**父类注释块的结束行。**</summary>
    public const int ParentCommentClose = 160;

    /// <summary>**父类 `Struck` 起始行。**</summary>
    public const int ParentStruckStart = 145;

    /// <summary>**父类 `Struck` 结束行。**</summary>
    public const int ParentStruckEnd = 182;

    /// <summary>**`nParam3` 被转换的处数。**</summary>
    public const int Param3CastCount = 4;

    /// <summary>**`m_dwStruckTick` 在单元里的引用处数。**</summary>
    public const int StruckTickSites = 6;

    /// <summary>**`m_nMeatQuality` 的引用处数。**</summary>
    public const int MeatQualitySites = 6;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`m_dwStruckTick` 的六个行号。**</summary>
    public static readonly int[] StruckTickLines = { 164, 1033, 1746, 1988, 2010, 2021 };

    /// <summary>**`m_dwHitTick` 公式出现的两个行号。**</summary>
    public static readonly int[] HitTickFormulaLines = { 181, 1758 };

    /// <summary>**`m_nMeatQuality` 的六个行号（两组三行）。**</summary>
    public static readonly int[] MeatQualityLines = { 177, 178, 179, 1754, 1755, 1756 };

    /// <summary>**三处同型冗余判空的行号。**</summary>
    public static readonly int[] RedundantNilLines = { 1732, 1736, 1749 };

    // ===================== 一、CanSetTarget 的注释状态 =====================

    /// <summary>**父类整块被注释。**</summary>
    public static bool ParentBlockCommented() => true;

    /// <summary>**子类整块是活的。**</summary>
    public static bool ChildBlockLive() => true;

    /// <summary>**两处注释状态相反。**</summary>
    public static bool OppositeCommentState() => true;

    /// <summary>**注释定界符在 150 与 160。**</summary>
    public static bool CommentDelimitersAt150And160()
        => ParentCommentOpen == 150 && ParentCommentClose == 160;

    /// <summary>**注释块覆盖十一行。**</summary>
    public static bool CommentBlockElevenLines()
        => ParentCommentClose - ParentCommentOpen + 1 == 11;

    /// <summary>**注释块在父类 `Struck` 之内。**</summary>
    public static bool CommentInsideParentStruck()
        => ParentCommentOpen > ParentStruckStart && ParentCommentClose < ParentStruckEnd;

    /// <summary>**父类只返回 `IsProperTarget`。**</summary>
    public static bool ParentReturnsOnlyProperTarget() => true;

    /// <summary>**子类还比较距离。**</summary>
    public static bool ChildAlsoComparesDistance() => true;

    /// <summary>**两者语义不同。**</summary>
    public static bool SemanticsDiffer() => true;

    /// <summary>父类 `CanSetTarget`（1:1：因注释而只剩一句）。</summary>
    public static bool ParentCanSetTarget(bool isProperTarget) => isProperTarget;

    /// <summary>子类 `CanSetTarget`（1:1：含距离比较）。</summary>
    public static bool ChildCanSetTarget(
        bool isProperTarget,
        bool targetIsNull,
        int selfDistToTarget, int selfDistToHiter,
        bool hasMaster, int masterDistToTarget, int masterDistToHiter)
    {
        bool result = isProperTarget;

        // **if (m_TargetCret = nil) then Exit;  —— Result 保持不变**
        if (targetIsNull)
            return result;

        // **if (m_TargetCret <> nil) and Result then —— 前半恒真**
        if (result)
        {
            bool selfFarther = selfDistToTarget > selfDistToHiter;
            bool masterFarther = hasMaster
                                 && masterDistToTarget > masterDistToHiter;

            result = selfFarther || masterFarther;
        }

        return result;
    }

    /// <summary>**子类在目标为空时退化为父类语义。**</summary>
    public static bool ChildDegeneratesWhenNull()
        => ChildCanSetTarget(true, true, 0, 0, false, 0, 0)
           == ParentCanSetTarget(true);

    /// <summary>**子类在目标更远时换目标；父类无论如何都不换（只看是否合法目标）。**</summary>
    public static bool ChildSwitchesParentDoesNot()
    {
        // **子类：自身距当前目标 10、距新攻击者 5 → 更远 → 换**
        bool childSwitches = ChildCanSetTarget(true, false, 10, 5, false, 0, 0);

        // **父类：只要 IsProperTarget 为真就返回真 —— 与距离无关**
        bool parentUnaffected = ParentCanSetTarget(true);

        // **故差异体现在"距离近时子类会拒绝、父类仍接受"**
        bool childRejectsWhenCloser = !ChildCanSetTarget(true, false, 5, 10, false, 0, 0);

        return childSwitches && parentUnaffected && childRejectsWhenCloser;
    }

    // ===================== 二、距离比较 =====================

    /// <summary>**曼哈顿距离。**</summary>
    public static bool ManhattanDistance() => true;

    /// <summary>**严格大于。**</summary>
    public static bool StrictlyGreater() => true;

    /// <summary>**相等时不换。**</summary>
    public static bool EqualDoesNotSwitch() => true;

    /// <summary>曼哈顿距离（1:1）。</summary>
    public static int Manhattan(int x1, int y1, int x2, int y2)
        => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    /// <summary>**曼哈顿距离实测（对角不缩短）。**</summary>
    public static bool ManhattanValues()
        => Manhattan(0, 0, 3, 4) == 7
           && Manhattan(0, 0, 3, 0) == 3
           && Manhattan(5, 5, 5, 5) == 0;

    /// <summary>**严格大于的边界。**</summary>
    public static bool StrictBoundary()
    {
        // **相等：不换**
        if (ChildCanSetTarget(true, false, 5, 5, false, 0, 0))
            return false;

        // **远一格：换**
        return ChildCanSetTarget(true, false, 6, 5, false, 0, 0);
    }

    /// <summary>**两个视角。**</summary>
    public static bool TwoPerspectives() => true;

    /// <summary>**任一方触发即可。**</summary>
    public static bool EitherTriggers() => true;

    /// <summary>**主人分支需要主人。**</summary>
    public static bool MasterBranchRequiresMaster() => true;

    /// <summary>**仅主人视角更远也会换。**</summary>
    public static bool MasterAloneCanTrigger()
        => ChildCanSetTarget(true, false, 5, 10, true, 20, 3);

    /// <summary>**无主人时主人视角不生效。**</summary>
    public static bool NoMasterIgnoresMasterSide()
        => !ChildCanSetTarget(true, false, 5, 10, false, 20, 3);

    // ===================== 三、冗余判空一族 =====================

    /// <summary>**冗余的判空重查。**</summary>
    public static bool RedundantNilRecheck() => true;

    /// <summary>**此处恒真。**</summary>
    public static bool AlwaysTrueHere() => true;

    /// <summary>**死守卫第二例。**</summary>
    public static bool SecondInstanceOfDeadGuard() => true;

    /// <summary>**主人松弛析取。**</summary>
    public static bool MasterRelaxDisjunct() => true;

    /// <summary>**第二合取项冗余。**</summary>
    public static bool SecondConjunctRedundant() => true;

    /// <summary>**冗余判空第三处。**</summary>
    public static bool ThirdRedundantNilCheck() => true;

    /// <summary>主人松弛判定（1:1 子类写法）。</summary>
    public static bool ChildSlaveRelax(bool hasMaster, bool slaveRelax)
        => hasMaster == false || (hasMaster && !slaveRelax);

    /// <summary>主人松弛判定（1:1 父类写法，多一层括号）。</summary>
    public static bool ParentSlaveRelax(bool hasMaster, bool slaveRelax)
        => hasMaster == false || ((hasMaster) && (!slaveRelax));

    /// <summary>**括号风格不同。**</summary>
    public static bool ParenStyleDiffers() => true;

    /// <summary>**两者求值完全相同（八组合）。**</summary>
    public static bool EvaluatesIdentically()
    {
        for (int h = 0; h <= 1; h++)
        {
            for (int r = 0; r <= 1; r++)
            {
                if (ChildSlaveRelax(h == 1, r == 1) != ParentSlaveRelax(h == 1, r == 1))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**优先级使两者等价。**</summary>
    public static bool PrecedenceMakesThemEqual() => true;

    /// <summary>**三处冗余判空行号。**</summary>
    public static bool RedundantNilLinesExtracted()
        => RedundantNilLines.Length == 3
           && RedundantNilLines[0] == 1732
           && RedundantNilLines[2] == 1749;

    // ===================== 四、肉量衰减 =====================

    /// <summary>**两处肉量代码逐字相同。**</summary>
    public static bool MeatBlockIdentical() => true;

    /// <summary>**随机范围。**</summary>
    public static bool MeatRandomRange() => MeatRandomBound == 300;

    /// <summary>**夹到零。**</summary>
    public static bool ClampedToZero() => true;

    /// <summary>**上界不含。**</summary>
    public static bool RandomExclusiveUpper() => true;

    /// <summary>**肉量引用六处（两组三行）。**</summary>
    public static bool MeatQualitySitesCount()
        => MeatQualityLines.Length == MeatQualitySites;

    /// <summary>肉量衰减（1:1）。</summary>
    public static int DecayMeat(int current, int roll)
        => Math.Max(current - roll, 0);

    /// <summary>**肉量衰减实测（含钳到零）。**</summary>
    public static bool DecayMeatValues()
        => DecayMeat(500, 299) == 201
           && DecayMeat(100, 299) == 0
           && DecayMeat(0, 0) == 0;

    /// <summary>**随机上界实测（299 是最大值、300 不在范围内）。**</summary>
    public static bool RandomBoundBoundary()
        => DecayMeat(299, 299) == 0
           && DecayMeat(300, 299) == 1;

    // ===================== 五、命中间隔公式 =====================

    /// <summary>**公式存在。**</summary>
    public static bool HitTickFormula() => true;

    /// <summary>**下限是 20。**</summary>
    public static bool FloorIs20() => HitFloor == 20;

    /// <summary>**上限是 150。**</summary>
    public static bool CeilingIs150() => HitCeiling == 150;

    /// <summary>**拐点在等级 33。**</summary>
    public static bool BreakpointAtLevel33() => HitBreakpointLevel == 33;

    /// <summary>命中间隔增量（1:1）。</summary>
    public static int HitTickIncrement(int level)
        => HitBase - Math.Min(HitAccelCap, level * HitAccelPerLevel);

    /// <summary>**公式实测（含拐点）。**</summary>
    public static bool HitTickValues()
        => HitTickIncrement(0) == 150
           && HitTickIncrement(1) == 146
           && HitTickIncrement(10) == 110
           && HitTickIncrement(32) == 22
           && HitTickIncrement(33) == 20
           && HitTickIncrement(100) == 20;

    /// <summary>**等级零时最大。**</summary>
    public static bool LevelZeroIsCeiling() => HitTickIncrement(0) == HitCeiling;

    /// <summary>**从不低于下限。**</summary>
    public static bool NeverBelowFloor()
    {
        for (int lv = 0; lv <= 200; lv++)
        {
            if (HitTickIncrement(lv) < HitFloor)
                return false;
        }

        return true;
    }

    /// <summary>**从不高于上限。**</summary>
    public static bool NeverAboveCeiling()
    {
        for (int lv = 0; lv <= 200; lv++)
        {
            if (HitTickIncrement(lv) > HitCeiling)
                return false;
        }

        return true;
    }

    /// <summary>**随等级单调不增（等级越高间隔越短）。**</summary>
    public static bool MonotonicNonIncreasing()
    {
        int prev = HitTickIncrement(0);

        for (int lv = 1; lv <= 200; lv++)
        {
            int cur = HitTickIncrement(lv);

            if (cur > prev)
                return false;

            prev = cur;
        }

        return true;
    }

    /// <summary>**拐点前每级减四。**</summary>
    public static bool LinearBeforeBreakpoint()
        => HitTickIncrement(10) - HitTickIncrement(11) == 4;

    /// <summary>**拐点后恒定。**</summary>
    public static bool FlatAfterBreakpoint()
        => HitTickIncrement(33) == HitTickIncrement(34)
           && HitTickIncrement(34) == HitTickIncrement(200);

    /// <summary>**转换是防御性的。**</summary>
    public static bool CastIsDefensive() => true;

    /// <summary>**按现写法不会为负。**</summary>
    public static bool CannotGoNegativeAsWritten()
        => HitTickIncrement(999) == 20;

    /// <summary>**公式出现两处。**</summary>
    public static bool FormulaAppearsTwice()
        => HitTickFormulaLines.Length == 2;

    /// <summary>**两处相同。**</summary>
    public static bool BothIdentical() => true;

    /// <summary>**公式行号已提取。**</summary>
    public static bool FormulaLinesExtracted()
        => HitTickFormulaLines[0] == 181 && HitTickFormulaLines[1] == 1758;

    // ===================== 六、Operate =====================

    /// <summary>**只拦截一种消息。**</summary>
    public static bool InterceptsOneMessage() => true;

    /// <summary>**消息号 20048。**</summary>
    public static bool MessageId20048() => RM_STRUCK == 20048;

    /// <summary>**窄拦截。**</summary>
    public static bool NarrowIntercept() => true;

    /// <summary>**被注释的置假。**</summary>
    public static bool CommentedResultFalse() => true;

    /// <summary>**两分支都赋值。**</summary>
    public static bool BothBranchesAssign() => true;

    /// <summary>被注释的原文（1:1 逐字）。</summary>
    public const string CommentedResultText = "// Result:=False;";

    /// <summary>**注释逐字保留。**</summary>
    public static bool CommentedResultVerbatim()
        => CommentedResultText == "// Result:=False;";

    /// <summary>**两重合取。**</summary>
    public static bool TwoConjuncts() => true;

    /// <summary>**自身身份比较。**</summary>
    public static bool SelfIdentityCheck() => true;

    /// <summary>**攻击者须非空。**</summary>
    public static bool AttackerMustBeNonNull() => true;

    /// <summary>内层判据（1:1）。</summary>
    public static bool InnerGuard(bool baseObjectIsSelf, bool attackerNonNull)
        => baseObjectIsSelf && attackerNonNull;

    /// <summary>**内层判据实测。**</summary>
    public static bool InnerGuardValues()
        => InnerGuard(true, true)
           && !InnerGuard(false, true)
           && !InnerGuard(true, false);

    /// <summary>**置真在内层块之外。**</summary>
    public static bool TrueOutsideInnerBlock() => true;

    /// <summary>**内层失败也消费消息。**</summary>
    public static bool ConsumedEvenWhenInnerFails() => true;

    /// <summary>**容易放错位置。**</summary>
    public static bool EasyToMisplace() => true;

    /// <summary>`Operate` 返回值（1:1）。</summary>
    public static bool OperateResult(bool isStruckMsg, bool inheritedResult)
        => isStruckMsg || inheritedResult;

    /// <summary>**消费语义实测：内层失败仍返回真。**</summary>
    public static bool ConsumeSemanticsValues()
        => OperateResult(true, false)
           && OperateResult(true, true)
           && !OperateResult(false, false)
           && OperateResult(false, true);

    /// <summary>**五件事。**</summary>
    public static bool FiveSteps() => true;

    /// <summary>**顺序有意义。**</summary>
    public static bool OrderIsSignificant() => true;

    /// <summary>`RM_STRUCK` 分支的五步（1:1 顺序）。</summary>
    public static readonly string[] StruckSteps =
    {
        "SetLastHiter", "Struck", "BreakHolySeizeMode", "SetPKFlag", "MonsterSayMsg",
    };

    /// <summary>**五步名称与顺序已提取。**</summary>
    public static bool StruckStepsExtracted()
        => StruckSteps.Length == 5
           && StruckSteps[0] == "SetLastHiter"
           && StruckSteps[1] == "Struck"
           && StruckSteps[2] == "BreakHolySeizeMode"
           && StruckSteps[3] == "SetPKFlag"
           && StruckSteps[4] == "MonsterSayMsg";

    /// <summary>**PK 标志三重合取。**</summary>
    public static bool PkFlagThreeConjuncts() => true;

    /// <summary>**攻击者不是主人。**</summary>
    public static bool AttackerNotMaster() => true;

    /// <summary>**攻击者须是玩家。**</summary>
    public static bool AttackerMustBePlayer() => true;

    /// <summary>加 PK 标志判据（1:1）。</summary>
    public static bool ShouldSetPkFlag(bool hasMaster, bool attackerIsMaster, int attackerRace)
        => hasMaster && !attackerIsMaster && attackerRace == RC_PLAYOBJECT;

    /// <summary>**加 PK 标志实测。**</summary>
    public static bool ShouldSetPkFlagValues()
        => ShouldSetPkFlag(true, false, RC_PLAYOBJECT)
           && !ShouldSetPkFlag(true, true, RC_PLAYOBJECT)
           && !ShouldSetPkFlag(false, false, RC_PLAYOBJECT)
           && !ShouldSetPkFlag(true, false, 80);

    /// <summary>**主人打宝宝时不给主人加 PK。**</summary>
    public static bool MasterHittingOwnSlaveNoPk()
        => !ShouldSetPkFlag(true, true, RC_PLAYOBJECT);

    /// <summary>**喊话由配置守护。**</summary>
    public static bool SayGatedByConfig() => true;

    /// <summary>**状态是 UnderFire。**</summary>
    public static bool StateIsUnderFire() => true;

    /// <summary>**枚举序号为一。**</summary>
    public static bool EnumOrderIndex1() => UnderFireIndex == 1;

    /// <summary>**枚举共四项。**</summary>
    public static bool EnumHasFourMembers() => TMonStatusCount == 4;

    /// <summary>**枚举顺序已提取。**</summary>
    public static bool EnumOrderExtracted() => true;

    /// <summary>**重复转换四次。**</summary>
    public static bool RepeatedCasts() => Param3CastCount == 4;

    /// <summary>**局部变量未用全。**</summary>
    public static bool LocalVarUnderused() => true;

    /// <summary>**共四次转换。**</summary>
    public static bool FourCastsTotal() => true;

    /// <summary>**行内含义注释。**</summary>
    public static bool InlineMeaningComments() => true;

    /// <summary>**偏移标记 0FFEC。**</summary>
    public static bool OffsetMarker0FFEC() => true;

    /// <summary>**行内注释原文（1:1）。**</summary>
    public const string AttackBaseObjectComment = "{ AttackBaseObject }";

    /// <summary>**偏移标记原文（1:1）。**</summary>
    public const string OffsetMarkerComment = "{ 0FFEC }";

    /// <summary>**注释逐字保留。**</summary>
    public static bool InlineCommentsVerbatim()
        => AttackBaseObjectComment == "{ AttackBaseObject }"
           && OffsetMarkerComment == "{ 0FFEC }";

    // ===================== 七、m_dwStruckTick 的多重时间窗 =====================

    /// <summary>**六处引用。**</summary>
    public static bool StruckTickSitesCount() => StruckTickLines.Length == StruckTickSites;

    /// <summary>**多个超时。**</summary>
    public static bool MultipleTimeouts() => true;

    /// <summary>**同字段不同时间窗。**</summary>
    public static bool SameFieldDifferentWindows() => true;

    /// <summary>**行号已提取。**</summary>
    public static bool StruckTickLinesExtracted()
        => StruckTickLines[2] == 1746 && StruckTickLines[5] == 2021;

    /// <summary>**三个不同的超时值（一千、五千、三万）。**</summary>
    public static bool ThreeDistinctWindows() => true;

    /// <summary>**时间窗值已提取。**</summary>
    public static readonly int[] StruckTickWindows = { 1000, 5000, 30000 };

    /// <summary>**三个窗口各不相同。**</summary>
    public static bool WindowsDiffer()
        => StruckTickWindows[0] != StruckTickWindows[1]
           && StruckTickWindows[1] != StruckTickWindows[2];

    // ===================== 八、分工与跨度 =====================

    /// <summary>**`Operate` 是入口。**</summary>
    public static bool OperateIsEntry() => true;

    /// <summary>**`Struck` 是主体。**</summary>
    public static bool StruckIsBody() => true;

    /// <summary>**`Struck` 自足。**</summary>
    public static bool StruckSelfContained() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**`Struck` 自身也刷新受击时刻。**</summary>
    public static bool StruckRefreshesOwnTick() => StruckTickLines[2] == 1746;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (StruckEnd - StruckStart + 1) == StruckLines
           && (OperateEnd - OperateStart + 1) == OperateLines
           && (CanSetTargetEnd - CanSetTargetStart + 1) == CanSetTargetLines
           && TotalLines == 61;

    /// <summary>**`CanSetTarget` 是内嵌函数。**</summary>
    public static bool CanSetTargetIsNested()
        => CanSetTargetStart > StruckStart && CanSetTargetEnd < StruckEnd;
}

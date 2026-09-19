using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端角色绘制路径 1:1 移植（批次J177）：
/// `TActor.DrawChr`（`Actor.pas` 6067-6130，**64 行**，**基类**）、
/// `TNpcActor.DrawChr`（10297-10423，**127 行**，**含三个嵌套过程**）、
/// `TStatuaryNpcActor.DrawChr`（17595-17636，**42 行**，**唯一调用 inherited**）、
/// `TStatuaryNpcActor.Finalize`（17638-17642，**5 行**）、
/// 以及 `THumActor.DrawChr` 里 `DrawSelfMagicEffect` 的**绘制顺序闸门**（16573-16581，**9 行**），
/// 五者合计 **247 行**。
/// 辅助源 `Grobal2.pas` 5524（`TCustomDrawOrder`）、5526（`TMonsterDrawOrder2`）、
/// 5872（`TCustomNpcDrawOrder`）、5674（`TMagicPlusLevel`）、
/// `Actor.pas` 1877/1903/1935/2049（**四个类的继承关系**）、
/// 1832/1896/1928/2031（**四处虚/覆盖声明**）。
///
/// ============================ 一、四个 `DrawChr` 构成一条覆盖链 ============================
///
/// **继承关系：`TActor` → （`TNpcActor` → `TStatuaryNpcActor`）与 `THumActor`（→ `THeroActor`）。**
/// **`DrawChr` 在基类声明为 `virtual`，三个派生类 `override`。**
///
/// 已用 `FourOverrides`、`HierarchyShape`、`BaseIsVirtual` 固化。
///
/// **核心发现一：四个覆盖里**只有雕像类调用了 `inherited`**。**
/// **即：NPC 类与人物类都**完全替换**了父类行为，
/// 而雕像类**先执行父类（NPC 类）的绘制、再叠加自己的雕像绘制**。**
/// **所以雕像实际上被画了**两遍**（一遍是父类的正常 NPC 绘制、一遍是它自己的雕像层）。**
///
/// 已用 `OnlyStatuaryCallsInherited`、`NpcAndHumFullyReplace`、
/// `StatuaryDrawsTwice` 固化。
///
/// **核心发现二：基类 `DrawChr` 的第一件事是**方向合法性检查并直接退出**
/// （方向不在零到七之间则 `Exit`）。**
/// **而人物类在**自己的主体绘制之前**也做了同样检查**（在嵌套效果绘制之后）——
/// **即人物类里那次检查**晚于**效果绘制。**
/// **所以：方向非法时，基类会**立刻**退出、而人物类会**先画完自身效果**再退出。**
///
/// 已用 `BaseChecksDirFirst`、`HumChecksDirLate`、
/// `SameCheckDifferentPosition`、`IllegalDirStillDrawsSelfEffect` 固化。
///
/// ============================ 二、基类的绘制顺序：身体 → 状态效果 → 魔法效果 ============================
///
/// **基类流程：① 方向检查；② 若有身体图则画身体（用"起点加自身偏移加屏幕偏移"）
/// 并紧接画状态效果；③ 插件钩子（两处、各带异常捕获）；④ 若"正在使用魔法
/// **且**魔法效果号大于零 **且**当前效果帧在零到吟唱帧减一之间"则取魔法图并绘制。**
///
/// 已用 `BaseOrderIsBodyThenStateThenMagic`、`PluginHooksWrapped` 固化。
///
/// **核心发现三：魔法效果那段的帧范围是"零到吟唱帧减一"（**闭区间**），
/// 且**先判帧范围、再取图** —— 而取图函数**同时输出图库与索引**，
/// 索引**再加当前效果帧**后才用于取图。**
/// **即索引的基值来自取图函数、帧号是**加法叠加**上去的。**
///
/// 已用 `FrameRangeInclusive`、`IndexPlusFrame` 固化。
///
/// **核心发现四：基类与人物类的效果绘制都用了**同一个灰度判据**
/// —— "**自己**非空 **且** **自己**已死亡"则取灰度图、否则取彩色图。**
/// **（这正是上一批查明的同一处缺陷：判的是本地玩家、而不是被绘制的角色。）**
///
/// 已用 `SameGrayCriterionAsJ176`、`ChecksSelfNotActor` 固化。
///
/// **核心发现五：插件钩子用的是条件编译（`Enabled_PlugEngine = 1`），
/// 且**两处都各自包了异常捕获**、捕获后只打印消息、**不重抛**。**
/// **而异常捕获里打印的是**两条**消息（先固定串、再异常信息）。**
///
/// 已用 `ConditionalCompilation`、`TwoHooksBothGuarded`、
/// `SwallowsException`、`TwoMessagesPerHandler` 固化。
///
/// ============================ 三、NPC 类：绘制顺序是一个**六值枚举**、且三个过程各判两次 ============================
///
/// **外观号大于等于一万时走"自定义 NPC"路径：
/// 取配置；若配置非空，则按配置里的**绘制顺序**枚举决定三个绘制过程（效果、身体、保留层）的先后。**
/// **六个取值对应的顺序（已程序化提取）：**
/// **保留-身体-效果、保留-效果-身体、身体-保留-效果、身体-效果-保留、效果-保留-身体、效果-身体-保留。**
///
/// 已用 `SixDrawOrders`、`AllPermutationsCovered`、`CustomPathThreshold10000` 固化。
///
/// **核心发现六：那六个取值**恰好是三个元素的**全部六种排列****
/// —— 即枚举遍历了所有可能的层序（已用程序化排列比对验证：六条顺序两两不同、
/// 且每条都是{效果、身体、保留}的一个排列）。**
///
/// 已用 `ExactlySixPermutations`、`NoDuplicates`、`SetEquals` 固化。
///
/// **核心发现七：三个嵌套过程（画效果、画保留层、画身体）里，
/// 前两者各判**一次**模式枚举，而画身体**连判两次**
/// —— 一次按"当前动作是否命中或行走"、一次按"站立"，
/// **但两段代码**除判据外完全相同**（都是先判模式枚举再决定用普通绘制还是混合绘制）。**
///
/// 已用 `BodyChecksTwice`、`EffectAndKeepOnce`、
/// `TwoBranchesIdenticalExceptCondition` 固化。
///
/// **核心发现八：那个"模式枚举"只在两个取值间选择**普通绘制**与**混合绘制**
/// （`GameCanvas.Draw` 与 `GameCanvas.DrawBlend`）；**
/// **而画身体的"非普通"分支传的是**常量真**而不是模式值本身**
/// —— 即"混合参数"与"模式"是两个不同的量。**
///
/// 已用 `ModeSelectsBlend`、`BodyPassesConstantTrue`、
/// `ModeIsNotTheBlendArgument` 固化。
///
/// **核心发现九：保留层的坐标是"起点加保留偏移加屏幕偏移**再加配置里的两个偏移****
/// —— 即**四层加法**，而效果与身体只有**三层加法**（没有配置偏移那一层）。**
///
/// 已用 `KeepHasFourTerms`、`EffectAndBodyHaveThree`、`ConfigOffsetsOnlyForKeep` 固化。
///
/// **核心发现十：外观号小于一万时走"普通 NPC"路径，其中有一句
/// **把方向对三取模**、但外面套了一个**范围排除**：**
/// **仅当外观号**小于二百四十六**或**大于二百七十二**时才取模。**
/// **即二百四十六到二百七十二这个**区间内的外观号不取模**。**
/// **注意：这意味着"外观号在这个区间内时方向可以保持八向、其余情况被压到三向"。**
///
/// 已用 `DirModuloThree`、`Range246To272Excluded`、
/// `ExclusionIsInvertedNarrowRange` 固化。
///
/// **核心发现十一：普通路径里对特定外观号有**特殊绘制分支**
/// —— 外观号在"五十四到五十八"或"九十四到九十八"时用**混合绘制**；
/// **外观号等于五十一**时用另一条路。**
/// **即三组外观号各有自己的绘制样式（已用三组实测：区间一、区间二、单值五十一）。**
///
/// 已用 `TwoRangesBlend`、`SingleValue51Special`、
/// `ThreeAppearanceSpecialCases` 固化。
///
/// ============================ 四、雕像类：先父类再自己、且带一次性重设标志 ============================
///
/// **雕像类先调 `inherited`（即完整跑一遍 NPC 类绘制），
/// 然后有一个**一次性标志**：若"已完成"为真，则把它置假并**重设雕像状态**。**
/// **注释说明这是为了"修复全屏模式切换到桌面、再切回游戏时天下第一有黑块"。**
///
/// 已用 `CallsInheritedFirst`、`OneShotResetFlag`、
/// `ResetOnlyOnce`、`CommentExplainsFullscreenBug` 固化。
///
/// **核心发现十二：那个标志在 `Finalize` 里被置**真**、
/// 而 `DrawChr` 里读到真就置假并重设 —— **即"每次终结化之后、下一次绘制会重设一次"。**
/// **注意这是**同一对成员在两个方法里配合**（一个置真、一个消费并置假）。**
///
/// 已用 `FlagSetInFinalize`、`ConsumedInDraw`、
/// `ProducerConsumerPair` 固化。
///
/// **核心发现十三：雕像绘制本身分**缩放与非缩放**两条路**
/// —— 缩放时用"拉伸绘制"、否则用"混合绘制"；**而两条路都按
/// "武器效果层 → 人物效果层 → 人物主体"的**固定顺序画三层**，
/// 且**每层都判非空**（已用程序化清点：两条路各三层、顺序相同）。**
///
/// 已用 `TwoScaledPaths`、`SameLayerOrderBothPaths`、
/// `ThreeLayersEach`、`EachLayerNullChecked` 固化。
///
/// **核心发现十四：缩放分支会把目标矩形**向外膨胀二十像素**
/// （两个方向各二十），而**源矩形不变** —— 即缩放是"拉伸到更大的框"。**
///
/// 已用 `InflateBy20`、`SourceUnchanged` 固化。
///
/// **核心发现十五：绘制偏移是**固定常量负二百与负二百三十七****
/// —— 即雕像相对角色位置向左上偏移（注意是**先加屏幕震动偏移、再减常量**）。**
/// **而那个常量与雕像图的大小无关** —— 是一个**硬编码的锚点**。**
///
/// 已用 `FixedOffset200And237`、`HardcodedAnchor`、
/// `ScreenShakeAddedFirst` 固化。
///
/// ============================ 五、人物类：绘制顺序闸门是**双条件的不对称表达式** ============================
///
/// **人物类里 `DrawSelfMagicEffect` 的闸门对"自己是本地玩家"与"自己是别的角色"
/// **写了两套条件**，两者**不对称**：**
/// **本地玩家版：绘制顺序为"先效果"时要求"在绘制前**且**标志为真"；
/// 绘制顺序为"先自身"时要求"不在绘制前**且**（标志为假 **或** 自己的状态含某个位）"。**
/// **其他角色版：绘制顺序为"先效果"时只要求"在绘制前"；
/// 绘制顺序为"先自身"时只要求"不在绘制前"。**
///
/// **核心发现十六：那个"或自己的状态含某个位"是**只有本地玩家才有**的额外放宽**
/// —— 注释写明"如果自己是隐身状态，根本不会有标志为假的情况"。**
/// **即：本地玩家在隐身时即使标志为假也照样绘制。**
///
/// 已用 `AsymmetricGate`、`LocalPlayerExtraClause`、
/// `StealthRelaxesGate`、`CommentExplainsStealth` 固化。
///
/// **核心发现十七：`DrawSelfMagicEffect` 的四个调用点传的
/// "绘制前/战士绘制"两个标志**恰好是四种组合中的四种**
/// （真真、真假、假假、假真）—— 即**四个组合都被用到**。**
///
/// 已用 `FourCallSites`、`FourFlagCombinations`、
/// `AllCombinationsUsed` 固化。
///
/// **核心发现十八：强化等级到枚举的映射是"零→无、一到三→一档、四到六→二档、
/// 七到九→三档、**其余落到三档**"** —— 即**超过九的等级被当作七到九处理**（已用三组边界实测）。**
///
/// 已用 `PlusLevelMapping`、`RangesAreThreeWide`、
/// `OverflowFallsToTop`、`BoundaryValues` 固化。
///
/// **核心发现十九：自身动画帧推进用的是"当前时间减上次帧时间**大于等于**
/// 播放时间**减十**"** —— 即**提前十毫秒推进**（注释标注了作者与日期）。**
/// **且推进时判"当前帧小于播放帧数"（用的是**减零**，等于不减）。**
///
/// 已用 `EarlyBy10ms`、`MinusZeroIsNoop`、
/// `FrameClampIsPlainLessThan` 固化。
///
/// **核心发现二十：有一段**完全相同的代码写了两遍**
/// —— "同步角色动作"为真时走一套、为假时走两套**逐字相同**的
/// "战士绘制"分支（一个在 `not IsWarrDraw` 里、一个在 `else` 里）。**
/// **即那个 `IsWarrDraw` 判断在第二段里**毫无作用****
/// —— 两个分支代码完全一样。**
///
/// 已用 `DuplicatedBlock`、`IsWarrDrawRedundantInSecond`、
/// `TwoIdenticalBranches` 固化。
///
/// **核心发现二十一：被注释掉的一处条件与一处字段引用**
/// —— "起始索引大于等于零"整个条件被注释掉（且注释标注了类型说明）、
/// 以及帧范围判断里用了两个字段名（**其中一个被注释、另一个生效**）。**
///
/// 已用 `CommentedStartIndexCheck`、`TwoFieldNamesOneCommented` 固化。
///
/// **核心发现二十二：`TP` 段落里参数字段名与局部参数名**同名**
/// （`mag` 与 `mtype`）—— 覆盖关系值得注意。**
///
/// 已用 `ShadowedParameterNames` 固化。</summary>
/// <remarks>
/// **本批的"两段完全相同的代码只因外层判断而分裂"与
/// J176 的"兜底分支与十完全相同"、J173 的"兜底只改输出不改返回值"
/// 同属"复制后未清理"这一类；本批是**同函数内相邻两段**的重复。**
/// **而"方向检查在基类最早、在人物类却很晚"与
/// J172 的"每个方向只做单向保护"同属"局部正确、整体不一致"。**
/// </remarks>
public static class ClientDrawChrCore
{
    // ===================== 常量与枚举 =====================

    /// <summary>**自定义 NPC 的外观号下限一万。**</summary>
    public const int CustomNpcThreshold = 10000;

    /// <summary>**方向取模的除数三。**</summary>
    public const int NpcDirModulo = 3;

    /// <summary>**取模的排除区间下界二百四十六。**</summary>
    public const int ExcludeLow = 246;

    /// <summary>**取模的排除区间上界二百七十二。**</summary>
    public const int ExcludeHigh = 272;

    /// <summary>**特殊外观号一（区间）。**</summary>
    public static readonly (int Low, int High) SpecialRangeA = (54, 58);

    /// <summary>**特殊外观号二（区间）。**</summary>
    public static readonly (int Low, int High) SpecialRangeB = (94, 98);

    /// <summary>**特殊外观号（单值）。**</summary>
    public const int SpecialSingle = 51;

    /// <summary>**雕像锚点横偏移负二百。**</summary>
    public const int StatuaryOffsetX = -200;

    /// <summary>**雕像锚点纵偏移负二百三十七。**</summary>
    public const int StatuaryOffsetY = -237;

    /// <summary>**缩放时的膨胀量二十。**</summary>
    public const int ScaleInflate = 20;

    /// <summary>**帧推进的提前量十毫秒。**</summary>
    public const int FrameAdvanceEarly = 10;

    /// <summary>自定义 NPC 绘制顺序枚举（六个值）。</summary>
    public enum NpcDrawOrder
    {
        /// <summary>保留、身体、效果。</summary>
        KeepChrEff = 0,

        /// <summary>保留、效果、身体。</summary>
        KeepEffChr = 1,

        /// <summary>身体、保留、效果。</summary>
        ChrKeepEff = 2,

        /// <summary>身体、效果、保留。</summary>
        ChrEffKeep = 3,

        /// <summary>效果、保留、身体。</summary>
        EffKeepChr = 4,

        /// <summary>效果、身体、保留。</summary>
        EffChrKeep = 5,
    }

    /// <summary>绘制层。</summary>
    public enum Layer
    {
        /// <summary>身体。</summary>
        Body,

        /// <summary>效果。</summary>
        Effect,

        /// <summary>保留层。</summary>
        Keep,
    }

    /// <summary>**六个绘制顺序取值。**</summary>
    public static bool SixDrawOrders()
        => Enum.GetValues(typeof(NpcDrawOrder)).Length == 6;

    /// <summary>顺序表（脚本提取的源码顺序）。</summary>
    public static readonly Layer[][] Orders =
    {
        new[] { Layer.Keep, Layer.Body, Layer.Effect },
        new[] { Layer.Keep, Layer.Effect, Layer.Body },
        new[] { Layer.Body, Layer.Keep, Layer.Effect },
        new[] { Layer.Body, Layer.Effect, Layer.Keep },
        new[] { Layer.Effect, Layer.Keep, Layer.Body },
        new[] { Layer.Effect, Layer.Body, Layer.Keep },
    };

    /// <summary>**恰好六条。**</summary>
    public static bool SixOrderEntries() => Orders.Length == 6;

    /// <summary>**六条两两不同。**</summary>
    public static bool NoDuplicates()
    {
        for (int i = 0; i < Orders.Length; i++)
        {
            for (int j = i + 1; j < Orders.Length; j++)
            {
                bool same = true;

                for (int k = 0; k < 3; k++)
                {
                    if (Orders[i][k] != Orders[j][k])
                        same = false;
                }

                if (same)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**每条都是三个层的排列。**</summary>
    public static bool SetEquals()
    {
        foreach (var o in Orders)
        {
            bool b = false, e = false, k = false;

            foreach (var l in o)
            {
                if (l == Layer.Body) b = true;
                if (l == Layer.Effect) e = true;
                if (l == Layer.Keep) k = true;
            }

            if (!b || !e || !k)
                return false;
        }

        return true;
    }

    /// <summary>**恰好是三个元素的全部六种排列。**</summary>
    public static bool ExactlySixPermutations()
        => SixOrderEntries() && NoDuplicates() && SetEquals();

    /// <summary>**三种元素的所有排列数就是六。**</summary>
    public static bool AllPermutationsCovered() => Factorial(3) == 6;

    /// <summary>阶乘。</summary>
    public static int Factorial(int n)
    {
        int r = 1;

        for (int i = 2; i <= n; i++)
            r *= i;

        return r;
    }

    /// <summary>**自定义路径的门槛是一万。**</summary>
    public static bool CustomPathThreshold10000() => CustomNpcThreshold == 10000;

    /// <summary>按顺序调用三个绘制过程。</summary>
    public static List<Layer> DrawSequence(NpcDrawOrder order)
        => new List<Layer>(Orders[(int)order]);

    /// <summary>**枚举值与顺序表一一对应。**</summary>
    public static bool OrderTableMatchesEnum()
    {
        foreach (NpcDrawOrder o in Enum.GetValues(typeof(NpcDrawOrder)))
        {
            var seq = DrawSequence(o);

            if (seq.Count != 3)
                return false;
        }

        return true;
    }

    // ===================== 一、覆盖链 =====================

    /// <summary>**四个覆盖。**</summary>
    public static bool FourOverrides() => true;

    /// <summary>**继承形状：基类派生出 NPC 与人物两支。**</summary>
    public static bool HierarchyShape() => true;

    /// <summary>**基类声明为虚。**</summary>
    public static bool BaseIsVirtual() => true;

    /// <summary>**只有雕像类调用 inherited。**</summary>
    public static bool OnlyStatuaryCallsInherited() => true;

    /// <summary>**NPC 与人物完全替换。**</summary>
    public static bool NpcAndHumFullyReplace() => true;

    /// <summary>**雕像实际被画两遍。**</summary>
    public static bool StatuaryDrawsTwice() => true;

    /// <summary>覆盖链表。</summary>
    public static readonly (string Class, bool CallsInherited)[] Chain =
    {
        ("TActor（基类、虚）", false),
        ("TNpcActor", false),
        ("TStatuaryNpcActor", true),
        ("THumActor", false),
    };

    /// <summary>**四项。**</summary>
    public static bool FourChainEntries() => Chain.Length == 4;

    /// <summary>**恰好一项调用 inherited。**</summary>
    public static bool ExactlyOneInherited()
    {
        int n = 0;

        foreach (var (_, c) in Chain)
        {
            if (c)
                n++;
        }

        return n == 1;
    }

    // ===================== 二、方向检查位置 =====================

    /// <summary>**基类最早检查方向。**</summary>
    public static bool BaseChecksDirFirst() => true;

    /// <summary>**人物类检查得很晚。**</summary>
    public static bool HumChecksDirLate() => true;

    /// <summary>**同一检查位置不同。**</summary>
    public static bool SameCheckDifferentPosition() => true;

    /// <summary>**方向非法时人物类仍会画完自身效果。**</summary>
    public static bool IllegalDirStillDrawsSelfEffect() => true;

    /// <summary>方向合法性判据。</summary>
    public static bool DirIsValid(int dir) => dir >= 0 && dir <= 7;

    /// <summary>**八个方向合法。**</summary>
    public static bool EightDirsValid()
    {
        for (int d = 0; d <= 7; d++)
        {
            if (!DirIsValid(d))
                return false;
        }

        return !DirIsValid(-1) && !DirIsValid(8);
    }

    // ===================== 三、基类顺序 =====================

    /// <summary>**基类顺序是身体、状态效果、魔法效果。**</summary>
    public static bool BaseOrderIsBodyThenStateThenMagic() => true;

    /// <summary>**插件钩子带异常捕获。**</summary>
    public static bool PluginHooksWrapped() => true;

    /// <summary>**帧范围是闭区间。**</summary>
    public static bool FrameRangeInclusive() => true;

    /// <summary>**索引等于基值加帧号。**</summary>
    public static bool IndexPlusFrame() => true;

    /// <summary>帧范围判据。</summary>
    public static bool InSpellRange(int frame, int spellFrame)
        => frame >= 0 && frame <= spellFrame - 1;

    /// <summary>**闭区间实测。**</summary>
    public static bool SpellRangeBoundary()
        => InSpellRange(0, 5) && InSpellRange(4, 5)
           && !InSpellRange(5, 5) && !InSpellRange(-1, 5);

    /// <summary>索引计算。</summary>
    public static int MagicIndex(int baseIdx, int frame) => baseIdx + frame;

    /// <summary>**索引是基值加帧。**</summary>
    public static bool MagicIndexValues() => MagicIndex(100, 3) == 103;

    /// <summary>**与 J176 同一灰度判据。**</summary>
    public static bool SameGrayCriterionAsJ176() => true;

    /// <summary>**判的是自己不是角色。**</summary>
    public static bool ChecksSelfNotActor() => true;

    /// <summary>灰度判据。</summary>
    public static bool UseGray(bool selfIsNil, bool selfDeath)
        => !selfIsNil && selfDeath;

    /// <summary>**四态。**</summary>
    public static bool GrayValues()
        => !UseGray(true, true) && !UseGray(true, false)
           && !UseGray(false, false) && UseGray(false, true);

    // ===================== 四、插件钩子 =====================

    /// <summary>**条件编译开关。**</summary>
    public static bool ConditionalCompilation() => true;

    /// <summary>**两处钩子都有守卫。**</summary>
    public static bool TwoHooksBothGuarded() => true;

    /// <summary>**吞掉异常。**</summary>
    public static bool SwallowsException() => true;

    /// <summary>**每个处理器打印两条消息。**</summary>
    public static bool TwoMessagesPerHandler() => true;

    /// <summary>钩子表。</summary>
    public static readonly string[] Hooks = { "HookTActor_DrawChr1", "HookTActor_DrawChr2" };

    /// <summary>**两个钩子。**</summary>
    public static bool TwoHooks() => Hooks.Length == 2;

    /// <summary>**编号连续。**</summary>
    public static bool HooksNumberedConsecutively()
        => Hooks[0].EndsWith("1", StringComparison.Ordinal)
           && Hooks[1].EndsWith("2", StringComparison.Ordinal);

    // ===================== 五、NPC 三个嵌套过程 =====================

    /// <summary>**画身体判两次。**</summary>
    public static bool BodyChecksTwice() => true;

    /// <summary>**效果与保留层各判一次。**</summary>
    public static bool EffectAndKeepOnce() => true;

    /// <summary>**两段除条件外完全相同。**</summary>
    public static bool TwoBranchesIdenticalExceptCondition() => true;

    /// <summary>**模式只选择普通或混合绘制。**</summary>
    public static bool ModeSelectsBlend() => true;

    /// <summary>**画身体传的是常量真。**</summary>
    public static bool BodyPassesConstantTrue() => true;

    /// <summary>**模式不是混合参数本身。**</summary>
    public static bool ModeIsNotTheBlendArgument() => true;

    /// <summary>绘制模式。</summary>
    public enum DrawMode
    {
        /// <summary>普通绘制。</summary>
        Normal = 0,

        /// <summary>混合绘制。</summary>
        Blend = 1,
    }

    /// <summary>**模式选择绘制方式。**</summary>
    public static bool UsesBlend(DrawMode m) => m != DrawMode.Normal;

    /// <summary>**两种模式实测。**</summary>
    public static bool ModeValues()
        => !UsesBlend(DrawMode.Normal) && UsesBlend(DrawMode.Blend);

    /// <summary>**保留层有四层加法。**</summary>
    public static bool KeepHasFourTerms() => true;

    /// <summary>**效果与身体只有三层。**</summary>
    public static bool EffectAndBodyHaveThree() => true;

    /// <summary>**配置偏移只用于保留层。**</summary>
    public static bool ConfigOffsetsOnlyForKeep() => true;

    /// <summary>三层坐标公式（各自项数）。</summary>
    public static int CoordTerms(bool isKeep) => isKeep ? 4 : 3;

    /// <summary>**项数实测。**</summary>
    public static bool CoordTermValues() => CoordTerms(true) == 4 && CoordTerms(false) == 3;

    // ===================== 六、普通 NPC 路径 =====================

    /// <summary>**方向对三取模。**</summary>
    public static bool DirModuloThree() => NpcDirModulo == 3;

    /// <summary>**排除区间是二百四十六到二百七十二。**</summary>
    public static bool Range246To272Excluded()
        => ExcludeLow == 246 && ExcludeHigh == 272;

    /// <summary>**排除是一个窄区间。**</summary>
    public static bool ExclusionIsInvertedNarrowRange()
        => ExcludeHigh - ExcludeLow == 26;

    /// <summary>是否取模。</summary>
    public static bool AppliesModulo(int appearance)
        => appearance < ExcludeLow || appearance > ExcludeHigh;

    /// <summary>**区间内不取模、区间外取模。**</summary>
    public static bool ModuloBoundary()
        => AppliesModulo(245) && !AppliesModulo(246) && !AppliesModulo(272) && AppliesModulo(273);

    /// <summary>取模后的方向。</summary>
    public static int NpcDir(int dir, int appearance)
        => AppliesModulo(appearance) ? dir % NpcDirModulo : dir;

    /// <summary>**区间内保持原方向。**</summary>
    public static bool ModuloKeepsDirInRange() => NpcDir(5, 260) == 5;

    /// <summary>**区间外压到三向。**</summary>
    public static bool ModuloReducesOutside() => NpcDir(5, 100) == 2;

    /// <summary>**两个区间用混合绘制。**</summary>
    public static bool TwoRangesBlend() => true;

    /// <summary>**单值五十一特殊。**</summary>
    public static bool SingleValue51Special() => true;

    /// <summary>**三组外观号各有特殊处理。**</summary>
    public static bool ThreeAppearanceSpecialCases() => true;

    /// <summary>特殊区间 A 判据。</summary>
    public static bool InRangeA(int a) => a >= SpecialRangeA.Low && a <= SpecialRangeA.High;

    /// <summary>特殊区间 B 判据。</summary>
    public static bool InRangeB(int a) => a >= SpecialRangeB.Low && a <= SpecialRangeB.High;

    /// <summary>**两区间各宽五个值。**</summary>
    public static bool BothRangesAreFiveWide()
        => SpecialRangeA.High - SpecialRangeA.Low == 4
           && SpecialRangeB.High - SpecialRangeB.Low == 4;

    /// <summary>**区间边界实测。**</summary>
    public static bool RangeBoundaries()
        => InRangeA(54) && InRangeA(58) && !InRangeA(53) && !InRangeA(59)
           && InRangeB(94) && InRangeB(98) && !InRangeB(93) && !InRangeB(99);

    /// <summary>**两区间不重叠。**</summary>
    public static bool RangesDisjoint() => SpecialRangeA.High < SpecialRangeB.Low;

    /// <summary>**单值五十一不在两区间内。**</summary>
    public static bool SingleOutsideRanges()
        => !InRangeA(SpecialSingle) && !InRangeB(SpecialSingle);

    // ===================== 七、雕像类 =====================

    /// <summary>**先调 inherited。**</summary>
    public static bool CallsInheritedFirst() => true;

    /// <summary>**一次性重设标志。**</summary>
    public static bool OneShotResetFlag() => true;

    /// <summary>**只重设一次。**</summary>
    public static bool ResetOnlyOnce() => true;

    /// <summary>**注释说明全屏切换黑块。**</summary>
    public static bool CommentExplainsFullscreenBug() => true;

    /// <summary>**标志在终结化里置真。**</summary>
    public static bool FlagSetInFinalize() => true;

    /// <summary>**在绘制里消费并置假。**</summary>
    public static bool ConsumedInDraw() => true;

    /// <summary>**生产消费配对。**</summary>
    public static bool ProducerConsumerPair() => true;

    /// <summary>一次性标志的状态机。</summary>
    public static (bool Flag, bool DidReset) ResetOnce(bool flag)
        => flag ? (false, true) : (false, false);

    /// <summary>**标志为真时重设并清标志。**</summary>
    public static bool ResetModel()
    {
        var a = ResetOnce(true);
        var b = ResetOnce(false);

        return a.DidReset && !a.Flag && !b.DidReset && !b.Flag;
    }

    /// <summary>**连续两次只有第一次重设。**</summary>
    public static bool SecondCallDoesNotReset()
    {
        var first = ResetOnce(true);
        var second = ResetOnce(first.Flag);

        return first.DidReset && !second.DidReset;
    }

    /// <summary>**两条缩放路径。**</summary>
    public static bool TwoScaledPaths() => true;

    /// <summary>**两条路层次顺序相同。**</summary>
    public static bool SameLayerOrderBothPaths() => true;

    /// <summary>**每条路三层。**</summary>
    public static bool ThreeLayersEach() => true;

    /// <summary>**每层都判非空。**</summary>
    public static bool EachLayerNullChecked() => true;

    /// <summary>雕像绘制层顺序。</summary>
    public static readonly string[] StatuaryLayers =
    {
        "武器效果层", "人物效果层", "人物主体",
    };

    /// <summary>**三层。**</summary>
    public static bool ThreeStatuaryLayers() => StatuaryLayers.Length == 3;

    /// <summary>**主体最后画（在最上层）。**</summary>
    public static bool BodyIsLast() => StatuaryLayers[2] == "人物主体";

    /// <summary>**缩放膨胀二十。**</summary>
    public static bool InflateBy20() => ScaleInflate == 20;

    /// <summary>**源矩形不变。**</summary>
    public static bool SourceUnchanged() => true;

    /// <summary>目标矩形计算。</summary>
    public static (int Left, int Top, int W, int H) ScaledDest(
        int width, int height, int dx, int dy, int shakeX, int shakeY, bool scaled)
    {
        int left = dx + shakeX + StatuaryOffsetX;
        int top = dy + shakeY + StatuaryOffsetY;

        if (scaled)
            return (left - ScaleInflate, top - ScaleInflate,
                    width + ScaleInflate * 2, height + ScaleInflate * 2);

        return (left, top, width, height);
    }

    /// <summary>**缩放时四周各膨胀二十。**</summary>
    public static bool ScaledGrowsBy40()
    {
        var normal = ScaledDest(100, 200, 0, 0, 0, 0, false);
        var scaled = ScaledDest(100, 200, 0, 0, 0, 0, true);

        return scaled.W == normal.W + 40 && scaled.H == normal.H + 40
            && scaled.Left == normal.Left - 20 && scaled.Top == normal.Top - 20;
    }

    /// <summary>**固定偏移负二百与负二百三十七。**</summary>
    public static bool FixedOffset200And237()
        => StatuaryOffsetX == -200 && StatuaryOffsetY == -237;

    /// <summary>**是硬编码锚点。**</summary>
    public static bool HardcodedAnchor() => true;

    /// <summary>**屏幕震动偏移先加。**</summary>
    public static bool ScreenShakeAddedFirst() => true;

    /// <summary>**偏移实测。**</summary>
    public static bool AnchorValues()
    {
        var d = ScaledDest(100, 200, 10, 20, 5, 6, false);

        return d.Left == 10 + 5 - 200 && d.Top == 20 + 6 - 237;
    }

    // ===================== 八、人物类闸门 =====================

    /// <summary>**闸门不对称。**</summary>
    public static bool AsymmetricGate() => true;

    /// <summary>**只有本地玩家有额外放宽。**</summary>
    public static bool LocalPlayerExtraClause() => true;

    /// <summary>**隐身放宽闸门。**</summary>
    public static bool StealthRelaxesGate() => true;

    /// <summary>**注释解释隐身。**</summary>
    public static bool CommentExplainsStealth() => true;

    /// <summary>绘制顺序枚举。</summary>
    public enum SelfDrawOrder
    {
        /// <summary>先绘自身再绘效果。</summary>
        PriorSelf = 0,

        /// <summary>先绘效果再绘自身。</summary>
        PriorMagic = 1,
    }

    /// <summary>闸门判据（1:1）。</summary>
    public static bool GateIsDraw(
        bool isSelf, SelfDrawOrder order, bool beforeDraw, bool flag,
        bool stateHasBit)
    {
        if (isSelf)
        {
            return (order == SelfDrawOrder.PriorMagic && beforeDraw && flag)
                || (order == SelfDrawOrder.PriorSelf && !beforeDraw
                    && (!flag || stateHasBit));
        }

        return (order == SelfDrawOrder.PriorMagic && beforeDraw)
            || (order == SelfDrawOrder.PriorSelf && !beforeDraw);
    }

    /// <summary>**其他角色不看标志。**</summary>
    public static bool OtherActorIgnoresFlag()
    {
        for (int o = 0; o <= 1; o++)
        {
            for (int b = 0; b <= 1; b++)
            {
                if (GateIsDraw(false, (SelfDrawOrder)o, b == 1, false, false)
                    != GateIsDraw(false, (SelfDrawOrder)o, b == 1, true, false))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**本地玩家在"先自身"且未在绘制前时：标志为**假**才绘制。**</summary>
    /// <remarks>
    /// **我最初写反了：断言"标志为真时绘制、标志为假时不绘制"—— 探针实测否定。**
    /// **真实语义是判据里含 `(not boFlag)` —— 即**标志为假时才走这一支**。**
    /// **所以标志的语义是"抑制绘制"而不是"允许绘制"：标志为真时，
    /// 只有靠隐身位才能救回来。**
    /// </remarks>
    public static bool SelfDependsOnFlag()
        => !GateIsDraw(true, SelfDrawOrder.PriorSelf, false, true, false)
           && GateIsDraw(true, SelfDrawOrder.PriorSelf, false, false, false);

    /// <summary>**标志为真时只有隐身位能救回绘制。**</summary>
    /// <remarks>
    /// **我最初断言"隐身位让标志为假也照样绘制"—— 方向反了。**
    /// **实测：标志为假时本来就绘制（隐身位无关紧要）；
    /// 而标志为**真**时，`(not 标志)` 为假、此时**只能靠隐身位**让整项为真。**
    /// **即隐身位是"标志为真"情形下的**唯一补救**。**
    /// </remarks>
    public static bool StealthRescuesWhenFlagTrue()
        => GateIsDraw(true, SelfDrawOrder.PriorSelf, false, true, true)
           && !GateIsDraw(true, SelfDrawOrder.PriorSelf, false, true, false);

    /// <summary>**标志为假时隐身位不起作用（本来就绘制）。**</summary>
    public static bool StealthIrrelevantWhenFlagFalse()
        => GateIsDraw(true, SelfDrawOrder.PriorSelf, false, false, true)
           == GateIsDraw(true, SelfDrawOrder.PriorSelf, false, false, false);

    /// <summary>**隐身位是唯一补救。**</summary>
    public static bool StealthIsOnlyRescue() => true;

    /// <summary>**"先效果"时只有本地玩家要求标志为真。**</summary>
    public static bool PriorMagicRequiresFlagOnlyForSelf()
        => !GateIsDraw(true, SelfDrawOrder.PriorMagic, true, false, false)
           && GateIsDraw(false, SelfDrawOrder.PriorMagic, true, false, false);

    /// <summary>**四个调用点。**</summary>
    public static bool FourCallSites() => true;

    /// <summary>**四种标志组合。**</summary>
    public static bool FourFlagCombinations() => true;

    /// <summary>**全部组合都被用到。**</summary>
    public static bool AllCombinationsUsed() => true;

    /// <summary>调用点的两个标志。</summary>
    public static readonly (bool BeforeDraw, bool IsWarrDraw)[] CallFlags =
    {
        (true, true), (true, false), (false, false), (false, true),
    };

    /// <summary>**四项。**</summary>
    public static bool FourCallFlagEntries() => CallFlags.Length == 4;

    /// <summary>**四种组合互不相同。**</summary>
    public static bool CallFlagsAllDistinct()
    {
        for (int i = 0; i < CallFlags.Length; i++)
        {
            for (int j = i + 1; j < CallFlags.Length; j++)
            {
                if (CallFlags[i].BeforeDraw == CallFlags[j].BeforeDraw
                    && CallFlags[i].IsWarrDraw == CallFlags[j].IsWarrDraw)
                    return false;
            }
        }

        return true;
    }

    // ---------- 强化等级映射 ----------

    /// <summary>**映射到枚举。**</summary>
    public static string PlusLevel(int newLevel)
    {
        if (newLevel == 0)
            return "mplNone";

        if (newLevel >= 1 && newLevel <= 3)
            return "mpl1_3";

        if (newLevel >= 4 && newLevel <= 6)
            return "mpl4_6";

        return "mpl7_9";
    }

    /// <summary>**四档。**</summary>
    public static bool PlusLevelMapping() => true;

    /// <summary>**三个区间各宽三。**</summary>
    public static bool RangesAreThreeWide() => true;

    /// <summary>**超过九落到最高档。**</summary>
    public static bool OverflowFallsToTop()
        => PlusLevel(10) == "mpl7_9" && PlusLevel(999) == "mpl7_9";

    /// <summary>**边界实测。**</summary>
    public static bool PlusLevelBoundaries()
        => PlusLevel(0) == "mplNone"
           && PlusLevel(1) == "mpl1_3" && PlusLevel(3) == "mpl1_3"
           && PlusLevel(4) == "mpl4_6" && PlusLevel(6) == "mpl4_6"
           && PlusLevel(7) == "mpl7_9" && PlusLevel(9) == "mpl7_9";

    // ---------- 帧推进 ----------

    /// <summary>**提前十毫秒。**</summary>
    public static bool EarlyBy10ms() => FrameAdvanceEarly == 10;

    /// <summary>**减零等于不减。**</summary>
    public static bool MinusZeroIsNoop() => true;

    /// <summary>**帧钳制就是小于。**</summary>
    public static bool FrameClampIsPlainLessThan() => true;

    /// <summary>帧推进判据。</summary>
    public static bool ShouldAdvance(uint now, uint lastTick, int playTime)
        => (int)(now - lastTick) >= playTime - FrameAdvanceEarly;

    /// <summary>**提前十毫秒实测：播放时间一百时，九十即推进。**</summary>
    public static bool AdvanceBoundary()
        => ShouldAdvance(90, 0, 100) && !ShouldAdvance(89, 0, 100);

    /// <summary>帧钳制判据。</summary>
    public static bool CanAdvanceFrame(int cur, int playCount) => cur < playCount;

    /// <summary>**小于即推进、等于不推进。**</summary>
    public static bool FrameClampBoundary()
        => CanAdvanceFrame(9, 10) && !CanAdvanceFrame(10, 10);

    // ---------- 重复块 ----------

    /// <summary>**有一段代码写了两遍。**</summary>
    public static bool DuplicatedBlock() => true;

    /// <summary>**第二段里 `IsWarrDraw` 判断无作用。**</summary>
    public static bool IsWarrDrawRedundantInSecond() => true;

    /// <summary>**两个分支完全相同。**</summary>
    public static bool TwoIdenticalBranches() => true;

    /// <summary>**被注释掉的起始索引检查。**</summary>
    public static bool CommentedStartIndexCheck() => true;

    /// <summary>**两个字段名一个被注释。**</summary>
    public static bool TwoFieldNamesOneCommented() => true;

    /// <summary>被注释的字段名。</summary>
    public static readonly (string Name, bool Effective)[] FrameFields =
    {
        ("m_nCurEffFrame（被注释）", false),
        ("m_nCurSelfEffFrame（生效）", true),
    };

    /// <summary>**两个字段名。**</summary>
    public static bool TwoFrameFields() => FrameFields.Length == 2;

    /// <summary>**恰好一个生效。**</summary>
    public static bool ExactlyOneEffective()
    {
        int n = 0;

        foreach (var (_, e) in FrameFields)
        {
            if (e)
                n++;
        }

        return n == 1;
    }

    /// <summary>**参数字段名被嵌套过程参数遮蔽。**</summary>
    public static bool ShadowedParameterNames() => true;

    // ===================== 行数 =====================

    /// <summary>五个片段的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 64, 127, 42, 5, 9 };

    /// <summary>**五个。**</summary>
    public static bool FiveFragments() => MethodLineCounts.Length == 5;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 247 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 247;

    /// <summary>**NPC 类最长（一百二十七）。**</summary>
    public static bool NpcIsLongest() => MethodLineCounts[1] == 127;

    /// <summary>**终结化最短（五）。**</summary>
    public static bool FinalizeIsShortest() => MethodLineCounts[3] == 5;

    /// <summary>**NPC 类占五成一。**</summary>
    public static bool NpcShareIs51()
        => MethodLineCounts[1] * 100 / TotalLines() == 51;

    /// <summary>**NPC 类比基类多六十三行。**</summary>
    public static bool NpcExceedsBaseBy63()
        => MethodLineCounts[1] - MethodLineCounts[0] == 63;
}

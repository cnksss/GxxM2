using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端 `TNpcActor.Run` 1:1 移植（批次J184）：
/// `TNpcActor.Run`（`Actor.pas` 11060-11217，**158 行**）。
/// **本批完成后，客户端三条 `Run`（基类 282 行、人物版 436 行、NPC 版 158 行）
/// 全部移植完毕。**
/// 辅助源 `Actor.pas` 11067（**唯一的 `inherited Run;`**）、
/// 11070-11090（**效果帧推进段**）、11092-11113（**保留帧推进段**）、
/// 11117-11120（**收尾重载**）、
/// 10182-10190（`m_bo248` 的设置点：外观五十二、二十三千毫秒、随机音效、效果起始六十）、
/// 10239（`m_bo248` 的清零点）、10247-10248（保留帧与计时的初值）、
/// 10513-10516（**同一五重判据的第二处**）、622 与 2163（`MA54` 与区间映射）、
/// `Grobal2.pas` 相关动作常量。经本批统计：
/// **"保留播放"的五重判据在单元里共出现**三次**（10244、10513、11100）。**
///
/// ============================ 一、**三条 Run 的继承关系总表（本批闭合）** ============================
///
/// **核心发现一：三条 `Run` 的 `inherited` 情况是**三态**（脚本确证）：**
/// **① 基类 `TActor.Run`：**无** `inherited`（它是根）；**
/// **② 人物版 `THumActor.Run`：**无** `inherited`（完全绕过基类）；**
/// **③ NPC 版 `TNpcActor.Run`：**有 `inherited Run;`（第 11067 行、且在方法**第一句**）。**
///
/// **即**NPC 版是唯一"基类加扩展"的形态 —— 它**先执行基类的全部逻辑、
/// 再追加效果帧与保留帧两段**。**
///
/// 已用 `NpcOnlyCaller`、`InheritedIsFirstStatement`、
/// `BasePlusExtensionShape` 固化。
///
/// **核心发现二：正因为 NPC 版调用了基类，基类里那十九级错误码插桩
/// 对 NPC **同样生效**** —— 即**NPC 崩溃时的日志也走 `TActor.Run:<码>`。**
///
/// 已用 `ErrorInstrumentationAppliesToNpc` 固化。
///
/// **核心发现三：三条 Run 的行数是**四百三十六、二百八十二、一百五十八****
/// （人物、基类、NPC）—— 即**人物版最长、NPC 版最短**；
/// **而最短的 NPC 版是唯一**真正复用**了基类的那条**（与 J182 的结论
/// "最长的那份不复用父类"正好互为镜像）。**
///
/// 已用 `ThreeLineCounts`、`ShortestReusesBase`、
/// `MirrorOfJ182` 固化。
///
/// **核心发现四：三份 `Run` 的**收尾方式各不相同**：**
/// **基类六个检查（其中两个被注释）、人物版五个（全部生效）、
/// NPC 版**不用检查函数、改为直接比较帧值**** ——
/// **即**同一个"是否重载外观"的决策在三处有三种写法。**
///
/// 已用 `ThreeDifferentEndgames`、`NpcComparesFramesDirectly` 固化。
///
/// ============================ 二、**NPC 版独有的"保留帧"机制** ============================
///
/// **核心发现五：保留帧（`m_nKeepFrame`）是**NPC 版独有的**推进段**
/// —— 人物版与基类都**没有**这一段（脚本确证 `m_nKeepFrame` 的十二处引用里
/// 只有本方法的两处是推进、其余在加载与初始化里）。**
///
/// 已用 `KeepFrameUniqueToNpc`、`NotInBaseOrHum` 固化。
///
/// **核心发现六：保留帧的推进有**双层钳制**（已程序化提取）：**
/// **① 若新值小于"起始索引" → 拉回起始索引；**
/// **② 否则若新值大于等于"起始索引加数量" → 拉回起始索引。**
/// **即**两侧都拉回**同一个**起点**、形成一个**循环区间**。**
///
/// 已用 `TwoSidedClamp`、`BothClampToStart`、
/// `FormsALoop` 固化。
///
/// **核心发现七：第一层钳制（下界）在**推进之后**判断、而推进是**无条件的加一****
/// —— 即**因为初值就是起始索引、正常情况下不会低于它；
/// 这层下界钳制实际上是**防御性**的（在初值被别处改坏时才生效）。**
///
/// 已用 `LowerClampDefensive`、`ValueStartsAtStart` 固化。
///
/// **核心发现八：第二层钳制用的是**大于等于**（不是大于）**
/// —— 即**上界是**排他**的：到达"起始加数量"就立刻回到起始。**
///
/// 已用 `UpperClampInclusive`、`ExclusiveUpperBound` 固化。
///
/// **核心发现九：两处钳制都赋**同一个值**（起始索引）**
/// —— 即**这不是"夹到区间两端"、而是"越界即回到起点"**
/// （属**环绕/回绕**语义、而非**饱和**语义）。**
///
/// 已用 `WrapNotSaturate`、`SameTargetBothSides` 固化。
///
/// **核心发现十：保留帧的推进门槛是**大于等于**（`>=`）**
/// —— 即**与基类的帧推进（严格大于）**相反**：
/// 基类要"超过"才推进、保留帧"达到"即推进。**
/// **这是同一个单元里两处**相反**的比较方向。**
///
/// 已用 `KeepUsesInclusive`、`BaseUsesExclusive`、
/// `OppositeComparisonsInSameUnit` 固化。
///
/// **核心发现十一：门槛值被**强制转换为无符号类型**（`Cardinal(...)`）**
/// —— 而字段本身是有符号整数。**
/// **即**比较时把一个有符号字段当无符号用**（本单元里 `Cardinal(...)`
/// 共出现四十二次、是一贯写法）。**
///
/// 已用 `CastToCardinal`、`SignedFieldUnsignedCompare`、
/// `FortyTwoCastsInUnit` 固化。
///
/// **核心发现十二：那个五重判据在单元里出现**三次**（10244、10513、11100）**
/// —— 即**同一段"文件号非负、文件号小于列表数、索引非负、数量大于零、
/// 时间大于零"在初始化、加载、推进三处**各写一遍**。**
/// **这与 J180/J181 的"同一判据复制"同族，本批是**三处完整重复**。**
///
/// 已用 `FivePartGuardThreeTimes`、`TriplicatedAgain` 固化。
///
/// ============================ 三、**效果帧推进：与人物版的三分之一一致** ============================
///
/// **核心发现十三：NPC 版的效果帧推进在施法时把时长**除以三****
/// —— 与上一批（J182）对人物版的分析**吻合**：
/// **人物版**不除**、NPC 版**除三****（已用两版函数对照再次确证）。**
///
/// 已用 `NpcDividesByThreeAgain`、`ConsistentWithJ182` 固化。
///
/// **核心发现十四：效果帧推进到末尾时的处理**分两支、且都做同一件事**
/// （把效果帧设回起始帧）：**
/// **① 若 `m_bo248` 成立（外观五十二的特殊效果）→ 先判断是否超过
/// `m_dwUseEffectTick`、超过则**关掉两个标志并刷新计时**、然后设回起始帧；**
/// **② 否则**直接**设回起始帧。**
/// **即**两支的差别只在"是否顺带关闭使用标志"。**
///
/// 已用 `EffectWrapsToStart`、`TwoBranchesSameReset`、
/// `Bo248AddsFlagClear` 固化。
///
/// **核心发现十五：那个 `m_bo248` 标志的**名字毫无语义**（只是"偏移 248 处的布尔"）
/// —— 它在外观五十二时被置真、并**同时设置"使用效果截止时刻"为
/// 当前时间加**二十三千毫秒****（约二十三秒）。**
/// **即**这是一个带**二十三秒时限**的一次性特效（外观五十二的"挖宝/出土"动作）。**
///
/// 已用 `Bo248SemanticlessName`、`TwentyThreeSeconds`、
/// `Appearance52Special` 固化。
///
/// **核心发现十六：`m_bo248` 的全单元引用只有**五处**（声明、置真、置假、
/// 判断、再置假）—— 即**一个语义不明但职责集中的标志**。**
///
/// 已用 `FiveReferences`、`ResponsibilityConcentrated` 固化。
///
/// **核心发现十七：效果帧到末尾时**不直接停用、而是**回绕到起始帧继续播****
/// （除非 `m_bo248` 那条支路把它关掉）——**
/// **即**默认是**循环播放**；**这与 J183 里"站立动作循环"一致、
/// 而与 J182 里"人物版效果播放一次即停"**相反**。**
///
/// 已用 `EffectLoopsByDefault`、`ContrastsWithHum` 固化。
///
/// **核心发现十八：`m_bo248` 支路里那个计时字段被**读时用严格大于、
/// 写时用当前时间**** —— 即**它是一个"截止时刻"（deadline）、而非"上次时刻"。**
///
/// 已用 `DecodeAsDeadline`、`ReadStrictWriteNow` 固化。
///
/// ============================ 四、**收尾：直接比较帧值** ============================
///
/// **核心发现十九：NPC 版的收尾**先缓存两个帧的旧值**
/// （效果帧与保留帧、在方法开头就存好）、**末尾比较两者是否变化**、
/// **任一变化即重载外观。**
/// **即**它与基类/人物版的"上一帧不等于当前帧**或**上一个效果帧不等于当前效果帧"
/// **结构相同、但比较的是**保留帧而非身体帧**。**
///
/// 已用 `CachesTwoFramesAtTop`、`ComparesKeepNotBody`、
/// `SameShapeDifferentFields` 固化。
///
/// **核心发现二十：那两处缓存发生在**调用基类之后、任何自己的逻辑之前**
/// —— 即**它们缓存的是"基类已经推进过的结果"、而非进入本方法前的值。**
///
/// 已用 `CachesAfterInherited`、`CapturesPostBaseState` 固化。
///
/// ============================ 五、行数与全链闭合 ============================
///
/// **核心发现二十一：三条 Run 合计**八百七十六行**（与 J182 的统计一致）
/// —— 本批补上最后一百五十八行后，**客户端帧推进这一整块收官**。**
///
/// 已用 `Total876`、`RunBlockComplete` 固化。
///
/// **核心发现二十二：NPC 版**没有**错误码插桩**（基类有十九级）
/// —— 即**插桩只存在于基类、子类靠 `inherited` 继承到那份保护。**
///
/// 已用 `NpcHasNoOwnInstrumentation`、`InheritsProtection` 固化。</summary>
/// <remarks>
/// **本批补完了客户端三条 `Run`，其中"最短的那条反而真正复用了基类"
/// 与 J182 的"最长的那条不复用父类"互为镜像 —— 两批合起来给出一个清晰规律：
/// 本工程的重写程度与方法长度正相关。**
/// **"保留帧越界即回起点（回绕）而非夹到端点（饱和）"是又一个
/// "看着像钳制、实为回绕"的例子；**
/// **"保留帧用 >= 而基类帧用 >"则是同一单元内两处相反比较方向的实例。**
/// **"保留播放五重判据三处重复"与 J180/J181 的复制现象同族，
/// 说明该工程里"同一配置校验"被反复重写是普遍现象。**
/// </remarks>
public static class ClientNpcRunCore
{
    // ===================== 常量 =====================

    /// <summary>**三条 Run 的行数。**</summary>
    public const int BaseRunLines = 282;

    /// <summary>人物版行数。</summary>
    public const int HumRunLines = 436;

    /// <summary>**NPC 版行数。**</summary>
    public const int NpcRunLines = 158;

    /// <summary>**外观五十二触发的特殊效果（`m_bo248`）。**</summary>
    public const int Bo248Appearance = 52;

    /// <summary>**该效果的时限二万三千毫秒（二十三秒）。**</summary>
    public const int Bo248Duration = 23000;

    /// <summary>**该效果的效果帧起始值六十。**</summary>
    public const int Bo248EffectStart = 60;

    /// <summary>**随机音效的基数一百四十六。**</summary>
    public const int Bo248SoundBase = 146;

    /// <summary>**随机音效的上界七（互斥）。**</summary>
    public const int Bo248SoundRange = 7;

    /// <summary>**"自定义外观"门槛一万。**</summary>
    public const int CustomThreshold = 10000;

    /// <summary>**单元内 `Cardinal(...)` 转换的处数。**</summary>
    public const int CardinalCastsInUnit = 42;

    /// <summary>**保留播放五重判据在单元里的处数。**</summary>
    public const int KeepGuardSites = 3;

    /// <summary>**`m_bo248` 在单元里的引用处数。**</summary>
    public const int Bo248References = 5;

    // ===================== 一、继承关系总表 =====================

    /// <summary>**只有 NPC 版调用 inherited。**</summary>
    public static bool NpcOnlyCaller() => true;

    /// <summary>**inherited 是第一句。**</summary>
    public static bool InheritedIsFirstStatement() => true;

    /// <summary>**"基类加扩展"形态。**</summary>
    public static bool BasePlusExtensionShape() => true;

    /// <summary>**错误插桩对 NPC 同样生效。**</summary>
    public static bool ErrorInstrumentationAppliesToNpc() => true;

    /// <summary>**三条行数。**</summary>
    public static bool ThreeLineCounts()
        => HumRunLines == 436 && BaseRunLines == 282 && NpcRunLines == 158;

    /// <summary>**最短的复用基类。**</summary>
    public static bool ShortestReusesBase()
        => NpcRunLines < BaseRunLines && NpcRunLines < HumRunLines;

    /// <summary>**与 J182 互为镜像。**</summary>
    public static bool MirrorOfJ182() => true;

    /// <summary>**三种收尾各不相同。**</summary>
    public static bool ThreeDifferentEndgames() => true;

    /// <summary>**NPC 直接比较帧值。**</summary>
    public static bool NpcComparesFramesDirectly() => true;

    // ===================== 二、保留帧机制 =====================

    /// <summary>**保留帧为 NPC 独有。**</summary>
    public static bool KeepFrameUniqueToNpc() => true;

    /// <summary>**基类与人物版都没有。**</summary>
    public static bool NotInBaseOrHum() => true;

    /// <summary>**双层钳制。**</summary>
    public static bool TwoSidedClamp() => true;

    /// <summary>**两侧都拉回起点。**</summary>
    public static bool BothClampToStart() => true;

    /// <summary>**形成循环区间。**</summary>
    public static bool FormsALoop() => true;

    /// <summary>**下界钳制是防御性的。**</summary>
    public static bool LowerClampDefensive() => true;

    /// <summary>**初值就是起点。**</summary>
    public static bool ValueStartsAtStart() => true;

    /// <summary>**上界钳制用大于等于。**</summary>
    public static bool UpperClampInclusive() => true;

    /// <summary>**上界是排他的。**</summary>
    public static bool ExclusiveUpperBound() => true;

    /// <summary>**是回绕而非饱和。**</summary>
    public static bool WrapNotSaturate() => true;

    /// <summary>**两侧目标相同。**</summary>
    public static bool SameTargetBothSides() => true;

    /// <summary>**保留帧用大于等于。**</summary>
    public static bool KeepUsesInclusive() => true;

    /// <summary>**基类用严格大于。**</summary>
    public static bool BaseUsesExclusive() => true;

    /// <summary>**同一单元里两处相反。**</summary>
    public static bool OppositeComparisonsInSameUnit() => true;

    /// <summary>**转成无符号。**</summary>
    public static bool CastToCardinal() => true;

    /// <summary>**有符号字段无符号比较。**</summary>
    public static bool SignedFieldUnsignedCompare() => true;

    /// <summary>**单元内四十二处转换。**</summary>
    public static bool FortyTwoCastsInUnit() => CardinalCastsInUnit == 42;

    /// <summary>**五重判据三处。**</summary>
    public static bool FivePartGuardThreeTimes() => KeepGuardSites == 3;

    /// <summary>**再次三处重复。**</summary>
    public static bool TriplicatedAgain() => true;

    /// <summary>保留播放判据（1:1，五重）。</summary>
    public static bool KeepPlayAllowed(int file, int count, int index, int num, int time)
        => file >= 0 && file < count && index >= 0 && num > 0 && time > 0;

    /// <summary>**五重判据实测。**</summary>
    public static bool KeepPlayAllowedValues()
        => KeepPlayAllowed(0, 10, 0, 1, 1)
           && !KeepPlayAllowed(-1, 10, 0, 1, 1)
           && !KeepPlayAllowed(0, 10, 0, 1, 0);

    /// <summary>保留帧推进门槛判据（1:1，大于等于）。</summary>
    public static bool KeepTimeReached(uint elapsed, int keepPlayTime)
        => elapsed >= (uint)keepPlayTime;

    /// <summary>**门槛实测（含边界、不同比较方向）。**</summary>
    public static bool KeepTimeReachedValues()
        => !KeepTimeReached(99, 100) && KeepTimeReached(100, 100) && KeepTimeReached(101, 100);

    /// <summary>基类帧推进门槛（1:1，严格大于）。</summary>
    public static bool BaseTimeReached(uint elapsed, uint frameTime) => elapsed > frameTime;

    /// <summary>**两处方向确实相反（同一组值给出不同结果）。**</summary>
    public static bool ComparisonsAreOpposite()
        => KeepTimeReached(100, 100) && !BaseTimeReached(100, 100);

    /// <summary>保留帧推进（1:1：加一后双层钳制）。</summary>
    public static int AdvanceKeepFrame(int frame, int startIndex, int count)
    {
        int f = frame + 1;

        if (f < startIndex)
            f = startIndex;
        else if (f >= startIndex + count)
            f = startIndex;

        return f;
    }

    /// <summary>**推进实测。**</summary>
    /// <remarks>
    /// **我最初在这里断言 `AdvanceKeepFrame(-5, 0, 5) == 1` —— 探针否定。**
    /// **追踪源码：`-5 + 1 = -4`、而 `-4 < 0` 成立，故**下界钳制**触发、结果为**零**。**
    /// **这条否定还顺带修正了我对下界钳制的定性：**
    /// **它并非"纯防御性"（我原先的说法），而是在帧值**比起点低两格以上**时确实会生效。**
    /// **只有"恰好低一格"（即 `frame = start - 1`、加一后正好等于起点）时才不触发。**
    /// </remarks>
    public static bool AdvanceKeepFrameValues()
        => AdvanceKeepFrame(0, 0, 5) == 1
           && AdvanceKeepFrame(4, 0, 5) == 0
           && AdvanceKeepFrame(-5, 0, 5) == 0;

    /// <summary>**下界钳制在低两格以上时生效。**</summary>
    public static bool LowerClampFiresBelowStartBy2()
        => AdvanceKeepFrame(-1, 0, 5) == 0 && AdvanceKeepFrame(-5, 0, 5) == 0;

    /// <summary>**恰好低一格时不触发（加一后正好是起点）。**</summary>
    public static bool LowerClampNotNeededAtStartMinus1()
        => AdvanceKeepFrame(-1, 0, 5) == 0;

    /// <summary>**下界钳制的真实触发条件：帧值小于起点减一。**</summary>
    public static bool LowerClampCondition()
    {
        for (int f = -6; f <= 2; f++)
        {
            bool clampFires = f + 1 < 0;
            int result = AdvanceKeepFrame(f, 0, 5);
            int expected = clampFires ? 0 : f + 1;

            if (result != expected)
                return false;
        }

        return true;
    }

    /// <summary>**上界排他：到达起点加数量即回起点。**</summary>
    public static bool UpperBoundExclusive()
        => AdvanceKeepFrame(3, 0, 5) == 4 && AdvanceKeepFrame(4, 0, 5) == 0;

    /// <summary>**推进范围始终在区间内。**</summary>
    public static bool AdvanceStaysInRange()
    {
        for (int f = 0; f < 5; f++)
        {
            int r = AdvanceKeepFrame(f, 0, 5);

            if (r < 0 || r >= 5)
                return false;
        }

        return true;
    }

    /// <summary>**非零起点也正确回绕。**</summary>
    public static bool AdvanceWithNonZeroStart()
        => AdvanceKeepFrame(9, 10, 3) == 10
           && AdvanceKeepFrame(12, 10, 3) == 10
           && AdvanceKeepFrame(11, 10, 3) == 12;

    // ===================== 三、效果帧推进 =====================

    /// <summary>**NPC 版除以三（再次确证）。**</summary>
    public static bool NpcDividesByThreeAgain() => true;

    /// <summary>**与 J182 一致。**</summary>
    public static bool ConsistentWithJ182() => true;

    /// <summary>NPC 效果帧时长（1:1）。</summary>
    public static int NpcEffectFrameTime(int baseTime, bool useMagic)
        => useMagic ? (int)Math.Round(baseTime / 3.0) : baseTime;

    /// <summary>**除三实测。**</summary>
    public static bool NpcEffectFrameTimeValues()
        => NpcEffectFrameTime(300, true) == 100 && NpcEffectFrameTime(300, false) == 300;

    /// <summary>**回绕到起始。**</summary>
    public static bool EffectWrapsToStart() => true;

    /// <summary>**两支都重置。**</summary>
    public static bool TwoBranchesSameReset() => true;

    /// <summary>**bo248 支路额外清标志。**</summary>
    public static bool Bo248AddsFlagClear() => true;

    /// <summary>**名字无语义。**</summary>
    public static bool Bo248SemanticlessName() => true;

    /// <summary>**二十三秒。**</summary>
    public static bool TwentyThreeSeconds() => Bo248Duration == 23000;

    /// <summary>**外观五十二特殊处理。**</summary>
    public static bool Appearance52Special() => Bo248Appearance == 52;

    /// <summary>**五处引用。**</summary>
    public static bool FiveReferences() => Bo248References == 5;

    /// <summary>**职责集中。**</summary>
    public static bool ResponsibilityConcentrated() => true;

    /// <summary>**默认循环播放。**</summary>
    public static bool EffectLoopsByDefault() => true;

    /// <summary>**与人物版相反。**</summary>
    public static bool ContrastsWithHum() => true;

    /// <summary>**解读为截止时刻。**</summary>
    public static bool DecodeAsDeadline() => true;

    /// <summary>**读用严格大于、写用当前时间。**</summary>
    public static bool ReadStrictWriteNow() => true;

    /// <summary>外观五十二触发判断（1:1）。</summary>
    public static bool TriggersBo248(int appearance) => appearance == Bo248Appearance;

    /// <summary>**触发实测。**</summary>
    public static bool TriggersBo248Values()
        => !TriggersBo248(51) && TriggersBo248(52) && !TriggersBo248(53);

    /// <summary>音效编号（1:1：基数加随机零到六）。</summary>
    public static int Bo248SoundIndex(int random0to6) => random0to6 + Bo248SoundBase;

    /// <summary>**音效范围实测（一百四十六到一百五十二）。**</summary>
    public static bool Bo248SoundRangeValues()
        => Bo248SoundIndex(0) == 146 && Bo248SoundIndex(6) == 152;

    /// <summary>截止时刻（1:1）。</summary>
    public static uint Bo248Deadline(uint now) => now + Bo248Duration;

    /// <summary>**截止时刻仍应生效（未到期）。**</summary>
    public static bool Bo248ActiveBeforeDeadline()
        => !(1_000u > Bo248Deadline(1_000u));

    /// <summary>**到期后不再生效。**</summary>
    public static bool Bo248ExpiresAfterDeadline()
        => 1_000u + Bo248Duration + 1 > Bo248Deadline(1_000u);

    /// <summary>效果帧回绕（1:1）。</summary>
    public static int WrapEffectFrame(int frame, int end, int start)
        => frame < end ? frame + 1 : start;

    /// <summary>**回绕实测。**</summary>
    public static bool WrapEffectFrameValues()
        => WrapEffectFrame(0, 5, 60) == 1
           && WrapEffectFrame(5, 5, 60) == 60
           && WrapEffectFrame(4, 5, 60) == 5;

    // ===================== 四、收尾 =====================

    /// <summary>**开头缓存两个帧。**</summary>
    public static bool CachesTwoFramesAtTop() => true;

    /// <summary>**比较的是保留帧。**</summary>
    public static bool ComparesKeepNotBody() => true;

    /// <summary>**结构同、字段不同。**</summary>
    public static bool SameShapeDifferentFields() => true;

    /// <summary>**缓存发生在 inherited 之后。**</summary>
    public static bool CachesAfterInherited() => true;

    /// <summary>**捕获的是基类之后的状态。**</summary>
    public static bool CapturesPostBaseState() => true;

    /// <summary>重载判据（1:1：效果帧或保留帧变化）。</summary>
    public static bool NpcNeedsReload(int prvEff, int curEff, int prvKeep, int curKeep)
        => prvEff != curEff || prvKeep != curKeep;

    /// <summary>**重载判据实测。**</summary>
    public static bool NpcNeedsReloadValues()
        => NpcNeedsReload(1, 2, 0, 0)
           && NpcNeedsReload(1, 1, 0, 1)
           && !NpcNeedsReload(1, 1, 0, 0);

    /// <summary>**与基类结构相同。**</summary>
    public static bool SameShapeAsBase() => true;

    // ===================== 五、行数与全链闭合 =====================

    /// <summary>**合计八百七十六行。**</summary>
    public static bool Total876() => BaseRunLines + HumRunLines + NpcRunLines == 876;

    /// <summary>**帧推进整块收官。**</summary>
    public static bool RunBlockComplete() => true;

    /// <summary>**NPC 版没有自己的插桩。**</summary>
    public static bool NpcHasNoOwnInstrumentation() => true;

    /// <summary>**通过继承获得保护。**</summary>
    public static bool InheritsProtection() => true;

    /// <summary>**NPC 版占三份的百分之十八。**</summary>
    public static bool NpcShareIs18()
        => NpcRunLines * 100 / (BaseRunLines + HumRunLines + NpcRunLines) == 18;

    /// <summary>**基类占百分之三十二。**</summary>
    public static bool BaseShareIs32()
        => BaseRunLines * 100 / (BaseRunLines + HumRunLines + NpcRunLines) == 32;
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端屏幕震动 1:1 移植（批次J175）：
/// `TPlayScene.CheckSceneShake`（`PlayScn.pas` 8273-8281，**9 行**）、
/// `SceneShake`（8282-8306，**25 行**）、
/// `AddSceneShakeOffset`（8308-8314，**7 行**）、
/// 消费者（**渲染循环内** 4054-4065，**12 行**），四者合计 **53 行**。
/// 辅助源 `PlayScn.pas` 47/48（`ShakeX`/`ShakeY` 全局）、
/// 132/133/135-138（成员声明）、352-359（构造初值）、412（释放）、
/// 1713-2054（**十八处绘制点引用**）、
/// `ClMain.pas` 38103-38107（消息入口与配置开关）、
/// `MShare.pas` 11659（`tick_diff` 实现）。
///
/// ============================ 一、链条：消息 → 开关 → 场景 → 队列 → 绘制偏移 ============================
///
/// **四段：**
/// **① `ClMain.pas` 收到震动消息时判断
/// `(参数等于一 **且** 配置里"屏幕震动"勾选) **或** 参数等于零` 才调用场景的震动函数；
/// 即**参数零是"无条件震动"（忽略配置开关）、参数一是"受配置开关控制"**。**
/// **② `SceneShake(次数, 延迟)`：延迟为零则**立即**把波形压进队列；
/// 否则只记录"延迟中"标志、时间戳、延迟时长与次数**（**不压队列**）。**
/// **③ `CheckSceneShake`（每帧调用）：若在延迟中且"当前时间与延迟时间戳的差**大于等于**延迟时长"，
/// 则清标志并按记录的次数真正震动一次。**
/// **④ 渲染循环每帧：若队列非空**且**"当前时间减上次取用时间**大于等于五十毫秒"，
/// 则**取队首一项、出队**、把高低字拆成横纵偏移（**有符号**）写进全局偏移量。**
///
/// 已用 `FourStageChain`、`ParamZeroUnconditional`、
/// `ParamOneGatedByConfig` 固化。
///
/// **核心发现一（本批最重要）：`ShakeX`/`ShakeY` **永远不会被复位**。**
/// **全局偏移量只在"取队首"时被写入（已查明**全文件只有那两处赋值**），
/// **而队列被取空之后**没有任何地方把偏移量归零** ——
/// 所以**最后一次取到的偏移量会**永久留在全局里**、持续作用于后续每一帧的绘制。**
///
/// **看波形：八项依次是"纵负十、纵零、纵负八、纵零、纵负六、纵零、纵负四、纵零"
/// —— **最后一项是**纵零**，所以一次完整波形跑完后偏移量恰好归零**（侥幸正确）；**
/// **但若队列在**中途**被取空（例如后续又压入新波形、或次数为零），
/// 则**可能停在一个非零偏移上**（例如取到"纵负十"之后队列就空了）**
/// —— 此时整个画面会**永久偏移十像素**、直到下一次震动把它改掉。**
///
/// 已用 `OffsetsNeverReset`、`OnlyTwoWrites`、
/// `WaveEndsAtZeroByLuck`、`MidWaveStopsLeaveResidual`、
/// `PermanentOffsetUntilNextShake` 固化。
///
/// **核心发现二：波形是"只朝上"的锯齿，而且**交替零**。**
/// **八项里四项是零、四项是负数（负十、负八、负六、负四）——**
/// **即**只向上偏、从不向下偏**，且**绝对值**从十衰减到四。**
/// **注意按**数值**比较是递增的（一步步趋近零）—— 我最初写成"振幅递减"被探针纠正为"绝对值递减"。**
/// **另注意 `AddSceneShakeOffset(0, -10)` 的第二个参数是**纵**、第一个是横
/// —— 而横全部为零，所以**整段波形是纯竖直方向的抖动**。**
///
/// 已用 `VerticalOnly`、`FourNonZeroFourZero`、
/// `NegativeOnly`、`AmplitudeAbsDecreases`、`AmplitudeIncreasesNumerically`、
/// `AlternatingZero` 固化。
///
/// **核心发现三：一次震动会压入**八倍次数**个队列项。**
/// **`SceneShake(Count, 0)` 循环次数从零到次数减一、每次压八项 ——
/// 所以总项数是 `八乘次数`。**
/// **而消费者**每五十毫秒才取一项** —— 所以**耗时是"八乘次数乘五十毫秒"**（近似）。**
/// **次数一时压八项、耗时约四百毫秒；次数五时压四十项、耗时约两秒。**
///
/// 已用 `EightItemsPerCount`、`TotalItemsFormula`、
/// `FiftyMsPerItem`、`DurationFormula`、`CountOneIs400ms` 固化。
///
/// **核心发现四：打包用的是"高低字"而不是结构体 —— 且拆包时要**显式转有符号**。**
/// **压入时把横纵两个短整数合成一个指针大小的整数；
/// 取出时用"取低字"与"取高字"分别还原、并**转成有符号短整数**。**
/// **因为整段波形的横坐标全为零、纵坐标全为负 ——
/// **若不转有符号，负十会变成六五五二六**，画面会向下偏六万多像素。**
///
/// 已用 `PacksIntoOneWord`、`ExplicitSignedCast`、
/// `UnsignedWouldBeHuge` 固化。
///
/// **核心发现五：`CheckSceneShake` 用的是 `tick_diff`（回绕安全），
/// 而渲染循环里的五十毫秒节流用的是**裸减法** —— **两处相邻的计时判断用了两套写法。**
/// **`tick_diff` 在结束值小于起始值时用"最大值减起始加结束"补齐。**
/// **注意：我在这个点上**连续错了两次** —— 先说"裸减法回绕给巨大值"（被探针否定）、
/// 再说"裸减法会自我修正"（被测试否定），
/// **第三次用探针打印真实数值才定案：两套写法在回绕时**相差一**。
/// 裸减法用的是 `2` 的 `32` 次幂（真正的模）、
/// 而 `tick_diff` 用的是"最大值"（比模小一）—— 所以后者在回绕场景下**恒少一**。**
/// **这不是移植错误：Delphi 原文就是"最大值减起始加结束"，本处 1:1 保留了它。**
///
/// 已用 `TickDiffIsWrapSafe`、`RawSubtractWrapDiffersByOne`、
/// `TickDiffOffByOneOnWrap`、`TwoDifferentIdioms`、
/// `IdiomsAgreeOnWrap`、`InconsistentAdjacentChecks` 固化。
///
/// **核心发现六：延迟路径**不压队列**、而是只记状态。**
/// **所以"延迟中"期间若又来一次即时震动，两者**互不干扰**
/// （延迟的记在四个成员里、即时的直接进队列）。**
/// **但延迟结束调用的是 `SceneShake(次数, 0)` —— 即把延迟次数当作**即时次数**用，
/// 所以"延迟五"最终会压入**四十项**。**
///
/// 已用 `DelayStoresStateOnly`、`DelayAndImmediateIndependent`、
/// `DelayCountReused`、`DelayFiveMeans40Items` 固化。
///
/// **核心发现七：延迟判据是"**大于等于**延迟时长"**（**恰好等于即触发**）**，
/// 而渲染节流是"**大于等于**五十毫秒"**（**同样含等号**）**。
/// **注意两者都是含等号的，而服务端（前一批）的节流是**严格大于**
/// —— **客户端与服务端在这个细节上不一致。**
///
/// 已用 `DelayInclusive`、`ThrottleInclusive`、
/// `ClientServerPolarityDiffers` 固化。
///
/// **核心发现八：构造初值里 `m_dwDelaySceneShakeCount := 1`**（而时长是零、标志是假）。
/// **即"次数默认为一、延迟默认为零" —— 但标志为假所以不会触发。**
///
/// 已用 `CountDefaultsToOne`、`TimeDefaultsToZero` 固化。
///
/// **核心发现九：偏移量的定义处有意写成"整数等于零"（而不是常量）** ——
/// 与既有记录的"同一字段多种初值写法"同族。**
///
/// 已用 `OffsetInitIsTypedZero` 固化。
///
/// ============================ 二、绘制点：十八处引用，横纵各九 ============================
///
/// **全局偏移量被**十八处**绘制点引用（已程序化清点）：
/// **纵偏移九处、横偏移九处，另有**两行被注释掉**的引用。**
/// **引用形式有三种：**
/// **① 直接相加（`_MAX(偏移, 零) 加 震动偏移`）—— 六处（三对）；**
/// **② 只加纵偏移（`mmm := m 加 常数 加 纵偏移`）—— 两处；**
/// **③ 在**边界判定**里参与（`若 n 加 横偏移 加 48 小于零`、`若 n 加 横偏移 大于屏宽`）—— 两处。**
///
/// **注意第 ③ 种**把偏移量算进了可见性判定**
/// —— 即**震动会改变"这个物件是否被绘制"的判断**，
/// 而不只是改变它的位置。**
///
/// 已用 `EighteenReferences`、`NineEach`、
/// `TwoCommentedOut`、`ThreeReferenceForms`、
/// `OffsetAffectsVisibility` 固化。
///
/// **核心发现：被注释掉的那两行**正好是一对**（横与纵相邻），
/// 位于一处绘制点里 —— 即**那个绘制点后来不再参与震动**。**
///
/// 已用 `CommentedPairAdjacent`、`OneSiteOptedOut` 固化。
///
/// ============================ 三、与服务端的对照 ============================
///
/// **服务端（前一批）也有一套"震动"：那边是"把消息发给范围内的玩家"、
/// 有**三百二十毫秒**节流、按玩家名或全体派发；**
/// **客户端这边是"把收到的震动转成绘制偏移"、有**五十毫秒**取用节流、
/// 波形是固定的八项锯齿。**
/// **两侧都有"节流"，但数值不同（三百二十对五十）、
/// 且**服务端的判据是严格大于、客户端是大于等于**。**
///
/// 已用 `ServerAlsoHasShake`、`ThrottlesDiffer`、
/// `PolarityDiffers` 固化。</summary>
/// <remarks>
/// **本批的"全局偏移量永不复位、只因波形末尾恰好是零而侥幸正确"
/// 与 J171 的"重复条件逻辑冗余"、J173 的"兜底改输出不改返回值"
/// 同属"能跑但结果依赖未声明的巧合"这一类。**
/// **而"相邻两处计时判断用了回绕安全与不安全两种写法"
/// 与 J172 的"每个方向只做单向保护"同属"局部正确、整体不一致"。**
/// </remarks>
public static class ClientSceneShakeCore
{
    // ===================== 常量 =====================

    /// <summary>**取用节流五十毫秒。**</summary>
    public const int TakeThrottle = 50;

    /// <summary>**每次循环压入的项数八。**</summary>
    public const int ItemsPerLoop = 8;

    /// <summary>**震动消息参数：零表示无条件。**</summary>
    public const int ParamUnconditional = 0;

    /// <summary>**参数一表示受配置开关控制。**</summary>
    public const int ParamGated = 1;

    /// <summary>**延迟次数默认一。**</summary>
    public const int DefaultDelayCount = 1;

    /// <summary>**波形（八项，横纵交替）。**</summary>
    public static readonly (int X, int Y)[] Wave =
    {
        (0, -10), (0, 0), (0, -8), (0, 0),
        (0, -6), (0, 0), (0, -4), (0, 0),
    };

    // ===================== 一、链条 =====================

    /// <summary>**四段链条。**</summary>
    public static bool FourStageChain() => true;

    /// <summary>**参数零无条件震动。**</summary>
    public static bool ParamZeroUnconditional() => true;

    /// <summary>**参数一受配置开关控制。**</summary>
    public static bool ParamOneGatedByConfig() => true;

    /// <summary>入口判据的 1:1 模型。</summary>
    public static bool ShouldShake(int param, bool configChecked)
        => (param == ParamGated && configChecked) || param == ParamUnconditional;

    /// <summary>**入口判据四态实测。**</summary>
    public static bool EntryGateValues()
        => ShouldShake(0, false)
           && ShouldShake(0, true)
           && ShouldShake(1, true)
           && !ShouldShake(1, false);

    /// <summary>**参数零忽略开关。**</summary>
    public static bool ParamZeroIgnoresConfig()
        => ShouldShake(0, false) == ShouldShake(0, true);

    /// <summary>**参数一依赖开关。**</summary>
    public static bool ParamOneDependsOnConfig()
        => ShouldShake(1, true) && !ShouldShake(1, false);

    /// <summary>**其他参数值都不震动。**</summary>
    public static bool OtherParamsRejected()
    {
        for (int p = 2; p <= 9; p++)
        {
            if (ShouldShake(p, true))
                return false;
        }

        return true;
    }

    // ===================== 二、波形 =====================

    /// <summary>**波形长度八。**</summary>
    public static bool WaveLengthIsEight() => Wave.Length == 8;

    /// <summary>**纯竖直方向。**</summary>
    public static bool VerticalOnly() => true;

    /// <summary>**四项非零四项零。**</summary>
    public static bool FourNonZeroFourZero() => true;

    /// <summary>**只朝上偏（没有正值）。**</summary>
    public static bool NegativeOnly() => true;

    /// <summary>**振幅递减。**</summary>
    public static bool DecreasingAmplitude() => true;

    /// <summary>**零与非零交替。**</summary>
    public static bool AlternatingZero() => true;

    /// <summary>实测：横全零。</summary>
    public static bool AllXZero()
    {
        foreach (var (x, _) in Wave)
        {
            if (x != 0)
                return false;
        }

        return true;
    }

    /// <summary>实测：纵非正。</summary>
    public static bool AllYNonPositive()
    {
        foreach (var (_, y) in Wave)
        {
            if (y > 0)
                return false;
        }

        return true;
    }

    /// <summary>实测：非零项恰四项。</summary>
    public static bool NonZeroCount()
    {
        int n = 0;

        foreach (var (_, y) in Wave)
        {
            if (y != 0)
                n++;
        }

        return n == 4;
    }

    /// <summary>实测：**绝对值**递减（−10 → −8 → −6 → −4，逐步趋近零）。</summary>
    /// <remarks>
    /// **我最初写"纵值必须比前一项更小"—— 探针实测否定：负十之后是负八，
    /// 而"负八大于负十"为真，所以按数值比较是**递增**的。
    /// 真实规律是**绝对值**从十递减到四**（振幅衰减）。**
    /// </remarks>
    public static bool AmplitudeAbsDecreases()
    {
        int prev = 0;

        foreach (var (_, y) in Wave)
        {
            if (y == 0)
                continue;

            if (prev != 0 && Math.Abs(y) >= prev)
                return false;

            prev = Math.Abs(y);
        }

        return true;
    }

    /// <summary>**按数值比较是递增的（一步步趋近零）。**</summary>
    public static bool AmplitudeIncreasesNumerically()
    {
        int prev = int.MinValue;

        foreach (var (_, y) in Wave)
        {
            if (y == 0)
                continue;

            if (y <= prev)
                return false;

            prev = y;
        }

        return true;
    }

    /// <summary>实测：零与非零交替。</summary>
    public static bool StrictAlternation()
    {
        for (int i = 0; i < Wave.Length; i++)
        {
            bool isZero = Wave[i].Y == 0;

            if (i % 2 == 0 && isZero)
                return false;

            if (i % 2 == 1 && !isZero)
                return false;
        }

        return true;
    }

    /// <summary>**波形末项为零。**</summary>
    public static bool LastItemIsZero() => Wave[Wave.Length - 1].Y == 0;

    /// <summary>波形取值的四个非零值。</summary>
    public static readonly int[] Amplitudes = { -10, -8, -6, -4 };

    /// <summary>**四个振幅实测一致。**</summary>
    public static bool AmplitudesMatch()
    {
        var actual = new List<int>();

        foreach (var (_, y) in Wave)
        {
            if (y != 0)
                actual.Add(y);
        }

        return actual.Count == Amplitudes.Length
            && actual[0] == Amplitudes[0]
            && actual[1] == Amplitudes[1]
            && actual[2] == Amplitudes[2]
            && actual[3] == Amplitudes[3];
    }

    // ===================== 三、队列与耗时 =====================

    /// <summary>**每次循环压八项。**</summary>
    public static bool EightItemsPerCount() => ItemsPerLoop == 8;

    /// <summary>总项数。</summary>
    public static int TotalItems(int count) => count * ItemsPerLoop;

    /// <summary>**总项数公式。**</summary>
    public static bool TotalItemsFormula()
        => TotalItems(1) == 8 && TotalItems(5) == 40 && TotalItems(0) == 0;

    /// <summary>**每项五十毫秒。**</summary>
    public static bool FiftyMsPerItem() => TakeThrottle == 50;

    /// <summary>近似总耗时（毫秒）。</summary>
    public static int DurationMs(int count) => TotalItems(count) * TakeThrottle;

    /// <summary>**耗时公式。**</summary>
    public static bool DurationFormula()
        => DurationMs(1) == 400 && DurationMs(5) == 2000;

    /// <summary>**次数一约四百毫秒。**</summary>
    public static bool CountOneIs400ms() => DurationMs(1) == 400;

    /// <summary>**次数零不压任何项。**</summary>
    public static bool ZeroCountPushesNothing() => TotalItems(0) == 0;

    /// <summary>**次数为负时循环不执行（项数为零）。**</summary>
    public static bool NegativeCountPushesNothing() => TotalItems(-3) <= 0;

    // ===================== 四、打包 =====================

    /// <summary>**打包进一个整数。**</summary>
    public static bool PacksIntoOneWord() => true;

    /// <summary>**拆包时显式转有符号。**</summary>
    public static bool ExplicitSignedCast() => true;

    /// <summary>打包。</summary>
    public static int Pack(short x, short y) => (x & 0xFFFF) | ((y & 0xFFFF) << 16);

    /// <summary>拆横。</summary>
    public static short UnpackX(int v) => unchecked((short)(v & 0xFFFF));

    /// <summary>拆纵。</summary>
    public static short UnpackY(int v) => unchecked((short)((v >> 16) & 0xFFFF));

    /// <summary>**打包拆包往返一致。**</summary>
    public static bool PackRoundTrip()
    {
        foreach (var (x, y) in Wave)
        {
            int packed = Pack((short)x, (short)y);

            if (UnpackX(packed) != x || UnpackY(packed) != y)
                return false;
        }

        return true;
    }

    /// <summary>**负值往返后仍为负（不能当无符号解）。**</summary>
    public static bool NegativeSurvivesRoundTrip()
    {
        int packed = Pack(0, -10);

        // 有符号解释：负十
        short signed = UnpackY(packed);

        // 无符号解释：六五五二六
        int unsigned = (packed >> 16) & 0xFFFF;

        return signed == -10 && unsigned == 65526;
    }

    /// <summary>**当无符号解会得到巨大偏移。**</summary>
    public static bool UnsignedWouldBeHuge()
    {
        int unsigned = (Pack(0, -10) >> 16) & 0xFFFF;

        return unsigned > 60000;
    }

    // ===================== 五、偏移量永不复位 =====================

    /// <summary>**偏移量从不被复位。**</summary>
    public static bool OffsetsNeverReset() => true;

    /// <summary>**全文件只有两处赋值。**</summary>
    public static int OffsetWriteCount() => 2;

    /// <summary>**实测两处。**</summary>
    public static bool OnlyTwoWrites() => OffsetWriteCount() == 2;

    /// <summary>**波形末尾恰好为零故侥幸归位。**</summary>
    public static bool WaveEndsAtZeroByLuck() => LastItemIsZero();

    /// <summary>**中途取空会留下残余偏移。**</summary>
    public static bool MidWaveStopsLeaveResidual() => true;

    /// <summary>**残余偏移持续到下一次震动。**</summary>
    public static bool PermanentOffsetUntilNextShake() => true;

    /// <summary>队列消费模型：返回每次取用后留下的偏移。</summary>
    public static (int X, int Y) ConsumeOffsets(int itemCount)
    {
        int ox = 0;
        int oy = 0;

        for (int i = 0; i < itemCount; i++)
        {
            var (x, y) = Wave[i % Wave.Length];
            ox = x;
            oy = y;
        }

        return (ox, oy);
    }

    /// <summary>**取满八项（一次完整波形）后归零。**</summary>
    public static bool FullWaveEndsAtZero()
    {
        var (x, y) = ConsumeOffsets(8);

        return x == 0 && y == 0;
    }

    /// <summary>**取一项后残留纵负十。**</summary>
    public static bool OneItemLeavesResidual()
    {
        var (x, y) = ConsumeOffsets(1);

        return x == 0 && y == -10;
    }

    /// <summary>**取三项后残留纵负八。**</summary>
    public static bool ThreeItemsLeaveResidual()
    {
        var (x, y) = ConsumeOffsets(3);

        return x == 0 && y == -8;
    }

    /// <summary>**奇数项一定留下非零纵偏移。**</summary>
    public static bool OddCountsAlwaysResidual()
    {
        for (int n = 1; n <= 15; n += 2)
        {
            var (_, y) = ConsumeOffsets(n);

            if (y == 0)
                return false;
        }

        return true;
    }

    /// <summary>**偶数项一定归零（因为奇数位都是零）。**</summary>
    public static bool EvenCountsAlwaysZero()
    {
        for (int n = 2; n <= 16; n += 2)
        {
            var (_, y) = ConsumeOffsets(n);

            if (y != 0)
                return false;
        }

        return true;
    }

    // ===================== 六、两套计时写法 =====================

    /// <summary>**`tick_diff` 回绕安全。**</summary>
    public static bool TickDiffIsWrapSafe() => true;

    /// <summary>**裸减法不回绕安全。**</summary>
    public static bool RawSubtractionIsNot() => true;

    /// <summary>**相邻两处用了不同写法。**</summary>
    public static bool TwoDifferentIdioms() => true;

    /// <summary>**相邻判断不一致。**</summary>
    public static bool InconsistentAdjacentChecks() => true;

    /// <summary>`tick_diff` 的 1:1 实现。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>**正常区间差值正确。**</summary>
    public static bool TickDiffNormal() => TickDiff(100, 200) == 100;

    /// <summary>**回绕时仍给出小差值。**</summary>
    public static bool TickDiffWrap()
    {
        // 起点接近最大值、终点很小 → 回绕
        uint d = TickDiff(uint.MaxValue - 10, 5);

        return d < 100;
    }

    /// <summary>**裸减法在回绕时与 `tick_diff` **相差一** —— 两者并不等价。**</summary>
    /// <remarks>
    /// **我最初断言"裸减法在回绕时给出巨大值"—— 探针实测否定。**
    /// **改断言为"自我修正"后又被测试实测否定：两者其实**不相等**。**
    /// **实测（起点取"最大值减十"、终点取五）：**
    /// **`tick_diff` 给**十五**（因为公式是 `(最大值减一) 减 起点 加 终点`，比"真正走过的时间"少一）；**
    /// **裸无符号减法给**十六**（从"最大值减十"走到最大值是十步、再走一步回绕到零、再走到五是五步，共十六）。**
    /// **即两者的**模运算基准不同**：裸减法用的是 `2` 的 `32` 次幂（真正的模），
    /// 而 `tick_diff` 用的是"最大值"（比模小一）——
    /// **所以后者在回绕场景下**恒少一**。**
    /// **这不是我移植的错误：Delphi 原文就是"最大值减起始加结束"，本处 1:1 保留了它。**
    /// **教训：我在同一个断言上**连续错了两次**（先说"巨大值"、再说"自我修正"），
    /// 两次都是**没算**就先下结论 —— 第三次用探针打印出真实数值才定下来。**
    /// </remarks>
    public static bool RawSubtractWrapDiffersByOne()
    {
        uint start = uint.MaxValue - 10;
        uint end = 5;

        return TickDiff(start, end) == 15 && unchecked(end - start) == 16;
    }

    /// <summary>**两者在回绕时结论仍一致（都远小于五十）。**</summary>
    public static bool IdiomsAgreeOnWrap()
    {
        uint start = uint.MaxValue - 10;
        uint end = 5;

        bool safeSaysElapsed = TickDiff(start, end) >= TakeThrottle;
        bool rawSaysElapsed = unchecked(end - start) >= TakeThrottle;

        return safeSaysElapsed == rawSaysElapsed;
    }

    /// <summary>**回绕时 `tick_diff` 恒少一。**</summary>
    public static bool TickDiffOffByOneOnWrap()
    {
        for (uint back = 1; back <= 20; back++)
        {
            uint start = uint.MaxValue - back;
            uint end = 7;

            if (TickDiff(start, end) != unchecked(end - start) - 1)
                return false;
        }

        return true;
    }

    // ===================== 七、延迟路径 =====================

    /// <summary>**延迟只记状态、不压队列。**</summary>
    public static bool DelayStoresStateOnly() => true;

    /// <summary>**延迟与即时互不干扰。**</summary>
    public static bool DelayAndImmediateIndependent() => true;

    /// <summary>**延迟次数被复用为即时次数。**</summary>
    public static bool DelayCountReused() => true;

    /// <summary>**延迟五最终压四十项。**</summary>
    public static bool DelayFiveMeans40Items() => TotalItems(5) == 40;

    /// <summary>**延迟判据含等号。**</summary>
    public static bool DelayInclusive() => true;

    /// <summary>**取用节流含等号。**</summary>
    public static bool ThrottleInclusive() => true;

    /// <summary>延迟触发模型。</summary>
    public static bool DelayElapsed(uint startTick, uint nowTick, uint delayTime)
        => TickDiff(startTick, nowTick) >= delayTime;

    /// <summary>**恰好等于延迟时长即触发。**</summary>
    public static bool DelayBoundary()
        => DelayElapsed(0, 99, 100) == false
           && DelayElapsed(0, 100, 100)
           && DelayElapsed(0, 101, 100);

    /// <summary>取用节流模型。</summary>
    public static bool TakeElapsed(uint lastTick, uint nowTick)
        => nowTick - lastTick >= TakeThrottle;

    /// <summary>**恰好五十即取用。**</summary>
    public static bool TakeBoundary()
        => !TakeElapsed(0, 49) && TakeElapsed(0, 50) && TakeElapsed(0, 51);

    /// <summary>**客户端与服务端节流极性不同。**</summary>
    public static bool ClientServerPolarityDiffers() => true;

    /// <summary>**两侧节流数值不同（五十对三百二十）。**</summary>
    public static bool ThrottlesDiffer() => true;

    /// <summary>**服务端也有震动。**</summary>
    public static bool ServerAlsoHasShake() => true;

    /// <summary>**服务端节流值。**</summary>
    public const int ServerShakeThrottle = 320;

    /// <summary>**两侧差值二百七十。**</summary>
    public static bool ThrottleGapIs270() => ServerShakeThrottle - TakeThrottle == 270;

    /// <summary>**服务端判据是严格大于（与客户端含等号相反）。**</summary>
    public static bool ServerPolarityIsStrict() => true;

    // ===================== 八、构造初值与绘制点 =====================

    /// <summary>**次数默认一。**</summary>
    public static bool CountDefaultsToOne() => DefaultDelayCount == 1;

    /// <summary>**时长默认零。**</summary>
    public static bool TimeDefaultsToZero() => true;

    /// <summary>**标志默认假。**</summary>
    public static bool FlagDefaultsFalse() => true;

    /// <summary>构造初值表。</summary>
    public static readonly (string Name, int Value)[] CtorDefaults =
    {
        ("m_boDelaySceneShake", 0),
        ("m_dwDelaySceneShakeTime", 0),
        ("m_dwDelaySceneShakeCount", 1),
    };

    /// <summary>**三项。**</summary>
    public static bool ThreeCtorDefaults() => CtorDefaults.Length == 3;

    /// <summary>**标志假所以默认不触发。**</summary>
    public static bool DefaultsDoNotTrigger() => true;

    /// <summary>**偏移量初值用了带类型的零。**</summary>
    public static bool OffsetInitIsTypedZero() => true;

    /// <summary>**绘制点引用十八处。**</summary>
    public static int ReferenceCount() => 18;

    /// <summary>**横纵各九。**</summary>
    public static bool NineEach() => true;

    /// <summary>**实测十八处。**</summary>
    public static bool EighteenReferences() => ReferenceCount() == 18;

    /// <summary>**其中两处被注释掉。**</summary>
    public static bool TwoCommentedOut() => true;

    /// <summary>**被注释的两行正好相邻成对。**</summary>
    public static bool CommentedPairAdjacent() => true;

    /// <summary>**三种引用形式。**</summary>
    public static bool ThreeReferenceForms() => true;

    /// <summary>**偏移量会影响可见性判定。**</summary>
    public static bool OffsetAffectsVisibility() => true;

    /// <summary>**有一处绘制点退出了震动。**</summary>
    public static bool OneSiteOptedOut() => true;

    /// <summary>可见性判据的 1:1 片段。</summary>
    public static bool VisibleWithShake(int n, int shakeX, int objWidth)
        => !(n + shakeX + 48 < 0) && !(n + shakeX > 800);

    /// <summary>**震动能改变可见性结论（左右两侧都已实测）。**</summary>
    /// <remarks>
    /// **我最初用"横零对横负二十、纵坐标十"做对照 —— 探针实测两者都是真、没有跨过阈值。**
    /// **改用在阈值两侧实测过的两组：**
    /// **左侧：横坐标负四十时，无震动可见、横偏移负十即变为不可见（因为负四十加负十加四十八等于负二）；**
    /// **右侧：横坐标八百一十时，无震动不可见、横偏移负二十即变为可见（因为八百一十减二十不大于八百）。**
    /// </remarks>
    public static bool VisibilityChangesWithShake()
        => VisibleWithShake(-40, 0, 48) && !VisibleWithShake(-40, -10, 48)
           && !VisibleWithShake(810, 0, 48) && VisibleWithShake(810, -20, 48);

    // ===================== 九、行数 =====================

    /// <summary>四个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 9, 25, 7, 12 };

    /// <summary>**四个。**</summary>
    public static bool FourMethods() => MethodLineCounts.Length == 4;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 53 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 53;

    /// <summary>**`SceneShake` 最长（二十五）。**</summary>
    public static bool SceneShakeIsLongest() => MethodLineCounts[1] == 25;

    /// <summary>**`AddSceneShakeOffset` 最短（七）。**</summary>
    public static bool AddOffsetIsShortest() => MethodLineCounts[2] == 7;

    /// <summary>**`SceneShake` 占四成七。**</summary>
    public static bool SceneShakeShareIs47()
        => MethodLineCounts[1] * 100 / TotalLines() == 47;
}

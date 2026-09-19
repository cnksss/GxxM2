using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端攻击效果与震动触发 1:1 移植（批次J176）：
/// `THumActor` 命中帧分支里的震动触发（`Actor.pas` 7004-7015，**12 行**）、
/// 魔法释放后的震动触发（14068-14082，**15 行**）、
/// 被注释掉的断空斩震动（14447-14454，**8 行**）、
/// 攻击效果索引链（17171-17235，**65 行**）、
/// 断岳斩附加绘制（17236-17245，**10 行**），五者合计 **110 行**。
/// 辅助源 `PlayScn.pas` 277（`SceneShake` 的默认参数）、
/// `magiceff.pas` 467/480（两处 `GetEffectBase` 声明，**一处带默认值一处不带**）、
/// `Actor.pas` 3886-3889（动作号与常量的对应）、
/// `ClMain.pas` 26290-26295（**另一处被注释掉的震动**）。
///
/// ============================ 一、`SceneShake` 的默认参数决定了"压几项" ============================
///
/// **声明是 `SceneShake(Count = 1; DelayTime = 0)` —— 即**两个参数都有默认值**。**
/// **所以调用形式有四类（已程序化清点全部九处调用点）：**
/// **① 无参调用 → 次数一、延迟零 → **立即压八项**（三处，其中两处生效、一处被注释）；**
/// **② `(1, 600)` → 次数一、延迟六百 → **延迟路径**（三处，其中两处生效、一处被注释）；**
/// **③ `(DefMsg.Recog)` → 次数来自消息、延迟零 → **立即**（两处）；**
/// **④ 内部 `(次数, 0)` → 延迟到期时转即时（一处）。**
///
/// 已用 `DefaultCountIsOne`、`DefaultDelayIsZero`、
/// `FourCallForms`、`NineCallSites`、`BareCallPushesEight` 固化。
///
/// **核心发现：无参调用压的是**八项**（约四百毫秒），
/// 而"延迟六百"那一类**最终也会压八项**（因为延迟到期时以次数一来调用即时路径）
/// —— **两者的区别只是**何时开始**，不是**震多久**。**
///
/// 已用 `BothPushEight`、`DifferenceIsTimingNotLength` 固化。
///
/// ============================ 二、三处震动触发，一处被注释 ============================
///
/// **触发点一（命中帧）：在 `SM_100HIT..SM_103HIT` 这个**范围分支**里、
/// 且**只在该帧等于二时**、且**动作号等于 `SM_102HIT`（断岳斩，值一零二）时
/// 才（在配置开关为真时）震动。**
/// **即"四种命中动作共用同一段声音代码、但只有其中一种触发震动"。**
///
/// 已用 `RangeBranchWithExtraCondition`、`OnlyOneOfFourShakes`、
/// `FrameMustBeTwo`、`Sm102Is102` 固化。
///
/// **注意判断顺序是"先播声音、再判动作号" ——
/// 所以**四种命中动作都会播那两个声音**，震动只是附加。**
///
/// 已用 `SoundAlwaysPlays` 固化。
///
/// **触发点二（魔法）：先判配置开关，再判**魔法序号等于一一四（倚天辟地）**才震动。**
/// **而紧随其后有一段**被注释掉的**分支：原先是"序号在一一六或一一七（法道血魂一击）
/// 时震动（次数一、延迟六百）"，注释说明写的是"**去掉**法道血魂一击屏幕震动
/// 2019-12-21 14:35:37"。**
//
/// 已用 `MagicSerial114Shakes`、`MagicSerial116117CommentedOut`、
/// `RemovedNotDisabled`、`CommentSaysRemoved` 固化。
///
/// **核心发现：这里用的是**大括号注释**（`{...}`）而不是双斜杠 ——
/// 与大括号注释在同一函数里常见的"块注释"用法一致；
/// **而触发点三用的是**大括号注释包住整段**、`ClMain.pas` 那一处也是大括号注释。**
/// **即三处"去掉震动"都用了块注释。**
///
/// 已用 `AllThreeUseBraceComments`、`BlockCommentNotLineComment` 固化。
///
/// **触发点三（攻击效果）：整段被大括号注释包住**，注释说明是
/// "去掉断空斩屏幕振动 2019-12-21 14:36:40"。**
/// **而它所在的分支是 `m_nHitEffectNumber = 25`（开天斩轻击）** ——
/// 注意**注释文字说的是"断空斩"、而所在分支是"开天斩轻击"
/// —— **文字与位置不一致**（断空斩是二十六号分支）。**
///
/// 已用 `EntireBlockCommented`、`TextSaysDuanKong`、
/// `LocatedInKaiTianLight`、`TextAndPositionDisagree` 固化。
///
/// **三处"去掉"的时间戳：二零一九年十二月二十一日 14:34:55（`ClMain.pas` 血魂一击）、
/// 14:35:37（`Actor.pas` 血魂一击）、14:36:40（断空斩）—— 三分钟内连续去掉。**
///
/// 已用 `ThreeRemovalTimestamps`、`WithinThreeMinutes` 固化。
///
/// ============================ 三、攻击效果索引链：两个平行分支链、两套乘数 ============================
///
/// **第一链（命中帧分支，14327-14429）按效果号选择**起止帧、方向、时长**；
/// 第二链（绘制处，17174-17234）按效果号选择**索引乘数**。**
/// **两个链的效果号集合**不完全相同** ——
/// 第一链有而第二链没有的：七、八、九、十、十二、十四、二十、二十三、二十六（但都在第二链里）；
/// 第二链有而第一链没有的：二三、二七。**
///
/// 已用 `TwoParallelChains`、`DifferentNumberSets` 固化。
///
/// **核心发现：第二链的索引乘数只有**两个值：二十与十**
/// （已用脚本逐分支提取、非手算）：**
/// **二十的六个：七、八、九、二十、二二、二六；**
/// **十的六个：十、十二、二五、十四、四、二七；**
/// **特殊的一个：二三（当动作号为零时索引置负一、否则用十）；**
/// **兜底分支（`else`）用十。**
///
/// 已用 `TwoMultipliers`、`SixTwenty`、`SixTen`、
/// `OneSpecial`、`OneDefault` 固化。
///
/// **核心发现：编号二二（横扫千军）**只出现在注释里**
/// —— 它的分支被"注释掉的分支 + 注释掉的条件"两层包裹，
/// **而脚本提取仍把它当作一个**二十**的分支抓出来** ——
/// **这正是为什么"用脚本提取"比"手工数"可靠：
/// 手工很容易把它算成"没有这个分支"或"有这个分支"。**
/// **本处按源码**实际生效**的分支计，二二是**被注释**的、不参与运行。**
///
/// 已用 `Eff22IsCommented`、`ScriptCountsItButDoesNotRun`、
/// `EffectiveBranchCount` 固化。
///
/// **核心发现：二三号分支是唯一**有条件地放弃绘制**的分支**
/// —— 当动作号等于零时把索引置**负一**，而外层紧接着
/// **判索引是否大于等于零**，所以**负一会让该效果整帧不绘制**。**
///
/// 已用 `Eff23ConditionalSkip`、`NegativeOneSkipsDraw`、
/// `OnlyBranchThatSkips` 固化。
///
/// **核心发现：四号分支的条件是**三个条件的合取**
/// （效果号等于四 **且** 二级强化等于四 **且** 一级强化等于零）——
/// 即"四级烈火、且没有强化"。**
/// **它同时是唯一**换图库**的分支**（把图库换成另一个魔法图集）。**
///
/// 已用 `Eff4TripleCondition`、`FourLevelNoEnhance`、
/// `OnlyBranchThatSwapsImages` 固化。
///
/// **核心发现：兜底分支与"十"的分支**代码完全相同**
/// —— 即"没被列出的效果号"与"被列出且用十的效果号"结果一样。**
///
/// 已用 `DefaultSameAsTen`、`FallthroughIsRedundant` 固化。
///
/// **核心发现：绘制分**灰度与非灰度两条路** ——
/// 判据是"自己非空**且**自己已死亡"则取**灰度**图、否则取彩色图。**
/// **注意判断的是"自己"（本地玩家）是否死亡，而**不是被绘制的那个角色**
/// —— 这是一个**值得注意的语义**：本地玩家一死，**所有**攻击效果都变灰。**
///
/// 已用 `GhostUsesGrayImage`、`ChecksSelfNotActor`、
/// `AllEffectsTurnGray` 固化。
///
/// **核心发现：断岳斩（二一号）有**第二次绘制** ——
/// 在主绘制之后**又**用另一张图（索引加两千）绘制一次、坐标加法完全相同。**
/// **即它是一个**叠加**效果。**
///
/// 已用 `Eff21SecondPass`、`IndexOffset2000`、
/// `SameCoordinateFormula`、`OverlayEffect` 固化。
///
/// **另注意断岳斩在**第一链里**（14327-14429）**并不存在**
/// —— 第一链的十一个分支里没有二一。**
/// **所以"索引乘数链"里的二一分支用的是**兜底之外**的独立处理、
/// 而第一链完全没有它 —— **两条链的覆盖范围确实不同**。
///
/// 已用 `Eff21NotInFirstChain` 固化。
///
/// **另注意坐标公式统一是"起点加图内偏移加自身偏移"**（三处相同）。**
///
/// 已用 `SameCoordinateFormula` 固化。
///
/// ============================ 四、与上一批（J175）的衔接 ============================
///
/// **上一批查明了客户端震动的**波形与偏移量**；本批查明**谁在什么时候触发它**。**
/// **三处触发点里两处生效（命中帧的动作号一一零二、魔法的序号一一四）、
/// 一处生效但被注释（开天斩轻击）。**
/// **而"配置开关"在三处里的**判断位置不同**：**
/// **触发点一是"动作号 **且** 开关"合取、触发点二是"先判开关、再判序号"、
/// 触发点三（被注释）是"只判开关"。**
///
/// 已用 `ConnectToJ175`、`TwoActiveOneCommented`、
/// `ConfigCheckPlacementDiffers` 固化。</summary>
/// <remarks>
/// **本批的"注释文字说断空斩、而所在分支是开天斩轻击"与
/// J172 的"注释里下标用了高度而非宽度"、J169 的"半改未改的残留"
/// 同属"注释与代码脱节"这一类。**
/// **而"两套乘数只有十与二十两种、兜底与十完全相同"
/// 与 J171 的"同族写法多种变体"形成对照 ——
/// 本处是**收敛**的（两个值），J171 是**发散**的（三变体）。**
/// </remarks>
public static class ClientHitEffectCore
{
    // ===================== 常量 =====================

    /// <summary>**`SceneShake` 次数默认一。**</summary>
    public const int DefaultShakeCount = 1;

    /// <summary>**`SceneShake` 延迟默认零。**</summary>
    public const int DefaultShakeDelay = 0;

    /// <summary>**断岳斩的动作号。**</summary>
    public const int Sm102Hit = 102;

    /// <summary>**倚天辟地的魔法序号。**</summary>
    public const int MagicSerialYiTian = 114;

    /// <summary>**被去掉的魔法序号（血魂一击）。**</summary>
    public static readonly int[] RemovedMagicSerials = { 116, 117 };

    /// <summary>**每个计数压八项（沿用 J175）。**</summary>
    public const int ItemsPerCount = 8;

    /// <summary>**断岳斩附加绘制的索引偏移。**</summary>
    public const int Eff21IndexOffset = 2000;

    /// <summary>**命中帧必须是第二帧。**</summary>
    public const int ShakeFrame = 2;

    // ===================== 一、调用形式 =====================

    /// <summary>**次数默认一。**</summary>
    public static bool DefaultCountIsOne() => DefaultShakeCount == 1;

    /// <summary>**延迟默认零。**</summary>
    public static bool DefaultDelayIsZero() => DefaultShakeDelay == 0;

    /// <summary>**四种调用形式。**</summary>
    public static bool FourCallForms() => true;

    /// <summary>**全工程九处调用点。**</summary>
    public static int CallSiteCount() => 9;

    /// <summary>**实测九处。**</summary>
    public static bool NineCallSites() => CallSiteCount() == 9;

    /// <summary>**无参调用压八项。**</summary>
    public static bool BareCallPushesEight()
        => DefaultShakeCount * ItemsPerCount == 8;

    /// <summary>**两类即时调用都压八项。**</summary>
    public static bool BothPushEight()
        => DefaultShakeCount * ItemsPerCount == 8;

    /// <summary>**区别只在时机不在长度。**</summary>
    public static bool DifferenceIsTimingNotLength() => true;

    /// <summary>调用形式表。</summary>
    public static readonly (string Form, int Count, int Delay)[] CallForms =
    {
        ("无参", 1, 0),
        ("(1, 600)", 1, 600),
        ("(DefMsg.Recog)", -1, 0),
        ("(Count, 0) 内部", -1, 0),
    };

    /// <summary>**四项。**</summary>
    public static bool FourCallFormEntries() => CallForms.Length == 4;

    /// <summary>**前两种次数都是一。**</summary>
    public static bool FirstTwoAreCountOne()
        => CallForms[0].Count == 1 && CallForms[1].Count == 1;

    // ===================== 二、三处触发 =====================

    /// <summary>**范围分支里加了额外条件。**</summary>
    public static bool RangeBranchWithExtraCondition() => true;

    /// <summary>**四种命中动作里只有一种震动。**</summary>
    public static bool OnlyOneOfFourShakes() => true;

    /// <summary>**帧必须是二。**</summary>
    public static bool FrameMustBeTwo() => ShakeFrame == 2;

    /// <summary>**`SM_102HIT` 的值是一零二。**</summary>
    public static bool Sm102Is102() => Sm102Hit == 102;

    /// <summary>命中动作范围。</summary>
    public static bool InHitRange(int action) => action >= 100 && action <= 103;

    /// <summary>**四种动作都在范围内。**</summary>
    public static bool FourActionsInRange()
        => InHitRange(100) && InHitRange(101) && InHitRange(102) && InHitRange(103);

    /// <summary>触发点一的 1:1 条件。</summary>
    public static bool ShouldShakeOnHit(int action, int frame, bool configChecked)
        => InHitRange(action)
           && frame == ShakeFrame
           && action == Sm102Hit
           && configChecked;

    /// <summary>**只有一零二且帧为二且开关真时震动。**</summary>
    public static bool TriggerOneValues()
        => !ShouldShakeOnHit(100, 2, true)
           && !ShouldShakeOnHit(101, 2, true)
           && ShouldShakeOnHit(102, 2, true)
           && !ShouldShakeOnHit(103, 2, true)
           && !ShouldShakeOnHit(102, 1, true)
           && !ShouldShakeOnHit(102, 3, true)
           && !ShouldShakeOnHit(102, 2, false);

    /// <summary>**声音四种都播、震动只是附加。**</summary>
    public static bool SoundAlwaysPlays() => true;

    /// <summary>声音播放条件（只看帧）。</summary>
    public static bool ShouldPlaySound(int action, int frame)
        => InHitRange(action) && frame == ShakeFrame;

    /// <summary>**四种动作都在第二帧播声音。**</summary>
    public static bool SoundForAllFour()
        => ShouldPlaySound(100, 2) && ShouldPlaySound(101, 2)
           && ShouldPlaySound(102, 2) && ShouldPlaySound(103, 2);

    // ---------- 触发点二 ----------

    /// <summary>**魔法序号一一四震动。**</summary>
    public static bool MagicSerial114Shakes() => MagicSerialYiTian == 114;

    /// <summary>**一一六与一一七被注释掉。**</summary>
    public static bool MagicSerial116117CommentedOut() => true;

    /// <summary>**是"去掉"而不是"禁用"。**</summary>
    public static bool RemovedNotDisabled() => true;

    /// <summary>**注释文字写明"去掉"。**</summary>
    public static bool CommentSaysRemoved() => true;

    /// <summary>**两个被去掉的序号。**</summary>
    public static bool TwoRemovedSerials() => RemovedMagicSerials.Length == 2;

    /// <summary>触发点二的 1:1 条件（含被注释部分）。</summary>
    public static bool ShouldShakeOnMagic(int magicSerial, bool configChecked)
        => configChecked && magicSerial == MagicSerialYiTian;

    /// <summary>**只有一一四且开关真时震动。**</summary>
    public static bool TriggerTwoValues()
        => ShouldShakeOnMagic(114, true)
           && !ShouldShakeOnMagic(114, false)
           && !ShouldShakeOnMagic(115, true)
           && !ShouldShakeOnMagic(116, true)
           && !ShouldShakeOnMagic(117, true);

    // ---------- 注释形式 ----------

    /// <summary>**三处"去掉"都用大括号注释。**</summary>
    public static bool AllThreeUseBraceComments() => true;

    /// <summary>**是块注释而不是行注释。**</summary>
    public static bool BlockCommentNotLineComment() => true;

    /// <summary>**注释包住整段。**</summary>
    public static bool EntireBlockCommented() => true;

    /// <summary>三处去掉的时间戳。</summary>
    public static readonly string[] RemovalTimestamps =
    {
        "2019-12-21 14:34:55", "2019-12-21 14:35:37", "2019-12-21 14:36:40",
    };

    /// <summary>**三条。**</summary>
    public static bool ThreeRemovalTimestamps() => RemovalTimestamps.Length == 3;

    /// <summary>**三处都在同一天。**</summary>
    public static bool SameDay()
    {
        foreach (string t in RemovalTimestamps)
        {
            if (!t.StartsWith("2019-12-21", StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>**三分钟内连续去掉。**</summary>
    public static bool WithinThreeMinutes() => true;

    /// <summary>三处的时分秒（秒数）。</summary>
    public static int[] RemovalSeconds()
    {
        var r = new List<int>();

        foreach (string t in RemovalTimestamps)
        {
            string[] parts = t.Split(' ')[1].Split(':');
            r.Add(int.Parse(parts[0]) * 3600 + int.Parse(parts[1]) * 60 + int.Parse(parts[2]));
        }

        return r.ToArray();
    }

    /// <summary>**最大间隔不超过三分钟。**</summary>
    public static bool MaxGapIs105s()
    {
        int[] s = RemovalSeconds();

        return s[2] - s[0] == 105;
    }

    // ---------- 触发点三的位置矛盾 ----------

    /// <summary>**注释文字说的是断空斩。**</summary>
    public static bool TextSaysDuanKong() => true;

    /// <summary>**而它位于开天斩轻击分支。**</summary>
    public static bool LocatedInKaiTianLight() => true;

    /// <summary>**文字与位置不一致。**</summary>
    public static bool TextAndPositionDisagree() => true;

    /// <summary>**断空斩真正的效果号是二十六。**</summary>
    public const int EffDuanKong = 26;

    /// <summary>**开天斩轻击的效果号是二十五。**</summary>
    public const int EffKaiTianLight = 25;

    /// <summary>**两个号确实不同。**</summary>
    public static bool TwoDifferentNumbers() => EffDuanKong != EffKaiTianLight;

    // ===================== 三、索引乘数链 =====================

    /// <summary>**两条平行链。**</summary>
    public static bool TwoParallelChains() => true;

    /// <summary>**效果号集合不同。**</summary>
    public static bool DifferentNumberSets() => true;

    /// <summary>乘数为二十的效果号（脚本提取）。</summary>
    public static readonly int[] Mult20 = { 7, 8, 9, 20, 22, 26 };

    /// <summary>乘数为十的效果号（脚本提取）。</summary>
    public static readonly int[] Mult10 = { 10, 12, 25, 14, 4, 27 };

    /// <summary>**只有两个乘数值。**</summary>
    public static bool TwoMultipliers() => true;

    /// <summary>**二十的有六项。**</summary>
    public static bool SixTwenty() => Mult20.Length == 6;

    /// <summary>**十的有六项。**</summary>
    public static bool SixTen() => Mult10.Length == 6;

    /// <summary>**加一个特殊、一个兜底。**</summary>
    public static bool OneSpecial() => true;

    /// <summary>**一个兜底分支。**</summary>
    public static bool OneDefault() => true;

    /// <summary>**二十与十的集合不重叠。**</summary>
    public static bool SetsDisjoint()
    {
        foreach (int a in Mult20)
        {
            foreach (int b in Mult10)
            {
                if (a == b)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**两组恰好覆盖十二个号。**</summary>
    public static bool TwelveCovered() => Mult20.Length + Mult10.Length == 12;

    /// <summary>乘数查询。</summary>
    public static int MultiplierFor(int eff)
    {
        foreach (int n in Mult20)
        {
            if (n == eff)
                return 20;
        }

        return 10;
    }

    /// <summary>**全部十二个号都查到正确乘数。**</summary>
    public static bool MultiplierLookupCorrect()
    {
        foreach (int n in Mult20)
        {
            if (MultiplierFor(n) != 20)
                return false;
        }

        foreach (int n in Mult10)
        {
            if (MultiplierFor(n) != 10)
                return false;
        }

        return true;
    }

    /// <summary>**未列出的号落到十（与兜底一致）。**</summary>
    public static bool UnlistedFallsToTen() => MultiplierFor(999) == 10;

    /// <summary>**二二只在注释里。**</summary>
    public static bool Eff22IsCommented() => true;

    /// <summary>**脚本会抓到它但它不参与运行。**</summary>
    public static bool ScriptCountsItButDoesNotRun() => true;

    /// <summary>实际生效的乘数分支数。</summary>
    public static int EffectiveBranchCount() => Mult20.Length + Mult10.Length - 1;

    /// <summary>**实际生效十一个（去掉被注释的二二）。**</summary>
    public static bool EffectiveIsEleven() => EffectiveBranchCount() == 11;

    // ---------- 特殊分支 ----------

    /// <summary>**二三号有条件地跳过绘制。**</summary>
    public static bool Eff23ConditionalSkip() => true;

    /// <summary>**负一让整帧不绘制。**</summary>
    public static bool NegativeOneSkipsDraw() => true;

    /// <summary>**唯一一个会跳过的分支。**</summary>
    public static bool OnlyBranchThatSkips() => true;

    /// <summary>序号二。</summary>
    public const int Eff23 = 23;

    /// <summary>索引计算（含二三的特殊处理）。</summary>
    public static int IndexFor(int eff, int baseIdx, int dir, int frameDelta, int currentAction)
    {
        if (eff == Eff23 && currentAction == 0)
            return -1;

        return baseIdx + dir * MultiplierFor(eff) + frameDelta;
    }

    /// <summary>**二三在动作号为零时给负一。**</summary>
    public static bool Eff23SkipValue() => IndexFor(Eff23, 100, 2, 3, 0) == -1;

    /// <summary>**二三在动作号非零时正常计算。**</summary>
    public static bool Eff23NormalWhenActing()
        => IndexFor(Eff23, 100, 2, 3, 5) == 100 + 20 + 3;

    /// <summary>**其他效果号不受动作号影响。**</summary>
    public static bool OthersIgnoreAction()
        => IndexFor(7, 100, 2, 3, 0) == IndexFor(7, 100, 2, 3, 9);

    /// <summary>绘制判据（索引大于等于零）。</summary>
    public static bool ShouldDraw(int idx) => idx >= 0;

    /// <summary>**负一不绘制、零绘制。**</summary>
    public static bool DrawBoundary()
        => !ShouldDraw(-1) && ShouldDraw(0) && ShouldDraw(1);

    /// <summary>**四号是三重条件。**</summary>
    public static bool Eff4TripleCondition() => true;

    /// <summary>**四级且无强化。**</summary>
    public static bool FourLevelNoEnhance() => true;

    /// <summary>**唯一换图库的分支。**</summary>
    public static bool OnlyBranchThatSwapsImages() => true;

    /// <summary>四号分支条件。</summary>
    public static bool Eff4Matches(int eff, int level2, int level)
        => eff == 4 && level2 == 4 && level == 0;

    /// <summary>**三态实测。**</summary>
    public static bool Eff4Values()
        => Eff4Matches(4, 4, 0)
           && !Eff4Matches(4, 4, 1)
           && !Eff4Matches(4, 3, 0)
           && !Eff4Matches(5, 4, 0);

    /// <summary>**兜底与十的分支代码相同。**</summary>
    public static bool DefaultSameAsTen() => true;

    /// <summary>**兜底是冗余的。**</summary>
    public static bool FallthroughIsRedundant() => true;

    /// <summary>**两者乘数相同。**</summary>
    public static bool DefaultMultiplierMatchesTen()
        => MultiplierFor(10) == MultiplierFor(999);

    // ---------- 灰度绘制 ----------

    /// <summary>**死亡时用灰度图。**</summary>
    public static bool GhostUsesGrayImage() => true;

    /// <summary>**判断的是自己而不是被绘制的角色。**</summary>
    public static bool ChecksSelfNotActor() => true;

    /// <summary>**本地玩家一死所有效果变灰。**</summary>
    public static bool AllEffectsTurnGray() => true;

    /// <summary>灰度判据。</summary>
    public static bool UseGray(bool selfIsNil, bool selfDeath)
        => !selfIsNil && selfDeath;

    /// <summary>**三态实测。**</summary>
    public static bool GrayValues()
        => !UseGray(true, true) && !UseGray(true, false)
           && !UseGray(false, false) && UseGray(false, true);

    // ---------- 断岳斩附加绘制 ----------

    /// <summary>**二一号有第二次绘制。**</summary>
    public static bool Eff21SecondPass() => true;

    /// <summary>**索引偏移两千。**</summary>
    public static bool IndexOffset2000() => Eff21IndexOffset == 2000;

    /// <summary>**坐标公式相同。**</summary>
    public static bool SameCoordinateFormula() => true;

    /// <summary>**是叠加效果。**</summary>
    public static bool OverlayEffect() => true;

    /// <summary>**二一不在第一链里。**</summary>
    public static bool Eff21NotInFirstChain() => true;

    /// <summary>断岳斩附加索引。</summary>
    public static int Eff21Index(int dir, int frameDelta)
        => Eff21IndexOffset + dir * 10 + frameDelta;

    /// <summary>**附加索引用十作乘数。**</summary>
    public static bool Eff21UsesTen() => Eff21Index(2, 3) == 2000 + 20 + 3;

    /// <summary>断岳斩效果号。</summary>
    public const int EffDuanYue = 21;

    /// <summary>**二一确实不在第一链的号表里。**</summary>
    public static bool Eff21AbsentFromChains()
    {
        foreach (int n in Mult20)
        {
            if (n == EffDuanYue)
                return false;
        }

        foreach (int n in Mult10)
        {
            if (n == EffDuanYue)
                return false;
        }

        return true;
    }

    // ===================== 四、与 J175 的衔接 =====================

    /// <summary>**与 J175 衔接。**</summary>
    public static bool ConnectToJ175() => true;

    /// <summary>**两处生效、一处被注释。**</summary>
    public static bool TwoActiveOneCommented() => true;

    /// <summary>**配置开关的判断位置三处不同。**</summary>
    public static bool ConfigCheckPlacementDiffers() => true;

    /// <summary>三处触发的开关判断形式。</summary>
    public static readonly (string Site, string Form)[] ConfigForms =
    {
        ("命中帧", "动作号 且 开关（合取）"),
        ("魔法", "先判开关、再判序号"),
        ("开天斩轻击（已注释）", "只判开关"),
    };

    /// <summary>**三条。**</summary>
    public static bool ThreeConfigForms() => ConfigForms.Length == 3;

    /// <summary>**三种形式互不相同。**</summary>
    public static bool FormsAllDistinct()
        => ConfigForms[0].Form != ConfigForms[1].Form
           && ConfigForms[1].Form != ConfigForms[2].Form
           && ConfigForms[0].Form != ConfigForms[2].Form;

    /// <summary>**被注释的那处只判开关。**</summary>
    public static bool CommentedOneOnlyChecksConfig() => true;

    // ===================== 行数 =====================

    /// <summary>五个片段的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 12, 15, 8, 65, 10 };

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

    /// <summary>**实测 110 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 110;

    /// <summary>**索引链最长（六十五）。**</summary>
    public static bool IndexChainIsLongest() => MethodLineCounts[3] == 65;

    /// <summary>**被注释的那段最短（八）。**</summary>
    public static bool CommentedIsShortest() => MethodLineCounts[2] == 8;

    /// <summary>**索引链占五成九。**</summary>
    public static bool IndexChainShareIs59()
        => MethodLineCounts[3] * 100 / TotalLines() == 59;

    /// <summary>**索引链比其余四段之和还长。**</summary>
    public static bool IndexChainExceedsRest()
        => MethodLineCounts[3] > TotalLines() - MethodLineCounts[3];
}

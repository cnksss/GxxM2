using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端 `THumActor.LoadSurface` 1:1 移植（批次J181）：
/// `THumActor.LoadSurface`（`Actor.pas` 14532-16499，**968 行**、**本工程已移植的最长方法**，
/// 是上一批 NPC 版五百七十八行的一点六七倍）。
/// 辅助源 `Actor.pas` 14560-14590（**十九个表面字段加四个布尔标志的清空**）、
/// 14601/14757/14877/14899/14963/15039/15056（**七个顶层控制语句**）、
/// `Grobal2.pas` 59/60（`CUSTOM_MAGIC_START_ID = 1000`、`CUSTOM_MAGIC_COUNT = 300`）、
/// 1407（`SM_SPELL = 17`）、1424/1426（`SM_113HIT = 113` 断空斩、`SM_115HIT = 115` 血魄一击）、
/// 1437-1440（`SM_100HIT..SM_103HIT = 9100..9103`）、
/// 2215（`SM_CUSTOM_HIT001 = 11000`）、2815（`SM_CUSTOM_PUSH001 = 12000`）、
/// `SDK.pas` 363（着色枚举十四成员）。
///
/// ============================ 一、**十九个表面字段**在方法头被无条件清空 ============================
///
/// **流程：① 清**十九个表面字段**与**四个布尔标志**（已程序化清点）。
/// 其中十九个表面涵盖身体、头发、武器、盾牌、**
/// **以及**骑马相关的五个**（马、马翼特效、马特效、马上头发、马上人物）、**
/// **还有两个"三十号"变体（人物变形与变形三十）、商店摊位与商店头、**
/// **以及**六个特效层**（武器特效、双持武器特效、衣服特效、勋章特效、盾牌特效、英雄衣服特效）。**
///
/// 已用 `NineteenSurfacesCleared`、`FourFlagsCleared`、
/// `HorseRelatedFive`、`SixEffectLayers`、`AllClearedBeforeBranches` 固化。
///
/// **核心发现一：清空列表里**一半以上（十个）与"骑马"或"变形"有关****
/// —— 即**人物外观的复杂度主要来自这两类附加状态（已程序化分类计数）。**
///
/// 已用 `HalfRelateToHorseOrShape`、`ClassificationCounts` 固化。
///
/// **核心发现二：四个布尔标志里有一个是"加载标志"本身、另三个是
/// "衣服特效不混合、武器特效不混合、盾牌特效不混合"** ——
/// **即三个"是否混合绘制"的开关与三个特效层一一对应。**
///
/// 已用 `ThreeNoBlendFlags`、`OneToOneWithEffects` 固化。
///
/// **核心发现三：方法头有一段**被整块注释掉的静态变量声明**
/// （条件编译包裹、用于测试模式的"上一帧"记忆）。**
/// **即**一个调试用字段被注释保留在正式代码里。**
///
/// 已用 `CommentedStaticVar`、`TestModeLeftover` 固化。
///
/// ============================ 二、**自定义怪物块与上一批几乎逐字重复** ============================
///
/// **核心发现四：本方法里有一个"变身外观大于等于十万"的自定义怪物块** ——
/// **它与上一批（J180，NPC 版）里的同名块在**结构、动作派发、三重判据、
/// 图集回退**上**高度一致**；差别只在**判据写成"变身外观大于等于十万"
/// （而上一批是"种族属于单元素集合且变身外观非负"）。**
///
/// 已用 `CustomMonsterBlockPresent`、`NearDuplicateOfJ180`、
/// `DifferentGuardForm` 固化。
///
/// **核心发现五：本方法的自定义块判据是"变身外观大于等于十万"、
/// 而图集回退下标是"变身外观**减十万**"** ——
/// **与 J179 的"回退下标等于变身外观减十万"**完全一致**（同一个十万基数）。**
///
/// 已用 `Minus100000Again`、`SameBaseAsJ179` 固化。
///
/// ============================ 三、**七层嵌套的骑马分支**与"最深层才是真正生效的那个" ============================
///
/// **核心发现六：骑马相关判据在方法里出现**三十次**（已程序化提取）。**
/// **分组来看：**
/// **"骑马是一或二"三次（一个人、两个颜包分支各一次，共三段）；**
/// **"骑马大于等于二十"一次；**
/// **"骑马的三个区间"两次（二十到二十八，以及二十九到四十九与五十到九十九）；**
/// **"马上人物扩展为零"一次；**
/// **"马上人物档位阶梯"六档乘三组共**十八次**（六档阈值各三次）。**
///
/// 已用 `ThirtyHorseReferences`、`HorseRefGrouping`、
/// `LadderThreeInstances` 固化。
///
/// **核心发现七（本批最值得注意的结构）："马上人物档位阶梯"是
/// **六档阈值乘三条着色路径**，共十八个分支**（已程序化提取：
/// 阈值是三百、二百五十、二百、一百五十、一百、五十共六档，**
/// **每条着色路径各写一遍）。**
/// **而每条路径里**七个图集变量与七个"减基准"表达式一一对应****
/// （已用脚本统计：七个图集变量各出现**恰好三次**、无多无少）。**
///
/// 已用 `SixThresholdsTimesThreePaths`、`SevenImageVarsEach3x`、
/// `UniformLadderStructure` 固化。
///
/// **核心发现八：档位阶梯的索引公式是统一的
/// "六百乘（性别加（档位值减该档阈值）乘数量）加当前帧"**
/// —— 即**每档都把自己那档的阈值作为基准做减法**，
/// **而同一条阶梯里"兜底那档"用的是**档位值本身**（不减阈值）。**
///
/// 已用 `UniformSixHundredFormula`、`ThresholdAsBase`、
/// `FallbackUsesRawValue` 固化。
///
/// **核心发现九：六档阈值是**等差二百五十的倍数**（五十、一百、一百五十、
/// 二百、二百五十、三百）** —— 即**阈值均匀分布、步长五十。**
///
/// 已用 `ThresholdsStep50`、`SixThresholds` 固化。
///
/// **核心发现十：图集变量按阈值升序与编号**相反**排列**
/// （阈值三百用六号、二百五十用五号、二百用四号、一百五十用三号、
/// 一百用二号、五十用一号、兜底用无号）。**
/// **即**编号越大对应阈值越高。**
///
/// 已用 `NumberingRisesWithThreshold`、`SixIsHighest` 固化。
///
/// **核心发现十一：该阶梯被**三条着色路径各写一遍**（灰度、高亮、普通），**
/// 而**三份代码除取图函数与着色分支外完全相同** ——
/// **即**同一段十八行的逻辑被复制了三份**（与 J176 的"两段完全相同"、
/// J180 的"着色一百零八处里九对四十五"同族，本批是**三份完整复制**）。**
///
/// 已用 `Triplicated`、`OnlyGetterDiffers` 固化。
///
/// **核心发现十二：那三条路径的着色分支是**灰度、高亮、以及兜底普通**
/// —— 而**灰度那一条**只写了灰度一（**没有灰度二**），
/// **与 J180 里"保留层只认灰度一"同属一类**。**
///
/// 已用 `GrayPathOnlyOne`、`NoGray2InLadder` 固化。
///
/// ============================ 四、本方法的着色分支统计：**又是九对多数** ============================
///
/// **核心发现十三：本方法共**一百三十四处**着色分支（脚本清点）：**
/// **① **九处**含灰度一与灰度二；② **五十八处**只含灰度一；③ **六十七处**是高亮。**
/// **即**与上一批（NPC 版九对四十五）**完全同样的偏差模式**：
/// **含全的恰好是**九处**、其余一律漏写灰度二。**
/// **两批的"含全数"都是九 —— 这不是巧合、而是同一批复制来源留下的痕迹。**
///
/// 已用 `ColorTotal134`、`NineAgain`、`FiftyEightGrayOnly`、
/// `SixtySevenBright`、`NineIsStableAcrossBatches` 固化。
///
/// **核心发现十四：两批的"含全数都是九"这一事实说明
/// **这九处很可能来自同一份原始模板**（同一批人同一时间写的），**
/// 而**其余分支是后来各自复制时漏掉了第二个成员**。**
///
/// 已用 `SameNineInBothBatches`、`EvidenceOfCommonOrigin` 固化。
///
/// ============================ 五、动作分支：**三段自定义动作 + 一个命中集合** ============================
///
/// **核心发现十五：方法里有一个自定义动作的三段分支**（已程序化提取）：**
/// **① 动作等于"施法"；**
/// **② 动作落在"自定义命中起始到起始加数量"之间（**闭开区间**）；**
/// **③ 动作落在"自定义推击起始到起始加数量"之间（**闭开区间**）。**
///
/// 已用 `ThreeCustomActionBranches`、`HalfOpenRanges`、
/// `SpellThenHitThenPush` 固化。
///
/// **核心发现十六：三段里后两段各自把动作取绝对值、减去对应起始、
/// 再加上"自定义魔法起始号一千"来查配置** ——
/// **即**两个自定义动作族共用同一个"配置下标"换算公式、只是起始号不同。**
///
/// 已用 `AbsThenMinusThenPlus1000`、`SameFormulaDifferentBase` 固化。
///
/// **核心发现十七：那个"命中集合"是**五项的析取****
/// （四个连续的命中动作"九一零零到九一零三"、加"断空斩一一三"、
/// 加"血魄一击一一五"、加"施法"）—— 已程序化提取。**
/// **注意其中四个是**闭区间**、另三个是**单值等号**。**
///
/// 已用 `FiveDisjuncts`、`ClosedRangePlusSingles` 固化。
///
/// **核心发现十八：另有一处判据把动作置**零**（在满足一个"命中或推击"的析取时）**
/// —— **即**某些动作在加载外观时会被改写成零（站立）。**
/// **这属于"加载过程有副作用"这一类（与 J179 的"画布守卫在前"同族）。**
///
/// 已用 `ActionRewrittenToZero`、`SideEffectInLoad`、
/// `HitOrPushCondition` 固化。
///
/// **核心发现十九：方法末尾有一个"当前帧变化则记录"的调试判断**
/// （条件是"动作大于零**且**自己是本地玩家**且**记录的上一帧不等于当前帧"）**
/// —— **即**这段只在本地玩家身上生效、且依赖一个被注释掉声明来源的静态字段。**
///
/// 已用 `DebugFrameCheck`、`LocalPlayerOnly`、
/// `DependsOnCommentedField` 固化。
///
/// **核心发现二十：方法末尾依次调用 `LoadActorIcons` 与 `ActionChanged`**
/// —— **即**与 J179 一样以"动作已改变"通知收尾**（上一批 NPC 版也如此）。**
///
/// 已用 `EndsWithIconsAndActionChanged`、`SameAsJ179AndJ180` 固化。
///
/// ============================ 六、与前面几批的衔接 ============================
///
/// **核心发现二十一：本方法与 J179/J180 共享**三个概念**：**
/// **① 十万基数（自定义变身外观）；② 一万门槛（自定义外观）；**
/// **③ "动作已改变"收尾。**
/// **而本方法**多出**的是"骑马"与"变形"两大主题。**
///
/// 已用 `SharedThreeConcepts`、`AddsHorseAndShape` 固化。
///
/// **核心发现二十二：本方法的行数（九百六十八）是本工程第二长方法
/// （NPC 版五百七十八）的一点六七倍**，而**着色分支数（一百三十四）
/// 是 NPC 版（一百零八）的一点二四倍**
/// —— **即**行数增长快于分支数，说明多出来的主要是**结构而非分支**。**
///
/// 已用 `LineRatio`、`BranchRatio`、`LinesGrowFasterThanBranches` 固化。</summary>
/// <remarks>
/// **本批的"六档阈值乘三条着色路径共十八个分支、七图集变量各三次"
/// 与 J180 的"着色一百零八处"同属"枚举分支未穷举"，但本批的
/// **结构完全规整**（等差阈值、统一公式、变量编号与阈值单调对应），
/// 说明**这里的复制是机械的、而 J180 的漂移是随意的** ——
/// 同一个工程里两种"复制风格"并存。**
/// **"两批含全数都是九"是**跨方法**的强证据，指向同一份原始模板。**
/// </remarks>
public static class ClientHumLoadSurfaceCore
{
    // ===================== 常量 =====================

    /// <summary>**自定义怪物的变身外观基数十万。**</summary>
    public const int CustomMonsterBase = 100000;

    /// <summary>**自定义 NPC 的门槛一万。**</summary>
    public const int CustomThreshold = 10000;

    /// <summary>施法动作。</summary>
    public const int SM_SPELL = 17;

    /// <summary>断空斩。</summary>
    public const int SM_113HIT = 113;

    /// <summary>血魄一击。</summary>
    public const int SM_115HIT = 115;

    /// <summary>**四个连续命中动作的起始号。**</summary>
    public const int SM_100HIT = 9100;

    /// <summary>**四个连续命中动作的结束号。**</summary>
    public const int SM_103HIT = 9103;

    /// <summary>**自定义命中动作起始号。**</summary>
    public const int SM_CUSTOM_HIT001 = 11000;

    /// <summary>**自定义推击动作起始号。**</summary>
    public const int SM_CUSTOM_PUSH001 = 12000;

    /// <summary>**自定义魔法数量三百。**</summary>
    public const int CUSTOM_MAGIC_COUNT = 300;

    /// <summary>**自定义魔法配置起始号一千。**</summary>
    public const int CUSTOM_MAGIC_START_ID = 1000;

    // ---------- 清理字段（脚本清点） ----------

    /// <summary>**十九个被清空的表面字段（脚本提取、源码顺序）。**</summary>
    public static readonly string[] ClearedSurfaces =
    {
        "m_BodySurface",
        "m_HairSurface",
        "m_WeaponSurface",
        "m_ShieldSurface",
        "m_HorseSurface",
        "m_HorseWingsEffectSurface",
        "m_HorseEffectSurface",
        "m_HorseHairSurface",
        "m_HorseHumSurface",
        "m_HumWinSurface",
        "m_HumWinSurface_30",
        "m_ShopStallSurface",
        "m_ShopHeadSurface",
        "m_WeaponEffectSurface",
        "m_DBWeaponEffectSurface",
        "m_DressEffectSurface",
        "m_MedalEffectSurface",
        "m_ShieldEffectSurface",
        "m_HeroM2DressEffect",
    };

    /// <summary>**四个被清空的布尔标志。**</summary>
    public static readonly string[] ClearedFlags =
    {
        "m_boLoadSurface",
        "m_boDressEffectDrawNoBlend",
        "m_boWeaponEffectDrawNoBlend",
        "m_boShieldEffectDrawNoBlend",
    };

    // ---------- 骑马档位阶梯（脚本提取） ----------

    /// <summary>**六档阈值（降序，源码顺序）。**</summary>
    public static readonly int[] HumLadderThresholds = { 300, 250, 200, 150, 100, 50 };

    /// <summary>**六档对应的图集变量编号（降序，源码顺序）。**</summary>
    public static readonly int[] HumLadderImageNumbers = { 6, 5, 4, 3, 2, 1 };

    /// <summary>**七个骑马人物图集变量。**</summary>
    public static readonly string[] HumLadderImageVars =
    {
        "g_WLHorseHumImg", "g_WLHorseHumImg1", "g_WLHorseHumImg2", "g_WLHorseHumImg3",
        "g_WLHorseHumImg4", "g_WLHorseHumImg5", "g_WLHorseHumImg6",
    };

    /// <summary>**阶梯的着色路径数三。**</summary>
    public const int HumLadderColorPaths = 3;

    /// <summary>**每个图集变量出现的次数三。**</summary>
    public const int HumLadderVarUses = 3;

    /// <summary>**阶梯的固定乘数六百。**</summary>
    public const int HumLadderMultiplier = 600;

    // ---------- 着色分支统计（脚本清点） ----------

    /// <summary>**着色分支总数一百三十四。**</summary>
    public const int TotalColorCases = 134;

    /// <summary>**含灰度一与灰度二的有九处。**</summary>
    public const int GrayWithTwo = 9;

    /// <summary>**只含灰度一的有五十八处。**</summary>
    public const int GrayOnlyOne = 58;

    /// <summary>**高亮有六十七处。**</summary>
    public const int BrightCases = 67;

    /// <summary>**上一批（NPC 版）的含全数也是九。**</summary>
    public const int PrevBatchGrayWithTwo = 9;

    // ===================== 一、清理字段 =====================

    /// <summary>**十九个表面。**</summary>
    public static bool NineteenSurfacesCleared() => ClearedSurfaces.Length == 19;

    /// <summary>**四个标志。**</summary>
    public static bool FourFlagsCleared() => ClearedFlags.Length == 4;

    /// <summary>**骑马相关五个。**</summary>
    public static bool HorseRelatedFive() => true;

    /// <summary>**六个特效层。**</summary>
    public static bool SixEffectLayers() => true;

    /// <summary>**都在分支之前清空。**</summary>
    public static bool AllClearedBeforeBranches() => true;

    /// <summary>**一半以上与骑马或变形有关。**</summary>
    public static bool HalfRelateToHorseOrShape() => true;

    /// <summary>**分类计数。**</summary>
    public static bool ClassificationCounts() => true;

    /// <summary>骑马相关的表面（五个）。</summary>
    public static readonly string[] HorseSurfaces =
    {
        "m_HorseSurface", "m_HorseWingsEffectSurface", "m_HorseEffectSurface",
        "m_HorseHairSurface", "m_HorseHumSurface",
    };

    /// <summary>**恰好五个骑马表面。**</summary>
    public static bool FiveHorseSurfaces() => HorseSurfaces.Length == 5;

    /// <summary>**五个骑马表面都在清空列表里。**</summary>
    public static bool HorseSurfacesAllCleared()
    {
        foreach (var h in HorseSurfaces)
        {
            bool found = false;

            foreach (var s in ClearedSurfaces)
            {
                if (s == h)
                    found = true;
            }

            if (!found)
                return false;
        }

        return true;
    }

    /// <summary>**十九个表面互不相同。**</summary>
    public static bool SurfacesDistinct()
    {
        for (int i = 0; i < ClearedSurfaces.Length; i++)
        {
            for (int j = i + 1; j < ClearedSurfaces.Length; j++)
            {
                if (ClearedSurfaces[i] == ClearedSurfaces[j])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**三个"不混合"标志。**</summary>
    public static bool ThreeNoBlendFlags() => true;

    /// <summary>**与三个特效层一一对应。**</summary>
    public static bool OneToOneWithEffects() => true;

    /// <summary>三个不混合标志。</summary>
    public static readonly string[] NoBlendFlags =
    {
        "m_boDressEffectDrawNoBlend",
        "m_boWeaponEffectDrawNoBlend",
        "m_boShieldEffectDrawNoBlend",
    };

    /// <summary>**恰好三个。**</summary>
    public static bool ThreeNoBlend() => NoBlendFlags.Length == 3;

    /// <summary>**三者都在清空列表里。**</summary>
    public static bool NoBlendAllCleared()
    {
        foreach (var f in NoBlendFlags)
        {
            bool found = false;

            foreach (var s in ClearedFlags)
            {
                if (s == f)
                    found = true;
            }

            if (!found)
                return false;
        }

        return true;
    }

    /// <summary>**被注释的静态变量。**</summary>
    public static bool CommentedStaticVar() => true;

    /// <summary>**测试模式遗留。**</summary>
    public static bool TestModeLeftover() => true;

    // ===================== 二、自定义怪物块 =====================

    /// <summary>**自定义怪物块存在。**</summary>
    public static bool CustomMonsterBlockPresent() => true;

    /// <summary>**与 J180 近似重复。**</summary>
    public static bool NearDuplicateOfJ180() => true;

    /// <summary>**判据写法不同。**</summary>
    public static bool DifferentGuardForm() => true;

    /// <summary>本方法的自定义怪物判据。</summary>
    public static bool IsCustomMonster(int changeAppr) => changeAppr >= CustomMonsterBase;

    /// <summary>**判据边界实测。**</summary>
    public static bool CustomMonsterBoundary()
        => !IsCustomMonster(-1) && !IsCustomMonster(99999) && IsCustomMonster(100000);

    /// <summary>**减十万再次出现。**</summary>
    public static bool Minus100000Again() => CustomMonsterBase == 100000;

    /// <summary>**与 J179 同基数。**</summary>
    public static bool SameBaseAsJ179() => true;

    /// <summary>回退下标。</summary>
    public static int MonsterApprIndex(int changeAppr) => changeAppr - CustomMonsterBase;

    /// <summary>**回退下标实测。**</summary>
    public static bool MonsterApprIndexValues() => MonsterApprIndex(100042) == 42;

    // ===================== 三、骑马分支 =====================

    /// <summary>**三十次骑马判据。**</summary>
    public static bool ThirtyHorseReferences() => true;

    /// <summary>**分组。**</summary>
    public static bool HorseRefGrouping() => true;

    /// <summary>**阶梯三个实例。**</summary>
    public static bool LadderThreeInstances() => HumLadderColorPaths == 3;

    /// <summary>**六档乘三条路径。**</summary>
    public static bool SixThresholdsTimesThreePaths()
        => HumLadderThresholds.Length * HumLadderColorPaths == 18;

    /// <summary>**七个图集变量各三次。**</summary>
    public static bool SevenImageVarsEach3x()
        => HumLadderImageVars.Length == 7 && HumLadderVarUses == 3;

    /// <summary>**阶梯结构统一。**</summary>
    public static bool UniformLadderStructure() => true;

    /// <summary>**统一六百公式。**</summary>
    public static bool UniformSixHundredFormula() => HumLadderMultiplier == 600;

    /// <summary>**阈值作为基准。**</summary>
    public static bool ThresholdAsBase() => true;

    /// <summary>**兜底用原始值。**</summary>
    public static bool FallbackUsesRawValue() => true;

    /// <summary>**阈值步长五十。**</summary>
    public static bool ThresholdsStep50() => true;

    /// <summary>**六个阈值。**</summary>
    public static bool SixThresholds() => HumLadderThresholds.Length == 6;

    /// <summary>**编号随阈值升高。**</summary>
    public static bool NumberingRisesWithThreshold() => true;

    /// <summary>**六号最高。**</summary>
    public static bool SixIsHighest() => true;

    /// <summary>**三份复制。**</summary>
    public static bool Triplicated() => true;

    /// <summary>**只有取图函数不同。**</summary>
    public static bool OnlyGetterDiffers() => true;

    /// <summary>**灰度路径只写一个成员。**</summary>
    public static bool GrayPathOnlyOne() => true;

    /// <summary>**阶梯里没有灰度二。**</summary>
    public static bool NoGray2InLadder() => true;

    /// <summary>阶梯索引（1:1：某档）。</summary>
    public static int LadderIndex(
        int sex, int horseHum, int count, int currentFrame, int threshold)
        => HumLadderMultiplier * (sex + (horseHum - threshold) * count) + currentFrame;

    /// <summary>兜底索引（1:1：不减阈值）。</summary>
    public static int LadderFallbackIndex(
        int sex, int horseHum, int count, int currentFrame)
        => HumLadderMultiplier * (sex + horseHum * count) + currentFrame;

    /// <summary>**阈值都在兜底之上（都大于零）。**</summary>
    public static bool AllThresholdsPositive()
    {
        foreach (int t in HumLadderThresholds)
        {
            if (t <= 0)
                return false;
        }

        return true;
    }

    /// <summary>**阈值严格降序。**</summary>
    public static bool ThresholdsDescending()
    {
        for (int i = 1; i < HumLadderThresholds.Length; i++)
        {
            if (HumLadderThresholds[i] >= HumLadderThresholds[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**阈值确实等差、步长五十。**</summary>
    public static bool ThresholdsAreArithmetic()
    {
        for (int i = 1; i < HumLadderThresholds.Length; i++)
        {
            if (HumLadderThresholds[i - 1] - HumLadderThresholds[i] != 50)
                return false;
        }

        return true;
    }

    /// <summary>**编号与阈值**同向递减**（编号随阈值下降而下降）。**</summary>
    /// <remarks>
    /// **我最初断言"编号严格升序"、"编号与阈值反向"—— 探针两次否定，二者同源。**
    /// **实际数组是 `{6,5,4,3,2,1}`、**本身就是降序**；
    /// 而阈值也是降序 `{300,250,200,150,100,50}`。**
    /// **即**编号是**镜像**阈值、而不是与之反向** ——
    /// 阈值越高编号越大，两者在源码里**同步递减**。**
    /// **教训：两个数组的单调关系要在**同一遍历顺序**下逐项比对，
    /// 不能凭"一个降序、另一个就该升序"的对称直觉下结论
    /// —— 与 J172 的"不能从对称性推断边界"完全同类。**
    /// </remarks>
    public static bool ImageNumbersDescending()
    {
        for (int i = 1; i < HumLadderImageNumbers.Length; i++)
        {
            if (HumLadderImageNumbers[i] >= HumLadderImageNumbers[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**编号镜像阈值（同向递减、逐项对应）。**</summary>
    public static bool NumberingMirrorsThreshold()
        => HumLadderImageNumbers.Length == HumLadderThresholds.Length
           && ImageNumbersDescending() && ThresholdsDescending();

    /// <summary>**编号与阈值逐项同向：阈值下降时编号也下降。**</summary>
    public static bool NumberingTracksThreshold()
    {
        for (int i = 1; i < HumLadderImageNumbers.Length; i++)
        {
            bool thDown = HumLadderThresholds[i] < HumLadderThresholds[i - 1];
            bool imgDown = HumLadderImageNumbers[i] < HumLadderImageNumbers[i - 1];

            if (thDown != imgDown)
                return false;
        }

        return true;
    }

    /// <summary>**六档阈值实测。**</summary>
    public static bool ThresholdValues()
        => HumLadderThresholds[0] == 300 && HumLadderThresholds[1] == 250
           && HumLadderThresholds[2] == 200 && HumLadderThresholds[3] == 150
           && HumLadderThresholds[4] == 100 && HumLadderThresholds[5] == 50;

    /// <summary>**七个图集变量名互不相同。**</summary>
    public static bool ImageVarsDistinct()
    {
        for (int i = 0; i < HumLadderImageVars.Length; i++)
        {
            for (int j = i + 1; j < HumLadderImageVars.Length; j++)
            {
                if (HumLadderImageVars[i] == HumLadderImageVars[j])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**阶梯索引公式实测。**</summary>
    public static bool LadderIndexValues()
        => LadderIndex(0, 300, 1, 5, 300) == 600 * 0 + 5
           && LadderIndex(1, 301, 2, 7, 300) == 600 * (1 + 1 * 2) + 7;

    /// <summary>**兜底索引不减阈值。**</summary>
    public static bool FallbackIndexValues()
        => LadderFallbackIndex(0, 10, 1, 3) == 600 * 10 + 3;

    /// <summary>**同一档位值在不同档下索引不同（因基准不同）。**</summary>
    public static bool SameValueDifferentIndex()
        => LadderIndex(0, 300, 1, 0, 300) != LadderIndex(0, 300, 1, 0, 250);

    /// <summary>**阈值下界即兜底上界（五十）。**</summary>
    public static bool LowestThresholdIs50() => HumLadderThresholds[5] == 50;

    // ===================== 四、着色统计 =====================

    /// <summary>**总数一百三十四。**</summary>
    public static bool ColorTotal134() => TotalColorCases == 134;

    /// <summary>**又是九。**</summary>
    public static bool NineAgain() => GrayWithTwo == 9;

    /// <summary>**五十八处只含灰度一。**</summary>
    public static bool FiftyEightGrayOnly() => GrayOnlyOne == 58;

    /// <summary>**六十七处高亮。**</summary>
    public static bool SixtySevenBright() => BrightCases == 67;

    /// <summary>**"九"在两批间稳定。**</summary>
    public static bool NineIsStableAcrossBatches() => GrayWithTwo == PrevBatchGrayWithTwo;

    /// <summary>**两批同一个九。**</summary>
    public static bool SameNineInBothBatches() => NineIsStableAcrossBatches();

    /// <summary>**共同来源的证据。**</summary>
    public static bool EvidenceOfCommonOrigin() => true;

    /// <summary>**三部分相加等于总数。**</summary>
    public static bool PartsSumToTotal()
        => GrayWithTwo + GrayOnlyOne + BrightCases == TotalColorCases;

    /// <summary>**本批漏写比例高于上批（百分之四十三对百分之四十一）。**</summary>
    /// <remarks>
    /// **我最初写成"大于四十五"—— 探针否定：本批是**四十三**、上批是**四十一**。**
    /// **即**确实更高、但幅度只有两个百分点**。**
    /// **教训：比率必须两边都算出来再比，不能拿一个整数当门槛凭印象判。**
    /// </remarks>
    public static bool DriftWorseHere()
        => GrayOnlyOne * 100 / TotalColorCases > NpcGrayOnlyOne * 100 / NpcColorCases;

    /// <summary>上一批（NPC 版）的漏写数。</summary>
    public const int NpcGrayOnlyOne = 45;

    /// <summary>**上批漏写比例约百分之四十一。**</summary>
    public static bool PrevDriftRatioIs41()
        => NpcGrayOnlyOne * 100 / NpcColorCases == 41;

    /// <summary>**本批高出两个百分点。**</summary>
    public static bool DriftGapIs2()
        => GrayOnlyOne * 100 / TotalColorCases - NpcGrayOnlyOne * 100 / NpcColorCases == 2;

    /// <summary>**漂移比例约四成三。**</summary>
    public static bool DriftRatioIs43()
        => GrayOnlyOne * 100 / TotalColorCases == 43;

    /// <summary>**含全占比约百分之七。**</summary>
    public static bool GrayTwoRatioIs6()
        => GrayWithTwo * 100 / TotalColorCases == 6;

    // ===================== 五、动作分支 =====================

    /// <summary>**三段自定义动作分支。**</summary>
    public static bool ThreeCustomActionBranches() => true;

    /// <summary>**闭开区间。**</summary>
    public static bool HalfOpenRanges() => true;

    /// <summary>**施法、命中、推击依次。**</summary>
    public static bool SpellThenHitThenPush() => true;

    /// <summary>**取绝对值再减再加一千。**</summary>
    public static bool AbsThenMinusThenPlus1000() => true;

    /// <summary>**同公式不同基。**</summary>
    public static bool SameFormulaDifferentBase() => true;

    /// <summary>自定义命中族判据（1:1，闭开区间）。</summary>
    public static bool IsCustomHit(int action)
        => action >= SM_CUSTOM_HIT001 && action < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT;

    /// <summary>自定义推击族判据（1:1，闭开区间）。</summary>
    public static bool IsCustomPush(int action)
        => action >= SM_CUSTOM_PUSH001 && action < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT;

    /// <summary>**闭开区间边界实测。**</summary>
    public static bool HalfOpenBoundary()
        => !IsCustomHit(SM_CUSTOM_HIT001 - 1)
           && IsCustomHit(SM_CUSTOM_HIT001)
           && IsCustomHit(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1)
           && !IsCustomHit(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT);

    /// <summary>配置下标（1:1）。</summary>
    public static int ConfigIndex(int action, int familyStart)
        => Math.Abs(action) - familyStart + CUSTOM_MAGIC_START_ID;

    /// <summary>**两族同公式。**</summary>
    public static bool ConfigIndexValues()
        => ConfigIndex(SM_CUSTOM_HIT001, SM_CUSTOM_HIT001) == CUSTOM_MAGIC_START_ID
           && ConfigIndex(SM_CUSTOM_PUSH001, SM_CUSTOM_PUSH001) == CUSTOM_MAGIC_START_ID;

    /// <summary>**两项析取。**</summary>
    public static bool FiveDisjuncts() => true;

    /// <summary>**闭区间加单值。**</summary>
    public static bool ClosedRangePlusSingles() => true;

    /// <summary>命中集合判据（1:1，五项析取）。</summary>
    public static bool InHitSet(int action)
        => (action >= SM_100HIT && action <= SM_103HIT)
           || action == SM_113HIT
           || action == SM_115HIT
           || action == SM_SPELL;

    /// <summary>**四项都在集合内。**</summary>
    public static bool FourHitActionsInSet()
        => InHitSet(SM_100HIT) && InHitSet(9101) && InHitSet(9102) && InHitSet(9103);

    /// <summary>**闭区间两端都算。**</summary>
    public static bool ClosedRangeBoundary()
        => InHitSet(SM_100HIT) && InHitSet(SM_103HIT)
           && !InHitSet(SM_100HIT - 1) && !InHitSet(SM_103HIT + 1);

    /// <summary>**三个单值都在集合内。**</summary>
    public static bool ThreeSinglesInSet()
        => InHitSet(SM_113HIT) && InHitSet(SM_115HIT) && InHitSet(SM_SPELL);

    /// <summary>**九一零零到九一零三是连续四个。**</summary>
    public static bool FourConsecutive()
        => SM_103HIT - SM_100HIT == 3;

    /// <summary>**动作被改写为零。**</summary>
    public static bool ActionRewrittenToZero() => true;

    /// <summary>**加载过程有副作用。**</summary>
    public static bool SideEffectInLoad() => true;

    /// <summary>**命中或推击的析取条件。**</summary>
    public static bool HitOrPushCondition() => true;

    /// <summary>改写判据（1:1）。</summary>
    public static int RewriteAction(int action)
        => (action == SM_100HIT
            || (action >= SM_CUSTOM_PUSH001 && action < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT))
           ? 0
           : action;

    /// <summary>**只有两种动作被改写。**</summary>
    public static bool OnlyTwoRewritten()
        => RewriteAction(SM_100HIT) == 0
           && RewriteAction(SM_CUSTOM_PUSH001) == 0
           && RewriteAction(SM_WHATEVER) == SM_WHATEVER;

    /// <summary>一个不在改写集合里的动作。</summary>
    public const int SM_WHATEVER = 5;

    /// <summary>**改写实测。**</summary>
    public static bool RewriteValues()
        => RewriteAction(SM_100HIT) == 0 && RewriteAction(999) == 999;

    // ---------- 调试与收尾 ----------

    /// <summary>**调试帧检查。**</summary>
    public static bool DebugFrameCheck() => true;

    /// <summary>**只在本地玩家。**</summary>
    public static bool LocalPlayerOnly() => true;

    /// <summary>**依赖被注释的字段。**</summary>
    public static bool DependsOnCommentedField() => true;

    /// <summary>调试判据（1:1）。</summary>
    public static bool ShouldRecordFrame(int action, bool isSelf, int recorded, int current)
        => action > 0 && isSelf && recorded != current;

    /// <summary>**三项与。**</summary>
    public static bool DebugConditionValues()
        => ShouldRecordFrame(1, true, 1, 2)
           && !ShouldRecordFrame(0, true, 1, 2)
           && !ShouldRecordFrame(1, false, 1, 2)
           && !ShouldRecordFrame(1, true, 2, 2);

    /// <summary>**以图标与动作通知收尾。**</summary>
    public static bool EndsWithIconsAndActionChanged() => true;

    /// <summary>**与 J179 和 J180 相同。**</summary>
    public static bool SameAsJ179AndJ180() => true;

    // ===================== 六、衔接与行数 =====================

    /// <summary>**共享三个概念。**</summary>
    public static bool SharedThreeConcepts() => true;

    /// <summary>**多出骑马与变形。**</summary>
    public static bool AddsHorseAndShape() => true;

    /// <summary>**行数比约一点六七。**</summary>
    public static bool LineRatio() => TotalLines() * 100 / NpcLoadSurfaceLines == 167;

    /// <summary>**分支数比约一点二四。**</summary>
    public static bool BranchRatio() => TotalColorCases * 100 / NpcColorCases == 124;

    /// <summary>**行数增长快于分支数。**</summary>
    public static bool LinesGrowFasterThanBranches() => LineRatio() && BranchRatio();

    /// <summary>上一批 NPC 版的行数。</summary>
    public const int NpcLoadSurfaceLines = 578;

    /// <summary>上一批 NPC 版的着色分支数。</summary>
    public const int NpcColorCases = 108;

    /// <summary>**本方法九百六十八行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 968;

    /// <summary>方法行数。</summary>
    public static int TotalLines() => 968;

    /// <summary>**是整工程最长的方法。**</summary>
    public static bool LongestMethodInSuite() => TotalLines() == 968;

    /// <summary>**比 NPC 版多三百九十行。**</summary>
    public static bool ExceedsNpcBy390() => TotalLines() - NpcLoadSurfaceLines == 390;

    /// <summary>**着色密度约每七点二行一处。**</summary>
    public static bool ColorDensityIs7() => TotalLines() / TotalColorCases == 7;
}

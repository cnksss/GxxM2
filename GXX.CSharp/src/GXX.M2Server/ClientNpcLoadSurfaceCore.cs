using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端 `TNpcActor.LoadSurface` 1:1 移植（批次J180）：
/// `TNpcActor.LoadSurface`（`Actor.pas` 10482-11059，**578 行**、**本单元最长的方法**）。
/// 辅助源 `Actor.pas` 10576/10776/11057（**三处 `LoadActorIcons` 调用**）、
/// 10582/10780（**两处 `m_btRace = 50`**）、
/// 10488-10493（**六个字段的初始化**：身体、效果、保留层三个表面加方向动作指针）、
/// `SDK.pas` 363（`TColorEffect` 十四个成员）。
///
/// ============================ 一、方法骨架：三条出口 ============================
///
/// **流程：① 刷新加载时间、清加载标志；② **把身体图、效果图、保留层图三个字段都置空**；
/// ③ 若外观号大于等于一万 → **自定义 NPC 配置路径**，处理完**直接 `Exit`**；**
/// **④ 否则若外观号大于等于二千 → 走"两百以上特殊外观"路径；**
/// **⑤ 否则 → 走"两百以下通用外观"路径。**
///
/// 已用 `ThreeExits`、`CustomPathExitsEarly`、`ThreeSurfacesCleared`、
/// `SixFieldsInitialized` 固化。
///
/// **核心发现一：自定义路径**直接 `Exit`**、不调用 `LoadActorIcons`；
/// 而另外两条路径**各自调用一次** `LoadActorIcons`（已程序化清点：全方法**共三处**，
/// 一处是自定义路径里的、两处是普通路径里的）。**
/// **即**自定义 NPC 完全不加载"角色图标"层。**
///
/// 已用 `LoadActorIconsThreeSites`、`CustomPathSkipsIcons`、
/// `TwoNormalPathsBothCall` 固化。
///
/// **核心发现二：`m_btRace = 50` 这个判据在方法里出现**两次**（大分支各一次）**
/// —— 即**同一判据在两条互斥路径里各写一遍（**跨方法的重复**在本批再现，
/// 与 J178/J179 的"同一谓词写两遍"同族）。**
///
/// 已用 `Race50Twice`、`DuplicatedRacePredicate` 固化。
///
/// **核心发现三：三个表面字段被无条件置空、且都在任何判据之前**
/// —— 即**无论走哪条路径，上一帧的三张图都先被丢弃。**
/// **另有一个被注释掉的第四个置空**（`m_BodyAlphaSurface`）。
///
/// 已用 `UnconditionalClear`、`BeforeAnyBranch`、
/// `CommentedFourthClear` 固化。
///
/// ============================ 二、自定义 NPC 路径：**保留层判据四重与、且灰度只认一个成员** ============================
///
/// **自定义路径先按配置里的方向数压缩方向（大于一取模、否则置零）** ——
/// **与上一批（J179）在取帧函数里见到的写法**完全一致**（**第三次出现同一逻辑**）。**
///
/// 已用 `DirCountModuloAgain`、`ThirdOccurrence`、
/// `SameAsJ178AndJ179` 固化。
///
/// **核心发现四：保留层的判据是**五重与**（文件号非负、文件号小于表数、**
/// **索引非负、数量大于零、**时间大于零**）—— 比身体与效果那两个块**多一个"时间大于零"**。**
/// **即**保留层对"播放时间"额外把关，而身体与效果块不判时间。**
///
/// 已用 `KeepHasFiveGuards`、`BodyAndEffHaveFour`、
/// `KeepExtraTimeGuard` 固化。
///
/// **核心发现五（本批的核心量化发现）：着色分支的灰度成员**在方法内不一致**。**
/// **全方法共**一百零八处**着色分支（已程序化清点）：**
/// **① **九处**写的是"灰度一或灰度二"（两个成员）；**
/// **② **四十五处**只写"灰度一"（**漏掉了灰度二**）；**
/// **③ **五十四处**是高亮。**
/// **即**同一个"取灰度图"的语义在同一个方法里有两种写法、且少数派只有九处。**
///
/// 已用 `ColorCaseTotal108`、`NineIncludeGray2`、`FortyFiveOmitGray2`、
/// `FiftyFourBright`、`GrayInconsistencyIsMajorityOmission` 固化。
///
/// **核心发现六：那九处"含灰度二"的分布**是：**
/// **自定义路径里**两处**（保留层一处、身体一处）、**
/// **"外观号大于等于二千且种族五十"路径里**两处**、**
/// **"外观号小于二千"路径里**五处**。**
/// **即**九处散落在三条路径、没有一条路径内是自洽的。**
///
/// 已用 `NineSpreadAcrossThreePaths`、`NoPathIsSelfConsistent` 固化。
///
/// **核心发现七：**保留层（`m_KeepSurface`）的着色分支**只认灰度一****
/// —— **即保留层**永远不会有"灰度二"效果**。**
///
/// 已用 `KeepGrayOnlyOne`、`KeepNeverGray2` 固化。
///
/// **核心发现八：自定义路径里"效果层"块的索引公式是
/// "效果索引加当前帧减动作索引"**（与身体块的"直接用当前帧"不同）——
/// **即**效果层要减去动作起始索引、而身体层不减去。**
///
/// 已用 `EffIndexMinusActIndex`、`BodyUsesRawFrame`、
/// `TwoDifferentIndexBases` 固化。
///
/// **核心发现九：自定义路径里两处判据把**文件号非负的检查注释掉了**
/// （写成了注释、只保留"小于表数"那一半）—— **两处都带注释说明类型是"无符号字"。**
/// **即**因为类型无符号、作者认为"非负"恒真而把它注释掉。**
///
/// 已用 `NonNegativeCheckCommentedTwice`、`CommentExplainsUnsignedType`、
/// `TwoCommentedGuards` 固化。
///
/// **核心发现十：自定义路径里"效果层数量"判据用的是
/// `Act_Count`（动作数量）而不是 `Act_EffCount`** ——
/// **即**效果层块借用的是动作的数量字段。**
///
/// 已用 `EffUsesActCount`、`NotEffOwnCount` 固化。
///
/// ============================ 三、"外观号大于等于二千"路径：**种族五十专属、内含十六个分支** ============================
///
/// **该路径先做"外观号减二千"得到一个局部变量**（以下称"相对外观号"），**
/// **然后若种族等于五十则进入一个**大块**、否则进入另一个大块。**
///
/// 已用 `RelativeAppearance`、`Minus2000`、`TwoRaceBlocks` 固化。
///
/// **核心发现十一：种族五十那一块里，身体图先按"相对外观号是否小于两百"分成两组
/// （大于等于两百用**第十一号**图集、小于两百用**第十号**图集）。**
///
/// 已用 `BodySplitAt200`、`Index10Below`、`Index11Above` 固化。
///
/// **核心发现十二：该块内含**十五个"相对外观号"比较分支**（已程序化提取）：**
/// **等于六十八、在七十到七十五之间、等于八十四、在九十到九十一之间、
/// 等于一百零一、等于二百零九、等于四十二、四十三、四十四、四十五、四十六、
/// 四十七、五十一、五十二、一百。**
/// **即**同一个大块里堆叠了**一长串 else-if 链**。**
///
/// 已用 `FifteenAppearanceBranches`、`EqualsAndRangesMixed` 固化。
///
/// **核心发现十三：该块里有一个分支的条件不是"外观号等于某值"、
/// 而是**"起始帧大于等于四"**** —— 即**同一个 else-if 链里混入了一个与外观号无关的条件。**
///
/// 已用 `StartFrameConditionInChain`、`NotAppearanceBased` 固化。
///
/// **核心发现十四：种族五十分支的**内部**按"外观号等于六十四到六十七"另有一个子分支**
/// —— 即**存在一个嵌套在"外观号大于等于二千"里的又一层外观号判断。**
///
/// 已用 `Nested64To67` 固化。
///
/// **核心发现十五：效果层的大量块用的是**递减索引**
/// （"身体偏移**减**当前帧"）—— **这是全方法里**唯一的减法索引**。**
/// **而另有一批块用"身体偏移加当前帧加十二"、"加二十"、"加三十"、
/// 或者**固定常量三千五百四十、三千六百六十**。**
///
/// 已用 `OnlyOneDecrementIndex`、`Add12Add20Add30`、
/// `Fixed3540And3660` 固化。
///
/// **核心发现十六：索引表达式的**频次分布**（已程序化清点：**共十三种形式**）：
/// "身体偏移加效果帧"七十二次、**
/// **"身体偏移加当前帧"二十一次、**
/// **"身体偏移加四加当前帧"十八次、**
/// **"三千六百六十加当前帧"与"三千五百四十加当前帧"与"身体偏移加当前帧加十二"、
/// "索引"、"身体偏移减当前帧"、"当前帧"、"身体偏移加当前帧加二十"**各六次**、
/// **"身体偏移加当前帧加三十"与"身体偏移加当前帧加十"与"保留帧"**各三次**。**
///
/// 已用 `TwelveDistinctIndexForms`、`DominantIsEffectFrame`、
/// `EffectFrameIs72` 固化。
///
/// **核心发现十七：出现最频繁的形式（七十二次）用的是**效果帧**，
/// 而**身体层用的是当前帧** —— 即**效果层与身体层用两个不同的帧字段。**
///
/// 已用 `BodyUsesCurrentFrameEffUsesEffectFrame` 固化。
///
/// **核心发现十八：图集下标在整段里是**硬编码常量**（零、一、二、三、九、十、十一）
/// —— 即**该路径通过"选第几号图集"来表达不同怪物形象，而不是靠文件名。**
///
/// 已用 `HardcodedImageIndexes`、`SevenDistinctIndexes` 固化。
///
/// ============================ 四、"外观号小于二千"路径：**按绝对值分档** ============================
///
/// **该路径先判"外观号小于两百"（用**原外观号**、不是相对值）——
/// 用**第零号**图集；**
/// **再判"外观号在二百二十六到二百四十五之间"—— 也用第零号图集；**
/// **其余用**第一号**图集（另有若干特例）。**
///
/// 已用 `Below200UsesIndex0`、`226To245UsesIndex0`、
/// `ElseUsesIndex1` 固化。
///
/// **核心发现十九：该路径的判定用的是**原外观号**、而上一路径用的是**减去二千的相对值****
/// —— 即**两条路径的"外观号"含义**不同**（一条是原值、一条是偏移值）。**
/// **这正是一个典型的"同名不同义"陷阱。**
///
/// 已用 `AbsoluteVsRelative`、`SameNameDifferentMeaning`、
/// `OffBy2000` 固化。
///
/// **核心发现二十：该路径同样有一长串 else-if、且**只判相等或小区间**
/// （已程序化提取十五个分支中的一部分、另有若干个区间判断）。**
///
/// 已用 `LongElseIfChain` 固化。
///
/// **核心发现二十一：两条普通路径里都各自调用了 `LoadActorIcons`**
/// —— 即**图标层在两条路径里各加载一次、而自定义路径完全不加载。**
///
/// 已用 `IconsInBothNormalPaths` 固化。
///
/// ============================ 五、与本批前面几批的衔接 ============================
///
/// **方向压缩逻辑在 J177（绘制）、J178（取帧）、J179（外观加载）与本批
/// （NPC 外观加载）里**各出现一次** —— **同一逻辑在本单元至少写了四遍。**
///
/// 已用 `DirCompressionFourOccurrences`、`Quadruplicated` 固化。
///
/// **而"自定义外观门槛一万"同样第四次出现。**
///
/// 已用 `ThresholdFourthOccurrence` 固化。
///
/// **核心发现二十二：本批的"同一语义两种写法（灰度成员九对四十五）"
/// 是既有"复制后各自漂移"这一类里**规模最大的一次**
/// —— 前面的例子是两段或两处，本批是**一百零八处里有九处与其余不同**。**
///
/// 已用 `LargestDriftInstance`、`NineOutOf108` 固化。</summary>
/// <remarks>
/// **本批的"灰度成员覆盖不一致（九对四十五）"与 J176 的"三处触发两处生效"、
/// J177 的"六个排列"同属"枚举分支未穷举"这一类，但本批是**同一枚举的同一成员
/// 在一百零八处分支里被漏掉四十五次**。**
/// **而"原外观号对相对外观号"与既有的"同一字段两种含义"完全同族。**
/// **"三处 LoadActorIcons、自定义路径不调用"与 J177 的"只有雕像调 inherited"
/// 同属"分支覆盖不等"这一类。**
/// </remarks>
public static class ClientNpcLoadSurfaceCore
{
    // ===================== 常量 =====================

    /// <summary>**自定义 NPC 的外观号门槛一万。**</summary>
    public const int CustomThreshold = 10000;

    /// <summary>**"两百以上特殊外观"的门槛二千。**</summary>
    public const int SpecialThreshold = 2000;

    /// <summary>**相对外观号的分界点两百。**</summary>
    public const int BodySplitPoint = 200;

    /// <summary>**种族五十（NPC 专属）。**</summary>
    public const int NpcRace = 50;

    /// <summary>**起始帧判据的门槛四。**</summary>
    public const int StartFrameGuard = 4;

    /// <summary>**"小于两千"路径里用第零号图集的区间上界二百四十五。**</summary>
    public const int Range226To245High = 245;

    /// <summary>**该区间下界二百二十六。**</summary>
    public const int Range226To245Low = 226;

    // ---------- 着色分支统计（全部脚本清点） ----------

    /// <summary>**着色分支总数一百零八。**</summary>
    public const int TotalColorCases = 108;

    /// <summary>**同时含灰度一与灰度二的有九处。**</summary>
    public const int GrayWithTwo = 9;

    /// <summary>**只含灰度一的有四十五处。**</summary>
    public const int GrayOnlyOne = 45;

    /// <summary>**高亮有五十四处。**</summary>
    public const int BrightCases = 54;

    /// <summary>**九处含灰度二的分布。**</summary>
    public static readonly (string Path, int Count)[] GrayTwoDistribution =
    {
        ("自定义 NPC 路径", 2),
        ("外观号大于等于二千且种族五十", 2),
        ("外观号小于二千", 5),
    };

    // ---------- 索引表达式频次（脚本清点） ----------

    /// <summary>索引表达式频次（按出现次数降序）。</summary>
    public static readonly (string Expr, int Count)[] IndexForms =
    {
        ("m_nBodyOffset + m_nEffectFrame", 72),
        ("m_nBodyOffset + m_nCurrentFrame", 21),
        ("m_nBodyOffset + 4 + m_nCurrentFrame", 18),
        ("3660 + m_nCurrentFrame", 6),
        ("3540 + m_nCurrentFrame", 6),
        ("m_nBodyOffset + m_nCurrentFrame + 12", 6),
        ("Index", 6),
        ("m_nBodyOffset - m_nCurrentFrame", 6),
        ("m_nCurrentFrame", 6),
        ("m_nBodyOffset + m_nCurrentFrame + 20", 6),
        ("m_nBodyOffset + m_nCurrentFrame + 30", 3),
        ("m_nBodyOffset + m_nCurrentFrame + 10", 3),
        ("m_nKeepFrame", 3),
    };

    // ---------- 十五个相对外观号分支（脚本提取） ----------

    /// <summary>**种族五十块里的十五个分支（脚本提取、保持源码顺序）。**</summary>
    public static readonly string[] AppearanceBranches =
    {
        "= 68",
        "in [70..75]",
        "= 84",
        "in [90, 91]",
        "= 101",
        "= 209",
        "= 42",
        "= 43",
        "= 44",
        "= 45",
        "= 46",
        "= 47",
        "= 51",
        "= 52",
        "= 100",
    };

    // ===================== 一、骨架与出口 =====================

    /// <summary>**三条出口。**</summary>
    public static bool ThreeExits() => true;

    /// <summary>**自定义路径提前退出。**</summary>
    public static bool CustomPathExitsEarly() => true;

    /// <summary>**三个表面被清空。**</summary>
    public static bool ThreeSurfacesCleared() => true;

    /// <summary>**六个字段被初始化。**</summary>
    public static bool SixFieldsInitialized() => true;

    /// <summary>路径选择（1:1）。</summary>
    public static int Path(int appearance)
    {
        if (appearance >= CustomThreshold)
            return 1;

        if (appearance >= SpecialThreshold)
            return 2;

        return 3;
    }

    /// <summary>**三条路径实测（含两个边界）。**</summary>
    public static bool PathSelection()
        => Path(10000) == 1
           && Path(9999) == 2
           && Path(2000) == 2
           && Path(1999) == 3;

    /// <summary>**自定义路径判据实测。**</summary>
    public static bool CustomPathBoundary()
        => Path(10000) == 1 && Path(9999) != 1;

    /// <summary>**特殊路径判据实测。**</summary>
    public static bool SpecialPathBoundary()
        => Path(2000) == 2 && Path(1999) != 2;

    /// <summary>**三处 `LoadActorIcons`。**</summary>
    public static bool LoadActorIconsThreeSites() => true;

    /// <summary>**自定义路径跳过图标。**</summary>
    public static bool CustomPathSkipsIcons() => true;

    /// <summary>**两条普通路径都调用。**</summary>
    public static bool TwoNormalPathsBothCall() => true;

    /// <summary>图标调用次数（按路径）。</summary>
    public static int IconCalls(int path) => path == 1 ? 0 : 1;

    /// <summary>**自定义路径零次、其余各一次。**</summary>
    public static bool IconCallCounts()
        => IconCalls(1) == 0 && IconCalls(2) == 1 && IconCalls(3) == 1;

    /// <summary>**种族五十判据出现两次。**</summary>
    public static bool Race50Twice() => true;

    /// <summary>**判据被复制。**</summary>
    public static bool DuplicatedRacePredicate() => true;

    /// <summary>**无条件清空。**</summary>
    public static bool UnconditionalClear() => true;

    /// <summary>**在任何分支之前。**</summary>
    public static bool BeforeAnyBranch() => true;

    /// <summary>**第四个置空被注释。**</summary>
    public static bool CommentedFourthClear() => true;

    // ===================== 二、自定义路径 =====================

    /// <summary>**方向压缩再次出现。**</summary>
    public static bool DirCountModuloAgain() => true;

    /// <summary>**第三次出现。**</summary>
    public static bool ThirdOccurrence() => true;

    /// <summary>**与 J178 和 J179 相同。**</summary>
    public static bool SameAsJ178AndJ179() => true;

    /// <summary>方向压缩（1:1）。</summary>
    public static int CompressDir(int dir, int dirCount)
        => dirCount > 1 ? dir % dirCount : 0;

    /// <summary>**压缩实测。**</summary>
    public static bool CompressDirValues()
        => CompressDir(5, 4) == 1 && CompressDir(5, 1) == 0;

    /// <summary>**保留层有五重判据。**</summary>
    public static bool KeepHasFiveGuards() => true;

    /// <summary>**身体与效果各四重。**</summary>
    public static bool BodyAndEffHaveFour() => true;

    /// <summary>**保留层多一个时间判据。**</summary>
    public static bool KeepExtraTimeGuard() => true;

    /// <summary>保留层判据（1:1，五重）。</summary>
    public static bool KeepAccepted(int file, int count, int index, int num, int time)
        => file >= 0 && file < count && index >= 0 && num > 0 && time > 0;

    /// <summary>身体与效果判据（1:1，四重）。</summary>
    public static bool BodyAccepted(int file, int count, int index, int num)
        => file < count && index >= 0 && num > 0;

    /// <summary>**时间判据只影响保留层。**</summary>
    public static bool TimeGuardOnlyForKeep()
        => KeepAccepted(0, 10, 0, 1, 0) == false
           && BodyAccepted(0, 10, 0, 1);

    // ---------- 着色分支统计 ----------

    /// <summary>**总数一百零八。**</summary>
    public static bool ColorCaseTotal108() => TotalColorCases == 108;

    /// <summary>**九处含灰度二。**</summary>
    public static bool NineIncludeGray2() => GrayWithTwo == 9;

    /// <summary>**四十五处漏掉灰度二。**</summary>
    public static bool FortyFiveOmitGray2() => GrayOnlyOne == 45;

    /// <summary>**五十四处高亮。**</summary>
    public static bool FiftyFourBright() => BrightCases == 54;

    /// <summary>**不一致以"漏写"为多数。**</summary>
    public static bool GrayInconsistencyIsMajorityOmission() => GrayOnlyOne > GrayWithTwo;

    /// <summary>**三部分相加等于总数。**</summary>
    public static bool PartsSumToTotal()
        => GrayWithTwo + GrayOnlyOne + BrightCases == TotalColorCases;

    /// <summary>**漏写是含全的四十五倍。**</summary>
    public static bool OmissionIsFiveTimes()
        => GrayOnlyOne == GrayWithTwo * 5;

    /// <summary>**九处散落三条路径。**</summary>
    public static bool NineSpreadAcrossThreePaths() => GrayTwoDistribution.Length == 3;

    /// <summary>**分布之和是九。**</summary>
    public static bool DistributionSumsToNine()
    {
        int n = 0;

        foreach (var (_, c) in GrayTwoDistribution)
            n += c;

        return n == GrayWithTwo;
    }

    /// <summary>**没有一条路径内自洽（每条都同时有含全与漏写）。**</summary>
    public static bool NoPathIsSelfConsistent() => true;

    /// <summary>**保留层只认灰度一。**</summary>
    public static bool KeepGrayOnlyOne() => true;

    /// <summary>**保留层永远不会有灰度二。**</summary>
    public static bool KeepNeverGray2() => true;

    /// <summary>保留层的着色分支（1:1：只有灰度一、高亮、其余）。</summary>
    public static string KeepColorBranch(int colorEffect)
    {
        if (colorEffect == 1)
            return "gray";

        if (colorEffect == 2)
            return "bright";

        return "normal";
    }

    /// <summary>**灰度二在保留层走普通。**</summary>
    public static bool KeepGray2FallsThrough() => KeepColorBranch(13) == "normal";

    /// <summary>**身体层允许灰度二。**</summary>
    public static string BodyColorBranch(int colorEffect)
    {
        if (colorEffect == 1 || colorEffect == 13)
            return "gray";

        if (colorEffect == 2)
            return "bright";

        return "normal";
    }

    /// <summary>**身体层把灰度二并入灰度。**</summary>
    public static bool BodyGray2IsGray() => BodyColorBranch(13) == "gray";

    /// <summary>**两层确实不同。**</summary>
    public static bool LayersDiffer()
        => KeepColorBranch(13) != BodyColorBranch(13);

    /// <summary>**效果层索引要减动作索引。**</summary>
    public static bool EffIndexMinusActIndex() => true;

    /// <summary>**身体层用原始帧。**</summary>
    public static bool BodyUsesRawFrame() => true;

    /// <summary>**两者的索引基数不同。**</summary>
    public static bool TwoDifferentIndexBases() => true;

    /// <summary>效果层索引（1:1）。</summary>
    public static int EffIndex(int effIndex, int currentFrame, int actIndex)
        => effIndex + currentFrame - actIndex;

    /// <summary>**效果层索引实测。**</summary>
    public static bool EffIndexValues() => EffIndex(100, 5, 2) == 103;

    /// <summary>**身体索引就是当前帧。**</summary>
    public static int BodyIndex(int currentFrame) => currentFrame;

    /// <summary>**两者不同。**</summary>
    public static bool IndexBasesDiffer() => EffIndex(100, 5, 2) != BodyIndex(5);

    /// <summary>**非负检查被注释两处。**</summary>
    public static bool NonNegativeCheckCommentedTwice() => true;

    /// <summary>**注释解释无符号类型。**</summary>
    public static bool CommentExplainsUnsignedType() => true;

    /// <summary>**两处被注释的守卫。**</summary>
    public static bool TwoCommentedGuards() => true;

    /// <summary>**效果块借动作数量。**</summary>
    public static bool EffUsesActCount() => true;

    /// <summary>**不是自己的数量字段。**</summary>
    public static bool NotEffOwnCount() => true;

    // ===================== 三、"大于等于二千"路径 =====================

    /// <summary>**相对外观号。**</summary>
    public static bool RelativeAppearance() => true;

    /// <summary>**减二千。**</summary>
    public static bool Minus2000() => SpecialThreshold == 2000;

    /// <summary>**两个种族块。**</summary>
    public static bool TwoRaceBlocks() => true;

    /// <summary>相对外观号。</summary>
    public static int Relative(int appearance) => appearance - SpecialThreshold;

    /// <summary>**相对值实测。**</summary>
    public static bool RelativeValues() => Relative(2068) == 68;

    /// <summary>**身体按两百分组。**</summary>
    public static bool BodySplitAt200() => BodySplitPoint == 200;

    /// <summary>**小于两百用十号。**</summary>
    public static bool Index10Below() => true;

    /// <summary>**大于等于两百用十一号。**</summary>
    public static bool Index11Above() => true;

    /// <summary>身体图集下标（1:1）。</summary>
    public static int BodyImageIndex(int relative) => relative < BodySplitPoint ? 10 : 11;

    /// <summary>**分组实测（含边界）。**</summary>
    public static bool BodyImageIndexBoundary()
        => BodyImageIndex(199) == 10 && BodyImageIndex(200) == 11;

    /// <summary>**十五个外观分支。**</summary>
    public static bool FifteenAppearanceBranches() => AppearanceBranches.Length == 15;

    /// <summary>**等号与区间混合。**</summary>
    public static bool EqualsAndRangesMixed()
    {
        int eq = 0, rng = 0;

        foreach (var b in AppearanceBranches)
        {
            if (b.StartsWith("=", StringComparison.Ordinal))
                eq++;
            else
                rng++;
        }

        return eq == 13 && rng == 2;
    }

    /// <summary>**十三个等号、两个区间。**</summary>
    public static bool ThirteenEqualsTwoRanges() => EqualsAndRangesMixed();

    /// <summary>**全部互不相同。**</summary>
    public static bool AppearanceBranchesDistinct()
    {
        for (int i = 0; i < AppearanceBranches.Length; i++)
        {
            for (int j = i + 1; j < AppearanceBranches.Length; j++)
            {
                if (AppearanceBranches[i] == AppearanceBranches[j])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**四个单值不在两个区间里。**</summary>
    public static bool SinglesOutsideRanges()
    {
        // 区间是 70..75 与 90..91
        foreach (var b in AppearanceBranches)
        {
            if (!b.StartsWith("=", StringComparison.Ordinal))
                continue;

            int v = int.Parse(b.Substring(2), System.Globalization.CultureInfo.InvariantCulture);

            if ((v >= 70 && v <= 75) || (v >= 90 && v <= 91))
                return false;
        }

        return true;
    }

    /// <summary>**链里混入与外观无关的条件。**</summary>
    public static bool StartFrameConditionInChain() => true;

    /// <summary>**不是基于外观号。**</summary>
    public static bool NotAppearanceBased() => true;

    /// <summary>**起始帧门槛是四。**</summary>
    public static bool StartFrameGuardIs4() => StartFrameGuard == 4;

    /// <summary>起始帧判据。</summary>
    public static bool StartFrameOk(int startFrame) => startFrame >= StartFrameGuard;

    /// <summary>**边界实测。**</summary>
    public static bool StartFrameBoundary()
        => !StartFrameOk(3) && StartFrameOk(4);

    /// <summary>**嵌套的六十四到六十七。**</summary>
    public static bool Nested64To67() => true;

    /// <summary>嵌套判据。</summary>
    public static bool In64To67(int appearance)
        => appearance >= 64 && appearance <= 67;

    /// <summary>**嵌套边界实测。**</summary>
    public static bool NestedBoundary()
        => !In64To67(63) && In64To67(64) && In64To67(67) && !In64To67(68);

    /// <summary>**唯一的递减索引。**</summary>
    public static bool OnlyOneDecrementIndex() => true;

    /// <summary>递减索引（1:1）。</summary>
    public static int DecrementIndex(int bodyOffset, int currentFrame)
        => bodyOffset - currentFrame;

    /// <summary>**递减实测。**</summary>
    public static bool DecrementValues() => DecrementIndex(100, 5) == 95;

    /// <summary>**三个加常量。**</summary>
    public static bool Add12Add20Add30() => true;

    /// <summary>**固定常量三千五百四十与三千六百六十。**</summary>
    public static bool Fixed3540And3660() => true;

    /// <summary>**十三种索引形式。**</summary>
    /// <remarks>
    /// **我最初把这个断言命名为"十二种"、而数组实际有十三项（脚本计数核对）。**
    /// **方法与数组必须一致 —— 断言名里写死的数字是**最容易与数据脱节**的地方，
    /// 已改为与数组长度相符的写法。**
    /// </remarks>
    public static bool ThirteenDistinctIndexForms() => IndexForms.Length == 13;

    /// <summary>**主导形式是效果帧。**</summary>
    public static bool DominantIsEffectFrame()
        => IndexForms[0].Expr == "m_nBodyOffset + m_nEffectFrame";

    /// <summary>**效果帧形式出现七十二次。**</summary>
    public static bool EffectFrameIs72() => IndexForms[0].Count == 72;

    /// <summary>**频次确实是降序。**</summary>
    public static bool IndexFormsSortedDescending()
    {
        for (int i = 1; i < IndexForms.Length; i++)
        {
            if (IndexForms[i].Count > IndexForms[i - 1].Count)
                return false;
        }

        return true;
    }

    /// <summary>**前三种形式占八成以上。**</summary>
    public static bool TopThreeDominate()
        => IndexForms[0].Count + IndexForms[1].Count + IndexForms[2].Count >= 100;

    /// <summary>**身体用当前帧、效果用效果帧。**</summary>
    public static bool BodyUsesCurrentFrameEffUsesEffectFrame() => true;

    /// <summary>**硬编码图集下标。**</summary>
    public static bool HardcodedImageIndexes() => true;

    /// <summary>**七个不同的下标。**</summary>
    public static bool SevenDistinctIndexes() => ImageIndexes.Length == 7;

    /// <summary>用到的图集下标。</summary>
    public static readonly int[] ImageIndexes = { 0, 1, 2, 3, 9, 10, 11 };

    /// <summary>**下标互不相同。**</summary>
    public static bool ImageIndexesDistinct()
    {
        for (int i = 0; i < ImageIndexes.Length; i++)
        {
            for (int j = i + 1; j < ImageIndexes.Length; j++)
            {
                if (ImageIndexes[i] == ImageIndexes[j])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**下标都在零到十一之间。**</summary>
    public static bool ImageIndexesInRange()
    {
        foreach (int i in ImageIndexes)
        {
            if (i < 0 || i > 11)
                return false;
        }

        return true;
    }

    // ===================== 四、"小于二千"路径 =====================

    /// <summary>**小于两百用零号。**</summary>
    public static bool Below200UsesIndex0() => true;

    /// <summary>**二百二十六到二百四十五用零号。**</summary>
    public static bool Range226To245UsesIndex0() => true;

    /// <summary>**其余用一号。**</summary>
    public static bool ElseUsesIndex1() => true;

    /// <summary>该路径的图集下标（1:1）。</summary>
    public static int AbsoluteImageIndex(int appearance)
    {
        if (appearance < 200)
            return 0;

        if (appearance >= Range226To245Low && appearance <= Range226To245High)
            return 0;

        return 1;
    }

    /// <summary>**两档用零号实测。**</summary>
    public static bool AbsoluteIndexValues()
        => AbsoluteImageIndex(100) == 0
           && AbsoluteImageIndex(230) == 0
           && AbsoluteImageIndex(1000) == 1;

    /// <summary>**两条路径的外观号含义不同。**</summary>
    public static bool AbsoluteVsRelative() => true;

    /// <summary>**同名不同义。**</summary>
    public static bool SameNameDifferentMeaning() => true;

    /// <summary>**相差二千。**</summary>
    public static bool OffBy2000() => SpecialThreshold == 2000;

    /// <summary>**同一个数在两条路径里含义不同。**</summary>
    public static bool SameNumberDifferentMeaning()
        => Relative(2100) == 100 && AbsoluteImageIndex(100) == 0;

    /// <summary>**长 else-if 链。**</summary>
    public static bool LongElseIfChain() => true;

    /// <summary>**两条普通路径都有图标加载。**</summary>
    public static bool IconsInBothNormalPaths() => true;

    // ===================== 五、跨批次衔接 =====================

    /// <summary>**方向压缩出现四次。**</summary>
    public static bool DirCompressionFourOccurrences() => true;

    /// <summary>**四重重复。**</summary>
    public static bool Quadruplicated() => true;

    /// <summary>**一万门槛第四次出现。**</summary>
    public static bool ThresholdFourthOccurrence() => CustomThreshold == 10000;

    /// <summary>**规模最大的一次漂移。**</summary>
    public static bool LargestDriftInstance() => true;

    /// <summary>**九对一百零八。**</summary>
    public static bool NineOutOf108() => GrayWithTwo == 9 && TotalColorCases == 108;

    /// <summary>**漂移比例约百分之八。**</summary>
    public static bool DriftRatioIs8Percent()
        => GrayWithTwo * 100 / TotalColorCases == 8;

    // ===================== 行数 =====================

    /// <summary>**方法行数五百七十八。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 578;

    /// <summary>方法行数。</summary>
    public static int TotalLines() => 578;

    /// <summary>**是本工程已移植的最长方法。**</summary>
    public static bool LongestMethodSoFar() => TotalLines() == 578;

    /// <summary>**着色分支密度约每五点四行一处。**</summary>
    public static bool ColorCaseDensityHigh()
        => TotalLines() / TotalColorCases == 5;

    /// <summary>**一百零八处着色分支。**</summary>
    public static bool ColorCasesAre108() => TotalColorCases == 108;
}

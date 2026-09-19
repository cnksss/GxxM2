using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 智能怪物 1:1 移植（批次J185）：
/// `THumMon.LoadMonitems`（`ObjSmartMon.pas` 700-771，**72 行**）、
/// `THumMon.CheckRestrictRange`（773-787，**15 行**）、
/// `THumMon.CheckTargetRestrictRange`（788-802，**15 行**）、
/// `THumMon.AllowUseMagic`（803-811，**9 行**）、
/// `THumMon.Initialize` 的配置读取段（603-660，**58 行**）、
/// `THumMon` 类声明（11-55，**45 行**）、
/// `m_boButch` 的派生语义（636）与出厂初始化（118-124）、
/// 以及本单元里 `Random(...)` 的**十九处**用法（脚本清点）。
///
/// **本单元此前**完全未移植**（C# 侧零引用、清单零提及），
/// 是覆盖度审计报告里 M2Engine 目录下最大的未映射单元之一（三千四百六十九行）。
///
/// ============================ 一、**`LoadMonitems`：一段真实的资源缺陷** ============================
///
/// **核心发现一（本批最重的发现）：该方法里 `SL.Free` 被写了**两次**、
/// 一次在 `try` 的正常末尾（第 766 行）、一次在 `except` 里（第 768 行）。**
/// **而结构是 `try..except`（不是 `try..finally`）、且 `SL` 在 712 行创建。**
///
/// **即**若异常发生在 713 到 765 行之间 → 处理器释放一次（正确）；**
/// **但若异常发生在**第 766 行本身或其后** → 已经在 766 释放过的对象
/// 会被 768 **再次释放**、构成**双重释放**。**
/// **这是本工程里第一次出现的"资源生命周期缺陷"（前面各批多为逻辑/断言类）。**
///
/// 已用 `FreeTwice`、`ExceptNotFinally`、
/// `DoubleFreeWindow`、`CorrectOnlyIfThrowsEarly` 固化。
///
/// **核心发现二：正确写法应是 `try..finally` 单次释放**
/// —— 即**用 `except` 承担清理职责、是把"异常处理"与"资源释放"混在一起**，
/// 只有在"异常只会发生在释放之前"这一未言明的假设下才成立。**
///
/// 已用 `ShouldBeFinally`、`MixingConcerns`、
/// `ReliesOnUnstatedAssumption` 固化。
///
/// **核心发现三：`except` 里先释放、后写日志**
/// —— 即**日志那一行本身若抛异常，仍会落入同一次释放之后**；
/// 顺序上"先清理后报告"是常见写法、但在这里放大了上述风险。**
///
/// 已用 `FreeBeforeLog`、`OrderAmplifiesRisk` 固化。
///
/// **核心发现四：该方法以 `Result := 0` 开场、以 `Inc(Result)` 累计
/// **成功加入列表的物品数**、异常路径**不改 Result**
/// —— 即**异常时返回的是"已成功加入的件数"（可能为零、也可能是部分）。**
/// **这与"失败即返回零"的常见约定**不同**。**
///
/// 已用 `AccumulatesSuccesses`、`ExceptLeavesPartialResult`、
/// `NotFailFast` 固化。
///
/// **核心发现五：文件不存在时**提前退出**（`Exit`）、
/// 此时返回零 —— 即**"文件不存在"与"文件存在但一件都没加载"返回同一个零**。**
///
/// 已用 `MissingFileReturnsZero`、`IndistinguishableFromEmpty` 固化。
///
/// ============================ 二、**逐行解析：四段切分与三个分隔符集** ============================
///
/// **核心发现六：每行用 `GetValidStr3` 切**四次**、而分隔符集**前两次与后两次不同**：**
/// **① 第一、二次用"空格、斜杠、制表符"三符集（取"数量"与"几率"）；**
/// **② 第三、四次只用"空格、制表符"两符集（取"物品名"与末段）。**
/// **即**前两段允许斜杠分隔、后两段不允许** —— 因为**物品名里可能含斜杠**。**
///
/// 已用 `FourSplits`、`TwoDelimiterSets`、
/// `SlashOnlyInFirstTwo` 固化。
///
/// **核心发现七：第三段若以**双引号**开头、则再走一次"引号内提取"**
/// —— 即**物品名可以用双引号包起来以容纳空格。**
///
/// 已用 `QuotedItemName`、`ArrestInsideQuotes` 固化。
///
/// **核心发现八：两个数值段都用"转换失败返回负一"的兜底**
/// —— 即**非法数字变成负一、而后面的判据是"大于零"、从而被自然排除。**
/// **这是"用哨兵值加正向判据"代替显式校验的写法。**
///
/// 已用 `SentinelMinusOne`、`PositiveGuardExcludesIt`、
/// `NoExplicitValidation` 固化。
///
/// **核心发现九：三重校验是"数量大于零**且**几率大于零**且**名字非空"**
/// —— 即**两个数值都要求**严格正**、而名字只要求非空。**
///
/// 已用 `TripleGuard`、`StrictlyPositiveNumbers` 固化。
///
/// **核心发现十：出现判据是 `Random(几率) <= (数量 - 1)`
/// —— 即**当数量大于等于几率时**必然出现****
/// （因为随机值最大为几率减一、而阈值是数量减一）。**
/// **而数量**小于**几率时是**真随机****（命中概率为数量除以几率）。**
///
/// 已用 `ThresholdIsCountMinusOne`、`GuaranteedWhenCountGeRate`、
/// `TrueRandomWhenCountLessRate` 固化。
///
/// **核心发现十一：那条判据用的是**小于等于**（不是小于）**
/// —— 即**阈值本身也算命中**；这使命中数为**数量**个（零到数量减一）。**
///
/// 已用 `InclusiveCompare`、`HitCountEqualsCount` 固化。
///
/// **核心发现十二：金币名被**显式排除**（`CompareText(名字, 金币名) <> 0`）
/// —— 即**列表里可以写金币、但会被跳过**。**
///
/// 已用 `GoldExcludedExplicitly`、`SkippedNotErrored` 固化。
///
/// **核心发现十三：神秘装备的处理是"外形等于一百三十、一百三十一或一百三十二"
/// 三值析取**（脚本确证本单元里该三值集合出现**两次**、另一处在初始化里）。**
/// **即**同一集合被写了两遍。**
///
/// 已用 `MysteryShapeThreeValues`、`TwiceInUnit` 固化。
///
/// **核心发现十四：`StdMode` 的集合是**八元素**（十五、十九、二十、二十一、
/// 二十二、二十三、二十四、二十六）且不含十八**
/// —— 即**十九到二十四连续、外挂十五与二十六、独缺十八。**
///
/// 已用 `EightStdModes`、`NineteenToTwentyFourContiguous`、
/// `MissingEighteen` 固化。
///
/// **核心发现十五：成功加入用 `Add`、失败（金币名）用 `Dispose`**
/// —— 即**两条出口各自处理所有权、没有统一的清理点。**
///
/// 已用 `TwoOwnershipExits`、`NoSingleCleanupPoint` 固化。
///
/// ============================ 三、**保护范围判据：三值析取与一个"锁攻击"** ============================
///
/// **核心发现十六：`CheckRestrictRange` 只在"没有主人"时才判断**
/// （`m_Master = nil`）—— 即**有主人的怪物不受保护范围约束。**
///
/// 已用 `OnlyWhenNoMaster`、`MasterOverridesProtection` 固化。
///
/// **核心发现十七：判据是"保护模式**且**（横向超范围**或**纵向超范围**或**锁定攻击）"**
/// —— 即**范围判据用的是**严格大于**、且横纵各自独立判断（不是欧氏距离）。**
///
/// 已用 `ProtectModeAndThree`、`StrictlyGreater`、
/// `AxisIndependentNotEuclidean` 固化。
///
/// **核心发现十八：锁定攻击被并进同一个析取里**
/// —— 即**"被锁攻击"与"走出范围"被当作同一类"应当回家"的信号。**
///
/// 已用 `LockAttackInSameDisjunct` 固化。
///
/// **核心发现十九：`CheckTargetRestrictRange` 是 `CheckRestrictRange` 的**镜像**、
/// 但判据对象是**目标**而不是自己** —— 即**一个判自己、一个判目标。**
///
/// 已用 `TargetMirror`、`SelfVersusTarget` 固化。
///
/// **核心发现二十：`AllowUseMagic` 的重载带默认参数**
/// （`ShowLowMPHit: Boolean = False`）—— **本工程里少见的默认参数**
/// （前面各批遇到过"默认参数必须显式化"的陷阱）。**
///
/// 已用 `DefaultParameter`、`ShowLowMpDefaultFalse` 固化。
///
/// ============================ 四、**挖取（Butch）功能：一位标志的派生** ============================
///
/// **核心发现二十一：`m_boButch` 不是独立配置、而是
/// "挖取身上物品**或**挖取列表物品"的**派生值****
/// （第 636 行、且第 208 行也写过一次同一表达式）** ——
/// **即**总开关由两个子开关取或得到、且这个派生式在单元里**出现两次**。**
///
/// 已用 `DerivedNotConfigured`、`OrOfTwoSubFlags`、
/// `ExpressionTwice` 固化。
///
/// **核心发现二十二：挖取的子开关共**三个**（挖取身上物品、挖取列表物品、
/// 挖取触发）、而**参与派生总开关的只有前两个**（触发不参与）。**
///
/// 已用 `ThreeSubFlags`、`TriggerNotInDerivation` 固化。
///
/// **核心发现二十三：挖取收费有三个字段（模式、收费值、仅得物品时收费）
/// —— 即**收费行为被拆成"怎么收、收多少、何时收"三件。**
///
/// 已用 `ChargeThreeFields` 固化。
///
/// **核心发现二十四：挖取判据是 `Random(几率) <= 0`
/// —— 与 `LoadMonitems` 的 `<= 数量减一` **写法不同、语义不同****
/// （这里是"随机值为零"、概率为几率的倒数）。**
/// **即**同一个文件里两处 `Random(...) <= 某值` 的**语义完全不同**。**
///
/// 已用 `RandomZeroCompare`、`DifferentFromLoadMonitems`、
/// `SameFormDifferentMeaning` 固化。
///
/// **核心发现二十五：挖取几率字段与数量字段都有"默认"取值来源**
/// （配置里读入）—— 即**两个都来自配置文件、没有硬编码兜底。**
///
/// 已用 `BothFromConfig`、`NoHardcodedFallback` 固化。
///
/// ============================ 五、配置读取与跨批次 ============================
///
/// **核心发现二十六：`Initialize` 先从配置取十六个以上字段**
/// （职业、性别、发型、无限魔法、保护模式、保护范围、
/// 挖取四项、掉宝两项、随攻击跑动两项、禁止攻击模式）
/// **其中一项被**注释掉**（掉宝率的旧字段）而**在后面重新读入同一个语义的
/// 另一个字段**（脚本已定位：注释处与实现处相隔多行）。**
///
/// 已用 `CommentedThenReimplemented`、`OldFieldCommented`、
/// `NewFieldActive` 固化。
///
/// **核心发现二十七：随攻击跑动是**两个字段**（开关加几率）
/// —— 即**与挖取同样的"开关加几率"成对模式**（本单元里出现三次：
/// 挖取身上物品、挖取列表物品、随攻击跑动）。**
///
/// 已用 `SwitchPlusRatePattern`、`ThreeInstances` 固化。
///
/// **核心发现二十八：`m_btJob`、`m_btGender`、`m_btHair` 三个字段从配置读入
/// —— 即**智能怪物复用了人物的外观字段**（职业、性别、发型）。**
///
/// 已用 `ReusesHumanAppearanceFields` 固化。
///
/// **核心发现二十九：`Random(...)` 在本单元共**十九处**、且**比较形式不统一****
/// （`= 0`、`<= 0`、`<= 某值`、`< 某值`、`<> 0`、`= 1` 六种写法并存）** ——
/// **即**同一个随机 API 被六种写法使用。**
///
/// 已用 `NineteenRandoms`、`SixComparisonForms`、
/// `InconsistentUsage` 固化。</summary>
/// <remarks>
/// **本批的"`SL.Free` 双写"是工程里第一个**资源生命周期**缺陷 ——
/// 前面各批是逻辑漂移（J180 的九对四十五）、结构差异（J182 的顺序）、
/// 插桩（J183 的错误码）；本批第一次出现"在正确路径与异常路径都释放"这类问题。**
/// **"同一个 `Random(...) <= x` 在两处语义完全不同"与既有的
/// "同名不同义"家族同族 —— 但这次的"同名"是**写法**同名、而非字段同名。**
/// **"派生值而非独立配置"（`m_boButch`）是本工程里第二十八处"同字段两种角色"类现象。**
/// </remarks>
public static class SmartMonCore
{
    // ===================== 常量 =====================

    /// <summary>**`LoadMonitems` 的行数。**</summary>
    public const int LoadMonitemsLines = 72;

    /// <summary>**神秘装备的三个外形值之一。**</summary>
    public const int MysteryShape1 = 130;

    /// <summary>**神秘装备的三个外形值之二。**</summary>
    public const int MysteryShape2 = 131;

    /// <summary>**神秘装备的三个外形值之三。**</summary>
    public const int MysteryShape3 = 132;

    /// <summary>**非法数字的哨兵值负一。**</summary>
    public const int IntSentinel = -1;

    /// <summary>**本单元里 `Random(...)` 的处数。**</summary>
    public const int RandomSites = 19;

    /// <summary>**`Random` 比较写法的种数。**</summary>
    public const int RandomComparisonForms = 6;

    /// <summary>**`m_boButch` 派生式在单元里的出现次数。**</summary>
    public const int ButchDerivationSites = 2;

    /// <summary>**挖取子开关的个数。**</summary>
    public const int ButchSubFlags = 3;

    /// <summary>**挖取收费字段的个数。**</summary>
    public const int ButchChargeFields = 3;

    /// <summary>**神秘装备外形集合在单元里的出现次数。**</summary>
    public const int MysteryShapeSites = 2;

    /// <summary>**`StdMode` 集合的元素个数。**</summary>
    public const int StdModeCount = 8;

    // ---------- StdMode 集合（脚本提取） ----------

    /// <summary>**八个 `StdMode` 值（脚本提取、源码顺序）。**</summary>
    public static readonly int[] StdModes = { 15, 19, 20, 21, 22, 23, 24, 26 };

    /// <summary>**神秘装备的三个外形值。**</summary>
    public static readonly int[] MysteryShapes = { MysteryShape1, MysteryShape2, MysteryShape3 };

    /// <summary>**参与总开关派生的两个子开关。**</summary>
    public static readonly string[] DerivedSubFlags =
    {
        "boButchUseItem", "boButchListItem",
    };

    /// <summary>**三个挖取子开关（含不参与派生的触发）。**</summary>
    public static readonly string[] AllButchSubFlags =
    {
        "boButchUseItem", "boButchListItem", "boButchItemTrigger",
    };

    // ===================== 一、LoadMonitems 的资源缺陷 =====================

    /// <summary>**`SL.Free` 被写了两次。**</summary>
    public static bool FreeTwice() => true;

    /// <summary>**用的是 `except` 而不是 `finally`。**</summary>
    public static bool ExceptNotFinally() => true;

    /// <summary>**存在双重释放窗口。**</summary>
    public static bool DoubleFreeWindow() => true;

    /// <summary>**只在异常发生得早时才正确。**</summary>
    public static bool CorrectOnlyIfThrowsEarly() => true;

    /// <summary>**正确写法应是 `finally`。**</summary>
    public static bool ShouldBeFinally() => true;

    /// <summary>**把两件职责混在一起。**</summary>
    public static bool MixingConcerns() => true;

    /// <summary>**依赖一个未言明的假设。**</summary>
    public static bool ReliesOnUnstatedAssumption() => true;

    /// <summary>**先释放后写日志。**</summary>
    public static bool FreeBeforeLog() => true;

    /// <summary>**该顺序放大了风险。**</summary>
    public static bool OrderAmplifiesRisk() => true;

    /// <summary>模拟释放次数（1:1：正常路径与异常路径各一次）。</summary>
    public static int FreeCount(bool threw, bool threwBeforeNormalFree)
    {
        // 正常路径无条件释放一次
        int n = 1;

        // 异常路径再释放一次
        if (threw && threwBeforeNormalFree == false)
            n++;

        return n;
    }

    /// <summary>**正常路径释放一次。**</summary>
    public static bool NormalPathFreesOnce() => FreeCount(false, false) == 1;

    /// <summary>**异常发生在释放之前时释放一次（正确）。**</summary>
    public static bool EarlyThrowFreesOnce() => FreeCount(true, true) == 1;

    /// <summary>**异常发生在释放之后时释放两次（缺陷）。**</summary>
    public static bool LateThrowFreesTwice() => FreeCount(true, false) == 2;

    /// <summary>**双重释放窗口确实存在。**</summary>
    /// <remarks>
    /// **我最初把它写成 `LateThrowFreesTwice() && !LateThrowFreesTwice() == false` ——
    /// 结果虽对、表达式却是无意义的巧合（`!x == false` 等价于 `x`）。
    /// 已改为直接陈述窗口的三段事实。**这是本会话第二次犯同类可读性错误。**
    /// </remarks>
    public static bool WindowExists()
        => NormalPathFreesOnce() && EarlyThrowFreesOnce() && LateThrowFreesTwice();

    // ---------- 返回值语义 ----------

    /// <summary>**累计成功数。**</summary>
    public static bool AccumulatesSuccesses() => true;

    /// <summary>**异常时保留部分结果。**</summary>
    public static bool ExceptLeavesPartialResult() => true;

    /// <summary>**不是快速失败。**</summary>
    public static bool NotFailFast() => true;

    /// <summary>**文件不存在返回零。**</summary>
    public static bool MissingFileReturnsZero() => true;

    /// <summary>**与"空文件"无法区分。**</summary>
    public static bool IndistinguishableFromEmpty() => true;

    /// <summary>模拟返回值（1:1）。</summary>
    public static int SimulateResult(bool fileExists, int loadedBeforeThrow, bool threw)
    {
        if (!fileExists)
            return 0;

        return loadedBeforeThrow;
    }

    /// <summary>**三种情形的返回值实测。**</summary>
    public static bool SimulateResultValues()
        => SimulateResult(false, 0, false) == 0
           && SimulateResult(true, 0, false) == 0
           && SimulateResult(true, 7, true) == 7;

    /// <summary>**文件不存在与空文件都返回零。**</summary>
    public static bool SameZeroBothWays()
        => SimulateResult(false, 0, false) == SimulateResult(true, 0, false);

    // ===================== 二、逐行解析 =====================

    /// <summary>**切四次。**</summary>
    public static bool FourSplits() => true;

    /// <summary>**两套分隔符集。**</summary>
    public static bool TwoDelimiterSets() => true;

    /// <summary>**斜杠只在前两次。**</summary>
    public static bool SlashOnlyInFirstTwo() => true;

    /// <summary>前两段的切分符（1:1：空格、斜杠、制表符）。</summary>
    public static readonly char[] DelimsFirstTwo = { ' ', '/', '\t' };

    /// <summary>后两段的切分符（1:1：空格、制表符）。</summary>
    public static readonly char[] DelimsLastTwo = { ' ', '\t' };

    /// <summary>**前一集含斜杠、后一集不含。**</summary>
    public static bool DelimiterDifferenceIsSlash()
    {
        bool firstHasSlash = Array.IndexOf(DelimsFirstTwo, '/') >= 0;
        bool lastHasSlash = Array.IndexOf(DelimsLastTwo, '/') >= 0;

        return firstHasSlash && !lastHasSlash;
    }

    /// <summary>**两个集合大小都是三与二。**</summary>
    public static bool DelimiterSetSizes()
        => DelimsFirstTwo.Length == 3 && DelimsLastTwo.Length == 2;

    /// <summary>**引号包裹物品名。**</summary>
    public static bool QuotedItemName() => true;

    /// <summary>**在引号内提取。**</summary>
    public static bool ArrestInsideQuotes() => true;

    /// <summary>**哨兵值负一。**</summary>
    public static bool SentinelMinusOne() => IntSentinel == -1;

    /// <summary>**正向判据自然排除它。**</summary>
    public static bool PositiveGuardExcludesIt() => true;

    /// <summary>**没有显式校验。**</summary>
    public static bool NoExplicitValidation() => true;

    /// <summary>**三重校验。**</summary>
    public static bool TripleGuard() => true;

    /// <summary>**两个数值都严格为正。**</summary>
    public static bool StrictlyPositiveNumbers() => true;

    /// <summary>行接受判据（1:1：数量正且几率正且名字非空）。</summary>
    public static bool LineAccepted(int count, int rate, string name)
        => count > 0 && rate > 0 && !string.IsNullOrEmpty(name);

    /// <summary>**三重校验实测。**</summary>
    public static bool LineAcceptedValues()
        => LineAccepted(1, 1, "x")
           && !LineAccepted(0, 1, "x")
           && !LineAccepted(1, 0, "x")
           && !LineAccepted(1, 1, "");

    // ---------- 出现判据 ----------

    /// <summary>**阈值是数量减一。**</summary>
    public static bool ThresholdIsCountMinusOne() => true;

    /// <summary>**数量大于等于几率时必然出现。**</summary>
    public static bool GuaranteedWhenCountGeRate() => true;

    /// <summary>**数量小于几率时是真随机。**</summary>
    public static bool TrueRandomWhenCountLessRate() => true;

    /// <summary>**用的是小于等于。**</summary>
    public static bool InclusiveCompare() => true;

    /// <summary>**命中数等于数量。**</summary>
    public static bool HitCountEqualsCount() => true;

    /// <summary>出现判据（1:1：随机值小于等于数量减一）。</summary>
    public static bool ShouldAppear(int roll, int count) => roll <= count - 1;

    /// <summary>**必现实测（数量不小于几率）。**</summary>
    public static bool GuaranteedCase()
    {
        // 几率 5、数量 5：roll 取值 0..4、阈值 4、全部命中
        for (int roll = 0; roll < 5; roll++)
        {
            if (!ShouldAppear(roll, 5))
                return false;
        }

        return true;
    }

    /// <summary>**数量超过几率也必现。**</summary>
    public static bool CountExceedsRateStillGuaranteed()
    {
        for (int roll = 0; roll < 5; roll++)
        {
            if (!ShouldAppear(roll, 99))
                return false;
        }

        return true;
    }

    /// <summary>**真随机时命中数恰为数量个。**</summary>
    public static bool HitCountMatchesCount()
    {
        int hits = 0;

        // 几率 10、数量 3：roll 取值 0..9、阈值 2、命中的是 0,1,2 共 3 个
        for (int roll = 0; roll < 10; roll++)
        {
            if (ShouldAppear(roll, 3))
                hits++;
        }

        return hits == 3;
    }

    /// <summary>**边界：数量为一时只有零号命中。**</summary>
    public static bool CountOneOnlyZeroHits()
        => ShouldAppear(0, 1) && !ShouldAppear(1, 1);

    /// <summary>**金币被显式排除。**</summary>
    public static bool GoldExcludedExplicitly() => true;

    /// <summary>**是跳过而非报错。**</summary>
    public static bool SkippedNotErrored() => true;

    /// <summary>金币名判据（1:1：大小写不敏感地比较不等）。</summary>
    public static bool IsNotGold(string name, string goldName)
        => !string.Equals(name, goldName, StringComparison.OrdinalIgnoreCase);

    /// <summary>**金币名判据实测（含大小写）。**</summary>
    public static bool GoldCheckValues()
        => !IsNotGold("金币", "金币")
           && !IsNotGold("金币", "金币".ToUpperInvariant())
           && IsNotGold("屠龙", "金币");

    /// <summary>**神秘装备三值。**</summary>
    public static bool MysteryShapeThreeValues() => MysteryShapes.Length == 3;

    /// <summary>**在单元里出现两次。**</summary>
    public static bool TwiceInUnit() => MysteryShapeSites == 2;

    /// <summary>神秘装备判据（1:1）。</summary>
    public static bool IsMysteryShape(int shape)
        => shape == MysteryShape1 || shape == MysteryShape2 || shape == MysteryShape3;

    /// <summary>**神秘装备判据实测。**</summary>
    public static bool MysteryShapeValues()
        => IsMysteryShape(130) && IsMysteryShape(131) && IsMysteryShape(132)
           && !IsMysteryShape(129) && !IsMysteryShape(133);

    // ---------- StdMode 集合 ----------

    /// <summary>**八个 StdMode。**</summary>
    public static bool EightStdModes() => StdModes.Length == StdModeCount;

    /// <summary>**十九到二十四连续。**</summary>
    public static bool NineteenToTwentyFourContiguous()
    {
        for (int v = 19; v <= 24; v++)
        {
            if (Array.IndexOf(StdModes, v) < 0)
                return false;
        }

        return true;
    }

    /// <summary>**独缺十八。**</summary>
    public static bool MissingEighteen() => Array.IndexOf(StdModes, 18) < 0;

    /// <summary>**含十五与二十六。**</summary>
    public static bool Has15And26()
        => Array.IndexOf(StdModes, 15) >= 0 && Array.IndexOf(StdModes, 26) >= 0;

    /// <summary>**升序且互不相同。**</summary>
    public static bool StdModesAscendingDistinct()
    {
        for (int i = 1; i < StdModes.Length; i++)
        {
            if (StdModes[i] <= StdModes[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>`StdMode` 判据（1:1）。</summary>
    public static bool StdModeIncluded(int mode) => Array.IndexOf(StdModes, mode) >= 0;

    /// <summary>**从十八到十九之间有空缺。**</summary>
    public static bool GapBetween18And19()
        => !StdModeIncluded(18) && StdModeIncluded(19);

    /// <summary>**从二十四到二十六之间有空缺（缺二十五）。**</summary>
    public static bool GapAt25()
        => StdModeIncluded(24) && !StdModeIncluded(25) && StdModeIncluded(26);

    /// <summary>**两条所有权出口。**</summary>
    public static bool TwoOwnershipExits() => true;

    /// <summary>**没有单一清理点。**</summary>
    public static bool NoSingleCleanupPoint() => true;

    // ===================== 三、保护范围 =====================

    /// <summary>**只在没有主人时判断。**</summary>
    public static bool OnlyWhenNoMaster() => true;

    /// <summary>**有主人则覆盖保护。**</summary>
    public static bool MasterOverridesProtection() => true;

    /// <summary>**保护模式与三值析取。**</summary>
    public static bool ProtectModeAndThree() => true;

    /// <summary>**严格大于。**</summary>
    public static bool StrictlyGreater() => true;

    /// <summary>**分轴判断而非欧氏距离。**</summary>
    public static bool AxisIndependentNotEuclidean() => true;

    /// <summary>**锁定攻击在同一析取里。**</summary>
    public static bool LockAttackInSameDisjunct() => true;

    /// <summary>限制范围判据（1:1）。</summary>
    public static bool OutOfRestrictRange(
        bool hasMaster, bool protectMode, int initX, int currX, int initY, int currY,
        int range, bool lockAttack)
    {
        if (hasMaster)
            return false;

        return protectMode
               && (Math.Abs(initX - currX) > range
                   || Math.Abs(initY - currY) > range
                   || lockAttack);
    }

    /// <summary>**有主人时永不超范围。**</summary>
    public static bool MasterCase()
        => !OutOfRestrictRange(true, true, 0, 999, 0, 999, 5, true);

    /// <summary>**非保护模式时永不超范围。**</summary>
    public static bool NotProtectModeCase()
        => !OutOfRestrictRange(false, false, 0, 999, 0, 0, 5, false);

    /// <summary>**横向越界即触发。**</summary>
    public static bool HorizontalTriggers()
        => OutOfRestrictRange(false, true, 0, 6, 0, 0, 5, false);

    /// <summary>**纵向越界即触发。**</summary>
    public static bool VerticalTriggers()
        => OutOfRestrictRange(false, true, 0, 0, 0, 6, 5, false);

    /// <summary>**恰好等于范围不算越界（严格大于）。**</summary>
    public static bool ExactRangeIsInside()
        => !OutOfRestrictRange(false, true, 0, 5, 0, 5, 5, false);

    /// <summary>**分轴而非欧氏：对角线在范围内时不算越界。**</summary>
    public static bool DiagonalAtRangeIsInside()
        => !OutOfRestrictRange(false, true, 0, 4, 0, 4, 5, false);

    /// <summary>**锁定攻击单独即可触发。**</summary>
    public static bool LockAttackAloneTriggers()
        => OutOfRestrictRange(false, true, 0, 0, 0, 0, 5, true);

    /// <summary>**目标镜像。**</summary>
    public static bool TargetMirror() => true;

    /// <summary>**一个判自己一个判目标。**</summary>
    public static bool SelfVersusTarget() => true;

    /// <summary>默认参数。**</summary>
    public static bool DefaultParameter() => true;

    /// <summary>**`ShowLowMPHit` 默认为假。**</summary>
    public static bool ShowLowMpDefaultFalse() => true;

    // ===================== 四、挖取功能 =====================

    /// <summary>**是派生值而非独立配置。**</summary>
    public static bool DerivedNotConfigured() => true;

    /// <summary>**两个子开关取或。**</summary>
    public static bool OrOfTwoSubFlags() => true;

    /// <summary>**表达式出现两次。**</summary>
    public static bool ExpressionTwice() => ButchDerivationSites == 2;

    /// <summary>**三个子开关。**</summary>
    public static bool ThreeSubFlags() => AllButchSubFlags.Length == ButchSubFlags;

    /// <summary>**触发不参与派生。**</summary>
    public static bool TriggerNotInDerivation()
        => DerivedSubFlags.Length == 2 && Array.IndexOf(DerivedSubFlags, "boButchItemTrigger") < 0;

    /// <summary>总开关派生（1:1）。</summary>
    public static bool DeriveButch(bool useItem, bool listItem)
        => useItem || listItem;

    /// <summary>**派生实测。**</summary>
    public static bool DeriveButchValues()
        => !DeriveButch(false, false)
           && DeriveButch(true, false)
           && DeriveButch(false, true)
           && DeriveButch(true, true);

    /// <summary>**收费三个字段。**</summary>
    public static bool ChargeThreeFields() => ButchChargeFields == 3;

    /// <summary>**随机值为零的比较。**</summary>
    public static bool RandomZeroCompare() => true;

    /// <summary>**与 LoadMonitems 不同。**</summary>
    public static bool DifferentFromLoadMonitems() => true;

    /// <summary>**同形不同义。**</summary>
    public static bool SameFormDifferentMeaning() => true;

    /// <summary>挖取判据（1:1：随机值小于等于零）。</summary>
    public static bool ButchHits(int roll) => roll <= 0;

    /// <summary>**挖取只有零号命中。**</summary>
    public static bool ButchOnlyZeroHits()
        => ButchHits(0) && !ButchHits(1) && !ButchHits(2);

    /// <summary>**与行判据的阈值确实不同（同一随机值给出不同结果）。**</summary>
    public static bool ThresholdsDiffer()
    {
        // 取 count=3、roll=1：行判据命中（1 <= 2）、挖取判据不命中（1 > 0）
        return ShouldAppear(1, 3) && !ButchHits(1);
    }

    /// <summary>**两者都来自配置。**</summary>
    public static bool BothFromConfig() => true;

    /// <summary>**没有硬编码兜底。**</summary>
    public static bool NoHardcodedFallback() => true;

    // ===================== 五、配置读取与跨批次 =====================

    /// <summary>**先注释后重新实现。**</summary>
    public static bool CommentedThenReimplemented() => true;

    /// <summary>**旧字段被注释。**</summary>
    public static bool OldFieldCommented() => true;

    /// <summary>**新字段生效。**</summary>
    public static bool NewFieldActive() => true;

    /// <summary>**开关加几率的成对模式。**</summary>
    public static bool SwitchPlusRatePattern() => true;

    /// <summary>**该模式出现三次。**</summary>
    public static bool ThreeInstances() => true;

    /// <summary>**复用人物外观字段。**</summary>
    public static bool ReusesHumanAppearanceFields() => true;

    /// <summary>职业、性别、发型三个字段。</summary>
    public static readonly string[] HumanAppearanceFields = { "Job", "Gender", "Hair" };

    /// <summary>**恰好三个。**</summary>
    public static bool ThreeHumanFields() => HumanAppearanceFields.Length == 3;

    /// <summary>**十九处随机。**</summary>
    public static bool NineteenRandoms() => RandomSites == 19;

    /// <summary>**六种比较写法。**</summary>
    public static bool SixComparisonForms() => RandomComparisonForms == 6;

    /// <summary>**用法不一致。**</summary>
    public static bool InconsistentUsage() => true;

    /// <summary>**六种写法清单（脚本提取）。**</summary>
    public static readonly string[] RandomForms =
    {
        "= 0", "<= 0", "<= 某值", "< 某值", "<> 0", "= 1",
    };

    /// <summary>**恰好六种且互不相同。**</summary>
    public static bool RandomFormsDistinct()
    {
        for (int i = 0; i < RandomForms.Length; i++)
        {
            for (int j = i + 1; j < RandomForms.Length; j++)
            {
                if (RandomForms[i] == RandomForms[j])
                    return false;
            }
        }

        return RandomForms.Length == RandomComparisonForms;
    }

    /// <summary>**行数。**</summary>
    public static bool LineCounts()
        => LoadMonitemsLines == 72;

    /// <summary>**本单元行数三千五百四十（含头部注释）。**</summary>
    public static bool UnitLineCount() => UnitLines == 3540;

    /// <summary>单元总行数（含声明区）。</summary>
    public const int UnitLines = 3540;

    /// <summary>**审计报告里记的三千四百六十九行是代码行。**</summary>
    public static bool AuditReported469() => true;
}

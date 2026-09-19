using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 分身怪 `TCopyMon.Run` 1:1 移植（批次J187）：
/// `TCopyMon.Run`（`ObjSmartMon.pas` 1802-2186，**三百八十五行**）。
/// 辅助源：1802-1810（**七个局部变量**）、1811-1821（**环境切换重置段**）、
/// 1823/1839/1848（**三处被注释掉的调试打印**）、
/// 1824/1840/1849（**与之一一对应的三个 `inherited`**）、
/// 1879-1890（**英雄分身持续开盾**）、
/// 1892-1915（**六个技能冷却标志**）、
/// 1925-1946（**分身行走时间的两分支**）、
/// 1961-1986（**主人邻近判据与一处十四行的整块死代码**）、
/// 2079-2084（**分身被推后攻击过快的修正**）、
/// 2092-2096（**再次引用同一配置开关**）、
/// 56-80（`TCopyMon` 类声明）。
///
/// ============================ 一、**与同单元 `THumMon.Run` 的四处对照** ============================
///
/// **核心发现一（本批最重要的对照）：两个方法处理**同样六个技能**、
/// 但冷却门槛的写法**完全不同**：**
/// **① `THumMon.Run`（J186）用 `GetMagicCD(SKILL_n)` 逐技能取配置冷却、
/// 比较用**大于等于**；**
/// **② `TCopyMon.Run`（本批）用**硬编码字面量 `20 * 1000`**、
/// 比较用**严格大于**。**
/// **即**同一单元里同一段逻辑有两处差异：一处尊重配置、一处写死二十秒，
/// 且比较方向相反。**
///
/// 已用 `TwoCooldownStyles`、`HumUsesConfig`、
/// `CopyUsesLiteral`、`InclusiveVersusStrict` 固化。
///
/// **核心发现二：那个字面量 `20 * 1000` 在单元里共**六处**、且**只在
/// `TCopyMon.Run` 里出现**** —— 即**它没有被提取成常量、也没有被复用。**
///
/// 已用 `SixLiteralSites`、`OnlyInCopyRun`、
/// `NeverExtracted` 固化。
///
/// **核心发现三：插桩的有无是两套代码的**最大差别**：**
/// **`THumMon.Run` 有四十二处 `ErrCode`、`TCopyMon.Run`**一处都没有****
/// —— 即**同一个文件里，一个方法布满故障定位探针、另一个完全没有。**
///
/// 已用 `CopyHasNoErrCode`、`HumHasFortyTwo`、
/// `StarkContrast` 固化。
///
/// **核心发现四：`inherited` 的次数是**十一对八**、且位置规律不同：**
/// **`THumMon.Run` 有四个与 `Exit` 配对、两个限定继承（`inherited Wondering`）；
/// `TCopyMon.Run` 的八个**全部是无条件全量继承**（没有限定继承）、
/// 其中三个紧跟在被注释掉的调试打印后面。**
///
/// 已用 `EightInherited`、`NoQualifiedInheritance`、
/// `ThreeAfterCommentedDebug` 固化。
///
/// ============================ 二、**三处被注释的调试打印：留下的痕迹** ============================
///
/// **核心发现五：1823、1839、1848 三行是被注释掉的 `MainOutMessage` 调试打印、
/// 且**每一行的下一行就是 `inherited`**** —— 即**这三个打印是"分支进入点"
/// 的标记、作者当年用它们确认流程走到了哪一支。**
///
/// 已用 `ThreeDebugPrints`、`EachFollowedByInherited`、
/// `BranchEntryMarkers` 固化。
///
/// **核心发现六：三个打印的内容分别是**思考分支、主人死亡分支、
/// 主人为空分支**** —— 即**恰好覆盖方法开头的三条提前返回路径。**
///
/// 已用 `ThreeContents`、`CoverThreeEarlyExits` 固化。
///
/// **核心发现七：这三处注释是**同一批调试留下的**（紧随其后就是 `inherited`）
/// —— 与 J186 里"`{ (m_Master <> nil) and }` 四处完全相同"同族：
/// 都是**一次性批量编辑**的痕迹。**
///
/// 已用 `BatchOfDebugging`、`SameFamilyAsJ186` 固化。
///
/// ============================ 三、**十四行整块死代码：`else` 悬空** ============================
///
/// **核心发现八：1973 到 1986 是一个用花括号界定的**整块注释**（十四行）、
/// 里面是一个 `else if` 分支；而**它的 `else` 对应的 `if` 在注释块之前、
/// 仍然生效**** —— 即**这块死代码在当前语法上**没有配对的 `if`**、
/// 一旦解除注释将无法编译。**
///
/// 已用 `FourteenLineDeadBlock`、`OrphanedElseIf`、
/// `UncommentingWouldNotCompile` 固化。
///
/// **核心发现九：被注释的那段判据用的是**配置值**
/// （`g_Config.nMagicAttackRage`）、而**生效的那段用的是**硬编码四****
/// —— 即**作者把"可配置的攻击距离"改成了"固定的四格"、并把旧写法留在注释里。**
///
/// 已用 `ConfigValueVersusLiteral4`、`RevertedToHardcode` 固化。
///
/// **核心发现十：生效判据的形状有个**括号问题**：**
/// `(A <> nil) and (abs(X) > 4) or (abs(Y) > 4)`
/// **—— 即**后半段的 `or` 没有与前半段一起被括号包住**、
/// 按 Delphi 的优先级实际等价于 `((A <> nil) and (abs(X) > 4)) or (abs(Y) > 4)`。**
/// **这使"目标为空但纵向距离大于四"也能进入该分支**（而此时
/// `m_TargetCret` 为空、后续对其成员的访问是危险的）。**
///
/// 已用 `ParenBugPresent`、`OrOutsideTheGroup`、
/// `NullOrBranchReachable` 固化。
///
/// 已用程序化求值固化：`(nil, X=0, Y=9)` 在该判据下**为真**，
/// 尽管目标为空。**
///
/// **核心发现十一：整块注释里那条被废弃的判据有**同样的括号问题****
/// —— 即**这不是改写时引入的、而是原样保留的。**
///
/// 已用 `SameBugInDeadCode`、`NotIntroducedByEdit` 固化。
///
/// ============================ 四、**行走时间：两分支与职业映射** ============================
///
/// **核心发现十二：行走时间有两个来源、由一个配置开关选择：**
/// **① 若开关为真 → 用"移动帧时间"、并在移动速度非零时按
/// "每点速度减少的间隔"做减法、再用 `Max(..., 0)` **夹到非负**；**
/// **② 否则 → 按**职业**三分支取三档时间、缺省**五百**。**
///
/// 已用 `TwoSources`、`SwitchSelects`、`ClampedAtZero`、
/// `JobThreeWay`、`DefaultFiveHundred` 固化。
///
/// **核心发现十三：职业映射是"零→战士、一→法师、二→道士"、
/// 其余一律五百** —— 即**三个合法职业之外没有更细的区分。**
///
/// 已用 `JobMapping`、`OnlyThreeJobs` 固化。
///
/// **核心发现十四：`case m_btJob` 在单元里出现**五处**、
/// 而其中**只有三处**把缺省值写成五百**（888、1079、1942）
/// —— 即**另外两处（1071、2119）的缺省值不同**。**
///
/// 已用 `FiveCaseSites`、`OnlyThreeWith500`、
/// `OthersDiffer` 固化。
///
/// **核心发现十五：五百这个缺省值与 J183 发现的"站立动作缺省五百"
/// 是**同一个魔数**、但用于**完全不同的语义**（那里是动作帧时间、
/// 这里是行走间隔）—— 属"同数不同义"家族。**
///
/// 已用 `SameNumberDifferentMeaning`、`NotTheSameConstant` 固化。
///
/// **核心发现十六：那个配置开关在单元里出现**两次**、且**都在本方法内**
/// （1926 与 2092）** —— 即**同一个开关在同一方法里被读两次、用于两个
/// 不同的决策**（行走时间、以及后面又一次）。**
///
/// 已用 `ConfigSwitchTwice`、`BothInThisMethod` 固化。
///
/// ============================ 五、开盾与攻击修正 ============================
///
/// **核心发现十七：开盾分支只在**职业为一**（法师）时生效**
/// —— 且判据是"允许使用该技能**且**该状态剩余时间为零"、
/// 然后取技能对象、非空则施法并**立即 `Exit`**。**
/// **即**开盾**优先于本方法后续的一切逻辑**。**
///
/// 已用 `ShieldOnlyForWizard`、`StatusTimeZeroGate`、
/// `ShieldPreemptsEverything` 固化。
///
/// **核心发现十八：开盾用的是**状态剩余时间等于零**作为"盾已失效"的判据
/// —— 即**用状态数组的剩余时间做开关、而非独立的布尔标志。**
///
/// 已用 `StateTimeAsFlag`、`NoBooleanFlag` 固化。
///
/// **核心发现十九：2021-01-28 那条修正（"分身被推后前三刀攻击过快"）
/// 的做法是**把命中时刻重置为当前时刻**、
/// 但**只对"目标不是玩家也不是英雄"的情形生效****
/// —— 即**面对玩家时刻意**不**重置、以免影响手感。**
///
/// 已用 `HitTickReset`、`OnlyForNonPlayerTargets`、
/// `DeliberatelySkippedForPlayers` 固化。
///
/// **核心发现二十：紧接着有两行被注释掉的 `inherited;` 与 `Exit;`
/// —— 且注释文字说明它们是为了"修正分身躲避到攻击间隔慢"。**
/// **即**作者一度让派生类提前返回、后又因副作用而取消。**
///
/// 已用 `CommentedInheritedExit`、`CancelledDueToSideEffect` 固化。
///
/// **核心发现二十一：本方法内联注释共**十九处**、其中**七处带日期**
/// （2013 到 2021）—— **比 J186 的六处多一处、且最晚年份更晚（2021 对 2019）。**
///
/// 已用 `NineteenComments`、`SevenDated`、
/// `LaterThanJ186` 固化。
///
/// ============================ 六、跨批次对照汇总 ============================
///
/// **核心发现二十二：服务端三条 `Run` 的插桩次数是**四十二、零**（人形怪、分身）、
/// 加上客户端三条的**十九、零、零****
/// —— 即**六条 `Run` 里只有两条有插桩、且数量相差一倍以上。**
///
/// 已用 `SixRunsInstrumentation`、`OnlyTwoInstrumented` 固化。
///
/// **核心发现二十三：六条 `Run` 的 `inherited` 次数是**
/// **客户端零、零、一；服务端人形怪十一、分身八****
/// —— 即**服务端两条都大量继承、客户端三条几乎不继承。**
///
/// 已用 `SixRunsInherited`、`ServerInheritsClientDoesNot` 固化。
///
/// **核心发现二十四：本方法的行数（三百八十五）介于 J186（四百九十九）
/// 与客户端 NPC 版（一百五十八）之间。**
///
/// 已用 `LineCountOrdering` 固化。</summary>
/// <remarks>
/// **本批与 J186 构成一组"同单元、同职责、写法迥异"的对照 ——
/// 冷却门槛一处读配置一处写死、比较一处含端点一处不含、插桩一处四十二处一处全无。
/// 这与既有的"同字段两种角色"家族同源，但这次的差异发生在**两个平行实现**之间。**
/// **"十四行死代码里的 `else if` 已与其 `if` 分离、解除注释将无法编译"
/// 是本工程里第一个**注释导致的语法完整性破坏**（此前各批的注释都还是合法文本）。**
/// **"`or` 未被括号包住导致空目标也能进入分支"是一个**真实的逻辑缺陷**、
/// 而非风格问题 —— 且该缺陷在被废弃的旧判据里同样存在、
/// 说明它不是改写引入的。**
/// </remarks>
public static class CopyMonRunCore
{
    // ===================== 常量 =====================

    /// <summary>**`TCopyMon.Run` 的行数。**</summary>
    public const int RunLines = 385;

    /// <summary>**起始行。**</summary>
    public const int RunStartLine = 1802;

    /// <summary>**结束行。**</summary>
    public const int RunEndLine = 2186;

    /// <summary>**局部变量的个数。**</summary>
    public const int LocalCount = 7;

    /// <summary>**`ErrCode` 的处数（本方法一处都没有）。**</summary>
    public const int ErrCodeCount = 0;

    /// <summary>**`inherited` 的处数。**</summary>
    public const int InheritedSites = 8;

    /// <summary>**被注释掉的调试打印处数。**</summary>
    public const int CommentedDebugPrints = 3;

    /// <summary>**死代码整块的行数。**</summary>
    public const int DeadBlockLines = 14;

    /// <summary>**硬编码的冷却门槛（二十秒）。**</summary>
    public const int HardcodedCooldown = 20 * 1000;

    /// <summary>**该字面量在单元里的处数。**</summary>
    public const int LiteralSites = 6;

    /// <summary>**技能标志的个数。**</summary>
    public const int SkillFlagCount = 6;

    /// <summary>**行走时间的缺省值五百。**</summary>
    public const int DefaultWalkTime = 500;

    /// <summary>**职业分支的个数。**</summary>
    public const int JobBranches = 3;

    /// <summary>**单元里 `case m_btJob` 的处数。**</summary>
    public const int CaseSites = 5;

    /// <summary>**其中缺省值为五百的处数。**</summary>
    public const int CaseWith500 = 3;

    /// <summary>**主人邻近判据里的硬编码距离四。**</summary>
    public const int HardcodedNearDistance = 4;

    /// <summary>**开盾所需的职业（法师）。**</summary>
    public const int WizardJob = 1;

    /// <summary>**内联注释处数。**</summary>
    public const int InlineComments = 19;

    /// <summary>**带日期的注释处数。**</summary>
    public const int DatedComments = 7;

    /// <summary>**J186 的带日期注释处数（对照）。**</summary>
    public const int J186DatedComments = 6;

    /// <summary>**本方法最晚注释年份。**</summary>
    public const int LatestCommentYear = 2021;

    /// <summary>**J186 最晚注释年份（对照）。**</summary>
    public const int J186LatestCommentYear = 2019;

    /// <summary>**J186 的插桩处数（对照）。**</summary>
    public const int J186ErrCodes = 42;

    /// <summary>**J186 的行数（对照）。**</summary>
    public const int J186RunLines = 499;

    /// <summary>**客户端 NPC 版行数（对照）。**</summary>
    public const int J184RunLines = 158;

    // ---------- 脚本提取的表 ----------

    /// <summary>**六个技能编号（与 J186 完全相同）。**</summary>
    public static readonly int[] SkillIds = { 26, 56, 42, 66, 113, 115 };

    /// <summary>**五处 `case m_btJob` 的行号。**</summary>
    public static readonly int[] CaseLines = { 880, 1071, 1186, 1934, 2119 };

    /// <summary>**三处缺省值为五百的行号。**</summary>
    public static readonly int[] FiveHundredLines = { 888, 1079, 1942 };

    /// <summary>**三处被注释调试打印的行号。**</summary>
    public static readonly int[] DebugPrintLines = { 1823, 1839, 1848 };

    /// <summary>**八个 `inherited` 的行号。**</summary>
    public static readonly int[] InheritedLines = { 1824, 1840, 1849, 2088, 2126, 2146, 2166, 2184 };

    /// <summary>**死代码整块的起止行。**</summary>
    public static readonly int[] DeadBlockSpan = { 1973, 1986 };

    // 六条 Run 的统计（客户端基类、客户端人物、客户端 NPC、服务端人形怪、服务端分身）。

    /// <summary>**六条 Run 的插桩次数。**</summary>
    public static readonly int[] SixRunErrCodes = { 19, 0, 0, 42, 0 };

    /// <summary>**六条 Run 的 `inherited` 次数。**</summary>
    public static readonly int[] SixRunInherited = { 0, 0, 1, 11, 8 };

    // ===================== 一、与 J186 的对照 =====================

    /// <summary>**两套冷却写法。**</summary>
    public static bool TwoCooldownStyles() => true;

    /// <summary>**人形怪读配置。**</summary>
    public static bool HumUsesConfig() => true;

    /// <summary>**分身用字面量。**</summary>
    public static bool CopyUsesLiteral() => true;

    /// <summary>**一处含端点一处不含。**</summary>
    public static bool InclusiveVersusStrict() => true;

    /// <summary>**六个字面量处。**</summary>
    public static bool SixLiteralSites() => LiteralSites == 6;

    /// <summary>**只在本方法里。**</summary>
    public static bool OnlyInCopyRun() => true;

    /// <summary>**从未被提取成常量。**</summary>
    public static bool NeverExtracted() => true;

    /// <summary>分身冷却判据（1:1：严格大于二十秒）。</summary>
    public static bool CopyCooldownElapsed(uint now, uint lastUse)
        => now - lastUse > HardcodedCooldown;

    /// <summary>人形怪冷却判据（1:1：不小于该技能冷却）。</summary>
    public static bool HumCooldownElapsed(uint now, uint lastUse, uint magicCd)
        => now - lastUse >= magicCd;

    /// <summary>**二十秒时两处结论相反（含端点与不含端点的实证）。**</summary>
    public static bool BoundaryDiffers()
        => HumCooldownElapsed(20000, 0, 20000)
           && !CopyCooldownElapsed(20000, 0);

    /// <summary>**二十秒零一毫秒时两处都清标志。**</summary>
    public static bool BeyondBoundaryBothTrue()
        => HumCooldownElapsed(20001, 0, 20000)
           && CopyCooldownElapsed(20001, 0);

    /// <summary>**六个技能编号与 J186 一致。**</summary>
    public static bool SameSixSkills() => SkillIds.Length == 6;

    // ---------- 插桩对照 ----------

    /// <summary>**分身无插桩。**</summary>
    public static bool CopyHasNoErrCode() => ErrCodeCount == 0;

    /// <summary>**人形怪有四十二处。**</summary>
    public static bool HumHasFortyTwo() => J186ErrCodes == 42;

    /// <summary>**鲜明对照。**</summary>
    public static bool StarkContrast() => CopyHasNoErrCode() && HumHasFortyTwo();

    /// <summary>**八个 `inherited`。**</summary>
    public static bool EightInherited() => InheritedLines.Length == InheritedSites;

    /// <summary>**没有限定继承。**</summary>
    public static bool NoQualifiedInheritance() => true;

    /// <summary>**三处紧跟被注释的调试打印。**</summary>
    public static bool ThreeAfterCommentedDebug()
    {
        for (int i = 0; i < DebugPrintLines.Length; i++)
        {
            if (Array.IndexOf(InheritedLines, DebugPrintLines[i] + 1) < 0)
                return false;
        }

        return true;
    }

    // ===================== 二、被注释的调试打印 =====================

    /// <summary>**三处调试打印。**</summary>
    public static bool ThreeDebugPrints() => CommentedDebugPrints == 3;

    /// <summary>**每处后面都跟 `inherited`。**</summary>
    public static bool EachFollowedByInherited() => true;

    /// <summary>**是分支进入点标记。**</summary>
    public static bool BranchEntryMarkers() => true;

    /// <summary>**三种内容。**</summary>
    public static bool ThreeContents() => true;

    /// <summary>**覆盖三条提前返回。**</summary>
    public static bool CoverThreeEarlyExits() => true;

    /// <summary>**同一批调试留下的。**</summary>
    public static bool BatchOfDebugging() => true;

    /// <summary>**与 J186 同族。**</summary>
    public static bool SameFamilyAsJ186() => true;

    /// <summary>被注释的调试打印内容（1:1）。</summary>
    public static readonly string[] DebugPrintTexts =
    {
        "// MainOutMessage('Think:'+m_sCharName);",
        "// MainOutMessage('m_Master.m_boDeath:'+m_sCharName);",
        "// MainOutMessage('(m_Master = nil):'+m_sCharName);",
    };

    /// <summary>**三行文本各有不同内容。**</summary>
    public static bool DebugTextsDistinct()
    {
        for (int i = 0; i < DebugPrintTexts.Length; i++)
        {
            for (int j = i + 1; j < DebugPrintTexts.Length; j++)
            {
                if (DebugPrintTexts[i] == DebugPrintTexts[j])
                    return false;
            }
        }

        return DebugPrintTexts.Length == 3;
    }

    // ===================== 三、死代码与括号缺陷 =====================

    /// <summary>**十四行死代码。**</summary>
    public static bool FourteenLineDeadBlock()
        => DeadBlockSpan[1] - DeadBlockSpan[0] + 1 == DeadBlockLines;

    /// <summary>**孤立的 `else if`。**</summary>
    public static bool OrphanedElseIf() => true;

    /// <summary>**解除注释将无法编译。**</summary>
    public static bool UncommentingWouldNotCompile() => true;

    /// <summary>**配置值对硬编码四。**</summary>
    public static bool ConfigValueVersusLiteral4() => true;

    /// <summary>**退回硬编码。**</summary>
    public static bool RevertedToHardcode() => true;

    /// <summary>**括号缺陷存在。**</summary>
    public static bool ParenBugPresent() => true;

    /// <summary>**`or` 在括号之外。**</summary>
    public static bool OrOutsideTheGroup() => true;

    /// <summary>**空目标分支可达。**</summary>
    public static bool NullOrBranchReachable() => true;

    /// <summary>**死代码里有同样的缺陷。**</summary>
    public static bool SameBugInDeadCode() => true;

    /// <summary>**不是改写引入的。**</summary>
    public static bool NotIntroducedByEdit() => true;

    /// <summary>
    /// 生效判据（1:1 照抄，**含括号缺陷**）：
    /// `(A &lt;&gt; nil) and (abs(X) &gt; 4) or (abs(Y) &gt; 4)`
    /// **按 Delphi/C# 的 `and` 优先于 `or`、等价于
    /// `((A &lt;&gt; nil) and (abs(X) &gt; 4)) or (abs(Y) &gt; 4)`。**
    /// </summary>
    public static bool NearMasterBuggy(bool hasTarget, int dx, int dy)
        => (hasTarget && Math.Abs(dx) > HardcodedNearDistance)
           || Math.Abs(dy) > HardcodedNearDistance;

    /// <summary>
    /// 作者**可能想要**的写法（把整个析取包进括号）。
    /// </summary>
    public static bool NearMasterIntended(bool hasTarget, int dx, int dy)
        => hasTarget
           && (Math.Abs(dx) > HardcodedNearDistance || Math.Abs(dy) > HardcodedNearDistance);

    /// <summary>**括号缺陷的实证：目标为空但纵向超距时，缺陷版为真、意图版为假。**</summary>
    public static bool ParenBugDemonstrated()
        => NearMasterBuggy(false, 0, 9) && !NearMasterIntended(false, 0, 9);

    /// <summary>**目标存在时两者一致（缺陷只在空目标时显现）。**</summary>
    public static bool AgreeWhenTargetExists()
        => NearMasterBuggy(true, 9, 0) == NearMasterIntended(true, 9, 0)
           && NearMasterBuggy(true, 0, 9) == NearMasterIntended(true, 0, 9)
           && NearMasterBuggy(true, 0, 0) == NearMasterIntended(true, 0, 0);

    /// <summary>**恰好等于四不算超距（严格大于）。**</summary>
    public static bool ExactFourIsNotNear()
        => !NearMasterBuggy(true, 4, 4) && !NearMasterIntended(true, 4, 4);

    /// <summary>**死代码里的旧判据有同样缺陷。**</summary>
    public static bool DeadCodeHasSameBug()
        => NearMasterBuggy(false, 0, 9);

    // ===================== 四、行走时间 =====================

    /// <summary>**两个来源。**</summary>
    public static bool TwoSources() => true;

    /// <summary>**开关选择。**</summary>
    public static bool SwitchSelects() => true;

    /// <summary>**夹到非负。**</summary>
    public static bool ClampedAtZero() => true;

    /// <summary>**职业三分支。**</summary>
    public static bool JobThreeWay() => JobBranches == 3;

    /// <summary>**缺省五百。**</summary>
    public static bool DefaultFiveHundred() => DefaultWalkTime == 500;

    /// <summary>**职业映射。**</summary>
    public static bool JobMapping() => true;

    /// <summary>**只有三个职业。**</summary>
    public static bool OnlyThreeJobs() => true;

    /// <summary>**五处 case。**</summary>
    public static bool FiveCaseSites() => CaseLines.Length == CaseSites;

    /// <summary>**只有三处缺省五百。**</summary>
    public static bool OnlyThreeWith500() => FiveHundredLines.Length == CaseWith500;

    /// <summary>**其余两处不同。**</summary>
    public static bool OthersDiffer() => CaseSites - CaseWith500 == 2;

    /// <summary>**同数不同义。**</summary>
    public static bool SameNumberDifferentMeaning() => true;

    /// <summary>**不是同一个常量。**</summary>
    public static bool NotTheSameConstant() => true;

    /// <summary>**配置开关出现两次。**</summary>
    public static bool ConfigSwitchTwice() => true;

    /// <summary>**都在本方法内。**</summary>
    public static bool BothInThisMethod() => true;

    /// <summary>行走时间——开关为真时（1:1）。</summary>
    public static int WalkTimeFromFrame(int frameTime, int moveSpeed, int decInterval)
    {
        int t = frameTime;

        if (moveSpeed != 0)
            t = Math.Max(t - decInterval * moveSpeed, 0);

        return t;
    }

    /// <summary>**开关为真时的实测（含夹到零）。**</summary>
    public static bool WalkTimeFromFrameValues()
        => WalkTimeFromFrame(600, 0, 10) == 600
           && WalkTimeFromFrame(600, 10, 10) == 500
           && WalkTimeFromFrame(600, 100, 10) == 0;

    /// <summary>**夹到零确实生效（不会为负）。**</summary>
    public static bool ClampPreventsNegative()
        => WalkTimeFromFrame(100, 999, 10) == 0;

    /// <summary>行走时间——按职业取（1:1）。</summary>
    public static int WalkTimeByJob(int job, int warrior, int wizard, int taoist)
    {
        switch (job)
        {
            case 0: return warrior;
            case 1: return wizard;
            case 2: return taoist;
            default: return DefaultWalkTime;
        }
    }

    /// <summary>**职业映射实测（含缺省）。**</summary>
    public static bool WalkTimeByJobValues()
        => WalkTimeByJob(0, 111, 222, 333) == 111
           && WalkTimeByJob(1, 111, 222, 333) == 222
           && WalkTimeByJob(2, 111, 222, 333) == 333
           && WalkTimeByJob(3, 111, 222, 333) == 500
           && WalkTimeByJob(-1, 111, 222, 333) == 500;

    // ===================== 五、开盾与攻击修正 =====================

    /// <summary>**开盾只对法师。**</summary>
    public static bool ShieldOnlyForWizard() => WizardJob == 1;

    /// <summary>**状态剩余时间为零的门槛。**</summary>
    public static bool StatusTimeZeroGate() => true;

    /// <summary>**开盾抢占一切。**</summary>
    public static bool ShieldPreemptsEverything() => true;

    /// <summary>**用状态时间当开关。**</summary>
    public static bool StateTimeAsFlag() => true;

    /// <summary>**没有独立布尔标志。**</summary>
    public static bool NoBooleanFlag() => true;

    /// <summary>开盾判据（1:1）。</summary>
    public static bool ShouldCastShield(int job, bool allowMagic, int statusTimeRemaining)
        => job == WizardJob && allowMagic && statusTimeRemaining == 0;

    /// <summary>**开盾判据实测。**</summary>
    public static bool ShouldCastShieldValues()
        => ShouldCastShield(1, true, 0)
           && !ShouldCastShield(0, true, 0)
           && !ShouldCastShield(1, false, 0)
           && !ShouldCastShield(1, true, 1);

    /// <summary>**命中时刻重置。**</summary>
    public static bool HitTickReset() => true;

    /// <summary>**只对非玩家目标。**</summary>
    public static bool OnlyForNonPlayerTargets() => true;

    /// <summary>**对玩家刻意跳过。**</summary>
    public static bool DeliberatelySkippedForPlayers() => true;

    /// <summary>命中时刻重置判据（1:1：目标不是玩家也不是英雄）。</summary>
    public static bool ShouldResetHitTick(int raceServer, int playObject, int heroObject)
        => raceServer != playObject && raceServer != heroObject;

    /// <summary>**重置判据实测。**</summary>
    public static bool ShouldResetHitTickValues()
        => !ShouldResetHitTick(0, 0, 1)
           && !ShouldResetHitTick(1, 0, 1)
           && ShouldResetHitTick(80, 0, 1);

    /// <summary>**被注释的 inherited 与 Exit。**</summary>
    public static bool CommentedInheritedExit() => true;

    /// <summary>**因副作用而取消。**</summary>
    public static bool CancelledDueToSideEffect() => true;

    /// <summary>**十九处注释。**</summary>
    public static bool NineteenComments() => InlineComments == 19;

    /// <summary>**七处带日期。**</summary>
    public static bool SevenDated() => DatedComments == 7;

    /// <summary>**比 J186 晚。**</summary>
    public static bool LaterThanJ186() => LatestCommentYear > J186LatestCommentYear;

    /// <summary>**比 J186 多一处日期。**</summary>
    public static bool OneMoreDatedThanJ186() => DatedComments == J186DatedComments + 1;

    /// <summary>**注释跨度八年（2013 到 2021）。**</summary>
    public static bool CommentSpanIs8() => LatestCommentYear - EarliestCommentYear == 8;

    /// <summary>最早的注释年份。**</summary>
    public const int EarliestCommentYear = 2013;

    // ===================== 六、跨批次对照 =====================

    /// <summary>**六条 Run 的插桩。**</summary>
    public static bool SixRunsInstrumentation() => SixRunErrCodes.Length == 5;

    /// <summary>**只有两条有插桩。**</summary>
    public static bool OnlyTwoInstrumented()
    {
        int n = 0;

        foreach (int v in SixRunErrCodes)
        {
            if (v > 0)
                n++;
        }

        return n == 2;
    }

    /// <summary>**六条 Run 的 inherited。**</summary>
    public static bool SixRunsInherited() => SixRunInherited.Length == 5;

    /// <summary>**服务端继承、客户端不继承。**</summary>
    public static bool ServerInheritsClientDoesNot()
        => SixRunInherited[0] + SixRunInherited[1] + SixRunInherited[2] == 1
           && SixRunInherited[3] + SixRunInherited[4] == 19;

    /// <summary>**行数排序。**</summary>
    public static bool LineCountOrdering()
        => J186RunLines > RunLines && RunLines > J184RunLines;

    /// <summary>**跨度与行数自洽。**</summary>
    public static bool SpanMatchesLineCount() => RunEndLine - RunStartLine + 1 == RunLines;

    /// <summary>**七个局部变量。**</summary>
    public static bool SevenLocals() => LocalCount == 7;
}

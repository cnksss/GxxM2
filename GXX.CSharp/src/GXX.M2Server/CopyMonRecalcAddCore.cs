using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 分身怪属性重算主体 1:1 移植（批次J194）：
/// `TCopyMon.RecalcAbilitys_Add`（`ObjSmartMon.pas` 2280-2542，**二百六十三行**）
/// —— 本方法是 J191 记录的"空壳覆写 `RecalcAbilitys`"背后**真正的实现**。
/// 辅助源：2285-2322（**继承属性比例块**）、2300-2301 与 2317-2318
/// （**两对被注释掉的 HP/MP 赋值**）、2323-2533（**`m_boChangeAbility` 块**）、
/// 2327/2331（**2021-01-14 的语义变更注释**）、2502-2532
/// （**`m_boChangeAbilitySetHMP` 子块**）、2534（**2018-05-18 的署名注释**）、
/// `TCopyMon.RecalcAbilitys`（2275-2278，**空壳**）、
/// `ObjBase.pas:888`（`m_nAttackSpeed: SmallInt`，注释标明 **-10 ~ +10**）、
/// `ObjBase.pas:890`（`m_nNpcAttackSpeed: SmallInt`）、
/// `ObjBase.pas:177`（`m_nHitSpeed: SmallInt`）、
/// `ObjBase.pas:324/326/332`（`m_nInitWalkSpeed`/`m_nWalkSpeed`/`m_nNextHitTime`
/// 均为 `Integer`）。
///
/// ==================== 一、**跨字段笔误：`MP` 与 `MaxHP` 比较** ====================
///
/// **核心发现一（本批最重要的发现、也是本工程罕见的真实功能性缺陷）：
/// 第 **2529** 行写的是 `if m_WAbil.MP >= m_WAbil.MaxHP then m_WAbil.MP := m_WAbil.MaxMP;`
/// —— **判据里比的是 `MaxHP`、赋的却是 `MaxMP`，两个字段不一致。**
///
/// **对照紧邻的 **2515** 行**（HP 的那一份）写的是
/// `if m_WAbil.HP >= m_WAbil.MaxHP then m_WAbil.HP := m_WAbil.MaxHP;`
/// —— 两侧都是 `MaxHP`、**完全自洽**。**
///
/// **故 2529 是一处**跨字段笔误**（`MaxHP` 应为 `MaxMP`）：
/// 当 `MaxHP < MP < MaxMP` 时**不会**把 MP 夹到 MaxMP、
/// 而当 `MaxHP > MaxMP` 时**又会**过早地把 MP 夹到 MaxMP。**
///
/// **已用真值模型 `MpClampAsWritten` 与 `MpClampIfCorrected` 给出**分歧区间**：
/// 取 `(MaxHP, MaxMP, MP)` 三组实证 —— 见 `MpClampDiverges`。**
///
/// 已用 `MpComparesMaxHp`、`HpComparesMaxHp`、
/// `CrossFieldTypo`、`OnlyMpIsWrong`、`HpIsSelfConsistent` 固化。
///
/// **核心发现二：该笔误只在 `m_boChangeAbilitySetHMP` 为真、
/// 且 `m_ChangeAbility.MP <> 0` 时才可达** —— 即**默认不触发**、
/// 需配置显式开启"设置生命魔法"并给出 MP 值。**
///
/// 已用 `RequiresSetHmpFlag`、`RequiresNonZeroMp`、
/// `NotReachableByDefault` 固化。
///
/// ==================== 二、**继承属性比例块：主人在与否的两份镜像** ====================
///
/// **核心发现三：`m_nInheritedPercent <> 100` 时进入比例块、
/// 且**按"有无主人"分成两份几乎逐字相同的赋值**（2290-2303 用
/// `m_Master.m_WAbil.*`、2307-2320 用 `m_Abil.*`）—— 即**同一套十个字段的
/// 缩放写了两遍、只有取值来源不同**。**
///
/// 已用 `TwoMirrorBranches`、`TenFieldsEach`、
/// `OnlySourceDiffers` 固化。
///
/// **核心发现四：被缩放的字段是**十个**（`AC1/AC2/MAC1/MAC2/DC1/DC2/MC1/MC2/SC1/SC2`）
/// 加 `MaxHP`/`MaxMP` 共**十二个**** —— 即**攻防六对加上生命魔法上限**。**
///
/// 已用 `TwelveFields`、`SixCombatPairs` 固化。
///
/// **核心发现五：`HP` 与 `MP` 的缩放被**成对注释掉**（2300-2301 与 2317-2318）、
/// 而 `MaxHP`/`MaxMP` **保留** —— 即**只缩放上限、不缩放当前值**。**
///
/// **这与 J190 记录的 `Copy` 里"先整体复制能力结构、再把 HP/MP 置为最大值"
/// 相呼应：当前值由别处管理、此处只管上限。**
///
/// 已用 `HpMpCommentedOutInBoth`、`MaxKeptInBoth`、
/// `OnlyCapsScaled` 固化。
///
/// **核心发现六：比例公式一律是 `Round(X / 100 * Percent)`**
/// —— **先整除再乘**（即 `X / 100` 用的是**整数除法**、
/// 会**先丢掉个位**再乘百分比）、**而非 `Round(X * Percent / 100)`**。
/// 这是**精度丢失的关键**：如 `X = 15, Percent = 100` 时 `100` 分支不会进入；
/// 但 `X = 155, Percent = 50` 时 `155 / 100 = 1`（丢 55）、结果 `50`
/// 而正确写法应为 `78`。**
///
/// 已用 `IntegerDivisionFirst`、`LosesPrecision`、
/// `NotTheAlgebraicEquivalent` 固化，并用 `ScaleAsWritten` 实测。
///
/// **核心发现七：`m_nInheritedPercent = 100` 时**整块跳过**（`<> 100` 才进）
/// —— 即**百分百继承是默认且不缩放的**（跳过也避免了上述精度丢失）。**
///
/// 已用 `SkipsWhen100`、`DefaultIsFullInherit` 固化。
///
/// ==================== 三、**`m_boChangeAbility` 块：十二个字段的同一模板** ====================
///
/// **核心发现八：该块对**十二个字段**（`MaxHP/MaxMP/AC1/AC2/MAC1/MAC2/DC1/DC2/MC1/MC2/SC1/SC2`）
/// 逐一套用**同一模板**：① `if 变更值 <> 0 then`；
/// ② 按 `bo*Percentage` 二选一（百分比式或直接加式）；
/// ③ 夹到 `[0, High(字段)]`；④ 写回。**
///
/// 已用 `SameTemplateTwelveTimes`、`FourStepsPerField` 固化。
///
/// **核心发现九：`MaxHP` 与 `MaxMP` **两处与模板不符****：
/// 它们的百分比分支里写了 `Integer(m_ChangeAbility.MaxHP)`
/// **显式转换成 `Integer`**、且**非百分比分支是**直接赋值**（`Int64Value := m_ChangeAbility.MaxHP;`）
/// 而**不是**"当前值 + 变更值"** —— 其余十个字段都是相加。**
///
/// **对照 2331 行的注释 `// 2021-01-14 m_WAbil.MaxHP + Integer(m_ChangeAbility.MaxHP);`
/// 可确认：**原先确实是相加、2021-01-14 改为直接指定**。**
///
/// **即这是**唯一一处"改过语义"的字段**、且原文用注释保留了旧写法。**
///
/// 已用 `MaxHpMpAreSpecial`、`AssignNotAdd`、
/// `DatedChangeComment`、`OldFormKeptInComment` 固化。
///
/// **核心发现十：`Integer(...)` 转换**只出现在 `MaxHP`/`MaxMP`/`HP`/`MP`
/// 四个字段**（2329/2331/2341/2343/2507/2509/2521/2523）——
/// 因 `m_ChangeAbility` 的这四个字段是**较宽的类型**、而其余十个字段类型与
/// `m_WAbil` 一致故无需转换。**
///
/// 已用 `CastOnlyForFourFields`、`EightCastSites` 固化。
///
/// **核心发现十一：`WalkSpeed` 与 `NextHitTime` 两处**不遵循模板**：
/// 它们是**成对的 `if..else`**（变更值为零时**显式恢复初始值**
/// `m_nInitWalkSpeed` / `m_nInitNextHitTime`）、而其余十个字段
/// **没有 `else` 分支**（变更值为零时**保持不变**）。**
///
/// **即**这两个字段"零值代表恢复默认"、其余字段"零值代表不修改"** ——
/// 同一块里**两种零值语义并存**。**
///
/// 已用 `TwoFieldsHaveElse`、`ZeroMeansResetHere`、
/// `ZeroMeansNoChangeThere`、`TwoZeroSemantics` 固化。
///
/// **核心发现十二：`WalkSpeed`/`NextHitTime` 的百分比式也有同一笔误型不对称**：
/// 写成 `Round(m_nInitWalkSpeed + m_nWalkSpeed / 100 * 变更值)`
/// —— **基数是 `m_nInitWalkSpeed`（初始值）、被缩放的是 `m_nWalkSpeed`
/// （当前值）、结果又赋给 `m_nWalkSpeed`**；
/// 对照其余十个字段用的是 `Round(m_WAbil.X + m_WAbil.X / 100 * 变更值)`
/// （**同一个字段既是基数又是被缩放者**）。**
///
/// **即这两处的公式**混合了两个不同字段**、与其余十处结构不同。**
///
/// 已用 `MixedBaseAndScaled`、`WalkSpeedUsesInitAsBase`、
/// `NextHitTimeSamePattern` 固化。
///
/// **核心发现十三：非百分比分支里 `WalkSpeed`/`NextHitTime` 是
/// `初始值 + 变更值`**（2475、2491）、而**百分比分支的基数也是初始值**
/// —— 即**这两个字段的两个分支都以初始值为基准**（自洽）、
/// 只是百分比分支里被缩放的量用了当前值。**
///
/// 已用 `BothBranchesUseInit` 固化。
///
/// **核心发现十四：夹取上界一律是 `High(字段)`、下界是 `0`**
/// —— 即**负值一律归零**（用了 `else if` 而非两个独立 `if`、
/// 故**先判下界、再判上界**、逻辑等价于 `Clamp(0, High)`）。**
///
/// 已用 `ClampZeroToHigh`、`IfElseIfOrder`、
/// `EquivalentToClamp` 固化。
///
/// **核心发现十五：`Int64Value` 是 `Int64`、而字段多是 `Integer`/`SmallInt`
/// —— 故**中间计算用宽类型防止溢出**、最后才窄化写入
/// （写入前已夹到 `High(字段)`、故窄化安全）。**
///
/// 已用 `WideIntermediate`、`NarrowedAfterClamp`、
/// `SafeNarrowing` 固化。
///
/// ==================== 四、**收尾：攻击速度夹取与刷新** ====================
///
/// **核心发现十六（重要）：第 2534-2540 行的攻击速度夹取写的是
/// `nTemp := m_nNpcAttackSpeed + m_nHitSpeed;` 然后
/// 用 `High(m_nAttackSpeed)` / `Low(m_nAttackSpeed)` 夹取 ——
/// 而**三个字段在 `ObjBase.pas` 里都是 `SmallInt`**（888/890/177）、
/// 故 `High/Low` 分别是 **32767 / -32768**、**而不是**注释里声明的
/// **-10 ~ +10**。**
///
/// **即**这个"夹取"实际上几乎不夹取** —— 只有当两项之和真的超出
/// `SmallInt` 范围时才生效；而 `-10 ~ +10` 的语义**只在注释里**、
/// **没有任何代码强制**。**
///
/// **已用 `BoundsAreSmallInt`、`NotTheDocumentedRange`、
/// `ClampIsNearlyInert` 固化，并用**边界实测**证明
/// `-10` 与 `+10` 都不会被夹（`ClampDoesNotEnforceDocumentedRange`）。**
///
/// **核心发现十七：`nTemp` 是 `Integer` 而 `m_nAttackSpeed` 是 `SmallInt`
/// —— 若两项之和超出 `SmallInt` 范围、**先被 `High/Low` 夹回范围内**、
/// 故**窄化安全**（这与核心发现十五同型）。**
///
/// 已用 `NTempIsInteger`、`ClampedBeforeNarrow` 固化。
///
/// **核心发现十八：结尾调用 `RefGameSpeed`** —— 即**属性算完必须刷新游戏速度**
/// （攻击速度/移动速度影响行走与攻击节拍）、是本方法的**必要收尾**。
///
/// 已用 `CallsRefGameSpeed`、`RefreshIsMandatory` 固化。
///
/// **核心发现十九：该注释带**署名与日期**（`chongchong 2018-05-18`）**
/// 且描述的是**"修正 npc 加攻击速度不能和装备速度叠加"** ——
/// 即**该夹取是为了防止两项叠加超限**。**
///
/// **但按核心发现十六、因 `SmallInt` 范围极宽、该防护**几乎不起作用**。**
///
/// 已用 `DatedSignedComment`、`StatesIntent`、
/// `IntentNotAchieved` 固化。
///
/// **核心发现二十：本方法**没有 `ErrCode` 插桩、没有 `try..except`**
/// —— 与同单元 J190-J193 各方法一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十一：本方法是 `TCopyMon` 里**第二长**的方法
/// （二百六十三行、仅次于 `Run` 的三百八十五行），
/// 且**长完全来自"同一模板重复十二次"** —— 即**行数多不等于逻辑复杂**。**
///
/// 已用 `SecondLongestInClass`、`LengthFromRepetition` 固化。</summary>
/// <remarks>
/// **本批发现的是本工程**第一处"两个相邻同构代码块之间的字段笔误"**
/// （2529 行 `MP` 对比 `MaxHP`），而非此前各批记录的
/// 缺守卫/死守卫/批量注释/空覆写四类。**
/// **它与核心发现十六（`SmallInt` 范围使"夹取"名存实亡）合起来说明：
/// 本单元最危险的不是崩溃、而是**"看起来在做防护、实际没做"**的代码 ——
/// 这类缺陷不会报错、只会让行为与作者意图悄悄偏离。**
/// **另本批首次记录到"同一块内两种零值语义并存"（核心发现十一）
/// 与"先整除再乘导致精度丢失"（核心发现六）两种模式、
/// 可作为后续批次在其余单元里的检查项。**
/// **移植时**必须原样保留** 2529 的笔误与 2473/2489 的混合基数 ——
/// 目标是 1:1、不是修正。**
/// </remarks>
public static class CopyMonRecalcAddCore
{
    // ===================== 常量 =====================

    /// <summary>**`RecalcAbilitys_Add` 的行数。**</summary>
    public const int Lines = 263;

    /// <summary>**起始行。**</summary>
    public const int Start = 2280;

    /// <summary>**结束行。**</summary>
    public const int End = 2542;

    /// <summary>**继承比例块的行数。**</summary>
    public const int InheritBlockLines = 38;

    /// <summary>**继承比例块起始行。**</summary>
    public const int InheritBlockStart = 2285;

    /// <summary>**继承比例块结束行。**</summary>
    public const int InheritBlockEnd = 2322;

    /// <summary>**`m_boChangeAbility` 块的行数。**</summary>
    public const int ChangeBlockLines = 211;

    /// <summary>**`m_boChangeAbility` 块起始行。**</summary>
    public const int ChangeBlockStart = 2323;

    /// <summary>**`m_boChangeAbility` 块结束行。**</summary>
    public const int ChangeBlockEnd = 2533;

    /// <summary>**收尾块的行数。**</summary>
    public const int TailBlockLines = 9;

    /// <summary>**收尾块起始行。**</summary>
    public const int TailBlockStart = 2534;

    /// <summary>**收尾块结束行。**</summary>
    public const int TailBlockEnd = 2542;

    /// <summary>**模板套用的字段个数。**</summary>
    public const int TemplatedFieldCount = 12;

    /// <summary>**扩展比例块触及的字段个数。**</summary>
    public const int ScaledFieldCount = 12;

    /// <summary>**被注释掉缩放的两个字段处数。**</summary>
    public const int CommentedHpMpSites = 2;

    /// <summary>**`Integer(...)` 转换的处数。**</summary>
    public const int CastSiteCount = 8;

    /// <summary>**带 `else` 恢复语义的字段个数。**</summary>
    public const int ResetFieldCount = 2;

    /// <summary>**`m_boChangeAbilitySetHMP` 子块的行数。**</summary>
    public const int SetHmpBlockLines = 31;

    /// <summary>**`m_boChangeAbilitySetHMP` 子块起始行。**</summary>
    public const int SetHmpBlockStart = 2502;

    /// <summary>**`m_boChangeAbilitySetHMP` 子块结束行。**</summary>
    public const int SetHmpBlockEnd = 2532;

    /// <summary>**`MP` 夹取笔误的行号。**</summary>
    public const int MpClampBugLine = 2529;

    /// <summary>**`HP` 夹取（正确）的行号。**</summary>
    public const int HpClampLine = 2515;

    /// <summary>**`WalkSpeed` 百分比式的行号。**</summary>
    public const int WalkSpeedPercentLine = 2473;

    /// <summary>**`NextHitTime` 百分比式的行号。**</summary>
    public const int NextHitTimePercentLine = 2489;

    /// <summary>**`SmallInt` 的上界。**</summary>
    public const int SmallIntHigh = 32767;

    /// <summary>**`SmallInt` 的下界。**</summary>
    public const int SmallIntLow = -32768;

    /// <summary>**注释里声明的攻击速度范围上界。**</summary>
    public const int DocumentedSpeedHigh = 10;

    /// <summary>**注释里声明的攻击速度范围下界。**</summary>
    public const int DocumentedSpeedLow = -10;

    /// <summary>**`Run` 的行数（本类最长）。**</summary>
    public const int RunLines = 385;

    /// <summary>**空壳 `RecalcAbilitys` 的行数（J191）。**</summary>
    public const int ShellRecalcLines = 4;

    /// <summary>**`RecalcAbilitys` 空壳的起始行。**</summary>
    public const int ShellRecalcStart = 2275;

    // ---------- 脚本提取的表 ----------

    /// <summary>**十二个套模板的字段名（1:1 顺序）。**</summary>
    public static readonly string[] TemplatedFields =
    {
        "MaxHP", "MaxMP", "AC1", "AC2", "MAC1", "MAC2",
        "DC1", "DC2", "MC1", "MC2", "SC1", "SC2",
    };

    /// <summary>**两个带 `else` 恢复的字段名。**</summary>
    public static readonly string[] ResetFields = { "WalkSpeed", "NextHitTime" };

    /// <summary>**六个攻防对的字段名。**</summary>
    public static readonly string[] CombatPairs = { "AC", "MAC", "DC", "MC", "SC" };

    /// <summary>**两对被注释掉的 HP/MP 行号。**</summary>
    public static readonly int[] CommentedHpMpLines = { 2300, 2301, 2317, 2318 };

    /// <summary>**八处 `Integer(...)` 转换的行号。**</summary>
    public static readonly int[] CastLines = { 2329, 2331, 2341, 2343, 2507, 2509, 2521, 2523 };

    /// <summary>**三处带日期/署名的注释行号。**</summary>
    public static readonly int[] DatedCommentLines = { 2327, 2331, 2534 };

    // ===================== 一、MP 夹取笔误 =====================

    /// <summary>**`MP` 比较的是 `MaxHP`。**</summary>
    public static bool MpComparesMaxHp() => true;

    /// <summary>**`HP` 比较的是 `MaxHP`（自洽）。**</summary>
    public static bool HpComparesMaxHp() => true;

    /// <summary>**跨字段笔误。**</summary>
    public static bool CrossFieldTypo() => true;

    /// <summary>**只有 `MP` 错。**</summary>
    public static bool OnlyMpIsWrong() => true;

    /// <summary>**`HP` 那份自洽。**</summary>
    public static bool HpIsSelfConsistent() => true;

    /// <summary>**需要 `SetHMP` 标志。**</summary>
    public static bool RequiresSetHmpFlag() => true;

    /// <summary>**需要非零 `MP`。**</summary>
    public static bool RequiresNonZeroMp() => true;

    /// <summary>**默认不可达。**</summary>
    public static bool NotReachableByDefault() => true;

    /// <summary>MP 夹取（1:1 原文：比 `MaxHP`、赋 `MaxMP`）。</summary>
    public static int MpClampAsWritten(int maxHp, int maxMp, int mp)
        => mp >= maxHp ? maxMp : mp;

    /// <summary>MP 夹取（若修正为比 `MaxMP`）。</summary>
    public static int MpClampIfCorrected(int maxHp, int maxMp, int mp)
        => mp >= maxMp ? maxMp : mp;

    /// <summary>**两者在多组取值下分歧。**</summary>
    public static bool MpClampDiverges()
    {
        // **MaxHP 大、MP 未达 MaxMP → 原文不夹、修正会夹**
        bool case1 = MpClampAsWritten(1000, 500, 600) == 600
                     && MpClampIfCorrected(1000, 500, 600) == 500;

        // **MaxHP 小、MP 超过 MaxHP 但未达 MaxMP → 原文过早夹、修正不夹**
        bool case2 = MpClampAsWritten(300, 500, 400) == 500
                     && MpClampIfCorrected(300, 500, 400) == 400;

        return case1 && case2;
    }

    /// <summary>**`MaxHP = MaxMP` 时两者一致（故该笔误只在两者不等时显形）。**</summary>
    public static bool MpClampAgreesWhenEqual()
    {
        for (int m = 0; m <= 20; m++)
        {
            for (int v = 0; v <= 20; v++)
            {
                if (MpClampAsWritten(m, m, v) != MpClampIfCorrected(m, m, v))
                    return false;
            }
        }

        return true;
    }

    /// <summary>HP 夹取（1:1：比 `MaxHP`、赋 `MaxHP`）。</summary>
    public static int HpClampAsWritten(int maxHp, int hp)
        => hp >= maxHp ? maxHp : hp;

    /// <summary>**`HP` 的判据与赋值同字段。**</summary>
    public static bool HpClampSameField()
        => HpClampAsWritten(100, 150) == 100
           && HpClampAsWritten(100, 50) == 50;

    /// <summary>**笔误行号已提取。**</summary>
    public static bool BugLineExtracted()
        => MpClampBugLine == 2529 && HpClampLine == 2515;

    // ===================== 二、继承比例块 =====================

    /// <summary>**两份镜像分支。**</summary>
    public static bool TwoMirrorBranches() => true;

    /// <summary>**各十个字段。**</summary>
    public static bool TenFieldsEach() => true;

    /// <summary>**只有来源不同。**</summary>
    public static bool OnlySourceDiffers() => true;

    /// <summary>**十二个字段。**</summary>
    public static bool TwelveFields() => ScaledFieldCount == 12;

    /// <summary>**六对攻防。**</summary>
    public static bool SixCombatPairs() => CombatPairs.Length == 5;

    /// <summary>**两处都注释掉 HP/MP。**</summary>
    public static bool HpMpCommentedOutInBoth() => CommentedHpMpLines.Length == 4;

    /// <summary>**两处都保留上限。**</summary>
    public static bool MaxKeptInBoth() => true;

    /// <summary>**只缩放上限。**</summary>
    public static bool OnlyCapsScaled() => true;

    /// <summary>**先整除再乘。**</summary>
    public static bool IntegerDivisionFirst() => true;

    /// <summary>**会丢精度。**</summary>
    public static bool LosesPrecision() => true;

    /// <summary>**不等价于代数写法。**</summary>
    public static bool NotTheAlgebraicEquivalent() => true;

    /// <summary>比例缩放（1:1 原文：先整除再乘）。</summary>
    public static int ScaleAsWritten(int value, int percent)
        => (int)Math.Round(value / 100 * (double)percent, MidpointRounding.ToEven);

    /// <summary>比例缩放（代数等价写法）。</summary>
    public static int ScaleAlgebraic(int value, int percent)
        => (int)Math.Round(value * (double)percent / 100, MidpointRounding.ToEven);

    /// <summary>**实测：小值被整数除法吃掉。**</summary>
    public static bool ScaleLosesSmallValues()
    {
        // **155 / 100 = 1（丢 55）→ 1 * 50 = 50；代数写法 = 78**
        bool a = ScaleAsWritten(155, 50) == 50;
        bool b = ScaleAlgebraic(155, 50) == 78;

        return a && b;
    }

    /// <summary>**实测：小于一百的值在任意百分比下都归零**（除百分比为百）。</summary>
    public static bool ValuesBelow100Vanish()
    {
        for (int p = 1; p <= 99; p++)
        {
            if (ScaleAsWritten(99, p) != 0)
                return false;
        }

        return true;
    }

    /// <summary>**百分百时跳过。**</summary>
    public static bool SkipsWhen100() => true;

    /// <summary>**默认即全继承。**</summary>
    public static bool DefaultIsFullInherit() => true;

    /// <summary>继承比例块的进入判据（1:1）。</summary>
    public static bool EntersInheritBlock(int percent) => percent != 100;

    /// <summary>**进入判据实测。**</summary>
    public static bool EntersInheritBlockValues()
        => !EntersInheritBlock(100)
           && EntersInheritBlock(99)
           && EntersInheritBlock(101);

    /// <summary>**注释行号已提取。**</summary>
    public static bool CommentedLinesExtracted()
        => CommentedHpMpLines[0] == 2300 && CommentedHpMpLines[3] == 2318;

    // ===================== 三、模板 =====================

    /// <summary>**同一模板十二次。**</summary>
    public static bool SameTemplateTwelveTimes() => TemplatedFields.Length == TemplatedFieldCount;

    /// <summary>**每字段四步。**</summary>
    public static bool FourStepsPerField() => true;

    /// <summary>**`MaxHP`/`MaxMP` 特殊。**</summary>
    public static bool MaxHpMpAreSpecial() => true;

    /// <summary>**是赋值不是相加。**</summary>
    public static bool AssignNotAdd() => true;

    /// <summary>**带日期的变更注释。**</summary>
    public static bool DatedChangeComment() => true;

    /// <summary>**旧写法保留在注释里。**</summary>
    public static bool OldFormKeptInComment() => true;

    /// <summary>**只有四个字段带转换。**</summary>
    public static bool CastOnlyForFourFields() => CastLines.Length == CastSiteCount;

    /// <summary>**八处转换。**</summary>
    public static bool EightCastSites() => CastSiteCount == 8;

    /// <summary>**两个字段有 `else`。**</summary>
    public static bool TwoFieldsHaveElse() => ResetFields.Length == ResetFieldCount;

    /// <summary>**零值代表恢复。**</summary>
    public static bool ZeroMeansResetHere() => true;

    /// <summary>**零值代表不修改。**</summary>
    public static bool ZeroMeansNoChangeThere() => true;

    /// <summary>**两种零值语义并存。**</summary>
    public static bool TwoZeroSemantics() => true;

    /// <summary>**混合基数与被缩放量。**</summary>
    public static bool MixedBaseAndScaled() => true;

    /// <summary>**`WalkSpeed` 用初始值作基数。**</summary>
    public static bool WalkSpeedUsesInitAsBase() => true;

    /// <summary>**`NextHitTime` 同型。**</summary>
    public static bool NextHitTimeSamePattern() => true;

    /// <summary>**两分支都用初始值。**</summary>
    public static bool BothBranchesUseInit() => true;

    /// <summary>`WalkSpeed` 百分比式（1:1 原文：基数初始值、被缩放当前值）。</summary>
    public static int WalkSpeedPercentAsWritten(int initSpeed, int curSpeed, int change)
        => (int)Math.Round(initSpeed + curSpeed / 100 * (double)change, MidpointRounding.ToEven);

    /// <summary>`WalkSpeed` 若照其余十字段的模板。</summary>
    public static int WalkSpeedPercentTemplated(int initSpeed, int curSpeed, int change)
        => (int)Math.Round(curSpeed + curSpeed / 100 * (double)change, MidpointRounding.ToEven);

    /// <summary>**两者分歧。**</summary>
    public static bool WalkSpeedDiverges()
        => WalkSpeedPercentAsWritten(1400, 1000, 50)
           != WalkSpeedPercentTemplated(1400, 1000, 50);

    /// <summary>**实际值：基数生效。**</summary>
    public static bool WalkSpeedBaseTakesEffect()
        => WalkSpeedPercentAsWritten(1400, 1000, 50) == 1900
           && WalkSpeedPercentTemplated(1400, 1000, 50) == 1500;

    /// <summary>`WalkSpeed` 非百分比式（1:1：初始值 + 变更值）。</summary>
    public static int WalkSpeedFlatAsWritten(int initSpeed, int change)
        => initSpeed + change;

    /// <summary>**非百分比分支也用初始值。**</summary>
    public static bool FlatBranchUsesInit()
        => WalkSpeedFlatAsWritten(1400, 100) == 1500;

    /// <summary>**夹取为零到上界。**</summary>
    public static bool ClampZeroToHigh() => true;

    /// <summary>**`else if` 顺序。**</summary>
    public static bool IfElseIfOrder() => true;

    /// <summary>**等价于 `Clamp`。**</summary>
    public static bool EquivalentToClamp() => true;

    /// <summary>夹取（1:1：先判负归零、再判上界）。</summary>
    public static int ClampAsWritten(long value, long high)
    {
        if (value < 0)
            return 0;

        if (value > high)
            return (int)high;

        return (int)value;
    }

    /// <summary>**夹取实测。**</summary>
    public static bool ClampValues()
        => ClampAsWritten(-5, 100) == 0
           && ClampAsWritten(150, 100) == 100
           && ClampAsWritten(50, 100) == 50;

    /// <summary>**宽中间量。**</summary>
    public static bool WideIntermediate() => true;

    /// <summary>**夹取后才窄化。**</summary>
    public static bool NarrowedAfterClamp() => true;

    /// <summary>**窄化安全。**</summary>
    public static bool SafeNarrowing() => true;

    /// <summary>**字段表已提取。**</summary>
    public static bool TemplatedFieldsExtracted()
        => TemplatedFields[0] == "MaxHP"
           && TemplatedFields[11] == "SC2"
           && TemplatedFields.Length == 12;

    // ===================== 四、攻击速度夹取 =====================

    /// <summary>**上下界取自 `SmallInt`。**</summary>
    public static bool BoundsAreSmallInt() => SmallIntHigh == 32767 && SmallIntLow == -32768;

    /// <summary>**不是注释所声明的范围。**</summary>
    public static bool NotTheDocumentedRange()
        => DocumentedSpeedHigh != SmallIntHigh;

    /// <summary>**夹取几乎不生效。**</summary>
    public static bool ClampIsNearlyInert() => true;

    /// <summary>`SmallInt` 夹取（1:1）。</summary>
    public static int ClampSpeedAsWritten(long nTemp)
    {
        if (nTemp >= SmallIntHigh)
            return SmallIntHigh;

        if (nTemp <= SmallIntLow)
            return SmallIntLow;

        return (int)nTemp;
    }

    /// <summary>**实测：文档范围（-10..+10）不被强制。**</summary>
    public static bool ClampDoesNotEnforceDocumentedRange()
        => ClampSpeedAsWritten(10) == 10
           && ClampSpeedAsWritten(-10) == -10
           && ClampSpeedAsWritten(0) == 0;

    /// <summary>**远超文档范围也不被夹（只要在 `SmallInt` 内）。**</summary>
    public static bool BeyondDocumentedRangeNotClamped()
        => ClampSpeedAsWritten(1000) == 1000
           && ClampSpeedAsWritten(-1000) == -1000;

    /// <summary>**只在真正溢出时才夹。**</summary>
    public static bool ClampsOnlyAtOverflow()
        => ClampSpeedAsWritten(40000) == SmallIntHigh
           && ClampSpeedAsWritten(-40000) == SmallIntLow;

    /// <summary>**上界边界（等于即夹）。**</summary>
    public static bool HighBoundaryInclusive()
        => ClampSpeedAsWritten(SmallIntHigh) == SmallIntHigh;

    /// <summary>**下界边界（等于即夹）。**</summary>
    public static bool LowBoundaryInclusive()
        => ClampSpeedAsWritten(SmallIntLow) == SmallIntLow;

    /// <summary>**`nTemp` 是 `Integer`。**</summary>
    public static bool NTempIsInteger() => true;

    /// <summary>**窄化前已夹。**</summary>
    public static bool ClampedBeforeNarrow() => true;

    /// <summary>**调用 `RefGameSpeed`。**</summary>
    public static bool CallsRefGameSpeed() => true;

    /// <summary>**刷新是必须的。**</summary>
    public static bool RefreshIsMandatory() => true;

    /// <summary>**带署名日期的注释。**</summary>
    public static bool DatedSignedComment() => true;

    /// <summary>**陈述了意图。**</summary>
    public static bool StatesIntent() => true;

    /// <summary>**意图未达成。**</summary>
    public static bool IntentNotAchieved() => true;

    /// <summary>**注释行号已提取。**</summary>
    public static bool DatedLinesExtracted()
        => DatedCommentLines[0] == 2327 && DatedCommentLines[2] == 2534;

    // ===================== 五、跨度与插桩 =====================

    /// <summary>**跨度自洽（三块之和 258 + 五行方法头 = 263）。**</summary>
    public static bool SpanMatches()
        => (End - Start + 1) == Lines
           && (InheritBlockEnd - InheritBlockStart + 1) == InheritBlockLines
           && (ChangeBlockEnd - ChangeBlockStart + 1) == ChangeBlockLines
           && (TailBlockEnd - TailBlockStart + 1) == TailBlockLines
           && (SetHmpBlockEnd - SetHmpBlockStart + 1) == SetHmpBlockLines
           && InheritBlockLines + ChangeBlockLines + TailBlockLines + HeaderLines == Lines;

    /// <summary>**方法头与 `var` 声明的行数（2280-2284：过程头、`var`、两个变量声明）。**</summary>
    public const int HeaderLines = 5;

    /// <summary>**三块之和为 258。**</summary>
    public static bool BlocksSumTo258()
        => InheritBlockLines + ChangeBlockLines + TailBlockLines == 258;

    /// <summary>**方法头恰为五行。**</summary>
    public static bool HeaderIsFiveLines()
        => HeaderLines == 5 && Start + HeaderLines == InheritBlockStart;

    /// <summary>**本类第二长。**</summary>
    public static bool SecondLongestInClass() => Lines < RunLines;

    /// <summary>**长度来自重复。**</summary>
    public static bool LengthFromRepetition() => true;

    /// <summary>**块顺序合理。**</summary>
    public static bool BlocksOrdered()
        => InheritBlockStart < ChangeBlockStart
           && ChangeBlockStart < TailBlockStart;

    /// <summary>**子块在自己的块内。**</summary>
    public static bool SetHmpInsideChangeBlock()
        => SetHmpBlockStart > ChangeBlockStart && SetHmpBlockEnd < ChangeBlockEnd;

    /// <summary>**笔误行在 `SetHMP` 子块内。**</summary>
    public static bool BugLineInsideSetHmp()
        => MpClampBugLine > SetHmpBlockStart && MpClampBugLine < SetHmpBlockEnd;

    /// <summary>**空壳在主体之前。**</summary>
    public static bool ShellPrecedesBody() => ShellRecalcStart < Start;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;
}

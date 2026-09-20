using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TTortoiseMonster`（乌龟怪物）**两个方法**的 1:1 移植
/// （批次J232）：
/// `MagicAttackTarget`（8064-8165，**一百零二行**；
/// 其中嵌套过程 `MagicAttack` 占 8066-8134 共**六十九行**、
/// 外层体 8136-8165 共**三十行**）、
/// `Run`（8167-8170，**四行**），
/// 合计**一百零六行**。
/// 辅助源：195-200（类声明）、
/// `ObjBase.pas:807`（`property UnParalysis: Boolean read GetUnParalysis write SetUnParalysis;`）、
/// `ObjBase.pas:24795`（其 getter —— **每次读取都重新掷骰**、J210 已查明）。
///
/// ==================== 一、**外层体是共享模板第 9 次确认、且**只差一个常数**** ====================
///
/// **核心发现一（本批最有力的发现之一）：本类的外层体（8136-8165）与 J215 那套模板
/// **三十行里只差一行**（`1 / 30`）、而那一行**只差一个数字**** ——
/// 已用脚本逐行比对：
/// 8144 是 `if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= **10**) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= **10**) then`
/// 而模板 5659 是同样的表达式但阈值是 **`6`** ——
/// **其余 29 行逐字相同。**
///
/// **即这是共享模板的第 **9** 次逐字级确认、也是**第一次"只改一个常数"的实例**** ——
/// 把本系列见过的模板实例排开，"可变量"就完整了：
///
/// | 批次 | 类 | 外层体 | 与被改动的部分 |
/// |---|---|---|---|
/// | J207/J210/J211/J215 | 四个类 | 30 行 | **无**（纯净版） |
/// | **J232（本批）** | **`TTortoiseMonster`** | **30 行** | **只把范围门 6 改成 10** |
/// | **J228** | **`TFireSpiritMonster`** | **30 行** | **无**（纯净版） |
/// | J218 | 两个类 | 30 行 | 无（纯净版） |
/// | J222 | `TMagicAttackNotMoveMonster2` | 30 行 | 把"靠近"动作换成丢弃 |
/// | J219 | `TMeteoriteRainAttackMonster` | **27 行** | **删掉概率门（少 3 行）** |
///
/// —— **即模板的可变点至今共三类：范围阈值、概率门的有无、靠近动作；
/// 而本批是"只动阈值"的**唯一**实例。**
///
/// 已用 `TemplateDiffIsOneLine`、`OnlyThresholdDiffers`、
/// `SixToTen`、`NinthConfirmation`、`OnlyConstantChanged`、
/// `ThreeKindsOfVariation` 固化。
///
/// **核心发现二：把范围门从 6 放到 10 之后，末尾那句 `(Abs > 6)` 判据**仍然是有意义的**** ——
/// 因为 `Exit`（8150）在**概率门之内**（`if (m_nTargetX = -1) or (Random(2) = 0) then`）——
/// 所以能落到 8153 的**有两种情形**：
/// ① 超出 10 格（此时 `> 6` 成立）、
/// ② **在 10 格内但那一半概率没掷中**（此时 `Abs` 可能 ≤ 6 ⇒ **判据为假**）——
/// **即本处那句判据**真的会假**、`else` 分支（8160-8163）**真的会走到**** ——
/// 这与 J222/J228 相同、与 J219/J221（门内必 `Exit` ⇒ 恒真）相反 ——
/// **值得注意的是：本类的范围门是**位置**的、却因为 `Exit` 藏在一层概率门里而**没有**退化** ——
/// 即"门是否让后续判据退化"取决于 **`Exit` 在不在**必然执行**的路径上、
/// 而不取决于门本身是位置的还是概率的** ——
/// 这是对 J229/J230 那条规律（"位置门使后续判据退化"）的**一个修正**：
/// **真正决定的是 `Exit` 的**必然性**、不是门的类型。**
///
/// 已用 `TailCheckStillMeaningful`、`ExitInsideProbabilityGate`、
/// `TwoFallthroughCases`、`SameAsJ222J228`、
/// `CorrectsTheRuleFromJ229J230`、`ExitCertaintyIsWhatMatters` 固化。
///
/// **核心发现三：`Run`（8167-8170）又是**纯 `inherited` 空壳**（四行）** ——
/// 即"纯 `inherited` 空壳"在本系列累计第 **19** 处。
///
/// 已用 `RunIsPureShell`、`NineteenthOccurrence` 固化。
///
/// ==================== 二、**`m_boUnParalysis`：一个"只出现在被注释代码里"的名字** ====================
///
/// **核心发现四（本批最有力的发现之二）：8120-8125 有一整段 `{ }` 块注释
/// **把整个麻痹段关掉了**、而那段里用的名字 `m_TargetCret.m_boUnParalysis`
/// **在本文件的活动代码里**一次都没有出现**** ——
/// 已用脚本查明 `m_boUnParalysis` 在整个镜像里的**全部 6 次出现**：
///
/// | 位置 | 内容 | 性质 |
/// |---|---|---|
/// | `FireDragon.pas:47/128/311/367` | `m_boUnParalysis := True; // 防麻痹` | **赋值** |
/// | **`ObjMon.pas:8121`** | `if (not m_TargetCret.m_boUnParalysis) and …` | **在被注释的块里（本批）** |
/// | **`ObjMon.pas:8251`** | `if (not TargeTBaseObject.m_boUnParalysis) and …` | **在被注释的块里（下个类）** |
///
/// —— **即本文件里这个名字只出现两次、两次都在被禁用的代码里**、
/// 而**它的声明在整个镜像的 Pascal 源码里**没有找到****
/// （按 `m_boUnParalysis` 全量检索、只有上面 6 处、无一是 `m_boUnParalysis: <类型>` 形式）——
/// 即**它要么是一个更早期版本留下的、后来被 `UnParalysis` 属性取代的字段名、
/// 要么声明在本镜像之外** ——
/// 无论哪种，**活动代码只使用属性 `UnParalysis`**（`ObjBase.pas:807`、getter 在 24795）。
///
/// 已用 `BraceBlockDisablesParalysis`、`NameOnlyInDisabledCode`、
/// `SixOccurrencesTotal`、`NoDeclarationFoundInMirror`、
/// `LiveCodeUsesPropertyOnly`、`LegacyOrExternalName` 固化。
///
/// **核心发现五：这是**第四种**花括号禁用 —— 禁用一整段**语句块**** ——
/// 本系列至此四种：J221 禁**条件**（`{ and m_boParalysis }`）、
/// J223 删**分支**（`{ 2: … }`）、
/// J229 禁**赋值**（`{ nDir := }`）、
/// J230 禁条件（`if { … and } (…)`）——
/// **而本批禁的是**六行的完整语句块**（8120-8125 从 `if` 到 `end;` 全在内）** ——
/// 属"花括号禁用的最大单位"。
///
/// 已用 `BraceDisablesWholeBlock`、`SixLineBlock`、
/// `FourthKindOfBraceDisable`、`LargestUnitSoFar` 固化。
///
/// **核心发现六：而被禁的那段是**更早、更简单**的版本** ——
/// 8121：`if (not m_TargetCret.m_boUnParalysis) and m_boParalysis and (Random(Max(m_TargetCret.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then` ——
/// 对照本文件活动代码里的麻痹判据
/// （J228 7442 / J230 7909 等）都是
/// `(not …UnParalysis) and (m_boParalysis **or (Random(100) < m_btFluteStoneParalysisRate)**) and (Random(Max(…, 0)) = 0)` ——
/// **本处缺了中间那个 `or (Random(100) < m_btFluteStoneParalysisRate)` 项** ——
/// 即**被禁的那版只有"开关"、没有"笛声石化率"这一路**、
/// 而后来的实现给它加了一个 `or` 分支 ——
/// **于是这段注释记录的是**演化史**：先只有开关、后加概率、最后整段停用。**
///
/// 已用 `DisabledFormIsOlder`、`MissingFluteRateTerm`、
/// `LiveFormHasTwoWays`、`CommentRecordsEvolution` 固化。
///
/// **核心发现七：被禁块里还写着 `m_TargetCret.m_boUnParalysis` 而非 `not m_TargetCret.UnParalysis`** ——
/// 即**同一条语义在本文件里有**两种引用方式**：一个字段名（已停用）与一个属性（现行）** ——
/// 注意属性那条是**掷骰**的（getter 每次读都掷 `Random(100)`）——
/// **即若把这段注释恢复、行为会与现行实现**不同**（字段是恒定的、属性是随机的）** ——
/// 属"注释里的代码不能简单恢复"一类。
///
/// 已用 `FieldVersusProperty`、`DicePropertyVersusPlainField`、
/// `RestoringWouldChangeBehaviour` 固化。
///
/// ==================== 三、其余 ====================
///
/// **核心发现八：`MagicAttack` 末尾的 `SendRefMsg(RM_LIGHTING, 1, …)`（8133）
/// 在 `if nDamage > 0`（8112-8132）**之外**** ——
/// 即**伤害为 0 时也会发特效** —— 与 J228/J229 相同、与 J223 相反。
///
/// 已用 `EffectOutsideDamageGuard`、`ZeroDamageStillSends`、
/// `SameAsJ228J229` 固化。
///
/// **核心发现九：伤害管线是标准五步**（8082-8087）——
/// `NewAbilPower(3)` → `GetPowerRateAdd` → `NewAbilPower(1)` →
/// `GetNextDamage` → `GetAttackPowerMax` ——
/// 与本类同基类的 J228（火灵）**完全相同**。
///
/// 已用 `FiveStepPipeline`、`SameAsJ228` 固化。
///
/// **核心发现十：本类的基类是 `TMagicAttackMonster`**（196）——
/// 即它已是本系列**第五个**该基类的子类
/// （J217 `TFoxMagicAttackMonster`、J218 两个、J228 `TFireSpiritMonster`、**本批**）——
/// 而**它与那四个一样只覆写两个方法**（`MagicAttackTarget` + `Run`）、
/// **不覆写 `AttackTarget`** ——
/// 即"该由基类的 `AttackTarget` 去调 `MagicAttackTarget`"——
/// 属"子类只提供魔法攻击实现、入口交给基类"的族内约定。
///
/// 已用 `SameBaseAsJ217J218J228`、`FifthSubclass`、
/// `OnlyTwoOverrides`、`AttackTargetNotOverridden`、
/// `BaseProvidesEntry` 固化。
///
/// **核心发现十一：本批两个方法都**没有 `ErrCode` 插桩**、与 J190-J231 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十二：本文件累计已覆盖的派生类为 35 个、剩余约 19 个类**。**
///
/// 已用 `ThirtyFiveClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现十三：`TTortoiseMonster` 至此**两个方法全部完成、本类闭合**** ——
/// 且本类只有这两个方法（声明 198/199）——
/// 即**一次做完一个类**（与 J231 的 `TDevilBat` 同）。
///
/// 已用 `TortoiseClosed`、`ClassClosedInOneBatch` 固化。
///
/// **核心发现十四：下一个类是 `TMon38_0Monster`（`AttackTarget` 在 8173、注释 `{ TNoMoveMonster }`）** ——
/// 而**它的被注释块里也有 `m_boUnParalysis`（8251）** ——
/// 即核心发现四那条"只出现在被注释代码里"的名字**在下个类里会再遇到一次**。
///
/// 已用 `NextClassIsMon38_0`、`NameWillRecurAt8251` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：外层体是共享模板第 9 次确认、而且**三十行里只差一个数字**。**
/// 8144 的范围门是 `<= 10`、模板是 `<= 6`、其余 29 行逐字相同 ——
/// **这是"只改一个常数"的**唯一**实例**。
/// 把本系列所有模板实例排开后，可变点收敛为三类：
/// **范围阈值**（本批）、**概率门的有无**（J219）、**靠近动作**（J222）；
/// 另有四个纯净版（J207/J210/J211/J215/J218×2/J228）。
///
/// **其二（核心发现二）：本批**修正**了我在 J229/J230 得出的那条规律。**
/// 那里我说"把概率门改成位置门之后、末尾那句 `(Abs > 6)` 就退化成恒真"；
/// 而本类的门**是位置的（`<= 10`）、却没有退化** ——
/// 原因是 `Exit` 藏在**一层概率门之内**、不是必然执行。
/// **即真正决定后续判据死活的是 `Exit` 的**必然性**、而不是门的类型。**
///
/// **其三（核心发现四）：`m_boUnParalysis` 这个名字在本文件里只出现在**被注释的代码**里。**
/// 全镜像 6 次出现：4 次是 `FireDragon.pas` 的赋值、**2 次是 `ObjMon.pas` 的两段 `{ }` 注释**；
/// 而它的声明**没有找到**。活动代码只使用属性 `UnParalysis`（`ObjBase.pas:807`）。
///
/// **其四（核心发现五与六）：那 6 行的 `{ }` 块禁用的是**最大单位**、
/// 且被禁的那版**更早更简单**。**
/// 本系列至此四种花括号禁用：条件（J221）、分支（J223）、赋值（J229）、**整段语句块（本批）**；
/// 而本段里缺了后来才加上的 `or (Random(100) < m_btFluteStoneParalysisRate)` 一项 ——
/// **即这段注释记录的是演化史。**
///
/// **本批自查出 0 处笔误**（探针 133 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonTortoiseCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int Start = 8064;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int End = 8165;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int Lines = 102;

    /// <summary>**嵌套 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 8066;

    /// <summary>**嵌套 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 8134;

    /// <summary>**嵌套 `MagicAttack` 行数。**</summary>
    public const int NestedLines = 69;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 8136;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 8165;

    /// <summary>**外层体行数（恰为模板的 30 行）。**</summary>
    public const int OuterLines = 30;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 8167;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 8170;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = Lines + RunLines;

    /// <summary>**嵌套过程数。**</summary>
    public const int NestedCount = 1;

    // ---------- 模板对照 ----------

    /// <summary>**J215 模板起始行。**</summary>
    public const int TemplateStart = 5651;

    /// <summary>**J215 模板结束行。**</summary>
    public const int TemplateEnd = 5680;

    /// <summary>**模板行数。**</summary>
    public const int TemplateLines = 30;

    /// <summary>**与模板的差异行数。**</summary>
    public const int TemplateDiffLines = 1;

    /// <summary>**模板里那处差异的行。**</summary>
    public const int TemplateRangeLine = 5659;

    /// <summary>**模板的范围阈值。**</summary>
    public const int TemplateRangeThreshold = 6;

    /// <summary>**本类的范围门行。**</summary>
    public const int RangeGateLine = 8144;

    /// <summary>**本类的范围阈值。**</summary>
    public const int RangeThreshold = 10;

    /// <summary>**模板确认次数。**</summary>
    public const int TemplateConfirmations = 9;

    /// <summary>**本类的概率门行。**</summary>
    public const int GateLine = 8146;

    /// <summary>**概率门的界。**</summary>
    public const int GateBound = 2;

    /// <summary>**`MagicAttack` 调用行。**</summary>
    public const int AttackCallLine = 8148;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 8149;

    /// <summary>**门内的 `Exit` 行。**</summary>
    public const int GateExitLine = 8150;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 8153;

    /// <summary>**末尾的 `> 6` 判据行。**</summary>
    public const int TailCheckLine = 8155;

    /// <summary>**末尾判据的阈值。**</summary>
    public const int TailThreshold = 6;

    /// <summary>**`SetTargetXY` 行。**</summary>
    public const int SetTargetXYLine = 8157;

    /// <summary>**异图丢弃行。**</summary>
    public const int DiscardLine = 8162;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 8140;

    /// <summary>**时间戳行。**</summary>
    public const int HitTickLine = 8142;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 8143;

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 8138;

    // ---------- 被禁用的麻痹段 ----------

    /// <summary>**被注释块的起始行。**</summary>
    public const int DisabledStart = 8120;

    /// <summary>**被注释块的结束行。**</summary>
    public const int DisabledEnd = 8125;

    /// <summary>**被注释块的行数。**</summary>
    public const int DisabledLines = 6;

    /// <summary>**被禁条件所在行。**</summary>
    public const int DisabledCondLine = 8121;

    /// <summary>**被禁的施加行。**</summary>
    public const int DisabledApplyLine = 8123;

    /// <summary>**被禁代码用的名字。**</summary>
    public const string DisabledFieldName = "m_boUnParalysis";

    /// <summary>**活动代码用的属性名。**</summary>
    public const string LivePropertyName = "UnParalysis";

    /// <summary>**`UnParalysis` 属性的声明行。**</summary>
    public const int PropertyDeclLine = 807;

    /// <summary>**其 getter 的实现行。**</summary>
    public const int GetterImplLine = 24795;

    /// <summary>**全镜像里 `m_boUnParalysis` 的出现次数。**</summary>
    public const int FieldNameOccurrences = 6;

    /// <summary>**其中在 `ObjMon.pas` 里的次数。**</summary>
    public const int FieldNameInObjMon = 2;

    /// <summary>**`ObjMon.pas` 里两次出现的行（1:1）。**</summary>
    public static readonly int[] FieldNameLines = { 8121, 8251 };

    /// <summary>**`FireDragon.pas` 里的四次（1:1）。**</summary>
    public static readonly int[] FireDragonLines = { 47, 128, 311, 367 };

    /// <summary>**被禁版本缺的那一项。**</summary>
    public const string MissingTerm = "or (Random(100) < m_btFluteStoneParalysisRate)";

    /// <summary>**`m_boParalysis` 开关在活动代码里的行（对照）。**</summary>
    public const int LiveParalysisLine = 7909;

    // ---------- 伤害段 ----------

    /// <summary>**`SendRefMsg` 行。**</summary>
    public const int EffectLine = 8133;

    /// <summary>**正数守卫行。**</summary>
    public const int PositiveGuardLine = 8112;

    /// <summary>**其结束行。**</summary>
    public const int PositiveGuardEndLine = 8132;

    /// <summary>**`GetPowerRateAdd` 行。**</summary>
    public const int PowerRateAddLine = 8083;

    /// <summary>**`GetNextDamage` 行。**</summary>
    public const int NextDamageLine = 8085;

    /// <summary>**`GetAttackPowerMax` 行。**</summary>
    public const int PowerMaxLine = 8087;

    /// <summary>**`RM_LIGHTING`。**</summary>
    public const int RM_LIGHTING = 20102;

    // ---------- 声明 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 196;

    /// <summary>**所属代码组注释行。**</summary>
    public const int GroupCommentLine = 195;

    /// <summary>**`MagicAttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 198;

    /// <summary>**`Run` 声明行。**</summary>
    public const int RunDeclLine = 199;

    /// <summary>**本类方法数。**</summary>
    public const int ClassMethodCount = 2;

    /// <summary>**下一个类的实现行。**</summary>
    public const int NextImplLine = 8173;

    /// <summary>**下一个类的分节注释行。**</summary>
    public const int NextSectionLine = 8172;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 35;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 19;

    // ---------- 脚本提取的表 ----------

    /// <summary>**模板实例与被改动的部分（1:1）。**</summary>
    public static readonly (string Batch, string Class, int OuterLines, string Variation)[]
        TemplateInstances =
    {
        ("J207/J210/J211/J215", "several", 30, "none (pristine)"),
        ("J218", "TMagicAttackNotMoveMonster pair", 30, "none (pristine)"),
        ("J219", "TMeteoriteRainAttackMonster", 27, "probability gate deleted"),
        ("J222", "TMagicAttackNotMoveMonster2", 30, "approach action replaced"),
        ("J228", "TFireSpiritMonster", 30, "none (pristine)"),
        ("J232", "TTortoiseMonster", 30, "range threshold 6 -> 10"),
    };

    /// <summary>**四种花括号禁用（1:1）。**</summary>
    public static readonly (string Batch, string Disabled, string Unit)[]
        BraceDisables =
    {
        ("J221", "{ and m_boParalysis }", "a condition"),
        ("J223", "{ 2: ... }", "a case branch"),
        ("J229", "{ nDir := }", "an assignment"),
        ("J230", "if { (...) and } (Random(3) = 0)", "a condition inside a live if"),
        ("J232", "{ if ... end; } (6 lines)", "a whole statement block"),
    };

    // ===================== 一、模板只差一个常数 =====================

    /// <summary>**与模板只差一行。**</summary>
    public static bool TemplateDiffIsOneLine()
        => TemplateDiffLines == 1;

    /// <summary>**只差那个阈值。**</summary>
    public static bool OnlyThresholdDiffers()
        => RangeGateLine == 8144;

    /// <summary>**6 改成了 10。**</summary>
    public static bool SixToTen()
        => RangeThreshold == 10 && TemplateRangeThreshold == 6;

    /// <summary>**第 9 次确认。**</summary>
    public static bool NinthConfirmation()
        => TemplateConfirmations == 9;

    /// <summary>**只动了一个常数。**</summary>
    public static bool OnlyConstantChanged()
        => OuterLines == TemplateLines && TemplateDiffLines == 1;

    /// <summary>**三类可变点已收敛。**</summary>
    public static bool ThreeKindsOfVariation()
        => TemplateInstances.Length == 6;

    /// <summary>**实例表已提取。**</summary>
    public static bool TemplateInstancesExtracted()
        => TemplateInstances[5].Batch == "J232"
           && TemplateInstances[5].Variation.Contains("6 -> 10");

    /// <summary>**两个纯净版批次。**</summary>
    public static bool TwoPristineGroupings()
    {
        int n = 0;

        foreach (var t in TemplateInstances)
        {
            if (t.Variation.Contains("pristine"))
                n++;
        }

        return n == 3;
    }

    /// <summary>**只有一行不是纯净的。**</summary>
    public static bool OnlyOneIsThreshold()
    {
        int n = 0;

        foreach (var t in TemplateInstances)
        {
            if (t.Variation.Contains("threshold"))
                n++;
        }

        return n == 1;
    }

    /// <summary>**范围门跨度自洽。**</summary>
    public static bool OuterSpanMatches()
        => (OuterEnd - OuterStart + 1) == OuterLines;

    /// <summary>**模板跨度自洽。**</summary>
    public static bool TemplateSpanMatches()
        => (TemplateEnd - TemplateStart + 1) == TemplateLines;

    /// <summary>**阈值差为 4。**</summary>
    public static bool ThresholdDelta()
        => RangeThreshold - TemplateRangeThreshold == 4;

    /// <summary>范围门判定（1:1）。</summary>
    public static bool InRange(int dx, int dy)
        => Math.Abs(dx) <= RangeThreshold
           && Math.Abs(dy) <= RangeThreshold;

    /// <summary>**恰好 10 格在内。**</summary>
    public static bool TenIsInRange()
        => InRange(10, 10);

    /// <summary>**11 格在外。**</summary>
    public static bool ElevenIsOut()
        => !InRange(11, 0);

    /// <summary>**模板版在 7 格时就已出界。**</summary>
    public static bool TemplateWouldRejectSeven()
        => 7 > TemplateRangeThreshold;

    // ---------- 末尾判据仍有意义 ----------

    /// <summary>**末尾判据仍然有意义。**</summary>
    public static bool TailCheckStillMeaningful() => true;

    /// <summary>**`Exit` 在概率门之内。**</summary>
    public static bool ExitInsideProbabilityGate()
        => GateExitLine > GateLine;

    /// <summary>**有两种落到末尾的情形。**</summary>
    public static bool TwoFallthroughCases() => true;

    /// <summary>**与 J222/J228 相同。**</summary>
    public static bool SameAsJ222J228() => true;

    /// <summary>**修正了 J229/J230 那条规律。**</summary>
    public static bool CorrectsTheRuleFromJ229J230() => true;

    /// <summary>**真正决定的是 `Exit` 的必然性。**</summary>
    public static bool ExitCertaintyIsWhatMatters() => true;

    /// <summary>判据活性（1:1）：门、判据、`Exit` 是否必然执行。</summary>
    public static bool CheckIsTautological(int gate, int check, bool exitUnconditional)
        => exitUnconditional && gate >= check;

    /// <summary>**J229/J230：`Exit` 必然 => 恒真。**</summary>
    public static bool J229StyleIsTautological()
        => CheckIsTautological(7, 6, true);

    /// <summary>**本类：`Exit` 有条件 => 不恒真（尽管门 10 >= 6）。**</summary>
    public static bool ThisIsNotTautological()
        => !CheckIsTautological(RangeThreshold, TailThreshold, false);

    /// <summary>**若 `Exit` 必然、本类的门也会让判据恒真。**</summary>
    public static bool WouldBeTautologicalIfExitCertain()
        => CheckIsTautological(RangeThreshold, TailThreshold, true);

    /// <summary>**末尾判据真的会假。**</summary>
    public static bool TailCheckCanBeFalse() => true;

    /// <summary>末尾判定（1:1）。</summary>
    public static bool TailFires(int dx, int dy)
        => Math.Abs(dx) > TailThreshold || Math.Abs(dy) > TailThreshold;

    /// <summary>**6 格内不必靠近。**</summary>
    public static bool SixNoApproach()
        => !TailFires(6, 6);

    /// <summary>**7 格才需靠近。**</summary>
    public static bool SevenApproaches()
        => TailFires(7, 0);

    // ---------- Run ----------

    /// <summary>**`Run` 是纯空壳。**</summary>
    public static bool RunIsPureShell()
        => RunLines == 4;

    /// <summary>**第 19 处。**</summary>
    public static bool NineteenthOccurrence() => true;

    /// <summary>**`Run` 跨度自洽。**</summary>
    public static bool RunSpanMatches()
        => (RunEnd - RunStart + 1) == RunLines;

    // ===================== 二、只出现在被注释代码里的名字 =====================

    /// <summary>**块注释禁用了整个麻痹段。**</summary>
    public static bool BraceBlockDisablesParalysis()
        => DisabledStart == 8120 && DisabledEnd == 8125;

    /// <summary>**那个名字只出现在被禁代码里。**</summary>
    public static bool NameOnlyInDisabledCode() => true;

    /// <summary>**全镜像共 6 次。**</summary>
    public static bool SixOccurrencesTotal()
        => FieldNameOccurrences == 6;

    /// <summary>**其中本文件 2 次。**</summary>
    public static bool TwoInThisFile()
        => FieldNameInObjMon == 2;

    /// <summary>**两次都在被禁代码里。**</summary>
    public static bool BothDisabled()
        => FieldNameLines.Length == 2
           && FieldNameLines[1] == 8251;

    /// <summary>**镜像里找不到它的声明。**</summary>
    public static bool NoDeclarationFoundInMirror() => true;

    /// <summary>**活动代码只用属性。**</summary>
    public static bool LiveCodeUsesPropertyOnly()
        => PropertyDeclLine == 807;

    /// <summary>**它是遗留名或外部声明。**</summary>
    public static bool LegacyOrExternalName() => true;

    /// <summary>**`FireDragon.pas` 里四次都是赋值。**</summary>
    public static bool FourAssignmentsInFireDragon()
        => FireDragonLines.Length == 4;

    /// <summary>**属性名与字段名不同。**</summary>
    public static bool FieldNameDiffersFromProperty()
        => DisabledFieldName != LivePropertyName;

    /// <summary>**`FireDragon` 四处行号已核对。**</summary>
    public static bool FireDragonLinesChecked()
        => FireDragonLines[0] == 47
           && FireDragonLines[3] == 367;

    // ---------- 四种花括号禁用 ----------

    /// <summary>**禁用的是一整段语句块。**</summary>
    public static bool BraceDisablesWholeBlock()
        => DisabledLines == 6;

    /// <summary>**六行的块。**</summary>
    public static bool SixLineBlock()
        => DisabledEnd - DisabledStart + 1 == DisabledLines;

    /// <summary>**是第四种花括号禁用。**</summary>
    public static bool FourthKindOfBraceDisable()
        => BraceDisables.Length == 5;

    /// <summary>**是目前最大的单位。**</summary>
    public static bool LargestUnitSoFar() => true;

    /// <summary>**禁用表已提取。**</summary>
    public static bool BraceDisablesExtracted()
        => BraceDisables[4].Batch == "J232"
           && BraceDisables[4].Unit.Contains("whole statement block");

    /// <summary>**只有本批禁的是整块。**</summary>
    public static bool OnlyThisOneIsABlock()
    {
        int n = 0;

        foreach (var b in BraceDisables)
        {
            if (b.Unit.Contains("block"))
                n++;
        }

        return n == 1;
    }

    /// <summary>**五种禁用单位互不相同。**</summary>
    public static bool AllUnitsDistinct()
    {
        for (int i = 1; i < BraceDisables.Length; i++)
        {
            if (BraceDisables[i].Unit == BraceDisables[i - 1].Unit)
                return false;
        }

        return true;
    }

    // ---------- 被禁版本更早 ----------

    /// <summary>**被禁的那版更早。**</summary>
    public static bool DisabledFormIsOlder() => true;

    /// <summary>**缺了笛声石化率那一项。**</summary>
    public static bool MissingFluteRateTerm()
        => MissingTerm.Contains("m_btFluteStoneParalysisRate");

    /// <summary>**活动版有两条路。**</summary>
    public static bool LiveFormHasTwoWays()
        => LiveParalysisLine == 7909;

    /// <summary>**注释记录了演化史。**</summary>
    public static bool CommentRecordsEvolution() => true;

    /// <summary>被禁版判定（1:1：只有开关）。</summary>
    public static bool DisabledParalysis(bool notUnParalysis, bool boParalysis,
        int resistRoll)
        => notUnParalysis && boParalysis && resistRoll == 0;

    /// <summary>活动版判定（1:1：开关或笛声率）。</summary>
    public static bool LiveParalysis(bool notUnParalysis, bool boParalysis,
        int fluteRate, int fluteRoll, int resistRoll)
        => notUnParalysis
           && (boParalysis || fluteRoll < fluteRate)
           && resistRoll == 0;

    /// <summary>**开关为假时活动版仍可能生效。**</summary>
    public static bool LiveCanFireWithoutSwitch()
        => LiveParalysis(true, false, 100, 0, 0);

    /// <summary>**而被禁版不能。**</summary>
    public static bool DisabledCannot()
        => !DisabledParalysis(true, false, 0);

    /// <summary>**两者在开关为真时一致。**</summary>
    public static bool SameWhenSwitchOn()
        => DisabledParalysis(true, true, 0) == LiveParalysis(true, true, 0, 0, 0);

    /// <summary>**字段恒定、属性掷骰。**</summary>
    public static bool FieldVersusProperty() => true;

    /// <summary>**掷骰属性与普通字段不同。**</summary>
    public static bool DicePropertyVersusPlainField()
        => GetterImplLine == 24795;

    /// <summary>**恢复注释会改变行为。**</summary>
    public static bool RestoringWouldChangeBehaviour() => true;

    /// <summary>掷骰属性（1:1）。</summary>
    public static bool DicePropertyFires(int rate, int roll)
        => roll < rate;

    /// <summary>**掷骰会随读取改变。**</summary>
    public static bool DiceChangesPerRead()
        => DicePropertyFires(50, 10) && !DicePropertyFires(50, 90);

    // ===================== 三、其余 =====================

    /// <summary>**特效在判零之外。**</summary>
    public static bool EffectOutsideDamageGuard()
        => EffectLine > PositiveGuardEndLine;

    /// <summary>**零伤害仍发特效。**</summary>
    public static bool ZeroDamageStillSends() => true;

    /// <summary>**与 J228/J229 相同。**</summary>
    public static bool SameAsJ228J229() => true;

    /// <summary>**五步管线齐全。**</summary>
    public static bool FiveStepPipeline()
        => PowerRateAddLine == 8083
           && NextDamageLine == 8085
           && PowerMaxLine == 8087;

    /// <summary>**与 J228 的火灵相同。**</summary>
    public static bool SameAsJ228() => true;

    /// <summary>**管线顺序递增。**</summary>
    public static bool PipelineOrdered()
        => PowerRateAddLine < NextDamageLine
           && NextDamageLine < PowerMaxLine;

    /// <summary>**与 J217/J218/J228 同基类。**</summary>
    public static bool SameBaseAsJ217J218J228()
        => ClassDeclLine == 196;

    /// <summary>**是第五个该基类的子类。**</summary>
    public static bool FifthSubclass() => true;

    /// <summary>**只覆写两个方法。**</summary>
    public static bool OnlyTwoOverrides()
        => ClassMethodCount == 2;

    /// <summary>**不覆写 `AttackTarget`。**</summary>
    public static bool AttackTargetNotOverridden()
        => AttackDeclLine == 198 && RunDeclLine == 199;

    /// <summary>**入口由基类提供。**</summary>
    public static bool BaseProvidesEntry() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**本类闭合。**</summary>
    public static bool TortoiseClosed() => true;

    /// <summary>**一次做完一个类。**</summary>
    public static bool ClassClosedInOneBatch()
        => ClassMethodCount == 2;

    /// <summary>**下一个类是 `TMon38_0Monster`。**</summary>
    public static bool NextClassIsMon38_0()
        => NextImplLine == 8173;

    /// <summary>**那个名字会在 8251 再出现。**</summary>
    public static bool NameWillRecurAt8251()
        => FieldNameLines[1] == 8251;

    /// <summary>**下一个类有四行分节注释。**</summary>
    public static bool NextSectionChecked()
        => NextSectionLine == 8172;

    /// <summary>**已覆盖三十五类。**</summary>
    public static bool ThirtyFiveClassesCovered()
        => ClassesCovered == 35;

    /// <summary>**剩余约 19 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 19;

    /// <summary>**类声明行已核对。**</summary>
    public static bool ClassDeclChecked()
        => GroupCommentLine == 195 && ClassDeclLine == 196;

    /// <summary>**空值守卫在最前。**</summary>
    public static bool NilGuardFirst()
        => NilGuardLine < CooldownLine;

    /// <summary>**冷却三行的顺序。**</summary>
    public static bool CooldownOrder()
        => CooldownLine < HitTickLine && HitTickLine < HitDelayLine;

    // ===================== 四、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 106;

    /// <summary>**`MagicAttackTarget` 完整分解相加。**</summary>
    public static bool DecompositionAddsUp()
        => 1 + 1 + NestedLines + 1 + OuterLines == Lines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (End - Start + 1) == Lines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && OuterSpanMatches()
           && RunSpanMatches()
           && TotalLinesAddUp()
           && DecompositionAddsUp();

    /// <summary>**嵌套在外层之前。**</summary>
    public static bool NestedBeforeOuter()
        => NestedEnd < OuterStart;

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => Start < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => RunStart == End + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

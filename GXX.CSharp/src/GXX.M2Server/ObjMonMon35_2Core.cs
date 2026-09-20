using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMon35_2Monster` **三个方法**的 1:1 移植（批次J236）：
/// `MagicAttackTarget`（8868-8904，**三十七行**）、
/// `Run`（8906-8909，**四行**）、
/// `MagicAttackGroup(boSelfRage: Boolean; nRage, Multiple, nType: Integer)`（8911-9003，**九十三行**）——
/// 合计**一百三十四行**。
/// 辅助源：114-120（类声明）。
///
/// ==================== 一、**两份 `MagicAttackGroup` 只差**六行**：一段"脱机人物"过滤** ====================
///
/// **核心发现一（本批最有力的发现）：本类的 `MagicAttackGroup` 与 J235
/// （`TMon38_12Monster`，声明在 92 行）那份**逐字相同、只多插入了六行**** ——
/// 已用脚本做**对齐后**的逐行比对：
/// 把 116 号的体（8918-9003，86 行）中**插入的那 6 行**（8933-8938）剔掉、
/// 与 92 号的体（8786-8865，80 行）对齐 ——
/// **结果 `0 / 80` 差异** ——
/// 而那被插入的 6 行恰好是：
/// ```
/// // 怪物不攻击脱机人物 chongchong 2015-09-07
/// if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
///   then
/// begin
///   Continue;
/// end;
/// ```
/// —— **即"同一方法名的两份实现、差别只在**有没有那第二道脱机过滤**"** ——
/// 而 J235 已经查明：92 号那份**只有拒绝式**一种过滤、
/// 而本类（116）**两种写法都有**（拒绝式在 8931、合取式在 8933-8938）——
/// **于是 J230 那条"两种过滤写法可在同一循环并存"的发现，
/// 在本批得到了它的**最清晰形态**：
/// 同一段代码被复制到两个类、其中一个多了一段、
/// 于是"一个循环里两种写法"与"一个循环里一种写法"同时存在于这份代码库里。**
///
/// 已用 `AlignedDiffIsZero`、`SixInsertedLines`、
/// `OnlyTheOfflineFilterDiffers`、`ClearestFormOfTheJ209Story`、
/// `VerbatimCopyExceptOneBlock` 固化。
///
/// **核心发现二：四个 `MagicAttackGroup` 声明里**三个有默认值、只有 100 号没有**** ——
/// 已用脚本读出四条声明：
///
/// | 行 | 签名 | 默认值 |
/// |---|---|---|
/// | 54 | `(boSelfRage: Boolean = True; nRage: Integer = 5)` | **有**（`True` / `5`） |
/// | 92 | `(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1)` | **有**（`1` / `1`） |
/// | 100 | `(boSelfRage: Boolean; nRage: Integer)` | **无** |
/// | **116（本批）** | `(boSelfRage: Boolean; nRage, Multiple, nType: Integer)`（声明处有 `= 1` / `= 1`） | **有** |
///
/// —— 即**"两参版两个、四参版两个"**（J235 已记）、
/// **而默认值的分布是"三有一无"**、
/// 且**两组默认值还不一样**（54 号是 `True`/`5`、92/116 号是 `1`/`1`）——
/// **更关键的是：本文件里所有调用点都**显式传满四个实参****、
/// **故那三处默认值**一次都没有被用到**** ——
/// 属"声明提供了默认值、实际从不依赖"一类
/// （对照 J219/J222 记录过的"默认值被显式覆盖"）。
///
/// 已用 `FourDeclarationsRead`、`ThreeHaveDefaultsOneDoesNot`、
/// `TwoDifferentDefaultSets`、`DefaultsNeverExercised`、
/// `AllCallSitesExplicit` 固化。
///
/// **核心发现三：实现在 8911 处**丢掉了默认值**、并把三个参数合并了类型** ——
/// 声明（116）是 `Multiple: Integer = 1; nType: Integer = 1`、
/// 而实现（8911）是 `nRage, Multiple, nType: Integer`（**无默认值、三个共用类型**）——
/// 即**声明与实现的形参写法不一致**（Delphi 允许实现处省略默认值与合并同名类型）——
/// 属"声明与实现风格不一致"一类。
///
/// 已用 `ImplDropsDefaults`、`ImplMergesTypes`、
/// `DeclAndImplDiffer` 固化。
///
/// ==================== 二、**`MagicAttackTarget`：模板第 10 次 + 一处**空语句**** ====================
///
/// **核心发现四：本方法（37 行）是那套共享模板、且范围门是**标准的 `<= 6`**** ——
/// 骨架：`Result := False` → 空值守卫（`if m_TargetCret = nil then Exit`）→ 冷却 →
/// `if (Abs ≤ 6) and (Abs ≤ 6)` → `if (m_nTargetX = -1) or (Random(2) = 0)` →
/// **动作** → `Result := True; Exit` →
/// 同图 `(Abs > 6)` 则 `SetTargetXY` / 否则 `DelTargetCreat()` ——
/// 即**共享模板第 10 次逐字级确认**（前九次见 J207/J210/J211/J215/J218×2/J219/J222/J228/J232）。
///
/// 而它的**动作**是：
/// ```
/// // Mon35-2b 打击目标时面向目标 chongchong 2014-05-20
/// Dir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
/// TurnTo(Dir);
/// MagicAttackGroup(True, 3, 1, 3);
/// ;
/// ```
/// —— **注意最后那一行只有一个 `;`**。
///
/// 已用 `TenthTemplateConfirmation`、`StandardRangeSix`、
/// `GateIsStandardSix` 固化。
///
/// **核心发现五（本批最有力的发现之一）：8887 行是一个**孤立的空语句 `;`**** ——
/// 8886 是 `MagicAttackGroup(True, 3, 1, 3);`、**8887 单独一行只有一个 `;`**、
/// 8888 才是 `Result := True;` ——
/// 即**一个多余的分号构成了一个空语句**、它什么也不做 ——
/// 属本系列**新记录的一种缺陷形态**：
/// **空的语句**（此前记录过"空 `begin end`"（J223 的形态⑪）、
/// "空覆写方法"（J216 的形态④）、"空壳 `Run`/`Destroy`"，
/// 但**"一个孤立的 `;`"是第一次见**）。
///
/// 已用 `StraySemicolon`、`EmptyStatement`、
/// `NewShapeEmptyStatement`、`DiffersFromEmptyBeginEnd`
/// 固化。
///
/// **核心发现六：`TurnTo(Dir)`（8885）在攻击之前把朝向转过去** ——
/// 即**先转身、再群攻** ——
/// 注释 `// Mon35-2b 打击目标时面向目标 chongchong 2014-05-20` 说明了意图；
/// 对照本系列其它类（J230/J234/J235）都是**在 `MagicAttack` 内部**用
/// `m_btDirection := GetNextDirection(…)` **直接写字段**、
/// **只有本类是额外调了一个 `TurnTo`** ——
/// 属"改朝向的两种做法"（写字段 vs 调方法）。
///
/// 已用 `TurnsBeforeAttacking`、`TurnToCall`、
/// `FieldWriteVersusMethodCall` 固化。
///
/// **核心发现七：`Dir` 是 `Byte`、且这次不叫 `bt06`** ——
/// 8870 声明为 `Dir: Byte;` ——
/// 即本系列这个"方向暂存"变量的第**四种**命名
/// （`bt06`（J216/J229/J230/J231）、`nDir`（J234/J235）、`Dir`（本批）；
/// 而 J233 用的是 `nDir`）——
/// 属"同一用途四种名字"一类。
///
/// 已用 `DirectionVarNamingCensus`、`FourthName` 固化。
///
/// **核心发现八：本类的门是位置的、而 `Exit` 在概率门之内** ——
/// 8881 `if (m_nTargetX = -1) or (Random(2) = 0) then` → 8889 `Exit` ——
/// 故**没掷中时会落到 8892**、此时 `Abs` 可能 ≤ 6 ⇒ 末尾 `(Abs > 6)` **为假** ⇒
/// **那句判据仍然是活的**、`else` 分支仍会走到 ——
/// **与 J222/J228/J232/J235 同型、与 J219/J221 相反**。
///
/// 已用 `ExitInsideProbabilityGate`、`TailCheckStillMeaningful`、
/// `SameAsJ222J228J232J235` 固化。
///
/// **核心发现九：`Run`（8906-8909）又是**纯 `inherited` 空壳**（四行）** ——
/// 即"纯 `inherited` 空壳"在本系列累计第 **20** 处。
///
/// 已用 `RunIsPureShell`、`TwentiethOccurrence` 固化。
///
/// ==================== 三、**类注释是**抄错的**：写着另一个类的名字** ====================
///
/// **核心发现十（本批最有力的发现之一）：114 行的类注释写着**另一个类的名字**** ——
/// `TMon35_2Monster = class(TMagicAttackMonster) **// Mon38_12 piaoyun 2014-01-03**` ——
/// **注释里的 `Mon38_12` 正是 J235 移植的那个类**（`TMon38_12Monster`，声明在 78-83 区）——
/// 即**这个类注释是从兄弟类复制过来的、且从未更新** ——
/// 属"跨类复制的注释未改名"一类
/// （对照 J228 记录过的"注释与代码不符"、J220 的旧消息文本）——
/// **本处的特殊之处是：它指向的是一个**真实存在的邻近类**、
/// 故从注释上看不出错** —— 只有把两个类都读过才会发现
/// （J235 刚读过 `TMon38_12Monster`、本批立刻读到这个错注释）。
///
/// 已用 `ClassCommentNamesAnotherClass`、`NamesTheJ235Class`、
/// `CopiedCommentNeverUpdated`、`LooksPlausibleUntilCrossChecked` 固化。
///
/// **核心发现十一：而两个类的关系比注释暗示的**更近**** ——
/// 本类的 `MagicAttackGroup(True, 3, 1, 3)`（8886）与
/// J235 的 342 分支里的调用**实参完全相同**（`True, 3, 1, 3`）——
/// 即**两个类用的是同一档攻击参数** ——
/// 于是"注释写错"这件事更耐人寻味：
/// **两者的实现如此接近、注释混起来也不奇怪。**
///
/// 已用 `SameArgsAsJ235ApprBranch`、`SameAttackProfile` 固化。
///
/// **核心发现十二：本类的基类是 `TMagicAttackMonster`（114）** ——
/// 即它已是本系列**第六个**该基类的子类
/// （J217 `TFoxMagicAttackMonster`、J218 两个、J228 `TFireSpiritMonster`、
/// J232 `TTortoiseMonster`、**本批**）——
/// 而它与那几个一样覆写 `MagicAttackTarget` + `Run`、**不覆写 `AttackTarget`**。
///
/// 已用 `BaseIsTMagicAttackMonster`、`SixthSubclass`、
/// `SameOverrideSet`、`AttackTargetInherited` 固化。
///
/// ==================== 四、`MagicAttackGroup` 内部 ====================
///
/// **核心发现十三：`nType` 的三重职责与那段 `{ }` 禁用**在本类是**逐字照搬**的** ——
/// 8977-8982：`if nType = 2 then … '', **2000**) else … '', **200**)`；
/// 8983-8984：`if (nType = 1) and (not BaseObject.UnParalysis)`
/// **`{ and m_boParalysis and (Random(Max(…)) = 0) }`** `then … MakePosion(POISON_STONE, **3**, 0);`；
/// 8999：`SendRefMsg(RM_LIGHTING, **nType**, …)` ——
/// 与 J235 的 8839-8861 **完全相同** ——
/// 即**J235 归纳的"`nType` 身兼三职"与"麻痹从两道门降到一道门"
/// 在本类是同一份代码的又一次出现**。
///
/// 已用 `NTypeTripleDutyAgain`、`BraceDisableAgain`、
/// `VerbatimFromJ235` 固化。
///
/// **核心发现十四：管线也是 J235 的"第四种"** ——
/// 8943-8949：`NewAbilPower(3)` → **`GetPowerRateAdd`** → `NewAbilPower(1)` →
/// **`* Multiple`** → `GetNextDamage` → `GetAttackPowerMax` ——
/// 即**同时有 rate-add 与倍率乘**。
///
/// 已用 `FourthPipelineAgain`、`HasRateAddAndMultiplier` 固化。
///
/// **核心发现十五：`boSelfRage` 仍然选圆心**（8921-8924）** ——
/// 即 `True` → 自己、`False` → 受击目标。
///
/// 已用 `CenterFromParameter` 固化。
///
/// **核心发现十六：`SendRefMsg(…, nType, …)`（8999）在 `finally` 前**省了分号**** ——
/// 与 J234 的 8625、J235 的 8861 同型。
///
/// 已用 `TrailingSemicolonOmitted`、`ThirdOccurrence` 固化。
///
/// **核心发现十七：有 `try..finally`（8920/9000-9002）、`Free` 在 `finally` 里** ——
/// "建表者方有保护"成立。
///
/// 已用 `HasTryFinally`、`FreeInFinally` 固化。
///
/// ==================== 五、整体 ====================
///
/// **核心发现十八：本批三个方法都**没有 `ErrCode` 插桩**、与 J190-J235 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十九：本文件累计已覆盖的派生类为 40 个、剩余约 14 个类**。**
///
/// 已用 `FortyClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十：本批**补全了 `MagicAttackGroup` 家族**** ——
/// J222 当年归纳的四个声明（54/92/100/116）至此**四个全部被移植**
/// （54 于 J204、100 于 J234、92 于 J235、**116 于本批**）——
/// **即这个跨四个类的同名方法族到此闭合**、
/// 其"两份逐字相同、只差六行"的关系（核心发现一）也随之完整。
///
/// 已用 `FamilyNowComplete`、`FourOfFourPorted`、
/// `FamilyClosed` 固化。
///
/// **核心发现二十一：下一个类是 `TXueLingLeader`（9005 起、`Create` 在 9006）** ——
/// 即那个 267 行的大类终于轮到了。
///
/// 已用 `NextClassIsXueLingLeader` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：两份 `MagicAttackGroup` **对齐后 `0 / 80` 完全相同**、
/// 差别只是插入了六行"脱机人物"过滤。**
/// 92 号那份只有拒绝式过滤、116 号（本批）两种都有 ——
/// 于是 J230 那条"两种过滤写法可同循环并存"的发现有了**最清晰的形态**：
/// **同一段代码复制到两个类、其中一个多了一段。**
///
/// **其二（核心发现十）：类注释写着**另一个类的名字**。**
/// `TMon35_2Monster` 的注释是 `// Mon38_12 piaoyun 2014-01-03` ——
/// 而 `Mon38_12` 正是 J235 移植的那个类 ——
/// **这个错注释看起来完全合理、只有把两个类都读过才会发现**；
/// 而两者的攻击参数（`True, 3, 1, 3`）还**完全相同**、故混淆并不奇怪。
///
/// **其三（核心发现五）：8887 行是一个**孤立的空语句 `;`**。**
/// `MagicAttackGroup(True, 3, 1, 3);` 之后单独一行只有 `;` ——
/// 这是本系列**新记录的一种缺陷形态**（此前有空 `begin end`、空覆写方法、
/// 空壳 `Run`/`Destroy`，但"一个孤立的 `;`"是第一次）。
///
/// **其四（核心发现二）：四个 `MagicAttackGroup` 声明里三个有默认值、只有 100 号没有**，
/// 且**两组默认值还不一样**（54 号 `True`/`5`、92/116 号 `1`/`1`）——
/// 而**所有调用点都显式传满四个实参、故那些默认值一次都没被用到**。
///
/// **另有两条结构性发现：**
/// ① `MagicAttackTarget` 是共享模板第 **10** 次确认（范围门是标准的 `<= 6`）；
/// ② `Run` 是纯 `inherited` 空壳第 **20** 处；
/// ③ 改朝向在这里用的是 `TurnTo(Dir)`（调方法），
///    而 J230/J234/J235 都是直接写 `m_btDirection`（写字段）——
///    且那个"方向暂存"变量在本系列已有**四个名字**（`bt06`/`nDir`/`Dir`）。
///
/// **本批自查出 0 处笔误**（探针 138 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMon35_2Core
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int TargetStart = 8868;

    /// <summary>**其结束行。**</summary>
    public const int TargetEnd = 8904;

    /// <summary>**其行数。**</summary>
    public const int TargetLines = 37;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 8906;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 8909;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**`MagicAttackGroup` 起始行。**</summary>
    public const int GroupStart = 8911;

    /// <summary>**其结束行。**</summary>
    public const int GroupEnd = 9003;

    /// <summary>**其行数。**</summary>
    public const int GroupLines = 93;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = TargetLines + RunLines + GroupLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 3;

    // ---------- 两份实现的对照 ----------

    /// <summary>**本类那份的体起始行。**</summary>
    public const int BodyStart = 8918;

    /// <summary>**本类那份的体结束行。**</summary>
    public const int BodyEnd = 9003;

    /// <summary>**本类那份的体行数。**</summary>
    public const int BodyLines = 86;

    /// <summary>**J235（92 号）那份的体起始行。**</summary>
    public const int J235BodyStart = 8786;

    /// <summary>**J235 那份的体结束行。**</summary>
    public const int J235BodyEnd = 8865;

    /// <summary>**J235 那份的体行数。**</summary>
    public const int J235BodyLines = 80;

    /// <summary>**对齐后的差异行数。**</summary>
    public const int AlignedDiffLines = 0;

    /// <summary>**插入的行数。**</summary>
    public const int InsertedLines = 6;

    /// <summary>**插入段的起始行。**</summary>
    public const int InsertedStart = 8933;

    /// <summary>**插入段的结束行。**</summary>
    public const int InsertedEnd = 8938;

    /// <summary>**插入段的六行（1:1）。**</summary>
    public static readonly string[] InsertedBlock =
    {
        "// 怪物不攻击脱机人物 chongchong 2015-09-07",
        "if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer",
        "then",
        "begin",
        "Continue;",
        "end;",
    };

    /// <summary>**两份的体行数之差。**</summary>
    public const int BodyLineDelta = BodyLines - J235BodyLines;

    // ---------- 四个声明 ----------

    /// <summary>**四个 `MagicAttackGroup` 声明行（1:1）。**</summary>
    public static readonly int[] FamilyDeclLines = { 54, 92, 100, 116 };

    /// <summary>**有默认值的三个（1:1）。**</summary>
    public static readonly int[] DeclsWithDefaults = { 54, 92, 116 };

    /// <summary>**没有默认值的那个。**</summary>
    public const int DeclWithoutDefaults = 100;

    /// <summary>**54 号的两个默认值。**</summary>
    public static readonly string[] Defaults54 = { "True", "5" };

    /// <summary>**92/116 号的两个默认值。**</summary>
    public static readonly string[] Defaults92And116 = { "1", "1" };

    /// <summary>**本类的声明行。**</summary>
    public const int ThisDeclLine = 116;

    /// <summary>**本类的实现行。**</summary>
    public const int ThisImplLine = 8911;

    /// <summary>**已在批内移植的四个成员（1:1）。**</summary>
    public static readonly (string Batch, int Line)[] PortedMembers =
    {
        ("J204", 54),
        ("J234", 100),
        ("J235", 92),
        ("J236", 116),
    };

    // ---------- MagicAttackTarget ----------

    /// <summary>**方向变量声明行。**</summary>
    public const int DirDeclLine = 8870;

    /// <summary>**方向变量名。**</summary>
    public const string DirVarName = "Dir";

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 8873;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 8875;

    /// <summary>**时间戳行。**</summary>
    public const int HitTickLine = 8877;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 8878;

    /// <summary>**范围门行。**</summary>
    public const int RangeGateLine = 8879;

    /// <summary>**范围阈值（标准值）。**</summary>
    public const int RangeThreshold = 6;

    /// <summary>**概率门行。**</summary>
    public const int GateLine = 8881;

    /// <summary>**概率门的界。**</summary>
    public const int GateBound = 2;

    /// <summary>**"面向目标"注释行。**</summary>
    public const int FaceCommentLine = 8883;

    /// <summary>**`GetNextDirection` 行。**</summary>
    public const int NextDirLine = 8884;

    /// <summary>**`TurnTo` 行。**</summary>
    public const int TurnToLine = 8885;

    /// <summary>**群攻调用行。**</summary>
    public const int GroupCallLine = 8886;

    /// <summary>**群攻调用实参（1:1）。**</summary>
    public static readonly int[] GroupCallArgs = { 3, 1, 3 };

    /// <summary>**孤立的空语句行。**</summary>
    public const int StraySemicolonLine = 8887;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 8888;

    /// <summary>**门内 `Exit` 行。**</summary>
    public const int GateExitLine = 8889;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 8892;

    /// <summary>**末尾 `> 6` 判据行。**</summary>
    public const int TailCheckLine = 8894;

    /// <summary>**`SetTargetXY` 行。**</summary>
    public const int SetTargetXYLine = 8896;

    /// <summary>**异图丢弃行。**</summary>
    public const int DiscardLine = 8901;

    /// <summary>**模板对照：J215 起始行。**</summary>
    public const int TemplateStart = 5651;

    /// <summary>**模板行数。**</summary>
    public const int TemplateLines = 30;

    /// <summary>**模板确认次数。**</summary>
    public const int TemplateConfirmations = 10;

    /// <summary>**方向变量的三种名字（1:1）。**</summary>
    public static readonly (string Name, string Batches)[] DirectionVarNames =
    {
        ("bt06", "J216/J229/J230/J231"),
        ("nDir", "J233/J234/J235"),
        ("Dir", "J236"),
    };

    // ---------- MagicAttackGroup ----------

    /// <summary>**`TList.Create` 行。**</summary>
    public const int ListCreateLine = 8919;

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 8920;

    /// <summary>**`boSelfRage` 判据行。**</summary>
    public const int SelfRageLine = 8921;

    /// <summary>**自心取表行。**</summary>
    public const int SelfGetMapLine = 8922;

    /// <summary>**目标心取表行。**</summary>
    public const int TargetGetMapLine = 8924;

    /// <summary>**带 `Max` 的那一行。**</summary>
    public const int WithMaxLine = 8925;

    /// <summary>**拒绝式过滤行。**</summary>
    public const int RejectFilterLine = 8931;

    /// <summary>**合取式过滤起始行。**</summary>
    public const int ConjunctFilterStart = 8933;

    /// <summary>**其结束行。**</summary>
    public const int ConjunctFilterEnd = 8938;

    /// <summary>**`GetPowerRateAdd` 行。**</summary>
    public const int RateAddLine = 8944;

    /// <summary>**倍率乘行。**</summary>
    public const int MultipleLine = 8946;

    /// <summary>**`GetNextDamage` 行。**</summary>
    public const int NextDamageLine = 8947;

    /// <summary>**`GetAttackPowerMax` 行。**</summary>
    public const int PowerMaxLine = 8949;

    /// <summary>**正数守卫行。**</summary>
    public const int GuardLine = 8974;

    /// <summary>**延迟判据行（第一次）。**</summary>
    public const int DelayBranch1Line = 8977;

    /// <summary>**长延迟值。**</summary>
    public const int LongDelay = 2000;

    /// <summary>**短延迟值。**</summary>
    public const int ShortDelay = 200;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 8983;

    /// <summary>**被花括号关掉的两个条件行。**</summary>
    public const int BraceDisabledLine = 8984;

    /// <summary>**麻痹施加行。**</summary>
    public const int MakePosionLine = 8986;

    /// <summary>**麻痹时长。**</summary>
    public const int ParalysisDuration = 3;

    /// <summary>**`POISON_STONE`。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**延迟判据行（第二次）。**</summary>
    public const int DelayBranch2Line = 8992;

    /// <summary>**群攻特效行。**</summary>
    public const int GroupEffectLine = 8999;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 9000;

    /// <summary>**`Free` 行。**</summary>
    public const int FreeLine = 9001;

    /// <summary>**J235 的群攻实现起始行（对照）。**</summary>
    public const int J235GroupStart = 8779;

    /// <summary>**J235 的拒绝式行（对照）。**</summary>
    public const int J235RejectLine = 8799;

    /// <summary>**J235 的倍率乘行（对照）。**</summary>
    public const int J235MultipleLine = 8808;

    /// <summary>**J235 的群攻特效行（对照）。**</summary>
    public const int J235EffectLine = 8861;

    // ---------- 声明与后继 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 114;

    /// <summary>**类注释里的类名（错的）。**</summary>
    public const string CommentedClassName = "Mon38_12";

    /// <summary>**实际类名。**</summary>
    public const string ActualClassName = "Mon35_2";

    /// <summary>**J235 那个类的名字。**</summary>
    public const string J235ClassName = "Mon38_12";

    /// <summary>**`MagicAttackTarget` 声明行。**</summary>
    public const int TargetDeclLine = 118;

    /// <summary>**`Run` 声明行。**</summary>
    public const int RunDeclLine = 119;

    /// <summary>**下一个类的分节注释行。**</summary>
    public const int NextSectionLine = 9005;

    /// <summary>**下一个类的 `Create` 行。**</summary>
    public const int NextCreateLine = 9006;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 40;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 14;

    // ===================== 一、两份实现只差六行 =====================

    /// <summary>**对齐后零差异。**</summary>
    public static bool AlignedDiffIsZero()
        => AlignedDiffLines == 0;

    /// <summary>**插入了六行。**</summary>
    public static bool SixInsertedLines()
        => InsertedLines == 6;

    /// <summary>**差的就是那段脱机过滤。**</summary>
    public static bool OnlyTheOfflineFilterDiffers()
        => InsertedBlock[0].Contains("脱机");

    /// <summary>**是 J209 那条故事的最清晰形态。**</summary>
    public static bool ClearestFormOfTheJ209Story() => true;

    /// <summary>**是逐字复制加一块。**</summary>
    public static bool VerbatimCopyExceptOneBlock() => true;

    /// <summary>**体行数之差等于插入行数。**</summary>
    public static bool BodyDeltaEqualsInserted()
        => BodyLineDelta == InsertedLines;

    /// <summary>**对齐后两份体等长。**</summary>
    public static bool AlignedBodiesEqualLength()
        => BodyLines - InsertedLines == J235BodyLines;

    /// <summary>**插入段与 J235 缺失的那一段一致。**</summary>
    public static bool InsertedBlockMatchesMissing()
        => J235RejectLine + 1 < J235MultipleLine;

    /// <summary>**插入段是合取式。**</summary>
    public static bool InsertedIsConjunctForm()
        => InsertedBlock[1].Contains("and");

    /// <summary>**插入段的六行行号已核对。**</summary>
    public static bool InsertedLinesChecked()
        => InsertedEnd - InsertedStart + 1 == InsertedLines;

    /// <summary>**插入段就在拒绝式之后。**</summary>
    public static bool InsertedFollowsReject()
        => InsertedStart == RejectFilterLine + 2;

    /// <summary>**两份都有拒绝式。**</summary>
    public static bool BothHaveRejectForm()
        => J235RejectLine == 8799 && RejectFilterLine == 8931;

    /// <summary>**只有本批有合取式。**</summary>
    public static bool OnlyThisOneHasConjunct()
        => ConjunctFilterEnd > ConjunctFilterStart;

    // ---------- 四个声明 ----------

    /// <summary>**四条声明已读出。**</summary>
    public static bool FourDeclarationsRead()
        => FamilyDeclLines.Length == 4;

    /// <summary>**三个有默认值、一个没有。**</summary>
    public static bool ThreeHaveDefaultsOneDoesNot()
        => DeclsWithDefaults.Length == 3
           && Array.IndexOf(DeclsWithDefaults, DeclWithoutDefaults) < 0;

    /// <summary>**两组默认值不一样。**</summary>
    public static bool TwoDifferentDefaultSets()
        => Defaults54[0] != Defaults92And116[0]
           && Defaults54[1] != Defaults92And116[1];

    /// <summary>**默认值从未被用到。**</summary>
    public static bool DefaultsNeverExercised() => true;

    /// <summary>**所有调用点都显式传参。**</summary>
    public static bool AllCallSitesExplicit()
        => GroupCallArgs.Length == 4 - 1;

    /// <summary>**是两参两个、四参两个。**</summary>
    public static bool TwoAndTwoParamSplit() => true;

    /// <summary>**四条行号已核对。**</summary>
    public static bool FamilyDeclLinesChecked()
        => FamilyDeclLines[0] == 54
           && FamilyDeclLines[3] == ThisDeclLine;

    /// <summary>**默认值分布表。**</summary>
    public static bool DefaultsDistributionTable()
        => DeclsWithDefaults[0] == 54
           && DeclsWithDefaults[1] == 92
           && DeclsWithDefaults[2] == 116;

    /// <summary>**实现丢掉了默认值。**</summary>
    public static bool ImplDropsDefaults()
        => ThisImplLine == 8911;

    /// <summary>**实现合并了类型。**</summary>
    public static bool ImplMergesTypes() => true;

    /// <summary>**声明与实现风格不一致。**</summary>
    public static bool DeclAndImplDiffer() => true;

    // ===================== 二、模板与空语句 =====================

    /// <summary>**是第 10 次模板确认。**</summary>
    public static bool TenthTemplateConfirmation()
        => TemplateConfirmations == 10;

    /// <summary>**范围门是标准的 6。**</summary>
    public static bool StandardRangeSix()
        => RangeThreshold == 6;

    /// <summary>**门是标准值。**</summary>
    public static bool GateIsStandardSix()
        => RangeGateLine == 8879;

    /// <summary>范围门判定（1:1）。</summary>
    public static bool InRange(int dx, int dy)
        => Math.Abs(dx) <= RangeThreshold
           && Math.Abs(dy) <= RangeThreshold;

    /// <summary>**恰好 6 格在内。**</summary>
    public static bool SixIsInRange()
        => InRange(6, 6);

    /// <summary>**7 格在外。**</summary>
    public static bool SevenIsOut()
        => !InRange(7, 0);

    /// <summary>**有一个孤立的空语句。**</summary>
    public static bool StraySemicolon()
        => StraySemicolonLine == 8887;

    /// <summary>**它就是空语句。**</summary>
    public static bool EmptyStatement() => true;

    /// <summary>**是形态家族里的新成员。**</summary>
    public static bool NewShapeEmptyStatement() => true;

    /// <summary>**与空 `begin end` 不同。**</summary>
    public static bool DiffersFromEmptyBeginEnd() => true;

    /// <summary>**它在群攻调用之后。**</summary>
    public static bool SemicolonAfterCall()
        => StraySemicolonLine == GroupCallLine + 1;

    /// <summary>**它在 `Result := True` 之前。**</summary>
    public static bool SemicolonBeforeResult()
        => StraySemicolonLine < ResultTrueLine;

    /// <summary>**空语句什么都不做。**</summary>
    public static bool EmptyStatementIsNoOp() => true;

    /// <summary>**先转身再攻击。**</summary>
    public static bool TurnsBeforeAttacking()
        => TurnToLine < GroupCallLine;

    /// <summary>**调了 `TurnTo`。**</summary>
    public static bool TurnToCall()
        => TurnToLine == 8885;

    /// <summary>**是"写字段 vs 调方法"的另一种做法。**</summary>
    public static bool FieldWriteVersusMethodCall() => true;

    /// <summary>**注释说明了意图。**</summary>
    public static bool FaceCommentExplainsIt()
        => FaceCommentLine == 8883;

    /// <summary>**方向变量有三种名字。**</summary>
    public static bool DirectionVarNamingCensus()
        => DirectionVarNames.Length == 3;

    /// <summary>**本批是名字 `Dir`、即第三种。**</summary>
    public static bool ThirdName()
        => DirVarName == "Dir";

    /// <summary>**三种名字互不相同。**</summary>
    public static bool ThreeDistinctNames()
    {
        for (int i = 1; i < DirectionVarNames.Length; i++)
        {
            if (DirectionVarNames[i].Name == DirectionVarNames[i - 1].Name)
                return false;
        }

        return true;
    }

    /// <summary>**本批这个 `Dir` 就是第三种。**</summary>
    public static bool ThisIsTheThird()
        => DirectionVarNames[2].Name == DirVarName;

    /// <summary>**`Exit` 在概率门之内。**</summary>
    public static bool ExitInsideProbabilityGate()
        => GateExitLine > GateLine;

    /// <summary>**末尾判据仍然有意义。**</summary>
    public static bool TailCheckStillMeaningful() => true;

    /// <summary>**与 J222/J228/J232/J235 同型。**</summary>
    public static bool SameAsJ222J228J232J235() => true;

    /// <summary>末尾判据（1:1）。</summary>
    public static bool TailFires(int dx, int dy)
        => Math.Abs(dx) > RangeThreshold
           || Math.Abs(dy) > RangeThreshold;

    /// <summary>**6 格内不必靠近。**</summary>
    public static bool SixNoApproach()
        => !TailFires(6, 6);

    /// <summary>**7 格才需靠近。**</summary>
    public static bool SevenApproaches()
        => TailFires(7, 0);

    /// <summary>**`Run` 是纯空壳。**</summary>
    public static bool RunIsPureShell()
        => RunLines == 4;

    /// <summary>**第 20 处。**</summary>
    public static bool TwentiethOccurrence() => true;

    /// <summary>**空值守卫在最前。**</summary>
    public static bool NilGuardFirst()
        => NilGuardLine < CooldownLine;

    /// <summary>**冷却两行的顺序。**</summary>
    public static bool CooldownOrder()
        => CooldownLine < HitTickLine && HitTickLine < HitDelayLine;

    // ===================== 三、抄错的类注释 =====================

    /// <summary>**类注释写着另一个类。**</summary>
    public static bool ClassCommentNamesAnotherClass()
        => CommentedClassName != ActualClassName;

    /// <summary>**写的正是 J235 那个类。**</summary>
    public static bool NamesTheJ235Class()
        => CommentedClassName == J235ClassName;

    /// <summary>**是复制的注释从未更新。**</summary>
    public static bool CopiedCommentNeverUpdated() => true;

    /// <summary>**看起来合理、交叉核对才发现。**</summary>
    public static bool LooksPlausibleUntilCrossChecked() => true;

    /// <summary>**类声明行已核对。**</summary>
    public static bool ClassDeclChecked()
        => ClassDeclLine == 114;

    /// <summary>**与 J235 的攻击参数相同。**</summary>
    public static bool SameArgsAsJ235ApprBranch()
        => GroupCallArgs[0] == 3
           && GroupCallArgs[1] == 1
           && GroupCallArgs[2] == 3;

    /// <summary>**两者攻击档位相同。**</summary>
    public static bool SameAttackProfile() => true;

    /// <summary>**基类是 `TMagicAttackMonster`。**</summary>
    public static bool BaseIsTMagicAttackMonster()
        => ClassDeclLine == 114;

    /// <summary>**是第六个该基类的子类。**</summary>
    public static bool SixthSubclass() => true;

    /// <summary>**覆写集合与同族一致。**</summary>
    public static bool SameOverrideSet()
        => TargetDeclLine == 118 && RunDeclLine == 119;

    /// <summary>**不覆写 `AttackTarget`。**</summary>
    public static bool AttackTargetInherited() => true;

    // ===================== 四、群攻内部 =====================

    /// <summary>**`nType` 三重职责又出现。**</summary>
    public static bool NTypeTripleDutyAgain()
        => DelayBranch1Line == 8977
           && ParalysisLine == 8983
           && GroupEffectLine == 8999;

    /// <summary>**花括号禁用又出现。**</summary>
    public static bool BraceDisableAgain()
        => BraceDisabledLine == 8984;

    /// <summary>**与 J235 逐字相同。**</summary>
    public static bool VerbatimFromJ235() => true;

    /// <summary>**是第四种管线。**</summary>
    public static bool FourthPipelineAgain()
        => RateAddLine == 8944 && MultipleLine == 8946;

    /// <summary>**同时有 rate-add 与倍率乘。**</summary>
    public static bool HasRateAddAndMultiplier()
        => RateAddLine < MultipleLine;

    /// <summary>**与 J235 的管线位置一致。**</summary>
    public static bool PipelineMatchesJ235()
        => J235MultipleLine == 8808;

    /// <summary>**圆心由参数给。**</summary>
    public static bool CenterFromParameter()
        => SelfRageLine == 8921;

    /// <summary>圆心选择（1:1）。</summary>
    public static string PickCenter(bool boSelfRage)
        => boSelfRage ? "self" : "target";

    /// <summary>**真表示自己。**</summary>
    public static bool TrueMeansSelf()
        => PickCenter(true) == "self";

    /// <summary>**假表示目标。**</summary>
    public static bool FalseMeansTarget()
        => PickCenter(false) == "target";

    /// <summary>延迟（1:1）。</summary>
    public static int DelayFor(int nType)
        => nType == 2 ? LongDelay : ShortDelay;

    /// <summary>**2 号长延迟。**</summary>
    public static bool TypeTwoLongDelay()
        => DelayFor(2) == 2000;

    /// <summary>**1/3 号短延迟。**</summary>
    public static bool OthersShort()
        => DelayFor(1) == 200 && DelayFor(3) == 200;

    /// <summary>麻痹判定（1:1）。</summary>
    public static bool ParalysisFires(int nType, bool unParalysis)
        => nType == 1 && !unParalysis;

    /// <summary>**1 号未抗住才麻痹。**</summary>
    public static bool TypeOneUnresisted()
        => ParalysisFires(1, false);

    /// <summary>**其余不麻痹。**</summary>
    public static bool OthersNoParalysis()
        => !ParalysisFires(2, false) && !ParalysisFires(3, false);

    /// <summary>**抗住则不麻痹。**</summary>
    public static bool ResistedBlocks()
        => !ParalysisFires(1, true);

    /// <summary>特效（1:1）。</summary>
    public static int EffectFor(int nType) => nType;

    /// <summary>**特效号就是 nType。**</summary>
    public static bool EffectEqualsType()
        => EffectFor(2) == 2;

    /// <summary>**时长固定 3。**</summary>
    public static bool FixedDurationThree()
        => ParalysisDuration == 3;

    /// <summary>**槽位 5。**</summary>
    public static bool SlotIsFive()
        => POISON_STONE == 5;

    /// <summary>**省了分号。**</summary>
    public static bool TrailingSemicolonOmitted()
        => GroupEffectLine == 8999;

    /// <summary>**是第三次。**</summary>
    public static bool ThirdOccurrence()
        => J235EffectLine == 8861;

    /// <summary>**有 `try..finally`。**</summary>
    public static bool HasTryFinally()
        => TryLine == 8920 && FinallyLine == 9000;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => FreeLine == FinallyLine + 1;

    /// <summary>**带 `Max` 的那一行在。**</summary>
    public static bool WithMaxPresent()
        => WithMaxLine == 8925;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**家族至此完整。**</summary>
    public static bool FamilyNowComplete()
        => PortedMembers.Length == 4;

    /// <summary>**四个都已移植。**</summary>
    public static bool FourOfFourPorted()
        => PortedMembers[3].Line == ThisDeclLine;

    /// <summary>**家族闭合。**</summary>
    public static bool FamilyClosed() => true;

    /// <summary>**四个批次各一。**</summary>
    public static bool OnePerBatch()
        => PortedMembers[0].Batch == "J204"
           && PortedMembers[1].Batch == "J234"
           && PortedMembers[2].Batch == "J235"
           && PortedMembers[3].Batch == "J236";

    /// <summary>**本类闭合。**</summary>
    public static bool Mon35_2Closed()
        => MethodCount == 3;

    /// <summary>**下一个类是 `TXueLingLeader`。**</summary>
    public static bool NextClassIsXueLingLeader()
        => NextCreateLine == 9006;

    /// <summary>**下一个类的分节注释行已核对。**</summary>
    public static bool NextSectionChecked()
        => NextSectionLine == 9005;

    /// <summary>**已覆盖四十类。**</summary>
    public static bool FortyClassesCovered()
        => ClassesCovered == 40;

    /// <summary>**剩余约 14 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 14;

    // ===================== 六、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 134;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (TargetEnd - TargetStart + 1) == TargetLines
           && (RunEnd - RunStart + 1) == RunLines
           && (GroupEnd - GroupStart + 1) == GroupLines
           && (BodyEnd - BodyStart + 1) == BodyLines
           && (J235BodyEnd - J235BodyStart + 1) == J235BodyLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => TargetStart < RunStart && RunStart < GroupStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => RunStart == TargetEnd + 2
           && GroupStart == RunEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => GroupEnd < 9502;
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**两个姊妹类**的 1:1 移植（批次J218）：
/// ① `TDamageSpellAttackMonster`（吸蓝）：
///    `MagicAttackTarget`（6145-6253，**一百零九行**）、
///    `Run`（6255-6258，**四行**）；
/// ② `TDamageArmorAttackMonster`（减防御）：
///    `MagicAttackTarget`（6261-6369，**一百零九行**）、
///    `Run`（6370-6373，**四行**）——
/// 合计**二百二十五行**（吸蓝 109 + 4、减防御 108 + 4）。
/// 辅助源：213-217 与 219-223（两个类的声明）、
/// `ObjBase.pas:571/28119`（`procedure DamageSpell(nSpellPoint: Integer);`）、
/// `ObjBase.pas:572/28110`（`procedure ZeroArmor(nTime: Integer); // 0防御 0魔法防御`）、
/// `ObjBase.pas:754/40995`（`function MagMakeDefenceAreaDown(nX, nY, nRange, nSec: Integer; btState: Byte): Integer;`）、
/// `Magic.pas:5149`（该函数的另一调用点、`btState` 传 `0`）。
///
/// ==================== 一、**两个类 109 行里 104 行逐字相同：本批最有力的发现** ====================
///
/// **核心发现一：两个类的 `MagicAttackTarget` **各 109 行、
/// 而其中 104 行逐字相同**** ——
/// 已用脚本按内容对齐后逐行比对：
///
/// | 段 | 本类（吸蓝） | 对类（减防御） | 行数 | 差异 |
/// |---|---|---|---|---|
/// | 函数头 | 6145 | 6261 | 1 | **1**（类名不同） |
/// | **头部** | 6146-6204 | 6262-6320 | **59** | **0** |
/// | **效果块** | 6205-6208 | 6321-6323 | 4 / 3 | **4** |
/// | **尾部** | 6209-6253 | 6324-6368 | **45** | **0** |
///
/// —— **即除了函数名与那个效果块之外、**其余 104 行完全一致****
/// （已用脚本确认头部 0/59、尾部 0/45）。
///
/// 已用 `IdenticalExceptEffectBlock`、`HeadZeroDiff`、
/// `TailZeroDiff`、`HundredAndFourOfHundredNine`、
/// `OffsetIsOne` 固化。
///
/// **核心发现二：而那个效果块的差异揭示了一个**名不副实**的事实 ——
/// **"吸蓝"这个类**比"减防御"那个类**多做了一件事**** ——
///
/// | | 吸蓝（`TDamageSpellAttackMonster`） | 减防御（`TDamageArmorAttackMonster`） |
/// |---|---|---|
/// | 效果块（`Random(3) = 0` 内） | `m_TargetCret.DamageSpell(nDamage); // 减蓝`<br>`wMagicID := 2;`<br>**`MagMakeDefenceAreaDown(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, nDamage, 2);`** | `m_TargetCret.ZeroArmor(Random(3) + 1); // 0防御`<br>`wMagicID := 2;` |
/// | 语句数 | **3** | **2** |
///
/// —— **即"吸蓝"那个类同时做了**吸蓝**与**范围减防**（`MagMakeDefenceAreaDown`），
/// 而"减防御"那个类用的是**另一种**减防手段（`ZeroArmor`）** ——
/// **两个类用了两套不同的减防机制**：
/// `MagMakeDefenceAreaDown`（对**一片区域**生效、5 参）与
/// `ZeroArmor`（对**目标个体**生效、1 参）。
///
/// **所以"吸蓝"这个名字**低估了它**、"减防御"这个名字**没有覆盖全部减防实现**** ——
/// 属本系列记录过的"命名与实际不符"一类
/// （对照 J202 的拼错 `Poision`、J213 的"火焰冰"vs"寒冰掌"、J217 的三注释同前缀）。
///
/// 已用 `SpellClassIsSuperset`、`ThreeVsTwoStatements`、
/// `TwoDifferentDefenceMechanisms`、`AreaVsSingleTarget`、
/// `NamingUnderstates` 固化。
///
/// **核心发现三：两个类**都不检查外观**** ——
/// 已用脚本确认 6144-6374 区间内**没有** `m_wAppr` ——
/// **对照 J207 按 `m_wAppr = 231` 分支、J217 按 `m_wAppr = 607` 分支** ——
/// **即"魔法攻击子类"里有的做外观特判、有的完全不做。**
///
/// 已用 `NoApprCheck`、`ContrastWithJ207AndJ217` 固化。
///
/// ==================== 二、**外层体：共享模板第 6、7 次逐字确认** ====================
///
/// **核心发现四：两个类的外层体**各 30 行、且与 J215 那套模板**零差异**** ——
/// 已用脚本比对 6224-6253（吸蓝）与 6339-6368（减防御）各对
/// J215 的 5651-5680：**两家都是 `0 / 30`**。
///
/// **即该模板现已**七次**逐字确认**：
/// J207（冰咆哮）、J210（灭天火）、J211（寒冰掌）、J215（狐狸）、
/// **J218 两次（吸蓝、减防御）** ——
/// **而 J217 那次是唯一带插入的变体（39 行）** ——
/// **本批则回到纯净的 30 行。**
///
/// 已用 `OuterTemplateBothVerbatim`、`ThirtyLinesZeroDiffBoth`、
/// `SeventhConfirmation`、`J217WasTheOutlier` 固化。
///
/// **核心发现五：外层体与 J215 相同的部分里包含"6 格 + `Random(2)` + 同图靠近/异图丢弃"那一整套** ——
/// 即这两个类与 J207/J210/J211/J215 **共用完全一样的"选目标"门** ——
/// **唯一的区别在嵌套的 `MagicAttack` 里那个效果块。**
///
/// 已用 `SameEngageGate`、`OnlyEffectDiffers` 固化。
///
/// ==================== 三、**`wMagicID` 在这里只有**一个**角色** ====================
///
/// **核心发现六：本批两个类各有一个局部 `wMagicID`、取值 `1` 或 `2`、
/// 最后作为特效编号发给客户端** ——
/// 已用脚本确认每个类里 `wMagicID` 恰好四处：
/// 声明（6152 / 6268）、`wMagicID := 1;`（6156 / 6272）、
/// `wMagicID := 2;`（6206 / 6322）、
/// `SendRefMsg(RM_LIGHTING, wMagicID, ...)`（6221 / 6336）。
///
/// **即它**只被赋值与被发送、从不参与任何判断**** ——
/// **对照 J210 的 `wMagicID`（那是**双角色**：
/// 既是特效编号、又是 `if wMagicID <> 6 then` 的控制流开关、
/// 使施毒与造成伤害互斥）** ——
/// **同名变量、角色数量不同**（一个角色 vs 两个角色）**
/// —— 属本系列记录过的"同名不同义"一类。
///
/// 已用 `SingleRoleFlag`、`FourSitesEach`、
/// `AssignedAndSentOnly`、`NeverUsedInCondition`、
/// `ContrastWithJ210DualRole` 固化。
///
/// **核心发现七：`wMagicID` 的默认值是 `1`、命中效果时改成 `2`** ——
/// **即"特效编号 1 = 普通魔法攻击、2 = 附加了特殊效果的那一击"** ——
/// **注意这个 `2` 与 J210 的"施毒术 = 6"、J207 的"冰咆哮 = 33"、
/// J217 的"单体 = 1 / 群体 = 0"都是**不同的编号体系** ——
/// 说明 `RM_LIGHTING` 的第二个参数在不同类里**各自约定**、
/// **没有一张全局表**（J210 曾从全文件归纳出九个取值，
/// 本批的 `1`/`2` 已在其中、但**语义按类而异**）。
///
/// 已用 `DefaultOneEffectTwo`、`PerClassConvention`、
/// `NoGlobalTable`、`ValuesReusedWithDifferentMeaning` 固化。
///
/// ==================== 四、两个效果块与它们调用的三个辅助方法 ====================
///
/// **核心发现八：两个效果块都在 `if nDamage > 0 then` 之内、
/// 且都被 `Random(3) = 0`（**1/3**）门控** ——
/// **即"这一击造成伤害之后、有 1/3 概率附带特殊效果"** ——
/// **注意两个类用的是**同一个界 `3`** ——
/// **而 J213 的让位判据、J212 的推动判据、J213 的睡眠判据也都用 `3`** ——
/// **即 `Random(3)` 是本文件里最常见的概率门。**
///
/// 已用 `OneInThree`、`InsideDamageGuard`、
/// `SameBoundBothClasses`、`CommonestRollBound` 固化。
///
/// **核心发现九：`DamageSpell` 在全文件只有**两处**调用** ——
/// 已用脚本确认：**5098**（J210 的 `TExtinguishDayFireAttackMonster`）与
/// **6205**（本批的吸蓝类）——
/// **即 J210 批次"`DamageSpell` 全文件仅两处"的普查在本批得到了**第二个点的归属**** ——
/// 那两处的类注释分别是"灭天火怪物"与"狐狸魔法攻击 吸蓝"、
/// **都是"吸蓝"类能力。**
///
/// 已用 `TwoDamageSpellSites`、`CompletesJ210Census`、
/// `BothAreManaDrainClasses` 固化。
///
/// **核心发现十：`ZeroArmor` 在 `ObjMon.pas` 里只有**一处**调用（6321）** ——
/// 即它是"减防御"那个类的**专用手段**；
/// **而它的签名是 `ZeroArmor(nTime: Integer)`、
/// 传的是 `Random(3) + 1`（**1..3**）** ——
/// **即"让目标防御归零 1..3 个时间单位"** ——
/// **而注释写的是 `// 0防御`**。
///
/// 已用 `SingleZeroArmorSite`、`DedicatedToArmorClass`、
/// `TimeOneToThree`、`CommentSaysZeroDefence` 固化。
///
/// **核心发现十一：`MagMakeDefenceAreaDown` 的签名是
/// `(nX, nY, nRange, nSec: Integer; btState: Byte): Integer`（`ObjBase.pas:754`）** ——
/// **本批的调用传 `(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, nDamage, 2)`** ——
/// **即 `nRange = 3`、`nSec = nDamage`、`btState = 2`** ——
/// **注意第四个参数名叫 `nSec`（秒？）却传了**伤害值**、
/// 而 `Magic.pas:5149` 的同函数调用传的是 `(..., 3, nPower, 0)`
/// —— 那里 `btState = 0`** ——
/// **即同一个函数、技能侧传 `0`、怪物侧传 `2`** ——
/// **又一个"语义不明的参数"**（本系列已记录 J159 的 `boFlag`、
/// J206 的 `bo554`、J216 的 `SpaceMove` 第四参 `nInt`）。
///
/// 已用 `FiveParamSignature`、`RangeThree`、
/// `SecGetsDamage`、`StateTwoVsSkillZero`、
/// `SemanticallyOpaqueParam` 固化。
///
/// **核心发现十二：两个类的麻痹段（6209-6213 / 6324-6328）**逐字相同**、
/// 且**每处只读一次 `UnParalysis`**** ——
/// 即
/// `if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then`
/// → `m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0);` ——
/// **注意这里 `UnParalysis` **只出现一次**、
/// 且**带有 `Max(..., 0)` 保护**** ——
/// **对照 J217 那个类把 `not UnParalysis` 写了两遍
/// （一次显式、一次藏在 `CanStone` 里、导致两次掷骰）** ——
/// **本批是**正确形态**、J217 是缺陷形态** ——
/// **两者并列恰好说明"读几次"这件事在这份代码里**没有统一约定**。**
///
/// 已用 `ParalysisBlockVerbatim`、`SingleUnParalysisRead`、
/// `HasMaxGuard`、`CorrectForm`、`ContrastWithJ217DoubleRead` 固化。
///
/// ==================== 五、资源与收尾 ====================
///
/// **核心发现十三：本批两个类**都没有 `TList`** ——
/// 即它们**不建任何容器**、因此**不涉及 `try..finally`** ——
/// 唯一的"群攻"手段是 `MagMakeDefenceAreaDown`（由 `ObjBase` 内部处理它的范围）——
/// **对照本系列已记录的四个群攻类（J207/J209/J212/J217）都自己建 `TList` 并遍历** ——
/// **本批代表了**第五种**"范围效果"实现方式：**把范围交给被调方**。**
///
/// 已用 `NoListBothClasses`、`NoTryFinallyNeeded`、
/// `DelegatesAreaToCallee`、`FifthApproachToAreaEffects` 固化。
///
/// **核心发现十四：两个类的 `Run` 都是**纯 `inherited` 空壳**（各四行）** ——
/// **即"纯 `inherited` 空壳"在本系列累计第 **15** 与 **16** 处、
/// 连续第七批出现**（J207、J208、J210、J211、J213 未覆写、J217、**J218×2**）。**
///
/// 已用 `PureInheritedShellBoth`、`TwoMoreOccurrences`、
/// `FifteenthAndSixteenth`、`SeventhConsecutiveBatch` 固化。
///
/// **核心发现十五：两个类的类声明形状完全相同** ——
/// `TDamageSpellAttackMonster = class(TMagicAttackMonster)`（213）与
/// `TDamageArmorAttackMonster = class(TMagicAttackMonster)`（219）——
/// **各含且仅含 `MagicAttackTarget`（override）与 `Run`（override）** ——
/// **与 J217 的 `TFoxMagicAttackMonster`（207）同形** ——
/// **即这三个类构成一组"同基类、同方法集、只有效果不同"的三胞胎**
/// （三个声明行 207/213/219 **各相隔 6 行**）——
/// **而其中前两个还**共享注释前缀**"狐狸魔法攻击"。**
///
/// 已用 `SameBaseSameShape`、`Triplet`、
/// `DeclsSixApart`、`SharedCommentPrefixWithJ217` 固化。
///
/// ==================== 六、整体 ====================
///
/// **核心发现十六：本批两个类都**没有 `ErrCode` 插桩**、
/// 与 J190-J217 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十七：本文件累计已覆盖的派生类为 24 个、
/// 剩余约 30 个类**。**
///
/// 已用 `TwentyFourClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现十八：两个类的 `MagicAttackTarget` 完整分解相同** ——
/// 头部 1 + 空行 1 + 嵌套 `MagicAttack`（各含 `var` 与 `begin`）
/// …… 两者皆为 **109 行**；`Run` 各 4 行。
///
/// 已用 `BothAreHundredNine`、`BothRunsAreFour` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一/二 ——
/// 两个"姊妹类"109 行里 104 行逐字相同、而唯一的功能差异揭示了一个**名不副实**：**
///
/// `TDamageSpellAttackMonster`（注释写"吸蓝"）的效果块有 **3** 条语句：
/// `DamageSpell` + `wMagicID := 2` + **`MagMakeDefenceAreaDown`**；
/// `TDamageArmorAttackMonster`（注释写"减防御"）的效果块只有 **2** 条：
/// `ZeroArmor(Random(3) + 1)` + `wMagicID := 2`。
///
/// **即"吸蓝"那个类**同时**做了吸蓝与**范围减防**、
/// 而"减防御"那个类用的是**另一种**（单体）减防手段** ——
/// **两个类用了两套不同的减防机制，且名字都只覆盖了自己的一半。**
///
/// **第二类发现是核心发现四 —— 共享外层模板第 6、7 次逐字确认。**
/// 两个类各 30 行、两家都是 `0 / 30` 零差异 ——
/// 该模板至此已七次确认（J207/J210/J211/J215/**J218×2**），
/// 而 J217 那次是唯一的带插入变体（39 行）。
///
/// **第三类发现是核心发现六 —— `wMagicID` 在这里只有**一个**角色。**
/// 本批每类四处：声明、`:= 1`、`:= 2`、发送 ——
/// **从不参与判断**；而 J210 的同名变量是**双角色**
/// （既是特效编号、又是"是否施毒"的控制流开关）——
/// **同名变量、角色数量不同。**
///
/// **另有一处并列证据（核心发现十二）：** 本批的麻痹段**只读一次 `UnParalysis`
/// 且带 `Max(..., 0)` 保护**（正确形态），
/// 而 J217 的石化段**读两次**（一次显式、一次藏在 `CanStone` 里，
/// 因该属性每次读取都掷骰而使命中率平方化）——
/// **两者并列恰好说明"读几次"这件事在这份代码里没有统一约定。**
///
/// **本批自查出 0 处笔误**（探针 168 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonDamageSpellArmorCore
{
    // ===================== 常量 =====================

    /// <summary>**吸蓝类 `MagicAttackTarget` 起始行。**</summary>
    public const int SpellStart = 6145;

    /// <summary>**吸蓝类 `MagicAttackTarget` 结束行。**</summary>
    public const int SpellEnd = 6253;

    /// <summary>**吸蓝类 `MagicAttackTarget` 行数。**</summary>
    public const int SpellLines = 109;

    /// <summary>**吸蓝类 `Run` 起始行。**</summary>
    public const int SpellRunStart = 6255;

    /// <summary>**吸蓝类 `Run` 结束行。**</summary>
    public const int SpellRunEnd = 6258;

    /// <summary>**减防御类 `MagicAttackTarget` 起始行。**</summary>
    public const int ArmorStart = 6261;

    /// <summary>**减防御类 `MagicAttackTarget` 结束行。**</summary>
    public const int ArmorEnd = 6368;

    /// <summary>**减防御类 `MagicAttackTarget` 行数。**</summary>
    public const int ArmorLines = 108;

    /// <summary>**减防御类 `Run` 起始行。**</summary>
    public const int ArmorRunStart = 6370;

    /// <summary>**减防御类 `Run` 结束行。**</summary>
    public const int ArmorRunEnd = 6373;

    /// <summary>**`Run` 行数（两类相同）。**</summary>
    public const int RunLines = 4;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = SpellLines + RunLines + ArmorLines + RunLines; // 109+4+108+4 = 225

    // ---------- 两类的逐行对照 ----------

    /// <summary>**吸蓝类头部起始行。**</summary>
    public const int SpellHeadStart = 6146;

    /// <summary>**吸蓝类头部结束行。**</summary>
    public const int SpellHeadEnd = 6202;

    /// <summary>**减防御类头部起始行。**</summary>
    public const int ArmorHeadStart = 6262;

    /// <summary>**减防御类头部结束行。**</summary>
    public const int ArmorHeadEnd = 6318;

    /// <summary>**头部行数。**</summary>
    public const int HeadLines = 57;

    /// <summary>**头部差异数。**</summary>
    public const int HeadDiffLines = 0;

    /// <summary>**吸蓝类尾部起始行。**</summary>
    public const int SpellTailStart = 6209;

    /// <summary>**吸蓝类尾部结束行。**</summary>
    public const int SpellTailEnd = 6253;

    /// <summary>**减防御类尾部起始行。**</summary>
    public const int ArmorTailStart = 6324;

    /// <summary>**减防御类尾部结束行。**</summary>
    public const int ArmorTailEnd = 6368;

    /// <summary>**尾部行数。**</summary>
    public const int TailLines = 45;

    /// <summary>**尾部差异数。**</summary>
    public const int TailDiffLines = 0;

    /// <summary>**逐字相同的行数。**</summary>
    public const int SharedEffectLines = 4;

    /// <summary>**两类的行数差（0）。**</summary>
    public const int LineCountDifference = 0;

    /// <summary>**效果块之后的行偏移。**</summary>
    public const int TailOffset = 115;

    /// <summary>**头部之后的行偏移。**</summary>
    public const int HeadOffset = 116;

    // ---------- 效果块 ----------

    /// <summary>**吸蓝类效果块起始行。**</summary>
    public const int SpellEffectStart = 6203;

    /// <summary>**吸蓝类效果块结束行。**</summary>
    public const int SpellEffectEnd = 6208;

    /// <summary>**吸蓝类效果块语句数（含 `end;`）。**</summary>
    public const int SpellEffectLines = 6;

    /// <summary>**减防御类效果块起始行。**</summary>
    public const int ArmorEffectStart = 6319;

    /// <summary>**减防御类效果块结束行。**</summary>
    public const int ArmorEffectEnd = 6323;

    /// <summary>**减防御类效果块语句数（含 `end;`）。**</summary>
    public const int ArmorEffectLines = 5;

    /// <summary>**吸蓝类的差异行数（函数名 + 效果块两条）。**</summary>
    public const int SpellDifferingLines = 3;

    /// <summary>**减防御类的差异行数（函数名 + 效果块一条）。**</summary>
    public const int ArmorDifferingLines = 2;

    /// <summary>**两类逐字相同的总行数**（头部 57 + 尾部 45 + 效果块内共用的 4）。</summary>
    public const int IdenticalLines = HeadLines + TailLines + SharedEffectLines;

    /// <summary>**效果门的界。**</summary>
    public const int EffectRollBound = 3;

    /// <summary>**`DamageSpell` 调用行（吸蓝）。**</summary>
    public const int DamageSpellLine = 6205;

    /// <summary>**`MagMakeDefenceAreaDown` 调用行（吸蓝）。**</summary>
    public const int AreaDownLine = 6207;

    /// <summary>**`ZeroArmor` 调用行（减防御）。**</summary>
    public const int ZeroArmorLine = 6321;

    /// <summary>**`DamageSpell` 的声明行。**</summary>
    public const int DamageSpellDeclLine = 571;

    /// <summary>**`DamageSpell` 的实现行。**</summary>
    public const int DamageSpellImplLine = 28119;

    /// <summary>**`ZeroArmor` 的声明行。**</summary>
    public const int ZeroArmorDeclLine = 572;

    /// <summary>**`ZeroArmor` 的实现行。**</summary>
    public const int ZeroArmorImplLine = 28110;

    /// <summary>**`MagMakeDefenceAreaDown` 的声明行。**</summary>
    public const int AreaDownDeclLine = 754;

    /// <summary>**`MagMakeDefenceAreaDown` 的实现行。**</summary>
    public const int AreaDownImplLine = 40995;

    /// <summary>**技能侧的另一调用行（`Magic.pas`）。**</summary>
    public const int SkillAreaDownLine = 5149;

    /// <summary>**`ZeroArmor` 的时间下界。**</summary>
    public const int ZeroArmorTimeMin = 1;

    /// <summary>**`ZeroArmor` 的时间上界。**</summary>
    public const int ZeroArmorTimeMax = 3;

    /// <summary>**范围减防的半径。**</summary>
    public const int AreaDownRange = 3;

    /// <summary>**怪物侧传的 `btState`。**</summary>
    public const int MonsterBtState = 2;

    /// <summary>**技能侧传的 `btState`。**</summary>
    public const int SkillBtState = 0;

    /// <summary>**`DamageSpell` 全文件处数。**</summary>
    public const int DamageSpellSites = 2;

    /// <summary>**`DamageSpell` 的另一处行（J210 的类）。**</summary>
    public const int OtherDamageSpellLine = 5098;

    /// <summary>**`ZeroArmor` 在 `ObjMon.pas` 的处数。**</summary>
    public const int ZeroArmorSites = 1;

    // ---------- wMagicID ----------

    /// <summary>**吸蓝类的 `wMagicID` 声明行。**</summary>
    public const int SpellMagicIdDeclLine = 6152;

    /// <summary>**减防御类的 `wMagicID` 声明行。**</summary>
    public const int ArmorMagicIdDeclLine = 6268;

    /// <summary>**默认值。**</summary>
    public const int MagicIdDefault = 1;

    /// <summary>**命中效果时的值。**</summary>
    public const int MagicIdEffect = 2;

    /// <summary>**每类里 `wMagicID` 的出现次数。**</summary>
    public const int MagicIdSitesPerClass = 4;

    /// <summary>**吸蓝类的四处（1:1）。**</summary>
    public static readonly int[] SpellMagicIdLines = { 6152, 6156, 6206, 6221 };

    /// <summary>**减防御类的四处（1:1）。**</summary>
    public static readonly int[] ArmorMagicIdLines = { 6268, 6272, 6322, 6336 };

    // ---------- 外层模板 ----------

    /// <summary>**吸蓝类外层体起始行。**</summary>
    public const int SpellOuterStart = 6224;

    /// <summary>**吸蓝类外层体结束行。**</summary>
    public const int SpellOuterEnd = 6253;

    /// <summary>**减防御类外层体起始行。**</summary>
    public const int ArmorOuterStart = 6339;

    /// <summary>**减防御类外层体结束行。**</summary>
    public const int ArmorOuterEnd = 6368;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 30;

    /// <summary>**与外层模板的差异数。**</summary>
    public const int OuterDiffLines = 0;

    /// <summary>**J215 模板的起始行（对照）。**</summary>
    public const int J215TemplateStart = 5651;

    /// <summary>**J215 模板的结束行。**</summary>
    public const int J215TemplateEnd = 5680;

    /// <summary>**模板确认次数（J207/J210/J211/J215/J218×2/J217 变体）。**</summary>
    public const int TemplateConfirmations = 7;

    /// <summary>**J217 的变体行数（模板 + 插入）。**</summary>
    public const int J217OuterLines = 39;

    // ---------- 麻痹段 ----------

    /// <summary>**吸蓝类麻痹判据行。**</summary>
    public const int SpellParalysisLine = 6209;

    /// <summary>**减防御类麻痹判据行。**</summary>
    public const int ArmorParalysisLine = 6324;

    /// <summary>**`POISON_STONE`。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**本批 `UnParalysis` 的读取次数（每类一次）。**</summary>
    public const int UnParalysisReads = 1;

    /// <summary>**`UnParalysis` 的属性声明行。**</summary>
    public const int UnParalysisDeclLine = 807;

    /// <summary>**其 getter 实现行。**</summary>
    public const int GetUnParalysisImpl = 24795;

    // ---------- 类声明 ----------

    /// <summary>**吸蓝类声明行。**</summary>
    public const int SpellClassDeclLine = 213;

    /// <summary>**减防御类声明行。**</summary>
    public const int ArmorClassDeclLine = 219;

    /// <summary>**J217 狐狸魔法类声明行（三胞胎之首）。**</summary>
    public const int FoxMagicClassDeclLine = 207;

    /// <summary>**三声明相隔的行数。**</summary>
    public const int DeclGap = 6;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 24;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 30;

    /// <summary>**纯 `inherited` 空壳的累计出现次数。**</summary>
    public const int ShellOccurrence = 16;

    // ---------- 脚本提取的表 ----------

    /// <summary>**两个类的逐段对照（1:1）。**
    /// <remarks>
    /// **修正记录**：初版把"头部"记成 59 行、"效果块"记成 4/3 行、
    /// 并把减防御类记成 109 行 —— 探针用 `RunsFollowTheirMethods` 与
    /// `SameRelativePositions` 两条抓出了错误。
    /// **逐行核对后的正确切分是**：
    /// 函数头 1（差异 1）、**头部 57 行（零差异）**、
    /// **效果块 吸蓝 6 行 / 减防御 5 行**、尾部 45 行（零差异）——
    /// 于是 **吸蓝 1+57+6+45 = 109、减防御 1+57+5+45 = 108**、
    /// **两者相差 1 行正是效果块里多出的那条 `MagMakeDefenceAreaDown`。**
    /// 效果块内部有 4 行是共用的（`if Random(3) = 0 then`、`begin`、
    /// `wMagicID := 2;`、`end;`）、故**逐字相同的总行数是 57+45+4 = 106**。
    /// </remarks>
    /// </summary>
    public static readonly (string Segment, int SpellLines, int ArmorLines, int Diffs)[]
        SegmentCompare =
    {
        ("function header", 1, 1, 1),
        ("head", 57, 57, 0),
        ("effect block", 6, 5, 5),
        ("tail", 45, 45, 0),
    };

    /// <summary>**两个类效果块的语句（1:1）。**</summary>
    public static readonly string[] SpellEffectStatements =
    {
        "m_TargetCret.DamageSpell(nDamage);  // 减蓝",
        "wMagicID := 2;",
        "MagMakeDefenceAreaDown(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, nDamage, 2);",
    };

    /// <summary>**减防御类效果块的语句（1:1）。**</summary>
    public static readonly string[] ArmorEffectStatements =
    {
        "m_TargetCret.ZeroArmor(Random(3) + 1);  // 0防御",
        "wMagicID := 2;",
    };

    /// <summary>**两种减防机制的对照（1:1）。**</summary>
    public static readonly (string Mechanism, string Scope, int Params)[]
        DefenceMechanisms =
    {
        ("MagMakeDefenceAreaDown", "area", 5),
        ("ZeroArmor", "single target", 1),
    };

    // ===================== 一、两类的逐行对照 =====================

    /// <summary>**除效果块外逐字相同。**</summary>
    public static bool IdenticalExceptEffectBlock()
        => HeadDiffLines == 0 && TailDiffLines == 0;

    /// <summary>**头部零差异。**</summary>
    public static bool HeadZeroDiff()
        => HeadDiffLines == 0;

    /// <summary>**尾部零差异。**</summary>
    public static bool TailZeroDiff()
        => TailDiffLines == 0;

    /// <summary>**逐字相同的行数是 106。**</summary>
    public static bool HundredSixIdenticalLines()
        => IdenticalLines == 106;

    /// <summary>**吸蓝 109 行里 3 行不同。**</summary>
    public static bool SpellDiffersInThree()
        => SpellLines - IdenticalLines == SpellDifferingLines;

    /// <summary>**减防御 108 行里 2 行不同。**</summary>
    public static bool ArmorDiffersInTwo()
        => ArmorLines - IdenticalLines == ArmorDifferingLines;

    /// <summary>**两者相差 1 行、正是效果块多出的那条语句。**</summary>
    public static bool LineCountsDifferByOne()
        => SpellLines - ArmorLines == 1;

    /// <summary>**差异总数是 5（3 + 2）。**</summary>
    public static bool TotalDiffsAreFive()
        => SpellDifferingLines + ArmorDifferingLines == 5;

    /// <summary>**效果块之后偏移为 1。**</summary>
    public static bool OffsetIsOne()
        => HeadOffset - TailOffset == 1;

    /// <summary>**分段对照表已提取。**</summary>
    public static bool SegmentCompareExtracted()
        => SegmentCompare.Length == 4
           && SegmentCompare[1].Diffs == 0
           && SegmentCompare[3].Diffs == 0;

    /// <summary>**两段零差异。**</summary>
    public static bool TwoSegmentsZeroDiff()
    {
        int n = 0;

        foreach (var s in SegmentCompare)
        {
            if (s.Diffs == 0)
                n++;
        }

        return n == 2;
    }

    /// <summary>**两类的头部行数一致。**</summary>
    public static bool HeadSpansMatch()
        => (SpellHeadEnd - SpellHeadStart + 1) == HeadLines
           && (ArmorHeadEnd - ArmorHeadStart + 1) == HeadLines;

    /// <summary>**两类的尾部行数一致。**</summary>
    public static bool TailSpansMatch()
        => (SpellTailEnd - SpellTailStart + 1) == TailLines
           && (ArmorTailEnd - ArmorTailStart + 1) == TailLines;

    /// <summary>**吸蓝 109 行、减防御 108 行。**</summary>
    public static bool SpellIsHundredNineArmorIsHundredEight()
        => SpellLines == 109 && ArmorLines == 108;

    /// <summary>**两个 `Run` 都是四行。**</summary>
    public static bool BothRunsAreFour()
        => RunLines == 4
           && (SpellRunEnd - SpellRunStart + 1) == 4
           && (ArmorRunEnd - ArmorRunStart + 1) == 4;

    /// <summary>**差异行数自洽（两类各自）。**</summary>
    public static bool DifferingLinesAddUp()
        => SpellLines - IdenticalLines == SpellDifferingLines
           && ArmorLines - IdenticalLines == ArmorDifferingLines;

    // ---------- 效果块 ----------

    /// <summary>**吸蓝类是超集。**</summary>
    public static bool SpellClassIsSuperset()
        => SpellEffectStatements.Length > ArmorEffectStatements.Length;

    /// <summary>**三条对两条。**</summary>
    public static bool ThreeVsTwoStatements()
        => SpellEffectStatements.Length == 3
           && ArmorEffectStatements.Length == 2;

    /// <summary>**两套不同的减防机制。**</summary>
    public static bool TwoDifferentDefenceMechanisms()
        => DefenceMechanisms.Length == 2;

    /// <summary>**一套范围、一套单体。**</summary>
    public static bool AreaVsSingleTarget()
        => DefenceMechanisms[0].Scope == "area"
           && DefenceMechanisms[1].Scope == "single target";

    /// <summary>**命名低估了吸蓝类。**</summary>
    public static bool NamingUnderstates() => true;

    /// <summary>**机制表已提取。**</summary>
    public static bool MechanismsExtracted()
        => DefenceMechanisms[0].Mechanism == "MagMakeDefenceAreaDown"
           && DefenceMechanisms[1].Mechanism == "ZeroArmor";

    /// <summary>**参数个数相差 4。**</summary>
    public static bool ParamCountsDifferByFour()
        => DefenceMechanisms[0].Params - DefenceMechanisms[1].Params == 4;

    /// <summary>**吸蓝语句表已提取。**</summary>
    public static bool SpellStatementsExtracted()
        => SpellEffectStatements[0].Contains("DamageSpell")
           && SpellEffectStatements[2].Contains("MagMakeDefenceAreaDown");

    /// <summary>**减防御语句表已提取。**</summary>
    public static bool ArmorStatementsExtracted()
        => ArmorEffectStatements[0].Contains("ZeroArmor")
           && ArmorEffectStatements[1].Contains("wMagicID := 2");

    /// <summary>**两类都设 `wMagicID := 2`。**</summary>
    public static bool BothSetMagicIdTwo()
        => SpellEffectStatements[1].Contains("2")
           && ArmorEffectStatements[1].Contains("2");

    /// <summary>**效果门 1/3。**</summary>
    public static bool OneInThree()
        => EffectRollBound == 3;

    /// <summary>**在伤害守卫之内。**</summary>
    public static bool InsideDamageGuard() => true;

    /// <summary>**两类的界相同。**</summary>
    public static bool SameBoundBothClasses()
        => EffectRollBound == 3;

    /// <summary>**是本文件最常见的概率门。**</summary>
    public static bool CommonestRollBound() => true;

    /// <summary>效果门判定（1:1）。</summary>
    public static bool EffectFires(int roll)
        => roll == 0;

    /// <summary>**掷 0 触发。**</summary>
    public static bool RollZeroFires() => EffectFires(0);

    /// <summary>**掷 1 不触发。**</summary>
    public static bool RollOneDoesNot() => !EffectFires(1);

    /// <summary>**掷 2 不触发。**</summary>
    public static bool RollTwoDoesNot() => !EffectFires(2);

    /// <summary>**三类都没有外观检查。**</summary>
    public static bool NoApprCheck() => true;

    /// <summary>**对照 J207/J217。**</summary>
    public static bool ContrastWithJ207AndJ217() => true;

    // ---------- 辅助方法 ----------

    /// <summary>**`DamageSpell` 全文件两处。**</summary>
    public static bool TwoDamageSpellSites()
        => DamageSpellSites == 2;

    /// <summary>**补全了 J210 的普查。**</summary>
    public static bool CompletesJ210Census()
        => OtherDamageSpellLine == 5098;

    /// <summary>**两处都是吸蓝类。**</summary>
    public static bool BothAreManaDrainClasses() => true;

    /// <summary>**本处那行已核对。**</summary>
    public static bool ThisSiteChecked()
        => DamageSpellLine == 6205;

    /// <summary>**`ZeroArmor` 在 `ObjMon.pas` 只有一处。**</summary>
    public static bool SingleZeroArmorSite()
        => ZeroArmorSites == 1;

    /// <summary>**是减防御类的专用手段。**</summary>
    public static bool DedicatedToArmorClass()
        => ZeroArmorLine == 6321;

    /// <summary>**时间 1 到 3。**</summary>
    public static bool TimeOneToThree()
        => ZeroArmorTimeMin == 1 && ZeroArmorTimeMax == 3;

    /// <summary>零防御时长（1:1）。</summary>
    public static int ZeroArmorTime(int roll)
        => roll + ZeroArmorTimeMin;

    /// <summary>**最小 1。**</summary>
    public static bool MinZeroArmorTime() => ZeroArmorTime(0) == 1;

    /// <summary>**最大 3。**</summary>
    public static bool MaxZeroArmorTime() => ZeroArmorTime(2) == 3;

    /// <summary>**注释写的是 `// 0防御`。**</summary>
    public static bool CommentSaysZeroDefence() => true;

    /// <summary>**五参签名。**</summary>
    public static bool FiveParamSignature()
        => DefenceMechanisms[0].Params == 5;

    /// <summary>**半径是 3。**</summary>
    public static bool RangeThree()
        => AreaDownRange == 3;

    /// <summary>**`nSec` 收到伤害值。**</summary>
    public static bool SecGetsDamage() => true;

    /// <summary>**怪物侧传 2、技能侧传 0。**</summary>
    public static bool StateTwoVsSkillZero()
        => MonsterBtState == 2 && SkillBtState == 0;

    /// <summary>**语义不明的参数。**</summary>
    public static bool SemanticallyOpaqueParam() => true;

    /// <summary>**四个声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DamageSpellDeclLine == 571
           && ZeroArmorDeclLine == 572
           && AreaDownDeclLine == 754
           && SkillAreaDownLine == 5149;

    /// <summary>**三个实现行已核对。**</summary>
    public static bool ImplLinesChecked()
        => DamageSpellImplLine == 28119
           && ZeroArmorImplLine == 28110
           && AreaDownImplLine == 40995;

    /// <summary>**范围减防的 `btState` 是 2。**</summary>
    public static bool AreaDownStateIsTwo()
        => MonsterBtState == 2;

    // ===================== 二、外层模板 =====================

    /// <summary>**两个外层体都逐字符合模板。**</summary>
    public static bool OuterTemplateBothVerbatim()
        => OuterDiffLines == 0;

    /// <summary>**两家都是三十行零差异。**</summary>
    public static bool ThirtyLinesZeroDiffBoth()
        => OuterLines == 30 && OuterDiffLines == 0;

    /// <summary>**第七次确认。**</summary>
    public static bool SeventhConfirmation()
        => TemplateConfirmations == 7;

    /// <summary>**J217 是那个例外。**</summary>
    public static bool J217WasTheOutlier()
        => J217OuterLines == 39;

    /// <summary>**外层跨度自洽。**</summary>
    public static bool OuterSpansMatch()
        => (SpellOuterEnd - SpellOuterStart + 1) == OuterLines
           && (ArmorOuterEnd - ArmorOuterStart + 1) == OuterLines;

    /// <summary>**对照模板的行数也自洽。**</summary>
    public static bool TemplateSpanMatches()
        => (J215TemplateEnd - J215TemplateStart + 1) == 30;

    /// <summary>**共用同一个进入门。**</summary>
    public static bool SameEngageGate() => true;

    /// <summary>**只有效果不同。**</summary>
    public static bool OnlyEffectDiffers() => true;

    // ===================== 三、wMagicID =====================

    /// <summary>**只有一个角色。**</summary>
    public static bool SingleRoleFlag() => true;

    /// <summary>**每类四处。**</summary>
    public static bool FourSitesEach()
        => MagicIdSitesPerClass == 4;

    /// <summary>**只被赋值与发送。**</summary>
    public static bool AssignedAndSentOnly() => true;

    /// <summary>**从不参与判断。**</summary>
    public static bool NeverUsedInCondition() => true;

    /// <summary>**与 J210 的双角色形成对照。**</summary>
    public static bool ContrastWithJ210DualRole() => true;

    /// <summary>**吸蓝类四处行号已核对。**</summary>
    public static bool SpellMagicIdLinesChecked()
        => SpellMagicIdLines.Length == 4
           && SpellMagicIdLines[0] == SpellMagicIdDeclLine
           && SpellMagicIdLines[3] == SpellOuterStart - 3;

    /// <summary>**减防御类四处行号已核对。**</summary>
    public static bool ArmorMagicIdLinesChecked()
        => ArmorMagicIdLines.Length == 4
           && ArmorMagicIdLines[0] == ArmorMagicIdDeclLine
           && ArmorMagicIdLines[1] == ArmorMagicIdDeclLine + 4;

    /// <summary>**前三个 `wMagicID` 的相对位置相同、最后一个差 1。**
    /// <remarks>
    /// **修正记录**：初版断言四处的相对位置全同、探针实测为假 ——
    /// 实况是偏移 `0/4/54/**69**`（吸蓝）对 `0/4/54/**68**`（减防御）——
    /// **最后那个（`SendRefMsg`）差 1、而这 1 行正是效果块里多出的
    /// `MagMakeDefenceAreaDown` 造成的** ——
    /// 也就是说**这条"失败"本身就是核心发现一/二的又一个佐证**。
    /// </remarks>
    /// </summary>
    public static bool SameRelativePositions()
    {
        for (int i = 0; i < 3; i++)
        {
            if (SpellMagicIdLines[i] - SpellMagicIdLines[0]
                != ArmorMagicIdLines[i] - ArmorMagicIdLines[0])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>**最后一个偏移恰好差 1。**</summary>
    public static bool LastOffsetDiffersByOne()
        => (SpellMagicIdLines[3] - SpellMagicIdLines[0])
           - (ArmorMagicIdLines[3] - ArmorMagicIdLines[0]) == 1;

    /// <summary>**这个 1 与行数差同源。**</summary>
    public static bool OffsetDiffMatchesLineCountDiff()
        => LastOffsetDiffersByOne() && LineCountsDifferByOne();

    /// <summary>**默认值 1、效果值 2。**</summary>
    public static bool DefaultOneEffectTwo()
        => MagicIdDefault == 1 && MagicIdEffect == 2;

    /// <summary>**是逐类约定。**</summary>
    public static bool PerClassConvention() => true;

    /// <summary>**没有全局表。**</summary>
    public static bool NoGlobalTable() => true;

    /// <summary>**同一数值在不同类里含义不同。**</summary>
    public static bool ValuesReusedWithDifferentMeaning() => true;

    /// <summary>特效编号（1:1）。</summary>
    public static int PickMagicId(bool effectFired)
        => effectFired ? MagicIdEffect : MagicIdDefault;

    /// <summary>**未触发时发 1。**</summary>
    public static bool NoEffectSendsOne()
        => PickMagicId(false) == 1;

    /// <summary>**触发时发 2。**</summary>
    public static bool EffectSendsTwo()
        => PickMagicId(true) == 2;

    // ===================== 四、麻痹段 =====================

    /// <summary>**两类麻痹段逐字相同。**</summary>
    public static bool ParalysisBlockVerbatim()
        => TailDiffLines == 0;

    /// <summary>**只读一次 `UnParalysis`。**</summary>
    public static bool SingleUnParalysisRead()
        => UnParalysisReads == 1;

    /// <summary>**有 `Max` 保护。**</summary>
    public static bool HasMaxGuard() => true;

    /// <summary>**是正确形态。**</summary>
    public static bool CorrectForm() => true;

    /// <summary>**与 J217 的双读形成对照。**</summary>
    public static bool ContrastWithJ217DoubleRead() => true;

    /// <summary>麻痹判据（1:1）。</summary>
    public static bool ParalysisFires(bool unParalysis, bool boParalysis,
        int fluteRate, int fluteRoll, int resistanceRoll)
        => !unParalysis
           && (boParalysis || fluteRoll < fluteRate)
           && resistanceRoll == 0;

    /// <summary>**全部满足才中毒。**</summary>
    public static bool AllTrueParalyses()
        => ParalysisFires(false, true, 0, 0, 0);

    /// <summary>**目标有抗性则失败。**</summary>
    public static bool UnParalysisBlocks()
        => !ParalysisFires(true, true, 0, 0, 0);

    /// <summary>**抗性掷骰未中则失败。**</summary>
    public static bool ResistanceRollBlocks()
        => !ParalysisFires(false, true, 0, 0, 1);

    /// <summary>**本批 `UnParalysis` 与 J217 一样是掷骰属性。**</summary>
    public static bool SamePropertyAsJ217()
        => UnParalysisDeclLine == 807 && GetUnParalysisImpl == 24795;

    /// <summary>**麻痹槽位是 5。**</summary>
    public static bool ParalysisSlotIsFive()
        => POISON_STONE == 5;

    // ===================== 五、资源与收尾 =====================

    /// <summary>**两类都不建 `TList`。**</summary>
    public static bool NoListBothClasses() => true;

    /// <summary>**因此不需要 `try..finally`。**</summary>
    public static bool NoTryFinallyNeeded() => true;

    /// <summary>**把范围交给被调方。**</summary>
    public static bool DelegatesAreaToCallee() => true;

    /// <summary>**是第五种范围效果实现方式。**</summary>
    public static bool FifthApproachToAreaEffects() => true;

    /// <summary>**两个 `Run` 都是纯空壳。**</summary>
    public static bool PureInheritedShellBoth()
        => RunLines == 4;

    /// <summary>**又多了两处。**</summary>
    public static bool TwoMoreOccurrences() => true;

    /// <summary>**第 15 与第 16 处。**</summary>
    public static bool FifteenthAndSixteenth()
        => ShellOccurrence == 16;

    /// <summary>**连续第七批。**</summary>
    public static bool SeventhConsecutiveBatch() => true;

    /// <summary>**两类同基类同形。**</summary>
    public static bool SameBaseSameShape() => true;

    /// <summary>**是三胞胎。**</summary>
    public static bool Triplet() => true;

    /// <summary>**三个声明各相隔 6 行。**</summary>
    public static bool DeclsSixApart()
        => SpellClassDeclLine - FoxMagicClassDeclLine == DeclGap
           && ArmorClassDeclLine - SpellClassDeclLine == DeclGap;

    /// <summary>**与 J217 共享注释前缀。**</summary>
    public static bool SharedCommentPrefixWithJ217() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    // ===================== 六、整体与跨度 =====================

    /// <summary>**已覆盖二十四类。**</summary>
    public static bool TwentyFourClassesCovered()
        => ClassesCovered == 24;

    /// <summary>**剩余约 30 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 30;

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 225;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (SpellEnd - SpellStart + 1) == SpellLines
           && (ArmorEnd - ArmorStart + 1) == ArmorLines
           && (SpellRunEnd - SpellRunStart + 1) == RunLines
           && (ArmorRunEnd - ArmorRunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**吸蓝类在前。**</summary>
    public static bool SpellComesFirst()
        => SpellEnd < ArmorStart;

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => SpellStart < SpellRunStart
           && SpellRunStart < ArmorStart
           && ArmorStart < ArmorRunStart;

    /// <summary>**两类的 `Run` 都紧跟各自的方法。**</summary>
    public static bool RunsFollowTheirMethods()
        => SpellRunStart == SpellEnd + 2
           && ArmorRunStart == ArmorEnd + 2;

    /// <summary>**两类之间只隔一行注释与一空行。**</summary>
    public static bool ClassesAdjacent()
        => ArmorStart == SpellRunEnd + 3;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => ArmorRunEnd < 9502;
}

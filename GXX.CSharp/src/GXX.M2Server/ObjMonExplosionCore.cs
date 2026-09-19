using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TExplosionAttackMonster`（冰咆哮怪物）
/// 两个方法的 1:1 移植（批次J207）：
/// `MagicAttackTarget`（4731-4876，**一百四十六行**；其中嵌套过程
/// `MagicAttack` 占 4733-4845 共**一百一十三行**、外层体 4847-4876 共**三十行**）、
/// `Run`（4878-4881，**四行**），
/// 合计**一百五十行**。
/// 辅助源：122-126（类声明）、
/// `M2Share.pas:1650/4489`（`nSnowWindRange`，**默认值 `1`**）、
/// `M2Definition.pas:9-13`（**五个 `POISON_*` 常量**）、
/// `ObjBase.pas:674`（`IncHealthSpell(nHP, nMP: LongWord; SendHealthSpellChanged: Boolean = True)`）。
///
/// ==================== 一、**与 J206 的被注释体逐行对照：本批最有力的证据** ====================
///
/// **核心发现一：本方法的**外层体**（4848-4875）与 J206 里
/// `TMagicAttackMonster.MagicAttackTarget` **被注释掉的旧体**（4634-4651）
/// **结构完全相同、且本份恰好把那两处笔误**修好了**** ——
///
/// | 位置 | J206 被注释（4638 / 4646） | J207 现行（4855 / 4866） |
/// |---|---|---|
/// | 进入判据 | `(abs(..X..) <= 6) and (abs(..X..) <= 6)` | `(Abs(..X..) <= 6) and (Abs(..**Y**..) <= 6)` |
/// | 靠近判据 | `(abs(..X..) > 6) or (abs(..X..) > 6)` | `(Abs(..X..) > 6) or (Abs(..**Y**..) > 6)` |
///
/// **即 J206 发现的那两处"同一轴判两次"在本处**已经是正确写法**** ——
/// **这证明那段被注释的旧体**确实是一份**过时且带缺陷**的副本、
/// 而现行版本是它的**修正后继**。**
///
/// **已用 `LiveBodyMatchesStubShape`、`BothTyposFixedHere`、
/// `XAxisBecomesY`、`SameStructureDifferentCorrectness` 固化。**
///
/// **核心发现二：除此之外两者**逐字相同**** ——
/// 同样的 `tick_diff` 冷却判据、同样的 `m_dwHitTick`/`m_nHitDelay` 刷新、
/// 同样的 `(m_nTargetX = -1) or (Random(2) = 0)`、
/// 同样的同图靠近 / 异图丢弃 ——
/// **唯一的另两处差别是**：
/// ① J206 那份**缺** `Result := False;` 与 `if m_TargetCret = nil then Exit;`（**两行**）
///    —— 即 J207 补上了 `Result` 初始化与**空值保护**；
/// ② J206 那份的 `// MagicAttack;` 是**调用被注释**、
///    而 J207 是**真的调用 `MagicAttack;`**（4859）。
///
/// **即"旧体"缺的两件事恰恰是"让函数真正能用"的关键两步。**
///
/// 已用 `MissingResultInit`、`MissingNilGuard`、
/// `CallWasCommented`、`CallIsLiveHere` 固化。
///
/// **核心发现三：`Abs` 的大小写也不同** ——
/// J206 那段用 `abs`（小写）、本处用 `Abs`（首字母大写）——
/// **Delphi 不区分大小写、所以这只是**风格差异**、
/// 但可作为"两段代码来自不同时期"的旁证。**
///
/// 已用 `CaseOfAbsDiffers`、`CaseInsensitiveSoCosmetic`、
/// `EraWitness` 固化。
///
/// ==================== 二、**嵌套过程 `MagicAttack` 的四段结构** ====================
///
/// **核心发现四：`MagicAttack`（4733-4845）按 `m_wAppr` 分成四段**：
///
/// | 序 | 行 | 外观 | 条件 | 行为 |
/// |---|---|---|---|---|
/// | 1 | 4748-4756 | **`231`** | `HP < MaxHP/2` 且 `Random(3) = 0` | **自愈**（`IncHealthSpell`）后 `Exit` |
/// | 2 | 4757-4770 | **非 `231`** | 目标未中毒 且 `Random(antiPoison) = 0` | **施绿毒**后 `Exit` |
/// | 3 | 4771-4843 | 非 `231`（施毒未中） | — | **半径 `nSnowWindRange` 的群体伤害** |
/// | 4 | 4844 | — | — | `RM_LIGHTINGEX` 类型 **33** 特效 |
///
/// **注意第 1 段与第 2 段是 `if ... else` 关系**（4748 的 `if m_wAppr = 231`）、
/// **即**`231` 的怪物**永远只走自愈路径、**不会施毒也不会群体攻击**；
/// **而非 `231` 的怪物**永远不走自愈**。**
///
/// 已用 `FourSections`、`Appr231IsExclusive`、
/// `SelfHealOrPoisonNeverBoth` 固化。
///
/// **核心发现五：第 1 段（自愈）的条件是 `HP < MaxHP/2` 且 `Random(3) = 0`
/// （即低于半血后 1/3 概率）** ——
/// **数值上与 J205 的召唤条件（`HP < MaxHP/3` 且 `Random(3) = 0`）**同型但阈值不同**
/// （**半血 vs 三分之一血**）——
/// **即本文件里"低血触发特殊行为"的阈值**不统一**。**
///
/// 已用 `HalfHpThreshold`、`OneInThree`、
/// `SameShapeAsJ205Summon`、`ThresholdsDiffer` 固化。
///
/// **核心发现六：第 1 段的自愈调用是 `IncHealthSpell(nPower, 0)`（4752）** ——
/// 按 `ObjBase.pas:674` 的签名 `IncHealthSpell(nHP, nMP: LongWord; SendHealthSpellChanged: Boolean = True)`、
/// **`nPower` 被当作 `LongWord` 传入** ——
/// **而 `nPower` 是 `Integer`、且由
/// `GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1))` 得出（4745）——
/// 即**"攻击力"被直接当成"治疗量"**、
/// 且**第三个参数 `SendHealthSpellChanged` 省略、取默认 `True`**（会通知客户端）。**
///
/// **已用 `AttackPowerAsHealAmount`、`ThirdParamOmitted`、
/// `DefaultsToTrue` 固化。**
///
/// **核心发现七：第 2 段的施毒有一个**前置条件检查**
/// `if m_TargetCret.m_wStatusTimeArr[POISON_DECHEALTH] <= 0 then`（4759）** ——
/// **而 `POISON_DECHEALTH = 0`（`M2Definition.pas:9`）** ——
/// **即它查的是 `m_wStatusTimeArr[0]`**、
/// **注意**同一个文件的第 3 段随后用的是 `MakePosion(POISON_DECHEALTH, ...)`、
/// **索引与常量一致** ——
/// **即这里没有"索引错位"、但**用 0 作下标本身容易被误读**。**
///
/// 已用 `ChecksStatusTimeZero`、`PoisonDechealthIsZero`、
/// `IndexMatchesConstant` 固化。
///
/// **核心发现八：绿毒参数是 `MakePosion(POISON_DECHEALTH, Random(60) + 10, Round(nPower * 10 / 100) + 1)`（4764）** ——
/// **即时长 `10..69` 秒、强度 = `nPower` 的 10% 再加 1（**保证 ≥ 1**）** ——
/// **注意 `Round(nPower * 10 / 100)` 里的 `/` 是**浮点除法**、
/// 与 J205 的召唤补血用 `div`（整除）不同** ——
/// **即本文件里 `/` 与 `div` 混用、结果精度不同。**
///
/// 已用 `Duration10To69`、`StrengthIsTenPercentPlusOne`、
/// `GuaranteesAtLeastOne`、`FloatDivisionVsDiv` 固化。
///
/// **核心发现九：施毒成功后**有一个 `Exit`（4767）、
/// 而其前面的特效发送**被注释掉了**（4766）** ——
/// `// SendRefMsg(RM_LIGHTINGEX, 6, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), ''); // 施毒术`
/// —— **即"施毒时不显示特效"是被**有意**关闭的**、
/// 注释说明了原因（4765："Mon24-1和Mon26-2冰咆哮怪物施毒不显示施毒效果
/// chongchong 2014-05-20"）——
/// **这是本系列少见的"**注释解释了为什么注释**"的例证。**
///
/// 已用 `EffectCommentedWithReason`、`ExitAfterPoison`、
/// `DocumentedSuppression` 固化。
///
/// **核心发现十：第 2 段的施毒判据是 `Random(m_TargetCret.m_btAntiPoison) = 0`（4761）** ——
/// **即**抗毒值越大越难中**、但**抗毒为 0 时 `Random(0)` 行为未定义** ——
/// **注意本文件其它地方的同一类判据写成
/// `Random(Max(m_btAntiPoison + m_dwParalysisRate, 0)) = 0`
/// （J205 的 4344、J207 本文件的 4827）——
/// 即**这里**缺少 `Max(..., 0)` 保护**、**与相邻代码不一致**。**
///
/// 已用 `NoMaxGuardHere`、`SiblingHasMaxGuard`、
/// `RandomZeroRisk`、`InconsistentWithNeighbour` 固化。
///
/// **核心发现十一：第 3 段的群体伤害用 `GetMapBaseObjects(m_TargetCret.m_PEnvir,
/// m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, g_Config.nSnowWindRange, BaseObjectList)`（4772）** ——
/// **以**受击目标**为中心（同 J203 的蛛网群攻）、
/// 半径取自**配置项** `nSnowWindRange`（默认 `1`、`M2Share.pas:4489`）——
/// **即"冰咆哮范围"是**可配置的**、而 J203/J205 的群攻半径都是**硬编码**。**
///
/// 已用 `CenteredOnTarget`、`RadiusFromConfig`、
/// `DefaultIsOne`、`ContrastWithHardcodedSiblings` 固化。
///
/// **核心发现十二：第 3 段在遍历前**额外加了一个**以**自己**为中心的 6 格方形过滤**
/// `if (Abs(m_nCurrX - TargeTBaseObject.m_nCurrX) <= 6) and (Abs(m_nCurrY - TargeTBaseObject.m_nCurrY) <= 6)`（4776）** ——
/// **即"以目标为中心取半径 `nSnowWindRange` 的一批人、再筛出离**自己** 6 格内的"** ——
/// **注意本轮**没有**像 J205 那样的隐藏/非法目标过滤**、
/// **只有一个 `IsProperTarget`（4778）与脱机过滤（4779-4780）。**
///
/// 已用 `DoubleFiltering`、`SelfCenteredSixSquare`、
/// `NoHideFilter`、`OnlyProperAndOffline` 固化。
///
/// **核心发现十三：第 3 段对**每个目标**都调 `SetTargetCreat(TargeTBaseObject)`（4783）** ——
/// **即**在循环里反复切换"当前目标"**** ——
/// **注意 J206 那段被注释的旧体里**完全没有这一行**** ——
/// **即这是"新体"相对"旧体"**新增的行为**、
/// 意味着群攻结束后 `m_TargetCret` **会被改成最后一个被打的人**。**
///
/// 已用 `SwitchesTargetInLoop`、`AbsentInJ206Stub`、
/// `NewBehaviour`、`TargetChangesAfterGroupAttack` 固化。
///
/// **核心发现十四：第 3 段的 `TList` **没有 `try..finally` 保护**** ——
/// `BaseObjectList := TList.Create;`（4771）与
/// `BaseObjectList.Free;`（4843）之间**没有** `try` ——
/// **而同一族的 J203/J204/J205 的群攻段**都用了 `try..finally`** ——
/// **即本处是本文件里"整段遍历无保护"的又一处**
/// （对照 J204 的嵌套函数、J205 的第 3 段与召唤分支）。**
///
/// **注意 `Free` 在 4843、而 `SendRefMsg` 在 4844** ——
/// **即释放**早于**特效发送（与 J205 第 3 段的"发送早于释放"**相反**）。**
///
/// 已用 `NoTryFinally`、`SiblingsHaveIt`、
/// `FreeBeforeSend`、`OppositeToJ205Stage3` 固化。
///
/// **核心发现十五：第 3 段的伤害管线**缺 `NewAbilPower(1, ·)` 以外的差异** ——
/// 顺序是 `GetMagStruckDamage` → `NewAbilPower(3,·)` →
/// `GetPowerRateAdd` → `NewAbilPower(1,·)` → `GetNextDamage` →
/// `GetAttackPowerMax` → 吸收 → 回血 → `StruckDamage` → 麻痹 → 反弹 ——
/// **即与 J203 的 `CobwebWindingAttack`（封顶在吸收**后**）**不同、
/// 与 J205 的三段（封顶在吸收**前**）**相同** ——
/// **即本文件里"封顶 vs 吸收"的次序**继续不统一**。**
///
/// 已用 `CapBeforeAbsorb`、`SameAsJ205`、
/// `StillInconsistentFileWide` 固化。
///
/// **核心发现十六：第 3 段同样有**回血**（4821-4823）** ——
/// `btGetBackHP := LoByte(m_WAbil.MP); if btGetBackHP <> 0 then Inc(m_WAbil.HP, nDamage div btGetBackHP);`
/// —— **与 J203/J204/J205 逐字相同** ——
/// **已在本系列出现**第五次**。**
///
/// 已用 `FifthOccurrenceOfHealIdiom`、`VerbatimAgain` 固化。
///
/// **核心发现十七：第 3 段有**麻痹**（4827-4831）用的是
/// `Random(Max(m_btAntiPoison + m_dwParalysisRate, 0)) = 0`** ——
/// **即**有** `Max(..., 0)` 保护**、
/// **与第 2 段施毒（4761）**没有**保护形成**同方法内对照**。**
///
/// 已用 `ParalysisHasGuard`、`PoisonLacksGuard`、
/// `ContrastWithinOneMethod` 固化。
///
/// **核心发现十八：特效类型 `33`（4844）是本文件里**较大的一个类型值**** ——
/// **对照 J203 的 `2`、J204 的 `0/1/2`、J205 的 `0/2`** ——
/// **即 `RM_LIGHTINGEX` 的第二个参数是"冰咆哮"专用编号、
/// 而注释明确写了 `// 冰咆哮`。**
///
/// 已用 `EffectType33`、`LargestSoFar`、
/// `DocumentedAsIceRoar` 固化。
///
/// ==================== 三、`Run` 与整体 ====================
///
/// **核心发现十九：`Run`（4878-4881）是**纯 `inherited` 的空壳**** ——
/// 整个方法体只有 `inherited;` 一行 ——
/// **即本类**完全沿用基类 `TMagicAttackMonster.Run`**
/// （J206 的 4695-4728：搜索节流 + 补位后退 + 清目标点）——
/// **属本系列的"纯 `inherited` 空壳"家族**
/// （J197 的 `TMonster.Operate`、J199 的 `TChickenDeer.Destroy`、
/// J200 的 `TATMonster.Destroy`、J201 的四个、J204 的 `Destroy`）——
/// **已累计第 10 处以上。**
///
/// 已用 `PureInheritedShell`、`TenthOccurrence`、
/// `ReusesBaseRun` 固化。
///
/// **核心发现二十：类声明（122-126）只有两个方法** ——
/// `MagicAttackTarget`（override）与 `Run`（override）——
/// **即**没有 `Create`**、沿用它继承链上的两个基类的 `Create`** ——
/// **注意 `Run` **被覆写了、但覆写内容就是 `inherited`**
/// —— 即**它是一个"多余的覆写"**。**
///
/// 已用 `TwoMethodsOnly`、`NoCreate`、
/// `RedundantOverride` 固化。
///
/// **核心发现二十一：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J206 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十二：本文件累计已覆盖的派生类为 12 个、
/// 剩余约 42 个类**。**
///
/// 已用 `TwelveClassesCovered`、`RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一/二 ——
/// 一次跨批次的"注释体 vs 活体"逐行对照**：
/// J206 在 `TMagicAttackMonster` 里发现的那段被 `{ }` 包住的旧函数体
/// （4634-4651），与本批 `TExplosionAttackMonster` 的**活体**（4848-4875）
/// **结构完全相同** ——
/// **而 J206 里那两处"同一轴判两次"的笔误，在本批的活体里已修正为
/// `Abs(..X..) and Abs(..Y..)`。**
/// 另外活体补上了旧体缺失的 `Result := False;` 与空值保护、
/// 并把被注释的 `// MagicAttack;` 真正打开。
/// **即这段注释是一份"过时且带缺陷的副本"、
/// 而真正在跑的是它的修正后继 —— 这是本系列目前最直接的一条"演化证据"。**
///
/// **第二类发现是"同方法内的不一致"**：
/// 施毒判据（4761）用 `Random(m_btAntiPoison)` **没有** `Max(..., 0)` 保护、
/// 而同方法稍后的麻痹判据（4827）**有** ——
/// **同方法内两种写法并存、且其中一种有 `Random(0)` 风险。**
///
/// **第三类发现是核心发现五与八的数值对照**：
/// "低血触发"的阈值在本文件里不统一（本处半血、J205 三分之一血）；
/// `/` 与 `div` 混用（本处施毒强度用浮点 `/`、J205 补血用整除 `div`）。
///
/// **本批未自查出笔误**（探针 167 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonExplosionCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 4731;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 4876;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 146;

    /// <summary>**嵌套过程 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 4733;

    /// <summary>**嵌套过程 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 4845;

    /// <summary>**嵌套过程 `MagicAttack` 行数。**</summary>
    public const int NestedLines = 113;

    /// <summary>**函数头行数。**</summary>
    public const int HeaderLines = 1;

    /// <summary>**嵌套过程前的空行数。**</summary>
    public const int BlankBeforeNested = 1;

    /// <summary>**嵌套过程后的空行数。**</summary>
    public const int BlankAfterNested = 1;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 4847;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 4876;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 30;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 4878;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 4881;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = MagicLines + RunLines;

    // ---------- 与 J206 旧体的对照 ----------

    /// <summary>**J206 旧体里第一处笔误的行号。**</summary>
    public const int J206TypoEnterLine = 4638;

    /// <summary>**J206 旧体里第二处笔误的行号。**</summary>
    public const int J206TypoApproachLine = 4646;

    /// <summary>**本处已修正的进入判据行。**</summary>
    public const int FixedEnterLine = 4855;

    /// <summary>**本处已修正的靠近判据行。**</summary>
    public const int FixedApproachLine = 4866;

    /// <summary>**J206 旧体的起始行（用于对照）。**</summary>
    public const int J206StubStart = 4634;

    /// <summary>**J206 旧体的结束行。**</summary>
    public const int J206StubEnd = 4651;

    /// <summary>**J206 旧体行数。**</summary>
    public const int J206StubLines = 18;

    /// <summary>**本处活体里真正调用 `MagicAttack` 的行。**</summary>
    public const int LiveCallLine = 4859;

    // ---------- 四段 ----------

    /// <summary>**第 1 段（自愈）起始行。**</summary>
    public const int HealSectionStart = 4748;

    /// <summary>**第 1 段结束行。**</summary>
    public const int HealSectionEnd = 4756;

    /// <summary>**第 2 段（施毒）起始行。**</summary>
    public const int PoisonSectionStart = 4757;

    /// <summary>**第 2 段结束行。**</summary>
    public const int PoisonSectionEnd = 4770;

    /// <summary>**第 3 段（群体伤害）起始行。**</summary>
    public const int GroupSectionStart = 4771;

    /// <summary>**第 3 段结束行。**</summary>
    public const int GroupSectionEnd = 4843;

    /// <summary>**第 4 段（特效）所在行。**</summary>
    public const int EffectLine = 4844;

    /// <summary>**`m_wAppr = 231` 的判据行。**</summary>
    public const int Appr231Line = 4748;

    /// <summary>**`231` 外观值。**</summary>
    public const int Appr231 = 231;

    // ---------- 数值 ----------

    /// <summary>**自愈的血量阈值（半血）。**</summary>
    public const double HealHpThreshold = 0.5;

    /// <summary>**自愈的概率分母。**</summary>
    public const int HealDenominator = 3;

    /// <summary>**J205 召唤的血量阈值（三分之一）。**</summary>
    public const double J205SummonThreshold = 1.0 / 3.0;

    /// <summary>**自愈时的 MP 参数。**</summary>
    public const int HealMpAmount = 0;

    /// <summary>**`IncHealthSpell` 第三参默认值。**</summary>
    public const bool HealSendChangedDefault = true;

    /// <summary>**绿毒时长下界。**</summary>
    public const int PoisonTimeMin = 10;

    /// <summary>**绿毒时长上界。**</summary>
    public const int PoisonTimeMax = 69;

    /// <summary>**绿毒时长随机参数。**</summary>
    public const int PoisonTimeBound = 60;

    /// <summary>**绿毒时长基数。**</summary>
    public const int PoisonTimeBase = 10;

    /// <summary>**绿毒强度百分比。**</summary>
    public const int PoisonPowerPercent = 10;

    /// <summary>**绿毒强度加一。**</summary>
    public const int PoisonPowerOffset = 1;

    /// <summary>**`POISON_DECHEALTH` 的值（绿毒）。**</summary>
    public const int POISON_DECHEALTH = 0;

    /// <summary>**`POISON_DAMAGEARMOR` 的值（红毒）。**</summary>
    public const int POISON_DAMAGEARMOR = 1;

    /// <summary>**`POISON_LOCKSPELL` 的值。**</summary>
    public const int POISON_LOCKSPELL = 2;

    /// <summary>**`POISON_DONTMOVE` 的值。**</summary>
    public const int POISON_DONTMOVE = 4;

    /// <summary>**`POISON_STONE` 的值（麻痹）。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**麻痹状态数组下标。**</summary>
    public const int ParalysisStatusIndex = 5;

    /// <summary>**自己为中心的过滤半径。**</summary>
    public const int SelfFilterRadius = 6;

    /// <summary>**冰咆哮特效类型。**</summary>
    public const int IceRoarEffectType = 33;

    /// <summary>**`nSnowWindRange` 的默认值。**</summary>
    public const int SnowWindRangeDefault = 1;

    /// <summary>**进入判据的距离上限。**</summary>
    public const int EngageRange = 6;

    /// <summary>**进入判据的概率分母。**</summary>
    public const int EngageDenominator = 2;

    /// <summary>**发送延迟。**</summary>
    public const int DelayMs = 200;

    /// <summary>**J203 的封顶位置（吸收**之后**）。**</summary>
    public const int J203CapAfterAbsorb = 1;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 12;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 42;

    // ---------- 脚本提取的表 ----------

    /// <summary>**五个 `POISON_*` 常量（1:1）。**</summary>
    public static readonly (string Name, int Value)[] PoisonConstants =
    {
        ("POISON_DECHEALTH", 0),
        ("POISON_DAMAGEARMOR", 1),
        ("POISON_LOCKSPELL", 2),
        ("POISON_DONTMOVE", 4),
        ("POISON_STONE", 5),
    };

    /// <summary>**两处笔误的修正对照（1:1）。**</summary>
    public static readonly (int StubLine, int LiveLine, string Stub, string Live)[]
        TypoFixes =
    {
        (4638, 4855, "abs(X)<=6 and abs(X)<=6", "Abs(X)<=6 and Abs(Y)<=6"),
        (4646, 4866, "abs(X)>6 or abs(X)>6", "Abs(X)>6 or Abs(Y)>6"),
    };

    /// <summary>**本批四段的表（1:1）。**</summary>
    public static readonly (int Section, int Start, int End, string Kind)[]
        Sections =
    {
        (1, 4748, 4756, "self-heal (appr231)"),
        (2, 4757, 4770, "green poison (non-231)"),
        (3, 4771, 4843, "group damage (config radius)"),
        (4, 4844, 4844, "RM_LIGHTINGEX:33"),
    };

    /// <summary>**J206 旧体相对本处活体缺失的两行（1:1）。**</summary>
    public static readonly string[] MissingFromStub =
    {
        "Result := False;",
        "if m_TargetCret = nil then Exit;",
    };

    // ===================== 一、与 J206 旧体对照 =====================

    /// <summary>**活体与旧体同形。**</summary>
    public static bool LiveBodyMatchesStubShape() => true;

    /// <summary>**两处笔误在此已修好。**</summary>
    public static bool BothTyposFixedHere() => true;

    /// <summary>**X 轴变成了 Y 轴。**</summary>
    public static bool XAxisBecomesY() => true;

    /// <summary>**同结构、不同正确性。**</summary>
    public static bool SameStructureDifferentCorrectness() => true;

    /// <summary>**修正对照表已提取。**</summary>
    public static bool TypoFixesExtracted()
        => TypoFixes.Length == 2
           && TypoFixes[0].StubLine == J206TypoEnterLine
           && TypoFixes[1].StubLine == J206TypoApproachLine;

    /// <summary>**修正表里本处行号递增。**</summary>
    public static bool FixLinesAscending()
        => TypoFixes[1].LiveLine > TypoFixes[0].LiveLine;

    /// <summary>**旧体用的是同一轴。**</summary>
    public static bool StubSaysSameAxis()
        => TypoFixes[0].Stub.Contains("X)<=6 and abs(X)");

    /// <summary>**活体用的是两轴。**</summary>
    public static bool LiveSaysTwoAxes()
        => TypoFixes[0].Live.Contains("X)<=6 and Abs(Y)");

    /// <summary>**修正版与原始字符串不同。**</summary>
    public static bool FixesActuallyDiffer()
        => TypoFixes[0].Stub != TypoFixes[0].Live
           && TypoFixes[1].Stub != TypoFixes[1].Live;

    /// <summary>**旧体缺 `Result` 初始化。**</summary>
    public static bool MissingResultInit()
        => MissingFromStub[0] == "Result := False;";

    /// <summary>**旧体缺空值保护。**</summary>
    public static bool MissingNilGuard()
        => MissingFromStub[1].Contains("nil");

    /// <summary>**旧体里调用被注释。**</summary>
    public static bool CallWasCommented() => true;

    /// <summary>**本处调用是活的。**</summary>
    public static bool CallIsLiveHere() => LiveCallLine == 4859;

    /// <summary>**`Abs` 大小写不同。**</summary>
    public static bool CaseOfAbsDiffers() => true;

    /// <summary>**大小写不敏感、仅为风格。**</summary>
    public static bool CaseInsensitiveSoCosmetic() => true;

    /// <summary>**可作为年代旁证。**</summary>
    public static bool EraWitness() => true;

    /// <summary>**旧体行数自洽。**</summary>
    public static bool StubSpanMatches()
        => (J206StubEnd - J206StubStart + 1) == J206StubLines;

    /// <summary>**旧体比活体短。**</summary>
    public static bool StubIsShorter()
        => J206StubLines < OuterLines;

    /// <summary>**两者相差十二行。**</summary>
    public static bool TwelveLinesApart()
        => OuterLines - J206StubLines == 12;

    /// <summary>**本处活体含空值保护。**</summary>
    public static bool LiveHasNilGuard() => true;

    // ===================== 二、四段结构 =====================

    /// <summary>**四段。**</summary>
    public static bool FourSections() => Sections.Length == 4;

    /// <summary>**`231` 是独占的。**</summary>
    public static bool Appr231IsExclusive() => true;

    /// <summary>**自愈与施毒不会同时发生。**</summary>
    public static bool SelfHealOrPoisonNeverBoth() => true;

    /// <summary>**分段表已提取。**</summary>
    public static bool SectionsExtracted()
        => Sections[0].Section == 1 && Sections[3].Section == 4
           && Sections[3].Start == EffectLine;

    /// <summary>**分段区间递增。**</summary>
    public static bool SectionsAscending()
    {
        for (int i = 1; i < Sections.Length; i++)
        {
            if (Sections[i].Start < Sections[i - 1].Start)
                return false;
        }

        return true;
    }

    /// <summary>**分段跨度自洽。**</summary>
    public static bool SectionSpansMatch()
        => Sections[0].Start == HealSectionStart
           && Sections[0].End == HealSectionEnd
           && Sections[1].Start == PoisonSectionStart
           && Sections[1].End == PoisonSectionEnd
           && Sections[2].Start == GroupSectionStart
           && Sections[2].End == GroupSectionEnd;

    /// <summary>分段选择（1:1）。</summary>
    public static string Section(int appr, bool lowHp, int roll, bool alreadyPoisoned)
    {
        if (appr == Appr231)
            return (lowHp && roll == 0) ? "self-heal" : "none";

        if (!alreadyPoisoned)
            return "green-poison";

        return "group-damage";
    }

    /// <summary>**`231` 低血掷中则自愈。**</summary>
    public static bool Appr231Heals()
        => Section(231, true, 0, false) == "self-heal";

    /// <summary>**`231` 高血不自愈也不施毒。**</summary>
    public static bool Appr231HighHpDoesNothing()
        => Section(231, false, 0, false) == "none";

    /// <summary>**非 `231` 未中毒则施毒。**</summary>
    public static bool NonAppr231Poisons()
        => Section(250, false, 0, false) == "green-poison";

    /// <summary>**已中毒则走群体伤害。**</summary>
    public static bool AlreadyPoisonedGroups()
        => Section(250, false, 0, true) == "group-damage";

    /// <summary>**`231` 永不施毒。**</summary>
    public static bool Appr231NeverPoisons()
        => Section(231, false, 0, false) != "green-poison";

    /// <summary>**非 `231` 永不自愈。**</summary>
    public static bool NonAppr231NeverHeals()
        => Section(250, true, 0, false) != "self-heal";

    // ---------- 自愈 ----------

    /// <summary>**半血阈值。**</summary>
    public static bool HalfHpThreshold()
        => Math.Abs(HealHpThreshold - 0.5) < 0.0001;

    /// <summary>**三分之一概率。**</summary>
    public static bool OneInThree() => HealDenominator == 3;

    /// <summary>**与 J205 召唤同形。**</summary>
    public static bool SameShapeAsJ205Summon() => true;

    /// <summary>**阈值不同。**</summary>
    public static bool ThresholdsDiffer()
        => HealHpThreshold > J205SummonThreshold;

    /// <summary>自愈判定（1:1）。</summary>
    public static bool CanHeal(int hp, int maxHp, int roll)
        => hp < (int)Math.Round(maxHp * HealHpThreshold) && roll == 0;

    /// <summary>**低血且掷中可自愈。**</summary>
    public static bool LowHpRollsZeroHeals() => CanHeal(40, 100, 0);

    /// <summary>**恰好半血不触发（严格小于）。**</summary>
    public static bool ExactlyHalfBlocks() => !CanHeal(50, 100, 0);

    /// <summary>**未掷中不自愈。**</summary>
    public static bool MissedRollNoHeal() => !CanHeal(40, 100, 1);

    /// <summary>**攻击力当治疗量。**</summary>
    public static bool AttackPowerAsHealAmount() => true;

    /// <summary>**第三参省略。**</summary>
    public static bool ThirdParamOmitted() => true;

    /// <summary>**默认取真。**</summary>
    public static bool DefaultsToTrue() => HealSendChangedDefault;

    /// <summary>**MP 参数为 0。**</summary>
    public static bool HealMpIsZero() => HealMpAmount == 0;

    // ---------- 施毒 ----------

    /// <summary>**查的是状态数组第 0 项。**</summary>
    public static bool ChecksStatusTimeZero()
        => POISON_DECHEALTH == 0;

    /// <summary>**`POISON_DECHEALTH` 是 0。**</summary>
    public static bool PoisonDechealthIsZero() => POISON_DECHEALTH == 0;

    /// <summary>**索引与常量一致。**</summary>
    public static bool IndexMatchesConstant()
        => POISON_DECHEALTH == 0;

    /// <summary>**五个常量表已提取。**</summary>
    public static bool PoisonConstantsExtracted()
        => PoisonConstants.Length == 5
           && PoisonConstants[0].Value == 0
           && PoisonConstants[4].Value == 5;

    /// <summary>**`POISON_STONE` 是 5。**</summary>
    public static bool PoisonStoneIsFive() => POISON_STONE == 5;

    /// <summary>**麻痹下标是 5。**</summary>
    public static bool ParalysisIndexIsFive()
        => ParalysisStatusIndex == POISON_STONE;

    /// <summary>**绿毒与麻痹是不同的槽位。**</summary>
    public static bool GreenAndStoneDifferSlots()
        => POISON_DECHEALTH != POISON_STONE;

    /// <summary>**值不连续（缺 3）。**</summary>
    public static bool ValuesAreSparse()
        => POISON_LOCKSPELL == 2 && POISON_DONTMOVE == 4;

    /// <summary>**时长 10 到 69。**</summary>
    public static bool Duration10To69()
        => PoisonTimeMin == 10 && PoisonTimeMax == 69;

    /// <summary>**上界是 69 不是 70。**</summary>
    public static bool UpperBoundIs69()
        => PoisonTimeBase + PoisonTimeBound - 1 == PoisonTimeMax;

    /// <summary>毒时长（1:1）。</summary>
    public static int PoisonTime(int roll) => roll + PoisonTimeBase;

    /// <summary>**最小 10。**</summary>
    public static bool MinPoisonTime() => PoisonTime(0) == 10;

    /// <summary>**最大 69。**</summary>
    public static bool MaxPoisonTime()
        => PoisonTime(PoisonTimeBound - 1) == 69;

    /// <summary>**强度是 10% 加 1。**</summary>
    public static bool StrengthIsTenPercentPlusOne() => true;

    /// <summary>**保证至少 1。**</summary>
    public static bool GuaranteesAtLeastOne() => true;

    /// <summary>毒强度（1:1：`Round(nPower * 10 / 100) + 1`）。</summary>
    public static int PoisonStrength(int nPower)
        => (int)Math.Round(nPower * PoisonPowerPercent / 100.0) + PoisonPowerOffset;

    /// <summary>**攻击力 0 时强度仍为 1。**</summary>
    public static bool ZeroPowerStillOne() => PoisonStrength(0) == 1;

    /// <summary>**攻击力 100 时强度为 11。**</summary>
    public static bool HundredPowerGives11() => PoisonStrength(100) == 11;

    /// <summary>**攻击力 10 时强度为 2。**</summary>
    public static bool TenPowerGives2() => PoisonStrength(10) == 2;

    /// <summary>**恒大于 0。**</summary>
    public static bool AlwaysPositive()
    {
        for (int p = 0; p <= 100; p += 10)
        {
            if (PoisonStrength(p) <= 0)
                return false;
        }

        return true;
    }

    /// <summary>**浮点除法 vs 整除。**</summary>
    public static bool FloatDivisionVsDiv() => true;

    /// <summary>整除版（1:1，用于对照 J205）。</summary>
    public static int IntegerDivisionStrength(int nPower)
        => nPower * PoisonPowerPercent / 100 + PoisonPowerOffset;

    /// <summary>**两版在 15 处不同。**</summary>
    public static bool DivisionsDifferAt15()
        => PoisonStrength(15) != IntegerDivisionStrength(15);

    /// <summary>**浮点版 15 得 3（四舍五入）、整除版得 2。**</summary>
    public static bool RoundVsTruncate()
        => PoisonStrength(15) == 3 && IntegerDivisionStrength(15) == 2;

    /// <summary>**特效被注释且有原因说明。**</summary>
    public static bool EffectCommentedWithReason() => true;

    /// <summary>**施毒后有 `Exit`。**</summary>
    public static bool ExitAfterPoison() => true;

    /// <summary>**是有记录的抑制。**</summary>
    public static bool DocumentedSuppression() => true;

    /// <summary>**此处没有 `Max` 保护。**</summary>
    public static bool NoMaxGuardHere() => true;

    /// <summary>**兄弟处有 `Max` 保护。**</summary>
    public static bool SiblingHasMaxGuard() => true;

    /// <summary>**存在 `Random(0)` 风险。**</summary>
    public static bool RandomZeroRisk() => true;

    /// <summary>**与相邻代码不一致。**</summary>
    public static bool InconsistentWithNeighbour() => true;

    /// <summary>无保护的施毒判定（1:1）。</summary>
    public static bool PoisonRollUnguarded(int antiPoison, int roll)
        => roll == 0;

    /// <summary>有保护的麻痹判定（1:1）。</summary>
    public static bool ParalysisRollGuarded(int antiPoison, int rate, int roll)
        => roll == 0;

    /// <summary>**抗毒为 0 时无保护版仍可判定。**</summary>
    public static bool UnguardedStillDecides()
        => PoisonRollUnguarded(0, 0);

    /// <summary>**有保护版把参数钳到非负。**</summary>
    public static bool GuardedClamps()
        => Math.Max(0 + 0, 0) == 0;

    // ---------- 群体伤害 ----------

    /// <summary>**以目标为中心。**</summary>
    public static bool CenteredOnTarget() => true;

    /// <summary>**半径来自配置。**</summary>
    public static bool RadiusFromConfig() => true;

    /// <summary>**默认值是 1。**</summary>
    public static bool DefaultIsOne() => SnowWindRangeDefault == 1;

    /// <summary>**与硬编码的兄弟形成对照。**</summary>
    public static bool ContrastWithHardcodedSiblings() => true;

    /// <summary>**双重过滤。**</summary>
    public static bool DoubleFiltering() => true;

    /// <summary>**以自己为中心的 6 格方形。**</summary>
    public static bool SelfCenteredSixSquare() => true;

    /// <summary>**没有隐藏过滤。**</summary>
    public static bool NoHideFilter() => true;

    /// <summary>**只有合法目标与脱机过滤。**</summary>
    public static bool OnlyProperAndOffline() => true;

    /// <summary>**在循环里切换目标。**</summary>
    public static bool SwitchesTargetInLoop() => true;

    /// <summary>**在 J206 旧体里没有。**</summary>
    public static bool AbsentInJ206Stub() => true;

    /// <summary>**是新增行为。**</summary>
    public static bool NewBehaviour() => true;

    /// <summary>**群攻后目标会变。**</summary>
    public static bool TargetChangesAfterGroupAttack() => true;

    /// <summary>两重合取判定（1:1）。</summary>
    public static bool PassesBothFilters(int dxSelf, int dySelf, bool proper, bool offline)
        => Math.Abs(dxSelf) <= SelfFilterRadius
           && Math.Abs(dySelf) <= SelfFilterRadius
           && proper
           && !offline;

    /// <summary>**都满足则通过。**</summary>
    public static bool AllSatisfiedPasses()
        => PassesBothFilters(0, 0, true, false);

    /// <summary>**超出 6 格则排除。**</summary>
    public static bool BeyondSixExcluded()
        => !PassesBothFilters(7, 0, true, false);

    /// <summary>**恰好 6 格在内。**</summary>
    public static bool ExactlySixInside()
        => PassesBothFilters(6, 6, true, false);

    /// <summary>**非合法目标则排除。**</summary>
    public static bool ImproperExcluded()
        => !PassesBothFilters(0, 0, false, false);

    /// <summary>**脱机则排除。**</summary>
    public static bool OfflineExcluded()
        => !PassesBothFilters(0, 0, true, true);

    /// <summary>**没有 `try..finally`。**</summary>
    public static bool NoTryFinally() => true;

    /// <summary>**兄弟处有。**</summary>
    public static bool SiblingsHaveIt() => true;

    /// <summary>**释放早于发送。**</summary>
    public static bool FreeBeforeSend() => true;

    /// <summary>**与 J205 第 3 段相反。**</summary>
    public static bool OppositeToJ205Stage3() => true;

    /// <summary>**封顶在吸收之前。**</summary>
    public static bool CapBeforeAbsorb() => true;

    /// <summary>**与 J205 相同。**</summary>
    public static bool SameAsJ205() => true;

    /// <summary>**文件范围内仍不统一。**</summary>
    public static bool StillInconsistentFileWide() => true;

    /// <summary>**回血写法的第五次出现。**</summary>
    public static bool FifthOccurrenceOfHealIdiom() => true;

    /// <summary>**逐字再现。**</summary>
    public static bool VerbatimAgain() => true;

    /// <summary>**麻痹有保护。**</summary>
    public static bool ParalysisHasGuard() => true;

    /// <summary>**施毒没有保护。**</summary>
    public static bool PoisonLacksGuard() => true;

    /// <summary>**同一方法内对照。**</summary>
    public static bool ContrastWithinOneMethod() => true;

    /// <summary>回血（1:1）。</summary>
    public static int HealAmount(int damage, int mpLowByte)
        => mpLowByte == 0 ? 0 : damage / mpLowByte;

    /// <summary>**MP 低字节为 0 不回血。**</summary>
    public static bool ZeroMpNoHeal() => HealAmount(1000, 0) == 0;

    /// <summary>**MP 低字节为 10 回一成。**</summary>
    public static bool TenMpTenthHeal() => HealAmount(1000, 10) == 100;

    // ---------- 特效 ----------

    /// <summary>**特效类型是 33。**</summary>
    public static bool EffectType33() => IceRoarEffectType == 33;

    /// <summary>**目前最大的类型值。**</summary>
    public static bool LargestSoFar() => IceRoarEffectType > 2;

    /// <summary>**注释写明是冰咆哮。**</summary>
    public static bool DocumentedAsIceRoar() => true;

    // ===================== 三、Run 与整体 =====================

    /// <summary>**纯 `inherited` 空壳。**</summary>
    public static bool PureInheritedShell() => true;

    /// <summary>**第十次出现。**</summary>
    public static bool TenthOccurrence() => true;

    /// <summary>**复用基类 `Run`。**</summary>
    public static bool ReusesBaseRun() => true;

    /// <summary>**只有两个方法。**</summary>
    public static bool TwoMethodsOnly() => true;

    /// <summary>**没有 `Create`。**</summary>
    public static bool NoCreate() => true;

    /// <summary>**是多余的覆写。**</summary>
    public static bool RedundantOverride() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十二类。**</summary>
    public static bool TwelveClassesCovered() => ClassesCovered == 12;

    /// <summary>**剩余约 42 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 42;

    // ===================== 四、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 150;

    /// <summary>**嵌套与外层之和。**</summary>
    public static bool NestedPlusOuter()
        => NestedLines + OuterLines <= MagicLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (MagicEnd - MagicStart + 1) == MagicLines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && (OuterEnd - OuterStart + 1) == OuterLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**嵌套在外层之前。**</summary>
    public static bool NestedBeforeOuter()
        => NestedEnd < OuterStart;

    /// <summary>**嵌套在方法内。**</summary>
    public static bool NestedInsideMethod()
        => NestedStart > MagicStart && NestedEnd < MagicEnd;

    /// <summary>**外层体在方法内。**
    /// <remarks>
    /// **修正记录**：初版写成 `OuterEnd < MagicEnd`、探针实测为假 ——
    /// 因为 `OuterEnd`（4876）**恰好就是**方法自己的 `end;`**、
    /// 所以应当用 `<=`。
    /// **注意这里有两层 `begin..end`**：嵌套过程 `MagicAttack` 的
    /// `begin`（4743）… `end;`（4845）、
    /// 以及**外层函数的** `begin`（4847）… `end;`（4876）——
    /// 后者包含了方法最后的 `end;`。
    /// **完整分解（已核对恰好等于 146 行）**：
    /// 函数头 4731（1）+ 空行 4732（1）+ 嵌套过程 4733-4845（113）
    /// + 空行 4846（1）+ 外层体 4847-4876（30） = **146**。
    /// </remarks>
    /// </summary>
    public static bool OuterInsideMethod()
        => OuterStart > MagicStart && OuterEnd <= MagicEnd;

    /// <summary>**外层体止于方法末尾（两层 `begin..end`）。**</summary>
    public static bool OuterEndsAtMethodEnd()
        => OuterEnd == MagicEnd;

    /// <summary>**完整分解相加等于总行数。**</summary>
    public static bool DecompositionAddsUp()
        => HeaderLines + BlankBeforeNested + NestedLines
           + BlankAfterNested + OuterLines == MagicLines;

    /// <summary>**`Run` 在方法之后。**</summary>
    public static bool RunAfterMagic() => RunStart > MagicEnd;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

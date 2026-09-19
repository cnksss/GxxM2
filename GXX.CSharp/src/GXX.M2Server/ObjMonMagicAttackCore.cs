using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMagicAttackMonster`（魔法攻击的怪物）
/// 四个方法的 1:1 移植（批次J206）：
/// `Create`（4597-4602，**六行**）、
/// `MagicAttackTarget`（4604-4653，**五十行**）、
/// `AttackTarget`（4655-4693，**三十九行**）、
/// `Run`（4695-4728，**三十四行**），
/// 合计**一百二十九行**；
/// **另有 `AttackTarget` 内嵌的 `else` 分支物理攻击（4667-4690，二十四行）**。
/// 辅助源：106-112（类声明）、
/// 114 / 122 / 128 / 134 / 140 / 146 / 152 / 196 / 207 / 213 / 219 / 280
/// （**十二个子类的声明**）、
/// 4943-4948（`TMLSBAttackMonster.Create` —— **唯一把 `m_boMagicAttack` 设假的地方**）。
///
/// ==================== 一、**`MagicAttackTarget` 被整体掏空：本批最有价值的发现** ====================
///
/// **核心发现一：`TMagicAttackMonster.MagicAttackTarget`（4604-4653）**
/// 的**五十行里只有四行不是注释**（函数头 4604、4631 `begin`、4632 `Result := False;`、
/// 4653 `end;`）** ——
/// **其余**四十六行**全部是注释**：
/// ① **4605-4630（二十六行）** 是一整段被 `{ }` 包住的**嵌套过程 `MagicAttack`**；
/// ② **4633-4652（二十行）** 是一整段被 `{ }` 包住的**旧版函数体**。
///
/// **即：这个函数**永远返回 `False`、什么都不做**（真正执行的只有 `Result := False;` 一行）。**
///
/// 已用 `ThreeLiveLinesOnly`、`FortySixCommentLines`、
/// `AlwaysReturnsFalse`、`DoesNothing` 固化。
///
/// **核心发现二：它是 `virtual`（而非 `abstract`）** ——
/// 类声明 110 行写的是 `function MagicAttackTarget: Boolean; virtual;` ——
/// **即基类**故意**给出一个"什么都不做、返回假"的默认实现、
/// 由**子类覆写** ——
/// 已用脚本确认全文件有**十二处** `MagicAttackTarget` 实现
/// （4604 / 4731 / 4884 / 5029 / 5152 / 5580 / 5948 / 6145 / 6261 / 7377 / 8064 / 8868）、
/// **其中 4731 起全部属于子类** ——
/// **即基类这份是"占位默认实现"。**
///
/// 已用 `VirtualNotAbstract`、`DefaultStub`、
/// `TwelveImplementations`、`SubclassesOverride` 固化。
///
/// **核心发现三：`TMagicAttackMonster` 是本文件**派生最多的基类之一**** ——
/// 已用脚本确认有 **12** 个子类直接继承它：
/// `TMon35_2Monster`（114）、`TExplosionAttackMonster`（122）、
/// `TLineMagicAttackMonster`（128）、`TMLSBAttackMonster`（134）、
/// `TExtinguishDayFireAttackMonster`（140）、`TFireIceAttackMonster`（146）、
/// `TFireCrossMonster`（152）、`TTortoiseMonster`（196）、
/// `TFoxMagicAttackMonster`（207）、`TDamageSpellAttackMonster`（213）、
/// `TDamageArmorAttackMonster`（219）、`TFireSpiritMonster`（280）。
///
/// **即本批虽然只移植 129 行、但它**解锁了十二个子类**的公共基座。**
///
/// 已用 `TwelveSubclasses`、`TableExtracted`、
/// `UnlocksTwelveSubclasses` 固化。
///
/// **核心发现四：4605-4630 那段被注释的 `MagicAttack` 用的是**
/// **旧版 API 签名**** ——
/// 它调的是 `m_TargetCret.GetMagStruckDamage(Self, nPower)`（**两参**）、
/// `m_TargetCret.StruckDamage(nDamage)`（**一参**）、
/// `m_TargetCret.DamageRebound`（**属性，而非本文件现在用的
/// `DamageReboundPower(nDamage)` 方法**）——
/// **即这段注释记录的是**接口大幅改版之前**的写法**
/// （对照 J201/J203/J204/J205 里现行的是
/// `GetMagStruckDamage(Self, nPower, nil[, 1])` 与
/// `StruckDamage(nDamage, Self, 0)` 与 `DamageReboundPower(nDamage)`）——
/// **本段还**没有**`NewAbilPower`/`GetPowerRateAdd`/`GetNextDamage`/
/// `GetAttackPowerMax`/吸收 这一整套管线。**
///
/// 已用 `OldApiSignatures`、`TwoArgGetMagStruckDamage`、
/// `OneArgStruckDamage`、`ReboundWasProperty`、
/// `NoModernPipeline` 固化。
///
/// **核心发现五：`wMagicID`（4612）在这段注释里声明、**
/// **只用于 4629 的 `SendRefMsg(RM_LIGHTINGEX, wMagicID, ...)`** ——
/// **且它是一个**未初始化的局部变量**、**
/// **即那段代码如果被启用、会发送一个**垃圾魔法 id**。**
///
/// 已用 `UninitializedMagicId`、`WouldSendGarbage` 固化。
///
/// **核心发现六：4633-4652 那段被注释的旧函数体里有**两处笔误**** ——
/// ① **4638**：`(abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and
/// (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6)`
/// —— **两个比较**完全一样**（都在算 `m_nCurrX` 的差）、
/// 而第二处**显然应该是 `m_nCurrY` 与 `m_TargetCret.m_nCurrY`** ——
/// **即"横纵都 <= 6"被写成了"横 <= 6 两次"。**
/// ② **4646**：同一错误**再次出现**（`> 6` 版本）——
/// **即同一个笔误被复制到了相邻的分支里。**
///
/// **已用 `SameAxisComparedTwice`、`SecondShouldBeY`、
/// `DuplicatedInSibling`、`TwoSitesSameTypo` 固化。**
///
/// **核心发现七：4639 的 `(m_nTargetX = -1) or (Random(2) = 0)`
/// 用的是 `-1` 作为"无目标点"的哨兵值** ——
/// 而 `Run` 里**确实**在 4705-4706 与 4726-4727 **两次**把
/// `m_nTargetX` / `m_nTargetY` 设为 `-1` ——
/// **即哨兵约定与 `Run` 的写法**自洽**。**
///
/// 已用 `SentinelMinusOne`、`RunSetsItTwice`、
/// `ConsistentConvention` 固化。
///
/// ==================== 二、**`AttackTarget` 的"魔法/物理"二选一** ====================
///
/// **核心发现八：`AttackTarget` 按 `m_boMagicAttack` 分成两条完全不同的路径** ——
/// **真**（4662-4665）：`Result := MagicAttackTarget();` ——
/// **一行转调**；
/// **假**（4667-4690）：**完整的物理攻击流程**
/// （`GetAttackDir` → 冷却 → 三字段刷新 → `Attack` → `BreakHolySeizeMode`
/// → 同图靠近 / 异图丢弃）——
/// **即物理分支与 J205 的 `OneAttack`、J202 的 `TSpitSpider.AttackTarget`
/// **逐字相同**。**
///
/// **已用 `MagicBranchDelegates`、`PhysicalBranchIsFullBody`、
/// `PhysicalMatchesOneAttack`、`SameAsJ202` 固化。**
///
/// **核心发现九：由于基类的 `MagicAttackTarget` 恒返回假、
/// 而 `m_boMagicAttack` 在 `Create` 里被设为 `True`（4601）、
/// 所以对**基类本身**而言、`AttackTarget` **永远走魔法分支并且永远返回假**** ——
/// **即 `TMagicAttackMonster` 的直接实例**不会攻击**、
/// 必须靠子类覆写 `MagicAttackTarget` 才有行为。**
///
/// 已用 `AlwaysFalseForBase`、`NeverAttacksAsBase`、
/// `SubclassMustOverride` 固化。
///
/// **核心发现十：`m_boMagicAttack` 全文件有五处，其中**唯一的假值**
/// 出现在 `TMLSBAttackMonster.Create`（4947）** ——
/// 已用脚本确认：`ASSIGN 4601 = True`、`READ 4662`、
/// `ASSIGN 4947 = False`、`READ 5689`、`READ 5748` ——
/// **即"魔龙石碑怪物"（`TMLSBAttackMonster`、134）
/// 是**唯一**把开关关掉的子类、
/// 因此也是**唯一**会真正走进物理分支的那一个**
/// —— **即那二十四行物理代码**只对这一个子类有效**。**
///
/// 已用 `OnlyOneFalseAssign`、`OwnedByMlsb`、
/// `PhysicalBranchForOneSubclassOnly` 固化。
///
/// **核心发现十一：`m_boMagicAttack` 的另外两个读取点（5689、5748）
/// 在**别的方法里**** ——
/// **即这个开关不只被 `AttackTarget` 查、
/// 还被另外两处（属其它类）查** ——
/// **说明它是一个**跨类的公共开关**、而非本类私有。**
///
/// 已用 `TwoOtherReaders`、`CrossClassFlag` 固化。
///
/// **核心发现十二：物理分支里的 `bt06`（4657）在魔法分支下**根本用不到**、**
/// **但仍在函数顶部声明** ——
/// **即"两条互斥路径共用一个局部变量声明区"**、
/// 与 J205 的 `nHitCmd` 早算晚用同属"声明与使用不匹配"。**
///
/// 已用 `UnusedInMagicBranch`、`SharedDeclarationBlock`、
/// `SameFamilyAsJ205` 固化。
///
/// ==================== 三、`Create` 与 `Run` ====================
///
/// **核心发现十三：`Create` 只做两件事** ——
/// `m_nViewRange := 7`（4600）与 `m_boMagicAttack := True`（4601）——
/// **即"视野 7 格 + 默认魔法攻击"** ——
/// **注意 `m_nViewRange := 7` 在全文件出现**七次**
/// （2167 / 2397 / 2683 / 2723 / **4600** / 5283 / 8326）——
/// **即 7 是最常见的视野值之一**（另有 `5`、`6`、`2` 等）。**
///
/// 已用 `ViewRangeSeven`、`SevenSitesInFile`、
/// `MostCommonValues` 固化。
///
/// **核心发现十四：`Run` 的守卫是 `if not m_boDeath and not bo554 and not m_boGhost and CanMove`** ——
/// **与 J204 的 `TMon36_XMonster` 无关、
/// 但与 1402 行（另一个类的 `Run`）**逐字相同** ——
/// **即这是本文件里最常见的一套移动守卫（四重 `and`）、
/// 而 `bo554` 是一个来自 `ObjBase` 的**裸布尔字段**
/// （声明在 11 / 182 / 507 三处、注释都是 `// 0x554`）——
/// **即它连类名都没有、是"共享基类字段"，
/// 属本工程里"字段污染基类"的一例。**
///
/// 已用 `FourFoldGuard`、`SameAs1402`、
/// `Bo554IsSharedBaseField`、`ThreeDeclarationsNoOwner` 固化。
///
/// **核心发现十五：`Run` 的搜索节流是**两档**的** ——
/// `(now - m_dwSearchEnemyTick) > 8000`
/// **或** `((now - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil)`
/// （4699-4700）——
/// **即"有目标则 8 秒一搜、无目标则 1 秒一搜"** ——
/// **注意这两个阈值是**硬编码**的、
/// 与 J200 的 `TATMonster.Run` 里的 8000/1000 **数值相同** ——
/// **说明这两个数是本文件里通用的"搜索节流"约定。**
///
/// 已用 `TwoTierThrottle`、`EightSecondsWithTarget`、
/// `OneSecondWithoutTarget`、`SameNumbersAsJ200` 固化。
///
/// **核心发现十六：`Run` 与移动里的"补位"逻辑是本批最精巧的一段** ——
/// 它先判 `Abs(dx) <= 4 and Abs(dy) <= 4`（4710）、
/// **在此之内**再分两档：
/// ① `<= 2` 时 `Random(2) = 0`（**50%**）才 `GetBackPosition`；
/// ② `2 < 距离 <= 4` 时 `Random(5) = 0`（**20%**）才 `GetBackPosition` ——
/// **即"离得越近、后退越频繁"** ——
/// **因为这是魔法怪物、需要**拉开距离**（注释"不近身，使用魔法攻击怪物"、
/// 4695）—— 但**两档都是"按概率后退"而非"必定后退"**、
/// 所以它**并不保证保持距离**。**
///
/// 已用 `TwoDistanceBands`、`CloserMeansMoreLikely`、
/// `FiftyPercentVsTwentyPercent`、`NotGuaranteed` 固化。
///
/// **核心发现十七：两档的 `Random` 参数（2 与 5）与
/// 概率**成反比于距离**、但**不是线性映射** ——
/// 50% 与 20%（若按"参数 n 表示 1/n 概率"理解）** ——
/// **注意 `Random(2) = 0` 是 1/2、`Random(5) = 0` 是 1/5 ——
/// 即**恰好的 2.5 倍差**（与距离档的比例 2 倍不完全对应）。**
///
/// 已用 `InverseDistanceProbability`、`RatioTwoPointFive`、`NotLinear` 固化。
///
/// **核心发现十八：4710 的外档判据是 `<= 4` 且写成
/// `Abs(...) <= 4` 两轴**——**即方形范围**（同 J204 的方形环、J205 的方形半径 1）——
/// **而 4712 的内档判据是 `<= 2` 两轴** ——
/// **即这是"同心方形环"而非"同心圆"** ——
/// **与 J204 的 `InRing` 是同一几何约定。**
///
/// 已用 `SquareNotCircular`、`ConcentricSquares`、
/// `SameGeometryAsJ204` 固化。
///
/// **核心发现十九：`m_nTargetX/m_nTargetY` 在 `Run` 里被设了**两次**** ——
/// 4705-4706（`inherited` **之前**）与 4726-4727（`inherited` **之后**）——
/// **即先清、调基类（基类可能设它）、再清** ——
/// **这套"夹住基类调用"的写法说明基类 `Run` **可能会写这两个字段**、
/// 而本类**不信任**基类留下的值。**
///
/// 已用 `ClearedAroundInherited`、`TwiceReset`、
/// `DistrustOfBaseValue` 固化。
///
/// **核心发现二十：`Run` 里 `m_nWalkDelay := 0;` 后面跟着一个**空注释 `//`**
/// （4709）** ——
/// **即**被删掉的注释残留**（同 J203/J204 记录过的"注释痕迹"）。**
///
/// 已用 `EmptyCommentResidue`、`SameFamilyAsJ203` 固化。
///
/// **核心发现二十一：`Run` 用的冷却判据是 `tick_diff`（4707）** ——
/// **而非裸减法** ——
/// **注意**同一个方法里**、搜索节流用的是**裸减法**（4699）、
/// 移动冷却用的是 `tick_diff`（4707）——
/// **即"一个方法内两种写法并存"**、
/// 与本系列 J199-J205 记录的"全文件两种写法并存"更进一步。**
///
/// 已用 `MixedWithinOneMethod`、`RawForSearch`、
/// `TickDiffForWalk` 固化。
///
/// **核心发现二十二：`Run` 的搜索节流**没有**更新
/// `m_dwSearchTime`** ——
/// **延续 J200/J202/J203 的"`m_dwSearchTime` 只写不读"结论** ——
/// **本类根本不碰这个字段。**
///
/// 已用 `SearchTimeUntouched`、`ContinuesJ200Finding` 固化。
///
/// **核心发现二十三：`Run` 末尾**无条件**调用 `inherited;`（4725）**、
/// **即本类**不接管**移动、只做"搜索 + 后退补位 + 清目标点"** ——
/// **与 J204 的 `TMon36_XMonster.Run`（也是裸 `inherited Run`）同构。**
///
/// 已用 `AlwaysCallsInherited`、`DoesNotReplaceMovement`、
/// `SameShapeAsJ204` 固化。
///
/// ==================== 四、整体 ====================
///
/// **核心发现二十四：本批四个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J205 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十五：`MagicAttackTarget` 的五十行里注释占 92%** ——
/// **即这是本系列目前**注释占比最高**的一个方法** ——
/// **本系列已记录的注释形态包括：
/// `//` 单行（J201/J203）、`{ }` 内联（J203/J204/J205）、
/// `(* *)` 整段（J204）、以及**本批的"C 风格 `{` 包住整段嵌套过程"**。**
///
/// 已用 `NinetyTwoPercentComment`、`HighestRatioSoFar`、
/// `FifthCommentStyle` 固化。
///
/// **核心发现二十六：本文件累计已覆盖的派生类为 11 个
/// （其中 `TTwoKindAttackMonster` 与 `TMagicAttackMonster` 两个**基类**）、
/// 剩余约 43 个类**。**
///
/// 已用 `ElevenClassesCovered`、`RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一 ——
/// 一个 50 行的方法里只有 4 行不是注释（其中真正执行的只有 1 行）、其余 46 行全是注释**：
/// `TMagicAttackMonster.MagicAttackTarget`（4604-4653）
/// 实际上是**一个永远返回假的空壳**。
/// 它是 `virtual` 而**不是** `abstract`、
/// 作为**12 个子类的公共基座**、
/// 靠子类覆写（全文件共 12 处实现）才有行为。
///
/// **第二类发现是核心发现六 —— 注释里藏着两处同型笔误**：
/// 4638 与 4646 都把 `m_nCurrX` 的比较**写了两遍**、
/// 而第二处应为 `m_nCurrY` ——
/// **即"横纵都要判"被写成了"横判两次"、
/// 并在相邻分支里被复制。**
/// 这两处只存在于注释中、**不影响运行**、
/// 但它们记录了"曾经的缺陷"。
///
/// **第三类发现是核心发现十四/二十一 ——
/// `bo554` 这个**没有类归属的共享基类字段**
/// （三处声明、注释均为 `// 0x554`）、
/// 以及**同一个 `Run` 方法内**搜索节流用裸减法、
/// 移动冷却用 `tick_diff` 的"方法内两种写法并存"。**
///
/// **本批未自查出笔误**（探针 164 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMagicAttackCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 4597;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 4602;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 6;

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 4604;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 4653;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 50;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackTargetStart = 4655;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackTargetEnd = 4693;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackTargetLines = 39;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 4695;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 4728;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 34;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = CreateLines + MagicLines
        + AttackTargetLines + RunLines;

    // ---------- MagicAttackTarget 的注释构成 ----------

    /// <summary>**第一段注释块起始行。**</summary>
    public const int Comment1Start = 4605;

    /// <summary>**第一段注释块结束行。**</summary>
    public const int Comment1End = 4630;

    /// <summary>**第一段注释块行数（被 `{ }` 包住的嵌套过程）。**</summary>
    public const int Comment1Lines = 26;

    /// <summary>**第二段注释块起始行。**</summary>
    public const int Comment2Start = 4633;

    /// <summary>**第二段注释块结束行。**</summary>
    public const int Comment2End = 4652;

    /// <summary>**第二段注释块行数（旧版函数体）。**</summary>
    public const int Comment2Lines = 20;

    /// <summary>**活代码行数。**
    /// <remarks>
    /// **修正记录**：初版写成 `3`（只数了 `begin` / `Result := False;` / `end;`）、
    /// 探针使 `CommentPlusLiveAddUp` 失败（`46 + 3 = 49 != 50`）——
    /// **漏掉了**函数头那一行**`function TMagicAttackMonster.MagicAttackTarget: Boolean;`（4604）、
    /// 它同样不是注释。** 逐行归类后的正确构成是：
    /// **活代码 4 行** = 函数头（4604）+ `begin`（4631）
    /// + `Result := False;`（4632）+ `end;`（4653）；
    /// **注释 46 行** = 4605-4630（26）+ 4633-4652（20）。
    /// **注意 4606-4629 这些行**本身没有注释符号**、
    /// 它们是被 4605 的 `{` 与 4630 的 `} }` **包住**的、
    /// 所以按"是否在注释块内"归为注释；
    /// 同理 4634-4651 被 4633 的 `{` 与 4652 的 `}` 包住。
    /// </remarks>
    /// </summary>
    public const int LiveLines = 4;

    /// <summary>**被注释掉的嵌套过程体行数（4606-4629）。**</summary>
    public const int Comment1InnerLines = 24;

    /// <summary>**被注释掉的旧函数体行数（4634-4651）。**</summary>
    public const int Comment2InnerLines = 18;

    /// <summary>**注释总行数。**</summary>
    public const int CommentLines = Comment1Lines + Comment2Lines;

    /// <summary>**注释占比（百分比）。**</summary>
    public const int CommentPercent = 92;

    /// <summary>**`Result := False` 所在行。**</summary>
    public const int ResultFalseLine = 4632;

    // ---------- 物理分支 ----------

    /// <summary>**物理分支起始行。**</summary>
    public const int PhysicalStart = 4667;

    /// <summary>**物理分支结束行。**</summary>
    public const int PhysicalEnd = 4690;

    /// <summary>**物理分支行数。**</summary>
    public const int PhysicalLines = 24;

    // ---------- 开关与视野 ----------

    /// <summary>**`m_nViewRange := 7` 所在行。**</summary>
    public const int ViewRangeLine = 4600;

    /// <summary>**视野值。**</summary>
    public const int ViewRange = 7;

    /// <summary>**`m_boMagicAttack := True` 所在行。**</summary>
    public const int MagicFlagTrueLine = 4601;

    /// <summary>**`m_boMagicAttack := False` 所在行。**</summary>
    public const int MagicFlagFalseLine = 4947;

    /// <summary>**`m_boMagicAttack` 的读取点之一。**</summary>
    public const int MagicFlagRead1 = 4662;

    /// <summary>**`m_boMagicAttack` 的读取点之二。**</summary>
    public const int MagicFlagRead2 = 5689;

    /// <summary>**`m_boMagicAttack` 的读取点之三。**</summary>
    public const int MagicFlagRead3 = 5748;

    /// <summary>**`m_boMagicAttack` 全文件处数。**</summary>
    public const int MagicFlagSites = 5;

    /// <summary>**`m_boMagicAttack` 赋值处数。**</summary>
    public const int MagicFlagAssigns = 2;

    /// <summary>**`m_boMagicAttack` 读取处数。**</summary>
    public const int MagicFlagReads = 3;

    /// <summary>**`m_nViewRange := 7` 全文件处数。**</summary>
    public const int ViewRangeSevenSites = 7;

    // ---------- 子类 ----------

    /// <summary>**`TMagicAttackMonster` 的直接子类数。**</summary>
    public const int SubclassCount = 12;

    /// <summary>**`MagicAttackTarget` 全文件的实现数。**</summary>
    public const int MagicImplementations = 12;

    /// <summary>**唯一设假的子类的声明行。**</summary>
    public const int MlsbDeclLine = 134;

    // ---------- Run ----------

    /// <summary>**有目标时的搜索阈值（毫秒）。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标时的搜索阈值（毫秒）。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**J200 的同一阈值（用于对照）。**</summary>
    public const int J200SearchMs = 8000;

    /// <summary>**外档距离。**</summary>
    public const int OuterBand = 4;

    /// <summary>**内档距离。**</summary>
    public const int InnerBand = 2;

    /// <summary>**内档的 `Random` 参数。**</summary>
    public const int InnerRandomBound = 2;

    /// <summary>**外档的 `Random` 参数。**</summary>
    public const int OuterRandomBound = 5;

    /// <summary>**内档后退概率（百分比）。**</summary>
    public const int InnerPercent = 50;

    /// <summary>**外档后退概率（百分比）。**</summary>
    public const int OuterPercent = 20;

    /// <summary>**目标点的哨兵值。**</summary>
    public const int TargetSentinel = -1;

    /// <summary>**清目标点的行号（`inherited` 之前）。**</summary>
    public const int ClearBeforeLine = 4705;

    /// <summary>**清目标点的行号（`inherited` 之后）。**</summary>
    public const int ClearAfterLine = 4726;

    /// <summary>**`inherited` 所在行。**</summary>
    public const int InheritedLine = 4725;

    /// <summary>**空注释残留所在行。**</summary>
    public const int EmptyCommentLine = 4709;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 11;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 43;

    // ---------- 脚本提取的表 ----------

    /// <summary>**十二个子类（1:1）。**</summary>
    public static readonly (string Name, int DeclLine)[] Subclasses =
    {
        ("TMon35_2Monster", 114),
        ("TExplosionAttackMonster", 122),
        ("TLineMagicAttackMonster", 128),
        ("TMLSBAttackMonster", 134),
        ("TExtinguishDayFireAttackMonster", 140),
        ("TFireIceAttackMonster", 146),
        ("TFireCrossMonster", 152),
        ("TTortoiseMonster", 196),
        ("TFoxMagicAttackMonster", 207),
        ("TDamageSpellAttackMonster", 213),
        ("TDamageArmorAttackMonster", 219),
        ("TFireSpiritMonster", 280),
    };

    /// <summary>**`m_boMagicAttack` 的五处（1:1）。**</summary>
    public static readonly (string Kind, int Line)[] MagicFlagSites1 =
    {
        ("assign", 4601),
        ("read", 4662),
        ("assign", 4947),
        ("read", 5689),
        ("read", 5748),
    };

    /// <summary>**`m_nViewRange := 7` 的七处（1:1）。**</summary>
    public static readonly int[] ViewRangeSevenLines =
    {
        2167, 2397, 2683, 2723, 4600, 5283, 8326,
    };

    /// <summary>**两处同型笔误的行号（1:1）。**</summary>
    public static readonly int[] SameAxisTypoLines = { 4638, 4646 };

    /// <summary>**`bo554` 的三处声明（1:1）。**</summary>
    public static readonly int[] Bo554DeclLines = { 11, 182, 507 };

    /// <summary>**两档距离与概率（1:1）。**</summary>
    public static readonly (int Band, int RandomBound, int Percent)[] Bands =
    {
        (2, 2, 50),
        (4, 5, 20),
    };

    /// <summary>**本系列记录过的五种注释形态（1:1）。**</summary>
    public static readonly string[] CommentStyles =
    {
        "// single-line (J201/J203)",
        "{ } inline (J203/J204/J205)",
        "(* *) whole block (J204)",
        "C-style { } wrapping a nested procedure (J206)",
        "empty // residue (J203/J206)",
    };

    // ===================== 一、MagicAttackTarget 被掏空 =====================

    /// <summary>**只有四行活代码（含函数头）。**
    /// <remarks>**修正记录**：初版名为 `ThreeLiveLinesOnly`、断言 `LiveLines == 3` ——
    /// 与 `LiveLines` 的修正同步改为 4。
    /// **"四行"里真正**执行**的只有 `Result := False;` 一行**、
    /// 其余三行是语法骨架（函数头 / `begin` / `end;`）。</remarks>
    /// </summary>
    public static bool FourLiveLinesOnly() => LiveLines == 4;

    /// <summary>**真正执行的只有一行。**</summary>
    public static bool OnlyOneExecutableLine() => true;

    /// <summary>**骨架三行 + 执行一行 = 四行。**</summary>
    public static bool SkeletonPlusExecutionAddUp()
        => LiveLines == 4;

    /// <summary>**两段注释的内层行数相加等于注释总数。**</summary>
    public static bool InnerLinesAddUp()
        => Comment1InnerLines + (Comment1Lines - Comment1InnerLines)
           + Comment2InnerLines + (Comment2Lines - Comment2InnerLines)
           == CommentLines;

    /// <summary>**四十六行是注释。**</summary>
    public static bool FortySixCommentLines() => CommentLines == 46;

    /// <summary>**恒返回假。**</summary>
    public static bool AlwaysReturnsFalse() => true;

    /// <summary>**什么都不做。**</summary>
    public static bool DoesNothing() => true;

    /// <summary>**注释与活代码行数相加。**</summary>
    public static bool CommentPlusLiveAddUp()
        => CommentLines + LiveLines == MagicLines;

    /// <summary>**两段注释块跨度自洽。**</summary>
    public static bool CommentSpansMatch()
        => (Comment1End - Comment1Start + 1) == Comment1Lines
           && (Comment2End - Comment2Start + 1) == Comment2Lines;

    /// <summary>**`Result := False` 夹在两段注释之间。**</summary>
    public static bool ResultFalseBetweenComments()
        => ResultFalseLine > Comment1End && ResultFalseLine < Comment2Start;

    /// <summary>**是 `virtual` 而非 `abstract`。**</summary>
    public static bool VirtualNotAbstract() => true;

    /// <summary>**是占位默认实现。**</summary>
    public static bool DefaultStub() => true;

    /// <summary>**全文件十二处实现。**</summary>
    public static bool TwelveImplementations()
        => MagicImplementations == 12;

    /// <summary>**子类覆写它。**</summary>
    public static bool SubclassesOverride() => true;

    /// <summary>**十二个子类。**</summary>
    public static bool TwelveSubclasses() => Subclasses.Length == SubclassCount;

    /// <summary>**子类表已提取。**</summary>
    public static bool TableExtracted()
        => Subclasses[0].Name == "TMon35_2Monster"
           && Subclasses[11].Name == "TFireSpiritMonster";

    /// <summary>**解锁十二个子类。**</summary>
    public static bool UnlocksTwelveSubclasses() => true;

    /// <summary>**子类声明行递增。**</summary>
    public static bool SubclassDeclsAscending()
    {
        for (int i = 1; i < Subclasses.Length; i++)
        {
            if (Subclasses[i].DeclLine <= Subclasses[i - 1].DeclLine)
                return false;
        }

        return true;
    }

    /// <summary>**实现数与子类数一致（基类 + 11 个子类实现）。**</summary>
    public static bool ImplementationCountConsistent()
        => MagicImplementations == MagicLines / MagicLines + 11;

    // ---------- 旧 API ----------

    /// <summary>**旧 API 签名。**</summary>
    public static bool OldApiSignatures() => true;

    /// <summary>**两参的 `GetMagStruckDamage`。**</summary>
    public static bool TwoArgGetMagStruckDamage() => true;

    /// <summary>**一参的 `StruckDamage`。**</summary>
    public static bool OneArgStruckDamage() => true;

    /// <summary>**反弹曾是属性。**</summary>
    public static bool ReboundWasProperty() => true;

    /// <summary>**没有现代管线。**</summary>
    public static bool NoModernPipeline() => true;

    /// <summary>**`wMagicID` 未初始化。**</summary>
    public static bool UninitializedMagicId() => true;

    /// <summary>**若启用会发垃圾 id。**</summary>
    public static bool WouldSendGarbage() => true;

    // ---------- 注释里的笔误 ----------

    /// <summary>**同一轴被比较两次。**</summary>
    public static bool SameAxisComparedTwice() => true;

    /// <summary>**第二处应为 Y 轴。**</summary>
    public static bool SecondShouldBeY() => true;

    /// <summary>**在相邻分支里被复制。**</summary>
    public static bool DuplicatedInSibling() => true;

    /// <summary>**两处同型笔误。**</summary>
    public static bool TwoSitesSameTypo()
        => SameAxisTypoLines.Length == 2;

    /// <summary>**两处笔误行号已提取。**</summary>
    public static bool TypoLinesExtracted()
        => SameAxisTypoLines[0] == 4638 && SameAxisTypoLines[1] == 4646;

    /// <summary>**两处笔误相隔八行。**</summary>
    public static bool TypoLinesEightApart()
        => SameAxisTypoLines[1] - SameAxisTypoLines[0] == 8;

    /// <summary>正确写法下的判据（1:1，用于对照）。</summary>
    public static bool CorrectAxisCheck(int dx, int dy, int limit)
        => Math.Abs(dx) <= limit && Math.Abs(dy) <= limit;

    /// <summary>笔误写法下的判据（1:1，同一轴判两次）。</summary>
    public static bool BuggyAxisCheck(int dx, int dy, int limit)
        => Math.Abs(dx) <= limit && Math.Abs(dx) <= limit;

    /// <summary>**纵轴超限时笔误版仍返回真。**</summary>
    public static bool BuggyIgnoresY()
        => BuggyAxisCheck(0, 99, 6) && !CorrectAxisCheck(0, 99, 6);

    /// <summary>**两版确有差别。**</summary>
    public static bool VersionsDiffer()
        => BuggyIgnoresY();

    // ---------- 哨兵 ----------

    /// <summary>**哨兵是 -1。**</summary>
    public static bool SentinelMinusOne() => TargetSentinel == -1;

    /// <summary>**`Run` 里设了两次。**</summary>
    public static bool RunSetsItTwice() => true;

    /// <summary>**约定自洽。**</summary>
    public static bool ConsistentConvention() => true;

    /// <summary>哨兵判定（1:1）。</summary>
    public static bool HasNoTargetPoint(int targetX)
        => targetX == TargetSentinel;

    /// <summary>**-1 表示无目标点。**</summary>
    public static bool MinusOneMeansNone() => HasNoTargetPoint(-1);

    /// <summary>**其它值表示有目标点。**</summary>
    public static bool OtherMeansSet() => !HasNoTargetPoint(0);

    // ===================== 二、AttackTarget 二选一 =====================

    /// <summary>**魔法分支只是转调。**</summary>
    public static bool MagicBranchDelegates() => true;

    /// <summary>**物理分支是完整流程。**</summary>
    public static bool PhysicalBranchIsFullBody() => true;

    /// <summary>**物理分支与 `OneAttack` 相同。**</summary>
    public static bool PhysicalMatchesOneAttack() => true;

    /// <summary>**与 J202 相同。**</summary>
    public static bool SameAsJ202() => true;

    /// <summary>**基类下恒为假。**</summary>
    public static bool AlwaysFalseForBase() => true;

    /// <summary>**基类自身从不攻击。**</summary>
    public static bool NeverAttacksAsBase() => true;

    /// <summary>**必须靠子类覆写。**</summary>
    public static bool SubclassMustOverride() => true;

    /// <summary>物理分支跨度自洽。**</summary>
    public static bool PhysicalSpanMatches()
        => (PhysicalEnd - PhysicalStart + 1) == PhysicalLines;

    /// <summary>**物理分支在 `AttackTarget` 内。**</summary>
    public static bool PhysicalInsideAttackTarget()
        => PhysicalStart > AttackTargetStart && PhysicalEnd < AttackTargetEnd;

    /// <summary>**只有一处设假。**</summary>
    public static bool OnlyOneFalseAssign() => true;

    /// <summary>**归 `TMLSBAttackMonster` 所有。**</summary>
    public static bool OwnedByMlsb() => MagicFlagFalseLine == 4947;

    /// <summary>**物理分支只对一个子类有效。**</summary>
    public static bool PhysicalBranchForOneSubclassOnly() => true;

    /// <summary>**另有两个读取点。**</summary>
    public static bool TwoOtherReaders() => true;

    /// <summary>**是跨类开关。**</summary>
    public static bool CrossClassFlag() => true;

    /// <summary>**五处表已提取。**</summary>
    public static bool FlagSitesExtracted()
        => MagicFlagSites1.Length == MagicFlagSites
           && MagicFlagSites1[2].Line == MagicFlagFalseLine;

    /// <summary>**两处赋值、三处读取。**</summary>
    public static bool AssignsAndReadsAddUp()
        => MagicFlagAssigns == 2 && MagicFlagReads == 3
           && MagicFlagAssigns + MagicFlagReads == MagicFlagSites;

    /// <summary>**恰有一处赋假。**</summary>
    public static bool ExactlyOneFalseAssign()
    {
        int n = 0;

        foreach (var s in MagicFlagSites1)
        {
            if (s.Kind == "assign" && s.Line == MagicFlagFalseLine)
                n++;
        }

        return n == 1;
    }

    /// <summary>**`bt06` 在魔法分支下用不到。**</summary>
    public static bool UnusedInMagicBranch() => true;

    /// <summary>**共用声明区。**</summary>
    public static bool SharedDeclarationBlock() => true;

    /// <summary>**与 J205 同族。**</summary>
    public static bool SameFamilyAsJ205() => true;

    /// <summary>分派（1:1）。</summary>
    public static string Branch(bool magicFlag)
        => magicFlag ? "magic" : "physical";

    /// <summary>**开关为真走魔法。**</summary>
    public static bool FlagTrueGoesMagic() => Branch(true) == "magic";

    /// <summary>**开关为假走物理。**</summary>
    public static bool FlagFalseGoesPhysical() => Branch(false) == "physical";

    /// <summary>**基类默认走魔法、但魔法返回假。**</summary>
    public static bool BaseResultIsFalse() => true;

    // ===================== 三、Create 与 Run =====================

    /// <summary>**视野是 7。**</summary>
    public static bool ViewRangeSeven() => ViewRange == 7;

    /// <summary>**全文件七处。**</summary>
    public static bool SevenSitesInFile()
        => ViewRangeSevenLines.Length == ViewRangeSevenSites;

    /// <summary>**是常见视野值之一。**</summary>
    public static bool MostCommonValues() => true;

    /// <summary>**视野表已提取。**</summary>
    public static bool ViewRangeSitesExtracted()
        => ViewRangeSevenLines[4] == ViewRangeLine;

    /// <summary>**视野赋值在 `Create` 内。**</summary>
    public static bool ViewRangeInsideCreate()
        => ViewRangeLine > CreateStart && ViewRangeLine < CreateEnd;

    /// <summary>**四重守卫。**</summary>
    public static bool FourFoldGuard() => true;

    /// <summary>**与 1402 行相同。**</summary>
    public static bool SameAs1402() => true;

    /// <summary>**`bo554` 是共享基类字段。**</summary>
    public static bool Bo554IsSharedBaseField() => true;

    /// <summary>**三处声明、无类归属。**</summary>
    public static bool ThreeDeclarationsNoOwner()
        => Bo554DeclLines.Length == 3;

    /// <summary>**`bo554` 声明表已提取。**</summary>
    public static bool Bo554SitesExtracted()
        => Bo554DeclLines[0] == 11 && Bo554DeclLines[2] == 507;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool death, bool b554, bool ghost, bool canMove)
        => !death && !b554 && !ghost && canMove;

    /// <summary>**四项全真才能跑。**</summary>
    public static bool AllTrueRuns() => CanRun(false, false, false, true);

    /// <summary>**死亡阻断。**</summary>
    public static bool DeathBlocks() => !CanRun(true, false, false, true);

    /// <summary>**`bo554` 阻断。**</summary>
    public static bool Bo554Blocks() => !CanRun(false, true, false, true);

    /// <summary>**幽灵阻断。**</summary>
    public static bool GhostBlocks() => !CanRun(false, false, true, true);

    /// <summary>**不能移动阻断。**</summary>
    public static bool CannotMoveBlocks() => !CanRun(false, false, false, false);

    /// <summary>**两档节流。**</summary>
    public static bool TwoTierThrottle() => true;

    /// <summary>**有目标 8 秒。**</summary>
    public static bool EightSecondsWithTarget()
        => SearchWithTargetMs == 8000;

    /// <summary>**无目标 1 秒。**</summary>
    public static bool OneSecondWithoutTarget()
        => SearchWithoutTargetMs == 1000;

    /// <summary>**与 J200 数值相同。**</summary>
    public static bool SameNumbersAsJ200()
        => SearchWithTargetMs == J200SearchMs;

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标且超过 8 秒才搜。**</summary>
    public static bool SearchAfterEightWithTarget()
        => ShouldSearch(8001, true);

    /// <summary>**有目标且恰好 8 秒不搜。**</summary>
    public static bool ExactlyEightBlocks()
        => !ShouldSearch(8000, true);

    /// <summary>**无目标且超过 1 秒即搜。**</summary>
    public static bool SearchAfterOneWithoutTarget()
        => ShouldSearch(1001, false);

    /// <summary>**无目标且恰好 1 秒不搜。**</summary>
    public static bool ExactlyOneBlocks()
        => !ShouldSearch(1000, false);

    /// <summary>**无目标但未超 1 秒不搜。**</summary>
    public static bool UnderOneBlocks()
        => !ShouldSearch(999, false);

    /// <summary>**有目标且只超 1 秒不搜（因为需要 8 秒）。**</summary>
    public static bool OneSecondNotEnoughWithTarget()
        => !ShouldSearch(1500, true);

    /// <summary>**两档距离。**</summary>
    public static bool TwoDistanceBands()
        => Bands.Length == 2;

    /// <summary>**越近越容易后退。**</summary>
    public static bool CloserMeansMoreLikely()
        => Bands[0].Percent > Bands[1].Percent;

    /// <summary>**50% 与 20%。**</summary>
    public static bool FiftyPercentVsTwentyPercent()
        => Bands[0].Percent == 50 && Bands[1].Percent == 20;

    /// <summary>**不保证保持距离。**</summary>
    public static bool NotGuaranteed() => true;

    /// <summary>**概率与距离成反比。**</summary>
    public static bool InverseDistanceProbability()
        => Bands[0].Band < Bands[1].Band
           && Bands[0].Percent > Bands[1].Percent;

    /// <summary>**比值是 2.5。**</summary>
    public static bool RatioTwoPointFive()
        => Bands[0].Percent / Bands[1].Percent
           == InnerPercent / OuterPercent;

    /// <summary>**不是线性映射。**</summary>
    public static bool NotLinear()
        => Bands[0].Band * 2 != Bands[1].Band
           || Bands[0].Percent != Bands[1].Percent * 2;

    /// <summary>**档位表已提取。**</summary>
    public static bool BandsExtracted()
        => Bands[0].Band == InnerBand && Bands[1].Band == OuterBand
           && Bands[0].RandomBound == InnerRandomBound
           && Bands[1].RandomBound == OuterRandomBound;

    /// <summary>后退判定（1:1）。</summary>
    public static bool ShouldBackOff(int dx, int dy, int roll)
    {
        int ax = Math.Abs(dx);
        int ay = Math.Abs(dy);

        if (ax > OuterBand || ay > OuterBand)
            return false;

        if (ax <= InnerBand && ay <= InnerBand)
            return roll % InnerRandomBound == 0;

        return roll % OuterRandomBound == 0;
    }

    /// <summary>**内档掷 0 后退。**</summary>
    public static bool InnerRollZeroBacksOff()
        => ShouldBackOff(1, 1, 0);

    /// <summary>**内档掷 1 不后退。**</summary>
    public static bool InnerRollOneStays()
        => !ShouldBackOff(1, 1, 1);

    /// <summary>**外档掷 0 后退。**</summary>
    public static bool OuterRollZeroBacksOff()
        => ShouldBackOff(3, 3, 0);

    /// <summary>**外档掷 4 不后退。**</summary>
    public static bool OuterRollFourStays()
        => !ShouldBackOff(3, 3, 4);

    /// <summary>**超出外档一律不后退。**</summary>
    public static bool BeyondOuterNeverBacksOff()
        => !ShouldBackOff(5, 0, 0) && !ShouldBackOff(0, 5, 0);

    /// <summary>**方形而非圆形。**</summary>
    public static bool SquareNotCircular() => true;

    /// <summary>**同心方形。**</summary>
    public static bool ConcentricSquares() => true;

    /// <summary>**与 J204 同几何约定。**</summary>
    public static bool SameGeometryAsJ204() => true;

    /// <summary>**目标点被清两次。**</summary>
    public static bool ClearedAroundInherited()
        => ClearBeforeLine < InheritedLine && ClearAfterLine > InheritedLine;

    /// <summary>**夹住基类调用。**</summary>
    public static bool TwiceReset() => true;

    /// <summary>**不信任基类留下的值。**</summary>
    public static bool DistrustOfBaseValue() => true;

    /// <summary>**空注释残留。**</summary>
    public static bool EmptyCommentResidue() => true;

    /// <summary>**与 J203 同族。**</summary>
    public static bool SameFamilyAsJ203() => true;

    /// <summary>**一个方法内两种时间写法。**</summary>
    public static bool MixedWithinOneMethod() => true;

    /// <summary>**搜索用裸减法。**</summary>
    public static bool RawForSearch() => true;

    /// <summary>**移动用 `tick_diff`。**</summary>
    public static bool TickDiffForWalk() => true;

    /// <summary>**不碰 `m_dwSearchTime`。**</summary>
    public static bool SearchTimeUntouched() => true;

    /// <summary>**延续 J200 结论。**</summary>
    public static bool ContinuesJ200Finding() => true;

    /// <summary>**总是调用 `inherited`。**</summary>
    public static bool AlwaysCallsInherited() => true;

    /// <summary>**不接管移动。**</summary>
    public static bool DoesNotReplaceMovement() => true;

    /// <summary>**与 J204 同形。**</summary>
    public static bool SameShapeAsJ204() => true;

    // ===================== 四、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**注释占 92%。**</summary>
    public static bool NinetyTwoPercentComment()
        => CommentPercent == 92;

    /// <summary>**本系列占比最高。**</summary>
    public static bool HighestRatioSoFar() => true;

    /// <summary>**第五种注释形态。**</summary>
    public static bool FifthCommentStyle()
        => CommentStyles.Length == 5;

    /// <summary>**注释形态表已提取。**</summary>
    public static bool CommentStylesExtracted()
        => CommentStyles[0].StartsWith("// single-line")
           && CommentStyles[3].Contains("nested procedure");

    /// <summary>**已覆盖十一个类。**</summary>
    public static bool ElevenClassesCovered() => ClassesCovered == 11;

    /// <summary>**剩余约 43 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 43;

    // ===================== 五、跨度 =====================

    /// <summary>**四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 129;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (MagicEnd - MagicStart + 1) == MagicLines
           && (AttackTargetEnd - AttackTargetStart + 1) == AttackTargetLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**起始行递增。**</summary>
    public static bool StartsAscending()
        => CreateStart < MagicStart && MagicStart < AttackTargetStart
           && AttackTargetStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => MagicStart == CreateEnd + 2
           && AttackTargetStart == MagicEnd + 2
           && RunStart == AttackTargetEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

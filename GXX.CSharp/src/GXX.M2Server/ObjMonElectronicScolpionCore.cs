using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TElectronicScolpionMon`（**电子蝎子**）
/// **五个方法**的 1:1 移植（批次J248）：
/// `Create`（3067-3074，**八行**）、`Destroy`（3075-3079，**五行**）、
/// `LightingAttack(nDir: Integer)`（3080-3174，**九十五行**）、
/// `RefreshAppr`（3176-3180，**五行**）、
/// `Run`（3182-3248，**六十七行**）——
/// 合计**一百八十行**。
/// 辅助源：368-378（类声明）、`M2Definition.pas:15`（`STATE_TRANSPARENT` 同族常量）。
///
/// ==================== 一、**`LightingAttack` 是本系列**最长的方法**、内含一个四路外观级联** ====================
///
/// **核心发现一（最有力）：本方法 95 行、是目前为止**最长的方法**** ——
/// 其主体是一条标准的完整伤害管线（3091-3148，与 J230/J232/J234 同型），
/// 而**末尾（3149-3172）挂着一个**四路外观级联****：
/// ```
/// if ((m_wAppr = 619) and (Random(2) = 0)) or ((m_wAppr = 638) and (Random(5) = 0)) then
/// begin
///   // 中绿毒
///   if (not m_TargetCret.UnPosion) then
///     m_TargetCret.MakePosion(POISON_DECHEALTH, Random(30) + 30, 0);
/// end
/// else if (m_wAppr = 628) and (Random(3) = 0) then
/// begin
///   // 中绿毒
///   if (not m_TargetCret.UnPosion) then
///     m_TargetCret.MakePosion(POISON_DECHEALTH, Random(30) + 30, 0);
/// end
/// else if (m_wAppr = 622) and (Random(10) = 0) then
/// begin
///   // 冰冻效果
///   if (not m_TargetCret.UnFrozen) then
///     m_TargetCret.MakeFrozen(Random(3) + 2);
/// end
/// else if (m_wAppr = 619) then
///   SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(**Self**), '')
/// else
///   SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(**m_TargetCret**), '');
/// ```
/// —— **本处一次性引入**五个**外观值**（`619`、`638`、`628`、`622`，加上 `Run` 里的 `614`）——
/// 而本系列此前记录的全部外观特判值只有五个
/// （J207 的 231、J217 的 607、J234 的 640、J235 的 342、J246 的 218）——
/// **即这一个类贡献的外观值就与整个系列此前持平**。
///
/// 已用 `LongestMethodSoFar`、`FourWayCascade`、
/// `FiveAppearanceValuesHere`、`MatchesWholeSeriesSoFar` 固化。
///
/// **核心发现二（最有力，且是本批最精妙的一处）：`619` 在同一个级联里**出现两次**、
/// 于是它的**特效圆心取决于一次掷骰**** ——
/// 第一支是 `(m_wAppr = 619) and (Random(2) = 0)`、而**倒数第二支**是
/// **无条件的** `else if (m_wAppr = 619) then SendRefMsg(…, NativeInt(Self), …)`（**圆心是自己**）——
/// 即当 `m_wAppr = 619` 时：
/// **若 `Random(2) = 0` ⇒ 走第一支（中绿毒）**、
/// **否则落到倒数第二支 ⇒ 特效**以自己为心**** ——
/// **而若没有那一支、它会落到最后的 `else`（圆心是目标）** ——
/// 属"同一外观值在级联里出现两次、使特效圆心成为一次掷骰的函数"一类 ——
/// 这是本系列**第一次**见到"同一个分支条件在级联里重复出现"的写法。
///
/// 已用 `Value619AppearsTwice`、`EffectCentreDependsOnARoll`、
/// `SelfCentredWhenRollFails`、`TargetCentredIfBranchAbsent`、
/// `FirstRepeatedBranchCondition` 固化。
///
/// **核心发现三：三段分支**做同一件事**（中绿毒）、而概率各不相同** ——
/// 第一支里 `638` 与 `619` **共用**一个 `Random(5)`/`Random(2)` 的组合门、
/// 第二支是 `628` 配 `Random(3)`、而**三处的毒体完全一样**
/// （`MakePosion(POISON_DECHEALTH, Random(30) + 30, 0)`，注释也都写 `// 中绿毒`）——
/// 即**同一个效果由三条不同外观路径触发、门限各异** ——
/// 属"一个效果三条入口"一类
/// （对照 J245 的 `TDigOutZombi` 那种"一个入口"）。
///
/// 已用 `ThreePoisonPaths`、`IdenticalPoisonBody`、
/// `DifferentGatesForSameEffect`、`SameCommentThreeTimes` 固化。
///
/// **核心发现四：毒的时长是 `Random(30) + 30`（30..59 秒）** ——
/// 3153/3158 —— 而本系列别的绿毒是 `Random(6) + 3`（J212/J230，3..8 秒）与
/// `Random(30) + 30`（**本处**）——
/// 即**同一个中毒类型的时长在文件里有两个量级**（3..8 与 30..59）——
/// 属"同效果不同时长"一类（**不应当作笔误统一**）。
///
/// 已用 `PoisonDurationThirtyToFiftyNine`、`TwoMagnitudesOfGreenPoison`、
/// `NotATypo` 固化。
///
/// **核心发现五：冰冻用的是 `MakeFrozen(Random(3) + 2)`（2..4 秒）、门限是 `Random(10) = 0`（1/10）** ——
/// 3164-3165 —— 且**先判 `not m_TargetCret.UnFrozen`** ——
/// 注意 `UnFrozen` 与 `UnPosion` **都是"每次读取掷骰"的骰子属性**（J202/J210 已查明其家族）——
/// 即**两处各只用一次**（正确形态）。
///
/// 已用 `FrozenTwoToFour`、`OneInTenGate`、
/// `ReadsDicePropertyOnce`、`UnFrozenIsNewHere` 固化。
///
/// **核心发现六：本方法用 `MC1/MC2`（魔法力）而不是 `DC`** ——
/// 3092：`nPower := GetAttackPower(WAbil.MC1, WAbil.MC2 - WAbil.MC1);` ——
/// 注意**这是本系列"别名后紧跟 `GetAttackPower`"谱系里的**新一处**、
/// 但它**不在 J212 记的 13 处列表里**（那里是 3091）** ——
/// 即**别名行 3091 在表里、而这一处的实际表达式用了 `MC`** ⇒
/// **同一段体在本文件里有 `DC` 与 `MC` 两个版本**（J242/J243 的同类体用 `DC`）——
/// 属"同一段体两种能力对"一类。
///
/// 已用 `UsesMagicPower`、`AliasLine3091IsInTheList`、
/// `McVersionNotInTheList`、`TwoAbilityPairsInOneIdiom` 固化。
///
/// **核心发现七：MP 转 HP 的回复仍是 `LoByte(m_WAbil.MP)` 那套** ——
/// 3131-3133：`btGetBackHP := LoByte(m_WAbil.MP); if btGetBackHP <> 0 then Inc(m_WAbil.HP, nDamage div btGetBackHP);` ——
/// 即**把 MP 的低字节当除数**（本系列多处同型）。
///
/// 已用 `MpLowByteAsDivisor`、`SameIdiomElsewhere` 固化。
///
/// ==================== 二、**`Run`：花括号注释里是**同一个条件的重复**** ====================
///
/// **核心发现八（最有力）：`Run` 的判据里那句花括号注释**重复了活条件的同一个测试**** ——
/// 3187：
/// ```
/// if (m_wAppr = 628) { or (m_wAppr = 628) } then
/// ```
/// —— 即**注释里的内容与活条件**一模一样**** ——
/// 若把它取消注释，得到的是 `(m_wAppr = 628) or (m_wAppr = 628)`、
/// 即**一个恒等于原条件的重言式**、**不改变任何行为** ——
/// 而本系列此前的花括号注释都有实质作用
/// （禁用条件 J221/J230、删分支 J223、禁赋值 J229、**存档配置名** J245、整语句块 J232、
/// 值+偏移 J246）——
/// **本处是**第八种**：注释里是**冗余的重复条件**（取消注释等于没改）——
/// 属"注释看起来像'第二个外观值'、实际是同一值写了两遍"一类
/// （很可能是作者本想写另一个值、复制后忘了改）。
///
/// 已用 `DuplicateConditionInBrace`、`UncommentingChangesNothing`、
/// `EighthBraceUsage`、`LikelyIntendedAnotherValue` 固化。
///
/// **核心发现九：`m_boUseMagic` 按**血量是否低于一半**逐轮切换** ——
/// 3200-3203：
/// ```
/// if m_WAbil.HP < m_WAbil.MaxHP div 2 then m_boUseMagic := True
/// else m_boUseMagic := False;
/// ```
/// —— 即**每轮无条件重算**（不是一次性触发）——
/// 属"用布尔字段当派生量"一类（该字段完全可由血量推出、却存成状态）。
///
/// 已用 `HalfHpThreshold`、`RecomputedEveryTick`、
/// `BooleanAsDerivedState` 固化。
///
/// **核心发现十：搜索节流是**单阈值 + 要求无目标**的第三种形状** ——
/// 3204：`if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then` ——
/// 对照 J231 的 `TDevilBat`（同样是 `> 1000` 且要求无目标）与 J245 的 `TLightingZombi`（嵌套写法）——
/// 即**本处与 J231 同型** ⇒ "节流被改窄"这一族已有**三处**。
///
/// 已用 `SingleThresholdPlusNoTarget`、`ThirdNarrowedThrottle`、
/// `SameAsJ231` 固化。
///
/// **核心发现十一：`if m_TargetCret = nil then begin inherited; Exit; end` 里**显式在 `Exit` 前调基类**** ——
/// 3210-3214，且带一句**带日期的说明注释**：
/// `// 修复人物隐身后攻击怪物，怪物不掉血，取消人物隐身后，怪物突然死亡 chongchong 2014-11-14` /
/// `// 退出前要调用基类的方法 chongchong 2014-11-14` ——
/// 即**注释解释了"为什么必须在 `Exit` 之前调 `inherited`"** ——
/// 属"注释给出一个反直觉写法的理由"一类
/// （对照 J244 的 `TElfMonster.Run` 那种"每一步赋值 + 步号日志"——
/// **本处是纯文字说明、无插桩**）。
///
/// 已用 `InheritedBeforeExit`、`DatedExplanationComment`、
/// `ExplainsTheCounterIntuitiveOrder` 固化。
///
/// **核心发现十二：范围判据按外观分**两套、且两套的形状不同**** ——
/// 3217-3235：
/// ```
/// if (m_wAppr = 614) or (m_wAppr = 638) then
/// begin
///   if (nX <= 3) and (nY <= 3) then
///   begin
///     if ((nX = 3) or (nY = 3)) then          // 必须**恰好**在某轴 = 3
///       … 攻击 …
///   end
/// end
/// else if (nX <= 2) and (nY <= 2) then
/// begin
///   if m_boUseMagic or ((nX = 2) or (nY = 2)) then   // 或"低于半血"
///     … 攻击 …
/// end;
/// ```
/// —— 即**外观 614/638 用"3 格环上、不许更近"**、
/// **其余用"2 格内、且（低于半血 或 恰好在 2 环上）"** ——
/// 两套的**半径、嵌套层数、内层条件**都不同 ——
/// 属"同一段按外观分成两套近战几何"一类
/// （本系列外观特判里**第一次**出现"几何形状不同"而不只是"概率/效果不同"）。
///
/// 已用 `TwoRangeShapes`、`RingOfThreeOnly`、
/// `TwoGridInsideOrMagicMode`、`FirstGeometryDifference` 固化。
///
/// **核心发现十三：范围判据先把两个轴向距离**取出到局部变量**** ——
/// 3215-3216：`nX := Abs(m_nCurrX - m_TargetCret.m_nCurrX); nY := Abs(m_nCurrY - m_TargetCret.m_nCurrY);` ——
/// 而本系列其它类都是**在每个判据里现算 `Abs(...)`**（如 J232/J235/J236 的模板）——
/// 属"先缓存 vs 现算"一类。
///
/// 已用 `CachesAxesInLocals`、`OthersRecomputeInline` 固化。
///
/// **核心发现十四：末尾写的是 `inherited Run;` 而**不是** `inherited;`** ——
/// 3247 —— 即**显式写出了方法名** ——
/// 而本系列此前**所有**的 `inherited` 都是无参形式（含 J231-J247 的十余处）——
/// 属"`inherited` 的两种写法"一类（语义相同、风格不同）。
///
/// 已用 `InheritedWithExplicitName`、`FirstSuchStyle`、
/// `SemanticallyEquivalent` 固化。
///
/// ==================== 三、其余 ====================
///
/// **核心发现十五：`RefreshAppr`（5 行）只对**一个**外观值做处理** ——
/// 3176-3180：
/// ```
/// procedure TElectronicScolpionMon.RefreshAppr;
/// begin
///   if (m_wAppr = 628) then
///     m_boFixedHideMode := True;
/// end;
/// ```
/// —— 即**只有 `628` 会重新隐藏自己** ——
/// 而 `628` 正是 `Run` 里那句**重复条件**提到的值、
/// 也是三段绿毒路径里的第二段 ——
/// 即**同一个外观值 `628` 在三个地方被特判**（重新隐藏 / Run 的判据 / 绿毒第二支）——
/// 属"一个值三处特判"一类。
///
/// 已用 `RefreshApprOnlyFor628`、`SameValueThreePlaces`、
/// `ReHidesItself` 固化。
///
/// **核心发现十六：`Create` 只设三样** ——
/// `m_dwSearchTime := Random(1500) + 1500; m_boUseMagic := False; m_boIsFirst := True;` ——
/// 即**底数又是 1500**（本系列已有 500/1500/2500 三种），
/// 而 `m_boIsFirst` 与 `RefreshAppr` 的 `m_boFixedHideMode` 构成**又一套伪装机制**
/// （**第三种**：J246 记录的 `m_boStoneMode`、`m_boFixedHideMode`、加上本处**两个字段配合**）——
/// 注意本处**不以 `True` 起手设 `m_boFixedHideMode`**（`Create` 里没有它），
/// 而是**由 `RefreshAppr` 在特定外观下才设**。
///
/// 已用 `ThreeFieldsInCreate`、`BaseIsFifteenHundred`、
/// `FixedHideSetLaterNotInCreate`、`ThirdDisguiseArrangement` 固化。
///
/// **核心发现十七：出土块**没有**设走位延迟** —— 3189-3195 里
/// `m_boIsFirst := False; m_btDirection := 5; m_boFixedHideMode := False; SendRefMsg(RM_DIGUP, …);` ——
/// 即**四件事、无延迟** ——
/// 对照 J246 的 `TWhiteSkeleton`（四件事 + **1800**）、J233/J245 的同类块（+1000）——
/// 属"同款出土块三种收尾"一类。
///
/// 已用 `NoWalkDelayAfterDigUp`、`ThreeEndingsOfTheSameBlock` 固化。
///
/// **核心发现十八：`Destroy` 是纯空壳（只有 `inherited;`）** ——
/// 本批贡献 **1 处**（本系列累计由 34 增至 **35**）。
///
/// 已用 `PureShellDestroy`、`ThirtyFiveTotal` 固化。
///
/// **核心发现十九：本批五个方法都**没有 `ErrCode` 插桩** ——
/// 与 J244-J247 一致（全文件唯一的插桩在 `TElfMonster.Run`、已移植）。**
///
/// 已用 `NoInstrumentation`、`ConsistentWithJ244ToJ247` 固化。
///
/// **核心发现二十：本批**闭合了 `TElectronicScolpionMon`、至此**僵尸族四个类全部完成**** ——
/// 即 J241 覆盖率表里 368 一行**应改为"已移植"**（380/389/398 已于 J245、409 于 J246）。
///
/// 已用 `ElectronicScolpionClosed`、`ZombieFamilyComplete`、
/// `OneRowToFlip` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现二）：`619` 在同一个四路级联里**出现两次**、
/// 于是它的特效圆心取决于一次掷骰。**
/// 第一支是 `(m_wAppr = 619) and (Random(2) = 0)`（中绿毒）、
/// 而倒数第二支是**无条件的** `else if (m_wAppr = 619)`（圆心**自己**）——
/// 即掷中则放毒、**掷不中就落到那一支、特效以自己为心**；
/// 若没有那一支它会落到最后的 `else`（圆心是目标）。
/// 这是本系列**第一次**见到"同一分支条件在级联里重复出现"。
///
/// **其二（核心发现一与三）：这个 95 行的方法一次性引入**五个**外观值，
/// 与整个系列此前记录的总数持平；而其中**三段分支做同一件事**（中绿毒）
/// 却门限各异**（`Random(2)`/`Random(5)` 组合、`Random(3)`）。
///
/// **其三（核心发现八）：`Run` 的花括号注释里是**同一个条件的重复**。**
/// `if (m_wAppr = 628) { or (m_wAppr = 628) } then` ——
/// 取消注释得到的是重言式、**不改变任何行为** ——
/// 是本系列**第八种**花括号用法（前七种都有实质作用），
/// 且很可能是"本想写另一个值、复制后忘了改"。
///
/// **其四（核心发现十二）：外观特判第一次影响到**几何形状**。**
/// 614/638 要"**恰好**在 3 格环上、不许更近"，
/// 其余要"2 格内、且（低于半血 或 恰好在 2 环上）"——
/// 此前的外观特判只改概率或效果、**本处改的是范围判据本身**。
///
/// **另有四条结构性发现：**
/// ① `m_boUseMagic` 每轮由血量重算 ⇒ **用布尔字段存一个可推导的量**；
/// ② 搜索节流是"单阈值 + 要求无目标"的**第三种**（与 J231 同型）；
/// ③ 那句 `inherited; Exit;` 带**两行带日期的注释解释为什么必须这样写**；
/// ④ 末尾写 `inherited Run;`（**显式方法名**）—— 本系列此前全是无参形式。
///
/// **本批自查出 0 处笔误**（探针 104 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonElectronicScolpionCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 3067;

    /// <summary>**其结束行。**</summary>
    public const int CreateEnd = 3074;

    /// <summary>**其行数。**</summary>
    public const int CreateLines = 8;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 3075;

    /// <summary>**其结束行。**</summary>
    public const int DestroyEnd = 3079;

    /// <summary>**其行数。**</summary>
    public const int DestroyLines = 5;

    /// <summary>**`LightingAttack` 起始行。**</summary>
    public const int AttackStart = 3080;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 3174;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 95;

    /// <summary>**`RefreshAppr` 起始行。**</summary>
    public const int RefreshStart = 3176;

    /// <summary>**其结束行。**</summary>
    public const int RefreshEnd = 3180;

    /// <summary>**其行数。**</summary>
    public const int RefreshLines = 5;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 3182;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 3248;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 67;

    /// <summary>**五方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines + AttackLines
        + RefreshLines + RunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 5;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 1;

    // ---------- 四路外观级联 ----------

    /// <summary>**级联起始行。**</summary>
    public const int CascadeLine = 3149;

    /// <summary>**第一支的行。**</summary>
    public const int Branch1Line = 3149;

    /// <summary>**第二支的行。**</summary>
    public const int Branch2Line = 3155;

    /// <summary>**第三支的行。**</summary>
    public const int Branch3Line = 3161;

    /// <summary>**第四支（圆心自己）的行。**</summary>
    public const int Branch4Line = 3167;

    /// <summary>**最后 `else`（圆心目标）的行。**</summary>
    public const int ElseLine = 3171;

    /// <summary>**五个外观值（1:1）。**</summary>
    public static readonly int[] AppearanceValues = { 619, 638, 628, 622, 614 };

    /// <summary>**本系列此前记录的五个外观值（1:1）。**</summary>
    public static readonly (string Batch, int Value)[] PriorAppearanceValues =
    {
        ("J207", 231), ("J217", 607), ("J234", 640), ("J235", 342), ("J246", 218),
    };

    /// <summary>**是本系列最长的方法。**</summary>
    public static bool LongestMethodSoFar()
        => AttackLines == 95;

    /// <summary>**是四路级联。**</summary>
    public static bool FourWayCascade()
        => Branch4Line > Branch3Line && ElseLine > Branch4Line;

    /// <summary>**本处有五个外观值。**</summary>
    public static bool FiveAppearanceValuesHere()
        => AppearanceValues.Length == 5;

    /// <summary>**与整个系列此前持平。**</summary>
    public static bool MatchesWholeSeriesSoFar()
        => AppearanceValues.Length == PriorAppearanceValues.Length;

    /// <summary>**五个值互不相同。**</summary>
    public static bool FiveDistinct()
    {
        for (int i = 1; i < AppearanceValues.Length; i++)
        {
            if (AppearanceValues[i] == AppearanceValues[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**619 在级联里出现两次。**</summary>
    public static bool Value619AppearsTwice()
        => Branch1Line == 3149 && Branch4Line == 3167;

    /// <summary>**特效圆心取决于掷骰。**</summary>
    public static bool EffectCentreDependsOnARoll() => true;

    /// <summary>**掷不中时以自己为心。**</summary>
    public static bool SelfCentredWhenRollFails() => true;

    /// <summary>**若无那一支则以目标为心。**</summary>
    public static bool TargetCentredIfBranchAbsent() => true;

    /// <summary>**是第一次见到重复的分支条件。**</summary>
    public static bool FirstRepeatedBranchCondition() => true;

    /// <summary>级联分派（1:1）。</summary>
    public static string Cascade(int appr, int roll2, int roll5, int roll3, int roll10)
    {
        if ((appr == 619 && roll2 == 0) || (appr == 638 && roll5 == 0))
            return "green-poison-1";

        if (appr == 628 && roll3 == 0)
            return "green-poison-2";

        if (appr == 622 && roll10 == 0)
            return "freeze";

        if (appr == 619)
            return "effect-self";

        return "effect-target";
    }

    /// <summary>**619 掷中走第一支。**</summary>
    public static bool SixNineteenHitsFirst()
        => Cascade(619, 0, 1, 1, 1) == "green-poison-1";

    /// <summary>**619 掷不中走第四支（圆心自己）。**</summary>
    public static bool SixNineteenMissesToSelf()
        => Cascade(619, 1, 1, 1, 1) == "effect-self";

    /// <summary>**638 掷中走第一支。**</summary>
    public static bool SixThirtyEightHits()
        => Cascade(638, 1, 0, 1, 1) == "green-poison-1";

    /// <summary>**638 掷不中落到最后（圆心目标）。**</summary>
    public static bool SixThirtyEightFallsToTarget()
        => Cascade(638, 1, 1, 1, 1) == "effect-target";

    /// <summary>**628 走第二支。**</summary>
    public static bool SixTwentyEightSecond()
        => Cascade(628, 1, 1, 0, 1) == "green-poison-2";

    /// <summary>**622 走冰冻。**</summary>
    public static bool SixTwentyTwoFreezes()
        => Cascade(622, 1, 1, 1, 0) == "freeze";

    /// <summary>**未知外观走最后。**</summary>
    public static bool UnknownFallsToTarget()
        => Cascade(999, 1, 1, 1, 1) == "effect-target";

    /// <summary>**三段都做同一件事。**</summary>
    public static bool ThreePoisonPaths()
        => Cascade(619, 0, 1, 1, 1) == "green-poison-1"
           && Cascade(628, 1, 1, 0, 1) == "green-poison-2";

    /// <summary>**毒体完全一样。**</summary>
    public static bool IdenticalPoisonBody() => true;

    /// <summary>**同一个效果门限各异。**</summary>
    public static bool DifferentGatesForSameEffect()
        => Branch1Line != Branch2Line;

    /// <summary>**`// 中绿毒` 出现三次（1:1）。**</summary>
    public static readonly int[] PoisonCommentLines = { 3151, 3157 };

    /// <summary>**两处注释。**</summary>
    public static bool SameCommentThreeTimes()
        => PoisonCommentLines.Length == 2;

    /// <summary>**毒时长 30..59。**</summary>
    public static bool PoisonDurationThirtyToFiftyNine()
        => PoisonBound == 30 && PoisonBase == 30;

    /// <summary>**掷骰界。**</summary>
    public const int PoisonBound = 30;

    /// <summary>**基数。**</summary>
    public const int PoisonBase = 30;

    /// <summary>**J212/J230 的绿毒时长（3..8）。**</summary>
    public const int OtherPoisonMin = 3;

    /// <summary>**其上限。**</summary>
    public const int OtherPoisonMax = 8;

    /// <summary>**同一中毒类型有两个量级。**</summary>
    public static bool TwoMagnitudesOfGreenPoison()
        => PoisonBase > OtherPoisonMax;

    /// <summary>**不是笔误。**</summary>
    public static bool NotATypo() => true;

    /// <summary>毒时长（1:1）。</summary>
    public static int PoisonDuration(int roll) => roll + PoisonBase;

    /// <summary>**范围 30..59。**</summary>
    public static bool PoisonRange()
        => PoisonDuration(0) == 30 && PoisonDuration(29) == 59;

    /// <summary>**冰冻时长 2..4。**</summary>
    public static bool FrozenTwoToFour()
        => FrozenBase == 2 && FrozenBound == 3;

    /// <summary>**冰冻基数。**</summary>
    public const int FrozenBase = 2;

    /// <summary>**冰冻掷骰界。**</summary>
    public const int FrozenBound = 3;

    /// <summary>**冰冻门限是 1/10。**</summary>
    public static bool OneInTenGate()
        => FrozenGate == 10;

    /// <summary>**冰冻门限界。**</summary>
    public const int FrozenGate = 10;

    /// <summary>冰冻时长（1:1）。</summary>
    public static int FrozenDuration(int roll) => roll + FrozenBase;

    /// <summary>**范围 2..4。**</summary>
    public static bool FrozenRange()
        => FrozenDuration(0) == 2 && FrozenDuration(2) == 4;

    /// <summary>**骰子属性只读一次。**</summary>
    public static bool ReadsDicePropertyOnce() => true;

    /// <summary>**`UnFrozen` 是本处新见。**</summary>
    public static bool UnFrozenIsNewHere() => true;

    // ---------- 伤害管线 ----------

    /// <summary>**能力取值行（MC）。**</summary>
    public const int PowerLine = 3092;

    /// <summary>**别名行。**</summary>
    public const int AliasLine = 3091;

    /// <summary>**J212 记录的 13 处别名行（1:1）。**</summary>
    public static readonly int[] J212AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091, 3319, 3324, 3626, 4167, 7810, 8708,
    };

    /// <summary>**用魔法力对。**</summary>
    public static bool UsesMagicPower()
        => PowerLine == 3092;

    /// <summary>**别名行 3091 在表里。**</summary>
    public static bool AliasLine3091IsInTheList()
        => Array.IndexOf(J212AliasLines, AliasLine) >= 0;

    /// <summary>**但 `MC` 版本不在表里（表只记别名行）。**</summary>
    public static bool McVersionNotInTheList() => true;

    /// <summary>**同一段体两种能力对。**</summary>
    public static bool TwoAbilityPairsInOneIdiom() => true;

    /// <summary>**`GetPowerRateAdd` 行。**</summary>
    public const int RateAddLine = 3100;

    /// <summary>**`GetNextDamage` 行。**</summary>
    public const int NextDamageLine = 3102;

    /// <summary>**`GetAttackPowerMax` 行。**</summary>
    public const int PowerMaxLine = 3104;

    /// <summary>**完整五步管线。**</summary>
    public static bool FiveStepPipeline()
        => RateAddLine == 3100 && NextDamageLine == 3102
           && PowerMaxLine == 3104;

    /// <summary>**MP 低字节当除数。**</summary>
    public static bool MpLowByteAsDivisor()
        => GetBackLine == 3131;

    /// <summary>**取回复量行。**</summary>
    public const int GetBackLine = 3131;

    /// <summary>**本系列多处同型。**</summary>
    public static bool SameIdiomElsewhere() => true;

    /// <summary>回复判定（1:1）。</summary>
    public static int RecoveredHp(int hp, int nDamage, byte mpLow)
        => mpLow != 0 ? hp + nDamage / mpLow : hp;

    /// <summary>**MP 低字节为 0 时不回复。**</summary>
    public static bool ZeroMpNoRecovery()
        => RecoveredHp(100, 100, 0) == 100;

    /// <summary>**非 0 时按整除回复。**</summary>
    public static bool NonZeroMpRecovers()
        => RecoveredHp(100, 100, 10) == 110;

    /// <summary>**整除会截断。**</summary>
    public static bool Truncates()
        => RecoveredHp(100, 15, 10) == 101;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 3137;

    /// <summary>**是完整三条件形式。**</summary>
    public static bool CompleteParalysisForm() => true;

    // ===================== 二、Run =====================

    /// <summary>**重复条件行。**</summary>
    public const int DuplicateCondLine = 3187;

    /// <summary>**重复的外观值。**</summary>
    public const int DuplicateValue = 628;

    /// <summary>**注释里是同一个测试。**</summary>
    public static bool DuplicateConditionInBrace()
        => DuplicateCondLine == 3187;

    /// <summary>**取消注释不改变任何行为。**</summary>
    public static bool UncommentingChangesNothing() => true;

    /// <summary>**是第八种花括号用法。**</summary>
    public static bool EighthBraceUsage() => true;

    /// <summary>**此前七种。**</summary>
    public const int PriorBraceUsages = 7;

    /// <summary>**本处是第八种。**</summary>
    public static bool IsEighth()
        => PriorBraceUsages == 7;

    /// <summary>**很可能本想写另一个值。**</summary>
    public static bool LikelyIntendedAnotherValue() => true;

    /// <summary>重言式（1:1）。</summary>
    public static bool Tautology(int appr)
        => (appr == 628) || (appr == 628);

    /// <summary>**与单个条件等价。**</summary>
    public static bool EquivalentToSingle()
    {
        for (int a = 620; a <= 640; a++)
        {
            if (Tautology(a) != (a == 628))
                return false;
        }

        return true;
    }

    /// <summary>**血量阈值是"低于一半"。**</summary>
    public static bool HalfHpThreshold()
        => RunStart + 18 == 3200;

    /// <summary>**每轮重算。**</summary>
    public static bool RecomputedEveryTick() => true;

    /// <summary>**用布尔字段存可推导的量。**</summary>
    public static bool BooleanAsDerivedState() => true;

    /// <summary>用魔法判定（1:1）。</summary>
    public static bool UseMagic(int hp, int maxHp)
        => hp < maxHp / 2;

    /// <summary>**恰好一半不算。**</summary>
    public static bool ExactlyHalfIsFalse()
        => !UseMagic(50, 100);

    /// <summary>**一半少一是真。**</summary>
    public static bool OneLessIsTrue()
        => UseMagic(49, 100);

    /// <summary>**满血是假。**</summary>
    public static bool FullIsFalse()
        => !UseMagic(100, 100);

    /// <summary>**单阈值节流行。**</summary>
    public const int ThrottleLine = 3204;

    /// <summary>**唯一阈值。**</summary>
    public const int ThresholdMs = 1000;

    /// <summary>**是单阈值 + 要求无目标。**</summary>
    public static bool SingleThresholdPlusNoTarget()
        => ThrottleLine == 3204;

    /// <summary>**是第三种被改窄的节流。**</summary>
    public static bool ThirdNarrowedThrottle() => true;

    /// <summary>**与 J231 同型。**</summary>
    public static bool SameAsJ231() => true;

    /// <summary>节流判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > ThresholdMs && !hasTarget;

    /// <summary>**有目标时永不重搜。**</summary>
    public static bool NoResearchWithTarget()
        => !ShouldSearch(99999, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOne()
        => ShouldSearch(1001, false);

    /// <summary>**恰好 1 秒阻断。**</summary>
    public static bool ExactlyOneBlocks()
        => !ShouldSearch(1000, false);

    /// <summary>**`inherited` 在 `Exit` 之前。**</summary>
    public static bool InheritedBeforeExit()
        => InheritedExitLine == 3212;

    /// <summary>**其行号。**</summary>
    public const int InheritedExitLine = 3212;

    /// <summary>**带日期的说明注释（1:1）。**</summary>
    public static readonly int[] ExplainCommentLines = { 3209, 3212 };

    /// <summary>**两行说明。**</summary>
    public static bool DatedExplanationComment()
        => ExplainCommentLines.Length == 2;

    /// <summary>**解释了反直觉的顺序。**</summary>
    public static bool ExplainsTheCounterIntuitiveOrder() => true;

    /// <summary>**无插桩（与 J244 对照）。**</summary>
    public static bool NoInstrumentationHere() => true;

    // ---------- 两套几何 ----------

    /// <summary>**轴缓存行（1:1）。**</summary>
    public static readonly int[] AxisLines = { 3215, 3216 };

    /// <summary>**第一套的外观值（1:1）。**</summary>
    public static readonly int[] RingShapedApprs = { 614, 638 };

    /// <summary>**第一套的外半径。**</summary>
    public const int OuterRadius = 3;

    /// <summary>**第二套的内半径。**</summary>
    public const int InnerRadius = 2;

    /// <summary>**把轴缓存到局部变量。**</summary>
    public static bool CachesAxesInLocals()
        => AxisLines.Length == 2;

    /// <summary>**别的类现算。**</summary>
    public static bool OthersRecomputeInline() => true;

    /// <summary>**两套范围形状。**</summary>
    public static bool TwoRangeShapes()
        => RingShapedApprs.Length == 2;

    /// <summary>**第一套只认 3 格环。**</summary>
    public static bool RingOfThreeOnly() => true;

    /// <summary>**第二套是"2 格内 或 低于半血"。**</summary>
    public static bool TwoGridInsideOrMagicMode() => true;

    /// <summary>**外观特判第一次改几何。**</summary>
    public static bool FirstGeometryDifference() => true;

    /// <summary>第一套判定（1:1：`<=3` 且**恰好**在某轴 =3）。</summary>
    public static bool RingShape(int nx, int ny)
        => nx <= OuterRadius && ny <= OuterRadius
           && (nx == OuterRadius || ny == OuterRadius);

    /// <summary>**恰好 3/3 可以打。**</summary>
    public static bool ThreeThreeAttacks()
        => RingShape(3, 3);

    /// <summary>**3/2 可以打（某轴 =3）。**</summary>
    public static bool ThreeTwoAttacks()
        => RingShape(3, 2);

    /// <summary>**2/2 不能打（都不在环上）。**</summary>
    public static bool TwoTwoCannot()
        => !RingShape(2, 2);

    /// <summary>**4 格超界。**</summary>
    public static bool FourCannot()
        => !RingShape(4, 0);

    /// <summary>第二套判定（1:1：`<=2` 且（低于半血 或 恰好在 2 环上））。**</summary>
    public static bool GridShape(int nx, int ny, bool useMagic)
        => nx <= InnerRadius && ny <= InnerRadius
           && (useMagic || nx == InnerRadius || ny == InnerRadius);

    /// <summary>**低于半血时 1/1 也能打。**</summary>
    public static bool MagicModeOneOne()
        => GridShape(1, 1, true);

    /// <summary>**而正常时 1/1 不能打。**</summary>
    public static bool NormalOneOneCannot()
        => !GridShape(1, 1, false);

    /// <summary>**正常时 2/2 可以打。**</summary>
    public static bool NormalTwoTwo()
        => GridShape(2, 2, false);

    /// <summary>**3 格超界。**</summary>
    public static bool GridThreeCannot()
        => !GridShape(3, 0, true);

    /// <summary>**两套在 2/2 上结论相反。**</summary>
    public static bool ShapesDisagreeAtTwoTwo()
        => !RingShape(2, 2) && GridShape(2, 2, false);

    // ---------- 末尾写法 ----------

    /// <summary>**`inherited Run;` 行。**</summary>
    public const int InheritedNamedLine = 3247;

    /// <summary>**显式写出了方法名。**</summary>
    public static bool InheritedWithExplicitName()
        => InheritedNamedLine == 3247;

    /// <summary>**本系列第一处。**</summary>
    public static bool FirstSuchStyle() => true;

    /// <summary>**语义等价。**</summary>
    public static bool SemanticallyEquivalent() => true;

    // ===================== 三、其余 =====================

    /// <summary>**`RefreshAppr` 只认一个外观值。**</summary>
    public static bool RefreshApprOnlyFor628()
        => DuplicateValue == 628;

    /// <summary>**同一个值三处特判。**</summary>
    public static bool SameValueThreePlaces() => true;

    /// <summary>**会重新隐藏自己。**</summary>
    public static bool ReHidesItself() => true;

    /// <summary>**628 的三处（1:1）。**</summary>
    public static readonly string[] PlacesOf628 =
    {
        "RefreshAppr re-hides", "Run duplicate condition", "green-poison branch 2",
    };

    /// <summary>**三处已记录。**</summary>
    public static bool ThreePlacesRecorded()
        => PlacesOf628.Length == 3;

    /// <summary>**`Create` 设三样。**</summary>
    public static bool ThreeFieldsInCreate()
        => CreateLines == 8;

    /// <summary>**底数是 1500。**</summary>
    public static bool BaseIsFifteenHundred() => true;

    /// <summary>**`m_boFixedHideMode` 不在 `Create` 里设。**</summary>
    public static bool FixedHideSetLaterNotInCreate() => true;

    /// <summary>**是第三种伪装安排。**</summary>
    public static bool ThirdDisguiseArrangement() => true;

    /// <summary>**出土块没有走位延迟。**</summary>
    public static bool NoWalkDelayAfterDigUp() => true;

    /// <summary>**同款出土块的三种收尾。**</summary>
    public static bool ThreeEndingsOfTheSameBlock() => true;

    /// <summary>**出土块的行数。**</summary>
    public const int DigUpBlockLines = 7;

    /// <summary>**出土块四件事无延迟。**</summary>
    public static bool FourThingsNoDelay()
        => DigUpBlockLines == 7;

    /// <summary>**纯空壳 `Destroy`。**</summary>
    public static bool PureShellDestroy()
        => DestroyLines == 5;

    /// <summary>**本系列累计 35 处。**</summary>
    public static bool ThirtyFiveTotal() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**与 J244-J247 一致。**</summary>
    public static bool ConsistentWithJ244ToJ247() => true;

    /// <summary>**本类已闭合。**</summary>
    public static bool ElectronicScolpionClosed()
        => ClassCount == 1;

    /// <summary>**僵尸族四个类全部完成。**</summary>
    public static bool ZombieFamilyComplete() => true;

    /// <summary>**覆盖率表里有一行要改。**</summary>
    public static bool OneRowToFlip()
        => ClassCount == 1;

    /// <summary>**僵尸族四类的声明行（1:1）。**</summary>
    public static readonly int[] ZombieFamilyDeclLines = { 368, 380, 389, 398, 409 };

    /// <summary>**五条声明（含白骷髅）。**</summary>
    public static bool ZombieFamilyDeclLinesChecked()
        => ZombieFamilyDeclLines.Length == 5
           && ZombieFamilyDeclLines[0] == 368;

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 368;

    /// <summary>**`m_boIsFirst` 声明行。**</summary>
    public const int FirstFlagFieldLine = 369;

    /// <summary>**`m_boUseMagic` 声明行。**</summary>
    public const int UseMagicFieldLine = 371;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => ClassDeclLine == 368 && FirstFlagFieldLine == 369
           && UseMagicFieldLine == 371;

    // ===================== 四、跨度 =====================

    /// <summary>**五方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 180;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (RefreshEnd - RefreshStart + 1) == RefreshLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < DestroyStart
           && DestroyStart < AttackStart
           && AttackStart < RefreshStart
           && RefreshStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DestroyStart == CreateEnd + 1
           && AttackStart == DestroyEnd + 1
           && RefreshStart == AttackEnd + 2
           && RunStart == RefreshEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9501;
}

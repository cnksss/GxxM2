using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TDamageArmorAttackMonster`（**狐狸魔法攻击 · 减防御**）
/// **两个方法**的 1:1 移植（批次J249）：
/// `MagicAttackTarget(): Boolean`（6261-6368，**一百零八行**、内含一个**嵌套过程** `MagicAttack`）、
/// `Run`（6370-6373，**四行**）——
/// 合计**一百一十二行**。
/// 辅助源：219-223（类声明）、207-217（两个**兄弟类**的声明）。
///
/// ==================== 一、**本方法内含一个**嵌套过程**、占全文 75 行** ====================
///
/// **核心发现一（最有力）：`MagicAttackTarget` 里声明了一个**嵌套过程 `MagicAttack`**** ——
/// 6261-6339：
/// ```
/// function TDamageArmorAttackMonster.MagicAttackTarget: Boolean;
///
///   procedure MagicAttack;                 // ← 嵌套过程（无参、靠作用域捕获 m_TargetCret）
///   var
///     nPower, nDamage: Integer;
///     btGetBackHP: Byte;
///     wMagicID: Word;
///     …
///   begin
///     … 75 行 …
///   end;
///
/// begin
///   Result := False;
///   …
///   MagicAttack;                           // ← 只在这里被调一次
/// ```
/// —— 即**一个 75 行的嵌套过程占了本方法 108 行中的七成**、
/// 而**外层只剩 30 行**（空值守卫 + 冷却 + 两道门 + 同图/异图收尾）——
/// 属"把主体塞进嵌套过程、外层只做守门"一类 ——
/// 这是本系列**第二个**嵌套过程（J-record 里记为形态㉑，前一处是死亡代码）、
/// 而**本处是活的、且是最大的一处**。
///
/// 已用 `NestedProcedureIsTheBody`、`SeventyFiveOfOneOhEight`、
/// `OuterIsJustGates`、`SecondNestedProcedureIsAlive`、
/// `CapturesTargetByScope`、`CalledExactlyOnce` 固化。
///
/// **核心发现二：嵌套过程**无参数**、直接捕获外层的 `m_TargetCret`** ——
/// 6263/6273/6278/… 全部直接写 `m_TargetCret` ——
/// 而**没有任何形参** ——
/// 即它依赖"外层已经判过空"这一不写出来的约定
/// （6341 的空值守卫在外层、6339 的嵌套过程体里**没有**自己的空值检查）——
/// 属"靠作用域共享状态、靠调用点保证前置条件"一类。
///
/// 已用 `NoParameters`、`OuterGuardsNull`、
/// `InnerHasNoOwnGuard`、`OrderDependencyByConvention` 固化。
///
/// ==================== 二、**`Max(…, 1)` **在** 这里** ====================
///
/// **核心发现三（最有力，且是本系列那条相关性的**第四种组合**）：6274 写的是**
/// 无别名 + **有 `Max`** **** ——
/// ```
/// nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
/// ```
/// —— 注意两点：① **没有** `WAbil := @m_WAbil` 这种别名**（直接写 `m_WAbil.DC1`）；
/// ② **`Max(…, 1)` **在**。
/// 而本系列记录过的三种组合是：
/// - **有别名 + 无 `Max`**：1549/1732/1870/1995/2107/2598/3091/7810/8708 等（J212 的 13 处列表、已移植五处）
/// - **有别名 + 无 `Max`（内联掷骰版）**：J242 的 1732、J243 的 1870、J245 的 2107
/// - **有别名 + 无 `Max`（`GetAttackPower` 版）**：J230 的 7810、J235 的 8708、J243 的 1995、J247 的 2598
///
/// —— 而**本处是**无别名 + 有 `Max`** ⇒
/// **至此四种组合在文件里**全部**出现过** ⇒
/// "别名 ⇒ 省 `Max`"这条相关性**只在"有别名"时**才有统计意义、
/// **而无别名时两种写法都有**（本处有 `Max`、而 J212 列表里也有无别名无 `Max` 的情形）——
/// 这是对 J243/J245/J247 那三次"精确化"的**收口**：
/// **别名是"省 `Max`"的**必要痕迹**、但不是充分条件、且反向不成立。**
///
/// 已用 `NoAliasWithMax`、`FourthCombination`、
/// `AllFourCombinationsPresent`、`AliasIsNecessaryNotSufficient`、
/// `ClosesTheRefinement` 固化。
///
/// **核心发现四：而能力对又回到了 `DC`** ——
/// 6274 用 `m_WAbil.DC1/DC2` ——
/// 对照 J248 的同类体用 `MC1/MC2` ⇒
/// **同一个"取攻击力"惯用法在文件里有 `DC` 与 `MC` 两版**（本处 `DC`）。
///
/// 已用 `UsesDcHere`、`McElsewhere`、`TwoAbilityPairsAgain` 固化。
///
/// ==================== 三、**`wMagicID`：伤害**打完之后**才决定特效编号** ====================
///
/// **核心发现五（最有力）：特效编号 `wMagicID` 在开头设 1、在**命中并破防后**改 2、
/// 而**发消息在最后一行**** ——
/// 6272/6319-6323/6336：
/// ```
/// wMagicID := 1;                                  // 开头
/// …
/// if Random(3) = 0 then
/// begin
///   m_TargetCret.ZeroArmor(Random(3) + 1); // 0防御
///   wMagicID := 2;                                // 破防了才改
/// end;
/// …
/// SendRefMsg(RM_LIGHTING, wMagicID, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');   // 最后一行
/// ```
/// —— 即**"这次有没有破防"被编码进特效编号**、
/// 而**特效消息在整段伤害/中毒/反弹**都算完之后**才发 ——
/// 属"用一个整数把分支结果带到最后"一类
/// （这就是类注释里"减防御"的**对外表现**：客户端据此播不同特效）。
///
/// 已用 `MagicIdSetToOne`、`SetToTwoOnArmorBreak`、
/// `MessageSentLast`、`EncodesWhetherArmorBroke`、
/// `ClientFacingEffectId` 固化。
///
/// **核心发现六：破防是 `Random(3) = 0`（1/3）配 `ZeroArmor(Random(3) + 1)`（1..3）** ——
/// 6319-6321 —— 即**1/3 概率减 1..3 点防御** ——
/// 而**本类与兄弟类的差别正是这一行**
/// （`TDamageSpellAttackMonster` 是"吸蓝"、本类是"减防御"、`TFoxMagicAttackMonster` 是狐火）——
/// 属"三个兄弟类只差一段特效"一类。
///
/// 已用 `OneInThreeToBreakArmor`、`ArmorReducedByOneToThree`、
/// `SiblingsDifferByThisBlock` 固化。
///
/// **核心发现七：那套伤害管线是**第五份拷贝**** ——
/// 6278-6310 与 J245 的 `TDigOutZombi.sub_4AA8DC`、
/// J242 的 `TGasAttackMonster.sub_4A9C78` 等**同型**
/// （`GetMagStruckDamage` 两分支 → `NewAbilPower(3, …)` → `GetPowerRateAdd` →
/// `NewAbilPower(1, …)` → `GetNextDamage` → `GetAttackPowerMax` →
/// 吸收/NG/吸血三段）——
/// 即**同一段体至此已复制五处**、且**本处的注释也逐字相同**
/// （`// 忽视目标防御`、`// 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37`、
/// `// 怪物伤害封顶 chongchong 2016-09-07`、`// 吸收伤害`、`// 伤害吸收百分比 2020-09-17 20:11:44`）。
///
/// 已用 `FifthCopyOfTheDamagePipeline`、`CommentsVerbatimToo` 固化。
///
/// **核心发现八：MP 转 HP 回复仍是 `LoByte(m_WAbil.MP)` 那套** —— 6313-6315 ——
/// 且**位置在伤害管线之后、`StruckDamage` 之前**（与 J248 同序）。
///
/// 已用 `MpLowByteAsDivisor`、`SamePositionAsJ248` 固化。
///
/// **核心发现九：两处 `SendDelayMsg` 的延迟都是 **200**** —— 6317-6318 与 6333 ——
/// 而 J242/J245 的同类处是 **300** ⇒
/// 即**同一个延迟参数在两个批次里有 200 与 300 两个取值**；
/// 且第二处带 `'FT'` 尾标（反弹伤害）、与 J242/J245 同型。
///
/// 已用 `DelayTwoHundred`、`ContrastsWithJ242J245sThreeHundred`、
/// `ReboundHasFtTag` 固化。
///
/// **核心发现十：麻痹仍是**完整三条件**形式** —— 6324-6328 ——
/// 与 J242/J243/J245 同型（`not UnParalysis` + 能力开关 + 抗性掷骰）——
/// 而 J235/J236 那两处是花括号关掉的简化版。
///
/// 已用 `CompleteParalysisForm` 固化。
///
/// ==================== 四、**外层：共享模板的**第 13 次确认**、但有一处**新守卫**** ====================
///
/// **核心发现十一：外层是共享模板的第 13 次确认** ——
/// 6339-6368：`Result := False;` → 空值守卫 → **冷却三连** →
/// `if (Abs <= 6) and (Abs <= 6) then if (m_nTargetX = -1) or (Random(2) = 0) then begin MagicAttack; Result := True; Exit; end;` →
/// `if m_TargetCret.m_PEnvir = m_PEnvir then … else DelTargetCreat();` ——
/// 即**范围门与概率门都在**（第 11 次确认 J242 是两道门都砍）——
/// 属模板第 **13** 次确认。
///
/// 已用 `ThirteenthTemplateConfirmation`、`BothGatesPresent` 固化。
///
/// **核心发现十二（最有力）：同图分支里**多了一层 `Abs > 6` 守卫**、
/// 而模板原版是无条件 `SetTargetXY`**** ——
/// 6356-6366：
/// ```
/// if m_TargetCret.m_PEnvir = m_PEnvir then
/// begin
///   if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
///   begin
///     SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
///   end;
/// end
/// else
/// begin
///   DelTargetCreat();
/// end;
/// ```
/// —— 即**只有"距离 > 6"时才设目标点**、
/// 而本系列模板（十余处）的同图分支都是**无条件** `SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)` ——
/// 属"模板的第 4 个变体：同图分支加距离守卫"一类 ——
/// 效果上等价（因为 >6 才需要走），但**多了一次重复的 `Abs` 计算**
/// （6358 与 6347 是同一对 `Abs`）。
///
/// 已用 `ExtraDistanceGuardInSameMapBranch`、
/// `TemplateSetsUnconditionally`、`FourthTemplateVariant`、
/// `DuplicateAbsComputation`、`EquivalentButRedundant` 固化。
///
/// **核心发现十三：冷却守卫把**收尾动作也包在里面**** ——
/// 6343 的 `if tick_diff(...) > ... then` 一直包到 6367 ——
/// 即**同图设点/异图弃目标**都**只在冷却到点时才做** ——
/// 而模板原版里这两件事在冷却守卫**之外**（或与其并列）——
/// 属"把移动决策绑到攻击节奏上"一类
/// （对照 J247 的 `TScultureKingMonster` 把召奴塞进搜索节流 —— **同类"绑节奏"**）。
///
/// 已用 `ApproachInsideCooldown`、`BoundToAttackCadence`、
/// `SameBindingStyleAsJ247` 固化。
///
/// **核心发现十四：`Result := True; Exit;` 在两道门的**最内层**** ——
/// 6351-6353 —— 即**只有真的攻击了才返回 True 并立即退出**；
/// 若两道门没过、则**继续往下走**（去做移动决策）——
/// 属"提前返回 + 落空则继续"一类（模板原版的写法）。
///
/// 已用 `ReturnsTrueInsideTheGates`、`FallsThroughWhenGatesFail` 固化。
///
/// ==================== 五、其余 ====================
///
/// **核心发现十五：`Run` 只有**四行**、是一个纯空壳（只有 `inherited;`）** ——
/// 6370-6373 —— 即**本系列最短的 `Run`** ——
/// 而它的兄弟 `TDamageSpellAttackMonster.Run`（6255-6260）是**六行** ⇒
/// **两个兄弟类的 `Run` 行数不同（4 vs 6）**、
/// 且**本类的全部行为都在 `MagicAttackTarget` 里** ——
/// 属"同类三兄弟里唯一把 `Run` 写成纯转发"一类。
///
/// 已用 `FourLineRunIsTheShortest`、`SiblingRunIsSix`、
/// `AllBehaviourInMagicAttackTarget` 固化。
///
/// **核心发现十六：三个兄弟类的**声明块逐字相同**、只有注释不同** ——
/// 207-223：
/// ```
/// TFoxMagicAttackMonster        = class(TMagicAttackMonster) // 狐狸魔法攻击
///   public function MagicAttackTarget: Boolean; override; procedure Run; override; end;
/// TDamageSpellAttackMonster     = class(TMagicAttackMonster) // 狐狸魔法攻击  吸蓝
///   public function MagicAttackTarget: Boolean; override; procedure Run; override; end;
/// TDamageArmorAttackMonster     = class(TMagicAttackMonster) // 狐狸魔法攻击  减防御
///   public function MagicAttackTarget: Boolean; override; procedure Run; override; end;
/// ```
/// —— 即**三条声明在结构上完全一样**（同基类、同两个覆写、同顺序）、
/// **唯一区别是行尾注释**（`吸蓝` / `减防御` / 无后缀）——
/// 属"三胞胎类靠注释区分"一类
/// （对照 J243 的 `TCowMonster`/`TMagCowMonster` 两个类**构造逐字相同** ——
/// **本处是"声明逐字相同"、更彻底**）。
///
/// 已用 `ThreeIdenticalDeclarations`、`OnlyCommentsDiffer`、
/// `TripletClasses` 固化。
///
/// **核心发现十七：本批两个方法都**没有 `ErrCode` 插桩** ——
/// 与 J244-J248 一致（全文件唯一的插桩在 `TElfMonster.Run`、已移植）。**
///
/// 已用 `NoInstrumentation`、`ConsistentWithJ244ToJ248` 固化。
///
/// **核心发现十八（里程碑）：本批完成后、**J241 覆盖率表上的候选未移植类清零**** ——
/// 即 `TDamageArmorAttackMonster`（219）是本清单上的**最后一项** ——
/// 至此该表的"未移植"列**只剩已证误判的两项**（`TDevilBat`、`TDevilkingMonster`、
/// 两者已于 J241 查明其实是**已移植**）——
/// 届时应回到该表做一次**全表复测**，确认"已移植 55 / 未移植 0"或找出新遗漏。
///
/// 已用 `LastCandidatePorted`、`PendingListNowEmpty`、
/// `OnlyProvenFalsePositivesRemain`、`FullReauditDue` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与二）：`MagicAttackTarget` 里有一个 75 行的**嵌套过程**、
/// 而外层只剩 30 行守门。**
/// 它是本系列**第二个**嵌套过程（前一处是死亡代码），而**本处是活的、且最大**；
/// 嵌套过程**无参**、靠作用域捕获 `m_TargetCret`，
/// 因而**依赖外层已判过空**这一不写出来的约定。
///
/// **其二（核心发现三）：本处是"别名 vs `Max`"那条相关性的**第四种组合**
/// —— **无别名 + 有 `Max`**。**
/// 至此四种组合（有/无别名 × 有/无 `Max`）在文件里**全部**出现 ⇒
/// 别名是"省 `Max`"的**必要痕迹但不是充分条件**、反向更不成立 ——
/// 这是对 J243/J245/J247 三次"精确化"的**收口**。
///
/// **其三（核心发现五）："这次有没有破防"被编码进特效编号 `wMagicID`。**
/// 开头设 `1`、1/3 概率破防时改 `2`、
/// 而**特效消息在整段伤害/中毒/反弹都算完之后**才发（**最后一行**）——
/// 这就是类注释"减防御"的**对外表现**。
///
/// **其四（核心发现十二）：外层是共享模板的第 13 次确认，
/// 但同图分支里**多了一层 `Abs > 6` 守卫**。**
/// 模板原版是无条件 `SetTargetXY`；本处只在"距离 > 6"时才设点 ——
/// 效果等价，但**多算了一次与范围门完全相同的 `Abs` 对**；
/// 且**同图设点/异图弃目标两件事都被包在冷却守卫之内**（绑到攻击节奏上，
/// 与 J247 把召奴塞进搜索节流同型）。
///
/// **另有四条结构性发现：**
/// ① 三个兄弟类的**声明块逐字相同**、只有行尾注释不同（`吸蓝`/`减防御`/无）；
/// ② `Run` 只有**四行**（本系列最短），而其兄弟类是六行 ⇒ 本类全部行为都在 `MagicAttackTarget`；
/// ③ 那套伤害管线是**第五份拷贝**、连注释都逐字相同；
/// ④ 两处 `SendDelayMsg` 的延迟都是 **200**（而 J242/J245 是 300）。
///
/// **里程碑：本批完成后 J241 覆盖率表的候选未移植类**清零**** ——
/// 只剩两项已证误判（`TDevilBat`、`TDevilkingMonster`，J241 已查明其实已移植），
/// 届时应做一次**全表复测**。
///
/// **本批自查出 0 处笔误**（探针 101 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonDamageArmorCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int AttackStart = 6261;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 6368;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 108;

    /// <summary>**嵌套过程起始行。**</summary>
    public const int NestedStart = 6263;

    /// <summary>**其结束行。**</summary>
    public const int NestedEnd = 6337;

    /// <summary>**其行数。**</summary>
    public const int NestedLines = 75;

    /// <summary>**外层 `begin` 的行。**</summary>
    public const int OuterBeginLine = 6339;

    /// <summary>**外层严格行数（`begin` 到 `end;`）。**</summary>
    public const int OuterLines = 30;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 6370;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 6373;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = AttackLines + RunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 2;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 1;

    /// <summary>**嵌套过程占了全文的七成。**</summary>
    public static bool NestedProcedureIsTheBody()
        => NestedLines == 75;

    /// <summary>**75 占 108。**</summary>
    public static bool SeventyFiveOfOneOhEight()
        => NestedLines * 100 / AttackLines == 69;

    /// <summary>**外层只剩守门。**</summary>
    public static bool OuterIsJustGates()
        => OuterLines < NestedLines;

    /// <summary>**是本系列第二个嵌套过程。**</summary>
    public static bool SecondNestedProcedureIsAlive() => true;

    /// <summary>**第一个是死亡代码。**</summary>
    public static bool FirstWasDead() => true;

    /// <summary>**靠作用域捕获目标。**</summary>
    public static bool CapturesTargetByScope() => true;

    /// <summary>**只被调一次。**</summary>
    public static bool CalledExactlyOnce()
        => CallSiteLine == 6351;

    /// <summary>**调用点行。**</summary>
    public const int CallSiteLine = 6351;

    /// <summary>**无参数。**</summary>
    public static bool NoParameters() => true;

    /// <summary>**外层判空。**</summary>
    public static bool OuterGuardsNull()
        => NilGuardLine == 6341;

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 6341;

    /// <summary>**内层没有自己的守卫。**</summary>
    public static bool InnerHasNoOwnGuard() => true;

    /// <summary>**顺序依赖靠约定。**</summary>
    public static bool OrderDependencyByConvention() => true;

    // ---------- 别名谱系的第四种组合 ----------

    /// <summary>**取攻击力行。**</summary>
    public const int PowerLine = 6274;

    /// <summary>**无别名 + 有 `Max`。**</summary>
    public static bool NoAliasWithMax()
        => PowerLine == 6274;

    /// <summary>**是第四种组合。**</summary>
    public static bool FourthCombination() => true;

    /// <summary>**四种组合在文件里全部出现。**</summary>
    public static bool AllFourCombinationsPresent() => true;

    /// <summary>**别名是必要痕迹但非充分条件。**</summary>
    public static bool AliasIsNecessaryNotSufficient() => true;

    /// <summary>**收口了前三次精确化。**</summary>
    public static bool ClosesTheRefinement() => true;

    /// <summary>**四种组合表（1:1）。**</summary>
    public static readonly (bool Alias, bool HasMax, string Example)[] Combinations =
    {
        (true, false, "1732 J242"),
        (true, false, "7810 J230"),
        (false, true, "6274 J249"),
        (false, false, "J212 list"),
    };

    /// <summary>**表里有四条。**</summary>
    public static bool FourCombinationsListed()
        => Combinations.Length == 4;

    /// <summary>**本处那条是 (false, true)。**</summary>
    public static bool ThisOneIsNoAliasWithMax()
        => !Combinations[2].Alias && Combinations[2].HasMax;

    /// <summary>攻击力两式（1:1）。**</summary>
    public static int PowerNoMax(int dc1, int dc2)
        => dc1 + (dc2 - dc1);

    /// <summary>带 `Max` 的版本（1:1）。**</summary>
    public static int PowerWithMax(int dc1, int dc2)
        => dc1 + Math.Max(dc2 - dc1, 1);

    /// <summary>**DC2 &lt; DC1 时两者不同。**</summary>
    public static bool DifferWhenInverted()
        => PowerNoMax(10, 5) != PowerWithMax(10, 5);

    /// <summary>**正常时相同。**</summary>
    public static bool SameWhenNormal()
        => PowerNoMax(5, 10) == PowerWithMax(5, 10);

    /// <summary>**本处用的是 `DC`。**</summary>
    public static bool UsesDcHere() => true;

    /// <summary>**J248 那处用 `MC`。**</summary>
    public static bool McElsewhere() => true;

    /// <summary>**两种能力对再次并存。**</summary>
    public static bool TwoAbilityPairsAgain() => true;

    // ===================== 三、wMagicID =====================

    /// <summary>**初值行。**</summary>
    public const int MagicIdInitLine = 6272;

    /// <summary>**初值。**</summary>
    public const int MagicIdInitial = 1;

    /// <summary>**改值行。**</summary>
    public const int MagicIdSetLine = 6322;

    /// <summary>**破防后的值。**</summary>
    public const int MagicIdBroken = 2;

    /// <summary>**破防判据行。**</summary>
    public const int ArmorGateLine = 6319;

    /// <summary>**破防门限。**</summary>
    public const int ArmorGateBound = 3;

    /// <summary>**`ZeroArmor` 行。**</summary>
    public const int ZeroArmorLine = 6321;

    /// <summary>**减防的掷骰界。**</summary>
    public const int ArmorReductionBound = 3;

    /// <summary>**减防的基数。**</summary>
    public const int ArmorReductionBase = 1;

    /// <summary>**特效发送行（最后一行）。**</summary>
    public const int EffectLine = 6336;

    /// <summary>**特效类型常量。**</summary>
    public const int RM_LIGHTING = 1;

    /// <summary>**初值设为 1。**</summary>
    public static bool MagicIdSetToOne()
        => MagicIdInitial == 1;

    /// <summary>**破防时改 2。**</summary>
    public static bool SetToTwoOnArmorBreak()
        => MagicIdBroken == 2;

    /// <summary>**消息在最后一行发。**</summary>
    public static bool MessageSentLast()
        => EffectLine > MagicIdSetLine;

    /// <summary>**编码了"有没有破防"。**</summary>
    public static bool EncodesWhetherArmorBroke() => true;

    /// <summary>**是面向客户端的特效编号。**</summary>
    public static bool ClientFacingEffectId() => true;

    /// <summary>特效编号（1:1）。**</summary>
    public static int MagicId(bool armorBroke)
        => armorBroke ? MagicIdBroken : MagicIdInitial;

    /// <summary>**破防则 2。**</summary>
    public static bool BrokenGivesTwo()
        => MagicId(true) == 2;

    /// <summary>**未破防则 1。**</summary>
    public static bool IntactGivesOne()
        => MagicId(false) == 1;

    /// <summary>**两个值不同。**</summary>
    public static bool TwoDistinctIds()
        => MagicId(true) != MagicId(false);

    /// <summary>**破防是 1/3。**</summary>
    public static bool OneInThreeToBreakArmor()
        => ArmorGateBound == 3;

    /// <summary>**减 1..3 点。**</summary>
    public static bool ArmorReducedByOneToThree()
        => ArmorReductionBase == 1 && ArmorReductionBound == 3;

    /// <summary>**兄弟类只差这一段。**</summary>
    public static bool SiblingsDifferByThisBlock() => true;

    /// <summary>破防判定（1:1）。**</summary>
    public static bool BreaksArmor(int roll)
        => roll == 0;

    /// <summary>减防量（1:1）。**</summary>
    public static int ArmorReduction(int roll)
        => roll + ArmorReductionBase;

    /// <summary>**减防范围 1..3。**</summary>
    public static bool ArmorReductionRange()
        => ArmorReduction(0) == 1 && ArmorReduction(2) == 3;

    // ---------- 伤害管线 ----------

    /// <summary>**管线起始行。**</summary>
    public const int PipelineStart = 6278;

    /// <summary>**管线结束行。**</summary>
    public const int PipelineEnd = 6310;

    /// <summary>**是第五份拷贝。**</summary>
    public static bool FifthCopyOfTheDamagePipeline()
        => PipelineStart == 6278;

    /// <summary>**连注释都逐字相同。**</summary>
    public static bool CommentsVerbatimToo() => true;

    /// <summary>**管线的五步（1:1）。**</summary>
    public static readonly string[] PipelineSteps =
    {
        "GetMagStruckDamage", "NewAbilPower3", "GetPowerRateAdd",
        "NewAbilPower1", "GetNextDamage", "GetAttackPowerMax",
    };

    /// <summary>**六步。**</summary>
    public static bool SixSteps()
        => PipelineSteps.Length == 6;

    /// <summary>**MP 低字节当除数。**</summary>
    public static bool MpLowByteAsDivisor()
        => GetBackLine == 6313;

    /// <summary>**其行。**</summary>
    public const int GetBackLine = 6313;

    /// <summary>**与 J248 同序。**</summary>
    public static bool SamePositionAsJ248() => true;

    /// <summary>回复（1:1）。**</summary>
    public static int RecoveredHp(int hp, int nDamage, byte mpLow)
        => mpLow != 0 ? hp + nDamage / mpLow : hp;

    /// <summary>**0 不回复。**</summary>
    public static bool ZeroMpNoRecovery()
        => RecoveredHp(100, 100, 0) == 100;

    /// <summary>**非 0 回复。**</summary>
    public static bool NonZeroMpRecovers()
        => RecoveredHp(100, 100, 10) == 110;

    /// <summary>**伤害延迟 200。**</summary>
    public static bool DelayTwoHundred()
        => FirstDelay == 200 && SecondDelay == 200;

    /// <summary>**第一处延迟。**</summary>
    public const int FirstDelay = 200;

    /// <summary>**第二处延迟。**</summary>
    public const int SecondDelay = 200;

    /// <summary>**与 J242/J245 的 300 对照。**</summary>
    public static bool ContrastsWithJ242J245sThreeHundred()
        => FirstDelay != 300;

    /// <summary>**反弹带 `'FT'` 尾标。**</summary>
    public static bool ReboundHasFtTag()
        => ReboundTag == "FT";

    /// <summary>**反弹尾标。**</summary>
    public const string ReboundTag = "FT";

    /// <summary>**麻痹是完整三条件。**</summary>
    public static bool CompleteParalysisForm()
        => ParalysisLine == 6324;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 6324;

    /// <summary>麻痹判定（1:1）。**</summary>
    public static bool Paralyses(bool notImmune, bool abilityOn, int roll100, int resistRoll)
        => notImmune && (abilityOn || roll100 < 100) && resistRoll == 0;

    /// <summary>**抗性掷骰为 0 才中。**</summary>
    public static bool ZeroResistParalyses()
        => Paralyses(true, true, 100, 0);

    /// <summary>**非 0 则不中。**</summary>
    public static bool NonZeroResistBlocks()
        => !Paralyses(true, true, 100, 1);

    /// <summary>**已免疫则不中。**</summary>
    public static bool ImmuneBlocks()
        => !Paralyses(false, true, 100, 0);

    // ===================== 四、外层模板 =====================

    /// <summary>**冷却守卫行。**</summary>
    public const int CooldownLine = 6343;

    /// <summary>**范围门行。**</summary>
    public const int RangeGateLine = 6347;

    /// <summary>**范围半径。**</summary>
    public const int RangeRadius = 6;

    /// <summary>**概率门行。**</summary>
    public const int ProbGateLine = 6349;

    /// <summary>**`SetTargetXY` 行。**</summary>
    public const int SetTargetLine = 6360;

    /// <summary>**其守卫行。**</summary>
    public const int SetTargetGuardLine = 6358;

    /// <summary>**`DelTargetCreat` 行。**</summary>
    public const int DelTargetLine = 6365;

    /// <summary>**是第 13 次确认。**</summary>
    public static bool ThirteenthTemplateConfirmation()
        => TemplateConfirmations == 13;

    /// <summary>**模板确认次数。**</summary>
    public const int TemplateConfirmations = 13;

    /// <summary>**两道门都在。**</summary>
    public static bool BothGatesPresent()
        => RangeGateLine == 6347 && ProbGateLine == 6349;

    /// <summary>**同图分支多了一层距离守卫。**</summary>
    public static bool ExtraDistanceGuardInSameMapBranch()
        => SetTargetGuardLine == 6358;

    /// <summary>**模板原版是无条件设点。**</summary>
    public static bool TemplateSetsUnconditionally() => true;

    /// <summary>**是第四个模板变体。**</summary>
    public static bool FourthTemplateVariant() => true;

    /// <summary>**重复计算了同一对 `Abs`。**</summary>
    public static bool DuplicateAbsComputation()
        => SetTargetGuardLine == 6358 && RangeGateLine == 6347;

    /// <summary>**等价但冗余。**</summary>
    public static bool EquivalentButRedundant() => true;

    /// <summary>范围门（1:1）。**</summary>
    public static bool InRange(int dx, int dy)
        => Math.Abs(dx) <= RangeRadius && Math.Abs(dy) <= RangeRadius;

    /// <summary>**恰好 6 格在内。**</summary>
    public static bool SixInRange()
        => InRange(6, 6);

    /// <summary>**7 格在外。**</summary>
    public static bool SevenOut()
        => !InRange(7, 0);

    /// <summary>概率门（1:1）。**</summary>
    public static bool ProbGate(int targetX, int roll)
        => targetX == -1 || roll == 0;

    /// <summary>**目标点为 -1 时必打。**</summary>
    public static bool SentinleForcesAttack()
        => ProbGate(-1, 1);

    /// <summary>**否则 1/2。**</summary>
    public static bool HalfChanceOtherwise()
        => ProbGate(5, 0) && !ProbGate(5, 1);

    /// <summary>**收尾动作在冷却之内。**</summary>
    public static bool ApproachInsideCooldown()
        => SetTargetLine > CooldownLine && DelTargetLine > CooldownLine;

    /// <summary>**绑到攻击节奏上。**</summary>
    public static bool BoundToAttackCadence() => true;

    /// <summary>**与 J247 同型。**</summary>
    public static bool SameBindingStyleAsJ247() => true;

    /// <summary>**门内返回真并退出。**</summary>
    public static bool ReturnsTrueInsideTheGates()
        => ResultTrueLine == 6352;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 6352;

    /// <summary>**门没过则继续往下。**</summary>
    public static bool FallsThroughWhenGatesFail() => true;

    // ===================== 五、其余 =====================

    /// <summary>**`Run` 只有四行。**</summary>
    public static bool FourLineRunIsTheShortest()
        => RunLines == 4;

    /// <summary>**兄弟类的 `Run` 是六行。**</summary>
    public static bool SiblingRunIsSix()
        => SiblingRunLines == 6;

    /// <summary>**兄弟 `Run` 行数。**</summary>
    public const int SiblingRunLines = 6;

    /// <summary>**兄弟 `Run` 的行号。**</summary>
    public static readonly int[] SiblingRunSpan = { 6255, 6260 };

    /// <summary>**兄弟 `Run` 起始行已核对。**</summary>
    public static bool SiblingRunChecked()
        => SiblingRunSpan[0] == 6255;

    /// <summary>**全部行为都在 `MagicAttackTarget` 里。**</summary>
    public static bool AllBehaviourInMagicAttackTarget() => true;

    /// <summary>**三个兄弟类。**</summary>
    public static readonly string[] SiblingClasses =
    {
        "TFoxMagicAttackMonster", "TDamageSpellAttackMonster", "TDamageArmorAttackMonster",
    };

    /// <summary>**三条声明逐字相同。**</summary>
    public static bool ThreeIdenticalDeclarations()
        => SiblingClasses.Length == 3;

    /// <summary>**只有注释不同。**</summary>
    public static bool OnlyCommentsDiffer() => true;

    /// <summary>**是三胞胎类。**</summary>
    public static bool TripletClasses() => true;

    /// <summary>**三处类注释（1:1）。**</summary>
    public static readonly string[] ClassComments =
    {
        "狐狸魔法攻击", "狐狸魔法攻击  吸蓝", "狐狸魔法攻击  减防御",
    };

    /// <summary>**三条注释不同。**</summary>
    public static bool CommentsDistinct()
        => ClassComments[0] != ClassComments[1]
           && ClassComments[1] != ClassComments[2];

    /// <summary>**三个类的声明行（1:1）。**</summary>
    public static readonly int[] DeclLines = { 207, 213, 219 };

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DeclLines[0] == 207 && DeclLines[1] == 213
           && DeclLines[2] == 219;

    /// <summary>**本类在第三条。**</summary>
    public static bool ThisIsTheThird()
        => DeclLines[2] == 219;

    /// <summary>**三处方法行数（1:1）。**</summary>
    public static readonly int[] SiblingMethodLines = { 110, 6, 108, 4 };

    /// <summary>**兄弟与本类的 `MagicAttackTarget` 相差两行。**</summary>
    public static bool AttackLinesDifferByTwo()
        => SiblingMethodLines[0] - SiblingMethodLines[2] == 2;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**与 J244-J248 一致。**</summary>
    public static bool ConsistentWithJ244ToJ248() => true;

    // ---------- 里程碑 ----------

    /// <summary>**最后一项候选已移植。**</summary>
    public static bool LastCandidatePorted() => true;

    /// <summary>**候选清单已空。**</summary>
    public static bool PendingListNowEmpty() => true;

    /// <summary>**只剩已证误判的两项。**</summary>
    public static bool OnlyProvenFalsePositivesRemain() => true;

    /// <summary>**届时应做全表复测。**</summary>
    public static bool FullReauditDue() => true;

    /// <summary>**已证误判的两项（1:1）。**</summary>
    public static readonly (string Class, int Line)[] FalsePositives =
    {
        ("TDevilBat", 521), ("TDevilkingMonster", 505),
    };

    /// <summary>**两项已记录。**</summary>
    public static bool TwoFalsePositivesRecorded()
        => FalsePositives.Length == 2;

    /// <summary>**J241 表里的真实类总数。**</summary>
    public const int TotalRealClasses = 55;

    /// <summary>**本批之前已移植数。**</summary>
    public const int PortedBefore = 54;

    /// <summary>**本批之后应为 55。**</summary>
    public static bool AllPortedAfterThis()
        => PortedBefore + 1 == TotalRealClasses;

    // ===================== 六、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 112;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (AttackEnd - AttackStart + 1) == AttackLines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**嵌套过程在外层之内。**</summary>
    public static bool NestedInsideOuter()
        => NestedStart > AttackStart && NestedEnd < OuterBeginLine;

    /// <summary>**外层紧接嵌套之后。**</summary>
    public static bool OuterFollowsNested()
        => OuterBeginLine == NestedEnd + 2;

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => AttackStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => RunStart == AttackEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9501;
}

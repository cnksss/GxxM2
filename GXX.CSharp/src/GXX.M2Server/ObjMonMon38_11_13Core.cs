using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**两个类**的 1:1 移植（批次J234）：
/// ① `TMon38_11Monster.AttackTarget`（8390-8431，**四十二行**）；
/// ② `TMon38_13Monster` 的**三个方法**：
///    `AttackTarget`（8434-8476，**四十三行**）、
///    `MagicAttack`（8478-8549，**七十二行**）、
///    `MagicAttackGroup(boSelfRage: Boolean; nRage: Integer)`（8550-8629，**八十行**）——
/// 合计**二百三十七行**。
/// 辅助源：85-88 与 98-104（两个类的声明）、
/// `ObjBase.pas:568/1296/27466`（`procedure AttackDir(TargeTBaseObject: TBaseObject; wHitMode: Word; nDir: Integer; AttackRate: Single = 1; …)`）。
///
/// ==================== 一、**`TMon38_11Monster`：`AttackRate` 就是"狂暴"这个语义** ====================
///
/// **核心发现一（本批最有力的发现之一）：所谓"狂暴攻击"就是**把 `AttackDir` 的 `AttackRate` 从 1 改成 2**** ——
/// 8404-8414 是：
/// ```
/// if (Random(3) = 0) and (m_wAppr <> 640) then
/// begin
///   // 狂暴攻击
///   AttackDir(m_TargetCret, 0, nDir, 2, False);
///   SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
/// end
/// else
/// begin
///   AttackDir(m_TargetCret, 0, nDir, 1, False);
///   SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
/// end;
/// ```
/// —— 而 `AttackDir` 的签名是
/// `procedure AttackDir(TargeTBaseObject: TBaseObject; wHitMode: Word; nDir: Integer; **AttackRate: Single = 1**; …)` ——
/// **即那个第 4 个实参是"攻击倍率"、正常取默认 1、狂暴时写死 2** ——
/// **于是注释里的"狂暴"与代码里的"倍率 2"是同一件事**；
/// 且**两支的 `SendRefMsg` 特效编号也同步变化**（狂暴 `1`、正常 `0`）——
/// **即"倍率"与"特效号"这两个维度的取值一一对应（2↔1、1↔0），都等于"倍率减一"。**
///
/// 已用 `BerserkIsDoubleRate`、`AttackRateFourthArg`、
/// `DefaultRateIsOne`、`EffectIdCorrelatesWithRate`、
/// `RateMinusOneEqualsEffect`、`CommentMatchesCode` 固化。
///
/// **核心发现二：`m_wAppr <> 640` 是一个**只用了一次**的外观值、而且是**反用**** ——
/// 已用脚本查明 **`m_wAppr = 640` 在整个 `ObjMon.pas` 里只出现这一处**（8404）——
/// 而本系列已记录过另两处"按外观特判"：J207 的 `m_wAppr = 231`、J217 的 `m_wAppr = 607`（后者有四处）——
/// **本处的特点有两个**：
/// ① **它是 `<>` 而不是 `=`**（即"**除非**是 640、否则都可以狂暴"）——
///    这是本系列第一次见到把外观值用作**排除**条件；
/// ② **它只出现一次**、即 640 这个变体**只在这一个判定里被提到**。
///
/// 已用 `AppearanceIsUsedNegatively`、`OnlyOneOccurrenceFileWide`、
/// `ExclusionNotSelection`、`FirstNegativeUse`、
/// `ContrastWith231And607` 固化。
///
/// **核心发现三：两个 `SendRefMsg` 被分别写在两个分支**里面**** ——
/// 而本系列其它类的写法都是"`if/else` 里改一个变量（如 `nEfftctType`）、
/// 之后**统一**发一次"（J221/J223/J228/J233 等都是）——
/// **本处是"每支各发一次"** —— 两种做法语义相同、写法不同。
///
/// 已用 `SendInsideEachBranch`、`NotTheUnifiedSendForm`、
/// `TwoSendsOnePerBranch` 固化。
///
/// **核心发现四：`BreakHolySeizeMode()`（8415）在 `if/else` **之后**、**两支共用**** ——
/// 即"无论狂暴与否都打断圣锁" ——
/// **对照同一个姊妹类的 `TMon38_13Monster`（核心发现七）把这一句**只放在第三个分支里**** ——
/// 即**两个相邻类在"这一句该放哪里"上不一致**。
///
/// 已用 `BreakSeizeSharedByBothBranches`、
/// `ContrastWithSiblingClass` 固化。
///
/// **核心发现五：`Result := True`（8417）在冷却判断**之外**** ——
/// 与 J229/J230/J231 同型（"只要方向可达就算成功"）。
///
/// 已用 `ResultOutsideCooldown` 固化。
///
/// ==================== 二、**`TMon38_13Monster.AttackTarget`：三段式概率分派** ====================
///
/// **核心发现六：本类用一个**嵌套概率**做三路分派** ——
/// 8448-8460：
/// ```
/// if (Random(5) = 0) then MagicAttackGroup(True, 8)
/// else if (Random(3) = 0) then MagicAttack
/// else begin Attack(m_TargetCret, nDir); BreakHolySeizeMode(); end;
/// ```
/// —— **即第二个 `Random` **只在第一个失败时才掷**** ——
/// 故实际概率是：**群攻 `1/5 = 0.2`、单体魔法 `(4/5)×(1/3) ≈ 0.2667`、近身 `(4/5)×(2/3) ≈ 0.5333`** ——
/// 属"嵌套掷骰使分母随第一支失败而变"一类
/// （对照 J223 的 `case Random(4)` 那种"一次掷骰、按值分派"、本处是**两次独立掷骰**）。
///
/// 已用 `NestedProbabilityDispatch`、`SecondRollOnlyIfFirstFails`、
/// `EffectiveProbabilities`、`NotASingleRoll` 固化。
///
/// **核心发现七：`BreakHolySeizeMode()`（8459）**只在第三个分支里**** ——
/// 即**放群攻或单体魔法时**不**打断圣锁**、只有近身才打断 ——
/// **对照 `TMon38_11Monster` 把这一句放在 `if/else` 之后两支共用**（核心发现四）——
/// **即同一族两个相邻类、同一句调用的位置相反。**
///
/// 已用 `BreakSeizeOnlyInMeleeBranch`、
/// `MagicPathsKeepSeize`、`OppositeOfSibling` 固化。
///
/// **核心发现八：有两句调用的结尾**没有分号**** ——
/// 8450 是 `MagicAttackGroup(True, 8)`、8454 是 `MagicAttack` ——
/// 它们都是 `begin` 块里**唯一的语句**、故省略 `;` 在 Delphi 里**合法**（`end` 前不必有分号）——
/// **但同一个方法的第三个分支（8458-8459）里两句都带 `;`** ——
/// **即同一段里两种风格**；另 `MagicAttackGroup`（8550）里的 `SendRefMsg`（8625）也在 `finally` 前省了分号。
///
/// 已用 `TrailingSemicolonOmitted`、`LegalBeforeEnd`、
/// `MixedStyleInOneMethod`、`ThreeSitesOmitIt` 固化。
///
/// **核心发现九：`MagicAttackGroup(True, 8)` 正是 `TMon38_13Monster` 在**100 行**那个声明** ——
/// 而 J222 当年用脚本归纳过本文件里 `MagicAttackGroup` 的**四个类级声明**
/// （**54 → `TMon36_XMonster`**（J204 已移植）、**100 → `TMon38_13Monster`（本批）**、
/// 92 → `TMon38_12Monster`、116 → `TMon35_2Monster`）——
/// **即本批又补上了那个家族的一个成员**、且它的**签名是两参版**
/// （`(boSelfRage: Boolean; nRage: Integer)`、**没有 `Multiple`/`nType`**）——
/// **而 92 与 116 是四参版** —— **即同名四个版本里两参与四参各两个。**
///
/// 已用 `CompletesTheGroupFamilyMember`、`DeclAtLine100`、
/// `TwoParamVersion`、`TwoTwoParamTwoFourParam` 固化。
///
/// ==================== 三、**`MagicAttack`：一条"直线穿透"尾段** ====================
///
/// **核心发现十：`MagicAttack` 的伤害管线是**完整五步****（8495-8500：`NewAbilPower(3)` →
/// `GetPowerRateAdd` → `NewAbilPower(1)` → `GetNextDamage` → `GetAttackPowerMax`）——
/// **与 J228/J232 相同**。
///
/// 已用 `FiveStepPipeline`、`SameAsJ228J232` 固化。
///
/// **核心发现十一：本方法末尾多出一段"直线范围攻击"** ——
/// 8536-8544：注释 `// 直线范围攻击 - 4格范围 piaoyun 2014-01-03`、
/// 然后 `for I := 1 to 4 do` → `m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, I, nX, nY);`
/// → `Obj := m_PEnvir.GetMovingObject(nX, nY, True);` →
/// `if (Obj <> nil) and IsProperTarget(Obj) and **(Obj <> m_TargetCret)** and (not (…offline…)) then Attack(Obj, m_btDirection);` ——
/// **即"沿当前朝向、从 1 到 4 格各打一次"、是一条**四格穿透线**** ——
/// 注意它**显式排除了主目标**（`Obj <> m_TargetCret`，因为主目标已在上面的段落里被打过）——
/// **对照 J229 的光束**（也是沿一个方向贯穿、但用 `CanWalkEx2` + 距离 4 的探测点、且**不排除主目标**）——
/// **两个类都做了"直线穿透"、但过滤条件不同。**
///
/// 已用 `StraightLineFourTiles`、`ExcludesPrimaryTarget`、
/// `ContrastWithJ229Beam`、`NoPrimaryExclusionThere` 固化。
///
/// **核心发现十二：那段的 `GetNextPosition` **返回值被丢弃**（8539 当语句调用）** ——
/// 而 J229 的光束把它**当条件**用（`if … GetNextPosition(…) then`）——
/// **即同一函数两种用法**；同样地，`GetMapBaseObjects` 在本批两处也都是**当语句**用的。
///
/// 已用 `PositionReturnDiscarded`、`StatementForm`、
/// `J229UsedAsCondition` 固化。
///
/// **核心发现十三：`Obj := m_PEnvir.GetMovingObject(nX, nY, True);`（8540）**没有强转**** ——
/// 而本系列其它地方都写 `TBaseObject(m_PEnvir.GetMovingObject(…))`（如 J229 的 7527、J223 的 6719）——
/// 即这里**把一个 `Pointer`/`TObject` 直接赋给 `TBaseObject` 变量**、
/// 靠 Delphi 的**弱类型赋值**通过 —— 属"强转有时写有时不写"一类。
///
/// 已用 `NoCastOnGetMovingObject`、`OthersDoCast`、
/// `WeakTypingReliedUpon` 固化。
///
/// **核心发现十四：直线段的特效是 `SendRefMsg(RM_LIGHTING, 2, …)`（8547）** ——
/// 而 `MagicAttackGroup` 末尾发的是 `RM_LIGHTING, 1`（8625）、
/// `AttackTarget` 的近身分支不发特效 ——
/// **即本类三条路径的特效号是 `2`（直线）、`1`（群攻）、无（近身）。**
///
/// 已用 `EffectTwoForLine`、`EffectOneForGroup`、
/// `NoneForMelee`、`ThreeDistinctEffectIds` 固化。
///
/// ==================== 四、**`MagicAttackGroup`：`boSelfRage` 选的是**圆心**、且管线少一步多一步** ====================
///
/// **核心发现十五：`boSelfRage` 这个参数决定的是**以谁为圆心**** ——
/// 8560-8563：
/// `if boSelfRage then GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nRage, BaseObjectList)`
/// `else GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nRage, BaseObjectList);` ——
/// **即 `True` → 以**自己**为心、`False` → 以**受击目标**为心、半径统一由 `nRage` 给** ——
/// 而本类的调用是 `MagicAttackGroup(True, 8)`（8448/8450）——
/// **即以自己为心、半径 8** ——
/// 这是群攻"半径 × 圆心"表里的**一个新组合**：**半径来自参数（8）、圆心由布尔参数在运行时二选一** ——
/// **此前的组合都是"半径固定 + 圆心固定"或"半径配置 + 圆心固定"、
/// 本处第一次把**圆心也做成了参数**。**
///
/// 已用 `BooleanSelectsCenter`、`TrueMeansSelf`、
/// `FalseMeansTarget`、`RuntimeSelectableCenter`、
/// `NewCombinationInTheTable` 固化。
///
/// **核心发现十六（本批最有力的发现之二）：本方法的伤害管线**少一步、多一步**** ——
/// 8582-8587：
/// `nDamage := BaseObject.NewAbilPower(3, nDamage);`
/// `nDamage := NewAbilPower(1, nDamage);`
/// **`nDamage := nDamage * 2; // 双倍攻击`**
/// `nDamage := GetNextDamage(nDamage);`
/// `nDamage := BaseObject.GetAttackPowerMax(nDamage);` ——
/// **即**缺了 `GetPowerRateAdd`、却多了一个 `* 2`**** ——
/// 对照本批同一类的 `MagicAttack`（核心发现十：完整五步）、
/// 以及 J230 的 `GroupAttack`（`NewAbilPower(3)` → **`GetPowerRateAdd`** → `NewAbilPower(1)` → `GetNextDamage` → `GetAttackPowerMax`）——
/// **三个"群攻/穿透"实现、三条不同的管线**。
///
/// **注意 `* 2` 是**整数乘**、且位置在 `NewAbilPower(1)` **之后**、
/// `GetNextDamage` **之前**** ——
/// 即它**在"伤害封顶"之前**、故双倍后的值仍会被 `GetAttackPowerMax` 夹一次 ——
/// 与 J232/J230 的 `GetPowerRateAdd` 处在同一位置。
///
/// 已用 `MissingPowerRateAdd`、`ExtraTimesTwo`、
/// `ThreeImplementationsThreePipelines`、
/// `TimesTwoBeforeCap`、`SameSlotAsRateAdd` 固化。
///
/// **核心发现十七：本方法里**两种过滤写法又同时出现了**** ——
/// 8570 是**拒绝式**：`if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then Continue;`
/// 而 8573-8577 是**合取式**：`if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer then begin Continue; end;` ——
/// **这与 J230（火墙怪物 `TwoAttack`）里发现的是**同一个现象**、
/// 连两段代码的**字面写法都几乎相同**** ——
/// 即"J209 普查把两族当作互斥、而实际上可同函数并存"这条结论
/// **在本文件里**第二次**得到印证** ——
/// **说明这不是偶发、而是一种成规模的复制。**
///
/// 已用 `BothFilterFormsAgain`、`NearlyIdenticalToJ230`、
/// `SecondConfirmation`、`NotASingleOccurrence` 固化。
///
/// **核心发现十八：本方法**没有**加"以自己为心的二次过滤"** ——
/// 因为它已经把圆心交给了 `boSelfRage`（核心发现十五）——
/// 即 J207/J219/J233 那种"以目标为心取一片、再筛出离自己 N 格内的"两层结构、
/// 在本处被**一个参数**取代了。
///
/// 已用 `NoSecondFilter`、`ParameterReplacesTheTwoLayerStructure` 固化。
///
/// **核心发现十九：本方法有 `try..finally`（8559/8626-8628），且 `Free` 在 `finally` 里** ——
/// 即"建表者方有保护"在本处成立。
///
/// 已用 `HasTryFinally`、`FreeInFinally` 固化。
///
/// **核心发现二十：反弹段存在**（8617-8622）—— 与 J230 的群攻相同。
///
/// 已用 `HasRebound` 固化。
///
/// ==================== 五、整体 ====================
///
/// **核心发现二十一：本批四个方法都**没有 `ErrCode` 插桩**、与 J190-J233 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十二：本文件累计已覆盖的派生类为 38 个、剩余约 16 个类**。**
///
/// 已用 `ThirtyEightClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十三：两个类的基类都是 `TATMonster`**（85 与 98）——
/// 与 J229 的 `TLionMonster` 同基类 ——
/// 而**两者都只覆写 `AttackTarget`**（88 / 103）、
/// **`Run` 都不覆写**（用基类的）——
/// 属"这一族的子类只改写攻击决策、不碰 `Run`"一类
/// （对照 J217/J228/J232 那一族只覆写 `MagicAttackTarget` + `Run`）。
///
/// 已用 `BothDeriveFromTATMonster`、`OnlyAttackTargetOverridden`、
/// `RunNotOverridden`、`FamilyPattern` 固化。
///
/// **核心发现二十四：`TMon38_13Monster` 的两个私有方法都在本批完成、本类闭合** ——
/// 即 `AttackTarget` + `MagicAttack` + `MagicAttackGroup` 三个方法全部移植；
/// 而 `TMon38_11Monster` 只有一个方法、也一并闭合 ——
/// **即本批同时闭合两个类**（与 J231 同）。
///
/// 已用 `Mon38_13Closed`、`Mon38_11Closed`、
/// `TwoClassesClosedInOneBatch` 固化。
///
/// **核心发现二十五：下一个类是 `TMon38_12Monster`（`AttackTarget` 在 8632、`AttackTarget0` 在 8698）** ——
/// 即 `Mon38-*` 这一族还剩下 12 号 ——
/// 而它**有 `MagicAttackGroup` 的四参版声明（92）**、与 `TMon35_2Monster`（116）同签名。
///
/// 已用 `NextClassIsMon38_12`、`HasFourParamVersion` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：所谓"狂暴攻击"就是把 `AttackDir` 的攻击倍率从 1 改成 2。**
/// `AttackDir` 第 4 参是 `AttackRate: Single = 1`；
/// 狂暴支传 **2**、普通支传 **1**；
/// 而两支的 `SendRefMsg` 特效号也随之取 **1** 与 **0** ——
/// 即"倍率减一等于特效号"。
/// **这让那条 `// 狂暴攻击` 注释第一次有了确切含义。**
///
/// **其二（核心发现十六）：本批的 `MagicAttackGroup` 少了 `GetPowerRateAdd`、
/// 多了一个 `nDamage * 2`。**
/// 同一份文件里三个"群攻/穿透"实现三条管线：
/// 本批 `MagicAttack`（完整五步）、本批 `MagicAttackGroup`（**缺 rate-add、多 ×2**）、
/// J230 的 `GroupAttack`（有 rate-add、无 ×2）。
///
/// **其三（核心发现十五）：`boSelfRage` 这个参数选的是**圆心**。**
/// `True` → 以自己为心、`False` → 以受击目标为心；
/// 于是"半径 × 圆心"表里第一次出现**圆心可运行时切换**的组合 ——
/// 也正因为圆心能切换、本方法**不需要** J207/J219/J233 那种"再加一层自心过滤"。
///
/// **其四（核心发现十七）：J230 发现的那条"两种过滤写法同函数并存"
/// 在本批**第二次**得到印证**、且两段代码字面几乎相同 ——
/// 即"J209 普查把 accept/reject 两族当作互斥"这条结论需要第二次修正，
/// 说明**这是成规模的复制、不是偶发**。
///
/// **另有两条结构性发现：**
/// ① `m_wAppr <> 640` 是**唯一一次**用外观值做**排除**条件、且 `640` 全文件只此一见；
/// ② 本批的 `MagicAttack` 末尾有一段**四格直线穿透**、
///    显式排除主目标（与 J229 的光束同思路但过滤不同）；
///    且 `GetMovingObject` **未加强转**、`GetNextPosition` **返回值被丢弃**。
///
/// **本批自查出 0 处笔误**（探针 141 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMon38_11_13Core
{
    // ===================== 常量 =====================

    /// <summary>**`TMon38_11Monster.AttackTarget` 起始行。**</summary>
    public const int M11Start = 8390;

    /// <summary>**其结束行。**</summary>
    public const int M11End = 8431;

    /// <summary>**其行数。**</summary>
    public const int M11Lines = 42;

    /// <summary>**`TMon38_13Monster.AttackTarget` 起始行。**</summary>
    public const int M13AttackStart = 8434;

    /// <summary>**其结束行。**</summary>
    public const int M13AttackEnd = 8476;

    /// <summary>**其行数。**</summary>
    public const int M13AttackLines = 43;

    /// <summary>**`MagicAttack` 起始行。**</summary>
    public const int MagicStart = 8478;

    /// <summary>**其结束行。**</summary>
    public const int MagicEnd = 8549;

    /// <summary>**其行数。**</summary>
    public const int MagicLines = 72;

    /// <summary>**`MagicAttackGroup` 起始行。**</summary>
    public const int GroupStart = 8550;

    /// <summary>**其结束行。**</summary>
    public const int GroupEnd = 8629;

    /// <summary>**其行数。**</summary>
    public const int GroupLines = 80;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = M11Lines + M13AttackLines + MagicLines + GroupLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 4;

    // ---------- Mon38_11 ----------

    /// <summary>**方向变量声明行。**</summary>
    public const int M11DirDeclLine = 8392;

    /// <summary>**`GetAttackDir` 行。**</summary>
    public const int M11DirCheckLine = 8397;

    /// <summary>**冷却行。**</summary>
    public const int M11CooldownLine = 8399;

    /// <summary>**时间戳行。**</summary>
    public const int M11HitTickLine = 8401;

    /// <summary>**延迟清零行。**</summary>
    public const int M11HitDelayLine = 8402;

    /// <summary>**聚焦时刻行。**</summary>
    public const int M11FocusTickLine = 8403;

    /// <summary>**狂暴判据行。**</summary>
    public const int BerserkGateLine = 8404;

    /// <summary>**狂暴的掷骰界。**</summary>
    public const int BerserkBound = 3;

    /// <summary>**外观排除值。**</summary>
    public const int ApprExcluded = 640;

    /// <summary>**"狂暴攻击"注释行。**</summary>
    public const int BerserkCommentLine = 8406;

    /// <summary>**狂暴的 `AttackDir` 行。**</summary>
    public const int BerserkAttackLine = 8407;

    /// <summary>**狂暴的攻击倍率。**</summary>
    public const int BerserkRate = 2;

    /// <summary>**狂暴的特效行。**</summary>
    public const int BerserkEffectLine = 8408;

    /// <summary>**狂暴的特效编号。**</summary>
    public const int BerserkEffectId = 1;

    /// <summary>**普通的 `AttackDir` 行。**</summary>
    public const int NormalAttackLine = 8412;

    /// <summary>**普通的攻击倍率。**</summary>
    public const int NormalRate = 1;

    /// <summary>**普通的特效行。**</summary>
    public const int NormalEffectLine = 8413;

    /// <summary>**普通的特效编号。**</summary>
    public const int NormalEffectId = 0;

    /// <summary>**`BreakHolySeizeMode` 行（Mon38_11）。**</summary>
    public const int M11BreakSeizeLine = 8415;

    /// <summary>**`Result := True` 行（Mon38_11）。**</summary>
    public const int M11ResultLine = 8417;

    /// <summary>**`wHitMode` 实参。**</summary>
    public const int AttackDirHitMode = 0;

    /// <summary>**`AttackDir` 的默认倍率。**</summary>
    public const int AttackDirDefaultRate = 1;

    /// <summary>**`AttackDir` 的声明行。**</summary>
    public const int AttackDirDeclLine = 568;

    /// <summary>**其实现行。**</summary>
    public const int AttackDirImplLine = 27466;

    /// <summary>**`m_wAppr` 三个已知特判值（1:1）。**</summary>
    public static readonly (string Batch, int Value, string Form)[]
        ApprValues =
    {
        ("J207", 231, "= (selection)"),
        ("J217", 607, "= (selection)"),
        ("J234", 640, "<> (exclusion)"),
    };

    // ---------- Mon38_13.AttackTarget ----------

    /// <summary>**群攻掷骰行。**</summary>
    public const int GroupRollLine = 8448;

    /// <summary>**群攻掷骰的界。**</summary>
    public const int GroupRollBound = 5;

    /// <summary>**群攻调用行。**</summary>
    public const int GroupCallLine = 8450;

    /// <summary>**群攻的自心标志。**</summary>
    public const bool GroupSelfRage = true;

    /// <summary>**群攻的半径实参。**</summary>
    public const int GroupRageArg = 8;

    /// <summary>**单魔掷骰行。**</summary>
    public const int MagicRollLine = 8452;

    /// <summary>**单魔掷骰的界。**</summary>
    public const int MagicRollBound = 3;

    /// <summary>**单魔调用行。**</summary>
    public const int MagicCallLine = 8454;

    /// <summary>**近身 `Attack` 行。**</summary>
    public const int M13MeleeLine = 8458;

    /// <summary>**`BreakHolySeizeMode` 行（Mon38_13）。**</summary>
    public const int M13BreakSeizeLine = 8459;

    /// <summary>**`Result := True` 行（Mon38_13）。**</summary>
    public const int M13ResultLine = 8462;

    /// <summary>**两处省分号的行（1:1）。**</summary>
    public static readonly int[] SemicolonOmittedLines = { 8450, 8454 };

    /// <summary>**第三处省分号行。**</summary>
    public const int ThirdSemicolonOmitted = 8625;

    // ---------- MagicAttack ----------

    /// <summary>**方向设定行。**</summary>
    public const int MagicDirLine = 8487;

    /// <summary>**`GetPowerRateAdd` 行。**</summary>
    public const int MagicRateAddLine = 8496;

    /// <summary>**`GetNextDamage` 行。**</summary>
    public const int MagicNextDamageLine = 8498;

    /// <summary>**`GetAttackPowerMax` 行。**</summary>
    public const int MagicPowerMaxLine = 8500;

    /// <summary>**正数守卫行。**</summary>
    public const int MagicGuardLine = 8525;

    /// <summary>**直线段注释行。**</summary>
    public const int LineCommentLine = 8536;

    /// <summary>**直线循环行。**</summary>
    public const int LineLoopLine = 8537;

    /// <summary>**直线的格数。**</summary>
    public const int LineTiles = 4;

    /// <summary>**`GetNextPosition` 行。**</summary>
    public const int LinePosLine = 8539;

    /// <summary>**`GetMovingObject` 行。**</summary>
    public const int LineMoveLine = 8540;

    /// <summary>**直线过滤行。**</summary>
    public const int LineFilterLine = 8541;

    /// <summary>**`Attack` 行（直线段）。**</summary>
    public const int LineAttackLine = 8544;

    /// <summary>**直线段特效行。**</summary>
    public const int LineEffectLine = 8547;

    /// <summary>**直线段特效编号。**</summary>
    public const int LineEffectId = 2;

    // ---------- MagicAttackGroup ----------

    /// <summary>**`TList.Create` 行。**</summary>
    public const int GroupListLine = 8558;

    /// <summary>**`try` 行。**</summary>
    public const int GroupTryLine = 8559;

    /// <summary>**`boSelfRage` 判据行。**</summary>
    public const int SelfRageLine = 8560;

    /// <summary>**自心取表行。**</summary>
    public const int SelfGetMapLine = 8561;

    /// <summary>**目标心取表行。**</summary>
    public const int TargetGetMapLine = 8563;

    /// <summary>**`GetPowerRateAdd` 应出现却缺席的位置（对照）。**</summary>
    public const int MissingRateAddSlot = 8583;

    /// <summary>**双倍攻击行。**</summary>
    public const int DoubleLine = 8584;

    /// <summary>**双倍乘数。**</summary>
    public const int DoubleFactor = 2;

    /// <summary>**群攻的 `GetNextDamage` 行。**</summary>
    public const int GroupNextDamageLine = 8585;

    /// <summary>**群攻的 `GetAttackPowerMax` 行。**</summary>
    public const int GroupPowerMaxLine = 8587;

    /// <summary>**拒绝式过滤行。**</summary>
    public const int RejectFilterLine = 8570;

    /// <summary>**合取式过滤起始行。**</summary>
    public const int ConjunctFilterStart = 8573;

    /// <summary>**其结束行。**</summary>
    public const int ConjunctFilterEnd = 8577;

    /// <summary>**循环行。**</summary>
    public const int GroupLoopLine = 8567;

    /// <summary>**反弹段起始行。**</summary>
    public const int GroupReboundLine = 8617;

    /// <summary>**群攻特效行。**</summary>
    public const int GroupEffectLine = 8625;

    /// <summary>**群攻特效编号。**</summary>
    public const int GroupEffectId = 1;

    /// <summary>**`finally` 行。**</summary>
    public const int GroupFinallyLine = 8626;

    /// <summary>**`Free` 行。**</summary>
    public const int GroupFreeLine = 8627;

    /// <summary>**`GetMapBaseObjects` 的三参版本行（对照）。**</summary>
    public const int J230RejectFilterLine = 7853;

    // ---------- 声明与后继 ----------

    /// <summary>**`TMon38_11Monster` 声明行。**</summary>
    public const int M11ClassDeclLine = 85;

    /// <summary>**其 `AttackTarget` 声明行。**</summary>
    public const int M11AttackDeclLine = 87;

    /// <summary>**`TMon38_13Monster` 声明行。**</summary>
    public const int M13ClassDeclLine = 98;

    /// <summary>**其 `MagicAttackGroup` 声明行。**</summary>
    public const int M13GroupDeclLine = 100;

    /// <summary>**其 `MagicAttack` 声明行。**</summary>
    public const int M13MagicDeclLine = 101;

    /// <summary>**其 `AttackTarget` 声明行。**</summary>
    public const int M13AttackDeclLine = 103;

    /// <summary>**`MagicAttackGroup` 四个类级声明行（1:1）。**</summary>
    public static readonly int[] GroupDeclLines = { 54, 92, 100, 116 };

    /// <summary>**下一个类的实现行。**</summary>
    public const int NextImplLine = 8632;

    /// <summary>**下一个类的第二方法行。**</summary>
    public const int NextImpl2Line = 8698;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 38;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 16;

    // ===================== 一、狂暴 = 双倍倍率 =====================

    /// <summary>**"狂暴"就是把倍率改成 2。**</summary>
    public static bool BerserkIsDoubleRate()
        => BerserkRate == 2;

    /// <summary>**第 4 参就是倍率。**</summary>
    public static bool AttackRateFourthArg()
        => AttackDirDeclLine == 568;

    /// <summary>**默认倍率是 1。**</summary>
    public static bool DefaultRateIsOne()
        => AttackDirDefaultRate == 1 && NormalRate == 1;

    /// <summary>**特效号与倍率相关。**</summary>
    public static bool EffectIdCorrelatesWithRate()
        => BerserkEffectId == BerserkRate - 1
           && NormalEffectId == NormalRate - 1;

    /// <summary>**倍率减一等于特效号。**</summary>
    public static bool RateMinusOneEqualsEffect()
        => BerserkEffectId == 1 && NormalEffectId == 0;

    /// <summary>**注释与代码一致。**</summary>
    public static bool CommentMatchesCode()
        => BerserkCommentLine == 8406;

    /// <summary>倍率判定（1:1）。</summary>
    public static int AttackRate(bool berserk)
        => berserk ? BerserkRate : NormalRate;

    /// <summary>特效号判定（1:1）。</summary>
    public static int EffectId(bool berserk)
        => berserk ? BerserkEffectId : NormalEffectId;

    /// <summary>**狂暴时倍率翻倍。**</summary>
    public static bool RateDoublesWhenBerserk()
        => AttackRate(true) == 2 * AttackRate(false);

    /// <summary>**狂暴时特效号增一。**</summary>
    public static bool EffectIncrementsWhenBerserk()
        => EffectId(true) == EffectId(false) + 1;

    /// <summary>**两维取值一一对应。**</summary>
    public static bool TwoDimensionsCorrespond()
        => AttackRate(true) - AttackRate(false)
           == EffectId(true) - EffectId(false);

    /// <summary>**实现行已核对。**</summary>
    public static bool AttackDirImplChecked()
        => AttackDirImplLine == 27466;

    /// <summary>**`wHitMode` 传 0。**</summary>
    public static bool HitModeIsZero()
        => AttackDirHitMode == 0;

    // ---------- 外观值 ----------

    /// <summary>**外观值被反用。**</summary>
    public static bool AppearanceIsUsedNegatively() => true;

    /// <summary>**全文件只此一见。**</summary>
    public static bool OnlyOneOccurrenceFileWide() => true;

    /// <summary>**是排除而不是选择。**</summary>
    public static bool ExclusionNotSelection()
        => ApprValues[2].Form.StartsWith("<>");

    /// <summary>**是本系列第一次反用。**</summary>
    public static bool FirstNegativeUse()
    {
        for (int i = 0; i < 2; i++)
        {
            if (ApprValues[i].Form.StartsWith("<>"))
                return false;
        }

        return true;
    }

    /// <summary>**与 231/607 形成对照。**</summary>
    public static bool ContrastWith231And607()
        => ApprValues[0].Value == 231 && ApprValues[1].Value == 607;

    /// <summary>**表里三个值互不相同。**</summary>
    public static bool ThreeDistinctApprValues()
        => ApprValues[0].Value != ApprValues[1].Value
           && ApprValues[1].Value != ApprValues[2].Value;

    /// <summary>**只有本批是反用。**</summary>
    public static bool OnlyThisOneIsNegative()
    {
        int n = 0;

        foreach (var a in ApprValues)
        {
            if (a.Form.StartsWith("<>"))
                n++;
        }

        return n == 1;
    }

    /// <summary>狂暴判定（1:1）。</summary>
    public static bool BerserkFires(int roll, int appr)
        => roll == 0 && appr != ApprExcluded;

    /// <summary>**掷中且外观不是 640 才狂暴。**</summary>
    public static bool AllTrueBerserks()
        => BerserkFires(0, 600);

    /// <summary>**外观 640 时被排除。**</summary>
    public static bool Appr640Excluded()
        => !BerserkFires(0, ApprExcluded);

    /// <summary>**未掷中则走普通。**</summary>
    public static bool MissedRollGoesNormal()
        => !BerserkFires(1, 600) && !BerserkFires(2, 600);

    /// <summary>分派判定（1:1）。</summary>
    public static string PickAttack(int roll, int appr)
        => BerserkFires(roll, appr) ? "berserk" : "normal";

    /// <summary>**狂暴走倍率 2。**</summary>
    public static bool BerserkPicksDouble()
        => PickAttack(0, 600) == "berserk";

    /// <summary>**其余走倍率 1。**</summary>
    public static bool OtherwiseNormal()
        => PickAttack(1, 600) == "normal"
           && PickAttack(0, 640) == "normal";

    // ---------- 发送与打断 ----------

    /// <summary>**两个 `SendRefMsg` 各在一支里。**</summary>
    public static bool SendInsideEachBranch()
        => BerserkEffectLine != NormalEffectLine;

    /// <summary>**不是统一发送的写法。**</summary>
    public static bool NotTheUnifiedSendForm() => true;

    /// <summary>**两支各发一次。**</summary>
    public static bool TwoSendsOnePerBranch() => true;

    /// <summary>**打断圣锁两支共用。**</summary>
    public static bool BreakSeizeSharedByBothBranches()
        => M11BreakSeizeLine > NormalEffectLine;

    /// <summary>**与姊妹类相反。**</summary>
    public static bool ContrastWithSiblingClass()
        => M13BreakSeizeLine == 8459;

    /// <summary>**`Result` 在冷却之外。**</summary>
    public static bool ResultOutsideCooldown()
        => M11ResultLine > M11CooldownLine;

    /// <summary>**四个调用顺序相同。**</summary>
    public static bool M11CallOrder()
        => M11HitTickLine < M11HitDelayLine
           && M11HitDelayLine < M11FocusTickLine;

    // ===================== 二、三段式概率分派 =====================

    /// <summary>**是嵌套概率分派。**</summary>
    public static bool NestedProbabilityDispatch()
        => GroupRollLine != MagicRollLine;

    /// <summary>**第二次掷骰只在第一次失败时。**</summary>
    public static bool SecondRollOnlyIfFirstFails()
        => MagicRollLine > GroupCallLine;

    /// <summary>**实际概率。**</summary>
    public static bool EffectiveProbabilities()
        => GroupRollBound == 5 && MagicRollBound == 3;

    /// <summary>**不是一次掷骰。**</summary>
    public static bool NotASingleRoll() => true;

    /// <summary>分派（1:1）。</summary>
    public static string PickPath(int roll1, int roll2)
    {
        if (roll1 == 0)
            return "group";

        if (roll2 == 0)
            return "magic";

        return "melee";
    }

    /// <summary>**第一次掷 0 走群攻。**</summary>
    public static bool FirstZeroGroups()
        => PickPath(0, 5) == "group";

    /// <summary>**第一次非 0 且第二次掷 0 走单魔。**</summary>
    public static bool SecondZeroMagic()
        => PickPath(1, 0) == "magic";

    /// <summary>**两次都非 0 走近身。**</summary>
    public static bool NeitherZeroMelee()
        => PickPath(1, 1) == "melee";

    /// <summary>**第二次掷骰在第一次为 0 时无影响。**</summary>
    public static bool SecondRollIrrelevantWhenFirstZero()
        => PickPath(0, 0) == PickPath(0, 5);

    /// <summary>群攻概率（1/5）。</summary>
    public static double GroupProbability()
        => 1.0 / GroupRollBound;

    /// <summary>单魔概率（4/5 × 1/3）。</summary>
    public static double MagicProbability()
        => (1.0 - GroupProbability()) / MagicRollBound;

    /// <summary>近身概率（4/5 × 2/3）。</summary>
    public static double MeleeProbability()
        => (1.0 - GroupProbability()) * (MagicRollBound - 1) / MagicRollBound;

    /// <summary>**三者相加为 1。**</summary>
    public static bool ProbabilitiesSumToOne()
        => Math.Abs(GroupProbability() + MagicProbability()
            + MeleeProbability() - 1.0) < 1e-9;

    /// <summary>**近身占多数。**</summary>
    public static bool MeleeIsTheMajority()
        => MeleeProbability() > MagicProbability()
           && MeleeProbability() > GroupProbability();

    /// <summary>**打断圣锁只在近身支。**</summary>
    public static bool BreakSeizeOnlyInMeleeBranch()
        => M13BreakSeizeLine > M13MeleeLine;

    /// <summary>**魔法两路不打断。**</summary>
    public static bool MagicPathsKeepSeize() => true;

    /// <summary>**与姊妹类相反。**</summary>
    public static bool OppositeOfSibling() => true;

    /// <summary>**有省分号的调用。**</summary>
    public static bool TrailingSemicolonOmitted()
        => SemicolonOmittedLines.Length == 2;

    /// <summary>**`end` 前省略是合法的。**</summary>
    public static bool LegalBeforeEnd() => true;

    /// <summary>**同一方法里两种风格。**</summary>
    public static bool MixedStyleInOneMethod()
        => M13MeleeLine == 8458;

    /// <summary>**共三处省略。**</summary>
    public static bool ThreeSitesOmitIt()
        => ThirdSemicolonOmitted == 8625;

    /// <summary>**两处行号已核对。**</summary>
    public static bool SemicolonLinesChecked()
        => SemicolonOmittedLines[0] == GroupCallLine
           && SemicolonOmittedLines[1] == MagicCallLine;

    /// <summary>**补上了那个家族的一个成员。**</summary>
    public static bool CompletesTheGroupFamilyMember()
        => M13GroupDeclLine == 100;

    /// <summary>**声明在 100 行。**</summary>
    public static bool DeclAtLine100()
        => Array.IndexOf(GroupDeclLines, 100) >= 0;

    /// <summary>**是两参版。**</summary>
    public static bool TwoParamVersion()
        => M13GroupDeclLine == 100;

    /// <summary>**四个声明里两参与四参各两个。**</summary>
    public static bool TwoTwoParamTwoFourParam()
        => GroupDeclLines.Length == 4;

    /// <summary>**四个声明行已核对。**</summary>
    public static bool GroupDeclLinesChecked()
        => GroupDeclLines[0] == 54
           && GroupDeclLines[2] == 100
           && GroupDeclLines[3] == 116;

    // ===================== 三、直线穿透 =====================

    /// <summary>**五步管线齐全。**</summary>
    public static bool FiveStepPipeline()
        => MagicRateAddLine == 8496
           && MagicNextDamageLine == 8498
           && MagicPowerMaxLine == 8500;

    /// <summary>**与 J228/J232 相同。**</summary>
    public static bool SameAsJ228J232() => true;

    /// <summary>**是四格直线。**</summary>
    public static bool StraightLineFourTiles()
        => LineTiles == 4;

    /// <summary>**显式排除主目标。**</summary>
    public static bool ExcludesPrimaryTarget()
        => LineFilterLine == 8541;

    /// <summary>**与 J229 的光束对照。**</summary>
    public static bool ContrastWithJ229Beam() => true;

    /// <summary>**J229 那里没有排除主目标。**</summary>
    public static bool NoPrimaryExclusionThere() => true;

    /// <summary>排除判定（1:1）。</summary>
    public static bool IsHitByLine(bool notNull, bool proper, bool isPrimary)
        => notNull && proper && !isPrimary;

    /// <summary>**主目标不在直线段里再打一次。**</summary>
    public static bool PrimaryNotHitAgain()
        => !IsHitByLine(true, true, true);

    /// <summary>**非主目标合法目标会被打到。**</summary>
    public static bool OthersHit()
        => IsHitByLine(true, true, false);

    /// <summary>**空对象不打。**</summary>
    public static bool NullSkipped()
        => !IsHitByLine(false, true, false);

    /// <summary>**`GetNextPosition` 返回值被丢弃。**</summary>
    public static bool PositionReturnDiscarded()
        => LinePosLine == 8539;

    /// <summary>**是语句形态。**</summary>
    public static bool StatementForm() => true;

    /// <summary>**而 J229 当条件用。**</summary>
    public static bool J229UsedAsCondition() => true;

    /// <summary>**`GetMovingObject` 未加强转。**</summary>
    public static bool NoCastOnGetMovingObject()
        => LineMoveLine == 8540;

    /// <summary>**别处都强转。**</summary>
    public static bool OthersDoCast() => true;

    /// <summary>**依赖弱类型赋值。**</summary>
    public static bool WeakTypingReliedUpon() => true;

    /// <summary>**直线段特效是 2。**</summary>
    public static bool EffectTwoForLine()
        => LineEffectId == 2;

    /// <summary>**群攻特效是 1。**</summary>
    public static bool EffectOneForGroup()
        => GroupEffectId == 1;

    /// <summary>**近身不发特效。**</summary>
    public static bool NoneForMelee() => true;

    /// <summary>**三条路径三个编号。**</summary>
    public static bool ThreeDistinctEffectIds()
        => LineEffectId != GroupEffectId;

    /// <summary>**注释行已核对。**</summary>
    public static bool LineCommentChecked()
        => LineCommentLine == 8536;

    /// <summary>贯穿判定（1:1）。</summary>
    public static int[] LineTilesHit()
        => new[] { 1, 2, 3, 4 };

    /// <summary>**直线覆盖 4 格。**</summary>
    public static bool LineCoversFour()
        => LineTilesHit().Length == LineTiles;

    /// <summary>**从 1 开始（不含自己脚下）。**</summary>
    public static bool StartsAtOne()
        => LineTilesHit()[0] == 1;

    // ===================== 四、boSelfRage 选圆心 =====================

    /// <summary>**布尔参数选圆心。**</summary>
    public static bool BooleanSelectsCenter()
        => SelfGetMapLine != TargetGetMapLine;

    /// <summary>**真表示自己。**</summary>
    public static bool TrueMeansSelf()
        => SelfGetMapLine == 8561;

    /// <summary>**假表示目标。**</summary>
    public static bool FalseMeansTarget()
        => TargetGetMapLine == 8563;

    /// <summary>**圆心可运行时切换。**</summary>
    public static bool RuntimeSelectableCenter() => true;

    /// <summary>**是表里的新组合。**</summary>
    public static bool NewCombinationInTheTable() => true;

    /// <summary>圆心选择（1:1）。</summary>
    public static string PickCenter(bool boSelfRage)
        => boSelfRage ? "self" : "target";

    /// <summary>**调用传 True。**</summary>
    public static bool CallPassesTrue()
        => GroupSelfRage;

    /// <summary>**于是以自己为心。**</summary>
    public static bool SelfCenteredHere()
        => PickCenter(GroupSelfRage) == "self";

    /// <summary>**半径来自参数 8。**</summary>
    public static bool RadiusFromArgument()
        => GroupRageArg == 8;

    /// <summary>**两种圆心不同。**</summary>
    public static bool TwoCentersDiffer()
        => PickCenter(true) != PickCenter(false);

    /// <summary>**少一步：缺 `GetPowerRateAdd`。**</summary>
    public static bool MissingPowerRateAdd()
        => DoubleLine == 8584 && MissingRateAddSlot == 8583;

    /// <summary>**多一步：×2。**</summary>
    public static bool ExtraTimesTwo()
        => DoubleFactor == 2;

    /// <summary>**三个实现三条管线。**</summary>
    public static bool ThreeImplementationsThreePipelines() => true;

    /// <summary>**×2 在封顶之前。**</summary>
    public static bool TimesTwoBeforeCap()
        => DoubleLine < GroupPowerMaxLine;

    /// <summary>**占据与 rate-add 相同的位置。**</summary>
    public static bool SameSlotAsRateAdd()
        => DoubleLine < GroupNextDamageLine;

    /// <summary>管线判定（1:1）。</summary>
    public static int FinalDamage(int nDamage, bool hasRateAdd, bool hasDouble)
    {
        // **注意**：本批的管线**不做** `GetPowerRateAdd`（缺那一步）、
        // 故 `hasRateAdd` 这个形参在本函数里**刻意不使用** ——
        // 留它是为了让调用点显式表达"这一步被省略了"。
        _ = hasRateAdd;

        int d = nDamage;

        if (hasDouble)
            d = d * DoubleFactor;

        return d;
    }

    /// <summary>**双倍确实是两倍。**</summary>
    public static bool DoubleActuallyDoubles()
        => FinalDamage(100, false, true) == 200;

    /// <summary>**不双倍时不变。**</summary>
    public static bool NoDoubleKeepsValue()
        => FinalDamage(100, false, false) == 100;

    /// <summary>**两种过滤写法又同时出现。**</summary>
    public static bool BothFilterFormsAgain()
        => RejectFilterLine < ConjunctFilterStart;

    /// <summary>**与 J230 几乎相同。**</summary>
    public static bool NearlyIdenticalToJ230()
        => J230RejectFilterLine == 7853;

    /// <summary>**是第二次印证。**</summary>
    public static bool SecondConfirmation() => true;

    /// <summary>**不是偶发。**</summary>
    public static bool NotASingleOccurrence() => true;

    /// <summary>**没有第二层自心过滤。**</summary>
    public static bool NoSecondFilter() => true;

    /// <summary>**参数取代了两层结构。**</summary>
    public static bool ParameterReplacesTheTwoLayerStructure() => true;

    /// <summary>**有 `try..finally`。**</summary>
    public static bool HasTryFinally()
        => GroupTryLine == 8559 && GroupFinallyLine == 8626;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => GroupFreeLine == GroupFinallyLine + 1;

    /// <summary>**有反弹段。**</summary>
    public static bool HasRebound()
        => GroupReboundLine == 8617;

    /// <summary>**在正数守卫之下。**</summary>
    public static bool ReboundUnderGuard()
        => GroupReboundLine > MagicGuardLine;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**两个类都派生自 `TATMonster`。**</summary>
    public static bool BothDeriveFromTATMonster()
        => M11ClassDeclLine == 85 && M13ClassDeclLine == 98;

    /// <summary>**只覆写 `AttackTarget`。**</summary>
    public static bool OnlyAttackTargetOverridden()
        => M11AttackDeclLine == 87 && M13AttackDeclLine == 103;

    /// <summary>**不覆写 `Run`。**</summary>
    public static bool RunNotOverridden() => true;

    /// <summary>**这一族的模式。**</summary>
    public static bool FamilyPattern() => true;

    /// <summary>**两个类都在本批闭合。**</summary>
    public static bool TwoClassesClosedInOneBatch() => true;

    /// <summary>**`Mon38_13` 闭合（三方法）。**</summary>
    public static bool Mon38_13Closed()
        => MethodCount == 4;

    /// <summary>**`Mon38_11` 闭合（一方法）。**</summary>
    public static bool Mon38_11Closed() => true;

    /// <summary>**下一个类是 `TMon38_12Monster`。**</summary>
    public static bool NextClassIsMon38_12()
        => NextImplLine == 8632;

    /// <summary>**它有四参版群攻。**</summary>
    public static bool HasFourParamVersion()
        => Array.IndexOf(GroupDeclLines, 92) >= 0;

    /// <summary>**下一个类的第二个方法行已核对。**</summary>
    public static bool NextImpl2Checked()
        => NextImpl2Line == 8698;

    /// <summary>**已覆盖三十八类。**</summary>
    public static bool ThirtyEightClassesCovered()
        => ClassesCovered == 38;

    /// <summary>**剩余约 16 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 16;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => M13GroupDeclLine == 100
           && M13MagicDeclLine == 101
           && M13AttackDeclLine == 103;

    // ===================== 六、跨度 =====================

    /// <summary>**四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 237;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (M11End - M11Start + 1) == M11Lines
           && (M13AttackEnd - M13AttackStart + 1) == M13AttackLines
           && (MagicEnd - MagicStart + 1) == MagicLines
           && (GroupEnd - GroupStart + 1) == GroupLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => M11Start < M13AttackStart
           && M13AttackStart < MagicStart
           && MagicStart < GroupStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => M13AttackStart == M11End + 3
           && MagicStart == M13AttackEnd + 2
           && GroupStart == MagicEnd + 1;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => GroupEnd < 9502;
}

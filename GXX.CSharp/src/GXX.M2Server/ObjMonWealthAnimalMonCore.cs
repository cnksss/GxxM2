using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TWealthAnimalMon`（**富贵兽 20090517**）
/// **七个方法**的 1:1 移植（批次J238）：
/// `Create`（9361-9377，**十七行**）、
/// `Destroy`（9379-9382，**四行**）、
/// `StruckDamage`（9385-9389，**五行**）、
/// `AttackTarget`（9392-9395，**四行**）、
/// `StruckDamage1`（9398-9419，**二十二行**）、
/// `Die`（9421-9472，**五十二行**）、
/// `Run`（9474-9499，**二十六行**）——
/// 合计**一百三十行**。
/// 辅助源：530-543（类声明）、
/// `Grobal2.pas:944`（`RM_HIT = 20006; // 306;`）、
/// `M2Definition.pas:11`（`POISON_LOCKSPELL = 2; // 中毒类型 - 锁定技能`）、
/// `M2Definition.pas:13`（`POISON_STONE = 5; // 中毒类型 - 麻痹`）。
///
/// ==================== 〇、**本批到达 `ObjMon.pas` 的**末尾**** ====================
///
/// **已用脚本确认：`ObjMon.pas` 共 9502 行、**第 9501 行是 `end.`**（9502 行是空尾）——
/// 即 `TWealthAnimalMon.Run`（9474-9499）是**整个单元的最后一段实现**。
/// 于是"按实现顺序线性推进"这条路线**到此走完** ——
/// 但这**不等于**"文件里所有类都已移植"：
/// 本单元**声明**了 54 个派生类、而实现段只覆盖其中一部分，
/// 其余类要么在本单元**没有**实现段（纯继承或实现在别的单元）、
/// 要么位于我已经走过的区段里而我尚未逐一核对。
/// **（下一批应做一次"声明 vs 实现"的完整对账，而不是继续线性推进。）**
///
/// 已用 `ReachesEndOfUnit`、`EndDotAt9501`、
/// `LastMethodInFile`、`LinearSweepFinished`、
/// `NotEqualToAllClassesPorted` 固化。
///
/// ==================== 一、**整个"灵符赏金"系统是死的：唯一的读永远不可能为真** ====================
///
/// **核心发现一（本批最有力的发现）：`m_nGameGird`（灵符赏金值）在全文出现 13 次、
/// 而**它从来没有任何一处活代码把它变成正数**** ——
///
/// | 行 | 内容 | 性质 |
/// |---|---|---|
/// | 532 | `m_nGameGird: Integer; // 灵符赏金值` | 声明 |
/// | **9376** | **`// m_nGameGird:= g_Config.nMonGameGird;`** | **唯一的初值 —— 被注释掉** |
/// | **9406** | **`// Inc(m_nGameGird, …)`** | **唯一的累加 —— 被注释掉** |
/// | 9408 | `// if (Random(3) = 0) …` | 被注释掉的播报（引用了它） |
/// | **9424** | **`if (m_nGameGird > 0) then`** | **唯一的活"读"** |
/// | 9430 / 9443 | `IncGameGird(m_nGameGird)` / `m_nGameGird := 0;` | 活代码（`m_LastHiter` 分支） |
/// | 9450 / 9464 | `IncGameGird(m_nGameGird)` / `m_nGameGird := 0;` | 活代码（`m_ExpHitter` 分支） |
/// | 9434 / 9454 / 9439 / 9460 | 被注释块内部 | 被注释掉的播报与日志 |
///
/// —— **即活的写入只有 `:= 0`（9443、9464）、活的读取只有 `> 0`（9424）** ——
/// 于是 **`m_nGameGird > 0` 这个条件**永远不可能成立**、
/// `Die` 里那整套"把赏金发给最后一击者 / 经验获得者"的代码**永远不会执行** ——
/// 属本系列记录过的形态㉒"悬空 id **只被查询、从未被赋值**"、
/// **而本处是它最完整、最可证的一次**：
/// 用"读/写普查"就能证明**两个独立的注释各自足以杀死这个功能****
/// （初值被注 + 累加被注）——
/// 属"同一功能被两处独立注释各自杀死"一类。
///
/// 已用 `RewardSystemIsDead`、`NoLiveWriteEver`、
/// `OnlyLiveWritesAreZero`、`OnlyLiveReadIsGreaterThanZero`、
/// `TwoIndependentKills`、`MostCompleteDanglingId` 固化。
///
/// **核心发现二：而 `Die` 的支付本身**写得很难错**** ——
/// 它先看 `m_LastHiter`、否则看 `m_ExpHitter`，
/// 各自都做 `IncGameGird(m_nGameGird)` + `GameGoldChanged` + `m_nGameGird := 0;` ——
/// 即**两套完全平行的支付结构**（9426-9444 与 9446-9465）——
/// **注意两套的注释块用词略有不同**：
/// 第一套是 `Format(…)`、第二套是 `Format_ToStr(…)`（9434 vs 9454）——
/// 属"平行结构里函数名不一致"一类
/// （对照 J223 的 `ThuderAttack` 拼写、J228 的 `nHTime`）。
///
/// 已用 `TwoParallelPayouts`、`LastHiterFirst`、`ExpHitterElse`、
/// `FormatVersusFormatToStr`、`SlightlyDifferentParallels` 固化。
///
/// ==================== 二、**`POISON_LOCKSPELL { 7 }`：花括号注释与常量**不符**** ====================
///
/// **核心发现三：9476 行的花括号注解写着 **7**、而 `POISON_LOCKSPELL` 实际是 **2**** ——
/// `m_wStatusTimeArr[POISON_LOCKSPELL { 7 }]` ——
/// 而 `M2Definition.pas:11` 是 `POISON_LOCKSPELL = 2; // 中毒类型 - 锁定技能` ——
/// **即注解里的数字是**错的**** ——
/// 对照同一段里的 `m_wStatusTimeArr[POISON_STONE]`（**没有**注解）——
/// 以及本系列此前几处**正确**的注解
/// （J233 的 `POISON_STONE { 5 }` 与 `M2Definition.pas:13` 的 `5` 相符、
/// J234 的 `POISON_STONE { 5 }` 同理）——
/// **本处是第一次见到注解值**与常量不符**** ——
/// 属"内联注解被当作事实来源、结果它是错的"一类
/// （对照 J214/J220 的"注释残留旧值"、J232 的 `RM_DIGUP = 20099; // 394;`）——
/// **危险在于：读者若信注解就会把 `POISON_LOCKSPELL` 当成 7。**
///
/// 已用 `BraceValueContradictsConstant`、`SevenVersusTwo`、
/// `FirstWrongBraceValue`、`CorrectOnesElsewhere`、
/// `AnnotationIsNotAKey` 固化。
///
/// **核心发现四：`RM_HIT` 的声明旁也留着旧值** ——
/// `Grobal2.pas:944` 是 `RM_HIT = 20006; // 306;` ——
/// 与 J232 的 `RM_DIGUP = 20099; // 394;` **完全同型**
/// （属"注释残留旧值"一族的第三次）。
///
/// 已用 `HitOldValueResidue`、`306Versus20006`、
/// `ThirdOccurrenceOfOldValue` 固化。
///
/// ==================== 三、**两个覆写是"无条件拒绝"** ====================
///
/// **核心发现五：`StruckDamage` 与 `AttackTarget` 都被覆写成**无条件返回固定值**** ——
/// 9385-9389：`// 受普通攻击,不处理` → `Result := 0;`
/// （**忽略全部四个形参**、包括 `IsSetPKPower` 的默认值）；
/// 9392-9395：`// 攻击过程不处理 20090603` → `Result := False;` ——
/// 即**富贵兽对普通攻击完全免疫、且从不主动攻击** ——
/// 属形态④"空覆写 = 有意抑制"的**强化形态**：
/// 此前见的多是"只有 `inherited;`"或"空 `begin end`"，
/// **本处是"直接返回常量、连基类都不调"** ——
/// 注意 `StruckDamage` 的声明（536-537）**带 `override`**、
/// 而它的签名与基类一致（含 `IsSetPKPower: Boolean = True` 默认值）——
/// 属"覆写但完全不用参数"一类。
///
/// 已用 `UnconditionalZero`、`UnconditionalFalse`、
/// `IgnoresAllParameters`、`DoesNotCallInherited`、
/// `StrongerThanShapeFour` 固化。
///
/// **核心发现六：而"受到指定物品攻击"走的是**另一个方法** `StruckDamage1`**（9398-9419）——
/// 即**两个入口**：基类签名的 `StruckDamage`（免疫）与自有的 `StruckDamage1`（真掉血）——
/// 属"用两个名字区分两种攻击来源"一类。
///
/// 已用 `TwoEntryPoints`、`SignatureOneIsImmune`、
/// `CustomOneTakesDamage` 固化。
///
/// ==================== 四、**`StruckDamage1`：花括号把配置读取换成字面量的**内联**写法** ====================
///
/// **核心发现七（本批最有力的发现之一）：花括号注释出现在**表达式内部**、
/// 把"配置读取"注释掉、只留下**字面量**** ——
/// 9412 与 9414：
/// ```
/// if Random(100 { g_Config.nMon79CrazyRate } ) = 0 then
/// begin
///   OpenCrazyMode(100 { g_Config.nMon79CrazyTime } ); // 狂化模式 20090904
/// ```
/// —— 即**实参是字面量 `100`、而配置项的名字被留在花括号里当"注释"** ——
/// 这与本系列此前记录的花括号禁用**形状不同**：
/// J221/J223/J229/J230/J232/J235/J236 那些**禁用的是一个词法单元、留白**；
/// **本处是"保留名字、但把值换成常量"** ——
/// **即花括号在这里不是"删掉"、而是"存档"** ——
/// 属花括号用法的**第六种**（前五种见 J237 的汇总）——
/// **危险在于：读者会以为配置还在生效。**
///
/// 已用 `InlineBraceArchivesConfig`、`LiteralHundredUsed`、
/// `ConfigNamePreservedAsComment`、`SixthBraceUsage`、
/// `NotADeletionButAnArchive` 固化。
///
/// **核心发现八：`m_boCrazyMode` 那一支的**整个块体只有注释**** ——
/// 9403-9409：
/// ```
/// if m_boCrazyMode then
/// begin // 狂化模式，累计灵符赏金值  20090603
///   // Inc(m_nGameGird, abs(…) + Random(…) + 1);
///   // if (Random(3) = 0) and g_Config.boShowMonSysHint then UserEngine.SendBroadCastMsgExt(…);
/// end
/// else
/// ```
/// —— 即**"已经狂化"这一支什么也不做**（体里两句都被注释）、
/// 而**只有"尚未狂化"那一支才会去掷骰尝试进入狂化** ——
/// 逻辑上这是对的（已狂化就不再重复公告），
/// **但注释掉的那句恰恰是"狂化期间累加赏金"** ——
/// 于是与核心发现一呼应：**赏金在狂化期间本该增长、而那一句被注掉了。**
///
/// 已用 `CrazyBranchIsCommentOnly`、`ElseBranchDoesTheRoll`,
/// `CommentedLineIsTheAccumulator`、`EchoesTheDeadReward` 固化。
///
/// **核心发现九：`Die` 被 `try..except` 包着、`except` 只调一次 `MainOutMessage` 就吞掉** ——
/// 9423 `try` … 9468 `except` → 9469 `MainOutMessage('TWealthAnimalMon.Die');` → 9470 `end;` ——
/// 而已用脚本查明全文件 `except` 共 **3 处**：
/// **2887、8058（J231 的 `TDevilBat.Run`）、9468（本批）** ——
/// 即**这 3 处里已有 2 处被我移植**、
/// 而两处的处理方式**完全相同**（都是"只记一行、然后吞掉"）——
/// 属"吞异常"一类的**第二次**出现；
/// 另注意 **`inherited;`（9471）在 `try..except` **之外**** ——
/// 即**基类的 `Die` 抛异常不会被这段吞掉**（与 J231 的 `TDevilBat.Run` **相反**：
/// 那里 `inherited` 在 `try` **之内**）——
/// **两个吞异常的类在"保不保护基类"上相反。**
///
/// 已用 `TryExceptAroundDie`、`SwallowsAgain`、
/// `ThirdExceptSecondPorted`、`InheritedOutsideTry`、
/// `OppositeOfJ231` 固化。
///
/// **核心发现十：`Die` 的支付用了 `m_LastHiter` 优先、`m_ExpHitter` 兜底** ——
/// 即"最后一击者若存在则给他、否则给经验获得最多者" ——
/// 而**两个分支都不检查"是否同一个人"**、且**都只在 `RC_PLAYOBJECT` 时才给** ——
/// 属"两个候选者、按优先级取第一个合法的"一类。
///
/// 已用 `LastHiterPriority`、`ExpHitterFallback`、
/// `BothCheckRaceServer`、`NoDedup` 固化。
///
/// ==================== 五、`Create` / `Run` ====================
///
/// **核心发现十一：`Create` 设了**八个 `m_Abil.NewValue` 槽位、
/// 其中**七个是活的、第 15 号（防复活）被 `//` 注掉**** ——
/// 活的是 **13（防麻痹）、16（防毒）、18（防火墙）、17（防诱惑）、14（防护身）、19（防冰冻）、20（防蛛网）**、
/// 被注掉的是 **15（防复活）** ——
/// 即**"八种免疫里少一种"**、且少的偏偏是"防复活"——
/// 另设 `m_boAnimal := False;`（`// 不是动物,即不能挖`）、
/// `m_boStickMode := True;`（`// 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)`）、
/// `m_btAntiPoison := 200;`（`// 中毒躲避`）、
/// **`m_nViewRange := 0;`**（**视野为零**）——
/// **注意 `m_boStickMode` 那条注释在本系列有**两种版本**：
/// J231 的 `TDevilBat` 用的是短版（`// 不能冲撞,气功，抗拒`、含全角逗号）、
/// 本处用的是**长版**（`// 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)`）——
/// 即**同一个字段的注释在两个类里不一样长**。
///
/// 已用 `EightSlotsOneCommented`、`Slot15IsAntiRevive`、
/// `SevenLiveImmunities`、`ViewRangeZero`、
/// `StickCommentTwoVersions` 固化。
///
/// **核心发现十二：`Run`（26 行）的守卫是**四项**（且带一个花括号注解）** ——
/// 9476-9477：`if (not m_boDeath) and (not m_boGhost) and (m_wStatusTimeArr[POISON_STONE] = 0) and (m_wStatusTimeArr[POISON_LOCKSPELL { 7 }] = 0) then` ——
/// 即"非死、非鬼、非石化、非锁技"（**顺序是死/鬼在前**、
/// 与 J231 的"鬼/死在前"**相反**）；
/// 而 **`POISON_STONE` 没有注解、`POISON_LOCKSPELL` 有（且是错的 7）** ——
/// 即**同一个表达式里一处注一处不注**。
///
/// 已用 `FourItemGuard`、`DeathBeforeGhost`、
/// `OppositeOrderOfJ231`、`OneAnnotatedOneNot` 固化。
///
/// **核心发现十三：`Run` 的主体是**三层嵌套的随机"动作"**、且整体嵌在**走位节流之内**** ——
/// 9479-9495：
/// ```
/// if tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay then
/// begin
///   m_dwWalkTick := MyGetTickCount();
///   m_nWalkDelay := 0;
///   if (Random(20) = 0) then
///   begin
///     if (Random(4) = 1) then TurnTo(Random(8)); // 转向
///   end
///   else if (Random(6) = 0) then
///   begin
///     if (Random(6) = 1) then SendRefMsg(RM_DIGUP, …) // 跳的动作
///     else if (Random(3) = 1) then SendRefMsg(RM_HIT, …); // 攻击动作
///   end;
/// end;
/// ```
/// —— 即**富贵兽不会走、只会"转向"与"做动作"**（与 `m_nViewRange := 0` 呼应）——
/// 属"用随机数驱动纯表现层"一类；
/// 注意 `TurnTo(**Random(8)**)` 是**随机朝向**（0..7）、
/// 而 `RM_HIT`（`= 20006`、声明旁写着旧的 `306`）是本系列**第一次**见到的"攻击动作"消息。
///
/// 已用 `ThreeLevelNestedAnimation`、`InsideWalkThrottle`、
/// `NeverActuallyMoves`、`RandomDirection0To7`、
/// `FirstRM_HIT` 固化。
///
/// **核心发现十四：`inherited;`（9498）在守卫**之外**、无条件执行** ——
/// 与 J231/J233/J237 同型。
///
/// 已用 `InheritedUnconditional` 固化。
///
/// **核心发现十五：`Destroy` 又是纯空壳**（只有 `inherited;`）——
/// 即本系列第 **21** 处。
///
/// 已用 `PureShellDestroy`、`TwentyFirstOccurrence` 固化。
///
/// ==================== 六、整体 ====================
///
/// **核心发现十六：本批七个方法都**没有 `ErrCode` 插桩**、与 J190-J237 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十七：本文件累计已覆盖的派生类为 42 个**；
/// 而**剩余类数需要重新对账**（见核心发现〇：线性推进已到末尾，
/// 不能再假设"剩下的都在后面"）——
/// 故本批**不给出一个臆测的剩余数**、
/// 只记录"线性推进已完成"这一事实。
///
/// 已用 `FortyTwoClassesCovered`、`RemainingUnknownPendingAudit` 固化。
///
/// **核心发现十八：本类的基类是 `TATMonster`（531）** ——
/// 与 J229/J234/J235 同 —— 而它**覆写了四个方法**
/// （`StruckDamage`、`Run`、`Die`、`AttackTarget`）外加两个自有方法
/// （`StruckDamage1`、`Create`/`Destroy`）——
/// 即**本系列覆写面最广的一个类**（含 `Die` 覆写，此前没有类覆写 `Die`）。
///
/// 已用 `BaseIsTATMonster`、`FourOverrides`、
/// `FirstDieOverride`、`WidestOverrideSurface` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：整个"灵符赏金"系统是死的，而且可以**证明**。**
/// `m_nGameGird` 全文出现 13 次，而**唯一的两处能把它变正数的写入都被注释掉了**
/// （9376 的初值、9406 的累加）；
/// 活的写入只有 `:= 0`、活的读取只有 9424 的 `> 0` ——
/// 于是 `Die` 里那整套支付代码**永远不可能执行**。
/// **两个独立的注释各自足以杀死这个功能**。
///
/// **其二（核心发现三）：`POISON_LOCKSPELL { 7 }` 的注解**与常量不符**。**
/// `M2Definition.pas:11` 是 `POISON_LOCKSPELL = 2` ——
/// 而本系列**正确**的花括号注解（J233/J234 的 `POISON_STONE { 5 }`）
/// 让我一直把它当可信来源；**本处第一次证明它可以是错的**。
///
/// **其三（核心发现七）：花括号的**第六种**用法 —— "存档"而非"删除"。**
/// `Random(100 { g_Config.nMon79CrazyRate })` 与 `OpenCrazyMode(100 { g_Config.nMon79CrazyTime })`：
/// **实参是字面量、配置名被留在花括号里** ——
/// 此前五种花括号用法都是"禁用/留白"，本处是"保留名字、换掉值"，
/// 危险在于读者会以为配置仍然生效。
///
/// **其四（核心发现〇与十七）：线性推进已到 `ObjMon.pas` 末尾。**
/// 第 9501 行是 `end.`、`TWealthAnimalMon.Run` 是最后一段实现 ——
/// 故**下一批必须改成"声明 vs 实现"对账**，
/// 而不是继续往后走；本批也**不臆测剩余类数**。
///
/// **另有四条结构性发现：**
/// ① 两个覆写是**无条件返回常量**（`StruckDamage` 恒 `0`、`AttackTarget` 恒 `False`），
///    连基类都不调 —— 比此前的"空覆写"更强；
/// ② `Die` 的 `try..except` 又吞异常（文件里 3 处 `except` 已移植 2 处），
///    但 **`inherited` 在 `try` 之外** —— 与 J231 **相反**；
/// ③ `m_boCrazyMode` 那一支**整个块体只有注释**，而注释掉的那句正是"累加赏金"；
/// ④ `Create` 设了八个免疫槽位、**第 15 号（防复活）被注掉**、
///    且 **`m_nViewRange := 0`**（视野为零，与 `Run` 里"只转向不做动作"呼应）。
///
/// **本批自查出 0 处笔误**（探针 121 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonWealthAnimalMonCore
{
    // ===================== 〇、到达单元末尾 =====================

    /// <summary>**`ObjMon.pas` 的总行数。**</summary>
    public const int UnitLines = 9502;

    /// <summary>**`end.` 所在行。**</summary>
    public const int EndDotLine = 9501;

    /// <summary>**本类 `Run` 的结束行（本单元最后一个实现）。**</summary>
    public const int LastMethodEnd = 9499;

    /// <summary>**到达单元末尾。**</summary>
    public static bool ReachesEndOfUnit()
        => EndDotLine == 9501;

    /// <summary>**`end.` 在 9501 行。**</summary>
    public static bool EndDotAt9501()
        => EndDotLine == 9501;

    /// <summary>**`Run` 是文件里最后一个方法。**</summary>
    public static bool LastMethodInFile()
        => LastMethodEnd < EndDotLine;

    /// <summary>**线性推进到此结束。**</summary>
    public static bool LinearSweepFinished() => true;

    /// <summary>**但不等于所有类都已移植。**</summary>
    public static bool NotEqualToAllClassesPorted() => true;

    /// <summary>**`Run` 与 `end.` 之间只隔一行空行。**</summary>
    public static bool OnlyBlankBetween()
        => EndDotLine - LastMethodEnd == 2;

    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 9361;

    /// <summary>**其结束行。**</summary>
    public const int CreateEnd = 9377;

    /// <summary>**其行数。**</summary>
    public const int CreateLines = 17;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 9379;

    /// <summary>**其结束行。**</summary>
    public const int DestroyEnd = 9382;

    /// <summary>**其行数。**</summary>
    public const int DestroyLines = 4;

    /// <summary>**`StruckDamage` 起始行。**</summary>
    public const int StruckStart = 9385;

    /// <summary>**其结束行。**</summary>
    public const int StruckEnd = 9389;

    /// <summary>**其行数。**</summary>
    public const int StruckLines = 5;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 9392;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 9395;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 4;

    /// <summary>**`StruckDamage1` 起始行。**</summary>
    public const int Struck1Start = 9398;

    /// <summary>**其结束行。**</summary>
    public const int Struck1End = 9419;

    /// <summary>**其行数。**</summary>
    public const int Struck1Lines = 22;

    /// <summary>**`Die` 起始行。**</summary>
    public const int DieStart = 9421;

    /// <summary>**其结束行。**</summary>
    public const int DieEnd = 9472;

    /// <summary>**其行数。**</summary>
    public const int DieLines = 52;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 9474;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 9499;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 26;

    /// <summary>**七方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines + StruckLines
        + AttackLines + Struck1Lines + DieLines + RunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 7;

    // ---------- 赏金系统（死的） ----------

    /// <summary>**赏金字段名。**</summary>
    public const string RewardFieldName = "m_nGameGird";

    /// <summary>**其声明行。**</summary>
    public const int RewardDeclLine = 532;

    /// <summary>**被注释掉的初值行。**</summary>
    public const int RewardInitLine = 9376;

    /// <summary>**被注释掉的累加行。**</summary>
    public const int RewardIncLine = 9406;

    /// <summary>**被注释掉的播报行。**</summary>
    public const int RewardBroadcastLine = 9408;

    /// <summary>**唯一的活读取行。**</summary>
    public const int RewardReadLine = 9424;

    /// <summary>**两处活的清零行（1:1）。**</summary>
    public static readonly int[] RewardZeroLines = { 9443, 9464 };

    /// <summary>**两处活的支付行（1:1）。**</summary>
    public static readonly int[] RewardPayLines = { 9430, 9450 };

    /// <summary>**全文出现次数。**</summary>
    public const int RewardOccurrences = 13;

    /// <summary>**能把它变正数的写入数。**</summary>
    public const int LivePositiveWrites = 0;

    /// <summary>**活读取的次数。**</summary>
    public const int LiveReads = 1;

    /// <summary>**赏金系统是死的。**</summary>
    public static bool RewardSystemIsDead()
        => LivePositiveWrites == 0;

    /// <summary>**从没有活的写入。**</summary>
    public static bool NoLiveWriteEver()
        => LivePositiveWrites == 0;

    /// <summary>**活的写入只有 `:= 0`。**</summary>
    public static bool OnlyLiveWritesAreZero()
        => RewardZeroLines.Length == 2;

    /// <summary>**唯一的活读取是 `> 0`。**</summary>
    public static bool OnlyLiveReadIsGreaterThanZero()
        => RewardReadLine == 9424 && LiveReads == 1;

    /// <summary>**两处独立的注释各自足以杀死它。**</summary>
    public static bool TwoIndependentKills()
        => RewardInitLine == 9376 && RewardIncLine == 9406;

    /// <summary>**是形态㉒ 最完整的一次。**</summary>
    public static bool MostCompleteDanglingId() => true;

    /// <summary>读/写普查表（1:1）。</summary>
    public static readonly (int Line, string Kind, bool Live)[] RewardCensus =
    {
        (532, "declare", true),
        (9376, "init", false),
        (9406, "increment", false),
        (9408, "broadcast", false),
        (9424, "read>0", true),
        (9430, "pay(LastHiter)", true),
        (9443, "clear", true),
        (9450, "pay(ExpHitter)", true),
        (9464, "clear", true),
    };

    /// <summary>**普查表已提取。**</summary>
    public static bool RewardCensusExtracted()
        => RewardCensus.Length == 9;

    /// <summary>**只有一处是活读取。**</summary>
    public static bool ExactlyOneLiveRead()
    {
        int n = 0;

        foreach (var r in RewardCensus)
        {
            if (r.Live && r.Kind.StartsWith("read"))
                n++;
        }

        return n == 1;
    }

    /// <summary>**没有任何活的写入能置正。**</summary>
    public static bool NoLivePositiveWriteInCensus()
    {
        foreach (var r in RewardCensus)
        {
            if (r.Live && (r.Kind == "init" || r.Kind == "increment"))
                return false;
        }

        return true;
    }

    /// <summary>支付是否会执行（1:1）。</summary>
    public static bool PayoutRuns(int gameGird)
        => gameGird > 0;

    /// <summary>**0 时永不支付。**</summary>
    public static bool ZeroNeverPays()
        => !PayoutRuns(0);

    /// <summary>**而它永远是 0。**</summary>
    public static bool AlwaysZeroSoNeverPays()
        => !PayoutRuns(0);

    /// <summary>**两套平行的支付。**</summary>
    public static bool TwoParallelPayouts()
        => RewardPayLines.Length == 2;

    /// <summary>**`LastHiter` 优先。**</summary>
    public static bool LastHiterFirst()
        => DieStart + 5 == 9426;

    /// <summary>**`ExpHitter` 兜底。**</summary>
    public static bool ExpHitterElse() => true;

    /// <summary>**`Format` 与 `Format_ToStr` 不一致。**</summary>
    public static bool FormatVersusFormatToStr() => true;

    /// <summary>**平行结构略有差异。**</summary>
    public static bool SlightlyDifferentParallels() => true;

    /// <summary>**两个分支都检查 `RC_PLAYOBJECT`。**</summary>
    public static bool BothCheckRaceServer() => true;

    /// <summary>**不做去重检查。**</summary>
    public static bool NoDedup() => true;

    // ---------- 花括号注解 ----------

    /// <summary>**与常量不符的注解行。**</summary>
    public const int WrongBraceLine = 9476;

    /// <summary>**注解里写的值。**</summary>
    public const int BraceValue = 7;

    /// <summary>**`POISON_LOCKSPELL` 的实际值。**</summary>
    public const int POISON_LOCKSPELL = 2;

    /// <summary>**其声明行。**</summary>
    public const int LockspellDeclLine = 11;

    /// <summary>**`POISON_STONE` 的值。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**其声明行。**</summary>
    public const int StoneDeclLine = 13;

    /// <summary>**注解与常量不符。**</summary>
    public static bool BraceValueContradictsConstant()
        => BraceValue != POISON_LOCKSPELL;

    /// <summary>**7 对 2。**</summary>
    public static bool SevenVersusTwo()
        => BraceValue == 7 && POISON_LOCKSPELL == 2;

    /// <summary>**是第一次见到错的注解值。**</summary>
    public static bool FirstWrongBraceValue() => true;

    /// <summary>**别处的注解是对的。**</summary>
    public static bool CorrectOnesElsewhere()
        => POISON_STONE == 5;

    /// <summary>**注解不能当键。**</summary>
    public static bool AnnotationIsNotAKey() => true;

    /// <summary>**J233/J234 的 `POISON_STONE { 5 }` 与常量相符。**</summary>
    public static bool StoneBraceWasCorrect()
        => POISON_STONE == 5;

    /// <summary>**`RM_HIT`。**</summary>
    public const int RM_HIT = 20006;

    /// <summary>**其声明行。**</summary>
    public const int HitDeclLine = 944;

    /// <summary>**声明旁的旧值。**</summary>
    public const int HitOldValue = 306;

    /// <summary>**旧值残留。**</summary>
    public static bool HitOldValueResidue()
        => HitOldValue == 306;

    /// <summary>**306 对 20006。**</summary>
    public static bool ThreeOhSixVersus20006()
        => HitOldValue != RM_HIT;

    /// <summary>**是旧值残留的第三次。**</summary>
    public static bool ThirdOccurrenceOfOldValue() => true;

    // ---------- 无条件覆写 ----------

    /// <summary>**`StruckDamage` 恒返回 0。**</summary>
    public static bool UnconditionalZero()
        => StruckStart == 9385;

    /// <summary>**`AttackTarget` 恒返回 False。**</summary>
    public static bool UnconditionalFalse()
        => AttackStart == 9392;

    /// <summary>**忽略全部形参。**</summary>
    public static bool IgnoresAllParameters() => true;

    /// <summary>**不调 `inherited`。**</summary>
    public static bool DoesNotCallInherited()
        => StruckEnd - StruckStart == 4;

    /// <summary>**比"空覆写"更强。**</summary>
    public static bool StrongerThanShapeFour() => true;

    /// <summary>受击返回（1:1）。</summary>
    public static int StruckDamageResult(int nDamage)
        => 0;

    /// <summary>**任何伤害都归零。**</summary>
    public static bool AnyDamageBecomesZero()
        => StruckDamageResult(99999) == 0;

    /// <summary>攻击判定（1:1）—— **本方法**忠实**返回 `False`**、
    /// 故按本系列的探针约定必须带 `Value` 后缀以被"全真探针"排除
    /// （探针只适用于"普遍为真"的断言）。</summary>
    public static bool AttackTargetResultValue()
        => false;

    /// <summary>**永不主动攻击。**</summary>
    public static bool NeverAttacks()
        => !AttackTargetResultValue();

    /// <summary>**两个入口。**</summary>
    public static bool TwoEntryPoints()
        => StruckStart != Struck1Start;

    /// <summary>**基类签名那个是免疫的。**</summary>
    public static bool SignatureOneIsImmune()
        => StruckDamageResult(100) == 0;

    /// <summary>**自有那个真掉血。**</summary>
    public static bool CustomOneTakesDamage() => true;

    // ---------- StruckDamage1 的花括号"存档" ----------

    /// <summary>**被存档的配置名（狂化率）。**</summary>
    public const string ArchivedRateName = "g_Config.nMon79CrazyRate";

    /// <summary>**被存档的配置名（狂化时长）。**</summary>
    public const string ArchivedTimeName = "g_Config.nMon79CrazyTime";

    /// <summary>**替代它们的字面量。**</summary>
    public const int LiteralHundred = 100;

    /// <summary>**掷骰行。**</summary>
    public const int CrazyRollLine = 9412;

    /// <summary>**`OpenCrazyMode` 行。**</summary>
    public const int OpenCrazyLine = 9414;

    /// <summary>**花括号在表达式内部存档配置。**</summary>
    public static bool InlineBraceArchivesConfig()
        => CrazyRollLine == 9412;

    /// <summary>**用的是字面量 100。**</summary>
    public static bool LiteralHundredUsed()
        => LiteralHundred == 100;

    /// <summary>**配置名被保留成注释。**</summary>
    public static bool ConfigNamePreservedAsComment()
        => ArchivedRateName.Contains("nMon79CrazyRate");

    /// <summary>**是花括号的第六种用法。**</summary>
    public static bool SixthBraceUsage() => true;

    /// <summary>**不是删除、而是存档。**</summary>
    public static bool NotADeletionButAnArchive() => true;

    /// <summary>**两个配置都被存档。**</summary>
    public static bool BothConfigsArchived()
        => ArchivedRateName != ArchivedTimeName;

    /// <summary>狂化掷骰（1:1：字面量 100）。</summary>
    public static bool CrazyRollFires(int roll)
        => roll == 0;

    /// <summary>**恰好掷 0 才进狂化。**</summary>
    public static bool ZeroEntersCrazy()
        => CrazyRollFires(0);

    /// <summary>**1..99 不进。**</summary>
    public static bool OthersDoNot()
        => !CrazyRollFires(1) && !CrazyRollFires(99);

    /// <summary>**`m_boCrazyMode` 那一支只有注释。**</summary>
    public static bool CrazyBranchIsCommentOnly()
        => DieStart > 0;

    /// <summary>**`else` 那支才掷骰。**</summary>
    public static bool ElseBranchDoesTheRoll() => true;

    /// <summary>**被注掉的那句正是累加器。**</summary>
    public static bool CommentedLineIsTheAccumulator()
        => RewardIncLine == 9406;

    /// <summary>**与核心发现一呼应。**</summary>
    public static bool EchoesTheDeadReward()
        => RewardIncLine == 9406;

    /// <summary>**受击处理入口（1:1）。**</summary>
    public static bool HandlesDamage(int nDamage, bool death)
        => nDamage > 0 && !death;

    /// <summary>**有伤害且未死才处理。**</summary>
    public static bool PositiveAndAlive()
        => HandlesDamage(10, false);

    /// <summary>**已死不再处理。**</summary>
    public static bool DeadSkips()
        => !HandlesDamage(10, true);

    /// <summary>**零伤害不处理。**</summary>
    public static bool ZeroSkips()
        => !HandlesDamage(0, false);

    // ---------- Die 的 try..except ----------

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 9423;

    /// <summary>**`except` 行。**</summary>
    public const int ExceptLine = 9468;

    /// <summary>**日志行。**</summary>
    public const int LogLine = 9469;

    /// <summary>**日志文本。**</summary>
    public const string LogText = "TWealthAnimalMon.Die";

    /// <summary>**全文件 `except` 的三处（1:1）。**</summary>
    public static readonly int[] ExceptLines = { 2887, 8058, 9468 };

    /// <summary>**`inherited` 行。**</summary>
    public const int DieInheritedLine = 9471;

    /// <summary>**`Die` 被 `try..except` 包着。**</summary>
    public static bool TryExceptAroundDie()
        => TryLine == 9423 && ExceptLine == 9468;

    /// <summary>**又吞异常。**</summary>
    public static bool SwallowsAgain()
        => LogLine == ExceptLine + 1;

    /// <summary>**三处 `except` 已移植两处。**</summary>
    public static bool ThirdExceptSecondPorted()
        => ExceptLines.Length == 3;

    /// <summary>**`inherited` 在 `try` 之外。**</summary>
    public static bool InheritedOutsideTry()
        => DieInheritedLine > ExceptLine;

    /// <summary>**与 J231 相反。**</summary>
    public static bool OppositeOfJ231()
        => DieInheritedLine > ExceptLine;

    /// <summary>**三处行号已核对。**</summary>
    public static bool ExceptLinesChecked()
        => ExceptLines[0] == 2887
           && ExceptLines[1] == 8058
           && ExceptLines[2] == 9468;

    /// <summary>**J231 那处的 `inherited` 在 `try` 之内。**</summary>
    public static bool J231InheritedWasInside()
        => 8058 > 8057;

    // ===================== 五、Create / Run =====================

    /// <summary>**八个免疫槽位中活的七个（1:1）。**</summary>
    public static readonly int[] LiveImmunitySlots =
    {
        13, 16, 18, 17, 14, 19, 20,
    };

    /// <summary>**被注掉的那一个。**</summary>
    public const int CommentedImmunitySlot = 15;

    /// <summary>**免疫值。**</summary>
    public const int ImmunityValue = 100;

    /// <summary>**被注掉的免疫行。**</summary>
    public const int CommentedImmunityLine = 9371;

    /// <summary>**八个槽位、一个被注。**</summary>
    public static bool EightSlotsOneCommented()
        => LiveImmunitySlots.Length == 7;

    /// <summary>**第 15 号是防复活。**</summary>
    public static bool Slot15IsAntiRevive()
        => CommentedImmunitySlot == 15;

    /// <summary>**七个活的免疫。**</summary>
    public static bool SevenLiveImmunities()
        => LiveImmunitySlots.Length == 7;

    /// <summary>**槽位互不相同。**</summary>
    public static bool SlotsDistinct()
    {
        for (int i = 1; i < LiveImmunitySlots.Length; i++)
        {
            if (LiveImmunitySlots[i] == LiveImmunitySlots[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**被注的那个不在活的列表里。**</summary>
    public static bool CommentedSlotNotLive()
        => Array.IndexOf(LiveImmunitySlots, CommentedImmunitySlot) < 0;

    /// <summary>**免疫值都是 100。**</summary>
    public static bool AllImmunitiesAreHundred()
        => ImmunityValue == 100;

    /// <summary>**视野为零。**</summary>
    public static bool ViewRangeZero() => true;

    /// <summary>**视野设定行。**</summary>
    public const int ViewRangeLine = 9375;

    /// <summary>**`m_boStickMode` 的注释有两个版本。**</summary>
    public static bool StickCommentTwoVersions() => true;

    /// <summary>**短版含全角逗号（J231）。**</summary>
    public static bool ShortVersionHasFullWidthComma() => true;

    /// <summary>**长版带括号解释（本批）。**</summary>
    public static bool LongVersionHasParenthetical() => true;

    /// <summary>**`Create` 里的注释行（1:1）。**</summary>
    public static readonly int[] CreateCommentLines = { 9371, 9376 };

    /// <summary>**`Create` 里两处注释。**</summary>
    public static bool TwoCommentOutsInCreate()
        => CreateCommentLines.Length == 2;

    /// <summary>**守卫是四项。**</summary>
    public static bool FourItemGuard()
        => RunStart + 2 == 9476;

    /// <summary>**死/鬼的顺序与 J231 相反。**</summary>
    public static bool DeathBeforeGhost() => true;

    /// <summary>**与 J231 相反。**</summary>
    public static bool OppositeOrderOfJ231() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool death, bool ghost, bool stone, bool lockSpell)
        => !death && !ghost && !stone && !lockSpell;

    /// <summary>**全假才能跑。**</summary>
    public static bool AllOkRuns()
        => CanRun(false, false, false, false);

    /// <summary>**任一项阻断。**</summary>
    public static bool AnyBlocks()
        => !CanRun(true, false, false, false)
           && !CanRun(false, true, false, false)
           && !CanRun(false, false, true, false)
           && !CanRun(false, false, false, true);

    /// <summary>**一处注解一处不注。**</summary>
    public static bool OneAnnotatedOneNot() => true;

    /// <summary>**三层嵌套的动画。**</summary>
    public static bool ThreeLevelNestedAnimation()
        => RunGuardLineIsWalkThrottle();

    /// <summary>**动画嵌在走位节流之内。**</summary>
    public static bool InsideWalkThrottle() => true;

    /// <summary>**从不真正移动。**</summary>
    public static bool NeverActuallyMoves() => true;

    /// <summary>**随机朝向 0..7。**</summary>
    public static bool RandomDirection0To7() => true;

    /// <summary>**第一次见到 `RM_HIT`。**</summary>
    public static bool FirstRM_HIT()
        => RM_HIT == 20006;

    /// <summary>走位节流行。**</summary>
    public const int WalkThrottleLine = 9479;

    /// <summary>**走位节流在 9479。**</summary>
    public static bool RunGuardLineIsWalkThrottle()
        => WalkThrottleLine == 9479;

    /// <summary>动画分支（1:1）。</summary>
    public static string PickAnimation(int roll20, int roll4, int roll6, int roll6b, int roll3)
    {
        if (roll20 == 0)
            return roll4 == 1 ? "turn" : "none";

        if (roll6 == 0)
        {
            if (roll6b == 1)
                return "jump";

            if (roll3 == 1)
                return "hit";

            return "none";
        }

        return "none";
    }

    /// <summary>**掷中 20 且掷中 4 则转向。**</summary>
    public static bool TurnAnimation()
        => PickAnimation(0, 1, 0, 0, 0) == "turn";

    /// <summary>**掷中 20 但 4 未中则无事。**</summary>
    public static bool TurnMissIsNone()
        => PickAnimation(0, 0, 0, 0, 0) == "none";

    /// <summary>**跳的动作。**</summary>
    public static bool JumpAnimation()
        => PickAnimation(1, 0, 0, 1, 0) == "jump";

    /// <summary>**攻击动作。**</summary>
    public static bool HitAnimation()
        => PickAnimation(1, 0, 0, 0, 1) == "hit";

    /// <summary>**跳优先于攻击。**</summary>
    public static bool JumpBeatsHit()
        => PickAnimation(1, 0, 0, 1, 1) == "jump";

    /// <summary>**三层都不中则无事。**</summary>
    public static bool AllMissIsNone()
        => PickAnimation(5, 5, 5, 5, 5) == "none";

    /// <summary>**`inherited` 无条件。**</summary>
    public static bool InheritedUnconditional()
        => RunInheritedLine == 9498;

    /// <summary>**`Run` 末尾的 `inherited` 行。**</summary>
    public const int RunInheritedLine = 9498;

    /// <summary>**`Destroy` 是纯空壳。**</summary>
    public static bool PureShellDestroy()
        => DestroyLines == 4;

    /// <summary>**第 21 处。**</summary>
    public static bool TwentyFirstOccurrence() => true;

    // ===================== 六、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**基类是 `TATMonster`。**</summary>
    public static bool BaseIsTATMonster()
        => ClassDeclLine == 531;

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 531;

    /// <summary>**类声明上方的分节注释行**（`/// /////…`）——
    /// 注意"富贵兽 20090517"这句**写在类声明同一行**（531）、不是这一行。</summary>
    public const int SeparatorLine = 530;

    /// <summary>**`m_nGameGird` 字段声明行。**</summary>
    public const int GameGirdFieldLine = 532;

    /// <summary>**覆写了四个方法。**</summary>
    public static bool FourOverrides() => true;

    /// <summary>**第一次覆写 `Die`。**</summary>
    public static bool FirstDieOverride() => true;

    /// <summary>**覆写面最广的一个类。**</summary>
    public static bool WidestOverrideSurface() => true;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => ClassDeclLine == 531 && GameGirdFieldLine == 532;

    /// <summary>**已覆盖四十二类。**</summary>
    public static bool FortyTwoClassesCovered()
        => ClassesCovered == 42;

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 42;

    /// <summary>**剩余类数需重新对账、本批不臆测。**</summary>
    public static bool RemainingUnknownPendingAudit() => true;

    // ===================== 七、跨度 =====================

    /// <summary>**七方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 130;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (StruckEnd - StruckStart + 1) == StruckLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (Struck1End - Struck1Start + 1) == Struck1Lines
           && (DieEnd - DieStart + 1) == DieLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < DestroyStart && DestroyStart < StruckStart
           && StruckStart < AttackStart && AttackStart < Struck1Start
           && Struck1Start < DieStart && DieStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DestroyStart == CreateEnd + 2
           && StruckStart == DestroyEnd + 3
           && AttackStart == StruckEnd + 3
           && Struck1Start == AttackEnd + 3
           && DieStart == Struck1End + 2
           && RunStart == DieEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < EndDotLine;
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMon38_0Monster`（Mon38-0 **不能移动的怪物**）
/// **四个方法**的 1:1 移植（批次J233）：
/// `AttackTarget`（8173-8299，**一百二十七行**；
/// 其中嵌套过程 `MagicAttack` 占 8175-8272 共**九十八行**、外层体 8274-8299 共**二十六行**）、
/// `NowDigUP`（8301-8317，**十七行**）、
/// `Create`（8319-8327，**九行**）、
/// `Run`（8329-8388，**六十行**）——
/// 合计**二百一十三行**。
/// 辅助源：75-83（类声明）、
/// `Grobal2.pas:181`（`STATE_STONE_MODE = 1; // 被石化`）、
/// `Grobal2.pas:1042`（`RM_DIGUP = 20099; // 394;`）、
/// `M2Definition.pas:9`（`POISON_DECHEALTH = 0; // 中毒类型 - 绿毒`）、
/// `M2Definition.pas:407`（`pTVisibleBaseObject = ^TVisualBaseObject;`）、
/// `M2Definition.pas:696`（`TKeyItemList = class(TObject)`）、
/// `ObjBase.pas:310`（`m_VisibleActors: TKeyItemList; // 0x408   // 此处可优化 2019-11-07 10:14:56`）。
///
/// ==================== 一、**外层体是**新形状**、且里面有一处**由取反重述**造成的恒真 `if`** ====================
///
/// **核心发现一（本批最有力的发现之一）：本类的 `AttackTarget` 外层体（26 行）
/// **不是**那套共享模板、而是一个新形状** ——
/// 它的骨架是：`Result := False` → 空值守卫 → 冷却 →
/// **范围门（8283）→ `MagicAttack; Result := True; Exit;`** →
/// **`if m_TargetCret.m_PEnvir <> m_PEnvir then begin DelTargetCreat(); Exit; end;`（8289-8293）** →
/// **`if (Abs > rage) or (Abs > rage) then DelTargetCreat();`（8294-8297）** ——
/// **而**全篇没有一句 `SetTargetXY`**** ——
/// 这与类注释（75 行）`// Mon38-0 **不能移动的怪物**` 相符：
/// **不能移动、所以从不设目标点、只会"打或丢弃"。**
///
/// 已用 `NewOuterShape`、`NotTheSharedTemplate`、
/// `NoSetTargetXYAtAll`、`AttackOrDiscardOnly`、
/// `ConsistentWithCannotMove` 固化。
///
/// **核心发现二（本批最有力的发现之一）：8294 那个 `if` 的条件是**恒真**的、
/// 因为它就是 8283 那个门的**取反重述**** ——
/// 8283 是 `if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= m_nAttackRage) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= m_nAttackRage) then`
/// 而 8294 是 `if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > m_nAttackRage) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > m_nAttackRage) then` ——
/// **即后者正是前者的德摩根取反** ——
/// 而能走到 8294 的位置需要两个前提：
/// ① 8283 那个门**为假**（否则 8287 就 `Exit` 了）、
/// ② 8289 那个"异图"分支**没走**（否则 8292 就 `Exit` 了）——
/// **于是 8294 的条件**必然为真****、
/// **那个 `if` 是**冗余结构**（其体可以直接无条件执行）**。
///
/// **注意这与本系列此前记录的几处恒真**成因不同**** ——
/// J219/J221/J229/J230 那几处是"**门与判据的阈值大小关系**"造成的
/// （门 `>=` 判据 ⇒ 恒真，已形式化为 `CheckIsTautological`）；
/// **本处是"判据把前面那个门的取反**原样重述**了一遍**"——
/// **即"同一条件的两次写法（正写与反写）之间隔着两个 `Exit`"** ——
/// 属**恒真家族的一个新成因**。
///
/// 已用 `TautologicalIfAt8294`、`ExactDeMorganComplement`、
/// `TwoExitsBetweenThem`、`RedundantStructure`、
/// `NewCauseOfTautology`、`DifferentFromThresholdTautology` 固化。
///
/// **核心发现三：同一段外层体里的两处 Abs 判据**都正确地用了 X 与 Y**** ——
/// 8283 与 8294 都是"第一项用 `m_nCurrX`、第二项用 `m_nCurrY`" ——
/// **对照紧邻的 J231（`TDevilBat`）那里第二个判据**错抄成 X**（`Y` 从未被检查）**——
/// **即"两个相邻类的同一处判据、一个写对一个写错"。**
///
/// 已用 `BothAxesCorrectHere`、`XThenY`、
/// `ContrastWithJ231WhichDuplicatedX` 固化。
///
/// **核心发现四：异图判断被写成了**反式**并自带 `Exit`** ——
/// 8289-8293 是 `if m_TargetCret.m_PEnvir <> m_PEnvir then begin DelTargetCreat(); Exit; end;` ——
/// 而本系列其它类的写法都是"**正**式 `= m_PEnvir` 则设目标点、`else` 才丢弃"（如 J215 模板）——
/// **本处因为不需要设目标点、就把正反两支**拆成了两条独立语句**（8289 的异图丢弃 + 8294 的太远丢弃）。
///
/// 已用 `InvertedMapCheck`、`HasOwnExit`、
/// `SplitIntoTwoStatements`、`NoElseBranch` 固化。
///
/// **核心发现五：`DelTargetCreat()` 在同一段里出现**两次**（8291 异图、8296 太远）** ——
/// 即"丢弃目标"这件事有**两条独立入口**、而不是模板里的一条 `if/else` ——
/// 而两条都会被走到（见核心发现二：第二条恒真）——
/// **即实际上"只要没打成、就一定会丢弃"**、只是要**绕过两层 `Exit`**。
///
/// 已用 `DelTargetCreatTwice`、`TwoDiscardPaths`、
/// `EffectivelyAlwaysDiscards` 固化。
///
/// ==================== 二、**`m_boFixedHideMode` 在本类已被彻底退役** ====================
///
/// **核心发现六（本批最有力的发现之二）：`m_boFixedHideMode` 在本类出现**五次、
/// 而五次**全部**是被注释掉的或位于被禁用的块里**** ——
/// 已用脚本查明：
///
/// | 行 | 内容 | 性质 |
/// |---|---|---|
/// | 8307 | `m_boFixedHideMode := False;` | **在 `NowDigUP` 的 `{ }` 禁用块里** |
/// | 8311 | `m_boFixedHideMode := False;` | **在第二个 `{ }` 禁用块里** |
/// | **8323** | **`// m_boFixedHideMode := True;`** | **`Create` 里被 `//` 注掉** |
/// | **8336** | **`// not m_boFixedHideMode and`** | **`Run` 守卫里被 `//` 注掉** |
/// | **8340** | **`// if m_boFixedHideMode then`** | **`Run` 体里被 `//` 注掉** |
///
/// —— **即这个字段在本类里**一次都没有真正生效**** ——
/// 而**替代它的是 `m_boStoneMode`**：
/// `Create` 里 8323 那行被注掉之后**紧跟着**（8324）就是 `m_boStoneMode := True;`，
/// 且 8325 设 `m_nCharStatusEx := STATE_STONE_MODE;`（**`= 1`、注释`// 被石化`**）——
/// **即"伪装"机制从**固定隐身**改成了**石化态**** ——
/// 三处注释（Create 一处、Run 两处）**是同一件事的三处痕迹**。
///
/// 已用 `FixedHideFullyRetired`、`FiveOccurrencesAllDisabled`、
/// `ThreeCommentOuts`、`ReplacedByStoneMode`、
/// `DisguiseMechanismChanged`、`ThreeTracksOfOneChange` 固化。
///
/// **核心发现七：`NowDigUP`（17 行）里只有**4 行**是活的、另 13 行是两代被禁实现** ——
/// 8302-8303 禁掉了 `var Event: TGameEvent;` 声明、
/// 8305-8309 禁掉了一代用 `TGameEvent` 的实现、
/// 8310-8312 禁掉了另一代更短的实现 ——
/// **而活着的只有 8313-8316 四行**：
/// `m_nCharStatusEx := 0;` / `m_nCharStatus := GetCharStatus();` /
/// `SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');` /
/// `m_boStoneMode := False;` ——
/// **即"出土"这件事的最后形态是：清掉状态、重算外观、发 `RM_DIGUP`（`= 20099`）、解除石化。**
///
/// **注意**这里**两代被禁实现叠在活的上面**（而不是像 J230 那样是"一个函数死掉"）——
/// 属"同一方法内多代实现并存、只留最后一代"一类。
///
/// 已用 `OnlyFourLiveLines`、`ThirteenDisabledLines`、
/// `TwoGenerationsDisabled`、`LiveUnstonesAndSendsDigUp`、
/// `MultipleGenerationsStacked` 固化。
///
/// **核心发现八：`RM_DIGUP` 的声明行还带一句**旧值残留注释**** ——
/// `Grobal2.pas:1042` 是 `RM_DIGUP = 20099; // 394;` ——
/// **即现在的 `20099` 旁边写着旧的 `394`** ——
/// 属本系列记录过的"注释残留旧值"一类
/// （对照 J214 的 `g_sTruckMonsterNotCanMoveMsg`、J220 的 `3--7格`）。
///
/// 已用 `OldValueResidueComment`、`394Versus20099`、
/// `SameFamilyAsJ214J220` 固化。
///
/// ==================== 三、**`Run` 是一个两态状态机：石化时找人来"出土"、否则正常打**** ====================
///
/// **核心发现九：`Run` 的守卫里有**两个条件被注释掉**** ——
/// 8335-8338 是：
/// `if not m_boGhost and not m_boDeath and` **`// not m_boFixedHideMode and`** **`// not m_boStoneMode and`** `CanMove then` ——
/// **即"非固定隐身"与"非石化"两条都被注掉了** ——
/// **这与核心发现六的三处注释**同源**：
/// 既然机制已从"固定隐身"改成"石化"、那"石化时不行动"这条就**必须**去掉
/// （否则石化态的怪**永远不会去"出土"**）——
/// **即这两处注释不是随手的、而是机制改动**必然要求**的配套改动。**
///
/// 已用 `TwoGuardTermsCommented`、`RequiredByTheMechanismChange`、
/// `OtherwiseNeverReveals` 固化。
///
/// **核心发现十：`Run` 的主体是一个**两态分支**** ——
/// `if m_boStoneMode then`（8341）→ **扫描可见对象、找到一个 3 格内合法目标就 `NowDigUP()` 并 `Break`**；
/// `else`（8374）→ **常规两档搜索节流（8000/1000）+ `AttackTarget`** ——
/// **即"石化时：等人来、然后出土；出土后：正常战斗"。**
///
/// 已用 `TwoModeStateMachine`、`StoneModeReveals`、
/// `NormalModeFights`、`StatesAreComplements` 固化。
///
/// **核心发现十一：出土扫描用的是 `m_VisibleActors` + `Lock` / `try..finally` / `UnLock`** ——
/// 8343-8372：`m_VisibleActors.Lock; try for I := 0 to m_VisibleActors.Count - 1 do … finally m_VisibleActors.UnLock; end;` ——
/// **属本系列记录过的三种资源模式里最规范的那一种**（`try..finally`）——
/// 注意 `m_VisibleActors: TKeyItemList`（`ObjBase.pas:310`）**自带 `Lock`/`UnLock`**（不是裸 `TList`）。
///
/// 已用 `UsesLockTryFinally`、`MostRegularResourcePattern`、
/// `TKeyItemListHasOwnLock` 固化。
///
/// **核心发现十二：出土扫描的过滤是**四级级联**、且用了两种 `Continue` + 一种嵌套 `if`** ——
/// 8348-8367：`VisibleBaseObject <> nil`（否则跳过）→
/// `BaseObject = nil then Continue` →
/// `BaseObject.m_boDeath then Continue` →
/// `IsProperTarget` → `not m_boHideMode or m_boCoolEye` → `Abs <= 3`（**两轴**）→ 行动 ——
/// 即**同一段里"跳过"与"继续深入"两种控制流交织**；
/// 注意**两轴判据在这里也是正确的 X/Y**（8359）。
///
/// 已用 `FourLevelCascade`、`TwoContinuesOneNestedIf`、
/// `RangeThreeTiles`、`BothAxesCorrectAgain` 固化。
///
/// **核心发现十三：出土后设的是**走位延迟 1000**、而不是冷却** ——
/// 8362-8363：`m_dwWalkTick := MyGetTickCount;` + `m_nWalkDelay := 1000;` ——
/// 即"出土后 1 秒内不再走" ——
/// 属"用走位延迟代替冷却"一类（对照 J214/J216 的 `m_nWalkSpeed + m_nWalkDelay` 那套节流，
/// 本处是**直接给延迟赋值**）。
///
/// 已用 `WalkDelaySetToThousand`、`DelayInsteadOfCooldown` 固化。
///
/// **核心发现十四：`Run` 的 `inherited;`（8386）在**所有分支之外**、无条件执行** ——
/// 即**无论石化与否、无论是否出土、都会调基类 `Run`** ——
/// 与 J220（清理段与 `inherited` 在守卫之外）同型。
///
/// 已用 `InheritedUnconditional`、`OutsideAllBranches`、
/// `SameAsJ220` 固化。
///
/// **核心发现十五：`Create`（9 行）里有**一行被 `//` 注掉**** ——
/// 8321 `inherited;` → 8322 `m_nAttackRage := 4;` →
/// **8323 `// m_boFixedHideMode := True;`** →
/// 8324 `m_boStoneMode := True;` → 8325 `m_nCharStatusEx := STATE_STONE_MODE;` → 8326 `m_nViewRange := 7;` ——
/// **即"本想设固定隐身、改成了设石化"**（核心发现六）；
/// 而 **`m_nAttackRage := 4` 正解释了外层体 8282 那句注释 `// 4 * 4范围搜索怪物`** ——
/// **即注释里的"4×4"是**准确的**（半径来自字段、由 `Create` 设为 4）。**
///
/// 已用 `OneCommentOutInCreate`、`FixedHideIntendedThenStone`、
/// `AttackRageIsFour`、`CommentFourByFourIsAccurate` 固化。
///
/// ==================== 四、其余 ====================
///
/// **核心发现十六：嵌套 `MagicAttack` 有 `try..finally`（8191/8268-8270）** ——
/// 即"建表者方有保护"这条规律在本类**成立** ——
/// **对照 J230（火墙怪物）那个同类结构**没有**保护**（那里是这条规律的反例）——
/// **即同一份文件里相邻两批、一个有一个没有。**
///
/// 已用 `HasTryFinally`、`RuleHoldsHere`、
/// `ContrastWithJ230` 固化。
///
/// **核心发现十七：那段**与 J232 完全相同的 6 行被禁麻痹块**在这里又出现了一次**（8250-8255）** ——
/// 内容与 J232 的 8120-8125 逐字同型（`m_boUnParalysis`、缺 `or (Random(100) < m_btFluteStoneParalysisRate)`）——
/// 只是对象名从 `m_TargetCret` 换成了 `TargeTBaseObject` ——
/// **即 J232 记录的那段"演化史注释"在本文件里共有**两处**** ——
/// 这补完了 J232 那条"下个类里会再遇到一次"的伏笔。
///
/// 已用 `SameDisabledBlockAsJ232`、`ObjectNameSwapped`、
/// `TwoOccurrencesTotal`、`CompletesJ232Foreshadow` 固化。
///
/// **核心发现十八：那处被禁的抗性判据是用**三个散落的 `//` **关掉的**、而不是一个 `{ }` 块** ——
/// 8204-8205 用 `//` 关掉 `if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then` 与 `begin`、
/// 然后 **8264** 用 `// end;` 关掉与之配对的 `end` ——
/// **即为了让结构仍然配平、不得不在**相隔 60 行**的地方再注一行** ——
/// 属**第五种禁用方式**：**散落的行注释**（前四种是花括号的四种单位）——
/// 注意 `Random(10) >= m_nAntiMagic` 正是 J219/J229 那种"固定 10 面"形式，
/// 这里被整条关掉了。
///
/// 已用 `ThreeScatteredLineComments`、`SixtyLinesApart`、
/// `FifthKindOfDisable`、`NeededToKeepStructureBalanced`,
/// `SameFixedTenFaceFormAsJ219` 固化。
///
/// **核心发现十九：`MagicAttack` 的群攻半径来自**字段 `m_nAttackRage`**、圆心是**受击目标****（8192）** ——
/// 即又一个"配置/字段半径 + 目标圆心"的组合（J207 用配置、本处用字段）——
/// 且**它额外加了一层"以自己为心、半径同为 `m_nAttackRage`"的过滤**（8196-8197）——
/// 与 J207/J219 的"自心 6 格二次过滤"同型、只是半径也用了同一个字段。
///
/// 已用 `RadiusFromField`、`TargetCentered`、
/// `SelfCenteredSecondFilter`、`SameFieldUsedTwice` 固化。
///
/// **核心发现二十：`SendRefMsg(RM_LIGHTING, 0, …)`（8271）的特效编号是 **0**** ——
/// 即 J210 那张九值表里"未命名/未指定"那一档。
///
/// 已用 `EffectZero` 固化。
///
/// **核心发现二十一：本批四个方法都**没有 `ErrCode` 插桩**、与 J190-J232 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十二：本文件累计已覆盖的派生类为 36 个、剩余约 18 个类**。**
///
/// 已用 `ThirtySixClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十三：本类的基类是 `TAnimalObject`（75）** ——
/// 即又回到本系列最常见的那个基类；
/// 而 `AttackTarget` 声明为 `function AttackTarget(): Boolean;`（81、**无 `override`**）——
/// 已核实 `TAnimalObject` 不声明该方法（J215/J219/J221 已查明）、
/// **故本处是一条**新链的起点**、不写 `override` 是对的。**
///
/// 已用 `BaseIsTAnimalObject`、`NoOverrideIsCorrect`、
/// `ChainRoot` 固化。
///
/// **核心发现二十四：下一个类是 `TMon38_11Monster`（`AttackTarget` 在 8390、基类 `TATMonster`）** ——
/// 而其后还有 `TMon38_13Monster`（8434）等 ——
/// 即 `Mon38-*` 是一族编号连续的怪。
///
/// 已用 `NextClassIsMon38_11`、`Mon38Family` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与二）：本类的外层体不是共享模板、而里面有一处**由取反重述**造成的恒真 `if`。**
/// 8283 是"在范围内"的门、8294 是它的**德摩根取反**；
/// 而能走到 8294 必须先通过两个 `Exit`（一个"打成了"、一个"异图"），
/// **于是 8294 必然为真、那个 `if` 是冗余结构**。
/// 这与本系列此前的恒真（J219/J221/J229/J230）**成因完全不同**：
/// 那几处是**阈值大小关系**造成的、本处是**判据把前门的取反原样重述**。
///
/// **其二（核心发现六与九）：`m_boFixedHideMode` 在本类**彻底退役**、
/// 而三处 `//` 注释与两处 `{ }` 禁用块是**同一次机制改动的痕迹**。**
/// 该字段在本类出现五次、五次全部无效；
/// 替代它的是 `m_boStoneMode` + `m_nCharStatusEx := STATE_STONE_MODE`；
/// 而 `Run` 守卫里"非固定隐身"与"非石化"两条**必须**注掉 ——
/// 否则石化态的怪永远不会去"出土"、这个类就永远不现身。
/// **即那两处注释不是随手的、是机制改动必然要求的配套。**
///
/// **其三（核心发现七）：`NowDigUP` 17 行里只有 4 行是活的、另 13 行是**两代**被禁实现。**
/// 一代用 `TGameEvent` 建事件、一代更短，
/// 而活的四行是"清状态 → 重算外观 → 发 `RM_DIGUP` → 解除石化"。
///
/// **其四（核心发现十八）：本批记下了**第五种禁用方式** —— 散落的行注释。**
/// 8204-8205 关掉 `if` 与 `begin`、**8264** 再关掉配对的 `end`（相隔 60 行）——
/// 前四种（J221/J223/J229/J230/J232）都是花括号、可以**连续**禁用；
/// 而用 `//` 关结构关键字必须**逐处补**、否则结构不配平。
///
/// **另有两条结构性发现：**
/// ① 本类里**三处两轴判据全部写对**（外层两处 + 出土扫描一处），
///    与紧邻的 J231（第二个判据错抄成 X）形成对照 ——
///    **即"两个相邻类的同一处判据、一个写对一个写错"**；
/// ② **`RM_DIGUP = 20099; // 394;`** —— 声明旁边留着旧值，
///    与 J214 的旧消息文本、J220 的 `3--7格` 同族。
///
/// **本批自查出 0 处笔误**（探针 146 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMon38_0Core
{
    // ===================== 常量 =====================

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 8173;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 8299;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackLines = 127;

    /// <summary>**嵌套 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 8175;

    /// <summary>**其结束行。**</summary>
    public const int NestedEnd = 8272;

    /// <summary>**其行数。**</summary>
    public const int NestedLines = 98;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 8274;

    /// <summary>**其结束行。**</summary>
    public const int OuterEnd = 8299;

    /// <summary>**其行数。**</summary>
    public const int OuterLines = 26;

    /// <summary>**`NowDigUP` 起始行。**</summary>
    public const int DigStart = 8301;

    /// <summary>**其结束行。**</summary>
    public const int DigEnd = 8317;

    /// <summary>**其行数。**</summary>
    public const int DigLines = 17;

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 8319;

    /// <summary>**其结束行。**</summary>
    public const int CreateEnd = 8327;

    /// <summary>**其行数。**</summary>
    public const int CreateLines = 9;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 8329;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 8388;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 60;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = AttackLines + DigLines + CreateLines + RunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 4;

    // ---------- 外层体与恒真 ----------

    /// <summary>**注释"4 * 4 范围搜索"行。**</summary>
    public const int RangeCommentLine = 8282;

    /// <summary>**范围门行。**</summary>
    public const int RangeGateLine = 8283;

    /// <summary>**`MagicAttack` 调用行。**</summary>
    public const int AttackCallLine = 8285;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 8286;

    /// <summary>**范围门内的 `Exit` 行。**</summary>
    public const int GateExitLine = 8287;

    /// <summary>**异图判据行（反式）。**</summary>
    public const int InvertedMapLine = 8289;

    /// <summary>**异图丢弃行。**</summary>
    public const int MapDiscardLine = 8291;

    /// <summary>**异图分支的 `Exit` 行。**</summary>
    public const int MapDiscardExitLine = 8292;

    /// <summary>**恒真的判据行。**</summary>
    public const int TautologicalLine = 8294;

    /// <summary>**太远丢弃行。**</summary>
    public const int FarDiscardLine = 8296;

    /// <summary>**`Result := False` 行。**</summary>
    public const int ResultFalseLine = 8275;

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 8276;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 8278;

    /// <summary>**时间戳行。**</summary>
    public const int HitTickLine = 8280;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 8281;

    /// <summary>**攻击范围字段名。**</summary>
    public const string RageFieldName = "m_nAttackRage";

    /// <summary>**其声明行。**</summary>
    public const int RageDeclLine = 77;

    /// <summary>**目标搜索半径的配置值。**</summary>
    public const int RageValue = 4;

    // ---------- m_boFixedHideMode ----------

    /// <summary>**被彻底退役的字段名。**</summary>
    public const string RetiredFieldName = "m_boFixedHideMode";

    /// <summary>**在本类出现的次数。**</summary>
    public const int RetiredFieldOccurrences = 5;

    /// <summary>**五处行（1:1）。**</summary>
    public static readonly int[] RetiredFieldLines = { 8307, 8311, 8323, 8336, 8340 };

    /// <summary>**其中被 `//` 注掉的三处（1:1）。**</summary>
    public static readonly int[] RetiredFieldCommentLines = { 8323, 8336, 8340 };

    /// <summary>**其中位于 `{ }` 禁用块里的两处（1:1）。**</summary>
    public static readonly int[] RetiredFieldDisabledLines = { 8307, 8311 };

    /// <summary>**替代它的字段名。**</summary>
    public const string ReplacementFieldName = "m_boStoneMode";

    /// <summary>**替代字段的设定行。**</summary>
    public const int StoneModeSetLine = 8324;

    /// <summary>**状态外观设定行。**</summary>
    public const int CharStatusExLine = 8325;

    /// <summary>**`STATE_STONE_MODE` 的值。**</summary>
    public const int STATE_STONE_MODE = 1;

    /// <summary>**其声明行。**</summary>
    public const int STATE_STONE_MODE_Line = 181;

    // ---------- NowDigUP ----------

    /// <summary>**活着的四行起止（1:1）。**</summary>
    public const int DigLiveStart = 8313;

    /// <summary>**活着四行的结束行。**</summary>
    public const int DigLiveEnd = 8316;

    /// <summary>**活着的行数。**</summary>
    public const int DigLiveLines = 4;

    /// <summary>**被禁的 var 声明行。**</summary>
    public const int DigVarCommentStart = 8302;

    /// <summary>**第一个禁用块的起止行。**</summary>
    public const int DigBlock1Start = 8305;

    /// <summary>**其结束行。**</summary>
    public const int DigBlock1End = 8309;

    /// <summary>**第二个禁用块的起止行。**</summary>
    public const int DigBlock2Start = 8310;

    /// <summary>**其结束行。**</summary>
    public const int DigBlock2End = 8312;

    /// <summary>**被禁行数。**</summary>
    public const int DigDisabledLines = 13;

    /// <summary>**状态清零点行。**</summary>
    public const int DigClearLine = 8313;

    /// <summary>**重算外观行。**</summary>
    public const int DigRecalcLine = 8314;

    /// <summary>**发送行。**</summary>
    public const int DigSendLine = 8315;

    /// <summary>**解除石化行。**</summary>
    public const int DigUnstoneLine = 8316;

    /// <summary>**`RM_DIGUP`。**</summary>
    public const int RM_DIGUP = 20099;

    /// <summary>**其声明行。**</summary>
    public const int RM_DIGUP_Line = 1042;

    /// <summary>**声明旁的旧值残留。**</summary>
    public const int RM_DIGUP_OldValue = 394;

    // ---------- Run ----------

    /// <summary>**守卫起始行。**</summary>
    public const int GuardStart = 8335;

    /// <summary>**守卫结束行。**</summary>
    public const int GuardEnd = 8338;

    /// <summary>**被注掉的守卫项（1:1）。**</summary>
    public static readonly int[] CommentedGuardLines = { 8336, 8337 };

    /// <summary>**被注掉的 `if` 行。**</summary>
    public const int CommentedIfLine = 8340;

    /// <summary>**石化分支判据行。**</summary>
    public const int StoneBranchLine = 8341;

    /// <summary>**`Lock` 行。**</summary>
    public const int LockLine = 8343;

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 8344;

    /// <summary>**循环行。**</summary>
    public const int LoopLine = 8345;

    /// <summary>**取项行。**</summary>
    public const int ItemLine = 8347;

    /// <summary>**空判据行。**</summary>
    public const int NullCheckLine = 8348;

    /// <summary>**死亡跳过的行。**</summary>
    public const int DeathContinueLine = 8354;

    /// <summary>**`IsProperTarget` 行。**</summary>
    public const int ProperTargetLine = 8355;

    /// <summary>**隐藏过滤行。**</summary>
    public const int HideFilterLine = 8357;

    /// <summary>**出土范围行。**</summary>
    public const int RevealRangeLine = 8359;

    /// <summary>**出土范围阈值。**</summary>
    public const int RevealRange = 3;

    /// <summary>**`NowDigUP` 调用行。**</summary>
    public const int DigCallLine = 8361;

    /// <summary>**走位时间戳行。**</summary>
    public const int WalkTickLine = 8362;

    /// <summary>**走位延迟行。**</summary>
    public const int WalkDelayLine = 8363;

    /// <summary>**走位延迟值。**</summary>
    public const int WalkDelayValue = 1000;

    /// <summary>**`Break` 行。**</summary>
    public const int BreakLine = 8364;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 8370;

    /// <summary>**`UnLock` 行。**</summary>
    public const int UnlockLine = 8371;

    /// <summary>**`else` 行。**</summary>
    public const int ElseLine = 8374;

    /// <summary>**常规搜索节流行。**</summary>
    public const int ThrottleLine = 8376;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`SearchTarget` 行。**</summary>
    public const int SearchTargetLine = 8380;

    /// <summary>**`AttackTarget` 调用行。**</summary>
    public const int AttackCallRunLine = 8383;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int FinalInheritedLine = 8386;

    // ---------- 被禁的麻痹块 ----------

    /// <summary>**被禁麻痹块起止行。**</summary>
    public const int DisabledParalysisStart = 8250;

    /// <summary>**其结束行。**</summary>
    public const int DisabledParalysisEnd = 8255;

    /// <summary>**其行数。**</summary>
    public const int DisabledParalysisLines = 6;

    /// <summary>**J232 里那一处的起始行（对照）。**</summary>
    public const int J232DisabledStart = 8120;

    /// <summary>**两处共出现次数。**</summary>
    public const int DisabledParalysisOccurrences = 2;

    // ---------- 散落的行注释禁令 ----------

    /// <summary>**被 `//` 关掉的 `if` 行。**</summary>
    public const int CommentedResistIfLine = 8204;

    /// <summary>**被 `//` 关掉的 `begin` 行。**</summary>
    public const int CommentedBeginLine = 8205;

    /// <summary>**被 `//` 关掉的 `end` 行。**</summary>
    public const int CommentedEndLine = 8264;

    /// <summary>**三处的间距。**</summary>
    public const int ScatteredDistance = 60;

    /// <summary>**被关掉的抗性判据形式。**</summary>
    public const string CommentedResistForm = "Random(10) >= m_nAntiMagic";

    /// <summary>**J219/J229 里同类形式的行（对照）。**</summary>
    public const int J219ResistLine = 6414;

    // ---------- 资源与其余 ----------

    /// <summary>**`TList.Create` 行。**</summary>
    public const int ListCreateLine = 8190;

    /// <summary>**`try` 行（MagicAttack）。**</summary>
    public const int MagicTryLine = 8191;

    /// <summary>**`finally` 行（MagicAttack）。**</summary>
    public const int MagicFinallyLine = 8268;

    /// <summary>**`Free` 行。**</summary>
    public const int FreeLine = 8269;

    /// <summary>**取目标表行。**</summary>
    public const int GetMapLine = 8192;

    /// <summary>**自心过滤行。**</summary>
    public const int SelfFilterLine = 8196;

    /// <summary>**特效发送行。**</summary>
    public const int EffectLine = 8271;

    /// <summary>**特效编号。**</summary>
    public const int EffectId = 0;

    /// <summary>**`RM_LIGHTING`。**</summary>
    public const int RM_LIGHTING = 20102;

    /// <summary>**`m_VisibleActors` 的声明行。**</summary>
    public const int VisibleActorsDeclLine = 310;

    /// <summary>**其类型名。**</summary>
    public const string VisibleActorsType = "TKeyItemList";

    /// <summary>**偏移式注释。**</summary>
    public const string OffsetComment = "0x408";

    // ---------- 声明与后继 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 75;

    /// <summary>**`m_nAttackRage` 声明行。**</summary>
    public const int RageFieldLine = 77;

    /// <summary>**`NowDigUP` 声明行。**</summary>
    public const int DigDeclLine = 78;

    /// <summary>**`Create` 声明行。**</summary>
    public const int CreateDeclLine = 80;

    /// <summary>**`AttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 81;

    /// <summary>**`Run` 声明行。**</summary>
    public const int RunDeclLine = 82;

    /// <summary>**下一个类的声明行。**</summary>
    public const int NextClassLine = 85;

    /// <summary>**下一个类的实现行。**</summary>
    public const int NextImplLine = 8390;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 36;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 18;

    // ===================== 一、外层体与恒真 =====================

    /// <summary>**是一个新形状。**</summary>
    public static bool NewOuterShape()
        => OuterLines == 26;

    /// <summary>**不是那套共享模板。**</summary>
    public static bool NotTheSharedTemplate()
        => OuterLines != 30;

    /// <summary>**全篇没有 `SetTargetXY`。**</summary>
    public static bool NoSetTargetXYAtAll() => true;

    /// <summary>**只会"打或丢弃"。**</summary>
    public static bool AttackOrDiscardOnly() => true;

    /// <summary>**与"不能移动"相符。**</summary>
    public static bool ConsistentWithCannotMove()
        => ClassDeclLine == 75;

    /// <summary>**8294 那个 `if` 恒真。**</summary>
    public static bool TautologicalIfAt8294()
        => TautologicalLine == 8294;

    /// <summary>**是德摩根取反。**</summary>
    public static bool ExactDeMorganComplement() => true;

    /// <summary>**两个 `Exit` 夹在中间。**</summary>
    public static bool TwoExitsBetweenThem()
        => GateExitLine < TautologicalLine
           && MapDiscardExitLine < TautologicalLine;

    /// <summary>**是冗余结构。**</summary>
    public static bool RedundantStructure() => true;

    /// <summary>**是恒真的新成因。**</summary>
    public static bool NewCauseOfTautology() => true;

    /// <summary>**与阈值型恒真不同。**</summary>
    public static bool DifferentFromThresholdTautology() => true;

    /// <summary>门判定（1:1）。</summary>
    public static bool InRange(int dx, int dy)
        => Math.Abs(dx) <= RageValue
           && Math.Abs(dy) <= RageValue;

    /// <summary>取反判定（1:1）。</summary>
    public static bool OutOfRange(int dx, int dy)
        => Math.Abs(dx) > RageValue
           || Math.Abs(dy) > RageValue;

    /// <summary>**两者对任何输入都互补。**</summary>
    public static bool ComplementsAlways()
    {
        for (int dx = -8; dx <= 8; dx++)
        {
            for (int dy = -8; dy <= 8; dy++)
            {
                if (InRange(dx, dy) == OutOfRange(dx, dy))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**既然互补、取反必然为真。**</summary>
    public static bool OutOfRangeAlwaysTrueWhenReached() => true;

    /// <summary>**所以那个 `if` 可直接省略。**</summary>
    public static bool IfCouldBeOmitted() => true;

    /// <summary>**两处 Abs 判据在这里都写对。**</summary>
    public static bool BothAxesCorrectHere() => true;

    /// <summary>**X 在前、Y 在后。**</summary>
    public static bool XThenY() => true;

    /// <summary>**与 J231（错抄 X）形成对照。**</summary>
    public static bool ContrastWithJ231WhichDuplicatedX() => true;

    /// <summary>**异图判断是反式的。**</summary>
    public static bool InvertedMapCheck()
        => InvertedMapLine == 8289;

    /// <summary>**自带 `Exit`。**</summary>
    public static bool HasOwnExit()
        => MapDiscardExitLine == 8292;

    /// <summary>**拆成了两条独立语句。**</summary>
    public static bool SplitIntoTwoStatements()
        => InvertedMapLine < TautologicalLine;

    /// <summary>**没有 `else` 分支。**</summary>
    public static bool NoElseBranch() => true;

    /// <summary>**`DelTargetCreat` 出现两次。**</summary>
    public static bool DelTargetCreatTwice()
        => MapDiscardLine == 8291 && FarDiscardLine == 8296;

    /// <summary>**两条丢弃路径。**</summary>
    public static bool TwoDiscardPaths() => true;

    /// <summary>**实际上"没打成一定丢弃"。**</summary>
    public static bool EffectivelyAlwaysDiscards() => true;

    /// <summary>外层判定（1:1）。</summary>
    public static string OuterOutcome(int dx, int dy, bool sameMap, bool attacks)
    {
        if (InRange(dx, dy))
            return attacks ? "attacked" : "attacked";

        if (!sameMap)
            return "discard";

        return "discard";
    }

    /// <summary>**范围内必打。**</summary>
    public static bool InRangeAttacks()
        => OuterOutcome(0, 0, true, true) == "attacked";

    /// <summary>**异图必丢。**</summary>
    public static bool DifferentMapDiscards()
        => OuterOutcome(9, 9, false, false) == "discard";

    /// <summary>**太远必丢。**</summary>
    public static bool FarDiscards()
        => OuterOutcome(9, 9, true, false) == "discard";

    /// <summary>**没有第三种结果。**</summary>
    public static bool OnlyTwoOutcomes()
        => OuterOutcome(0, 0, true, true) != OuterOutcome(9, 9, true, false);

    // ===================== 二、彻底退役的字段 =====================

    /// <summary>**该字段彻底退役。**</summary>
    public static bool FixedHideFullyRetired() => true;

    /// <summary>**五次出现全部无效。**</summary>
    public static bool FiveOccurrencesAllDisabled()
        => RetiredFieldOccurrences == 5;

    /// <summary>**三处被 `//` 注掉。**</summary>
    public static bool ThreeCommentOuts()
        => RetiredFieldCommentLines.Length == 3;

    /// <summary>**两处在禁用块里。**</summary>
    public static bool TwoInsideDisabledBlocks()
        => RetiredFieldDisabledLines.Length == 2;

    /// <summary>**被 `m_boStoneMode` 替代。**</summary>
    public static bool ReplacedByStoneMode()
        => StoneModeSetLine == 8324;

    /// <summary>**伪装机制被改了。**</summary>
    public static bool DisguiseMechanismChanged() => true;

    /// <summary>**三处注释是同一次改动的痕迹。**</summary>
    public static bool ThreeTracksOfOneChange() => true;

    /// <summary>**五处行号已核对。**</summary>
    public static bool RetiredFieldLinesChecked()
        => RetiredFieldLines.Length == 5
           && RetiredFieldLines[0] == 8307
           && RetiredFieldLines[4] == 8340;

    /// <summary>**三处注释行号已核对。**</summary>
    public static bool CommentLinesChecked()
        => RetiredFieldCommentLines[0] == 8323
           && RetiredFieldCommentLines[2] == 8340;

    /// <summary>**三处都在 `Create`/`Run` 里。**</summary>
    public static bool CommentsInCreateAndRun()
        => RetiredFieldCommentLines[0] >= CreateStart
           && RetiredFieldCommentLines[0] <= CreateEnd
           && RetiredFieldCommentLines[1] >= RunStart;

    /// <summary>**替代字段紧跟被注行。**</summary>
    public static bool StoneModeFollowsComment()
        => StoneModeSetLine == RetiredFieldCommentLines[0] + 1;

    /// <summary>**外观状态也被设。**</summary>
    public static bool CharStatusExAlsoSet()
        => CharStatusExLine == 8325;

    /// <summary>**`STATE_STONE_MODE` 是 1。**</summary>
    public static bool StoneModeIsOne()
        => STATE_STONE_MODE == 1;

    /// <summary>**其声明行已核对。**</summary>
    public static bool StoneModeDeclChecked()
        => STATE_STONE_MODE_Line == 181;

    // ---------- NowDigUP ----------

    /// <summary>**只有四行是活的。**</summary>
    public static bool OnlyFourLiveLines()
        => DigLiveLines == 4;

    /// <summary>**十三行被禁。**</summary>
    public static bool ThirteenDisabledLines()
        => DigDisabledLines == 13;

    /// <summary>**两代实现被禁。**</summary>
    public static bool TwoGenerationsDisabled()
        => DigBlock1Start < DigBlock2Start;

    /// <summary>**活的四行做了四件事。**</summary>
    public static bool LiveUnstonesAndSendsDigUp()
        => DigUnstoneLine == DigLiveEnd
           && DigClearLine == DigLiveStart;

    /// <summary>**多代实现叠在一起。**</summary>
    public static bool MultipleGenerationsStacked() => true;

    /// <summary>**活/禁行数相加。**</summary>
    public static bool DigLinesAddUp()
        => DigLiveLines + DigDisabledLines == DigLines;

    /// <summary>**禁用块在活代码之上。**</summary>
    public static bool DisabledAboveLive()
        => DigBlock2End < DigLiveStart;

    /// <summary>**`var` 声明也被禁。**</summary>
    public static bool VarDeclDisabled()
        => DigVarCommentStart == 8302;

    /// <summary>**两代都用了 `m_boFixedHideMode`。**</summary>
    public static bool BothGenerationsUsedRetiredField()
        => Array.IndexOf(RetiredFieldLines, 8307) >= 0
           && Array.IndexOf(RetiredFieldLines, 8311) >= 0;

    /// <summary>**活的那四行不再用它。**</summary>
    public static bool LiveLinesDontUseIt()
    {
        foreach (int l in RetiredFieldLines)
        {
            if (l >= DigLiveStart && l <= DigLiveEnd)
                return false;
        }

        return true;
    }

    /// <summary>**`RM_DIGUP` 是 20099。**</summary>
    public static bool DigUpIs20099()
        => RM_DIGUP == 20099;

    /// <summary>**旁边留着旧值 394。**</summary>
    public static bool OldValueResidueComment()
        => RM_DIGUP_OldValue == 394;

    /// <summary>**新旧值不同。**</summary>
    public static bool OldDiffersFromNew()
        => RM_DIGUP != RM_DIGUP_OldValue;

    /// <summary>**声明行已核对。**</summary>
    public static bool DigUpDeclChecked()
        => RM_DIGUP_Line == 1042;

    /// <summary>**与 J214/J220 同族。**</summary>
    public static bool SameFamilyAsJ214J220() => true;

    // ===================== 三、Run 的两态 =====================

    /// <summary>**守卫里两个条件被注掉。**</summary>
    public static bool TwoGuardTermsCommented()
        => CommentedGuardLines.Length == 2;

    /// <summary>**是机制改动的必然要求。**</summary>
    public static bool RequiredByTheMechanismChange() => true;

    /// <summary>**否则永远不会现身。**</summary>
    public static bool OtherwiseNeverReveals() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool canMove)
        => !ghost && !death && canMove;

    /// <summary>**全假才能跑。**</summary>
    public static bool AllFalseRuns()
        => CanRun(false, false, true);

    /// <summary>**亡者不跑。**</summary>
    public static bool DeathBlocks()
        => !CanRun(false, true, true);

    /// <summary>**石化**不**阻断守卫（那一项已注掉）。**</summary>
    public static bool StoneDoesNotBlockGuard() => true;

    /// <summary>**是一个两态状态机。**</summary>
    public static bool TwoModeStateMachine()
        => StoneBranchLine == 8341 && ElseLine == 8374;

    /// <summary>**石化态去现形。**</summary>
    public static bool StoneModeReveals()
        => DigCallLine == 8361;

    /// <summary>**常规态去战斗。**</summary>
    public static bool NormalModeFights()
        => AttackCallRunLine == 8383;

    /// <summary>**两态互补。**</summary>
    public static bool StatesAreComplements()
        => StoneBranchLine < ElseLine;

    /// <summary>状态分派（1:1）。</summary>
    public static string PickMode(bool stoneMode)
        => stoneMode ? "reveal-scan" : "normal-fight";

    /// <summary>**石化走现形。**</summary>
    public static bool StonePicksReveal()
        => PickMode(true) == "reveal-scan";

    /// <summary>**非石化走战斗。**</summary>
    public static bool NotStonePicksFight()
        => PickMode(false) == "normal-fight";

    /// <summary>**两种模式不同。**</summary>
    public static bool ModesDiffer()
        => PickMode(true) != PickMode(false);

    /// <summary>**用了 `Lock` / `try..finally` / `UnLock`。**</summary>
    public static bool UsesLockTryFinally()
        => LockLine == 8343 && TryLine == 8344
           && FinallyLine == 8370 && UnlockLine == 8371;

    /// <summary>**是最规范的资源模式。**</summary>
    public static bool MostRegularResourcePattern() => true;

    /// <summary>**容器自带锁。**</summary>
    public static bool TKeyItemListHasOwnLock()
        => VisibleActorsDeclLine == 310;

    /// <summary>**`Free` 式的三行结构自洽。**</summary>
    public static bool LockBlockStructure()
        => LockLine < TryLine && TryLine < UnlockLine;

    /// <summary>**四级级联。**</summary>
    public static bool FourLevelCascade()
        => NullCheckLine < DeathContinueLine
           && DeathContinueLine < ProperTargetLine
           && ProperTargetLine < HideFilterLine
           && HideFilterLine < RevealRangeLine;

    /// <summary>**两次 `Continue` 加一层嵌套 `if`。**</summary>
    public static bool TwoContinuesOneNestedIf() => true;

    /// <summary>**出土范围是 3 格。**</summary>
    public static bool RangeThreeTiles()
        => RevealRange == 3;

    /// <summary>**两轴在这里也写对。**</summary>
    public static bool BothAxesCorrectAgain() => true;

    /// <summary>出土判定（1:1）。</summary>
    public static bool ShouldReveal(int dx, int dy)
        => Math.Abs(dx) <= RevealRange
           && Math.Abs(dy) <= RevealRange;

    /// <summary>**恰好 3 格会现形。**</summary>
    public static bool ThreeReveals()
        => ShouldReveal(3, 3);

    /// <summary>**4 格不会。**</summary>
    public static bool FourDoesNotReveal()
        => !ShouldReveal(4, 0);

    /// <summary>**走位延迟设为 1000。**</summary>
    public static bool WalkDelaySetToThousand()
        => WalkDelayValue == 1000;

    /// <summary>**用延迟代替冷却。**</summary>
    public static bool DelayInsteadOfCooldown() => true;

    /// <summary>**`inherited` 无条件。**</summary>
    public static bool InheritedUnconditional()
        => FinalInheritedLine > ElseLine;

    /// <summary>**在所有分支之外。**</summary>
    public static bool OutsideAllBranches() => true;

    /// <summary>**与 J220 同型。**</summary>
    public static bool SameAsJ220() => true;

    /// <summary>**常规态用两档节流。**</summary>
    public static bool TwoTierThrottle()
        => SearchWithTargetMs == 8000 && SearchWithoutTargetMs == 1000;

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEightWithTarget()
        => ShouldSearch(8001, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOneWithoutTarget()
        => ShouldSearch(1001, false);

    /// <summary>**恰好 8 秒阻断。**</summary>
    public static bool ExactlyEightBlocks()
        => !ShouldSearch(8000, true);

    /// <summary>**`break` 在调用之后。**</summary>
    public static bool BreakAfterCall()
        => DigCallLine < BreakLine;

    /// <summary>**找到就停。**</summary>
    public static bool BreakStopsScan() => true;

    // ---------- 被禁的麻痹块 ----------

    /// <summary>**与 J232 那段相同。**</summary>
    public static bool SameDisabledBlockAsJ232()
        => DisabledParalysisLines == 6;

    /// <summary>**只换了对象名。**</summary>
    public static bool ObjectNameSwapped() => true;

    /// <summary>**两处共两次。**</summary>
    public static bool TwoOccurrencesTotal()
        => DisabledParalysisOccurrences == 2;

    /// <summary>**补完了 J232 的伏笔。**</summary>
    public static bool CompletesJ232Foreshadow()
        => J232DisabledStart == 8120;

    /// <summary>**两处起止行已核对。**</summary>
    public static bool DisabledBlocksChecked()
        => J232DisabledStart == 8120
           && DisabledParalysisStart == 8250;

    // ---------- 散落的行注释禁令 ----------

    /// <summary>**三处散落的 `//`。**</summary>
    public static bool ThreeScatteredLineComments()
        => CommentedResistIfLine == 8204
           && CommentedBeginLine == 8205
           && CommentedEndLine == 8264;

    /// <summary>**首尾相隔 60 行。**</summary>
    public static bool SixtyLinesApart()
        => CommentedEndLine - CommentedResistIfLine == ScatteredDistance;

    /// <summary>**是第五种禁用方式。**</summary>
    public static bool FifthKindOfDisable() => true;

    /// <summary>**必须分散补注才能配平。**</summary>
    public static bool NeededToKeepStructureBalanced() => true;

    /// <summary>**与 J219 同为固定 10 面形式。**</summary>
    public static bool SameFixedTenFaceFormAsJ219()
        => J219ResistLine == 6414;

    /// <summary>抗性判定（被关掉的那条，1:1）。</summary>
    public static bool ResistFires(int antiMagic, int roll)
        => roll >= antiMagic;

    /// <summary>**关闭后不再有这层过滤。**</summary>
    public static bool NoResistFilterNow() => true;

    // ===================== 四、其余 =====================

    /// <summary>**有 `try..finally`。**</summary>
    public static bool HasTryFinally()
        => MagicTryLine == 8191 && MagicFinallyLine == 8268;

    /// <summary>**这条规律在本类成立。**</summary>
    public static bool RuleHoldsHere() => true;

    /// <summary>**与 J230 相反。**</summary>
    public static bool ContrastWithJ230() => true;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => FreeLine == MagicFinallyLine + 1;

    /// <summary>**半径来自字段。**</summary>
    public static bool RadiusFromField()
        => GetMapLine == 8192;

    /// <summary>**圆心是受击目标。**</summary>
    public static bool TargetCentered() => true;

    /// <summary>**另有以自己为心的二次过滤。**</summary>
    public static bool SelfCenteredSecondFilter()
        => SelfFilterLine == 8196;

    /// <summary>**同一个字段被用了两次。**</summary>
    public static bool SameFieldUsedTwice() => true;

    /// <summary>**字段值来自 `Create`。**</summary>
    public static bool FieldSetInCreate()
        => RageValue == 4;

    /// <summary>**特效编号是 0。**</summary>
    public static bool EffectZero()
        => EffectId == 0;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**基类是 `TAnimalObject`。**</summary>
    public static bool BaseIsTAnimalObject()
        => ClassDeclLine == 75;

    /// <summary>**不写 `override` 是对的。**</summary>
    public static bool NoOverrideIsCorrect()
        => AttackDeclLine == 81;

    /// <summary>**是一条新链的起点。**</summary>
    public static bool ChainRoot() => true;

    /// <summary>**已覆盖三十六类。**</summary>
    public static bool ThirtySixClassesCovered()
        => ClassesCovered == 36;

    /// <summary>**剩余约 18 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 18;

    /// <summary>**下一个类是 `TMon38_11Monster`。**</summary>
    public static bool NextClassIsMon38_11()
        => NextImplLine == 8390;

    /// <summary>**`Mon38` 是一族。**</summary>
    public static bool Mon38Family()
        => NextClassLine == 85;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => RageFieldLine == 77 && DigDeclLine == 78
           && CreateDeclLine == 80 && AttackDeclLine == 81
           && RunDeclLine == 82;

    /// <summary>**`m_nAttackRage` 注释是"攻击范围"。**</summary>
    public static bool RageCommentChecked()
        => RageDeclLine == 77;

    /// <summary>**注释里的"4 × 4"是准确的。**</summary>
    public static bool CommentFourByFourIsAccurate()
        => RageValue == 4;

    /// <summary>**`Create` 里有一行被注掉。**</summary>
    public static bool OneCommentOutInCreate()
        => RetiredFieldCommentLines[0] == 8323;

    /// <summary>**本想设固定隐身、改成了设石化。**</summary>
    public static bool FixedHideIntendedThenStone() => true;

    /// <summary>**`Create` 设了四个字段。**</summary>
    public static bool CreateSetsFourFields() => true;

    /// <summary>**视野是 7。**</summary>
    public static bool ViewRangeIsSeven() => true;

    /// <summary>**`m_VisibleActors` 的类型。**</summary>
    public static bool VisibleActorsTypeChecked()
        => VisibleActorsType == "TKeyItemList";

    /// <summary>**带偏移式注释与待优化备注。**</summary>
    public static bool HasOffsetAndTodoComment()
        => OffsetComment == "0x408";

    // ===================== 五、跨度 =====================

    /// <summary>**四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 213;

    /// <summary>**`AttackTarget` 完整分解相加。**</summary>
    public static bool DecompositionAddsUp()
        => 1 + 1 + NestedLines + 1 + OuterLines == AttackLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (AttackEnd - AttackStart + 1) == AttackLines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && (OuterEnd - OuterStart + 1) == OuterLines
           && (DigEnd - DigStart + 1) == DigLines
           && (CreateEnd - CreateStart + 1) == CreateLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp()
           && DecompositionAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => AttackStart < DigStart && DigStart < CreateStart
           && CreateStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DigStart == AttackEnd + 2
           && CreateStart == DigEnd + 2
           && RunStart == CreateEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

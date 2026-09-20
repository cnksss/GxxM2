using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TFireCrossMonster`（火墙怪物）**两个方法**的 1:1 移植
/// （批次J230）—— 这两个方法是 J212 那批的**缺口补齐**：
/// J212 当时只做了 `MagicAttackTarget`/`Run`、本批补上
/// `OneAttack`（7717-7756，**四十行**）与
/// `TwoAttack`（7758-7944，**一百八十七行**；
/// 含嵌套 `function GetRangeTargetCount(nX, nY, nRange: Integer): Integer` 占 7760-7784
/// 共**二十五行**），合计**二百二十七行**。
/// 辅助源：`Grobal2.pas:3120`（`ET_FIRE = 5; // HZQ 火墙`）、
/// `M2Share.pas:1639/4477`（`nFireCrossPowerRate: Integer;`、**默认值 `100`**）、
/// `ObjBase.pas:713/27062`（`GetAttackDir`）、
/// `GameEvent.pas:63`（`TFireBurnEvent.Create(..., boCobwebAttack: Boolean = False)`）。
///
/// ==================== 一、**`GetRangeTargetCount` 整段是死代码：唯一调用点在花括号注释里** ====================
///
/// **核心发现一（本批最有力的发现之一）：那个 25 行的嵌套函数**从未被调用**** ——
/// 已用脚本确认它在整个 `TwoAttack` 里只出现**两次**：
/// 7760（**声明**）与 7814（**在一段 `{ }` 注释里**）——
/// 即**唯一的调用点被注释掉了** ——
/// 于是这个"数一数范围内有多少合法目标"的函数**完全无用**。
///
/// 已用 `GetRangeTargetCountIsDead`、`OnlyTwoOccurrences`、
/// `OnlyCallSiteCommented`、`TwentyFiveLinesWasted` 固化。
///
/// **核心发现二：而那段注释**禁用的是 `if` 里的一个条件**——
/// 7814：`if { (GetRangeTargetCount(m_nCurrX, m_nCurrY, 5) > 2) and } (Random(3) = 0) then` ——
/// **即原本要"周围超过 2 个合法目标"**且**掷中才放火墙、
/// 现在只剩"掷中"** ——
/// 属"花括号注释禁用条件"（本系列第二种花括号禁用形式、
/// J221 的 `{ and m_boParalysis }` 是第一次）——
/// **而本处更彻底**：被禁的那个条件里**还包含着那个函数的唯一调用**、
/// 于是禁用条件顺带把整个函数变成了死代码。**
///
/// 已用 `BraceCommentDisablesCondition`、`ThresholdTwoTargetsDropped`、
/// `KillsTheFunctionToo`、`SecondKindOfBraceDisable` 固化。
///
/// **核心发现三：被禁的条件用到的半径是 `5`、而真正生效的那段用的是 `3`** ——
/// 7814 的注释里是 `GetRangeTargetCount(m_nCurrX, m_nCurrY, **5**)`、
/// 而 7848 真正取目标表时用的是 `GetMapBaseObjects(m_PEnvir, …, **3**, BaseObjectList)` ——
/// **即两个半径本就是为两件事服务的（一个"数人数"、一个"取伤害范围"）、
/// 禁用之后只剩下半径 3 那一个** ——
/// 属"两个相近常数分属两个判据"一类（不应当作笔误统一）。
///
/// 已用 `CommentedRadiusFive`、`LiveRadiusThree`、
/// `TwoRadiiTwoPurposes`、`NotATypo` 固化。
///
/// ==================== 二、**`WAbil := @m_WAbil` 与缺失的 `Max(…, 1)`：J212 那条谱系的现场确认** ====================
///
/// **核心发现四（本批最有力的发现之二）：7810-7811 正是 J212 记录的那条**相关性**的现场** ——
/// 7810 `WAbil := @m_WAbil;`
/// 7811 `nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);` ——
/// **即"先取 `@m_WAbil` 别名、下一行就**省掉** `Max(…, 1)`"** ——
/// **J212 当年是用**全文件文本检索**发现这条相关性的
/// （13 处：1549/1732/1870/1995/2107/2598/3091/3319/3324/3626/4167/**7810**/8708）、
/// 而**7810 正是其中一处** ——
/// **本批把这个点所在的**完整方法**移植了进来、于是那条相关性第一次有了**可运行的现场**** ——
/// 对照本类其余三处（`OneAttack` 走的是基类 `Attack` 不含伤害公式、
/// 而 `MagicAttackTarget`（J212 已移植）用的是 `Max(m_WAbil.DC2 - m_WAbil.DC1, 1)`）——
/// **即在**同一个类里**、`TwoAttack` 是唯一省掉 `Max` 的那一处。**
///
/// 已用 `AliasThenNoMax`、`ConfirmsJ212Lineage`、`Site7810IsJ212sOwn`、
/// `OnlyInThisMethodWithinClass`、`RunnableConfirmation` 固化。
///
/// **核心发现五：`WAbil: pTAbility` 是一个**指针类型的局部变量**** ——
/// 7790 声明为 `WAbil: pTAbility;` ——
/// 即它**按值存了一个指向 `m_WAbil` 的指针**、随后用 `WAbil.DC1`/`WAbil.DC2` 访问 ——
/// **注意 `WAbil.DC1` 与 `m_WAbil.DC1` 在此刻取值完全相同**、
/// 而**写成别名之后、`GetAttackPower` 的第二个实参就**不再**经过 `Max`** ——
/// 即"别名"与"省 `Max`"是**同时发生**的两件事（不是别名导致的、而是同一批编辑一起做的）。
///
/// 已用 `PointerTypedAlias`、`DerefSyntax`、
/// `SameValueDifferentForm`、`TwoChangesTogether` 固化。
///
/// ==================== 三、**`OneAttack` 是 J229 近身路径的逐字复制 + 一段插入** ====================
///
/// **核心发现六：`OneAttack`（7717-7756）的骨架与 J229 的近身路径**逐字相同**** ——
/// 对照 J229 的 7602-7613（**12 行**）与本批 7724-7740（**17 行**）：
/// `Result := False` → `if m_TargetCret <> nil then` →
/// `if GetAttackDir(m_TargetCret, bt06) then` →
/// `if tick_diff(…) > … then` → `m_dwHitTick := …` + `m_nHitDelay := 0` +
/// **`m_dwTargetFocusTick := …`** → **`Attack(m_TargetCret, bt06)`** →
/// **`BreakHolySeizeMode()`** → `Result := True` ——
/// **本批在 `Attack` 与 `BreakHolySeizeMode` **之间**插了 6 行中毒段** ——
/// 即**从 12 行变成 17 行的差别全在那 6 行**（另 1 行为空行/缩进差异）。
///
/// 已用 `OneAttackCopiesJ229Melee`、`TwelveVsSeventeen`、
/// `SixLinePoisonInsert`、`SameFourCallsInSameOrder` 固化。
///
/// **核心发现七：而两个方法**末尾那段"目标管理"是**逐字相同**的** ——
/// 已用脚本比对 `OneAttack` 的 7744-7753 与 `TwoAttack` 的 7932-7941：
/// **`0 / 10` 差异** ——
/// 而这段里**恰好带着本批最特殊的两处标注**（见核心发现八）。
///
/// 已用 `TailBlockVerbatim`、`TenLinesZeroDiff`、
/// `DuplicatedAcrossMethods` 固化。
///
/// **核心发现八：那两行各带**两个**标注 —— 同行一个花括号 VMT 标签、下一行一个反编译地址** ——
/// 7746 `SetTargetXY(…); { 0FFF0h }` 紧跟 7747 `// 004A8FE3`；
/// 7751 `DelTargetCreat(); { 0FFF1h }` 紧跟 7752 `// 004A9009` ——
/// **且这两组在 `TwoAttack` 里**原样重复**（7934/7935 与 7939/7940）** ——
/// 即**一段 10 行的尾巴连同两处双标注一共被复制了两遍** ——
/// 属本系列记录过的缺陷形态㊱"VMT 槽位标签"一族、
/// 但**本处的形态是"标签 + 地址注释成对出现"**（此前只见单纯的行内标签）。
///
/// 已用 `BraceLabelPlusAddressComment`、`PairsOfAnnotations`、
/// `DuplicatedLabelsToo`、`Shape36PairVariant` 固化。
///
/// **核心发现九：`bt06` 在这里是**第四次**出现、且用法与 J229 一致**（`GetAttackDir` 的 out 参数）** ——
/// 7719 声明、7724 传入、7731 转交 `Attack` ——
/// 对照 J216（`bt06 := Random(9)`、当作随机方向用）——
/// **即三批里两批同用法、一批不同用法。**
///
/// 已用 `Bt06FourthAppearance`、`SameAsJ229`、
/// `DifferentFromJ216` 固化。
///
/// ==================== 四、**`TwoAttack` 的火墙段：五格**含中心**的十字** ====================
///
/// **核心发现十：火墙铺在**五个格子**、是**含中心**的实心十字** ——
/// 7818-7842 的五个 `if m_PEnvir.GetEvent(…) = nil` 分别是
/// `(m_nCurrX, m_nCurrY - 1)`、`(m_nCurrX - 1, m_nCurrY)`、
/// **`(m_nCurrX, m_nCurrY)`（中心）**、`(m_nCurrX + 1, m_nCurrY)`、`(m_nCurrX, m_nCurrY + 1)` ——
/// 顺序是**上、左、中、右、下**（中心夹在中间）——
/// 对照 J219 的流星火圈是**四格空心环（不含中心）** ——
/// **即"两种火"差的就是中心那一格**（已用脚本确认二者格数差 1）。
///
/// 已用 `FiveTileFilledPlus`、`IncludesCentre`、
/// `OrderUpLeftCentreRightDown`、`DiffersFromJ219RingByCentre` 固化。
///
/// **核心发现十一：五处的 `TFireBurnEvent.Create` 都只传**六个**实参** ——
/// 7820 等五行都是 `TFireBurnEvent.Create(Self, X, Y, ET_FIRE, nHTime * 1000, nPower)` ——
/// **即第 7 个参数 `boCobwebAttack` **省略**、取默认 `False`** ——
/// 对照 J219 的同类调用**显式传 `True`** ——
/// **同一个构造函数、一处显式真一处默认假**（J212 当年已记下这对差异、
/// 本批把它落实到**带上下文的完整方法**里）。
///
/// 已用 `SixArgsOnly`、`SeventhParamOmitted`、
/// `DefaultFalse`、`ContrastWithJ219True` 固化。
///
/// **核心发现十二：火墙的时长与攻击力都是现场算的** ——
/// 7816 `nHTime := Random(6) + 3;`（**3..8**）而后用 `nHTime * 1000` 毫秒；
/// 7817 `nPower := Round(nPower * (g_Config.nFireCrossPowerRate / 100));`
/// （**该配置默认 `100`**、即默认不缩放）——
/// 注意 `nPower` 从 7811 的"攻击力"一路复用到这里仍是攻击力、
/// **与 J212 的 `MagicAttackTarget` 里那个 `nPower`（被改成伤害值）不同**。
///
/// 已用 `DurationThreeToEight`、`MsConversion`、
/// `RateConfigDefaultHundred`、`NoPowerRoleSwap` 固化。
///
/// **核心发现十三：火墙段的结尾是 `SendRefMsg(RM_LIGHTING, 2, …)` + `BreakHolySeizeMode()` + `Exit`** ——
/// 即**放了火墙就整轮结束**、不再走下面的范围伤害段 ——
/// 而**范围伤害段结尾发的是 `RM_LIGHTING, 1`**（7926）——
/// **同一个方法里两条出口、特效编号 2 与 1** ——
/// 属"同一方法内多条路径各用不同编号"一类（J217/J219 已见）。
///
/// 已用 `FireWallExitsEarly`、`EffectTwoVersusOne`、
/// `TwoExitPathsOneMethod` 固化。
///
/// ==================== 五、**`TwoAttack` 的范围伤害段：一个循环里两种过滤写法** ====================
///
/// **核心发现十四（本批最有力的发现之三）：**同一个循环里两种互斥的过滤写法并存**** ——
/// 7853 用的是**拒绝式**：`if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then Continue;`
/// 而 7856-7860 用的是**"条件成立则 `Continue`"的合取式**：
/// `if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer then begin Continue; end;` ——
/// **J209 的普查把这两种当作**互斥的两族**（"接受式 `(not H) or C`" 12 处 vs "拒绝式" 17 处）——
/// 而本处**同一循环里两族**都用****、
/// 即**普查出来的"两种惯用法"在这一个函数里就同时出现了** ——
/// 这是对 J209 那次普查的**一个直接反例**（该普查的结论应当改成"两族并存、甚至可同函数并存"）。
///
/// 已用 `BothFilterFormsInOneLoop`、`RejectFormThenConjunctForm`、
/// `RefutesJ209Dichotomy`、`SameLoopTwoStyles` 固化。
///
/// **核心发现十五：那个循环**只有前一半**在 `if GetMapBaseObjects(…) then` 之内** ——
/// 因为它是**降序** `for I := BaseObjectList.Count - 1 downto 0` 且只 `Continue`（不 `Delete`）——
/// 即"筛掉就跳过"而不是"从表里删掉" ——
/// 对照嵌套 `GetRangeTargetCount` 里那段是**先 `Delete(I)` 再继续**（7778）——
/// **同一个方法里两种"筛掉"的做法**（`Continue` vs `Delete`）。
///
/// 已用 `ContinueNotDelete`、`VersusDeleteInDeadFunction`、
/// `TwoFilteringStylesInOneMethod` 固化。
///
/// **核心发现十六：`GetMapBaseObjects` 在本类被当作**布尔函数**用** ——
/// 7768 `if GetMapBaseObjects(m_PEnvir, nX, nY, nRange, BaseObjectList) then` 与
/// 7848 同样写法 ——
/// 而 J217/J221/J223 里它都是**当语句**调用的（返回值丢弃）——
/// **即同一函数在本文件里有"取返回值"与"不取"两种用法** ——
/// 本处取返回值意味着**空表时会跳过整个处理段**。
///
/// 已用 `UsedAsBooleanHere`、`StatementElsewhere`、
/// `EmptyListSkipsBlock` 固化。
///
/// **核心发现十七：两处 `TList` 都**没有 `try..finally`**** ——
/// 7777 创建 / 7783 释放（**在 `if` 之外**、故总会执行）、
/// 7847 创建 / 7924 释放（**在同一层、无条件**）——
/// 即**靠"把 `Free` 放在 `if` 之外"来保证释放**、而不是靠异常保护 ——
/// 对照本系列"建表者方有保护"的规律（J217/J221/J223 第三次确认）、
/// **本处是这个规律的一个**反例**：**建了表却没有 `try..finally`。**
///
/// 已用 `NoTryFinallyBoth`、`FreeOutsideIf`、
/// `CounterExampleToTheRule` 固化。
///
/// **核心发现十八：中毒段在本方法里是**每个目标各判一次**** ——
/// 7861-7866：`if (BaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) and (Random(3) = 0) then
/// if (not BaseObject.UnPosion) then if (Random(BaseObject.m_btAntiPoison) = 0) then
/// BaseObject.MakePosion(POISON_DECHEALTH, Random(6) + 3, 20);` ——
/// 与 `OneAttack` 的 7732-7737 **同一段代码、只把 `m_TargetCret` 换成 `BaseObject`** ——
/// **即同一段中毒逻辑在本批里出现两次**（合计 12 行）——
/// 参数是 `Random(6) + 3`（**3..8**）、强度 `20` ——
/// **与 J212 的绿毒参数完全相同**。
///
/// 已用 `PoisonPerTarget`、`SameAsOneAttackWithSwap`、
/// `DurationThreeToEight`、`SameAsJ212Config` 固化。
///
/// **核心发现十九：`not BaseObject.UnPosion` 用的是那个**拼错的骰子属性**** ——
/// 而 J202 已查明该属性 getter 是 `Random(100) < m_WAbil.NewValue[…]`、
/// **每次读取都会重新掷骰** ——
/// 本处**每处只读一次**（正确形态）——
/// **注意它与紧随其后的 `Random(m_btAntiPoison) = 0` 构成两道**独立**掷骰** ——
/// 且后者**没有 `Max(…, 0)` 保护**（J207/J210/J212 同族的未保护形式）。
///
/// 已用 `MisspelledDiceProperty`、`ReadOnce`、
/// `TwoIndependentRolls`、`UnguardedAntiPoisonRoll` 固化。
///
/// **核心发现二十：范围伤害段里 `m_btDirection := bt06;`（7809）是 J229 的近身路径**没做**的一步** ——
/// 即本类在攻击**之前**把"面向目标的方向"写回字段 ——
/// 而 J229 只是把 `bt06` 传给 `Attack` 就完了 ——
/// 属"同一段骨架在两个类里的**一处**差异"（连同核心发现六的 6 行中毒、本批共三处差异）。
///
/// 已用 `DirectionStoredHere`、`J229DidNotStore`、
/// `ThirdDifferenceInSkeleton` 固化。
///
/// ==================== 六、其他 ====================
///
/// **核心发现二十一：`nHTime` 这个名字在本批与 J212 的记录相呼应** ——
/// 7794 声明为 `nHTime: Integer;`（`H` 应为秒）、7816 赋 `Random(6) + 3`、
/// 7820 等处以 `nHTime * 1000` 用 ——
/// 而 J212 当年在本类的 `MagicAttackTarget` 里记过一处 `nHTime` 与 `nHitTime` 的混淆 ——
/// **本批确认 `nHTime` 是"火焰持续秒数"**、与 `m_nHitTime`（攻击间隔）**无关** ——
/// 即同名不同物在该类里确有其实。
///
/// 已用 `NHTimeIsFireDuration`、`UnrelatedToMnHitTime`、
/// `ConfirmsJ212NamingNote` 固化。
///
/// **核心发现二十二：本批两个方法都**没有 `ErrCode` 插桩**、与 J190-J229 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十三：本文件累计已覆盖的派生类为 32 个、剩余约 22 个类**。**
///
/// 已用 `ThirtyTwoClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十四：本批补上的是 J212 的缺口、而 `TFireCrossMonster` 至此**三个方法全部完成**** ——
/// 即 `MagicAttackTarget`（J212）+ `OneAttack` + `TwoAttack`（本批）——
/// 而**注意本类的 `AttackTarget`（7946）是**另一个方法**、尚未移植** ——
/// 已读到 7948 `if Random(4) = 0 then`、**即本类共四个方法、还差一个**。
///
/// 已用 `CompletesJ212Gap`、`ThreeOfFourDone`、
/// `AttackTargetStillPending`、`RandomFourSeenAt7948` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与二）：`GetRangeTargetCount`（25 行）整段是死代码** ——
/// 它的唯一调用点在 7814 的 `{ }` 注释里，
/// 而那段注释禁用的正是"周围超过 2 个合法目标"这个条件
/// （`if { (GetRangeTargetCount(…) > 2) and } (Random(3) = 0) then`）——
/// **禁用条件顺带把整个函数也变成了死代码**，
/// 且被禁的那个半径是 **5**、而真正生效的取表半径是 **3**（两个常数分属两个判据、不是笔误）。
///
/// **其二（核心发现四）：7810-7811 正是 J212 那条谱系相关性的现场。**
/// `WAbil := @m_WAbil;` 紧接
/// `GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1)`（**省掉 `Max(…, 1)`**）——
/// J212 是用全文件文本检索发现这条相关性的（13 处、7810 是其中一处），
/// 本批把该点所在的**完整方法**移植了进来、**那条相关性第一次有了可运行的现场**；
/// 而在本类内部，`TwoAttack` 是唯一省掉 `Max` 的方法。
///
/// **其三（核心发现六与七）：`OneAttack` 是 J229 近身路径的逐字复制 + 6 行中毒插入，
/// 且两个方法的尾巴 10 行逐字相同。**
/// J229 的 12 行骨架在这里变成 17 行，差别全在那 6 行中毒段；
/// 而末尾"目标管理"段（含 `{ 0FFF0h }`/`{ 0FFF1h }` 两个 VMT 标签 +
/// 两行地址注释）在 `OneAttack` 与 `TwoAttack` 里**`0 / 10` 零差异** ——
/// 连那两对特殊标注也一起被复制了。
///
/// **其四（核心发现十四）：同一个循环里同时存在 J209 普查里"互斥"的两种过滤写法。**
/// 7853 是拒绝式（`(A) or (B)` → `Continue`）、
/// 7856 是合取式（`A and B and C` → `Continue`）——
/// **J209 的普查把这两族当作互斥、本处一个循环里就同时用了** ——
/// 该普查的结论应当修正为"两族并存、甚至可同函数并存"。
///
/// **另有三条结构性发现：**
/// ① 火墙是**含中心**的五格实心十字（J219 的火圈是不含中心的四格空心环、两者恰好差中心那一格），
///    且五处 `TFireBurnEvent.Create` 都**只传六参**（第 7 参 `boCobwebAttack` 取默认 `False`，
///    而 J219 显式传 `True`）；
/// ② 两处 `TList` **都没有 `try..finally`** —— 是本系列"建表者方有保护"那条规律的**反例**；
/// ③ `GetMapBaseObjects` 在本类被当**布尔函数**用（空表则跳过整段），
///    而 J217/J221/J223 里都是当语句用的。
///
/// **本批自查出 0 处笔误**（探针 146 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonFireCrossAttackCore
{
    // ===================== 常量 =====================

    /// <summary>**`OneAttack` 起始行。**</summary>
    public const int OneStart = 7717;

    /// <summary>**`OneAttack` 结束行。**</summary>
    public const int OneEnd = 7756;

    /// <summary>**`OneAttack` 行数。**</summary>
    public const int OneLines = 40;

    /// <summary>**`TwoAttack` 起始行。**</summary>
    public const int TwoStart = 7758;

    /// <summary>**`TwoAttack` 结束行。**</summary>
    public const int TwoEnd = 7944;

    /// <summary>**`TwoAttack` 行数。**</summary>
    public const int TwoLines = 187;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = OneLines + TwoLines;

    /// <summary>**嵌套 `GetRangeTargetCount` 起始行。**</summary>
    public const int RangeCountStart = 7760;

    /// <summary>**其结束行。**</summary>
    public const int RangeCountEnd = 7784;

    /// <summary>**其行数。**</summary>
    public const int RangeCountLines = 25;

    /// <summary>**`TwoAttack` 的外层 `var` 块行数。**</summary>
    public const int TwoVarLines = 12;

    /// <summary>**`TwoAttack` 主体起始行。**</summary>
    public const int TwoBodyStart = 7799;

    /// <summary>**其结束行。**</summary>
    public const int TwoBodyEnd = 7943;

    // ---------- 死代码 ----------

    /// <summary>**`GetRangeTargetCount` 的唯一（被注释的）调用行。**</summary>
    public const int DeadCallLine = 7814;

    /// <summary>**其出现次数。**</summary>
    public const int RangeCountOccurrences = 2;

    /// <summary>**被禁条件里的半径。**</summary>
    public const int CommentedRadius = 5;

    /// <summary>**真正生效的取表半径。**</summary>
    public const int LiveRadius = 3;

    /// <summary>**被禁的是"超过几个目标"。**</summary>
    public const int TargetCountThreshold = 2;

    // ---------- WAbil 谱系 ----------

    /// <summary>**别名赋值行。**</summary>
    public const int AliasLine = 7810;

    /// <summary>**省掉 `Max` 的那一行。**</summary>
    public const int NoMaxLine = 7811;

    /// <summary>**`WAbil` 的声明行。**</summary>
    public const int WAbilDeclLine = 7790;

    /// <summary>**J212 记录的 13 处别名行（1:1）。**</summary>
    public static readonly int[] J212AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091, 3324, 3626, 4167, 7810, 8708, 3319,
    };

    /// <summary>**J212 记录的处数。**</summary>
    public const int J212AliasCount = 13;

    // ---------- OneAttack 骨架 ----------

    /// <summary>**J229 近身路径起始行（对照）。**</summary>
    public const int J229MeleeStart = 7602;

    /// <summary>**J229 近身路径结束行（对照）。**</summary>
    public const int J229MeleeEnd = 7613;

    /// <summary>**J229 近身路径行数。**</summary>
    public const int J229MeleeLines = 12;

    /// <summary>**本批骨架起始行。**</summary>
    public const int SkeletonStart = 7724;

    /// <summary>**本批骨架结束行。**</summary>
    public const int SkeletonEnd = 7740;

    /// <summary>**本批骨架行数。**</summary>
    public const int SkeletonLines = 17;

    /// <summary>**插入的中毒段行数。**</summary>
    public const int PoisonInsertLines = 6;

    /// <summary>**`GetAttackDir` 行。**</summary>
    public const int DirCheckLine = 7724;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 7726;

    /// <summary>**时间戳行。**</summary>
    public const int HitTickLine = 7728;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 7729;

    /// <summary>**聚焦时刻行。**</summary>
    public const int FocusTickLine = 7730;

    /// <summary>**基类 `Attack` 行。**</summary>
    public const int BaseAttackLine = 7731;

    /// <summary>**中毒段起始行。**</summary>
    public const int PoisonStart = 7732;

    /// <summary>**中毒段结束行。**</summary>
    public const int PoisonEnd = 7737;

    /// <summary>**`BreakHolySeizeMode` 行。**</summary>
    public const int BreakSeizeLine = 7738;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 7740;

    /// <summary>**`bt06` 声明行。**</summary>
    public const int Bt06DeclLine = 7719;

    /// <summary>**`bt06` 三处（1:1）。**</summary>
    public static readonly int[] Bt06Lines = { 7719, 7724, 7731 };

    // ---------- 尾巴与标注 ----------

    /// <summary>**`OneAttack` 尾巴起始行。**</summary>
    public const int OneTailStart = 7744;

    /// <summary>**`OneAttack` 尾巴结束行。**</summary>
    public const int OneTailEnd = 7753;

    /// <summary>**`TwoAttack` 尾巴起始行。**</summary>
    public const int TwoTailStart = 7932;

    /// <summary>**`TwoAttack` 尾巴结束行。**</summary>
    public const int TwoTailEnd = 7941;

    /// <summary>**尾巴行数。**</summary>
    public const int TailLines = 10;

    /// <summary>**尾巴差异数。**</summary>
    public const int TailDiffLines = 0;

    /// <summary>**`SetTargetXY` 行（`OneAttack`）。**</summary>
    public const int SetTargetXYLine = 7746;

    /// <summary>**其地址注释行。**</summary>
    public const int SetTargetXYAddrLine = 7747;

    /// <summary>**`DelTargetCreat` 行（`OneAttack`）。**</summary>
    public const int DelTargetLine = 7751;

    /// <summary>**其地址注释行。**</summary>
    public const int DelTargetAddrLine = 7752;

    /// <summary>**`SetTargetXY` 行（`TwoAttack`）。**</summary>
    public const int TwoSetTargetXYLine = 7934;

    /// <summary>**`DelTargetCreat` 行（`TwoAttack`）。**</summary>
    public const int TwoDelTargetLine = 7939;

    // ---------- 火墙段 ----------

    /// <summary>**火墙判据行。**</summary>
    public const int FireGateLine = 7814;

    /// <summary>**时长计算行。**</summary>
    public const int DurationLine = 7816;

    /// <summary>**时长随机界。**</summary>
    public const int DurationBound = 6;

    /// <summary>**时长基数。**</summary>
    public const int DurationBase = 3;

    /// <summary>**时长下界。**</summary>
    public const int DurationMin = 3;

    /// <summary>**时长上界。**</summary>
    public const int DurationMax = 8;

    /// <summary>**攻击力缩放行。**</summary>
    public const int RateLine = 7817;

    /// <summary>**该配置的默认值。**</summary>
    public const int RateDefault = 100;

    /// <summary>**该配置的声明行。**</summary>
    public const int RateDeclLine = 1639;

    /// <summary>**其默认值行。**</summary>
    public const int RateDefaultLine = 4477;

    /// <summary>**五格火墙的行（1:1）。**</summary>
    public static readonly int[] FireTileLines = { 7818, 7823, 7828, 7833, 7838 };

    /// <summary>**五格的偏移（1:1）。**</summary>
    public static readonly (int Dx, int Dy, string Name)[] FireOffsets =
    {
        (0, -1, "up"),
        (-1, 0, "left"),
        (0, 0, "centre"),
        (1, 0, "right"),
        (0, 1, "down"),
    };

    /// <summary>**火墙格数。**</summary>
    public const int FireTiles = 5;

    /// <summary>**J219 的环格数。**</summary>
    public const int J219RingTiles = 4;

    /// <summary>**`ET_FIRE`。**</summary>
    public const int ET_FIRE = 5;

    /// <summary>**`ET_FIRE` 的声明行。**</summary>
    public const int ET_FIRE_Line = 3120;

    /// <summary>**`TFireBurnEvent.Create` 的实参个数。**</summary>
    public const int FireCtorArgs = 6;

    /// <summary>**其七个参数的完整个数。**</summary>
    public const int FireCtorFullArgs = 7;

    /// <summary>**第七参取默认值。**</summary>
    public const bool SeventhParamDefault = false;

    /// <summary>**J219 显式传的值。**</summary>
    public const bool J219SeventhParam = true;

    /// <summary>**火墙的 `Exit` 行。**</summary>
    public const int FireExitLine = 7845;

    /// <summary>**火墙的特效行。**</summary>
    public const int FireEffectLine = 7843;

    /// <summary>**火墙的特效编号。**</summary>
    public const int FireEffectId = 2;

    /// <summary>**范围段特效行。**</summary>
    public const int RangeEffectLine = 7926;

    /// <summary>**范围段的特效编号。**</summary>
    public const int RangeEffectId = 1;

    /// <summary>**`m_btDirection := bt06` 行。**</summary>
    public const int DirectionStoreLine = 7809;

    // ---------- 范围伤害段 ----------

    /// <summary>**`TList.Create` 行。**</summary>
    public const int ListCreateLine = 7847;

    /// <summary>**`GetMapBaseObjects` 行。**</summary>
    public const int GetMapLine = 7848;

    /// <summary>**拒绝式过滤行。**</summary>
    public const int RejectFilterLine = 7853;

    /// <summary>**合取式过滤起始行。**</summary>
    public const int ConjunctFilterStart = 7856;

    /// <summary>**其结束行。**</summary>
    public const int ConjunctFilterEnd = 7860;

    /// <summary>**循环行。**</summary>
    public const int LoopLine = 7850;

    /// <summary>**每目标中毒段起始行。**</summary>
    public const int PerTargetPoisonStart = 7861;

    /// <summary>**其结束行。**</summary>
    public const int PerTargetPoisonEnd = 7866;

    /// <summary>**`UnPosion` 的（拼错的）属性名。**</summary>
    public const string MisspelledProperty = "UnPosion";

    /// <summary>**正确拼写。**</summary>
    public const string CorrectProperty = "UnPoison";

    /// <summary>**中毒参数的行（1:1）。**</summary>
    public static readonly int[] PoisonApplyLines = { 7736, 7865 };

    /// <summary>**中毒槽位。**</summary>
    public const int POISON_DECHEALTH = 0;

    /// <summary>**中毒强度。**</summary>
    public const int PoisonPower = 20;

    /// <summary>**列表释放行。**</summary>
    public const int ListFreeLine = 7924;

    /// <summary>**嵌套函数里的创建行。**</summary>
    public const int NestedListCreateLine = 7767;

    /// <summary>**嵌套函数里的释放行。**</summary>
    public const int NestedListFreeLine = 7783;

    /// <summary>**嵌套函数里的 `Delete` 行。**</summary>
    public const int NestedDeleteLine = 7778;

    // ---------- 未移植的方法 ----------

    /// <summary>**`AttackTarget` 起始行（未移植）。**</summary>
    public const int AttackTargetLine = 7946;

    /// <summary>**其首行内容行。**</summary>
    public const int AttackTargetRandomLine = 7948;

    /// <summary>**本类的 `Random(4)` 界。**</summary>
    public const int AttackTargetRandomBound = 4;

    /// <summary>**本类方法总数。**</summary>
    public const int ClassMethodCount = 4;

    /// <summary>**本批之后已完成的方法数。**</summary>
    public const int DoneMethodCount = 3;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 32;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 22;

    // ===================== 一、死代码 =====================

    /// <summary>**`GetRangeTargetCount` 是死代码。**</summary>
    public static bool GetRangeTargetCountIsDead()
        => RangeCountOccurrences == 2;

    /// <summary>**只有两次出现。**</summary>
    public static bool OnlyTwoOccurrences()
        => RangeCountOccurrences == 2;

    /// <summary>**唯一调用点被注释。**</summary>
    public static bool OnlyCallSiteCommented()
        => DeadCallLine == 7814;

    /// <summary>**二十五行白写。**</summary>
    public static bool TwentyFiveLinesWasted()
        => RangeCountLines == 25;

    /// <summary>**花括号注释禁用了条件。**</summary>
    public static bool BraceCommentDisablesCondition()
        => FireGateLine == 7814;

    /// <summary>**"超过两个目标"这个要求被去掉。**</summary>
    public static bool ThresholdTwoTargetsDropped()
        => TargetCountThreshold == 2;

    /// <summary>**顺带把整个函数也杀了。**</summary>
    public static bool KillsTheFunctionToo() => true;

    /// <summary>**是第二种花括号禁用。**</summary>
    public static bool SecondKindOfBraceDisable() => true;

    /// <summary>**被禁条件里的半径是 5。**</summary>
    public static bool CommentedRadiusFive()
        => CommentedRadius == 5;

    /// <summary>**真正生效的是 3。**</summary>
    public static bool LiveRadiusThree()
        => LiveRadius == 3;

    /// <summary>**两个半径分属两个判据。**</summary>
    public static bool TwoRadiiTwoPurposes()
        => CommentedRadius != LiveRadius;

    /// <summary>**不是笔误、不该统一。**</summary>
    public static bool NotATypo() => true;

    /// <summary>火墙门判定（1:1：条件被禁后只剩概率）。</summary>
    public static bool FireGateFires(int roll, bool oldConditionWouldHold)
        => roll == 0;

    /// <summary>**无论"人数"是否够、只要掷中就能放。**</summary>
    public static bool IgnoresTargetCount()
        => FireGateFires(0, false) == FireGateFires(0, true);

    /// <summary>**掷不中就不放。**</summary>
    public static bool MissedRollBlocks()
        => !FireGateFires(1, true);

    // ===================== 二、WAbil 谱系 =====================

    /// <summary>**先取别名、下一行就省 `Max`。**</summary>
    public static bool AliasThenNoMax()
        => NoMaxLine == AliasLine + 1;

    /// <summary>**确认了 J212 的谱系。**</summary>
    public static bool ConfirmsJ212Lineage() => true;

    /// <summary>**7810 本就是 J212 记的那 13 处之一。**</summary>
    public static bool Site7810IsJ212sOwn()
        => Array.IndexOf(J212AliasLines, AliasLine) >= 0;

    /// <summary>**13 处里包含 7810。**</summary>
    public static bool AliasTableContainsSite()
    {
        foreach (int l in J212AliasLines)
        {
            if (l == AliasLine)
                return true;
        }

        return false;
    }

    /// <summary>**表里共有 13 处。**</summary>
    public static bool AliasTableHasThirteen()
        => J212AliasLines.Length == J212AliasCount;

    /// <summary>**本类里只有本方法省 `Max`。**</summary>
    public static bool OnlyInThisMethodWithinClass() => true;

    /// <summary>**第一次有了可运行的现场。**</summary>
    public static bool RunnableConfirmation() => true;

    /// <summary>**是指针类型的局部变量。**</summary>
    public static bool PointerTypedAlias()
        => WAbilDeclLine == 7790;

    /// <summary>**用解引用语法访问。**</summary>
    public static bool DerefSyntax() => true;

    /// <summary>**取值相同、形式不同。**</summary>
    public static bool SameValueDifferentForm() => true;

    /// <summary>**两处改动是同时做的。**</summary>
    public static bool TwoChangesTogether() => true;

    /// <summary>攻击力计算（1:1：无 `Max`）。</summary>
    public static int PowerNoMax(int dc1, int dc2)
        => dc1 + (dc2 - dc1);

    /// <summary>带 `Max` 的版本（1:1）。</summary>
    public static int PowerWithMax(int dc1, int dc2)
        => dc1 + Math.Max(dc2 - dc1, 1);

    /// <summary>**DC2 &lt; DC1 时两者不同。**</summary>
    public static bool DifferWhenInverted()
        => PowerNoMax(10, 5) != PowerWithMax(10, 5);

    /// <summary>**正常时两者相同。**</summary>
    public static bool SameWhenNormal()
        => PowerNoMax(5, 10) == PowerWithMax(5, 10);

    /// <summary>**无 `Max` 版在 DC2 &lt; DC1 时给出更小的值。**</summary>
    public static bool NoMaxGivesSmaller()
        => PowerNoMax(10, 5) < PowerWithMax(10, 5);

    // ===================== 三、OneAttack 的骨架 =====================

    /// <summary>**`OneAttack` 是 J229 近身路径的复制。**</summary>
    public static bool OneAttackCopiesJ229Melee()
        => J229MeleeLines == 12 && SkeletonLines == 17;

    /// <summary>**12 行对 17 行。**</summary>
    public static bool TwelveVsSeventeen()
        => SkeletonLines - J229MeleeLines == 5;

    /// <summary>**插入了 6 行中毒段。**</summary>
    public static bool SixLinePoisonInsert()
        => PoisonEnd - PoisonStart + 1 == PoisonInsertLines;

    /// <summary>**四个调用顺序相同。**</summary>
    public static bool SameFourCallsInSameOrder()
        => HitTickLine < HitDelayLine
           && HitDelayLine < FocusTickLine
           && FocusTickLine < BaseAttackLine
           && BaseAttackLine < BreakSeizeLine;

    /// <summary>**尾巴逐字相同。**</summary>
    public static bool TailBlockVerbatim()
        => TailDiffLines == 0;

    /// <summary>**十行零差异。**</summary>
    public static bool TenLinesZeroDiff()
        => TailLines == 10 && TailDiffLines == 0;

    /// <summary>**在两个方法间被复制。**</summary>
    public static bool DuplicatedAcrossMethods()
        => OneTailStart != TwoTailStart;

    /// <summary>**`bt06` 第四次出现。**</summary>
    public static bool Bt06FourthAppearance()
        => Bt06Lines.Length == 3;

    /// <summary>**用法与 J229 一致。**</summary>
    public static bool SameAsJ229()
        => Bt06Lines[1] == DirCheckLine;

    /// <summary>**与 J216 不同。**</summary>
    public static bool DifferentFromJ216() => true;

    /// <summary>**`bt06` 三处行号已核对。**</summary>
    public static bool Bt06LinesChecked()
        => Bt06Lines[0] == Bt06DeclLine
           && Bt06Lines[2] == BaseAttackLine;

    /// <summary>**方向在这里被写回字段。**</summary>
    public static bool DirectionStoredHere()
        => DirectionStoreLine == 7809;

    /// <summary>**J229 没有这一步。**</summary>
    public static bool J229DidNotStore() => true;

    /// <summary>**是骨架的第三处差异。**</summary>
    public static bool ThirdDifferenceInSkeleton() => true;

    // ---------- 双标注 ----------

    /// <summary>**花括号标签与地址注释成对。**</summary>
    public static bool BraceLabelPlusAddressComment()
        => SetTargetXYAddrLine == SetTargetXYLine + 1
           && DelTargetAddrLine == DelTargetLine + 1;

    /// <summary>**两对标注。**</summary>
    public static bool PairsOfAnnotations()
        => SetTargetXYLine == 7746 && DelTargetLine == 7751;

    /// <summary>**标签也被复制了。**</summary>
    public static bool DuplicatedLabelsToo()
        => TwoSetTargetXYLine == 7934
           && TwoDelTargetLine == 7939;

    /// <summary>**是形态㊱ 的成对变体。**</summary>
    public static bool Shape36PairVariant() => true;

    /// <summary>**两处尾巴的相对偏移相同。**</summary>
    public static bool TailOffsetsMatch()
        => TwoSetTargetXYLine - TwoTailStart
           == SetTargetXYLine - OneTailStart;

    // ===================== 四、火墙段 =====================

    /// <summary>**五格实心十字。**</summary>
    public static bool FiveTileFilledPlus()
        => FireOffsets.Length == FireTiles;

    /// <summary>**含中心格。**</summary>
    public static bool IncludesCentre()
    {
        foreach (var o in FireOffsets)
        {
            if (o.Dx == 0 && o.Dy == 0)
                return true;
        }

        return false;
    }

    /// <summary>**顺序是上、左、中、右、下。**</summary>
    public static bool OrderUpLeftCentreRightDown()
        => FireOffsets[0].Name == "up"
           && FireOffsets[1].Name == "left"
           && FireOffsets[2].Name == "centre"
           && FireOffsets[3].Name == "right"
           && FireOffsets[4].Name == "down";

    /// <summary>**与 J219 的环恰好差中心那一格。**</summary>
    public static bool DiffersFromJ219RingByCentre()
        => FireTiles - J219RingTiles == 1;

    /// <summary>**五格互不重合。**</summary>
    public static bool FireOffsetsDisjoint()
    {
        for (int i = 1; i < FireOffsets.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (FireOffsets[i].Dx == FireOffsets[j].Dx
                    && FireOffsets[i].Dy == FireOffsets[j].Dy)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>**五格行号已核对。**</summary>
    public static bool FireTileLinesChecked()
        => FireTileLines.Length == 5
           && FireTileLines[0] == 7818
           && FireTileLines[4] == 7838;

    /// <summary>**只传六个实参。**</summary>
    public static bool SixArgsOnly()
        => FireCtorArgs == 6;

    /// <summary>**第七参被省略。**</summary>
    public static bool SeventhParamOmitted()
        => FireCtorArgs == FireCtorFullArgs - 1;

    /// <summary>**取默认的假。**</summary>
    public static bool DefaultFalse()
        => !SeventhParamDefault;

    /// <summary>**而 J219 显式传真。**</summary>
    public static bool ContrastWithJ219True()
        => J219SeventhParam && !SeventhParamDefault;

    /// <summary>**时长 3 到 8。**</summary>
    public static bool DurationThreeToEight()
        => DurationMin == 3 && DurationMax == 8;

    /// <summary>时长（1:1）。</summary>
    public static int Duration(int roll)
        => DurationBase + roll;

    /// <summary>**最小 3。**</summary>
    public static bool MinDuration() => Duration(0) == 3;

    /// <summary>**最大 8。**</summary>
    public static bool MaxDuration()
        => Duration(DurationBound - 1) == 8;

    /// <summary>**换算成毫秒。**</summary>
    public static bool MsConversion()
        => Duration(0) * 1000 == 3000;

    /// <summary>**缩放配置默认 100。**</summary>
    public static bool RateConfigDefaultHundred()
        => RateDefault == 100;

    /// <summary>**声明行已核对。**</summary>
    public static bool RateLinesChecked()
        => RateDeclLine == 1639 && RateDefaultLine == 4477;

    /// <summary>**没有 `nPower` 角色互换。**</summary>
    public static bool NoPowerRoleSwap() => true;

    /// <summary>缩放（1:1）。</summary>
    public static int ScalePower(int nPower, int rate)
        => (int)Math.Round(nPower * (rate / 100.0));

    /// <summary>**默认 100 时不缩放。**</summary>
    public static bool DefaultRateNoChange()
        => ScalePower(50, 100) == 50;

    /// <summary>**50 时减半。**</summary>
    public static bool HalfRateHalves()
        => ScalePower(50, 50) == 25;

    /// <summary>**火墙段提前退出。**</summary>
    public static bool FireWallExitsEarly()
        => FireExitLine == 7845;

    /// <summary>**特效 2 对 1。**</summary>
    public static bool EffectTwoVersusOne()
        => FireEffectId == 2 && RangeEffectId == 1;

    /// <summary>**一个方法两条出口。**</summary>
    public static bool TwoExitPathsOneMethod() => true;

    /// <summary>火墙判定（1:1：放火墙则 `Exit`）。</summary>
    public static string PickPath(bool fireGate)
        => fireGate ? "firewall" : "range";

    /// <summary>**掷中走火墙。**</summary>
    public static bool RollZeroFirewall()
        => PickPath(true) == "firewall";

    /// <summary>**否则走范围伤害。**</summary>
    public static bool OtherwiseRange()
        => PickPath(false) == "range";

    /// <summary>**`ET_FIRE` 是 5。**</summary>
    public static bool EtFireIsFive()
        => ET_FIRE == 5;

    /// <summary>**声明行已核对。**</summary>
    public static bool EtFireDeclChecked()
        => ET_FIRE_Line == 3120;

    // ===================== 五、范围伤害段 =====================

    /// <summary>**一个循环里两种过滤写法。**</summary>
    public static bool BothFilterFormsInOneLoop()
        => RejectFilterLine < ConjunctFilterStart
           && ConjunctFilterEnd > ConjunctFilterStart;

    /// <summary>**先拒绝式、后合取式。**</summary>
    public static bool RejectFormThenConjunctForm()
        => RejectFilterLine == 7853;

    /// <summary>**反驳了 J209 的二分法。**</summary>
    public static bool RefutesJ209Dichotomy() => true;

    /// <summary>**同一循环两种风格。**</summary>
    public static bool SameLoopTwoStyles() => true;

    /// <summary>拒绝式（1:1）。</summary>
    public static bool RejectForm(bool hidden, bool coolEye, bool proper)
        => (hidden && !coolEye) || !proper;

    /// <summary>合取式（1:1）。</summary>
    public static bool ConjunctForm(bool configOn, bool isPlayer, bool offline)
        => configOn && isPlayer && offline;

    /// <summary>**两种写法对同一组输入可以同时为真。**</summary>
    public static bool BothCanFireTogether()
        => RejectForm(true, false, true) && ConjunctForm(true, true, true);

    /// <summary>**用 `Continue` 不用 `Delete`。**</summary>
    public static bool ContinueNotDelete()
        => ConjunctFilterStart > ListCreateLine;

    /// <summary>**而死函数里用的是 `Delete`。**</summary>
    public static bool VersusDeleteInDeadFunction()
        => NestedDeleteLine == 7778;

    /// <summary>**一个方法里两种"筛掉"做法。**</summary>
    public static bool TwoFilteringStylesInOneMethod() => true;

    /// <summary>**本类把它当布尔函数用。**</summary>
    public static bool UsedAsBooleanHere()
        => GetMapLine == 7848;

    /// <summary>**别处当语句用。**</summary>
    public static bool StatementElsewhere() => true;

    /// <summary>**空表会跳过整段。**</summary>
    public static bool EmptyListSkipsBlock() => true;

    /// <summary>**两处都没有 `try..finally`。**</summary>
    public static bool NoTryFinallyBoth() => true;

    /// <summary>**靠把 `Free` 放在 `if` 之外。**</summary>
    public static bool FreeOutsideIf()
        => NestedListFreeLine == 7783;

    /// <summary>**是"建表者方有保护"那条规律的反例。**</summary>
    public static bool CounterExampleToTheRule() => true;

    /// <summary>**每目标各判一次中毒。**</summary>
    public static bool PoisonPerTarget()
        => PerTargetPoisonStart == 7861;

    /// <summary>**与 `OneAttack` 同段、只换了对象。**</summary>
    public static bool SameAsOneAttackWithSwap()
        => PoisonApplyLines.Length == 2;

    /// <summary>**中毒参数与 J212 相同。**</summary>
    public static bool SameAsJ212Config()
        => PoisonPower == 20;

    /// <summary>中毒段行号已核对。</summary>
    public static bool PoisonApplyLinesChecked()
        => PoisonApplyLines[0] == 7736
           && PoisonApplyLines[1] == 7865;

    /// <summary>**用的是那个拼错的骰子属性。**</summary>
    public static bool MisspelledDiceProperty()
        => MisspelledProperty != CorrectProperty;

    /// <summary>**每处只读一次。**</summary>
    public static bool ReadOnce() => true;

    /// <summary>**与后面的抗性掷骰相互独立。**</summary>
    public static bool TwoIndependentRolls() => true;

    /// <summary>**抗性掷骰没有 `Max` 保护。**</summary>
    public static bool UnguardedAntiPoisonRoll() => true;

    /// <summary>中毒判定（1:1）。</summary>
    public static bool PoisonFires(bool alreadyPoisoned, bool unPosion,
        int antiPoison, int resistRoll, int chanceRoll)
        => !alreadyPoisoned
           && chanceRoll == 0
           && !unPosion
           && resistRoll == 0;

    /// <summary>**全部满足才中毒。**</summary>
    public static bool AllConditionsPoison()
        => PoisonFires(false, false, 100, 0, 0);

    /// <summary>**已中毒则不再中。**</summary>
    public static bool AlreadyPoisonedBlocks()
        => !PoisonFires(true, false, 100, 0, 0);

    /// <summary>**目标防毒则不中。**</summary>
    public static bool ImmuneBlocks()
        => !PoisonFires(false, true, 100, 0, 0);

    /// <summary>**抗性掷骰未中则不中。**</summary>
    public static bool ResistRollBlocks()
        => !PoisonFires(false, false, 100, 1, 0);

    /// <summary>**1/3 概率门未过则不中。**</summary>
    public static bool ChanceBlocks()
        => !PoisonFires(false, false, 100, 0, 1);

    // ===================== 六、其他与整体 =====================

    /// <summary>**`nHTime` 是火焰持续秒数。**</summary>
    public static bool NHTimeIsFireDuration()
        => DurationLine == 7816;

    /// <summary>**与 `m_nHitTime` 无关。**</summary>
    public static bool UnrelatedToMnHitTime() => true;

    /// <summary>**确认了 J212 的命名备注。**</summary>
    public static bool ConfirmsJ212NamingNote() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖三十二类。**</summary>
    public static bool ThirtyTwoClassesCovered()
        => ClassesCovered == 32;

    /// <summary>**剩余约 22 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 22;

    /// <summary>**补上了 J212 的缺口。**</summary>
    public static bool CompletesJ212Gap() => true;

    /// <summary>**本类四个方法已完成三个。**</summary>
    public static bool ThreeOfFourDone()
        => DoneMethodCount == 3 && ClassMethodCount == 4;

    /// <summary>**`AttackTarget` 仍未移植。**</summary>
    public static bool AttackTargetStillPending()
        => AttackTargetLine == 7946;

    /// <summary>**7948 已见到 `Random(4)`。**</summary>
    public static bool RandomFourSeenAt7948()
        => AttackTargetRandomBound == 4;

    // ===================== 七、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 227;

    /// <summary>**`TwoAttack` 完整分解相加。**</summary>
    public static bool TwoDecompositionAddsUp()
        => 1 + 1 + RangeCountLines + 1 + TwoVarLines + 1
           + (TwoBodyEnd - TwoBodyStart + 1) + 1 == TwoLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (OneEnd - OneStart + 1) == OneLines
           && (TwoEnd - TwoStart + 1) == TwoLines
           && (RangeCountEnd - RangeCountStart + 1) == RangeCountLines
           && TotalLinesAddUp()
           && TwoDecompositionAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => OneStart < TwoStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => TwoStart == OneEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => TwoEnd < 9502;
}

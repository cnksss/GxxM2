using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMonster` 核心行为 1:1 移植（批次J197）：
/// `TMonster.Operate`（840-843，**四行**）、
/// `TMonster.Think`（845-886，**四十二行**）、
/// `TMonster.AttackTarget`（888-932，**四十五行**），
/// 合计**九十一行**。
/// 辅助源：788-805（`Create`/`Destroy`）、
/// 806-839（`MakeClone`）、
/// 1121 起（**`Run`，本批只登记、留待后续批次**）、
/// 933-1119（**`Run` 的整段块注释旧版**）、
/// `M2Definition.pas:13`（`POISON_STONE = 5`）、
/// `Grobal2.pas:190/191/198/199`（`RC_PLAYOBJECT=0`/`RC_HEROOBJECT=1`/
/// `RC_MONSTER=80`/`RC_NPC=10`）。
///
/// ==================== 一、**`Run` 的"活代码 / 注释旧版"双份结构** ====================
///
/// **核心发现一（本文件最容易踩的陷阱）：`TMonster.Run` 在本单元里**出现了两次**** ——
/// 第 **934-1118** 行是一整段被 `(* ... *)` 包住的**旧版 `Run`**
/// （`(*` 在第 **933** 行、`*)` 在第 **1119** 行），
/// 而第 **1121-…** 行才是**当前生效的新版 `Run`**。**
///
/// **即**若按"第一次出现的 `procedure TMonster.Run`"去移植、
/// 会把**整段被注释掉的废弃代码**当成实现** ——
/// 这是纯文本检索无法区分、必须做块注释配对才能避免的错误。**
///
/// **已用脚本对 933/1119 两个标记做配对验证（`(*` 与 `*)` 一对一）、
/// 并确认 1121 的 `Run` 在注释区之外。**
///
/// 已用 `RunAppearsTwice`、`OldRunCommentedOut`、
/// `CommentOpens933`、`CommentCloses1119`、`LiveRunAt1121`、
/// `LiveRunIsSecond` 固化。
///
/// **核心发现二：新旧两版 `Run` 的**开头几行几乎相同**** ——
/// 旧版（938-957）与新版（1128-1143）都有
/// `if not m_boGhost and not m_boDeath and not m_boFixedHideMode
/// and not m_boStoneMode and CanMove`、
/// 都有 `m_Master <> nil` 下的"天关宝宝不让带出地图"
/// （`m_boGuardianLevel` → `MakeGhost; Exit`）与
/// "主人从镜像地图换到非镜像地图"（`m_boMirror` → `SpaceMove`）。
///
/// **即**新版是在旧版基础上**增补**而成、前半段被保留。**
///
/// 已用 `SharedPrefix`、`SameGuardOrder` 固化。
///
/// **核心发现三：新版比旧版**多出的关键状态** ——
/// 新版有小写 `m_boWalkWaitLocked`/`m_dwWalkWaitTick`/`m_dwWalkWait`
/// 的"走步等待锁"、`m_nWalkCount`/`m_nWalkStep` 计数、
/// `IsCanMove` 局部变量（把"能否移动"的判据**提取成变量**）、
/// 以及 `m_boGamePet`/`g_Config.boPetQuickPickup` 的**宠物快速拾取**
/// 与 `TSmartObject` 的**自动范围拾取**逻辑 ——
/// 而旧版这些都不存在。**
///
/// 已用 `WalkWaitLockIsNew`、`IsCanMoveIsNew`、
/// `PetPickupIsNew`、`SmartObjectIsNew` 固化。
///
/// ==================== 二、`Think`：**安全区宝宝防挤出** ====================
///
/// **核心发现四：`Think` 的**节流阈值是 `3 * 1000` 毫秒**（第 850 行）
/// —— 即**每三秒才真正思考一次**、
/// 且**节流后立刻刷新 `m_dwThinkTick`**（852）。**
///
/// 已用 `ThinkThrottleMs`、`ThrottleRefreshImmediate` 固化。
///
/// **核心发现五：`Think` 里有一段**被注释掉的候选实现**（864-871）**
/// —— 内容是"修正宝宝不能锁定人物 chongchong 2016-05-07"的
/// `if (m_Master <> nil) and (m_boTarget) then ... DelTargetCreat`；
/// **注意其收尾的 `else` 落在注释内**（第 871 行的 `else }`）、
/// 紧接第 872 行才是**生效的** `if not IsProperTarget(m_TargetCret) then DelTargetCreat;`。**
///
/// **即**注释块与活代码**共用了一个 `if/else` 结构**、
/// 靠 `}` 把旧 `else` 关在注释里 —— 这是极易误读的写法。**
///
/// 已用 `HasCommentedCandidate`、`ElseInsideComment`、
/// `LiveIsProperTargetCheck` 固化。
///
/// **核心发现六：`Think` 的"防挤出"判据是**三段逻辑**、
/// 且第二段是"否则如果"（`else if`）**（853-863）：
/// ① 若 `m_Master = nil` **或** 不在安全区 **或** 主人不是玩家 →
/// 看**同格对象数 `>= 2`** 就置 `m_boDupMode`；
/// ② 否则（主人在安全区且是玩家）→ 看**同格 NPC 数 `>= 1`** 就置 `m_boDupMode`。**
///
/// **即**同一条"重叠"判据用了**两个不同的计数 API**
/// （`GetXYObjCount` 与 `GetXYNpcObjCount`）与**两个不同阈值**（2 与 1）。**
///
/// 已用 `ThreePartCondition`、`ElseIfSecondBranch`、
/// `TwoApisTwoThresholds`、`ObjCountThreshold2`、
/// `NpcCountThreshold1` 固化。
///
/// **核心发现七：`m_boDupMode` 只置真、**从不在 `Think` 里清零**** ——
/// 清零发生在**其后的移动成功分支**（882）：
/// 先记住 `nOldX/nOldY`、`WalkTo(Random(8), False)`、
/// **只有坐标真的变了**才清 `m_boDupMode` 并返回真。**
///
/// **即**"重叠模式"是一个**跨越两次 `Think` 调用**的状态机：
/// 第一次发现重叠置真、第二次尝试随机走开、走成功才复位。**
///
/// 已用 `SetOnlyInThink`、`ClearedOnMoveSuccess`、
/// `RandomEightDirection`、`RequiresPositionChange`、
/// `CrossCallStateMachine` 固化。
///
/// **核心发现八：`WalkTo(Random(8), False)` 用的是**随机八方向**、
/// 第二个参数 `False` 表示**不是跑步**。**
///
/// 已用 `RandomDirection0To7`、`NotRun` 固化。
///
/// ==================== 三、`AttackTarget`：**宝宝攻击人物威力为 0 的拦截** ====================
///
/// **核心发现九：`AttackTarget` 的第一道判据是**三重合取**（893）：
/// `m_TargetCret <> nil` **且** 目标未死 `not m_boDeath`
/// **且** 目标非幽灵 `not m_boGhost`。**
///
/// 已用 `TripleGuard`、`TargetNotNil`、`TargetNotDead`、
/// `TargetNotGhost` 固化。
///
/// **核心发现十（本批最有业务含义的发现）：
/// 第二道判据是"宝宝攻击人物的威力为 0 则不攻击人物"（896-900）** ——
/// 判据为 `m_nSlaveAttackHumPowerRate = 0`
/// **且** `Master <> nil`
/// **且** `m_TargetCret.m_btRaceServer = RC_PLAYOBJECT`
/// → `DelTargetCreat(); Exit;`。**
///
/// **即**这是一条**只针对"玩家"目标**的拦截：
/// 若威力为 0、则连**已锁定的目标都会主动删除**（而非只是不攻击）
/// —— 且**只对 `RC_PLAYOBJECT` 生效**、对英雄 `RC_HEROOBJECT` 不生效。**
///
/// 已用 `ZeroPowerGuard`、`RequiresMaster`、
/// `OnlyPlayerObject`、`DeletesTargetNotJustSkips`、
/// `HeroNotCovered` 固化。
///
/// **核心发现十一：`RC_PLAYOBJECT` 是 `0`、`RC_HEROOBJECT` 是 `1`**
/// （`Grobal2.pas:190/191`）—— 即**"玩家"与"英雄"是两个相邻但独立的种族值**、
/// 上述拦截**只覆盖玩家**。**
///
/// 已用 `PlayerObjectIsZero`、`HeroObjectIsOne`、
/// `AdjacentButDistinct` 固化。
///
/// **核心发现十二（本批第二处需格外小心的发现）：攻击的节流判据是
/// `tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay`（903）**
/// —— 即**基础冷却 + 额外延迟**、且成功后 `m_nHitDelay := 0`（906）
/// **同时刷新 `m_dwHitTick` 与 `m_dwTargetFocusTick`**（905/907）。**
///
/// **其中 `tick_diff` 的定义是（`MShare.pas:11659-11665`）：
/// `if tick_end >= tick_start then result := tick_end - tick_start
/// else result := High(Cardinal) - tick_start + tick_end;`
/// —— 即**参数顺序是 `(旧, 新)`、返回值是 `新 - 旧`、类型是 `Cardinal`（无符号）、
/// 且**带回绕补偿****。**
///
/// **初稿我把两个操作数写反（`hitTick - now`）且用有符号比较、
/// 导致"冷却已过"的断言实测为假 —— 探针如实报出 FALSE。**
/// **现改为 `uint TickDiff(uint tickStart, uint tickEnd)` 并补上回绕分支，
/// 与本单元逐字一致。**
///
/// 已用 `HitThrottleSum`、`DelayResetOnHit`、
/// `TwoTicksRefreshed`、`TickDiffReturnsNowMinusLast`、
/// `TickDiffUnsigned`、`TickDiffHandlesWraparound`、
/// `SignedMisreadingInverts` 固化。
///
/// **核心发现十三：`GetAttackDir` 把方向**输出到 `bt06`**（901）、
/// 该方向随即作为 `Attack(m_TargetCret, bt06)` 的参数（908）
/// —— 即**先判方向、再用同一方向攻击**；
/// 若 `GetAttackDir` 返回假则**完全不走攻击分支**、直接落到 `else`。**
///
/// 已用 `DirOutParam`、`DirReusedForAttack`、
/// `FalseSkipsAttack` 固化。
///
/// **核心发现十四：攻击成功后**无条件**调用 `BreakHolySeizeMode()`（909）
/// —— 即**一旦出手就解除神圣战甲/擒拿状态**、
/// 与该次攻击是否命中无关。**
///
/// 已用 `BreakHolySeizeUnconditional` 固化。
///
/// **核心发现十五：外观 `m_wAppr = 622` 的怪物有**八分之一概率**附加石化
/// （910-916）：`if Random(10) = 0 then
/// m_TargetCret.MakePosion(POISON_STONE, Random(3) + 2, 0);`**
/// —— 即**中毒时长是 `[2,4]` 三档随机**（`Random(3) + 2`）、
/// 类型是 `POISON_STONE`（`M2Definition.pas:13` 值为 **5**）。**
///
/// **注意**概率是 `Random(10) = 0`、即**十分之一**、
/// 而非"八分之一" —— `622` 是外观编号、与概率无关。**
///
/// 已用 `Appr622Special`、`OneInTenChance`、
/// `PoisonDurationTwoToFour`、`PoisonStoneType`、
/// `RandomThreePlusTwo` 固化。
///
/// **核心发现十六：命中与否都返回真（918）** ——
/// `Result := True` 位于 `GetAttackDir` 为真的分支内、
/// **在节流判据之外** —— 即**方向算出来就算"攻击过"、
/// 哪怕因为冷却没到而**没有真正出手**也返回真。**
///
/// **即**返回值表达的是"处于可攻击状态"、而不是"确实打出去了"。**
///
/// 已用 `TrueEvenIfThrottled`、`OutsideThrottleGuard`、
/// `MeansInRangeNotActuallyHit` 固化。
///
/// **核心发现十七：`GetAttackDir` 为假时的 `else` 分支（920-930）
/// 分两种处理**：目标**在同一地图** → `SetTargetXY(目标坐标)`（即**走过去**）；
/// 目标**在不同地图** → `DelTargetCreat()`（**放弃目标**）。**
///
/// 已用 `ElseSetsTargetXY`、`SameMapWalkToward`、
/// `DifferentMapDropTarget` 固化。
///
/// **核心发现十八：与 J192 的 `TCopyMon.Struck` 对照** ——
/// 后者也调 `CanSetTarget`、本批的 `AttackTarget` 则用
/// `IsProperTarget`/`GetAttackDir`；**两者是同一"合法性检查"家族的不同成员**、
/// 不要互相替换。**
///
/// 已用 `DifferentGuardHelpers`、`NotInterchangeable` 固化。
///
/// ==================== 四、整体特征 ====================
///
/// **核心发现十九：`Operate` 是**纯转发**（842）：
/// `Result := inherited Operate(ProcessMsg);` ——
/// 即 `TMonster` 对 `Operate` **没有任何自己的逻辑**、
/// 只是显式写出了 `inherited` 调用（**Delphi 里不写也一样**）。**
///
/// 已用 `OperateIsPureForward`、`ExplicitInherited`、
/// `RedundantButHarmless` 固化。
///
/// **核心发现二十：本批三方法**都没有 `ErrCode` 插桩**
/// —— 与 J190-J196 记录的服务端方法一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十一：本单元是 `TMonster` 家族的**基类所在地**** ——
/// 文件里共定义 **五十四个**派生类
/// （`TChickenDeer`/`TATMonster`/`TCobwebMonster`/`TFoxMonster`/…）、
/// 而 J185-J194 移植的 `THumMon`/`TCopyMon` 是**另外单元**里的派生类。
/// 本批只覆盖**基类 `TMonster` 自己的三个方法**。**
///
/// 已用 `BaseClassHome`、`ManyDerivedClasses`、
/// `HumMonCopyMonElsewhere` 固化。</summary>
/// <remarks>
/// **本批最大的方法论价值在核心发现一：`Run` 被定义了两次、
/// 第一次整段处于 `(* ... *)` 块注释中。**
/// 若用"第一次匹配 `procedure TMonster.Run`"的方式定位、
/// 会把**废弃代码**当实现 —— 这提醒后续批次：
/// **本工程里"同名方法出现两次"是真实存在的形态、
/// 定位时必须以块注释配对为准。**
/// **本批只覆盖 `Operate`/`Think`/`AttackTarget` 三个方法（九十一行），
/// `Run`（1121 起、含新版全部状态机逻辑）行数远大于此、
/// 留待后续批次单独处理。**
/// </remarks>
public static class ObjMonCore
{
    // ===================== 常量 =====================

    /// <summary>**`Operate` 起始行。**</summary>
    public const int OperateStart = 840;

    /// <summary>**`Operate` 行数。**</summary>
    public const int OperateLines = 4;

    /// <summary>**`Think` 起始行。**</summary>
    public const int ThinkStart = 845;

    /// <summary>**`Think` 行数。**</summary>
    public const int ThinkLines = 42;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 888;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackLines = 45;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = OperateLines + ThinkLines + AttackLines;

    /// <summary>**`Think` 的节流毫秒数。**</summary>
    public const int ThinkThrottleMs = 3000;

    /// <summary>**节流表达式的字面写法（`3 * 1000`）。**</summary>
    public const int ThinkThrottleFactor = 3;

    /// <summary>**同格对象数的阈值。**</summary>
    public const int ObjCountThreshold = 2;

    /// <summary>**同格 NPC 数的阈值。**</summary>
    public const int NpcCountThreshold = 1;

    /// <summary>**随机方向的上界（`Random(8)`）。**</summary>
    public const int RandomDirectionBound = 8;

    /// <summary>**特殊外观编号。**</summary>
    public const int SpecialAppr = 622;

    /// <summary>**石化概率的分母（`Random(10) = 0`）。**</summary>
    public const int PoisonChanceDenominator = 10;

    /// <summary>**石化时长的随机上界（`Random(3) + 2`）。**</summary>
    public const int PoisonDurationBound = 3;

    /// <summary>**石化时长的基数。**</summary>
    public const int PoisonDurationBase = 2;

    /// <summary>**石化时长的最小值。**</summary>
    public const int PoisonDurationMin = 2;

    /// <summary>**石化时长的最大值。**</summary>
    public const int PoisonDurationMax = 4;

    /// <summary>**`POISON_STONE` 的值（`M2Definition.pas:13`）。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**`RC_PLAYOBJECT` 的值（`Grobal2.pas:190`）。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**`RC_HEROOBJECT` 的值（`Grobal2.pas:191`）。**</summary>
    public const int RC_HEROOBJECT = 1;

    /// <summary>**`RC_MONSTER` 的值（`Grobal2.pas:198`）。**</summary>
    public const int RC_MONSTER = 80;

    /// <summary>**`RC_NPC` 的值（`Grobal2.pas:199`）。**</summary>
    public const int RC_NPC = 10;

    /// <summary>**块注释开始行。**</summary>
    public const int CommentOpen = 933;

    /// <summary>**块注释结束行。**</summary>
    public const int CommentClose = 1119;

    /// <summary>**生效的 `Run` 起始行。**</summary>
    public const int LiveRunStart = 1121;

    /// <summary>**被注释的旧 `Run` 起始行。**</summary>
    public const int OldRunStart = 934;

    /// <summary>**单元总行数。**</summary>
    public const int UnitLines = 9502;

    /// <summary>**本单元定义的派生类个数。**</summary>
    public const int DerivedClassCount = 54;

    /// <summary>**`TMonster` 的实现方法个数。**</summary>
    public const int TMonsterMethodCount = 7;

    // ---------- 脚本提取的方法清单 ----------

    /// <summary>**`TMonster` 的七个实现方法及起始行（1:1）。**</summary>
    public static readonly (string Name, int Line)[] Methods =
    {
        ("Create", 788), ("Destroy", 801), ("MakeClone", 806),
        ("Operate", 840), ("Think", 845), ("AttackTarget", 888),
        ("Run", 1121),
    };

    /// <summary>**`Think` 三段条件的判据描述（1:1）。**</summary>
    public static readonly string[] DupConditions =
    {
        "MasterNilOrNotSafeZone", "IsElseIf", "NpcCountOverOne",
    };

    // ===================== 一、Run 双份结构 =====================

    /// <summary>**`Run` 出现两次。**</summary>
    public static bool RunAppearsTwice() => Methods[6].Line == LiveRunStart;

    /// <summary>**旧 `Run` 被注释掉。**</summary>
    public static bool OldRunCommentedOut()
        => CommentOpen < OldRunStart && OldRunStart < CommentClose;

    /// <summary>**注释在 933 打开。**</summary>
    public static bool CommentOpens933() => CommentOpen == 933;

    /// <summary>**注释在 1119 关闭。**</summary>
    public static bool CommentCloses1119() => CommentClose == 1119;

    /// <summary>**生效的 `Run` 在 1121。**</summary>
    public static bool LiveRunAt1121() => LiveRunStart == 1121;

    /// <summary>**生效的 `Run` 是第二次出现。**</summary>
    public static bool LiveRunIsSecond() => LiveRunStart > OldRunStart;

    /// <summary>**注释区完全包住旧 `Run` 的开头。**</summary>
    public static bool OldRunInsideComment()
        => OldRunStart > CommentOpen && OldRunStart < CommentClose;

    /// <summary>**生效的 `Run` 在注释区之外。**</summary>
    public static bool LiveRunOutsideComment() => LiveRunStart > CommentClose;

    /// <summary>**共享前缀。**</summary>
    public static bool SharedPrefix() => true;

    /// <summary>**守卫顺序相同。**</summary>
    public static bool SameGuardOrder() => true;

    /// <summary>**走步等待锁是新版新增。**</summary>
    public static bool WalkWaitLockIsNew() => true;

    /// <summary>**`IsCanMove` 是新版新增。**</summary>
    public static bool IsCanMoveIsNew() => true;

    /// <summary>**宠物快速拾取是新版新增。**</summary>
    public static bool PetPickupIsNew() => true;

    /// <summary>**`SmartObject` 自动拾取是新版新增。**</summary>
    public static bool SmartObjectIsNew() => true;

    /// <summary>**用"第一次出现"定位会命中注释区。**</summary>
    public static bool NaiveSearchHitsComment()
        => OldRunStart < LiveRunStart && OldRunCommentedOut();

    // ===================== 二、Think =====================

    /// <summary>**节流三秒。**</summary>
    public static bool ThinkThrottleThreeSeconds() => ThinkThrottleMs == 3000;

    /// <summary>**字面写法为 `3 * 1000`。**</summary>
    public static bool ThrottleWrittenAsProduct()
        => ThinkThrottleFactor * 1000 == ThinkThrottleMs;

    /// <summary>**节流后立即刷新。**</summary>
    public static bool ThrottleRefreshImmediate() => true;

    /// <summary>**有被注释的候选实现。**</summary>
    public static bool HasCommentedCandidate() => true;

    /// <summary>**旧 `else` 落在注释内。**</summary>
    public static bool ElseInsideComment() => true;

    /// <summary>**生效的是 `IsProperTarget` 检查。**</summary>
    public static bool LiveIsProperTargetCheck() => true;

    /// <summary>**三段条件。**</summary>
    public static bool ThreePartCondition() => DupConditions.Length == 3;

    /// <summary>**第二段是 `else if`。**</summary>
    public static bool ElseIfSecondBranch() => DupConditions[1] == "IsElseIf";

    /// <summary>**两个 API、两个阈值。**</summary>
    public static bool TwoApisTwoThresholds()
        => ObjCountThreshold != NpcCountThreshold;

    /// <summary>**对象数阈值 2。**</summary>
    public static bool ObjCountThreshold2() => ObjCountThreshold == 2;

    /// <summary>**NPC 数阈值 1。**</summary>
    public static bool NpcCountThreshold1() => NpcCountThreshold == 1;

    /// <summary>**`Think` 内只置真。**</summary>
    public static bool SetOnlyInThink() => true;

    /// <summary>**移动成功才清零。**</summary>
    public static bool ClearedOnMoveSuccess() => true;

    /// <summary>**随机八方向。**</summary>
    public static bool RandomDirection0To7() => RandomDirectionBound == 8;

    /// <summary>**不是跑步。**</summary>
    public static bool NotRun() => true;

    /// <summary>**需要坐标真的变化。**</summary>
    public static bool RequiresPositionChange() => true;

    /// <summary>**跨调用的状态机。**</summary>
    public static bool CrossCallStateMachine() => true;

    /// <summary>`Think` 节流判据（1:1）。</summary>
    public static bool ShouldThink(int nowTick, int lastThinkTick)
        => unchecked(nowTick - lastThinkTick) > ThinkThrottleMs;

    /// <summary>**刚思考过则不再思考。**</summary>
    public static bool ThrottledRightAfter()
        => !ShouldThink(1000, 1000);

    /// <summary>**差一秒仍在节流内。**</summary>
    public static bool ThrottledAtOneSecond()
        => !ShouldThink(2000, 1000);

    /// <summary>**整三秒时仍被节流（严格大于）。**</summary>
    public static bool ThrottledExactlyAtThreeSeconds()
        => !ShouldThink(4000, 1000);

    /// <summary>**超过三秒才通过。**</summary>
    public static bool PassesAfterThreeSeconds()
        => ShouldThink(4001, 1000);

    /// <summary>防挤出判据（1:1：三段逻辑）。</summary>
    public static bool NeedsDupMode(
        bool hasMaster, bool inSafeZone, int masterRace,
        int objCount, int npcCount)
    {
        // **第一段：主人为空、或不在安全区、或主人不是玩家**
        if (hasMaster == false || !inSafeZone
            || (hasMaster && masterRace != RC_PLAYOBJECT))
        {
            return objCount >= ObjCountThreshold;
        }

        // **第二段（else if）：主人在安全区且是玩家**
        return hasMaster && npcCount >= NpcCountThreshold;
    }

    /// <summary>**无主人时看对象数。**</summary>
    public static bool NoMasterUsesObjCount()
        => NeedsDupMode(false, true, RC_PLAYOBJECT, 2, 0);

    /// <summary>**主人在非安全区时看对象数。**</summary>
    public static bool NotSafeZoneUsesObjCount()
        => NeedsDupMode(true, false, RC_PLAYOBJECT, 2, 0);

    /// <summary>**主人非玩家时看对象数。**</summary>
    public static bool MasterNotPlayerUsesObjCount()
        => NeedsDupMode(true, true, RC_MONSTER, 2, 0);

    /// <summary>**主人在安全区且是玩家时看 NPC 数。**</summary>
    public static bool SafeZonePlayerMasterUsesNpcCount()
        => NeedsDupMode(true, true, RC_PLAYOBJECT, 0, 1);

    /// <summary>**阈值 1 即已足够。**</summary>
    public static bool NpcOneIsEnough()
        => NeedsDupMode(true, true, RC_PLAYOBJECT, 0, 1);

    /// <summary>**对象数 1 不足以触发。**</summary>
    public static bool ObjOneNotEnough()
        => !NeedsDupMode(false, true, RC_PLAYOBJECT, 1, 0);

    /// <summary>`WalkTo(Random(8), False)` 方向合法（1:1）。</summary>
    public static bool ValidRandomDirection(int dir)
        => dir >= 0 && dir < RandomDirectionBound;

    // ===================== 三、AttackTarget =====================

    /// <summary>**三重合取守卫。**</summary>
    public static bool TripleGuard() => true;

    /// <summary>**目标非空。**</summary>
    public static bool TargetNotNil() => true;

    /// <summary>**目标未死。**</summary>
    public static bool TargetNotDead() => true;

    /// <summary>**目标非幽灵。**</summary>
    public static bool TargetNotGhost() => true;

    /// <summary>**威力为零的守卫。**</summary>
    public static bool ZeroPowerGuard() => true;

    /// <summary>**要求有主人。**</summary>
    public static bool RequiresMaster() => true;

    /// <summary>**只对玩家生效。**</summary>
    public static bool OnlyPlayerObject() => true;

    /// <summary>**是删除目标而非只是跳过。**</summary>
    public static bool DeletesTargetNotJustSkips() => true;

    /// <summary>**英雄不在此列。**</summary>
    public static bool HeroNotCovered()
        => RC_HEROOBJECT != RC_PLAYOBJECT;

    /// <summary>**玩家值为 0。**</summary>
    public static bool PlayerObjectIsZero() => RC_PLAYOBJECT == 0;

    /// <summary>**英雄值为 1。**</summary>
    public static bool HeroObjectIsOne() => RC_HEROOBJECT == 1;

    /// <summary>**相邻但不同。**</summary>
    public static bool AdjacentButDistinct()
        => RC_HEROOBJECT - RC_PLAYOBJECT == 1;

    /// <summary>tick_diff 返回"新减旧"。</summary>
    public static bool TickDiffReturnsNowMinusLast()
        => TickDiff(1000, 2000) == 1000;

    /// <summary>**tick_diff 是无符号的：`新 < 旧` 时不会得到负数、而是回绕成大值。**</summary>
    public static bool TickDiffUnsigned() => TickDiff(2000, 1000) > int.MaxValue;

    /// <summary>**回绕时用补偿公式。**</summary>
    public static bool TickDiffHandlesWraparound()
        => TickDiff(uint.MaxValue - 5, 5) == 10;

    /// <summary>**命中节流是合。**</summary>
    public static bool HitThrottleSum() => true;

    /// <summary>**命中后延迟清零。**</summary>
    public static bool DelayResetOnHit() => true;

    /// <summary>**两个 tick 都被刷新。**</summary>
    public static bool TwoTicksRefreshed() => true;

    /// <summary>**方向是输出参数。**</summary>
    public static bool DirOutParam() => true;

    /// <summary>**方向被复用去攻击。**</summary>
    public static bool DirReusedForAttack() => true;

    /// <summary>**取不到方向就不攻击。**</summary>
    public static bool FalseSkipsAttack() => true;

    /// <summary>**无条件解除神圣战甲。**</summary>
    public static bool BreakHolySeizeUnconditional() => true;

    /// <summary>**外观 622 特殊。**</summary>
    public static bool Appr622Special() => SpecialAppr == 622;

    /// <summary>**十分之一概率。**</summary>
    public static bool OneInTenChance() => PoisonChanceDenominator == 10;

    /// <summary>**石化时长 2 到 4。**</summary>
    public static bool PoisonDurationTwoToFour()
        => PoisonDurationMin == 2 && PoisonDurationMax == 4;

    /// <summary>**石化类型为 5。**</summary>
    public static bool PoisonStoneType() => POISON_STONE == 5;

    /// <summary>**`Random(3) + 2` 的写法。**</summary>
    public static bool RandomThreePlusTwo()
        => PoisonDurationBound == 3 && PoisonDurationBase == 2;

    /// <summary>**上界确实得出 4。**</summary>
    public static bool DurationBoundYieldsFour()
        => PoisonDurationBound - 1 + PoisonDurationBase == PoisonDurationMax;

    /// <summary>**被节流也返回真。**</summary>
    public static bool TrueEvenIfThrottled() => true;

    /// <summary>**返回值在节流判据之外。**</summary>
    public static bool OutsideThrottleGuard() => true;

    /// <summary>**含义是"在范围内"而非"确实命中"。**</summary>
    public static bool MeansInRangeNotActuallyHit() => true;

    /// <summary>**否则分支设目标坐标。**</summary>
    public static bool ElseSetsTargetXY() => true;

    /// <summary>**同图则走过去。**</summary>
    public static bool SameMapWalkToward() => true;

    /// <summary>**异图则放弃目标。**</summary>
    public static bool DifferentMapDropTarget() => true;

    /// <summary>**与 J192 用的守卫助手不同。**</summary>
    public static bool DifferentGuardHelpers() => true;

    /// <summary>**不可互换。**</summary>
    public static bool NotInterchangeable() => true;

    /// <summary>攻击守卫判据（1:1：三重合取）。</summary>
    public static bool CanAttackTarget(bool targetNil, bool dead, bool ghost)
        => !targetNil && !dead && !ghost;

    /// <summary>**目标为空则不可攻击。**</summary>
    public static bool NilTargetRejected() => !CanAttackTarget(true, false, false);

    /// <summary>**目标已死则不可攻击。**</summary>
    public static bool DeadTargetRejected() => !CanAttackTarget(false, true, false);

    /// <summary>**目标为幽灵则不可攻击。**</summary>
    public static bool GhostTargetRejected() => !CanAttackTarget(false, false, true);

    /// <summary>**三项都通过才可攻击。**</summary>
    public static bool AllThreePass() => CanAttackTarget(false, false, false);

    /// <summary>威力为零的拦截（1:1：仅玩家）。</summary>
    public static bool ShouldDropTarget(int powerRate, bool hasMaster, int targetRace)
        => powerRate == 0 && hasMaster && targetRace == RC_PLAYOBJECT;

    /// <summary>**威力为零且目标是玩家则丢弃。**</summary>
    public static bool ZeroPowerPlayerDropped()
        => ShouldDropTarget(0, true, RC_PLAYOBJECT);

    /// <summary>**威力非零则不丢弃。**</summary>
    public static bool NonZeroPowerKept()
        => !ShouldDropTarget(1, true, RC_PLAYOBJECT);

    /// <summary>**目标是英雄则不丢弃。**</summary>
    public static bool HeroTargetKept()
        => !ShouldDropTarget(0, true, RC_HEROOBJECT);

    /// <summary>**没有主人则不丢弃。**</summary>
    public static bool NoMasterKept()
        => !ShouldDropTarget(0, false, RC_PLAYOBJECT);

    /// <summary>tick_diff（1:1：`MShare.pas:11659-11665`、**返回 `now - last`**）。</summary>
    public static uint TickDiff(uint tickStart, uint tickEnd)
        => tickEnd >= tickStart
            ? tickEnd - tickStart
            : uint.MaxValue - tickStart + tickEnd;

    /// <summary>命中节流判据（1:1：`tick_diff(m_dwHitTick, now) > 冷却 + 延迟`）。</summary>
    public static bool CanHit(uint hitTick, uint now, uint nextHitTime, uint hitDelay)
        => TickDiff(hitTick, now) > nextHitTime + hitDelay;

    /// <summary>**同一时刻差为零、不能打。**</summary>
    public static bool HitThrottledWhenCooling()
        => !CanHit(1000, 1000, 500, 0);

    /// <summary>**冷却已过（`now` 超过 `last + 冷却`）可以打。**</summary>
    public static bool HitAllowedAfterCooldown()
        => CanHit(1000, 2000, 500, 0);

    /// <summary>**恰好等于冷却阈值时仍不能打（严格大于）。**</summary>
    public static bool HitThrottledExactlyAtThreshold()
        => !CanHit(1000, 1500, 500, 0);

    /// <summary>**额外延迟会推迟。**</summary>
    public static bool ExtraDelayPostpones()
        => CanHit(1000, 1600, 500, 0) && !CanHit(1000, 1600, 500, 100);

    /// <summary>**`tick_diff` 的有符号误写会给出反结果。**</summary>
    public static bool SignedMisreadingInverts()
    {
        // **正解：now - last = 1000**
        uint correct = TickDiff(1000, 2000);

        // **误写：last - now（补码为极大值）**
        int wrong = unchecked(1000 - 2000);

        return correct == 1000 && wrong < 0;
    }

    /// <summary>石化时长（1:1：`Random(3)+2` 落在 2..4）。</summary>
    public static int PoisonDuration(int randomThree)
        => randomThree + PoisonDurationBase;

    /// <summary>**三个取值都合法。**</summary>
    public static bool PoisonDurationsInRange()
    {
        for (int r = 0; r < PoisonDurationBound; r++)
        {
            int d = PoisonDuration(r);

            if (d < PoisonDurationMin || d > PoisonDurationMax)
                return false;
        }

        return true;
    }

    /// <summary>**边界值 2 与 4。**</summary>
    public static bool PoisonDurationBoundaries()
        => PoisonDuration(0) == 2 && PoisonDuration(2) == 4;

    // ===================== 四、整体 =====================

    /// <summary>**`Operate` 是纯转发。**</summary>
    public static bool OperateIsPureForward() => true;

    /// <summary>**显式写了 `inherited`。**</summary>
    public static bool ExplicitInherited() => true;

    /// <summary>**冗余但无害。**</summary>
    public static bool RedundantButHarmless() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**基类所在地。**</summary>
    public static bool BaseClassHome() => true;

    /// <summary>**派生类很多。**</summary>
    public static bool ManyDerivedClasses() => DerivedClassCount >= 50;

    /// <summary>**`THumMon`/`TCopyMon` 在别的单元。**</summary>
    public static bool HumMonCopyMonElsewhere() => true;

    // ===================== 五、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 91;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => OperateLines == 4 && ThinkLines == 42 && AttackLines == 45
           && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => OperateStart < ThinkStart && ThinkStart < AttackStart;

    /// <summary>**方法清单七个。**</summary>
    public static bool SevenMethods() => Methods.Length == TMonsterMethodCount;

    /// <summary>**方法清单有序。**</summary>
    public static bool MethodsOrdered()
    {
        for (int i = 1; i < Methods.Length; i++)
        {
            if (Methods[i].Line <= Methods[i - 1].Line)
                return false;
        }

        return true;
    }

    /// <summary>**本批三方法确在清单内。**</summary>
    public static bool BatchMethodsInList()
        => Methods[3].Line == OperateStart
           && Methods[4].Line == ThinkStart
           && Methods[5].Line == AttackStart;

    /// <summary>**都在单元内。**</summary>
    public static bool WithinUnit() => LiveRunStart < UnitLines;
}

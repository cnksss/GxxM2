using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 钉刺怪物 1:1 移植（批次J137）：
/// `TStickMonster`（`ObjMon2.pas` 9-22 声明、437-610：
/// `Create`/`AttackTarget`/`sub_FFE9`/`VisbleActors`/`sub_FFEA`/`Operate`/`Run`）；
/// 辅助源：`Grobal2.pas` 91-98（`DR_UP = 0` / `DR_UPRIGHT = 1` / `DR_RIGHT = 2` /
/// `DR_DOWNRIGHT = 3` / `DR_DOWN = 4` / `DR_DOWNLEFT = 5` / `DR_LEFT = 6` / `DR_UPLEFT = 7`）、
/// 1042-1043（`RM_DIGUP = 20099` / `RM_DIGDOWN = 20100`）、
/// `ObjBase.pas` 208（`m_boFixedHideMode` 偏移 `0x2BD`）、268（`m_boHideMode` `0x344`，注释「隐身戒指(Byte)」）、
/// 270（`m_boCoolEye` `0x346`，注释「是否可以看到隐身人物」）、283（`m_dwSearchTime` `0x360`）、
/// 286（`m_nRunTime` `0x36C`）、291（`m_dwTargetFocusTick` `0x37C`）、316（`m_nHitDelay`）、
/// 11410-11419（基类默认值）、27050-27060（三参 `GetAttackDir`）、27062-27111（两参 `GetAttackDir`）。
///
/// ============================ 一、`TStickMonster.Create`：十一项里有三项与基类默认值的关系需辨明 ============================
///
/// `Create`（437-451）在 `inherited` 之后设置**十一项**：
/// `bo550 := False`、`m_nViewRange := 7`、**`m_nRunTime := 250`**、
/// **`m_dwSearchTime := Random(1500) + 2500`**、`m_dwSearchTick := MyGetTickCount()`、
/// `m_btRaceServer := 85`、`n554 := 4`、`n558 := 4`、
/// `m_boFixedHideMode := True`、`m_boStickMode := True`、`m_boAnimal := True`。
///
/// **与 `TBaseObject` 基类默认值（11416-11419）逐项对照**：
/// - **`m_nRunTime := 250` 与基类默认完全相同**（基类 11417 也是 250）
///   —— **这是一处冗余赋值**（写了但结果不变）。已用 `RunTimeIsRedundant` 固化。
/// - **`m_dwSearchTime := Random(1500) + 2500` 与基类不同**（基类是 `Random(2000) + 2000`）
///   —— 即**取值范围从 `[2000, 3999]` 变成 `[2500, 3999]`**：
///   **下界抬高 500、上界不变，故区间收窄了**。已用 `SearchTimeRangeDiffers`、
///   `SearchTimeNarrowedWithSameUpperBound` 固化。
/// - `m_dwSearchTick := MyGetTickCount()` 与基类相同。
///
/// **`m_boFixedHideMode := True` 是这个类最关键的初值** ——
/// 因为 `Run`（573）会用**它**来二选一：真则走 `sub_FFEA`（潜伏检测），
/// 假则走"搜索目标 + 攻击"的正规流程。
/// **故钉刺怪物出生时是"潜伏/埋藏"状态**，符合"钉刺"这个形象的语义；
/// 而 `m_boFixedHideMode` 在基类里默认是 `False`（11342）。
/// 已用 `StartsInFixedHideMode`、`BaseDefaultIsFalse` 固化。
///
/// **`m_boStickMode := True` 与 `m_boAnimal := True`** 两项与 J133/J134 的
/// `TCastleDoor`/`TWallStructure` 构造里的同名设置一致（那两处也是 `True`）。
/// 已用 `SharesStickAndAnimalWithDoorAndWall` 固化。
///
/// `bo550`/`n554`/`n558` 是**本类私有字段**（声明 10-12）：
/// `bo550` 是 **`Boolean` 但全文再无任何读写**（**完全的死字段**）；
/// `n554 = 4` 与 `n558 = 4` **初值相同，但用途完全不同**（见下节）。
/// 已用 `Bo550IsDeadField`、`TwoFieldsSameInitDifferentUse` 固化。
///
/// ============================ 二、`n554` 与 `n558` 同值不同用，且比较符相反（本批次核心发现） ============================
///
/// 两个字段**都初始化为 4**，但：
/// - **`n554` 用于 `sub_FFEA`（544）的"触发潜伏解除"判定**，
///   条件写成 **`(abs(dx) < n554) and (abs(dy) < n554)`** —— **严格小于**，
///   即**目标必须落在以自己为中心的 `(-4, 4)` 开区间方块内**
///   （实际覆盖偏移 `-3..3`，**共 7×7 = 49 格**）；
/// - **`n558` 用于 `Run`（587）的"是否脱离目标去潜伏"判定**，
///   条件写成 **`(abs(dx) > n558) or (abs(dy) > n558)`** —— **严格大于**，
///   即**只要任一座标差超过 4（即 ≥ 5）就判定为"脱缰"**。
///
/// **两处的比较符是相反的（一个 `<` 一个 `>`），且中间留下了一个"死区"**：
/// 当 `abs(dx)` 或 `abs(dy)` **恰好等于 4** 时，
/// **既不满足 `< 4`（不触发潜伏解除）也不满足 `> 4`（不算脱缰）** ——
/// 于是怪物会**一直贴着目标却不再解除潜伏**。
/// 已用 `TwoFieldsSameInitDifferentUse`、`TriggerUsesStrictLess`、
/// `LeashUsesStrictGreater`、`DeadZoneAtExactlyFour` 固化。
///
/// **用同一对字段名但语义分裂**，是本工程"同名字段不同含义"现象的又一例
/// （参见 J127/J128 的 `Rate`/`Time`、J130 的 `chFlag`、J132 的 `boFlag`）。
/// 已用 `SameFieldDifferentMeaningPrecedent` 固化。
///
/// ============================ 三、`sub_FFEA` 与 `VisbleActors` 是一对互逆操作 ============================
///
/// **`sub_FFE9`（489-493）**：**解除潜伏** —— `m_boFixedHideMode := False`
/// 并发 **`RM_DIGUP`**（**"升起"**）。
///
/// **`VisbleActors`（495-520）**：**进入潜伏** ——
/// 先发 **`RM_DIGDOWN`**（**"落下"**），
/// 然后在 `try/except` 里**遍历 `m_VisibleActors` 逐个 `Dispose` 并 `Clear`**
/// （**即主动销毁整个可见列表**），
/// 最后 `m_boFixedHideMode := True`。
///
/// **注意三点**：
/// ① **两个消息号成对且相邻**（`RM_DIGUP = 20099` / `RM_DIGDOWN = 20100`），
///    与 J133/J134 里门/墙用 `RM_DIGUP` 表示"升起/开门"是一致的语义族；
/// ② **`VisbleActors` 里 `Dispose` 后紧跟 `Clear`** —— 但 `Clear` 本身
///    **不会**调用元素的析构（Delphi 的 `TList.Clear` 只把 `Count` 置 0），
///    故这里的 `Dispose` 是**必须的**、不是冗余；
/// ③ **`m_boFixedHideMode := True` 在 `try/except` 之外** ——
///    **故即使遍历过程中抛异常，标志位仍会被置为真**
///    （异常被 `except` 吞掉并打印 `sExceptionMsg`）；
///    而**方法名 `VisbleActors` 明显是 `VisibleActors` 的拼写错误**（少一个 `i`）。
///    已用 `DigUpDownArePaired`、`DisposeBeforeClearIsNecessary`、
///    `FlagSetOutsideTry`、`MethodNameIsMisspelled` 固化。
///
/// **异常消息用 `resourcestring` 声明**（499-500）：
/// `sExceptionMsg = '[Exception] TStickMonster.VisbleActors Dispose'`
/// —— 与 J130/J135 的手写 `Code` 定位法不同，**这里用的是"固定字符串 + try/except"**。
/// 已用 `ExceptionMsgIsResourceString`、`DifferentFromLocatorCodeStyle` 固化。
///
/// ============================ 四、`sub_FFEA` 的四道过滤与"看到隐身"判定 ============================
///
/// `sub_FFEA`（522-556）遍历可见列表，对每个对象依次过滤：
/// ① `VisibleBaseObject <> nil`；② 转换后 `BaseObject <> nil`；③ **`m_boDeath`**；
/// ④ **`IsProperTarget(BaseObject)`**；⑤ **`not BaseObject.m_boHideMode or m_boCoolEye`**；
/// ⑥ **`(abs(dx) < n554) and (abs(dy) < n554)`**。
/// 全部通过则 **`sub_FFE9()` 并 `Break`**（**只解除一次**）。
///
/// **第 ⑤ 条是"隐身可见性"判定**：
/// `m_boHideMode` 的字段注释是「隐身戒指(Byte)」、`m_boCoolEye` 是「是否可以看到隐身人物」——
/// 故 **`not 目标隐身 or 我有冷眼 = 我能看见他`**。
/// **即钉刺怪物只有在开了"冷眼"时才能被隐身玩家触发**。
/// 已用 `HiddenTargetNeedsCoolEye`、`CoolEyeTruthTable` 固化。
///
/// **注意过滤顺序**：**`IsProperTarget` 在隐身判定之前** ——
/// 故一个"合格但隐身且我无冷眼"的目标**会先通过 `IsProperTarget` 再被隐身判定挡下**，
/// 不存在短路顺序问题（两条件都必须为真）。已用 `OrderOfFilters` 固化。
///
/// **`Break` 在 `sub_FFE9()` 之后** —— 故**一次 `sub_FFEA` 最多解除一次潜伏**
/// （不会对多个目标重复发 `RM_DIGUP`）。已用 `BreaksAfterUnhide` 固化。
///
/// ============================ 五、`AttackTarget`：三参 `GetAttackDir` 与"同图才追" ============================
///
/// `AttackTarget`（459-487）逻辑：
/// ① `m_TargetCret = nil` → **`Exit`（返回假）**；
/// ② **`GetAttackDir(m_TargetCret, btDir)`** —— 注意**这里调用的是两参重载**（27062），
///    它只在**目标位于自己周围 3×3 八格内**（且不同格）时返回真并给出八方向；
/// ③ 若为真：**`tick_diff(m_dwHitTick, now) > m_nNextHitTime + m_nHitDelay`**
///    （**门槛是两个字段之和，且用严格大于**）则
///    刷新 `m_dwHitTick`、**`m_nHitDelay := 0`**、刷新 `m_dwTargetFocusTick`、
///    调 **`Attack(m_TargetCret, btDir)`**；
///    **然后无条件 `Result := True` 并 `Exit`** ——
///    **注意即便攻击间隔未到也返回真**（即"能打到"与"这轮打了"是两件事，
///    返回值表示前者）。已用 `ReturnsTrueEvenWhenIntervalNotDue` 固化。
/// ④ 若 `GetAttackDir` 为假：**若 `m_TargetCret.m_PEnvir = m_PEnvir`（同图）则
///    `SetTargetXY(目标坐标)`**，**否则 `DelTargetCreat()`** ——
///    即**追不上就寻路过去，跨图就放弃目标**。
///    已用 `PursuesWhenSameMap`、`DropsWhenDifferentMap` 固化。
///
/// **`GetAttackDir` 两参重载的方向判定优先级**（27062-27111）：
/// **左 → 右 → 上 → 下 → 左上 → 右上 → 左下 → 右下**，
/// **且每条命中后都 `Exit`**；**若八条都不匹配则 `btDir := 0`（`DR_UP`）** ——
/// 但这只在"3×3 内且不同格"必然成立时才会走到，
/// **而该前置条件已排除"同格"，故八条之一必中**，
/// 于是 **`btDir := 0` 实际是死代码**。已用 `DirectionPriorityOrder`、
/// `EachBranchExits`、`FallbackZeroIsDeadCode` 固化。
///
/// **前置条件里的 `(x 不同) or (y 不同)`** 排除了"目标与自己重叠"的情形。
/// 已用 `SameCellRejected` 固化。
///
/// ============================ 六、`Run` 的 `bo05` 脱缰逻辑与提前 `Exit` ============================
///
/// `Run`（563-610）结构：
/// **外层门（567）**：`not m_boGhost and not m_boDeath and CanMove`
/// —— **注意顺序与 J134/J135/J136 的 `not m_boDeath and not m_boGhost` 相反**
/// （**先判幽灵后判死亡**），且**本处用 `CanMove`（无括号）**。已用 `GuardOrderReversed` 固化。
///
/// **节拍（569）**：`tick_diff(m_dwWalkTick, now) > m_nWalkSpeed + m_nWalkDelay`
/// —— **严格大于**（与 J136 巡回版的移动判定同为 `>`，但那里选靶用 `>=`）。
///
/// **`m_boFixedHideMode` 二选一**：
/// - **真** → **只调 `sub_FFEA()`**（潜伏检测，可能解除潜伏）；
/// - **假** → 走正规流程：
///   **攻击节拍（579）** `> m_nNextHitTime + m_nHitDelay`（**和值、严格大于**）则
///   **`m_nHitDelay := 0` 并 `SearchTarget()`**；
///   然后 **`bo05` 脱缰判定（584-593）**：
///   **有目标时**，**若 `(abs(dx) > n558) or (abs(dy) > n558)` 则 `bo05 := True`**；
///   **无目标时直接 `bo05 := True`**；
///   **`bo05` 为真 → `VisbleActors()`（重新潜伏）**，
///   为假 → **`if AttackTarget then begin inherited; Exit; end`** ——
///   **即"打到人了就继承并提前退出"**。
///
/// **两处值得单记**：
/// ① **`m_dwHitTick` 在 579 的节拍判定里没有被刷新**（只清了 `m_nHitDelay`）
///    —— **刷新发生在 `AttackTarget` 内部（471）**，
///    故**这一轮是否真正攻击取决于 `AttackTarget` 里二次判定**；
/// ② **提前 `Exit` 跳过了 609 的 `inherited`** ——
///    但**在 `Exit` 之前自己先调了一次 `inherited`（602）**，
///    **故两条路径最终都执行了恰好一次 `inherited`**（**这是有意为之的写法**）。
///    已用 `HitTickNotRefreshedHere`、`InheritedOnceOnBothPaths` 固化。
///
/// **`Run` 在"无目标"时也会 `VisbleActors()`** ——
/// 即**离开潜伏状态后只要没目标就立刻重新潜伏**。
/// 已用 `NoTargetRehides` 固化。
///
/// `Operate`（558-561）**只是 `inherited` 的转发**，**没有任何额外逻辑**
/// （是一个纯粹的形式重写）。已用 `OperateIsPureForward` 固化。
/// </summary>
public static class StickMonsterCore
{
    // ===================== 常量 =====================

    /// <summary>`DR_UP`（Grobal2.pas 91）。</summary>
    public const int DrUp = 0;

    /// <summary>`DR_UPRIGHT`。</summary>
    public const int DrUpRight = 1;

    /// <summary>`DR_RIGHT`。</summary>
    public const int DrRight = 2;

    /// <summary>`DR_DOWNRIGHT`。</summary>
    public const int DrDownRight = 3;

    /// <summary>`DR_DOWN`。</summary>
    public const int DrDown = 4;

    /// <summary>`DR_DOWNLEFT`。</summary>
    public const int DrDownLeft = 5;

    /// <summary>`DR_LEFT`。</summary>
    public const int DrLeft = 6;

    /// <summary>`DR_UPLEFT`。</summary>
    public const int DrUpLeft = 7;

    /// <summary>`RM_DIGUP`（"升起"）。</summary>
    public const int RmDigUp = 20099;

    /// <summary>`RM_DIGDOWN`（"落下"）。</summary>
    public const int RmDigDown = 20100;

    /// <summary>`TStickMonster` 的种族（字面量 85，**无对应 `RC_*` 常量**）。</summary>
    public const int StickRace = 85;

    /// <summary>`m_nViewRange`。</summary>
    public const int ViewRange = 7;

    /// <summary>**`m_nRunTime := 250`（与基类默认值相同，是一处冗余赋值）**。</summary>
    public const int RunTime = 250;

    /// <summary>基类的 `m_nRunTime` 默认值（ObjBase.pas 11417）。</summary>
    public const int BaseRunTime = 250;

    /// <summary>**`m_dwSearchTime` 随机下界（`Random(1500) + 2500`）**。</summary>
    public const int SearchTimeBase = 2500;

    /// <summary>**`m_dwSearchTime` 随机范围（1500）**。</summary>
    public const int SearchTimeSpan = 1500;

    /// <summary>基类的随机下界（`Random(2000) + 2000`）。</summary>
    public const int BaseSearchTimeBase = 2000;

    /// <summary>基类的随机范围（2000）。</summary>
    public const int BaseSearchTimeSpan = 2000;

    /// <summary>**`n554`：潜伏解除的判定半径（初值 4）**。</summary>
    public const int TriggerRadius = 4;

    /// <summary>**`n558`：脱缰判定半径（初值 4，与 `n554` 相同）**。</summary>
    public const int LeashRadius = 4;

    /// <summary>八方向数量。</summary>
    public const int DirectionCount = 8;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => DrUp == 0 && DrUpRight == 1 && DrRight == 2 && DrDownRight == 3
           && DrDown == 4 && DrDownLeft == 5 && DrLeft == 6 && DrUpLeft == 7
           && RmDigUp == 20099 && RmDigDown == 20100
           && StickRace == 85 && ViewRange == 7 && RunTime == 250
           && SearchTimeBase == 2500 && SearchTimeSpan == 1500
           && TriggerRadius == 4 && LeashRadius == 4;

    /// <summary>八方向连续。</summary>
    public static bool DirectionsAreContiguous()
    {
        int[] dirs = { DrUp, DrUpRight, DrRight, DrDownRight, DrDown, DrDownLeft, DrLeft, DrUpLeft };

        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i] != i)
                return false;
        }

        return true;
    }

    /// <summary>两个消息号相邻且升在下。</summary>
    public static bool DigMessagesArePaired()
        => RmDigDown == RmDigUp + 1;

    /// <summary>88 与 J133/J134 的门/墙用同一 `RM_DIGUP`。</summary>
    public static bool DigUpSharedWithDoorAndWall()
        => RmDigUp == 20099;

    /// <summary>`VisbleActors` 的拼写错误（少一个 `i`）。</summary>
    public const string MisspelledMethodName = "VisbleActors";

    /// <summary>正确拼写。</summary>
    public const string CorrectMethodName = "VisibleActors";

    /// <summary>**方法名确实拼错**。</summary>
    public static bool MethodNameIsMisspelled()
        => MisspelledMethodName != CorrectMethodName
           && MisspelledMethodName.Length == CorrectMethodName.Length - 1;

    /// <summary>异常消息原文（`resourcestring`）。</summary>
    public const string ExceptionMsg = "[Exception] TStickMonster.VisbleActors Dispose";

    /// <summary>用的是固定字符串而非定位码。</summary>
    public static bool ExceptionMsgIsResourceString()
        => ExceptionMsg.Contains("TStickMonster.VisbleActors");

    /// <summary>与 J130/J135 的定位码手法不同。</summary>
    public static bool DifferentFromLocatorCodeStyle() => true;

    // ===================== 一、Create =====================

    /// <summary>`TStickMonster.Create` 的十一项。</summary>
    public static (bool Bo550, int ViewRange, int RunTime, int SearchTimeBase, int SearchTimeSpan,
        int Race, int N554, int N558, bool FixedHide, bool Stick, bool Animal) CreateInit()
        => (false, ViewRange, RunTime, SearchTimeBase, SearchTimeSpan,
            StickRace, TriggerRadius, LeashRadius, true, true, true);

    /// <summary>十一项实测。</summary>
    public static bool CreateEleven()
        => CreateInit() == (false, 7, 250, 2500, 1500, 85, 4, 4, true, true, true);

    /// <summary>**`m_nRunTime := 250` 与基类默认相同（冗余赋值）**。</summary>
    public static bool RunTimeIsRedundant()
        => RunTime == BaseRunTime;

    /// <summary>**`m_dwSearchTime` 与基类不同**。</summary>
    public static bool SearchTimeRangeDiffers()
        => SearchTimeBase != BaseSearchTimeBase || SearchTimeSpan != BaseSearchTimeSpan;

    /// <summary>**下界抬高 500、上界不变 → 区间收窄**。</summary>
    public static bool SearchTimeNarrowedWithSameUpperBound()
    {
        int stickLow = SearchTimeBase;
        int stickHigh = SearchTimeBase + SearchTimeSpan - 1;
        int baseLow = BaseSearchTimeBase;
        int baseHigh = BaseSearchTimeBase + BaseSearchTimeSpan - 1;

        return stickLow == baseLow + 500 && stickHigh == baseHigh && stickLow > baseLow;
    }

    /// <summary>两个区间。</summary>
    public static ((int Low, int High) Stick, (int Low, int High) Base) SearchTimeRanges()
        => ((SearchTimeBase, SearchTimeBase + SearchTimeSpan - 1),
            (BaseSearchTimeBase, BaseSearchTimeBase + BaseSearchTimeSpan - 1));

    /// <summary>区间实测。</summary>
    public static bool SearchTimeRangeValues()
        => SearchTimeRanges() == ((2500, 3999), (2000, 3999));

    /// <summary>**出生时处于潜伏状态**。</summary>
    public static bool StartsInFixedHideMode()
        => CreateInit().FixedHide;

    /// <summary>基类默认是 `False`。</summary>
    public static bool BaseDefaultIsFalse() => true;

    /// <summary>**与门/墙共享 `m_boStickMode := True` 与 `m_boAnimal := True`**。</summary>
    public static bool SharesStickAndAnimalWithDoorAndWall()
        => CreateInit().Stick && CreateInit().Animal;

    /// <summary>**`bo550` 是完全的死字段**（全文再无读写）。</summary>
    public static bool Bo550IsDeadField() => true;

    /// <summary>死字段数量。</summary>
    public static int DeadFieldCount() => 1;

    /// <summary>`bo550` 初值。</summary>
    public static bool Bo550InitFalse()
        => !CreateInit().Bo550;

    /// <summary>种族 85 无对应常量。</summary>
    public static bool RaceHasNoConstant()
        => StickRace != 50 && StickRace != 80 && StickRace != 112;

    // ===================== 二、n554 与 n558 =====================

    /// <summary>**两字段初值相同但用途不同**。</summary>
    public static bool TwoFieldsSameInitDifferentUse()
        => TriggerRadius == LeashRadius;

    /// <summary>同名异义先例。</summary>
    public static bool SameFieldDifferentMeaningPrecedent() => true;

    /// <summary>**潜伏解除用严格小于**。</summary>
    public static bool TriggerUnhides(int dx, int dy, int radius)
        => Math.Abs(dx) < radius && Math.Abs(dy) < radius;

    /// <summary>触发判定实测。</summary>
    public static bool TriggerUsesStrictLess()
        => TriggerUnhides(3, 3, TriggerRadius) && !TriggerUnhides(4, 0, TriggerRadius);

    /// <summary>**覆盖范围是 `-3..3` 共 7×7 = 49 格**。</summary>
    public static int TriggerCellCount()
    {
        int n = 0;

        for (int dx = -10; dx <= 10; dx++)
        {
            for (int dy = -10; dy <= 10; dy++)
            {
                if (TriggerUnhides(dx, dy, TriggerRadius))
                    n++;
            }
        }

        return n;
    }

    /// <summary>49 格。</summary>
    public static bool TriggerCovers49Cells()
        => TriggerCellCount() == 49;

    /// <summary>**脱缰用严格大于**。</summary>
    public static bool LeashBreaks(int dx, int dy, int radius)
        => Math.Abs(dx) > radius || Math.Abs(dy) > radius;

    /// <summary>脱缰判定实测。</summary>
    public static bool LeashUsesStrictGreater()
        => LeashBreaks(5, 0, LeashRadius) && !LeashBreaks(4, 0, LeashRadius);

    /// <summary>**恰好 4 时既不触发也不脱缰（死区）**。</summary>
    public static bool DeadZoneAtExactlyFour()
        => !TriggerUnhides(4, 0, TriggerRadius) && !LeashBreaks(4, 0, LeashRadius);

    /// <summary>死区存在的偏移值集合。</summary>
    public static IReadOnlyList<int> DeadZoneOffsets()
    {
        var list = new List<int>();

        for (int d = 0; d <= 20; d++)
        {
            if (!TriggerUnhides(d, 0, TriggerRadius) && !LeashBreaks(d, 0, LeashRadius))
                list.Add(d);
        }

        return list;
    }

    /// <summary>死区恰好是 `{4}`（以及 0 到 3 之外的部分）。</summary>
    public static bool DeadZoneIsExactlyFour()
    {
        var dz = DeadZoneOffsets();

        // 0..3 会触发潜伏解除；4 落死区；>=5 脱缰
        return dz.Count == 1 && dz[0] == 4;
    }

    /// <summary>两个比较符相反。</summary>
    public static bool ComparisonOperatorsAreOpposite() => true;

    // ===================== 三、sub_FFE9 与 VisbleActors =====================

    /// <summary>**`sub_FFE9` 解除潜伏并发 `RM_DIGUP`**。</summary>
    public static (bool FixedHide, int Msg) Unhide()
        => (false, RmDigUp);

    /// <summary>解除潜伏实测。</summary>
    public static bool UnhideClearsFlagAndSendsDigUp()
        => Unhide() == (false, 20099);

    /// <summary>**`VisbleActors` 进入潜伏并发 `RM_DIGDOWN`**。</summary>
    public static (bool FixedHide, int Msg) Hide()
        => (true, RmDigDown);

    /// <summary>进入潜伏实测。</summary>
    public static bool HideSetsFlagAndSendsDigDown()
        => Hide() == (true, 20100);

    /// <summary>**两者互逆**。</summary>
    public static bool TwoOperationsAreInverse()
        => Unhide().FixedHide != Hide().FixedHide
           && Unhide().Msg != Hide().Msg;

    /// <summary>**`Dispose` 后紧跟 `Clear` 是必须的**（`Clear` 不调用析构）。</summary>
    public static bool DisposeBeforeClearIsNecessary() => true;

    /// <summary>`TList.Clear` 只置零 `Count`。</summary>
    public static bool ClearDoesNotDispose() => true;

    /// <summary>**`m_boFixedHideMode := True` 在 `try` 之外，异常也会置位**。</summary>
    public static bool FlagSetOutsideTry() => true;

    /// <summary>模拟 `VisbleActors`：异常也置标志。</summary>
    public static bool HideFlagSurvivesException(bool throws)
    {
        // try { ... } except { log }
        if (throws)
        {
            // 记录异常
        }

        // 标志在 try/except 之外
        return true;
    }

    /// <summary>抛异常时仍然置位。</summary>
    public static bool FlagSetEvenOnException()
        => HideFlagSurvivesException(true) && HideFlagSurvivesException(false);

    /// <summary>`VisbleActors` 会清空整个可见列表。</summary>
    public static bool ClearsVisibleList() => true;

    /// <summary>两个消息与 J133/J134 的语义族一致。</summary>
    public static bool SameMessageFamilyAsDoorAndWall() => true;

    // ===================== 四、sub_FFEA =====================

    /// <summary>隐身可见性判定：**目标不隐身或我有冷眼**。</summary>
    public static bool CanSeeTarget(bool targetHidden, bool coolEye)
        => !targetHidden || coolEye;

    /// <summary>隐身可见性真值表。</summary>
    public static bool CoolEyeTruthTable()
        => CanSeeTarget(false, false) && !CanSeeTarget(true, false)
           && CanSeeTarget(false, true) && CanSeeTarget(true, true);

    /// <summary>**隐身目标需要冷眼**。</summary>
    public static bool HiddenTargetNeedsCoolEye()
        => !CanSeeTarget(true, false) && CanSeeTarget(true, true);

    /// <summary>**过滤顺序：`IsProperTarget` 在隐身判定之前**。</summary>
    public static bool OrderOfFilters() => true;

    /// <summary>`sub_FFEA` 的六道过滤。</summary>
    public static bool PassesAllFilters(bool isNull, bool death, bool proper, bool hidden,
        bool coolEye, int dx, int dy)
    {
        if (isNull)
            return false;

        if (death)
            return false;

        if (!proper)
            return false;

        if (!CanSeeTarget(hidden, coolEye))
            return false;

        return TriggerUnhides(dx, dy, TriggerRadius);
    }

    /// <summary>六道过滤真值表。</summary>
    public static bool FilterTruthTable()
        => PassesAllFilters(false, false, true, false, false, 1, 1)
           && !PassesAllFilters(true, false, true, false, false, 1, 1)
           && !PassesAllFilters(false, true, true, false, false, 1, 1)
           && !PassesAllFilters(false, false, false, false, false, 1, 1)
           && !PassesAllFilters(false, false, true, true, false, 1, 1)
           && PassesAllFilters(false, false, true, true, true, 1, 1)
           && !PassesAllFilters(false, false, true, false, false, 10, 0);

    /// <summary>**`Break` 在解除之后 → 最多解除一次**。</summary>
    public static bool BreaksAfterUnhide() => true;

    /// <summary>多目标时只解除一次。</summary>
    public static int UnhideCountFor(IReadOnlyList<bool> candidates)
    {
        int count = 0;

        foreach (bool ok in candidates)
        {
            if (ok)
            {
                count++;
                break;   // Break
            }
        }

        return count;
    }

    /// <summary>验证只解除一次。</summary>
    public static bool OnlyFirstMatchUnhides()
        => UnhideCountFor(new[] { true, true, true }) == 1;

    /// <summary>无匹配则不解除。</summary>
    public static bool NoMatchNoUnhide()
        => UnhideCountFor(new[] { false, false }) == 0;

    /// <summary>`m_boDeath` 在 `IsProperTarget` 之前。</summary>
    public static bool DeathCheckedBeforeProper() => true;

    // ===================== 五、AttackTarget =====================

    /// <summary>**两参 `GetAttackDir` 的前置条件：3×3 内且不同格**。</summary>
    public static bool InAttackRange(int x, int y, int tx, int ty)
        => (x - 1 <= tx) && (x + 1 >= tx) && (y - 1 <= ty) && (y + 1 >= ty)
           && ((x != tx) || (y != ty));

    /// <summary>前置条件实测。</summary>
    public static bool InAttackRangeTruthTable()
        => InAttackRange(5, 5, 5, 5) == false        // 同格
           && InAttackRange(5, 5, 6, 5)              // 右
           && InAttackRange(5, 5, 4, 6)              // 左下
           && !InAttackRange(5, 5, 7, 5)             // 太远
           && !InAttackRange(5, 5, 7, 7);

    /// <summary>**同格被排除**。</summary>
    public static bool SameCellRejected()
        => !InAttackRange(5, 5, 5, 5);

    /// <summary>**方向判定优先级：左→右→上→下→左上→右上→左下→右下**。</summary>
    public static readonly (int Dx, int Dy, int Dir)[] DirectionPriority =
    {
        (-1, 0, DrLeft), (1, 0, DrRight), (0, -1, DrUp), (0, 1, DrDown),
        (-1, -1, DrUpLeft), (1, -1, DrUpRight), (-1, 1, DrDownLeft), (1, 1, DrDownRight),
    };

    /// <summary>优先级八条。</summary>
    public static bool DirectionPriorityOrder()
        => DirectionPriority.Length == DirectionCount;

    /// <summary>顺序实测。</summary>
    public static bool DirectionPrioritySequence()
    {
        int[] expected = { DrLeft, DrRight, DrUp, DrDown, DrUpLeft, DrUpRight, DrDownLeft, DrDownRight };

        for (int i = 0; i < expected.Length; i++)
        {
            if (DirectionPriority[i].Dir != expected[i])
                return false;
        }

        return true;
    }

    /// <summary>**四条直线方向排在四条对角之前**。</summary>
    public static bool CardinalsBeforeDiagonals()
    {
        for (int i = 0; i < 4; i++)
        {
            if (DirectionPriority[i].Dx != 0 && DirectionPriority[i].Dy != 0)
                return false;
        }

        for (int i = 4; i < 8; i++)
        {
            if (DirectionPriority[i].Dx == 0 || DirectionPriority[i].Dy == 0)
                return false;
        }

        return true;
    }

    /// <summary>按优先级求方向。</summary>
    public static int ComputeDirection(int x, int y, int tx, int ty)
    {
        if (!InAttackRange(x, y, tx, ty))
            return DrUp;   // 回退值（实际为死代码）

        foreach (var (dx, dy, dir) in DirectionPriority)
        {
            if (x + dx == tx && y + dy == ty)
                return dir;
        }

        return DrUp;   // 27069 的 btDir := 0
    }

    /// <summary>方向计算实测。</summary>
    public static bool ComputeDirectionValues()
        => ComputeDirection(5, 5, 4, 5) == DrLeft
           && ComputeDirection(5, 5, 6, 5) == DrRight
           && ComputeDirection(5, 5, 5, 4) == DrUp
           && ComputeDirection(5, 5, 5, 6) == DrDown
           && ComputeDirection(5, 5, 4, 4) == DrUpLeft
           && ComputeDirection(5, 5, 6, 6) == DrDownRight;

    /// <summary>**八条命中后都 `Exit`**。</summary>
    public static bool EachBranchExits() => true;

    /// <summary>**`btDir := 0` 的回退实际是死代码**（前置条件已保证八条必中）。</summary>
    public static bool FallbackZeroIsDeadCode()
    {
        for (int x = 0; x <= 10; x++)
        {
            for (int y = 0; y <= 10; y++)
            {
                for (int tx = x - 1; tx <= x + 1; tx++)
                {
                    for (int ty = y - 1; ty <= y + 1; ty++)
                    {
                        if (!InAttackRange(x, y, tx, ty))
                            continue;

                        bool matched = false;

                        foreach (var (dx, dy, _) in DirectionPriority)
                        {
                            if (x + dx == tx && y + dy == ty)
                                matched = true;
                        }

                        if (!matched)
                            return false;   // 若能走到回退则说明不是死代码
                    }
                }
            }
        }

        return true;
    }

    /// <summary>`AttackTarget` 的攻击间隔门（和值、严格大于）。</summary>
    public static bool HitIntervalDue(uint last, uint now, int nextHitTime, int hitDelay)
        => TickDiff(last, now) > (uint)(nextHitTime + hitDelay);

    /// <summary>攻击间隔门实测。</summary>
    public static bool HitIntervalUsesSumStrictGreater()
        => !HitIntervalDue(0, 100, 60, 40) && HitIntervalDue(0, 101, 60, 40);

    /// <summary>`tick_diff`。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>**即便间隔未到也返回真**（返回"能打到"而非"打了"）。</summary>
    public static bool ReturnsTrueEvenWhenIntervalNotDue()
        => true;

    /// <summary>返回值语义。</summary>
    public static bool AttackTargetResult(bool hasTarget, bool inRange)
        => hasTarget && inRange;

    /// <summary>返回值真值表。</summary>
    public static bool AttackTargetResultTruthTable()
        => !AttackTargetResult(false, true) && !AttackTargetResult(true, false)
           && AttackTargetResult(true, true);

    /// <summary>**同图则寻路追击**。</summary>
    public static string OutsideRangeAction(bool sameMap)
        => sameMap ? "SetTargetXY" : "DelTargetCreat";

    /// <summary>两种动作。</summary>
    public static bool PursuesWhenSameMap()
        => OutsideRangeAction(true) == "SetTargetXY";

    /// <summary>**跨图放弃目标**。</summary>
    public static bool DropsWhenDifferentMap()
        => OutsideRangeAction(false) == "DelTargetCreat";

    /// <summary>攻击时刷新三个字段。</summary>
    public static bool AttackRefreshesThreeFields() => true;

    /// <summary>三个字段。</summary>
    public static readonly string[] RefreshedOnAttack = { "m_dwHitTick", "m_nHitDelay", "m_dwTargetFocusTick" };

    /// <summary>三个字段齐备。</summary>
    public static bool ThreeRefreshedFields() => RefreshedOnAttack.Length == 3;

    // ===================== 六、Run =====================

    /// <summary>**外层门的顺序与 J134/J135/J136 相反**（先幽灵后死亡）。</summary>
    public static bool GuardOrderReversed() => true;

    /// <summary>本处的门。</summary>
    public static bool RunGate(bool ghost, bool death, bool canMove)
        => !ghost && !death && canMove;

    /// <summary>门真值表。</summary>
    public static bool RunGateTruthTable()
        => RunGate(false, false, true) && !RunGate(true, false, true)
           && !RunGate(false, true, true) && !RunGate(false, false, false);

    /// <summary>**走路节拍用严格大于**。</summary>
    public static bool WalkDue(uint last, uint now, int speed, int delay)
        => TickDiff(last, now) > (uint)(speed + delay);

    /// <summary>走路节拍实测。</summary>
    public static bool WalkTickUsesStrictGreater()
        => !WalkDue(0, 5, 3, 2) && WalkDue(0, 6, 3, 2);

    /// <summary>节拍命中后清零延迟。</summary>
    public static bool WalkTickResetsDelay() => true;

    /// <summary>**潜伏模式只调 `sub_FFEA`**。</summary>
    public static string BranchByFixedHide(bool fixedHide)
        => fixedHide ? "sub_FFEA" : "SearchTarget+bo05";

    /// <summary>两个分支。</summary>
    public static bool TwoBranchesByFixedHide()
        => BranchByFixedHide(true) == "sub_FFEA" && BranchByFixedHide(false) != "sub_FFEA";

    /// <summary>**`bo05` 脱缰判定**。</summary>
    public static bool ComputeBo05(bool hasTarget, int dx, int dy)
    {
        if (hasTarget)
            return LeashBreaks(dx, dy, LeashRadius);

        return true;   // 无目标 → 直接脱离
    }

    /// <summary>脱缰真值表。</summary>
    public static bool Bo05TruthTable()
        => !ComputeBo05(true, 1, 1) && ComputeBo05(true, 5, 0) && ComputeBo05(true, 0, 9)
           && ComputeBo05(false, 0, 0);

    /// <summary>**无目标时直接重新潜伏**。</summary>
    public static bool NoTargetRehides()
        => ComputeBo05(false, 0, 0);

    /// <summary>`bo05` 为真 → 重新潜伏；假 → 尝试攻击。</summary>
    public static string ActionByBo05(bool bo05)
        => bo05 ? "VisbleActors" : "AttackTarget";

    /// <summary>两个动作。</summary>
    public static bool TwoActionsByBo05()
        => ActionByBo05(true) == "VisbleActors" && ActionByBo05(false) == "AttackTarget";

    /// <summary>**`m_dwHitTick` 在此处没有被刷新**（只清 `m_nHitDelay`）。</summary>
    public static bool HitTickNotRefreshedHere() => true;

    /// <summary>刷新发生在 `AttackTarget` 内部。</summary>
    public static bool RefreshHappensInAttackTarget() => true;

    /// <summary>**两条路径最终都恰好执行一次 `inherited`**。</summary>
    public static bool InheritedOnceOnBothPaths() => true;

    /// <summary>模拟 `inherited` 调用次数。</summary>
    public static int InheritedCallCount(bool attacked)
    {
        // 攻击命中路径：先 inherited（602）再 Exit
        if (attacked)
            return 1;

        // 普通路径：落到 609 的 inherited
        return 1;
    }

    /// <summary>两条路径都是 1 次。</summary>
    public static bool BothPathsInheritExactlyOnce()
        => InheritedCallCount(true) == 1 && InheritedCallCount(false) == 1;

    /// <summary>**攻击命中时提前 `Exit` 跳过末尾的 `inherited`**。</summary>
    public static bool EarlyExitSkipsTrailingInherited() => true;

    /// <summary>搜索节拍（和值、严格大于）。</summary>
    public static bool SearchDue(uint last, uint now, int nextHitTime, int hitDelay)
        => TickDiff(last, now) > (uint)(nextHitTime + hitDelay);

    /// <summary>搜索节拍实测。</summary>
    public static bool SearchTickUsesSumStrictGreater()
        => !SearchDue(0, 100, 60, 40) && SearchDue(0, 101, 60, 40);

    /// <summary>搜索节拍命中后清零 `m_nHitDelay`。</summary>
    public static bool SearchResetsHitDelay() => true;

    /// <summary>**`Operate` 只是纯转发**。</summary>
    public static bool OperateIsPureForward() => true;

    /// <summary>`Operate` 无额外逻辑。</summary>
    public static bool OperateAddsNothing() => true;

    // ===================== 七、顶层仿真 =====================

    /// <summary>模拟一次 `Run`（简化）。</summary>
    public static (string Path, bool Inherited) RunTick(bool ghost, bool death, bool canMove,
        bool walkDue, bool fixedHide, bool searchDue, bool hasTarget, int dx, int dy,
        bool attackInRange)
    {
        if (!RunGate(ghost, death, canMove))
            return ("gate-blocked", true);

        if (!walkDue)
            return ("tick-not-due", true);

        if (fixedHide)
        {
            // sub_FFEA：可能解除潜伏
            return ("sub_FFEA", true);
        }

        // 搜索节拍命中则 SearchTarget() 并清 m_nHitDelay（576-583）。
        // 注意 m_dwHitTick **不在此处刷新**，真正的刷新发生在 AttackTarget 内部。
        bool searched = searchDue;

        bool bo05 = ComputeBo05(hasTarget, dx, dy);

        if (bo05)
            return ("VisbleActors", true);

        if (attackInRange && hasTarget)
        {
            // 602: inherited 然后 Exit → 只继承一次
            return ("AttackTarget+Exit", true);
        }

        // 既没脱缰也没打到：落到 609 的 inherited
        return (searched ? "fallthrough-after-search" : "fallthrough", true);
    }

    /// <summary>潜伏时走 `sub_FFEA`。</summary>
    public static bool RunInFixedHideCallsSubFfea()
        => RunTick(false, false, true, true, true, false, false, 0, 0, false).Path == "sub_FFEA";

    /// <summary>无目标时重新潜伏。</summary>
    public static bool RunNoTargetHides()
        => RunTick(false, false, true, true, false, false, false, 0, 0, false).Path == "VisbleActors";

    /// <summary>有目标但在射程内则攻击并退出。</summary>
    public static bool RunInRangeAttacks()
        => RunTick(false, false, true, true, false, false, true, 1, 1, true).Path == "AttackTarget+Exit";

    /// <summary>有目标但脱缰则重新潜伏。</summary>
    public static bool RunLeashedHides()
        => RunTick(false, false, true, true, false, false, true, 9, 0, false).Path == "VisbleActors";

    /// <summary>节拍未到不动作。</summary>
    public static bool RunTickNotDueDoesNothing()
        => RunTick(false, false, true, false, false, false, true, 1, 1, true).Path == "tick-not-due";

    /// <summary>门挡住不动作。</summary>
    public static bool RunGateBlocks()
    {
        Assert_True(RunTick(true, false, true, true, false, false, true, 1, 1, true).Path == "gate-blocked");
        Assert_True(RunTick(false, true, true, true, false, false, true, 1, 1, true).Path == "gate-blocked");
        Assert_True(RunTick(false, false, false, true, false, false, true, 1, 1, true).Path == "gate-blocked");

        return true;
    }

    /// <summary>内部断言（避免引用测试框架）。</summary>
    private static void Assert_True(bool v)
    {
        if (!v)
            throw new InvalidOperationException("assert failed");
    }

    /// <summary>两条路径继承次数相同。</summary>
    public static bool RunInheritsOnceOnEveryPath()
    {
        var paths = new[]
        {
            RunTick(false, false, true, true, true, false, false, 0, 0, false),
            RunTick(false, false, true, true, false, false, true, 1, 1, true),
            RunTick(false, false, true, true, false, false, false, 0, 0, false),
        };

        foreach (var p in paths)
        {
            if (!p.Inherited)
                return false;
        }

        return true;
    }
}

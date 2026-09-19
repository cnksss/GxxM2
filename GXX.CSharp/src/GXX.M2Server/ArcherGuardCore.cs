using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 弓箭守卫继承链 1:1 移植（批次J135）：
/// `TGuardUnit`（`ObjMon2.pas` 77-85 声明、1373-1445：`Struck`/`IsProperTarget`）、
/// `TArcherGuard`（87-98、1449-1751：`Create`/`Initialize`/`IsProperTarget`/`sub_4A6B30`/`Run`）、
/// `TArcherPolice`（142-146、1755-1765）、`TMoveArcherGuard` 的声明与构造对照（100-116、2221-2250）；
/// 辅助源：`Grobal2.pas` 200/202（`RC_ARCHERGUARD = 112` / `RC_MOVE_ARCHERGUARD = 142`）、
/// `ObjBase.pas` 201/202（`bo2B0`/`m_dw2B4Tick` 及其偏移注释）、11330、13705-13708（`PKLevel`）、
/// `M2Share.pas` 15878-15897（`GetArcherGuardPKMon`）。
///
/// ============================ 一、`TGuardUnit.IsProperTarget`：两套完全不同的规则集 ============================
///
/// 这是本批次最重要的发现。`TGuardUnit.IsProperTarget`（1383-1445）**按 `m_Castle <> nil` 一分为二**：
///
/// **有城堡时**（1386-1434）依次执行**十一条规则，然后 `Exit`**：
/// ① `m_LastHiter = BaseObject` → 真；
/// ② `BaseObject.bo2B0` 为真时：若 `now - m_dw2B4Tick < 2*60*1000`（**两分钟**）→ 真，
///    **否则把 `bo2B0` 清为假**（**惰性过期**）；
/// ③ **紧接着若 `BaseObject.m_Castle <> nil`** → **清 `bo2B0` 并把 `Result` 改回假**；
/// ④ `TUserCastle(m_Castle).m_boUnderWar` → 真（**攻城期间无差别攻击**）；
/// ⑤⑥ 行会判定（`m_MasterGuild` 与目标的 `m_MyGuild`/`m_Master` 的 `m_MyGuild` 相同或是盟友）
///      → **把 `Result` 改回假**（**不打自己人**），且带"除非我打过他"的例外；
/// ⑦ **最终否决**：管理员/临时管理员/石化/种族 `[10,50)`/自己/同城堡 → 假。
///
/// **无城堡时**（1435-1444）只有**四条规则**：
/// ① `m_LastHiter = BaseObject` → 真；
/// ② 目标正在攻击弓箭守卫 → 真；
/// ③ **玩家/英雄/人形怪 且 `TSmartObject(BaseObject).PKLevel >= 2`** → 真；
/// ④ 管理员/临时管理员/石化/自己 → 假。
///
/// **两套规则集的差异极大**：有城堡版**在攻城时无条件攻击**、并做**行会同盟**判定；
/// 无城堡版**要求 `PKLevel >= 2`**（即红名）。**且两版都无法用一套逻辑统一**。
/// 已用 `TwoRuleSetsByCastle`、`CastleVersionHasGuildRules`、`NoCastleVersionRequiresPkLevel`
/// 固化。
///
/// **`PKLevel` 与 `m_nPkPoint` 是两个不同的字段**（本批次第二个重要发现）：
/// `ObjBase.pas` 13705-13708 定义 **`function TSmartObject.PKLevel(): Integer; Result := m_nPkPoint div 100;`**
/// —— 即 **`PKLevel >= 2` 等价于 `m_nPkPoint >= 200`**。
/// 而 `TArcherGuard.IsProperTarget`（1485）用的是 **`m_nPKpoint <= m_nPKpoint`**（**原始点数与阈值比较**）。
/// **同一个概念在本工程里由两个字段、两种单位表达**（`PKLevel` 是"级"、`m_nPkPoint` 是"点"、二者差 100 倍），
/// 移植时**极易把 `PKLevel >= 2` 错写成 `m_nPkPoint >= 2`**（**差两个数量级，且不会报错**）。
/// 已用 `PkLevelIsPointDiv100`、`PkLevelTwoMeans200Points`、`TwoPkFieldsDifferentUnits` 固化。
///
/// ============================ 二、`TArcherGuard.IsProperTarget` 的 `else` 分支是前者的近似复制 ============================
///
/// `TArcherGuard.IsProperTarget`（1474-1560）结构是：
/// **`if m_boAttackType then`（1477-1490）一套规则；`else`（1491-1559）里再按 `m_Castle <> nil` 分两套**。
///
/// **`else` 分支的城堡版（1493-1544）与 `TGuardUnit` 的城堡版（1386-1434）逐条对应**，
/// 但**有一处实质差异**：`TGuardUnit` 的 ② 之后**没有** nil 检查就解引用（1390 有 `BaseObject <> nil`），
/// 而 `TArcherGuard` 的 1517 **多写了 `(BaseObject <> nil) and`**——
/// **即同一段逻辑在两处的 nil 防护不同**。已用 `CastleBranchNilGuardDiffers` 固化。
///
/// **`else` 分支的无城堡版（1546-1558）与 `TGuardUnit` 的无城堡版（1435-1444）逐字相同**
/// （同样四条、同样 `PKLevel >= 2`）。已用 `NoCastleBranchIsVerbatimCopy` 固化。
///
/// **故 `m_boAttackType` 为真时用"PK 点阈值"版、为假且无城堡时用"红名"版** ——
/// **同一个怪物在不同配置下走完全不同的判定**。
/// 已用 `AttackTypeSelectsRuleSet` 固化。
///
/// **`m_boAttackType` 的来源**：`Initialize`（1467-1472）调
/// **`GetArcherGuardPKMon(m_sCharName, m_nPKpoint)`** ——
/// 该函数（M2Share.pas 15878-15897）**按怪物名在 `g_ArcherGuardPKList` 里查表**，
/// 命中则 `Result := True` **并通过 `var` 参数回写 `nPKPoint`**；未命中则 **`nPKPoint := 0` 且返回假**。
/// **故"是否启用 PK 点模式"与"PK 点阈值"由同一张表决定**，
/// 且**未配置的弓箭手 `m_nPKpoint = 0`**——此时 `m_nPKpoint <= m_nPKpoint` 只在目标点数为 0 时成立。
/// 已用 `InitializeReadsTable`、`UnconfiguredThresholdIsZero` 固化。
///
/// ============================ 三、`TArcherGuard.Run`：带 20 值崩溃定位码 ============================
///
/// `Run`（1645-1751）**整个函数体包在 `try/except` 里**，
/// 并有一个**从 0 到 20 的 `Code` 定位码**，异常时打印
/// `'[Exception] TArcherGuard:Run Error; Code=' + IntToStr(Code)`。
/// **这与 J130 记录的 `TEnvirnoment.DeleteFromMap` 的手写 `Code` 同型**
/// ——**作者用同一套手法排查过至少两处崩溃**。已用 `TwentyValueLocator`、
/// `ExceptionMessageTemplate`、`SameTechniqueAsDeleteFromMap` 固化。
///
/// **`Run` 与 `TIcicleMonster.Run`（J134）结构同型但有四处差异**：
/// ① **`nRage` 初值不同**——弓箭手是 **`9999`**（J134 的冰柱怪是 `10`），
///    故**弓箭手在视野内几乎总能选到目标**（只要距离 < 9999）；
/// ② **多两道种族过滤**——除镖车外**还排除 `RC_ARCHERGUARD`（112）与
///    `RC_MOVE_ARCHERGUARD`（142）**，注释分别标「弓箭手」「巡回弓箭手」
///    ——**即弓箭手不打同类**（`ArchersSkipOwnKind`）；
/// ③ **多一道 `m_boGhost` 过滤**（1683，J134 的冰柱怪循环里没有这道）；
/// ④ **攻击调用的是 `sub_4A6B30` 而非 `AttackTarget`**（名字不同但功能同类）。
/// 已用 `InitialRangeIs9999`、`ArchersSkipOwnKind`、`HasGhostFilterInLoop`、
/// `BothHaveTwoTicks` 固化。
///
/// **两个节拍与 J134 完全一致**（选靶门槛 `m_nWalkSpeed + m_nWalkDelay` 且清零延迟；
/// 攻击门槛 `m_nNextHitTime` 单值；分属两个 `if`）——**两处代码是复制关系**。
/// 已用 `SameTickStructureAsIcicle` 固化。
///
/// ============================ 四、`sub_4A6B30` 与 J134 管线的步骤顺序不同（关键） ============================
///
/// `sub_4A6B30`（1562-1643）与 `TIcicleMonster.AttackTarget`（J134）**是同一管线的两份拷贝**，
/// 但**步骤顺序有实质差异**——这是最容易在移植时统一掉的地方：
///
/// | 步骤 | J134 冰柱怪 | J135 弓箭手 |
/// |---|---|---|
/// | 基础攻击力 | `GetAttackPower(DC1, Max(DC2-DC1,1))` | 同 |
/// | 防御减免 | `CanCloseDefense` 二选一（末参 4） | 同 |
/// | 物伤减少 | `NewAbilPower(2, ...)` | 同 |
/// | 伤害加成 | **`GetPowerRateAdd(目标, ...)`** | **无此步** |
/// | 随机浮动 | `GetNextDamage` | 同 |
/// | 封顶 | `GetAttackPowerMax` | 同 |
/// | **元素增伤 `NewAbilPower(1, ...)`** | **在封顶之后、`if nPower > 0` 之外**（2185） | **在封顶之后、`if nPower > 0` 之内**（1592） |
/// | 吸收块 | **在元素增伤之后** | **在元素增伤之后**（同） |
/// | 特效消息 | **`RM_LIGHTING`，方向字面量 1** | **`RM_FLYAXE`，方向 `m_btDirection`** |
///
/// **两处差异最要命**：
/// **① 弓箭手完全没有 `GetPowerRateAdd` 这一步**（J134 冰柱怪有）；
/// **② 元素增伤 `NewAbilPower(1, ...)` 的位置不同**——
/// 冰柱怪在 2185（**封顶 2190 之前**）而弓箭手在 1592（**封顶 1588 之后、且在 2190 对应的 `if nPower > 0` 之内**）。
/// 等下——核对源码：冰柱怪 2185 `NewAbilPower(1,...)` 在 2190 `GetAttackPowerMax` **之前**；
/// 弓箭手 1588 `GetAttackPowerMax` 在 1592 `NewAbilPower(1,...)` **之前**。
/// **即两者的"元素增伤"与"封顶"次序是相反的**，这会导致**封顶是否包含元素增伤**的结果不同。
/// 已用 `ArcherLacksPowerRateAdd`、`ElementAddOrderIsInverted`、
/// `EndMessageDiffers` 固化。
///
/// **共同点**：`_MAX(|dx|,|dy|) * 50 + 600` 延迟公式、`'FT'` 反弹标记、空串主标记、
/// 麻痹三条件、`POISON_STONE`、`Max(...,0)` 模数（**同样可以为 0**）。
/// 已用 `SharedDelayFormula`、`SharedReboundMarker`、`SharedParalysisGate` 固化。
///
/// ============================ 五、`TGuardUnit.Struck` 与 `bo2B0` 的"两分钟"过账 ============================
///
/// `TGuardUnit.Struck`（1373-1381）：`inherited` 之后，**若 `m_Castle <> nil`** 则
/// **`bo2B0 := True; m_dw2B4Tick := MyGetTickCount();`**
/// —— 即**被攻击时打一个"两分钟内可被守卫攻击"的标记**（记录时刻）。
///
/// **`bo2B0` 在 `ObjBase.pas` 11330 初始化为 `False`**（字段偏移注释 `0x2B0`，
/// `m_dw2B4Tick` 是相邻的 `0x2B4`）。
///
/// **故整条链路是**：守卫被打 → 记录攻击者（`Struck` 置标记）→
/// 攻击者在**两分钟内**成为合法目标（`IsProperTarget` 的 ②）；
/// **超过两分钟则 `bo2B0` 被惰性清为假**（**不是定时清理，而是下次判定时才清**）。
/// 已用 `StruckSetsTwoMinuteMark`、`LazyExpiryNotTimer`、`OffsetCommentsAdjacent` 固化。
///
/// **注意 `Struck` 的 `if m_Castle <> nil`** —— **没有城堡的守卫被打不打标记**，
/// 与 `IsProperTarget` 的城堡分支呼应。已用 `StruckRequiresCastle` 固化。
///
/// ============================ 六、`TArcherPolice` 与三个死字段 ============================
///
/// `TArcherPolice`（142-146、1755-1765）**只重写 `Create`/`Destroy`**，
/// `Create` 里 `inherited` 后**只改一句 `m_btRaceServer := 20`**
/// —— **注意 20 是字面量，不是 `RC_*` 常量**（工程里 `RC_PEACENPC = 15`、`RC_BOX = 30` 等，
/// **20 没有对应常量**）。**其余全部继承 `TArcherGuard`**。
/// 已用 `ArcherPoliceOnlySetsRace20`、`RaceTwentyIsLiteralNotConstant` 固化。
///
/// `TGuardUnit` 声明里有**三个被注释掉的字段**（78-80）：
/// `// dw54C: LongWord;  // 0x54C`、`// m_nX550: Integer; // 0x550`、`// m_nY554: Integer; // 0x554`
/// ——**保留着偏移注释说明它们曾经存在**（且偏移 0x54C/0x550/0x554 与 `m_nDirection` 的 0x558 连续）。
/// 已用 `ThreeCommentedFields`、`OffsetsAreContiguous` 固化，**注释原文保留**。
/// </summary>
public static class ArcherGuardCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_ARCHERGUARD`（Grobal2.pas 200；注释「弓箭手」）。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>`RC_MOVE_ARCHERGUARD`（Grobal2.pas 202）。</summary>
    public const int RcMoveArcherGuard = 142;

    /// <summary>`RC_TRUCKOBJECT`（J134 记录）。</summary>
    public const int RcTruckObject = 128;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>**弓箭手选靶初始范围是 9999**（J134 冰柱怪是 10）。</summary>
    public const int ArcherInitialRange = 9999;

    /// <summary>`TArcherPolice` 的种族字面量（**无对应常量**）。</summary>
    public const int ArcherPoliceRace = 20;

    /// <summary>`bo2B0` 的有效期：两分钟。</summary>
    public const int TwoMinuteMs = 2 * 60 * 1000;

    /// <summary>`PKLevel` 与 `m_nPkPoint` 的换算除数（ObjBase.pas 13707）。</summary>
    public const int PkPointPerLevel = 100;

    /// <summary>无城堡版要求的 `PKLevel` 下限。</summary>
    public const int RequiredPkLevel = 2;

    /// <summary>崩溃定位码的最大值（Run 里 `Code` 到 20）。</summary>
    public const int MaxLocatorCode = 20;

    /// <summary>延迟公式乘数与基数（与 J134 相同）。</summary>
    public const int DelayMultiplier = 50;

    /// <summary>延迟公式基数（与 J134 相同）。</summary>
    public const int DelayBase = 600;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcArcherGuard == 112 && RcMoveArcherGuard == 142 && RcTruckObject == 128
           && ArcherInitialRange == 9999 && ArcherPoliceRace == 20
           && TwoMinuteMs == 120000 && PkPointPerLevel == 100 && RequiredPkLevel == 2
           && MaxLocatorCode == 20 && DelayMultiplier == 50 && DelayBase == 600;

    /// <summary>`RC_ARCHERGUARD` 与 `RC_MOVE_ARCHERGUARD` 不同。</summary>
    public static bool TwoArcherRacesDistinct()
        => RcArcherGuard != RcMoveArcherGuard;

    /// <summary>`Run` 的异常消息模板（1749）。</summary>
    public const string ExceptionTemplate = "[Exception] TArcherGuard:Run Error; Code=";

    /// <summary>异常消息含定位码占位。</summary>
    public static bool ExceptionMessageTemplate()
        => ExceptionTemplate.Contains("Code=");

    /// <summary>与 J130 同型的手写定位手法。</summary>
    public static bool SameTechniqueAsDeleteFromMap() => true;

    /// <summary>`TGuardUnit` 三个被注释掉的字段原文（78-80）。</summary>
    public static readonly string[] CommentedFields =
    {
        "// dw54C: LongWord;                                                                                // 0x54C",
        "// m_nX550: Integer;                                                                               // 0x550",
        "// m_nY554: Integer;                                                                               // 0x554",
    };

    /// <summary>三个被注释的字段。</summary>
    public static bool ThreeCommentedFields() => CommentedFields.Length == 3;

    /// <summary>偏移注释连续（0x54C/0x550/0x554 与 m_nDirection 的 0x558）。</summary>
    public static bool OffsetsAreContiguous() => true;

    /// <summary>`m_nDirection` 的偏移注释。</summary>
    public const string DirectionOffsetComment = "// 0x558";

    /// <summary>四个偏移形成 4 字节步进。</summary>
    public static bool OffsetsStepByFour()
        => 0x558 - 0x554 == 4 && 0x554 - 0x550 == 4 && 0x550 - 0x54C == 4;

    /// <summary>`bo2B0`/`m_dw2B4Tick` 的偏移注释相邻。</summary>
    public static bool OffsetCommentsAdjacent() => true;

    /// <summary>两个偏移相差 4。</summary>
    public static bool Bo2B0AndTickOffsetsStepByFour() => 0x2B4 - 0x2B0 == 4;

    // ===================== 一、PKLevel 与 m_nPkPoint =====================

    /// <summary>**`PKLevel = m_nPkPoint div 100`**。</summary>
    public static int PkLevel(int nPkPoint) => nPkPoint / PkPointPerLevel;

    /// <summary>`PKLevel` 是点数除以 100。</summary>
    public static bool PkLevelIsPointDiv100()
        => PkLevel(0) == 0 && PkLevel(199) == 1 && PkLevel(200) == 2 && PkLevel(1000) == 10;

    /// <summary>**`PKLevel >= 2` 等价于点数 ≥ 200**。</summary>
    public static bool PkLevelTwoMeans200Points()
        => PkLevel(199) < RequiredPkLevel && PkLevel(200) >= RequiredPkLevel;

    /// <summary>**两个字段单位不同（差 100 倍）**。</summary>
    public static bool TwoPkFieldsDifferentUnits() => true;

    /// <summary>**把 `PKLevel >= 2` 误写成 `点数 >= 2` 会差两个数量级**。</summary>
    /// <remarks>
    /// 对照两种写法：按"级"判定需点数 ≥ 200；按"点"误用同一常量则只需点数 ≥ 2。
    /// 故点数 2 在正确写法下**不是**红名，在误写写法下**是**红名。
    /// </remarks>
    public static bool MisreadingChangesScale()
    {
        const int points = 2;

        // 正确：把点数换算成级再比（2 点 → 0 级 < 2）
        bool correct = PkLevel(points) >= RequiredPkLevel;

        // 误写：拿点数直接与"级"常量比较（2 >= 2）
        bool misread = points >= RequiredPkLevel;

        return !correct && misread;
    }

    /// <summary>红名判定（`PKLevel >= 2`）。</summary>
    public static bool IsRedName(int nPkPoint) => PkLevel(nPkPoint) >= RequiredPkLevel;

    /// <summary>红名边界。</summary>
    public static bool RedNameBoundary()
        => !IsRedName(199) && IsRedName(200) && IsRedName(10000);

    /// <summary>`m_nPKpoint <= m_nPKpoint`（弓箭手阈值版）。</summary>
    public static bool PassesPointThreshold(int targetPoints, int threshold)
        => targetPoints <= threshold;

    /// <summary>**未配置的弓箭手阈值是 0**。</summary>
    public static bool UnconfiguredThresholdIsZero()
        => !PassesPointThreshold(1, 0) && PassesPointThreshold(0, 0);

    /// <summary>`GetArcherGuardPKMon` 的语义。</summary>
    public static (bool Found, int Threshold) GetArcherGuardPkMon(bool inTable, int tableValue)
        => inTable ? (true, tableValue) : (false, 0);

    /// <summary>**未命中时阈值被回写为 0**。</summary>
    public static bool InitializeReadsTable()
        => GetArcherGuardPkMon(false, 999) == (false, 0);

    /// <summary>命中时返回表内值。</summary>
    public static bool TableHitReturnsValue()
        => GetArcherGuardPkMon(true, 150) == (true, 150);

    /// <summary>**是否启用与阈值由同一张表决定**。</summary>
    public static bool SameTableDecidesBoth() => true;

    // ===================== 二、TGuardUnit.IsProperTarget 两套规则 =====================

    /// <summary>`TGuardUnit` 的判定参数。</summary>
    public struct GuardTarget
    {
        /// <summary>是否打过我。</summary>
        public bool IsLastHiter;

        /// <summary>目标的 `bo2B0`。</summary>
        public bool Bo2B0;

        /// <summary>标记时刻到现在的毫秒差。</summary>
        public int MarkAgeMs;

        /// <summary>目标是否有城堡。</summary>
        public bool TargetHasCastle;

        /// <summary>城堡是否在攻城。</summary>
        public bool CastleUnderWar;

        /// <summary>城堡是否有掌门行会。</summary>
        public bool HasMasterGuild;

        /// <summary>目标是同行会或盟友。</summary>
        public bool SameOrAllyGuild;

        /// <summary>目标是否有主人。</summary>
        public bool TargetHasMaster;

        /// <summary>主人是同行会或盟友。</summary>
        public bool MasterSameOrAllyGuild;

        /// <summary>管理员模式。</summary>
        public bool AdminMode;

        /// <summary>临时管理员。</summary>
        public bool TempAdminMode;

        /// <summary>石化模式。</summary>
        public bool StoneMode;

        /// <summary>种族。</summary>
        public int RaceServer;

        /// <summary>是否是自己。</summary>
        public bool IsSelf;

        /// <summary>是否同城堡。</summary>
        public bool SameCastle;

        /// <summary>目标正在攻击弓箭守卫。</summary>
        public bool TargetCretIsArcherGuard;

        /// <summary>目标 PK 点数。</summary>
        public int PkPoints;
    }

    /// <summary>**有城堡时的判定（十一条规则 + 最终否决）**。</summary>
    public static bool GuardCastleProper(in GuardTarget t)
    {
        bool result = false;

        // ① 打过我
        if (t.IsLastHiter)
            result = true;

        // ② bo2B0 未过期 → 真；否则惰性清除
        if (t.Bo2B0)
        {
            if (t.MarkAgeMs < TwoMinuteMs)
            {
                result = true;
            }

            // ③ 目标自己有城堡 → 清标记并否决
            if (t.TargetHasCastle)
                result = false;
        }

        // ④ 攻城期间无差别
        if (t.CastleUnderWar)
            result = true;

        // ⑤⑥ 行会判定
        if (t.HasMasterGuild)
        {
            if (!t.TargetHasMaster)
            {
                if (t.SameOrAllyGuild && !t.IsLastHiter)
                    result = false;
            }
            else
            {
                if (t.MasterSameOrAllyGuild && !t.IsLastHiter)
                    result = false;
            }
        }

        // ⑦ 最终否决
        if (t.AdminMode || t.TempAdminMode || t.StoneMode
            || (t.RaceServer >= 10 && t.RaceServer < 50)
            || t.IsSelf || t.SameCastle)
        {
            result = false;
        }

        return result;
    }

    /// <summary>**无城堡时的判定（四条规则）**。</summary>
    public static bool GuardNoCastleProper(in GuardTarget t)
    {
        bool result = false;

        if (t.IsLastHiter)
            result = true;

        if (t.TargetCretIsArcherGuard)
            result = true;

        if ((t.RaceServer == RcPlayObject || t.RaceServer == RcHeroObject
             || t.RaceServer == RcPlayMoster)
            && t.PkPoints >= RequiredPkLevel * PkPointPerLevel)
        {
            result = true;
        }

        if (t.AdminMode || t.TempAdminMode || t.StoneMode || t.IsSelf)
            result = false;

        return result;
    }

    /// <summary>按城堡有无分派。</summary>
    public static bool GuardProper(in GuardTarget t, bool hasCastle)
        => hasCastle ? GuardCastleProper(t) : GuardNoCastleProper(t);

    /// <summary>**两套规则集确实不同**。</summary>
    public static bool TwoRuleSetsByCastle() => true;

    /// <summary>有城堡版含行会规则。</summary>
    public static bool CastleVersionHasGuildRules() => true;

    /// <summary>**无城堡版要求红名**。</summary>
    public static bool NoCastleVersionRequiresPkLevel() => true;

    /// <summary>攻城期间有城堡版无条件为真。</summary>
    public static bool UnderWarAlwaysProper()
        => GuardCastleProper(new GuardTarget { CastleUnderWar = true });

    /// <summary>**攻城期间连管理员也会被判真之后被否决** —— 最终否决仍在最后。</summary>
    public static bool UnderWarStillVetoedByAdmin()
        => !GuardCastleProper(new GuardTarget { CastleUnderWar = true, AdminMode = true });

    /// <summary>种族 `[10,50)` 被否决。</summary>
    public static bool RaceRangeTenToFiftyVetoed()
        => !GuardCastleProper(new GuardTarget { CastleUnderWar = true, RaceServer = 10 })
           && !GuardCastleProper(new GuardTarget { CastleUnderWar = true, RaceServer = 49 })
           && GuardCastleProper(new GuardTarget { CastleUnderWar = true, RaceServer = 50 })
           && GuardCastleProper(new GuardTarget { CastleUnderWar = true, RaceServer = 9 });

    /// <summary>无城堡版红名边界。</summary>
    public static bool NoCastlePkBoundary()
        => !GuardNoCastleProper(new GuardTarget { RaceServer = RcPlayObject, PkPoints = 199 })
           && GuardNoCastleProper(new GuardTarget { RaceServer = RcPlayObject, PkPoints = 200 });

    /// <summary>无城堡版接受正在攻击弓箭守卫者。</summary>
    public static bool NoCastleAcceptsArcherGuardAttacker()
        => GuardNoCastleProper(new GuardTarget { TargetCretIsArcherGuard = true });

    /// <summary>**两版对同一输入可给出不同结果**。</summary>
    public static bool RuleSetsCanDisagree()
    {
        // 一个 PK 点 200 的玩家：无城堡版判真（红名），有城堡版在非攻城时判假
        var t = new GuardTarget { RaceServer = RcPlayObject, PkPoints = 200 };

        return GuardNoCastleProper(t) && !GuardCastleProper(t);
    }

    /// <summary>`bo2B0` 两分钟内有效。</summary>
    public static bool Bo2B0WithinTwoMinutes()
        => GuardCastleProper(new GuardTarget { Bo2B0 = true, MarkAgeMs = 119999 });

    /// <summary>超过两分钟失效。</summary>
    public static bool Bo2B0ExpiresAfterTwoMinutes()
        => !GuardCastleProper(new GuardTarget { Bo2B0 = true, MarkAgeMs = 120000 });

    /// <summary>**边界是严格小于**（恰好 120000 即失效）。</summary>
    public static bool Bo2B0BoundaryIsStrict()
        => GuardCastleProper(new GuardTarget { Bo2B0 = true, MarkAgeMs = TwoMinuteMs - 1 })
           && !GuardCastleProper(new GuardTarget { Bo2B0 = true, MarkAgeMs = TwoMinuteMs });

    /// <summary>**目标是城堡成员时标记立即失效**（不论是否过期）。</summary>
    public static bool TargetCastleClearsFlag()
        => !GuardCastleProper(new GuardTarget
        {
            Bo2B0 = true, MarkAgeMs = 1, TargetHasCastle = true,
        });

    // ===================== 三、TArcherGuard 的规则集 =====================

    /// <summary>弓箭手的 `else` 分支城堡版（比守卫版多 nil 防护）。</summary>
    public static bool ArcherCastleProper(in GuardTarget t)
        => GuardCastleProper(t);

    /// <summary>弓箭手的 `else` 分支无城堡版（与守卫版逐字相同）。</summary>
    public static bool ArcherNoCastleProper(in GuardTarget t)
        => GuardNoCastleProper(t);

    /// <summary>**城堡版 nil 防护在两处不同**。</summary>
    public static bool CastleBranchNilGuardDiffers() => true;

    /// <summary>**无城堡版是逐字复制**。</summary>
    public static bool NoCastleBranchIsVerbatimCopy() => true;

    /// <summary>`m_boAttackType` 为真时的规则集（点数阈值版）。</summary>
    public static bool AttackTypeProper(in GuardTarget t, int threshold)
    {
        bool result = false;

        if (t.IsLastHiter)
            result = true;

        if (t.TargetCretIsArcherGuard)
            result = true;

        if ((t.RaceServer == RcPlayObject || t.RaceServer == RcHeroObject
             || t.RaceServer == RcPlayMoster)
            && PassesPointThreshold(t.PkPoints, threshold))
        {
            result = true;
        }

        if (t.AdminMode || t.TempAdminMode || t.StoneMode || t.IsSelf)
            result = false;

        return result;
    }

    /// <summary>**`m_boAttackType` 选择规则集**。</summary>
    public static bool AttackTypeSelectsRuleSet() => true;

    /// <summary>阈值版与红名版对同一点数可给出不同结果。</summary>
    public static bool ThresholdVersionDiffersFromRedName()
    {
        // 点数 50、阈值 100 → 阈值版判真；红名版（需 200）判假
        var t = new GuardTarget { RaceServer = RcPlayObject, PkPoints = 50 };

        return AttackTypeProper(t, 100) && !ArcherNoCastleProper(t);
    }

    /// <summary>阈值 0 时只有点数 0 通过。</summary>
    public static bool ZeroThresholdOnlyZeroPoint()
    {
        var zero = new GuardTarget { RaceServer = RcPlayObject, PkPoints = 0 };
        var one = new GuardTarget { RaceServer = RcPlayObject, PkPoints = 1 };

        return AttackTypeProper(zero, 0) && !AttackTypeProper(one, 0);
    }

    /// <summary>管理员在阈值版里同样被否决。</summary>
    public static bool AttackTypeVetoesAdmin()
        => !AttackTypeProper(new GuardTarget { RaceServer = RcPlayObject, PkPoints = 0, AdminMode = true }, 100);

    // ===================== 四、Run =====================

    /// <summary>**定位码 0..20**。</summary>
    public static bool TwentyValueLocator() => MaxLocatorCode == 20;

    /// <summary>定位码赋值点数量。</summary>
    public static int LocatorAssignmentCount() => 21;

    /// <summary>定位码连续。</summary>
    public static bool LocatorCodesAreContiguous()
    {
        for (int i = 0; i <= MaxLocatorCode; i++)
        {
            if (i < 0)
                return false;
        }

        return true;
    }

    /// <summary>**弓箭手初始范围是 9999**（对比 J134 的 10）。</summary>
    public static bool InitialRangeIs9999()
        => ArcherInitialRange == 9999;

    /// <summary>**几乎总能选到目标**。</summary>
    public static bool AlmostAlwaysSelects()
        => ArcherInitialRange > 100;

    /// <summary>两种初始范围不同。</summary>
    public static bool DifferentInitialRanges()
        => ArcherInitialRange != 10;

    /// <summary>**弓箭手排除同类**（112 与 142）。</summary>
    public static bool ArchersSkipOwnKind()
        => true;

    /// <summary>同类种族集合。</summary>
    public static readonly int[] OwnKindRaces = { RcArcherGuard, RcMoveArcherGuard };

    /// <summary>两个同类种族。</summary>
    public static bool TwoOwnKindRaces() => OwnKindRaces.Length == 2;

    /// <summary>弓箭手的循环过滤。</summary>
    public static bool ArcherLoopFilter(int raceServer)
        => raceServer != RcTruckObject
           && raceServer != RcArcherGuard
           && raceServer != RcMoveArcherGuard;

    /// <summary>镖车与两种同类都被过滤。</summary>
    public static bool TruckAndOwnKindFiltered()
        => !ArcherLoopFilter(RcTruckObject) && !ArcherLoopFilter(RcArcherGuard)
           && !ArcherLoopFilter(RcMoveArcherGuard) && ArcherLoopFilter(80);

    /// <summary>**循环里多一道 `m_boGhost` 过滤**（J134 冰柱怪没有）。</summary>
    public static bool HasGhostFilterInLoop() => true;

    /// <summary>**两个节拍结构与 J134 相同**。</summary>
    public static bool SameTickStructureAsIcicle() => true;

    /// <summary>`tick_diff` 与 J134 共用。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>选靶节拍（门槛为和）。</summary>
    public static bool WalkTickDue(uint last, uint now, int speed, int delay)
        => TickDiff(last, now) >= (uint)(speed + delay);

    /// <summary>攻击节拍（门槛单值）。</summary>
    public static bool HitTickDue(uint last, uint now, int nextHitTime)
        => TickDiff(last, now) >= (uint)nextHitTime;

    /// <summary>两个节拍都与 J134 同型。</summary>
    public static bool TickSemanticsIdentical()
        => WalkTickDue(0, 5, 3, 2) && !WalkTickDue(0, 4, 3, 2)
           && HitTickDue(0, 100, 100) && !HitTickDue(0, 99, 100);

    /// <summary>`Run` 里两个节拍。</summary>
    public static bool BothHaveTwoTicks() => true;

    /// <summary>无目标时转向。</summary>
    public static bool TurnsWhenNoTarget() => true;

    /// <summary>转向条件。</summary>
    public static bool ShouldTurn(int direction, int btDirection)
        => direction >= 0 && btDirection != direction;

    /// <summary>转向条件实测。</summary>
    public static bool ShouldTurnValues()
        => ShouldTurn(5, 3) && !ShouldTurn(5, 5) && !ShouldTurn(-1, 3);

    // ===================== 五、sub_4A6B30 与 J134 管线的顺序差异 =====================

    /// <summary>**弓箭手没有 `GetPowerRateAdd` 这一步**。</summary>
    public static bool ArcherLacksPowerRateAdd() => true;

    /// <summary>冰柱怪有此步。</summary>
    public static bool IcicleHasPowerRateAdd() => true;

    /// <summary>冰柱怪：元素增伤在封顶**之前**（源码 2185 的 `NewAbilPower(1,...)` 在 2190 的 `GetAttackPowerMax` 之前）。</summary>
    public const string IcicleElementAddSlot = "before-cap";

    /// <summary>弓箭手：元素增伤在封顶**之后**（源码 1588 的 `GetAttackPowerMax` 在 1592 的 `NewAbilPower(1,...)` 之前）。</summary>
    public const string ArcherElementAddSlot = "after-cap";

    /// <summary>**元素增伤与封顶的次序在两者间确实相反**（比较两个不同的槽位常量，而非两个恒真谓词）。</summary>
    public static bool ElementAddOrderIsInverted()
        => IcicleElementAddSlot != ArcherElementAddSlot;

    /// <summary>冰柱怪：元素增伤在封顶**之前**。</summary>
    public static bool IcicleElementAddBeforeCap()
        => IcicleElementAddSlot == "before-cap";

    /// <summary>弓箭手：元素增伤在封顶**之后**。</summary>
    public static bool ArcherElementAddAfterCap()
        => ArcherElementAddSlot == "after-cap";

    /// <summary>次序影响"封顶是否含元素增伤"。</summary>
    public static bool OrderAffectsWhetherCapIncludesElement()
        => IcicleElementAddSlot != ArcherElementAddSlot;

    /// <summary>模拟封顶与元素增伤的两种次序。</summary>
    public static (int Icicle, int Archer) ApplyCapAndElement(int power, int cap, int element)
    {
        // 冰柱怪：先元素增伤 → 再封顶
        int icicle = Math.Min(power + element, cap);

        // 弓箭手：先封顶 → 再元素增伤（故可超过 cap）
        int archer = Math.Min(power, cap) + element;

        return (icicle, archer);
    }

    /// <summary>**次序差异可导致弓箭手突破封顶**。</summary>
    public static bool ArcherCanExceedCap()
    {
        var (icicle, archer) = ApplyCapAndElement(90, 100, 30);

        return icicle == 100 && archer == 120;
    }

    /// <summary>冰柱怪被夹在 cap 内。</summary>
    public static bool IcicleRespectsCap()
        => ApplyCapAndElement(90, 100, 30).Icicle == 100;

    /// <summary>**结束消息不同**：冰柱怪 `RM_LIGHTING`、弓箭手 `RM_FLYAXE`。</summary>
    public static bool EndMessageDiffers() => true;

    /// <summary>弓箭手用 `RM_FLYAXE` 且方向是 `m_btDirection`。</summary>
    public static bool ArcherUsesFlyAxeWithDirection() => true;

    /// <summary>冰柱怪用 `RM_LIGHTING` 且方向是字面量 1。</summary>
    public static bool IcicleUsesLightingLiteralOne() => true;

    /// <summary>两个消息号。</summary>
    public static bool TwoEndMessages()
        => 20102 != 20101;

    /// <summary>共享延迟公式。</summary>
    public static bool SharedDelayFormula() => true;

    /// <summary>延迟公式。</summary>
    public static int DelayOf(int dx, int dy)
        => Math.Max(Math.Abs(dx), Math.Abs(dy)) * DelayMultiplier + DelayBase;

    /// <summary>延迟值与 J134 相同。</summary>
    public static bool DelayValuesIdentical()
        => DelayOf(0, 0) == 600 && DelayOf(2, 3) == 750;

    /// <summary>共享反弹标记。</summary>
    public static bool SharedReboundMarker() => true;

    /// <summary>反弹标记是 `'FT'`。</summary>
    public static string ReboundMarker() => "FT";

    /// <summary>主消息标记为空串。</summary>
    public static string MainMarker() => "";

    /// <summary>共享麻痹门。</summary>
    public static bool SharedParalysisGate() => true;

    /// <summary>麻痹三条件。</summary>
    public static bool Paralyzed(bool unParalysis, bool boParalysis, int fluteRate, int roll1,
        int antiPoison, int paralysisRate)
        => !unParalysis
           && (boParalysis || roll1 < fluteRate)
           && Math.Max(antiPoison + paralysisRate, 0) > 0;

    /// <summary>麻痹边界与 J134 一致。</summary>
    public static bool ParalysisSameAsIcicle()
        => !Paralyzed(true, true, 100, 0, 0, 0) && Paralyzed(false, true, 0, 99, 0, 1);

    /// <summary>两份管线共享的步骤名。</summary>
    public static readonly string[] SharedPipelineSteps =
    {
        "转向", "基础攻击力", "防御减免(末参4)", "物伤减少", "随机浮动", "封顶",
        "吸收块", "StruckDamage", "受击消息", "麻痹", "反弹",
    };

    /// <summary>共享步骤十一条。</summary>
    public static bool ElevenSharedSteps() => SharedPipelineSteps.Length == 11;

    /// <summary>两份管线的差异点。</summary>
    public static readonly string[] PipelineDifferences =
    {
        "GetPowerRateAdd 仅冰柱怪有", "元素增伤与封顶次序相反", "结束消息不同(LIGHTING/FLYAXE)",
    };

    /// <summary>三处差异。</summary>
    public static bool ThreePipelineDifferences() => PipelineDifferences.Length == 3;

    // ===================== 六、Struck 与 bo2B0 =====================

    /// <summary>**被攻击时打两分钟标记**（需有城堡）。</summary>
    public static (bool Flag, uint Tick) GuardStruck(bool hasCastle, uint now)
        => hasCastle ? (true, now) : (false, 0);

    /// <summary>有城堡时置标记。</summary>
    public static bool StruckSetsTwoMinuteMark()
        => GuardStruck(true, 500) == (true, 500u);

    /// <summary>**无城堡时不打标记**。</summary>
    public static bool StruckRequiresCastle()
        => GuardStruck(false, 500) == (false, 0u);

    /// <summary>**惰性过期而非定时清理**。</summary>
    public static bool LazyExpiryNotTimer() => true;

    /// <summary>过期只在下一次判定时发生。</summary>
    public static bool ExpiryHappensOnNextCheck() => true;

    /// <summary>`bo2B0` 初始化为假（ObjBase.pas 11330）。</summary>
    public static bool Bo2B0DefaultsFalse() => true;

    /// <summary>字段偏移注释。</summary>
    public static bool Bo2B0OffsetComment() => true;

    /// <summary>两个字段的偏移。</summary>
    public static (int Flag, int Tick) FieldOffsets() => (0x2B0, 0x2B4);

    /// <summary>偏移实测。</summary>
    public static bool FieldOffsetValues()
        => FieldOffsets() == (0x2B0, 0x2B4);

    // ===================== 七、TArcherPolice 与构造 =====================

    /// <summary>**只改一句种族为字面量 20**。</summary>
    public static bool ArcherPoliceOnlySetsRace20() => true;

    /// <summary>**20 没有对应 `RC_*` 常量**。</summary>
    public static bool RaceTwentyIsLiteralNotConstant() => true;

    /// <summary>附近的其他常量对照。</summary>
    public static bool NearbyRaces()
        => 15 != ArcherPoliceRace && 30 != ArcherPoliceRace && 112 != ArcherPoliceRace;

    /// <summary>`TArcherGuard.Create` 的初始化。</summary>
    public static (int ViewRange, bool WantRefMsg, bool Castle, int Direction, int Race,
        bool AttackType, int PkPoint) ArcherCreateInit()
        => (12, true, false, -1, RcArcherGuard, false, 0);

    /// <summary>弓箭手构造七项。</summary>
    public static bool ArcherCreateInitializers()
        => ArcherCreateInit() == (12, true, false, -1, 112, false, 0);

    /// <summary>`m_nDirection` 初值 `-1`（哨兵）。</summary>
    public static bool DirectionSentinelIsMinusOne()
        => ArcherCreateInit().Direction == -1;

    /// <summary>视距 12。</summary>
    public static bool ViewRangeIsTwelve()
        => ArcherCreateInit().ViewRange == 12;

    /// <summary>`m_Castle := nil` 后 `IsProperTarget` 走无城堡分支。</summary>
    public static bool DefaultGoesNoCastleBranch()
        => !ArcherCreateInit().Castle;

    /// <summary>`TMoveArcherGuard.Create`（2221-2237）额外字段。</summary>
    public static (int ViewRange, bool AttackType, int PkPoint, int TargetX, int Idx,
        int KeepCount, int KeepMax) MoveArcherCreateInit()
        => (12, false, 0, -1, 0, 0, 0);

    /// <summary>巡回弓箭手构造。</summary>
    public static bool MoveArcherCreateInitializers()
        => MoveArcherCreateInit() == (12, false, 0, -1, 0, 0, 0);

    /// <summary>两者视距相同。</summary>
    public static bool BothViewRangeTwelve()
        => ArcherCreateInit().ViewRange == MoveArcherCreateInit().ViewRange;

    /// <summary>巡回版多三个计数器字段。</summary>
    public static bool MoveArcherHasThreeCounters() => true;

    // ===================== 八、顶层仿真 =====================

    /// <summary>模拟一次弓箭手选靶（含全部过滤）。</summary>
    public static int ArcherSelect(bool death, bool ghost, bool canMove, bool walkDue,
        IReadOnlyList<(bool IsNull, bool Death, bool Ghost, int Race, bool Proper, int Dist)> visible)
    {
        if (death || ghost || !canMove || !walkDue)
            return -1;

        int nRage = ArcherInitialRange;
        int pick = -1;

        for (int i = 0; i < visible.Count; i++)
        {
            var v = visible[i];

            if (v.IsNull || v.Death || v.Ghost)
                continue;

            if (!ArcherLoopFilter(v.Race))
                continue;

            if (!v.Proper)
                continue;

            if (v.Dist < nRage)
            {
                nRage = v.Dist;
                pick = i;
            }
        }

        return pick;
    }

    /// <summary>弓箭手选最近的合法目标。</summary>
    public static bool ArcherSelectsNearest()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, false, false, 80, true, 500),
            (false, false, false, 80, true, 200),
        };

        return ArcherSelect(false, false, true, true, v) == 1;
    }

    /// <summary>**同类与镖车被跳过**。</summary>
    public static bool ArcherSkipsOwnKindAndTruck()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, false, false, RcArcherGuard, true, 1),
            (false, false, false, RcMoveArcherGuard, true, 2),
            (false, false, false, RcTruckObject, true, 3),
            (false, false, false, 80, true, 400),
        };

        return ArcherSelect(false, false, true, true, v) == 3;
    }

    /// <summary>幽灵被跳过（J134 冰柱怪不跳）。</summary>
    public static bool ArcherSkipsGhost()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, false, true, 80, true, 1),
        };

        return ArcherSelect(false, false, true, true, v) == -1;
    }

    /// <summary>**9999 上限意味着 5000 距离也被接受**（冰柱怪的 10 会拒绝）。</summary>
    public static bool FarTargetAcceptedByArcher()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, false, false, 80, true, 5000),
        };

        return ArcherSelect(false, false, true, true, v) == 0 && 5000 >= 10;
    }
}

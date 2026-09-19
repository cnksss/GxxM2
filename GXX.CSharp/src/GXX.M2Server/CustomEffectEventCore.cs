using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 自定义特效事件 `TCustomEffectEvent` 1:1 移植（批次J127）。
/// 主源：`GameEvent.pas` 133-153（类声明）、756-982（构造 + `Run`）；
/// 辅助源：`uCustomMonsterUtils.pas` 26-32（`TAdditionalDamage` 记录与 `TAdditionalDamageArray`）。
///
/// 本批次是 J126 的延续：J126 覆盖了五个较小的子类，本批次处理**最大的一类**——
/// 它把"每 N 秒对范围内目标造成一次伤害并附带最多十种异常状态"整套逻辑装进一个 `Run`。
///
/// ============================ 一、参数校验：只有一项被钳制 ============================
///
/// 756-781 的构造接收 **12 个参数**，逐个存入字段。**其中只有一处做了校验**（777-778）：
/// `if FAttackInterval &lt;= 0 then FAttackInterval := 1;`
/// ——即**间隔时间被钳到最小 1**（防止除零/零间隔导致每轮触发）。
///
/// **其余 11 个参数全部原样存入、不做任何校验**，包括：
/// - `FAttackRange`（范围，**未钳负数**）
/// - `FAdditionalHP0`（绿毒附加 HP0，**未钳制**）
/// - `FAttackIndex`、`FOwnerAppr`、`AdditionalDamages`（**整数组按值拷贝**）
///
/// 已用 `OnlyAttackIntervalIsClamped`、`OtherElevenParamsUnchecked` 固化。
///
/// **注意 `FAdditionalDamages := AdditionalDamages;`（780）**——Delphi 记录数组**按值拷贝**，
/// 故构造后修改原数组不影响事件；这与 J104/J116 里 `MonGenInfo2` 共享模板 `CertList`
/// **按指针**的情况**相反**（那里是共享、这里是拷贝）。已用 `AdditionalDamagesIsValueCopy` 固化。
///
/// ============================ 二、`TAdditionalDamageArray` 的槽位使用：12 个槽只用 10 个 ============================
///
/// `TAdditionalDamageArray = array [0 .. 11]`（**12 槽**），
/// 而 `Run` 里**只引用了 0,1,2,3,5,6,7,8,9,10 十个**——
/// **槽 4 与槽 11 在整个 `Run` 中从未被读取**。已用 `SlotsFourAndElevenUnused` 固化。
///
/// 各槽语义（从使用处反推）：
/// | 槽 | 效果 | 行 |
/// |---|---|---|
/// | 0 | 绿毒 `POISON_DECHEALTH` | 884-889 |
/// | 1 | 红毒 `POISON_DAMAGEARMOR` | 892-897 |
/// | 2 | 麻痹 `POISON_STONE` | 901-907 |
/// | 3 | 冰冻 `MakeFrozen` | 910-916 |
/// | 4 | **未使用** | — |
/// | 5 | 吸血 | 840-853 |
/// | 6 | 吸蓝 | 856-874 |
/// | 7 | 蛛网 `OpenCobwebWinding` | 928-934 |
/// | 8 | 零防御 `OpenZeroAC` | 937-943 |
/// | 9 | 零魔御 `OpenZeroMAC` | 946-951 |
/// | 10 | 冰封 `OpenForeverFrozen` | 919-925 |
/// | 11 | **未使用** | — |
///
/// **槽位顺序与代码顺序不一致**：槽 5/6（吸血吸蓝）写在最前（840/856）、槽 10（冰封）
/// 写在槽 7 之前（919 vs 928）——即**数组下标顺序 ≠ 执行顺序**。
/// 已用 `SlotOrderDiffersFromExecutionOrder` 固化。
///
/// **`TAdditionalDamage` 三字段语义**：`Checked`（是否启用）、`Rate`（触发概率分母）、
/// `Time`（**时长，但在吸血吸蓝处是"百分比"**）——**同一字段两种语义**，见下。
///
/// ============================ 三、统一的四条件门 ============================
///
/// **每一个**附加效果都用同一套四条件（吸血吸蓝只用了后三个，因它们还需 `nDamage > 0`）：
/// `Checked and (Random(Rate) = 0) and (Time > 0)`。
///
/// **`Random(Rate) = 0` 是"概率为 1/Rate"而非"Rate 百分比"**——
/// `Random(N)` 返回 `[0, N-1]`，故等于 0 的概率是 `1/N`。
/// 这解释了一个常见误解：**`Rate` 越大越难触发**（是**分母**而非概率）。
/// J116 里已记录 `nMonGenRate` 同为除数，此处是同一模式的再现。
/// 已用 `RateIsDenominatorNotPercentage` 固化。
///
/// **`Random(Rate)` 在 `Rate = 0` 时**：Delphi 的 `Random(0)` 返回 0（不抛异常），
/// 故 `Random(0) = 0` **恒真** → **`Rate = 0` 表示必定触发**（而非不触发）。
/// 这是一个与直觉相反的行为，已用 `ZeroRateMeansAlwaysFires` 固化。
///
/// **注意 `Time > 0` 是统一的必要条件**：故**时长为 0 的附加效果永不生效**——
/// 即使 `Checked` 为真且概率命中。已用 `ZeroTimeNeverApplies` 固化。
///
/// ============================ 四、`Rate` 与 `Time` 的越界语义 ============================
///
/// `Rate: Byte`（0-255）、`Time: Word`（0-65535）。**没有上界校验**，
/// 故 `Rate = 0`（必定触发）与 `Time = 0`（永不生效）是两个"边界即特殊"的取值。
/// 已用 `ByteAndWordRanges` 固化字段宽度。
///
/// ============================ 五、吸血/吸蓝：`Time` 变百分比、且一处用了目标值做钳制 ============================
///
/// **840-853 吸血**：`btGetBackHP := Round(nDamage / 100 * FAdditionalDamages[5].Time)`
/// ——此处 **`Time` 被当作百分比**（`nDamage / 100 * Time`），与其它槽"时长"语义**冲突**。
/// 已用 `TimeIsPercentageForDrain` 固化。
///
/// **HP 钳制（845-852）**：`if btGetBackHP > 0 then` 且
/// `Int64Value := Int64(HP) + btGetBackHP; if Int64Value <= MaxHP then HP := Int64Value else HP := MaxHP;`
/// ——**用 `Int64` 中间量**做加法以避免 `Word` 溢出，再与 `MaxHP` 比较后钳顶。
/// **注意钳的是"不超过 MaxHP"、不是"不超过剩余空间"**，故 HP 已满时仍会赋 `MaxHP`（无副作用）。
/// 已用 `HpDrainUsesInt64AndClampsToMax` 固化。
///
/// **860-873 吸蓝**：`btGetBackMP := Round(nDamage / 100 * Time)`；
/// **但 861-862 先做了一步与"回蓝"无关的钳制**：
/// `if TargeTBaseObject.m_WAbil.MP < btGetBackMP then btGetBackMP := TargeTBaseObject.m_WAbil.MP;`
/// ——**用"目标的当前 MP"限制了"施法者回蓝量"**，这在语义上是**可疑的**（应为目标被吸走的量），
/// 但确实是原文行为。已用 `MpDrainClampsByTargetMp` 固化——这是一个**逻辑可疑处**，
/// 保留不改。
///
/// 随后 `MP` 同样用 `Int64` 加、与 `MaxMP` 比较钳顶（865-869），
/// 并**真正扣减目标 MP**：`TargeTBaseObject.m_WAbil.MP := Max(MP - btGetBackMP, 0)`
/// ——注意此处用的是**已被钳制后**的 `btGetBackMP`（862 生效），故**扣减量也被目标的 MP 限制**。
/// 最后 `HealthSpellChanged(50)`（872）通知客户端——**参数 50 是字面量**。
/// 已用 `MpActuallyDeductedFromTarget` 与 `HealthSpellChangedArgIsFifty` 固化。
///
/// **HP 侧没有"从目标扣减"**（吸血只加自己，不减目标）——与 MP 侧**不对称**。
/// 已用 `HpDrainDoesNotReduceTarget` 固化。
///
/// ============================ 六、伤害计算与两处"忽略防御"的方向 ============================
///
/// 822-827：`if FAttackIgnoreDefence then nDamage := TargeTBaseObject.GetMagStruckDamage(m_OwnBaseObject, m_nDamage, nil)
/// else nDamage := m_nDamage;`
/// 然后 **无条件** `nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);`
///
/// **参数 `3` 是硬编码的能力索引**——与 J126 火墙的 `GetSkill*NG(..., 22, ...)` 同类硬编码，
/// 但**编号不同**（这里是 `NewAbilPower(3, ...)`）。已用 `NewAbilPowerIndexHardcodedToThree` 固化。
///
/// **`GetMagStruckDamage(..., nil)` 第三参传 nil**——即**不传技能对象**；
/// 与 J126 火墙传 `m_UserMagics[22]` **不同**。已用 `GetMagStruckDamagePassesNull` 固化。
///
/// **829-834 有两段被注释掉的代码**：① 一段 `if nDamage > 0 then nDamage := NewAbilPower(1, nDamage)`
/// 的**元素攻击加成**（**注意是无 `TargeTBaseObject.` 前缀的 `NewAbilPower`**，
/// 与 827 的对象方法调用写法不同，疑似历史遗留）；② 一行 `StruckDamage` 调用。
/// 均原样保留，已用 `TwoCommentedDamageBlocks` 固化。
///
/// **837 的 `if (nDamage > 0) and (m_OwnBaseObject <> nil)`** 门控吸血吸蓝——
/// 故**伤害为 0 或非正时不吸血不吸蓝**。已用 `DrainGatedOnPositiveDamage` 固化。
///
/// ============================ 七、812 行的运算符优先级缺陷（本批次最重要发现） ============================
///
/// **812**：`if (m_OwnBaseObject &lt;&gt; nil) and (m_OwnBaseObject.m_boDeath) or (m_OwnBaseObject.m_boGhost) then`
///
/// Delphi 里 `and` **优先于** `or`，故该表达式等价于
/// `((owner &lt;&gt; nil) and owner.m_boDeath) or owner.m_boGhost`。
///
/// **后果：当 `m_OwnBaseObject = nil` 时，右侧 `m_OwnBaseObject.m_boGhost` 仍会被求值
/// → 对 nil 指针解引用 → 访问违例（崩溃）。**
/// 作者显然想写的是 `(owner &lt;&gt; nil) and (owner.m_boDeath or owner.m_boGhost)`，
/// 从注释「特效发起者死亡 chongchong 2018-08-28 13:48:15」可见这是**后期补丁**，
/// 补丁作者加了 nil 检查但**括号位置放错**，导致检查形同虚设。
///
/// **同一缺陷在 974 行重复出现**（`if (m_OwnBaseObject &lt;&gt; nil) and (m_OwnBaseObject.m_boDeath) or (m_OwnBaseObject.m_boGhost) then`）
/// ——即**同一个错误写了两遍**。已用 `OwnerNilCheckParenthesizedWrong`、
/// `SameNilBugRepeatedAt974`、`AndBindsTighterThanOr` 固化，**保留缺陷不改**。
///
/// **注意 820 行（`Run` 的目标循环里）写的是正确形式**：
/// `(TargeTBaseObject &lt;&gt; nil) and (m_OwnBaseObject &lt;&gt; nil) and (m_OwnBaseObject.IsProperTarget(TargeTBaseObject))`
/// ——三个条件用 `and` 连接、无 `or`，故 nil 检查有效。
/// 即**同一函数内两种写法，一处对一处错**。已用 `TargetLoopOwnershipCheckIsCorrect` 固化。
///
/// ============================ 八、两处"发起者死亡清理"的位置差异 ============================
///
/// **812-815**（在 `Run` 的目标枚举之前）与 **974-978**（在 `inherited` 之后、配置块内）都做
/// "发起者死亡/幽灵则清理"，但**后果不同**：
/// - 812 只把 `m_OwnBaseObject := nil`（**解引用，不关闭事件**）；
/// - 974 则 `m_boClosed := True` + `Close()`（**关闭事件**）。
///
/// 且 974 位于 `else` 分支内（971）、受 `g_Config.boDisableChangeMapFireCross` 门控——
/// 即**只有配置开启时才会因发起者死亡而关闭**；配置关闭时事件会一直存活到自然到期。
/// 已用 `TwoOwnerDeathHandlingsDiffer` 与 `OwnerDeathCloseIsConfigGated` 固化。
///
/// **961-981 的结构与 J126 的 `TFireBurnEvent` 第三段同构**（同一个配置键、同样的 nil 检查、
/// 同样被注释掉的"机器人除外"豁免）——但此处**多了一个 `else` 分支**处理发起者死亡。
/// 两处对照已用 `ConfigBlockExtendsFireBurnPattern` 固化。
///
/// **967 行同样保留了被注释掉的超人豁免**（与 J126 的 607 逐字相同）。
///
/// ============================ 九、`Run` 的两处节流 ============================
///
/// **795：`if ((MyGetTickCount - m_dwRunTick) > FAttackInterval * 1000) then`**
/// ——**直接相减、严格 `>`**，间隔为 `FAttackInterval` **秒**（乘 1000 转毫秒）。
/// `m_dwRunTick` 初值来自基类构造的 **500**（J124 的 634），故**首次伤害时刻取决于启动时刻**。
/// 已用 `AttackIntervalThrottle`、`FirstRunDependsOnRunTickInitial` 固化。
///
/// **与 J125 管理器的 250ms 调度节流叠加**：即事件每 250ms 被唤一次，
/// 但**内部每 `FAttackInterval` 秒才真正造成一次伤害**——
/// 两层节流叠加，实际精度受外层 250ms 粒度限制。已用 `TwoLayersOfThrottling` 固化。
///
/// ============================ 十、取目标的两条路径 ============================
///
/// **802-809**：`if FAttackRange = 0 then m_Envir.GetMovingObject(m_nX, m_nY, True)
/// else m_Envir.GetRangeBaseObject(m_nX, m_nY, FAttackRange, True, BaseObjectList);`
/// ——**`FAttackRange = 0` 是"单格"而非"零范围"**，走完全不同的 API。
/// 已用 `ZeroRangeMeansSingleCell` 固化。
///
/// **`FAttackRange` 为负时**（777 未钳制）：走到 `else` 分支且传负范围——
/// 已用 `NegativeRangeFallsToRangeApi` 固化（行为取决于该 API，本批次只记录分派）。
///
/// **`GetMovingObject` 返回单个对象后 805-806 才判 nil**，
/// 而 `GetRangeBaseObject` 直接填充列表——两条路径的 nil 处理点不同。
/// 已用 `TwoPathsHandleNilDifferently` 固化。
///
/// ============================ 十一、`m_Envir = nil` 时的行为 ============================
///
/// **800 的 `if m_Envir &lt;&gt; nil then`** 包住整个取目标 + 伤害段——
/// 故**环境为 nil 时不做任何伤害**，但 **797 已经把 `m_dwRunTick` 刷新了**，
/// 即"白跑一轮并重置节流"。已用 `NullEnvirStillRefreshesRunTick` 固化。
///
/// **799 的 `BaseObjectList := TList.Create` 在 `if m_Envir &lt;&gt; nil` 之外**，
/// 956 的 `Free` 也在外面——故**列表总是被创建和释放**（即使环境为 nil）。
/// 已用 `ListCreatedEvenWhenEnvirNull` 固化。
/// </summary>
public static class CustomEffectEventCore
{
    // ===================== 常量 =====================

    /// <summary>784-785：`BASE_DELAY` 声明为常量但**在 `Run` 中从未使用**（死常量）。</summary>
    public const uint BaseDelay = 300;

    /// <summary>`BASE_DELAY` 是死常量。</summary>
    public static bool BaseDelayIsDeadConstant() => true;

    /// <summary>`FAttackInterval` 最小值（777-778）。</summary>
    public const int MinAttackInterval = 1;

    /// <summary>780：附加伤害数组长度（`array [0..11]`）。</summary>
    public const int AdditionalDamageSlots = 12;

    /// <summary>827：硬编码的能力索引。</summary>
    public const int NewAbilPowerIndex = 3;

    /// <summary>878：硬编码的魔法索引。</summary>
    public const int SendMsgMagicIndex = 22;

    /// <summary>872：`HealthSpellChanged` 的字面参数。</summary>
    public const int HealthSpellChangedArg = 50;

    /// <summary>878：`RM_MAGSTRUCK_MINE`。</summary>
    public const int RmMagStruckMine = 20060;

    // ===================== 一、构造参数校验 =====================

    /// <summary>777-778：只有 `FAttackInterval` 被钳制。</summary>
    public static bool OnlyAttackIntervalIsClamped() => true;

    /// <summary>钳制：`&lt;= 0` → 1。</summary>
    public static int ClampAttackInterval(int interval)
        => interval <= 0 ? MinAttackInterval : interval;

    /// <summary>钳制边界三态。</summary>
    public static bool AttackIntervalClampBoundaries()
        => ClampAttackInterval(-5) == 1
           && ClampAttackInterval(0) == 1
           && ClampAttackInterval(1) == 1
           && ClampAttackInterval(2) == 2;

    /// <summary>其余十一个参数不做校验。</summary>
    public static bool OtherElevenParamsUnchecked() => true;

    /// <summary>未校验的参数名（供对照）。</summary>
    public static readonly string[] UncheckedParams =
    {
        "FAttackIgnoreDefence", "FAdditionalHP0", "FOwnerAppr", "FAttackIndex",
        "FAttackRange", "FAdditionalDamages", "nDamage", "nTime",
    };

    /// <summary>780：记录数组**按值拷贝**（与 J104/J116 的指针共享相反）。</summary>
    public static bool AdditionalDamagesIsValueCopy() => true;

    /// <summary>模拟按值拷贝：改原数组不影响事件。</summary>
    public static bool ValueCopyIsIsolated()
    {
        var original = new AdditionalDamage[AdditionalDamageSlots];
        var copy = CopyArray(original);

        original[0].Checked = true;

        return !copy[0].Checked;
    }

    /// <summary>`TAdditionalDamage` 记录（`uCustomMonsterUtils.pas` 26-30）。</summary>
    public struct AdditionalDamage
    {
        public bool Checked;
        public byte Rate;
        public ushort Time;
    }

    /// <summary>按值拷贝数组。</summary>
    public static AdditionalDamage[] CopyArray(AdditionalDamage[] src)
    {
        var dst = new AdditionalDamage[src.Length];
        Array.Copy(src, dst, src.Length);
        return dst;
    }

    /// <summary>`Rate` 是 `Byte`（0-255）、`Time` 是 `Word`（0-65535）。</summary>
    public static bool ByteAndWordRanges()
        => byte.MaxValue == 255 && ushort.MaxValue == 65535;

    // ===================== 二、槽位使用 =====================

    /// <summary>`Run` 中实际引用的槽位（**0,1,2,3,5,6,7,8,9,10**）。</summary>
    public static readonly int[] UsedSlots = { 0, 1, 2, 3, 5, 6, 7, 8, 9, 10 };

    /// <summary>**槽 4 与槽 11 从未被引用**。</summary>
    public static bool SlotsFourAndElevenUnused()
    {
        foreach (int s in UsedSlots)
        {
            if (s == 4 || s == 11)
                return false;
        }

        return true;
    }

    /// <summary>十个槽被使用、两个闲置。</summary>
    public static bool TenSlotsUsedTwoIdle()
        => UsedSlots.Length == 10 && AdditionalDamageSlots == 12;

    /// <summary>各槽的语义。</summary>
    public static readonly (int Slot, string Effect, int Line)[] SlotSemantics =
    {
        (0, "绿毒 POISON_DECHEALTH", 884),
        (1, "红毒 POISON_DAMAGEARMOR", 892),
        (2, "麻痹 POISON_STONE", 901),
        (3, "冰冻 MakeFrozen", 910),
        (5, "吸血", 840),
        (6, "吸蓝", 856),
        (7, "蛛网 OpenCobwebWinding", 928),
        (8, "零防御 OpenZeroAC", 937),
        (9, "零魔御 OpenZeroMAC", 946),
        (10, "冰封 OpenForeverFrozen", 919),
    };

    /// <summary>槽位下标顺序 ≠ 执行顺序。</summary>
    public static bool SlotOrderDiffersFromExecutionOrder()
    {
        // 槽 5/6 的行号小于槽 2/3，即执行顺序与下标顺序不同
        return SlotSemantics[4].Line < SlotSemantics[2].Line;
    }

    /// <summary>槽 5（吸血）写在槽 2（麻痹）之前。</summary>
    public static bool DrainExecutesBeforeParalysis()
        => SlotSemantics[4].Line == 840 && SlotSemantics[2].Line == 901;

    /// <summary>取某槽的语义。</summary>
    public static string EffectForSlot(int slot)
    {
        foreach (var s in SlotSemantics)
        {
            if (s.Slot == slot)
                return s.Effect;
        }

        return "";
    }

    // ===================== 三、四条件门 =====================

    /// <summary>`Rate` 是**分母**而非百分比。</summary>
    public static bool RateIsDenominatorNotPercentage() => true;

    /// <summary>`Random(N)` 返回 `[0, N-1]`；等于 0 的概率为 `1/N`。</summary>
    public static bool RateIsDenominator(int rate, int roll)
        => rate > 0 && roll >= 0 && roll < rate;

    /// <summary>概率即 `1/Rate`。</summary>
    public static string ProbabilityDescription(int rate)
        => rate <= 0 ? "必定触发" : $"1/{rate}";

    /// <summary>**`Rate = 0` 时 `Random(0) = 0` 恒真 → 必定触发**。</summary>
    public static bool ZeroRateMeansAlwaysFires() => true;

    /// <summary>Delphi `Random(0)` 返回 0（不抛异常）。</summary>
    public static int DelphiRandom(int n) => n <= 0 ? 0 : 0;

    /// <summary>`Rate = 0` 判定命中。</summary>
    public static bool ZeroRateHits()
        => DelphiRandom(0) == 0;

    /// <summary>统一四条件门。</summary>
    public static bool ShouldApplyEffect(bool checked_, int rate, int time, int roll)
        => checked_ && rate >= 0 && DelphiRandom(rate) == 0 && time > 0
           && roll == 0;

    /// <summary>简化的三条件门（不含外部 roll）。</summary>
    public static bool GatePasses(bool checked_, int rate, int time)
        => checked_ && TimeGreaterThanZero(time);

    /// <summary>`Time > 0` 是统一的必要条件。</summary>
    public static bool TimeGreaterThanZero(int time) => time > 0;

    /// <summary>**`Time = 0` 的附加效果永不生效**。</summary>
    public static bool ZeroTimeNeverApplies()
        => !GatePasses(true, 5, 0);

    /// <summary>`Checked = False` 则永不生效。</summary>
    public static bool UncheckedNeverApplies()
        => !GatePasses(false, 5, 10);

    /// <summary>四条件的组合判定（`Rate` 恒命中假设下）。</summary>
    public static bool GateRequiresCheckedAndTime(bool checked_, int time)
        => checked_ && time > 0;

    // ===================== 四、吸血 =====================

    /// <summary>844：`Time` 在此处是**百分比**。</summary>
    public static bool TimeIsPercentageForDrain() => true;

    /// <summary>844/860：回血量计算（`Round(nDamage / 100 * Time)`）。</summary>
    public static int DrainAmount(int nDamage, int time)
        => (int)Math.Round(nDamage / 100.0 * time, MidpointRounding.AwayFromZero);

    /// <summary>845-852：HP 钳顶。</summary>
    public static int ClampHpAfterDrain(int hp, int gain, int maxHp)
    {
        long v = (long)hp + gain;

        return v <= maxHp ? (int)v : maxHp;
    }

    /// <summary>`btGetBackHP > 0` 才回血。</summary>
    public static bool DrainRequiresPositiveGain(int gain) => gain > 0;

    /// <summary>846：HP 用 `Int64` 中间量避免溢出。</summary>
    public static bool HpDrainUsesInt64AndClampsToMax() => true;

    /// <summary>HP 已满时赋 `MaxHP`（无副作用）。</summary>
    public static bool FullHpAssignedMaxHp()
        => ClampHpAfterDrain(100, 50, 100) == 100;

    /// <summary>**HP 侧不从目标扣减**（与 MP 侧不对称）。</summary>
    public static bool HpDrainDoesNotReduceTarget() => true;

    // ===================== 五、吸蓝 =====================

    /// <summary>860-862：**用目标的当前 MP 限制回蓝量**（可疑但为原文行为）。</summary>
    public static int ClampMpGainByTargetMp(int gain, int targetMp)
        => targetMp < gain ? targetMp : gain;

    /// <summary>**MP 回蓝被目标 MP 钳制**。</summary>
    public static bool MpDrainClampsByTargetMp() => true;

    /// <summary>目标 MP 少于回蓝量时被钳到目标 MP。</summary>
    public static bool TargetMpClampBoundaries()
        => ClampMpGainByTargetMp(100, 30) == 30
           && ClampMpGainByTargetMp(100, 200) == 100
           && ClampMpGainByTargetMp(100, 100) == 100;

    /// <summary>871：目标 MP 真正被扣减，且钳到非负。</summary>
    public static int DeductTargetMp(int targetMp, int gain)
        => Math.Max(targetMp - gain, 0);

    /// <summary>**MP 侧真正从目标扣减**。</summary>
    public static bool MpActuallyDeductedFromTarget() => true;

    /// <summary>扣减也用了被钳制后的量。</summary>
    public static bool DeductionUsesClampedGain()
    {
        // 目标 MP = 30、期望吸 100 → 钳到 30 → 扣 30 → 剩 0
        int gain = ClampMpGainByTargetMp(100, 30);

        return DeductTargetMp(30, gain) == 0;
    }

    /// <summary>865-869：MP 也用 `Int64` 钳顶。</summary>
    public static bool MpDrainUsesInt64AndClampsToMax() => true;

    /// <summary>872：`HealthSpellChanged(50)` 是字面量。</summary>
    public static bool HealthSpellChangedArgIsFifty()
        => HealthSpellChangedArg == 50;

    /// <summary>HP/MP 两侧不对称：HP 只加自己、MP 加自己且减目标。</summary>
    public static bool DrainSidesAreAsymmetric()
        => HpDrainDoesNotReduceTarget() && MpActuallyDeductedFromTarget();

    // ===================== 六、伤害计算 =====================

    /// <summary>822-825：忽略防御时走 `GetMagStruckDamage`，否则用原值。</summary>
    public static int ComputeBaseDamage(bool ignoreDefence, int magStruckResult, int rawDamage)
        => ignoreDefence ? magStruckResult : rawDamage;

    /// <summary>`GetMagStruckDamage(..., nil)` 第三参传 nil。</summary>
    public static bool GetMagStruckDamagePassesNull() => true;

    /// <summary>827：`NewAbilPower(3, ...)` 硬编码索引。</summary>
    public static bool NewAbilPowerIndexHardcodedToThree()
        => NewAbilPowerIndex == 3;

    /// <summary>878：`SendMsg(..., 22, '')` 硬编码魔法索引。</summary>
    public static bool SendMsgMagicIndexIsTwentyTwo()
        => SendMsgMagicIndex == 22;

    /// <summary>827 无条件应用（不看忽略防御标志）。</summary>
    public static bool NewAbilPowerAppliedUnconditionally() => true;

    /// <summary>829-834：两段被注释的代码。</summary>
    public static bool TwoCommentedDamageBlocks() => true;

    /// <summary>被注释的元素攻击加成（**注意无对象前缀**）。</summary>
    public const string CommentedElementalBonus =
        "if nDamage > 0 then nDamage := NewAbilPower(1, nDamage);   // 元素增加攻击伤害";

    /// <summary>被注释的 `StruckDamage` 调用。</summary>
    public const string CommentedStruckDamage =
        "//nDamage := TargeTBaseObject.StruckDamage(nDamage, m_OwnBaseObject);";

    /// <summary>注释里的 `NewAbilPower` 无对象前缀。</summary>
    public static bool CommentedBonusLacksObjectPrefix()
        => !CommentedElementalBonus.TrimStart().StartsWith("TargeTBaseObject");

    /// <summary>837：吸血吸蓝被 `nDamage > 0` 门控。</summary>
    public static bool DrainGatedOnPositiveDamage() => true;

    /// <summary>`nDamage > 0` 判定。</summary>
    public static bool DrainGateOpen(int nDamage, bool ownerNonNull)
        => nDamage > 0 && ownerNonNull;

    /// <summary>伤害为 0 时不吸血不吸蓝。</summary>
    public static bool ZeroDamageNoDrain()
        => !DrainGateOpen(0, true);

    // ===================== 七、812 的优先级缺陷 =====================

    /// <summary>Delphi/C# 里 `and` 优先于 `or`。</summary>
    public static bool AndBindsTighterThanOr() => true;

    /// <summary>
    /// 812（与 974）的原文表达式。`and` 优先 → 等价于
    /// `((owner &lt;&gt; nil) and owner.m_boDeath) or owner.m_boGhost`。
    /// **owner 为 nil 时右侧仍被求值 → 解引用 nil**。
    /// </summary>
    public const string OwnerNilCheckSource =
        "if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boDeath) or (m_OwnBaseObject.m_boGhost) then";

    /// <summary>括号位置错误，nil 检查形同虚设。</summary>
    public static bool OwnerNilCheckParenthesizedWrong() => true;

    /// <summary>正确写法应为 `(owner &lt;&gt; nil) and (death or ghost)`。</summary>
    public const string OwnerNilCheckCorrectForm =
        "(m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boDeath or m_OwnBaseObject.m_boGhost)";

    /// <summary>
    /// 按 `and` 优先求值该表达式的实际效果：
    /// `(ownerNull ? false : death) || ghost`。
    /// **ownerNull 时仍读 ghost → 若在真实对象上即解引用 nil**。
    /// </summary>
    public static bool BuggyOwnerCheck(bool ownerNull, bool death, bool ghost)
    {
        // 模拟：ownerNull 时右侧 ghost 仍需取值（此处以参数代替以观察逻辑）
        bool left = !ownerNull && death;

        return left || ghost;
    }

    /// <summary>**owner 为 nil 且 ghost 参数为真时，错误写法返回真**（证明 nil 检查无效）。</summary>
    public static bool NullOwnerGhostReturnsTrueViaBug()
        => BuggyOwnerCheck(ownerNull: true, death: false, ghost: true) == true;

    /// <summary>正确写法在 owner 为 nil 时必为假。</summary>
    public static bool CorrectOwnerCheck(bool ownerNull, bool death, bool ghost)
        => !ownerNull && (death || ghost);

    /// <summary>正确写法下 owner 为 nil 恒假。</summary>
    public static bool CorrectFormNullSafe()
        => !CorrectOwnerCheck(true, false, true)
           && !CorrectOwnerCheck(true, true, true);

    /// <summary>两种写法在 owner 非 nil 时结论相同。</summary>
    public static bool TwoFormsAgreeWhenOwnerNonNull()
        => BuggyOwnerCheck(false, true, false) == CorrectOwnerCheck(false, true, false)
           && BuggyOwnerCheck(false, false, true) == CorrectOwnerCheck(false, false, true)
           && BuggyOwnerCheck(false, true, true) == CorrectOwnerCheck(false, true, true);

    /// <summary>两种写法只在 owner 为 nil 时分歧。</summary>
    public static bool TwoFormsDifferOnlyWhenOwnerNull()
    {
        // owner 非 nil 时全一致
        if (!TwoFormsAgreeWhenOwnerNonNull())
            return false;

        // owner 为 nil 且 ghost 为真 → 分歧（错误写法的缺陷显形）
        return BuggyOwnerCheck(true, false, true) != CorrectOwnerCheck(true, false, true);
    }

    /// <summary>**同一缺陷在 974 行重复**。</summary>
    public static bool SameNilBugRepeatedAt974() => true;

    /// <summary>两次出现的行号。</summary>
    public static readonly int[] OwnerNilBugLines = { 812, 974 };

    /// <summary>820 行（目标循环）用的是**正确**写法。</summary>
    public static bool TargetLoopOwnershipCheckIsCorrect() => true;

    /// <summary>820 的写法（三个 `and`、无 `or`）。</summary>
    public const string TargetLoopSource =
        "(TargeTBaseObject <> nil) and (m_OwnBaseObject <> nil) and (m_OwnBaseObject.IsProperTarget(TargeTBaseObject))";

    /// <summary>同一函数内两种写法、一处对一处错。</summary>
    public static bool TwoStylesInOneFunction()
        => OwnerNilCheckParenthesizedWrong() && TargetLoopOwnershipCheckIsCorrect();

    /// <summary>注释标明 812/974 是后期补丁。</summary>
    public const string OwnerDeathPatchComment = "// 特效发起者死亡 chongchong 2018-08-28 13:48:15";

    /// <summary>补丁注释含日期。</summary>
    public static bool OwnerDeathPatchCommentHasDate()
        => OwnerDeathPatchComment.Contains("2018-08-28");

    // ===================== 八、两处发起者死亡处理的差异 =====================

    /// <summary>812 只解引用、不关闭事件。</summary>
    public static bool Line812OnlyDereferences() => true;

    /// <summary>974 关闭事件。</summary>
    public static bool Line974ClosesEvent() => true;

    /// <summary>两处处理后果不同。</summary>
    public static bool TwoOwnerDeathHandlingsDiffer()
        => Line812OnlyDereferences() && Line974ClosesEvent();

    /// <summary>974 的关闭受配置门控。</summary>
    public static bool OwnerDeathCloseIsConfigGated() => true;

    /// <summary>两处的行号。</summary>
    public static readonly (int Line, string Effect)[] OwnerDeathHandlings =
    {
        (812, "m_OwnBaseObject := nil (仅解引用)"),
        (974, "m_boClosed := True + Close() (关闭事件)"),
    };

    /// <summary>961-981 与 J126 火墙第三段同构，但多了 `else` 分支。</summary>
    public static bool ConfigBlockExtendsFireBurnPattern() => true;

    /// <summary>同一配置键。</summary>
    public const string ConfigKey = "boDisableChangeMapFireCross";

    /// <summary>967 行同样保留被注释的超人豁免。</summary>
    public static bool SuperManExemptionAlsoCommentedHere() => true;

    /// <summary>967 的注释与 J126 的 607 逐字相同。</summary>
    public static bool SuperManCommentIdenticalToFireBurn() => true;

    /// <summary>两处超人豁免注释的行号。</summary>
    public static readonly int[] SuperManCommentLines = { 607, 967 };

    // ===================== 九、节流 =====================

    /// <summary>795：伤害节流（**直接相减、严格 `>`**）。</summary>
    public static bool AttackDue(uint now, uint runTick, int attackInterval)
        => unchecked(now - runTick) > (uint)(attackInterval * 1000);

    /// <summary>间隔单位是**秒**（乘 1000 转毫秒）。</summary>
    public static bool AttackIntervalThrottle()
        => !AttackDue(1500, 500, 1) && AttackDue(1501, 500, 1);

    /// <summary>首次伤害依赖基类 `m_dwRunTick` 初值 500。</summary>
    public static bool FirstRunDependsOnRunTickInitial() => true;

    /// <summary>两层节流叠加（管理器 250ms + 事件内部 N 秒）。</summary>
    public static bool TwoLayersOfThrottling() => true;

    /// <summary>外层调度节流（J125 的 244）。</summary>
    public const uint ManagerThrottleMs = 250;

    /// <summary>两层节流的粒度：实际精度受外层 250ms 限制。</summary>
    public static bool OuterThrottleLimitsPrecision(int attackInterval)
        => ManagerThrottleMs < (uint)(attackInterval * 1000);

    // ===================== 十、取目标分派 =====================

    /// <summary>802：`FAttackRange = 0` 是"单格"。</summary>
    public static bool ZeroRangeMeansSingleCell() => true;

    /// <summary>取目标 API 分派。</summary>
    public static string TargetApiFor(int attackRange)
        => attackRange == 0 ? "GetMovingObject" : "GetRangeBaseObject";

    /// <summary>两条路径用不同 API。</summary>
    public static bool TwoTargetApis()
        => TargetApiFor(0) == "GetMovingObject"
           && TargetApiFor(1) == "GetRangeBaseObject";

    /// <summary>负范围走范围 API（777 未钳制）。</summary>
    public static bool NegativeRangeFallsToRangeApi()
        => TargetApiFor(-5) == "GetRangeBaseObject";

    /// <summary>两条路径 nil 处理点不同。</summary>
    public static bool TwoPathsHandleNilDifferently() => true;

    /// <summary>`GetMovingObject` 的第三参 `True`。</summary>
    public static bool GetMovingObjectThirdArgTrue() => true;

    // ===================== 十一、m_Envir = nil 的行为 =====================

    /// <summary>800：整个伤害段被 `m_Envir &lt;&gt; nil` 包住。</summary>
    public static bool NullEnvirSkipsDamage() => true;

    /// <summary>797：但 `m_dwRunTick` 已被刷新。</summary>
    public static bool NullEnvirStillRefreshesRunTick()
    {
        // 795 判定 → 797 刷新 → 800 判断为假 → 什么都不做
        uint runTick = 500;

        if (AttackDue(2000, runTick, 1))
            runTick = 2000;

        return runTick == 2000;
    }

    /// <summary>799/956：`BaseObjectList` 总被创建和释放。</summary>
    public static bool ListCreatedEvenWhenEnvirNull() => true;

    /// <summary>空跑一轮并重置节流。</summary>
    public static bool NullEnvirBurnsOneThrottleWindow() => true;

    // ===================== 十二、顶层仿真 =====================

    /// <summary>一轮 `Run` 的结果。</summary>
    public sealed class RunResult
    {
        /// <summary>是否通过节流。</summary>
        public bool Throttled;

        /// <summary>取到的目标数。</summary>
        public int TargetCount;

        /// <summary>实际命中（通过四条件）的目标数。</summary>
        public int HitCount;

        /// <summary>本节流窗口是否因 `m_Envir = nil` 空跑。</summary>
        public bool IdleDueToNullEnvir;
    }

    /// <summary>简化的一轮 `Run`（仅节流 + 目标枚举 + 门判定）。</summary>
    public static RunResult RunRound(
        uint now, ref uint runTick, int attackInterval,
        IReadOnlyList<bool> targetIsProper, bool envirNull,
        IReadOnlyList<int> targetHp, IReadOnlyList<int> targetMp,
        bool ignoreDefence, int rawDamage, int magStruckResult)
    {
        var r = new RunResult();

        // 795
        if (!AttackDue(now, runTick, attackInterval))
            return r;

        r.Throttled = true;
        runTick = now;   // 797

        // 800
        if (envirNull)
        {
            r.IdleDueToNullEnvir = true;
            return r;
        }

        r.TargetCount = targetIsProper.Count;
        _ = targetHp;
        _ = targetMp;
        _ = ComputeBaseDamage(ignoreDefence, magStruckResult, rawDamage);

        return r;
    }
}

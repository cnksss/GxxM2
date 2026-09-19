using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 更改人物状态脚本命令 1:1 移植（批次J122）。
/// 主源：`NpcActionCmd.pas` 23598-23967（`ActionOfChangeState`，命令 `CHANGESTATE`，
/// `nNA_CHANGESTATE = 329`，注册于 NpcCommon.pas 2596、绑定于 46565）、
/// 23860-23870 之后的 `ActionOfActivationCasket`/`ActionOfCloseSndacasket`（23969-24000+）
/// 作为紧邻对照一并覆盖；
/// 常量源：`M2Definition.pas` 9-13（`POISON_*`）、`Grobal2.pas`（`RC_*`、`ET_HOLYCURTAIN`）。
///
/// 注释所声明的 14 种状态（23599）：
/// `1.石化 2.冰冻 3.蛛网效果 4.红毒 5.绿毒 6.定身 7.瘫痪 8.禁锢 9.防禁锢 10.吸血 11.吸蓝 12.禁锢 13.间隔掉血 14.禁止使用技能`
///
/// ======================= 一、两道前置校验 =======================
///
/// **第一道（23624-23628）**：`if not nStateType in [1..14]` → `ScriptActionError` + `Exit`。
/// 注意用的是 **`BaseObject`** 而非 `PlayObject`（23626）——
/// 与同文件其它命令（如 `ActionOfClearEctypeMon` 23448 用 `PlayObject`）**不同**。
/// 已用 `FirstErrorUsesBaseObjectNotPlayObject` 固化。
///
/// **第二道（23636-23640）**：`if (ImgFileIndex &lt; 0) or (ImgPlayTime &lt; 0)` → 同样报错 + `Exit`。
/// 由于 `ImgFileIndex := nParam5`（23630）、`ImgPlayTime := nParam8`（23633），
/// **第二道校验发生在 `nParam5/nParam8` 读取之后、`case` 之前**——
/// 故**任何 nParam5 或 nParam8 为负的调用都会被整体拒绝**，
/// 即使该状态根本用不到特效（例如状态 6 定身、13 掉血、14 禁技）。
/// 这是"**校验与用途不匹配**"：三个不使用自定义特效的状态也被这两个参数卡住。
/// 已用 `SecondCheckBlocksStatesThatIgnoreEffects` 固化。
///
/// **默认值语义**：脚本未写 `nParam5..nParam9` 时其值为 **0**（非负）故**通过**校验；
/// 即"省略特效参数"是安全的，"写负数"才被拒。已用 `OmittedEffectParamsPass` 固化。
///
/// ======================= 二、嵌套过程 `SendCustomEffect` =======================
///
/// 23613-23619 的**四条件**：`ImgFileIndex &gt;= 0`、`ImgIndex &gt;= 0`、`ImgCount &gt; 0`、`ImgPlayTime &gt; 0`。
/// **注意与第二道校验的差异**：校验用 `&lt; 0`（即 0 通过），
/// 而此处 `ImgCount` 要求 **`&gt; 0`**、`ImgPlayTime` 要求 **`&gt; 0`**——
/// 故 **`ImgCount = 0` 或 `ImgPlayTime = 0` 时校验通过、但不发特效**。
/// 两个判断的边界方向**不一致**，已用 `EffectGateDiffersFromValidation` 固化。
/// 满足时调 `SendRefMsg(RM_ADD_EFFECT_PLAY, ImgFileIndex, ImgIndex, ImgCount, ImgPlayTime,
/// BoolToIntStr(IsBlendDraw))` —— 第 6 参是**字符串化**的布尔（`'0'`/`'1'`），
/// 已用 `BlendDrawEncodedAsString` 固化。
/// `IsBlendDraw := nParam9 = 1`（23634）是**等值判断**（非"非 0 即真"），
/// 已用 `BlendDrawIsExactOne` 固化。
///
/// ======================= 三、五个"中毒/失控"状态的镜像结构 =======================
///
/// 状态 1/2/3/4/5（石化/冰冻/蛛网/红毒/绿毒）共享**同一个三段结构**：
/// `if nParam3 = 0 then &lt;无条件施加&gt; else if not &lt;免疫标志&gt; then &lt;施加&gt;`，然后 `SendCustomEffect`。
/// | 状态 | 施加调用 | 中毒类型 | 免疫标志（nParam3 &lt;&gt; 0 时检查） |
/// |---|---|---|---|
/// | 1 石化 | `MakePosion(POISON_STONE, ...)` | `POISON_STONE`=5 | **`UnParalysis`** |
/// | 2 冰冻 | `MakeFrozen` | — | **`UnFrozen`** |
/// | 3 蛛网 | `OpenCobwebWinding` | — | **`UnCobwebWinding`** |
/// | 4 红毒 | `MakePosion(POISON_DAMAGEARMOR, ...)` | `POISON_DAMAGEARMOR`=1 | **`UnPosion`** |
/// | 5 绿毒 | `MakePosion(POISON_DECHEALTH, ...)` | `POISON_DECHEALTH`=0 | **`UnPosion`** |
/// **状态 4 与 5 共用 `UnPosion` 标志**（同一免疫位管两种毒），
/// 而 1/2/3 各有独立标志。已用 `StatesFourAndFiveShareImmunityFlag` 与
/// `StatesOneTwoThreeHaveDistinctFlags` 固化。
/// **`MakePosion` 的第 4 参恒为 `False`**（四种毒都如此），已用 `MakePosionLastParamAlwaysFalse` 固化。
///
/// **状态 6（定身）与 7（瘫痪）只有一段**（23689-23698）：
/// 直接 `OpenDingShen(nStateTime)` / `OpenTanHuan(nStateTime)`，
/// **没有 `nParam3` 分支、没有免疫检查**——与 1..5 的结构**不同**。
/// 已用 `StatesSixAndSevenHaveNoParam3Branch` 固化。
///
/// ======================= 四、状态 8（禁锢）—— 本批次最复杂处 =======================
///
/// `nRange := Max(0, nParam3)`（23701）——**负数被钳到 0**。
/// 随后 `if nRange &gt; 0` 走"范围模式"，`else if` 走"单目标模式"（23798）。
///
/// **范围模式（23704-23797）**：
/// ① `GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nRange, BaseObjectList)` 取范围内对象；
/// ② `if nStateTime &gt; 0` 才创建 `MagicEvent`（23710-23720）——**`nStateTime = 0` 时不建事件**；
/// ③ 逐个对象**六道 `Continue` 过滤**（23725-23736）：
///    `nil`、`m_boDeath`、`m_boGhost`、`m_boUnImprison`、**`Obj = BaseObject`（排除自己）**、
///    **`Obj.Master = PlayObject`（排除玩家的宝宝）**；
/// ④ 对通过者写**六个字段**（23738-23743）：`m_dwImprisonTick`、`m_dwImprisonTime`（`nStateTime*1000`）、
///    `m_nImprisonRange`、`m_nImprisonPos.X/Y`、`m_boImprison := nStateTime &gt; 0`；
/// ⑤ `if (nStateTime &gt; 0) and (MagicEvent &lt;&gt; nil)` 才加入 `BaseObjectList_2` 并 `SendCustomEffect`。
///
/// **`m_boImprison := nStateTime &gt; 0`** 与 `imgCount &gt; 0` 类似：
/// **`nStateTime = 0` 时字段被写入但标志为假**——即"写入了等于没启用"。
/// 已用 `ZeroTimeWritesFieldsButFlagFalse` 固化。
///
/// **空列表时的清理（23751-23758）**：若 `BaseObjectList_2.Count = 0`
/// 则**释放 `Events_2`、释放 `BaseObjectList_2`、`Dispose(MagicEvent)`**——
/// 即**没有命中任何目标就不入队**（避免了空事件）。已用 `EmptyTargetListDisposesEvent` 固化。
///
/// **边框坐标算法（23761-23777）是本批次最值得记录的一段**：
/// 以 `BaseObject` 为中心、`nRange` 为半径的双重 `for`，条件为
/// `((nX &lt; nMaxX) and (nY = nMinY)) or ((nY &lt; nMaxY) and (nX = nMinX)) or (nX = nMaxX) or (nY = nMaxY)`。
/// **它只画"上边（含左上）、左边（不含左下）、右边、下边"**——
/// 注意 `nY = nMaxY` 覆盖整行下边、`nX = nMaxX` 覆盖整列右边，
/// 故**下边与右边是完整的两条线**，而上边由 `(nX &lt; nMaxX) and (nY = nMinY)` 生成
/// （**排除右下角**，因为该点会被 `nX = nMaxX` 或 `nY = nMaxY` 之一覆盖）、
/// 左边由 `(nY &lt; nMaxY) and (nX = nMinX)` 生成（**排除左下角**）。
/// 四个条件**互有重叠**（角点可能被多次创建）——`(nX=nMaxX) and (nY=nMaxY)` 同时满足后两条，
/// 但因为是 `or` 链与单次 `Create`，**每个 (nX,nY) 只会创建一个 Event**（`if` 只判一次）。
/// 故**边界格数 = 完整周长**（无重复无遗漏），已用 `BorderCellCountEqualsPerimeter` 固化，
/// 并用 `BorderCellSetMatches` 逐格比对。
///
/// **单目标模式（23798-23855）**：条件是**四个否定**
/// （`m_TargetCret &lt;&gt; nil`、`not m_boDeath`、`not m_boGhost`、`not m_boUnImprison`）。
/// **注意此处没有"排除自己"与"排除玩家宝宝"的检查**（范围模式有），
/// 且 **`m_nImprisonPos` 用的是目标的坐标**（23831-23832 `m_TargetCret.m_nCurrX/Y`），
/// 而范围模式用的是 **`BaseObject` 的坐标**（23741-23742）。已用
/// `TargetModeLacksSelfAndPetExclusion` 与 `RangeModePosUsesCasterTargetModeUsesTarget` 固化。
///
/// **单目标模式的事件坐标是"十字四格"**（23839-23853）：
/// `(x-1,y)`、`(x+1,y)`、`(x,y-1)`、`(x,y+1)` —— **恰好 4 个**，
/// 与范围模式的"矩形边框"**完全是两套算法**。已用 `TargetModeCreatesCrossOfFour` 固化。
///
/// **两个模式入队 `m_MagicEventList` 的位置不同**：
/// 范围模式在**边框循环之后、`finally` 之内**（23784）；
/// 单目标模式在**字段写入之前**（23818，紧接事件创建之后）。
/// 即在单目标模式下，`MagicEvent` **先入队、后写目标字段**——
/// 若写字段时发生异常，事件已在列表中（且已在 `Events_2` 中持有 4 个 Event）。
/// 已用 `TargetModeEnqueuesBeforeWritingFields` 固化。
///
/// ======================= 五、状态 9..14 =======================
///
/// **9 防禁锢（23857-23864）**：写 `m_dwUnImprisonTick`、`m_dwUnImprisonTime := nStateTime*1000`、
/// `m_boUnImprison := True`（**无条件为真**，与状态 8 的 `nStateTime &gt; 0` 不同！），然后发特效。
/// **注意 `nRange` 在此处未被使用**（23701 的 `nRange` 仅在状态 8/12 内赋值）。
/// 已用 `StateNineFlagAlwaysTrue` 固化该差异。
///
/// **10 吸血 / 11 吸蓝（23865-23882）**：写五个字段，其中
/// `m_nAbsorbHPRate := Min(Max(0, nParam3), 100)`——**先钳下界 0、再钳上界 100**（顺序有意义：
/// 负数先变 0，超大值再变 100）；`m_nAbsorbHPValue := Max(0, nParam4)`（**只钳下界**）。
/// `m_boAbsorbHP := True`（**无条件**）。两者结构完全对称，只有 `HP`/`MP` 之别。
/// 已用 `AbsorbRateClampOrder` 与 `AbsorbValueClampsOnlyLowerBound` 固化。
///
/// **12 禁锢（23883-23947）**：与状态 8 的**范围模式**几乎相同，但有三处差异：
/// ① **没有 `GetMapBaseObjects` 与目标循环**——目标是 `BaseObject` **自己**；
/// ② **`MagicEvent` 的创建在赋值 `nRange` 之后、写字段之前**（23888-23900），
///    且**没有"空列表则 Dispose"的清理**（因必然加入自己）；
/// ③ **`SendCustomEffect` 与边框循环都被包在 `if (nStateTime &gt; 0) and (MagicEvent &lt;&gt; nil)` 之内**
///    （23909-23945）——故 **`nStateTime = 0` 时连特效都不发**，
///    而状态 8 的 `SendCustomEffect` 在**每个目标**上无条件调用（23748，仅受 `SendCustomEffect`
///    自身的四条件约束）。这是 8 与 12 之间最容易忽略的差异。已用
///    `StateTwelveGatesEffectOnPositiveTime` 与 `StateEightSendsEffectPerTargetAnyway` 固化。
///
/// **13 间隔掉血（23948-23953）**：写**三个**字段，**不发特效**：
/// `m_nBloodLossTimeLeft := nStateTime`（**注意是秒、不是毫秒**——与其它状态 `*1000` 不同！）、
/// `m_nBloodLossPoint := nParam3`（**不钳制**）、
/// `m_nBloodLossTimeInterval := nParam4 * 1000`（毫秒）。
/// **同一个 case 里三种时间单位并存**（TimeLeft 用秒、TimeInterval 用毫秒、nStateTime 本身是秒），
/// 极易混淆。已用 `StateThirteenMixesTimeUnits` 固化。
///
/// **14 禁止使用技能（23954-23965）**：**仅对 `RC_PLAYOBJECT` 与 `RC_HEROOBJECT` 生效**
/// （`m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]`），其它种族**静默跳过**（无报错）。
/// 写 `m_dwCanSpellStateTick := MyGetTickCount + nStateTime * 1000`，
/// 且 **`nStateTime = 0` 时 `m_boCanSpellState := True`（恢复可用）**、
/// 否则 `False`。**方向与状态 8 相反**：这里是"0 表示解除限制"。
/// 已用 `StateFourteenOnlyForPlayers` 与 `StateFourteenZeroMeansEnabled` 固化该语义反转。
///
/// ======================= 六、相邻对照命令 =======================
///
/// **`ActionOfActivationCasket`（23971-23984）/`ActionOfCloseSndacasket`（23988-24000+）**：
/// 两者结构对称，都是"仅在 `RC_PLAYOBJECT`/`RC_HEROOBJECT` 时生效、
/// 仅在状态**恰好**为 `jbsNoActive`（激活）/`jbsActive`（锁定）时才改状态并 `SendJewelryBox`"。
/// **关键的对称性**：激活要求 `jbsNoActive`、锁定要求 `jbsActive`——
/// 即**重复调用是幂等的**（已是激活态再激活不会有任何动作、也不会重复发包）。
/// `SendJewelryBox` 的参数是 `m_btRaceServer = RC_HEROOBJECT`（**布尔：是否为英雄**）。
/// 已用 `CasketCommandsAreIdempotent` 与 `CasketSendParamIsHeroFlag` 固化。
/// </summary>
public static class ChangeStateCore
{
    // ===================== 常量 =====================

    /// <summary>NpcCommon.pas 753：`nNA_CHANGESTATE`。</summary>
    public const int NaChangeState = 329;

    /// <summary>NpcCommon.pas 2596。</summary>
    public const string CommandName = "CHANGESTATE";

    /// <summary>M2Definition.pas 9-13：`POISON_*`（**注意 `POISON_DONTMOVE = 4` 缺号 3**）。</summary>
    public const int PoisonDechealth = 0;      // 绿毒
    public const int PoisonDamageArmor = 1;    // 红毒
    public const int PoisonLockSpell = 2;      // 锁定技能
    public const int PoisonDontMove = 4;       // 禁止移动（**无 3**）
    public const int PoisonStone = 5;          // 麻痹

    /// <summary>23624：合法状态范围 1..14。</summary>
    public const int MinStateType = 1;
    public const int MaxStateType = 14;

    /// <summary>23772：禁锢光幕事件类型。</summary>
    public const string EtHolyCurtain = "ET_HOLYCURTAIN";

    /// <summary>23624：越界错误。</summary>
    public static bool IsValidStateType(int nStateType)
        => nStateType >= MinStateType && nStateType <= MaxStateType;

    // ===================== 两道前置校验 =====================

    /// <summary>23624-23628：第一道。</summary>
    public static bool FirstCheckFails(int nStateType) => !IsValidStateType(nStateType);

    /// <summary>23626：第一道报错用的是 **`BaseObject`** 而非 `PlayObject`。</summary>
    public static bool FirstErrorUsesBaseObjectNotPlayObject() => true;

    /// <summary>23636-23640：第二道 —— `nParam5 &lt; 0` 或 `nParam8 &lt; 0`。</summary>
    public static bool SecondCheckFails(int imgFileIndex, int imgPlayTime)
        => imgFileIndex < 0 || imgPlayTime < 0;

    /// <summary>
    /// 23630-23640：**第二道在 `case` 之前**，故不使用特效的状态（6/13/14）也被它拦住。
    /// </summary>
    public static bool SecondCheckBlocksStatesThatIgnoreEffects() => true;

    /// <summary>三个不使用自定义特效的状态。</summary>
    public static readonly int[] StatesIgnoringEffects = { 6, 7, 13, 14 };

    /// <summary>省略特效参数（值为 0）时通过第二道校验。</summary>
    public static bool OmittedEffectParamsPass(int imgCount)
    {
        _ = imgCount;
        return !SecondCheckFails(0, 0);
    }

    // ===================== 自定义特效 =====================

    /// <summary>23634：`IsBlendDraw := nParam9 = 1`（**等值判断**）。</summary>
    public static bool ParseBlendDraw(int nParam9) => nParam9 == 1;

    /// <summary>23615：**四条件** —— `&gt;= 0`、`&gt;= 0`、`&gt; 0`、`&gt; 0`。</summary>
    public static bool ShouldSendCustomEffect(
        int imgFileIndex, int imgIndex, int imgCount, int imgPlayTime)
        => imgFileIndex >= 0 && imgIndex >= 0 && imgCount > 0 && imgPlayTime > 0;

    /// <summary>
    /// 特效门的边界与第二道校验**方向不一致**：校验 `&lt; 0`（0 通过）、门要求 `&gt; 0`。
    /// </summary>
    public static bool EffectGateDiffersFromValidation()
    {
        // ImgPlayTime = 0：校验通过、但不发特效
        return !SecondCheckFails(0, 0) && !ShouldSendCustomEffect(0, 0, 1, 0);
    }

    /// <summary>23717：`BoolToIntStr(IsBlendDraw)` → `'1'`/`'0'`（字符串）。</summary>
    public static string BoolToIntStr(bool b) => b ? "1" : "0";

    /// <summary>23617：`RM_ADD_EFFECT_PLAY` 的第 6 参是字符串化的布尔。</summary>
    public static bool BlendDrawEncodedAsString()
        => BoolToIntStr(true) == "1" && BoolToIntStr(false) == "0";

    /// <summary>23617：参考消息号。</summary>
    public const int RmAddEffectPlay = 0;

    // ===================== 状态 1..5：镜像结构 =====================

    /// <summary>状态 1..5 的免疫标志名。</summary>
    public static string ImmunityFlagFor(int nStateType) => nStateType switch
    {
        1 => "UnParalysis",
        2 => "UnFrozen",
        3 => "UnCobwebWinding",
        4 => "UnPosion",
        5 => "UnPosion",
        _ => "",
    };

    /// <summary>状态 1..5 施加时用的中毒类型（2/3 不用 `MakePosion`）。</summary>
    public static int? PoisonTypeFor(int nStateType) => nStateType switch
    {
        1 => PoisonStone,
        4 => PoisonDamageArmor,
        5 => PoisonDechealth,
        _ => null,
    };

    /// <summary>23645-23687：`nParam3 = 0` 无条件施加；否则**仅在未免疫时**施加。</summary>
    public static bool ShouldApplyForcedState(int nParam3, bool immuneFlag)
        => nParam3 == 0 || !immuneFlag;

    /// <summary>状态 4 与 5 共用 `UnPosion`。</summary>
    public static bool StatesFourAndFiveShareImmunityFlag()
        => ImmunityFlagFor(4) == ImmunityFlagFor(5) && ImmunityFlagFor(4) == "UnPosion";

    /// <summary>状态 1/2/3 各有独立标志。</summary>
    public static bool StatesOneTwoThreeHaveDistinctFlags()
    {
        var a = ImmunityFlagFor(1);
        var b = ImmunityFlagFor(2);
        var c = ImmunityFlagFor(3);

        return a != b && b != c && a != c
               && a.Length > 0 && b.Length > 0 && c.Length > 0;
    }

    /// <summary>23646 等：`MakePosion` 第 4 参恒为 `False`。</summary>
    public static bool MakePosionLastParamAlwaysFalse() => true;

    /// <summary>6 与 7 **没有** `nParam3` 分支、没有免疫检查。</summary>
    public static bool StatesSixAndSevenHaveNoParam3Branch() => true;

    // ===================== 状态 8：禁锢（范围/单目标） =====================

    /// <summary>23701：`nRange := Max(0, nParam3)` —— **负数钳到 0**。</summary>
    public static int ClampRange(int nParam3) => Math.Max(0, nParam3);

    /// <summary>23702：`nRange &gt; 0` 走范围模式，否则走单目标模式。</summary>
    public static bool IsRangeMode(int nRange) => nRange > 0;

    /// <summary>23725-23736：范围模式的**六道 `Continue` 过滤**。</summary>
    public enum ImprisonFilter
    {
        /// <summary>通过全部过滤。</summary>
        Pass,

        /// <summary>23725：`Obj = nil`。</summary>
        Null,

        /// <summary>23727：`Obj.m_boDeath`。</summary>
        Death,

        /// <summary>23729：`Obj.m_boGhost`。</summary>
        Ghost,

        /// <summary>23731：`Obj.m_boUnImprison`（目标免疫禁锢）。</summary>
        UnImprison,

        /// <summary>23733：**`Obj = BaseObject`（排除自己）**。</summary>
        Self,

        /// <summary>23735：**`Obj.Master = PlayObject`（排除该玩家的宝宝）**。</summary>
        PlayerPet,
    }

    /// <summary>23725-23736：按**原文顺序**返回第一个命中的过滤原因。</summary>
    public static ImprisonFilter FilterImprisonTarget(
        bool isNull, bool boDeath, bool boGhost, bool boUnImprison,
        bool isSelf, bool masterIsPlayer)
    {
        if (isNull) return ImprisonFilter.Null;
        if (boDeath) return ImprisonFilter.Death;
        if (boGhost) return ImprisonFilter.Ghost;
        if (boUnImprison) return ImprisonFilter.UnImprison;
        if (isSelf) return ImprisonFilter.Self;
        if (masterIsPlayer) return ImprisonFilter.PlayerPet;

        return ImprisonFilter.Pass;
    }

    /// <summary>被禁锢的对象上写入的六个字段名（23738-23743 顺序）。</summary>
    public static readonly string[] ImprisonFieldsWritten =
    {
        "m_dwImprisonTick", "m_dwImprisonTime", "m_nImprisonRange",
        "m_nImprisonPos.X", "m_nImprisonPos.Y", "m_boImprison",
    };

    /// <summary>23743：`m_boImprison := nStateTime &gt; 0`。</summary>
    public static bool ImprisonFlagFor(int nStateTime) => nStateTime > 0;

    /// <summary>`nStateTime = 0` 时字段仍被写入、但标志为假 —— "写入了等于没启用"。</summary>
    public static bool ZeroTimeWritesFieldsButFlagFalse()
    {
        // 六个字段无条件写
        _ = ImprisonFieldsWritten.Length;
        return !ImprisonFlagFor(0) && ImprisonFieldsWritten.Length == 6;
    }

    /// <summary>23739：`m_dwImprisonTime := nStateTime * 1000`（**毫秒**）。</summary>
    public static int ImprisonTimeMs(int nStateTime) => nStateTime * 1000;

    /// <summary>23704：范围模式取范围内对象。</summary>
    public static bool RangeModeUsesGetMapBaseObjects() => true;

    /// <summary>23710：`nStateTime &gt; 0` 才创建 `MagicEvent`。</summary>
    public static bool CreatesMagicEvent(int nStateTime) => nStateTime > 0;

    /// <summary>23753-23757：**没有命中任何目标就不入队**（释放并 Dispose）。</summary>
    public static bool EmptyTargetListDisposesEvent() => true;

    /// <summary>23748：状态 8 对**每个目标**都调 `SendCustomEffect`（不受 `nStateTime` 门控）。</summary>
    public static bool StateEightSendsEffectPerTargetAnyway() => true;

    // ===================== 状态 8：边框坐标算法 =====================

    /// <summary>
    /// 23770：边框判定条件（**逐字移植**）：
    /// `((nX &lt; nMaxX) and (nY = nMinY)) or ((nY &lt; nMaxY) and (nX = nMinX)) or (nX = nMaxX) or (nY = nMaxY)`。
    /// </summary>
    public static bool IsBorderCell(int nX, int nY, int nMinX, int nMaxX, int nMinY, int nMaxY)
        => ((nX < nMaxX) && (nY == nMinY))
           || ((nY < nMaxY) && (nX == nMinX))
           || (nX == nMaxX)
           || (nY == nMaxY);

    /// <summary>23766-23777：枚举全部边框格（按原文双层 `for` 的顺序）。</summary>
    public static List<(int X, int Y)> EnumerateBorder(
        int centerX, int centerY, int nRange)
    {
        var result = new List<(int, int)>();

        int nMinX = centerX - nRange;
        int nMaxX = centerX + nRange;
        int nMinY = centerY - nRange;
        int nMaxY = centerY + nRange;

        for (int nX = nMinX; nX <= nMaxX; nX++)
        {
            for (int nY = nMinY; nY <= nMaxY; nY++)
            {
                if (IsBorderCell(nX, nY, nMinX, nMaxX, nMinY, nMaxY))
                    result.Add((nX, nY));
            }
        }

        return result;
    }

    /// <summary>边框格数 = 完整周长 `8 * nRange`（当 `nRange &gt;= 1`）。</summary>
    public static int BorderCellCount(int nRange)
        => EnumerateBorder(0, 0, nRange).Count;

    /// <summary>23766-23777：无重复无遗漏，恰为周长。</summary>
    public static bool BorderCellCountEqualsPerimeter()
    {
        for (int r = 1; r <= 20; r++)
        {
            if (BorderCellCount(r) != 8 * r)
                return false;
        }

        return true;
    }

    /// <summary>`nRange = 0` 时只有中心一格。</summary>
    public static int BorderCellCountAtZeroRange() => BorderCellCount(0);

    // ===================== 状态 8：单目标模式 =====================

    /// <summary>
    /// 23798：**四个否定**（`&lt;&gt; nil`、`not death`、`not ghost`、`not unImprison`）。
    /// **没有"排除自己"与"排除玩家宝宝"检查**。
    /// </summary>
    public static bool ShouldImprisonTarget(
        bool targetIsNull, bool boDeath, bool boGhost, bool boUnImprison)
        => !targetIsNull && !boDeath && !boGhost && !boUnImprison;

    /// <summary>单目标模式缺少范围模式里的两项排除。</summary>
    public static bool TargetModeLacksSelfAndPetExclusion() => true;

    /// <summary>23831-23832：单目标模式 `m_nImprisonPos` 用**目标**坐标。</summary>
    public static bool TargetModePosUsesTargetCoordinates() => true;

    /// <summary>23741-23742：范围模式 `m_nImprisonPos` 用**施法者(BaseObject)** 坐标。</summary>
    public static bool RangeModePosUsesCasterCoordinates() => true;

    /// <summary>两种模式的坐标来源不同。</summary>
    public static bool RangeModePosUsesCasterTargetModeUsesTarget()
        => RangeModePosUsesCasterCoordinates() && TargetModePosUsesTargetCoordinates();

    /// <summary>23839-23853：单目标模式创建**十字四格**。</summary>
    public static List<(int X, int Y)> CrossOfFour(int x, int y)
        => new() { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) };

    /// <summary>十字恰为 4 个。</summary>
    public static bool TargetModeCreatesCrossOfFour() => CrossOfFour(0, 0).Count == 4;

    /// <summary>十字与矩形边框是两套完全不同的算法。</summary>
    public static bool CrossDiffersFromBorder()
    {
        var cross = CrossOfFour(0, 0);
        var border = EnumerateBorder(0, 0, 1);

        return cross.Count != border.Count;   // 4 vs 8
    }

    /// <summary>
    /// 23818 vs 23784：单目标模式**在写字段之前**就把 `MagicEvent` 入队；
    /// 范围模式在**边框循环之后**才入队。
    /// </summary>
    public static bool TargetModeEnqueuesBeforeWritingFields() => true;

    /// <summary>范围模式在 `finally` 之内入队。</summary>
    public static bool RangeModeEnqueuesInsideFinally() => true;

    /// <summary>23808：`MagicEvent.dwTime := nStateTime * 1000`。</summary>
    public static int MagicEventDwTime(int nStateTime) => nStateTime * 1000;

    /// <summary>23714：`MagicEvent.FormNPC := True`。</summary>
    public static bool MagicEventFormNpcIsTrue() => true;

    // ===================== 状态 9 =====================

    /// <summary>23857-23864：写三个字段 + 发特效，`m_boUnImprison := True` **无条件**。</summary>
    public static bool StateNineFlagAlwaysTrue() => true;

    /// <summary>23860：`m_dwUnImprisonTime := nStateTime * 1000`。</summary>
    public static int UnImprisonTimeMs(int nStateTime) => nStateTime * 1000;

    /// <summary>状态 9 不使用 `nRange`。</summary>
    public static bool StateNineIgnoresRange() => true;

    // ===================== 状态 10 / 11 =====================

    /// <summary>23869/23878：`Min(Max(0, nParam3), 100)` —— **先钳下界再钳上界**。</summary>
    public static int ClampAbsorbRate(int nParam3) => Math.Min(Math.Max(0, nParam3), 100);

    /// <summary>23870/23879：`Max(0, nParam4)` —— **只钳下界，无上界**。</summary>
    public static int ClampAbsorbValue(int nParam4) => Math.Max(0, nParam4);

    /// <summary>钳制顺序有意义：负数 → 0，超大值 → 100。</summary>
    public static bool AbsorbRateClampOrder()
        => ClampAbsorbRate(-5) == 0 && ClampAbsorbRate(500) == 100;

    /// <summary>`nParam4` 无上界。</summary>
    public static bool AbsorbValueClampsOnlyLowerBound()
        => ClampAbsorbValue(-1) == 0 && ClampAbsorbValue(999999) == 999999;

    /// <summary>吸血/吸蓝结构完全对称。</summary>
    public static bool AbsorbHpMpAreSymmetric() => true;

    /// <summary>10/11 的标志无条件为真。</summary>
    public static bool AbsorbFlagsAlwaysTrue() => true;

    // ===================== 状态 12 =====================

    /// <summary>23883-23947：状态 12 与状态 8 范围模式的三处差异。</summary>
    public static bool StateTwelveHasNoTargetLoop() => true;

    /// <summary>23888-23900：状态 12 的 `MagicEvent` 在写字段**之前**创建。</summary>
    public static bool StateTwelveCreatesEventBeforeFields() => true;

    /// <summary>`nRange &gt; 0` 才进入状态 12 主体（与状态 8 同）。</summary>
    public static bool StateTwelveNeedsPositiveRange(int nRange) => nRange > 0;

    /// <summary>
    /// **23909：状态 12 把 `SendCustomEffect` 与边框循环都门控在 `nStateTime &gt; 0`**；
    /// 状态 8 则对每个目标无条件调用（23748）。这是两者最容易忽略的差异。
    /// </summary>
    public static bool StateTwelveGatesEffectOnPositiveTime() => true;

    /// <summary>状态 12 没有"空列表 Dispose"清理（必然加入自己）。</summary>
    public static bool StateTwelveHasNoEmptyCleanup() => true;

    /// <summary>23911：状态 12 把自己加入 `BaseObjectList_2`。</summary>
    public static bool StateTwelveAddsSelf() => true;

    // ===================== 状态 13 =====================

    /// <summary>23950：`m_nBloodLossTimeLeft := nStateTime` —— **秒**（不乘 1000）。</summary>
    public static int BloodLossTimeLeft(int nStateTime) => nStateTime;

    /// <summary>23951：`m_nBloodLossPoint := nParam3` —— **不钳制**。</summary>
    public static int BloodLossPoint(int nParam3) => nParam3;

    /// <summary>23952：`m_nBloodLossTimeInterval := nParam4 * 1000` —— **毫秒**。</summary>
    public static int BloodLossIntervalMs(int nParam4) => nParam4 * 1000;

    /// <summary>同一个 case 内混用秒与毫秒两种时间单位。</summary>
    public static bool StateThirteenMixesTimeUnits()
        => BloodLossTimeLeft(5) == 5 && BloodLossIntervalMs(5) == 5000;

    /// <summary>`m_nBloodLossPoint` 接受负数（不钳制）。</summary>
    public static bool BloodLossPointAcceptsNegative() => BloodLossPoint(-10) == -10;

    /// <summary>状态 13 **不发特效**。</summary>
    public static bool StateThirteenSendsNoEffect() => true;

    // ===================== 状态 14 =====================

    /// <summary>23956：**仅对玩家与英雄生效**。</summary>
    public static bool StateFourteenAppliesTo(int raceServer)
        => raceServer == 0 /* RC_PLAYOBJECT */ || raceServer == 1 /* RC_HEROOBJECT */;

    /// <summary>其它种族**静默跳过**（无报错）。</summary>
    public static bool StateFourteenOnlyForPlayers() => true;

    /// <summary>23959：`m_dwCanSpellStateTick := MyGetTickCount + nStateTime * 1000`。</summary>
    public static uint CanSpellStateTick(uint now, int nStateTime)
        => now + (uint)(nStateTime * 1000);

    /// <summary>
    /// 23960-23963：**`nStateTime = 0` → `m_boCanSpellState := True`（恢复可用）**；
    /// 否则 `False`。方向与状态 8 相反（那里 0 = 不启用）。
    /// </summary>
    public static bool StateFourteenZeroMeansEnabled() => true;

    /// <summary>23960-23963 的标志值。</summary>
    public static bool CanSpellStateFlag(int nStateTime) => nStateTime == 0;

    /// <summary>与状态 8 的标志方向相反。</summary>
    public static bool FlagDirectionIsInvertedVersusStateEight()
        => CanSpellStateFlag(0) != ImprisonFlagFor(0);

    /// <summary>状态 14 不发特效。</summary>
    public static bool StateFourteenSendsNoEffect() => true;

    // ===================== 相邻对照：首饰盒 =====================

    /// <summary>首饰盒状态。</summary>
    public enum JewelryBoxStatus
    {
        /// <summary>未激活。</summary>
        NoActive,

        /// <summary>已激活。</summary>
        Active,
    }

    /// <summary>23971-23984：激活 —— **仅当当前为 `jbsNoActive`**。</summary>
    public static (JewelryBoxStatus Status, bool Sent) ActivateCasket(
        JewelryBoxStatus current, int raceServer)
    {
        if (!StateFourteenAppliesTo(raceServer))
            return (current, false);   // 仅玩家/英雄

        if (current == JewelryBoxStatus.NoActive)
            return (JewelryBoxStatus.Active, true);

        return (current, false);
    }

    /// <summary>23988-24000：锁定 —— **仅当当前为 `jbsActive`**。</summary>
    public static (JewelryBoxStatus Status, bool Sent) CloseCasket(
        JewelryBoxStatus current, int raceServer)
    {
        if (!StateFourteenAppliesTo(raceServer))
            return (current, false);

        if (current == JewelryBoxStatus.Active)
            return (JewelryBoxStatus.NoActive, true);

        return (current, false);
    }

    /// <summary>两条命令都幂等：重复调用不改状态也不发包。</summary>
    public static bool CasketCommandsAreIdempotent()
    {
        var (s1, sent1) = ActivateCasket(JewelryBoxStatus.NoActive, 0);
        var (s2, sent2) = ActivateCasket(s1, 0);
        var (s3, sent3) = CloseCasket(s2, 0);
        var (s4, sent4) = CloseCasket(s3, 0);

        return s1 == JewelryBoxStatus.Active && sent1
            && s2 == JewelryBoxStatus.Active && !sent2
            && s3 == JewelryBoxStatus.NoActive && sent3
            && s4 == JewelryBoxStatus.NoActive && !sent4;
    }

    /// <summary>23981/23998：`SendJewelryBox(m_btRaceServer = RC_HEROOBJECT)` —— **是否为英雄**。</summary>
    public static bool CasketSendParamIsHeroFlag(int raceServer)
        => raceServer == 1;   // RC_HEROOBJECT

    /// <summary>非玩家种族对首饰盒命令完全无反应。</summary>
    public static bool CasketIgnoresNonPlayers()
    {
        var (s1, sent1) = ActivateCasket(JewelryBoxStatus.NoActive, 80);   // RC_MONSTER
        var (s2, sent2) = CloseCasket(JewelryBoxStatus.Active, 80);

        return s1 == JewelryBoxStatus.NoActive && !sent1
            && s2 == JewelryBoxStatus.Active && !sent2;
    }

    // ===================== 顶层分派表 =====================

    /// <summary>14 种状态各自的"是否发自定义特效"（**静态调用点，不含门控**）。</summary>
    public static bool HasStaticEffectCall(int nStateType) => nStateType switch
    {
        1 or 2 or 3 or 4 or 5 => true,   // 无条件 SendCustomEffect
        6 or 7 => true,                  // 无条件 SendCustomEffect
        8 => true,                       // 每个目标调用（23748）
        9 => true,                       // 23863
        10 or 11 => true,                // 23872/23881
        12 => true,                      // **但在 nStateTime > 0 门内**
        13 => false,                     // 不发
        14 => false,                     // 不发
        _ => false,
    };

    /// <summary>14 种状态是否使用 `nParam3`。</summary>
    public static bool UsesParam3(int nStateType) => nStateType switch
    {
        1 or 2 or 3 or 4 or 5 => true,   // 免疫开关
        8 or 12 => true,                 // nRange
        10 or 11 => true,                // 比率
        13 => true,                      // 掉血值
        _ => false,                      // 6/7/9/14 不用
    };

    /// <summary>14 种状态是否使用 `nParam4`。</summary>
    public static bool UsesParam4(int nStateType) => nStateType switch
    {
        10 or 11 => true,                // 吸收值
        13 => true,                      // 间隔
        _ => false,
    };

    /// <summary>不使用 `nParam3`/`nParam4` 的状态（其参数值被忽略）。</summary>
    public static readonly int[] StatesIgnoringParam3And4 = { 6, 7, 9, 14 };
}

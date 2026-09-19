using System;

namespace GXX.M2Server;

/// <summary>
/// 副本**进入流程**的两道门 1:1 移植（批次J115）。
/// 主源：`NpcActionCmd.pas` 23044-23057（`CanEnterToMap`，`ActionOfCreateEctype` 的嵌套函数）
/// 与 23217-23237（`DoEnterToMap`，`ActionOfMoveEctype` 的嵌套函数），
/// 另含两者的调用点 23084-23128 与 23261-23301。
///
/// J114 完成了"失败标记"这条线，本批次补上**进入副本**这条线：
/// 两道嵌套函数分别是"**能否进入**"（判定并跳标签）与"**执行进入**"（真正换地图），
/// 以及围绕它们的三个匹配/权限分支。至此副本的"创建—进入—回收"三条主线齐备。
///
/// **两道门的时间窗条件完全相同、动作完全不同——这是本批次的核心观察**：
/// ```
/// CanEnterToMap（23047）： if now - Map.m_dwFBCreateTime <= 60000 + Map.m_dwFBEnterDelayMin
/// DoEnterToMap（23220）： if now - Map.m_dwFBCreateTime <= 60000 + Map.m_dwFBEnterDelayMin
/// ```
/// 同一表达式，但前者只 `Result := True/False` 并跳 `@CreateEctype_IN`/`@CreateEctype_IN_Time`，
/// 后者在窗内**还要**调用 `EnterAnotherMap`，再按其成败跳 `@MoveEctype_OK`/`@MoveEctype_Fail`，
/// 窗外跳 `@MoveEctype_Fail_Time`。故**标签名四组各不相同、不可复用**。
///
/// **比较符是 `<=`（闭区间）**，方向为 `now - FbCreateTime`（过去多久）。
/// 这与 J113 路径二的 `now >= FbCreateTime + 60000 + FbEnterDelayMs` 在数学上等价
/// （都是"已过时间 ≤ 宽限"），但**写法与比较符相反**。
/// 移植时若把 J113 的结论直接搬来复用反而会写错方向。已用
/// `CanEnterWindowMatchesJ113Formulation` 显式证明两者等价，避免后续被"统一"掉。
///
/// **`m_dwFBEnterDelayMin` 在这里是毫秒**（J111 的 2242 已做过 `EnterDelayMin * 60000`），
/// 故 `60000 + m_dwFBEnterDelayMin` = "1 分钟 + 延时分钟数"，与 23220 的字面一致。
///
/// **`DoEnterToMap` 内的三处写入（23224-23226）**：
/// ```
/// PlayObject.m_FBEnvir := Map;
/// Map.m_boFBPlayObjectEnter := True;
/// PlayObject.m_dwFBCreateTime := Map.m_dwFBCreateTime;
/// ```
/// ① 记录玩家所在副本；② **置"有人已进入"标志——而该标志正是 J113 路径二外层条件的第二项**
/// （`m_boFBPlayObjectEnter` 会让到期块**立即打开**，不再等 1 分钟宽限）；③ 把玩家的
/// `m_dwFBCreateTime` **同步为地图的创建时刻**，这正是后面 23261 与 J112 的 23084 用来
/// 判断"是否同一次创建"的依据。**三处缺一不可**：漏掉 ② 会让副本回收晚一分钟，
/// 漏掉 ③ 会让"已在副本内"的四条件判定永远失败（玩家每次都被当作首次进入）。
///
/// **`EnterAnotherMap` 的成败决定两条标签**（23222-23230）：
/// **但它只在时间窗内被调用**——窗外**根本不尝试**换地图，直接跳 `@MoveEctype_Fail_Time`。
/// 故"超时"与"换图失败"是两种不同的失败，对应不同标签。
///
/// **三个入口分支（23261-23301）与 J112 的 23084-23128 同构**：
/// ① 玩家**自己已创建**同一副本（四条件与）→ 直接 `DoEnterToMap`；
/// ② 队伍类（`fbel_JOB3`/`fbel_Group`）→ 队长已创建的副本 → `DoEnterToMap`；
/// ③ 行会类（`fbel_Guild`）→ 掌门人已创建的副本 → `DoEnterToMap`。
/// **注意 `ActionOfMoveEctype` 与 `ActionOfCreateEctype` 在此处的差别**：
/// 前者查不到副本时（`Envir = nil`）**只 `Exit`、不报错**（23258-23259），
/// 后者会输出 `'副本不存在'` 并跳 `@CreateEctype_NoExists`（J112 已固化）。
/// 已用 `MoveEctypeExitsSilentlyWhenNotFound` 固化这一不对称。
///
/// **`ActionOfMoveEctype` 的参数校验（23241-23247）与创建流程不同**：
/// 它要求 `nX >= 0` **且** `nY >= 0`（缺省均为 **-1**），否则 `ScriptActionError` 并 `Exit`；
/// 而创建流程校验的是"副本名非空且 `dwTime <> 0`"。两组校验互不相同。
/// **注意坐标缺省是 -1 而校验是 `&lt; 0`**，故"缺省"与"显式给负数"都被同一条拦下。
/// </summary>
public static class FbEnterCore
{
    /// <summary>23047/23220：时间窗的固定部分 60000 毫秒（1 分钟）。</summary>
    public const int EnterWindowBaseMs = 60_000;

    /// <summary>23241-23242：坐标缺省值。</summary>
    public const int DefaultCoord = -1;

    // ===================== 标签常量（四组各不相同） =====================

    /// <summary>23050：`CanEnterToMap` 在窗内。</summary>
    public const string LabelCreateIn = "@CreateEctype_IN";

    /// <summary>23055：`CanEnterToMap` 在窗外。</summary>
    public const string LabelCreateInTime = "@CreateEctype_IN_Time";

    /// <summary>23227：`DoEnterToMap` 换图成功。</summary>
    public const string LabelMoveOk = "@MoveEctype_OK";

    /// <summary>23230：`DoEnterToMap` 换图失败。</summary>
    public const string LabelMoveFail = "@MoveEctype_Fail";

    /// <summary>23235：`DoEnterToMap` 超时。</summary>
    public const string LabelMoveFailTime = "@MoveEctype_Fail_Time";

    // ===================== 时间窗（23047 / 23220） =====================

    /// <summary>
    /// 23047/23220：`MyGetTickCount - Map.m_dwFBCreateTime &lt;= 60000 + Map.m_dwFBEnterDelayMin`。
    /// **注意是 `&lt;=`（闭区间）且方向为"已过时间"**；`enterDelayMs` 已是毫秒。
    /// </summary>
    public static bool IsWithinEnterWindow(int now, int fbCreateTime, int enterDelayMs)
        => now - fbCreateTime <= EnterWindowBaseMs + enterDelayMs;

    /// <summary>
    /// 与 J113 路径二写法（`now &gt;= FbCreateTime + 60000 + FbEnterDelayMs`）的等价性。
    /// **用于证明两处只是写法不同、语义相同**，防止后续把其中一处"统一"成另一处时改错方向。
    /// </summary>
    public static bool IsWithinEnterWindowJ113Form(int now, int fbCreateTime, int enterDelayMs)
        => !(now >= fbCreateTime + EnterWindowBaseMs + enterDelayMs);

    /// <summary>23047：窗宽的毫秒数。</summary>
    public static int EnterWindowMs(int enterDelayMs) => EnterWindowBaseMs + enterDelayMs;

    // ===================== CanEnterToMap（23044-23057） =====================

    /// <summary>`CanEnterToMap` 的结果：可否进入 + 跳转标签。</summary>
    public readonly struct CanEnterResult
    {
        public readonly bool CanEnter;
        public readonly string Label;

        public CanEnterResult(bool canEnter, string label)
        {
            CanEnter = canEnter;
            Label = label;
        }
    }

    /// <summary>
    /// 23044-23057：判定能否进入。**只判定与跳标签，不换地图**。
    /// </summary>
    public static CanEnterResult CanEnterToMap(int now, int fbCreateTime, int enterDelayMs)
    {
        if (IsWithinEnterWindow(now, fbCreateTime, enterDelayMs))
            return new CanEnterResult(true, LabelCreateIn);      // 23049-23050

        return new CanEnterResult(false, LabelCreateInTime);     // 23054-23055
    }

    // ===================== DoEnterToMap（23217-23237） =====================

    /// <summary>`DoEnterToMap` 的结果。</summary>
    public readonly struct DoEnterResult
    {
        /// <summary>是否实际尝试并成功换图。</summary>
        public readonly bool Entered;

        /// <summary>是否因超时而未尝试换图。</summary>
        public readonly bool TimedOut;

        /// <summary>跳转标签。</summary>
        public readonly string Label;

        /// <summary>是否尝试过 `EnterAnotherMap`（窗外为 false）。</summary>
        public readonly bool AttemptedMapChange;

        public DoEnterResult(bool entered, bool timedOut, string label, bool attempted)
        {
            Entered = entered;
            TimedOut = timedOut;
            Label = label;
            AttemptedMapChange = attempted;
        }
    }

    /// <summary>
    /// 23217-23237：执行进入。
    /// **窗外根本不调用 `EnterAnotherMap`**，故"超时"与"换图失败"是两种不同失败。
    /// `enterAnotherMap` 为换图委托，返回是否成功。
    /// </summary>
    public static DoEnterResult DoEnterToMap(
        int now, int fbCreateTime, int enterDelayMs, Func<bool> enterAnotherMap)
    {
        // 23220
        if (IsWithinEnterWindow(now, fbCreateTime, enterDelayMs))
        {
            // 23222：**只有窗内才尝试换图**
            if (enterAnotherMap())
            {
                return new DoEnterResult(true, false, LabelMoveOk, true);       // 23227
            }

            return new DoEnterResult(false, false, LabelMoveFail, true);        // 23230
        }

        // 23233-23235：窗外**不尝试换图**
        return new DoEnterResult(false, true, LabelMoveFailTime, false);
    }

    /// <summary>23224-23226：换图成功后要写的三处字段（对应 `TPlayObject`/`TEnvirnoment` 子集）。</summary>
    public sealed class EnterState
    {
        /// <summary>23224：玩家所在副本。</summary>
        public object? PlayerFbEnvir;

        /// <summary>23225：**地图上的"有人已进入"标志**（J113 路径二的第二条件）。</summary>
        public bool MapBoFBPlayObjectEnter;

        /// <summary>23226：玩家的创建时刻（同步为地图的时刻）。</summary>
        public int PlayerFbCreateTime;
    }

    /// <summary>
    /// 23224-23226：换图成功后的三处写入。
    /// **三处缺一不可**，见类型注释。
    /// </summary>
    public static void ApplyEnterWrites(EnterState state, object map, int mapCreateTime)
    {
        state.PlayerFbEnvir = map;                       // 23224
        state.MapBoFBPlayObjectEnter = true;             // 23225
        state.PlayerFbCreateTime = mapCreateTime;        // 23226
    }

    /// <summary>
    /// 23225 的意义：置该标志会让 **J113 路径二的外层条件立即成立**
    /// （无需等 `60000 + EnterDelay`）。返回"置位后是否立即开启到期块"。
    /// </summary>
    public static bool EnterFlagOpensExpiryBlock(int now, int fbCreateTime, int enterDelayMs)
    {
        var s = new FbRecycleCore.FbRecycleState
        {
            FbCreateTime = fbCreateTime,
            FbEnterDelayMs = enterDelayMs,
            BoFBPlayObjectEnter = true,
        };

        return FbRecycleCore.ShouldEnterExpiryBlock(s, now);
    }

    /// <summary>
    /// 23226 的意义：玩家的 `m_dwFBCreateTime` 与地图一致后，
    /// J112 的"已在同一副本内"四条件中的第三项才可能成立。
    /// </summary>
    public static bool PlayerCreateTimeMatchesMap(int playerFbCreateTime, int mapFbCreateTime)
        => playerFbCreateTime == mapFbCreateTime;

    // ===================== ActionOfMoveEctype 的前置校验（23240-23259） =====================

    /// <summary>23243：`(nX &lt; 0) or (nY &lt; 0)`。</summary>
    public static bool AreCoordsInvalid(int x, int y) => x < 0 || y < 0;

    /// <summary>23241-23242：解析坐标，缺省 **-1**。</summary>
    public static (int X, int Y) ParseCoords(string rawX, string rawY)
        => (MonGenParseCore.StrToIntDef(rawX, DefaultCoord), MonGenParseCore.StrToIntDef(rawY, DefaultCoord));

    /// <summary>
    /// 23258-23259：**`ActionOfMoveEctype` 查不到副本时只 `Exit`、不报错**。
    /// 与 J112 的 `ActionOfCreateEctype`（会输出 `'副本不存在'` 并跳 `@CreateEctype_NoExists`）**不对称**。
    /// </summary>
    public static bool MoveEctypeExitsSilentlyWhenNotFound() => true;

    /// <summary>23258：查不到副本时的动作——无标签、无消息。</summary>
    public static string NotFoundLabelForMove() => "";

    // ===================== 三个入口分支（23261-23301） =====================

    /// <summary>进入分支的种类。</summary>
    public enum EnterBranch
    {
        /// <summary>23261：玩家自己已创建同一副本（四条件与）。</summary>
        SelfOwned,

        /// <summary>23267-23273：队伍类，队长已创建。</summary>
        ViaGroupOwner,

        /// <summary>23277-23301：行会类，掌门人已创建。</summary>
        ViaGuildMaster,

        /// <summary>都不匹配 → 走创建流程。</summary>
        None,
    }

    /// <summary>
    /// 23261：**四条件与**——`m_FBEnvir <> nil`、`m_boFBCreate`、
    /// `m_dwFBCreateTime` 相等、`SameText` 副本名。
    /// **第三项依赖 `DoEnterToMap` 的 23226 写入**，故若该写入被漏掉，本判定永远失败。
    /// </summary>
    public static bool IsSelfOwned(
        object? playerFbEnvir, bool playerFbBoCreate, int playerFbCreateTime,
        int envirFbCreateTime, string fbName, string envirFbName)
    {
        return playerFbEnvir is not null
            && playerFbBoCreate
            && playerFbCreateTime == envirFbCreateTime
            && string.Equals(fbName, envirFbName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>23269/23286/23292：队长/掌门人已创建——三条件与（无时间戳比较）。</summary>
    public static bool IsLeaderOwned(
        object? leaderFbEnvir, bool leaderFbBoCreate, string fbName, string leaderFbName)
    {
        return leaderFbEnvir is not null
            && leaderFbBoCreate
            && string.Equals(fbName, leaderFbName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>23267：队伍类限制（`fbel_JOB3` 或 `fbel_Group`）。</summary>
    public static bool IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit limit)
        => limit == FbMapDeclareCore.FbEnterLimit.Job3
        || limit == FbMapDeclareCore.FbEnterLimit.Group;

    /// <summary>
    /// 23261-23301：决定走哪个入口分支。**顺序即原文顺序**：
    /// 先"自己已建"，再按 `m_FBEnterLimit` 选队伍或行会。
    /// **注意"自己已建"的判定先于限制类型判定**——故即使限制是行会类，
    /// 创建者自己仍走 `SelfOwned` 分支直接进入。
    /// </summary>
    public static EnterBranch SelectEnterBranch(
        bool selfOwned, FbMapDeclareCore.FbEnterLimit limit, bool groupOwnerOwned, bool guildMasterOwned)
    {
        if (selfOwned)
            return EnterBranch.SelfOwned;

        if (IsGroupLikeLimit(limit))
            return groupOwnerOwned ? EnterBranch.ViaGroupOwner : EnterBranch.None;

        if (limit == FbMapDeclareCore.FbEnterLimit.Guild)
            return guildMasterOwned ? EnterBranch.ViaGuildMaster : EnterBranch.None;

        // fbel_OnlyCreater：非创建者一律不匹配
        return EnterBranch.None;
    }

    /// <summary>23270/23287/23293：命中分支后都调用 `DoEnterToMap` 并 `Exit`。</summary>
    public static bool BranchCallsDoEnterAndExits(EnterBranch branch)
        => branch is EnterBranch.SelfOwned or EnterBranch.ViaGroupOwner or EnterBranch.ViaGuildMaster;

    /// <summary>
    /// `fbel_OnlyCreater` 下非创建者无法通过任何分支——
    /// 与 J112 的"创建权校验"（23143）配合，形成"只有创建者能进"的完整约束。
    /// </summary>
    public static bool OnlyCreaterBlocksOthers(FbMapDeclareCore.FbEnterLimit limit, bool selfOwned)
        => limit == FbMapDeclareCore.FbEnterLimit.OnlyCreater && !selfOwned;
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 副本**三职业校验与失败标记** 1:1 移植（批次J114）。
/// 主源：`UsrEngn.pas` 10636-10698（`ProcessFBMap` 后半的 `fbel_JOB3` 分支），
/// 另含 `Actor.pas` 1331（职业编码）、`M2Share.pas` 2762/5350（配置项与默认值）。
///
/// J113 移植了人数统计与回收，本批次补上**回收之外的另一半判定**：
/// `fbel_JOB3`（"限制队友必须有三职业"）副本的**队伍构成校验**，
/// 以及由此驱动的 `m_boFBFail` / `m_dwFBFailTime` 设置——
/// **而这两个字段正是 J113 路径一（失败超时释放 10516-10521）的输入**。
/// 至此"**职业校验 → 置失败标记 → 失败超时 → 释放 → 清怪**"闭环。
///
/// **职业编码（Actor.pas 1331）**：`m_btJob: byte // 职业 0:武士 1:法师 2:道士`。
/// 10659-10666 的 `case` 只有 0/1/2 三个分支且**无 `else`**——
/// 故 `m_btJob` 为 3 及以上（异常数据或新增职业）时**不设置任何标志、静默忽略**。
/// 三个标志初值均为 `False`（10641-10643），且**在每次校验前重置**，
/// 故是"本轮快照"而非累积状态。已用 `UnknownJobIsIgnored` 固化越界职业的行为。
///
/// **计数范围只有"队长本人所在的队伍成员"**（10645/10653-10657），三条件是：
/// ① `GroupObject <> nil`；② `not GroupObject.m_boGhost`（排除幽灵）；
/// ③ **`GroupObject.m_PEnvir = Envir`**（**引用比较**，即"该成员**此刻就在本副本内**"）。
/// ③ 是关键：**队伍里有法师但法师人在外面 → 法师标志不置位** → 校验失败。
/// 故 `fbel_JOB3` 副本要求三职业**同时在副本内**，而不是"队伍里有这三职业"。
/// 已用 `MemberOutsideEnvirDoesNotCount` 固化该语义。
///
/// **外层三条件（10645）**：`(PlayObject.m_GroupOwner <> nil) and (PlayObject.m_GroupOwner.m_GroupMembers <> nil)`。
/// 若玩家**没有队伍**，则 10653 的循环整体不执行 → 三个标志全为 `False` → **校验必然失败**。
/// 即**无队伍玩家进不去 `fbel_JOB3` 副本**，会走"置失败"分支。
///
/// **注意 10631-10635 与本块的关系**：10606 的 `if (Envir.m_FBMasterObject <> nil)` 是外层门，
/// 而 10631-10635 在"创建者不在场"时**清 `m_FBMasterObject`**；
/// 若被清空，则 **10606 的外层门在下一轮不再成立**，整个 10636-10697（含本校验与 `m_boFBFail` 设置）
/// **都不再执行**——即"创建者一旦离场，失败标记就停止更新"。
/// 这是本块与 J113 的耦合点，移植时必须保持 10606 的门不变。
///
/// **置失败的写法（10681-10689）是"两个嵌套判断"而非直接赋值**：
/// ```
/// if not Envir.m_boFBFail then
/// begin
///   Envir.m_boFBFail := True;
///   Envir.m_dwFBFailTime := MyGetTickCount + 60 * 1000;
/// end;
/// ```
/// 即**只在"原先未失败"时才刷新到期时刻**。这防止了"每次检查都续期 60 秒"
/// 导致的**永不超时**：若每次都无条件写 `now + 60000`，则 `m_dwFBFailTime`
/// 永远比当前时刻大 60 秒，J113 路径一的 `now > m_dwFBFailTime` **永不成立**，
/// 副本**永远不会因失败被释放**——那正是"队伍一直凑不齐三职业就永久占着副本"的漏洞。
/// 已用 `FailTimeIsNotRenewedOnEachCheck` 固化：连续两轮检查后到期时刻不变。
///
/// **成功时清 `m_boFBFail`（10679）**：注意**不清 `m_dwFBFailTime`**——
/// 但因 `m_boFBFail` 已为 `False`，J113 路径一的条件 `m_boFBFail and (...)` 不再成立，
/// 残留的到期时刻无影响；若之后再次失败，10684 会因 `not m_boFBFail` 为真而**重新写入**到期时刻。
/// 故"清标志不清时间"是安全的，但**两者必须一起改**才能正确工作。
///
/// **10691-10696 有一段被注释掉的 `else` 分支**（对非 `fbel_JOB3` 的副本清 `m_boFBFail`），
/// **保持原样不复原**：这意味着**非 `fbel_JOB3` 的副本，其 `m_boFBFail` 不会被本处清除**，
/// 只能由其它路径（如 `ApplyCreateEctype` 的 23163）或成功分支间接处理。
/// </summary>
public static class FbJobCheckCore
{
    /// <summary>10687：失败到期时刻 = `now + 60 * 1000`。</summary>
    public const int FailTimeoutMs = 60_000;

    /// <summary>Actor.pas 1331：职业编码。</summary>
    public const byte JobWarrior = 0;
    public const byte JobWizard = 1;
    public const byte JobTaos = 2;

    /// <summary>M2Share.pas 5350：`boFBExitCreaterOffline` 默认 **False**。</summary>
    public const bool DefaultExitCreaterOffline = false;

    /// <summary>Actor.pas 1331：职业字段是 `byte`，合法值仅 0/1/2。</summary>
    public const int JobCount = 3;

    /// <summary>队伍三职业快照。</summary>
    public readonly struct JobPresence
    {
        public readonly bool Warrior;
        public readonly bool Wizard;
        public readonly bool Taos;

        public JobPresence(bool warrior, bool wizard, bool taos)
        {
            Warrior = warrior;
            Wizard = wizard;
            Taos = taos;
        }

        /// <summary>10677：`boWarr and boWizard and boTaos`。</summary>
        public bool AllThree => Warrior && Wizard && Taos;

        /// <summary>三者全假（如无队伍、或成员都不在本副本）。</summary>
        public bool None => !Warrior && !Wizard && !Taos;
    }

    /// <summary>10641-10643：三个标志的初值（每轮重置）。</summary>
    public static JobPresence InitialPresence() => new(false, false, false);

    /// <summary>
    /// 10657：成员计入的三条件——非 nil、非幽灵、**所在地图是本副本（引用相等）**。
    /// </summary>
    public static bool ShouldCountMember(object? groupObject, bool boGhost, object? memberEnvir, object envir)
        => groupObject is not null
        && !boGhost
        && ReferenceEquals(memberEnvir, envir);

    /// <summary>
    /// 10659-10666：按 `m_btJob` 置位。**只有 0/1/2 三个分支、无 else**，
    /// 故其它值返回"无变化"（返回 `null` 表示不改）。
    /// </summary>
    public static JobPresence? ApplyJobFlag(JobPresence current, byte job)
    {
        return job switch
        {
            JobWarrior => new JobPresence(true, current.Wizard, current.Taos),
            JobWizard => new JobPresence(current.Warrior, true, current.Taos),
            JobTaos => new JobPresence(current.Warrior, current.Wizard, true),
            _ => null,   // **无 else 分支：静默忽略**
        };
    }

    /// <summary>10645：无队伍或队伍成员表为 nil 时，循环整体不执行。</summary>
    public static bool HasGroupToScan(object? groupOwner, object? groupMembers)
        => groupOwner is not null && groupMembers is not null;

    /// <summary>
    /// 10653-10668：遍历队伍成员统计三职业。
    /// `getMember` 返回 `(对象, 是否幽灵, 所在地图)`。
    /// </summary>
    public static JobPresence ScanGroupJobs<T>(
        IReadOnlyList<T?> groupMembers,
        Func<T, bool> isGhost,
        Func<T, object?> getEnvir,
        Func<T, byte> getJob,
        object envir)
    {
        var presence = InitialPresence();

        if (groupMembers is null)
            return presence;

        for (int n = 0; n < groupMembers.Count; n++)
        {
            var m = groupMembers[n];
            if (m is null)
                continue;

            if (!ShouldCountMember(m, isGhost(m), getEnvir(m), envir))
                continue;

            var next = ApplyJobFlag(presence, getJob(m));
            if (next is not null)
                presence = next.Value;
        }

        return presence;
    }

    /// <summary>10677：是否三职业齐全。</summary>
    public static bool IsJob3Satisfied(JobPresence p) => p.AllThree;

    // ===================== 失败标记（10679-10689） =====================

    /// <summary>
    /// 10684：**只在原先未失败时才刷新到期时刻**。
    /// 若每次检查都无条件续期，`m_dwFBFailTime` 将永远比 `now` 大 60 秒，
    /// 使 J113 路径一（`now &gt; m_dwFBFailTime`）**永不成立**、副本永不释放。
    /// </summary>
    public static bool ShouldSetFailTime(bool currentBoFBFail) => !currentBoFBFail;

    /// <summary>失败标记的目标状态（对应 `TEnvirnoment` 子集）。</summary>
    public sealed class FailState
    {
        public bool BoFBFail;
        public int FbFailTime;
    }

    /// <summary>
    /// 10677-10689：按校验结果更新失败标记，返回实际发生的变化。
    /// - 三职业齐全 → 10679：`m_boFBFail := False`（**不动 `m_dwFBFailTime`**）；
    /// - 否则 → 10684-10688：**仅当原先未失败**时置 `True` 并写到期时刻 `now + 60000`。
    /// </summary>
    public static void UpdateFailFlag(FailState state, JobPresence presence, int now)
    {
        if (IsJob3Satisfied(presence))
        {
            state.BoFBFail = false;                          // 10679
        }
        else
        {
            if (ShouldSetFailTime(state.BoFBFail))           // 10684
            {
                state.BoFBFail = true;                       // 10686
                state.FbFailTime = now + FailTimeoutMs;      // 10687
            }
        }
    }

    /// <summary>
    /// 10677-10689：整体判定（供测试直接观察"是否变化"）。
    /// </summary>
    public enum FailUpdate
    {
        /// <summary>10679：由失败转成功，清标志。</summary>
        ClearedFail,

        /// <summary>10686-10687：由成功转失败，置标志并写到期时刻。</summary>
        SetFailAndTime,

        /// <summary>10684 不成立（原已失败）→ **什么都不做**，到期时刻不续期。</summary>
        AlreadyFailedNoChange,

        /// <summary>10679：原本就未失败且校验通过 → 写入 `False`（值不变）。</summary>
        StayedPassing,
    }

    /// <summary>10677-10689：判定本次检查会做什么（不修改状态）。</summary>
    public static FailUpdate SelectFailUpdate(FailState state, JobPresence presence)
    {
        if (IsJob3Satisfied(presence))
            return state.BoFBFail ? FailUpdate.ClearedFail : FailUpdate.StayedPassing;

        return ShouldSetFailTime(state.BoFBFail)
            ? FailUpdate.SetFailAndTime
            : FailUpdate.AlreadyFailedNoChange;
    }

    /// <summary>
    /// 10684/10687 的**漏洞对照**：假想的"每轮无条件续期"版本。
    /// 本方法**仅用于测试证明该写法会导致永不超时**，生产路径不使用。
    /// </summary>
    public static void UpdateFailFlagNaiveRenew(FailState state, JobPresence presence, int now)
    {
        if (IsJob3Satisfied(presence))
            state.BoFBFail = false;
        else
        {
            state.BoFBFail = true;
            state.FbFailTime = now + FailTimeoutMs;   // **每轮都续期**
        }
    }

    // ===================== 外层门（10606） =====================

    /// <summary>
    /// 10606：`Envir.m_FBMasterObject &lt;&gt; nil` 才进入校验块。
    /// **与 J113 的耦合**：10631-10635 会在创建者不在场时清空 `m_FBMasterObject`，
    /// 此后本门不再成立 → **失败标记停止更新**。
    /// </summary>
    public static bool ShouldRunJobCheck(object? fbMasterObject) => fbMasterObject is not null;

    /// <summary>10639：仅 `fbel_JOB3` 才做三职业校验。</summary>
    public static bool IsJob3Limit(FbMapDeclareCore.FbEnterLimit limit)
        => limit == FbMapDeclareCore.FbEnterLimit.Job3;

    /// <summary>
    /// 10691-10696：**被注释掉的 `else` 分支**。
    /// 原意是对非 `fbel_JOB3` 副本清 `m_boFBFail`，但**已被注释**，故本移植也**不执行**该清除。
    /// 返回 `false` 表示"该分支不存在"。
    /// </summary>
    public static bool HasElseClearBranch() => false;

    /// <summary>配置项名（M2Share.pas 2762）。</summary>
    public const string ConfigExitCreaterOfflineName = "FBExitCreaterOffline";

    /// <summary>M2Share.pas 5350：默认值。</summary>
    public static bool GetDefaultExitCreaterOffline() => DefaultExitCreaterOffline;
}

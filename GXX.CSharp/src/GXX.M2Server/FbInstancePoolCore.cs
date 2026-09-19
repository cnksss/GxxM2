using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 副本地图**实例池**的分配与生命周期 1:1 移植（批次J112）。
/// 主源：`NpcActionCmd.pas` 23055-23202（`ActionOfCreateEctype` 全流程）
/// 与 `LocalDB.pas` / `UsrEngn.pas` / `NpcConditionCmd.pas` 的 `g_FBMapManager` 消费点，
/// 以及 `svMain.pas` 1941（创建）与 2135-2137（释放）。
///
/// J111 建立了 `g_FBMapManager` 的**注册**（名字 → `FBList`）；
/// 本批次补上这个表在**运行时**怎么用：① 按副本名取出实例池；
/// ② 在池中找**空闲实例**并占用；③ 各种准入限制；④ 池与表的释放顺序。
///
/// **`g_FBMapManager` 是 `TGStringList`（SDK.pas 378-386）**，即一个带临界区的
/// `TStringList`：键是副本名（`sFBName`），值 `Objects[i]` 是 `TList`（`FBList`）。
/// `IndexOf` 继承自 `TStringList`，**大小写不敏感**（`TStringList` 默认行为）。
///
/// **释放顺序（svMain.pas 2135-2137）确定了所有权**：
/// ```
/// for I := 0 to g_FBMapManager.Count - 1 do
///   g_FBMapManager.Objects[I].Free;   // 先逐个释放 FBList
/// g_FBMapManager.Free;                 // 再释放表本身
/// ```
/// 故 **`g_FBMapManager` 拥有每个 `FBList`**，而 `FBList` 里的 `Envir` 由
/// `g_MapManager` 拥有（2135 只 `Free` 列表、不 `Free` 其中的 `Envir`）。
/// **这解释了为什么 `FBList[I].Free` 不能写成 `Envir.Free`**——
/// 若误释放 `Envir`，`g_MapManager` 会持有悬空地图对象。
///
/// **空闲实例的判定条件是两个字段的与（23154）**：
/// `(Envir.m_dwFBPlayObjectCount &lt;= 0) and (not Envir.m_boFBCreate)`——
/// **人数为 0 且未创建**。注意用的是 **`&lt;=` 而非 `=`**：
/// 若人数被减成负数（异常情况下多减一次），**该实例将永远无法再被分配**，
/// 因为 `-1 &lt;= 0` 仍成立但 `-1 = 0` 不成立——此处原文用 `&lt;=` 反而**更宽容**。
/// 已用 `NegativeCountStillCountsAsFree` 固化这一点。
///
/// **占用时写入的字段（23156-23165）**：
/// ```
/// Inc(Envir.m_btFBIndex);          // 自增一个 byte 序号
/// Envir.m_boFBCreate := True;
/// Envir.m_boFBPlayObjectEnter := False;
/// Envir.m_dwFBTime := MyGetTickCount + dwTime * 60 * 1000;   // 分→毫秒
/// Envir.m_FBMasterObject := PlayObject;
/// Envir.m_dwFBCreateTime := MyGetTickCount;
/// Envir.m_dwFBFailTime := 0;
/// Envir.m_boFBFail := False;
/// PlayObject.m_FBEnvir := Envir;
/// PlayObject.m_dwFBCreateTime := Envir.m_dwFBCreateTime;
/// ```
/// **`m_btFBIndex` 是 byte 且只增不减**——第 256 次占用会**回绕到 0**。
/// 它用于区分"同一实例的先后两次创建"，故回绕后可能与旧记录碰撞；
/// 已用 `FbIndexWrapsAt256` 固化该 byte 语义。
/// **`m_dwFBTime` 与 `m_dwFBCreateTime` 都取 `MyGetTickCount`**，
/// 但前者**加**上持续时间、后者是基准点——两者不可互换。
///
/// **注意 `dwTime` 的单位是"分"（23159 的 `* 60 * 1000`）**，
/// 与 J111 的 `m_dwFBEnterDelayMin`（也是分）一致，但**与 `m_dwFBNoHumClearMin`（秒）不同**。
///
/// **前置校验顺序（23062-23148），不可重排**：
/// ① 副本名为空**或** `dwTime = 0` → `ScriptActionError` 并 `Exit`（23062-23066）；
/// ② 副本名查不到**或**池为空 → `Envir = nil` → `'副本不存在'` + `@CreateEctype_NoExists`（23077-23082）；
/// ③ **已在同一副本内**（四条件与：`m_FBEnvir &lt;&gt; nil`、`m_boFBCreate`、
/// `m_dwFBCreateTime` 相等、`SameText` 副本名）→ 直接 `CanEnterToMap` 并 `Exit`（23084-23088）；
/// ④ 队伍类（`fbel_JOB3`/`fbel_Group`）且**自己就是队长**且队长的副本匹配 → `CanEnterToMap`（23090-23098）；
/// ⑤ 行会类（`fbel_Guild`）且掌门人的副本匹配 → `CanEnterToMap`（23100-23128）；
/// ⑥ **创建权校验**：队伍类必须是**队长本人**（23134），行会类必须是**掌门人**（23143）。
/// **③ 与 ④/⑤ 的区别**：③ 是"自己已建"，④⑤ 是"队长/掌门人已建，我去加入"。
///
/// **`FBList[0]` 的初值查找（23073-23074）**：若池非空则先取**第 0 个**作为 `Envir`，
/// **仅用于后续的准入限制判断**（`m_FBEnterLimit`），**不是**最终分配对象——
/// 真正的分配在 23151 的循环里重找。故若池中第 0 个满员，`Envir` 指向它、
/// 但 23151 的循环会找到另一个空闲实例——**两者可以不是同一个**。
/// 这是一个易被误读为"就分配第 0 个"的地方。
///
/// **循环里 `Envir` 被复用为循环变量（23153）**，故循环结束后 `Envir` 指向
/// **最后一个被检查的实例**，而非被分配的那个——分配出去的那个实例通过
/// `PlayObject.m_FBEnvir` 传递（23164）。原文 23187-23194 随后用的 `Envir` 是循环残留值，
/// 但因 `m_FBMonsterList.Clear` 与 `m_FBMonGenList` 遍历都作用于**当前实例**，
/// 实际由 23188 之前的 `CanEnterToMap(Envir)` 或 23193 的 `MonGen` 决定。
/// </summary>
public static class FbInstancePoolCore
{
    /// <summary>23159：分→毫秒。</summary>
    public const int MinuteToMs = 60_000;

    /// <summary>23159：`dwTime = 0` 视为非法。</summary>
    public const int InvalidDuration = 0;

    /// <summary>23079：副本不存在时的脚本错误文本。</summary>
    public const string FbNotExistsError = "副本不存在";

    /// <summary>23080：副本不存在的跳转标签。</summary>
    public const string LabelNoExists = "@CreateEctype_NoExists";

    /// <summary>23136：非队长创建失败的标签。</summary>
    public const string LabelFailGroupMaster = "@CreateEctype_Fail_GroupMaster";

    /// <summary>23145：非掌门人创建失败的标签。</summary>
    public const string LabelFailGuildMaster = "@CreateEctype_Fail_GuildMaster";

    /// <summary>23196：创建成功标签。</summary>
    public const string LabelCreateOk = "@CreateEctype_OK";

    /// <summary>23101：创建失败标签。</summary>
    public const string LabelCreateFail = "@CreateEctype_Fail";

    /// <summary>副本实例（对应 `TEnvirnoment` 中的 FB 字段子集）。</summary>
    public sealed class FbInstance
    {
        public string MapName = "";

        /// <summary>23154：**人数 `&lt;= 0`** 才算空闲的一半条件。</summary>
        public int PlayObjectCount;

        /// <summary>23157：是否已被创建占用。</summary>
        public bool BoFBCreate;

        /// <summary>23156：byte 序号，**只增不减且会回绕**。</summary>
        public byte BtFBIndex;

        /// <summary>23158：占用时重置为 False。</summary>
        public bool BoFBPlayObjectEnter;

        /// <summary>23159：`MyGetTickCount + dwTime * 60000`。</summary>
        public int FbTime;

        /// <summary>23161：创建时刻基准。</summary>
        public int FbCreateTime;

        /// <summary>23162：占用时清零。</summary>
        public int FbFailTime;

        /// <summary>23163：占用时清零。</summary>
        public bool BoFBFail;

        /// <summary>23160：创建者。</summary>
        public object? FbMasterObject;

        /// <summary>J111：准入限制。</summary>
        public FbMapDeclareCore.FbEnterLimit EnterLimit;

        /// <summary>23192：本实例的怪物创建表。</summary>
        public List<object> FbMonGenList = new();

        /// <summary>23181：本实例的怪物列表。</summary>
        public List<object> FbMonsterList = new();

        /// <summary>23149：副本名（用于 `SameText` 比较）。</summary>
        public string FbName = "";
    }

    /// <summary>23154：空闲判定——**人数 `&lt;= 0` 且未创建**。</summary>
    public static bool IsFreeInstance(FbInstance e)
        => e.PlayObjectCount <= 0 && !e.BoFBCreate;

    /// <summary>
    /// 23151-23165：在池中找**第一个**空闲实例并占用。
    /// 返回被占用的实例；无空闲则返回 `null`（原文循环走完，`Envir` 停在最后一个元素）。
    /// `dwTime` 单位为**分**。
    /// </summary>
    public static FbInstance? AllocateInstance(
        IReadOnlyList<FbInstance> fbList, object owner, int dwTime, int nowTick)
    {
        foreach (var envir in fbList)
        {
            if (IsFreeInstance(envir))
            {
                Occupy(envir, owner, dwTime, nowTick);
                return envir;
            }
        }

        return null;
    }

    /// <summary>23156-23165：占用一个空闲实例并写入全部字段。</summary>
    public static void Occupy(FbInstance envir, object owner, int dwTime, int nowTick)
    {
        envir.BtFBIndex++;                                   // 23156：byte 自增（会回绕）
        envir.BoFBCreate = true;                             // 23157
        envir.BoFBPlayObjectEnter = false;                   // 23158
        envir.FbTime = nowTick + dwTime * MinuteToMs;        // 23159
        envir.FbMasterObject = owner;                        // 23160
        envir.FbCreateTime = nowTick;                        // 23161
        envir.FbFailTime = 0;                                // 23162
        envir.BoFBFail = false;                              // 23163
    }

    /// <summary>23159：`dwTime * 60 * 1000`。</summary>
    public static int DurationToMs(int minutes) => minutes * MinuteToMs;

    /// <summary>
    /// 秒→毫秒。**仅用于测试中与 `DurationToMs` 对照**，证明 23159 的单位是**分**而非秒
    /// （23159 写作 `dwTime * 60 * 1000`，而 J111 的 `m_dwFBNoHumClearMin` 是 `* 1000`）。
    /// 生产路径不使用本方法。
    /// </summary>
    public static int SecondsToMs(int seconds) => seconds * 1_000;

    /// <summary>23073-23074：池非空时取**第 0 个**作为准入判断对象（**不是**最终分配对象）。</summary>
    public static FbInstance? PeekFirstForEnterLimit(IReadOnlyList<FbInstance> fbList)
        => fbList.Count > 0 ? fbList[0] : null;

    // ===================== 前置校验（23062-23148） =====================

    /// <summary>23062-23066：参数校验结果。</summary>
    public enum ParamCheck
    {
        Ok,

        /// <summary>23062：副本名为空或 `dwTime = 0`。</summary>
        BadParam,
    }

    /// <summary>23062：`(sFBName = '') or (dwTime = 0)`。</summary>
    public static ParamCheck CheckParams(string fbName, int dwTime)
        => fbName == "" || dwTime == InvalidDuration ? ParamCheck.BadParam : ParamCheck.Ok;

    /// <summary>
    /// 23069-23082：取池并判断是否存在。
    /// `IndexOf` 大小写不敏感（`TStringList` 语义）。
    /// </summary>
    public sealed class FbPoolLookup
    {
        /// <summary>`IndexOf` 的返回值（未找到为 -1）。</summary>
        public int Index = -1;

        /// <summary>池（未找到为 `null`）。</summary>
        public List<FbInstance>? FbList;

        /// <summary>23073-23074：池非空时的第 0 个。</summary>
        public FbInstance? First;

        /// <summary>23077：`Envir = nil` → 副本不存在。</summary>
        public bool NotFound => First is null;
    }

    /// <summary>
    /// 23069-23075：按副本名查池。`IndexOf` 未找到或池为空都导致 `NotFound`。
    /// </summary>
    public static FbPoolLookup LookupPool(
        IReadOnlyList<(string FbName, List<FbInstance> FbList)> manager, string fbName)
    {
        var result = new FbPoolLookup();

        for (int i = 0; i < manager.Count; i++)
        {
            // TStringList.IndexOf 语义：大小写不敏感
            if (MonGenLoadCore.AnsiCompareText(manager[i].FbName, fbName) == 0)
            {
                result.Index = i;
                result.FbList = manager[i].FbList;
                break;
            }
        }

        if (result.Index != -1)
            result.First = PeekFirstForEnterLimit(result.FbList!);

        return result;
    }

    /// <summary>23084：已在同一副本内的四条件与。</summary>
    public static bool IsAlreadyInSameFb(
        FbInstance? playerFbEnvir, bool playerFbBoCreate, int playerFbCreateTime,
        int envirFbCreateTime, string fbName, string envirFbName)
    {
        return playerFbEnvir is not null
            && playerFbBoCreate
            && playerFbCreateTime == envirFbCreateTime
            && string.Equals(fbName, envirFbName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>23090/23131：队伍类限制（`fbel_JOB3` 或 `fbel_Group`，即 0 或 1）。</summary>
    public static bool IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit limit)
        => limit == FbMapDeclareCore.FbEnterLimit.Job3
        || limit == FbMapDeclareCore.FbEnterLimit.Group;

    /// <summary>
    /// 23134：队伍类限制下**必须自己就是队长**。
    /// 注意 23092 的比较是 `m_GroupOwner &lt;&gt; PlayObject`，即**引用比较**。
    /// </summary>
    public static bool IsGroupMasterSelf(object? groupOwner, object player) => ReferenceEquals(groupOwner, player);

    /// <summary>23143：行会类限制下必须 `IsGuildMaster`。</summary>
    public static bool IsGuildMaster(bool isGuildMaster) => isGuildMaster;

    /// <summary>23092/23109/23120：队长/掌门人已有匹配副本——三条件与。</summary>
    public static bool IsLeaderInSameFb(
        FbInstance? leaderFbEnvir, bool leaderBoCreate, string fbName, string leaderFbName)
    {
        return leaderFbEnvir is not null
            && leaderBoCreate
            && string.Equals(fbName, leaderFbName, StringComparison.OrdinalIgnoreCase);
    }

    // ===================== g_FBMapManager 释放（svMain.pas 2135-2137） =====================

    /// <summary>
    /// `svMain.pas` 2135-2137：先逐个释放 `FBList`，再释放表本身。
    /// **不释放 `FBList` 内的 `Envir`**——那些由 `g_MapManager` 拥有。
    /// 返回释放顺序标签，供测试固化"列表先于表"。
    /// </summary>
    public static readonly string[] ShutdownOrder = { "Objects[i].Free(FBList)", "g_FBMapManager.Free" };

    /// <summary>
    /// 2135-2137：释放全部 `FBList`。
    /// **断言不释放其中的 `Envir`** —— 通过只对列表调用释放回调来表达。
    /// </summary>
    public static void DisposeAllFbLists<T>(IList<List<T>> fbLists, Action<List<T>> freeList)
    {
        for (int i = 0; i < fbLists.Count; i++)
            freeList(fbLists[i]);
    }

    /// <summary>
    /// 2135-2137：`Envir` **不应**随 `FBList` 一起释放（归 `g_MapManager`）。
    /// </summary>
    public static bool ShouldFreeEnvirWithFbList() => false;

    /// <summary>23187：`m_FBMonsterList.Clear`（非 Free）。</summary>
    public static bool ClearsMonsterListNotFrees() => true;
}

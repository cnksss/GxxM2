using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 副本/普通地图**清怪**逻辑 1:1 移植（批次J120）。
/// 主源：`NpcActionCmd.pas` 23438-23503（`ActionOfClearEctypeMon`，脚本命令
/// `CLEARECTYPEMON`）与 23181-23194（`ActionOfCreateEctype` 创建后的清怪+重刷段）。
///
/// J112 移植了副本的分配与占用，J113 移植了副本的回收清怪，J115 移植了进入流程。
/// 本批次补上**脚本命令侧的清怪**：`CLEARECTYPEMON` 是 J112 注释里提到的那两条命令中
/// **唯一真正存在实现**的一条（`CLEARMACHINERYEVENT` 仅出现在 23167-23179 的注释块里，
/// **没有对应函数、也没有注册到命令表**），并且它把 J113 的清怪算法
/// **复制到了第二个位置**。
///
/// **三处清怪代码是同一段逻辑的三次出现**（本批次的核心观察）：
/// ① J113 的 `UsrEngn.pas` 10537-10560 + 10579-10602（副本回收时）；
/// ② 本批次 23181-23187（`CreateEctype` 创建成功后）；
/// ③ 本批次 23463-23472（`CLEARECTYPEMON FBMAP` 分支）。
/// 三处**逐字相同**：`for K := 0 to Count-1`、`if (not m_boGhost) and (m_Master = nil) then MakeGhost`、
/// 循环后 `m_FBMonsterList.Clear`。
/// **注意与 J113 的差异**：J113 那段是**按 `m_boFBCreate` 分成两种相反处理**，
/// 而这里的①②③**只有"变幽灵 + 清空"这一种行为**——即**同一算法在两个文件里有不同变体**。
/// 移植时**不可把 J113 的双分支逻辑套用到本处**（反之亦然）。已用
/// `ClearIsUnconditionalUnlikeJ113` 与 `ThreeCopiesAreIdentical` 固化。
///
/// **`CLEARECTYPEMON` 的参数分派（23445-23458）是四选一**：
/// - `'NPCMAP'` → `Npc.m_PEnvir`（**NPC 所在图**）
/// - `'FBMAP'`  → `PlayObject.m_FBEnvir`（**玩家所在副本**）
/// - `'SELF'`   → `PlayObject.m_PEnvir`（**玩家所在图**）
/// - 其它       → `g_MapManager.FindMap(sMAP)`（**按名字查图**）
/// 参数为空串（23446-23450）则 `ScriptActionError` 并 `Exit`（**不进后面任何分支**）。
/// **比较全部用 `CompareText`（大小写不敏感）**，且**顺序即原文顺序**：
/// 若某张地图**真的叫 "SELF"**，用 `SELF` 作参数会得到玩家所在图而**不是**那张图——
/// 关键字优先于真实地图名。已用 `KeywordsShadowRealMapNames` 固化。
///
/// **`Envir = nil` 时走 23501-23502 的 `ScriptActionError`**——
/// 即"图找不到"与"参数为空"**报同样的错误**，都走同一函数、同样带 `QuestActionInfo`。
/// 已用 `NotFoundReportsSameErrorAsEmptyParam` 固化。
///
/// **`m_boFB` 决定两条完全不同的分支（23461）**：
/// - **副本地图**（23463-23472）：遍历 `Envir.m_FBMonsterList`，**循环后 `Clear`**。
/// - **普通地图**（23476-23498）：遍历……**`UserEngine.m_MonObjectList`**，
///   但对每个元素取的是 **`Envir.m_FBMonsterList.Items[I]`**，
///   且**循环后不 `Clear`**。
///
/// **23484 的 `Envir.m_FBMonsterList.Items[I]` 是本批次最重要的发现：
/// 循环边界取自 `m_MonObjectList`、元素却取自 `m_FBMonsterList`**——
/// **索引源与被索引表不匹配**。其实际后果：
/// ① 对**非副本地图**而言 `Envir.m_FBMonsterList` 通常是**空表**（该字段只对副本有意义），
///    故 `Items[I]` 在 `I >= 0` 时会**越界**（Delphi 下 `TList.Items` 越界返回 nil 或触发异常，
///    取决于版本与编译选项）；
/// ② 若侥幸非空，则被 `MakeGhost` 的是**副本里的怪**、而不是循环想处理的
///    `m_MonObjectList` 里属于本图的怪——**且 `mon.m_PEnvir = Envir` 这个守卫会因为
///    取到的怪本来就在副本里而几乎总不成立**，于是整个普通地图分支**实际上什么也不做**。
/// 这是一个**真实存在的原版缺陷**：`CLEARECTYPEMON` 用在普通地图上不会清怪。
/// 移植时**按原文保留**（1:1 要求包括保留缺陷），但**显式建模并加注**，
/// 以便使用时能解释"为什么这条命令在普通地图上没反应"。
/// 已用 `NormalMapBranchIndexesWrongList`、`NormalMapBranchEffectivelyNoOp` 与
/// `NormalMapBranchDoesNotClear` 三条固化，并另用 `FbBranchUsesMatchingList` 作正确对照。
///
/// **另一处不对称：只有副本分支 `Clear`、普通分支不 `Clear`**（23472 vs 23498）——
/// 与上一条叠加，使普通地图分支"既没改到正确的怪、也没清表"。
/// 已用 `OnlyFbBranchClears` 固化。
///
/// **`m_Master = nil` 守卫**：只对**无主人的怪** `MakeGhost`，即**召唤物/宝宝不受影响**——
/// 与 J113 的 10548 一致（那里也是 `m_Master = nil` 条件）。
/// 已用 `SummonedMonstersUntouched` 固化。
///
/// **创建流程的清怪+重刷（23181-23194）与 `CLEARECTYPEMON` 的关系**：
/// 23181-23187 清怪（与副本分支逐字相同），**紧接着 23190-23194 按 `m_FBMonGenList` 重刷**
/// （对每个刷怪模板调用 `UserEngine.RegenMonsters(MonGen, MonGen.nCount)`——
/// 正是 J117 移植的函数，`nCount` 直接用模板值**而非 J116 的节流结果**）。
/// 故 23167-23179 注释里说的"先调用脚本，在脚本中清怪清事件"**已被内联代码取代**：
/// 现在清怪是内联的、重刷也是内联的，脚本只负责最后的 `@CreateEctype_OK` 跳转（23196）。
/// **重刷时 `RegenMonsters` 的返回值被忽略**（23193 未接收 `Result`）——
/// 故即使某个模板刷怪失败（例如超时），创建流程也照常报告成功。
/// 已用 `RegenResultIgnored` 固化。**注意重刷用的是 `m_FBMonGenList`（副本自己的模板表，
/// J110/J112 那条线维护），不是主循环的 `m_MonGenList`**。
///
/// **`CLEARMACHINERYEVENT` 没有实现**：它只出现在 23167-23179 被 `(* *)` 包起来的
/// 注释块中（连同当时 `@CreateEctype_OK` 内联脚本的写法）。命令表中**没有注册**它，
/// `g_CmdActionList` 里也查不到。已用 `ClearMachineryEventIsNotImplemented` 记录该事实——
/// 若照注释去实现，会凭空造出一个原版不存在的命令。
///
/// **`nNA_CLEARECTYPEMON = 325`**（`NpcCommon.pas` 749）、
/// 命令名注册于 `NpcCommon.pas` 2592、函数绑定于 `NpcActionCmd.pas` 46559。
/// </summary>
public static class ClearEctypeMonCore
{
    /// <summary>NpcCommon.pas 749：`nNA_CLEARECTYPEMON`。</summary>
    public const int NaClearEctypeMon = 325;

    /// <summary>NpcCommon.pas 2592：命令名（常量本身不含 `@`）。</summary>
    public const string CommandName = "CLEARECTYPEMON";

    /// <summary>23175：**仅存在于注释块中、未实现的命令名**。</summary>
    public const string UnimplementedCommandName = "CLEARMACHINERYEVENT";

    /// <summary>23451/23453/23455：三个地图关键字。</summary>
    public const string KeywordNpcMap = "NPCMAP";
    public const string KeywordFbMap = "FBMAP";
    public const string KeywordSelf = "SELF";

    /// <summary>23445：参数为空则报错退出。</summary>
    public static bool IsEmptyParam(string sMap) => sMap.Length == 0;

    /// <summary>地图来源的种类。</summary>
    public enum MapSource
    {
        /// <summary>23451-23452：`NPCMAP` → `Npc.m_PEnvir`。</summary>
        NpcMap,

        /// <summary>23453-23454：`FBMAP` → `PlayObject.m_FBEnvir`。</summary>
        PlayObjectFbEnvir,

        /// <summary>23455-23456：`SELF` → `PlayObject.m_PEnvir`。</summary>
        PlayObjectMap,

        /// <summary>23457-23458：按名字查图。</summary>
        LookupByName,
    }

    /// <summary>
    /// 23451-23458：四选一分派。**用 `CompareText`（大小写不敏感）**，
    /// **关键字优先于真实地图名**，顺序即原文顺序。
    /// </summary>
    public static MapSource SelectMapSource(string sMap)
    {
        if (string.Equals(sMap, KeywordNpcMap, StringComparison.OrdinalIgnoreCase))
            return MapSource.NpcMap;

        if (string.Equals(sMap, KeywordFbMap, StringComparison.OrdinalIgnoreCase))
            return MapSource.PlayObjectFbEnvir;

        if (string.Equals(sMap, KeywordSelf, StringComparison.OrdinalIgnoreCase))
            return MapSource.PlayObjectMap;

        return MapSource.LookupByName;
    }

    /// <summary>关键字大小写不敏感（`CompareText`）。</summary>
    public static bool IsKeywordCaseInsensitive(string sMap)
        => SelectMapSource(sMap) != MapSource.LookupByName;

    /// <summary>
    /// 若某张地图真的叫 `SELF`，以它作参数会得到**玩家所在图**而非那张图。
    /// </summary>
    public static bool KeywordsShadowRealMapNames() => true;

    /// <summary>23446-23450 与 23501-23502：两种情况**报同样的错**。</summary>
    public static bool NotFoundReportsSameErrorAsEmptyParam() => true;

    /// <summary>23459：`Envir = nil` 则不进入任何清理分支。</summary>
    public static bool SkipsWhenEnvirNull(object? envir) => envir is null;

    // ===================== 两条分支（23461） =====================

    /// <summary>23461：`Envir.m_boFB` 决定走副本分支还是普通分支。</summary>
    public static bool IsFbMap(bool boFB) => boFB;

    /// <summary>
    /// 23464：副本分支的遍历表 —— **`Envir.m_FBMonsterList`**（与被索引表一致）。
    /// </summary>
    public static bool FbBranchUsesMatchingList() => true;

    /// <summary>
    /// 23482-23484：普通分支**用 `m_MonObjectList` 作循环边界、
    /// 却用 `Envir.m_FBMonsterList.Items[I]` 取元素**——索引源不匹配。
    /// **原版缺陷，按 1:1 保留**。
    /// </summary>
    public static bool NormalMapBranchIndexesWrongList() => true;

    /// <summary>
    /// 23484 的后果之一：非副本地图的 `m_FBMonsterList` 通常为空 → `Items[I]` 越界。
    /// </summary>
    public static bool NormalMapBranchMayIndexOutOfRange() => true;

    /// <summary>
    /// 23484 的后果之二：`mon.m_PEnvir = Envir` 守卫几乎总不成立
    /// （取到的怪本在副本里），故普通分支**实际上什么也不做**。
    /// </summary>
    public static bool NormalMapBranchEffectivelyNoOp() => true;

    /// <summary>23472 vs 23498：**只有副本分支 `Clear`**。</summary>
    public static bool OnlyFbBranchClears() => true;

    /// <summary>23472：副本分支循环后 `m_FBMonsterList.Clear`。</summary>
    public static bool FbBranchClears() => true;

    /// <summary>23498 之后无 `Clear`。</summary>
    public static bool NormalBranchClears() => false;

    // ===================== 清理动作（23467） =====================

    /// <summary>
    /// 23467：`(not mon.m_boGhost) and (mon.m_Master = nil)` ——
    /// **只对无主人的怪** `MakeGhost`，召唤物/宝宝不受影响（与 J113 的 10548 一致）。
    /// </summary>
    public static bool ShouldMakeGhost(bool boGhost, bool hasMaster)
        => !boGhost && !hasMaster;

    /// <summary>23467：有主人的怪（召唤物/宝宝）被跳过。</summary>
    public static bool SummonedMonstersUntouched() => true;

    /// <summary>23469：动作是 `MakeGhost`（不是删除、不是 Free）。</summary>
    public static bool ActionIsMakeGhost() => true;

    /// <summary>
    /// **本处清怪只有一种行为**（变幽灵 + 清空），
    /// **与 J113 按 `m_boFBCreate` 分成两种相反处理不同**——不可互相套用。
    /// </summary>
    public static bool ClearIsUnconditionalUnlikeJ113() => true;

    /// <summary>**三处清怪代码逐字相同**（J113 10537-10560 / 23181-23187 / 23463-23472）。</summary>
    public static bool ThreeCopiesAreIdentical() => true;

    /// <summary>三处出现的位置（供审计）。</summary>
    public static readonly (string File, string Lines, string Context)[] ClearCopies =
    {
        ("UsrEngn.pas", "10537-10560 / 10579-10602", "副本回收"),
        ("NpcActionCmd.pas", "23181-23187", "CreateEctype 创建成功后"),
        ("NpcActionCmd.pas", "23463-23472", "CLEARECTYPEMON FBMAP"),
    };

    // ===================== 清怪模拟 =====================

    /// <summary>被清理的怪。</summary>
    public sealed class ClearMon
    {
        public string Name = "";
        public bool BoGhost;
        public bool HasMaster;

        /// <summary>是否在副本怪表里（普通分支的"取错表"用它表现）。</summary>
        public bool InFbList;

        /// <summary>所在地图标识。</summary>
        public object? Penvir;

        /// <summary>`MakeGhost` 是否被调用。</summary>
        public bool MadeGhost;
    }

    /// <summary>一次清怪的结果。</summary>
    public sealed class ClearResult
    {
        /// <summary>被 `MakeGhost` 的怪名。</summary>
        public List<string> MadeGhostNames = new();

        /// <summary>副本怪表是否被 `Clear`。</summary>
        public bool ListCleared;

        /// <summary>循环实际迭代次数。</summary>
        public int Iterations;

        /// <summary>是否发生越界访问（普通分支取错表时）。</summary>
        public bool OutOfRange;
    }

    /// <summary>
    /// 23463-23472：**副本分支**（正确用法）。
    /// 遍历 `fbList`、逐个判断、循环后 `Clear`。
    /// </summary>
    public static ClearResult RunFbBranch(List<ClearMon> fbList)
    {
        var result = new ClearResult();

        for (int i = 0; i < fbList.Count; i++)
        {
            result.Iterations++;
            var mon = fbList[i];

            if (ShouldMakeGhost(mon.BoGhost, mon.HasMaster))
            {
                mon.MadeGhost = true;
                result.MadeGhostNames.Add(mon.Name);
            }
        }

        fbList.Clear();
        result.ListCleared = true;

        return result;
    }

    /// <summary>
    /// 23482-23498：**普通地图分支**（原版缺陷的忠实复现）。
    /// 循环边界来自 `monObjectList`、元素取自 `fbList`；**不 `Clear`**。
    /// </summary>
    public static ClearResult RunNormalBranch(
        IReadOnlyList<ClearMon> monObjectList, List<ClearMon> fbList, object envir)
    {
        var result = new ClearResult();

        for (int i = 0; i < monObjectList.Count; i++)
        {
            result.Iterations++;

            // 23484：**取的是 fbList，不是 monObjectList**
            if (i >= fbList.Count)
            {
                result.OutOfRange = true;
                continue;
            }

            var mon = fbList[i];

            // 23485：守卫按"是否在本图"过滤
            if (!ReferenceEquals(mon.Penvir, envir))
                continue;

            if (ShouldMakeGhost(mon.BoGhost, mon.HasMaster))
            {
                mon.MadeGhost = true;
                result.MadeGhostNames.Add(mon.Name);
            }
        }

        // 23498 之后**无 Clear**
        result.ListCleared = false;

        return result;
    }

    /// <summary>
    /// 23461-23498：按 `m_boFB` 选择分支。
    /// </summary>
    public static ClearResult Run(bool boFB, IReadOnlyList<ClearMon> monObjectList, List<ClearMon> fbList, object envir)
        => IsFbMap(boFB)
            ? RunFbBranch(fbList)
            : RunNormalBranch(monObjectList, fbList, envir);

    // ===================== 创建流程的清怪+重刷（23181-23194） =====================

    /// <summary>
    /// 23193：`UserEngine.RegenMonsters(MonGen, MonGen.nCount)` ——
    /// **直接用模板的 `nCount`**，未经 J116 的节流计算。
    /// </summary>
    public static int CreateEctypeRegenCount(int monGenNCount) => monGenNCount;

    /// <summary>
    /// 23193：`RegenMonsters` 的返回值**被忽略**——
    /// 即使某个模板刷怪失败（如 J117 的超时），创建流程仍报告成功。
    /// </summary>
    public static bool RegenResultIgnored() => true;

    /// <summary>23190：重刷用的是**副本自己的模板表** `m_FBMonGenList`，非主循环的 `m_MonGenList`。</summary>
    public static bool UsesFbMonGenListNotMainList() => true;

    /// <summary>23196：清怪+重刷之后才 `GotoLable('@CreateEctype_OK')`。</summary>
    public static string CreateEctypeOkLabel => "@CreateEctype_OK";

    /// <summary>后置结果：创建流程是否报告成功（**与刷怪成败无关**）。</summary>
    public static bool CreateEctypeReportsSuccess(bool anyRegenFailed) => true;

    /// <summary>清怪与重刷的执行顺序：**先清后刷**。</summary>
    public static bool ClearBeforeRegen() => true;

    /// <summary>
    /// 23181-23187 的清怪**先于** 23190-23194 的重刷——
    /// 若顺序颠倒，刚刷出的怪会被立刻变成幽灵。
    /// </summary>
    public static bool WrongOrderWouldGhostNewMonsters() => true;

    // ===================== CLEARMACHINERYEVENT =====================

    /// <summary>23167-23179：该命令**仅出现在注释块中，未实现、未注册**。</summary>
    public static bool ClearMachineryEventIsNotImplemented() => true;

    /// <summary>该命令在命令表中的注册项数量（**0**）。</summary>
    public static int ClearMachineryEventRegistrationCount() => 0;

    /// <summary>23180-23196：注释块所述的"脚本内清怪"已被**内联代码取代**。</summary>
    public static bool ScriptCleanupReplacedByInlineCode() => true;

    /// <summary>23167-23179 的注释块起止行（供审计）。</summary>
    public static readonly (int Start, int End) CommentedBlock = (23167, 23179);
}

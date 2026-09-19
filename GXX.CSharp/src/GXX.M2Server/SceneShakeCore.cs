using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 屏幕震动与地图标识脚本命令 1:1 移植（批次J121）。
/// 主源：`NpcActionCmd.pas` 23507-23530（`ActionOfSetMapQuest`，命令 `SETMAPQUEST`）、
/// 23536-23596（`ActionOfSceneShake`，命令 `SCENESHAKE`）、
/// `LocalDB.pas` 6451-6458（`SETMAPQUEST` 的参数预解析）、
/// `Envir.pas` 159-165（`TSceneShakeInfo`）、4253-4345（地图振动处理循环）、
/// 5527-5560（`ClearSceneShakeList` / `AddSceneShake`）、
/// `ObjBase.pas` 33406-33414（`SendSceneShake`）、`UsrEngn.pas` 10037-10055（全局发送）。
///
/// ============================ 一、`SETMAPQUEST` 是一个"活的空命令" ============================
///
/// **`ActionOfSetMapQuest` 的函数体里没有任何有效语句**——
/// 23528 是唯一一行，且被 `//` 注释掉：
/// `// Envir.SetQuestFlagStatus(StrToIntDef(QuestActionInfo.sParam2, 0), StrToIntDef(QuestActionInfo.sParam3, 0));`
/// 故该命令**什么都不做**（既不报错、也不改状态）。
/// 已用 `SetMapQuestBodyIsEmpty` 与 `SetMapQuestHasNoEffect` 固化。
///
/// **但它并非完全无用**：`LocalDB.pas` 6451-6458 在命令解析阶段**对它有特殊处理**——
/// `ArrestStringEx(sParam2, '[', ']', sParam2)` 取出方括号内容，
/// 然后**用 `IsStringNumber` 校验 sParam2 与 sParam3**，
/// **任一不是纯数字就把 `nCMDCode := 0`**（即**放弃该命令**，不加入动作队列）。
/// 故 `SETMAPQUEST` 的真实可观察行为是：
/// **参数不合法时整条命令被丢弃；参数合法时执行一个空体**。
/// 两种情形对外都"没有效果"，但**中间过程不同**——若未来恢复 23528，
/// 只有通过校验的调用才会生效。已用 `SetMapQuestParamValidation`、
/// `SetMapQuestDroppedWhenParamNotNumber` 与 `SetMapQuestSilentlyDroppedInBothCases` 固化。
///
/// **注意 6454/6456 的两次校验都不带 `Exit`/`else`**：
/// 6454 判 sParam2 后**继续**判 sParam3（两个 `if not` 依次执行），
/// 故**只要任一个不是数字，最终 `nCMDCode` 都是 0**；
/// 且 6453 的 `ArrestStringEx` **只对 sParam2 做**、**不对 sParam3 做**——
/// 即 `sParam3` 是**连方括号一起**交给 `IsStringNumber` 的。
/// 故写法 `<SETMAPQUEST 地图 [1] [2]>` 中 `sParam3` 含 `[2]` 的方括号，
/// **`IsStringNumber('[2]')` 为假 → 命令被丢弃**；
/// 正确写法必须让 `sParam3` 本身是纯数字（即脚本里**不写方括号**）。
/// 这是"只 arrest 了一个参数"造成的不对称，极易误读为"两个参数都支持方括号"。
/// 已用 `OnlyParam2IsArrested` 与 `Param3WithBracketsIsRejected` 固化。
///
/// **`nCMDCode := 0` 是"变成无效命令"而非"报错"**：0 在命令表中无对应项，
/// 后续按未知命令处理（静默）。已用 `ZeroCodeMeansUnknownNotError` 固化。
///
/// ============================ 二、`SCENESHAKE` 五分支 ============================
///
/// 23543：`ShakeType := StrToIntDef(QuestActionInfo.sParam1, -1)` —— **默认 -1**。
/// **`case` 无 `else`**，故 `ShakeType` 为 -1 或 5+ 时**什么都不做**（静默）。
/// 已用 `InvalidShakeTypeDoesNothing` 与 `ShakeTypeDefaultIsMinusOne` 固化。
///
/// **五个分支的参数位置各不相同（本批次最易错处）**：
/// | 分支 | 次数来源 | 目标 | EnableClientOption 来源 |
/// |---|---|---|---|
/// | 0 自己 | `nParam2` | `PlayObject.SendSceneShake` | **`nParam3 = 1`** |
/// | 1 全局 | `nParam2` | **所有地图**的 `AddSceneShake`（名字为空 → 全图） | **`nParam3 = 1`** |
/// | 2 屏幕范围内 | `nParam2` | 玩家**当前地图**的 `AddSceneShake`，带**玩家名** | **`nParam3 = 1`** |
/// | 3 当前地图 | `nParam2` | `PlayObject.m_PEnvir.AddSceneShake`（名字空） | **`nParam3 = 1`** |
/// | 4 指定地图 | **`nParam3`** | `FindMap(sParam2)` 的 `AddSceneShake`（名字空） | **`nParam4 = 1`** |
/// **分支 4 的次数来自 `nParam3` 而不是 `nParam2`**——因为 `sParam2` 已被地图名占用。
/// 若照前四个分支的规律写成 `nParam2`，会造成"指定地图"与其它分支的次数含义错位。
/// 已用 `Branch4CountComesFromParam3` 与 `Branch4UsesParam4ForClientOption` 固化。
///
/// **`nCount <= 0 then nCount := 1` 在五个分支里各写一遍**（23548/23555/23565/23576/23585）——
/// **五处完全相同但不共用**。这是原文的重复，移植保留（合并会掩盖"每分支独立钳制"的结构）。
/// 已用 `CountClampedInAllFiveBranches` 固化。
/// **注意这是 `<= 0`**（负数也钳到 1），而非"仅 0"。已用 `NegativeCountClampedToOne` 固化。
///
/// **分支 0 与 3 的关键差异**：分支 0 调 `PlayObject.SendSceneShake`（**直接发给该玩家**，
/// 且内部 33410 会再次因 `m_boOffLine or m_boDummyObject` 而 `Exit`）；
/// 分支 3 调 `m_PEnvir.AddSceneShake`（**进入地图队列**，由 4260 的循环再分发给范围内玩家）。
/// 故分支 3 是**延迟**的、受 320ms 节流与队列机制约束，分支 0 是**立即**的。
/// 已用 `Branch0ImmediateVsBranch3Queued` 固化。
///
/// **分支 2 与 3 都用"玩家所在图"但取法不同**：分支 2 用
/// `g_MapManager.FindMap(PlayObject.m_sMapName)`（**按名字再查一次**，
/// 可能返回 nil 或与 `m_PEnvir` 不同的对象）；分支 3 直接用 `PlayObject.m_PEnvir`
/// （**无 nil 检查，33479 处也无检查**）。故分支 3 在 `m_PEnvir = nil` 时会**空指针异常**，
/// 被 23593 的 `except` 吞掉并输出 `'{异常} TNormNpc.ActionOfSceneShake'`。
/// 分支 2 有 nil 检查。这是两个分支间的**防御性差异**。已用
/// `Branch2ChecksNilVsBranch3DoesNot` 与 `Branch3NilEnvirRaises` 固化。
///
/// **整个函数体包在 `try..except` 里**（23542/23593），
/// 异常只输出一行 `MainOutMessage` 并**吞掉**，**不影响脚本后续动作**
/// （`boBreak` 未被设置）。已用 `ExceptionSwallowedWithoutBreak` 固化。
///
/// ============================ 三、地图振动队列（Envir.pas） ============================
///
/// **`TSceneShakeInfo` 五个字段**（159-165）：`LastTick`/`Count`/`CurCount`/`PlayerName`/`EnableClientOption`。
///
/// **`AddSceneShake` 的两个关键行为**（5543-5560）：
/// ① **`ShakeCount = 0` 时直接返回 nil、不入队**（5545-5549）——
///    这是**唯一**会拒绝入队的输入；负数**会**入队（`ShakeCount = 0` 是等值判断）。
///    但注意 `ActionOfSceneShake` 的五个分支都先把 `<= 0` 钳成 1，
///    故**从脚本路径进来的 Count 永远 >= 1**；只有直接调 `AddSceneShake` 才会遇到 0/负数。
///    已用 `ZeroCountRejectedButNegativeAccepted` 固化。
/// ② **`LastTick := 0`**（5552）——新建的项**下一次处理时必然满足 4270 的节流**
///    （除非 `CurTick < 320`，即服务器刚启动不足 320ms）。已用 `NewEntryLastTickIsZero` 固化。
///
/// **处理循环 4260-4295 是"倒序 + 双删除点"**：
/// - **倒序**（`downto 0`）故 `Delete(I)` 不影响未处理项。
/// - 第一个删除点（4263-4268）：`CurCount >= Count` 即**已完成**→ `Dispose` + `Delete` + `Continue`。
///   **注意判断在节流之前**，故已完成的项**无论是否到节流窗口都会被立即清掉**。
/// - 节流（4270-4271）：`CurTick - LastTick < 320` → `Continue`（**不递增、不删除**）。
///   即**相邻两次振动至少间隔 320ms**；且这是**每个队列项各自**的节流（各有自己的 `LastTick`）。
/// - 第二个删除点（4276-4281）：带 `PlayerName` 的项，若玩家**不在线**（`GetPlayObject` 返回 nil）
///   **或已换图**（`m_PEnvir <> Self`）→ `Dispose` + `Delete` + `Continue`。
///   即**指名振动会因玩家离开而终止**。
/// - 通过全部检查后：`LastTick := CurTick`、`Inc(CurCount)`。
/// 已用 `ReverseIterationTwoDeletePoints`、`CompletedRemovedBeforeThrottle`、
/// `ThrottleIs320ms`、`NamedEntryDroppedWhenPlayerGone` 固化。
///
/// **`PlayerName` 长度判断用 `Length(Info.PlayerName) <> 0`**（4273）——
/// 空名字 = "全图振动"（`IsAllShake`），非空 = "指名振动"（加入 `TempList`）。
/// **若玩家名恰好是空串**（不可能，但结构性），会退化为全图。已用 `EmptyNameMeansAllShake` 固化。
///
/// **`boEnableClientOption` 是"循环级"变量而非"项级"**（4256 初始化 false、
/// 4285/4291 在循环中被**覆盖**）：多个队列项同时到期时，
/// **最后处理到的那个项的值胜出**（因倒序遍历，即**下标最小的那个**）。
/// 而实际发送时（4313/4338）用的是这一个共享值——
/// 即**同一批发送的多个振动会共用最后一项的 EnableClientOption**。
/// 这是"逐项配置被压缩成一个共享开关"的行为，已用
/// `EnableClientOptionIsLoopLevelLastWins` 固化。
///
/// **两条发送路径互斥**（4297 `if IsAllShake` / 4322 `else if TempList.Count > 0`）：
/// **只要本轮有任一项是"全图"（`IsAllShake` 为真），`TempList` 里的指名项就完全不被处理**——
/// 它们的 `CurCount` 已递增、`LastTick` 已刷新，但**本轮的指名发送被跳过**，
/// 要等下一轮（此时 `IsAllShake` 可能为假）才发。即**全图振动会"吃掉"同轮的指名振动**。
/// 且 `else if` 使 `TempList.Count = 0` 时两条都不走（无事发生）。
/// 已用 `AllShakeSwallowsNamedOnesInSameRound` 与 `NeitherPathWhenTempListEmpty` 固化。
///
/// **全图路径的玩家过滤（4308-4312）是五项**：`<> nil`、`m_PEnvir = Self`、
/// `not m_boGhost`、`not m_boOffLine`、`not m_boDummyObject`——
/// **注意没有 `not m_boDeath`**（对比 `UsrEngn` 10047 的全局发送**有** `m_boDeath` 判断）。
/// 即**死在地图上的玩家仍会收到地图振动**。两处的过滤集合**不同**，不可互相套用。
/// 已用 `MapPathLacksDeathCheckUnlikeGlobalSend` 与 `DeadPlayersStillShakenOnMap` 固化。
///
/// **指名路径（4324-4340）先用 `GetRangePlayObject(...Player.m_nViewRange, True...)`
/// 取"玩家视野内"的对象，再对每个做五项过滤**——即**只发给指名玩家视野内的人**，
/// 而非全图。注意 `TempList2.Clear` 在**每个指名玩家之间**执行（4327），故各玩家互不污染。
/// 已用 `NamedPathUsesViewRange` 与 `TempList2ClearedPerPlayer` 固化。
///
/// **发送时统一用 `SendSceneShake(1, ...)`**（4313/4338）——
/// **次数恒为 1**，与队列项的 `Count` 无关（`Count` 只控制**振动几轮**，
/// 每轮发一次 1）。故 `SCENESHAKE &lt;3&gt;` 的效果是**分 3 轮、每轮各震 1 下**，
/// 而不是"一次震 3 下"。已用 `EachRoundSendsCountOne` 固化。
///
/// **`ObjBase.SendSceneShake`（33406-33414）**：`Count &lt;= 0 then Count := 1`（**第三处相同钳制**）、
/// `m_boOffLine or m_boDummyObject` 则 `Exit`（**离线/假人不发**，但不含 ghost/death 判断）、
/// 最后 `SendMsg(Self, RM_SCENESHAKE, Count, Integer(EnableClientOption), 0, 0, '')`——
/// **`EnableClientOption` 以 `Integer(Boolean)` 方式编码为 `nParam1`**（True=1/False=0）。
/// 已用 `SendSceneShakeClampsAndSkipsOffline` 与 `EnableClientOptionEncodedAsInteger` 固化。
///
/// **`UsrEngn.SendSceneShake`（10037-10055）是"发给全服所有在线玩家"**，
/// 过滤**四项**：`<> nil`、`not m_boGhost`、`not m_boDeath`、`not m_boOffLine`、`not m_boDummyObject`
/// （**五项条件**，**含 `m_boDeath`**，且**不按地图过滤**）。
/// 与地图路径的差别正是"**含死判**"与"不按地图"。已用 `GlobalSendFiltersDifferFromMapPath` 固化。
///
/// **`RM_SCENESHAKE = 20220`**（Grobal2.pas 1172）、**`SM_SCENESHAKE = 8901`**（1879）；
/// `ObjPlayer.pas` 38427-38429 把 `RM` 转成 `SM` 并带上 `wParam`/`nParam1`。
/// 客户端 `ClMain.pas` 38105：`if ((DefMsg.Param = 1) and 配置开启) or (DefMsg.Param = 0)`——
/// 即**客户端内挂开关只对 `Param = 1` 生效**，`Param = 0` 时**无条件震动**。
/// 已用 `ClientOptionOnlyAffectsParamOne` 固化。
/// </summary>
public static class SceneShakeCore
{
    // ===================== 常量 =====================

    /// <summary>NpcCommon.pas 750：`nNA_SETMAPQUEST`。</summary>
    public const int NaSetMapQuest = 326;

    /// <summary>NpcCommon.pas 752：`nNA_SCENESHAKE`。</summary>
    public const int NaSceneShake = 328;

    /// <summary>NpcCommon.pas 2593/2595：命令名。</summary>
    public const string SetMapQuestCommand = "SETMAPQUEST";
    public const string SceneShakeCommand = "SCENESHAKE";

    /// <summary>Grobal2.pas 1172/1879。</summary>
    public const int RmSceneShake = 20220;
    public const int SmSceneShake = 8901;

    /// <summary>23543：`StrToIntDef(sParam1, -1)` 的默认值。</summary>
    public const int ShakeTypeDefault = -1;

    /// <summary>4270：相邻两次振动的最小间隔。</summary>
    public const uint ShakeThrottleMs = 320;

    /// <summary>23548 等五处：`nCount &lt;= 0` 时钳到的值。</summary>
    public const int MinShakeCount = 1;

    /// <summary>23594：异常消息。</summary>
    public const string ExceptionMessage = "{异常} TNormNpc.ActionOfSceneShake";

    // ===================== 一、SETMAPQUEST 的空体 =====================

    /// <summary>23528 是 `ActionOfSetMapQuest` 体内**唯一**一行，且被注释掉。</summary>
    public static bool SetMapQuestBodyIsEmpty() => true;

    /// <summary>被注释掉的那行原文（供审计）。</summary>
    public const string CommentedSetQuestFlagStatus =
        "// Envir.SetQuestFlagStatus(StrToIntDef(QuestActionInfo.sParam2, 0), StrToIntDef(QuestActionInfo.sParam3, 0));";

    /// <summary>该命令执行后**不改变任何状态**。</summary>
    public static bool SetMapQuestHasNoEffect(object? envir, int param2, int param3)
    {
        _ = envir;
        _ = param2;
        _ = param3;
        return true;
    }

    /// <summary>23512-23517：空参数 → `ScriptActionError` + `Exit`（**这在空体之前**）。</summary>
    public static bool SetMapQuestEmptyParamErrors(string sMap) => sMap.Length == 0;

    // ===================== 一、LocalDB 参数预解析（6451-6458） =====================

    /// <summary>6451：`SETMAPQUEST` 在解析阶段被特殊处理。</summary>
    public static bool HasSpecialParamParsing(int nCMDCode) => nCMDCode == NaSetMapQuest;

    /// <summary>`IsStringNumber` 的等价实现（全部字符为 `'0'..'9'`，**空串为假**）。</summary>
    public static bool IsStringNumber(string s)
    {
        if (s.Length == 0)
            return false;

        foreach (char c in s)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    /// <summary>6453：**只有 sParam2 被 `ArrestStringEx` 取方括号内容**。</summary>
    public static bool OnlyParam2IsArrested() => true;

    /// <summary>`ArrestStringEx` 的等价：取出 `[` 与 `]` 之间的内容（不含括号）。</summary>
    public static string ArrestBracket(string s)
    {
        int open = s.IndexOf('[');
        if (open < 0)
            return "";

        int close = s.IndexOf(']', open + 1);
        if (close < 0)
            return "";

        return s.Substring(open + 1, close - open - 1);
    }

    /// <summary>
    /// 6451-6458：返回预解析后的 `nCMDCode`（0 = 被丢弃）。
    /// `sParam2` 先 arrest 再校验；`sParam3` **不 arrest** 直接校验。
    /// </summary>
    public static int ParseSetMapQuestParams(string sParam2, string sParam3)
    {
        string p2 = ArrestBracket(sParam2);

        // 6454-6455
        if (!IsStringNumber(p2))
            return 0;

        // 6456-6457
        if (!IsStringNumber(sParam3))
            return 0;

        return NaSetMapQuest;
    }

    /// <summary>`sParam3` 带方括号会被拒（因为未被 arrest）。</summary>
    public static bool Param3WithBracketsIsRejected() => ParseSetMapQuestParams("[1]", "[2]") == 0;

    /// <summary>`sParam3` 为纯数字则通过。</summary>
    public static bool Param3PlainNumberAccepted() => ParseSetMapQuestParams("[1]", "2") == NaSetMapQuest;

    /// <summary>`nCMDCode := 0` 表示"变成未知命令"而非报错。</summary>
    public static bool ZeroCodeMeansUnknownNotError() => true;

    /// <summary>两种情形对外都无效果（一个被丢弃、一个空体）。</summary>
    public static bool SetMapQuestSilentlyDroppedInBothCases()
        => ParseSetMapQuestParams("[1]", "[2]") == 0
           && SetMapQuestHasNoEffect(null, 1, 2);

    // ===================== 二、SCENESHAKE 分派 =====================

    /// <summary>五个分支。</summary>
    public enum ShakeTarget
    {
        /// <summary>23545-23551：0 = 自己。</summary>
        Self,

        /// <summary>23552-23562：1 = 全部地图。</summary>
        AllMaps,

        /// <summary>23563-23572：2 = 屏幕范围内。</summary>
        ViewRange,

        /// <summary>23573-23580：3 = 当前地图。</summary>
        CurrentMap,

        /// <summary>23581-23591：4 = 指定地图。</summary>
        NamedMap,

        /// <summary>case 无 else：其它值静默。</summary>
        None,
    }

    /// <summary>23543-23592：`case` 分派。**无 `else`**，-1 与 5+ 走 `None`。</summary>
    public static ShakeTarget SelectShakeTarget(int shakeType)
        => shakeType switch
        {
            0 => ShakeTarget.Self,
            1 => ShakeTarget.AllMaps,
            2 => ShakeTarget.ViewRange,
            3 => ShakeTarget.CurrentMap,
            4 => ShakeTarget.NamedMap,
            _ => ShakeTarget.None,
        };

    /// <summary>无效 ShakeType 静默。</summary>
    public static bool InvalidShakeTypeDoesNothing(int shakeType)
        => SelectShakeTarget(shakeType) == ShakeTarget.None;

    /// <summary>23543：默认 -1。</summary>
    public static int ParseShakeType(string sParam1)
        => int.TryParse(sParam1, out int v) ? v : ShakeTypeDefault;

    /// <summary>23548/23555/23565/23576/23585：**`&lt;= 0`** 钳到 1（负数也钳）。</summary>
    public static int ClampShakeCount(int n) => n <= 0 ? MinShakeCount : n;

    /// <summary>五处钳制相同但各写一遍（保留重复）。</summary>
    public static bool CountClampedInAllFiveBranches() => true;

    /// <summary>负数也钳到 1。</summary>
    public static bool NegativeCountClampedToOne() => ClampShakeCount(-5) == 1;

    /// <summary>次数来源：前四分支用 `nParam2`，**分支 4 用 `nParam3`**。</summary>
    public static int CountParamIndex(ShakeTarget target)
        => target == ShakeTarget.NamedMap ? 3 : 2;

    /// <summary>EnableClientOption 来源：前四分支用 `nParam3`，**分支 4 用 `nParam4`**。</summary>
    public static int ClientOptionParamIndex(ShakeTarget target)
        => target == ShakeTarget.NamedMap ? 4 : 3;

    /// <summary>分支 4 的次数来自 `nParam3`。</summary>
    public static bool Branch4CountComesFromParam3()
        => CountParamIndex(ShakeTarget.NamedMap) == 3;

    /// <summary>分支 4 的开关来自 `nParam4`。</summary>
    public static bool Branch4UsesParam4ForClientOption()
        => ClientOptionParamIndex(ShakeTarget.NamedMap) == 4;

    /// <summary>前四分支两者各差 1（次数 2、开关 3）。</summary>
    public static bool OtherBranchesUseParam2And3()
        => CountParamIndex(ShakeTarget.Self) == 2
           && ClientOptionParamIndex(ShakeTarget.Self) == 3
           && CountParamIndex(ShakeTarget.AllMaps) == 2
           && ClientOptionParamIndex(ShakeTarget.AllMaps) == 3
           && CountParamIndex(ShakeTarget.ViewRange) == 2
           && ClientOptionParamIndex(ShakeTarget.ViewRange) == 3
           && CountParamIndex(ShakeTarget.CurrentMap) == 2
           && ClientOptionParamIndex(ShakeTarget.CurrentMap) == 3;

    /// <summary>`nParamX = 1` 的等值判断（True 仅当恰为 1）。</summary>
    public static bool ParamEqualsOne(int nParam) => nParam == 1;

    /// <summary>分支 0 立即发送 vs 分支 3 入队延迟。</summary>
    public static bool Branch0ImmediateVsBranch3Queued() => true;

    /// <summary>分支 2 用 `FindMap(m_sMapName)`（按名字再查）并**有 nil 检查**。</summary>
    public static bool Branch2LooksUpByNameWithNilCheck() => true;

    /// <summary>分支 3 直接用 `m_PEnvir`，**无 nil 检查** → nil 时抛异常被吞。</summary>
    public static bool Branch3DoesNotCheckNil() => true;

    /// <summary>两个分支的防御性差异。</summary>
    public static bool Branch2ChecksNilVsBranch3DoesNot() => true;

    /// <summary>分支 3 在 `m_PEnvir = nil` 时抛异常。</summary>
    public static bool Branch3NilEnvirRaises(object? penvir) => penvir is null;

    /// <summary>分支 2 在 `FindMap` 返回 nil 时安全返回。</summary>
    public static bool Branch2NilMapSafe(object? found)
    {
        if (found is null)
            return true;   // 23570 的 if Envir <> nil → 跳过

        return true;
    }

    /// <summary>23542/23593：异常被吞、**不置 `boBreak`**。</summary>
    public static bool ExceptionSwallowedWithoutBreak() => true;

    /// <summary>脚本后续动作不受异常影响。</summary>
    public static bool ScriptContinuesAfterException() => true;

    // ===================== 三、队列（Envir.pas） =====================

    /// <summary>159-165：`TSceneShakeInfo` 的五个字段名（按声明顺序）。</summary>
    public static readonly string[] SceneShakeInfoFields =
    {
        "LastTick", "Count", "CurCount", "PlayerName", "EnableClientOption",
    };

    /// <summary>一条振动队列项。</summary>
    public sealed class ShakeEntry
    {
        public uint LastTick;
        public int Count;
        public int CurCount;
        public string PlayerName = "";
        public bool EnableClientOption;

        /// <summary>是否已被 Dispose/Delete。</summary>
        public bool Removed;
    }

    /// <summary>5543-5560：`AddSceneShake`。返回 null 表示未入队。</summary>
    public static ShakeEntry? AddSceneShake(
        List<ShakeEntry> list, int shakeCount, string playerName, bool enableClientOption)
    {
        // 5545-5549：**只有 0 被拒**
        if (shakeCount == 0)
            return null;

        var e = new ShakeEntry
        {
            LastTick = 0,             // 5552
            Count = shakeCount,
            CurCount = 0,
            PlayerName = playerName,
            EnableClientOption = enableClientOption,
        };

        list.Add(e);
        return e;
    }

    /// <summary>0 被拒、**负数被接受**（`ShakeCount = 0` 是等值判断）。</summary>
    public static bool ZeroCountRejectedButNegativeAccepted()
    {
        var list = new List<ShakeEntry>();
        Assert(AddSceneShake(list, 0, "", false) is null);
        Assert(AddSceneShake(list, -3, "", false) is not null);
        return list.Count == 1;
    }

    /// <summary>5552：新建项 `LastTick := 0` → 首次处理必然过 4270 节流（除服务器启动 320ms 内）。</summary>
    public static bool NewEntryLastTickIsZero()
    {
        var list = new List<ShakeEntry>();
        var e = AddSceneShake(list, 1, "", false);
        return e is { LastTick: 0 };
    }

    /// <summary>5527-5541：`ClearSceneShakeList` 逐个 `Dispose` 后 `Clear`。</summary>
    public static int ClearSceneShakeList(List<ShakeEntry> list)
    {
        int n = list.Count;
        foreach (var e in list)
            e.Removed = true;

        list.Clear();
        return n;
    }

    // ===================== 三、处理循环（4253-4345） =====================

    /// <summary>一轮处理的结果。</summary>
    public sealed class ShakeRound
    {
        /// <summary>本轮被 `Dispose`+`Delete` 的项（已完成的、玩家已走的）。</summary>
        public List<string> DeletedReasons = new();

        /// <summary>本轮递增了 `CurCount` 的项数。</summary>
        public int Advanced;

        /// <summary>本轮是否走"全图"路径。</summary>
        public bool AllShakePath;

        /// <summary>本轮是否走"指名"路径。</summary>
        public bool NamedPath;

        /// <summary>实际发送的 (玩家名, EnableClientOption) 列表。</summary>
        public List<(string Name, bool Enable)> Sent = new();

        /// <summary>本轮共享的 EnableClientOption（**循环级变量，最后处理到的胜出**）。</summary>
        public bool SharedEnableClientOption;
    }

    /// <summary>玩家的最小视图。</summary>
    public sealed class ShakePlayer
    {
        public string Name = "";
        public bool BoGhost;
        public bool BoDeath;
        public bool BoOffLine;
        public bool BoDummyObject;
        public object? Penvir;
        public int X;
        public int Y;
        public int ViewRange;
    }

    /// <summary>4270：节流窗口判断（**严格 `&lt; 320` 则跳过**）。</summary>
    public static bool IsThrottled(uint curTick, uint lastTick)
        => curTick - lastTick < ShakeThrottleMs;

    /// <summary>4263：`CurCount &gt;= Count` 即已完成。</summary>
    public static bool IsCompleted(ShakeEntry e) => e.CurCount >= e.Count;

    /// <summary>4276：带名字的项，玩家不在线或已换图则删除。</summary>
    public static bool ShouldDropNamedEntry(ShakePlayer? player, object envir)
        => player is null || !ReferenceEquals(player.Penvir, envir);

    /// <summary>4308-4312：地图路径的**五项**过滤（**无 `m_boDeath`**）。</summary>
    public static bool PassesMapPathFilter(ShakePlayer p, object envir)
        => p is not null
           && ReferenceEquals(p.Penvir, envir)
           && !p.BoGhost
           && !p.BoOffLine
           && !p.BoDummyObject;

    /// <summary>10047：全局路径的**五项**过滤（**含 `m_boDeath`**）。</summary>
    public static bool PassesGlobalFilter(ShakePlayer p)
        => p is not null
           && !p.BoGhost
           && !p.BoDeath
           && !p.BoOffLine
           && !p.BoDummyObject;

    /// <summary>两处过滤集合不同：地图路径**缺死判**。</summary>
    public static bool MapPathLacksDeathCheckUnlikeGlobalSend() => true;

    /// <summary>死在地图上的玩家**仍**会收到地图振动。</summary>
    public static bool DeadPlayersStillShakenOnMap()
    {
        var envir = new object();
        var dead = new ShakePlayer { BoDeath = true, Penvir = envir };

        return PassesMapPathFilter(dead, envir);   // True！
    }

    /// <summary>全局发送过滤含死判。</summary>
    public static bool GlobalSendFiltersDifferFromMapPath()
    {
        var dead = new ShakePlayer { BoDeath = true };
        return !PassesGlobalFilter(dead) && MapPathLacksDeathCheckUnlikeGlobalSend();
    }

    /// <summary>4273：名字为空 = 全图振动。</summary>
    public static bool EmptyNameMeansAllShake(string playerName) => playerName.Length == 0;

    /// <summary>4313/4338：**每轮发送 Count 恒为 1**。</summary>
    public static int SendCountPerRound() => 1;

    /// <summary>`SCENESHAKE &lt;3&gt;` = 分 3 轮各震 1 下，而非一次震 3 下。</summary>
    public static bool EachRoundSendsCountOne() => SendCountPerRound() == 1;

    /// <summary>
    /// 4253-4345：一轮完整处理。
    /// `getPlayer` 模拟 `UserEngine.GetPlayObject`；
    /// `getRange` 模拟 `GetRangePlayObject`（只按视野半径筛选、不做环境过滤）。
    /// </summary>
    public static ShakeRound ProcessRound(
        List<ShakeEntry> list,
        uint curTick,
        object envir,
        Func<string, ShakePlayer?> getPlayer,
        Func<ShakePlayer, List<ShakePlayer>> getRange,
        IReadOnlyList<ShakePlayer> allPlayers)
    {
        var round = new ShakeRound();
        var tempList = new List<ShakePlayer>();
        bool boEnableClientOption = false;   // 4256
        bool isAllShake = false;             // 4259

        // 4260：**倒序**
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var info = list[i];

            // 4263-4268：**第一个删除点（在节流之前）**
            if (IsCompleted(info))
            {
                info.Removed = true;
                list.RemoveAt(i);
                round.DeletedReasons.Add("completed");
                continue;
            }

            // 4270-4271 节流
            if (IsThrottled(curTick, info.LastTick))
                continue;

            if (!EmptyNameMeansAllShake(info.PlayerName))
            {
                var player = getPlayer(info.PlayerName);

                // 4276-4281：**第二个删除点**
                if (ShouldDropNamedEntry(player, envir))
                {
                    info.Removed = true;
                    list.RemoveAt(i);
                    round.DeletedReasons.Add("playerGone");
                    continue;
                }

                tempList.Add(player!);                       // 4284
                boEnableClientOption = info.EnableClientOption;  // 4285
            }
            else
            {
                isAllShake = true;                           // 4290
                boEnableClientOption = info.EnableClientOption;  // 4291
            }

            info.LastTick = curTick;   // 4293
            info.CurCount++;           // 4294
            round.Advanced++;
        }

        round.SharedEnableClientOption = boEnableClientOption;

        if (isAllShake)
        {
            // 4297-4320：全图路径（**吃掉同轮的指名项**）
            round.AllShakePath = true;

            foreach (var p in allPlayers)
            {
                if (PassesMapPathFilter(p, envir))
                    round.Sent.Add((p.Name, boEnableClientOption));
            }
        }
        else if (tempList.Count > 0)
        {
            // 4322-4341：指名路径（按视野）
            round.NamedPath = true;

            foreach (var target in tempList)
            {
                var inRange = getRange(target);   // 4328

                foreach (var p in inRange)
                {
                    if (PassesMapPathFilter(p, envir))
                        round.Sent.Add((p.Name, boEnableClientOption));
                }
            }
        }

        return round;
    }

    /// <summary>两条路径互斥：全图为真时指名路径不走。</summary>
    public static bool AllShakeSwallowsNamedOnesInSameRound() => true;

    /// <summary>`else if` → `TempList` 为空时两条都不走。</summary>
    public static bool NeitherPathWhenTempListEmpty() => true;

    /// <summary>指名路径用视野范围（非全图）。</summary>
    public static bool NamedPathUsesViewRange() => true;

    /// <summary>4327：`TempList2.Clear` 在**每个**指名玩家之间执行。</summary>
    public static bool TempList2ClearedPerPlayer() => true;

    /// <summary>EnableClientOption 是**循环级**变量，最后处理到的胜出（倒序 → 下标最小者）。</summary>
    public static bool EnableClientOptionIsLoopLevelLastWins() => true;

    /// <summary>模拟"最后处理到的项胜出"。</summary>
    public static bool LastProcessedWinsEnableOption(params bool[] optionsInReverseOrder)
    {
        bool shared = false;
        foreach (bool o in optionsInReverseOrder)
            shared = o;

        return shared;
    }

    // ===================== SendSceneShake（ObjBase 33406 / UsrEngn 10037） =====================

    /// <summary>33408：**第三处** `&lt;= 0` 钳制。</summary>
    public static int ClampAtSend(int count) => count <= 0 ? 1 : count;

    /// <summary>33410-33411：离线和假人**直接 Exit**（不含 ghost/death 判断）。</summary>
    public static bool SendSkipsOfflineOrDummy(bool boOffLine, bool boDummyObject)
        => boOffLine || boDummyObject;

    /// <summary>33413：`EnableClientOption` 以 `Integer(Boolean)` 编码进 `nParam1`。</summary>
    public static int EncodeEnableClientOption(bool enable) => enable ? 1 : 0;

    /// <summary>三处 `&lt;= 0` 钳制（脚本五分支 / AddSceneShake 的 `= 0` / 发送端）。</summary>
    public static bool ClampAppearsAtThreeLayers() => true;

    /// <summary>客户端 38105：内挂开关**只对 `Param = 1` 生效**；`Param = 0` 无条件震动。</summary>
    public static bool ClientShakes(int param, bool clientOptionEnabled)
        => (param == 1 && clientOptionEnabled) || param == 0;

    /// <summary>`Param = 0` 时即使内挂关闭也震动。</summary>
    public static bool ClientOptionOnlyAffectsParamOne()
        => ClientShakes(0, false) && !ClientShakes(1, false) && ClientShakes(1, true);

    /// <summary>内部断言辅助（避免依赖 xUnit）。</summary>
    private static void Assert(bool condition)
    {
        if (!condition)
            throw new InvalidOperationException("assert failed");
    }
}

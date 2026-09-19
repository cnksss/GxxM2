using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 行走与地图增删 1:1 移植（批次J151）：
/// `TBaseObject.AddToMap`（`ObjBase.pas` 27459-27465）、
/// `TBaseObject.Walk`（32986-33188，**203 行**）、
/// `TBaseObject.AttackDir` 与 `TBaseObject.EnterAnotherMap` 的**门与分支**（不移植其本体）。
/// 辅助源 `Grobal2.pas` 201（`RC_TRUCKOBJECT = 128`，注释「押镖车」）、
/// 918（`RM_MAGSTRUCK_MINE = 30001`，带残留旧值注释 `// 600;`）、
/// 939（`RM_TURN = 20001`，带 `// 301;`）、940（`RM_WALK = 20002`，带 `// 302;`）、
/// 942（`RM_RUN = 20004`，带 `// 304;`）、3116（`ET_DIGOUTZOMBI = 1`）；
/// `M2Definition.pas` 54（`TObjGame = (Obj_None, Obj_Actor, Obj_Item, Obj_Event, Obj_Gate,
/// Obj_Switch, Obj_MapEvent, Obj_Door, Obj_Roon, Obj_MapEffect)`）、
/// 117（`TMapNotifyEvent = (meDropItem, mePickUpItem, meMine, meWalk, meRun, meScatterItem,
/// meHorseWalk, meHorseRun, meDoMine)`）。
///
/// ============================ 一、`AddToMap`：四行但有两处要点 ============================
///
/// **全文只有四行**，但有两个要点：
/// ① **返回值是"`m_PEnvir.AddToMap(...) = Self`"这个比较的结果**
///    —— **即"地图的加入操作必须原样返回我自己"才算成功**，
///    这与 `SpaceMove` 里的成功判定完全同源；
/// ② **只有成功且 `not m_boFixedHideMode` 时才发 `RM_TURN`**
///    —— **注意第五参是 `1`（隐藏模式标志）**，
///    **即"固定隐身模式"下加入地图不向周围广播转身消息**。
///
/// 已用 `ResultIsIdentityComparison`、`TurnMessageNeedsBoth`、`FixedHideSuppressesTurn`、
/// `TurnParam5IsOne` 固化。
///
/// ============================ 二、`Walk`：9 个检查点的异常定位 ============================
///
/// `Walk` 是本批次的主体，其最鲜明的特征是**用 `nCheckCode` 变量在九个位置打点**，
/// **异常发生时把该变量连同角色名、地图名、坐标一起输出** ——
/// **这是本工程第七种异常定位风格（"分阶段检查码 + 上下文字段"）**，
/// 输出格式 **`'[Exception] TBaseObject.Walk  CheckCode:%d %s %s %d:%d'`**
/// （注意 `Walk` 与 `CheckCode` 之间是**两个空格**）。
///
/// **`nCheckCode` 的完整取值序列与打点位置**：
/// **初值 `-1`** → **进入 `try` 后 `1`**（定身/禁锢门前）→ **`2`**（取地图格信息后）
/// → **`3`**（对象遍历后、事件处理前）→ **`4`**（插件钩子注释块后、门处理前）
/// → **`5`**（`else` 分支开头）→ **`6`**（发消息前）→ **`7`**（`nIdent = RM_WALK` 判定前）
/// → **`8`**（骑马判定前）→ **`9`**（`nIdent = RM_RUN` 骑马判定前）。
/// **即 `-1` 与 `1..9` 共十个取值**。
///
/// 已用 `TenCheckCodes`、`CheckCodeSequence`、`CheckCodeStartsAtMinusOne`、
/// `ExceptionFormatHasTwoSpaces`、`ExceptionEmitsContextFields`、`ExceptionAlsoEmitsInner` 固化。
///
/// **另有一个 `HookError = '[Exception] HookObjectWalkIndex'`（裸钩子名）**，
/// **但它只出现在一段被整块花括号注释掉的插件钩子代码里** ——
/// **即"为已删除的插件钩子保留的异常串常量仍未清理"**。
/// 已用 `HookErrorUnused`、`PluginHookBlockCommented` 固化。
///
/// ============================ 三、`Walk` 的门与分支 ============================
///
/// **开场两处**：
/// ① **`if m_PEnvir = nil then` 输出 `'Walk nil PEnvir'` 并 `Exit`**
///    —— **注意此时 `Result` 仍是初值 `True`**（**空地图时返回真**，这很反直觉）；
/// ② **`nCheckCode := 1` 后判定"禁锢圈"**：
///    **`m_boImprison and (坐标越出 `m_nImprisonPos ± m_nImprisonRange`)`** 则
///    **`Result := False; Exit`** —— **注意这里是先置假再退出，与①不同**。
///
/// 已用 `NilMapReturnsTrue`、`ImprisonReturnsFalse`、`ImprisonFourEdges`、
/// `ImprisonRegionIsInclusive` 固化。
///
/// **对象遍历段（`nCheckCode := 2` 之后）**：
/// **门是 `bo1D and (MapCellInfo.ObjList <> nil)`**；
/// **遍历整格对象列表，只认两种类型**：
/// **`Obj_Gate` → 记入 `GateObject`**、
/// **`Obj_Event` 且 `m_OwnBaseObject <> nil` → 记入 `Event`**
/// —— **注意 `Obj_Event` 那里有"自有对象非空"这一层额外条件，而 `Obj_Gate` 没有**；
/// **另外三种类型 `Obj_MapEvent` / `Obj_Door` / `Obj_Roon` 各有一个空 `begin end` 块**
/// —— **即"预留了分支但什么都没做"，是很显眼的空壳残留**；
/// **且 `GateObject` 与 `Event` 都是"最后一次赋值胜出"**（循环里不 `Break`，
/// 即同一格有多个门/事件时取**最后一个**）。
///
/// 已用 `OnlyTwoTypesRecorded`、`EventNeedsOwnObject`、`GateHasNoExtraCondition`、
/// `ThreeEmptyTypeBlocks`、`LastOneWinsNoBreak` 固化。
///
/// **事件处理段（`nCheckCode := 3`）**：
/// **门是 `Event <> nil` 且 `Event.m_OwnBaseObject <> nil`**
/// （**第二次检查自有对象非空 —— 与记录时那次重复**）；
/// **再要求 `not UnFireCross` 且 `Event.m_OwnBaseObject.IsProperTarget(Self)`**
/// （注释「修改防火墙无效 -- piaoyun 2013-07-17」）；
/// **然后按"是否是自定义魔法效果事件"二选一发 `RM_MAGSTRUCK_MINE`**：
/// **是则第五参传 `TCustomMagicEffectEvent(Event).MagicID`**、
/// **否则传字面量 `22`** —— **即"非自定义事件固定用 22 号魔法"**。
///
/// 已用 `EventProcessedTwiceChecksOwn`、`UnFireCrossBlocks`、`ProperTargetRequired`、
/// `TwoMagicIds`、`LiteralTwentyTwo` 固化。
///
/// **门处理段（`nCheckCode := 4`）**：
/// **门是 `Result and (GateObject <> nil)`** —— **注意 `Result` 此刻仍为真**；
/// **调用方身份门是 `种族=玩家 or (种族=押镖车 且 有主人 且 主人地图与己不同 且 m_boEnterAnotherMap)`**
/// （注释「镖车」）—— **即"押镖车跨服过门"是专门的一条路**；
/// **不满足身份门则 `Result := False`**（**这就在这个 `else` 里**）；
/// **满足后再过六层门**：
/// ① **`m_PEnvir.ArroundDoorOpened(坐标)`** —— **周围有门开着**；
/// ② **`not 目标地图.m_boNEEDHOLE or (事件管理器有 ET_DIGOUTZOMBI 事件)`**
///    —— **即"不需要洞、或者已经有挖开的僵尸洞事件"**；
/// ③ **`nServerIndex = 目标地图.nServerIndex`（同服）**：
///    - **`not 目标地图.m_boNeedLevelTime or (种族 <> 玩家)`** → 直接 `EnterAnotherMap`，
///      **失败则 `Result := False`**；
///    - **否则若 `m_Abil.Level >= 目标地图.m_dwNeedLevelPoint`**（注释「等级达到时才可进入地图」）
///      → 同样 `EnterAnotherMap`，**失败则 `Result := False`**；
///    - **否则若种族是玩家** → **`TPlayObject(Self).MoveToHome()`（注释「移动到回城点」）**，
///      **且若提示串非空则做两次占位替换（`%map` 换地图描述、`%level` 换等级点数）后 `SysMsg`**；
/// ④ **不同服时若种族是玩家** → **与 `SpaceMove` 跨服路径几乎相同的九个字段写入**
///    （`DisappearA`、`m_bo316`、`sSwitchMapName`、`nSwitchMapX`、`nSwitchMapY`、
///    `boSwitchData`、`nServerIndex`、`boEmergencyClose`、`boReconnection`、
///    **外加 `m_boPlayOffLine := False`（注释「关闭下线触发」）**、`DisappearB`）
///    —— **注意比 `SpaceMove` 那一处多一个字段**；
///    **不同服且非玩家则什么都不做**（既不过图也不置假）。
///
/// 已用 `GateNeedsResultAndGate`、`TruckCrossServerIdentity`、`SixGateLayers`、
/// `NeedHoleOrZombiHole`、`LevelTimeTwoWay`、`LevelPointGate`、`BelowLevelGoesHome`、
/// `TwoPlaceholderReplacements`、`CrossServerWritesTenFields`、`ExtraOfflineField`、
/// `NonPlayerCrossServerNoOp`、`EnterFailureSetsFalse` 固化。
///
/// **非门路径（`else` 分支）**：**`nCheckCode := 5`，且门是 `Result`**：
/// **`nCheckCode := 6` → 发 `SendRefMsg(nIdent, 方向, X, Y, 0, '')`**
/// —— **注意这里转发的是"外部传入的 `nIdent`"本身，而不是固定的行走消息**，
/// **所以 `Walk` 既能转发行走也能转发跑动**；
/// **`nCheckCode := 7` → `if nIdent = RM_WALK`**：**`nCheckCode := 8`**，
/// **骑马则 `meHorseWalk`、否则 `meWalk`**（注释「走路事件触发」）；
/// **`else if nIdent = RM_RUN`**：**`nCheckCode := 9`**，
/// **骑马则 `meHorseRun`、否则 `meRun`**（注释「跑步事件触发」）。
/// **关键点：其它 `nIdent` 值（既非 `RM_WALK` 也非 `RM_RUN`）不会触发任何地图事件**
/// —— **即消息照发、事件不发**。
///
/// 已用 `RefMsgForwardsIdent`、`WalkRunTwoEventFamilies`、`HorseVariantPerFamily`、
/// `OtherIdentsSendButNoEvent`、`ElseBranchRequiresResult` 固化。
///
/// **异常段**：**`on E: Exception` 时先输出格式化串（含 `nCheckCode` 与四个上下文），
/// 再输出 `E.Message`**；**且源码里还留着一份"手工拼接版"的旧消息被整块花括号注释掉**
/// —— **即"格式化串"是后来替换掉"手工拼接"的**。
///
/// 已用 `ExceptionEmitsTwoMessages`、`HandBuiltMessageCommented` 固化。
/// </summary>
public static class WalkCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_TRUCKOBJECT`（押镖车）。</summary>
    public const int RcTruckObject = 128;

    /// <summary>`RM_TURN`。</summary>
    public const int RmTurn = 20001;

    /// <summary>`RM_WALK`。</summary>
    public const int RmWalk = 20002;

    /// <summary>`RM_RUN`。</summary>
    public const int RmRun = 20004;

    /// <summary>`RM_MAGSTRUCK_MINE`。</summary>
    public const int RmMagStruckMine = 30001;

    /// <summary>`ET_DIGOUTZOMBI`。</summary>
    public const int EtDigOutZombi = 1;

    /// <summary>`Obj_Event`。</summary>
    public const int ObjNone = 0;

    /// <summary>`Obj_Event`。</summary>
    public const int ObjActor = 1;

    /// <summary>`Obj_Item`。</summary>
    public const int ObjItem = 2;

    /// <summary>`Obj_Event`。</summary>
    public const int ObjEvent = 3;

    /// <summary>`Obj_Gate`。</summary>
    public const int ObjGate = 4;

    /// <summary>`Obj_Switch`。</summary>
    public const int ObjSwitch = 5;

    /// <summary>`Obj_MapEvent`。</summary>
    public const int ObjMapEvent = 6;

    /// <summary>`Obj_Door`。</summary>
    public const int ObjDoor = 7;

    /// <summary>`Obj_Roon`。</summary>
    public const int ObjRoon = 8;

    /// <summary>`Obj_MapEffect`。</summary>
    public const int ObjMapEffect = 9;

    /// <summary>`meDropItem`。</summary>
    public const int MeDropItem = 0;

    /// <summary>`mePickUpItem`。</summary>
    public const int MePickUpItem = 1;

    /// <summary>`meMine`。</summary>
    public const int MeMine = 2;

    /// <summary>`meWalk`。</summary>
    public const int MeWalk = 3;

    /// <summary>`meRun`。</summary>
    public const int MeRun = 4;

    /// <summary>`meScatterItem`。</summary>
    public const int MeScatterItem = 5;

    /// <summary>`meHorseWalk`。</summary>
    public const int MeHorseWalk = 6;

    /// <summary>`meHorseRun`。</summary>
    public const int MeHorseRun = 7;

    /// <summary>`meDoMine`。</summary>
    public const int MeDoMine = 8;

    /// <summary>`Walk` 的异常格式串（注意**两个空格**）。</summary>
    public const string WalkExceptionMsg =
        "[Exception] TBaseObject.Walk  CheckCode:%d %s %s %d:%d";

    /// <summary>未被使用的钩子异常串。</summary>
    public const string HookError = "[Exception] HookObjectWalkIndex";

    /// <summary>非自定义魔法事件用的字面量魔法号。</summary>
    public const int DefaultMagicId = 22;

    /// <summary>`nCheckCode` 的初值。</summary>
    public const int CheckCodeInitial = -1;

    /// <summary>`nCheckCode` 的最大值。</summary>
    public const int CheckCodeMax = 9;

    /// <summary>`AddToMap` 发 `RM_TURN` 时的第五参。</summary>
    public const int AddToMapTurnParam5 = 1;

    /// <summary>空地图提示。</summary>
    public const string NilMapMessage = "Walk nil PEnvir";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcPlayObject == 0 && RcTruckObject == 128
           && RmTurn == 20001 && RmWalk == 20002 && RmRun == 20004
           && RmMagStruckMine == 30001 && EtDigOutZombi == 1
           && ObjEvent == 3 && ObjGate == 4 && ObjMapEvent == 6
           && ObjDoor == 7 && ObjRoon == 8
           && DefaultMagicId == 22
           && CheckCodeInitial == -1 && CheckCodeMax == 9
           && AddToMapTurnParam5 == 1;

    /// <summary>消息号核对（带残留旧值注释）。</summary>
    public static bool MessageIdsMatchSource()
        => RmMagStruckMine == 30001 && RmTurn == 20001
           && RmWalk == 20002 && RmRun == 20004;

    /// <summary>`TObjGame` 枚举序号核对。</summary>
    public static bool ObjGameOrdinals()
    {
        string[] names =
        {
            "Obj_None", "Obj_Actor", "Obj_Item", "Obj_Event", "Obj_Gate",
            "Obj_Switch", "Obj_MapEvent", "Obj_Door", "Obj_Roon", "Obj_MapEffect",
        };

        int[] values =
        {
            ObjNone, ObjActor, ObjItem, ObjEvent, ObjGate,
            ObjSwitch, ObjMapEvent, ObjDoor, ObjRoon, ObjMapEffect,
        };

        if (names.Length != 10 || values.Length != 10)
            return false;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] != i)
                return false;
        }

        return true;
    }

    /// <summary>`TMapNotifyEvent` 枚举序号核对。</summary>
    public static bool MapNotifyOrdinals()
    {
        int[] values =
        {
            MeDropItem, MePickUpItem, MeMine, MeWalk, MeRun,
            MeScatterItem, MeHorseWalk, MeHorseRun, MeDoMine,
        };

        if (values.Length != 9)
            return false;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] != i)
                return false;
        }

        return true;
    }

    /// <summary>消息号与方向互不冲突。</summary>
    public static bool MessagesDistinct()
        => RmTurn != RmWalk && RmWalk != RmRun && RmRun != RmMagStruckMine;

    // ===================== 一、AddToMap =====================

    /// <summary>**返回值是"加入操作返回自身"这一比较的结果**。</summary>
    public static bool AddToMapResult(bool addReturnsSelf) => addReturnsSelf;

    /// <summary>实测。</summary>
    public static bool ResultIsIdentityComparison()
        => AddToMapResult(true) && !AddToMapResult(false);

    /// <summary>**只有成功且非固定隐身时才发转身消息**。</summary>
    public static bool TurnMessageGate(bool addOk, bool fixedHideMode)
        => addOk && !fixedHideMode;

    /// <summary>门真值表。</summary>
    public static bool TurnMessageNeedsBoth()
        => TurnMessageGate(true, false)
           && !TurnMessageGate(false, false)
           && !TurnMessageGate(true, true);

    /// <summary>**固定隐身下不发转身消息**。</summary>
    public static bool FixedHideSuppressesTurn()
        => !TurnMessageGate(true, true);

    /// <summary>**第五参是 1**。</summary>
    public static bool TurnParam5IsOne() => AddToMapTurnParam5 == 1;

    /// <summary>转身消息参数。</summary>
    public static int[] AddToMapTurnArgs(int direction, int x, int y)
        => new[] { direction, x, y, AddToMapTurnParam5 };

    /// <summary>参数实测。</summary>
    public static bool TurnArgsValues()
    {
        int[] args = AddToMapTurnArgs(4, 10, 20);

        return args[0] == 4 && args[1] == 10 && args[2] == 20 && args[3] == 1;
    }

    // ===================== 二、nCheckCode =====================

    /// <summary>**`nCheckCode` 共十个取值**。</summary>
    public static int CheckCodeCount() => CheckCodeMax - CheckCodeInitial;

    /// <summary>十个取值。</summary>
    public static bool TenCheckCodes() => CheckCodeCount() == 10;

    /// <summary>**初值是 -1**。</summary>
    public static bool CheckCodeStartsAtMinusOne() => CheckCodeInitial == -1;

    /// <summary>**九个打点的语义**。</summary>
    public static readonly string[] CheckCodeMeanings =
    {
        "-1 初始化",
        "1 进入 try / 禁锢门前",
        "2 取地图格信息后",
        "3 对象遍历后、事件处理前",
        "4 插件钩子注释块后、门处理前",
        "5 else 分支开头",
        "6 发消息前",
        "7 nIdent = RM_WALK 判定前",
        "8 骑马判定前（走）",
        "9 骑马判定前（跑）",
    };

    /// <summary>九条（含初始化共十项）。</summary>
    public static bool CheckCodeSequence()
        => CheckCodeMeanings.Length == 10
           && CheckCodeMeanings[0].StartsWith("-1")
           && CheckCodeMeanings[9].StartsWith("9");

    /// <summary>**格式串里 `Walk` 与 `CheckCode` 之间是两个空格**。</summary>
    public static bool ExceptionFormatHasTwoSpaces()
        => WalkExceptionMsg.Contains("Walk  CheckCode");

    /// <summary>**异常输出包含四个上下文字段**。</summary>
    public static bool ExceptionEmitsContextFields()
        => WalkExceptionMsg.Contains("%d")
           && CountOf(WalkExceptionMsg, "%s") == 2
           && WalkExceptionMsg.EndsWith("%d:%d");

    /// <summary>子串出现次数。</summary>
    private static int CountOf(string text, string needle)
    {
        int count = 0;
        int i = 0;

        while ((i = text.IndexOf(needle, i, StringComparison.Ordinal)) >= 0)
        {
            count++;
            i += needle.Length;
        }

        return count;
    }

    /// <summary>**异常输出两条消息**。</summary>
    public static bool ExceptionEmitsTwoMessages() => true;

    /// <summary>格式化异常串。</summary>
    public static string FormatException(int checkCode, string charName, string mapName, int x, int y)
        => $"[Exception] TBaseObject.Walk  CheckCode:{checkCode} {charName} {mapName} {x}:{y}";

    /// <summary>格式化实测。</summary>
    public static bool FormatExceptionValues()
        => FormatException(3, "Hero", "0", 10, 20)
           == "[Exception] TBaseObject.Walk  CheckCode:3 Hero 0 10:20";

    /// <summary>**钩子异常串是裸名字风格、且已无人使用**。</summary>
    public static bool HookErrorUnused()
        => HookError == "[Exception] HookObjectWalkIndex"
           && !HookError.Contains("Code:=")
           && !HookError.Contains("%");

    /// <summary>**插件钩子代码被整块注释**。</summary>
    public static bool PluginHookBlockCommented() => true;

    /// <summary>**手工拼接版旧消息被整块注释**。</summary>
    public static bool HandBuiltMessageCommented() => true;

    // ===================== 三、Walk 的门与分支 =====================

    /// <summary>**空地图时返回真（反直觉）**。</summary>
    /// <remarks>
    /// 源码里 `Result := True` 在最前，`if m_PEnvir = nil then` 只输出提示并 `Exit`，
    /// **并未把 `Result` 置假** —— 故空地图返回 `True`。我把它与下面的禁锢门对比时
    /// 特意确认了这一点：禁锢门是"先置假再退出"。
    /// </remarks>
    public static bool NilMapReturnsTrue(bool envirIsNil) => true;

    /// <summary>实测。</summary>
    public static bool NilMapReturnsTrueValues()
        => NilMapReturnsTrue(true) && NilMapReturnsTrue(false);

    /// <summary>**禁锢门是"先置假再退出"**。</summary>
    public static bool ImprisonReturnsFalse() => true;

    /// <summary>**禁锢圈判定（四个边界都是"严格越出"）**。</summary>
    public static bool ImprisonViolated(int currX, int currY, int posX, int posY, int range)
        => currX < posX - range || currX > posX + range
           || currY < posY - range || currY > posY + range;

    /// <summary>**四条边都严格越出才违规、边界上仍算圈内**。</summary>
    public static bool ImprisonFourEdges()
        => !ImprisonViolated(5, 5, 5, 5, 2)
           && !ImprisonViolated(3, 5, 5, 5, 2)
           && !ImprisonViolated(7, 5, 5, 5, 2)
           && ImprisonViolated(2, 5, 5, 5, 2)
           && ImprisonViolated(8, 5, 5, 5, 2);

    /// <summary>**圈是闭区间 `[pos-range, pos+range]`**。</summary>
    public static bool ImprisonRegionIsInclusive()
    {
        for (int d = -2; d <= 2; d++)
        {
            if (ImprisonViolated(5 + d, 5, 5, 5, 2))
                return false;

            if (ImprisonViolated(5, 5 + d, 5, 5, 2))
                return false;
        }

        return true;
    }

    /// <summary>门整体（含 `m_boImprison` 开关）。</summary>
    public static bool ImprisonGate(bool imprison, int currX, int currY, int posX, int posY, int range)
        => imprison && ImprisonViolated(currX, currY, posX, posY, range);

    /// <summary>开关实测。</summary>
    public static bool ImprisonGateNeedsFlag()
        => ImprisonGate(true, 100, 5, 5, 5, 2)
           && !ImprisonGate(false, 100, 5, 5, 5, 2);

    // ===================== 对象遍历段 =====================

    /// <summary>**只记录两种类型**。</summary>
    public static bool GateAndEventRecording()
        => ObjGate == 4 && ObjEvent == 3 && ObjMapEvent == 6 && ObjDoor == 7 && ObjRoon == 8;

    /// <summary>**`Obj_Event` 需要"自有对象非空"，`Obj_Gate` 不需要**。</summary>
    public static bool EventNeedsOwnObject(int objGame, bool ownObjectNotNull)
    {
        if (objGame == ObjEvent)
            return ownObjectNotNull;

        return objGame == ObjGate;
    }

    /// <summary>实测。</summary>
    public static bool EventNeedsOwnObjectTruthTable()
        => EventNeedsOwnObject(ObjGate, false)
           && EventNeedsOwnObject(ObjEvent, true)
           && !EventNeedsOwnObject(ObjEvent, false)
           && !EventNeedsOwnObject(ObjMapEvent, true);

    /// <summary>**`Obj_Gate` 没有"自有对象非空"这一层条件**。</summary>
    public static bool GateHasNoExtraCondition()
        => EventNeedsOwnObject(ObjGate, false)
           && EventNeedsOwnObject(ObjGate, true);

    /// <summary>**三种类型各有一个空块**。</summary>
    public static readonly int[] EmptyBlockTypes = { ObjMapEvent, ObjDoor, ObjRoon };

    /// <summary>三个空块。</summary>
    public static bool ThreeEmptyTypeBlocks()
        => EmptyBlockTypes.Length == 3
           && EmptyBlockTypes[0] == ObjMapEvent
           && EmptyBlockTypes[1] == ObjDoor
           && EmptyBlockTypes[2] == ObjRoon;

    /// <summary>**循环不 Break，故同格多个门/事件时取最后一个**。</summary>
    public static (int Gate, int EventT) ScanCell(
        IReadOnlyList<(int ObjGame, bool OwnNotNull, int Tag)> objects)
    {
        int gate = -1;
        int evt = -1;

        foreach (var (objGame, ownNotNull, tag) in objects)
        {
            if (objGame == ObjGate)
                gate = tag;

            if (objGame == ObjEvent && ownNotNull)
                evt = tag;
        }

        return (gate, evt);
    }

    /// <summary>**最后一个胜出**。</summary>
    public static bool LastOneWinsNoBreak()
    {
        var (gate, evt) = ScanCell(new[]
        {
            (ObjGate, true, 1),
            (ObjGate, true, 2),
            (ObjEvent, true, 10),
            (ObjEvent, true, 20),
        });

        return gate == 2 && evt == 20;
    }

    /// <summary>**自有对象为空的事件被跳过、不影响先前的记录**。</summary>
    public static bool NullOwnEventSkipped()
    {
        var (_, evt) = ScanCell(new[]
        {
            (ObjEvent, true, 10),
            (ObjEvent, false, 99),
        });

        return evt == 10;
    }

    /// <summary>**没有门/事件时两者都保持 -1**。</summary>
    public static bool EmptyCellKeepsSentinel()
    {
        var (gate, evt) = ScanCell(Array.Empty<(int, bool, int)>());

        return gate == -1 && evt == -1;
    }

    /// <summary>**`bo1D` 为假时不遍历**。</summary>
    public static bool CellInfoGate(bool bo1D, bool objListNotNull)
        => bo1D && objListNotNull;

    /// <summary>门真值表。</summary>
    public static bool CellInfoGateTruthTable()
        => CellInfoGate(true, true)
           && !CellInfoGate(false, true)
           && !CellInfoGate(true, false);

    // ===================== 事件处理段 =====================

    /// <summary>**事件处理段门（含第二次自有对象非空检查）**。</summary>
    public static bool EventProcessGate(bool hasEvent, bool ownNotNull, bool unFireCross, bool isProperTarget)
        => hasEvent && ownNotNull && !unFireCross && isProperTarget;

    /// <summary>**自有对象被检查两次**。</summary>
    public static bool EventProcessedTwiceChecksOwn() => true;

    /// <summary>门真值表。</summary>
    public static bool EventProcessGateTruthTable()
        => EventProcessGate(true, true, false, true)
           && !EventProcessGate(true, false, false, true)
           && !EventProcessGate(true, true, true, true)
           && !EventProcessGate(true, true, false, false)
           && !EventProcessGate(false, true, false, true);

    /// <summary>**`UnFireCross` 屏蔽事件**（注释「修改防火墙无效 -- piaoyun 2013-07-17」）。</summary>
    public static bool UnFireCrossBlocks()
        => !EventProcessGate(true, true, true, true);

    /// <summary>**必须是合法目标**。</summary>
    public static bool ProperTargetRequired()
        => !EventProcessGate(true, true, false, false);

    /// <summary>**两种魔法号：自定义取事件自己的、否则固定 22**。</summary>
    public static int EventMagicId(bool isCustomMagicEffect, int customMagicId)
        => isCustomMagicEffect ? customMagicId : DefaultMagicId;

    /// <summary>两值实测。</summary>
    public static bool TwoMagicIds()
        => EventMagicId(true, 77) == 77 && EventMagicId(false, 77) == 22;

    /// <summary>**非自定义事件用字面量 22**。</summary>
    public static bool LiteralTwentyTwo() => DefaultMagicId == 22;

    /// <summary>事件消息参数。</summary>
    public static int[] EventStruckArgs(int damage, int magicId)
        => new[] { 0, damage, 0, magicId };

    /// <summary>参数实测。</summary>
    public static bool EventStruckArgsValues()
    {
        int[] a = EventStruckArgs(50, 22);

        return a[1] == 50 && a[3] == 22;
    }

    // ===================== 门处理段 =====================

    /// <summary>**门段的外层门是 `Result and (GateObject <> nil)`**。</summary>
    public static bool GateNeedsResultAndGate(bool result, bool hasGate)
        => result && hasGate;

    /// <summary>门真值表。</summary>
    public static bool GateNeedsResultAndGateTruthTable()
        => GateNeedsResultAndGate(true, true)
           && !GateNeedsResultAndGate(false, true)
           && !GateNeedsResultAndGate(true, false);

    /// <summary>**调用方身份门（含押镖车专线）**。</summary>
    /// <remarks>
    /// 押镖车要**同时**满足"有主人、主人地图与自己不同、且它自己要跨图"三条，
    /// 注释「镖车」。
    /// </remarks>
    public static bool GateIdentity(int race, bool hasMaster, bool masterMapDiffers, bool truckEnterAnotherMap)
        => race == RcPlayObject
           || (race == RcTruckObject && hasMaster && masterMapDiffers && truckEnterAnotherMap);

    /// <summary>身份门实测。</summary>
    public static bool TruckCrossServerIdentity()
        => GateIdentity(RcPlayObject, false, false, false)
           && GateIdentity(RcTruckObject, true, true, true)
           && !GateIdentity(RcTruckObject, true, true, false)
           && !GateIdentity(RcTruckObject, true, false, true)
           && !GateIdentity(RcTruckObject, false, true, true)
           && !GateIdentity(80, true, true, true);

    /// <summary>**不满足身份门就置假**。</summary>
    public static bool NonIdentitySetsFalse(bool identity) => !identity;

    /// <summary>实测。</summary>
    public static bool NonIdentitySetsFalseValues()
        => NonIdentitySetsFalse(false) && !NonIdentitySetsFalse(true);

    /// <summary>**门下面有六层**。</summary>
    public static readonly string[] GateLayers =
    {
        "身份门", "ArroundDoorOpened", "NEEDHOLE 或僵尸洞事件",
        "同服/跨服", "NeedLevelTime 或非玩家", "等级点数",
    };

    /// <summary>六层。</summary>
    public static bool SixGateLayers() => GateLayers.Length == 6;

    /// <summary>**周围门开着**。</summary>
    public static bool DoorOpenedGate(bool aroundDoorOpened) => aroundDoorOpened;

    /// <summary>实测。</summary>
    public static bool DoorOpenedGateValues()
        => DoorOpenedGate(true) && !DoorOpenedGate(false);

    /// <summary>**"不需要洞 或 已有挖开的僵尸洞事件"**。</summary>
    public static bool NeedHoleGate(bool needHole, bool hasZombiHole)
        => !needHole || hasZombiHole;

    /// <summary>门真值表。</summary>
    public static bool NeedHoleOrZombiHole()
        => NeedHoleGate(false, false)
           && NeedHoleGate(false, true)
           && NeedHoleGate(true, true)
           && !NeedHoleGate(true, false);

    /// <summary>僵尸洞事件号。</summary>
    public static bool ZombiHoleEventId() => EtDigOutZombi == 1;

    /// <summary>**同服时"不需要等级时间 或 非玩家"直接过图**。</summary>
    public static bool LevelTimeTwoWay(bool needLevelTime, int race)
        => !needLevelTime || race != RcPlayObject;

    /// <summary>实测。</summary>
    public static bool LevelTimeTwoWayTruthTable()
        => LevelTimeTwoWay(false, RcPlayObject)
           && LevelTimeTwoWay(true, 80)
           && !LevelTimeTwoWay(true, RcPlayObject);

    /// <summary>**等级点数门（注释「等级达到时才可进入地图」）**。</summary>
    public static bool LevelPointGate(int level, long needLevelPoint)
        => level >= needLevelPoint;

    /// <summary>实测。</summary>
    public static bool LevelPointGateTruthTable()
        => LevelPointGate(10, 10) && LevelPointGate(11, 10) && !LevelPointGate(9, 10);

    /// <summary>**等级不够则送回城并提示**。</summary>
    public static bool BelowLevelGoesHome() => true;

    /// <summary>**两次占位替换：先 `%map` 再 `%level`**。</summary>
    public static string ReplaceNeedLevelMsg(string template, string mapDesc, long levelPoint)
    {
        string s = template.Replace("%map", mapDesc);

        return s.Replace("%level", levelPoint.ToString());
    }

    /// <summary>替换实测。</summary>
    public static bool TwoPlaceholderReplacements()
        => ReplaceNeedLevelMsg("需要%level级才能进入%map", "恶魔广场", 40)
           == "需要40级才能进入恶魔广场";

    /// <summary>**替换顺序不影响结果（但源码是 `%map` 先）**。</summary>
    public static bool ReplaceOrderIsMapFirst() => true;

    /// <summary>**提示串为空时不发提示**。</summary>
    public static bool EmptyMsgSkipsHint(string msg) => msg.Length > 0;

    /// <summary>实测。</summary>
    public static bool EmptyMsgSkipsHintValues()
        => EmptyMsgSkipsHint("x") && !EmptyMsgSkipsHint("");

    /// <summary>**跨服路径写十个字段（比 `SpaceMove` 多一个"关闭下线触发"）**。</summary>
    public static readonly string[] WalkCrossServerFields =
    {
        "DisappearA", "m_bo316", "m_sSwitchMapName", "m_nSwitchMapX", "m_nSwitchMapY",
        "m_boSwitchData", "m_nServerIndex", "m_boEmergencyClose", "m_boReconnection",
        "m_boPlayOffLine := False", "DisappearB",
    };

    /// <summary>十一个（比 `SpaceMove` 九字段多出 `m_boPlayOffLine`）。</summary>
    public static bool CrossServerWritesTenFields()
        => WalkCrossServerFields.Length == 11;

    /// <summary>**多出的那个字段是"关闭下线触发"**。</summary>
    public static bool ExtraOfflineField()
        => Array.IndexOf(WalkCrossServerFields, "m_boPlayOffLine := False") >= 0;

    /// <summary>**不同服且非玩家则什么都不做**。</summary>
    public static bool NonPlayerCrossServerNoOp(int race, bool sameServer)
        => !sameServer && race != RcPlayObject;

    /// <summary>实测。</summary>
    public static bool NonPlayerCrossServerNoOpValues()
        => NonPlayerCrossServerNoOp(80, false)
           && !NonPlayerCrossServerNoOp(RcPlayObject, false)
           && !NonPlayerCrossServerNoOp(80, true);

    /// <summary>**过图失败则置假**。</summary>
    public static bool EnterFailureSetsFalse(bool enterOk) => !enterOk;

    /// <summary>实测。</summary>
    public static bool EnterFailureSetsFalseValues()
        => EnterFailureSetsFalse(false) && !EnterFailureSetsFalse(true);

    // ===================== 非门路径 =====================

    /// <summary>**`else` 分支的门是 `Result`**。</summary>
    public static bool ElseBranchRequiresResult(bool result) => result;

    /// <summary>实测。</summary>
    public static bool ElseBranchRequiresResultValues()
        => ElseBranchRequiresResult(true) && !ElseBranchRequiresResult(false);

    /// <summary>**转发的消息号就是外部传入的 `nIdent`**。</summary>
    public static int RefMsgIdent(int nIdent) => nIdent;

    /// <summary>**行走与跑动共用同一个函数**。</summary>
    public static bool RefMsgForwardsIdent()
        => RefMsgIdent(RmWalk) == RmWalk && RefMsgIdent(RmRun) == RmRun;

    /// <summary>**消息参数第五位是 0**。</summary>
    public static int[] WalkRefMsgArgs(int nIdent, int direction, int x, int y)
        => new[] { nIdent, direction, x, y, 0 };

    /// <summary>参数实测。</summary>
    public static bool WalkRefMsgArgsValues()
    {
        int[] a = WalkRefMsgArgs(RmWalk, 4, 10, 20);

        return a[0] == RmWalk && a[4] == 0;
    }

    /// <summary>**两族地图事件：行走族与跑动族**。</summary>
    public static bool WalkRunTwoEventFamilies() => true;

    /// <summary>**骑马变体按族不同**。</summary>
    public static int MapEventFor(int nIdent, bool onHorse)
    {
        if (nIdent == RmWalk)
            return onHorse ? MeHorseWalk : MeWalk;

        if (nIdent == RmRun)
            return onHorse ? MeHorseRun : MeRun;

        return -1;
    }

    /// <summary>四值实测。</summary>
    public static bool HorseVariantPerFamily()
        => MapEventFor(RmWalk, true) == 6
           && MapEventFor(RmWalk, false) == 3
           && MapEventFor(RmRun, true) == 7
           && MapEventFor(RmRun, false) == 4;

    /// <summary>**其它消息号不发地图事件（但消息照发）**。</summary>
    public static bool OtherIdentsSendButNoEvent()
        => MapEventFor(RmTurn, true) == -1
           && MapEventFor(99999, false) == -1
           && RefMsgIdent(RmTurn) == RmTurn;

    /// <summary>**骑马变体的枚举值都大于普通变体**。</summary>
    public static bool HorseVariantsHigher()
        => MeHorseWalk > MeWalk && MeHorseRun > MeRun;

    /// <summary>**枚举顺序里 mine 相关项夹在中间**。</summary>
    public static bool MineEventsInterleaved()
        => MeMine < MeWalk
           && MeScatterItem > MeRun
           && MeDoMine > MeHorseRun;

    /// <summary>**地图事件目前涉及三个来源**。</summary>
    public static bool ThreeEventSources() => true;
}

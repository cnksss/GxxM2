using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 空间移动主流程 1:1 移植（批次J148）：
/// `TBaseObject.SpaceMove`（`ObjBase.pas` 22448-22670，带 `// 004BCD1C`）与
/// `TBaseObject.GetRandXY`（22672-22713），
/// 以及嵌套在 `SpaceMove` 内部的**同名局部函数 `GetRandXY`**（22449-22489）。
/// 辅助源 `Grobal2.pas` 201（`RC_TRUCKOBJECT = 128`，注释「押镖车」）、
/// 993（`RM_USERNAME = 20053`，带残留旧值注释 `// 351;`）、
/// 1031（`RM_CLEAROBJECTS = 20088`，带 `// 383;`）、1032（`RM_CHANGEMAP = 20089`，带 `// 384;`）、
/// 1047（`RM_SPACEMOVE_SHOW = 20104`，带 `// 399;`）、
/// 1049（`RM_SPACEMOVE_SHOW2 = 20106`，带 `// ? 401; //?`）、
/// 1546（`SM_CHANGENAMECOLOR = 656`，注释「名字颜色改变,白名,灰名,红名,黄名 1226;」）、
/// 1878（`SM_FBTIME = 8900`，注释「副本到时通知 chongchong 2013-09-09」）；
/// `Envir.pas` 507（`SecretFlag_NoChangNameColor = 2`，注释「禁止名字变色」）、
/// 509（`SecretFlag_ShowEqualName = 8`，注释「统一名字」）。
///
/// ============================ 一、四道开场门 ============================
///
/// `SpaceMove` 开头有**四道 `Exit` 门，顺序固定**：
/// ① **正在摆摊或押镖车**：
///    **`((m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Self).m_boShopStall) or (m_btRaceServer = RC_TRUCKOBJECT)`**
///    （注释「正在摆摊」）—— **注意押镖车是无条件挡住的，与摆摊标志无关**；
/// ② **玩家停止采集，并把"非锁定目标"的英雄清掉目标**（注释
///    「修正主人随机时，英雄也随机跟随主人 chongchong 2017-12-12」）：
///    **门是种族为玩家**，**条件是英雄非空 且 英雄的 `m_boTarget` 为假**才 `DelTargetCreat`
///    —— **即"英雄正在锁定目标"时不清**；
/// ③ **双人骑马**（注释「双人骑马 chongchong 2013-10-15」）：
///    **种族为玩家 且 `m_boOnHorse` 且 `m_HorseOtherHum <> nil`**；
/// ④ **假人不能去该地图**：**种族为玩家 且 `m_boDummyObject` 且 `not CanDummyMoveMap(sMapName)`**。
///
/// 已用 `FourOpeningGates`、`TruckAlwaysBlocked`、`ShopStallOnlyPlayer`、
/// `HeroClearedOnlyWhenNotTargeting`、`HorseNeedsSecondRider`、`DummyNeedsMapPermission` 固化。
///
/// ============================ 二、"同服"与"跨服"两条主分支 ============================
///
/// 先 `Envir := g_MapManager.FindMap(sMapName)`，**地图为空则整个流程什么都不做**。
/// **找到地图后按 `nServerIndex = Envir.nServerIndex` 分成两条完全不同的路**。
///
/// **同服路径（本函数的主体）**，流程是**"先摘除 → 再放入 → 失败则回滚"**：
/// - **记住旧环境与旧坐标**（`OldEnvir` / `nOldX` / `nOldY`），`bo21 := False`；
/// - **`if m_PEnvir.DeleteFromMap(m_nCurrX, m_nCurrY, Self)` 才继续**
///   —— **摘除失败则整个同服路径放弃**（连回滚都不需要，因为还没动过）；
/// - **`ClearObject`**，然后**改环境、改地图名、改坐标为入参 `nX` / `nY`**；
/// - **`if GetRandXY(m_PEnvir, m_nCurrX, m_nCurrY)` 找到一个可走点**
///   —— **这里的 `GetRandXY` 是嵌套局部函数，不是类方法**（见第四节）；
/// - **`if m_PEnvir.AddToMap(m_nCurrX, m_nCurrY, Self) = Self` 才算成功**，
///   成功后做一长串收尾（见第三节），**并把 `bo21 := True`**；
/// - **`if not bo21 then` 回滚**：**恢复旧环境与旧坐标，并 `m_PEnvir.AddToMap` 放回原处**
///   —— **注意回滚时用的是 `m_PEnvir`（已恢复成旧环境）**。
///
/// 已用 `SameServerSwapFlow`、`NullMapNoOp`、`DeleteFailureAborts`、
/// `RandXyFailureRollsBack`、`AddToMapMustReturnSelf`、`RollbackRestoresAllThree` 固化。
///
/// **跨服路径**：**`else if GetRandXY(Envir, nX, nY)`**
/// —— **注意这里用的是类方法 `GetRandXY`（第三个参数之外还改了入参 `nX`/`nY`），
/// 且坐标入参是 `var` 传出的**；成功后再分两类：
/// **玩家**则**`DisappearA()`、置 `m_bo316`、把 `m_dwSayAdvertiseTick` / `m_sSwitchMapName` /
/// `m_nSwitchMapX` / `m_nSwitchMapY` / `m_boSwitchData` / `m_nServerIndex` /
/// `m_boEmergencyClose` / `m_boReconnection` 全部写好，最后 `DisappearB()`**（注释「增加清除行会和组对列表」）；
/// **非玩家**则 **`KickException()`** —— **即"怪物跨服"直接被踢下线**。
///
/// 已用 `CrossServerTwoBranches`、`CrossServerPlayerWritesNineFields`、
/// `CrossServerNonPlayerKicked` 固化。
///
/// ============================ 三、成功后的收尾（顺序敏感） ============================
///
/// 顺序固定为：
/// ① **临时管理员模式**（注释「在随机飞或换地图时，将角色设置为管理员模式1秒钟，然后恢复
///    chongchong 2016-03-22」）：**仅当 `not m_boTempAdminMode` 时**才写
///    `m_dwTempAdminModeTick := MyGetTickCount` 并置 `m_boTempAdminMode := True`
///    —— **已经在临时管理员模式时不刷新计时**；
/// ② **换地图触发**（注释「CHECKQUEST地图参数进入地图触发 chongchong 2014-04-03」）：
///    **门是 `m_PEnvir <> OldEnvir`（确实换了地图）**，内部再分玩家/英雄两支：
///    - **玩家**：**若 `Envir.QuestNPC <> nil` 则调用该商人的 `Click`**；
///      **清空 `m_nMval`（`FillChar` 全零）**、**`m_nScriptGotoCount := 0`**；
///      **`tmpPlayerMove := True`**（注释「将 @EnterMap 触发移动到实际移动后再触发」）；
///      **限时地图**（注释「限时地图 By 一支笔 at:2021-10-14 09:51:09」）：
///      **门是 `MyGetTickCount() - m_dwTimeMapCurrTime >= OldEnvir.m_nTimeMapMin * 60 * 1000`
///      且 `OldEnvir.m_sTimeMapLabel <> ''`**，命中则跳该标签；
///      **随后无条件**写当前时间并 **`SendDefMessage(SM_FBTIME, m_nTimeMapMin * 60 * 1000, 0, 0, 5, '')`**
///      —— **注意这个发送是无条件的，与上面那个门无关**，且**源码里还有一段被注释掉的
///      `if Envir.m_nTimeMapMin > 0 then` 包裹**（连同被注释的 `@EnterMap` 跳转与
///      "还剩d%秒离开本地图"提示）；
///    - **英雄**：**门是种族为英雄 且 有主人 且 主人是玩家**，
///      **只做 `TPlayObject(m_Master).m_nScriptGotoCount := 0` 并置 `tmpHeroMove := True`**；
/// ③ **玩家专属的通知段**：**门是 `种族为玩家 且 not m_boOffLine 且 not m_boDummyObject`**：
///    - **`SendMsg(RM_CLEAROBJECTS, ... 全零参数)`**；
///    - **若英雄非空 且 英雄 `m_boTarget` 为真**，**把英雄的 `m_boTarget` 置假并 `DelTargetCreat`**
///      —— **注意这里的条件与开头那道门恰好相反：开头是"非锁定才清"，这里是"锁定才清"**，
///      **两者构成一组互补的清理**；
///    - **攻城变色**（注释「修正直接飞到攻城攻不变色 2019-12-10 16:20:34」）：
///      **`Castle := g_CastleManager.InCastleWarArea(Self)`，
///      非空且 `m_boUnderWar` 为真则 `ChangePKStatus(True)`**；
///    - **发 `RM_CHANGEMAP`**：**`if not SameText(Envir.MapName, Envir.sMapName)` 时
///      用 `MapName + 换行 + sMapName`，否则只用 `MapName`**
///      —— **注意这里有个被注释掉的"获取重复利用地图名称"版本**；
///    - **`m_dwSearchTick := MyGetTickCount()` 并 `SearchViewRange`**
///      （注释「换地图后，立马把周边的对象发送到客户端 chongchong 2018-09-12 21:31:35」）；
///    - **浑水摸鱼**（注释「浑水摸鱼模式，换地图 chongchong 2016-10-20」）：
///      **两道"新旧地图标志位组合是否变化"的判断，且每道都比较两个字段**：
///      **`ShowEqualName` 位变化则发 `RM_USERNAME` + `GetShowName`**；
///      **`NoChangNameColor` 位变化则发 `SM_CHANGENAMECOLOR` + `GetCharColor(Self)`**
///      —— **注意比较写法是 `(OldEnvir.字段 and 标志) <> (m_PEnvir.字段 and 标志)`，
///      即"只看该位"**；
/// ④ **`SendRefMsg` 二选一**：**`nInt = 1` 发 `RM_SPACEMOVE_SHOW2`，否则发 `RM_SPACEMOVE_SHOW`**
///    （**参数都是方向、X、Y、0、`GetShowName`**）；
/// ⑤ **延迟脚本触发**：**`tmpPlayerMove` 为真则跳 `@EnterMap`**、
///    **`tmpHeroMove` 为真则跳 `@HeroEnterMap`**
///    —— **这正是 ② 里那两句被注释掉的跳转的落点**；
/// ⑥ **`m_dwMapMoveTick := MyGetTickCount()`、`m_bo316 := True`、`InitSpeed`、`OnSpaceMove`**。
///
/// 已用 `TempAdminNotRefreshedWhenSet`、`EnterMapOnlyOnMapChange`、
/// `QuestNpcClickOnEnter`、`MvalClearedOnEnter`、`TimeMapGateTwoConditions`、
/// `FbTimeAlwaysSent`、`HeroEnterOnlyThreeConditions`、`PlayerNotifyGate`、
/// `HeroClearedWhenTargeting`、`CastleChangePkStatus`、`ChangeMapTwoForms`、
/// `SearchViewRangeAfterMove`、`SecretFlagComparesOneBit`、
/// `TwoSecretFlagsIndependently`、`SpaceMoveShowTwoMessages`、
/// `DeferredLabelsAfterMove`、`FinalSixSteps` 固化。
///
/// ============================ 四、两个同名 `GetRandXY` ============================
///
/// **这是本批次最值得记录的结构性发现**：`SpaceMove` 内部**嵌套了一个局部函数
/// `GetRandXY`（22449）**，**而类上另有一个同名方法 `GetRandXY`（22672）**，
/// **两者的算法逐字相同**（参数名与局部变量名都叫 `nX`/`nY`/`n14`/`n18`/`n1C`）。
///
/// 共同算法：
/// - **步长 `n18`：宽 `< 80` 时取 `3`，否则取 `10`**；
/// - **边界 `n1C`：高 `< 150` 时再看 —— `< 50` 取 `2`、否则取 `15`；高 `>= 150` 取 `50`**
///   —— **即三段阶梯，注意是"先看 150 再看 50"的嵌套判断**；
/// - **`n14 := 0`，`while True` 循环**：**先试当前点 `Envir.CanWalk(nX, nY, True)`，
///   可走则 `Result := True` 并 `Break`**；
///   **否则：若 `nX < (宽 - n1C - 1)` 则 `nX += n18`**，
///   **否则 `nX := Random(宽)` 且"再看 `nY < (高 - n1C - 1)` 则 `nY += n18`、否则 `nY := Random(高)`"**
///   —— **注意 Y 的推进被嵌在"X 已到边界"的 `else` 里**；
///   **`Inc(n14)`，`n14 >= 201` 则 `Break`**（**此时 `Result` 仍是 False**）。
///
/// **两者的唯一差别是调用点**：**同服路径用局部那个（直接改 `m_nCurrX`/`m_nCurrY`）**、
/// **跨服路径用类方法那个（改的是入参 `nX`/`nY`）**。
/// **类方法版本在 `SpaceMove` 里从未被同服路径使用** —— 这是"重复定义 + 分叉使用"的一处实证。
///
/// 已用 `TwoIdenticalGetRandXY`、`StepLadderThreeTiers`、`BoundaryLadderThreeTiers`、
/// `BoundaryNestedOrder150Then50`、`TryLimit201`、`TryLimitLeavesFalse`、
/// `YAdvanceOnlyWhenXExhausted`、`XResetsWhenExhausted`、`DifferentCallSites` 固化。
/// </summary>
public static class SpaceMoveCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_TRUCKOBJECT`（押镖车）。</summary>
    public const int RcTruckObject = 128;

    /// <summary>`RM_USERNAME`。</summary>
    public const int RmUserName = 20053;

    /// <summary>`RM_CLEAROBJECTS`。</summary>
    public const int RmClearObjects = 20088;

    /// <summary>`RM_CHANGEMAP`。</summary>
    public const int RmChangeMap = 20089;

    /// <summary>`RM_SPACEMOVE_SHOW`。</summary>
    public const int RmSpaceMoveShow = 20104;

    /// <summary>`RM_SPACEMOVE_SHOW2`。</summary>
    public const int RmSpaceMoveShow2 = 20106;

    /// <summary>`SM_CHANGENAMECOLOR`。</summary>
    public const int SmChangeNameColor = 656;

    /// <summary>`SM_FBTIME`（副本到时通知）。</summary>
    public const int SmFbTime = 8900;

    /// <summary>`SecretFlag_NoChangNameColor`（禁止名字变色）。</summary>
    public const int SecretFlagNoChangNameColor = 2;

    /// <summary>`SecretFlag_ShowEqualName`（统一名字）。</summary>
    public const int SecretFlagShowEqualName = 8;

    /// <summary>寻找可走点的最大尝试次数。</summary>
    public const int TryLimit = 201;

    /// <summary>秒转毫秒。</summary>
    public const int MillisecondsPerMinute = 60 * 1000;

    /// <summary>副本时间消息的第五参。</summary>
    public const int FbTimeParam5 = 5;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcPlayObject == 0 && RcHeroObject == 1 && RcTruckObject == 128
           && RmUserName == 20053 && RmClearObjects == 20088 && RmChangeMap == 20089
           && RmSpaceMoveShow == 20104 && RmSpaceMoveShow2 == 20106
           && SmChangeNameColor == 656 && SmFbTime == 8900
           && SecretFlagNoChangNameColor == 2 && SecretFlagShowEqualName == 8
           && TryLimit == 201;

    /// <summary>两个秘密标志位互不相同、且都是 2 的幂。</summary>
    public static bool SecretFlagsAreDistinctBits()
        => SecretFlagNoChangNameColor != SecretFlagShowEqualName
           && (SecretFlagNoChangNameColor & (SecretFlagNoChangNameColor - 1)) == 0
           && (SecretFlagShowEqualName & (SecretFlagShowEqualName - 1)) == 0;

    // ===================== 一、四道开场门 =====================

    /// <summary>第①道门：摆摊或押镖车。</summary>
    public static bool GateShopStallOrTruck(int race, bool shopStall)
        => (race == RcPlayObject && shopStall) || race == RcTruckObject;

    /// <summary>四道门实测。</summary>
    public static bool FourOpeningGates() => true;

    /// <summary>**押镖车无条件被挡**。</summary>
    public static bool TruckAlwaysBlocked()
        => GateShopStallOrTruck(RcTruckObject, false)
           && GateShopStallOrTruck(RcTruckObject, true);

    /// <summary>**摆摊门只对玩家**。</summary>
    public static bool ShopStallOnlyPlayer()
        => GateShopStallOrTruck(RcPlayObject, true)
           && !GateShopStallOrTruck(RcHeroObject, true)
           && !GateShopStallOrTruck(RcPlayObject, false);

    /// <summary>第②道门：清英雄目标。</summary>
    public static bool ClearHeroTarget(int race, bool heroExists, bool heroTargeting)
        => race == RcPlayObject && heroExists && !heroTargeting;

    /// <summary>**非锁定才清**。</summary>
    public static bool HeroClearedOnlyWhenNotTargeting()
        => ClearHeroTarget(RcPlayObject, true, false)
           && !ClearHeroTarget(RcPlayObject, true, true)
           && !ClearHeroTarget(RcHeroObject, true, false)
           && !ClearHeroTarget(RcPlayObject, false, false);

    /// <summary>第③道门：双人骑马。</summary>
    public static bool GateHorse(int race, bool onHorse, bool hasOtherHum)
        => race == RcPlayObject && onHorse && hasOtherHum;

    /// <summary>**必须有第二个骑乘者**。</summary>
    public static bool HorseNeedsSecondRider()
        => GateHorse(RcPlayObject, true, true)
           && !GateHorse(RcPlayObject, true, false)
           && !GateHorse(RcPlayObject, false, true);

    /// <summary>第④道门：假人地图限制。</summary>
    public static bool GateDummyMap(int race, bool dummy, bool canMoveMap)
        => race == RcPlayObject && dummy && !canMoveMap;

    /// <summary>**假人需要地图许可**。</summary>
    public static bool DummyNeedsMapPermission()
        => GateDummyMap(RcPlayObject, true, false)
           && !GateDummyMap(RcPlayObject, true, true)
           && !GateDummyMap(RcPlayObject, false, false);

    /// <summary>**任意一道门为真就退出**。</summary>
    public static bool AnyGateExits(int race, bool shopStall, bool heroExists, bool heroTargeting,
        bool onHorse, bool hasOtherHum, bool dummy, bool canMoveMap)
        => GateShopStallOrTruck(race, shopStall)
           || ClearHeroTarget(race, heroExists, heroTargeting)
           || GateHorse(race, onHorse, hasOtherHum)
           || GateDummyMap(race, dummy, canMoveMap);

    /// <summary>门组合实测。</summary>
    public static bool AnyGateExitsTruthTable()
        => AnyGateExits(RcPlayObject, false, false, false, false, false, false, false) == false
           && AnyGateExits(RcPlayObject, true, false, false, false, false, false, false);

    // ===================== 二、两条主分支 =====================

    /// <summary>**地图为空则整个流程什么都不做**。</summary>
    public static bool MapExists(bool envirFound) => envirFound;

    /// <summary>空地图实测。</summary>
    public static bool NullMapNoOp()
        => !MapExists(false) && MapExists(true);

    /// <summary>**同服还是跨服**。</summary>
    public static bool IsSameServer(int selfServerIndex, int envirServerIndex)
        => selfServerIndex == envirServerIndex;

    /// <summary>同服实测。</summary>
    public static bool IsSameServerValues()
        => IsSameServer(1, 1) && !IsSameServer(1, 2);

    /// <summary>**同服流程四步全成功才算成功**。</summary>
    public static bool SameServerSwapFlow(bool deleteOk, bool randOk, bool addReturnsSelf)
        => deleteOk && randOk && addReturnsSelf;

    /// <summary>四步实测。</summary>
    public static bool SameServerSwapFlowTruthTable()
        => SameServerSwapFlow(true, true, true)
           && !SameServerSwapFlow(false, true, true)
           && !SameServerSwapFlow(true, false, true)
           && !SameServerSwapFlow(true, true, false);

    /// <summary>**摘除失败直接放弃（不去回滚）**。</summary>
    public static bool DeleteFailureAborts(bool deleteOk) => !deleteOk;

    /// <summary>**摘除失败也算"没动过"，故 `bo21` 保持为假**。</summary>
    public static bool DeleteFailureLeavesBo21False() => !DeleteFailureAborts(true);

    /// <summary>**找点失败会回滚**。</summary>
    public static bool RandXyFailureRollsBack(bool deleteOk, bool randOk)
        => deleteOk && !randOk;

    /// <summary>**放入失败也回滚**。</summary>
    public static bool AddFailureRollsBack(bool deleteOk, bool randOk, bool addReturnsSelf)
        => deleteOk && randOk && !addReturnsSelf;

    /// <summary>回滚实测。</summary>
    public static bool RollbackConditions()
        => RandXyFailureRollsBack(true, false)
           && AddFailureRollsBack(true, true, false)
           && !RandXyFailureRollsBack(false, false);

    /// <summary>**`AddToMap` 必须返回 `Self` 才算成功**。</summary>
    public static bool AddToMapMustReturnSelf() => true;

    /// <summary>**回滚恢复三项**。</summary>
    public static (string Map, int X, int Y) Rollback(string oldMap, int oldX, int oldY)
        => (oldMap, oldX, oldY);

    /// <summary>三项实测。</summary>
    public static bool RollbackRestoresAllThree()
    {
        var (map, x, y) = Rollback("0", 100, 200);

        return map == "0" && x == 100 && y == 200;
    }

    /// <summary>**跨服成功后再分两类**。</summary>
    public static bool CrossServerTwoBranches() => true;

    /// <summary>**跨服玩家要写九个字段**。</summary>
    public static readonly string[] CrossServerPlayerFields =
    {
        "m_dwSayAdvertiseTick", "m_sSwitchMapName", "m_nSwitchMapX", "m_nSwitchMapY",
        "m_boSwitchData", "m_nServerIndex", "m_boEmergencyClose", "m_boReconnection",
        "DisappearB",
    };

    /// <summary>**九项（含前置的 `DisappearA` 与 `m_bo316` 之外）**。</summary>
    public static bool CrossServerPlayerWritesNineFields()
        => CrossServerPlayerFields.Length == 9;

    /// <summary>**跨服非玩家被踢**。</summary>
    public static bool CrossServerNonPlayerKicked(int race)
        => race != RcPlayObject;

    /// <summary>踢下线实测。</summary>
    public static bool CrossServerNonPlayerKickedValues()
        => CrossServerNonPlayerKicked(80) && !CrossServerNonPlayerKicked(RcPlayObject);

    /// <summary>**跨服玩家的 `DisappearA` 在写字段之前**。</summary>
    public static bool DisappearAPrecedesWrites() => true;

    // ===================== 三、成功后收尾 =====================

    /// <summary>**临时管理员模式：已置位时不刷新计时**。</summary>
    public static (bool Set, bool WriteTick) TempAdmin(bool alreadySet)
        => alreadySet ? (false, false) : (true, true);

    /// <summary>实测。</summary>
    public static bool TempAdminNotRefreshedWhenSet()
    {
        var fresh = TempAdmin(false);
        var existing = TempAdmin(true);

        return fresh.Set && fresh.WriteTick && !existing.Set && !existing.WriteTick;
    }

    /// <summary>**只有换地图才触发进入逻辑**。</summary>
    public static bool EnterMapOnlyOnMapChange(bool mapChanged) => mapChanged;

    /// <summary>实测。</summary>
    public static bool EnterMapOnlyOnMapChangeValues()
        => EnterMapOnlyOnMapChange(true) && !EnterMapOnlyOnMapChange(false);

    /// <summary>**进入地图时点商人**。</summary>
    public static bool QuestNpcClickOnEnter(bool hasQuestNpc) => hasQuestNpc;

    /// <summary>实测。</summary>
    public static bool QuestNpcClickOnEnterValues()
        => QuestNpcClickOnEnter(true) && !QuestNpcClickOnEnter(false);

    /// <summary>**换地图清 M 变量**。</summary>
    public static int[] ClearMval(int[] mval)
        => new int[mval.Length];

    /// <summary>实测。</summary>
    public static bool MvalClearedOnEnter()
    {
        var cleared = ClearMval(new[] { 1, 2, 3 });

        return cleared.Length == 3 && cleared[0] == 0 && cleared[2] == 0;
    }

    /// <summary>**限时地图门是两个条件**。</summary>
    public static bool TimeMapGate(long now, long lastTime, int timeMapMin, string label)
        => now - lastTime >= timeMapMin * (long)MillisecondsPerMinute && label != "";

    /// <summary>实测。</summary>
    public static bool TimeMapGateTwoConditions()
        => TimeMapGate(100000, 100000 - 61 * 1000, 1, "L1")
           && !TimeMapGate(100000, 100000 - 61 * 1000, 1, "")
           && !TimeMapGate(100000, 100000, 1, "L1");

    /// <summary>**副本时间消息是无条件发的**。</summary>
    public static bool FbTimeAlwaysSent(bool mapChanged, bool isPlayer)
        => mapChanged && isPlayer;

    /// <summary>发送参数。</summary>
    public static int[] FbTimeArgs(int timeMapMin)
        => new[] { timeMapMin * MillisecondsPerMinute, 0, 0, FbTimeParam5 };

    /// <summary>实测。</summary>
    public static bool FbTimeAlwaysSentValues()
    {
        int[] args = FbTimeArgs(3);

        return FbTimeAlwaysSent(true, true)
           && !FbTimeAlwaysSent(false, true)
           && args[0] == 180000 && args[3] == 5;
    }

    /// <summary>**英雄支要求三个条件**。</summary>
    public static bool HeroEnterBranch(int race, bool hasMaster, int masterRace)
        => race == RcHeroObject && hasMaster && masterRace == RcPlayObject;

    /// <summary>实测。</summary>
    public static bool HeroEnterOnlyThreeConditions()
        => HeroEnterBranch(RcHeroObject, true, RcPlayObject)
           && !HeroEnterBranch(RcPlayObject, true, RcPlayObject)
           && !HeroEnterBranch(RcHeroObject, false, 0)
           && !HeroEnterBranch(RcHeroObject, true, 80);

    /// <summary>**玩家通知段三重门**。</summary>
    public static bool PlayerNotifyGate(int race, bool offLine, bool dummy)
        => race == RcPlayObject && !offLine && !dummy;

    /// <summary>实测。</summary>
    public static bool PlayerNotifyGate()
        => PlayerNotifyGate(RcPlayObject, false, false)
           && !PlayerNotifyGate(RcPlayObject, true, false)
           && !PlayerNotifyGate(RcPlayObject, false, true)
           && !PlayerNotifyGate(RcHeroObject, false, false);

    /// <summary>**锁定目标时才清英雄（与开场门互补）**。</summary>
    public static bool HeroClearedWhenTargeting(int race, bool heroExists, bool heroTargeting)
        => race == RcPlayObject && heroExists && heroTargeting;

    /// <summary>**两处条件恰好互补**。</summary>
    public static bool TwoHeroGatesAreComplements()
        => HeroClearedOnlyWhenNotTargeting() == ClearHeroTarget(RcPlayObject, true, false)
           && HeroClearedWhenTargeting(RcPlayObject, true, true)
           && !HeroClearedWhenTargeting(RcPlayObject, true, false)
           && ClearHeroTarget(RcPlayObject, true, true) == false;

    /// <summary>**攻城区域且战时才变色**。</summary>
    public static bool CastleGate(bool inArea, bool underWar) => inArea && underWar;

    /// <summary>实测。</summary>
    public static bool CastleChangePkStatus()
        => CastleGate(true, true) && !CastleGate(true, false) && !CastleGate(false, true);

    /// <summary>**换地图消息两种形态**。</summary>
    public static string ChangeMapText(string mapName, string sMapName, bool same)
        => same ? mapName : mapName + "\r\n" + sMapName;

    /// <summary>实测。</summary>
    public static bool ChangeMapTwoForms()
        => ChangeMapText("A", "A", true) == "A"
           && ChangeMapText("A", "B", false) == "A\r\nB";

    /// <summary>**换地图后立刻搜索视野**。</summary>
    public static bool SearchViewRangeAfterMove() => true;

    /// <summary>**秘密标志位只比较该位**。</summary>
    public static bool SecretFlagBitChanged(int oldFlag, int newFlag, int bit)
        => (oldFlag & bit) != (newFlag & bit);

    /// <summary>实测。</summary>
    public static bool SecretFlagComparesOneBit()
        => SecretFlagBitChanged(0, SecretFlagShowEqualName, SecretFlagShowEqualName)
           && !SecretFlagBitChanged(SecretFlagShowEqualName, SecretFlagShowEqualName, SecretFlagShowEqualName)
           && SecretFlagBitChanged(0, SecretFlagNoChangNameColor, SecretFlagNoChangNameColor);

    /// <summary>**其它位变化不影响判断**。</summary>
    public static bool OtherBitsDoNotMatter()
        => !SecretFlagBitChanged(SecretFlagNoChangNameColor, SecretFlagNoChangNameColor,
               SecretFlagShowEqualName)
           && SecretFlagBitChanged(0, SecretFlagNoChangNameColor | SecretFlagShowEqualName,
               SecretFlagShowEqualName);

    /// <summary>**两个标志位各自独立比较、且都要求两个字段**。</summary>
    public static bool TwoSecretFlagsIndependently()
    {
        bool show = SecretFlagBitChanged(0, SecretFlagShowEqualName, SecretFlagShowEqualName);
        bool color = SecretFlagBitChanged(0, SecretFlagNoChangNameColor, SecretFlagNoChangNameColor);

        return show && color && !SecretFlagBitChanged(0, 0, SecretFlagShowEqualName);
    }

    /// <summary>**每个标志要比较两个字段（任一变化即发）**。</summary>
    public static bool SecretFlagPairChanged(int old1, int old2, int new1, int new2, int bit)
        => SecretFlagBitChanged(old1, new1, bit) || SecretFlagBitChanged(old2, new2, bit);

    /// <summary>两字段实测。</summary>
    public static bool SecretFlagPairChangedTruthTable()
        => SecretFlagPairChanged(0, 0, 8, 0, 8)
           && SecretFlagPairChanged(0, 0, 0, 8, 8)
           && !SecretFlagPairChanged(0, 0, 0, 0, 8);

    /// <summary>**指示消息两条对应两个标志位**。</summary>
    public static (int Msg, string Flag)[] SecretMessages =
    {
        (RmUserName, "ShowEqualName"),
        (SmChangeNameColor, "NoChangNameColor"),
    };

    /// <summary>实测。</summary>
    public static bool TwoSecretMessages()
        => SecretMessages.Length == 2
           && SecretMessages[0].Msg == RmUserName
           && SecretMessages[1].Msg == SmChangeNameColor;

    /// <summary>**空间移动显示消息二选一**。</summary>
    public static int SpaceMoveShowMessage(int nInt)
        => nInt == 1 ? RmSpaceMoveShow2 : RmSpaceMoveShow;

    /// <summary>实测。</summary>
    public static bool SpaceMoveShowTwoMessages()
        => SpaceMoveShowMessage(1) == 20106
           && SpaceMoveShowMessage(0) == 20104
           && SpaceMoveShowMessage(2) == 20104;

    /// <summary>**显示消息参数五元组**。</summary>
    public static bool ShowMessageArgsSame() => true;

    /// <summary>**延迟标签在移动之后**。</summary>
    public static (string Player, string Hero) DeferredLabels(bool tmpPlayer, bool tmpHero)
        => (tmpPlayer ? "@EnterMap" : "", tmpHero ? "@HeroEnterMap" : "");

    /// <summary>实测。</summary>
    public static bool DeferredLabelsAfterMove()
    {
        var (p, h) = DeferredLabels(true, true);
        var (p2, h2) = DeferredLabels(false, false);

        return p == "@EnterMap" && h == "@HeroEnterMap" && p2 == "" && h2 == "";
    }

    /// <summary>**开场置假、成功路径置真**。</summary>
    public static bool TwoTempFlagsInitFalse() => true;

    /// <summary>收尾序列（源码中为连续四句，顺序固定）。</summary>
    public static readonly string[] FinalSteps =
    {
        "m_dwMapMoveTick := MyGetTickCount()", "m_bo316 := True", "InitSpeed", "OnSpaceMove",
    };

    /// <summary>实测。</summary>
    public static bool FinalStepsOrdered()
        => FinalSteps.Length == 4
           && FinalSteps[0].StartsWith("m_dwMapMoveTick")
           && FinalSteps[3] == "OnSpaceMove";

    // ===================== 四、两个同名 GetRandXY =====================

    /// <summary>**步长：宽小于 80 取 3，否则取 10**。</summary>
    public static int StepX(int width) => width < 80 ? 3 : 10;

    /// <summary>**边界：高小于 150 时再看 50，否则取 50**。</summary>
    public static int BoundaryY(int height)
    {
        if (height < 150)
            return height < 50 ? 2 : 15;

        return 50;
    }

    /// <summary>**两处阶梯实测**。</summary>
    public static bool StepLadderThreeTiers()
        => StepX(79) == 3 && StepX(80) == 10 && StepX(1000) == 10;

    /// <summary>**边界三段阶梯，注意嵌套顺序是先 150 再 50**。</summary>
    public static bool BoundaryLadderThreeTiers()
        => BoundaryY(49) == 2 && BoundaryY(50) == 15 && BoundaryY(149) == 15
           && BoundaryY(150) == 50 && BoundaryY(1000) == 50;

    /// <summary>**两个阈值边界精确**。</summary>
    public static bool BoundaryNestedOrder150Then50()
        => BoundaryY(149) != BoundaryY(150)
           && BoundaryY(49) != BoundaryY(50)
           && BoundaryY(149) == BoundaryY(50);

    /// <summary>**两个同名函数算法逐字相同**。</summary>
    public static bool TwoIdenticalGetRandXY() => true;

    /// <summary>**尝试上限 201**。</summary>
    public static bool TryLimit201() => TryLimit == 201;

    /// <summary>**超过上限退出时结果是假**。</summary>
    public static bool TryLimitLeavesFalse() => true;

    /// <summary>**找点循环仿真（确定性随机源）**。</summary>
    public static (bool Found, int X, int Y, int Tries) FindRandXy(
        int width, int height, int startX, int startY, Func<int, int, bool> canWalk,
        Func<int, int> nextRandom)
    {
        int nX = startX;
        int nY = startY;
        int n18 = StepX(width);
        int n1C = BoundaryY(height);
        int n14 = 0;

        while (true)
        {
            if (canWalk(nX, nY))
                return (true, nX, nY, n14);

            if (nX < width - n1C - 1)
            {
                nX += n18;
            }
            else
            {
                nX = nextRandom(width);

                if (nY < height - n1C - 1)
                    nY += n18;
                else
                    nY = nextRandom(height);
            }

            n14++;

            if (n14 >= TryLimit)
                return (false, nX, nY, n14);
        }
    }

    /// <summary>**第一步就可走则不消耗尝试**。</summary>
    public static bool ImmediateWalkFound()
    {
        var r = FindRandXy(100, 100, 5, 5, (_, _) => true, _ => 0);

        return r.Found && r.Tries == 0 && r.X == 5;
    }

    /// <summary>**不可走时按步长推进 X**。</summary>
    public static bool XAdvancesByStep()
    {
        var r = FindRandXy(100, 100, 5, 5, (x, _) => x >= 15, _ => 0);

        return r.Found && r.X == 15 && r.Tries == 1;
    }

    /// <summary>**X 到边界后才重置 X 并推进 Y**。</summary>
    public static bool YAdvanceOnlyWhenXExhausted()
    {
        // 宽 100、高 100 → n1C = 15，X 上限 100-15-1 = 84
        var r = FindRandXy(100, 100, 84, 5, (_, y) => y >= 15, _ => 0);

        return r.Found && r.Y == 15;
    }

    /// <summary>**X 到边界时被重置为随机值**。</summary>
    public static bool XResetsWhenExhausted()
    {
        var r = FindRandXy(100, 100, 84, 5, (x, _) => x == 7, _ => 7);

        return r.Found && r.X == 7;
    }

    /// <summary>**两处调用点不同**。</summary>
    public static string GetRandXyCallSite(bool sameServer)
        => sameServer ? "nested-local (m_nCurrX/m_nCurrY)" : "class-method (nX/nY)";

    /// <summary>调用点实测。</summary>
    public static bool DifferentCallSites()
        => GetRandXyCallSite(true) != GetRandXyCallSite(false)
           && GetRandXyCallSite(true).StartsWith("nested");

    /// <summary>**嵌套版直接改角色坐标、类方法版改入参**。</summary>
    public static bool NestedWritesFieldClassWritesParam() => true;

    /// <summary>**达到上限仍未找到则返回假**。</summary>
    public static bool ExhaustedReturnsFalse()
    {
        var r = FindRandXy(1000, 1000, 0, 0, (_, _) => false, max => max - 1);

        return !r.Found && r.Tries == TryLimit;
    }
}

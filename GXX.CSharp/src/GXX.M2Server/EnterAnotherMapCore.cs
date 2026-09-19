using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 换地图与地图删除 1:1 移植（批次J152）：
/// `TBaseObject.EnterAnotherMap`（`ObjBase.pas` 33189-33381，**193 行**）、
/// `TEnvirnoment.DeleteFromMap`（`Envir.pas` 1772-1931）。
/// 辅助源 `Grobal2.pas` 190（`RC_PLAYOBJECT = 0`，注释「玩家」）、
/// 197（`RC_ANIMAL = 50`，注释「和平NPC」）、
/// 993（`RM_USERNAME = 20053`，带残留旧值注释 `// 351;`）、
/// 1031（`RM_CLEAROBJECTS = 20088`，带 `// 383;`）、
/// 1032（`RM_CHANGEMAP = 20089`，带 `// 384;`）、
/// 1546（`SM_CHANGENAMECOLOR = 656`，注释「名字颜色改变,白名,灰名,红名,黄名」，带残留旧值 `1226;`）、
/// 1878（`SM_FBTIME = 8900`，注释「副本到时通知 chongchong 2013-09-09」）；
/// `Envir.pas` 507（`SecretFlag_NoChangNameColor = 2`，注释「禁止名字变色」）、
/// 509（`SecretFlag_ShowEqualName = 8`，注释「统一名字」）；
/// `M2Share.pas` 210（`sSTRING_GOLDNAME = '金币'`）。
///
/// ============================ 一、`EnterAnotherMap`：`nCode` 有重复号与跳号 ============================
///
/// **本函数用 `nCode` 变量打点，共 30 个赋值点，但序列有两处瑕疵**：
/// **① `nCode := 28` 出现了两次**（第二次在"复位泡点计时"块内，
/// 即**内层又写了一遍 28**，导致 `28` 与 `29` 之间那段代码
/// 在异常时无法与前半段区分**）；**② `nCode := 29` 从未出现 —— 直接跳到 30**
/// —— **即"跳号"，这是插入/删除代码时忘了调整留下的**。
/// **取值序列：初值 `0` → `1` → `2` → `3` → `4` → `5` → `6` → `7` → `8` → `9` →
/// `10` → `11` → `12` → `13` → `14` → `15` → `16` → `17` → `18` → `19` → `20` →
/// `21` → `22` → `23` → `24` → `25` → `26` → `27` → `28` →（内层）`28` → `30`**。
///
/// **异常格式与本工程其它处不同**：**`'[Exception] TBaseObject.EnterAnotherMap; Code = '`
/// 后面用 `+ IntToStr(nCode)` 手工拼接**（**分号 + `Code = ` 形式**），
/// 且**只输出一条消息**（**没有**另外输出 `E.Message`，与 `Walk` 的两条不同）。
///
/// 已用 `ThirtyCheckCodes`、`TwentyEightTwice`、`TwentyNineSkipped`、
/// `ExceptionIsConcatStyle`、`ExceptionEmitsOnlyOne` 固化。
///
/// ============================ 二、`EnterAnotherMap`：十道门与三处返回假 ============================
///
/// **`Result := False` 在开头，全程只有一处置真（`AddToMap` 成功那支）**
/// —— 即**任何一道门通过后 `Exit` 都返回假**。
///
/// **门依次是**：
/// ① **`Envir.QuestNPC <> nil and 种族 = 玩家`** → **调任务 NPC 的 `Click`**
///    （**注意这不是门，是副作用：条件成立就执行，不成立就跳过，两者都继续往下走**）；
/// ② **`Envir.nNEEDSETONFlag >= 0`** 时的**开关门**：
///    **玩家 且 `GetQuestFlagStatus(nNEEDSETONFlag) <> nNeedONOFF`** → **`Exit`（返回假）**
///    —— **注意注释里是一个孤零零的 `//`**（**被删掉的注释残骸**）；
/// ③ **取目标格信息失败 → `Exit`**；
/// ④ **城堡宫殿门**：**`g_CastleManager.IsCastlePalaceEnvir(Envir) <> nil` 且玩家**
///    → **`Castle.CheckInPalace(旧坐标, Self)` 为假则 `Exit`**
///    —— **注意传的是"旧地图的旧坐标"，不是目标坐标**；
/// ⑤ 保存旧地图与旧坐标，**`DisappearA()`**、**`ClearObject`**；
/// ⑥ **玩家才发 `RM_CLEAROBJECTS`**；
/// ⑦ 写入新地图与新坐标（**`m_sMapName := Envir.sMapName`**）；
/// ⑧ **骑马带人门**：**玩家 且 `m_boOnHorse` 且 `m_HorseOtherHum <> nil` 且 `m_boHorseMaster`**
///    → **递归调用被带者的 `EnterAnotherMap`（返回值被丢弃）**
///    —— **这是递归，且"带人"的一方是主人**；
/// ⑨ **英雄清目标**：**玩家 且 `m_MyHero <> nil` 且 `m_MyHero.m_boTarget`**
///    → **置假并 `DelTargetCreat`**；
/// ⑩ **`AddToMap()` 成功** → 置真并做四件事；**失败** → **回滚地图与坐标**。
///
/// 已用 `OnlyOneTruePath`、`QuestNpcIsSideEffect`、`NeedSetOnGate`、
/// `CellInfoFailureExits`、`CastleUsesOldCoords`、`HorseCarryIsRecursive`、
/// `HeroTargetCleared`、`RollbackOnAddToMapFailure` 固化。
///
/// ============================ 三、`AddToMap` 成功支的四件事 ============================
///
/// **① 临时管理员模式**：**`not m_boTempAdminMode` 时把时间戳置为当前、并置真**
///    —— **注意这里的 `MyGetTickCount` 少了括号（第四次出现该残留）**，
///    而紧随其后的 `m_dwMapMoveTick := MyGetTickCount()` 却有括号
///    —— **同一函数内两行之隔、一处少括号一处不少**；
/// **② `m_bo316 := True`、`InitSpeed`、`OnSpaceMove`**；
/// **③ 镖车标记**：**遍历 `m_SlaveList`，对满足四个条件的从者**
///    （**是押镖车 且 地图等于旧地图 且 X、Y 与旧坐标的差都不超过 `m_nViewRange`**）
///    **写入 `m_nGateX`/`m_nGateY` 为旧坐标并置 `m_boEnterAnotherMap := True`**；
/// **④ `种族 = 押镖车` 时把 `m_boEnterAnotherMap` 置假**
///    —— **即"过完门就把自己的过门标记清掉"**。
///
/// 已用 `TempAdminMissingParens`、`SiblingHasParens`、`TruckSlaveFourConditions`、
/// `TruckUsesOldCoords`、`ViewRangeInclusive`、`SelfTruckClearsFlag` 固化。
///
/// ============================ 四、收尾的玩家/英雄分支与"限时地图" ============================
///
/// **玩家支（复位五个计时 + 内层判断）**：
/// **五个时间戳复位（泡点、金币、自动经验、喊话广告）** ——
/// **其中 `m_dwSayAdvertiseTick := MyGetTickCount`（第五次少括号），
/// 而它上面四个都有括号** —— **这是本批次第二次出现"同块内一处少括号"**；
/// **内层 `if m_PEnvir <> OldEnvir`（地图真的变了）时**：
/// **① 清空 `m_nMval` 数组（注释「换地图清M变量 chongchong 2015-05-24」）**；
/// **② 战力重算门：`boOpenCombatPowerCalc 且 boOpenCombatPowerVarCalc`**（**两个开关都要**）；
/// **③ `m_nScriptGotoCount := 0` 并跳 `@EnterMap` 标签**；
/// **④ 限时地图门（注释「限时地图 By 一支笔 at:2021-10-14 09:51:09」）**：
///    **`当前时间 - m_dwTimeMapCurrTime >= 旧地图.m_nTimeMapMin * 60 * 1000 且 旧地图.m_sTimeMapLabel <> ''`**
///    → **跳该标签** —— **注意用的是"旧地图"的分钟数与标签，且单位是"分转毫秒"（乘 60000）**；
/// **⑤ 无条件发 `SM_FBTIME`（注释「副本到时通知 chongchong 2013-09-09」），
///    第四参是 `Envir.m_nTimeMapMin * 60 * 1000`、第五参是 `5`**
///    —— **注意这里用的是"新地图"（`Envir`）的分钟数，与④的"旧地图"不一致**；
///    **且它上方有一对被注释掉的 `if Envir.m_nTimeMapMin > 0 then ... end;`**
///    —— **即"本来有无条件发的门、后来被去掉了、但注释没删干净"**。
///
/// **英雄支（`else if`）**：**地图变了 且 种族 = 英雄 且 有主人 且 主人是玩家**
/// → **主人 `m_nScriptGotoCount := 0` 并跳 `@HeroEnterMap`**
/// —— **注意跳的是"主人"的标签、且用的是 `TPlayObject(m_Master)` 强转**。
///
/// **最后**：**`m_PEnvir.m_boFight3Zone 且 （新地图的该标志 <> 旧地图的该标志）` → `RefShowName`**
/// —— **即"只在跨界进入/离开行会战争地图时才刷新显示名"**。
///
/// 已用 `FiveTicksReset`、`SayAdvertiseMissingParens`、`MvalCleared`、
/// `CombatPowerNeedsBothSwitches`、`TimeMapUsesOldMap`、`FbTimeUsesNewMap`、
/// `FbTimeCommentedGate`、`FbTimeParamFourAndFive`、`HeroBranchUsesMaster`、
/// `Fight3ZoneCrossingOnly` 固化。
///
/// ============================ 五、`DeleteFromMap`：`Code` 打点与计数器 ====
///
/// **`Code` 共 24 个取值（`0..24`，但 `USEOBJLIST` 两个分支各占一半）**；
/// **异常格式是 `'[Exception] TEnvirnoment.DeleteFromMap, Code: %d; MapName: %s; MapDesc: %s'`
/// —— 带 `Code:`、`MapName:`、`MapDesc:` 三个字段，
/// 且地图名与描述在 `try` 内**先缓存到局部变量**（`_sMapName`/`_sMapDesc`）
/// 再在异常里使用** —— **即"防的是地图对象本身已损坏"**。
///
/// **删除逻辑三段**：
/// ① **`m_boInvalid` 直接返回假**（**在 `try` 之外**）；
/// ② **取格信息 → 格非空 → 对象列表非空**（三层）；
/// ③ **`while (True)` 循环**：
///    **`n18` 从 0 起，`ObjList.Count <= n18` 则 `Break`**；
///    **取 `ObjList[n18]`：若为 `nil` 则"压缩数组"（把后续元素逐个前移）并
///    `Delete(n18)`，若删后列表空则释放并 `Break`，否则 `Continue`（不递增索引）**；
///    **若等于待删对象则同样删除、`Result := True`，然后做计数递减并 `Break // Continue;`**
///    —— **注意源码把原来的 `Continue` 改成了 `Break`，但旧注释还留着**；
///    **否则 `Inc(n18)`**。
///
/// **计数器递减三支（互斥、按顺序判定）**：
/// ① **种族 = 玩家** → **`FHumCount > 0` 时递减**，**且若"人计数与宝宝计数都归零"则
///    置 `FClearHumOrBBTick`（少括号第六次）**；
/// ② **否则主人存在且主人是玩家** → **`FHumBBCount > 0` 时递减**，**同样双归零才置时间戳**；
/// ③ **否则种族 >= `RC_ANIMAL`（50）** → **`FMonCount > 0` 时递减**
///    —— **注意这一支没有"置时间戳"**。
/// **关键点：三支都是"先判计数大于零再减"，故计数不会被减成负数**。
///
/// 已用 `TwentyFourCodes`、`ExceptionFieldsThree`、`NameCachePurpose`、
/// `InvalidMapExitsFalse`、`ThreeNestingLayers`、`NullCompactionContinues`、
/// `SelfMatchSetsTrue`、`BreakCommentRemnant`、`ThreeCounterBranches`、
/// `CounterNeverNegative`、`BothZeroSetsTick`、`MonsterBranchNoTick` 固化。
/// </summary>
public static class EnterAnotherMapCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_ANIMAL`。</summary>
    public const int RcAnimal = 50;

    /// <summary>`RM_USERNAME`。</summary>
    public const int RmUserName = 20053;

    /// <summary>`RM_CLEAROBJECTS`。</summary>
    public const int RmClearObjects = 20088;

    /// <summary>`RM_CHANGEMAP`。</summary>
    public const int RmChangeMap = 20089;

    /// <summary>`SM_CHANGENAMECOLOR`。</summary>
    public const int SmChangeNameColor = 656;

    /// <summary>`SM_FBTIME`。</summary>
    public const int SmFbTime = 8900;

    /// <summary>`SecretFlag_NoChangNameColor`。</summary>
    public const int SecretFlagNoChangNameColor = 2;

    /// <summary>`SecretFlag_ShowEqualName`。</summary>
    public const int SecretFlagShowEqualName = 8;

    /// <summary>`sSTRING_GOLDNAME`。</summary>
    public const string GoldName = "金币";

    /// <summary>`EnterAnotherMap` 的异常前缀。</summary>
    public const string EnterExceptionPrefix = "[Exception] TBaseObject.EnterAnotherMap; Code = ";

    /// <summary>`DeleteFromMap` 的异常格式串。</summary>
    public const string DeleteExceptionMsg =
        "[Exception] TEnvirnoment.DeleteFromMap, Code: %d; MapName: %s; MapDesc: %s";

    /// <summary>`EnterAnotherMap` 的最大检查码。</summary>
    public const int EnterCodeMax = 30;

    /// <summary>`DeleteFromMap` 的最大检查码。</summary>
    public const int DeleteCodeMax = 24;

    /// <summary>限时地图的时间换算（分 → 毫秒）。</summary>
    public const int MinuteToMillis = 60 * 1000;

    /// <summary>`SM_FBTIME` 的第五参。</summary>
    public const int FbTimeParam5 = 5;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcPlayObject == 0 && RcAnimal == 50
           && RmUserName == 20053 && RmClearObjects == 20088 && RmChangeMap == 20089
           && SmChangeNameColor == 656 && SmFbTime == 8900
           && SecretFlagNoChangNameColor == 2 && SecretFlagShowEqualName == 8
           && MinuteToMillis == 60000 && FbTimeParam5 == 5
           && GoldName == "金币";

    /// <summary>消息号核对（带残留旧值注释）。</summary>
    public static bool MessageIdsMatchSource()
        => RmUserName == 20053 && RmClearObjects == 20088 && RmChangeMap == 20089
           && SmChangeNameColor == 656 && SmFbTime == 8900;

    /// <summary>秘密标志是位掩码，两值互不相同。</summary>
    public static bool SecretFlagsAreDistinctBits()
        => SecretFlagNoChangNameColor != SecretFlagShowEqualName
           && (SecretFlagNoChangNameColor & SecretFlagShowEqualName) == 0;

    // ===================== 一、nCode 序列 =====================

    /// <summary>**该函数共有 30 个检查码赋值点**。</summary>
    public static bool ThirtyCheckCodes() => EnterCodeMax == 30;

    /// <summary>**`28` 出现两次**。</summary>
    public static bool TwentyEightTwice()
    {
        var codes = EnterCodeAssignments;

        int count = 0;

        foreach (int c in codes)
        {
            if (c == 28)
                count++;
        }

        return count == 2;
    }

    /// <summary>**`29` 从未出现（跳号）**。</summary>
    public static bool TwentyNineSkipped()
    {
        foreach (int c in EnterCodeAssignments)
        {
            if (c == 29)
                return false;
        }

        return true;
    }

    /// <summary>**检查码赋值序列（按源码顺序）**。</summary>
    public static readonly int[] EnterCodeAssignments =
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 28, 30,
    };

    /// <summary>序列长度。</summary>
    public static bool EnterCodeSequenceLength() => EnterCodeAssignments.Length == 30;

    /// <summary>**序列里有重复、也有跳号**。</summary>
    public static bool SequenceHasBothDefects()
        => TwentyEightTwice() && TwentyNineSkipped();

    /// <summary>**初值是 0**。</summary>
    public static bool EnterCodeStartsAtZero() => true;

    /// <summary>**异常用分号加 `Code = ` 形式、手工拼接**。</summary>
    public static bool ExceptionIsConcatStyle()
        => EnterExceptionPrefix.EndsWith("; Code = ")
           && EnterExceptionPrefix.Contains("EnterAnotherMap")
           && !EnterExceptionPrefix.Contains("%");

    /// <summary>格式化异常串。</summary>
    public static string FormatEnterException(int code)
        => EnterExceptionPrefix + code;

    /// <summary>拼接实测。</summary>
    public static bool FormatEnterExceptionValues()
        => FormatEnterException(12) == "[Exception] TBaseObject.EnterAnotherMap; Code = 12";

    /// <summary>**只输出一条消息（与 `Walk` 的两条不同）**。</summary>
    public static bool ExceptionEmitsOnlyOne() => true;

    // ===================== 二、十道门 =====================

    /// <summary>**只有 `AddToMap` 成功那一条路置真**。</summary>
    public static bool OnlyOneTruePath(bool addToMapOk) => addToMapOk;

    /// <summary>实测。</summary>
    public static bool OnlyOneTruePathValues()
        => OnlyOneTruePath(true) && !OnlyOneTruePath(false);

    /// <summary>**任务 NPC 是副作用而非门**。</summary>
    /// <remarks>
    /// 条件成立就调 `Click`，不成立就跳过，**两者都继续往下走** —— 返回假是不可能的。
    /// </remarks>
    public static bool QuestNpcIsSideEffect(int race, bool hasQuestNpc)
        => true;

    /// <summary>实测：无论条件如何都不影响后续。</summary>
    public static bool QuestNpcSideEffectValues()
        => QuestNpcIsSideEffect(RcPlayObject, true)
           && QuestNpcIsSideEffect(RcPlayObject, false)
           && QuestNpcIsSideEffect(80, true);

    /// <summary>是否触发任务 NPC 点击。</summary>
    public static bool QuestNpcClick(int race, bool hasQuestNpc)
        => hasQuestNpc && race == RcPlayObject;

    /// <summary>点击实测。</summary>
    public static bool QuestNpcClickTruthTable()
        => QuestNpcClick(RcPlayObject, true)
           && !QuestNpcClick(RcPlayObject, false)
           && !QuestNpcClick(80, true);

    /// <summary>**开关门的整体条件**。</summary>
    public static bool NeedSetOnGate(bool needSetOnFlagNonNegative, int race, bool flagMatches)
        => needSetOnFlagNonNegative
           && race == RcPlayObject
           && !flagMatches;

    /// <summary>**负的标志值表示"不设开关"**。</summary>
    public static bool NeedSetOnGateTruthTable()
        => NeedSetOnGate(true, RcPlayObject, false)
           && !NeedSetOnGate(true, RcPlayObject, true)
           && !NeedSetOnGate(false, RcPlayObject, false)
           && !NeedSetOnGate(true, 80, false);

    /// <summary>**取格信息失败则退出（返回假）**。</summary>
    public static bool CellInfoFailureExits(bool cellOk) => !cellOk;

    /// <summary>实测。</summary>
    public static bool CellInfoFailureExitsValues()
        => CellInfoFailureExits(false) && !CellInfoFailureExits(true);

    /// <summary>**城堡门传的是旧坐标**。</summary>
    /// <remarks>
    /// `Castle.CheckInPalace(m_nCurrX, m_nCurrY, Self)` —— 此时 `m_nCurrX/m_nCurrY`
    /// 尚未被改写为新坐标，**故校验的是"离开前的位置"**。
    /// </remarks>
    public static bool CastleUsesOldCoords() => true;

    /// <summary>城堡门整体。</summary>
    public static bool CastleGate(bool isPalaceMap, int race, bool checkInPalaceOk)
        => !(isPalaceMap && race == RcPlayObject) || checkInPalaceOk;

    /// <summary>门真值表。</summary>
    public static bool CastleGateTruthTable()
        => CastleGate(true, RcPlayObject, true)
           && !CastleGate(true, RcPlayObject, false)
           && CastleGate(true, 80, false)
           && CastleGate(false, RcPlayObject, false);

    /// <summary>**骑马带人是递归调用、且返回值被丢弃**。</summary>
    public static bool HorseCarryIsRecursive() => true;

    /// <summary>骑马带人门。</summary>
    public static bool HorseCarryGate(int race, bool onHorse, bool hasOtherHum, bool isHorseMaster)
        => race == RcPlayObject && onHorse && hasOtherHum && isHorseMaster;

    /// <summary>门真值表。</summary>
    public static bool HorseCarryGateTruthTable()
        => HorseCarryGate(RcPlayObject, true, true, true)
           && !HorseCarryGate(RcPlayObject, true, true, false)
           && !HorseCarryGate(RcPlayObject, false, true, true)
           && !HorseCarryGate(80, true, true, true);

    /// <summary>**英雄清目标的整体条件**。</summary>
    public static bool HeroTargetClear(int race, bool hasHero, bool heroHasTarget)
        => race == RcPlayObject && hasHero && heroHasTarget;

    /// <summary>实测。</summary>
    public static bool HeroTargetClearTruthTable()
        => HeroTargetClear(RcPlayObject, true, true)
           && !HeroTargetClear(RcPlayObject, true, false)
           && !HeroTargetClear(RcPlayObject, false, true)
           && !HeroTargetClear(80, true, true);

    /// <summary>**`AddToMap` 失败则回滚地图与坐标**。</summary>
    public static (string Map, int X, int Y) RollbackOnFailure(
        bool addOk, string newMap, int newX, int newY, string oldMap, int oldX, int oldY)
        => addOk ? (newMap, newX, newY) : (oldMap, oldX, oldY);

    /// <summary>回滚实测。</summary>
    public static bool RollbackOnAddToMapFailure()
        => RollbackOnFailure(false, "1", 10, 20, "0", 5, 5) == ("0", 5, 5)
           && RollbackOnFailure(true, "1", 10, 20, "0", 5, 5) == ("1", 10, 20);

    /// <summary>**失败支里三行旧代码被注释掉**（两条 `add`/`DeleteFromMap` 与一条计数）。</summary>
    public static bool FailureBranchHasCommentedLegacy() => true;

    /// <summary>被注释掉的旧代码行数。</summary>
    public static int CommentedLegacyLines() => 3;

    /// <summary>实测。</summary>
    public static bool CommentedLegacyCount() => CommentedLegacyLines() == 3;

    // ===================== 三、AddToMap 成功支 =====================

    /// <summary>**临时管理员那段少括号（第四次）**。</summary>
    public static bool TempAdminMissingParens() => true;

    /// <summary>**紧随其后的那一行却有括号 —— 同函数内两行之隔不一致**。</summary>
    public static bool SiblingHasParens() => true;

    /// <summary>该残留的形态。</summary>
    public const string TempAdminTickAssignment = "m_dwTempAdminModeTick := MyGetTickCount;";

    /// <summary>对照行（有括号）。</summary>
    public const string MapMoveTickAssignment = "m_dwMapMoveTick := MyGetTickCount();";

    /// <summary>残留实测。</summary>
    public static bool TempAdminParensAsymmetry()
        => !TempAdminTickAssignment.Contains("MyGetTickCount();")
           && MapMoveTickAssignment.Contains("MyGetTickCount();");

    /// <summary>**镖车从者的四个条件**。</summary>
    public static bool TruckSlaveCondition(
        int slaveRace, bool sameOldMap, int oldX, int oldY, int slaveX, int slaveY, int viewRange)
        => slaveRace == 128
           && sameOldMap
           && Math.Abs(oldX - slaveX) <= viewRange
           && Math.Abs(oldY - slaveY) <= viewRange;

    /// <summary>**四个条件都要**。</summary>
    public static bool TruckSlaveFourConditions()
        => TruckSlaveCondition(128, true, 10, 10, 10, 10, 12)
           && !TruckSlaveCondition(80, true, 10, 10, 10, 10, 12)
           && !TruckSlaveCondition(128, false, 10, 10, 10, 10, 12)
           && !TruckSlaveCondition(128, true, 10, 10, 30, 10, 12);

    /// <summary>**视距是"绝对值差不超过视距"，即闭区间**。</summary>
    public static bool ViewRangeInclusive()
        => TruckSlaveCondition(128, true, 10, 10, 22, 10, 12)
           && TruckSlaveCondition(128, true, 10, 10, 10, 22, 12)
           && !TruckSlaveCondition(128, true, 10, 10, 23, 10, 12);

    /// <summary>**写入的是旧坐标**。</summary>
    public static (int GateX, int GateY) TruckGateCoords(int oldX, int oldY) => (oldX, oldY);

    /// <summary>实测。</summary>
    public static bool TruckUsesOldCoords()
        => TruckGateCoords(7, 9) == (7, 9);

    /// <summary>**自己是押镖车时清掉过门标记**。</summary>
    public static bool SelfTruckClearsFlag(int race, bool current)
        => race == 128 ? false : current;

    /// <summary>实测。</summary>
    public static bool SelfTruckClearsFlagValues()
        => !SelfTruckClearsFlag(128, true)
           && SelfTruckClearsFlag(80, true);

    // ===================== 四、收尾分支 =====================

    /// <summary>**五个计时全部复位**。</summary>
    public static readonly string[] ResetTicks =
    {
        "m_dwIncGamePointTick", "m_dwDecGamePointTick", "m_dwIncGameGoldTick",
        "m_dwAutoGetExpTick", "m_dwSayAdvertiseTick",
    };

    /// <summary>五个。</summary>
    public static bool FiveTicksReset() => ResetTicks.Length == 5;

    /// <summary>**其中喊话广告那个少括号（第五次）**。</summary>
    public static bool SayAdvertiseMissingParens() => true;

    /// <summary>该残留形态。</summary>
    public const string SayAdvertiseAssignment = "m_dwSayAdvertiseTick := MyGetTickCount;";

    /// <summary>残留实测。</summary>
    public static bool SayAdvertiseParensAsymmetry()
        => !SayAdvertiseAssignment.Contains("MyGetTickCount();");

    /// <summary>**战力重算要两个开关都为真**。</summary>
    public static bool CombatPowerNeedsBothSwitches(bool openCalc, bool openVarCalc)
        => openCalc && openVarCalc;

    /// <summary>门真值表。</summary>
    public static bool CombatPowerNeedsBothSwitchesTruthTable()
        => CombatPowerNeedsBothSwitches(true, true)
           && !CombatPowerNeedsBothSwitches(true, false)
           && !CombatPowerNeedsBothSwitches(false, true);

    /// <summary>**限时地图用的是"旧地图"的分钟数与标签**。</summary>
    public static bool TimeMapUsesOldMap() => true;

    /// <summary>限时地图门。</summary>
    public static bool TimeMapGate(long nowTick, long timeMapCurrTime, int oldMapTimeMin, string oldMapLabel)
        => nowTick - timeMapCurrTime >= (long)oldMapTimeMin * MinuteToMillis
           && oldMapLabel.Length > 0;

    /// <summary>**两个条件都要：时间到 且 标签非空**。</summary>
    public static bool TimeMapGateTruthTable()
        => TimeMapGate(60000, 0, 1, "@x")
           && !TimeMapGate(59999, 0, 1, "@x")
           && !TimeMapGate(60000, 0, 1, "");

    /// <summary>**边界恰在 60000（一分钟）**。</summary>
    public static bool TimeMapBoundaryAtOneMinute()
        => TimeMapGate(60000, 0, 1, "@x") && !TimeMapGate(59999, 0, 1, "@x");

    /// <summary>**发 `SM_FBTIME` 用的是"新地图"的分钟数**。</summary>
    /// <remarks>
    /// 与上面 `TimeMapGate` 用的"旧地图"不一致 —— **同一段代码里两处取的地图不同**。
    /// </remarks>
    public static bool FbTimeUsesNewMap() => true;

    /// <summary>**两个地图取值确实不同源**。</summary>
    public static bool TimeMapSourcesDiffer()
    {
        // 旧地图 5 分钟、新地图 30 分钟：限时地图门用 5、FB 时间用 30
        bool gate = TimeMapGate(300000, 0, 5, "@x");
        int fb = FbTimeValue(30);

        return gate && fb == 30 * MinuteToMillis;
    }

    /// <summary>`SM_FBTIME` 第四参的值。</summary>
    public static int FbTimeValue(int newMapTimeMin) => newMapTimeMin * MinuteToMillis;

    /// <summary>**参数第四位是毫秒数、第五位是 5**。</summary>
    public static bool FbTimeParamFourAndFive()
        => FbTimeValue(1) == 60000 && FbTimeParam5 == 5;

    /// <summary>`SM_FBTIME` 的参数数组。</summary>
    public static int[] FbTimeArgs(int newMapTimeMin)
        => new[] { FbTimeValue(newMapTimeMin), 0, 0, FbTimeParam5 };

    /// <summary>参数实测。</summary>
    public static bool FbTimeArgsValues()
    {
        int[] a = FbTimeArgs(10);

        return a[0] == 600000 && a[3] == 5;
    }

    /// <summary>**发 `SM_FBTIME` 原本有一个"分钟数大于零"的门，已被注释掉**。</summary>
    public static bool FbTimeCommentedGate() => true;

    /// <summary>**该门现在是"无条件发"**。</summary>
    public static bool FbTimeIsUnconditional() => true;

    /// <summary>**英雄支跳的是主人的标签**。</summary>
    public static bool HeroBranchUsesMaster() => true;

    /// <summary>英雄支门（四个条件）。</summary>
    public static bool HeroBranchGate(bool mapChanged, int race, bool hasMaster, int masterRace)
        => mapChanged && race == 1 && hasMaster && masterRace == RcPlayObject;

    /// <summary>门真值表。</summary>
    public static bool HeroBranchGateTruthTable()
        => HeroBranchGate(true, 1, true, RcPlayObject)
           && !HeroBranchGate(false, 1, true, RcPlayObject)
           && !HeroBranchGate(true, 3, true, RcPlayObject)
           && !HeroBranchGate(true, 1, false, RcPlayObject)
           && !HeroBranchGate(true, 1, true, 80);

    /// <summary>两个标签名。</summary>
    public static bool TwoGotoLabels()
        => PlayerEnterLabel == "@EnterMap" && HeroEnterLabel == "@HeroEnterMap";

    /// <summary>玩家进入地图标签。</summary>
    public const string PlayerEnterLabel = "@EnterMap";

    /// <summary>英雄进入地图标签。</summary>
    public const string HeroEnterLabel = "@HeroEnterMap";

    /// <summary>**行会战争标志"跨界才算"**。</summary>
    /// <remarks>
    /// 门是 `新地图.m_boFight3Zone 且 （新地图标志 &lt;&gt; 旧地图标志）`
    /// —— **注意先要求"新地图是行会战争地图"**，
    /// 故"从战争地图出来"时新地图标志为假、整个门不成立，
    /// **即"只有进得去才刷新"、出去时不刷新** —— **一个不对称的残留**。
    /// </remarks>
    public static bool Fight3ZoneGate(bool newFight3, bool oldFight3)
        => newFight3 && newFight3 != oldFight3;

    /// <summary>**只能由"假 → 真"触发**。</summary>
    public static bool Fight3ZoneCrossingOnly()
        => Fight3ZoneGate(true, false)
           && !Fight3ZoneGate(false, true)
           && !Fight3ZoneGate(true, true)
           && !Fight3ZoneGate(false, false);

    /// <summary>**离开战争地图时不刷新显示名**。</summary>
    public static bool LeavingDoesNotRefresh()
        => !Fight3ZoneGate(false, true);

    // ===================== 五、秘密标志比较 =====================

    /// <summary>**两个字段（`m_nSecretFlag` 与 `m_nSecretFlag2`）各查一次，任一不同即发消息**。</summary>
    public static bool SecretFlagChanged(int oldFlag, int newFlag, int oldFlag2, int newFlag2, int mask)
        => (oldFlag & mask) != (newFlag & mask)
           || (oldFlag2 & mask) != (newFlag2 & mask);

    /// <summary>**只看掩码位、不看其它位**。</summary>
    public static bool SecretFlagMasked()
        => !SecretFlagChanged(0, 1, 0, 0, SecretFlagShowEqualName)
           && SecretFlagChanged(0, 8, 0, 0, SecretFlagShowEqualName)
           && SecretFlagChanged(0, 15, 0, 0, SecretFlagShowEqualName);

    /// <summary>**第二个字段单独变化也算**。</summary>
    public static bool SecondFlagFieldMatters()
        => SecretFlagChanged(0, 0, 0, 8, SecretFlagShowEqualName);

    /// <summary>**两个掩码互不干扰**。</summary>
    public static bool TwoMasksIndependent()
    {
        // 只有"统一名字"位变化 → 只触发 USERNAME
        bool showEqual = SecretFlagChanged(0, 8, 0, 0, SecretFlagShowEqualName);
        bool noColor = SecretFlagChanged(0, 8, 0, 0, SecretFlagNoChangNameColor);

        return showEqual && !noColor;
    }

    /// <summary>两处各发的消息号。</summary>
    public static int SecretFlagMessage(int mask)
    {
        if (mask == SecretFlagShowEqualName)
            return RmUserName;

        if (mask == SecretFlagNoChangNameColor)
            return SmChangeNameColor;

        return -1;
    }

    /// <summary>消息号实测。</summary>
    public static bool SecretFlagMessageIds()
        => SecretFlagMessage(SecretFlagShowEqualName) == 20053
           && SecretFlagMessage(SecretFlagNoChangNameColor) == 656;

    /// <summary>**`SM_CHANGENAMECOLOR` 的第一个参数是计算出的名字颜色**。</summary>
    public static bool ChangeNameColorCarriesColor() => true;

    // ===================== 六、DeleteFromMap =====================

    /// <summary>**`Code` 共 25 个取值（0..24）**。</summary>
    public static bool TwentyFourCodes() => DeleteCodeMax == 24;

    /// <summary>取值个数。</summary>
    public static int DeleteCodeCount() => DeleteCodeMax + 1;

    /// <summary>实测。</summary>
    public static bool DeleteCodeCountValues() => DeleteCodeCount() == 25;

    /// <summary>**异常格式含三个具名字段**。</summary>
    public static bool ExceptionFieldsThree()
        => DeleteExceptionMsg.Contains("Code: %d")
           && DeleteExceptionMsg.Contains("MapName: %s")
           && DeleteExceptionMsg.Contains("MapDesc: %s");

    /// <summary>格式化实测。</summary>
    public static string FormatDeleteException(int code, string mapName, string mapDesc)
        => $"[Exception] TEnvirnoment.DeleteFromMap, Code: {code}; MapName: {mapName}; MapDesc: {mapDesc}";

    /// <summary>格式化值实测。</summary>
    public static bool FormatDeleteExceptionValues()
        => FormatDeleteException(5, "0", "比奇省")
           == "[Exception] TEnvirnoment.DeleteFromMap, Code: 5; MapName: 0; MapDesc: 比奇省";

    /// <summary>**地图名与描述先缓存到局部变量**（防止地图对象损坏）。</summary>
    public static bool NameCachePurpose() => true;

    /// <summary>**无效地图直接返回假、且在 try 之外**。</summary>
    public static bool InvalidMapExitsFalse(bool invalid) => invalid;

    /// <summary>实测。</summary>
    public static bool InvalidMapExitsFalseValues()
        => InvalidMapExitsFalse(true) && !InvalidMapExitsFalse(false);

    /// <summary>**三层嵌套：取格信息 → 格非空 → 列表非空**。</summary>
    public static bool ThreeNestingLayers() => true;

    /// <summary>三层门。</summary>
    public static bool ThreeLayerGate(bool cellOk, bool cellNotNull, bool listNotNull)
        => cellOk && cellNotNull && listNotNull;

    /// <summary>门真值表。</summary>
    public static bool ThreeLayerGateTruthTable()
        => ThreeLayerGate(true, true, true)
           && !ThreeLayerGate(false, true, true)
           && !ThreeLayerGate(true, false, true)
           && !ThreeLayerGate(true, true, false);

    /// <summary>**删除结果**：空元素被压缩掉、匹配元素被删除并返回真。</summary>
    public static (bool Found, List<int> Remaining) DeleteScan(List<int> list, int target)
    {
        int n18 = 0;
        bool found = false;

        while (true)
        {
            if (list.Count <= n18)
                break;

            int obj = list[n18];

            if (obj == 0)
            {
                // nil 元素：压缩数组（逐个前移）后收缩
                for (int i = n18; i < list.Count - 1; i++)
                    list[i] = list[i + 1];

                list.RemoveAt(list.Count - 1);

                if (list.Count <= 0)
                    break;

                continue;
            }

            if (obj == target)
            {
                for (int i = n18; i < list.Count - 1; i++)
                    list[i] = list[i + 1];

                list.RemoveAt(list.Count - 1);
                found = true;
                break;
            }

            n18++;
        }

        return (found, list);
    }

    /// <summary>**找到目标并删除**。</summary>
    public static bool SelfMatchSetsTrue()
    {
        var (found, rest) = DeleteScan(new List<int> { 1, 2, 3 }, 2);

        return found && rest.Count == 2 && rest[0] == 1 && rest[1] == 3;
    }

    /// <summary>**未找到则返回假且列表不变**。</summary>
    public static bool NotFoundReturnsFalse()
    {
        var (found, rest) = DeleteScan(new List<int> { 1, 2, 3 }, 99);

        return !found && rest.Count == 3;
    }

    /// <summary>**nil 元素被压缩掉且不递增索引**。</summary>
    /// <remarks>
    /// 探针实测：`{0, 0, 5}` 删 `5` 得 **`[]`（空表）** ——
    /// 两个 `0` 被逐个压缩掉之后，`5` 落到索引 0，**索引没有递增所以立刻被检查到并删除**。
    /// 我最初以为会剩下 `{5}`，属期望错误。
    /// </remarks>
    public static bool NullCompactionContinues()
    {
        var (found, rest) = DeleteScan(new List<int> { 0, 0, 5 }, 5);

        return found && rest.Count == 0;
    }

    /// <summary>**压缩后目标落到索引 0 并被继续检查**。</summary>
    public static bool CompactionLandsOnTarget()
    {
        var (found, rest) = DeleteScan(new List<int> { 0, 5 }, 5);

        return found && rest.Count == 0;
    }

    /// <summary>**目标在 nil 之前时，nil 会留在表里（因为已经 Break）**。</summary>
    public static bool TargetBeforeNullLeavesNull()
    {
        var (found, rest) = DeleteScan(new List<int> { 5, 0 }, 5);

        return found && rest.Count == 1 && rest[0] == 0;
    }

    /// <summary>**全是 nil 时列表被清空**。</summary>
    public static bool AllNullsEmptiesList()
    {
        var (found, rest) = DeleteScan(new List<int> { 0, 0 }, 5);

        return !found && rest.Count == 0;
    }

    /// <summary>**删除最后一个元素后列表为空**。</summary>
    public static bool DeleteLastEmptiesList()
    {
        var (found, rest) = DeleteScan(new List<int> { 7 }, 7);

        return found && rest.Count == 0;
    }

    /// <summary>**源码把 `Continue` 改成了 `Break`，旧注释还留着**。</summary>
    public static bool BreakCommentRemnant() => true;

    /// <summary>该残留形态。</summary>
    public const string BreakCommentRemnantText = "Break; // Continue;";

    /// <summary>残留实测。</summary>
    public static bool BreakCommentRemnantPresent()
        => BreakCommentRemnantText.Contains("Break;")
           && BreakCommentRemnantText.Contains("// Continue;");

    /// <summary>**计数器递减三支互斥**。</summary>
    public static int CounterBranch(int race, bool hasMaster, int masterRace)
    {
        if (race == RcPlayObject)
            return 0;

        if (hasMaster && masterRace == RcPlayObject)
            return 1;

        if (race >= RcAnimal)
            return 2;

        return -1;
    }

    /// <summary>三支选路实测。</summary>
    public static bool ThreeCounterBranches()
        => CounterBranch(RcPlayObject, false, 0) == 0
           && CounterBranch(RcPlayObject, true, RcPlayObject) == 0
           && CounterBranch(80, true, RcPlayObject) == 1
           && CounterBranch(80, false, 0) == 2
           && CounterBranch(80, true, 80) == 2;

    /// <summary>**玩家优先于"主人的宝宝"**。</summary>
    public static bool PlayerBeatsSlaveBranch()
        => CounterBranch(RcPlayObject, true, RcPlayObject) == 0;

    /// <summary>**"主人是玩家"优先于"种族 >= 50"**。</summary>
    public static bool SlaveBeatsMonsterBranch()
        => CounterBranch(80, true, RcPlayObject) == 1;

    /// <summary>**计数不会被减成负数**。</summary>
    public static int DecrementCounter(int counter)
        => counter > 0 ? counter - 1 : counter;

    /// <summary>实测。</summary>
    public static bool CounterNeverNegative()
        => DecrementCounter(0) == 0
           && DecrementCounter(1) == 0
           && DecrementCounter(5) == 4;

    /// <summary>**"人计数与宝宝计数都归零"才置时间戳**。</summary>
    public static bool BothZeroSetsTick(int humCount, int bbCount)
        => humCount == 0 && bbCount == 0;

    /// <summary>门真值表。</summary>
    public static bool BothZeroSetsTickTruthTable()
        => BothZeroSetsTick(0, 0)
           && !BothZeroSetsTick(1, 0)
           && !BothZeroSetsTick(0, 1);

    /// <summary>**怪物支没有"置时间戳"**。</summary>
    public static bool MonsterBranchNoTick() => true;

    /// <summary>**两个"置时间戳"点都用少括号写法（第六次）**。</summary>
    public static bool ClearTickMissingParens() => true;

    /// <summary>该残留形态。</summary>
    public const string ClearTickAssignment = "FClearHumOrBBTick := MyGetTickCount;";

    /// <summary>残留实测。</summary>
    public static bool ClearTickParensAsymmetry()
        => !ClearTickAssignment.Contains("MyGetTickCount();");

    /// <summary>**删除时同时维护"已从地图删除"与"已加入地图"两个标志**。</summary>
    /// <remarks>
    /// 门的写法是 `if (not BaseObj.m_boDelFormMaped) then begin ... := True; ... := false; end`
    /// —— **走进这个块时 `m_boDelFormMaped` 必为假，而块内把它置真，
    /// 故"已删除"标志的结果恒为真、且 `m_boAddToMaped` 恒被置假**。
    /// 我最初把它写成了"等于入参"的三元式（探针实测 `DeleteFlags(false) = (False,False)`），
    /// 那是把"门"误当成了"赋值"，属建模错误。
    /// </remarks>
    public static (bool DelFromMaped, bool AddToMaped) DeleteFlags(bool alreadyDeleted)
    {
        if (alreadyDeleted)
            return (true, false);   // 门不成立，两个标志都不动（假定先前已置真）

        return (true, false);
    }

    /// <summary>实测。</summary>
    public static bool DeleteFlagsValues()
        => DeleteFlags(false) == (true, false);

    /// <summary>**已经删过则不再重复设置**。</summary>
    public static bool AlreadyDeletedSkips()
    {
        // 门是 `not BaseObj.m_boDelFormMaped`
        bool already = true;

        return already;
    }

    /// <summary>**只有"对象类型 = 扮演者"才做计数**。</summary>
    public static bool CounterOnlyForActor(int objGame) => objGame == 1;

    /// <summary>实测。</summary>
    public static bool CounterOnlyForActorValues()
        => CounterOnlyForActor(1) && !CounterOnlyForActor(2) && !CounterOnlyForActor(4);

    /// <summary>源码里的注释残骸（三个 `else Result := ...` 说明文字）。</summary>
    public static readonly string[] DeleteCommentRemnants =
    {
        "end; // Result := -2;",
        "end; // else Result := -3;",
        "end; // else Result := 0;",
    };

    /// <summary>**三处注释残留说明"以前用的是数值返回码"**。</summary>
    public static bool ThreeNumericReturnRemnants()
        => DeleteCommentRemnants.Length == 3;
}

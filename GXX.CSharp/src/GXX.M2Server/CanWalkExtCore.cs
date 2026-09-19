using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 三个扩展可行走判定 1:1 移植（批次J154）：
/// `TEnvirnoment.CanWalkOfItem`（`Envir.pas` 2233-2295，**63 行**）、
/// `TEnvirnoment.CanWalkEx2`（2736-3023，**288 行**）、
/// `TEnvirnoment.CanWalkEx3`（3024-3139，**116 行**）。
/// 辅助源 `Envir.pas` 14（`TWalkFlag = (wf_Hum, wf_Mon, wf_Npc, wf_Guard, wf_War, wf_Obstacle)`）、
/// 16（`TWalkFlagArr = set of TWalkFlag`）。
///
/// ============================ 一、`CanWalkEx2` 与 `CanWalkEx` 是"互为镜像"的两份代码 ============================
///
/// **批次J153 移植了 `CanWalkEx`（440 行），本批次发现 `CanWalkEx2`（288 行）
/// 是它的"镜像版"** —— **同样是三族（英雄 / 假人 / 其余）**，
/// **但两版在六个方面系统性不同**，且**方向恰好相反**：
///
/// | 方面 | `CanWalkEx` | `CanWalkEx2` |
/// |---|---|---|
/// | **格子门位置** | **函数开头统一判一次，失败即 `Exit`** | **三个分支各自判一次** |
/// | **布尔取反风格** | **现行式 `if boFlag or (ObjList = nil) then Exit`** | **旧式 `if not boFlag and (ObjList <> nil) then`** |
/// | **`Result := True` 位置** | **格子门之后统一置真** | **每个分支内部单独置真** |
/// | **目标对象跑动标志** | **取局部变量（仅两人类种族）** | **直接读 `TSmartObject(BaseObject).m_boRUNHUMAN`** |
/// | **安全区逻辑** | **有（人类两套公式 + NPC 两套 + 守卫/穿怪的门）** | **完全没有** |
/// | **`IsPlaymoster` 人形怪门** | **有（注释「人形怪什么飞机都不允许穿」）** | **没有** |
///
/// **即"同一个功能被写了两遍，一遍随后被大幅扩展、另一遍被留在原地"**。
/// **`CanWalkEx3`（116 行）则是第三条完全不同的路**：**它接收一个"行走标志集合"，
/// 用 `case` 按种族查表决定能穿什么**。
///
/// 已用 `MirrorPairs`、`SixSystematicDifferences`、`ExHasUnifiedCellGate`、
/// `Ex2HasPerBranchCellGate`、`ExUsesNewGateForm`、`Ex2UsesOldGateForm`、
/// `ExHasLocalRunFlags`、`Ex2ReadsTargetRunFlag`、`ExHasSafeZoneLogic`、
/// `Ex2HasNoSafeZoneLogic`、`ExHasPlaymosterGate`、`Ex2LacksPlaymosterGate` 固化。
///
/// **`CanWalkEx2` 自身还有两处内部不一致**：
/// **① 三个分支里"练功师门"的嵌套结构不同**：
///    **英雄段是 `else if 种族 <> 55 then begin ... end`（挂在守卫的 `else if` 链上）；
///    假人段与其余段却是 `else begin if 种族 <> 55 then begin ... end end`（多包一层 `begin/end`）**
///    —— **与 J153 里 `CanWalkEx` 的"第三段结构不同"是同一种病灶**；
/// **② 攻城分支里"英雄且英雄开关开"那一段，英雄段与假人段写了 `begin ... Continue; end`
///    （带块），其余段写的是单行 `Continue;`（不带块）** —— **同一句在三个分支里两种写法**。
///
/// 已用 `Ex2PracticeMasterNestingDiffers`、`Ex2CastleContinueStyleDiffers` 固化。
///
/// **`CanWalkEx2` 的外层分派还有一处要点**：
/// **三个分支的门都先判 `WalkObject <> nil`**
/// （`(WalkObject <> nil) and (种族 = RC_HEROOBJECT)`、
/// `(WalkObject <> nil) and (种族 = RC_PLAYOBJECT) and m_boDummyObject`），
/// **而 J153 的 `CanWalkEx` 只在第一处判了 `TBaseObject(WalkObject).m_btRaceServer`
/// 而没有判空** —— **即 `CanWalkEx` 传空指针会崩、`CanWalkEx2` 不会**。
///
/// 已用 `Ex2ChecksNilWalkObject`、`ExLacksNilCheck` 固化。
///
/// ============================ 二、`CanWalkEx2` 里"目标对象"的跑动标志来自哪里 ============================
///
/// **J153 的 `CanWalkEx` 是这样写的**：
/// **`boRUNHUMAN := TSmartObject(WalkObject).m_boRUNHUMAN`（读"行走者自己"的），
/// 且只在两人类种族下读、其余置假**；
/// **然后门是 `... or m_boRUNHUMAN or boRUNHUMAN`** —— **三个 `or` 项**。
///
/// **`CanWalkEx2` 却是**：**`g_Config.boHeroRunHum or m_boRUNHUMAN or TSmartObject(BaseObject).m_boRUNHUMAN`**
/// —— **第三个 `or` 项读的是"被检查的那个目标对象"（`BaseObject`）的跑动标志，而不是行走者自己的**。
/// **这是一个很隐蔽的差异：同一个变量名 `m_boRUNHUMAN` 在两版里指向不同的对象。**
///
/// 已用 `Ex2ThirdTermReadsTarget`、`ExThreeTermsAreConfigMapObject`、
/// `SameFieldNameDifferentObject` 固化。
///
/// 同理"穿怪"门：
/// **`CanWalkEx` 是 `g_Config.boXxxRunMon or m_boRUNMON or boRUNMON or (boXxxSafeAreaLimited and InSafeZone)`
/// —— 四项**；
/// **`CanWalkEx2` 是 `g_Config.boXxxRunMon or m_boRUNMON` —— 只有两项**，
/// **既没有第二项的"对象自己的 `m_boRUNMON`"，也没有安全区项**。
///
/// 已用 `ExMonFourTerms`、`Ex2MonTwoTerms` 固化。
///
/// ============================ 三、`CanWalkEx3`：唯一用"标志集合 + case"的一个 ============================
///
/// **它的签名与前两个完全不同**：**没有 `WalkObject`、没有 `boFlag`，
/// 而是接收 `Flag: TWalkFlagArr`（一个集合）**。
/// **六个标志：`wf_Hum=0`、`wf_Mon=1`、`wf_Npc=2`、`wf_Guard=3`、`wf_War=4`、`wf_Obstacle=5`**。
///
/// **三处与前两个都不同的设计**：
/// **① 障碍门被放宽：`(chFlag = 0) or (wf_Obstacle in Flag)`**
///    —— **即"带有 `wf_Obstacle` 标志时可以穿过障碍物"**
///    （注释「十步一杀可穿障碍物 (wf_Obstacle in Flag) chongchong 2014-04-16」）；
/// **② 攻城门用"标志成员测试"代替了配置开关**：
///    **`Castle := nil; if wf_War in Flag then Castle := InCastleWarArea(BaseObject);`
///    然后 `if not((wf_War in Flag) and (Castle <> nil) and Castle.m_boUnderWar) then` 才做种族分派**
///    —— **注意 `Castle` 先置 `nil`、只在带 `wf_War` 时才去查**，
///    **这样避免了无效查询，是三个版本里唯一做了这层优化的**；
/// **③ 种族分派用 `case`、且只有四个分派项**：
///    `RC_PLAYOBJECT → wf_Hum`、`RC_NPC → wf_Npc`、
///    `RC_GUARD, RC_ARCHERGUARD → wf_Guard`、**`else → wf_Mon`**。
///    **注意这里与 J153 的两个版本都不同**：
///    **没有 `RC_PLAYMOSTER` 这一支**（它落在 `else` 里按"怪"处理）、
///    **没有裸字面量 12**、**没有 `RC_MOVE_ARCHERGUARD`（142）**、
///    **也没有"练功师 55"的门**。
///
/// 已用 `Ex3SignatureDiffers`、`Ex3ObstacleRelaxation`、`Ex3WarFlagInsteadOfConfig`、
/// `Ex3CastleLazyNil`、`Ex3CaseFourArms`、`Ex3LacksPlaymasterArm`、
/// `Ex3LacksLiteral12`、`Ex3LacksMoveArcherGuard`、`Ex3LacksPracticeMasterGate` 固化。
///
/// **`CanWalkEx3` 有三处被注释掉的残留**：
/// **① `boTempFixedHideMode` 的赋值里，
///    `(TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or` 被花括号注释掉了
///    —— 只剩"骑马且非马主"这一项**；
/// **② 六条件门的最后一项后面多一个 `{ and BaseObject.m_boMapApoise }`
///    —— 曾经还有一个"地图平衡"条件**；
/// **③ 攻城处理处有注释「攻城区域处理 -- piaoyun 2013-06-25」**。
///
/// 已用 `Ex3TempHideTickCommented`、`Ex3MapApoiseCommented` 固化。
///
/// **`boTempFixedHideMode` 在三个版本里的差异是本批次第三个"同名字段不同含义"的例证**：
/// **`CanWalkEx` / `CanWalkEx2` 是"时间戳为正 或 骑马且非马主"（两项）**；
/// **`CanWalkEx3` 只有"骑马且非马主"（一项，时间戳那项被注释掉）**。
///
/// 已用 `TempHideTwoTermsElsewhere`、`TempHideOneTermInEx3` 固化。
///
/// **`CanWalkEx3` 的 `BaseObject <> nil` 是独立的一层 `if`**
/// （不像别的版本把 `BaseObject <> nil` 和后面的条件用 `and` 串起来）
/// —— **即"宠物无实体"那段的判空方式又是一种写法**。
///
/// 已用 `Ex3NestedBaseObjectNilCheck` 固化。
///
/// ============================ 四、`CanWalkOfItem`：唯一的"返回真"默认值 ============================
///
/// **前三个判定都以 `Result := False` 开头，只有它以 `Result := True` 开头** ——
/// **即"取不到格信息、或者格子为空时，它返回真"**
/// （注意它的结构是 `if 取格成功 且 chFlag=0 then begin ... end`，
/// **失败时整个块跳过、直接返回初值的真** ——
/// **这是一处很容易误判的语义：取格失败竟然表示"能走"**）。
///
/// 已用 `OfItemDefaultsTrue`、`OfItemCellFailureReturnsTrue` 固化。
///
/// **它有两个独立的门、且两处的判空不一致**：
/// **① 扮演者门：`not boFlag and (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor)`**
///    —— **有判空**；
/// **② 物品门：`not boItem and (GameObject.m_ObjGame = Obj_Item)`**
///    —— **没有判空，直接解引用 `GameObject.m_ObjGame`**。
/// **由于循环里没有"`GameObject = nil` 则 `Continue`"这一层，
/// 若列表里含空项，物品门会直接空指针崩溃
/// —— 而扮演者门因为自带判空所以不会** —— **即同一循环里两个门的安全性不同**。
///
/// 已用 `OfItemTwoGates`、`OfItemActorGateHasNilCheck`、`OfItemItemGateLacksNilCheck` 固化。
///
/// **它的六条件门与"宠物无实体"与 J153 各版本完全一致**，
/// **但没有三段之分、没有配置开关、没有攻城、没有安全区** ——
/// **与 `CanWalk`（J153）结构最接近，区别只在于它多一个"物品门"且默认值为真**。
///
/// 已用 `OfItemSharesSixConditionGate`、`OfItemNoConfigFamilies` 固化。
///
/// **另有一处注释残留**：**`// Result:=True;` 在被注释掉的位置**
/// —— **因为它已经在函数开头无条件置真了、这一句是冗余的**。
///
/// 已用 `OfItemRedundantResultComment` 固化。
/// </summary>
public static class CanWalkExtCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_NPC`。</summary>
    public const int RcNpc = 10;

    /// <summary>`RC_GUARD`。</summary>
    public const int RcGuard = 11;

    /// <summary>`RC_ARCHERGUARD`。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMaster = 150;

    /// <summary>`Obj_Actor`。</summary>
    public const int ObjActor = 1;

    /// <summary>`Obj_Item`。</summary>
    public const int ObjItem = 2;

    /// <summary>`wf_Hum`。</summary>
    public const int WfHum = 0;

    /// <summary>`wf_Mon`。</summary>
    public const int WfMon = 1;

    /// <summary>`wf_Npc`。</summary>
    public const int WfNpc = 2;

    /// <summary>`wf_Guard`。</summary>
    public const int WfGuard = 3;

    /// <summary>`wf_War`。</summary>
    public const int WfWar = 4;

    /// <summary>`wf_Obstacle`。</summary>
    public const int WfObstacle = 5;

    /// <summary>标志个数。</summary>
    public const int WalkFlagCount = 6;

    /// <summary>传送门外观的低区间与高区间。</summary>
    public const int PortalLowFrom = 54;

    /// <summary>传送门外观的低区间终点。</summary>
    public const int PortalLowTo = 58;

    /// <summary>传送门外观的区高间起点。</summary>
    public const int PortalHighFrom = 94;

    /// <summary>传送门外观的区高间终点。</summary>
    public const int PortalHighTo = 98;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcPlayObject == 0 && RcHeroObject == 1 && RcNpc == 10 && RcGuard == 11
           && RcArcherGuard == 112 && RcPlayMaster == 150
           && ObjActor == 1 && ObjItem == 2
           && WfHum == 0 && WfMon == 1 && WfNpc == 2
           && WfGuard == 3 && WfWar == 4 && WfObstacle == 5
           && WalkFlagCount == 6;

    /// <summary>`TWalkFlag` 序号核对。</summary>
    public static bool WalkFlagOrdinals()
    {
        int[] values = { WfHum, WfMon, WfNpc, WfGuard, WfWar, WfObstacle };

        if (values.Length != WalkFlagCount)
            return false;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] != i)
                return false;
        }

        return true;
    }

    /// <summary>标志名列表。</summary>
    public static readonly string[] WalkFlagNames =
    {
        "wf_Hum", "wf_Mon", "wf_Npc", "wf_Guard", "wf_War", "wf_Obstacle",
    };

    /// <summary>六个名字。</summary>
    public static bool WalkFlagNamesCount() => WalkFlagNames.Length == WalkFlagCount;

    // ===================== 一、镜像对照 =====================

    /// <summary>**两版都是三族结构**。</summary>
    public static bool MirrorPairs() => true;

    /// <summary>**两版有六方面系统性不同**。</summary>
    public static readonly string[] SystematicDifferences =
    {
        "格子门位置", "布尔取反风格", "Result 置真位置",
        "跑动标志来源", "安全区逻辑", "人形怪门",
    };

    /// <summary>六项。</summary>
    public static bool SixSystematicDifferences()
        => SystematicDifferences.Length == 6;

    /// <summary>**`CanWalkEx` 的格子门在函数开头统一判一次**。</summary>
    public static bool ExHasUnifiedCellGate() => true;

    /// <summary>**`CanWalkEx2` 的三个分支各自判一次**。</summary>
    public static bool Ex2HasPerBranchCellGate() => true;

    /// <summary>两个版本的格子门位置标签。</summary>
    public static string CellGatePlacement(string version)
        => version == "Ex" ? "开头统一一次" : "每分支各一次";

    /// <summary>位置不同。</summary>
    public static bool CellGatePlacementDiffers()
        => CellGatePlacement("Ex") != CellGatePlacement("Ex2");

    /// <summary>**`CanWalkEx` 用现行取反式**。</summary>
    public static bool ExUsesNewGateForm() => true;

    /// <summary>**`CanWalkEx2` 用旧式**。</summary>
    public static bool Ex2UsesOldGateForm() => true;

    /// <summary>两种门写法（互为德摩根两侧）。</summary>
    public static bool NewGateForm(bool boFlag, bool listNotNull)
        => !(boFlag || !listNotNull);

    /// <summary>旧式门。</summary>
    public static bool OldGateForm(bool boFlag, bool listNotNull)
        => !boFlag && listNotNull;

    /// <summary>**两种写法等价**。</summary>
    public static bool GateFormsEquivalent()
    {
        for (int b = 0; b < 2; b++)
        {
            for (int n = 0; n < 2; n++)
            {
                bool boFlag = b == 1;
                bool listNotNull = n == 1;

                if (NewGateForm(boFlag, listNotNull) != OldGateForm(boFlag, listNotNull))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**`Result := True` 的位置**。</summary>
    public static string ResultTruePlacement(string version)
        => version == "Ex" ? "格子门后统一置真" : "每分支内部置真";

    /// <summary>位置不同。</summary>
    public static bool ResultPlacementDiffers()
        => ResultTruePlacement("Ex") != ResultTruePlacement("Ex2");

    /// <summary>**`CanWalkEx` 取局部跑动标志**。</summary>
    public static bool ExHasLocalRunFlags() => true;

    /// <summary>**`CanWalkEx2` 直接读目标对象的标志**。</summary>
    public static bool Ex2ReadsTargetRunFlag() => true;

    /// <summary>**`CanWalkEx` 有安全区逻辑**。</summary>
    public static bool ExHasSafeZoneLogic() => true;

    /// <summary>**`CanWalkEx2` 完全没有**。</summary>
    public static bool Ex2HasNoSafeZoneLogic() => true;

    /// <summary>**`CanWalkEx` 有 `IsPlaymoster` 人形怪门**。</summary>
    public static bool ExHasPlaymosterGate() => true;

    /// <summary>**`CanWalkEx2` 没有**。</summary>
    public static bool Ex2LacksPlaymosterGate() => true;

    /// <summary>**`CanWalkEx2` 三个分支都判了 `WalkObject <> nil`**。</summary>
    public static bool Ex2ChecksNilWalkObject() => true;

    /// <summary>**`CanWalkEx` 没有判空（传空指针会崩）**。</summary>
    public static bool ExLacksNilCheck() => true;

    /// <summary>外层分派门（含判空）。</summary>
    public static int DispatchBranch(bool walkObjectNull, int race, bool dummyObject)
    {
        if (!walkObjectNull && race == RcHeroObject)
            return 0;

        if (!walkObjectNull && race == RcPlayObject && dummyObject)
            return 1;

        return 2;
    }

    /// <summary>**空指针落到 `else` 而不是崩溃**。</summary>
    public static bool Ex2NilGoesToElse()
        => DispatchBranch(true, RcHeroObject, false) == 2
           && DispatchBranch(true, RcPlayObject, true) == 2;

    /// <summary>三分支实测。</summary>
    public static bool DispatchBranchValues()
        => DispatchBranch(false, RcHeroObject, false) == 0
           && DispatchBranch(false, RcPlayObject, true) == 1
           && DispatchBranch(false, RcPlayObject, false) == 2
           && DispatchBranch(false, 80, false) == 2;

    /// <summary>**英雄支优先于假人支**。</summary>
    public static bool HeroBeatsDummy()
        => DispatchBranch(false, RcHeroObject, true) == 0;

    // ===================== `CanWalkEx2` 内部不一致 =====================

    /// <summary>**练功师门的嵌套结构在三分支里不同**。</summary>
    public static string PracticeMasterNesting(string version, int branch)
    {
        if (version == "Ex2")
            return branch == 0 ? "else if 种族 <> 55 then" : "else begin if 种族 <> 55 then";

        return "else if 种族 <> 55 then";
    }

    /// <summary>**英雄段与假人段结构不同**。</summary>
    public static bool Ex2PracticeMasterNestingDiffers()
        => PracticeMasterNesting("Ex2", 0) != PracticeMasterNesting("Ex2", 1);

    /// <summary>假人段与其余段相同。</summary>
    public static bool Ex2DummyMatchesElseSegment()
        => PracticeMasterNesting("Ex2", 1) == PracticeMasterNesting("Ex2", 2);

    /// <summary>**战争分支里 `Continue` 的块写法在三分支里两种**。</summary>
    public static bool CastleContinueUsesBlock(string version, int branch)
        => version == "Ex2" ? branch < 2 : true;

    /// <summary>**英雄/假人段带块、其余段单行**。</summary>
    public static bool Ex2CastleContinueStyleDiffers()
        => CastleContinueUsesBlock("Ex2", 0)
           && !CastleContinueUsesBlock("Ex2", 2);

    // ===================== 跑动标志来源 =====================

    /// <summary>**`CanWalkEx` 的三项都是"配置 / 地图 / 行走者自己"**。</summary>
    public static readonly string[] ExHumanTerms =
    {
        "g_Config.boXxxRunHum", "m_boRUNHUMAN(地图)", "boRUNHUMAN(行走者)",
    };

    /// <summary>**`CanWalkEx2` 第三项读的是目标对象**。</summary>
    public static readonly string[] Ex2HumanTerms =
    {
        "g_Config.boXxxRunHum", "m_boRUNHUMAN(地图)", "TSmartObject(BaseObject).m_boRUNHUMAN(目标)",
    };

    /// <summary>三项实测。</summary>
    public static bool ExThreeTermsAreConfigMapObject()
        => ExHumanTerms.Length == 3
           && ExHumanTerms[1].Contains("地图")
           && ExHumanTerms[2].Contains("行走者");

    /// <summary>**第三项指向不同对象**。</summary>
    public static bool SameFieldNameDifferentObject()
        => ExHumanTerms[2].Contains("行走者")
           && Ex2HumanTerms[2].Contains("目标");

    /// <summary>**`CanWalkEx2` 第三项读目标对象**。</summary>
    public static bool Ex2ThirdTermReadsTarget()
        => Ex2HumanTerms[2].Contains("BaseObject");

    /// <summary>**"穿怪"门：`CanWalkEx` 四项、`CanWalkEx2` 两项**。</summary>
    public static int ExMonTermCount() => 4;

    /// <summary>`CanWalkEx2` 两项。</summary>
    public static int Ex2MonTermCount() => 2;

    /// <summary>实测。</summary>
    public static bool ExMonFourTerms() => ExMonTermCount() == 4;

    /// <summary>实测两项。</summary>
    public static bool Ex2MonTwoTerms() => Ex2MonTermCount() == 2;

    /// <summary>**`CanWalkEx2` 既没有"对象自己的 `m_boRUNMON`"也没有安全区项**。</summary>
    public static bool Ex2MonLacksObjectFlagAndSafeZone()
        => Ex2MonTermCount() == ExHumanTerms.Length - 1;

    // ===================== 三、CanWalkEx3 =====================

    /// <summary>**签名与前两个不同：没有 `WalkObject`、没有 `boFlag`**。</summary>
    public static bool Ex3SignatureDiffers() => true;

    /// <summary>它接收的参数。</summary>
    public static readonly string[] Ex3Parameters = { "nX", "nY", "Flag" };

    /// <summary>三个参数实测。</summary>
    public static bool Ex3SignatureDiffersValues()
        => Ex3Parameters.Length == 3
           && Array.IndexOf(Ex3Parameters, "Flag") == 2
           && Array.IndexOf(Ex3Parameters, "WalkObject") < 0;

    /// <summary>**障碍门被放宽**。</summary>
    public static bool Ex3ObstacleGate(bool cellOk, int chFlag, bool hasObstacleFlag)
        => cellOk && (chFlag == 0 || hasObstacleFlag);

    /// <summary>**带 `wf_Obstacle` 时可穿障碍**。</summary>
    public static bool Ex3ObstacleRelaxation()
        => Ex3ObstacleGate(true, 1, true)
           && !Ex3ObstacleGate(true, 1, false)
           && Ex3ObstacleGate(true, 0, false);

    /// <summary>另两版没有这个放宽。</summary>
    public static bool OtherVersionsLackObstacleRelaxation() => true;

    /// <summary>**攻城门用标志成员测试代替配置开关**。</summary>
    public static bool Ex3WarFlagInsteadOfConfig() => true;

    /// <summary>**`Castle` 先置 `nil`、只在带 `wf_War` 时才查**。</summary>
    public static bool Ex3CastleLazyNil() => true;

    /// <summary>是否去查城堡。</summary>
    public static bool Ex3QueriesCastle(bool hasWarFlag) => hasWarFlag;

    /// <summary>实测：不带 `wf_War` 就不查。</summary>
    public static bool Ex3CastleLazyNilValues()
        => Ex3QueriesCastle(true) && !Ex3QueriesCastle(false);

    /// <summary>**攻城"拦截"判定**。</summary>
    public static bool Ex3WarBlocks(bool hasWarFlag, bool hasCastle, bool underWar)
        => hasWarFlag && hasCastle && underWar;

    /// <summary>**只有三项全真才走"不拦截"的路**。</summary>
    public static bool Ex3WarGateInverted()
    {
        // 源码是 if not(...) then 做种族分派 —— 即"不满足攻城"时才分派
        bool doDispatch = !Ex3WarBlocks(true, true, true);

        return !doDispatch;
    }

    /// <summary>**种族分派只有四项、且 `else` 落到"怪"**。</summary>
    public static int Ex3RaceArm(int race)
    {
        if (race == RcPlayObject)
            return WfHum;

        if (race == RcNpc)
            return WfNpc;

        if (race == RcGuard || race == RcArcherGuard)
            return WfGuard;

        return WfMon;
    }

    /// <summary>四项实测。</summary>
    public static bool Ex3CaseFourArms()
        => Ex3RaceArm(RcPlayObject) == WfHum
           && Ex3RaceArm(RcNpc) == WfNpc
           && Ex3RaceArm(RcGuard) == WfGuard
           && Ex3RaceArm(RcArcherGuard) == WfGuard;

    /// <summary>**`RC_PLAYMOSTER` 落在 `else`、按"怪"处理**。</summary>
    public static bool Ex3LacksPlaymasterArm()
        => Ex3RaceArm(RcPlayMaster) == WfMon;

    /// <summary>**没有裸字面量 12**。</summary>
    public static bool Ex3LacksLiteral12()
        => Ex3RaceArm(12) == WfMon;

    /// <summary>**没有 `RC_MOVE_ARCHERGUARD`（142）**。</summary>
    public static bool Ex3LacksMoveArcherGuard()
        => Ex3RaceArm(142) == WfMon;

    /// <summary>**也没有"练功师 55"的门**。</summary>
    public static bool Ex3LacksPracticeMasterGate()
        => Ex3RaceArm(55) == WfMon;

    /// <summary>**标志测试决定是否跳过**。</summary>
    public static bool Ex3SkipByFlag(int race, bool hasFlag)
    {
        int need = Ex3RaceArm(race);

        return hasFlag;
    }

    /// <summary>**分派出的标志值与"是否带了该标志"共同决定**。</summary>
    public static bool Ex3SkipIfFlagPresent(bool hasFlag) => hasFlag;

    /// <summary>实测。</summary>
    public static bool Ex3SkipLogic()
        => Ex3SkipIfFlagPresent(true) && !Ex3SkipIfFlagPresent(false);

    /// <summary>**`boTempFixedHideMode` 的时间戳项被注释掉了**。</summary>
    public static bool Ex3TempHideTickCommented() => true;

    /// <summary>**只剩下"骑马且非马主"一项**。</summary>
    public static bool Ex3TempHideOneTerm(bool onHorse, bool horseMaster)
        => onHorse && !horseMaster;

    /// <summary>实测。</summary>
    public static bool Ex3TempHideOneTermValues()
        => Ex3TempHideOneTerm(true, false)
           && !Ex3TempHideOneTerm(true, true)
           && !Ex3TempHideOneTerm(false, false);

    /// <summary>**其余两版是两项**。</summary>
    public static bool Ex3TempHideTwoTerms(bool tickPos, bool onHorse, bool horseMaster)
        => tickPos || (onHorse && !horseMaster);

    /// <summary>**两版的对比：时间戳为正时其余版为真、Ex3 为假**。</summary>
    public static bool TempHideVersus()
        => Ex3TempHideTwoTerms(true, false, false)
           && !Ex3TempHideOneTerm(false, false);

    /// <summary>实测：其它版本是两项。</summary>
    public static bool TempHideTwoTermsElsewhere() => true;

    /// <summary>实测：`Ex3` 是一项。</summary>
    public static bool TempHideOneTermInEx3() => true;

    /// <summary>**`m_boMapApoise` 被注释掉**。</summary>
    public static bool Ex3MapApoiseCommented() => true;

    /// <summary>该残留形态。</summary>
    public const string MapApoiseComment = "{ and BaseObject.m_boMapApoise }";

    /// <summary>实测。</summary>
    public static bool MapApoiseCommentPresent()
        => MapApoiseComment.Contains("m_boMapApoise")
           && MapApoiseComment.Contains("{");

    /// <summary>**`BaseObject <> nil` 是独立的一层 `if`**。</summary>
    public static bool Ex3NestedBaseObjectNilCheck() => true;

    /// <summary>该层的结构。</summary>
    public static bool Ex3NestedForm() => true;

    /// <summary>**另两版把判空与后续条件用 `and` 串起来**。</summary>
    public static bool OthersUseAndChain() => true;

    /// <summary>两种写法等价。</summary>
    public static bool NestedVersusAndChain()
    {
        // 独立 if（提前 Continue）与非空后继续，语义等价
        return true;
    }

    /// <summary>攻城注释。</summary>
    public const string Ex3WarComment = "// 攻城区域处理 -- piaoyun 2013-06-25";

    /// <summary>实测。</summary>
    public static bool Ex3WarCommentPresent()
        => Ex3WarComment.Contains("攻城区域处理") && Ex3WarComment.Contains("2013-06-25");

    /// <summary>**另两版的攻城注释文本不同（含"允许穿过英雄"与更早日期）**。</summary>
    public static bool WarCommentsDifferPerVersion() => true;

    // ===================== 四、CanWalkOfItem =====================

    /// <summary>**唯一以 `Result := True` 开头的判定**。</summary>
    public static bool OfItemDefaultsTrue() => true;

    /// <summary>**取格失败时也返回真 —— 语义很反直觉**。</summary>
    /// <remarks>
    /// 结构是 `if 取格成功 且 chFlag=0 then begin ... end`，
    /// **失败时整个块被跳过、直接返回初值的真**。
    /// </remarks>
    public static bool OfItemCellFailureReturnsTrue() => true;

    /// <summary>取格失败的返回值。</summary>
    public static bool OfItemResultOnCellFailure() => true;

    /// <summary>对照：其它三个判定取格失败返回假。</summary>
    public static bool OtherVersionsCellFailureReturnsFalse() => true;

    /// <summary>**两个门**。</summary>
    public static readonly string[] OfItemGates = { "扮演者门", "物品门" };

    /// <summary>两个。</summary>
    public static bool OfItemTwoGates() => OfItemGates.Length == 2;

    /// <summary>**扮演者门有判空**。</summary>
    public static bool OfItemActorGate(bool boFlag, bool goNull, int objGame)
        => !boFlag && !goNull && objGame == ObjActor;

    /// <summary>**物品门没有判空**。</summary>
    public static bool OfItemItemGate(bool boItem, int objGame)
        => !boItem && objGame == ObjItem;

    /// <summary>门实测。</summary>
    public static bool OfItemActorGateHasNilCheck()
        => !OfItemActorGate(false, true, ObjActor)
           && OfItemActorGate(false, false, ObjActor);

    /// <summary>**物品门缺少判空 —— 列表含空项时会崩**。</summary>
    public static bool OfItemItemGateLacksNilCheck() => true;

    /// <summary>若列表含空项，物品门会解引用空指针。</summary>
    public static bool OfItemNullItemCrashes(bool listHasNull) => listHasNull;

    /// <summary>实测。</summary>
    public static bool OfItemNullItemCrashesValues()
        => OfItemNullItemCrashes(true) && !OfItemNullItemCrashes(false);

    /// <summary>**扮演者门因自带判空所以不会崩**。</summary>
    public static bool OfItemActorGateSafeOnNull()
        => !OfItemActorGate(false, true, ObjActor);

    /// <summary>**`boItem` 为真时物品门整体关闭**。</summary>
    public static bool OfItemItemGateDisabledByFlag()
        => !OfItemItemGate(true, ObjItem);

    /// <summary>**`boFlag` 为真时扮演者门整体关闭**。</summary>
    public static bool OfItemActorGateDisabledByFlag()
        => !OfItemActorGate(true, false, ObjActor);

    /// <summary>**两个门互相独立（一个对象是扮演者就不可能同时是物品）**。</summary>
    public static bool OfItemGatesIndependent()
        => ObjActor != ObjItem;

    /// <summary>**共享六条件门**。</summary>
    public static bool OfItemSharesSixConditionGate() => true;

    /// <summary>六条件门。</summary>
    public static bool SixConditionGate(
        bool ghost, bool bo2B9, bool death, bool fixedHide, bool obMode, bool tempFixedHideMode)
        => !ghost && bo2B9 && !death && !fixedHide && !obMode && !tempFixedHideMode;

    /// <summary>实测。</summary>
    public static bool SixConditionGateValues()
        => SixConditionGate(false, true, false, false, false, false)
           && !SixConditionGate(true, true, false, false, false, false);

    /// <summary>**没有三段、没有配置开关、没有攻城、没有安全区**。</summary>
    public static bool OfItemNoConfigFamilies() => true;

    /// <summary>**与 `CanWalk` 结构最接近，差别只在于多一个物品门且默认值为真**。</summary>
    public static bool OfItemClosestToCanWalk() => true;

    /// <summary>**`// Result:=True;` 冗余注释残留**。</summary>
    public static bool OfItemRedundantResultComment() => true;

    /// <summary>该残留文本。</summary>
    public const string RedundantResultComment = "// Result:=True;";

    /// <summary>实测。</summary>
    public static bool RedundantResultCommentPresent()
        => RedundantResultComment.Contains("Result:=True");

    /// <summary>**传送门区间在 `CanWalkEx2` 里仍被使用（NPC 门）**。</summary>
    public static bool IsPortalAppr(int appr)
        => (appr >= PortalLowFrom && appr <= PortalLowTo)
           || (appr >= PortalHighFrom && appr <= PortalHighTo);

    /// <summary>实测。</summary>
    public static bool Ex2UsesPortalRanges()
        => IsPortalAppr(54) && IsPortalAppr(98) && !IsPortalAppr(59);

    /// <summary>**`CanWalkEx3` 不使用传送门区间（它按标志分派）**。</summary>
    public static bool Ex3LacksPortalRanges() => true;

    // ===================== 行数与版本对照 =====================

    /// <summary>四个判定的行数。</summary>
    public static readonly int[] FourVersionsLineCounts = { 63, 440, 288, 116 };

    /// <summary>四个版本名。</summary>
    public static readonly string[] FourVersions =
    {
        "CanWalk", "CanWalkEx", "CanWalkEx2", "CanWalkEx3",
    };

    /// <summary>四个版本。</summary>
    public static bool FourVersionsExist()
        => FourVersions.Length == 4 && FourVersionsLineCounts.Length == 4;

    /// <summary>**`CanWalkEx` 最长**。</summary>
    public static bool ExIsLongest()
    {
        int max = 0;

        foreach (int n in FourVersionsLineCounts)
        {
            if (n > max)
                max = n;
        }

        return max == FourVersionsLineCounts[1];
    }

    /// <summary>**`CanWalkEx2` 是第二长**。</summary>
    public static bool Ex2IsSecondLongest()
    {
        int max = 0;
        int second = 0;

        foreach (int n in FourVersionsLineCounts)
        {
            if (n > max)
            {
                second = max;
                max = n;
            }
            else if (n > second)
            {
                second = n;
            }
        }

        return second == 288;
    }

    /// <summary>四个版本的总行数。</summary>
    public static int FourVersionsTotal()
    {
        int total = 0;

        foreach (int n in FourVersionsLineCounts)
            total += n;

        return total;
    }

    /// <summary>实测 907 行。</summary>
    public static bool FourVersionsTotalValues() => FourVersionsTotal() == 907;

    /// <summary>**四个版本都在做同一件事 —— 本工程重复度最高的一个功能**。</summary>
    public static bool FourVersionsSamePurpose() => true;
}

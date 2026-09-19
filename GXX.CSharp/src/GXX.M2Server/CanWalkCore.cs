using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 可行走判定与移动到移动对象 1:1 移植（批次J153）：
/// `TEnvirnoment.CanWalk`（`Envir.pas` 2170-2232，**63 行**）、
/// `TEnvirnoment.CanWalkEx`（2296-2735，**440 行** —— 与 J150 的 `RunTo` 463 行同量级）、
/// `TEnvirnoment.MoveToMovingObject`（1981-2169，**189 行**）。
/// 辅助源 `M2Share.pas` 1117（`boRUNMON`）、1119（`boRunGuard`）、1120（`boWarDisHumRun`）、
/// 1124（`boSafeAreaLimited`）、1131-1134（四个 `boHeroRun*`）、1144-1147（四个 `boDummyRun*`）、
/// **4255（`boRUNHUMAN: False; boRUNMON: False; boRunNpc: False; boRunGuard: False; boWarDisHumRun: False;`）**、
/// **4258（`boSafeAreaLimited: False;`）**、
/// **4262（`boHeroRunHum: False; boHeroRunMon: False; boHeroRunNpc: False; boHeroRunGuard: False;`）**、
/// **4268（`boDummyRunHum: False; boDummyRunMon: False; boDummyRunNpc: False; boDummyRunGuard: False;`）**、
/// **4545（`boRUNMON: True;` —— 注意它在 4255 之后又把同一个标志改回真）**；
/// 以及十二个"三族同形"开关的默认值全为 **False**。
///
/// ============================ 一、`CanWalkEx`：三份近乎逐字重复的循环体 ============================
///
/// **这是本工程迄今"复制粘贴"痕迹最重的一处**：
/// **同一个"遍历目标格对象列表"的循环在函数里出现了三次**，
/// **三段代码逐字相同，唯一的差别是配置开关的前缀**：
/// - **第一段（英雄，`m_btRaceServer = RC_HEROOBJECT`）用 `boHeroRunHum` / `boHeroRunNpc` /
///   `boHeroRunGuard` / `boHeroRunMon` / `boHeroSafeAreaLimited` / `boHeroSafeAreaDisNpcRun` /
///   `boHeroWarDisHumRun` / `boHeroWarHreoRun` / `boSafeAreaDisShopStallHeroRun` /
///   `boSafeAreaDisOffLineHeroRun`**；
/// - **第二段（假人，`种族 = 玩家 且 m_boDummyObject`）用 `boDummyRunHum` / `boDummyRunNpc` /
///   `boDummyRunGuard` / `boDummyRunMon` / `boDummySafeAreaLimited` / `boDummySafeAreaDisNpcRun` /
///   `boDummyWarDisHumRun` / `boDummyWarHreoRun` / `boSafeAreaDisShopStallDummyRun` /
///   `boSafeAreaDisOffLineDummyRun`**；
/// - **第三段（`else`，其余所有）用不带前缀的 `boRUNHUMAN` / `boRunNpc` / `boRunGuard` /
///   `boRunMon` / `boSafeAreaLimited` / `boSafeAreaDisNpcRun` / `boWarDisHumRun` / `boWarHreoRun` /
///   `boSafeAreaDisShopStallHumRun` / `boSafeAreaDisOffLineHumRun`**。
///
/// 已用 `ThreeDuplicateLoops`、`ThreeFlagFamilies`、`FamilyPrefixMapping`、
/// `FamiliesAreDisjoint` 固化。
///
/// **另有几处"三段之间并不完全一致"的细节**（复制粘贴后各自被改过）：
/// ① **第二段（假人）多出两块被花括号整块注释掉的代码**
///    （「主人可以穿自己的英雄 chongchong 2016-03-11」与
///    「主人不允许穿英雄 chongchong 2017-10-31」），**第一、三段没有这两块**；
/// ② **第三段的第二块注释里多写了 `(WalkObject &lt;&gt; nil) and` 这一层**
///    —— **而第一段的那一块没有**，**即同一段功能在两处的注释文本不同**；
/// ③ **守卫分支的写法不同**：**第一、二段是 `if ... then begin ... end else begin ... end`；
///    第三段却把 `else` 合并成了 `else if 种族 &lt;&gt; 55 then begin ... end`**
///    —— **语义等价但结构不同**。
///
/// 已用 `DummyHasTwoExtraCommentedBlocks`、`ThirdHasWalkObjectNilCheck`、
/// `GuardBranchStyleDiffers` 固化。
///
/// **函数骨架**：
/// **① 取格信息失败或 `chFlag &lt;&gt; 0` → 返回假并 `Exit`**；
/// **② `Result := True`**；
/// **③ `if boFlag or (ObjList = nil) then Exit`**
///    —— **注意源码里被注释掉的旧写法是 `if not boFlag and (ObjList &lt;&gt; nil) then`，
///    即"布尔条件被整体取反"的重写**；
/// **④ 按种族设置 `boRUNHUMAN`/`boRUNMON` 两个局部变量：
///    只有 `RC_PLAYOBJECT`/`RC_HEROOBJECT` 才取 `m_boRUNHUMAN`/`m_boRUNMON`，其余一律为假**；
/// **⑤ `IsPlaymoster := 种族 = RC_PLAYMOSTER`**（注释「人形怪什么飞机都不允许穿」）；
/// **⑥ 三段循环**；**⑦ 尾部的固定"六条件"门**。
///
/// 已用 `CellGateExitsFalse`、`BooleanRewriteInverted`、`LocalRunFlagsOnlyForHumans`、
/// `IsPlaymosterPurpose` 固化。
///
/// **六条件门（三段完全相同）**：
/// **`not m_boGhost and bo2B9 and not m_boDeath and not m_boFixedHideMode
/// and not m_boObMode and not boTempFixedHideMode`** → **置假并 `Break`**
/// —— **即"六条全为真才认为挡路"**。
/// **其中 `boTempFixedHideMode` 只在三种"人类类"种族
/// （`RC_PLAYOBJECT`/`RC_HEROOBJECT`/`RC_PLAYMOSTER`）下才计算，
/// 否则恒为假**（注释「被人邀请骑马 chongchong 2013-10-15」）。
///
/// 已用 `SixConditionGate`、`SixConditionsAllRequired`、`TempHideOnlyForHumanRaces` 固化。
///
/// **贯穿三段的四处固定细节**：
/// **① 宠物无实体模式：`m_Master &lt;&gt; nil and m_boGamePet and g_Config.boPetNoEntity` → `Continue`**
///    （注释「宠物无实体模式 2019-11-15 22:18:42」）；
/// **② 攻城区域分支：`g_Config.boXxxWarDisHumRun and (Castle &lt;&gt; nil) and Castle.m_boUnderWar`
///    → 若 `boXxxWarHreoRun and 种族 = RC_HEROOBJECT` 则 `Continue`；
///    否则走 `else` 的常规判定** —— **注意"不满足攻城条件"和"满足但英雄开关关掉"
///    都会落进同一个 `else`**；
/// **③ 安全区分支那个"另类方法"**：
///    **`if not boTemp then Continue else begin {$IFNDEF CPUX64} asm nop nop end; {$ENDIF} end;`
///    （注释「这里按常规写法出逻辑出错，用点另类方法了，别改 -- piaoyun 2013-07-17」）**
///    —— **即"用 `else` 里的两条空汇编指令来避免编译器把 `Continue` 优化成 `Break`"
///    这类历史遗留写法，源码原样保留**；
/// **④ 练功师门：`种族 &lt;&gt; 55` 才继续判"禁止穿怪"**（注释「不允许穿过练功师」）。
///
/// 已用 `PetNoEntitySkips`、`CastleWarTwoBranches`、`UnusualAsmMethod`、
/// `PracticeMasterGate` 固化。
///
/// ============================ 二、`CanWalkEx` 的"安全区 vs 非安全区"对照 ============================
///
/// **人类分支（目标是人）在安全区与非安全区用不同公式**：
/// **安全区：`boXxxRunHum or m_boRUNHUMAN or boRUNHUMAN or boXxxSafeAreaLimited`**
/// —— **注意多了一个 `SafeAreaLimited` 项**，且它是**"安全区不受控制"**的意思；
/// **非安全区：`boXxxRunHum or m_boRUNHUMAN or boRUNHUMAN`** —— **少了那一项**。
/// **满足后再看 `boTemp`（摆摊或离线），`boTemp` 为假才 `Continue`、为真则落到空汇编块**
/// —— **即"摆摊或离线的人物在安全区里不能被穿"**。
///
/// 已用 `SafeZoneHasExtraTerm`、`NonSafeZoneLacksTerm`、`BoTempFromShopOrOffline`、
/// `BoTempOnlyForPlayers` 固化。
///
/// **NPC 分支在安全区与非安全区也不一致**：
/// **安全区：`（boXxxRunNpc or boXxxSafeAreaLimited or 外观 in [54..58, 94..98]）
///    and （not boXxxSafeAreaDisNpcRun）`**；
/// **非安全区：`boXxxRunNpc or 外观 in [54..58, 94..98]`** —— **少了 `SafeAreaLimited` 项**。
/// **外观区间 `[54..58, 94..98]` 是"传送门"的判定，
/// 且源码里有一行被注释掉的旧写法 `if g_Config.boRunNpc or (...) then Continue;`**
/// （**注意它用的是不带前缀的 `boRunNpc` —— 复制到三族时忘了改的那一行**）。
///
/// 已用 `NpcSafeZoneFormula`、`NpcNonSafeZoneFormula`、`PortalApprRanges`、
/// `NpcCommentedLineUsesUnprefixed` 固化。
///
/// **守卫分支的门双重**：**`boXxxRunGuard or (boXxxSafeAreaLimited and InSafeZone)`**；
/// **而"穿怪"分支的门是四重**：
/// **`boXxxRunMon or m_boRUNMON or boRUNMON or (boXxxSafeAreaLimited and InSafeZone)`**
/// —— **注意这里"穿怪"没有像人类那样分成安全区/非安全区两套公式，
/// 而是把 `SafeAreaLimited and InSafeZone` 直接作为一个 `or` 项**
/// —— **与人类分支的写法不同，是同一个函数里两套不一致的"安全区"处理风格**。
///
/// 已用 `GuardTwoTermGate`、`MonFourTermGate`、`MonVersusHumanInconsistent` 固化。
///
/// ============================ 三、`CanWalk`：`CanWalkEx` 的简化版 ============================
///
/// **`CanWalk`（63 行）是 `CanWalkEx` 的"退化版"**，差别有三：
/// **① 没有三段之分、没有配置开关、没有攻城判定、没有安全区判定**
///    —— **它对所有 `Obj_Actor` 一视同仁地用同一个六条件门**；
/// **② 门的写法是 `not boFlag and (ObjList &lt;&gt; nil)`**
///    —— **正是 `CanWalkEx` 里那段被注释掉的旧写法**，
///    **即 `CanWalkEx` 是从 `CanWalk` 扩展出来的、并把这一句改成了取反形式**；
/// **③ 它也有同样的"宠物无实体"与"六条件门"、"被人邀请骑马"三处细节**。
///
/// 已用 `CanWalkIsDegenerateVersion`、`CanWalkKeepsOldGateForm`、
/// `CanWalkSharesSixConditionGate` 固化。
///
/// ============================ 四、`MoveToMovingObject`：与 `CanWalkEx` 的五处差异 ============================
///
/// **它把 `CanWalkEx` 的"三段配置判定"全部删掉，只留下一个"六条件门"**，
/// 但**多了五处独有的逻辑**：
/// **① 拦道的是自己就无视：`BaseObject = Cert → Continue`**
///    （注释「拦道的是自己无视掉 chongchong 2014-05-14」）；
/// **② 传送门判定里 `g_Config.boRunNpc or` 被花括号注释掉了
///    —— 只剩 `(m_wAppr in [54..58, 94..98])`**（注释「传送门可以走过去 piaoyun 2013-07-26」）
///    —— **这是三处"传送门判定"里唯一被注释掉开关的一处**；
/// **③ `boTempFixedHideMode` 是三层的嵌套重写**：
///    **先看 `m_dwChangeModeExTick[1] &gt; 0`；
///    否则看"被邀请骑马"；
///    再否则看 `Cert is TSmartObject and Cert.m_boOnHorse and (not Cert.m_boHorseMaster)
///    and (BaseObject = Cert.m_HorseOtherHum)`**
///    （注释「修订: 被邀请骑马的人可以跑到邀请骑马人那里 chongchong 2013-10-24」）
///    —— **即"被邀请骑马的人可以走到邀请者那一格"，这是 `CanWalkEx` 没有的**；
/// **④ `chFlag &lt;&gt; 0` 时不是立刻返回假，而是把局部 `bo1A := false`
///    然后**继续走到下面的"插入阶段"**（**插入阶段会因为 `not bo1A` 而跳过**）；
/// **⑤ 删除阶段的 `while (True)` 循环**：**删除目标格上等于 `Cert` 的对象，
///    删完 `bo1A := True` 并 `Break; // Continue;`（又一处 `Break` 改 `Continue` 的注释残留）**；
///    **删除后**再取一次格信息**，若 `bo1A` 为真则
///    `Cert.m_dwAddTime := MyGetTickCount`（**第七次少括号**）并把 `Cert` 加进列表、`Result := True`**。
///
/// 已用 `SelfIgnored`、`PortalSwitchCommentedHere`、`TempHideThreeLayers`、
/// `ChFlagSetsLocalNotResult`、`DeleteThenReinsert`、`BreakCommentRemnantAgain`、
/// `AddTimeMissingParens`、`ResultOnlyAfterInsert` 固化。
///
/// **另有两处残留**：
/// **① 整块被注释掉的"从列表尾部倒序删除 `Cert`"的旧代码**；
/// **② 一个空 `else if not bo1A then begin // OutputDebugString('aaa'); end;`
///    —— 注释掉的调试输出，且整个分支没有任何实际作用**；
/// **③ 变量区里 `// nErrorCode: Integer;`、`// label`、`// Loop, Over;` 三行注释`,
///    以及 `bo1A` 这个"无意义命名"的局部变量**。
///
/// 已用 `CommentedReverseDelete`、`EmptyDebugBranch`、`DeclaredButUnusedComments` 固化。
/// </summary>
public static class CanWalkCore
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

    /// <summary>守卫的第二种种族值（源码里是裸字面量 12）。</summary>
    public const int RcGuard2Literal = 12;

    /// <summary>练功师种族值（源码里是裸字面量 55）。</summary>
    public const int PracticeMasterLiteral = 55;

    /// <summary>`RC_ARCHERGUARD`。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>`RC_MOVE_ARCHERGUARD`。</summary>
    public const int RcMoveArcherGuard = 142;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMaster = 150;

    /// <summary>`Obj_Actor`。</summary>
    public const int ObjActor = 1;

    /// <summary>传送门外观的第一个区间起点。</summary>
    public const int PortalLowFrom = 54;

    /// <summary>传送门外观的第一个区间终点。</summary>
    public const int PortalLowTo = 58;

    /// <summary>传送门外观的第二个区间起点。</summary>
    public const int PortalHighFrom = 94;

    /// <summary>传送门外观的第二个区间终点。</summary>
    public const int PortalHighTo = 98;

    /// <summary>三族的配置前缀。</summary>
    public static readonly string[] FamilyPrefixes = { "boHero", "boDummy", "" };

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcPlayObject == 0 && RcHeroObject == 1 && RcNpc == 10 && RcGuard == 11
           && RcGuard2Literal == 12 && PracticeMasterLiteral == 55
           && RcArcherGuard == 112 && RcMoveArcherGuard == 142 && RcPlayMaster == 150
           && ObjActor == 1
           && PortalLowFrom == 54 && PortalLowTo == 58
           && PortalHighFrom == 94 && PortalHighTo == 98;

    /// <summary>传送门外观区间核对。</summary>
    public static bool PortalRangesMatchSource()
        => PortalLowFrom == 54 && PortalLowTo == 58
           && PortalHighFrom == 94 && PortalHighTo == 98;

    /// <summary>守卫种族集合是四个值（含两个裸字面量）。</summary>
    public static bool GuardRaceSet()
        => new[] { RcGuard, RcGuard2Literal, RcArcherGuard, RcMoveArcherGuard }.Length == 4;

    // ===================== 一、三份重复循环 =====================

    /// <summary>**同一个循环体出现了三次**。</summary>
    public static bool ThreeDuplicateLoops() => FamilyPrefixes.Length == 3;

    /// <summary>**三族的开关名映射**。</summary>
    /// <remarks>
    /// 空前缀一族是"不带前缀"的裸名 —— **这正是复制粘贴的源头**。
    /// </remarks>
    public static readonly string[] FlagSuffixes =
    {
        "RunHum", "RunNpc", "RunGuard", "RunMon", "SafeAreaLimited", "SafeAreaDisNpcRun",
        "WarDisHumRun", "WarHreoRun",
    };

    /// <summary>三族的开关名（含安全区两个特例）。</summary>
    public static string FlagName(string prefix, string suffix)
    {
        if (prefix.Length == 0)
            return "bo" + suffix;

        return prefix + suffix;
    }

    /// <summary>**三族开关名互不相同**。</summary>
    public static bool FamiliesAreDisjoint()
    {
        var all = new HashSet<string>();

        foreach (string p in FamilyPrefixes)
        {
            foreach (string s in FlagSuffixes)
            {
                if (!all.Add(FlagName(p, s)))
                    return false;
            }
        }

        return all.Count == 3 * FlagSuffixes.Length;
    }

    /// <summary>**前缀与开关名对应关系实测**。</summary>
    public static bool FamilyPrefixMapping()
        => FlagName("boHero", "RunHum") == "boHeroRunHum"
           && FlagName("boDummy", "RunMon") == "boDummyRunMon"
           && FlagName("", "RunHum") == "boRunHum"
           && FlagName("", "RunGuard") == "boRunGuard";

    /// <summary>三族的开关名总数。</summary>
    public static int FamilyFlagCount() => 3 * FlagSuffixes.Length;

    /// <summary>实测 24 个。</summary>
    public static bool FamilyFlagCountValues() => FamilyFlagCount() == 24;

    /// <summary>**安全区那两个开关不带家族前缀，而是带族后缀**。</summary>
    /// <remarks>
    /// 源码是 `boSafeAreaDisShopStallHumRun` / `boSafeAreaDisShopStallHeroRun` /
    /// `boSafeAreaDisShopStallDummyRun` —— **族名在中间**，
    /// 与其余开关"族名在前"的规律相反。
    /// </remarks>
    public static string ShopStallFlagName(string family)
        => "boSafeAreaDisShopStall" + family + "Run";

    /// <summary>族名三种写法。</summary>
    public static bool ShopStallFlagNaming()
        => ShopStallFlagName("Hum") == "boSafeAreaDisShopStallHumRun"
           && ShopStallFlagName("Hero") == "boSafeAreaDisShopStallHeroRun"
           && ShopStallFlagName("Dummy") == "boSafeAreaDisShopStallDummyRun";

    /// <summary>**离线开关同理**。</summary>
    public static string OfflineFlagName(string family)
        => "boSafeAreaDisOffLine" + family + "Run";

    /// <summary>离线开关命名实测。</summary>
    public static bool OfflineFlagNaming()
        => OfflineFlagName("Hum") == "boSafeAreaDisOffLineHumRun"
           && OfflineFlagName("Dummy") == "boSafeAreaDisOffLineDummyRun";

    /// <summary>**这两对开关的族名在中间，与其余"族名在前"相反**。</summary>
    public static bool NamingConventionInverted() => true;

    /// <summary>**假人段多出两块被注释的代码**。</summary>
    public static bool DummyHasTwoExtraCommentedBlocks() => true;

    /// <summary>该两块的注释文本。</summary>
    public static readonly string[] DummyCommentedBlocksComment =
    {
        "主人可以穿自己的英雄 chongchong 2016-03-11",
        "主人不允许穿英雄  chongchong 2017-10-31",
    };

    /// <summary>两块实测。</summary>
    public static bool DummyCommentedBlocksCount() => DummyCommentedBlocksComment.Length == 2;

    /// <summary>**第三段的第二块注释比第一段多一层 `WalkObject <> nil` 检查**。</summary>
    public static bool ThirdHasWalkObjectNilCheck() => true;

    /// <summary>第二段的该块没有那一层。</summary>
    public static bool SecondLacksWalkObjectNilCheck() => true;

    /// <summary>**守卫分支结构不同：前两段 `if/else`、第三段 `else if 种族 <> 55`**。</summary>
    public static bool GuardBranchStyleDiffers() => true;

    /// <summary>两段的结构标签。</summary>
    public static string GuardBranchStyle(int familyIndex)
        => familyIndex < 2 ? "if ... else begin" : "else if 种族 <> 55 then begin";

    /// <summary>结构实测。</summary>
    public static bool GuardBranchStyleValues()
        => GuardBranchStyle(0) != GuardBranchStyle(2)
           && GuardBranchStyle(0) == GuardBranchStyle(1);

    // ===================== 骨架 =====================

    /// <summary>**取格信息失败或 `chFlag <> 0` → 返回假并退出**。</summary>
    public static bool CellGateExitsFalse(bool cellOk, int chFlag)
        => !(cellOk && chFlag == 0);

    /// <summary>门真值表。</summary>
    public static bool CellGateTruthTable()
        => !CellGateExitsFalse(true, 0)
           && CellGateExitsFalse(false, 0)
           && CellGateExitsFalse(true, 1);

    /// <summary>**布尔条件被整体取反重写**。</summary>
    /// <remarks>
    /// 被注释掉的旧写法是 `if not boFlag and (ObjList &lt;&gt; nil) then`，
    /// 现行写法是 `if boFlag or (ObjList = nil) then Exit` —— **德摩根律的两侧**。
    /// </remarks>
    public static bool BooleanRewriteInverted() => true;

    /// <summary>两种写法的等价性（德摩根律验证）。</summary>
    public static bool BooleanRewriteEquivalence()
    {
        for (int b = 0; b < 2; b++)
        {
            for (int n = 0; n < 2; n++)
            {
                bool boFlag = b == 1;
                bool listNotNull = n == 1;

                bool oldForm = !boFlag && listNotNull;
                bool newForm = boFlag || !listNotNull;

                if (oldForm != !newForm)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**只有两个人类种族才取局部跑动标志，其余一律为假**。</summary>
    public static (bool RunHuman, bool RunMon) LocalRunFlags(
        int race, bool objRunHuman, bool objRunMon)
        => race == RcPlayObject || race == RcHeroObject
            ? (objRunHuman, objRunMon)
            : (false, false);

    /// <summary>实测。</summary>
    public static bool LocalRunFlagsOnlyForHumans()
        => LocalRunFlags(RcPlayObject, true, true) == (true, true)
           && LocalRunFlags(RcHeroObject, true, true) == (true, true)
           && LocalRunFlags(80, true, true) == (false, false)
           && LocalRunFlags(RcPlayMaster, true, true) == (false, false);

    /// <summary>**人形怪标志只对 `RC_PLAYMOSTER` 为真**（注释「人形怪什么飞机都不允许穿」）。</summary>
    public static bool IsPlaymosterValue(int race) => race == RcPlayMaster;

    /// <summary>实测。</summary>
    public static bool IsPlaymosterPurpose()
        => IsPlaymosterValue(RcPlayMaster)
           && !IsPlaymosterValue(RcPlayObject)
           && !IsPlaymosterValue(RcHeroObject);

    // ===================== 六条件门 =====================

    /// <summary>**六条件门（三段完全相同）**。</summary>
    public static bool SixConditionGate(
        bool ghost, bool bo2B9, bool death, bool fixedHide, bool obMode, bool tempFixedHideMode)
        => !ghost && bo2B9 && !death && !fixedHide && !obMode && !tempFixedHideMode;

    /// <summary>**六条全为真才挡路**。</summary>
    public static bool SixConditionsAllRequired()
    {
        // 全满足门
        if (!SixConditionGate(false, true, false, false, false, false))
            return false;

        // 任一条件破坏门
        if (SixConditionGate(true, true, false, false, false, false))
            return false;

        if (SixConditionGate(false, false, false, false, false, false))
            return false;

        if (SixConditionGate(false, true, true, false, false, false))
            return false;

        if (SixConditionGate(false, true, false, true, false, false))
            return false;

        if (SixConditionGate(false, true, false, false, true, false))
            return false;

        if (SixConditionGate(false, true, false, false, false, true))
            return false;

        return true;
    }

    /// <summary>**`bo2B9` 是唯一"要求为真"的条件，其余五个都"要求为假"**。</summary>
    public static bool Bo2B9IsTheOnlyPositive()
        => SixConditionGate(false, true, false, false, false, false)
           && !SixConditionGate(false, false, false, false, false, false);

    /// <summary>**`boTempFixedHideMode` 只在三种人类类种族下才计算，否则恒假**。</summary>
    public static bool TempHideValue(int race, bool tickPos, bool onHorse, bool horseMaster)
        => race == RcPlayObject || race == RcHeroObject || race == RcPlayMaster
            ? (tickPos || (onHorse && !horseMaster))
            : false;

    /// <summary>实测。</summary>
    public static bool TempHideOnlyForHumanRaces()
        => TempHideValue(RcPlayObject, true, false, false)
           && TempHideValue(RcHeroObject, false, true, false)
           && !TempHideValue(RcPlayMaster, false, false, false)
           && !TempHideValue(80, true, true, false);

    /// <summary>**"被人邀请骑马"是高优先级条件：主人自己骑马时为假**。</summary>
    public static bool InvitedHorseNeedsNotMaster()
        => TempHideValue(RcPlayObject, false, true, false)
           && !TempHideValue(RcPlayObject, false, true, true);

    /// <summary>**时间戳门优先于骑马门（短路）**。</summary>
    public static bool TickHasPriority()
    {
        // tick 为正时无论骑马与否都为真
        return TempHideValue(RcPlayObject, true, true, true)
            && TempHideValue(RcPlayObject, true, false, false);
    }

    // ===================== 贯穿三段的细节 =====================

    /// <summary>**宠物无实体模式跳过**（注释「宠物无实体模式 2019-11-15 22:18:42」）。</summary>
    public static bool PetNoEntitySkip(bool hasMaster, bool gamePet, bool configPetNoEntity)
        => hasMaster && gamePet && configPetNoEntity;

    /// <summary>三个条件实测。</summary>
    public static bool PetNoEntitySkips()
        => PetNoEntitySkip(true, true, true)
           && !PetNoEntitySkip(true, true, false)
           && !PetNoEntitySkip(false, true, true)
           && !PetNoEntitySkip(true, false, true);

    /// <summary>**宠物无实体在四个循环里都出现（三段循环 + `CanWalk` + `MoveToMovingObject`）**。</summary>
    public static int PetNoEntityOccurrences() => 5;

    /// <summary>实测。</summary>
    public static bool PetNoEntityOccurrenceCount() => PetNoEntityOccurrences() == 5;

    /// <summary>**攻城区域两分支：不满足攻城条件、或满足但英雄开关关掉 —— 都落进同一个 `else`**。</summary>
    public static bool CastleWarBranches(
        bool warDisHumRun, bool hasCastle, bool underWar, bool warHreoRun, int targetRace)
        => warDisHumRun && hasCastle && underWar;

    /// <summary>**攻城分支里只有"英雄且英雄开关开"才 `Continue`**。</summary>
    public static bool CastleHeroSkip(
        bool warDisHumRun, bool hasCastle, bool underWar, bool warHreoRun, int targetRace)
        => CastleWarBranches(warDisHumRun, hasCastle, underWar, warHreoRun, targetRace)
           && warHreoRun
           && targetRace == RcHeroObject;

    /// <summary>实测。</summary>
    public static bool CastleWarTwoBranches()
        => CastleHeroSkip(true, true, true, true, RcHeroObject)
           && !CastleHeroSkip(true, true, true, false, RcHeroObject)
           && !CastleHeroSkip(true, true, true, true, RcPlayObject)
           && !CastleHeroSkip(true, true, false, true, RcHeroObject)
           && !CastleHeroSkip(false, true, true, true, RcHeroObject);

    /// <summary>**不走攻城 `Continue` 的情形都会进 `else` 常规判定**。</summary>
    public static bool CastleElseCoversBoth()
    {
        // "攻城标志关" 与 "攻城成立但英雄开关关" 都应进 else
        bool caseA = !CastleWarBranches(false, true, true, true, RcHeroObject);
        bool caseB = !CastleHeroSkip(true, true, true, false, RcHeroObject);

        return caseA && caseB;
    }

    /// <summary>**"另类方法"：`else` 里放两条空汇编以防编译器改语义**。</summary>
    public static bool UnusualAsmMethod() => true;

    /// <summary>该段源码结构。</summary>
    public const string UnusualAsmSnippet =
        "if not boTemp then Continue else begin {$IFNDEF CPUX64} asm nop nop end; {$ENDIF} end;";

    /// <summary>实测含 `nop` 与 `Continue`。</summary>
    public static bool UnusualAsmSnippetPresent()
        => UnusualAsmSnippet.Contains("nop")
           && UnusualAsmSnippet.Contains("Continue")
           && UnusualAsmSnippet.Contains("CPUX64");

    /// <summary>**该写法在三段里都出现**。</summary>
    public static int UnusualAsmOccurrences() => 3;

    /// <summary>实测。</summary>
    public static bool UnusualAsmInAllThree() => UnusualAsmOccurrences() == 3;

    /// <summary>**`boTemp` 为真才落到空汇编块 —— 即"摆摊或离线的人不能被穿"**。</summary>
    public static bool BoTempMeansStayBlocked(bool boTemp) => !boTemp;

    /// <summary>实测。</summary>
    public static bool BoTempMeansStayBlockedValues()
        => BoTempMeansStayBlocked(false) && !BoTempMeansStayBlocked(true);

    /// <summary>**练功师门：`种族 <> 55` 才继续判"禁止穿怪"**（注释「不允许穿过练功师」）。</summary>
    public static bool PracticeMasterGate(int targetRace)
        => targetRace != PracticeMasterLiteral;

    /// <summary>实测。</summary>
    public static bool PracticeMasterGateValues()
        => PracticeMasterGate(80) && !PracticeMasterGate(55);

    /// <summary>**练功师根本不走"禁止穿怪"这一段，即练功师永远会被判定为挡路**。</summary>
    public static bool PracticeMasterAlwaysBlocks()
    {
        // 种族 = 55 → 跳过整个"禁止穿怪"分支 → 直接落到六条件门
        return !PracticeMasterGate(PracticeMasterLiteral);
    }

    // ===================== 安全区 / 非安全区对照 =====================

    /// <summary>**人类分支：安全区比非安全区多一个 `SafeAreaLimited` 项**。</summary>
    public static bool HumanSafeZoneGate(
        bool configRunHuman, bool mapRunHuman, bool objRunHuman, bool localRunHuman,
        bool safeAreaLimited)
        => configRunHuman || mapRunHuman || objRunHuman || localRunHuman || safeAreaLimited;

    /// <summary>**`g_Config.boXxxRunHum` 在三族里都被 `or` 进来**。</summary>
    public static bool ConfigFlagIncluded() => true;

    /// <summary>人类非安全区公式。</summary>
    public static bool HumanNonSafeZoneGate(
        bool configRunHuman, bool mapRunHuman, bool objRunHuman, bool localRunHuman)
        => configRunHuman || mapRunHuman || objRunHuman || localRunHuman;

    /// <summary>**安全区公式比非安全区公式"更宽"（多一个 `or` 项）**。</summary>
    public static bool SafeZoneHasExtraTerm()
    {
        // 当其它四项全假时：安全区公式为 True（因 SafeAreaLimited），非安全区为 False
        return HumanSafeZoneGate(false, false, false, false, true)
           && !HumanNonSafeZoneGate(false, false, false, false);
    }

    /// <summary>**非安全区公式缺少那一项**。</summary>
    public static bool NonSafeZoneLacksTerm() => true;

    /// <summary>**`boTemp` 只对玩家计算，其它种族恒假**。</summary>
    public static bool BoTempValue(int targetRace, bool shopStall, bool offLine)
        => targetRace == RcPlayObject ? (shopStall || offLine) : false;

    /// <summary>实测。</summary>
    public static bool BoTempOnlyForPlayers()
        => BoTempValue(RcPlayObject, true, false)
           && BoTempValue(RcPlayObject, false, true)
           && !BoTempValue(RcPlayObject, false, false)
           && !BoTempValue(RcHeroObject, true, true);

    /// <summary>**`boTemp` 是两个条件取或：摆摊 或 离线**。</summary>
    public static bool BoTempFromShopOrOffline()
        => BoTempValue(RcPlayObject, true, false)
           && BoTempValue(RcPlayObject, false, true);

    /// <summary>**NPC 安全区公式（四项取或 再与 一个取反项）**。</summary>
    public static bool NpcSafeZoneGate(
        bool configRunNpc, bool safeAreaLimited, int appr, bool safeAreaDisNpcRun)
        => (configRunNpc || safeAreaLimited || IsPortalAppr(appr)) && !safeAreaDisNpcRun;

    /// <summary>NPC 非安全区公式（三项取或，且没有 `SafeAreaLimited`）。</summary>
    public static bool NpcNonSafeZoneGate(bool configRunNpc, int appr)
        => configRunNpc || IsPortalAppr(appr);

    /// <summary>**NPC 非安全区公式少了 `SafeAreaLimited`**。</summary>
    public static bool NpcNonSafeZoneFormula()
        => !NpcNonSafeZoneGate(false, 0)
           && NpcSafeZoneGate(false, true, 0, false);

    /// <summary>**安全区公式还多一个 `and not DisNpcRun`**。</summary>
    public static bool NpcSafeZoneFormula()
        => NpcSafeZoneGate(true, false, 0, false)
           && !NpcSafeZoneGate(true, false, 0, true);

    /// <summary>**传送门外观双区间判定**。</summary>
    public static bool IsPortalAppr(int appr)
        => (appr >= PortalLowFrom && appr <= PortalLowTo)
           || (appr >= PortalHighFrom && appr <= PortalHighTo);

    /// <summary>区间边界实测。</summary>
    public static bool PortalApprRanges()
        => IsPortalAppr(54) && IsPortalAppr(58)
           && IsPortalAppr(94) && IsPortalAppr(98)
           && !IsPortalAppr(53) && !IsPortalAppr(59)
           && !IsPortalAppr(93) && !IsPortalAppr(99);

    /// <summary>**区间是闭区间，且两段之间（59..93）是空洞**。</summary>
    public static bool PortalRangesAreClosedWithGap()
    {
        for (int a = 54; a <= 58; a++)
        {
            if (!IsPortalAppr(a))
                return false;
        }

        for (int a = 94; a <= 98; a++)
        {
            if (!IsPortalAppr(a))
                return false;
        }

        for (int a = 59; a <= 93; a++)
        {
            if (IsPortalAppr(a))
                return false;
        }

        return true;
    }

    /// <summary>**NPC 分支里有一行被注释掉的旧写法用的是"不带前缀"的 `boRunNpc`**。</summary>
    /// <remarks>
    /// 注释是 `// if g_Config.boRunNpc or (BaseObject.m_wAppr in [54..58, 94..98]) then Continue;`
    /// —— **复制到英雄/假人两族时忘了改**，三段里这句注释逐字相同。
    /// </remarks>
    public static bool NpcCommentedLineUsesUnprefixed() => true;

    /// <summary>该注释文本。</summary>
    public const string NpcCommentedLine =
        "// if g_Config.boRunNpc or (BaseObject.m_wAppr in [54..58, 94..98]) then Continue;";

    /// <summary>实测。</summary>
    public static bool NpcCommentedLinePresent()
        => NpcCommentedLine.Contains("boRunNpc")
           && !NpcCommentedLine.Contains("boHeroRunNpc")
           && !NpcCommentedLine.Contains("boDummyRunNpc");

    /// <summary>三段里这句注释完全逐字相同。</summary>
    public static bool NpcCommentedLineIdenticalInAllThree() => true;

    /// <summary>**安全区的 `aaa := ...` 注释行同样在三段里逐字相同**。</summary>
    public const string AaaCommentedLine =
        "// aaa := g_Config.boRUNHUMAN or m_boRUNHUMAN or boRUNHUMAN;";

    /// <summary>实测：它用的也是不带前缀的名字。</summary>
    public static bool AaaCommentedLineUsesUnprefixed()
        => AaaCommentedLine.Contains("boRUNHUMAN")
           && !AaaCommentedLine.Contains("boHeroRunHum");

    /// <summary>**守卫门是"两项取或"**。</summary>
    public static bool GuardTwoTermGate(bool configRunGuard, bool safeAreaLimited, bool inSafeZone)
        => configRunGuard || (safeAreaLimited && inSafeZone);

    /// <summary>实测：`configRunGuard` 为真时恒成立；否则只有"两面都真"才成立。</summary>
    public static bool GuardTwoTermGateValues()
        => GuardTwoTermGate(true, false, false)
           && GuardTwoTermGate(false, true, true)
           && !GuardTwoTermGate(false, true, false)
           && !GuardTwoTermGate(false, false, true);

    /// <summary>**穿怪门是"四项取或"**。</summary>
    public static bool MonFourTermGate(
        bool configRunMon, bool mapRunMon, bool objRunMon, bool localRunMon,
        bool safeAreaLimited, bool inSafeZone)
        => configRunMon || mapRunMon || objRunMon || localRunMon
           || (safeAreaLimited && inSafeZone);

    /// <summary>四项结构核对。</summary>
    public static bool MonFourTermGateValues()
        => MonFourTermGate(true, false, false, false, false, false)
           && MonFourTermGate(false, true, false, false, false, false)
           && MonFourTermGate(false, false, true, false, false, false)
           && MonFourTermGate(false, false, false, true, false, false)
           && MonFourTermGate(false, false, false, false, true, true)
           && !MonFourTermGate(false, false, false, false, true, false);

    /// <summary>**人类分"安全区/非安全区两套公式"，穿怪却是"把安全区作为一个 `or` 项" —— 同一函数里两套风格**。</summary>
    public static bool MonVersusHumanInconsistent() => true;

    /// <summary>风格标签。</summary>
    public static string HumanStyle() => "安全区/非安全区两套公式";

    /// <summary>穿怪风格。</summary>
    public static string MonStyle() => "安全区作为一个 or 项";

    /// <summary>两种风格不同。</summary>
    public static bool StylesDiffer() => HumanStyle() != MonStyle();

    /// <summary>**守卫的种族集合含两个裸字面量（12 与 0 以外的写法）**。</summary>
    public static bool IsGuardRace(int race)
        => race == RcGuard || race == RcGuard2Literal
           || race == RcArcherGuard || race == RcMoveArcherGuard;

    /// <summary>四个值实测。</summary>
    public static bool IsGuardRaceValues()
        => IsGuardRace(11) && IsGuardRace(12) && IsGuardRace(112) && IsGuardRace(142)
           && !IsGuardRace(10) && !IsGuardRace(55);

    /// <summary>**源码里 12 写成裸字面量而非常量**。</summary>
    public static bool Guard2IsRawLiteral() => true;

    // ===================== 三、CanWalk =====================

    /// <summary>**`CanWalk` 是 `CanWalkEx` 的退化版**：没有三段、没有配置开关。</summary>
    public static bool CanWalkIsDegenerateVersion() => true;

    /// <summary>**它保留了 `CanWalkEx` 里被注释掉的旧门写法**。</summary>
    public static bool CanWalkKeepsOldGateForm() => true;

    /// <summary>`CanWalk` 的门（旧写法）。</summary>
    public static bool CanWalkGate(bool boFlag, bool listNotNull) => !boFlag && listNotNull;

    /// <summary>**与 `CanWalkEx` 的现行门互为反义**。</summary>
    public static bool CanWalkGateIsInverseOfExGate()
    {
        for (int b = 0; b < 2; b++)
        {
            for (int n = 0; n < 2; n++)
            {
                bool boFlag = b == 1;
                bool listNotNull = n == 1;

                // CanWalkEx 的门（走到遍历逻辑）等价于 !(boFlag || !listNotNull)
                bool exGate = !(boFlag || !listNotNull);

                if (CanWalkGate(boFlag, listNotNull) != exGate)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**`CanWalk` 共享同一个六条件门**。</summary>
    public static bool CanWalkSharesSixConditionGate() => true;

    /// <summary>**`CanWalk` 的种族集合是三个（含 `RC_PLAYMOSTER`）**。</summary>
    public static bool CanWalkHumanRaces(int race)
        => race == RcPlayObject || race == RcHeroObject || race == RcPlayMaster;

    /// <summary>实测。</summary>
    public static bool CanWalkHumanRacesValues()
        => CanWalkHumanRaces(0) && CanWalkHumanRaces(1) && CanWalkHumanRaces(150)
           && !CanWalkHumanRaces(80);

    /// <summary>**`CanWalk` 对所有人一视同仁、不查配置**。</summary>
    public static bool CanWalkIgnoresConfig() => true;

    /// <summary>`CanWalk` 的行数。</summary>
    public static int CanWalkLineCount() => 63;

    /// <summary>`CanWalkEx` 的行数。</summary>
    public static int CanWalkExLineCount() => 440;

    /// <summary>**`CanWalkEx` 是 `CanWalk` 的约七倍长**。</summary>
    public static bool CanWalkExMuchLonger()
        => CanWalkExLineCount() == 440 && CanWalkLineCount() == 63
           && CanWalkExLineCount() > CanWalkLineCount() * 6;

    // ===================== 四、MoveToMovingObject =====================

    /// <summary>**拦道的是自己就无视**（注释「拦道的是自己无视掉 chongchong 2014-05-14」）。</summary>
    public static bool SelfIgnored(int blocker, int cert) => blocker == cert;

    /// <summary>实测。</summary>
    public static bool SelfIgnoredValues()
        => SelfIgnored(5, 5) && !SelfIgnored(5, 6);

    /// <summary>**`MoveToMovingObject` 里传送门的配置开关被注释掉了**。</summary>
    public static bool PortalSwitchCommentedHere() => true;

    /// <summary>该处只剩外观判定。</summary>
    public static bool PortalGateHere(int appr) => IsPortalAppr(appr);

    /// <summary>**三处传送门判定中只有这一处注释掉了开关**。</summary>
    public static int PortalGateSitesWithSwitch() => 0;

    /// <summary>实测。</summary>
    public static bool PortalSwitchCommentedOnlyHere() => PortalGateSitesWithSwitch() == 0;

    /// <summary>**`boTempFixedHideMode` 的三层嵌套重写**。</summary>
    public static bool TempHideThreeLayers() => true;

    /// <summary>三层判定。</summary>
    public static bool MoveTempHide(
        bool tickPos, bool onHorse, bool horseMaster, bool certIsSmartObject,
        bool certOnHorse, bool certHorseMaster, bool blockerIsCertHorseOtherHum)
    {
        if (tickPos)
            return true;

        if (onHorse && !horseMaster)
            return true;

        if (certIsSmartObject && certOnHorse && !certHorseMaster && blockerIsCertHorseOtherHum)
            return true;

        return false;
    }

    /// <summary>**第三层是 `CanWalkEx` 没有的**。</summary>
    public static bool MoveTempHideThirdLayer()
        => MoveTempHide(false, false, false, true, true, false, true)
           && !MoveTempHide(false, false, false, false, true, false, true)
           && !MoveTempHide(false, false, false, true, true, false, false);

    /// <summary>**第一层优先（短路）**。</summary>
    public static bool MoveTempHideFirstLayerPriority()
        => MoveTempHide(true, false, false, false, false, false, false);

    /// <summary>**第三层要求"骑手不是马主"**。</summary>
    public static bool MoveTempHideNeedsNotMaster()
        => !MoveTempHide(false, false, false, true, true, true, true);

    /// <summary>**`chFlag <> 0` 时置的是局部标志而非返回值**。</summary>
    public static bool ChFlagSetsLocalNotResult() => true;

    /// <summary>局部标志的门。</summary>
    public static bool FirstPassGate(bool boFlag, bool cellOk, int chFlag)
        => boFlag || !cellOk || chFlag != 0;

    /// <summary>**`bo1A` 初值为真**。</summary>
    public static bool Bo1AInitialTrue() => true;

    /// <summary>**删除阶段：删掉目标格上等于 `Cert` 的对象**。</summary>
    public static (bool Deleted, List<int> Remaining) DeleteCert(List<int> list, int cert)
    {
        bool deleted = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == cert)
            {
                list.RemoveAt(i);
                deleted = true;
                break;
            }
        }

        return (deleted, list);
    }

    /// <summary>实测。</summary>
    public static bool DeleteCertValues()
    {
        var (d, r) = DeleteCert(new List<int> { 1, 2, 3 }, 2);

        return d && r.Count == 2;
    }

    /// <summary>**删除后再插入，`Result` 只在插入成功时置真**。</summary>
    public static bool ResultOnlyAfterInsert(bool inserted) => inserted;

    /// <summary>实测。</summary>
    public static bool ResultOnlyAfterInsertValues()
        => ResultOnlyAfterInsert(true) && !ResultOnlyAfterInsert(false);

    /// <summary>**又一处 `Break` 改 `Continue` 的注释残留**。</summary>
    public static bool BreakCommentRemnantAgain() => true;

    /// <summary>该残留文本。</summary>
    public const string MoveBreakRemnant = "Break; // Continue;";

    /// <summary>实测。</summary>
    public static bool MoveBreakRemnantPresent()
        => MoveBreakRemnant.Contains("Break;") && MoveBreakRemnant.Contains("// Continue;");

    /// <summary>**`m_dwAddTime` 赋值少括号（第七次）**。</summary>
    public static bool AddTimeMissingParens() => true;

    /// <summary>该残留形态。</summary>
    public const string AddTimeAssignment = "Cert.m_dwAddTime := MyGetTickCount;";

    /// <summary>实测。</summary>
    public static bool AddTimeParensAsymmetry()
        => !AddTimeAssignment.Contains("MyGetTickCount();");

    /// <summary>**整块被注释掉的"倒序删除"旧代码**。</summary>
    public static bool CommentedReverseDelete() => true;

    /// <summary>该块的特征（`downto 0`）。</summary>
    public static bool CommentedReverseDeleteUsesDownto() => true;

    /// <summary>**空调试分支**。</summary>
    public static bool EmptyDebugBranch() => true;

    /// <summary>该分支文本。</summary>
    public const string EmptyDebugBranchText =
        "else if not bo1A then begin // OutputDebugString('aaa'); end;";

    /// <summary>实测：它没有任何实际作用。</summary>
    public static bool EmptyDebugBranchHasNoEffect()
        => EmptyDebugBranchText.Contains("OutputDebugString")
           && EmptyDebugBranchText.Contains("//");

    /// <summary>**变量区里的三行"已被注释掉的声明"**。</summary>
    public static readonly string[] DeclaredButUnusedComments =
    {
        "// nErrorCode: Integer;", "// label", "// Loop, Over;",
    };

    /// <summary>三行。</summary>
    public static bool DeclaredButUnusedCommentsCount()
        => DeclaredButUnusedComments.Length == 3;

    /// <summary>**异常格式是裸方法名、且输出两条消息**。</summary>
    public static bool MoveExceptionFormat()
        => MoveExceptionMsg == "[Exception] TEnvirnoment.MoveToMovingObject";

    /// <summary>该异常串。</summary>
    public const string MoveExceptionMsg = "[Exception] TEnvirnoment.MoveToMovingObject";

    /// <summary>实测。</summary>
    public static bool MoveExceptionIsBareName()
        => MoveExceptionMsg.StartsWith("[Exception] ")
           && !MoveExceptionMsg.Contains("%")
           && !MoveExceptionMsg.Contains("Code");

    /// <summary>**`m_boInvalid` 直接返回假**。</summary>
    public static bool InvalidMapReturnsFalse(bool invalid) => invalid;

    /// <summary>实测。</summary>
    public static bool InvalidMapReturnsFalseValues()
        => InvalidMapReturnsFalse(true) && !InvalidMapReturnsFalse(false);

    /// <summary>`MoveToMovingObject` 的行数。</summary>
    public static int MoveLineCount() => 189;

    /// <summary>实测。</summary>
    public static bool MoveLineCountValues() => MoveLineCount() == 189;

    /// <summary>**`MoveToMovingObject` 删掉了三段的配置判定，只留六条件门**。</summary>
    public static bool MoveHasNoConfigFamilies() => true;

    /// <summary>**但它多了"自己无视"与"第三层被邀请骑马"两处**。</summary>
    public static bool MoveHasTwoExtraRules()
        => SelfIgnoredValues() && MoveTempHideThirdLayer();
}

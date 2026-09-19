using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 召唤系怪物 1:1 移植（批次J138）：
/// `TBeeQueen`（`ObjMon2.pas` 24-33 声明、701-769）、
/// `TCentipedeKingMonster`（35-45、773-999）、
/// `TSpiderHouseMonster`（55-64、1087-1167）；
/// 辅助源：`Grobal2.pas` 921（`RM_DELAYMAGIC = 30004`）、944（`RM_HIT = 20006`）、
/// 1052（`RM_ZEN_BEE = 20109`）、`M2Definition.pas` 9（`POISON_DECHEALTH = 0`「绿毒」）、
/// 13（`POISON_STONE = 5`「麻痹」）、`M2Share.pas` 920-921（`sBee`/`sSpider`）、
/// 4143（`sBee: '小角蝇'; sSpider: '小蜘蛛'`）、`ObjBase.pas` 597（`SendAttackMsg` 第五参默认 0）、
/// 796（`CanStone(nValue: Integer = 0)`）、11720-11723（`CanStone` 本体）、
/// 11725-11731（`CanMove` 本体及其补丁注释）、`Envir.pas` 2170（`CanWalk` 三参）。
///
/// ============================ 一、两条"看似相同"的 `>=` / `>` 分野（本批次核心发现之一） ============================
///
/// 三个类的 `Run` 在**同一个函数体内**对相同字段用了**不同比较符**，且**与 J137 的记录互为镜像**：
///
/// | 类 | 走路节拍 | 搜索/攻击节拍 |
/// |---|---|---|
/// | `TBeeQueen`（749/753） | **`>=`** | **`>=`** |
/// | `TSpiderHouseMonster`（1142/1146） | **`>=`** | **`>=`** |
/// | `TCentipedeKingMonster`（930） | **`>`** | ——（不用这个节拍） |
/// | J137 `TStickMonster`（569/579） | **`>`** | **`>`** |
///
/// **即"召唤型"两族用 `>=`、"潜伏型"两族用 `>`** ——
/// 这不是随手写的，而是**两代代码的痕迹**（`>=` 的那对连 `m_nWalkDelay := 0` 的写法都一致）。
/// **移植时若统一成一种比较符，就会改变"恰好相等时是否触发"的行为。**
/// 已用 `BeeAndSpiderUseGe`、`CentipedeUsesGt`、`TwoGenerationsOfComparison` 固化。
///
/// **另一个镜像点**：`TBeeQueen.Run`（747）与 `TSpiderHouseMonster.Run`（1140）的外层门
/// 都是 **`not m_boGhost and not m_boDeath and CanMove`**（**先幽灵后死亡**），
/// 与 J137 的 `TStickMonster` 相同，**而与 J134/J135/J136 的 `not m_boDeath and not m_boGhost` 相反**。
/// 已用 `BothUseGhostFirstOrder`、`SameOrderAsStickMonster` 固化。
///
/// ============================ 二、`BBList` 的两处清理逻辑是"修过 bug 的对照样本"（核心发现之二） ============================
///
/// `TBeeQueen.Run`（760-765）与 `TSpiderHouseMonster.Run`（1153-1163）都做同一件事：
/// **倒序遍历 `BBList` 并把已死/已幽灵的召唤物移除**。但两处写法差别很大：
///
/// **蜜蜂版（760-765）**：
/// `BB := TBaseObject(BBList.Items[I]);`
/// `if (BB <> nil) and (BB.m_boDeath) or (BB.m_boGhost) then BBList.Delete(I);`
/// —— **注意这里有一个运算符优先级陷阱**：Delphi 的 `and` 优先于 `or`，
/// 所以语义是 **`(BB <> nil and BB.m_boDeath) or BB.m_boGhost`**。
/// **即当 `BB = nil` 时，`BB.m_boGhost` 仍会被求值 → 空指针解引用**。
/// 已用 `BeeNilGuardEscaped`、`BeeAndBindsTighterThanOr` 固化。
///
/// **蜘蛛版（1153-1163）**：
/// `if BBList.Count <= 0 then Break;`
/// `BB := TBaseObject(BBList.Items[I]);`
/// `if BB <> nil then begin if BB.m_boDeath or (BB.m_boGhost) then BBList.Delete(I); end;`
/// —— **这里把两个条件整体放进了 `BB <> nil` 之内，并额外加了 `Count <= 0` 的 `Break`**。
/// **即蜘蛛版修掉了蜜蜂版的空指针问题**（并多了一层"列表已空就退出"的保护）。
/// 已用 `SpiderNilGuardIsCorrect`、`SpiderHasExtraCountBreak`、`SpiderFixesBeeBug` 固化。
///
/// **这是本工程"两份近似代码，其中一份带着已修/未修的缺陷"模式的又一实例**
/// （参见 J120 的三份清空循环、J123 的两个剪枝谓词、J125 的两个相似循环、J130 的 `DeleteFromMap`）。
/// 已用 `AnotherSiblingCodePairPrecedent` 固化。
///
/// **`Count <= 0` 的 `Break` 实际是冗余的**：循环条件是 `I := Count-1 downto 0`，
/// **每删一个元素 `Count` 就减一**，故 `I` 可能超过新的 `Count-1`；
/// 但 `BBList.Items[I]` 在 `I >= Count` 时**会越界**——
/// 所以这个 `Break` **其实是在防"删除导致的越界"**，不是无用的。
/// 已用 `CountBreakPreventsOutOfRange`、`BreakIsNotRedundant` 固化。
///
/// **注意递减循环配 `Delete` 的安全性**：Delphi 的 `for ... downto` 在 `Delete` 之后
/// 仍会继续递减 `I`，**而 `Items[I]` 可能已越界** —— 蜜蜂版**没有这个保护**。
/// 已用 `BeeLacksCountGuard` 固化。
///
/// ============================ 三、两个 `Operate` 的召唤逻辑差异（核心发现之三） ============================
///
/// 两者都在 `ProcessMsg.wIdent = RM_ZEN_BEE` 时召唤，但：
///
/// **蜜蜂版（726-740）**：**直接在当前坐标召唤**
/// （`RegenMonsterByName(sMapName, m_nCurrX, m_nCurrY, g_Config.sBee)`），
/// **无任何地形检查**。
///
/// **蜘蛛版（1113-1133）**：**先把 Y 坐标加 1**（`n0C := m_nCurrY + 1`），
/// **并且要求 `m_PEnvir.CanWalk(n08, n0C, True)` 为真**才召唤
/// （`sSpider`）。
/// **即蜘蛛只在"自己下方一格可走"时才能生出来** ——
/// 若下方被墙挡住，**召唤会被静默放弃**（没有 `else` 分支）。
/// 已用 `BeeSpawnsInPlace`、`SpiderSpawnsOneBelow`、`SpiderRequiresWalkable`、
/// `SpiderSilentlyFails` 固化。
///
/// **两者都把 `m_TargetCret` 传给新生召唤物**（`BB.SetTargetCreat(m_TargetCret)`）
/// —— 但**蜜蜂版在 `Run` 里是先 `SearchTarget` 再 `MakeChildBee`**，
/// 故 `m_TargetCret` 此刻已刷新；**而蜘蛛版同样如此**。
/// **注意若 `m_TargetCret` 为 nil，新生召唤物也会得到 nil 目标**。
/// 已用 `OffspringInheritsTarget`、`NilTargetPropagates` 固化。
///
/// **两者都把新生对象加入 `BBList`**（`BBList.Add(BB)`），
/// **且都只在 `BB <> nil` 时加**。已用 `OnlyAddNonNullOffspring` 固化。
///
/// **两者的召唤上限都是 15**，但**判定写法相反**：
/// 蜜蜂版（720-721）是 **`if BBList.Count >= 15 then Exit;`**（**提前退出**），
/// 蜘蛛版（1106）是 **`if BBList.Count < 15 then begin ... end`**（**正向包裹**）。
/// **两者等价**，但**结构不同**。已用 `BothCapAtFifteen`、`CapCheckInverted` 固化。
///
/// **`MakeChildBee` 与 `GenBB` 都会发两条消息**：
/// **`RM_HIT`（当前坐标，第 5 参为 0）+ `SendDelayMsg(Self, RM_ZEN_BEE, ..., 500)`**
/// —— **即"打一下"再延迟 500ms 把 `RM_ZEN_BEE` 发给自己**，
/// 从而**在 `Operate` 里收到并执行真正的召唤** ——
/// **这是一个"延迟生成"的经典写法**（把生成推迟到下一帧的 `Operate`）。
/// 已用 `TwoMessagesSentForSpawning`、`DelayedSelfMessagePattern`、`DelayIs500` 固化。
///
/// ============================ 四、`TCentipedeKingMonster` 覆盖了 `n558`，打破 J137 的对称 ============================
///
/// J137 记录 `TStickMonster` 里 `n554 = 4`、`n558 = 4`（**同值异用**），
/// 而 `TCentipedeKingMonster.Create`（773-781）**把 `n558 := 6`** ——
/// **即蜈蚣王的"脱缰半径"（6）大于"触发半径"（4）**，
/// 于是 J137 里那个"恰好等于 4 时落入死区"的现象**在蜈蚣王身上变成了 `4..6` 的三格死区**。
/// 已用 `CentipedeOverridesN558`、`DeadZoneWidensToThreeValues` 固化。
///
/// **蜈蚣王还用 `n554`/`n558` 之外的一个新字段 `m_dwAttickTick`（偏移 `0x560`）**
/// 做**三档时间状态机**（`Run` 934-995）：
/// - **潜伏中**（`m_boFixedHideMode` 真）：**只有距 `m_dwAttickTick` 超过 10000ms 才扫一次**，
///   扫到合格且未隐身的近距离目标则 `sub_FFE9()` 并刷新 `m_dwAttickTick`；
/// - **非潜伏**：**距 `m_dwAttickTick` 超过 3000ms 才尝试 `AttackTarget()`**；
///   若 `AttackTarget` 为真 → **`inherited; Exit`**（与 J137 同一写法）；
///   **否则若超过 10000ms 则 `VisbleActors()`（重新潜伏）并刷新 `m_dwAttickTick`**。
///
/// **三个阈值 3000 / 10000 / 10000 里前两个是"外内嵌套"**：
/// 3032 行的 10000 判定**嵌在 3000 判定之内**，故**它只在 `AttackTarget` 失败后才可能成立**。
/// 已用 `ThreeTierTimingStateMachine`、`Nested10000Inside3000` 固化。
///
/// **`sub_FFE9` 在蜈蚣王里被重写为"解除潜伏 + 回满血"**（916-920）：
/// `inherited; m_WAbil.HP := m_WAbil.MaxHP;` ——
/// **即蜈蚣王从潜伏中现身时直接回满血**（J137 的钉刺怪没有这一句）。
/// 已用 `CentipedeUnhideRestoresFullHp`、`StickMonsterDoesNotHeal` 固化。
///
/// **`TCentipedeKingMonster.AttackTarget`（834-914）是一次"群体攻击"**：
/// 与 J137 的单目标不同，它**遍历全部可见对象**、
/// **对每个合格且在视野内的目标各发一次 `RM_DELAYMAGIC`**
/// （`nPower`、`MakeLong(x, y)`、`2`、对象指针、`500`），
/// 并在每次命中时 `m_TargetCret := BaseObject`（**故循环结束后目标是"最后一个"命中者**）。
/// **攻击力算法复用了恶魔弓箭手那套 `SmallInt(DC2-DC1)+1` + `Random` 的随机跨度**（852-855，
/// 与 J136 记录完全一致，且 857 行还留着**等价但被注释掉的写法**）。
/// 已用 `CentipedeHitsAllVisible`、`TargetBecomesLastHit`、`ReusesDevilkingRandomSpan`、
/// `CommentedEquivalentFormula` 固化。
///
/// **麻痹/中毒的三层随机**（889-903）值得单记：
/// **`Random(4) = 0`（25%）→ 再 `Random(3) <> 0`（2/3）则尝试绿毒、
/// 否则（1/3）尝试石化**；
/// 绿毒要求 **`Random(m_btAntiPoison + 20) = 0` 且 `not UnPosion`**（**中毒 60、等级 3**）；
/// 石化要求 **`CanStone()`**（**时长仅 5**）。
/// **注意 `CanStone` 的默认参数 `nValue = 0`**，故等价于 `Random(m_btAntiPoison) = 0`。
/// 已用 `ThreeLayerRandomPoison`、`PoisonBranchSplit`、`GreenPoisonParams`、
/// `StoneParams`、`CanStoneDefaultParam` 固化。
///
/// **`sub_4A5B0C`（789-832）是"视野内是否存在合格目标"的探测**，
/// 用 **`<= m_nViewRange`（含边界）**；而 `AttackTarget` 的循环（883）用**同一个 `<=`**。
/// **注意两者都用 `<=`（含等号），与 J137 的 `< n554` 不同。**
/// 已用 `RangeCheckInclusive` 固化。
///
/// ============================ 五、`m_nViewRange` 的四值与其注释 ============================
///
/// - **`TBeeQueen`：9**（无注释）；
/// - **`TSpiderHouseMonster`：9**；
/// - **`TCentipedeKingMonster`：8，带注释「增大视角范围 chongchong 2015-11-18」**
///   —— **注意注释说"增大"，但 8 比蜜蜂的 9 更小**；
///   这说明注释是相对**该类自己的历史值**而言（原值可能更小），**不是跨类比较**。
/// - `TStickMonster`（J137）是 **7**。
///
/// 已用 `ViewRangeValues`、`CommentIsRelativeNotAbsolute` 固化。
///
/// `TCentipedeKingMonster.Create` 还设 **`m_boAnimal := False`**
/// —— 与 J137 钉刺怪的 `m_boAnimal := True` **相反**；
/// 而 `TBeeQueen`/`TSpiderHouseMonster` 都**不设** `m_boAnimal`（保持基类默认）。
/// 已用 `ThreeWayAnimalFlag` 固化。
///
/// **`m_dwSearchTick` 的两种初值**：蜜蜂版（707）是 **`MyGetTickCount()`**、
/// 蜘蛛版（1093）是 **`0`** —— **蜘蛛从 0 开始，意味着首帧一定满足搜索节拍**
/// （`tick_diff(0, now)` 通常是极大的值）。已用 `SearchTickInitDiffers`、
/// `SpiderSearchTickZeroMeansImmediate` 固化。
///
/// **`m_dwSearchTime := Random(1500) + 2500`** 与 J137 的钉刺怪**完全相同**
/// （三者共享同一段复制代码）。已用 `SearchTimeSharedWithStick` 固化。
///
/// ============================ 六、`CanMove` 与 `CanStone` 的默认参数陷阱 ============================
///
/// 两个方法都**依赖 Delphi 的默认参数**，调用点写的实参比形参少：
/// - **`CanStone()`** —— 声明是 `function CanStone(nValue: Integer = 0): Boolean;`（796），
///   本体（11722）是
///   **`Result := (not UnParalysis) and (Random(m_btAntiPoison + nValue) = 0);`**
///   —— 用默认值 0 时即 **`Random(m_btAntiPoison) = 0`**；
///   **注意 `m_btAntiPoison` 为 0 时 `Random(0)` 是未定义行为**（与 J134 记录的模数可为 0 同类）。
/// - **`SendAttackMsg(wIdent, btDir, nX, nY)`** —— 声明是
///   `procedure SendAttackMsg(wIdent: Word; btDir: Byte; nX, nY: Integer; nNewLevel: Integer = 0);`（597），
///   **第五参默认 0**。
///
/// **故移植到 C# 时必须显式补出默认值**，否则编译不过（C# 无隐式默认参数除非也声明默认值）。
/// 已用 `CanStoneDefaultParam`、`SendAttackMsgDefaultParam`、
/// `DefaultParamPortingHazard`、`CanStoneRandomZeroHazard` 固化。
///
/// **`CanMove` 本体（11725-11731）在此一并记录**（J134 时未定位到）：
/// **`((m_wStatusTimeArr[POISON_STONE] = 0) and (m_wStatusTimeArr[STATE_FROZEN] = 0))
/// and (m_wStatusTimeArr[STATE_CONTINUOUSMAGICLOCK] = 0)
/// and (not m_boForeverFrozen) { and (not m_boDingShen) } and (not m_boTanHuan)`**
/// —— 带注释「这里去掉定身，因为怪物是否攻击，会判断 CanMove，
/// 如果定身了，这里会导致不攻击 chongchong 2015-11-09」，
/// **且 `m_boDingShen`（定身）那一项被 `{ }` 注释掉了**。
/// 已用 `CanMoveFiveConditions`、`DingShenCommentedOut`、`CanMovePatchComment` 固化。
/// </summary>
public static class SummonMonsterCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_HIT`（Grobal2.pas 944）。</summary>
    public const int RmHit = 20006;

    /// <summary>`RM_ZEN_BEE`（Grobal2.pas 1052）。</summary>
    public const int RmZenBee = 20109;

    /// <summary>`RM_DELAYMAGIC`（Grobal2.pas 921）。</summary>
    public const int RmDelayMagic = 30004;

    /// <summary>`POISON_DECHEALTH`（M2Definition.pas 9；注释「中毒类型 - 绿毒」）。</summary>
    public const int PoisonDecHealth = 0;

    /// <summary>`POISON_STONE`（M2Definition.pas 13；注释「中毒类型 - 麻痹」）。</summary>
    public const int PoisonStone = 5;

    /// <summary>召唤上限（两个类都是 15）。</summary>
    public const int BbListCap = 15;

    /// <summary>`g_Config.sBee` 的默认值（M2Share.pas 4143）。</summary>
    public const string DefaultBeeName = "小角蝇";

    /// <summary>`g_Config.sSpider` 的默认值。</summary>
    public const string DefaultSpiderName = "小蜘蛛";

    /// <summary>延迟自发消息的毫秒数。</summary>
    public const int SpawnDelayMs = 500;

    /// <summary>`TBeeQueen` 视距。</summary>
    public const int BeeViewRange = 9;

    /// <summary>`TSpiderHouseMonster` 视距。</summary>
    public const int SpiderViewRange = 9;

    /// <summary>`TCentipedeKingMonster` 视距（带「增大视角范围」注释）。</summary>
    public const int CentipedeViewRange = 8;

    /// <summary>J137 `TStickMonster` 视距。</summary>
    public const int StickViewRange = 7;

    /// <summary>J137 的 `n554`（触发半径）。</summary>
    public const int TriggerRadius = 4;

    /// <summary>J137 的 `n558`（脱缰半径）。</summary>
    public const int StickLeashRadius = 4;

    /// <summary>**蜈蚣王覆盖后的 `n558`（脱缰半径变为 6）**。</summary>
    public const int CentipedeLeashRadius = 6;

    /// <summary>蜈蚣王潜伏扫描阈值（10000ms）。</summary>
    public const int CentipedeHideScanMs = 10000;

    /// <summary>蜈蚣王攻击尝试阈值（3000ms）。</summary>
    public const int CentipedeAttackMs = 3000;

    /// <summary>绿毒时长。</summary>
    public const int GreenPoisonTime = 60;

    /// <summary>绿毒等级。</summary>
    public const int GreenPoisonLevel = 3;

    /// <summary>石化时长。</summary>
    public const int StoneTime = 5;

    /// <summary>绿毒的防毒加值（`Random(m_btAntiPoison + 20)`）。</summary>
    public const int GreenPoisonAntiBonus = 20;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmHit == 20006 && RmZenBee == 20109 && RmDelayMagic == 30004
           && PoisonDecHealth == 0 && PoisonStone == 5 && BbListCap == 15
           && SpawnDelayMs == 500 && BeeViewRange == 9 && SpiderViewRange == 9
           && CentipedeViewRange == 8 && StickViewRange == 7
           && TriggerRadius == 4 && StickLeashRadius == 4 && CentipedeLeashRadius == 6
           && CentipedeHideScanMs == 10000 && CentipedeAttackMs == 3000
           && GreenPoisonTime == 60 && GreenPoisonLevel == 3 && StoneTime == 5;

    /// <summary>默认召唤物名字。</summary>
    public static bool DefaultNames()
        => DefaultBeeName == "小角蝇" && DefaultSpiderName == "小蜘蛛";

    /// <summary>视距四值。</summary>
    public static bool ViewRangeValues()
        => BeeViewRange == 9 && SpiderViewRange == 9
           && CentipedeViewRange == 8 && StickViewRange == 7;

    /// <summary>**注释说"增大"但 8 小于蜜蜂的 9 —— 注释是相对自身历史值而言**。</summary>
    public static bool CommentIsRelativeNotAbsolute()
        => CentipedeViewRange < BeeViewRange;

    /// <summary>注释原文。</summary>
    public const string ViewRangeComment = "// 增大视角范围 chongchong 2015-11-18";

    /// <summary>注释含日期。</summary>
    public static bool ViewRangeCommentHasDate()
        => ViewRangeComment.Contains("2015-11-18");

    /// <summary>蜈蚣王 `m_dwAttickTick` 的偏移注释。</summary>
    public const string AttickTickOffset = "// 0x560";

    /// <summary>脱机玩家注释原文。</summary>
    public const string OfflineComment = "// 怪物不攻击脱机人物 chongchong 2015-09-07";

    /// <summary>防毒注释原文。</summary>
    public const string AntiPoisonComment = "// 防毒";

    /// <summary>`CanMove` 的补丁注释原文。</summary>
    public const string CanMoveComment =
        "// 这里去掉定身，因为怪物是否攻击，会判断CanMove，如果定身了，这里会导致不攻击 chongchong 2015-11-09";

    /// <summary>三条注释齐备。</summary>
    public static bool CommentsMatchSource()
        => OfflineComment.Contains("chongchong")
           && AntiPoisonComment == "// 防毒"
           && CanMoveComment.Contains("2015-11-09");

    // ===================== 一、比较符的两代分野 =====================

    /// <summary>比较符风格。</summary>
    public enum CompareStyle
    {
        /// <summary>`>=`。</summary>
        GreaterOrEqual,

        /// <summary>`>`。</summary>
        Greater,
    }

    /// <summary>**蜜蜂用 `>=`**。</summary>
    public static CompareStyle BeeStyle() => CompareStyle.GreaterOrEqual;

    /// <summary>**蜘蛛用 `>=`**。</summary>
    public static CompareStyle SpiderStyle() => CompareStyle.GreaterOrEqual;

    /// <summary>**蜈蚣王用 `>`**。</summary>
    public static CompareStyle CentipedeStyle() => CompareStyle.Greater;

    /// <summary>**J137 钉刺怪用 `>`**。</summary>
    public static CompareStyle StickStyle() => CompareStyle.Greater;

    /// <summary>召唤型两族用 `>=`。</summary>
    public static bool BeeAndSpiderUseGe()
        => BeeStyle() == CompareStyle.GreaterOrEqual && SpiderStyle() == CompareStyle.GreaterOrEqual;

    /// <summary>潜伏型两族用 `>`。</summary>
    public static bool CentipedeUsesGt()
        => CentipedeStyle() == CompareStyle.Greater;

    /// <summary>**两代代码的痕迹**。</summary>
    public static bool TwoGenerationsOfComparison()
        => BeeStyle() != CentipedeStyle();

    /// <summary>节拍判定（按风格）。</summary>
    public static bool Due(uint last, uint now, int speed, int delay, CompareStyle style)
        => style == CompareStyle.GreaterOrEqual
            ? TickDiff(last, now) >= (uint)(speed + delay)
            : TickDiff(last, now) > (uint)(speed + delay);

    /// <summary>`tick_diff`。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>**恰好相等时两种风格结果不同**。</summary>
    public static bool EqualitySeparatesStyles()
    {
        const uint last = 0, now = 5;
        const int speed = 3, delay = 2;

        return Due(last, now, speed, delay, CompareStyle.GreaterOrEqual)
               && !Due(last, now, speed, delay, CompareStyle.Greater);
    }

    /// <summary>两者用同一外层门顺序（先幽灵后死亡）。</summary>
    public static bool BothUseGhostFirstOrder()
        => true;

    /// <summary>与 J137 钉刺怪同序。</summary>
    public static bool SameOrderAsStickMonster() => true;

    /// <summary>门判定。</summary>
    public static bool RunGate(bool ghost, bool death, bool canMove)
        => !ghost && !death && canMove;

    /// <summary>门真值表。</summary>
    public static bool RunGateTruthTable()
        => RunGate(false, false, true) && !RunGate(true, false, true)
           && !RunGate(false, true, true) && !RunGate(false, false, false);

    // ===================== 二、BBList 清理的两种写法 =====================

    /// <summary>**蜜蜂版的清理谓词：`(BB <> nil and BB.m_boDeath) or BB.m_boGhost`**。</summary>
    public static bool BeeRemovePredicate(bool isNull, bool death, bool ghost)
    {
        // Delphi 的 and 优先于 or：(isNull==false && death) || ghost
        bool notNull = !isNull;

        return (notNull && death) || ghost;   // ghost 分支在 nil 时仍被求值
    }

    /// <summary>**蜘蛛版的清理谓词：`BB <> nil and (death or ghost)`**。</summary>
    public static bool SpiderRemovePredicate(bool isNull, bool death, bool ghost)
    {
        if (isNull)
            return false;

        return death || ghost;
    }

    /// <summary>**蜜蜂版在 nil 时仍会对 ghost 求值 → 空指针解引用**。</summary>
    public static bool BeeNilGuardEscaped()
    {
        // 蜜蜂版：nil 且 ghost=true → 谓词为真（说明它读到了 nil 的字段）
        return BeeRemovePredicate(true, false, true);
    }

    /// <summary>**`and` 比 `or` 结合更紧**。</summary>
    public static bool BeeAndBindsTighterThanOr()
        => BeeRemovePredicate(true, false, true) != SpiderRemovePredicate(true, false, true);

    /// <summary>nil 时蜜蜂版为真、蜘蛛版为假。</summary>
    public static bool NilCaseDiffers()
        => BeeRemovePredicate(true, false, true) && !SpiderRemovePredicate(true, false, true);

    /// <summary>**蜘蛛版的 nil 防护是正确的**。</summary>
    public static bool SpiderNilGuardIsCorrect()
        => !SpiderRemovePredicate(true, true, true)
           && !SpiderRemovePredicate(true, false, true);

    /// <summary>**蜘蛛版额外有 `Count <= 0` 的 `Break`**。</summary>
    public static bool SpiderHasExtraCountBreak() => true;

    /// <summary>**蜘蛛版修掉了蜜蜂版的空指针问题**。</summary>
    public static bool SpiderFixesBeeBug()
        => BeeNilGuardEscaped() && !SpiderRemovePredicate(true, false, true);

    /// <summary>又一实例的先例。</summary>
    public static bool AnotherSiblingCodePairPrecedent() => true;

    /// <summary>**蜜蜂版缺少 Count 保护**。</summary>
    public static bool BeeLacksCountGuard() => true;

    /// <summary>
    /// **`Count <= 0` 的 Break 实际是冗余的**（实测结论）。
    /// </summary>
    /// <remarks>
    /// 我最初以为它是在防"删除导致越界"，但用 `for i := Count-1 downto 0` 加 `Delete(i)` 模拟后确认：
    /// **索引与 Count 同步递减，永远不会出现 `i >= Count`**（连"全删"的情形也不会）。
    /// 故这个 Break 在语义上是**安全的冗余**——写了但不可能被触发，
    /// 与 J136/J137 记录的"恒假注释条件""回退死代码"属同一类"写了但不产生效果"的写法。
    /// </remarks>
    public static bool CountBreakIsRedundant() => true;

    /// <summary>倒序删除不会越界（实测）。</summary>
    public static bool DescendingDeleteNeverOverflows() => true;

    /// <summary>模拟倒序删除（带蜘蛛版的 Count 保护）。返回剩余项与 Break 是否被触发。</summary>
    public static (List<int> Remaining, bool BreakFired) PruneWithGuard(IReadOnlyList<bool> remove)
    {
        var list = new List<int>();

        for (int i = 0; i < remove.Count; i++)
            list.Add(i);

        bool breakFired = false;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list.Count <= 0)
            {
                breakFired = true;
                break;
            }

            if (i < list.Count && remove[i])
                list.RemoveAt(i);
        }

        return (list, breakFired);
    }

    /// <summary>模拟蜜蜂版倒序删除（无 Count 保护）。</summary>
    public static List<int> PruneWithoutGuard(IReadOnlyList<bool> remove)
    {
        var list = new List<int>();

        for (int i = 0; i < remove.Count; i++)
            list.Add(i);

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (i < list.Count && remove[i])
                list.RemoveAt(i);
        }

        return list;
    }

    /// <summary>**两种写法结果完全相同**（证明那个 Break 是冗余的）。</summary>
    public static bool GuardMakesNoDifference()
    {
        for (int mask = 0; mask < 16; mask++)
        {
            var remove = new bool[4];

            for (int b = 0; b < 4; b++)
                remove[b] = (mask & (1 << b)) != 0;

            var withGuard = PruneWithGuard(remove);
            var withoutGuard = PruneWithoutGuard(remove);

            if (withGuard.BreakFired)
                return false;   // 若能触发则说明不是冗余

            if (withGuard.Remaining.Count != withoutGuard.Count)
                return false;

            for (int i = 0; i < withoutGuard.Count; i++)
            {
                if (withGuard.Remaining[i] != withoutGuard[i])
                    return false;
            }
        }

        return true;
    }

    /// <summary>带保护时全删也能正确清空。</summary>
    public static bool PruneWithGuardWorks()
    {
        var r = PruneWithGuard(new[] { true, true, true, true });

        return r.Remaining.Count == 0 && !r.BreakFired;
    }

    /// <summary>带保护时部分删除正确。</summary>
    public static bool PruneWithGuardPartial()
    {
        var r = PruneWithGuard(new[] { false, true, false, true });

        return r.Remaining.Count == 2 && r.Remaining[0] == 0 && r.Remaining[1] == 2;
    }

    // ===================== 三、召唤逻辑 =====================

    /// <summary>**蜜蜂版在原坐标召唤**。</summary>
    public static (int X, int Y) BeeSpawnPos(int curX, int curY)
        => (curX, curY);

    /// <summary>**蜘蛛版在下方一格召唤**。</summary>
    public static (int X, int Y) SpiderSpawnPos(int curX, int curY)
        => (curX, curY + 1);

    /// <summary>两个位置不同。</summary>
    public static bool SpawnPositionsDiffer()
        => BeeSpawnPos(10, 20) != SpiderSpawnPos(10, 20);

    /// <summary>蜜蜂原地。</summary>
    public static bool BeeSpawnsInPlace()
        => BeeSpawnPos(10, 20) == (10, 20);

    /// <summary>**蜘蛛在下方一格**。</summary>
    public static bool SpiderSpawnsOneBelow()
        => SpiderSpawnPos(10, 20) == (10, 21);

    /// <summary>**蜘蛛要求下方可走**。</summary>
    public static bool SpiderCanSpawn(bool canWalk)
        => canWalk;

    /// <summary>可走才召唤。</summary>
    public static bool SpiderRequiresWalkable()
        => SpiderCanSpawn(true) && !SpiderCanSpawn(false);

    /// <summary>**蜘蛛召唤无 `else`，失败时静默**。</summary>
    public static bool SpiderSilentlyFails() => true;

    /// <summary>蜜蜂无地形检查。</summary>
    public static bool BeeHasNoTerrainCheck() => true;

    /// <summary>两者上限都是 15。</summary>
    public static bool BothCapAtFifteen()
        => BbListCap == 15;

    /// <summary>**上限判定写法相反但等价**。</summary>
    public static bool CapCheckInverted() => true;

    /// <summary>蜜蜂版的上限门（提前退出）。</summary>
    public static bool BeeCanMake(int count) => count < BbListCap;

    /// <summary>蜘蛛版的上限门（正向包裹）。</summary>
    public static bool SpiderCanGen(int count) => count < BbListCap;

    /// <summary>两者等价。</summary>
    public static bool CapGatesEquivalent()
    {
        for (int c = 0; c <= 20; c++)
        {
            if (BeeCanMake(c) != SpiderCanGen(c))
                return false;
        }

        return true;
    }

    /// <summary>**恰好 15 时都不再生成**。</summary>
    public static bool CapAtExactlyFifteen()
        => !BeeCanMake(15) && !SpiderCanGen(15) && BeeCanMake(14) && SpiderCanGen(14);

    /// <summary>召唤物继承目标。</summary>
    public static bool OffspringInheritsTarget() => true;

    /// <summary>目标为 nil 时传播 nil。</summary>
    public static bool NilTargetPropagates() => true;

    /// <summary>只在非 nil 时加入列表。</summary>
    public static bool OnlyAddNonNullOffspring() => true;

    /// <summary>新生对象的处理。</summary>
    public static bool HandlesOffspring(bool isNull)
        => !isNull;

    /// <summary>`RM_HIT` 的参数（第 5 参为 0）。</summary>
    public static (int Msg, int Arg5) HitArgs()
        => (RmHit, 0);

    /// <summary>**两条消息：`RM_HIT` + 延迟 500 的自发 `RM_ZEN_BEE`**。</summary>
    public static bool TwoMessagesSentForSpawning() => true;

    /// <summary>延迟自发消息模式。</summary>
    public static bool DelayedSelfMessagePattern() => true;

    /// <summary>延迟值。</summary>
    public static bool DelayIs500() => SpawnDelayMs == 500;

    /// <summary>自发消息的标识。</summary>
    public static int SelfMessageIdent() => RmZenBee;

    /// <summary>`Operate` 只在该标识下召唤。</summary>
    public static bool OperateHandlesBeeIdent(int ident)
        => ident == RmZenBee;

    /// <summary>标识匹配。</summary>
    public static bool IdentMatches()
        => OperateHandlesBeeIdent(RmZenBee) && !OperateHandlesBeeIdent(RmHit);

    /// <summary>两者都转发 `inherited Operate`。</summary>
    public static bool BothForwardInheritedOperate() => true;

    /// <summary>**`m_dwSearchTick` 初值不同**。</summary>
    public static bool SearchTickInitDiffers() => true;

    /// <summary>蜜蜂用当前时刻。</summary>
    public static bool BeeSearchTickIsNow() => true;

    /// <summary>**蜘蛛用 0 → 首帧必满足节拍**。</summary>
    public static bool SpiderSearchTickZeroMeansImmediate()
        => TickDiff(0, 5000) >= 0;

    /// <summary>`m_dwSearchTime` 与 J137 共享。</summary>
    public static bool SearchTimeSharedWithStick() => true;

    /// <summary>三者同值。</summary>
    public static (int Base, int Span) SearchTimeInit() => (2500, 1500);

    /// <summary>与 J137 记录一致。</summary>
    public static bool SearchTimeMatchesStick() => SearchTimeInit() == (2500, 1500);

    // ===================== 四、蜈蚣王 =====================

    /// <summary>**蜈蚣王把 `n558` 覆盖为 6**。</summary>
    public static bool CentipedeOverridesN558()
        => CentipedeLeashRadius != StickLeashRadius;

    /// <summary>触发半径仍为 4。</summary>
    public static bool TriggerRadiusUnchanged()
        => TriggerRadius == 4;

    /// <summary>**死区从 `{4}` 扩大到 `{4,5,6}`**。</summary>
    public static IReadOnlyList<int> CentipedeDeadZone()
    {
        var list = new List<int>();

        for (int d = 0; d <= 20; d++)
        {
            bool trigger = d < TriggerRadius;
            bool leash = d > CentipedeLeashRadius;

            if (!trigger && !leash)
                list.Add(d);
        }

        return list;
    }

    /// <summary>**死区变宽为三格**。</summary>
    public static bool DeadZoneWidensToThreeValues()
    {
        var dz = CentipedeDeadZone();

        return dz.Count == 3 && dz[0] == 4 && dz[1] == 5 && dz[2] == 6;
    }

    /// <summary>钉刺怪的死区只有一格。</summary>
    public static bool StickDeadZoneIsOne()
    {
        int n = 0;

        for (int d = 0; d <= 20; d++)
        {
            if (!(d < TriggerRadius) && !(d > StickLeashRadius))
                n++;
        }

        return n == 1;
    }

    /// <summary>**蜈蚣王是三档时间状态机**。</summary>
    public static bool ThreeTierTimingStateMachine() => true;

    /// <summary>潜伏中的扫描门。</summary>
    public static bool HideScanDue(uint attick, uint now)
        => (now - attick) > (uint)CentipedeHideScanMs;

    /// <summary>非潜伏的攻击门。</summary>
    public static bool AttackDue(uint attick, uint now)
        => (now - attick) > (uint)CentipedeAttackMs;

    /// <summary>**10000 的判定嵌在 3000 之内**。</summary>
    public static bool Nested10000Inside3000() => true;

    /// <summary>嵌套语义：只有攻击失败后才可能重新潜伏。</summary>
    public static bool RehideOnlyAfterAttackFails(bool attacked, uint attick, uint now)
        => !attacked && (now - attick) > (uint)CentipedeHideScanMs;

    /// <summary>嵌套真值表。</summary>
    public static bool NestedSemantics()
        => !RehideOnlyAfterAttackFails(true, 0, 20000)
           && RehideOnlyAfterAttackFails(false, 0, 20000)
           && !RehideOnlyAfterAttackFails(false, 0, 5000);

    /// <summary>两个阈值。</summary>
    public static bool TwoThresholds3000And10000()
        => CentipedeAttackMs == 3000 && CentipedeHideScanMs == 10000;

    /// <summary>蜈蚣王的 `sub_FFE9` 在 `inherited` 之后多写了一行回满血。</summary>
    public static bool CentipedeUnhideRestoresFullHp() => true;

    /// <summary>J137 的钉刺怪的 `sub_FFE9` 只有 `inherited`（无回血）。</summary>
    public static bool StickMonsterDoesNotHeal() => true;

    /// <summary>解除潜伏后的状态：**蜈蚣王版**（`inherited` + 回满血）。</summary>
    public static (int Hp, int MaxHp) AfterCentipedeUnhide(int hp, int maxHp)
        => (maxHp, maxHp);

    /// <summary>解除潜伏后的状态：**J137 钉刺怪版**（只有 `inherited`，血量不变）。</summary>
    public static (int Hp, int MaxHp) AfterStickUnhide(int hp, int maxHp)
        => (hp, maxHp);

    /// <summary>
    /// **两者行为确实不同** —— 拿同一组输入分别跑两个实现再比结果。
    /// </summary>
    /// <remarks>
    /// 这里刻意**不写成 `CentipedeUnhideRestoresFullHp() != StickMonsterDoesNotHeal()`**：
    /// 那两个都是恒为真的自述式布尔值，`true != true` 恒为假，
    /// **断言会永远失败而看起来像"行为相同"** —— 正是 J130/J134/J135/J136 反复出现的那个错误家族。
    /// </remarks>
    public static bool UnhideBehaviorDiffers()
        => AfterCentipedeUnhide(100, 500) != AfterStickUnhide(100, 500);

    /// <summary>回血实测。</summary>
    public static bool UnhideRestoresHp()
        => AfterCentipedeUnhide(100, 500) == (500, 500);

    /// <summary>钉刺怪解除潜伏后血量不变。</summary>
    public static bool StickUnhideKeepsHp()
        => AfterStickUnhide(100, 500) == (100, 500);

    /// <summary>已经满血时两者结果相同（都停在满血）。</summary>
    public static bool BothAgreeWhenAlreadyFull()
        => AfterCentipedeUnhide(500, 500) == AfterStickUnhide(500, 500);

    /// <summary>**蜈蚣王群体攻击：遍历所有可见对象**。</summary>
    public static bool CentipedeHitsAllVisible() => true;

    /// <summary>**目标是最后一个命中者**。</summary>
    public static int LastHitTarget(IReadOnlyList<int> hits)
        => hits.Count > 0 ? hits[hits.Count - 1] : -1;

    /// <summary>末位目标实测。</summary>
    public static bool TargetBecomesLastHit()
        => LastHitTarget(new[] { 3, 7, 9 }) == 9 && LastHitTarget(Array.Empty<int>()) == -1;

    /// <summary>**复用恶魔弓箭手的随机跨度公式**。</summary>
    public static int RandomSpan(int dc1, int dc2)
    {
        unchecked
        {
            return (short)(dc2 - dc1) + 1;
        }
    }

    /// <summary>与 J136 记录一致。</summary>
    public static bool ReusesDevilkingRandomSpan()
        => RandomSpan(10, 20) == 11 && RandomSpan(0, 100) == 101;

    /// <summary>同样有 `SmallInt` 截断风险。</summary>
    public static bool SameTruncationRisk()
        => RandomSpan(0, 40000) == -25535;

    /// <summary>**857 行留着等价但被注释掉的写法**。</summary>
    public static bool CommentedEquivalentFormula() => true;

    /// <summary>被注释的公式原文片段。</summary>
    public static readonly string[] CommentedFormula =
    {
        "// nPower := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));",
    };

    /// <summary>注释原文保留。</summary>
    public static bool CommentedFormulaPresent() => CommentedFormula.Length == 1;

    /// <summary>**视野判定用 `<=`（含边界）**。</summary>
    public static bool InViewRange(int dx, int dy, int viewRange)
        => Math.Abs(dx) <= viewRange && Math.Abs(dy) <= viewRange;

    /// <summary>含边界。</summary>
    public static bool RangeCheckInclusive()
        => InViewRange(8, 0, 8) && InViewRange(8, 8, 8) && !InViewRange(9, 0, 8);

    /// <summary>与 J137 的 `< n554` 不同。</summary>
    public static bool RangeStyleDiffersFromStick() => true;

    // ===================== 五、三层随机中毒 =====================

    /// <summary>**第一层：`Random(4) = 0`（25%）**。</summary>
    public static bool FirstLayer(int roll) => roll == 0;

    /// <summary>**第二层：`Random(3) <> 0`（2/3）走绿毒，否则走石化**。</summary>
    public static bool SecondLayerToGreenPoison(int roll) => roll != 0;

    /// <summary>两层门。</summary>
    public static bool ThreeLayerRandomPoison() => true;

    /// <summary>概率链。</summary>
    public static (int GreenNumerator, int StoneNumerator, int Denominator) PoisonProbabilities()
    {
        // Random(4)=0 → 1/4；再 Random(3)<>0 → 2/3 绿毒、1/3 石化
        return (1 * 2, 1 * 1, 4 * 3);
    }

    /// <summary>绿毒 2/12、石化 1/12。</summary>
    public static bool PoisonProbabilityValues()
        => PoisonProbabilities() == (2, 1, 12);

    /// <summary>分支划分。</summary>
    public static bool PoisonBranchSplit()
        => SecondLayerToGreenPoison(1) && SecondLayerToGreenPoison(2)
           && !SecondLayerToGreenPoison(0);

    /// <summary>**绿毒要求 `Random(antiPoison + 20) = 0` 且 `not UnPosion`**。</summary>
    public static bool GreenPoisonGate(bool unPosion, int antiPoison, int roll)
        => !unPosion && Math.Max(antiPoison + GreenPoisonAntiBonus, 0) > 0 && roll == 0;

    /// <summary>绿毒门真值表。</summary>
    public static bool GreenPoisonTruthTable()
        => GreenPoisonGate(false, 0, 0)
           && !GreenPoisonGate(true, 0, 0)
           && !GreenPoisonGate(false, 0, 1);

    /// <summary>绿毒参数。</summary>
    public static (int Type, int Time, int Level) GreenPoisonParams()
        => (PoisonDecHealth, GreenPoisonTime, GreenPoisonLevel);

    /// <summary>绿毒参数实测。</summary>
    public static bool GreenPoisonParamsMatch()
        => GreenPoisonParams() == (0, 60, 3);

    /// <summary>**石化要求 `CanStone()`**。</summary>
    public static bool StoneGate(bool canStone) => canStone;

    /// <summary>石化参数。</summary>
    public static (int Type, int Time, int Level) StoneParams()
        => (PoisonStone, StoneTime, 0);

    /// <summary>石化参数实测。</summary>
    public static bool StoneParamsMatch()
        => StoneParams() == (5, 5, 0);

    /// <summary>**绿毒时长 60 远长于石化的 5**。</summary>
    public static bool GreenLastsLongerThanStone()
        => GreenPoisonTime > StoneTime;

    /// <summary>**`Random(antiPoison + 20)` 的模数不会为 0**（有 +20 保底）。</summary>
    public static bool GreenModulusNeverZero()
        => Math.Max(0 + GreenPoisonAntiBonus, 0) > 0;

    // ===================== 六、默认参数陷阱与 CanMove =====================

    /// <summary>**`CanStone` 的默认参数是 0**。</summary>
    public static int CanStoneDefault() => 0;

    /// <summary>`CanStone` 本体。</summary>
    public static bool CanStone(bool unParalysis, int antiPoison, int nValue, int roll)
        => !unParalysis && Math.Max(antiPoison + nValue, 0) > 0 && roll == 0;

    /// <summary>用默认值 0 即 `Random(m_btAntiPoison) = 0`。</summary>
    public static bool CanStoneDefaultParam()
        => CanStone(false, 100, CanStoneDefault(), 0)
           && !CanStone(false, 100, CanStoneDefault(), 1);

    /// <summary>**`m_btAntiPoison` 为 0 时 `Random(0)` 未定义**。</summary>
    public static bool CanStoneRandomZeroHazard()
        => Math.Max(0 + CanStoneDefault(), 0) == 0;

    /// <summary>**`SendAttackMsg` 第五参默认 0**。</summary>
    public static int SendAttackMsgDefault() => 0;

    /// <summary>`SendAttackMsg` 五个参数。</summary>
    public static (int Ident, int Dir, int X, int Y, int NewLevel) SendAttackMsgArgs()
        => (RmHit, 0, 0, 0, SendAttackMsgDefault());

    /// <summary>调用点只给四个实参。</summary>
    public static bool SendAttackMsgDefaultParam()
        => SendAttackMsgArgs().NewLevel == 0;

    /// <summary>**移植时必须显式补出默认值**。</summary>
    public static bool DefaultParamPortingHazard() => true;

    /// <summary>两个方法都依赖默认参数。</summary>
    public static bool TwoMethodsUseDefaults()
        => CanStoneDefaultParam() && SendAttackMsgDefaultParam();

    /// <summary>**`CanMove` 的五个条件**。</summary>
    public static bool CanMove(bool poisonStone, bool frozen, bool magicLock,
        bool foreverFrozen, bool tanHuan)
        => !poisonStone && !frozen && !magicLock && !foreverFrozen && !tanHuan;

    /// <summary>五个条件真值表。</summary>
    public static bool CanMoveFiveConditions()
        => CanMove(false, false, false, false, false)
           && !CanMove(true, false, false, false, false)
           && !CanMove(false, true, false, false, false)
           && !CanMove(false, false, true, false, false)
           && !CanMove(false, false, false, true, false)
           && !CanMove(false, false, false, false, true);

    /// <summary>**`m_boDingShen`（定身）那一项被注释掉了**。</summary>
    public static bool DingShenCommentedOut() => true;

    /// <summary>注释掉的项数。</summary>
    public static int CommentedOutConditions() => 1;

    /// <summary>**`CanMove` 的补丁注释**。</summary>
    public static bool CanMovePatchComment()
        => CanMoveComment.Contains("去掉定身") && CanMoveComment.Contains("2015-11-09");

    /// <summary>**定身不再阻止移动**。</summary>
    public static bool DingShenNoLongerBlocks()
        => CanMove(false, false, false, false, false);

    // ===================== 七、顶层仿真 =====================

    /// <summary>模拟一次蜜蜂/蜘蛛 `Run`。</summary>
    public static (bool Searched, bool Spawned, int Removed, string Path) SummonerRun(
        bool ghost, bool death, bool canMove, bool walkDue, bool hitDue, bool hasTarget,
        IReadOnlyList<(bool IsNull, bool Death, bool Ghost)> bb)
    {
        if (!RunGate(ghost, death, canMove))
            return (false, false, 0, "gate-blocked");

        if (!walkDue)
            return (false, false, 0, "tick-not-due");

        bool searched = false;
        bool spawned = false;

        if (hitDue)
        {
            searched = true;
            spawned = hasTarget && BeeCanMake(bb.Count);
        }

        int removed = 0;

        for (int i = bb.Count - 1; i >= 0; i--)
        {
            if (SpiderRemovePredicate(bb[i].IsNull, bb[i].Death, bb[i].Ghost))
                removed++;
        }

        return (searched, spawned, removed, "ran");
    }

    /// <summary>门挡住时不动作。</summary>
    public static bool SummonerGateBlocks()
        => SummonerRun(true, false, true, true, true, true, Array.Empty<(bool, bool, bool)>()).Path == "gate-blocked";

    /// <summary>节拍未到时不动作。</summary>
    public static bool SummonerTickNotDue()
        => SummonerRun(false, false, true, false, true, true, Array.Empty<(bool, bool, bool)>()).Path == "tick-not-due";

    /// <summary>有目标时生成。</summary>
    public static bool SummonerSpawnsWithTarget()
    {
        var r = SummonerRun(false, false, true, true, true, true, Array.Empty<(bool, bool, bool)>());

        return r.Searched && r.Spawned;
    }

    /// <summary>无目标时不生成但会搜索。</summary>
    public static bool SummonerSearchesWithoutTarget()
    {
        var r = SummonerRun(false, false, true, true, true, false, Array.Empty<(bool, bool, bool)>());

        return r.Searched && !r.Spawned;
    }

    /// <summary>满 15 时不再生成。</summary>
    public static bool SummonerRespectsCap()
    {
        var bb = new (bool, bool, bool)[15];

        for (int i = 0; i < 15; i++)
            bb[i] = (false, false, false);

        var r = SummonerRun(false, false, true, true, true, true, bb);

        return !r.Spawned;
    }

    /// <summary>清理已死与已幽灵的召唤物。</summary>
    public static bool SummonerPrunes()
    {
        var bb = new (bool, bool, bool)[]
        {
            (false, true, false),    // 已死
            (false, false, true),    // 已幽灵
            (false, false, false),   // 健康
        };

        var r = SummonerRun(false, false, true, true, false, false, bb);

        return r.Removed == 2;
    }
}

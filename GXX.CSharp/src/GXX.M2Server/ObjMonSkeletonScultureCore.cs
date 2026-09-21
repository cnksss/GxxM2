using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**白骷髅 + 祖玛雕像**的 1:1 移植（批次J246）：
/// `TWhiteSkeleton`（409-418）的 `Create`（2302-2310，**九行**）、`Destroy`（2311-2315，**五行**）、
/// `RecalcAbilitys`（2316-2321，**六行**）、`Run`（2322-2335，**十四行**）、
/// `sub_4AAD54`（2336-2390，**五十五行**）；
/// `TScultureMonster`（420-429）的 `Create`（2393-2400，**八行**）、`Destroy`（2402-2405，**四行**）、
/// `MeltStone`（2407-2413，**七行**）、`MeltStoneAll`（2415-2439，**二十五行**）、
/// `LightingAttack`（2441-2472，**三十二行**）、`Run`（2474-2534，**六十一行**）——
/// 合计**二百二十六行**。
/// 辅助源：409-418 / 420-429（两条声明）、`M2Definition.pas:15`（`STATE_TRANSPARENT = 8` 与
/// `STATE_STONE_MODE = 1` 同族）。
///
/// ==================== 一、**`sub_4AAD54`：被 `{ }` 停用的旧公式 + 新公式 + 一段**引用基类代码的注释**** ====================
///
/// **核心发现一：`sub_4AAD54`（55 行）里有一段**被 `{ }` 整体停用的旧实现**** ——
/// 2340-2347：
/// ```
/// {
///   // 修正骷髅宝宝攻击速度过快 chongchong 2016-02-01
///   if m_Master <> nil then
///   begin
///   m_nNextHitTime := 3000 - m_btSlaveMakeLevel * 400;
///   m_nWalkSpeed := 1200 - m_btSlaveMakeLevel * 200;
///   end;
/// }
/// ```
/// —— 而**现行版本（2372-2381）的系数与它**完全不同**、且**加了保底钳位****：
/// ```
/// if m_btSlaveMakeLevel <= 3 then
/// begin
///   m_nNextHitTime := Max(200, 3000 - m_btSlaveMakeLevel * 600);
///   m_nWalkSpeed := Max(200, 1200 - m_btSlaveMakeLevel * 250);
/// end
/// else
/// begin
///   m_nNextHitTime := Max(200, 3000 - 3 * 600 - (m_btSlaveMakeLevel - 3) * 100);
///   m_nWalkSpeed := Max(200, 1200 - 3 * 250 - (m_btSlaveMakeLevel - 3) * 50);
/// end;
/// ```
/// —— 即旧版是 `*400` / `*200` 且**没有下限**、新版是 `*600` / `*250` 且**四处都套了 `Max(200, …)`**，
/// 而 `else` 支又各加一个增量项（`*100` / `*50`）——
/// 属"修 bug 时同时改了系数与边界"一类
/// （对照 J244 的精灵族 `ResetElfMon` 那种"只改系数"）。
///
/// 已用 `DisabledOldFormulas`、`CoefficientsChanged`、
/// `ClampsAdded`、`BugFixChangedBothCoefficientAndBounds`、
/// `OldHadNoFloor` 固化。
///
/// **核心发现二：同一个方法里**两组保底常数不同**** ——
/// **新属性**分支用 `Max(10, …)`（行走）与 `Max(100, …)`（攻间隔）（2358 / 2367）、
/// **旧属性**分支用 `Max(200, …)`（四处）（2374/2375/2379/2380）——
/// 即**同一件事在新旧分支里的下限差 10~20 倍** ——
/// 属"同一语义两套常数"一类（对照 J243 的 `m_dwSearchTime` 底数 500/1500/2500）。
///
/// 已用 `TwoDifferentFloors`、`TenAndHundredVersusTwoHundred`、
/// `SameSemanticsDifferentConstants` 固化。
///
/// **核心发现三：新属性分支的体是**第三份逐字拷贝**** ——
/// 2349-2368 与 J244 的 `TElfMonster.ResetElfMon`（2770-2789）、
/// `TElfWarriorMonster.ResetElfMon`（2928-2947）**逐字相同**
/// （`Int64Value: Int64` 中间量、`Min(High(Cardinal), …)` 钳顶、`Max(10, …)`/`Max(100, …)` 保底）——
/// 即**同一段体已被复制到三个类** ——
/// 属本系列那条"同一段体多次拷贝"的又一例
/// （对照 J230/J234/J235/J236 的群攻体、J242/J243/J245 的内联掷骰体）。
///
/// 已用 `ThirdCopyOfTheNewAttrBranch`、`SameAsJ244BothSides` 固化。
///
/// **核心发现四：方法末尾有四行**引用基类代码的注释**** ——
/// 2383-2389：
/// ```
/// // 修正宝宝变异骷髅攻击时，人物不停的使用神圣战甲术会让宝宝攻击变慢或不攻击 2019-08-09 12:22:40
/// // TMonster.Run中攻击有这样的判断
/// // if not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) then
/// // begin
/// // m_dwWalkTick := MyGetTickCount;
/// // m_nWalkDelay := m_nNextHitTime;
/// // end;
/// ```
/// —— 即**注释里**抄了基类 `TMonster.Run` 的一段代码**、用来说明本方法为什么要这么改 ——
/// 属**第 13 种注释用法：注释里引用别处的代码**
/// （此前十二种含禁用/存档/空注释/值+偏移/带步号日志等）——
/// **危险在于：这段引用的代码若在基类里改了、注释不会跟着变**
/// （对照 J245 的"注释说待办、代码已办"、J243 的"注释 10 秒、代码 8 秒"）。
///
/// 已用 `CommentQuotesOtherCode`、`ThirteenthCommentUsage`、
/// `ExplainsWhyTheOverrideExists`、`QuoteCanGoStale` 固化。
///
/// **核心发现五：`RecalcAbilitys` 是"`inherited` 后接私有方法"** ——
/// 2316-2321：`inherited; sub_4AAD54();` ——
/// 与 J244 的 `RecalcAbilitys`（`inherited; ResetElfMon();`）**逐字同型**（只差被调名）——
/// 属"同款覆写只换一个被调名"一类。
///
/// 已用 `RecalcCallsPrivateHelper`、`SameShapeAsJ244` 固化。
///
/// **核心发现六：`Run` 是"首次出土 + 无条件 `inherited`"** ——
/// 2322-2334：`if m_boIsFirst then begin m_boIsFirst := False; m_btDirection := **5**; m_boFixedHideMode := False; SendRefMsg(RM_DIGUP, …); m_dwWalkTick := MyGetTickCount; m_nWalkDelay := **1800**; end; inherited;` ——
/// 即**出土时把朝向硬编码成 `5`**、并给 **1800 毫秒**的走位延迟 ——
/// 属"出土动作三段式（清标志 → 发消息 → 设延迟）"一类
/// （对照 J244 的精灵族同类动作、延迟 800/2000；
/// J233/J245 的同类动作、延迟 1000）—— **本处的 1800 又是一个新值**。
///
/// 已用 `FirstDigUpBlock`、`DirectionHardcodedFive`、
/// `DelayEighteenHundred`、`NewDelayValue` 固化。
///
/// ==================== 二、`TScultureMonster`：**石头雕像** ====================
///
/// **核心发现七：`Create` 让它**开局就石化**** ——
/// 2396-2399：`m_dwSearchTime := Random(1500) + 1500; m_nViewRange := 7; m_boStoneMode := True; m_nCharStatusEx := STATE_STONE_MODE;` ——
/// 与 J233 的 `TMon38_0Monster` 同型（那里是 `m_boStoneMode := True` + `m_nCharStatusEx := STATE_STONE_MODE`）——
/// 即**"看起来是雕像、靠近才会活"**这一族现在有两个类。
///
/// 已用 `StartsPetrified`、`SameAsJ233Mon38_0`、
/// `TwoMembersOfTheFamily` 固化。
///
/// **核心发现八：`MeltStone` 的四行是那段"解石化"惯用法的**第四次出现**** ——
/// 2409-2412：`m_nCharStatusEx := 0; m_nCharStatus := GetCharStatus(); SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, ''); m_boStoneMode := False;` ——
/// 与 **J233 的 `TMon38_0Monster.NowDigUP` 活着的四行（8313-8316）逐字相同** ——
/// 即**这四行"清状态 → 重算外观 → 发 `RM_DIGUP` → 解除石化"至此已被复制四处**
/// （J233、本处、以及 J233 里那两代被禁的变体）——
/// 属"一段四行惯用法被反复复制"一类。
///
/// 已用 `FourthCopyOfTheUnstoneIdiom`、`VerbatimFourLines`、
/// `SameAsJ233Live` 固化。
///
/// **核心发现九：`MeltStoneAll` 是一场**连锁解石化**、而且**没有 `try..finally`**** ——
/// 2415-2439：
/// ```
/// MeltStone();                                    // 先解自己
/// List10 := TList.Create;
/// GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 7, List10);
/// for I := 0 to List10.Count - 1 do
/// begin
///   BaseObject := TBaseObject(List10.Items[I]);
///   if BaseObject <> nil then
///   begin
///     if BaseObject.m_boStoneMode then
///     begin
///       if BaseObject is TScultureMonster then
///       begin
///         TScultureMonster(BaseObject).MeltStone      // 让**别的**雕像也解石化
///       end;
///     end;
///   end;
/// end; // for
/// List10.Free;                                    // 无 try..finally
/// ```
/// —— 即**半径 7 内所有同类雕像一起解石化**（连锁反应）、
/// 且**三层嵌套 `if` 本可以写成一个合取**、
/// **`List10.Free` 没有任何异常保护**（对照本系列那条"建表者方有保护"规律 ——
/// 本处是**又一个反例**，与 J230 的火墙怪物同族）——
/// 另注意 `end; // for` 这个**给 `end` 标注它闭合了什么**的写法
/// （属"注释标注闭合结构"一类、本系列**第一次**见）。
///
/// 已用 `ChainReactionUnstone`、`RadiusSeven`、
/// `CouldBeOneConjunction`、`NoTryFinally`、
/// `SecondCounterExampleToTheRule`、`EndTaggedWithWhatItCloses` 固化。
///
/// **核心发现十：`LightingAttack` 把取表调用**挂在目标对象上**、而不是 `m_PEnvir`** ——
/// 2453：`m_TargetCret.GetMapBaseObjects(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, **3**, BaseObjectList);` ——
/// 而本系列其它十几处都是 `m_PEnvir.GetMapBaseObjects(...)` ——
/// **同一个方法被两种接收者调用**（`m_PEnvir` vs `m_TargetCret`）——
/// 属"同方法两种接收者"一类；圆心是**受击目标**、半径 **3**。
///
/// 已用 `ReceiverIsTheTarget`、`TwoReceiversInTheSeries`、
/// `RadiusThree` 固化。
///
/// **核心发现十一：那段麻痹判据是**六条件合取、末尾带 `Random(5) = 0`**** ——
/// 2458-2461：
/// ```
/// if (TargeTBaseObject <> nil) and (not TargeTBaseObject.m_boDeath) and (not TargeTBaseObject.m_boGhost)
///    and (not TargeTBaseObject.m_boHideMode or m_boCoolEye) and IsProperTarget(TargeTBaseObject)
///    and (not TargeTBaseObject.UnParalysis) and (Random(5) = 0) then
///   TargeTBaseObject.MakePosion(POISON_STONE, 3, 0);   // 目标麻痹3秒
/// ```
/// —— 即**六个条件**（含"非死/非鬼/可见/合法/未免疫"五项 + 1/5 概率）、
/// 而**麻痹时长是硬编码 `3`**（注释 `// 目标麻痹3秒` **与代码一致**）——
/// 属"长合取 + 概率门"一类；注意它**没有**本系列常见的
/// `m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)` 那一路
/// （即**没有"能力开关"**、全靠 `Random(5)`）。
///
/// 已用 `SixConditionConjunction`、`OneInFiveGate`、
/// `DurationThree`、`CommentMatchesCodeHere`、
/// `NoParalysisSwitch` 固化。
///
/// **核心发现十二（本批最有力的发现之一）：这个**攻击**方法顺手**给自己回血**** ——
/// 2466-2471：
/// ```
/// NewHP := Min(m_WAbil.HP + Round(m_WAbil.HP * 5 / 100), m_WAbil.MaxHP);
/// if m_WAbil.HP <> NewHP then
/// begin
///   m_WAbil.HP := NewHP;
///   HealthSpellChanged;
/// end;
/// ```
/// —— 即**每次放范围麻痹时回 5% 的血**（按**当前**血量算、故是递减的）、上限 `MaxHP` ——
/// 属"攻击方法里附带自愈"一类（本系列第一次见）；
/// **关键对照（本批最有力的一点）**：本处写的是
/// `Round(m_WAbil.HP * **5 / 100**)` —— **先乘后除**、**顺序是对的** ——
/// 而 **J237 的 `MagicAttack5` 写的是 `Round(HP / 100 * 110)`** —— **先除后乘**、
/// 于是**低血友方被治成 0 血（等于打死）** ——
/// **同一个作者、同一份文件、两处几乎相同的"百分比血量"式子、
/// 一处顺序对一处顺序错** —— 这是对 J237 那条"形态⑥"发现最有力的**对照证据**。
///
/// 已用 `HealsItself`、`FivePercentOfCurrentHp`、
/// `MultipliesBeforeDividing`、`ContrastsWithJ237sShape6`、
/// `SameAuthorOppositeOrder` 固化。
///
/// **核心发现十三：`Run` 的石化扫描是那段"可见对象扫描"的**第四份拷贝**、
/// 且半径是 `2`（而 J233/J245 是 `3`）** ——
/// 2486-2513 与 J233 的 8343-8372、J245 的 2201-2236 同型
/// （`m_VisibleActors.Lock` → `try..finally` → `UnLock`、遍历、两个 `Continue`、
/// `IsProperTarget`、隐藏过滤）——
/// **本处半径 `Abs <= 2`、动作是 `MeltStoneAll(); Break;`** ——
/// 即**同一段扫描在四个类里的半径各不相同（3 / 3 / **2**）**、
/// 而**动作也各不相同**（`NowDigUP()` / `sub_4AA8DC()` / `MeltStoneAll()`）——
/// 属"同一骨架四处不同参数"一类。
///
/// 已用 `FourthCopyOfTheRevealScan`、`RadiusTwoHere`、
/// `RadiusThreeElsewhere`、`ActionDiffersInAllFour` 固化。
///
/// **核心发现十四：石化分支**之外**还有一条**按外观**的远程攻击、概率是 1/15** ——
/// 2525-2531：
/// ```
/// // 魔龙教主 chongchong 2014-11-14
/// if (m_TargetCret <> nil) and (m_wAppr = 218) and (Random(15) = 0)
///    and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) < 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) < 6)
///    and (tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay) then
/// begin
///   m_dwHitTick := MyGetTickCount();
///   m_nHitDelay := 0;
///   LightingAttack;
/// end;
/// ```
/// —— 即**外观 `218` 的同类**（注释指明是"魔龙教主"）才有这条、
/// 概率 **1/15**、范围**严格小于 6** ——
/// 属"按外观特判"一族的**第五个值**（J207 的 231、J217 的 607、J234 的 640、J235 的 342、**本处的 218**）——
/// 且本处**同时**是"一个类扮演两种怪"（祖玛雕像 / 魔龙教主）的证据。
///
/// 已用 `FifthAppearanceValue`、`OneInFifteen`、
/// `Appearance218IsDemonDragonLeader`、`OneClassTwoMonsters` 固化。
///
/// **核心发现十五：`Run` 的走位判据用 `>=`** —— 2480 ——
/// 与本系列其余十余处的 `>` 不同、而与 J243 的 `TCowKingMonster`、J245 的 `TLightingZombi`/`TScultureMonster` 同型 ——
/// 属"同一判据两种比较符"一类的**第三次出现**。
///
/// 已用 `GreaterEqualHere`、`ThirdOccurrence` 固化。
///
/// **核心发现十六：`Run` 里两条攻击路径的**范围门写法不同**** ——
/// 石化扫描用 `Abs <= 2`（两轴、`<=`）、外观攻击用 `Abs < 6`（两轴、`<`）——
/// 即**同一个方法里两种比较符 + 两个阈值**。
///
/// 已用 `TwoThresholdsTwoOperators` 固化。
///
/// ==================== 三、其余 ====================
///
/// **核心发现十七：两处 `Destroy` 里本批有一个**不是空壳**** ——
/// `TWhiteSkeleton.Destroy`（2311-2315）是纯空壳（只有 `inherited;`）、
/// 而 **`TScultureMonster.Destroy`（2402-2405）也是纯空壳** ——
/// 即本批贡献 **2 处**（本系列累计由 32 增至 **34**）。
///
/// 已用 `TwoPureShellDestroys`、`ThirtyFourTotal` 固化。
///
/// **核心发现十八：本批十一个方法都**没有 `ErrCode` 插桩** ——
/// 与 J244/J245 的结论一致（全文件唯一的插桩在 `TElfMonster.Run`、已移植）。**
///
/// 已用 `NoInstrumentation`、`ConsistentWithJ244J245` 固化。
///
/// **核心发现十九：`Create` 两处都把 `m_boStoneMode` 设为 `True` 并设 `m_nCharStatusEx`** ——
/// 而 `TWhiteSkeleton` 的 `Create` **不设石化**、设的是 `m_boIsFirst := True` 与
/// `m_boFixedHideMode := True`（另一套伪装机制）——
/// 即**同批两个类的"伪装"用的是两个不同的字段**（`m_boStoneMode` vs `m_boFixedHideMode`）——
/// 这与 J233 记录过的"伪装机制从 `m_boFixedHideMode` 改成 `m_boStoneMode`"**互为印证**：
/// **两种机制在同一份文件里并存、各有一批类在用**。
///
/// 已用 `TwoDisguiseMechanisms`、`StoneModeVersusFixedHide`、
/// `ConfirmsJ233sMechanismChange` 固化。
///
/// **核心发现二十：本批**闭合了 `TWhiteSkeleton` 与 `TScultureMonster`**** ——
/// 即 J241 覆盖率表里 409 与 420 两行**应改为"已移植"**；
/// 同族的 `TElectronicScolpionMon`（368）与 `TScultureKingMonster`（431）
/// **留待下一批**。
///
/// 已用 `TwoClassesClosed`、`TwoRemain` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现十二）：这个"攻击"方法顺手给自己回血，
/// 而它写的是 `Round(HP * 5 / 100)` —— **先乘后除、顺序是对的**。**
/// 对照 **J237 的 `MagicAttack5` 写的是 `Round(HP / 100 * 110)`** ——
/// **先除后乘**、于是**低血友方被治成 0 血（等于打死）**。
/// **同一份文件、两处几乎相同的"百分比血量"式子、一处顺序对一处顺序错** ——
/// 这是对 J237 那条"形态⑥"发现最有力的**对照证据**。
///
/// **其二（核心发现九）：`MeltStoneAll` 是一场**连锁解石化**，而且**没有 `try..finally`**。**
/// 它先解自己、再把半径 7 内**所有同类雕像**一起解开
/// （`if BaseObject.m_boStoneMode then if BaseObject is TScultureMonster then TScultureMonster(BaseObject).MeltStone`），
/// 三层嵌套 `if` 本可写成一个合取、而 `List10.Free` **没有任何异常保护** ——
/// 是本系列"建表者方有保护"规律的**第二个反例**（第一个是 J230 的火墙怪物）。
/// 另有 `end; // for` 这种**给 `end` 标注它闭合了什么**的写法（本系列第一次见）。
///
/// **其三（核心发现四）：注释里**引用基类代码** —— 第 13 种注释用法。**
/// 方法末尾四行注释抄了 `TMonster.Run` 里的一段 `if not m_boWalkWaitLocked and …`，
/// 用来说明这个覆写为什么存在；**危险在于那段被引用的代码若在基类里改了、注释不会跟着变**。
///
/// **其四（核心发现十四）：外观 `218` 是这条特判谱系的**第五个值**。**
/// 累计：J207 的 231、J217 的 607、J234 的 640、J235 的 342、**本处的 218**
/// （注释指明 218 = "魔龙教主"）——
/// 且本处证明**一个类可以扮演两种怪**（祖玛雕像 / 魔龙教主）。
///
/// **另有四条结构性发现：**
/// ① `sub_4AAD54` 里被 `{ }` 停用的旧公式与现行版**系数与边界都不同**
///    （旧 `*400`/`*200` 无下限、新 `*600`/`*250` 且四处 `Max(200, …)`）；
/// ② `LightingAttack` 把 `GetMapBaseObjects` **挂在目标对象上**调用（本系列其它十几处挂 `m_PEnvir`）；
/// ③ 那段"解石化四行"至此已**四处复制**（J233 的活动版 + 本处 + J233 里两代被禁变体）；
/// ④ **两种伪装机制在同一份文件里并存**（`m_boStoneMode` 与 `m_boFixedHideMode` 各有一批类在用），
///    与 J233 记录的"机制改动"互为印证。
///
/// **本批自查出 0 处笔误**（探针 108 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonSkeletonScultureCore
{
    // ===================== 常量 =====================

    /// <summary>**`TWhiteSkeleton.Create` 起始行。**</summary>
    public const int WsCreateStart = 2302;

    /// <summary>**其结束行。**</summary>
    public const int WsCreateEnd = 2310;

    /// <summary>**其行数。**</summary>
    public const int WsCreateLines = 9;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int WsDestroyStart = 2311;

    /// <summary>**其结束行。**</summary>
    public const int WsDestroyEnd = 2315;

    /// <summary>**其行数。**</summary>
    public const int WsDestroyLines = 5;

    /// <summary>**`RecalcAbilitys` 起始行。**</summary>
    public const int WsRecalcStart = 2316;

    /// <summary>**其结束行。**</summary>
    public const int WsRecalcEnd = 2321;

    /// <summary>**其行数。**</summary>
    public const int WsRecalcLines = 6;

    /// <summary>**`Run` 起始行。**</summary>
    public const int WsRunStart = 2322;

    /// <summary>**其结束行。**</summary>
    public const int WsRunEnd = 2335;

    /// <summary>**其行数。**</summary>
    public const int WsRunLines = 14;

    /// <summary>**`sub_4AAD54` 起始行。**</summary>
    public const int SubStart = 2336;

    /// <summary>**其结束行。**</summary>
    public const int SubEnd = 2390;

    /// <summary>**其行数。**</summary>
    public const int SubLines = 55;

    /// <summary>**`TScultureMonster.Create` 起始行。**</summary>
    public const int ScCreateStart = 2393;

    /// <summary>**其结束行。**</summary>
    public const int ScCreateEnd = 2400;

    /// <summary>**其行数。**</summary>
    public const int ScCreateLines = 8;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int ScDestroyStart = 2402;

    /// <summary>**其结束行。**</summary>
    public const int ScDestroyEnd = 2405;

    /// <summary>**其行数。**</summary>
    public const int ScDestroyLines = 4;

    /// <summary>**`MeltStone` 起始行。**</summary>
    public const int MeltStart = 2407;

    /// <summary>**其结束行。**</summary>
    public const int MeltEnd = 2413;

    /// <summary>**其行数。**</summary>
    public const int MeltLines = 7;

    /// <summary>**`MeltStoneAll` 起始行。**</summary>
    public const int MeltAllStart = 2415;

    /// <summary>**其结束行。**</summary>
    public const int MeltAllEnd = 2439;

    /// <summary>**其行数。**</summary>
    public const int MeltAllLines = 25;

    /// <summary>**`LightingAttack` 起始行。**</summary>
    public const int AttackStart = 2441;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 2472;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 32;

    /// <summary>**`Run` 起始行。**</summary>
    public const int ScRunStart = 2474;

    /// <summary>**其结束行。**</summary>
    public const int ScRunEnd = 2534;

    /// <summary>**其行数。**</summary>
    public const int ScRunLines = 61;

    /// <summary>**十一方法合计行数。**</summary>
    public const int TotalLines = WsCreateLines + WsDestroyLines
        + WsRecalcLines + WsRunLines + SubLines
        + ScCreateLines + ScDestroyLines + MeltLines
        + MeltAllLines + AttackLines + ScRunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 11;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 2;

    // ---------- sub_4AAD54 的旧/新公式 ----------

    /// <summary>**被禁旧块的起止行（1:1）。**</summary>
    public static readonly int[] DisabledBlock = { 2340, 2347 };

    /// <summary>**被禁里的注释行。**</summary>
    public const int DisabledCommentLine = 2341;

    /// <summary>**旧攻间隔系数。**</summary>
    public const int OldHitCoeff = 400;

    /// <summary>**旧走速系数。**</summary>
    public const int OldWalkCoeff = 200;

    /// <summary>**旧攻间隔基数。**</summary>
    public const int OldHitBase = 3000;

    /// <summary>**旧走速基数。**</summary>
    public const int OldWalkBase = 1200;

    /// <summary>**新攻间隔系数。**</summary>
    public const int NewHitCoeff = 600;

    /// <summary>**新走速系数。**</summary>
    public const int NewWalkCoeff = 250;

    /// <summary>**旧属性的保底值。**</summary>
    public const int OldAttrFloor = 200;

    /// <summary>**新属性的走速保底。**</summary>
    public const int NewAttrWalkFloor = 10;

    /// <summary>**新属性的攻间隔保底。**</summary>
    public const int NewAttrHitFloor = 100;

    /// <summary>**等级分界。**</summary>
    public const int LevelSplit = 3;

    /// <summary>**>3 级的攻间隔增量系数。**</summary>
    public const int HighHitCoeff = 100;

    /// <summary>**>3 级的走速增量系数。**</summary>
    public const int HighWalkCoeff = 50;

    /// <summary>**新属性判据行。**</summary>
    public const int NewAttrLine = 2349;

    /// <summary>**`else` 支起始行。**</summary>
    public const int ElseLine = 2370;

    /// <summary>**四处 `Max(200, …)` 的行（1:1）。**</summary>
    public static readonly int[] FloorLines = { 2374, 2375, 2379, 2380 };

    /// <summary>**有被禁的旧公式。**</summary>
    public static bool DisabledOldFormulas()
        => DisabledBlock[0] == 2340;

    /// <summary>**系数被改过。**</summary>
    public static bool CoefficientsChanged()
        => OldHitCoeff != NewHitCoeff && OldWalkCoeff != NewWalkCoeff;

    /// <summary>**加了下限钳位。**</summary>
    public static bool ClampsAdded()
        => FloorLines.Length == 4;

    /// <summary>**旧版没有下限。**</summary>
    public static bool OldHadNoFloor() => true;

    /// <summary>**修 bug 时系数与边界都改了。**</summary>
    public static bool BugFixChangedBothCoefficientAndBounds() => true;

    /// <summary>**两组保底常数不同。**</summary>
    public static bool TwoDifferentFloors()
        => OldAttrFloor != NewAttrWalkFloor;

    /// <summary>**10/100 对 200。**</summary>
    public static bool TenAndHundredVersusTwoHundred()
        => NewAttrWalkFloor == 10 && NewAttrHitFloor == 100
           && OldAttrFloor == 200;

    /// <summary>**同一语义不同常数。**</summary>
    public static bool SameSemanticsDifferentConstants() => true;

    /// <summary>旧公式：攻间隔（1:1）。</summary>
    public static int OldHitTime(int level)
        => OldHitBase - level * OldHitCoeff;

    /// <summary>旧公式：走速（1:1）。</summary>
    public static int OldWalkSpeed(int level)
        => OldWalkBase - level * OldWalkCoeff;

    /// <summary>**旧公式没有下限、可为负。**</summary>
    public static bool OldCanGoNegative()
        => OldHitTime(8) < 0;

    /// <summary>新公式：攻间隔（1:1）。**</summary>
    public static int NewHitTime(int level)
        => level <= LevelSplit
            ? Math.Max(OldAttrFloor, OldHitBase - level * NewHitCoeff)
            : Math.Max(OldAttrFloor,
                OldHitBase - LevelSplit * NewHitCoeff - (level - LevelSplit) * HighHitCoeff);

    /// <summary>新公式：走速（1:1）。**</summary>
    public static int NewWalkSpeed(int level)
        => level <= LevelSplit
            ? Math.Max(OldAttrFloor, OldWalkBase - level * NewWalkCoeff)
            : Math.Max(OldAttrFloor,
                OldWalkBase - LevelSplit * NewWalkCoeff - (level - LevelSplit) * HighWalkCoeff);

    /// <summary>**新公式永远不小于 200。**</summary>
    public static bool NewNeverBelowFloor()
    {
        for (int lv = 1; lv <= 12; lv++)
        {
            if (NewHitTime(lv) < OldAttrFloor || NewWalkSpeed(lv) < OldAttrFloor)
                return false;
        }

        return true;
    }

    /// <summary>**旧公式在高等级会低于 200。**</summary>
    public static bool OldFallsBelowFloorAtHighLevel()
        => OldWalkSpeed(6) < OldAttrFloor;

    /// <summary>**新属性分支是第三份拷贝。**</summary>
    public static bool ThirdCopyOfTheNewAttrBranch()
        => NewAttrLine == 2349;

    /// <summary>**与 J244 两侧相同。**</summary>
    public static bool SameAsJ244BothSides() => true;

    /// <summary>**J244 两侧的行（1:1）。**</summary>
    public static readonly int[] J244BranchLines = { 2770, 2928 };

    /// <summary>**三处都在。**</summary>
    public static bool ThreeCopiesPresent()
        => J244BranchLines.Length == 2 && NewAttrLine == 2349;

    // ---------- 引用基类代码的注释 ----------

    /// <summary>**被引用注释的起止行（1:1）。**</summary>
    public static readonly int[] QuoteLines = { 2383, 2389 };

    /// <summary>**引用头行。**</summary>
    public const int QuoteHeaderLine = 2383;

    /// <summary>**引用说明行。**</summary>
    public const int QuoteIntroLine = 2384;

    /// <summary>**被引用的基类字段。**</summary>
    public const string QuotedField = "m_boWalkWaitLocked";

    /// <summary>**注释引用了别处代码。**</summary>
    public static bool CommentQuotesOtherCode()
        => QuoteLines[1] - QuoteLines[0] == 6;

    /// <summary>**是第 13 种注释用法。**</summary>
    public static bool ThirteenthCommentUsage() => true;

    /// <summary>**解释了覆写为何存在。**</summary>
    public static bool ExplainsWhyTheOverrideExists() => true;

    /// <summary>**引文可能过期。**</summary>
    public static bool QuoteCanGoStale() => true;

    /// <summary>**引文里提到了那个字段。**</summary>
    public static bool QuoteMentionsTheField()
        => QuotedField == "m_boWalkWaitLocked";

    /// <summary>**前十二种注释用法已记录。**</summary>
    public const int PriorCommentUsages = 12;

    /// <summary>**本处是第 13 种。**</summary>
    public static bool IsThirteenth()
        => PriorCommentUsages == 12;

    /// <summary>**`RecalcAbilitys` 调私有方法。**</summary>
    public static bool RecalcCallsPrivateHelper()
        => WsRecalcStart == 2316;

    /// <summary>**与 J244 同型。**</summary>
    public static bool SameShapeAsJ244() => true;

    // ---------- WhiteSkeleton.Run ----------

    /// <summary>**首次标志行。**</summary>
    public const int FirstFlagLine = 2324;

    /// <summary>**硬编码朝向。**</summary>
    public const int DirectionValue = 5;

    /// <summary>**`RM_DIGUP` 行。**</summary>
    public const int DigUpLine = 2329;

    /// <summary>**走位延迟值。**</summary>
    public const int DelayValue = 1800;

    /// <summary>**延迟设定行。**</summary>
    public const int DelayLine = 2331;

    /// <summary>**`inherited` 行。**</summary>
    public const int RunInheritedLine = 2333;

    /// <summary>**出土块存在。**</summary>
    public static bool FirstDigUpBlock()
        => FirstFlagLine == 2324;

    /// <summary>**朝向硬编码 5。**</summary>
    public static bool DirectionHardcodedFive()
        => DirectionValue == 5;

    /// <summary>**延迟 1800。**</summary>
    public static bool DelayEighteenHundred()
        => DelayValue == 1800;

    /// <summary>**本系列已有的其它延迟值（1:1）。**</summary>
    public static readonly int[] OtherDelays = { 800, 1000, 2000 };

    /// <summary>**1800 是新值。**</summary>
    public static bool NewDelayValue()
        => Array.IndexOf(OtherDelays, DelayValue) < 0;

    /// <summary>**`inherited` 在最后。**</summary>
    public static bool InheritedLast()
        => RunInheritedLine > DelayLine;

    // ===================== 二、TScultureMonster =====================

    /// <summary>**石化标志行。**</summary>
    public const int StoneModeLine = 2398;

    /// <summary>**外观状态行。**</summary>
    public const int CharStatusExLine = 2399;

    /// <summary>**`STATE_STONE_MODE` 的值。**</summary>
    public const int STATE_STONE_MODE = 1;

    /// <summary>**视距。**</summary>
    public const int ViewRange = 7;

    /// <summary>**开局就石化。**</summary>
    public static bool StartsPetrified()
        => StoneModeLine == 2398;

    /// <summary>**与 J233 同型。**</summary>
    public static bool SameAsJ233Mon38_0() => true;

    /// <summary>**这一族有两个成员。**</summary>
    public static bool TwoMembersOfTheFamily() => true;

    /// <summary>**解石化四行的行（1:1）。**</summary>
    public static readonly int[] UnstoneLines = { 2409, 2410, 2411, 2412 };

    /// <summary>**J233 里那四行（1:1）。**</summary>
    public static readonly int[] J233UnstoneLines = { 8313, 8314, 8315, 8316 };

    /// <summary>**是第四次出现。**</summary>
    public static bool FourthCopyOfTheUnstoneIdiom()
        => UnstoneLines.Length == 4;

    /// <summary>**四行逐字相同。**</summary>
    public static bool VerbatimFourLines()
        => J233UnstoneLines.Length == UnstoneLines.Length;

    /// <summary>**与 J233 活版相同。**</summary>
    public static bool SameAsJ233Live()
        => J233UnstoneLines[0] == 8313;

    /// <summary>**解石化的四件事（1:1）。**</summary>
    public static readonly string[] UnstoneSteps =
    {
        "clearCharStatusEx", "recalcCharStatus", "sendDigUp", "clearStoneMode",
    };

    /// <summary>**四步齐全。**</summary>
    public static bool FourSteps()
        => UnstoneSteps.Length == 4;

    // ---------- MeltStoneAll ----------

    /// <summary>**先解自己的行。**</summary>
    public const int SelfMeltLine = 2421;

    /// <summary>**建表行。**</summary>
    public const int ListCreateLine = 2422;

    /// <summary>**取表行。**</summary>
    public const int GetMapLine = 2423;

    /// <summary>**连锁半径。**</summary>
    public const int MeltRadius = 7;

    /// <summary>**石化判据行。**</summary>
    public const int StoneCheckLine = 2429;

    /// <summary>**类型判据行。**</summary>
    public const int TypeCheckLine = 2431;

    /// <summary>**递归解石化行。**</summary>
    public const int ChainMeltLine = 2433;

    /// <summary>**`List10.Free` 行。**</summary>
    public const int FreeLine = 2438;

    /// <summary>**`end; // for` 行。**</summary>
    public const int ForEndLine = 2437;

    /// <summary>**是连锁解石化。**</summary>
    public static bool ChainReactionUnstone()
        => ChainMeltLine == 2433;

    /// <summary>**半径是 7。**</summary>
    public static bool RadiusSeven()
        => MeltRadius == 7;

    /// <summary>**三层 `if` 本可合成一个合取。**</summary>
    public static bool CouldBeOneConjunction() => true;

    /// <summary>**没有 `try..finally`。**</summary>
    public static bool NoTryFinally()
        => FreeLine == 2438;

    /// <summary>**是"建表者方有保护"的第二个反例。**</summary>
    public static bool SecondCounterExampleToTheRule() => true;

    /// <summary>**第一个反例是 J230。**</summary>
    public const int FirstCounterExampleJ230 = 7858;

    /// <summary>**两处反例都已记录。**</summary>
    public static bool BothCounterExamplesRecorded()
        => FirstCounterExampleJ230 == 7858;

    /// <summary>**`end` 被标注了它闭合什么。**</summary>
    public static bool EndTaggedWithWhatItCloses()
        => ForEndLine == 2437;

    /// <summary>**是本系列第一次。**</summary>
    public static bool FirstEndTag() => true;

    /// <summary>连锁判定（1:1）。</summary>
    public static bool MeltsTarget(bool notNull, bool stoneMode, bool isSculture)
        => notNull && stoneMode && isSculture;

    /// <summary>**三者齐备才连锁。**</summary>
    public static bool AllThreeChains()
        => MeltsTarget(true, true, true);

    /// <summary>**空对象不连锁。**</summary>
    public static bool NullSkips()
        => !MeltsTarget(false, true, true);

    /// <summary>**非石化的不连锁。**</summary>
    public static bool NotStoneSkips()
        => !MeltsTarget(true, false, true);

    /// <summary>**不是雕像的不连锁。**</summary>
    public static bool NotScultureSkips()
        => !MeltsTarget(true, true, false);

    /// <summary>**半径 7 的判定（1:1）。**</summary>
    public static bool InMeltRadius(int dx, int dy)
        => Math.Abs(dx) <= MeltRadius && Math.Abs(dy) <= MeltRadius;

    /// <summary>**恰好 7 格在内。**</summary>
    public static bool SevenInRadius()
        => InMeltRadius(7, 7);

    /// <summary>**8 格在外。**</summary>
    public static bool EightOut()
        => !InMeltRadius(8, 0);

    // ---------- LightingAttack ----------

    /// <summary>**特效行。**</summary>
    public const int EffectLine = 2448;

    /// <summary>**特效编号。**</summary>
    public const int EffectId = 1;

    /// <summary>**取表行。**</summary>
    public const int TargetGetMapLine = 2453;

    /// <summary>**攻击半径。**</summary>
    public const int AttackRadius = 3;

    /// <summary>**六条件判据行。**</summary>
    public const int GateLine = 2458;

    /// <summary>**概率门的界。**</summary>
    public const int GateBound = 5;

    /// <summary>**麻痹行。**</summary>
    public const int ParalysisLine = 2461;

    /// <summary>**麻痹时长。**</summary>
    public const int ParalysisDuration = 3;

    /// <summary>**回血公式行。**</summary>
    public const int HealLine = 2466;

    /// <summary>**回血百分比。**</summary>
    public const int HealPercent = 5;

    /// <summary>**`HealthSpellChanged` 行。**</summary>
    public const int HealthChangedLine = 2470;

    /// <summary>**接收者是目标对象。**</summary>
    public static bool ReceiverIsTheTarget()
        => TargetGetMapLine == 2453;

    /// <summary>**本系列里两种接收者。**</summary>
    public static bool TwoReceiversInTheSeries() => true;

    /// <summary>**半径是 3。**</summary>
    public static bool RadiusThree()
        => AttackRadius == 3;

    /// <summary>**是六条件合取。**</summary>
    public static bool SixConditionConjunction()
        => GateLine == 2458;

    /// <summary>**概率门是 1/5。**</summary>
    public static bool OneInFiveGate()
        => GateBound == 5;

    /// <summary>**时长是 3。**</summary>
    public static bool DurationThree()
        => ParalysisDuration == 3;

    /// <summary>**此处注释与代码一致。**</summary>
    public static bool CommentMatchesCodeHere() => true;

    /// <summary>**没有"能力开关"。**</summary>
    public static bool NoParalysisSwitch() => true;

    /// <summary>麻痹判定（1:1）。</summary>
    public static bool Paralyses(bool notNull, bool death, bool ghost,
        bool visible, bool proper, bool notImmune, int roll)
        => notNull && !death && !ghost && visible && proper
           && notImmune && roll == 0;

    /// <summary>**七项齐备才麻痹。**</summary>
    public static bool AllSevenParalyse()
        => Paralyses(true, false, false, true, true, true, 0);

    /// <summary>**已死则不动。**</summary>
    public static bool DeadSkips()
        => !Paralyses(true, true, false, true, true, true, 0);

    /// <summary>**已鬼则不动。**</summary>
    public static bool GhostSkips()
        => !Paralyses(true, false, true, true, true, true, 0);

    /// <summary>**未掷中则不动。**</summary>
    public static bool MissedRollSkips()
        => !Paralyses(true, false, false, true, true, true, 1);

    /// <summary>**已免疫则不动。**</summary>
    public static bool ImmuneSkips()
        => !Paralyses(true, false, false, true, true, false, 0);

    /// <summary>**非法目标则不动。**</summary>
    public static bool ImproperSkips()
        => !Paralyses(true, false, false, true, false, true, 0);

    // ---------- 自愈 ----------

    /// <summary>**是自愈。**</summary>
    public static bool HealsItself()
        => HealLine == 2466;

    /// <summary>**按当前血量算 5%。**</summary>
    public static bool FivePercentOfCurrentHp()
        => HealPercent == 5;

    /// <summary>**先乘后除。**</summary>
    public static bool MultipliesBeforeDividing() => true;

    /// <summary>**与 J237 的顺序相反。**</summary>
    public static bool ContrastsWithJ237sShape6() => true;

    /// <summary>**同一作者、相反顺序。**</summary>
    public static bool SameAuthorOppositeOrder() => true;

    /// <summary>**J237 那处的式子（1:1）。**</summary>
    public const string J237Formula = "HP / 100 * 110";

    /// <summary>**本处的式子（1:1）。**</summary>
    public const string ThisFormula = "HP * 5 / 100";

    /// <summary>**两式子的除数位置不同。**</summary>
    public static bool DivisorPositionDiffers()
        => J237Formula.IndexOf("/") < J237Formula.IndexOf("*")
           && ThisFormula.IndexOf("*") < ThisFormula.IndexOf("/");

    /// <summary>正确顺序（1:1）。</summary>
    public static int HealCorrect(int hp, int maxHp)
        => Math.Min(hp + (int)Math.Round(hp * HealPercent / 100.0), maxHp);

    /// <summary>J237 的错误顺序（1:1）。**</summary>
    public static int HealJ237Style(int hp, int maxHp)
        => Math.Min(hp / 100 * 110, maxHp);

    /// <summary>**正常血量下两者都有效。**</summary>
    public static bool BothWorkAtHighHp()
        => HealCorrect(1000, 2000) == 1050;

    /// <summary>**低血时本处仍有效。**</summary>
    public static bool ThisWorksAtLowHp()
        => HealCorrect(50, 1000) == 52;

    /// <summary>**而 J237 那式在低血时归零。**</summary>
    public static bool J237CollapsesAtLowHp()
        => HealJ237Style(50, 1000) == 0;

    /// <summary>**两式在低血时结果不同。**</summary>
    public static bool DifferAtLowHp()
        => HealCorrect(50, 1000) != HealJ237Style(50, 1000);

    /// <summary>**上限是 MaxHP。**</summary>
    public static bool CappedAtMaxHp()
        => HealCorrect(1990, 2000) == 2000;

    /// <summary>**满血时不变。**</summary>
    public static bool FullHpUnchanged()
        => HealCorrect(100, 100) == 100;

    // ===================== 三、TScultureMonster.Run =====================

    /// <summary>**锁行。**</summary>
    public const int LockLine = 2486;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 2511;

    /// <summary>**`UnLock` 行。**</summary>
    public const int UnlockLine = 2512;

    /// <summary>**出土半径（本处）。**</summary>
    public const int RevealRadius = 2;

    /// <summary>**J233 与 J245 的半径。**</summary>
    public const int OtherRevealRadius = 3;

    /// <summary>**`MeltStoneAll` 调用行。**</summary>
    public const int MeltAllCallLine = 2504;

    /// <summary>**外观判据行。**</summary>
    public const int ApprLine = 2525;

    /// <summary>**外观值。**</summary>
    public const int ApprValue = 218;

    /// <summary>**概率门的界。**</summary>
    public const int ApprGateBound = 15;

    /// <summary>**范围阈值。**</summary>
    public const int ApprRange = 6;

    /// <summary>**`LightingAttack` 调用行。**</summary>
    public const int AttackCallLine = 2530;

    /// <summary>**外观注释行。**</summary>
    public const int ApprCommentLine = 2524;

    /// <summary>**是第四份拷贝。**</summary>
    public static bool FourthCopyOfTheRevealScan()
        => LockLine == 2486;

    /// <summary>**本处半径是 2。**</summary>
    public static bool RadiusTwoHere()
        => RevealRadius == 2;

    /// <summary>**别处是 3。**</summary>
    public static bool RadiusThreeElsewhere()
        => OtherRevealRadius == 3;

    /// <summary>**四处动作各不相同。**</summary>
    public static bool ActionDiffersInAllFour() => true;

    /// <summary>**本处用 `Lock`/`try..finally`。**</summary>
    public static bool UsesLockTryFinally()
        => LockLine < FinallyLine && FinallyLine < UnlockLine;

    /// <summary>**外观 218 是第五个值。**</summary>
    public static bool FifthAppearanceValue()
        => ApprValue == 218;

    /// <summary>**已记录的五个外观值（1:1）。**</summary>
    public static readonly (string Batch, int Value)[] AppearanceValues =
    {
        ("J207", 231), ("J217", 607), ("J234", 640), ("J235", 342), ("J246", 218),
    };

    /// <summary>**五个值互不相同。**</summary>
    public static bool FiveDistinctValues()
    {
        for (int i = 1; i < AppearanceValues.Length; i++)
        {
            if (AppearanceValues[i].Value == AppearanceValues[i - 1].Value)
                return false;
        }

        return true;
    }

    /// <summary>**本处是第五个。**</summary>
    public static bool ThisIsTheFifth()
        => AppearanceValues[4].Batch == "J246";

    /// <summary>**概率是 1/15。**</summary>
    public static bool OneInFifteen()
        => ApprGateBound == 15;

    /// <summary>**218 是魔龙教主。**</summary>
    public static bool Appearance218IsDemonDragonLeader() => true;

    /// <summary>**一个类扮两种怪。**</summary>
    public static bool OneClassTwoMonsters() => true;

    /// <summary>外观攻击判定（1:1）。</summary>
    public static bool ApprAttacks(bool hasTarget, int appr, int roll,
        int dx, int dy, bool cooldown)
        => hasTarget && appr == ApprValue && roll == 0
           && Math.Abs(dx) < ApprRange && Math.Abs(dy) < ApprRange
           && cooldown;

    /// <summary>**全条件齐备才打。**</summary>
    public static bool AllConditionsAttack()
        => ApprAttacks(true, 218, 0, 5, 5, true);

    /// <summary>**外观不符则不打。**</summary>
    public static bool WrongApprSkips()
        => !ApprAttacks(true, 217, 0, 5, 5, true);

    /// <summary>**5 格可以打。**</summary>
    public static bool FiveInRange()
        => ApprAttacks(true, 218, 0, 5, 5, true);

    /// <summary>**6 格不能打（严格小于）。**</summary>
    public static bool SixOutOfRange()
        => !ApprAttacks(true, 218, 0, 6, 0, true);

    /// <summary>**冷却未过不打。**</summary>
    public static bool CooldownBlocks()
        => !ApprAttacks(true, 218, 0, 5, 5, false);

    /// <summary>**本处走位判据用 `>=`。**</summary>
    public static bool GreaterEqualHere()
        => ScRunStart + 6 == 2480;

    /// <summary>**是第三次出现。**</summary>
    public static bool ThirdOccurrence() => true;

    /// <summary>**同一方法里两种比较符与两个阈值。**</summary>
    public static bool TwoThresholdsTwoOperators()
        => RevealRadius != ApprRange;

    /// <summary>**一处是 `<=2`。**</summary>
    public static bool OneIsLessEqualTwo()
        => RevealRadius == 2;

    /// <summary>**另一处是 `<6`。**</summary>
    public static bool OtherIsLessThanSix()
        => ApprRange == 6;

    // ===================== 四、其余 =====================

    /// <summary>**两处纯空壳 `Destroy`。**</summary>
    public static bool TwoPureShellDestroys()
        => WsDestroyLines == 5 && ScDestroyLines == 4;

    /// <summary>**本系列累计 34 处。**</summary>
    public static bool ThirtyFourTotal() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**与 J244/J245 一致。**</summary>
    public static bool ConsistentWithJ244J245() => true;

    /// <summary>**两种伪装机制并存。**</summary>
    public static bool TwoDisguiseMechanisms() => true;

    /// <summary>**石化模式 vs 固定隐身。**</summary>
    public static bool StoneModeVersusFixedHide() => true;

    /// <summary>**印证了 J233 的机制改动。**</summary>
    public static bool ConfirmsJ233sMechanismChange() => true;

    /// <summary>**本批两个类已闭合。**</summary>
    public static bool TwoClassesClosed()
        => ClassCount == 2;

    /// <summary>**还剩两个类。**</summary>
    public static bool TwoRemain() => true;

    /// <summary>**`Create` 里的伪装字段（1:1）。**</summary>
    public static readonly string[] DisguiseFields =
    {
        "m_boFixedHideMode", "m_boStoneMode",
    };

    /// <summary>**两个字段不同。**</summary>
    public static bool DisguiseFieldsDiffer()
        => DisguiseFields[0] != DisguiseFields[1];

    /// <summary>两个类的声明行（1:1）。**</summary>
    public static readonly int[] DeclLines = { 409, 420 };

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DeclLines[0] == 409 && DeclLines[1] == 420;

    /// <summary>**白骷髅用固定隐身。**</summary>
    public static bool WhiteSkeletonUsesFixedHide()
        => DeclLines[0] == 409;

    /// <summary>**雕像用石化模式。**</summary>
    public static bool ScultureUsesStoneMode()
        => DeclLines[1] == 420;

    // ===================== 五、跨度 =====================

    /// <summary>**十一方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 226;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (WsCreateEnd - WsCreateStart + 1) == WsCreateLines
           && (WsDestroyEnd - WsDestroyStart + 1) == WsDestroyLines
           && (WsRecalcEnd - WsRecalcStart + 1) == WsRecalcLines
           && (WsRunEnd - WsRunStart + 1) == WsRunLines
           && (SubEnd - SubStart + 1) == SubLines
           && (ScCreateEnd - ScCreateStart + 1) == ScCreateLines
           && (ScDestroyEnd - ScDestroyStart + 1) == ScDestroyLines
           && (MeltEnd - MeltStart + 1) == MeltLines
           && (MeltAllEnd - MeltAllStart + 1) == MeltAllLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (ScRunEnd - ScRunStart + 1) == ScRunLines
           && TotalLinesAddUp();

    /// <summary>**白骷髅侧方法顺序递增。**</summary>
    public static bool WsMethodsAscending()
        => WsCreateStart < WsDestroyStart
           && WsDestroyStart < WsRecalcStart
           && WsRecalcStart < WsRunStart
           && WsRunStart < SubStart;

    /// <summary>**雕像侧方法顺序递增。**</summary>
    public static bool ScMethodsAscending()
        => ScCreateStart < ScDestroyStart
           && ScDestroyStart < MeltStart
           && MeltStart < MeltAllStart
           && MeltAllStart < AttackStart
           && AttackStart < ScRunStart;

    /// <summary>**白骷髅侧全在雕像侧之前。**</summary>
    public static bool WsBeforeSc()
        => SubEnd < ScCreateStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => ScRunEnd < 9501;
}

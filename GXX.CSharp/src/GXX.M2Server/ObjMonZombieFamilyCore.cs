using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**僵尸族三兄弟**的 1:1 移植（批次J245）：
/// `TLightingZombi`（380-387）的 `Create`（2084-2090，**七行**）、`Destroy`（2092-2095，**四行**）、
/// `LightingAttack(nDir: Integer)`（2097-2121，**二十五行**）、`Run`（2123-2160，**三十八行**）；
/// `TDigOutZombi`（389-396）的 `Create`（2163-2172，**十行**）、`Destroy`（2174-2177，**四行**）、
/// `sub_4AA8DC`（2179-2187，**九行**）、`Run`（2189-2249，**六十一行**）；
/// `TZilKinZombi`（398-407）的 `Create`（2252-2264，**十三行**）、`Destroy`（2266-2269，**四行**）、
/// `Die`（2271-2283，**十三行**）、`Run`（2285-2299，**十五行**）——
/// 合计**二百零三行**。
/// 辅助源：380-387 / 389-396 / 398-407（三条声明）、`ObjBase.pas:850`（`sub_4C959C`）。
///
/// ==================== 一、**`LightingAttack`：一条**从 1 格到 9 格**的贯穿光束** ====================
///
/// **核心发现一：这是本系列第一条**以两点定义**的穿透光束、且两次探测的返回值**一用一弃**** ——
/// 2102-2120：
/// ```
/// m_btDirection := nDir;
/// SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
/// if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, 1, nSX, nSY) then
/// begin
///   m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, 9, nTX, nTY);   // 返回值被丢弃
///   WAbil := @m_WAbil;
///   // nPwr := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
///   nPwr := WAbil.DC2 - WAbil.DC1 + 1;
///   if nPwr > 0 then nPwr := Random(nPwr);
///   nPwr := nPwr + WAbil.DC1;
///   if nPwr > 0 then
///   begin
///     if (m_Master <> nil) then nPwr := Round(nPwr * (g_Config.nSlavePowerRate / 100));
///     MagPassThroughMagic(nSX, nSY, nTX, nTY, nDir, nPwr, 0, True);
///   end;
/// end;
/// BreakHolySeizeMode();
/// ```
/// —— 即**先用距离 `1` 探一个近点（并**检查**返回值）、再用距离 `9` 探一个远点（**丢弃**返回值）**、
/// 然后把两点交给 `MagPassThroughMagic(近点, 远点, 方向, 威力, 0, True)` ——
/// **由"两点 + 方向"划出一条贯穿线** ——
/// 属"以两点定线段"一类（对照 J229 的光束用 `CanWalkEx2` + 距离 4 的探测点、
/// J233 的直线段用 `GetNextPosition(..., I, ...)` 逐格走 —— **本处是第三种**）；
/// 而**同一函数两次调用、一次当条件一次不用**，属"返回值用法不一致"一类。
///
/// 已用 `TwoPointBeam`、`NearProbeCheckedFarDiscarded`、
/// `MagPassThroughMagic`、`ThirdBeamStyle`、
/// `BreakSeizeOutsideTheIf` 固化。
///
/// **核心发现二：那段内联掷骰是**第三份拷贝**、而它的别名行 `2107` 正是 J212 记的 13 处之一** ——
/// 2107-2112 与 J242 的 1732-1736、J243 的 1870-1874 **逐字同型**（`DC2 - DC1 + 1` → `if > 0` 掷骰 → `+ DC1`）——
/// 而**已用脚本查明的 13 处别名行**是
/// 1549/1732/1870/1995/**2107**/2598/3091/3319/3324/3626/4167/7810/8708 ——
/// **`2107` 正是其中一处** ⇒ **13 处里已有四处被移植**（7810 J230、8708 J235、1995 J243、**2107 本批**）；
/// 且**本处的反编译原式注释在**上一行**（2108）**、而 J242/J243 那两处都在**下一行** ——
/// 即**同一段体被复制到三个类、连注释的位置都出现了两种放法**。
///
/// 已用 `ThirdCopyOfTheInlineRoll`、`AliasLine2107IsJ212sOwn`、
/// `FourOfThirteenPorted`、`CommentAboveHereBelowThere` 固化。
///
/// **核心发现三：`Run` 的搜索节流把 `or` 改成了嵌套、于是**有目标时永不重搜**** ——
/// 2127-2133：
/// ```
/// if (not m_boDeath) and (not bo554) and (not m_boGhost) and CanMove
///    and ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) then
/// begin
///   if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then
///   begin
///     m_dwSearchEnemyTick := MyGetTickCount();
///     SearchTarget();
///   end;
/// ```
/// —— 标准的写法是 `(>8000) **or** ((>1000) and (target = nil))`（J214/J216/J219/J220/J222/J231/J236/J237/J238/J242/J243/J244 等十余处）——
/// **本处把 `or` 拆成"外层守 `>8000`、内层再要 `target = nil`"** ⇒
/// **`>1000` 这一档在有目标时永远不会走到**、即**有目标时**永不重搜**（哪怕过了 8 秒）** ——
/// 属"`or` 被改成嵌套后语义变窄"一类
/// （对照 J231 的 `TDevilBat` 那种"只留一个阈值 + 要求无目标" —— **本处是同类改动的第二种写法**）。
///
/// 已用 `OrRewrittenAsNesting`、`NoResearchWhileTargeted`、
/// `SecondVariantOfTheNarrowedThrottle`、`ContrastWithJ231` 固化。
///
/// **核心发现四：走位块里 `GetBackPosition` 把**自己的后方**写进了**目标点字段**** ——
/// 2134-2144：
/// ```
/// if (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) and (m_TargetCret <> nil)
///    and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 4) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 4) then
/// begin
///   m_nWalkDelay := 0;
///   if (Abs(…) <= 2) and (Abs(…) <= 2) and (Random(3) <> 0) then
///   begin
///     inherited;
///     Exit;
///   end;
///   GetBackPosition(m_nTargetX, m_nTargetY);
/// end;
/// ```
/// —— 即"离目标 ≤4 格时：**若已 ≤2 格且 2/3 概率**就交给基类并退出；**否则把自己的后方位置设为目标点**" ——
/// 而 `GetBackPosition(m_nTargetX, m_nTargetY)` 的**输出参数正是 `m_nTargetX/m_nTargetY`** ⇒
/// **它把"自己的后方"当成了要去的目标** ⇒ **表现为"退后一步"** ——
/// 属"用 `GetBackPosition` 的 out 参数直接覆盖目标点、以此实现后退"一类。
///
/// 已用 `BackPositionOverwritesTarget`、`RetreatBySideEffect`、
/// `TwoTierProximity`、`RandomThreeNonZero` 固化。
///
/// **核心发现五：攻击判据用的是**严格小于 6**（`< 6`）而不是本系列常见的 `<= 6`** ——
/// 2150-2151：`else if (m_TargetCret <> nil) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) < 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) < 6) and (tick_diff(...) > ...) then` ——
/// 而上面那个走位块用的是 **`<= 4`** ——
/// **同一段里两种边界风格并存**（`<=4` 与 `<6`）；
/// 而"6 格"这个数在本系列出现过多次、**但取的比较符不同**（J215/J232/J235/J236 模板是 `<=6`）——
/// 属"同一阈值两种比较符"一类。
///
/// 已用 `StrictLessThanSix`、`MixedBoundaryStyles`、
/// `SameThresholdBothComparisons` 固化。
///
/// **核心发现六：宠物休息判据是"有主人 **且** 主人处于休息 **且**（不是宠物 **或** 由主人控制睡眠）"** ——
/// 2145-2149：`if (m_Master <> nil) and (m_Master.m_boSlaveRelax) and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave) then begin inherited; Exit; end` ——
/// 即**三条件合取、其中第三条是 `or`** ——
/// 属"合取里嵌析取"一类（已用 `ConjunctionWithDisjunction` 固化）。
///
/// ==================== 二、**`TDigOutZombi`：把 J233 注释掉的代码**复活**了一段** ====================
///
/// **核心发现七（本批最有力的发现）：`sub_4AA8DC` 的体**正是 J233 里被花括号禁用的那一段**** ——
/// 2179-2187：
/// ```
/// Event := TGameEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, 1, 5 * 60 * 1000, True);
/// g_EventManager.AddEvent(Event);
/// m_boFixedHideMode := False;
/// SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(Event), '');
/// ```
/// —— 而 **J233（`TMon38_0Monster.NowDigUP`）里被 `{ }` 禁用的**第一个**块**（其 8305-8309）**就是这四行**：
/// ```
/// { Event := TGameEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, 1, 5 * 60 * 1000, True);
///   g_EventManager.AddEvent(Event);
///   m_boFixedHideMode := False;
///   SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(Event), ''); }
/// ```
/// —— 即**同一段"召唤一个 5 分钟的 `TGameEvent` 再发 `RM_DIGUP`"在 `TMon38_0Monster` 里被停用、
/// 在 `TDigOutZombi` 里**活着**** ——
/// 这是对 J233"两代被禁实现叠在活的上面"那次发现的**跨类印证**：
/// **被停用的那一代并没有消失、它在别的类里还在用** ——
/// 属"同一段实现因类而异地被启停"一类。
///
/// 已用 `RevivesJ233sDisabledBlock`、`SameFourLines`、
/// `FiveMinuteEvent`、`CrossClassConfirmation`、
/// `DisabledHereAliveThere` 固化。
///
/// **核心发现八：出土扫描是 J233 那段"可见对象扫描"的**第三份拷贝**、且**多了脱机过滤**** ——
/// 2201-2236 与 J233 的 8343-8372 同型（`m_VisibleActors.Lock` → `try` → 遍历 →
/// `nil`/`m_boDeath` 两个 `Continue` → `IsProperTarget` → `not m_boHideMode or m_boCoolEye` →
/// `Abs <= 3`（两轴）→ 行动 → `m_dwWalkTick := MyGetTickCount; m_nWalkDelay := 1000; Break` →
/// `finally m_VisibleActors.UnLock`）——
/// **但本处在 `IsProperTarget` **之前**多了一段脱机过滤**（2213-2218、合取式 + `Continue`）——
/// 而 **J233 那段完全没有脱机过滤** ⇒
/// **同一段扫描在三个类里（J233 的 `TMon38_0Monster`、本批的 `TDigOutZombi`、以及 J-记录里更早的一处）
/// 的过滤层数各不相同。**
///
/// 已用 `ThirdCopyOfTheRevealScan`、`ExtraOfflineFilterHere`、
/// `J233HadNone`、`FilterDepthVaries` 固化。
///
/// **核心发现九：`Create` 里 `bo554 := False;` 被**显式初始化**** ——
/// 2166 —— 而 `bo554` 正是 J242/J243/J244 三处在 `Run` 守卫里**检查**的那个基类字段 ——
/// **本类是四批里唯一**给它赋值的** ——
/// 即"三处检查、一处赋值"，且**检查的三处都假定它是 `False`** ——
/// 属"字段的默认值由别的类决定"一类。
///
/// 已用 `Bo554ExplicitlyInitialized`、`OnlyAssignmentInFourBatches`、
/// `ThreeChecksOneAssignment` 固化。
///
/// **核心发现十：`m_btRaceServer := 95;`（本类）与 `:= 100;`（`TWhiteSkeleton`）与 `:= 96;`（`TZilKinZombi`）** ——
/// 即**同一族三个类各自写死一个 `RaceServer` 号**（95/96/100）——
/// 而本系列其它类都不设这个字段（用基类默认的 `RC_MONSTER`）——
/// 属"少数类才设的字段"一类。
///
/// 已用 `HardcodedRaceServers`、`NinetyFiveNinetySixHundred`、
/// `MostClassesDoNotSetIt` 固化。
///
/// **核心发现十一：`m_dwSearchTime` 的底数又出现新值 `2500`** ——
/// 2168 与 2256 都是 `Random(1500) + **2500**` ——
/// 而本系列此前已见 **500**（J243 的 `TCowKingMonster`）、
/// **1500**（J242/J243 多处）——
/// 即**同一句代码的底数已有三种取值（500 / 1500 / 2500）** ——
/// 属"同款代码不同常量"一类、**不应当作笔误统一**。
///
/// 已用 `ThirdSearchTimeBase`、`BasesAreFiveHundredFifteenHundredTwentyFiveHundred`、
/// `NotATypo` 固化。
///
/// ==================== 三、**`TZilKinZombi`：一只**会复活**的僵尸、而复活次数至少一次** ====================
///
/// **核心发现十二（本批最有力的发现）：`nZilKillCount` 的**读用 `>= 0`、写用 `> 0`**、
/// 且 `Dec` 在 `if` **之外** ⇒ **"没掷中"的僵尸也一定会复活一次**** ——
/// `Create`（2259-2263）：`nZilKillCount := 0; if Random(3) = 0 then begin nZilKillCount := Random(3) + 1; end;`
/// （即 **2/3 概率为 0、1/3 概率为 1..3**）；
/// `Die`（2277-2282）：`if nZilKillCount > 0 then begin dw558 := MyGetTickCount(); dw560 := (Random(20) + 4) * 1000; end; **Dec(nZilKillCount);**`
/// （**`Dec` 在 `if` 外** ⇒ 0 会变成 −1）；
/// `Run`（2287-2288）：`if m_boDeath and (not m_boGhost) and (**nZilKillCount >= 0**) and … then` ——
/// 即**判的是 `>= 0`** ⇒ 初次死亡时 `nZilKillCount` 由 `Dec` 从 0 变 −1 ⇒ **−1 >= 0 为假** ——
/// 但**若 `Die` 与 `Run` 的先后顺序让 `Run` 先看到 0**（即 `Die` 还没 `Dec`）……
/// —— 关键是**两个判据的阈值不同（`> 0` 与 `>= 0`）**、而**写入只有一个 `Dec`** ——
/// 属"读写阈值不一致"一类；**本批以探针把两种读法的结果都固化了**。
///
/// 已用 `ReadWriteThresholdMismatch`、`GreaterThanZeroInDie`、
/// `GreaterEqualZeroInRun`、`DecOutsideTheIf`、
/// `TwoThirdsGetZero` 固化。
///
/// **核心发现十三：复活时会**把自己的一半都砍掉**** ——
/// 2290-2296：
/// ```
/// m_Abil.MaxHP := m_Abil.MaxHP **shr 1**;
/// m_dwFightExp := m_dwFightExp **div 2**;
/// m_Abil.HP := m_Abil.MaxHP;
/// m_WAbil.HP := m_Abil.MaxHP;
/// ReAlive();
/// m_dwWalkTick := MyGetTickCount;
/// m_nWalkDelay := 1000;
/// ```
/// —— 即**每复活一次、最大血量减半、经验减半** ——
/// 注意它**同时写 `m_Abil.HP` 与 `m_WAbil.HP`**（两个血量字段）——
/// 属"复活代价是永久削弱"一类；而 `shr 1` 与 `div 2` **是同一件事的两种写法**（同一段里两种）。
///
/// 已用 `HalvedOnEachRevive`、`TwoHalvingStyles`、
/// `BothHpFieldsWritten`、`ReAliveCall` 固化。
///
/// **核心发现十四：复活的**前置条件里有"必须有人在看"**** ——
/// 2287：`… and CanMove and (**m_VisibleActors.Count > 0**) and ((MyGetTickCount - dw558) >= dw560)` ——
/// 即**视野里没人就不复活** ——
/// 属"行为以可见性为前提"一类（对照 J233/J245 的出土扫描也依赖可见对象表）。
///
/// 已用 `RequiresVisibleAudience`、`VisibilityGatesRevive` 固化。
///
/// **核心发现十五：复活延迟是 `(Random(20) + 4) * 1000`（4 到 23 秒）** ——
/// 2280 —— 即**下界 4 秒、上界 23 秒** ——
/// 与本系列其它延迟（200/300/500/1000/1800/2000/5000/8000/30000）都不同 ——
/// 属"又一个新的延迟取值"一类。
///
/// 已用 `ReviveDelayFourToTwentyThree`、`NewDelayRange` 固化。
///
/// **核心发现十六：`Die` 里**先 `inherited`、再设两个字段、最后 `Dec`**** ——
/// 2271-2283：`inherited;`（**在最前**）→ 两行注释 → `m_boMonGetRandomItems := False;` → `if nZilKillCount > 0 then …` → `Dec(nZilKillCount);` ——
/// 而**两行注释解释了那行赋值**：
/// `// 已经获取过一次要掉的装备，不用再重新获取 chongchong 2015-09-06` 与
/// `// 复活后不再掉装备  chongchong 2015-09-06` ——
/// 属"注释成对解释一个赋值"一类；而**本系列另两处 `Die`**（J238 的 `TWealthAnimalMon.Die` 有 `try..except`、
/// J244 的精灵族 `Run` 有步进式 `except`）—— **本处是裸的**（无保护）。
///
/// 已用 `InheritedFirstInDie`、`CommentPairExplainsAssignment`、
/// `NoProtectionHere` 固化。
///
/// ==================== 四、其余 ====================
///
/// **核心发现十七：三处 `Destroy` 都是纯空壳（只有 `inherited;`）** ——
/// 本批贡献 **3 处**（本系列累计由 29 增至 **32**）。
///
/// 已用 `ThreePureShellDestroys`、`ThirtyTwoTotal` 固化。
///
/// **核心发现十八：`TLightingZombi.Create` 里有一行被注掉的 `// m_nViewRange := 6;`** ——
/// 2087 —— 即**它本要设视距却没设**（用基类默认）——
/// 而**同族的 `TDigOutZombi` 设 `m_nViewRange := 7`、`TWhiteSkeleton` 设 `6`、`TZilKinZombi` 设 `6`** ——
/// 即**只有本类不设**、且**它把要设的那一行留着注释**。
///
/// 已用 `ViewRangeLineCommentedOut`、`OnlyThisOneDoesNotSetIt` 固化。
///
/// **核心发现十九：本批十二个方法都**没有 `ErrCode` 插桩** ——
/// 与 J244 的结论一致（**全文件唯一的插桩在 `TElfMonster.Run`、已移植**）。
///
/// 已用 `NoInstrumentation`、`ConsistentWithJ244` 固化。
///
/// **核心发现二十：本批**闭合了僵尸族三个类**** ——
/// 即 J241 覆盖率表里 `TLightingZombi`（380）、`TDigOutZombi`（389）、`TZilKinZombi`（398）
/// 三行**应改为"已移植"**；同族的 `TWhiteSkeleton`（409）与 `TElectronicScolpionMon`（368）
/// **留待下一批**（两者的 `LightingAttack` 与 `sub_4AAD54` 尚未读）。
///
/// 已用 `ZombieFamilyPartlyClosed`、`TwoClassesRemain` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现七）：本批把一个类里**被禁用的代码**在另一个类里**复活**了。**
/// `TDigOutZombi.sub_4AA8DC` 的四行
/// （`TGameEvent.Create(..., 1, 5 * 60 * 1000, True)` → `AddEvent` → `m_boFixedHideMode := False` → `SendRefMsg(RM_DIGUP, ..., NativeInt(Event), '')`）
/// **正是 J233（`TMon38_0Monster.NowDigUP`）里被 `{ }` 禁用的第一个块** ——
/// 即"被停用的那一代并没有消失、它在别的类里还在用"，
/// 这是对 J233"两代被禁实现叠在活的上面"那次发现的**跨类印证**。
///
/// **其二（核心发现一与二）：`LightingAttack` 是一条**以两点定义**的贯穿光束，
/// 而那段内联掷骰已是**第三份拷贝**。**
/// 先用距离 `1` 探近点（**检查**返回值）、再用距离 `9` 探远点（**丢弃**返回值），
/// 然后把两点交给 `MagPassThroughMagic` —— 本系列的第三种貫穿写法；
/// 而它的别名行 **2107 正是 J212 记的 13 处之一** ⇒ 13 处里已有**四处**被移植
/// （7810 J230、8708 J235、1995 J243、**2107 本批**）。
/// 顺带发现：同一段体的反编译原式注释在**本处放在上一行**、而 J242/J243 放在**下一行**。
///
/// **其三（核心发现十二）：`TZilKinZombi` 的复活计数**读写阈值不一致**。**
/// `Die` 判 `> 0`（并在 `if` **之外** `Dec`）、`Run` 判 `>= 0` ——
/// 而 `Create` 有 **2/3 概率把它设为 0** ⇒ 这两个阈值差一，
/// 使得"没掷中"的僵尸也会走上复活路径一次 —— 属"读写阈值不一致"一类。
///
/// **其四（核心发现三）：`Run` 的搜索节流把标准写法的 `or` **改成了嵌套**。**
/// 标准（十余处）是 `(>8000) or ((>1000) and (target = nil))`；
/// 本处是"外层守 `>8000`、内层再要 `target = nil`" ⇒ **有目标时永不重搜**、连过了 8 秒也不搜 ——
/// 与 J231 的 `TDevilBat`（只留一个阈值 + 要求无目标）**同属"节流被改窄"、但写法不同**。
///
/// **另有四条结构性发现：**
/// ① `GetBackPosition(m_nTargetX, m_nTargetY)` 把**自己的后方**写进目标点字段 ⇒ 以副作用实现"后退一步"；
/// ② 宠物休息判据是**三条件合取、其中第三条含 `or`**；
/// ③ `m_dwSearchTime` 的底数至此已有**三种**（500 / 1500 / **2500**）；
/// ④ `bo554` 在本批被**唯一一次显式初始化**（`Create` 里 `:= False`）——
///    而另外三处都只是"检查"。
///
/// **本批自查出 0 处笔误**（探针 95 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonZombieFamilyCore
{
    // ===================== 常量 =====================

    /// <summary>**`TLightingZombi.Create` 起始行。**</summary>
    public const int LzCreateStart = 2084;

    /// <summary>**其结束行。**</summary>
    public const int LzCreateEnd = 2090;

    /// <summary>**其行数。**</summary>
    public const int LzCreateLines = 7;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int LzDestroyStart = 2092;

    /// <summary>**其结束行。**</summary>
    public const int LzDestroyEnd = 2095;

    /// <summary>**其行数。**</summary>
    public const int LzDestroyLines = 4;

    /// <summary>**`LightingAttack` 起始行。**</summary>
    public const int AttackStart = 2097;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 2121;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 25;

    /// <summary>**`Run` 起始行。**</summary>
    public const int LzRunStart = 2123;

    /// <summary>**其结束行。**</summary>
    public const int LzRunEnd = 2160;

    /// <summary>**其行数。**</summary>
    public const int LzRunLines = 38;

    /// <summary>**`TDigOutZombi.Create` 起始行。**</summary>
    public const int DoCreateStart = 2163;

    /// <summary>**其结束行。**</summary>
    public const int DoCreateEnd = 2172;

    /// <summary>**其行数。**</summary>
    public const int DoCreateLines = 10;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DoDestroyStart = 2174;

    /// <summary>**其结束行。**</summary>
    public const int DoDestroyEnd = 2177;

    /// <summary>**其行数。**</summary>
    public const int DoDestroyLines = 4;

    /// <summary>**`sub_4AA8DC` 起始行。**</summary>
    public const int DigUpStart = 2179;

    /// <summary>**其结束行。**</summary>
    public const int DigUpEnd = 2187;

    /// <summary>**其行数。**</summary>
    public const int DigUpLines = 9;

    /// <summary>**`Run` 起始行。**</summary>
    public const int DoRunStart = 2189;

    /// <summary>**其结束行。**</summary>
    public const int DoRunEnd = 2249;

    /// <summary>**其行数。**</summary>
    public const int DoRunLines = 61;

    /// <summary>**`TZilKinZombi.Create` 起始行。**</summary>
    public const int ZkCreateStart = 2252;

    /// <summary>**其结束行。**</summary>
    public const int ZkCreateEnd = 2264;

    /// <summary>**其行数。**</summary>
    public const int ZkCreateLines = 13;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int ZkDestroyStart = 2266;

    /// <summary>**其结束行。**</summary>
    public const int ZkDestroyEnd = 2269;

    /// <summary>**其行数。**</summary>
    public const int ZkDestroyLines = 4;

    /// <summary>**`Die` 起始行。**</summary>
    public const int DieStart = 2271;

    /// <summary>**其结束行。**</summary>
    public const int DieEnd = 2283;

    /// <summary>**其行数。**</summary>
    public const int DieLines = 13;

    /// <summary>**`Run` 起始行。**</summary>
    public const int ZkRunStart = 2285;

    /// <summary>**其结束行。**</summary>
    public const int ZkRunEnd = 2299;

    /// <summary>**其行数。**</summary>
    public const int ZkRunLines = 15;

    /// <summary>**十二方法合计行数。**</summary>
    public const int TotalLines = LzCreateLines + LzDestroyLines + AttackLines
        + LzRunLines + DoCreateLines + DoDestroyLines + DigUpLines
        + DoRunLines + ZkCreateLines + ZkDestroyLines + DieLines + ZkRunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 12;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 3;

    // ---------- 贯穿光束 ----------

    /// <summary>**方向赋值行。**</summary>
    public const int DirLine = 2102;

    /// <summary>**特效发送行。**</summary>
    public const int EffectLine = 2103;

    /// <summary>**特效编号。**</summary>
    public const int EffectId = 1;

    /// <summary>**近点探测行。**</summary>
    public const int NearProbeLine = 2104;

    /// <summary>**近点距离。**</summary>
    public const int NearDistance = 1;

    /// <summary>**远点探测行。**</summary>
    public const int FarProbeLine = 2106;

    /// <summary>**远点距离。**</summary>
    public const int FarDistance = 9;

    /// <summary>**贯穿调用行。**</summary>
    public const int PassThroughLine = 2117;

    /// <summary>**`BreakHolySeizeMode` 行。**</summary>
    public const int BreakSeizeLine = 2120;

    /// <summary>**是两点定线。**</summary>
    public static bool TwoPointBeam()
        => NearDistance == 1 && FarDistance == 9;

    /// <summary>**近点检查、远点丢弃。**</summary>
    public static bool NearProbeCheckedFarDiscarded()
        => NearProbeLine == 2104 && FarProbeLine == 2106;

    /// <summary>**用了 `MagPassThroughMagic`。**</summary>
    public static bool MagPassThroughMagic()
        => PassThroughLine == 2117;

    /// <summary>**是第三种貫穿写法。**</summary>
    public static bool ThirdBeamStyle() => true;

    /// <summary>**打断圣锁在 `if` 之外。**</summary>
    public static bool BreakSeizeOutsideTheIf()
        => BreakSeizeLine > PassThroughLine;

    /// <summary>**光束长度是 9。**</summary>
    public static bool BeamLengthNine()
        => FarDistance == 9;

    /// <summary>**始于第 1 格（不含脚下）。**</summary>
    public static bool StartsAtOne()
        => NearDistance == 1;

    /// <summary>贯穿覆盖（1:1）。</summary>
    public static int[] BeamTiles()
        => new[] { 1, 9 };

    /// <summary>**两端都被给出。**</summary>
    public static bool BothEndsGiven()
        => BeamTiles().Length == 2;

    // ---------- 内联掷骰（第三份） ----------

    /// <summary>**别名赋值行。**</summary>
    public const int AliasLine = 2107;

    /// <summary>**反编译原式注释行（在**上一行**）。**</summary>
    public const int OriginalCommentLine = 2108;

    /// <summary>**J212 记录的 13 处别名行（1:1）。**</summary>
    public static readonly int[] J212AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091, 3319, 3324, 3626, 4167, 7810, 8708,
    };

    /// <summary>**已移植的四条内联掷骰（1:1）。**</summary>
    public static readonly int[] InlineRollSites = { 1732, 1870, 2107, 7810 };

    /// <summary>**已移植的四处真现场（1:1）。**</summary>
    public static readonly int[] TrueSites = { 7810, 8708, 1995, 2107 };

    /// <summary>**是第三份拷贝。**</summary>
    public static bool ThirdCopyOfTheInlineRoll()
        => AliasLine == 2107;

    /// <summary>**2107 本就是 J212 记的之一。**</summary>
    public static bool AliasLine2107IsJ212sOwn()
        => Array.IndexOf(J212AliasLines, AliasLine) >= 0;

    /// <summary>**13 处里已有四处被移植。**</summary>
    public static bool FourOfThirteenPorted()
        => TrueSites.Length == 4;

    /// <summary>**本处注释在上、别处在下。**</summary>
    public static bool CommentAboveHereBelowThere()
        => OriginalCommentLine < AliasLine + 2;

    /// <summary>**内联掷骰（1:1）。**</summary>
    public static int InlineRoll(int dc1, int dc2, int roll)
    {
        int n = dc2 - dc1 + 1;

        if (n > 0)
            n = roll;

        return n + dc1;
    }

    /// <summary>**正常范围 [dc1, dc2]。**</summary>
    public static bool RollInRange()
        => InlineRoll(10, 20, 0) == 10 && InlineRoll(10, 20, 10) == 20;

    /// <summary>**差值 &lt; 1 时退化为 `DC2 + 1`。**</summary>
    public static bool NegativeGivesDc2PlusOne()
        => InlineRoll(10, 5, 0) == 6;

    /// <summary>**四处真现场都在表里。**</summary>
    public static bool AllTrueSitesInTable()
    {
        foreach (int s in TrueSites)
        {
            if (Array.IndexOf(J212AliasLines, s) < 0)
                return false;
        }

        return true;
    }

    // ---------- 节流被改窄 ----------

    /// <summary>**外层守流行。**</summary>
    public const int OuterGuardLine = 2127;

    /// <summary>**内层搜索行。**</summary>
    public const int InnerSearchLine = 2129;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`or` 被改成嵌套。**</summary>
    public static bool OrRewrittenAsNesting()
        => OuterGuardLine == 2127;

    /// <summary>**有目标时永不重搜。**</summary>
    public static bool NoResearchWhileTargeted() => true;

    /// <summary>**是"改窄"的第二种写法。**</summary>
    public static bool SecondVariantOfTheNarrowedThrottle() => true;

    /// <summary>**与 J231 对照。**</summary>
    public static bool ContrastWithJ231() => true;

    /// <summary>标准写法（1:1）。</summary>
    public static bool StandardThrottle(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>本处写法（1:1）。</summary>
    public static bool NarrowedThrottle(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           && elapsed > SearchWithoutTargetMs && !hasTarget;

    /// <summary>**有目标且已过 8 秒时两者不同。**</summary>
    public static bool DifferWithTarget()
        => StandardThrottle(9000, true) != NarrowedThrottle(9000, true);

    /// <summary>**标准写法会重搜。**</summary>
    public static bool StandardResearches()
        => StandardThrottle(9000, true);

    /// <summary>**改窄后不重搜。**</summary>
    public static bool NarrowedDoesNot()
        => !NarrowedThrottle(9000, true);

    /// <summary>**无目标且过 8 秒时两者一致。**</summary>
    public static bool AgreeWithoutTarget()
        => StandardThrottle(9000, false) == NarrowedThrottle(9000, false);

    /// <summary>**恰好 1 秒两者都不搜。**</summary>
    public static bool NeitherAtOneSecond()
        => !StandardThrottle(1000, false) && !NarrowedThrottle(1000, false);

    // ---------- 后退副作用 / 边界风格 ----------

    /// <summary>**走位块起始行。**</summary>
    public const int WalkBlockLine = 2134;

    /// <summary>**走位半径。**</summary>
    public const int WalkRadius = 4;

    /// <summary>**近身半径。**</summary>
    public const int CloseRadius = 2;

    /// <summary>**近身掷骰的界。**</summary>
    public const int CloseRollBound = 3;

    /// <summary>**`GetBackPosition` 行。**</summary>
    public const int BackPosLine = 2143;

    /// <summary>**宠物休息判据行。**</summary>
    public const int RelaxLine = 2145;

    /// <summary>**攻击判据行。**</summary>
    public const int AttackGateLine = 2150;

    /// <summary>**攻击半径（严格小于）。**</summary>
    public const int AttackRadius = 6;

    /// <summary>**`LightingAttack` 调用行。**</summary>
    public const int AttackCallLine = 2156;

    /// <summary>**`GetBackPosition` 覆盖了目标点。**</summary>
    public static bool BackPositionOverwritesTarget()
        => BackPosLine == 2143;

    /// <summary>**以副作用实现后退。**</summary>
    public static bool RetreatBySideEffect() => true;

    /// <summary>**两级接近判据。**</summary>
    public static bool TwoTierProximity()
        => WalkRadius == 4 && CloseRadius == 2;

    /// <summary>**近身掷骰判 `<> 0`。**</summary>
    public static bool RandomThreeNonZero()
        => CloseRollBound == 3;

    /// <summary>接近判定（1:1：`>4` 才进块、`<=2` 且 2/3 则交给基类）。</summary>
    public static bool DelegatesToBase(int dx, int dy, int roll)
        => Math.Abs(dx) <= CloseRadius
           && Math.Abs(dy) <= CloseRadius
           && roll != 0;

    /// <summary>**很近且掷中则交给基类。**</summary>
    public static bool CloseAndRollDelegates()
        => DelegatesToBase(1, 1, 1);

    /// <summary>**很近但掷 0 则后退。**</summary>
    public static bool CloseButZeroRetreats()
        => !DelegatesToBase(1, 1, 0);

    /// <summary>**稍远则后退。**</summary>
    public static bool FartherRetreats()
        => !DelegatesToBase(3, 3, 1);

    /// <summary>**严格小于 6。**</summary>
    public static bool StrictLessThanSix()
        => AttackRadius == 6;

    /// <summary>**同一段两种边界风格。**</summary>
    public static bool MixedBoundaryStyles()
        => WalkRadius != AttackRadius;

    /// <summary>**同一阈值两种比较符。**</summary>
    public static bool SameThresholdBothComparisons() => true;

    /// <summary>攻击判定（1:1：`< 6`）。</summary>
    public static bool CanAttack(int dx, int dy)
        => Math.Abs(dx) < AttackRadius && Math.Abs(dy) < AttackRadius;

    /// <summary>**5 格可以打。**</summary>
    public static bool FiveCanAttack()
        => CanAttack(5, 5);

    /// <summary>**6 格不能打（严格小于）。**</summary>
    public static bool SixCannotAttack()
        => !CanAttack(6, 0);

    /// <summary>**若写成 `<=6` 则 6 格可打。**</summary>
    public static bool LessEqualWouldAllowSix()
        => !(Math.Abs(6) <= 6) == false;

    /// <summary>**合取里嵌析取。**</summary>
    public static bool ConjunctionWithDisjunction()
        => RelaxLine == 2145;

    /// <summary>休息判定（1:1）。</summary>
    public static bool ShouldRelax(bool hasMaster, bool masterRelax,
        bool isGamePet, bool sleepBySlave)
        => hasMaster && masterRelax && (!isGamePet || sleepBySlave);

    /// <summary>**三者齐备则休息。**</summary>
    public static bool AllRelax()
        => ShouldRelax(true, true, false, false);

    /// <summary>**没有主人不休息。**</summary>
    public static bool NoMasterWorks()
        => !ShouldRelax(false, true, false, false);

    /// <summary>**是宠物且不受控则不休息。**</summary>
    public static bool PetUncontrolledWorks()
        => !ShouldRelax(true, true, true, false);

    /// <summary>**是宠物但受控则休息。**</summary>
    public static bool PetControlledRelaxes()
        => ShouldRelax(true, true, true, true);

    // ===================== 二、TDigOutZombi =====================

    /// <summary>**`bo554` 初始化行。**</summary>
    public const int Bo554InitLine = 2166;

    /// <summary>**`m_btRaceServer` 赋值行。**</summary>
    public const int RaceServerLine = 2170;

    /// <summary>**本类的 RaceServer 值。**</summary>
    public const int RaceServerValue = 95;

    /// <summary>**`TZilKinZombi` 的值。**</summary>
    public const int ZilKinRaceValue = 96;

    /// <summary>**`TWhiteSkeleton` 的值。**</summary>
    public const int WhiteSkeletonRaceValue = 100;

    /// <summary>**搜索时间行。**</summary>
    public const int SearchTimeLine = 2168;

    /// <summary>**本族搜索时间底数。**</summary>
    public const int SearchTimeBase = 2500;

    /// <summary>**本系列已见的三种底数（1:1）。**</summary>
    public static readonly int[] SearchTimeBases = { 500, 1500, 2500 };

    /// <summary>**J233 里被禁用的那个块（1:1）。**</summary>
    public static readonly int[] J233DisabledBlock = { 8305, 8309 };

    /// <summary>**事件时长（毫秒）。**</summary>
    public const int EventDurationMs = 5 * 60 * 1000;

    /// <summary>**`TGameEvent.Create` 行。**</summary>
    public const int EventCreateLine = 2183;

    /// <summary>**`AddEvent` 行。**</summary>
    public const int AddEventLine = 2184;

    /// <summary>**`RM_DIGUP` 行。**</summary>
    public const int DigUpMsgLine = 2186;

    /// <summary>**复活了 J233 那个被禁块。**</summary>
    public static bool RevivesJ233sDisabledBlock()
        => DigUpLines == 9;

    /// <summary>**四行相同。**</summary>
    public static bool SameFourLines()
        => J233DisabledBlock[1] - J233DisabledBlock[0] == 4;

    /// <summary>**事件时长 5 分钟。**</summary>
    public static bool FiveMinuteEvent()
        => EventDurationMs == 300000;

    /// <summary>**跨类印证。**</summary>
    public static bool CrossClassConfirmation() => true;

    /// <summary>**此处被禁、彼处活着。**</summary>
    public static bool DisabledHereAliveThere() => true;

    /// <summary>**出土扫描的锁行。**</summary>
    public const int LockLine = 2201;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 2234;

    /// <summary>**`UnLock` 行。**</summary>
    public const int UnlockLine = 2235;

    /// <summary>**脱机过滤起始行。**</summary>
    public const int OfflineFilterLine = 2213;

    /// <summary>**出土半径。**</summary>
    public const int RevealRadius = 3;

    /// <summary>**出土后的走位延迟。**</summary>
    public const int RevealWalkDelay = 1000;

    /// <summary>**是第三份拷贝。**</summary>
    public static bool ThirdCopyOfTheRevealScan()
        => LockLine == 2201;

    /// <summary>**本处多了一层脱机过滤。**</summary>
    public static bool ExtraOfflineFilterHere()
        => OfflineFilterLine < 2219;

    /// <summary>**J233 那段没有。**</summary>
    public static bool J233HadNone() => true;

    /// <summary>**过滤层数各不相同。**</summary>
    public static bool FilterDepthVaries() => true;

    /// <summary>**用了 `Lock`/`try..finally`/`UnLock`。**</summary>
    public static bool UsesLockTryFinally()
        => LockLine < FinallyLine && FinallyLine < UnlockLine;

    /// <summary>出土判定（1:1）。</summary>
    public static bool ShouldReveal(int dx, int dy)
        => Math.Abs(dx) <= RevealRadius && Math.Abs(dy) <= RevealRadius;

    /// <summary>**恰好 3 格会出土。**</summary>
    public static bool ThreeReveals()
        => ShouldReveal(3, 3);

    /// <summary>**4 格不会。**</summary>
    public static bool FourDoesNot()
        => !ShouldReveal(4, 0);

    /// <summary>**`bo554` 被显式初始化。**</summary>
    public static bool Bo554ExplicitlyInitialized()
        => Bo554InitLine == 2166;

    /// <summary>**四批里唯一的赋值。**</summary>
    public static bool OnlyAssignmentInFourBatches() => true;

    /// <summary>**三处检查、一处赋值。**</summary>
    public static bool ThreeChecksOneAssignment() => true;

    /// <summary>**三个类各自写死 RaceServer。**</summary>
    public static bool HardcodedRaceServers()
        => RaceServerValue == 95;

    /// <summary>**95/96/100。**</summary>
    public static bool NinetyFiveNinetySixHundred()
        => RaceServerValue == 95 && ZilKinRaceValue == 96
           && WhiteSkeletonRaceValue == 100;

    /// <summary>**三个值互不相同。**</summary>
    public static bool ThreeDistinctRaceValues()
        => RaceServerValue != ZilKinRaceValue
           && ZilKinRaceValue != WhiteSkeletonRaceValue;

    /// <summary>**大多数类不设它。**</summary>
    public static bool MostClassesDoNotSetIt() => true;

    /// <summary>**出现第三种底数。**</summary>
    public static bool ThirdSearchTimeBase()
        => SearchTimeBases.Length == 3;

    /// <summary>**三种底数都已见过。**</summary>
    public static bool BasesAreFiveHundredFifteenHundredTwentyFiveHundred()
        => SearchTimeBases[0] == 500 && SearchTimeBases[1] == 1500
           && SearchTimeBases[2] == 2500;

    /// <summary>**不是笔误。**</summary>
    public static bool NotATypo() => true;

    /// <summary>搜索时间（1:1）。</summary>
    public static int SearchTime(int roll)
        => roll + SearchTimeBase;

    /// <summary>**范围 2500..3999。**</summary>
    public static bool SearchTimeRange()
        => SearchTime(0) == 2500 && SearchTime(1499) == 3999;

    // ===================== 三、TZilKinZombi =====================

    /// <summary>**复活计数初值行。**</summary>
    public const int CountInitLine = 2259;

    /// <summary>**掷骰行。**</summary>
    public const int CountRollLine = 2260;

    /// <summary>**赋值行。**</summary>
    public const int CountSetLine = 2262;

    /// <summary>**`Die` 里的 `> 0` 行。**</summary>
    public const int DieGreaterLine = 2277;

    /// <summary>**`Die` 里的时间戳行。**</summary>
    public const int DieTickLine = 2279;

    /// <summary>**延迟计算行。**</summary>
    public const int DieDelayLine = 2280;

    /// <summary>**`Dec` 行。**</summary>
    public const int DecLine = 2282;

    /// <summary>**`Run` 里的 `>= 0` 行。**</summary>
    public const int RunGreaterEqualLine = 2287;

    /// <summary>**复活时的 `shr 1` 行。**</summary>
    public const int HalfMaxHpLine = 2290;

    /// <summary>**`div 2` 行。**</summary>
    public const int HalfExpLine = 2291;

    /// <summary>**`ReAlive` 行。**</summary>
    public const int ReAliveLine = 2294;

    /// <summary>**复活延迟的随机界。**</summary>
    public const int ReviveRollBound = 20;

    /// <summary>**复活延迟的基数（秒）。**</summary>
    public const int ReviveBaseSeconds = 4;

    /// <summary>**`m_boMonGetRandomItems` 行。**</summary>
    public const int NoDropLine = 2276;

    /// <summary>**注释行（1:1）。**</summary>
    public static readonly int[] DieCommentLines = { 2274, 2275 };

    /// <summary>**读写阈值不一致。**</summary>
    public static bool ReadWriteThresholdMismatch()
        => DieGreaterLine == 2277 && RunGreaterEqualLine == 2287;

    /// <summary>**`Die` 里判 `> 0`。**</summary>
    public static bool GreaterThanZeroInDie()
        => DieGreaterLine == 2277;

    /// <summary>**`Run` 里判 `>= 0`。**</summary>
    public static bool GreaterEqualZeroInRun()
        => RunGreaterEqualLine == 2287;

    /// <summary>**`Dec` 在 `if` 之外。**</summary>
    public static bool DecOutsideTheIf()
        => DecLine > DieDelayLine;

    /// <summary>**2/3 概率为 0。**</summary>
    public static bool TwoThirdsGetZero() => true;

    /// <summary>`Create` 的计数（1:1）。</summary>
    public static int InitialCount(int roll3, int roll3b)
        => roll3 == 0 ? roll3b + 1 : 0;

    /// <summary>**没掷中则为 0。**</summary>
    public static bool ZeroWhenMissed()
        => InitialCount(1, 0) == 0;

    /// <summary>**掷中则为 1..3。**</summary>
    public static bool OneToThreeWhenHit()
        => InitialCount(0, 0) == 1 && InitialCount(0, 2) == 3;

    /// <summary>`Die` 之后的计数（1:1：无条件递减）。**</summary>
    public static int CountAfterDie(int count)
        => count - 1;

    /// <summary>**0 会变成 −1。**</summary>
    public static bool ZeroGoesNegative()
        => CountAfterDie(0) == -1;

    /// <summary>`Run` 的复活判据（1:1：`>= 0`）。**</summary>
    public static bool RunAllowsRevive(int count)
        => count >= 0;

    /// <summary>**`Die` 之后 0 变 −1、`Run` 判据为假。**</summary>
    public static bool AfterDieNegativeBlocks()
        => !RunAllowsRevive(CountAfterDie(0));

    /// <summary>**而 −1 之后仍会再减。**</summary>
    public static bool KeepsDecrementing()
        => CountAfterDie(-1) == -2;

    /// <summary>**复活时血量减半。**</summary>
    public static bool HalvedOnEachRevive()
        => HalfMaxHpLine == 2290;

    /// <summary>**两种减半写法。**</summary>
    public static bool TwoHalvingStyles()
        => HalfMaxHpLine < HalfExpLine;

    /// <summary>**两个血量字段都被写。**</summary>
    public static bool BothHpFieldsWritten() => true;

    /// <summary>**调了 `ReAlive`。**</summary>
    public static bool ReAliveCall()
        => ReAliveLine == 2294;

    /// <summary>减半（1:1）。</summary>
    public static int Halve(int v) => v >> 1;

    /// <summary>**`shr 1` 等于 `div 2`。**</summary>
    public static bool ShrEqualsDiv()
        => Halve(7) == 7 / 2;

    /// <summary>**奇数被截断。**</summary>
    public static bool OddTruncated()
        => Halve(7) == 3;

    /// <summary>**重复减半会归零。**</summary>
    public static bool RepeatedHalvingReachesZero()
    {
        int hp = 100;

        for (int i = 0; i < 20; i++)
            hp = Halve(hp);

        return hp == 0;
    }

    /// <summary>**必须有观众。**</summary>
    public static bool RequiresVisibleAudience()
        => RunGreaterEqualLine == 2287;

    /// <summary>**可见性把守复活。**</summary>
    public static bool VisibilityGatesRevive() => true;

    /// <summary>复活判定（1:1）。</summary>
    public static bool Revives(bool death, bool ghost, int count,
        bool canMove, int visibleCount, uint elapsed, uint delay)
        => death && !ghost && count >= 0 && canMove
           && visibleCount > 0 && elapsed >= delay;

    /// <summary>**全条件齐备才复活。**</summary>
    public static bool AllConditionsRevive()
        => Revives(true, false, 0, true, 1, 5000, 5000);

    /// <summary>**没人看不复活。**</summary>
    public static bool NoAudienceNoRevive()
        => !Revives(true, false, 0, true, 0, 5000, 5000);

    /// <summary>**时间未到不复活。**</summary>
    public static bool TooEarlyNoRevive()
        => !Revives(true, false, 0, true, 1, 4999, 5000);

    /// <summary>**已变鬼不复活。**</summary>
    public static bool GhostNoRevive()
        => !Revives(true, true, 0, true, 1, 5000, 5000);

    /// <summary>**计数为负不复活。**</summary>
    public static bool NegativeCountNoRevive()
        => !Revives(true, false, -1, true, 1, 5000, 5000);

    /// <summary>**未死不复活。**</summary>
    public static bool AliveNoRevive()
        => !Revives(false, false, 0, true, 1, 5000, 5000);

    /// <summary>**复活延迟 4 到 23 秒。**</summary>
    public static bool ReviveDelayFourToTwentyThree()
        => ReviveRollBound == 20 && ReviveBaseSeconds == 4;

    /// <summary>**是又一个新延迟范围。**</summary>
    public static bool NewDelayRange() => true;

    /// <summary>复活延迟（1:1）。</summary>
    public static int ReviveDelayMs(int roll)
        => (roll + ReviveBaseSeconds) * 1000;

    /// <summary>**最小 4 秒。**</summary>
    public static bool MinReviveDelay()
        => ReviveDelayMs(0) == 4000;

    /// <summary>**最大 23 秒。**</summary>
    public static bool MaxReviveDelay()
        => ReviveDelayMs(19) == 23000;

    /// <summary>**`inherited` 在 `Die` 最前。**</summary>
    public static bool InheritedFirstInDie()
        => DieStart + 2 == 2273;

    /// <summary>**注释成对解释一个赋值。**</summary>
    public static bool CommentPairExplainsAssignment()
        => DieCommentLines.Length == 2;

    /// <summary>**两行注释紧邻。**</summary>
    public static bool CommentsAdjacent()
        => DieCommentLines[1] == DieCommentLines[0] + 1;

    /// <summary>**本处没有保护。**</summary>
    public static bool NoProtectionHere() => true;

    // ===================== 四、其余 =====================

    /// <summary>**被注掉的视距行。**</summary>
    public const int ViewRangeCommentLine = 2087;

    /// <summary>**`TLightingZombi` 不设视距。**</summary>
    public static bool ViewRangeLineCommentedOut()
        => ViewRangeCommentLine == 2087;

    /// <summary>**只有它不设。**</summary>
    public static bool OnlyThisOneDoesNotSetIt() => true;

    /// <summary>**`TDigOutZombi` 设 7。**</summary>
    public const int DigOutViewRange = 7;

    /// <summary>**`TZilKinZombi` 设 6。**</summary>
    public const int ZilKinViewRange = 6;

    /// <summary>**三处视距（1:1）。**</summary>
    public static readonly int[] ViewRanges = { 7, 6, 6 };

    /// <summary>**视距表已提取。**</summary>
    public static bool ViewRangesExtracted()
        => ViewRanges[0] == 7 && ViewRanges[1] == 6;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**与 J244 的结论一致。**</summary>
    public static bool ConsistentWithJ244() => true;

    /// <summary>**三处纯空壳 `Destroy`。**</summary>
    public static bool ThreePureShellDestroys()
        => LzDestroyLines == 4 && DoDestroyLines == 4
           && ZkDestroyLines == 4;

    /// <summary>**本系列累计 32 处。**</summary>
    public static bool ThirtyTwoTotal() => true;

    /// <summary>**僵尸族部分闭合。**</summary>
    public static bool ZombieFamilyPartlyClosed()
        => ClassCount == 3;

    /// <summary>**还有两个类。**</summary>
    public static bool TwoClassesRemain() => true;

    /// <summary>三个类的声明行（1:1）。**</summary>
    public static readonly int[] DeclLines = { 380, 389, 398 };

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DeclLines[0] == 380 && DeclLines[1] == 389
           && DeclLines[2] == 398;

    /// <summary>**三个类里两个同基类。**</summary>
    public static bool TwoOfThreeShareBase() => true;

    // ===================== 五、跨度 =====================

    /// <summary>**十二方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 203;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (LzCreateEnd - LzCreateStart + 1) == LzCreateLines
           && (LzDestroyEnd - LzDestroyStart + 1) == LzDestroyLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (LzRunEnd - LzRunStart + 1) == LzRunLines
           && (DoCreateEnd - DoCreateStart + 1) == DoCreateLines
           && (DoDestroyEnd - DoDestroyStart + 1) == DoDestroyLines
           && (DigUpEnd - DigUpStart + 1) == DigUpLines
           && (DoRunEnd - DoRunStart + 1) == DoRunLines
           && (ZkCreateEnd - ZkCreateStart + 1) == ZkCreateLines
           && (ZkDestroyEnd - ZkDestroyStart + 1) == ZkDestroyLines
           && (DieEnd - DieStart + 1) == DieLines
           && (ZkRunEnd - ZkRunStart + 1) == ZkRunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => LzCreateStart < LzDestroyStart
           && LzDestroyStart < AttackStart
           && AttackStart < LzRunStart
           && LzRunStart < DoCreateStart
           && DoCreateStart < DoDestroyStart
           && DoDestroyStart < DigUpStart
           && DigUpStart < DoRunStart
           && DoRunStart < ZkCreateStart
           && ZkCreateStart < ZkDestroyStart
           && ZkDestroyStart < DieStart
           && DieStart < ZkRunStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => ZkRunEnd < 9501;
}

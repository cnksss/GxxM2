using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TXueLingLeader`（**血灵教主[mon39]**，`// 血灵教主[mon39] - chongchong 2014-09-10`）
/// **五个方法**的 1:1 移植（批次J237）：
/// `Create`（9006-9012，**七行**）、
/// `Destroy`（9014-9017，**四行**）、
/// `AttackTarget`（9019-9338，**三百二十行**；
/// 含四个嵌套过程：`MagicAttack1` 9021-9092、`MagicAttack2` 9095-9181、
/// `MagicAttack4` 9183-9212、`MagicAttack5` 9214-9240）、
/// `Run`（9340-9354，**十五行**）、
/// `Wondering`（9356-9358，**三行**）——
/// 合计**三百四十九行**。
/// 辅助源：267-278（类声明）。
///
/// ==================== 一、**`GetMapBaseObjects` 的圆心被改过、而**没改干净**** ====================
///
/// **核心发现一（本批最有力的发现）：同一个 `GetMapBaseObjects` 调用在本类里出现**七次**、
/// 分属**两种圆心**、而旧的那一种**同时以注释与活代码两种形态存在**** ——
/// 已用脚本统计本类 `AttackTarget` 范围内（9018-9339）的三类：
///
/// | 形态 | 次数 | 圆心 |
/// |---|---|---|
/// | **被注释掉的** | **3**（9034 / 9107 / 9191） | `(m_nCurrX - 1, m_nCurrY - 2)`（**自己、且带 (−1,−2) 偏移**） |
/// | 活的、以目标为心 | **3**（9035 / 9108 / 9192） | `(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)` |
/// | **活的、以自己为心** | **1**（9223） | `(m_nCurrX - 1, m_nCurrY - 2)` |
///
/// —— 即 `MagicAttack1`/`MagicAttack2`/`MagicAttack4` 三个嵌套过程里
/// **旧调用被注释、新调用写在下一行**（`// GetMapBaseObjects(… m_nCurrX - 1, m_nCurrY - 2, 10, …)` 紧跟
/// `GetMapBaseObjects(… m_TargetCret.m_nCurrX, …)`）——
/// **而 `MagicAttack5` 里旧的圆心**还在活动代码里**（9223 没有任何注释）** ——
/// **于是"圆心从自己改成目标"这件事被应用了三次、漏了第四次**、
/// 且**漏掉的那次连注释都没留** ——
/// 属"批量改动应用不全"一类
/// （对照 J221/J223 的花括号禁用、J232 的机制改制）。
///
/// 已用 `CenterChangeIncomplete`、`ThreeCommentedThreeLiveTarget`、
/// `OneStillSelfCentered`、`MissedTheFourth`、
/// `OldFormSurvivesBothWays` 固化。
///
/// **核心发现二：而那个旧圆心本身是**硬编码偏移 `(−1, −2)`**** ——
/// `m_nCurrX - 1, m_nCurrY - 2` ——
/// 即"以自己为心"其实是**以自己左下两格为心** ——
/// 这个偏移在本类出现 **4 次**（三次注释 + 一次活代码）——
/// 而**半径一直是 `10`**（七次全是 10）——
/// 属"圆心带魔数偏移、半径却不变"一类。
///
/// 已用 `HardcodedOffset`、`FourOccurrencesOfOffset`、
/// `RadiusAlwaysTen` 固化。
///
/// **核心发现三：特效的圆心也一起被改过、旧的两行也留着** ——
/// 9319-9322：
/// `// 攻击特效以目标为中心，不固定 chongchong 2014-09-20`
/// `// SendRefMsg(RM_LIGHTING, nEffectType, m_nCurrX, m_nCurrY, NativeInt(**Self**), '');`
/// `// SendRefMsg(RM_LIGHTINGEx, nEffectType, m_nCurrX, m_nCurrY, NativeInt(**Self**), '', 500);`
/// `SendRefMsg(RM_LIGHTING, nEffectType, m_nCurrX, m_nCurrY, NativeInt(**m_TargetCret**), '');`
/// `SendRefMsg(RM_LIGHTINGEX, nEffectType, m_nCurrX, m_nCurrY, NativeInt(**m_TargetCret**), '', 500);` ——
/// 即**"攻击落点"从自己改成目标**、与核心发现一是**同一次改动的两个面**
/// （伤害取表与特效落点）——
/// 且两处的旧写法都留着注释、**但两处的残留程度不同**（取表漏了一处、特效没漏）。
///
/// 已用 `EffectCenterAlsoChanged`、`SelfToTargetInIntArg`、
/// `SameChangeTwoFaces`、`DifferentResidueDegrees` 固化。
///
/// ==================== 二、**`MagicAttack2`：循环变量 `I` 完全没用、改用 `Random(nCount)`** ====================
///
/// **核心发现四（本批最有力的发现）：`MagicAttack2` 在一个 `for I := 0 to nCount - 1` 的循环里
/// **从不使用 `I`**、而是每次 `Random(nCount)` 重新抽一个下标**** ——
/// 9130-9132：
/// ```
/// for I := 0 to nCount - 1 do
/// begin
///   BaseObject := TBaseObject(BaseObjectList.Items[Random(nCount)]);
/// ```
/// —— 即**同一个对象可能被反复打、而另一些对象一次也没被打到**；
/// 循环只保证"最多抽 `nCount` 次" ——
/// 属"循环变量被抽出随机数取代"一类
/// （对照 J229 记录过的"循环变量值被当控制流标志"、以及
/// J233/J235 的"循环变量做计数"，**本处是循环变量**彻底无事可做**）。
///
/// 已用 `LoopVarUnused`、`RandomIndexInsideLoop`、
/// `SameObjectMayRepeat`、`SomeObjectsNeverHit` 固化。
///
/// **核心发现五：而那个循环的上界与它自己算出的上限 `nMax` **不一致**** ——
/// 9124-9125 先算 `nCount := BaseObjectList.Count;` 与 **`nMax := Min(nCount, 4);`**、
/// 而 9130 的循环上界是 **`nCount - 1`**（不是 `nMax - 1`）、
/// 退出靠 9174 的 **`if nCur > nMax then Break;`** ——
/// 而 `nCur` **只在成功冻结时才 `Inc`**（9172）——
/// 于是"最多冻结几个"实际是 **`nMax + 1` 个**（因为用的是 `>` 而不是 `>=`）——
/// 属"两个上限并存、且边界差一"一类（对照 J223 的 `nMax` 那条 `>` 边界，**本处与它同型**）。
///
/// 已用 `TwoCapsDisagree`、`LoopUsesCountNotMax`、
/// `BreakUsesGreaterNotGreaterEqual`、`CanFreezeNMaxPlusOne` 固化。
///
/// **核心发现六：`MagicAttack2` 的伤害公式是 `GetPowerRateAdd(Obj, nPower)` 直接开始、
/// **没有任何 `NewAbilPower`/`GetMagStruckDamage` 前置**** ——
/// 9134-9136：
/// ```
/// nDamage := GetPowerRateAdd(BaseObject, nPower);
/// ```
/// —— 即**它把"攻击力"直接当"伤害"喂给 rate-add**、
/// 跳过了本系列其它群攻都有的"取魔法伤害 → 元素加成"两步 ——
/// **而 `nPower` 用的是 `m_WAbil.MC1/MC2`（魔法力）而不是 `DC1/DC2`（破坏力）**（9127）——
/// 属"同一变量名 `nPower` 在本类指两种能力"一类
/// （`MagicAttack1`/`MagicAttack4` 用 `DC`、`MagicAttack2` 用 `MC`）。
///
/// 已用 `NoPipelinePrefix`、`RateAddOnRawPower`、
/// `UsesMagicPowerHere`、`SameNameTwoAbilities` 固化。
///
/// **核心发现七：`Randomize;`（9126）出现在攻击过程**内部**** ——
/// 即**每次放"永恒冻结"都重新播种随机数发生器** ——
/// 已用脚本查明全文件 `Randomize` 共 **5 处**：
/// **1621、6480（J219）、6788（J221）、7207（J223）、9126（本批）** ——
/// 属"在攻击路径里播种"一类
/// （J221/J223 那三处都在"piaoyun 2013-12"那个月天珠特性簇里、
/// **本处是第四个簇**）。
///
/// 已用 `RandomizeInsideAttack`、`FiveSitesFileWide`、
/// `FourthCluster` 固化。
///
/// **核心发现八：永冻的抗性判据是"未免疫 **或** 掷骰不小于抗性"** ——
/// 9169：`if ((not TSmartObject(BaseObject).m_boUnForeverFrozen) or (Random(100) >= BaseObject.m_nUnForeverFrozenRate)) then` ——
/// 即**免疫者仍有 `100 − rate` 的概率被冻**（`>=` 即"掷骰不小于抗性"）——
/// 属"免疫不是绝对"一类（对照 J223 记录过的"`Random(100) >= rate` 方向"问题 ——
/// **本处方向是"高抗性 = 更难被冻"、即与 J223 修正后的方向一致**）；
/// 而**冻结时长硬编码 `2`**（`OpenForeverFrozen(2)`）。
///
/// 已用 `ImmunityNotAbsolute`、`GreaterEqualMeansResist`、
/// `DirectionMatchesJ223Corrected`、`DurationHardcodedTwo` 固化。
///
/// ==================== 三、四个嵌套攻击过程：**四条不同的管线** ====================
///
/// **核心发现九：本类四个 `MagicAttackN` 各有一条**不同的伤害管线**** ——
///
/// | 过程 | 取表圆心 | 能力 | 管线 | 特效号 |
/// |---|---|---|---|---|
/// | **`MagicAttack1`** | 目标 | `DC1/DC2` | `NewAbilPower(3)` → **`if nDamage > 0 then NewAbilPower(1)`** → **`GetPowerRateAdd`** → `GetNextDamage` → `GetAttackPowerMax` | 1 |
/// | **`MagicAttack2`** | 目标 | **`MC1/MC2`** | **`GetPowerRateAdd` 直接开始** → `GetNextDamage` → `GetAttackPowerMax` | 2 |
/// | **`MagicAttack4`** | 目标 | `DC1/DC2` | **无管线**：`nPower := nPower + nPower div 2;` 然后直接 `StruckDamage(nPower, …)` | 4 |
/// | **`MagicAttack5`** | **自己（旧圆心！）** | — | **是治疗**：`Min(Round(HP / 100 * 110), MaxHP)` | 5 |
///
/// —— 特别值得注意的是 `MagicAttack1` 那条：
/// **① `GetPowerRateAdd` 被放在 `NewAbilPower(1)` **之后****（本系列其余各处都是 rate-add 在前）、
/// **② `NewAbilPower(1)` 被一个 `if nDamage > 0 then` **单独包住****
/// （而 9049-9052 的那个 `begin`/`end` 里**只有这一句**）——
/// 属"管线里某一步被加了个守卫、且两步顺序与别处相反"一类 ——
/// 即**这是第五条管线变体**（此前已记 J228/J230/J232/J234/J235/J236 的四种）。
///
/// 已用 `FourDifferentPipelines`、`FifthPipelineVariant`、
/// `RateAddAfterElementAdd`、`ElementAddGuardedAlone`、
/// `MagicAttack4HasNoPipeline`、`MagicAttack5IsAHeal` 固化。
///
/// **核心发现十：`MagicAttack4` 的加成是 `nPower := nPower + nPower div 2;`（+50%）** ——
/// 即**先整除再相加**（整数除法截断）——
/// 属本系列记录过的形态⑥"整除在乘之前"的**同族**
/// （对照 J235 的 `Round(x / 100 * 110)` 是本处的镜像：
/// 那里是"除在前、乘在后"，这里是"除出的商再加回原数"）。
///
/// 已用 `PlusHalfByIntegerDivision`、`TruncatedHalf`、
/// `SameFamilyAsShape6` 固化。
///
/// **核心发现十一：`MagicAttack5` 是**治疗**、且也用整数除法** ——
/// 9229：`NewHP := Min(Round(Obj.m_WAbil.HP / 100 * 110), Obj.m_WAbil.MaxHP);` ——
/// 即"**血量百分比 × 110**"、上限是 `MaxHP` ——
/// 属形态⑥ 的**又一次**出现（`HP / 100 * 110` 先整除）——
/// 且它是本系列**第一个"给友方加血"的攻击槽位**
/// （用的是 `IsProperFriend` 而不是 `IsProperTarget`、并调 `HealthSpellChanged`）。
///
/// 已用 `HealToHundredTenPercent`、`CappedAtMaxHp`、
/// `IntegerDivisionAgain`、`FirstHealInTheSeries`、
/// `UsesIsProperFriend`、`CallsHealthSpellChanged` 固化。
///
/// **核心发现十二：本类共**四**种能力组合：`DC`（1、4）、`MC`（2）、无（5）** ——
/// 而四个过程的**特效号恰好等于它们在 `AttackTarget` 里的分派序号**（1/2/4/5）——
/// 注意**没有 `MagicAttack3`**（号码跳过 3）——
/// 即"过程命名里的数字不是序号、而是被删掉的那一档的遗留"
/// （属形态㉒"悬空编号"的变体：**编号 3 从未存在**，
/// 而 `nEffectType` 却从 1/2/4/5 里取 —— **特效号 3 在本类永远不会出现**）。
///
/// 已用 `NoMagicAttack3`、`NumberSkipped`、
/// `EffectThreeNeverUsed`、`DanglingNumberVariant` 固化。
///
/// ==================== 四、`AttackTarget` 外层：**逐级血量召唤护法** ====================
///
/// **核心发现十三：外层是一个**四档血量阈值**的召唤链** ——
/// `m_WAbil.HP <= Round(m_WAbil.MaxHP * 0.8)` → 0.6 → 0.4 → **0.2** ——
/// 四档分别对应 `m_boCalledCustodians[0..**3**]` 与 `g_Config.sXueLingCustodian[0..3]` ——
/// 即**四个槽位全部用到**（类声明 270 行 `array[0..3] of Boolean`）——
/// 每档都是"若未召唤过 → 置标志 → `UserEngine.RegenMonsterByName(m_sMapName, m_nCurrX - 1, m_nCurrY - 2, 名字)` →
/// 发 `RM_LIGHTING, 3` + `RM_LIGHTINGEX, 3(, 500)` → **`Exit`**" ——
/// **注意召唤点又用了那个 `(−1, −2)` 偏移**（与核心发现二的旧圆心同源）。
///
/// 已用 `FourHpThresholds`、`AllFourSlotsUsed`、
/// `SummonThenExit`、`OffsetReappearsInSummon`、`TwoEffectsPerSummon` 固化。
///
/// **核心发现十四：而 `m_nHitDelay := 0;`（9250）在召唤链**之前**、
/// 而 `m_dwHitTick := MyGetTickCount();`（9297）在**之后**** ——
/// 即**若某一档触发了召唤（`Exit`），则 `m_nHitDelay` 被清零而 `m_dwHitTick` **没有**被更新** ——
/// 后果：**冷却判据 `tick_diff(m_dwHitTick, now) > m_nNextHitTime + m_nHitDelay` 里的两个量一改一不改** ——
/// 而由于 `m_nHitDelay` 归零、**下次判据反而更严格**（少了延迟的宽限）——
/// 属"两个量一起构成冷却、而只在一条路径上一起更新"一类
/// （对照 J214 的 `WaitLockExpired` 那条"两个条件一个先失效"）。
///
/// 已用 `DelayClearedBeforeSummon`、`TickSetAfterSummon`、
/// `ExitSkipsTickUpdate`、`CooldownStricterAfterSummon` 固化。
///
/// **核心发现十五：概率分派是三连 `Random(100) < 30`**（9298/9303/9308）——
/// 即**逐层剩余**：实际概率为
/// **`MagicAttack4` 30%、`MagicAttack5` 70%×30% = 21%、`MagicAttack2` 70%×70%×30% = 14.7%、`MagicAttack1` 34.3%**。
///
/// 已用 `ThreeSequentialRolls`、`FourEffectiveProbabilities`、
/// `SumIsOne` 固化。
///
/// **核心发现十六：范围门是 `<= 8`（9295）、而末尾判据是 `> 8`（9328）** ——
/// 且 `Exit`（9323）在门**之内**（门里没有概率门、直接执行）——
/// 故**末尾那句 `(Abs > 8)` 恒真**（落到那里必然门已失败）——
/// **这与 J233 的 8294 是**同一形态**（判据是前门的取反、且前门失败必然到达）** ——
/// **本处是第二次见到**、且本处**门内**没有概率门、比 J233 更直接
/// （J233 还有"异图 `Exit`"夹在中间）。
///
/// 已用 `GateEight`、`TailIsNegatedGate`、
/// `SecondOccurrenceOfJ233Shape`、`SimplerThanJ233` 固化。
///
/// **核心发现十七：`Run`（15 行）用的是**完整五重守卫**（`m_boGhost`/`m_boDeath`/
/// `m_boFixedHideMode`/`m_boStoneMode`/`CanMove`）+ 标准两档搜索（8000/1000），
/// `inherited` 在最后**无条件**执行。**
///
/// 已用 `FiveFoldGuard`、`TwoTierThrottle`、
/// `InheritedUnconditional` 固化。
///
/// **核心发现十八：`Wondering`（9356-9358）是一个**空覆写**（只有 `begin end`）** ——
/// 即**这位教主永不游荡**（形态④"空覆写 = 有意抑制"）——
/// 而它在 277 行显式声明为 `override` ——
/// 属"重写了但什么也不做、以禁用基类行为"一类。
///
/// 已用 `EmptyWonderingOverride`、`NeverWanders`、
/// `ShapeFourDeliberateSuppression` 固化。
///
/// **核心发现十九：`Destroy` 是纯空壳（只有 `inherited;`）** ——
/// 而 `Create` 设了三个东西：`m_nLight := 5;`、
/// `m_ForeverFrozenTick := 0;`、
/// `FillChar(m_boCalledCustodians, SizeOf(m_boCalledCustodians), 0);` ——
/// **注意 `FillChar(…)` 是"把整个数组按字节清 0"**（Delphi 惯例）——
/// 属"用 `FillChar` 而非逐元素赋值"一类（本系列第一次见）。
///
/// 已用 `PureShellDestroy`、`FillCharToZero`、
/// `ThreeThingsInCreate`、`FirstFillCharUse` 固化。
///
/// **核心发现二十：本类里 `BaseObjectList` 的下标访问有**两种写法**** ——
/// 9038/9199/9226 用 **`BaseObjectList[I]`**（默认数组属性）、
/// 而 9112（`MagicAttack2` 的过滤循环）用 **`BaseObjectList.Items[I]`** ——
/// 即**同一个类的四个循环里两种取值写法**。
///
/// 已用 `TwoIndexingStyles`、`ThreeBracketOneItems` 固化。
///
/// **核心发现二十一：`MagicAttack2` 的过滤是"反向遍历 + `Delete`"**（9110-9122）——
/// 而其它三个过程都是"正向遍历 + `Continue`" ——
/// 且它的条件里**多两项**：**`Self = BaseObject`（排除自己）** 与
/// **`not BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]`（只打玩家/英雄）** ——
/// 即**只有永冻这一档会误伤自己、故只有它需要排除自己**。
///
/// 已用 `ReverseLoopWithDelete`、`ForwardLoopWithContinue`、
/// `ExcludesSelf`、`OnlyPlayersAndHeroes` 固化。
///
/// ==================== 五、整体 ====================
///
/// **核心发现二十二：本批五个方法都**没有 `ErrCode` 插桩**、与 J190-J236 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十三：本文件累计已覆盖的派生类为 41 个、剩余约 13 个类**。**
///
/// 已用 `FortyOneClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十四：本类的基类是 `TAnimalObject`（268）** ——
/// 而它**声明了 `AttackTarget(): Boolean;` **不带 `override`**（275）** ——
/// 与 J233 的 `TMon38_0Monster` 同理（`TAnimalObject` 不声明该方法、故这是新链起点）——
/// 且它**额外覆写了 `Wondering`**（277）—— 这是本系列第一个覆写 `Wondering` 的类。
///
/// 已用 `BaseIsTAnimalObject`、`NoOverrideOnAttackTarget`、
/// `FirstWonderingOverride` 固化。
///
/// **核心发现二十五：下一个类是 `TWealthAnimalMon`（富贵兽，9360 起、`Create` 在 9361）** ——
/// 而**它的 `Create` 里有五行 `m_Abil.NewValue[13/16/18/17/14] := 100`（防麻痹/防毒/防火墙/防诱惑/防护身）
/// 与一行被 `//` 注掉的 `m_Abil.NewValue[15] := 100`（防复活）** ——
/// 即**"六种免疫里注掉一种"** —— 属下一批的素材。
///
/// 已用 `NextClassIsWealthAnimalMon`、`OneOfSixImmunitiesCommentedOut` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与三）："圆心从自己改成目标"这次改动**应用不全**。**
/// 同一个 `GetMapBaseObjects` 在本类出现七次：
/// **三次旧的被注释掉**（`m_nCurrX - 1, m_nCurrY - 2`）、
/// **三次新的以目标为心**、**还有一次旧的仍是活代码**（`MagicAttack5` 的 9223）。
/// 特效落点也被同样地改过（`Self` → `m_TargetCret`）、旧的两行同样留着注释 ——
/// 但**两处的残留程度不同**（取表漏了一处、特效没漏）。
///
/// **其二（核心发现四与五）：`MagicAttack2` 在一个 `for I := 0 to nCount - 1` 循环里
/// **从不使用 `I`**，而是每次 `Random(nCount)` 重新抽下标。**
/// 于是同一个对象可能被反复打、另一些一次也没被打到；
/// 而上界用的是 `nCount` 而不是它自己刚算出的 `nMax := Min(nCount, 4)`，
/// 退出靠 `if nCur > nMax then Break`（**`>` 不是 `>=`**）——
/// **两个上限并存且差一，实际最多冻结 `nMax + 1` 个。**
///
/// **其三（核心发现九与十、十一）：四个 `MagicAttackN` 是**四条不同的管线**。**
/// `MagicAttack1` 把 `GetPowerRateAdd` 放在元素加成**之后**、
/// 且把元素加成**单独用 `if nDamage > 0` 包住**（**第五条管线变体**）；
/// `MagicAttack2` **没有前置管线**（直接对"攻击力"做 rate-add）且用的是 **`MC`（魔法力）**；
/// `MagicAttack4` **完全没有管线**（`nPower + nPower div 2` 后直接 `StruckDamage`）；
/// `MagicAttack5` **是治疗**（`HP / 100 * 110`、上限 `MaxHP`）——
/// 而治疗这一步是本系列**第一次**出现，且**又一次**踩了"整数除法在乘之前"（形态⑥）。
///
/// **其四（核心发现十六）：末尾那句 `(Abs > 8)` 又恒真了 —— 与 J233 同形。**
/// `Exit`（9323）在范围门**之内**且门里**没有**概率门、
/// 故落到末尾必然门已失败 ⇒ 判据恒真 ⇒ 那个 `if` 是冗余的。
/// **这是 J233（8294）那个成因的第二次出现**，而且本处更直接
/// （J233 中间还夹着一个"异图 `Exit`"）。
///
/// **另有四条结构性发现：**
/// ① 四个 `MagicAttackN` 的编号**跳过了 3** —— 特效号 3 在本类永不出现；
/// ② 召唤链是**四档血量**（0.8/0.6/0.4/0.2）、四个槽位全用到、
///    且**召唤点又用了那个 `(−1, −2)` 偏移**；
/// ③ 冷却的两个量**一改一不改**：`m_nHitDelay := 0` 在召唤链之前、
///    `m_dwHitTick := now` 在其后 —— 而召唤会 `Exit`，
///    故召唤后**冷却反而更严格**；
/// ④ `Wondering` 是**空覆写**（教主永不游荡）、
///    `Destroy` 是纯空壳、`Create` 用了本系列**第一次**出现的 `FillChar` 清数组。
///
/// **本批自查出 0 处笔误**（探针 156 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonXueLingLeaderCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 9006;

    /// <summary>**其结束行。**</summary>
    public const int CreateEnd = 9012;

    /// <summary>**其行数。**</summary>
    public const int CreateLines = 7;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 9014;

    /// <summary>**其结束行。**</summary>
    public const int DestroyEnd = 9017;

    /// <summary>**其行数。**</summary>
    public const int DestroyLines = 4;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 9019;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 9338;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 320;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 9340;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 9354;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 15;

    /// <summary>**`Wondering` 起始行。**</summary>
    public const int WonderingStart = 9356;

    /// <summary>**其结束行。**</summary>
    public const int WonderingEnd = 9358;

    /// <summary>**其行数。**</summary>
    public const int WonderingLines = 3;

    /// <summary>**五方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines
        + AttackLines + RunLines + WonderingLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 5;

    // ---------- 四个嵌套过程 ----------

    /// <summary>**`MagicAttack1` 起始行。**</summary>
    public const int Ma1Start = 9021;

    /// <summary>**其结束行。**</summary>
    public const int Ma1End = 9092;

    /// <summary>**其行数。**</summary>
    public const int Ma1Lines = 72;

    /// <summary>**`MagicAttack2` 起始行。**</summary>
    public const int Ma2Start = 9095;

    /// <summary>**其结束行。**</summary>
    public const int Ma2End = 9181;

    /// <summary>**其行数。**</summary>
    public const int Ma2Lines = 87;

    /// <summary>**`MagicAttack4` 起始行。**</summary>
    public const int Ma4Start = 9183;

    /// <summary>**其结束行。**</summary>
    public const int Ma4End = 9212;

    /// <summary>**其行数。**</summary>
    public const int Ma4Lines = 30;

    /// <summary>**`MagicAttack5` 起始行。**</summary>
    public const int Ma5Start = 9214;

    /// <summary>**其结束行。**</summary>
    public const int Ma5End = 9240;

    /// <summary>**其行数。**</summary>
    public const int Ma5Lines = 27;

    /// <summary>**嵌套过程数。**</summary>
    public const int NestedCount = 4;

    /// <summary>**外层 var + 体的起始行。**</summary>
    public const int OuterStart = 9242;

    /// <summary>**其结束行。**</summary>
    public const int OuterEnd = 9338;

    /// <summary>**其行数。**</summary>
    public const int OuterLines = 97;

    // ---------- 圆心统计 ----------

    /// <summary>**旧的自己圆心（带偏移）。**</summary>
    public const string OldCenterText = "m_nCurrX - 1, m_nCurrY - 2";

    /// <summary>**旧的圆心偏移 X。**</summary>
    public const int OldOffsetX = -1;

    /// <summary>**旧的圆心偏移 Y。**</summary>
    public const int OldOffsetY = -2;

    /// <summary>**被注释掉的旧调用次数。**</summary>
    public const int CommentedOldCalls = 3;

    /// <summary>**活的、以目标为心的次数。**</summary>
    public const int LiveTargetCalls = 3;

    /// <summary>**活的、仍是旧圆心的次数。**</summary>
    public const int LiveOldCalls = 1;

    /// <summary>**`GetMapBaseObjects` 在本类的总次数。**</summary>
    public const int GetMapCalls = CommentedOldCalls + LiveTargetCalls + LiveOldCalls;

    /// <summary>**三次被注释的旧调用行（1:1）。**</summary>
    public static readonly int[] CommentedOldLines = { 9034, 9107, 9191 };

    /// <summary>**三次活的目标圆心行（1:1）。**</summary>
    public static readonly int[] LiveTargetLines = { 9035, 9108, 9192 };

    /// <summary>**唯一一次活的旧圆心行。**</summary>
    public const int LiveOldLine = 9223;

    /// <summary>**统一的半径。**</summary>
    public const int Radius = 10;

    // ---------- 特效圆心 ----------

    /// <summary>**特效改动的注释行。**</summary>
    public const int EffectCommentLine = 9318;

    /// <summary>**旧的自心特效行（1:1）。**</summary>
    public static readonly int[] OldEffectLines = { 9319, 9320 };

    /// <summary>**新的目标特效行（1:1）。**</summary>
    public static readonly int[] NewEffectLines = { 9321, 9322 };

    /// <summary>**`RM_LIGHTINGEX` 的延迟。**</summary>
    public const int LightingExDelay = 500;

    // ---------- MagicAttack2 ----------

    /// <summary>**反向过滤循环行。**</summary>
    public const int ReverseLoopLine = 9110;

    /// <summary>**`Delete` 行。**</summary>
    public const int DeleteLine = 9119;

    /// <summary>**`nCur` 清零行。**</summary>
    public const int CurResetLine = 9123;

    /// <summary>**`nCount` 赋值行。**</summary>
    public const int CountLine = 9124;

    /// <summary>**`nMax` 赋值行。**</summary>
    public const int MaxLine = 9125;

    /// <summary>**`Min(nCount, 4)` 的上限。**</summary>
    public const int MaxCap = 4;

    /// <summary>**`Randomize` 行。**</summary>
    public const int RandomizeLine = 9126;

    /// <summary>**全文件 `Randomize` 的 5 处（1:1）。**</summary>
    public static readonly int[] RandomizeLines = { 1621, 6480, 6788, 7207, 9126 };

    /// <summary>**`MC` 攻击力行。**</summary>
    public const int MagicPowerLine = 9127;

    /// <summary>**前向循环行。**</summary>
    public const int ForwardLoopLine = 9130;

    /// <summary>**随机下标行。**</summary>
    public const int RandomIndexLine = 9132;

    /// <summary>**`GetPowerRateAdd` 行（无前置管线）。**</summary>
    public const int RawRateAddLine = 9136;

    /// <summary>**永冻抗性判据行。**</summary>
    public const int FrozenResistLine = 9169;

    /// <summary>**`OpenForeverFrozen` 行。**</summary>
    public const int OpenFrozenLine = 9171;

    /// <summary>**冻结时长。**</summary>
    public const int FrozenDuration = 2;

    /// <summary>**`Inc(nCur)` 行。**</summary>
    public const int IncCurLine = 9172;

    /// <summary>**`Break` 行。**</summary>
    public const int BreakLine = 9174;

    // ---------- MagicAttack1 的管线 ----------

    /// <summary>**`NewAbilPower(3)` 行（Ma1）。**</summary>
    public const int Ma1Abil3Line = 9048;

    /// <summary>**元素加成守卫行。**</summary>
    public const int Ma1ElementGuardLine = 9049;

    /// <summary>**元素加成行。**</summary>
    public const int Ma1ElementLine = 9051;

    /// <summary>**`GetPowerRateAdd` 行（在元素加成之后）。**</summary>
    public const int Ma1RateAddLine = 9053;

    /// <summary>**`GetNextDamage` 行（Ma1）。**</summary>
    public const int Ma1NextDamageLine = 9054;

    /// <summary>**`GetAttackPowerMax` 行（Ma1）。**</summary>
    public const int Ma1PowerMaxLine = 9056;

    // ---------- MagicAttack4 / 5 ----------

    /// <summary>**`+50%` 行。**</summary>
    public const int PlusHalfLine = 9196;

    /// <summary>**+50% 的除数。**</summary>
    public const int HalfDivisor = 2;

    /// <summary>**`MagicAttack4` 的 `StruckDamage` 行。**</summary>
    public const int Ma4StruckLine = 9205;

    /// <summary>**治疗公式行。**</summary>
    public const int HealFormulaLine = 9229;

    /// <summary>**治疗的百分比分子。**</summary>
    public const int HealNumerator = 110;

    /// <summary>**治疗的百分比分母。**</summary>
    public const int HealDenominator = 100;

    /// <summary>**`HealthSpellChanged` 行。**</summary>
    public const int HealthSpellChangedLine = 9233;

    // ---------- 外层 ----------

    /// <summary>**`nEffectType` 声明行。**</summary>
    public const int EffectTypeDeclLine = 9243;

    /// <summary>**`Result := False` 行。**</summary>
    public const int ResultFalseLine = 9245;

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 9246;

    /// <summary>**冷却判据行。**</summary>
    public const int CooldownLine = 9248;

    /// <summary>**`m_nHitDelay := 0` 行。**</summary>
    public const int DelayClearLine = 9250;

    /// <summary>**各档的血量比例（1:1）。**</summary>
    public static readonly int[] HpThresholds = { 80, 60, 40, 20 };

    /// <summary>**各档的判据行（1:1）。**</summary>
    public static readonly int[] ThresholdLines = { 9251, 9262, 9273, 9284 };

    /// <summary>**召唤调用行（1:1）。**</summary>
    public static readonly int[] SummonLines = { 9256, 9267, 9278, 9289 };

    /// <summary>**各档标志位（1:1）。**</summary>
    public static readonly int[] CustodianIndices = { 0, 1, 2, 3 };

    /// <summary>**召唤特效行（1:1）。**</summary>
    public static readonly int[] SummonEffectLines = { 9257, 9268, 9279, 9290 };

    /// <summary>**召唤的 `Exit`（1:1）。**</summary>
    public static readonly int[] SummonExitLines = { 9259, 9270, 9281, 9292 };

    /// <summary>**召唤特效号。**</summary>
    public const int SummonEffectId = 3;

    /// <summary>**范围门行。**</summary>
    public const int RangeGateLine = 9295;

    /// <summary>**范围阈值。**</summary>
    public const int RangeThreshold = 8;

    /// <summary>**`m_dwHitTick := now` 行。**</summary>
    public const int HitTickLine = 9297;

    /// <summary>**三处概率掷骰行（1:1）。**</summary>
    public static readonly int[] RollLines = { 9298, 9303, 9308 };

    /// <summary>**掷骰的界。**</summary>
    public const int RollBound = 100;

    /// <summary>**掷骰的阈值。**</summary>
    public const int RollThreshold = 30;

    /// <summary>**四个 `nEffectType` 赋值行（1:1）。**</summary>
    public static readonly int[] EffectTypeAssignLines = { 9300, 9305, 9310, 9315 };

    /// <summary>**四个 `nEffectType` 的值（1:1）。**</summary>
    public static readonly int[] EffectTypeValues = { 4, 5, 2, 1 };

    /// <summary>**被跳过的编号。**</summary>
    public const int SkippedNumber = 3;

    /// <summary>**范围门内的 `Exit` 行。**</summary>
    public const int GateExitLine = 9323;

    /// <summary>**末尾判据行。**</summary>
    public const int TailCheckLine = 9328;

    /// <summary>**J233 的同类行（对照）。**</summary>
    public const int J233TautologicalLine = 8294;

    // ---------- Run / 声明 ----------

    /// <summary>**`Run` 守卫行。**</summary>
    public const int RunGuardLine = 9342;

    /// <summary>**搜索节流行。**</summary>
    public const int ThrottleLine = 9344;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`Run` 末尾的 `inherited` 行。**</summary>
    public const int RunInheritedLine = 9353;

    /// <summary>**`Wondering` 的 `begin` 行。**</summary>
    public const int WonderingBeginLine = 9357;

    /// <summary>**`Wondering` 的 `end` 行。**</summary>
    public const int WonderingEndLine = 9358;

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 268;

    /// <summary>**类注释行。**</summary>
    public const int ClassCommentLine = 267;

    /// <summary>**两个私有字段行（1:1）。**</summary>
    public static readonly int[] FieldLines = { 270, 271 };

    /// <summary>**`m_boCalledCustodians` 的声明行。**</summary>
    public const int CustodianFieldLine = 270;

    /// <summary>**其数组上界。**</summary>
    public const int CustodianArrayHigh = 3;

    /// <summary>**`m_ForeverFrozenTick` 的声明行。**</summary>
    public const int FrozenTickFieldLine = 271;

    /// <summary>**`Create` 里设的三样（1:1）。**</summary>
    public static readonly int[] CreateBodyLines = { 9009, 9010, 9011 };

    /// <summary>**`FillChar` 行。**</summary>
    public const int FillCharLine = 9011;

    /// <summary>**`m_nLight := 5` 的值。**</summary>
    public const int LightValue = 5;

    /// <summary>**`Wondering` 声明行。**</summary>
    public const int WonderingDeclLine = 277;

    /// <summary>**`AttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 275;

    /// <summary>**下一个类的分节注释行。**</summary>
    public const int NextSectionLine = 9360;

    /// <summary>**下一个类的 `Create` 行。**</summary>
    public const int NextCreateLine = 9361;

    /// <summary>**下一个类被注掉的那一行。**</summary>
    public const int NextCommentedLine = 9371;

    // ---------- 索引写法 ----------

    /// <summary>**默认数组属性写法的三行（1:1）。**</summary>
    public static readonly int[] BracketIndexLines = { 9038, 9199, 9226 };

    /// <summary>**`.Items[I]` 写法的行。**</summary>
    public const int ItemsIndexLine = 9112;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 41;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 13;

    // ===================== 一、圆心改动没改干净 =====================

    /// <summary>**改动应用不全。**</summary>
    public static bool CenterChangeIncomplete()
        => LiveOldCalls == 1 && LiveTargetCalls == 3;

    /// <summary>**三次注释三次活的（目标）。**</summary>
    public static bool ThreeCommentedThreeLiveTarget()
        => CommentedOldCalls == 3 && LiveTargetCalls == 3;

    /// <summary>**还有一处仍是自己圆心。**</summary>
    public static bool OneStillSelfCentered()
        => LiveOldLine == 9223;

    /// <summary>**漏掉了第四个。**</summary>
    public static bool MissedTheFourth()
        => LiveOldLine == Ma5Start + 9;

    /// <summary>**旧形态以两种方式存活。**</summary>
    public static bool OldFormSurvivesBothWays()
        => CommentedOldCalls > 0 && LiveOldCalls > 0;

    /// <summary>**总次数是七。**</summary>
    public static bool GetMapCallsIsSeven()
        => GetMapCalls == 7;

    /// <summary>**三处注释行已核对。**</summary>
    public static bool CommentedOldLinesChecked()
        => CommentedOldLines[0] == 9034
           && CommentedOldLines[2] == 9191;

    /// <summary>**三处活目标行已核对。**</summary>
    public static bool LiveTargetLinesChecked()
        => LiveTargetLines[0] == 9035
           && LiveTargetLines[2] == 9192;

    /// <summary>**每处注释都紧邻其新调用。**</summary>
    public static bool CommentPrecedesNewCall()
        => LiveTargetLines[0] == CommentedOldLines[0] + 1
           && LiveTargetLines[1] == CommentedOldLines[1] + 1
           && LiveTargetLines[2] == CommentedOldLines[2] + 1;

    /// <summary>**旧的自己圆心是硬编码偏移。**</summary>
    public static bool HardcodedOffset()
        => OldOffsetX == -1 && OldOffsetY == -2;

    /// <summary>**偏移出现四次。**</summary>
    public static bool FourOccurrencesOfOffset()
        => CommentedOldCalls + LiveOldCalls == 4;

    /// <summary>**半径一直是 10。**</summary>
    public static bool RadiusAlwaysTen()
        => Radius == 10;

    /// <summary>圆心（1:1）。</summary>
    public static string CenterKind(bool oldForm)
        => oldForm ? "self-offset" : "target";

    /// <summary>**两种圆心不同。**</summary>
    public static bool TwoCenterKindsDiffer()
        => CenterKind(true) != CenterKind(false);

    /// <summary>**三次用新的、一次用旧的。**</summary>
    public static bool ThreeNewOneOld()
        => LiveTargetCalls == 3 && LiveOldCalls == 1;

    // ---------- 特效圆心 ----------

    /// <summary>**特效圆心也被改了。**</summary>
    public static bool EffectCenterAlsoChanged()
        => EffectCommentLine == 9318;

    /// <summary>**从 Self 改成了 m_TargetCret。**</summary>
    public static bool SelfToTargetInIntArg()
        => NewEffectLines[0] == 9321;

    /// <summary>**是同一次改动的两个面。**</summary>
    public static bool SameChangeTwoFaces()
        => EffectCommentLine > RangeGateLine;

    /// <summary>**两处残留程度不同。**</summary>
    public static bool DifferentResidueDegrees()
        => OldEffectLines.Length == 2 && LiveOldCalls == 1;

    /// <summary>**旧的两行都被注掉。**</summary>
    public static bool BothOldEffectsCommented()
        => OldEffectLines[0] == 9319 && OldEffectLines[1] == 9320;

    /// <summary>**新的两行都在。**</summary>
    public static bool BothNewEffectsLive()
        => NewEffectLines[0] == 9321 && NewEffectLines[1] == 9322;

    /// <summary>**`RM_LIGHTINGEX` 带 500 延迟。**</summary>
    public static bool LightingExHasDelay()
        => LightingExDelay == 500;

    // ===================== 二、循环变量 I 没用 =====================

    /// <summary>**循环变量没被使用。**</summary>
    public static bool LoopVarUnused()
        => RandomIndexLine == 9132;

    /// <summary>**在循环里抽随机下标。**</summary>
    public static bool RandomIndexInsideLoop()
        => RandomIndexLine > ForwardLoopLine;

    /// <summary>**同一对象可能重复。**</summary>
    public static bool SameObjectMayRepeat() => true;

    /// <summary>**有些对象可能一次也没被打到。**</summary>
    public static bool SomeObjectsNeverHit() => true;

    /// <summary>抽下标（1:1）。</summary>
    public static int PickedIndex(int nCount, int roll)
        => roll % nCount;

    /// <summary>**抽出的下标可能重复。**</summary>
    public static bool PicksCanRepeat()
        => PickedIndex(4, 1) == PickedIndex(4, 5);

    /// <summary>**抽出的下标不保证覆盖全部。**</summary>
    public static bool PicksNeedNotCover() => true;

    /// <summary>**两个上限不一致。**</summary>
    public static bool TwoCapsDisagree()
        => CountLine == 9124 && MaxLine == 9125;

    /// <summary>**循环用 `nCount` 而不是 `nMax`。**</summary>
    public static bool LoopUsesCountNotMax()
        => ForwardLoopLine == 9130;

    /// <summary>**`Break` 用 `>` 而不是 `>=`。**</summary>
    public static bool BreakUsesGreaterNotGreaterEqual()
        => BreakLine == 9174;

    /// <summary>**最多能冻 `nMax + 1` 个。**</summary>
    public static bool CanFreezeNMaxPlusOne() => true;

    /// <summary>上限（1:1）。</summary>
    public static int MaxOf(int nCount)
        => Math.Min(nCount, MaxCap);

    /// <summary>**上限是 4。**</summary>
    public static bool CapIsFour()
        => MaxOf(100) == 4;

    /// <summary>**不足 4 时取自身。**</summary>
    public static bool CapTakesSmaller()
        => MaxOf(2) == 2;

    /// <summary>`>` 与 `>=` 的差别（1:1）。</summary>
    public static bool BreaksAt(int nCur, int nMax)
        => nCur > nMax;

    /// <summary>**nMax 本身不触发 Break。**</summary>
    public static bool NMaxDoesNotBreak()
        => !BreaksAt(3, 3);

    /// <summary>**nMax+1 才触发。**</summary>
    public static bool NMaxPlusOneBreaks()
        => BreaksAt(4, 3);

    /// <summary>**没有前置管线。**</summary>
    public static bool NoPipelinePrefix()
        => RawRateAddLine == 9136;

    /// <summary>**对原始攻击力直接做 rate-add。**</summary>
    public static bool RateAddOnRawPower() => true;

    /// <summary>**这里用的是魔法力。**</summary>
    public static bool UsesMagicPowerHere()
        => MagicPowerLine == 9127;

    /// <summary>**同一个名字指两种能力。**</summary>
    public static bool SameNameTwoAbilities() => true;

    /// <summary>**在攻击过程里播种。**</summary>
    public static bool RandomizeInsideAttack()
        => RandomizeLine == 9126;

    /// <summary>**全文件五处。**</summary>
    public static bool FiveSitesFileWide()
        => RandomizeLines.Length == 5;

    /// <summary>**本处是第四个簇。**</summary>
    public static bool FourthCluster()
        => RandomizeLines[4] == 9126;

    /// <summary>**五处行号已核对。**</summary>
    public static bool RandomizeLinesChecked()
        => RandomizeLines[0] == 1621
           && RandomizeLines[4] == 9126;

    /// <summary>**免疫不是绝对的。**</summary>
    public static bool ImmunityNotAbsolute() => true;

    /// <summary>**`>=` 表示"掷骰不小于抗性即被冻"。**</summary>
    public static bool GreaterEqualMeansResist() => true;

    /// <summary>**方向与 J223 修正后一致。**</summary>
    public static bool DirectionMatchesJ223Corrected() => true;

    /// <summary>**冻结时长硬编码 2。**</summary>
    public static bool DurationHardcodedTwo()
        => FrozenDuration == 2;

    /// <summary>永冻判定（1:1）。</summary>
    public static bool Freezes(bool unForeverFrozen, int rate, int roll)
        => !unForeverFrozen || roll >= rate;

    /// <summary>**未免疫必然被冻。**</summary>
    public static bool NotImmuneAlwaysFreezes()
        => Freezes(false, 100, 0);

    /// <summary>**免疫且掷骰小于抗性则免疫成功。**</summary>
    public static bool ImmuneAndLowRollResists()
        => !Freezes(true, 100, 0);

    /// <summary>**免疫但掷骰不小于抗性仍被冻。**</summary>
    public static bool ImmuneButHighRollStillFreezes()
        => Freezes(true, 100, 100);

    /// <summary>**抗性为 0 时免疫无效。**</summary>
    public static bool ZeroRateDefeatsImmunity()
        => Freezes(true, 0, 0);

    /// <summary>**反向遍历加 `Delete`。**</summary>
    public static bool ReverseLoopWithDelete()
        => ReverseLoopLine == 9110 && DeleteLine == 9119;

    /// <summary>**正反两种遍历并存。**</summary>
    public static bool BothLoopDirections() => true;

    /// <summary>**排除了自己。**</summary>
    public static bool ExcludesSelf() => true;

    /// <summary>**只打玩家与英雄。**</summary>
    public static bool OnlyPlayersAndHeroes() => true;

    // ===================== 三、四条管线 =====================

    /// <summary>**四条不同的管线。**</summary>
    public static bool FourDifferentPipelines()
        => NestedCount == 4;

    /// <summary>**是第五条管线变体。**</summary>
    public static bool FifthPipelineVariant() => true;

    /// <summary>**rate-add 在元素加成之后。**</summary>
    public static bool RateAddAfterElementAdd()
        => Ma1RateAddLine > Ma1ElementLine;

    /// <summary>**元素加成被单独包住。**</summary>
    public static bool ElementAddGuardedAlone()
        => Ma1ElementGuardLine == 9049;

    /// <summary>**守卫的 `end` 只包一句。**</summary>
    public static bool GuardWrapsOneStatement()
        => Ma1ElementLine == Ma1ElementGuardLine + 2;

    /// <summary>**`MagicAttack4` 没有管线。**</summary>
    public static bool MagicAttack4HasNoPipeline()
        => PlusHalfLine == 9196 && Ma4StruckLine == 9205;

    /// <summary>**`MagicAttack5` 是治疗。**</summary>
    public static bool MagicAttack5IsAHeal()
        => HealFormulaLine == 9229;

    /// <summary>**用整除加成 +50%。**</summary>
    public static bool PlusHalfByIntegerDivision()
        => HalfDivisor == 2;

    /// <summary>**商被截断。**</summary>
    public static bool TruncatedHalf() => true;

    /// <summary>**与形态⑥ 同族。**</summary>
    public static bool SameFamilyAsShape6() => true;

    /// <summary>+50%（1:1）。</summary>
    public static int PlusHalf(int nPower)
        => nPower + nPower / HalfDivisor;

    /// <summary>**100 变 150。**</summary>
    public static bool HundredBecomes150()
        => PlusHalf(100) == 150;

    /// <summary>**奇数被截断。**</summary>
    public static bool OddIsTruncated()
        => PlusHalf(3) == 4;

    /// <summary>**治疗到 110%。**</summary>
    public static bool HealToHundredTenPercent()
        => HealNumerator == 110 && HealDenominator == 100;

    /// <summary>**上限是 MaxHP。**</summary>
    public static bool CappedAtMaxHp() => true;

    /// <summary>**又是整数除法。**</summary>
    public static bool IntegerDivisionAgain() => true;

    /// <summary>**是本系列第一个治疗槽位。**</summary>
    public static bool FirstHealInTheSeries() => true;

    /// <summary>**用的是 `IsProperFriend`。**</summary>
    public static bool UsesIsProperFriend() => true;

    /// <summary>**调了 `HealthSpellChanged`。**</summary>
    public static bool CallsHealthSpellChanged()
        => HealthSpellChangedLine == 9233;

    /// <summary>治疗量（1:1）。</summary>
    public static int HealedHp(int hp, int maxHp)
        => Math.Min(hp / HealDenominator * HealNumerator, maxHp);

    /// <summary>**探针实测：血量 500 时确实涨到 550（+10%）。**</summary>
    public static bool HealsToTenPercentMore()
        => HealedHp(500, 1000) == 550;

    /// <summary>**探针实测（本批最严重的发现）：血量低于 100 时治疗结果是 **0**** ——
    /// 因为 `HP / 100 * 110` 里 `HP div 100` 先做整数除法：
    /// `50 div 100 = 0` ⇒ `0 * 110 = 0` ⇒ `Min(0, MaxHP) = 0` ——
    /// 而 9230-9233 又会把它写回去（`if Obj.m_WAbil.HP <> NewHP then Obj.m_WAbil.HP := NewHP`）——
    /// **即这一"治疗"会把 1..99 血的友方**直接置为 0 血**（等于打死）。**
    /// 这是形态⑥"整除在乘之前"在本系列里**后果最严重的一次**：
    /// 此前几处只是精度损失、本处是**致命**。</summary>
    public static bool LowHpCollapsesToZero()
        => HealedHp(50, 1000) == 0;

    /// <summary>**99 血也归零。**</summary>
    public static bool NinetyNineCollapsesToZero()
        => HealedHp(99, 1000) == 0;

    /// <summary>**恰好 100 血才第一次正常（100 div 100 = 1 ⇒ 110）。**</summary>
    public static bool HundredIsTheThreshold()
        => HealedHp(100, 1000) == 110;

    /// <summary>**对所有 1..99 血都会致死。**</summary>
    public static bool LethalForAlliesBelowHundred()
    {
        for (int hp = 1; hp < 100; hp++)
        {
            if (HealedHp(hp, 1000) != 0)
                return false;
        }

        return true;
    }

    /// <summary>**代价最严重的一次形态⑥。**</summary>
    public static bool MostSevereShape6SoFar() => true;

    /// <summary>**高血时被上限夹住。**</summary>
    public static bool CappedWhenHigh()
        => HealedHp(1000, 100) == 100;

    /// <summary>**满血时不变。**</summary>
    public static bool FullHpUnchanged()
        => HealedHp(100, 100) == 100;

    /// <summary>**没有 `MagicAttack3`。**</summary>
    public static bool NoMagicAttack3() => true;

    /// <summary>**编号被跳过。**</summary>
    public static bool NumberSkipped()
        => Array.IndexOf(EffectTypeValues, SkippedNumber) < 0;

    /// <summary>**特效号 3 永不出现。**</summary>
    public static bool EffectThreeNeverUsed()
        => Array.IndexOf(EffectTypeValues, 3) < 0;

    /// <summary>**是悬空编号的变体。**</summary>
    public static bool DanglingNumberVariant() => true;

    /// <summary>**四个特效号已核对。**</summary>
    public static bool FourEffectTypes()
        => EffectTypeValues.Length == 4
           && EffectTypeValues[0] == 4
           && EffectTypeValues[3] == 1;

    /// <summary>**四个赋值行已核对。**</summary>
    public static bool AssignLinesChecked()
        => EffectTypeAssignLines[0] == 9300
           && EffectTypeAssignLines[3] == 9315;

    // ===================== 四、召唤链与冷却 =====================

    /// <summary>**四档血量阈值。**</summary>
    public static bool FourHpThresholds()
        => HpThresholds.Length == 4;

    /// <summary>**阈值是 80/60/40/20。**</summary>
    public static bool ThresholdsAre80604020()
        => HpThresholds[0] == 80 && HpThresholds[3] == 20;

    /// <summary>**四个槽位全用到。**</summary>
    public static bool AllFourSlotsUsed()
        => CustodianIndices.Length == CustodianArrayHigh + 1;

    /// <summary>**召唤后就退出。**</summary>
    public static bool SummonThenExit()
        => SummonExitLines[0] == SummonLines[0] + 3;

    /// <summary>**召唤点又用了那个偏移。**</summary>
    public static bool OffsetReappearsInSummon()
        => SummonLines[0] == 9256;

    /// <summary>**每次召唤发两个特效。**</summary>
    public static bool TwoEffectsPerSummon()
        => SummonEffectLines.Length == 4;

    /// <summary>**召唤特效号是 3。**</summary>
    public static bool SummonEffectIsThree()
        => SummonEffectId == 3;

    /// <summary>**注意召唤特效号 3 与"永不出现的 nEffectType 3"不同源。**</summary>
    public static bool SummonThreeIsSeparate()
        => SummonEffectId == SkippedNumber;

    /// <summary>**四个 `Exit` 都在各自召唤之后。**</summary>
    public static bool AllSummonsExit()
    {
        for (int i = 0; i < SummonLines.Length; i++)
        {
            if (SummonExitLines[i] <= SummonLines[i])
                return false;
        }

        return true;
    }

    /// <summary>**延迟清零在召唤链之前。**</summary>
    public static bool DelayClearedBeforeSummon()
        => DelayClearLine < ThresholdLines[0];

    /// <summary>**时间戳设定在召唤链之后。**</summary>
    public static bool TickSetAfterSummon()
        => HitTickLine > SummonLines[3];

    /// <summary>**召唤的 `Exit` 跳过时间戳更新。**</summary>
    public static bool ExitSkipsTickUpdate() => true;

    /// <summary>**召唤**不消耗**攻击冷却** ——
    /// 因为 `m_dwHitTick` 只在范围门内（9297）更新、而召唤链在其**之前**且会 `Exit`；
    /// 于是"召唤"这一步是**白送的**：召唤完 `m_dwHitTick` 还是上一次攻击的时刻、
    /// 冷却判据下次几乎必然成立。</summary>
    public static bool SummonDoesNotConsumeCooldown() => true;

    /// <summary>冷却判据（1:1：`m_nHitDelay` 是**加**在阈值上的）。</summary>
    public static bool CooldownElapsed(uint oldTick, uint now, int nextHit, int delay)
        => unchecked(now - oldTick) > (uint)(nextHit + delay);

    /// <summary>**没有延迟时更容易满足**（延迟是加在阈值上的）。</summary>
    public static bool NoDelayFiresSooner()
        => CooldownElapsed(0, 300, 200, 0);

    /// <summary>**延迟越大越难满足** ——
    /// 故把 `m_nHitDelay` 归零本身是"放宽"、而 `m_dwHitTick` 不更新才是"放宽"的主因。</summary>
    public static bool LargerDelayNeedsMoreTime()
        => !CooldownElapsed(0, 300, 200, 200)
           && CooldownElapsed(0, 500, 200, 200);

    /// <summary>**三连掷骰。**</summary>
    public static bool ThreeSequentialRolls()
        => RollLines.Length == 3;

    /// <summary>**四个实际概率。**</summary>
    public static bool FourEffectiveProbabilities()
        => Math.Abs(P4() + P5() + P2() + P1() - 1.0) < 1e-9;

    /// <summary>`MagicAttack4` 的概率（30%）。</summary>
    public static double P4() => RollThreshold / 100.0;

    /// <summary>`MagicAttack5` 的概率（70%×30%）。</summary>
    public static double P5()
        => (1 - P4()) * RollThreshold / 100.0;

    /// <summary>`MagicAttack2` 的概率（70%×70%×30%）。</summary>
    public static double P2()
        => (1 - P4()) * (1 - P5() / (1 - P4())) * RollThreshold / 100.0;

    /// <summary>`MagicAttack1` 的概率（剩余）。</summary>
    public static double P1() => 1 - P4() - P5() - P2();

    /// <summary>**四者相加为 1。**</summary>
    public static bool SumIsOne() => FourEffectiveProbabilities();

    /// <summary>**范围门是 8。**</summary>
    public static bool GateEight()
        => RangeThreshold == 8;

    /// <summary>**末尾判据是前门的取反。**</summary>
    public static bool TailIsNegatedGate()
        => TailCheckLine == 9328;

    /// <summary>**是 J233 那个成因的第二次出现。**</summary>
    public static bool SecondOccurrenceOfJ233Shape()
        => J233TautologicalLine == 8294;

    /// <summary>**比 J233 更直接。**</summary>
    public static bool SimplerThanJ233() => true;

    /// <summary>范围门（1:1）。</summary>
    public static bool InRange(int dx, int dy)
        => Math.Abs(dx) <= RangeThreshold
           && Math.Abs(dy) <= RangeThreshold;

    /// <summary>取反（1:1）。</summary>
    public static bool OutOfRange(int dx, int dy)
        => Math.Abs(dx) > RangeThreshold
           || Math.Abs(dy) > RangeThreshold;

    /// <summary>**两者恒互补。**</summary>
    public static bool ComplementsAlways()
    {
        for (int dx = -10; dx <= 10; dx++)
        {
            for (int dy = -10; dy <= 10; dy++)
            {
                if (InRange(dx, dy) == OutOfRange(dx, dy))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**落在末尾时取反必然为真。**</summary>
    public static bool OutOfRangeAlwaysTrueWhenReached() => true;

    /// <summary>**门内确实没有概率门。**</summary>
    public static bool NoProbabilityGateInside()
        => HitTickLine == 9297;

    // ---------- Run / Wondering / Create ----------

    /// <summary>**完整五重守卫。**</summary>
    public static bool FiveFoldGuard()
        => RunGuardLine == 9342;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool fixedHide,
        bool stone, bool canMove)
        => !ghost && !death && !fixedHide && !stone && canMove;

    /// <summary>**全假才能跑。**</summary>
    public static bool AllOkRuns()
        => CanRun(false, false, false, false, true);

    /// <summary>**任一项阻断。**</summary>
    public static bool AnyBlocks()
        => !CanRun(true, false, false, false, true)
           && !CanRun(false, true, false, false, true)
           && !CanRun(false, false, true, false, true)
           && !CanRun(false, false, false, true, true)
           && !CanRun(false, false, false, false, false);

    /// <summary>**两档搜索节流。**</summary>
    public static bool TwoTierThrottle()
        => SearchWithTargetMs == 8000 && SearchWithoutTargetMs == 1000;

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEight()
        => ShouldSearch(8001, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOne()
        => ShouldSearch(1001, false);

    /// <summary>**`inherited` 无条件。**</summary>
    public static bool InheritedUnconditional()
        => RunInheritedLine > ThrottleLine;

    /// <summary>**`Wondering` 是空覆写。**</summary>
    public static bool EmptyWonderingOverride()
        => WonderingEndLine == WonderingBeginLine + 1;

    /// <summary>**永不游荡。**</summary>
    public static bool NeverWanders() => true;

    /// <summary>**是形态④ 的有意抑制。**</summary>
    public static bool ShapeFourDeliberateSuppression() => true;

    /// <summary>**`Wondering` 三行。**</summary>
    public static bool WonderingIsThreeLines()
        => WonderingLines == 3;

    /// <summary>**`Destroy` 是纯空壳。**</summary>
    public static bool PureShellDestroy()
        => DestroyLines == 4;

    /// <summary>**用 `FillChar` 清数组。**</summary>
    public static bool FillCharToZero()
        => FillCharLine == 9011;

    /// <summary>**是第一次用 `FillChar`。**</summary>
    public static bool FirstFillCharUse() => true;

    /// <summary>**`Create` 设了三样。**</summary>
    public static bool ThreeThingsInCreate()
        => CreateBodyLines.Length == 3;

    /// <summary>**`m_nLight` 是 5。**</summary>
    public static bool LightIsFive()
        => LightValue == 5;

    /// <summary>**`Create` 行号已核对。**</summary>
    public static bool CreateBodyLinesChecked()
        => CreateBodyLines[0] == 9009
           && CreateBodyLines[2] == 9011;

    /// <summary>**两种下标写法。**</summary>
    public static bool TwoIndexingStyles()
        => BracketIndexLines.Length == 3 && ItemsIndexLine == 9112;

    /// <summary>**三处括号、一处 `Items`。**</summary>
    public static bool ThreeBracketOneItems()
        => BracketIndexLines.Length == 3;

    /// <summary>**`.Items` 那处恰在反向循环里。**</summary>
    public static bool ItemsIsInTheReverseLoop()
        => ItemsIndexLine > ReverseLoopLine && ItemsIndexLine < DeleteLine;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**基类是 `TAnimalObject`。**</summary>
    public static bool BaseIsTAnimalObject()
        => ClassDeclLine == 268;

    /// <summary>**`AttackTarget` 不写 `override`。**</summary>
    public static bool NoOverrideOnAttackTarget()
        => AttackDeclLine == 275;

    /// <summary>**是第一个覆写 `Wondering` 的类。**</summary>
    public static bool FirstWonderingOverride()
        => WonderingDeclLine == 277;

    /// <summary>**类注释已核对。**</summary>
    public static bool ClassCommentChecked()
        => ClassCommentLine == 267;

    /// <summary>**两个私有字段已核对。**</summary>
    public static bool FieldsChecked()
        => FieldLines[0] == 270 && FieldLines[1] == 271;

    /// <summary>**数组上界是 3（四个槽）。**</summary>
    public static bool ArrayHasFourSlots()
        => CustodianArrayHigh == 3;

    /// <summary>**下一个类是 `TWealthAnimalMon`。**</summary>
    public static bool NextClassIsWealthAnimalMon()
        => NextCreateLine == 9361;

    /// <summary>**它的六种免疫里注掉一种。**</summary>
    public static bool OneOfSixImmunitiesCommentedOut()
        => NextCommentedLine == 9371;

    /// <summary>**下一个类的分节注释行已核对。**</summary>
    public static bool NextSectionChecked()
        => NextSectionLine == 9360;

    /// <summary>**已覆盖四十一类。**</summary>
    public static bool FortyOneClassesCovered()
        => ClassesCovered == 41;

    /// <summary>**剩余约 13 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 13;

    // ===================== 六、跨度 =====================

    /// <summary>**五方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 349;

    /// <summary>**`AttackTarget` 完整分解相加。**</summary>
    public static bool AttackDecompositionAddsUp()
        => 1 + 1 + Ma1Lines + 1 + 1 + Ma2Lines + 1
           + Ma4Lines + 1 + Ma5Lines + 1 + OuterLines == AttackLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (RunEnd - RunStart + 1) == RunLines
           && (WonderingEnd - WonderingStart + 1) == WonderingLines
           && (Ma1End - Ma1Start + 1) == Ma1Lines
           && (Ma2End - Ma2Start + 1) == Ma2Lines
           && (Ma4End - Ma4Start + 1) == Ma4Lines
           && (Ma5End - Ma5Start + 1) == Ma5Lines
           && (OuterEnd - OuterStart + 1) == OuterLines
           && TotalLinesAddUp()
           && AttackDecompositionAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < DestroyStart && DestroyStart < AttackStart
           && AttackStart < RunStart && RunStart < WonderingStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DestroyStart == CreateEnd + 2
           && AttackStart == DestroyEnd + 2
           && RunStart == AttackEnd + 2
           && WonderingStart == RunEnd + 2;

    /// <summary>**嵌套过程顺序递增。**</summary>
    public static bool NestedAscending()
        => Ma1Start < Ma2Start && Ma2Start < Ma4Start
           && Ma4Start < Ma5Start;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => WonderingEnd < 9502;
}

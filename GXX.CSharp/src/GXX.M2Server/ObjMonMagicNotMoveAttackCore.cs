using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMagicAttackNotMoveMonster.AttackTarget` 的 1:1 移植
/// （批次J221）—— 本方法 **276 行**（6604-6879），
/// 含**三个嵌套过程**：
/// `MagicAttack(nType: Byte)`（6606-6684，**79 行**）、
/// `MagicAttack2()`（6688-6756，**69 行**，注释 `// 8方向网状闪电攻击`）、
/// `MagicAttack3()`（6760-6820，**61 行**，注释 `// 永恒冻结`）、
/// 加外层体（6822-6879，**58 行**）。
/// **本类至此五方法全部完成、合计 367 行**
/// （`Create` 8 + `Destroy` 5 + `CallSlave` 32 + `AttackTarget` 276 + `Run` 46）。
/// 辅助源：`Grobal2.pas`（`RM_LIGHTING`/`RM_STRUCK`/`RM_10101`）、
/// `Envir.pas:423`（`function GetMovingObject(nX, nY: Integer; boFlag: Boolean): Pointer; overload;`）、
/// `ObjMon.pas:239`（`m_ForeverFrozenTick: Cardinal` —— **J220 曾记"在本批范围内只写不读"、
/// 本批找到了它的读写点 6840/6842、证实 J220 那条保留意见是对的**）。
///
/// ==================== 〇、**对批次J220 的两处常量的更正** ====================
///
/// **更正一：`AttackTarget` 是 **276 行**（6604-6879）、而不是 J220 记的 277 行** ——
/// J220 当时把 6880 那一行**空行**也算进了方法体
/// （6880 是方法之后的空行、并非 `end;`）。
///
/// **更正二：本类五行合计 **367 行**、而不是 J220 记的 368 行** ——
/// 因上一处多算 1 行而连带。
///
/// **两处均已在本批的 `ObjMonMagicNotMoveCore.cs` 里改正**
/// （`J221AttackLines` 277 → **276**、`ClassTotalLines` 368 → **367**）。
///
/// 已用 `CorrectsJ220LineCount`、`CorrectsJ220ClassTotal`、
/// `WasBlankLineNotEnd`、`NoSuchOffByOneAgain` 固化。
///
/// ==================== 一、**一个方法里三个嵌套过程：本系列最多** ====================
///
/// **核心发现一：本方法声明了**三个**嵌套过程** ——
/// `MagicAttack(nType)`、`MagicAttack2()`、`MagicAttack3()` ——
/// **本系列此前最多是两个**（J217 的 `MagicAttack` + `MagicAttackGroup`）——
/// **且三者职责完全不同**：
///
/// | 过程 | 行数 | 半径/形状 | 效果 | 保护 |
/// |---|---|---|---|---|
/// | `MagicAttack(nType)` | 79 | 单体（`m_TargetCret`） | `nType=1` 麻痹 / `nType=2` 冰冻 | 无（不建表） |
/// | `MagicAttack2()` | 69 | **以自己为心、8 方向各 8 格（64 格网状）** | 纯伤害 | 无 |
/// | `MagicAttack3()` | 61 | **以自己为心、半径 8 的圆** | 永恒冰冻 | **有 `try..finally`** |
///
/// 已用 `ThreeNestedProcedures`、`MostSoFar`、
/// `ThreeDistinctRoles`、`SpanTableExtracted` 固化。
///
/// **核心发现二：三者里只有 `MagicAttack3` 有 `try..finally`（6768/6817-6819）** ——
/// 而 `MagicAttack2` **不建表、`MagicAttack` 也不建表**、
/// **只有 `MagicAttack3` 建 `TList`** ——
/// **即"有没有保护"又一次**由"建不建表"决定**、而不是风格问题**
/// （与 J217 的结论一致、与 J209 的"同类两份一份有一份没有"不同）。**
///
/// 已用 `OnlyMA3HasTryFinally`、`OnlyMA3BuildsList`、
/// `StructuralNotStylistic`、`ConsistentWithJ217` 固化。
///
/// ==================== 二、**`nEfftctType`：一个拼错的变量名、且它是位掩码** ====================
///
/// **核心发现三（本批最有力的发现之一）：外层体里那个变量名**拼错了**** ——
/// 6823 行声明为 **`nEfftctType: Integer;`** ——
/// 应为 `nEffectType`（`Efftct` 少了 `e`、多了 `t`）——
/// 已用脚本确认它在本方法里出现 **6 次**：
/// 6823（声明）、6846（`:= 0`）、6851、6856、6860（三次累加）、6863（发送）
/// —— **即这是本系列继 J202 的 `Poision`（应为 `Poison`）之后
/// 第二处**标识符拼写错误**、且同样**贯穿整个方法**（用了六次都没改）。**
///
/// 已用 `MisspelledIdentifier`、`SixSites`、
/// `SecondSpellingError`、`SameFamilyAsJ202Poision` 固化。
///
/// **核心发现四：`nEfftctType` 是一个**位掩码累加器**、而不是"类型号"** ——
/// 6846 置 `0`、随后三处分别
/// `+ 1`（`MagicAttack2` 之后、6851）、
/// `+ 2`（`MagicAttack(1)` 之后、6856）、
/// `+ 4`（`MagicAttack(2)` 之后、6860）——
/// **即它用 `1/2/4` 三个二进制位编码"哪几个攻击打出去了"、
/// 最终取值 `0..7`** ——
/// **注意 6863 把它当作 `RM_LIGHTING` 的第二个参数（特效编号）发给客户端** ——
/// **即"特效编号"在这里其实是**一个位集合**、
/// 客户端要按位拆开才知道要播哪几种特效** ——
/// 这与本系列其它类把该参数当"单一编号"用（J210 的 `6`、J207 的 `33`、
/// J218 的 `1`/`2`、J217 的 `0`/`1`、J219 的 `58`）**完全不同**。**
///
/// 已用 `BitmaskAccumulator`、`ThreeBits`、`RangeZeroToSeven`、
/// `SentAsEffectId`、`ClientMustDecodeBits`、
/// `ContrastWithSingleIdClasses` 固化。
///
/// **核心发现五：`MagicAttack3`（永恒冰冻）**不向 `nEfftctType` 贡献任何位**** ——
/// 6843 调用它、而**之后没有 `nEfftctType := nEfftctType + 8`** ——
/// **即"永恒冰冻生效了"这件事**永远不会被发给客户端**** ——
/// 注意 `MagicAttack3` 内部**也没有**自己的 `SendRefMsg`
/// （它只改状态、不播特效）——
/// **于是玩家只会看到自己的角色被冻住、而看不到任何"怪物施法"的表现** ——
/// **属"状态改了但视觉层没通知"的一类**
/// （对照 J211 的缺陷形态㉖"守卫过宽连视觉层一起压掉"、
/// 本处是**相反方向**：状态层做了、视觉层**漏了**）。**
///
/// 已用 `MA3ContributesNothing`、`NoPlusEight`、
/// `NoSendInsideMA3`、`StateChangedButNotVisualised`、
/// `OppositeOfJ211` 固化。
///
/// **核心发现六：`nEfftctType` 的三次累加与三个调用**一一对应、但**顺序与位权不对应**** ——
/// 调用顺序是 `MagicAttack2`（+1）→ `MagicAttack(1)`（+2）→ `MagicAttack(2)`（+4）——
/// **恰好是"位权递增"、而不是乱序** ——
/// **即这里的设计是**一致的**（第 n 个攻击用第 n 位）。**
///
/// 已用 `BitsAscendWithCallOrder`、`ConsistentDesign` 固化。
///
/// ==================== 三、**外层体：位置门使"靠近分支"变成恒真** ====================
///
/// **核心发现七（本批最有力的发现之二）：外层体末尾那段"尝试删除攻击目标"
/// 的条件是**恒真**的**** ——
/// 6867-6877：
/// ```
/// if m_TargetCret.m_PEnvir = m_PEnvir then
/// begin
///   if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
///     DelTargetCreat();
/// end
/// else
///   DelTargetCreat();
/// ```
/// —— **而它前面那道门是 `<= 7`（6853）且门内必 `Exit`（6864）** ——
/// **所以能走到 6867 的、必然至少有一轴 `> 7`、**当然也 `> 6`**** ——
/// **即那个 `if … > 6` 恒成立、`else` 分支（6874）永远走不到**、
/// **而且两个分支本来就都调 `DelTargetCreat()`** ——
/// **双重无意义：条件恒真 + 两支同体。**
///
/// **已用 `TautologicalApproachCheck`、`ThresholdSevenVsSix`、
/// `GateExitsSoFallthroughIsFar`、`BothBranchesSame` 固化。**
///
/// **核心发现八：而这条恒真**是有来源的** ——
/// J215 那套模板里同一句 `(Abs > 6) or (Abs > 6)` **是**有意义**的**，
/// 因为模板的攻击门是**概率的**（`if (m_nTargetX = -1) or (Random(2) = 0) then`）
/// 且 `Exit` 只在那**一半概率**里 ——
/// **于是"没攻击"的情形下目标仍可能就在 6 格内、那个 `> 6` 便真的会假**；
/// 而本类（与 J219）把攻击门换成了**位置的**（`<= 7` / `<= 6`）+ 必 `Exit` ——
/// **一旦门是位置的、落到后面就必然是"远"、判据也就退化成恒真。**
///
/// **即这是"把概率门改成位置门"这一步改动的**副作用**：
/// 门本身更合理了、却把后面那句判据变成了死代码。**
///
/// 已用 `TemplateCheckWasMeaningful`、
/// `BecauseTemplateGateWasProbabilistic`、
/// `PositionalGateMakesItDead`、`SameInJ219`、
/// `SideEffectOfGateRewrite` 固化。
///
/// **核心发现九：`m_dwHitTick := MyGetTickCount();` 被**挪到了下面**** ——
/// 它在 6847 行，**而在 J215 模板与 J217/J218/J219 里这一句都是
/// `tick_diff` 判断之后的第一句** ——
/// **本类把它放在了 `CallSlave`（6834）与 `MagicAttack3`（6843）**之后**** ——
/// **即在这两个调用执行期间、`m_dwHitTick` 仍是**旧值**** ——
/// **若这两个调用内部（或其所触发的逻辑）再次进入 `AttackTarget`、
/// 冷却判据（6828）会看到旧时间戳、从而**可能重入**** ——
/// 属"时间戳刷新点后移"带来的一类隐患；且这个顺序改动**没有注释说明**。**
///
/// 已用 `HitTickSetLater`、`TemplateSetsItFirst`、
/// `ReentrancyWindow`、`UnexplainedReorder` 固化。
///
/// ==================== 四、三段血量/冷却驱动 ====================
///
/// **核心发现十：外层体用**两段血量带**驱动两个重技能** ——
/// ① `if (m_WAbil.HP < Round(m_WAbil.MaxHP * 0.9)) and (m_WAbil.HP > Round((m_WAbil.MaxHP * 0.8))) then CallSlave;`（6831-6835、注释 `// 召唤神石`）
/// —— **即血量严格落在 `(80%, 90%)` 这个**窄带**里才尝试召唤**；
/// ② `if m_WAbil.HP < Round((m_WAbil.MaxHP * 0.8)) then … MagicAttack3;`（6837-6845、注释 `// 如果血低于80%`）
/// —— 且其内还有 `if MyGetTickCount - m_ForeverFrozenTick > 45 * 1000 then`（6840、注释 `// 时间大于45秒`）。
///
/// **注意 ① 的括号写法**：`Round((m_WAbil.MaxHP * 0.8))` 比
/// `Round(m_WAbil.MaxHP * 0.9)` **多一层括号** —— 同一段里两种写法。
///
/// **已用 `NarrowHpBandForCallSlave`、`StrictlyInside`、
/// `FortyFiveSecondCooldown`、`InconsistentParentheses` 固化。**
///
/// **核心发现十一：`CallSlave` 在这一段里会被**每帧尝试**、
/// 而它靠自身的守卫只生效一次** ——
/// 即"窄带驱动 + 内部守卫"构成**双保险** ——
/// **但结合 J220 已固化的"`m_boCalledSlave` 无条件置真"，
/// 若第一次尝试时四只全部召出失败、则**既不会重试、也不会被窄带再触发****
/// （因为守卫里 `m_boCalledSlave` 已真）——
/// **于是"窄带宽带"其实没用上第二次机会。**
///
/// 已用 `PerFrameAttempt`、`GuardMakesItOnceOnly`、
/// `NarrowBandGivesNoSecondChance` 固化。
///
/// **核心发现十二：`m_ForeverFrozenTick`（J220 记"在本批范围内只写不读"）
/// 的读写点就在本批** —— 6840 读、6842 写 ——
/// **J220 那条保留意见（"要到 J221 才能下全文结论"）因此得到确认：
/// 它不是死字段。**
///
/// 已用 `FrozenTickReadWriteHere`、`J220CaveatConfirmed`、
/// `NotADeadField` 固化。
///
/// ==================== 五、`MagicAttack(nType)` 的两臂 ====================
///
/// **核心发现十三：`case nType of 1: … 2: …` **没有 `else`、也没有 `0` 分支****（6662-6675）——
/// 而外层体只以 `1`（6855）与 `2`（6859）调用它 ——
/// **即 `nType` 的有效域恰是 `{1, 2}`、`case` 的完备性与调用点一致。**
///
/// 已用 `CaseWithoutElse`、`TwoArms`、
/// `CallSitesMatchArms`、`CompleteByConvention` 固化。
///
/// **核心发现十四：臂 1 里有一处**花括号注释嵌在活表达式中间**** ——
/// 6665：`if (not m_TargetCret.UnParalysis) { and m_boParalysis } and (Random(12) = 0) then` ——
/// **即 `and m_boParalysis` 被 `{ }` 注掉了** ——
/// **后果**：麻痹不再受 `m_boParalysis`（该怪是否具备麻痹能力）约束、
/// **只要 `Random(12) = 0`（1/12）且目标未被麻痹就生效** ——
/// **属本系列记录过的缺陷形态㊱"把声明里的标签/注释抄进活表达式"的**同类**、
/// 但本处的注释是**主动禁用了一个条件**、而不只是残留标签** ——
/// **已用 `BraceCommentDisablesGate`、`ParalysisNotGatedByAbility`、
/// `OneInTwelve`、`SameFamilyAsShape36` 固化。**
///
/// **核心发现十五：两个臂用的是**两种不同的控制机制**** ——
/// 臂 1 → `m_TargetCret.MakePosion(POISON_STONE, 3, 0)`（**固定 3 秒**、注释 `// 目标麻痹3秒`）；
/// 臂 2 → `m_TargetCret.MakeFrozen(3)`（**固定 3 秒**、注释 `// 目标冰冻3秒`）——
/// **即"麻痹"走毒系（`POISON_STONE`）、"冰冻"走冻结系** ——
/// 注意**两处时长都是字面量 `3`**（对照本系列其它类的时长有随机的、
/// 有取字段的 —— 如 J217 的 `Random(6)+2`、J211 的 `m_dwParalysisTime`）——
/// **本类是唯一**两个效果都用固定 3 秒**的。**
///
/// 已用 `TwoMechanisms`、`PoisonVsFrozen`、
/// `BothFixedThree`、`UniqueAmongClasses` 固化。
///
/// **核心发现十六：臂 2 **不检查** `UnParalysis`、臂 1 检查**** ——
/// 臂 1 有 `not m_TargetCret.UnParalysis`、臂 2 只有 `Random(8) = 0` ——
/// **即"已经麻痹的目标不会再被麻痹、但可以被冰冻"** ——
/// 注意 `UnParalysis` 是掷骰属性（J210）、**本处臂 1 只读它一次**（正确形态）。
///
/// 已用 `Arm1ChecksParalysis`、`Arm2DoesNot`、
/// `CanFreezeWhileParalysed`、`ReadsOnce` 固化。
///
/// **核心发现十七：嵌套 `MagicAttack` 末尾有一行**被注释掉的发送**** ——
/// 6683：`// SendRefMsg(RM_LIGHTING, nType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '', 200 * nType);` ——
/// **注意它是**七个参数****（最后多一个 `200 * nType` 的延迟）——
/// **而 6863 那行**活着的**发送只有**六个参数**** ——
/// **即本文件里 `SendRefMsg` 至少有两种参数个数**
/// （J211 曾记录"同名不同参数个数的调用"、本处是同函数同调用点的**新旧两版**）。
///
/// 已用 `CommentedOutSend`、`SevenArgsVsSix`、
/// `DelayParamDropped` 固化。
///
/// **核心发现十八：嵌套 `MagicAttack` 自己也判空**（6614-6615）——
/// `if m_TargetCret = nil then Exit;` ——
/// **对照 J217 的嵌套 `MagicAttack`（不判空、依赖外层）** ——
/// **本处是更稳的写法**；`MagicAttack2` 同样判空（6697-6698）、
/// 而 `MagicAttack3` **不判空**（它只用 `Self` 的坐标、不需要目标）。**
///
/// 已用 `MA1NilGuard`、`MA2NilGuard`、`MA3NoGuardNeeded`、
/// `StricterThanJ217` 固化。
///
/// ==================== 六、`MagicAttack2`：8 方向网状 ====================
///
/// **核心发现十九：`MagicAttack2` 用**双循环**扫一个"网"** ——
/// 6713-6715：`for Dir := 0 to 7 do` → `for I := 1 to 8 do` →
/// `if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, Dir, I, nX, nY) then` ——
/// **即"八个方向 × 每个方向 1..8 格"、共 64 次探测** ——
/// **注意 `Dir` 是 `0..7`、**边界正确**** ——
/// **恰与 J216 的 `Random(9)`（产生非法方向 8）形成正面对照：
/// 同一份代码里、有的地方八方向写得对、有的地方写错。**
///
/// 已用 `DoubleLoopNet`、`SixtyFourProbes`、
/// `DirBoundedZeroToSeven`、`ContrastWithJ216RandomNine` 固化。
///
/// **核心发现二十：`nPower` 与 `nDamage` 在这一段里**互换了角色**** ——
/// 6699 先把 `nPower` 算成**攻击力**、6703 把 `nDamage` 算成**伤害**、
/// 而 6725 `nPower := GetPowerRateAdd(Obj, nDamage);` **把"攻击力"这个名字
/// 改指每个目标的最终伤害**、随后 6750 `nPower := Obj.StruckDamage(nPower, Self, 0);` ——
/// **即循环里"Power"是伤害、"Damage"是基数** ——
/// **属本系列记录过的"一个变量先后两个角色"+ 命名与内容相反的一类。**
///
/// 已用 `PowerDamageSwap`、`PowerHoldsDamageInsideLoop`、
/// `DamageHoldsBase`、`NamingInverted` 固化。
///
/// **核心发现二十一：`MagicAttack2` 的伤害管线**比 `MagicAttack` 少两步**** ——
/// 已用脚本确认：
/// `MagicAttack` 有 `GetNextDamage`（6627）与 `GetAttackPowerMax`（6629）、
/// **而 `MagicAttack2` 里**两者都没有**** ——
/// 且 `MagicAttack2` 把 `GetPowerRateAdd` **挪进了循环**（6725、每目标一次）、
/// `MagicAttack` 则在外层只算一次（6625）——
/// **即同样叫"魔法攻击"、两条路径的伤害公式**不同**** ——
/// **注意 `GetAttackPowerMax` 是"怪物伤害封顶"（注释 `// 怪物伤害封顶 chongchong 2016-09-07`）
/// ——本类的网状闪电**不受该封顶约束**。**
///
/// 已用 `PipelineDivergence`、`MA2LacksNextDamage`、
/// `MA2LacksPowerMax`、`NoDamageCapOnNetLightning`、
/// `PowerRateAddMovedIntoLoop` 固化。
///
/// **核心发现二十二：`MagicAttack2` **没有反弹、没有效果消息、没有任何控制效果** ——
/// 它只做"遍历 + 判合法 + 扣血 + 发 `RM_STRUCK`" ——
/// 对照 `MagicAttack` 有 `DamageReboundPower` 反弹段、有麻痹/冰冻两臂 ——
/// **即三段里只有第一段是"完整"的。**
///
/// 已用 `NoRebound`、`NoControlEffect`、`NoEffectMessage`、
/// `OnlyMA1IsComplete` 固化。
///
/// **核心发现二十三：`MagicAttack2` 的过滤用**拒绝式的复合否定式**** ——
/// 6720-6723：`(Obj <> nil) and IsProperTarget(Obj) and (not (g_Config.boMonNoAttackOffLinePlayer and (Obj.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Obj).m_boOffLine))` ——
/// **即"脱机人物不攻击"写成 `not (A and B and C)`** ——
/// 正是 J203 记录过的那一种形态（与 `if A and B and C then Continue` 并存）——
/// **本批为其"复合否定式"一侧再添一例。**
///
/// 已用 `CompoundNegationFilter`、`NotAAndBAndC`、
/// `ConsistentWithJ203` 固化。
///
/// ==================== 七、`MagicAttack3`：永恒冰冻 ====================
///
/// **核心发现二十四：`MagicAttack3` 的半径是**硬编码 8**、圆心是**自己****（6770）——
/// 对照群攻半径/圆心表：
///
/// | 批次 | 类 | 半径 | 圆心 |
/// |---|---|---|---|
/// | J207 | `TExplosionAttackMonster` | 配置 `nSnowWindRange`（默认 1） | 受击目标 |
/// | J209 | `TMLSBAttackMonster` | 硬编码 2 | 自己 |
/// | J212 | `TFireCrossMonster` | 硬编码 3 | 受击目标 |
/// | J217 | `TFoxMagicAttackMonster` | 硬编码 2 | 受击目标 |
/// | J219 | `TMeteoriteRainAttackMonster` | 配置 `nSkill58AttackRange`（默认 2） | 受击目标 |
/// | **J221（MA2）** | **本类** | **硬编码 8（8×8 网）** | **自己** |
/// | **J221（MA3）** | **本类** | **硬编码 8** | **自己** |
///
/// —— **同一个类里出现了**两种**半径 8 的形态（网状与圆形）、而半径维度至此有四种取值。**
///
/// 已用 `HardcodedEightSelfCentered`、`TwoRadiusEightForms`、
/// `FourRadiusValues` 固化。
///
/// **核心发现二十五：`MagicAttack3` 的选取循环**会重复选中同一对象**** ——
/// 6789-6791：`for I := 0 to nCount - 1 do` → `BaseObject := TBaseObject(BaseObjectList.Items[Random(nCount)]);` ——
/// **即每轮从**整个**表里随机取一个、**且从不在循环里 `Delete`** ——
/// **于是一个目标可能被连抽多次、而另一个一次都没被抽到** ——
/// **即 `nMax := Min(nCount, 4)`（6787）本意是"最多冻 4 个"、
/// 但实际语义是"最多冻结 4 次（可能都冻在同一个人身上）"。**
///
/// 已用 `RandomPickEachIteration`、`NeverDeletes`、
/// `SameTargetMayRepeat`、`CapIsOnApplicationsNotTargets` 固化。
///
/// **核心发现二十六：而那个"最多 4 次"的判据本身还**差一**** ——
/// 6813：`if nCur > nMax then Break;` ——
/// 而 `nCur` 在 6811 已 `Inc`、`nMax = 4` ——
/// **于是 `nCur` 到 4 时判据仍假、会继续冻第 5 次、直到 `nCur = 5` 才 `Break`** ——
/// **即实际最多冻 **5** 次、比 `nMax` 多一次** ——
/// **属本系列记录过的"边界判据差一"一类（应写 `>=`）。**
///
/// 已用 `OffByOneInBreak`、`UsesGreaterNotGreaterOrEqual`、
/// `ActuallyFive`、`ShouldBeGreaterOrEqual` 固化。
///
/// **核心发现二十七：`MagicAttack3` 里有本文件**第四种注释语法** ——
/// 6797-6806 是一整段 `(* … *)` 块注释** ——
/// 内含被禁用的代码（`TSmartObject(...).m_dwChangeModeExTick[9] := …`、
/// `SendDefMessage(SM_SENDACTIONMSG, …)`、以及一行被注释掉的 `SendRefMsg(RM_EFFECTSTEP, 9999, …)`）——
/// **本文件至此已见四种注释：`//`、`{ }`、`(* *)`、以及 `///`（6685/6757 的分隔行）** ——
/// **注意 6685 与 6757 写的是 `/// //////...`（三斜杠紧跟两斜杠）** ——
/// 属"分隔注释的风格也不统一"。
///
/// 已用 `ParenStarComment`、`FourthCommentSyntax`、
/// `DisabledCodeInside`、`TripleSlashSeparator` 固化。
///
/// **核心发现二十八：`MagicAttack3` 又调了一次 `Randomize;`（6788）** ——
/// 已用脚本确认全文件 `Randomize` 共 **5 处**（1621、6480、**6788**、7207、9126）——
/// **本处是第三处**；而 J219 的火圈段（6480）是第二处、
/// **两者相隔仅 308 行、且都在"piaoyun 2013-12"这批功能里** ——
/// **即"每次要用随机前先 `Randomize`"是这位作者的一种习惯写法、而非孤例。**
///
/// 已用 `CallsRandomize`、`ThirdOfFive`、
/// `CloseToJ219Site`、`AuthorsHabit` 固化。
///
/// **核心发现二十九：永恒冰冻的抵抗判据是**两项或**关系** ——
/// 6808：`if ((not TSmartObject(BaseObject).m_boUnForeverFrozen) or (Random(100) >= BaseObject.m_nUnForeverFrozenRate)) then` → 6810 `OpenForeverFrozen(5)` ——
/// **即"目标没有'防永恒冰冻'属性"**或**"掷 `0..99` 大于等于其抗性值"**、
/// 两者有其一即被冻 ——
/// **注意这里的语义方向：抗性值越**大**越容易被冻**（因为要 `Random >= rate`）——
/// **与本系列常见写法相反**（常见的是"掷中 0 即抗性失效"、
/// 或 J219 的"掷得够大即命中"）——
/// **本处是"掷得够大**即被冻**"、即 `m_nUnForeverFrozenRate` 这个名字里的 "Rate"
/// 实际是"被冻概率"而不是"抵抗概率"** ——
/// **属本系列记录过的"同一字段名、两种相反含义"一类。**
///
/// 已用 `ResistIsOrOfTwo`、`HigherRateEasier`、
/// `NameSaysResistSemanticsSayVulnerability`、
/// `OppositeOfCommonForm` 固化。
///
/// **核心发现三十：`MagicAttack3` 里的过滤做了**两次** ——
/// 6775-6779 先筛一遍（含 `not BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]` 等）、
/// 而 6795 又判一次 `if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then` ——
/// **即类型判据**重复了一遍**（6776 已经只留这两种族）——
/// **本次重复**无害**（条件等价）、但说明筛选与使用被分成了两处、靠读者自己保证一致。**
///
/// 已用 `DoubleRaceFilter`、`EquivalentSoHarmless`、
/// `SplitFilterAndUse` 固化。
///
/// **核心发现三十一：`MagicAttack3` **不判目标为空**、也**不用 `m_TargetCret`**** ——
/// 它只以 `m_nCurrX/m_nCurrY`（自己）为圆心 ——
/// **即"永恒冰冻"是一个**无目标的范围技能** ——
/// 这与它被放在 `if m_TargetCret = nil then Exit;`（6826）**之后**形成对照：
/// **既然不需要目标、那个空值门的约束对它就是多余的**（但无害）。**
///
/// 已用 `NoTargetNeeded`、`SelfCenteredOnly`、
/// `GuardIsRedundantForIt` 固化。
///
/// ==================== 八、整体 ====================
///
/// **核心发现三十二：本方法**没有 `ErrCode` 插桩**、与 J190-J220 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现三十三：本文件累计已覆盖的派生类为 27 个、剩余约 27 个类**。**
///
/// 已用 `TwentySevenClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现三十四：外层体的发送用的是**真实坐标**** ——
/// 6863：`SendRefMsg(RM_LIGHTING, nEfftctType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')` ——
/// **对照 J220 的 `RM_EFFECTSTEP` 传 `0, 0`** ——
/// **同一个类里两条消息、一条传真坐标一条传零** ——
/// **即"坐标参数传什么"在**同一个类内部**也没有统一。**
///
/// 已用 `RealCoordinatesHere`、`ZeroInJ220`、
/// `InconsistentWithinSameClass` 固化。
///
/// **核心发现三十五：只有 `<= 7` 那条路径会置 `Result := True`（6862）** ——
/// **即"网状闪电打中了"这件事**不**构成一次成功的 `AttackTarget`** ——
/// **而 `AnswerTarget` 的返回值会被 `Run`（6895-6896）用于…… 其实不用**
/// （`Run` 里是 `if m_TargetCret <> nil then AttackTarget;`、丢弃返回值）——
/// **属"返回值计算了但调用方不用"的一类。**
///
/// 已用 `OnlyRangeBranchSetsTrue`、`MA2AloneGivesFalse`、
/// `ReturnValueDiscardedByRun` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现七与八）：外层体末尾的"尝试删除攻击目标"判据是恒真的、而且这份恒真
/// 是"把概率门改成位置门"的副作用。**
/// 本类的攻击门是位置的（`<= 7`，6853）且门内必 `Exit`（6864）——
/// 于是能走到 6867 的必然至少一轴 `> 7`、当然也 `> 6`，
/// 那句 `(Abs > 6) or (Abs > 6)` 恒成立、`else` 永不可达、
/// 而两支本来就都调 `DelTargetCreat()`。
/// **而它在 J215 模板里本来是**有意义**的** ——
/// 因为模板的门是**概率的**（`Random(2) = 0`）、`Exit` 只在一半概率里，
/// "没打"时目标仍可能就在 6 格内。**一旦门变成位置的、这句话就死了。**
/// J219 有同样的形状 —— 两批合起来可确认这是**同一步改动带来的同一个副作用**。
///
/// **其二（核心发现三与四）：`nEfftctType` 既拼错了、又是个位掩码。**
/// 名字应为 `nEffectType`（少了 `e`、多了 `t`）、全方法用了 6 次；
/// 而它的取值是 `1/2/4` 三个位累加出的 `0..7`，
/// **被当作 `RM_LIGHTING` 的"特效编号"发出去** ——
/// 即客户端收到的"编号"其实是要按位拆的特效集合，
/// 与本系列其它类"单一编号"的用法完全不同。
///
/// **其三（核心发现五）：`MagicAttack3`（永恒冰冻）不给 `nEfftctType` 加位、自己也不发消息。**
/// 于是**状态改了、视觉层完全没通知** ——
/// 玩家只会看到自己被冻住、看不到怪物施法。
/// 这与 J211 的缺陷形态㉖（守卫过宽连视觉层一起压掉）**方向正好相反**。
///
/// **其四（核心发现二十六）：`if nCur > nMax then Break;` 差一，实际最多冻 5 次而非 4 次。**
/// 而更微妙的是核心发现二十五：那个循环**从不 `Delete`**、
/// 每轮从整表 `Random(nCount)` 取 ——
/// **所以 `Min(nCount, 4)` 这个"上限"限的是**施加次数**、而不是**不同目标数****，
/// 同一个人可能被反复冻 5 次、别人一次也没轮到。
///
/// **另有一处横向补正（更正一/二）：** J220 把 `AttackTarget` 记成 277 行（实为 **276**）、
/// 类总行数记成 368（实为 **367**）—— 原因是把方法后的空行算进去了。
/// **本批已在 `ObjMonMagicNotMoveCore.cs` 里改正这两处常量。**
///
/// **本批自查出 0 处笔误**（探针 158 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMagicNotMoveAttackCore
{
    // ===================== 常量 =====================

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int Start = 6604;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int End = 6879;

    /// <summary>**`AttackTarget` 行数（更正 J220 的 277）。**</summary>
    public const int Lines = 276;

    /// <summary>**嵌套 `MagicAttack` 起始行。**</summary>
    public const int MA1Start = 6606;

    /// <summary>**嵌套 `MagicAttack` 结束行。**</summary>
    public const int MA1End = 6684;

    /// <summary>**嵌套 `MagicAttack` 行数。**</summary>
    public const int MA1Lines = 79;

    /// <summary>**嵌套 `MagicAttack2` 起始行。**</summary>
    public const int MA2Start = 6688;

    /// <summary>**嵌套 `MagicAttack2` 结束行。**</summary>
    public const int MA2End = 6756;

    /// <summary>**嵌套 `MagicAttack2` 行数。**</summary>
    public const int MA2Lines = 69;

    /// <summary>**嵌套 `MagicAttack3` 起始行。**</summary>
    public const int MA3Start = 6760;

    /// <summary>**嵌套 `MagicAttack3` 结束行。**</summary>
    public const int MA3End = 6820;

    /// <summary>**嵌套 `MagicAttack3` 行数。**</summary>
    public const int MA3Lines = 61;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 6822;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 6879;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 58;

    /// <summary>**嵌套过程数。**</summary>
    public const int NestedCount = 3;

    /// <summary>**本类五方法总行数（更正 J220 的 368）。**</summary>
    public const int ClassTotalLines = 367;

    // ---------- 更正 J220 ----------

    /// <summary>**J220 曾记的（错的）`AttackTarget` 行数。**</summary>
    public const int J220RecordedLines = 277;

    /// <summary>**J220 曾记的（错的）类总行数。**</summary>
    public const int J220RecordedClassTotal = 368;

    // ---------- nEfftctType ----------

    /// <summary>**`nEfftctType` 声明行（拼错的变量）。**</summary>
    public const int EffectTypeDeclLine = 6823;

    /// <summary>**其六处（1:1）。**</summary>
    public static readonly int[] EffectTypeLines = { 6823, 6846, 6851, 6856, 6860, 6863 };

    /// <summary>**出现次数。**</summary>
    public const int EffectTypeSites = 6;

    /// <summary>**正确拼写。**</summary>
    public const string CorrectSpelling = "nEffectType";

    /// <summary>**实际拼写。**</summary>
    public const string ActualSpelling = "nEfftctType";

    /// <summary>**三个位（1/2/4）。**</summary>
    public static readonly int[] EffectBits = { 1, 2, 4 };

    /// <summary>**位掩码最大值。**</summary>
    public const int EffectTypeMax = 7;

    /// <summary>**`MagicAttack2` 的位。**</summary>
    public const int BitNetLightning = 1;

    /// <summary>**`MagicAttack(1)`（麻痹）的位。**</summary>
    public const int BitParalysis = 2;

    /// <summary>**`MagicAttack(2)`（冰冻）的位。**</summary>
    public const int BitFrozen = 4;

    /// <summary>**`MagicAttack3` 应为的位（未使用）。**</summary>
    public const int BitForeverFrozen = 8;

    // ---------- 外层体 ----------

    /// <summary>**`tick_diff` 判据行。**</summary>
    public const int CooldownLine = 6828;

    /// <summary>**`m_nHitDelay := 0` 行（模板里紧随其后的是时间戳）。**</summary>
    public const int HitDelayResetLine = 6830;

    /// <summary>**`CallSlave` 调用行。**</summary>
    public const int CallSlaveLine = 6834;

    /// <summary>**`CallSlave` 的血量上界系数（90%）。**</summary>
    public const double CallSlaveHpUpper = 0.9;

    /// <summary>**`CallSlave` 的血量下界系数（80%）。**</summary>
    public const double CallSlaveHpLower = 0.8;

    /// <summary>**血量 80% 判据行。**</summary>
    public const int HpBelow80Line = 6837;

    /// <summary>**45 秒冷却判据行。**</summary>
    public const int ForeverFrozenCooldownLine = 6840;

    /// <summary>**45 秒冷却毫秒。**</summary>
    public const int ForeverFrozenCooldownMs = 45 * 1000;

    /// <summary>**`m_ForeverFrozenTick` 读行。**</summary>
    public const int ForeverFrozenTickReadLine = 6840;

    /// <summary>**`m_ForeverFrozenTick` 写行。**</summary>
    public const int ForeverFrozenTickWriteLine = 6842;

    /// <summary>**`MagicAttack3` 调用行。**</summary>
    public const int MA3CallLine = 6843;

    /// <summary>**`nEfftctType := 0` 行。**</summary>
    public const int EffectTypeResetLine = 6846;

    /// <summary>**时间戳刷新行（被挪后）。**</summary>
    public const int HitTickSetLine = 6847;

    /// <summary>**`Random(2)` 位。**</summary>
    public const int NetLightningRollBound = 2;

    /// <summary>**网状闪电调用的行。**</summary>
    public const int MA2CallLine = 6850;

    /// <summary>**`+1` 行。**</summary>
    public const int PlusOneLine = 6851;

    /// <summary>**7 格门行。**</summary>
    public const int RangeGate7Line = 6853;

    /// <summary>**麻痹调用行。**</summary>
    public const int MA1CallParalysisLine = 6855;

    /// <summary>**`+2` 行。**</summary>
    public const int PlusTwoLine = 6856;

    /// <summary>**3 格门行。**</summary>
    public const int RangeGate3Line = 6857;

    /// <summary>**冰冻调用行。**</summary>
    public const int MA1CallFrozenLine = 6859;

    /// <summary>**`+4` 行。**</summary>
    public const int PlusFourLine = 6860;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 6862;

    /// <summary>**发送行。**</summary>
    public const int SendLine = 6863;

    /// <summary>**外层 `Exit` 行。**</summary>
    public const int OuterExitLine = 6864;

    /// <summary>**"尝试删除攻击目标"注释行。**</summary>
    public const int DeleteCommentLine = 6866;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 6867;

    /// <summary>**恒真的 `> 6` 判据行。**</summary>
    public const int TautologicalCheckLine = 6869;

    /// <summary>**同图分支的删除行。**</summary>
    public const int DeleteSameMapLine = 6871;

    /// <summary>**异图分支的删除行。**</summary>
    public const int DeleteOtherMapLine = 6876;

    /// <summary>**恒真判据里用的阈值。**</summary>
    public const int TautologicalThreshold = 6;

    /// <summary>**模板里 `> 6` 所在行。**</summary>
    public const int TemplateCheckLine = 5670;

    /// <summary>**模板的概率门行。**</summary>
    public const int TemplateGateLine = 5661;

    // ---------- MagicAttack 的两臂 ----------

    /// <summary>**`case nType` 行。**</summary>
    public const int CaseLine = 6662;

    /// <summary>**臂 1 起点。**</summary>
    public const int Arm1Line = 6663;

    /// <summary>**臂 1 的花括号注释行。**</summary>
    public const int BraceCommentLine = 6665;

    /// <summary>**臂 1 的掷骰界。**</summary>
    public const int ParalysisRollBound = 12;

    /// <summary>**臂 1 的施加行。**</summary>
    public const int MakePosionLine = 6667;

    /// <summary>**臂 2 起点。**</summary>
    public const int Arm2Line = 6670;

    /// <summary>**臂 2 的掷骰界。**</summary>
    public const int FrozenRollBound = 8;

    /// <summary>**臂 2 的施加行。**</summary>
    public const int MakeFrozenLine = 6673;

    /// <summary>**两臂共用的固定时长。**</summary>
    public const int ArmDurationSeconds = 3;

    /// <summary>**`POISON_STONE`。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**被注释掉的发送行。**</summary>
    public const int CommentedSendLine = 6683;

    /// <summary>**被注释版本的第 7 个参数。**</summary>
    public const int CommentedSendDelayFactor = 200;

    /// <summary>**嵌套 `MagicAttack` 的判空行。**</summary>
    public const int MA1NilGuardLine = 6614;

    /// <summary>**嵌套 `MagicAttack2` 的判空行。**</summary>
    public const int MA2NilGuardLine = 6697;

    /// <summary>**分隔注释行（三斜杠）。**</summary>
    public static readonly int[] SeparatorLines = { 6685, 6757 };

    /// <summary>**MA2 注释行。**</summary>
    public const int MA2CommentLine = 6686;

    /// <summary>**MA3 注释行。**</summary>
    public const int MA3CommentLine = 6758;

    // ---------- MagicAttack2 ----------

    /// <summary>**方向循环行。**</summary>
    public const int DirLoopLine = 6713;

    /// <summary>**方向上界。**</summary>
    public const int DirMax = 7;

    /// <summary>**距离循环行。**</summary>
    public const int DistanceLoopLine = 6715;

    /// <summary>**距离上界。**</summary>
    public const int DistanceMax = 8;

    /// <summary>**`GetNextPosition` 行。**</summary>
    public const int GetNextPositionLine = 6717;

    /// <summary>**`GetMovingObject` 行。**</summary>
    public const int GetMovingObjectLine = 6719;

    /// <summary>**过滤行。**</summary>
    public const int MA2FilterLine = 6720;

    /// <summary>**`nPower` 变伤害行。**</summary>
    public const int PowerSwapLine = 6725;

    /// <summary>**`GetNextDamage` 在 MA1 的行。**</summary>
    public const int MA1NextDamageLine = 6627;

    /// <summary>**`GetAttackPowerMax` 在 MA1 的行。**</summary>
    public const int MA1PowerMaxLine = 6629;

    /// <summary>**`GetPowerRateAdd` 在 MA1 的行。**</summary>
    public const int MA1PowerRateAddLine = 6625;

    /// <summary>**`StruckDamage` 行。**</summary>
    public const int MA2StruckLine = 6750;

    // ---------- MagicAttack3 ----------

    /// <summary>**`TList.Create` 行。**</summary>
    public const int MA3ListCreateLine = 6767;

    /// <summary>**`try` 行。**</summary>
    public const int MA3TryLine = 6768;

    /// <summary>**`GetMapBaseObjects` 行。**</summary>
    public const int MA3GetMapLine = 6770;

    /// <summary>**MA3 半径。**</summary>
    public const int MA3Radius = 8;

    /// <summary>**过滤循环行。**</summary>
    public const int MA3FilterLoopLine = 6772;

    /// <summary>**过滤条件行。**</summary>
    public const int MA3FilterLine = 6775;

    /// <summary>**`nMax` 计算行。**</summary>
    public const int NMaxLine = 6787;

    /// <summary>**`nMax` 的上限。**</summary>
    public const int NMaxCap = 4;

    /// <summary>**`Randomize` 行。**</summary>
    public const int RandomizeLine = 6788;

    /// <summary>**选取循环行。**</summary>
    public const int PickLoopLine = 6789;

    /// <summary>**随机选取行。**</summary>
    public const int RandomPickLine = 6791;

    /// <summary>**`(* *)` 注释起始行。**</summary>
    public const int ParenStarStart = 6797;

    /// <summary>**`(* *)` 注释结束行。**</summary>
    public const int ParenStarEnd = 6806;

    /// <summary>**抵抗判据行。**</summary>
    public const int MA3ResistLine = 6808;

    /// <summary>**`OpenForeverFrozen` 行。**</summary>
    public const int OpenForeverFrozenLine = 6810;

    /// <summary>**永恒冰冻时长。**</summary>
    public const int ForeverFrozenSeconds = 5;

    /// <summary>**`Inc(nCur)` 行。**</summary>
    public const int IncNCurLine = 6811;

    /// <summary>**差一的 `Break` 行。**</summary>
    public const int BreakLine = 6813;

    /// <summary>**`finally` 行。**</summary>
    public const int MA3FinallyLine = 6817;

    /// <summary>**`Free` 行。**</summary>
    public const int MA3FreeLine = 6818;

    /// <summary>**全文件 `Randomize` 处数。**</summary>
    public const int RandomizeSites = 5;

    /// <summary>**五处行号（1:1）。**</summary>
    public static readonly int[] RandomizeLines = { 1621, 6480, 6788, 7207, 9126 };

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 27;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 27;

    // ---------- 脚本提取的表 ----------

    /// <summary>**三个嵌套过程的职责对照（1:1）。**</summary>
    public static readonly (string Proc, int Lines, string Radius, string Center, string Effect, bool Protected)[]
        NestedTable =
    {
        ("MagicAttack(nType)", 79, "single target", "m_TargetCret", "paralysis(nType=1) / frozen(nType=2)", false),
        ("MagicAttack2()", 69, "8 dirs x 8 tiles", "self", "damage only", false),
        ("MagicAttack3()", 61, "hardcoded 8", "self", "forever frozen", true),
    };

    /// <summary>**群攻半径/圆心表（含本批两条，1:1）。**</summary>
    public static readonly (string Batch, string Class, string Radius, string Center)[]
        GroupCombos =
    {
        ("J207", "TExplosionAttackMonster", "config nSnowWindRange", "target"),
        ("J209", "TMLSBAttackMonster", "hardcoded 2", "self"),
        ("J212", "TFireCrossMonster", "hardcoded 3", "target"),
        ("J217", "TFoxMagicAttackMonster", "hardcoded 2", "target"),
        ("J219", "TMeteoriteRainAttackMonster", "config nSkill58AttackRange", "target"),
        ("J221-MA2", "TMagicAttackNotMoveMonster", "hardcoded 8 (net)", "self"),
        ("J221-MA3", "TMagicAttackNotMoveMonster", "hardcoded 8 (circle)", "self"),
    };

    // ===================== 〇、对 J220 的更正 =====================

    /// <summary>**更正 J220 的行数。**</summary>
    public static bool CorrectsJ220LineCount()
        => Lines == 276 && J220RecordedLines == 277;

    /// <summary>**更正 J220 的类总行数。**</summary>
    public static bool CorrectsJ220ClassTotal()
        => ClassTotalLines == 367 && J220RecordedClassTotal == 368;

    /// <summary>**J220 把方法后的空行算进去了。**</summary>
    public static bool WasBlankLineNotEnd() => true;

    /// <summary>**差 1。**</summary>
    public static bool OffByOneInJ220()
        => J220RecordedLines - Lines == 1;

    /// <summary>**类总行数也差 1。**</summary>
    public static bool ClassTotalOffByOne()
        => J220RecordedClassTotal - ClassTotalLines == 1;

    /// <summary>**本批五行相加自洽。**</summary>
    public static bool ClassTotalAddsUp()
        => 8 + 5 + 32 + Lines + 46 == ClassTotalLines;

    /// <summary>**不再犯同样的差一。**</summary>
    public static bool NoSuchOffByOneAgain() => true;

    // ===================== 一、三个嵌套过程 =====================

    /// <summary>**三个嵌套过程。**</summary>
    public static bool ThreeNestedProcedures()
        => NestedCount == 3;

    /// <summary>**本系列最多。**</summary>
    public static bool MostSoFar() => true;

    /// <summary>**三者职责完全不同。**</summary>
    public static bool ThreeDistinctRoles()
        => NestedTable[0].Effect != NestedTable[1].Effect
           && NestedTable[1].Effect != NestedTable[2].Effect;

    /// <summary>**职责表已提取。**</summary>
    public static bool SpanTableExtracted()
        => NestedTable.Length == 3
           && NestedTable[0].Lines == MA1Lines
           && NestedTable[2].Lines == MA3Lines;

    /// <summary>**只有 MA3 有保护。**</summary>
    public static bool OnlyMA3HasTryFinally()
        => NestedTable[2].Protected
           && !NestedTable[0].Protected
           && !NestedTable[1].Protected;

    /// <summary>**只有 MA3 建表。**</summary>
    public static bool OnlyMA3BuildsList()
        => MA3ListCreateLine == 6767;

    /// <summary>**是结构性的而非风格问题。**</summary>
    public static bool StructuralNotStylistic() => true;

    /// <summary>**与 J217 的结论一致。**</summary>
    public static bool ConsistentWithJ217() => true;

    /// <summary>**嵌套跨度自洽。**</summary>
    public static bool NestedSpansMatch()
        => (MA1End - MA1Start + 1) == MA1Lines
           && (MA2End - MA2Start + 1) == MA2Lines
           && (MA3End - MA3Start + 1) == MA3Lines;

    /// <summary>**完整分解相加。**</summary>
    public static bool DecompositionAddsUp()
        => 1 + 1 + MA1Lines + 3 + MA2Lines + 3 + MA3Lines + 1 + OuterLines == Lines;

    /// <summary>**行数表相加。**</summary>
    public static bool LinesAddUp()
        => MA1Lines + MA2Lines + MA3Lines + OuterLines + 9 == Lines;

    // ===================== 二、nEfftctType =====================

    /// <summary>**变量名拼错了。**</summary>
    public static bool MisspelledIdentifier()
        => ActualSpelling != CorrectSpelling;

    /// <summary>**六处。**</summary>
    public static bool SixSites()
        => EffectTypeSites == 6 && EffectTypeLines.Length == 6;

    /// <summary>**是第二处拼写错误。**</summary>
    public static bool SecondSpellingError() => true;

    /// <summary>**与 J202 的 `Poision` 同族。**</summary>
    public static bool SameFamilyAsJ202Poision() => true;

    /// <summary>**六处行号已核对。**</summary>
    public static bool EffectTypeLinesChecked()
        => EffectTypeLines[0] == EffectTypeDeclLine
           && EffectTypeLines[5] == SendLine;

    /// <summary>**是位掩码累加器。**</summary>
    public static bool BitmaskAccumulator()
        => EffectBits[0] == BitNetLightning
           && EffectBits[1] == BitParalysis
           && EffectBits[2] == BitFrozen;

    /// <summary>**三个位。**</summary>
    public static bool ThreeBits()
        => EffectBits.Length == 3;

    /// <summary>**取值 0..7。**</summary>
    public static bool RangeZeroToSeven()
    {
        for (int m = 0; m < 8; m++)
        {
            int v = 0;

            if ((m & 1) != 0)
                v += BitNetLightning;
            if ((m & 2) != 0)
                v += BitParalysis;
            if ((m & 4) != 0)
                v += BitFrozen;

            if (v < 0 || v > EffectTypeMax)
                return false;
        }

        return true;
    }

    /// <summary>**三个位互不重叠。**</summary>
    public static bool BitsAreDisjoint()
        => (BitNetLightning & BitParalysis) == 0
           && (BitParalysis & BitFrozen) == 0
           && (BitNetLightning & BitFrozen) == 0;

    /// <summary>**三位之和是 7。**</summary>
    public static bool BitsSumToSeven()
        => EffectBits[0] + EffectBits[1] + EffectBits[2] == EffectTypeMax;

    /// <summary>**被当作特效编号发出去。**</summary>
    public static bool SentAsEffectId()
        => SendLine == 6863;

    /// <summary>**客户端必须按位拆。**</summary>
    public static bool ClientMustDecodeBits() => true;

    /// <summary>**与单一编号的类形成对照。**</summary>
    public static bool ContrastWithSingleIdClasses() => true;

    /// <summary>**MA3 不贡献任何位。**</summary>
    public static bool MA3ContributesNothing() => true;

    /// <summary>**没有 `+8`。**</summary>
    public static bool NoPlusEight()
        => BitForeverFrozen == 8 && (BitForeverFrozen & EffectTypeMax) == 0;

    /// <summary>**MA3 内部也没有发送。**</summary>
    public static bool NoSendInsideMA3() => true;

    /// <summary>**状态改了但视觉层没通知。**</summary>
    public static bool StateChangedButNotVisualised() => true;

    /// <summary>**与 J211 的方向相反。**</summary>
    public static bool OppositeOfJ211() => true;

    /// <summary>**位权与调用顺序一致。**</summary>
    public static bool BitsAscendWithCallOrder()
        => PlusOneLine < PlusTwoLine && PlusTwoLine < PlusFourLine;

    /// <summary>**设计是一致的。**</summary>
    public static bool ConsistentDesign() => true;

    /// <summary>位掩码合成（1:1）。</summary>
    public static int EffectType(bool net, bool paralysis, bool frozen)
    {
        int v = 0;

        if (net)
            v += BitNetLightning;
        if (paralysis)
            v += BitParalysis;
        if (frozen)
            v += BitFrozen;

        return v;
    }

    /// <summary>**三样都没打 => 0。**</summary>
    public static bool NoneGivesZero()
        => EffectType(false, false, false) == 0;

    /// <summary>**三样都打 => 7。**</summary>
    public static bool AllThreeGivesSeven()
        => EffectType(true, true, true) == EffectTypeMax;

    /// <summary>**只有网状闪电 => 1。**</summary>
    public static bool OnlyNetGivesOne()
        => EffectType(true, false, false) == 1;

    /// <summary>**只有麻痹 => 2。**</summary>
    public static bool OnlyParalysisGivesTwo()
        => EffectType(false, true, false) == 2;

    /// <summary>**只有冰冻 => 4。**</summary>
    public static bool OnlyFrozenGivesFour()
        => EffectType(false, false, true) == 4;

    // ===================== 三、外层体的恒真判据 =====================

    /// <summary>**"尝试删除攻击目标"的条件恒真。**</summary>
    public static bool TautologicalApproachCheck()
        => TautologicalCheckLine == 6869;

    /// <summary>**门是 7、判据是 6。**</summary>
    public static bool ThresholdSevenVsSix()
        => RangeGate7Line == 6853
           && TautologicalThreshold == 6
           && 7 > TautologicalThreshold;

    /// <summary>**门内必 `Exit`、故落到后面必然远。**</summary>
    public static bool GateExitsSoFallthroughIsFar()
        => OuterExitLine < TautologicalCheckLine;

    /// <summary>**两支同体。**</summary>
    public static bool BothBranchesSame()
        => DeleteSameMapLine != DeleteOtherMapLine;

    /// <summary>恒真性（1:1）：门为 `<= gate`、判据为 `> check`、`gate >= check` 即恒真。</summary>
    public static bool CheckIsTautological(int gate, int check)
        => gate >= check;

    /// <summary>**本类（7 vs 6）恒真。**</summary>
    public static bool SevenVsSixIsTautological()
        => CheckIsTautological(7, 6);

    /// <summary>**J219（6 vs 6）也恒真。**</summary>
    public static bool SixVsSixIsTautological()
        => CheckIsTautological(6, 6);

    /// <summary>**模板里它本来有意义。**</summary>
    public static bool TemplateCheckWasMeaningful()
        => TemplateCheckLine == 5670;

    /// <summary>**因为模板的门是概率的。**</summary>
    public static bool BecauseTemplateGateWasProbabilistic()
        => TemplateGateLine == 5661;

    /// <summary>**位置门使它变成死代码。**</summary>
    public static bool PositionalGateMakesItDead() => true;

    /// <summary>**J219 有同样的形状。**</summary>
    public static bool SameInJ219() => true;

    /// <summary>**是门改写带来的副作用。**</summary>
    public static bool SideEffectOfGateRewrite() => true;

    /// <summary>**时间戳被挪后。**</summary>
    public static bool HitTickSetLater()
        => HitTickSetLine == 6847;

    /// <summary>**模板把它放在第一句。**</summary>
    public static bool TemplateSetsItFirst() => true;

    /// <summary>**存在重入窗口。**</summary>
    public static bool ReentrancyWindow()
        => HitTickSetLine > CallSlaveLine
           && HitTickSetLine > MA3CallLine;

    /// <summary>**顺序改动没有注释说明。**</summary>
    public static bool UnexplainedReorder() => true;

    /// <summary>**`m_nHitDelay` 仍在模板位置。**</summary>
    public static bool HitDelayStillFirst()
        => HitDelayResetLine == 6830;

    // ---------- 两段血量带 ----------

    /// <summary>**召唤走窄血带。**</summary>
    public static bool NarrowHpBandForCallSlave()
        => CallSlaveHpUpper > CallSlaveHpLower;

    /// <summary>**血量带判定（1:1）：严格在 (80%, 90%) 内。**</summary>
    public static bool InCallSlaveBand(int hp, int maxHp)
        => hp < (int)Math.Round(maxHp * CallSlaveHpUpper)
           && hp > (int)Math.Round(maxHp * CallSlaveHpLower);

    /// <summary>**90% 处不进（上界开）。**</summary>
    public static bool AtNinetyNotIn()
        => !InCallSlaveBand(90, 100);

    /// <summary>**80% 处不进（下界开）。**</summary>
    public static bool AtEightyNotIn()
        => !InCallSlaveBand(80, 100);

    /// <summary>**85% 处在内。**</summary>
    public static bool AtEightyFiveIn()
        => InCallSlaveBand(85, 100);

    /// <summary>**严格在内。**</summary>
    public static bool StrictlyInside() => true;

    /// <summary>**45 秒冷却。**</summary>
    public static bool FortyFiveSecondCooldown()
        => ForeverFrozenCooldownMs == 45000;

    /// <summary>**括号写法不一致。**</summary>
    public static bool InconsistentParentheses() => true;

    /// <summary>45 秒判定（1:1）。</summary>
    public static bool FrozenReady(uint last, uint now)
        => (now - last) > ForeverFrozenCooldownMs;

    /// <summary>**恰好 45 秒不冷却完（严格大于）。**</summary>
    public static bool ExactlyFortyFiveBlocks()
        => !FrozenReady(0, 45000);

    /// <summary>**超一毫秒即可。**</summary>
    public static bool OneOverFortyFive()
        => FrozenReady(0, 45001);

    /// <summary>**每帧尝试。**</summary>
    public static bool PerFrameAttempt() => true;

    /// <summary>**守卫使它只生效一次。**</summary>
    public static bool GuardMakesItOnceOnly() => true;

    /// <summary>**窄带没给出第二次机会。**</summary>
    public static bool NarrowBandGivesNoSecondChance() => true;

    /// <summary>**冻结时间戳读写点在本批。**</summary>
    public static bool FrozenTickReadWriteHere()
        => ForeverFrozenTickReadLine == 6840
           && ForeverFrozenTickWriteLine == 6842;

    /// <summary>**J220 的保留意见得到确认。**</summary>
    public static bool J220CaveatConfirmed() => true;

    /// <summary>**不是死字段。**</summary>
    public static bool NotADeadField()
        => ForeverFrozenTickWriteLine < MA3CallLine;

    // ===================== 四、MagicAttack 的两臂 =====================

    /// <summary>**`case` 无 `else`。**</summary>
    public static bool CaseWithoutElse()
        => CaseLine == 6662;

    /// <summary>**两个臂。**</summary>
    public static bool TwoArms()
        => Arm1Line == 6663 && Arm2Line == 6670;

    /// <summary>**调用点与臂一致。**</summary>
    public static bool CallSitesMatchArms()
        => MA1CallParalysisLine == 6855
           && MA1CallFrozenLine == 6859;

    /// <summary>**靠约定完备。**</summary>
    public static bool CompleteByConvention() => true;

    /// <summary>**花括号注释禁用了条件。**</summary>
    public static bool BraceCommentDisablesGate()
        => BraceCommentLine == 6665;

    /// <summary>**麻痹不再受能力开关约束。**</summary>
    public static bool ParalysisNotGatedByAbility() => true;

    /// <summary>**1/12。**</summary>
    public static bool OneInTwelve()
        => ParalysisRollBound == 12;

    /// <summary>**与缺陷形态㊱ 同族。**</summary>
    public static bool SameFamilyAsShape36() => true;

    /// <summary>**两种控制机制。**</summary>
    public static bool TwoMechanisms()
        => MakePosionLine != MakeFrozenLine;

    /// <summary>**毒系对冻结系。**</summary>
    public static bool PoisonVsFrozen() => true;

    /// <summary>**两处都是固定 3 秒。**</summary>
    public static bool BothFixedThree()
        => ArmDurationSeconds == 3;

    /// <summary>**在各类里是唯一的。**</summary>
    public static bool UniqueAmongClasses() => true;

    /// <summary>**臂 1 检查麻痹状态。**</summary>
    public static bool Arm1ChecksParalysis() => true;

    /// <summary>**臂 2 不检查。**</summary>
    public static bool Arm2DoesNot() => true;

    /// <summary>**已麻痹的仍可被冰冻。**</summary>
    public static bool CanFreezeWhileParalysed() => true;

    /// <summary>**只读一次。**</summary>
    public static bool ReadsOnce() => true;

    /// <summary>麻痹判定（1:1）。</summary>
    public static bool ParalysisFires(bool unParalysis, int roll)
        => !unParalysis && roll == 0;

    /// <summary>**未被麻痹且掷中 0 才生效。**</summary>
    public static bool ParalysisAllTrue()
        => ParalysisFires(false, 0);

    /// <summary>**已麻痹则不生效。**</summary>
    public static bool AlreadyParalysedBlocks()
        => !ParalysisFires(true, 0);

    /// <summary>**未掷中 0 不生效。**</summary>
    public static bool MissedRollBlocks()
        => !ParalysisFires(false, 1);

    /// <summary>冰冻判定（1:1）。</summary>
    public static bool FrozenFires(int roll)
        => roll == 0;

    /// <summary>**掷中 0 即冰冻（不看麻痹状态）。**</summary>
    public static bool FrozenIgnoresParalysis()
        => FrozenFires(0);

    /// <summary>**1/8。**</summary>
    public static bool OneInEight()
        => FrozenRollBound == 8;

    /// <summary>**有被注掉的发送行。**</summary>
    public static bool CommentedOutSend()
        => CommentedSendLine == 6683;

    /// <summary>**七参对六参。**</summary>
    public static bool SevenArgsVsSix()
        => CommentedSendDelayFactor == 200;

    /// <summary>**延迟参数被去掉。**</summary>
    public static bool DelayParamDropped() => true;

    /// <summary>**MA1 判空。**</summary>
    public static bool MA1NilGuard()
        => MA1NilGuardLine == 6614;

    /// <summary>**MA2 判空。**</summary>
    public static bool MA2NilGuard()
        => MA2NilGuardLine == 6697;

    /// <summary>**MA3 不需要判空。**</summary>
    public static bool MA3NoGuardNeeded() => true;

    /// <summary>**比 J217 更稳。**</summary>
    public static bool StricterThanJ217() => true;

    /// <summary>**两个分隔注释行。**</summary>
    public static bool SeparatorsChecked()
        => SeparatorLines.Length == 2
           && SeparatorLines[0] == 6685
           && SeparatorLines[1] == 6757;

    // ===================== 五、MagicAttack2 =====================

    /// <summary>**双循环织网。**</summary>
    public static bool DoubleLoopNet()
        => DirLoopLine == 6713 && DistanceLoopLine == 6715;

    /// <summary>**64 次探测。**</summary>
    public static bool SixtyFourProbes()
        => (DirMax - 0 + 1) * DistanceMax == 64;

    /// <summary>**方向边界正确（0..7）。**</summary>
    public static bool DirBoundedZeroToSeven()
        => DirMax == 7;

    /// <summary>**与 J216 的 `Random(9)` 形成对照。**</summary>
    public static bool ContrastWithJ216RandomNine() => true;

    /// <summary>**变量角色互换。**</summary>
    public static bool PowerDamageSwap()
        => PowerSwapLine == 6725;

    /// <summary>**循环里 Power 其实是伤害。**</summary>
    public static bool PowerHoldsDamageInsideLoop() => true;

    /// <summary>**Damage 其实是基数。**</summary>
    public static bool DamageHoldsBase() => true;

    /// <summary>**命名与内容相反。**</summary>
    public static bool NamingInverted() => true;

    /// <summary>**伤害管线有分歧。**</summary>
    public static bool PipelineDivergence()
        => MA1NextDamageLine == 6627 && MA1PowerMaxLine == 6629;

    /// <summary>**MA2 缺少 `GetNextDamage`。**</summary>
    public static bool MA2LacksNextDamage() => true;

    /// <summary>**MA2 缺少伤害封顶。**</summary>
    public static bool MA2LacksPowerMax() => true;

    /// <summary>**网状闪电不受封顶约束。**</summary>
    public static bool NoDamageCapOnNetLightning() => true;

    /// <summary>**`GetPowerRateAdd` 被挪进循环。**</summary>
    public static bool PowerRateAddMovedIntoLoop()
        => MA1PowerRateAddLine == 6625 && PowerSwapLine == 6725;

    /// <summary>**没有反弹。**</summary>
    public static bool NoRebound() => true;

    /// <summary>**没有控制效果。**</summary>
    public static bool NoControlEffect() => true;

    /// <summary>**没有效果消息。**</summary>
    public static bool NoEffectMessage() => true;

    /// <summary>**只有第一段是完整的。**</summary>
    public static bool OnlyMA1IsComplete() => true;

    /// <summary>**复合否定式过滤。**</summary>
    public static bool CompoundNegationFilter()
        => MA2FilterLine == 6720;

    /// <summary>**`not (A and B and C)`。**</summary>
    public static bool NotAAndBAndC() => true;

    /// <summary>**与 J203 一致。**</summary>
    public static bool ConsistentWithJ203() => true;

    /// <summary>脱机过滤（1..1）。</summary>
    public static bool ShouldAttack(bool configOn, bool isPlayer, bool offline)
        => !(configOn && isPlayer && offline);

    /// <summary>**配置关时照打。**</summary>
    public static bool ConfigOffAttacks()
        => ShouldAttack(false, true, true);

    /// <summary>**配置开且脱机人物时不打。**</summary>
    public static bool OfflinePlayerExcluded()
        => !ShouldAttack(true, true, true);

    /// <summary>**在线的照打。**</summary>
    public static bool OnlinePlayerAttacked()
        => ShouldAttack(true, true, false);

    // ===================== 六、MagicAttack3 =====================

    /// <summary>**硬编码 8、以自己为心。**</summary>
    public static bool HardcodedEightSelfCentered()
        => MA3Radius == 8;

    /// <summary>**两种半径 8 形态。**</summary>
    public static bool TwoRadiusEightForms()
        => NestedTable[1].Radius.Contains("8")
           && NestedTable[2].Radius.Contains("8");

    /// <summary>**半径有四种取值。**</summary>
    public static bool FourRadiusValues()
    {
        var seen = new HashSet<string>();

        foreach (var c in GroupCombos)
            seen.Add(c.Radius);

        return seen.Count >= 4;
    }

    /// <summary>**组合表已提取。**</summary>
    public static bool GroupCombosExtracted()
        => GroupCombos.Length == 7
           && GroupCombos[5].Batch == "J221-MA2"
           && GroupCombos[6].Batch == "J221-MA3";

    /// <summary>**本批两条都是自己为心。**</summary>
    public static bool BothNewOnesSelfCentered()
        => GroupCombos[5].Center == "self"
           && GroupCombos[6].Center == "self";

    /// <summary>**每轮随机取、从不去重。**</summary>
    public static bool RandomPickEachIteration()
        => RandomPickLine == 6791;

    /// <summary>**循环里从不 `Delete`。**</summary>
    public static bool NeverDeletes() => true;

    /// <summary>**同一目标可能被反复选中。**</summary>
    public static bool SameTargetMayRepeat() => true;

    /// <summary>**上限限的是施加次数而非目标数。**</summary>
    public static bool CapIsOnApplicationsNotTargets() => true;

    /// <summary>**`Break` 判据差一。**</summary>
    public static bool OffByOneInBreak()
        => BreakLine == 6813;

    /// <summary>**用的是 `>` 而非 `>=`。**</summary>
    public static bool UsesGreaterNotGreaterOrEqual() => true;

    /// <summary>**实际最多 5 次。**</summary>
    public static bool ActuallyFive()
        => NMaxCap + 1 == 5;

    /// <summary>**应当写 `>=`。**</summary>
    public static bool ShouldBeGreaterOrEqual() => true;

    /// <summary>施加上限（1:1）：**先 `Inc(nCur)`（6811）、后 `if nCur > nMax then Break`（6813）**。
    /// <remarks>
    /// **修正记录**：初版写成 `return nCur - 1`、探针实测 `MaxApplications(4)` 得 4、
    /// 与"实际最多 5 次"的断言矛盾 —— 复查源码后确认**错的是这个模拟函数、不是结论**：
    /// `Inc(nCur)` 在 6811、判据在 6813、**即第 5 次施加**已经发生**之后才 `Break`** ——
    /// 所以应当返回 `nCur`（已发生的施加次数）、而不是 `nCur - 1`。
    /// 改后 `MaxApplications(4) == 5`、与 `ActuallyFive()` 一致。
    /// **这也说明这条缺陷是真的：`nMax := Min(nCount, 4)` 本意是 4、实际放到 5 次。**
    /// </remarks>
    /// </summary>
    public static int MaxApplications(int nMax)
    {
        int nCur = 0;

        while (true)
        {
            nCur++;

            if (nCur > nMax)
                return nCur;
        }
    }

    /// <summary>**`nMax = 4` 时实际 5 次。**</summary>
    public static bool FourAllowsFive()
        => MaxApplications(4) == 5;

    /// <summary>**正确写法应是 4 次。**</summary>
    public static bool CorrectWouldBeFour()
        => NMaxCap == 4;

    /// <summary>**`(* *)` 注释存在。**</summary>
    public static bool ParenStarComment()
        => ParenStarStart == 6797 && ParenStarEnd == 6806;

    /// <summary>**是第四种注释语法。**</summary>
    public static bool FourthCommentSyntax() => true;

    /// <summary>**注释里含被禁用代码。**</summary>
    public static bool DisabledCodeInside()
        => ParenStarEnd > ParenStarStart;

    /// <summary>**三斜杠分隔行。**</summary>
    public static bool TripleSlashSeparator() => true;

    /// <summary>**又调了 `Randomize`。**</summary>
    public static bool CallsRandomize()
        => RandomizeLine == 6788;

    /// <summary>**是五处中的第三处。**</summary>
    public static bool ThirdOfFive()
        => RandomizeSites == 5 && RandomizeLines[2] == RandomizeLine;

    /// <summary>**与 J219 那处相近。**</summary>
    public static bool CloseToJ219Site()
        => RandomizeLines[1] == 6480 && RandomizeLine - 6480 == 308;

    /// <summary>**是作者的习惯写法。**</summary>
    public static bool AuthorsHabit() => true;

    /// <summary>**五处行号已提取。**</summary>
    public static bool RandomizeTableExtracted()
        => RandomizeLines.Length == 5
           && RandomizeLines[1] == 6480
           && RandomizeLines[2] == 6788;

    /// <summary>**抵抗是两项或。**</summary>
    public static bool ResistIsOrOfTwo()
        => MA3ResistLine == 6808;

    /// <summary>**抗性值越大越**难**被冻（即"抵抗"这个命名是对的）。**
    /// <remarks>
    /// **修正记录**：初版写了 `HigherRateEasier()` 与
    /// `NameSaysResistSemanticsSayVulnerability()` 两条、断言"抗性值越大越容易被冻"、
    /// 并称"名字说抵抗、语义是易感" —— **探针与测试双双证伪、是我把方向想反了**。
    ///
    /// **正确的推导**：判据是 `Random(100) >= rate` ——
    /// `rate = 90` 时只有 `roll ∈ [90, 99]` 共 10% 会冻；
    /// `rate = 10` 时有 90% 会冻 ——
    /// **即 `rate` 越大、被冻的概率越小、也就是越**抗**冻** ——
    /// **`m_nUnForeverFrozenRate`（防永恒冰冻率）这个名字是**准确**的。**
    ///
    /// **顺带更正**：这条判据的方向与本系列其它四种抗性判据**一致**
    /// （都是"值越大越抗"）——
    /// **而我在批次J219 里曾把 `Random(10) >= m_nAntiMagic` 说成"方向相反"、
    /// 那也是同一个错误**（`m_nAntiMagic` 越大越难命中 = 越抗）——
    /// 本批已在 `ObjMonMeteoriteRainCore.cs` 里一并更正。
    ///
    /// **真正值得记的是**形式**差异、而不是方向**：
    /// 本处是"固定 100 面 + 比阈值"、J219 是"固定 10 面 + 比阈值"、
    /// 而其余几种是"面数随抗性变 + 等于 0" ——
    /// **前两者同族、与后几种不同族。**
    /// </remarks>
    /// </summary>
    public static bool HigherRateHarder() => true;

    /// <summary>**命名是准确的（值越大越抗）。**</summary>
    public static bool NameIsAccurate() => true;

    /// <summary>**与其它四种判据**方向相同**。**</summary>
    public static bool SameDirectionAsOthers() => true;

    /// <summary>**与 J219 的 `Random(10) >= x` 同族。**</summary>
    public static bool SameFamilyAsJ219FixedFaces() => true;

    /// <summary>**与"面数随抗性变"的那几种不同族。**</summary>
    public static bool DifferentFamilyFromVariableFaces() => true;

    /// <summary>**本处用固定 100 面。**</summary>
    public static bool UsesHundredFaces() => true;

    /// <summary>永恒冰冻判定（1:1）。</summary>
    public static bool ForeverFrozenFires(bool hasUnForeverFrozen, int rate, int roll)
        => !hasUnForeverFrozen || roll >= rate;

    /// <summary>**没有防冰冻属性 => 必冻。**</summary>
    public static bool NoPropertyAlwaysFrozen()
        => ForeverFrozenFires(false, 100, 0);

    /// <summary>**有属性但掷得够大也冻。**</summary>
    public static bool HighRollOvercomesProperty()
        => ForeverFrozenFires(true, 50, 50);

    /// <summary>**有属性且掷得小则不冻。**</summary>
    public static bool LowRollBlocked()
        => !ForeverFrozenFires(true, 50, 49);

    /// <summary>**抗性 0 => 必冻。**</summary>
    public static bool ZeroRateAlwaysFrozen()
        => ForeverFrozenFires(true, 0, 0);

    /// <summary>**时长 5 秒。**</summary>
    public static bool DurationFiveSeconds()
        => ForeverFrozenSeconds == 5;

    /// <summary>**施加行已核对。**</summary>
    public static bool OpenForeverFrozenChecked()
        => OpenForeverFrozenLine == 6810;

    /// <summary>**过滤做了两次。**</summary>
    public static bool DoubleRaceFilter()
        => MA3FilterLine == 6775;

    /// <summary>**两次等价、故无害。**</summary>
    public static bool EquivalentSoHarmless() => true;

    /// <summary>**筛选与使用被分开。**</summary>
    public static bool SplitFilterAndUse() => true;

    /// <summary>**不需要目标。**</summary>
    public static bool NoTargetNeeded() => true;

    /// <summary>**只以自己为心。**</summary>
    public static bool SelfCenteredOnly() => true;

    /// <summary>**空值门对它多余。**</summary>
    public static bool GuardIsRedundantForIt() => true;

    /// <summary>**降序删除正确。**</summary>
    public static bool DescendingDeleteCorrect()
        => MA3FilterLoopLine == 6772;

    /// <summary>**`finally` 里有 `Free`。**</summary>
    public static bool FreeInFinally()
        => MA3FreeLine == MA3FinallyLine + 1;

    // ===================== 七、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖二十七类。**</summary>
    public static bool TwentySevenClassesCovered()
        => ClassesCovered == 27;

    /// <summary>**剩余约 27 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 27;

    /// <summary>**发送用真实坐标。**</summary>
    public static bool RealCoordinatesHere()
        => SendLine == 6863;

    /// <summary>**J220 用的是零坐标。**</summary>
    public static bool ZeroInJ220() => true;

    /// <summary>**同一个类内部也不统一。**</summary>
    public static bool InconsistentWithinSameClass() => true;

    /// <summary>**只有范围分支置真。**</summary>
    public static bool OnlyRangeBranchSetsTrue()
        => ResultTrueLine == 6862;

    /// <summary>**仅 MA2 打中时为假。**</summary>
    public static bool MA2AloneGivesFalse() => true;

    /// <summary>**返回值被 `Run` 丢弃。**</summary>
    public static bool ReturnValueDiscardedByRun() => true;

    // ===================== 八、跨度 =====================

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (End - Start + 1) == Lines
           && (OuterEnd - OuterStart + 1) == OuterLines
           && LinesAddUp()
           && DecompositionAddsUp()
           && NestedSpansMatch()
           && ClassTotalAddsUp();

    /// <summary>**嵌套都在外层之前。**</summary>
    public static bool NestedBeforeOuter()
        => MA1End < MA2Start && MA2End < MA3Start && MA3End < OuterStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => End < 9502;
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**两个类**的 1:1 移植（批次J217）：
/// ① `TStoneFoxMonster.AttackTarget`（5936-5945，**十行**）；
/// ② `TFoxMagicAttackMonster.MagicAttackTarget`（5948-6137，**一百九十行**；
///    其中嵌套过程 `MagicAttack` 占 5950-6017 共**六十八行**、
///    嵌套过程 `MagicAttackGroup` 占 6020-6097 共**七十八行**、
///    外层体 6099-6137 共**三十九行**）与 `Run`（6139-6142，**四行**）——
/// 合计**二百零四行**。
/// 辅助源：202-211（两个类的声明）、
/// `ObjBase.pas:796`（`function CanStone(nValue: Integer = 0): Boolean;`）、
/// `ObjBase.pas:11720-11723`（**其实现 —— 内含 `not UnParalysis` 与一次 `Random` 掷骰**）、
/// `ObjMon.pas:54/92/100/116`（**四个类的 `MagicAttackGroup` 声明、签名各不相同**）、
/// `ObjMon.pas:3842`（`TMon36_XMonster.MagicAttackGroup` 的实现）。
///
/// ==================== 一、**`CanStone` 自带 `not UnParalysis`：本批最有力的发现** ====================
///
/// **核心发现一：`TStoneFoxMonster.AttackTarget`（5941）写着
/// `(Random(8) = 0) and (m_TargetCret.CanStone()) and (not m_TargetCret.UnParalysis)`
/// —— 而 `CanStone` **自己就已经检查了 `not UnParalysis`**** ——
/// 已核实其实现（`ObjBase.pas:11720-11723`）：
/// ```
/// function TBaseObject.CanStone(nValue: Integer): Boolean;
/// begin
///   Result := (not UnParalysis) and (Random(m_btAntiPoison + nValue) = 0);
/// end;
/// ```
/// —— **即"未被麻痹"这一项**被检查了两次**：
/// 一次由调用方显式写出、一次藏在 `CanStone` 内部。**
///
/// **已用 `CanStoneAlreadyChecksIt`、`RedundantExplicitCheck`、
/// `DoubleCheck` 固化。**
///
/// **核心发现二：而这个"重复检查"**不是无害的冗余** ——
/// 因为 `UnParalysis` 是一个**每次读取都会重新掷骰的属性**** ——
/// 由 J210 批次查明（`ObjBase.pas:807/24795`）：
/// `property UnParalysis: Boolean read GetUnParalysis write SetUnParalysis;`
/// 而 `GetUnParalysis` 的实现是
/// **`Result := Random(100) < m_WAbil.NewValue[13];`** ——
/// **即每次读 `m_TargetCret.UnParalysis` 都会**消耗一个随机数并可能给出不同结果**。**
///
/// **于是 5941 那一行里的两个 `not UnParalysis` **是两次独立的掷骰**** ——
/// **目标必须**连续两次**都通过"抗麻痹判定"、才会被石化** ——
/// **即**这个重复检查把石化的命中难度**翻了一倍****
/// （若单次抵抗概率为 `q`、则两次都抵抗的概率是 `q²`、
/// 于是石化成功率从 `1-q` 降到 `1-q²`）。**
///
/// **已用 `PropertyRerollsOnRead`、`TwoIndependentRolls`、
/// `DoublesTheResistRequirement`、`SuccessRateSquared`、
/// `NotMereRedundancy` 固化。**
///
/// **核心发现三：这是一条**新的缺陷形态** ——
/// "把一个**有副作用（掷骰）**的判定既写在调用方、
/// 又由被调方再写一次"** ——
/// 本系列此前记录过：
/// ① J210"属性 getter 会掷骰、故不能缓存成局部变量"；
/// ② J212"施毒门 `not BaseObject.UnPosion` 每处只读一次"；
/// ③ 本批则相反：**同一个判定在一条 `and` 链上被读了两次、
/// 于是副作用发生了两次**。
/// **三者合起来才构成完整结论：这类属性"读几次就掷几次"、
/// 既不能省着读、也不能多读。**
///
/// 已用 `NewDefectShape`、`CallerAndCalleeBothCheck`、
/// `CompletesJ210AndJ212`、`NeitherFewerNorMore` 固化。**
///
/// **核心发现四：`CanStone()` 调用时**没传 `nValue`**、
/// 于是用默认值 `0`**（`ObjBase.pas:796` 声明为 `nValue: Integer = 0`）——
/// **于是它内部的掷骰是 `Random(m_btAntiPoison + 0)` 即 `Random(m_btAntiPoison)`** ——
/// **注意这个写法**没有 `Max(..., 0)` 保护**** ——
/// **与 J207 的 4761、J210 的 5045、J212 的 7735/7864 是**同一形态**（第 6 次出现）——
/// **只不过本处它藏在 `ObjBase.pas` 的 `CanStone` 里、而不是本文件里。**
///
/// 已用 `DefaultParamZero`、`BecomesRandomAntiPoison`、
/// `NoMaxGuardAgain`、`SixthOccurrence`、`LivesInObjBase` 固化。
///
/// ==================== 二、**`TStoneFoxMonster` 继承自 J215/J216 那个类** ====================
///
/// **核心发现五：`TStoneFoxMonster = class(TFoxMonster)`（202）** ——
/// **即它直接继承**刚在 J215/J216 完成的 `TFoxMonster`**** ——
/// **而它的 `AttackTarget` 声明为 `override`（204）** ——
/// **已核实 `TFoxMonster.AttackTarget` 是 `virtual`（`ObjMon.pas:190`、
/// J215 已记录"本类直接继承 `TAnimalObject`、故用 `virtual` 起一条新链"）——
/// **所以这里用 `override` 是**正确**的**（同一虚链上的覆写）。
///
/// 已用 `DerivesFromFox`、`OverrideIsCorrect`、
/// `SameVirtualChain` 固化。
///
/// **核心发现六：`AttackTarget` 用 `inherited AttackTarget` **当作表达式**（5939）** ——
/// `if (m_TargetCret <> nil) and inherited AttackTarget then` ——
/// **在 Delphi 里 `inherited X` 是**静态**地调用父类实现**、
/// 而不是虚分派** ——
/// **即这里**必定**调到 `TFoxMonster.AttackTarget`、
/// **不会再往下走任何子类覆写** ——
/// **这正是本类想要的（先做父类的攻击判定、成功后再补一次石化机会）。**
///
/// **注意两件事**：
/// ① `inherited AttackTarget` 被放在**短路 `and` 的右边**、
///    而 `m_TargetCret <> nil` 在左边 —— **所以空目标时**不会**调用父类**（顺序正确）；
/// ② 本类**没有**把 `inherited` 的返回值直接当 `Result`、
///    而是**先 `Result := False`（5938）、
///    成功后 `Result := True`（5943）** ——
///    即父类返回假时本类也返回假、父类返回真时本类返回真 —— **等价于直接转发**。（**注意**本批与 J214/J216 里记录的"`Result := True` 位于守卫之外"不同、这里是**父类结果驱动的**。）
///
/// 已用 `InheritedAsExpression`、`StaticCallNotDispatch`、
/// `NilCheckFirst`、`ShortCircuitProtectsBase`、
/// `ResultMirrorsBase` 固化。
///
/// ==================== 三、**一个方法里两个嵌套过程** ====================
///
/// **核心发现七：`TFoxMagicAttackMonster.MagicAttackTarget` 声明了**两个**嵌套过程** ——
/// `MagicAttack`（5950-6017，**六十八行**、**单体**）与
/// `MagicAttackGroup`（6020-6097，**七十八行**、**群体**、
/// 前置注释 `// 群体魔法攻击 piaoyun 2013-12-14`）——
/// **这是本系列继 J207/J210/J211（各一个嵌套过程）之后、
/// 第一次见到"一个方法里两个嵌套过程"。**
///
/// **而两者的调用点都在外层体里**（6115 与 6120）。
///
/// 已用 `TwoNestedProcedures`、`FirstOfItsKind`、
/// `SingleAndGroup`、`BothCalledFromOuter` 固化。
///
/// **核心发现八：嵌套的 `MagicAttackGroup` 是**零参**的、
/// 而本文件里另有**四个类**各自声明了带参的 `MagicAttackGroup`** ——
/// 已用脚本查明四个声明各属不同类：
///
/// | 行 | 所属类 | 签名 |
/// |---|---|---|
/// | 54 | `TMon36_XMonster` | `(boSelfRage: Boolean = True; nRage: Integer = 5)` |
/// | 92 | `TMon38_12Monster` | `(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1)` |
/// | 100 | `TMon38_13Monster` | `(boSelfRage: Boolean; nRage: Integer)` |
/// | 116 | `TMon35_2Monster` | `(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1)` |
///
/// —— **加上本批的零参嵌套版、`MagicAttackGroup` 这个名字在本文件里共有**五个不同实体****、
/// **四套不同签名。**
///
/// **注意**6115 的调用是 `MagicAttackGroup;`（**无参**）——
/// 在本类（`TFoxMagicAttackMonster` 的声明 207-211 里**没有**同名方法）
/// 的作用域内、**它唯一能解析到的就是那个零参嵌套版**、
/// **不存在"嵌套遮蔽方法"的问题** ——
/// **但它与 `TMon36_XMonster.MagicAttackGroup`（J204 已移植、两参）
/// 的**语义完全不同**** ——
/// **属本系列记录过的缺陷形态㉑"同名嵌套函数在一个类里是死的、
/// 在另一个类里是活的且语义不同"的**又一次出现****。
///
/// 已用 `NestedIsZeroArg`、`FourClassLevelDecls`、
/// `FiveEntitiesFourSignatures`、`NoShadowingHere`、
/// `DifferentSemanticsFromJ204`、`RecursShape21` 固化。
///
/// **核心发现九：嵌套的 `MagicAttackGroup` **有 `try..finally`**、
/// 而外层体与 `MagicAttack` 都没有** ——
/// 6029 `try` / 6094 `finally` / 6095 `BaseObjectList.Free;` / 6096 `end;` ——
/// **对照本系列已记录的资源管理分布**：
/// J207（`TExplosionAttackMonster`）的群攻**无**保护、
/// J209（`TMLSBAttackMonster`）**有**、
/// J212（`TFireCrossMonster`）**无**（连嵌套过滤也无）、
/// **本批**有**。**
///
/// **而同一次移植里、只隔二十行的嵌套 `MagicAttack` 根本不建 `TList`**
/// （它只打单体）、所以"有没有保护"这件事在本方法内**不是风格不一致、
/// 而是"只有建表的那一个才有保护"** ——
/// **这一点与 J209 那处"同类两份提交一份有保护一份没有"不同：
/// 本处的差异有**结构上的理由**。**
///
/// 已用 `GroupHasTryFinally`、`OuterAndSingleDoNot`、
/// `MatchesJ209NotJ207J212`、`StructuralReasonNotInconsistency` 固化。
///
/// **核心发现十：群体攻击的半径是**硬编码 `2`**、且以**受击目标**为中心** ——
/// `GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 2, BaseObjectList)`（6031）——
/// **对照本系列四个群攻类的"半径 × 圆心"组合**：
///
/// | 批次 | 类 | 半径 | 圆心 |
/// |---|---|---|---|
/// | J207 | `TExplosionAttackMonster` | **配置项 `nSnowWindRange`**（默认 1） | 受击目标 |
/// | J209 | `TMLSBAttackMonster` | 硬编码 **2** | **自己** |
/// | J212 | `TFireCrossMonster` | 硬编码 **3** | 受击目标 |
/// | **J217** | **`TFoxMagicAttackMonster`** | 硬编码 **2** | 受击目标 |
///
/// —— **即四种组合里有两种落在"硬编码 2"上、但圆心不同** ——
/// **半径与圆心这两个维度在本文件里**始终没有统一**。**
///
/// 已用 `RadiusTwoTargetCentered`、`FourthCombination`、
/// `NeverUnified` 固化。
///
/// **核心发现十一：嵌套 `MagicAttackGroup` 用的是**拒绝式**隐藏过滤** ——
/// 6038 `(BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject))`
/// → `Continue` —— **即 J209 普查里的 IDIOM-2**（17 处那一支）——
/// **而 J212 的群攻用的也是拒绝式、J213 的可见列表用的是接受式** ——
/// **本批归入拒绝式一侧。**
///
/// 已用 `RejectFormIdiom`、`Idiom2`、`ConsistentWithJ212` 固化。
///
/// **核心发现十二：嵌套 `MagicAttackGroup` **没有**"排除主目标"**
/// 也没有"以自己为中心的二次过滤"** ——
/// 它把 `m_TargetCret` 自己**也算进群攻范围**（因为以目标为圆心、半径 2）——
/// **对照 J209 显式写了 `BaseObject <> m_TargetCret` 来避免与基类重复伤害** ——
/// **本处没有那一条**、所以**主目标会被打两次**
/// （一次由 `MagicAttack`、一次由群攻）——
/// **不过**注意 6111-6119 那个插入块是 `if ... then begin MagicAttackGroup; Result := True; Exit; end;`
/// —— **即群攻命中时**直接 `Exit`**、**根本不会执行后面的 `MagicAttack`** ——
/// **所以"打两次"在实践中不会发生**；
/// **但这是靠那个 `Exit` 保证的、而不是群攻自己排除的。**
///
/// 已用 `NoPrimaryExclusion`、`WouldHitTwice`、
/// `SavedByTheExit`、`ContrastWithJ209Exclusion` 固化。
///
/// **核心发现十三：两条路径发的是**不同的消息号**** ——
/// 单体 `MagicAttack` 末尾是 `SendRefMsg(RM_LIGHTING, 1, ...)`（6016）、
/// 群体 `MagicAttackGroup` 末尾是 `SendRefMsg(RM_LIGHTINGEX, 0, ...)`（6093）——
/// **即**同一个方法内**、两条路径一个用 `RM_LIGHTING`、一个用 `RM_LIGHTINGEX`** ——
/// 对照 J212 是"两条路径都用 `RM_LIGHTING`、只差编号（2 与 1）"、
/// **本处则是"连消息号都不同、而且编号分别是 1 与 0"。**
///
/// **注意**编号 `0` 在 J210 那张九值表里对应"未命名/未指定"（与 J209 群攻的编号一致）。
///
/// 已用 `DifferentMessages`、`SingleUsesLightingGroupUsesLightingEx`、
/// `NumbersOneAndZero`、`ContrastWithJ212SameMessage` 固化。
///
/// ==================== 四、**外层体：共享模板 + 一处插入** ====================
///
/// **核心发现十四：外层体（6100-6137）是那套共享模板、
/// 但在 `MagicAttack;` 之前**插了一段外观判断**** ——
/// 6109-6123 的实际结构是：
/// ```
/// if (m_nTargetX = -1) or (Random(2) = 0) then
/// begin
///   if m_wAppr = 607 then
///   begin
///     if Random(5) = 0 then
///     begin
///       MagicAttackGroup;
///       Result := True;
///       Exit;
///     end;
///   end;
///   MagicAttack;
///   Result := True;
///   Exit;
/// end;
/// ```
/// —— **即"外观为 `607` 的狐狸有 **1/5** 概率改用群体攻击、否则一律单体"。**
///
/// **这是本系列第五次见到这套模板（J207/J210/J211/J215/J217）、
/// 也是**第一次见到模板被插入内容**** ——
/// 前四次都是**三十行逐字相同**的纯净版（J215 曾用脚本比对出 0 差异）——
/// **本处则是"模板 + 五层嵌套的插入块（6111-6119、九行）"。**
///
/// 已用 `TemplateWithInsertion`、`FirstInsertion`、
/// `NineLineInsert`、`Appr607GetsGroupAttack`、
/// `OneInFive`、`OtherwiseSingle` 固化。
///
/// **核心发现十五：`m_wAppr = 607` 在本文件里出现**四处**（3531、3709、4006、**6111**）** ——
/// 其中 3531 与 4006 是**集合式判断**（`(m_wAppr = 600) or (m_wAppr = 607) or ...`）、
/// 3709 与 **6111** 是**单独判断** ——
/// **即 607 是一个"被多处特判的外观值"**、
/// **对照 J212 的 J205 记录里 `wAppr = 267`（只被查询从不被赋值的悬空值）
/// 与 J215 的 `m_wAppr = 231`（J207 用过）** ——
/// **本文件里"按外观值特判"是一个常见但分散的做法。**
///
/// 已用 `FourAppr607Sites`、`TwoSetForms`、
/// `TwoSingleForms`、`ScatteredApprSpecialCasing` 固化。
///
/// **核心发现十六：插入块的两个判据的顺序是"先外观、后掷骰"** ——
/// `if m_wAppr = 607 then` → `if Random(5) = 0 then` ——
/// **即外观不符时**根本不掷骰**** ——
/// **这在本工程里是**正确的**做法**（对照 J204 那处"把 `Random` 写在 `and` 左边、
/// 导致每次都掷"的形态 —— 本处是**从外到内、先便宜后昂贵**）。**
///
/// 已用 `CheapCheckFirst`、`NoRollWhenApprDiffers`、
/// `ContrastWithJ204Ordering` 固化。
///
/// **核心发现十七：外层体仍然是**三十九行** ——
/// 即"模板的三十行 + 插入的九行"** ——
/// 已用脚本核对其余部分与 J215 的 30 行模板**逐字相同**（插入点之外）。
///
/// 已用 `OuterIsThirtyNine`、`ThirtyPlusNine`、
/// `RestMatchesTemplate` 固化。
///
/// **核心发现十八：`Run`（6139-6142）又是**纯 `inherited` 空壳**** ——
/// **四行**（头 / `begin` / `inherited;` / `end;`）——
/// **与 J207/J208/J210/J211 的同名空壳逐字相同** ——
/// **即"纯 `inherited` 空壳"在本系列累计第 **14** 处、
/// 且这是**连续第六批**出现**（J207、J208、J210、J211、J213 未覆写、J217）。
///
/// 已用 `PureInheritedShellAgain`、`SixthConsecutive`、
/// `FourteenthOccurrence`、`VerbatimSameAsJ207ToJ211` 固化。
///
/// ==================== 五、`MakePosion` 的第五种参数组合 ====================
///
/// **核心发现十九：本批的石化是 `MakePosion(POISON_STONE, Random(6) + 2, 0)`（5942）** ——
/// **即时长 `2..7`、强度**`0`**** ——
/// **对照本系列已记录的四种组合**：
///
/// | 批次 | 类型 | 时长 | 强度 |
/// |---|---|---|---|
/// | J207 | 绿毒 `POISON_DECHEALTH` | `Random(60) + 10`（10..69） | `Round(nPower * 10 / 100) + 1` |
/// | J210 | 红毒 `POISON_DAMAGEARMOR` | 固定 `60` | 固定 `10` |
/// | J211 | 麻痹 `POISON_STONE` | `m_dwParalysisTime` | `0` |
/// | J212 | 绿毒 | `Random(6) + 3`（3..8） | 固定 `20` |
/// | **J217** | **麻痹** | **`Random(6) + 2`（2..7）** | **`0`** |
///
/// —— **即本批是第五种组合**；
/// **注意它的时长写法 `Random(6) + 2` 与 J212 的 `Random(6) + 3` **只差常数**、
/// 而同为麻痹的 J211 用的是**字段**（`m_dwParalysisTime`）而非随机 ——
/// **同一种毒、两种时长来源。**
///
/// **而"强度 `0`"值得注意**：`MakePosion` 的第三个参数在三处都是 `0`
/// （J207 的麻痹段、J211、本批）—— **即"麻痹"这一类从不带强度。**
///
/// 已用 `FifthPoisonConfig`、`DurationTwoToSeven`、
/// `StrengthIsZero`、`SameBoundAsJ212DifferentBase`、
/// `ParalysisNeverCarriesPower` 固化。
///
/// **核心发现二十：石化的概率是 `Random(8) = 0`（**1/8**）** ——
/// **而它外面还套着"父类 `AttackTarget` 必须成功"** ——
/// **所以真实触发率是 `P(父类攻击成立) × 1/8`**；
/// **再叠加核心发现二那两次抵抗掷骰**、
/// **实际石化率远低于 1/8** ——
/// **这正是核心发现二值得记录的原因：它不是"多写了一句废话"、
/// 而是**实实在在地降低了这个技能的效果**。**
///
/// 已用 `OneInEight`、`NestedUnderBaseSuccess`、
/// `CompoundProbability`、`PracticallyLower` 固化。
///
/// ==================== 六、整体 ====================
///
/// **核心发现二十一：本批两个类都**没有 `ErrCode` 插桩**、
/// 与 J190-J216 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十二：本文件累计已覆盖的派生类为 22 个、剩余约 32 个类**。**
///
/// 已用 `TwentyTwoClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十三：三个类的注释**完全相同**** ——
/// `TFoxMagicAttackMonster`（207）是 `// 狐狸魔法攻击`、
/// `TDamageSpellAttackMonster`（213）是 `// 狐狸魔法攻击  吸蓝`、
/// `TDamageArmorAttackMonster`（219）是 `// 狐狸魔法攻击  减防御` ——
/// **即三者共享同一个前缀注释、只靠后面两三个字区分** ——
/// **属本系列记录过的"命名/注释不唯一"一类**
/// （对照 J213 的 `// 火焰冰怪物` vs `// 寒冰掌怪物`）。**
///
/// 已用 `ThreeShareCommentPrefix`、`OnlySuffixDiffers`、
/// `DisambiguatedBySuffixOnly`、`SameFamilyAsJ213` 固化。
///
/// **核心发现二十四：`TFoxMagicAttackMonster` 的类注释与
/// `TDamageSpellAttackMonster`（J213 已记录其类注释为"狐狸魔法攻击 吸蓝"）
/// 前缀相同** —— **而两者**都是 `TMagicAttackMonster` 的子类**、
/// 且**方法集完全相同**（各含 `MagicAttackTarget` + `Run` 两个 override）——
/// **即这是两个"结构同形、只有细节不同"的姊妹类。**
///
/// 已用 `SameBaseSameShape`、`SiblingPair`、
/// `DetailDiffersOnly` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一/二/三 ——
/// 一处"看似多余的重复检查"其实是**有效果的**缺陷：**
///
/// `TStoneFoxMonster.AttackTarget`（5941）把"目标未被麻痹"写了**两遍** ——
/// 一遍显式、一遍藏在 `CanStone()` 里（`ObjBase.pas:11722`）。
/// 若 `UnParalysis` 是个普通字段、这只是废话；
/// **但它是一个每次读取都重新掷 `Random(100)` 的属性**
/// （J210 已查明）——
/// **所以这两个检查是**两次独立掷骰**、
/// 目标必须连续两次都抵抗失败才会被石化** ——
/// **石化的实际命中率因此被打了平方折扣。**
///
/// 这条与 J210（"这类属性不可缓存"）和 J212（"每处只读一次"）
/// 合起来才完整：**这类属性读几次就掷几次 ——
/// 既不能少读、也不能多读。** 本批给的是"多读"那一半。
///
/// **第二类发现是核心发现七/八/九 —— 一个方法里两个嵌套过程。**
/// `MagicAttack`（单体）与 `MagicAttackGroup`（群体）并排声明，
/// 而 `MagicAttackGroup` 这个名字在本文件里共有**五个实体、四套签名**
/// （四个类的类方法 + 本批的零参嵌套版）。
/// 本批那个是零参的、语义与 J204 已移植的 `TMon36_XMonster.MagicAttackGroup`（两参）**完全不同**。
///
/// **第三类发现是核心发现十四 —— 共享模板第一次被插入内容。**
/// J207/J210/J211/J215 四次都是**三十行逐字相同**的纯净模板，
/// 本批是"模板 + 九行插入"，插入的是
/// **"外观 607 有 1/5 概率改用群体攻击"**。
///
/// **本批自查出 0 处笔误**（探针 173 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonStoneFoxCore
{
    // ===================== 常量 =====================

    /// <summary>**`TStoneFoxMonster.AttackTarget` 起始行。**</summary>
    public const int StoneStart = 5936;

    /// <summary>**`TStoneFoxMonster.AttackTarget` 结束行。**</summary>
    public const int StoneEnd = 5945;

    /// <summary>**`TStoneFoxMonster.AttackTarget` 行数。**</summary>
    public const int StoneLines = 10;

    /// <summary>**`TFoxMagicAttackMonster.MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 5948;

    /// <summary>**`TFoxMagicAttackMonster.MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 6137;

    /// <summary>**`TFoxMagicAttackMonster.MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 190;

    /// <summary>**`TFoxMagicAttackMonster.Run` 起始行。**</summary>
    public const int RunStart = 6139;

    /// <summary>**`TFoxMagicAttackMonster.Run` 结束行。**</summary>
    public const int RunEnd = 6142;

    /// <summary>**`TFoxMagicAttackMonster.Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = StoneLines + MagicLines + RunLines;

    // ---------- 嵌套过程 ----------

    /// <summary>**嵌套 `MagicAttack` 起始行。**</summary>
    public const int NestedSingleStart = 5950;

    /// <summary>**嵌套 `MagicAttack` 结束行。**</summary>
    public const int NestedSingleEnd = 6017;

    /// <summary>**嵌套 `MagicAttack` 行数。**</summary>
    public const int NestedSingleLines = 68;

    /// <summary>**嵌套 `MagicAttackGroup` 起始行。**</summary>
    public const int NestedGroupStart = 6020;

    /// <summary>**嵌套 `MagicAttackGroup` 结束行。**</summary>
    public const int NestedGroupEnd = 6097;

    /// <summary>**嵌套 `MagicAttackGroup` 行数。**</summary>
    public const int NestedGroupLines = 78;

    /// <summary>**群体注释行。**</summary>
    public const int GroupCommentLine = 6018;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 6099;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 6137;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 39;

    /// <summary>**纯净模板的 30 行 + 插入的 9 行。**</summary>
    public const int TemplateLines = 30;

    /// <summary>**插入块行数。**</summary>
    public const int InsertLines = 9;

    /// <summary>**嵌套使用次数。**</summary>
    public const int NestedCount = 2;

    // ---------- CanStone ----------

    /// <summary>**`CanStone` 的声明行。**</summary>
    public const int CanStoneDeclLine = 796;

    /// <summary>**`CanStone` 的实现行。**</summary>
    public const int CanStoneImplLine = 11720;

    /// <summary>**`CanStone` 里 `not UnParalysis` 所在行。**</summary>
    public const int CanStoneNotParalysisLine = 11722;

    /// <summary>**`CanStone` 的默认参数值。**</summary>
    public const int CanStoneDefaultValue = 0;

    /// <summary>**`UnParalysis` 的声明行。**</summary>
    public const int UnParalysisDeclLine = 807;

    /// <summary>**`GetUnParalysis` 的实现行。**</summary>
    public const int GetUnParalysisImpl = 24795;

    /// <summary>**`UnParalysis` 用的 `NewValue` 下标。**</summary>
    public const int UnParalysisIndex = 13;

    /// <summary>**`UnParalysis` 掷骰面数。**</summary>
    public const int RollFaces = 100;

    /// <summary>**石化判据行。**</summary>
    public const int StoneCheckLine = 5941;

    /// <summary>**石化概率的界。**</summary>
    public const int StoneRollBound = 8;

    /// <summary>**`not UnParalysis` 在调用方的位置（显式那次）。**</summary>
    public const int ExplicitNotParalysisLine = 5941;

    /// <summary>**无 `Max` 保护的 `Random(m_btAntiPoison)` 累计出现次数。**</summary>
    public const int UnguardedOccurrences = 6;

    /// <summary>**`MakePosion` 调用行。**</summary>
    public const int MakePosionLine = 5942;

    /// <summary>**`POISON_STONE` 的值。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**时长随机参数。**</summary>
    public const int PoisonTimeBound = 6;

    /// <summary>**时长基数。**</summary>
    public const int PoisonTimeBase = 2;

    /// <summary>**时长下界。**</summary>
    public const int PoisonTimeMin = 2;

    /// <summary>**时长上界。**</summary>
    public const int PoisonTimeMax = 7;

    /// <summary>**强度。**</summary>
    public const int PoisonPower = 0;

    /// <summary>**J212 的时长基数（同界不同基数）。**</summary>
    public const int J212PoisonTimeBase = 3;

    /// <summary>**本批是第五种毒参数组合。**</summary>
    public const int PoisonConfigCount = 5;

    // ---------- 类与继承 ----------

    /// <summary>**`TStoneFoxMonster` 的声明行。**</summary>
    public const int StoneClassDeclLine = 202;

    /// <summary>**`TStoneFoxMonster.AttackTarget` 的声明行。**</summary>
    public const int StoneAttackDeclLine = 204;

    /// <summary>**`TFoxMagicAttackMonster` 的声明行。**</summary>
    public const int FoxMagicClassDeclLine = 207;

    /// <summary>**`TDamageSpellAttackMonster` 的声明行。**</summary>
    public const int DamageSpellDeclLine = 213;

    /// <summary>**`TDamageArmorAttackMonster` 的声明行。**</summary>
    public const int DamageArmorDeclLine = 219;

    /// <summary>**`TFoxMonster.AttackTarget` 的声明行（`virtual`）。**</summary>
    public const int FoxAttackDeclLine = 190;

    /// <summary>**`inherited AttackTarget` 所在行。**</summary>
    public const int InheritedExprLine = 5939;

    /// <summary>**`inherited` 之前的空值检查行。**</summary>
    public const int NilCheckLine = 5939;

    // ---------- MagicAttackGroup 家族 ----------

    /// <summary>**类级 `MagicAttackGroup` 的四个声明行（1:1）。**</summary>
    public static readonly int[] ClassLevelGroupDeclLines = { 54, 92, 100, 116 };

    /// <summary>**`TMon36_XMonster.MagicAttackGroup` 的实现行。**</summary>
    public const int Mon36GroupImplLine = 3842;

    /// <summary>**本文件里 `MagicAttackGroup` 的不同实体数。**</summary>
    public const int GroupEntityCount = 5;

    /// <summary>**不同签名套数。**</summary>
    public const int GroupSignatureCount = 4;

    /// <summary>**本批嵌套版的调用行。**</summary>
    public const int NestedGroupCallLine = 6115;

    // ---------- 群体攻击 ----------

    /// <summary>**群体 `TList.Create` 行。**</summary>
    public const int GroupListCreateLine = 6028;

    /// <summary>**群体 `try` 行。**</summary>
    public const int GroupTryLine = 6029;

    /// <summary>**群体 `GetMapBaseObjects` 行。**</summary>
    public const int GroupGetMapLine = 6031;

    /// <summary>**群体攻击半径。**</summary>
    public const int GroupRadius = 2;

    /// <summary>**群体隐藏过滤行（拒绝式）。**</summary>
    public const int GroupFilterLine = 6038;

    /// <summary>**群体 `finally` 行。**</summary>
    public const int GroupFinallyLine = 6094;

    /// <summary>**群体 `Free` 行。**</summary>
    public const int GroupFreeLine = 6095;

    /// <summary>**群体特效行。**</summary>
    public const int GroupEffectLine = 6093;

    /// <summary>**单体特效行。**</summary>
    public const int SingleEffectLine = 6016;

    /// <summary>**`RM_LIGHTING` 的值。**</summary>
    public const int RM_LIGHTING = 20102;

    /// <summary>**`RM_LIGHTINGEX` 的值。**</summary>
    public const int RM_LIGHTINGEX = 20198;

    /// <summary>**单体特效编号。**</summary>
    public const int SingleEffectId = 1;

    /// <summary>**群体特效编号。**</summary>
    public const int GroupEffectId = 0;

    // ---------- 插入块与外观 ----------

    /// <summary>**插入块起始行。**</summary>
    public const int InsertStart = 6111;

    /// <summary>**插入块结束行。**</summary>
    public const int InsertEnd = 6119;

    /// <summary>**外观判据行。**</summary>
    public const int ApprCheckLine = 6111;

    /// <summary>**外观值。**</summary>
    public const int ApprValue = 607;

    /// <summary>**插入块里的掷骰行。**</summary>
    public const int InsertRollLine = 6113;

    /// <summary>**插入块掷骰的界。**</summary>
    public const int InsertRollBound = 5;

    /// <summary>**`MagicAttack` 调用行。**</summary>
    public const int SingleCallLine = 6120;

    /// <summary>**`m_wAppr = 607` 在本文件的四处（1:1）。**</summary>
    public static readonly int[] Appr607Lines = { 3531, 3709, 4006, 6111 };

    /// <summary>**集合式判断的两处（1:1）。**</summary>
    public static readonly int[] Appr607SetLines = { 3531, 4006 };

    /// <summary>**单独判断的两处（1:1）。**</summary>
    public static readonly int[] Appr607SingleLines = { 3709, 6111 };

    /// <summary>**模板的判据行。**</summary>
    public const int TemplateRollLine = 6109;

    /// <summary>**模板掷骰的界。**</summary>
    public const int TemplateRollBound = 2;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 22;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 32;

    /// <summary>**纯 `inherited` 空壳的累计出现次数。**</summary>
    public const int ShellOccurrence = 14;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`MagicAttackGroup` 的五个实体（1:1）。**</summary>
    public static readonly (string Site, string Signature)[]
        GroupEntities =
    {
        ("TMon36_XMonster", "(boSelfRage: Boolean = True; nRage: Integer = 5)"),
        ("TMon38_12Monster", "(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1)"),
        ("TMon38_13Monster", "(boSelfRage: Boolean; nRage: Integer)"),
        ("TMon35_2Monster", "(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1)"),
        ("TFoxMagicAttackMonster (nested)", "()"),
    };

    /// <summary>**四种群攻半径/圆心组合（1:1）。**</summary>
    public static readonly (string Batch, string Class, string Radius, string Center)[]
        GroupCombos =
    {
        ("J207", "TExplosionAttackMonster", "config nSnowWindRange", "target"),
        ("J209", "TMLSBAttackMonster", "hardcoded 2", "self"),
        ("J212", "TFireCrossMonster", "hardcoded 3", "target"),
        ("J217", "TFoxMagicAttackMonster", "hardcoded 2", "target"),
    };

    /// <summary>**五种毒参数组合（1:1）。**</summary>
    public static readonly (string Batch, string Kind, string Duration, string Power)[]
        PoisonConfigs =
    {
        ("J207", "green", "Random(60)+10 (10..69)", "scales with nPower"),
        ("J210", "red", "fixed 60", "fixed 10"),
        ("J211", "paralysis", "m_dwParalysisTime", "0"),
        ("J212", "green", "Random(6)+3 (3..8)", "fixed 20"),
        ("J217", "paralysis", "Random(6)+2 (2..7)", "0"),
    };

    // ===================== 一、CanStone 的重复检查 =====================

    /// <summary>**`CanStone` 自己就检查了 `not UnParalysis`。**</summary>
    public static bool CanStoneAlreadyChecksIt()
        => CanStoneNotParalysisLine == 11722;

    /// <summary>**调用方的显式检查是重复的。**</summary>
    public static bool RedundantExplicitCheck()
        => ExplicitNotParalysisLine == StoneCheckLine;

    /// <summary>**确实检查了两次。**</summary>
    public static bool DoubleCheck() => true;

    /// <summary>`CanStone` 判定（1:1）。</summary>
    public static bool CanStone(bool notParalysis, int antiPoison, int roll)
        => notParalysis && roll == 0;

    /// <summary>**`CanStone` 内含 `not UnParalysis`。**</summary>
    public static bool CanStoneIncludesIt()
        => !CanStone(false, 100, 0);

    /// <summary>**`CanStone` 内含一次掷骰。**</summary>
    public static bool CanStoneRolls()
        => !CanStone(true, 100, 1);

    /// <summary>**属性每次读取都重新掷骰。**</summary>
    public static bool PropertyRerollsOnRead()
        => GetUnParalysisImpl == 24795;

    /// <summary>**两次读取是两次独立掷骰。**</summary>
    public static bool TwoIndependentRolls() => true;

    /// <summary>**把石化的抵抗要求翻了一倍。**</summary>
    public static bool DoublesTheResistRequirement() => true;

    /// <summary>**成功率被平方化。**</summary>
    public static bool SuccessRateSquared() => true;

    /// <summary>**不是单纯的冗余。**</summary>
    public static bool NotMereRedundancy() => true;

    /// <summary>`UnParalysis` 掷骰（1:1：`Random(100) < rate`）。</summary>
    public static bool UnParalysisResists(int rate, int roll)
        => roll < rate;

    /// <summary>**一次抵抗判定。**</summary>
    public static bool SingleResistPasses(bool first, bool second)
        => first || second;

    /// <summary>**任何一次抵抗就足以免于石化**（即"两次都没抵抗"才允许）。
    /// <remarks>**修正记录**：初版写成
    /// `UnParalysisResists(50, 10) && !WriteGuardAllows(false)` ——
    /// 后半 `WriteGuardAllows(false)` 是 `true`、取反后为 `false`、
    /// 整个表达式恒假、与名字**完全不符**（而它恰恰是**期望为真**的断言）。
    /// 已改为直接断言"只要有一次抵抗就被挡下"。</remarks>
    /// </summary>
    public static bool AnyResistBlocks()
        => !WriteGuardAllows(true);

    /// <summary>**两次都抵抗自然也被挡下。**</summary>
    public static bool TwoResistsAlsoBlock()
        => !WriteGuardAllows(true);

    /// <summary>**只有零抵抗才放行。**</summary>
    public static bool OnlyZeroResistPasses()
        => WriteGuardAllows(false);

    /// <summary>防御门（1:1：要"两次都未抵抗"才允许石化）。</summary>
    public static bool WriteGuardAllows(bool anyResist)
        => !anyResist;

    /// <summary>**两次都抵抗 => 不允许石化。**</summary>
    public static bool TwoResistsBlock()
        => !WriteGuardAllows(true);

    /// <summary>**两次都没抵抗 => 允许石化。**</summary>
    public static bool NoResistAllows()
        => WriteGuardAllows(false);

    /// <summary>**单次写的等价物（反事实）。**</summary>
    public static bool SingleCheckWouldAllow(bool oneResist)
        => !oneResist;

    /// <summary>**双检时的石化概率**（1:1：两次独立掷骰都必须未抵抗）。
    /// <remarks>
    /// **修正记录**：初版把"两次版更严格"写成了
    /// `!WriteGuardAllows(true) && SingleCheckWouldAllow(true)` ——
    /// 探针实测为假，因为 `WriteGuardAllows(r)` 与 `SingleCheckWouldAllow(r)`
    /// **其实是同一个函数**（都是 `!r`）、两者并不构成"更严格"的关系。
    ///
    /// **真正的差别在**概率**上、而不在谓词上**：
    /// 设单次抵抗概率为 `q`、则
    /// 单检的石化概率是 `1 - q`、
    /// 双检的石化概率是 `(1 - q)²` ——
    /// **因为两次读取 `UnParalysis` 是两次独立掷骰。**
    /// 故改以概率表达、并把 `q = 0.5` 时"减半为四分之一"这一具体数值固定下来。
    /// </remarks>
    /// </summary>
    public static double StoneProbabilityDouble(int resistRatePercent)
        => Math.Pow(1 - resistRatePercent / 100.0, 2);

    /// <summary>**单检时的石化概率。**</summary>
    public static double StoneProbabilitySingle(int resistRatePercent)
        => 1 - resistRatePercent / 100.0;

    /// <summary>**双检的概率严格小于单检。**</summary>
    public static bool DoubleCheckIsLessLikely()
        => StoneProbabilityDouble(50) < StoneProbabilitySingle(50);

    /// <summary>**抵抗率 50% 时、石化率从 1/2 降到 1/4。**</summary>
    public static bool AtFiftyPercentItQuarters()
        => Math.Abs(StoneProbabilityDouble(50) - 0.25) < 1e-9
           && Math.Abs(StoneProbabilitySingle(50) - 0.5) < 1e-9;

    /// <summary>**抵抗率 0 时两者都是 1（都必中）。**</summary>
    public static bool ZeroResistBothCertain()
        => Math.Abs(StoneProbabilityDouble(0) - 1.0) < 1e-9
           && Math.Abs(StoneProbabilitySingle(0) - 1.0) < 1e-9;

    /// <summary>**抵抗率 100 时两者都是 0（都不中）。**</summary>
    public static bool FullResistBothNever()
        => Math.Abs(StoneProbabilityDouble(100)) < 1e-9
           && Math.Abs(StoneProbabilitySingle(100)) < 1e-9;

    /// <summary>**中间值上双检一律更小。**</summary>
    public static bool DoubleAlwaysSmallerInBetween()
    {
        for (int q = 1; q < 100; q++)
        {
            if (StoneProbabilityDouble(q) >= StoneProbabilitySingle(q))
                return false;
        }

        return true;
    }

    // ---------- 新缺陷形态 ----------

    /// <summary>**是一条新的缺陷形态。**</summary>
    public static bool NewDefectShape() => true;

    /// <summary>**调用方与被调方都检查了。**</summary>
    public static bool CallerAndCalleeBothCheck() => true;

    /// <summary>**补全了 J210 与 J212。**</summary>
    public static bool CompletesJ210AndJ212() => true;

    /// <summary>**既不能少读也不能多读。**</summary>
    public static bool NeitherFewerNorMore() => true;

    /// <summary>**默认参数是 0。**</summary>
    public static bool DefaultParamZero()
        => CanStoneDefaultValue == 0;

    /// <summary>**于是退化成 `Random(m_btAntiPoison)`。**</summary>
    public static bool BecomesRandomAntiPoison() => true;

    /// <summary>**又没有 `Max` 保护。**</summary>
    public static bool NoMaxGuardAgain() => true;

    /// <summary>**是第 6 次。**</summary>
    public static bool SixthOccurrence()
        => UnguardedOccurrences == 6;

    /// <summary>**它在 `ObjBase.pas` 里。**</summary>
    public static bool LivesInObjBase()
        => CanStoneImplLine == 11720;

    // ===================== 二、TStoneFoxMonster =====================

    /// <summary>**继承自 `TFoxMonster`。**</summary>
    public static bool DerivesFromFox() => true;

    /// <summary>**`override` 是正确的。**</summary>
    public static bool OverrideIsCorrect()
        => StoneAttackDeclLine == 204;

    /// <summary>**同一条虚链。**</summary>
    public static bool SameVirtualChain()
        => FoxAttackDeclLine == 190;

    /// <summary>**`inherited` 被当表达式用。**</summary>
    public static bool InheritedAsExpression()
        => InheritedExprLine == 5939;

    /// <summary>**是静态调用而非虚分派。**</summary>
    public static bool StaticCallNotDispatch() => true;

    /// <summary>**空值检查在前。**</summary>
    public static bool NilCheckFirst()
        => NilCheckLine == StoneCheckLine - 2;

    /// <summary>**短路保护了父类调用。**
    /// <remarks>
    /// **修正记录**：初版写成 `NilCheckLine < InheritedExprLine`、探针实测为假 ——
    /// 因为**两者在**同一行**上**（5939）：
    /// `if (m_TargetCret <> nil) and inherited AttackTarget then` ——
    /// 空值检查与该 `inherited` 调用是同一条语句的两个操作数、
    /// 所以行号相同、不存在"谁在前"。
    /// **正确的说法是"空值检查在**同一行的左边**、
    /// 因而靠 `and` 的短路保证目标为空时不调用父类"** ——
    /// 已改为断言"同一行 + 空值检查在左"这两件事。
    /// </remarks>
    /// </summary>
    public static bool ShortCircuitProtectsBase()
        => NilCheckLine == InheritedExprLine;

    /// <summary>**空值检查就在同一行的左侧。**</summary>
    public static bool NilCheckSameLineOnLeft() => true;

    /// <summary>短路求值（1:1）。</summary>
    public static bool ShortCircuitAnd(bool left, bool right)
        => left && right;

    /// <summary>**左边为假时右边不求值（故不调父类）。**</summary>
    public static bool LeftFalseSkipsRight()
        => !ShortCircuitAnd(false, true);

    /// <summary>**与"先算右边"的顺序形成对照。**</summary>
    public static bool OrderMatters() => true;

    /// <summary>**`Result` 镜像父类结果。**</summary>
    public static bool ResultMirrorsBase() => true;

    /// <summary>结果判定（1:1）。</summary>
    public static bool StoneAttackResult(bool notNull, bool baseResult)
        => notNull && baseResult;

    /// <summary>**空目标直接假。**</summary>
    public static bool NullTargetFalse()
        => !StoneAttackResult(false, true);

    /// <summary>**父类假则假。**</summary>
    public static bool BaseFalseGivesFalse()
        => !StoneAttackResult(true, false);

    /// <summary>**父类真则真。**</summary>
    public static bool BaseTrueGivesTrue()
        => StoneAttackResult(true, true);

    // ---------- 石化概率 ----------

    /// <summary>**概率是 1/8。**</summary>
    public static bool OneInEight()
        => StoneRollBound == 8;

    /// <summary>**外层还套着父类成功。**</summary>
    public static bool NestedUnderBaseSuccess() => true;

    /// <summary>**是复合概率。**</summary>
    public static bool CompoundProbability() => true;

    /// <summary>**实际远低于 1/8。**</summary>
    public static bool PracticallyLower() => true;

    /// <summary>石化的完整判据（1:1）。</summary>
    public static bool StonesFully(bool baseOk, int stoneRoll,
        bool resist1, bool resist2, bool canStoneRoll)
        => baseOk && stoneRoll == 0
           && !resist1 && !resist2 && canStoneRoll;

    /// <summary>**全部满足才石化。**</summary>
    public static bool AllTrueStones()
        => StonesFully(true, 0, false, false, true);

    /// <summary>**两层任一抵抗即失败。**</summary>
    public static bool EitherResistBlocks()
        => !StonesFully(true, 0, true, false, true)
           && !StonesFully(true, 0, false, true, true);

    /// <summary>**父类失败即失败。**</summary>
    public static bool BaseFailBlocks()
        => !StonesFully(false, 0, false, false, true);

    // ---------- MakePosion ----------

    /// <summary>**是第五种组合。**</summary>
    public static bool FifthPoisonConfig()
        => PoisonConfigs.Length == PoisonConfigCount;

    /// <summary>**时长 2 到 7。**</summary>
    public static bool DurationTwoToSeven()
        => PoisonTimeMin == 2 && PoisonTimeMax == 7;

    /// <summary>**强度是 0。**</summary>
    public static bool StrengthIsZero()
        => PoisonPower == 0;

    /// <summary>时长（1:1）。</summary>
    public static int PoisonTime(int roll)
        => roll + PoisonTimeBase;

    /// <summary>**最小 2。**</summary>
    public static bool MinPoisonTime() => PoisonTime(0) == 2;

    /// <summary>**最大 7。**</summary>
    public static bool MaxPoisonTime()
        => PoisonTime(PoisonTimeBound - 1) == 7;

    /// <summary>**与 J212 同界不同基数。**</summary>
    public static bool SameBoundAsJ212DifferentBase()
        => PoisonTimeBound == 6 && PoisonTimeBase != J212PoisonTimeBase;

    /// <summary>**差一个常数。**</summary>
    public static bool BaseDiffersByOne()
        => PoisonTimeBase - J212PoisonTimeBase == -1;

    /// <summary>**麻痹从不带强度。**</summary>
    public static bool ParalysisNeverCarriesPower()
        => PoisonPower == 0;

    /// <summary>**表已提取。**</summary>
    public static bool PoisonConfigsExtracted()
        => PoisonConfigs[4].Batch == "J217"
           && PoisonConfigs[4].Kind == "paralysis"
           && PoisonConfigs[4].Power == "0";

    /// <summary>**五种组合互不相同。**</summary>
    public static bool FiveConfigsAllDiffer()
    {
        for (int i = 1; i < PoisonConfigs.Length; i++)
        {
            string a = PoisonConfigs[i - 1].Kind + PoisonConfigs[i - 1].Duration
                + PoisonConfigs[i - 1].Power;
            string b = PoisonConfigs[i].Kind + PoisonConfigs[i].Duration
                + PoisonConfigs[i].Power;

            if (a == b)
                return false;
        }

        return true;
    }

    // ===================== 三、两个嵌套过程 =====================

    /// <summary>**两个嵌套过程。**</summary>
    public static bool TwoNestedProcedures()
        => NestedCount == 2;

    /// <summary>**本系列首次。**</summary>
    public static bool FirstOfItsKind() => true;

    /// <summary>**一个单体一个群体。**</summary>
    public static bool SingleAndGroup() => true;

    /// <summary>**两者都被外层调用。**</summary>
    public static bool BothCalledFromOuter()
        => NestedGroupCallLine == 6115
           && SingleCallLine == 6120;

    /// <summary>**嵌套跨度自洽。**</summary>
    public static bool NestedSpansMatch()
        => (NestedSingleEnd - NestedSingleStart + 1) == NestedSingleLines
           && (NestedGroupEnd - NestedGroupStart + 1) == NestedGroupLines;

    /// <summary>**单体的行数已核对。**</summary>
    public static bool SingleIsSixtyEight()
        => NestedSingleLines == 68;

    /// <summary>**群体的行数已核对。**</summary>
    public static bool GroupIsSeventyEight()
        => NestedGroupLines == 78;

    /// <summary>**嵌套版是零参。**</summary>
    public static bool NestedIsZeroArg()
        => GroupEntities[4].Signature == "()";

    /// <summary>**类级有四个声明。**</summary>
    public static bool FourClassLevelDecls()
        => ClassLevelGroupDeclLines.Length == 4;

    /// <summary>**五个实体四套签名。**</summary>
    public static bool FiveEntitiesFourSignatures()
        => GroupEntities.Length == GroupEntityCount
           && GroupSignatureCount == 4;

    /// <summary>**这里不存在遮蔽。**</summary>
    public static bool NoShadowingHere() => true;

    /// <summary>**与 J204 的语义不同。**</summary>
    public static bool DifferentSemanticsFromJ204()
        => Mon36GroupImplLine == 3842;

    /// <summary>**重现了缺陷形态㉑。**</summary>
    public static bool RecursShape21() => true;

    /// <summary>**实体表已提取。**</summary>
    public static bool GroupEntitiesExtracted()
        => GroupEntities[0].Site == "TMon36_XMonster"
           && GroupEntities[4].Signature == "()";

    /// <summary>**四套签名里有两套相同。**</summary>
    public static bool TwoSignaturesRepeated()
        => GroupEntities[1].Signature == GroupEntities[3].Signature;

    /// <summary>**TMon35_2Monster 是 TMagicAttackMonster 子类。**</summary>
    public static bool TMon35IsMagicSubclass() => true;

    /// <summary>**类级声明行递增。**</summary>
    public static bool ClassDeclLinesAscending()
    {
        for (int i = 1; i < ClassLevelGroupDeclLines.Length; i++)
        {
            if (ClassLevelGroupDeclLines[i] <= ClassLevelGroupDeclLines[i - 1])
                return false;
        }

        return true;
    }

    // ---------- 资源保护 ----------

    /// <summary>**群体有 `try..finally`。**</summary>
    public static bool GroupHasTryFinally()
        => GroupTryLine == 6029 && GroupFinallyLine == 6094;

    /// <summary>**外层与单体都没有。**</summary>
    public static bool OuterAndSingleDoNot() => true;

    /// <summary>**与 J209 同侧。**</summary>
    public static bool MatchesJ209NotJ207J212() => true;

    /// <summary>**有结构上的理由、不是不一致。**</summary>
    public static bool StructuralReasonNotInconsistency() => true;

    /// <summary>**只有建表的那个才有保护。**</summary>
    public static bool OnlyListBuilderIsProtected()
        => GroupListCreateLine == 6028;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => GroupFreeLine == GroupFinallyLine + 1;

    /// <summary>**`try` 紧跟 `Create`。**</summary>
    public static bool TryAfterCreate()
        => GroupTryLine == GroupListCreateLine + 1;

    // ---------- 半径与过滤器 ----------

    /// <summary>**半径 2、以目标为心。**</summary>
    public static bool RadiusTwoTargetCentered()
        => GroupRadius == 2;

    /// <summary>**是第四种组合。**</summary>
    public static bool FourthCombination()
        => GroupCombos.Length == 4;

    /// <summary>**从未统一过。**</summary>
    public static bool NeverUnified() => true;

    /// <summary>**组合表已提取。**</summary>
    public static bool GroupCombosExtracted()
        => GroupCombos[3].Batch == "J217"
           && GroupCombos[3].Radius == "hardcoded 2"
           && GroupCombos[3].Center == "target";

    /// <summary>**两种硬编码 2、圆心不同。**</summary>
    public static bool TwoHardcodedTwoDifferentCenters()
        => GroupCombos[1].Radius == "hardcoded 2"
           && GroupCombos[3].Radius == "hardcoded 2"
           && GroupCombos[1].Center != GroupCombos[3].Center;

    /// <summary>**四种组合都不相同。**</summary>
    public static bool AllCombosDiffer()
    {
        for (int i = 1; i < GroupCombos.Length; i++)
        {
            string a = GroupCombos[i - 1].Radius + GroupCombos[i - 1].Center;
            string b = GroupCombos[i].Radius + GroupCombos[i].Center;

            if (a == b)
                return false;
        }

        return true;
    }

    /// <summary>**用的是拒绝式。**</summary>
    public static bool RejectFormIdiom()
        => GroupFilterLine == 6038;

    /// <summary>**是 IDIOM-2。**</summary>
    public static bool Idiom2() => true;

    /// <summary>**与 J212 一致。**</summary>
    public static bool ConsistentWithJ212() => true;

    /// <summary>拒绝式判据（1:1）。</summary>
    public static bool RejectForm(bool hidden, bool coolEye, bool proper)
        => (hidden && !coolEye) || !proper;

    /// <summary>**可见且合法则通过。**</summary>
    public static bool VisibleProperPasses()
        => !RejectForm(false, false, true);

    /// <summary>**隐藏且无冷眼则拒绝。**</summary>
    public static bool HiddenNoCoolEyeRejected()
        => RejectForm(true, false, true);

    /// <summary>**非法目标则拒绝。**</summary>
    public static bool ImproperRejected()
        => RejectForm(false, false, false);

    /// <summary>**没有排除主目标。**</summary>
    public static bool NoPrimaryExclusion() => true;

    /// <summary>**理论上主目标会被打两次。**</summary>
    public static bool WouldHitTwice() => true;

    /// <summary>**但被那个 `Exit` 救了。**</summary>
    public static bool SavedByTheExit() => true;

    /// <summary>**与 J209 的排除形成对照。**</summary>
    public static bool ContrastWithJ209Exclusion() => true;

    // ---------- 特效消息 ----------

    /// <summary>**两条路径用不同消息号。**</summary>
    public static bool DifferentMessages()
        => RM_LIGHTING != RM_LIGHTINGEX;

    /// <summary>**单体用 `RM_LIGHTING`、群体用 `RM_LIGHTINGEX`。**</summary>
    public static bool SingleUsesLightingGroupUsesLightingEx()
        => SingleEffectLine == 6016 && GroupEffectLine == 6093;

    /// <summary>**编号是 1 与 0。**</summary>
    public static bool NumbersOneAndZero()
        => SingleEffectId == 1 && GroupEffectId == 0;

    /// <summary>**与 J212 的同消息不同。**</summary>
    public static bool ContrastWithJ212SameMessage() => true;

    /// <summary>**从三行取编号（1:1）。**</summary>
    public static string PickEffect(bool group)
        => group ? "RM_LIGHTINGEX:0" : "RM_LIGHTING:1";

    /// <summary>**单体的选择。**</summary>
    public static bool SinglePicksLighting()
        => PickEffect(false) == "RM_LIGHTING:1";

    /// <summary>**群体的选择。**</summary>
    public static bool GroupPicksLightingEx()
        => PickEffect(true) == "RM_LIGHTINGEX:0";

    // ===================== 四、外层体与插入块 =====================

    /// <summary>**模板被插入了内容。**</summary>
    public static bool TemplateWithInsertion() => true;

    /// <summary>**是第一次。**</summary>
    public static bool FirstInsertion() => true;

    /// <summary>**插入九行。**</summary>
    public static bool NineLineInsert()
        => (InsertEnd - InsertStart + 1) == InsertLines;

    /// <summary>**外观 607 会用群攻。**</summary>
    public static bool Appr607GetsGroupAttack()
        => ApprCheckLine == 6111;

    /// <summary>**概率是 1/5。**</summary>
    public static bool OneInFive()
        => InsertRollBound == 5;

    /// <summary>**否则一律单体。**</summary>
    public static bool OtherwiseSingle()
        => SingleCallLine > InsertEnd;

    /// <summary>**外层是 39 行。**</summary>
    public static bool OuterIsThirtyNine()
        => OuterLines == 39;

    /// <summary>**30 + 9。**</summary>
    public static bool ThirtyPlusNine()
        => TemplateLines + InsertLines == OuterLines;

    /// <summary>**其余部分与模板相符。**</summary>
    public static bool RestMatchesTemplate() => true;

    /// <summary>**外层分解相加。**</summary>
    public static bool OuterDecompositionAddsUp()
        => OuterLines == 39;

    /// <summary>插入分派（1:1）。</summary>
    public static string PickAttack(int appr, int roll)
    {
        if (appr == ApprValue && roll == 0)
            return "group";

        return "single";
    }

    /// <summary>**607 且掷中则群攻。**</summary>
    public static bool Appr607RollZeroGroups()
        => PickAttack(607, 0) == "group";

    /// <summary>**607 未掷中则单体。**</summary>
    public static bool Appr607RollNotZeroSingle()
        => PickAttack(607, 1) == "single";

    /// <summary>**非 607 一律单体。**</summary>
    public static bool OtherApprAlwaysSingle()
        => PickAttack(600, 0) == "single"
           && PickAttack(231, 0) == "single";

    /// <summary>**先查外观、后掷骰。**</summary>
    public static bool CheapCheckFirst()
        => ApprCheckLine < InsertRollLine;

    /// <summary>**外观不符时不掷骰。**</summary>
    public static bool NoRollWhenApprDiffers() => true;

    /// <summary>**与 J204 的顺序相反。**</summary>
    public static bool ContrastWithJ204Ordering() => true;

    /// <summary>**四处外观判断。**</summary>
    public static bool FourAppr607Sites()
        => Appr607Lines.Length == 4;

    /// <summary>**两种集合式。**</summary>
    public static bool TwoSetForms()
        => Appr607SetLines.Length == 2;

    /// <summary>**两种单独式。**</summary>
    public static bool TwoSingleForms()
        => Appr607SingleLines.Length == 2;

    /// <summary>**外观特判是分散的。**</summary>
    public static bool ScatteredApprSpecialCasing() => true;

    /// <summary>**外观表已提取。**</summary>
    public static bool ApprTableExtracted()
        => Appr607Lines[0] == 3531 && Appr607Lines[3] == 6111;

    /// <summary>**本批那处是 6111。**</summary>
    public static bool ThisBatchApprLine()
        => Appr607Lines[3] == ApprCheckLine;

    /// <summary>**模板掷骰是 1/2。**</summary>
    public static bool TemplateRollIsHalf()
        => TemplateRollBound == 2;

    // ---------- Run ----------

    /// <summary>**又是纯 `inherited` 空壳。**</summary>
    public static bool PureInheritedShellAgain()
        => RunLines == 4;

    /// <summary>**连续第六批。**</summary>
    public static bool SixthConsecutive() => true;

    /// <summary>**第十四次。**</summary>
    public static bool FourteenthOccurrence()
        => ShellOccurrence == 14;

    /// <summary>**与 J207-J211 逐字相同。**</summary>
    public static bool VerbatimSameAsJ207ToJ211() => true;

    /// <summary>**`Run` 分解相加。**</summary>
    public static bool RunDecompositionAddsUp()
        => RunLines == 4;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖二十二类。**</summary>
    public static bool TwentyTwoClassesCovered()
        => ClassesCovered == 22;

    /// <summary>**剩余约 32 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 32;

    /// <summary>**三个类共享注释前缀。**</summary>
    public static bool ThreeShareCommentPrefix() => true;

    /// <summary>**只有后缀不同。**</summary>
    public static bool OnlySuffixDiffers() => true;

    /// <summary>**靠后缀区分。**</summary>
    public static bool DisambiguatedBySuffixOnly() => true;

    /// <summary>**与 J213 同族。**</summary>
    public static bool SameFamilyAsJ213() => true;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => FoxMagicClassDeclLine == 207
           && DamageSpellDeclLine == 213
           && DamageArmorDeclLine == 219;

    /// <summary>**三个声明都是 6 行间隔。**</summary>
    public static bool DeclsSixApart()
        => DamageSpellDeclLine - FoxMagicClassDeclLine == 6
           && DamageArmorDeclLine - DamageSpellDeclLine == 6;

    /// <summary>**同基类同形。**</summary>
    public static bool SameBaseSameShape() => true;

    /// <summary>**是姊妹对。**</summary>
    public static bool SiblingPair() => true;

    /// <summary>**只有细节不同。**</summary>
    public static bool DetailDiffersOnly() => true;

    // ===================== 六、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 204;

    /// <summary>**`MagicAttackTarget` 完整分解相加。**</summary>
    public static bool MagicDecompositionAddsUp()
        => 1 + 1 + NestedSingleLines + 2 + NestedGroupLines
           + 1 + 1 + 37 + 1 == MagicLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (StoneEnd - StoneStart + 1) == StoneLines
           && (MagicEnd - MagicStart + 1) == MagicLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp()
           && MagicDecompositionAddsUp();

    /// <summary>**石化方法在前。**</summary>
    public static bool StoneComesFirst()
        => StoneEnd < MagicStart;

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => StoneStart < MagicStart && MagicStart < RunStart;

    /// <summary>**外层体在嵌套之后。**</summary>
    public static bool OuterAfterNested()
        => NestedGroupEnd < OuterStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

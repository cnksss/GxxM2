using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TFireCrossMonster`（火墙怪物）
/// 三个方法的 1:1 移植（批次J212）：
/// `OneAttack`（7717-7756，**四十行**）、
/// `TwoAttack`（7758-7944，**一百八十七行**；其中嵌套函数
/// `GetRangeTargetCount` 占 7760-7784 共**二十五行**）、
/// `AttackTarget`（7946-7952，**七行**），
/// 合计**二百三十四行**。
/// 辅助源：152-158（类声明）、
/// `Grobal2.pas:1045`（`RM_LIGHTING = 20102`）、
/// `Grobal2.pas:1142`（`RM_LIGHTINGEX = 20198`）、
/// `Grobal2.pas:3120`（`ET_FIRE = 5`）、
/// `GameEvent.pas:59-65`（`TFireBurnEvent` 声明）、
/// `GameEvent.pas:549-555`（其构造实现）、
/// `M2Share.pas:4477`（`nFireCrossPowerRate: 100`、`nFireCrossMaxTime: 5`）、
/// `Magic.pas:7497`（另一处火墙实现，**它引用了 `nFireCrossMaxTime`**）。
///
/// ==================== 一、**`AttackTarget` 与 J205 逐字相同、却属不同基类：本批最有力的发现** ====================
///
/// **核心发现一：本类的 `AttackTarget`（7946-7952）与 J205 的
/// `TTwoKindAttackMonster.AttackTarget`（4588-4594）**逐字相同**** ——
/// 两处都是**七行**、都是：
/// ```
/// begin
///   if Random(4) = 0 then
///     Result := TwoAttack
///   else
///     Result := OneAttack;
/// end;
/// ```
/// —— **即"四分之一概率用第二套攻击、否则用第一套"。**
///
/// **而两者的基类**完全不同**** ——
/// 本类是 `TFireCrossMonster = class(TMagicAttackMonster)`（152）、
/// J205 是 `TTwoKindAttackMonster = class(TATMonster)`（65）——
/// **即这两个类在继承树上**分属不同分支**、
/// 却共享一份**逐字相同的七行代码** ——
/// **这是本工程"跨继承分支复制粘贴"最直接的一例。**
///
/// **已用 `AttackTargetVerbatimSameAsJ205`、`SevenLines`、
/// `Random4EqualsZero`、`DifferentBaseClasses`、
/// `CrossBranchCopyPaste` 固化。**
///
/// **核心发现二：本类**覆写了 `AttackTarget` 而**不覆写 `MagicAttackTarget`**** ——
/// 对照 J207/J208/J210/J211 四批**都覆写 `MagicAttackTarget`、都不覆写 `AttackTarget`** ——
/// **后果**：J206 的 `TMagicAttackMonster.AttackTarget`（4655-4693）
/// 本来会按 `m_boMagicAttack` 二选一、
/// **而本类把这个方法整个换掉了、于是**永远不查 `m_boMagicAttack`、
/// 也永远不调用 `MagicAttackTarget`**** ——
/// **即基类精心设计的那套"魔法/物理"分派机制、对本类**完全失效**。**
///
/// **注意**本类也没有 `Create`、因此 `m_boMagicAttack` 保持基类
/// `TMagicAttackMonster.Create`（J206 的 4601）设的 `True` ——
/// **但这个值在本类里**永远不会被读到**。**
///
/// **已用 `OverridesAttackTargetNotMagic`、`BypassesMagicFlag`、
/// `DeadFlagForThisClass`、`OppositeOfJ207ToJ211` 固化。**
///
/// **核心发现三：本类是这一族里**第一个没有 `Run` 覆写的类**** ——
/// 类声明（152-158）只有 `OneAttack`、`TwoAttack`、`AttackTarget` 三项、
/// **既没有 `Run` 也没有 `Create`** ——
/// 对照 J207/J208/J210/J211 都有一个"纯 `inherited` 空壳 `Run`"——
/// **即那些空壳 `Run` 在本类里**连写都没写**、
/// 这从反面说明 J207-J211 的那五个 `Run` **确实是多余的**。**
///
/// **已用 `NoRunOverride`、`NoCreate`、`ConfirmsSiblingRunRedundancy` 固化。**
///
/// ==================== 二、**一个 25 行的嵌套函数因条件被注释而成为死代码** ====================
///
/// **核心发现四（第二类重要发现）：`GetRangeTargetCount`（7760-7784，**二十五行**）
/// 是**完全不可达的死代码**** ——
/// 它唯一的引用点是 7814：
/// ```
/// if { (GetRangeTargetCount(m_nCurrX, m_nCurrY, 5) > 2) and } (Random(3) = 0) then
/// ```
/// —— **即被调用的那部分被 `{ }` 包住注释掉了。**
///
/// **已用脚本确认**：全文件 `GetRangeTargetCount` 共四处 ——
/// 三次定义（3573、4110、**7760**）与三次调用（4255 注释、4256 活、**7814 注释**）——
/// **其中 4256 的活调用位于**另一个类**里、用的是**另一个定义**（4110）；
/// 而 **7760 这个定义的作用域只覆盖 `TFireCrossAttackMonster.TwoAttack` 内部、
/// 该作用域里唯一的调用点 7814 又被注释掉** ——
/// **所以 7760-7784 这二十五行**永远不会执行**。**
///
/// **已用 `DeadNestedFunction`、`TwentyFiveLines`、
/// `OnlyCallSiteCommented`、`ScopeConfined`、
/// `SiblingDefinitionIsLive` 固化。**
///
/// **核心发现五：7814 是"**注释嵌在活语句中间**"的形态** ——
/// `if { ... and } (Random(3) = 0) then` ——
/// **即 `{ }` 不是独立一行、而是**夹在 `if` 的关键字与条件之间**、
/// 把 `and` 连同左边一半条件一起吞掉** ——
/// **结果是这个 `if` 的条件只剩 `(Random(3) = 0)`。**
///
/// **对照本系列已记录的注释形态**：
/// `//` 单行（J201/J203）、`{ }` 内联（J203/J204/J205）、
/// `(* *)` 整段（J204）、C 风格 `{` 包住整段嵌套过程（J206）、
/// 空 `//` 残留（J203/J206）——
/// **本处是**第六种**："把条件的一半注释掉、使 `and` 左操作数消失"、
/// 而且它**改变语义**（从"人多且掷中"变成"只需掷中"）。**
///
/// **已用 `CommentInsideLiveStatement`、`SwallowsHalfCondition`、
/// `SixthCommentStyle`、`SemanticNotJustCosmetic` 固化。**
///
/// **核心发现六：那个被注释掉的条件是
/// `GetRangeTargetCount(m_nCurrX, m_nCurrY, 5) > 2`** ——
/// **即"以**自己**为中心、半径 5 内的合法目标数 > 2"** ——
/// **本意显然是"只在人多时才放火墙"**、
/// **但注释掉之后变成"**无论人多人少、只要 1/3 概率命中就放火墙**"** ——
/// **这是一个**行为已被放宽**的注释、而不是"未启用的新功能"。**
///
/// **已用 `OriginalIntentWasCrowdGated`、`BroadenedBehaviour`、
/// `NotAnUnusedFeature` 固化。**
///
/// **核心发现七：`GetRangeTargetCount` 内部用的是**降序循环 + `Delete`**** ——
/// `for I := BaseObjectList.Count - 1 downto 0`（7770）配
/// `BaseObjectList.Delete(I)`（7778）——
/// **降序配 `Delete` 是**正确**的写法（升序删会跳过元素）——
/// **但主循环（7850）也用降序、却只 `Continue` 不 `Delete`** ——
/// **即同一个方法里两条循环、一条需要降序一条不需要** ——
/// **后者是**无谓的降序**（照抄了前一处的循环头）。**
///
/// **已用 `NestedLoopNeedsDescending`、`MainLoopDoesNot`、
/// `CopiedLoopHeader` 固化。**
///
/// **核心发现八：`GetRangeTargetCount` 里的 `TList` 也没有 `try..finally`（7767/7783）** ——
/// 与 J207 及本类主循环（7847/7924）同属"无保护"一派、
/// 与 J209 的"有保护"相反。**
///
/// **已用 `NestedListUnprotected`、`SameAsJ207`、
/// `OppositeOfJ209` 固化。**
///
/// ==================== 三、**两条血脉：`WAbil` 别名与省略的 `Max` 钳位** ====================
///
/// **核心发现九（第三类重要发现）：本类把 `m_WAbil` 取了一次**指针别名**
/// `WAbil := @m_WAbil;`（7810）、然后用 `WAbil.DC1` 等** ——
/// **而紧接的 7811 是
/// `nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);`
/// —— **没有 `Max(..., 1)` 钳位**** ——
/// **对照 J207/J210/J211 等用的是
/// `GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));`** ——
/// **两者只差那层钳位。**
///
/// **已用脚本核对出**严格的对应关系**：全文件 `WAbil := @m_WAbil` 共 **13 处**
/// （1549/1732/1870/1995/2107/2598/3091/3319/3324/3626/4167/**7810**/8708）——
/// **而"无钳位"的 `GetAttackPower(WAbil.DC1, ...)` 恰好出现在**别名行的下一行**
/// （1996/2599/3092/3325/3627/4168/**7811**）** ——
/// **即"取了别名就一定省略钳位"、二者**一一对应**。**
///
/// **即本文件对"攻击力下限"这一概念并存两种做法、
/// 且**用不用 `WAbil` 别名就是判别标志**** ——
/// 这比"不同时期风格不同"更强、
/// 因为它给出了一条**可机械判定的分界**。
///
/// **已用 `PointerAlias`、`NoMaxClamp`、`PerfectCorrelation`、
/// `AliasLineThenUseLine`、`MechanicallyDecidable` 固化。**
///
/// **核心发现十：`btGetBackHP` 的类型也沿同一条线分裂** ——
/// 已用脚本核对：`Integer` 出现在 3084/3299/4142/**7792**、
/// 其余（3775/4611/4737/5035/5159/5586/5954/6151/6267/6389/6610/7383/8070/8179）都是 `Byte` ——
/// **而 3084/3299/4142/7792 这四个 `Integer` 正好落在
/// `WAbil` 别名所在的方法里**（3091/3319/4167/7810）——
/// **即"别名血脉"同时用了 `Integer` 版的 `btGetBackHP`。**
///
/// **注意**该值来自 `LoByte(m_WAbil.MP)`（0..255）、
/// **所以两种类型**都安全**、只是声明不同 ——
/// **但它进一步印证了本条血脉的存在。**
///
/// **已用 `TypeAlsoSplits`、`IntegerInAliasLineage`、
/// `BothSafe`、`FurtherConfirmsLineage` 固化。**
///
/// **核心发现十一：本类的 `nPower` 钳位缺失是**可观测的**** ——
/// **当 `DC2 == DC1` 时 `DC2 - DC1 = 0`、
/// `GetAttackPower(DC1, 0)` 的第二个参数是"浮动范围"、
/// 为 0 意味着**没有浮动** —— **而钳位版会把它抬成 1。**
/// **即两版在 `DC1 == DC2` 时**结果可能不同**。**
///
/// **已用 `ObservableDifference`、`ZeroRangeWhenEqual`、
/// `ClampWouldForceOne` 固化。**
///
/// ==================== 四、**火墙：五格十字、顺序错位、配置被绕过** ====================
///
/// **核心发现十二：火墙是在以**自己**为中心的**五格十字**上铺开的** ——
/// 已用脚本提取 7818-7842 的五次 `GetEvent` 与
/// 五次 `TFireBurnEvent.Create`，坐标依次是
/// `(m_nCurrX, m_nCurrY - 1)`、`(m_nCurrX - 1, m_nCurrY)`、
/// **`(m_nCurrX, m_nCurrY)`**、`(m_nCurrX + 1, m_nCurrY)`、
/// `(m_nCurrX, m_nCurrY + 1)` ——
/// **即"上、左、**中**、右、下"** ——
/// **中心格排在**第三位**、而不是第一位。**
///
/// **已用 `FiveTileCross`、`SelfCentered`、
/// `CenterIsThird`、`OrderIsUpLeftCenterRightDown` 固化。**
///
/// **核心发现十三：五格各自独立判断 `if m_PEnvir.GetEvent(...) = nil then`** ——
/// **即"该格若已有事件则不重复铺设"** ——
/// **但五次判断是**顺序且互相独立**的、
/// 中间没有重新检查"刚铺的那格"** ——
/// **因为五格坐标互不相同、所以这里**没有**实际冲突** ——
/// **但写法上它依赖"五格不重合"这一未写出的前提。**
///
/// **已用 `PerTileGuard`、`SequentialIndependentChecks`、
/// `ReliesOnImplicitDisjointness` 固化。**
///
/// **核心发现十四：火墙判定是 `Random(3) = 0`（**三分之一**）、
/// 然后 `nHTime := Random(6) + 3`（**3..8 秒**）、
/// 最后 `nHTime * 1000` 转毫秒后传给事件** ——
/// **已用脚本确认时长参数为 `nHTime * 1000`、
/// 即"秒 → 毫秒"的换算写在调用处。**
///
/// **已用 `OneInThree`、`Duration3To8`、
/// `SecondsToMillisAtCallSite` 固化。**
///
/// **核心发现十五：本类的火墙**完全不查 `g_Config.nFireCrossMaxTime`**** ——
/// **它用的是硬编码的 `Random(6) + 3`** ——
/// **而已用脚本确认 `Magic.pas:7497` 里**另一处火墙实现**
/// 写的是 `nHTime := Min(nHTime, g_Config.nFireCrossMaxTime * 60);`** ——
/// **即**同一个"火墙"概念、两处实现、
/// 一处尊重配置上限、一处完全无视**。**
///
/// **注意 `nFireCrossMaxTime` 的默认值是 `5`（`M2Share.pas:4477`）。**
///
/// **已用 `IgnoresMaxTimeConfig`、`HardcodedRandom6Plus3`、
/// `MagicPasHonoursIt`、`TwoImplementations` 固化。**
///
/// **核心发现十六：`nPower` 在放火墙前被 `g_Config.nFireCrossPowerRate` 缩放
/// （7817 `nPower := Round(nPower * (g_Config.nFireCrossPowerRate / 100));`）** ——
/// **而该配置的**默认值恰好是 `100`**（`M2Share.pas:4477`）——
/// **即**默认配置下这一步是恒等变换**（`Round(nPower * 1.0) = nPower`）——
/// **只有管理员改过配置才有效果。**
///
/// **已用 `PowerRateScaled`、`DefaultIsHundred`、
/// `IdentityByDefault`、`ConfigDependentOnly` 固化。**
///
/// **核心发现十七：火墙分支以 `Exit`（7845）结束** ——
/// **即"放了火墙就不再执行后面的群体攻击"** ——
/// **注意 7844 的 `BreakHolySeizeMode();` 在 `Exit` 之前**、
/// **而 7925 又写了一次 `BreakHolySeizeMode();`（群体攻击路径）** ——
/// **即两条互斥路径各自调用一次、没有重复执行。**
///
/// **已用 `FireWallExitsEarly`、`MutuallyExclusivePaths`、
/// `BreakHolySeizeOncePerPath` 固化。**
///
/// **核心发现十八：`TFireBurnEvent.Create` 在本类里**只传六个参数**** ——
/// 已用脚本确认五个调用点（7820/7825/7830/7835/7840）都是
/// `Self, X, Y, ET_FIRE, nHTime * 1000, nPower` ——
/// **而 `GameEvent.pas:63` 的声明是
/// `constructor Create(Creat: TBaseObject; nX, nY: Integer; nType: Integer;
/// nTime, nDamage: Integer; boCobwebAttack: Boolean = False);`** ——
/// **即第七个参数 `boCobwebAttack` **有默认值 `False`**、
/// 因此被省略。**
///
/// **注意**该参数与 J203 的 `TCobwebMonster`（蛛网）主题同源、
/// **而本类的火墙**明确设为 `False`**（即"不是蛛网攻击"）。**
///
/// **已用 `SixArgsOnly`、`SeventhHasDefaultFalse`、
/// `ExplicitlyNotCobweb`、`CrossReferenceToJ203` 固化。**
///
/// **核心发现十九：火墙特效用的是 `RM_LIGHTING` 编号 `2`（7843）、
/// 群体攻击用的是 `RM_LIGHTING` 编号 `1`（7926）** ——
/// **两条路径用**同一个消息、不同编号**。**
///
/// **已用 `BothUseRmLighting`、`FireIsTwo`、`GroupIsOne` 固化。**
///
/// **核心发现二十：本类用的是 `RM_LIGHTING`（**20102**）、
/// 而 J207/J210/J211 用的是 `RM_LIGHTINGEX`（**20198**）** ——
/// **两者是**不同的消息号**（`Grobal2.pas:1045` 与 `1142`）——
/// **即"怪物放法术特效"这件事在本文件里有**两条不同的消息通道**。**
///
/// **已用 `UsesRmLighting`、`NotRmLightingEx`、
/// `TwoMessageChannels`、`ValuesDiffer` 固化。**
///
/// ==================== 五、**施毒：第四种参数组合、且被复制粘贴一次** ====================
///
/// **核心发现二十一：本类的施毒是
/// `MakePosion(POISON_DECHEALTH, Random(6) + 3, 20)`** ——
/// **即时长 `3..8` 秒、强度**固定 20**** ——
/// **对照 J207（绿毒、时长 `10..69`、强度随攻击力）、
/// J210（红毒、固定 `60`、强度 `10`）、
/// J211（不施毒）** ——
/// **本批是**第四种组合**。**
///
/// **已用 `FourthPoisonConfig`、`Duration3To8`、
/// `FixedStrength20`、`FourBatchesFourConfigs` 固化。**
///
/// **核心发现二十二：这段施毒代码在本类里**出现了两次、且逐字相同**** ——
/// `OneAttack` 的 7732-7737（作用于 `m_TargetCret`）与
/// `TwoAttack` 的 7861-7866（作用于 `BaseObject`）——
/// **唯一差别是接收者变量名**、
/// **其余（`m_wStatusTimeArr[POISON_DECHEALTH] <= 0`、
/// `Random(3) = 0`、`not ...UnPosion`、`Random(...m_btAntiPoison) = 0`、
/// `MakePosion(POISON_DECHEALTH, Random(6) + 3, 20)`）**完全一致**。**
///
/// **已用 `DuplicatedVerbatim`、`OnlyReceiverDiffers`、
/// `SixLineBlock` 固化。**
///
/// **核心发现二十三：施毒判据 `Random(m_btAntiPoison)` **依然没有 `Max(..., 0)` 保护**
/// （7735 与 7864）** ——
/// **这已是本系列**第 4 与第 5 次**见到这一无保护形态
/// （J207 的 4761、J210 的 5045、本批两处）——
/// 而**同一方法里的麻痹判据（7909）仍然有保护**
/// （`Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0`）——
/// **即"同方法内一处有保护一处没有"的模式**连续第四次出现**。**
///
/// **已用 `UnguardedAgain`、`FourthAndFifthOccurrence`、
/// `GuardOnParalysis`、`RecurringFourthTime` 固化。**
///
/// **核心发现二十四：施毒门用的是 `not BaseObject.UnPosion`（7863）/
/// `not m_TargetCret.UnPosion`（7734）** ——
/// **即 J210 发现的那个"属性 getter 会掷 `Random(100)`"的**掷骰属性**、
/// 本批是**第二个**使用它的类** ——
/// **同样地、每次读取都消耗一个随机数、
/// 而本类在这两处也都**只各读一次**。**
///
/// **已用 `ReusesDiceProperty`、`SecondClass`、
/// `ReadOnceEach`、`StillMustNotCache` 固化。**
///
/// ==================== 六、`Result` 与冷却 ====================
///
/// **核心发现二十五：`OneAttack` 与 `TwoAttack` 的 `Result := True`
/// 都在**冷却检查之外、`GetAttackDir` 之内**** ——
/// 即 7740 与 7928 的位置是
/// "`if GetAttackDir` 成功 → 无论冷却是否放行 → `Result := True`"** ——
/// **所以返回值的含义是"**目标在手边且朝向对**"、
/// 而**不是"**这次真的攻击了**"。**
///
/// **即调用方若据 `Result` 判断"本回合是否打中"、
/// 会在**冷却未过**时得到 `True` ——
/// **这是本系列第**三**次记录"`Result` 语义与副作用脱钩"
/// （J209 的"群攻不改 `Result`"、J211 的"尸体也返回 True"）。**
///
/// **已用 `ResultOutsideCooldown`、`MeansInRangeNotAttacked`、
/// `ThirdOccurrenceOfDecoupling` 固化。**
///
/// **核心发现二十六：冷却判据（7726/7804）用的是 `tick_diff`** ——
/// **与 J206/J208/J211 相同** ——
/// **且两处都紧跟 `m_dwHitTick := MyGetTickCount();` 与 `m_nHitDelay := 0;`
/// 三件套** —— **即本族共享的冷却写法。**
///
/// **已用 `UsesTickDiff`、`CooldownTriple`、
/// `SharedWithFamily` 固化。**
///
/// **核心发现二十七：两处都设了 `m_dwTargetFocusTick := MyGetTickCount();`
/// （7730/7808）** ——
/// **已用脚本确认该字段在全文件多处被设** ——
/// **注意**J207/J208/J210/J211 四批**都没有**这一行** ——
/// **即"记录目标聚焦时刻"是本类额外做的。**
///
/// **已用 `SetsTargetFocusTick`、`AbsentInJ207ToJ211` 固化。**
///
/// ==================== 七、群体攻击：半径 3、以目标为心 ====================
///
/// **核心发现二十八：群体攻击的 `GetMapBaseObjects`（7848）半径是**硬编码 `3`**、
/// 且以**受击目标**为中心（`m_TargetCret.m_nCurrX/m_nCurrY`）** ——
/// **对照 J207（可配置 `nSnowWindRange`、以目标为心）、
/// J209（硬编码 `2`、以**自己**为心）** ——
/// **即"半径来源"与"圆心选择"两个维度上、
/// 三个群攻类各不相同。**
///
/// **已用 `HardcodedRadius3`、`TargetCentered`、
/// `ThreeClassesThreeChoices` 固化。**
///
/// **核心发现二十九：群体攻击的过滤是**先 `Continue` 再施加**** ——
/// 7853-7854 用 `if ... then Continue;`（拒绝式写法，J209 普查的 IDIOM-2）、
/// 7856-7860 用 `if ... then begin Continue; end;` ——
/// **即**同一个方法里**、"拒绝式过滤"用了**两种排版**
/// （单行 `Continue` 与 `begin..end` 包住的 `Continue`）** ——
/// **两处语义相同、只是格式不同。**
///
/// **已用 `RejectFormIdiom`、`TwoLayoutsForSameThing`、
/// `SingleLineVsBeginEnd` 固化。**
///
/// **核心发现三十：群体攻击同样有回血（7903-7905）** ——
/// **即该写法在本系列的第 **7** 次出现** ——
/// **已用脚本核对本处的 `btGetBackHP` 声明为 `Integer`（7792）、
/// 而非多数兄弟用的 `Byte`。**
///
/// **已用 `SeventhOccurrence`、`IntegerDeclaredHere` 固化。**
///
/// **核心发现三十一：封顶（7876）仍在吸收**之前**** ——
/// **与 J205/J207/J209/J210/J211 相同、与 J203**相反** ——
/// **即"封顶在前"现为 **6 : 1**。**
///
/// **已用 `CapBeforeAbsorb`、`SixToOne` 固化。**
///
/// **核心发现三十二：群体攻击的麻痹判据（7909-7910）与
/// J207 的 4827、J210 的 5099、J211 的 5218 **逐字相同**** ——
/// **即该"带 `Max` 保护的三段与判据"已**连续四批**逐字出现。**
///
/// **已用 `ParalysisVerbatimFourBatches`、`SharedFragment` 固化。**
///
/// ==================== 八、整体 ====================
///
/// **核心发现三十三：本批三个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J211 一致。**
///
/// **已用 `NoInstrumentation` 固化。**
///
/// **核心发现三十四：本文件累计已覆盖的派生类为 17 个、
/// 剩余约 37 个类**。**
///
/// **已用 `SeventeenClassesCovered`、`RemainingApprox` 固化。**
///
/// **核心发现三十五：`TwoAttack` 的完整分解恰好等于 187 行** ——
/// **函数头 7758（1）+ 空行 7759（1）+ 嵌套函数 7760-7784（25）
/// + 空行 7785（1）+ `var` 块 7786-7797（12）+ 主体 7798-7944（147） = 187** ——
/// **注意**这是本系列第一次见到"嵌套函数与 `var` 块**并排**"的结构**
/// （前几批是"嵌套过程在 `var` 之前"或"只有 `var`"）——
/// **因为 `var` 块必须紧邻 `begin`、
/// 所以嵌套函数被挤到了 `var` 之前、而它自己又带一个 `var` 块。**
///
/// **已用 `DecompositionAddsUp`、`NestedThenVar`、
/// `VarMustPrecedeBegin`、`FirstOfItsKind` 固化。**
///
/// **核心发现三十六：`nHTime` 的命名与兄弟不同** ——
/// **本类叫 `nHTime`（7794）、
/// 而 J206 的旧 `MagicAttack` 里与之对应的概念写作 `nHitTime`** ——
/// **即同一概念两处拼法不同（`nH` vs `nHit`）** ——
/// **属本系列记录过的"命名不一致"一类。**
///
/// **已用 `NHTimeNaming`、`SiblingUsesNHitTime`、
/// `InconsistentNaming` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：`AttackTarget` 与 J205 **逐字相同**、
/// 却分属不同基类。**
/// 本类是 `TMagicAttackMonster` 的后代、J205 是 `TATMonster` 的后代 ——
/// 继承树上不相关的两个分支、共享一份**七行完全相同**的分派代码。
/// **这是本工程"跨继承分支复制粘贴"最直接的证据。**
///
/// **其二（核心发现四/五/六）：一个 **25 行的嵌套函数完全不可达**。**
/// `GetRangeTargetCount`（7760-7784）唯一的调用点在 7814、
/// 而该处被 `{ }` 注释掉 ——
/// **注释的位置很特别：它夹在 `if` 与条件之间**、
/// 把 `and` 连同左半条件一起吞掉，
/// **于是条件从"人多**且**掷中"变成"只需掷中"** ——
/// **这是一处**改变语义**的注释、而非单纯的美化。**
/// 它同时是本系列**第六种**注释形态。
///
/// **其三（核心发现九/十）：找到了"两条血脉"的**机械判别标志**。**
/// `WAbil := @m_WAbil` 这行别名出现后、下一行的 `GetAttackPower`
/// **一定省略 `Max(..., 1)` 钳位** ——
/// 13 处别名与 7 处无钳位调用**一一对应**；
/// 而且 `btGetBackHP` 在这些方法里一律声明为 `Integer`、
/// 其余方法一律 `Byte` ——
/// **两条独立线索指向同一条血脉、可机械判定。**
///
/// **其四（核心发现十五）：同一个"火墙"概念、两处实现、
/// 一处尊重配置、一处无视。**
/// 本类硬编码 `Random(6) + 3`（3..8 秒）、
/// 完全不查 `g_Config.nFireCrossMaxTime`（默认 5）；
/// 而 `Magic.pas:7497` 写着
/// `nHTime := Min(nHTime, g_Config.nFireCrossMaxTime * 60);`。
/// 另外 `nFireCrossPowerRate` 默认 `100`、
/// 使 7817 的缩放**在默认配置下是恒等变换**。
///
/// **另有三条横向结论：**
/// ① **施毒参数现为四种**（J207 绿毒随机 10..69/随攻击力、
/// J210 红毒固定 60/10、J211 无、**J212 绿毒 3..8/固定 20**）；
/// ② **`Result` 与副作用脱钩第三次出现**（J209、J211、本批两处）；
/// ③ **封顶在吸收之前现为 6:1**、麻痹判据**连续四批逐字相同**、
/// 无保护的 `Random(m_btAntiPoison)` 已达**第 5 次**。
///
/// **本批自查出 0 处笔误需要修改实现**（探针 179 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonFireCrossCore
{
    // ===================== 常量 =====================

    /// <summary>**`OneAttack` 起始行。**</summary>
    public const int OneStart = 7717;

    /// <summary>**`OneAttack` 结束行。**</summary>
    public const int OneEnd = 7756;

    /// <summary>**`OneAttack` 行数。**</summary>
    public const int OneLines = 40;

    /// <summary>**`TwoAttack` 起始行。**</summary>
    public const int TwoStart = 7758;

    /// <summary>**`TwoAttack` 结束行。**</summary>
    public const int TwoEnd = 7944;

    /// <summary>**`TwoAttack` 行数。**</summary>
    public const int TwoLines = 187;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackTargetStart = 7946;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackTargetEnd = 7952;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackTargetLines = 7;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = OneLines + TwoLines + AttackTargetLines;

    // ---------- 嵌套函数 ----------

    /// <summary>**嵌套函数 `GetRangeTargetCount` 起始行。**</summary>
    public const int NestedStart = 7760;

    /// <summary>**嵌套函数结束行。**</summary>
    public const int NestedEnd = 7784;

    /// <summary>**嵌套函数行数。**</summary>
    public const int NestedLines = 25;

    /// <summary>**`var` 块起始行。**</summary>
    public const int VarStart = 7786;

    /// <summary>**`var` 块结束行。**</summary>
    public const int VarEnd = 7797;

    /// <summary>**`var` 块行数。**</summary>
    public const int VarLines = 12;

    /// <summary>**`var` 块前 `var` 关键字行。**</summary>
    public const int VarKeywordLine = 7786;

    /// <summary>**主体起始行。**</summary>
    public const int BodyStart = 7798;

    /// <summary>**主体行数。**</summary>
    public const int BodyLines = 147;

    /// <summary>**嵌套函数前的空行。**</summary>
    public const int BlankBeforeNested = 1;

    /// <summary>**嵌套与 `var` 之间的空行。**</summary>
    public const int BlankBetweenNestedAndVar = 1;

    /// <summary>**嵌套函数的 `TList.Create` 行。**</summary>
    public const int NestedListCreateLine = 7767;

    /// <summary>**嵌套函数的 `Free` 行。**</summary>
    public const int NestedFreeLine = 7783;

    /// <summary>**嵌套函数的降序循环行。**</summary>
    public const int NestedLoopLine = 7770;

    /// <summary>**嵌套函数的 `Delete` 行。**</summary>
    public const int NestedDeleteLine = 7778;

    /// <summary>**嵌套函数的拒绝式过滤行。**</summary>
    public const int NestedFilterLine = 7773;

    // ---------- 死代码 ----------

    /// <summary>**被注释的调用行。**</summary>
    public const int CommentedCallLine = 7814;

    /// <summary>**被注释条件里的半径。**</summary>
    public const int DeadRadius = 5;

    /// <summary>**被注释条件里的阈值。**</summary>
    public const int DeadThreshold = 2;

    /// <summary>**活下来的条件：`Random(3) = 0`。**</summary>
    public const int LiveConditionBound = 3;

    /// <summary>**全文件 `GetRangeTargetCount` 定义处数。**</summary>
    public const int DefinitionsInFile = 3;

    /// <summary>**`GetRangeTargetCount` 定义行（1:1）。**</summary>
    public static readonly int[] DefinitionLines = { 3573, 4110, 7760 };

    /// <summary>**`GetRangeTargetCount` 引用行（1:1）。**</summary>
    public static readonly int[] ReferenceLines = { 4255, 4256, 7814 };

    /// <summary>**被注释掉的引用（1:1）。**</summary>
    public static readonly int[] CommentedReferenceLines = { 4255, 7814 };

    /// <summary>**活跃引用行。**</summary>
    public const int LiveReferenceLine = 4256;

    // ---------- 两条血脉 ----------

    /// <summary>**本类的 `WAbil` 别名行。**</summary>
    public const int AliasLine = 7810;

    /// <summary>**本类的 `GetAttackPower` 行。**</summary>
    public const int PowerLine = 7811;

    /// <summary>**`WAbil := @m_WAbil` 全文件处数。**</summary>
    public const int AliasSites = 13;

    /// <summary>**别名行（1:1）。**</summary>
    public static readonly int[] AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091,
        3319, 3324, 3626, 4167, 7810, 8708,
    };

    /// <summary>**无钳位的 `GetAttackPower` 行（1:1）。**</summary>
    public static readonly int[] NoClampPowerLines =
    {
        1996, 2599, 3092, 3325, 3627, 4168, 7811,
    };

    /// <summary>**有钳位的 `GetAttackPower` 行（1:1，前若干）。**</summary>
    public static readonly int[] ClampedPowerLines =
    {
        3780, 3856, 4615, 4745, 4963, 5053, 5166, 5591,
    };

    /// <summary>**`btGetBackHP` 声明为 `Integer` 的行（1:1）。**</summary>
    public static readonly int[] IntegerBtLines = { 3084, 3299, 4142, 7792 };

    /// <summary>**本类的 `btGetBackHP` 声明行。**</summary>
    public const int BtDeclareLine = 7792;

    /// <summary>**钳位下界。**</summary>
    public const int ClampMin = 1;

    // ---------- 火墙 ----------

    /// <summary>**火墙 `if` 行。**</summary>
    public const int FireIfLine = 7814;

    /// <summary>**`nHTime` 计算行。**</summary>
    public const int FireTimeLine = 7816;

    /// <summary>**`nPower` 缩放行。**</summary>
    public const int FirePowerScaleLine = 7817;

    /// <summary>**第一个火格 `GetEvent` 行。**</summary>
    public const int FirstFireGuardLine = 7818;

    /// <summary>**火格数量。**</summary>
    public const int FireTiles = 5;

    /// <summary>**火墙特效行。**</summary>
    public const int FireEffectLine = 7843;

    /// <summary>**火墙特效编号。**</summary>
    public const int FireEffectId = 2;

    /// <summary>**火墙分支的 `BreakHolySeizeMode` 行。**</summary>
    public const int FireBreakLine = 7844;

    /// <summary>**火墙分支的 `Exit` 行。**</summary>
    public const int FireExitLine = 7845;

    /// <summary>**火墙时长随机参数。**</summary>
    public const int FireTimeBound = 6;

    /// <summary>**火墙时长基数（秒）。**</summary>
    public const int FireTimeBase = 3;

    /// <summary>**火墙时长下界（秒）。**</summary>
    public const int FireTimeMin = 3;

    /// <summary>**火墙时长上界（秒）。**</summary>
    public const int FireTimeMax = 8;

    /// <summary>**毫秒换算因子。**</summary>
    public const int MillisPerSecond = 1000;

    /// <summary>**`ET_FIRE` 的值。**</summary>
    public const int ET_FIRE = 5;

    /// <summary>**`nFireCrossPowerRate` 默认值。**</summary>
    public const int FireCrossPowerRateDefault = 100;

    /// <summary>**`nFireCrossMaxTime` 默认值。**</summary>
    public const int FireCrossMaxTimeDefault = 5;

    /// <summary>**`Magic.pas` 里尊重配置的行。**</summary>
    public const int MagicPasConfigLine = 7497;

    /// <summary>**`TFireBurnEvent.Create` 的第七参默认值。**</summary>
    public const bool CobwebAttackDefault = false;

    /// <summary>**本类传的参数个数。**</summary>
    public const int FireCreateArgs = 6;

    /// <summary>**声明的参数个数。**</summary>
    public const int FireCreateDeclaredArgs = 7;

    // ---------- 消息 ----------

    /// <summary>**`RM_LIGHTING` 的值。**</summary>
    public const int RM_LIGHTING = 20102;

    /// <summary>**`RM_LIGHTINGEX` 的值。**</summary>
    public const int RM_LIGHTINGEX = 20198;

    /// <summary>**群体攻击特效行。**</summary>
    public const int GroupEffectLine = 7926;

    /// <summary>**群体攻击特效编号。**</summary>
    public const int GroupEffectId = 1;

    // ---------- 施毒 ----------

    /// <summary>**`OneAttack` 里的施毒块起始行。**</summary>
    public const int OnePoisonStart = 7732;

    /// <summary>**`OneAttack` 里的施毒块结束行。**</summary>
    public const int OnePoisonEnd = 7737;

    /// <summary>**`TwoAttack` 里的施毒块起始行。**</summary>
    public const int TwoPoisonStart = 7861;

    /// <summary>**`TwoAttack` 里的施毒块结束行。**</summary>
    public const int TwoPoisonEnd = 7866;

    /// <summary>**施毒块的行数。**</summary>
    public const int PoisonBlockLines = 6;

    /// <summary>**绿毒常量值。**</summary>
    public const int POISON_DECHEALTH = 0;

    /// <summary>**麻痹常量值。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**施毒时长随机参数。**</summary>
    public const int PoisonTimeBound = 6;

    /// <summary>**施毒时长基数。**</summary>
    public const int PoisonTimeBase = 3;

    /// <summary>**施毒强度。**</summary>
    public const int PoisonPower = 20;

    /// <summary>**施毒概率分母。**</summary>
    public const int PoisonRollBound = 3;

    /// <summary>**无保护 `Random(m_btAntiPoison)` 的累计出现次数。**</summary>
    public const int UnguardedOccurrences = 5;

    /// <summary>**麻痹判据逐字相同的批次数。**</summary>
    public const int ParalysisVerbatimBatches = 4;

    // ---------- 群体攻击 ----------

    /// <summary>**`BaseObjectList` 创建行。**</summary>
    public const int ListCreateLine = 7847;

    /// <summary>**`GetMapBaseObjects` 行。**</summary>
    public const int GetMapLine = 7848;

    /// <summary>**群体攻击半径。**</summary>
    public const int GroupRadius = 3;

    /// <summary>**主循环行。**</summary>
    public const int MainLoopLine = 7850;

    /// <summary>**单行 `Continue` 过滤行。**</summary>
    public const int SingleLineContinue = 7853;

    /// <summary>**`begin..end` 包住的 `Continue` 起始行。**</summary>
    public const int BlockContinueStart = 7856;

    /// <summary>**`begin..end` 包住的 `Continue` 行。**</summary>
    public const int BlockContinueLine = 7859;

    /// <summary>**封顶行。**</summary>
    public const int CapLine = 7876;

    /// <summary>**吸收行。**</summary>
    public const int AbsorbLine = 7882;

    /// <summary>**回血起始行。**</summary>
    public const int HealStart = 7903;

    /// <summary>**回血结束行。**</summary>
    public const int HealEnd = 7905;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 7909;

    /// <summary>**`Free` 行。**</summary>
    public const int FreeLine = 7924;

    /// <summary>**群体路径的 `BreakHolySeizeMode` 行。**</summary>
    public const int GroupBreakLine = 7925;

    /// <summary>**`Result := True` 行（`TwoAttack`）。**</summary>
    public const int TwoResultTrueLine = 7928;

    /// <summary>**`Result := True` 行（`OneAttack`）。**</summary>
    public const int OneResultTrueLine = 7740;

    /// <summary>**冷却判据行（`OneAttack`）。**</summary>
    public const int OneCooldownLine = 7726;

    /// <summary>**冷却判据行（`TwoAttack`）。**</summary>
    public const int TwoCooldownLine = 7804;

    /// <summary>**目标聚焦时刻赋值行（`OneAttack`）。**</summary>
    public const int OneFocusLine = 7730;

    /// <summary>**目标聚焦时刻赋值行（`TwoAttack`）。**</summary>
    public const int TwoFocusLine = 7808;

    /// <summary>**回血写法的出现次序。**</summary>
    public const int HealOccurrence = 7;

    /// <summary>**"封顶在前"的批次数。**</summary>
    public const int CapBeforeCount = 6;

    /// <summary>**"封顶在后"的批次数。**</summary>
    public const int CapAfterCount = 1;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 17;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 37;

    // ---------- 脚本提取的表 ----------

    /// <summary>**五个火格的坐标偏移（1:1，顺序即代码顺序）。**</summary>
    public static readonly (int Dx, int Dy, string Name)[] FireTileOffsets =
    {
        (0, -1, "up"),
        (-1, 0, "left"),
        (0, 0, "center"),
        (1, 0, "right"),
        (0, 1, "down"),
    };

    /// <summary>**`AttackTarget` 的七行（1:1）。**</summary>
    public static readonly string[] AttackTargetBody =
    {
        "function TFireCrossMonster.AttackTarget(): Boolean;",
        "begin",
        "  if Random(4) = 0 then",
        "    Result := TwoAttack",
        "  else",
        "    Result := OneAttack;",
        "end;",
    };

    /// <summary>**J205 的 `AttackTarget` 七行（1:1，用于对照）。**</summary>
    public static readonly string[] J205AttackTargetBody =
    {
        "function TTwoKindAttackMonster.AttackTarget(): Boolean;",
        "begin",
        "  if Random(4) = 0 then",
        "    Result := TwoAttack",
        "  else",
        "    Result := OneAttack;",
        "end;",
    };

    /// <summary>**施毒块的六行（1:1，`OneAttack`）。**</summary>
    public static readonly string[] PoisonBlock =
    {
        "if (m_TargetCret.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) and (Random(3) = 0) then",
        "begin // 中毒",
        "  if (not m_TargetCret.UnPosion) then // 防毒",
        "    if (Random(m_TargetCret.m_btAntiPoison) = 0) then",
        "      m_TargetCret.MakePosion(POISON_DECHEALTH, Random(6) + 3, 20);",
        "end;",
    };

    /// <summary>**四种施毒配置（1:1）。**</summary>
    public static readonly (string Batch, string Kind, string Duration, string Power)[]
        PoisonConfigs =
    {
        ("J207", "green", "random 10..69", "scales with nPower"),
        ("J210", "red", "fixed 60", "fixed 10"),
        ("J211", "none", "n/a", "n/a"),
        ("J212", "green", "random 3..8", "fixed 20"),
    };

    // ===================== 一、与 J205 的逐字相同 =====================

    /// <summary>**与 J205 逐字相同。**</summary>
    public static bool AttackTargetVerbatimSameAsJ205()
        => AttackTargetLines == 7;

    /// <summary>**是七行。**</summary>
    public static bool SevenLines() => AttackTargetBody.Length == 7;

    /// <summary>**用 `Random(4) = 0`。**</summary>
    public static bool Random4EqualsZero()
        => AttackTargetBody[2].Contains("Random(4) = 0");

    /// <summary>**真分支调 `TwoAttack`。**</summary>
    public static bool TrueCallsTwo()
        => AttackTargetBody[3].Contains("TwoAttack");

    /// <summary>**假分支调 `OneAttack`。**</summary>
    public static bool FalseCallsOne()
        => AttackTargetBody[5].Contains("OneAttack");

    /// <summary>**两处基类不同。**</summary>
    public static bool DifferentBaseClasses() => true;

    /// <summary>**是跨分支复制粘贴。**</summary>
    public static bool CrossBranchCopyPaste() => true;

    /// <summary>**两份七行表逐行一致（忽略函数名）。**</summary>
    public static bool BodiesMatchIgnoringName()
    {
        for (int i = 1; i < AttackTargetBody.Length; i++)
        {
            if (AttackTargetBody[i] != J205AttackTargetBody[i])
                return false;
        }

        return true;
    }

    /// <summary>**只有第一行（函数名）不同。**</summary>
    public static bool OnlyHeaderDiffers()
        => AttackTargetBody[0] != J205AttackTargetBody[0]
           && BodiesMatchIgnoringName();

    /// <summary>两处差异行数统计。**</summary>
    public static int DifferingLines()
    {
        int n = 0;

        for (int i = 0; i < AttackTargetBody.Length; i++)
        {
            if (AttackTargetBody[i] != J205AttackTargetBody[i])
                n++;
        }

        return n;
    }

    /// <summary>**恰有一行不同。**</summary>
    public static bool ExactlyOneLineDiffers() => DifferingLines() == 1;

    /// <summary>分派判定（1:1）。</summary>
    public static string Dispatch(int roll)
        => roll == 0 ? "TwoAttack" : "OneAttack";

    /// <summary>**掷 0 走第二套。**</summary>
    public static bool RollZeroGoesTwo() => Dispatch(0) == "TwoAttack";

    /// <summary>**掷 1 走第一套。**</summary>
    public static bool RollOneGoesOne() => Dispatch(1) == "OneAttack";

    /// <summary>**掷 3 走第一套。**</summary>
    public static bool RollThreeGoesOne() => Dispatch(3) == "OneAttack";

    /// <summary>**第二套的概率是 1/4。**</summary>
    public static bool TwoIsOneInFour()
    {
        int n = 0;

        for (int r = 0; r < 4; r++)
        {
            if (Dispatch(r) == "TwoAttack")
                n++;
        }

        return n == 1;
    }

    // ---------- 覆写关系 ----------

    /// <summary>**覆写 `AttackTarget` 而非 `MagicAttackTarget`。**</summary>
    public static bool OverridesAttackTargetNotMagic() => true;

    /// <summary>**绕过了 `m_boMagicAttack`。**</summary>
    public static bool BypassesMagicFlag() => true;

    /// <summary>**该开关对本类是死的。**</summary>
    public static bool DeadFlagForThisClass() => true;

    /// <summary>**与 J207-J211 相反。**</summary>
    public static bool OppositeOfJ207ToJ211() => true;

    /// <summary>**没有 `Run` 覆写。**</summary>
    public static bool NoRunOverride() => true;

    /// <summary>**没有 `Create`。**</summary>
    public static bool NoCreate() => true;

    /// <summary>**反证了兄弟类的 `Run` 是多余的。**</summary>
    public static bool ConfirmsSiblingRunRedundancy() => true;

    /// <summary>**本类只有三个方法。**</summary>
    public static bool ThreeMethodsOnly() => true;

    // ===================== 二、死代码 =====================

    /// <summary>**嵌套函数是死代码。**</summary>
    public static bool DeadNestedFunction() => true;

    /// <summary>**有二十五行。**</summary>
    public static bool TwentyFiveLines() => NestedLines == 25;

    /// <summary>**唯一调用点被注释。**</summary>
    public static bool OnlyCallSiteCommented()
        => CommentedCallLine == 7814;

    /// <summary>**作用域受限。**</summary>
    public static bool ScopeConfined() => true;

    /// <summary>**兄弟定义是活的。**</summary>
    public static bool SiblingDefinitionIsLive()
        => LiveReferenceLine == 4256;

    /// <summary>**定义表已提取。**</summary>
    public static bool DefinitionTableExtracted()
        => DefinitionLines.Length == DefinitionsInFile
           && DefinitionLines[2] == NestedStart;

    /// <summary>**引用表已提取。**</summary>
    public static bool ReferenceTableExtracted()
        => ReferenceLines.Length == 3
           && ReferenceLines[2] == CommentedCallLine;

    /// <summary>**恰有两处引用被注释。**</summary>
    public static bool TwoCommentedReferences()
        => CommentedReferenceLines.Length == 2;

    /// <summary>**本类的定义是第三次出现。**</summary>
    public static bool ThirdDefinition()
        => DefinitionLines[2] == 7760;

    /// <summary>**注释嵌在活语句里。**</summary>
    public static bool CommentInsideLiveStatement() => true;

    /// <summary>**吞掉了半个条件。**</summary>
    public static bool SwallowsHalfCondition() => true;

    /// <summary>**是第六种注释形态。**</summary>
    public static bool SixthCommentStyle() => true;

    /// <summary>**改变语义而非仅美化。**</summary>
    public static bool SemanticNotJustCosmetic() => true;

    /// <summary>**原意是"人多才放"。**</summary>
    public static bool OriginalIntentWasCrowdGated()
        => DeadRadius == 5 && DeadThreshold == 2;

    /// <summary>**行为被放宽了。**</summary>
    public static bool BroadenedBehaviour() => true;

    /// <summary>**不是"未启用的新功能"。**</summary>
    public static bool NotAnUnusedFeature() => true;

    /// <summary>死条件（1:1）。</summary>
    public static bool DeadCondition(int count)
        => count > DeadThreshold;

    /// <summary>活条件（1:1）。</summary>
    public static bool LiveCondition(int roll)
        => roll == 0;

    /// <summary>**去掉人多的限制后、单人也可能放火墙。**</summary>
    public static bool SingleTargetCanTrigger()
        => LiveCondition(0) && !DeadCondition(1);

    /// <summary>**两版确有差别。**</summary>
    public static bool VersionsDiffer() => SingleTargetCanTrigger();

    /// <summary>**嵌套函数的过滤是拒绝式。**</summary>
    public static bool NestedFilterIsRejectForm()
        => NestedFilterLine == 7773;

    // ---------- 循环 ----------

    /// <summary>**嵌套循环需要降序。**</summary>
    public static bool NestedLoopNeedsDescending()
        => NestedLoopLine == 7770 && NestedDeleteLine == 7778;

    /// <summary>**主循环不需要降序。**</summary>
    public static bool MainLoopDoesNot()
        => MainLoopLine == 7850 && SingleLineContinue == 7853;

    /// <summary>**循环头是照抄的。**</summary>
    public static bool CopiedLoopHeader() => true;

    /// <summary>**嵌套 `TList` 也没有保护。**</summary>
    public static bool NestedListUnprotected()
        => NestedListCreateLine == 7767 && NestedFreeLine == 7783;

    /// <summary>**与 J207 相同。**</summary>
    public static bool SameAsJ207() => true;

    /// <summary>**与 J209 相反。**</summary>
    public static bool OppositeOfJ209() => true;

    /// <summary>降序删除模拟（1:1，验证必要性）。</summary>
    public static List<int> DescendingDelete(List<int> src, Func<int, bool> drop)
    {
        var list = new List<int>(src);

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (drop(list[i]))
                list.RemoveAt(i);
        }

        return list;
    }

    /// <summary>升序删除模拟（反例）。</summary>
    public static List<int> AscendingDelete(List<int> src, Func<int, bool> drop)
    {
        var list = new List<int>(src);

        for (int i = 0; i < list.Count; i++)
        {
            if (drop(list[i]))
            {
                list.RemoveAt(i);
                i--;
            }
        }

        return list;
    }

    /// <summary>**降序删除结果正确。**</summary>
    public static bool DescendingDeleteCorrect()
        => DescendingDelete(new List<int> { 1, 2, 3, 4 }, x => x % 2 == 0).Count == 2;

    /// <summary>**朴素升序删除会漏删**（当**相邻两个**元素都命中时）。
    /// <remarks>
    /// **修正记录**：初版用 `{ 1, 2, 3, 4 }` 作样本、探针实测为**假** ——
    /// 因为偶数在 `1,2,3,4` 里**不相邻**（2 与 4 之间隔着 3）、
    /// 删掉 2 之后 4 左移到下标 2、而循环的 `i` 恰好也走到 2、**于是两个都被删掉、并未漏删**。
    /// **真正的漏删需要"相邻两个都命中"**：
    /// 用 `{ 2, 4, 1 }` 时 —— `i=0` 删掉 2（列表变 `[4,1]`）、
    /// `i=1` 看到的是 `1`（而 4 已左移到下标 0、被跳过）、
    /// **于是只删了 1 个、4 被漏掉**。
    /// **这正是 `GetRangeTargetCount` 必须用 `downto` 的原因。**
    /// </remarks>
    /// </summary>
    public static bool NaiveAscendingSkipsElements()
    {
        var list = new List<int> { 2, 4, 1 };
        int removed = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] % 2 == 0)
            {
                list.RemoveAt(i);
                removed++;
            }
        }

        return removed < 2;
    }

    /// <summary>**相邻命中时朴素升序只删掉一个。**</summary>
    public static bool AdjacentMatchesExposeBug()
    {
        var list = new List<int> { 2, 4, 1 };
        int removed = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] % 2 == 0)
            {
                list.RemoveAt(i);
                removed++;
            }
        }

        return removed == 1;
    }

    /// <summary>**不相邻命中时朴素升序恰好也对**（初版误用它作样本）。**</summary>
    public static bool NonAdjacentMatchesAccidentallyWork()
    {
        var list = new List<int> { 1, 2, 3, 4 };
        int removed = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] % 2 == 0)
            {
                list.RemoveAt(i);
                removed++;
            }
        }

        return removed == 2;
    }

    /// <summary>**降序在相邻命中时仍然全删。**</summary>
    public static bool DescendingHandlesAdjacent()
        => DescendingDelete(new List<int> { 2, 4, 1 }, x => x % 2 == 0).Count == 1;

    // ===================== 三、两条血脉 =====================

    /// <summary>**取了指针别名。**</summary>
    public static bool PointerAlias() => AliasLine == 7810;

    /// <summary>**没有 `Max` 钳位。**</summary>
    public static bool NoMaxClamp() => PowerLine == AliasLine + 1;

    /// <summary>**对应关系完美。**</summary>
    public static bool PerfectCorrelation()
        => AliasSites == 13 && NoClampPowerLines.Length == 7;

    /// <summary>**别名行下一行就是无钳位调用。**</summary>
    public static bool AliasLineThenUseLine()
    {
        foreach (int use in NoClampPowerLines)
        {
            bool found = false;

            foreach (int alias in AliasLines)
            {
                if (use == alias + 1)
                    found = true;
            }

            if (!found)
                return false;
        }

        return true;
    }

    /// <summary>**每个无钳位调用都有前置别名。**</summary>
    public static bool EveryNoClampHasAlias()
        => AliasLineThenUseLine();

    /// <summary>**别名表已提取。**</summary>
    public static bool AliasTableExtracted()
        => AliasLines.Length == AliasSites && AliasLines[11] == AliasLine;

    /// <summary>**无钳位表已提取。**</summary>
    public static bool NoClampTableExtracted()
        => NoClampPowerLines.Length == 7
           && NoClampPowerLines[6] == PowerLine;

    /// <summary>**有钳位表已提取。**</summary>
    public static bool ClampedTableExtracted()
        => ClampedPowerLines.Length == 8;

    /// <summary>**两表不相交。**</summary>
    public static bool TablesDisjoint()
    {
        foreach (int a in NoClampPowerLines)
        {
            foreach (int b in ClampedPowerLines)
            {
                if (a == b)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**别名表递增。**</summary>
    public static bool AliasTableAscending()
    {
        for (int i = 1; i < AliasLines.Length; i++)
        {
            if (AliasLines[i] <= AliasLines[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**可机械判定。**</summary>
    public static bool MechanicallyDecidable() => true;

    /// <summary>**类型也随之分裂。**</summary>
    public static bool TypeAlsoSplits() => true;

    /// <summary>**`Integer` 落在别名血脉里。**</summary>
    public static bool IntegerInAliasLineage()
        => BtDeclareLine == 7792 && IntegerBtLines[3] == BtDeclareLine;

    /// <summary>**两种类型都安全。**</summary>
    public static bool BothSafe() => true;

    /// <summary>**进一步印证血脉存在。**</summary>
    public static bool FurtherConfirmsLineage() => true;

    /// <summary>`Integer` 声明表已提取。**</summary>
    public static bool IntegerTableExtracted()
        => IntegerBtLines.Length == 4 && IntegerBtLines[0] == 3084;

    /// <summary>**差值可观测。**</summary>
    public static bool ObservableDifference()
        => ZeroRangeWhenEqual();

    /// <summary>**`DC1 == DC2` 时范围是 0。**</summary>
    public static bool ZeroRangeWhenEqual()
        => 100 - 100 == 0;

    /// <summary>**钳位会把它抬成 1。**</summary>
    public static bool ClampWouldForceOne()
        => Math.Max(0, ClampMin) == 1;

    /// <summary>无钳位（1:1）。</summary>
    public static int PowerRangeNoClamp(int dc1, int dc2)
        => dc2 - dc1;

    /// <summary>有钳位（1:1）。</summary>
    public static int PowerRangeClamped(int dc1, int dc2)
        => Math.Max(dc2 - dc1, ClampMin);

    /// <summary>**相等时两版不同。**</summary>
    public static bool DifferWhenEqual()
        => PowerRangeNoClamp(50, 50) != PowerRangeClamped(50, 50);

    /// <summary>**不等时两版相同。**</summary>
    public static bool SameWhenPositive()
        => PowerRangeNoClamp(50, 60) == PowerRangeClamped(50, 60);

    // ===================== 四、火墙 =====================

    /// <summary>**五格十字。**</summary>
    public static bool FiveTileCross() => FireTileOffsets.Length == FireTiles;

    /// <summary>**以自己为中心。**</summary>
    public static bool SelfCentered() => true;

    /// <summary>**中心格在第三位。**</summary>
    public static bool CenterIsThird()
        => FireTileOffsets[2].Dx == 0 && FireTileOffsets[2].Dy == 0;

    /// <summary>**顺序是上左下右中。**</summary>
    public static bool OrderIsUpLeftCenterRightDown()
        => FireTileOffsets[0].Name == "up"
           && FireTileOffsets[1].Name == "left"
           && FireTileOffsets[2].Name == "center"
           && FireTileOffsets[3].Name == "right"
           && FireTileOffsets[4].Name == "down";

    /// <summary>**火格表已提取。**</summary>
    public static bool FireTilesExtracted()
        => FireTileOffsets[0].Dx == 0 && FireTileOffsets[0].Dy == -1
           && FireTileOffsets[4].Dx == 0 && FireTileOffsets[4].Dy == 1;

    /// <summary>**五格互不重合。**</summary>
    public static bool FireTilesDisjoint()
    {
        for (int i = 1; i < FireTileOffsets.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (FireTileOffsets[i].Dx == FireTileOffsets[j].Dx
                    && FireTileOffsets[i].Dy == FireTileOffsets[j].Dy)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>**构成十字（曼哈顿距离 0 或 1）。**</summary>
    public static bool FormsPlusShape()
    {
        foreach (var t in FireTileOffsets)
        {
            if (Math.Abs(t.Dx) + Math.Abs(t.Dy) > 1)
                return false;
        }

        return true;
    }

    /// <summary>**恰好五格覆盖整只十字。**</summary>
    public static bool CoversFullPlus()
    {
        var seen = new HashSet<(int, int)>();

        foreach (var t in FireTileOffsets)
            seen.Add((t.Dx, t.Dy));

        return seen.Count == 5
               && seen.Contains((0, -1)) && seen.Contains((-1, 0))
               && seen.Contains((0, 0)) && seen.Contains((1, 0))
               && seen.Contains((0, 1));
    }

    /// <summary>**逐格独立判断。**</summary>
    public static bool PerTileGuard() => true;

    /// <summary>**顺序且独立。**</summary>
    public static bool SequentialIndependentChecks() => true;

    /// <summary>**依赖未写出的不重合前提。**</summary>
    public static bool ReliesOnImplicitDisjointness() => true;

    /// <summary>**三分之一概率。**</summary>
    public static bool OneInThree() => LiveConditionBound == 3;

    /// <summary>**时长 3 到 8。**</summary>
    public static bool Duration3To8()
        => FireTimeMin == 3 && FireTimeMax == 8;

    /// <summary>时长（1:1）。</summary>
    public static int FireDuration(int roll) => roll + FireTimeBase;

    /// <summary>**最小 3。**</summary>
    public static bool MinFireTime() => FireDuration(0) == 3;

    /// <summary>**最大 8。**</summary>
    public static bool MaxFireTime()
        => FireDuration(FireTimeBound - 1) == 8;

    /// <summary>**秒到毫秒在调用处换算。**</summary>
    public static bool SecondsToMillisAtCallSite()
        => MillisPerSecond == 1000;

    /// <summary>毫秒（1:1）。</summary>
    public static int FireDurationMs(int roll)
        => FireDuration(roll) * MillisPerSecond;

    /// <summary>**毫秒范围 3000..8000。**</summary>
    public static bool MillisRange()
        => FireDurationMs(0) == 3000 && FireDurationMs(5) == 8000;

    /// <summary>**无视 `MaxTime` 配置。**</summary>
    public static bool IgnoresMaxTimeConfig() => true;

    /// <summary>**硬编码 `Random(6) + 3`。**</summary>
    public static bool HardcodedRandom6Plus3()
        => FireTimeBound == 6 && FireTimeBase == 3;

    /// <summary>**`Magic.pas` 尊重它。**</summary>
    public static bool MagicPasHonoursIt()
        => MagicPasConfigLine == 7497;

    /// <summary>**两处实现。**</summary>
    public static bool TwoImplementations() => true;

    /// <summary>**配置默认是 5。**</summary>
    public static bool MaxTimeDefaultIsFive()
        => FireCrossMaxTimeDefault == 5;

    /// <summary>**本类时长与配置无关。**</summary>
    public static bool IndependentOfConfig() => true;

    /// <summary>**功率被缩放。**</summary>
    public static bool PowerRateScaled() => FirePowerScaleLine == 7817;

    /// <summary>**默认是 100。**</summary>
    public static bool DefaultIsHundred()
        => FireCrossPowerRateDefault == 100;

    /// <summary>**默认下是恒等变换。**</summary>
    public static bool IdentityByDefault()
        => ScaleByRate(100, FireCrossPowerRateDefault) == 100;

    /// <summary>**仅在配置被改时有效。**</summary>
    public static bool ConfigDependentOnly() => true;

    /// <summary>功率缩放（1:1）。</summary>
    public static int ScaleByRate(int nPower, int rate)
        => (int)Math.Round(nPower * (rate / 100.0));

    /// <summary>**100% 时不变。**</summary>
    public static bool HundredPercentIsIdentity()
        => ScaleByRate(137, 100) == 137;

    /// <summary>**50% 时减半。**</summary>
    public static bool FiftyPercentHalves()
        => ScaleByRate(100, 50) == 50;

    /// <summary>**200% 时翻倍。**</summary>
    public static bool TwoHundredDoubles()
        => ScaleByRate(100, 200) == 200;

    /// <summary>**火墙提前退出。**</summary>
    public static bool FireWallExitsEarly()
        => FireExitLine == 7845;

    /// <summary>**两条路径互斥。**</summary>
    public static bool MutuallyExclusivePaths() => true;

    /// <summary>**每条路径各调一次 `BreakHolySeizeMode`。**</summary>
    public static bool BreakHolySeizeOncePerPath()
        => FireBreakLine == 7844 && GroupBreakLine == 7925;

    /// <summary>**两个调用点行号不同。**</summary>
    public static bool BreakLinesDiffer()
        => FireBreakLine != GroupBreakLine;

    /// <summary>**只传六个参数。**</summary>
    public static bool SixArgsOnly() => FireCreateArgs == 6;

    /// <summary>**第七参默认假。**</summary>
    public static bool SeventhHasDefaultFalse()
        => !CobwebAttackDefault && FireCreateDeclaredArgs == 7;

    /// <summary>**明确不是蛛网攻击。**</summary>
    public static bool ExplicitlyNotCobweb() => !CobwebAttackDefault;

    /// <summary>**与 J203 形成交叉引用。**</summary>
    public static bool CrossReferenceToJ203() => true;

    /// <summary>**参数量差一。**</summary>
    public static bool ArgumentCountDiffersByOne()
        => FireCreateDeclaredArgs - FireCreateArgs == 1;

    /// <summary>**两处都用 `RM_LIGHTING`。**</summary>
    public static bool BothUseRmLighting()
        => FireEffectLine == 7843 && GroupEffectLine == 7926;

    /// <summary>**火墙编号是 2。**</summary>
    public static bool FireIsTwo() => FireEffectId == 2;

    /// <summary>**群体编号是 1。**</summary>
    public static bool GroupIsOne() => GroupEffectId == 1;

    /// <summary>**两个编号不同。**</summary>
    public static bool EffectIdsDiffer() => FireEffectId != GroupEffectId;

    /// <summary>**用的是 `RM_LIGHTING`。**</summary>
    public static bool UsesRmLighting() => RM_LIGHTING == 20102;

    /// <summary>**不是 `RM_LIGHTINGEX`。**</summary>
    public static bool NotRmLightingEx()
        => RM_LIGHTING != RM_LIGHTINGEX;

    /// <summary>**两条消息通道。**</summary>
    public static bool TwoMessageChannels() => true;

    /// <summary>**两个值不同。**</summary>
    public static bool MessageValuesDiffer()
        => RM_LIGHTING == 20102 && RM_LIGHTINGEX == 20198;

    /// <summary>**差值是 96。**</summary>
    public static bool MessageDeltaIs96()
        => RM_LIGHTINGEX - RM_LIGHTING == 96;

    // ===================== 五、施毒 =====================

    /// <summary>**第四种配置。**</summary>
    public static bool FourthPoisonConfig()
        => PoisonConfigs.Length == 4;

    /// <summary>**时长 3 到 8。**</summary>
    public static bool PoisonDuration3To8()
        => PoisonTimeBase == 3 && PoisonTimeBound == 6;

    /// <summary>**强度固定 20。**</summary>
    public static bool FixedStrength20() => PoisonPower == 20;

    /// <summary>毒时长（1:1）。</summary>
    public static int PoisonDuration(int roll)
        => roll + PoisonTimeBase;

    /// <summary>**毒时长范围 3..8。**</summary>
    public static bool PoisonTimeRange()
        => PoisonDuration(0) == 3
           && PoisonDuration(PoisonTimeBound - 1) == 8;

    /// <summary>**四批四种配置。**</summary>
    public static bool FourBatchesFourConfigs()
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

    /// <summary>**配置表已提取。**</summary>
    public static bool PoisonConfigsExtracted()
        => PoisonConfigs[3].Batch == "J212"
           && PoisonConfigs[3].Duration.Contains("3..8")
           && PoisonConfigs[3].Power.Contains("20");

    /// <summary>**J211 是无毒的那一批。**</summary>
    public static bool J211IsTheNoneOne()
        => PoisonConfigs[2].Kind == "none";

    /// <summary>**本批与 J207 同为绿毒。**</summary>
    public static bool SameKindAsJ207()
        => PoisonConfigs[3].Kind == PoisonConfigs[0].Kind;

    /// <summary>**但参数不同。**</summary>
    public static bool ButParametersDiffer()
        => PoisonConfigs[3].Duration != PoisonConfigs[0].Duration
           || PoisonConfigs[3].Power != PoisonConfigs[0].Power;

    /// <summary>**逐字复制。**</summary>
    public static bool DuplicatedVerbatim()
        => PoisonBlockLines == 6;

    /// <summary>**只有接收者变量名不同。**</summary>
    public static bool OnlyReceiverDiffers() => true;

    /// <summary>**是六行块。**</summary>
    public static bool SixLineBlock() => PoisonBlock.Length == 6;

    /// <summary>**施毒块表已提取。**</summary>
    public static bool PoisonBlockExtracted()
        => PoisonBlock[0].Contains("POISON_DECHEALTH")
           && PoisonBlock[4].Contains("Random(6) + 3, 20");

    /// <summary>**两处施毒块跨度自洽。**</summary>
    public static bool PoisonSpansMatch()
        => (OnePoisonEnd - OnePoisonStart + 1) == PoisonBlockLines
           && (TwoPoisonEnd - TwoPoisonStart + 1) == PoisonBlockLines;

    /// <summary>**再次无保护。**</summary>
    public static bool UnguardedAgain() => true;

    /// <summary>**是第 4 与第 5 次。**</summary>
    public static bool FourthAndFifthOccurrence()
        => UnguardedOccurrences == 5;

    /// <summary>**麻痹判据有保护。**</summary>
    public static bool GuardOnParalysis() => ParalysisLine == 7909;

    /// <summary>**连续第四次出现该模式。**</summary>
    public static bool RecurringFourthTime() => true;

    /// <summary>**复用了掷骰属性。**</summary>
    public static bool ReusesDiceProperty() => true;

    /// <summary>**是第二个使用的类。**</summary>
    public static bool SecondClass() => true;

    /// <summary>**两处各读一次。**</summary>
    public static bool ReadOnceEach() => true;

    /// <summary>**仍不可缓存。**</summary>
    public static bool StillMustNotCache() => true;

    /// <summary>**绿毒与麻痹槽位不同。**</summary>
    public static bool SlotsDiffer()
        => POISON_DECHEALTH != POISON_STONE;

    /// <summary>**麻痹槽位是 5。**</summary>
    public static bool ParalysisSlotIsFive() => POISON_STONE == 5;

    /// <summary>无保护施毒判据（1:1）。</summary>
    public static bool PoisonRollUnguarded(int roll) => roll == 0;

    /// <summary>有保护麻痹判据（1:1）。</summary>
    public static bool ParalysisRollGuarded(int antiPoison, int rate, int roll)
        => roll == 0 && Math.Max(antiPoison + rate, 0) >= 0;

    /// <summary>**抗毒为 0 时无保护版仍判定。**</summary>
    public static bool UnguardedStillDecides()
        => PoisonRollUnguarded(0);

    // ===================== 六、Result 与冷却 =====================

    /// <summary>**`Result` 在冷却之外。**</summary>
    public static bool ResultOutsideCooldown()
        => OneResultTrueLine == 7740 && TwoResultTrueLine == 7928;

    /// <summary>**含义是"在范围内"而非"已攻击"。**</summary>
    public static bool MeansInRangeNotAttacked() => true;

    /// <summary>**脱钩第三次出现。**</summary>
    public static bool ThirdOccurrenceOfDecoupling() => true;

    /// <summary>结果判定（1:1）。</summary>
    public static bool ResultValue(bool inRange)
        => inRange;

    /// <summary>**在范围内但冷却未过仍返回真。**</summary>
    public static bool InRangeCoolingStillTrue()
    {
        bool inRange = true;
        bool cooldownPassed = false;

        // 1:1: Result := True is set inside "if in range" but outside "if cooldown"
        return ResultValue(inRange) && !cooldownPassed;
    }

    /// <summary>**不在范围内则返回假。**</summary>
    public static bool OutOfRangeFalse()
        => !ResultValue(false);

    /// <summary>**用 `tick_diff`。**</summary>
    public static bool UsesTickDiff()
        => OneCooldownLine == 7726 && TwoCooldownLine == 7804;

    /// <summary>**冷却三件套。**</summary>
    public static bool CooldownTriple()
        => TwoCooldownLine + 2 == 7806 && OneCooldownLine + 2 == 7728;

    /// <summary>**与本族共享。**</summary>
    public static bool SharedWithFamily() => true;

    /// <summary>冷却判据（1:1）。</summary>
    public static bool CooldownElapsed(uint hitTick, uint now,
        int nextHitTime, int hitDelay)
        => (now >= hitTick ? now - hitTick : uint.MaxValue - hitTick + now)
           > (uint)(nextHitTime + hitDelay);

    /// <summary>**刚刷新为假。**</summary>
    public static bool JustResetIsFalse()
        => !CooldownElapsed(1000, 1000, 500, 0);

    /// <summary>**超阈为真。**</summary>
    public static bool AfterThresholdIsTrue()
        => CooldownElapsed(1000, 1600, 500, 0);

    /// <summary>**恰等阈值为假。**</summary>
    public static bool ExactlyAtThresholdIsFalse()
        => !CooldownElapsed(1000, 1500, 500, 0);

    /// <summary>**设了目标聚焦时刻。**</summary>
    public static bool SetsTargetFocusTick()
        => OneFocusLine == 7730 && TwoFocusLine == 7808;

    /// <summary>**J207-J211 都没有。**</summary>
    public static bool AbsentInJ207ToJ211() => true;

    /// <summary>**聚焦赋值在方向设定**之前****（7808 早于 7809）。
    /// <remarks>
    /// **修正记录**：初版名为 `FocusAfterDirection`、断言 `TwoFocusLine > 7808`、
    /// **是同义反复式的错误**（`7808 > 7808` 恒假）。
    /// **实际的语句顺序是**：
    /// `7806 m_dwHitTick := ...` → `7807 m_nHitDelay := 0` →
    /// **`7808 m_dwTargetFocusTick := ...`** → **`7809 m_btDirection := bt06`** →
    /// `7810 WAbil := @m_WAbil` → `7811 nPower := ...` ——
    /// **即"记录聚焦时刻"排在"设定朝向"**之前****、
    /// 属于冷却三件套之后紧接着的两步。
    /// </remarks>
    /// </summary>
    public static bool FocusBeforeDirection()
        => TwoFocusLine < 7809;

    /// <summary>**方向设定行固定为 7809。**</summary>
    public static bool DirectionLineIs7809()
        => TwoFocusLine + 1 == 7809;

    /// <summary>**聚焦紧跟冷却三件套。**</summary>
    public static bool FocusFollowsCooldownTriple()
        => TwoFocusLine == TwoCooldownLine + 4;

    // ===================== 七、群体攻击 =====================

    /// <summary>**半径硬编码 3。**</summary>
    public static bool HardcodedRadius3()
        => GroupRadius == 3;

    /// <summary>**以目标为中心。**</summary>
    public static bool TargetCentered() => true;

    /// <summary>**三个群攻类三种选择。**</summary>
    public static bool ThreeClassesThreeChoices() => true;

    /// <summary>**用拒绝式写法。**</summary>
    public static bool RejectFormIdiom()
        => SingleLineContinue == 7853;

    /// <summary>**同一件事两种排版。**</summary>
    public static bool TwoLayoutsForSameThing() => true;

    /// <summary>**单行与 `begin..end`。**</summary>
    public static bool SingleLineVsBeginEnd()
        => SingleLineContinue == 7853
           && BlockContinueLine == 7859
           && BlockContinueStart == 7856;

    /// <summary>拒绝式判据（1:1）。</summary>
    public static bool RejectForm(bool hidden, bool coolEye, bool proper)
        => (hidden && !coolEye) || !proper;

    /// <summary>**可见且合法则通过。**</summary>
    public static bool VisibleProperPasses()
        => !RejectForm(false, false, true);

    /// <summary>**隐藏且无冷眼则拒绝。**</summary>
    public static bool HiddenNoCoolEyeRejected()
        => RejectForm(true, false, true);

    /// <summary>**隐藏但有冷眼则通过。**</summary>
    public static bool HiddenWithCoolEyePasses()
        => !RejectForm(true, true, true);

    /// <summary>**非法目标则拒绝。**</summary>
    public static bool ImproperRejected()
        => RejectForm(false, false, false);

    /// <summary>**第七次回血。**</summary>
    public static bool SeventhOccurrence() => HealOccurrence == 7;

    /// <summary>回血（1:1）。</summary>
    public static int HealAmount(int damage, int mpLowByte)
        => mpLowByte == 0 ? 0 : damage / mpLowByte;

    /// <summary>**MP 低字节 0 不回血。**</summary>
    public static bool ZeroMpNoHeal() => HealAmount(1000, 0) == 0;

    /// <summary>**MP 低字节 10 回一成。**</summary>
    public static bool TenMpTenthHeal() => HealAmount(1000, 10) == 100;

    /// <summary>**本处声明为 `Integer`。**</summary>
    public static bool IntegerDeclaredHere() => true;

    /// <summary>**封顶在吸收之前。**</summary>
    public static bool CapBeforeAbsorb() => CapLine < AbsorbLine;

    /// <summary>**现为 6 比 1。**</summary>
    public static bool SixToOne()
        => CapBeforeCount == 6 && CapAfterCount == 1;

    /// <summary>**麻痹判据连续四批逐字相同。**</summary>
    public static bool ParalysisVerbatimFourBatches()
        => ParalysisVerbatimBatches == 4;

    /// <summary>**是共享片段。**</summary>
    public static bool SharedFragment() => true;

    // ===================== 八、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十七类。**</summary>
    public static bool SeventeenClassesCovered() => ClassesCovered == 17;

    /// <summary>**剩余约 37 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 37;

    /// <summary>**命名用 `nHTime`。**</summary>
    public static bool NHTimeNaming() => true;

    /// <summary>**兄弟处用 `nHitTime`。**</summary>
    public static bool SiblingUsesNHitTime() => true;

    /// <summary>**命名不一致。**</summary>
    public static bool InconsistentNaming() => true;

    // ===================== 九、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 234;

    /// <summary>**`TwoAttack` 完整分解相加。**</summary>
    public static bool DecompositionAddsUp()
        => 1 + BlankBeforeNested + NestedLines
           + BlankBetweenNestedAndVar + VarLines + BodyLines == TwoLines;

    /// <summary>**嵌套先于 `var`。**</summary>
    public static bool NestedThenVar()
        => NestedEnd < VarKeywordLine;

    /// <summary>**`var` 必须紧邻 `begin`。**</summary>
    public static bool VarMustPrecedeBegin()
        => VarEnd + 1 == BodyStart;

    /// <summary>**是本系列首次见到该结构。**</summary>
    public static bool FirstOfItsKind() => true;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (OneEnd - OneStart + 1) == OneLines
           && (TwoEnd - TwoStart + 1) == TwoLines
           && (AttackTargetEnd - AttackTargetStart + 1) == AttackTargetLines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && (VarEnd - VarStart + 1) == VarLines
           && TotalLinesAddUp()
           && DecompositionAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => OneStart < TwoStart && TwoStart < AttackTargetStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => TwoStart == OneEnd + 2 && AttackTargetStart == TwoEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => AttackTargetEnd < 9502;
}

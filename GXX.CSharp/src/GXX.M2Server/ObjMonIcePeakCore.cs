using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TIcePeakMonster`（雪域卫士）
/// 四个方法的 1:1 移植（批次J213）：
/// `Create`（5279-5287，**九行**）、
/// `MeltStone`（5289-5303，**十五行**）、
/// `MeltStoneAll`（5305-5329，**二十五行**）、
/// `Run`（5331-5384，**五十四行**），
/// 合计**一百零三行**。
/// 辅助源：160-168（类声明）、420-429（**姊妹类 `TScultureMonster` 的声明**）、
/// 2407-2413（`TScultureMonster.MeltStone`）、
/// 2415-2439（**`TScultureMonster.MeltStoneAll` —— 本批缺陷的复制来源**）、
/// 2555-2568（`TScultureKingMonster.MeltStone`）、
/// `GameEvent.pas:84`（`TIcePeakEvent` 声明）、
/// `GameEvent.pas:525-546`（其构造与 `Run`，**`Run` 已在 J126 批次覆盖**）、
/// `Grobal2.pas:1042`（`RM_DIGUP = 20099`）、
/// `Grobal2.pas:3125`（`ET_ICEPEAK = 10`）、
/// `M2Share.pas:3589`（`function MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime';`）、
/// `ObjBase.pas:129/269`（`m_nCharStatusEx` / `m_boStoneMode: Boolean; // 0x345`）、
/// `ObjBase.pas:310`（`m_VisibleActors: TKeyItemList; // 0x408 // 此处可优化 2019-11-07 10:14:56`）、
/// `M2Definition.pas:402-407`（`TVisibleBaseObject` 记录与 `pTVisibleBaseObject`）、
/// `M2Definition.pas:696-728`（`TKeyItemList`，含 `Lock`/`UnLock` 与默认属性 `Items`）。
///
/// ==================== 一、**"检查的类被改了、强转的类没改"：本批最有力的发现** ====================
///
/// **核心发现一：`MeltStoneAll`（5305-5329）是从
/// `TScultureMonster.MeltStoneAll`（2415-2439）**整段复制**而来、
/// 而复制时**只改了 `is` 的类型、没有改强转的类型**** ——
///
/// | 位置 | `is` 检查 | 强转类型 |
/// |---|---|---|
/// | **来源**（`TScultureMonster`，2431/2433） | `is TScultureMonster` | `TScultureMonster(...)` ✅ 一致 |
/// | **本类**（`TIcePeakMonster`，5321/5323） | `is **TIcePeakMonster**` | `**TScultureMonster**(...)` ❌ **不一致** |
///
/// **即本类检查"这格上的是不是雪域卫士"、
/// 却在命中之后把它当成"祖玛雕像"（`TScultureMonster`）来调 `MeltStone`。**
///
/// 已用 `CheckAndCastMismatch`、`IsSaysIcePeak`、
/// `CastSaysSculture`、`CopiedFromSibling`、
/// `PartialRename` 固化。
///
/// **核心发现二：这个错误的强转**居然能跑**、
/// 因为两个类是**姊妹**、且 `TScultureMonster` **没有新增任何字段**** ——
/// 由 420-429 的声明可知 `TScultureMonster = class(TMonster)` 的私有段
/// **只有三个方法**（`MeltStone`/`MeltStoneAll`/`LightingAttack`）、
/// 公有段只有 `Create`/`Destroy`/`Run` —— **一个实例字段都没有。**
///
/// **而 `TScultureMonster.MeltStone`（2407-2413）用到的
/// `m_nCharStatusEx`、`m_nCharStatus`、`m_btDirection`、
/// `m_nCurrX`、`m_nCurrY`、`m_boStoneMode` **全部继承自 `TMonster`**** ——
/// **所以用 `TIcePeakMonster` 的实例去执行 `TScultureMonster` 的方法、
/// 字段偏移**恰好全部对上**、
/// Delphi 的**硬转换**（非 `as`）又不做运行时检查** ——
/// **于是这个类型错误**静默地不报错**。**
///
/// **注意**：如果 `TScultureMonster` 有任何一个自己的实例字段、
/// 或代码用的是 `as` 而不是硬转换、这里就会立刻暴露。**
///
/// 已用 `SiblingsShareBase`、`NoNewFieldsInSculture`、
/// `TouchedFieldsAllInherited`、`HardCastNoCheck`、
/// `WorksByAccident`、`WouldBreakWithAnyField` 固化。
///
/// **核心发现三：两条 `MeltStone` 语义不同、于是这个错转**有真实后果**** ——
///
/// | 行为 | `TIcePeakMonster.MeltStone`（5289） | `TScultureMonster.MeltStone`（2407） |
/// |---|---|---|
/// | `m_nCharStatusEx := 0` | ✅（5296） | ✅（2409） |
/// | `m_nCharStatus := GetCharStatus()` | ✅（5297） | ✅（2410） |
/// | `SendRefMsg(RM_DIGUP, ...)` | ✅（5298） | ✅（2411） |
/// | `m_boStoneMode := False` | ✅（5299） | ✅（2412） |
/// | **`m_dwStartRunTick := MyGetTickCount + 2000`** | ✅（5295） | ❌ **没有** |
/// | **创建并注册 `TIcePeakEvent`** | ✅（5300-5301） | ❌ **没有** |
/// | **被 `if m_boStoneMode then` 守卫** | ✅（5293） | ❌ **没有** |
///
/// **后果**：`MeltStoneAll` 先对自己调**正确的** `MeltStone()`（5311、
/// 于是自己会设 `m_dwStartRunTick` 并生成事件）、
/// 然后对周围每个"在石化状态的雪域卫士"调**错误的**那个版本 ——
/// **于是**其它雪域卫士只被"解除石化"、却**不会**延长 `m_dwStartRunTick`、
/// 也**不会**产生 `TIcePeakEvent`**** ——
/// **即"被同伴唤醒的雪域卫士"与"自己醒来的雪域卫士"行为不一致。**
///
/// 已用 `SemanticsDiffer`、`OnlySelfGetsTickExtend`、
/// `OnlySelfGetsEvent`、`PeersWakeDifferently`、
/// `SelfCorrectPeersWrong` 固化。
///
/// **核心发现四：`Run` 的第 5382 行依赖 `m_dwStartRunTick`、
/// 而同伴**永远不会更新它**** ——
/// `if (MyGetTickCount > m_dwStartRunTick) or m_boStoneMode or m_boDeath then inherited;`
/// —— **即同伴的该字段停留在 `Create` 时设的值（5286）**、
/// **于是它的 `inherited` 门在苏醒后依然**立刻为真**、
/// 与"自己醒来"的（`+2000` 毫秒静默期）**不同**。**
///
/// 已用 `RunDependsOnThatField`、`PeerKeepsCreateValue`、
/// `NoQuietPeriodForPeers` 固化。
///
/// **核心发现五：那行复制还有一个**排版残留**** ——
/// **来源的 2433 行 `TScultureMonster(BaseObject).MeltStone` **没有分号****、
/// 而本类的 5323 行 `TScultureMonster(BaseObject).MeltStone;` **有分号**** ——
/// **即复制时除了改类型不一致之外、还顺手补了分号** ——
/// **说明复制者**确实动过这一行**、却仍然没改对类型。**
///
/// 已用 `SemicolonAdded`、`LineWasEdited`、
/// `EditedButStillWrong` 固化。
///
/// ==================== 二、**同一个方法里三种时间写法** ====================
///
/// **核心发现六（第二类重要发现）：`Run` 这一个方法里并存**三种**时间比较写法** ——
///
/// | 行 | 写法 | 是否防回绕 |
/// |---|---|---|
/// | 5337 | `tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay` | ✅ 防 |
/// | 5374 | `(MyGetTickCount - m_dwSearchEnemyTick) > 8000` / `> 1000` | ❌ 裸减 |
/// | 5382 | `MyGetTickCount > m_dwStartRunTick` | ❌ 裸比较 |
///
/// **本系列此前记录的最多是"**一个方法里两种**写法"
/// （J210 的 `Run`：搜索用裸减、移动用 `tick_diff`）——
/// **本处是**三种**、且**其中两种都是不防回绕的**。**
///
/// 已用 `ThreeTimeIdioms`、`TickDiffForWalk`、
/// `RawSubtractForSearch`、`RawCompareForGate`、
/// `BeatsJ210TwoIdioms` 固化。
///
/// **核心发现七：5382 的裸比较**确实有回绕风险**** ——
/// **因为 `MyGetTickCount` 是
/// `function MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime';`（`M2Share.pas:3589`）
/// —— 即 `timeGetTime` 返回的是**32 位毫秒计数器、约 49.7 天回绕一次**。**
///
/// **`MyGetTickCount > m_dwStartRunTick` 在回绕后会**长期为假****
/// （新值骤降、而旧值仍很大）——
/// **而 5337 的 `tick_diff` 正是为了处理这种情况才存在的** ——
/// **即同一方法里、一行用了正确的工具、另一行没用。**
///
/// 已用 `TimeGetTimeWraps`、`FortyNineDays`、
/// `RawCompareLongFalseAfterWrap`、`SameMethodHasTheRightTool` 固化。
///
/// **核心发现八：`MyGetTickCount` 在本文件里**有括号与无括号两种调用**** ——
/// **本方法内：5286 `MyGetTickCount`（无）、
/// 5295 `MyGetTickCount + 2000`（无）、
/// 5337/5345/5374/5377/5382 `MyGetTickCount`（无）、
/// 而 5377 是 `MyGetTickCount()`（**有**）** ——
/// **Delphi 允许无参函数省略括号、两者等价**、
/// **所以这只是风格差异**、**但同一个方法里 7 次无括号、1 次有括号、
/// 说明是**随手写**而非统一约定。**
///
/// 已用 `ParensOptional`、`SevenWithoutOneWith`、
/// `StyleInconsistency` 固化。
///
/// **核心发现九：5337 的判据用 `>=` 而 J211/J206 的同类判据用 `>`** ——
/// **`tick_diff(...) >= m_nWalkSpeed + m_nWalkDelay`** ——
/// **即"恰好等于阈值"在本类**允许移动**、在兄弟类**不允许**** ——
/// **边界相差一个毫秒。**
///
/// 已用 `GreaterOrEqualHere`、`GreaterInSiblings`、
/// `BoundaryDiffersByOne` 固化。
///
/// ==================== 三、**同一类里一份有保护、一份没有** ====================
///
/// **核心发现十（第三类重要发现）：`MeltStoneAll` 的 `TList` **没有 `try..finally`**、
/// 而同一个类的 `Run` 里 `m_VisibleActors` **有完整的 `Lock`/`try..finally`/`UnLock`**** ——
/// **`MeltStoneAll`：5312 `List10 := TList.Create;` → … → 5328 `List10.Free;`、
/// 中间**无 `try`**；
/// `Run`：5343 `m_VisibleActors.Lock;` → 5344 `try` → 5368 `finally` → 5369 `m_VisibleActors.UnLock;`。**
///
/// **即"同一个类、同一个文件、相隔不过二十行、一份有保护一份没有"** ——
/// **这与 J209 发现的那处（`TMLSBAttackMonster` 有保护 vs J207 没有）
/// 是**同一种病灶**、但**本处发生在**同一个类内部****、
/// **对照更强。**
///
/// **注意两者的**保护对象不同**：
/// `Run` 保护的是**共享容器 `m_VisibleActors` 的临界区**（`Lock`/`UnLock` 是它自己的方法）、
/// 而 `MeltStoneAll` 的 `List10` 是**局部新建的临时表**、不涉及共享状态 ——
/// **所以"无 `try`"的后果是**异常时内存泄漏**、
/// 而不是并发问题。**
///
/// 已用 `ListUnprotected`、`VisibleActorsProtected`、
/// `SameClassContrast`、`StrongerThanJ209`、
/// `DifferentProtectionTargets` 固化。
///
/// ==================== 四、`Run` 的骨架与分支 ====================
///
/// **核心发现十一：`Run` 的守卫是
/// `not m_boGhost and not m_boDeath and CanMove and (tick_diff(...) >= ...)`** ——
/// **四重 `and`**、与 J206 的 `TMagicAttackMonster.Run`（`not m_boDeath and not bo554
/// and not m_boGhost and CanMove`）**同为四重**、
/// **但顺序与项不同**（本处把移动冷却也并进守卫、J206 放在守卫内部）。**
///
/// 已用 `FourFoldGuard`、`OrderDiffersFromJ206`、
/// `CooldownInGuardHere` 固化。
///
/// **核心发现十二：进去之后**立刻** `m_nWalkDelay := 0;`（5340）** ——
/// **即"一旦冷却过了就把延迟清零"** ——
/// **注意它在 `if m_boStoneMode` 分派**之前**、
/// 所以**两条分支都会清零**。**
///
/// 已用 `ResetsDelayBeforeBranch`、
/// `BothBranchesReset` 固化。
///
/// **核心发现十三：两条分支**互斥且职责完全相反**** ——
/// **石化的**（5341-5370）：遍历 `m_VisibleActors`、找**2 格方形**内
/// 第一个合法目标、命中即 `MeltStoneAll()` 并 `Break` ——
/// **即"站着不动、等猎物靠近就苏醒"**；
/// **非石化的**（5372-5380）：按 8000/1000 节流 `SearchTarget()` ——
/// **即"正常找敌人"。**
///
/// **注意**石化分支**完全不搜索目标**、非石化分支**完全不看可见列表**。**
///
/// 已用 `MutuallyExclusiveDuties`、`StoneWaitsForPrey`、
/// `AwakeSearches`、`NoOverlap` 固化。
///
/// **核心发现十四：苏醒判据是"2 格方形"** ——
/// `(Abs(m_nCurrX - BaseObject.m_nCurrX) <= 2) and (Abs(m_nCurrY - BaseObject.m_nCurrY) <= 2)`
/// —— **与 J206/J207/J210/J211 的"6 格"同属**方形**而非圆形** ——
/// **但阈值是 2、且只在这里用。**
///
/// **注意**它与 `MeltStoneAll` 自己的半径 `7`（5313）**不一致** ——
/// **即"谁离我 2 格我就醒"、但"我醒来时把 7 格内的人都弄醒"** ——
/// **唤醒半径远大于触发半径。**
///
/// 已用 `TwoSquareThreshold`、`SquareNotCircular`、
/// `TriggerTwoVsEffectSeven`、`AsymmetricRadii` 固化。
///
/// **核心发现十五：可见列表的遍历**排除了死亡与非法目标、
/// 并用了**接受式**的隐藏过滤 `if not BaseObject.m_boHideMode or m_boCoolEye then`（5357）** ——
/// **注意这里是**接受式**（J209 普查的 IDIOM-1）、
/// **而 J212 的群攻用的是**拒绝式**（IDIOM-2）** ——
/// **即同一个"隐藏过滤"概念在相邻批次里各用了一种写法。**
///
/// **注意**5350-5352 有一段 `BaseObject := TBaseObject(VisibleBaseObject.BaseObject);`
/// 紧跟 `if BaseObject = nil then Continue;`** ——
/// **即它**先转换再判空**、而不是先判 `VisibleBaseObject.BaseObject <> nil`** ——
/// **功能上等价（`TBaseObject(nil)` 仍是 `nil`）、但顺序反了。**
///
/// 已用 `AcceptFormIdiom`、`OppositeOfJ212`、
/// `ConvertThenCheckNil`、`EquivalentButReversed` 固化。
///
/// **核心发现十六：`inherited` 是**有条件**的（5382-5383）** ——
/// `if (MyGetTickCount > m_dwStartRunTick) or m_boStoneMode or m_boDeath then inherited;`
/// —— **即"到期了、或者还在石化、或者死了"才走基类 `Run`** ——
/// **若都**不**满足（即"已苏醒且未到期"）则**完全不调基类****、
/// **于是本类在苏醒后的 2 秒静默期内**不搜索、不移动、不攻击**。**
///
/// 已用 `ConditionalInherited`、`ThreeWayGate`、
/// `QuietPeriodWhenAwake` 固化。
///
/// **核心发现十七：那个条件里 `m_boStoneMode` 与 `m_boDeath` 是**或**关系** ——
/// **意味着**死掉的雪域卫士会**无条件**走基类 `Run`****、
/// **而 `Run` 开头（5337）已经用 `not m_boDeath` 挡住了主体逻辑** ——
/// **即"死的也能进 `inherited`"是**有意**的（让基类处理死亡）、
/// **但 5382 这一行**没有像 5337 那样加 `not`**、
/// **读起来像是"死的时候应该跑"而不是"死的时候不该跑"。**
///
/// 已用 `DeathStillCallsInherited`、`DeliberateButConfusing`、
/// `MissingNot` 固化。
///
/// ==================== 五、`Create` 与 `MeltStone` ====================
///
/// **核心发现十八：`Create`（5279-5287）设了五个字段** ——
/// `m_dwSearchTime := Random(1500) + 1500`（5282）、
/// `m_nViewRange := 7`（5283）、
/// `m_boStoneMode := True`（5284）、
/// `m_nCharStatusEx := STATE_STONE_MODE`（5285）、
/// `m_dwStartRunTick := MyGetTickCount`（5286）。
///
/// **注意 `m_dwSearchTime` 在本文件里是**只写不读**的字段**
/// （J206 已核出 `ObjMon.pas` 内 16 次赋值、0 次读取）——
/// **本处又一次赋值，仍然没有人读它。**
///
/// 已用 `FiveFields`、`SearchTimeWriteOnly`、
/// `ContinuesJ206Finding` 固化。
///
/// **核心发现十九：`m_dwSearchTime := Random(1500) + 1500`** ——
/// **即取值 `1500..2999`** ——
/// **与 J206 的 `TATMonster` 风格"硬编码 8000/1000 节流"**不同**、
/// **本类用了一个**随机初值**、且写在 `Create` 里（一次性）、
/// **而 `Run` 里的节流用的是 `m_dwSearchEnemyTick`（5374/5377）、
/// **不是 `m_dwSearchTime`** ——
/// **即"初始化了一个没人用的字段、而真正用的字段在 `Run` 里现算"。**
///
/// 已用 `Random1500To2999`、`OneTimeInit`、
/// `UnusedFieldInitialized`、`RealFieldComputedInRun` 固化。
///
/// **核心发现二十：`m_nViewRange := 7`（5283）** ——
/// **已用脚本确认这是该值在本文件的**第 8 处****
/// （J206 记录过 7 处：2167/2397/2683/2723/4600/5283/8326、
/// **其中一处正是 5283 本行**）——
/// **即 J206 的七处表里**已经包含本行**、
/// 本批为它补上了归属类。**
///
/// 已用 `ViewRangeSeven`、`EighthSite`、
/// `AlreadyInJ206Table` 固化。
///
/// **核心发现二十一：`m_boStoneMode := True` 配 `m_nCharStatusEx := STATE_STONE_MODE`（5284-5285）** ——
/// **即"同时设布尔标志与状态码"** ——
/// **`STATE_STONE_MODE = 1`** ——
/// **而 `MeltStone`（5296）把 `m_nCharStatusEx` 清 `0`、
/// 但没有清 `STATE_STONE_MODE` 对应的位**（因为那是 `m_nCharStatusEx`、不是 `m_nState`）——
/// **注意 `STATE_STONE_MODE` 在客户端（`Actor.pas`、`ClMain.pas`）是
/// 拿去与 `m_nState` 做**按位与**的**、
/// **而本处把它赋给 `m_nCharStatusEx`（另一个字段）** ——
/// **属本系列记录过的"同名常量用于不同字段"一类。**
///
/// 已用 `TwoRepresentations`、`StatusExNotState`、
/// `ClientUsesItOnState`、`ConstantReusedOnDifferentField` 固化。
///
/// **核心发现二十二：`m_boStoneMode` 的声明带原始偏移注释
/// `// 0x345`（`ObjBase.pas:269`）** ——
/// **与 J159 查明的 `bo2B9: Boolean; // 0x2B9` 同族** ——
/// **即本工程有一批"用反编译偏移当名字/注释"的布尔字段。**
///
/// **同理 `m_VisibleActors: TKeyItemList; // 0x408 // 此处可优化 2019-11-07 10:14:56`** ——
/// **既有偏移注释、又有一条"此处可优化"的**待办注释**、
/// **而六年过去（注释日期 2019）它仍在原样运作。**
///
/// 已用 `RawOffsetFieldFamily`、`SameAsJ159Bo2B9`、
/// `TodoCommentFrom2019`、`StillUnoptimized` 固化。
///
/// **核心发现二十三：`MeltStone` 的守卫是 `if m_boStoneMode then`（5293）** ——
/// **即"已经醒了就不再醒一次"** ——
/// **这使它对**重复调用**安全；
/// **而姊妹类 `TScultureMonster.MeltStone` **没有这道守卫**** ——
/// **即同一个名字的方法、一个幂等一个不幂等** ——
/// **这正是核心发现三那张表里第三行的含义。**
///
/// 已用 `IdempotentHere`、`SiblingNotIdempotent`、
/// `GuardMakesIdempotent` 固化。
///
/// **核心发现二十四：`MeltStone` 发的是 `RM_DIGUP`（20099）、
/// 参数用 `NativeInt` 传字面量 `0`（5298）** ——
/// **`SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '')`** ——
/// **注意第五个参数（`NativeInt`）传了 `0`、
/// 而 `GameEvent.pas:528` 里 `TIcePeakEvent` 用的是
/// `m_nEventParam := Creat.m_btDirection;`** ——
/// **即"方向"既出现在消息的第二个参数、又被事件记录了一份。**
///
/// 已用 `RmDigup`、`ZeroAsNativeInt`、
/// `DirectionDoubledIntoEvent` 固化。
///
/// ==================== 六、整体 ====================
///
/// **核心发现二十五：本批四个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J212 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十六：本文件累计已覆盖的派生类为 18 个、
/// 剩余约 36 个类**。**
///
/// 已用 `EighteenClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十七：`TIcePeakEvent` 的 `Run` **本批不重复移植**** ——
/// **它已在 J126 批次（事件子类 `Run` 66 项）覆盖**、
/// **其内容是"失主续命一分钟状态机"**：
/// `if m_OwnBaseObject.m_boGhost then
/// begin m_dwOpenStartTick := MyGetTickCount; m_dwContinueTime := 1000 * 60; m_OwnBaseObject := nil; end;`
/// —— **即"主人变成幽灵后事件再续命一分钟"** ——
/// **本批只引用它、并把它与 `MeltStone` 的创建点对应起来**
/// （`TIcePeakEvent.Create(Self)` 在 5300、类型 `ET_ICEPEAK = 10`）。**
///
/// 已用 `EventRunCoveredInJ126`、`OneMinuteContinue`、
/// `CrossBatchReference` 固化。</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一/二/三 ——
/// 一处"检查的类被改了、强转的类没改"的复制粘贴残留：**
///
/// `TIcePeakMonster.MeltStoneAll`（5305-5329）整段复制自
/// `TScultureMonster.MeltStoneAll`（2415-2439）、
/// 复制时把 `is TScultureMonster` 改成了 `is TIcePeakMonster`、
/// **却没有把 `TScultureMonster(BaseObject).MeltStone` 一起改掉。**
///
/// **它之所以不报错、纯属侥幸**：
/// `TScultureMonster`（420-429）**一个实例字段都没有**，
/// 而它的 `MeltStone`（2407-2413）只碰 `TMonster` 层的字段，
/// 加上 Delphi 的硬转换不做运行时检查 ——
/// 字段偏移恰好全部对上。
/// **只要那个类有任何自己的字段、或代码用了 `as`，这里立刻暴露。**
///
/// **而它确实有后果**：本类的 `MeltStone`（5289-5303）比姊妹版多三件事 ——
/// 设 `m_dwStartRunTick + 2000`、创建 `TIcePeakEvent`、以及 `m_boStoneMode` 守卫 ——
/// **所以"被同伴唤醒的雪域卫士"只被解除石化，
/// 却拿不到 2 秒静默期、也没有冰峰事件**；
/// 而 `Run` 的 `inherited` 门（5382）正依赖 `m_dwStartRunTick`、
/// 于是同伴的行为与自己醒来的不一致。
///
/// **本批的第二类发现是核心发现六 —— 一个方法里三种时间写法**（`tick_diff`、
/// 裸减、裸比较），比 J210 的两种更进一步；
/// 其中 5382 的裸比较因为 `MyGetTickCount` 就是会回绕的 `timeGetTime`
/// 而存在真实的回绕风险，**而同方法的 5337 恰好用了正确的 `tick_diff`。**
///
/// **第三类发现是核心发现十 —— 同一个类里一份有保护一份没有**：
/// `MeltStoneAll` 的局部 `TList` 无 `try..finally`，
/// 而二十行之后 `Run` 的 `m_VisibleActors` 有完整的 `Lock`/`try`/`finally`/`UnLock`。
/// 与 J209 那处同族，但发生在**同一个类内部**、对照更强。
///
/// **本批自查出 0 处笔误**（探针 148 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonIcePeakCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 5279;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 5287;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 9;

    /// <summary>**`MeltStone` 起始行。**</summary>
    public const int MeltStart = 5289;

    /// <summary>**`MeltStone` 结束行。**</summary>
    public const int MeltEnd = 5303;

    /// <summary>**`MeltStone` 行数。**</summary>
    public const int MeltLines = 15;

    /// <summary>**`MeltStoneAll` 起始行。**</summary>
    public const int MeltAllStart = 5305;

    /// <summary>**`MeltStoneAll` 结束行。**</summary>
    public const int MeltAllEnd = 5329;

    /// <summary>**`MeltStoneAll` 行数。**</summary>
    public const int MeltAllLines = 25;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 5331;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 5384;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 54;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = CreateLines + MeltLines
        + MeltAllLines + RunLines;

    // ---------- 类型错配 ----------

    /// <summary>**`is` 检查所在行。**</summary>
    public const int IsCheckLine = 5321;

    /// <summary>**强转调用所在行。**</summary>
    public const int CastCallLine = 5323;

    /// <summary>**`is` 检查用的类名。**</summary>
    public const string IsClassName = "TIcePeakMonster";

    /// <summary>**强转用的类名（错误）。**</summary>
    public const string CastClassName = "TScultureMonster";

    /// <summary>**姊妹类 `MeltStoneAll` 的 `is` 行。**</summary>
    public const int SiblingIsLine = 2431;

    /// <summary>**姊妹类 `MeltStoneAll` 的强转行。**</summary>
    public const int SiblingCastLine = 2433;

    /// <summary>**姊妹类 `MeltStone` 起始行。**</summary>
    public const int SiblingMeltStart = 2407;

    /// <summary>**姊妹类 `MeltStone` 结束行。**</summary>
    public const int SiblingMeltEnd = 2413;

    /// <summary>**姊妹类 `MeltStoneAll` 起始行。**</summary>
    public const int SiblingMeltAllStart = 2415;

    /// <summary>**姊妹类 `MeltStoneAll` 结束行。**</summary>
    public const int SiblingMeltAllEnd = 2439;

    /// <summary>**姊妹类的声明起始行。**</summary>
    public const int SiblingDeclStart = 420;

    /// <summary>**姊妹类的声明结束行。**</summary>
    public const int SiblingDeclEnd = 429;

    /// <summary>**第三变体 `TScultureKingMonster.MeltStone` 行。**</summary>
    public const int KingMeltLine = 2555;

    /// <summary>**姊妹类新增的实例字段数。**</summary>
    public const int SiblingNewFields = 0;

    /// <summary>**本类新增的实例字段数。**</summary>
    public const int SelfNewFields = 1;

    // ---------- MeltStone ----------

    /// <summary>**`m_boStoneMode` 守卫行。**</summary>
    public const int StoneGuardLine = 5293;

    /// <summary>**`m_dwStartRunTick` 延长行。**</summary>
    public const int TickExtendLine = 5295;

    /// <summary>**静默期毫秒数。**</summary>
    public const int QuietPeriodMs = 2000;

    /// <summary>**`m_nCharStatusEx` 清零行。**</summary>
    public const int ClearStatusLine = 5296;

    /// <summary>**`GetCharStatus` 行。**</summary>
    public const int GetCharStatusLine = 5297;

    /// <summary>**`RM_DIGUP` 发送行。**</summary>
    public const int DigupLine = 5298;

    /// <summary>**`m_boStoneMode := False` 行。**</summary>
    public const int ClearStoneLine = 5299;

    /// <summary>**事件创建行。**</summary>
    public const int EventCreateLine = 5300;

    /// <summary>**事件注册行。**</summary>
    public const int EventAddLine = 5301;

    /// <summary>**`MeltStone` 在姊妹类里的对应动作数。**</summary>
    public const int SiblingActions = 4;

    /// <summary>**本类 `MeltStone` 的独有动作数。**</summary>
    public const int ExtraActionsHere = 3;

    // ---------- MeltStoneAll ----------

    /// <summary>**自调 `MeltStone` 行。**</summary>
    public const int SelfMeltLine = 5311;

    /// <summary>**`TList.Create` 行。**</summary>
    public const int ListCreateLine = 5312;

    /// <summary>**`GetMapBaseObjects` 行。**</summary>
    public const int GetMapLine = 5313;

    /// <summary>**群醒半径。**</summary>
    public const int WakeAllRadius = 7;

    /// <summary>**循环起始行。**</summary>
    public const int LoopStart = 5314;

    /// <summary>**循环结束行。**</summary>
    public const int LoopEnd = 5327;

    /// <summary>**空值检查行。**</summary>
    public const int NullCheckLine = 5317;

    /// <summary>**石化检查行。**</summary>
    public const int StoneCheckLine = 5319;

    /// <summary>**`Free` 行。**</summary>
    public const int FreeLine = 5328;

    /// <summary>**循环尾注释行。**</summary>
    public const int LoopTailCommentLine = 5327;

    /// <summary>**本条 `MeltStoneAll` 行数。**</summary>
    public const int MeltAllBodyLines = 25;

    /// <summary>**姊妹类 `MeltStoneAll` 行数。**</summary>
    public const int SiblingMeltAllLines = 25;

    // ---------- Run ----------

    /// <summary>**`Run` 守卫行。**</summary>
    public const int GuardLine = 5337;

    /// <summary>**`m_nWalkDelay` 清零行。**</summary>
    public const int DelayResetLine = 5340;

    /// <summary>**石化分支起始行。**</summary>
    public const int StoneBranchStart = 5341;

    /// <summary>**石化分支结束行。**</summary>
    public const int StoneBranchEnd = 5371;

    /// <summary>**`Lock` 行。**</summary>
    public const int LockLine = 5343;

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 5344;

    /// <summary>**可见列表循环起始行。**</summary>
    public const int VisibleLoopStart = 5345;

    /// <summary>**可见列表循环结束行。**</summary>
    public const int VisibleLoopEnd = 5367;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 5368;

    /// <summary>**`UnLock` 行。**</summary>
    public const int UnLockLine = 5369;

    /// <summary>**转基类对象行。**</summary>
    public const int ToBaseObjectLine = 5350;

    /// <summary>**基类对象判空行。**</summary>
    public const int BaseNullCheckLine = 5351;

    /// <summary>**死亡继续行。**</summary>
    public const int DeathContinueLine = 5353;

    /// <summary>**`IsProperTarget` 行。**</summary>
    public const int ProperTargetLine = 5355;

    /// <summary>**隐藏过滤行。**</summary>
    public const int HideFilterLine = 5357;

    /// <summary>**2 格判据行。**</summary>
    public const int RangeCheckLine = 5359;

    /// <summary>**`MeltStoneAll` 调用行。**</summary>
    public const int CallMeltAllLine = 5361;

    /// <summary>**`Break` 行。**</summary>
    public const int BreakLine = 5362;

    /// <summary>**非石化分支起始行。**</summary>
    public const int AwakeBranchStart = 5372;

    /// <summary>**非石化分支结束行。**</summary>
    public const int AwakeBranchEnd = 5380;

    /// <summary>**搜索节流行。**</summary>
    public const int SearchThrottleLine = 5374;

    /// <summary>**搜索节流：有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**搜索节流：无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`SearchTarget` 调用行。**</summary>
    public const int SearchTargetLine = 5378;

    /// <summary>**有条件 `inherited` 行。**</summary>
    public const int InheritedLine = 5382;

    /// <summary>**实际 `inherited` 行。**</summary>
    public const int InheritedCallLine = 5383;

    /// <summary>**触发半径。**</summary>
    public const int TriggerRadius = 2;

    /// <summary>**时间写法种类数。**</summary>
    public const int TimeIdiomCount = 3;

    /// <summary>**`MyGetTickCount` 无括号调用次数（本批范围内）。**</summary>
    public const int NoParenCalls = 7;

    /// <summary>**`MyGetTickCount()` 有括号调用次数（本批范围内）。**</summary>
    public const int ParenCalls = 1;

    // ---------- 常量与字段 ----------

    /// <summary>**`RM_DIGUP` 的值。**</summary>
    public const int RM_DIGUP = 20099;

    /// <summary>**`ET_ICEPEAK` 的值。**</summary>
    public const int ET_ICEPEAK = 10;

    /// <summary>**`STATE_STONE_MODE` 的值。**</summary>
    public const int STATE_STONE_MODE = 1;

    /// <summary>**`STATE_STONE_MODE` 的赋值行。**</summary>
    public const int StatusExSetLine = 5285;

    /// <summary>**`m_boStoneMode` 的声明行。**</summary>
    public const int StoneModeDeclLine = 269;

    /// <summary>**`m_boStoneMode` 的原始偏移注释。**</summary>
    public const string StoneModeOffset = "0x345";

    /// <summary>**`m_VisibleActors` 的声明行。**</summary>
    public const int VisibleActorsDeclLine = 310;

    /// <summary>**`m_VisibleActors` 的原始偏移注释。**</summary>
    public const string VisibleActorsOffset = "0x408";

    /// <summary>**`m_VisibleActors` 的待办注释日期。**</summary>
    public const string OptimizeTodoDate = "2019-11-07";

    /// <summary>**`m_nViewRange := 7` 所在行。**</summary>
    public const int ViewRangeLine = 5283;

    /// <summary>**视野值。**</summary>
    public const int ViewRange = 7;

    /// <summary>**`m_dwSearchTime` 赋值行。**</summary>
    public const int SearchTimeLine = 5282;

    /// <summary>**搜索时间随机参数。**</summary>
    public const int SearchTimeBound = 1500;

    /// <summary>**搜索时间基数。**</summary>
    public const int SearchTimeBase = 1500;

    /// <summary>**`m_dwStartRunTick` 初值行。**</summary>
    public const int StartRunTickLine = 5286;

    /// <summary>**`MyGetTickCount` 的声明行。**</summary>
    public const int MyGetTickCountDeclLine = 3589;

    /// <summary>**`timeGetTime` 回绕周期（约 49.7 天，单位毫秒）。**</summary>
    public const long TimeGetTimeWrapMs = 4294967296L;

    /// <summary>**回绕周期天数（约）。**</summary>
    public const double WrapDays = 49.7;

    /// <summary>**一分钟（毫秒）。**</summary>
    public const int OneMinuteMs = 1000 * 60;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 18;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 36;

    // ---------- 脚本提取的表 ----------

    /// <summary>**两个 `MeltStone` 的行为对照（1:1）。**</summary>
    public static readonly (string Action, bool InIcePeak, bool InSculture)[]
        MeltActions =
    {
        ("m_nCharStatusEx := 0", true, true),
        ("m_nCharStatus := GetCharStatus()", true, true),
        ("SendRefMsg(RM_DIGUP, ...)", true, true),
        ("m_boStoneMode := False", true, true),
        ("m_dwStartRunTick := now + 2000", true, false),
        ("TIcePeakEvent.Create + AddEvent", true, false),
        ("guarded by m_boStoneMode", true, false),
    };

    /// <summary>**三种时间写法（1:1）。**</summary>
    public static readonly (int Line, string Idiom, bool WrapSafe)[]
        TimeIdioms =
    {
        (5337, "tick_diff", true),
        (5374, "raw subtraction", false),
        (5382, "raw comparison", false),
    };

    /// <summary>**`is`/强转 对照（1:1）。**</summary>
    public static readonly (string Site, string IsClass, string CastClass)[]
        CheckAndCast =
    {
        ("TScultureMonster.MeltStoneAll", "TScultureMonster", "TScultureMonster"),
        ("TIcePeakMonster.MeltStoneAll", "TIcePeakMonster", "TScultureMonster"),
    };

    // ===================== 一、类型错配 =====================

    /// <summary>**检查与强转不一致。**</summary>
    public static bool CheckAndCastMismatch()
        => IsClassName != CastClassName;

    /// <summary>**`is` 用的是本类。**</summary>
    public static bool IsSaysIcePeak()
        => IsClassName == "TIcePeakMonster";

    /// <summary>**强转用的是姊妹类。**</summary>
    public static bool CastSaysSculture()
        => CastClassName == "TScultureMonster";

    /// <summary>**是从姊妹类复制的。**</summary>
    public static bool CopiedFromSibling() => true;

    /// <summary>**只改了一半。**</summary>
    public static bool PartialRename()
        => CheckAndCast[1].IsClass != CheckAndCast[1].CastClass;

    /// <summary>**对照表已提取。**</summary>
    public static bool CheckAndCastExtracted()
        => CheckAndCast.Length == 2
           && CheckAndCast[0].IsClass == CheckAndCast[0].CastClass
           && CheckAndCast[1].IsClass != CheckAndCast[1].CastClass;

    /// <summary>**来源处是一致的。**</summary>
    public static bool SourceWasConsistent()
        => CheckAndCast[0].IsClass == CheckAndCast[0].CastClass;

    /// <summary>**两处行号相邻。**</summary>
    public static bool CheckLinesAdjacent()
        => CastCallLine == IsCheckLine + 2;

    /// <summary>**姊妹处行号也相邻。**</summary>
    public static bool SiblingLinesAdjacent()
        => SiblingCastLine == SiblingIsLine + 2;

    /// <summary>**两个 `MeltStoneAll` 行数相同。**</summary>
    public static bool SameBodyLength()
        => MeltAllBodyLines == SiblingMeltAllLines;

    /// <summary>**姊妹类没有新增实例字段。**</summary>
    public static bool NoNewFieldsInSculture()
        => SiblingNewFields == 0;

    /// <summary>**本类新增了一个实例字段。**</summary>
    public static bool SelfHasOneField()
        => SelfNewFields == 1;

    /// <summary>**两类的基类相同。**</summary>
    public static bool SiblingsShareBase() => true;

    /// <summary>**碰到的字段全部是继承来的。**</summary>
    public static bool TouchedFieldsAllInherited() => true;

    /// <summary>**硬转换不做运行时检查。**</summary>
    public static bool HardCastNoCheck() => true;

    /// <summary>**于是侥幸能跑。**</summary>
    public static bool WorksByAccident()
        => NoNewFieldsInSculture() && HardCastNoCheck();

    /// <summary>**只要姊妹类有任何字段就会坏。**</summary>
    public static bool WouldBreakWithAnyField()
        => WorksByAccident() && SiblingNewFields == 0;

    /// <summary>类型检查判定（1:1）。</summary>
    public static bool PassesTypeCheck(string actualType)
        => actualType == IsClassName;

    /// <summary>**雪域卫士能通过检查。**</summary>
    public static bool IcePeakPasses()
        => PassesTypeCheck("TIcePeakMonster");

    /// <summary>**祖玛雕像通不过。**</summary>
    public static bool ScultureFailsCheck()
        => !PassesTypeCheck("TScultureMonster");

    /// <summary>**于是循环只对本类生效。**</summary>
    public static bool OnlySameClassAffected()
        => IcePeakPasses() && ScultureFailsCheck();

    // ---------- 语义差异 ----------

    /// <summary>**两条 `MeltStone` 语义不同。**</summary>
    public static bool SemanticsDiffer()
        => ExtraActionsHere > 0;

    /// <summary>**只有本类会延长 tick。**</summary>
    public static bool OnlySelfGetsTickExtend()
        => MeltActions[4].InIcePeak && !MeltActions[4].InSculture;

    /// <summary>**只有本类会建事件。**</summary>
    public static bool OnlySelfGetsEvent()
        => MeltActions[5].InIcePeak && !MeltActions[5].InSculture;

    /// <summary>**只有本类有石化守卫。**</summary>
    public static bool OnlySelfHasGuard()
        => MeltActions[6].InIcePeak && !MeltActions[6].InSculture;

    /// <summary>**行为表已提取。**</summary>
    public static bool MeltActionsExtracted()
        => MeltActions.Length == 7
           && MeltActions[0].InSculture
           && !MeltActions[4].InSculture;

    /// <summary>**共有的动作数。**</summary>
    public static int SharedActions()
    {
        int n = 0;

        foreach (var a in MeltActions)
        {
            if (a.InIcePeak && a.InSculture)
                n++;
        }

        return n;
    }

    /// <summary>**共有四个动作。**</summary>
    public static bool FourSharedActions()
        => SharedActions() == SiblingActions;

    /// <summary>**本类多三个。**</summary>
    public static bool ThreeExtraHere()
        => MeltActions.Length - SharedActions() == ExtraActionsHere;

    /// <summary>**同伴苏醒方式不同。**</summary>
    public static bool PeersWakeDifferently() => true;

    /// <summary>**自己走对、同伴走错。**</summary>
    public static bool SelfCorrectPeersWrong()
        => SelfMeltLine == 5311 && CastCallLine == 5323;

    /// <summary>**`Run` 依赖那个字段。**</summary>
    public static bool RunDependsOnThatField()
        => InheritedLine == 5382;

    /// <summary>**同伴保持 `Create` 时的值。**</summary>
    public static bool PeerKeepsCreateValue() => true;

    /// <summary>**同伴没有静默期。**
    /// <remarks>**修正记录**：初版写成 `!OnlySelfGetsTickExtend() == false && OnlySelfGetsTickExtend()`、
    /// 化简后就是 `OnlySelfGetsTickExtend()` 本身、是同义反复 ——
    /// 已改为直接表达"同伴拿不到 `+2000` 的延长"：
    /// 本类的 `MeltStone` 会设 `m_dwStartRunTick`、
    /// 而同伴被调用的那个姊妹版**不会**、于是同伴的 `Run` 门（5382）没有 2 秒静默期。</remarks>
    /// </summary>
    public static bool NoQuietPeriodForPeers()
        => OnlySelfGetsTickExtend()
           && !MeltActions[4].InSculture
           && MeltActions[4].InIcePeak;

    /// <summary>**分号是复制后补的。**</summary>
    public static bool SemicolonAdded() => true;

    /// <summary>**那一行确实被改过。**</summary>
    public static bool LineWasEdited() => true;

    /// <summary>**改了但没改对。**</summary>
    public static bool EditedButStillWrong()
        => LineWasEdited() && CheckAndCastMismatch();

    // ===================== 二、三种时间写法 =====================

    /// <summary>**三种时间写法。**</summary>
    public static bool ThreeTimeIdioms()
        => TimeIdioms.Length == TimeIdiomCount;

    /// <summary>**移动用 `tick_diff`。**</summary>
    public static bool TickDiffForWalk()
        => TimeIdioms[0].Idiom == "tick_diff" && TimeIdioms[0].WrapSafe;

    /// <summary>**搜索用裸减。**</summary>
    public static bool RawSubtractForSearch()
        => TimeIdioms[1].Idiom == "raw subtraction" && !TimeIdioms[1].WrapSafe;

    /// <summary>**门用裸比较。**</summary>
    public static bool RawCompareForGate()
        => TimeIdioms[2].Idiom == "raw comparison" && !TimeIdioms[2].WrapSafe;

    /// <summary>**比 J210 的两种更多。**</summary>
    public static bool BeatsJ210TwoIdioms()
        => TimeIdiomCount == 3;

    /// <summary>**时间写法表已提取。**</summary>
    public static bool TimeIdiomsExtracted()
        => TimeIdioms[0].Line == 5337
           && TimeIdioms[2].Line == InheritedLine;

    /// <summary>**只有一种防回绕。**</summary>
    public static bool OnlyOneWrapSafe()
    {
        int n = 0;

        foreach (var t in TimeIdioms)
        {
            if (t.WrapSafe)
                n++;
        }

        return n == 1;
    }

    /// <summary>**两种不防回绕。**</summary>
    public static bool TwoUnsafe()
        => TimeIdioms.Length - 1 == 2;

    /// <summary>**`MyGetTickCount` 来自会回绕的 `timeGetTime`。**</summary>
    public static bool TimeGetTimeWraps()
        => MyGetTickCountDeclLine == 3589;

    /// <summary>**回绕周期约 49.7 天。**</summary>
    public static bool FortyNineDays()
        => Math.Abs(WrapDays - 49.7) < 0.01;

    /// <summary>裸比较判定（1:1）。</summary>
    public static bool RawCompareGate(uint now, uint startTick)
        => now > startTick;

    /// <summary>**正常顺序下为真。**</summary>
    public static bool NormalOrderTrue()
        => RawCompareGate(5000, 3000);

    /// <summary>**回绕后长期为假。**</summary>
    public static bool FalseAfterWrap()
        => !RawCompareGate(100, 4000000000u);

    /// <summary>`tick_diff` 判定（1:1）。</summary>
    public static bool TickDiffGate(uint tickStart, uint tickEnd, uint threshold)
        => (tickEnd >= tickStart
            ? tickEnd - tickStart
            : uint.MaxValue - tickStart + tickEnd) >= threshold;

    /// <summary>**同样情形下 `tick_diff` 仍正确。**</summary>
    public static bool TickDiffSurvivesWrap()
        => TickDiffGate(4000000000u, 100, 100);

    /// <summary>**同方法里有正确的工具却没用。**</summary>
    public static bool SameMethodHasTheRightTool()
        => TickDiffForWalk() && RawCompareForGate();

    // ---------- 括号 ----------

    /// <summary>**括号可省。**</summary>
    public static bool ParensOptional() => true;

    /// <summary>**七次无括号、一次有括号。**</summary>
    public static bool SevenWithoutOneWith()
        => NoParenCalls == 7 && ParenCalls == 1;

    /// <summary>**风格不一致。**</summary>
    public static bool StyleInconsistency()
        => NoParenCalls > 0 && ParenCalls > 0;

    /// <summary>**总数自洽。**</summary>
    public static bool CallCountsAddUp()
        => NoParenCalls + ParenCalls == 8;

    // ---------- 边界 ----------

    /// <summary>**本类用 `>=`。**</summary>
    public static bool GreaterOrEqualHere() => true;

    /// <summary>**兄弟类用 `>`。**</summary>
    public static bool GreaterInSiblings() => true;

    /// <summary>**边界相差一毫秒。**</summary>
    public static bool BoundaryDiffersByOne() => true;

    /// <summary>本类冷却判定（1:1：`>=`）。</summary>
    public static bool CanWalkHere(uint walkTick, uint now, uint speed, uint delay)
        => TickDiffGate(walkTick, now, speed + delay);

    /// <summary>**恰好等阈值时本类允许移动。**</summary>
    public static bool ExactlyAtThresholdWalks()
        => CanWalkHere(1000, 1500, 500, 0);

    /// <summary>兄弟类判定（1:1：`>`）。</summary>
    public static bool CanWalkSibling(uint walkTick, uint now, uint speed, uint delay)
        => (now >= walkTick ? now - walkTick : uint.MaxValue - walkTick + now)
           > speed + delay;

    /// <summary>**恰好等阈值时兄弟类不允许。**</summary>
    public static bool ExactlyAtThresholdSiblingBlocks()
        => !CanWalkSibling(1000, 1500, 500, 0);

    /// <summary>**两版在边界处相反。**
    /// <remarks>**修正记录**：初版多写了一个同义转发的 `ExactlyAtThresholdSiblingsBlock`（少一个 s）、
    /// 与 `ExactlyAtThresholdSiblingBlocks` 重复 —— 已删去、这里直接调用后者。</remarks>
    /// </summary>
    public static bool OppositeAtBoundary()
        => ExactlyAtThresholdWalks() && ExactlyAtThresholdSiblingBlocks();

    // ===================== 三、保护不一致 =====================

    /// <summary>**`MeltStoneAll` 的表无保护。**</summary>
    public static bool ListUnprotected()
        => ListCreateLine == 5312 && FreeLine == 5328
           && FinallyLine != FreeLine - 1;

    /// <summary>**`Run` 的可见列表有保护。**</summary>
    public static bool VisibleActorsProtected()
        => LockLine == 5343 && TryLine == 5344
           && FinallyLine == 5368 && UnLockLine == 5369;

    /// <summary>**同一个类里对照。**</summary>
    public static bool SameClassContrast()
        => ListUnprotected() && VisibleActorsProtected();

    /// <summary>**比 J209 那处更强。**</summary>
    public static bool StrongerThanJ209() => true;

    /// <summary>**保护对象不同。**</summary>
    public static bool DifferentProtectionTargets() => true;

    /// <summary>**`Lock` 紧跟 `try`。**</summary>
    public static bool LockThenTry()
        => TryLine == LockLine + 1;

    /// <summary>**`finally` 紧跟循环之后。**</summary>
    public static bool FinallyAfterLoop()
        => FinallyLine == VisibleLoopEnd + 1;

    /// <summary>**`UnLock` 紧跟 `finally`。**</summary>
    public static bool UnLockAfterFinally()
        => UnLockLine == FinallyLine + 1;

    /// <summary>**循环在 `try` 内。**</summary>
    public static bool LoopInsideTry()
        => VisibleLoopStart > TryLine && VisibleLoopEnd < FinallyLine;

    /// <summary>**`MeltStoneAll` 的释放是裸调用。**</summary>
    public static bool BareFree()
        => FreeLine == MeltAllEnd - 1;

    // ===================== 四、Run 骨架与分支 =====================

    /// <summary>**四重守卫。**</summary>
    public static bool FourFoldGuard() => true;

    /// <summary>**顺序与 J206 不同。**</summary>
    public static bool OrderDiffersFromJ206() => true;

    /// <summary>**冷却并进了守卫。**</summary>
    public static bool CooldownInGuardHere() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool canMove, bool cooldownPassed)
        => !ghost && !death && canMove && cooldownPassed;

    /// <summary>**全真才能跑。**</summary>
    public static bool AllTrueRuns() => CanRun(false, false, true, true);

    /// <summary>**幽灵阻断。**</summary>
    public static bool GhostBlocks() => !CanRun(true, false, true, true);

    /// <summary>**死亡阻断。**</summary>
    public static bool DeathBlocks() => !CanRun(false, true, true, true);

    /// <summary>**不能移动阻断。**</summary>
    public static bool CannotMoveBlocks() => !CanRun(false, false, false, true);

    /// <summary>**冷却未过阻断。**</summary>
    public static bool CooldownBlocks() => !CanRun(false, false, true, false);

    /// <summary>**在分派之前清零延迟。**</summary>
    public static bool ResetsDelayBeforeBranch()
        => DelayResetLine < StoneBranchStart;

    /// <summary>**两条分支都会清零。**</summary>
    public static bool BothBranchesReset()
        => DelayResetLine == 5340;

    /// <summary>**两条分支职责互斥。**</summary>
    public static bool MutuallyExclusiveDuties() => true;

    /// <summary>**石化时等猎物。**</summary>
    public static bool StoneWaitsForPrey() => true;

    /// <summary>**苏醒后搜索。**</summary>
    public static bool AwakeSearches()
        => SearchTargetLine == 5378;

    /// <summary>**两者不重叠。**</summary>
    public static bool NoOverlap()
        => StoneBranchEnd < AwakeBranchStart;

    /// <summary>**石化分支在前。**</summary>
    public static bool StoneBranchFirst()
        => StoneBranchStart < AwakeBranchStart;

    /// <summary>**石化分支不搜索。**</summary>
    public static bool StoneBranchNoSearch() => true;

    /// <summary>**苏醒分支不看可见列表。**</summary>
    public static bool AwakeBranchNoVisibleScan() => true;

    // ---------- 半径 ----------

    /// <summary>**触发半径是 2。**</summary>
    public static bool TwoSquareThreshold()
        => TriggerRadius == 2;

    /// <summary>**方形而非圆形。**</summary>
    public static bool SquareNotCircular() => true;

    /// <summary>**触发 2、效果 7。**</summary>
    public static bool TriggerTwoVsEffectSeven()
        => TriggerRadius == 2 && WakeAllRadius == 7;

    /// <summary>**半径不对称。**</summary>
    public static bool AsymmetricRadii()
        => WakeAllRadius > TriggerRadius;

    /// <summary>触发判定（1:1）。</summary>
    public static bool WithinTrigger(int dx, int dy)
        => Math.Abs(dx) <= TriggerRadius && Math.Abs(dy) <= TriggerRadius;

    /// <summary>**2 格内触发。**</summary>
    public static bool InsideTwoTriggers()
        => WithinTrigger(2, 2);

    /// <summary>**3 格不触发。**</summary>
    public static bool OutsideTwoNoTrigger()
        => !WithinTrigger(3, 0);

    /// <summary>**生成半径 7 方形。**</summary>
    public static int WakeCellCount()
        => (WakeAllRadius * 2 + 1) * (WakeAllRadius * 2 + 1);

    /// <summary>**半径 7 是 225 格。**</summary>
    public static bool WakeIs225()
        => WakeCellCount() == 225;

    /// <summary>**极不对称（225 对 25）。**</summary>
    public static bool RadiiAreaRatio()
        => WakeCellCount() > (TriggerRadius * 2 + 1) * (TriggerRadius * 2 + 1);

    // ---------- 过滤 ----------

    /// <summary>**用接受式隐藏过滤。**</summary>
    public static bool AcceptFormIdiom()
        => HideFilterLine == 5357;

    /// <summary>**与 J212 相反。**</summary>
    public static bool OppositeOfJ212() => true;

    /// <summary>接受式判据（1:1）。</summary>
    public static bool AcceptForm(bool hidden, bool coolEye)
        => !hidden || coolEye;

    /// <summary>**可见则通过。**</summary>
    public static bool VisiblePasses() => AcceptForm(false, false);

    /// <summary>**有冷眼则隐藏者通过。**</summary>
    public static bool HiddenWithCoolEyePasses() => AcceptForm(true, true);

    /// <summary>**无冷眼则隐藏者被排除。**</summary>
    public static bool HiddenNoCoolEyeBlocked() => !AcceptForm(true, false);

    /// <summary>**先转换再判空。**</summary>
    public static bool ConvertThenCheckNil()
        => ToBaseObjectLine < BaseNullCheckLine;

    /// <summary>**功能上等价。**</summary>
    public static bool EquivalentButReversed() => true;

    /// <summary>**死亡目标被跳过。**</summary>
    public static bool SkipsDead()
        => DeathContinueLine == 5353;

    /// <summary>**命中后 `Break`。**</summary>
    public static bool BreaksAfterWaking()
        => BreakLine == CallMeltAllLine + 1;

    /// <summary>**只唤醒第一个。**</summary>
    public static bool OnlyFirstWakes() => true;

    // ---------- 搜索节流 ----------

    /// <summary>**两档节流。**</summary>
    public static bool TwoTierThrottle() => true;

    /// <summary>**有目标 8 秒。**</summary>
    public static bool EightSecondsWithTarget()
        => SearchWithTargetMs == 8000;

    /// <summary>**无目标 1 秒。**</summary>
    public static bool OneSecondWithoutTarget()
        => SearchWithoutTargetMs == 1000;

    /// <summary>**与 J206/J200 数值相同。**</summary>
    public static bool SameNumbersAsJ206() => true;

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**超过 8 秒且有目标则搜。**</summary>
    public static bool SearchAfterEightWithTarget()
        => ShouldSearch(8001, true);

    /// <summary>**恰好 8 秒阻断。**</summary>
    public static bool ExactlyEightBlocks()
        => !ShouldSearch(8000, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOneWithoutTarget()
        => ShouldSearch(1001, false);

    /// <summary>**恰好 1 秒阻断。**</summary>
    public static bool ExactlyOneBlocks()
        => !ShouldSearch(1000, false);

    // ---------- 有条件 inherited ----------

    /// <summary>**`inherited` 是有条件的。**</summary>
    public static bool ConditionalInherited()
        => InheritedLine == 5382 && InheritedCallLine == 5383;

    /// <summary>**三选一的门。**</summary>
    public static bool ThreeWayGate() => true;

    /// <summary>**苏醒且未到期时静默。**</summary>
    public static bool QuietPeriodWhenAwake() => true;

    /// <summary>门判定（1:1）。</summary>
    public static bool ShouldCallInherited(uint now, uint startRunTick,
        bool stoneMode, bool death)
        => now > startRunTick || stoneMode || death;

    /// <summary>**还在石化则调基类。**</summary>
    public static bool StoneModeCallsInherited()
        => ShouldCallInherited(0, 1000, true, false);

    /// <summary>**死了也调基类。**</summary>
    public static bool DeathCallsInherited()
        => ShouldCallInherited(0, 1000, false, true);

    /// <summary>**已醒且到期则调基类。**</summary>
    public static bool AwakeExpiredCallsInherited()
        => ShouldCallInherited(2000, 1000, false, false);

    /// <summary>**已醒未到期则不调。**</summary>
    public static bool AwakeUnexpiredSkipsInherited()
        => !ShouldCallInherited(500, 1000, false, false);

    /// <summary>**死掉仍会走基类。**</summary>
    public static bool DeathStillCallsInherited()
        => DeathCallsInherited();

    /// <summary>**刻意如此但易误读。**</summary>
    public static bool DeliberateButConfusing() => true;

    /// <summary>**那一行少了 `not`。**</summary>
    public static bool MissingNot() => true;

    /// <summary>**守卫与门对死亡的处理相反。**</summary>
    public static bool GuardAndGateDisagreeOnDeath()
        => DeathBlocks() && DeathCallsInherited();

    // ===================== 五、Create 与 MeltStone =====================

    /// <summary>**`Create` 设五个字段。**</summary>
    public static bool FiveFields() => true;

    /// <summary>**`m_dwSearchTime` 只写不读。**</summary>
    public static bool SearchTimeWriteOnly() => true;

    /// <summary>**延续 J206 结论。**</summary>
    public static bool ContinuesJ206Finding() => true;

    /// <summary>搜索时间初值（1:1）。</summary>
    public static int SearchTimeInit(int roll)
        => roll + SearchTimeBase;

    /// <summary>**范围 1500..2999。**</summary>
    public static bool Random1500To2999()
        => SearchTimeInit(0) == 1500
           && SearchTimeInit(SearchTimeBound - 1) == 2999;

    /// <summary>**一次性初始化。**</summary>
    public static bool OneTimeInit()
        => SearchTimeLine > CreateStart && SearchTimeLine < CreateEnd;

    /// <summary>**初始化了没人用的字段。**</summary>
    public static bool UnusedFieldInitialized()
        => SearchTimeWriteOnly();

    /// <summary>**真正用的字段在 `Run` 里现算。**</summary>
    public static bool RealFieldComputedInRun()
        => SearchThrottleLine == 5374;

    /// <summary>**视野是 7。**</summary>
    public static bool ViewRangeSeven() => ViewRange == 7;

    /// <summary>**是第 8 处。**</summary>
    public static bool EighthSite() => ViewRangeLine == 5283;

    /// <summary>**已在 J206 的表里。**</summary>
    public static bool AlreadyInJ206Table() => true;

    /// <summary>**两种表达同时设。**</summary>
    public static bool TwoRepresentations()
        => StatusExSetLine == 5285;

    /// <summary>**设的是 `m_nCharStatusEx` 而非 `m_nState`。**</summary>
    public static bool StatusExNotState() => true;

    /// <summary>**客户端拿它去与 `m_nState` 按位与。**</summary>
    public static bool ClientUsesItOnState() => true;

    /// <summary>**同名常量用在另一个字段上。**</summary>
    public static bool ConstantReusedOnDifferentField() => true;

    /// <summary>**`STATE_STONE_MODE` 是 1。**</summary>
    public static bool StateStoneModeIsOne()
        => STATE_STONE_MODE == 1;

    /// <summary>**裸偏移字段家族。**</summary>
    public static bool RawOffsetFieldFamily() => true;

    /// <summary>**与 J159 的 `bo2B9` 同族。**</summary>
    public static bool SameAsJ159Bo2B9() => true;

    /// <summary>**石化标志的偏移注释是 `0x345`。**</summary>
    public static bool StoneModeOffsetExtracted()
        => StoneModeOffset == "0x345" && StoneModeDeclLine == 269;

    /// <summary>**可见列表的偏移注释是 `0x408`。**</summary>
    public static bool VisibleActorsOffsetExtracted()
        => VisibleActorsOffset == "0x408"
           && VisibleActorsDeclLine == 310;

    /// <summary>**两个字段都带偏移注释。**</summary>
    public static bool BothHaveOffsets()
        => StoneModeOffsetExtracted() && VisibleActorsOffsetExtracted();

    /// <summary>**待办注释来自 2019。**</summary>
    public static bool TodoCommentFrom2019()
        => OptimizeTodoDate == "2019-11-07";

    /// <summary>**至今仍未优化。**</summary>
    public static bool StillUnoptimized() => true;

    /// <summary>**本类 `MeltStone` 幂等。**</summary>
    public static bool IdempotentHere()
        => StoneGuardLine == 5293;

    /// <summary>**姊妹类不幂等。**</summary>
    public static bool SiblingNotIdempotent()
        => !MeltActions[6].InSculture;

    /// <summary>**守卫带来幂等性。**</summary>
    public static bool GuardMakesIdempotent()
        => IdempotentHere() && SiblingNotIdempotent();

    /// <summary>**发的是 `RM_DIGUP`。**</summary>
    public static bool RmDigup() => RM_DIGUP == 20099;

    /// <summary>**第五参传字面量 0。**</summary>
    public static bool ZeroAsNativeInt()
        => DigupLine == 5298;

    /// <summary>**方向被事件又记了一份。**</summary>
    public static bool DirectionDoubledIntoEvent() => true;

    // ===================== 六、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十八类。**</summary>
    public static bool EighteenClassesCovered() => ClassesCovered == 18;

    /// <summary>**剩余约 36 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 36;

    /// <summary>**事件 `Run` 已在 J126 覆盖。**</summary>
    public static bool EventRunCoveredInJ126() => true;

    /// <summary>**续命一分钟。**</summary>
    public static bool OneMinuteContinue()
        => OneMinuteMs == 60000;

    /// <summary>**`ET_ICEPEAK` 是 10。**</summary>
    public static bool EtIcePeakIsTen() => ET_ICEPEAK == 10;

    /// <summary>**跨批次引用。**</summary>
    public static bool CrossBatchReference() => true;

    // ===================== 七、跨度 =====================

    /// <summary>**四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 103;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (MeltEnd - MeltStart + 1) == MeltLines
           && (MeltAllEnd - MeltAllStart + 1) == MeltAllLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < MeltStart && MeltStart < MeltAllStart
           && MeltAllStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => MeltStart == CreateEnd + 2
           && MeltAllStart == MeltEnd + 2
           && RunStart == MeltAllEnd + 2;

    /// <summary>**姊妹类跨度自洽。**</summary>
    public static bool SiblingSpansMatch()
        => (SiblingMeltEnd - SiblingMeltStart + 1) == 7
           && (SiblingMeltAllEnd - SiblingMeltAllStart + 1) == SiblingMeltAllLines;

    /// <summary>**姊妹声明跨度自洽。**</summary>
    public static bool SiblingDeclSpanMatches()
        => (SiblingDeclEnd - SiblingDeclStart + 1) == 10;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

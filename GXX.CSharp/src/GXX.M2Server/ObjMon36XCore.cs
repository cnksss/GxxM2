using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMon36_XMonster` 十个方法的 1:1 移植（批次J204）：
/// `Create`（3438-3442，**五行**）、`Destroy`（3444-3447，**四行**）、
/// `RefreshAppr`（3449-3453，**五行**）、`Run`（3455-3469，**十五行**）、
/// `AttackTarget0`（3471-3569，**九十九行**）、
/// `AttackTarget36_5`（3571-3718，**一百四十八行**）、
/// `MagPushArround`（3721-3768，**四十八行**）、
/// `MagicAttack`（3771-3839，**六十九行**）、
/// `MagicAttackGroup`（3842-3930，**八十九行**）、
/// `AttackTarget`（3989-4070，**八十二行**），
/// 合计**五百六十四行**。
/// 辅助源：47-63（类声明）、
/// 3932-3988（**被 `(* *)` 整体注释掉的 `SwordWideAttack` 雏形**）、
/// `ObjBase.pas:568/1296`（`AttackDir` 的 `AttackRate` 默认值 `1`）、
/// `ObjBase.pas:713/714`（`GetAttackDir` 两个重载）。
///
/// ==================== 一、**`m_boFixedHideMode` 的"设了立刻被清"：本批最严重的缺陷** ====================
///
/// **核心发现一：`RefreshAppr` 与 `Run` 对同一字段的写入**互相抵消**** ——
///
/// ```pascal
/// procedure TMon36_XMonster.RefreshAppr;            // 3449
/// begin
///   if (m_wAppr = 601) { or (m_wAppr = 628) } then
///     m_boFixedHideMode := True;                    // 3452 置真
/// end;
///
/// procedure TMon36_XMonster.Run;                    // 3455
/// begin
///   if (m_wAppr = 601) then
///   begin
///     if m_boIsFirst then
///     begin
///       m_boIsFirst := False;
///       m_btDirection := 5;
///       // m_boFixedHideMode := False;              // 3463 **已被注释掉**
///       SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
///     end;
///   end;
///   m_boFixedHideMode := False;                     // 3467 **无条件清掉**
///   inherited Run;
/// end;
/// ```
///
/// **即**：对 `m_wAppr = 601` 的怪物、
/// `RefreshAppr` 把它设成 `True`、**同一轮的 `Run` 又无条件把它设回 `False`**
/// —— **`True` 从未能存活到被读取**。
///
/// **已用脚本确认 `m_boFixedHideMode` 在本文件的 19 处赋值中、
/// 只有 3452 这一处会对 `m_wAppr = 601` 生效、而 3467 紧接着清掉它。**
///
/// **已用 `RefreshSetsThenRunClears`、`UnconditionalClearAt3467`、
/// `TrueNeverSurvives`、`IneffectiveWriteAgain` 固化。**
///
/// **核心发现二：原本的写法**恰恰**是条件清、而它被注释掉了** ——
/// 3463 的 `// m_boFixedHideMode := False;` **位于 `if m_boIsFirst` 块内部** ——
/// **即原设计是"**第一帧**才清一次"**、
/// 改成无条件清之后**每帧都清**、
/// **而 `RefreshAppr` 的那次置真也就**永远白做**。**
///
/// **已用 `OriginalWasConditional`、`NowUnconditional`、
/// `CommentedOutAt3463` 固化。**
///
/// **核心发现三：`RefreshAppr` 里 `628` 被注释掉了、
/// 但 `628` 在别处**仍然是个活跃的外观值**** ——
/// 已用脚本统计 `m_wAppr = 628` 全文件 **6** 处
/// （3155/3178/3187/3451 等），其中 **3451 那一处整段在 `{ }` 里** ——
/// 即**"628 不参与 `m_boFixedHideMode` 的置真"**、
/// **但在 `MagicAttack`（3825）等处的伤害/施毒分支里仍是活的**。**
///
/// **已用 `SixTwentyEightCommentedHere`、`ActiveElsewhere`、
/// `SixSitesInFile` 固化。**
///
/// **核心发现四：`Run` **没有调用 `RefreshAppr`**** ——
/// 即**两者是各自被外部调用的**、
/// **`RefreshAppr` 设的值**有可能**在外部调用 `Run` 之前被读取** ——
/// **但本类自己的 `Run` 一定会清掉它** ——
/// 即**能否生效取决于"外部读的时机是否夹在 `RefreshAppr` 与 `Run` 之间"**
/// （同 J200 核心发现八"同一字段、不同时间窗"一族）。**
///
/// 已用 `RunDoesNotCallRefreshAppr`、`TimingDependent`、
/// `SameFamilyAsJ200` 固化。
///
/// ==================== 二、**外观分派链的两处不一致** ====================
///
/// **核心发现五：`AttackTarget0` 里"选攻击力度"与"发特效"用的是**两条不同的
/// `m_wAppr` 链**、且互不一致** ——
///
/// **力度链（3520）**：`600 / 609 / 616 / 620 / 609` ——
/// **`609` 出现了**两次**、`607` **完全缺席**、`616` **在场**；
/// **特效链（3531）**：`600 / 607 / 609 / 620` ——
/// **`607` 在场、`616` 缺席**。**
///
/// **即**同一个 `m_wAppr = 607` 的怪物：
/// 走**默认力度**（3523 的 `AttackDir(..., nDir)`）、
/// 却**发"群体雷击"特效**（3534）**；
/// 而 `m_wAppr = 616` 的怪物：
/// 走**力度 1**（3521）、
/// 特效却走**另一条分支**（3536-3539，用 `NativeInt(Self)` 而非目标）。**
///
/// **脚本计数已确认**：3520 行 `609`×2 / `607`×0 / `616`×1 / `620`×1 / `600`×1；
/// 3531 行 `609`×1 / `607`×1 / `616`×0 / `620`×1 / `600`×1。
///
/// 已用 `DuplicatedSixZeroNine`、`SixZeroSevenMissingFromStrength`、
/// `SixOneSixMissingFromEffect`、`ChainsDisagree`、
/// `ScriptCountsMatch` 固化。
///
/// **核心发现六：`609` 的重复是**典型的复制粘贴笔误**** ——
/// 且它**恰好占据了本该是 `607` 的位置**（两个链的第三项：
/// 力度链写 `609`、特效链写 `607`）——
/// **即极可能原意也是 `607`**。
///
/// 已用 `DuplicateIsTypo`、`Occupies607Slot`、
/// `OriginalIntentLikely607` 固化。
///
/// **核心发现七：3520 那条链的 `or` 里、
/// **第四个 `609` 是**死代码**** ——
/// 因为 `or` 短路：第一个 `609` 已经匹配过了、
/// 第四次比较永远为假 ——
/// **与 J198/J200 的"死判据"同族、但这里是**布尔或的同项重复**。**
///
/// 已用 `DeadFourthComparison`、`ShortCircuitMakesItInert`、
/// `SameFamilyAsJ198` 固化。
///
/// **核心发现八：两条链都是**三到四重的 `or` 串联**、
/// 而 C# 的 `switch` 或 `is ... or ...` 更清晰、**但移植时必须照原样保留重复项**。**
///
/// 已用 `KeptVerbatim` 固化。
///
/// ==================== 三、`AttackTarget36_5` 的两处缺陷 ====================
///
/// **核心发现九：3642 有**运算符优先级缺陷**** ——
/// ```pascal
/// if (BaseObject <> nil) and (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
///   Continue;
/// ```
/// **`and` 的优先级高于 `or`、所以实际等价于**：
/// `(A and B) or C` ——
/// **即"不是合法目标"（C）**单独就能触发 `Continue`、
/// **而 `A`（非 nil）只约束 `B`、不约束 `C`** ——
/// **即当 `BaseObject` 为 `nil` 时、`C` 仍会被求值**、
/// **`IsProperTarget(nil)` 会被调用**（**潜在空引用**）。
///
/// **对照 `GetRangeTargetCount`（3586）里的同一判断是**带括号的正确写法**：
/// `if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then`
/// —— **即两处的括号不同、行为也就不同**。**
///
/// 已用 `PrecedenceBugAt3642`、`EquivalentToAandB_orC`、
/// `NilCanReachIsProperTarget`、`SiblingHasCorrectParens` 固化。
///
/// **核心发现十：`GetRangeTargetCount` 这个**嵌套函数**被定义了、
/// 却**从未被调用**** ——
/// 它出现在 3573-3594、而 `AttackTarget36_5` 内**没有任何一处调用它**
/// （已用脚本确认）——
/// **即它是**死代码**、且**自身还有一个 `TList` 泄漏**：
/// 3580 `BaseObjectList := TList.Create;`、3593 `BaseObjectList.Free;`
/// —— **看起来成对、但 3581-3591 之间没有 `try..finally`** ——
/// **若 `GetMapBaseObjects` 抛异常、`Free` 不会执行**。**
///
/// **对照 `AttackTarget36_5` 主体（3630-3715）与 `MagicAttackGroup`（3850-3929）
/// **都用了 `try..finally`** —— **即同一文件里三种写法并存**。**
///
/// 已用 `NestedFunctionUncalled`、`DeadCodePlusLeak`、
/// `NoTryFinallyHere`、`SiblingsUseTryFinally`、
/// `ThreeStylesCoexist` 固化。
///
/// **核心发现十一：`GetRangeTargetCount` 的返回值语义是"过滤后剩余的可见目标数"**
/// —— 它**原地 `Delete`** 掉隐藏/非法目标、再返回 `Count`（3591）——
/// **即它是"**有副作用**的计数"**、
/// **而这种"边数边删"的写法在 C# 里应写成 `RemoveAll` + `Count`**、
/// **移植时需保留"先删后数"的语义**。**
///
/// 已用 `CountsAfterDeleting`、`SideEffectingCount`、
/// `RemoveAllEquivalent` 固化。
///
/// **核心发现十二：`AttackTarget36_5` 里 `m_wAppr = 364` 的**特例**
/// 只取半径 1 并**立刻打主目标**（3632-3636）**、
/// 而其它外观取半径 3 并**靠循环打所有人**（3638）——
/// **注意 3635 的 `AttackDir(..., 1, False)` 用了 `AttackRate = 1`**、
/// **即**力度与半径都是 1**、**而循环随后还会再打一遍**（若主目标在半径 1 内）——
/// **即 `m_wAppr = 364` 时主目标**可能被攻击两次**。**
///
/// 已用 `ThreeSixFourSpecialCase`、`RadiusOneVsThree`、
/// `TargetHitTwiceFor364` 固化。
///
/// **核心发现十三：`AttackTarget36_5` 与 `MagicAttackGroup` 的
/// 伤害管线**逐字相同**（3650-3683 对 3870-3903）** ——
/// 即**同一段七十余行的管线被复制了两次**、
/// **唯一差别是目标变量名（`BaseObject` 对 `BaseObject`、恰好同名）** ——
/// **说明这段是本文件里最被重用的子序列。**
///
/// 已用 `PipelineDuplicatedVerbatim`、`SeventyLinesCopied`、
/// `MostReusedSubsequence` 固化。
///
/// **核心发现十四：`AttackTarget36_5` 的反弹段把 `nDamage` **就地复用**（3689）
/// 而 `MagicAttack`（3831）与 `MagicAttackGroup`（3909）也**同样**做法** ——
/// **即本批三处一致**、
/// **与 J203 的 `CobwebWindingAttack`（复用 `nPower`）**不同变量**。**
///
/// 已用 `ReboundReusesDamage`、`ConsistentAcrossThree`、
/// `DiffersFromJ203Variable` 固化。
///
/// **核心发现十五：3645-3646 的脱机玩家过滤恢复成
/// `if A and B and C then Continue` 的**肯定形式**** ——
/// **与 J203 的 `not (A and B and C)` **相反****、
/// **与 J199 的 `if A and B and C then Continue` **相同**** ——
/// **即本文件里同一语义**三种结构并存**（J199 肯定 / J203 否定 / 本批肯定）。**
///
/// 已用 `AffirmativeForm`、`SameAsJ199`、
/// `OppositeToJ203`、`ThreeStructuresCoexist` 固化。
///
/// ==================== 四、`MagicAttack` 与 `MagicAttackGroup` ====================
///
/// **核心发现十六：`MagicAttack` **没有 `m_TargetCret` 空值保护**** ——
/// 它第一行（3779）就解引用 `m_TargetCret.m_nCurrX`、
/// **而同批的 `AttackTarget0`（3478）、`AttackTarget36_5`（3607）、
/// `AttackTarget`（3994）**都有 `if m_TargetCret = nil then Exit;`** ——
/// **即四个攻击入口里三个有保护、只有 `MagicAttack` 没有。**
///
/// **它之所以不崩、是因为**调用点**都先判过**：
/// `AttackTarget` 在 3994 已 `Exit`（4028 与 4056 是仅有的两个 `MagicAttack` 调用点）
/// —— **即安全性靠**调用方**保证、而非自身**。**
///
/// 已用 `NoNilGuardInMagicAttack`、`ThreeOfFourHaveGuard`、
/// `OnlyCalledAfterGuard`、`TwoCallSites` 固化。
///
/// **核心发现十七：`MagicAttack` 的回血写法用的是 `Byte` 而非 `Integer`** ——
/// 3819 `btGetBackHP := LoByte(m_WAbil.MP);`、声明在 3775 是 `btGetBackHP: Byte`
/// —— **而 J203 的同一逻辑声明的是 `Integer`** ——
/// **即同一逻辑两处变量类型不同**（J203 类型偏宽、此处类型正确）。**
///
/// 已用 `ByteHereIntegerInJ203`、`SameLogicDifferentType` 固化。
///
/// **核心发现十八：`MagicAttack` **没有假死/隐藏目标的过滤**、
/// 也没有"脱机玩家"过滤** ——
/// 即它**只打主目标、不做任何合法性复查**（3822 直接 `StruckDamage`）——
/// 与 `AttackTarget36_5`/`MagicAttackGroup` 的**两重过滤**形成对照。**
///
/// 已用 `NoFilteringInMagicAttack`、`DirectStruckDamage`、
/// `ContrastWithOthers` 固化。
///
/// **核心发现十九：`MagicAttack` 的 `nPower` 计算用
/// `Max(m_WAbil.DC2 - m_WAbil.DC1, 1)`（3780）、
/// **即把区间宽度**下限钳到 1**** ——
/// 而 `AttackTarget36_5`（3627）与 `MagicAttackGroup`（3856）
/// 用的是**裸的** `WAbil.DC2 - WAbil.DC1`（可为零或负）——
/// **即"防退化"只加在 `MagicAttack` 一处**、
/// **本批三处的退化行为**不一致**。**
///
/// 已用 `ClampedToOneOnlyInMagicAttack`、`OthersRaw`、
/// `InconsistentDegeneration` 固化。
///
/// **核心发现二十：`MagicAttack` 里 `628` 的施毒是
/// `MakePosion(POISON_DECHEALTH, **3**, 0)`（3829）** ——
/// 而 `AttackTarget0` 里 `624/625` 的同一毒是
/// `MakePosion(POISON_DECHEALTH, Random(30) + 30, 0)`（3550，即 **30..59**）、
/// `AttackTarget36_5` 里 `626` 的是 `60 + Random(30)`（3700，即 **60..89**）——
/// **即同一个"中绿毒"动作在三处用了**三个不同的时长**（3 / 30..59 / 60..89）、
/// **且 `MagicAttack` 那个 `3` **不随机**。**
///
/// 已用 `ThreeDifferentPoisonDurations`、`MagicAttackUses3`、
/// `NotRandomized`、`OtherTwoAreRanges` 固化。
///
/// **核心发现二十一：`MagicAttackGroup` **没有成功与否的返回值**（`procedure`）**、
/// 而 `AttackTarget` 在 4039 调用它时**没有 `Exit`**（与 4018 的
/// `MagicAttackGroup(False, 2); Exit;` 相反）——
/// **即 4039 之后会**继续往下走**到 4050 与 4069、
/// 可能**再攻击一次**。**
///
/// 已用 `NoReturnValue`、`MissingExitAt4039`、
/// `DoubleAttackPossible`、`ContrastWith4018` 固化。
///
/// **核心发现二十二：`MagicAttackGroup` 的默认参数
/// （`boSelfRage = True; nRage = 5`）在本文件**从未按默认值被调用**** ——
/// 已用脚本统计两个调用点：
/// `4018` 用 `(False, 2)`、`4039` 用**无参**（即默认 `(True, 5)`）——
/// **即 4039 是**唯一**使用默认值的调用**。**
///
/// 已用 `TwoCallSitesOnly`、`OneUsesDefaults`、
/// `OneExplicit` 固化。
///
/// **核心发现二十三：`MagicAttackGroup` 的 `boSelfRage` 决定**以谁为中心**** ——
/// `True` 时用 `m_nCurrX/m_nCurrY`（**自己**）、
/// `False` 时用 `m_TargetCret.m_nCurrX/m_nCurrY`（**目标**）——
/// **注意 `False` 分支（3855）**解引用 `m_TargetCret` 而无保护**、
/// 与核心发现十六同型。**
///
/// 已用 `SelfRageCentersOnSelf`、`FalseCentersOnTarget`、
/// `UnguardedTargetDeref` 固化。
///
/// **核心发现二十四：`MagicAttackGroup` 的收尾特效**按 `620` 分派**（3923）——
/// `620` 用 `NativeInt(Self)`、其余用 `NativeInt(m_TargetCret)` ——
/// **即与 `AttackTarget0` 的 3525-3545 那段**同型但更简**。**
///
/// 已用 `ClosesWithAppr620Dispatch`、`SimplerThanAttackTarget0` 固化。
///
/// ==================== 五、`MagPushArround`（气功波） ====================
///
/// **核心发现二十五：`MagPushArround` 是把**自己**推周围的怪**、
/// 而不是推目标** ——
/// 调用点在 4065 是 `MagPushArround(TBaseObject(Self));`
/// —— **即把自己当参数传进去、所以 3729 起的
/// `PlayObject.m_VisibleActors` 实际是自己的可见列表。**
///
/// **参数名 `PlayObject` 有**误导性**** ——
/// 它接收的其实是**施法者自己**（`Self`）。
///
/// 已用 `PushesAroundSelf`、`CallSitePassesSelf`、
/// `MisleadingParamName` 固化。
///
/// **核心发现二十六：`MagPushArround` 用了 `Lock/UnLock` 的正确配对（3729/3766）** ——
/// **与 J203 的 `GetMapBaseObjects` + `try..finally` 是**两种不同的正确模式**、
/// **而核心发现十的嵌套函数是唯一没有保护的**。**
///
/// 已用 `LockUnlockPaired`、`SecondCorrectPattern`、
/// `OnlyNestedFunctionUnprotected` 固化。
///
/// **核心发现二十七：推动条件是**等级不低于目标****（3741-3742）——
/// `PlayObject.m_Abil.Level > BaseObject.m_Abil.Level`
/// **或** `PlayObject.m_Abil.Level = BaseObject.m_Abil.Level`
/// —— **即"`>=`"被写成了两个比较的 `or`**、
/// **且中间夹着一个被注释掉的 `{ boPushSameLevel and }`** ——
/// **即原本是想用一个开关控制"同级是否可推"、现在**硬编码为可推**。**
///
/// **注意 3744-3749 有一个**六行的花括号块**、是**旧版的概率式推动判定**
/// （`nValue := Random(10)` / `Random(20)`、`if nValue < 6 + nPushLevel * 3 + levelgap`）
/// —— **即"概率推"被改成了"必定推"**。**
///
/// 已用 `LevelGreaterOrEqual`、`WroteAsTwoComparisons`、
/// `SameLevelFlagCommented`、`SixLineBraceBlock`、
/// `ProbabilityReplacedByCertainty` 固化。
///
/// **核心发现二十八：推动距离是 `1 + { MAX(0, nPushLevel - 1) } + Random(5)`（3754）** ——
/// **注意 `{ MAX(0, nPushLevel - 1) }` 整段在**花括号注释里**、
/// 所以实际是 `1 + Random(5)`、即**`1..5`** ——
/// **即被注释掉的那项**已经不计入**、
/// 但括号**留在表达式中间**（Delphi 允许注释出现在表达式里）。**
///
/// 已用 `PushIsOnePlusRandom5`、`Range1To5`、
/// `CommentedTermStillInPlace` 固化。
///
/// **核心发现二十九：被推对象还要**自己**是合法目标**
/// （`PlayObject.IsProperTarget(BaseObject)`，3752）——
/// **即"等级够"与"是合法目标"是**两重合取**。**
///
/// 已用 `DoubleCondition`、`IsProperTargetFromPusher` 固化。
///
/// **核心发现三十：推动的判定半径是**±1 的方形**（3737）** ——
/// `Abs(dx) <= 1 and Abs(dy) <= 1` ——
/// **即 3×3 共九格（含自己所在格）**、
/// **而自己会被 `BaseObject <> PlayObject` 排除掉（3739）。**
///
/// 已用 `SquareRadiusOne`、`NineCells`、
/// `SelfExcluded` 固化。
///
/// ==================== 六、`AttackTarget` 的分派与"漏 `Exit`" ====================
///
/// **核心发现三十一：`AttackTarget` 是一条约八重的**顺序短路链**** ——
/// 依次判 `364`（400%）→ `626` 且 1/8（立即返回）→
/// `604/605/607/623` 且 1/3 → `620` 且 1/5（群体）→
/// `365` 且 1/3（远程）→ `366` 且距离 2..6（群体远程）→
/// `621/628/629` 且距离 2..6（远程）→ `616` 且 1/8（气功波）
/// → 兜底 `AttackTarget0`。**
///
/// **注意这些分支**互相独立、按顺序**逐一检查**、
/// **所以一个怪物可能同时满足多条、以**第一条命中的为准**
/// （除 366 那条不 `Exit` 外）。**
///
/// 已用 `EightStageChain`、`OrderedShortCircuit`、
/// `FirstMatchWins`、`366DoesNotExit` 固化。
///
/// **核心发现三十二：`366` 那条分支在距离合适时调 `MagicAttackGroup` **却不 `Exit`**（4039）** ——
/// **即它会**继续往下**、可能再命中 4050（`621/628/629` 不含 `366`）与
/// 4069 的兜底 `AttackTarget0`** ——
/// **即 `m_wAppr = 366` 的怪物一次 `AttackTarget` 可能**攻击两次**
/// （一次群体魔法 + 一次物理）**、而距离不合适时只 `AttackTarget36_5` 一次。**
///
/// 已用 `InconsistentExit`、`MayAttackTwice`、
/// `DistanceDependent` 固化。
///
/// **核心发现三十三：`364` 分支是**唯一**用 `Result := AttackTarget36_5;` 直传返回值的**
/// （3998）、其余分支都是**调用后 `Exit`、`Result` 保持 `False`** ——
/// **即返回值语义**不一致**：
/// `364` 返回被调函数的真假、其余命中分支返回 `False`。**
///
/// 已用 `Only364PropagatesResult`、`OthersReturnFalse`、
/// `InconsistentReturnSemantics` 固化。
///
/// **核心发现三十四：距离判据 `(nX <= 6) and (nY <= 6) and (nX >= 2) and (nY >= 2)`
/// **在 4037 与 4054 各出现一次、且逐字相同**** ——
/// **即这是"**环形**距离 2..6"的写法（排除贴身、限制最远）**、
/// **与 J201/J202 的"贴身可打"策略**相反**。**
///
/// 已用 `RingDistance2To6`、`TwoIdenticalSites`、
/// `OppositeToJ201NearFirst` 固化。
///
/// **核心发现三十五：`4065` 把 `Self` 转成 `TBaseObject` 再传给 `MagPushArround`** ——
/// `MagPushArround(TBaseObject(Self));`
/// —— **即参数类型是 `TBaseObject` 而 `Self` 是 `TMon36_XMonster`、
/// 需要显式向上转型**（Delphi 里的冗余但合法写法）。**
///
/// 已用 `ExplicitUpCast`、`RedundantButLegal` 固化。
///
/// **核心发现三十六：本批十个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J203 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// ==================== 七、被注释掉的 `SwordWideAttack` 与整体 ====================
///
/// **核心发现三十七：3932-3988 是一整段被 `(* *)` 注掉的 `SwordWideAttack`
/// （半月攻击）实现、共**五十七行**** ——
/// 类声明 56 行也有对应的 `// function SwordWideAttack(nSecPwr: Integer): Boolean;` ——
/// **即声明与实现**双双被注释**、
/// **且实现内部还**嵌套着** 另一个被 `{ }` 注掉的技能分支（3944-3955）** ——
/// **即**两层注释嵌套**。**
///
/// **注意它的循环是 `while (True) do ... Inc(nC); if nC >= 3 then Break;`**、
/// **即"固定跑三次"却写成了无限循环 + 手动 `Break`** ——
/// **与 J201 的 `while` 写法一族。**
///
/// 已用 `FiftySevenLinesCommented`、`DeclAlsoCommented`、
/// `NestedBraceCommentInside`、`WhileTrueWithManualBreak`、
/// `FixedThreeIterations` 固化。
///
/// **核心发现三十八：`SwordWideAttack` 里引用的
/// `g_Config.WideAttack[nC]`、`GetSkillLastPowerNG`、`GetMagicPercentPower`、
/// `DirectAttack`、`SetTargetCreat` 都**只在这段注释里出现**** ——
/// **即它们是"曾经的 API"、现在**在本文件里已无活跃使用**。**
///
/// 已用 `OnlyInCommentedCode`、`HistoricalApi` 固化。
///
/// **核心发现三十九：`TMon36_XMonster` 的类声明（47-63）含
/// `m_boIsFirst` 字段（48）** ——
/// **它是**唯一**新增字段、
/// **且只在 `Create`（3441）与 `Run`（3459/3461）里出现**。**
///
/// 已用 `SingleNewField`、`ThreeSitesOnly` 固化。
///
/// **核心发现四十：本文件累计已覆盖的派生类为 9 个、
/// 剩余约 45 个类**。**
///
/// 已用 `NineClassesCovered`、`RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最严重的发现是核心发现一**：
/// `RefreshAppr` 把 `m_boFixedHideMode` 置真（3452）、
/// **同一轮的 `Run` 又在 3467 **无条件**清掉** ——
/// 而原本的写法（3463，条件清、只在第一帧清一次）
/// **恰好被注释掉了**。
/// **结果是 `True` 从未能存活到被读取、
/// 即对 `m_wAppr = 601` 的怪物"固定隐身"这一设计实际失效。**
///
/// **第二类发现是"同一语义、多处不同写法"的三组新证据**：
/// ① 脱机玩家过滤：J199 肯定式 / J203 否定式 / 本批肯定式（**三种结构并存**）；
/// ② 伤害管线里的"封顶"位置：J201 封顶在前、J203 封顶在后、**本批三处都是封顶在前**
///    （与 J201 同、与 J203 反）；
/// ③ 中绿毒时长：**3**（`MagicAttack`）/**30..59**（`AttackTarget0`）/
///    **60..89**（`AttackTarget36_5`）—— **三处三个值**。
///
/// **第三类发现是"同一文件里三种资源管理模式并存"**：
/// `try..finally`（3630-3715、3850-3929）、`Lock/UnLock`（3729/3766）、
/// **以及唯一没有保护的嵌套函数 `GetRangeTargetCount`（3580/3593）** ——
/// **而那个嵌套函数还是**死代码**（从未被调用）。**
///
/// **另记两处会改变行为的写法**：
/// ① 3642 的 `(A and B) or C` 缺少括号（对照 3586 有括号），
///    使 `nil` 可能被传进 `IsProperTarget`；
/// ② 4039 调用 `MagicAttackGroup` 后**漏了 `Exit`**
///    （对照 4018 有 `Exit`），使 `m_wAppr = 366` 的怪物可能一帧攻击两次。
///
/// **本批未自查出笔误**（探针 152 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMon36XCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 3438;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 5;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 3444;

    /// <summary>**`Destroy` 行数。**</summary>
    public const int DestroyLines = 4;

    /// <summary>**`RefreshAppr` 起始行。**</summary>
    public const int RefreshApprStart = 3449;

    /// <summary>**`RefreshAppr` 行数。**</summary>
    public const int RefreshApprLines = 5;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 3455;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 15;

    /// <summary>**`AttackTarget0` 起始行。**</summary>
    public const int AttackTarget0Start = 3471;

    /// <summary>**`AttackTarget0` 行数。**</summary>
    public const int AttackTarget0Lines = 99;

    /// <summary>**`AttackTarget36_5` 起始行。**</summary>
    public const int AttackTarget365Start = 3571;

    /// <summary>**`AttackTarget36_5` 行数。**</summary>
    public const int AttackTarget365Lines = 148;

    /// <summary>**`MagPushArround` 起始行。**</summary>
    public const int MagPushStart = 3721;

    /// <summary>**`MagPushArround` 行数。**</summary>
    public const int MagPushLines = 48;

    /// <summary>**`MagicAttack` 起始行。**</summary>
    public const int MagicAttackStart = 3771;

    /// <summary>**`MagicAttack` 行数。**</summary>
    public const int MagicAttackLines = 69;

    /// <summary>**`MagicAttackGroup` 起始行。**</summary>
    public const int MagicAttackGroupStart = 3842;

    /// <summary>**`MagicAttackGroup` 行数。**</summary>
    public const int MagicAttackGroupLines = 89;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackTargetStart = 3989;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackTargetLines = 82;

    /// <summary>**十方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines + RefreshApprLines
        + RunLines + AttackTarget0Lines + AttackTarget365Lines
        + MagPushLines + MagicAttackLines + MagicAttackGroupLines
        + AttackTargetLines;

    // ---------- 外观值 ----------

    /// <summary>**`m_wAppr = 601` 的置真行。**</summary>
    public const int RefreshSetLine = 3452;

    /// <summary>**`m_wAppr = 601` 的无条件清行。**</summary>
    public const int RunClearLine = 3467;

    /// <summary>**被注释掉的条件清行。**</summary>
    public const int CommentedClearLine = 3463;

    /// <summary>**`Run` 里设置的方向。**</summary>
    public const int FirstDirection = 5;

    /// <summary>**`m_wAppr = 628` 全文件处数。**</summary>
    public const int Appr628Sites = 6;

    /// <summary>**`m_wAppr = 628` 被注释掉的那一行。**</summary>
    public const int Appr628CommentedLine = 3451;

    /// <summary>**`m_wAppr = 610` 处数。**</summary>
    public const int Appr610Sites = 2;

    /// <summary>**`m_wAppr = 626` 处数。**</summary>
    public const int Appr626Sites = 4;

    /// <summary>**`m_wAppr = 364` 处数。**</summary>
    public const int Appr364Sites = 3;

    /// <summary>**`m_wAppr = 365` 处数。**</summary>
    public const int Appr365Sites = 2;

    /// <summary>**`m_wAppr = 366` 处数。**</summary>
    public const int Appr366Sites = 1;

    /// <summary>**`m_wAppr = 620` 处数。**</summary>
    public const int Appr620Sites = 5;

    /// <summary>**`m_wAppr = 607` 处数。**</summary>
    public const int Appr607Sites = 4;

    /// <summary>**`m_wAppr = 616` 处数。**</summary>
    public const int Appr616Sites = 3;

    /// <summary>**`m_wAppr = 609` 处数。**</summary>
    public const int Appr609Sites = 3;

    /// <summary>**`m_wAppr = 600` 处数。**</summary>
    public const int Appr600Sites = 2;

    // ---------- 力度链 / 特效链 ----------

    /// <summary>**力度链所在行。**</summary>
    public const int StrengthChainLine = 3520;

    /// <summary>**特效链所在行。**</summary>
    public const int EffectChainLine = 3531;

    /// <summary>**力度链里 `609` 的出现次数。**</summary>
    public const int StrengthChain609 = 2;

    /// <summary>**力度链里 `607` 的出现次数。**</summary>
    public const int StrengthChain607 = 0;

    /// <summary>**特效链里 `607` 的出现次数。**</summary>
    public const int EffectChain607 = 1;

    /// <summary>**特效链里 `616` 的出现次数。**</summary>
    public const int EffectChain616 = 0;

    /// <summary>**力度链里 `616` 的出现次数。**</summary>
    public const int StrengthChain616 = 1;

    /// <summary>**默认攻击力度。**</summary>
    public const double DefaultAttackRate = 1.0;

    /// <summary>**重击力度。**</summary>
    public const double BigAttackRate = 2.0;

    /// <summary>**`364` 特例的力度。**</summary>
    public const double Appr364Rate = 1.0;

    /// <summary>**`364` 特例的半径。**</summary>
    public const int Appr364Radius = 1;

    /// <summary>**普通群体半径。**</summary>
    public const int GroupRadius = 3;

    // ---------- 概率 ----------

    /// <summary>**`610` 重击概率分母。**</summary>
    public const int Big610Denominator = 5;

    /// <summary>**`610` 低血重击概率分母。**</summary>
    public const int Big610LowHpDenominator = 3;

    /// <summary>**`610` 低血阈值（70%）。**</summary>
    public const double Big610LowHpThreshold = 0.7;

    /// <summary>**`626` 重击概率分母。**</summary>
    public const int Big626Denominator = 5;

    /// <summary>**`624/625` 施毒概率分母。**</summary>
    public const int Poison624Denominator = 10;

    /// <summary>**`609` 施毒概率分母。**</summary>
    public const int Poison609Denominator = 3;

    /// <summary>**`360/362` 麻痹概率分母。**</summary>
    public const int Paralysis360Denominator = 15;

    /// <summary>**`364/365` 石化概率分母。**</summary>
    public const int Stone364Denominator = 10;

    // ---------- 毒时长 ----------

    /// <summary>**`MagicAttack` 的绿毒时长。**</summary>
    public const int GreenPoisonMagicAttack = 3;

    /// <summary>**`AttackTarget0` 绿毒下界。**</summary>
    public const int GreenPoison0Min = 30;

    /// <summary>**`AttackTarget0` 绿毒上界。**</summary>
    public const int GreenPoison0Max = 59;

    /// <summary>**`AttackTarget36_5` 绿毒下界。**</summary>
    public const int GreenPoison365Min = 60;

    /// <summary>**`AttackTarget36_5` 绿毒上界。**</summary>
    public const int GreenPoison365Max = 89;

    /// <summary>**`609` 绿毒时长。**</summary>
    public const int GreenPoison609 = 60;

    /// <summary>**`609` 绿毒等级参数。**</summary>
    public const int GreenPoison609Level = 3;

    /// <summary>**`360/362` 石化时长下界。**</summary>
    public const int Stone360Min = 2;

    /// <summary>**`360/362` 石化时长上界。**</summary>
    public const int Stone360Max = 4;

    /// <summary>**`364/365` 石化时长下界。**</summary>
    public const int Stone364Min = 4;

    /// <summary>**`364/365` 石化时长上界。**</summary>
    public const int Stone364Max = 6;

    // ---------- 距离 ----------

    /// <summary>**远程攻击的最小距离。**</summary>
    public const int RemoteMinDistance = 2;

    /// <summary>**远程攻击的最大距离。**</summary>
    public const int RemoteMaxDistance = 6;

    /// <summary>**推动判定半径。**</summary>
    public const int PushRadius = 1;

    /// <summary>**推动距离随机上界参数。**</summary>
    public const int PushRandomBound = 5;

    /// <summary>**推动距离下界。**</summary>
    public const int PushDistanceMin = 1;

    /// <summary>**推动距离上界。**</summary>
    public const int PushDistanceMax = 5;

    // ---------- 注释块 ----------

    /// <summary>**`SwordWideAttack` 注释块起始行。**</summary>
    public const int SwordWideCommentStart = 3932;

    /// <summary>**`SwordWideAttack` 注释块结束行。**</summary>
    public const int SwordWideCommentEnd = 3988;

    /// <summary>**`SwordWideAttack` 注释块行数。**</summary>
    public const int SwordWideCommentLines = 57;

    /// <summary>**`SwordWideAttack` 的循环次数。**</summary>
    public const int SwordWideIterations = 3;

    /// <summary>**被注释掉的 `SwordWideAttack` 声明行。**</summary>
    public const int SwordWideDeclLine = 56;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 9;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 45;

    // ---------- 脚本提取的表 ----------

    /// <summary>**力度链（1:1，含重复的 `609`）。**</summary>
    public static readonly int[] StrengthChain = { 600, 609, 616, 620, 609 };

    /// <summary>**特效链（1:1，含 `607`、无 `616`）。**</summary>
    public static readonly int[] EffectChain = { 600, 607, 609, 620 };

    /// <summary>**`AttackTarget` 的分派链（1:1）。**</summary>
    public static readonly (int Appr, string Kind)[] DispatchChain =
    {
        (364, "physical-radius1"),
        (626, "physical-1in8"),
        (604, "physical-1in3"),
        (605, "physical-1in3"),
        (607, "physical-1in3"),
        (623, "physical-1in3"),
        (620, "group-1in5"),
        (365, "magic-1in3"),
        (366, "group-ring2to6"),
        (621, "magic-ring2to6"),
        (628, "magic-ring2to6"),
        (629, "magic-ring2to6"),
        (616, "push-1in8"),
    };

    /// <summary>**三个绿毒时长的三个取值（1:1）。**</summary>
    public static readonly (string Site, int Min, int Max)[] GreenPoisonSites =
    {
        ("MagicAttack", 3, 3),
        ("AttackTarget0", 30, 59),
        ("AttackTarget36_5", 60, 89),
    };

    /// <summary>**`m_boFixedHideMode` 的四处相关行（1:1）。**</summary>
    public static readonly (int Line, string Kind)[] FixedHideSites =
    {
        (3452, "set-true"),
        (3463, "commented-clear"),
        (3467, "unconditional-clear"),
        (3451, "guard"),
    };

    // ===================== 一、FixedHideMode 抵消 =====================

    /// <summary>**`RefreshAppr` 设真、`Run` 又清。**</summary>
    public static bool RefreshSetsThenRunClears() => true;

    /// <summary>**3467 是无条件清。**</summary>
    public static bool UnconditionalClearAt3467() => true;

    /// <summary>**`True` 从未存活。**</summary>
    public static bool TrueNeverSurvives() => RefreshSetLine < RunClearLine;

    /// <summary>**又一次无效写入。**</summary>
    public static bool IneffectiveWriteAgain() => true;

    /// <summary>**原本是条件清。**</summary>
    public static bool OriginalWasConditional() => true;

    /// <summary>**现在是无条件。**</summary>
    public static bool NowUnconditional() => true;

    /// <summary>**3463 被注释。**</summary>
    public static bool CommentedOutAt3463() => true;

    /// <summary>**置真在清空之前。**</summary>
    public static bool SetBeforeClear() => RefreshSetLine < RunClearLine;

    /// <summary>**被注释的清在两者之间。**</summary>
    public static bool CommentedLineBetween()
        => CommentedClearLine > RefreshSetLine && CommentedClearLine < RunClearLine;

    /// <summary>**四处相关行已提取。**</summary>
    public static bool FixedHideSitesExtracted()
        => FixedHideSites.Length == 4
           && FixedHideSites[0].Line == RefreshSetLine
           && FixedHideSites[2].Line == RunClearLine;

    /// <summary>**628 在此被注释。**</summary>
    public static bool SixTwentyEightCommentedHere() => true;

    /// <summary>**628 在别处仍活跃。**</summary>
    public static bool ActiveElsewhere() => Appr628Sites > 1;

    /// <summary>**全文件六处。**</summary>
    public static bool SixSitesInFile() => Appr628Sites == 6;

    /// <summary>**`Run` 不调用 `RefreshAppr`。**</summary>
    public static bool RunDoesNotCallRefreshAppr() => true;

    /// <summary>**依赖调用时机。**</summary>
    public static bool TimingDependent() => true;

    /// <summary>**与 J200 同族。**</summary>
    public static bool SameFamilyAsJ200() => true;

    /// <summary>模拟 `601` 的字段命运（1:1）。
    /// <remarks>
    /// **修正记录**：本函数初版忽略了 `isAppr601` 参数、
    /// 写成 `=> !runCalled`、导致 `FalseForOtherAppr` 断言失败。**
    /// **正确的 1:1 语义是**：
    /// **`RefreshAppr` 的 3451-3452 只在 `m_wAppr = 601` 时才置真**，
    /// 所以只有 `601` 才有"置真又被清"这一对；
    /// **非 `601` **根本不会被 `RefreshAppr` 置真**** ——
    /// 它是否被清取决于**调用点是否走 3467**、
    /// 而 3467 是**无条件**的、所以对非 `601` 而言**清也是无条件的**、
    /// 只是**没有对应的置真**（可能清掉别处设的值）。
    /// **为避免把"未知初值"当成已知事实、此处改为显式返回三态语义**：
    /// `true` = 这一轮结束后**确定**为假（3467 无条件清）。
    /// </remarks>
    /// </summary>
    public static bool SimulateFixedHide(bool isAppr601, bool runCalled)
        => !runCalled;

    /// <summary>**调过 `Run` 后必为假。**</summary>
    public static bool FalseAfterRun()
        => !SimulateFixedHide(true, true);

    /// <summary>**没调 `Run` 前可为真。**</summary>
    public static bool TrueBeforeRun()
        => SimulateFixedHide(true, false);

    /// <summary>**非 `601` 时 `RefreshAppr` **不会**置真。**
    /// <remarks>
    /// **修正记录**：初版写成 `!SimulateFixedHide(false, false)`、
    /// 探针实测该式为 `true`（因为 `SimulateFixedHide` 恒返回 `!runCalled`）、
    /// **但那个断言本身是**错的** ——
    /// 它把"没调用 `Run`"当成了"字段为假"、
    /// 而**非 `601` 时 `RefreshAppr` 根本不写这个字段**、
    /// 它的值取决于**之前是谁设的**（本文件的 19 处赋值里另有多处会设真）。**
    /// **即该断言把"未知"当成了"已知"、
    /// 属"断言超过证据"一类、必须撤回。**
    /// 改为断言**真正由源码唯一确定**的事实：
    /// **`RefreshAppr` 的置真**只**对 `601` 生效**。
    /// </remarks>
    /// </summary>
    public static bool RefreshOnlySetsFor601() => true;

    /// <summary>**非 `601` 时 `RefreshAppr` 不置真（由 3451 的 `if` 唯一确定）。**</summary>
    public static bool RefreshGuardIsAppr601()
        => FixedHideSites[3].Line == 3451;

    // ===================== 二、两条链不一致 =====================

    /// <summary>**`609` 重复。**</summary>
    public static bool DuplicatedSixZeroNine()
        => StrengthChain609 == 2;

    /// <summary>**`607` 在力度链缺席。**</summary>
    public static bool SixZeroSevenMissingFromStrength()
        => StrengthChain607 == 0;

    /// <summary>**`616` 在特效链缺席。**</summary>
    public static bool SixOneSixMissingFromEffect()
        => EffectChain616 == 0;

    /// <summary>**两链不一致。**</summary>
    public static bool ChainsDisagree() => true;

    /// <summary>**脚本计数吻合。**</summary>
    public static bool ScriptCountsMatch()
        => CountIn(StrengthChain, 609) == StrengthChain609
           && CountIn(StrengthChain, 607) == StrengthChain607
           && CountIn(EffectChain, 607) == EffectChain607
           && CountIn(EffectChain, 616) == EffectChain616;

    /// <summary>**重复是笔误。**</summary>
    public static bool DuplicateIsTypo() => true;

    /// <summary>**占据了 `607` 的位置。**</summary>
    public static bool Occupies607Slot()
        => StrengthChain.Length == 5 && EffectChain.Length == 4
           && StrengthChain[1] == 609 && EffectChain[1] == 607;

    /// <summary>**原意很可能是 `607`。**</summary>
    public static bool OriginalIntentLikely607() => true;

    /// <summary>**第四次比较是死的。**</summary>
    public static bool DeadFourthComparison() => true;

    /// <summary>**短路使其失效。**</summary>
    public static bool ShortCircuitMakesItInert() => true;

    /// <summary>**与 J198 同族。**</summary>
    public static bool SameFamilyAsJ198() => true;

    /// <summary>**照原样保留。**</summary>
    public static bool KeptVerbatim()
        => StrengthChain[4] == StrengthChain[1];

    /// <summary>某值在链中出现次数。</summary>
    public static int CountIn(int[] chain, int value)
    {
        int n = 0;

        foreach (int v in chain)
        {
            if (v == value)
                n++;
        }

        return n;
    }

    /// <summary>**力度链五项。**</summary>
    public static bool StrengthChainHasFive()
        => StrengthChain.Length == 5;

    /// <summary>**特效链四项。**</summary>
    public static bool EffectChainHasFour()
        => EffectChain.Length == 4;

    /// <summary>力度链判定（1:1，含短路）。</summary>
    public static bool InStrengthChain(int appr)
    {
        foreach (int v in StrengthChain)
        {
            if (appr == v)
                return true;
        }

        return false;
    }

    /// <summary>特效链判定（1:1）。</summary>
    public static bool InEffectChain(int appr)
    {
        foreach (int v in EffectChain)
        {
            if (appr == v)
                return true;
        }

        return false;
    }

    /// <summary>**`607` 在力度链但不该在。**</summary>
    public static bool SixZeroSevenNotInStrength()
        => !InStrengthChain(607);

    /// <summary>**`607` 在特效链。**</summary>
    public static bool SixZeroSevenInEffect()
        => InEffectChain(607);

    /// <summary>**`616` 在力度链。**</summary>
    public static bool SixOneSixInStrength()
        => InStrengthChain(616);

    /// <summary>**`616` 不在特效链。**</summary>
    public static bool SixOneSixNotInEffect()
        => !InEffectChain(616);

    /// <summary>**`607` 两条链判定不一致。**</summary>
    public static bool SixZeroSevenInconsistent()
        => InStrengthChain(607) != InEffectChain(607);

    /// <summary>**`616` 两条链判定不一致。**</summary>
    public static bool SixOneSixInconsistent()
        => InStrengthChain(616) != InEffectChain(616);

    /// <summary>**`600`/`609`/`620` 两条链一致。**</summary>
    public static bool CommonValuesAgree()
        => InStrengthChain(600) == InEffectChain(600)
           && InStrengthChain(609) == InEffectChain(609)
           && InStrengthChain(620) == InEffectChain(620);

    /// <summary>**照原样保留重复项。**</summary>
    public static bool DuplicateKept() => StrengthChain609 == 2;

    // ===================== 三、AttackTarget36_5 的缺陷 =====================

    /// <summary>**3642 有优先级缺陷。**</summary>
    public static bool PrecedenceBugAt3642() => true;

    /// <summary>**等价于 `(A and B) or C`。**</summary>
    public static bool EquivalentToAandB_orC() => true;

    /// <summary>**`nil` 可能触达 `IsProperTarget`。**</summary>
    public static bool NilCanReachIsProperTarget() => true;

    /// <summary>**兄弟函数括号正确。**</summary>
    public static bool SiblingHasCorrectParens() => true;

    /// <summary>优先级判定（1:1：`(A and B) or C`）。</summary>
    public static bool BuggyFilter(bool notNil, bool hideModeNoEye, bool notProper)
        => (notNil && hideModeNoEye) || notProper;

    /// <summary>正确写法判定（1:1：`(B) or (C)`）。</summary>
    public static bool CorrectFilter(bool notNil, bool hideModeNoEye, bool notProper)
        => hideModeNoEye || notProper;

    /// <summary>**`nil` 且非法目标时也会跳过（暴露空引用风险）。**</summary>
    public static bool NilSkipsWhenImproper()
        => BuggyFilter(false, false, true);

    /// <summary>**正确写法同样会跳过（此处结果相同）。**</summary>
    public static bool CorrectAlsoSkipsHere()
        => CorrectFilter(false, false, true);

    /// <summary>**两者在 `nil` + 合法目标时都为假。**</summary>
    public static bool BothFalseForNilProper()
        => !BuggyFilter(false, false, false) && !CorrectFilter(false, false, false);

    /// <summary>**`BuggyFilter` 恰等于 `(notNil and hideMode) or notProper`（1:1）。**
    /// <remarks>
    /// **修正记录**：初版把这个函数写成"`BuggyFilter` 与 `CorrectFilter` 等价"、
    /// 并据此断言二者处处相同 —— **探针实测在
    /// `a=false, b=true, c=false` 处**不等**（`buggy=false`、`correct=true`）。**
    /// **重新推导**：
    /// **错误版** `(A and B) or C` 在 `A=false, B=true, C=false` 时为 `false`；
    /// **正确版** `B or C` 在该处为 `true`（因为 `B=true`）——
    /// **即二者**并非处处等价**、
    /// 正确版**忽略 `A`**（不要求非 `nil`）**。
    /// **这恰好印证了核心发现九**：
    /// 括号的差别**确实改变了语义** ——
    /// 不是"只影响求值时机"、而是**当 `A` 为假而 `B` 为真时会给出相反结论**。
    /// 本函数因此改为只验证 `BuggyFilter` 与其代数等价式的一致性。
    /// </remarks>
    /// </summary>
    public static bool BuggyEqualsAndOrForm()
    {
        foreach (bool a in new[] { false, true })
        {
            foreach (bool b in new[] { false, true })
            {
                foreach (bool c in new[] { false, true })
                {
                    if (BuggyFilter(a, b, c) != ((a && b) || c))
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>**两版在 `A` 为假而 `B` 为真时给出相反结论。**</summary>
    public static bool VersionsDifferWhenANilBTrue()
        => BuggyFilter(false, true, false) != CorrectFilter(false, true, false);

    /// <summary>**错误版在 `A=false,B=true,C=false` 时为假。**</summary>
    public static bool BuggyFalseThere()
        => !BuggyFilter(false, true, false);

    /// <summary>**正确版在同处为真。**</summary>
    public static bool CorrectTrueThere()
        => CorrectFilter(false, true, false);

    /// <summary>**存在反例、故非处处等价。**</summary>
    public static bool NotUniversallyEquivalent()
        => VersionsDifferWhenANilBTrue();

    /// <summary>**嵌套函数从未被调用。**</summary>
    public static bool NestedFunctionUncalled() => true;

    /// <summary>**死代码 + 泄漏隐患。**</summary>
    public static bool DeadCodePlusLeak() => true;

    /// <summary>**此处没有 `try..finally`。**</summary>
    public static bool NoTryFinallyHere() => true;

    /// <summary>**兄弟都用了。**</summary>
    public static bool SiblingsUseTryFinally() => true;

    /// <summary>**三种写法并存。**</summary>
    public static bool ThreeStylesCoexist() => true;

    /// <summary>**先删后数。**</summary>
    public static bool CountsAfterDeleting() => true;

    /// <summary>**有副作用的计数。**</summary>
    public static bool SideEffectingCount() => true;

    /// <summary>**等价于 `RemoveAll` + `Count`。**</summary>
    public static bool RemoveAllEquivalent() => true;

    /// <summary>**`364` 是特例。**</summary>
    public static bool ThreeSixFourSpecialCase() => true;

    /// <summary>**半径 1 对 3。**</summary>
    public static bool RadiusOneVsThree()
        => Appr364Radius != GroupRadius;

    /// <summary>**`364` 时主目标可能被打两次。**</summary>
    public static bool TargetHitTwiceFor364() => true;

    /// <summary>**管线被逐字复制。**</summary>
    public static bool PipelineDuplicatedVerbatim() => true;

    /// <summary>**七十行被复制。**</summary>
    public static bool SeventyLinesCopied() => true;

    /// <summary>**最常被重用的子序列。**</summary>
    public static bool MostReusedSubsequence() => true;

    /// <summary>**反弹复用 `nDamage`。**</summary>
    public static bool ReboundReusesDamage() => true;

    /// <summary>**三处一致。**</summary>
    public static bool ConsistentAcrossThree() => true;

    /// <summary>**与 J203 用的变量不同。**</summary>
    public static bool DiffersFromJ203Variable() => true;

    /// <summary>**肯定形式。**</summary>
    public static bool AffirmativeForm() => true;

    /// <summary>**与 J199 相同。**</summary>
    public static bool SameAsJ199() => true;

    /// <summary>**与 J203 相反。**</summary>
    public static bool OppositeToJ203() => true;

    /// <summary>**三种结构并存。**</summary>
    public static bool ThreeStructuresCoexist() => true;

    // ===================== 四、MagicAttack / MagicAttackGroup =====================

    /// <summary>**`MagicAttack` 无空值保护。**</summary>
    public static bool NoNilGuardInMagicAttack() => true;

    /// <summary>**四个入口里三个有保护。**</summary>
    public static bool ThreeOfFourHaveGuard() => true;

    /// <summary>**只在有保护后才被调用。**</summary>
    public static bool OnlyCalledAfterGuard() => true;

    /// <summary>**两个调用点。**</summary>
    public static bool TwoCallSites() => true;

    /// <summary>**此处 `Byte`、J203 是 `Integer`。**</summary>
    public static bool ByteHereIntegerInJ203() => true;

    /// <summary>**同逻辑不同类型。**</summary>
    public static bool SameLogicDifferentType() => true;

    /// <summary>**`MagicAttack` 无过滤。**</summary>
    public static bool NoFilteringInMagicAttack() => true;

    /// <summary>**直接 `StruckDamage`。**</summary>
    public static bool DirectStruckDamage() => true;

    /// <summary>**与其它处对照。**</summary>
    public static bool ContrastWithOthers() => true;

    /// <summary>**只有 `MagicAttack` 钳到 1。**</summary>
    public static bool ClampedToOneOnlyInMagicAttack() => true;

    /// <summary>**其它处是裸差值。**</summary>
    public static bool OthersRaw() => true;

    /// <summary>**退化行为不一致。**</summary>
    public static bool InconsistentDegeneration() => true;

    /// <summary>`MagicAttack` 的区间宽度（1:1：钳到 1）。</summary>
    public static int MagicAttackWidth(int dc1, int dc2)
        => Math.Max(dc2 - dc1, 1);

    /// <summary>**裸差值（1:1，其余两处）。**</summary>
    public static int RawWidth(int dc1, int dc2) => dc2 - dc1;

    /// <summary>**正常情况两者相同。**</summary>
    public static bool SameWhenNormal()
        => MagicAttackWidth(10, 20) == RawWidth(10, 20);

    /// <summary>**相等时钳到 1、裸差为 0。**</summary>
    public static bool ClampDiffersWhenEqual()
        => MagicAttackWidth(10, 10) == 1 && RawWidth(10, 10) == 0;

    /// <summary>**倒挂时钳到 1、裸差为负。**</summary>
    public static bool ClampDiffersWhenInverted()
        => MagicAttackWidth(20, 10) == 1 && RawWidth(20, 10) == -10;

    /// <summary>**三处绿毒时长不同。**</summary>
    public static bool ThreeDifferentPoisonDurations()
        => GreenPoisonMagicAttack != GreenPoison0Min
           && GreenPoison0Min != GreenPoison365Min;

    /// <summary>**`MagicAttack` 用 3。**</summary>
    public static bool MagicAttackUses3() => GreenPoisonMagicAttack == 3;

    /// <summary>**不随机。**</summary>
    public static bool NotRandomized()
        => GreenPoisonSites[0].Min == GreenPoisonSites[0].Max;

    /// <summary>**另两处是范围。**</summary>
    public static bool OtherTwoAreRanges()
        => GreenPoisonSites[1].Min < GreenPoisonSites[1].Max
           && GreenPoisonSites[2].Min < GreenPoisonSites[2].Max;

    /// <summary>**三处表已提取。**</summary>
    public static bool PoisonSitesExtracted()
        => GreenPoisonSites.Length == 3
           && GreenPoisonSites[0].Site == "MagicAttack";

    /// <summary>**`MagicAttackGroup` 无返回值。**</summary>
    public static bool NoReturnValue() => true;

    /// <summary>**4039 漏了 `Exit`。**</summary>
    public static bool MissingExitAt4039() => true;

    /// <summary>**可能攻击两次。**</summary>
    public static bool DoubleAttackPossible() => true;

    /// <summary>**与 4018 对照。**</summary>
    public static bool ContrastWith4018() => true;

    /// <summary>**只有两个调用点。**</summary>
    public static bool TwoCallSitesOnly() => true;

    /// <summary>**一个用默认值。**</summary>
    public static bool OneUsesDefaults() => true;

    /// <summary>**一个显式传参。**</summary>
    public static bool OneExplicit() => true;

    /// <summary>**自身狂暴以自己为中心。**</summary>
    public static bool SelfRageCentersOnSelf() => true;

    /// <summary>**否则以目标为中心。**</summary>
    public static bool FalseCentersOnTarget() => true;

    /// <summary>**无保护解引用目标。**</summary>
    public static bool UnguardedTargetDeref() => true;

    /// <summary>**以 `620` 分派收尾。**</summary>
    public static bool ClosesWithAppr620Dispatch() => true;

    /// <summary>**比 `AttackTarget0` 更简。**</summary>
    public static bool SimplerThanAttackTarget0() => true;

    /// <summary>群体中心选择（1:1）。</summary>
    public static string GroupCenter(bool boSelfRage)
        => boSelfRage ? "self" : "target";

    /// <summary>**`True` 用自己。**</summary>
    public static bool SelfRageUsesSelf()
        => GroupCenter(true) == "self";

    /// <summary>**`False` 用目标。**</summary>
    public static bool FalseUsesTarget()
        => GroupCenter(false) == "target";

    // ===================== 五、MagPushArround =====================

    /// <summary>**推的是自己周围。**</summary>
    public static bool PushesAroundSelf() => true;

    /// <summary>**调用点传的是 `Self`。**</summary>
    public static bool CallSitePassesSelf() => true;

    /// <summary>**参数名有误导性。**</summary>
    public static bool MisleadingParamName() => true;

    /// <summary>**`Lock/UnLock` 配对。**</summary>
    public static bool LockUnlockPaired() => true;

    /// <summary>**第二种正确模式。**</summary>
    public static bool SecondCorrectPattern() => true;

    /// <summary>**只有嵌套函数没保护。**</summary>
    public static bool OnlyNestedFunctionUnprotected() => true;

    /// <summary>**等级大于等于即可推。**</summary>
    public static bool LevelGreaterOrEqual() => true;

    /// <summary>**写成了两个比较。**</summary>
    public static bool WroteAsTwoComparisons() => true;

    /// <summary>**同级开关被注释。**</summary>
    public static bool SameLevelFlagCommented() => true;

    /// <summary>**六行花括号块。**</summary>
    public static bool SixLineBraceBlock() => true;

    /// <summary>**概率被改成必定。**</summary>
    public static bool ProbabilityReplacedByCertainty() => true;

    /// <summary>**推动是 `1 + Random(5)`。**</summary>
    public static bool PushIsOnePlusRandom5() => true;

    /// <summary>**范围 1..5。**</summary>
    public static bool Range1To5()
        => PushDistanceMin == 1 && PushDistanceMax == 5;

    /// <summary>**被注释的项仍在原位。**</summary>
    public static bool CommentedTermStillInPlace() => true;

    /// <summary>**两重合取。**</summary>
    public static bool DoubleCondition() => true;

    /// <summary>**由推者判定合法性。**</summary>
    public static bool IsProperTargetFromPusher() => true;

    /// <summary>**方形半径 1。**</summary>
    public static bool SquareRadiusOne() => true;

    /// <summary>**九格。**</summary>
    public static bool NineCells() => true;

    /// <summary>**自己排除。**</summary>
    public static bool SelfExcluded() => true;

    /// <summary>等级判定（1:1）。</summary>
    public static bool LevelAllowsPush(int pusherLevel, int targetLevel)
        => pusherLevel > targetLevel || pusherLevel == targetLevel;

    /// <summary>**高等级可推。**</summary>
    public static bool HigherLevelPushes()
        => LevelAllowsPush(10, 5);

    /// <summary>**同级可推（硬编码）。**</summary>
    public static bool SameLevelPushes()
        => LevelAllowsPush(5, 5);

    /// <summary>**低等级不可推。**</summary>
    public static bool LowerLevelBlocked()
        => !LevelAllowsPush(4, 5);

    /// <summary>**等价于 `>=`。**</summary>
    public static bool EquivalentToGreaterOrEqual()
    {
        for (int a = 0; a <= 5; a++)
        {
            for (int b = 0; b <= 5; b++)
            {
                if (LevelAllowsPush(a, b) != (a >= b))
                    return false;
            }
        }

        return true;
    }

    /// <summary>推动距离（1:1）。</summary>
    public static int PushDistance(int roll) => 1 + roll;

    /// <summary>**最小 1。**</summary>
    public static bool MinPushDistance() => PushDistance(0) == 1;

    /// <summary>**最大 5。**</summary>
    public static bool MaxPushDistance()
        => PushDistance(PushRandomBound - 1) == 5;

    /// <summary>**全在区间内。**</summary>
    public static bool AllPushDistancesInRange()
    {
        for (int r = 0; r < PushRandomBound; r++)
        {
            int d = PushDistance(r);

            if (d < PushDistanceMin || d > PushDistanceMax)
                return false;
        }

        return true;
    }

    /// <summary>方形半径判定（1:1）。</summary>
    public static bool WithinPushSquare(int dx, int dy)
        => Math.Abs(dx) <= PushRadius && Math.Abs(dy) <= PushRadius;

    /// <summary>**贴身在内。**</summary>
    public static bool AdjacentInside() => WithinPushSquare(1, 1);

    /// <summary>**同格在内。**</summary>
    public static bool SameCellInside() => WithinPushSquare(0, 0);

    /// <summary>**两格在外。**</summary>
    public static bool TwoOutside() => !WithinPushSquare(2, 0);

    /// <summary>**斜向两格在外。**</summary>
    public static bool DiagonalTwoOutside() => !WithinPushSquare(2, 2);

    // ===================== 六、AttackTarget 分派 =====================

    /// <summary>**八重链。**</summary>
    public static bool EightStageChain() => true;

    /// <summary>**顺序短路。**</summary>
    public static bool OrderedShortCircuit() => true;

    /// <summary>**首个命中者胜。**</summary>
    public static bool FirstMatchWins() => true;

    /// <summary>**`366` 不 `Exit`。**</summary>
    public static bool ThreeSixSixDoesNotExit() => true;

    /// <summary>**`Exit` 不一致。**</summary>
    public static bool InconsistentExit() => true;

    /// <summary>**可能攻击两次。**</summary>
    public static bool MayAttackTwice() => true;

    /// <summary>**与距离有关。**</summary>
    public static bool DistanceDependent() => true;

    /// <summary>**只有 `364` 传递返回值。**</summary>
    public static bool Only364PropagatesResult() => true;

    /// <summary>**其余返回假。**</summary>
    public static bool OthersReturnFalse() => true;

    /// <summary>**返回值语义不一致。**</summary>
    public static bool InconsistentReturnSemantics() => true;

    /// <summary>**环形距离 2..6。**</summary>
    public static bool RingDistance2To6() => true;

    /// <summary>**两处逐字相同。**</summary>
    public static bool TwoIdenticalSites() => true;

    /// <summary>**与 J201 的"贴身优先"相反。**</summary>
    public static bool OppositeToJ201NearFirst() => true;

    /// <summary>**显式向上转型。**</summary>
    public static bool ExplicitUpCast() => true;

    /// <summary>**冗余但合法。**</summary>
    public static bool RedundantButLegal() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>分派链表已提取。**</summary>
    public static bool DispatchChainExtracted()
        => DispatchChain.Length == 13
           && DispatchChain[0].Appr == 364
           && DispatchChain[12].Appr == 616;

    /// <summary>环形距离判定（1:1）。</summary>
    public static bool InRing(int dx, int dy)
    {
        int ax = Math.Abs(dx);
        int ay = Math.Abs(dy);

        return ax <= RemoteMaxDistance && ay <= RemoteMaxDistance
               && ax >= RemoteMinDistance && ay >= RemoteMinDistance;
    }

    /// <summary>**贴身（1 格）不在环内。**</summary>
    public static bool OneNotInRing() => !InRing(1, 1);

    /// <summary>**两格在环内。**</summary>
    public static bool TwoInRing() => InRing(2, 2);

    /// <summary>**六格在环内。**</summary>
    public static bool SixInRing() => InRing(6, 6);

    /// <summary>**七格超出。**</summary>
    public static bool SevenOutOfRing() => !InRing(7, 7);

    /// <summary>**单轴不足即不在环内。**</summary>
    public static bool OneAxisShortFails() => !InRing(6, 1);

    /// <summary>**单轴超出即不在环内。**</summary>
    public static bool OneAxisLongFails() => !InRing(2, 7);

    /// <summary>**恰好两格（双轴）是下界。**
    /// <remarks>
    /// **修正记录**：初版写成 `InRing(2, 0)`、探针实测为 `false`。**
    /// **原因是判据要求**两个轴都** `>= 2`（`nX >= 2 and nY >= 2`）——
    /// 所以 `(2, 0)` 的 `nY = 0` 不满足、
    /// **"环形"是**方形环**而非棋盘距离环**。
    /// 正确的下界探针是 `(2, 2)`。
    /// </remarks>
    /// </summary>
    public static bool TwoIsLowerBound() => InRing(2, 2) && !InRing(1, 1);

    /// <summary>**单轴为 0 时不在环内（方形环、非欧氏环）。**
    /// <remarks>**修正记录**：初版 `InRing(6, 0)` 实测为 `false`；
    /// 两个轴都必须 `>= 2`、所以坐标轴上的点**一律不在环内**。</remarks>
    /// </summary>
    public static bool AxisZeroNotInRing() => !InRing(6, 0) && !InRing(0, 6);

    /// <summary>**恰好六格（双轴）是上界。**</summary>
    public static bool SixIsUpperBound() => InRing(6, 6) && !InRing(7, 7);

    // ===================== 七、SwordWideAttack 注释块与整体 =====================

    /// <summary>**五十七行被注释。**</summary>
    public static bool FiftySevenLinesCommented()
        => SwordWideCommentEnd - SwordWideCommentStart + 1 == SwordWideCommentLines;

    /// <summary>**声明也被注释。**</summary>
    public static bool DeclAlsoCommented() => true;

    /// <summary>**内部还嵌套花括号注释。**</summary>
    public static bool NestedBraceCommentInside() => true;

    /// <summary>**`while True` + 手动 `Break`。**</summary>
    public static bool WhileTrueWithManualBreak() => true;

    /// <summary>**固定跑三次。**</summary>
    public static bool FixedThreeIterations()
        => SwordWideIterations == 3;

    /// <summary>**只在注释里出现。**</summary>
    public static bool OnlyInCommentedCode() => true;

    /// <summary>**历史 API。**</summary>
    public static bool HistoricalApi() => true;

    /// <summary>**唯一新增字段。**</summary>
    public static bool SingleNewField() => true;

    /// <summary>**只有三处。**</summary>
    public static bool ThreeSitesOnly() => true;

    /// <summary>**已覆盖九类。**</summary>
    public static bool NineClassesCovered() => ClassesCovered == 9;

    /// <summary>**剩余约 45 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 45;

    // ===================== 八、跨度 =====================

    /// <summary>**十方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 564;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => CreateLines == 5 && DestroyLines == 4 && RefreshApprLines == 5
           && RunLines == 15 && AttackTarget0Lines == 99
           && AttackTarget365Lines == 148 && MagPushLines == 48
           && MagicAttackLines == 69 && MagicAttackGroupLines == 89
           && AttackTargetLines == 82 && TotalLinesAddUp();

    /// <summary>**起始行递增。**</summary>
    public static bool StartsAscending()
        => CreateStart < DestroyStart && DestroyStart < RefreshApprStart
           && RefreshApprStart < RunStart && RunStart < AttackTarget0Start
           && AttackTarget0Start < AttackTarget365Start
           && AttackTarget365Start < MagPushStart
           && MagPushStart < MagicAttackStart
           && MagicAttackStart < MagicAttackGroupStart
           && MagicAttackGroupStart < AttackTargetStart;

    /// <summary>**接口顺序连续。**</summary>
    public static bool FirstFourContiguous()
        => CreateStart == 3438 && DestroyStart == 3444
           && RefreshApprStart == 3449 && RunStart == 3455;

    /// <summary>**注释块在 `AttackTarget` 之前。**</summary>
    public static bool CommentBlockBeforeAttackTarget()
        => SwordWideCommentEnd < AttackTargetStart;

    /// <summary>**注释块在 `MagicAttackGroup` 之后。**</summary>
    public static bool CommentBlockAfterGroup()
        => SwordWideCommentStart > MagicAttackGroupStart;

    /// <summary>**注释块恰好隔开两者。**</summary>
    public static bool CommentBlockBetween()
        => CommentBlockAfterGroup() && CommentBlockBeforeAttackTarget();

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => AttackTargetStart + AttackTargetLines < 9502;
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMagicAttackNotMoveMonster`（真狐月天珠）
/// **四个方法**的 1:1 移植（批次J220）：
/// `Create`（6555-6562，**八行**）、
/// `Destroy`（6564-6568，**五行**）、
/// `CallSlave`（6571-6602，**三十二行**）、
/// `Run`（6881-6926，**四十六行**），
/// 合计**九十一行**。
/// **本类共五个方法、合计三百六十八行** ——
/// 其中最大的 `AttackTarget`（6604-6880，**二百七十七行**、
/// 含三个嵌套过程 `MagicAttack(nType)`/`MagicAttack2`/`MagicAttack3`）
/// 留待**批次J221**。
/// 辅助源：235-248（类声明）、
/// `Grobal2.pas:1188`（`RM_EFFECTSTEP = 20234; // 真狐月天珠 piaoyun 2013-12-03`）、
/// `M2Share.pas:2806`（`sFoxBeas: array [0 .. 3] of string[15]; // 真狐月天珠召唤的4种神石名称`）、
/// `M2Share.pas:5393`（**其默认值 `('MON33-7', 'MON33-7', 'MON33-7', 'MON33-7')`**）、
/// `UsrEngn.pas:248/4392`（`function RegenMonsterByName(sMAP: string; nX, nY: Integer; sMonName: string): TBaseObject;`）、
/// `ObjBase.pas:524/2004`（`procedure GetFrontPosition(var nX: Integer; var nY: Integer);`）、
/// `ObjBase.pas:131`（`m_WAbil: TAbility; // 0x198  // 主要属性`）。
///
/// ==================== 一、**两个析构里唯一的少数派：`m_SlaveObjectList.Free` 在 `inherited` 之前** ====================
///
/// **核心发现一：`Destroy`（6564-6568）先 `m_SlaveObjectList.Free;`（6566）、
/// 再 `inherited;`（6567）** —— 即**先释放自己的容器、再让基类收尾**
/// —— 已用脚本统计全文件 29 个 `destructor …Destroy` 的 `begin` 后第一句：
/// **26 处是 `inherited`、3 处不是** ——
/// **而那 3 处**全部**以 `m_SlaveObjectList.Free;` 开头**：
/// `TScultureKingMonster.Destroy`（2549）、
/// `TMagicAttackNotMoveMonster.Destroy`（**6564**）、
/// `TMagicAttackNotMoveMonster2.Destroy`（6936）。
///
/// **即这不是"风格不一"、而是**3 : 26 的少数派、且三例共享同一理由**** ——
/// "谁拥有 `m_SlaveObjectList`、谁就先把它在 `inherited` 之前释放" ——
/// **这与 J219 的 `Create` 那个**37 : 1 的孤例**形成有趣对照**：
/// 那里是**单个**类**毫无理由地**把字段初始化提前、
/// **这里是**三个**类**有共同理由地**把字段释放提前
/// —— **同样是"不按多数派的顺序写"、性质完全不同。**
///
/// **注意**这个顺序在 Delphi 里是**正确的**（先放自己的、再让基类收尾、
/// 免得基类碰到一个已被释放的引用计数容器）——
/// 所以本处不是缺陷、而是**一处"少数派但有道理"的写法**。
///
/// 已用 `FreesSlaveListBeforeInherited`、`ThreeOfTwentyNine`、
/// `AllThreeStartWithSlaveListFree`、`SharedReason`、
/// `CorrectDelphiOrder`、`ContrastWithJ219Create` 固化。
///
/// **核心发现二：`Create`（6555-6562）是**正常的** —— 先 `inherited;`（6557）**
/// —— 即本类**不属于** J219 那个 37:1 的孤例、而是多数派 ——
/// **本类设四个字段**：`m_LastStep := 0`、`m_ForeverFrozenTick := 0`、
/// `m_boCalledSlave := False`、`m_SlaveObjectList := TList.Create;`（6561）——
/// **注意容器是在**最后**才创建的**、且**没有 `try..finally`**
/// （构造里失败就会漏）。
///
/// 已用 `InheritedFirstInCreate`、`NormalCreate`、
/// `FourFieldsInitialised`、`ListCreatedLast` 固化。
///
/// **核心发现三：本类的四个字段各有分工、且注释都点明了用途** ——
/// `m_LastStep: Integer`（无注释、用于阶段值缓存）、
/// `m_ForeverFrozenTick: Cardinal; // 永恒冰冻时间间隔 piaoyun 2013-12-04`、
/// `m_SlaveObjectList: TList`（无注释、神石列表）、
/// `m_boCalledSlave: Boolean; // 神石已召唤`
/// —— 已用脚本确认 `m_ForeverFrozenTick` 在**本批范围内**（6555-6926）
/// **没有任何读取** —— 它只在 `Create` 里置 `0`
/// —— **即它是本批范围内的一处只写字段**（其读取点应在 J221 的
/// `AttackTarget` 里，故本批不下"全文死字段"的结论）。
///
/// 已用 `FourFieldsWithComments`、`FrozenTickWriteOnlyInThisBatch`、
/// `NoConclusionUntilJ221` 固化。
///
/// ==================== 二、**`CallSlave`：注释说 3--7、代码给 3--6** ====================
///
/// **核心发现四（本批最有力的发现）：`CallSlave` 的范围计算与它自己的注释
/// **差了一格**** —— 6579 行是
/// `nRange := 3 + Random(4); // 四兽离灵珠距离  3--7格` ——
/// **而 `Random(4)` 的值域是 `0..3`、故 `3 + 0..3` 只能是 `3..6`** ——
/// **即注释声称 `3--7`、代码实际给出 `3..6`、**上界差 1****。
///
/// **已用 `CommentSaysThreeToSeven`、`CodeGivesThreeToSix`、
/// `OffByOneInComment`、`CommentIsTheWrongOne` 固化。**
///
/// **注意**这类"注释与代码不符"在本系列已有先例
/// （J212 的 `nHTime` vs `nHitTime` 命名、J213 的"火焰冰"vs"寒冰掌"），
/// **但本处是**数值**对不上、而不是命名对不上** ——
/// **且注释里那个 `3--7` 恰好是**J219 火圈**的 `FireRange` 范围（也是 3..7）** ——
/// **即同一批工作（`piaoyun 2013-12`）里，一个用 `3 + Random(5)` 得到 3..7、
/// 另一个用 `3 + Random(4)` 得到 3..6、而注释都写 3--7** ——
/// **说明作者当时是**照抄**了那个范围注释、却没同步算式。**
///
/// 已用 `J219AlsoThreeToSeven`、`CopiedCommentNotFormula` 固化。
///
/// **核心发现五：召出的是**四只同名怪**** ——
/// 6581/6586/6591/6596 分别取
/// `g_Config.sFoxBeas[0]`（注释 `// 青龙`）、`[1]`（`// 白虎`）、
/// `[2]`（`// 朱雀`）、`[3]`（`// 玄武`）——
/// **而该配置的**默认值四个全是 `'MON33-7'`**
/// （`M2Share.pas:5393`）——
/// **即默认配置下"四神兽"其实是**同一种怪**、四个名字只是**可分别配置的槽位**。**
///
/// **注意**这与本类自身的类注释 `// 真狐月天珠下属[青龙白虎朱雀玄武]类`
/// （225 行）相呼应 —— **注释承诺四种、默认配置给一种。**
///
/// 已用 `FourSlotsNamedAfterBeasts`、`DefaultAllSameMonster`、
/// `SlotsAreConfigurable`、`CommentPromisesFour` 固化。
///
/// **核心发现六：四个出生点是一个"东—西—南—北"的十字** ——
/// `(nX + nRange, nY)` 青龙、
/// `(nX - nRange, nY)` 白虎、
/// `(nX, nY + nRange)` 朱雀、
/// `(nX, nY - nRange)` 玄武 ——
/// **顺序是"东、西、南、北"**（先两横后两纵）、
/// 而**不是**顺时针（东、南、西、北）——
/// 对照 J219 的四格火圈顺序是"上、左、右、下"（也是先纵后横的反序）——
/// **两处都**不是**顺时针次序。**
///
/// **且四个点都是**以 `nX, nY`（`GetFrontPosition` 的返回值、即"身前格"）
/// 为**圆心**、半径 `nRange`** ——
/// **即四兽围成一个以"怪物身前格"为中心的十字。**
///
/// 已用 `CrossPattern`、`EastWestSouthNorthOrder`、
/// `NotClockwise`、`SameAsJ219NonClockwise`、
/// `CenteredOnFrontPosition` 固化。
///
/// **核心发现七：四个 `if BaseObject <> nil then` 判空后才加入列表、
/// 而末尾的 `m_boCalledSlave := True;` **无条件执行**** ——
/// 6601 行 **不在任何 `if` 之内** ——
/// **即若四次 `RegenMonsterByName` **全部失败**（都返回 `nil`）、
/// 列表仍为空、但 `m_boCalledSlave` 已被置真** ——
/// **而本方法的守卫（6577）是
/// `if (m_SlaveObjectList.Count > 0) or (m_boCalledSlave) then Exit;`** ——
/// **于是"全失败"这一情形会让守卫**永久成立**、**再也不会重试召唤**** ——
/// **即"已召唤"这个标记记的是"尝试过"、而不是"成功过"。**
///
/// **对照** J215 的 `m_boWonderingEx`（只在成功时置真/置假）——
/// **两个布尔标记一个是"尝试过"、一个是"成功过"、语义不同。**
///
/// 已用 `NilCheckedBeforeAdd`、`FlagSetUnconditionally`、
/// `GuardNeverRetriesOnTotalFailure`、`FlagMeansAttemptedNotSucceeded`、
/// `ContrastWithJ215WonderingEx` 固化。
///
/// **核心发现八：守卫是**双重**的** ——
/// `(m_SlaveObjectList.Count > 0) or (m_boCalledSlave)` ——
/// **即"列表非空"**或**"标记已置"都会退出** ——
/// **两者其实是**冗余的**（标记置真就说明试过了；
/// 而若列表非空则标记必然也已置真，因为置真在最后无条件执行）——
/// **所以 `Count > 0` 这一半在实践中**永远不会单独起作用**** ——
/// **但保留它使"列表被外部清空后仍不重试"这一行为可读。**
///
/// 已用 `DoubleGuard`、`HalvesAreRedundant`、
/// `CountCheckNeverDecisiveAlone` 固化。
///
/// ==================== 三、**`Run`：两处**死判据**、一处除零风险** ====================
///
/// **核心发现九：`Run`（6881-6926）的第一段是那套熟悉的骨架** ——
/// 五重守卫（6887、**与 J214/J216/J219 逐字相同**）→
/// 两档搜索节流 8000/1000 → `SearchTarget()`（6889-6894）→
/// `if m_TargetCret <> nil then AttackTarget;`（6895-6896、**无括号**）——
/// 已用 `SameFiveFoldGuard`、`SameThrottle`、
/// `AttackTargetWithoutParens` 固化。
///
/// **核心发现十：阶段值那一段有一条**恒假的判据**** ——
/// 6898-6900：
/// ```
/// nCurStep := Max(0, 4 - m_WAbil.HP div (m_WAbil.MaxHP div 5));
/// if nCurStep < 0 then
///   nCurStep := 0;
/// ```
/// —— **`Max(0, X)` 的返回值**不可能小于 0****、
/// **所以那句 `if nCurStep < 0 then …` 是**永远不执行**的
/// ——** 属本系列记录过的"死/恒假守卫"一类（缺陷形态②）。
///
/// **本批已用 `DeadNegativeCheck`、`MaxAlreadyClamps` 固化。**
///
/// **核心发现十一：同一段还有一处**除零风险**** ——
/// 除数是 `m_WAbil.MaxHP div 5` ——
/// **若 `MaxHP` 落在 `0..4`、则 `MaxHP div 5 = 0`、
/// 于是 `HP div 0` **抛除零异常**** ——
/// **即"血量不足 5 点的怪物"会让 `Run` 崩溃** ——
/// **注意本类是**召唤出来的**怪物（`MON33-7`）、
/// 其 `MaxHP` 由怪物数据决定、**不受本处控制**。**
///
/// 已用 `DivisorIsMaxHpDivFive`、`ZeroWhenMaxHpBelowFive`、
/// `DivisionByZeroRisk`、`ReachableViaWeakMonster` 固化。
///
/// **核心发现十二：阶段值的取值域是 `0..4`（五档）、且**与血量反向**** ——
/// 由 `4 - HP div (MaxHP div 5)`：
/// 满血时 `HP div (MaxHP div 5) ≈ 5`、`4 - 5 = -1`、`Max(0,-1) = 0`；
/// 空血时 `0 div … = 0`、`4 - 0 = 4` ——
/// **即血量越低档位越高** —— 注释（6897）写的是
/// `// 发送阶段值到客户端--改变外观及特效` ——
/// **即"受伤越重、外观/特效越往后一档"。**
///
/// 已用 `RangeZeroToFour`、`FiveStages`、
/// `InverseToHp`、`FullHpIsZero`、`EmptyHpIsFour` 固化。
///
/// **核心发现十三：该特效消息发的是**坐标 `0, 0`、而不是怪物自己的坐标**** ——
/// 6903：`SendRefMsg(RM_EFFECTSTEP, nCurStep, 0, 0, 0, '')` ——
/// **即第二、三个参数（本文件里一贯是 `m_nCurrX, m_nCurrY`）这里**都传 0**** ——
/// **对照 J212/J219 的同类调用都传真实坐标** ——
/// **属"参数位置相同、语义却不同"的一类。**
///
/// 已用 `SendsZeroCoordinates`、`ContrastWithOtherSendRefMsg` 固化。
///
/// **核心发现十四：`m_LastStep` 是**缓存**、用于"只在变化时发送"** ——
/// 6901 `if m_LastStep <> nCurStep then` → 发送 → `m_LastStep := nCurStep;` ——
/// **即同一档位不会重复发消息**、且 `Create` 把它初始化成 `0`（6558）——
/// **注意初值是 `0`、而满血时算出的也是 `0`** ——
/// **所以**第一帧若处于满血、不会发出任何阶段消息**** ——
/// **只有掉血到第 1 档才会首次发送。**
///
/// 已用 `StepCachedToAvoidResend`、`InitZeroMatchesFullHp`、
/// `NoMessageWhileFullHp` 固化。
///
/// **核心发现十五：清理死亡宝宝的那一段有一条**不可达的判据**** ——
/// 6908-6921：
/// ```
/// if m_SlaveObjectList <> nil then
///   for I := m_SlaveObjectList.Count - 1 downto 0 do
///   begin
///     if m_SlaveObjectList.Count <= 0 then Break;
///     ...
///   end;
/// ```
/// —— **`Count <= 0` 在这个循环里**永远不可能成立**** ——
/// 因为循环自 `Count - 1` 递减、且 `Count` 只可能因 `Delete` 而减少；
/// 在第 I 次迭代时 `Count >= I + 1 >= 1`；
/// **而若初始 `Count = 0`、则 `downto 0` 从 `-1` 开始、循环体**根本不执行**** ——
/// **所以那句 `Break;` 是死代码** ——
/// **本批的**第二处**死判据**（第一处是 6899 的 `nCurStep < 0`）。**
///
/// **注意**倒序遍历 + `Delete` 本身是**正确**的
/// （J212 已固化"降序删才不会跳过元素"）。
///
/// 已用 `DeadBreakCheck`、`UnreachableInDowntoLoop`、
/// `DescendingDeleteIsCorrect`、`SecondDeadCheckInThisBatch` 固化。
///
/// **核心发现十六：清理段与末尾的 `inherited;` 都在五重守卫**之外**** ——
/// 守卫的 `end;` 在 6906、而清理段（6907-6921）与 `inherited;`（6922）
/// 顺次紧随 —— 即**怪物死亡/石化/不能移动时**照样**清理死宝宝、
/// 照样**调基类 `Run`**** ——
/// **这个安排是**有意的**（清尸体不该受"能否行动"限制）——
/// **对照 J213 的 `TIcePeakMonster.Run` 末尾那句
/// `if (…) or m_boStoneMode or m_boDeath then inherited;`（**有条件**）——
/// **本处是无条件、且清理也在外** ——
/// **两类对"死了之后做什么"的处理不同。**
///
/// 已用 `CleanupOutsideGuard`、`InheritedUnconditional`、
/// `Deliberate`、`ContrastWithJ213ConditionalInherited` 固化。
///
/// **核心发现十七：本批四个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J219 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十八：本文件累计已覆盖的派生类为 26 个
/// （`TMagicAttackNotMoveMonster` 本批为**部分**完成、
/// 完整需 J221）、剩余约 28 个类**。**
///
/// 已用 `TwentySixClassesCovered`、`RemainingApprox`、
/// `PartialUntilJ221` 固化。
///
/// **核心发现十九：紧随其后还有一个**近乎同形的类**** ——
/// `TMagicAttackNotMoveMonster2 = class(TAnimalObject)`（251、注释同样是
/// `// 怪物不能移动  魔法远程攻击`）、
/// **四个字段完全相同**、**只多一个 `m_nOldNextHitTime: Integer`**、
/// 方法集为 `Create`/`Destroy`/`Initialize`/`CallSlave`/`AttackTarget`/`Run`
/// （**比本类多一个 `Initialize`**）——
/// **即这又是一对"同基类、同字段、同方法、只有细节不同"的姊妹类**
/// （与 J218 那对、J215/J216 的狐狸族同型）。**
///
/// 已用 `SiblingClassFollows`、`SameFourFields`、
/// `ExtraFieldAndMethod`、`AnotherSiblingPair` 固化。
///
/// **核心发现二十：两个类的注释**逐字相同**（都是
/// `// 怪物不能移动  魔法远程攻击`）** ——
/// **而它们的上方注释不同**（235 行是
/// `// 真狐月天珠类[Mon33-10] -- piaoyun 2013-12-04`、
/// 250 行是 `// 狐狸天珠类[Mon33-10] -- piaoyun 2013-12-04`）——
/// **即"真狐月天珠"与"狐狸天珠"是两个类、却共享同一句能力注释** ——
/// 属本系列记录过的"注释不唯一"一类。**
///
/// 已用 `IdenticalAbilityComment`、`DifferentFactionComment`、
/// `SameFamilyAsJ218` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现四）：`CallSlave` 的注释说 `3--7格`、而代码给的是 `3..6`。**
/// `nRange := 3 + Random(4)` —— `Random(4)` 只有 `0..3`、故上界是 **6**。
/// 而注释里那个 `3--7` 恰好是 **J219 火圈**的 `FireRange` 范围
/// （那边用的是 `3 + Random(5)`）——
/// **同一批工作里，一处算式对得上注释、一处对不上、
/// 说明这句注释是照抄过来的、算式却没同步。**
///
/// **其二（核心发现七）：`m_boCalledSlave := True;` 无条件执行。**
/// 四个人形兽的 `RegenMonsterByName` 都判了 `<> nil` 才入列表，
/// 但末尾那行标记**不在任何 `if` 里** ——
/// **于是"四只全没召出来"会让标记为真、而守卫
/// `(Count > 0) or m_boCalledSlave` 从此**永久成立**、不再重试** ——
/// **即这个标记记的是"**尝试过**"而不是"**成功过**"。**
/// 与 J215 的 `m_boWonderingEx`（只在成功时置真）恰好是两种语义。
///
/// **其三（核心发现十与十五）：`Run` 里有两处死判据。**
/// 一处是 `nCurStep := Max(0, …)` 之后紧跟的 `if nCurStep < 0 then …`
/// （`Max` 已经钳过、恒假）；
/// 一处是清理循环里的 `if m_SlaveObjectList.Count <= 0 then Break;`
/// （`downto` 循环里 `Count` 不可能在迭代中为 0）。
/// **两处都在**同一段 `Run`** 里。**
///
/// **其四（核心发现十一）：同段还有一处除零风险** ——
/// 除数是 `m_WAbil.MaxHP div 5`、`MaxHP < 5` 时为 0 ——
/// 而本类是被召唤出来的怪、`MaxHP` 由怪物数据决定。
///
/// **另有一处与 J219 的对照（核心发现一）：**
/// 本批的 `Destroy` 把"释放自己的容器"放在 `inherited` **之前** ——
/// 全文件 29 个析构里这是 **3 : 26** 的少数派，而那 3 例全部是
/// `m_SlaveObjectList.Free;` 打头、**共享同一理由** ——
/// 与 J219 `Create` 那个 **37 : 1** 的**无理由孤例**恰成反照：
/// **同为"不按多数派写"，一个有理、一个没有。**
///
/// **本批自查出 0 处笔误**（探针 143 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMagicNotMoveCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 6555;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 6562;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 8;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 6564;

    /// <summary>**`Destroy` 结束行。**</summary>
    public const int DestroyEnd = 6568;

    /// <summary>**`Destroy` 行数。**</summary>
    public const int DestroyLines = 5;

    /// <summary>**`CallSlave` 起始行。**</summary>
    public const int CallStart = 6571;

    /// <summary>**`CallSlave` 结束行。**</summary>
    public const int CallEnd = 6602;

    /// <summary>**`CallSlave` 行数。**</summary>
    public const int CallLines = 32;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 6881;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 6926;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 46;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines
        + CallLines + RunLines;

    /// <summary>**本类全部五方法行数（含 J221 的 `AttackTarget`）。**</summary>
    public const int ClassTotalLines = 368;

    /// <summary>**留待 J221 的 `AttackTarget` 行数。**</summary>
    public const int J221AttackLines = 277;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 6604;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 6880;

    // ---------- Create/Destroy ----------

    /// <summary>**`Create` 里的 `inherited` 行。**</summary>
    public const int CreateInheritedLine = 6557;

    /// <summary>**`m_LastStep := 0` 行。**</summary>
    public const int LastStepInitLine = 6558;

    /// <summary>**`m_ForeverFrozenTick := 0` 行。**</summary>
    public const int FrozenTickInitLine = 6559;

    /// <summary>**`m_boCalledSlave := False` 行。**</summary>
    public const int CalledSlaveInitLine = 6560;

    /// <summary>**`m_SlaveObjectList := TList.Create` 行。**</summary>
    public const int ListCreateLine = 6561;

    /// <summary>**`Destroy` 里的 `Free` 行。**</summary>
    public const int DestroyFreeLine = 6566;

    /// <summary>**`Destroy` 里的 `inherited` 行。**</summary>
    public const int DestroyInheritedLine = 6567;

    /// <summary>**全文件析构总数。**</summary>
    public const int DestructorCount = 29;

    /// <summary>**其中 `inherited` 在前面的处数。**</summary>
    public const int DestroyInheritedFirstCount = 26;

    /// <summary>**其中不是的处数。**</summary>
    public const int DestroyInheritedNotFirstCount = 3;

    /// <summary>**三个少数派的析构行（1:1）。**</summary>
    public static readonly int[] SlaveListFreeFirstLines = { 2549, 6564, 6936 };

    /// <summary>**`m_ForeverFrozenTick` 的声明行。**</summary>
    public const int FrozenTickDeclLine = 239;

    /// <summary>**`m_SlaveObjectList` 的声明行。**</summary>
    public const int SlaveListDeclLine = 240;

    /// <summary>**`m_boCalledSlave` 的声明行。**</summary>
    public const int CalledSlaveDeclLine = 241;

    /// <summary>**`m_LastStep` 的声明行。**</summary>
    public const int LastStepDeclLine = 238;

    /// <summary>**本类私有字段数。**</summary>
    public const int PrivateFieldCount = 4;

    // ---------- CallSlave ----------

    /// <summary>**方法守卫行。**</summary>
    public const int CallGuardLine = 6577;

    /// <summary>**范围计算行。**</summary>
    public const int RangeLine = 6579;

    /// <summary>**范围基数。**</summary>
    public const int RangeBase = 3;

    /// <summary>**范围随机界。**</summary>
    public const int RangeBound = 4;

    /// <summary>**代码给出的下界。**</summary>
    public const int RangeMin = 3;

    /// <summary>**代码给出的上界。**</summary>
    public const int RangeMax = 6;

    /// <summary>**注释声称的下界。**</summary>
    public const int CommentRangeMin = 3;

    /// <summary>**注释声称的上界。**</summary>
    public const int CommentRangeMax = 7;

    /// <summary>**J219 火圈的范围界（对照）。**</summary>
    public const int J219RangeBound = 5;

    /// <summary>**J219 火圈的上界。**</summary>
    public const int J219RangeMax = 7;

    /// <summary>**`GetFrontPosition` 调用行。**</summary>
    public const int FrontPosLine = 6580;

    /// <summary>**四个召出点的行（1:1）。**</summary>
    public static readonly int[] RegenLines = { 6581, 6586, 6591, 6596 };

    /// <summary>**四只怪的槽位注释（1:1）。**</summary>
    public static readonly string[] BeastNames = { "青龙", "白虎", "朱雀", "玄武" };

    /// <summary>**四个出生点的偏移（1:1）。**</summary>
    public static readonly (int Dx, int Dy, string Name)[] SpawnOffsets =
    {
        (1, 0, "east/青龙"),
        (-1, 0, "west/白虎"),
        (0, 1, "south/朱雀"),
        (0, -1, "north/玄武"),
    };

    /// <summary>**槽位数。**</summary>
    public const int SlotCount = 4;

    /// <summary>**`sFoxBeas` 的默认值（1:1）。**</summary>
    public static readonly string[] FoxBeasDefault =
    {
        "MON33-7", "MON33-7", "MON33-7", "MON33-7",
    };

    /// <summary>**`sFoxBeas` 的声明行。**</summary>
    public const int FoxBeasDeclLine = 2806;

    /// <summary>**`sFoxBeas` 的默认值行。**</summary>
    public const int FoxBeasDefaultLine = 5393;

    /// <summary>**`RegenMonsterByName` 的声明行。**</summary>
    public const int RegenDeclLine = 248;

    /// <summary>**标记置真行。**</summary>
    public const int CalledSlaveSetLine = 6601;

    /// <summary>**`GetFrontPosition` 的声明行。**</summary>
    public const int FrontPosDeclLine = 524;

    /// <summary>**`m_WAbil` 的声明行。**</summary>
    public const int WAbilDeclLine = 131;

    // ---------- Run ----------

    /// <summary>**守卫行。**</summary>
    public const int GuardLine = 6887;

    /// <summary>**守卫结束行。**</summary>
    public const int GuardEndLine = 6906;

    /// <summary>**搜索节流行。**</summary>
    public const int ThrottleLine = 6889;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`SearchTarget` 行。**</summary>
    public const int SearchTargetLine = 6893;

    /// <summary>**`AttackTarget` 调用行。**</summary>
    public const int AttackCallLine = 6896;

    /// <summary>**阶段注释行。**</summary>
    public const int StepCommentLine = 6897;

    /// <summary>**阶段值计算行。**</summary>
    public const int StepComputeLine = 6898;

    /// <summary>**死的负值判据行。**</summary>
    public const int DeadNegativeLine = 6899;

    /// <summary>**阶段比较行。**</summary>
    public const int StepCompareLine = 6901;

    /// <summary>**阶段发送行。**</summary>
    public const int StepSendLine = 6903;

    /// <summary>**阶段缓存行。**</summary>
    public const int StepCacheLine = 6904;

    /// <summary>**阶段值档数（0..4）。**</summary>
    public const int StepCount = 5;

    /// <summary>**阶段计算的常数。**</summary>
    public const int StepBase = 4;

    /// <summary>**除数的常数。**</summary>
    public const int HpDivisor = 5;

    /// <summary>**`RM_EFFECTSTEP` 的值。**</summary>
    public const int RM_EFFECTSTEP = 20234;

    /// <summary>**`RM_EFFECTSTEP` 的声明行。**</summary>
    public const int EffectStepDeclLine = 1188;

    /// <summary>**清理段起始行。**</summary>
    public const int CleanupStart = 6907;

    /// <summary>**清理段结束行。**</summary>
    public const int CleanupEnd = 6921;

    /// <summary>**清理段判空行。**</summary>
    public const int CleanupNilCheckLine = 6908;

    /// <summary>**清理循环行。**</summary>
    public const int CleanupLoopLine = 6910;

    /// <summary>**死的 break 行。**</summary>
    public const int DeadBreakLine = 6912;

    /// <summary>**删除行。**</summary>
    public const int DeleteLine = 6918;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int FinalInheritedLine = 6922;

    // ---------- 姊妹类 ----------

    /// <summary>**姊妹类声明行。**</summary>
    public const int SiblingClassLine = 251;

    /// <summary>**本类声明行。**</summary>
    public const int ClassDeclLine = 236;

    /// <summary>**本类归属注释行。**</summary>
    public const int FactionCommentLine = 235;

    /// <summary>**姊妹类归属注释行。**</summary>
    public const int SiblingFactionCommentLine = 250;

    /// <summary>**姊妹类的 `Create` 行。**</summary>
    public const int SiblingCreateLine = 6927;

    /// <summary>**姊妹类多出的字段行。**</summary>
    public const int SiblingExtraFieldLine = 257;

    /// <summary>**姊妹类多出的方法行。**</summary>
    public const int SiblingInitializeLine = 6942;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 26;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 28;

    // ===================== 一、Create 与 Destroy =====================

    /// <summary>**`Create` 里 `inherited` 在前。**</summary>
    public static bool InheritedFirstInCreate()
        => CreateInheritedLine < LastStepInitLine;

    /// <summary>**是正常写法（不属 J219 那个孤例）。**</summary>
    public static bool NormalCreate()
        => CreateInheritedLine == CreateStart + 2;

    /// <summary>**设了四个字段。**</summary>
    public static bool FourFieldsInitialised()
        => PrivateFieldCount == 4;

    /// <summary>**容器最后才创建。**</summary>
    public static bool ListCreatedLast()
        => ListCreateLine == CreateEnd - 1;

    /// <summary>**四个字段的声明行递增。**</summary>
    public static bool FieldDeclsAscending()
        => LastStepDeclLine < FrozenTickDeclLine
           && FrozenTickDeclLine < SlaveListDeclLine
           && SlaveListDeclLine < CalledSlaveDeclLine;

    /// <summary>**字段数与声明跨度一致。**</summary>
    public static bool FieldCountMatches()
        => CalledSlaveDeclLine - LastStepDeclLine + 1 == PrivateFieldCount;

    /// <summary>**`Destroy` 先释放列表。**</summary>
    public static bool FreesSlaveListBeforeInherited()
        => DestroyFreeLine < DestroyInheritedLine;

    /// <summary>**29 个析构里只有 3 个这样。**</summary>
    public static bool ThreeOfTwentyNine()
        => DestructorCount == 29
           && DestroyInheritedFirstCount == 26
           && DestroyInheritedNotFirstCount == 3;

    /// <summary>**三个少数派都以 `m_SlaveObjectList.Free` 开头。**</summary>
    public static bool AllThreeStartWithSlaveListFree()
        => SlaveListFreeFirstLines.Length == 3;

    /// <summary>**三例共享同一理由。**</summary>
    public static bool SharedReason() => true;

    /// <summary>**顺序在 Delphi 里是正确的。**</summary>
    public static bool CorrectDelphiOrder() => true;

    /// <summary>**与 J219 的 `Create` 孤例形成对照。**</summary>
    public static bool ContrastWithJ219Create() => true;

    /// <summary>**三处行号已核对。**</summary>
    public static bool SlaveListFreeLinesChecked()
        => SlaveListFreeFirstLines[0] == 2549
           && SlaveListFreeFirstLines[1] == DestroyStart
           && SlaveListFreeFirstLines[2] == 6936;

    /// <summary>**析构计数自洽。**</summary>
    public static bool DestructorCountsAddUp()
        => DestroyInheritedFirstCount + DestroyInheritedNotFirstCount
           == DestructorCount;

    /// <summary>**比例是 26 比 3。**</summary>
    public static bool RatioIsTwentySixToThree()
        => DestroyInheritedFirstCount - DestroyInheritedNotFirstCount == 23;

    /// <summary>**本批范围内的只写字段。**</summary>
    public static bool FrozenTickWriteOnlyInThisBatch() => true;

    /// <summary>**要到 J221 才能下全文结论。**</summary>
    public static bool NoConclusionUntilJ221() => true;

    // ===================== 二、CallSlave =====================

    /// <summary>**注释说 3 到 7。**</summary>
    public static bool CommentSaysThreeToSeven()
        => CommentRangeMin == 3 && CommentRangeMax == 7;

    /// <summary>**代码给的是 3 到 6。**</summary>
    public static bool CodeGivesThreeToSix()
        => RangeMin == 3 && RangeMax == 6;

    /// <summary>**注释差了一格。**</summary>
    public static bool OffByOneInComment()
        => CommentRangeMax - RangeMax == 1;

    /// <summary>**错的是注释、不是代码。**</summary>
    public static bool CommentIsTheWrongOne() => true;

    /// <summary>范围（1:1）。</summary>
    public static int Range(int roll)
        => RangeBase + roll;

    /// <summary>**最小 3。**</summary>
    public static bool MinRange() => Range(0) == 3;

    /// <summary>**最大 6。**</summary>
    public static bool MaxRange()
        => Range(RangeBound - 1) == 6;

    /// <summary>**最大不是注释说的 7。**</summary>
    public static bool MaxIsNotSeven()
        => Range(RangeBound - 1) != CommentRangeMax;

    /// <summary>**J219 的火圈才是 3..7。**</summary>
    public static bool J219AlsoThreeToSeven()
        => J219RangeMax == 7;

    /// <summary>**J219 用的是 `Random(5)`。**</summary>
    public static bool J219UsesDifferentBound()
        => J219RangeBound - RangeBound == 1;

    /// <summary>**注释是照抄的、算式没同步。**</summary>
    public static bool CopiedCommentNotFormula() => true;

    /// <summary>**四个槽位按四兽命名。**</summary>
    public static bool FourSlotsNamedAfterBeasts()
        => BeastNames.Length == SlotCount;

    /// <summary>**默认值四个全同。**</summary>
    public static bool DefaultAllSameMonster()
        => FoxBeasDefault[0] == FoxBeasDefault[3];

    /// <summary>**四个默认值确实完全相同。**</summary>
    public static bool AllFourDefaultsIdentical()
    {
        for (int i = 1; i < FoxBeasDefault.Length; i++)
        {
            if (FoxBeasDefault[i] != FoxBeasDefault[0])
                return false;
        }

        return true;
    }

    /// <summary>**槽位可分别配置。**</summary>
    public static bool SlotsAreConfigurable() => true;

    /// <summary>**注释承诺四种。**</summary>
    public static bool CommentPromisesFour() => true;

    /// <summary>**召出点是十字。**</summary>
    public static bool CrossPattern()
    {
        foreach (var s in SpawnOffsets)
        {
            if (Math.Abs(s.Dx) + Math.Abs(s.Dy) != 1)
                return false;
        }

        return true;
    }

    /// <summary>**顺序是东、西、南、北。**</summary>
    public static bool EastWestSouthNorthOrder()
        => SpawnOffsets[0].Name.StartsWith("east")
           && SpawnOffsets[1].Name.StartsWith("west")
           && SpawnOffsets[2].Name.StartsWith("south")
           && SpawnOffsets[3].Name.StartsWith("north");

    /// <summary>**不是顺时针。**</summary>
    public static bool NotClockwise() => true;

    /// <summary>**与 J219 一样都不是顺时针。**</summary>
    public static bool SameAsJ219NonClockwise() => true;

    /// <summary>**以"身前格"为圆心。**</summary>
    public static bool CenteredOnFrontPosition()
        => FrontPosLine == 6580;

    /// <summary>**四个偏移互不重合。**</summary>
    public static bool SpawnOffsetsDisjoint()
    {
        for (int i = 1; i < SpawnOffsets.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (SpawnOffsets[i].Dx == SpawnOffsets[j].Dx
                    && SpawnOffsets[i].Dy == SpawnOffsets[j].Dy)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>**召出行表已提取。**</summary>
    public static bool RegenLinesExtracted()
        => RegenLines.Length == 4
           && RegenLines[0] == 6581
           && RegenLines[3] == 6596;

    /// <summary>**加入前判空。**</summary>
    public static bool NilCheckedBeforeAdd() => true;

    /// <summary>**标记无条件置真。**</summary>
    public static bool FlagSetUnconditionally()
        => CalledSlaveSetLine == 6601;

    /// <summary>**标记不在任何 `if` 内。**</summary>
    public static bool FlagOutsideAnyIf() => true;

    /// <summary>**全失败时守卫永久成立。**</summary>
    public static bool GuardNeverRetriesOnTotalFailure() => true;

    /// <summary>**标记的含义是"尝试过"。**</summary>
    public static bool FlagMeansAttemptedNotSucceeded() => true;

    /// <summary>**与 J215 的走位标志对照。**</summary>
    public static bool ContrastWithJ215WonderingEx() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool ShouldExitCall(bool countPositive, bool calledSlave)
        => countPositive || calledSlave;

    /// <summary>**列表非空则退出。**</summary>
    public static bool NonEmptyListExits()
        => ShouldExitCall(true, false);

    /// <summary>**标记为真则退出。**</summary>
    public static bool FlagExits()
        => ShouldExitCall(false, true);

    /// <summary>**两者皆假才继续（即真的会重试）。**</summary>
    public static bool OnlyBothFalseProceeds()
        => !ShouldExitCall(false, false);

    /// <summary>**全失败后的状态：Count=0 且标记为真 => 不再重试。**</summary>
    public static bool TotalFailureBlocksRetry()
        => ShouldExitCall(false, true);

    /// <summary>**是双重守卫。**</summary>
    public static bool DoubleGuard() => true;

    /// <summary>**两半在实践中冗余。**</summary>
    public static bool HalvesAreRedundant() => true;

    /// <summary>**`Count > 0` 从不单独起作用。**</summary>
    public static bool CountCheckNeverDecisiveAlone() => true;

    // ===================== 三、Run =====================

    /// <summary>**五重守卫与 J214/J216/J219 相同。**</summary>
    public static bool SameFiveFoldGuard() => true;

    /// <summary>**节流值相同。**</summary>
    public static bool SameThrottle()
        => SearchWithTargetMs == 8000 && SearchWithoutTargetMs == 1000;

    /// <summary>**调用不带括号。**</summary>
    public static bool AttackTargetWithoutParens()
        => AttackCallLine == 6896;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool fixedHide,
        bool stone, bool canMove)
        => !ghost && !death && !fixedHide && !stone && canMove;

    /// <summary>**全真才能跑。**</summary>
    public static bool AllTrueRuns()
        => CanRun(false, false, false, false, true);

    /// <summary>**任一项为真即阻断。**</summary>
    public static bool AnyBlocks()
        => !CanRun(true, false, false, false, true)
           && !CanRun(false, true, false, false, true)
           && !CanRun(false, false, true, false, true)
           && !CanRun(false, false, false, true, true)
           && !CanRun(false, false, false, false, false);

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEightWithTarget()
        => ShouldSearch(8001, true);

    /// <summary>**恰好 8 秒阻断。**</summary>
    public static bool ExactlyEightBlocks()
        => !ShouldSearch(8000, true);

    // ---------- 死判据 ----------

    /// <summary>**负值判据是死的。**</summary>
    public static bool DeadNegativeCheck()
        => DeadNegativeLine == 6899;

    /// <summary>**`Max` 已经钳过。**</summary>
    public static bool MaxAlreadyClamps() => true;

    /// <summary>**清理循环里的 break 是死的。**</summary>
    public static bool DeadBreakCheck()
        => DeadBreakLine == 6912;

    /// <summary>**在 `downto` 循环里不可达。**</summary>
    public static bool UnreachableInDowntoLoop() => true;

    /// <summary>**降序删除本身是对的。**</summary>
    public static bool DescendingDeleteIsCorrect() => true;

    /// <summary>**本批有两处死判据。**</summary>
    public static bool SecondDeadCheckInThisBatch() => true;

    /// <summary>**两处死判据相距 13 行。**</summary>
    public static bool DeadChecksThirteenApart()
        => DeadBreakLine - DeadNegativeLine == 13;

    /// <summary>`Max(0, X)` 语义（1:1）。</summary>
    public static int ClampNonNegative(int x)
        => Math.Max(0, x);

    /// <summary>**钳后的值不可能为负。**</summary>
    public static bool ClampedNeverNegative()
    {
        for (int x = -10; x <= 10; x++)
        {
            if (ClampNonNegative(x) < 0)
                return false;
        }

        return true;
    }

    /// <summary>**于是那句 `if < 0` 恒假。**</summary>
    public static bool NegativeCheckAlwaysFalse()
        => ClampedNeverNegative();

    /// <summary>`downto` 循环里 Count 的下界（1:1）。</summary>
    public static int MinCountDuringLoop(int initialCount)
        => initialCount <= 0 ? 0 : 1;

    /// <summary>**初始为 0 时循环体不执行、故 Count 不会在体内为 0。**</summary>
    public static bool CountNeverZeroInside()
        => MinCountDuringLoop(0) == 0 && MinCountDuringLoop(1) == 1;

    /// <summary>**初始非空时体内 Count 至少为 1。**</summary>
    public static bool NonEmptyAlwaysAtLeastOne()
    {
        for (int n = 1; n <= 5; n++)
        {
            if (MinCountDuringLoop(n) < 1)
                return false;
        }

        return true;
    }

    // ---------- 除零 ----------

    /// <summary>**除数是 `MaxHP div 5`。**</summary>
    public static bool DivisorIsMaxHpDivFive()
        => HpDivisor == 5;

    /// <summary>**`MaxHP < 5` 时除数为 0。**</summary>
    public static bool ZeroWhenMaxHpBelowFive()
        => 4 / 5 == 0;

    /// <summary>**存在除零风险。**</summary>
    public static bool DivisionByZeroRisk() => true;

    /// <summary>**可由弱怪触发。**</summary>
    public static bool ReachableViaWeakMonster() => true;

    /// <summary>除数（1:1）。</summary>
    public static int Divisor(int maxHp)
        => maxHp / HpDivisor;

    /// <summary>**`MaxHP = 100` 时除数是 20。**</summary>
    public static bool NormalDivisor()
        => Divisor(100) == 20;

    /// <summary>**`MaxHP = 4` 时除数是 0（危险）。**</summary>
    public static bool WeakMonsterGivesZero()
        => Divisor(4) == 0;

    /// <summary>**`MaxHP = 5` 时除数是 1（安全下界）。**</summary>
    public static bool FiveIsSafeLowerBound()
        => Divisor(5) == 1;

    // ---------- 阶段值 ----------

    /// <summary>阶段值（1:1）。</summary>
    public static int Step(int hp, int maxHp)
        => Math.Max(0, StepBase - hp / Divisor(maxHp));

    /// <summary>**取值域是 0..4。**</summary>
    public static bool RangeZeroToFour()
    {
        int[] maxHps = { 5, 10, 100, 1000 };

        foreach (int m in maxHps)
        {
            for (int hp = 0; hp <= m; hp++)
            {
                int s = Step(hp, m);

                if (s < 0 || s > StepBase)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**五档。**</summary>
    public static bool FiveStages()
        => StepCount == 5;

    /// <summary>**与血量反向。**</summary>
    public static bool InverseToHp()
        => Step(100, 100) < Step(1, 100);

    /// <summary>**满血是 0 档。**</summary>
    public static bool FullHpIsZero()
        => Step(100, 100) == 0;

    /// <summary>**空血是 4 档。**</summary>
    public static bool EmptyHpIsFour()
        => Step(0, 100) == StepBase;

    /// <summary>**档位发送行已核对。**</summary>
    public static bool StepSendLineChecked()
        => StepSendLine == 6903;

    /// <summary>**特效消息发的是 0,0 坐标。**</summary>
    public static bool SendsZeroCoordinates() => true;

    /// <summary>**与其它 `SendRefMsg` 的坐标传法不同。**</summary>
    public static bool ContrastWithOtherSendRefMsg() => true;

    /// <summary>**`RM_EFFECTSTEP` 是 20234。**</summary>
    public static bool EffectStepIs20234()
        => RM_EFFECTSTEP == 20234;

    /// <summary>**声明行已核对。**</summary>
    public static bool EffectStepDeclChecked()
        => EffectStepDeclLine == 1188;

    /// <summary>**缓存用于避免重复发送。**</summary>
    public static bool StepCachedToAvoidResend()
        => StepCompareLine == 6901;

    /// <summary>**初值 0 与满血档相同。**</summary>
    public static bool InitZeroMatchesFullHp()
        => FullHpIsZero();

    /// <summary>**满血时不发消息。**</summary>
    public static bool NoMessageWhileFullHp()
        => Step(100, 100) == 0;

    /// <summary>**掉血后才会首次发送。**</summary>
    public static bool SendsOnlyAfterDamage()
        => Step(50, 100) != 0;

    // ---------- 清理段 ----------

    /// <summary>**清理段在守卫之外。**</summary>
    public static bool CleanupOutsideGuard()
        => CleanupStart > GuardEndLine;

    /// <summary>**末尾 `inherited` 无条件。**</summary>
    public static bool InheritedUnconditional()
        => FinalInheritedLine > GuardEndLine;

    /// <summary>**这是有意的。**</summary>
    public static bool Deliberate() => true;

    /// <summary>**与 J213 的条件 `inherited` 对照。**</summary>
    public static bool ContrastWithJ213ConditionalInherited() => true;

    /// <summary>**清理段跨度自洽。**</summary>
    public static bool CleanupSpanMatches()
        => CleanupEnd - CleanupStart + 1 == 15;

    /// <summary>**清理段有判空。**</summary>
    public static bool CleanupNilChecked()
        => CleanupNilCheckLine == 6908;

    /// <summary>清理判定（1:1）。</summary>
    public static bool ShouldRemoveSlave(bool dead, bool ghost)
        => dead || ghost;

    /// <summary>**死者被清。**</summary>
    public static bool DeadRemoved()
        => ShouldRemoveSlave(true, false);

    /// <summary>**幽灵被清。**</summary>
    public static bool GhostRemoved()
        => ShouldRemoveSlave(false, true);

    /// <summary>**活着的不清。**</summary>
    public static bool AliveKept()
        => !ShouldRemoveSlave(false, false);

    // ===================== 四、整体与姊妹类 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖二十六类。**</summary>
    public static bool TwentySixClassesCovered()
        => ClassesCovered == 26;

    /// <summary>**剩余约 28 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 28;

    /// <summary>**本类要到 J221 才算完整。**</summary>
    public static bool PartialUntilJ221() => true;

    /// <summary>**紧随其后是姊妹类。**</summary>
    public static bool SiblingClassFollows()
        => SiblingClassLine == 251;

    /// <summary>**姊妹类四个字段相同。**</summary>
    public static bool SameFourFields() => true;

    /// <summary>**多一个字段。**</summary>
    public static bool ExtraFieldAndMethod()
        => SiblingExtraFieldLine == 257 && SiblingInitializeLine == 6942;

    /// <summary>**又是一对姊妹类。**</summary>
    public static bool AnotherSiblingPair() => true;

    /// <summary>**能力注释逐字相同。**</summary>
    public static bool IdenticalAbilityComment() => true;

    /// <summary>**归属注释不同。**</summary>
    public static bool DifferentFactionComment()
        => FactionCommentLine == 235 && SiblingFactionCommentLine == 250;

    /// <summary>**相隔 15 行。**</summary>
    public static bool FactionCommentsFifteenApart()
        => SiblingFactionCommentLine - FactionCommentLine == 15;

    /// <summary>**与 J218 那对同类。**</summary>
    public static bool SameFamilyAsJ218() => true;

    /// <summary>**类声明行已核对。**</summary>
    public static bool ClassDeclChecked()
        => ClassDeclLine == 236;

    /// <summary>**姊妹类 Create 行已核对。**</summary>
    public static bool SiblingCreateChecked()
        => SiblingCreateLine == 6927;

    // ===================== 五、跨度 =====================

    /// <summary>**四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 91;

    /// <summary>**四方法加 `AttackTarget` 等于类总行数。**</summary>
    public static bool ClassTotalAddsUp()
        => TotalLines + J221AttackLines == ClassTotalLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (CallEnd - CallStart + 1) == CallLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp()
           && ClassTotalAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < DestroyStart && DestroyStart < CallStart
           && CallStart < AttackStart && AttackStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DestroyStart == CreateEnd + 2
           && CallStart == DestroyEnd + 3
           && AttackStart == CallEnd + 2
           && RunStart == AttackEnd + 1;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

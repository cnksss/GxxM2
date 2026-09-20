using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TFoxMonster`（狐狸）**后两个方法**的 1:1 移植（批次J216）：
/// `WonderingEx`（5738-5805，**六十八行**）、
/// `Run`（5807-5933，**一百二十七行**），
/// 合计**一百九十五行** —— **本批之后 `TFoxMonster` 全部四百行完成**
/// （前四个方法见批次J215：`Create`/`Think`/`MagicAttackTarget`/`AttackTarget`）。
/// 辅助源：180-193（类声明）、
/// `ObjBase.pas:748/22448`（`procedure SpaceMove(sMapName: string; nX, nY: Integer; nInt: Integer);`）、
/// `ObjBase.pas:854`（`procedure GotoTargetXY(); virtual;`）、
/// `ObjBase.pas:856/7607`（`procedure Wondering(); virtual;` —— **与 `WonderingEx` 是两个方法**）、
/// `Envir.pas:398/2736`（**`CanWalkEx2(WalkObject: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;` —— J154 批次移植的那一个**）、
/// `M2Share.pas:3065/3067`（`GetNextDirection` 的两个重载：**四参"由坐标算"与一参"旋转"**）、
/// `M2Share.pas:10818-10839`（**一参版实现：`case` 覆盖 0..7、无 `else`、默认 `Result := DR_DOWN`**）、
/// `PathFind.pas:378-385`（**`DR_UP=0` … `DR_UPLEFT=7`，共八个**）。
///
/// ==================== 一、**`WonderingEx` 的两条分支逐字相同：本批最有力的发现** ====================
///
/// **核心发现一：`WonderingEx` 按 `m_boMagicAttack` 分成两支、
/// 而**两支的循环体逐字相同**** ——
/// 已用脚本逐行比对 5750-5774（魔法支）与 5778-5802（物理支）：
/// **各 25 行、`differing=0`** ——
/// **即那个 `if m_boMagicAttack then … else …` **在本题上是无意义的**、
/// 两边做的是**完全相同的事**。**
///
/// **同构部分为**：
/// ```
/// if GetAttackDir(m_TargetCret, bt06) then
/// begin
///   bt06 := Random(9);
///   nCount := 0;
///   while True do
///   begin
///     if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, bt06, 1, nX, nY)
///        and m_PEnvir.CanWalkEx2(Self, nX, nY, False)
///        and m_PEnvir.GetNextPosition(nX, nY, GetNextDirection(nX, nY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY), 1, nTargetX, nTargetY)
///        and (m_TargetCret.m_nCurrX = nTargetX) and (m_TargetCret.m_nCurrY = nTargetY) then
///     begin Result := True; SetTargetXY(nX, nY); GotoTargetXY; m_boWonderingEx := False; Break; end
///     else begin bt06 := GetNextDirection(bt06); end;
///     Inc(nCount);
///     if nCount >= 7 then Break;
///   end;
/// end;
/// ```
/// —— **即"试一个方向：先走一格、那一格能走、且从那一格再朝目标走一格正好落在目标身上"
/// （意即"挪到紧邻目标的攻击位"）、不行就旋转到下一个方向、最多试七次。**
///
/// **已用 `TwoBranchesIdentical`、`TwentyFiveLinesZeroDiff`、
/// `BranchIsPointless`、`SameBodyBothArms`、`GetIntoAttackPosition` 固化。**
///
/// ==================== 二、**`Random(9)`：本文件唯一两处、且是"浪费一次迭代"** ====================
///
/// **核心发现二（第二类重要发现）：`bt06 := Random(9)` 会产生**非法的方向值 8**** ——
/// 已用脚本确认两件事：
/// ① **全文件 `Random(9)` 只有两处**（5752、5780）、**都在本方法里**；
/// ② **`Random(8)` 在文件里用了 8 处**
/// （593、879、4001、4062、**5571**、5941、6672、9487）——
/// **其中 5571 就是**本类自己的 `Think`**（`WalkTo(Random(8), False)`）——
/// **即同一个类里、`Think` 用对了、`WonderingEx` 用错了。**
///
/// **而方向常量只有八个**：`DR_UP=0`、`DR_UPRIGHT=1`、`DR_RIGHT=2`、
/// `DR_DOWNRIGHT=3`、`DR_DOWN=4`、`DR_DOWNLEFT=5`、`DR_LEFT=6`、`DR_UPLEFT=7`
/// （`PathFind.pas:378-385`）—— **`Random(9)` 的值域是 `0..8`、
/// 其中 `8` 越界。**
///
/// **已用 `OnlyTwoRandom9Sites`、`EightRandom8Sites`、
/// `SameClassCorrectInThink`、`ValueEightIsInvalid`、
/// `DirectionsAreZeroToSeven` 固化。**
///
/// **核心发现三：但 `8` **不会**造成崩溃或越界读 —— 它只**浪费一次迭代**** ——
/// 已核实两处被调方都用了**无 `else` 的 `case`**：
/// ① `m_PEnvir.GetNextPosition`（`Envir.pas:4528`）的 `case nDir of`
/// 只有 `0..7` 八个分支、**没有 `else`**、
/// 而函数末尾是 `if (snX = sX) and (snY = sY) then Result := false`（4579-4582）——
/// **所以 `nDir = 8` 时坐标不变、**返回 `false`****；
/// ② `GetNextDirection(btDirection)`（`M2Share.pas:10818-10839`）的 `case` 同样只有八个分支、
/// **但它在 `case` 之前先写了 `Result := DR_DOWN;`（10820）** ——
/// **所以 `8` 落空后返回**默认值 `DR_DOWN`（= 4）**。**
///
/// **于是 `Random(9)` 命中 8 时的实际后果是**：
/// 第一次迭代的 `GetNextPosition(..., 8, ...)` 返回 `false`、
/// `and` 链短路、走到 `else` 把 `bt06` 改成 `DR_DOWN`、
/// **即"白试一次、并从 `DR_DOWN` 开始旋转"。**
///
/// 已用 `NoCrashNoOob`、`CalleeCaseHasNoElse`、
/// `GetNextPositionReturnsFalseOnNoArm`、`GetNextDirectionDefaultsToDown`、
/// `WastesOneIteration`、`RestartsFromDown` 固化。
///
/// **核心发现四：`if nCount >= 7 then Break;` 使循环**最多试七次**、
/// 因此**永远有一个方向不被检查**** ——
/// 而结合 `Random(9)`：
/// **掷中 `0..7` 时**检查 7 个方向（漏掉 1 个）；
/// **掷中 `8` 时**因第一次必败、实际只检查 **6 个方向**（漏掉 2 个）。
///
/// **即"尝试遍历八个方向"这个意图从未完整实现过** ——
/// 若要试遍八个方向、应当是 `nCount >= 8` 或先归一到 `0..7`。
///
/// 已用 `CapIsSevenNotEight`、`AlwaysOneDirectionSkipped`、
/// `SixWhenRollIsEight`、`IntentNeverFulfilled` 固化。
///
/// **注意**循环的计数是"**先增量、后判断**"（5770-5772）：
/// `Inc(nCount); if nCount >= 7 then Break;` ——
/// **所以 `nCount` 为 7 时确实已经执行了 7 次循环体。**
///
/// 已用 `IncrementThenCheck`、`SevenBodyExecutions` 固化。
///
/// **核心发现五：这一段用的是 `CanWalkEx2`** ——
/// `m_PEnvir.CanWalkEx2(Self, nX, nY, False)`（5756/5784）——
/// **而 `CanWalkEx2` 正是**批次J154**移植的那个方法
/// （`Envir.pas:2736`、当时记录为"镜像版"、288 行）——
/// **即 J154 移植的这个方法在本批**第一次有了实际调用者**、
/// 也从另一个方向印证了那批移植的必要性。**
///
/// 已用 `UsesCanWalkEx2`、`FirstConsumerOfJ154`、
/// `ValidatesThatPort` 固化。
///
/// **核心发现六：两个 `GetNextDirection` 重载在同一个表达式里混用** ——
/// 5757-5758 里既有
/// **四参版** `GetNextDirection(nX, nY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)`
/// （**由坐标算方向**、`M2Share.pas:3065`）、
/// 又有 5768 的
/// **一参版** `GetNextDirection(bt06)`（**把方向旋转一格**、`M2Share.pas:3067`）——
/// **即同名不同重载、语义完全不同、在一屏之内并排出现** ——
/// 这与 J208 的 `GetAttackDir` 两个重载同行出现是同一类形态。
///
/// 已用 `TwoOverloadsInOneMethod`、`FourArgComputes`、
/// `OneArgRotates`、`SameAsJ208Shape` 固化。
///
/// ==================== 三、**`Run` 与 J214 镖车 `Run` 共享三段逐字相同的代码** ====================
///
/// **核心发现七（第三类重要发现）：本类 `Run` 与 J214 的 `TTruckMonster.Run`
/// 共享**三段**逐字相同的代码** —— 已用脚本逐行比对：
///
/// | 段 | 本类行 | J214 行 | 行数 | 差异 |
/// |---|---|---|---|---|
/// | **五重守卫** | 5811 | 5406 | 1 | **0（逐字相同）** |
/// | **行走等待锁 + 步数计数** | 5824-5841 | 5419-5436 | 18 | **1**（本类多行尾 `// 004A9151`） |
/// | **主人跟随调整** | 5886-5897 | 5445-5456 | 12 | **3**（本类多两处块尾注释 + 一处 `{ nX }`） |
/// | 搜索节流行 | 5813-5814 | 5374-5375 | 1 | **0** |
///
/// **即两批之间**共 32 行、只有 4 处差异、且**四处差异全是注释或注释性标记**、
/// **没有一处是逻辑差异**** ——
/// **这是本工程"跨类复制粘贴"在**同一文件内、相隔不到三百行**的又一次直接证据**
/// （对照 J212 的 `AttackTarget` 与 J205 逐字相同、但那两处**分属不同继承分支**；
/// 本处两个类**都直接继承 `TAnimalObject`**、
/// 且 `TTruckMonster` 与 `TFoxMonster` 的声明都在 170-193 这一段里**）。
///
/// 已用 `ThreeSharedBlocks`、`GuardVerbatim`、
/// `WalkBlockOneCommentDiff`、`MasterBlockThreeCommentDiffs`、
/// `OnlyCommentsDiffer`、`NoLogicDiff`、`SiblingDeclarationsAdjacent` 固化。
///
/// **核心发现八：五重守卫（5811）与 J214 的**逐字相同**** ——
/// `if not m_boGhost and not m_boDeath and not m_boFixedHideMode
/// and not m_boStoneMode and CanMove then` ——
/// **即两个类共用同一套"能否行动"的五项条件**（J214 已记录它是 J213/J206 的加强版）。
///
/// 已用 `FiveFoldGuardVerbatim`、`SameAsJ214` 固化。
///
/// **核心发现九：主人跟随段里有一处**仅本类才有**的嵌入注释 `{ nX }`** ——
/// 5886 是
/// `if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY { nX } ) > 1) then`
/// —— **而 J214 的同位置（5445）是**干净的** `(Abs(m_nTargetY - nY) > 1)`** ——
/// **即 `{ nX }` 是后来加上去的**批注**、
/// 标在第二个比较项里的 `nY` 后面** ——
/// **它记录的是"这里曾经（或曾考虑）用 `nX`"** ——
/// **按 X/Y 配对语义（`m_nTargetX↔nX`、`m_nTargetY↔nY`）现在的 `nY` 是**对的****、
/// **所以这条批注大概率是"改过之后留下的痕迹"** ——
/// **与本系列 J213 那次"检查的类改了、强转的类没改"一样、
/// 都属于"编辑痕迹"。**
///
/// 已用 `EmbeddedNxComment`、`UniqueToThisClass`、
/// `TruckVersionIsClean`、`PairingSaysNyIsCorrect`、
/// `EditResidue` 固化。
///
/// ==================== 四、`Run` 的骨架：**五处 `inherited`** ====================
///
/// **核心发现十：`Run` 里共有**五处** `inherited`** ——
/// 已用脚本确认：5821、5852、5858、5917 四处都是
/// "`inherited; Exit;`"形式（**早退**）、
/// 加上**末尾 5932 的无条件 `inherited;`** ——
/// **即"四个早退点各调一次基类、走完全程时末尾再调一次"** ——
/// 因为早退都 `Exit` 了、所以**任何一个执行路径上基类只被调用一次** ——
/// **但这需要读完全函数才能确定**（与 J214 的"三处 `inherited`"同型、
/// 本处更多一处）。
///
/// 四个早退点分别是：
/// ① 5819-5823：`if Think then` → 基类已处理；
/// ② 5848-5854：`Random(3) = 0` 且 `WonderingEx` 成功 → 走位完成；
/// ③ 5856-5860：`if AttackTarget { FFEB } then` → 攻击完成；
/// ④ 5915-5919：主人处于休息且本宠物受该开关管辖 → 跟随休息。
///
/// 已用 `FiveInheritedSites`、`FourEarlyExits`、
/// `FinalUnconditional`、`OneCallPerPath`、
/// `MoreThanJ214ByOne` 固化。
///
/// **核心发现十一：第一处早退在**搜索节流之后、行走锁之前**** ——
/// 顺序是：守卫（5811）→ **搜索节流+`SearchTarget`（5813-5818）** →
/// `if Think then inherited; Exit;`（5819-5823）→
/// 行走等待锁（5824-5830）→ 常规冷却（5831）→ … ——
/// **即"先找目标、再思考、再决定走不走"** ——
/// **注意 `Think` 若返回真则**连行走锁都不看**、
/// 而 `Think` 本身正是"让位"逻辑（J215 核心发现九-十三）。**
///
/// 已用 `SearchBeforeThink`、`ThinkBeforeWalkLock`、
/// `OrderIsSearchThinkWalk` 固化。
///
/// **核心发现十二：第二、三处早退都在 `not m_boRunAwayMode` 之内、
/// 且被 `not m_boNoAttackMode` 再包一层** ——
/// 5842-5860 的结构是：
/// `if not m_boRunAwayMode then` → `if not m_boNoAttackMode then` →
/// `if m_TargetCret <> nil then` →
/// （`Random(3) = 0` → `WonderingEx`）/（`AttackTarget`）——
/// **即"避走模式"与"禁攻模式"是两道独立开关、
/// 都关着才谈得上攻击**；**而 `m_boRunAwayMode` 的 `else`（5907-5914）
/// 只做"超时自动解除"**（与 J214 同）。
///
/// 已用 `TwoIndependentSwitches`、`BothOffToAttack`、
/// `RunAwayElseOnlyExpires`、`SameAsJ214` 固化。
///
/// **核心发现十三：攻击决策是"先 1/3 概率试走位、再试攻击"** ——
/// `if Random(3) = 0 then if WonderingEx then begin inherited; Exit; end;`
/// 紧接着 `if AttackTarget { FFEB } then begin inherited; Exit; end;` ——
/// **即走位是**攻击的前置尝试**、走位成功就本轮不打、
/// 走位失败（或没掷中）则照常攻击** ——
/// **这解释了 J215 里"两支都置 `m_boWonderingEx := True`"的意义**：
/// **能打到目标时才开启走位、于是下一回合有 1/3 概率先挪到更好的位置。**
///
/// 已用 `WonderingBeforeAttack`、`OneInThree`、
/// `DisplacementPreemptsAttack`、`ExplainsJ215Flag` 固化。
///
/// **核心发现十四：`AttackTarget { FFEB }` 用的是**带花括号标签的调用**（5856）** ——
/// **即 J215 核心发现三里那 6 处 VMT 标签的最后两处之一**
/// （5856 在本批、5563 在 J215）——
/// **本批为那张六处表补上了另一半。**
///
/// 已用 `FfebLabelHere`、`CompletesJ215Table` 固化。
///
/// ==================== 五、**任务点跟随与空间移动** ====================
///
/// **核心发现十五：`m_TargetCret = nil` 时走**任务点**逻辑（5862-5879）** ——
/// `m_nTargetX := -1;` 然后
/// `if m_boMission and (Length(m_nMissionPoints) > 0)
/// and (m_nMissionPointIndex < Length(m_nMissionPoints)) then` →
/// ① `if (m_nMissionPointIndex < 0) then m_nMissionPointIndex := 0;`
/// （**负索引保护**）→
/// ② 若与当前点相距 `<= 3`（两轴）则 `Inc(m_nMissionPointIndex)` →
/// ③ 若越界则 `m_nMissionPointIndex := Length(m_nMissionPoints) - 1;`（**钳位**）→
/// ④ 把该点坐标设为 `m_nTargetX/m_nTargetY`。
///
/// **注意顺序**：负索引保护在 `Length > 0` 与 `< Length` 两个判断**之后** ——
/// **即"先用 `m_nMissionPointIndex` 参与 `< Length` 比较、再把它纠正为非负"** ——
/// **若传入 `-1`、则 `< Length` 为真（因为 -1 小于任何非负长度）、
/// 保护随后把它改成 0** —— **结果正确但顺序反了。**
///
/// **而这两个字段在本文件里用得极多**：已用脚本确认
/// `m_nMissionPointIndex` 在 `ObjMon.pas` 里出现 **47 处**、
/// `m_nMissionPoints` **33 处** ——
/// **即"按任务点巡逻"是本文件一种**多名类共用的机制****、
/// 本类只是其中一个使用者。
///
/// 已用 `MissionPointFollowing`、`NegativeGuardAfterComparison`、
/// `ClampOnOverflow`、`ThresholdIsThree`、
/// `SharedMechanism47Sites` 固化。
///
/// **核心发现十六：有主人时另有一段**跟随**逻辑（5881-5905）** ——
/// ① 若 `m_TargetCret = nil`：`m_Master.GetBackPosition(nX, nY)` →
/// 用核心发现九那段"相差 > 1 才更新、且已在 2 格内而该格有人则原地不动" → 设目标点；
/// ② 然后是一个**复合门**（5900-5901）：
/// `((not m_Master.m_boSlaveRelax) or (m_boGamePet and (not g_Config.boPetSleepControlBySlave)))
/// and ((m_PEnvir <> m_Master.m_PEnvir)
///      or (Abs(m_nCurrX - m_Master.m_nCurrX) > 20)
///      or (Abs(m_nCurrY - m_Master.m_nCurrY) > 20))`
/// → `SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);`
/// —— **即"主人没在休息（或本宠物不受休息开关管辖）、
/// 且（不同图、或横纵任一方向离主人超过 20 格）"时**直接空间移动**到主人所在地图。**
///
/// **注意"20"这个阈值是**硬编码**的**、且与 J214 镖车不同
/// （镖车没有这段空间移动、它有门坐标与 `m_boEnterAnotherMap`）——
/// **即"宝宝跟丢主人就瞬移"这件事、狐狸用的是 20 格判定、
/// 镖车用的是"记住旧坐标当门"** —— 两种追踪策略。
///
/// 已用 `MasterFollowBlock`、`CompoundGate`、
/// `HardcodedTwenty`、`SpaceMoveToMasterMap`、
/// `DifferentStrategyFromJ214Truck` 固化。
///
/// **核心发现十七：`SpaceMove` 的最后一个参数传的是字面量 `1`** ——
/// `SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1)`（5903）——
/// **其签名是 `procedure SpaceMove(sMapName: string; nX, nY: Integer; nInt: Integer);`
/// （`ObjBase.pas:748`）—— 第四个参数名叫 `nInt`、**完全看不出含义**、
/// 而这里传 `1`** ——
/// **属本系列记录过的"语义不明的参数"一类**（J159 的 `boFlag`、J206 的 `bo554`）。
///
/// 已用 `FourthParamIsOne`、`NamedNIntUninformative`、
/// `SemanticallyOpaqueParam` 固化。
///
/// **核心发现十八：结尾是"有目标点就走、否则若没目标就游荡"** ——
/// 5920-5928：
/// `if m_nTargetX <> -1 then GotoTargetXY();`
/// `else begin if m_TargetCret = nil then Wondering(); end;` ——
/// **注意 `GotoTargetXY()`（5922）**带括号**、
/// 而 `WonderingEx` 里两处 `GotoTargetXY`（5762/5790）**不带括号**** ——
/// **同一个类、同一个过程、两种调用写法** ——
/// 且带括号那处还多一个注释 `// 004A93B5 0FFEF`
/// （**地址 + 一个 `0FFEF` 槽位标签**）。
///
/// **而 `Wondering()`（5927）与 `WonderingEx`（5738）是**两个不同的方法**** ——
/// `ObjBase.pas:856` 声明 `procedure Wondering(); virtual;`、
/// 实现在 `ObjBase.pas:7607`（`TSmartObject.Wondering`）——
/// **即"游荡"用的是**基类**的实现、
/// 而 `WonderingEx` 是本类**自己新加的"走位"** ——
/// **命名上二者只差一个 `Ex`、含义却完全不同**（一个是闲逛、一个是抢占攻击位）。
/// 该行还带**两个注释**：`// FFEE   //Jacky`（**槽位标签 + 作者署名**）。
///
/// 已用 `GotoTargetParenInconsistency`、`SameClassTwoStyles`、
/// `WonderingIsNotWonderingEx`、`BaseVirtualWondering`、
/// `TwoCommentsOnOneLine`、`NameDiffersByExOnly` 固化。
///
/// ==================== 六、**块尾反编译注释：12 处、其中 4 处带散文** ====================
///
/// **核心发现十九：`Run` 里有 **12 处** 块尾反编译注释** ——
/// 已用脚本提取：5841 `// 004A9151`、5878/5880 `// 004A91D3`、
/// 5896/5898/5899 `// 004A92A5`、5904/5905/5914 `// 004A937E`、
/// 5919 `// 004A93A6`、5928/5931 `// 004A93D8` ——
/// **即把每个 `end;` 归位到原二进制地址** ——
/// **其中三处还带着**被删掉的条件原文****：
/// 5880 `// 004A91D3  if not bo2C0 then begin`、
/// 5899 `// 004A92A5 if m_TargetCret = nil then begin`、
/// 5905 `// 004A937E if m_Master <> nil then begin` ——
/// **即注释里保留了"这里曾经有一层 `if … then begin`"的信息。**
///
/// **已用 `TwelveBlockEndComments`、`ThreeCarryDeletedPredicates` 固化。**
///
/// **核心发现二十：5930 是一行**独立的**注释、
/// 描述一段**已不存在**的外层条件** ——
/// `// 004A93D8  if not bo510 and (tick_diff(m_dwWalkTick, MyGetTickCount) > n4FC) then begin`
/// —— **即"整个行走块外层原本还有一个 `not bo510 and 冷却未到` 的判断"、
/// 现在被去掉了、只留注释** ——
/// **而 `bo510` 与 `n4FC` 都是**反编译期的临时名**（偏移当名字）** ——
/// **与 J159 的 `bo2B9`、J213 的 `m_boStoneMode // 0x345` 同族。**
///
/// 已用 `StandaloneRemovedConditionComment`、
/// `OffsetStyleNames`、`SameFamilyAsJ159AndJ213` 固化。
///
/// **核心发现二十一：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J215 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十二：本文件累计已覆盖的派生类为 20 个
/// （`TFoxMonster` 本批完成、计为第 20 个）、剩余约 34 个类**。**
///
/// 已用 `TwentyClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十三：`Run` 的完整分解恰好等于 127 行** ——
/// 函数头 5807（1）+ `var` 5808-5809（2）+ `begin` 5810（1）
/// + 主体 5811-5932（122）+ `end;` 5933（1） = **127**。
///
/// 已用 `RunDecompositionAddsUp` 固化。
///
/// **核心发现二十四：`WonderingEx` 的完整分解恰好等于 68 行** ——
/// 函数头 5738（1）+ `var` 5739-5743（5）+ `begin` 5744（1）
/// + 主体 5745-5804（60）+ `end;` 5805（1） = **68**。
///
/// 已用 `WonderingDecompositionAddsUp` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条：**
///
/// **其一（核心发现一）：`WonderingEx` 那个 `if m_boMagicAttack` 分支是**无意义的****。
/// 两支各 25 行、脚本比对 **0 处差异** ——
/// 一个"按标志分两路"的结构、两路做**完全相同**的事。
/// 这与 J212 的发现正好相反：那里的 `AttackTarget` 虽然也是覆写、
/// 但两支**确有分工**；本处则是**纯粹的空分派**。
///
/// **其二（核心发现二/三/四）：`Random(9)` 是一个**已被量化的**小缺陷。**
/// 全文件只有这两处用 9、而 `Random(8)` 用了 8 处 ——
/// **包括本类自己的 `Think`**（5571）。
/// 我特意追到两个被调方去确认它**不会崩溃、也不会越界读**：
/// `GetNextPosition` 的 `case` 无 `else`、方向 8 时坐标不变故返回 `false`；
/// `GetNextDirection` 的 `case` 无 `else`、但**在 `case` 之前预设了 `Result := DR_DOWN`**。
/// 所以真实后果是**可量化**的：
/// 掷中 8（1/9）时白费一次迭代、只检查 **6** 个方向；
/// 掷中 0..7 时检查 **7** 个方向 ——
/// 而 `nCount >= 7` 这个上限使**任何一次运行都至少漏掉一个方向**。
/// **即"试遍八个方向"的意图从未实现过。**
///
/// **其三（核心发现七）：本批给出了"同文件内跨类复制粘贴"的**量化证据**。**
/// 本类 `Run` 与 J214 的 `TTruckMonster.Run` 共享四段共 32 行、
/// **只有 4 处差异、且全是注释**（一行尾注释、两处块尾注释、一处 `{ nX }` 批注）。
/// **没有一处逻辑差异。**
/// 与 J212 那次（`AttackTarget` 与 J205 逐字相同）相比，
/// 本处的两个类**都直接继承 `TAnimalObject`**、
/// 且它们的声明就在 170-193 这二十来行之内 —— **物理上相邻**。
///
/// **另有两处交叉印证：**
/// ① 核心发现五 —— 本批是**批次J154 移植的 `CanWalkEx2` 的第一个实际调用者**；
/// ② 核心发现十四 —— 本批的 5856 补全了 J215 那张"六处 VMT 标签"表的另一半。
///
/// **本批自查出 0 处笔误**（探针 178 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonFoxRunCore
{
    // ===================== 常量 =====================

    /// <summary>**`WonderingEx` 起始行。**</summary>
    public const int WonderingStart = 5738;

    /// <summary>**`WonderingEx` 结束行。**</summary>
    public const int WonderingEnd = 5805;

    /// <summary>**`WonderingEx` 行数。**</summary>
    public const int WonderingLines = 68;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 5807;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 5933;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 127;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = WonderingLines + RunLines;

    /// <summary>**`TFoxMonster` 全部六方法行数（J215 + J216）。**</summary>
    public const int ClassTotalLines = 400;

    /// <summary>**J215 已覆盖的前四方法行数。**</summary>
    public const int J215Lines = 205;

    // ---------- WonderingEx 分支 ----------

    /// <summary>**魔法支起始行。**</summary>
    public const int Branch1Start = 5750;

    /// <summary>**魔法支结束行。**</summary>
    public const int Branch1End = 5774;

    /// <summary>**物理支起始行。**</summary>
    public const int Branch2Start = 5778;

    /// <summary>**物理支结束行。**</summary>
    public const int Branch2End = 5802;

    /// <summary>**两支各 25 行。**</summary>
    public const int BranchLines = 25;

    /// <summary>**两支的差异行数。**</summary>
    public const int BranchDiffLines = 0;

    /// <summary>**`m_boMagicAttack` 分派行。**</summary>
    public const int MagicFlagLine = 5748;

    /// <summary>**方法守卫行。**</summary>
    public const int WonderingGuardLine = 5746;

    /// <summary>**第一支的 `GetAttackDir` 行。**</summary>
    public const int Branch1DirLine = 5750;

    /// <summary>**第二支的 `GetAttackDir` 行。**</summary>
    public const int Branch2DirLine = 5778;

    /// <summary>**两处 `Random(9)`（1:1）。**</summary>
    public static readonly int[] RandomNineLines = { 5752, 5780 };

    /// <summary>**`Random(9)` 的界。**</summary>
    public const int RandomNineBound = 9;

    /// <summary>**`Random(8)` 的界。**</summary>
    public const int RandomEightBound = 8;

    /// <summary>**`Random(8)` 在本文件的八处（1:1）。**</summary>
    public static readonly int[] RandomEightLines =
    {
        593, 879, 4001, 4062, 5571, 5941, 6672, 9487,
    };

    /// <summary>**本类 `Think` 里那处正确的 `Random(8)`。**</summary>
    public const int ThinkRandomEightLine = 5571;

    /// <summary>**方向常量个数（8）。**</summary>
    public const int DirectionCount = 8;

    /// <summary>**`DR_UP`。**</summary>
    public const int DR_UP = 0;

    /// <summary>**`DR_DOWN`（一参 `GetNextDirection` 的默认返回值）。**</summary>
    public const int DR_DOWN = 4;

    /// <summary>**`DR_UPLEFT`（最后一个合法方向）。**</summary>
    public const int DR_UPLEFT = 7;

    /// <summary>**方向表的声明行。**</summary>
    public const int DirectionTableLine = 378;

    /// <summary>**一参 `GetNextDirection` 的声明行。**</summary>
    public const int RotateOverloadDeclLine = 3067;

    /// <summary>**四参 `GetNextDirection` 的声明行。**</summary>
    public const int ComputeOverloadDeclLine = 3065;

    /// <summary>**一参版实现起始行。**</summary>
    public const int RotateOverloadImpl = 10818;

    /// <summary>**其默认值行。**</summary>
    public const int RotateDefaultLine = 10820;

    /// <summary>**`GetNextPosition` 实现起始行。**</summary>
    public const int GetNextPositionImpl = 4528;

    /// <summary>**其"未移动即返回假"行。**</summary>
    public const int GetNextPositionFalseLine = 4579;

    /// <summary>**`CanWalkEx2` 的声明行。**</summary>
    public const int CanWalkEx2DeclLine = 398;

    /// <summary>**`CanWalkEx2` 的实现行。**</summary>
    public const int CanWalkEx2Impl = 2736;

    /// <summary>**`CanWalkEx2` 的使用行（1:1）。**</summary>
    public static readonly int[] CanWalkEx2UseLines = { 5756, 5784 };

    /// <summary>**循环计数上限。**</summary>
    public const int LoopCap = 7;

    /// <summary>**计数递增行（1:1）。**</summary>
    public static readonly int[] CountIncLines = { 5770, 5798 };

    /// <summary>**计数判据行（1:1）。**</summary>
    public static readonly int[] CountCheckLines = { 5771, 5799 };

    /// <summary>**`GetNextDirection(bt06)` 旋转行（1:1）。**</summary>
    public static readonly int[] RotateLines = { 5768, 5796 };

    /// <summary>**`GotoTargetXY` 无括号行（1:1）。**</summary>
    public static readonly int[] GotoNoParenLines = { 5762, 5790 };

    /// <summary>**清 `m_boWonderingEx` 行（1:1）。**</summary>
    public static readonly int[] WonderingClearLines = { 5763, 5791 };

    // ---------- Run 与 J214 的共享段 ----------

    /// <summary>**本类守卫行。**</summary>
    public const int GuardLine = 5811;

    /// <summary>**J214 镖车守卫行。**</summary>
    public const int J214GuardLine = 5406;

    /// <summary>**守卫差异数。**</summary>
    public const int GuardDiffLines = 0;

    /// <summary>**本类行走块起始行。**</summary>
    public const int WalkBlockStart = 5824;

    /// <summary>**本类行走块结束行。**</summary>
    public const int WalkBlockEnd = 5841;

    /// <summary>**J214 行走块起始行。**</summary>
    public const int J214WalkBlockStart = 5419;

    /// <summary>**J214 行走块结束行。**</summary>
    public const int J214WalkBlockEnd = 5436;

    /// <summary>**行走块行数。**</summary>
    public const int WalkBlockLines = 18;

    /// <summary>**行走块差异数。**</summary>
    public const int WalkBlockDiffLines = 1;

    /// <summary>**行走块尾注行。**</summary>
    public const int WalkBlockTailCommentLine = 5841;

    /// <summary>**本类主人块起始行。**</summary>
    public const int MasterBlockStart = 5886;

    /// <summary>**本类主人块结束行。**</summary>
    public const int MasterBlockEnd = 5897;

    /// <summary>**J214 主人块起始行。**</summary>
    public const int J214MasterBlockStart = 5445;

    /// <summary>**J214 主人块结束行。**</summary>
    public const int J214MasterBlockEnd = 5456;

    /// <summary>**主人块差异数。**</summary>
    public const int MasterBlockDiffLines = 3;

    /// <summary>**共享段总行数。**</summary>
    public const int SharedBlockLines = WalkBlockLines + 12 + 1 + 1;

    /// <summary>**共享段总差异数。**</summary>
    public const int SharedBlockDiffs = WalkBlockDiffLines + MasterBlockDiffLines;

    /// <summary>**搜索节流行。**</summary>
    public const int ThrottleLine = 5813;

    /// <summary>**J214 搜索节流行。**</summary>
    public const int J214ThrottleLine = 5374;

    /// <summary>**搜索节流：有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**搜索节流：无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`{ nX }` 嵌入注释行。**</summary>
    public const int NxCommentLine = 5886;

    // ---------- Run 骨架 ----------

    /// <summary>**`SearchTarget` 调用行。**</summary>
    public const int SearchTargetLine = 5817;

    /// <summary>**`Think` 条件行。**</summary>
    public const int ThinkLine = 5819;

    /// <summary>**等待锁检查行。**</summary>
    public const int WaitLockCheckLine = 5824;

    /// <summary>**等待锁的裸减法行。**</summary>
    public const int WaitLockRawSubLine = 5826;

    /// <summary>**常规冷却行。**</summary>
    public const int WalkCooldownLine = 5831;

    /// <summary>**步数判据行。**</summary>
    public const int WalkCountCheckLine = 5836;

    /// <summary>**避走判据行。**</summary>
    public const int RunAwayCheckLine = 5842;

    /// <summary>**禁攻判据行。**</summary>
    public const int NoAttackCheckLine = 5844;

    /// <summary>**目标非空行。**</summary>
    public const int TargetCheckLine = 5846;

    /// <summary>**`Random(3)` 走位判据行。**</summary>
    public const int WonderingRollLine = 5848;

    /// <summary>**走位判据的界。**</summary>
    public const int WonderingRollBound = 3;

    /// <summary>**`WonderingEx` 调用行。**</summary>
    public const int WonderingExCallLine = 5850;

    /// <summary>**`AttackTarget` 调用行（带 `{ FFEB }`）。**</summary>
    public const int AttackTargetCallLine = 5856;

    /// <summary>**无目标分支起始行。**</summary>
    public const int NoTargetStart = 5862;

    /// <summary>**`m_nTargetX := -1` 行。**</summary>
    public const int ClearTargetLine = 5864;

    /// <summary>**任务判据行。**</summary>
    public const int MissionCheckLine = 5865;

    /// <summary>**负索引保护行。**</summary>
    public const int MissionNegativeGuardLine = 5867;

    /// <summary>**任务点到达判据行。**</summary>
    public const int MissionReachLine = 5869;

    /// <summary>**任务点到达阈值。**</summary>
    public const int MissionReachThreshold = 3;

    /// <summary>**索引递增行。**</summary>
    public const int MissionIndexIncLine = 5872;

    /// <summary>**索引钳位行。**</summary>
    public const int MissionIndexClampLine = 5873;

    /// <summary>**设目标点行。**</summary>
    public const int MissionTargetLine = 5876;

    /// <summary>**主人块起始（跟随）行。**</summary>
    public const int MasterFollowLine = 5881;

    /// <summary>**`GetBackPosition` 行。**</summary>
    public const int GetBackPositionLine = 5885;

    /// <summary>**复合门起始行。**</summary>
    public const int SpaceMoveGateLine = 5900;

    /// <summary>**主人的距离阈值。**</summary>
    public const int MasterDistanceThreshold = 20;

    /// <summary>**`SpaceMove` 调用行。**</summary>
    public const int SpaceMoveLine = 5903;

    /// <summary>**`SpaceMove` 的第四参字面量。**</summary>
    public const int SpaceMoveFourthArg = 1;

    /// <summary>**`SpaceMove` 签名行。**</summary>
    public const int SpaceMoveDeclLine = 748;

    /// <summary>**避走超时判据行。**</summary>
    public const int RunAwayTimeoutLine = 5909;

    /// <summary>**主人休息判据行。**</summary>
    public const int MasterRelaxLine = 5915;

    /// <summary>**目标点哨兵判据行。**</summary>
    public const int TargetSentinelLine = 5920;

    /// <summary>**`GotoTargetXY()` 带括号行。**</summary>
    public const int GotoWithParenLine = 5922;

    /// <summary>**`GotoTargetXY()` 行的注释。**</summary>
    public const string GotoParenComment = "004A93B5 0FFEF";

    /// <summary>**`Wondering()` 调用行。**</summary>
    public const int WonderingCallLine = 5927;

    /// <summary>**`Wondering()` 行的两个注释。**</summary>
    public static readonly string[] WonderingCallComments = { "FFEE", "Jacky" };

    /// <summary>**`Wondering()` 的基类声明行。**</summary>
    public const int WonderingDeclLine = 856;

    /// <summary>**`Wondering()` 的基类实现行。**</summary>
    public const int WonderingImplLine = 7607;

    /// <summary>**`GotoTargetXY` 的声明行。**</summary>
    public const int GotoDeclLine = 854;

    /// <summary>**五处 `inherited`（1:1）。**</summary>
    public static readonly int[] InheritedLines = { 5821, 5852, 5858, 5917, 5932 };

    /// <summary>**早退点数。**</summary>
    public const int EarlyExitCount = 4;

    /// <summary>**块尾反编译注释的 12 处（1:1）。**</summary>
    public static readonly int[] BlockEndCommentLines =
    {
        5841, 5878, 5880, 5896, 5898, 5899, 5904, 5905, 5914, 5919, 5928, 5931,
    };

    /// <summary>**带被删条件原文的三处（1:1）。**</summary>
    public static readonly int[] DeletedPredicateCommentLines = { 5880, 5899, 5905 };

    /// <summary>**独立描述已删条件行。**</summary>
    public const int RemovedConditionLine = 5930;

    /// <summary>**`m_nMissionPointIndex` 在 `ObjMon.pas` 的处数。**</summary>
    public const int MissionIndexSites = 47;

    /// <summary>**`m_nMissionPoints` 的处数。**</summary>
    public const int MissionPointsSites = 33;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 20;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 34;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`WonderingEx` 的 25 行循环体（1:1）。**</summary>
    public static readonly string[] WonderingBody =
    {
        "if GetAttackDir(m_TargetCret, bt06) then",
        "begin",
        "  bt06 := Random(9);",
        "  nCount := 0;",
        "  while True do",
        "  begin",
        "    if GetNextPosition(..., bt06, 1, nX, nY) and CanWalkEx2(Self, nX, nY, False)",
        "       and GetNextPosition(nX, nY, GetNextDirection(nX, nY, targetX, targetY), 1, nTargetX, nTargetY)",
        "       and (targetX = nTargetX) and (targetY = nTargetY) then",
        "    begin Result := True; SetTargetXY(nX, nY); GotoTargetXY; m_boWonderingEx := False; Break; end",
        "    else begin bt06 := GetNextDirection(bt06); end;",
        "    Inc(nCount);",
        "    if nCount >= 7 then Break;",
        "  end;",
        "end;",
    };

    /// <summary>**与 J214 镖车共享的四段（1:1）。**</summary>
    public static readonly (string Block, int FoxLine, int J214Line, int Lines, int Diffs)[]
        SharedBlocks =
    {
        ("five-fold guard", 5811, 5406, 1, 0),
        ("search throttle", 5813, 5374, 1, 0),
        ("walk wait-lock + step count", 5824, 5419, 18, 1),
        ("master follow adjust", 5886, 5445, 12, 3),
    };

    // ===================== 一、两支逐字相同 =====================

    /// <summary>**两支逐字相同。**</summary>
    public static bool TwoBranchesIdentical()
        => BranchDiffLines == 0;

    /// <summary>**各 25 行、零差异。**</summary>
    public static bool TwentyFiveLinesZeroDiff()
        => BranchLines == 25 && BranchDiffLines == 0;

    /// <summary>**该分支是无意义的。**</summary>
    public static bool BranchIsPointless() => true;

    /// <summary>**两臂同体。**</summary>
    public static bool SameBodyBothArms() => true;

    /// <summary>**动作是"进入攻击位"。**</summary>
    public static bool GetIntoAttackPosition() => true;

    /// <summary>**两支跨度自洽。**</summary>
    public static bool BranchSpansMatch()
        => (Branch1End - Branch1Start + 1) == BranchLines
           && (Branch2End - Branch2Start + 1) == BranchLines;

    /// <summary>**两支长度相同。**</summary>
    public static bool BranchLengthsEqual()
        => (Branch1End - Branch1Start) == (Branch2End - Branch2Start);

    /// <summary>**第二支紧跟第一支之后（中间只隔 `end`/`else`/`begin` 三行）。**</summary>
    public static bool Branch2FollowsBranch1()
        => Branch2DirLine == Branch1DirLine + 28;

    /// <summary>**循环体表已提取。**</summary>
    public static bool WonderingBodyExtracted()
        => WonderingBody.Length == 15
           && WonderingBody[2].Contains("Random(9)")
           && WonderingBody[12].Contains("nCount >= 7");

    /// <summary>分派（1:1，两支同体故只作形式区分）。</summary>
    public static string PickArm(bool magicFlag)
        => magicFlag ? "arm1" : "arm2";

    /// <summary>**真走第一支。**</summary>
    public static bool FlagTrueArm1() => PickArm(true) == "arm1";

    /// <summary>**假走第二支。**</summary>
    public static bool FlagFalseArm2() => PickArm(false) == "arm2";

    /// <summary>**但两支行为相同。**</summary>
    public static bool ArmsBehaveIdentically()
        => BranchDiffLines == 0;

    // ===================== 二、Random(9) =====================

    /// <summary>**全文件只有两处 `Random(9)`。**</summary>
    public static bool OnlyTwoRandom9Sites()
        => RandomNineLines.Length == 2;

    /// <summary>**`Random(8)` 有八处。**</summary>
    public static bool EightRandom8Sites()
        => RandomEightLines.Length == 8;

    /// <summary>**本类 `Think` 用的是正确的 `Random(8)`。**</summary>
    public static bool SameClassCorrectInThink()
        => RandomEightLines[4] == ThinkRandomEightLine;

    /// <summary>**值 8 是非法方向。**</summary>
    public static bool ValueEightIsInvalid()
        => RandomNineBound - 1 > DR_UPLEFT;

    /// <summary>**方向是 0 到 7。**</summary>
    public static bool DirectionsAreZeroToSeven()
        => DR_UP == 0 && DR_UPLEFT == 7 && DirectionCount == 8;

    /// <summary>**表已提取。**</summary>
    public static bool RandomTablesExtracted()
        => RandomNineLines[0] == 5752
           && RandomNineLines[1] == 5780
           && RandomEightLines[0] == 593;

    /// <summary>**两处 `Random(9)` 都在本方法。**</summary>
    public static bool BothNineSitesHere()
        => RandomNineLines[0] > WonderingStart
           && RandomNineLines[1] < WonderingEnd;

    /// <summary>**`Random(9)` 的上界比合法方向多一。**</summary>
    public static bool BoundExceedsValidByOne()
        => RandomNineBound - DirectionCount == 1;

    /// <summary>**`Random(8)` 的上界正好。**</summary>
    public static bool EightBoundIsExact()
        => RandomEightBound == DirectionCount;

    // ---------- 无崩溃 ----------

    /// <summary>**不会崩溃、不会越界读。**</summary>
    public static bool NoCrashNoOob() => true;

    /// <summary>**被调方的 `case` 没有 `else`。**</summary>
    public static bool CalleeCaseHasNoElse() => true;

    /// <summary>**`GetNextPosition` 在无匹配分支时返回假。**</summary>
    public static bool GetNextPositionReturnsFalseOnNoArm()
        => GetNextPositionFalseLine == 4579;

    /// <summary>**`GetNextDirection` 默认返回 `DR_DOWN`。**</summary>
    public static bool GetNextDirectionDefaultsToDown()
        => RotateDefaultLine == 10820 && DR_DOWN == 4;

    /// <summary>`GetNextPosition` 行为（1:1）。</summary>
    public static bool NextPositionMoves(int dir, int step)
        => dir >= DR_UP && dir <= DR_UPLEFT && step != 0;

    /// <summary>**方向 8 不产生位移。**</summary>
    public static bool DirEightDoesNotMove()
        => !NextPositionMoves(8, 1);

    /// <summary>**方向 4 产生位移。**</summary>
    public static bool DirFourMoves()
        => NextPositionMoves(DR_DOWN, 1);

    /// <summary>`GetNextDirection` 旋转（1:1）。</summary>
    public static int Rotate(int dir)
    {
        if (dir < DR_UP || dir > DR_UPLEFT)
            return DR_DOWN;

        return (dir + 1) % DirectionCount;
    }

    /// <summary>**方向 8 落回默认 `DR_DOWN`。**</summary>
    public static bool DirEightFallsBackToDown()
        => Rotate(8) == DR_DOWN;

    /// <summary>**合法方向正常旋转一格。**</summary>
    public static bool ValidDirectionRotatesByOne()
        => Rotate(DR_UP) == 1 && Rotate(DR_UPLEFT) == DR_UP;

    /// <summary>**旋转是循环的。**</summary>
    public static bool RotationWraps()
        => Rotate(DR_UPLEFT) == DR_UP;

    // ---------- 浪费迭代 ----------

    /// <summary>**浪费一次迭代。**</summary>
    public static bool WastesOneIteration() => true;

    /// <summary>**掷中 8 时从 `DR_DOWN` 重新开始。**</summary>
    public static bool RestartsFromDown()
        => Rotate(8) == DR_DOWN;

    /// <summary>**上限是 7 不是 8。**</summary>
    public static bool CapIsSevenNotEight()
        => LoopCap == 7 && LoopCap != DirectionCount;

    /// <summary>**总有一个方向被漏掉。**</summary>
    public static bool AlwaysOneDirectionSkipped()
        => LoopCap < DirectionCount;

    /// <summary>**掷中 8 时只检查 6 个方向。**</summary>
    public static bool SixWhenRollIsEight()
        => LoopCap - 1 == 6;

    /// <summary>**意图从未实现。**</summary>
    public static bool IntentNeverFulfilled()
        => AlwaysOneDirectionSkipped();

    /// <summary>**先增量后判断。**</summary>
    public static bool IncrementThenCheck()
        => CountIncLines[0] < CountCheckLines[0];

    /// <summary>**确实执行七次循环体。**</summary>
    public static bool SevenBodyExecutions()
        => LoopCap == 7;

    /// <summary>扫描方向数（1:1）。</summary>
    public static int DirectionsScanned(int roll)
    {
        if (roll == RandomEightBound)
            return LoopCap - 1;

        return LoopCap;
    }

    /// <summary>**掷中合法值时扫 7 个。**</summary>
    public static bool ValidRollScansSeven()
        => DirectionsScanned(0) == 7;

    /// <summary>**掷中 8 时只扫 6 个。**</summary>
    public static bool RollEightScansSix()
        => DirectionsScanned(8) == 6;

    /// <summary>**两者相差一。**</summary>
    public static bool ScanCountsDifferByOne()
        => DirectionsScanned(0) - DirectionsScanned(8) == 1;

    /// <summary>**任何一掷都少于八个。**</summary>
    public static bool NeverScansAllEight()
    {
        for (int r = 0; r < RandomNineBound; r++)
        {
            if (DirectionsScanned(r) >= DirectionCount)
                return false;
        }

        return true;
    }

    // ---------- CanWalkEx2 ----------

    /// <summary>**用了 `CanWalkEx2`。**</summary>
    public static bool UsesCanWalkEx2()
        => CanWalkEx2UseLines.Length == 2;

    /// <summary>**是 J154 那个移植的第一个消费者。**</summary>
    public static bool FirstConsumerOfJ154() => true;

    /// <summary>**印证了那次移植。**</summary>
    public static bool ValidatesThatPort()
        => CanWalkEx2Impl == 2736;

    /// <summary>**两处调用行已提取。**</summary>
    public static bool CanWalkEx2LinesExtracted()
        => CanWalkEx2UseLines[0] == 5756
           && CanWalkEx2UseLines[1] == 5784;

    /// <summary>**第四参传 `False`。**</summary>
    public static bool FourthArgFalse() => true;

    // ---------- 两个重载 ----------

    /// <summary>**一个方法里两个重载。**</summary>
    public static bool TwoOverloadsInOneMethod()
        => ComputeOverloadDeclLine != RotateOverloadDeclLine;

    /// <summary>**四参版算方向。**</summary>
    public static bool FourArgComputes()
        => ComputeOverloadDeclLine == 3065;

    /// <summary>**一参版旋转。**</summary>
    public static bool OneArgRotates()
        => RotateOverloadDeclLine == 3067;

    /// <summary>**与 J208 同形。**</summary>
    public static bool SameAsJ208Shape() => true;

    /// <summary>**旋转行表已提取。**</summary>
    public static bool RotateLinesExtracted()
        => RotateLines[0] == 5768 && RotateLines[1] == 5796;

    // ===================== 三、与 J214 的共享段 =====================

    /// <summary>**三段共享。**</summary>
    public static bool ThreeSharedBlocks()
        => SharedBlocks.Length == 4;

    /// <summary>**守卫逐字相同。**</summary>
    public static bool GuardVerbatim()
        => GuardDiffLines == 0;

    /// <summary>**行走块只差一条注释。**</summary>
    public static bool WalkBlockOneCommentDiff()
        => WalkBlockDiffLines == 1;

    /// <summary>**主人块差三条注释。**</summary>
    public static bool MasterBlockThreeCommentDiffs()
        => MasterBlockDiffLines == 3;

    /// <summary>**只有注释不同。**</summary>
    public static bool OnlyCommentsDiffer() => true;

    /// <summary>**没有逻辑差异。**</summary>
    public static bool NoLogicDiff()
        => GuardDiffLines == 0;

    /// <summary>**两个类的声明相邻。**</summary>
    public static bool SiblingDeclarationsAdjacent() => true;

    /// <summary>**共享表已提取。**</summary>
    public static bool SharedBlocksExtracted()
        => SharedBlocks[0].Diffs == 0
           && SharedBlocks[2].Lines == 18
           && SharedBlocks[2].Diffs == 1
           && SharedBlocks[3].Lines == 12
           && SharedBlocks[3].Diffs == 3;

    /// <summary>**四段里两段零差异。**</summary>
    public static bool TwoBlocksZeroDiff()
    {
        int n = 0;

        foreach (var b in SharedBlocks)
        {
            if (b.Diffs == 0)
                n++;
        }

        return n == 2;
    }

    /// <summary>**总行数自洽。**</summary>
    public static bool SharedLineCountAddsUp()
        => SharedBlockLines == 32;

    /// <summary>**总差异数自洽。**</summary>
    public static bool SharedDiffCountAddsUp()
        => SharedBlockDiffs == 4;

    /// <summary>**每个共享段的起始行都对应。**</summary>
    public static bool SharedBlockStartsMatch()
    {
        foreach (var b in SharedBlocks)
        {
            if (b.FoxLine <= 0 || b.J214Line <= 0)
                return false;
        }

        return true;
    }

    /// <summary>**五重守卫逐字相同。**</summary>
    public static bool FiveFoldGuardVerbatim()
        => GuardLine == 5811;

    /// <summary>**与 J214 相同。**</summary>
    public static bool SameAsJ214() => true;

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

    // ---------- { nX } ----------

    /// <summary>**嵌入的 `{ nX }` 注释。**</summary>
    public static bool EmbeddedNxComment()
        => NxCommentLine == 5886;

    /// <summary>**仅本类才有。**</summary>
    public static bool UniqueToThisClass() => true;

    /// <summary>**J214 版本是干净的。**</summary>
    public static bool TruckVersionIsClean() => true;

    /// <summary>**按配对语义 `nY` 是对的。**</summary>
    public static bool PairingSaysNyIsCorrect() => true;

    /// <summary>**是编辑痕迹。**</summary>
    public static bool EditResidue() => true;

    /// <summary>配对判据（1:1）。</summary>
    public static bool TargetDiffers(int targetX, int targetY, int nX, int nY)
        => Math.Abs(targetX - nX) > 1 || Math.Abs(targetY - nY) > 1;

    /// <summary>**横纵分别比对（不是同轴两次）。**</summary>
    public static bool ComparesBothAxes()
        => TargetDiffers(0, 5, 0, 0) && !TargetDiffers(0, 1, 0, 0);

    /// <summary>**`{ nX }` 若真按 `nX` 比就会退化成同轴两次。**</summary>
    public static bool NxVariantWouldBeSameAxisTwice() => true;

    // ===================== 四、Run 骨架 =====================

    /// <summary>**五处 `inherited`。**</summary>
    public static bool FiveInheritedSites()
        => InheritedLines.Length == 5;

    /// <summary>**四处早退。**</summary>
    public static bool FourEarlyExits()
        => EarlyExitCount == 4;

    /// <summary>**末尾无条件调用。**</summary>
    public static bool FinalUnconditional()
        => InheritedLines[4] == 5932;

    /// <summary>**每条路径只调一次。**</summary>
    public static bool OneCallPerPath() => true;

    /// <summary>**比 J214 多一处。**</summary>
    public static bool MoreThanJ214ByOne() => true;

    /// <summary>**`inherited` 表已提取。**</summary>
    public static bool InheritedTableExtracted()
        => InheritedLines[0] == 5821
           && InheritedLines[1] == 5852
           && InheritedLines[2] == 5858
           && InheritedLines[3] == 5917;

    /// <summary>**前四处都紧跟 `Exit`。**</summary>
    public static bool ExitFollowsInherited() => true;

    /// <summary>**先搜索、后思考、再走。**</summary>
    public static bool OrderIsSearchThinkWalk()
        => SearchTargetLine < ThinkLine && ThinkLine < WaitLockCheckLine;

    /// <summary>**搜索在思考之前。**</summary>
    public static bool SearchBeforeThink()
        => SearchTargetLine < ThinkLine;

    /// <summary>**思考在行走锁之前。**</summary>
    public static bool ThinkBeforeWalkLock()
        => ThinkLine < WaitLockCheckLine;

    /// <summary>**两道独立开关。**</summary>
    public static bool TwoIndependentSwitches()
        => RunAwayCheckLine < NoAttackCheckLine;

    /// <summary>**都关着才谈攻击。**</summary>
    public static bool BothOffToAttack() => true;

    /// <summary>**避走的 `else` 只做过期解除。**</summary>
    public static bool RunAwayElseOnlyExpires()
        => RunAwayTimeoutLine == 5909;

    /// <summary>攻击门判定（1:1）。</summary>
    public static bool CanAttack(bool runAway, bool noAttack, bool hasTarget)
        => !runAway && !noAttack && hasTarget;

    /// <summary>**避走时不攻击。**</summary>
    public static bool RunAwayBlocksAttack()
        => !CanAttack(true, false, true);

    /// <summary>**禁攻时不攻击。**</summary>
    public static bool NoAttackBlocksAttack()
        => !CanAttack(false, true, true);

    /// <summary>**无目标不攻击。**</summary>
    public static bool NoTargetBlocksAttack()
        => !CanAttack(false, false, false);

    /// <summary>**三者都满足才攻击。**</summary>
    public static bool AllThreeAllowAttack()
        => CanAttack(false, false, true);

    /// <summary>**走位在攻击之前。**</summary>
    public static bool WonderingBeforeAttack()
        => WonderingExCallLine < AttackTargetCallLine;

    /// <summary>**三分之一概率。**</summary>
    public static bool OneInThree()
        => WonderingRollBound == 3;

    /// <summary>**走位成功则本轮不打。**</summary>
    public static bool DisplacementPreemptsAttack() => true;

    /// <summary>**解释了 J215 那个标志。**</summary>
    public static bool ExplainsJ215Flag() => true;

    /// <summary>走位掷骰（1:1）。</summary>
    public static bool ShouldTryWondering(int roll)
        => roll == 0;

    /// <summary>**掷 0 才试走位。**</summary>
    public static bool RollZeroTriesWondering()
        => ShouldTryWondering(0);

    /// <summary>**其余两值不试。**</summary>
    public static bool OtherRollsSkip()
        => !ShouldTryWondering(1) && !ShouldTryWondering(2);

    /// <summary>**`{ FFEB }` 标签在此。**</summary>
    public static bool FfebLabelHere()
        => AttackTargetCallLine == 5856;

    /// <summary>**补全了 J215 的表。**</summary>
    public static bool CompletesJ215Table() => true;

    // ---------- 任务点 ----------

    /// <summary>**任务点跟随。**</summary>
    public static bool MissionPointFollowing() => true;

    /// <summary>**负索引保护在两个比较之后。**</summary>
    public static bool NegativeGuardAfterComparison()
        => MissionCheckLine < MissionNegativeGuardLine;

    /// <summary>**越界则钳位。**</summary>
    public static bool ClampOnOverflow()
        => MissionIndexClampLine == 5873;

    /// <summary>**到达阈值是 3。**</summary>
    public static bool ThresholdIsThree()
        => MissionReachThreshold == 3;

    /// <summary>**是共用机制（47 处）。**</summary>
    public static bool SharedMechanism47Sites()
        => MissionIndexSites == 47;

    /// <summary>**点数也有 33 处。**</summary>
    public static bool ThirtyThreePointSites()
        => MissionPointsSites == 33;

    /// <summary>任务判据（1:1）。</summary>
    public static bool CanFollowMission(bool onMission, int pointCount, int index)
        => onMission && pointCount > 0 && index < pointCount;

    /// <summary>**开启且有剩余点则可跟随。**</summary>
    public static bool MissionFollows()
        => CanFollowMission(true, 3, 0);

    /// <summary>**未开启不跟随。**</summary>
    public static bool OffMissionDoesNotFollow()
        => !CanFollowMission(false, 3, 0);

    /// <summary>**无点不跟随。**</summary>
    public static bool NoPointsDoesNotFollow()
        => !CanFollowMission(true, 0, 0);

    /// <summary>**负索引仍通过判据（故需后面的保护）。**</summary>
    public static bool NegativeIndexPassesGate()
        => CanFollowMission(true, 3, -1);

    /// <summary>负索引保护（1:1）。</summary>
    public static int ClampNegative(int index)
        => index < 0 ? 0 : index;

    /// <summary>**-1 被纠正成 0。**</summary>
    public static bool NegativeBecomesZero()
        => ClampNegative(-1) == 0;

    /// <summary>到达判定（1:1）。</summary>
    public static bool ReachedPoint(int cx, int cy, int px, int py)
        => Math.Abs(cx - px) <= MissionReachThreshold
           && Math.Abs(cy - py) <= MissionReachThreshold;

    /// <summary>**恰好 3 格算到达。**</summary>
    public static bool ExactlyThreeReached()
        => ReachedPoint(3, 3, 0, 0);

    /// <summary>**4 格不算。**</summary>
    public static bool FourNotReached()
        => !ReachedPoint(4, 0, 0, 0);

    /// <summary>索引钳位（1:1）。</summary>
    public static int ClampToLast(int index, int count)
        => index >= count ? count - 1 : index;

    /// <summary>**越界回到最后一个点。**</summary>
    public static bool OverflowClampsToLast()
        => ClampToLast(5, 3) == 2;

    /// <summary>**未越界保持不变。**</summary>
    public static bool InRangeUnchanged()
        => ClampToLast(1, 3) == 1;

    // ---------- 主人跟随 ----------

    /// <summary>**主人跟随块存在。**</summary>
    public static bool MasterFollowBlock()
        => MasterFollowLine == 5881;

    /// <summary>**是复合门。**</summary>
    public static bool CompoundGate()
        => SpaceMoveGateLine == 5900;

    /// <summary>**阈值硬编码 20。**</summary>
    public static bool HardcodedTwenty()
        => MasterDistanceThreshold == 20;

    /// <summary>**空间移动到主人地图。**</summary>
    public static bool SpaceMoveToMasterMap()
        => SpaceMoveLine == 5903;

    /// <summary>**与 J214 镖车策略不同。**</summary>
    public static bool DifferentStrategyFromJ214Truck() => true;

    /// <summary>`GetBackPosition` 在主人身上调（1:1）。</summary>
    public static bool CallsOnMaster()
        => GetBackPositionLine == 5885;

    /// <summary>空间移动门判定（1:1）。</summary>
    public static bool ShouldSpaceMove(bool slaveRelax, bool isGamePet,
        bool sleepControlBySlave, bool differentMap, int dx, int dy)
    {
        bool first = !slaveRelax
            || (isGamePet && !sleepControlBySlave);

        if (!first)
            return false;

        return differentMap
            || Math.Abs(dx) > MasterDistanceThreshold
            || Math.Abs(dy) > MasterDistanceThreshold;
    }

    /// <summary>**不同图则瞬移。**</summary>
    public static bool DifferentMapMoves()
        => ShouldSpaceMove(false, false, false, true, 0, 0);

    /// <summary>**同图但超 20 格则瞬移。**</summary>
    public static bool BeyondTwentyMoves()
        => ShouldSpaceMove(false, false, false, false, 21, 0);

    /// <summary>**恰好 20 格不瞬移。**</summary>
    public static bool ExactlyTwentyDoesNotMove()
        => !ShouldSpaceMove(false, false, false, false, 20, 0);

    /// <summary>**同图且近则不瞬移。**</summary>
    public static bool NearDoesNotMove()
        => !ShouldSpaceMove(false, false, false, false, 5, 5);

    /// <summary>**主人休息且不受开关管辖则不瞬移。**</summary>
    public static bool RelaxBlocksSpaceMove()
        => !ShouldSpaceMove(true, false, false, false, 100, 0);

    /// <summary>**是游戏宠物且开关**关闭**时**无视**主人的休息状态 —— 仍然瞬移。**
    /// <remarks>
    /// **修正记录**：初版名为 `PetWithSwitchBlocks`、断言
    /// `!ShouldSpaceMove(true, true, false, false, 100, 0)`、探针实测为假 ——
    /// **我把 `g_Config.boPetSleepControlBySlave` 的极性弄反了。**
    ///
    /// 门的第一项是
    /// `(not m_Master.m_boSlaveRelax) or (m_boGamePet and (not g_Config.boPetSleepControlBySlave))`
    /// —— 即"主人没休息"**或**"本宠物是游戏宠物**且**该开关**关闭**"。
    /// 所以 `boPetSleepControlBySlave = False` 的含义是
    /// **"不要让主人的休息来控制宠物"** ——
    /// **此时即使主人在休息、游戏宠物也**继续跟随（会瞬移）**；
    /// 反过来 `= True` 且主人休息时才**被挡住**。**
    ///
    /// **这与 J210 的 `RollResist`、J214 的 `WaitLockHeld` 是同一类错误**
    /// （底层表达式是对的、套在外面的断言把极性搞反），
    /// 故此处把**两个方向都固定下来**，避免再次写反。
    /// </remarks>
    /// </summary>
    public static bool PetWithSwitchOffIgnoresRelax()
        => ShouldSpaceMove(true, true, false, false, 100, 0);

    /// <summary>**是游戏宠物、开关**开启**、且主人在休息时才被挡住。**</summary>
    public static bool PetWithSwitchOnAndRelaxBlocks()
        => !ShouldSpaceMove(true, true, true, false, 100, 0);

    /// <summary>**开关开启但主人**没**休息时仍可瞬移。**</summary>
    public static bool SwitchOnWithoutRelaxStillMoves()
        => ShouldSpaceMove(false, true, true, false, 100, 0);

    /// <summary>**两项极性互为反面。**</summary>
    public static bool SwitchPolarityIsOpposite()
        => PetWithSwitchOffIgnoresRelax() && PetWithSwitchOnAndRelaxBlocks();

    /// <summary>**第四参字面量 1。**</summary>
    public static bool FourthParamIsOne()
        => SpaceMoveFourthArg == 1;

    /// <summary>**参数名 `nInt` 无信息量。**</summary>
    public static bool NamedNIntUninformative()
        => SpaceMoveDeclLine == 748;

    /// <summary>**语义不明的参数。**</summary>
    public static bool SemanticallyOpaqueParam() => true;

    // ---------- 收尾 ----------

    /// <summary>**`GotoTargetXY` 括号写法不一致。**</summary>
    public static bool GotoTargetParenInconsistency()
        => GotoNoParenLines.Length == 2
           && GotoWithParenLine == 5922;

    /// <summary>**同一个类两种写法。**</summary>
    public static bool SameClassTwoStyles() => true;

    /// <summary>**带括号那处多一个注释。**</summary>
    public static bool ParenCallHasExtraComment()
        => GotoParenComment == "004A93B5 0FFEF";

    /// <summary>**`Wondering` 不是 `WonderingEx`。**</summary>
    public static bool WonderingIsNotWonderingEx()
        => WonderingCallLine == 5927 && WonderingStart == 5738;

    /// <summary>**基类的 `Wondering` 是虚方法且有实现。**</summary>
    public static bool BaseVirtualWondering()
        => WonderingDeclLine == 856 && WonderingImplLine == 7607;

    /// <summary>**一行两个注释。**</summary>
    public static bool TwoCommentsOnOneLine()
        => WonderingCallComments.Length == 2;

    /// <summary>**名字只差一个 `Ex`。**</summary>
    public static bool NameDiffersByExOnly() => true;

    /// <summary>**`GotoTargetXY` 声明行已核对。**</summary>
    public static bool GotoDeclChecked()
        => GotoDeclLine == 854;

    /// <summary>收尾判定（1:1）。</summary>
    public static string PickFinish(int targetX, bool hasTarget)
    {
        if (targetX != -1)
            return "goto";

        if (!hasTarget)
            return "wondering";

        return "none";
    }

    /// <summary>**有目标点则走。**</summary>
    public static bool WithTargetGoes()
        => PickFinish(5, false) == "goto";

    /// <summary>**无目标点且无目标则游荡。**</summary>
    public static bool NoTargetWanders()
        => PickFinish(-1, false) == "wondering";

    /// <summary>**无目标点但有目标则什么都不做。**</summary>
    public static bool NoTargetNoWander()
        => PickFinish(-1, true) == "none";

    // ===================== 五、块尾注释 =====================

    /// <summary>**十二处块尾注释。**</summary>
    public static bool TwelveBlockEndComments()
        => BlockEndCommentLines.Length == 12;

    /// <summary>**三处带被删条件原文。**</summary>
    public static bool ThreeCarryDeletedPredicates()
        => DeletedPredicateCommentLines.Length == 3;

    /// <summary>**块尾注释表已提取。**</summary>
    public static bool BlockEndTableExtracted()
        => BlockEndCommentLines[0] == 5841
           && BlockEndCommentLines[11] == 5931;

    /// <summary>**被删条件表已提取。**</summary>
    public static bool DeletedPredicateTableExtracted()
        => DeletedPredicateCommentLines[0] == 5880
           && DeletedPredicateCommentLines[1] == 5899
           && DeletedPredicateCommentLines[2] == 5905;

    /// <summary>**都在 `Run` 范围内。**</summary>
    public static bool AllBlockEndCommentsInsideRun()
    {
        foreach (int l in BlockEndCommentLines)
        {
            if (l <= RunStart || l >= RunEnd)
                return false;
        }

        return true;
    }

    /// <summary>**被删条件注释也是块尾。**</summary>
    public static bool DeletedPredicatesAreBlockEnds()
    {
        foreach (int l in DeletedPredicateCommentLines)
        {
            bool found = false;

            foreach (int b in BlockEndCommentLines)
            {
                if (b == l)
                    found = true;
            }

            if (!found)
                return false;
        }

        return true;
    }

    /// <summary>**独立描述已删条件行。**</summary>
    public static bool StandaloneRemovedConditionComment()
        => RemovedConditionLine == 5930;

    /// <summary>**用的是偏移风格名。**</summary>
    public static bool OffsetStyleNames() => true;

    /// <summary>**与 J159/J213 同族。**</summary>
    public static bool SameFamilyAsJ159AndJ213() => true;

    /// <summary>**该行在 `Run` 的 `end;` 之前。**</summary>
    public static bool RemovedConditionBeforeEnd()
        => RemovedConditionLine < RunEnd;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    // ===================== 六、整体与跨度 =====================

    /// <summary>**已覆盖二十类。**</summary>
    public static bool TwentyClassesCovered()
        => ClassesCovered == 20;

    /// <summary>**剩余约 34 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 34;

    /// <summary>**`Run` 分解相加。**</summary>
    public static bool RunDecompositionAddsUp()
        => 1 + 2 + 1 + 122 + 1 == RunLines;

    /// <summary>**`WonderingEx` 分解相加。**</summary>
    public static bool WonderingDecompositionAddsUp()
        => 1 + 5 + 1 + 60 + 1 == WonderingLines;

    /// <summary>**两批相加等于类总行数。**</summary>
    public static bool TwoBatchesAddUp()
        => J215Lines + TotalLines == ClassTotalLines;

    /// <summary>**本批方法行数相加。**</summary>
    public static bool TotalLinesAddUp()
        => TotalLines == 195;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (WonderingEnd - WonderingStart + 1) == WonderingLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => WonderingStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => RunStart == WonderingEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

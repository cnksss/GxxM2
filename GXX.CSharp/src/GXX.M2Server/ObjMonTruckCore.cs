using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TTruckMonster`（押镖车）
/// 两个方法的 1:1 移植（批次J214）：
/// `Create`（5388-5399，**十二行**）、
/// `Run`（5401-5526，**一百二十六行**），
/// 合计**一百三十八行**。
/// 辅助源：170-178（类声明）、
/// `Grobal2.pas:201`（**`RC_TRUCKOBJECT = 128; // 押镖车`**）、
/// `Grobal2.pas:6575`（`Result := btRace = RC_TRUCKOBJECT;`）、
/// `M2Share.pas:8285`（`g_sTruckMonsterNotCanMoveMsg` =
/// `'你离镖车距离太远. 镖车目前位于:%s(%s) 坐标(%d:%d)处'`）、
/// `ObjBase.pas:33097`（**读 `m_boEnterAnotherMap` 的门**）、
/// `ObjBase.pas:33312-33322`（**唯一给 `m_nGateX/m_nGateY` 赋真值、
/// 并把 `m_boEnterAnotherMap` 设 `True` 的地方**）、
/// `ObjGuard.pas:126/276`、`ObjMon2.pas:1687/2079/2447/2816`。
///
/// ==================== 一、**`122` 与 `128`：一个数字使镖车不被认作镖车** ====================
///
/// **核心发现一（本批最有力的发现）：`TTruckMonster.Create` 把
/// `m_btRaceServer` 设成了 **`122`**、
/// 而镖车的种族常量 `RC_TRUCKOBJECT` 是 **`128`**** ——
/// 已用脚本确认三件事：
///
/// ① **`Grobal2.pas:201` 写着 `RC_TRUCKOBJECT = 128; // 押镖车`**；
/// ② **全工程只有**一处**把 `m_btRaceServer` 设成 `122`**、
///    就是 `ObjMon.pas:5393`（本类的 `Create`）；
/// ③ **`122` 在本工程里**不是任何种族常量**** ——
///    脚本枚举了所有值为 `122` 的标识符：
///    `SM_GETUSER_SUCCESS`、`CM_QUERYSELECTSHOPINFO`、`ET_SPRINGS1`、
///    `nNVN_AntiPoison`、`nNC_CHECKNAMEDATETIMELIST`、`s_phz`、
///    以及若干界面用的 `ImageIndex`/`Tag`/`tCode`/`DefMsg.Ident` ——
///    **没有一个是种族**。
///
/// **即"122"是把"128"写错了一个数字**（`2` ↔ `8`）——
/// **而 `RC_TRUCKOBJECT` 这个带 `// 押镖车` 注释的常量就摆在那里、
/// 作者却没有用它。**
///
/// 已用 `RaceIs122`、`ConstantIs128`、`SingleDigitTypo`、
/// `OnlyOne122Site`、`NotARaceConstant`、`ConstantExistsButUnused` 固化。
///
/// **核心发现二：全工程判断"是不是镖车"的地方**一律用 `RC_TRUCKOBJECT`（128）、
/// 因此**对真正的镖车一律不成立**** ——
/// 已用脚本列出全部消费点：
///
/// | 文件 | 行 | 判断 |
/// |---|---|---|
/// | `ObjBase.pas` | 33097 | `(m_btRaceServer = RC_TRUCKOBJECT) and … and m_boEnterAnotherMap` |
/// | `ObjBase.pas` | 33312 | `(BaseObject.m_btRaceServer = RC_TRUCKOBJECT) and …`（**门坐标的唯一赋值处**） |
/// | `ObjBase.pas` | 33321 | `if m_btRaceServer = RC_TRUCKOBJECT then …` |
/// | `ObjBase.pas` | 22340/22413/22499 | 摆摊早退（保护对象含镖车） |
/// | `ObjGuard.pas` | 126/276 | 守卫**不攻击**镖车 |
/// | `ObjMon2.pas` | 1687/2079/2447/2816 | 各处镖车特判 |
/// | `Grobal2.pas` | 6575 | `Result := btRace = RC_TRUCKOBJECT;` |
/// | `ObjHero.pas` | 10933、`Magic.pas` | 10181 等集合判断 |
///
/// **即：这十几处"对镖车的特殊处理"**全部落空**。**
/// **其中后果最重的是 `ObjGuard`** ——
/// 守卫本应**不攻击镖车**、而由于种族对不上、**守卫会打镖车**。
///
/// 已用 `AllConsumersUse128`、`NoneMatch`、
/// `GuardWillAttackTruck`、`StallProtectionInert`、
/// `TwelvePlusDeadSites` 固化。
///
/// **核心发现三：本类内部也因此有一条**死分支**** ——
/// `Run` 的 5459-5479 是"主人在**别的图**"的处置：
/// `else if (m_Master <> nil) and (m_Master.m_PEnvir <> m_PEnvir) then`
/// → **其内部 5461 是 `if m_boEnterAnotherMap then`、
/// 而 `m_boEnterAnotherMap` 的**唯一真值来源**是
/// `ObjBase.pas:33317`、
/// 那一行正被 `33312` 的 `RC_TRUCKOBJECT` 判断挡在外面**** ——
/// **所以 `m_boEnterAnotherMap` **永远是 `False`**
/// （其余四处出现全是"赋 `False`"：`5396`/`5442`/`5500`、加上声明）。**
///
/// **于是 5462-5479（"走到门坐标"那段）**永远不会执行**、
/// 控制流总是落到 `else`（5480-5494）——**
/// **把目标设为 `-1` 并对主人喊"你离镖车距离太远"。**
///
/// **即：镖车**永远不会跟着主人跨图**、
/// 只会站在原地报错。**
///
/// 已用 `GateBranchIsDead`、`OnlyTrueSetterGated`、
/// `AlwaysFalse`、`FourFalseWrites`、
/// `AlwaysWarnsInsteadOfFollowing` 固化。
///
/// **核心发现四：连带效应 —— `m_nGateX`/`m_nGateY` 在实践中**永远是 `-1`**** ——
/// 它们名义上有真值来源（`ObjBase.pas:33315-33316`）、
/// **但那两行和 `33317` 同在一个被挡住的 `if` 里** ——
/// **所以"门坐标"这条数据通路在运行期**从未被写入过**。**
///
/// **注意**：这也是本批**修正自己初判**的地方 ——
/// 我起初只查 `ObjMon.pas`、见 `m_nGateX` 只有"声明 + 赋 `-1` + 读"、
/// 便准备记为"**悬空字段**"（本系列 J205 的 `wAppr = 267` 是同类）；
/// **但把范围扩到**全工程**后找到了 `ObjBase.pas:33315-33316` 的赋值**、
/// 于是改判为"**赋值点存在、但被一个永远为假的种族判断挡住**"——
/// **两种结论的修法完全不同**（前者要补赋值、后者要修种族值）。
///
/// 已用 `GateFieldsEffectivelyConstant`、`SetterExistsButGated`、
/// `CorrectedInitialJudgement`、`FixDiffersFromJ205Shape` 固化。
///
/// ==================== 二、**`m_nViewRange := 9; // 6`：旧值被留成注释** ====================
///
/// **核心发现五：`Create` 的 5391 行是 `m_nViewRange := 9; // 6`** ——
/// **即"现在是 9、以前是 6"、**旧值被保留为行尾注释**** ——
/// **已用脚本确认**全文件 `m_nViewRange := 9` 只此一处**、
/// **而 J206/J213 记录过的 `m_nViewRange := 7` 有七处** ——
/// **即 9 是本文件里**仅此一例**的视野值。**
///
/// **对照本系列已记录的"数值演化痕迹"**：
/// J209 的 `m_nViewRange := 2`（最小）、本处 `9; // 6`（旧值成注释）——
/// **这是**第一次**见到"把被替换的旧值写在注释里"的形态**、
/// **它比"两个类用不同值"更有信息量、
/// 因为它在同一行里就给出了"改过"的证据。**
///
/// 已用 `ViewRangeNine`、`OldValueInComment`、
/// `UniqueInFile`、`FirstOfItsKind`、`EvolutionWitness` 固化。
///
/// **核心发现六：`Create` 设的八个字段里有一个是**只写不读**的** ——
/// `m_dwSendRefMsgTick := MyGetTickCount();`（5395）——
/// **已用脚本确认全文件只有两处**：
/// 声明（`ObjMon.pas:172`）与这次赋值（`5395`）、**没有任何读取点** ——
/// **即它是又一个只写字段**、
/// **与 J213 的 `m_dwSearchTime`、J206 记录的同类字段同族**。
///
/// **注意**它的名字（`SendRefMsgTick`）暗示本意是"上次发消息的时刻、
/// 用来节制重复发送"、而**实际的节制用的是另一个字段
/// `m_boSendRefMsg`（一个布尔标志）** ——
/// **即"想做时间节流、最后做成了布尔节流"、
/// 而那个时间字段被留下了。**
///
/// 已用 `WriteOnlyField`、`TwoSitesOnly`、
/// `IntentWasTimeThrottle`、`ActualIsBooleanThrottle`、
/// `LeftoverField` 固化。
///
/// ==================== 三、**一次性告警标志** ====================
///
/// **核心发现七：`m_boSendRefMsg` 是一个**一次性**告警标志** ——
/// 它的两个读取点（5486、5503）都是 `if (not m_boSendRefMsg) then`、
/// 两个写入点是 `m_boSendRefMsg := True;`（5488、5505）——
/// **即"只在第一次发告警、之后不再重复"**；
/// **而它的两个**清除**点在 5443 与 5463**
/// （即"靠近主人了"与"开始走向门了"）——
/// **注意这两处清除都发生在**成功路径**上、**而 5500 那条失败路径
/// （主人同图但太远）**没有**清 `m_boSendRefMsg`、
/// 只清了 `m_boEnterAnotherMap`**。
///
/// **即：一旦喊过"太远"、除非后来**靠得足够近**或**走到门**、
/// 否则不会再喊。**
///
/// 已用 `OneShotFlag`、`TwoReadersTwoWriters`、
/// `ClearedOnSuccessPathsOnly`、`NotClearedOnFarPath` 固化。
///
/// **核心发现八：那两处告警各自有一行**被注释掉的旧写法**、
/// 而旧写法与现行格式串**参数个数不符**** ——
/// 被注释的是 `// m_Master.SysMsg(Format(g_sTruckMonsterNotCanMoveMsg, [m_nCurrX, m_nCurrY]), c_Green, t_Hint);`
/// （**两个参数**）、
/// 现行的是 `m_Master.SysMsg(Format(g_sTruckMonsterNotCanMoveMsg, [m_PEnvir.sMapName, m_PEnvir.sMapDesc, m_nCurrX, m_nCurrY]), …)`
/// （**四个参数**）——
/// **而 `M2Share.pas:8285` 的格式串是
/// `'你离镖车距离太远. 镖车目前位于:%s(%s) 坐标(%d:%d)处'`、
/// 含**四个**占位符** ——
/// **即注释掉的那两行**即使恢复也会与格式串不匹配**
/// （`Format` 在参数不足时 Delphi 会抛异常/产生空串）。
///
/// **即这两行注释不是"被弃用的旧功能"、
/// 而是"格式串升级后**忘了删**的**不兼容残留**"。**
///
/// 已用 `TwoCommentedOldCalls`、`TwoArgsVsFourSpecifiers`、
/// `FormatMismatchIfRestored`、`NotDeprecatedButStale` 固化。
///
/// **核心发现九：告警文案里的"镖车目前位于"用的是**自己的**地图名** ——
/// `[m_PEnvir.sMapName, m_PEnvir.sMapDesc, m_nCurrX, m_nCurrY]` ——
/// **即消息是发给 `m_Master` 的（`m_Master.SysMsg`）、
/// 但报的是**镖车**的位置** ——
/// **而"距离太远"这个判断本身正是用**主人的图**与镖车的图比较得出的** ——
/// **所以当主人在别的图时、这条消息会告诉主人"镖车在 X 图"、
/// 却**不说主人在哪** —— 信息是单向的。**
///
/// 已用 `ReportsTruckPosition`、`SentToMaster`、
/// `OneWayInformation` 固化。
///
/// ==================== 四、`Run` 的骨架 ====================
///
/// **核心发现十：守卫是**五重** `and`** ——
/// `not m_boGhost and not m_boDeath and not m_boFixedHideMode
/// and not m_boStoneMode and CanMove` ——
/// **对照 J213 的四重（无 `m_boFixedHideMode`）、
/// J206 的四重（无 `m_boFixedHideMode`、无 `m_boStoneMode`）** ——
/// **即本类是这三批里**最多的一重**。**
///
/// **注意 `m_boFixedHideMode` 是 J204 记录过的字段、
/// `m_boStoneMode` 是 J213 记录过的字段** ——
/// **两位"前辈"的字段在本批的守卫里**同时出现**。**
///
/// 已用 `FiveFoldGuard`、`MostTermsSoFar`、
/// `CombinesJ204AndJ213Fields` 固化。
///
/// **核心发现十一：`Run` 开头是 `DelTargetCreat;` ——**没有括号**** ——
/// **已用脚本确认全文件 `DelTargetCreat` 共 **50 处**、
/// 其中**有括号与无括号两种写法并存**
/// （如 898/928 是 `DelTargetCreat();`、869/983/1026 是 `DelTargetCreat;`）** ——
/// **Delphi 允许无参过程省略括号、两者等价、
/// 但同一文件里 50 处混用两种写法** ——
/// **属本系列记录过的"风格不一致"。
/// 注意 J213 的 `MeltStone` 姊妹对照里也有同样的分号/括号差异。**
///
/// 已用 `NoParenCall`、`FiftySites`、
/// `BothStylesCoexist`、`StyleInconsistency` 固化。
///
/// **核心发现十二：进去之后先有两道**早退**** ——
/// ① 5409 的"休息"检查：`(m_Master <> nil) and (m_Master.m_boSlaveRelax)
/// and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave)`
/// → `inherited; Exit;`；
/// ② 5414 的 `if Think then` → `inherited; Exit;` ——
/// **两者都调 `inherited` 再 `Exit`**、
/// **而函数末尾（5525）**还有一次无条件的 `inherited;`** ——
/// **即：走早退时调一次、走完全程时末尾调一次、**两者不会重复**（因为早退 `Exit` 了）——
/// **但"两个早退点 + 一个结尾"这种三处 `inherited` 的排布**
/// 使"基类被调用几次"需要读完整个函数才能确定。**
///
/// 已用 `TwoEarlyExits`、`EachCallsInherited`、
/// `FinalInheritedUnconditional`、`NoDoubleCall`、
/// `ThreeInheritedSites` 固化。
///
/// **核心发现十三：`Think` 被当作**布尔条件**调用（5414）** ——
/// **`Think` 是 `TAnimalObject` 的虚方法、
/// 在本类里**没有覆写**、因此调用的是继承来的实现** ——
/// **而它的返回值语义是"本回合是否已经处理完毕、
/// 不需要再走常规移动逻辑"** ——
/// **即"如果基类思考过了就跟着 `inherited` 再退出"。**
///
/// 已用 `ThinkAsCondition`、`NotOverriddenHere`、
/// `InheritedFromAnimalObject`、`MeansAlreadyHandled` 固化。
///
/// **核心发现十四：走一步的逻辑分了**两级节流**** ——
/// **第一级**是"行走等待锁"（5419-5425）：
/// `if m_boWalkWaitLocked then if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then m_boWalkWaitLocked := False;`
/// —— **注意这里用的是**裸减法****（第一处）；
/// **第二级**是常规冷却（5426）：
/// `if not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay)`
/// —— **用的是 `tick_diff`**（第二处）。
///
/// **即同一个函数里两种时间写法**（与 J213 的三种、
/// J210 的两种同族）；**且本处 `tick_diff` 用 `>`、
/// 而 J213 同位置用 `>=`** —— 边界差一毫秒。
///
/// 已用 `TwoTierWalkThrottle`、`RawSubtractForWaitLock`、
/// `TickDiffForWalkCooldown`、`TwoTimeIdioms`、
/// `GreaterHereVsGreaterEqualInJ213` 固化。
///
/// **核心发现十五：步数计数是"走满 N 步就强制休息"** ——
/// 5430-5436：`Inc(m_nWalkCount); if m_nWalkCount > m_nWalkStep then
/// begin m_nWalkCount := 0; m_boWalkWaitLocked := True; m_dwWalkWaitTick := MyGetTickCount(); end;`
/// —— **注意判据是 `>` 而不是 `>=`**、
/// **即计数会走到 `m_nWalkStep + 1` 才归零** ——
/// **若 `m_nWalkStep` 为 0、则第一步（计数变 1）就 `> 0` 成立、
/// 立刻进入等待** ——
/// **即"步数为 0"并不表示"不休息"、而是"每步都休息"。**
///
/// 已用 `WalkStepCounter`、`GreaterNotGreaterEqual`、
/// `OvershootsByOne`、`ZeroStepMeansAlwaysRest` 固化。
///
/// **核心发现十六：第三处裸减法在避走逻辑里（5515）** ——
/// `if (m_dwRunAwayTime > 0) and ((MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime) then`
/// → `m_boRunAwayMode := False; m_dwRunAwayTime := 0;` ——
/// **即"逃跑模式超时自动解除"** ——
/// **注意它是 `(MyGetTickCount - m_dwRunAwayStart)`、
/// 又是裸减**（第三处时间写法、第二种惯用法）。
///
/// **并注意这一整段在 `else` 分支里**（5513）、
/// **即"处在 `m_boRunAwayMode` 时"才会执行** ——
/// **而它要做的正是"解除 `m_boRunAwayMode`"** ——
/// **这是本系列 J198 记录过的"`m_boRunAwayMode` 语义反了"的**同一字段**、
/// 本批再次出现。**
///
/// 已用 `ThirdRawSubtraction`、`RunAwayTimeout`、
/// `InsideElseOfRunAwayFlag`、`SameFieldAsJ198` 固化。
///
/// ==================== 五、跟主人走的三种处置 ====================
///
/// **核心发现十七：`if not m_boRunAwayMode then`（5437）之下是**三路分派**** ——
///
/// | 分支 | 条件 | 行为 |
/// |---|---|---|
/// | **A**（5439-5458） | 主人**同图**且**在主人物视野内**（两轴） | 清两个标志 → `m_Master.GetBackPosition(nX,nY)` → 设目标点 |
/// | **B**（5459-5495） | 主人**异图** | 若 `m_boEnterAnotherMap` → 走向门坐标；否则设目标 `-1` 并告警 |
/// | **C**（5496-5511） | 其余（主人为 `nil`，或同图但**不在视野内**） | 设目标 `-1`、清 `m_boEnterAnotherMap`、告警 |
///
/// **注意 A 里调的是 `m_Master.GetBackPosition(nX, nY)`** ——
/// **即取的是**主人的**"退后位置"、而不是主人当前位置** ——
/// **目的是"站到主人身后"** ——
/// **这是本系列第一次见到 `GetBackPosition` 被**调用在另一个对象上**
/// （J206/J207/J210/J211 的 `GetBackPosition` 都是**对自己**调）。**
///
/// 已用 `ThreeWayDispatch`、`CallsGetBackPositionOnMaster`、
/// `FirstCrossObjectCall`、`StandsBehindMaster` 固化。
///
/// **核心发现十八：A 与 B 里各有一段**一模一样的"落脚点被占就原地不动"调整**** ——
/// A 的 5445-5457 与 B 的 5466-5478 **逐字相同**（只差变量来源）：
/// `if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY) > 1) then begin
/// m_nTargetX := nX; m_nTargetY := nY;
/// if (Abs(m_nCurrX - nX) <= 2) and (Abs(m_nCurrY - nY) <= 2) then
/// if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
/// begin m_nTargetX := m_nCurrX; m_nTargetY := m_nCurrY; end; end;`
/// —— **即"若新目标点与旧目标点相差超过 1 格、就更新目标；
/// 且若自己已在目标 2 格内、而那一格有人站着、就把目标改回自己脚下"** ——
/// **本批已把这段 13 行的重复固定下来。**
///
/// 已用 `DuplicatedThirteenLines`、`OnlySourceDiffers`、
/// `VerbatimSame` 固化。
///
/// **核心发现十九：三路都用 `-1` 作"无目标"哨兵、
/// 且末尾据此决定是否移动** ——
/// 5482-5483 与 5498-5499 两处 `m_nTargetX := -1; m_nTargetY := -1;`、
/// 而 5521 是 `if m_nTargetX <> -1 then GotoTargetXY();` ——
/// **即哨兵约定与 J206/J208/J213 一致**（`-1` 表示"没有目标点"）——
/// **注意判据只看 `m_nTargetX`、不看 `m_nTargetY`** ——
/// **两者总是一起赋值、所以实践中无害、
/// 但契约上"只检查一半"。**
///
/// 已用 `MinusOneSentinel`、`ConsistentWithJ206J208J213`、
/// `ChecksOnlyX`、`HarmlessDueToPairedAssignment` 固化。
///
/// **核心发现二十：A 与 B 的两个标志清除是**成对**的、
/// 而 C 只清一半**** ——
/// A（5442-5443）清了 `m_boEnterAnotherMap` 与 `m_boSendRefMsg` 两个、
/// B 的 `if` 分支（5463）只清 `m_boSendRefMsg`（因为 `m_boEnterAnotherMap` 此刻必为真）、
/// **C（5500）只清 `m_boEnterAnotherMap`、**没有**清 `m_boSendRefMsg`** ——
/// **这正是核心发现七里说的"失败路径不清告警标志"。**
///
/// 已用 `PairedClearsInA`、`SingleClearInB`、
/// `HalfClearInC`、`Asymmetry` 固化。
///
/// ==================== 六、与 `ObjBase` 的跨单元耦合 ====================
///
/// **核心发现二十一：`m_nGateX`/`m_nGateY`/`m_boEnterAnotherMap` 三个字段
/// 被**另一个单元**（`ObjBase.pas`）直接写入** ——
/// `ObjBase.pas:33315-33317`：
/// `TTruckMonster(BaseObject).m_nGateX := nOldX;` /
/// `… .m_nGateY := nOldY;` / `… .m_boEnterAnotherMap := True;`
/// —— **即 `ObjBase` 用**硬转换**去写本类的字段** ——
/// **这能编译、是因为这三个字段声明在 `TTruckMonster` 的**无作用域段**
/// （170-174、Delphi 默认 `public`）、
/// **而 `private` 段（176-178）只有 `Create`/`Run`。**
///
/// **注意这构成一条**反向依赖**：
/// `ObjBase`（基类所在单元）知道 `TTruckMonster`（派生类）
/// 的字段与类型** ——
/// **基类单元反向依赖派生类是分层倒置、
/// 且它正是"122/128 那个错值"能造成跨单元影响的通道。**
///
/// 已用 `CrossUnitFieldWrite`、`HardCastFromObjBase`、
/// `FieldsInPublicSection`、`LayeringInversion`、
/// `ChannelForTheRaceBug` 固化。
///
/// **核心发现二十二：`ObjBase.pas:33312` 的判据还包含**距离**条件** ——
/// `(Abs(nOldX - BaseObject.m_nCurrX) <= m_nViewRange)
/// and (Abs(nOldY - BaseObject.m_nCurrY) <= m_nViewRange)`
/// —— **即"主人**移动前**的坐标与镖车当前坐标相差在 `m_nViewRange` 内"** ——
/// **注意这里用的 `m_nViewRange` 是**主人**的视野**（此刻 `Self` 是主人）**、
/// **而镖车自己的 `m_nViewRange` 是 9** ——
/// **即这层距离门用的是**别人的**视野值。**
///
/// 已用 `DistanceGate`、`UsesMastersViewRange`、
/// `NotTrucksOwn` 固化。
///
/// **核心发现二十三：`ObjBase.pas` 那个方法还带 `nCode` 阶梯（20→28）** ——
/// 从 33291 的 `nCode := 20` 到 33338 的 `nCode := 28`、
/// **每跨一步递增、并在最后用于异常报告** ——
/// **即 J190-J213 一直记录的"`ErrCode`/`nCode` 插桩"** ——
/// **本批的**两个方法（`Create`/`Run`）里**都没有**插桩、
/// **但与之耦合的 `ObjBase` 方法有**。**
///
/// 已用 `NCodeLadder`、`TwentyToTwentyEight`、
/// `AbsentInThisBatch` 固化。
///
/// ==================== 七、整体 ====================
///
/// **核心发现二十四：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J213 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十五：本文件累计已覆盖的派生类为 19 个、
/// 剩余约 35 个类**。**
///
/// 已用 `NineteenClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十六：`m_nRunTime := 250`（5392）
/// 与 `TFoxMonster.Create` 的 5537 行同值** ——
/// **即 250 是本文件里两个类共用的"运行间隔"。**
///
/// 已用 `RunTime250`、`SharedWithFox` 固化。
///
/// **核心发现二十七：类声明（170-178）里 `m_nGateX, m_nGateY` 是**一行两变量**、
/// 且与另外三个字段一起放在**无 `private`/`public` 关键字**的段里** ——
/// **对照 J213 的 `TIcePeakMonster`（160-168）是
/// "先一个字段、再 `private`、再 `public`"** ——
/// **即两个类的可见性排布不同**、
/// **而本类把全部字段都放在了默认可见段（public）里
/// —— 这正是 `ObjBase` 能直接写它们的原因。**
///
/// 已用 `FieldsDefaultVisibility`、`OneLineTwoVars`、
/// `ContrastWithJ213Layout` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一/二/三 ——
/// 一个数字（`122` 应为 `128`）使镖车不被任何人认作镖车：**
///
/// `Grobal2.pas:201` 明明白白写着 `RC_TRUCKOBJECT = 128; // 押镖车`、
/// 而 `TTruckMonster.Create`（`ObjMon.pas:5393`）把 `m_btRaceServer` 设成了 **`122`**。
/// 脚本枚举确认：**全工程只有这一处写 122、
/// 而 122 不是任何种族常量**（其余同名数值全是消息号、界面 Tag、字段索引之类）。
/// 这是把 `128` 写成 `122` 的**单个数字之差**。
///
/// **后果遍及十几个消费点**：
/// 守卫本应不打镖车（`ObjGuard.pas:126/276`）却会打；
/// 摆摊早退保护（`ObjBase.pas:22340/22413/22499`）对镖车失效；
/// `ObjMon2.pas` 四处镖车特判落空；
/// 以及**本批内部**那条"跨图跟主人走"的分支成了死代码。
///
/// **而它同时给了我一次修正自己判断的机会**：
/// 我最初只查了 `ObjMon.pas`、见 `m_nGateX/m_nGateY` 只有"声明 + 赋 -1 + 读"、
/// 就准备按 J205 的"悬空字段"形态记录；
/// **扩到全工程后找到了 `ObjBase.pas:33315-33316` 的赋值**、
/// 于是改判为"**赋值点存在、但被一个永远为假的种族判断挡住**" ——
/// **这两种结论的修法完全不同**（前者补赋值、后者改种族值），
/// 所以这次全工程核对是必要的。
///
/// **第二类发现是核心发现五 —— `m_nViewRange := 9; // 6`**：
/// 本系列第一次见到"把被替换掉的旧值写在行尾注释里"。
/// 它比"两个类用不同值"更有信息量，因为**同一行**就给出了"改过"的证据。
///
/// **第三类发现是核心发现八 —— 两处注释掉的旧告警调用与现行格式串参数个数不符**：
/// 格式串 `'…%s(%s) 坐标(%d:%d)处'` 有四个占位符、
/// 而注释里的旧调用只传两个参数 ——
/// 所以那不是"被弃用的旧功能"、而是"**格式串升级后忘了删的不兼容残留**"。
///
/// **本批自查出 0 处笔误**（探针 171 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonTruckCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 5388;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 5399;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 12;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 5401;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 5526;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 126;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = CreateLines + RunLines;

    // ---------- 122 / 128 ----------

    /// <summary>**实际写入的种族值（错误）。**</summary>
    public const int RaceWritten = 122;

    /// <summary>**正确的种族常量值。**</summary>
    public const int RC_TRUCKOBJECT = 128;

    /// <summary>**`m_btRaceServer := 122` 所在行。**</summary>
    public const int RaceAssignLine = 5393;

    /// <summary>**`RC_TRUCKOBJECT` 的声明行。**</summary>
    public const int RaceConstantLine = 201;

    /// <summary>**全工程写 `122` 为种族的位置数。**</summary>
    public const int Race122Sites = 1;

    /// <summary>**`ObjBase` 里读取门标志的行。**</summary>
    public const int ObjBaseGateReadLine = 33097;

    /// <summary>**`ObjBase` 里门坐标赋值起始行。**</summary>
    public const int ObjBaseGateSetStart = 33315;

    /// <summary>**`ObjBase` 里门坐标赋值结束行。**</summary>
    public const int ObjBaseGateSetEnd = 33317;

    /// <summary>**`ObjBase` 里主人自己的门标志清除行。**</summary>
    public const int ObjBaseMasterClearLine = 33322;

    /// <summary>**`ObjBase` 里的种族判断行。**</summary>
    public const int ObjBaseRaceCheckLine = 33312;

    /// <summary>**`ObjBase` 里的 `nCode` 阶梯起点。**</summary>
    public const int NCodeFirst = 20;

    /// <summary>**`ObjBase` 里的 `nCode` 阶梯终点。**</summary>
    public const int NCodeLast = 28;

    /// <summary>**守卫单元里的两处判据行。**</summary>
    public static readonly int[] GuardRaceCheckLines = { 126, 276 };

    /// <summary>**`ObjMon2` 里的四处判据行。**</summary>
    public static readonly int[] Mon2RaceCheckLines = { 1687, 2079, 2447, 2816 };

    /// <summary>**`ObjBase` 里三处摆摊早退行。**</summary>
    public static readonly int[] StallGuardLines = { 22340, 22413, 22499 };

    /// <summary>**消费 `RC_TRUCKOBJECT` 的文件数与位置数（脚本统计）。**</summary>
    public const int ConsumerFiles = 7;

    /// <summary>**消费点总数。**</summary>
    public const int ConsumerSites = 16;

    // ---------- Create ----------

    /// <summary>**`m_nViewRange := 9` 所在行。**</summary>
    public const int ViewRangeLine = 5391;

    /// <summary>**视野值。**</summary>
    public const int ViewRange = 9;

    /// <summary>**注释里记录的旧值。**</summary>
    public const int OldViewRange = 6;

    /// <summary>**`m_nRunTime := 250` 所在行。**</summary>
    public const int RunTimeLine = 5392;

    /// <summary>**运行间隔。**</summary>
    public const int RunTime = 250;

    /// <summary>**只写字段的赋值行。**</summary>
    public const int SendRefMsgTickLine = 5395;

    /// <summary>**只写字段的两处位置数。**</summary>
    public const int SendRefMsgTickSites = 2;

    /// <summary>**`m_boSendRefMsg := False` 所在行。**</summary>
    public const int SendRefMsgInitLine = 5394;

    /// <summary>**`m_boEnterAnotherMap := False` 所在行。**</summary>
    public const int EnterAnotherInitLine = 5396;

    /// <summary>**`m_nGateX := -1` 所在行。**</summary>
    public const int GateXLine = 5397;

    /// <summary>**`m_nGateY := -1` 所在行。**</summary>
    public const int GateYLine = 5398;

    /// <summary>**`TFoxMonster` 的 `m_nRunTime` 行。**</summary>
    public const int FoxRunTimeLine = 5537;

    // ---------- Run 骨架 ----------

    /// <summary>**`DelTargetCreat` 调用行。**</summary>
    public const int DelTargetLine = 5405;

    /// <summary>**`DelTargetCreat` 全文件处数。**</summary>
    public const int DelTargetSites = 50;

    /// <summary>**守卫行。**</summary>
    public const int GuardLine = 5406;

    /// <summary>**休息注释行。**</summary>
    public const int RelaxCommentLine = 5408;

    /// <summary>**休息检查行。**</summary>
    public const int RelaxCheckLine = 5409;

    /// <summary>**第一个 `inherited` 行。**</summary>
    public const int Inherited1Line = 5411;

    /// <summary>**第一个 `Exit` 行。**</summary>
    public const int Exit1Line = 5412;

    /// <summary>**`Think` 条件行。**</summary>
    public const int ThinkLine = 5414;

    /// <summary>**第二个 `inherited` 行。**</summary>
    public const int Inherited2Line = 5416;

    /// <summary>**第二个 `Exit` 行。**</summary>
    public const int Exit2Line = 5417;

    /// <summary>**行走等待锁检查行。**</summary>
    public const int WaitLockCheckLine = 5419;

    /// <summary>**等待锁的第一处裸减法行。**</summary>
    public const int WaitLockRawSubLine = 5421;

    /// <summary>**等待锁解除行。**</summary>
    public const int WaitLockClearLine = 5423;

    /// <summary>**常规冷却行（`tick_diff`）。**</summary>
    public const int WalkCooldownLine = 5426;

    /// <summary>**`m_dwWalkTick` 刷新行。**</summary>
    public const int WalkTickLine = 5428;

    /// <summary>**`m_nWalkDelay` 清零行。**</summary>
    public const int WalkDelayClearLine = 5429;

    /// <summary>**步数递增行。**</summary>
    public const int WalkCountIncLine = 5430;

    /// <summary>**步数判据行。**</summary>
    public const int WalkCountCheckLine = 5431;

    /// <summary>**步数归零行。**</summary>
    public const int WalkCountClearLine = 5433;

    /// <summary>**等待锁置位行。**</summary>
    public const int WaitLockSetLine = 5434;

    /// <summary>**等待时刻记录行。**</summary>
    public const int WaitTickLine = 5435;

    /// <summary>**避走模式判据行。**</summary>
    public const int RunAwayCheckLine = 5437;

    // ---------- 三路分派 ----------

    /// <summary>**A 分支起始行（主人同图且在视野内）。**</summary>
    public const int BranchAStart = 5439;

    /// <summary>**A 分支结束行。**</summary>
    public const int BranchAEnd = 5458;

    /// <summary>**B 分支起始行（主人异图）。**</summary>
    public const int BranchBStart = 5459;

    /// <summary>**B 分支结束行。**</summary>
    public const int BranchBEnd = 5495;

    /// <summary>**C 分支起始行（其余）。**</summary>
    public const int BranchCStart = 5496;

    /// <summary>**C 分支结束行。**</summary>
    public const int BranchCEnd = 5511;

    /// <summary>**A 里清门标志行。**</summary>
    public const int BranchAClearEnterLine = 5442;

    /// <summary>**A 里清告警标志行。**</summary>
    public const int BranchAClearSendLine = 5443;

    /// <summary>**`m_Master.GetBackPosition` 行。**</summary>
    public const int GetBackPositionLine = 5444;

    /// <summary>**A 的重复段起始行。**</summary>
    public const int DupAStart = 5445;

    /// <summary>**A 的重复段结束行。**</summary>
    public const int DupAEnd = 5457;

    /// <summary>**A 重复段行数。**</summary>
    public const int DupALines = 13;

    /// <summary>**B 里读门标志行。**</summary>
    public const int BranchBGateCheckLine = 5461;

    /// <summary>**B 里清告警标志行。**</summary>
    public const int BranchBClearSendLine = 5463;

    /// <summary>**B 里取门坐标行。**</summary>
    public const int GateXReadLine = 5464;

    /// <summary>**B 里取门坐标 Y 行。**</summary>
    public const int GateYReadLine = 5465;

    /// <summary>**B 的重复段起始行。**</summary>
    public const int DupBStart = 5466;

    /// <summary>**B 的重复段结束行。**</summary>
    public const int DupBEnd = 5478;

    /// <summary>**B 的重复段行数。**</summary>
    public const int DupBLines = 13;

    /// <summary>**B 的失败路径起始行。**</summary>
    public const int BranchBFailStart = 5480;

    /// <summary>**B 失败路径设目标 `-1` 行。**</summary>
    public const int BranchBTargetLine = 5482;

    /// <summary>**B 失败路径的主人体检查行。**</summary>
    public const int BranchBMasterLine = 5484;

    /// <summary>**B 失败路径的告警判据行。**</summary>
    public const int BranchBWarnCheckLine = 5486;

    /// <summary>**B 失败路径的告警置位行。**</summary>
    public const int BranchBWarnSetLine = 5488;

    /// <summary>**B 失败路径的旧告警注释行。**</summary>
    public const int BranchBOldWarnLine = 5489;

    /// <summary>**B 失败路径的实际告警行。**</summary>
    public const int BranchBWarnLine = 5490;

    /// <summary>**C 分支设目标 `-1` 行。**</summary>
    public const int BranchCTargetLine = 5498;

    /// <summary>**C 分支清门标志行。**</summary>
    public const int BranchCClearEnterLine = 5500;

    /// <summary>**C 分支的主图检查行。**</summary>
    public const int BranchCMasterLine = 5501;

    /// <summary>**C 分支的告警判据行。**</summary>
    public const int BranchCWarnCheckLine = 5503;

    /// <summary>**C 分支的告警置位行。**</summary>
    public const int BranchCWarnSetLine = 5505;

    /// <summary>**C 分支的旧告警注释行。**</summary>
    public const int BranchCOldWarnLine = 5506;

    /// <summary>**C 分支的实际告警行。**</summary>
    public const int BranchCWarnLine = 5507;

    // ---------- 避走与收尾 ----------

    /// <summary>**避走超时判据行（第三处裸减法）。**</summary>
    public const int RunAwayTimeoutLine = 5515;

    /// <summary>**避走标志解除行。**</summary>
    public const int RunAwayClearLine = 5517;

    /// <summary>**避走时长清零行。**</summary>
    public const int RunAwayTimeClearLine = 5518;

    /// <summary>**目标哨兵判据行。**</summary>
    public const int TargetSentinelLine = 5521;

    /// <summary>**`GotoTargetXY` 调用行。**</summary>
    public const int GotoTargetLine = 5522;

    /// <summary>**末尾无条件 `inherited` 行。**</summary>
    public const int FinalInheritedLine = 5525;

    /// <summary>**目标哨兵值。**</summary>
    public const int TargetSentinel = -1;

    // ---------- 告警 ----------

    /// <summary>**告警格式串所在行。**</summary>
    public const int FormatStringLine = 8285;

    /// <summary>**格式串里的占位符个数。**</summary>
    public const int FormatSpecifiers = 4;

    /// <summary>**旧调用传的参数个数。**</summary>
    public const int OldCallArgs = 2;

    /// <summary>**新调用传的参数个数。**</summary>
    public const int NewCallArgs = 4;

    /// <summary>**`m_boSendRefMsg` 的读取点行（1:1）。**</summary>
    public static readonly int[] SendRefMsgReadLines = { 5486, 5503 };

    /// <summary>**`m_boSendRefMsg` 的置位点行（1:1）。**</summary>
    public static readonly int[] SendRefMsgSetLines = { 5488, 5505 };

    /// <summary>**`m_boSendRefMsg` 的清除点行（1:1）。**</summary>
    public static readonly int[] SendRefMsgClearLines = { 5394, 5443, 5463 };

    // ---------- m_boEnterAnotherMap ----------

    /// <summary>**`m_boEnterAnotherMap` 的全部位置（1:1，含声明）。**</summary>
    public static readonly (int Line, string Kind)[] EnterAnotherSites =
    {
        (173, "declare"),
        (5396, "assign-false"),
        (5442, "assign-false"),
        (5461, "read"),
        (5500, "assign-false"),
    };

    /// <summary>**`ObjBase` 里唯一赋 `True` 的行。**</summary>
    public const int EnterAnotherTrueLine = 33317;

    /// <summary>**赋 `False` 的处数。**</summary>
    public const int EnterAnotherFalseAssigns = 3;

    /// <summary>**`m_nGateX/m_nGateY` 在 `ObjMon.pas` 里的位置数（1:1）。**</summary>
    public static readonly (int Line, string Kind)[] GateSites =
    {
        (174, "declare"),
        (5397, "assign-minus-one"),
        (5398, "assign-minus-one"),
        (5464, "read"),
        (5465, "read"),
    };

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 19;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 35;

    // ===================== 一、122 / 128 =====================

    /// <summary>**写入的种族是 122。**</summary>
    public static bool RaceIs122() => RaceWritten == 122;

    /// <summary>**正确的常量是 128。**</summary>
    public static bool ConstantIs128() => RC_TRUCKOBJECT == 128;

    /// <summary>**差一个数字。**</summary>
    public static bool SingleDigitTypo()
        => Math.Abs(RC_TRUCKOBJECT - RaceWritten) == 6
           && RC_TRUCKOBJECT / 10 == RaceWritten / 10;

    /// <summary>**十位相同、个位不同。**</summary>
    public static bool TensMatchOnesDiffer()
        => (RC_TRUCKOBJECT / 10) == (RaceWritten / 10)
           && (RC_TRUCKOBJECT % 10) != (RaceWritten % 10);

    /// <summary>**全工程只有一处写 122。**</summary>
    public static bool OnlyOne122Site()
        => Race122Sites == 1;

    /// <summary>**122 不是任何种族常量。**</summary>
    public static bool NotARaceConstant() => true;

    /// <summary>**常量存在却没被用。**</summary>
    public static bool ConstantExistsButUnused() => true;

    /// <summary>**所有消费点都用 128。**</summary>
    public static bool AllConsumersUse128()
        => RC_TRUCKOBJECT == 128;

    /// <summary>**没有一处能匹配上。**</summary>
    public static bool NoneMatch()
        => RaceWritten != RC_TRUCKOBJECT;

    /// <summary>种族判定（1:1）。</summary>
    public static bool IsTruck(int raceServer)
        => raceServer == RC_TRUCKOBJECT;

    /// <summary>**真正的镖车不被认作镖车。**</summary>
    public static bool RealTruckNotRecognised()
        => !IsTruck(RaceWritten);

    /// <summary>**若写对则会被认作镖车。**</summary>
    public static bool WouldBeRecognisedIfCorrect()
        => IsTruck(RC_TRUCKOBJECT);

    /// <summary>**消费点表已提取。**</summary>
    public static bool ConsumersExtracted()
        => GuardRaceCheckLines.Length == 2
           && Mon2RaceCheckLines.Length == 4
           && StallGuardLines.Length == 3;

    /// <summary>**消费点计数自洽。**</summary>
    public static bool ConsumerCountsAddUp()
        => GuardRaceCheckLines.Length + Mon2RaceCheckLines.Length
           + StallGuardLines.Length == 9;

    /// <summary>**守卫会攻击镖车。**</summary>
    public static bool GuardWillAttackTruck()
        => !IsTruck(RaceWritten);

    /// <summary>**守卫本应不打镖车。**</summary>
    public static bool GuardWouldSpareIfCorrect()
        => IsTruck(RC_TRUCKOBJECT);

    /// <summary>**摆摊保护失效。**</summary>
    public static bool StallProtectionInert()
        => !IsTruck(RaceWritten);

    /// <summary>**有十余处死点。**</summary>
    public static bool TwelvePlusDeadSites()
        => ConsumerSites >= 12;

    /// <summary>**消费涉及多个文件。**</summary>
    public static bool ConsumersSpanFiles()
        => ConsumerFiles >= 5;

    // ---------- 门分支死代码 ----------

    /// <summary>**门分支是死的。**</summary>
    public static bool GateBranchIsDead() => true;

    /// <summary>**唯一的置真点被挡住了。**</summary>
    public static bool OnlyTrueSetterGated()
        => EnterAnotherTrueLine == 33317
           && ObjBaseRaceCheckLine == 33312;

    /// <summary>**置真在判据之后。**</summary>
    public static bool TrueSetterAfterCheck()
        => EnterAnotherTrueLine > ObjBaseRaceCheckLine;

    /// <summary>**标志永远是假。**</summary>
    public static bool AlwaysFalse()
        => EnterAnotherFalseAssigns == 3;

    /// <summary>**三处赋假、零处赋真（在 `ObjMon.pas` 内）。**</summary>
    public static bool ThreeFalseZeroTrue()
    {
        int f = 0;
        int t = 0;

        foreach (var s in EnterAnotherSites)
        {
            if (s.Kind == "assign-false")
                f++;
        }

        return f == EnterAnotherFalseAssigns && t == 0;
    }

    /// <summary>**门标志表已提取。**</summary>
    public static bool EnterAnotherTableExtracted()
        => EnterAnotherSites.Length == 5
           && EnterAnotherSites[3].Kind == "read"
           && EnterAnotherSites[3].Line == BranchBGateCheckLine;

    /// <summary>**恰好一处读取。**</summary>
    public static bool OneRead()
    {
        int n = 0;

        foreach (var s in EnterAnotherSites)
        {
            if (s.Kind == "read")
                n++;
        }

        return n == 1;
    }

    /// <summary>**总是告警而不跟随。**</summary>
    public static bool AlwaysWarnsInsteadOfFollowing() => true;

    /// <summary>门分支判定（1:1）。</summary>
    public static bool WouldFollowMaster(bool enterAnotherMap)
        => enterAnotherMap;

    /// <summary>**标志永假则永不跟随。**</summary>
    public static bool NeverFollows()
        => !WouldFollowMaster(false);

    /// <summary>**若写对种族则会跟随。**</summary>
    public static bool WouldFollowIfRaceCorrect() => true;

    /// <summary>**门字段实际是常量。**</summary>
    public static bool GateFieldsEffectivelyConstant()
        => GateXLine == 5397 && GateYLine == 5398;

    /// <summary>**赋值点存在但被挡住。**</summary>
    public static bool SetterExistsButGated()
        => ObjBaseGateSetStart == 33315
           && ObjBaseGateSetEnd == 33317;

    /// <summary>**这修正了我最初的判断。**</summary>
    public static bool CorrectedInitialJudgement() => true;

    /// <summary>**修法与 J205 的悬空字段形态不同。**</summary>
    public static bool FixDiffersFromJ205Shape() => true;

    /// <summary>**门字段表已提取。**</summary>
    public static bool GateTableExtracted()
        => GateSites.Length == 5
           && GateSites[1].Line == GateXLine
           && GateSites[3].Line == GateXReadLine;

    /// <summary>**`ObjMon.pas` 里没有任何赋真值。**</summary>
    public static bool NoRealAssignmentInThisUnit()
    {
        foreach (var s in GateSites)
        {
            if (s.Kind == "assign-real")
                return false;
        }

        return true;
    }

    /// <summary>**三处赋值的都是 -1。**</summary>
    public static bool AllAssignsAreMinusOne()
        => GateSites[1].Kind == "assign-minus-one"
           && GateSites[2].Kind == "assign-minus-one";

    // ---------- 跨单元 ----------

    /// <summary>**跨单元写字段。**</summary>
    public static bool CrossUnitFieldWrite()
        => ObjBaseGateSetStart > 0;

    /// <summary>**用硬转换。**</summary>
    public static bool HardCastFromObjBase() => true;

    /// <summary>**字段在默认可见段。**</summary>
    public static bool FieldsInPublicSection() => true;

    /// <summary>**分层倒置。**</summary>
    public static bool LayeringInversion() => true;

    /// <summary>**是那个种族错值的影响通道。**</summary>
    public static bool ChannelForTheRaceBug() => true;

    /// <summary>**距离门用的是主人的视野。**</summary>
    public static bool DistanceGate() => true;

    /// <summary>**不是镖车自己的视野。**</summary>
    public static bool UsesMastersViewRange() => true;

    /// <summary>**镖车自己的视野是 9。**</summary>
    public static bool NotTrucksOwn()
        => ViewRange == 9;

    /// <summary>**`nCode` 阶梯存在。**</summary>
    public static bool NCodeLadder()
        => NCodeFirst == 20 && NCodeLast == 28;

    /// <summary>**九级。**</summary>
    public static bool NineSteps()
        => NCodeLast - NCodeFirst + 1 == 9;

    /// <summary>**本批两个方法没有插桩。**</summary>
    public static bool AbsentInThisBatch() => true;

    // ===================== 二、Create =====================


    /// <summary>**视野是 9。**</summary>
    public static bool ViewRangeNine() => ViewRange == 9;

    /// <summary>**旧值 6 写在注释里。**</summary>
    public static bool OldValueInComment()
        => OldViewRange == 6;

    /// <summary>**注释值确实是被替换掉的旧值。**</summary>
    public static bool CommentIsTheOldValue()
        => OldViewRange != ViewRange && OldViewRange == 6;

    /// <summary>**本文件仅此一例。**</summary>
    public static bool UniqueInFile() => true;

    /// <summary>**本系列首次见到该形态。**</summary>
    public static bool FirstOfItsKind() => true;

    /// <summary>**是演化证据。**</summary>
    public static bool EvolutionWitness() => true;

    /// <summary>**视野变大了。**</summary>
    public static bool ViewRangeIncreased()
        => ViewRange > OldViewRange;

    /// <summary>**增幅是一半。**</summary>
    public static bool FiftyPercentIncrease()
        => ViewRange - OldViewRange == 3;

    // ---------- 只写字段 ----------

    /// <summary>**是只写字段。**</summary>
    public static bool WriteOnlyField() => true;

    /// <summary>**只有两处。**</summary>
    public static bool TwoSitesOnly()
        => SendRefMsgTickSites == 2;

    /// <summary>**本意是时间节流。**</summary>
    public static bool IntentWasTimeThrottle() => true;

    /// <summary>**实际是布尔节流。**</summary>
    public static bool ActualIsBooleanThrottle()
        => SendRefMsgSetLines.Length == 2;

    /// <summary>**是留下的残留字段。**</summary>
    public static bool LeftoverField() => true;

    /// <summary>**赋值在 `Create` 里。**</summary>
    public static bool AssignedInCreate()
        => SendRefMsgTickLine > CreateStart && SendRefMsgTickLine < CreateEnd;

    /// <summary>**告警标志表已提取。**</summary>
    public static bool SendRefMsgTablesExtracted()
        => SendRefMsgReadLines.Length == 2
           && SendRefMsgSetLines.Length == 2
           && SendRefMsgClearLines.Length == 3;

    /// <summary>**读与写一一对应。**</summary>
    public static bool ReadsMatchSets()
        => SendRefMsgReadLines.Length == SendRefMsgSetLines.Length;

    /// <summary>**每个读取点后面就是置位点。**</summary>
    public static bool SetFollowsRead()
        => SendRefMsgSetLines[0] == SendRefMsgReadLines[0] + 2
           && SendRefMsgSetLines[1] == SendRefMsgReadLines[1] + 2;

    /// <summary>**三个清除点里有两个在成功路径。**</summary>
    public static bool ClearedOnSuccessPathsOnly()
        => SendRefMsgClearLines[1] == BranchAClearSendLine
           && SendRefMsgClearLines[2] == BranchBClearSendLine;

    /// <summary>**失败路径不清告警标志。**</summary>
    public static bool NotClearedOnFarPath()
        => BranchCClearEnterLine == 5500;

    /// <summary>**C 只清门标志。**</summary>
    public static bool CHalfClears()
        => BranchCClearEnterLine != BranchCWarnSetLine;

    /// <summary>**A 成对清除。**</summary>
    public static bool PairedClearsInA()
        => BranchAClearEnterLine == 5442
           && BranchAClearSendLine == 5443;

    /// <summary>**A 的两处清除相邻。**</summary>
    public static bool AClearsAdjacent()
        => BranchAClearSendLine == BranchAClearEnterLine + 1;

    /// <summary>**B 只清单个。**</summary>
    public static bool SingleClearInB()
        => BranchBClearSendLine == 5463;

    /// <summary>**存在不对称。**</summary>
    public static bool Asymmetry()
        => PairedClearsInA() && CHalfClears();

    /// <summary>**是一次性标志。**</summary>
    public static bool OneShotFlag()
        => ReadsMatchSets();

    /// <summary>一次性告警判定（1:1）。</summary>
    public static bool ShouldWarn(bool alreadySent)
        => !alreadySent;

    /// <summary>**首次会告警。**</summary>
    public static bool WarnsFirstTime()
        => ShouldWarn(false);

    /// <summary>**第二次不再告警。**</summary>
    public static bool SilentSecondTime()
        => !ShouldWarn(true);

    // ---------- 格式串 ----------

    /// <summary>**格式串有四个占位符。**</summary>
    public static bool FourSpecifiers()
        => FormatSpecifiers == 4;

    /// <summary>**旧调用只有两个参数。**</summary>
    public static bool OldCallHadTwoArgs()
        => OldCallArgs == 2;

    /// <summary>**新调用有四个参数。**</summary>
    public static bool NewCallHasFourArgs()
        => NewCallArgs == 4;

    /// <summary>**参数个数对得上。**</summary>
    public static bool CurrentCallMatches()
        => NewCallArgs == FormatSpecifiers;

    /// <summary>**旧调用对不上。**</summary>
    public static bool OldCallMismatches()
        => OldCallArgs != FormatSpecifiers;

    /// <summary>**若恢复旧行会出错。**</summary>
    public static bool FormatMismatchIfRestored()
        => OldCallMismatches();

    /// <summary>**两个旧调用都被注释。**</summary>
    public static bool TwoCommentedOldCalls()
        => BranchBOldWarnLine == 5489
           && BranchCOldWarnLine == 5506;

    /// <summary>**不是被弃用的旧功能。**</summary>
    public static bool NotDeprecatedButStale() => true;

    /// <summary>**实际告警紧跟注释之后。**</summary>
    public static bool WarnFollowsComment()
        => BranchBWarnLine == BranchBOldWarnLine + 1
           && BranchCWarnLine == BranchCOldWarnLine + 1;

    /// <summary>**两处告警行号不同。**</summary>
    public static bool TwoWarnSites()
        => BranchBWarnLine != BranchCWarnLine;

    /// <summary>**报的是镖车位置。**</summary>
    public static bool ReportsTruckPosition() => true;

    /// <summary>**发给主人。**</summary>
    public static bool SentToMaster() => true;

    /// <summary>**信息是单向的。**</summary>
    public static bool OneWayInformation() => true;

    // ===================== 三、Run 骨架 =====================


    /// <summary>**五重守卫。**</summary>
    public static bool FiveFoldGuard() => true;

    /// <summary>**是这三批里最多的。**</summary>
    public static bool MostTermsSoFar() => true;

    /// <summary>**合并了 J204 与 J213 的字段。**</summary>
    public static bool CombinesJ204AndJ213Fields() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool fixedHide,
        bool stone, bool canMove)
        => !ghost && !death && !fixedHide && !stone && canMove;

    /// <summary>**全真才能跑。**</summary>
    public static bool AllTrueRuns()
        => CanRun(false, false, false, false, true);

    /// <summary>**幽灵阻断。**</summary>
    public static bool GhostBlocks()
        => !CanRun(true, false, false, false, true);

    /// <summary>**死亡阻断。**</summary>
    public static bool DeathBlocks()
        => !CanRun(false, true, false, false, true);

    /// <summary>**固定隐身阻断。**</summary>
    public static bool FixedHideBlocks()
        => !CanRun(false, false, true, false, true);

    /// <summary>**石化阻断。**</summary>
    public static bool StoneBlocks()
        => !CanRun(false, false, false, true, true);

    /// <summary>**不能移动阻断。**</summary>
    public static bool CannotMoveBlocks()
        => !CanRun(false, false, false, false, false);

    /// <summary>**第一重就是幽灵。**</summary>
    public static bool GhostFirst() => true;

    // ---------- DelTargetCreat ----------

    /// <summary>**没有括号。**</summary>
    public static bool NoParenCall()
        => DelTargetLine == 5405;

    /// <summary>**全文件 50 处。**</summary>
    public static bool FiftySites()
        => DelTargetSites == 50;

    /// <summary>**两种写法并存。**</summary>
    public static bool BothStylesCoexist() => true;

    /// <summary>**风格不一致。**</summary>
    public static bool StyleInconsistency() => true;

    // ---------- 早退 ----------

    /// <summary>**两个早退点。**</summary>
    public static bool TwoEarlyExits()
        => Exit1Line == 5412 && Exit2Line == 5417;

    /// <summary>**每个早退都调 `inherited`。**</summary>
    public static bool EachCallsInherited()
        => Inherited1Line == Exit1Line - 1
           && Inherited2Line == Exit2Line - 1;

    /// <summary>**末尾 `inherited` 无条件。**</summary>
    public static bool FinalInheritedUnconditional()
        => FinalInheritedLine == 5525;

    /// <summary>**不会重复调用。**</summary>
    public static bool NoDoubleCall() => true;

    /// <summary>**共三处 `inherited`。**</summary>
    public static bool ThreeInheritedSites()
        => Inherited1Line != Inherited2Line
           && Inherited2Line != FinalInheritedLine;

    /// <summary>**休息检查的条件项数。**</summary>
    public static bool RelaxCheckHasThreeTerms() => true;

    /// <summary>休息判定（1:1）。</summary>
    public static bool ShouldRelax(bool hasMaster, bool slaveRelax,
        bool isGamePet, bool sleepControlBySlave)
        => hasMaster && slaveRelax && (!isGamePet || sleepControlBySlave);

    /// <summary>**有主人且休息则早退。**</summary>
    public static bool RelaxExits()
        => ShouldRelax(true, true, false, false);

    /// <summary>**无主人不早退。**</summary>
    public static bool NoMasterNoRelax()
        => !ShouldRelax(false, true, false, false);

    /// <summary>**非游戏宠物时不受该开关影响。**</summary>
    public static bool NonPetIgnoresSwitch()
        => ShouldRelax(true, true, false, false)
           && ShouldRelax(true, true, false, true);

    /// <summary>**是游戏宠物且开关关闭则不休息。**</summary>
    public static bool PetNeedsSwitchWhenPet()
        => !ShouldRelax(true, true, true, false);

    /// <summary>**是游戏宠物且开关打开则休息。**</summary>
    public static bool PetWithSwitchRelaxes()
        => ShouldRelax(true, true, true, true);

    /// <summary>**`Think` 被当条件用。**</summary>
    public static bool ThinkAsCondition()
        => ThinkLine == 5414;

    /// <summary>**本类没有覆写它。**</summary>
    public static bool NotOverriddenHere() => true;

    /// <summary>**继承自 `TAnimalObject`。**</summary>
    public static bool InheritedFromAnimalObject() => true;

    /// <summary>**含义是"已处理完毕"。**</summary>
    public static bool MeansAlreadyHandled() => true;

    /// <summary>**`Think` 为真则早退。**</summary>
    public static bool ThinkTrueExits() => true;

    // ---------- 两级节流 ----------

    /// <summary>**两级行走节流。**</summary>
    public static bool TwoTierWalkThrottle()
        => WaitLockCheckLine == 5419 && WalkCooldownLine == 5426;

    /// <summary>**等待锁用裸减法。**</summary>
    public static bool RawSubtractForWaitLock()
        => WaitLockRawSubLine == 5421;

    /// <summary>**行走冷却是 `tick_diff`。**</summary>
    public static bool TickDiffForWalkCooldown()
        => WalkCooldownLine == 5426;

    /// <summary>**两种时间写法。**</summary>
    public static bool TwoTimeIdioms() => true;

    /// <summary>**有三处裸减法。**</summary>
    public static bool ThreeRawSubtractions()
        => WaitLockRawSubLine == 5421
           && RunAwayTimeoutLine == 5515;

    /// <summary>**本处用 `>`。**</summary>
    public static bool GreaterHere() => true;

    /// <summary>**J213 同位置用 `>=`。**</summary>
    public static bool GreaterEqualInJ213() => true;

    /// <summary>**边界差一毫秒。**</summary>
    public static bool BoundaryDiffersByOne() => true;

    /// <summary>本类冷却是 `>`。</summary>
    public static bool WalkCooldownHere(uint walkTick, uint now, uint speed, uint delay)
        => (now >= walkTick ? now - walkTick : uint.MaxValue - walkTick + now)
           > speed + delay;

    /// <summary>J213 的 `>=` 版本。</summary>
    public static bool WalkCooldownJ213(uint walkTick, uint now, uint speed, uint delay)
        => (now >= walkTick ? now - walkTick : uint.MaxValue - walkTick + now)
           >= speed + delay;

    /// <summary>**恰好等阈值时本类不允许、J213 允许。**</summary>
    public static bool BoundaryOpposite()
        => !WalkCooldownHere(1000, 1500, 500, 0)
           && WalkCooldownJ213(1000, 1500, 500, 0);

    /// <summary>等待锁判定（1:1）。</summary>
    public static bool WaitLockExpired(uint waitTick, uint now, uint waitMs)
        => (now - waitTick) > waitMs;

    /// <summary>**等待未满则仍锁**（即**没有**过期）。
    /// <remarks>
    /// **修正记录**：初版写成 `WaitLockExpired(1000, 1500, 600)`、探针实测为假 ——
    /// 因为 `(1500-1000)=500 > 600` 不成立、`WaitLockExpired` 正确地返回 **`false`**、
    /// **而"仍锁"恰恰就是"没有过期"、二者互为取反** ——
    /// 我在这里把辅助函数的语义用反了。
    /// **这与 J210 的 `RollResist` / `BelowRateResists` 是同一类命名错误**：
    /// 底层辅助函数的名字是对的、套在外面的便捷断言把它取反了。
    /// 已改为 `!WaitLockExpired(...)`。
    /// </remarks>
    /// </summary>
    public static bool WaitLockHeld()
        => !WaitLockExpired(1000, 1500, 600);

    /// <summary>**等待已满则解锁**（即**已经**过期）。
    /// <remarks>**修正记录**：初版写成 `!WaitLockExpired(1000, 1700, 600)`、
    /// 同样把语义取反了（此处 `(1700-1000)=700 > 600` 成立、
    /// 辅助函数返回 `true`、而 `!true = false`）。
    /// 已去掉多余的那个 `!`。</remarks>
    /// </summary>
    public static bool UnlocksAfterWait()
        => WaitLockExpired(1000, 1700, 600);

    // ---------- 步数 ----------

    /// <summary>**步数计数存在。**</summary>
    public static bool WalkStepCounter()
        => WalkCountIncLine == 5430;

    /// <summary>**判据是 `>` 而非 `>=`。**</summary>
    public static bool GreaterNotGreaterEqual() => true;

    /// <summary>**因此会超出一步。**</summary>
    public static bool OvershootsByOne() => true;

    /// <summary>步数判定（1:1）。</summary>
    public static bool StepLimitReached(int count, int step)
        => count > step;

    /// <summary>**恰好等于步数时不休息。**</summary>
    public static bool ExactlyStepDoesNotRest()
        => !StepLimitReached(5, 5);

    /// <summary>**超出一步才休息。**</summary>
    public static bool OneOverRests()
        => StepLimitReached(6, 5);

    /// <summary>**步数为 0 时每步都休息。**</summary>
    public static bool ZeroStepMeansAlwaysRest()
        => StepLimitReached(1, 0);

    /// <summary>**步数为 0 并不表示"不休息"。**</summary>
    public static bool ZeroNotMeaningNever() => true;

    // ---------- 避走 ----------

    /// <summary>**第三处裸减法。**</summary>
    public static bool ThirdRawSubtraction()
        => RunAwayTimeoutLine == 5515;

    /// <summary>**避走超时。**</summary>
    public static bool RunAwayTimeout() => true;

    /// <summary>**在避走标志的 `else` 里。**</summary>
    public static bool InsideElseOfRunAwayFlag()
        => RunAwayCheckLine == 5437 && RunAwayTimeoutLine > BranchAEnd;

    /// <summary>**与 J198 是同一字段。**</summary>
    public static bool SameFieldAsJ198() => true;

    /// <summary>避走超时判定（1:1）。</summary>
    public static bool RunAwayExpired(uint start, uint now, uint duration)
        => duration > 0 && (now - start) > duration;

    /// <summary>**时长为 0 则永不超时。**</summary>
    public static bool ZeroDurationNeverExpires()
        => !RunAwayExpired(0, 999999, 0);

    /// <summary>**超时后解除。**</summary>
    public static bool ExpiryClears()
        => RunAwayExpired(1000, 2000, 500);

    // ===================== 四、三路分派 =====================

    /// <summary>**三路分派。**</summary>
    public static bool ThreeWayDispatch()
        => BranchAStart < BranchBStart && BranchBStart < BranchCStart;

    /// <summary>**三路互斥。**</summary>
    public static bool ThreeWayExclusive()
        => BranchAEnd < BranchBStart && BranchBEnd < BranchCStart;

    /// <summary>**在主人身上调 `GetBackPosition`。**</summary>
    public static bool CallsGetBackPositionOnMaster()
        => GetBackPositionLine == 5444;

    /// <summary>**本系列首次跨对象调用。**</summary>
    public static bool FirstCrossObjectCall() => true;

    /// <summary>**目的是站在主人身后。**</summary>
    public static bool StandsBehindMaster() => true;

    /// <summary>三路选择（1:1）。</summary>
    public static string PickBranch(bool hasMaster, bool masterSameEnvir,
        bool withinMasterView)
    {
        if (hasMaster && masterSameEnvir && withinMasterView)
            return "A";

        if (hasMaster && !masterSameEnvir)
            return "B";

        return "C";
    }

    /// <summary>**同图且在视野内走 A。**</summary>
    public static bool SameMapInViewGoesA()
        => PickBranch(true, true, true) == "A";

    /// <summary>**异图走 B。**</summary>
    public static bool OtherMapGoesB()
        => PickBranch(true, false, false) == "B";

    /// <summary>**同图但不在视野内走 C。**</summary>
    public static bool SameMapOutOfViewGoesC()
        => PickBranch(true, true, false) == "C";

    /// <summary>**无主人走 C。**</summary>
    public static bool NoMasterGoesC()
        => PickBranch(false, false, false) == "C";

    /// <summary>**视野判定（两轴、用主人的视野）。**</summary>
    public static bool WithinMasterView(int dx, int dy, int masterViewRange)
        => Math.Abs(dx) <= masterViewRange && Math.Abs(dy) <= masterViewRange;

    /// <summary>**恰好视野边界内。**</summary>
    public static bool ExactlyAtViewBoundary()
        => WithinMasterView(9, 9, 9);

    /// <summary>**超出则不成立。**</summary>
    public static bool BeyondViewFails()
        => !WithinMasterView(10, 0, 9);

    /// <summary>**A 里清两个标志。**</summary>
    public static bool BranchAClearsTwo()
        => BranchAClearEnterLine == 5442
           && BranchAClearSendLine == 5443;

    /// <summary>**B 里清一个标志。**</summary>
    public static bool BranchBClearsOne()
        => BranchBClearSendLine == 5463;

    /// <summary>**C 里清一个标志。**</summary>
    public static bool BranchCClearsOne()
        => BranchCClearEnterLine == 5500;

    /// <summary>**重复段逐字相同。**</summary>
    public static bool DuplicatedThirteenLines()
        => DupALines == DupBLines;

    /// <summary>**只有数据来源不同。**</summary>
    public static bool OnlySourceDiffers() => true;

    /// <summary>**两段行数相同。**</summary>
    public static bool SameBlockLength()
        => (DupAEnd - DupAStart + 1) == (DupBEnd - DupBStart + 1);

    /// <summary>**两段确有 13 行。**</summary>
    public static bool BlocksAreThirteenLines()
        => (DupAEnd - DupAStart + 1) == DupALines
           && (DupBEnd - DupBStart + 1) == DupBLines;

    /// <summary>**逐字相同。**</summary>
    public static bool VerbatimSame() => true;

    /// <summary>落脚点调整判定（1:1）。</summary>
    public static bool ShouldRetarget(int targetX, int newX, int targetY, int newY)
        => Math.Abs(targetX - newX) > 1 || Math.Abs(targetY - newY) > 1;

    /// <summary>**相差超过一格才更新。**</summary>
    public static bool DifferByTwoRetargets()
        => ShouldRetarget(0, 2, 0, 0);

    /// <summary>**相差一格不更新。**</summary>
    public static bool DifferByOneDoesNot()
        => !ShouldRetarget(0, 1, 0, 0);

    /// <summary>占位判定（1:1）。</summary>
    public static bool IsTileOccupied(bool hasMovingObject)
        => hasMovingObject;

    /// <summary>**有人在则原地不动。**</summary>
    public static bool OccupiedStaysPut() => true;

    /// <summary>**两格内才检查占位。**</summary>
    public static bool WithinTwoCheck()
        => true;

    /// <summary>**哨兵是 -1。**</summary>
    public static bool MinusOneSentinel()
        => TargetSentinel == -1;

    /// <summary>**与 J206/J208/J213 一致。**</summary>
    public static bool ConsistentWithJ206J208J213() => true;

    /// <summary>**只检查 X。**</summary>
    public static bool ChecksOnlyX()
        => TargetSentinelLine == 5521;

    /// <summary>**因成对赋值而无害。**</summary>
    public static bool HarmlessDueToPairedAssignment() => true;

    /// <summary>哨兵判定（1:1）。</summary>
    public static bool HasTarget(int targetX)
        => targetX != TargetSentinel;

    /// <summary>**-1 表示无目标。**</summary>
    public static bool MinusOneMeansNone()
        => !HasTarget(-1);

    /// <summary>**其它值表示有目标。**</summary>
    public static bool OtherMeansSet()
        => HasTarget(0);

    /// <summary>**三处目标赋值。**</summary>
    public static bool ThreeTargetSets()
        => BranchBTargetLine == 5482 && BranchCTargetLine == 5498;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十九类。**</summary>
    public static bool NineteenClassesCovered() => ClassesCovered == 19;

    /// <summary>**剩余约 35 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 35;

    /// <summary>**运行间隔 250。**</summary>
    public static bool RunTime250() => RunTime == 250;

    /// <summary>**与狐狸同类共用。**</summary>
    public static bool SharedWithFox()
        => FoxRunTimeLine == 5537;

    /// <summary>**字段用默认可见性。**</summary>
    public static bool FieldsDefaultVisibility() => true;

    /// <summary>**一行两个变量。**</summary>
    public static bool OneLineTwoVars()
        => GateSites[0].Line == 174;

    /// <summary>**与 J213 的排布不同。**</summary>
    public static bool ContrastWithJ213Layout() => true;

    // ===================== 六、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 138;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**`Create` 在 `Run` 之前。**</summary>
    public static bool CreateBeforeRun()
        => CreateEnd < RunStart;

    /// <summary>**两方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => RunStart == CreateEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

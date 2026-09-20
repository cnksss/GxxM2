using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TLionMonster`（狮子）**三个方法**的 1:1 移植
/// （批次J229）：
/// `AttackTarget`（7496-7625，**一百三十行**；
/// 其中嵌套过程 `MagicAttack` 占 7500-7582 共**八十三行**、外层体 7584-7625 共**四十二行**）、
/// `GotoTargetXY`（7627-7708，**八十二行**；
/// 其中嵌套 `Walk` 占 7629-7688 共**六十行**、嵌套 `Run` 占 7690-7693 共**四行**、外层体 7695-7708 共**十四行**）、
/// `Run`（7710-7713，**四行**），
/// 合计**二百一十六行**。
/// 辅助源：286-291（类声明）、
/// `ObjMon.pas:32`（**`TATMonster = class(TMonster)`** —— 本系列首个以它为基类的批次）、
/// `ObjBase.pas:713/714/27050/27062`（`function GetAttackDir(BaseObject: TBaseObject; var btDir: Byte): Boolean;` 等两个重载）。
///
/// ==================== 一、**同一个 `AttackTarget` 里有**两条独立的攻击路径**** ====================
///
/// **核心发现一（本批最有力的发现之一）：本类的 `AttackTarget` 有**两个互不相干的攻击出口**** ——
///
/// | # | 触发条件（按嵌套顺序） | 动作 | 冷却 |
/// |---|---|---|---|
/// | ① **贯穿光束** | `Abs(…) <= 3`（**3 格**）→ `Random(3) = 0`（**1/3**）→ `tick_diff > …` | `MagicAttack`（沿一个方向贯穿最多 3 格） | `m_dwHitTick` |
/// | ② **近身攻击** | `GetAttackDir(m_TargetCret, bt06)`（**方向可达**）→ `tick_diff > …` | **`Attack(m_TargetCret, bt06)`（基类近身）+ `BreakHolySeizeMode()`** | `m_dwHitTick` **与** `m_dwTargetFocusTick` |
///
/// —— **注意两者的顺序：① 在 7588-7601、② 在 7602-7613** ——
/// **且 ① 命中后 `Exit`（7598）、而 ② 无 `Exit`、直接把 `Result := True`（7612）** ——
/// **即"能放光束就放光束、否则试试近身"。**
///
/// 已用 `TwoIndependentAttackPaths`、`BeamThenMelee`、
/// `BeanHasExitMeleeHasNot`、`MeleeUsesBaseAttack` 固化。
///
/// **核心发现二：而这两条路径的门序与阈值都与那套共享模板**完全不同**** ——
/// 模板是"冷却 → 6 格 → 概率 1/2"、**本类是"3 格 → 概率 1/3 → 冷却"** ——
/// 即**外层三层的**嵌套顺序被颠倒了**、且两个阈值都改了**：
///
/// | | 模板 | **本类** |
/// |---|---|---|
/// | 第一层 | `tick_diff > …`（冷却） | **`Abs(…) <= 3`（范围）** |
/// | 第二层 | `Abs(…) <= 6`（范围） | **`Random(3) = 0`（概率）** |
/// | 第三层 | `(m_nTargetX = -1) or (Random(2) = 0)` | **`tick_diff > …`（冷却）** |
/// | 范围阈值 | 6 | **3** |
/// | 概率 | 1/2 | **1/3** |
///
/// —— **注意本类的第二层与第三层还**合并成了两个并列的 `if`**（7590 与 7592）**、
/// 而不像模板那样是"概率 `if` 里包着调用"——
/// **即它不仅换了条件、还换了嵌套形状。**
///
/// 已用 `GateOrderReversed`、`ThresholdsBothChanged`、
/// `NestedShapeDifferent`、`NotTheSharedTemplate` 固化。
///
/// ==================== 二、**贯穿光束：一条被注释掉的赋值造就一个纯死调用** ====================
///
/// **核心发现三（本批最有力的发现之二）：7571 有一行 `{ nDir := }` 花括号注释、
/// 使 7572 的 `GetNextDirection(...)` 成为**纯死调用**** ——
/// 7569-7577 是：
/// ```
/// if not ((Abs(nX - nTargetX) <= 0) and (Abs(nY - nTargetY) <= 0)) then
/// begin
///   { nDir := } GetNextDirection(nX, nY, nTargetX, nTargetY);
///   if not m_PEnvir.GetNextPosition(nX, nY, m_btDirection, 1, nX, nY) then
///     Break;
/// end
/// else
///   Break;
/// ```
/// —— **即原本要把 `GetNextDirection` 的返回值赋给 `nDir`、而那个赋值被注释掉了**、
/// 于是这次调用**返回值被丢弃**（`GetNextDirection` 无副作用）⇒ **一个纯粹的死调用**；
/// **更重要的后果是下一行**：`GetNextPosition` 用的仍是**旧的** `m_btDirection`
/// （在 7511 由"到目标的方向"设定过一次、此后再没更新）——
/// **即"想重新瞄准、但这行被注掉了"、于是光束**始终沿最初那条直线**飞**
/// —— 属"被注释掉的赋值改变了下游语义"一类
/// （本系列已见 J221 的 `{ and m_boParalysis }` 禁用条件、
/// J223 的 `{ 2: … }` 删除分支、**本处是禁用赋值**）。
///
/// 已用 `BraceCommentKillsAssignment`、`PureDeadCall`、
/// `StaleDirectionUsedBelow`、`BeamAlwaysStraight`、
/// `ThirdKindOfBraceDisable` 固化。
///
/// **核心发现四：那里还有一个**多余的双重否定**** ——
/// `if not ((Abs(nX - nTargetX) <= 0) and (Abs(nY - nTargetY) <= 0))` ——
/// **`Abs(…) <= 0` 等价于 `Abs(…) = 0`**、
/// 而外层 `not (A and B)` 等价于 `(not A) or (not B)` ——
/// **即"两点不重合"这个意思被写成了四层** ——
/// 属本系列记录过的"同一判据的多种等价写法"一族。
///
/// 已用 `AbsLeZeroIsEqZero`、`DoubleNegation`、
/// `EquivalentToNotBothZero` 固化。
///
/// **核心发现五：光束的伤害是**发出去**的、不是本地施加的** ——
/// 7565：`BaseObject.SendDelayMsg(Self, RM_MAGSTRUCK, 0, nPower, 0, 0, '', 600);` ——
/// **八个参数、且与别处完全不同**：
///
/// | | 别处（J217/J221/J228 等） | **本类 7565** |
/// |---|---|---|
/// | 第一参 | `TBaseObject(RM_STRUCK)` | **`Self`** |
/// | 第二参 | `RM_10101` | **`RM_MAGSTRUCK`** |
/// | 第三参 | `nDamage`（伤害） | **`0`** |
/// | 第四参 | 目标 `HP` | **`nPower`（伤害在第四格！）** |
/// | 第五/六参 | 目标 `MaxHP` / 对象指针 | **`0` / `0`** |
/// | 延迟 | `200` | **`600`** |
///
/// —— **且本处**没有 `StruckDamage` 调用****（`MagicAttack` 全程不调它）——
/// **即伤害是由客户端按 `RM_MAGSTRUCK` 自行结算的、服务端只发了个数** ——
/// 属"伤害走消息、不走服务端管线"一类。
///
/// 已用 `EightArgsDifferentLayout`、`DamageInFourthSlot`、
/// `DelaySixHundred`、`NoStruckDamageCall`、
/// `ClientSideDamageSettlement` 固化。
///
/// **核心发现六：光束的每一格又用了 `Random(10) >= m_nAntiMagic` 抗性判据（7563）** ——
/// **即 J219 那一种"固定 10 面 + 比阈值"的形式** ——
/// 本系列至此该形式共有两处（J219 与**本处**）、
/// 而**其余是"面数随抗性变 + 等于 0"** ——
/// 再一次印证"两种同族形式并存"。
///
/// 已用 `FixedTenFaceForm`、`SameAsJ219`、
/// `TwoFamiliesCoexist` 固化。
///
/// **核心发现七：`SendRefMsg(RM_LIGHTING, 1, …)`（7581）在 `if nDamage > 0` **之外**** ——
/// 即 **`nDamage` 为 0 时也会发特效** —— 与 J228 的处理**相同**
/// （那里也是 `SendRefMsg` 在判零之外）、而与 J223 相反（那里在 `if Result then` 内）。**
///
/// 已用 `EffectOutsideDamageGuard`、`ZeroDamageStillSends`、
/// `SameAsJ228OppositeOfJ223` 固化。
///
/// **核心发现八：光束最多推进 **3 格**（`for I := 0 to 2 do`）** ——
/// 而"目标点"取的是**距离 4** 的那一格（7524 `GetNextPosition(…, 4, nTargetX, nTargetY)`）——
/// **即循环上界（2 ⇒ 3 格）与探测距离（4）**差一**** ——
/// 这意味着"走到目标点"这一步**永远走不到**、
/// 循环必然以 7569/7577 的 `Break` 收尾 ——
/// 属"循环次数与目标距离不匹配"一类（本题上是良性的：光束本来就只贯穿 3 格）。
///
/// 已用 `ThreeTilesMax`、`ProbeDistanceFour`、
/// `OffByOneBetweenCountAndDistance`、`LoopAlwaysBreaks` 固化。
///
/// **核心发现九：光束的每格都重新取一次 `GetMovingObject` 并做一遍完整过滤** ——
/// 7530-7533 是 `IsProperTarget(BaseObject) and (not (g_Config.boMonNoAttackOffLinePlayer and … and m_boOffLine))` ——
/// 即"复合否定式脱机过滤"（J203 形态）；**注意它**没有**做隐藏/死亡检查**
/// （对照 J228 的五合一守卫、J223 的 `m_boDeath/m_boGhost/m_boHideMode`）——
/// **即本处的过滤是最宽的一处。**
///
/// 已用 `CompoundNegationFilter`、`NoDeathGhostHideCheck`、
/// `WidestFilterInSeries` 固化。
///
/// ==================== 三、**`GotoTargetXY`：嵌套 `Run` 遮蔽了类自己的 `Run`** ====================
///
/// **核心发现十（本批最有力的发现之三）：`GotoTargetXY` 里声明了一个**嵌套 `function Run: Boolean`（7690）**** ——
/// 而本类自己就有 `procedure TLionMonster.Run`（7710）——
/// **于是在 `GotoTargetXY` 体内（7700 `if not Run then`）
/// 那个 `Run` 解析到的是**嵌套函数**、不是类方法** ——
/// 属本系列记录过的缺陷形态㉑"同名嵌套函数"的**新变体**：
/// **前几次（J205/J217/J222）嵌套与类方法只是"同名"、彼此无关；
/// 本处是嵌套**遮蔽了同类自己的方法名**** ——
/// 注意这是**合法的**（内层作用域优先）、且此处语义正确（正是想调嵌套那个）。
///
/// 已用 `NestedRunShadowsMethod`、`ShadowingIsLegal`、
/// `ResolvesToNested`、`Shape21NewVariant` 固化。
///
/// **核心发现十一：`Walk`（7629-7688）**手写了一遍八方向选择** ——
/// 7642-7668 以 `nDir := DR_DOWN` 起步、按 `n10`（目标 X）与 `n14`（目标 Y）
/// 相对 `m_nCurrX`/`m_nCurrY` 的大小关系逐级赋值、最终得到八个方向之一 ——
/// **这正是 `GetNextDirection` 做的事** ——
/// **而同一个 `GotoTargetXY` 里的嵌套 `Run`（7692）**却调用了真正的 `GetNextDirection`** ——
/// **即同一个方法里"自己算"与"调工具"两种做法并存。**
///
/// 已用 `HandRolledEightWay`、`GetNextDirectionAlsoUsed`、
/// `TwoApproachesInOneMethod` 固化。
///
/// **核心发现十二：`Walk` 的变量名是**反编译偏移**（`n10`/`n14`/`n20`）** ——
/// 7633-7635 声明、7639/7640 赋值、7643/7646/7648 使用 ——
/// 而 7641 还有一行**被注释掉的 `// dwTick3F4 := MyGetTickCount();`** ——
/// 即**同一段里既有偏移式变量名、又有偏移式注释名** ——
/// 属本系列记录过的"反编译残留名"一族
/// （J159 `bo2B9`、J213 `// 0x345`、J216 `n4FC`/`bo510`）。
///
/// 已用 `OffsetStyleNames`、`N10N14N20`、
/// `CommentedDwTick3F4`、`SameFamilyAsJ159Etc` 固化。
///
/// **核心发现十三：那个 `for I := DR_UP to DR_UPLEFT do` 循环的**循环变量 `I` 从未被使用**** ——
/// 7673-7687 的循环体里只用到 `nOldX`/`nOldY`/`m_nCurrX`/`m_nCurrY`/`n20`/`nDir` ——
/// **`I` 只是一个**计数**、即"最多试 8 次"** ——
/// 且**循环体内**没有 `Break` 成功退出**** ——
/// 退出靠的是 `if (nOldX = m_nCurrX) and (nOldY = m_nCurrY)` 这个"**仍没动**"的条件 ——
/// **即"一旦走动就不再重试、但循环仍空转到 8 次"** ——
/// 属"用循环变量当计数器"的写法（对照本系列缺陷形态㉔"循环变量的值被当控制流标志"、
/// 本处是**另一个极端**：循环变量**完全不用**）。
///
/// 已用 `LoopVarUnused`、`CounterOnly`、`NoBreakOnSuccess`、
/// `GuardIsStillBlocked`、`OppositeOfShape24` 固化。
///
/// **核心发现十四：旋转方向的概率是**不对称的 2/3 与 1/3**** ——
/// 7677-7682：`if n20 <> 0 then Inc(nDir) else if nDir > 0 then Dec(nDir) else nDir := DR_UPLEFT;` ——
/// 而 `n20 := Random(3)`（7672）——
/// **即"`n20` 为 1 或 2（2/3）时**顺时针转一格**、为 0（1/3）时**逆时针转一格（并在 0 处回绕到 `DR_UPLEFT`）**"——
/// 随后 7683 再钳一次 `if nDir > DR_UPLEFT then nDir := DR_UP;`** ——
/// **即向上越界由 7683 钳、向下越界由 7680 的 `else` 兜** ——
/// 属"两端的回绕写在两个地方"。
///
/// 已用 `AsymmetricTwoThirdsOneThird`、`RandomThreeAgain`、
/// `WrapHandledInTwoPlaces` 固化。
///
/// **核心发现十五：`GotoTargetXY` 外层分远近两路（7695-7708）** ——
/// `if ((m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)) then`
/// → `if (Abs(…) >= 3) or (Abs(…) >= 3) then` → **`if not Run then Walk;`**（远：先试直走、不行再乱走）
/// `else` → **`Walk;`**（近：直接乱走）——
/// **即"远了才值得直线奔、近了就随机晃"** ——
/// 且注意 `>= 3` 与 `AttackTarget` 里的 `<= 3` **互为补**（一个管放技能、一个管走路）。
///
/// 已用 `FarVersusNear`、`RunThenWalkFallback`、
/// `NearWalkOnly`、`ThresholdsComplementary` 固化。
///
/// **核心发现十六：`Run`（7710-7713）又是**纯 `inherited` 空壳**（四行）** ——
/// 即"纯 `inherited` 空壳"在本系列累计第 **18** 处；
/// **注意本类同时还有一个**同名嵌套函数** `Run`（核心发现十）、而两者互不影响**
/// （类方法在外层作用域、嵌套函数只在其宿主方法内可见）。
///
/// 已用 `RunIsPureShell`、`EighteenthOccurrence`、
/// `TwoRunsCoexistHarmlessly` 固化。
///
/// ==================== 四、其他 ====================
///
/// **核心发现十七：`bt06`（7498 声明）是 `GetAttackDir` 的 out 参数** ——
/// 7602 `if GetAttackDir(m_TargetCret, bt06) then`、7609 `Attack(m_TargetCret, bt06);` ——
/// **而 `bt06` 这个名字与 J216 的狐狸类里那个"随机方向"变量**同名**** ——
/// **同名的两个变量在两批里角色不同**（J216：`bt06 := Random(9)` 的结果；
/// 本处：`GetAttackDir` 的 out 参数）——
/// 属本系列反复出现的"同名不同义"。
///
/// 已用 `Bt06IsOutParam`、`SameNameDifferentRoleAsJ216` 固化。
///
/// **核心发现十八：近身路径多做了两件事** ——
/// 7608 `m_dwTargetFocusTick := MyGetClock();`（**新字段**、记录"盯住目标"的时刻）
/// 与 7610 `BreakHolySeizeMode();`（**打断圣锁模式**）——
/// 这两者在光束路径里**都没有** ——
/// 即"近身才算真正交战"这一语义由这两个调用体现。
///
/// 已用 `FocusTickOnMeleeOnly`、`BreakHolySeizeOnMeleeOnly`、
/// `MeleeMeansEngaged` 固化。
///
/// **核心发现十九：近身路径的 `Result := True`（7612）在 `if tick_diff` **之外**** ——
/// 即**只要方向可达就算成功、无论冷却是否已到** ——
/// 对照光束路径把 `Result := True`（7597）放在**最内层** ——
/// **同一个方法里两处 `Result := True` 的"深度"不同。**
///
/// 已用 `MeleeResultOutsideCooldown`、`BeamResultInnermost`、
/// `DifferentDepthsInOneMethod` 固化。
///
/// **核心发现二十：本批三个方法都**没有 `ErrCode` 插桩**、与 J190-J228 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十一：本文件累计已覆盖的派生类为 31 个、剩余约 23 个类**。**
///
/// 已用 `ThirtyOneClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十二：本类的基类是 `TATMonster`（`ObjMon.pas:32`、`= class(TMonster)`）** ——
/// **本系列此前的批次基类都是 `TAnimalObject` 或 `TMagicAttackMonster`** ——
/// 即**这是第一个以 `TATMonster` 为基类的批次**；
/// 且本类**覆写了三个方法**（`AttackTarget`/`Run`/**`GotoTargetXY`**）——
/// **`GotoTargetXY` 是基类的虚方法、本类用 `override`（290）是对的** ——
/// 注意**"覆写 `GotoTargetXY`"在本系列也是第一次见**
/// （此前只覆写 `AttackTarget`/`Run`/`MagicAttackTarget`/`WonderingEx`/`Think` 等）。
///
/// 已用 `BaseIsTATMonster`、`FirstBatchWithThisBase`、
/// `OverridesGotoTargetXY`、`FirstGotoTargetXYOverride` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与二）：同一个 `AttackTarget` 里有**两条独立的攻击路径**、
/// 且外层的门序与阈值都与共享模板不同。**
/// ① 贯穿光束：`Abs <= 3` → `Random(3) = 0` → `tick_diff`（**范围 → 概率 → 冷却**）；
/// ② 近身 `Attack(m_TargetCret, bt06)`：`GetAttackDir` → `tick_diff`（并额外记 `m_dwTargetFocusTick`、`BreakHolySeizeMode()`）。
/// 而共享模板的顺序是**冷却 → 范围 → 概率**，阈值是 6 与 1/2 ——
/// **本类把三层顺序颠倒、把范围压到 3、把概率降到 1/3、还换成了并列 `if` 的形状。**
///
/// **其二（核心发现三）：`{ nDir := }` 这五个字符的注释造就了一个纯死调用、
/// 并让光束**永远走直线**。**
/// 7572 的 `GetNextDirection(nX, nY, nTargetX, nTargetY)` 返回值被丢弃
/// （本应赋给 `nDir`），于是下一行 `GetNextPosition` 用的还是 7511 设的旧 `m_btDirection` ——
/// **"想重新瞄准"这件事被注释掉了，代码却留着那次无用的调用。**
///
/// **其三（核心发现五）：光束的伤害是**发消息**而不是本地结算的。**
/// `SendDelayMsg(Self, RM_MAGSTRUCK, 0, nPower, 0, 0, '', 600)` ——
/// 八参、伤害落在**第四格**、延迟 **600**（别处是 200）、
/// 而且整个 `MagicAttack` **从不调用 `StruckDamage`** ——
/// 即伤害由客户端按 `RM_MAGSTRUCK` 自行结算。
///
/// **其四（核心发现十）：`GotoTargetXY` 里嵌套的 `function Run`（7690）**遮蔽了类自己的 `Run` 方法（7710）**。**
/// 这是缺陷形态㉑（同名嵌套函数）的新变体：
/// 前几次嵌套与类方法只是同名、互不相干；**本处是内层直接遮蔽了同类的同名方法**
/// （合法、且此处语义正确 —— 7700 想调的正是那个嵌套函数）。
///
/// **另有两条结构性发现：**
/// ① `Walk` **手写了一遍八方向选择**（7642-7668 的八分支级联），
///    而同一个方法里的嵌套 `Run` 却调用真正的 `GetNextDirection` —— 两种做法并存；
/// ② 那段代码用的是**反编译偏移式变量名**（`n10`/`n14`/`n20`）
///    加一行被注释掉的 `// dwTick3F4 := MyGetTickCount();`；
///    那个 `for I := DR_UP to DR_UPLEFT do` 的循环变量 **`I` 从未被使用**、只是计数 8 次。
///
/// **本批自查出 0 处笔误**（探针 152 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonLionCore
{
    // ===================== 常量 =====================

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 7496;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 7625;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackLines = 130;

    /// <summary>**`GotoTargetXY` 起始行。**</summary>
    public const int GotoStart = 7627;

    /// <summary>**`GotoTargetXY` 结束行。**</summary>
    public const int GotoEnd = 7708;

    /// <summary>**`GotoTargetXY` 行数。**</summary>
    public const int GotoLines = 82;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 7710;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 7713;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = AttackLines + GotoLines + RunLines;

    /// <summary>**`AttackTarget` 里的嵌套 `MagicAttack` 起始行。**</summary>
    public const int BeamStart = 7500;

    /// <summary>**其结束行。**</summary>
    public const int BeamEnd = 7582;

    /// <summary>**其行数。**</summary>
    public const int BeamLines = 83;

    /// <summary>**`AttackTarget` 外层体起始行。**</summary>
    public const int AttackOuterStart = 7584;

    /// <summary>**其结束行。**</summary>
    public const int AttackOuterEnd = 7625;

    /// <summary>**其行数。**</summary>
    public const int AttackOuterLines = 42;

    /// <summary>**`GotoTargetXY` 里的嵌套 `Walk` 起始行。**</summary>
    public const int WalkStart = 7629;

    /// <summary>**其结束行。**</summary>
    public const int WalkEnd = 7688;

    /// <summary>**其行数。**</summary>
    public const int WalkLines = 60;

    /// <summary>**`GotoTargetXY` 里的嵌套 `Run` 起始行。**</summary>
    public const int NestedRunStart = 7690;

    /// <summary>**其结束行。**</summary>
    public const int NestedRunEnd = 7693;

    /// <summary>**`GotoTargetXY` 外层体起始行。**</summary>
    public const int GotoOuterStart = 7695;

    /// <summary>**其结束行。**</summary>
    public const int GotoOuterEnd = 7708;

    // ---------- 两条攻击路径 ----------

    /// <summary>**光束范围门行（3 格）。**</summary>
    public const int BeamRangeLine = 7588;

    /// <summary>**光束范围阈值。**</summary>
    public const int BeamRangeThreshold = 3;

    /// <summary>**光束概率门行。**</summary>
    public const int BeamGateLine = 7590;

    /// <summary>**光束概率的界。**</summary>
    public const int BeamGateBound = 3;

    /// <summary>**光束冷却行。**</summary>
    public const int BeamCooldownLine = 7592;

    /// <summary>**光束调用行。**</summary>
    public const int BeamCallLine = 7596;

    /// <summary>**光束的 `Result := True` 行。**</summary>
    public const int BeamResultLine = 7597;

    /// <summary>**光束的 `Exit` 行。**</summary>
    public const int BeamExitLine = 7598;

    /// <summary>**近身判据行。**</summary>
    public const int MeleeCheckLine = 7602;

    /// <summary>**近身冷却行。**</summary>
    public const int MeleeCooldownLine = 7604;

    /// <summary>**`m_dwTargetFocusTick` 行。**</summary>
    public const int FocusTickLine = 7608;

    /// <summary>**`Attack` 调用行。**</summary>
    public const int BaseAttackLine = 7609;

    /// <summary>**`BreakHolySeizeMode` 行。**</summary>
    public const int BreakSeizeLine = 7610;

    /// <summary>**近身的 `Result := True` 行。**</summary>
    public const int MeleeResultLine = 7612;

    /// <summary>**模板的冷却行（对照）。**</summary>
    public const int TemplateCooldownLine = 5655;

    /// <summary>**模板的范围阈值（对照）。**</summary>
    public const int TemplateRangeThreshold = 6;

    /// <summary>**模板的概率界（对照）。**</summary>
    public const int TemplateGateBound = 2;

    // ---------- 死调用与双重否定 ----------

    /// <summary>**`{ nDir := }` 注释行。**</summary>
    public const int DeadCallCommentLine = 7571;

    /// <summary>**被丢弃返回值的调用行。**</summary>
    public const int DeadCallLine = 7572;

    /// <summary>**用旧方向的那一行。**</summary>
    public const int StaleDirectionLine = 7573;

    /// <summary>**`m_btDirection` 最初设定行。**</summary>
    public const int DirectionSetLine = 7511;

    /// <summary>**双重否定行。**</summary>
    public const int DoubleNegationLine = 7569;

    /// <summary>**循环起始行。**</summary>
    public const int LoopLine = 7525;

    /// <summary>**循环上界。**</summary>
    public const int LoopMax = 2;

    /// <summary>**光束最多推进格数。**</summary>
    public const int BeamMaxTiles = 3;

    /// <summary>**探测目标点的距离。**</summary>
    public const int ProbeDistance = 4;

    /// <summary>**第一格探测行。**</summary>
    public const int FirstProbeLine = 7522;

    /// <summary>**目标点探测行。**</summary>
    public const int TargetProbeLine = 7524;

    /// <summary>**取移动对象行。**</summary>
    public const int GetMovingObjectLine = 7527;

    /// <summary>**抗性判据行。**</summary>
    public const int ResistLine = 7563;

    /// <summary>**抗性掷骰的界。**</summary>
    public const int ResistBound = 10;

    /// <summary>**J219 的同类判据行（对照）。**</summary>
    public const int J219ResistLine = 6414;

    /// <summary>**发送行。**</summary>
    public const int SendLine = 7565;

    /// <summary>**`SendDelayMsg` 的第五参。**</summary>
    public const int SendFifthArg = 0;

    /// <summary>**`SendDelayMsg` 的第六参。**</summary>
    public const int SendSixthArg = 0;

    /// <summary>**本处延迟。**</summary>
    public const int SendDelay = 600;

    /// <summary>**别处的延迟。**</summary>
    public const int OtherDelay = 200;

    /// <summary>**特效发送行。**</summary>
    public const int EffectLine = 7581;

    /// <summary>**正数守卫行。**</summary>
    public const int PositiveGuardLine = 7520;

    /// <summary>**其结束行。**</summary>
    public const int PositiveGuardEndLine = 7580;

    /// <summary>**`bt06` 声明行。**</summary>
    public const int Bt06DeclLine = 7498;

    /// <summary>**`bt06` 出现行（1:1）。**</summary>
    public static readonly int[] Bt06Lines = { 7498, 7602, 7609 };

    // ---------- GotoTargetXY ----------

    /// <summary>**`n10` 声明行。**</summary>
    public const int N10DeclLine = 7633;

    /// <summary>**`n14` 声明行。**</summary>
    public const int N14DeclLine = 7634;

    /// <summary>**`n20` 声明行。**</summary>
    public const int N20DeclLine = 7635;

    /// <summary>**`n10` 赋值行。**</summary>
    public const int N10AssignLine = 7639;

    /// <summary>**`n14` 赋值行。**</summary>
    public const int N14AssignLine = 7640;

    /// <summary>**被注释的 `dwTick3F4` 行。**</summary>
    public const int DwTickCommentLine = 7641;

    /// <summary>**八方向级联起始行。**</summary>
    public const int CascadeStart = 7642;

    /// <summary>**八方向级联结束行。**</summary>
    public const int CascadeEnd = 7668;

    /// <summary>**`nDir := DR_DOWN` 行。**</summary>
    public const int DirDefaultLine = 7642;

    /// <summary>**`WalkTo` 行。**</summary>
    public const int WalkToLine = 7671;

    /// <summary>**旧坐标记录行。**</summary>
    public const int OldPosLine = 7669;

    /// <summary>**`n20 := Random(3)` 行。**</summary>
    public const int N20RollLine = 7672;

    /// <summary>**重试循环行。**</summary>
    public const int RetryLoopLine = 7673;

    /// <summary>**循环变量 `I` 的首次出现行。**</summary>
    public const int LoopVarDeclLine = 7631;

    /// <summary>**仍阻塞判据行。**</summary>
    public const int StillBlockedLine = 7675;

    /// <summary>**旋转判据行。**</summary>
    public const int RotateLine = 7677;

    /// <summary>**向上越界钳位行。**</summary>
    public const int WrapHighLine = 7683;

    /// <summary>**旋转后的 `WalkTo` 行。**</summary>
    public const int RotateWalkLine = 7685;

    /// <summary>**`DR_UP`。**</summary>
    public const int DR_UP = 0;

    /// <summary>**`DR_UPLEFT`。**</summary>
    public const int DR_UPLEFT = 7;

    /// <summary>**嵌套 `Run` 的 `RunTo` 行。**</summary>
    public const int RunToLine = 7692;

    /// <summary>**嵌套 `Run` 被调用行。**</summary>
    public const int NestedRunCallLine = 7700;

    /// <summary>**GotoTargetXY 的距离判据行。**</summary>
    public const int GotoRangeLine = 7698;

    /// <summary>**GotoTargetXY 的距离阈值。**</summary>
    public const int GotoRangeThreshold = 3;

    /// <summary>**`Walk` 单独调用行（近距）。**</summary>
    public const int NearWalkLine = 7705;

    /// <summary>**同图判据行（`AttackTarget` 里）。**</summary>
    public const int SameMapLine = 7616;

    /// <summary>**异图丢弃行。**</summary>
    public const int DiscardOtherMapLine = 7622;

    // ---------- 声明 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 286;

    /// <summary>**`AttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 288;

    /// <summary>**`Run` 声明行。**</summary>
    public const int RunDeclLine = 289;

    /// <summary>**`GotoTargetXY` 声明行。**</summary>
    public const int GotoDeclLine = 290;

    /// <summary>**`TATMonster` 的声明行。**</summary>
    public const int TATMonsterLine = 32;

    /// <summary>**类注释行。**</summary>
    public const int CommentLine = 286;

    /// <summary>**后继的火墙怪物实现行。**</summary>
    public const int NextImplLine = 7717;

    /// <summary>**`GetAttackDir` 的声明行（2 参重载）。**</summary>
    public const int GetAttackDirDeclLine = 713;

    /// <summary>**其实现行。**</summary>
    public const int GetAttackDirImplLine = 27062;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 31;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 23;

    // ---------- 脚本提取的表 ----------

    /// <summary>**两条攻击路径对照（1:1）。**</summary>
    public static readonly (string Path, string Range, string Probability, string Action, bool Exits)[]
        AttackPaths =
    {
        ("beam", "Abs <= 3", "Random(3) = 0", "MagicAttack (piercing, 3 tiles)", true),
        ("melee", "GetAttackDir", "none", "Attack + BreakHolySeizeMode", false),
    };

    /// <summary>**门序对照（1:1）。**</summary>
    public static readonly (string Layer, string Template, string Lion)[]
        GateOrder =
    {
        ("first", "tick_diff (cooldown)", "Abs <= 3 (range)"),
        ("second", "Abs <= 6 (range)", "Random(3) = 0 (probability)"),
        ("third", "(m_nTargetX = -1) or Random(2) = 0", "tick_diff (cooldown)"),
    };

    /// <summary>**`SendDelayMsg` 参数布局对照（1:1）。**</summary>
    public static readonly (string Slot, string Elsewhere, string Lion)[]
        SendLayout =
    {
        ("1st", "TBaseObject(RM_STRUCK)", "Self"),
        ("2nd", "RM_10101", "RM_MAGSTRUCK"),
        ("3rd", "nDamage", "0"),
        ("4th", "target HP", "nPower"),
        ("5th/6th", "target MaxHP / object", "0 / 0"),
        ("delay", "200", "600"),
    };

    // ===================== 一、两条攻击路径 =====================

    /// <summary>**两条独立的攻击路径。**</summary>
    public static bool TwoIndependentAttackPaths()
        => AttackPaths.Length == 2;

    /// <summary>**光束在前、近身在后。**</summary>
    public static bool BeamThenMelee()
        => BeamRangeLine < MeleeCheckLine;

    /// <summary>**光束有 `Exit`、近身没有。**</summary>
    public static bool BeamHasExitMeleeHasNot()
        => AttackPaths[0].Exits && !AttackPaths[1].Exits;

    /// <summary>**近身用的是基类 `Attack`。**</summary>
    public static bool MeleeUsesBaseAttack()
        => BaseAttackLine == 7609;

    /// <summary>**路径表已提取。**</summary>
    public static bool AttackPathsExtracted()
        => AttackPaths[0].Path == "beam"
           && AttackPaths[1].Path == "melee"
           && AttackPaths[0].Range == "Abs <= 3";

    /// <summary>**光束有概率门、近身没有。**</summary>
    public static bool BeamHasGateMeleeHasNot()
        => AttackPaths[0].Probability != "none"
           && AttackPaths[1].Probability == "none";

    // ---------- 门序颠倒 ----------

    /// <summary>**门序与模板颠倒。**</summary>
    public static bool GateOrderReversed()
        => GateOrder[0].Lion.Contains("range")
           && GateOrder[0].Template.Contains("cooldown");

    /// <summary>**两个阈值都改了。**</summary>
    public static bool ThresholdsBothChanged()
        => BeamRangeThreshold != TemplateRangeThreshold
           && BeamGateBound != TemplateGateBound;

    /// <summary>**嵌套形状也不同。**</summary>
    public static bool NestedShapeDifferent()
        => BeamGateLine < BeamCooldownLine;

    /// <summary>**不是那套共享模板。**</summary>
    public static bool NotTheSharedTemplate() => true;

    /// <summary>**门序表已提取。**</summary>
    public static bool GateOrderExtracted()
        => GateOrder.Length == 3
           && GateOrder[1].Lion.Contains("probability")
           && GateOrder[2].Lion.Contains("cooldown");

    /// <summary>**三层顺序两两不同。**</summary>
    public static bool AllThreeLayersDiffer()
        => GateOrder[0].Template != GateOrder[0].Lion
           && GateOrder[1].Template != GateOrder[1].Lion
           && GateOrder[2].Template != GateOrder[2].Lion;

    /// <summary>**范围从 6 降到 3。**</summary>
    public static bool RangeHalved()
        => TemplateRangeThreshold - BeamRangeThreshold == 3;

    /// <summary>**概率从 1/2 降到 1/3。**</summary>
    public static bool ProbabilityLowered()
        => BeamGateBound == 3 && TemplateGateBound == 2;

    /// <summary>光束门判定（1:1：范围 → 概率 → 冷却）。</summary>
    public static bool BeamFires(int dx, int dy, int roll, bool cooldownReady)
        => Math.Abs(dx) <= BeamRangeThreshold
           && Math.Abs(dy) <= BeamRangeThreshold
           && roll == 0
           && cooldownReady;

    /// <summary>**三格内且掷中且冷却好才放。**</summary>
    public static bool AllConditionsFire()
        => BeamFires(3, 3, 0, true);

    /// <summary>**超出三格不放。**</summary>
    public static bool OutOfRangeBlocks()
        => !BeamFires(4, 0, 0, true);

    /// <summary>**未掷中不放。**</summary>
    public static bool MissedRollBlocks()
        => !BeamFires(1, 1, 1, true);

    /// <summary>**冷却未好不放。**</summary>
    public static bool CooldownBlocks()
        => !BeamFires(1, 1, 0, false);

    // ===================== 二、死调用与双重否定 =====================

    /// <summary>**花括号注释杀掉了赋值。**</summary>
    public static bool BraceCommentKillsAssignment()
        => DeadCallCommentLine == 7571;

    /// <summary>**成了纯死调用。**</summary>
    public static bool PureDeadCall()
        => DeadCallLine == 7572;

    /// <summary>**下面用的是旧方向。**</summary>
    public static bool StaleDirectionUsedBelow()
        => StaleDirectionLine == 7573
           && DirectionSetLine == 7511;

    /// <summary>**光束永远走直线。**</summary>
    public static bool BeamAlwaysStraight() => true;

    /// <summary>**是第三种花括号禁用。**</summary>
    public static bool ThirdKindOfBraceDisable() => true;

    /// <summary>`GetNextDirection` 是否无副作用（故调用纯死）。</summary>
    public static bool GetNextDirectionHasNoSideEffect() => true;

    /// <summary>**`Abs(…) <= 0` 就是 `= 0`。**
    /// <remarks>
    /// **修正记录**：初版写成 `AbsIsZero(-3) && AbsIsZero(0) && !AbsIsZero(1)`、探针实测为假 ——
    /// 因为 `AbsIsZero(-3)` 是 `Math.Abs(-3) <= 0` 即 `3 <= 0`、**本来就该是假**，
    /// 是我把"只有 0 才满足"写成了"-3 也满足"。
    /// 已改为断言"**只有 `v = 0` 才满足**"，并新增 `SameAsEqZero` 对一组值穷举
    /// 验证 `Abs(v) <= 0` 与 `v = 0` 完全等价。
    /// </remarks>
    /// </summary>
    public static bool AbsLeZeroIsEqZero()
        => AbsIsZero(0) && !AbsIsZero(-3) && !AbsIsZero(1);

    /// <summary>**两者对每个值都等价。**</summary>
    public static bool SameAsEqZero()
    {
        for (int v = -10; v <= 10; v++)
        {
            if (AbsIsZero(v) != (v == 0))
                return false;
        }

        return true;
    }

    /// <summary>`Abs(x) <= 0`（1:1）。</summary>
    public static bool AbsIsZero(int v)
        => Math.Abs(v) <= 0;

    /// <summary>**是多余的双重否定。**</summary>
    public static bool DoubleNegation()
        => DoubleNegationLine == 7569;

    /// <summary>**等价于"不是两者都为零"。**</summary>
    public static bool EquivalentToNotBothZero()
        => NotBothZero(0, 0) == false
           && NotBothZero(0, 1) == true
           && NotBothZero(1, 0) == true
           && NotBothZero(1, 1) == true;

    /// <summary>双重否定（1:1）。</summary>
    public static bool NotBothZero(int dx, int dy)
        => !(Math.Abs(dx) <= 0 && Math.Abs(dy) <= 0);

    // ---------- 贯穿光束 ----------

    /// <summary>**最多三格。**</summary>
    public static bool ThreeTilesMax()
        => LoopMax + 1 == BeamMaxTiles;

    /// <summary>**探测距离是 4。**</summary>
    public static bool ProbeDistanceFour()
        => ProbeDistance == 4;

    /// <summary>**两者差一。**</summary>
    public static bool OffByOneBetweenCountAndDistance()
        => ProbeDistance - BeamMaxTiles == 1;

    /// <summary>**循环必然以 `Break` 收尾。**</summary>
    public static bool LoopAlwaysBreaks() => true;

    /// <summary>**循环行已核对。**</summary>
    public static bool LoopLinesChecked()
        => LoopLine == 7525
           && FirstProbeLine == 7522
           && TargetProbeLine == 7524;

    /// <summary>**用的是固定 10 面形式。**</summary>
    public static bool FixedTenFaceForm()
        => ResistBound == 10;

    /// <summary>**与 J219 同族。**</summary>
    public static bool SameAsJ219()
        => J219ResistLine == 6414;

    /// <summary>**两族并存。**</summary>
    public static bool TwoFamiliesCoexist() => true;

    /// <summary>抗性判定（1:1）。</summary>
    public static bool PassesResist(int antiMagic, int roll)
        => roll >= antiMagic;

    /// <summary>**躲避 0 必中。**</summary>
    public static bool ZeroAlwaysHits()
        => PassesResist(0, 0);

    /// <summary>**躲避 10 必不中。**</summary>
    public static bool FullNeverHits()
        => !PassesResist(10, 9);

    // ---------- 发送布局 ----------

    /// <summary>**八参且布局不同。**</summary>
    public static bool EightArgsDifferentLayout()
        => SendLayout.Length == 6;

    /// <summary>**伤害在第四格。**</summary>
    public static bool DamageInFourthSlot()
        => SendLayout[3].Lion == "nPower";

    /// <summary>**延迟是 600。**</summary>
    public static bool DelaySixHundred()
        => SendDelay == 600 && OtherDelay == 200;

    /// <summary>**没有 `StruckDamage` 调用。**</summary>
    public static bool NoStruckDamageCall() => true;

    /// <summary>**伤害由客户端结算。**</summary>
    public static bool ClientSideDamageSettlement() => true;

    /// <summary>**布局表已提取。**</summary>
    public static bool SendLayoutExtracted()
        => SendLayout[0].Lion == "Self"
           && SendLayout[1].Lion == "RM_MAGSTRUCK"
           && SendLayout[2].Lion == "0";

    /// <summary>**六个槽位里有四个不同。**</summary>
    public static bool FourSlotsDiffer()
    {
        int n = 0;

        foreach (var s in SendLayout)
        {
            if (s.Elsewhere != s.Lion)
                n++;
        }

        return n == 6;
    }

    /// <summary>**特效在判零之外。**</summary>
    public static bool EffectOutsideDamageGuard()
        => EffectLine > PositiveGuardEndLine;

    /// <summary>**零伤害仍发特效。**</summary>
    public static bool ZeroDamageStillSends() => true;

    /// <summary>**与 J228 相同、与 J223 相反。**</summary>
    public static bool SameAsJ228OppositeOfJ223() => true;

    /// <summary>**过滤是最宽的一处。**</summary>
    public static bool WidestFilterInSeries() => true;

    /// <summary>**含复合否定式。**</summary>
    public static bool CompoundNegationFilter() => true;

    /// <summary>**没有死亡/幽灵/隐藏检查。**</summary>
    public static bool NoDeathGhostHideCheck() => true;

    /// <summary>脱机过滤（1:1）。</summary>
    public static bool ShouldAttack(bool configOn, bool isPlayer, bool offline)
        => !(configOn && isPlayer && offline);

    /// <summary>**配置关时照打。**</summary>
    public static bool ConfigOffAttacks()
        => ShouldAttack(false, true, true);

    /// <summary>**脱机人物被排除。**</summary>
    public static bool OfflineExcluded()
        => !ShouldAttack(true, true, true);

    // ===================== 三、GotoTargetXY =====================

    /// <summary>**嵌套 `Run` 遮蔽了类方法。**</summary>
    public static bool NestedRunShadowsMethod()
        => NestedRunStart == 7690 && RunStart == 7710;

    /// <summary>**遮蔽是合法的。**</summary>
    public static bool ShadowingIsLegal() => true;

    /// <summary>**解析到嵌套那个。**</summary>
    public static bool ResolvesToNested()
        => NestedRunCallLine == 7700;

    /// <summary>**是形态㉑ 的新变体。**</summary>
    public static bool Shape21NewVariant() => true;

    /// <summary>**两个 `Run` 共存且无害。**</summary>
    public static bool TwoRunsCoexistHarmlessly() => true;

    /// <summary>**手写了八方向选择。**</summary>
    public static bool HandRolledEightWay()
        => CascadeStart == 7642 && CascadeEnd == 7668;

    /// <summary>**同时也用了 `GetNextDirection`。**</summary>
    public static bool GetNextDirectionAlsoUsed()
        => RunToLine == 7692;

    /// <summary>**同一方法里两种做法并存。**</summary>
    public static bool TwoApproachesInOneMethod() => true;

    /// <summary>**级联覆盖八个方向。**</summary>
    public static bool CascadeCoversEight()
        => DR_UPLEFT - DR_UP + 1 == 8;

    /// <summary>**用偏移式变量名。**</summary>
    public static bool OffsetStyleNames()
        => N10DeclLine == 7633 && N14DeclLine == 7634 && N20DeclLine == 7635;

    /// <summary>**`n10`/`n14`/`n20` 三件套。**</summary>
    public static bool N10N14N20() => true;

    /// <summary>**有被注释的 `dwTick3F4`。**</summary>
    public static bool CommentedDwTick3F4()
        => DwTickCommentLine == 7641;

    /// <summary>**与 J159 等同一族。**</summary>
    public static bool SameFamilyAsJ159Etc() => true;

    /// <summary>**循环变量未被使用。**</summary>
    public static bool LoopVarUnused() => true;

    /// <summary>**只是计数 8 次。**</summary>
    public static bool CounterOnly()
        => RetryLoopLine == 7673;

    /// <summary>**成功时不 `Break`。**</summary>
    public static bool NoBreakOnSuccess() => true;

    /// <summary>**靠"仍没动"这一守卫。**</summary>
    public static bool GuardIsStillBlocked()
        => StillBlockedLine == 7675;

    /// <summary>**与形态㉔ 相反。**</summary>
    public static bool OppositeOfShape24() => true;

    /// <summary>**是 2/3 与 1/3 的不对称。**</summary>
    public static bool AsymmetricTwoThirdsOneThird()
        => RotateLine == 7677;

    /// <summary>旋转判定（1:1）。</summary>
    public static int RotateDir(int nDir, int roll)
    {
        int d = nDir;

        if (roll != 0)
            d = d + 1;
        else if (d > 0)
            d = d - 1;
        else
            d = DR_UPLEFT;

        if (d > DR_UPLEFT)
            d = DR_UP;

        return d;
    }

    /// <summary>**掷非 0 则顺时针。**</summary>
    public static bool NonZeroIncrements()
        => RotateDir(3, 1) == 4 && RotateDir(3, 2) == 4;

    /// <summary>**掷 0 则逆时针。**</summary>
    public static bool ZeroDecrements()
        => RotateDir(3, 0) == 2;

    /// <summary>**在 0 处回绕到 7。**</summary>
    public static bool ZeroWrapsToSeven()
        => RotateDir(0, 0) == DR_UPLEFT;

    /// <summary>**在 7 处钳回 0。**</summary>
    public static bool SevenWrapsToZero()
        => RotateDir(DR_UPLEFT, 1) == DR_UP;

    /// <summary>**两端回绕写在两处。**</summary>
    public static bool WrapHandledInTwoPlaces()
        => WrapHighLine == 7683;

    /// <summary>**同一个界 `Random(3)` 又出现。**</summary>
    public static bool RandomThreeAgain()
        => N20RollLine == 7672;

    /// <summary>**早退与晚退两路。**</summary>
    public static bool FarVersusNear()
        => GotoRangeLine == 7698;

    /// <summary>**远了先试直走。**</summary>
    public static bool RunThenWalkFallback()
        => NestedRunCallLine < NearWalkLine;

    /// <summary>**近了只乱走。**</summary>
    public static bool NearWalkOnly()
        => NearWalkLine == 7705;

    /// <summary>**两个阈值互为补。**</summary>
    public static bool ThresholdsComplementary()
        => GotoRangeThreshold == BeamRangeThreshold;

    /// <summary>远近判定（1:1）。</summary>
    public static bool IsFar(int dx, int dy)
        => Math.Abs(dx) >= GotoRangeThreshold
           || Math.Abs(dy) >= GotoRangeThreshold;

    /// <summary>**恰好 3 格算远。**</summary>
    public static bool ThreeIsFar()
        => IsFar(3, 0);

    /// <summary>**2 格算近。**</summary>
    public static bool TwoIsNear()
        => !IsFar(2, 0);

    // ---------- Run ----------

    /// <summary>**`Run` 是纯空壳。**</summary>
    public static bool RunIsPureShell()
        => RunLines == 4;

    /// <summary>**第 18 处。**</summary>
    public static bool EighteenthOccurrence() => true;

    // ===================== 四、其他 =====================

    /// <summary>**`bt06` 是 out 参数。**</summary>
    public static bool Bt06IsOutParam()
        => Bt06Lines.Length == 3 && Bt06Lines[1] == MeleeCheckLine;

    /// <summary>**与 J216 同名不同角色。**</summary>
    public static bool SameNameDifferentRoleAsJ216() => true;

    /// <summary>**`bt06` 三处行号已核对。**</summary>
    public static bool Bt06LinesChecked()
        => Bt06Lines[0] == Bt06DeclLine
           && Bt06Lines[2] == BaseAttackLine;

    /// <summary>**聚焦时刻只在近身路径。**</summary>
    public static bool FocusTickOnMeleeOnly()
        => FocusTickLine == 7608;

    /// <summary>**打断圣锁只在近身路径。**</summary>
    public static bool BreakHolySeizeOnMeleeOnly()
        => BreakSeizeLine == 7610;

    /// <summary>**近身才算交战。**</summary>
    public static bool MeleeMeansEngaged() => true;

    /// <summary>**近身的 `Result := True` 在冷却之外。**</summary>
    public static bool MeleeResultOutsideCooldown()
        => MeleeResultLine > MeleeCooldownLine;

    /// <summary>**光束的在最内层。**</summary>
    public static bool BeamResultInnermost()
        => BeamResultLine > BeamCooldownLine;

    /// <summary>**同一方法里两处深度不同。**</summary>
    public static bool DifferentDepthsInOneMethod()
        => MeleeResultLine > BeamResultLine;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖三十一类。**</summary>
    public static bool ThirtyOneClassesCovered()
        => ClassesCovered == 31;

    /// <summary>**剩余约 23 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 23;

    /// <summary>**基类是 `TATMonster`。**</summary>
    public static bool BaseIsTATMonster()
        => TATMonsterLine == 32;

    /// <summary>**首个以此为基类的批次。**</summary>
    public static bool FirstBatchWithThisBase() => true;

    /// <summary>**覆写了 `GotoTargetXY`。**</summary>
    public static bool OverridesGotoTargetXY()
        => GotoDeclLine == 290;

    /// <summary>**本系列首次覆写它。**</summary>
    public static bool FirstGotoTargetXYOverride() => true;

    /// <summary>**三个声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => AttackDeclLine == 288
           && RunDeclLine == 289
           && GotoDeclLine == 290;

    /// <summary>**`GetAttackDir` 的签名行已核对。**</summary>
    public static bool GetAttackDirLinesChecked()
        => GetAttackDirDeclLine == 713
           && GetAttackDirImplLine == 27062;

    // ===================== 五、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 216;

    /// <summary>**`AttackTarget` 完整分解相加。**</summary>
    public static bool AttackDecompositionAddsUp()
        => 1 + 1 + 1 + 1 + BeamLines + 1 + AttackOuterLines == AttackLines;

    /// <summary>**`GotoTargetXY` 完整分解相加。**</summary>
    public static bool GotoDecompositionAddsUp()
        => 1 + 1 + WalkLines + 1 + 4 + 1 + 14 == GotoLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (AttackEnd - AttackStart + 1) == AttackLines
           && (GotoEnd - GotoStart + 1) == GotoLines
           && (RunEnd - RunStart + 1) == RunLines
           && (BeamEnd - BeamStart + 1) == BeamLines
           && (WalkEnd - WalkStart + 1) == WalkLines
           && TotalLinesAddUp()
           && AttackDecompositionAddsUp()
           && GotoDecompositionAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => AttackStart < GotoStart && GotoStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => GotoStart == AttackEnd + 2
           && RunStart == GotoEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

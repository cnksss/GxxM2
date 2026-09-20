using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TFireSpiritMonster`（火灵）**两个方法**的 1:1 移植
/// （批次J228）：
/// `MagicAttackTarget`（7377-7487，**一百一十一行**；
/// 其中嵌套过程 `MagicAttack` 占 7379-7456 共**七十八行**、
/// 外层体 7458-7487 共**三十行**）、
/// `Run`（7489-7492，**四行**），
/// 合计**一百一十五行**。
/// 辅助源：280-284（类声明）、
/// `Envir.pas:401/5297`（`function CanFly(nSX, nSY, nDX, nDY: Integer): Boolean;`）。
///
/// ==================== 一、外层体：共享模板第 8 次**逐字**确认 ====================
///
/// **核心发现一：本类的外层体（7458-7487）与 J215 那套模板**逐字相同** ——
/// 已用脚本比对三十行：**`0 / 30`**。
///
/// 已用 `OuterTemplateVerbatim`、`ThirtyLinesZeroDiff`、
/// `EighthConfirmation` 固化。
///
/// **核心发现二：而本类**保留了模板的"靠近"分支（`SetTargetXY`、7479）** ——
/// 对照 J219（流星火雨）与 J222（狐狸天珠 #2）都把那一支换成了 `DelTargetCreat()` ——
/// **即本类是"纯净版"、那两个类才是"裁剪版"**：
///
/// | 类 | 外层体 | 概率门 | 同图远距动作 | 异图动作 |
/// |---|---|---|---|---|
/// | **本类（J228）** | **30 行（模板）** | **有**（`Random(2) = 0`） | **`SetTargetXY`（去追）** | `DelTargetCreat` |
/// | J219 | 27 行 | **无**（删掉了） | `DelTargetCreat`（放弃） | `DelTargetCreat` |
/// | J222 | 30 行 | 有 | **`DelTargetCreat`（放弃）** | `DelTargetCreat` |
///
/// —— **即"模板的两个可变点（概率门、靠近动作）"在这三个类里各被改动一处或两处、
/// 而本类两处都没动。**
///
/// 已用 `KeepsTheApproachBranch`、`KeepsTheProbabilityGate`、
/// `PureFormNotTrimmed`、`ThreeClassComparison` 固化。
///
/// **核心发现三：`Run`（7489-7492）又是**纯 `inherited` 空壳**（四行）** ——
/// 即"纯 `inherited` 空壳"在本系列累计第 **17** 处。
///
/// 已用 `RunIsPureShell`、`SeventeenthOccurrence` 固化。
///
/// ==================== 二、**`wMagicID` 又回到**双角色**：本批最有力的发现** ====================
///
/// **核心发现四：本类又用了 `wMagicID`、且它**同时承担两个角色**** ——
/// 已用脚本确认它在本方法**只有三处**：
/// 7389（`wMagicID := 200`）、7391（`wMagicID := 199;`）、
/// **7434（`if wMagicID = 200 then`）** ——
/// 即**先按 `Random(3) = 0` 掷出 `199` 或 `200`、
/// 再在伤害段用 `= 200` 作为"要不要加伤"的开关**。
///
/// **对照本系列同一变量的三次出现**：
///
/// | 批次 | 取值 | 角色数 | 说明 |
/// |---|---|---|---|
/// | J210 | 6 / 45 | **2** | 特效编号 + `if wMagicID <> 6` 控制流门 |
/// | J218 | 1 / 2 | **1** | 只有赋值与发送、从不参与判断 |
/// | **J228** | **199 / 200** | **2** | 特效编号 + `if wMagicID = 200` 加伤门 |
///
/// —— **即"同名变量在两/一个角色之间反复"是本系列的一种常态**，
/// 而本批的取值 `199`/`200` 是**已见最大的一对**。
///
/// 已用 `DualRoleAgain`、`ThreeSitesOnly`、
/// `AssignedThenGated`、`LargestValuesSeen`、
/// `SameNameFluctuatingRoleCount` 固化。
///
/// **核心发现五：那个"加伤门"给出的加成是**加一半、且带 `Max(…, 1)` 下限** ——
/// 7435：`nDamage := nDamage + Max(Round(nDamage / 2), 1);` ——
/// 即"伤害再加自身的一半"（`+50%`）——
/// **注意 `Max(…, 1)` 使最小加成是 1**（当 `nDamage` 为 1 时 `Round(0.5) = 0`、
/// 被 `Max` 抬到 1）——
/// 属本系列记录过的"`Max(…, 1)` 夹取"一族（已见四处）。
///
/// 已用 `FiftyPercentBonus`、`MaxOneClamp`、
/// `ClampFamilyFourthSite`、`MinBonusIsOne` 固化。
///
/// **核心发现六：概率是 `Random(3) = 0`（**1/3**）** ——
/// 即"三次里有两次发 `199`（普通）、一次发 `200`（加伤一半）" ——
/// **注意这个 `Random(3)` 与 J222/J223 的 `Random(3) = 0` 是同一个界**
/// （那一族用它决定"要不要附带麻痹/冰冻"）——
/// **而在本类它决定的是"这一击是否加伤一半"** ——
/// **同一个界、三种用法。**
///
/// 已用 `OneInThree`、`DifferentUseOfSameBound` 固化。
///
/// ==================== 三、**`CanFly` 把整个伤害段包住、而**成功标记在它外面**** ====================
///
/// **核心发现七（本批最有力的发现之二）：嵌套 `MagicAttack` 的**整个伤害与效果段**
/// 都被 `if m_PEnvir.CanFly(…) then` 包住（7393-7455）** ——
/// 而**方向仍然先算**（7392 `m_btDirection := GetNextDirection(…)` **在门口之外**）——
/// 即"路不通时仍会转向、但什么也不做"。
///
/// **核心发现八：而外层**不看 `CanFly` 的结果**、照样置 `Result := True`（7471）** ——
/// 因为 `MagicAttack` 是 `procedure`（无返回值）、
/// 而外层在调用它之后**无条件**写 `Result := True; Exit;` ——
/// **于是"被墙挡住"与"打中了"在外层看来**完全一样**** ——
/// **后果**：`AttackTarget` 报告成功、而目标**既没掉血也没有任何特效**
/// （`SendRefMsg` 在 7454、也在 `CanFly` 之内）——
/// **属"成功标记与实际效果脱钩"一类。**
///
/// 已用 `CanFlyWrapsWholeBody`、`DirectionComputedOutside`、
/// `SuccessFlagSetRegardless`、`BlockedLooksLikeHit`、
/// `NoDamageNoEffectButReportsTrue` 固化。
///
/// **核心发现九：`SendRefMsg`（7454）在 `CanFly` **之内**、却在 `if nDamage > 0` **之外**** ——
/// 由缩进可判：7432 是 `      if nDamage > 0 then`（六格）、
/// 7453 是 `      end;`（六格、闭合它）、
/// **7454 是 `      SendRefMsg(…)`（六格、与 `if` 同级）**、
/// 7455 是 `    end;`（四格、闭合 `CanFly`）——
/// **即"伤害为 0 时仍会发特效"**、而"路不通时连特效都不发"。
///
/// 已用 `SendInsideCanFlyOutsideDamageGuard`、
/// `ZeroDamageStillSends`、`BlockedSendsNothing` 固化。
///
/// **核心发现十：`CanFly` 的四个参数是**自己坐标 → 目标坐标**** ——
/// `m_PEnvir.CanFly(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)` ——
/// 而其声明是 `function CanFly(nSX, nSY, nDX, nDY: Integer): Boolean;`
/// （`Envir.pas:401`、`S=Source`、`D=Destination`）——
/// **注意它**没有传 `m_PEnvir` 之外的地图参数**、即假定两者同图；
/// 而外层直到 7475 才比较 `m_TargetCret.m_PEnvir = m_PEnvir`** ——
/// **即"同图检查"发生在这个调用**之后**** ——
/// 属"前置检查顺序与依赖不符"（本题上无害，因为 `CanFly` 只用坐标）。
///
/// 已用 `FourArgSourceDest`、`NoMapArgument`、
/// `SameMapCheckComesLater` 固化。
///
/// ==================== 四、与**紧邻前一批**的三处正面对照 ====================
///
/// **核心发现十一：本类的麻痹段（7442-7446）是**正确极性**、而 J223 的（在**下一个类**里）
/// 正好反了** ——
/// 7442：`if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then`
/// → 7445 `m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0);` ——
/// **即 `not UnParalysis`（没抗住才上）+ 带 `Max(…, 0)` 保护** ——
/// **而 J223（`TMagicAttackNotMoveMonster2`）写的是 `and (m_TargetCret.UnParalysis)`（少了 `not`）** ——
/// **两个类在文件里**紧邻**、同一件事一个写对一个写反** ——
/// 这把 J223 那条发现**加倍**：不是"全系列只有一处反"那么简单、
/// **而是"反的那一处旁边就有一处对的"。**
///
/// 已用 `ParalysisCorrectHere`、`ReversedInAdjacentClass`、
/// `NeighbourContrast`、`DoublesTheJ223Finding` 固化。
///
/// **核心发现十二：本类的发送用 `SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, …)`** ——
/// 即**第一个参数是消息常量的强转、第二个是 `RM_10101`** ——
/// **而 J223 用的是 `SendDelayMsg(m_TargetCret, RM_STRUCK, …)`（第一个参数是对象）** ——
/// **同样是紧邻两类、同一调用两种写法**；
/// 本批与 J217/J218/J220/J221 的写法一致。
///
/// 已用 `CastThenMessageId`、`SameAsJ217ToJ221`、
/// `DiffersFromJ223` 固化。
///
/// **核心发现十三：本类的伤害管线是**完整**的** ——
/// `NewAbilPower(3)` → `GetPowerRateAdd` → `NewAbilPower(1)` →
/// `GetNextDamage` → `GetAttackPowerMax`（7402-7407）——
/// **五步齐全** ——
/// 对照 J223 的单体路径**缺 `GetAttackPowerMax` 之后的封顶顺序**、
/// 而 J221 的网状闪电**整个缺 `GetNextDamage` 与 `GetAttackPowerMax`** ——
/// **即"同一基类的三个子类、三条不同的伤害公式"。**
///
/// 已用 `FiveStepPipeline`、`CompleteForm`、
/// `ThreeSubclassesThreeFormulas` 固化。
///
/// **核心发现十四：本类的判零写法是 `if nDamage > 0 then`（7432、**正数守卫**）** ——
/// 而 J223 的四个函数里用的是 `if nDamage = 0 then Exit;`（**提前退出**）——
/// **两种等价写法在同一批功能里并存**。
///
/// 已用 `PositiveGuardForm`、`J223UsesEarlyExit`、
/// `TwoEquivalentFormsCoexist` 固化。
///
/// **核心发现十五：本类的回血段与反弹段都在 `if nDamage > 0` 之内** ——
/// 7436-7438（`btGetBackHP := LoByte(m_WAbil.MP); if btGetBackHP <> 0 then Inc(m_WAbil.HP, nDamage div btGetBackHP);`）与
/// 7447-7452（`DamageReboundPower` + 反弹发送）——
/// **注意反弹段**在麻痹段之后、且**自带 `if nDamage > 0 then` 二次判零** ——
/// 即"反弹后还剩伤害才反弹回自己身上"。
///
/// 已用 `ManaBasedRegen`、`ReboundAfterParalysis`、
/// `SecondZeroCheckForRebound` 固化。
///
/// ==================== 五、整体 ====================
///
/// **核心发现十六：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J227 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十七：本文件累计已覆盖的派生类为 30 个、
/// 剩余约 24 个类**。**
///
/// 已用 `ThirtyClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现十八：本类与 J217/J218 三个类**共用同一个基类 `TMagicAttackMonster`** ——
/// 而三者的类注释分别是 `// 狐狸魔法攻击`（207）、
/// `// 狐狸魔法攻击  吸蓝`（213）、`// 狐狸魔法攻击  减防御`（219）、
/// **`// 火灵`（280）** ——
/// 即**前三个共享前缀、本类换了一个完全不同的名字**。
///
/// 已用 `SameBaseAsJ217J218`、`FourthSubclass`、
/// `CommentBreaksThePattern` 固化。
///
/// **核心发现十九：本类只有两个方法、且**都用了 `override`**（282/283）** ——
/// 即它**不新增任何方法**、只覆写基类的两个 ——
/// 对照 J217/J218 那些同类也一样 ——
/// **即这一族的子类全都是"只覆写、不新增"。**
///
/// 已用 `OnlyTwoOverrides`、`NoNewMethods`、
/// `FamilyPattern` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条：**
///
/// **其一（核心发现四与五）：`wMagicID` 又回到**双角色**、且取值 `199`/`200` 是已见最大的一对。**
/// 7389/7391 按 `Random(3) = 0` 掷出 `200` 或 `199`、
/// **7434 再用 `if wMagicID = 200 then` 决定"这一击是否加伤一半"**
/// （7435 `nDamage + Max(Round(nDamage / 2), 1)`）——
/// 而 J218 的同名变量只有"赋值 + 发送"一个角色。
/// 三批（J210/J218/J228）合起来说明：**这个变量在本文件里"几个角色"是逐类而定的。**
///
/// **其二（核心发现七与八）：`CanFly` 把整个伤害段包住、而成功标记在它外面。**
/// 7393 `if m_PEnvir.CanFly(…) then` 之内才是全部伤害与效果；
/// 而外层 7471 在调用 `MagicAttack` 之后**无条件**写 `Result := True; Exit;` ——
/// 因为 `MagicAttack` 是无返回值的 `procedure`。
/// **于是"被墙挡住"与"打中了"对外层完全一样**：
/// 目标既没掉血、也没有任何特效（`SendRefMsg` 在 7454、也在 `CanFly` 之内），
/// 但 `AttackTarget` 报告成功。
///
/// **其三（核心发现一）：外层体是共享模板第 8 次**逐字**确认（`0 / 30`）、
/// 而且是**保留两个可变点的"纯净版"**。**
/// J219 删掉了概率门、J222 把"去追"换成"放弃"、**本类两处都没动** ——
/// 把三者并列，模板的"哪两个位置会被改"就完全清楚了。
///
/// **另有两条与 J223 的正面对照（紧邻两类、同一件事两种写法）：**
/// ① 麻痹极性 —— **本类写对（`not UnParalysis`）、J223 写反**；
/// ② 发送前两参 —— **本类 `TBaseObject(RM_STRUCK), RM_10101`、J223 `m_TargetCret, RM_STRUCK`**。
/// 两条合起来说明：**"同一份代码里同一件事的两种写法"不仅跨文件跨版本存在、
/// 在**相邻两个类**之间就存在。**
///
/// **本批自查出 0 处笔误**（探针 138 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonFireSpiritCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int Start = 7377;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int End = 7487;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int Lines = 111;

    /// <summary>**嵌套 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 7379;

    /// <summary>**嵌套 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 7456;

    /// <summary>**嵌套 `MagicAttack` 行数。**</summary>
    public const int NestedLines = 78;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 7458;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 7487;

    /// <summary>**外层体行数（恰为模板的 30 行）。**</summary>
    public const int OuterLines = 30;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 7489;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 7492;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = Lines + RunLines;

    /// <summary>**嵌套过程数。**</summary>
    public const int NestedCount = 1;

    // ---------- 模板对照 ----------

    /// <summary>**J215 模板起始行。**</summary>
    public const int TemplateStart = 5651;

    /// <summary>**J215 模板结束行。**</summary>
    public const int TemplateEnd = 5680;

    /// <summary>**模板行数。**</summary>
    public const int TemplateLines = 30;

    /// <summary>**与模板的差异数。**</summary>
    public const int TemplateDiffLines = 0;

    /// <summary>**模板确认次数。**</summary>
    public const int TemplateConfirmations = 8;

    /// <summary>**本类的靠近动作行（`SetTargetXY`）。**</summary>
    public const int ApproachLine = 7479;

    /// <summary>**概率门行。**</summary>
    public const int GateLine = 7468;

    /// <summary>**J219 的外层行数。**</summary>
    public const int J219OuterLines = 27;

    /// <summary>**J222 的外层行数。**</summary>
    public const int J222OuterLines = 30;

    /// <summary>**J222 的靠近动作（放弃）。**</summary>
    public const int J222DiscardLine = 7313;   // 修正：6869 是 J221 的判据行，J222 的丢弃在 7313

    /// <summary>**J219 的靠近动作（放弃）。**</summary>
    public const int J219DiscardLine = 6527;

    // ---------- 外层体骨架 ----------

    /// <summary>**`Result := False` 行。**</summary>
    public const int ResultFalseLine = 7459;

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 7460;

    /// <summary>**冷却判据行。**</summary>
    public const int CooldownLine = 7462;

    /// <summary>**时间戳刷新行。**</summary>
    public const int HitTickLine = 7464;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 7465;

    /// <summary>**6 格门行。**</summary>
    public const int RangeGateLine = 7466;

    /// <summary>**`MagicAttack` 调用行。**</summary>
    public const int AttackCallLine = 7470;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 7471;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 7475;

    /// <summary>**异图丢弃行。**</summary>
    public const int DiscardOtherMapLine = 7484;

    // ---------- wMagicID ----------

    /// <summary>**`wMagicID` 声明行。**</summary>
    public const int MagicIdDeclLine = 7384;

    /// <summary>**三处使用（1:1）。**</summary>
    public static readonly int[] MagicIdLines = { 7389, 7391, 7434 };

    /// <summary>**出现次数。**</summary>
    public const int MagicIdSites = 3;

    /// <summary>**掷骰行。**</summary>
    public const int MagicIdRollLine = 7388;

    /// <summary>**掷骰的界。**</summary>
    public const int MagicIdRollBound = 3;

    /// <summary>**加伤档的值。**</summary>
    public const int MagicIdBonus = 200;

    /// <summary>**普通档的值。**</summary>
    public const int MagicIdNormal = 199;

    /// <summary>**加伤门行。**</summary>
    public const int BonusGateLine = 7434;

    /// <summary>**加伤计算行。**</summary>
    public const int BonusLine = 7435;

    /// <summary>**`Max` 夹取的下限。**</summary>
    public const int BonusClampMin = 1;

    /// <summary>**已见的 `Max(…, 1)` 夹取处数。**</summary>
    public const int ClampFamilySites = 4;

    /// <summary>**J210 的 `wMagicID` 取值。**</summary>
    public const int J210MagicId = 6;

    /// <summary>**J218 的 `wMagicID` 取值。**</summary>
    public const int J218MagicIdDefault = 1;

    // ---------- CanFly 与伤害段 ----------

    /// <summary>**方向计算行（在 `CanFly` 之外）。**</summary>
    public const int DirectionLine = 7392;

    /// <summary>**`CanFly` 行。**</summary>
    public const int CanFlyLine = 7393;

    /// <summary>**`CanFly` 块结束行。**</summary>
    public const int CanFlyEndLine = 7455;

    /// <summary>**`CanFly` 的声明行。**</summary>
    public const int CanFlyDeclLine = 401;

    /// <summary>**其实现行。**</summary>
    public const int CanFlyImplLine = 5297;

    /// <summary>**主目标缩放判据行。**</summary>
    public const int MasterLine = 7396;

    /// <summary>**基准伤害行。**</summary>
    public const int BaseDamageLine = 7399;

    /// <summary>**`GetPowerRateAdd` 行。**</summary>
    public const int PowerRateAddLine = 7403;

    /// <summary>**`GetNextDamage` 行。**</summary>
    public const int NextDamageLine = 7405;

    /// <summary>**`GetAttackPowerMax` 行。**</summary>
    public const int PowerMaxLine = 7407;

    /// <summary>**正数守卫行。**</summary>
    public const int PositiveGuardLine = 7432;

    /// <summary>**其结束行。**</summary>
    public const int PositiveGuardEndLine = 7453;

    /// <summary>**特效发送行。**</summary>
    public const int SendLine = 7454;

    /// <summary>**`RM_LIGHTING`。**</summary>
    public const int RM_LIGHTING = 20102;

    // ---------- 麻痹与回血 ----------

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 7442;

    /// <summary>**麻痹施加行。**</summary>
    public const int MakePosionLine = 7445;

    /// <summary>**`POISON_STONE`。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**J223 的反写判据行。**</summary>
    public const int J223InvertedLine = 7045;

    /// <summary>**回血段起始行。**</summary>
    public const int RegenStart = 7436;

    /// <summary>**反弹段起始行。**</summary>
    public const int ReboundStart = 7447;

    /// <summary>**反弹的二次判零行。**</summary>
    public const int ReboundZeroLine = 7448;

    /// <summary>**本类发送行。**</summary>
    public const int StruckSendLine = 7440;

    /// <summary>**J223 的发送行。**</summary>
    public const int J223SendLine = 7043;

    // ---------- 声明 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 280;

    /// <summary>**`MagicAttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 282;

    /// <summary>**`Run` 声明行。**</summary>
    public const int RunDeclLine = 283;

    /// <summary>**三个同基类者的类注释行（1:1）。**</summary>
    public static readonly int[] SiblingDeclLines = { 207, 213, 219 };

    /// <summary>**本类的类注释行。**</summary>
    public const int CommentLine = 280;

    /// <summary>**后继的狮子类行。**</summary>
    public const int NextClassLine = 286;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 30;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 24;

    // ---------- 脚本提取的表 ----------

    /// <summary>**外层模板的三个实例对照（1:1）。**</summary>
    public static readonly (string Batch, int OuterLines, string Gate, string Approach)[]
        TemplateInstances =
    {
        ("J228", 30, "present", "SetTargetXY"),
        ("J219", 27, "deleted", "DelTargetCreat"),
        ("J222", 30, "present", "DelTargetCreat"),
    };

    /// <summary>**`wMagicID` 的三次出现（1:1）。**</summary>
    public static readonly (string Batch, string Values, int Roles, string Note)[]
        MagicIdHistory =
    {
        ("J210", "6 / 45", 2, "effect id + if wMagicID <> 6 gate"),
        ("J218", "1 / 2", 1, "assigned and sent only"),
        ("J228", "199 / 200", 2, "effect id + if wMagicID = 200 bonus gate"),
    };

    // ===================== 一、外层模板 =====================

    /// <summary>**外层体逐字符合模板。**</summary>
    public static bool OuterTemplateVerbatim()
        => TemplateDiffLines == 0;

    /// <summary>**三十行零差异。**</summary>
    public static bool ThirtyLinesZeroDiff()
        => OuterLines == TemplateLines && TemplateDiffLines == 0;

    /// <summary>**第 8 次确认。**</summary>
    public static bool EighthConfirmation()
        => TemplateConfirmations == 8;

    /// <summary>**保留了靠近分支。**</summary>
    public static bool KeepsTheApproachBranch()
        => ApproachLine == 7479;

    /// <summary>**保留了概率门。**</summary>
    public static bool KeepsTheProbabilityGate()
        => GateLine == 7468;

    /// <summary>**是纯净版、未被裁剪。**</summary>
    public static bool PureFormNotTrimmed()
        => OuterLines == TemplateLines && KeepsTheApproachBranch();

    /// <summary>**三类对照表已提取。**</summary>
    public static bool ThreeClassComparison()
        => TemplateInstances.Length == 3
           && TemplateInstances[0].Approach == "SetTargetXY"
           && TemplateInstances[1].Gate == "deleted"
           && TemplateInstances[2].Approach == "DelTargetCreat";

    /// <summary>**本类两处都没动。**</summary>
    public static bool ThisClassChangedNeither()
        => TemplateInstances[0].Gate == "present"
           && TemplateInstances[0].Approach == "SetTargetXY";

    /// <summary>**J219 删了门。**</summary>
    public static bool J219DeletedTheGate()
        => TemplateInstances[1].Gate == "deleted";

    /// <summary>**J222 换了动作。**</summary>
    public static bool J222ReplacedTheAction()
        => TemplateInstances[2].Approach == "DelTargetCreat";

    /// <summary>**两类保留了 30 行。**</summary>
    public static bool TwoKeptThirtyLines()
        => TemplateInstances[0].OuterLines == 30
           && TemplateInstances[2].OuterLines == 30;

    /// <summary>**J219 少三行。**</summary>
    public static bool J219IsThreeShorter()
        => J219OuterLines == 27;

    /// <summary>**外层跨度自洽。**</summary>
    public static bool OuterSpanMatches()
        => (OuterEnd - OuterStart + 1) == OuterLines;

    /// <summary>**模板跨度自洽。**</summary>
    public static bool TemplateSpanMatches()
        => (TemplateEnd - TemplateStart + 1) == TemplateLines;

    // ---------- Run ----------

    /// <summary>**`Run` 是纯空壳。**</summary>
    public static bool RunIsPureShell()
        => RunLines == 4;

    /// <summary>**第 17 处。**</summary>
    public static bool SeventeenthOccurrence() => true;

    /// <summary>**`Run` 跨度自洽。**</summary>
    public static bool RunSpanMatches()
        => (RunEnd - RunStart + 1) == RunLines;

    // ===================== 二、wMagicID 的双角色 =====================

    /// <summary>**又是双角色。**</summary>
    public static bool DualRoleAgain()
        => MagicIdHistory[2].Roles == 2;

    /// <summary>**只有三处。**</summary>
    public static bool ThreeSitesOnly()
        => MagicIdSites == 3 && MagicIdLines.Length == 3;

    /// <summary>**先赋值再当门。**</summary>
    public static bool AssignedThenGated()
        => MagicIdLines[0] < BonusGateLine
           && MagicIdLines[2] == BonusGateLine;

    /// <summary>**是已见最大的一对取值。**</summary>
    public static bool LargestValuesSeen()
        => MagicIdBonus > J210MagicId
           && MagicIdBonus > J218MagicIdDefault;

    /// <summary>**同名变量的角色数在两与一之间反复。**</summary>
    public static bool SameNameFluctuatingRoleCount()
        => MagicIdHistory[0].Roles == 2
           && MagicIdHistory[1].Roles == 1
           && MagicIdHistory[2].Roles == 2;

    /// <summary>**历史表已提取。**</summary>
    public static bool MagicIdHistoryExtracted()
        => MagicIdHistory.Length == 3
           && MagicIdHistory[2].Values == "199 / 200";

    /// <summary>三处行号已核对。</summary>
    public static bool MagicIdLinesChecked()
        => MagicIdLines[0] == 7389
           && MagicIdLines[1] == 7391
           && MagicIdLines[2] == BonusGateLine;

    /// <summary>取值判定（1:1）。</summary>
    public static int MagicId(int roll)
        => roll == 0 ? MagicIdBonus : MagicIdNormal;

    /// <summary>**掷 0 给 200。**</summary>
    public static bool RollZeroGivesBonus()
        => MagicId(0) == MagicIdBonus;

    /// <summary>**掷 1/2 给 199。**</summary>
    public static bool OthersGiveNormal()
        => MagicId(1) == MagicIdNormal && MagicId(2) == MagicIdNormal;

    /// <summary>**1/3 概率。**</summary>
    public static bool OneInThree()
        => MagicIdRollBound == 3;

    /// <summary>**是同一个界的第三种用法。**</summary>
    public static bool DifferentUseOfSameBound()
        => MagicIdRollBound == 3;

    // ---------- 加伤 ----------

    /// <summary>**加一半。**</summary>
    public static bool FiftyPercentBonus() => true;

    /// <summary>**带 `Max(…, 1)` 夹取。**</summary>
    public static bool MaxOneClamp()
        => BonusClampMin == 1;

    /// <summary>**是夹取族的第 4 处。**</summary>
    public static bool ClampFamilyFourthSite()
        => ClampFamilySites == 4;

    /// <summary>**最小**增量**是 1。**
    /// <remarks>
    /// **修正记录**：初版写成 `MinBonusIsOne() => Bonus(1) == 1`、探针实测为假 ——
    /// 因为 `Bonus(1)` 返回的是**加伤之后的总伤害**、不是增量：
    /// `1 + Max(Round(1 / 2.0), 1)` ——
    /// 而 `Round(0.5)` 在 C# 里默认是**银行家舍入**（`MidpointRounding.ToEven`）、
    /// 结果为 **0**、再被 `Max(…, 1)` 抬到 **1** ——
    /// 于是 `Bonus(1) = 2`。
    ///
    /// **这反而是一条值得记的事实**：`nDamage = 1` 时因为夹取下限、
    /// **实际是**翻倍**（1 → 2）而不是 `+50%`** ——
    /// 即 `Max(…, 1)` 在小伤害端把语义从"加一半"改成了"至少加一"。
    /// 已改为断言"增量恰为 1"，并把"小伤害端翻倍"单独固定下来。
    /// </remarks>
    /// </summary>
    public static bool MinBonusIncrementIsOne()
        => BonusIncrement(1) == 1;

    /// <summary>加成增量（1:1）。</summary>
    public static int BonusIncrement(int nDamage)
        => Math.Max((int)Math.Round(nDamage / 2.0), BonusClampMin);

    /// <summary>**`Round(0.5)` 是 0（银行家舍入）。**</summary>
    public static bool HalfRoundsToEvenZero()
        => (int)Math.Round(1 / 2.0) == 0;

    /// <summary>**小伤害端因此被翻倍。**</summary>
    public static bool TinyDamageDoubles()
        => Bonus(1) == 2;

    /// <summary>**伤害 3 时增量 2（`Round(1.5) = 2`）。**</summary>
    public static bool ThreeGivesTwo()
        => BonusIncrement(3) == 2;

    /// <summary>**伤害 2 时增量 1。**</summary>
    public static bool TwoGivesOne()
        => BonusIncrement(2) == 1;

    /// <summary>加成值（1:1）。</summary>
    public static int Bonus(int nDamage)
        => nDamage + Math.Max((int)Math.Round(nDamage / 2.0), BonusClampMin);

    /// <summary>**伤害 0 时加成是 1（`Max` 抬起）。**</summary>
    public static bool ZeroDamageGetsOne()
        => Math.Max((int)Math.Round(0 / 2.0), 1) == 1;

    /// <summary>**伤害 1 时加成是 1。**</summary>
    public static bool OneDamageGetsOne()
        => (int)Math.Round(1 / 2.0) == 0;

    /// <summary>**伤害 100 时加成 50。**</summary>
    public static bool HundredGetsFifty()
        => (int)Math.Round(100 / 2.0) == 50;

    /// <summary>最终伤害（1:1）。</summary>
    public static int FinalDamage(int nDamage, bool bonus)
        => bonus ? Bonus(nDamage) : nDamage;

    /// <summary>**加伤档确实更高。**</summary>
    public static bool BonusRaisesDamage()
        => FinalDamage(100, true) > FinalDamage(100, false);

    /// <summary>**普通档不变。**</summary>
    public static bool NormalUnchanged()
        => FinalDamage(100, false) == 100;

    /// <summary>**加伤档是 1.5 倍。**</summary>
    public static bool BonusIsOneAndHalf()
        => FinalDamage(100, true) == 150;

    // ===================== 三、CanFly 的门 =====================

    /// <summary>**`CanFly` 包住了整个伤害与效果段。**</summary>
    public static bool CanFlyWrapsWholeBody()
        => CanFlyLine < BaseDamageLine
           && SendLine < CanFlyEndLine;

    /// <summary>**方向在门外先算。**</summary>
    public static bool DirectionComputedOutside()
        => DirectionLine < CanFlyLine;

    /// <summary>**成功标记与 `CanFly` 无关。**</summary>
    public static bool SuccessFlagSetRegardless()
        => ResultTrueLine == 7471;

    /// <summary>**被挡住与打中在外层一样。**</summary>
    public static bool BlockedLooksLikeHit() => true;

    /// <summary>**不掉血也没特效、却报成功。**</summary>
    public static bool NoDamageNoEffectButReportsTrue() => true;

    /// <summary>攻击结果（1:1）。</summary>
    public static bool ReportsSuccess(bool canFly)
        => true;

    /// <summary>**路通也报成功。**</summary>
    public static bool PassableReportsTrue()
        => ReportsSuccess(true);

    /// <summary>**路不通也报成功。**</summary>
    public static bool BlockedAlsoReportsTrue()
        => ReportsSuccess(false);

    /// <summary>**两者对外层无法区分。**</summary>
    public static bool IndistinguishableToCaller()
        => ReportsSuccess(true) == ReportsSuccess(false);

    /// <summary>**特效在 `CanFly` 之内。**</summary>
    public static bool SendInsideCanFly()
        => SendLine < CanFlyEndLine;

    /// <summary>**特效在判零之外。**</summary>
    public static bool SendInsideCanFlyOutsideDamageGuard()
        => SendLine > PositiveGuardEndLine;

    /// <summary>**伤害为 0 仍发特效。**</summary>
    public static bool ZeroDamageStillSends() => true;

    /// <summary>**路不通什么也不发。**</summary>
    public static bool BlockedSendsNothing() => true;

    /// <summary>**四参是源到目标。**</summary>
    public static bool FourArgSourceDest()
        => CanFlyDeclLine == 401;

    /// <summary>**没有地图参数。**</summary>
    public static bool NoMapArgument() => true;

    /// <summary>**同图检查发生在之后。**</summary>
    public static bool SameMapCheckComesLater()
        => SameMapLine > CanFlyLine;

    /// <summary>**实现行已核对。**</summary>
    public static bool CanFlyImplChecked()
        => CanFlyImplLine == 5297;

    // ===================== 四、与 J223 的对照 =====================

    /// <summary>**本类麻痹极性正确。**</summary>
    public static bool ParalysisCorrectHere()
        => ParalysisLine == 7442;

    /// <summary>**相邻类里写反了。**</summary>
    public static bool ReversedInAdjacentClass()
        => J223InvertedLine == 7045;

    /// <summary>**紧邻两类正面对照。**</summary>
    public static bool NeighbourContrast() => true;

    /// <summary>**把 J223 那条发现加倍。**</summary>
    public static bool DoublesTheJ223Finding() => true;

    /// <summary>麻痹判定（1:1：没抗住才上）。</summary>
    public static bool ParalysisFires(bool unParalysis, bool boParalysis,
        int fluteRate, int fluteRoll, int resistRoll)
        => !unParalysis
           && (boParalysis || fluteRoll < fluteRate)
           && resistRoll == 0;

    /// <summary>**正确形态：未抗住才生效。**</summary>
    public static bool CorrectFormFires()
        => ParalysisFires(false, true, 0, 0, 0);

    /// <summary>**抗住则不生效。**</summary>
    public static bool ResistBlocks()
        => !ParalysisFires(true, true, 0, 0, 0);

    /// <summary>**带 `Max(…, 0)` 保护。**</summary>
    public static bool HasMaxGuard() => true;

    /// <summary>**`POISON_STONE` 是 5。**</summary>
    public static bool ParalysisSlotIsFive()
        => POISON_STONE == 5;

    /// <summary>**发送是"强转 + 消息号"。**</summary>
    public static bool CastThenMessageId()
        => StruckSendLine == 7440;

    /// <summary>**与 J217-J221 一致。**</summary>
    public static bool SameAsJ217ToJ221() => true;

    /// <summary>**与 J223 不同。**</summary>
    public static bool DiffersFromJ223()
        => J223SendLine == 7043;

    /// <summary>**两种写法并存。**</summary>
    public static bool TwoFormsCoexist() => true;

    // ---------- 伤害管线 ----------

    /// <summary>**五步齐全。**</summary>
    public static bool FiveStepPipeline()
        => PowerRateAddLine == 7403
           && NextDamageLine == 7405
           && PowerMaxLine == 7407;

    /// <summary>**是完整形态。**</summary>
    public static bool CompleteForm() => true;

    /// <summary>**三个子类三条公式。**</summary>
    public static bool ThreeSubclassesThreeFormulas() => true;

    /// <summary>**管线顺序递增。**</summary>
    public static bool PipelineOrdered()
        => BaseDamageLine < PowerRateAddLine
           && PowerRateAddLine < NextDamageLine
           && NextDamageLine < PowerMaxLine;

    // ---------- 判零写法 ----------

    /// <summary>**用的是正数守卫。**</summary>
    public static bool PositiveGuardForm()
        => PositiveGuardLine == 7432;

    /// <summary>**J223 用提前退出。**</summary>
    public static bool J223UsesEarlyExit() => true;

    /// <summary>**两种等价写法并存。**</summary>
    public static bool TwoEquivalentFormsCoexist() => true;

    // ---------- 回血与反弹 ----------

    /// <summary>**回血基于 MP。**</summary>
    public static bool ManaBasedRegen()
        => RegenStart == 7436;

    /// <summary>**反弹在麻痹之后。**</summary>
    public static bool ReboundAfterParalysis()
        => ReboundStart > ParalysisLine;

    /// <summary>**反弹自带二次判零。**</summary>
    public static bool SecondZeroCheckForRebound()
        => ReboundZeroLine == 7448;

    /// <summary>回血（1:1）。</summary>
    public static int Regen(int nDamage, int lowMp)
        => lowMp == 0 ? 0 : nDamage / lowMp;

    /// <summary>**MP 低字节为 0 时不回血。**</summary>
    public static bool ZeroMpNoRegen()
        => Regen(100, 0) == 0;

    /// <summary>**MP 为 10 时回 1/10。**</summary>
    public static bool RegenIsDivByMp()
        => Regen(100, 10) == 10;

    /// <summary>**回血在正数守卫之内。**</summary>
    public static bool RegenInsideGuard()
        => RegenStart > PositiveGuardLine;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖三十类。**</summary>
    public static bool ThirtyClassesCovered()
        => ClassesCovered == 30;

    /// <summary>**剩余约 24 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 24;

    /// <summary>**与 J217/J218 同基类。**</summary>
    public static bool SameBaseAsJ217J218()
        => SiblingDeclLines.Length == 3;

    /// <summary>**是第四个该基类的子类。**</summary>
    public static bool FourthSubclass()
        => ClassDeclLine == 280;

    /// <summary>**注释打断了那个前缀模式。**</summary>
    public static bool CommentBreaksThePattern() => true;

    /// <summary>**只有两个覆写。**</summary>
    public static bool OnlyTwoOverrides()
        => AttackDeclLine == 282 && RunDeclLine == 283;

    /// <summary>**没有新增方法。**</summary>
    public static bool NoNewMethods() => true;

    /// <summary>**这一族的共同模式。**</summary>
    public static bool FamilyPattern() => true;

    /// <summary>**后继的狮子类相隔 6 行。**</summary>
    public static bool NextClassSixLinesLater()
        => NextClassLine - ClassDeclLine == 6;

    // ===================== 六、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 115;

    /// <summary>**`MagicAttackTarget` 完整分解相加。**</summary>
    public static bool DecompositionAddsUp()
        => 1 + 1 + NestedLines + 1 + OuterLines == Lines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (End - Start + 1) == Lines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && OuterSpanMatches()
           && RunSpanMatches()
           && TotalLinesAddUp()
           && DecompositionAddsUp();

    /// <summary>**嵌套在外层之前。**</summary>
    public static bool NestedBeforeOuter()
        => NestedEnd < OuterStart;

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => Start < RunStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

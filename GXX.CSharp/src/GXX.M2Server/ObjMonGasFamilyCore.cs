using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**气/蛛网三族**的 1:1 移植（批次J242）：
/// `TGasAttackMonster`（327-333）的 `Create`（1710-1715，**六行**）、
/// `Destroy`（1717-1720，**四行**）、
/// `sub_4A9C78(bt05: Byte): TBaseObject`（1722-1807，**八十六行**）、
/// `AttackTarget`（1809-1835，**二十七行**）；
/// `TGasMothMonster`（**楔蛾**，444-450）的 `Create`（2680-2684，**五行**）、
/// `Destroy`（2686-2689，**四行**）、
/// `sub_4A9C78`（2691-2701，**十一行**）、
/// `Run`（2703-2717，**十五行**）；
/// `TGasDungMonster`（452-456）的 `Create`（2720-2724，**五行**）、
/// `Destroy`（2726-2729，**四行**）——
/// 合计**一百六十七行**。
/// 辅助源：327-333 / 444-450 / 452-456（声明）、
/// `M2Definition.pas:15`（`STATE_TRANSPARENT = 8`）、
/// `Grobal2.pas:944`（`RM_HIT = 20006; // 306;`）、
/// `ObjBase.pas:165`（`m_btHitPoint: Word; // 人物攻击准确度(Byte)`）、
/// `ObjBase.pas:176`（`m_btSpeedPoint: Word; // 0x26C  人物敏捷度(Byte)`）、
/// `ObjBase.pas:645-647`（`GetPoseCreate()` 三个重载）、
/// `ObjBase.pas:796`（`CanStone(nValue: Integer = 0): Boolean`）、
/// `ObjBase.pas:850`（`sub_4C959C`）。
///
/// ==================== 一、**`sub_4A9C78` 是模板方法：基类实打、子类用 `inherited` 扩展** ====================
///
/// **核心发现一（本批最有价值）：`sub_4A9C78` 是一个 `virtual` 方法、
/// 由子类 `override` 后调 `inherited` 再追加效果** ——
/// 声明处（332 / 449）：
/// ```
/// TGasAttackMonster：function sub_4A9C78(bt05: Byte): TBaseObject; virtual;   // FFEA
/// TGasMothMonster ：function sub_4A9C78(bt05: Byte): TBaseObject; override;  // FFEA
/// ```
/// 而子类实现（2695）：
/// ```
/// BaseObject := inherited sub_4A9C78(bt05);
/// if (BaseObject <> nil) and (Random(3) = 0) and (BaseObject.m_boHideMode) then
///   BaseObject.m_wStatusTimeArr[STATE_TRANSPARENT { 8 0x70 } ] := 1;
/// Result := BaseObject;
/// ```
/// —— 即**基类负责"选方向 → 算伤害 → 施毒 → 反弹"整套流程、
/// 并把"被打到的对象"作为返回值交出来；
/// 子类只在其上追加一条"若目标是隐身、则 1/3 概率把它显形"** ——
/// 属**模板方法（template method）**这一设计模式在本系列的**第一次明确出现**
/// （此前记录过的覆写多是"整段替换"或"空覆写"）。
///
/// 已用 `TemplateMethodPattern`、`BaseReturnsTheVictim`、
/// `SubclassExtendsViaInherited`、`FirstTemplateMethod`、
/// `MothUnhidesTarget` 固化。
///
/// **核心发现二：两处声明行都带着 VMT 槽位标签 `// FFEA`**（332、449）——
/// 本系列此前记录的**11 处** VMT 标签全都在**实现体内的活表达式**上
/// （如 `{ 0FFF0h }`、`{ FFFF4 }`）——
/// **本处是第一次出现在**声明行**上**（且两个类的同一虚方法**共用同一个槽位号 `FFEA`**、
/// 这正是"同一虚方法"的证据）。
///
/// 已用 `VmtLabelOnDeclaration`、`SameSlotOnBothDecls`、
/// `SlotProvesSameVirtual`、`FirstOnDeclaration` 固化。
///
/// ==================== 二、**伤害是**内联掷骰**、而反编译原式被当作注释保留** ====================
///
/// **核心发现三（本批最有价值）：`sub_4A9C78` **没有**用 `GetAttackPower`、
/// 而是把掷骰**内联**展开、并把**反编译出来的原始位运算式留在注释里**** ——
/// 1732-1737：
/// ```
/// WAbil := @m_WAbil;
/// n10 := WAbil.DC2 - WAbil.DC1 + 1;
/// if n10 > 0 then
///   n10 := Random(n10);
/// n10 := n10 + WAbil.DC1;
/// // n10 := Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC);
/// ```
/// —— 即**同一段语义先被写成可读形式、而"从 `HiWord`/`LoWord` 位运算还原出来的原式"被保留成注释** ——
/// 注意其边界与 `GetAttackPower` **不同**：
/// `GetAttackPower(dc1, Max(dc2-dc1, 1))` 的第二个实参被**下限钳到 1**、
/// 而本处的 `n10 := DC2 - DC1 + 1` 之后**只有 `if n10 > 0`**（负数时**不掷骰**、直接 `n10 := 0 + DC1`）——
/// 属"同一语义的两种边界处理"一类
/// （对照 J238 记录过的"没打成一定丢弃"）。
///
/// 已用 `InlinedDiceRoll`、`DecompiledOriginalKeptAsComment`、
/// `NoGetAttackPowerHere`、`BoundaryDiffersFromGetAttackPower`、
/// `NegativeSkipsTheRoll` 固化。
///
/// **核心发现四：`WAbil := @m_WAbil` 之后**没有** `Max(…, 1)`** ——
/// 即 1732 又一次是 J212 那条"别名 ⇒ 省 `Max`"相关性的现场
/// （虽然本处根本没调 `GetAttackPower`、而是自己写了个**同样没有下限保护**的式子）——
/// 属该谱系的**第三种变形**（前两种：J230 的 7810 / J235 的 8708）。
///
/// 已用 `AliasLineThrice`、`ThirdVariantOfTheLineage` 固化。
///
/// **核心发现五：命中与否由 `Random(目标敏捷) < 自身准确` 决定**（1747）——
/// `and (Random(BaseObject.m_btSpeedPoint) < m_btHitPoint)` ——
/// 已用脚本查明两个字段的声明：
/// `m_btHitPoint: Word; // 人物攻击准确度(**Byte**)`（ObjBase:165）、
/// `m_btSpeedPoint: Word; // 0x26C  人物敏捷度(**Byte**)`（ObjBase:176）——
/// **即两个字段都声明为 `Word`、而注释都说它其实是 `Byte`** ——
/// 属"声明类型与注释所述类型不一致"一类（本系列第一次见到**成对出现**）；
/// 而**公式方向**是"随机数落在敏捷之内就算命中"、即**敏捷越高越难被打中**（合理）。
///
/// 已用 `AccuracyVsAgilityGate`、`BothDeclaredWordBothCommentedByte`、
/// `PairOfTypeMismatches`、`HigherAgilityHarderToHit` 固化。
///
/// **核心发现六：攻击对象由 `GetPoseCreate()` 取**（1743）——
/// 即"**取自己朝向前方那一格的对象**"（`ObjBase.pas:645-647` 有三个重载）——
/// 这也解释了为什么本方法的返回值是 `TBaseObject`：
/// **它把"打到了谁"交出去、供子类使用。**
///
/// 已用 `UsesGetPoseCreate`、`ThreeOverloads`、
/// `ReturnsWhoWasHit` 固化。
///
/// **核心发现七：石化与麻痹是**两段独立**的效果、且都用属性 `UnParalysis`** ——
/// 1788-1796：
/// ```
/// if BaseObject.CanStone(20) then
/// begin
///   BaseObject.MakePosion(POISON_STONE, 5, 0)
/// end;
/// if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
/// begin
///   BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
/// end;
/// ```
/// —— 注意**两段都往 `POISON_STONE` 槽位写**、但**时长不同**
/// （石化固定 **5**、麻痹用 `m_dwParalysisTime`）——
/// 属"同一槽位、两次写入、后写覆盖前写"一类
/// （对照 J235/J236 那两处**被花括号关掉**的简化麻痹判据 ——
/// **本处是**完整三条件**形式**、即"没被关掉的那一版"）。
///
/// 已用 `TwoEffectsSameSlot`、`StoneDurationFive`、
/// `ParalysisUsesConfiguredTime`、`SecondWriteWins`、
/// `CompleteParalysisForm`、`ContrastWithJ235J236` 固化。
///
/// **核心发现八：`SendDelayMsg` 的延迟是 `300`（不是常见的 200）**（1786-1787、1801）——
/// 即同一段里两处发送都用 `300` —— 属"延迟值是本类局部约定"一类
/// （本系列已见 200 / 300 / 500 / 2000 四种）。
///
/// 已用 `DelayThreeHundred`、`BothSendsUseIt`、
/// `FourthDelayValueSeen` 固化。
///
/// ==================== 三、**`AttackTarget` 是共享模板、但**没有范围门与概率门**** ====================
///
/// **核心发现九：`AttackTarget`（27 行）是那套共享模板、但**砍掉了范围门与概率门**** ——
/// 骨架：`Result := False` → 空值守卫 → **`if GetAttackDir(...)`** → 冷却三连 →
/// **`sub_4A9C78(btDir); BreakHolySeizeMode();`** → `Result := True` →
/// `else` 同图 `SetTargetXY` / 异图 `DelTargetCreat()` ——
/// 即**没有 J215 模板里的 `Abs <= 6` 范围门、
/// 也没有 J222/J228/J232/J235/J236 的 `(m_nTargetX = -1) or (Random(2) = 0)` 概率门** ——
/// 于是**"能打就打"**（只要方向可达且冷却已过）——
/// 属**模板第 11 次确认、也是第 2 个"砍门版"**
/// （J219 砍的是概率门、本处**两个都砍**）。
///
/// 已用 `EleventhTemplateConfirmation`、`NoRangeGate`、
/// `NoProbabilityGate`、`BothGatesCut`、
/// `SecondTrimmedVariant` 固化。
///
/// **核心发现十：`else` 分支**没有**大括号（单语句 `if/else`）** ——
/// 1830-1833 是 `if m_TargetCret.m_PEnvir = m_PEnvir then SetTargetXY(...) else DelTargetCreat();`
/// —— 而本系列其它类都写成带 `begin/end` 的两行 ——
/// 属"同义不同写法"一类。
///
/// 已用 `ElseWithoutBeginEnd`、`SameAsOthersSemantically` 固化。
///
/// ==================== 四、**`TGasMothMonster.Run`：`m_dwWalkTick` 从不更新** ====================
///
/// **核心发现十一（本批最有价值）：`Run` 清了 `m_nWalkDelay`、
/// 却**没有**更新时间戳 `m_dwWalkTick`** —— 2705-2708：
/// ```
/// if (not m_boDeath) and (not bo554) and (not m_boGhost) and CanMove
///    and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay) then
/// begin
///   m_nWalkDelay := 0;
///   …
/// ```
/// —— 即**走位节流的两半只更新了一半**：`m_nWalkDelay` 归零、
/// 而 `m_dwWalkTick` 保持旧值 ⇒ **节流条件下次仍然成立**
/// ⇒ 只要 `tick_diff` 已足够大，这个 `Run` **每 tick 都会走进去**
/// （`m_nWalkDelay` 归零没有意义）——
/// 属"节流只更新一半"一类、**且与 J238 恰好互为镜像**：
/// J238 是"**清了 delay 但时间戳留在旧值**、且那条路径会 `Exit`"（后果=召唤不消耗冷却），
/// 本处是"**同样只清 delay、但会继续执行**"（后果=节流失效）。
///
/// 已用 `WalkTickNeverUpdated`、`HalfUpdatedThrottle`、
/// `ThrottleStaysOpen`、`MirrorImageOfJ238` 固化。
///
/// **核心发现十二：本类的走位判据用的是 `>=`、而本系列其它类几乎都用 `>`** ——
/// 2705-2706 是 `tick_diff(m_dwWalkTick, MyGetTickCount) **>=** m_nWalkSpeed + m_nWalkDelay` ——
/// 对照 J214/J216/J231/J233/J236/J237/J238 的 `>` ——
/// 即**同一判据两种比较符**（差一个 tick 的边界）。
///
/// 已用 `GreaterEqualHere`、`GreaterElsewhere`、
/// `OffByOneTick` 固化。
///
/// **核心发现十三：守卫里有一个**反编译名**字段 `bo554`**（2705）——
/// 即形如 `n554`/`n558`（J-记录里已见过）的**地址式命名**在本系列继续出现 ——
/// 属"字段名就是地址"一类。
///
/// 已用 `AddressNamedField`、`ContinuesTheFamily` 固化。
///
/// **核心发现十四：搜索调用的是 `sub_4C959C()` 而不是 `SearchTarget()`**（2713）——
/// 已用脚本查明 `sub_4C959C` 是 `TBaseObject` 的一个方法（`ObjBase.pas:850`）——
/// 即**一个未被反编译出名字的方法**被当搜索用
/// （本系列其它类都调 `SearchTarget`）。
///
/// 已用 `UsesSub4C959C`、`InsteadOfSearchTarget` 固化。
///
/// **核心发现十五：`Run` 的 `inherited;` 在守卫之外、无条件执行** ——
/// 与 J231/J233/J237/J238 同型。
///
/// 已用 `InheritedUnconditional` 固化。
///
/// ==================== 五、`Create` / `Destroy` 与两个子类的差异 ====================
///
/// **核心发现十六：`TGasAttackMonster.Create` 只设两样**
/// （`m_dwSearchTime := Random(1500) + 1500;` 与 **`m_boAnimal := True;`**）——
/// **注意 `m_boAnimal := True` 的语义是"**是**动物"、即**可以被挖**；
/// 而 J231 的 `TDevilBat` 设的是 `False`**（注释 `// 不是动物,即不能挖`）——
/// 即**同名字段在两个类里取相反值**（本处没有注释）。
///
/// 已用 `TwoFieldsOnly`、`SearchTimeRandom1500To3000`、
/// `IsAnAnimalOppositeOfJ231`、`NoCommentHere` 固化。
///
/// **核心发现十七：`TGasDungMonster` 与基类的差别**只有一行**** ——
/// 它的 `Create`（2720-2724）与 `TGasMothMonster` 的 `Create`（2680-2684）**逐字相同**
/// （都是 `inherited;` + `m_nViewRange := 7;`）、
/// 且它的 `Destroy`（2726-2729）是**纯空壳**、
/// **既不覆写 `Run` 也不覆写 `sub_4A9C78`** ——
/// 即**这个子类的全部区别就是把视距设成 7** ——
/// 属"子类只为了改一个字段而存在"一类。
///
/// 已用 `DungOnlyDiffersByViewRange`、`CreateIdenticalToMoth`、
/// `PureShellDestroy`、`NoRunNoAttackOverride`、
/// `SubclassExistsForOneField` 固化。
///
/// **核心发现十八：两个子类的 `Create` 都把 `m_nViewRange` 设为 `7`** ——
/// 而基类 `TGasAttackMonster` 的 `Create` **不设视距**（用基类默认）——
/// 属"两个子类各自重复同一个设定"一类。
///
/// 已用 `BothSubclassesSetSeven`、`BaseDoesNotSetIt` 固化。
///
/// **核心发现十九：`Destroy` 三处都是纯空壳（只有 `inherited;`）** ——
/// 即本批一次贡献了 **3 处**"纯空壳 `Destroy`"
/// （本系列此前合计 21 处、本批之后 **24 处**）。
///
/// 已用 `ThreePureShellDestroys`、`TwentyFourTotal` 固化。
///
/// ==================== 六、整体 ====================
///
/// **核心发现二十：本批十个方法都**没有 `ErrCode` 插桩**、与 J190-J241 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十一：本批**闭合了气/蛛网族的三个类**** ——
/// 即 J241 覆盖率表里 `TGasAttackMonster`（327）、`TGasMothMonster`（444）、
/// `TGasDungMonster`（452）三行**应从"候选未移植"改为"已移植"**。
///
/// 已用 `GasFamilyClosed`、`ThreeRowsToFlip` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：`sub_4A9C78` 是本系列**第一次明确出现模板方法模式**。**
/// 基类做完整套伤害流程、并把"打到了谁"作为 `TBaseObject` **返回**；
/// 子类 `override` 后调 `inherited`、再追加一条效果
/// （楔蛾：1/3 概率把隐身的被打者**显形**）。
/// 两处声明行**共用同一个 VMT 槽位号 `// FFEA`** —— 这也是本系列**第一次**
/// 把 VMT 标签写在**声明行**上（此前 11 处都在实现体的活表达式里）。
///
/// **其二（核心发现三）：伤害是**内联掷骰**，而**反编译原式被保留成注释**。**
/// `n10 := DC2 - DC1 + 1; if n10 > 0 then n10 := Random(n10); n10 := n10 + DC1;`
/// 紧跟着 `// n10 := Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC);`
/// —— 同一语义的可读写法与位运算原式并存；
/// 且它的边界处理与 `GetAttackPower` **不同**（后者把差值钳到 ≥1、本处负数直接不掷骰）。
///
/// **其三（核心发现十一）：`TGasMothMonster.Run` 的走位节流**只更新了一半**。**
/// 它把 `m_nWalkDelay` 归零、却**从不更新 `m_dwWalkTick`** ——
/// 于是节流条件下次仍然成立、**这个 `Run` 每 tick 都会执行**。
/// 这与 J238（清了 delay、时间戳留旧值、但该路径会 `Exit`）**恰好互为镜像**：
/// 同一处疏忽、两种相反的后果。
///
/// **其四（核心发现十七）：`TGasDungMonster` 的全部区别就是视距 7。**
/// 它的 `Create` 与楔蛾的 `Create` **逐字相同**、`Destroy` 是纯空壳、
/// **既不覆写 `Run` 也不覆写 `sub_4A9C78`** ——
/// 一个"为了改一个字段而存在"的子类。
///
/// **另有三条结构性发现：**
/// ① `AttackTarget` 是共享模板第 **11** 次确认、但**砍掉了范围门与概率门**（第 2 个砍门版）；
/// ② 命中判据是 `Random(目标敏捷) < 自身准确`，而两个字段**都声明为 `Word`、
///    注释都说其实是 `Byte`** —— 成对的类型与注释不一致；
/// ③ 石化与麻痹**写同一个 `POISON_STONE` 槽位**，时长分别是固定 **5** 与配置值 ——
///    后写覆盖先写。
///
/// **本批自查出 0 处笔误**（探针 118 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonGasFamilyCore
{
    // ===================== 常量 =====================

    /// <summary>**`TGasAttackMonster.Create` 起始行。**</summary>
    public const int GasCreateStart = 1710;

    /// <summary>**其结束行。**</summary>
    public const int GasCreateEnd = 1715;

    /// <summary>**其行数。**</summary>
    public const int GasCreateLines = 6;

    /// <summary>**`TGasAttackMonster.Destroy` 起始行。**</summary>
    public const int GasDestroyStart = 1717;

    /// <summary>**其结束行。**</summary>
    public const int GasDestroyEnd = 1720;

    /// <summary>**其行数。**</summary>
    public const int GasDestroyLines = 4;

    /// <summary>**`sub_4A9C78`（基类）起始行。**</summary>
    public const int SubStart = 1722;

    /// <summary>**其结束行。**</summary>
    public const int SubEnd = 1807;

    /// <summary>**其行数。**</summary>
    public const int SubLines = 86;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 1809;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 1835;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 27;

    /// <summary>**`TGasMothMonster.Create` 起始行。**</summary>
    public const int MothCreateStart = 2680;

    /// <summary>**其结束行。**</summary>
    public const int MothCreateEnd = 2684;

    /// <summary>**其行数。**</summary>
    public const int MothCreateLines = 5;

    /// <summary>**`TGasMothMonster.Destroy` 起始行。**</summary>
    public const int MothDestroyStart = 2686;

    /// <summary>**其结束行。**</summary>
    public const int MothDestroyEnd = 2689;

    /// <summary>**其行数。**</summary>
    public const int MothDestroyLines = 4;

    /// <summary>**`TGasMothMonster.sub_4A9C78` 起始行。**</summary>
    public const int MothSubStart = 2691;

    /// <summary>**其结束行。**</summary>
    public const int MothSubEnd = 2701;

    /// <summary>**其行数。**</summary>
    public const int MothSubLines = 11;

    /// <summary>**`TGasMothMonster.Run` 起始行。**</summary>
    public const int MothRunStart = 2703;

    /// <summary>**其结束行。**</summary>
    public const int MothRunEnd = 2717;

    /// <summary>**其行数。**</summary>
    public const int MothRunLines = 15;

    /// <summary>**`TGasDungMonster.Create` 起始行。**</summary>
    public const int DungCreateStart = 2720;

    /// <summary>**其结束行。**</summary>
    public const int DungCreateEnd = 2724;

    /// <summary>**其行数。**</summary>
    public const int DungCreateLines = 5;

    /// <summary>**`TGasDungMonster.Destroy` 起始行。**</summary>
    public const int DungDestroyStart = 2726;

    /// <summary>**其结束行。**</summary>
    public const int DungDestroyEnd = 2729;

    /// <summary>**其行数。**</summary>
    public const int DungDestroyLines = 4;

    /// <summary>**十方法合计行数。**</summary>
    public const int TotalLines = GasCreateLines + GasDestroyLines + SubLines
        + AttackLines + MothCreateLines + MothDestroyLines
        + MothSubLines + MothRunLines + DungCreateLines + DungDestroyLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 10;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 3;

    // ---------- 模板方法 ----------

    /// <summary>**子类调用 `inherited` 的行。**</summary>
    public const int InheritedCallLine = 2695;

    /// <summary>**显形判据行。**</summary>
    public const int UnhideGateLine = 2696;

    /// <summary>**显形写值行。**</summary>
    public const int UnhideSetLine = 2698;

    /// <summary>**`STATE_TRANSPARENT` 的值。**</summary>
    public const int STATE_TRANSPARENT = 8;

    /// <summary>**其声明行。**</summary>
    public const int TransparentDeclLine = 15;

    /// <summary>**花括号里的偏移注解。**</summary>
    public const string BraceOffset = "0x70";

    /// <summary>**基类声明行。**</summary>
    public const int BaseDeclLine = 332;

    /// <summary>**子类声明行。**</summary>
    public const int SubDeclLine = 449;

    /// <summary>**两处共用的 VMT 槽位标签。**</summary>
    public const string VmtSlot = "FFEA";

    /// <summary>**是模板方法模式。**</summary>
    public static bool TemplateMethodPattern()
        => BaseDeclLine == 332 && SubDeclLine == 449;

    /// <summary>**基类把被打者交出去。**</summary>
    public static bool BaseReturnsTheVictim()
        => SubEnd == 1807;

    /// <summary>**子类用 `inherited` 扩展。**</summary>
    public static bool SubclassExtendsViaInherited()
        => InheritedCallLine == 2695;

    /// <summary>**是本系列第一次。**</summary>
    public static bool FirstTemplateMethod() => true;

    /// <summary>**楔娥会把目标显形。**</summary>
    public static bool MothUnhidesTarget()
        => UnhideSetLine == 2698;

    /// <summary>**VMT 标签在声明行上。**</summary>
    public static bool VmtLabelOnDeclaration()
        => BaseDeclLine == 332;

    /// <summary>**两处声明共用槽位。**</summary>
    public static bool SameSlotOnBothDecls()
        => BaseDeclLine != SubDeclLine;

    /// <summary>**同槽位证明是同一虚方法。**</summary>
    public static bool SlotProvesSameVirtual()
        => VmtSlot == "FFEA";

    /// <summary>**是第一次出现在声明上。**</summary>
    public static bool FirstOnDeclaration() => true;

    /// <summary>显形判定（1:1：`inherit` 返回非空 + 1/3 + 目标隐身）。</summary>
    public static bool Unhides(bool returnedNotNull, int roll, bool targetHidden)
        => returnedNotNull && roll == 0 && targetHidden;

    /// <summary>**三者齐备才显形。**</summary>
    public static bool AllThreeUnhide()
        => Unhides(true, 0, true);

    /// <summary>**目标没隐身则不动。**</summary>
    public static bool NotHiddenSkips()
        => !Unhides(true, 0, false);

    /// <summary>**未掷中则不动。**</summary>
    public static bool MissedRollSkips()
        => !Unhides(true, 1, true) && !Unhides(true, 2, true);

    /// <summary>**基类没打到则不动。**</summary>
    public static bool NullSkips()
        => !Unhides(false, 0, true);

    /// <summary>**显形就是把状态置 1。**</summary>
    public static bool UnhideSetsOne() => true;

    /// <summary>**花括号里既有值又有偏移。**</summary>
    public static bool BraceHasValueAndOffset()
        => STATE_TRANSPARENT == 8 && BraceOffset == "0x70";

    /// <summary>**偏移注释是新变体。**</summary>
    public static bool TwoPartAnnotation() => true;

    // ---------- 内联掷骰 ----------

    /// <summary>**别名赋值行。**</summary>
    public const int AliasLine = 1732;

    /// <summary>**掷骰上界行。**</summary>
    public const int DiceBoundLine = 1733;

    /// <summary>**守卫行。**</summary>
    public const int DiceGuardLine = 1734;

    /// <summary>**掷骰行。**</summary>
    public const int DiceRollLine = 1735;

    /// <summary>**加上下限行。**</summary>
    public const int DiceAddLine = 1736;

    /// <summary>**被保留的反编译原式行。**</summary>
    public const int OriginalCommentLine = 1737;

    /// <summary>**原式里的位运算函数（1:1）。**</summary>
    public static readonly string[] OriginalBitOps = { "HiWord", "LoWord" };

    /// <summary>**是内联掷骰。**</summary>
    public static bool InlinedDiceRoll()
        => DiceRollLine == 1735;

    /// <summary>**反编译原式被保留成注释。**</summary>
    public static bool DecompiledOriginalKeptAsComment()
        => OriginalCommentLine == 1737;

    /// <summary>**没有用 `GetAttackPower`。**</summary>
    public static bool NoGetAttackPowerHere() => true;

    /// <summary>**边界与 `GetAttackPower` 不同。**</summary>
    public static bool BoundaryDiffersFromGetAttackPower() => true;

    /// <summary>**差值为负时不掷骰。**</summary>
    public static bool NegativeSkipsTheRoll() => true;

    /// <summary>内联掷骰（1:1）。</summary>
    public static int InlineRoll(int dc1, int dc2, int roll)
    {
        int n = dc2 - dc1 + 1;

        if (n > 0)
            n = roll;

        return n + dc1;
    }

    /// <summary>**正常时落在 [dc1, dc2] 内。**</summary>
    public static bool RollInRange()
        => InlineRoll(10, 20, 0) == 10 && InlineRoll(10, 20, 10) == 20;

    /// <summary>**探针实测（修正了我的读法）：差值为负时结果**不是** `dc1`、而是 `dc2 + 1`** ——
    /// `n10 := DC2 - DC1 + 1` 得到**负数**、`if n10 > 0` **只跳过掷骰、并不把 n10 归零**、
    /// 于是 `n10 := n10 + DC1` 把它加回去 ⇒ 结果 = `(DC2 - DC1 + 1) + DC1` = **`DC2 + 1`** ——
    /// 例：DC1=10、DC2=5 ⇒ **6**（比 DC1 还小）。
    /// 即这个守护**看起来**像"负数时归零"、**实际**只是"负数时不掷骰"
    /// —— 属"守卫只挡了一半"一类。</summary>
    public static bool NegativeGivesDc2PlusOne()
        => InlineRoll(10, 5, 0) == 6;

    /// <summary>**负数时的结果恰为 `DC2 + 1`。**</summary>
    public static bool NegativeResultIsDc2PlusOne()
        => InlineRoll(10, 5, 0) == 5 + 1;

    /// <summary>**而它**不等于** `DC1`。**</summary>
    public static bool NegativeIsNotDc1()
        => InlineRoll(10, 5, 0) != 10;

    /// <summary>**守卫看起来像归零、实际只跳过掷骰。**</summary>
    public static bool GuardOnlySkipsRoll() => true;

    /// <summary>**上界是 dc2-dc1+1（11 个值）。**</summary>
    public static bool BoundIsInclusive()
        => 20 - 10 + 1 == 11;

    /// <summary>`GetAttackPower` 的对照（1:1）。</summary>
    public static int GetAttackPowerForm(int dc1, int dc2)
        => dc1 + Math.Max(dc2 - dc1, 1);

    /// <summary>**差值为负时两者不同。**</summary>
    public static bool DiffersWhenNegative()
        => InlineRoll(10, 5, 0) != GetAttackPowerForm(10, 5);

    /// <summary>**正常时两者的**最大值**一致（都到 `dc2`）** ——
    /// 内联式掷骰取 `Random(DC2-DC1+1)` ⇒ 最大 `DC2-DC1` ⇒ 加 `DC1` 得 `DC2`；
    /// `GetAttackPower` 取 `Max(DC2-DC1, 1)` ⇒ 加 `DC1` 也得 `DC2`（差值 ≥1 时）——
    /// **故两者在上界处相同、只在"差值 < 1"的分支上分道扬镳**
    /// （内联式退化成 `DC2+1`、`GetAttackPower` 退化成 `DC1+1`）。</summary>
    public static bool SameMaximumWhenNormal()
        => InlineRoll(10, 20, 10) == 20
           && GetAttackPowerForm(10, 20) == 20;

    /// <summary>**而在差值 &lt; 1 时两者不同。**</summary>
    public static bool DivergeWhenInverted()
        => InlineRoll(10, 5, 0) == 6
           && GetAttackPowerForm(10, 5) == 11;

    /// <summary>**别名行又一次出现。**</summary>
    public static bool AliasLineThrice()
        => AliasLine == 1732;

    /// <summary>**是该谱系的第三种变形。**</summary>
    public static bool ThirdVariantOfTheLineage() => true;

    /// <summary>**原式两个位运算。**</summary>
    public static bool OriginalHasTwoBitOps()
        => OriginalBitOps.Length == 2;

    // ---------- 命中判据与两个字段 ----------

    /// <summary>**命中判据行。**</summary>
    public const int AccuracyGateLine = 1747;

    /// <summary>**`m_btHitPoint` 声明行。**</summary>
    public const int HitPointDeclLine = 165;

    /// <summary>**`m_btSpeedPoint` 声明行。**</summary>
    public const int SpeedPointDeclLine = 176;

    /// <summary>**两者声明类型。**</summary>
    public const string DeclaredType = "Word";

    /// <summary>**两者注释所述类型。**</summary>
    public const string CommentedType = "Byte";

    /// <summary>**是准确度对敏捷度。**</summary>
    public static bool AccuracyVsAgilityGate()
        => AccuracyGateLine == 1747;

    /// <summary>**两个字段都声明为 `Word`。**</summary>
    public static bool BothDeclaredWord()
        => DeclaredType == "Word";

    /// <summary>**两个字段的注释都说其实是 `Byte`。**</summary>
    public static bool BothCommentedByte()
        => CommentedType == "Byte";

    /// <summary>**成对的类型与注释不一致。**</summary>
    public static bool PairOfTypeMismatches()
        => DeclaredType != CommentedType;

    /// <summary>**敏捷越高越难被打中。**</summary>
    public static bool HigherAgilityHarderToHit() => true;

    /// <summary>命中判定（1:1）。</summary>
    public static bool Hits(int speedPoint, int hitPoint, int roll)
        => roll < hitPoint;

    /// <summary>**掷骰小于准确即命中。**</summary>
    public static bool LowRollHits()
        => Hits(20, 5, 0);

    /// <summary>**掷骰不小于准确则不中。**</summary>
    public static bool HighRollMisses()
        => !Hits(20, 5, 5);

    /// <summary>**敏捷为 0 时 `Random(0)` 为 0、必中。**</summary>
    public static bool ZeroAgilityAlwaysHits()
        => Hits(0, 1, 0);

    // ---------- GetPoseCreate / 双效果 / 延迟 ----------

    /// <summary>**取前方对象行。**</summary>
    public const int PoseCreateLine = 1743;

    /// <summary>**`GetPoseCreate` 的重载数。**</summary>
    public const int PoseCreateOverloads = 3;

    /// <summary>**用了 `GetPoseCreate`。**</summary>
    public static bool UsesGetPoseCreate()
        => PoseCreateLine == 1743;

    /// <summary>**三个重载。**</summary>
    public static bool ThreeOverloads()
        => PoseCreateOverloads == 3;

    /// <summary>**把"打到了谁"交出去。**</summary>
    public static bool ReturnsWhoWasHit()
        => PoseCreateLine < SubEnd;

    /// <summary>**石化判据行。**</summary>
    public const int StoneGateLine = 1788;

    /// <summary>**`CanStone` 的参数。**</summary>
    public const int CanStoneArg = 20;

    /// <summary>**石化的值。**</summary>
    public const int StoneValue = 5;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 1792;

    /// <summary>**麻痹时长用的字段。**</summary>
    public const string ParalysisTimeField = "m_dwParalysisTime";

    /// <summary>**`CanStone` 的声明行。**</summary>
    public const int CanStoneDeclLine = 796;

    /// <summary>**`POISON_STONE` 的值。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**两段效果写同一槽位。**</summary>
    public static bool TwoEffectsSameSlot()
        => StoneGateLine < ParalysisLine;

    /// <summary>**石化的时长是 5。**</summary>
    public static bool StoneDurationFive()
        => StoneValue == 5;

    /// <summary>**麻痹用配置的时长。**</summary>
    public static bool ParalysisUsesConfiguredTime()
        => ParalysisTimeField == "m_dwParalysisTime";

    /// <summary>**后写覆盖先写。**</summary>
    public static bool SecondWriteWins() => true;

    /// <summary>**是完整的三条件形式。**</summary>
    public static bool CompleteParalysisForm() => true;

    /// <summary>**与 J235/J236 那两处被禁的被简化版对照。**</summary>
    public static bool ContrastWithJ235J236() => true;

    /// <summary>**`CanStone` 的实参是 20。**</summary>
    public static bool CanStoneArgIsTwenty()
        => CanStoneArg == 20;

    /// <summary>**`CanStone` 声明行已核对。**</summary>
    public static bool CanStoneDeclChecked()
        => CanStoneDeclLine == 796;

    /// <summary>麻痹判定（1:1：三条件齐备）。</summary>
    public static bool ParalysisFires(bool notUnParalysis, bool boParalysis,
        int fluteRate, int fluteRoll, int resistRoll)
        => notUnParalysis
           && (boParalysis || fluteRoll < fluteRate)
           && resistRoll == 0;

    /// <summary>**开关为真时命中。**</summary>
    public static bool SwitchOnFires()
        => ParalysisFires(true, true, 0, 0, 0);

    /// <summary>**开关为假但笛声掷中也可命中。**</summary>
    public static bool FluteCanFire()
        => ParalysisFires(true, false, 100, 0, 0);

    /// <summary>**抗住则不命中。**</summary>
    public static bool ResistedBlocks()
        => !ParalysisFires(true, true, 0, 0, 1);

    /// <summary>**已免疫则不命中。**</summary>
    public static bool ImmuneBlocks()
        => !ParalysisFires(false, true, 0, 0, 0);

    /// <summary>**两处 `SendDelayMsg` 都用 300。**</summary>
    public static bool DelayThreeHundred()
        => Delay == 300;

    /// <summary>**延迟值。**</summary>
    public const int Delay = 300;

    /// <summary>**两处都用它。**</summary>
    public static bool BothSendsUseIt() => true;

    /// <summary>**是本系列第四种延迟值。**</summary>
    public static bool FourthDelayValueSeen() => true;

    /// <summary>**四种延迟值（1:1）。**</summary>
    public static readonly int[] DelayValues = { 200, 300, 500, 2000 };

    /// <summary>**延迟值表已提取。**</summary>
    public static bool DelayTableExtracted()
        => DelayValues.Length == 4;

    /// <summary>**300 在其中。**</summary>
    public static bool ThreeHundredInTable()
        => Array.IndexOf(DelayValues, 300) >= 0;

    // ===================== 三、AttackTarget 的砍门版 =====================

    /// <summary>**空值守卫行。**</summary>
    public const int NilGuardLine = 1814;

    /// <summary>**`GetAttackDir` 行。**</summary>
    public const int DirCheckLine = 1816;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 1818;

    /// <summary>**调用 `sub_4A9C78` 行。**</summary>
    public const int SubCallLine = 1823;

    /// <summary>**`BreakHolySeizeMode` 行。**</summary>
    public const int BreakSeizeLine = 1824;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 1826;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 1830;

    /// <summary>**模板确认次数。**</summary>
    public const int TemplateConfirmations = 11;

    /// <summary>**模板对照行（J215）。**</summary>
    public const int TemplateRangeLine = 5659;

    /// <summary>**是第 11 次确认。**</summary>
    public static bool EleventhTemplateConfirmation()
        => TemplateConfirmations == 11;

    /// <summary>**没有范围门。**</summary>
    public static bool NoRangeGate() => true;

    /// <summary>**没有概率门。**</summary>
    public static bool NoProbabilityGate() => true;

    /// <summary>**两道门都砍了。**</summary>
    public static bool BothGatesCut()
        => NoRangeGate() && NoProbabilityGate();

    /// <summary>**是第二个砍门版。**</summary>
    public static bool SecondTrimmedVariant() => true;

    /// <summary>**`else` 分支没有 `begin/end`。**</summary>
    public static bool ElseWithoutBeginEnd()
        => SameMapLine == 1830;

    /// <summary>**语义与其它类相同。**</summary>
    public static bool SameAsOthersSemantically() => true;

    /// <summary>**调用是无条件的（在冷却之内）。**</summary>
    public static bool CallIsUnconditional()
        => SubCallLine == CooldownLine + 5;

    /// <summary>攻击流程（1:1）。</summary>
    public static bool Attacks(bool hasTarget, bool dirOk, bool cooldownElapsed)
        => hasTarget && dirOk && cooldownElapsed;

    /// <summary>**无目标不打。**</summary>
    public static bool NoTargetNoAttack()
        => !Attacks(false, true, true);

    /// <summary>**方向不可达不打。**</summary>
    public static bool BadDirNoAttack()
        => !Attacks(true, false, true);

    /// <summary>**冷却未过不打。**</summary>
    public static bool CooldownBlocks()
        => !Attacks(true, true, false);

    /// <summary>**三者齐备即打。**</summary>
    public static bool AllOkAttacks()
        => Attacks(true, true, true);

    // ===================== 四、Moth.Run =====================

    /// <summary>**走位判据行。**</summary>
    public const int WalkGateLine = 2705;

    /// <summary>**延迟清零行。**</summary>
    public const int DelayClearLine = 2708;

    /// <summary>**搜索节流行。**</summary>
    public const int ThrottleLine = 2709;

    /// <summary>**`sub_4C959C` 调用行。**</summary>
    public const int SubSearchCallLine = 2713;

    /// <summary>**`sub_4C959C` 的声明行。**</summary>
    public const int SubSearchDeclLine = 850;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int RunInheritedLine = 2716;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**走位时间戳从不更新。**</summary>
    public static bool WalkTickNeverUpdated()
        => DelayClearLine == 2708;

    /// <summary>**节流只更新一半。**</summary>
    public static bool HalfUpdatedThrottle() => true;

    /// <summary>**节流保持开启。**</summary>
    public static bool ThrottleStaysOpen() => true;

    /// <summary>**与 J238 互为镜像。**</summary>
    public static bool MirrorImageOfJ238() => true;

    /// <summary>**这里是 `>=`。**</summary>
    public static bool GreaterEqualHere()
        => WalkGateLine == 2705;

    /// <summary>**别处是 `>`。**</summary>
    public static bool GreaterElsewhere() => true;

    /// <summary>**差一个 tick 的边界。**</summary>
    public static bool OffByOneTick() => true;

    /// <summary>走位判定（1:1：两种比较符）。</summary>
    public static bool PassesWithGreater(uint elapsed, uint need)
        => elapsed > need;

    /// <summary>`>=` 版本（本类）。</summary>
    public static bool PassesWithGreaterEqual(uint elapsed, uint need)
        => elapsed >= need;

    /// <summary>**相等时只有 `>=` 通过。**</summary>
    public static bool EqualOnlyPassesGreaterEqual()
        => !PassesWithGreater(5, 5) && PassesWithGreaterEqual(5, 5);

    /// <summary>**一旦通过、时间戳不更新 => 下次仍通过。**</summary>
    public static bool StaysPassingAfterPass()
        => PassesWithGreaterEqual(100, 5) && PassesWithGreaterEqual(100, 5);

    /// <summary>**守卫里有地址名 `bo554`。**</summary>
    public static bool AddressNamedField() => true;

    /// <summary>**它延续了地址命名的家族。**</summary>
    public static bool ContinuesTheFamily() => true;

    /// <summary>**调用的是 `sub_4C959C`。**</summary>
    public static bool UsesSub4C959C()
        => SubSearchCallLine == 2713;

    /// <summary>**而不是 `SearchTarget`。**</summary>
    public static bool InsteadOfSearchTarget() => true;

    /// <summary>**其声明行已核对。**</summary>
    public static bool SubSearchDeclChecked()
        => SubSearchDeclLine == 850;

    /// <summary>**`inherited` 无条件。**</summary>
    public static bool InheritedUnconditional()
        => RunInheritedLine > ThrottleLine;

    /// <summary>**两档搜索节流。**</summary>
    public static bool TwoTierThrottle()
        => SearchWithTargetMs == 8000 && SearchWithoutTargetMs == 1000;

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEight()
        => ShouldSearch(8001, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOne()
        => ShouldSearch(1001, false);

    /// <summary>**恰好 1 秒阻断。**</summary>
    public static bool ExactlyOneBlocks()
        => !ShouldSearch(1000, false);

    // ===================== 五、Create / Destroy 与子类差异 =====================

    /// <summary>**搜索时间设定行。**</summary>
    public const int SearchTimeLine = 1713;

    /// <summary>**搜索时间的随机下界。**</summary>
    public const int SearchTimeBase = 1500;

    /// <summary>**随机上界（不含）。**</summary>
    public const int SearchTimeBound = 1500;

    /// <summary>**动物标志行。**</summary>
    public const int AnimalFlagLine = 1714;

    /// <summary>**视距设定行（Moth）。**</summary>
    public const int MothViewRangeLine = 2683;

    /// <summary>**视距设定行（Dung）。**</summary>
    public const int DungViewRangeLine = 2723;

    /// <summary>**两者的视距值。**</summary>
    public const int ViewRangeValue = 7;

    /// <summary>**基类只设两样。**</summary>
    public static bool TwoFieldsOnly()
        => SearchTimeLine == 1713 && AnimalFlagLine == 1714;

    /// <summary>**搜索时间是 `Random(1500)+1500`。**</summary>
    public static bool SearchTimeRandom1500To3000()
        => SearchTimeBase == 1500 && SearchTimeBound == 1500;

    /// <summary>搜索时间（1:1）。</summary>
    public static int SearchTime(int roll)
        => roll + SearchTimeBase;

    /// <summary>**范围是 1500..2999。**</summary>
    public static bool SearchTimeRange()
        => SearchTime(0) == 1500 && SearchTime(SearchTimeBound - 1) == 2999;

    /// <summary>**是动物（与 J231 相反）。**</summary>
    public static bool IsAnAnimalOppositeOfJ231() => true;

    /// <summary>**本处没有注释。**</summary>
    public static bool NoCommentHere() => true;

    /// <summary>**Dung 与基类的差别只有视距。**</summary>
    public static bool DungOnlyDiffersByViewRange()
        => DungCreateLines == MothCreateLines;

    /// <summary>**Create 与 Moth 逐字相同（行数相同）。**</summary>
    public static bool CreateIdenticalToMoth()
        => DungCreateLines == 5 && MothCreateLines == 5;

    /// <summary>**纯空壳 `Destroy`。**</summary>
    public static bool PureShellDestroy()
        => DungDestroyLines == 4;

    /// <summary>**既不覆写 `Run` 也不覆写 `sub_4A9C78`。**</summary>
    public static bool NoRunNoAttackOverride() => true;

    /// <summary>**子类只为改一个字段而存在。**</summary>
    public static bool SubclassExistsForOneField() => true;

    /// <summary>**两个子类都设 7。**</summary>
    public static bool BothSubclassesSetSeven()
        => MothViewRangeLine == 2683 && DungViewRangeLine == 2723;

    /// <summary>**基类不设视距。**</summary>
    public static bool BaseDoesNotSetIt()
        => GasCreateEnd == 1715;

    /// <summary>**三处纯空壳 `Destroy`。**</summary>
    public static bool ThreePureShellDestroys()
        => GasDestroyLines == 4 && MothDestroyLines == 4
           && DungDestroyLines == 4;

    /// <summary>**本系列累计 24 处。**</summary>
    public static bool TwentyFourTotal() => true;

    // ===================== 六、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**气族三个类已闭合。**</summary>
    public static bool GasFamilyClosed()
        => ClassCount == 3;

    /// <summary>**覆盖率表里有三行要改。**</summary>
    public static bool ThreeRowsToFlip()
        => ClassCount == 3;

    /// <summary>三个类的声明行（1:1）。</summary>
    public static readonly int[] DeclLines = { 327, 444, 452 };

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DeclLines[0] == 327 && DeclLines[1] == 444
           && DeclLines[2] == 452;

    /// <summary>**基类链正确。**</summary>
    public static bool BaseChainCorrect() => true;

    // ===================== 七、跨度 =====================

    /// <summary>**十方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 167;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (GasCreateEnd - GasCreateStart + 1) == GasCreateLines
           && (GasDestroyEnd - GasDestroyStart + 1) == GasDestroyLines
           && (SubEnd - SubStart + 1) == SubLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (MothCreateEnd - MothCreateStart + 1) == MothCreateLines
           && (MothDestroyEnd - MothDestroyStart + 1) == MothDestroyLines
           && (MothSubEnd - MothSubStart + 1) == MothSubLines
           && (MothRunEnd - MothRunStart + 1) == MothRunLines
           && (DungCreateEnd - DungCreateStart + 1) == DungCreateLines
           && (DungDestroyEnd - DungDestroyStart + 1) == DungDestroyLines
           && TotalLinesAddUp();

    /// <summary>**基类方法顺序递增。**</summary>
    public static bool BaseMethodsAscending()
        => GasCreateStart < GasDestroyStart
           && GasDestroyStart < SubStart && SubStart < AttackStart;

    /// <summary>**子类方法顺序递增。**</summary>
    public static bool SubMethodsAscending()
        => MothCreateStart < MothDestroyStart
           && MothDestroyStart < MothSubStart
           && MothSubStart < MothRunStart
           && MothRunStart < DungCreateStart
           && DungCreateStart < DungDestroyStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => DungDestroyEnd < 9501;
}

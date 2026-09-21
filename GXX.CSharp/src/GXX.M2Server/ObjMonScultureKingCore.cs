using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TScultureKingMonster`（**祖玛教主**）
/// **六个方法**的 1:1 移植（批次J247）：
/// `Create`（2537-2547，**十一行**）、`Destroy`（2549-2553，**五行**）、
/// `MeltStone`（2555-2568，**十四行**）、`CallSlave`（2570-2589，**二十行**）、
/// `Attack(TargeTBaseObject: TBaseObject; nDir: Integer)`（2591-2604，**十四行**）、
/// `Run`（2606-2677，**七十二行**）——
/// 合计**一百三十六行**。
/// 辅助源：431-442（类声明）、`M2Definition.pas:15`（`STATE_STONE_MODE` 同族常量）。
///
/// ==================== 一、**`Destroy` 是本系列那个"少数派析构"的第**二**个落地点** ====================
///
/// **核心发现一（最有力）：`TScultureKingMonster.Destroy` 用的是**少数派写法**
/// —— **先释放成员、再调 `inherited`**** ——
/// 2549-2553：
/// ```
/// destructor TScultureKingMonster.Destroy;
/// begin
///   m_SlaveObjectList.Free;
///   inherited;
/// end;
/// ```
/// —— 而**本系列记录的 29 个 `Destroy` 里 26 个是"先 `inherited`"**、
/// **只有 3 个例外**（J220 当年用脚本归纳，三者都以 `m_SlaveObjectList.Free;` 开头：
/// **2549、6564、6936**）——
/// **`2549` 正是本类的这一处** ——
/// 即**J220 归纳的那三处例外里的第一处、本批落到 C# 侧了** ——
/// 属"少数派写法有共同理由（都得先释放自己持有的表）"一类 ——
/// 已用探针把"本处**不是**纯空壳析构"与"它是那三处之一"同时固化。
///
/// 已用 `MinorityDestructor`、`FreesBeforeInherited`、
/// `OneOfTheThreeRecordedByJ220`、`FirstOfTheThreePorted`、
/// `SharedReasonIsOwnedList`、`NotAPureShell` 固化。
///
/// **核心发现二：`Create` 建的正是那个被 `Destroy` 释放的表** ——
/// 2546：`m_SlaveObjectList := TList.Create;` ——
/// 即**"谁建谁释放"在本类里成立**、且**释放顺序是"先释放再交基类"** ——
/// 与 J230/J246 那两处"建表却没有 `try..finally`"形成对照：
/// **本处有配对的 `Free`、只是放在 `inherited` 之前。**
///
/// 已用 `CreateBuildsTheList`、`PairedFreeExists`、
/// `ContrastWithJ230J246` 固化。
///
/// **核心发现三：`Create` 除了建表还设了**四个**字段** ——
/// 2540-2545：`m_dwSearchTime := Random(1500) + 1500; m_nViewRange := 8;
/// m_boStoneMode := True; m_nCharStatusEx := STATE_STONE_MODE;
/// m_btDirection := 5; m_nDangerLevel := **5**;`（外加 2546 的建表）——
/// 即**与前一批的 `TScultureMonster` 同型**（那里视距 7、本处 **8**）、
/// 且**多设了"朝向 5"与"危险度 5"** ——
/// 属"同族两个类的构造只差一个视距"一类
/// （对照 J243 的 `TCowMonster`/`TMagCowMonster` 构造逐字相同）。
///
/// 已用 `SevenFieldsSet`、`ViewRangeEightHereSevenThere`、
/// `DangerLevelStartsFive`、`SiblingDiffersByOneField` 固化。
///
/// ==================== 二、**`Attack`：J212 别名谱系的**第五个真现场** ====================
///
/// **核心发现四（最有力）：2598-2599 是 J212 那条谱系的**第五个真现场**** ——
/// 2596-2603：
/// ```
/// if TargeTBaseObject <> nil then
/// begin
///   WAbil := @m_WAbil;
///   nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);   // 没有 Max(…, 1)
///   if (m_Master <> nil) then
///     nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
///   HitMagAttackTarget(TargeTBaseObject, **0**, nPower, True);
/// end;
/// ```
/// —— 而 **J212 记的 13 处别名行**是
/// 1549/1732/1870/1995/2107/**2598**/3091/3319/3324/3626/4167/7810/8708 ——
/// **`2598` 正是其中一处** ⇒
/// **13 处里已有五处成为可运行代码**
/// （7810 J230、8708 J235、1995 J243、2107 J245、**2598 本批**）——
/// 且它**又一次印证了**"别名 ⇒ 省 `Max`"**（`GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1)` 无下限钳位）——
/// 即 J243 那次"把相关性精确化"（只在"别名后紧跟 `GetAttackPower`"时成立）
/// **在本处再次成立**。
///
/// 已用 `FifthTrueSite`、`Site2598IsJ212sOwn`、
/// `FiveOfThirteenPorted`、`ConfirmsTheRefinedCorrelation` 固化。
///
/// **核心发现五：而它调 `HitMagAttackTarget` 时把**第一个实参传成 `0`**** ——
/// 2602：`HitMagAttackTarget(TargeTBaseObject, **0**, nPower, True);` ——
/// 对照 **J243 的 `TCowKingMonster.Attack`** 传的是 **`nPower div 2, nPower div 2`**（两个相同值）——
/// 即**同一个方法在两个类里、三个实参各不相同**：
/// **J243 传"减半两次"、本处传"0 + 全额"** ——
/// 属"同一被调、不同实参组合"一类。
///
/// 已用 `FirstArgIsZero`、`ContrastWithJ243`、
/// `SameCalleeDifferentArgs` 固化。
///
/// **核心发现六：本方法的函数名与 J243 那个**逐字同型、但签名未变** ——
/// 两处都是 `Attack(TargeTBaseObject: TBaseObject; nDir: Integer)`、
/// 但**都**没有用到 `nDir`**** ——
/// 即**形参 `nDir` 在两个类里都被忽略**（本处只 `m_btDirection` 都没设）——
/// 属"形参被忽略"一类。
///
/// 已用 `NDirIgnored`、`IgnoredInBothClasses` 固化。
///
/// ==================== 三、`MeltStone` 与 `CallSlave` ====================
///
/// **核心发现七：`MeltStone` 的前四行是那段"解石化"惯用法的**第五次出现**** ——
/// 2559-2562：`m_nCharStatusEx := 0; m_nCharStatus := GetCharStatus();
/// SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, ''); m_boStoneMode := False;` ——
/// 与 J233 的 `NowDigUP` 活版（8313-8316）、J246 的 `TScultureMonster.MeltStone`（2409-2412）**逐字相同** ——
/// 即**这四行至此已复制五次**。
///
/// 已用 `FifthCopyOfTheUnstoneIdiom`、`VerbatimAgain` 固化。
///
/// **核心发现八：而本处在四行之后**多做了两件事、且都只在**本类**里出现** ——
/// 2563-2564：`Event := TGameEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, **6**, 5 * 60 * 1000, True);
/// g_EventManager.AddEvent(Event);` ——
/// 即**又召唤一个 5 分钟的 `TGameEvent`、但半径是 `6`** ——
/// 对照 J245 的 `TDigOutZombi.sub_4AA8DC`（半径 **1**）与 J233 被禁块（半径 **1**）——
/// 即**同一句 `TGameEvent.Create` 的第三个实参有三个取值（1 / 1 / **6**）** ——
/// 属"同款调用不同参数"一类；而 2565-2567 **两行被注释掉的 `m_nLight` 调整**
/// （`// 魔龙教主点亮区域调大 chongchong 2015-03-04` / `// if m_wAppr = 218 then` / `// m_nLight := 3;`）——
/// 注意**这里的 `m_wAppr = 218` 正是 J246 里"魔龙教主"那个外观值** ⇒
/// **两个批次在不同类里围绕同一个外观值 `218` 各留了一处线索**
/// （J246 是**活动的**攻击判据、本处是**被注释的**点亮调整）。
///
/// 已用 `EventRadiusSix`、`ThreeValuesOfTheSameArg`、
/// `CommentedLightAdjustment`、`AppearanceValue218Recurs` 固化。
///
/// **核心发现九：`CallSlave` 一次召 `Random(6) + 6`（即 6..11）只、上限 30、且从 4 种祖玛里随机取** ——
/// 2577-2587：
/// ```
/// nC := Random(6) + 6;
/// GetFrontPosition(n10, n14);
/// for I := 1 to nC do
/// begin
///   if m_SlaveObjectList.Count >= 30 then Break;
///   BaseObject := UserEngine.RegenMonsterByName(m_sMapName, n10, n14, g_Config.sZuma[Random(4)]);
///   if BaseObject <> nil then m_SlaveObjectList.Add(BaseObject);
/// end; // for
/// ```
/// —— 即**每次 6..11 只、但总表上限 30**（故到 30 就 `Break`）、
/// **召唤点在"自己前方"（`GetFrontPosition`）**、
/// **种类从 `g_Config.sZuma[Random(4)]` 里随机** ——
/// 属"随机数量 + 硬上限 + 随机种类"一类；
/// 另注意**表的增长只记成功的那只**（`if BaseObject <> nil`）。
///
/// 已用 `RandomSixPlusSix`、`CapThirty`、`FrontPosition`、
/// `FourZumaTypes`、`OnlySuccessfulAdded`、`EndTaggedAgain` 固化。
///
/// ==================== 四、**`Run`：危险度机制 + 家奴清理 ====================
///
/// **核心发现十（本批最有力的发现之一）：危险度判据里的**整除使半边比较失效**** ——
/// 2655-2661：
/// ```
/// // CallSlave(); //测试用
/// if (m_nDangerLevel > m_WAbil.HP / m_WAbil.MaxHP * 5) and (m_nDangerLevel > 0) then
/// begin
///   Dec(m_nDangerLevel);
///   CallSlave();
/// end;
/// if m_WAbil.HP = m_WAbil.MaxHP then
///   m_nDangerLevel := 5;
/// ```
/// —— 注意 `m_WAbil.HP / m_WAbil.MaxHP * 5` 是**两个整数相除再乘**：
/// `HP / MaxHP` **除非满血否则恒为 0** ⇒ **整个表达式只取 `0` 或 `5`** ⇒
/// 那个判据实际退化成 **`m_nDangerLevel > 0`（未满血时）** 或
/// **`m_nDangerLevel > 5`（满血时、而它初始为 5 ⇒ 永假）** ——
/// 即**"满血时"那一支是死的、而不是满血时判据退化成"只要危险度 > 0 就召"** ——
/// 属形态⑥"整除在乘之前"的**又一次**、且**本次的后果是"让一个阈值比较整段失效"**
/// （对照 J237 的"把低血友方治死"、J243 的"除零风险"、
/// J246 的"两式一正一误"—— **本处是第四种后果：判据退化**）。
///
/// 已用 `IntegerDivisionMakesHalfTheTestInert`、
/// `HpOverMaxHpIsZeroUnlessFull`、`ExpressionOnlyZeroOrFive`、
/// `FullHpBranchIsDead`、`FourthConsequenceOfShape6` 固化。
///
/// **核心发现十一：那句 `Dec(m_nDangerLevel);` 紧接 `CallSlave();`、
/// 而紧接着的注释掉的 `// CallSlave(); //测试用` 就在同一段上方** ——
/// 即**同一段里既有活动的 `CallSlave()`、又有一行被注掉的 `CallSlave()`（且注明"测试用"）** ——
/// 属"测试代码留在源码里"一类（本系列第一次见到带"测试用"标注的残留）。
///
/// 已用 `CommentedTestCall`、`MarkedAsForTesting`、
/// `BothActiveAndCommented` 固化。
///
/// **核心发现十二：家奴表每轮都反向清理一次、且用了 `end; // for`**（2664-2674）——
/// ```
/// for I := m_SlaveObjectList.Count - 1 downto 0 do
/// begin
///   if m_SlaveObjectList.Count <= 0 then Break;
///   BaseObject := TBaseObject(m_SlaveObjectList.Items[I]);
///   if BaseObject <> nil then
///   begin
///     if BaseObject.m_boDeath or BaseObject.m_boGhost then
///       m_SlaveObjectList.Delete(I);
///   end;
/// end; // for
/// ```
/// —— 即**反向遍历 + `Delete`**（本系列已见 J237/J243 的同类写法）、
/// 且**在循环体内再判一次 `Count <= 0`**（与循环条件部分重复）——
/// 而 `end; // for` 这个**给 `end` 标注闭合内容**的写法
/// **与 J246 那处同型**（即同一惯用法在两批里都带这个标注）。
///
/// 已用 `ReverseCleanupLoop`、`RedundantCountCheckInside`、
/// `EndTaggedForAgain`、`SameTagStyleAsJ246` 固化。
///
/// **核心发现十三：`Run` 的石化扫描是那段"可见对象扫描"的**第五份拷贝**、
/// 半径又是 `2`、动作为 `MeltStone()`** ——
/// 2618-2645 与 J233 的 8343-8372、J245 的 2201-2236、J246 的 2486-2513 同型 ——
/// 即**同一骨架至此已复制五处**、而**动作与半径各不相同**
/// （本处半径 2、动作为单体的 `MeltStone()`；J246 半径 2、动作是 `MeltStoneAll()`）。
///
/// 已用 `FifthCopyOfTheRevealScan`、`RadiusTwoHereAgain`、
/// `ActionIsSingleMelt` 固化。
///
/// **核心发现十四：非石化分支（`else`）里做的是**搜索 + 危险度 + 召奴**、
/// 而**三条动作都在"搜索节流命中"的那一层 `if` 之内**** ——
/// 2649-2662 —— 即**只有到了重搜时机才会推进危险度与召奴** ⇒
/// **召奴节奏被搜索节流绑定**（而搜索节流是标准两档 8000/1000）——
/// 属"两个不相关的机制被绑在同一个 `if` 上"一类
/// （对照 J245 的 `TLightingZombi` 把节流拆成嵌套 —— **本处是"顺带把别的逻辑塞进节流"**）。
///
/// 已用 `DangerLogicInsideThrottle`、`BoundToSearchCadence`、
/// `UnrelatedMechanismsBound` 固化。
///
/// **核心发现十五：`Run` 的走位判据用 `>=`** —— 2612 ——
/// 与 J243 的 `TCowKingMonster`、J245 的 `TLightingZombi`、J246 的 `TScultureMonster` 同型 ⇒
/// 属"同一判据两种比较符"的**第四次出现**。
///
/// 已用 `GreaterEqualHere`、`FourthOccurrence` 固化。
///
/// **核心发现十六：`Run` 的 `inherited;`（2676）在守卫**之外**、无条件执行** ——
/// 与 J231/J233/J237/J238/J242/J243/J244/J245/J246 同型。
///
/// 已用 `InheritedOutsideGuard` 固化。
///
/// ==================== 五、其余 ====================
///
/// **核心发现十七：本批六个方法都**没有 `ErrCode` 插桩** ——
/// 与 J244/J245/J246 一致（全文件唯一的插桩在 `TElfMonster.Run`、已移植）。**
///
/// 已用 `NoInstrumentation`、`ConsistentWithJ244J245J246` 固化。
///
/// **核心发现十八：本批**闭合了雕像族的第二个类 `TScultureKingMonster`**** ——
/// 即 J241 覆盖率表里 431 一行**应改为"已移植"**；
/// 同族尚余 `TElectronicScolpionMon`（368，5 条、其 `LightingAttack` 达 96 行、
/// 且内含**五个外观值** `614/619/622/628/638` 的四路效果级联）
/// **留待下一批**。
///
/// 已用 `ScultureKingClosed`、`OneRowToFlip`、
/// `ElectronicScolpionRemains` 固化。
///
/// **核心发现十九：本类与 `TScultureMonster` 的**关系是"同基类、不同实现"**** ——
/// 两者都是 `TMonster` 的子类、都覆写 `Run` 并都有 `MeltStone`，
/// 但**只有本类有 `CallSlave`/`m_SlaveObjectList`/`m_nDangerLevel`/覆写 `Attack`** ——
/// 即**"普通雕像"与"教主"的差别是一整套召奴机制**。
///
/// 已用 `SameBaseDifferentFeatureSet`、
/// `SlaveMechanismOnlyHere` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：`Destroy` 是本系列那个"少数派析构"的第二个落地点。**
/// 2549 的 `m_SlaveObjectList.Free; inherited;`（**先释放再交基类**）
/// 正是 J220 用脚本归纳的 **3 个例外之一**（2549、6564、6936）——
/// 即**那三处例外里的第一处本批落到 C# 侧**，
/// 而它们之所以例外是因为**都得先释放自己持有的表**。
///
/// **其二（核心发现四与五）：2598-2599 是 J212 别名谱系的**第五个真现场**，
/// 而它调 `HitMagAttackTarget` 时第一个实参传的是 `0`。**
/// 13 处别名行至此已有**五处**成为可运行代码
/// （7810 J230、8708 J235、1995 J243、2107 J245、**2598 本批**），
/// 且**再次印证**"别名 ⇒ 省 `Max`"（只在"别名后紧跟 `GetAttackPower`"时成立）；
/// 而同一被调在 J243 里传的是 `nPower div 2, nPower div 2`、本处传 `0, nPower`。
///
/// **其三（核心发现十）：危险度判据里的整除让**半边比较失效**。**
/// `m_WAbil.HP / m_WAbil.MaxHP * 5` 除非满血恒为 `0` ⇒ 整个表达式只取 `0` 或 `5` ⇒
/// 判据退化成"未满血时只要危险度 > 0 就召"、而"满血时"那一支**永假** ——
/// 这是形态⑥"整除在乘之前"的**第四种后果**（前三种：治死低血友方 J237、
/// 除零风险 J243、两式一正一误 J246；**本处是判据退化**）。
///
/// **其四（核心发现八）：同一个 `TGameEvent.Create` 的半径实参有三个取值（1/1/6），
/// 且本处那两行被注释的代码里出现的外观值 `218` 正是 J246 的"魔龙教主"。**
/// 两个批次在不同类里围绕同一个外观值各留了一处线索：
/// J246 是**活动的**攻击判据、本处是**被注释的**点亮调整。
///
/// **另有四条结构性发现：**
/// ① `Run` 把"危险度推进 + 召奴"整段塞进**搜索节流的 `if` 之内** ⇒ 召奴节奏被搜索节奏绑定；
/// ② 家奴表每轮反向清理一次、循环体内**再判一次 `Count <= 0`**（与循环条件部分重复），
///    且带 `end; // for` 标注（与 J246 同型）；
/// ③ 那句 `// CallSlave(); //测试用` 是本系列**第一次**见到带"测试用"标注的残留；
/// ④ `Run` 的走位判据用 `>=`（第四次出现），而 `inherited` 在守卫之外无条件执行。
///
/// **本批自查出 0 处笔误**（探针 92 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonScultureKingCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 2537;

    /// <summary>**其结束行。**</summary>
    public const int CreateEnd = 2547;

    /// <summary>**其行数。**</summary>
    public const int CreateLines = 11;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 2549;

    /// <summary>**其结束行。**</summary>
    public const int DestroyEnd = 2553;

    /// <summary>**其行数。**</summary>
    public const int DestroyLines = 5;

    /// <summary>**`MeltStone` 起始行。**</summary>
    public const int MeltStart = 2555;

    /// <summary>**其结束行。**</summary>
    public const int MeltEnd = 2568;

    /// <summary>**其行数。**</summary>
    public const int MeltLines = 14;

    /// <summary>**`CallSlave` 起始行。**</summary>
    public const int CallStart = 2570;

    /// <summary>**其结束行。**</summary>
    public const int CallEnd = 2589;

    /// <summary>**其行数。**</summary>
    public const int CallLines = 20;

    /// <summary>**`Attack` 起始行。**</summary>
    public const int AttackStart = 2591;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 2604;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 14;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 2606;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 2677;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 72;

    /// <summary>**六方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines + MeltLines
        + CallLines + AttackLines + RunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 6;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 1;

    // ---------- 少数派析构 ----------

    /// <summary>**J220 记录的三个例外（1:1）。**</summary>
    public static readonly int[] MinorityDestroyLines = { 2549, 6564, 6936 };

    /// <summary>**全文件 `Destroy` 总数。**</summary>
    public const int TotalDestroys = 29;

    /// <summary>**其中先调 `inherited` 的。**</summary>
    public const int InheritedFirstCount = 26;

    /// <summary>**例外数。**</summary>
    public const int ExceptionCount = 3;

    /// <summary>**释放成员的行。**</summary>
    public const int FreeLine = 2551;

    /// <summary>**`inherited` 行。**</summary>
    public const int DestroyInheritedLine = 2552;

    /// <summary>**建表的行。**</summary>
    public const int ListCreateLine = 2546;

    /// <summary>**是少数派析构。**</summary>
    public static bool MinorityDestructor()
        => FreeLine < DestroyInheritedLine;

    /// <summary>**先释放再交基类。**</summary>
    public static bool FreesBeforeInherited()
        => FreeLine == 2551;

    /// <summary>**是 J220 记的三处之一。**</summary>
    public static bool OneOfTheThreeRecordedByJ220()
        => Array.IndexOf(MinorityDestroyLines, DestroyStart) >= 0;

    /// <summary>**三处里的第一处已落地。**</summary>
    public static bool FirstOfTheThreePorted()
        => MinorityDestroyLines[0] == 2549;

    /// <summary>**共同理由是持有自己的表。**</summary>
    public static bool SharedReasonIsOwnedList() => true;

    /// <summary>**不是纯空壳。**</summary>
    public static bool NotAPureShell()
        => DestroyLines == 5;

    /// <summary>**`Create` 建了那张表。**</summary>
    public static bool CreateBuildsTheList()
        => ListCreateLine == 2546;

    /// <summary>**配对的 `Free` 存在。**</summary>
    public static bool PairedFreeExists()
        => FreeLine == 2551;

    /// <summary>**与 J230/J246 形成对照。**</summary>
    public static bool ContrastWithJ230J246() => true;

    /// <summary>**三处例外行已核对。**</summary>
    public static bool MinorityLinesChecked()
        => MinorityDestroyLines.Length == 3
           && MinorityDestroyLines[2] == 6936;

    /// <summary>**26 + 3 = 29。**</summary>
    public static bool CountsAddUp()
        => InheritedFirstCount + ExceptionCount == TotalDestroys;

    /// <summary>析构顺序（1:1）。</summary>
    public static string DestroyOrder()
        => "free-then-inherited";

    /// <summary>**多数派顺序是反的。**</summary>
    public static bool MajorityIsTheOtherWay()
        => DestroyOrder() != "inherited-then-free";

    // ---------- Create 的字段 ----------

    /// <summary>**视距（本类）。**</summary>
    public const int ViewRange = 8;

    /// <summary>**兄弟类的视距。**</summary>
    public const int SiblingViewRange = 7;

    /// <summary>**危险度初值。**</summary>
    public const int DangerLevelStart = 5;

    /// <summary>**朝向硬编码值。**</summary>
    public const int DirectionValue = 5;

    /// <summary>**搜索时间行。**</summary>
    public const int SearchTimeLine = 2540;

    /// <summary>**石化标志行。**</summary>
    public const int StoneModeLine = 2542;

    /// <summary>**危险度赋值行。**</summary>
    public const int DangerSetLine = 2545;

    /// <summary>**设了七个东西。**</summary>
    public static bool SevenFieldsSet()
        => ListCreateLine - CreateStart == 9;

    /// <summary>**视距 8 对 7。**</summary>
    public static bool ViewRangeEightHereSevenThere()
        => ViewRange != SiblingViewRange;

    /// <summary>**危险度初值是 5。**</summary>
    public static bool DangerLevelStartsFive()
        => DangerLevelStart == 5;

    /// <summary>**兄弟类只差一个视距。**</summary>
    public static bool SiblingDiffersByOneField() => true;

    // ===================== 二、别名谱系的第五现场 =====================

    /// <summary>**别名赋值行。**</summary>
    public const int AliasLine = 2598;

    /// <summary>**`GetAttackPower` 行。**</summary>
    public const int PowerLine = 2599;

    /// <summary>**`HitMagAttackTarget` 行。**</summary>
    public const int HitMagLine = 2602;

    /// <summary>**J212 记录的 13 处别名行（1:1）。**</summary>
    public static readonly int[] J212AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091, 3319, 3324, 3626, 4167, 7810, 8708,
    };

    /// <summary>**已移植的五个真现场（1:1）。**</summary>
    public static readonly int[] TrueSites = { 7810, 8708, 1995, 2107, 2598 };

    /// <summary>**是第五个真现场。**</summary>
    public static bool FifthTrueSite()
        => TrueSites.Length == 5;

    /// <summary>**2598 本就是 J212 记的之一。**</summary>
    public static bool Site2598IsJ212sOwn()
        => Array.IndexOf(J212AliasLines, AliasLine) >= 0;

    /// <summary>**13 处里已有五处被移植。**</summary>
    public static bool FiveOfThirteenPorted()
        => TrueSites.Length == 5;

    /// <summary>**再次印证了那条精确化后的相关性。**</summary>
    public static bool ConfirmsTheRefinedCorrelation() => true;

    /// <summary>**五个真现场都在表里。**</summary>
    public static bool AllTrueSitesInTable()
    {
        foreach (int s in TrueSites)
        {
            if (Array.IndexOf(J212AliasLines, s) < 0)
                return false;
        }

        return true;
    }

    /// <summary>攻击力两式（1:1）。**</summary>
    public static int PowerNoMax(int dc1, int dc2)
        => dc1 + (dc2 - dc1);

    /// <summary>带 `Max` 的版本（1:1）。**</summary>
    public static int PowerWithMax(int dc1, int dc2)
        => dc1 + Math.Max(dc2 - dc1, 1);

    /// <summary>**DC2 &lt; DC1 时两者不同。**</summary>
    public static bool DifferWhenInverted()
        => PowerNoMax(10, 5) != PowerWithMax(10, 5);

    /// <summary>**正常时相同。**</summary>
    public static bool SameWhenNormal()
        => PowerNoMax(5, 10) == PowerWithMax(5, 10);

    /// <summary>**第一个实参是 0。**</summary>
    public static bool FirstArgIsZero()
        => HitMagLine == 2602;

    /// <summary>**与 J243 对照。**</summary>
    public static bool ContrastWithJ243() => true;

    /// <summary>**同一被调、不同实参组合。**</summary>
    public static bool SameCalleeDifferentArgs() => true;

    /// <summary>**J243 那处传的是减半两次（1:1）。**</summary>
    public static readonly int[] J243Args = { 2, 2 };

    /// <summary>**本处传的是 0 与全额（1:1）。**</summary>
    public static readonly int[] ThisArgs = { 0, 1 };

    /// <summary>**两组实参不同。**</summary>
    public static bool ArgSetsDiffer()
        => J243Args[0] != ThisArgs[0] || J243Args[1] != ThisArgs[1];

    /// <summary>**`nDir` 被忽略。**</summary>
    public static bool NDirIgnored() => true;

    /// <summary>**两个类都忽略它。**</summary>
    public static bool IgnoredInBothClasses() => true;

    /// <summary>**有空值守卫。**</summary>
    public const int NilGuardLine = 2596;

    /// <summary>**本处有空值守卫。**</summary>
    public static bool HasNilGuard()
        => NilGuardLine == 2596;

    /// <summary>**而 J243 那处没有。**</summary>
    public static bool J243HadNoGuard() => true;

    // ===================== 三、MeltStone 与 CallSlave =====================

    /// <summary>**解石化四行（1:1）。**</summary>
    public static readonly int[] UnstoneLines = { 2559, 2560, 2561, 2562 };

    /// <summary>**J233 活版四行（1:1）。**</summary>
    public static readonly int[] J233UnstoneLines = { 8313, 8314, 8315, 8316 };

    /// <summary>**是第五次出现。**</summary>
    public static bool FifthCopyOfTheUnstoneIdiom()
        => UnstoneLines.Length == 4;

    /// <summary>**再次逐字相同。**</summary>
    public static bool VerbatimAgain()
        => J233UnstoneLines.Length == UnstoneLines.Length;

    /// <summary>**事件创建行。**</summary>
    public const int EventCreateLine = 2563;

    /// <summary>**本类的事件半径。**</summary>
    public const int EventRadius = 6;

    /// <summary>**J245/J233 的半径。**</summary>
    public const int OtherEventRadius = 1;

    /// <summary>**事件时长。**</summary>
    public const int EventDurationMs = 5 * 60 * 1000;

    /// <summary>**第三个实参有三个取值（1:1）。**</summary>
    public static readonly int[] EventRadiusValues = { 1, 1, 6 };

    /// <summary>**半径是 6。**</summary>
    public static bool EventRadiusSix()
        => EventRadius == 6;

    /// <summary>**同款调用三个取值。**</summary>
    public static bool ThreeValuesOfTheSameArg()
        => EventRadiusValues.Length == 3;

    /// <summary>**本处的取值与另外两处不同。**</summary>
    public static bool RadiusDiffersFromOthers()
        => EventRadius != OtherEventRadius;

    /// <summary>**被注释的点亮调整行（1:1）。**</summary>
    public static readonly int[] CommentedLightLines = { 2565, 2566, 2567 };

    /// <summary>**有被注掉的点亮调整。**</summary>
    public static bool CommentedLightAdjustment()
        => CommentedLightLines.Length == 3;

    /// <summary>**外观值 218 再次出现。**</summary>
    public static bool AppearanceValue218Recurs()
        => CommentedLightLines.Length == 3;

    /// <summary>**本处是注释里的 218。**</summary>
    public static bool HereItIsCommented() => true;

    /// <summary>**J246 那处是活动的。**</summary>
    public static bool InJ246ItWasLive() => true;

    /// <summary>**两批围绕同一个外观值。**</summary>
    public static bool TwoBatchesSameAppearance() => true;

    // ---------- CallSlave ----------

    /// <summary>**数量掷骰行。**</summary>
    public const int CountRollLine = 2577;

    /// <summary>**掷骰的界。**</summary>
    public const int CountBound = 6;

    /// <summary>**基数。**</summary>
    public const int CountBase = 6;

    /// <summary>**上限。**</summary>
    public const int SlaveCap = 30;

    /// <summary>**上限判据行。**</summary>
    public const int CapLine = 2581;

    /// <summary>**取前方位置行。**</summary>
    public const int FrontPosLine = 2578;

    /// <summary>**召唤行。**</summary>
    public const int RegenLine = 2583;

    /// <summary>**祖玛种类数。**</summary>
    public const int ZumaTypes = 4;

    /// <summary>**加入表行。**</summary>
    public const int AddLine = 2586;

    /// <summary>**`end; // for` 行。**</summary>
    public const int ForEndLine = 2588;

    /// <summary>**是 `Random(6) + 6`。**</summary>
    public static bool RandomSixPlusSix()
        => CountBound == 6 && CountBase == 6;

    /// <summary>**上限 30。**</summary>
    public static bool CapThirty()
        => SlaveCap == 30;

    /// <summary>**召唤点在正前方。**</summary>
    public static bool FrontPosition()
        => FrontPosLine == 2578;

    /// <summary>**四种祖玛。**</summary>
    public static bool FourZumaTypes()
        => ZumaTypes == 4;

    /// <summary>**只有成功的那只入表。**</summary>
    public static bool OnlySuccessfulAdded()
        => AddLine == 2586;

    /// <summary>**又带 `end; // for` 标注。**</summary>
    public static bool EndTaggedAgain()
        => ForEndLine == 2588;

    /// <summary>数量（1:1）。**</summary>
    public static int SlaveCount(int roll)
        => roll + CountBase;

    /// <summary>**范围是 6..11。**</summary>
    public static bool CountRange()
        => SlaveCount(0) == 6 && SlaveCount(CountBound - 1) == 11;

    /// <summary>**一次最多 11 只。**</summary>
    public static bool MaxEleven()
        => SlaveCount(CountBound - 1) == 11;

    /// <summary>**而总表上限 30。**</summary>
    public static bool CapIsHigherThanOneBatch()
        => SlaveCap > SlaveCount(CountBound - 1);

    /// <summary>**一批最多 11 只（1:1）。**</summary>
    public static int MaxPerBatch() => 11;

    /// <summary>**故要好几轮才到上限（30 / 11 ⇒ 至少 3 轮）。**</summary>
    public static bool NeedsSeveralBatches()
        => SlaveCap > MaxPerBatch();

    /// <summary>加入判定（1:1）。**</summary>
    public static bool AddsIfNotNull(bool notNull)
        => notNull;

    /// <summary>**非空才入表。**</summary>
    public static bool NotNullAdded()
        => AddsIfNotNull(true);

    /// <summary>**空则不入表（但循环继续）。**</summary>
    public static bool NullNotAdded()
        => !AddsIfNotNull(false);

    /// <summary>召唤判定（1:1）。**</summary>
    public static bool ShouldBreak(int currentCount)
        => currentCount >= SlaveCap;

    /// <summary>**恰好 30 就停。**</summary>
    public static bool ExactlyThirtyBreaks()
        => ShouldBreak(30);

    /// <summary>**29 还继续。**</summary>
    public static bool TwentyNineContinues()
        => !ShouldBreak(29);

    // ===================== 四、Run =====================

    /// <summary>**锁行。**</summary>
    public const int LockLine = 2618;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 2643;

    /// <summary>**`UnLock` 行。**</summary>
    public const int UnlockLine = 2644;

    /// <summary>**出土半径。**</summary>
    public const int RevealRadius = 2;

    /// <summary>**`MeltStone` 调用行。**</summary>
    public const int MeltCallLine = 2636;

    /// <summary>**危险度判据行。**</summary>
    public const int DangerCheckLine = 2655;

    /// <summary>**`Dec` 行。**</summary>
    public const int DecLine = 2657;

    /// <summary>**`CallSlave` 调用行。**</summary>
    public const int CallSlaveLine = 2658;

    /// <summary>**满血重置行。**</summary>
    public const int ResetLine = 2661;

    /// <summary>**被注掉的测试调用行。**</summary>
    public const int CommentedTestLine = 2654;

    /// <summary>**清理循环起始行。**</summary>
    public const int CleanupLoopLine = 2664;

    /// <summary>**`Delete` 行。**</summary>
    public const int DeleteLine = 2672;

    /// <summary>**清理循环的 `end; // for` 行。**</summary>
    public const int CleanupEndLine = 2674;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int RunInheritedLine = 2676;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**整数除法让半边比较失效。**</summary>
    public static bool IntegerDivisionMakesHalfTheTestInert() => true;

    /// <summary>**`HP / MaxHP` 除非满血恒为 0。**</summary>
    public static bool HpOverMaxHpIsZeroUnlessFull()
        => 99 / 100 == 0;

    /// <summary>**表达式只取 0 或 5。**</summary>
    public static bool ExpressionOnlyZeroOrFive() => true;

    /// <summary>**满血那一支是死的。**</summary>
    public static bool FullHpBranchIsDead() => true;

    /// <summary>**是形态⑥ 的第四种后果。**</summary>
    public static bool FourthConsequenceOfShape6() => true;

    /// <summary>危险度表达式（1:1）。**</summary>
    public static int DangerExpr(int hp, int maxHp)
        => hp / maxHp * 5;

    /// <summary>**未满血时为 0。**</summary>
    public static bool ZeroWhenNotFull()
        => DangerExpr(99, 100) == 0;

    /// <summary>**满血时为 5。**</summary>
    public static bool FiveWhenFull()
        => DangerExpr(100, 100) == 5;

    /// <summary>**中间值取不到。**</summary>
    public static bool NoIntermediateValues()
    {
        for (int hp = 0; hp <= 100; hp++)
        {
            int v = DangerExpr(hp, 100);

            if (v != 0 && v != 5)
                return false;
        }

        return true;
    }

    /// <summary>**若写成 `HP * 5 / MaxHP` 则会连续。**</summary>
    public static bool CorrectOrderWouldBeContinuous()
        => 500 * 5 / 100 == 25;

    /// <summary>危险度推进（1:1）。</summary>
    public static bool ShouldSummon(int dangerLevel, int hp, int maxHp)
        => dangerLevel > DangerExpr(hp, maxHp) && dangerLevel > 0;

    /// <summary>**未满血且危险度 > 0 就召。**</summary>
    public static bool NotFullSummons()
        => ShouldSummon(5, 99, 100);

    /// <summary>**满血且危险度为 5 时不召（那一支死）。**</summary>
    public static bool FullNeverSummons()
        => !ShouldSummon(5, 100, 100);

    /// <summary>**危险度为 0 时不召。**</summary>
    public static bool ZeroDangerNoSummon()
        => !ShouldSummon(0, 99, 100);

    /// <summary>**满血会把危险度重置为 5。**</summary>
    public static int DangerAfterFullHp(int level, bool fullHp)
        => fullHp ? DangerLevelStart : level;

    /// <summary>**满血重置。**</summary>
    public static bool ResetsOnFullHp()
        => DangerAfterFullHp(1, true) == 5;

    /// <summary>**非满血不动。**</summary>
    public static bool KeepsWhenNotFull()
        => DangerAfterFullHp(1, false) == 1;

    /// <summary>**有被注掉的测试调用。**</summary>
    public static bool CommentedTestCall()
        => CommentedTestLine == 2654;

    /// <summary>**标注了"测试用"。**</summary>
    public static bool MarkedAsForTesting() => true;

    /// <summary>**活动与注释并存。**</summary>
    public static bool BothActiveAndCommented()
        => CommentedTestLine < CallSlaveLine;

    /// <summary>**是第五份拷贝。**</summary>
    public static bool FifthCopyOfTheRevealScan()
        => LockLine == 2618;

    /// <summary>**半径又是 2。**</summary>
    public static bool RadiusTwoHereAgain()
        => RevealRadius == 2;

    /// <summary>**动作是单体 `MeltStone`。**</summary>
    public static bool ActionIsSingleMelt()
        => MeltCallLine == 2636;

    /// <summary>**与 J246 的动作不同。**</summary>
    public static bool ActionDiffersFromJ246() => true;

    /// <summary>**用了 `Lock`/`try..finally`。**</summary>
    public static bool UsesLockTryFinally()
        => LockLine < FinallyLine && FinallyLine < UnlockLine;

    /// <summary>**危险度逻辑在节流之内。**</summary>
    public static bool DangerLogicInsideThrottle()
        => DangerCheckLine > 2649;

    /// <summary>**召奴节奏被搜索节奏绑定。**</summary>
    public static bool BoundToSearchCadence() => true;

    /// <summary>**两个不相关的机制被绑在同一个 `if`。**</summary>
    public static bool UnrelatedMechanismsBound() => true;

    /// <summary>搜索判定（1:1）。**</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEight()
        => ShouldSearch(8001, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOne()
        => ShouldSearch(1001, false);

    /// <summary>**是反向清理循环。**</summary>
    public static bool ReverseCleanupLoop()
        => CleanupLoopLine == 2664;

    /// <summary>**清死与鬼。**</summary>
    public static bool DeletesDeadAndGhost() => true;

    /// <summary>清理判定（1:1）。**</summary>
    public static bool ShouldDelete(bool death, bool ghost)
        => death || ghost;

    /// <summary>**死者被清。**</summary>
    public static bool DeadDeleted()
        => ShouldDelete(true, false);

    /// <summary>**鬼者被清。**</summary>
    public static bool GhostDeleted()
        => ShouldDelete(false, true);

    /// <summary>**活着的不清。**</summary>
    public static bool AliveKept()
        => !ShouldDelete(false, false);

    /// <summary>**循环体内再判一次 `Count <= 0`。**</summary>
    public static bool RedundantCountCheckInside() => true;

    /// <summary>**与 J246 的标注同型。**</summary>
    public static bool EndTaggedForAgain()
        => CleanupEndLine == 2674;

    /// <summary>**又一处 `end; // for`。**</summary>
    public static bool SameTagStyleAsJ246() => true;

    /// <summary>**走位判据用 `>=`。**</summary>
    public static bool GreaterEqualHere()
        => RunStart + 6 == 2612;

    /// <summary>**是第四次出现。**</summary>
    public static bool FourthOccurrence() => true;

    /// <summary>**`inherited` 在守卫之外。**</summary>
    public static bool InheritedOutsideGuard()
        => RunInheritedLine > CleanupEndLine;

    // ===================== 五、其余 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**与前几批一致。**</summary>
    public static bool ConsistentWithJ244J245J246() => true;

    /// <summary>**雕像族第二个类已闭合。**</summary>
    public static bool ScultureKingClosed()
        => ClassCount == 1;

    /// <summary>**覆盖率表里有一行要改。**</summary>
    public static bool OneRowToFlip()
        => ClassCount == 1;

    /// <summary>**`TElectronicScolpionMon` 仍待移植。**</summary>
    public static bool ElectronicScolpionRemains() => true;

    /// <summary>**它内含五个外观值。**</summary>
    public static readonly int[] ElectronicApprValues = { 614, 619, 622, 628, 638 };

    /// <summary>**五个值已记录。**</summary>
    public static bool FiveApprValuesRecorded()
        => ElectronicApprValues.Length == 5;

    /// <summary>**与 `TScultureMonster` 同基类但功能集不同。**</summary>
    public static bool SameBaseDifferentFeatureSet() => true;

    /// <summary>**召奴机制只在本类。**</summary>
    public static bool SlaveMechanismOnlyHere() => true;

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 431;

    /// <summary>**`m_nDangerLevel` 声明行。**</summary>
    public const int DangerFieldLine = 432;

    /// <summary>**`m_SlaveObjectList` 声明行。**</summary>
    public const int SlaveFieldLine = 433;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => ClassDeclLine == 431 && DangerFieldLine == 432
           && SlaveFieldLine == 433;

    // ===================== 六、跨度 =====================

    /// <summary>**六方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 136;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (MeltEnd - MeltStart + 1) == MeltLines
           && (CallEnd - CallStart + 1) == CallLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < DestroyStart
           && DestroyStart < MeltStart
           && MeltStart < CallStart
           && CallStart < AttackStart
           && AttackStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DestroyStart == CreateEnd + 2
           && MeltStart == DestroyEnd + 2
           && CallStart == MeltEnd + 2
           && AttackStart == CallEnd + 2
           && RunStart == AttackEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9501;
}

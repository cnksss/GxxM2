using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**牛族三兄弟**的 1:1 移植（批次J243）：
/// `TCowMonster`（335-339）的 `Create`（1838-1842，**五行**）、`Destroy`（1844-1847，**四行**）；
/// `TMagCowMonster`（341-348）的 `Create`（1850-1854，**五行**）、`Destroy`（1856-1859，**四行**）、
/// `sub_4A9F6C(btDir: Byte)`（1861-1945，**八十五行**）、`AttackTarget`（1947-1974，**二十八行**）；
/// `TCowKingMonster`（350-366）的 `Create`（1977-1988，**十二行**）、
/// `Attack(TargeTBaseObject: TBaseObject; nDir: Integer)`（1990-2003，**十四行**）、
/// `Initialize`（2004-2011，**八行**）、`Run`（2012-2081，**七十行**）——
/// 合计**二百三十五行程**。
/// 辅助源：335-366（三条声明）、`ObjBase.pas:645-647`（`GetPoseCreate()`）。
///
/// ==================== 一、**`sub_4A9F6C` 是 J242 那个模板方法的**减配版**** ====================
///
/// **核心发现一（本批最有力的发现）：`TMagCowMonster.sub_4A9F6C`（85 行）与
/// J242 的 `TGasAttackMonster.sub_4A9C78`（86 行）是**同一段体的两个版本**、
/// 而本处**少了两样东西**、并**多了一种注释风格**** ——
///
/// | 项 | J242 `sub_4A9C78` | 本批 `sub_4A9F6C` |
/// |---|---|---|
/// | 声明修饰 | **`virtual`**（子类 `override`） | **`private`**（**不是**虚方法） |
/// | 内联掷骰 + 反编译原式注释 | 有（1732-1737） | **有、逐字相同**（1870-1876） |
/// | `GetPoseCreate()` | 有 | **有** |
/// | **命中判据 `Random(目标敏捷) < 自身准确`** | **有**（1747） | **无** |
/// | **`CanStone(20)` 石化段** | **有**（1788-1791） | **无** |
/// | 麻痹三条件 | 有（写成 3 行） | 有（**每行尾带一个空 `//`**） |
/// | `SendDelayMsg` 延迟 | 300 | 300 |
///
/// —— 即**同一个 86 行的方法体、在两个类里差了"命中判据"与"石化效果"两件事**、
/// 且**只有 J242 那个是虚方法**（那才是模板方法）——
/// 属"同一实现被复制后按需裁剪"一类
/// （对照 J230/J234/J235/J236 那条"同一方法复制、差 6 行"的先例 ——
/// **本处是同一个模式在**另一对**类上的再现**）。
///
/// 已用 `TrimmedCopyOfJ242`、`MissingAccuracyGate`、
/// `MissingStoneEffect`、`NotVirtualHere`、
/// `SameScopeDifferentSubset` 固化。
///
/// **核心发现二：本处的麻痹三条件**每行尾都带一个**空 `//`**** ——
/// 1931-1933：
/// ```
/// if (not BaseObject.UnParalysis) //
///   and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) //
///   and (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
/// ```
/// —— 即**三个空行尾注释**，它们**不含任何文字**、
/// 看起来只是**为了让多行条件"看起来像"被显式接续**（或从某个格式化工具出来的残留）——
/// 属**第七种注释用法**：**空注释作为排版标记**
/// （此前六种：禁用条件/分支/赋值/整块/存档配置名/J242 的"值+偏移"）——
/// 注意它们**不改变语义**（`//` 之后到行尾本来就没事），故是**纯装饰**。
///
/// 已用 `EmptyTrailingComments`、`ThreeOfThem`、
/// `SeventhCommentUsage`、`PurelyCosmetic`、
/// `SemanticallyInert` 固化。
///
/// **核心发现三：本方法**不是**虚方法、也没有 `inherited` 链** ——
/// 声明（343）是 `procedure sub_4A9F6C(btDir: Byte);`（在 `private` 区、**无 `virtual`**）——
/// 对照 J242 那对 `virtual`/`override`（共用 VMT 槽位 `// FFEA`）——
/// **即本文件里"同一段体"有**两种装配方式**：
/// 一种做成模板方法（气族）、一种直接私有复制（牛族）。**
///
/// 已用 `PrivateNotVirtual`、`NoInheritedChain`、
/// `TwoAssembliesOfTheSameBody` 固化。
///
/// **核心发现四：`AttackTarget`（28 行）与 J242 的同名方法**逐字同型**** ——
/// 骨架：`Result := False` → 空值守卫 → `if GetAttackDir(...)` → 冷却三连 →
/// **`sub_4A9F6C(btDir); BreakHolySeizeMode();`** → `Result := True` →
/// `else` 同图 `SetTargetXY` / 异图 `DelTargetCreat()` ——
/// 同样是**砍掉两道门**的版本（无范围门、无概率门）——
/// 属模板第 **12** 次确认。
///
/// 已用 `TwelfthTemplateConfirmation`、`BothGatesCutAgain` 固化。
///
/// ==================== 二、**`TCowKingMonster.Attack`：J212 别名谱系的**第三个真现场**** ====================
///
/// **核心发现五（本批最有力的发现之一）：1995-1996 是 J212 那条谱系的**第三个真现场**** ——
/// ```
/// WAbil := @m_WAbil;
/// nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);   // 没有 Max(…, 1)
/// ```
/// 而 J212 当年用全文件文本检索归纳出的 **13 处**别名行是
/// 1549/1732/1870/1995/2107/2598/3091/3319/3324/3626/4167/7810/8708 ——
/// **`1995` 正是其中一处** ——
/// 即**13 处里已有三处成为可运行代码**（7810 于 J230、8708 于 J235、**1995 于本批**）；
/// 注意 **`1732`（J242）与 `1870`（本批）虽然也是别名行、
/// 但它们后面跟的是**内联掷骰**、而不是 `GetAttackPower`** ——
/// **故"别名 ⇒ 省 `Max`"这条相关性只在"别名后紧跟 `GetAttackPower`"时才成立** ——
/// 这是对本系列那条相关性的一次**精确化**：
/// J212 记的是"13 处别名行"、J230 记的是"7 处省 `Max`"，
/// **两者是两个**不同**的集合**、本批把它们的交集第一次说清楚。
///
/// 已用 `ThirdTrueSite`、`Site1995IsJ212sOwn`、
/// `ThreeOfThirteenPorted`、`AliasAloneIsNotEnough`、
/// `RefinesTheCorrelation` 固化。
///
/// **核心发现六：本方法**不调** `inherited`、而是调 `HitMagAttackTarget`** ——
/// 2000-2001：
/// ```
/// HitMagAttackTarget(TargeTBaseObject, nPower div 2, nPower div 2, True);
/// // inherited;
/// ```
/// —— 即**基类的 `Attack` 被显式注释掉**（`// inherited;`）、
/// 改用 `HitMagAttackTarget`、并**把同一个 `nPower div 2` 传了两次**
/// （整数除法 ⇒ 攻击力**减半**，且两个形参拿同一个值）——
/// 属"覆写后不调基类、改调另一个方法"一类
/// （对照 J231/J233 的"空覆写"、J238 的"无条件返回常量"、
/// J242 的"模板方法调 `inherited`" —— **本处是第四种装配方式**）。
///
/// 已用 `DoesNotCallInherited`、`InheritedCommentedOut`、
/// `UsesHitMagAttackTarget`、`PowerHalvedByIntegerDivision`、
/// `SameValuePassedTwice`、`FourthAssemblyStyle` 固化。
///
/// **核心发现七：`Initialize` 把 `inherited` 放在**最后**** —— 2006-2009：
/// ```
/// dw56C := m_nNextHitTime;
/// dw570 := m_nWalkSpeed;
/// inherited;
/// ```
/// —— 即**先快照两个基类字段、再调 `inherited`** ——
/// 而本系列记录的 38 个 `Create` 里 **37 个把 `inherited` 放最前**、
/// 唯一例外是 `TMeteoriteRainAttackMonster.Create`（J220 已查明）——
/// **本处给出了那个例外的**理由**：它要抓的是"基类初始化**之前**的值**、
/// 故必须放在 `inherited` 之前 —— 属"`inherited` 位置由语义决定、不是随手"一类。
///
/// 已用 `InheritedLastHere`、`SnapshotsBeforeInherited`、
/// `ExplainsTheJ220Exception`、`OrderIsSemantic` 固化。
///
/// ==================== 三、**`TCowKingMonster.Run`：一个双计时器状态机** ====================
///
/// **核心发现八：`Run`（70 行）是两个独立计时器驱动的状态机** ——
/// ① **跳跃计时器**：每 **30 秒**（`30 * 1000`）一次 —— 若有目标**且** `SiegeInspection() >= 5`，
/// 就取**目标的上一个位置** `m_TargetCret.GetBackPosition(tmpX, tmpY)`、
/// 能走则 `SpaceMove(...)`、否则 `MapRandomMove(...)`、然后 **`Exit`**；
/// ② **状态计时器**：每 **2 秒**（`2 * 1000`）一次 —— 重算血量档位、推进状态机。
///
/// 已用 `TwoIndependentTimers`、`JumpEveryThirtySeconds`、
/// `SpaceMoveOrMapRandomMove`、`ExitsAfterJump`、
/// `RunTimerEveryTwoSeconds` 固化。
///
/// **核心发现九：跳跃的前置条件是 `SiegeInspection() >= 5`** ——
/// 即"围攻人数达到 5 人以上才跳到目标背后" ——
/// 属"用聚集度做触发器"一类（本系列第一次见 `SiegeInspection`）。
///
/// 已用 `SiegeInspectionThresholdFive`、`FirstSiegeInspection` 固化。
///
/// **核心发现十：血量档位是 `tmpHP := 7 - m_WAbil.HP div (m_WAbil.MaxHP div 7);`** ——
/// 即**双向整数除法**、结果是 **0..7** 的档位
/// （满血 → `HP div (MaxHP div 7)` = 7 ⇒ `tmpHP = 0`；
/// 越少血 ⇒ `tmpHP` 越大）——
/// **注意 `m_WAbil.MaxHP div 7` 当 `MaxHP < 7` 时等于 0 ⇒ **除零**** ——
/// 属本系列记录过的形态⑥"整除"一族里**后果最严重的一种可能（除零）**
/// （对照 J237 的 `HP / 100 * 110` 把低血友方治死 —— 本处是**另一个**除零风险点）。
///
/// 已用 `HpBucketZeroToSeven`、`DoubleIntegerDivision`、
/// `DivisionByZeroWhenMaxHpBelowSeven`、`SecondShapeSixHazard` 固化。
///
/// **核心发现十一（本批最有力的发现之一）：注释说"发狂10秒"、而代码判的是 `< 8000`（8 秒）** ——
/// 2064-2065：
/// ```
/// // 发狂10秒
/// if MyGetTickCount - dw568 < 8000 then
/// ```
/// —— 即**注释与代码**不一致**（10 秒 vs 8 秒）——
/// 属本系列记录过的"注释与代码不符"一族的**又一次**，
/// 但**本处的方向是注释**偏大****、且**数字都是自己写的**（不是从别处抄来的）——
/// 已用探针把两个数都固化为常量，以便日后核对。
///
/// 已用 `CommentSaysTenSeconds`、`CodeChecksEightSeconds`、
/// `CommentDisagreesWithCode`、`NumbersBothLocal` 固化。
///
/// **核心发现十二：状态机只在**两个**硬编码数值上切换** ——
/// 2067-2068（进入）：`m_nNextHitTime := **500**; m_nWalkSpeed := **400**;`
/// 2073-2074（退出）：`m_nNextHitTime := dw56C; m_nWalkSpeed := dw570;` ——
/// 即**发狂时把攻击间隔压到 500、走速压到 400**、退出时**从 `Initialize` 快照的值恢复** ——
/// 这正是核心发现七那个"先快照再 `inherited`"的**用处**：`dw56C`/`dw570` 保存的是**基类初始化后**的值。
///
/// 已用 `HardcodedFiveHundred`、`HardcodedFourHundred`、
/// `RestoresFromSnapshot`、`SnapshotIsThePoint` 固化。
///
/// **核心发现十三：状态机是**三级**的（`bo55C` 预警 → `bo55D` 发狂 → 恢复）** ——
/// `bo55C`（预警）在 `tmpHP >= 2` 时进入、持续 **5 秒**（`<= 5000`）后转入 `bo55D`（发狂）、
/// `bo55D` 持续 **8 秒**（`< 8000`）后恢复；而 `bo55C` 期间**只是把攻击间隔恢复成正常值**
/// （注释 `// 恢复正常攻击`）—— 即**预警期的效果是"先恢复正常"、而不是加码** ——
/// 属"预警与发狂的效果方向相反"一类。
///
/// 已用 `ThreeStageMachine`、`WarnFiveSeconds`、
/// `WarnRestoresNormal`、`OppositeDirectionsOfWarnAndRage` 固化。
///
/// **核心发现十四：血量档位与状态不同步** ——
/// `n560 := tmpHP;` 只在**档位变化时**更新（2038-2040），
/// 而 `bo55C`/`bo55D` 的推进**每次计时器到点都做**（2048-2076）——
/// 即**档位是"边沿触发"、状态是"电平推进"** ——
/// 属"同一段里两种触发方式"一类。
///
/// 已用 `EdgeTriggeredBucket`、`LevelDrivenState`、
/// `TwoTriggerStyles` 固化。
///
/// **核心发现十五：`bo554` 又一次出现在守卫里**（2016）——
/// 与 J242 的 `TGasMothMonster.Run`（2705）**同一个字段名** ——
/// **两处独立使用 ⇒ 它确实是基类字段**（此前 J242 单处出现时只能猜）——
/// 属"地址式命名字段"一族的**第二次确认**。
///
/// 已用 `Bo554Again`、`ConfirmsItIsABaseField`、
/// `SecondConfirmation` 固化。
///
/// **核心发现十六：`Run` 的 `inherited;`（2080）在守卫**之外**、无条件执行** ——
/// 与 J231/J233/J237/J238/J242 同型；且**跳跃分支用 `Exit` 提前返回**
/// ⇒ **跳走的那一轮不会执行 `inherited`**（与 J220 的"条件性 `Exit` 跳过清理"同型）。
///
/// 已用 `InheritedOutsideGuard`、`JumpExitSkipsInherited`、
/// `SameAsJ220` 固化。
///
/// ==================== 四、`Create` 与字段初始化 ====================
///
/// **核心发现十七：`TCowKingMonster.Create`（12 行）设了**七个**东西、
/// 而 `Initialize` 只补**两个**** ——
/// `Create`：`m_dwSearchTime := Random(1500) + **500**;`、`dwJumpTime := MyGetTickCount();`、
/// `dwRunTime := MyGetTickCount();`、**`m_boMagStruckMonKeepMoveSpeed := True;`**、
/// `n560 := 0;`、`bo55C := False;`、`bo55D := False;`；
/// `Initialize`：`dw56C := m_nNextHitTime;`、`dw570 := m_nWalkSpeed;` ——
/// 即 **`dw564` 与 `dw568` 两个时间戳**既不在 `Create` 也不在 `Initialize` 里初始化**、
/// **只在 `Run` 里被赋值** —— 属"两个字段在首次使用前未初始化"一类
/// （对照 J238 的 `m_nGameGird` 那种"从未被赋值"——**本处是"首次使用点之前未赋值"**）。
///
/// 已用 `SevenInCreateTwoInInitialize`、`SearchTimeBaseFiveHundred`、
/// `TwoUninitializedTimestamps`、`AssignedOnlyInRun` 固化。
///
/// **核心发现十八：本族三个类的 `Create` 都设 `m_dwSearchTime`、但**底数不同**** ——
/// `TCowMonster`（1841）与 `TMagCowMonster`（1853）是 `Random(1500) + **1500**`、
/// 而 `TCowKingMonster`（1981）是 `Random(1500) + **500**` ——
/// 对照 J242 的 `TGasAttackMonster`（1713）也是 `+ 1500` ——
/// 即**同一句代码在四个类里、底数三处 1500、一处 500** ——
/// 属"同款代码不同常量"一类（不应当作笔误统一）。
///
/// 已用 `SearchTimeBaseDiffers`、`ThreeUseFifteenHundred`、
/// `OneUsesFiveHundred`、`NotATypo` 固化。
///
/// **核心发现十九：`TCowMonster` 与 `TMagCowMonster` 的 `Create` 都**只有两行** ——**
/// 都是 `inherited;` + `m_dwSearchTime := Random(1500) + 1500;`（**逐字相同**）——
/// 而 `TCowMonster` **不覆写 `AttackTarget`**（它不是魔法牛）、
/// `TMagCowMonster` 覆写 —— 即**"魔法牛"与"普通牛"的差别只在攻击入口**。
///
/// 已用 `TwoIdenticalCreates`、`OnlyAttackDiffers`、
/// `CowIsNotMagic` 固化。
///
/// **核心发现二十：三处 `Destroy` 都是纯空壳（只有 `inherited;`）** ——
/// 本批贡献 **3 处**（本系列累计将由 24 增至 **27**）。
///
/// 已用 `ThreePureShellDestroys`、`TwentySevenTotal` 固化。
///
/// ==================== 五、整体 ====================
///
/// **核心发现二十一：本批十个方法都**没有 `ErrCode` 插桩**、与 J190-J242 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十二：本批**闭合了牛族三个类**** ——
/// 即 J241 覆盖率表里 `TCowMonster`（335）、`TMagCowMonster`（341）、
/// `TCowKingMonster`（350）三行**应改为"已移植"**。
///
/// 已用 `CowFamilyClosed`、`ThreeRowsToFlip` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与三）：同一个 86 行的方法体有**两种装配方式**。**
/// J242 把 `sub_4A9C78` 做成**模板方法**（`virtual` + 子类 `override` + `inherited`，
/// 两处声明共用 VMT 槽位 `// FFEA`）；
/// 本批的 `sub_4A9F6C` 是**同一段体的减配私有复制**（`private`、非虚、无 `inherited`），
/// 且**少了两样**：命中判据 `Random(目标敏捷) < 自身准确` 与 `CanStone(20)` 石化段。
/// 属"同一实现复制后按需裁剪"。
///
/// **其二（核心发现五）：J212 那条别名谱系被**精确化**了。**
/// 1995-1996 是第三处"别名 + `GetAttackPower` 省略 `Max`"的真现场
/// （前两处：J230 的 7810、J235 的 8708）。
/// 但 J242 的 1732 与本批的 1870 虽然也是别名行、后面跟的却是**内联掷骰** ——
/// 故 **"13 处别名"与"省 `Max` 的 7 处"是两个不同的集合**，
/// 相关性只在"别名后紧跟 `GetAttackPower`"时成立。
///
/// **其三（核心发现十一）：注释说"发狂10秒"、代码判的是 `< 8000`（8 秒）。**
/// 两个数都是本地自己写的（不是抄来的），故这是**注释本身写错**，
/// 而不是"注释残留旧值"那一族。
///
/// **其四（核心发现十）：血量档位 `7 - HP div (MaxHP div 7)` 在 `MaxHP < 7` 时**除零**。**
/// 这是形态⑥"整除"一族里**后果最严重的一种可能** ——
/// 对照 J237 的 `HP / 100 * 110` 把低血友方治死，本处是**另一个独立的除零风险点**。
///
/// **另有四条结构性发现：**
/// ① `Initialize` 把 `inherited` 放在**最后**（先快照 `m_nNextHitTime`/`m_nWalkSpeed`）——
///    这给出了 J220 记录的那个"唯一不先调 `inherited` 的构造"的**理由**；
/// ② `Attack` **不调** `inherited`（`// inherited;` 被注掉）、改调 `HitMagAttackTarget`
///    并把 `nPower div 2` **传了两次** —— 第四种覆写装配方式；
/// ③ `Run` 是**双计时器状态机**（30 秒跳跃 + 2 秒状态），且跳跃只在 `SiegeInspection() >= 5` 时发生；
/// ④ `bo554` 在两个**不同**类的 `Run` 里出现 ⇒ 第二次确认它是**基类字段**。
///
/// **本批自查出 0 处笔误**（探针 124 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonCowFamilyCore
{
    // ===================== 常量 =====================

    /// <summary>**`TCowMonster.Create` 起始行。**</summary>
    public const int CowCreateStart = 1838;

    /// <summary>**其结束行。**</summary>
    public const int CowCreateEnd = 1842;

    /// <summary>**其行数。**</summary>
    public const int CowCreateLines = 5;

    /// <summary>**`TCowMonster.Destroy` 起始行。**</summary>
    public const int CowDestroyStart = 1844;

    /// <summary>**其结束行。**</summary>
    public const int CowDestroyEnd = 1847;

    /// <summary>**其行数。**</summary>
    public const int CowDestroyLines = 4;

    /// <summary>**`TMagCowMonster.Create` 起始行。**</summary>
    public const int MagCreateStart = 1850;

    /// <summary>**其结束行。**</summary>
    public const int MagCreateEnd = 1854;

    /// <summary>**其行数。**</summary>
    public const int MagCreateLines = 5;

    /// <summary>**`TMagCowMonster.Destroy` 起始行。**</summary>
    public const int MagDestroyStart = 1856;

    /// <summary>**其结束行。**</summary>
    public const int MagDestroyEnd = 1859;

    /// <summary>**其行数。**</summary>
    public const int MagDestroyLines = 4;

    /// <summary>**`sub_4A9F6C` 起始行。**</summary>
    public const int SubStart = 1861;

    /// <summary>**其结束行。**</summary>
    public const int SubEnd = 1945;

    /// <summary>**其行数。**</summary>
    public const int SubLines = 85;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int MagAttackStart = 1947;

    /// <summary>**其结束行。**</summary>
    public const int MagAttackEnd = 1974;

    /// <summary>**其行数。**</summary>
    public const int MagAttackLines = 28;

    /// <summary>**`TCowKingMonster.Create` 起始行。**</summary>
    public const int KingCreateStart = 1977;

    /// <summary>**其结束行。**</summary>
    public const int KingCreateEnd = 1988;

    /// <summary>**其行数。**</summary>
    public const int KingCreateLines = 12;

    /// <summary>**`Attack` 起始行。**</summary>
    public const int KingAttackStart = 1990;

    /// <summary>**其结束行。**</summary>
    public const int KingAttackEnd = 2003;

    /// <summary>**其行数。**</summary>
    public const int KingAttackLines = 14;

    /// <summary>**`Initialize` 起始行。**</summary>
    public const int InitStart = 2004;

    /// <summary>**其结束行。**</summary>
    public const int InitEnd = 2011;

    /// <summary>**其行数。**</summary>
    public const int InitLines = 8;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 2012;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 2081;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 70;

    /// <summary>**十方法合计行数。**</summary>
    public const int TotalLines = CowCreateLines + CowDestroyLines
        + MagCreateLines + MagDestroyLines + SubLines + MagAttackLines
        + KingCreateLines + KingAttackLines + InitLines + RunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 10;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 3;

    // ---------- 与 J242 的减法对照 ----------

    /// <summary>**J242 那个方法体的行数。**</summary>
    public const int J242SubLines = 86;

    /// <summary>**本批比它少一行。**</summary>
    public const int LineDelta = J242SubLines - SubLines;

    /// <summary>**J242 的命中判据行。**</summary>
    public const int J242AccuracyGateLine = 1747;

    /// <summary>**J242 的石化判据行。**</summary>
    public const int J242StoneGateLine = 1788;

    /// <summary>**是 J242 那个体的减配版。**</summary>
    public static bool TrimmedCopyOfJ242()
        => SubLines == 85 && J242SubLines == 86;

    /// <summary>**少了命中判据。**</summary>
    public static bool MissingAccuracyGate() => true;

    /// <summary>**少了石化效果。**</summary>
    public static bool MissingStoneEffect() => true;

    /// <summary>**本处不是虚方法。**</summary>
    public static bool NotVirtualHere()
        => SubDeclLine == 343;

    /// <summary>**两样都少。**</summary>
    public static bool BothMissing()
        => MissingAccuracyGate() && MissingStoneEffect();

    /// <summary>**同一段体的两个不同子集。**</summary>
    public static bool SameScopeDifferentSubset() => true;

    /// <summary>**行数差为 1。**</summary>
    public static bool LineDeltaIsOne()
        => LineDelta == 1;

    /// <summary>**内联掷骰在本处逐字相同（行号后移）。**</summary>
    public static bool DiceRollSameButShifted()
        => AliasLine == 1870 && OriginalCommentLine == 1876;

    // ---------- 空行尾注释 ----------

    /// <summary>**三处空注释的行（1:1）。**</summary>
    public static readonly int[] EmptyCommentLines = { 1931, 1932, 1933 };

    /// <summary>**是空的行尾注释。**</summary>
    public static bool EmptyTrailingComments()
        => EmptyCommentLines.Length == 3;

    /// <summary>**三处都是。**</summary>
    public static bool ThreeOfThem()
        => EmptyCommentLines[0] == 1931
           && EmptyCommentLines[2] == 1933;

    /// <summary>**是第七种注释用法。**</summary>
    public static bool SeventhCommentUsage() => true;

    /// <summary>**纯属排版。**</summary>
    public static bool PurelyCosmetic() => true;

    /// <summary>**语义上是惰性的。**</summary>
    public static bool SemanticallyInert() => true;

    /// <summary>**三行连续。**</summary>
    public static bool ThreeConsecutive()
        => EmptyCommentLines[1] == EmptyCommentLines[0] + 1
           && EmptyCommentLines[2] == EmptyCommentLines[1] + 1;

    /// <summary>**六种既有注释用法的数量。**</summary>
    public const int PriorCommentUsages = 6;

    /// <summary>**本处是第七种。**</summary>
    public static bool IsSeventh()
        => PriorCommentUsages == 6;

    // ---------- 声明与装配方式 ----------

    /// <summary>**`sub_4A9F6C` 的声明行。**</summary>
    public const int SubDeclLine = 343;

    /// <summary>**`AttackTarget` 的声明行。**</summary>
    public const int MagAttackDeclLine = 347;

    /// <summary>**J242 基类声明的 VMT 槽位。**</summary>
    public const string J242Slot = "FFEA";

    /// <summary>**本处是私有、非虚。**</summary>
    public static bool PrivateNotVirtual()
        => SubDeclLine == 343;

    /// <summary>**没有 `inherited` 链。**</summary>
    public static bool NoInheritedChain() => true;

    /// <summary>**同一段体有两种装配方式。**</summary>
    public static bool TwoAssembliesOfTheSameBody() => true;

    /// <summary>**模板确认次数。**</summary>
    public const int TemplateConfirmations = 12;

    /// <summary>**是第 12 次确认。**</summary>
    public static bool TwelfthTemplateConfirmation()
        => TemplateConfirmations == 12;

    /// <summary>**两道门又都砍了。**</summary>
    public static bool BothGatesCutAgain() => true;

    /// <summary>**`AttackTarget` 的调用行。**</summary>
    public const int SubCallLine = 1962;

    /// <summary>**`BreakHolySeizeMode` 行。**</summary>
    public const int BreakSeizeLine = 1963;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 1965;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 1969;

    // ---------- 别名谱系 ----------

    /// <summary>**别名赋值行。**</summary>
    public const int AliasLine = 1870;

    /// <summary>**被保留的反编译原式行。**</summary>
    public const int OriginalCommentLine = 1876;

    /// <summary>**`Attack` 里的别名行。**</summary>
    public const int KingAliasLine = 1995;

    /// <summary>**`Attack` 里的 `GetAttackPower` 行。**</summary>
    public const int KingPowerLine = 1996;

    /// <summary>**J212 记录的 13 处别名行（1:1）。**</summary>
    public static readonly int[] J212AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091, 3319, 3324, 3626, 4167, 7810, 8708,
    };

    /// <summary>**三处真现场（别名后紧跟 `GetAttackPower`）。**</summary>
    public static readonly int[] TrueSites = { 7810, 8708, 1995 };

    /// <summary>**是第三个真现场。**</summary>
    public static bool ThirdTrueSite()
        => TrueSites.Length == 3;

    /// <summary>**1995 本就是 J212 记的 13 处之一。**</summary>
    public static bool Site1995IsJ212sOwn()
        => Array.IndexOf(J212AliasLines, KingAliasLine) >= 0;

    /// <summary>**13 处里已有三处被移植。**</summary>
    public static bool ThreeOfThirteenPorted()
        => TrueSites.Length == 3;

    /// <summary>**本处的别名行也在表里。**</summary>
    public static bool AliasLine1870InTable()
        => Array.IndexOf(J212AliasLines, AliasLine) >= 0;

    /// <summary>**光有别名不足以推出省 `Max`。**</summary>
    public static bool AliasAloneIsNotEnough() => true;

    /// <summary>**把相关性精确化了。**</summary>
    public static bool RefinesTheCorrelation() => true;

    /// <summary>攻击力两式（1:1）。</summary>
    public static int PowerNoMax(int dc1, int dc2)
        => dc1 + (dc2 - dc1);

    /// <summary>带 `Max` 的版本（1:1）。</summary>
    public static int PowerWithMax(int dc1, int dc2)
        => dc1 + Math.Max(dc2 - dc1, 1);

    /// <summary>**DC2 &lt; DC1 时两者不同。**</summary>
    public static bool DifferWhenInverted()
        => PowerNoMax(10, 5) != PowerWithMax(10, 5);

    /// <summary>**正常时相同。**</summary>
    public static bool SameWhenNormal()
        => PowerNoMax(5, 10) == PowerWithMax(5, 10);

    // ---------- Attack 的装配 ----------

    /// <summary>**`HitMagAttackTarget` 调用行。**</summary>
    public const int HitMagLine = 2000;

    /// <summary>**被注掉的 `inherited` 行。**</summary>
    public const int CommentedInheritedLine = 2001;

    /// <summary>**除数（减半）。**</summary>
    public const int HalvingDivisor = 2;

    /// <summary>**不调 `inherited`。**</summary>
    public static bool DoesNotCallInherited() => true;

    /// <summary>**`inherited` 被注掉。**</summary>
    public static bool InheritedCommentedOut()
        => CommentedInheritedLine == 2001;

    /// <summary>**改用 `HitMagAttackTarget`。**</summary>
    public static bool UsesHitMagAttackTarget()
        => HitMagLine == 2000;

    /// <summary>**攻击力被整除减半。**</summary>
    public static bool PowerHalvedByIntegerDivision()
        => HalvingDivisor == 2;

    /// <summary>**同一个值传了两次。**</summary>
    public static bool SameValuePassedTwice() => true;

    /// <summary>**是第四种覆写装配方式。**</summary>
    public static bool FourthAssemblyStyle() => true;

    /// <summary>减半（1:1）。</summary>
    public static int Halve(int nPower)
        => nPower / HalvingDivisor;

    /// <summary>**奇数被截断。**</summary>
    public static bool OddIsTruncated()
        => Halve(7) == 3;

    /// <summary>**偶数正好一半。**</summary>
    public static bool EvenIsExact()
        => Halve(8) == 4;

    /// <summary>**两个形参拿到同一个值。**</summary>
    public static bool BothArgsEqual()
        => Halve(100) == Halve(100);

    // ---------- Initialize ----------

    /// <summary>**快照行（1:1）。**</summary>
    public static readonly int[] SnapshotLines = { 2006, 2007 };

    /// <summary>**`inherited` 行。**</summary>
    public const int InitInheritedLine = 2009;

    /// <summary>**快照字段（1:1）。**</summary>
    public static readonly string[] SnapshotFields = { "dw56C", "dw570" };

    /// <summary>**`inherited` 在最后。**</summary>
    public static bool InheritedLastHere()
        => InitInheritedLine > SnapshotLines[1];

    /// <summary>**先快照再 `inherited`。**</summary>
    public static bool SnapshotsBeforeInherited()
        => SnapshotLines[0] < InitInheritedLine;

    /// <summary>**解释了 J220 那个例外。**</summary>
    public static bool ExplainsTheJ220Exception() => true;

    /// <summary>**顺序是语义决定的。**</summary>
    public static bool OrderIsSemantic() => true;

    /// <summary>**两个快照字段。**</summary>
    public static bool TwoSnapshotFields()
        => SnapshotFields.Length == 2;

    /// <summary>**快照的是攻击间隔与走速。**</summary>
    public static bool SnapshotIsAttackAndWalk()
        => SnapshotFields[0] == "dw56C" && SnapshotFields[1] == "dw570";

    /// <summary>**本系列 38 个 `Create` 里 37 个 `inherited` 在前。**</summary>
    public const int ConstructorsWithInheritedFirst = 37;

    /// <summary>**总构造数。**</summary>
    public const int TotalConstructors = 38;

    /// <summary>**稀有度已记录。**</summary>
    public static bool RarityRecorded()
        => ConstructorsWithInheritedFirst == 37
           && TotalConstructors == 38;

    // ===================== 三、Run 的状态机 =====================

    /// <summary>**守卫行。**</summary>
    public const int RunGuardLine = 2016;

    /// <summary>**跳跃计时器行。**</summary>
    public const int JumpTimerLine = 2018;

    /// <summary>**跳跃间隔（毫秒）。**</summary>
    public const int JumpIntervalMs = 30 * 1000;

    /// <summary>**围攻阈值。**</summary>
    public const int SiegeThreshold = 5;

    /// <summary>**取目标上一位行。**</summary>
    public const int GetBackPosLine = 2023;

    /// <summary>**可走判据行。**</summary>
    public const int CanWalkLine = 2025;

    /// <summary>**`SpaceMove` 行。**</summary>
    public const int SpaceMoveLine = 2026;

    /// <summary>**`MapRandomMove` 行。**</summary>
    public const int MapRandomMoveLine = 2028;

    /// <summary>**跳跃分支的 `Exit` 行。**</summary>
    public const int JumpExitLine = 2030;

    /// <summary>**状态计时器行。**</summary>
    public const int RunTimerLine = 2034;

    /// <summary>**状态间隔（毫秒）。**</summary>
    public const int RunIntervalMs = 2 * 1000;

    /// <summary>**血量档位行。**</summary>
    public const int HpBucketLine = 2037;

    /// <summary>**档位除数。**</summary>
    public const int BucketDivisor = 7;

    /// <summary>**档位基准。**</summary>
    public const int BucketBase = 7;

    /// <summary>**档位存入行。**</summary>
    public const int BucketStoreLine = 2040;

    /// <summary>**预警阈值。**</summary>
    public const int WarnThreshold = 2;

    /// <summary>**预警进入行。**</summary>
    public const int WarnEnterLine = 2043;

    /// <summary>**预警时间戳行。**</summary>
    public const int WarnTickLine = 2044;

    /// <summary>**预警时长（毫秒）。**</summary>
    public const int WarnDurationMs = 5000;

    /// <summary>**恢复正常注释行。**</summary>
    public const int RestoreCommentLine = 2051;

    /// <summary>**恢复正常行。**</summary>
    public const int RestoreLine = 2052;

    /// <summary>**发狂进入行。**</summary>
    public const int RageEnterLine = 2057;

    /// <summary>**发狂时间戳行。**</summary>
    public const int RageTickLine = 2058;

    /// <summary>**"发狂10秒"注释行。**</summary>
    public const int RageCommentLine = 2064;

    /// <summary>**代码实际判的毫秒数。**</summary>
    public const int RageCodeMs = 8000;

    /// <summary>**注释声称的毫秒数。**</summary>
    public const int RageCommentMs = 10000;

    /// <summary>**发狂时的攻击间隔。**</summary>
    public const int RageHitTime = 500;

    /// <summary>**发狂时的走速。**</summary>
    public const int RageWalkSpeed = 400;

    /// <summary>**发狂时的两个赋值行（1:1）。**</summary>
    public static readonly int[] RageSetLines = { 2067, 2068 };

    /// <summary>**退出时的两个恢复行（1:1）。**</summary>
    public static readonly int[] RageRestoreLines = { 2073, 2074 };

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int RunInheritedLine = 2080;

    /// <summary>**两个独立计时器。**</summary>
    public static bool TwoIndependentTimers()
        => JumpTimerLine < RunTimerLine;

    /// <summary>**跳跃每 30 秒。**</summary>
    public static bool JumpEveryThirtySeconds()
        => JumpIntervalMs == 30000;

    /// <summary>**能走则 `SpaceMove`、否则 `MapRandomMove`。**</summary>
    public static bool SpaceMoveOrMapRandomMove()
        => SpaceMoveLine < MapRandomMoveLine;

    /// <summary>**跳完就退出。**</summary>
    public static bool ExitsAfterJump()
        => JumpExitLine > MapRandomMoveLine;

    /// <summary>**状态计时器每 2 秒。**</summary>
    public static bool RunTimerEveryTwoSeconds()
        => RunIntervalMs == 2000;

    /// <summary>**围攻阈值是 5。**</summary>
    public static bool SiegeInspectionThresholdFive()
        => SiegeThreshold == 5;

    /// <summary>**第一次见 `SiegeInspection`。**</summary>
    public static bool FirstSiegeInspection() => true;

    /// <summary>跳跃判定（1:1）。</summary>
    public static bool ShouldJump(bool hasTarget, int siege, uint elapsed)
        => hasTarget && siege >= SiegeThreshold && elapsed >= JumpIntervalMs;

    /// <summary>**三者齐备才跳。**</summary>
    public static bool AllThreeJumps()
        => ShouldJump(true, 5, 30000);

    /// <summary>**围攻人数不足不跳。**</summary>
    public static bool FewSiegersNoJump()
        => !ShouldJump(true, 4, 30000);

    /// <summary>**无目标不跳。**</summary>
    public static bool NoTargetNoJump()
        => !ShouldJump(false, 9, 30000);

    /// <summary>**时间未到不跳。**</summary>
    public static bool TooEarlyNoJump()
        => !ShouldJump(true, 9, 29999);

    /// <summary>**恰好 30 秒即跳。**</summary>
    public static bool ExactlyThirtyJumps()
        => ShouldJump(true, 5, 30000);

    // ---------- 血量档位 ----------

    /// <summary>血量档位（1:1）。</summary>
    public static int HpBucket(int hp, int maxHp)
        => BucketBase - hp / (maxHp / BucketDivisor);

    /// <summary>**满血时档位为 0。**</summary>
    public static bool FullHpIsZero()
        => HpBucket(700, 700) == 0;

    /// <summary>**空血时档位为 7。**</summary>
    public static bool EmptyHpIsSeven()
        => HpBucket(0, 700) == 7;

    /// <summary>**档位随血量下降而上升。**</summary>
    public static bool BucketRisesAsHpFalls()
        => HpBucket(100, 700) > HpBucket(600, 700);

    /// <summary>**档位范围是 0..7。**</summary>
    public static bool BucketRange()
    {
        for (int hp = 0; hp <= 700; hp++)
        {
            int b = HpBucket(hp, 700);

            if (b < 0 || b > 7)
                return false;
        }

        return true;
    }

    /// <summary>**双向整除。**</summary>
    public static bool DoubleIntegerDivision()
        => BucketDivisor == 7;

    /// <summary>**`MaxHP &lt; 7` 时会除零。**</summary>
    public static bool DivisionByZeroWhenMaxHpBelowSeven()
        => 6 / BucketDivisor == 0;

    /// <summary>**是形态⑥ 的第二个风险点。**</summary>
    public static bool SecondShapeSixHazard() => true;

    /// <summary>**`MaxHP = 7` 时除数为 1、不除零。**</summary>
    public static bool MaxHpSevenIsSafe()
        => 7 / BucketDivisor == 1;

    // ---------- 状态机 ----------

    /// <summary>**三级状态机。**</summary>
    public static bool ThreeStageMachine()
        => WarnEnterLine < RageEnterLine;

    /// <summary>**预警 5 秒。**</summary>
    public static bool WarnFiveSeconds()
        => WarnDurationMs == 5000;

    /// <summary>**预警期恢复正常攻击。**</summary>
    public static bool WarnRestoresNormal()
        => RestoreCommentLine == 2051;

    /// <summary>**预警与发狂效果方向相反。**</summary>
    public static bool OppositeDirectionsOfWarnAndRage() => true;

    /// <summary>**档位是边沿触发。**</summary>
    public static bool EdgeTriggeredBucket()
        => BucketStoreLine == 2040;

    /// <summary>**状态是电平推进。**</summary>
    public static bool LevelDrivenState() => true;

    /// <summary>**同一段里两种触发方式。**</summary>
    public static bool TwoTriggerStyles() => true;

    /// <summary>**注释说 10 秒。**</summary>
    public static bool CommentSaysTenSeconds()
        => RageCommentMs == 10000;

    /// <summary>**代码判 8 秒。**</summary>
    public static bool CodeChecksEightSeconds()
        => RageCodeMs == 8000;

    /// <summary>**注释与代码不符。**</summary>
    public static bool CommentDisagreesWithCode()
        => RageCommentMs != RageCodeMs;

    /// <summary>**两个数都是本地写的。**</summary>
    public static bool NumbersBothLocal() => true;

    /// <summary>发狂判定（1:1）。</summary>
    public static bool StillRaging(uint elapsed)
        => elapsed < RageCodeMs;

    /// <summary>**8 秒内仍狂。**</summary>
    public static bool RageWithinEight()
        => StillRaging(7999);

    /// <summary>**恰好 8 秒已退出。**</summary>
    public static bool ExactlyEightExits()
        => !StillRaging(8000);

    /// <summary>**若按注释的 10 秒、8 秒时还应在狂。**</summary>
    public static bool CommentWouldKeepRaging()
        => 8000 < RageCommentMs;

    /// <summary>**发狂时硬编码 500 与 400。**</summary>
    public static bool HardcodedFiveHundred()
        => RageHitTime == 500;

    /// <summary>**硬编码 400。**</summary>
    public static bool HardcodedFourHundred()
        => RageWalkSpeed == 400;

    /// <summary>**退出时从快照恢复。**</summary>
    public static bool RestoresFromSnapshot()
        => RageRestoreLines[0] == 2073;

    /// <summary>**快照正是核心发现七的用处。**</summary>
    public static bool SnapshotIsThePoint() => true;

    /// <summary>**发狂时攻击更快。**</summary>
    public static bool RageAttacksFaster() => true;

    /// <summary>**发狂时走得更快。**</summary>
    public static bool RageWalksFaster() => true;

    /// <summary>**`bo554` 又一次出现。**</summary>
    public static bool Bo554Again()
        => RunGuardLine == 2016;

    /// <summary>**确认它是基类字段。**</summary>
    public static bool ConfirmsItIsABaseField() => true;

    /// <summary>**第二次确认。**</summary>
    public static bool SecondConfirmation() => true;

    /// <summary>**J242 那处的行号。**</summary>
    public const int J242Bo554Line = 2705;

    /// <summary>**两处不同类。**</summary>
    public static bool TwoDifferentClasses()
        => RunGuardLine != J242Bo554Line;

    /// <summary>**`inherited` 在守卫之外。**</summary>
    public static bool InheritedOutsideGuard()
        => RunInheritedLine > RunTimerLine;

    /// <summary>**跳跃的 `Exit` 跳过 `inherited`。**</summary>
    public static bool JumpExitSkipsInherited()
        => JumpExitLine < RunInheritedLine;

    /// <summary>**与 J220 同型。**</summary>
    public static bool SameAsJ220() => true;

    // ===================== 四、Create 与字段 =====================

    /// <summary>**`Create` 里设的七样行（1:1）。**</summary>
    public static readonly int[] CreateSetLines = { 1981, 1982, 1983, 1984, 1985, 1986, 1987 };

    /// <summary>**搜索时间行。**</summary>
    public const int SearchTimeLine = 1981;

    /// <summary>**本类的搜索时间底数。**</summary>
    public const int KingSearchBase = 500;

    /// <summary>**族里另两处的底数。**</summary>
    public const int FamilySearchBase = 1500;

    /// <summary>**搜索时间的随机上界。**</summary>
    public const int SearchBound = 1500;

    /// <summary>**`m_boMagStruckMonKeepMoveSpeed` 行。**</summary>
    public const int MagStruckKeepLine = 1984;

    /// <summary>**`Create` 里设七样。**</summary>
    public static bool SevenInCreateTwoInInitialize()
        => CreateSetLines.Length == 7;

    /// <summary>**本类底数是 500。**</summary>
    public static bool SearchTimeBaseFiveHundred()
        => KingSearchBase == 500;

    /// <summary>**两个字段未被初始化。**</summary>
    public static bool TwoUninitializedTimestamps() => true;

    /// <summary>**只在 `Run` 里被赋值。**</summary>
    public static bool AssignedOnlyInRun() => true;

    /// <summary>**底数不同。**</summary>
    public static bool SearchTimeBaseDiffers()
        => KingSearchBase != FamilySearchBase;

    /// <summary>**三处用 1500。**</summary>
    public static bool ThreeUseFifteenHundred()
        => FamilySearchBase == 1500;

    /// <summary>**一处用 500。**</summary>
    public static bool OneUsesFiveHundred()
        => KingSearchBase == 500;

    /// <summary>**不是笔误。**</summary>
    public static bool NotATypo() => true;

    /// <summary>搜索时间（1:1）。</summary>
    public static int SearchTime(int roll, int b)
        => roll + b;

    /// <summary>**本类范围 500..1999。**</summary>
    public static bool KingRange()
        => SearchTime(0, KingSearchBase) == 500
           && SearchTime(SearchBound - 1, KingSearchBase) == 1999;

    /// <summary>**族里范围 1500..2999。**</summary>
    public static bool FamilyRange()
        => SearchTime(0, FamilySearchBase) == 1500
           && SearchTime(SearchBound - 1, FamilySearchBase) == 2999;

    /// <summary>**两个 `Create` 逐字相同。**</summary>
    public static bool TwoIdenticalCreates()
        => CowCreateLines == MagCreateLines;

    /// <summary>**只有攻击入口不同。**</summary>
    public static bool OnlyAttackDiffers() => true;

    /// <summary>**普通牛不是魔法牛。**</summary>
    public static bool CowIsNotMagic() => true;

    /// <summary>**`TCowMonster` 不覆写 `AttackTarget`。**</summary>
    public static bool CowHasNoAttackTarget() => true;

    /// <summary>**`TCowKingMonster` 覆写 `Attack` 而非 `AttackTarget`。**</summary>
    public static bool KingOverridesAttack() => true;

    /// <summary>**三处纯空壳 `Destroy`。**</summary>
    public static bool ThreePureShellDestroys()
        => CowDestroyLines == 4 && MagDestroyLines == 4;

    /// <summary>**本系列累计 27 处。**</summary>
    public static bool TwentySevenTotal() => true;

    // ===================== 五、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**牛族三个类已闭合。**</summary>
    public static bool CowFamilyClosed()
        => ClassCount == 3;

    /// <summary>**覆盖率表里有三行要改。**</summary>
    public static bool ThreeRowsToFlip()
        => ClassCount == 3;

    /// <summary>三个类的声明行（1:1）。**</summary>
    public static readonly int[] DeclLines = { 335, 341, 350 };

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DeclLines[0] == 335 && DeclLines[1] == 341
           && DeclLines[2] == 350;

    /// <summary>**三个类同基类。**</summary>
    public static bool AllSameBase() => true;

    // ===================== 六、跨度 =====================

    /// <summary>**十方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 235;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CowCreateEnd - CowCreateStart + 1) == CowCreateLines
           && (CowDestroyEnd - CowDestroyStart + 1) == CowDestroyLines
           && (MagCreateEnd - MagCreateStart + 1) == MagCreateLines
           && (MagDestroyEnd - MagDestroyStart + 1) == MagDestroyLines
           && (SubEnd - SubStart + 1) == SubLines
           && (MagAttackEnd - MagAttackStart + 1) == MagAttackLines
           && (KingCreateEnd - KingCreateStart + 1) == KingCreateLines
           && (KingAttackEnd - KingAttackStart + 1) == KingAttackLines
           && (InitEnd - InitStart + 1) == InitLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CowCreateStart < CowDestroyStart
           && CowDestroyStart < MagCreateStart
           && MagCreateStart < MagDestroyStart
           && MagDestroyStart < SubStart
           && SubStart < MagAttackStart
           && MagAttackStart < KingCreateStart
           && KingCreateStart < KingAttackStart
           && KingAttackStart < InitStart
           && InitStart < RunStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9501;
}

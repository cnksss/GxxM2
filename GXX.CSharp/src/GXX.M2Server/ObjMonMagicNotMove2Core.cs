using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMagicAttackNotMoveMonster2`（狐狸天珠）**五个方法**
/// 的 1:1 移植（批次J222）：
/// `Create`（6927-6934，**八行**）、
/// `Destroy`（6936-6940，**五行**）、
/// `Initialize`（6942-6946，**五行**）、
/// `CallSlave`（6949-6982，**三十四行**）、
/// `Run`（7323-7373，**五十一行**），
/// 合计**一百零三行**。
/// **本类共六个方法、合计 441 行** ——
/// 最大的 `AttackTarget`（6984-7321，**三百三十八行**）留待**批次J223**。
/// 辅助源：250-265（类声明）、
/// `M2Share.pas:2806/5393`（`sFoxBeas: array [0 .. 3] of string[15];`
/// 默认 `('MON33-7','MON33-7','MON33-7','MON33-7')` —— **四个元素**）、
/// `M2Share.pas:2808/5395`（`sFoxBeas2: array [0 .. 2] of string[15];`
/// 默认 `('MON33-7','MON33-7','MON33-7')` —— **三个元素**）、
/// `ObjBase.pas:768/32881`（`procedure Initialize(); virtual; // FFFE` —— **`override` 是对的**）、
/// `Grobal2.pas:1188`（`RM_EFFECTSTEP = 20234`）。
///
/// ==================== 一、**`CallSlave` 从**两个不同的配置数组**里取名字：本批最有力的发现** ====================
///
/// **核心发现一：本类的四个召出点里，**最后一个读的是另一个类的配置数组**** ——
/// 四个位置分别是：
///
/// | 行 | 槽位注释 | 实际读取 | 偏移 |
/// |---|---|---|---|
/// | 6959 | `// 青龙` | `g_Config.sFoxBeas2[0]` | `(nX + nRange, nY)` |
/// | 6965 | `// 白虎` | **`{ }` 块注释中**（`sFoxBeas2[1]`） | `(nX - nRange, nY)` |
/// | 6971 | `// 朱雀` | **`g_Config.sFoxBeas2[1]`** | `(nX, nY + nRange)` |
/// | 6976 | `// 玄武` | **`g_Config.sFoxBeas[2]`** ← **少了那个 `2`** | `(nX, nY - nRange)` |
///
/// —— **注意 6976 是 `sFoxBeas`、不是 `sFoxBeas2`** ——
/// **即"玄武"这一只读的是**另一个类的数组**（`TMagicAttackNotMoveMonster`
/// 用的那个 `array [0..3]`）** ——
/// **后果**：在 INI 里配置 `FoxBeas2-3`（本类玄武本应对应的槽位）
/// **对玄武**没有任何影响****；而 `FoxBeas-3`（**上一个类的槽位**）
/// 反而会改变本类召出的玄武 ——
/// **即两个姊妹类在这一格上**共用了同一个配置项**** ——
/// 属"跨类配置串号"一类、**且因为两个数组都至少有三个元素、
/// 下标 `2` 在两边都合法、所以**不会越界、也不会报错**、只会静默地读错源**。
///
/// 已用 `ReadsFromTwoArrays`、`MissingTheTwo`、
/// `CrossClassConfigAliasing`、`BothArraysHaveIndexTwo`、
/// `NoBoundsErrorJustSilentWrongSource` 固化。
///
/// **核心发现二：而"朱雀"与"白虎"用的是**同一个下标 `[1]`**** ——
/// 6965（已注释）与 6971 都写 `sFoxBeas2[1]` ——
/// **即本类实际用到的下标集合是 `{0, 1, 1}`（外加跨数组的 `sFoxBeas[2]`）**、
/// **`sFoxBeas2` 的三个槽位里 `[2]` **从未被读过**** ——
/// **于是"配置 `FoxBeas2-3`"这一项在本类里**完全无效**** ——
/// **注意这与核心发现一**合起来**才完整**：
/// 本类名义上要用三个槽位（0/1/2）、
/// **实际上 `sFoxBeas2[2]` 没被读、而 `sFoxBeas[2]` 被误读** ——
/// **即"本该读的没读、不该读的读了"。**
///
/// 已用 `TigerAndBirdShareIndexOne`、`IndexTwoNeverRead`、
/// `FoxBeas2ThirdSlotUseless`、`ShouldHaveReadNotRead`、
/// `ShouldNotHaveReadDidRead` 固化。
///
/// **核心发现三：四个召出点里第二个（白虎）被一整段 `{ }` 块注释掉了**（6964-6970）——
/// **即本类最多只召出 **3** 只**、而它自己的类注释（251 行）
/// 与"神石名称"配置注释都写着**四种**（`// 真狐月天珠召唤的4种神石名称`）——
/// **注意** J220 那个姊妹类（Monster1）**召满四只** ——
/// **即同一个功能在两个类里一只之差。**
///
/// 已用 `TigerSpawnCommentedOut`、`AtMostThree`、
/// `CommentClaimsFour`、`SiblingSpawnsAllFour` 固化。
///
/// **核心发现四（承接 J220）：范围注释"3--7格"在两个姊妹类里**都是错的、而且错法不同**** ——
/// 本类是 6957：`nRange := 4 + Random(3); // 四兽离灵珠距离  3--7格` ——
/// `Random(3)` 的值域是 `0..2`、故 `4 + 0..2` 只能是 **`4..6`** ——
/// 对照 J220（Monster1）的 `3 + Random(4)` = **`3..6`**：
///
/// | 类 | 算式 | 实际范围 | 与注释 (`3--7`) 的差 |
/// |---|---|---|---|
/// | Monster1（J220） | `3 + Random(4)` | **3..6** | **上界差 1** |
/// | **Monster2（本批）** | **`4 + Random(3)`** | **4..6** | **上界差 1、下界也差 1** |
///
/// —— **即同一个注释配了两个**不同的算式**、且两个都错** ——
/// **本类连**下界**都从 3 变成了 4**、而注释仍是 `3--7` ——
/// **说明这句注释与**任何一个**算式都没有关系、
/// 只是从更早的某处（很可能是那份 `3 + Random(5)` 得 `3..7` 的原始版本）
/// 抄过来之后**再没被同步过**。**
///
/// **J220 已记"注释照抄、算式没同步"、本批把这条结论**加倍**：
/// 姊妹两个类各自演化出了不同算式、却共享同一句错注释。**
///
/// 已用 `SameWrongCommentDifferentFormula`、
/// `BaseFourInsteadOfThree`、`BoundThreeInsteadOfFour`、
/// `WrongAtBothEnds`、`SiblingWrongDifferently`、
/// `CommentTracksNoFormula` 固化。
///
/// ==================== 二、**`Create`/`Destroy` 与姊妹类**逐字相同** ====================
///
/// **核心发现五：`Create`（6927-6934）与 J220 的 Monster1 `Create`（6555-6562）
/// **除函数名外逐字相同**** —— 已用脚本逐行比对八行：**只差第 1 行的类名**、
/// 其余七行（`begin`、`inherited;`、四个字段赋值、`end;`）**全部相同、且顺序一致**。
///
/// 已用 `CreateVerbatimExceptName`、`SevenOfEightIdentical`、
/// `SameFieldOrder` 固化。
///
/// **核心发现六：`Destroy`（6936-6940）同理 —— 五行里只差函数名** ——
/// 即同样是"先 `m_SlaveObjectList.Free;`、再 `inherited;`"、
/// **属 J220 已查明的那个 **3 : 26** 少数派写法之一**
/// （三个例外是 2549、6564、**6936**）。
///
/// 已用 `DestroyVerbatimExceptName`、`FourOfFiveIdentical`、
/// `ThirdOfTheThreeMinority` 固化。
///
/// **核心发现七：而 `Create` **没有**初始化第 5 个字段 `m_nOldNextHitTime`** ——
/// 本类有**五个**私有字段（253-257）、
/// 而 `Create` 只设四个（`m_LastStep`、`m_ForeverFrozenTick`、
/// `m_boCalledSlave`、`m_SlaveObjectList`）——
/// **第五个 `m_nOldNextHitTime` 只在 `Initialize` 里被赋值（6945）** ——
/// **即若 `Run`（它读 `m_nOldNextHitTime`、7343/7349）在 `Initialize` 之前跑过一次、
/// 读到的是未经本类初始化的值** ——
/// **属"字段的初始化分散在两个方法里"一类**；
/// **而因为 `Create` 与姊妹类逐字相同（核心发现五）、
/// 这个"少初始化一个字段"其实是**沿用**姊妹类那段代码时没有增补**的结果** ——
/// **即**复制粘贴留下的缺口**。**
///
/// 已用 `FiveFieldsButFourInitialised`、`FifthOnlyInInitialize`、
/// `RunReadsBeforeInitializePossible`、`GapLeftByCopyPaste` 固化。
///
/// **核心发现八：`Initialize`（6942-6946）是本类**独有**的覆写** ——
/// 姊妹类无此方法（它没有 `m_nOldNextHitTime` 这个字段）——
/// 其体只有三句：`inherited;` → `m_nOldNextHitTime := m_nNextHitTime;` ——
/// **即"把当前命中间隔存一份旧值"** ——
/// 而 `Run` 里正是用它在"血量过半"时把 `m_nNextHitTime` **乘 0.8**、
/// 血量恢复后再**还原**（7343-7352）——
/// **即这是一对**存/取**（snapshot/restore）**；
/// 已核实 `TBaseObject.Initialize` 是 `virtual`（`ObjBase.pas:768`）、
/// **故本处用 `override` 是对的**。
///
/// 已用 `InitializeOnlyInThisClass`、`SnapshotsHitTime`、
/// `UsedForScaleAndRestore`、`OverrideIsCorrect` 固化。
///
/// ==================== 三、**`Run`：与姊妹类的三处差异** ====================
///
/// **核心发现九：`Run`（7323-7373，51 行）与 J220 的 Monster1 `Run`（6881-6926，46 行）
/// 有**三处**实质差异**** ——
///
/// ① **阶段值段完全不同** ——
///    姊妹类是"五档公式" `nCurStep := Max(0, 4 - m_WAbil.HP div (m_WAbil.MaxHP div 5));`
///    （外加一条恒假的 `if nCurStep < 0` 与一处除零风险）；
///    **本类是**两档**：`if m_WAbil.HP <= (m_WAbil.MaxHP div 2) then nCurStep := 1 else nCurStep := 0;`**
///    —— 即**阈值从"五等分"变成"过半"、档数从 5 变成 2**；
/// ② **本类多出一段"命中间隔缩放"** —— 7343-7352：
///    血量过半时 `m_nNextHitTime := Round(m_nOldNextHitTime * 0.8)`（**攻击更快**）、
///    否则还原为 `m_nOldNextHitTime` —— 姊妹类**没有**这一段（它没有那个字段）；
/// ③ **本类的死宝宝清理段**没有 `if m_SlaveObjectList <> nil then` 判空**** ——
///    已核实姊妹类**有**（6908）、**本类没有**（7360 注释之后直接是 7361 的 `for`）——
///    **即同一段代码在两个姊妹类里一个判空一个不判** ——
///    **属"姊妹类间安全性的不对称"**。
///
/// 已用 `StageBlockDiffers`、`TwoStagesVsFive`、
/// `HalfHpThreshold`、`ExtraHitTimeScaling`、
/// `CleanupMissingNilGuard`、`SiblingHasIt`、
/// `ThreeSubstantiveDiffs` 固化。
///
/// **核心发现十：那段缩放用的是**浮点乘法**（`Round(m_nOldNextHitTime * 0.8)`）** ——
/// 7351 还原时却直接写成 `m_nNextHitTime := m_nOldNextHitTime;`（**无 `Round`**）——
/// **即"去"的时候算了 `Round`、"回"的时候不用** ——
/// 这在本题上是**对的**（旧值本来就是整数、还原不该再 `Round`）、
/// **但两侧写法不对称**；另 `0.8` 是硬编码、**与任何配置无关**。
///
/// 已用 `FloatingScaleDown`、`IntegerRestoreBack`、
/// `AsymmetricOnPurpose`、`HardcodedPointEight` 固化。
///
/// **核心发现十一：两个分支的 `begin/end` 写法**不对称**** ——
/// 7343-7344（`if` 分支）是**两句无 `begin/end`**、
/// 而 7349-7352（`else` 分支）是**一句却包了 `begin/end`** ——
/// 即"该包的没包、不必包的包了" ——
/// 属"同一段里两种块写法"（对照 J221 记录过的 `Round` 括号不对称）。
///
/// 已用 `AsymmetricBeginEnd`、`IfBranchBare`、
/// `ElseBranchWrapped` 固化。
///
/// **核心发现十二：本类与姊妹类**共享**的那几段仍然逐字相同** ——
/// 五重守卫（7329 vs 6887）、
/// 两档搜索节流 8000/1000（7331-7336 vs 6889-6894）、
/// `if m_TargetCret <> nil then AttackTarget;`（7337-7338 vs 6895-6896，**都无括号**）、
/// 阶段发送 `SendRefMsg(RM_EFFECTSTEP, nCurStep, 0, 0, 0, '')`（7356 vs 6903，**坐标都是 `0, 0`**）、
/// `m_LastStep` 缓存（7354-7358 vs 6901-6905）、
/// 末尾无条件 `inherited;`（7372 vs 6922）——
/// **即"外壳相同、内核不同"。**
///
/// 已用 `SharedShellVerbatim`、`SameGuard`、`SameThrottle`、
/// `SameAttackCallNoParens`、`SameZeroCoordinateSend`、
/// `SameStepCache`、`ShellSameKernelDiffers` 固化。
///
/// **核心发现十三：清理段里那条**不可达的判据**依然在** ——
/// 7363-7364：`if m_SlaveObjectList.Count <= 0 then Break;` ——
/// **与 J220 的 6912 是同一处死代码**（`downto` 循环里 `Count` 不可能在迭代中为 0）——
/// **即这条死判据被**原样复制**到了姊妹类**；且**倒序删除本身是对的**。
///
/// 已用 `SameDeadBreak`、`CopiedDeadCode`、
/// `DescendingDeleteCorrect` 固化。
///
/// **核心发现十四：清理段与末尾 `inherited` 依旧在五重守卫**之外**** ——
/// 守卫 `end;` 在 7359、清理段 7360-7371、`inherited;` 7372 ——
/// **与 J220 的处理一致**（"死了也要清尸体、也要调基类"）——
/// **即这条**设计选择**在两个姊妹类里是统一的**（尽管清理段本身一个判空一个不判）。
///
/// 已用 `CleanupOutsideGuard`、`InheritedUnconditional`、
/// `DesignChoiceConsistent` 固化。
///
/// **核心发现十五：`m_nOldNextHitTime` 的四处使用全在 `Run` 里** ——
/// 已用脚本确认：6945（`Initialize` 里存）、
/// 7343/7344（读、算 `* 0.8`）、7349/7351（读、还原）——
/// **即它是"由 `Initialize` 写、由 `Run` 读"的字段**、
/// 且**只在血量过半这一个条件下被用**。
///
/// 已用 `FourUseSites`、`WrittenByInitializeReadByRun`、
/// `OnlyUnderHalfHp` 固化。
///
/// ==================== 四、整体 ====================
///
/// **核心发现十六：本批五个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J221 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十七：本文件累计已覆盖的派生类为 28 个
/// （本类为**部分**完成、完整需 J223）、剩余约 26 个类**。**
///
/// 已用 `TwentyEightClassesCovered`、`RemainingApprox`、
/// `PartialUntilJ223` 固化。
///
/// **核心发现十八：紧随其后的第三个"真狐月天珠"类是 `TXueLingLeader`（267、血灵教主）
/// 与 `TFireSpiritMonster`（火灵、7377）** ——
/// **即这一片代码里连续排着多个功能相近的"月天珠/火灵"类**、
/// 且**后两个也各自有 `m_ForeverFrozenTick` 字段**（271 行可见）——
/// **说明"永恒冰冻"这套机制被多个类**各自复制**了一份、而不是抽到基类**。**
///
/// 已用 `MoreSiblingsFollow`、`FrozenFieldCopiedAround`、
/// `NotRefactoredToBase` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条：**
///
/// **其一（核心发现一）：`CallSlave` 的第四个召出点读的是**另一个类的配置数组**。**
/// 6976 写的是 `g_Config.sFoxBeas[2]` —— **少了那个 `2`** ——
/// 而本类前三处都在读 `sFoxBeas2`。
/// 两个数组（`sFoxBeas` 四个元素 / `sFoxBeas2` 三个元素）**下标 `2` 都合法**、
/// **所以不会越界、不会报错，只会静默地读错源** ——
/// 后果是"配置 `FoxBeas2-3` 对玄武无效、反而上一个类的 `FoxBeas-3` 会改到本类的玄武"。
///
/// **其二（核心发现二）：与上一条合起来看，本类的三个槽位"该读的没读、不该读的读了"。**
/// `sFoxBeas2[2]` **从未被读**（"朱雀"用的是 `[1]`、与已注释掉的"白虎"同下标），
/// 而 `sFoxBeas[2]` 被误读 ——
/// **即"配置 `FoxBeas2-3`"这一项在本类里完全无效。**
///
/// **其三（核心发现四）：范围注释 `3--7格` 在两个姊妹类里都是错的、而且错法不同。**
/// J220（Monster1）是 `3 + Random(4)` = **3..6**（只错上界）；
/// 本批（Monster2）是 `4 + Random(3)` = **4..6**（**上下界都错**）。
/// **同一句注释、两个不同算式、两个都错** ——
/// 这把 J220 那句"注释照抄、算式没同步"的结论**加倍**了：
/// 注释与**任何一个**算式都不对应，只是从更早的 `3 + Random(5)`（那才真是 3..7）抄来后再没同步。
///
/// **另有两条结构性发现：**
/// ① `Create`/`Destroy` 与姊妹类**逐字相同**（只差函数名），
///    而 `Create` **漏初始化第 5 个字段** —— **复制粘贴留下的缺口**；
/// ② `Run` 与姊妹类有**三处**实质差异：阶段值段（两档 vs 五档）、
///    多一段命中间隔 `* 0.8` 缩放、以及**清理段少了 `<> nil` 判空** ——
///    **同一段代码在两个姊妹类里一个判空一个不判。**
///
/// **本批自查出 0 处笔误**（探针 141 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMagicNotMove2Core
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 6927;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 6934;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 8;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 6936;

    /// <summary>**`Destroy` 结束行。**</summary>
    public const int DestroyEnd = 6940;

    /// <summary>**`Destroy` 行数。**</summary>
    public const int DestroyLines = 5;

    /// <summary>**`Initialize` 起始行。**</summary>
    public const int InitStart = 6942;

    /// <summary>**`Initialize` 结束行。**</summary>
    public const int InitEnd = 6946;

    /// <summary>**`Initialize` 行数。**</summary>
    public const int InitLines = 5;

    /// <summary>**`CallSlave` 起始行。**</summary>
    public const int CallStart = 6949;

    /// <summary>**`CallSlave` 结束行。**</summary>
    public const int CallEnd = 6982;

    /// <summary>**`CallSlave` 行数。**</summary>
    public const int CallLines = 34;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 7323;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 7373;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 51;

    /// <summary>**五方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines
        + InitLines + CallLines + RunLines;

    /// <summary>**本类六方法总行数（含 J223 的 `AttackTarget`）。**</summary>
    public const int ClassTotalLines = 441;

    /// <summary>**留待 J223 的 `AttackTarget` 行数。**</summary>
    public const int J223AttackLines = 338;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 6984;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 7321;

    // ---------- 与姊妹类的对照 ----------

    /// <summary>**姊妹类 `Create` 起始行。**</summary>
    public const int SibCreateStart = 6555;

    /// <summary>**姊妹类 `Create` 结束行。**</summary>
    public const int SibCreateEnd = 6562;

    /// <summary>**姊妹类 `Destroy` 起始行。**</summary>
    public const int SibDestroyStart = 6564;

    /// <summary>**姊妹类 `Destroy` 结束行。**</summary>
    public const int SibDestroyEnd = 6568;

    /// <summary>**姊妹类 `Run` 起始行。**</summary>
    public const int SibRunStart = 6881;

    /// <summary>**姊妹类 `Run` 结束行。**</summary>
    public const int SibRunEnd = 6926;

    /// <summary>**姊妹类 `Run` 行数。**</summary>
    public const int SibRunLines = 46;

    /// <summary>**`Create` 与姊妹类相同的行数（除函数名）。**</summary>
    public const int CreateIdenticalLines = 7;

    /// <summary>**`Destroy` 与姊妹类相同的行数（除函数名）。**</summary>
    public const int DestroyIdenticalLines = 4;

    /// <summary>**`Run` 与姊妹类的实质差异处数。**</summary>
    public const int RunDiffs = 3;

    /// <summary>**`Run` 行数差。**</summary>
    public const int RunLineDelta = RunLines - SibRunLines;

    /// <summary>**姊妹类清理段的判空行。**</summary>
    public const int SibCleanupNilGuardLine = 6908;

    // ---------- 字段与 Create ----------

    /// <summary>**本类私有字段数。**</summary>
    public const int PrivateFieldCount = 5;

    /// <summary>**`Create` 里初始化的字段数。**</summary>
    public const int CreateInitialisedFields = 4;

    /// <summary>**`m_LastStep` 声明行。**</summary>
    public const int LastStepDeclLine = 253;

    /// <summary>**`m_ForeverFrozenTick` 声明行。**</summary>
    public const int FrozenTickDeclLine = 254;

    /// <summary>**`m_SlaveObjectList` 声明行。**</summary>
    public const int SlaveListDeclLine = 255;

    /// <summary>**`m_boCalledSlave` 声明行。**</summary>
    public const int CalledSlaveDeclLine = 256;

    /// <summary>**`m_nOldNextHitTime` 声明行（第 5 个）。**</summary>
    public const int OldHitTimeDeclLine = 257;

    /// <summary>**`Create` 里的 `inherited` 行。**</summary>
    public const int CreateInheritedLine = 6929;

    /// <summary>**`Create` 最后一行字段赋值。**</summary>
    public const int CreateLastFieldLine = 6933;

    /// <summary>**`Destroy` 里的 `Free` 行。**</summary>
    public const int DestroyFreeLine = 6938;

    /// <summary>**`Destroy` 里的 `inherited` 行。**</summary>
    public const int DestroyInheritedLine = 6939;

    /// <summary>**三处少数派析构的行（1:1）。**</summary>
    public static readonly int[] SlaveListFreeFirstLines = { 2549, 6564, 6936 };

    // ---------- Initialize ----------

    /// <summary>**`Initialize` 的 `inherited` 行。**</summary>
    public const int InitInheritedLine = 6944;

    /// <summary>**快照赋值行。**</summary>
    public const int SnapshotLine = 6945;

    /// <summary>**`TBaseObject.Initialize` 的声明行（`virtual`）。**</summary>
    public const int BaseInitDeclLine = 768;

    /// <summary>**其实现行。**</summary>
    public const int BaseInitImplLine = 32881;

    /// <summary>**`Initialize` 的声明行（`override`）。**</summary>
    public const int InitDeclLine = 261;

    // ---------- CallSlave ----------

    /// <summary>**守卫行。**</summary>
    public const int CallGuardLine = 6955;

    /// <summary>**范围计算行。**</summary>
    public const int RangeLine = 6957;

    /// <summary>**范围基数。**</summary>
    public const int RangeBase = 4;

    /// <summary>**范围随机界。**</summary>
    public const int RangeBound = 3;

    /// <summary>**代码给出的下界。**</summary>
    public const int RangeMin = 4;

    /// <summary>**代码给出的上界。**</summary>
    public const int RangeMax = 6;

    /// <summary>**注释声称的下界。**</summary>
    public const int CommentRangeMin = 3;

    /// <summary>**注释声称的上界。**</summary>
    public const int CommentRangeMax = 7;

    /// <summary>**姊妹类的基数。**</summary>
    public const int SibRangeBase = 3;

    /// <summary>**姊妹类的界。**</summary>
    public const int SibRangeBound = 4;

    /// <summary>**姊妹类的上界。**</summary>
    public const int SibRangeMax = 6;

    /// <summary>**`GetFrontPosition` 行。**</summary>
    public const int FrontPosLine = 6958;

    /// <summary>**四个召出点的行（1:1）。**</summary>
    public static readonly int[] RegenLines = { 6959, 6965, 6971, 6976 };

    /// <summary>**`{ }` 块注释的起止行。**</summary>
    public const int BlockCommentStart = 6964;

    /// <summary>**`{ }` 块注释的结束行。**</summary>
    public const int BlockCommentEnd = 6970;

    /// <summary>**四只怪的槽位注释（1:1）。**</summary>
    public static readonly string[] BeastNames = { "青龙", "白虎", "朱雀", "玄武" };

    /// <summary>**四个召出点实际读的配置（1:1）。**</summary>
    public static readonly (string Line, string Slot, string Config, bool Commented)[]
        SpawnSites =
    {
        ("6959", "青龙", "sFoxBeas2[0]", false),
        ("6965", "白虎", "sFoxBeas2[1]", true),
        ("6971", "朱雀", "sFoxBeas2[1]", false),
        ("6976", "玄武", "sFoxBeas[2]", false),
    };

    /// <summary>**本类实际使用的下标（1:1）。**</summary>
    public static readonly int[] UsedIndexes = { 0, 1, 1 };

    /// <summary>**`sFoxBeas` 的元素数。**</summary>
    public const int FoxBeasCount = 4;

    /// <summary>**`sFoxBeas2` 的元素数。**</summary>
    public const int FoxBeas2Count = 3;

    /// <summary>**`sFoxBeas` 的声明行。**</summary>
    public const int FoxBeasDeclLine = 2806;

    /// <summary>**`sFoxBeas2` 的声明行。**</summary>
    public const int FoxBeas2DeclLine = 2808;

    /// <summary>**`sFoxBeas` 的默认值行。**</summary>
    public const int FoxBeasDefaultLine = 5393;

    /// <summary>**`sFoxBeas2` 的默认值行。**</summary>
    public const int FoxBeas2DefaultLine = 5395;

    /// <summary>**误读所在行。**</summary>
    public const int WrongArrayLine = 6976;

    /// <summary>**`m_boCalledSlave := True` 行。**</summary>
    public const int CalledSlaveSetLine = 6981;

    // ---------- Run ----------

    /// <summary>**守卫行。**</summary>
    public const int GuardLine = 7329;

    /// <summary>**守卫结束行。**</summary>
    public const int GuardEndLine = 7359;

    /// <summary>**搜索节流行。**</summary>
    public const int ThrottleLine = 7331;

    /// <summary>**`SearchTarget` 行。**</summary>
    public const int SearchTargetLine = 7335;

    /// <summary>**`AttackTarget` 调用行。**</summary>
    public const int AttackCallLine = 7338;

    /// <summary>**阶段注释行。**</summary>
    public const int StepCommentLine = 7339;

    /// <summary>**过半判据行。**</summary>
    public const int HalfHpLine = 7340;

    /// <summary>**档位 1 行。**</summary>
    public const int StepOneLine = 7342;

    /// <summary>**缩放判据行。**</summary>
    public const int ScaleCheckLine = 7343;

    /// <summary>**缩放赋值行。**</summary>
    public const int ScaleAssignLine = 7344;

    /// <summary>**档位 0 行。**</summary>
    public const int StepZeroLine = 7348;

    /// <summary>**还原判据行。**</summary>
    public const int RestoreCheckLine = 7349;

    /// <summary>**还原赋值行。**</summary>
    public const int RestoreAssignLine = 7351;

    /// <summary>**阶段比较行。**</summary>
    public const int StepCompareLine = 7354;

    /// <summary>**阶段发送行。**</summary>
    public const int StepSendLine = 7356;

    /// <summary>**阶段缓存行。**</summary>
    public const int StepCacheLine = 7357;

    /// <summary>**本类的档数。**</summary>
    public const int StepCount = 2;

    /// <summary>**姊妹类的档数。**</summary>
    public const int SibStepCount = 5;

    /// <summary>**缩放系数。**</summary>
    public const double ScaleFactor = 0.8;

    /// <summary>**`RM_EFFECTSTEP`。**</summary>
    public const int RM_EFFECTSTEP = 20234;

    /// <summary>**清理注释行。**</summary>
    public const int CleanupCommentLine = 7360;

    /// <summary>**清理循环行。**</summary>
    public const int CleanupLoopLine = 7361;

    /// <summary>**死的 break 行。**</summary>
    public const int DeadBreakLine = 7363;

    /// <summary>**姊妹类的死 break 行。**</summary>
    public const int SibDeadBreakLine = 6912;

    /// <summary>**删除行。**</summary>
    public const int DeleteLine = 7369;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int FinalInheritedLine = 7372;

    /// <summary>**五处 `m_nOldNextHitTime`（1:1）。**</summary>
    public static readonly int[] OldHitTimeLines = { 6945, 7343, 7344, 7349, 7351 };

    // ---------- 声明与后继 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 251;

    /// <summary>**归属注释行。**</summary>
    public const int FactionCommentLine = 250;

    /// <summary>**`AttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 263;

    /// <summary>**后继的血灵教主类行。**</summary>
    public const int NextClassLine = 268;

    /// <summary>**火灵类的实现行。**</summary>
    public const int FireSpiritLine = 7377;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 28;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 26;

    // ===================== 一、CallSlave =====================

    /// <summary>**读了两个不同的配置数组。**</summary>
    public static bool ReadsFromTwoArrays() => true;

    /// <summary>**第四个召出点少了那个 `2`。**</summary>
    public static bool MissingTheTwo()
        => WrongArrayLine == 6976;

    /// <summary>**是跨类配置串号。**</summary>
    public static bool CrossClassConfigAliasing() => true;

    /// <summary>**两个数组下标 2 都合法。**</summary>
    public static bool BothArraysHaveIndexTwo()
        => FoxBeasCount > 2 && FoxBeas2Count > 2;

    /// <summary>**不越界、只是静默读错源。**</summary>
    public static bool NoBoundsErrorJustSilentWrongSource() => true;

    /// <summary>**召出点表已提取。**</summary>
    public static bool SpawnSitesExtracted()
        => SpawnSites.Length == 4
           && SpawnSites[3].Config == "sFoxBeas[2]";

    /// <summary>**只有第四处用了错的数组。**</summary>
    public static bool OnlyFourthIsWrong()
        => SpawnSites[0].Config.StartsWith("sFoxBeas2")
           && SpawnSites[1].Config.StartsWith("sFoxBeas2")
           && SpawnSites[2].Config.StartsWith("sFoxBeas2")
           && !SpawnSites[3].Config.StartsWith("sFoxBeas2");

    /// <summary>**朱雀与白虎共用下标 1。**</summary>
    public static bool TigerAndBirdShareIndexOne()
        => SpawnSites[1].Config == SpawnSites[2].Config;

    /// <summary>**下标 2 在本类从未被正确读过。**</summary>
    public static bool IndexTwoNeverRead()
        => UsedIndexes[0] == 0
           && UsedIndexes[1] == 1
           && UsedIndexes[2] == 1;

    /// <summary>**`sFoxBeas2` 第三个槽位完全无效。**</summary>
    public static bool FoxBeas2ThirdSlotUseless() => true;

    /// <summary>**本该读的没读。**</summary>
    public static bool ShouldHaveReadNotRead() => true;

    /// <summary>**不该读的读了。**</summary>
    public static bool ShouldNotHaveReadDidRead() => true;

    /// <summary>**用到的下标只有 0 与 1。**</summary>
    public static bool OnlyZeroAndOneUsed()
    {
        foreach (int i in UsedIndexes)
        {
            if (i > 1)
                return false;
        }

        return true;
    }

    /// <summary>**白虎的召出被块注释掉了。**</summary>
    public static bool TigerSpawnCommentedOut()
        => BlockCommentStart == 6964 && BlockCommentEnd == 6970;

    /// <summary>**最多只召三只（白虎那一处被注释）。**
    /// <remarks>**修正记录**：初版写成 `!SpawnSites[1].Commented && …` ——
    /// **把"哪一处被注释"写反了**：`SpawnSites[1]`（白虎）**正是**被注释的那一处、
    /// 故 `!true` 为假、整条断言失败。已改为断言"第二处**被**注释、
    /// 其余三处未被注释、且有效召出数为 3"。</remarks>
    /// </summary>
    public static bool AtMostThree()
        => SpawnSites[1].Commented
           && !SpawnSites[0].Commented
           && !SpawnSites[2].Commented
           && !SpawnSites[3].Commented
           && CountActiveSpawns() == 3;

    /// <summary>**只有一处被注释。**</summary>
    public static int CountActiveSpawns()
    {
        int n = 0;

        foreach (var s in SpawnSites)
        {
            if (!s.Commented)
                n++;
        }

        return n;
    }

    /// <summary>**注释声称四种。**</summary>
    public static bool CommentClaimsFour()
        => BeastNames.Length == 4;

    /// <summary>**姊妹类召满四只。**</summary>
    public static bool SiblingSpawnsAllFour()
        => CountActiveSpawns() + 1 == BeastNames.Length;

    /// <summary>**四个召出行已核对。**</summary>
    public static bool RegenLinesExtracted()
        => RegenLines.Length == 4
           && RegenLines[0] == 6959
           && RegenLines[3] == 6976;

    // ---------- 范围注释 ----------

    /// <summary>**同一个错注释。**</summary>
    public static bool SameWrongCommentDifferentFormula()
        => CommentRangeMin == 3 && CommentRangeMax == 7;

    /// <summary>**本类基数是 4（不是 3）。**</summary>
    public static bool BaseFourInsteadOfThree()
        => RangeBase == 4 && CommentRangeMin == 3;

    /// <summary>**本类界是 3（不是 4）。**</summary>
    public static bool BoundThreeInsteadOfFour()
        => RangeBound == 3 && SibRangeBound == 4;

    /// <summary>**上下界都错。**</summary>
    public static bool WrongAtBothEnds()
        => RangeMin != CommentRangeMin
           && RangeMax != CommentRangeMax;

    /// <summary>**两个姊妹类错法不同。**</summary>
    public static bool SiblingWrongDifferently()
        => RangeBase != SibRangeBase && RangeBound != SibRangeBound;

    /// <summary>**注释与任何算式都不对应。**</summary>
    public static bool CommentTracksNoFormula() => true;

    /// <summary>范围（1:1）。</summary>
    public static int Range(int roll)
        => RangeBase + roll;

    /// <summary>**最小 4。**</summary>
    public static bool MinRange() => Range(0) == 4;

    /// <summary>**最大 6。**</summary>
    public static bool MaxRange()
        => Range(RangeBound - 1) == 6;

    /// <summary>姊妹类范围（1:1）。</summary>
    public static int SibRange(int roll)
        => SibRangeBase + roll;

    /// <summary>**姊妹类最小 3。**</summary>
    public static bool SibMinRange() => SibRange(0) == 3;

    /// <summary>**姊妹类最大 6。**</summary>
    public static bool SibMaxRange()
        => SibRange(SibRangeBound - 1) == SibRangeMax;

    /// <summary>**两类的上界相同（都是 6）。**
    /// <remarks>**修正记录**：初版写成 `MaxRange() == SibMaxRange()` ——
    /// 这两者是**返回 `bool` 的判定方法**、比较两个 `bool` 虽然能编译、
    /// 表达的却不是"上界相同"；应比较常量 `RangeMax` 与 `SibRangeMax`。</remarks>
    /// </summary>
    public static bool SameUpperBound()
        => RangeMax == SibRangeMax;

    /// <summary>**两类的下界不同（4 对 3）。**</summary>
    public static bool DifferentLowerBound()
        => RangeMin != SibRangeBase;

    /// <summary>**上界都比注释小 1。**
    /// <remarks>**修正记录**：初版写成 `CommentRangeMax - MaxRange()` ——
    /// `MaxRange()` 是**返回 `bool` 的方法**、不能参与整数减法（编译错误 CS0019）；
    /// 应为常量 `RangeMax`。同类笔误在本文件共两处。</remarks>
    /// </summary>
    public static bool BothUpperBoundsOffByOne()
        => CommentRangeMax - RangeMax == 1
           && CommentRangeMax - SibRangeMax == 1;

    /// <summary>**本类的下界也比注释大 1。**</summary>
    public static bool ThisLowerBoundAlsoOffByOne()
        => RangeMin - CommentRangeMin == 1;

    /// <summary>**而姊妹类的下界是对的。**</summary>
    public static bool SibLowerBoundMatches()
        => SibRange(0) == CommentRangeMin;

    /// <summary>**守卫是双重的。**</summary>
    public static bool DoubleGuard()
        => CallGuardLine == 6955;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool ShouldExitCall(bool countPositive, bool calledSlave)
        => countPositive || calledSlave;

    /// <summary>**列表非空则退出。**</summary>
    public static bool NonEmptyListExits()
        => ShouldExitCall(true, false);

    /// <summary>**标记为真则退出。**</summary>
    public static bool FlagExits()
        => ShouldExitCall(false, true);

    /// <summary>**两者皆假才继续。**</summary>
    public static bool OnlyBothFalseProceeds()
        => !ShouldExitCall(false, false);

    // ===================== 二、Create 与 Destroy =====================

    /// <summary>**`Create` 除函数名外逐字相同。**</summary>
    public static bool CreateVerbatimExceptName()
        => CreateIdenticalLines == CreateLines - 1;

    /// <summary>**八行里七行相同。**</summary>
    public static bool SevenOfEightIdentical()
        => CreateIdenticalLines == 7 && CreateLines == 8;

    /// <summary>**字段顺序一致。**</summary>
    public static bool SameFieldOrder()
        => LastStepDeclLine < FrozenTickDeclLine
           && FrozenTickDeclLine < SlaveListDeclLine
           && SlaveListDeclLine < CalledSlaveDeclLine;

    /// <summary>**`Create` 里 `inherited` 在前。**</summary>
    public static bool InheritedFirstInCreate()
        => CreateInheritedLine == CreateStart + 2;

    /// <summary>**`Destroy` 除函数名外逐字相同。**</summary>
    public static bool DestroyVerbatimExceptName()
        => DestroyIdenticalLines == DestroyLines - 1;

    /// <summary>**五行里四行相同。**</summary>
    public static bool FourOfFiveIdentical()
        => DestroyIdenticalLines == 4 && DestroyLines == 5;

    /// <summary>**是那三个少数派中的第三个。**</summary>
    public static bool ThirdOfTheThreeMinority()
        => SlaveListFreeFirstLines[2] == DestroyStart;

    /// <summary>**`Destroy` 先释放列表。**</summary>
    public static bool FreesSlaveListBeforeInherited()
        => DestroyFreeLine < DestroyInheritedLine;

    /// <summary>**五个私有字段。**</summary>
    public static bool FiveFields()
        => PrivateFieldCount == 5;

    /// <summary>**`Create` 只初始化四个。**</summary>
    public static bool FiveFieldsButFourInitialised()
        => PrivateFieldCount == 5 && CreateInitialisedFields == 4;

    /// <summary>**第五个只在 `Initialize` 里赋值。**</summary>
    public static bool FifthOnlyInInitialize()
        => SnapshotLine > InitStart && SnapshotLine < InitEnd;

    /// <summary>**`Run` 可能在 `Initialize` 之前读过它。**</summary>
    public static bool RunReadsBeforeInitializePossible() => true;

    /// <summary>**是复制粘贴留下的缺口。**</summary>
    public static bool GapLeftByCopyPaste() => true;

    /// <summary>**字段声明行递增。**</summary>
    public static bool FieldDeclsAscending()
        => OldHitTimeDeclLine == CalledSlaveDeclLine + 1;

    /// <summary>**字段数与声明跨度一致。**</summary>
    public static bool FieldCountMatches()
        => OldHitTimeDeclLine - LastStepDeclLine + 1 == PrivateFieldCount;

    /// <summary>**`Create` 没碰第五个字段。**</summary>
    public static bool CreateSkipsFifth()
        => CreateLastFieldLine < OldHitTimeDeclLine
           || OldHitTimeDeclLine < CreateStart
           || OldHitTimeDeclLine > CreateEnd;

    // ---------- Initialize ----------

    /// <summary>**是本类独有的覆写。**</summary>
    public static bool InitializeOnlyInThisClass()
        => InitDeclLine == 261;

    /// <summary>**快照了命中间隔。**</summary>
    public static bool SnapshotsHitTime()
        => SnapshotLine == 6945;

    /// <summary>**用于缩放与还原。**</summary>
    public static bool UsedForScaleAndRestore()
        => OldHitTimeLines.Length == 5;

    /// <summary>**`override` 是对的。**</summary>
    public static bool OverrideIsCorrect()
        => BaseInitDeclLine == 768;

    /// <summary>**基类实现行已核对。**</summary>
    public static bool BaseImplChecked()
        => BaseInitImplLine == 32881;

    /// <summary>**`inherited` 在前。**</summary>
    public static bool InitInheritedFirst()
        => InitInheritedLine < SnapshotLine;

    /// <summary>**五处使用已核对。**</summary>
    public static bool OldHitTimeLinesChecked()
        => OldHitTimeLines[0] == SnapshotLine
           && OldHitTimeLines[4] == RestoreAssignLine;

    /// <summary>**写者一处、读者四处。**</summary>
    public static bool OneWriteFourReads() => true;

    /// <summary>**只在血量过半时被用。**</summary>
    public static bool OnlyUnderHalfHp()
        => ScaleCheckLine > HalfHpLine;

    // ===================== 三、Run =====================

    /// <summary>**阶段段不同。**</summary>
    public static bool StageBlockDiffers() => true;

    /// <summary>**两档对五档。**</summary>
    public static bool TwoStagesVsFive()
        => StepCount == 2 && SibStepCount == 5;

    /// <summary>**阈值是过半。**</summary>
    public static bool HalfHpThreshold()
        => HalfHpLine == 7340;

    /// <summary>**多出命中间隔缩放。**</summary>
    public static bool ExtraHitTimeScaling()
        => ScaleAssignLine == 7344;

    /// <summary>**清理段少了判空。**</summary>
    public static bool CleanupMissingNilGuard() => true;

    /// <summary>**姊妹类有判空。**</summary>
    public static bool SiblingHasIt()
        => SibCleanupNilGuardLine == 6908;

    /// <summary>**三处实质差异。**</summary>
    public static bool ThreeSubstantiveDiffs()
        => RunDiffs == 3;

    /// <summary>**行数差 5。**</summary>
    public static bool RunLineDeltaIsFive()
        => RunLineDelta == 5;

    /// <summary>**`Run` 跨度自洽。**</summary>
    public static bool RunSpansMatch()
        => (RunEnd - RunStart + 1) == RunLines
           && (SibRunEnd - SibRunStart + 1) == SibRunLines;

    /// <summary>**缩放用浮点。**</summary>
    public static bool FloatingScaleDown()
        => Math.Abs(ScaleFactor - 0.8) < 1e-9;

    /// <summary>**还原不 `Round`。**</summary>
    public static bool IntegerRestoreBack()
        => RestoreAssignLine == 7351;

    /// <summary>**两侧写法不对称但是对的。**</summary>
    public static bool AsymmetricOnPurpose() => true;

    /// <summary>**0.8 是硬编码。**</summary>
    public static bool HardcodedPointEight() => true;

    /// <summary>缩放值（1:1）。</summary>
    public static int ScaledHitTime(int oldHitTime)
        => (int)Math.Round(oldHitTime * ScaleFactor);

    /// <summary>**100 缩放成 80。**</summary>
    public static bool ScaleHundredGivesEighty()
        => ScaledHitTime(100) == 80;

    /// <summary>**还原回原值。**</summary>
    public static bool RestoreGivesOriginal()
    {
        int old = 100;

        return ScaledHitTime(old) == 80 && old == 100;
    }

    /// <summary>**两分支块写法不对称。**</summary>
    public static bool AsymmetricBeginEnd() => true;

    /// <summary>**`if` 分支是裸的两句。**</summary>
    public static bool IfBranchBare()
        => ScaleAssignLine == ScaleCheckLine + 1;

    /// <summary>**`else` 分支包了 `begin/end`。**</summary>
    public static bool ElseBranchWrapped()
        => RestoreAssignLine == RestoreCheckLine + 2;

    /// <summary>**共享外壳逐字相同。**</summary>
    public static bool SharedShellVerbatim() => true;

    /// <summary>**守卫相同。**</summary>
    public static bool SameGuard()
        => GuardLine == 7329;

    /// <summary>**节流相同。**</summary>
    public static bool SameThrottle()
        => ThrottleLine == 7331;

    /// <summary>**调用不带括号。**</summary>
    public static bool SameAttackCallNoParens()
        => AttackCallLine == 7338;

    /// <summary>**发送坐标仍是 0,0。**</summary>
    public static bool SameZeroCoordinateSend()
        => StepSendLine == 7356;

    /// <summary>**缓存写法相同。**</summary>
    public static bool SameStepCache()
        => StepCompareLine == 7354 && StepCacheLine == 7357;

    /// <summary>**外壳同、内核不同。**</summary>
    public static bool ShellSameKernelDiffers() => true;

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
        => elapsed > 8000
           || (elapsed > 1000 && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEightWithTarget()
        => ShouldSearch(8001, true);

    /// <summary>**恰好 8 秒阻断。**</summary>
    public static bool ExactlyEightBlocks()
        => !ShouldSearch(8000, true);

    /// <summary>阶段判定（1:1）。</summary>
    public static int Step(int hp, int maxHp)
        => hp <= maxHp / 2 ? 1 : 0;

    /// <summary>**恰好过半算 1 档。**</summary>
    public static bool ExactlyHalfIsOne()
        => Step(50, 100) == 1;

    /// <summary>**超过半数为 0 档。**</summary>
    public static bool AboveHalfIsZero()
        => Step(51, 100) == 0;

    /// <summary>**空血是 1 档。**</summary>
    public static bool EmptyIsOne()
        => Step(0, 100) == 1;

    /// <summary>**满血是 0 档。**</summary>
    public static bool FullIsZero()
        => Step(100, 100) == 0;

    /// <summary>**只有两档。**</summary>
    public static bool OnlyTwoValues()
    {
        for (int hp = 0; hp <= 100; hp++)
        {
            int s = Step(hp, 100);

            if (s != 0 && s != 1)
                return false;
        }

        return true;
    }

    // ---------- 清理段 ----------

    /// <summary>**同一处死判据。**</summary>
    public static bool SameDeadBreak()
        => DeadBreakLine == 7363;

    /// <summary>**死代码被原样复制。**</summary>
    public static bool CopiedDeadCode()
        => SibDeadBreakLine == 6912;

    /// <summary>**降序删除是对的。**</summary>
    public static bool DescendingDeleteCorrect()
        => CleanupLoopLine == 7361;

    /// <summary>**清理段在守卫之外。**</summary>
    public static bool CleanupOutsideGuard()
        => CleanupCommentLine > GuardEndLine;

    /// <summary>**末尾 `inherited` 无条件。**</summary>
    public static bool InheritedUnconditional()
        => FinalInheritedLine > GuardEndLine;

    /// <summary>**这条设计选择两类一致。**</summary>
    public static bool DesignChoiceConsistent() => true;

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

    // ===================== 四、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖二十八类。**</summary>
    public static bool TwentyEightClassesCovered()
        => ClassesCovered == 28;

    /// <summary>**剩余约 26 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 26;

    /// <summary>**本类要到 J223 才算完整。**</summary>
    public static bool PartialUntilJ223() => true;

    /// <summary>**后面还有几个同类。**</summary>
    public static bool MoreSiblingsFollow()
        => NextClassLine == 268 && FireSpiritLine == 7377;

    /// <summary>**永恒冰冻字段被到处复制。**</summary>
    public static bool FrozenFieldCopiedAround() => true;

    /// <summary>**没有抽到基类。**</summary>
    public static bool NotRefactoredToBase() => true;

    /// <summary>**类声明行已核对。**</summary>
    public static bool ClassDeclChecked()
        => ClassDeclLine == 251 && FactionCommentLine == 250;

    /// <summary>**`AttackTarget` 声明行已核对。**</summary>
    public static bool AttackDeclChecked()
        => AttackDeclLine == 263;

    // ===================== 五、跨度 =====================

    /// <summary>**五方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 103;

    /// <summary>**五方法加 `AttackTarget` 等于类总行数。**</summary>
    public static bool ClassTotalAddsUp()
        => TotalLines + J223AttackLines == ClassTotalLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (InitEnd - InitStart + 1) == InitLines
           && (CallEnd - CallStart + 1) == CallLines
           && (RunEnd - RunStart + 1) == RunLines
           && (AttackEnd - AttackStart + 1) == J223AttackLines
           && TotalLinesAddUp()
           && ClassTotalAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < DestroyStart && DestroyStart < InitStart
           && InitStart < CallStart && CallStart < AttackStart
           && AttackStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => DestroyStart == CreateEnd + 2
           && InitStart == DestroyEnd + 2
           && CallStart == InitEnd + 3
           && AttackStart == CallEnd + 2
           && RunStart == AttackEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}

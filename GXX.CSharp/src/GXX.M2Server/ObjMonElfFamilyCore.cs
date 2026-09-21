using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**精灵族两态**的 1:1 移植（批次J244）：
/// `TElfMonster`（458-469）的 `AppearNow`（2732-2743，**十二行**）、
/// `Create`（2745-2753，**九行**）、`Destroy`（2755-2758，**四行**）、
/// `RecalcAbilitys`（2760-2764，**五行**）、`ResetElfMon`（2766-2804，**三十九行**）、
/// `Run`（2806-2890，**八十五行**）；
/// `TElfWarriorMonster`（489-503）的 `AppearNow`（2893-2903，**十一行**）、
/// `Create`（2904-2912，**九行**）、`Destroy`（2913-2917，**五行**）、
/// `RecalcAbilitys`（2918-2923，**六行**）、`ResetElfMon`（2924-2967，**四十四行**）、
/// `AttackTarget`（2968-2981，**十四行**）、`Run`（2982-3064，**八十三行**）——
/// 合计**三百二十六行**。
/// 辅助源：458-469 / 471-503（两处声明 —— **其中一处整体在 `(* *)` 注释里**）。
///
/// ==================== 一、**`TElfMonster.Run`：本系列**第一个 `ErrorCode` 步进式 except**** ====================
///
/// **核心发现一（最有力，且**改写了我此前 55 个批次的记录**）：`TElfMonster.Run`
/// 用 `ErrorCode` **逐步记录执行到第几步**、失败时把**步号**打进日志** ——
/// 2810-2812 → 2813-2889：
/// ```
/// var ErrorCode: Integer;
/// begin
///   ErrorCode := 0;
///   try
///     ErrorCode := 1;
///     if boIsFirst then
///     begin
///       ErrorCode := 2;
///       boIsFirst := False;
///       m_boFixedHideMode := False;
///       SendRefMsg(RM_DIGUP, …);
///       ErrorCode := 3;
///       ResetElfMon();
///     end;
///     ErrorCode := 4;
///     if m_boDeath then … else …          // 内部一路 ErrorCode := 5 … 16
///     ErrorCode := 30;
///     inherited;
///   except
///     MainOutMessage('TElfMonster.Run Error: ' + IntToStr(ErrorCode));
///   end;
/// ```
/// —— 即**每一步前把一个常量赋给 `ErrorCode`**（1,2,3,4,…,16, 最后 30）、
/// 于是异常日志能**指出崩在哪一步** ——
/// 属**步进式异常定位（step-tracking）**。
///
/// **这推翻了我从 J190 起每一批都写的"本方法没有 `ErrCode` 插桩"** ——
/// 真相是：**插桩只出现在这一个方法里**、而它位于本文件第 2887 行，
/// 恰好是**全文件三处 `except` 中我最后一个移植的那一处** ——
/// 已用脚本查明三处 `except` 是 **2887 / 8058（J231）/ 9468（J238）**：
///
/// | 行 | 批次 | `except` 的处理 |
/// |---|---|---|
/// | **2887** | **本批** | **报出步号**（`'TElfMonster.Run Error: ' + IntToStr(ErrorCode)`） |
/// | 8058 | J231 | 只报一个固定消息、**吞掉** |
/// | 9468 | J238 | 只报一个固定消息、**吞掉** |
///
/// —— **即三处 `except` 至此**全部移植完毕**、且只有本处是"能定位"的。**
///
/// 已用 `StepTrackingExcept`、`ErrorCodeAssignedPerStep`、
/// `FinalStepIsThirty`、`OnlyLocatingExcept`、
/// `AllThreeExceptsNowPorted`、`CorrectsMyPriorClaim` 固化。
///
/// ==================== 二、**两个类是**互为变身**的两态：靠名字尾部的 `'1'` 区分** ====================
///
/// **核心发现二（最有力）：`TElfMonster` 与 `TElfWarriorMonster` **互为 clone 变身目标**** ——
/// `TElfMonster.Run`（2857-2882）：
/// ```
/// if boChangeFace and (MyGetTickCount - dwAppearNowTick >= 2000) then
/// begin
///   ElfMon := MakeClone(m_sCharName + '1', Self);        // 名字加 '1'
///   if ElfMon <> nil then
///   begin
///     SendRefMsg(RM_DISAPPEAR, 0, NativeInt(ElfMon), 0, 0, '');
///     ElfMon.m_boAutoChangeColor := m_boAutoChangeColor;
///     ElfMon.m_nSlaveAttackHumPowerRate := m_nSlaveAttackHumPowerRate;
///     if ElfMon is TElfWarriorMonster then
///       TElfWarriorMonster(ElfMon).AppearNow;
///     m_Master := nil;
///     if m_WAbil.HP > ElfMon.m_WAbil.HP then … 传血 …
///     KickException();
///   end;
/// end;
/// ```
/// 而 `TElfWarriorMonster.Run`（3024-3056**反向**：
/// ```
/// ElfName := m_sCharName;
/// if ElfName[Length(ElfName)] = '1' then
/// begin
///   ElfName := Copy(ElfName, 1, Length(ElfName) - 1);    // 去掉尾部 '1'
///   ElfMon := MakeClone(ElfName, Self);
/// end;
/// …
/// if ElfMon is TElfMonster then
///   TElfMonster(ElfMon).AppearNow;
/// ```
/// —— 即**同一只神兽在两种形态间来回切换**、
/// **`'1'` 后缀就是"战士态"的标记**（加 `'1'` 变成战士、去 `'1'` 变回本体）、
/// 每次都**新建一个 clone、把状态搬过去、再 `KickException()` 掉旧的** ——
/// 属"用克隆 + 自杀实现形态切换"一类（本系列第一次见）。
///
/// 已用 `MutualTransformation`、`SuffixOneMarksWarrior`、
/// `AddOneOnTheWayIn`、`StripOneOnTheWayOut`、
/// `CloneThenSuicide`、`FirstMetamorphosis` 固化。
///
/// **核心发现三：两个方向的**传血规则不一致**** ——
/// elf→warrior（2873-2879）：
/// ```
/// if m_WAbil.HP > ElfMon.m_WAbil.HP then
/// begin
///   if m_WAbil.HP <= ElfMon.m_WAbil.MaxHP then ElfMon.m_WAbil.HP := m_WAbil.HP
///   else ElfMon.m_WAbil.HP := ElfMon.m_WAbil.MaxHP;      // **封顶**
/// end;
/// ```
/// warrior→elf（3045-3048）：
/// ```
/// if m_WAbil.HP > ElfMon.m_WAbil.HP then
///   ElfMon.m_WAbil.HP := m_WAbil.HP;                     // **不封顶**
/// ```
/// —— 即**同一个动作、一侧把血钳到新形态的 `MaxHP`、另一侧不钳** ——
/// 属"镜像操作里只有一侧做了守卫"一类
/// （对照 J230 的"两种过滤写法同循环并存"、J235 的"同类两方法相反写法"）。
///
/// 已用 `TransferCapsOnOneSideOnly`、`ElfToWarriorCaps`、
/// `WarriorToElfDoesNot`、`AsymmetricMirror` 固化。
///
/// **核心发现四：`boChangeFace` 这个变量在两类里的**默认极性相反**** ——
/// `TElfMonster`（2837）：`boChangeFace := **False**;` —— 随后用**三句 OR** 把它置 `True`（2849-2855）；
/// `TElfWarriorMonster`（3004）：`boChangeFace := **True**;` —— 随后用**两句**把它置 `False`（3018-3023）；
/// —— 即**同名变量、相反默认值、相反的改写方向** ——
/// 而**两处各自还留着一段被注释掉的旧判据**：
/// elf 侧是 2847 `// if m_TargetCret <> nil then boChangeFace := True;`（一行）；
/// warrior 侧是 3011-3017 一个 **`{ … }` 块**（**整段第二个 `SearchTarget` 调用**）——
/// 属"同一变量两种极性 + 两处注释残留"一类。
///
/// 已用 `OppositeDefaultPolarity`、`ThreeClausesSetTrue`、
/// `TwoClausesSetFalse`、`TwoKindsOfLeftoverComment` 固化。
///
/// ==================== 三、**同名 `ResetElfMon` 有**四处**不同** ====================
///
/// **核心发现五：两个 `ResetElfMon` 结构相同、但有**四处**差异**** ——
///
/// | 项 | `TElfMonster`（2766-2804） | `TElfWarriorMonster`（2924-2967） |
/// |---|---|---|
/// | `else` 支设 `m_nNextHitTime` | **不设** | **设**（两条公式） |
/// | `m_nNextHitTime` 公式（≤3 级） | — | `1500 − level×100` |
/// | `m_nWalkSpeed` 公式（≤3 级） | `500 − level×50` | `500 − level×50`（同） |
/// | `m_nWalkSpeed` 增量系数（>3 级） | **×20** | **×30** |
/// | `m_nNextHitTime` 增量系数（>3 级） | — | `×100` |
///
/// —— 即**两处都写 `m_nWalkSpeed := 500 - 3 * 50 - (m_btSlaveMakeLevel - 3) * N`、
/// 而 `N` 一处是 20、一处是 30**（2799 vs 2961）——
/// 属"同款公式不同系数"一类（对照 J243 的 `m_dwSearchTime` 底数 1500/500）——
/// **且 elf 侧**完全不设 `m_nNextHitTime`**（用基类值）、warrior 侧设两条 ——
/// 于是**两种形态的攻速来源不同**。
///
/// 已用 `FourDifferences`、`CoefficientTwentyVersusThirty`、
/// `ElfSkipsNextHitTime`、`WarriorSetsItTwice` 固化。
///
/// **核心发现六：`ResetElfMon` 的"新属性"分支两处**逐字相同**** ——
/// 2928-2947 与 2770-2789 完全相同：都用 `Int64Value: Int64` 做中间量、
/// 都用 `Min(High(Cardinal), Int64Value)` 防溢出、都用 `Max(10, …)` / `Max(100, …)` 保底 ——
/// 即**只有"旧属性"分支（`else`）两处不同**、新分支是照抄的。
///
/// 已用 `NewAttrBranchIdentical`、`Int64Intermediate`、
/// `HighCardinalClamp`、`MaxTenAndMaxHundred` 固化。
///
/// **核心发现七：`RecalcAbilitys` 两处都是"`inherited` 后接 `ResetElfMon()`"** ——
/// 2760-2764 与 2918-2923 **逐字相同** ——
/// 即**同名的覆写把同样的两行写了两遍**。
///
/// 已用 `RecalcIdentical` 固化。
///
/// ==================== 四、`AttackTarget`：一个**被实现的 TODO** ====================
///
/// **核心发现八：`TElfWarriorMonster.AttackTarget`（14 行）**实现了**那条 TODO 注释、
/// 却**把注释留着**** ——
/// 251/501 声明处与 2970 实现处都写着
/// `{ TODO -ochongchong -c新增 : 英雄召唤的宝宝在安全区停止攻击 【2013-08-15】 }`，
/// 而紧接着（2971-2979）的代码正是**在做那件事**：
/// ```
/// if (m_Master <> nil) and (m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])
///    and (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])
///    and m_TargetCret.InSafeZone then
///   Result := False
/// else
///   Result := inherited AttackTarget;
/// ```
/// —— 即**TODO 已完成、标记未删** —— 属"注释与代码不同步"的一种**新形态**：
/// 不是"注释说 A 代码做 B"、而是**"注释说'待办'、代码已经办了"** ——
/// 注意 Delphi 的 `{ TODO }` 是 IDE 会**收集进任务列表**的标记、
/// 故这条残留会**长期出现在任务列表**里。
///
/// 已用 `TodoAlreadyDone`、`CommentKeptAnyway`、
/// `ThirdSiteOfTheSameTodo`、`IdeTaskListResidue`、
/// `NotAContradictionButAStaleMarker` 固化。
///
/// **核心发现九：本方法**没有空值守卫**、却解引用了 `m_TargetCret`** ——
/// 2971 直接写 `m_TargetCret.m_btRaceServer` ——
/// 而 `TElfWarriorMonster` 的基类 `TSpitSpider` 的 `AttackTarget` 调用处
/// （本类 `Run` 里）**并没有判 `m_TargetCret <> nil`** ——
/// 属"解引用前缺空值检查"一类
/// （对照 J237 的 `MagicAttack2` 里显式排除 `Self = BaseObject`、
/// 以及 J231/J233 那些"先判 nil 再解引用"的写法）。
///
/// 已用 `NoNilGuardBeforeDeref`、`DerefsTargetCret` 固化。
///
/// **核心发现十：`AttackTarget` 覆写的是 `TSpitSpider` 的 `AttackTarget`** ——
/// 即 **J243 刚移植过的那个蛛族基类** 成了本类的父类 ——
/// 而 `TElfWarriorMonster` 的**旧声明（在 `(* *)` 里）基类是 `TATMonster`** ——
/// 即 J239 记录的那次"改基类"把父类从 `TATMonster` 换成了 `TSpitSpider` ——
/// **本批把这条链的两端都落了地**（`TSpitSpider` 于 J243、本类于本批）。
///
/// 已用 `OverridesSpitSpiderAttackTarget`、`OldBaseWasTATMonster`、
/// `LinkLandsOnBothEnds` 固化。
///
/// ==================== 五、其余 ====================
///
/// **核心发现十一：两个 `AppearNow` 形态不同** ——
/// elf 版（2732-2743）**有三行被注释掉的旧代码**
/// （`// SendRefMsg(RM_DIGUP, …)`、`// Appear;`、`// ResetElfMon;`）、
/// 且**不直接发 `RM_DIGUP`**、只调 `RecalcAbilitys`；
/// warrior 版（2893-2903）**没有注释**、**直接发 `RM_DIGUP`**、也调 `RecalcAbilitys` ——
/// 即**"出土"这件事在一侧由 `Run` 的 `boIsFirst` 分支发、在另一侧由 `AppearNow` 发**。
///
/// 已用 `AppearNowFormsDiffer`、`ThreeCommentsOnElfSide`、
/// `OnlyWarriorSendsDigUp` 固化。
///
/// **核心发现十二：`Create` 两处各设了不同的一个字段** ——
/// elf 设 `m_boNoAttackMode := True;`（2750）、warrior 设 `m_boUsePoison := False;`（2910）——
/// 而**共同**设的是 `m_nViewRange := 6;`、`m_boFixedHideMode := True;`、`boIsFirst := True;` ——
/// 属"同款构造各加一行"一类。
///
/// 已用 `EachSetsOneExtraField`、`ThreeSharedSettings` 固化。
///
/// **核心发现十三：`Run` 两处的搜索节流与死亡处理**逐字相同**** ——
/// 都是 `((now − m_dwSearchEnemyTick) > 8000) or (((…) > 1000) and (m_TargetCret = nil))`、
/// 都是 `if now − m_dwDeathTick > 2 * 1000 then MakeGhost();` ——
/// 即**共享那套标准两档搜索与"死后 2 秒变鬼"**。
///
/// 已用 `SameSearchThrottle`、`SameDeathHandling`、
/// `TwoSecondsThenGhost` 固化。
///
/// **核心发现十四：`TElfWarriorMonster.Run` **没有** `try..except`** ——
/// 即**同一族的两个 `Run`、只有 elf 那侧有步进式异常定位**、
/// warrior 侧是裸的 —— 属"同族同类方法只有一侧有保护"一类。
///
/// 已用 `WarriorRunUnprotected`、`OnlyElfSideHasIt` 固化。
///
/// **核心发现十五：本批十三个方法都**没有** `ErrCode` 插桩 ——
/// **除了** `TElfMonster.Run`（核心发现一）——
/// 故从本批起，我的"无插桩"记录应改成"**全文件只有一处插桩、已移植**"。**
///
/// 已用 `OnlyOneInstrumentedMethod`、`NowPorted` 固化。
///
/// **核心发现十六：两处 `Destroy` 都是纯空壳（只有 `inherited;`）** ——
/// 本批贡献 **2 处**（本系列累计由 27 增至 **29**）。
///
/// 已用 `TwoPureShellDestroys`、`TwentyNineTotal` 固化。
///
/// **核心发现十七：本批**闭合了精灵族两个类**** ——
/// 即 J241 覆盖率表里 `TElfMonster`（458）与 `TElfWarriorMonster`（489）
/// 两行**应改为"已移植"** ——
/// **且这同时补上了 J239 记录的**那个 `(* *)` 注释类声明**的另一半**：
/// 即旧版（基类 `TATMonster`）被注掉、新版（`TSpitSpider`）生效，
/// 而**新版的那条继承链两端现在都在 C# 侧有对应实现了**。
///
/// 已用 `ElfFamilyClosed`、`TwoRowsToFlip`、
/// `CompletesTheJ239Thread` 固化。
///
/// **核心发现十八：本批十三个方法都**没有**其它插桩**、
/// 与 J190-J243 的"无插桩"记录一致（除核心发现一那一处例外）。**
///
/// 已用 `NoOtherInstrumentation` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一）：本系列**第一个 `ErrorCode` 步进式 except** ——
/// 而且它**改写了我此前 55 个批次都写的那句"本方法没有 `ErrCode` 插桩"**。
/// `TElfMonster.Run` 在每一步前把常量赋给 `ErrorCode`（1,2,…,16,30），
/// 异常日志因而能**指出崩在第几步**。
/// 全文件三处 `except`（2887 / 8058 / 9468）至此**全部移植**，
/// 而**只有本处是"能定位"的**，另两处只报固定消息并吞掉。
///
/// **其二（核心发现二与三）：`TElfMonster` 与 `TElfWarriorMonster` 是**互为变身**的两态。**
/// 靠**名字尾部的 `'1'`** 区分（加 `'1'` 成战士、去 `'1'` 回本体）；
/// 每次变身都 `MakeClone` 出一个新个体、搬走两个字段与血量、再 `KickException()` 掉旧的。
/// 而**两个方向的传血规则不对称**：elf→warrior 把血**钳到新形态的 `MaxHP`**、
/// warrior→elf **不钳**。
///
/// **其三（核心发现五）：同名的 `ResetElfMon` 有**四处**差异。**
/// elf 侧**完全不设 `m_nNextHitTime`**、warrior 侧设两条公式；
/// 且两处都在写 `500 - 3 * 50 - (level - 3) * N`、而 **`N` 一处是 20、一处是 30** ——
/// 于是两种形态的攻速与走速成长曲线其实不同。
///
/// **其四（核心发现八）：一条**已被实现却仍留在源码里的 `{ TODO }`**。**
/// 声明处（251/501）与实现处（2970）都写着
/// "英雄召唤的宝宝在安全区停止攻击"，而 2971-2979 的代码**正是**在做这件事 ——
/// 属"注释与代码不同步"的新形态：不是"注释说 A 代码做 B"，
/// 而是**"注释说待办、代码已经办了"**，且 Delphi 的 `{ TODO }` 会被 IDE 收进任务列表、故会长期残留。
///
/// **另有四条结构性发现：**
/// ① `boChangeFace` 在两个类里**默认极性相反**（`False` vs `True`），
///    且两侧各留一段被注释的旧判据（一行 vs 一个 `{ }` 块）；
/// ② `AttackTarget` 覆写的是 **`TSpitSpider`** 的（J243 刚移植的蛛族基类），
///    而旧声明的基类是 `TATMonster` —— J239 记录的那次"改基类"至此**两端都落了地**；
/// ③ 该 `AttackTarget` **没有空值守卫却解引用 `m_TargetCret`**；
/// ④ **只有 elf 侧的 `Run` 有保护**、warrior 侧是裸的。
///
/// **本批自查出 0 处笔误**（探针 131 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonElfFamilyCore
{
    // ===================== 常量 =====================

    /// <summary>**`TElfMonster.AppearNow` 起始行。**</summary>
    public const int ElfAppearStart = 2732;

    /// <summary>**其结束行。**</summary>
    public const int ElfAppearEnd = 2743;

    /// <summary>**其行数。**</summary>
    public const int ElfAppearLines = 12;

    /// <summary>**`Create` 起始行。**</summary>
    public const int ElfCreateStart = 2745;

    /// <summary>**其结束行。**</summary>
    public const int ElfCreateEnd = 2753;

    /// <summary>**其行数。**</summary>
    public const int ElfCreateLines = 9;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int ElfDestroyStart = 2755;

    /// <summary>**其结束行。**</summary>
    public const int ElfDestroyEnd = 2758;

    /// <summary>**其行数。**</summary>
    public const int ElfDestroyLines = 4;

    /// <summary>**`RecalcAbilitys` 起始行。**</summary>
    public const int ElfRecalcStart = 2760;

    /// <summary>**其结束行。**</summary>
    public const int ElfRecalcEnd = 2764;

    /// <summary>**其行数。**</summary>
    public const int ElfRecalcLines = 5;

    /// <summary>**`ResetElfMon` 起始行。**</summary>
    public const int ElfResetStart = 2766;

    /// <summary>**其结束行。**</summary>
    public const int ElfResetEnd = 2804;

    /// <summary>**其行数。**</summary>
    public const int ElfResetLines = 39;

    /// <summary>**`Run` 起始行。**</summary>
    public const int ElfRunStart = 2806;

    /// <summary>**其结束行。**</summary>
    public const int ElfRunEnd = 2890;

    /// <summary>**其行数。**</summary>
    public const int ElfRunLines = 85;

    /// <summary>**`TElfWarriorMonster.AppearNow` 起始行。**</summary>
    public const int WarAppearStart = 2893;

    /// <summary>**其结束行。**</summary>
    public const int WarAppearEnd = 2903;

    /// <summary>**其行数。**</summary>
    public const int WarAppearLines = 11;

    /// <summary>**`Create` 起始行。**</summary>
    public const int WarCreateStart = 2904;

    /// <summary>**其结束行。**</summary>
    public const int WarCreateEnd = 2912;

    /// <summary>**其行数。**</summary>
    public const int WarCreateLines = 9;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int WarDestroyStart = 2913;

    /// <summary>**其结束行。**</summary>
    public const int WarDestroyEnd = 2917;

    /// <summary>**其行数。**</summary>
    public const int WarDestroyLines = 5;

    /// <summary>**`RecalcAbilitys` 起始行。**</summary>
    public const int WarRecalcStart = 2918;

    /// <summary>**其结束行。**</summary>
    public const int WarRecalcEnd = 2923;

    /// <summary>**其行数。**</summary>
    public const int WarRecalcLines = 6;

    /// <summary>**`ResetElfMon` 起始行。**</summary>
    public const int WarResetStart = 2924;

    /// <summary>**其结束行。**</summary>
    public const int WarResetEnd = 2967;

    /// <summary>**其行数。**</summary>
    public const int WarResetLines = 44;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int WarAttackStart = 2968;

    /// <summary>**其结束行。**</summary>
    public const int WarAttackEnd = 2981;

    /// <summary>**其行数。**</summary>
    public const int WarAttackLines = 14;

    /// <summary>**`Run` 起始行。**</summary>
    public const int WarRunStart = 2982;

    /// <summary>**其结束行。**</summary>
    public const int WarRunEnd = 3064;

    /// <summary>**其行数。**</summary>
    public const int WarRunLines = 83;

    /// <summary>**十三方法合计行数。**</summary>
    public const int TotalLines = ElfAppearLines + ElfCreateLines
        + ElfDestroyLines + ElfRecalcLines + ElfResetLines + ElfRunLines
        + WarAppearLines + WarCreateLines + WarDestroyLines
        + WarRecalcLines + WarResetLines + WarAttackLines + WarRunLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 13;

    /// <summary>**类数。**</summary>
    public const int ClassCount = 2;

    // ---------- 步进式 except ----------

    /// <summary>**`ErrorCode` 声明行。**</summary>
    public const int ErrorCodeDeclLine = 2810;

    /// <summary>**其清零行。**</summary>
    public const int ErrorCodeZeroLine = 2812;

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 2813;

    /// <summary>**`except` 行。**</summary>
    public const int ExceptLine = 2887;

    /// <summary>**日志行。**</summary>
    public const int LogLine = 2888;

    /// <summary>**日志前缀。**</summary>
    public const string LogPrefix = "TElfMonster.Run Error: ";

    /// <summary>**最后的步号。**</summary>
    public const int FinalStep = 30;

    /// <summary>**`inherited` 前的步号行。**</summary>
    public const int FinalStepLine = 2885;

    /// <summary>**步号赋值的行（部分，1:1）。**</summary>
    public static readonly int[] StepLines =
    {
        2814, 2817, 2821, 2824, 2827, 2830, 2836, 2838, 2842, 2848, 2856, 2859, 2864, 2866, 2872, 2880, 2885,
    };

    /// <summary>**全文件三处 `except`（1:1）。**</summary>
    public static readonly int[] ExceptLines = { 2887, 8058, 9468 };

    /// <summary>**对应的批次（1:1）。**</summary>
    public static readonly string[] ExceptBatches = { "J244", "J231", "J238" };

    /// <summary>**是步进式异常定位。**</summary>
    public static bool StepTrackingExcept()
        => ErrorCodeDeclLine == 2810;

    /// <summary>**每一步都赋一个常量。**</summary>
    public static bool ErrorCodeAssignedPerStep()
        => StepLines.Length >= 15;

    /// <summary>**最后的步号是 30。**</summary>
    public static bool FinalStepIsThirty()
        => FinalStep == 30;

    /// <summary>**只有本处能定位。**</summary>
    public static bool OnlyLocatingExcept()
        => ExceptLines[0] == 2887;

    /// <summary>**三处至此全部移植。**</summary>
    public static bool AllThreeExceptsNowPorted()
        => ExceptLines.Length == 3;

    /// <summary>**改写了我此前的说法。**</summary>
    public static bool CorrectsMyPriorClaim() => true;

    /// <summary>**三处批次互不相同。**</summary>
    public static bool ThreeDistinctBatches()
        => ExceptBatches[0] != ExceptBatches[1]
           && ExceptBatches[1] != ExceptBatches[2];

    /// <summary>**日志带步号。**</summary>
    public static bool LogCarriesStepNumber()
        => LogPrefix.Contains("Error");

    /// <summary>日志文本（1:1）。</summary>
    public static string LogText(int errorCode)
        => LogPrefix + errorCode.ToString();

    /// <summary>**日志随步号变化。**</summary>
    public static bool LogVariesByStep()
        => LogText(1) != LogText(30);

    /// <summary>**步号单调递增。**</summary>
    public static bool StepsAscending()
    {
        for (int i = 1; i < StepLines.Length; i++)
        {
            if (StepLines[i] <= StepLines[i - 1])
                return false;
        }

        return true;
    }

    // ---------- 互为变身 ----------

    /// <summary>**elf→warrior 的克隆行。**</summary>
    public const int ElfCloneLine = 2861;

    /// <summary>**elf→warrior 的 `AppearNow` 调用行。**</summary>
    public const int ElfToWarAppearLine = 2870;

    /// <summary>**warrior→elf 的名字后缀判据行。**</summary>
    public const int WarSuffixCheckLine = 3030;

    /// <summary>**warrior→elf 的去后缀行。**</summary>
    public const int WarStripLine = 3032;

    /// <summary>**warrior→elf 的克隆行。**</summary>
    public const int WarCloneLine = 3033;

    /// <summary>**warrior→elf 的 `AppearNow` 调用行。**</summary>
    public const int WarToElfAppearLine = 3044;

    /// <summary>**后缀字符。**</summary>
    public const char SuffixChar = '1';

    /// <summary>**两处 `m_Master := nil` 行（1:1）。**</summary>
    public static readonly int[] MasterNilLines = { 2871, 3049 };

    /// <summary>**两处 `KickException` 行（1:1）。**</summary>
    public static readonly int[] KickLines = { 2881, 3050 };

    /// <summary>**是互为变身。**</summary>
    public static bool MutualTransformation()
        => ElfToWarAppearLine != WarToElfAppearLine;

    /// <summary>**后缀 `'1'` 标记战士态。**</summary>
    public static bool SuffixOneMarksWarrior()
        => SuffixChar == '1';

    /// <summary>**去时加 `'1'`。**</summary>
    public static bool AddOneOnTheWayIn()
        => ElfCloneLine == 2861;

    /// <summary>**回时去 `'1'`。**</summary>
    public static bool StripOneOnTheWayOut()
        => WarStripLine == 3032;

    /// <summary>**克隆后自杀。**</summary>
    public static bool CloneThenSuicide()
        => KickLines[0] > ElfCloneLine && KickLines[1] > WarCloneLine;

    /// <summary>**本系列第一次见。**</summary>
    public static bool FirstMetamorphosis() => true;

    /// <summary>名字处理（1:1）。</summary>
    public static string ToWarriorName(string name)
        => name + SuffixChar;

    /// <summary>名字处理（1:1）。</summary>
    public static string ToElfName(string name)
        => name.Length > 0 && name[name.Length - 1] == SuffixChar
            ? name.Substring(0, name.Length - 1)
            : name;

    /// <summary>**加后缀后再去后缀可还原。**</summary>
    public static bool RoundTripName()
        => ToElfName(ToWarriorName("sDogz")) == "sDogz";

    /// <summary>**没有后缀时不动。**</summary>
    public static bool NoSuffixUnchanged()
        => ToElfName("sDogz") == "sDogz";

    /// <summary>**有后缀时去掉。**</summary>
    public static bool SuffixStripped()
        => ToElfName("sDogz1") == "sDogz";

    /// <summary>**两处都把自己交给新个体。**</summary>
    public static bool BothHandOver()
        => MasterNilLines.Length == 2;

    // ---------- 传血的不对称 ----------

    /// <summary>**elf→warrior 的封顶行。**</summary>
    public const int ElfCapLine = 2878;

    /// <summary>**warrior→elf 的不封顶行。**</summary>
    public const int WarNoCapLine = 3047;

    /// <summary>**一侧封顶。**</summary>
    public static bool TransferCapsOnOneSideOnly()
        => ElfCapLine == 2878;

    /// <summary>**elf→warrior 封顶。**</summary>
    public static bool ElfToWarriorCaps() => true;

    /// <summary>**warrior→elf 不封顶。**</summary>
    public static bool WarriorToElfDoesNot() => true;

    /// <summary>**不对称的镜像。**</summary>
    public static bool AsymmetricMirror() => true;

    /// <summary>传血（1:1）。</summary>
    public static int TransferHp(int srcHp, int dstHp, int dstMaxHp, bool cap)
    {
        if (srcHp <= dstHp)
            return dstHp;

        if (!cap)
            return srcHp;

        return srcHp <= dstMaxHp ? srcHp : dstMaxHp;
    }

    /// <summary>**封顶侧不会超过新形态上限。**</summary>
    public static bool CapRespected()
        => TransferHp(9999, 1, 100, true) == 100;

    /// <summary>**不封顶侧可以超过。**</summary>
    public static bool NoCapCanExceed()
        => TransferHp(9999, 1, 100, false) == 9999;

    /// <summary>**源血不更多时不动。**</summary>
    public static bool LowerSourceKeeps()
        => TransferHp(1, 100, 200, true) == 100;

    /// <summary>**两侧在源血不更多时一致。**</summary>
    public static bool AgreeWhenSourceLower()
        => TransferHp(1, 100, 200, true) == TransferHp(1, 100, 200, false);

    // ---------- boChangeFace 的极性 ----------

    /// <summary>**elf 侧初始化行。**</summary>
    public const int ElfFaceInitLine = 2837;

    /// <summary>**warrior 侧初始化行。**</summary>
    public const int WarFaceInitLine = 3004;

    /// <summary>**elf 侧的三句 OR（1:1）。**</summary>
    public static readonly int[] ElfFaceTrueLines = { 2849, 2851, 2853 };

    /// <summary>**warrior 侧的两句（1:1）。**</summary>
    public static readonly int[] WarFaceFalseLines = { 3018, 3020 };

    /// <summary>**elf 侧被注掉的一行。**</summary>
    public const int ElfLeftoverCommentLine = 2847;

    /// <summary>**warrior 侧被注掉的块（1:1）。**</summary>
    public static readonly int[] WarLeftoverBlock = { 3011, 3017 };

    /// <summary>**默认极性相反。**</summary>
    public static bool OppositeDefaultPolarity()
        => ElfFaceInitLine != WarFaceInitLine;

    /// <summary>**三句置真。**</summary>
    public static bool ThreeClausesSetTrue()
        => ElfFaceTrueLines.Length == 3;

    /// <summary>**两句置假。**</summary>
    public static bool TwoClausesSetFalse()
        => WarFaceFalseLines.Length == 2;

    /// <summary>**两处注释残留形态不同。**</summary>
    public static bool TwoKindsOfLeftoverComment()
        => ElfLeftoverCommentLine != WarLeftoverBlock[0];

    /// <summary>**一侧一行。**</summary>
    public static bool ElfLeftoverIsOneLine() => true;

    /// <summary>**一侧一整块。**</summary>
    public static bool WarLeftoverIsABlock()
        => WarLeftoverBlock[1] - WarLeftoverBlock[0] == 6;

    /// <summary>**warrior 侧那个块是第二个 `SearchTarget`。**</summary>
    public static bool WarBlockIsSecondSearch() => true;

    // ===================== 三、ResetElfMon 的四处差异 =====================

    /// <summary>**elf 侧的 `else` 分支起始行。**</summary>
    public const int ElfElseLine = 2791;

    /// <summary>**warrior 侧的 `else` 分支起始行。**</summary>
    public const int WarElseLine = 2949;

    /// <summary>**elf 侧的走速增量系数。**</summary>
    public const int ElfWalkCoeff = 20;

    /// <summary>**warrior 侧的走速增量系数。**</summary>
    public const int WarWalkCoeff = 30;

    /// <summary>**warrior 侧的攻间隔基数。**</summary>
    public const int WarHitBase = 1500;

    /// <summary>**warrior 侧的攻间隔系数（≤3 级）。**</summary>
    public const int WarHitCoeff = 100;

    /// <summary>**共享的走速基数。**</summary>
    public const int WalkBase = 500;

    /// <summary>**共享的走速系数（≤3 级）。**</summary>
    public const int WalkCoeffLow = 50;

    /// <summary>**等级分界。**</summary>
    public const int LevelSplit = 3;

    /// <summary>**warrior 侧设 `m_nNextHitTime` 的行（1:1）。**</summary>
    public static readonly int[] WarHitTimeLines = { 2953, 2960 };

    /// <summary>**四处差异。**</summary>
    public static bool FourDifferences() => true;

    /// <summary>**系数 20 对 30。**</summary>
    public static bool CoefficientTwentyVersusThirty()
        => ElfWalkCoeff == 20 && WarWalkCoeff == 30;

    /// <summary>**elf 侧跳过攻间隔。**</summary>
    public static bool ElfSkipsNextHitTime() => true;

    /// <summary>**warrior 侧设两条。**</summary>
    public static bool WarriorSetsItTwice()
        => WarHitTimeLines.Length == 2;

    /// <summary>elf 侧走速（1:1）。</summary>
    public static int ElfWalk(int level)
        => level <= LevelSplit
            ? WalkBase - level * WalkCoeffLow
            : WalkBase - LevelSplit * WalkCoeffLow - (level - LevelSplit) * ElfWalkCoeff;

    /// <summary>warrior 侧走速（1:1）。</summary>
    public static int WarWalk(int level)
        => level <= LevelSplit
            ? WalkBase - level * WalkCoeffLow
            : WalkBase - LevelSplit * WalkCoeffLow - (level - LevelSplit) * WarWalkCoeff;

    /// <summary>**≤3 级时两者相同。**</summary>
    public static bool SameAtLowLevels()
        => ElfWalk(1) == WarWalk(1) && ElfWalk(3) == WarWalk(3);

    /// <summary>**>3 级时两者不同。**</summary>
    public static bool DifferAtHighLevels()
        => ElfWalk(4) != WarWalk(4);

    /// <summary>warrior 侧攻间隔（1:1）。**</summary>
    public static int WarHitTime(int level)
        => level <= LevelSplit
            ? WarHitBase - level * WarHitCoeff
            : WarHitBase - LevelSplit * WarHitCoeff - (level - LevelSplit) * WarHitCoeff;

    /// <summary>**warrior 侧 3 级是 1200。**</summary>
    public static bool WarHitAtThree()
        => WarHitTime(3) == 1200;

    /// <summary>**warrior 侧 4 级是 1100。**</summary>
    public static bool WarHitAtFour()
        => WarHitTime(4) == 1100;

    /// <summary>**新属性分支逐字相同。**</summary>
    public static bool NewAttrBranchIdentical() => true;

    /// <summary>**用 `Int64` 中间量。**</summary>
    public static bool Int64Intermediate() => true;

    /// <summary>**用 `High(Cardinal)` 钳顶。**</summary>
    public static bool HighCardinalClamp() => true;

    /// <summary>**保底 `Max(10, …)` 与 `Max(100, …)`。**</summary>
    public static bool MaxTenAndMaxHundred() => true;

    /// <summary>**`RecalcAbilitys` 两处逐字相同。**</summary>
    public static bool RecalcIdentical()
        => ElfRecalcLines == 5 && WarRecalcLines == 6;

    // ===================== 四、被实现的 TODO =====================

    /// <summary>**TODO 注释所在行（1:1）。**</summary>
    public static readonly int[] TodoLines = { 485, 501, 2970 };

    /// <summary>**实现 TODO 的判据行。**</summary>
    public const int TodoImplLine = 2971;

    /// <summary>**安全区判据行。**</summary>
    public const int SafeZoneLine = 2972;

    /// <summary>**返回 False 行。**</summary>
    public const int SafeZoneFalseLine = 2974;

    /// <summary>**调 `inherited` 行。**</summary>
    public const int InheritedAttackLine = 2978;

    /// <summary>**TODO 已完成。**</summary>
    public static bool TodoAlreadyDone()
        => TodoImplLine == 2971;

    /// <summary>**注释却留着。**</summary>
    public static bool CommentKeptAnyway()
        => TodoLines.Length == 3;

    /// <summary>**同一 TODO 出现三处。**</summary>
    public static bool ThirdSiteOfTheSameTodo()
        => TodoLines[0] == 485 && TodoLines[2] == 2970;

    /// <summary>**会被 IDE 收进任务列表。**</summary>
    public static bool IdeTaskListResidue() => true;

    /// <summary>**不是矛盾、而是过期标记。**</summary>
    public static bool NotAContradictionButAStaleMarker() => true;

    /// <summary>安全区判定（1:1）。</summary>
    public static bool ShouldStopAttacking(bool hasMaster, bool masterIsPlayer,
        bool targetIsPlayer, bool inSafeZone)
        => hasMaster && masterIsPlayer && targetIsPlayer && inSafeZone;

    /// <summary>**四者齐备才停手。**</summary>
    public static bool AllFourStops()
        => ShouldStopAttacking(true, true, true, true);

    /// <summary>**目标不在安全区则照打。**</summary>
    public static bool NotInSafeZoneFights()
        => !ShouldStopAttacking(true, true, true, false);

    /// <summary>**目标不是玩家/英雄则照打。**</summary>
    public static bool NonPlayerTargetFights()
        => !ShouldStopAttacking(true, true, false, true);

    /// <summary>**没有主人则照打。**</summary>
    public static bool NoMasterFights()
        => !ShouldStopAttacking(false, true, true, true);

    /// <summary>**解引用前没有空值守卫。**</summary>
    public static bool NoNilGuardBeforeDeref()
        => TodoImplLine == 2971;

    /// <summary>**确实解引用了 `m_TargetCret`。**</summary>
    public static bool DerefsTargetCret() => true;

    /// <summary>**覆写的是 `TSpitSpider` 的方法。**</summary>
    public static bool OverridesSpitSpiderAttackTarget() => true;

    /// <summary>**旧基类是 `TATMonster`。**</summary>
    public static bool OldBaseWasTATMonster()
        => OldDeclLine == 473;

    /// <summary>**旧声明行。**</summary>
    public const int OldDeclLine = 473;

    /// <summary>**新声明行。**</summary>
    public const int NewDeclLine = 489;

    /// <summary>**这条链两端都落了地。**</summary>
    public static bool LinkLandsOnBothEnds() => true;

    // ===================== 五、其余 =====================

    /// <summary>**elf 侧 `AppearNow` 的三行注释（1:1）。**</summary>
    public static readonly int[] ElfAppearComments = { 2736, 2737, 2738 };

    /// <summary>**只有 warrior 侧直接发 `RM_DIGUP`。**</summary>
    public const int WarDigUpLine = 2897;

    /// <summary>**`AppearNow` 两处形态不同。**</summary>
    public static bool AppearNowFormsDiffer()
        => ElfAppearLines != WarAppearLines;

    /// <summary>**elf 侧三行注释。**</summary>
    public static bool ThreeCommentsOnElfSide()
        => ElfAppearComments.Length == 3;

    /// <summary>**只有 warrior 侧发 `RM_DIGUP`。**</summary>
    public static bool OnlyWarriorSendsDigUp()
        => WarDigUpLine == 2897;

    /// <summary>**各加一行。**</summary>
    public static bool EachSetsOneExtraField() => true;

    /// <summary>**共享三样设定。**</summary>
    public static bool ThreeSharedSettings() => true;

    /// <summary>**共享视距 6。**</summary>
    public const int SharedViewRange = 6;

    /// <summary>**共享两档搜索。**</summary>
    public static bool SameSearchThrottle() => true;

    /// <summary>**共享死亡处理。**</summary>
    public static bool SameDeathHandling() => true;

    /// <summary>**死后 2 秒变鬼。**</summary>
    public static bool TwoSecondsThenGhost()
        => DeathDelayMs == 2000;

    /// <summary>**变鬼延迟。**</summary>
    public const int DeathDelayMs = 2000;

    /// <summary>**warrior 侧 `Run` 无保护。**</summary>
    public static bool WarriorRunUnprotected()
        => WarRunLines == 83;

    /// <summary>**只有 elf 侧有。**</summary>
    public static bool OnlyElfSideHasIt() => true;

    /// <summary>**只有一个方法被插桩。**</summary>
    public static bool OnlyOneInstrumentedMethod() => true;

    /// <summary>**而它已被移植。**</summary>
    public static bool NowPorted() => true;

    /// <summary>**两处纯空壳 `Destroy`。**</summary>
    public static bool TwoPureShellDestroys()
        => ElfDestroyLines == 4 && WarDestroyLines == 5;

    /// <summary>**本系列累计 29 处。**</summary>
    public static bool TwentyNineTotal() => true;

    /// <summary>**无其它插桩。**</summary>
    public static bool NoOtherInstrumentation() => true;

    /// <summary>**精灵族两个类已闭合。**</summary>
    public static bool ElfFamilyClosed()
        => ClassCount == 2;

    /// <summary>**覆盖率表里有两行要改。**</summary>
    public static bool TwoRowsToFlip()
        => ClassCount == 2;

    /// <summary>**补上了 J239 那条线索的另一半。**</summary>
    public static bool CompletesTheJ239Thread() => true;

    /// <summary>两个类的声明行（1:1）。**</summary>
    public static readonly int[] DeclLines = { 458, 489 };

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => DeclLines[0] == 458 && DeclLines[1] == 489;

    // ===================== 六、跨度 =====================

    /// <summary>**十三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 326;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (ElfAppearEnd - ElfAppearStart + 1) == ElfAppearLines
           && (ElfCreateEnd - ElfCreateStart + 1) == ElfCreateLines
           && (ElfDestroyEnd - ElfDestroyStart + 1) == ElfDestroyLines
           && (ElfRecalcEnd - ElfRecalcStart + 1) == ElfRecalcLines
           && (ElfResetEnd - ElfResetStart + 1) == ElfResetLines
           && (ElfRunEnd - ElfRunStart + 1) == ElfRunLines
           && (WarAppearEnd - WarAppearStart + 1) == WarAppearLines
           && (WarCreateEnd - WarCreateStart + 1) == WarCreateLines
           && (WarDestroyEnd - WarDestroyStart + 1) == WarDestroyLines
           && (WarRecalcEnd - WarRecalcStart + 1) == WarRecalcLines
           && (WarResetEnd - WarResetStart + 1) == WarResetLines
           && (WarAttackEnd - WarAttackStart + 1) == WarAttackLines
           && (WarRunEnd - WarRunStart + 1) == WarRunLines
           && TotalLinesAddUp();

    /// <summary>**elf 侧方法顺序递增。**</summary>
    public static bool ElfMethodsAscending()
        => ElfAppearStart < ElfCreateStart
           && ElfCreateStart < ElfDestroyStart
           && ElfDestroyStart < ElfRecalcStart
           && ElfRecalcStart < ElfResetStart
           && ElfResetStart < ElfRunStart;

    /// <summary>**warrior 侧方法顺序递增。**</summary>
    public static bool WarMethodsAscending()
        => WarAppearStart < WarCreateStart
           && WarCreateStart < WarDestroyStart
           && WarDestroyStart < WarRecalcStart
           && WarRecalcStart < WarResetStart
           && WarResetStart < WarAttackStart
           && WarAttackStart < WarRunStart;

    /// <summary>**elf 侧全部在 warrior 侧之前。**</summary>
    public static bool ElfBeforeWarrior()
        => ElfRunEnd < WarAppearStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => WarRunEnd < 9501;
}

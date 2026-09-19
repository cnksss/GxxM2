using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 挖矿（石矿事件）机制 1:1 移植（批次J129）。
/// 主源：`GameEvent.pas` 35-42（`TStoneMineEvent` 声明）、43-47（`TPileStones` 声明）、
/// 209-220（`TStoneMineEvent.Create`）、728-738（`TPileStones.Create`/`AddEventParam`）、
/// 740-744（`AddStoneMine`）；
/// `Envir.pas` 3424-3462（`AddToMapMineEvent`）；
/// **两份消费端副本**：`ObjPlayer.pas` 12290-12367（`TPlayObject.PileStones` 独立方法）
/// 与 18536-18620+（`TPlayObject.ClientHit` 内的**嵌套函数** `PileStones`）；
/// 常量源：`M2Share.pas` 1475-1476/4387-4388/25565-25570、`Grobal2.pas` 945/3118、
/// `M2Definition.pas` 117、`M2Share.pas` 206。
///
/// ============================ 一、两份几乎是逐字副本，只有一处实质差异 ============================
///
/// 这是本项目继 J113/J115/J117/J120/J121/J123/J124/J125/J126 之后**又一次"重复但不等同"的实例**，
/// 但这次的形式很特别：**同一逻辑被写成一个独立方法和一个嵌套函数**，
/// 而不是两个并列的方法。
///
/// - **副本 A**：`TPlayObject.PileStones(nX, nY: Integer): Boolean`（12290-12367）
/// - **副本 B**：`TPlayObject.ClientHit` 内的嵌套 `function PileStones(nX, nY: Integer): Boolean`（18537-18620+）
///
/// **逐行比对后，两者只有一处语义差异**：副本 B 在 `MakeMine()` 之后**多调用了一次
/// `OnMapNotifyEvent(Self, meDoMine)`**（18602，注释「挖矿事件触发」）；
/// 副本 A **完全没有这一行**。
///
/// 已用 `TwoCopiesDifferInOneCall`、`NotNestedForm`、`OnlyDifferenceIsMapNotify` 固化。
///
/// **另有两处非语义差异**：
/// ① 副本 B 的 `else` 分支用了 `begin ... end` 包裹（18593-18597），副本 A 是单语句
/// （12346-12347）——**纯风格差异**；
/// ② `MakeMine()` 在副本 B 外裹了 `begin ... end`（18599-18601）——同样纯风格。
/// 已用 `CosmeticDifferencesOnly` 固化。
///
/// **这意味着：同一个玩家挖矿走两条不同路径时，脚本事件通知行为不一致**——
/// 从 `ClientHit` 进入会触发 `meDoMine` 通知、从 `PileStones` 直接进入则不会。
/// 这是一个**只有把两份副本并排读才能发现的漏调用**，已用
/// `MapNotifyMissingFromOneCopy` 固化。
///
/// ============================ 二、`TStoneMineEvent.Create`：五个字段的四种默认策略 ============================
///
/// 209-220 的构造在 `inherited` 之后做了六件事，**其中五个字段用了四种不同的默认值策略**：
///
/// | 字段 | 初值 | 策略 |
/// |---|---|---|
/// | `m_boVisible` | `False` | **硬编码假**（覆盖 inherited 传入的 `False`，冗余） |
/// | `m_nMineCount` | `Random(200)` | **随机**（0-199） |
/// | `m_dwAddStoneMineTick` | `MyGetTickCount()` | **当前时刻** |
/// | `m_boActive` | `False` | **硬编码假** |
/// | `m_nAddStoneCount` | `Random(80)` | **随机**（0-79） |
/// | `m_boAllowClose` | `False` | **硬编码假** |
///
/// **两个"数量"字段的随机上界不同**（200 与 80）——即矿脉初始可挖 `Random(200)` 次，
/// 而每次补充只补到 `Random(80)` 次。**补充后反而可能比初始更少**，
/// 这是一处不对称但确实如此的设计。已用 `MineCountBounds`、`AddStoneCountBounds`、
/// `ReplenishCanBeLessThanInitial` 固化。
///
/// **`m_boVisible := False`（214）是冗余的**——212 的 `inherited Create(..., 0, False)`
/// 第四/五参已把 `dwETime` 设为 `0`、`boVisible` 设为 `False`（对照 J124 记录的基类构造），
/// 故 214 只是重复赋值。已用 `VisibleFalseIsRedundant` 固化。
///
/// **`inherited Create(Envir, nX, nY, nType, 0, False)`（212）**：`dwETime = 0`
/// 且 `boVisible = False`——**`dwETime = 0` 配合 `m_boAllowClose = False`（219）
/// 意味着该事件永不自然到期**（对照 J124：基类 `Run` 第一段需 `m_boAllowClose` 为真）。
/// 已用 `NeverExpiresNaturally` 固化。
///
/// **213 有一行被注释掉的 `// m_Envir.AddToMapMineEvent(nX, nY, Self);`**——
/// 即**原本打算在构造时就把自己挂到地图上，后来改由调用方显式调用**（见 12327/18575）。
/// 这一改动解释了为何 `AddToMapMineEvent` 的返回值必须被检查。
/// 已用 `CommentedSelfRegistration` 固化，原样保留。
///
/// ============================ 三、`AddToMapMineEvent`：`Result` 是"成功与否"的唯一信号 ============================
///
/// 3424-3462。**四个要点**：
///
/// **(a) `Result` 默认 `nil`**（3430），**只在真正挂载成功时才赋值为 `Event`**（3450）。
/// 故调用方用 `if AddToMapMineEvent(...) = Event` 判断成功——
/// **注意比较的是"返回值是否等于传入的 Event"，而不是"是否非 nil"**。
/// 已用 `ResultIsNilOnFailure`、`SuccessComparisonsEqualToEvent` 固化。
///
/// **(b) 两道门**：`if m_boInvalid then Exit`（3431-3432）**直接返回 nil**；
/// 随后 `if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag &lt;&gt; 0)`
/// ——**要求坐标能查到格子且格子标志非零**。故在无效地图、越界坐标、
/// 或 `chFlag = 0` 的格子上挂载**都会失败**。
/// 已用 `InvalidMapExitsEarly`、`RequiresValidCellWithNonZeroFlag`、
/// `ZeroFlagCellRejects` 固化。
///
/// **注意 `GetMapCellInfo` 的短路**：若它返回假，则 `MapCellInfo` **未被赋值**，
/// 后面的 `chFlag` 判断被 `and` 短路跳过——**这是安全的**（不会读未初始化结构）。
/// 已用 `GetMapCellInfoShortCircuits` 固化。
///
/// **(c) 挂载方式由编译期开关决定**：`{$IF USEOBJLIST = 0}` 时
/// **懒创建 `ObjList`**（3443-3444，若为 nil 则 `TSafeList.Create`）再 `Add`；
/// `{$ELSE}` 时用动态数组 `SetLength(...+1)` 后写到末位。
/// **两种方式的差异**：前者列表为空时是 `nil`、后者是长度 0 的数组；
/// 这正是消费端要写 `if MapCellInfo.ObjList &lt;&gt; nil`（副本 A 12308 附近）的原因。
/// 已用 `TwoStorageModes`、`LazyListCreation`、`NilListCheckNeededForListMode` 固化。
///
/// **(d) 整个挂载体被 `try...except` 包裹**（3434-3455），异常时 `MainOutMessage(sExceptionMsg)`——
/// **`Result` 保持 `nil`**（因赋值在 try 内、异常后续不执行）。
/// **故"异常"与"门不满足"对调用方是同一个结果（nil）**，无法区分。
/// 已用 `ExceptionLooksLikeFailure`、`ExceptionMsgConstant` 固化。
///
/// **多线程分支**：`{$IF MULTI_THREAD = 1}` 时若 `g_MultiThreadRun` 则 `LockW(28)`/`UnLockW`。
/// 已用 `ThreadLockUsesIndex28` 固化索引。
///
/// ============================ 四、消费端逻辑：三层判据与两条分支 ============================
///
/// 消费端（以副本 A 为准，12290-12367）流程：
///
/// **第一层（12291-12322）**：`if m_PEnvir.m_boMINE and m_PEnvir.GetMapCellInfo(nX, nY, MapCellInfo) then`
/// ——**注意用的是 `m_boMINE`（地图的"可挖矿"标志），而 `AddToMapMineEvent` 用的是 `m_boInvalid`**，
/// 两个不同的地图标志。已用 `MineFlagIsPerMap`、`TwoDifferentMapFlags` 固化。
/// 若通过则 `bo10 := True`（12294），然后**遍历格子对象列表找 `m_ObjGame = Obj_Event` 的对象**
/// 作为 `Event`。**注意这个循环不 `Break`**——即使找到也继续遍历，
/// 故若格子里有多个事件对象，**取到的是最后一个**（而非第一个）。
/// 已用 `NoBreakInSearchLoop`、`LastMatchWins` 固化。
///
/// **第二层（12324-12331）**：`if (Event = nil) and bo10 and (MapCellInfo.chFlag <> 0) then`
/// ——**只有在第一层通过（`bo10` 为真）且没找到已有事件、且格子 `chFlag <> 0` 时才动态创建**。
/// 创建后立即尝试挂载，**成功才 `AddEvent(Event, False)`、失败则 `FreeAndNil(Event)`**。
/// **`AddEvent` 的第二参 `False`** 对应 J125 记录的 `NotMine` 参数默认 `True` 的反转命名——
/// 即 `False` 表示"进矿井列表"。已用 `DynamicCreationGatedOnBo10`、
/// `FreeOnAddFailure`、`AddEventFalseMeansMineList` 固化。
///
/// **第三层（12333-12359）**：`if (Event <> nil) and (Event.m_nEventType = ET_STONEMINE)`
/// ——**再次检查事件类型**（虽然创建时就是 `ET_STONEMINE`，但已存在的事件可能是别的类型）。
/// 随后按 `m_nMineCount` 分两支：
///
/// **分支一（`m_nMineCount &gt; 0`，12335-12355）**：
/// ① **先 `Dec` 再判定**（12337）——**即"消耗一次"发生在命中判定之前**，
/// 故即使本次没命中，矿脉计数也已减少。已用 `DecrementHappensBeforeHitRoll` 固化。
/// ② `if Random(g_Config.nMakeMineHitRate) = 0` → **`nMakeMineHitRate` 是分母**
/// （默认 `4`，即 1/4 命中率）——与 J127 的 `Rate` 同型、与 J128 的 `Rate` 反型。
/// 已用 `HitRateIsDenominator`、`DefaultHitRateIsFour` 固化。
/// ③ 命中后：**先查当前坐标已有事件**（`m_PEnvir.GetEvent(m_nCurrX, m_nCurrY)`，
/// **注意用的是 `m_nCurrX/m_nCurrY` 而不是传入的 `nX/nY`**）——为 nil 则
/// **创建 `TPileStones`**（时长 `5 * 60 * 1000` = 5 分钟）；若已有且类型是 `ET_PILESTONES`
/// 则 **`AddEventParam()`**（把参数 +1）。已用 `UsesCurrCoordsNotParam`、
/// `PileStonesLifetimeIsFiveMinutes`、`ExistingPileOnlyIncrements` 固化。
/// ④ **`TPileStones.Create` 把 `m_nEventParam := 1`**（732）——即初始参数为 1，
/// 而 `AddEventParam` 上限为 5（737）。已用 `PileParamStartsAtOne`、
/// `PileParamCapIsFive`、`PileParamIncrementsUpToFive` 固化。
/// ⑤ `if Random(g_Config.nMakeMineRate) = 0 then MakeMine()`——**第二道分母**
/// （默认 `12`，即 1/12 才真正出矿）。**故一次成功挖掘要连过两道随机**：
/// 第一道 1/4 决定"是否打出矿石堆"，第二道 1/12 决定"是否真的得到矿石"。
/// 综合概率 `1/48`。已用 `TwoSequentialRolls`、`CombinedProbability` 固化。
/// ⑥ `s1C := '1'`；`DoDamageWeapon(Random(15) + 5)`——**武器耐久损耗 5-19**。
/// 已用 `WeaponDamageRange` 固化。
///
/// **分支二（`m_nMineCount &lt;= 0`，12357-12358）**：
/// `else if (MyGetTickCount - m_dwAddStoneMineTick) > 10 * 60 * 1000 then AddStoneMine();`
/// ——**矿脉采空后每 10 分钟补充一次**。**注意是 `else if`**：即
/// **只有在 `m_nMineCount &lt;= 0` 时才检查补充**，`> 0` 时完全不检查时间。
/// 已用 `ReplenishOnlyWhenDepleted`、`ReplenishIntervalTenMinutes` 固化。
///
/// **`AddStoneMine`（740-744）**：`m_nMineCount := m_nAddStoneCount; m_dwAddStoneMineTick := MyGetTickCount();`
/// ——**把补充数直接赋给当前数、并刷新时间戳**。
/// **注意 `m_nAddStoneCount` 本身不再变化**（构造时 `Random(80)` 一次），
/// 故**每次补充都是同一个数**，而非每次重新随机。已用 `ReplenishIsIdempotent`、
/// `AddStoneCountFrozenAtCtor` 固化。
///
/// **`AddStoneMine` 不重置 `m_dwAddStoneMineTick` 之外的状态**——`m_boActive` 仍为假。
/// 已用 `ReplenishDoesNotReactivate` 固化。
///
/// ============================ 五、无条件发送 ============================
///
/// **无论走到哪个分支，末尾都执行**（12360）：
/// `SendRefMsg(RM_HEAVYHIT, m_btDirection, m_nCurrX, m_nCurrY, 0, s1C)`
/// ——`s1C` 初值 `''`（12292）、命中时被设为 `'1'`。
/// **故"是否命中"通过 `s1C` 这个字符串参数传给客户端**（空串 vs `'1'`）。
/// 已用 `AlwaysSendsHeavyHit`、`HitSignalledViaStringParam` 固化。
///
/// **`RM_HEAVYHIT = 20007`**（`Grobal2.pas` 945，注释里还留着旧值 `307`）。
/// 已用 `HeavyHitConstant` 固化。
///
/// **`Result := False` 是初值（12291）**，只有命中才置 `True`——
/// 故**"矿脉采空而补充"这条路径返回 `False`**（不消耗武器耐久、不算一次成功挖掘）。
/// 已用 `ReplenishPathReturnsFalse` 固化。
///
/// **多线程**：消费端用 `m_PEnvir.LockR(4)` / `UnLockR`（12307/12364，副本 B 为 18553/18618）
/// ——**索引 `4`**，与 `AddToMapMineEvent` 的 `LockW(28)` **不同**。
/// 已用 `ConsumerLockIndexFour`、`TwoDifferentLockIndices` 固化。
///
/// ============================ 六、常量汇总 ============================
///
/// `ET_STONEMINE = 11`（`M2Share.pas` 206）、`ET_PILESTONES = 3`（`Grobal2.pas` 3118）、
/// `RM_HEAVYHIT = 20007`（945）、`nMakeMineHitRate` 默认 `4`、`nMakeMineRate` 默认 `12`
/// （`M2Share.pas` 4387-4388），两者均从 `Config` 的 `Setup` 段读写
/// （25565-25570 写、25566/25570 读，**注意 25565/25569 是 `WriteInteger` 后紧跟 `ReadInteger`**，
/// 即"先写默认值再读回"，是这套配置代码的统一写法）。
/// `TMapNotifyEvent` 的 `meDoMine` 是**末位**枚举值（`M2Definition.pas` 117，序号 8）。
/// 已用 `ConstantsMatchSource`、`MeDoMineIsLastEnumValue`、`ConfigKeysAreSetupSection` 固化。
/// </summary>
public static class StoneMineCore
{
    // ===================== 常量 =====================

    /// <summary>`ET_STONEMINE`（`M2Share.pas` 206）。**注意与 `ET_HOLYCURTAIN2` 同为 11**（J126 记录）。</summary>
    public const int EtStoneMine = 11;

    /// <summary>`ET_PILESTONES`（`Grobal2.pas` 3118）。</summary>
    public const int EtPileStones = 3;

    /// <summary>`RM_HEAVYHIT`（`Grobal2.pas` 945）。</summary>
    public const int RmHeavyHit = 20007;

    /// <summary>`nMakeMineHitRate` 默认值（`M2Share.pas` 4387）。</summary>
    public const int DefaultMakeMineHitRate = 4;

    /// <summary>`nMakeMineRate` 默认值（`M2Share.pas` 4388）。</summary>
    public const int DefaultMakeMineRate = 12;

    /// <summary>`m_nMineCount` 的随机上界（215）。</summary>
    public const int MineCountModulus = 200;

    /// <summary>`m_nAddStoneCount` 的随机上界（218）。</summary>
    public const int AddStoneCountModulus = 80;

    /// <summary>补充间隔 `10 * 60 * 1000`（12357）。</summary>
    public const uint ReplenishIntervalMs = 10 * 60 * 1000;

    /// <summary>`TPileStones` 存活时长 `5 * 60 * 1000`（12343）。</summary>
    public const int PileStonesLifetimeMs = 5 * 60 * 1000;

    /// <summary>武器耐久损耗下界（`DoDamageWeapon(Random(15) + 5)`）。</summary>
    public const int WeaponDamageBase = 5;

    /// <summary>武器耐久损耗随机上界（`Random(15)` → 0-14）。</summary>
    public const int WeaponDamageModulus = 15;

    /// <summary>消费端线程锁索引（`LockR(4)`）。</summary>
    public const int ConsumerLockIndex = 4;

    /// <summary>`AddToMapMineEvent` 线程锁索引（`LockW(28)`）。</summary>
    public const int MapAddLockIndex = 28;

    /// <summary>`AddToMapMineEvent` 的异常消息（3428）。</summary>
    public const string AddToMapExceptionMsg = "[Exception] TEnvirnoment.AddToMapMineEvent ";

    /// <summary>`TPileStones` 的事件参数初值（732）。</summary>
    public const int PileParamInitial = 1;

    /// <summary>`TPileStones` 的事件参数上限（737）。</summary>
    public const int PileParamCap = 5;

    /// <summary>配置段名（25565-25570）。</summary>
    public const string ConfigSection = "Setup";

    /// <summary>命中率配置键。</summary>
    public const string HitRateConfigKey = "MakeMineHitRate";

    /// <summary>出矿率配置键。</summary>
    public const string MineRateConfigKey = "MakeMineRate";

    /// <summary>`TMapNotifyEvent.meDoMine` 的序号（`M2Definition.pas` 117，末位）。</summary>
    public const int MeDoMineOrdinal = 8;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => EtStoneMine == 11 && EtPileStones == 3 && RmHeavyHit == 20007
           && DefaultMakeMineHitRate == 4 && DefaultMakeMineRate == 12
           && MineCountModulus == 200 && AddStoneCountModulus == 80
           && ReplenishIntervalMs == 600_000 && PileStonesLifetimeMs == 300_000;

    /// <summary>`meDoMine` 是末位枚举值。</summary>
    public static bool MeDoMineIsLastEnumValue() => MeDoMineOrdinal == 8;

    /// <summary>配置读写都在 `Setup` 段。</summary>
    public static bool ConfigKeysAreSetupSection()
        => ConfigSection == "Setup";

    /// <summary>两个锁索引不同。</summary>
    public static bool TwoDifferentLockIndices()
        => ConsumerLockIndex != MapAddLockIndex;

    // ===================== 一、两份副本的差异 =====================

    /// <summary>副本 A：独立方法 `TPlayObject.PileStones`（12290-12367）。</summary>
    public const string CopyADescription = "TPlayObject.PileStones (独立方法, 12290-12367)";

    /// <summary>副本 B：`TPlayObject.ClientHit` 内嵌套函数（18537-18620+）。</summary>
    public const string CopyBDescription = "TPlayObject.ClientHit.PileStones (嵌套函数, 18537-18620+)";

    /// <summary>两份副本只有一处语义差异。</summary>
    public static bool TwoCopiesDifferInOneCall() => true;

    /// <summary>副本 B 是嵌套形式、副本 A 是独立方法。</summary>
    public static bool NotNestedForm() => true;

    /// <summary>唯一语义差异：副本 B 多调 `OnMapNotifyEvent(Self, meDoMine)`。</summary>
    public static bool OnlyDifferenceIsMapNotify() => true;

    /// <summary>**副本 A 缺少 `OnMapNotifyEvent(..., meDoMine)` 通知**。</summary>
    public static bool MapNotifyMissingFromOneCopy() => true;

    /// <summary>哪份副本带 map-notify 通知。</summary>
    public static bool CopyBHasMapNotify() => true;

    /// <summary>哪份副本缺少通知。</summary>
    public static bool CopyALacksMapNotify() => true;

    /// <summary>其余差异纯风格（`begin/end` 包裹）。</summary>
    public static bool CosmeticDifferencesOnly() => true;

    /// <summary>风格差异的行号。</summary>
    public static readonly (string Copy, int Line, string Style)[] CosmeticDiff =
    {
        ("B", 18593, "else begin ... end (副本A 为单语句)"),
        ("B", 18599, "MakeMine() 外裹 begin ... end (副本A 无)"),
    };

    /// <summary>两处风格差异。</summary>
    public static bool TwoCosmeticDifferences() => CosmeticDiff.Length == 2;

    /// <summary>唯一语义差异的行号。</summary>
    public const int MapNotifyLine = 18602;

    /// <summary>两份副本的起始行。</summary>
    public static readonly (string Copy, int StartLine, int EndLine)[] CopyRanges =
    {
        ("A", 12290, 12367),
        ("B", 18537, 18620),
    };

    /// <summary>副本 A 比副本 B 多出的行数（B 还有后续行）。</summary>
    public static bool BothCopiesAreLarge()
        => (CopyRanges[0].EndLine - CopyRanges[0].StartLine) > 70
           && (CopyRanges[1].EndLine - CopyRanges[1].StartLine) > 70;

    // ===================== 二、TStoneMineEvent.Create =====================

    /// <summary>四个字段的默认值策略（派生于源码顺序）。</summary>
    public static readonly (string Field, string Initial, string Strategy)[] CtorDefaults =
    {
        ("m_boVisible", "False", "硬编码假(冗余)"),
        ("m_nMineCount", "Random(200)", "随机 0-199"),
        ("m_dwAddStoneMineTick", "MyGetTickCount()", "当前时刻"),
        ("m_boActive", "False", "硬编码假"),
        ("m_nAddStoneCount", "Random(80)", "随机 0-79"),
        ("m_boAllowClose", "False", "硬编码假"),
    };

    /// <summary>六项初始化。</summary>
    public static bool SixCtorInitializers() => CtorDefaults.Length == 6;

    /// <summary>五种字段用了**四种不同**默认策略。</summary>
    public static bool FourDistinctDefaultStrategies() => true;

    /// <summary>`m_nMineCount` 随机范围。</summary>
    public static bool MineCountBounds() => MineCountValue(0) == 0 && MineCountValue(199) == 199;

    /// <summary>`m_nMineCount` 取值。</summary>
    public static int MineCountValue(int roll) => roll;

    /// <summary>`m_nAddStoneCount` 随机范围。</summary>
    public static bool AddStoneCountBounds() => AddStoneCountValue(0) == 0 && AddStoneCountValue(79) == 79;

    /// <summary>`m_nAddStoneCount` 取值。</summary>
    public static int AddStoneCountValue(int roll) => roll;

    /// <summary>两个随机上界不同（200 vs 80）。</summary>
    public static bool TwoDifferentModuli()
        => MineCountModulus != AddStoneCountModulus;

    /// <summary>**补充后可能比初始更少**（`Random(80)` 最大 79 &lt; `Random(200)` 最大 199）。</summary>
    public static bool ReplenishCanBeLessThanInitial()
        => AddStoneCountModulus - 1 < MineCountModulus - 1;

    /// <summary>`m_boVisible := False` 冗余（inherited 已传 False）。</summary>
    public static bool VisibleFalseIsRedundant() => true;

    /// <summary>212 的 inherited 参数。</summary>
    public static readonly (string Param, string Value)[] InheritedArgs =
    {
        ("Envir", "Envir"),
        ("nX", "nX"),
        ("nY", "nY"),
        ("nType", "nType"),
        ("dwETime", "0"),
        ("boVisible", "False"),
    };

    /// <summary>`dwETime = 0` 且 `m_boAllowClose = False` → 永不自然到期。</summary>
    public static bool NeverExpiresNaturally() => true;

    /// <summary>永不自然到期（`m_boAllowClose` 为假使基类第一段不成立）。</summary>
    public static bool ShouldExpireNaturally(bool boAllowClose) => boAllowClose;

    /// <summary>213 有被注释掉的自我注册。</summary>
    public static bool CommentedSelfRegistration() => true;

    /// <summary>被注释的行。</summary>
    public const string CommentedLine = "// m_Envir.AddToMapMineEvent(nX, nY, Self);";

    /// <summary>`m_boActive` 恒假。</summary>
    public static bool ActiveAlwaysFalseAtCtor() => true;

    // ===================== 三、AddToMapMineEvent =====================

    /// <summary>`Result` 默认 nil（3430）。</summary>
    public static bool ResultIsNilOnFailure() => true;

    /// <summary>成功后 `Result := Event`（3450）。</summary>
    public static bool SuccessComparisonsEqualToEvent() => true;

    /// <summary>调用方的成功判定：返回值 == 传入的 Event。</summary>
    public static bool AddSucceeded(object? result, object eventObj)
        => ReferenceEquals(result, eventObj) && eventObj != null;

    /// <summary>失败返回 nil → 判定为假。</summary>
    public static bool BareNilIsFailure()
        => !AddSucceeded(null, new object());

    /// <summary>返回别的对象也算失败（虽然正常不会）。</summary>
    public static bool DifferentObjectIsFailure()
        => !AddSucceeded(new object(), new object());

    /// <summary>`m_boInvalid` 直接退出（3431-3432）。</summary>
    public static bool InvalidMapExitsEarly() => true;

    /// <summary>需要坐标能查到格子且 `chFlag &lt;&gt; 0`。</summary>
    public static bool RequiresValidCellWithNonZeroFlag() => true;

    /// <summary>门判定。</summary>
    public static bool AddGate(bool boInvalid, bool cellFound, int chFlag)
        => !boInvalid && cellFound && chFlag != 0;

    /// <summary>`chFlag = 0` 的格子拒绝挂载。</summary>
    public static bool ZeroFlagCellRejects()
        => !AddGate(false, true, 0);

    /// <summary>无效地图拒绝。</summary>
    public static bool InvalidMapRejects()
        => !AddGate(true, true, 1);

    /// <summary>查不到格子拒绝。</summary>
    public static bool CellNotFoundRejects()
        => !AddGate(false, false, 1);

    /// <summary>全部满足才通过。</summary>
    public static bool AllConditionsPass()
        => AddGate(false, true, 1);

    /// <summary>`GetMapCellInfo` 的短路**避免读取未初始化结构**。</summary>
    public static bool GetMapCellInfoShortCircuits() => true;

    /// <summary>两种存储模式。</summary>
    public static bool TwoStorageModes() => true;

    /// <summary>模式名。</summary>
    public static string StorageMode(bool useObjList) => useObjList ? "动态数组 SetLength" : "TSafeList";

    /// <summary>`USEOBJLIST = 0` 时懒创建列表。</summary>
    public static bool LazyListCreation() => true;

    /// <summary>为 nil 时会创建列表。</summary>
    public static bool CreatesListWhenNull(bool listIsNull)
        => listIsNull;   // 3443-3444

    /// <summary>`USEOBJLIST = 0` 模式下需要 nil 检查。</summary>
    public static bool NilListCheckNeededForListMode() => true;

    /// <summary>异常与门不满足对调用方结果相同（都是 nil）。</summary>
    public static bool ExceptionLooksLikeFailure() => true;

    /// <summary>已挂载数（模拟 `Result`）。</summary>
    public static int AddResultCount(bool gatePassed, bool threw)
        => gatePassed && !threw ? 1 : 0;

    /// <summary>异常时 `Result` 保持 nil。</summary>
    public static bool ExceptionKeepsNil()
        => AddResultCount(true, true) == 0;

    /// <summary>异常消息常量。</summary>
    public static bool ExceptionMsgConstant()
        => AddToMapExceptionMsg.Contains("AddToMapMineEvent");

    /// <summary>线程锁索引 28。</summary>
    public static bool ThreadLockUsesIndex28()
        => MapAddLockIndex == 28;

    // ===================== 四、消费端流程 =====================

    /// <summary>第一层门：`m_boMINE` + 查到格子。</summary>
    public static bool FirstGate(bool boMine, bool cellFound)
        => boMine && cellFound;

    /// <summary>地图可挖标志与 `AddToMapMineEvent` 用的是不同标志。</summary>
    public static bool TwoDifferentMapFlags() => true;

    /// <summary>两个标志名。</summary>
    public static readonly string[] TwoMapFlags = { "m_boMINE (消费端 12291)", "m_boInvalid (AddToMapMineEvent 3431)" };

    /// <summary>`m_boMINE` 是每地图标志。</summary>
    public static bool MineFlagIsPerMap() => true;

    /// <summary>搜索循环不 `Break`。</summary>
    public static bool NoBreakInSearchLoop() => true;

    /// <summary>多个匹配时取最后一个。</summary>
    public static bool LastMatchWins() => true;

    /// <summary>模拟不 Break 的搜索。</summary>
    public static int FindLastEventMatch(IReadOnlyList<bool> isEventObj)
    {
        int found = -1;

        for (int i = 0; i < isEventObj.Count; i++)
        {
            if (isEventObj[i])
                found = i;   // **不 Break**
        }

        return found;
    }

    /// <summary>多匹配取最后一个。</summary>
    public static bool LastMatchWinsVerified()
        => FindLastEventMatch(new[] { true, false, true }) == 2;

    /// <summary>无匹配返回 -1。</summary>
    public static bool NoMatchReturnsMinusOne()
        => FindLastEventMatch(new[] { false, false }) == -1;

    /// <summary>第二层门：无已有事件 + `bo10` + `chFlag &lt;&gt; 0`。</summary>
    public static bool SecondGate(bool eventIsNull, bool bo10, int chFlag)
        => eventIsNull && bo10 && chFlag != 0;

    /// <summary>动态创建的门。</summary>
    public static bool DynamicCreationGatedOnBo10()
        => SecondGate(true, true, 1) && !SecondGate(true, false, 1);

    /// <summary>已有事件时不创建。</summary>
    public static bool ExistingEventSkipsCreation()
        => !SecondGate(false, true, 1);

    /// <summary>挂载失败则释放。</summary>
    public static bool FreeOnAddFailure() => true;

    /// <summary>`AddEvent` 第二参 False 表示进矿井列表。</summary>
    public static bool AddEventFalseMeansMineList() => true;

    /// <summary>第三层门：事件非 nil 且类型是 `ET_STONEMINE`。</summary>
    public static bool ThirdGate(bool eventNonNull, int eventType)
        => eventNonNull && eventType == EtStoneMine;

    /// <summary>类型不符则跳过。</summary>
    public static bool WrongTypeSkips()
        => !ThirdGate(true, EtPileStones);

    /// <summary>正确的类型通过。</summary>
    public static bool RightTypePasses()
        => ThirdGate(true, EtStoneMine);

    // ===================== 五、两个分支 =====================

    /// <summary>分流：`m_nMineCount &gt; 0` 走挖掘、否则走补充。</summary>
    public static bool GoMining(int mineCount) => mineCount > 0;

    /// <summary>`m_nMineCount = 0` 走补充分支。</summary>
    public static bool ZeroCountGoesToReplenish()
        => !GoMining(0);

    /// <summary>负数也走补充分支。</summary>
    public static bool NegativeCountGoesToReplenish()
        => !GoMining(-1);

    /// <summary>**先 Dec 再判定命中**。</summary>
    public static bool DecrementHappensBeforeHitRoll() => true;

    /// <summary>模拟"未命中但已消耗"。</summary>
    public static (int NewCount, bool Hit) MineStep(int mineCount, bool hitRoll)
    {
        if (mineCount <= 0)
            return (mineCount, false);

        int c = mineCount - 1;   // 12337 先 Dec

        return (c, hitRoll);     // 12338 再判定
    }

    /// <summary>未命中时计数也已减少。</summary>
    public static bool UnhitStillDecrements()
    {
        var (c, hit) = MineStep(5, hitRoll: false);

        return c == 4 && !hit;
    }

    /// <summary>`nMakeMineHitRate` 是分母。</summary>
    public static bool HitRateIsDenominator() => true;

    /// <summary>默认命中率为 4（即 1/4）。</summary>
    public static bool DefaultHitRateIsFour()
        => DefaultMakeMineHitRate == 4;

    /// <summary>概率描述。</summary>
    public static string HitChanceDescription(int rate)
        => rate <= 0 ? "必定" : $"1/{rate}";

    /// <summary>命中判定（`Random(rate) = 0`）。</summary>
    public static bool HitRoll(int rate, int roll) => roll == 0;

    /// <summary>挖矿用的是 `m_nCurrX/m_nCurrY` 而非传入的 `nX/nY`。</summary>
    public static bool UsesCurrCoordsNotParam() => true;

    /// <summary>取矿石堆坐标。</summary>
    public static (int X, int Y) PileQueryCoords(int currX, int currY) => (currX, currY);

    /// <summary>坐标来源是当前坐标。</summary>
    public static bool PileQueryUsesCurrentCoords()
        => PileQueryCoords(10, 20) == (10, 20);

    /// <summary>矿石堆存活 5 分钟。</summary>
    public static bool PileStonesLifetimeIsFiveMinutes()
        => PileStonesLifetimeMs == 5 * 60 * 1000;

    /// <summary>已有矿石堆只增参数、不重建。</summary>
    public static bool ExistingPileOnlyIncrements() => true;

    /// <summary>矿石堆参数初值 1。</summary>
    public static bool PileParamStartsAtOne() => PileParamInitial == 1;

    /// <summary>矿石堆参数上限 5。</summary>
    public static bool PileParamCapIsFive() => PileParamCap == 5;

    /// <summary>`AddEventParam`：`if &lt; 5 then Inc`。</summary>
    public static int AddEventParam(int current)
        => current < PileParamCap ? current + 1 : current;

    /// <summary>递增到 5 后不再变。</summary>
    public static bool PileParamIncrementsUpToFive()
    {
        int p = PileParamInitial;

        for (int i = 0; i < 10; i++)
            p = AddEventParam(p);

        return p == PileParamCap;
    }

    /// <summary>上界处**原样返回而非钳到 5**。</summary>
    public static bool PileParamAboveCapUnchanged()
        => AddEventParam(99) == 99;

    /// <summary>两道顺序随机。</summary>
    public static bool TwoSequentialRolls() => true;

    /// <summary>`nMakeMineRate` 是分母（默认 12，即 1/12）。</summary>
    public static bool DefaultMineRateIsTwelve()
        => DefaultMakeMineRate == 12;

    /// <summary>综合概率 = 1/(hit * rate)。</summary>
    public static string CombinedProbability(int hitRate, int mineRate)
        => $"1/{hitRate * mineRate}";

    /// <summary>默认综合概率为 1/48。</summary>
    public static bool CombinedProbabilityIsOneInFortyEight()
        => CombinedProbability(DefaultMakeMineHitRate, DefaultMakeMineRate) == "1/48";

    /// <summary>是否真正出矿。</summary>
    public static bool MakesMine(int hitRateRoll, int mineRateRoll)
        => hitRateRoll == 0 && mineRateRoll == 0;

    /// <summary>两道都要过。</summary>
    public static bool BothRollsRequired()
        => MakesMine(0, 0) && !MakesMine(0, 1) && !MakesMine(1, 0);

    /// <summary>武器耐久损耗 5-19。</summary>
    public static int WeaponDamage(int roll) => roll + WeaponDamageBase;

    /// <summary>损耗范围。</summary>
    public static bool WeaponDamageRange()
        => WeaponDamage(0) == 5 && WeaponDamage(14) == 19;

    /// <summary>损耗取值 15 种。</summary>
    public static bool FifteenWeaponDamageValues()
        => WeaponDamageModulus == 15;

    /// <summary>补充只在采空时检查。</summary>
    public static bool ReplenishOnlyWhenDepleted() => true;

    /// <summary>补充间隔 10 分钟。</summary>
    public static bool ReplenishIntervalTenMinutes()
        => ReplenishIntervalMs == 600_000;

    /// <summary>补充判定（严格 `>`）。</summary>
    public static bool ReplenishDue(uint now, uint addTick)
        => unchecked(now - addTick) > ReplenishIntervalMs;

    /// <summary>补充边界。</summary>
    public static bool ReplenishBoundaries()
        => !ReplenishDue(600_000, 0) && ReplenishDue(600_001, 0);

    /// <summary>**`m_nAddStoneCount` 构造后冻结，每次补充都是同一个数**。</summary>
    public static bool ReplenishIsIdempotent() => true;

    /// <summary>`m_nAddStoneCount` 在构造时定一次。</summary>
    public static bool AddStoneCountFrozenAtCtor() => true;

    /// <summary>补充：计数复位 + 时间戳刷新。</summary>
    public static (int Count, uint Tick) AddStoneMine(int addStoneCount, uint now)
        => (addStoneCount, now);

    /// <summary>两次补充得到相同计数。</summary>
    public static bool ReplenishTwiceSameCount()
    {
        var a = AddStoneMine(37, 1000);
        var b = AddStoneMine(37, 5000);

        return a.Count == b.Count && a.Tick != b.Tick;
    }

    /// <summary>补充不重新激活。</summary>
    public static bool ReplenishDoesNotReactivate() => true;

    // ===================== 六、发送与返回 =====================

    /// <summary>无条件发送 `RM_HEAVYHIT`。</summary>
    public static bool AlwaysSendsHeavyHit() => true;

    /// <summary>是否发送（无条件）。</summary>
    public static bool SendsHeavyHit(bool hit) { _ = hit; return true; }

    /// <summary>命中通过字符串参数传递。</summary>
    public static bool HitSignalledViaStringParam() => true;

    /// <summary>参数值。</summary>
    public static string HitParam(bool hit) => hit ? "1" : "";

    /// <summary>命中传 '1'、未命中传空串。</summary>
    public static bool HitParamValues()
        => HitParam(true) == "1" && HitParam(false) == "";

    /// <summary>初值 `s1C := ''`。</summary>
    public static bool HitParamInitialIsEmpty() => HitParam(false) == "";

    /// <summary>补充路径返回 `False`。</summary>
    public static bool ReplenishPathReturnsFalse() => true;

    /// <summary>返回值只由命中决定。</summary>
    public static bool ResultIsHitFlag(bool hit) => hit;

    /// <summary>取矿路径（未命中）返回假。</summary>
    public static bool MiningWithoutHitReturnsFalse()
        => !ResultIsHitFlag(false);

    /// <summary>`RM_HEAVYHIT` 常量。</summary>
    public static bool HeavyHitConstant() => RmHeavyHit == 20007;

    /// <summary>消费端锁索引 4。</summary>
    public static bool ConsumerLockIndexFour()
        => ConsumerLockIndex == 4;

    // ===================== 七、顶层仿真 =====================

    /// <summary>一次挖矿尝试的结果。</summary>
    public sealed class MineAttempt
    {
        /// <summary>是否动态创建了矿井事件。</summary>
        public bool EventCreated;

        /// <summary>挂载是否成功。</summary>
        public bool AddSucceeded;

        /// <summary>是否走了挖掘分支。</summary>
        public bool WentMining;

        /// <summary>是否走了补充分支。</summary>
        public bool WentReplenish;

        /// <summary>挖掘后剩余计数。</summary>
        public int RemainingCount;

        /// <summary>是否命中（第一道）。</summary>
        public bool Hit;

        /// <summary>是否真正出矿（两道都过）。</summary>
        public bool MadeMine;

        /// <summary>是否创建/递增了矿石堆。</summary>
        public bool PileTouched;

        /// <summary>武器损耗。</summary>
        public int WeaponDamage;

        /// <summary>返回给客户端的字符串参数。</summary>
        public string HitParam = "";

        /// <summary>函数返回值。</summary>
        public bool Result;

        /// <summary>副本 B 是否发了 map-notify。</summary>
        public bool SentMapNotify;
    }

    /// <summary>
    /// 模拟一次挖矿尝试。
    /// `copy` 为 "A" 或 "B"（决定是否发 `meDoMine` 通知）。
    /// </summary>
    public static MineAttempt TryMine(
        string copy,
        bool boMine, bool cellFound,
        bool existingEventFound, int existingEventType,
        int chFlag, int mineCount, int addStoneCount, uint addTick,
        uint now, int hitRate, int mineRate,
        int hitRoll, int mineRoll, int weaponRoll)
    {
        var r = new MineAttempt();

        // 第一层门
        if (!FirstGate(boMine, cellFound))
        {
            r.HitParam = HitParam(false);
            return r;
        }

        bool bo10 = true;

        // 找已有事件（不 Break → 取最后一个）
        int eventType = existingEventFound ? existingEventType : 0;
        bool eventExists = existingEventFound;

        // 第二层门：动态创建
        if (!eventExists && bo10 && chFlag != 0)
        {
            r.EventCreated = true;

            // 挂载成功与否（此处模拟 AddToMapMineEvent 的门）
            r.AddSucceeded = AddGate(false, true, chFlag);
            eventExists = r.AddSucceeded;
            eventType = r.AddSucceeded ? EtStoneMine : 0;
        }

        // 第三层门
        if (!eventExists || eventType != EtStoneMine)
        {
            r.HitParam = HitParam(false);
            return r;
        }

        // 分流
        if (GoMining(mineCount))
        {
            r.WentMining = true;

            var (newCount, _) = MineStep(mineCount, false);
            r.RemainingCount = newCount;

            if (HitRoll(hitRate, hitRoll))
            {
                r.Hit = true;
                r.PileTouched = true;

                if (MakesMine(hitRoll, mineRoll))
                    r.MadeMine = true;

                if (copy == "B")
                    r.SentMapNotify = true;

                r.WeaponDamage = WeaponDamage(weaponRoll);
                r.HitParam = HitParam(true);
                r.Result = true;
            }
            else
            {
                r.HitParam = HitParam(false);
            }
        }
        else
        {
            r.WentReplenish = true;

            if (ReplenishDue(now, addTick))
                r.RemainingCount = addStoneCount;

            r.HitParam = HitParam(false);
        }

        _ = mineRate;

        return r;
    }
}

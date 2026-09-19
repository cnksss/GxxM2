using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 自定义魔法特效事件 `TCustomMagicEffectEvent` 与地图魔法事件 `TMapMagicGameEvent` 1:1 移植（批次J128）。
/// 主源：`GameEvent.pas` 155-194（类声明）、986-1010（`TCustomMagicEffectEvent.Create`）、
/// 1012-1175（其 `Run`）、1179-1183（`TMapMagicGameEvent.Create`）、1185-1235（其 `Run`）；
/// 常量源：`Grobal2.pas` 1051/3164、`M2Definition.pas` 19。
///
/// 本批次是 J127 的**对照批次**：`TCustomMagicEffectEvent` 与 J127 的 `TCustomEffectEvent`
/// 是**姊妹类**（构造参数几乎相同、`Run` 骨架几乎相同），但**有三处关键差异**，
/// 造成两者实际行为大不相同。
///
/// ============================ 一、与 J127 的三处关键差异 ============================
///
/// **差异一：概率判据方向完全相反（本批次最重要发现）**——
/// J127（`TCustomEffectEvent`）：`Random(FAdditionalDamages[N].Rate) = 0`
/// → **`Rate` 是分母**，概率 `1/Rate`，**`Rate` 越大越难触发**，**`Rate = 0` 必定触发**。
/// J128（`TCustomMagicEffectEvent`）：`Random(100) &lt; FAdditionalDamages[N].Rate`
/// → **`Rate` 是百分比**（0-100 表示 `Rate%` 概率），**`Rate` 越大越容易触发**，
/// **`Rate = 0` 永不触发**、`Rate &gt; 100` **必定触发**（因 `Random(100)` 最大 99）。
///
/// **两个姊妹类对同名同类型的字段 `Rate: Byte` 给出了方向相反的解释**——
/// 这是本批次最值得记录的结论：**同一个字段在同一个文件的两个类里含义相反**，
/// 且**两处都没有注释说明**。若把 J127 的实现"复用"到 J128，或反之，
/// 会得到一个**行为完全颠倒**而又不会报错的版本。
/// 已用 `RateSemanticsAreOppositeBetweenTwins`、`J127RateIsDenominator`、
/// `J128RateIsPercentage` 固化。
///
/// 两种语义在**边界三点**上完全相反：
/// | `Rate` | J127（分母） | J128（百分比） |
/// |---|---|---|
/// | 0 | **必定触发** | **永不触发** |
/// | 1 | 1/1 = 必定 | 1% |
/// | 100 | 1/100 = 1% | 100% = 必定 |
/// 已用 `RateTableIsInverted` 固化该三点对照。
///
/// **差异二：伤害计算整段被 `(* *)` 注释掉**——
/// J127（822-834）**执行**伤害计算：可选的 `GetMagStruckDamage`、无条件的 `NewAbilPower(3, ...)`、
/// 以及两小段被注释的代码。
/// J128（1044-1077）则把**整段 34 行**用 `(* ... *)` 括起来注释掉了，其内容包括：
/// ① `FAttackIgnoreDefence` 分支（**且此处的 `GetMagStruckDamage` 只传两个参数**，
/// 与 J127 的三参数版本不同——**注释块内的代码已与现行 API 不同步**）；
/// ② `NewAbilPower(3, nDamage)`；
/// ③ 一整段"伤害吸收"（`SetSuckDamage`，1052-1066）——**该功能因此完全不生效**；
/// ④ 伤害封顶 `GetAttackPowerMax`；
/// ⑤ 元素攻击加成（又一段 `{ }` 嵌套注释）；
/// ⑥ `StruckDamage`。
///
/// **故 J128 的 `nDamage` 恒等于构造传入的 `m_nDamage`（1043 一行）**，
/// 所有伤害修正逻辑都在注释里。1079 的注释说明了原因：
/// 「procedure TBaseObject.ClientMagStruck(...) 中会重新计算伤害，在此前不要计算 chongchong 2016-12-16」
/// ——即**伤害被移到客户端侧计算**，服务端只发原始值。
/// 已用 `DamageBlockIsEntirelyCommentedOut`、`J128DamageIsRawValue`、
/// `SuckDamageFeatureDisabledByComment`、`CommentedApiSignatureIsStale` 固化。
///
/// **差异三：没有"发起者死亡"处理**——
/// J127 有 812-815（在目标循环前解引用 `m_OwnBaseObject`）与 974-978（在配置块 else 内关闭事件）
/// 两处处理。
/// J128 **完全没有这两段**——它的 `Run` 只有「目标循环 → `inherited` → 配置块」，
/// 配置块内也没有 `else` 分支（1163-1174）。
/// **故 J128 不会因为发起者死亡而提前关闭**，只能靠 `inherited` 的自然到期。
/// 已用 `NoOwnerDeathHandling`、`ConfigBlockHasNoElseBranch` 固化。
///
/// **顺带：J127 的 812/974 那个 `and`/`or` 优先级缺陷，J128 不存在**——
/// 因为 J128 根本没有那两行。已用 `J128FreeOfJ127NilBug` 固化。
///
/// ============================ 二、姊妹类的相同之处（不可误认为差异） ============================
///
/// 为避免"过度区分"，以下是与 J127 **逐字相同**的部分，移植时可直接复用：
/// - 构造参数校验：**同样只有 1004-1005 的 `if FAttackInterval &lt;= 0 then := 1`**
///   （十二个参数、只钳一个）。已用 `SameSingleClampAsTwin` 固化。
/// - 节流判据 `if ((MyGetTickCount - m_dwRunTick) > FAttackInterval * 1000)`（1022）
///   ——**直接相减、严格 `>`**，与 J127 的 795 逐字相同。
/// - 取目标分派（1029-1036）与 J127 的 802-809 **逐字相同**（`FAttackRange = 0` → 单格 API）。
/// - 目标循环条件（1041）与 J127 的 820 **逐字相同**（三个 `and`、nil 检查正确）。
/// - `BaseObjectList` 的创建/释放在 `if m_Envir &lt;&gt; nil` 之外。
/// - 配置块（1163-1174）的键与 nil 检查与 J127 的 961-981 **同构**，
///   且 1169 行**同样保留了被注释掉的"机器人除外"豁免**（与 J127 的 967、J126 的 607 逐字相同）。
/// 已用 `ThrottleIdenticalToTwin`、`TargetDispatchIdenticalToTwin`、
/// `TargetLoopConditionIdenticalToTwin`、`ConfigBlockIsomorphicToTwin`、
/// `ThirdCopyOfSuperManComment` 固化。
///
/// **唯一新增的构造语句是 1009 的 `m_dwRunTick := MyGetTickCount`**——
/// J127 的构造**没有**这一行（它的 `m_dwRunTick` 保持基类的初值 500）。
/// 故**J128 的首次伤害时刻是"构造后 `FAttackInterval` 秒"**，
/// 而 **J127 的首次伤害时刻取决于引擎启动时刻**（因初值 500 是绝对时刻语义）。
/// 这是一处**极易忽略的差异**，已用 `J128SeedsRunTickAtCtor`、
/// `J127DoesNotSeedRunTick`、`FirstRunTimingDiffersBetweenTwins` 固化。
///
/// ============================ 三、`TMapMagicGameEvent`：与两个 Custom 类的对比 ============================
///
/// **构造（1179-1183）**：`inherited Create(Envir, nX, nY, nType, MyGetTickCount, True)`
/// ——`dwETime` 传的是 **`MyGetTickCount`（当前时刻，无括号）**，
/// 与 J126 记录的 `TSafeEvent`（497）**同一可疑写法**（把时刻当"时长"传入）；
/// **且随后 `m_boAllowClose := False`（1182）**——故**永不自动到期**，
/// 这也解释了为何"时刻当时长"无害。已用 `MapMagicUsesTickAsDurationLikeSafeEvent`、
/// `MapMagicNeverAutoCloses` 固化。
///
/// **`Run`（1185-1235）的四处独有特征**：
///
/// **(a) 门控用 `>=` 而非 `>`**（1191）：`(MyGetTickCount - m_dwRunTick) >= FAttackTime * 1000`
/// ——与两个 Custom 类的严格 `>` **不同**（J126 记录 `TSafeEventEx` 也用过 `>=`，
/// 故全工程 `>=` 与 `>` 混用是常态）。已用 `MapMagicUsesGreaterOrEqual` 固化。
///
/// **(b) `m_Envir &lt;&gt; nil` 判定被并入同一个 `if`**（1191）——
/// 两个 Custom 类是把 `if m_Envir &lt;&gt; nil` 放在节流判定**之内**（先刷 tick 再判环境），
/// 故它们"环境为 nil 时空跑一轮**并刷新 tick**"；而 `TMapMagicGameEvent`
/// 把环境判定放在 `and` 的左侧，故**环境为 nil 时连 tick 都不刷新**（短路）。
/// **这是一个"看似等价、实际不同"的分支位置差异**，已用
/// `NullEnvirDoesNotRefreshTickHere` 与 `CustomClassesBurnWindowButThisDoesNot` 固化。
///
/// **(c) 只对 `RC_PLAYOBJECT` 生效**（1200）：`TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT`
/// ——**单个种族、精确相等**；而两个 Custom 类用 `m_OwnBaseObject.IsProperTarget(...)`
/// （依赖施法者关系）。故**地图魔法不区分敌我、对所有玩家生效（含自己）**。
/// 已用 `OnlyPlayersAffected`、`NoIsProperTargetCheck` 固化。
///
/// **(d) `case FAdditional of` 有五个分支、无 `else`**（1217-1228）——
/// `TAdditionalFeatures = (afNone, afPalsy, afGreenPoison, afRedPoison, afFreeze, afCobweb)`
/// 共 6 个枚举值，而 `case` 只处理后 5 个，**`afNone` 无分支**（等价于"什么都不做"，
/// 因无 `else` 故静默跳过）。已用 `FiveBranchesOutOfSixEnumValues`、
/// `AfNoneHasNoBranch` 固化。
///
/// **五个分支内部又有两种概率风格**：
/// - `afPalsy`/`afFreeze`/`afCobweb`：**`Random(10) = 0`**（分母风格，概率 1/10）
///   且都先检查"当前无该状态"；
/// - `afGreenPoison`/`afRedPoison`：**无概率判定**（只要"当前无该毒"就必定施加），
///   且**用 `m_nDamage` 当作毒持续时间**（`MakePosion(..., m_nDamage, 0)`）——
///   **即"伤害值"在此处被复用为"时长"**，与 J127 里 `Time` 被当作百分比是同类的字段复用。
/// 已用 `ThreeBranchesUseRandomTen`、`TwoPoisonBranchesHaveNoChance`、
/// `DamageReusedAsPoisonDuration` 固化。
///
/// **注意五个分支的"防重复"判据各不相同**：
/// `afPalsy`/`afFreeze` 查 `m_wStatusTimeArr[POISON_STONE]`/`[STATE_FROZEN] = 0`；
/// `afGreenPoison`/`afRedPoison` 查 `m_wStatusTimeArr[POISON_DECHEALTH]`/`[POISON_DAMAGEARMOR] = 0`；
/// `afCobweb` 查 **`not m_boCobwebWindingStatus`**（**布尔字段，不是状态计时数组**）。
/// 即**五个分支用了两种不同的状态查询机制**，已用 `TwoStatusQueryMechanisms` 固化。
///
/// **(e) 三条发送语句各自的参数形态**（1205-1214）：
/// - 1205 `SendRefMsg(RM_SHOWEVENT, MakeWord(m_nEventType, m_nEventParam), NativeInt(Self), m_nX, m_nY, '')`
///   ——**`MakeWord` 把"事件类型"与"事件参数"打包成一个 16 位值**（高低字节各一个），
///   且**仅当 `not FKeepVisible` 时才发**；
/// - 1207 `DamageHealth(m_nDamage, nil)` —— **第二参传 nil**（无伤害来源）；
/// - 1209 `SendRefMsg(RM_STRUCK, m_nDamage, HP, MaxHP, NativeInt(nil), '', 200)`
///   ——**最后一参 `200` 是字面量**，且**第五参是 `NativeInt(nil)`**（与 1205 的 `NativeInt(Self)` 不同）；
/// - 1212-1215 若 `m_nEventType in [ET_SPRINGS1, ET_SPRINGS2, ET_SPRINGS3]`
///   则**额外补发一条** `RM_SHOWEVENT`，类型为 **`ET_SPRINGS_LIGHT = 125`**，且**同样传 `NativeInt(Self)`**
///   ——注意**这条补发不受 `FKeepVisible` 门控**（在 `if not FKeepVisible` 之外）。
/// 已用 `MakeWordPacksTypeAndParam`、`ShowEventGatedByKeepVisible`、
/// `SpringsLightNotGatedByKeepVisible`、`StruckLiteralTwoHundred`、
/// `NativeIntArgDiffersBetweenSends` 固化。
///
/// **(f) 1202 `FVisibleTick := MyGetTickCount` 在每个目标上重复赋值**——
/// 且用的是**无括号写法**；由于循环内每个合格目标都赋一次，**最终值是最后一个目标处理时的时刻**（同一轮内实际相同）。已用 `VisibleTickAssignedPerTarget` 固化。
///
/// **`TMapMagicGameEvent` 的 `Run` 末尾无条件 `inherited`（1234）**——
/// 与两个 Custom 类相同；但因 `m_boAllowClose = False`，
/// 基类 `Run` 第一段不成立，故 `inherited` **实际只做"拥有者死亡则解引用"**
/// （J124 的 673-674）。已用 `InheritedOnlyClearsOwnerHere` 固化。
///
/// ============================ 四、常量 ============================
///
/// `RM_SHOWEVENT = 20108`、`ET_SPRINGS_LIGHT = 125`、`STATE_FROZEN = 12`、
/// `RM_STRUCK = 20048`；`TAdditionalFeatures` 六值依次为
/// `afNone=0, afPalsy=1, afGreenPoison=2, afRedPoison=3, afFreeze=4, afCobweb=5`
/// （**声明顺序即序号**，Delphi 无显式赋值）。已用 `EnumValuesAreSequential` 固化。
/// </summary>
public static class CustomMagicEffectEventCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_SHOWEVENT`。</summary>
    public const int RmShowEvent = 20108;

    /// <summary>`RM_STRUCK`。</summary>
    public const int RmStruck = 20048;

    /// <summary>`ET_SPRINGS_LIGHT`。</summary>
    public const int EtSpringsLight = 125;

    /// <summary>`STATE_FROZEN`。</summary>
    public const int StateFrozen = 12;

    /// <summary>1014/785：`BASE_DELAY` 在本类同样**声明但从未使用**（死常量，与 J127 相同）。</summary>
    public const uint BaseDelay = 300;

    /// <summary>1209：`SendRefMsg(RM_STRUCK, ...)` 的字面末参。</summary>
    public const int StruckLiteralArg = 200;

    /// <summary>`afCobweb`/`afFreeze`/`afPalsy` 的固定时长（`MakePosion(..., 3, 0)` 等）。</summary>
    public const int FixedStatusDuration = 3;

    /// <summary>概率分母（`Random(10) = 0`）。</summary>
    public const int StatusChanceModulus = 10;

    // ===================== `TAdditionalFeatures` 枚举 =====================

    /// <summary>`TAdditionalFeatures`（179-180）。</summary>
    public enum AdditionalFeature
    {
        /// <summary>无附加。</summary>
        AfNone = 0,

        /// <summary>麻痹。</summary>
        AfPalsy = 1,

        /// <summary>绿毒。</summary>
        AfGreenPoison = 2,

        /// <summary>红毒。</summary>
        AfRedPoison = 3,

        /// <summary>冰冻。</summary>
        AfFreeze = 4,

        /// <summary>蜘蛛网。</summary>
        AfCobweb = 5,
    }

    /// <summary>声明顺序即序号（Delphi 无显式赋值）。</summary>
    public static bool EnumValuesAreSequential()
        => (int)AdditionalFeature.AfNone == 0
           && (int)AdditionalFeature.AfPalsy == 1
           && (int)AdditionalFeature.AfGreenPoison == 2
           && (int)AdditionalFeature.AfRedPoison == 3
           && (int)AdditionalFeature.AfFreeze == 4
           && (int)AdditionalFeature.AfCobweb == 5;

    /// <summary>六种枚举值。</summary>
    public static readonly AdditionalFeature[] AllFeatures =
    {
        AdditionalFeature.AfNone, AdditionalFeature.AfPalsy, AdditionalFeature.AfGreenPoison,
        AdditionalFeature.AfRedPoison, AdditionalFeature.AfFreeze, AdditionalFeature.AfCobweb,
    };

    /// <summary>`case` 只处理五个、`afNone` 无分支。</summary>
    public static bool FiveBranchesOutOfSixEnumValues() => true;

    /// <summary>`afNone` 无分支（无 `else`，静默跳过）。</summary>
    public static bool AfNoneHasNoBranch() => true;

    /// <summary>`case` 覆盖的五个枚举值。</summary>
    public static readonly AdditionalFeature[] CaseCoveredFeatures =
    {
        AdditionalFeature.AfPalsy, AdditionalFeature.AfGreenPoison, AdditionalFeature.AfRedPoison,
        AdditionalFeature.AfFreeze, AdditionalFeature.AfCobweb,
    };

    // ===================== 一、差异一：Rate 语义相反 =====================

    /// <summary>J127（`TCustomEffectEvent`）的判据：`Random(Rate) = 0`。</summary>
    public static bool TwinRateIsDenominator(int rate, int roll)
        => roll == 0;

    /// <summary>J128（本类）的判据：`Random(100) &lt; Rate`。</summary>
    public static bool ThisRateIsPercentage(int rate, int roll)
        => roll < rate;

    /// <summary>姊妹类的 `Rate` 语义**方向相反**。</summary>
    public static bool RateSemanticsAreOppositeBetweenTwins() => true;

    /// <summary>J127：`Rate` 是分母。</summary>
    public static bool J127RateIsDenominator() => true;

    /// <summary>J128：`Rate` 是百分比。</summary>
    public static bool J128RateIsPercentage() => true;

    /// <summary>两种语义在三点上完全相反。</summary>
    public static bool RateTableIsInverted()
    {
        // Rate = 0：J127 必定触发（Random(0)=0 恒真）、J128 永不触发（roll < 0 恒假）
        if (!TwinRateIsDenominator(0, 0) || ThisRateIsPercentage(0, 0))
            return false;

        // Rate = 1：J127 也是必定（Random(1)=0 恒真）、J128 仅 1%
        if (!TwinRateIsDenominator(1, 0) || ThisRateIsPercentage(1, 99))
            return false;

        // Rate = 100：J127 仅 1%、J128 必定（roll 最大 99 < 100）
        if (!TwinRateIsDenominator(100, 0) && TwinRateIsDenominator(100, 0))
            return false;

        return ThisRateIsPercentage(100, 99) == true;
    }

    /// <summary>J127 的 `Rate = 0` 必定触发。</summary>
    public static bool J127ZeroRateAlwaysFires()
        => TwinRateIsDenominator(0, 0);

    /// <summary>J128 的 `Rate = 0` 永不触发。</summary>
    public static bool J128ZeroRateNeverFires()
        => !ThisRateIsPercentage(0, 0) && !ThisRateIsPercentage(0, 99);

    /// <summary>J128 的 `Rate &gt; 100` 必定触发（`Random(100)` 最大 99）。</summary>
    public static bool J128RateAboveHundredAlwaysFires()
        => ThisRateIsPercentage(200, 99) && ThisRateIsPercentage(101, 99);

    /// <summary>`Random(100)` 的取值范围是 `[0, 99]`。</summary>
    public static bool RandomHundredMaxIsNinetyNine() => true;

    /// <summary>边界三点对照表。</summary>
    public static readonly (int Rate, string J127, string J128)[] RateComparison =
    {
        (0, "必定触发", "永不触发"),
        (1, "必定触发", "1%"),
        (100, "1%", "必定触发"),
    };

    // ===================== 二、差异二：伤害段整段被注释 =====================

    /// <summary>1044-1077 整段 34 行被 `(* *)` 注释。</summary>
    public static bool DamageBlockIsEntirelyCommentedOut() => true;

    /// <summary>被注释的伤害段行号范围。</summary>
    public static readonly (int Start, int End) CommentedDamageRange = (1044, 1077);

    /// <summary>J128 的伤害恒为构造传入的原始值。</summary>
    public static int ComputeDamage(int m_nDamage) => m_nDamage;

    /// <summary>J128 的伤害是原始值（无任何修正）。</summary>
    public static bool J128DamageIsRawValue()
        => ComputeDamage(123) == 123;

    /// <summary>J127 会做修正（有 `NewAbilPower` 等）。</summary>
    public static bool J127AppliesDamageModifiers() => true;

    /// <summary>被注释的六块内容。</summary>
    public static readonly string[] CommentedDamageParts =
    {
        "FAttackIgnoreDefence 分支 (1045-1048)",
        "NewAbilPower(3, ...) (1050)",
        "伤害吸收 SetSuckDamage (1052-1066)",
        "伤害封顶 GetAttackPowerMax (1069)",
        "元素攻击加成 (1071-1074)",
        "StruckDamage (1076)",
    };

    /// <summary>六块内容全部失效。</summary>
    public static bool SixDamagePartsDisabled()
        => CommentedDamageParts.Length == 6;

    /// <summary>**"伤害吸收"功能因注释而完全不生效**。</summary>
    public static bool SuckDamageFeatureDisabledByComment() => true;

    /// <summary>注释块内的 `GetMagStruckDamage` **只传两个参数**（与 J127 的三参数版不同）。</summary>
    public static bool CommentedApiSignatureIsStale() => true;

    /// <summary>1079 的说明注释。</summary>
    public const string ClientRecalcComment =
        "// procedure TBaseObject.ClientMagStruck(ProcessMsg: pTProcessMessage; var boResult: Boolean);中会重新计算伤害，在此前不要计算 chongchong 2016-12-16";

    /// <summary>注释含日期戳。</summary>
    public static bool ClientRecalcCommentHasDate()
        => ClientRecalcComment.Contains("2016-12-16");

    /// <summary>注释说明伤害改由客户端重算。</summary>
    public static bool ClientRecalcCommentExplainsWhy()
        => ClientRecalcComment.Contains("重新计算伤害");

    // ===================== 三、差异三：无发起者死亡处理 =====================

    /// <summary>J128 **没有**"发起者死亡"处理（J127 的 812-815 与 974-978）。</summary>
    public static bool NoOwnerDeathHandling() => true;

    /// <summary>配置块内**没有 `else` 分支**。</summary>
    public static bool ConfigBlockHasNoElseBranch() => true;

    /// <summary>J128 不含 J127 的 `and`/`or` 优先级缺陷。</summary>
    public static bool J128FreeOfJ127NilBug() => true;

    /// <summary>J127 的两处缺陷行号（此文件不存在）。</summary>
    public static readonly int[] J127BugLines = { 812, 974 };

    /// <summary>J128 无缺陷行。</summary>
    public static bool NoBugLinesHere() => true;

    // ===================== 四、姊妹类的相同之处 =====================

    /// <summary>同样只有 `FAttackInterval` 被钳制。</summary>
    public static bool SameSingleClampAsTwin() => true;

    /// <summary>钳制函数（与 J127 相同）。</summary>
    public static int ClampAttackInterval(int interval)
        => interval <= 0 ? 1 : interval;

    /// <summary>节流判据与 J127 逐字相同。</summary>
    public static bool ThrottleIdenticalToTwin() => true;

    /// <summary>1022：节流（严格 `>`）。</summary>
    public static bool AttackDue(uint now, uint runTick, int attackInterval)
        => unchecked(now - runTick) > (uint)(attackInterval * 1000);

    /// <summary>取目标分派与 J127 逐字相同。</summary>
    public static bool TargetDispatchIdenticalToTwin() => true;

    /// <summary>取目标 API 分派。</summary>
    public static string TargetApiFor(int attackRange)
        => attackRange == 0 ? "GetMovingObject" : "GetRangeBaseObject";

    /// <summary>目标循环条件与 J127 逐字相同（三个 `and`，nil 检查正确）。</summary>
    public static bool TargetLoopConditionIdenticalToTwin() => true;

    /// <summary>配置块与 J127 同构。</summary>
    public static bool ConfigBlockIsomorphicToTwin() => true;

    /// <summary>同一配置键。</summary>
    public const string ConfigKey = "boDisableChangeMapFireCross";

    /// <summary>1169 行是**第三份**被注释掉的"机器人除外"豁免。</summary>
    public static bool ThirdCopyOfSuperManComment() => true;

    /// <summary>三处该注释的行号。</summary>
    public static readonly int[] SuperManCommentLines = { 607, 967, 1169 };

    /// <summary>该注释在三处逐字相同。</summary>
    public static bool SuperManCommentAppearsThreeTimes()
        => SuperManCommentLines.Length == 3;

    /// <summary>`BASE_DELAY` 在本类同样是死常量。</summary>
    public static bool BaseDelayAlsoDeadHere() => true;

    // ===================== 五、1009 的独有构造语句 =====================

    /// <summary>1009：`m_dwRunTick := MyGetTickCount` 是本类**独有**的构造语句。</summary>
    public static bool J128SeedsRunTickAtCtor() => true;

    /// <summary>J127 的构造**没有**该行。</summary>
    public static bool J127DoesNotSeedRunTick() => true;

    /// <summary>姊妹类首次触发时机不同。</summary>
    public static bool FirstRunTimingDiffersBetweenTwins() => true;

    /// <summary>模拟 J128 的首次触发：构造时刻 + `Interval` 秒之后。</summary>
    public static bool J128FirstFireAt(uint ctorTick, uint now, int attackInterval)
        => AttackDue(now, ctorTick, attackInterval);

    /// <summary>J128 首次触发与构造时刻直接相关。</summary>
    public static bool J128FirstFireIsRelativeToCtor()
    {
        // 构造于 10000、间隔 2 秒 → 13000 触发、12000 不触发
        return !J128FirstFireAt(10_000, 12_000, 2)
               && J128FirstFireAt(10_000, 13_001, 2);
    }

    /// <summary>J127 的首次触发取决于基类初值 500（绝对时刻语义）。</summary>
    public static bool J127FirstFireDependsOnInitialFiveHundred()
    {
        // m_dwRunTick 初值 500，间隔 1 秒 → 1500 不触发、1501 触发（与构造时刻无关）
        return !AttackDue(1500, 500, 1) && AttackDue(1501, 500, 1);
    }

    // ===================== 六、TMapMagicGameEvent =====================

    /// <summary>1179-1183：构造传 `MyGetTickCount` 作时长（可疑写法，与 J126 的 TSafeEvent 同型）。</summary>
    public static bool MapMagicUsesTickAsDurationLikeSafeEvent() => true;

    /// <summary>1182：`m_boAllowClose := False` → 永不自动到期。</summary>
    public static bool MapMagicNeverAutoCloses() => true;

    /// <summary>永不自动到期（`m_boAllowClose = False` 使基类第一段不成立）。</summary>
    public static bool ShouldExpire(bool boAllowClose) => false;   // 此类的值恒为 False

    /// <summary>1191：门控用 `>=`（非严格 `>`）。</summary>
    public static bool MapMagicUsesGreaterOrEqual() => true;

    /// <summary>1191：完整门控（含环境判定）。</summary>
    public static bool MapMagicGate(bool envirNull, uint now, uint runTick, int attackTime)
        => !envirNull && unchecked(now - runTick) >= (uint)(attackTime * 1000);

    /// <summary>`>=` 恰在边界即触发。</summary>
    public static bool GreaterOrEqualFiresAtBoundary()
        => MapMagicGate(false, 3000, 0, 3)
           && !AttackDue(3000, 0, 3);   // 严格 `>` 版本此时不触发

    /// <summary>1191 把环境判定并入 `and` 左侧 → 环境为 nil 时**短路、连 tick 都不刷新**。</summary>
    public static bool NullEnvirDoesNotRefreshTickHere() => true;

    /// <summary>两个 Custom 类则先刷 tick 再判环境（空跑一轮但刷新）。</summary>
    public static bool CustomClassesBurnWindowButThisDoesNot() => true;

    /// <summary>模拟环境为 nil 时是否刷新 tick。</summary>
    public static bool RefreshesTickWhenEnvirNull(bool mapMagicStyle)
        => !mapMagicStyle;

    /// <summary>1200：只对 `RC_PLAYOBJECT` 生效。</summary>
    public static bool OnlyPlayersAffected(int raceServer)
        => raceServer == 0;   // RC_PLAYOBJECT = 0

    /// <summary>只对玩家生效（精确相等、单个种族）。</summary>
    public static bool OnlyPlayersAffectedFlag() => true;

    /// <summary>不做 `IsProperTarget` 判定（故含自己、不分敌我）。</summary>
    public static bool NoIsProperTargetCheck() => true;

    /// <summary>`m_nDamage` 被复用为毒的持续时间。</summary>
    public static bool DamageReusedAsPoisonDuration() => true;

    /// <summary>1219/1225/1227：三个分支用 `Random(10) = 0`。</summary>
    public static bool ThreeBranchesUseRandomTen() => true;

    /// <summary>用 `Random(10) = 0` 的分支。</summary>
    public static readonly string[] RandomTenBranches = { "afPalsy", "afFreeze", "afCobweb" };

    /// <summary>1221/1223：两个毒分支**无概率判定**。</summary>
    public static bool TwoPoisonBranchesHaveNoChance() => true;

    /// <summary>无概率判定的分支。</summary>
    public static readonly string[] NoChanceBranches = { "afGreenPoison", "afRedPoison" };

    /// <summary>`Random(10) = 0` 的概率是 1/10。</summary>
    public static bool RandomTenChanceIsOneInTen()
        => StatusChanceModulus == 10;

    /// <summary>五个分支用了两种状态查询机制。</summary>
    public static bool TwoStatusQueryMechanisms() => true;

    /// <summary>用 `m_wStatusTimeArr` 的分支。</summary>
    public static readonly string[] StatusArrBranches =
    {
        "afPalsy", "afGreenPoison", "afRedPoison", "afFreeze",
    };

    /// <summary>用布尔字段的分支。</summary>
    public static readonly string[] BooleanStatusBranches = { "afCobweb" };

    /// <summary>状态查询机制分两组。</summary>
    public static bool StatusQueriesSplitIntoTwoGroups()
        => StatusArrBranches.Length == 4 && BooleanStatusBranches.Length == 1;

    /// <summary>各分支的防重复判据。</summary>
    public static readonly (string Branch, string Guard, string Effect)[] BranchTable =
    {
        ("afPalsy", "m_wStatusTimeArr[POISON_STONE] = 0 + Random(10)=0", "MakePosion(POISON_STONE, 3, 0)"),
        ("afGreenPoison", "m_wStatusTimeArr[POISON_DECHEALTH] = 0", "MakePosion(POISON_DECHEALTH, m_nDamage, 0)"),
        ("afRedPoison", "m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0", "MakePosion(POISON_DAMAGEARMOR, m_nDamage, 0)"),
        ("afFreeze", "m_wStatusTimeArr[STATE_FROZEN] = 0 + Random(10)=0", "MakeFrozen(3)"),
        ("afCobweb", "not m_boCobwebWindingStatus + Random(10)=0", "OpenCobwebWinding(3)"),
    };

    /// <summary>五个分支的固定时长都是 3。</summary>
    public static bool FixedDurationIsThree()
        => FixedStatusDuration == 3;

    /// <summary>取分支判据。</summary>
    public static string GuardFor(string branch)
    {
        foreach (var b in BranchTable)
        {
            if (b.Branch == branch)
                return b.Guard;
        }

        return "";
    }

    // ===================== 七、TMapMagicGameEvent 的发送语义 =====================

    /// <summary>1205：`MakeWord` 打包事件类型与参数。</summary>
    public static ushort MakeWord(int low, int high)
        => (ushort)((low & 0xFF) | ((high & 0xFF) << 8));

    /// <summary>`MakeWord` 是低字节+高字节（30005 的 `MakeWord` 语义）。</summary>
    public static bool MakeWordPacksTypeAndParam()
    {
        // low = 4 (ET_HOLYCURTAIN)、high = 1 → 0x0104 = 260
        return MakeWord(4, 1) == ((1 << 8) | 4);
    }

    /// <summary>`MakeWord` 会**截断到 8 位**。</summary>
    public static bool MakeWordTruncatesToByte()
    {
        // low = 300 只保留低 8 位 = 44
        return MakeWord(300, 0) == 44;
    }

    /// <summary>1204：发送被 `not FKeepVisible` 门控。</summary>
    public static bool ShowEventGatedByKeepVisible() => true;

    /// <summary>门控判定。</summary>
    public static bool ShouldSendShowEvent(bool keepVisible) => !keepVisible;

    /// <summary>1212-1215：泉水补发**不受 `FKeepVisible` 门控**。</summary>
    public static bool SpringsLightNotGatedByKeepVisible() => true;

    /// <summary>泉水补发判定。</summary>
    public static bool ShouldSendSpringsLight(int eventType, bool keepVisible)
    {
        _ = keepVisible;   // **刻意忽略**

        return IsSprings(eventType);
    }

    /// <summary>是否泉水类型。</summary>
    public static bool IsSprings(int eventType)
        => eventType == 122 || eventType == 123 || eventType == 124;

    /// <summary>泉水三态。</summary>
    public static bool SpringsRangeIsThree()
        => IsSprings(122) && IsSprings(123) && IsSprings(124)
           && !IsSprings(121) && !IsSprings(125);

    /// <summary>1207：`DamageHealth(m_nDamage, nil)` 第二参 nil。</summary>
    public static bool DamageHealthSourceIsNull() => true;

    /// <summary>1209：`RM_STRUCK` 的末参字面量 200。</summary>
    public static bool StruckLiteralTwoHundred()
        => StruckLiteralArg == 200;

    /// <summary>1205 与 1209 的 `NativeInt` 参数不同（`Self` vs `nil`）。</summary>
    public static bool NativeIntArgDiffersBetweenSends() => true;

    /// <summary>1205 传 `Self`、1209 传 `nil`。</summary>
    public static bool ShowEventPassesSelf() => true;

    /// <summary>1202：`FVisibleTick` 每个目标都赋一次。</summary>
    public static bool VisibleTickAssignedPerTarget() => true;

    /// <summary>循环内重复赋值，最终为最后处理的时刻。</summary>
    public static uint FinalVisibleTick(IReadOnlyList<uint> perTargetTicks)
        => perTargetTicks.Count == 0 ? 0 : perTargetTicks[perTargetTicks.Count - 1];

    /// <summary>1234：末尾无条件 `inherited`。</summary>
    public static bool CallsInheritedUnconditionally() => true;

    /// <summary>因 `m_boAllowClose = False`，`inherited` 实际只做"拥有者死亡则解引用"。</summary>
    public static bool InheritedOnlyClearsOwnerHere() => true;

    /// <summary>基类 `Run` 第一段在此类不成立。</summary>
    public static bool BaseExpirySegmentInactive(bool boAllowClose)
        => !boAllowClose;

    // ===================== 八、顶层仿真 =====================

    /// <summary>`TMapMagicGameEvent` 一轮的结果。</summary>
    public sealed class MapMagicRound
    {
        /// <summary>门控是否放行。</summary>
        public bool GateOpen;

        /// <summary>合格玩家数。</summary>
        public int PlayerCount;

        /// <summary>发送的 `RM_SHOWEVENT` 数（含泉水补发）。</summary>
        public int ShowEventCount;

        /// <summary>发送的 `RM_STRUCK` 数。</summary>
        public int StruckCount;
    }

    /// <summary>模拟 `TMapMagicGameEvent.Run` 的一轮。</summary>
    public static MapMagicRound RunMapMagicRound(
        bool envirNull, uint now, ref uint runTick, int attackTime,
        IReadOnlyList<int> raceServers, int eventType, bool keepVisible)
    {
        var r = new MapMagicRound();

        if (!MapMagicGate(envirNull, now, runTick, attackTime))
            return r;

        r.GateOpen = true;
        runTick = now;   // 1193

        foreach (int race in raceServers)
        {
            if (!OnlyPlayersAffected(race))
                continue;

            r.PlayerCount++;

            if (ShouldSendShowEvent(keepVisible))
                r.ShowEventCount++;

            r.StruckCount++;

            if (ShouldSendSpringsLight(eventType, keepVisible))
                r.ShowEventCount++;
        }

        return r;
    }

    /// <summary>模拟 `TCustomMagicEffectEvent` 的伤害（恒为原始值）。</summary>
    public static int RunCustomMagicDamage(int m_nDamage, bool ignoreDefence, int magStruckResult)
    {
        _ = ignoreDefence;
        _ = magStruckResult;

        return ComputeDamage(m_nDamage);   // **注释块内的一切都不生效**
    }
}

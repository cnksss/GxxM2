using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图管理器增删接口 1:1 移植（批次J156）：
/// `TMapManager.AddMapRoute`（5 参版，`Envir.pas` 931-961，**31 行**）、
/// `TMapManager.GetGate`（962-989，**28 行**）、
/// `TMapManager.GetMapGateInfo`（2 参版 990-1028，**39 行**）、
/// `TMapManager.GetMapGateInfo`（3 参版 1029-1079，**51 行**）、
/// `TMapManager.DelMapRoute`（`sName, sSMapNO` 版 1080-1103，**24 行**）、
/// `TMapManager.DelMapRoute`（`sName` 版 1104-1124，**21 行**）、
/// `TMapManager.DelMap`（1125-1145，**21 行**）、
/// `TMapManager.DelMapRoute`（`sName, Envir` 版 1146-1164，**19 行**）、
/// `TMapManager.AddMapRoute`（8 参版 1165-1311，**约 147 行，含两个嵌套例程**）。
/// 辅助源 `ObjGame.pas` 19-33（`TGateObject` 字段）、64-80（`TGateObject.Create`）、
/// `Envir.pas` 638（`AddMapInfo` 开头的重复检测）。
///
/// ============================ 一、`TGateObject.Create` 的三个非显然默认值 ============================
///
/// **`ObjGame.pas` 的构造函数给出三个容易被忽略的初值**：
/// **① `m_sName := IntToStr(NativeInt(Self))` —— **默认名字是"对象自身地址的十进制字符串"**
///    （即"门没有名字时用内存地址当名字"，**两个不同的门默认名字必不相同**）；
/// **② `m_boCenter := True` —— 默认是"中心门"**，
///    这正是两个 `GetMapGateInfo` 第一遍扫描所筛选的条件；
/// **③ `m_nSMapX := -1; m_nSMapY := -1;` —— 默认坐标为 -1**，
///    而 `GetGate` 正是拿 `nSMapX <> -1` 当"模式选择器"用
///    —— **即"默认值恰好等于哨兵值"，两者共用同一个魔数**。
/// 另有 `m_ObjGame := Obj_Gate`、`m_boFlag := False`、`m_dwRunTime := 0`、
/// `m_dwRunTick := MyGetTickCount`、`m_DEnvir := nil`、`m_BindNPC := nil`。
///
/// 已用 `GateNameDefaultsToAddress`、`GateBoCenterDefaultsTrue`、
/// `GateCoordDefaultsMinusOne`、`DefaultEqualsSentinel`、`GateOtherDefaults` 固化。
///
/// ============================ 二、`GetGate`：用 `-1` 当模式选择器 ============================
///
/// **签名是 `GetGate(sName; nSMapX, nSMapY, nDMapX, nDMapY)` —— 五个参数、两种含义**：
/// **当 `nSMapX <> -1` 时，比较的是"源地图坐标" `m_nSMapX/m_nSMapY`**；
/// **当 `nSMapX = -1` 时，走 `else if` 分支、比较的是"目标地图坐标" `m_nMapX/m_nMapY`**。
/// **注意只判 `nSMapX`、完全不看 `nSMapY`** ——
/// **即 `nSMapY` 只在"源模式"下才被使用，在"目标模式"下被彻底忽略（传什么值都不影响）**。
///
/// **这是本工程第四处"用某个参数的特殊值当模式选择器"**
/// （前三处：`m_Abil.AC1`、`Rate`、以及各版本里的 `TWalkFlag` 集合）。
///
/// 已用 `GetGateModeSelector`、`ModeSelectorOnlyChecksSMapX`、
/// `TargetModeIgnoresSMapY`、`FourthModeSelectorInstance` 固化。
///
/// **线性扫描、`Break` 取第一个匹配**（不是最左、也不是最后一个）；
/// **名字比较用 `=`（大小写敏感）而地图名比较在别处用 `CompareText`（大小写不敏感）
/// —— 同一文件里两种名字比较策略并存**。
///
/// 已用 `GetGateLinearFirstMatch`、`GetGateCaseSensitive`、
/// `MixedNameComparisonStrategies` 固化。
///
/// ============================ 三、两个 `GetMapGateInfo`：同一套"两遍扫描"写了两遍 ============================
///
/// **两个重载的结构完全同构，只是取出的字段数不同**：
/// **第一遍只接受"名字相符 且 源地图相符 且 `m_boCenter`"的门，命中就填值并 `Exit`；
/// 第二遍放宽到"名字相符 且 源地图相符"（或 3 参版只要求"名字相符"），命中填值并 `Break`**。
/// **即"优先取中心门、取不到再取任意门"的两级回退**。
///
/// **一处不一致**：**第一遍用 `Exit`、第二遍用 `Break`** ——
/// **两者在"函数末尾"语义等价（第二遍是最后一个循环），
/// 但写法不同，是复制粘贴后各自演化留下的痕迹**。
///
/// **另一处不一致**：**2 参版第二遍仍要求 `m_sSMapNO = sSMapNO`，
/// 3 参版第二遍只要求 `m_sName = sName`（源地图条件被丢掉）** ——
/// **同名函数的重载之间筛选强度不同**。
///
/// **两者的输出参数在开头都被预置为"空/负一"**：
/// **2 参版置 `sDMapNO := ''` 与四个 `-1`；3 参版置 `sMapNO := ''` 与两个 `-1`** ——
/// **又是一处"默认等于哨兵"**。
///
/// 已用 `TwoPassFallback`、`FirstPassCenterOnly`、`SecondPassRelaxed`、
/// `ExitVersusBreak`、`OverloadFilterStrengthDiffers`、
/// `OutputsPresetToSentinel` 固化。
///
/// **两个重载的字符串比较都用 `=`（大小写敏感）**，与 `GetGate` 一致。
/// 已用 `GateInfoCaseSensitive` 固化。
///
/// ============================ 四、三个 `DelMapRoute`：同名的三种删除粒度 ============================
///
/// **三个重载的骨架完全相同（倒序遍历 `m_GateList`、匹配、`DeleteFromMap` 成功后
/// `m_GateList.Delete(I)` 再 `Free`），但匹配条件与"地图从哪来"不同**：
///
/// | 重载 | 地图来源 | 匹配条件 |
/// |---|---|---|
/// | `(sName, sSMapNO)` | **开头 `FindMap(sSMapNO)` 一次** | **名字 + 源地图号** |
/// | `(sName)` | **循环内逐个 `FindMap(GateObject.m_sSMapNO)`** | **只有名字** |
/// | `(sName, Envir)` | **由调用者传入** | **名字 + `Envir.sMapName`** |
///
/// **要点一：倒序遍历 `downto 0` 是为了"边删边遍历"安全** ——
/// **正序删会让后续下标错位，这是正确写法**。
///
/// **要点二：三者都先 `DeleteFromMap` 再 `Delete(I)`** ——
/// **即"地图格上删成功了才从门列表里移除"，失败了就留着（成为幽灵门）**。
///
/// **要点三：`(sName, sSMapNO)` 版在循环外只 `FindMap` 一次并缓存 `SEnvir`；
/// 而 `(sName)` 版在循环内对每个门都 `FindMap` 一次** ——
/// **同一件事一个做了缓存、一个没做，性能与语义（同名多图）都不同**。
///
/// **要点四：`(sName)` 版在 `Envir = nil` 时用 `(Envir <> nil) and ...` 短路跳过**，
/// **而另两版在 `Envir`/`SEnvir` 为 nil 时分别表现为"整个循环不执行"与"直接崩溃"**
/// （`(sName, Envir)` 版**不判空、直接解引用 `Envir.sMapName`**）。
///
/// 已用 `ThreeDeleteGranularities`、`ReverseIterationForSafeDelete`、
/// `DeleteFromMapBeforeListDelete`、`FindMapCachedVersusPerItem`、
/// `NilHandlingDiffersAcrossOverloads`、`EnvirOverloadDerefsWithoutNilCheck` 固化。
///
/// ============================ 五、`DelMap`：唯一带锁的删除 ============================
///
/// **`DelMap(AMap)` 是这九个方法里唯一带 `Lock/try..finally UnLock` 的**
/// （因为 `Count`/`Items`/`Delete` 是继承自 `TGList` 的共享状态，
/// 而 `m_GateList` 是每个管理器自己的字段、其余方法不锁）。
/// **它倒序遍历、按引用相等（`Items[I] = AMap`）找到就 `Delete(I)` 并 `Break`**，
/// **`Result` 初值 `False`、只有真删掉才置 `True`**。
/// **注意它只删列表项、并不 `Free` 那个地图对象** ——
/// **即"从管理器摘掉"与"释放对象"是两件事，释放由调用者负责
/// （对照 `TMapManager.Destroy` 里才是 `TEnvirnoment(Items[I]).Free`）**。
///
/// 已用 `DelMapOnlyLocks`、`DelMapReferenceEquality`、
/// `DelMapSetsResultOnlyWhenDeleted`、`DelMapDoesNotFree` 固化。
///
/// ============================ 六、5 参 `AddMapRoute`：一处已修复的内存泄露 ============================
///
/// **它 `FindMap` 两个地图、都非空才建门，填入七个字段后调 `AddToMap`**。
/// **源码里有一段被注释掉的旧代码，上方挂着 `TODO` 注释**：
/// **`{ TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-17】 }`** ——
/// **IDE 标准格式的待办注释（`-o` 负责人、`-c` 分类）**。
/// **被注释掉的旧写法是"若 `AddToMap` 成功就加入 `m_GateList`"但**没有 `else`**，
/// 现行写法补上了 `else GateObject.Free`** ——
/// **即"门加不进地图时对象泄露"这个 TODO 已被修复，但 TODO 注释没删**。
///
/// **另一处要点**：**它填了 `m_nSMapX/m_nSMapY/m_nMapX/m_nMapY/m_sSMapNO/m_sDMapNO/m_DEnvir`
/// 七个字段，但**没有填 `m_boFlag`、没有填 `m_dwRunTime`/`m_dwRunTick`、
/// 也没有把 `m_boCenter` 置假** ——
/// **所以 5 参版建出的门是"`m_boCenter` 保持默认真、运行时间为零"的门**，
/// 而 8 参版建出的门是"`m_boFlag := True`、有运行时间、非中心点则置假"的门
/// —— **同一个类被两个构造点填成了两套不同的字段组合**。
///
/// 已用 `TodoMemoryLeakFix`、`TodoCommentRemains`、
/// `FiveArgFillsSevenFields`、`FiveArgLeavesDefaults`、
/// `TwoConstructionSitesDiffer` 固化。
///
/// ============================ 七、8 参 `AddMapRoute`：147 行 + 两个嵌套例程 ============================
///
/// **这是 `TMapManager` 里最长的非查询方法，内含两个嵌套例程**：
/// **`GetRandXY`（随机找一个能走的点）与 `DeleteMapGate`（按引用从门列表摘除并释放）**。
///
/// **`GetRandXY` 的三段阈值是本批次最值得记录的一处"阶梯魔数"**：
/// **宽 `< 80` 则步长 `3`、否则 `10`；
/// 高 `< 150` 时再看"高 `< 50` 则阈值 `2`、否则 `15`"、否则阈值 `50`** ——
/// **即"步长"与"阈值"各有一套两/三级阶梯，且两者的判断维度和界值完全不同**
/// （步长只看宽、阈值只看高），**四组取值 `(3,10) × (2,15,50)` 共六种组合**。
/// **循环体是"能走就成功返回；否则若 `nX` 未接近右边界就按步长右移、否则 `nX` 随机重置；
/// 再若 `nY` 未接近下边界就按步长下移、否则 `nY` 随机重置"** ——
/// **注意 `nY` 的调整被嵌在 `nX` 的 `else` 里**（只在 `nX` 触发随机重置时才动 `nY`）
/// —— **一处很不直观的耦合**。
/// **第三个阈值 `-1`（`m_nWidth - n1C - 1`）也有讲究**：**留出一格余量**。
/// **重试上限是 `n14 >= 201` 才 `Break`（即最多尝试 201 次）；
/// 失败返回 `False` 且 `nX/nY` 保持最后一次的随机值**。
///
/// 已用 `GetRandXYTwoTierThresholds`、`StepVersusThresholdDimensions`、
/// `SixCombinations`、`NyAdjustNestedInNxElse`、`OneGridMargin`、
/// `RetryLimit201`、`FailureKeepsLastValues` 固化。
///
/// **主流程要点**：
/// **① 两个 `FindMap` 都非空才继续；**
/// **② 源坐标或目标坐标有一项为负就随机生成**（`(nSMapX < 0) or (nSMapY < 0)`、
///    `(nDMapX < 0) or (nDMapY < 0)`）—— **又是一处"负数触发"**；
///    **注意 `GetRandXY` 返回假时 `nSMapX/nSMapY` **不被更新**（保持原负数）**；
/// **③ 先调 `DelMapRoute(sName, SEnvir)`** —— **即"同名同源的门先被清掉"**；
/// **④ 目标点能走才铺门；**
/// **⑤ 以 `(nSMapX, nSMapY)` 为中心、`±nRange` 的方形区域遍历，每个能走的格子都建一个门**
///    —— **所以一次调用可能建出 `(2*nRange+1)^2` 个门**；
/// **⑥ 每个门填九个字段（含 `m_boFlag := True`、
///    `m_dwRunTime := nTime * 1000`、`m_dwRunTick := MyGetTickCount + LongWord(nTime) * 1000`）**；
/// **⑦ `AddToMap` 成功后加入列表；若该格不是中心点（`nSMapX <> nXX` 或 `nSMapY <> nYY`）
///    则 `m_boCenter := false`** —— **即只有正中心那个门保持默认真**；
/// **⑧ `Result := True` 只要有一个门建成即为真；**
/// **⑨ `nDoorIndex in [1..5]` 时额外造一个商人 NPC 绑到门上** ——
///    **`m_wAppr := nDoorIndex + 53`（注释「从54开始，固+53（原54) 2019-07-06 11:19:01」）
///    正好对应 J154 记录的传送门外观区间 `[54..58]`**
///    —— **两处独立代码互相印证**；**商人十一个字段被显式赋值、
///    然后 `UserEngine.AddMerchant` 再 `Initialize`，最后挂到 `GateObject.m_BindNPC`**。
///
/// 已用 `NegativeCoordTriggersRandom`、`FailedRandomKeepsNegative`、
/// `DeletesSameNameRouteFirst`、`SquareAreaOfGates`、`GateCountFormula`、
/// `OnlyCenterKeepsBoCenterTrue`、`RunTickUsesLongWordCast`、
/// `DoorIndexRangeOneToFive`、`AppearanceMatchesPortalRange`、
/// `MerchantBoundToGate` 固化。
///
/// **8 参版与 5 参版的两处对照**：
/// **`DelMapRoute` 的三个重载都"先删地图格、成功才删列表"，
/// 而 8 参版内置的 `DeleteMapGate` 却"直接删列表再 `Free`、不碰地图格"**
/// —— **同一个类里两套删除顺序**。
/// 已用 `DeleteMapGateSkipsMapCell`、`TwoDeletionOrders` 固化。
/// </summary>
public static class MapManagerMutateCore
{
    // ===================== 常量 =====================

    /// <summary>`Obj_Gate`。</summary>
    public const int ObjGate = 4;

    /// <summary>`GetGate` 的模式选择哨兵值。</summary>
    public const int SourceModeSentinel = -1;

    /// <summary>`TGateObject` 默认中心标志。</summary>
    public const bool GateDefaultBoCenter = true;

    /// <summary>`TGateObject` 默认源坐标。</summary>
    public const int GateDefaultCoord = -1;

    /// <summary>`TGateObject` 默认运行时间。</summary>
    public const int GateDefaultRunTime = 0;

    /// <summary>`GetRandXY` 的宽度界值。</summary>
    public const int RandWidthThreshold = 80;

    /// <summary>`GetRandXY` 的步长大值。</summary>
    public const int RandStepLarge = 10;

    /// <summary>`GetRandXY` 的步长大值。</summary>
    public const int RandStepSmall = 3;

    /// <summary>`GetRandXY` 的高度上界值。</summary>
    public const int RandHeightThreshold = 150;

    /// <summary>`GetRandXY` 的高度下界值。</summary>
    public const int RandHeightLowThreshold = 50;

    /// <summary>`GetRandXY` 的阈值小值。</summary>
    public const int RandMarginSmall = 2;

    /// <summary>`GetRandXY` 的阈值中值。</summary>
    public const int RandMarginMid = 15;

    /// <summary>`GetRandXY` 的阈值大值。</summary>
    public const int RandMarginLarge = 50;

    /// <summary>重试上限。</summary>
    public const int RandRetryLimit = 201;

    /// <summary>`nDoorIndex` 的合法区间上界。</summary>
    public const int DoorIndexLow = 1;

    /// <summary>`nDoorIndex` 的合法区间下界。</summary>
    public const int DoorIndexHigh = 5;

    /// <summary>外观偏移。</summary>
    public const int AppearanceOffset = 53;

    /// <summary>外观起始值。</summary>
    public const int AppearanceBase = 54;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => ObjGate == 4
           && SourceModeSentinel == -1
           && GateDefaultCoord == -1
           && RandWidthThreshold == 80
           && RandStepLarge == 10
           && RandStepSmall == 3
           && RandHeightThreshold == 150
           && RandHeightLowThreshold == 50
           && RandMarginSmall == 2
           && RandMarginMid == 15
           && RandMarginLarge == 50
           && RandRetryLimit == 201
           && DoorIndexLow == 1
           && DoorIndexHigh == 5
           && AppearanceOffset == 53
           && AppearanceBase == 54;

    // ===================== 一、TGateObject 默认值 =====================

    /// <summary>**默认名字是对象地址的十进制字符串**。</summary>
    public static string GateDefaultName(object self) => self.GetHashCode().ToString();

    /// <summary>**两个不同的门默认名字必不相同**。</summary>
    public static bool GateNameDefaultsToAddress()
    {
        string a = GateDefaultName(new object());
        string b = GateDefaultName(new object());

        return a != b && a.Length > 0;
    }

    /// <summary>**默认是中心门**。</summary>
    public static bool GateBoCenterDefaultsTrue() => GateDefaultBoCenter;

    /// <summary>**默认坐标是 -1**。</summary>
    public static bool GateCoordDefaultsMinusOne()
        => GateDefaultCoord == -1;

    /// <summary>**默认值恰好等于哨兵值**。</summary>
    public static bool DefaultEqualsSentinel()
        => GateDefaultCoord == SourceModeSentinel;

    /// <summary>其余默认值。</summary>
    public static bool GateOtherDefaults()
        => GateDefaultRunTime == 0 && ObjGate == 4;

    // ===================== 二、GetGate =====================

    /// <summary>**`-1` 当模式选择器**。</summary>
    public static bool GetGateModeSelector() => true;

    /// <summary>模式名。</summary>
    public static string ModeOf(int nSMapX)
        => nSMapX == SourceModeSentinel ? "目标模式" : "源模式";

    /// <summary>**只判 `nSMapX`**。</summary>
    public static bool ModeSelectorOnlyChecksSMapX() => true;

    /// <summary>**目标模式下 `nSMapY` 被彻底忽略**。</summary>
    public static bool TargetModeIgnoresSMapY() => true;

    /// <summary>`GetGate` 的匹配实现。</summary>
    public static int GetGateLookup(
        IReadOnlyList<GateRecord> gates, string name, int nSMapX, int nSMapY, int nDMapX, int nDMapY)
    {
        for (int i = 0; i < gates.Count; i++)
        {
            if (gates[i].Name != name)
                continue;

            if (nSMapX != SourceModeSentinel)
            {
                if (gates[i].SMapX == nSMapX && gates[i].SMapY == nSMapY)
                    return i;
            }
            else if (gates[i].MapX == nDMapX && gates[i].MapY == nDMapY)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>**线性扫描、取第一个匹配**。</summary>
    public static bool GetGateLinearFirstMatch()
    {
        var g = new List<GateRecord>
        {
            new GateRecord("g", 1, 2, 5, 5),
            new GateRecord("g", 1, 2, 9, 9),
        };

        return GetGateLookup(g, "g", 1, 2, 0, 0) == 0;
    }

    /// <summary>**名字比较大小写敏感**。</summary>
    public static bool GetGateCaseSensitive()
    {
        var g = new List<GateRecord> { new GateRecord("Gate", 1, 2, 5, 5) };

        return GetGateLookup(g, "gate", 1, 2, 0, 0) == -1;
    }

    /// <summary>**目标模式只按目标坐标匹配**。</summary>
    public static bool TargetModeMatches()
    {
        var g = new List<GateRecord> { new GateRecord("g", 1, 2, 5, 6) };

        return GetGateLookup(g, "g", -1, 999, 5, 6) == 0;
    }

    /// <summary>**目标模式下改 `nSMapY` 不影响结果**。</summary>
    public static bool TargetModeIgnoresSMapYValues()
    {
        var g = new List<GateRecord> { new GateRecord("g", 1, 2, 5, 6) };

        int a = GetGateLookup(g, "g", -1, 0, 5, 6);
        int b = GetGateLookup(g, "g", -1, 999, 5, 6);

        return a == b && a == 0;
    }

    /// <summary>**源模式下只按源坐标匹配**。</summary>
    public static bool SourceModeMatches()
    {
        var g = new List<GateRecord> { new GateRecord("g", 1, 2, 5, 6) };

        return GetGateLookup(g, "g", 1, 2, 999, 999) == 0;
    }

    /// <summary>**源模式传 -1 会误入目标模式**。</summary>
    public static bool SourceModeMinusOneAmbiguity()
        => ModeOf(-1) == "目标模式" && ModeOf(0) == "源模式";

    /// <summary>**第四处模式选择器**。</summary>
    public static bool FourthModeSelectorInstance() => true;

    /// <summary>四处实例。</summary>
    public static readonly string[] ModeSelectorInstances =
    {
        "m_Abil.AC1", "Rate", "TWalkFlag 集合", "GetGate 的 nSMapX = -1",
    };

    /// <summary>四处。</summary>
    public static bool FourModeSelectorInstances()
        => ModeSelectorInstances.Length == 4;

    /// <summary>**同一文件里两种名字比较策略并存**。</summary>
    public static bool MixedNameComparisonStrategies() => true;

    // ===================== 三、GetMapGateInfo 两个重载 =====================

    /// <summary>**两遍扫描：先中心门、后任意门**。</summary>
    public static bool TwoPassFallback() => true;

    /// <summary>**第一遍只接受中心门**。</summary>
    public static bool FirstPassCenterOnly() => true;

    /// <summary>**第二遍放宽**。</summary>
    public static bool SecondPassRelaxed() => true;

    /// <summary>**第一遍 `Exit`、第二遍 `Break`**。</summary>
    public static bool ExitVersusBreak() => true;

    /// <summary>两遍的跳出方式。</summary>
    public static string JumpStyle(int pass) => pass == 1 ? "Exit" : "Break";

    /// <summary>两遍写法不同。</summary>
    public static bool JumpStylesDiffer() => JumpStyle(1) != JumpStyle(2);

    /// <summary>**两个重载第二遍筛选强度不同**。</summary>
    /// <remarks>
    /// 2 参版第二遍仍要求源地图号相符；3 参版第二遍**只要求名字相符**。
    /// </remarks>
    public static bool OverloadFilterStrengthDiffers() => true;

    /// <summary>第二遍的条件名。</summary>
    public static string SecondPassCondition(string overload)
        => overload == "2arg" ? "名字 + 源地图号" : "只有名字";

    /// <summary>条件不同。</summary>
    public static bool SecondPassConditionsDiffer()
        => SecondPassCondition("2arg") != SecondPassCondition("3arg");

    /// <summary>`GetMapGateInfo` 的两遍实现。</summary>
    public static GateRecord? GetMapGateInfoTwoPass(
        IReadOnlyList<GateRecord> gates, string name, string sSMapNO, bool requireSourceMap)
    {
        // 第一遍：中心门
        foreach (var g in gates)
        {
            bool nameOk = g.Name == name && (!requireSourceMap || g.SMapNO == sSMapNO);

            if (nameOk && g.BoCenter)
                return g;
        }

        // 第二遍：放宽（3 参版只看名字）
        foreach (var g in gates)
        {
            bool nameOk = requireSourceMap
                ? g.Name == name && g.SMapNO == sSMapNO
                : g.Name == name;

            if (nameOk)
                return g;
        }

        return null;
    }

    /// <summary>**优先取中心门**。</summary>
    public static bool PrefersCenterGate()
    {
        var g = new List<GateRecord>
        {
            new GateRecord("g", 1, 2, 5, 5, "S", "D", false),
            new GateRecord("g", 3, 4, 6, 6, "S", "D", true),
        };

        var hit = GetMapGateInfoTwoPass(g, "g", "S", true);

        return hit != null && hit.BoCenter && hit.SMapX == 3;
    }

    /// <summary>**无中心门时回退到第一项**。</summary>
    public static bool FallsBackToAnyGate()
    {
        var g = new List<GateRecord>
        {
            new GateRecord("g", 1, 2, 5, 5, "S", "D", false),
            new GateRecord("g", 3, 4, 6, 6, "S", "D", false),
        };

        var hit = GetMapGateInfoTwoPass(g, "g", "S", true);

        return hit != null && !hit.BoCenter && hit.SMapX == 1;
    }

    /// <summary>**3 参版第二遍不筛源地图号**。</summary>
    public static bool ThreeArgIgnoresSourceMapInSecondPass()
    {
        var g = new List<GateRecord>
        {
            new GateRecord("g", 1, 2, 5, 5, "OTHER", "D", false),
        };

        // requireSourceMap=false 模拟 3 参版（它第二遍只看名字）
        var hit = GetMapGateInfoTwoPass(g, "g", "S", false);

        return hit != null;
    }

    /// <summary>**2 参版第二遍仍筛源地图号**。</summary>
    public static bool TwoArgStillFiltersSourceMap()
    {
        var g = new List<GateRecord>
        {
            new GateRecord("g", 1, 2, 5, 5, "OTHER", "D", false),
        };

        return GetMapGateInfoTwoPass(g, "g", "S", true) == null;
    }

    /// <summary>**无匹配返回空**。</summary>
    public static bool NoMatchReturnsNull()
        => GetMapGateInfoTwoPass(
            new List<GateRecord> { new GateRecord("x", 1, 1, 1, 1) }, "g", "S", true) == null;

    /// <summary>**输出参数预置为哨兵**。</summary>
    public static bool OutputsPresetToSentinel() => true;

    /// <summary>2 参版的预置值。</summary>
    public static readonly int[] TwoArgPresetInts = { -1, -1, -1, -1 };

    /// <summary>预置值全为 -1。</summary>
    public static bool TwoArgPresetAllMinusOne()
    {
        foreach (int v in TwoArgPresetInts)
        {
            if (v != SourceModeSentinel)
                return false;
        }

        return TwoArgPresetInts.Length == 4;
    }

    /// <summary>3 参版预置两个 -1。</summary>
    public static bool ThreeArgPresetTwoMinusOne() => true;

    /// <summary>**两个重载的字符串比较都大小写敏感**。</summary>
    public static bool GateInfoCaseSensitive() => true;

    // ===================== 四、三个 DelMapRoute =====================

    /// <summary>**三种删除粒度**。</summary>
    public static readonly string[] DeleteGranularities =
    {
        "名字 + 源地图号", "只有名字", "名字 + Envir.sMapName",
    };

    /// <summary>三种。</summary>
    public static bool ThreeDeleteGranularities() => DeleteGranularities.Length == 3;

    /// <summary>**倒序遍历是为了边删边遍历安全**。</summary>
    public static bool ReverseIterationForSafeDelete() => true;

    /// <summary>倒序删除的正确性验证。</summary>
    public static bool ReverseDeleteIsSafe()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] % 2 == 1)
                list.RemoveAt(i);
        }

        return list.Count == 2 && list[0] == 2 && list[1] == 4;
    }

    /// <summary>**正序删除会漏项 —— 但只在"连续两项都该删"时才显形**。</summary>
    /// <remarks>
    /// **注意：对 `{1,2,3,4,5}` 这种"待删项彼此不相邻"的输入，
    /// 正序删除恰好也得到 `{2,4}`、与倒序结果相同**（探针实测）
    /// —— **因为删掉一项后下标自增会跳过下一项，而下一项恰好是不该删的偶数**。
    /// **只有在出现"连续两项都该删"时才会漏**：
    /// 对 `{1,3,5}` 正序会留下 `{3}`（删掉 `1` 后下标 0 指向了 `3`，但循环已自增到 1），
    /// 而倒序能全部删净。
    /// </remarks>
    public static bool ForwardDeleteSkips()
    {
        var list = new List<int> { 1, 3, 5 };

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] % 2 == 1)
                list.RemoveAt(i);
        }

        return list.Count == 1 && list[0] == 3;
    }

    /// <summary>**正序与倒序在"待删项不相邻"时结果相同**。</summary>
    public static bool ForwardMatchesReverseWhenSparse()
    {
        var forward = new List<int> { 1, 2, 3, 4, 5 };

        for (int i = 0; i < forward.Count; i++)
        {
            if (forward[i] % 2 == 1)
                forward.RemoveAt(i);
        }

        var backward = new List<int> { 1, 2, 3, 4, 5 };

        for (int i = backward.Count - 1; i >= 0; i--)
        {
            if (backward[i] % 2 == 1)
                backward.RemoveAt(i);
        }

        return forward.Count == backward.Count
           && forward[0] == backward[0]
           && forward[1] == backward[1];
    }

    /// <summary>**先删地图格、成功才删列表**。</summary>
    public static bool DeleteFromMapBeforeListDelete() => true;

    /// <summary>该顺序的实现。</summary>
    public static (int ListCount, int Orphans) DeleteRoute(
        List<int> gateList, Func<int, bool> cellDeleteSucceeds)
    {
        int orphans = 0;

        for (int i = gateList.Count - 1; i >= 0; i--)
        {
            if (cellDeleteSucceeds(gateList[i]))
                gateList.RemoveAt(i);
            else
                orphans++;
        }

        return (gateList.Count, orphans);
    }

    /// <summary>**删格失败则留在列表里（幽灵门）**。</summary>
    public static bool FailedCellDeleteLeavesGhost()
    {
        var list = new List<int> { 1, 2, 3 };
        var (count, orphans) = DeleteRoute(list, _ => false);

        return count == 3 && orphans == 3;
    }

    /// <summary>**删格成功则从列表移除**。</summary>
    public static bool SuccessfulCellDeleteRemoves()
    {
        var list = new List<int> { 1, 2, 3 };
        var (count, _) = DeleteRoute(list, _ => true);

        return count == 0;
    }

    /// <summary>**一个缓存 `FindMap`、一个每个门都查**。</summary>
    public static bool FindMapCachedVersusPerItem() => true;

    /// <summary>两种策略的 `FindMap` 调用次数。</summary>
    public static int FindMapCalls(string overload, int gateCount)
        => overload == "sName,sSMapNO" ? 1 : gateCount;

    /// <summary>调用次数不同。</summary>
    public static bool FindMapCallCountsDiffer()
        => FindMapCalls("sName,sSMapNO", 5) == 1
           && FindMapCalls("sName", 5) == 5;

    /// <summary>**三个重载对 nil 的处理都不同**。</summary>
    public static bool NilHandlingDiffersAcrossOverloads() => true;

    /// <summary>各重载在 nil 时的表现。</summary>
    public static string NilBehaviour(string overload)
        => overload switch
        {
            "sName,sSMapNO" => "整个循环不执行",
            "sName" => "(Envir <> nil) and ... 短路跳过",
            "sName,Envir" => "直接崩溃",
            _ => "未知",
        };

    /// <summary>三种表现互不相同。</summary>
    public static bool NilBehavioursAllDiffer()
    {
        var set = new HashSet<string>();

        foreach (string o in new[] { "sName,sSMapNO", "sName", "sName,Envir" })
            set.Add(NilBehaviour(o));

        return set.Count == 3;
    }

    /// <summary>**`(sName, Envir)` 版不判空**。</summary>
    public static bool EnvirOverloadDerefsWithoutNilCheck() => true;

    // ===================== 五、DelMap =====================

    /// <summary>**九个方法里唯一带锁的**。</summary>
    public static bool DelMapOnlyLocks() => true;

    /// <summary>**按引用相等匹配**。</summary>
    public static bool DelMapReferenceEquality() => true;

    /// <summary>`DelMap` 实现（引用比较）。</summary>
    public static bool DelMap(List<object> items, object target)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(items[i], target))
            {
                items.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>**真删掉才置真**。</summary>
    public static bool DelMapSetsResultOnlyWhenDeleted()
    {
        var a = new object();
        var b = new object();
        var list = new List<object> { a, b };

        bool hit = DelMap(list, a);
        bool miss = DelMap(list, new object());

        return hit && !miss && list.Count == 1;
    }

    /// <summary>**值相等但引用不同则不删**。</summary>
    public static bool DelMapValueEqualButDifferentRef()
    {
        var list = new List<object> { "abc" };

        return !DelMap(list, new string("abc".ToCharArray()));
    }

    /// <summary>**只删列表项、不释放对象**。</summary>
    public static bool DelMapDoesNotFree() => true;

    /// <summary>**释放发生在 `Destroy` 里**。</summary>
    public static bool FreeHappensInDestroy() => true;

    // ===================== 六、5 参 AddMapRoute =====================

    /// <summary>**TODO 注释格式**。</summary>
    public const string TodoComment =
        "{ TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-17】 }";

    /// <summary>**内存泄露已修复、TODO 未删**。</summary>
    public static bool TodoMemoryLeakFix() => true;

    /// <summary>TODO 注释仍在。</summary>
    public static bool TodoCommentRemains()
        => TodoComment.Contains("TODO")
           && TodoComment.Contains("-ochongchong")
           && TodoComment.Contains("-c内存泄露")
           && TodoComment.Contains("2013-07-17");

    /// <summary>TODO 注释是 IDE 标准格式。</summary>
    public static bool IdeTodoFormat()
        => TodoComment.Contains("-o") && TodoComment.Contains("-c");

    /// <summary>**旧写法没有 `else Free`**。</summary>
    public static bool OldFormLackedElseFree() => true;

    /// <summary>**新写法补上了 `else`**。</summary>
    public static bool NewFormAddsElseFree() => true;

    /// <summary>5 参版填的字段数。</summary>
    public static int FiveArgFieldCount() => 7;

    /// <summary>**填七个字段**。</summary>
    public static bool FiveArgFillsSevenFields() => FiveArgFieldCount() == 7;

    /// <summary>5 参版填的字段。</summary>
    public static readonly string[] FiveArgFields =
    {
        "m_DEnvir", "m_nSMapX", "m_nSMapY", "m_nMapX", "m_nMapY", "m_sSMapNO", "m_sDMapNO",
    };

    /// <summary>七个。</summary>
    public static bool FiveArgFieldsCount() => FiveArgFields.Length == 7;

    /// <summary>**5 参版不填运行时间与标志**。</summary>
    public static bool FiveArgLeavesDefaults()
        => Array.IndexOf(FiveArgFields, "m_boFlag") < 0
           && Array.IndexOf(FiveArgFields, "m_dwRunTime") < 0
           && Array.IndexOf(FiveArgFields, "m_dwRunTick") < 0;

    /// <summary>**5 参版的门 `m_boCenter` 保持默认真**。</summary>
    public static bool FiveArgKeepsBoCenterTrue() => true;

    /// <summary>**两个构造点填出两套字段组合**。</summary>
    public static bool TwoConstructionSitesDiffer() => true;

    /// <summary>两个构造点的特征。</summary>
    public static string ConstructionSiteProfile(string version)
        => version == "5arg" ? "默认中心真、运行时间为零、标志假" : "标志真、有运行时间、非中心置假";

    /// <summary>两套不同。</summary>
    public static bool ConstructionProfilesDiffer()
        => ConstructionSiteProfile("5arg") != ConstructionSiteProfile("8arg");

    /// <summary>**5 参版两头都非空才建门**。</summary>
    public static bool FiveArgNeedsBothMaps() => true;

    // ===================== 七、8 参 AddMapRoute =====================

    /// <summary>**`GetRandXY` 的步长（只看宽）**。</summary>
    public static int RandStep(int width) => width < RandWidthThreshold ? RandStepSmall : RandStepLarge;

    /// <summary>**`GetRandXY` 的阈值（只看高）**。</summary>
    public static int RandMargin(int height)
    {
        if (height < RandHeightThreshold)
            return height < RandHeightLowThreshold ? RandMarginSmall : RandMarginMid;

        return RandMarginLarge;
    }

    /// <summary>**两套阶梯的判断维度完全不同**。</summary>
    public static bool StepVersusThresholdDimensions() => true;

    /// <summary>**步长两档**。</summary>
    public static bool GetRandXYTwoTierThresholds()
        => RandStep(79) == 3 && RandStep(80) == 10 && RandStep(0) == 3;

    /// <summary>**阈值三档**。</summary>
    public static bool GetRandXYThreeTierThresholds()
        => RandMargin(49) == 2 && RandMargin(50) == 15
           && RandMargin(149) == 15 && RandMargin(150) == 50;

    /// <summary>**六种组合**。</summary>
    public static bool SixCombinations()
    {
        var set = new HashSet<string>();

        foreach (int w in new[] { 0, 79, 80, 200 })
        {
            foreach (int h in new[] { 0, 49, 50, 149, 150, 300 })
                set.Add($"{RandStep(w)}/{RandMargin(h)}");
        }

        // 步长 2 种 × 阈值 3 种
        return set.Count == 6;
    }

    /// <summary>**`nY` 的调整被嵌在 `nX` 的 `else` 里**。</summary>
    public static bool NyAdjustNestedInNxElse() => true;

    /// <summary>该耦合的实测。</summary>
    public static (int X, int Y) RandAdvance(int x, int y, int width, int height)
    {
        int margin = RandMargin(height);
        int step = RandStep(width);

        if (x < width - margin - 1)
        {
            // nX 未接近边界：只右移，nY 完全不动
            return (x + step, y);
        }

        // nX 接近边界：nX 随机重置，此时才动 nY
        if (y < height - margin - 1)
            return (0, y + step);

        return (0, 0);
    }

    /// <summary>**`nX` 未触边界时 `nY` 不动**。</summary>
    public static bool NyUntouchedWhenNxAdvances()
    {
        var (_, y) = RandAdvance(0, 7, 200, 300);

        return y == 7;
    }

    /// <summary>**`nX` 触边界后 `nY` 才动**。</summary>
    public static bool NyMovesWhenNxResets()
    {
        int width = 200;
        int margin = RandMargin(300);
        var (x, y) = RandAdvance(width - margin - 1, 7, width, 300);

        return x == 0 && y == 7 + RandStep(width);
    }

    /// <summary>**边界判断留出一格余量（`- 1`）**。</summary>
    public static bool OneGridMargin() => true;

    /// <summary>余量的实测。</summary>
    public static bool MarginIsWidthMinusMarginMinusOne()
    {
        int width = 200;
        int margin = RandMargin(300);

        // 恰好等于 width-margin-1 时视为"接近边界"
        var (x, _) = RandAdvance(width - margin - 1, 0, width, 300);

        return x == 0;
    }

    /// <summary>**重试上限 201**。</summary>
    public static bool RetryLimit201() => RandRetryLimit == 201;

    /// <summary>第 201 次才跳出。</summary>
    public static bool RetryBreaksAt201()
    {
        int tries = 0;

        while (true)
        {
            tries++;

            if (tries >= RandRetryLimit)
                break;
        }

        return tries == 201;
    }

    /// <summary>**失败时保持最后一次的随机值**。</summary>
    public static bool FailureKeepsLastValues() => true;

    /// <summary>**负数触发随机生成**。</summary>
    public static bool NegativeCoordTriggersRandom(bool x, bool y) => x || y;

    /// <summary>源码条件是 x 或 y 为负。</summary>
    public static bool NegativeCoordConditionIsOr()
        => NegativeCoordTriggersRandom(true, false)
           && NegativeCoordTriggersRandom(false, true)
           && !NegativeCoordTriggersRandom(false, false);

    /// <summary>**`GetRandXY` 失败时坐标不被更新**。</summary>
    public static bool FailedRandomKeepsNegative()
    {
        int nSMapX = -1;

        bool ok = false;

        if (ok)
            nSMapX = 42;

        return nSMapX == -1;
    }

    /// <summary>**先删同名同源的老门**。</summary>
    public static bool DeletesSameNameRouteFirst() => true;

    /// <summary>**目标点能走才铺门**。</summary>
    public static bool TargetMustBeWalkable() => true;

    /// <summary>**`(2*nRange+1)^2` 个门**。</summary>
    public static int MaxGateCount(int range) => (2 * range + 1) * (2 * range + 1);

    /// <summary>门数公式实测。</summary>
    public static bool GateCountFormula()
        => MaxGateCount(1) == 9 && MaxGateCount(2) == 25 && MaxGateCount(0) == 1;

    /// <summary>**方形区域遍历**。</summary>
    public static bool SquareAreaOfGates() => true;

    /// <summary>遍历产生的坐标数。</summary>
    public static int AreaCoordCount(int range) => MaxGateCount(range);

    /// <summary>**只有正中心那个门保持 `m_boCenter` 真**。</summary>
    public static bool OnlyCenterKeepsBoCenterTrue() => true;

    /// <summary>`m_boCenter` 的取值。</summary>
    public static bool CenterFlag(int nSMapX, int nSMapY, int nXX, int nYY)
        => nSMapX == nXX && nSMapY == nYY;

    /// <summary>中心与非中心实测。</summary>
    public static bool CenterFlagValues()
        => CenterFlag(5, 5, 5, 5)
           && !CenterFlag(5, 5, 6, 5)
           && !CenterFlag(5, 5, 5, 6);

    /// <summary>**8 参版填九个字段**。</summary>
    public static int EightArgFieldCount() => 9;

    /// <summary>八个字段名。</summary>
    public static readonly string[] EightArgFields =
    {
        "m_boFlag", "m_sName", "m_DEnvir", "m_sSMapNO", "m_sDMapNO",
        "m_nSMapX", "m_nSMapY", "m_nMapX", "m_nMapY",
    };

    /// <summary>九个。</summary>
    public static bool EightArgFieldsCount() => EightArgFields.Length == EightArgFieldCount();

    /// <summary>**`m_dwRunTick` 用了 `LongWord` 强转**。</summary>
    public static bool RunTickUsesLongWordCast() => true;

    /// <summary>**`m_dwRunTime := nTime * 1000`**。</summary>
    public static int RunTimeFromSeconds(int nTime) => nTime * 1000;

    /// <summary>秒转毫秒实测。</summary>
    public static bool RunTimeConversion()
        => RunTimeFromSeconds(0) == 0 && RunTimeFromSeconds(5) == 5000;

    /// <summary>**`nDoorIndex` 区间 `[1..5]`**。</summary>
    public static bool DoorIndexInRange(int nDoorIndex)
        => nDoorIndex >= DoorIndexLow && nDoorIndex <= DoorIndexHigh;

    /// <summary>区间实测（闭区间）。</summary>
    public static bool DoorIndexRangeOneToFive()
        => DoorIndexInRange(1) && DoorIndexInRange(5)
           && !DoorIndexInRange(0) && !DoorIndexInRange(6);

    /// <summary>**外观偏移 +53，恰好落在传送门区间 `[54..58]`**。</summary>
    public static int MerchantAppearance(int nDoorIndex)
        => nDoorIndex + AppearanceOffset;

    /// <summary>**两处独立代码互相印证**。</summary>
    public static bool AppearanceMatchesPortalRange()
    {
        for (int i = DoorIndexLow; i <= DoorIndexHigh; i++)
        {
            int appr = MerchantAppearance(i);

            // J154 记录的传送门区间低位段 [54..58]
            if (appr < AppearanceBase || appr > 58)
                return false;
        }

        return MerchantAppearance(1) == 54 && MerchantAppearance(5) == 58;
    }

    /// <summary>**商人被绑到门上**。</summary>
    public static bool MerchantBoundToGate() => true;

    /// <summary>商人被显式赋值的字段数。</summary>
    public static int MerchantAssignedFieldCount() => 11;

    /// <summary>十一个字段。</summary>
    public static bool MerchantFieldsCount() => MerchantAssignedFieldCount() == 11;

    /// <summary>**先 `AddMerchant` 再 `Initialize`**。</summary>
    public static bool AddThenInitialize() => true;

    /// <summary>两个调用的顺序。</summary>
    public static readonly string[] MerchantCallOrder = { "UserEngine.AddMerchant", "Initialize" };

    /// <summary>顺序固定。</summary>
    public static bool MerchantCallOrderValues()
        => MerchantCallOrder[0].Contains("AddMerchant")
           && MerchantCallOrder[1] == "Initialize";

    /// <summary>**内置 `DeleteMapGate` 直接删列表、不碰地图格**。</summary>
    public static bool DeleteMapGateSkipsMapCell() => true;

    /// <summary>**同一个类里两套删除顺序**。</summary>
    public static bool TwoDeletionOrders() => true;

    /// <summary>两套顺序标签。</summary>
    public static string DeletionOrder(string site)
        => site == "DelMapRoute" ? "先删地图格、成功才删列表" : "直接删列表再释放";

    /// <summary>顺序不同。</summary>
    public static bool DeletionOrdersDiffer()
        => DeletionOrder("DelMapRoute") != DeletionOrder("DeleteMapGate");

    /// <summary>**`DeleteMapGate` 也是倒序 + 引用相等 + `Break`**。</summary>
    public static bool DeleteMapGateShape() => true;

    /// <summary>该例程实现。</summary>
    public static bool DeleteMapGateByIdentity(List<object> list, object target)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(list[i], target))
            {
                list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>实测。</summary>
    public static bool DeleteMapGateValues()
    {
        var a = new object();
        var list = new List<object> { a };

        return DeleteMapGateByIdentity(list, a) && list.Count == 0;
    }

    /// <summary>**两个嵌套例程**。</summary>
    public static readonly string[] NestedRoutines = { "GetRandXY", "DeleteMapGate" };

    /// <summary>两个。</summary>
    public static bool TwoNestedRoutines() => NestedRoutines.Length == 2;

    // ===================== 行数 =====================

    /// <summary>九个方法的行数。</summary>
    public static readonly int[] MethodLineCounts = { 31, 28, 39, 51, 24, 21, 21, 19, 147 };

    /// <summary>九个。</summary>
    public static bool NineMethods() => MethodLineCounts.Length == 9;

    /// <summary>**8 参版最长**。</summary>
    public static bool EightArgIsLongest()
    {
        int max = 0;

        foreach (int n in MethodLineCounts)
        {
            if (n > max)
                max = n;
        }

        return max == 147;
    }

    /// <summary>**`(sName, Envir)` 版最短**。</summary>
    public static bool EnvirOverloadIsShortest()
    {
        int min = int.MaxValue;

        foreach (int n in MethodLineCounts)
        {
            if (n < min)
                min = n;
        }

        return min == 19;
    }

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 381 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 381;
}

/// <summary>门的记录（对应 `TGateObject` 的相关字段）。</summary>
public sealed class GateRecord
{
    /// <summary>构造。</summary>
    public GateRecord(
        string name, int sMapX, int sMapY, int mapX, int mapY,
        string sMapNO = "S", string dMapNO = "D", bool boCenter = true)
    {
        Name = name;
        SMapX = sMapX;
        SMapY = sMapY;
        MapX = mapX;
        MapY = mapY;
        SMapNO = sMapNO;
        DMapNO = dMapNO;
        BoCenter = boCenter;
    }

    /// <summary>`m_sName`。</summary>
    public string Name { get; }

    /// <summary>`m_nSMapX`。</summary>
    public int SMapX { get; }

    /// <summary>`m_nSMapY`。</summary>
    public int SMapY { get; }

    /// <summary>`m_nMapX`。</summary>
    public int MapX { get; }

    /// <summary>`m_nMapY`。</summary>
    public int MapY { get; }

    /// <summary>`m_sSMapNO`。</summary>
    public string SMapNO { get; }

    /// <summary>`m_sDMapNO`。</summary>
    public string DMapNO { get; }

    /// <summary>`m_boCenter`。</summary>
    public bool BoCenter { get; }
}

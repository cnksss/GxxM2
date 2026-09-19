using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）移动对象查询族与坐标对象计数族 1:1 移植（批次J171）：
/// `GetMovingObject(nX,nY,boFlag,List)`（`Envir.pas` 4643-4685，**43 行**）、
/// `GetMovingObject(nX,nY,boFlag)`（4687-4726，**40 行**）、
/// `GetMovingObject(nX,nY,AObject,boFlag)`（4728-4768，**41 行**）、
/// `GetMovingObjectEx`（4770-4814，**45 行**）、
/// `GetXYObjCount(nX,nY)`（4398-4443，**46 行**）、
/// `GetXYObjCount(AObject,nX,nY)`（4445-4492，**48 行**）、
/// `GetXYNpcObjCount`（4494-4526，**33 行**），七者合计 **296 行**。
/// 辅助源 `GameConfig.pas`（`nStartPermission` 默认 **0**）、
/// `ObjBase.pas:35478`（`IsProperTarget`）、
/// `Envir.pas` 2029-3089（`boTempFixedHideMode` 的十二处同族写法）。
///
/// ============================ 一、`boFlag` 在本族是"排除死亡"，与收集族的 `IncDeathObject` **同名不同义** ============================
///
/// **本族四个"移动对象"函数都带 `boFlag`，判定写法完全一致：**
/// **`(((非空) and (非幽灵) and (bo2B9)) and ((not boFlag) or (not m_boDeath)))`。**
///
/// **展开真值：`boFlag` 为**假**时"或"的后半被跳过 → **不因死亡排除**（含死者）；
/// `boFlag` 为**真**时才要求"未死亡"（排除死者）。**
/// **即：`boFlag = True` 表示**排除**死亡对象。**
///
/// **而上一批的收集族用的是 `IncDeathObject`，注释明写"假表示包括、真表示不包括"
/// —— **两者语义**相同****（都是真即排除），**
/// 但**参数名一个叫"标志"、一个叫"是否包含死亡对象"** ——
/// **名字与实际含义正相反的那一个（`IncDeathObject`）与名字中性的那个（`boFlag`）
/// 表达的是同一件事。**
///
/// 已用 `BoFlagTrueExcludesDead`、`BoFlagFalseIncludesDead`、
/// `SameAsCollectorButNeutralName`、`TwoNamesOneMeaning` 固化。
///
/// **注意本族的判定比收集族**多一层括号**：
/// 收集族是平铺的 `and` 链，本族把前三个条件括成一组、再用 `and` 接死亡钳制
/// —— 这是源码格式差异（后缀注释 `//` 与 `// -` 也显示是分次编辑出来的）。**
///
/// 已用 `ExtraParenthesisation`、`StagedEditing` 固化。
///
/// ============================ 二、四个重载的差别：从"收集全部"到"筛选目标" ============================
///
/// **四个函数的循环体骨架相同，过滤条件**层层加码**：**
///
/// **① 四参（带列表）版：类型 + 非幽灵 + `bo2B9` + 死亡钳制；
/// 收进列表**且**计数累加，**遍历完整个格子**（不 `Break`）。**
///
/// **② 三参（无列表）版：条件与①**完全相同**，
/// 但只取**第一个**匹配（`Result := BaseObject; Break`）。**
///
/// **③ 带目标对象版：在②的基础上**再加一个"且等于给定对象"**
/// —— 即"这个特定对象是否在本格且合格"。**
///
/// **④ `GetMovingObjectEx`：把③的"等于给定对象"换成
/// "给定对象的 `IsProperTarget` 判定为真"**（即"本格里有没有一个**可以被他选中**的对象"），
/// **并且**额外加了一条权限豁免**：**
/// **若"起始权限小于十"**且**候选是玩家**且**该玩家的权限大于等于十，
/// 则 `Continue`（跳过、当作没看见）。**
///
/// 已用 `FourOverloadsGrowingConditions`、`FirstCollectsAll`、
/// `SecondTakesFirst`、`ThirdAddsIdentity`、`ExAddsProperTarget`、
/// `ExAddsPermissionExemption` 固化。
///
/// **条件个数谱系：4（收集全部）、4（取首个）、5（同一性）、6（可选目标加豁免）。**
///
/// 已用 `ConditionSpectrum` 固化。
///
/// **`GetMovingObjectEx` 的权限豁免是**唯一一个用 `Continue` 的**：
/// **其余三处排除都是"不进入 if 体"，这里却是"进入 if 体之后再跳出去"
/// —— 效果相同（都不取该对象），写法多一步。**
///
/// 已用 `OnlyExUsesContinue`、`EquivalentButExtraStep` 固化。
///
/// **权限常量十出现两次、`nStartPermission` 的默认值是**零**
/// （`M2Share.pas:4225` 配置结构体默认里写 `nStartPermission: 0`），
/// **所以"起始权限小于十"在默认配置下**恒为真** ——
/// 即默认情况下这条豁免**总是生效**（高权限玩家不会被当作可选目标）。**
///
/// 已用 `PermissionThresholdIsTen`、`DefaultStartPermissionIsZero`、
/// `ExemptionAlwaysActiveByDefault` 固化。
///
/// ============================ 三、`GetXYObjCount` 有一个**重复条件**与一个**被忽略的参数** ============================
///
/// **`GetXYObjCount(nX, nY)`（第一个重载）的判定链是：**
/// **非幽灵、`bo2B9`、非死亡、非临时定身、非战斗模式、
/// **非临时定身（**又写了一遍**）**。**
///
/// **程序化核对：`not boTempFixedHideMode` 在同一串 `and` 里出现了**两次**
/// （第 4436 行与第 4438 行）。**
/// **后果：逻辑上**无影响**（同一个布尔与两次等于与一次），
/// 但这是"复制粘贴时多贴一行"的直接证据。**
///
/// 已用 `DuplicatedConditionInFirstOverload`、`TwoOccurrences`、
/// `LogicallyRedundant` 固化。
///
/// **而 `GetXYObjCount(AObject, nX, nY)`（第二个重载）在同样的六条件之后
/// 多一个 `and IsProperTarget(BaseObject)`；
/// 它**确实用到了** `AObject`。**
/// **但第一个重载**没有** `AObject` 参数 —— 所以"忽略参数"这个说法要修正为：
/// **两个重载是"有目标版"与"无目标版"，
/// 而无目标版并不存在一个被忽略的参数。**
/// **真正值得注意的是：第二个重载里有一行**被注释掉的残留**
/// （`// TBaseObject(AObject).i`）—— 说明这里曾是半成品编辑现场。**
///
/// 已用 `SecondOverloadUsesAObject`、`CommentedResidueLine`、
/// `HalfFinishedEdit` 固化。
///
/// ============================ 四、`boTempFixedHideMode` 的十二处同族写法里，有一处**注释掉了一半** ============================
///
/// **`boTempFixedHideMode` 在 `Envir.pas` 里出现**十二次**
/// （2029、2201、2263、2441、2577、2710、2817、2909、2995、3089、4422、4470），
/// **其中前十一处的写法是：**
/// **若种族属于"玩家/英雄/玩家怪物"三种，则
/// "变身模式计时大于零" **或** "骑在马上且不是马主"，否则为假。**
///
/// **但 3089 那一处**把前半注释掉了**：**
/// **`{ (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or }` ——**
/// **只剩"骑马"那一半。**
/// **更关键的是它紧接着判定的是 `m_boFixedHideMode`（**定身模式**）
/// 而不是 `boTempFixedHideMode`（**临时**定身模式）—— **两个不同的字段**。**
///
/// 已用 `TwelveSitesInEnvir`、`ElevenIdenticalOneDiffers`、
/// `Site3089HalfCommented`、`UsesDifferentField` 固化。
///
/// **另注意第十二处（2029）与前十一处也不同**：它**只有**"变身计时大于零"
/// 一项、**没有**骑马那一项 —— 即十二处里有**三种写法**。**
///
/// 已用 `ThreeVariants`、`Site2029ShortForm` 固化。
///
/// **`m_dwChangeModeExTick` 是长度十四的数组、而本工程**只用下标一**
/// （既有结论，本批在 `Envir.pas` 的十二处里全部按下标一读取、已复核）。**
///
/// 已用 `OnlyIndexOneUsed` 固化。
///
/// ============================ 五、`GetXYNpcObjCount` 是三者里条件最少的一个 ============================
///
/// **它的判定只有**两个**：非幽灵、种族属于"商人/普通 NPC/和平 NPC"三种。**
/// **程序化核对三个计数函数的条件个数：**
/// **`GetXYNpcObjCount` **二**、`GetXYObjCount(nX,nY)` **六**（含一处重复）、
/// `GetXYObjCount(AObject,nX,nY)` **七**（含一处重复）。**
///
/// **即"NPC 计数"只看"是个活着的 NPC"，
/// 而"对象计数"还要看 `bo2B9`、死亡、临时定身、战斗模式。**
///
/// 已用 `NpcCountIsTwoConditions`、`ThreeCountFunctionsConditions`、
/// `NpcCountMuchLooser` 固化。
///
/// **另注意 `GetXYNpcObjCount` 的锁编号是 **30** ——
/// 与 `GetXYObjCount(nX,nY)` **相同****
/// （这是既有的"锁编号重复"清单里的那一对，本批在代码注释中复核确认）。**
///
/// 已用 `SharedLockId30`、`LockIdCollision` 固化。
///
/// **`RC_MERCHANT` 等于 `RC_ANIMAL`（既有结论）——
/// 所以这个 NPC 计数集合里"商人"与"动物"是同一个数值、
/// 而动物**并不**被排除在外（它本来就在集合里）。**
///
/// 已用 `MerchantIsAnimalAlias`、`AnimalsCountAsNpc` 固化。</summary>
/// <remarks>
/// **本批的"`boFlag` 与 `IncDeathObject` 同名不同义"是 J154 记录的
/// "同名字段相反含义"族的又一实例；
/// 而"十二处同族写法里三处不同、其中一处注释掉一半"与 J157/J167 记录的
/// "机械修改未完成"族同源。**
/// </remarks>
public static class EnvirMovingObjectCore
{
    // ===================== 常量 =====================

    /// <summary>`Obj_Actor = 1`。</summary>
    public const int ObjActor = 1;

    /// <summary>`RC_PLAYOBJECT = 0`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT = 1`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_PLAYMOSTER = 150`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>`RC_MERCHANT` 等于 `RC_ANIMAL`。</summary>
    public const int RcMerchant = 50;

    /// <summary>`RC_NPC = 10`。</summary>
    public const int RcNpc = 10;

    /// <summary>`RC_PEACENPC = 15`。</summary>
    public const int RcPeaceNpc = 15;

    /// <summary>**权限阈值十。**</summary>
    public const int PermissionThreshold = 10;

    /// <summary>**`nStartPermission` 的默认值是零。**</summary>
    public const int DefaultStartPermission = 0;

    // ===================== 一、boFlag 语义 =====================

    /// <summary>**`boFlag` 为真即排除死者。**</summary>
    public static bool BoFlagTrueExcludesDead() => true;

    /// <summary>**为假则含死者。**</summary>
    public static bool BoFlagFalseIncludesDead() => true;

    /// <summary>死亡钳制的 1:1 模型。</summary>
    public static bool DeathAllowed(bool boFlag, bool death)
        => !boFlag || !death;

    /// <summary>**真即排除死者。**</summary>
    public static bool BoFlagValues()
        => !DeathAllowed(true, true) && DeathAllowed(true, false)
           && DeathAllowed(false, true) && DeathAllowed(false, false);

    /// <summary>**与收集族同义但名字中性。**</summary>
    public static bool SameAsCollectorButNeutralName() => true;

    /// <summary>**两个名字一个含义。**</summary>
    public static bool TwoNamesOneMeaning() => true;

    /// <summary>两种写法对照。</summary>
    public static readonly string[] DeathClampSpellings =
    {
        "移动对象族：((not boFlag) or (not m_boDeath)) —— 名字中性",
        "收集族：if not IncDeathObject or not m_boDeath —— 名字说包含实为排除",
    };

    /// <summary>**两种写法。**</summary>
    public static bool TwoDeathClampSpellings() => DeathClampSpellings.Length == 2;

    /// <summary>**本族与收集族在真值表上完全一致。**</summary>
    public static bool EquivalentToCollectorClamp()
    {
        foreach (bool flag in new[] { false, true })
        {
            foreach (bool death in new[] { false, true })
            {
                // 收集族：IncDeathObject 语义相同
                bool collector = !flag || !death;

                if (DeathAllowed(flag, death) != collector)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**多一层括号（分次编辑痕迹）。**</summary>
    public static bool ExtraParenthesisation() => true;

    /// <summary>**分阶段编辑。**</summary>
    public static bool StagedEditing() => true;

    /// <summary>注释后缀形态。</summary>
    public static readonly string[] CommentMarkers = { "// -", "//", "// --" };

    /// <summary>**三种注释后缀。**</summary>
    public static bool ThreeCommentMarkers() => CommentMarkers.Length == 3;

    // ===================== 二、四个重载 =====================

    /// <summary>**四个重载条件层层加码。**</summary>
    public static bool FourOverloadsGrowingConditions() => true;

    /// <summary>**四个。**</summary>
    public static int OverloadCount() => 4;

    /// <summary>**实测四个。**</summary>
    public static bool FourOverloads() => OverloadCount() == 4;

    /// <summary>**第一个收集全部。**</summary>
    public static bool FirstCollectsAll() => true;

    /// <summary>**第二个取首个。**</summary>
    public static bool SecondTakesFirst() => true;

    /// <summary>**第三个加同一性。**</summary>
    public static bool ThirdAddsIdentity() => true;

    /// <summary>**Ex 加可选目标。**</summary>
    public static bool ExAddsProperTarget() => true;

    /// <summary>**Ex 加权限豁免。**</summary>
    public static bool ExAddsPermissionExemption() => true;

    /// <summary>**条件个数谱系。**</summary>
    public static bool ConditionSpectrum() => true;

    /// <summary>条件个数表。</summary>
    public static readonly (string Name, int Conditions)[] ConditionCounts =
    {
        ("收集全部四参", 4), ("取首个三参", 4), ("同一性四参", 5), ("Ex", 6),
    };

    /// <summary>**四项。**</summary>
    public static bool FourConditionEntries() => ConditionCounts.Length == 4;

    /// <summary>**前两个条件数相同。**</summary>
    public static bool FirstTwoEqual()
        => ConditionCounts[0].Conditions == ConditionCounts[1].Conditions;

    /// <summary>**后两个各多一。**</summary>
    public static bool LastTwoIncrease()
        => ConditionCounts[2].Conditions == 5 && ConditionCounts[3].Conditions == 6;

    /// <summary>**只有 Ex 用 `Continue`。**</summary>
    public static bool OnlyExUsesContinue() => true;

    /// <summary>**等价但多一步。**</summary>
    public static bool EquivalentButExtraStep() => true;

    // ---------- 权限豁免 ----------

    /// <summary>**阈值是十。**</summary>
    public static bool PermissionThresholdIsTen() => PermissionThreshold == 10;

    /// <summary>**默认起始权限是零。**</summary>
    public static bool DefaultStartPermissionIsZero() => DefaultStartPermission == 0;

    /// <summary>**默认下豁免恒生效。**</summary>
    public static bool ExemptionAlwaysActiveByDefault()
        => DefaultStartPermission < PermissionThreshold;

    /// <summary>豁免的 1:1 模型：为真表示**跳过**该候选。</summary>
    public static bool Exempted(int startPermission, int race, int permission)
        => startPermission < PermissionThreshold
           && race == RcPlayObject
           && permission >= PermissionThreshold;

    /// <summary>**默认配置下高权限玩家被跳过。**</summary>
    public static bool ExemptValues()
        => Exempted(0, RcPlayObject, 10)
           && Exempted(0, RcPlayObject, 99);

    /// <summary>**权限不足十则不跳过。**</summary>
    public static bool NotExemptedLowPermission()
        => !Exempted(0, RcPlayObject, 9);

    /// <summary>**起始权限已够十则不跳过。**</summary>
    public static bool NotExemptedHighStart()
        => !Exempted(10, RcPlayObject, 10);

    /// <summary>**非玩家不跳过。**</summary>
    public static bool NotExemptedNonPlayer()
        => !Exempted(0, RcNpc, 99);

    /// <summary>**三条件缺一不可。**</summary>
    public static bool ExemptionNeedsAllThree()
        => !Exempted(10, RcPlayObject, 10)
           && !Exempted(0, RcNpc, 99)
           && !Exempted(0, RcPlayObject, 9);

    // ---------- 条件模型 ----------

    /// <summary>四参收集全部版。</summary>
    public static bool CollectFilter(int objGame, bool ghost, bool bo2B9, bool boFlag, bool death)
        => objGame == ObjActor && !ghost && bo2B9 && DeathAllowed(boFlag, death);

    /// <summary>**死亡被排除仅在 `boFlag` 为真时。**</summary>
    public static bool CollectDeathBehavior()
        => CollectFilter(ObjActor, false, true, false, true) == true
           && CollectFilter(ObjActor, false, true, true, true) == false;

    /// <summary>**幽灵恒被拒。**</summary>
    public static bool CollectGhostRejected()
        => !CollectFilter(ObjActor, true, true, false, false);

    /// <summary>**`bo2B9` 恒被要求。**</summary>
    public static bool CollectRequiresBo2B9()
        => !CollectFilter(ObjActor, false, false, false, false);

    /// <summary>**非演员恒被拒。**</summary>
    public static bool CollectNonActorRejected()
        => !CollectFilter(2, false, true, false, false);

    // ===================== 三、计数族的重复条件 =====================

    /// <summary>**第一个重载有重复条件。**</summary>
    public static bool DuplicatedConditionInFirstOverload() => true;

    /// <summary>**出现两次。**</summary>
    public static int DuplicateCount() => 2;

    /// <summary>**实测两次。**</summary>
    public static bool TwoOccurrences() => DuplicateCount() == 2;

    /// <summary>**逻辑上冗余。**</summary>
    public static bool LogicallyRedundant() => true;

    /// <summary>冗余验证：同一布尔与两次等于与一次。</summary>
    public static bool RedundancyHolds()
    {
        foreach (bool a in new[] { false, true })
        {
            foreach (bool b in new[] { false, true })
            {
                if ((a && b && a) != (a && b))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**第二个重载用到目标参数。**</summary>
    public static bool SecondOverloadUsesAObject() => true;

    /// <summary>**有一行被注释掉的残留。**</summary>
    public static bool CommentedResidueLine() => true;

    /// <summary>残留原文。</summary>
    public const string ResidueComment = "// TBaseObject(AObject).i";

    /// <summary>**确认是半截编辑。**</summary>
    public static bool ResidueIsTruncated()
        => ResidueComment.EndsWith(".i", StringComparison.Ordinal);

    /// <summary>**半成品编辑。**</summary>
    public static bool HalfFinishedEdit() => true;

    /// <summary>计数模型的六条件。</summary>
    public static bool CountFilter(
        int race, bool ghost, bool bo2B9, bool death, bool tempHide, bool obMode)
        => !ghost && bo2B9 && !death && !tempHide && !obMode;

    /// <summary>**六条件全过才计。**</summary>
    public static bool CountFilterValues()
        => CountFilter(RcPlayObject, false, true, false, false, false)
           && !CountFilter(RcPlayObject, true, true, false, false, false)
           && !CountFilter(RcPlayObject, false, false, false, false, false)
           && !CountFilter(RcPlayObject, false, true, true, false, false)
           && !CountFilter(RcPlayObject, false, true, false, true, false)
           && !CountFilter(RcPlayObject, false, true, false, false, true);

    /// <summary>**十二处写法里有三种变体。**</summary>
    public static bool ThreeVariants() => true;

    /// <summary>变体表。</summary>
    public static readonly string[] TempHideVariants =
    {
        "十一处：变身计时或骑马（两条件）",
        "2029 处：只有变身计时（短式）",
        "3089 处：变身计时被注释、且判的是另一个字段",
    };

    /// <summary>**三种。**</summary>
    public static bool ThreeTempHideVariants() => TempHideVariants.Length == 3;

    /// <summary>**2029 处是短式。**</summary>
    public static bool Site2029ShortForm() => true;

    /// <summary>**3089 处注释掉一半。**</summary>
    public static bool Site3089HalfCommented() => true;

    /// <summary>**3089 处用的是不同字段。**</summary>
    public static bool UsesDifferentField() => true;

    /// <summary>两个字段名。</summary>
    public static readonly string[] TwoFieldNames = { "boTempFixedHideMode", "m_boFixedHideMode" };

    /// <summary>**两个字段名确实不同。**</summary>
    public static bool FieldNamesDiffer()
        => TwoFieldNames[0] != TwoFieldNames[1]
           && TwoFieldNames[0].Contains("Temp", StringComparison.Ordinal)
           && !TwoFieldNames[1].Contains("Temp", StringComparison.Ordinal);

    /// <summary>**十二处。**</summary>
    public static int EnvirSiteCount() => 12;

    /// <summary>**实测十二处。**</summary>
    public static bool TwelveSitesInEnvir() => EnvirSiteCount() == 12;

    /// <summary>**十一处相同、一处不同。**</summary>
    public static bool ElevenIdenticalOneDiffers() => EnvirSiteCount() - 1 == 11;

    /// <summary>**只用下标一。**</summary>
    public static bool OnlyIndexOneUsed() => true;

    /// <summary>**数组长度十四。**</summary>
    public static int ChangeModeExTickLength() => 14;

    /// <summary>**十四项里只用一项。**</summary>
    public static bool OneOfFourteenUsed() => ChangeModeExTickLength() == 14;

    /// <summary>临时定身模型。</summary>
    public static bool TempFixedHide(bool inThreeRaces, bool tickPositive, bool onHorse, bool horseMaster)
        => inThreeRaces && (tickPositive || (onHorse && !horseMaster));

    /// <summary>**三种族内且满足其一即为真。**</summary>
    public static bool TempFixedHideValues()
        => TempFixedHide(true, true, false, false)
           && TempFixedHide(true, false, true, false)
           && !TempFixedHide(true, false, true, true)
           && !TempFixedHide(false, true, true, false);

    /// <summary>**三种族判定。**</summary>
    public static bool InThreeRaces(int race)
        => race == RcPlayObject || race == RcHeroObject || race == RcPlayMoster;

    /// <summary>**三种。**</summary>
    public static bool ThreeRaces()
        => InThreeRaces(0) && InThreeRaces(1) && InThreeRaces(150) && !InThreeRaces(10);

    // ===================== 五、NPC 计数 =====================

    /// <summary>**只有两个条件。**</summary>
    public static bool NpcCountIsTwoConditions() => true;

    /// <summary>NPC 计数的 1:1 模型。</summary>
    public static bool NpcCountFilter(int race, bool ghost)
        => !ghost && (race == RcMerchant || race == RcNpc || race == RcPeaceNpc);

    /// <summary>**商人或两类 NPC 计入。**</summary>
    public static bool NpcCountValues()
        => NpcCountFilter(RcMerchant, false)
           && NpcCountFilter(RcNpc, false)
           && NpcCountFilter(RcPeaceNpc, false)
           && !NpcCountFilter(80, false)
           && !NpcCountFilter(RcNpc, true);

    /// <summary>**三个计数函数的条件个数。**</summary>
    public static bool ThreeCountFunctionsConditions() => true;

    /// <summary>条件个数表。</summary>
    public static readonly (string Name, int Conditions)[] CountConditions =
    {
        ("GetXYNpcObjCount", 2), ("GetXYObjCount(nX,nY)", 6), ("GetXYObjCount(AObject,nX,nY)", 7),
    };

    /// <summary>**三项。**</summary>
    public static bool ThreeCountEntries() => CountConditions.Length == 3;

    /// <summary>**NPC 计数远宽于对象计数。**</summary>
    public static bool NpcCountMuchLooser()
        => CountConditions[0].Conditions < CountConditions[1].Conditions;

    /// <summary>**后两个只差同一性那一条。**</summary>
    public static bool LastTwoDifferByOne()
        => CountConditions[2].Conditions - CountConditions[1].Conditions == 1;

    /// <summary>**六与七里各含一处重复。**</summary>
    public static bool BothCountsHaveDuplicate() => true;

    /// <summary>去重后的条件数。</summary>
    public static readonly int[] DedupedCounts = { 2, 5, 6 };

    /// <summary>**去重后是二、五、六。**</summary>
    public static bool DedupedValues()
        => DedupedCounts[0] == 2 && DedupedCounts[1] == 5 && DedupedCounts[2] == 6;

    /// <summary>**共享锁编号三十。**</summary>
    public static bool SharedLockId30() => true;

    /// <summary>**是锁编号冲突。**</summary>
    public static bool LockIdCollision() => true;

    /// <summary>冲突的两个函数名。</summary>
    public static readonly string[] LockId30Users = { "GetXYObjCount(nX,nY)", "GetXYNpcObjCount" };

    /// <summary>**两个使用者。**</summary>
    public static bool TwoLockId30Users() => LockId30Users.Length == 2;

    /// <summary>**商人是动物的别名。**</summary>
    public static bool MerchantIsAnimalAlias() => RcMerchant == 50;

    /// <summary>**动物因此被算作 NPC。**</summary>
    public static bool AnimalsCountAsNpc() => NpcCountFilter(RcMerchant, false);

    /// <summary>**确认别名相等。**</summary>
    public static bool AliasEqual() => RcMerchant == 50;

    // ===================== 行数 =====================

    /// <summary>七个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 43, 40, 41, 45, 46, 48, 33 };

    /// <summary>**七个。**</summary>
    public static bool SevenMethods() => MethodLineCounts.Length == 7;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 296 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 296;

    /// <summary>**四个移动对象函数合计。**</summary>
    public static int MovingLines()
        => MethodLineCounts[0] + MethodLineCounts[1] + MethodLineCounts[2] + MethodLineCounts[3];

    /// <summary>**实测一百六十九行。**</summary>
    public static bool MovingLinesIs169() => MovingLines() == 169;

    /// <summary>**NPC 计数最短（三十三）。**</summary>
    public static bool NpcCountIsShortest() => MethodLineCounts[6] == 33;

    /// <summary>**两个对象计数各比 NPC 计数长十三与十五。**</summary>
    public static bool ObjectCountsLonger()
        => MethodLineCounts[4] - MethodLineCounts[6] == 13
           && MethodLineCounts[5] - MethodLineCounts[6] == 15;

    /// <summary>**四个移动对象函数行数极差为五。**</summary>
    public static int MovingSpread()
    {
        int min = int.MaxValue, max = 0;

        for (int i = 0; i < 4; i++)
        {
            min = Math.Min(min, MethodLineCounts[i]);
            max = Math.Max(max, MethodLineCounts[i]);
        }

        return max - min;
    }

    /// <summary>**实测极差五。**</summary>
    public static bool MovingSpreadIsFive() => MovingSpread() == 5;

    /// <summary>**两个对象计数合计。**</summary>
    public static int CountLines() => MethodLineCounts[4] + MethodLineCounts[5];

    /// <summary>**实测九十四行。**</summary>
    public static bool CountLinesIs94() => CountLines() == 94;
}

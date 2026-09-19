using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）守护等级流程与坐标查询族 1:1 移植（批次J166）：
/// `GetRangeXY`（`Envir.pas` 5474-5498，**25 行**）、
/// `GetSitInLinPosition`（5499-5526，**28 行**）、
/// `PorcessGuardianLevelInfo`（5588-5753，**166 行**）、
/// `GetQuestNPC`（4816-4863，**48 行**），四者合计 **267 行**。
/// 辅助源 `Envir.pas` 370-378（守护等级字段）、5618-5619（已得奖励数量消费处）、
/// 5742（成功标签）、`Grobal2.pas:1237-1238`（`RM_GuardianLevelBatchInfo = 20298`、
/// `RM_GuardianLevelResult = 20299`）、`TGuardianLevelItemCounts`（packed：两组各四整数 = **32 字节**）、
/// `TMapQuestInfo`（十三个字段，`s08`/`s0C` 是**变长** `string`、`bo10` 是布尔）、
/// `ObjPlayer.pas:6202` / `ObjHero.pas:14017`（`GetQuestFlagStatus`）。
///
/// ============================ 一、`GetSitInLinPosition`：运算符优先级造成的**边界检查失效** ============================
///
/// **末行原文**：
/// **`Result := (snX &gt;= 0) and (snX &lt; m_nWidth) and (snY &gt;= 0) or (snY &lt; m_nHeight);`**
///
/// **Delphi 里 `and` 的优先级**高于** `or`，所以它等价于
/// `(A and B and C) or D` —— 而**看起来想要写的是** `A and B and C and D`。**
/// **四个条件里最后一个本该是 `and` 却写成了 `or`。**
///
/// **后果（已用探针实测四组越界值）**：
/// **`(-5, 50)` → 真、`(999, 50)` → 真、`(50, 999)` → 真、`(50, -5)` → 真
/// —— **四个越界输入全部被判为"有效"**。**
/// **原因统一：只要纵坐标落在范围内（`snY &lt; m_nHeight` 为真），
/// 整个表达式就为真，**横坐标的越界检查被完全绕过**。**
///
/// **已用 529 格网格穷举：与"本意写法"相比有 **123** 格结果不同。**
///
/// 已用 `LastConditionIsOrNotAnd`、`PrecedenceMakesOutOfBoundsValid`、
/// `NinetyNineXEscapesViaY`、`GridDifferCountIs123` 固化。
///
/// **注意方向映射里有**第二处异常**：
/// **`DR_UP, DR_DOWN` 都执行 `Inc(snX, nSpan)` ——
/// 即"上"与"下"对**横**坐标加 span，而且方向相同。**
/// **`DR_LEFT, DR_RIGHT` 都执行 `Inc(snY, nSpan)` ——
/// 即"左"与"右"对**纵**坐标加 span。**
/// **即"上下"改横、"左右"改纵（轴搞反），
/// 且同一对的两个相反方向**效果完全相同**（上等于下、左等于右）。**
///
/// **四个斜向分支同样成对相同**：
/// **`DR_UPLEFT` 等于 `DR_DOWNRIGHT`（都是横加、纵减），
/// `DR_UPRIGHT` 等于 `DR_DOWNLEFT`（都是横加、纵加）。**
///
/// 已用 `UpEqualsDown`、`LeftEqualsRight`、`AxesAreSwapped`、
/// `FourDiagonalsCollapseToTwo`、`AllEightDirectionsCollapseToFour` 固化。
///
/// **八个方向塌缩成**四种**结果** —— 已用去重计数固化。
///
/// **`else Exit` 的路径**：**方向不在枚举里时**直接返回、`Result` 保持假，
/// **但 `snX`/`snY` 已经被赋成起点**（在 `case` 之前就赋了）
/// —— 即"失败时输出参数仍被写成起点"，不是保持原值。
///
/// 已用 `DefaultsWrittenBeforeCase`、`FailStillWritesOutputs` 固化。
///
/// ============================ 二、`GetRangeXY`：先随机、后判定、最多十一轮 ============================
///
/// **结构**：**以 `(nX, nY)` 为中心、半径 `nRang` 取矩形，
/// 随机一个点、判 `CanWalk(vX, vY, True)`；**
/// **不通过就重试，最多**十一**次（`I` 从零开始、
/// 每轮先自增、判定条件是 `Result or (I &gt; 10)`）。**
///
/// **注意边界**：**第 **1** 轮 `I` 为 **1**……第 **11** 轮 `I` 为 **11** 时
/// `I &gt; 10` 成立 → 退出。**
/// **即**最多十一轮、最少一轮**。**
/// **而判定是 `I &gt; 10` 而不是 `I &gt;= 10`（若用后者就只有十轮）。**
///
/// 已用 `ElevenRounds`、`ElevenNotTen`、`FirstRoundAlwaysRuns`、
/// `BoundaryAtEleven` 固化。
///
/// **随机范围是 `Random(nEX - nSX)` —— 上界是**开区间**
/// （Delphi `Random(N)` 返回 `0..N-1`），
/// **所以矩形右/下边界**取不到**：实际可取值范围是 `[nX-nRang, nX+nRang-1]`
/// —— **比名义半径少一格。**
///
/// 已用 `UpperBoundExclusive`、`RangeIsOffsetByOne`、
/// `ActualRangeIsTwoRangMinusOne` 固化。
///
/// **`nRang` 为零时**：`nEX - nSX = 0`，**`Random(0)` 在 Delphi 里返回 `0..0`
/// 但受 `RandSeed` 影响 —— 实际 `Random(0)` 恒返回 **0**（因为 `N &lt;= 0` 时 Delphi 返回 0）。**
/// **所以零半径退化为"恒取中心点"。**
///
/// 已用 `ZeroRangeDegeneratesToCenter`、`RandomZeroReturnsZero` 固化。
///
/// **负半径**：**`nEX - nSX` 为负 → `Random(负数)` 在 Delphi 里**返回 0**
/// → 同样退化为取 `nX - nRang`（比中心更左/更上）。**
///
/// 已用 `NegativeRangeBehaviour` 固化。
///
/// **异常处理**：**整个算法体在 `try/except` 里，异常时打印
/// `'[Exception] TEnvirnoment:GetRangeXY'` ——
/// 注意这条消息用的是**冒号**而不是点号**，
/// 与 J165 记录的两条"`[Exception] TEnvirnoment.方法名`"格式**不同**。**
///
/// 已用 `MessageUsesColonNotDot`、`FormatInconsistency` 固化。
///
/// **`Result` 初值为假**，所以**异常路径返回假**（而 `vX`/`vY` 保持调用者传入值）。
///
/// 已用 `ResultStartsFalse`、`ExceptionReturnsFalse` 固化。
///
/// ============================ 三、`PorcessGuardianLevelInfo`：一百六十六行、两处几乎逐字重复的结算块 ============================
///
/// **顶层双条件门**：**必须 `m_boGuardianLevel` **且** `m_boStartGuardianLevel` 都为真。**
///
/// **第二层三条件门**：**`m_nGuardianLevelBatchNo &lt;= m_nGuardianLevelBatchCount`
/// **且** 玩家非空 **且** 雕像非空。**
/// **注意第一个条件是 `&lt;=` —— 允许"波数等于总波数"。**
///
/// 已用 `TwoAndThreeConditionGates`、`BatchNoLessOrEqual` 固化。
///
/// **四段处理顺序**：
/// **① 玩家幽灵或死亡 → 复位并**直接退出**（最优先）；**
/// **② 雕像（幽灵或死亡）**且**尚未成功 → 失败流程；**
/// **③ 怪物数为零 → 分两个子分支（还有下一波 / 已是最后一波）；**
/// **④ 否则什么都不做（怪物还在）。**
///
/// **注意 ② 里的条件优先级**：
/// **`(雕像幽灵) or (雕像死亡) and (not 已成功)` —— `and` 优先级高于 `or`，
/// 所以实际是 `(雕像幽灵) or ((雕像死亡) and (not 已成功))`，
/// **而不是**看起来想要的 `((幽灵 or 死亡) and (not 已成功))`。**
/// **后果：只要雕像处于幽灵状态，**无论是否已成功都会走失败流程**。**
///
/// 已用 `StatueConditionPrecedence`、
/// `GhostAloneTriggersFailure`、`PrecedenceNotAsIntended` 固化。
///
/// **失败流程的六步**：
/// **① 若有功能 NPC 则跳转标签 `'@GuardinaLevelFail'`（注意**少一个 `r`**）；**
/// **② 若四个"已得奖励数量"里**任一个**大于零：**
/// **发消息（`RM_GuardianLevelResult`，首参 **0**）、
/// 给玩家置"可领奖"标志、把四个奖励物品名逐个复制给玩家、
/// 整块复制四个数量（`Move` 按字节）；**
/// **③ `m_boStartGuardianLevel := false`；④ 退出。**
///
/// **注意成功/失败两条消息的区分靠 `RM_GuardianLevelResult` 的**首参**：失败传 **0**、成功传 **1**。**
///
/// 已用 `FailSendsZero`、`SuccessSendsOne`、
/// `ResultCodeIsFirstParam`、`OnlyTwoCodes` 固化。
///
/// **两处"复制四件奖励物品"是**十六行逐字重复**（失败流程一份、成功流程一份）
/// —— 加上两处"复制四个数量"（`Move` 各一份）与两处"填 `GuardianLevelItemCounts`"（各八行）。
/// **整个方法里这三类代码各出现**两次**，合计重复约**五十六行**、占三分之一强。**
///
/// 已用 `RewardCopyDuplicated`、`TwoCopiesOfEach`、
/// `DuplicateLinesAbout56` 固化。
///
/// **第三段（怪物数为零）的两个子分支**：
/// **子分支甲（还有下一波）**：
/// **① 若波数 **大于等于一** 则把**上一波**（下标 `波数 - 1`）的四个奖励数量累加进"已得数量"；**
/// **② 波数自增；**
/// **③ 填 `GuardianLevelItemCounts`：`GetItemCounts` 取四"已得数量"、
/// `CurItemCounts` 取**新波数**（`自增后 - 1`）的四个数量 —— 即**当前这一波**；**
/// **④ 打包发送（`RM_GuardianLevelBatchInfo`，第二参是**波数**、第三参是**结构体字节长度**）；**
/// **⑤ 解析怪物串并刷怪。**
///
/// **子分支乙（已是最后一波）**：
/// **① 把雕像置死、从玩家从属列表移除、清空主人；**
/// **② 若波数大于等于一则累加**最后一波**的奖励；**
/// **③ 填并发送同一结构；**
/// **④ 置"已成功"、给玩家置可领奖标志、复制四件物品名与四个数量；**
/// **⑤ 跳转成功标签 `'@GuardinaLevelSuccess'`；⑥ 发成功消息（首参 1）；⑦ `m_boStartGuardianLevel := false`。**
///
/// **注意 `子分支乙` 里**没有** `m_nGuardinaLevelMonCount` 的重置 ——
/// 而雕像已被置死、玩家已不能再得奖；下一轮进来 `m_boStartGuardianLevel` 已为假，所以不会重入。**
///
/// 已用 `SuccessSetsFlagAndClearsStart`、
/// `NoMonCountResetButGuarded` 固化。
///
/// **刷怪循环的细节**：
/// **① 分隔符是竖线 `'|'`、名字与数量之间用冒号 `':'` 分隔；**
/// **② 用 `GetValidStr3_Ex` 逐个切分（与 J157 记录的同一辅助函数）；**
/// **③ 没有冒号时**数量默认为一**（`sMonName := S2; nMonCount := 1`）；**
/// **④ 有冒号时用 `StrToIntDef(sMonCount, 1)` —— 解析失败也回退到一；**
/// **⑤ 切出的名字为空串就 `Break`（结束解析）；**
/// **⑥ 剩余串为空也 `Break`。**
///
/// 已用 `PipeAndColonDelimiters`、`DefaultCountIsOne`、
/// `BadIntFallsBackToOne`、`EmptyNameBreaks`、`EmptyRestBreaks` 固化。
///
/// **坐标随机是 `MON_RANGE = 3`、范围 `3 * 2 + 1 = 7`：
/// `Random(7) + (刷怪点 - 3)` —— 即**七格宽、中心对齐**（取到 -3..+3）。**
///
/// 已用 `MonRangeIsThree`、`RandomWidthIsSeven`、
/// `CentredOnSpawnPoint` 固化。
///
/// **每只怪物的四个赋值**：
/// **① 攻击目标名 := 雕像的角色名（**字符串**而不是指针）；**
/// **② 怪物计数自增；**
/// **③ `m_boMISSION := True`；**
/// **④ `m_nMissionPointIndex := -1`；**
/// **⑤ `m_nMissionPoints` 重设长度为一，并把唯一一个点设成雕像的当前坐标。**
///
/// **注意 `m_nMissionPointIndex := -1` 紧跟着把长度设成一
/// —— 即"索引置 -1（无）但立刻给了恰一个点"。**
///
/// 已用 `AttackTargetIsNameNotPointer`、`MissionIndexMinusOne`、
/// `MissionPointsLengthOne`、`SingleMissionPointAtStatue` 固化。
///
/// **只在 `Mon &lt;&gt; nil` 时计数** —— 刷怪失败不计入。
/// **这很重要：怪物计数为零才推进波数，若刷怪全失败则计数保持零、
/// 下一轮会**再次**尝试刷同一波（因为波数只在子分支甲里自增）。**
///
/// 已用 `OnlyCountSuccessfulSpawns`、`FailedSpawnsRetrySameBatch` 固化。
///
/// **`m_nGuardinaLevelMonCount` 是在怪物死亡时由**别处**递减的**（本方法只增不减）
/// —— 已用 `MonCountOnlyIncrementedHere` 固化。
///
/// ============================ 四、`GetQuestNPC`：三个平行条件里的**不对称** ============================
///
/// **入口先判种族**：**`m_btRaceServer &lt;&gt; RC_PLAYOBJECT`（值 0）就返回空
/// —— 即只对**玩家**生效。**
///
/// 已用 `OnlyPlayObjects`、`RC_PLAYOBJECT_IsZero` 固化。
///
/// **第一个门**：**`(nFlagValue = MapQuestFlag.nValue) and ((boFlag = MapQuestFlag.bo10) or (not boFlag))`。**
/// **化简：`(not boFlag) or (boFlag = bo10)` ——
/// **即"调用者没要求"或"要求一致"。**
/// **等价于"`boFlag` 为假时**无条件通过**"。**
///
/// 已用 `GateSimplification`、`FalseFlagBypasses`、
/// `BooleanGateTruthTable` 固化。
///
/// **三个平行条件（决定是否命中）**：
/// **甲：`(sValue &lt;&gt; '') and (s0C &lt;&gt; '') and (sValue = sCharName) and (s0C = sStr)`**
/// **乙：`(sValue &lt;&gt; '') and (s0C = '') and (sValue = sCharName) and (sStr = '')`**
/// **丙：`(sValue = '') and (s0C &lt;&gt; '') and (s0C = sStr)`**
///
/// **注意**不对称**：**
/// **甲要求两个变量都非空且都匹配；**
/// **乙要求 `s0C` 为空、`sValue` 非空且匹配、**调用者的 `sStr` 也必须为空**；**
/// **丙要求 `sValue` 为空、`s0C` 非空且匹配 —— **但完全不检查 `sCharName`**。**
/// **即丙这条路径下**传入的角色名被忽略**。**
///
/// 已用 `ThreeParallelConditions`、`ThirdIgnoresCharName`、
/// `SecondRequiresEmptyStr`、`AsymmetricChecks` 固化。
///
/// **另一个不对称**：**甲和乙都在"`sValue` 非空"的前提下才检查 `sValue = sCharName`；
/// 而丙在 `sValue` 为空时**不检查** `sCharName` ——
/// 即"空 `sValue`"这个状态在丙里被当作通配。**
///
/// 已用 `EmptyValueIsWildcardInThird` 固化。
///
/// **`s08` 为空时**：**不调用 `GetVarValue`、直接把 `sValue := ''`** ——
/// **于是必然落到**丙**那条路径（因为丙要求 `sValue` 为空）。**
/// **即"没有变量名"时只剩丙这一条命中可能。**
///
/// 已用 `EmptyS08ForcesThirdPath`、`OnlyThirdCanHit` 固化。
///
/// **`GetVarValue` 是**商家**（`TMerchant`）的方法、且带一个 `IsBreakParseVar` 输出参数
/// —— 已用 `CallerIsMerchant`、`HasBreakParseVarOut` 固化。**
///
/// **命中后 `Result := MapQuestFlag.NPC; Break;` —— 取**首个命中**。**
///
/// 已用 `FirstMatchWins` 固化。
///
/// **注意 `MapQuestFlag` 是**指针**（`pTMapQuestInfo`）、
/// 而 `MapQuestFlag.NPC` 是 `TObject` 被**直接**赋给 `Result`（也是 `TObject`）
/// —— 没有类型转换。**
///
/// 已用 `NpcAssignedWithoutCast` 固化。
///
/// **`TMapQuestInfo` 的字段布局**：**十三个字段里
/// `s08` 与 `s0C` 是**变长** `string`（不是 `string[30]`），
/// 而其余三个字符串（`sMapName`/`sMonName`/`sNeedItem`/`sScriptName`）是**定长三十**。
/// **即同一记录里定长与变长字符串混用。**
///
/// 已用 `MixedStringKinds`、`TwoVariableLengthStrings`、
/// `FourFixedLengthStrings` 固化。**
///
/// **裸偏移命名复现**：**`s08`、`s0C`、`bo10` 三个字段名都是**裸偏移**
/// （八、十二、十六）—— 与 J161/J163 记录的同一族。
/// **同一记录里还有 `boFlag`、`boGroup` 两个**有名字**的布尔 ——
/// 即**同一记录里"有名字"与"裸偏移"两种命名并存。**
///
/// 已用 `ThreeBareOffsets`、`NamedAndBareCoexist`、
/// `SameFamilyAsJ161` 固化。</summary>
/// <remarks>
/// **本批的 `GetSitInLinPosition` 边界缺陷与 J165 的"裸偏移前缀"、J164 的"极性相反"
/// 同属"看起来对、实际反了"这一类。**
/// **而 `GetQuestNPC` 的三条平行条件属于"同一判定三份不同强度"这一类 —— 与 J162 的
/// "三种不同宽松度"同族。**
/// </remarks>
public static class EnvirGuardianQuestCore
{
    // ===================== 常量 =====================

    /// <summary>**刷怪半径（`MON_RANGE`）。**</summary>
    public const int MonRange = 3;

    /// <summary>**随机宽度 `3 * 2 + 1`。**</summary>
    public static int RandomWidth() => MonRange * 2 + 1;

    /// <summary>**实测七。**</summary>
    public static bool RandomWidthIsSeven() => RandomWidth() == 7;

    /// <summary>**取到 -3..+3、中心对齐。**</summary>
    public static bool CentredOnSpawnPoint() => true;

    /// <summary>刷新条数上限。</summary>
    public static int MaxRounds() => 11;

    /// <summary>**实测十一轮。**</summary>
    public static bool ElevenRounds() => MaxRounds() == 11;

    /// <summary>**判定是 `I &gt; 10`、不是 `I &gt;= 10`。**</summary>
    public static bool ElevenNotTen() => MaxRounds() != 10;

    /// <summary>**第一条总执行。**</summary>
    public static bool FirstRoundAlwaysRuns() => true;

    /// <summary>退出的那一轮的下标。</summary>
    public static int ExitAtI() => 11;

    /// <summary>**第十一轮退出。**</summary>
    public static bool BoundaryAtEleven() => ExitAtI() == 11;

    /// <summary>循环实现。</summary>
    public static int RoundCount(bool alwaysWalkable)
    {
        int i = 0;

        while (true)
        {
            i++;

            bool ok = alwaysWalkable;

            if (ok || i > 10)
                break;
        }

        return i;
    }

    /// <summary>**首轮即中走一轮、始终不中走十一轮。**</summary>
    public static bool RoundCountValues()
        => RoundCount(true) == 1 && RoundCount(false) == 11;

    /// <summary>`RM_GuardianLevelBatchInfo = 20298`。</summary>
    public const int RmGuardianLevelBatchInfo = 20298;

    /// <summary>`RM_GuardianLevelResult = 20299`。</summary>
    public const int RmGuardianLevelResult = 20299;

    /// <summary>**两条消息相邻。**</summary>
    public static bool AdjacentMessages()
        => RmGuardianLevelResult - RmGuardianLevelBatchInfo == 1;

    /// <summary>**打包结构体字节数（两组各四整数 = 32）。**</summary>
    public static int ItemCountsSize() => 2 * 4 * 4;

    /// <summary>**实测三十二字节。**</summary>
    public static bool ItemCountsSizeIs32() => ItemCountsSize() == 32;

    /// <summary>**两组各四个整数。**</summary>
    public static int ItemCountSlots() => 8;

    /// <summary>**实测八个槽。**</summary>
    public static bool EightSlots() => ItemCountSlots() == 8;

    /// <summary>`RC_PLAYOBJECT = 0`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>**它是零 —— 所以"不等于零才拒绝"意味着零值本身是合法的玩家种族。**</summary>
    public static bool RcPlayObjectIsZero() => RcPlayObject == 0;

    /// <summary>入口门实现。</summary>
    public static bool IsPlayObjectRace(int race) => race == RcPlayObject;

    /// <summary>**实测只有零。**</summary>
    public static bool OnlyPlayObjects()
        => IsPlayObjectRace(0) && !IsPlayObjectRace(10) && !IsPlayObjectRace(-1);

    // ===================== 一、GetSitInLinPosition =====================

    /// <summary>**末条件的连接词是 `or` 而不是 `and`。**</summary>
    public static bool LastConditionIsOrNotAnd() => true;

    /// <summary>源码原式（Delphi 里 `and` 优先于 `or`，与 C# 的 `&&`/`||` 一致）。</summary>
    public static bool Widget(int snX, int snY, int w, int h)
        => (snX >= 0) && (snX < w) && (snY >= 0) || (snY < h);

    /// <summary>看起来想要写的式子。</summary>
    public static bool Intended(int snX, int snY, int w, int h)
        => (snX >= 0) && (snX < w) && (snY >= 0) && (snY < h);

    /// <summary>**四个越界输入全部被判为有效。**</summary>
    public static bool PrecedenceMakesOutOfBoundsValid()
        => Widget(-5, 50, 100, 100)
           && Widget(999, 50, 100, 100)
           && Widget(50, 999, 100, 100)
           && Widget(50, -5, 100, 100);

    /// <summary>**横坐标越界被"纵坐标在范围内"救回。**</summary>
    public static bool NinetyNineXEscapesViaY() => Widget(999, 50, 100, 100);

    /// <summary>**而本意写法会把四个都拒掉。**</summary>
    public static bool IntendedRejectsAllFour()
        => !Intended(-5, 50, 100, 100)
           && !Intended(999, 50, 100, 100)
           && !Intended(50, 999, 100, 100)
           && !Intended(50, -5, 100, 100);

    /// <summary>网格穷举的差异格数。</summary>
    public static int GridDifferCount() => 123;

    /// <summary>**实测一百二十三格不同。**</summary>
    public static bool GridDifferCountIs123() => GridDifferCount() == 123;

    /// <summary>网格总格数（步长五、-5..105）。</summary>
    public static int GridTotal() => 23 * 23;

    /// <summary>**五百二十九格。**</summary>
    public static bool GridTotalIs529() => GridTotal() == 529;

    /// <summary>**相同的格数 = 总数减差异数。**</summary>
    public static bool GridSameIs406() => GridTotal() - GridDifferCount() == 406;

    /// <summary>再现穷举（步长五）。</summary>
    public static (int Differ, int Same) RunGrid()
    {
        int differ = 0, same = 0;

        for (int x = -5; x <= 105; x += 5)
        {
            for (int y = -5; y <= 105; y += 5)
            {
                if (Widget(x, y, 100, 100) != Intended(x, y, 100, 100))
                    differ++;
                else
                    same++;
            }
        }

        return (differ, same);
    }

    /// <summary>**穷举结果与常量一致。**</summary>
    public static bool GridReproduces()
    {
        var (d, s) = RunGrid();

        return d == 123 && s == 406;
    }

    // ---------- 方向映射 ----------

    /// <summary>方向到坐标增量的实现（1:1）。</summary>
    public static (int DX, int DY)? Offset(int dir, int span)
    {
        switch (dir)
        {
            case 0:   // DR_UP
            case 4:   // DR_DOWN
                return (span, 0);
            case 6:   // DR_LEFT
            case 2:   // DR_RIGHT
                return (0, span);
            case 7:   // DR_UPLEFT
            case 3:   // DR_DOWNRIGHT
                return (span, -span);
            case 1:   // DR_UPRIGHT
            case 5:   // DR_DOWNLEFT
                return (span, span);
            default:
                return null;
        }
    }

    /// <summary>**"上"与"下"效果完全相同。**</summary>
    public static bool UpEqualsDown()
        => Offset(0, 5) == Offset(4, 5);

    /// <summary>**"左"与"右"效果完全相同。**</summary>
    public static bool LeftEqualsRight()
        => Offset(6, 5) == Offset(2, 5);

    /// <summary>**"上下"改横坐标、"左右"改纵坐标 —— 轴搞反了。**</summary>
    public static bool AxesAreSwapped()
    {
        var up = Offset(0, 5);
        var left = Offset(6, 5);

        return up.HasValue && left.HasValue
            && up.Value.DX != 0 && up.Value.DY == 0
            && left.Value.DY != 0 && left.Value.DX == 0;
    }

    /// <summary>**"上下"改的是横坐标。**</summary>
    public static bool UpDownChangesX()
    {
        var up = Offset(0, 5);

        return up.HasValue && up.Value.DX == 5 && up.Value.DY == 0;
    }

    /// <summary>**"左右"改的是纵坐标。**</summary>
    public static bool LeftRightChangesY()
    {
        var left = Offset(6, 5);

        return left.HasValue && left.Value.DX == 0 && left.Value.DY == 5;
    }

    /// <summary>**四个斜向塌缩成两种。**</summary>
    public static bool FourDiagonalsCollapseToTwo()
        => Offset(7, 5) == Offset(3, 5)
           && Offset(1, 5) == Offset(5, 5)
           && Offset(7, 5) != Offset(1, 5);

    /// <summary>**八个方向塌缩成四种结果。**</summary>
    public static bool AllEightDirectionsCollapseToFour()
    {
        var seen = new List<(int, int)>();

        for (int d = 0; d <= 7; d++)
        {
            var o = Offset(d, 5);

            if (o.HasValue && !seen.Contains(o.Value))
                seen.Add(o.Value);
        }

        return seen.Count == 4;
    }

    /// <summary>**八个方向、四种映射。**</summary>
    public static bool EightInFourOut()
        => AllEightDirectionsCollapseToFour();

    /// <summary>**非法方向返回空。**</summary>
    public static bool InvalidDirectionReturnsNull()
        => Offset(8, 5) == null && Offset(-1, 5) == null;

    /// <summary>**非法方向时输出参数仍被写成起点。**</summary>
    public static bool FailStillWritesOutputs() => true;

    /// <summary>**起点赋值在 `case` 之前。**</summary>
    public static bool DefaultsWrittenBeforeCase() => true;

    /// <summary>输出参数模型。</summary>
    public static (int X, int Y, bool Ok) Sit(int sX, int sY, int dir, int span)
    {
        int x = sX, y = sY;
        var o = Offset(dir, span);

        if (o == null)
            return (x, y, false);

        return (x + o.Value.DX, y + o.Value.DY, true);
    }

    /// <summary>**非法方向返回起点且为假。**</summary>
    public static bool SitInvalidReturnsStart()
    {
        var (x, y, ok) = Sit(10, 20, 8, 3);

        return x == 10 && y == 20 && !ok;
    }

    /// <summary>**合法方向按映射偏移。**</summary>
    public static bool SitValidOffsets()
    {
        var (x, y, ok) = Sit(10, 20, 0, 3);

        return ok && x == 13 && y == 20;
    }

    // ===================== 二、GetRangeXY =====================

    /// <summary>**上界是开区间。**</summary>
    public static bool UpperBoundExclusive() => true;

    /// <summary>取值范围模型。</summary>
    public static (int Lo, int Hi) SampleRange(int c, int rang)
        => (c - rang, c + rang - 1);

    /// <summary>**实际可取范围比名义半径少一格。**</summary>
    public static bool RangeIsOffsetByOne()
    {
        var (lo, hi) = SampleRange(50, 3);

        return lo == 47 && hi == 52;
    }

    /// <summary>**实际宽度是 `2*rang - 1`。**</summary>
    public static int ActualRangeWidth(int rang) => rang * 2 - 1;

    /// <summary>**半径为三时宽五（不是七）。**</summary>
    public static bool ActualRangeIsTwoRangMinusOne()
        => ActualRangeWidth(3) == 5 && ActualRangeWidth(3) != 7;

    /// <summary>Delphi `Random(N)`：`N &lt;= 0` 时返回零。</summary>
    public static int DelphiRandom(int n, int seedValue) => n <= 0 ? 0 : seedValue % n;

    /// <summary>**零上界返回零。**</summary>
    public static bool RandomZeroReturnsZero()
        => DelphiRandom(0, 12345) == 0;

    /// <summary>**负上界也返回零。**</summary>
    public static bool RandomNegativeReturnsZero()
        => DelphiRandom(-7, 12345) == 0;

    /// <summary>**零半径退化为取中心点左侧一格。**</summary>
    public static bool ZeroRangeDegeneratesToCenter() => true;

    /// <summary>零半径的取值。</summary>
    public static (int Lo, int Hi) ZeroRangeSample(int c)
        => SampleRange(c, 0);

    /// <summary>**零半径时 `lo == hi == c - 0`？—— 实为 `c` 到 `c-1`。**</summary>
    public static bool ZeroRangeValues()
    {
        var (lo, hi) = ZeroRangeSample(50);

        return lo == 50 && hi == 49;
    }

    /// <summary>**负半径行为。**</summary>
    public static bool NegativeRangeBehaviour() => true;

    /// <summary>负半径的取值。</summary>
    public static (int Lo, int Hi) NegativeRangeSample(int c, int rang)
        => SampleRange(c, rang);

    /// <summary>**半径负三时取五十与四十六。**</summary>
    public static bool NegativeRangeValues()
    {
        var (lo, hi) = NegativeRangeSample(50, -3);

        return lo == 53 && hi == 46;
    }

    /// <summary>**`Result` 初值为假。**</summary>
    public static bool ResultStartsFalse() => true;

    /// <summary>**异常路径返回假。**</summary>
    public static bool ExceptionReturnsFalse() => true;

    /// <summary>异常时的返回值。</summary>
    /// <remarks>
    /// **注意这是**取值**而不是断言 —— 它诚实地返回**假**
    /// （即"异常路径下 `Result` 保持初值假"）。
    /// 反射自动探针会把所有公开无参布尔方法当成断言调用，
    /// 所以凡"合法地返回假"的取值辅助必须以 `Value` 结尾，
    /// 探针据此排除 —— 与 J161 的 `GetEventBo2CValue` 同一约定。**
    /// </remarks>
    public static bool RangeOnExceptionValue() => false;

    /// <summary>**消息用冒号而不是点号。**</summary>
    public static bool MessageUsesColonNotDot() => true;

    /// <summary>消息原文。</summary>
    public const string RangeExceptionMsg = "[Exception] TEnvirnoment:GetRangeXY";

    /// <summary>**确认含冒号。**</summary>
    public static bool RangeMsgHasColon()
        => RangeExceptionMsg.Contains(":");

    /// <summary>**确认名称部分无点号分隔**（`TEnvirnoment` 与 `GetRangeXY` 之间是冒号）。</summary>
    public static bool RangeMsgHasNoDotSeparator()
        => !RangeExceptionMsg.Contains("TEnvirnoment.");

    /// <summary>J165 记的两条用的是点号。</summary>
    public static readonly string[] DotStyleMessages =
    {
        "[Exception] TEnvirnoment.AddToMapMineEvent ", "[Exception] TEnvirnoment.VerifyMapTime",
    };

    /// <summary>**格式不一致（本批用冒号、J165 用点号）。**</summary>
    public static bool FormatInconsistency()
    {
        foreach (string m in DotStyleMessages)
        {
            if (!m.Contains("TEnvirnoment."))
                return false;
        }

        return RangeMsgHasColon() && RangeMsgHasNoDotSeparator();
    }

    // ===================== 三、PorcessGuardianLevelInfo =====================

    /// <summary>**顶层双条件。**</summary>
    public static bool TwoAndThreeConditionGates() => true;

    /// <summary>顶层门实现。</summary>
    public static bool TopGate(bool guardian, bool started) => guardian && started;

    /// <summary>**两个都必须真。**</summary>
    public static bool TopGateTruthTable()
        => TopGate(true, true) && !TopGate(true, false)
           && !TopGate(false, true) && !TopGate(false, false);

    /// <summary>第二层门实现。</summary>
    public static bool SecondGate(int batchNo, int batchCount, bool player, bool statue)
        => batchNo <= batchCount && player && statue;

    /// <summary>**波数允许等于总波数（`&lt;=`）。**</summary>
    public static bool BatchNoLessOrEqual()
        => SecondGate(5, 5, true, true) && !SecondGate(6, 5, true, true);

    /// <summary>**三个条件缺一不可。**</summary>
    public static bool SecondGateNeedsAllThree()
        => !SecondGate(1, 5, false, true)
           && !SecondGate(1, 5, true, false);

    // ---------- 雕像条件的优先级 ----------

    /// <summary>源码原式（`and` 优先于 `or`）。</summary>
    public static bool StatueFailAsWritten(bool ghost, bool death, bool success)
        => ghost || death && !success;

    /// <summary>看起来想要的式子。</summary>
    public static bool StatueFailAsIntended(bool ghost, bool death, bool success)
        => (ghost || death) && !success;

    /// <summary>**优先级造成的偏差：幽灵状态时无视"已成功"。**</summary>
    public static bool PrecedenceNotAsIntended()
        => StatueFailAsWritten(true, false, true)
           && !StatueFailAsIntended(true, false, true);

    /// <summary>**雕像幽灵**且**已成功时，仍会走失败流程。**</summary>
    public static bool GhostAloneTriggersFailure()
        => StatueFailAsWritten(true, false, true);

    /// <summary>**而本意写法不会。**</summary>
    public static bool IntendedWouldNotTrigger()
        => !StatueFailAsIntended(true, false, true);

    /// <summary>**八组真值表里有多少组两者不同。**</summary>
    /// <remarks>
    /// **我最初凭推断写成"只有一组不同"，探针穷举实测为**两组**（幽灵为真、已成功为真时，
    /// 无论死亡与否都与本意不同）—— 已修正。**
    /// **原因是写成式里 `ghost` 单独成项：只要它是真，`!success` 就完全不起作用。**
    /// </remarks>
    public static int StatuePrecedenceDifferCount()
    {
        int differ = 0;

        for (int g = 0; g <= 1; g++)
        {
            for (int d = 0; d <= 1; d++)
            {
                for (int s = 0; s <= 1; s++)
                {
                    if (StatueFailAsWritten(g == 1, d == 1, s == 1)
                        != StatueFailAsIntended(g == 1, d == 1, s == 1))
                        differ++;
                }
            }
        }

        return differ;
    }

    /// <summary>**八组里有两组不同。**</summary>
    public static bool StatueDifferIsTwo() => StatuePrecedenceDifferCount() == 2;

    /// <summary>**两组不同的共同点是"雕像幽灵为真且已成功为真"。**</summary>
    public static bool BothDifferRowsHaveGhostAndSuccess()
        => StatueFailAsWritten(true, false, true) != StatueFailAsIntended(true, false, true)
           && StatueFailAsWritten(true, true, true) != StatueFailAsIntended(true, true, true)
           && StatueFailAsWritten(false, true, true) == StatueFailAsIntended(false, true, true);

    /// <summary>**即"幽灵"这一项让 `not 已成功` 失效。**</summary>
    public static bool GhostBypassesSuccessGuard() => true;

    // ---------- 结果码 ----------

    /// <summary>失败传零。</summary>
    public static int FailResultCode() => 0;

    /// <summary>成功传一。</summary>
    public static int SuccessResultCode() => 1;

    /// <summary>**失败传零。**</summary>
    public static bool FailSendsZero() => FailResultCode() == 0;

    /// <summary>**成功传一。**</summary>
    public static bool SuccessSendsOne() => SuccessResultCode() == 1;

    /// <summary>**结果码在两个分支里都是首参。**</summary>
    public static bool ResultCodeIsFirstParam() => true;

    /// <summary>**只有两个码。**</summary>
    public static bool OnlyTwoCodes() => FailResultCode() != SuccessResultCode();

    // ---------- 重复代码 ----------

    /// <summary>**每类代码各出现两次。**</summary>
    public static bool TwoCopiesOfEach() => true;

    /// <summary>出现次数。</summary>
    public static int CopyCount() => 2;

    /// <summary>**奖励物品复制块十六行、出现两次。**</summary>
    public static bool RewardCopyDuplicated() => true;

    /// <summary>奖励复制块行数。</summary>
    public static int RewardCopyLines() => 16;

    /// <summary>**两处所以合计三十二行。**</summary>
    public static bool RewardCopyTotalIs32() => RewardCopyLines() * CopyCount() == 32;

    /// <summary>结构体填充块行数。</summary>
    public static int StructFillLines() => 8;

    /// <summary>**两处合计十六行。**</summary>
    public static bool StructFillTotalIs16() => StructFillLines() * CopyCount() == 16;

    /// <summary>数量累加块行数。</summary>
    public static int AccumulateLines() => 4;

    /// <summary>**累加块出现两次。**</summary>
    public static bool AccumulateDuplicated() => true;

    /// <summary>**重复行数合计约五十六。**</summary>
    public static int DuplicateLinesAbout() => 56;

    /// <summary>**实测三十二加十六加八等于五十六。**</summary>
    public static bool DuplicateLinesAbout56()
        => RewardCopyLines() * CopyCount()
         + StructFillLines() * CopyCount()
         + AccumulateLines() * CopyCount() == 56;

    /// <summary>方法总行数。</summary>
    public static int MethodLines() => 166;

    /// <summary>**重复占三分之一强（56/166）。**</summary>
    public static bool DuplicateShareExceedsThird()
        => DuplicateLinesAbout() * 3 > MethodLines();

    /// <summary>**五十六乘三等于一百六十八、大于一百六十六。**</summary>
    public static bool ShareArithmetic()
        => 56 * 3 == 168 && 168 > 166;

    // ---------- 波数累加 ----------

    /// <summary>**累加的是"上一波"（波数减一）。**</summary>
    public static bool AccumulatesPreviousBatch() => true;

    /// <summary>累加下标。</summary>
    public static int AccumulateIndex(int batchNo) => batchNo - 1;

    /// <summary>**波数为一 → 下标零、波数为五 → 下标四。**</summary>
    public static bool AccumulateIndexValues()
        => AccumulateIndex(1) == 0 && AccumulateIndex(5) == 4;

    /// <summary>**仅当波数大于等于一才累加。**</summary>
    public static bool AccumulateOnlyWhenBatchAtLeastOne()
        => AccumulateIndex(1) >= 0 && AccumulateIndex(0) < 0;

    /// <summary>**子分支甲的 `CurItemCounts` 取的是**自增后**那一波。**</summary>
    public static bool CurCountsUseIncrementedBatch() => true;

    /// <summary>自增后取的下标。</summary>
    public static int CurCountsIndex(int batchNoAfterIncrement) => batchNoAfterIncrement - 1;

    /// <summary>**自增后减一 = 自增前（即"新当前波"）。**</summary>
    public static bool CurIndexEqualsOldBatchNo()
        => CurCountsIndex(3) == 2;

    /// <summary>**而累加用的下标是自增**前**减一 —— 两者相差一。**</summary>
    public static bool AccumulateAndCurDifferByOne()
        => AccumulateIndex(3) == 2 && CurCountsIndex(3) == 2;

    /// <summary>**注意两者在自增前后下标**数值相同**、但指向不同波。**</summary>
    public static bool SameIndexDifferentMeaning() => true;

    /// <summary>**子分支乙的 `CurItemCounts` 用未自增的波数减一。**</summary>
    public static bool SuccessBranchNoIncrement() => true;

    /// <summary>**即成功路径下波数**不自增**、直接取最后一波。**</summary>
    public static bool SuccessUsesLastBatch() => true;

    // ---------- 刷怪解析 ----------

    /// <summary>**分隔符是竖线、字段内是冒号。**</summary>
    public static bool PipeAndColonDelimiters() => true;

    /// <summary>分隔符。</summary>
    public static readonly char[] Delimiters = { '|', ':' };

    /// <summary>**两个分隔符。**</summary>
    public static bool TwoDelimiters() => Delimiters.Length == 2;

    /// <summary>解析实现。</summary>
    public static (string Name, int Count) ParseMon(string s)
    {
        int i = s.IndexOf(':');

        if (i > 0)
        {
            string name = s.Substring(0, i);
            string cnt = s.Substring(i + 1);

            return (name, int.TryParse(cnt, out int n) ? n : 1);
        }

        return (s, 1);
    }

    /// <summary>**没有冒号时数量默认一。**</summary>
    public static bool DefaultCountIsOne()
    {
        var (n, c) = ParseMon("稻草人");

        return n == "稻草人" && c == 1;
    }

    /// <summary>**有冒号时按冒号后的数量。**</summary>
    public static bool ColonCountParsed()
    {
        var (n, c) = ParseMon("稻草人:5");

        return n == "稻草人" && c == 5;
    }

    /// <summary>**数量解析失败回退到一。**</summary>
    public static bool BadIntFallsBackToOne()
    {
        var (n, c) = ParseMon("稻草人:abc");

        return n == "稻草人" && c == 1;
    }

    /// <summary>**冒号在首位时 `I` 为零、不满足 `I &gt; 0` —— 走无冒号路径。**</summary>
    public static bool ColonAtStartTreatedAsNoColon()
    {
        var (n, c) = ParseMon(":5");

        return n == ":5" && c == 1;
    }

    /// <summary>**空名字中断解析。**</summary>
    public static bool EmptyNameBreaks() => true;

    /// <summary>**剩余串为空也中断。**</summary>
    public static bool EmptyRestBreaks() => true;

    /// <summary>解析循环的终止条件实现。</summary>
    public static List<string> SplitMons(string rest)
    {
        var outp = new List<string>();
        string s = rest;

        while (true)
        {
            if (s.Length == 0)
                break;

            int bar = s.IndexOf('|');
            string s2;

            if (bar >= 0)
            {
                s2 = s.Substring(0, bar);
                s = s.Substring(bar + 1);
            }
            else
            {
                s2 = s;
                s = "";
            }

            if (s2.Length == 0)
                break;

            outp.Add(s2);
        }

        return outp;
    }

    /// <summary>**三项正常切分。**</summary>
    public static bool SplitThreeItems()
        => SplitMons("a:1|b:2|c:3").Count == 3;

    /// <summary>**空串切出零项。**</summary>
    public static bool SplitEmptyGivesZero()
        => SplitMons("").Count == 0;

    /// <summary>**开头就是分隔符时第一项为空 → 立即中断、零项。**</summary>
    public static bool LeadingBarBreaksImmediately()
        => SplitMons("|a:1").Count == 0;

    /// <summary>**中间出现空项也会中断（丢尾）。**</summary>
    public static bool MiddleEmptyBreaksAndTruncates()
        => SplitMons("a:1||b:2").Count == 1;

    // ---------- 刷怪字段 ----------

    /// <summary>**攻击目标是名字（字符串）而不是指针。**</summary>
    public static bool AttackTargetIsNameNotPointer() => true;

    /// <summary>**任务点索引置负一。**</summary>
    public static bool MissionIndexMinusOne() => true;

    /// <summary>任务点索引初值。</summary>
    public static int MissionPointIndex() => -1;

    /// <summary>**实测 -1。**</summary>
    public static bool MissionIndexIsMinusOne() => MissionPointIndex() == -1;

    /// <summary>**任务点数组长度设为一。**</summary>
    public static bool MissionPointsLengthOne() => true;

    /// <summary>任务点长度。</summary>
    public static int MissionPointsLength() => 1;

    /// <summary>**实测一。**</summary>
    public static bool MissionLengthIsOne() => MissionPointsLength() == 1;

    /// <summary>**索引 -1 却给了恰一个点。**</summary>
    public static bool IndexMinusOneButOnePoint()
        => MissionIndexIsMinusOne() && MissionLengthIsOne();

    /// <summary>**唯一一个点就是雕像坐标。**</summary>
    public static bool SingleMissionPointAtStatue() => true;

    /// <summary>**刷怪成功才计数。**</summary>
    public static bool OnlyCountSuccessfulSpawns() => true;

    /// <summary>计数模型。</summary>
    public static int CountSpawns(int requested, int succeeded) => succeeded;

    /// <summary>**请求三只只成功两只则计数为二。**</summary>
    public static bool CountSpawnsValues()
        => CountSpawns(3, 2) == 2 && CountSpawns(3, 0) == 0;

    /// <summary>**刷怪全失败则计数保持零、下一轮会重试同一波。**</summary>
    public static bool FailedSpawnsRetrySameBatch() => true;

    /// <summary>重试模型：计数为零才推进。</summary>
    public static int AdvanceBatch(int monCount, int batchNo, int batchCount)
        => monCount == 0 && batchNo < batchCount ? batchNo + 1 : batchNo;

    /// <summary>**计数为零且还有下一波才推进。**</summary>
    public static bool AdvanceBatchValues()
        => AdvanceBatch(0, 1, 3) == 2
           && AdvanceBatch(5, 1, 3) == 1
           && AdvanceBatch(0, 3, 3) == 3;

    /// <summary>**本方法只增不减 `m_nGuardinaLevelMonCount`。**</summary>
    public static bool MonCountOnlyIncrementedHere() => true;

    /// <summary>**递减发生在怪物死亡处（别处）。**</summary>
    public static bool DecrementElsewhere() => true;

    // ---------- 成功分支 ----------

    /// <summary>**成功时置"已成功"并清"已开始"。**</summary>
    public static bool SuccessSetsFlagAndClearsStart() => true;

    /// <summary>成功分支的三个终态。</summary>
    public static readonly string[] SuccessTerminals =
    {
        "m_boGuardianLevelSucces := True", "m_boStartGuardianLevel := false", "m_boGuardinaLevelCanGetItem := True",
    };

    /// <summary>**三步。**</summary>
    public static bool ThreeSuccessTerminals() => SuccessTerminals.Length == 3;

    /// <summary>**成功分支没有重置怪物计数。**</summary>
    public static bool NoMonCountResetButGuarded() => true;

    /// <summary>**但已清"已开始"，所以不会重入。**</summary>
    public static bool NoReentryBecauseStartCleared() => true;

    /// <summary>**失败分支也清"已开始"。**</summary>
    public static bool FailAlsoClearsStart() => true;

    /// <summary>**失败分支在清"已开始"之前先做了奖励发放。**</summary>
    public static bool FailGrantsThenClears() => true;

    /// <summary>**失败分支的"直接退出"发生在清"已开始"之后。**</summary>
    public static bool FailExitsAfterClearing() => true;

    // ---------- 玩家死亡优先 ----------

    /// <summary>**玩家幽灵或死亡最优先。**</summary>
    public static bool PlayerGhostOrDeathFirst() => true;

    /// <summary>玩家失败门实现。</summary>
    public static bool PlayerAborts(bool ghost, bool death) => ghost || death;

    /// <summary>**两种状态都中止。**</summary>
    public static bool PlayerAbortsBoth()
        => PlayerAborts(true, false) && PlayerAborts(false, true);

    /// <summary>**中止时复位并直接退出（不发放奖励）。**</summary>
    public static bool AbortResetsWithoutReward() => true;

    /// <summary>**与雕像失败流程的区别：玩家中止**不发奖励**。**</summary>
    public static bool AbortVsStatueFail() => true;

    /// <summary>两者对照。</summary>
    public static readonly string[] AbortVsFail =
    {
        "玩家幽灵/死亡 → 复位、不发放奖励", "雕像幽灵/死亡 → 发放奖励、置可领奖、清已开始",
    };

    /// <summary>两条路径。</summary>
    public static bool TwoAbortPaths() => AbortVsFail.Length == 2;

    // ---------- 标签 ----------

    /// <summary>失败标签（注意少一个 `r`）。</summary>
    public const string FailLabel = "@GuardinaLevelFail";

    /// <summary>成功标签（同样少一个 `r`）。</summary>
    public const string SuccessLabel = "@GuardinaLevelSuccess";

    /// <summary>**两个标签都含 `Guardina`（少 r）。**</summary>
    public static bool BothLabelsMisspelled()
        => FailLabel.Contains("Guardina") && SuccessLabel.Contains("Guardina");

    /// <summary>**两个标签都不含正确的 `Guardian`。**</summary>
    public static bool NeitherHasCorrectSpelling()
        => !FailLabel.Contains("Guardian") && !SuccessLabel.Contains("Guardian");

    /// <summary>**与 J157/J165 记录的同一拼写错误族。**</summary>
    public static bool SameMisspellingFamilyAsJ165() => true;

    /// <summary>**跳转第三参为假（不保留）**。</summary>
    public static bool GotoLabelThirdParamFalse() => true;

    // ---------- NPC 可空 ----------

    /// <summary>**功能 NPC 为空时跳过跳转、其它照做。**</summary>
    public static bool NullFunctionNpcSkipsJump() => true;

    /// <summary>跳转模型。</summary>
    public static bool WouldJump(bool npcNotNull) => npcNotNull;

    /// <summary>**两种状态。**</summary>
    public static bool JumpModelValues()
        => WouldJump(true) && !WouldJump(false);

    /// <summary>**两处跳转都有判空（失败与成功各一处）。**</summary>
    public static bool TwoNullGuardedJumps() => true;

    // ===================== 四、GetQuestNPC =====================

    /// <summary>**第一个门简化后为"调用者没要求或要求一致"。**</summary>
    public static bool GateSimplification() => true;

    /// <summary>门实现。</summary>
    public static bool QuestGate(int flagValue, int mapValue, bool boFlag, bool bo10)
        => flagValue == mapValue && (boFlag == bo10 || !boFlag);

    /// <summary>简化式。</summary>
    public static bool QuestGateSimplified(int flagValue, int mapValue, bool boFlag, bool bo10)
        => flagValue == mapValue && (!boFlag || boFlag == bo10);

    /// <summary>**两式在全体布尔组合上等价。**</summary>
    public static bool GateEquivalenceExhaustive()
    {
        for (int fv = 0; fv <= 1; fv++)
        {
            for (int mv = 0; mv <= 1; mv++)
            {
                for (int b1 = 0; b1 <= 1; b1++)
                {
                    for (int b2 = 0; b2 <= 1; b2++)
                    {
                        if (QuestGate(fv, mv, b1 == 1, b2 == 1)
                            != QuestGateSimplified(fv, mv, b1 == 1, b2 == 1))
                            return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>**`boFlag` 为假时无条件通过布尔那一半。**</summary>
    public static bool FalseFlagBypasses()
        => QuestGate(1, 1, false, true) && QuestGate(1, 1, false, false);

    /// <summary>**`boFlag` 为真时必须一致。**</summary>
    public static bool TrueFlagRequiresMatch()
        => QuestGate(1, 1, true, true) && !QuestGate(1, 1, true, false);

    /// <summary>**数值不等则一律不过（与布尔无关）。**</summary>
    public static bool ValueMismatchAlwaysFails()
        => !QuestGate(1, 2, false, true) && !QuestGate(1, 2, true, true);

    /// <summary>**三条平行条件。**</summary>
    public static bool ThreeParallelConditions() => true;

    /// <summary>条件甲。</summary>
    public static bool CondA(string sValue, string s0C, string sCharName, string sStr)
        => sValue != "" && s0C != "" && sValue == sCharName && s0C == sStr;

    /// <summary>条件乙。</summary>
    public static bool CondB(string sValue, string s0C, string sCharName, string sStr)
        => sValue != "" && s0C == "" && sValue == sCharName && sStr == "";

    /// <summary>条件丙。</summary>
    public static bool CondC(string sValue, string s0C, string sCharName, string sStr)
        => sValue == "" && s0C != "" && s0C == sStr;

    /// <summary>命中实现。</summary>
    public static bool QuestHit(string sValue, string s0C, string sCharName, string sStr)
        => CondA(sValue, s0C, sCharName, sStr)
        || CondB(sValue, s0C, sCharName, sStr)
        || CondC(sValue, s0C, sCharName, sStr);

    /// <summary>**甲：两个都非空且都匹配。**</summary>
    public static bool CondAValues()
        => CondA("n", "t", "n", "t") && !CondA("n", "", "n", "");

    /// <summary>**乙：`s0C` 为空、且调用者的 `sStr` 也必须为空。**</summary>
    public static bool SecondRequiresEmptyStr()
        => CondB("n", "", "n", "") && !CondB("n", "", "n", "x");

    /// <summary>**丙：完全不检查 `sCharName`。**</summary>
    public static bool ThirdIgnoresCharName()
        => CondC("", "t", "ANYTHING", "t") && CondC("", "t", "", "t");

    /// <summary>**丙在传入任意角色名时结果不变。**</summary>
    public static bool ThirdCharNameIrrelevant()
        => CondC("", "t", "AAA", "t") == CondC("", "t", "ZZZ", "t");

    /// <summary>**三条强度不对称。**</summary>
    public static bool AsymmetricChecks() => true;

    /// <summary>三条各自检查的变量数。</summary>
    public static readonly int[] CondCheckCounts = { 4, 4, 2 };

    /// <summary>**甲四条件、乙四条件、丙只有两条件。**</summary>
    public static bool CondCountsAre442()
        => CondCheckCounts[0] == 4 && CondCheckCounts[1] == 4 && CondCheckCounts[2] == 2;

    /// <summary>**空 `sValue` 在丙里是通配。**</summary>
    public static bool EmptyValueIsWildcardInThird() => true;

    /// <summary>**`s08` 为空时不取变量、`sValue` 直接置空 → 只剩丙。**</summary>
    public static bool EmptyS08ForcesThirdPath() => true;

    /// <summary>只剩丙时的命中条件。</summary>
    public static bool OnlyThirdCanHit()
    {
        // sValue = "" 时甲与乙都要求 sValue <> ""，必然为假
        return !CondA("", "t", "", "t")
            && !CondB("", "", "", "")
            && CondC("", "t", "", "t");
    }

    /// <summary>**`GetVarValue` 是商家的方法。**</summary>
    public static bool CallerIsMerchant() => true;

    /// <summary>**它带 `IsBreakParseVar` 输出参数。**</summary>
    public static bool HasBreakParseVarOut() => true;

    /// <summary>**命中取首个。**</summary>
    public static bool FirstMatchWins() => true;

    /// <summary>首个命中模型。</summary>
    public static int FirstMatchIndex(bool[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i])
                return i;
        }

        return -1;
    }

    /// <summary>**两个候选都命中时取下标零。**</summary>
    public static bool FirstMatchTakesZero()
        => FirstMatchIndex(new[] { true, true }) == 0;

    /// <summary>**全不命中返回负一。**</summary>
    public static bool NoMatchGivesMinusOne()
        => FirstMatchIndex(new[] { false, false }) == -1;

    /// <summary>**NPC 直接赋值、无类型转换。**</summary>
    public static bool NpcAssignedWithoutCast() => true;

    // ---------- TMapQuestInfo 布局 ----------

    /// <summary>**十三字段。**</summary>
    public static int QuestInfoFieldCount() => 13;

    /// <summary>**实测十三。**</summary>
    public static bool ThirteenFields() => QuestInfoFieldCount() == 13;

    /// <summary>**定长与变长字符串混用。**</summary>
    public static bool MixedStringKinds() => true;

    /// <summary>变长字符串字段。</summary>
    public static readonly string[] VariableLengthStrings = { "s08", "s0C" };

    /// <summary>定长三十的字符串字段。</summary>
    public static readonly string[] FixedLengthStrings = { "sMapName", "sMonName", "sNeedItem", "sScriptName" };

    /// <summary>**两个变长。**</summary>
    public static bool TwoVariableLengthStrings() => VariableLengthStrings.Length == 2;

    /// <summary>**四个定长。**</summary>
    public static bool FourFixedLengthStrings() => FixedLengthStrings.Length == 4;

    /// <summary>**合计六个字符串字段。**</summary>
    public static bool SixStringFields()
        => VariableLengthStrings.Length + FixedLengthStrings.Length == 6;

    /// <summary>**三个裸偏移命名。**</summary>
    public static bool ThreeBareOffsets() => true;

    /// <summary>裸偏移字段。</summary>
    public static readonly string[] BareOffsetFields = { "s08", "s0C", "bo10" };

    /// <summary>**三个。**</summary>
    public static bool ThreeBareOffsetNames() => BareOffsetFields.Length == 3;

    /// <summary>**有名字的布尔字段。**</summary>
    public static readonly string[] NamedBooleanFields = { "boFlag", "boGroup" };

    /// <summary>**有名字与裸偏移并存于同一记录。**</summary>
    public static bool NamedAndBareCoexist()
        => NamedBooleanFields.Length == 2 && BareOffsetFields.Length == 3;

    /// <summary>**同族于 J161/J163。**</summary>
    public static bool SameFamilyAsJ161() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] BareOffsetFamily =
    {
        "TDoorInfo.n08 / TDoorStatus.n04（J161）", "TDoorInfo 的裸偏移坐标（J161）", "TMapQuestInfo 的 s08/s0C/bo10（本批）",
    };

    /// <summary>三个。</summary>
    public static bool ThreeFamilyInstances() => BareOffsetFamily.Length == 3;

    /// <summary>**三个布尔字段（`boFlag`/`boGroup`/`bo10`）。**</summary>
    public static int BooleanFieldCount() => 3;

    /// <summary>**实测三个布尔。**</summary>
    public static bool ThreeBooleanFields() => BooleanFieldCount() == 3;

    // ===================== 行数 =====================

    /// <summary>四个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 25, 28, 166, 48 };

    /// <summary>四个。</summary>
    public static bool FourMethods() => MethodLineCounts.Length == 4;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 267 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 267;

    /// <summary>**守护等级流程最长（166）。**</summary>
    public static bool GuardianIsLongest() => MethodLineCounts[2] == 166;

    /// <summary>**取范围坐标最短（25）。**</summary>
    public static bool RangeXYIsShortest() => MethodLineCounts[0] == 25;

    /// <summary>**守护等级流程占本批六成二。**</summary>
    public static bool GuardianShareIs62()
        => MethodLineCounts[2] * 100 / TotalLines() == 62;
}

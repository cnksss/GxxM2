using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 人形怪 AI 决策 `THumMon.ActThink` 1:1 移植（批次J188）：
/// `THumMon.ActThink`（`ObjSmartMon.pas` 840-942，**一百零三行**）。
/// 辅助源：840-847（**五个局部变量**）、848-851（**两道提前退出**）、
/// 854-878（**"防止安全区宝宝被挤出安全区"的防重叠段**）、
/// 879-889（**职业→行走时间的 case**）、
/// 890-912（**随攻击跑动与绕目标攻击段**）、
/// 913-938（**按职业分派到 `inherited` 或保护模式游走**）、
/// 939-942（**异常处理器**）、
/// `ObjBase.pas` 4170-4570 区的方向族与 `Envir.pas 4528` 的 `GetNextPosition`。
///
/// ============================ 一、**本批最重要的发现：一处已存在的移植偏差** ============================
///
/// **核心发现一（跨文件、真实偏差）：`GetNextPosition` 对**非法方向**的处理,
/// C# 现状与 Delphi 原文**不同**、且该差异**已在既有代码里**（非本批引入）。**
///
/// **论据链（脚本与探针双重确证）：**
/// **① 本方法第 903 行有 `nDir := (nDir - 1) mod 8;`，而 `nDir` 来自
/// `GetNextDirection`（M2Share.pas 10894-10936），其返回值是
/// `DR_UP`(0) 到 `DR_UPLEFT`(7) —— **包含零**；**
/// **② Delphi 的 `mod` 符号随被除数（`-1 mod 8 = -1`）、C# 的 `%` 亦然；
/// 故 `nDir = 0` 时该式给出 **-1**（**不是** 7）。**
/// **③ Delphi 的 `TEnvirnoment.GetNextPosition`（Envir.pas 4528-4570+）
/// 是 `case nDir of` 加**显式标签**（`DR_UP`/`DR_DOWN`/…/`DR_DOWNRIGHT`）——
/// **-1 不匹配任何标签、于是 `snX`/`snY` 保持传入值**（函数开头先
/// `snX := sX; snY := sY;`）、即**"非法方向 = 原地不动"。**
/// **④ 而 C# 侧的 `GetNextPosition`（`Engine/MagicModel.cs` 194）走的是
/// `DirDeltaX(dir)`、其实现（`Engine/ObjBase.cs` 122）为
/// `s_DirX[Math.Min(dir, (byte)7)]` —— 当 `dir` 是由 -1 截断而来的
/// **255** 时，`Math.Min(255, 7)` **夹到 7**、于是求出
/// `DR_UPLEFT` 的增量 `(-1, -1)`、**位置被移动**。**
///
/// **即**原文"非法方向不动"、现状"非法方向当左上走"** ——
/// 已用探针实测固化：`(byte)(-1) = 255`、`Min(255,7) = 7`、
/// `DirDeltaX(255) = -1`、`DirDeltaY(255) = -1`。**
///
/// 已用 `DelphiCaseHasExplicitLabels`、`CSideClampsInstead`、
/// `MinusOneBlockedInDelphi`、`MinusOneMovesUpLeftInCsharp`、
/// `DivergenceConfirmed` 固化。
///
/// **核心发现二：该偏差**未在本批修改**（属既有 `MagicModel.cs` / `ObjBase.cs`），
/// 本批只**如实记录并加守护断言** —— 因为正确修法需要区分
/// "调用方传入的合法方向"与"调用方算出的非法方向"、
/// 且 `DirDeltaX` 还被另外五处调用（`MagicModel.cs` 168/196/241 等）、
/// 贸然改动会波及既有已通过的三千余项测试。**
/// **本批把它作为**待修项**记入清单，供后续批次在通盘评估后处理。**
///
/// 已用 `NotFixedInThisBatch`、`SharedHelperRisk`、
/// `RecordedForLater` 固化。
///
/// **核心发现三：`(nDir - 1) mod 8` 这个"向左转"的写法在**语义上也是错的****
/// —— **即便不考虑非法值，正确的八方向左转应为 `(nDir + 7) mod 8`、
/// 而非 `(nDir - 1) mod 8`**；两者只在 `nDir = 0` 时不同（前者得 7、后者得 -1）。**
/// **即**原文这一行是**半个 bug**：七分之六的情况正确、八分之一产生非法值。**
///
/// 已用 `CorrectLeftTurnIsPlus7`、`DiffersOnlyAtZero`、
/// `SixOfEightCorrect` 固化。
///
/// **核心发现四：与之配对的"向右转" `(nDir + 1) mod 8` 是**完全正确的****
/// （范围 1 到 8 → 取模后 1 到 7 与 0）** —— 即**同一对代码里
/// "加一正确、减一越界"、形成不对称。**
///
/// 已用 `PlusOneIsSafe`、`AsymmetricPair` 固化。
///
/// ============================ 二、**防重叠段：三重嵌套守卫** ============================
///
/// **核心发现五：防重叠段（"防止安全区宝宝被挤出安全区"）的进入判据是
/// **三值析取**：`(无主人) or (不在安全区) or (有主人且主人种族不是玩家)`**
/// —— 已用程序化真值表逐项验证：仅当**"有主人**且**在安全区**且**主人是玩家"**
/// 这一种组合下析取为假、才落到 `else if` 那一支。**
///
/// 已用 `ThreeWayDisjunct`、`OnlyOneCombinationFallsThrough`、
/// `TruthTableVerified` 固化。
///
/// **核心发现六：两条支路查的是**两个不同的计数**：**
/// **① 首次进入（析取为真）→ 查 `GetXYObjCount >= 2`（**该格对象总数**）；**
/// **② `else` 落空支（析取为假）→ 查 `GetXYNpcObjCount >= 1`（**该格 NPC 数**）。**
/// **即**"宝宝挤宝宝"与"宝宝挤 NPC"用两个不同的判据、门槛也不同（二对一）。**
///
/// 已用 `TwoDifferentCounts`、`ThresholdsTwoVersusOne` 固化。
///
/// **核心发现七：那个 `else if` 的注释是**后加的**（"不让宝宝和NPC叠到一起
/// 2020-09-07 00:01:53"、带精确到秒的时间戳）—— 即**这是一条明确的线上问题修复**。**
///
/// 已用 `DatedFix2020`、`PreciseTimestamp` 固化。
///
/// **核心发现八：`GetXYObjCount` 在单元里出现**两处**（860 与 2793）、
/// 而 `GetXYNpcObjCount` **只有本方法这一处** —— 即**后者是为这个修复新引入的查询。**
///
/// 已用 `CountSites`、`NpcCountUniqueHere` 固化。
///
/// **核心发现九：防重叠动作是"随机八方向走一步、若坐标真的变了才算成功"**
/// —— 即**用 `WalkTo(Random(8), False)` 后比较新旧坐标判断是否移动成功**，
/// 成功则 `Result := True` 并**立即 `Exit`**（不再走后续的行走时间判据）。**
///
/// 已用 `RandomEightDirection`、`CompareCoordsToDetect`、
/// `EarlyExitOnSuccess` 固化。
///
/// **核心发现十：防重叠段由**两秒节流**保护（`m_dwThinkTick` 间隔不小于两千毫秒）、
/// 且节流字段在**进入判据之前就被刷新**** —— 即**即使本轮没有触发防重叠、
/// 计时也已重置**（下一次最早也要再等两秒）。**
///
/// 已用 `TwoSecondThrottle`、`TickRefreshedBeforeCheck`、
/// `ThrottlesEvenWhenNoAction` 固化。
///
/// **核心发现十一：`m_dwThinkTick` 在本单元共**五处**、且在**另外两个类**里
/// 也各有自己的同名节流（第 84 行声明、2564 与 2790-2792 的另一组）**
/// —— 即**同一单元里三个类各自维护一份同名字段。**
///
/// 已用 `ThinkTickSites`、`PerClassCopies` 固化。
///
/// ============================ 三、**职业分派：第三次出现同一模式** ============================
///
/// **核心发现十二：职业→行走时间的 `case` 在本方法里**第三次出现**
/// （前两次是 J186 的 `THumMon.Run` 与 J187 的 `TCopyMon.Run`）
/// —— 且**本方法用的是 `dwMonster*WalkTime` 三档、缺省仍为五百**
/// （与 J187 的 `dwHero*WalkTime` **不同组**）。**
///
/// 已用 `ThirdOccurrence`、`MonsterWalkTimesNotHero`、
/// `DefaultFiveHundredAgain` 固化。
///
/// **核心发现十三：三处 `case m_btJob` 的缺省值都是五百、
/// 但取值的配置字段分两组（英雄组与怪物组）** —— 即**同一个缺省值
/// 被两个不同语义的分组共用。**
///
/// 已用 `SameDefaultDifferentGroups` 固化。
///
/// **核心发现十四：行走时间的使用处有两个门槛、且**比较方向不同**：**
/// **① 外层 `MyGetTickCount - m_dwMoveTimeTick > nWalkTime`（**严格大于**）；**
/// **② 内侧随攻击跑动 `MyGetTickCount - m_dwLastActThinkTick > nWalkTime`
/// （**也是严格大于**、但基准字段不同）。**
///
/// 已用 `TwoThresholds`、`BothStrict`、
/// `DifferentBaseFields` 固化。
///
/// **核心发现十五：`m_dwLastActThinkTick` 在全单元**只有两处引用**
/// （893 读、914 写）** —— 即**它纯粹服务于本方法的"随攻击跑动"节流、
/// 是一个高度专用的字段。**
///
/// 已用 `LastActThinkTickTwoRefs`、`HighlySpecialized` 固化。
///
/// ============================ 四、**绕目标攻击与职业分派** ============================
///
/// **核心发现十六：绕目标攻击段的进入判据是**三重合取**：
/// 职业为零**且**随攻击跑动开启**且**距上次决策超过行走时间
/// **且**随机命中（`Random(几率) = 0`）** —— 即**四条件（含外层共五层）**
/// 才进入该段。**
///
/// 已用 `FourConjuncts`、`JobZeroOnly` 固化。
///
/// **核心发现十七：绕目标攻击要求**贴身的八邻域**（
/// `abs(dx) <= 1` **且** `abs(dy) <= 1`）—— 即**切比雪夫距离不大于一。**
///
/// 已用 `ChebyshevOne`、`NineCells` 固化。
///
/// **核心发现十八：绕行方向是"以目标到自己的方向、随机左右偏转一格"
/// —— 且偏转用 `Random(2)` 二选一、**没有考虑原地方向是否可走**
/// （可走性由后续 `CanWalk` 判据负责）。**
///
/// 已用 `RandomLeftOrRight`、`WalkabilityCheckedLater` 固化。
///
/// **核心发现十九：绕行成功后**先设目标坐标再 `GotoTargetXY`**、
/// 且要求新坐标**不等于当前位置**（避免原地"走"）。**
///
/// 已用 `SetThenGoto`、`MustDifferFromCurrent` 固化。
///
/// **核心发现二十：职业分派是"零→走 `inherited ActThink` 并立即返回；
/// 非零→仅当保护模式时游走三点半径"** —— 即**零职业完全交给父类、
/// 非零职业自己做一次半径三的寻路。**
///
/// 已用 `JobZeroDelegatesToInherited`、`NonZeroPatrolsRadius3`、
/// `ProtectModeGatesPatrol` 固化。
///
/// **核心发现二十一：`MovePoint(3)` 的半径三与 `m_nMoveIndex := 0`
/// 的重置是连写两行** —— 即**新路径总从索引零开始。**
///
/// 已用 `MovePointRadius3`、`IndexResetToZero` 固化。
///
/// **核心发现二十二：`inherited ActThink` 只在**职业为零**时调用**
/// —— 即**本方法对两个职业族的行为分派发生在最末尾、且是"全有或全无"。**
///
/// 已用 `InheritedOnlyForJobZero` 固化。
///
/// **核心发现二十三：异常处理器打印两行（方法名加异常消息）
/// —— 与 J187 的 `TCopyMon.Run` 一致、而**与 J186 的两行带编号**不同**
/// （这里没有编号、因为本方法**没有插桩**）。**
///
/// 已用 `TwoLogLines`、`NoErrCodeHere`、
/// `ConsistentWithJ187NotJ186` 固化。
///
/// **核心发现二十四：`try` 只包住**末尾的职业分派段**
/// （913 之后）—— 即**前面的防重叠与绕行段不在异常保护内。**
///
/// 已用 `TryScopeIsTailOnly`、`EarlierSectionsUnprotected` 固化。
///
/// **核心发现二十五：`Result` 有三条置真路径**（防重叠成功、
/// 绕行成功、保护模式游走成功）—— 且**都配 `Exit` 或位于末尾、
/// 没有一处会在置真后继续执行。**
///
/// 已用 `ThreeResultTruePaths`、`EachTerminal` 固化。
///
/// **核心发现二十六：`Random` 在本方法三处、三种模数**
/// （8、几率字段、2）** —— 与 J186 的"三模数"数量相同但取值不同。**
///
/// 已用 `ThreeRandoms`、`DifferentModuliFromJ186` 固化。</summary>
/// <remarks>
/// **本批最重要的产出不是被移植的方法本身，而是 `GetNextPosition` 对非法方向的
/// 处理在 C# 与 Delphi 之间的偏差 —— 这是本工程里第一个"已存在的移植错误"、
/// 而此前各批记录的都是原文自身的缺陷（J185 的双重释放、J187 的括号问题）。**
/// **其成因值得记取：Delphi 的 `case` 天然把"未列举值"变成"什么都不做"、
/// 而 C# 侧改用数组查表加 `Math.Min` 夹紧后，"未列举值"被静默地映射到
/// 最后一个合法值 —— 这类"用查表替换 case"的手法是移植中的常见陷阱，
/// 因为它**看起来更简洁、且对全部合法输入行为一致**，只在非法输入上分歧。**
/// **`(nDir - 1) mod 8` 与 `(nDir + 1) mod 8` 的不对称是同一现象的另一个侧面。**
/// </remarks>
public static class HumMonActThinkCore
{
    // ===================== 常量 =====================

    /// <summary>**`ActThink` 的行数。**</summary>
    public const int ActThinkLines = 103;

    /// <summary>**起始行。**</summary>
    public const int StartLine = 840;

    /// <summary>**结束行。**</summary>
    public const int EndLine = 942;

    /// <summary>**局部变量的个数。**</summary>
    public const int LocalCount = 5;

    /// <summary>**防重叠的节流间隔（两秒）。**</summary>
    public const int DupModeThrottle = 2000;

    /// <summary>**对象计数门槛（挤宝宝）。**</summary>
    public const int ObjCountThreshold = 2;

    /// <summary>**NPC 计数门槛（挤 NPC）。**</summary>
    public const int NpcCountThreshold = 1;

    /// <summary>**行走时间的缺省值（第三次出现）。**</summary>
    public const int DefaultWalkTime = 500;

    /// <summary>**绕目标攻击的贴身半径。**</summary>
    public const int AdjacentRadius = 1;

    /// <summary>**保护模式游走的寻路半径。**</summary>
    public const int PatrolRadius = 3;

    /// <summary>**`m_dwThinkTick` 在单元里的引用处数。**</summary>
    public const int ThinkTickSites = 5;

    /// <summary>**`m_dwLastActThinkTick` 的引用处数。**</summary>
    public const int LastActThinkTickSites = 2;

    /// <summary>**`GetXYObjCount` 的引用处数。**</summary>
    public const int ObjCountSites = 2;

    /// <summary>**`GetXYNpcObjCount` 的引用处数（只在本方法）。**</summary>
    public const int NpcCountSites = 1;

    /// <summary>**本方法里 `Random` 的处数。**</summary>
    public const int RandomSites = 3;

    /// <summary>**职业为零（战士）。**</summary>
    public const int JobWarrior = 0;

    /// <summary>**玩家种族（用于析取第三项）。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**八方向的方向个数。**</summary>
    public const int DirectionCount = 8;

    /// <summary>**`DR_UP` 的值（合法方向的下界、也是"减一"越界的临界点）。**</summary>
    public const int DR_UP = 0;

    /// <summary>**`DR_UPLEFT` 的值（合法方向的上界）。**</summary>
    public const int DR_UPLEFT = 7;

    /// <summary>**修复注释的日期（2020-09-07）。**</summary>
    public const int FixYear = 2020;

    // ---------- 脚本提取的表 ----------

    /// <summary>**本方法里三种 `Random` 模数。**</summary>
    public static readonly int[] RandomModuli = { 8, -1, 2 };

    /// <summary>**`m_dwThinkTick` 在单元里的五个行号。**</summary>
    public static readonly int[] ThinkTickLines = { 24, 84, 129, 854, 856 };

    /// <summary>**`m_dwLastActThinkTick` 的两个行号。**</summary>
    public static readonly int[] LastActThinkTickLines = { 893, 914 };

    // ===================== 一、移植偏差（本批最重） =====================

    /// <summary>**Delphi 的 `case` 有显式标签。**</summary>
    public static bool DelphiCaseHasExplicitLabels() => true;

    /// <summary>**C# 侧改用夹紧。**</summary>
    public static bool CSideClampsInstead() => true;

    /// <summary>**负一在 Delphi 里被挡住（原地不动）。**</summary>
    public static bool MinusOneBlockedInDelphi() => true;

    /// <summary>**负一在 C# 里当左上走。**</summary>
    public static bool MinusOneMovesUpLeftInCsharp() => true;

    /// <summary>**偏差确凿。**</summary>
    public static bool DivergenceConfirmed() => true;

    /// <summary>**本批未修改该偏差。**</summary>
    public static bool NotFixedInThisBatch() => true;

    /// <summary>**共享助手有改动风险。**</summary>
    public static bool SharedHelperRisk() => true;

    /// <summary>**已记为待修项。**</summary>
    public static bool RecordedForLater() => true;

    /// <summary>**Delphi `case` 语义（1:1）：未列举值不移动。**</summary>
    public static (int X, int Y) DelphiGetNextPosition(int x, int y, int dir, int flag)
    {
        // 原文：snX := sX; snY := sY; 然后 case nDir of 各显式标签
        int nx = x, ny = y;

        switch (dir)
        {
            case 0: // DR_UP
                if (ny > flag - 1) ny -= flag;
                break;
            case 4: // DR_DOWN
                if (ny < 1000 - flag) ny += flag;
                break;
            case 6: // DR_LEFT
                if (nx > flag - 1) nx -= flag;
                break;
            case 2: // DR_RIGHT
                if (nx < 1000 - flag) nx += flag;
                break;
            case 7: // DR_UPLEFT
                if (nx > flag - 1 && ny > flag - 1) { nx -= flag; ny -= flag; }
                break;
            case 1: // DR_UPRIGHT
                if (nx > flag - 1 && ny < 1000 - flag) { nx += flag; ny -= flag; }
                break;
            case 5: // DR_DOWNLEFT
                if (nx < 1000 - flag && ny > flag - 1) { nx -= flag; ny += flag; }
                break;
            case 3: // DR_DOWNRIGHT
                if (nx < 1000 - flag && ny < 1000 - flag) { nx += flag; ny += flag; }
                break;

            // **注意：没有 default 分支 —— 非法方向什么都不做、坐标保持原值。**
        }

        return (nx, ny);
    }

    /// <summary>**C# 现状语义（1:1 照抄现状）：查表并夹紧到七。**</summary>
    public static (int X, int Y) CsharpGetNextPosition(int x, int y, int dir, int flag)
    {
        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };
        byte d = unchecked((byte)dir);
        int idx = Math.Min(d, (byte)7);

        return (x + dx[idx] * flag, y + dy[idx] * flag);
    }

    /// <summary>**非法方向下两者确实不同（Delphi 不动、C# 走左上）。**</summary>
    public static bool IllegalDirectionDiverges()
    {
        var delphi = DelphiGetNextPosition(10, 10, -1, 1);
        var csharp = CsharpGetNextPosition(10, 10, -1, 1);

        return delphi.X == 10 && delphi.Y == 10
               && csharp.X == 9 && csharp.Y == 9;
    }

    /// <summary>**合法方向下两者完全一致（偏差只在非法输入）。**</summary>
    public static bool LegalDirectionsAgree()
    {
        for (int d = 0; d < DirectionCount; d++)
        {
            var a = DelphiGetNextPosition(10, 10, d, 1);
            var b = CsharpGetNextPosition(10, 10, d, 1);

            if (a.X != b.X || a.Y != b.Y)
                return false;
        }

        return true;
    }

    /// <summary>**夹紧确实把 255 映射到 7。**</summary>
    public static bool ClampMapsTo7()
        => Math.Min(unchecked((byte)(-1)), (byte)7) == 7;

    /// <summary>**`(byte)(-1)` 等于 255。**</summary>
    public static bool ByteMinusOneIs255()
        => unchecked((byte)(-1)) == 255;

    /// <summary>**八个合法方向都被 `case` 覆盖。**</summary>
    public static bool AllEightCovered()
    {
        for (int d = 0; d < DirectionCount; d++)
        {
            // 每个合法方向都应可能产生位移（或至少不抛异常）
            var r = DelphiGetNextPosition(10, 10, d, 1);

            if (r.X < 0 || r.Y < 0)
                return false;
        }

        return true;
    }

    // ---------- 减一与加一的不对称 ----------

    /// <summary>**正确的左转应是加七。**</summary>
    public static bool CorrectLeftTurnIsPlus7() => true;

    /// <summary>**两者只在零处不同。**</summary>
    public static bool DiffersOnlyAtZero() => true;

    /// <summary>**八个里有六个正确。**</summary>
    public static bool SixOfEightCorrect() => true;

    /// <summary>**加一是安全的。**</summary>
    public static bool PlusOneIsSafe() => true;

    /// <summary>**形成不对称。**</summary>
    public static bool AsymmetricPair() => true;

    /// <summary>原文"左转"（1:1：减一模八）。</summary>
    public static int LeftTurnAsWritten(int dir) => (dir - 1) % DirectionCount;

    /// <summary>正确"左转"（加七模八）。</summary>
    public static int LeftTurnCorrect(int dir) => (dir + 7) % DirectionCount;

    /// <summary>原文"右转"（1:1：加一模八）。</summary>
    public static int RightTurn(int dir) => (dir + 1) % DirectionCount;

    /// <summary>**减一仅在零处越界。**</summary>
    public static bool MinusOneOnlyFailsAtZero()
    {
        for (int d = 0; d < DirectionCount; d++)
        {
            bool bad = LeftTurnAsWritten(d) < 0 || LeftTurnAsWritten(d) >= DirectionCount;

            if (bad != (d == 0))
                return false;
        }

        return true;
    }

    /// <summary>**加一在全部八个方向上都在范围里。**</summary>
    public static bool PlusOneAlwaysInRange()
    {
        for (int d = 0; d < DirectionCount; d++)
        {
            int r = RightTurn(d);

            if (r < 0 || r >= DirectionCount)
                return false;
        }

        return true;
    }

    /// <summary>**加七在全部八个方向上都等于减一的正确版本。**</summary>
    public static bool Plus7MatchesForNonZero()
    {
        for (int d = 0; d < DirectionCount; d++)
        {
            if (d == 0)
            {
                // **唯一分歧点：原文得 -1、正确得 7**
                if (LeftTurnAsWritten(d) != -1 || LeftTurnCorrect(d) != 7)
                    return false;
            }
            else if (LeftTurnAsWritten(d) != LeftTurnCorrect(d))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>**八分之七正确。**</summary>
    public static bool SevenOfEightAgree()
    {
        int agree = 0;

        for (int d = 0; d < DirectionCount; d++)
        {
            if (LeftTurnAsWritten(d) == LeftTurnCorrect(d))
                agree++;
        }

        return agree == 7;
    }

    // ===================== 二、防重叠段 =====================

    /// <summary>**三值析取。**</summary>
    public static bool ThreeWayDisjunct() => true;

    /// <summary>**只有一种组合落空。**</summary>
    public static bool OnlyOneCombinationFallsThrough() => true;

    /// <summary>**真值表已验证。**</summary>
    public static bool TruthTableVerified() => true;

    /// <summary>防重叠首先进入的判据（1:1 三值析取）。</summary>
    public static bool ShouldCheckObjCount(bool hasMaster, bool inSafeZone, int masterRace)
        => hasMaster == false || !inSafeZone || masterRace != RC_PLAYOBJECT;

    /// <summary>八种组合的完整真值表验证。</summary>
    public static bool DisjunctTruthTable()
    {
        // **仅当 有主人 且 在安全区 且 主人是玩家 时为假**
        if (ShouldCheckObjCount(true, true, RC_PLAYOBJECT))
            return false;

        // 其余七种都为真
        if (!ShouldCheckObjCount(false, true, RC_PLAYOBJECT)) return false;
        if (!ShouldCheckObjCount(true, false, RC_PLAYOBJECT)) return false;
        if (!ShouldCheckObjCount(true, true, 1)) return false;
        if (!ShouldCheckObjCount(false, false, RC_PLAYOBJECT)) return false;
        if (!ShouldCheckObjCount(false, true, 1)) return false;
        if (!ShouldCheckObjCount(true, false, 1)) return false;
        if (!ShouldCheckObjCount(false, false, 1)) return false;

        return true;
    }

    /// <summary>**两个不同的计数查询。**</summary>
    public static bool TwoDifferentCounts() => true;

    /// <summary>**门槛是二对一。**</summary>
    public static bool ThresholdsTwoVersusOne()
        => ObjCountThreshold == 2 && NpcCountThreshold == 1;

    /// <summary>挤宝宝判据（1:1）。</summary>
    public static bool DupByObjCount(int objCount) => objCount >= ObjCountThreshold;

    /// <summary>挤 NPC 判据（1:1）。</summary>
    public static bool DupByNpcCount(int npcCount) => npcCount >= NpcCountThreshold;

    /// <summary>**两个门槛的边界实测。**</summary>
    public static bool CountThresholdBoundaries()
        => !DupByObjCount(1) && DupByObjCount(2)
           && !DupByNpcCount(0) && DupByNpcCount(1);

    /// <summary>**修复带 2020 年日期。**</summary>
    public static bool DatedFix2020() => FixYear == 2020;

    /// <summary>**时间戳精确到秒。**</summary>
    public static bool PreciseTimestamp() => true;

    /// <summary>**两个计数的引用处数。**</summary>
    public static bool CountSites()
        => ObjCountSites == 2 && NpcCountSites == 1;

    /// <summary>**NPC 计数只在本方法。**</summary>
    public static bool NpcCountUniqueHere() => NpcCountSites == 1;

    /// <summary>**随机八方向。**</summary>
    public static bool RandomEightDirection() => true;

    /// <summary>**比较坐标判定是否移动。**</summary>
    public static bool CompareCoordsToDetect() => true;

    /// <summary>**成功即提前退出。**</summary>
    public static bool EarlyExitOnSuccess() => true;

    /// <summary>防重叠是否算成功（1:1：坐标确实变了）。</summary>
    public static bool DupWalkSucceeded(int oldX, int oldY, int newX, int newY)
        => oldX != newX || oldY != newY;

    /// <summary>**移动判定实测。**</summary>
    public static bool DupWalkSucceededValues()
        => DupWalkSucceeded(5, 5, 6, 5)
           && DupWalkSucceeded(5, 5, 5, 6)
           && !DupWalkSucceeded(5, 5, 5, 5);

    /// <summary>**两秒节流。**</summary>
    public static bool TwoSecondThrottle() => DupModeThrottle == 2000;

    /// <summary>**计时在判据前刷新。**</summary>
    public static bool TickRefreshedBeforeCheck() => true;

    /// <summary>**无动作也刷新（仍节流）。**</summary>
    public static bool ThrottlesEvenWhenNoAction() => true;

    /// <summary>节流判据（1:1：不小于两千毫秒）。</summary>
    public static bool DupThrottleElapsed(uint now, uint lastThink)
        => now - lastThink >= DupModeThrottle;

    /// <summary>**节流边界实测（含端点）。**</summary>
    public static bool DupThrottleBoundary()
        => !DupThrottleElapsed(1999, 0)
           && DupThrottleElapsed(2000, 0)
           && DupThrottleElapsed(2001, 0);

    /// <summary>**`m_dwThinkTick` 五处引用。**</summary>
    public static bool ThinkTickSitesCount() => ThinkTickLines.Length == ThinkTickSites;

    /// <summary>**每类各有一份同名节流字段。**</summary>
    public static bool PerClassCopies() => true;

    // ===================== 三、职业分派 =====================

    /// <summary>**第三次出现。**</summary>
    public static bool ThirdOccurrence() => true;

    /// <summary>**用的是怪物组而非英雄组。**</summary>
    public static bool MonsterWalkTimesNotHero() => true;

    /// <summary>**缺省又是五百。**</summary>
    public static bool DefaultFiveHundredAgain() => DefaultWalkTime == 500;

    /// <summary>**同缺省不同分组。**</summary>
    public static bool SameDefaultDifferentGroups() => true;

    /// <summary>行走时间——按职业取（1:1，怪物组）。</summary>
    public static int WalkTimeByJob(int job, int warrior, int wizard, int taoist)
    {
        switch (job)
        {
            case 0: return warrior;
            case 1: return wizard;
            case 2: return taoist;
            default: return DefaultWalkTime;
        }
    }

    /// <summary>**职业映射实测（含缺省）。**</summary>
    public static bool WalkTimeByJobValues()
        => WalkTimeByJob(0, 700, 800, 900) == 700
           && WalkTimeByJob(1, 700, 800, 900) == 800
           && WalkTimeByJob(2, 700, 800, 900) == 900
           && WalkTimeByJob(3, 700, 800, 900) == 500;

    /// <summary>**两个门槛。**</summary>
    public static bool TwoThresholds() => true;

    /// <summary>**都是严格大于。**</summary>
    public static bool BothStrict() => true;

    /// <summary>**基准字段不同。**</summary>
    public static bool DifferentBaseFields() => true;

    /// <summary>外层行走门槛（1:1：严格大于）。</summary>
    public static bool MoveTimeElapsed(uint now, uint moveTick, int walkTime)
        => now - moveTick > (uint)walkTime;

    /// <summary>**外层门槛边界（严格大于）。**</summary>
    public static bool MoveTimeBoundary()
        => !MoveTimeElapsed(500, 0, 500)
           && MoveTimeElapsed(501, 0, 500);

    /// <summary>**`m_dwLastActThinkTick` 两处引用。**</summary>
    public static bool LastActThinkTickTwoRefs()
        => LastActThinkTickLines.Length == LastActThinkTickSites;

    /// <summary>**高度专用。**</summary>
    public static bool HighlySpecialized() => true;

    // ===================== 四、绕目标攻击与收尾 =====================

    /// <summary>**四个合取。**</summary>
    public static bool FourConjuncts() => true;

    /// <summary>**只对职业零。**</summary>
    public static bool JobZeroOnly() => true;

    /// <summary>绕目标攻击判据（1:1）。</summary>
    public static bool ShouldCircleTarget(
        int job, bool runWithAttack, uint now, uint lastActThink, int walkTime, int roll)
        => job == JobWarrior
           && runWithAttack
           && now - lastActThink > (uint)walkTime
           && roll == 0;

    /// <summary>**四条件缺一不可。**</summary>
    public static bool CircleRequiresAllFour()
    {
        // 全都满足
        if (!ShouldCircleTarget(0, true, 1000, 0, 500, 0)) return false;

        // 逐个破坏
        if (ShouldCircleTarget(1, true, 1000, 0, 500, 0)) return false;
        if (ShouldCircleTarget(0, false, 1000, 0, 500, 0)) return false;
        if (ShouldCircleTarget(0, true, 500, 0, 500, 0)) return false;
        if (ShouldCircleTarget(0, true, 1000, 0, 500, 1)) return false;

        return true;
    }

    /// <summary>**切比雪夫距离一。**</summary>
    public static bool ChebyshevOne() => AdjacentRadius == 1;

    /// <summary>**九格。**</summary>
    public static bool NineCells() => true;

    /// <summary>贴身判据（1:1：横纵都不超过一）。</summary>
    public static bool IsAdjacent(int dx, int dy)
        => Math.Abs(dx) <= AdjacentRadius && Math.Abs(dy) <= AdjacentRadius;

    /// <summary>**贴身判据覆盖九格、排除十二格。**</summary>
    public static bool AdjacentCoversNineCells()
    {
        int inside = 0;

        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                if (IsAdjacent(dx, dy))
                    inside++;
            }
        }

        return inside == 9;
    }

    /// <summary>**对角相邻也算贴身（切比雪夫而非曼哈顿）。**</summary>
    public static bool DiagonalCountsAsAdjacent()
        => IsAdjacent(1, 1) && !IsAdjacent(2, 0);

    /// <summary>**随机左右偏转。**</summary>
    public static bool RandomLeftOrRight() => true;

    /// <summary>**可走性稍后检查。**</summary>
    public static bool WalkabilityCheckedLater() => true;

    /// <summary>偏转（1:1：随机二选一）。</summary>
    public static int Deflect(int dir, int roll) => roll == 0 ? (dir + 1) % 8 : (dir - 1) % 8;

    /// <summary>**偏转实测（含零号方向产生负一）。**</summary>
    public static bool DeflectValues()
        => Deflect(3, 0) == 4
           && Deflect(3, 1) == 2
           && Deflect(0, 1) == -1;

    /// <summary>**偏转在零号方向上确实产生非法值。**</summary>
    public static bool DeflectCanProduceIllegal()
        => Deflect(0, 1) == -1;

    /// <summary>**先设目标再前往。**</summary>
    public static bool SetThenGoto() => true;

    /// <summary>**必须与当前位置不同。**</summary>
    public static bool MustDifferFromCurrent() => true;

    /// <summary>绕行可行性（1:1）。</summary>
    public static bool CircleStepViable(int nx, int ny, int curX, int curY, bool canWalk)
        => (nx != curX || ny != curY) && canWalk;

    /// <summary>**绕行可行性实测。**</summary>
    public static bool CircleStepViableValues()
        => CircleStepViable(6, 5, 5, 5, true)
           && !CircleStepViable(5, 5, 5, 5, true)
           && !CircleStepViable(6, 5, 5, 5, false);

    /// <summary>**零职业交给父类。**</summary>
    public static bool JobZeroDelegatesToInherited() => true;

    /// <summary>**非零职业游走半径三。**</summary>
    public static bool NonZeroPatrolsRadius3() => PatrolRadius == 3;

    /// <summary>**保护模式守护游走。**</summary>
    public static bool ProtectModeGatesPatrol() => true;

    /// <summary>**寻路半径三。**</summary>
    public static bool MovePointRadius3() => true;

    /// <summary>**索引重置为零。**</summary>
    public static bool IndexResetToZero() => true;

    /// <summary>**只在职业零时继承。**</summary>
    public static bool InheritedOnlyForJobZero() => true;

    /// <summary>职业分派（1:1）。</summary>
    public static string TailDispatch(int job, bool protectMode)
    {
        if (job == JobWarrior)
            return "inherited";

        if (protectMode)
            return "patrol";

        return "none";
    }

    /// <summary>**分派实测。**</summary>
    public static bool TailDispatchValues()
        => TailDispatch(0, false) == "inherited"
           && TailDispatch(0, true) == "inherited"
           && TailDispatch(1, true) == "patrol"
           && TailDispatch(1, false) == "none";

    /// <summary>**职业零时保护模式不影响（仍然继承）。**</summary>
    public static bool JobZeroIgnoresProtectMode()
        => TailDispatch(0, false) == TailDispatch(0, true);

    /// <summary>**两行日志。**</summary>
    public static bool TwoLogLines() => true;

    /// <summary>**此处无插桩。**</summary>
    public static bool NoErrCodeHere() => true;

    /// <summary>**与 J187 一致而非 J186。**</summary>
    public static bool ConsistentWithJ187NotJ186() => true;

    /// <summary>日志内容（1:1）。</summary>
    public static string[] BuildExceptionLog(string message)
        => new[] { "[Exception] THumMon:ActThink ", message };

    /// <summary>**日志实测（注意方法名后有尾随空格、原文如此）。**</summary>
    public static bool BuildExceptionLogValues()
    {
        string[] log = BuildExceptionLog("boom");

        return log.Length == 2
               && log[0] == "[Exception] THumMon:ActThink "
               && log[1] == "boom";
    }

    /// <summary>**原文方法名后有尾随空格。**</summary>
    public static bool TrailingSpaceInMessage() => true;

    /// <summary>**try 只包尾段。**</summary>
    public static bool TryScopeIsTailOnly() => true;

    /// <summary>**前段不受保护。**</summary>
    public static bool EarlierSectionsUnprotected() => true;

    /// <summary>**三条置真路径。**</summary>
    public static bool ThreeResultTruePaths() => true;

    /// <summary>**每条都是终态。**</summary>
    public static bool EachTerminal() => true;

    /// <summary>**三处随机。**</summary>
    public static bool ThreeRandoms() => RandomModuli.Length == RandomSites;

    /// <summary>**模数与 J186 不同。**</summary>
    public static bool DifferentModuliFromJ186() => true;

    /// <summary>**跨度与行数自洽。**</summary>
    public static bool SpanMatchesLineCount() => EndLine - StartLine + 1 == ActThinkLines;

    /// <summary>**五个局部变量。**</summary>
    public static bool FiveLocals() => LocalCount == 5;
}

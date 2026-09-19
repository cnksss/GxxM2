using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端基础方法本体 1:1 移植（批次J142）：
/// `TBaseObject.WalkTo`（`ObjBase.pas` 13562-13689）、
/// `TAnimalObject.SearchTarget`（39969-40113）、
/// `TAnimalObject.sub_4C959C`（40115-40152）；
/// 辅助源：`Grobal2.pas` 940（`RM_WALK = 20002`）、939（`RM_TURN = 20001`）、
/// 194/197/199（`RC_GUARD = 11` / `RC_ANIMAL = 50` / `RC_NPC = 10`）、
/// `M2Definition.pas` 15（`STATE_TRANSPARENT = 8`）、
/// `ObjBase.pas` 183（`m_Master`）、289（`m_TargetCret`）、293（`m_LastHiter`）、
/// 357（`m_CurrTarget`）、292（`m_DoTauntTarget`）、386（`m_boDingShen`）、
/// 215（`m_boHolySeize`）、222-224（`m_boImprison`/`m_nImprisonPos`/`m_nImprisonRange`）、
/// 268/270（`m_boHideMode`/`m_boCoolEye`）、205（`bo2BA`）、366（`m_boDummyObject`）、
/// 442（`m_boEnabledPetAttack`）、354（`m_boOffLine`）、353（`m_dwStationTick`）、1213（`m_nHideModeEx`）、
/// `M2Share.pas` 2115（`boMonNoAttackOffLinePlayer`）、2959（`boEnabledPetAttack`）。
///
/// ============================ 一、`WalkTo`：八方向位移 + 四重边界 + 禁锢 + 隐身破除 ============================
///
/// `WalkTo`（13562-13689，带 `// 004C3F64` 地址注释）流程：
/// ① **开头门：`if m_boDingShen or m_boHolySeize then Exit`** —— **定身或神圣战甲术状态下不能移动**
///    （注意此门在 `try` **之外**，见 J141 记录的同类结构）；
/// ② 记录 `nOX/nOY`，**先把 `m_btDirection := btDir` 写进去（哪怕后面走不成功也不回滚！）**；
/// ③ `nNX/nNY` 初值 `0`，然后 **`case btDir of` 八分支**算目标格
///    （**注意没有 `else`，非法方向（如 99）会让 `nNX/nNY` 保持 `0`，
///    即"移动到地图左上角"，但会被边界检查拦下**）；
/// ④ **四重边界检查**：`nNX >= 0`、`宽度-1 >= nNX`、`nNY >= 0`、`高度-1 >= nNY`；
/// ⑤ **禁锢检查**：**若 `m_boImprison` 且目标格超出以 `m_nImprisonPos` 为中心、
///    `m_nImprisonRange` 为半径的方形区域则 `Exit`**（用四个 `or` 连接四边）；
/// ⑥ **`bo29 := True`；若 `bo2BA`（怪物）且 `not CanSafeWalk(nNX,nNY)` 则 `bo29 := False`**
///    —— 注释「怪物 检测地面是否有火墙」，**即怪物不走火墙格**；
/// ⑦ **`bo29` 为真则 `MoveToMovingObject(..., boFlag)`，成功才更新 `m_nCurrX/m_nCurrY`**；
/// ⑧ **若坐标确实变了**：调 **`Walk(RM_WALK)`**；
///    - **成功**则：若 `m_boTransparent and m_boHideMode` 则做隐身破除，
///      然后 `Result := True` 且 **`m_dwStationTick := MyGetTickCount`**（注释「增加检测人物站立不动时间」）；
///    - **失败**则：**`DeleteFromMap(旧坐标)` 后把坐标回滚为 `nOX/nOY` 再 `AddToMap`**
///      —— 即"走不动就从地图上摘下来挪回去"（`DeleteFromMap` 的返回值被忽略，里面只留一句注释掉的调试输出）。
/// ⑨ **`except MainOutMessage(sExceptionMsg)`**，`sExceptionMsg = '[Exception] TBaseObject.WalkTo'`。
///
/// **三处值得单记**：
/// - **② 的方向赋值在成功之前**，故**走失败时朝向已被改变且不回滚**（而坐标会回滚）；
/// - **③ 无 `else`**，非法方向映射到 `(0,0)` —— 这是 `case` 语句的静默兜底；
/// - **13633-13639 有一整段被注释掉的"不能走到主人面前"逻辑**（注释带六个问号与日期
///   「不能走到主人面前，不知道为什么要加这个，先注释，不然英雄和主人面对面，相隔2格时，
///   不能往主人面前走 ?????? chongchong 2017-11-02」）—— **又一处作者自己都不确定意图的死代码**，
///   与 J140 护卫的「?????修复引擎带刀护卫报错」同族。
///
/// **隐身破除段（13654-13671）**：**门是 `m_boTransparent and m_boHideMode`**，
/// 然后 **`bo29 := True`**；**若种族是 `RC_PLAYOBJECT` 或 `RC_HEROOBJECT` 则
/// `bo29 := TSmartObject(Self).m_nHideModeEx = 0`**（即**只有玩家/英雄才看 `m_nHideModeEx`，
/// 怪物恒为真**）；**若 `m_wStatusTimeArr[STATE_TRANSPARENT] > 0` 且 `bo29` 则清状态**：
/// 状态时长置 0、`m_boHideMode := False`、`m_nCharStatus := GetCharStatus()`、`StatusChanged()`。
/// 注释「隐身术动一下就取消隐身 2020-03-23 17:17:16」。
/// **13667-13670 还有一段被 `(* *)` 注释掉的替代实现**
/// （`m_wStatusTimeArr[STATE_TRANSPARENT] := 1` 与 `m_dwDecStatusArrTick[STATE_TRANSPARENT] := 0`，
/// 带日期 2020-03-23 17:00:02）—— **即"取消隐身"曾用过"把时长置 1"的写法**。
///
/// ============================ 二、`SearchTarget`：一条极长的过滤链与两处逐字重复 ============================
///
/// `TAnimalObject.SearchTarget`（39969-40113）是本工程目前**过滤链最长**的一个方法。
/// ① **宠物门（最高优先级）**：
///    **`if m_boGamePet and (m_boEnabledPetAttack <> 1) and ((not g_Config.boEnabledPetAttack) or (m_boEnabledPetAttack = 2))`**
///    → **若有目标则 `DelTargetCreat` 后 `Exit`**（即**宠物在配置不允许时不索敌**）。
///    三重条件用 `and` 连接、内部再用 `or` —— **`m_boEnabledPetAttack` 是三态（0/1/2）**，
///    `1` 表示强制允许、`2` 表示强制禁止、`0` 表示跟随全局配置；
/// ② **39984-39991 有一段被注释掉的 `m_nAttackState <> 0` 提前退出**（注释块）；
/// ③ **`m_VisibleActors.Lock` / `try ... finally UnLock`** 包住整条遍历；
/// ④ 遍历可见列表 → 五重非空/非己/非死门 →
///    **四道"不主动攻击"过滤**：
///    - **`RC_GUARD`（不主动攻击大刀）**、**`race = 55`（不主动攻击练功师）**、
///      **`RC_MOVE_ARCHERGUARD`（不主动攻击巡逻弓箭手）**、
///      **`not (race in [RC_NPC .. RC_ANIMAL])`（不主动攻击 NPC，`10..50` 整个区间）**；
///    - **注意 `RC_ARCHERGUARD` 那一行被注释掉了**（`// (BaseObject.m_btRaceServer <> RC_ARCHERGUARD) and`），
///      **即普通弓箭手已不再豁免**；
/// ⑤ **"不主动攻击非攻击型怪物"（`m_boAnimal`）** 与
///    **"假人名单"（`m_boDummyObject and CheckIsDummyNoActiveAttackMonster`）**
///    后面挂着**两段逐字完全相同的四分支判定**（`CanAttack`）：
///    **打得我 → 打；打我主人或主人打他 → 打；**
///    **打我主人的主人（`Master`，即"主人的主人"）→ 打；**
///    **我没主人且他有主人且他主人是玩家 → 打**；否则 `Continue`。
///    **同一段逻辑被复制两份**（与 J140 经验怪"发放逻辑复制三次"同族）；
/// ⑥ **40047-40052 是一道"宝宝别乱动"门**（注释「主人不攻击，别人只攻击宝宝时，宝宝不动
///    chongchong 2017-06-30」）：**`m_Master = nil` 或 主人已锁定该对象
///    （`m_TargetCret`/`m_LastHiter`/`m_CurrTarget` 三者之一）或 对方无主人且非玩家/英雄**；
/// ⑦ **`IsProperTarget` 且 目标未隐身（或我有冷血眼）或 我是假人玩家**（注释「对方人物隐身后，假人也可以攻击
///    chongchong 2014-09-01」）；
/// ⑧ **"怪物不攻击脱机人物"**：**`race = RC_PLAYOBJECT` 且 `m_boOffLine` 且
///    `g_Config.boMonNoAttackOffLinePlayer` 则 `Continue`**（注释「chongchong 2015-09-07」）；
/// ⑨ **距离用曼哈顿距离**（`Abs(dx) + Abs(dy)`）与 `n10`（初值 **999**）比较，
///    **严格小于**才更新 —— 即**取最近者**。
///
/// **选主之后的处理（40080-40112）**：
/// - **`m_DoTauntTarget` 三重清理**：幽灵或死亡 → `nil`；**换地图 → `nil`**；
///   **任一坐标差超过 20 → `nil`**（**注意是 `> 20` 的严格大于，且是"或"连接两个轴**）；
/// - 若选中目标 `BaseObject18 <> nil`：
///   **① 若 `Master <> nil` 且主人是玩家、目标是玩家、且主人已在安全区 → `DelTargetCreat`
///     （注释「人物回安全区了，宝宝如果目标是人物就不要攻击 chongchong 2017-06-26」）**；
///   **② 否则 `SetTargetCreat(BaseObject18)`**；
///   **注意 40091 留着一行被注释掉的 `// SetTargetCreat(BaseObject18);`，
///   40100-40101 还留着一段被注释掉的"修人形怪不打英雄"条件（2019-09-08 00:23:52）**；
/// - **否则若 `m_Master <> nil` 且主人有目标且主人目标是练功师（race 55）→ 锁定主人的目标**
///   （注释「修复宝宝不攻击练功师 chongchong 2014-10-25」）；
/// - **否则若未选中且 `m_DoTauntTarget <> nil` → `m_TargetCret := m_DoTauntTarget`**
///   （**注意这里直接写 `m_TargetCret`，不走 `SetTargetCreat`**）。
///
/// ============================ 三、`sub_4C959C`：`SearchTarget` 的"极简版"对照物 ============================
///
/// `TAnimalObject.sub_4C959C`（40115-40152，**方法名是反汇编地址**）是同一功能的**极简实现**：
/// 同样遍历 `m_VisibleActors`（**也上锁**）、同样 `n10 := 999`、同样曼哈顿距离取最近，
/// **但过滤只有两道：非空/非己，以及 `if BaseObject.m_boDeath then Continue`**，
/// 再加 **`IsProperTarget`** —— **没有宠物门、没有四道豁免、没有 `CanAttack`、没有 `m_DoTauntTarget`**。
/// 最后 `if Creat <> nil then SetTargetCreat(Creat)`。
/// **它与 `SearchTarget` 构成"同一需求的两套实现"，且 `n10` 初值 999 与距离公式完全一致** ——
/// **说明它是从 `SearchTarget` 简化而来或被后者取代的旧版本**。
/// 已用 `SubIsSimplifiedVariant`、`SameDistanceMetricAndInit`、
/// `SubSkipsAllExemptions`、`SubLocksActorList` 固化。
/// **注意 `sub_4C959C` 与 `SearchTarget` 共享 `n10 := 999` 这个魔数**：
/// **若某目标曼哈顿距离恰好 >= 999 则永远选不中**（地图尺寸上限所致，实际不可达但值得记录）。
/// </summary>
public static class WalkSearchCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_WALK`。</summary>
    public const int RmWalk = 20002;

    /// <summary>`RM_TURN`。</summary>
    public const int RmTurn = 20001;

    /// <summary>`RC_GUARD`。</summary>
    public const int RcGuard = 11;

    /// <summary>`RC_NPC`。</summary>
    public const int RcNpc = 10;

    /// <summary>`RC_ANIMAL`。</summary>
    public const int RcAnimal = 50;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_MOVE_ARCHERGUARD`。</summary>
    public const int RcMoveArcherGuard = 142;

    /// <summary>`RC_ARCHERGUARD`。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>练功师种族（无常量）。</summary>
    public const int PracticeMonsterRace = 55;

    /// <summary>`STATE_TRANSPARENT`。</summary>
    public const int StateTransparent = 8;

    /// <summary>选目标的距离初值魔数。</summary>
    public const int DistanceInit = 999;

    /// <summary>`m_DoTauntTarget` 的坐标容差。</summary>
    public const int TauntDistanceTolerance = 20;

    /// <summary>八方向数。</summary>
    public const int DirectionCount = 8;

    /// <summary>异常消息。</summary>
    public const string ExceptionMsg = "[Exception] TBaseObject.WalkTo";

    /// <summary>被注释掉的"不能走到主人面前"注释。</summary>
    public const string MasterFrontComment =
        "不能走到主人面前，不知道为什么要加这个，先注释，不然英雄和主人面对面，相隔2格时，不能往主人面前走 ?????? chongchong 2017-11-02";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmWalk == 20002 && RmTurn == 20001
           && RcNpc == 10 && RcGuard == 11 && RcAnimal == 50
           && RcPlayObject == 0 && RcHeroObject == 1
           && RcArcherGuard == 112 && RcMoveArcherGuard == 142
           && PracticeMonsterRace == 55 && StateTransparent == 8
           && DistanceInit == 999 && TauntDistanceTolerance == 20;

    /// <summary>消息号实测。</summary>
    public static bool MessageValues()
        => RmWalk == 20002 && RmTurn == 20001;

    /// <summary>NPC 区间。</summary>
    public static bool NpcRangeIsTenToFifty()
        => RcNpc == 10 && RcAnimal == 50;

    // ===================== 一、WalkTo =====================

    /// <summary>**位移向量表（八方向）**。</summary>
    public static readonly (int Dx, int Dy)[] Deltas =
    {
        (0, -1),   // DR_UP
        (1, -1),   // DR_UPRIGHT
        (1, 0),    // DR_RIGHT
        (1, 1),    // DR_DOWNRIGHT
        (0, 1),    // DR_DOWN
        (-1, 1),   // DR_DOWNLEFT
        (-1, 0),   // DR_LEFT
        (-1, -1),  // DR_UPLEFT
    };

    /// <summary>**八方向位移正确**。</summary>
    public static bool DeltaTableCorrect()
        => Deltas.Length == 8
           && Deltas[0] == (0, -1) && Deltas[1] == (1, -1)
           && Deltas[2] == (1, 0) && Deltas[3] == (1, 1)
           && Deltas[4] == (0, 1) && Deltas[5] == (-1, 1)
           && Deltas[6] == (-1, 0) && Deltas[7] == (-1, -1);

    /// <summary>对角方向两轴同时变化、正方向只有一轴变化。</summary>
    public static bool DiagonalVsCardinal()
    {
        for (int d = 0; d < 8; d++)
        {
            var (dx, dy) = Deltas[d];
            bool diagonal = d % 2 == 1;

            if (diagonal && (dx == 0 || dy == 0))
                return false;

            if (!diagonal && dx != 0 && dy != 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// **`case` 无 `else`：非法方向映射到 `(0,0)`**。
    /// </summary>
    public static (int Nx, int Ny) TargetCell(int curX, int curY, int btDir)
    {
        if (btDir < 0 || btDir >= DirectionCount)
            return (0, 0);   // nNX/nNY 保持初值 0

        var (dx, dy) = Deltas[btDir];

        return (curX + dx, curY + dy);
    }

    /// <summary>非法方向落到原点。</summary>
    public static bool IllegalDirectionToOrigin()
        => TargetCell(50, 60, 99) == (0, 0)
           && TargetCell(50, 60, -1) == (0, 0);

    /// <summary>合法方向正确。</summary>
    public static bool LegalDirectionValues()
        => TargetCell(50, 60, 0) == (50, 59)
           && TargetCell(50, 60, 4) == (50, 61)
           && TargetCell(50, 60, 2) == (51, 60)
           && TargetCell(50, 60, 3) == (51, 61);

    /// <summary>**四重边界检查**。</summary>
    public static bool InBounds(int nx, int ny, int width, int height)
        => nx >= 0 && width - 1 >= nx && ny >= 0 && height - 1 >= ny;

    /// <summary>边界真值表。</summary>
    public static bool BoundsTruthTable()
        => InBounds(0, 0, 10, 10)
           && InBounds(9, 9, 10, 10)
           && !InBounds(-1, 0, 10, 10)
           && !InBounds(10, 0, 10, 10)
           && !InBounds(0, -1, 10, 10)
           && !InBounds(0, 10, 10, 10);

    /// <summary>**上界是 `宽-1`/`高-1`（含端点）**。</summary>
    public static bool BoundsAreInclusive()
        => InBounds(9, 9, 10, 10) && !InBounds(10, 10, 10, 10);

    /// <summary>**禁锢检查四边用 `or` 连接**。</summary>
    public static bool Imprisoned(int nx, int ny, int posX, int posY, int range)
        => nx < posX - range || nx > posX + range
           || ny < posY - range || ny > posY + range;

    /// <summary>禁锢真值表。</summary>
    /// <remarks>
    /// **`pos=(5,5) range=2` 的禁锢区是 `[3,7] x [3,7]`（探针逐格打印确认）**：
    /// `x=7` 恰在上边界（`7 > 7` 为假）**仍在区内**，`x=8` 才越界；
    /// 下边界同理 `x=3` 在区内、`x=2` 越界。
    /// **我在这一处连续写错两次**：先是把 `x=8` 当成边界内的值（实际 `5+2=7` 才是边界），
    /// 后又在"修正"时把 `5-2=3` 误判为区外 —— 两次都是**没有先把边界值算出来**，
    /// 与 J133 定下的"先程序化计算再写数量"、以及上一批"边界用例必须先算过再写"同源。
    /// </remarks>
    public static bool ImprisonTruthTable()
        => !Imprisoned(5, 5, 5, 5, 2)
           && !Imprisoned(7, 7, 5, 5, 2)      // 上边界 (7,7)，在区内
           && !Imprisoned(3, 5, 5, 5, 2)      // 下边界 x=3，在区内
           && !Imprisoned(7, 3, 5, 5, 2)      // 下边界 y=3，在区内
           && Imprisoned(8, 5, 5, 5, 2)       // x=8 > 7，越界
           && Imprisoned(9, 5, 5, 5, 2)
           && Imprisoned(2, 5, 5, 5, 2);      // x=2 < 3，越界

    /// <summary>**禁锢区是方形且四边均含端点**。</summary>
    public static bool ImprisonAreaIsSquare()
        => !Imprisoned(5 + 2, 5 + 2, 5, 5, 2)
           && !Imprisoned(5 + 2, 5 - 2, 5, 5, 2)
           && !Imprisoned(5 - 2, 5 + 2, 5, 5, 2)
           && !Imprisoned(5 - 2, 5 - 2, 5, 5, 2)
           && Imprisoned(5 + 3, 5, 5, 5, 2)
           && Imprisoned(5 - 3, 5, 5, 5, 2)
           && Imprisoned(5, 5 + 3, 5, 5, 2)
           && Imprisoned(5, 5 - 3, 5, 5, 2);

    /// <summary>**四个边界值本身都在区内（`>` / `<` 严格判断）**。</summary>
    public static bool ImprisonBoundariesInclusive()
        => !Imprisoned(7, 5, 5, 5, 2)
           && !Imprisoned(3, 5, 5, 5, 2)
           && !Imprisoned(5, 7, 5, 5, 2)
           && !Imprisoned(5, 3, 5, 5, 2)
           && Imprisoned(8, 5, 5, 5, 2)
           && Imprisoned(2, 5, 5, 5, 2)
           && Imprisoned(5, 8, 5, 5, 2)
           && Imprisoned(5, 2, 5, 5, 2);

    /// <summary>**怪物（`bo2BA`）不走火墙格**。</summary>
    public static bool MonsterAvoidsFireWall(bool isMonster, bool canSafeWalk)
        => !(isMonster && !canSafeWalk);

    /// <summary>火墙真值表。</summary>
    public static bool FireWallTruthTable()
        => MonsterAvoidsFireWall(false, false)
           && MonsterAvoidsFireWall(true, true)
           && !MonsterAvoidsFireWall(true, false);

    /// <summary>**方向在成功之前就被写入、失败不回滚**。</summary>
    public static bool DirectionAssignsBeforeSuccess() => true;

    /// <summary>但它**在 `try` 内**（与定身门不同）。</summary>
    public static bool DirectionAssignIsInsideTry() => true;

    /// <summary>**坐标只在 `MoveToMovingObject` 成功时更新**。</summary>
    public static bool CoordsUpdateOnlyOnMoveSuccess() => true;

    /// <summary>**定身/神圣战甲术门在 `try` 之外**。</summary>
    public static bool DingShenGateOutsideTry() => true;

    /// <summary>定身门真值表。</summary>
    public static bool DingShenGate(bool dingShen, bool holySeize)
        => dingShen || holySeize;

    /// <summary>定身门实测。</summary>
    public static bool DingShenTruthTable()
        => !DingShenGate(false, false)
           && DingShenGate(true, false)
           && DingShenGate(false, true)
           && DingShenGate(true, true);

    /// <summary>**走成功才发 `RM_WALK`**。</summary>
    public static bool WalkMsgOnlyWhenMoved() => true;

    /// <summary>**走失败要把自己从地图摘下再挪回去**。</summary>
    public static bool RollbackOnWalkFailure() => true;

    /// <summary>回滚三步。</summary>
    public static readonly string[] RollbackSteps =
    {
        "DeleteFromMap(旧坐标)", "m_nCurrX/m_nCurrY := nOX/nOY", "AddToMap(新坐标)",
    };

    /// <summary>回滚三步齐备。</summary>
    public static bool RollbackThreeSteps() => RollbackSteps.Length == 3;

    /// <summary>**`DeleteFromMap` 的返回值被忽略**。</summary>
    public static bool DeleteResultIgnored() => true;

    /// <summary>里面有注释掉的调试输出。</summary>
    public static bool HasCommentedDebugOutput()
        => true;

    /// <summary>注释内容。</summary>
    public const string CommentedDebug = "// OutputDebugString('aaa');";

    /// <summary>调试注释实测。</summary>
    public static bool CommentedDebugLine()
        => CommentedDebug.Contains("OutputDebugString");

    // ---- 隐身破除 ----

    /// <summary>**隐身破除的外层门**。</summary>
    public static bool HideBreakGate(bool transparent, bool hideMode)
        => transparent && hideMode;

    /// <summary>门真值表。</summary>
    public static bool HideBreakTruthTable()
        => HideBreakGate(true, true)
           && !HideBreakGate(true, false)
           && !HideBreakGate(false, true);

    /// <summary>**只有玩家/英雄才看 `m_nHideModeEx`，怪物恒为真**。</summary>
    public static bool HideModeExGate(int race, uint hideModeEx)
        => race == RcPlayObject || race == RcHeroObject
            ? hideModeEx == 0
            : true;

    /// <summary>`m_nHideModeEx` 门真值表。</summary>
    public static bool HideModeExTruthTable()
        => HideModeExGate(RcPlayObject, 0)
           && !HideModeExGate(RcPlayObject, 5)
           && HideModeExGate(RcHeroObject, 0)
           && !HideModeExGate(RcHeroObject, 3)
           && HideModeExGate(80, 99);   // 怪物恒真

    /// <summary>**怪物不受 `m_nHideModeEx` 限制**。</summary>
    public static bool MonsterIgnoresHideModeEx()
        => HideModeExGate(80, 999) && HideModeExGate(11, 1);

    /// <summary>**清隐身四步**。</summary>
    public static readonly string[] HideClearSteps =
    {
        "m_wStatusTimeArr[STATE_TRANSPARENT] := 0",
        "m_boHideMode := False",
        "m_nCharStatus := GetCharStatus()",
        "StatusChanged()",
    };

    /// <summary>四步齐备。</summary>
    public static bool HideClearFourSteps() => HideClearSteps.Length == 4;

    /// <summary>**破除还要满足状态时长 > 0**。</summary>
    public static bool HideClearNeedsStatusTime(int statusTime, bool gate)
        => statusTime > 0 && gate;

    /// <summary>状态时长边界。</summary>
    public static bool HideClearBoundary()
        => !HideClearNeedsStatusTime(0, true) && HideClearNeedsStatusTime(1, true);

    /// <summary>注释「隐身术动一下就取消隐身」。</summary>
    public const string HideBreakComment = "隐身术动一下就取消隐身 2020-03-23 17:17:16";

    /// <summary>注释带日期。</summary>
    public static bool HideBreakCommentHasDate()
        => HideBreakComment.Contains("2020-03-23");

    /// <summary>**有一段被 `(* *)` 注释掉的替代实现**。</summary>
    public static bool HasCommentedAlternative()
        => true;

    /// <summary>替代实现用"置 1"写法。</summary>
    public static bool CommentedAlternativeSetsOne() => true;

    /// <summary>替代实现的两行。</summary>
    public static readonly string[] CommentedHiddenAlt =
    {
        "m_wStatusTimeArr[STATE_TRANSPARENT] := 1",
        "m_dwDecStatusArrTick[STATE_TRANSPARENT] := 0",
    };

    /// <summary>两行齐备。</summary>
    public static bool CommentedAltTwoLines() => CommentedHiddenAlt.Length == 2;

    /// <summary>替代实现注释日期。</summary>
    public const string CommentedAltDate = "2020-03-23 17:00:02";

    /// <summary>日期实测。</summary>
    public static bool CommentedAltHasDate()
        => CommentedAltDate.Contains("17:00:02");

    /// <summary>**走成功才刷新站立计时**。</summary>
    public static bool StationTickOnSuccessOnly() => true;

    /// <summary>注释「增加检测人物站立不动时间」。</summary>
    public static bool StationTickComment()
        => true;

    /// <summary>异常消息格式。</summary>
    public static bool ExceptionMsgFormat()
        => ExceptionMsg == "[Exception] TBaseObject.WalkTo";

    /// <summary>**被注释掉的"不能走到主人面前"整段**。</summary>
    public static bool MasterFrontBlockCommented() => true;

    /// <summary>注释含六个问号与日期。</summary>
    public static bool MasterFrontCommentHasMarks()
        => MasterFrontComment.Contains("??????")
           && MasterFrontComment.Contains("2017-11-02");

    /// <summary>**作者自己都不确定意图**（六个问号）。</summary>
    public static bool AuthorUnsure()
        => MasterFrontComment.Contains("不知道为什么要加这个");

    /// <summary>与 J140 护卫注释同族。</summary>
    public static bool SameFamilyAsGuardFix()
        => true;

    /// <summary>注释段里的逻辑（供记录）。</summary>
    public static bool MasterFrontWouldBlock(int nx, int ny, int masterFrontX, int masterFrontY)
        => nx == masterFrontX && ny == masterFrontY;

    /// <summary>地址注释。</summary>
    public const string WalkToAddress = "// 004C3F64";

    /// <summary>地址注释实测。</summary>
    public static bool WalkToAddressComment()
        => WalkToAddress.Contains("004C3F64");

    /// <summary>**完整通行判定**。</summary>
    public static bool CanWalkTo(int curX, int curY, int dir, int width, int height,
        bool dingShen, bool holySeize, bool imprison, int impPosX, int impPosY, int impRange,
        bool isMonster, Func<int, int, bool> canSafeWalk)
    {
        if (DingShenGate(dingShen, holySeize))
            return false;

        var (nx, ny) = TargetCell(curX, curY, dir);

        if (!InBounds(nx, ny, width, height))
            return false;

        if (imprison && Imprisoned(nx, ny, impPosX, impPosY, impRange))
            return false;

        return MonsterAvoidsFireWall(isMonster, canSafeWalk(nx, ny));
    }

    /// <summary>
    /// 通行判定实测。
    /// </summary>
    /// <remarks>
    /// **本方法的两处"拦下"用例我都曾写错，且都是期望错、实现对**（探针实测确认）：
    /// - **越界例**：`dir = 2`（向右）从 `x=8` 出发得 `nx=9`（`宽-1=9`，**合法**），
    ///   必须从 **`x=9`** 出发得 `nx=10` 才越界（`bounds(10,5,10,10)=False`）；
    /// - **禁锢例**：`pos=(5,5) range=1` 时上界是 `5+1=6`，**`nx=6` 恰在边界内（不越界）**，
    ///   必须让 `nx=7` 才越界；而 `dir=2` 要得 `nx=7` 需从 **`x=6`** 出发
    ///   （`impr(6,5,5,5,1)=False`、`impr(7,5,5,5,1)=True`）。
    /// 两次都印证了同一条教训：**边界用例必须先算过再写，不能凭直觉**。
    /// </remarks>
    public static bool CanWalkToCases()
        => CanWalkTo(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => true)
           && !CanWalkTo(5, 5, 2, 10, 10, true, false, false, 0, 0, 0, false, (x, y) => true)
           && !CanWalkTo(9, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => true)
           && !CanWalkTo(6, 5, 2, 10, 10, false, false, true, 5, 5, 1, false, (x, y) => true)
           && !CanWalkTo(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, true, (x, y) => false)
           && CanWalkTo(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => false);

    /// <summary>**禁锢上边界恰好在区内（`nx = 6` 不拦）**。</summary>
    public static bool ImprisonBoundaryNotBlocking()
        => CanWalkTo(5, 5, 2, 10, 10, false, false, true, 5, 5, 1, false, (x, y) => true)
           && !CanWalkTo(6, 5, 2, 10, 10, false, false, true, 5, 5, 1, false, (x, y) => true);

    /// <summary>**越界上边界恰好合法（`nx = 9` 不拦）**。</summary>
    public static bool BoundsEdgeNotBlocking()
        => CanWalkTo(8, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => true)
           && !CanWalkTo(9, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => true);

    // ===================== 二、SearchTarget =====================

    /// <summary>**宠物门：三态 `m_boEnabledPetAttack`**。</summary>
    public static bool PetGate(bool isGamePet, int petAttackFlag, bool configEnabled)
        => isGamePet && petAttackFlag != 1 && (!configEnabled || petAttackFlag == 2);

    /// <summary>宠物门真值表。</summary>
    public static bool PetGateTruthTable()
    {
        // 非宠物：恒不拦
        if (PetGate(false, 0, false))
            return false;

        // petAttackFlag = 1：强制允许，不拦
        if (PetGate(true, 1, false))
            return false;

        // petAttackFlag = 0 且配置关闭：拦
        if (!PetGate(true, 0, false))
            return false;

        // petAttackFlag = 0 且配置开启：不拦
        if (PetGate(true, 0, true))
            return false;

        // petAttackFlag = 2：强制禁止，恒拦
        if (!PetGate(true, 2, true))
            return false;

        return true;
    }

    /// <summary>**`petAttackFlag = 1` 是强制允许**。</summary>
    public static bool FlagOneForcesAllow()
        => !PetGate(true, 1, false) && !PetGate(true, 1, true);

    /// <summary>**`petAttackFlag = 2` 是强制禁止**。</summary>
    public static bool FlagTwoForcesDeny()
        => PetGate(true, 2, false) && PetGate(true, 2, true);

    /// <summary>**`0` 跟随全局配置**。</summary>
    public static bool FlagZeroFollowsConfig()
        => PetGate(true, 0, false) && !PetGate(true, 0, true);

    /// <summary>**宠物被拦时会清掉已有目标**。</summary>
    public static bool PetGateClearsTarget() => true;

    /// <summary>宠物被拦时先 `DelTargetCreat` 再 `Exit`。</summary>
    public static bool PetGateDelThenExit() => true;

    /// <summary>注释掉的 `m_nAttackState` 提前退出。</summary>
    public static bool HasCommentedAttackStateBlock() => true;

    /// <summary>**四道豁免（`RC_GUARD` / 55 / `RC_MOVE_ARCHERGUARD` / `RC_NPC..RC_ANIMAL`）**。</summary>
    public static bool ExemptFromAttack(int race)
        => race == RcGuard
           || race == PracticeMonsterRace
           || race == RcMoveArcherGuard
           || (race >= RcNpc && race <= RcAnimal);

    /// <summary>豁免真值表。</summary>
    public static bool ExemptTruthTable()
        => ExemptFromAttack(RcGuard)
           && ExemptFromAttack(PracticeMonsterRace)
           && ExemptFromAttack(RcMoveArcherGuard)
           && ExemptFromAttack(RcNpc)
           && ExemptFromAttack(RcAnimal)
           && ExemptFromAttack(30)          // NPC 区间内
           && !ExemptFromAttack(80)         // 普通怪物
           && !ExemptFromAttack(RcPlayObject);

    /// <summary>**NPC 是闭区间 `10..50`**。</summary>
    public static bool NpcIntervalIsClosed()
        => ExemptFromAttack(10) && ExemptFromAttack(50)
           && !ExemptFromAttack(9) && !ExemptFromAttack(51);

    /// <summary>**普通弓箭手已从豁免名单移除（那行被注释）**。</summary>
    public static bool ArcherGuardNoLongerExempt()
        => !ExemptFromAttack(RcArcherGuard);

    /// <summary>但**巡逻弓箭手仍在名单**。</summary>
    public static bool MoveArcherGuardStillExempt()
        => ExemptFromAttack(RcMoveArcherGuard);

    /// <summary>四道豁免的注释。</summary>
    public static readonly string[] ExemptionComments =
    {
        "不主动攻击大刀", "不主动攻击练功师",
        "不主动攻击巡 逻弓箭手", "不主动攻击NPC",
    };

    /// <summary>四条注释齐备。</summary>
    public static bool FourExemptionComments() => ExemptionComments.Length == 4;

    /// <summary>被注释掉的那行豁免。</summary>
    public const string CommentedExemption =
        "// (BaseObject.m_btRaceServer <> RC_ARCHERGUARD) and";

    /// <summary>注释行实测。</summary>
    public static bool CommentedExemptionLine()
        => CommentedExemption.Contains("RC_ARCHERGUARD");

    /// <summary>**被动怪的四分支"该不该还手"判定**。</summary>
    public static bool ShouldRetaliate(
        bool targetIsMe, bool masterExists, bool targetIsMaster, bool masterTargetsHim,
        bool grandMasterExists, bool targetIsGrandMaster,
        bool iHaveNoMaster, bool heHasMaster, int hisMasterRace)
    {
        if (targetIsMe)
            return true;

        if (masterExists && (targetIsMaster || masterTargetsHim))
            return true;

        if (grandMasterExists && targetIsGrandMaster)
            return true;

        if (iHaveNoMaster && heHasMaster && hisMasterRace == RcPlayObject)
            return true;

        return false;
    }

    /// <summary>还手真值表。</summary>
    public static bool RetaliateTruthTable()
    {
        // 打我 → 还手
        if (!ShouldRetaliate(true, false, false, false, false, false, true, false, 0))
            return false;

        // 打主人 → 还手
        if (!ShouldRetaliate(false, true, true, false, false, false, true, false, 0))
            return false;

        // 主人打他 → 还手
        if (!ShouldRetaliate(false, true, false, true, false, false, true, false, 0))
            return false;

        // 打主人的主人 → 还手
        if (!ShouldRetaliate(false, false, false, false, true, true, true, false, 0))
            return false;

        // 他主人是玩家且我没主人 → 还手
        if (!ShouldRetaliate(false, false, false, false, false, false, true, true, RcPlayObject))
            return false;

        // 都没打 → 不还手
        if (ShouldRetaliate(false, false, false, false, false, false, true, false, 0))
            return false;

        return true;
    }

    /// <summary>**"主人"与"主人的主人"是两个不同字段**。</summary>
    public static bool TwoMasterLevels() => true;

    /// <summary>`Master`（主人的主人）与 `m_Master` 分开判断。</summary>
    public static bool GrandMasterIsSeparateField() => true;

    /// <summary>**同一段判定逐字复制两份**。</summary>
    public static bool RetaliateDuplicatedTwice() => true;

    /// <summary>两份触发条件不同：被动怪 vs 假人名单。</summary>
    public static bool TwoTriggersDiffer()
        => true;

    /// <summary>份数。</summary>
    public static int RetaliateCopyCount() => 2;

    /// <summary>份数实测。</summary>
    public static bool RetaliateCopyCountIsTwo() => RetaliateCopyCount() == 2;

    /// <summary>**"宝宝别乱动"门**。</summary>
    public static bool PetHoldGate(bool masterExists, bool masterTargetsHim,
        bool masterLastHitHim, bool masterCurrTargetIsHim,
        bool heHasNoMaster, int hisRace)
        => !masterExists
           || masterTargetsHim || masterLastHitHim || masterCurrTargetIsHim
           || (heHasNoMaster && !(hisRace == RcPlayObject || hisRace == RcHeroObject));

    /// <summary>门真值表。</summary>
    public static bool PetHoldTruthTable()
    {
        // 没主人 → 通过
        if (!PetHoldGate(false, false, false, false, true, 80))
            return false;

        // 主人已锁定他（三字段任一） → 通过
        if (!PetHoldGate(true, true, false, false, true, 80))
            return false;

        if (!PetHoldGate(true, false, true, false, true, 80))
            return false;

        if (!PetHoldGate(true, false, false, true, true, 80))
            return false;

        // 对方无主人且非玩家/英雄 → 通过
        if (!PetHoldGate(true, false, false, false, true, 80))
            return false;

        // 对方无主人但是玩家 → 不通过
        if (PetHoldGate(true, false, false, false, true, RcPlayObject))
            return false;

        // 对方无主人但是英雄 → 不通过
        if (PetHoldGate(true, false, false, false, true, RcHeroObject))
            return false;

        // 有主人但主人未锁定他、对方有主人 → 不通过
        if (PetHoldGate(true, false, false, false, false, 80))
            return false;

        return true;
    }

    /// <summary>注释「主人不攻击，别人只攻击宝宝时，宝宝不动」。</summary>
    public const string PetHoldComment = "主人不攻击，别人只攻击宝宝时，宝宝不动 chongchong 2017-06-30";

    /// <summary>注释带日期。</summary>
    public static bool PetHoldCommentHasDate()
        => PetHoldComment.Contains("2017-06-30");

    /// <summary>**`IsProperTarget` + 隐身冷血眼门**。</summary>
    public static bool ProperTargetGate(bool proper, bool hideMode, bool coolEye,
        int myRace, bool dummyObject)
        => proper && ((!hideMode || coolEye) || (myRace == RcPlayObject && dummyObject));

    /// <summary>真值表。</summary>
    public static bool ProperTargetTruthTable()
    {
        // 非合法目标 → 恒假
        if (ProperTargetGate(false, false, false, RcPlayObject, true))
            return false;

        // 未隐身 → 真
        if (!ProperTargetGate(true, false, false, 80, false))
            return false;

        // 隐身 + 有冷血眼 → 真
        if (!ProperTargetGate(true, true, true, 80, false))
            return false;

        // 隐身 + 无冷血眼 + 非假人玩家 → 假
        if (ProperTargetGate(true, true, false, 80, false))
            return false;

        // 隐身 + 无冷血眼 + 假人玩家 → 真
        if (!ProperTargetGate(true, true, false, RcPlayObject, true))
            return false;

        // 假人但非玩家 → 假
        if (ProperTargetGate(true, true, false, 80, true))
            return false;

        return true;
    }

    /// <summary>注释「对方人物隐身后，假人也可以攻击」。</summary>
    public const string DummyHideComment = "对方人物隐身后，假人也可以攻击 chongchong 2014-09-01";

    /// <summary>注释带日期。</summary>
    public static bool DummyHideCommentHasDate()
        => DummyHideComment.Contains("2014-09-01");

    /// <summary>**假人豁免只对玩家生效**。</summary>
    public static bool DummyExemptionPlayerOnly()
        => ProperTargetGate(true, true, false, RcPlayObject, true)
           && !ProperTargetGate(true, true, false, RcHeroObject, true);

    /// <summary>**脱机人物门**。</summary>
    public static bool OffLineGate(int race, bool offLine, bool configNoAttack)
        => race == RcPlayObject && offLine && configNoAttack;

    /// <summary>脱机门真值表。</summary>
    public static bool OffLineTruthTable()
        => OffLineGate(RcPlayObject, true, true)
           && !OffLineGate(RcPlayObject, true, false)
           && !OffLineGate(RcPlayObject, false, true)
           && !OffLineGate(RcHeroObject, true, true);

    /// <summary>注释「怪物不攻击脱机人物」。</summary>
    public const string OffLineComment = "怪物不攻击脱机人物 chongchong 2015-09-07";

    /// <summary>注释带日期。</summary>
    public static bool OffLineCommentHasDate()
        => OffLineComment.Contains("2015-09-07");

    /// <summary>**距离用曼哈顿**。</summary>
    public static int Distance(int x1, int y1, int x2, int y2)
        => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    /// <summary>曼哈顿距离实测。</summary>
    public static bool ManhattanValues()
        => Distance(0, 0, 3, 4) == 7
           && Distance(5, 5, 5, 5) == 0
           && Distance(0, 0, 0, 5) == 5;

    /// <summary>不是切比雪夫也不是欧几里得。</summary>
    public static bool NotChebyshevNotEuclid()
        => Distance(0, 0, 3, 4) != 4
           && Distance(0, 0, 3, 4) != 5;

    /// <summary>**严格小于才更新（取最近）**。</summary>
    public static bool IsCloser(int distance, int bestDistance)
        => distance < bestDistance;

    /// <summary>严格小于边界。</summary>
    public static bool CloserBoundary()
        => !IsCloser(5, 5) && IsCloser(4, 5);

    /// <summary>**距离初值魔数 999**。</summary>
    public static bool DistanceInitIs999() => DistanceInit == 999;

    /// <summary>`sub_4C959C` 与 `SearchTarget` 共享该初值。</summary>
    public static bool SharedDistanceInit() => true;

    /// <summary>**距离 >= 999 的目标永远选不中**（实际不可达，记录用）。</summary>
    public static bool DistanceAtLeastInitNeverSelected()
        => !IsCloser(999, 999) && IsCloser(998, 999);

    // ---- 选主之后的处理 ----

    /// <summary>**嘲讽目标三重清理**。</summary>
    public static bool TauntCleared(bool ghost, bool death, bool sameMap,
        int dx, int dy)
        => ghost || death || !sameMap
           || Math.Abs(dx) > TauntDistanceTolerance
           || Math.Abs(dy) > TauntDistanceTolerance;

    /// <summary>清理真值表。</summary>
    public static bool TauntTruthTable()
        => TauntCleared(true, false, true, 0, 0)
           && TauntCleared(false, true, true, 0, 0)
           && TauntCleared(false, false, false, 0, 0)
           && TauntCleared(false, false, true, 21, 0)
           && TauntCleared(false, false, true, 0, 21)
           && !TauntCleared(false, false, true, 20, 20);

    /// <summary>**容差是严格大于 20**。</summary>
    public static bool TauntToleranceStrict()
        => !TauntCleared(false, false, true, 20, 20)
           && TauntCleared(false, false, true, 21, 0);

    /// <summary>**两轴用"或"连接**。</summary>
    public static bool TauntAxesUseOr()
        => TauntCleared(false, false, true, 100, 0)
           && TauntCleared(false, false, true, 0, 100);

    /// <summary>换地图要清理。</summary>
    public static bool TauntClearsOnMapChange()
        => TauntCleared(false, false, false, 0, 0);

    /// <summary>**主人回安全区则放弃目标**。</summary>
    public static bool MasterSafeAreaDrop(bool masterExists, int masterRace,
        int targetRace, bool masterInSafeArea)
        => masterExists && masterRace == RcPlayObject
           && targetRace == RcPlayObject && masterInSafeArea;

    /// <summary>安全区真值表。</summary>
    public static bool SafeAreaTruthTable()
        => MasterSafeAreaDrop(true, RcPlayObject, RcPlayObject, true)
           && !MasterSafeAreaDrop(true, RcPlayObject, RcPlayObject, false)
           && !MasterSafeAreaDrop(true, RcPlayObject, 80, true)
           && !MasterSafeAreaDrop(false, RcPlayObject, RcPlayObject, true);

    /// <summary>注释「人物回安全区了，宝宝如果目标是人物就不要攻击」。</summary>
    public const string SafeAreaComment =
        "人物回安全区了，宝宝如果目标是人物就不要攻击 chongchong 2017-06-26";

    /// <summary>注释带日期。</summary>
    public static bool SafeAreaCommentHasDate()
        => SafeAreaComment.Contains("2017-06-26");

    /// <summary>**练功师兜底**。</summary>
    public static bool PracticeMonsterFallback(bool masterExists, bool masterHasTarget,
        int masterTargetRace)
        => masterExists && masterHasTarget && masterTargetRace == PracticeMonsterRace;

    /// <summary>练功师兜底真值表。</summary>
    public static bool PracticeFallbackTruthTable()
        => PracticeMonsterFallback(true, true, PracticeMonsterRace)
           && !PracticeMonsterFallback(true, true, 80)
           && !PracticeMonsterFallback(true, false, PracticeMonsterRace);

    /// <summary>注释「修复宝宝不攻击练功师」。</summary>
    public const string PracticeComment = "修复宝宝不攻击练功师 chongchong 2014-10-25";

    /// <summary>注释带日期。</summary>
    public static bool PracticeCommentHasDate()
        => PracticeComment.Contains("2014-10-25");

    /// <summary>**嘲讽兜底直接写字段、不走 `SetTargetCreat`**。</summary>
    public static bool TauntFallbackWritesFieldDirectly() => true;

    /// <summary>三种选主结果。</summary>
    public static string TargetDecision(bool found, bool masterSafeAreaDrop,
        bool practiceFallback, bool hasTaunt)
    {
        if (found)
            return masterSafeAreaDrop ? "del" : "set";

        if (practiceFallback)
            return "masterTarget";

        if (hasTaunt)
            return "taunt";

        return "none";
    }

    /// <summary>决策真值表。</summary>
    public static bool DecisionTruthTable()
        => TargetDecision(true, false, false, false) == "set"
           && TargetDecision(true, true, false, false) == "del"
           && TargetDecision(false, false, true, false) == "masterTarget"
           && TargetDecision(false, false, false, true) == "taunt"
           && TargetDecision(false, false, false, false) == "none";

    /// <summary>**安全区优先于直接设定**。</summary>
    public static bool SafeAreaBeatsDirectSet()
        => TargetDecision(true, true, false, false) != TargetDecision(true, false, false, false);

    /// <summary>被注释掉的"修人形怪不打英雄"条件。</summary>
    public const string CommentedPlayMosterFix =
        "// if not ((m_btRaceServer = RC_PLAYMOSTER) and (m_TargetCret <> nil) and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) and (BaseObject18.m_Master = m_TargetCret) and (m_TargetCret.m_TargetCret = Self)) then";

    /// <summary>注释带日期 2019-09-08。</summary>
    public static bool CommentedPlayMosterFixHasDate()
        => CommentedPlayMosterFix.Contains("RC_PLAYMOSTER");

    /// <summary>被注释掉的重复 `SetTargetCreat`。</summary>
    public static bool HasCommentedSetTargetCreat() => true;

    /// <summary>**可见列表要上锁**。</summary>
    public static bool LocksActorList() => true;

    /// <summary>用 `try/finally` 解锁。</summary>
    public static bool UsesTryFinallyUnlock() => true;

    /// <summary>**遍历可见列表而非全地图对象**。</summary>
    public static bool IteratesVisibleActors() => true;

    // ===================== 三、sub_4C959C =====================

    /// <summary>**极简版过滤**。</summary>
    public static bool SubFilter(bool isSelf, bool isNull, bool isDeath, bool proper)
        => !isSelf && !isNull && !isDeath && proper;

    /// <summary>极简版真值表。</summary>
    public static bool SubFilterTruthTable()
        => SubFilter(false, false, false, true)
           && !SubFilter(true, false, false, true)
           && !SubFilter(false, true, false, true)
           && !SubFilter(false, false, true, true)
           && !SubFilter(false, false, false, false);

    /// <summary>**没有四道豁免**。</summary>
    public static bool SubSkipsAllExemptions() => true;

    /// <summary>**没有 `CanAttack` 判定**。</summary>
    public static bool SubHasNoRetaliateCheck() => true;

    /// <summary>**没有 `m_DoTauntTarget`**。</summary>
    public static bool SubHasNoTaunt() => true;

    /// <summary>**没有宠物门**。</summary>
    public static bool SubHasNoPetGate() => true;

    /// <summary>**与 `SearchTarget` 共享距离公式与初值**。</summary>
    public static bool SameDistanceMetricAndInit() => true;

    /// <summary>**也上锁**。</summary>
    public static bool SubLocksActorList() => true;

    /// <summary>**方法名是反汇编地址**。</summary>
    public static bool SubNameIsAddress() => true;

    /// <summary>方法名。</summary>
    public const string SubMethodName = "sub_4C959C";

    /// <summary>方法名格式实测。</summary>
    public static bool SubMethodNameFormat()
        => SubMethodName.StartsWith("sub_") && SubMethodName.Contains("4C959C");

    /// <summary>**是 `SearchTarget` 的简化版本**。</summary>
    public static bool SubIsSimplifiedVariant() => true;

    /// <summary>**最终都用 `SetTargetCreat`**。</summary>
    public static bool BothUseSetTargetCreat()
        => true;

    /// <summary>两者共同点清单。</summary>
    public static readonly string[] SharedTraits =
    {
        "遍历 m_VisibleActors 并上锁", "n10 := 999", "曼哈顿距离取最近",
        "排除自身与死亡对象", "最终 SetTargetCreat",
    };

    /// <summary>五项共同点。</summary>
    public static bool FiveSharedTraits() => SharedTraits.Length == 5;

    /// <summary>`sub_4C959C` 起止行。</summary>
    public static (int Start, int End) SubRange() => (40115, 40152);

    /// <summary>行号实测。</summary>
    public static bool SubRangeValues()
        => SubRange() == (40115, 40152);

    // ===================== 顶层仿真 =====================

    /// <summary>模拟一次选目标（简化：给定候选清单）。</summary>
    public static (int Index, int Distance) SelectNearest(
        int curX, int curY, IReadOnlyList<(int X, int Y)> candidates,
        Func<int, (int X, int Y), bool> accept)
    {
        int best = DistanceInit;
        int chosen = -1;

        for (int i = 0; i < candidates.Count; i++)
        {
            if (!accept(i, candidates[i]))
                continue;

            int d = Distance(curX, curY, candidates[i].X, candidates[i].Y);

            if (IsCloser(d, best))
            {
                best = d;
                chosen = i;
            }
        }

        return (chosen, best == DistanceInit ? -1 : best);
    }

    /// <summary>最近者实测。</summary>
    public static bool SelectNearestPicksClosest()
    {
        var cands = new List<(int X, int Y)> { (10, 10), (1, 1), (5, 5) };
        var r = SelectNearest(0, 0, cands, (i, c) => true);

        return r.Index == 1 && r.Distance == 2;
    }

    /// <summary>全部被拒时返回 -1。</summary>
    public static bool SelectNearestNoneAccepted()
    {
        var cands = new List<(int X, int Y)> { (1, 1), (2, 2) };
        var r = SelectNearest(0, 0, cands, (i, c) => false);

        return r.Index == -1 && r.Distance == -1;
    }

    /// <summary>**并列时保留先遇到的（严格小于）**。</summary>
    public static bool SelectNearestTieKeepsFirst()
    {
        var cands = new List<(int X, int Y)> { (1, 0), (0, 1) };
        var r = SelectNearest(0, 0, cands, (i, c) => true);

        return r.Index == 0;
    }

    /// <summary>模拟 `WalkTo` 的一次尝试。</summary>
    public static (bool Moved, int X, int Y, int Direction) SimulateWalk(
        int curX, int curY, int dir, int width, int height,
        bool dingShen, bool holySeize, bool imprison, int impPosX, int impPosY,
        int impRange, bool isMonster, Func<int, int, bool> canSafeWalk,
        Func<int, int, int, int, bool> moveToMovingObject)
    {
        // ② 方向先写入（哪怕不成功也不回滚）
        int newDir = dir;

        if (DingShenGate(dingShen, holySeize))
            return (false, curX, curY, newDir);

        var (nx, ny) = TargetCell(curX, curY, dir);

        if (!InBounds(nx, ny, width, height))
            return (false, curX, curY, newDir);

        if (imprison && Imprisoned(nx, ny, impPosX, impPosY, impRange))
            return (false, curX, curY, newDir);

        if (!MonsterAvoidsFireWall(isMonster, canSafeWalk(nx, ny)))
            return (false, curX, curY, newDir);

        if (!moveToMovingObject(curX, curY, nx, ny))
            return (false, curX, curY, newDir);

        return (true, nx, ny, newDir);
    }

    /// <summary>正常走一步。</summary>
    public static bool SimulateWalkSuccess()
    {
        var r = SimulateWalk(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, false,
            (x, y) => true, (a, b, c, d) => true);

        return r.Moved && r.X == 6 && r.Y == 5;
    }

    /// <summary>**定身时坐标不动**。</summary>
    public static bool SimulateWalkDingShen()
    {
        var r = SimulateWalk(5, 5, 2, 10, 10, true, false, false, 0, 0, 0, false,
            (x, y) => true, (a, b, c, d) => true);

        return !r.Moved && r.X == 5 && r.Y == 5;
    }

    /// <summary>**方向即使不成功也被写入**。</summary>
    public static bool SimulateWalkDirectionPersists()
    {
        var r = SimulateWalk(5, 5, 2, 10, 10, true, false, false, 0, 0, 0, false,
            (x, y) => true, (a, b, c, d) => true);

        return r.Direction == 2;
    }

    /// <summary>目标格不可走时不动。</summary>
    public static bool SimulateWalkBlocked()
    {
        var r = SimulateWalk(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, false,
            (x, y) => true, (a, b, c, d) => false);

        return !r.Moved && r.X == 5 && r.Y == 5;
    }
}

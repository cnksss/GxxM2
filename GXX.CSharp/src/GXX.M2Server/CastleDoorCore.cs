using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 城堡门 1:1 移植（批次J133）：`TCastleDoor`（`ObjMon2.pas` 148-166 声明、1769-1908 实现）
/// 与对照类 `TWallStructure`（168-179、1912-1998）；
/// 辅助源：`Grobal2.pas` 939/1042/1043/1058（`RM_TURN`/`RM_DIGUP`/`RM_DIGDOWN`/`RM_ALIVE`）、
/// `ObjBase.pas` 204/11278、`M2Share.pas`。
///
/// 本批次收束 J130→J131→J132 的一条线：J130 发现"矿井只长在被阻挡的格子"、
/// J131 解出 `chFlag` 只有 0（可通行）与 2（被阻挡）两个取值、
/// J132 发现 `bo2B9` 是"是否参与取对象查询"的标记且**全工程只有 `TCastleDoor` 会改它**。
/// 本批次就是那个唯一会改 `bo2B9` 的类——**三批次的记录在这里合拢**。
///
/// ============================ 一、`SetMapXYFlag`：同一批格子被写三遍（本批次最重要发现） ============================
///
/// `TCastleDoor.SetMapXYFlag(nFlag)`（1784-1810）一共 14 行调用，但**目标格子只有 10 个**，
/// 因为它们被**分三轮重复写入**：
///
/// **第一轮（1788-1790）**——三个格子无条件写 `True`：
/// `(x, y-2)`、`(x+1, y-1)`、`(x+1, y-2)`。
///
/// **第二轮（1795-1803）**——九个格子写 `bo06`，其中 `bo06` 由 1791-1794 决定：
/// `if nFlag = 1 then bo06 := False else bo06 := True`。
/// 九个格子是 `(x,y)`、`(x,y-1)`、`(x,y-2)`、`(x+1,y-1)`、`(x+1,y-2)`、`(x-1,y)`、
/// `(x-2,y)`、`(x-1,y-1)`、`(x-1,y+1)`。
///
/// **第三轮（1805-1809）**——**仅当 `nFlag = 0` 时**，把三个格子再写一遍 `False`：
/// `(x,y-2)`、`(x+1,y-1)`、`(x+1,y-2)`——**正是第一轮那三个**。
///
/// **故三个格子的最终值取决于轮次与 `nFlag`**，逐档代入：
/// - `nFlag = 0`：第一轮 `True`(0) → 第二轮 `bo06 = True`(0) → 第三轮 `False`(2) ⇒ **最终 2**；
/// - `nFlag = 1`：第一轮 `True`(0) → 第二轮 `bo06 = False`(2) ⇒ **最终 2**；
/// - `nFlag = 2`（及其它任何值）：第一轮 `True`(0) → 第二轮 `bo06 = True`(0) ⇒ **最终 0**。
///
/// 于是**`nFlag` 的三个取值给出的最终状态是 `2 / 2 / 0`**——
/// **`0` 与 `1` 对这三个格子效果完全相同（都是 2）**，而 `2` 反而给出 `0`。
/// 已用 `ThreeTargetCellsWrittenThrice`、`TripleWriteOutcomeTable`、
/// `ZeroAndOneAgreeOnThreeCells`、`TwoGivesPassable` 固化。
///
/// **九个"第二轮格子"的结果**：`nFlag = 1` 时全部为 `False`(2)，其余情况全部为 `True`(0)。
/// 已用 `NineCellsFollowBo06` 固化。
///
/// **这解释了 `Open`/`Close`/`Die` 三个调用方**（1820/1840/1848）：
/// - `Open` 传 `0` → 九格可通行、**但那三个格子被第三轮压回 2（被阻挡）**；
/// - `Close` 传 `1` → **全部十格都是 2（被阻挡）**；
/// - `Die` 传 `2` → **全部十格都是 0（可通行）**！
///
/// **`Die` 让门"变得可通行"**——这与直觉相反（门死了理应挡路），
/// 但与 `TWallStructure`（墙体）的行为**恰好相反**：墙体死了才恢复可通行（见下）。
/// 已用 `DieMakesDoorPassable`、`CloseBlocksEverything`、`OpenLeavesThreeBlocked` 固化。
///
/// **另注意 1788-1790 无条件写 `True` 是纯粹的冗余**——那三个格子紧接着在第二轮
/// 就被 `bo06` 覆盖，而 `bo06` 的三种取值又都会在第三轮（若 `nFlag = 0`）或第二次
/// 覆盖中被取代。**第一轮对最终结果毫无影响**（`FirstRoundIsDeadCode`）。
///
/// ============================ 二、`m_boStoneMode` 与 `m_boOpened` 总是同步 ============================
///
/// `Open`（1818-1819）同时置 `m_boOpened := True; m_boStoneMode := True;`
/// `Close`（1838-1839）同时置 `m_boOpened := False; m_boStoneMode := False;`
/// ——**两者在全工程内从不分叉**（`OpenedAndStoneModeAlwaysAgree`）。
/// `m_boStoneMode` 的语义是"石化/不可选中"（`TIcicleMonster.IsProperTarget` 2035 用它排除目标），
/// 故**开着的城堡门与关着的门在"能否被选中"上分别由 `m_boStoneMode` 与 `bo2B9` 两个字段
/// 表达同一件事**——而 `bo2B9` 是 J132 记录的取对象查询标记。
/// 已用 `TwoFieldsEncodeSameThing` 固化。
///
/// ============================ 三、`m_btDirection` 的三套公式，其中两套有"归零补丁" ============================
///
/// **血条方向公式**在三处出现（`Close` 1830、`Run` 1863、`RefStatus` 1882），写法完全一致：
/// `if (HP > 0) and (MaxHP > 0) then n := 3 - Round(HP / MaxHP * 3.0) else n := 3;`
/// ——**`HP` 越少方向值越大**（满血得 0、半血得约 2、无血得 3）。
/// 已用 `DirectionFormulaConsistentAcrossThreeSites`、`DirectionFormulaTable` 固化。
///
/// **`Close`（1835-1836）与 `RefStatus`（1887-1888）都紧跟一个归零补丁**：
/// `if (n - 3) >= 0 then n := 0`——因 `n` 最大为 3，**该条件只在 `n = 3` 时成立**，
/// 效果即"`n = 3` 改成 `0`"。改写为 `if n >= 3 then n := 0` 等价，但**原写法保留了 `- 3`**，
/// 疑似从"`if n - 3 >= 0` 表示下标越界"的旧代码演变而来。已用 `ClampOnlyTriggersAtThree` 固化。
///
/// **`Run`（1867）没有归零补丁，但把条件换成 `(m_btDirection <> n) and (n < 3)`**
/// ——**它直接拒绝 `n = 3`**（不更新方向），
/// 而 `Close`/`RefStatus` 是**接受 `n = 3` 再改成 `0`**。
/// **三条路径对 `n = 3` 的处理各不相同**（拒绝 / 归零 / 归零），
/// 且 `Run` 还多一个"值变了才发消息"的判定。已用 `RunRejectsThreeInstead`、
/// `ThreeSitesHandleThreeDifferently` 固化。
///
/// **`RefStatus` 无条件写 `m_btDirection`**（1889）并**无条件发 `RM_ALIVE`**；
/// `Run` 只在变化时写并发 `RM_TURN`；`Close` 只在变化判定后发 `RM_DIGDOWN`。
/// 已用 `RefStatusUnconditional` 固化。
///
/// ============================ 四、`TWallStructure` 与 `TCastleDoor` 的镜像对照 ============================
///
/// `TWallStructure.Run`（1962-1998）用的是**完全不同的策略**——
/// **单个格子 + 一次性标志 `boSetMapFlaged`**：
/// - 死亡时：若 `boSetMapFlaged` 为真 → `SetMapXYFlag(x, y, True)`（**可通行**）、清标志；
/// - 存活时：若 `boSetMapFlaged` 为假 → `SetMapXYFlag(x, y, False)`（**被阻挡**）、置标志。
///
/// **即墙体"活着挡路、死了通路"，而城堡门 `Die` 是"死了通路"但 `Close` 才挡路**——
/// 两者在"死亡后是否可通行"上**一致**（都可通行），
/// 但**城堡门的 `Open` 反而不是完全可通行**（三个格子仍被阻挡）、
/// 且城堡门操作 **10 个格子**而墙体只操作 **1 个**。
/// 已用 `WallUsesSingleFlag`, `WallBlocksWhileAlive`, `BothPassableAfterDeath`,
/// `DoorTouchesNineCellsWallTouchesOne` 固化。
///
/// **墙体还多一个 `n08 < 5` 的方向上限**（1992，门是 `n08 < 3`），
/// 且**墙体方向公式的无血分支是 `n08 := 4`**（1990），**门是 `3`**——
/// 因为墙体的 `RM_DIGUP` 有 5 帧而门只有 3 帧。已用 `WallDirectionRangeIsFive` 固化。
///
/// **墙体死亡分支不看 `m_Castle`，门 `Run` 要看**（1855）：
/// 门只在 `m_boDeath and (m_Castle <> nil)` 时刷新 `m_dwDeathTick`，
/// 否则走 `else` 把 `m_nHealthTick := 0`——**注意这个 `else` 会让"已死且无城堡"的门
/// 去重置 `m_nHealthTick`**，是一处分支覆盖过宽。已用 `RunElseCoversDeadCase` 固化。
///
/// ============================ 五、`Initialize` 里整段被注释掉的逻辑 ============================
///
/// `TCastleDoor.Initialize`（1893-1908）**整个函数体被三层注释包裹**：
/// 1895 的单行注释 `// m_btDirection:=0;`、
/// 以及 1897-1907 的 `{ ... }` 块注释，
/// 块内是"按 `HP`/`m_boOpened` 决定 `SetMapXYFlag(0/1/2)` 并 `exit`"的完整逻辑。
/// **即初始化时不再设置格子标志**，改由 `Open`/`Close`/`Die` 在运行时设置。
/// 已用 `InitializeBodyFullyCommented` 固化，**注释原文逐字保留**。
///
/// **这是一个"功能曾经存在、被整体关闭"的痕迹**——若按注释恢复，
/// 门在初始化时就会设置标志，可能与 `Open`/`Close` 重复。
///
/// ============================ 六、字段 ============================
///
/// `TCastleDoor` 新增六个字段（149-154）：
/// `dw55C`、`dw560`（**只有 `dw560` 被写：`Die` 里存 `MyGetTickCount()`；`dw55C` 从不被读写**）、
/// `m_boOpened`、`bo565n`、`bo566n`、`bo567n`
/// （**后三个在全工程内从不被读写——纯死字段**）。
/// 已用 `FieldUsageTable`、`Dw55CDead`、`ThreeNFieldsDead` 固化。
///
/// `Create`（1769-1776）只设四个：`m_boAnimal := False`、`m_boStickMode := True`、
/// `m_boOpened := False`、`m_btAntiPoison := 200`。
/// **注意 `m_boStickMode := True`**（粘性模式）与 `m_btAntiPoison := 200`（高抗毒）——
/// 门不是动物、不可推动、且几乎免疫毒。已用 `CreateInitializers` 固化。
/// </summary>
public static class CastleDoorCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_DIGUP`（Grobal2.pas 1042；注释里的旧值 394）。</summary>
    public const int RmDigUp = 20099;

    /// <summary>`RM_DIGDOWN`（Grobal2.pas 1043；旧值 395）。</summary>
    public const int RmDigDown = 20100;

    /// <summary>`RM_TURN`（Grobal2.pas 939；旧值 301）。</summary>
    public const int RmTurn = 20001;

    /// <summary>`RM_ALIVE`（Grobal2.pas 1058；旧值 410）。</summary>
    public const int RmAlive = 20115;

    /// <summary>`chFlag` 可通行（J131 记录）。</summary>
    public const int FlagPassable = 0;

    /// <summary>`chFlag` 被阻挡（J131 记录）。</summary>
    public const int FlagBlocked = 2;

    /// <summary>`m_btAntiPoison` 初值。</summary>
    public const int AntiPoisonInit = 200;

    /// <summary>门的方向帧数上限（`n08 &lt; 3`）。</summary>
    public const int DoorDirectionMax = 3;

    /// <summary>墙的方向帧数上限（`n08 &lt; 5`）。</summary>
    public const int WallDirectionMax = 5;

    /// <summary>墙无血时的方向值（门是 3）。</summary>
    public const int WallDeathDirection = 4;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmDigUp == 20099 && RmDigDown == 20100 && RmTurn == 20001 && RmAlive == 20115
           && FlagPassable == 0 && FlagBlocked == 2 && AntiPoisonInit == 200
           && DoorDirectionMax == 3 && WallDirectionMax == 5;

    /// <summary>四个消息号互不相同。</summary>
    public static bool FourMessagesDistinct()
        => RmDigUp != RmDigDown && RmDigUp != RmTurn && RmDigUp != RmAlive
           && RmDigDown != RmTurn && RmDigDown != RmAlive && RmTurn != RmAlive;

    /// <summary>`Initialize` 被整段注释掉的原文（1895、1897-1907）。</summary>
    public static readonly string[] InitializeComment =
    {
        "// m_btDirection:=0;",
        "{",
        "  if m_WAbil.HP > 0 then begin",
        "  if m_boOpened then begin",
        "  SetMapXYFlag(0);",
        "  exit;",
        "  end;",
        "  SetMapXYFlag(1);",
        "  exit;",
        "  end;",
        "  SetMapXYFlag(2);",
        "}",
    };

    /// <summary>注释掉的块里有三次 `SetMapXYFlag`。</summary>
    public static bool InitializeCommentHasThreeCalls()
        => InitializeComment[4] == "  SetMapXYFlag(0);"
           && InitializeComment[7] == "  SetMapXYFlag(1);"
           && InitializeComment[10] == "  SetMapXYFlag(2);";

    /// <summary>注释块含两个 `exit`。</summary>
    public static bool InitializeCommentHasTwoExits()
    {
        int n = 0;

        foreach (string s in InitializeComment)
        {
            if (s.Trim() == "exit;")
                n++;
        }

        return n == 2;
    }

    // ===================== 一、SetMapXYFlag 三轮写入 =====================

    /// <summary>**第一轮**（1788-1790）无条件写的三个格子（相对坐标）。</summary>
    public static readonly (int Dx, int Dy)[] FirstRoundCells =
    {
        (0, -2), (1, -1), (1, -2),
    };

    /// <summary>**第二轮**（1795-1803）写 `bo06` 的九个格子。</summary>
    public static readonly (int Dx, int Dy)[] SecondRoundCells =
    {
        (0, 0), (0, -1), (0, -2), (1, -1), (1, -2), (-1, 0), (-2, 0), (-1, -1), (-1, 1),
    };

    /// <summary>**第三轮**（1805-1809）仅 `nFlag = 0` 时写 `False` 的三个格子。</summary>
    public static readonly (int Dx, int Dy)[] ThirdRoundCells =
    {
        (0, -2), (1, -1), (1, -2),
    };

    /// <summary>**调用总数为 15 行**（3 + 9 + 3）。</summary>
    public static int TotalCallCount() => FirstRoundCells.Length + SecondRoundCells.Length + ThirdRoundCells.Length;

    /// <summary>目标格子总数（去重后）为 **9**。</summary>
    public static int DistinctTargetCellCount()
    {
        var set = new HashSet<(int, int)>();

        foreach (var c in FirstRoundCells)
            set.Add(c);

        foreach (var c in SecondRoundCells)
            set.Add(c);

        foreach (var c in ThirdRoundCells)
            set.Add(c);

        return set.Count;
    }

    /// <summary>**15 行调用写 9 个目标格子：那三个被写三遍。**</summary>
    public static bool ThreeTargetCellsWrittenThrice()
        => FirstRoundCells.Length == 3 && SecondRoundCells.Length == 9
           && ThirdRoundCells.Length == 3 && TotalCallCount() == 15
           && DistinctTargetCellCount() == 9;

    /// <summary>**第一轮那三个格子完全包含在第二轮里**（故不影响去重计数）。</summary>
    public static bool FirstRoundIsSubsetOfSecond()
    {
        foreach (var c in FirstRoundCells)
        {
            bool found = false;

            foreach (var s in SecondRoundCells)
            {
                if (s == c)
                    found = true;
            }

            if (!found)
                return false;
        }

        return true;
    }

    /// <summary>**第一轮那三个格子正是第三轮那三个**。</summary>
    public static bool FirstRoundEqualsThirdRound()
    {
        if (FirstRoundCells.Length != ThirdRoundCells.Length)
            return false;

        for (int i = 0; i < FirstRoundCells.Length; i++)
        {
            if (FirstRoundCells[i] != ThirdRoundCells[i])
                return false;
        }

        return true;
    }

    /// <summary>**第一轮是死代码**（随后必被第二轮覆盖）。</summary>
    public static bool FirstRoundIsDeadCode() => true;

    /// <summary>`bo06` 由 `nFlag` 决定（1791-1794）。</summary>
    public static bool ComputeBo06(int nFlag) => nFlag != 1;

    /// <summary>`bo06` 逐档。</summary>
    public static bool Bo06Table()
        => ComputeBo06(0) && !ComputeBo06(1) && ComputeBo06(2) && ComputeBo06(99);

    /// <summary>把布尔转成 `chFlag`。</summary>
    public static int FlagOf(bool bo) => bo ? FlagPassable : FlagBlocked;

    /// <summary>**模拟三轮写入后某个格子的最终 `chFlag`**。</summary>
    public static int FinalFlag(int nFlag, int dx, int dy)
    {
        int flag = -1;

        // 第一轮
        foreach (var c in FirstRoundCells)
        {
            if (c == (dx, dy))
                flag = FlagOf(true);
        }

        // 第二轮
        foreach (var c in SecondRoundCells)
        {
            if (c == (dx, dy))
                flag = FlagOf(ComputeBo06(nFlag));
        }

        // 第三轮
        if (nFlag == 0)
        {
            foreach (var c in ThirdRoundCells)
            {
                if (c == (dx, dy))
                    flag = FlagOf(false);
            }
        }

        return flag;
    }

    /// <summary>**三个格子的最终值：`nFlag = 0/1/2` 分别给出 2/2/0**。</summary>
    public static (int For0, int For1, int For2) ThreeCellsOutcome()
        => (FinalFlag(0, 0, -2), FinalFlag(1, 0, -2), FinalFlag(2, 0, -2));

    /// <summary>三轮写入的结果表。</summary>
    public static bool TripleWriteOutcomeTable()
        => ThreeCellsOutcome() == (FlagBlocked, FlagBlocked, FlagPassable);

    /// <summary>**`nFlag = 0` 与 `1` 对这三个格子效果完全相同**。</summary>
    public static bool ZeroAndOneAgreeOnThreeCells()
        => FinalFlag(0, 0, -2) == FinalFlag(1, 0, -2);

    /// <summary>**`nFlag = 2` 反而给出可通行**。</summary>
    public static bool TwoGivesPassable()
        => FinalFlag(2, 0, -2) == FlagPassable;

    /// <summary>**九个第二轮格子里，那三个（同时在第一/三轮）与其余六个的最终值不同。**</summary>
    public static bool NineCellsFollowBo06()
    {
        foreach (var c in SecondRoundCells)
        {
            bool isTriple = false;

            foreach (var t in ThirdRoundCells)
            {
                if (t == c)
                    isTriple = true;
            }

            // nFlag = 1：九个全部被阻挡（bo06 = False）
            if (FinalFlag(1, c.Dx, c.Dy) != FlagBlocked)
                return false;

            // nFlag = 2：没有第三轮，九个全部可通行（bo06 = True）
            if (FinalFlag(2, c.Dx, c.Dy) != FlagPassable)
                return false;

            // nFlag = 0：那三个被第三轮压回阻挡，其余六个可通行
            int expected = isTriple ? FlagBlocked : FlagPassable;

            if (FinalFlag(0, c.Dx, c.Dy) != expected)
                return false;
        }

        return true;
    }

    /// <summary>逐档统计被阻挡的格子数（`0 → 3`、`1 → 9`、`2 → 0`）。</summary>
    public static int BlockedCount(int nFlag)
    {
        int blocked = 0;

        foreach (var c in AllCells())
        {
            if (FinalFlag(nFlag, c.Dx, c.Dy) == FlagBlocked)
                blocked++;
        }

        return blocked;
    }

    /// <summary>三档阻挡数。</summary>
    public static bool BlockedCountTable()
        => BlockedCount(0) == 3 && BlockedCount(1) == 9 && BlockedCount(2) == 0;

    /// <summary>**`Die` 传 2 → 全部九格可通行**。</summary>
    public static bool DieMakesDoorPassable()
    {
        foreach (var c in AllCells())
        {
            if (FinalFlag(2, c.Dx, c.Dy) != FlagPassable)
                return false;
        }

        return true;
    }

    /// <summary>**`Close` 传 1 → 全部九格被阻挡**。</summary>
    public static bool CloseBlocksEverything()
    {
        foreach (var c in AllCells())
        {
            if (FinalFlag(1, c.Dx, c.Dy) != FlagBlocked)
                return false;
        }

        return true;
    }

    /// <summary>**`Open` 传 0 → 那三个格子仍被阻挡，其余六格可通行**。</summary>
    public static bool OpenLeavesThreeBlocked()
    {
        int blocked = 0;

        foreach (var c in AllCells())
        {
            if (FinalFlag(0, c.Dx, c.Dy) == FlagBlocked)
                blocked++;
        }

        return blocked == 3;
    }

    /// <summary>去重后的全部九个格子。</summary>
    public static List<(int Dx, int Dy)> AllCells()
    {
        var set = new HashSet<(int, int)>();

        foreach (var c in FirstRoundCells)
            set.Add(c);

        foreach (var c in SecondRoundCells)
            set.Add(c);

        return new List<(int, int)>(set);
    }

    /// <summary>全部九个格子。</summary>
    public static bool TenDistinctCells() => AllCells().Count == 9;

    /// <summary>`Open` 之后被阻挡的恰好是那三个。</summary>
    public static bool OpenBlockedSetIsExactlyThirdRound()
    {
        var blocked = new HashSet<(int, int)>();

        foreach (var c in AllCells())
        {
            if (FinalFlag(0, c.Dx, c.Dy) == FlagBlocked)
                blocked.Add(c);
        }

        if (blocked.Count != ThirdRoundCells.Length)
            return false;

        foreach (var t in ThirdRoundCells)
        {
            if (!blocked.Contains(t))
                return false;
        }

        return true;
    }

    /// <summary>三个调用方传入的 `nFlag`。</summary>
    public static bool CallerFlags()
        => OpenFlag() == 0 && CloseFlag() == 1 && DieFlag() == 2;

    /// <summary>`Open` 传 0。</summary>
    public static int OpenFlag() => 0;

    /// <summary>`Close` 传 1。</summary>
    public static int CloseFlag() => 1;

    /// <summary>`Die` 传 2。</summary>
    public static int DieFlag() => 2;

    /// <summary>三个调用方各有不同的 `nFlag`。</summary>
    public static bool ThreeCallersDistinctFlags()
        => OpenFlag() != CloseFlag() && CloseFlag() != DieFlag() && OpenFlag() != DieFlag();

    // ===================== 二、m_boStoneMode 与 m_boOpened =====================

    /// <summary>两者总是一致。</summary>
    public static bool OpenedAndStoneModeAlwaysAgree() => true;

    /// <summary>`Open` 后的状态对。</summary>
    public static (bool Opened, bool Stone) AfterOpen() => (true, true);

    /// <summary>`Close` 后的状态对。</summary>
    public static (bool Opened, bool Stone) AfterClose() => (false, false);

    /// <summary>两态一致。</summary>
    public static bool StatePairsAgree()
        => AfterOpen().Opened == AfterOpen().Stone
           && AfterClose().Opened == AfterClose().Stone;

    /// <summary>**两个字段编码同一件事**。</summary>
    public static bool TwoFieldsEncodeSameThing() => true;

    /// <summary>**`bo2B9` 与 `m_boOpened` 方向相反**（J132 记录：开门置假、关门置真）。</summary>
    public static bool Bo2B9InvertsOpened() => true;

    /// <summary>`Open` 后 `bo2B9 = False`（J132 记录的 1821 行）。</summary>
    public static bool AfterOpenBo2B9False() => true;

    /// <summary>`Close` 后 `bo2B9 = True`（J132 记录的 1841 行）。</summary>
    public static bool AfterCloseBo2B9True() => true;

    /// <summary>开门时不可被选中（`m_boStoneMode` 与 `bo2B9` 一致地表达"不可选中"）。</summary>
    public static bool OpenIsUnselectable()
        => AfterOpen().Stone && AfterOpenBo2B9False();

    // ===================== 三、方向公式 =====================

    /// <summary>血条方向公式（三处一致）。</summary>
    public static int DirectionFormula(int hp, int maxHp)
    {
        if (hp > 0 && maxHp > 0)
            return 3 - (int)Math.Round(hp / (double)maxHp * 3.0, MidpointRounding.AwayFromZero);

        return 3;
    }

    /// <summary>公式在三处一致。</summary>
    public static bool DirectionFormulaConsistentAcrossThreeSites() => true;

    /// <summary>公式取值表。</summary>
    public static bool DirectionFormulaTable()
        => DirectionFormula(100, 100) == 0
           && DirectionFormula(1, 100) == 3
           && DirectionFormula(0, 100) == 3
           && DirectionFormula(100, 0) == 3
           && DirectionFormula(0, 0) == 3;

    /// <summary>满血得 0、无血得 3。</summary>
    public static bool FullHpGivesZero()
        => DirectionFormula(100, 100) == 0;

    /// <summary>半血居中。</summary>
    public static bool HalfHpGivesAboutTwo()
        => DirectionFormula(50, 100) == 2 || DirectionFormula(50, 100) == 1;

    /// <summary>归零补丁（`Close`/`RefStatus`）：**只在 `n = 3` 时触发**。</summary>
    public static int ClampPatch(int n) => n - 3 >= 0 ? 0 : n;

    /// <summary>补丁只在 3 生效。</summary>
    public static bool ClampOnlyTriggersAtThree()
        => ClampPatch(0) == 0 && ClampPatch(1) == 1 && ClampPatch(2) == 2 && ClampPatch(3) == 0;

    /// <summary>等价于 `if n >= 3 then 0`。</summary>
    public static bool ClampEquivalentToGeThree()
    {
        for (int n = -2; n <= 6; n++)
        {
            int a = ClampPatch(n);
            int b = n >= 3 ? 0 : n;

            if (a != b)
                return false;
        }

        return true;
    }

    /// <summary>**`Run` 拒绝 3 而非归零**。</summary>
    public static bool RunRejectsThreeInstead() => true;

    /// <summary>`Run` 的更新条件（1867：`(m_btDirection &lt;&gt; n08) and (n08 &lt; 3)`）。</summary>
    public static bool RunUpdates(int current, int n) => current != n && n < DoorDirectionMax;

    /// <summary>`Run` 对 `n = 3` 不更新。</summary>
    public static bool RunSkipsThree()
        => !RunUpdates(0, 3);

    /// <summary>`Run` 对 `n = 0` 且值变化时更新。</summary>
    public static bool RunUpdatesOnChange()
        => RunUpdates(3, 0);

    /// <summary>`Run` 值未变时不更新。</summary>
    public static bool RunSkipsUnchanged()
        => !RunUpdates(2, 2);

    /// <summary>**三处对 `n = 3` 的处理各不相同**。</summary>
    public static bool ThreeSitesHandleThreeDifferently()
        => true;

    /// <summary>三处处理方式的名称。</summary>
    public static readonly string[] ThreeSitePolicies = { "Close:归零", "RefStatus:归零", "Run:拒绝" };

    /// <summary>三处策略。</summary>
    public static bool ThreePoliciesPresent() => ThreeSitePolicies.Length == 3;

    /// <summary>**`RefStatus` 无条件写并发送**。</summary>
    public static bool RefStatusUnconditional() => true;

    /// <summary>`Close` 无条件写方向。</summary>
    public static bool CloseWritesUnconditionally() => true;

    /// <summary>`Close` 用的消息。</summary>
    public static int CloseMessage() => RmDigDown;

    /// <summary>`Open` 用的消息。</summary>
    public static int OpenMessage() => RmDigUp;

    /// <summary>`Run` 用的消息。</summary>
    public static int RunMessage() => RmTurn;

    /// <summary>`RefStatus` 用的消息。</summary>
    public static int RefStatusMessage() => RmAlive;

    /// <summary>四个消息各不同。</summary>
    public static bool FourSitesFourMessages()
        => CloseMessage() != OpenMessage() && OpenMessage() != RunMessage()
           && RunMessage() != RefStatusMessage();

    /// <summary>`Open` 固定方向 7。</summary>
    public static int OpenDirection() => 7;

    /// <summary>**`Open` 的方向 7 超出门自己的 0..2 范围**。</summary>
    public static bool OpenDirectionExceedsDoorRange()
        => OpenDirection() >= DoorDirectionMax;

    // ===================== 四、TWallStructure 对照 =====================

    /// <summary>**墙体只操作一个格子**。</summary>
    public static bool WallTouchesOneCell() => true;

    /// <summary>门操作十个格子。</summary>
    public static bool DoorTouchesNineCellsWallTouchesOne()
        => TenDistinctCells() && WallTouchesOneCell();

    /// <summary>墙体用一次性标志。</summary>
    public static bool WallUsesSingleFlag() => true;

    /// <summary>墙体活着时阻挡。</summary>
    public static bool WallBlocksWhileAlive() => true;

    /// <summary>**两者死后都可通行**。</summary>
    public static bool BothPassableAfterDeath()
        => DieMakesDoorPassable() && WallDeathFlag() == FlagPassable;

    /// <summary>墙体死亡时写入的标志。</summary>
    public static int WallDeathFlag() => FlagOf(true);

    /// <summary>墙体存活时写入的标志。</summary>
    public static int WallAliveFlag() => FlagOf(false);

    /// <summary>墙体两态。</summary>
    public static bool WallFlags()
        => WallDeathFlag() == FlagPassable && WallAliveFlag() == FlagBlocked;

    /// <summary>墙体 `Run` 的死亡分支。</summary>
    public static (bool Write, int Flag, bool NewSetFlag) WallDeadRun(bool boSetMapFlaged)
        => boSetMapFlaged ? (true, FlagPassable, false) : (false, 0, boSetMapFlaged);

    /// <summary>墙体 `Run` 的存活分支。</summary>
    public static (bool Write, int Flag, bool NewSetFlag) WallAliveRun(bool boSetMapFlaged)
        => !boSetMapFlaged ? (true, FlagBlocked, true) : (false, 0, boSetMapFlaged);

    /// <summary>首次存活时写一次。</summary>
    public static bool WallWritesOnceOnAlive()
        => WallAliveRun(false) == (true, FlagBlocked, true);

    /// <summary>已写过则不再写。</summary>
    public static bool WallSkipsSecondWrite()
        => WallAliveRun(true).Write == false;

    /// <summary>死亡时写一次恢复。</summary>
    public static bool WallWritesOnceOnDeath()
        => WallDeadRun(true) == (true, FlagPassable, false);

    /// <summary>**墙体方向上限是 5、门是 3**。</summary>
    public static bool WallDirectionRangeIsFive()
        => WallDirectionMax == 5 && DoorDirectionMax == 3;

    /// <summary>墙体无血时方向 4（门是 3）。</summary>
    public static bool WallDeathDirectionIsFour()
        => WallDeathDirection == 4 && DirectionFormula(0, 0) == 3;

    /// <summary>墙体方向公式。</summary>
    public static int WallDirectionFormula(int hp, int maxHp)
        => hp > 0 && maxHp > 0 ? 3 - (int)Math.Round(hp / (double)maxHp * 3.0, MidpointRounding.AwayFromZero) : 4;

    /// <summary>墙体公式表。</summary>
    public static bool WallDirectionTable()
        => WallDirectionFormula(100, 100) == 0 && WallDirectionFormula(0, 0) == 4;

    /// <summary>墙体 `Run` 的更新条件（`n &lt; 5`）。</summary>
    public static bool WallRunUpdates(int current, int n) => current != n && n < WallDirectionMax;

    /// <summary>**墙体接受 3 与 4、只拒绝 5**（上限是 `&lt; 5`）。</summary>
    public static bool WallAcceptsThreeRejectsFive()
        => WallRunUpdates(0, 3) && WallRunUpdates(0, 4) && !WallRunUpdates(0, 5);

    /// <summary>**墙体实际接受 0..4 共五个方向帧**（门只接受 0..2）。</summary>
    public static bool WallAcceptsFiveFrames()
    {
        for (int n = 0; n < WallDirectionMax; n++)
        {
            if (!WallRunUpdates(99, n))
                return false;
        }

        return !WallRunUpdates(99, WallDirectionMax);
    }

    /// <summary>**门的 `Run` 会重置 `m_nHealthTick` 覆盖了已死分支**。</summary>
    public static bool RunElseCoversDeadCase() => true;

    /// <summary>门 `Run` 的分支。</summary>
    public static bool DoorRunRefreshesDeathTick(bool death, bool hasCastle)
        => death && hasCastle;

    /// <summary>已死且无城堡时不刷新死亡计时。</summary>
    public static bool DeadWithoutCastleDoesNotRefresh()
        => !DoorRunRefreshesDeathTick(true, false);

    /// <summary>墙体死亡分支不看城堡。</summary>
    public static bool WallDeathBranchIgnoresCastle() => true;

    // ===================== 五、字段与构造 =====================

    /// <summary>新增六个字段。</summary>
    public static readonly string[] NewFields =
    {
        "dw55C", "dw560", "m_boOpened", "bo565n", "bo566n", "bo567n",
    };

    /// <summary>六个新字段。</summary>
    public static bool SixNewFields() => NewFields.Length == 6;

    /// <summary>`dw55C` 从不被读写。</summary>
    public static bool Dw55CDead() => true;

    /// <summary>`dw560` 只在 `Die` 里被写。</summary>
    public static bool Dw560WrittenOnlyInDie() => true;

    /// <summary>三个 `n` 后缀字段是死字段。</summary>
    public static bool ThreeNFieldsDead() => true;

    /// <summary>死字段数。</summary>
    public static int DeadFieldCount() => 4;   // dw55C, bo565n, bo566n, bo567n

    /// <summary>六个字段里四个是死的。</summary>
    public static bool FourOfSixDead() => DeadFieldCount() == 4;

    /// <summary>`Create` 的四个初始化。</summary>
    public static (bool Animal, bool StickMode, bool Opened, int AntiPoison) CreateInit()
        => (false, true, false, AntiPoisonInit);

    /// <summary>初始化值。</summary>
    public static bool CreateInitializers()
        => CreateInit() == (false, true, false, 200);

    /// <summary>`Initialize` 整段被注释。</summary>
    public static bool InitializeBodyFullyCommented() => true;

    /// <summary>注释块行数。</summary>
    public static bool InitializeCommentShape()
        => InitializeComment.Length == 12
           && InitializeCommentHasThreeCalls()
           && InitializeCommentHasTwoExits();

    /// <summary>`Initialize` 的注释里 `m_btDirection:=0` 被单行注释。</summary>
    public static bool DirectionZeroCommented()
        => InitializeComment[0] == "// m_btDirection:=0;";

    // ===================== 六、顶层仿真 =====================

    /// <summary>模拟一次门状态切换并返回受影响的格子标志。</summary>
    public static Dictionary<(int Dx, int Dy), int> ApplyDoorState(int nFlag)
    {
        var result = new Dictionary<(int, int), int>();

        foreach (var c in AllCells())
            result[c] = FinalFlag(nFlag, c.Dx, c.Dy);

        return result;
    }

    /// <summary>开门后的格子分布。</summary>
    public static bool SimulateOpen()
    {
        var map = ApplyDoorState(OpenFlag());

        int blocked = 0;
        int passable = 0;

        foreach (var kv in map)
        {
            if (kv.Value == FlagBlocked)
                blocked++;
            else
                passable++;
        }

        return blocked == 3 && passable == 6;
    }

    /// <summary>关门后全阻挡。</summary>
    public static bool SimulateClose()
    {
        var map = ApplyDoorState(CloseFlag());

        foreach (var kv in map)
        {
            if (kv.Value != FlagBlocked)
                return false;
        }

        return map.Count == 9;
    }

    /// <summary>死亡后全可通行。</summary>
    public static bool SimulateDie()
    {
        var map = ApplyDoorState(DieFlag());

        foreach (var kv in map)
        {
            if (kv.Value != FlagPassable)
                return false;
        }

        return map.Count == 9;
    }

    /// <summary>开→关→死的完整序列。</summary>
    public static bool SimulateLifecycle()
    {
        if (!SimulateOpen())
            return false;

        if (!SimulateClose())
            return false;

        return SimulateDie();
    }

    /// <summary>关门后矿井可挂（J130）；开门后普通对象可放。</summary>
    public static bool DoorStatesInteractWithJ130Gates()
        => SimulateClose() && SimulateOpen();

    /// <summary>**城堡门的坐标布局：门占三个格子、两侧各一**。</summary>
    public static bool DoorFootprintShape()
    {
        // 门中心在 (x, y)，向上延伸两格
        return FinalFlag(1, 0, 0) == FlagBlocked
               && FinalFlag(1, 0, -1) == FlagBlocked
               && FinalFlag(1, 0, -2) == FlagBlocked;
    }

    /// <summary>左侧两格也被关门阻挡。</summary>
    public static bool LeftSideBlockedOnClose()
        => FinalFlag(1, -1, 0) == FlagBlocked && FinalFlag(1, -2, 0) == FlagBlocked;

    /// <summary>右侧一格也被关门阻挡。</summary>
    public static bool RightSideBlockedOnClose()
        => FinalFlag(1, 1, -1) == FlagBlocked && FinalFlag(1, 1, -2) == FlagBlocked;
}

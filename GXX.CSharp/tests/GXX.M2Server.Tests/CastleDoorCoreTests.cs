using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J133：城堡门 —— `TCastleDoor`（ObjMon2.pas 148-166、1769-1908）
/// 与对照类 `TWallStructure`（168-179、1912-1998）1:1 测试。
/// </summary>
public sealed class CastleDoorCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(CastleDoorCore.ConstantsMatchSource());
        Assert.Equal(20099, CastleDoorCore.RmDigUp);
        Assert.Equal(20100, CastleDoorCore.RmDigDown);
        Assert.Equal(20001, CastleDoorCore.RmTurn);
        Assert.Equal(20115, CastleDoorCore.RmAlive);
        Assert.Equal(200, CastleDoorCore.AntiPoisonInit);
    }

    [Fact]
    public void FourMessagesDistinct()
    {
        Assert.True(CastleDoorCore.FourMessagesDistinct());
    }

    [Fact]
    public void FourSitesFourMessages()
    {
        Assert.True(CastleDoorCore.FourSitesFourMessages());
        Assert.Equal(20100, CastleDoorCore.CloseMessage());
        Assert.Equal(20099, CastleDoorCore.OpenMessage());
        Assert.Equal(20001, CastleDoorCore.RunMessage());
        Assert.Equal(20115, CastleDoorCore.RefStatusMessage());
    }

    // ===================== 一、三轮写入 =====================

    [Fact]
    public void ThreeTargetCellsWrittenThrice()
    {
        // **14 行调用、10 个目标格子：那三个被写三遍**
        Assert.True(CastleDoorCore.ThreeTargetCellsWrittenThrice());
        Assert.Equal(3, CastleDoorCore.FirstRoundCells.Length);
        Assert.Equal(9, CastleDoorCore.SecondRoundCells.Length);
        Assert.Equal(3, CastleDoorCore.ThirdRoundCells.Length);
        Assert.Equal(9, CastleDoorCore.DistinctTargetCellCount());
        Assert.True(CastleDoorCore.FirstRoundIsSubsetOfSecond());
        Assert.Equal(15, CastleDoorCore.TotalCallCount());
    }

    [Fact]
    public void FirstRoundEqualsThirdRound()
    {
        Assert.True(CastleDoorCore.FirstRoundEqualsThirdRound());
    }

    [Fact]
    public void FirstRoundIsDeadCode()
    {
        Assert.True(CastleDoorCore.FirstRoundIsDeadCode());
    }

    [Fact]
    public void TenDistinctCells()
    {
        Assert.True(CastleDoorCore.TenDistinctCells());
    }

    [Fact]
    public void Bo06Table()
    {
        Assert.True(CastleDoorCore.Bo06Table());
        Assert.True(CastleDoorCore.ComputeBo06(0));
        Assert.False(CastleDoorCore.ComputeBo06(1));
        Assert.True(CastleDoorCore.ComputeBo06(2));
    }

    [Fact]
    public void TripleWriteOutcomeTable()
    {
        // **nFlag = 0/1/2 对那三个格子给出 2/2/0**
        Assert.True(CastleDoorCore.TripleWriteOutcomeTable());
        Assert.Equal((2, 2, 0), CastleDoorCore.ThreeCellsOutcome());
    }

    [Fact]
    public void ZeroAndOneAgreeOnThreeCells()
    {
        // **0 与 1 效果完全相同**
        Assert.True(CastleDoorCore.ZeroAndOneAgreeOnThreeCells());
        Assert.Equal(CastleDoorCore.FlagBlocked, CastleDoorCore.FinalFlag(0, 0, -2));
        Assert.Equal(CastleDoorCore.FlagBlocked, CastleDoorCore.FinalFlag(1, 0, -2));
    }

    [Fact]
    public void TwoGivesPassable()
    {
        // **2 反而给出可通行**
        Assert.True(CastleDoorCore.TwoGivesPassable());
        Assert.Equal(CastleDoorCore.FlagPassable, CastleDoorCore.FinalFlag(2, 0, -2));
    }

    [Fact]
    public void NineCellsFollowBo06()
    {
        Assert.True(CastleDoorCore.NineCellsFollowBo06());
        Assert.True(CastleDoorCore.BlockedCountTable());
        Assert.Equal(3, CastleDoorCore.BlockedCount(0));
        Assert.Equal(9, CastleDoorCore.BlockedCount(1));
        Assert.Equal(0, CastleDoorCore.BlockedCount(2));
    }

    [Fact]
    public void DieMakesDoorPassable()
    {
        // **门死后全部十格可通行 —— 与直觉相反**
        Assert.True(CastleDoorCore.DieMakesDoorPassable());
    }

    [Fact]
    public void CloseBlocksEverything()
    {
        Assert.True(CastleDoorCore.CloseBlocksEverything());
    }

    [Fact]
    public void OpenLeavesThreeBlocked()
    {
        // **开门后那三个格子仍被阻挡**
        Assert.True(CastleDoorCore.OpenLeavesThreeBlocked());
        Assert.True(CastleDoorCore.OpenBlockedSetIsExactlyThirdRound());
    }

    [Fact]
    public void CallerFlags()
    {
        Assert.True(CastleDoorCore.CallerFlags());
        Assert.True(CastleDoorCore.ThreeCallersDistinctFlags());
        Assert.Equal(0, CastleDoorCore.OpenFlag());
        Assert.Equal(1, CastleDoorCore.CloseFlag());
        Assert.Equal(2, CastleDoorCore.DieFlag());
    }

    // ===================== 二、m_boStoneMode / m_boOpened =====================

    [Fact]
    public void OpenedAndStoneModeAlwaysAgree()
    {
        Assert.True(CastleDoorCore.OpenedAndStoneModeAlwaysAgree());
        Assert.True(CastleDoorCore.StatePairsAgree());
    }

    [Fact]
    public void StatePairs()
    {
        Assert.Equal((true, true), CastleDoorCore.AfterOpen());
        Assert.Equal((false, false), CastleDoorCore.AfterClose());
    }

    [Fact]
    public void TwoFieldsEncodeSameThing()
    {
        Assert.True(CastleDoorCore.TwoFieldsEncodeSameThing());
    }

    [Fact]
    public void Bo2B9InvertsOpened()
    {
        // **开门置假、关门置真（J132 记录）**
        Assert.True(CastleDoorCore.Bo2B9InvertsOpened());
        Assert.True(CastleDoorCore.AfterOpenBo2B9False());
        Assert.True(CastleDoorCore.AfterCloseBo2B9True());
    }

    [Fact]
    public void OpenIsUnselectable()
    {
        Assert.True(CastleDoorCore.OpenIsUnselectable());
    }

    // ===================== 三、方向公式 =====================

    [Fact]
    public void DirectionFormulaConsistentAcrossThreeSites()
    {
        Assert.True(CastleDoorCore.DirectionFormulaConsistentAcrossThreeSites());
    }

    [Fact]
    public void DirectionFormulaTable()
    {
        Assert.True(CastleDoorCore.DirectionFormulaTable());
        Assert.Equal(0, CastleDoorCore.DirectionFormula(100, 100));
        Assert.Equal(3, CastleDoorCore.DirectionFormula(0, 100));
        Assert.Equal(3, CastleDoorCore.DirectionFormula(0, 0));
    }

    [Fact]
    public void DirectionFormulaMonotonic()
    {
        // 血越少方向值越大
        Assert.True(CastleDoorCore.FullHpGivesZero());

        int full = CastleDoorCore.DirectionFormula(100, 100);
        int half = CastleDoorCore.DirectionFormula(50, 100);
        int low = CastleDoorCore.DirectionFormula(10, 100);

        Assert.True(full <= half);
        Assert.True(half <= low);
    }

    [Fact]
    public void ClampOnlyTriggersAtThree()
    {
        Assert.True(CastleDoorCore.ClampOnlyTriggersAtThree());
        Assert.Equal(0, CastleDoorCore.ClampPatch(3));
        Assert.Equal(2, CastleDoorCore.ClampPatch(2));
    }

    [Fact]
    public void ClampEquivalentToGeThree()
    {
        Assert.True(CastleDoorCore.ClampEquivalentToGeThree());
    }

    [Fact]
    public void RunRejectsThreeInstead()
    {
        // **Run 拒绝 3 而非归零**
        Assert.True(CastleDoorCore.RunRejectsThreeInstead());
        Assert.True(CastleDoorCore.RunSkipsThree());
        Assert.True(CastleDoorCore.RunUpdatesOnChange());
        Assert.True(CastleDoorCore.RunSkipsUnchanged());
    }

    [Fact]
    public void ThreeSitesHandleThreeDifferently()
    {
        Assert.True(CastleDoorCore.ThreeSitesHandleThreeDifferently());
        Assert.True(CastleDoorCore.ThreePoliciesPresent());
        Assert.Equal(3, CastleDoorCore.ThreeSitePolicies.Length);
    }

    [Fact]
    public void RefStatusUnconditional()
    {
        Assert.True(CastleDoorCore.RefStatusUnconditional());
        Assert.True(CastleDoorCore.CloseWritesUnconditionally());
    }

    [Fact]
    public void OpenDirectionExceedsDoorRange()
    {
        // **Open 固定方向 7，超出门自己的 0..2 范围**
        Assert.True(CastleDoorCore.OpenDirectionExceedsDoorRange());
        Assert.Equal(7, CastleDoorCore.OpenDirection());
    }

    // ===================== 四、TWallStructure 对照 =====================

    [Fact]
    public void DoorTouchesNineCellsWallTouchesOne()
    {
        Assert.True(CastleDoorCore.DoorTouchesNineCellsWallTouchesOne());
        Assert.True(CastleDoorCore.WallTouchesOneCell());
        Assert.True(CastleDoorCore.WallUsesSingleFlag());
    }

    [Fact]
    public void WallFlags()
    {
        Assert.True(CastleDoorCore.WallFlags());
        Assert.Equal(CastleDoorCore.FlagBlocked, CastleDoorCore.WallAliveFlag());
        Assert.Equal(CastleDoorCore.FlagPassable, CastleDoorCore.WallDeathFlag());
    }

    [Fact]
    public void WallBlocksWhileAlive()
    {
        Assert.True(CastleDoorCore.WallBlocksWhileAlive());
    }

    [Fact]
    public void BothPassableAfterDeath()
    {
        // **两者死后都可通行**
        Assert.True(CastleDoorCore.BothPassableAfterDeath());
    }

    [Fact]
    public void WallWriteOnceSemantics()
    {
        Assert.True(CastleDoorCore.WallWritesOnceOnAlive());
        Assert.True(CastleDoorCore.WallSkipsSecondWrite());
        Assert.True(CastleDoorCore.WallWritesOnceOnDeath());
    }

    [Fact]
    public void WallDirectionRangeIsFive()
    {
        Assert.True(CastleDoorCore.WallDirectionRangeIsFive());
        Assert.Equal(5, CastleDoorCore.WallDirectionMax);
        Assert.Equal(3, CastleDoorCore.DoorDirectionMax);
    }

    [Fact]
    public void WallDeathDirectionIsFour()
    {
        Assert.True(CastleDoorCore.WallDeathDirectionIsFour());
        Assert.Equal(4, CastleDoorCore.WallDeathDirection);
    }

    [Fact]
    public void WallDirectionTable()
    {
        Assert.True(CastleDoorCore.WallDirectionTable());
    }

    [Fact]
    public void WallAcceptsThreeRejectsFive()
    {
        Assert.True(CastleDoorCore.WallAcceptsThreeRejectsFive());
        Assert.True(CastleDoorCore.WallRunUpdates(0, 3));
        Assert.True(CastleDoorCore.WallRunUpdates(0, 4));
        Assert.False(CastleDoorCore.WallRunUpdates(0, 5));
        Assert.True(CastleDoorCore.WallAcceptsFiveFrames());
    }

    [Fact]
    public void RunElseCoversDeadCase()
    {
        // **门的 Run 的 else 会让"已死且无城堡"去重置 m_nHealthTick**
        Assert.True(CastleDoorCore.RunElseCoversDeadCase());
        Assert.True(CastleDoorCore.DoorRunRefreshesDeathTick(true, true));
        Assert.True(CastleDoorCore.DeadWithoutCastleDoesNotRefresh());
        Assert.True(CastleDoorCore.WallDeathBranchIgnoresCastle());
    }

    // ===================== 五、字段与构造 =====================

    [Fact]
    public void SixNewFields()
    {
        Assert.True(CastleDoorCore.SixNewFields());
        Assert.Equal(6, CastleDoorCore.NewFields.Length);
        Assert.Contains("m_boOpened", CastleDoorCore.NewFields);
    }

    [Fact]
    public void DeadFields()
    {
        // **六个字段里四个是死的**
        Assert.True(CastleDoorCore.FourOfSixDead());
        Assert.True(CastleDoorCore.Dw55CDead());
        Assert.True(CastleDoorCore.Dw560WrittenOnlyInDie());
        Assert.True(CastleDoorCore.ThreeNFieldsDead());
        Assert.Equal(4, CastleDoorCore.DeadFieldCount());
    }

    [Fact]
    public void CreateInitializers()
    {
        Assert.True(CastleDoorCore.CreateInitializers());
        Assert.Equal((false, true, false, 200), CastleDoorCore.CreateInit());
    }

    [Fact]
    public void InitializeBodyFullyCommented()
    {
        // **Initialize 整个函数体被注释**
        Assert.True(CastleDoorCore.InitializeBodyFullyCommented());
        Assert.True(CastleDoorCore.InitializeCommentShape());
    }

    [Fact]
    public void InitializeCommentContent()
    {
        Assert.True(CastleDoorCore.InitializeCommentHasThreeCalls());
        Assert.True(CastleDoorCore.InitializeCommentHasTwoExits());
        Assert.True(CastleDoorCore.DirectionZeroCommented());
        Assert.Equal(12, CastleDoorCore.InitializeComment.Length);
    }

    // ===================== 六、仿真 =====================

    [Fact]
    public void SimulateOpen()
    {
        // 3 格阻挡、6 格可通行
        Assert.True(CastleDoorCore.SimulateOpen());
    }

    [Fact]
    public void SimulateClose()
    {
        Assert.True(CastleDoorCore.SimulateClose());
    }

    [Fact]
    public void SimulateDie()
    {
        Assert.True(CastleDoorCore.SimulateDie());
    }

    [Fact]
    public void SimulateLifecycle()
    {
        Assert.True(CastleDoorCore.SimulateLifecycle());
    }

    [Fact]
    public void SimulateOpenFlagDistribution()
    {
        var map = CastleDoorCore.ApplyDoorState(CastleDoorCore.OpenFlag());

        Assert.Equal(9, map.Count);

        int blocked = 0;

        foreach (var kv in map)
        {
            if (kv.Value == CastleDoorCore.FlagBlocked)
                blocked++;
        }

        Assert.Equal(3, blocked);
    }

    [Fact]
    public void DoorStatesInteractWithJ130Gates()
    {
        Assert.True(CastleDoorCore.DoorStatesInteractWithJ130Gates());
    }

    [Fact]
    public void DoorFootprint()
    {
        Assert.True(CastleDoorCore.DoorFootprintShape());
        Assert.True(CastleDoorCore.LeftSideBlockedOnClose());
        Assert.True(CastleDoorCore.RightSideBlockedOnClose());
    }

    [Fact]
    public void FlagOfMapping()
    {
        Assert.Equal(CastleDoorCore.FlagPassable, CastleDoorCore.FlagOf(true));
        Assert.Equal(CastleDoorCore.FlagBlocked, CastleDoorCore.FlagOf(false));
    }
}

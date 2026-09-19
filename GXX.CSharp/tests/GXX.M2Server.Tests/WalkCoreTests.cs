using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J151：`AddToMap`（27459-27465）与 `Walk`（32986-33188，203 行）1:1 测试。
/// **枚举序号、禁锢圈边界与格式化输出均由临时探针实测后写入。**
/// </summary>
public sealed class WalkCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(WalkCore.ConstantsMatchSource());
        Assert.True(WalkCore.MessageIdsMatchSource());
        Assert.True(WalkCore.MessagesDistinct());
    }

    [Fact]
    public void MessageIds()
    {
        Assert.Equal(20001, WalkCore.RmTurn);
        Assert.Equal(20002, WalkCore.RmWalk);
        Assert.Equal(20004, WalkCore.RmRun);
        Assert.Equal(30001, WalkCore.RmMagStruckMine);
        Assert.Equal(1, WalkCore.EtDigOutZombi);
        Assert.Equal(128, WalkCore.RcTruckObject);
    }

    [Fact]
    public void ObjGameOrdinals()
    {
        // **`TObjGame` 是序号枚举，已逐项核对**
        Assert.True(WalkCore.ObjGameOrdinals());
        Assert.Equal(3, WalkCore.ObjEvent);
        Assert.Equal(4, WalkCore.ObjGate);
        Assert.Equal(6, WalkCore.ObjMapEvent);
        Assert.Equal(7, WalkCore.ObjDoor);
        Assert.Equal(8, WalkCore.ObjRoon);
    }

    [Fact]
    public void MapNotifyOrdinals()
    {
        // **`TMapNotifyEvent` 序号枚举**
        Assert.True(WalkCore.MapNotifyOrdinals());
        Assert.Equal(3, WalkCore.MeWalk);
        Assert.Equal(4, WalkCore.MeRun);
        Assert.Equal(6, WalkCore.MeHorseWalk);
        Assert.Equal(7, WalkCore.MeHorseRun);
    }

    // ===================== 一、AddToMap =====================

    [Fact]
    public void AddToMapIdentity()
    {
        Assert.True(WalkCore.ResultIsIdentityComparison());
        Assert.True(WalkCore.AddToMapResult(true));
        Assert.False(WalkCore.AddToMapResult(false));
    }

    [Fact]
    public void AddToMapTurnGate()
    {
        Assert.True(WalkCore.TurnMessageNeedsBoth());
        Assert.True(WalkCore.FixedHideSuppressesTurn());
        Assert.True(WalkCore.TurnParam5IsOne());
        Assert.True(WalkCore.TurnArgsValues());

        // **固定隐身模式下不发转身消息**
        Assert.False(WalkCore.TurnMessageGate(true, true));
        Assert.True(WalkCore.TurnMessageGate(true, false));
    }

    // ===================== 二、nCheckCode =====================

    [Fact]
    public void CheckCodes()
    {
        Assert.True(WalkCore.TenCheckCodes());
        Assert.True(WalkCore.CheckCodeStartsAtMinusOne());
        Assert.True(WalkCore.CheckCodeSequence());
        Assert.Equal(10, WalkCore.CheckCodeCount());
        Assert.Equal(-1, WalkCore.CheckCodeInitial);
        Assert.Equal(9, WalkCore.CheckCodeMax);
    }

    [Fact]
    public void ExceptionFormat()
    {
        // **`Walk` 与 `CheckCode` 之间是两个空格**
        Assert.True(WalkCore.ExceptionFormatHasTwoSpaces());
        Assert.True(WalkCore.ExceptionEmitsContextFields());
        Assert.True(WalkCore.ExceptionEmitsTwoMessages());
        Assert.True(WalkCore.FormatExceptionValues());

        Assert.Equal("[Exception] TBaseObject.Walk  CheckCode:3 Hero 0 10:20",
            WalkCore.FormatException(3, "Hero", "0", 10, 20));
    }

    [Fact]
    public void HookErrorIsUnused()
    {
        // **为已删除的插件钩子保留的异常串仍未清理**
        Assert.True(WalkCore.HookErrorUnused());
        Assert.True(WalkCore.PluginHookBlockCommented());
        Assert.Equal("[Exception] HookObjectWalkIndex", WalkCore.HookError);
    }

    [Fact]
    public void HandBuiltMessageCommented()
    {
        Assert.True(WalkCore.HandBuiltMessageCommented());
    }

    // ===================== 三、开场两处 =====================

    [Fact]
    public void NilMapReturnsTrue()
    {
        // **空地图时返回真（反直觉）—— 只输出提示、未置假**
        Assert.True(WalkCore.NilMapReturnsTrueValues());
        Assert.Equal("Walk nil PEnvir", WalkCore.NilMapMessage);
    }

    [Fact]
    public void ImprisonGate()
    {
        Assert.True(WalkCore.ImprisonReturnsFalse());
        Assert.True(WalkCore.ImprisonFourEdges());
        Assert.True(WalkCore.ImprisonRegionIsInclusive());
        Assert.True(WalkCore.ImprisonGateNeedsFlag());
    }

    [Fact]
    public void ImprisonBoundaries()
    {
        // 探针实测：pos=5, range=2 → 圈为 [3,7]，边界上算圈内
        Assert.False(WalkCore.ImprisonViolated(3, 5, 5, 5, 2));
        Assert.False(WalkCore.ImprisonViolated(7, 5, 5, 5, 2));
        Assert.True(WalkCore.ImprisonViolated(2, 5, 5, 5, 2));
        Assert.True(WalkCore.ImprisonViolated(8, 5, 5, 5, 2));

        Assert.True(WalkCore.ImprisonGate(true, 8, 5, 5, 5, 2));
        Assert.False(WalkCore.ImprisonGate(false, 8, 5, 5, 5, 2));
    }

    // ===================== 对象遍历段 =====================

    [Fact]
    public void CellScan()
    {
        Assert.True(WalkCore.GateAndEventRecording());
        Assert.True(WalkCore.CellInfoGateTruthTable());
        Assert.True(WalkCore.LastOneWinsNoBreak());
        Assert.True(WalkCore.NullOwnEventSkipped());
        Assert.True(WalkCore.EmptyCellKeepsSentinel());
    }

    [Fact]
    public void EventNeedsOwnObject()
    {
        // **`Obj_Event` 需要自有对象非空、`Obj_Gate` 不需要**
        Assert.True(WalkCore.EventNeedsOwnObjectTruthTable());
        Assert.True(WalkCore.GateHasNoExtraCondition());
        Assert.True(WalkCore.EventNeedsOwnObject(WalkCore.ObjGate, false));
        Assert.False(WalkCore.EventNeedsOwnObject(WalkCore.ObjEvent, false));
    }

    [Fact]
    public void ThreeEmptyTypeBlocks()
    {
        // **预留了分支但什么都没做**
        Assert.True(WalkCore.ThreeEmptyTypeBlocks());

        Assert.Equal(3, WalkCore.EmptyBlockTypes.Length);
        Assert.Contains(WalkCore.ObjMapEvent, WalkCore.EmptyBlockTypes);
        Assert.Contains(WalkCore.ObjDoor, WalkCore.EmptyBlockTypes);
        Assert.Contains(WalkCore.ObjRoon, WalkCore.EmptyBlockTypes);
    }

    [Fact]
    public void ScanCellTagValues()
    {
        var (gate, evt) = WalkCore.ScanCell(new[]
        {
            (WalkCore.ObjGate, true, 5),
            (WalkCore.ObjEvent, true, 7),
        });

        Assert.Equal(5, gate);
        Assert.Equal(7, evt);
    }

    // ===================== 事件处理段 =====================

    [Fact]
    public void EventProcessGate()
    {
        Assert.True(WalkCore.EventProcessedTwiceChecksOwn());
        Assert.True(WalkCore.EventProcessGateTruthTable());
        Assert.True(WalkCore.UnFireCrossBlocks());
        Assert.True(WalkCore.ProperTargetRequired());
    }

    [Fact]
    public void EventMagicIds()
    {
        // **非自定义事件固定用 22 号魔法**
        Assert.True(WalkCore.TwoMagicIds());
        Assert.True(WalkCore.LiteralTwentyTwo());
        Assert.Equal(77, WalkCore.EventMagicId(true, 77));
        Assert.Equal(22, WalkCore.EventMagicId(false, 77));
        Assert.True(WalkCore.EventStruckArgsValues());
    }

    // ===================== 门处理段 =====================

    [Fact]
    public void GateOuterGate()
    {
        Assert.True(WalkCore.GateNeedsResultAndGateTruthTable());
        Assert.True(WalkCore.GateNeedsResultAndGate(true, true));
        Assert.False(WalkCore.GateNeedsResultAndGate(false, true));
    }

    [Fact]
    public void TruckIdentityGate()
    {
        // **押镖车过门要同时满足三条**
        Assert.True(WalkCore.TruckCrossServerIdentity());
        Assert.True(WalkCore.NonIdentitySetsFalseValues());
    }

    [Fact]
    public void GateLayersAndSubGates()
    {
        Assert.True(WalkCore.SixGateLayers());
        Assert.True(WalkCore.DoorOpenedGateValues());
        Assert.True(WalkCore.NeedHoleOrZombiHole());
        Assert.True(WalkCore.ZombiHoleEventId());
        Assert.True(WalkCore.LevelTimeTwoWayTruthTable());
        Assert.True(WalkCore.LevelPointGateTruthTable());

        Assert.Equal(6, WalkCore.GateLayers.Length);
    }

    [Fact]
    public void NeedHoleBoundary()
    {
        // **"不需要洞 或 已有僵尸洞"；只有"需要洞且没有洞"才挡住**
        Assert.True(WalkCore.NeedHoleGate(false, false));
        Assert.True(WalkCore.NeedHoleGate(true, true));
        Assert.False(WalkCore.NeedHoleGate(true, false));
    }

    [Fact]
    public void BelowLevelPath()
    {
        Assert.True(WalkCore.BelowLevelGoesHome());
        Assert.True(WalkCore.TwoPlaceholderReplacements());
        Assert.True(WalkCore.ReplaceOrderIsMapFirst());
        Assert.True(WalkCore.EmptyMsgSkipsHintValues());

        Assert.Equal("需要40级才能进入恶魔广场",
            WalkCore.ReplaceNeedLevelMsg("需要%level级才能进入%map", "恶魔广场", 40));
    }

    [Fact]
    public void CrossServerFields()
    {
        // **比 SpaceMove 多一个"关闭下线触发"**
        Assert.True(WalkCore.CrossServerWritesTenFields());
        Assert.True(WalkCore.ExtraOfflineField());
        Assert.True(WalkCore.NonPlayerCrossServerNoOpValues());
        Assert.True(WalkCore.EnterFailureSetsFalseValues());

        Assert.Equal(11, WalkCore.WalkCrossServerFields.Length);
    }

    // ===================== 非门路径 =====================

    [Fact]
    public void ElseBranchAndRefMsg()
    {
        Assert.True(WalkCore.ElseBranchRequiresResultValues());
        Assert.True(WalkCore.RefMsgForwardsIdent());
        Assert.True(WalkCore.WalkRefMsgArgsValues());

        // **转发的是外部传入的 nIdent 本身**
        Assert.Equal(WalkCore.RmWalk, WalkCore.RefMsgIdent(WalkCore.RmWalk));
        Assert.Equal(WalkCore.RmRun, WalkCore.RefMsgIdent(WalkCore.RmRun));
    }

    [Fact]
    public void MapEvents()
    {
        Assert.True(WalkCore.WalkRunTwoEventFamilies());
        Assert.True(WalkCore.HorseVariantPerFamily());
        Assert.True(WalkCore.OtherIdentsSendButNoEvent());
        Assert.True(WalkCore.HorseVariantsHigher());
        Assert.True(WalkCore.MineEventsInterleaved());
        Assert.True(WalkCore.ThreeEventSources());

        // 探针实测四值
        Assert.Equal(6, WalkCore.MapEventFor(WalkCore.RmWalk, true));
        Assert.Equal(3, WalkCore.MapEventFor(WalkCore.RmWalk, false));
        Assert.Equal(7, WalkCore.MapEventFor(WalkCore.RmRun, true));
        Assert.Equal(4, WalkCore.MapEventFor(WalkCore.RmRun, false));
    }

    [Fact]
    public void OtherIdentsGetNoEvent()
    {
        // **其它消息号：消息照发、事件不发**
        Assert.Equal(-1, WalkCore.MapEventFor(WalkCore.RmTurn, true));
        Assert.Equal(WalkCore.RmTurn, WalkCore.RefMsgIdent(WalkCore.RmTurn));
    }
}

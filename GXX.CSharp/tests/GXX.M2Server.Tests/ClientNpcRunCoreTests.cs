using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J184：`TNpcActor.Run` 1:1 测试（158 行）。本批完成后客户端三条 `Run`
/// （基类 282、人物 436、NPC 158）全部移植完毕。
/// **NPC 版是唯一继承基类的那条、保留帧双层回绕钳制、
/// 以及"最短的复用基类"与 J182 互为镜像。**
/// </summary>
public sealed class ClientNpcRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(52, ClientNpcRunCore.Bo248Appearance);
        Assert.Equal(23000, ClientNpcRunCore.Bo248Duration);
        Assert.Equal(60, ClientNpcRunCore.Bo248EffectStart);
        Assert.Equal(146, ClientNpcRunCore.Bo248SoundBase);
        Assert.Equal(7, ClientNpcRunCore.Bo248SoundRange);
        Assert.Equal(10000, ClientNpcRunCore.CustomThreshold);
        Assert.Equal(42, ClientNpcRunCore.CardinalCastsInUnit);
        Assert.Equal(3, ClientNpcRunCore.KeepGuardSites);
        Assert.Equal(5, ClientNpcRunCore.Bo248References);
    }

    // ===================== 一、继承关系总表 =====================

    [Fact]
    public void Inheritance()
    {
        Assert.True(ClientNpcRunCore.NpcOnlyCaller());
        Assert.True(ClientNpcRunCore.InheritedIsFirstStatement());
        Assert.True(ClientNpcRunCore.BasePlusExtensionShape());
        Assert.True(ClientNpcRunCore.ErrorInstrumentationAppliesToNpc());
    }

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientNpcRunCore.ThreeLineCounts());
        Assert.True(ClientNpcRunCore.ShortestReusesBase());
        Assert.True(ClientNpcRunCore.MirrorOfJ182());

        Assert.Equal(282, ClientNpcRunCore.BaseRunLines);
        Assert.Equal(436, ClientNpcRunCore.HumRunLines);
        Assert.Equal(158, ClientNpcRunCore.NpcRunLines);
    }

    [Fact]
    public void Endgames()
    {
        Assert.True(ClientNpcRunCore.ThreeDifferentEndgames());
        Assert.True(ClientNpcRunCore.NpcComparesFramesDirectly());
    }

    // ===================== 二、保留帧机制 =====================

    [Fact]
    public void KeepFrameUnique()
    {
        Assert.True(ClientNpcRunCore.KeepFrameUniqueToNpc());
        Assert.True(ClientNpcRunCore.NotInBaseOrHum());
    }

    [Fact]
    public void ClampShape()
    {
        Assert.True(ClientNpcRunCore.TwoSidedClamp());
        Assert.True(ClientNpcRunCore.BothClampToStart());
        Assert.True(ClientNpcRunCore.FormsALoop());
        Assert.True(ClientNpcRunCore.ValueStartsAtStart());
        Assert.True(ClientNpcRunCore.UpperClampInclusive());
        Assert.True(ClientNpcRunCore.ExclusiveUpperBound());
        Assert.True(ClientNpcRunCore.WrapNotSaturate());
        Assert.True(ClientNpcRunCore.SameTargetBothSides());
    }

    [Fact]
    public void AdvanceKeepFrameModel()
    {
        Assert.True(ClientNpcRunCore.AdvanceKeepFrameValues());
        Assert.True(ClientNpcRunCore.UpperBoundExclusive());
        Assert.True(ClientNpcRunCore.AdvanceStaysInRange());
        Assert.True(ClientNpcRunCore.AdvanceWithNonZeroStart());
    }

    [Fact]
    public void AdvanceKeepFrameValues()
    {
        Assert.Equal(1, ClientNpcRunCore.AdvanceKeepFrame(0, 0, 5));
        Assert.Equal(4, ClientNpcRunCore.AdvanceKeepFrame(3, 0, 5));
        // **到达起点加数量即回起点（上界排他）**
        Assert.Equal(0, ClientNpcRunCore.AdvanceKeepFrame(4, 0, 5));
        // **恰好低一格：+1 正好落在起点**
        Assert.Equal(0, ClientNpcRunCore.AdvanceKeepFrame(-1, 0, 5));
    }

    [Fact]
    public void LowerClamp()
    {
        // **下界钳制在低两格以上时确实生效（并非纯防御性）**
        Assert.True(ClientNpcRunCore.LowerClampFiresBelowStartBy2());
        Assert.True(ClientNpcRunCore.LowerClampNotNeededAtStartMinus1());
        Assert.True(ClientNpcRunCore.LowerClampCondition());

        Assert.Equal(0, ClientNpcRunCore.AdvanceKeepFrame(-5, 0, 5));
        Assert.Equal(0, ClientNpcRunCore.AdvanceKeepFrame(-2, 0, 5));
    }

    [Fact]
    public void ComparisonDirections()
    {
        // **保留帧用 >=、基类帧用 > —— 同一单元两处相反**
        Assert.True(ClientNpcRunCore.KeepUsesInclusive());
        Assert.True(ClientNpcRunCore.BaseUsesExclusive());
        Assert.True(ClientNpcRunCore.OppositeComparisonsInSameUnit());
        Assert.True(ClientNpcRunCore.ComparisonsAreOpposite());
    }

    [Fact]
    public void ThresholdBoundaries()
    {
        Assert.True(ClientNpcRunCore.KeepTimeReachedValues());

        // **保留帧：达到即推进**
        Assert.False(ClientNpcRunCore.KeepTimeReached(99, 100));
        Assert.True(ClientNpcRunCore.KeepTimeReached(100, 100));
        // **基类：必须超过**
        Assert.False(ClientNpcRunCore.BaseTimeReached(100, 100));
        Assert.True(ClientNpcRunCore.BaseTimeReached(101, 100));
    }

    [Fact]
    public void CardinalCast()
    {
        Assert.True(ClientNpcRunCore.CastToCardinal());
        Assert.True(ClientNpcRunCore.SignedFieldUnsignedCompare());
        Assert.True(ClientNpcRunCore.FortyTwoCastsInUnit());
    }

    [Fact]
    public void FivePartGuard()
    {
        Assert.True(ClientNpcRunCore.FivePartGuardThreeTimes());
        Assert.True(ClientNpcRunCore.TriplicatedAgain());
        Assert.True(ClientNpcRunCore.KeepPlayAllowedValues());

        // **五重：文件号非负、小于表数、索引非负、数量正、时间正**
        Assert.True(ClientNpcRunCore.KeepPlayAllowed(0, 10, 0, 1, 1));
        Assert.False(ClientNpcRunCore.KeepPlayAllowed(-1, 10, 0, 1, 1));
        Assert.False(ClientNpcRunCore.KeepPlayAllowed(10, 10, 0, 1, 1));
        Assert.False(ClientNpcRunCore.KeepPlayAllowed(0, 10, -1, 1, 1));
        Assert.False(ClientNpcRunCore.KeepPlayAllowed(0, 10, 0, 0, 1));
        Assert.False(ClientNpcRunCore.KeepPlayAllowed(0, 10, 0, 1, 0));
    }

    // ===================== 三、效果帧推进 =====================

    [Fact]
    public void EffectDivision()
    {
        Assert.True(ClientNpcRunCore.NpcDividesByThreeAgain());
        Assert.True(ClientNpcRunCore.ConsistentWithJ182());
        Assert.True(ClientNpcRunCore.NpcEffectFrameTimeValues());

        Assert.Equal(100, ClientNpcRunCore.NpcEffectFrameTime(300, true));
        Assert.Equal(300, ClientNpcRunCore.NpcEffectFrameTime(300, false));
    }

    [Fact]
    public void EffectWrap()
    {
        Assert.True(ClientNpcRunCore.EffectWrapsToStart());
        Assert.True(ClientNpcRunCore.TwoBranchesSameReset());
        Assert.True(ClientNpcRunCore.Bo248AddsFlagClear());
        Assert.True(ClientNpcRunCore.WrapEffectFrameValues());

        Assert.Equal(1, ClientNpcRunCore.WrapEffectFrame(0, 5, 60));
        Assert.Equal(60, ClientNpcRunCore.WrapEffectFrame(5, 5, 60));
    }

    [Fact]
    public void Bo248()
    {
        Assert.True(ClientNpcRunCore.Bo248SemanticlessName());
        Assert.True(ClientNpcRunCore.TwentyThreeSeconds());
        Assert.True(ClientNpcRunCore.Appearance52Special());
        Assert.True(ClientNpcRunCore.FiveReferences());
        Assert.True(ClientNpcRunCore.ResponsibilityConcentrated());
        Assert.True(ClientNpcRunCore.TriggersBo248Values());

        Assert.False(ClientNpcRunCore.TriggersBo248(51));
        Assert.True(ClientNpcRunCore.TriggersBo248(52));
    }

    [Fact]
    public void Bo248Sound()
    {
        Assert.True(ClientNpcRunCore.Bo248SoundRangeValues());

        Assert.Equal(146, ClientNpcRunCore.Bo248SoundIndex(0));
        Assert.Equal(152, ClientNpcRunCore.Bo248SoundIndex(6));
    }

    [Fact]
    public void Bo248Deadline()
    {
        Assert.True(ClientNpcRunCore.DecodeAsDeadline());
        Assert.True(ClientNpcRunCore.ReadStrictWriteNow());
        Assert.True(ClientNpcRunCore.Bo248ActiveBeforeDeadline());
        Assert.True(ClientNpcRunCore.Bo248ExpiresAfterDeadline());

        Assert.Equal(24000u, ClientNpcRunCore.Bo248Deadline(1000u));
    }

    [Fact]
    public void Looping()
    {
        Assert.True(ClientNpcRunCore.EffectLoopsByDefault());
        Assert.True(ClientNpcRunCore.ContrastsWithHum());
    }

    // ===================== 四、收尾 =====================

    [Fact]
    public void Ending()
    {
        Assert.True(ClientNpcRunCore.CachesTwoFramesAtTop());
        Assert.True(ClientNpcRunCore.ComparesKeepNotBody());
        Assert.True(ClientNpcRunCore.SameShapeDifferentFields());
        Assert.True(ClientNpcRunCore.CachesAfterInherited());
        Assert.True(ClientNpcRunCore.CapturesPostBaseState());
        Assert.True(ClientNpcRunCore.SameShapeAsBase());
    }

    [Fact]
    public void ReloadCondition()
    {
        Assert.True(ClientNpcRunCore.NpcNeedsReloadValues());

        Assert.True(ClientNpcRunCore.NpcNeedsReload(1, 2, 0, 0));
        Assert.True(ClientNpcRunCore.NpcNeedsReload(1, 1, 0, 1));
        Assert.False(ClientNpcRunCore.NpcNeedsReload(1, 1, 0, 0));
    }

    // ===================== 五、行数与全链闭合 =====================

    [Fact]
    public void ChainComplete()
    {
        Assert.True(ClientNpcRunCore.Total876());
        Assert.True(ClientNpcRunCore.RunBlockComplete());
        Assert.True(ClientNpcRunCore.NpcHasNoOwnInstrumentation());
        Assert.True(ClientNpcRunCore.InheritsProtection());
    }

    [Fact]
    public void Shares()
    {
        Assert.True(ClientNpcRunCore.NpcShareIs18());
        Assert.True(ClientNpcRunCore.BaseShareIs32());

        Assert.Equal(876, ClientNpcRunCore.BaseRunLines + ClientNpcRunCore.HumRunLines + ClientNpcRunCore.NpcRunLines);
        Assert.Equal(18, ClientNpcRunCore.NpcRunLines * 100
            / (ClientNpcRunCore.BaseRunLines + ClientNpcRunCore.HumRunLines + ClientNpcRunCore.NpcRunLines));
    }
}

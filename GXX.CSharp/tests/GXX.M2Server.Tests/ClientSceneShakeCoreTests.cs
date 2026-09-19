using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J175：客户端屏幕震动 1:1 测试。
/// **全局偏移量永不复位（仅因波形末项为零而侥幸归位）、
/// 波形八项只朝上且绝对值衰减、高低字打包的有符号还原、
/// 以及回绕安全与裸减法两套写法实测一致。**
/// </summary>
public sealed class ClientSceneShakeCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(50, ClientSceneShakeCore.TakeThrottle);
        Assert.Equal(8, ClientSceneShakeCore.ItemsPerLoop);
        Assert.Equal(0, ClientSceneShakeCore.ParamUnconditional);
        Assert.Equal(1, ClientSceneShakeCore.ParamGated);
        Assert.Equal(1, ClientSceneShakeCore.DefaultDelayCount);
        Assert.Equal(320, ClientSceneShakeCore.ServerShakeThrottle);
    }

    // ===================== 一、链条 =====================

    [Fact]
    public void Chain()
    {
        Assert.True(ClientSceneShakeCore.FourStageChain());
        Assert.True(ClientSceneShakeCore.ParamZeroUnconditional());
        Assert.True(ClientSceneShakeCore.ParamOneGatedByConfig());
    }

    [Fact]
    public void EntryGate()
    {
        Assert.True(ClientSceneShakeCore.EntryGateValues());

        // 参数零：忽略开关
        Assert.True(ClientSceneShakeCore.ShouldShake(0, false));
        Assert.True(ClientSceneShakeCore.ShouldShake(0, true));

        // 参数一：依赖开关
        Assert.True(ClientSceneShakeCore.ShouldShake(1, true));
        Assert.False(ClientSceneShakeCore.ShouldShake(1, false));
    }

    [Fact]
    public void EntryGateSemantics()
    {
        Assert.True(ClientSceneShakeCore.ParamZeroIgnoresConfig());
        Assert.True(ClientSceneShakeCore.ParamOneDependsOnConfig());
        Assert.True(ClientSceneShakeCore.OtherParamsRejected());
    }

    // ===================== 二、波形 =====================

    [Fact]
    public void WaveShape()
    {
        Assert.True(ClientSceneShakeCore.WaveLengthIsEight());
        Assert.True(ClientSceneShakeCore.VerticalOnly());
        Assert.True(ClientSceneShakeCore.FourNonZeroFourZero());
        Assert.True(ClientSceneShakeCore.NegativeOnly());
        Assert.True(ClientSceneShakeCore.AlternatingZero());
        Assert.Equal(8, ClientSceneShakeCore.Wave.Length);
    }

    [Fact]
    public void WaveMeasured()
    {
        Assert.True(ClientSceneShakeCore.AllXZero());
        Assert.True(ClientSceneShakeCore.AllYNonPositive());
        Assert.True(ClientSceneShakeCore.NonZeroCount());
        Assert.True(ClientSceneShakeCore.StrictAlternation());
    }

    [Fact]
    public void WaveValues()
    {
        Assert.Equal((0, -10), ClientSceneShakeCore.Wave[0]);
        Assert.Equal((0, 0), ClientSceneShakeCore.Wave[1]);
        Assert.Equal((0, -8), ClientSceneShakeCore.Wave[2]);
        Assert.Equal((0, 0), ClientSceneShakeCore.Wave[3]);
        Assert.Equal((0, -6), ClientSceneShakeCore.Wave[4]);
        Assert.Equal((0, -4), ClientSceneShakeCore.Wave[6]);
        Assert.Equal((0, 0), ClientSceneShakeCore.Wave[7]);
    }

    [Fact]
    public void Amplitudes()
    {
        // **绝对值衰减（一步步趋近零）**
        Assert.True(ClientSceneShakeCore.AmplitudeAbsDecreases());

        // **按数值比较是递增的**
        Assert.True(ClientSceneShakeCore.AmplitudeIncreasesNumerically());

        Assert.True(ClientSceneShakeCore.AmplitudesMatch());
        Assert.Equal(4, ClientSceneShakeCore.Amplitudes.Length);
        Assert.Equal(new[] { -10, -8, -6, -4 }, ClientSceneShakeCore.Amplitudes);
    }

    [Fact]
    public void LastItemZero()
    {
        Assert.True(ClientSceneShakeCore.LastItemIsZero());
        Assert.Equal(0, ClientSceneShakeCore.Wave[7].Y);
    }

    // ===================== 三、队列与耗时 =====================

    [Fact]
    public void QueueSize()
    {
        Assert.True(ClientSceneShakeCore.EightItemsPerCount());
        Assert.True(ClientSceneShakeCore.TotalItemsFormula());
        Assert.Equal(8, ClientSceneShakeCore.TotalItems(1));
        Assert.Equal(40, ClientSceneShakeCore.TotalItems(5));
        Assert.Equal(0, ClientSceneShakeCore.TotalItems(0));
    }

    [Fact]
    public void Duration()
    {
        Assert.True(ClientSceneShakeCore.FiftyMsPerItem());
        Assert.True(ClientSceneShakeCore.DurationFormula());
        Assert.True(ClientSceneShakeCore.CountOneIs400ms());
        Assert.Equal(400, ClientSceneShakeCore.DurationMs(1));
        Assert.Equal(2000, ClientSceneShakeCore.DurationMs(5));
    }

    [Fact]
    public void ZeroAndNegativeCounts()
    {
        Assert.True(ClientSceneShakeCore.ZeroCountPushesNothing());
        Assert.True(ClientSceneShakeCore.NegativeCountPushesNothing());
        Assert.Equal(0, ClientSceneShakeCore.TotalItems(0));
    }

    // ===================== 四、打包 =====================

    [Fact]
    public void Packing()
    {
        Assert.True(ClientSceneShakeCore.PacksIntoOneWord());
        Assert.True(ClientSceneShakeCore.ExplicitSignedCast());
        Assert.True(ClientSceneShakeCore.PackRoundTrip());
    }

    [Fact]
    public void PackModel()
    {
        int packed = ClientSceneShakeCore.Pack(0, -10);

        Assert.Equal(0, ClientSceneShakeCore.UnpackX(packed));
        Assert.Equal(-10, ClientSceneShakeCore.UnpackY(packed));
    }

    [Fact]
    public void SignedCastMatters()
    {
        // **负值往返后仍为负；当无符号解则是六五五二六**
        Assert.True(ClientSceneShakeCore.NegativeSurvivesRoundTrip());
        Assert.True(ClientSceneShakeCore.UnsignedWouldBeHuge());

        int packed = ClientSceneShakeCore.Pack(0, -10);
        Assert.Equal(-10, ClientSceneShakeCore.UnpackY(packed));
        Assert.Equal(65526, (packed >> 16) & 0xFFFF);
    }

    // ===================== 五、偏移量永不复位 =====================

    [Fact]
    public void OffsetsNeverReset()
    {
        Assert.True(ClientSceneShakeCore.OffsetsNeverReset());
        Assert.True(ClientSceneShakeCore.OnlyTwoWrites());
        Assert.Equal(2, ClientSceneShakeCore.OffsetWriteCount());
        Assert.True(ClientSceneShakeCore.WaveEndsAtZeroByLuck());
        Assert.True(ClientSceneShakeCore.MidWaveStopsLeaveResidual());
        Assert.True(ClientSceneShakeCore.PermanentOffsetUntilNextShake());
    }

    [Fact]
    public void FullWaveEndsAtZero()
    {
        // **取满八项后恰好归零（侥幸）**
        Assert.True(ClientSceneShakeCore.FullWaveEndsAtZero());

        var (x, y) = ClientSceneShakeCore.ConsumeOffsets(8);
        Assert.Equal(0, x);
        Assert.Equal(0, y);
    }

    [Fact]
    public void MidWaveResidual()
    {
        Assert.True(ClientSceneShakeCore.OneItemLeavesResidual());
        Assert.True(ClientSceneShakeCore.ThreeItemsLeaveResidual());

        Assert.Equal(-10, ClientSceneShakeCore.ConsumeOffsets(1).Y);
        Assert.Equal(-8, ClientSceneShakeCore.ConsumeOffsets(3).Y);
    }

    [Fact]
    public void ParityOfResidual()
    {
        // **奇数项一定留下非零纵偏移、偶数项一定归零**
        Assert.True(ClientSceneShakeCore.OddCountsAlwaysResidual());
        Assert.True(ClientSceneShakeCore.EvenCountsAlwaysZero());
    }

    // ===================== 六、两套计时写法 =====================

    [Fact]
    public void TickDiffSemantics()
    {
        Assert.True(ClientSceneShakeCore.TickDiffIsWrapSafe());
        Assert.True(ClientSceneShakeCore.TwoDifferentIdioms());
        Assert.True(ClientSceneShakeCore.InconsistentAdjacentChecks());
        Assert.True(ClientSceneShakeCore.TickDiffNormal());
        Assert.Equal(100u, ClientSceneShakeCore.TickDiff(100, 200));
    }

    [Fact]
    public void WrapBehaviour()
    {
        Assert.True(ClientSceneShakeCore.TickDiffWrap());

        // **两套写法在回绕时相差一（我连续两次判断错误后实测定案）**
        Assert.True(ClientSceneShakeCore.RawSubtractWrapDiffersByOne());
        Assert.True(ClientSceneShakeCore.TickDiffOffByOneOnWrap());

        // **两者结论仍一致（都远小于五十）**
        Assert.True(ClientSceneShakeCore.IdiomsAgreeOnWrap());
    }

    [Fact]
    public void WrapModel()
    {
        uint start = uint.MaxValue - 10;
        uint end = 5;

        // **`tick_diff` 是十五、裸减法是十六 —— 相差一**
        Assert.Equal(15u, ClientSceneShakeCore.TickDiff(start, end));
        Assert.Equal(16u, unchecked(end - start));
    }

    // ===================== 七、延迟路径 =====================

    [Fact]
    public void DelayPath()
    {
        Assert.True(ClientSceneShakeCore.DelayStoresStateOnly());
        Assert.True(ClientSceneShakeCore.DelayAndImmediateIndependent());
        Assert.True(ClientSceneShakeCore.DelayCountReused());
        Assert.True(ClientSceneShakeCore.DelayFiveMeans40Items());
        Assert.Equal(40, ClientSceneShakeCore.TotalItems(5));
    }

    [Fact]
    public void InclusiveBoundaries()
    {
        Assert.True(ClientSceneShakeCore.DelayInclusive());
        Assert.True(ClientSceneShakeCore.ThrottleInclusive());
        Assert.True(ClientSceneShakeCore.DelayBoundary());
        Assert.True(ClientSceneShakeCore.TakeBoundary());
    }

    [Fact]
    public void DelayModel()
    {
        // **恰好等于延迟时长即触发**
        Assert.False(ClientSceneShakeCore.DelayElapsed(0, 99, 100));
        Assert.True(ClientSceneShakeCore.DelayElapsed(0, 100, 100));
        Assert.True(ClientSceneShakeCore.DelayElapsed(0, 101, 100));
    }

    [Fact]
    public void TakeModel()
    {
        // **恰好五十即取用**
        Assert.False(ClientSceneShakeCore.TakeElapsed(0, 49));
        Assert.True(ClientSceneShakeCore.TakeElapsed(0, 50));
        Assert.True(ClientSceneShakeCore.TakeElapsed(0, 51));
    }

    // ===================== 八、与服务端对照 =====================

    [Fact]
    public void ServerContrast()
    {
        Assert.True(ClientSceneShakeCore.ServerAlsoHasShake());
        Assert.True(ClientSceneShakeCore.ThrottlesDiffer());
        Assert.True(ClientSceneShakeCore.ClientServerPolarityDiffers());
        Assert.True(ClientSceneShakeCore.ServerPolarityIsStrict());
        Assert.True(ClientSceneShakeCore.ThrottleGapIs270());
        Assert.Equal(270, ClientSceneShakeCore.ServerShakeThrottle - ClientSceneShakeCore.TakeThrottle);
    }

    // ===================== 九、构造初值与绘制点 =====================

    [Fact]
    public void CtorDefaults()
    {
        Assert.True(ClientSceneShakeCore.CountDefaultsToOne());
        Assert.True(ClientSceneShakeCore.TimeDefaultsToZero());
        Assert.True(ClientSceneShakeCore.FlagDefaultsFalse());
        Assert.True(ClientSceneShakeCore.ThreeCtorDefaults());
        Assert.True(ClientSceneShakeCore.DefaultsDoNotTrigger());
        Assert.True(ClientSceneShakeCore.OffsetInitIsTypedZero());
        Assert.Equal(3, ClientSceneShakeCore.CtorDefaults.Length);
    }

    [Fact]
    public void ReferenceCounts()
    {
        Assert.Equal(18, ClientSceneShakeCore.ReferenceCount());
        Assert.True(ClientSceneShakeCore.EighteenReferences());
        Assert.True(ClientSceneShakeCore.NineEach());
        Assert.True(ClientSceneShakeCore.TwoCommentedOut());
        Assert.True(ClientSceneShakeCore.CommentedPairAdjacent());
        Assert.True(ClientSceneShakeCore.ThreeReferenceForms());
        Assert.True(ClientSceneShakeCore.OneSiteOptedOut());
    }

    [Fact]
    public void VisibilityAffected()
    {
        Assert.True(ClientSceneShakeCore.OffsetAffectsVisibility());
        Assert.True(ClientSceneShakeCore.VisibilityChangesWithShake());
    }

    [Fact]
    public void VisibilityModel()
    {
        // 左侧：横坐标负四十，无震动可见、偏移负十即不可见
        Assert.True(ClientSceneShakeCore.VisibleWithShake(-40, 0, 48));
        Assert.False(ClientSceneShakeCore.VisibleWithShake(-40, -10, 48));

        // 右侧：横坐标八百一十，无震动不可见、偏移负二十即可见
        Assert.False(ClientSceneShakeCore.VisibleWithShake(810, 0, 48));
        Assert.True(ClientSceneShakeCore.VisibleWithShake(810, -20, 48));
    }

    // ===================== 十、行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientSceneShakeCore.FourMethods());
        Assert.Equal(4, ClientSceneShakeCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 9, 25, 7, 12 }, ClientSceneShakeCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(ClientSceneShakeCore.SceneShakeIsLongest());
        Assert.True(ClientSceneShakeCore.AddOffsetIsShortest());
        Assert.True(ClientSceneShakeCore.SceneShakeShareIs47());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(ClientSceneShakeCore.TotalLinesValues());
        Assert.Equal(53, ClientSceneShakeCore.TotalLines());
    }
}

using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J176：客户端攻击效果与震动触发 1:1 测试。
/// **三处触发点（两处生效、一处被注释）、
/// 索引乘数链只有十与二十两种值、
/// 二三号有条件跳过绘制、四号三重条件换图库、
/// 以及注释文字与所在分支不一致。**
/// </summary>
public sealed class ClientHitEffectCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1, ClientHitEffectCore.DefaultShakeCount);
        Assert.Equal(0, ClientHitEffectCore.DefaultShakeDelay);
        Assert.Equal(102, ClientHitEffectCore.Sm102Hit);
        Assert.Equal(114, ClientHitEffectCore.MagicSerialYiTian);
        Assert.Equal(2000, ClientHitEffectCore.Eff21IndexOffset);
        Assert.Equal(2, ClientHitEffectCore.ShakeFrame);
        Assert.Equal(8, ClientHitEffectCore.ItemsPerCount);
    }

    // ===================== 一、调用形式 =====================

    [Fact]
    public void CallForms()
    {
        Assert.True(ClientHitEffectCore.DefaultCountIsOne());
        Assert.True(ClientHitEffectCore.DefaultDelayIsZero());
        Assert.True(ClientHitEffectCore.FourCallForms());
        Assert.True(ClientHitEffectCore.FourCallFormEntries());
        Assert.True(ClientHitEffectCore.FirstTwoAreCountOne());
        Assert.Equal(4, ClientHitEffectCore.CallForms.Length);
    }

    [Fact]
    public void CallSites()
    {
        Assert.Equal(9, ClientHitEffectCore.CallSiteCount());
        Assert.True(ClientHitEffectCore.NineCallSites());
    }

    [Fact]
    public void BareCallLength()
    {
        // **无参调用压八项**
        Assert.True(ClientHitEffectCore.BareCallPushesEight());
        Assert.Equal(8, ClientHitEffectCore.DefaultShakeCount * ClientHitEffectCore.ItemsPerCount);
    }

    [Fact]
    public void BothFormsSameLength()
    {
        Assert.True(ClientHitEffectCore.BothPushEight());
        Assert.True(ClientHitEffectCore.DifferenceIsTimingNotLength());
    }

    // ===================== 二、触发点一 =====================

    [Fact]
    public void TriggerOneShape()
    {
        Assert.True(ClientHitEffectCore.RangeBranchWithExtraCondition());
        Assert.True(ClientHitEffectCore.OnlyOneOfFourShakes());
        Assert.True(ClientHitEffectCore.FrameMustBeTwo());
        Assert.True(ClientHitEffectCore.Sm102Is102());
        Assert.True(ClientHitEffectCore.FourActionsInRange());
    }

    [Fact]
    public void TriggerOneValues()
    {
        Assert.True(ClientHitEffectCore.TriggerOneValues());

        // **只有一零二、帧为二、开关真时震动**
        Assert.False(ClientHitEffectCore.ShouldShakeOnHit(100, 2, true));
        Assert.False(ClientHitEffectCore.ShouldShakeOnHit(101, 2, true));
        Assert.True(ClientHitEffectCore.ShouldShakeOnHit(102, 2, true));
        Assert.False(ClientHitEffectCore.ShouldShakeOnHit(103, 2, true));
    }

    [Fact]
    public void TriggerOneFrameAndConfig()
    {
        Assert.False(ClientHitEffectCore.ShouldShakeOnHit(102, 1, true));
        Assert.False(ClientHitEffectCore.ShouldShakeOnHit(102, 3, true));
        Assert.False(ClientHitEffectCore.ShouldShakeOnHit(102, 2, false));
        Assert.True(ClientHitEffectCore.ShouldShakeOnHit(102, 2, true));
    }

    [Fact]
    public void SoundIndependent()
    {
        // **四种动作都在第二帧播声音、震动只是附加**
        Assert.True(ClientHitEffectCore.SoundAlwaysPlays());
        Assert.True(ClientHitEffectCore.SoundForAllFour());
    }

    // ===================== 三、触发点二 =====================

    [Fact]
    public void TriggerTwoShape()
    {
        Assert.True(ClientHitEffectCore.MagicSerial114Shakes());
        Assert.True(ClientHitEffectCore.MagicSerial116117CommentedOut());
        Assert.True(ClientHitEffectCore.RemovedNotDisabled());
        Assert.True(ClientHitEffectCore.CommentSaysRemoved());
        Assert.True(ClientHitEffectCore.TwoRemovedSerials());
        Assert.Equal(2, ClientHitEffectCore.RemovedMagicSerials.Length);
    }

    [Fact]
    public void TriggerTwoValues()
    {
        Assert.True(ClientHitEffectCore.TriggerTwoValues());

        Assert.True(ClientHitEffectCore.ShouldShakeOnMagic(114, true));
        Assert.False(ClientHitEffectCore.ShouldShakeOnMagic(114, false));
        Assert.False(ClientHitEffectCore.ShouldShakeOnMagic(116, true));
        Assert.False(ClientHitEffectCore.ShouldShakeOnMagic(117, true));
    }

    // ===================== 四、注释形式与时间戳 =====================

    [Fact]
    public void CommentStyle()
    {
        Assert.True(ClientHitEffectCore.AllThreeUseBraceComments());
        Assert.True(ClientHitEffectCore.BlockCommentNotLineComment());
        Assert.True(ClientHitEffectCore.EntireBlockCommented());
    }

    [Fact]
    public void Timestamps()
    {
        Assert.True(ClientHitEffectCore.ThreeRemovalTimestamps());
        Assert.True(ClientHitEffectCore.SameDay());
        Assert.True(ClientHitEffectCore.WithinThreeMinutes());
        Assert.Equal(3, ClientHitEffectCore.RemovalTimestamps.Length);
    }

    [Fact]
    public void TimestampGap()
    {
        // **首末相隔恰好一百零五秒**
        Assert.True(ClientHitEffectCore.MaxGapIs105s());

        int[] s = ClientHitEffectCore.RemovalSeconds();
        Assert.Equal(new[] { 52495, 52537, 52600 }, s);
        Assert.Equal(105, s[2] - s[0]);
    }

    [Fact]
    public void TextPositionMismatch()
    {
        // **注释说断空斩、位置在开天斩轻击**
        Assert.True(ClientHitEffectCore.TextSaysDuanKong());
        Assert.True(ClientHitEffectCore.LocatedInKaiTianLight());
        Assert.True(ClientHitEffectCore.TextAndPositionDisagree());
        Assert.True(ClientHitEffectCore.TwoDifferentNumbers());
        Assert.Equal(26, ClientHitEffectCore.EffDuanKong);
        Assert.Equal(25, ClientHitEffectCore.EffKaiTianLight);
    }

    // ===================== 五、索引乘数链 =====================

    [Fact]
    public void ChainStructure()
    {
        Assert.True(ClientHitEffectCore.TwoParallelChains());
        Assert.True(ClientHitEffectCore.DifferentNumberSets());
        Assert.True(ClientHitEffectCore.TwoMultipliers());
        Assert.True(ClientHitEffectCore.SixTwenty());
        Assert.True(ClientHitEffectCore.SixTen());
        Assert.True(ClientHitEffectCore.OneSpecial());
        Assert.True(ClientHitEffectCore.OneDefault());
    }

    [Fact]
    public void MultiplierTables()
    {
        Assert.Equal(6, ClientHitEffectCore.Mult20.Length);
        Assert.Equal(6, ClientHitEffectCore.Mult10.Length);
        Assert.Equal(new[] { 7, 8, 9, 20, 22, 26 }, ClientHitEffectCore.Mult20);
        Assert.Equal(new[] { 10, 12, 25, 14, 4, 27 }, ClientHitEffectCore.Mult10);
    }

    [Fact]
    public void MultiplierSets()
    {
        Assert.True(ClientHitEffectCore.SetsDisjoint());
        Assert.True(ClientHitEffectCore.TwelveCovered());
        Assert.True(ClientHitEffectCore.MultiplierLookupCorrect());
        Assert.True(ClientHitEffectCore.UnlistedFallsToTen());
    }

    [Fact]
    public void MultiplierModel()
    {
        Assert.Equal(20, ClientHitEffectCore.MultiplierFor(7));
        Assert.Equal(20, ClientHitEffectCore.MultiplierFor(26));
        Assert.Equal(10, ClientHitEffectCore.MultiplierFor(10));
        Assert.Equal(10, ClientHitEffectCore.MultiplierFor(27));
        Assert.Equal(10, ClientHitEffectCore.MultiplierFor(999));
    }

    [Fact]
    public void CommentedBranch()
    {
        // **二二只在注释里、不参与运行**
        Assert.True(ClientHitEffectCore.Eff22IsCommented());
        Assert.True(ClientHitEffectCore.ScriptCountsItButDoesNotRun());
        Assert.True(ClientHitEffectCore.EffectiveIsEleven());
        Assert.Equal(11, ClientHitEffectCore.EffectiveBranchCount());
    }

    // ===================== 六、特殊分支 =====================

    [Fact]
    public void Eff23Special()
    {
        Assert.True(ClientHitEffectCore.Eff23ConditionalSkip());
        Assert.True(ClientHitEffectCore.NegativeOneSkipsDraw());
        Assert.True(ClientHitEffectCore.OnlyBranchThatSkips());
    }

    [Fact]
    public void Eff23Model()
    {
        // **动作号为零时给负一**
        Assert.True(ClientHitEffectCore.Eff23SkipValue());
        Assert.Equal(-1, ClientHitEffectCore.IndexFor(23, 100, 2, 3, 0));

        // **动作号非零时正常计算**
        Assert.True(ClientHitEffectCore.Eff23NormalWhenActing());
        Assert.Equal(123, ClientHitEffectCore.IndexFor(23, 100, 2, 3, 5));

        // **其他效果号不受动作号影响**
        Assert.True(ClientHitEffectCore.OthersIgnoreAction());
    }

    [Fact]
    public void DrawBoundary()
    {
        Assert.True(ClientHitEffectCore.DrawBoundary());
        Assert.False(ClientHitEffectCore.ShouldDraw(-1));
        Assert.True(ClientHitEffectCore.ShouldDraw(0));
    }

    [Fact]
    public void Eff4Special()
    {
        Assert.True(ClientHitEffectCore.Eff4TripleCondition());
        Assert.True(ClientHitEffectCore.FourLevelNoEnhance());
        Assert.True(ClientHitEffectCore.OnlyBranchThatSwapsImages());
        Assert.True(ClientHitEffectCore.Eff4Values());
    }

    [Fact]
    public void Eff4Model()
    {
        Assert.True(ClientHitEffectCore.Eff4Matches(4, 4, 0));
        Assert.False(ClientHitEffectCore.Eff4Matches(4, 4, 1));
        Assert.False(ClientHitEffectCore.Eff4Matches(4, 3, 0));
        Assert.False(ClientHitEffectCore.Eff4Matches(5, 4, 0));
    }

    [Fact]
    public void DefaultRedundancy()
    {
        Assert.True(ClientHitEffectCore.DefaultSameAsTen());
        Assert.True(ClientHitEffectCore.FallthroughIsRedundant());
        Assert.True(ClientHitEffectCore.DefaultMultiplierMatchesTen());
    }

    // ===================== 七、灰度绘制 =====================

    [Fact]
    public void GrayImage()
    {
        Assert.True(ClientHitEffectCore.GhostUsesGrayImage());
        Assert.True(ClientHitEffectCore.ChecksSelfNotActor());
        Assert.True(ClientHitEffectCore.AllEffectsTurnGray());
        Assert.True(ClientHitEffectCore.GrayValues());
    }

    [Fact]
    public void GrayModel()
    {
        // **"自己非空且已死亡"才用灰度 —— 三态为假**
        Assert.False(ClientHitEffectCore.UseGray(true, true));
        Assert.False(ClientHitEffectCore.UseGray(true, false));
        Assert.False(ClientHitEffectCore.UseGray(false, false));
        Assert.True(ClientHitEffectCore.UseGray(false, true));
    }

    // ===================== 八、断岳斩附加绘制 =====================

    [Fact]
    public void Eff21Overlay()
    {
        Assert.True(ClientHitEffectCore.Eff21SecondPass());
        Assert.True(ClientHitEffectCore.IndexOffset2000());
        Assert.True(ClientHitEffectCore.SameCoordinateFormula());
        Assert.True(ClientHitEffectCore.OverlayEffect());
        Assert.True(ClientHitEffectCore.Eff21NotInFirstChain());
    }

    [Fact]
    public void Eff21Model()
    {
        Assert.True(ClientHitEffectCore.Eff21UsesTen());
        Assert.Equal(2023, ClientHitEffectCore.Eff21Index(2, 3));
    }

    [Fact]
    public void Eff21Absent()
    {
        Assert.True(ClientHitEffectCore.Eff21AbsentFromChains());
        Assert.Equal(21, ClientHitEffectCore.EffDuanYue);
    }

    // ===================== 九、与 J175 衔接 =====================

    [Fact]
    public void LinkToJ175()
    {
        Assert.True(ClientHitEffectCore.ConnectToJ175());
        Assert.True(ClientHitEffectCore.TwoActiveOneCommented());
        Assert.True(ClientHitEffectCore.ConfigCheckPlacementDiffers());
    }

    [Fact]
    public void ConfigForms()
    {
        Assert.True(ClientHitEffectCore.ThreeConfigForms());
        Assert.True(ClientHitEffectCore.FormsAllDistinct());
        Assert.True(ClientHitEffectCore.CommentedOneOnlyChecksConfig());
        Assert.Equal(3, ClientHitEffectCore.ConfigForms.Length);
    }

    // ===================== 十、行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientHitEffectCore.FiveFragments());
        Assert.Equal(5, ClientHitEffectCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 12, 15, 8, 65, 10 }, ClientHitEffectCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(ClientHitEffectCore.IndexChainIsLongest());
        Assert.True(ClientHitEffectCore.CommentedIsShortest());
        Assert.True(ClientHitEffectCore.IndexChainShareIs59());
        Assert.True(ClientHitEffectCore.IndexChainExceedsRest());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(ClientHitEffectCore.TotalLinesValues());
        Assert.Equal(110, ClientHitEffectCore.TotalLines());
    }
}

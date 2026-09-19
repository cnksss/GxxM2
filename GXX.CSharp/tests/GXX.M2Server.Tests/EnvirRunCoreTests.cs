using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J174：`TEnvirnoment` 初始化与主循环 1:1 测试。
/// **两个节流的边界用模型三态实测、
/// 振动项五种处置分类逐一验证、
/// 天气过期与回收判据的严格性、
/// 以及初始化标志与实际状态可以不一致。**
/// </summary>
public sealed class EnvirRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1000, EnvirRunCore.RunThrottle);
        Assert.Equal(320, EnvirRunCore.ShakeThrottle);
        Assert.Equal(22, EnvirRunCore.WeatherEffectCount);
        Assert.Equal(56, EnvirRunCore.AllShakeLockId);
        Assert.Equal(2, EnvirRunCore.MinDimension);
    }

    // ===================== 一、Initialize =====================

    [Fact]
    public void InitializeSemantics()
    {
        Assert.True(EnvirRunCore.FreesOldAllocatesNew());
        Assert.True(EnvirRunCore.CorrectOrder());
        Assert.True(EnvirRunCore.WindowBetweenAssignAndAlloc());
        Assert.True(EnvirRunCore.FailureLeavesInconsistent());
    }

    [Fact]
    public void InitializeFlag()
    {
        // **标志在条件之外被无条件清掉**
        Assert.True(EnvirRunCore.FlagClearedUnconditionally());
        Assert.True(EnvirRunCore.InvalidSizeDeinitialises());
        Assert.True(EnvirRunCore.FlagAndStateCanDiverge());
    }

    [Fact]
    public void InitializeModel()
    {
        Assert.True(EnvirRunCore.InitValid());

        var r = EnvirRunCore.Initialize(100, 100, 0, 0, false);
        Assert.True(r.Flag);
        Assert.Equal(100, r.Width);
        Assert.Equal(100, r.Height);
        Assert.True(r.Allocated);
    }

    [Fact]
    public void InitializeInvalid()
    {
        Assert.True(EnvirRunCore.InitInvalidKeepsMembers());
        Assert.True(EnvirRunCore.InitInvalidDeinitialises());

        // 非法尺寸：标志假、但成员值与分配状态保持原样
        var r = EnvirRunCore.Initialize(0, 0, 50, 60, true);
        Assert.False(r.Flag);
        Assert.Equal(50, r.Width);
        Assert.Equal(60, r.Height);
        Assert.True(r.Allocated);
    }

    [Fact]
    public void BothDimensionsChecked()
    {
        Assert.True(EnvirRunCore.BothDimensionsChecked());
        Assert.True(EnvirRunCore.MinDimensionIsTwo());

        Assert.True(EnvirRunCore.Initialize(2, 2, 0, 0, false).Flag);
        Assert.False(EnvirRunCore.Initialize(1, 2, 0, 0, false).Flag);
        Assert.False(EnvirRunCore.Initialize(2, 1, 0, 0, false).Flag);
    }

    [Fact]
    public void NamingAndComments()
    {
        Assert.True(EnvirRunCore.LoopsOverMemberDims());
        Assert.True(EnvirRunCore.ConfusableNames());
        Assert.True(EnvirRunCore.CommentedDirectIndexing());
        Assert.True(EnvirRunCore.LaterRefactoredToCall());
        Assert.True(EnvirRunCore.CommentedIndexingShape());
        Assert.Contains("MapCellArray[", EnvirRunCore.CommentedIndexing, StringComparison.Ordinal);
    }

    [Fact]
    public void Allocation()
    {
        Assert.True(EnvirRunCore.NoOverflowCheck());
        Assert.True(EnvirRunCore.CellCountValues());
        Assert.Equal(5000, EnvirRunCore.CellCount(100, 50));
        Assert.Equal(4, EnvirRunCore.CellCount(2, 2));
    }

    // ===================== 二、Run 节流 =====================

    [Fact]
    public void RunThrottle()
    {
        Assert.True(EnvirRunCore.ThrottleIs1000ms());
        Assert.True(EnvirRunCore.ThrottleUsesLessOrEqual());
        Assert.True(EnvirRunCore.MustExceedStrictly());
        Assert.True(EnvirRunCore.ExactThousandStillSkips());
        Assert.True(EnvirRunCore.ThrottleBoundaries());
        Assert.True(EnvirRunCore.RunIsOncePerSecond());
    }

    [Fact]
    public void RunThrottleModel()
    {
        // **必须严格大于一千才跑**
        Assert.False(EnvirRunCore.ShouldRun(999));
        Assert.False(EnvirRunCore.ShouldRun(1000));
        Assert.True(EnvirRunCore.ShouldRun(1001));
    }

    [Fact]
    public void LastRunTick()
    {
        Assert.Equal(2, EnvirRunCore.LastRunTickUses());
        Assert.True(EnvirRunCore.LastRunTickOnlyTwoUses());
    }

    // ---------- 天气 ----------

    [Fact]
    public void WeatherArray()
    {
        Assert.True(EnvirRunCore.ArrayLengthIs22());
        Assert.True(EnvirRunCore.CommentedThreeWeatherEffects());
        Assert.True(EnvirRunCore.ReplacedByArray());
        Assert.True(EnvirRunCore.CommentedNotDeleted());
        Assert.True(EnvirRunCore.ThreeOldFields());
        Assert.Equal(3, EnvirRunCore.OldWeatherFields.Length);
    }

    [Fact]
    public void WeatherExpiry()
    {
        Assert.True(EnvirRunCore.WeatherExpiryStrictGreater());
        Assert.True(EnvirRunCore.ExactDurationNotExpired());
        Assert.True(EnvirRunCore.WeatherBoundaries());
        Assert.True(EnvirRunCore.UnusedNeverExpires());
    }

    [Fact]
    public void WeatherExpiryModel()
    {
        // **严格大于才过期 —— 恰好等于不算**
        Assert.False(EnvirRunCore.WeatherExpired(true, 500, 401, 100));
        Assert.False(EnvirRunCore.WeatherExpired(true, 500, 400, 100));
        Assert.True(EnvirRunCore.WeatherExpired(true, 500, 399, 100));

        // 未使用永不过期
        Assert.False(EnvirRunCore.WeatherExpired(false, 9999, 0, 1));
    }

    [Fact]
    public void WeatherNotification()
    {
        Assert.True(EnvirRunCore.NotifiesOnlyIfChanged());
    }

    // ===================== 三、地图振动 =====================

    [Fact]
    public void SceneShakeStructure()
    {
        Assert.True(EnvirRunCore.SceneShakeIteratesBackwards());
        Assert.True(EnvirRunCore.SecondBackwardIteration());
        Assert.True(EnvirRunCore.ReclaimBeforeThrottle());
        Assert.True(EnvirRunCore.ReclaimWhenCountReached());
        Assert.True(EnvirRunCore.Throttle320ms());
    }

    [Fact]
    public void SceneShakeCleanup()
    {
        Assert.True(EnvirRunCore.PlayerGoneReclaimed());
        Assert.True(EnvirRunCore.WrongMapReclaimed());
        Assert.True(EnvirRunCore.EmptyNameMeansAll());
        Assert.True(EnvirRunCore.TicksRecordedAtEnd());
        Assert.True(EnvirRunCore.ThrottledItemsRetried());
        Assert.True(EnvirRunCore.SkipDoesNotUpdate());
    }

    [Fact]
    public void ReclaimBoundary()
    {
        // **次数已达即回收（大于等于）**
        Assert.True(EnvirRunCore.ReclaimBoundary());
        Assert.False(EnvirRunCore.ShouldReclaim(4, 5));
        Assert.True(EnvirRunCore.ShouldReclaim(5, 5));
        Assert.True(EnvirRunCore.ShouldReclaim(6, 5));
    }

    [Fact]
    public void ShakeThrottleBoundary()
    {
        // **小于三百二十才跳过**
        Assert.True(EnvirRunCore.ShakeThrottleBoundary());
        Assert.True(EnvirRunCore.ShakeThrottled(319, 0));
        Assert.False(EnvirRunCore.ShakeThrottled(320, 0));
        Assert.False(EnvirRunCore.ShakeThrottled(321, 0));
    }

    [Fact]
    public void FiveActionsReachable()
    {
        Assert.True(EnvirRunCore.FiveActionsReachable());
    }

    [Fact]
    public void ClassifyModel()
    {
        // 回收优先于节流
        Assert.Equal(EnvirRunCore.ShakeAction.ReclaimCount,
            EnvirRunCore.Classify(5, 5, 100, 0, false, true, true));

        // 被节流
        Assert.Equal(EnvirRunCore.ShakeAction.SkipThrottled,
            EnvirRunCore.Classify(0, 5, 100, 0, false, true, true));

        // 玩家失效回收
        Assert.Equal(EnvirRunCore.ShakeAction.ReclaimInvalid,
            EnvirRunCore.Classify(0, 5, 9999, 0, false, false, true));

        // 地图不对也回收
        Assert.Equal(EnvirRunCore.ShakeAction.ReclaimInvalid,
            EnvirRunCore.Classify(0, 5, 9999, 0, false, true, false));

        // 个体派发
        Assert.Equal(EnvirRunCore.ShakeAction.Individual,
            EnvirRunCore.Classify(0, 5, 9999, 0, false, true, true));

        // 全体派发
        Assert.Equal(EnvirRunCore.ShakeAction.All,
            EnvirRunCore.Classify(0, 5, 9999, 0, true, false, false));
    }

    [Fact]
    public void ClassifyPrecedence()
    {
        Assert.True(EnvirRunCore.ReclaimTakesPrecedence());
        Assert.True(EnvirRunCore.WrongMapAlsoReclaims());
        Assert.True(EnvirRunCore.EmptyNameIgnoresPlayer());
    }

    // ---------- 客户端选项 ----------

    [Fact]
    public void ClientOption()
    {
        Assert.True(EnvirRunCore.SingleOptionNotPerItem());
        Assert.True(EnvirRunCore.LastItemWinsOption());
        Assert.True(EnvirRunCore.OptionCanBeOverwritten());
        Assert.True(EnvirRunCore.FoldOptionValues());
        Assert.True(EnvirRunCore.DiffersFromAnySemantics());
    }

    [Fact]
    public void FoldOptionModel()
    {
        // **最后一项决定结果 —— 不是"任一为真"**
        Assert.False(EnvirRunCore.FoldOption(new[] { true, true, false }));
        Assert.True(EnvirRunCore.FoldOption(new[] { false, false, true }));
        Assert.False(EnvirRunCore.FoldOption(new[] { true, false }));
    }

    // ---------- 派发 ----------

    [Fact]
    public void Dispatch()
    {
        Assert.True(EnvirRunCore.SameGuardsBothPaths());
        Assert.True(EnvirRunCore.AllShakeScansAllPlayers());
        Assert.True(EnvirRunCore.IndividualUsesRangeQuery());
        Assert.True(EnvirRunCore.RangeIsViewRange());
        Assert.True(EnvirRunCore.AllShakeLockIs56());
    }

    [Fact]
    public void FiveGuards()
    {
        Assert.True(EnvirRunCore.FiveGuardsRequired());
        Assert.True(EnvirRunCore.GuardsShared());

        Assert.True(EnvirRunCore.FiveGuards(true, true, true, true, true));
        Assert.False(EnvirRunCore.FiveGuards(false, true, true, true, true));
        Assert.False(EnvirRunCore.FiveGuards(true, false, true, true, true));
        Assert.False(EnvirRunCore.FiveGuards(true, true, false, true, true));
        Assert.False(EnvirRunCore.FiveGuards(true, true, true, false, true));
        Assert.False(EnvirRunCore.FiveGuards(true, true, true, true, false));
    }

    // ---------- 变量覆盖 ----------

    [Fact]
    public void VariableShadowing()
    {
        Assert.True(EnvirRunCore.VariableShadowedAcrossLoops());
        Assert.True(EnvirRunCore.ReliesOnEvaluationOrder());
        Assert.True(EnvirRunCore.AccidentallyCorrect());
        Assert.True(EnvirRunCore.OuterDoesNotReuseAfterInner());
    }

    // ---------- 临时列表 ----------

    [Fact]
    public void TempLists()
    {
        Assert.True(EnvirRunCore.TempList2ReusedAndCleared());
        Assert.True(EnvirRunCore.ClearMakesCumulativeSafe());
        Assert.True(EnvirRunCore.FreedAfterUnlock());
        Assert.True(EnvirRunCore.LeakOnException());
        Assert.True(EnvirRunCore.LockNotReleasedOnException());
        Assert.True(EnvirRunCore.CallsGuardianLevelLast());
    }

    // ===================== 四、关联 =====================

    [Fact]
    public void Integration()
    {
        Assert.True(EnvirRunCore.IntegratesPriorBatches());
        Assert.True(EnvirRunCore.TwoThrottles());
        Assert.True(EnvirRunCore.ThrottlesDiffer());
        Assert.True(EnvirRunCore.ThrottleGapIs680());
        Assert.Equal(680, EnvirRunCore.RunThrottle - EnvirRunCore.ShakeThrottle);
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirRunCore.TwoMethods());
        Assert.Equal(2, EnvirRunCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 35, 139 }, EnvirRunCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(EnvirRunCore.RunIsMuchLonger());
        Assert.True(EnvirRunCore.RunShareIs79());
        Assert.True(EnvirRunCore.LineGapIs104());
        Assert.True(EnvirRunCore.GapValues());
        Assert.Equal(104, EnvirRunCore.MethodLineCounts[1] - EnvirRunCore.MethodLineCounts[0]);
    }

    [Fact]
    public void Totals()
    {
        Assert.True(EnvirRunCore.TotalLinesValues());
        Assert.Equal(174, EnvirRunCore.TotalLines());
    }
}

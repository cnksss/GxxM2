using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J194：`TCopyMon.RecalcAbilitys_Add` 1:1 测试（263 行）。
/// **核心是一处跨字段笔误**：2529 行 `MP` 的夹取判据比的是 `MaxHP`、
/// 赋的却是 `MaxMP`，而紧邻的 2515 行 `HP` 那份两侧都是 `MaxHP`（自洽）。
/// 另记录"先整除再乘导致精度丢失"与"`SmallInt` 范围使夹取名存实亡"两项。
/// </summary>
public sealed class CopyMonRecalcAddCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(263, CopyMonRecalcAddCore.Lines);
        Assert.Equal(2280, CopyMonRecalcAddCore.Start);
        Assert.Equal(2542, CopyMonRecalcAddCore.End);
        Assert.Equal(38, CopyMonRecalcAddCore.InheritBlockLines);
        Assert.Equal(2285, CopyMonRecalcAddCore.InheritBlockStart);
        Assert.Equal(2322, CopyMonRecalcAddCore.InheritBlockEnd);
        Assert.Equal(211, CopyMonRecalcAddCore.ChangeBlockLines);
        Assert.Equal(2323, CopyMonRecalcAddCore.ChangeBlockStart);
        Assert.Equal(2533, CopyMonRecalcAddCore.ChangeBlockEnd);
        Assert.Equal(9, CopyMonRecalcAddCore.TailBlockLines);
        Assert.Equal(2534, CopyMonRecalcAddCore.TailBlockStart);
        Assert.Equal(2542, CopyMonRecalcAddCore.TailBlockEnd);
        Assert.Equal(12, CopyMonRecalcAddCore.TemplatedFieldCount);
        Assert.Equal(12, CopyMonRecalcAddCore.ScaledFieldCount);
        Assert.Equal(2, CopyMonRecalcAddCore.CommentedHpMpSites);
        Assert.Equal(8, CopyMonRecalcAddCore.CastSiteCount);
        Assert.Equal(2, CopyMonRecalcAddCore.ResetFieldCount);
        Assert.Equal(31, CopyMonRecalcAddCore.SetHmpBlockLines);
        Assert.Equal(2502, CopyMonRecalcAddCore.SetHmpBlockStart);
        Assert.Equal(2532, CopyMonRecalcAddCore.SetHmpBlockEnd);
        Assert.Equal(2529, CopyMonRecalcAddCore.MpClampBugLine);
        Assert.Equal(2515, CopyMonRecalcAddCore.HpClampLine);
        Assert.Equal(2473, CopyMonRecalcAddCore.WalkSpeedPercentLine);
        Assert.Equal(2489, CopyMonRecalcAddCore.NextHitTimePercentLine);
        Assert.Equal(32767, CopyMonRecalcAddCore.SmallIntHigh);
        Assert.Equal(-32768, CopyMonRecalcAddCore.SmallIntLow);
        Assert.Equal(10, CopyMonRecalcAddCore.DocumentedSpeedHigh);
        Assert.Equal(-10, CopyMonRecalcAddCore.DocumentedSpeedLow);
        Assert.Equal(385, CopyMonRecalcAddCore.RunLines);
        Assert.Equal(4, CopyMonRecalcAddCore.ShellRecalcLines);
        Assert.Equal(2275, CopyMonRecalcAddCore.ShellRecalcStart);
        Assert.Equal(5, CopyMonRecalcAddCore.HeaderLines);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(CopyMonRecalcAddCore.SpanMatches());
        Assert.True(CopyMonRecalcAddCore.BlocksSumTo258());
        Assert.True(CopyMonRecalcAddCore.HeaderIsFiveLines());
        Assert.True(CopyMonRecalcAddCore.BlocksOrdered());
        Assert.True(CopyMonRecalcAddCore.SetHmpInsideChangeBlock());
        Assert.True(CopyMonRecalcAddCore.BugLineInsideSetHmp());
        Assert.True(CopyMonRecalcAddCore.ShellPrecedesBody());
        Assert.True(CopyMonRecalcAddCore.SecondLongestInClass());
        Assert.True(CopyMonRecalcAddCore.LengthFromRepetition());
        Assert.True(CopyMonRecalcAddCore.NoInstrumentation());
    }

    // ===================== 一、MP 夹取笔误 =====================

    [Fact]
    public void CrossFieldTypoFacts()
    {
        Assert.True(CopyMonRecalcAddCore.MpComparesMaxHp());
        Assert.True(CopyMonRecalcAddCore.HpComparesMaxHp());
        Assert.True(CopyMonRecalcAddCore.CrossFieldTypo());
        Assert.True(CopyMonRecalcAddCore.OnlyMpIsWrong());
        Assert.True(CopyMonRecalcAddCore.HpIsSelfConsistent());
        Assert.True(CopyMonRecalcAddCore.RequiresSetHmpFlag());
        Assert.True(CopyMonRecalcAddCore.RequiresNonZeroMp());
        Assert.True(CopyMonRecalcAddCore.NotReachableByDefault());
        Assert.True(CopyMonRecalcAddCore.BugLineExtracted());
    }

    [Fact]
    public void MpClampDiverges()
    {
        Assert.True(CopyMonRecalcAddCore.MpClampDiverges());
        Assert.True(CopyMonRecalcAddCore.MpClampAgreesWhenEqual());

        // **MaxHP 大、MP 未达 MaxMP：原文不夹、修正会夹**
        Assert.Equal(600, CopyMonRecalcAddCore.MpClampAsWritten(1000, 500, 600));
        Assert.Equal(500, CopyMonRecalcAddCore.MpClampIfCorrected(1000, 500, 600));

        // **MaxHP 小、MP 超过 MaxHP 但未达 MaxMP：原文过早夹、修正不夹**
        Assert.Equal(500, CopyMonRecalcAddCore.MpClampAsWritten(300, 500, 400));
        Assert.Equal(400, CopyMonRecalcAddCore.MpClampIfCorrected(300, 500, 400));
    }

    [Fact]
    public void HpClampIsSelfConsistent()
    {
        Assert.True(CopyMonRecalcAddCore.HpClampSameField());

        // **两侧同为 MaxHP：超过则夹、未超则留**
        Assert.Equal(100, CopyMonRecalcAddCore.HpClampAsWritten(100, 150));
        Assert.Equal(50, CopyMonRecalcAddCore.HpClampAsWritten(100, 50));
    }

    // ===================== 二、继承比例块 =====================

    [Fact]
    public void InheritBlockFacts()
    {
        Assert.True(CopyMonRecalcAddCore.TwoMirrorBranches());
        Assert.True(CopyMonRecalcAddCore.TenFieldsEach());
        Assert.True(CopyMonRecalcAddCore.OnlySourceDiffers());
        Assert.True(CopyMonRecalcAddCore.TwelveFields());
        Assert.True(CopyMonRecalcAddCore.SixCombatPairs());
        Assert.True(CopyMonRecalcAddCore.HpMpCommentedOutInBoth());
        Assert.True(CopyMonRecalcAddCore.MaxKeptInBoth());
        Assert.True(CopyMonRecalcAddCore.OnlyCapsScaled());
        Assert.True(CopyMonRecalcAddCore.SkipsWhen100());
        Assert.True(CopyMonRecalcAddCore.DefaultIsFullInherit());
        Assert.True(CopyMonRecalcAddCore.CommentedLinesExtracted());
    }

    [Fact]
    public void CommentedHpMpLines()
    {
        Assert.Equal(new[] { 2300, 2301, 2317, 2318 }, CopyMonRecalcAddCore.CommentedHpMpLines);
    }

    [Fact]
    public void EntersInheritBlock()
    {
        Assert.True(CopyMonRecalcAddCore.EntersInheritBlockValues());

        Assert.False(CopyMonRecalcAddCore.EntersInheritBlock(100));
        Assert.True(CopyMonRecalcAddCore.EntersInheritBlock(99));
        Assert.True(CopyMonRecalcAddCore.EntersInheritBlock(101));
    }

    [Fact]
    public void IntegerDivisionLosesPrecision()
    {
        Assert.True(CopyMonRecalcAddCore.IntegerDivisionFirst());
        Assert.True(CopyMonRecalcAddCore.LosesPrecision());
        Assert.True(CopyMonRecalcAddCore.NotTheAlgebraicEquivalent());
        Assert.True(CopyMonRecalcAddCore.ScaleLosesSmallValues());
        Assert.True(CopyMonRecalcAddCore.ValuesBelow100Vanish());

        // **155 / 100 = 1（丢 55）→ 50；代数写法 = 78**
        Assert.Equal(50, CopyMonRecalcAddCore.ScaleAsWritten(155, 50));
        Assert.Equal(78, CopyMonRecalcAddCore.ScaleAlgebraic(155, 50));

        // **小于一百的值在 1..99 的百分比下全部归零**
        Assert.Equal(0, CopyMonRecalcAddCore.ScaleAsWritten(99, 99));
    }

    // ===================== 三、模板 =====================

    [Fact]
    public void TemplateFacts()
    {
        Assert.True(CopyMonRecalcAddCore.SameTemplateTwelveTimes());
        Assert.True(CopyMonRecalcAddCore.FourStepsPerField());
        Assert.True(CopyMonRecalcAddCore.MaxHpMpAreSpecial());
        Assert.True(CopyMonRecalcAddCore.AssignNotAdd());
        Assert.True(CopyMonRecalcAddCore.DatedChangeComment());
        Assert.True(CopyMonRecalcAddCore.OldFormKeptInComment());
        Assert.True(CopyMonRecalcAddCore.TemplatedFieldsExtracted());
    }

    [Fact]
    public void TemplatedFields()
    {
        Assert.Equal(12, CopyMonRecalcAddCore.TemplatedFields.Length);
        Assert.Equal("MaxHP", CopyMonRecalcAddCore.TemplatedFields[0]);
        Assert.Equal("MaxMP", CopyMonRecalcAddCore.TemplatedFields[1]);
        Assert.Equal("AC1", CopyMonRecalcAddCore.TemplatedFields[2]);
        Assert.Equal("SC2", CopyMonRecalcAddCore.TemplatedFields[11]);
    }

    [Fact]
    public void CastSites()
    {
        Assert.True(CopyMonRecalcAddCore.CastOnlyForFourFields());
        Assert.True(CopyMonRecalcAddCore.EightCastSites());

        Assert.Equal(new[] { 2329, 2331, 2341, 2343, 2507, 2509, 2521, 2523 },
            CopyMonRecalcAddCore.CastLines);
    }

    [Fact]
    public void TwoZeroSemantics()
    {
        Assert.True(CopyMonRecalcAddCore.TwoFieldsHaveElse());
        Assert.True(CopyMonRecalcAddCore.ZeroMeansResetHere());
        Assert.True(CopyMonRecalcAddCore.ZeroMeansNoChangeThere());
        Assert.True(CopyMonRecalcAddCore.TwoZeroSemantics());

        Assert.Equal(new[] { "WalkSpeed", "NextHitTime" }, CopyMonRecalcAddCore.ResetFields);
    }

    [Fact]
    public void WalkSpeedMixedBase()
    {
        Assert.True(CopyMonRecalcAddCore.MixedBaseAndScaled());
        Assert.True(CopyMonRecalcAddCore.WalkSpeedUsesInitAsBase());
        Assert.True(CopyMonRecalcAddCore.NextHitTimeSamePattern());
        Assert.True(CopyMonRecalcAddCore.BothBranchesUseInit());
        Assert.True(CopyMonRecalcAddCore.WalkSpeedDiverges());
        Assert.True(CopyMonRecalcAddCore.WalkSpeedBaseTakesEffect());
        Assert.True(CopyMonRecalcAddCore.FlatBranchUsesInit());

        // **基数用初始值 → 1900；若照其余十处模板 → 1500**
        Assert.Equal(1900, CopyMonRecalcAddCore.WalkSpeedPercentAsWritten(1400, 1000, 50));
        Assert.Equal(1500, CopyMonRecalcAddCore.WalkSpeedPercentTemplated(1400, 1000, 50));

        // **非百分比分支：初始值 + 变更值**
        Assert.Equal(1500, CopyMonRecalcAddCore.WalkSpeedFlatAsWritten(1400, 100));
    }

    [Fact]
    public void Clamping()
    {
        Assert.True(CopyMonRecalcAddCore.ClampZeroToHigh());
        Assert.True(CopyMonRecalcAddCore.IfElseIfOrder());
        Assert.True(CopyMonRecalcAddCore.EquivalentToClamp());
        Assert.True(CopyMonRecalcAddCore.ClampValues());
        Assert.True(CopyMonRecalcAddCore.WideIntermediate());
        Assert.True(CopyMonRecalcAddCore.NarrowedAfterClamp());
        Assert.True(CopyMonRecalcAddCore.SafeNarrowing());

        // **负值归零、超上界夹到上界**
        Assert.Equal(0, CopyMonRecalcAddCore.ClampAsWritten(-5, 100));
        Assert.Equal(100, CopyMonRecalcAddCore.ClampAsWritten(150, 100));
        Assert.Equal(50, CopyMonRecalcAddCore.ClampAsWritten(50, 100));
    }

    // ===================== 四、攻击速度夹取 =====================

    [Fact]
    public void SpeedClampFacts()
    {
        Assert.True(CopyMonRecalcAddCore.BoundsAreSmallInt());
        Assert.True(CopyMonRecalcAddCore.NotTheDocumentedRange());
        Assert.True(CopyMonRecalcAddCore.ClampIsNearlyInert());
        Assert.True(CopyMonRecalcAddCore.ClampDoesNotEnforceDocumentedRange());
        Assert.True(CopyMonRecalcAddCore.BeyondDocumentedRangeNotClamped());
        Assert.True(CopyMonRecalcAddCore.ClampsOnlyAtOverflow());
        Assert.True(CopyMonRecalcAddCore.HighBoundaryInclusive());
        Assert.True(CopyMonRecalcAddCore.LowBoundaryInclusive());
        Assert.True(CopyMonRecalcAddCore.NTempIsInteger());
        Assert.True(CopyMonRecalcAddCore.ClampedBeforeNarrow());
    }

    [Fact]
    public void DocumentedRangeIsNotEnforced()
    {
        // **注释声明 -10 ~ +10，但代码用 SmallInt 范围**
        Assert.Equal(10, CopyMonRecalcAddCore.ClampSpeedAsWritten(10));
        Assert.Equal(-10, CopyMonRecalcAddCore.ClampSpeedAsWritten(-10));

        // **远超文档范围也不被夹**
        Assert.Equal(1000, CopyMonRecalcAddCore.ClampSpeedAsWritten(1000));
        Assert.Equal(-1000, CopyMonRecalcAddCore.ClampSpeedAsWritten(-1000));

        // **只在真正超出 SmallInt 时才夹**
        Assert.Equal(32767, CopyMonRecalcAddCore.ClampSpeedAsWritten(40000));
        Assert.Equal(-32768, CopyMonRecalcAddCore.ClampSpeedAsWritten(-40000));
    }

    [Fact]
    public void TailRefresh()
    {
        Assert.True(CopyMonRecalcAddCore.CallsRefGameSpeed());
        Assert.True(CopyMonRecalcAddCore.RefreshIsMandatory());
        Assert.True(CopyMonRecalcAddCore.DatedSignedComment());
        Assert.True(CopyMonRecalcAddCore.StatesIntent());
        Assert.True(CopyMonRecalcAddCore.IntentNotAchieved());
        Assert.True(CopyMonRecalcAddCore.DatedLinesExtracted());

        Assert.Equal(new[] { 2327, 2331, 2534 }, CopyMonRecalcAddCore.DatedCommentLines);
    }
}

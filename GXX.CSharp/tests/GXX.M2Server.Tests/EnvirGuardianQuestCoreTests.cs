using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J166：`TEnvirnoment` 守护等级流程与坐标查询族 1:1 测试。
/// **运算符优先级缺陷用四组越界值加网格穷举固证、
/// 方向塌缩用去重计数、条件强度不对称用逐条真值表。**
/// </summary>
public sealed class EnvirGuardianQuestCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void MonRangeConstants()
    {
        Assert.Equal(3, EnvirGuardianQuestCore.MonRange);
        Assert.Equal(7, EnvirGuardianQuestCore.RandomWidth());
        Assert.True(EnvirGuardianQuestCore.RandomWidthIsSeven());
        Assert.True(EnvirGuardianQuestCore.CentredOnSpawnPoint());
    }

    [Fact]
    public void RoundLimit()
    {
        // **判定是 I > 10，所以最多十一轮**
        Assert.Equal(11, EnvirGuardianQuestCore.MaxRounds());
        Assert.True(EnvirGuardianQuestCore.ElevenRounds());
        Assert.True(EnvirGuardianQuestCore.ElevenNotTen());
        Assert.True(EnvirGuardianQuestCore.FirstRoundAlwaysRuns());
        Assert.Equal(11, EnvirGuardianQuestCore.ExitAtI());
        Assert.True(EnvirGuardianQuestCore.BoundaryAtEleven());
    }

    [Fact]
    public void RoundCountValues()
    {
        Assert.True(EnvirGuardianQuestCore.RoundCountValues());
        Assert.Equal(1, EnvirGuardianQuestCore.RoundCount(true));
        Assert.Equal(11, EnvirGuardianQuestCore.RoundCount(false));
    }

    [Fact]
    public void MessageConstants()
    {
        Assert.Equal(20298, EnvirGuardianQuestCore.RmGuardianLevelBatchInfo);
        Assert.Equal(20299, EnvirGuardianQuestCore.RmGuardianLevelResult);
        Assert.True(EnvirGuardianQuestCore.AdjacentMessages());
    }

    [Fact]
    public void ItemCountsStruct()
    {
        // **两组各四整数、packed 共三十二字节**
        Assert.Equal(32, EnvirGuardianQuestCore.ItemCountsSize());
        Assert.True(EnvirGuardianQuestCore.ItemCountsSizeIs32());
        Assert.Equal(8, EnvirGuardianQuestCore.ItemCountSlots());
        Assert.True(EnvirGuardianQuestCore.EightSlots());
    }

    [Fact]
    public void PlayObjectRace()
    {
        Assert.Equal(0, EnvirGuardianQuestCore.RcPlayObject);
        Assert.True(EnvirGuardianQuestCore.RcPlayObjectIsZero());
        Assert.True(EnvirGuardianQuestCore.OnlyPlayObjects());
        Assert.True(EnvirGuardianQuestCore.IsPlayObjectRace(0));
        Assert.False(EnvirGuardianQuestCore.IsPlayObjectRace(10));
    }

    // ===================== 一、GetSitInLinPosition =====================

    [Fact]
    public void PrecedenceDefect()
    {
        // **末条件是 or 而不是 and**
        Assert.True(EnvirGuardianQuestCore.LastConditionIsOrNotAnd());
        Assert.True(EnvirGuardianQuestCore.PrecedenceMakesOutOfBoundsValid());
        Assert.True(EnvirGuardianQuestCore.NinetyNineXEscapesViaY());
        Assert.True(EnvirGuardianQuestCore.IntendedRejectsAllFour());
    }

    [Fact]
    public void OutOfBoundsAccepted()
    {
        // **四个越界输入全部被判为有效**
        Assert.True(EnvirGuardianQuestCore.Widget(-5, 50, 100, 100));
        Assert.True(EnvirGuardianQuestCore.Widget(999, 50, 100, 100));
        Assert.True(EnvirGuardianQuestCore.Widget(50, 999, 100, 100));
        Assert.True(EnvirGuardianQuestCore.Widget(50, -5, 100, 100));

        // **而本意写法会把它们全拒掉**
        Assert.False(EnvirGuardianQuestCore.Intended(-5, 50, 100, 100));
        Assert.False(EnvirGuardianQuestCore.Intended(999, 50, 100, 100));
        Assert.False(EnvirGuardianQuestCore.Intended(50, 999, 100, 100));
        Assert.False(EnvirGuardianQuestCore.Intended(50, -5, 100, 100));
    }

    [Fact]
    public void GridDifferCount()
    {
        // **五百二十九格里一百二十三格不同**
        Assert.Equal(123, EnvirGuardianQuestCore.GridDifferCount());
        Assert.True(EnvirGuardianQuestCore.GridDifferCountIs123());
        Assert.Equal(529, EnvirGuardianQuestCore.GridTotal());
        Assert.True(EnvirGuardianQuestCore.GridTotalIs529());
        Assert.True(EnvirGuardianQuestCore.GridSameIs406());
        Assert.True(EnvirGuardianQuestCore.GridReproduces());
    }

    [Fact]
    public void GridEnumeration()
    {
        var (d, s) = EnvirGuardianQuestCore.RunGrid();
        Assert.Equal(123, d);
        Assert.Equal(406, s);
        Assert.Equal(EnvirGuardianQuestCore.GridTotal(), d + s);
    }

    // ---------- 方向映射 ----------

    [Fact]
    public void AxesAreSwapped()
    {
        // **"上下"改横坐标、"左右"改纵坐标**
        Assert.True(EnvirGuardianQuestCore.AxesAreSwapped());
        Assert.True(EnvirGuardianQuestCore.UpDownChangesX());
        Assert.True(EnvirGuardianQuestCore.LeftRightChangesY());
    }

    [Fact]
    public void OppositeDirectionsCollapse()
    {
        // **"上"等于"下"、"左"等于"右"**
        Assert.True(EnvirGuardianQuestCore.UpEqualsDown());
        Assert.True(EnvirGuardianQuestCore.LeftEqualsRight());
    }

    [Fact]
    public void DiagonalsCollapse()
    {
        Assert.True(EnvirGuardianQuestCore.FourDiagonalsCollapseToTwo());
        Assert.True(EnvirGuardianQuestCore.AllEightDirectionsCollapseToFour());
        Assert.True(EnvirGuardianQuestCore.EightInFourOut());
    }

    [Fact]
    public void EightDirectionsFourResults()
    {
        var seen = new List<(int, int)>();

        for (int d = 0; d <= 7; d++)
        {
            var o = EnvirGuardianQuestCore.Offset(d, 5);
            Assert.NotNull(o);

            if (!seen.Contains(o!.Value))
            {
                seen.Add(o.Value);
            }
        }

        Assert.Equal(4, seen.Count);
    }

    [Fact]
    public void InvalidDirection()
    {
        Assert.True(EnvirGuardianQuestCore.InvalidDirectionReturnsNull());
        Assert.Null(EnvirGuardianQuestCore.Offset(8, 5));
        Assert.Null(EnvirGuardianQuestCore.Offset(-1, 5));
    }

    [Fact]
    public void OutputsWrittenBeforeCase()
    {
        // **失败时输出参数仍被写成起点**
        Assert.True(EnvirGuardianQuestCore.DefaultsWrittenBeforeCase());
        Assert.True(EnvirGuardianQuestCore.FailStillWritesOutputs());
        Assert.True(EnvirGuardianQuestCore.SitInvalidReturnsStart());
        Assert.True(EnvirGuardianQuestCore.SitValidOffsets());
    }

    // ===================== 二、GetRangeXY =====================

    [Fact]
    public void UpperBoundExclusive()
    {
        // **上界开区间 —— 实际范围比名义半径少一格**
        Assert.True(EnvirGuardianQuestCore.UpperBoundExclusive());
        Assert.True(EnvirGuardianQuestCore.RangeIsOffsetByOne());
        Assert.Equal(47, EnvirGuardianQuestCore.SampleRange(50, 3).Lo);
        Assert.Equal(52, EnvirGuardianQuestCore.SampleRange(50, 3).Hi);
    }

    [Fact]
    public void ActualRangeWidth()
    {
        Assert.Equal(5, EnvirGuardianQuestCore.ActualRangeWidth(3));
        Assert.True(EnvirGuardianQuestCore.ActualRangeIsTwoRangMinusOne());
        Assert.NotEqual(7, EnvirGuardianQuestCore.ActualRangeWidth(3));
    }

    [Fact]
    public void DelphiRandomSemantics()
    {
        // **N <= 0 时返回零**
        Assert.True(EnvirGuardianQuestCore.RandomZeroReturnsZero());
        Assert.True(EnvirGuardianQuestCore.RandomNegativeReturnsZero());
        Assert.Equal(0, EnvirGuardianQuestCore.DelphiRandom(0, 12345));
        Assert.Equal(0, EnvirGuardianQuestCore.DelphiRandom(-7, 12345));
    }

    [Fact]
    public void ZeroAndNegativeRange()
    {
        Assert.True(EnvirGuardianQuestCore.ZeroRangeDegeneratesToCenter());
        Assert.True(EnvirGuardianQuestCore.ZeroRangeValues());
        Assert.Equal(50, EnvirGuardianQuestCore.ZeroRangeSample(50).Lo);
        Assert.Equal(49, EnvirGuardianQuestCore.ZeroRangeSample(50).Hi);

        Assert.True(EnvirGuardianQuestCore.NegativeRangeBehaviour());
        Assert.True(EnvirGuardianQuestCore.NegativeRangeValues());
        Assert.Equal(53, EnvirGuardianQuestCore.NegativeRangeSample(50, -3).Lo);
        Assert.Equal(46, EnvirGuardianQuestCore.NegativeRangeSample(50, -3).Hi);
    }

    [Fact]
    public void ResultStartsFalse()
    {
        Assert.True(EnvirGuardianQuestCore.ResultStartsFalse());
        Assert.True(EnvirGuardianQuestCore.ExceptionReturnsFalse());
        Assert.False(EnvirGuardianQuestCore.RangeOnExceptionValue());
    }

    [Fact]
    public void MessageFormatInconsistency()
    {
        // **用冒号而不是点号 —— 与 J165 两条格式不同**
        Assert.True(EnvirGuardianQuestCore.MessageUsesColonNotDot());
        Assert.True(EnvirGuardianQuestCore.RangeMsgHasColon());
        Assert.True(EnvirGuardianQuestCore.RangeMsgHasNoDotSeparator());
        Assert.True(EnvirGuardianQuestCore.FormatInconsistency());
        Assert.Contains(":", EnvirGuardianQuestCore.RangeExceptionMsg, StringComparison.Ordinal);
        Assert.DoesNotContain("TEnvirnoment.", EnvirGuardianQuestCore.RangeExceptionMsg, StringComparison.Ordinal);
        Assert.Equal(2, EnvirGuardianQuestCore.DotStyleMessages.Length);
    }

    // ===================== 三、PorcessGuardianLevelInfo =====================

    [Fact]
    public void GateStructure()
    {
        Assert.True(EnvirGuardianQuestCore.TwoAndThreeConditionGates());
        Assert.True(EnvirGuardianQuestCore.TopGateTruthTable());
        Assert.True(EnvirGuardianQuestCore.TopGate(true, true));
        Assert.False(EnvirGuardianQuestCore.TopGate(true, false));
        Assert.False(EnvirGuardianQuestCore.TopGate(false, true));
    }

    [Fact]
    public void BatchNoLessOrEqual()
    {
        // **允许波数等于总波数**
        Assert.True(EnvirGuardianQuestCore.BatchNoLessOrEqual());
        Assert.True(EnvirGuardianQuestCore.SecondGate(5, 5, true, true));
        Assert.False(EnvirGuardianQuestCore.SecondGate(6, 5, true, true));
        Assert.True(EnvirGuardianQuestCore.SecondGateNeedsAllThree());
    }

    [Fact]
    public void StatuePrecedence()
    {
        Assert.True(EnvirGuardianQuestCore.PrecedenceNotAsIntended());
        Assert.True(EnvirGuardianQuestCore.GhostAloneTriggersFailure());
        Assert.True(EnvirGuardianQuestCore.IntendedWouldNotTrigger());
    }

    [Fact]
    public void StatueDifferCount()
    {
        // **八组里两组不同（我最初凭推断写成一组、探针实测两组）**
        Assert.Equal(2, EnvirGuardianQuestCore.StatuePrecedenceDifferCount());
        Assert.True(EnvirGuardianQuestCore.StatueDifferIsTwo());
        Assert.True(EnvirGuardianQuestCore.BothDifferRowsHaveGhostAndSuccess());
        Assert.True(EnvirGuardianQuestCore.GhostBypassesSuccessGuard());
    }

    [Fact]
    public void StatueTruthTable()
    {
        // 幽灵真、死亡假、已成功真 → 写成式为真、本意为假
        Assert.True(EnvirGuardianQuestCore.StatueFailAsWritten(true, false, true));
        Assert.False(EnvirGuardianQuestCore.StatueFailAsIntended(true, false, true));

        // 幽灵真、死亡真、已成功真 → 同样分歧
        Assert.True(EnvirGuardianQuestCore.StatueFailAsWritten(true, true, true));
        Assert.False(EnvirGuardianQuestCore.StatueFailAsIntended(true, true, true));

        // 幽灵假、死亡真、已成功真 → 两者一致（都不触发）
        Assert.False(EnvirGuardianQuestCore.StatueFailAsWritten(false, true, true));
        Assert.False(EnvirGuardianQuestCore.StatueFailAsIntended(false, true, true));
    }

    [Fact]
    public void ResultCodes()
    {
        Assert.True(EnvirGuardianQuestCore.FailSendsZero());
        Assert.True(EnvirGuardianQuestCore.SuccessSendsOne());
        Assert.True(EnvirGuardianQuestCore.ResultCodeIsFirstParam());
        Assert.True(EnvirGuardianQuestCore.OnlyTwoCodes());
        Assert.Equal(0, EnvirGuardianQuestCore.FailResultCode());
        Assert.Equal(1, EnvirGuardianQuestCore.SuccessResultCode());
    }

    [Fact]
    public void DuplicateCode()
    {
        Assert.True(EnvirGuardianQuestCore.TwoCopiesOfEach());
        Assert.Equal(2, EnvirGuardianQuestCore.CopyCount());
        Assert.True(EnvirGuardianQuestCore.RewardCopyDuplicated());
        Assert.True(EnvirGuardianQuestCore.RewardCopyTotalIs32());
        Assert.True(EnvirGuardianQuestCore.StructFillTotalIs16());
        Assert.True(EnvirGuardianQuestCore.AccumulateDuplicated());
    }

    [Fact]
    public void DuplicateShare()
    {
        Assert.True(EnvirGuardianQuestCore.DuplicateLinesAbout56());
        Assert.Equal(56, EnvirGuardianQuestCore.DuplicateLinesAbout());
        Assert.True(EnvirGuardianQuestCore.DuplicateShareExceedsThird());
        Assert.True(EnvirGuardianQuestCore.ShareArithmetic());
        Assert.Equal(166, EnvirGuardianQuestCore.MethodLines());
    }

    [Fact]
    public void AccumulateIndex()
    {
        // **累加上一波（波数减一）**
        Assert.True(EnvirGuardianQuestCore.AccumulatesPreviousBatch());
        Assert.True(EnvirGuardianQuestCore.AccumulateIndexValues());
        Assert.True(EnvirGuardianQuestCore.AccumulateOnlyWhenBatchAtLeastOne());
        Assert.Equal(0, EnvirGuardianQuestCore.AccumulateIndex(1));
        Assert.Equal(4, EnvirGuardianQuestCore.AccumulateIndex(5));
    }

    [Fact]
    public void CurCountsIndex()
    {
        Assert.True(EnvirGuardianQuestCore.CurCountsUseIncrementedBatch());
        Assert.Equal(2, EnvirGuardianQuestCore.CurCountsIndex(3));
        Assert.True(EnvirGuardianQuestCore.CurIndexEqualsOldBatchNo());
        Assert.True(EnvirGuardianQuestCore.AccumulateAndCurDifferByOne());
        Assert.True(EnvirGuardianQuestCore.SameIndexDifferentMeaning());
    }

    [Fact]
    public void SuccessBranch()
    {
        Assert.True(EnvirGuardianQuestCore.SuccessBranchNoIncrement());
        Assert.True(EnvirGuardianQuestCore.SuccessUsesLastBatch());
        Assert.True(EnvirGuardianQuestCore.SuccessSetsFlagAndClearsStart());
        Assert.True(EnvirGuardianQuestCore.ThreeSuccessTerminals());
        Assert.Equal(3, EnvirGuardianQuestCore.SuccessTerminals.Length);
        Assert.True(EnvirGuardianQuestCore.NoMonCountResetButGuarded());
        Assert.True(EnvirGuardianQuestCore.NoReentryBecauseStartCleared());
    }

    [Fact]
    public void FailBranch()
    {
        Assert.True(EnvirGuardianQuestCore.FailAlsoClearsStart());
        Assert.True(EnvirGuardianQuestCore.FailGrantsThenClears());
        Assert.True(EnvirGuardianQuestCore.FailExitsAfterClearing());
    }

    [Fact]
    public void PlayerAbort()
    {
        // **玩家幽灵/死亡最优先、且不发奖励**
        Assert.True(EnvirGuardianQuestCore.PlayerGhostOrDeathFirst());
        Assert.True(EnvirGuardianQuestCore.PlayerAbortsBoth());
        Assert.True(EnvirGuardianQuestCore.PlayerAborts(true, false));
        Assert.True(EnvirGuardianQuestCore.PlayerAborts(false, true));
        Assert.False(EnvirGuardianQuestCore.PlayerAborts(false, false));
        Assert.True(EnvirGuardianQuestCore.AbortResetsWithoutReward());
        Assert.True(EnvirGuardianQuestCore.AbortVsStatueFail());
        Assert.Equal(2, EnvirGuardianQuestCore.AbortVsFail.Length);
    }

    [Fact]
    public void LabelsMisspelled()
    {
        // **两个标签都少一个 r**
        Assert.True(EnvirGuardianQuestCore.BothLabelsMisspelled());
        Assert.True(EnvirGuardianQuestCore.NeitherHasCorrectSpelling());
        Assert.True(EnvirGuardianQuestCore.SameMisspellingFamilyAsJ165());
        Assert.Contains("Guardina", EnvirGuardianQuestCore.FailLabel, StringComparison.Ordinal);
        Assert.Contains("Guardina", EnvirGuardianQuestCore.SuccessLabel, StringComparison.Ordinal);
        Assert.Equal("@GuardinaLevelFail", EnvirGuardianQuestCore.FailLabel);
        Assert.Equal("@GuardinaLevelSuccess", EnvirGuardianQuestCore.SuccessLabel);
    }

    [Fact]
    public void GotoLabelParam()
    {
        Assert.True(EnvirGuardianQuestCore.GotoLabelThirdParamFalse());
        Assert.True(EnvirGuardianQuestCore.TwoNullGuardedJumps());
    }

    [Fact]
    public void NullFunctionNpc()
    {
        Assert.True(EnvirGuardianQuestCore.NullFunctionNpcSkipsJump());
        Assert.True(EnvirGuardianQuestCore.JumpModelValues());
        Assert.True(EnvirGuardianQuestCore.WouldJump(true));
        Assert.False(EnvirGuardianQuestCore.WouldJump(false));
    }

    // ---------- 刷怪解析 ----------

    [Fact]
    public void Delimiters()
    {
        Assert.True(EnvirGuardianQuestCore.PipeAndColonDelimiters());
        Assert.True(EnvirGuardianQuestCore.TwoDelimiters());
        Assert.Equal(new[] { '|', ':' }, EnvirGuardianQuestCore.Delimiters);
    }

    [Fact]
    public void ParseMon()
    {
        Assert.True(EnvirGuardianQuestCore.DefaultCountIsOne());
        Assert.True(EnvirGuardianQuestCore.ColonCountParsed());
        Assert.True(EnvirGuardianQuestCore.BadIntFallsBackToOne());
        Assert.True(EnvirGuardianQuestCore.ColonAtStartTreatedAsNoColon());

        Assert.Equal(("稻草人", 1), EnvirGuardianQuestCore.ParseMon("稻草人"));
        Assert.Equal(("稻草人", 5), EnvirGuardianQuestCore.ParseMon("稻草人:5"));
        Assert.Equal(("稻草人", 1), EnvirGuardianQuestCore.ParseMon("稻草人:abc"));
    }

    [Fact]
    public void SplitMons()
    {
        Assert.True(EnvirGuardianQuestCore.EmptyNameBreaks());
        Assert.True(EnvirGuardianQuestCore.EmptyRestBreaks());
        Assert.True(EnvirGuardianQuestCore.SplitThreeItems());
        Assert.True(EnvirGuardianQuestCore.SplitEmptyGivesZero());
        Assert.True(EnvirGuardianQuestCore.LeadingBarBreaksImmediately());
        Assert.True(EnvirGuardianQuestCore.MiddleEmptyBreaksAndTruncates());

        Assert.Equal(3, EnvirGuardianQuestCore.SplitMons("a:1|b:2|c:3").Count);
        Assert.Empty(EnvirGuardianQuestCore.SplitMons(""));
        Assert.Empty(EnvirGuardianQuestCore.SplitMons("|a:1"));
        Assert.Single(EnvirGuardianQuestCore.SplitMons("a:1||b:2"));
    }

    // ---------- 刷怪字段 ----------

    [Fact]
    public void MissionFields()
    {
        // **索引 -1 却给了恰一个点**
        Assert.True(EnvirGuardianQuestCore.AttackTargetIsNameNotPointer());
        Assert.True(EnvirGuardianQuestCore.MissionIndexMinusOne());
        Assert.True(EnvirGuardianQuestCore.MissionIndexIsMinusOne());
        Assert.True(EnvirGuardianQuestCore.MissionPointsLengthOne());
        Assert.True(EnvirGuardianQuestCore.MissionLengthIsOne());
        Assert.True(EnvirGuardianQuestCore.IndexMinusOneButOnePoint());
        Assert.True(EnvirGuardianQuestCore.SingleMissionPointAtStatue());
        Assert.Equal(-1, EnvirGuardianQuestCore.MissionPointIndex());
        Assert.Equal(1, EnvirGuardianQuestCore.MissionPointsLength());
    }

    [Fact]
    public void SpawnCounting()
    {
        // **只有成功才计数 —— 全失败会重试同一波**
        Assert.True(EnvirGuardianQuestCore.OnlyCountSuccessfulSpawns());
        Assert.True(EnvirGuardianQuestCore.CountSpawnsValues());
        Assert.Equal(2, EnvirGuardianQuestCore.CountSpawns(3, 2));
        Assert.Equal(0, EnvirGuardianQuestCore.CountSpawns(3, 0));
        Assert.True(EnvirGuardianQuestCore.FailedSpawnsRetrySameBatch());
        Assert.True(EnvirGuardianQuestCore.AdvanceBatchValues());
    }

    [Fact]
    public void AdvanceBatchValues()
    {
        Assert.Equal(2, EnvirGuardianQuestCore.AdvanceBatch(0, 1, 3));
        Assert.Equal(1, EnvirGuardianQuestCore.AdvanceBatch(5, 1, 3));
        Assert.Equal(3, EnvirGuardianQuestCore.AdvanceBatch(0, 3, 3));
    }

    [Fact]
    public void MonCountDirection()
    {
        // **本方法只增不减**
        Assert.True(EnvirGuardianQuestCore.MonCountOnlyIncrementedHere());
        Assert.True(EnvirGuardianQuestCore.DecrementElsewhere());
    }

    // ===================== 四、GetQuestNPC =====================

    [Fact]
    public void QuestGate()
    {
        Assert.True(EnvirGuardianQuestCore.GateSimplification());
        Assert.True(EnvirGuardianQuestCore.GateEquivalenceExhaustive());
        Assert.True(EnvirGuardianQuestCore.FalseFlagBypasses());
        Assert.True(EnvirGuardianQuestCore.TrueFlagRequiresMatch());
        Assert.True(EnvirGuardianQuestCore.ValueMismatchAlwaysFails());
    }

    [Fact]
    public void QuestGateTruthTable()
    {
        // boFlag 为假时布尔那一半无条件通过
        Assert.True(EnvirGuardianQuestCore.QuestGate(1, 1, false, true));
        Assert.True(EnvirGuardianQuestCore.QuestGate(1, 1, false, false));

        // boFlag 为真时必须一致
        Assert.True(EnvirGuardianQuestCore.QuestGate(1, 1, true, true));
        Assert.False(EnvirGuardianQuestCore.QuestGate(1, 1, true, false));

        // 数值不等一律不过
        Assert.False(EnvirGuardianQuestCore.QuestGate(1, 2, false, true));
        Assert.False(EnvirGuardianQuestCore.QuestGate(1, 2, true, true));
    }

    [Fact]
    public void ThreeConditions()
    {
        Assert.True(EnvirGuardianQuestCore.ThreeParallelConditions());
        Assert.True(EnvirGuardianQuestCore.CondAValues());
        Assert.True(EnvirGuardianQuestCore.SecondRequiresEmptyStr());
        Assert.True(EnvirGuardianQuestCore.ThirdIgnoresCharName());
        Assert.True(EnvirGuardianQuestCore.ThirdCharNameIrrelevant());
        Assert.True(EnvirGuardianQuestCore.AsymmetricChecks());
    }

    [Fact]
    public void ConditionStrength()
    {
        // **甲四条件、乙四条件、丙只有两条件**
        Assert.True(EnvirGuardianQuestCore.CondCountsAre442());
        Assert.Equal(new[] { 4, 4, 2 }, EnvirGuardianQuestCore.CondCheckCounts);

        Assert.True(EnvirGuardianQuestCore.CondA("n", "t", "n", "t"));
        Assert.False(EnvirGuardianQuestCore.CondA("n", "", "n", ""));
        Assert.True(EnvirGuardianQuestCore.CondB("n", "", "n", ""));
        Assert.False(EnvirGuardianQuestCore.CondB("n", "", "n", "x"));
        Assert.True(EnvirGuardianQuestCore.CondC("", "t", "ANYTHING", "t"));
    }

    [Fact]
    public void EmptyValueWildcard()
    {
        // **空 sValue 在丙里是通配**
        Assert.True(EnvirGuardianQuestCore.EmptyValueIsWildcardInThird());
        Assert.True(EnvirGuardianQuestCore.EmptyS08ForcesThirdPath());
        Assert.True(EnvirGuardianQuestCore.OnlyThirdCanHit());

        // sValue 为空时甲与乙必然为假
        Assert.False(EnvirGuardianQuestCore.CondA("", "t", "", "t"));
        Assert.False(EnvirGuardianQuestCore.CondB("", "", "", ""));
    }

    [Fact]
    public void VarValue()
    {
        Assert.True(EnvirGuardianQuestCore.CallerIsMerchant());
        Assert.True(EnvirGuardianQuestCore.HasBreakParseVarOut());
    }

    [Fact]
    public void FirstMatchWins()
    {
        Assert.True(EnvirGuardianQuestCore.FirstMatchWins());
        Assert.True(EnvirGuardianQuestCore.FirstMatchTakesZero());
        Assert.True(EnvirGuardianQuestCore.NoMatchGivesMinusOne());
        Assert.Equal(0, EnvirGuardianQuestCore.FirstMatchIndex(new[] { true, true }));
        Assert.Equal(-1, EnvirGuardianQuestCore.FirstMatchIndex(new[] { false, false }));
    }

    [Fact]
    public void NpcAssignment()
    {
        Assert.True(EnvirGuardianQuestCore.NpcAssignedWithoutCast());
    }

    // ---------- TMapQuestInfo 布局 ----------

    [Fact]
    public void QuestInfoLayout()
    {
        // **十三个字段、定长与变长字符串混用**
        Assert.Equal(13, EnvirGuardianQuestCore.QuestInfoFieldCount());
        Assert.True(EnvirGuardianQuestCore.ThirteenFields());
        Assert.True(EnvirGuardianQuestCore.MixedStringKinds());
        Assert.True(EnvirGuardianQuestCore.TwoVariableLengthStrings());
        Assert.True(EnvirGuardianQuestCore.FourFixedLengthStrings());
        Assert.True(EnvirGuardianQuestCore.SixStringFields());
        Assert.Equal(new[] { "s08", "s0C" }, EnvirGuardianQuestCore.VariableLengthStrings);
        Assert.Equal(4, EnvirGuardianQuestCore.FixedLengthStrings.Length);
    }

    [Fact]
    public void BareOffsetFamily()
    {
        // **三个裸偏移命名、两个有名字的布尔 —— 同记录并存**
        Assert.True(EnvirGuardianQuestCore.ThreeBareOffsets());
        Assert.True(EnvirGuardianQuestCore.ThreeBareOffsetNames());
        Assert.True(EnvirGuardianQuestCore.NamedAndBareCoexist());
        Assert.True(EnvirGuardianQuestCore.SameFamilyAsJ161());
        Assert.True(EnvirGuardianQuestCore.ThreeFamilyInstances());
        Assert.Equal(new[] { "s08", "s0C", "bo10" }, EnvirGuardianQuestCore.BareOffsetFields);
        Assert.Equal(2, EnvirGuardianQuestCore.NamedBooleanFields.Length);
        Assert.Equal(3, EnvirGuardianQuestCore.BareOffsetFamily.Length);
    }

    [Fact]
    public void BooleanFields()
    {
        Assert.Equal(3, EnvirGuardianQuestCore.BooleanFieldCount());
        Assert.True(EnvirGuardianQuestCore.ThreeBooleanFields());
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 25, 28, 166, 48 }, EnvirGuardianQuestCore.MethodLineCounts);
        Assert.True(EnvirGuardianQuestCore.FourMethods());
    }

    [Fact]
    public void LineExtremes()
    {
        Assert.True(EnvirGuardianQuestCore.GuardianIsLongest());
        Assert.True(EnvirGuardianQuestCore.RangeXYIsShortest());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirGuardianQuestCore.TotalLinesValues());
        Assert.Equal(267, EnvirGuardianQuestCore.TotalLines());
        Assert.True(EnvirGuardianQuestCore.GuardianShareIs62());
    }
}

using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J153：`CanWalk`（63 行）、`CanWalkEx`（440 行）、`MoveToMovingObject`（189 行）1:1 测试。
/// **三族开关命名、传送门区间、六条件门与三层 temp-hide 均经探针实测。**
/// </summary>
public sealed class CanWalkCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(CanWalkCore.ConstantsMatchSource());
        Assert.True(CanWalkCore.PortalRangesMatchSource());
        Assert.True(CanWalkCore.GuardRaceSet());
    }

    [Fact]
    public void RaceValues()
    {
        Assert.Equal(0, CanWalkCore.RcPlayObject);
        Assert.Equal(1, CanWalkCore.RcHeroObject);
        Assert.Equal(10, CanWalkCore.RcNpc);
        Assert.Equal(112, CanWalkCore.RcArcherGuard);
        Assert.Equal(142, CanWalkCore.RcMoveArcherGuard);
        Assert.Equal(150, CanWalkCore.RcPlayMaster);

        // **两个裸字面量**
        Assert.Equal(12, CanWalkCore.RcGuard2Literal);
        Assert.Equal(55, CanWalkCore.PracticeMasterLiteral);
    }

    [Fact]
    public void PortalRanges()
    {
        Assert.Equal(54, CanWalkCore.PortalLowFrom);
        Assert.Equal(58, CanWalkCore.PortalLowTo);
        Assert.Equal(94, CanWalkCore.PortalHighFrom);
        Assert.Equal(98, CanWalkCore.PortalHighTo);
    }

    // ===================== 一、三份重复循环 =====================

    [Fact]
    public void ThreeDuplicateLoops()
    {
        Assert.True(CanWalkCore.ThreeDuplicateLoops());
        Assert.Equal(3, CanWalkCore.FamilyPrefixes.Length);
    }

    [Fact]
    public void FamilyFlagNames()
    {
        Assert.True(CanWalkCore.FamilyPrefixMapping());
        Assert.True(CanWalkCore.FamiliesAreDisjoint());
        Assert.True(CanWalkCore.FamilyFlagCountValues());
        Assert.Equal(24, CanWalkCore.FamilyFlagCount());

        Assert.Equal("boHeroRunHum", CanWalkCore.FlagName("boHero", "RunHum"));
        Assert.Equal("boDummyRunMon", CanWalkCore.FlagName("boDummy", "RunMon"));
        Assert.Equal("boRunHum", CanWalkCore.FlagName("", "RunHum"));
    }

    [Fact]
    public void NamingConventionInverted()
    {
        // **安全区两个开关的族名在中间，与其余"族名在前"相反**
        Assert.True(CanWalkCore.ShopStallFlagNaming());
        Assert.True(CanWalkCore.OfflineFlagNaming());
        Assert.True(CanWalkCore.NamingConventionInverted());

        Assert.Equal("boSafeAreaDisShopStallHeroRun", CanWalkCore.ShopStallFlagName("Hero"));
        Assert.Equal("boSafeAreaDisOffLineDummyRun", CanWalkCore.OfflineFlagName("Dummy"));
    }

    [Fact]
    public void DummyExtraCommentedBlocks()
    {
        // **假人段多出两块被注释的代码**
        Assert.True(CanWalkCore.DummyHasTwoExtraCommentedBlocks());
        Assert.True(CanWalkCore.DummyCommentedBlocksCount());
        Assert.Equal(2, CanWalkCore.DummyCommentedBlocksComment.Length);
    }

    [Fact]
    public void ThirdSegmentDiffers()
    {
        // **第三段多一层 `WalkObject <> nil`；守卫分支结构也不同**
        Assert.True(CanWalkCore.ThirdHasWalkObjectNilCheck());
        Assert.True(CanWalkCore.SecondLacksWalkObjectNilCheck());
        Assert.True(CanWalkCore.GuardBranchStyleDiffers());
        Assert.True(CanWalkCore.GuardBranchStyleValues());

        Assert.NotEqual(CanWalkCore.GuardBranchStyle(0), CanWalkCore.GuardBranchStyle(2));
        Assert.Equal(CanWalkCore.GuardBranchStyle(0), CanWalkCore.GuardBranchStyle(1));
    }

    // ===================== 骨架 =====================

    [Fact]
    public void CellGate()
    {
        Assert.True(CanWalkCore.CellGateTruthTable());
        Assert.False(CanWalkCore.CellGateExitsFalse(true, 0));
        Assert.True(CanWalkCore.CellGateExitsFalse(true, 1));
    }

    [Fact]
    public void BooleanRewrite()
    {
        // **德摩根律的两侧等价**
        Assert.True(CanWalkCore.BooleanRewriteInverted());
        Assert.True(CanWalkCore.BooleanRewriteEquivalence());
    }

    [Fact]
    public void LocalRunFlags()
    {
        Assert.True(CanWalkCore.LocalRunFlagsOnlyForHumans());

        Assert.Equal((true, true), CanWalkCore.LocalRunFlags(0, true, true));
        Assert.Equal((false, false), CanWalkCore.LocalRunFlags(80, true, true));
    }

    [Fact]
    public void IsPlaymoster()
    {
        // **只对 `RC_PLAYMOSTER` 为真**
        Assert.True(CanWalkCore.IsPlaymosterPurpose());
        Assert.True(CanWalkCore.IsPlaymosterValue(150));
        Assert.False(CanWalkCore.IsPlaymosterValue(0));
    }

    // ===================== 六条件门 =====================

    [Fact]
    public void SixConditionGate()
    {
        Assert.True(CanWalkCore.SixConditionsAllRequired());
        Assert.True(CanWalkCore.SixConditionGate(false, true, false, false, false, false));
        Assert.False(CanWalkCore.SixConditionGate(true, true, false, false, false, false));
    }

    [Fact]
    public void Bo2B9IsOnlyPositive()
    {
        // **其它五个条件都"要求为假"**
        Assert.True(CanWalkCore.Bo2B9IsTheOnlyPositive());
    }

    [Fact]
    public void TempHideValues()
    {
        Assert.True(CanWalkCore.TempHideOnlyForHumanRaces());
        Assert.True(CanWalkCore.InvitedHorseNeedsNotMaster());
        Assert.True(CanWalkCore.TickHasPriority());

        // **非人类种族恒假**
        Assert.False(CanWalkCore.TempHideValue(80, true, true, false));
    }

    // ===================== 贯穿三段的细节 =====================

    [Fact]
    public void PetNoEntity()
    {
        Assert.True(CanWalkCore.PetNoEntitySkips());
        Assert.True(CanWalkCore.PetNoEntityOccurrenceCount());
        Assert.Equal(5, CanWalkCore.PetNoEntityOccurrences());
    }

    [Fact]
    public void CastleWarBranches()
    {
        Assert.True(CanWalkCore.CastleWarTwoBranches());
        Assert.True(CanWalkCore.CastleElseCoversBoth());
    }

    [Fact]
    public void UnusualAsmMethod()
    {
        // **用 else 里的两条空汇编避免编译器改语义**
        Assert.True(CanWalkCore.UnusualAsmMethod());
        Assert.True(CanWalkCore.UnusualAsmSnippetPresent());
        Assert.True(CanWalkCore.UnusualAsmInAllThree());
        Assert.Equal(3, CanWalkCore.UnusualAsmOccurrences());
    }

    [Fact]
    public void BoTempSemantics()
    {
        Assert.True(CanWalkCore.BoTempMeansStayBlockedValues());
        Assert.True(CanWalkCore.BoTempOnlyForPlayers());
        Assert.True(CanWalkCore.BoTempFromShopOrOffline());
    }

    [Fact]
    public void PracticeMasterGate()
    {
        // **种族 55 跳过整个"禁止穿怪"分支**
        Assert.True(CanWalkCore.PracticeMasterGateValues());
        Assert.True(CanWalkCore.PracticeMasterAlwaysBlocks());
        Assert.False(CanWalkCore.PracticeMasterGate(55));
    }

    // ===================== 安全区 / 非安全区 =====================

    [Fact]
    public void HumanSafeZoneHasExtraTerm()
    {
        Assert.True(CanWalkCore.SafeZoneHasExtraTerm());
        Assert.True(CanWalkCore.NonSafeZoneLacksTerm());
        Assert.True(CanWalkCore.ConfigFlagIncluded());

        // 其它全假时：安全区公式为真（多出的 SafeAreaLimited 项）
        Assert.True(CanWalkCore.HumanSafeZoneGate(false, false, false, false, true));
        Assert.False(CanWalkCore.HumanNonSafeZoneGate(false, false, false, false));
    }

    [Fact]
    public void NpcFormulas()
    {
        Assert.True(CanWalkCore.NpcSafeZoneFormula());
        Assert.True(CanWalkCore.NpcNonSafeZoneFormula());
    }

    [Fact]
    public void PortalAppr()
    {
        Assert.True(CanWalkCore.PortalApprRanges());
        Assert.True(CanWalkCore.PortalRangesAreClosedWithGap());

        // **闭区间且中段 59..93 是空洞**
        Assert.True(CanWalkCore.IsPortalAppr(54));
        Assert.True(CanWalkCore.IsPortalAppr(58));
        Assert.False(CanWalkCore.IsPortalAppr(59));
        Assert.False(CanWalkCore.IsPortalAppr(93));
        Assert.True(CanWalkCore.IsPortalAppr(94));
    }

    [Fact]
    public void CommentedLines()
    {
        // **三段里这两句注释逐字相同、且都用不带前缀的名字**
        Assert.True(CanWalkCore.NpcCommentedLineUsesUnprefixed());
        Assert.True(CanWalkCore.NpcCommentedLinePresent());
        Assert.True(CanWalkCore.NpcCommentedLineIdenticalInAllThree());
        Assert.True(CanWalkCore.AaaCommentedLineUsesUnprefixed());
    }

    [Fact]
    public void GuardAndMonGates()
    {
        Assert.True(CanWalkCore.GuardTwoTermGateValues());
        Assert.True(CanWalkCore.MonFourTermGateValues());
    }

    [Fact]
    public void MonVersusHumanStyle()
    {
        // **同一函数里两套不一致的"安全区"处理风格**
        Assert.True(CanWalkCore.MonVersusHumanInconsistent());
        Assert.True(CanWalkCore.StylesDiffer());
        Assert.NotEqual(CanWalkCore.HumanStyle(), CanWalkCore.MonStyle());
    }

    [Fact]
    public void GuardRaceSetValues()
    {
        // **含两个裸字面量**
        Assert.True(CanWalkCore.IsGuardRaceValues());
        Assert.True(CanWalkCore.Guard2IsRawLiteral());
        Assert.True(CanWalkCore.IsGuardRace(12));
        Assert.False(CanWalkCore.IsGuardRace(10));
    }

    // ===================== 三、CanWalk =====================

    [Fact]
    public void CanWalkIsDegenerate()
    {
        Assert.True(CanWalkCore.CanWalkIsDegenerateVersion());
        Assert.True(CanWalkCore.CanWalkKeepsOldGateForm());
        Assert.True(CanWalkCore.CanWalkSharesSixConditionGate());
        Assert.True(CanWalkCore.CanWalkIgnoresConfig());
    }

    [Fact]
    public void CanWalkGateInverse()
    {
        // **与 `CanWalkEx` 的现行门互为反义**
        Assert.True(CanWalkCore.CanWalkGateIsInverseOfExGate());
    }

    [Fact]
    public void CanWalkHumanRaces()
    {
        Assert.True(CanWalkCore.CanWalkHumanRacesValues());
        Assert.True(CanWalkCore.CanWalkHumanRaces(150));
        Assert.False(CanWalkCore.CanWalkHumanRaces(80));
    }

    [Fact]
    public void LineCounts()
    {
        Assert.True(CanWalkCore.CanWalkExMuchLonger());
        Assert.Equal(63, CanWalkCore.CanWalkLineCount());
        Assert.Equal(440, CanWalkCore.CanWalkExLineCount());
    }

    // ===================== 四、MoveToMovingObject =====================

    [Fact]
    public void SelfIgnored()
    {
        Assert.True(CanWalkCore.SelfIgnoredValues());
        Assert.True(CanWalkCore.SelfIgnored(5, 5));
        Assert.False(CanWalkCore.SelfIgnored(5, 6));
    }

    [Fact]
    public void PortalSwitchCommentedHere()
    {
        // **三处传送门判定里唯一注释掉开关的一处**
        Assert.True(CanWalkCore.PortalSwitchCommentedHere());
        Assert.True(CanWalkCore.PortalSwitchCommentedOnlyHere());
        Assert.Equal(0, CanWalkCore.PortalGateSitesWithSwitch());
    }

    [Fact]
    public void MoveTempHideThreeLayers()
    {
        Assert.True(CanWalkCore.TempHideThreeLayers());
        Assert.True(CanWalkCore.MoveTempHideThirdLayer());
        Assert.True(CanWalkCore.MoveTempHideFirstLayerPriority());
        Assert.True(CanWalkCore.MoveTempHideNeedsNotMaster());
    }

    [Fact]
    public void ChFlagSetsLocal()
    {
        Assert.True(CanWalkCore.ChFlagSetsLocalNotResult());
        Assert.True(CanWalkCore.Bo1AInitialTrue());
    }

    [Fact]
    public void DeleteThenInsert()
    {
        Assert.True(CanWalkCore.DeleteCertValues());
        Assert.True(CanWalkCore.ResultOnlyAfterInsertValues());

        Assert.True(CanWalkCore.ResultOnlyAfterInsert(true));
        Assert.False(CanWalkCore.ResultOnlyAfterInsert(false));
    }

    [Fact]
    public void MoveRemnants()
    {
        // **又一处 Break 改 Continue；第七次少括号**
        Assert.True(CanWalkCore.BreakCommentRemnantAgain());
        Assert.True(CanWalkCore.MoveBreakRemnantPresent());
        Assert.True(CanWalkCore.AddTimeMissingParens());
        Assert.True(CanWalkCore.AddTimeParensAsymmetry());
    }

    [Fact]
    public void EmptyDebugBranch()
    {
        Assert.True(CanWalkCore.EmptyDebugBranch());
        Assert.True(CanWalkCore.EmptyDebugBranchHasNoEffect());
    }

    [Fact]
    public void DeclaredButUnusedComments()
    {
        Assert.True(CanWalkCore.DeclaredButUnusedCommentsCount());
        Assert.Equal(3, CanWalkCore.DeclaredButUnusedComments.Length);
    }

    [Fact]
    public void MoveException()
    {
        // **裸方法名、无格式占位符**
        Assert.True(CanWalkCore.MoveExceptionFormat());
        Assert.True(CanWalkCore.MoveExceptionIsBareName());
        Assert.Equal("[Exception] TEnvirnoment.MoveToMovingObject", CanWalkCore.MoveExceptionMsg);
    }

    [Fact]
    public void MoveMisc()
    {
        Assert.True(CanWalkCore.InvalidMapReturnsFalseValues());
        Assert.True(CanWalkCore.MoveLineCountValues());
        Assert.True(CanWalkCore.MoveHasNoConfigFamilies());
        Assert.True(CanWalkCore.MoveHasTwoExtraRules());
        Assert.True(CanWalkCore.CommentedReverseDelete());
        Assert.True(CanWalkCore.CommentedReverseDeleteUsesDownto());
    }
}

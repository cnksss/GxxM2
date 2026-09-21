using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J244：`ObjMon.pas` 中精灵族两态（`TElfMonster` + `TElfWarriorMonster`）十三方法的 1:1 测试（326 行）。
/// **本批最有价值的发现**：
/// ① 本系列**第一个 `ErrorCode` 步进式 except**（`TElfMonster.Run`，第 2887 行）——
///    它**改写了**我此前 55 个批次都写的"本方法没有 `ErrCode` 插桩"；
///    全文件三处 `except`（2887/8058/9468）至此**全部移植**、而只有本处能**报出步号**；
/// ② 两个类是**互为变身**的两态，靠**名字尾部的 `'1'`** 区分（加 `'1'` 成战士、去 `'1'` 回本体），
///    每次变身都 `MakeClone` + 搬状态 + `KickException()` 掉旧的；
/// ③ 两个方向的**传血规则不对称** —— elf→warrior 把血钳到新形态的 `MaxHP`、warrior→elf 不钳；
/// ④ 同名的 `ResetElfMon` 有**四处**差异，含走速增量系数 **×20 对 ×30**；
/// ⑤ 一条**已被实现却仍留在源码里的 `{ TODO }`**。
/// </summary>
public sealed class ObjMonElfFamilyCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(2732, ObjMonElfFamilyCore.ElfAppearStart);
        Assert.Equal(2743, ObjMonElfFamilyCore.ElfAppearEnd);
        Assert.Equal(12, ObjMonElfFamilyCore.ElfAppearLines);
        Assert.Equal(2745, ObjMonElfFamilyCore.ElfCreateStart);
        Assert.Equal(2753, ObjMonElfFamilyCore.ElfCreateEnd);
        Assert.Equal(9, ObjMonElfFamilyCore.ElfCreateLines);
        Assert.Equal(2755, ObjMonElfFamilyCore.ElfDestroyStart);
        Assert.Equal(2758, ObjMonElfFamilyCore.ElfDestroyEnd);
        Assert.Equal(4, ObjMonElfFamilyCore.ElfDestroyLines);
        Assert.Equal(2760, ObjMonElfFamilyCore.ElfRecalcStart);
        Assert.Equal(2764, ObjMonElfFamilyCore.ElfRecalcEnd);
        Assert.Equal(5, ObjMonElfFamilyCore.ElfRecalcLines);
        Assert.Equal(2766, ObjMonElfFamilyCore.ElfResetStart);
        Assert.Equal(2804, ObjMonElfFamilyCore.ElfResetEnd);
        Assert.Equal(39, ObjMonElfFamilyCore.ElfResetLines);
        Assert.Equal(2806, ObjMonElfFamilyCore.ElfRunStart);
        Assert.Equal(2890, ObjMonElfFamilyCore.ElfRunEnd);
        Assert.Equal(85, ObjMonElfFamilyCore.ElfRunLines);

        Assert.Equal(2893, ObjMonElfFamilyCore.WarAppearStart);
        Assert.Equal(2903, ObjMonElfFamilyCore.WarAppearEnd);
        Assert.Equal(11, ObjMonElfFamilyCore.WarAppearLines);
        Assert.Equal(2904, ObjMonElfFamilyCore.WarCreateStart);
        Assert.Equal(2912, ObjMonElfFamilyCore.WarCreateEnd);
        Assert.Equal(9, ObjMonElfFamilyCore.WarCreateLines);
        Assert.Equal(2913, ObjMonElfFamilyCore.WarDestroyStart);
        Assert.Equal(2917, ObjMonElfFamilyCore.WarDestroyEnd);
        Assert.Equal(5, ObjMonElfFamilyCore.WarDestroyLines);
        Assert.Equal(2918, ObjMonElfFamilyCore.WarRecalcStart);
        Assert.Equal(2923, ObjMonElfFamilyCore.WarRecalcEnd);
        Assert.Equal(6, ObjMonElfFamilyCore.WarRecalcLines);
        Assert.Equal(2924, ObjMonElfFamilyCore.WarResetStart);
        Assert.Equal(2967, ObjMonElfFamilyCore.WarResetEnd);
        Assert.Equal(44, ObjMonElfFamilyCore.WarResetLines);
        Assert.Equal(2968, ObjMonElfFamilyCore.WarAttackStart);
        Assert.Equal(2981, ObjMonElfFamilyCore.WarAttackEnd);
        Assert.Equal(14, ObjMonElfFamilyCore.WarAttackLines);
        Assert.Equal(2982, ObjMonElfFamilyCore.WarRunStart);
        Assert.Equal(3064, ObjMonElfFamilyCore.WarRunEnd);
        Assert.Equal(83, ObjMonElfFamilyCore.WarRunLines);

        Assert.Equal(326, ObjMonElfFamilyCore.TotalLines);
        Assert.Equal(13, ObjMonElfFamilyCore.MethodCount);
        Assert.Equal(2, ObjMonElfFamilyCore.ClassCount);

        Assert.Equal(2810, ObjMonElfFamilyCore.ErrorCodeDeclLine);
        Assert.Equal(2812, ObjMonElfFamilyCore.ErrorCodeZeroLine);
        Assert.Equal(2813, ObjMonElfFamilyCore.TryLine);
        Assert.Equal(2887, ObjMonElfFamilyCore.ExceptLine);
        Assert.Equal(2888, ObjMonElfFamilyCore.LogLine);
        Assert.Equal("TElfMonster.Run Error: ", ObjMonElfFamilyCore.LogPrefix);
        Assert.Equal(30, ObjMonElfFamilyCore.FinalStep);
        Assert.Equal(2885, ObjMonElfFamilyCore.FinalStepLine);

        Assert.Equal(2861, ObjMonElfFamilyCore.ElfCloneLine);
        Assert.Equal(2870, ObjMonElfFamilyCore.ElfToWarAppearLine);
        Assert.Equal(3030, ObjMonElfFamilyCore.WarSuffixCheckLine);
        Assert.Equal(3032, ObjMonElfFamilyCore.WarStripLine);
        Assert.Equal(3033, ObjMonElfFamilyCore.WarCloneLine);
        Assert.Equal(3044, ObjMonElfFamilyCore.WarToElfAppearLine);
        Assert.Equal('1', ObjMonElfFamilyCore.SuffixChar);
        Assert.Equal(2878, ObjMonElfFamilyCore.ElfCapLine);
        Assert.Equal(3047, ObjMonElfFamilyCore.WarNoCapLine);

        Assert.Equal(2837, ObjMonElfFamilyCore.ElfFaceInitLine);
        Assert.Equal(3004, ObjMonElfFamilyCore.WarFaceInitLine);
        Assert.Equal(2847, ObjMonElfFamilyCore.ElfLeftoverCommentLine);

        Assert.Equal(2791, ObjMonElfFamilyCore.ElfElseLine);
        Assert.Equal(2949, ObjMonElfFamilyCore.WarElseLine);
        Assert.Equal(20, ObjMonElfFamilyCore.ElfWalkCoeff);
        Assert.Equal(30, ObjMonElfFamilyCore.WarWalkCoeff);
        Assert.Equal(1500, ObjMonElfFamilyCore.WarHitBase);
        Assert.Equal(100, ObjMonElfFamilyCore.WarHitCoeff);
        Assert.Equal(500, ObjMonElfFamilyCore.WalkBase);
        Assert.Equal(50, ObjMonElfFamilyCore.WalkCoeffLow);
        Assert.Equal(3, ObjMonElfFamilyCore.LevelSplit);

        Assert.Equal(2971, ObjMonElfFamilyCore.TodoImplLine);
        Assert.Equal(2972, ObjMonElfFamilyCore.SafeZoneLine);
        Assert.Equal(2974, ObjMonElfFamilyCore.SafeZoneFalseLine);
        Assert.Equal(2978, ObjMonElfFamilyCore.InheritedAttackLine);
        Assert.Equal(473, ObjMonElfFamilyCore.OldDeclLine);
        Assert.Equal(489, ObjMonElfFamilyCore.NewDeclLine);
        Assert.Equal(2897, ObjMonElfFamilyCore.WarDigUpLine);
        Assert.Equal(6, ObjMonElfFamilyCore.SharedViewRange);
        Assert.Equal(2000, ObjMonElfFamilyCore.DeathDelayMs);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonElfFamilyCore.SpanMatches());
        Assert.True(ObjMonElfFamilyCore.TotalLinesAddUp());
        Assert.True(ObjMonElfFamilyCore.ElfMethodsAscending());
        Assert.True(ObjMonElfFamilyCore.WarMethodsAscending());
        Assert.True(ObjMonElfFamilyCore.ElfBeforeWarrior());
        Assert.True(ObjMonElfFamilyCore.WithinUnit());
    }

    // ===================== 一、步进式 except =====================

    [Fact]
    public void StepTrackingExceptFacts()
    {
        Assert.True(ObjMonElfFamilyCore.StepTrackingExcept());
        Assert.True(ObjMonElfFamilyCore.ErrorCodeAssignedPerStep());
        Assert.True(ObjMonElfFamilyCore.FinalStepIsThirty());
        Assert.True(ObjMonElfFamilyCore.OnlyLocatingExcept());
        Assert.True(ObjMonElfFamilyCore.AllThreeExceptsNowPorted());
        Assert.True(ObjMonElfFamilyCore.CorrectsMyPriorClaim());
        Assert.True(ObjMonElfFamilyCore.ThreeDistinctBatches());
        Assert.True(ObjMonElfFamilyCore.LogCarriesStepNumber());
        Assert.True(ObjMonElfFamilyCore.LogVariesByStep());
        Assert.True(ObjMonElfFamilyCore.StepsAscending());

        Assert.Equal(new[] { 2887, 8058, 9468 }, ObjMonElfFamilyCore.ExceptLines);
        Assert.Equal(new[] { "J244", "J231", "J238" }, ObjMonElfFamilyCore.ExceptBatches);
        Assert.True(ObjMonElfFamilyCore.StepLines.Length >= 15);
    }

    [Fact]
    public void LogTextBoundaries()
    {
        Assert.Equal("TElfMonster.Run Error: 1", ObjMonElfFamilyCore.LogText(1));
        Assert.Equal("TElfMonster.Run Error: 30", ObjMonElfFamilyCore.LogText(30));
        Assert.NotEqual(ObjMonElfFamilyCore.LogText(1), ObjMonElfFamilyCore.LogText(30));
    }

    // ===================== 二、互为变身 =====================

    [Fact]
    public void MetamorphosisFacts()
    {
        Assert.True(ObjMonElfFamilyCore.MutualTransformation());
        Assert.True(ObjMonElfFamilyCore.SuffixOneMarksWarrior());
        Assert.True(ObjMonElfFamilyCore.AddOneOnTheWayIn());
        Assert.True(ObjMonElfFamilyCore.StripOneOnTheWayOut());
        Assert.True(ObjMonElfFamilyCore.CloneThenSuicide());
        Assert.True(ObjMonElfFamilyCore.FirstMetamorphosis());
        Assert.True(ObjMonElfFamilyCore.BothHandOver());

        Assert.Equal(new[] { 2871, 3049 }, ObjMonElfFamilyCore.MasterNilLines);
        Assert.Equal(new[] { 2881, 3050 }, ObjMonElfFamilyCore.KickLines);
    }

    [Fact]
    public void NameTransformBoundaries()
    {
        Assert.Equal("sDogz1", ObjMonElfFamilyCore.ToWarriorName("sDogz"));
        Assert.Equal("sDogz", ObjMonElfFamilyCore.ToElfName("sDogz1"));
        Assert.True(ObjMonElfFamilyCore.RoundTripName());
        Assert.True(ObjMonElfFamilyCore.NoSuffixUnchanged());
        Assert.True(ObjMonElfFamilyCore.SuffixStripped());
    }

    [Fact]
    public void TransferAsymmetryFacts()
    {
        Assert.True(ObjMonElfFamilyCore.TransferCapsOnOneSideOnly());
        Assert.True(ObjMonElfFamilyCore.ElfToWarriorCaps());
        Assert.True(ObjMonElfFamilyCore.WarriorToElfDoesNot());
        Assert.True(ObjMonElfFamilyCore.AsymmetricMirror());
        Assert.True(ObjMonElfFamilyCore.CapRespected());
        Assert.True(ObjMonElfFamilyCore.NoCapCanExceed());
        Assert.True(ObjMonElfFamilyCore.LowerSourceKeeps());
        Assert.True(ObjMonElfFamilyCore.AgreeWhenSourceLower());
    }

    [Fact]
    public void TransferBoundaries()
    {
        Assert.Equal(100, ObjMonElfFamilyCore.TransferHp(9999, 1, 100, true));
        Assert.Equal(9999, ObjMonElfFamilyCore.TransferHp(9999, 1, 100, false));
        Assert.Equal(100, ObjMonElfFamilyCore.TransferHp(1, 100, 200, true));
        Assert.Equal(100, ObjMonElfFamilyCore.TransferHp(1, 100, 200, false));
        Assert.Equal(50, ObjMonElfFamilyCore.TransferHp(50, 10, 100, true));
    }

    [Fact]
    public void PolarityFacts()
    {
        Assert.True(ObjMonElfFamilyCore.OppositeDefaultPolarity());
        Assert.True(ObjMonElfFamilyCore.ThreeClausesSetTrue());
        Assert.True(ObjMonElfFamilyCore.TwoClausesSetFalse());
        Assert.True(ObjMonElfFamilyCore.TwoKindsOfLeftoverComment());
        Assert.True(ObjMonElfFamilyCore.ElfLeftoverIsOneLine());
        Assert.True(ObjMonElfFamilyCore.WarLeftoverIsABlock());
        Assert.True(ObjMonElfFamilyCore.WarBlockIsSecondSearch());

        Assert.Equal(new[] { 2849, 2851, 2853 }, ObjMonElfFamilyCore.ElfFaceTrueLines);
        Assert.Equal(new[] { 3018, 3020 }, ObjMonElfFamilyCore.WarFaceFalseLines);
        Assert.Equal(new[] { 3011, 3017 }, ObjMonElfFamilyCore.WarLeftoverBlock);
    }

    // ===================== 三、ResetElfMon 的四处差异 =====================

    [Fact]
    public void ResetDifferenceFacts()
    {
        Assert.True(ObjMonElfFamilyCore.FourDifferences());
        Assert.True(ObjMonElfFamilyCore.CoefficientTwentyVersusThirty());
        Assert.True(ObjMonElfFamilyCore.ElfSkipsNextHitTime());
        Assert.True(ObjMonElfFamilyCore.WarriorSetsItTwice());
        Assert.True(ObjMonElfFamilyCore.NewAttrBranchIdentical());
        Assert.True(ObjMonElfFamilyCore.Int64Intermediate());
        Assert.True(ObjMonElfFamilyCore.HighCardinalClamp());
        Assert.True(ObjMonElfFamilyCore.MaxTenAndMaxHundred());
        Assert.True(ObjMonElfFamilyCore.RecalcIdentical());

        Assert.Equal(new[] { 2953, 2960 }, ObjMonElfFamilyCore.WarHitTimeLines);
    }

    [Fact]
    public void WalkCoefficientBoundaries()
    {
        Assert.True(ObjMonElfFamilyCore.SameAtLowLevels());
        Assert.True(ObjMonElfFamilyCore.DifferAtHighLevels());

        Assert.Equal(450, ObjMonElfFamilyCore.ElfWalk(1));
        Assert.Equal(350, ObjMonElfFamilyCore.ElfWalk(3));
        Assert.Equal(330, ObjMonElfFamilyCore.ElfWalk(4));
        Assert.Equal(450, ObjMonElfFamilyCore.WarWalk(1));
        Assert.Equal(350, ObjMonElfFamilyCore.WarWalk(3));
        Assert.Equal(320, ObjMonElfFamilyCore.WarWalk(4));
    }

    [Fact]
    public void HitTimeBoundaries()
    {
        Assert.True(ObjMonElfFamilyCore.WarHitAtThree());
        Assert.True(ObjMonElfFamilyCore.WarHitAtFour());

        Assert.Equal(1400, ObjMonElfFamilyCore.WarHitTime(1));
        Assert.Equal(1200, ObjMonElfFamilyCore.WarHitTime(3));
        Assert.Equal(1100, ObjMonElfFamilyCore.WarHitTime(4));
    }

    // ===================== 四、被实现的 TODO =====================

    [Fact]
    public void TodoFacts()
    {
        Assert.True(ObjMonElfFamilyCore.TodoAlreadyDone());
        Assert.True(ObjMonElfFamilyCore.CommentKeptAnyway());
        Assert.True(ObjMonElfFamilyCore.ThirdSiteOfTheSameTodo());
        Assert.True(ObjMonElfFamilyCore.IdeTaskListResidue());
        Assert.True(ObjMonElfFamilyCore.NotAContradictionButAStaleMarker());

        Assert.Equal(new[] { 485, 501, 2970 }, ObjMonElfFamilyCore.TodoLines);
    }

    [Fact]
    public void SafeZoneBoundaries()
    {
        Assert.True(ObjMonElfFamilyCore.AllFourStops());
        Assert.True(ObjMonElfFamilyCore.NotInSafeZoneFights());
        Assert.True(ObjMonElfFamilyCore.NonPlayerTargetFights());
        Assert.True(ObjMonElfFamilyCore.NoMasterFights());

        Assert.True(ObjMonElfFamilyCore.ShouldStopAttacking(true, true, true, true));
        Assert.False(ObjMonElfFamilyCore.ShouldStopAttacking(true, true, true, false));
        Assert.False(ObjMonElfFamilyCore.ShouldStopAttacking(true, false, true, true));
        Assert.False(ObjMonElfFamilyCore.ShouldStopAttacking(false, true, true, true));
    }

    [Fact]
    public void InheritanceChainFacts()
    {
        Assert.True(ObjMonElfFamilyCore.NoNilGuardBeforeDeref());
        Assert.True(ObjMonElfFamilyCore.DerefsTargetCret());
        Assert.True(ObjMonElfFamilyCore.OverridesSpitSpiderAttackTarget());
        Assert.True(ObjMonElfFamilyCore.OldBaseWasTATMonster());
        Assert.True(ObjMonElfFamilyCore.LinkLandsOnBothEnds());
    }

    // ===================== 五、其余 =====================

    [Fact]
    public void AppearAndCreateFacts()
    {
        Assert.True(ObjMonElfFamilyCore.AppearNowFormsDiffer());
        Assert.True(ObjMonElfFamilyCore.ThreeCommentsOnElfSide());
        Assert.True(ObjMonElfFamilyCore.OnlyWarriorSendsDigUp());
        Assert.True(ObjMonElfFamilyCore.EachSetsOneExtraField());
        Assert.True(ObjMonElfFamilyCore.ThreeSharedSettings());

        Assert.Equal(new[] { 2736, 2737, 2738 },
            ObjMonElfFamilyCore.ElfAppearComments);
    }

    [Fact]
    public void SharedBehaviourFacts()
    {
        Assert.True(ObjMonElfFamilyCore.SameSearchThrottle());
        Assert.True(ObjMonElfFamilyCore.SameDeathHandling());
        Assert.True(ObjMonElfFamilyCore.TwoSecondsThenGhost());
        Assert.True(ObjMonElfFamilyCore.WarriorRunUnprotected());
        Assert.True(ObjMonElfFamilyCore.OnlyElfSideHasIt());
        Assert.True(ObjMonElfFamilyCore.OnlyOneInstrumentedMethod());
        Assert.True(ObjMonElfFamilyCore.NowPorted());
        Assert.True(ObjMonElfFamilyCore.NoOtherInstrumentation());
    }

    [Fact]
    public void DestroyFacts()
    {
        Assert.True(ObjMonElfFamilyCore.TwoPureShellDestroys());
        Assert.True(ObjMonElfFamilyCore.TwentyNineTotal());
    }

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonElfFamilyCore.ElfFamilyClosed());
        Assert.True(ObjMonElfFamilyCore.TwoRowsToFlip());
        Assert.True(ObjMonElfFamilyCore.CompletesTheJ239Thread());
        Assert.True(ObjMonElfFamilyCore.DeclLinesChecked());

        Assert.Equal(new[] { 458, 489 }, ObjMonElfFamilyCore.DeclLines);
    }
}

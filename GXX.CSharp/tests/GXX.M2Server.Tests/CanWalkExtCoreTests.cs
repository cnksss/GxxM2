using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J154：`CanWalkOfItem`（63 行）、`CanWalkEx2`（288 行）、`CanWalkEx3`（116 行）1:1 测试。
/// **`wf_*` 序号、镜像六差异、`case` 四分派与物品门安全性均经探针实测。**
/// </summary>
public sealed class CanWalkExtCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(CanWalkExtCore.ConstantsMatchSource());
        Assert.True(CanWalkExtCore.WalkFlagOrdinals());
        Assert.True(CanWalkExtCore.WalkFlagNamesCount());
    }

    [Fact]
    public void WalkFlagValues()
    {
        // **`TWalkFlag` 六项序号枚举**
        Assert.Equal(0, CanWalkExtCore.WfHum);
        Assert.Equal(1, CanWalkExtCore.WfMon);
        Assert.Equal(2, CanWalkExtCore.WfNpc);
        Assert.Equal(3, CanWalkExtCore.WfGuard);
        Assert.Equal(4, CanWalkExtCore.WfWar);
        Assert.Equal(5, CanWalkExtCore.WfObstacle);
        Assert.Equal(6, CanWalkExtCore.WalkFlagCount);
    }

    [Fact]
    public void ObjAndRaceValues()
    {
        Assert.Equal(1, CanWalkExtCore.ObjActor);
        Assert.Equal(2, CanWalkExtCore.ObjItem);
        Assert.Equal(112, CanWalkExtCore.RcArcherGuard);
        Assert.Equal(150, CanWalkExtCore.RcPlayMaster);
    }

    // ===================== 一、镜像对照 =====================

    [Fact]
    public void MirrorStructure()
    {
        Assert.True(CanWalkExtCore.MirrorPairs());
        Assert.True(CanWalkExtCore.SixSystematicDifferences());
        Assert.Equal(6, CanWalkExtCore.SystematicDifferences.Length);
    }

    [Fact]
    public void CellGatePlacement()
    {
        Assert.True(CanWalkExtCore.ExHasUnifiedCellGate());
        Assert.True(CanWalkExtCore.Ex2HasPerBranchCellGate());
        Assert.True(CanWalkExtCore.CellGatePlacementDiffers());
        Assert.NotEqual(CanWalkExtCore.CellGatePlacement("Ex"), CanWalkExtCore.CellGatePlacement("Ex2"));
    }

    [Fact]
    public void GateForms()
    {
        // **新旧两种写法互为德摩根两侧、完全等价**
        Assert.True(CanWalkExtCore.ExUsesNewGateForm());
        Assert.True(CanWalkExtCore.Ex2UsesOldGateForm());
        Assert.True(CanWalkExtCore.GateFormsEquivalent());
    }

    [Fact]
    public void ResultPlacement()
    {
        Assert.True(CanWalkExtCore.ResultPlacementDiffers());
        Assert.NotEqual(CanWalkExtCore.ResultTruePlacement("Ex"), CanWalkExtCore.ResultTruePlacement("Ex2"));
    }

    [Fact]
    public void RunFlagAndSafeZoneAndPlaymoster()
    {
        Assert.True(CanWalkExtCore.ExHasLocalRunFlags());
        Assert.True(CanWalkExtCore.Ex2ReadsTargetRunFlag());
        Assert.True(CanWalkExtCore.ExHasSafeZoneLogic());
        Assert.True(CanWalkExtCore.Ex2HasNoSafeZoneLogic());
        Assert.True(CanWalkExtCore.ExHasPlaymosterGate());
        Assert.True(CanWalkExtCore.Ex2LacksPlaymosterGate());
    }

    [Fact]
    public void NilCheckDiffers()
    {
        // **`CanWalkEx2` 判空、`CanWalkEx` 不判（传空指针会崩）**
        Assert.True(CanWalkExtCore.Ex2ChecksNilWalkObject());
        Assert.True(CanWalkExtCore.ExLacksNilCheck());
        Assert.True(CanWalkExtCore.Ex2NilGoesToElse());
    }

    [Fact]
    public void DispatchBranches()
    {
        Assert.True(CanWalkExtCore.DispatchBranchValues());
        Assert.True(CanWalkExtCore.HeroBeatsDummy());

        Assert.Equal(0, CanWalkExtCore.DispatchBranch(false, 1, false));
        Assert.Equal(1, CanWalkExtCore.DispatchBranch(false, 0, true));
        Assert.Equal(2, CanWalkExtCore.DispatchBranch(false, 0, false));
    }

    // ===================== CanWalkEx2 内部不一致 =====================

    [Fact]
    public void PracticeMasterNesting()
    {
        // **英雄段与假人段结构不同，假人段与其余段相同**
        Assert.True(CanWalkExtCore.Ex2PracticeMasterNestingDiffers());
        Assert.True(CanWalkExtCore.Ex2DummyMatchesElseSegment());
    }

    [Fact]
    public void CastleContinueStyle()
    {
        // **英雄/假人段带块、其余段单行**
        Assert.True(CanWalkExtCore.Ex2CastleContinueStyleDiffers());
        Assert.True(CanWalkExtCore.CastleContinueUsesBlock("Ex2", 0));
        Assert.False(CanWalkExtCore.CastleContinueUsesBlock("Ex2", 2));
    }

    // ===================== 跑动标志来源 =====================

    [Fact]
    public void RunFlagSources()
    {
        Assert.True(CanWalkExtCore.ExThreeTermsAreConfigMapObject());
        Assert.True(CanWalkExtCore.SameFieldNameDifferentObject());
        Assert.True(CanWalkExtCore.Ex2ThirdTermReadsTarget());
        Assert.Equal(3, CanWalkExtCore.ExHumanTerms.Length);
        Assert.Equal(3, CanWalkExtCore.Ex2HumanTerms.Length);
    }

    [Fact]
    public void MonTermCounts()
    {
        // **`CanWalkEx` 四项、`CanWalkEx2` 两项**
        Assert.True(CanWalkExtCore.ExMonFourTerms());
        Assert.True(CanWalkExtCore.Ex2MonTwoTerms());
        Assert.True(CanWalkExtCore.Ex2MonLacksObjectFlagAndSafeZone());
        Assert.Equal(4, CanWalkExtCore.ExMonTermCount());
        Assert.Equal(2, CanWalkExtCore.Ex2MonTermCount());
    }

    // ===================== 三、CanWalkEx3 =====================

    [Fact]
    public void Ex3Signature()
    {
        // **签名不同：没有 `WalkObject`、没有 `boFlag`**
        Assert.True(CanWalkExtCore.Ex3SignatureDiffers());
        Assert.True(CanWalkExtCore.Ex3SignatureDiffersValues());
        Assert.DoesNotContain("WalkObject", CanWalkExtCore.Ex3Parameters);
        Assert.Contains("Flag", CanWalkExtCore.Ex3Parameters);
    }

    [Fact]
    public void Ex3ObstacleRelaxation()
    {
        // **带 `wf_Obstacle` 时可穿障碍物**
        Assert.True(CanWalkExtCore.Ex3ObstacleRelaxation());
        Assert.True(CanWalkExtCore.OtherVersionsLackObstacleRelaxation());

        Assert.True(CanWalkExtCore.Ex3ObstacleGate(true, 1, true));
        Assert.False(CanWalkExtCore.Ex3ObstacleGate(true, 1, false));
    }

    [Fact]
    public void Ex3WarGate()
    {
        Assert.True(CanWalkExtCore.Ex3WarFlagInsteadOfConfig());
        Assert.True(CanWalkExtCore.Ex3CastleLazyNil());
        Assert.True(CanWalkExtCore.Ex3CastleLazyNilValues());
        Assert.True(CanWalkExtCore.Ex3WarGateInverted());
        Assert.True(CanWalkExtCore.Ex3WarCommentPresent());
        Assert.True(CanWalkExtCore.WarCommentsDifferPerVersion());
    }

    [Fact]
    public void Ex3CaseArms()
    {
        // **只有四分派，且 `else` 落到"怪"**
        Assert.True(CanWalkExtCore.Ex3CaseFourArms());
        Assert.True(CanWalkExtCore.Ex3LacksPlaymasterArm());
        Assert.True(CanWalkExtCore.Ex3LacksLiteral12());
        Assert.True(CanWalkExtCore.Ex3LacksMoveArcherGuard());
        Assert.True(CanWalkExtCore.Ex3LacksPracticeMasterGate());

        Assert.Equal(CanWalkExtCore.WfMon, CanWalkExtCore.Ex3RaceArm(150));
        Assert.Equal(CanWalkExtCore.WfMon, CanWalkExtCore.Ex3RaceArm(12));
        Assert.Equal(CanWalkExtCore.WfMon, CanWalkExtCore.Ex3RaceArm(142));
        Assert.Equal(CanWalkExtCore.WfMon, CanWalkExtCore.Ex3RaceArm(55));
    }

    [Fact]
    public void Ex3SkipLogic()
    {
        Assert.True(CanWalkExtCore.Ex3SkipLogic());
        Assert.True(CanWalkExtCore.Ex3SkipIfFlagPresent(true));
        Assert.False(CanWalkExtCore.Ex3SkipIfFlagPresent(false));
    }

    [Fact]
    public void Ex3TempHide()
    {
        // **时间戳项被注释、只剩一项**
        Assert.True(CanWalkExtCore.Ex3TempHideTickCommented());
        Assert.True(CanWalkExtCore.Ex3TempHideOneTermValues());
        Assert.True(CanWalkExtCore.TempHideVersus());
        Assert.True(CanWalkExtCore.TempHideTwoTermsElsewhere());
        Assert.True(CanWalkExtCore.TempHideOneTermInEx3());

        // 时间戳为正时：其它版本为真、Ex3 为假
        Assert.True(CanWalkExtCore.Ex3TempHideTwoTerms(true, false, false));
        Assert.False(CanWalkExtCore.Ex3TempHideOneTerm(false, false));
    }

    [Fact]
    public void Ex3Comments()
    {
        Assert.True(CanWalkExtCore.Ex3MapApoiseCommented());
        Assert.True(CanWalkExtCore.MapApoiseCommentPresent());
        Assert.True(CanWalkExtCore.Ex3NestedBaseObjectNilCheck());
        Assert.True(CanWalkExtCore.OthersUseAndChain());
        Assert.True(CanWalkExtCore.NestedVersusAndChain());
    }

    [Fact]
    public void Ex3LacksPortalRanges()
    {
        Assert.True(CanWalkExtCore.Ex3LacksPortalRanges());
        Assert.True(CanWalkExtCore.Ex2UsesPortalRanges());
    }

    // ===================== 四、CanWalkOfItem =====================

    [Fact]
    public void OfItemDefaultTrue()
    {
        // **唯一以真开头、且取格失败也返回真**
        Assert.True(CanWalkExtCore.OfItemDefaultsTrue());
        Assert.True(CanWalkExtCore.OfItemCellFailureReturnsTrue());
        Assert.True(CanWalkExtCore.OfItemResultOnCellFailure());
        Assert.True(CanWalkExtCore.OtherVersionsCellFailureReturnsFalse());
    }

    [Fact]
    public void OfItemGates()
    {
        Assert.True(CanWalkExtCore.OfItemTwoGates());
        Assert.Equal(2, CanWalkExtCore.OfItemGates.Length);
        Assert.True(CanWalkExtCore.OfItemGatesIndependent());
    }

    [Fact]
    public void OfItemNilCheckAsymmetry()
    {
        // **扮演者门有判空、物品门没有**
        Assert.True(CanWalkExtCore.OfItemActorGateHasNilCheck());
        Assert.True(CanWalkExtCore.OfItemItemGateLacksNilCheck());
        Assert.True(CanWalkExtCore.OfItemNullItemCrashesValues());
        Assert.True(CanWalkExtCore.OfItemActorGateSafeOnNull());
    }

    [Fact]
    public void OfItemGateFlags()
    {
        Assert.True(CanWalkExtCore.OfItemItemGateDisabledByFlag());
        Assert.True(CanWalkExtCore.OfItemActorGateDisabledByFlag());
    }

    [Fact]
    public void OfItemSharesSixCondition()
    {
        Assert.True(CanWalkExtCore.OfItemSharesSixConditionGate());
        Assert.True(CanWalkExtCore.SixConditionGateValues());
        Assert.True(CanWalkExtCore.OfItemNoConfigFamilies());
        Assert.True(CanWalkExtCore.OfItemClosestToCanWalk());
    }

    [Fact]
    public void OfItemRedundantComment()
    {
        Assert.True(CanWalkExtCore.OfItemRedundantResultComment());
        Assert.True(CanWalkExtCore.RedundantResultCommentPresent());
        Assert.Equal("// Result:=True;", CanWalkExtCore.RedundantResultComment);
    }

    // ===================== 版本对照 =====================

    [Fact]
    public void FourVersions()
    {
        Assert.True(CanWalkExtCore.FourVersionsExist());
        Assert.True(CanWalkExtCore.ExIsLongest());
        Assert.True(CanWalkExtCore.Ex2IsSecondLongest());
        Assert.True(CanWalkExtCore.FourVersionsTotalValues());
        Assert.True(CanWalkExtCore.FourVersionsSamePurpose());
    }

    [Fact]
    public void LineCounts()
    {
        Assert.Equal(new[] { 63, 440, 288, 116 }, CanWalkExtCore.FourVersionsLineCounts);
        Assert.Equal(907, CanWalkExtCore.FourVersionsTotal());
    }
}

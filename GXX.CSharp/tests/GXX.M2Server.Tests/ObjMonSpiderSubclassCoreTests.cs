using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J202：`ObjMon.pas` 中 `TSpitSpider.AttackTarget` 与两个子类
/// `THighRiskSpider` / `TBigPoisionSpider` 的 1:1 测试（五方法合计 49 行）。
/// **本批最有价值的发现**：两个子类都把父类刚设成 `True` 的
/// `m_boAnimal` 改回 `False` —— "毒蜘蛛家族里只有父类自己是动物"；
/// 两个子类的唯一实质差别是 `m_boUsePoison`。
/// </summary>
public sealed class ObjMonSpiderSubclassCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1653, ObjMonSpiderSubclassCore.AttackStart);
        Assert.Equal(1681, ObjMonSpiderSubclassCore.AttackEnd);
        Assert.Equal(29, ObjMonSpiderSubclassCore.AttackLines);
        Assert.Equal(1683, ObjMonSpiderSubclassCore.HighRiskCommentLine);
        Assert.Equal(1684, ObjMonSpiderSubclassCore.HighRiskCreateStart);
        Assert.Equal(6, ObjMonSpiderSubclassCore.HighRiskCreateLines);
        Assert.Equal(1691, ObjMonSpiderSubclassCore.HighRiskDestroyStart);
        Assert.Equal(4, ObjMonSpiderSubclassCore.HighRiskDestroyLines);
        Assert.Equal(1696, ObjMonSpiderSubclassCore.BigPoisonCommentLine);
        Assert.Equal(1697, ObjMonSpiderSubclassCore.BigPoisonCreateStart);
        Assert.Equal(6, ObjMonSpiderSubclassCore.BigPoisonCreateLines);
        Assert.Equal(1704, ObjMonSpiderSubclassCore.BigPoisonDestroyStart);
        Assert.Equal(4, ObjMonSpiderSubclassCore.BigPoisonDestroyLines);
        Assert.Equal(49, ObjMonSpiderSubclassCore.TotalLines);
        Assert.Equal(1526, ObjMonSpiderSubclassCore.ParentCreateStart);
        Assert.Equal(6, ObjMonSpiderSubclassCore.UsePoisonSites);
        Assert.Equal(4, ObjMonSpiderSubclassCore.UsePoisonAssignCount);
        Assert.Equal(1619, ObjMonSpiderSubclassCore.UsePoisonReadLine);
        Assert.Equal(1, ObjMonSpiderSubclassCore.AdjacentRadius);
        Assert.Equal(2, ObjMonSpiderSubclassCore.RangeRadius);
        Assert.Equal(2, ObjMonSpiderSubclassCore.CenterIndex);
        Assert.Equal(7, ObjMonSpiderSubclassCore.ClassesCovered);
        Assert.Equal(47, ObjMonSpiderSubclassCore.RemainingClasses);
        Assert.Equal(5, ObjMonSpiderSubclassCore.SpiderFamilySize);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonSpiderSubclassCore.SpanMatches());
        Assert.True(ObjMonSpiderSubclassCore.TotalLinesAddUp());
        Assert.True(ObjMonSpiderSubclassCore.StartsAscending());
        Assert.True(ObjMonSpiderSubclassCore.CommentsPrecedeBlocks());
        Assert.True(ObjMonSpiderSubclassCore.ParentBeforeSubclasses());
        Assert.True(ObjMonSpiderSubclassCore.WithinUnit());
        Assert.True(ObjMonSpiderSubclassCore.NoInstrumentation());
    }

    // ===================== 一、子类反转父类标志 =====================

    [Fact]
    public void SubclassesFlipParentFlags()
    {
        Assert.True(ObjMonSpiderSubclassCore.BothSubclassesInheritParent());
        Assert.True(ObjMonSpiderSubclassCore.BothFlipAnimalToFalse());
        Assert.True(ObjMonSpiderSubclassCore.ParentSetsAnimalTrue());
        Assert.True(ObjMonSpiderSubclassCore.OnlyParentIsAnimal());
        Assert.True(ObjMonSpiderSubclassCore.OnlyDifferenceIsPoison());
        Assert.True(ObjMonSpiderSubclassCore.HighRiskDisablesPoison());
        Assert.True(ObjMonSpiderSubclassCore.BigPoisonKeepsPoison());
        Assert.True(ObjMonSpiderSubclassCore.ClassNameMisspelled());
        Assert.True(ObjMonSpiderSubclassCore.CorrectSpellingIsPoison());
        Assert.True(ObjMonSpiderSubclassCore.FamilyConfigExtracted());
        Assert.True(ObjMonSpiderSubclassCore.TableHasOneTrueAnimal());
        Assert.True(ObjMonSpiderSubclassCore.TableHasTwoTruePoison());
        Assert.True(ObjMonSpiderSubclassCore.SubclassesAgreeOnAnimal());
        Assert.True(ObjMonSpiderSubclassCore.SubclassesDifferOnPoison());
        Assert.True(ObjMonSpiderSubclassCore.DifferenceIsPoisonOnly());
        Assert.True(ObjMonSpiderSubclassCore.ParentWriteOverwrittenImmediately());
        Assert.True(ObjMonSpiderSubclassCore.IneffectiveWrite());
        Assert.True(ObjMonSpiderSubclassCore.SearchTimeStillUnread());
        Assert.True(ObjMonSpiderSubclassCore.NoRunOverrideInSubclasses());
    }

    [Fact]
    public void FamilyConfigTable()
    {
        Assert.Equal(3, ObjMonSpiderSubclassCore.FamilyConfig.Length);

        // **父类：动物 + 施毒**
        Assert.Equal("TSpitSpider", ObjMonSpiderSubclassCore.FamilyConfig[0].ClassName);
        Assert.True(ObjMonSpiderSubclassCore.FamilyConfig[0].Animal);
        Assert.True(ObjMonSpiderSubclassCore.FamilyConfig[0].UsePoison);

        // **高危蜘蛛：都不是**
        Assert.Equal("THighRiskSpider", ObjMonSpiderSubclassCore.FamilyConfig[1].ClassName);
        Assert.False(ObjMonSpiderSubclassCore.FamilyConfig[1].Animal);
        Assert.False(ObjMonSpiderSubclassCore.FamilyConfig[1].UsePoison);

        // **大毒蜘蛛：不是动物但施毒**
        Assert.Equal("TBigPoisionSpider", ObjMonSpiderSubclassCore.FamilyConfig[2].ClassName);
        Assert.False(ObjMonSpiderSubclassCore.FamilyConfig[2].Animal);
        Assert.True(ObjMonSpiderSubclassCore.FamilyConfig[2].UsePoison);

        Assert.Equal(2, ObjMonSpiderSubclassCore.SubclassDecls.Length);
    }

    [Fact]
    public void PoisonSiteCensus()
    {
        Assert.True(ObjMonSpiderSubclassCore.PoisonReadOnlyAt1619());
        Assert.True(ObjMonSpiderSubclassCore.SixSitesInFile());
        Assert.True(ObjMonSpiderSubclassCore.FourAssignsPlusDecl());
        Assert.True(ObjMonSpiderSubclassCore.SitesExtracted());
        Assert.True(ObjMonSpiderSubclassCore.ExactlyOneRead());

        Assert.Equal(6, ObjMonSpiderSubclassCore.UsePoisonSites1.Length);
        Assert.Equal("decl@306", ObjMonSpiderSubclassCore.UsePoisonSites1[0]);
        Assert.Equal("read@1619", ObjMonSpiderSubclassCore.UsePoisonSites1[2]);
        Assert.Equal("assign@2910", ObjMonSpiderSubclassCore.UsePoisonSites1[5]);
    }

    // ===================== 二、AttackTarget 三段 =====================

    [Fact]
    public void AttackTargetFacts()
    {
        Assert.True(ObjMonSpiderSubclassCore.ThreeSegments());
        Assert.True(ObjMonSpiderSubclassCore.TrueEvenWhenOnCooldown());
        Assert.True(ObjMonSpiderSubclassCore.CallerCannotDistinguish());
        Assert.True(ObjMonSpiderSubclassCore.EarlyReturnSkipsApproach());
        Assert.True(ObjMonSpiderSubclassCore.InRangeStaysPut());
        Assert.True(ObjMonSpiderSubclassCore.SameMapApproaches());
        Assert.True(ObjMonSpiderSubclassCore.OtherMapDiscards());
        Assert.True(ObjMonSpiderSubclassCore.NoDeathCheck());
        Assert.True(ObjMonSpiderSubclassCore.UsesTickDiff());
        Assert.True(ObjMonSpiderSubclassCore.ConsistentWithJ199());
        Assert.True(ObjMonSpiderSubclassCore.OppositeToJ200());
        Assert.True(ObjMonSpiderSubclassCore.StrictGreater());
        Assert.True(ObjMonSpiderSubclassCore.ExactlyEqualBlocks());
        Assert.True(ObjMonSpiderSubclassCore.ThreeFieldsRefreshed());
        Assert.True(ObjMonSpiderSubclassCore.TickReadTwice());
        Assert.True(ObjMonSpiderSubclassCore.SameAsJ200Pattern());
        Assert.True(ObjMonSpiderSubclassCore.DelayClearedOnExecute());
        Assert.True(ObjMonSpiderSubclassCore.SameAsJ199WalkDelay());
        Assert.True(ObjMonSpiderSubclassCore.BreakHolySeizeAfterAttack());
        Assert.True(ObjMonSpiderSubclassCore.OrderNotCommutative());
        Assert.True(ObjMonSpiderSubclassCore.CalledWithParens());
        Assert.True(ObjMonSpiderSubclassCore.OutParamDir());
        Assert.True(ObjMonSpiderSubclassCore.DirValidOnlyWhenTrue());
        Assert.True(ObjMonSpiderSubclassCore.DirUninitialized());
        Assert.True(ObjMonSpiderSubclassCore.SafeOnlyByControlFlow());
    }

    [Fact]
    public void CooldownBoundaries()
    {
        // **严格大于：恰好等于间隔时不吐**
        Assert.True(ObjMonSpiderSubclassCore.ExactlyEqualDoesNotSpit());
        Assert.False(ObjMonSpiderSubclassCore.CanSpit(0, 500, 500, 0));

        // **超过一毫秒就吐**
        Assert.True(ObjMonSpiderSubclassCore.OnePastSpits());
        Assert.True(ObjMonSpiderSubclassCore.CanSpit(0, 501, 500, 0));

        // **未到不吐**
        Assert.True(ObjMonSpiderSubclassCore.NotYetDoesNotSpit());
        Assert.False(ObjMonSpiderSubclassCore.CanSpit(0, 499, 500, 0));

        // **附加延迟会推迟**
        Assert.True(ObjMonSpiderSubclassCore.DelayPostpones());
        Assert.True(ObjMonSpiderSubclassCore.CanSpit(0, 500, 400, 0));
        Assert.False(ObjMonSpiderSubclassCore.CanSpit(0, 500, 400, 200));
    }

    [Fact]
    public void ThreeWayDecision()
    {
        Assert.True(ObjMonSpiderSubclassCore.NilTargetReturnsFalse());
        Assert.True(ObjMonSpiderSubclassCore.InRangeCooledSpits());
        Assert.True(ObjMonSpiderSubclassCore.InRangeNotCooledWaits());
        Assert.True(ObjMonSpiderSubclassCore.NotInRangeSameMapApproaches());
        Assert.True(ObjMonSpiderSubclassCore.NotInRangeOtherMapDiscards());

        Assert.Equal("false-exit",
            ObjMonSpiderSubclassCore.Decide(true, false, false, false));
        Assert.Equal("spit",
            ObjMonSpiderSubclassCore.Decide(false, true, true, true));
        Assert.Equal("wait",
            ObjMonSpiderSubclassCore.Decide(false, true, false, true));
        Assert.Equal("approach",
            ObjMonSpiderSubclassCore.Decide(false, false, false, true));
        Assert.Equal("discard",
            ObjMonSpiderSubclassCore.Decide(false, false, false, false));
    }

    // ===================== 三、TargetInSpitRange 两条路径 =====================

    [Fact]
    public void TwoHitPaths()
    {
        Assert.True(ObjMonSpiderSubclassCore.TwoHitPaths());
        Assert.True(ObjMonSpiderSubclassCore.AdjacentAlwaysHits());
        Assert.True(ObjMonSpiderSubclassCore.TableOnlyForOuterRing());
        Assert.True(ObjMonSpiderSubclassCore.CenterCellUnneeded());
        Assert.True(ObjMonSpiderSubclassCore.TwoDirectionFunctions());
        Assert.True(ObjMonSpiderSubclassCore.Path1UsesGetAttackDir());
        Assert.True(ObjMonSpiderSubclassCore.Path2UsesGetNextDirection());
        Assert.True(ObjMonSpiderSubclassCore.Path1ExitsEarly());
        Assert.True(ObjMonSpiderSubclassCore.Path2FallsThrough());

        Assert.Equal(2, ObjMonSpiderSubclassCore.HitPaths.Length);
    }

    [Fact]
    public void RangeClassificationBoundaries()
    {
        // **贴身（一格内）落在路径①**
        Assert.True(ObjMonSpiderSubclassCore.CenterIsAdjacent());
        Assert.True(ObjMonSpiderSubclassCore.OneOneIsAdjacent());
        Assert.Equal("adjacent", ObjMonSpiderSubclassCore.ClassifyRange(0, 0));
        Assert.Equal("adjacent", ObjMonSpiderSubclassCore.ClassifyRange(1, 1));

        // **两格落在路径②（查表）**
        Assert.True(ObjMonSpiderSubclassCore.TwoIsOuter());
        Assert.True(ObjMonSpiderSubclassCore.TwoTwoIsOuter());
        Assert.Equal("outer", ObjMonSpiderSubclassCore.ClassifyRange(2, 0));
        Assert.Equal("outer", ObjMonSpiderSubclassCore.ClassifyRange(2, 2));

        // **三格超出范围**
        Assert.True(ObjMonSpiderSubclassCore.ThreeIsOut());
        Assert.True(ObjMonSpiderSubclassCore.OutOfRangeTakesPriority());
        Assert.Equal("out-of-range", ObjMonSpiderSubclassCore.ClassifyRange(3, 0));

        // **单轴超出即超出**
        Assert.Equal("out-of-range", ObjMonSpiderSubclassCore.ClassifyRange(0, 3));
    }

    [Fact]
    public void CellIndexMapping()
    {
        Assert.True(ObjMonSpiderSubclassCore.CenterIndexIsTwo());
        Assert.True(ObjMonSpiderSubclassCore.CenterMapsToTwo());
        Assert.True(ObjMonSpiderSubclassCore.EdgesMapToZeroAndFour());

        Assert.Equal(2, ObjMonSpiderSubclassCore.CellIndex(0));
        Assert.Equal(0, ObjMonSpiderSubclassCore.CellIndex(-2));
        Assert.Equal(4, ObjMonSpiderSubclassCore.CellIndex(2));
        Assert.Equal(1, ObjMonSpiderSubclassCore.CellIndex(-1));
        Assert.Equal(3, ObjMonSpiderSubclassCore.CellIndex(1));
    }

    // ===================== 四、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonSpiderSubclassCore.SpiderFamilyComplete());
        Assert.True(ObjMonSpiderSubclassCore.FiveClassesInFamily());
        Assert.True(ObjMonSpiderSubclassCore.ConfigOnlySubclasses());
        Assert.True(ObjMonSpiderSubclassCore.TwoMethodsEach());
        Assert.True(ObjMonSpiderSubclassCore.SevenClassesCovered());
        Assert.True(ObjMonSpiderSubclassCore.RemainingApprox());
    }
}

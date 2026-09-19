using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J190：`TCopyMon.ActThink`（12 行）与 `TCopyMon.Copy`（48 行）1:1 测试。
/// **本批最重要的发现是"缺 nil 守卫"一族在本单元凑齐三处**：
/// ① `ActThink` 解引用 `m_TargetCret` 而不判空（父类 `THumMon.ActThink`
/// 第 849 行有显式守卫）；② `Copy` 的 `if Source = nil then Exit;` 被注释掉
/// 而方法体仍无条件解引用 `Source`；③ `m_UseItems :=` 的赋值亦被注释、改用 `Move`。
/// </summary>
public sealed class CopyMonActThinkCopyCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(12, CopyMonActThinkCopyCore.ActThinkLines);
        Assert.Equal(1790, CopyMonActThinkCopyCore.ActThinkStart);
        Assert.Equal(1801, CopyMonActThinkCopyCore.ActThinkEnd);
        Assert.Equal(48, CopyMonActThinkCopyCore.CopyLines);
        Assert.Equal(2195, CopyMonActThinkCopyCore.CopyStart);
        Assert.Equal(2242, CopyMonActThinkCopyCore.CopyEnd);
        Assert.Equal(60, CopyMonActThinkCopyCore.TotalLines);
        Assert.Equal(112, CopyMonActThinkCopyCore.VK_F1);
        Assert.Equal(0, CopyMonActThinkCopyCore.JobWarrior);
        Assert.Equal(0, CopyMonActThinkCopyCore.RC_PLAYOBJECT);
        Assert.Equal(1, CopyMonActThinkCopyCore.RC_HEROOBJECT);
        Assert.Equal(10, CopyMonActThinkCopyCore.ViewRangeMin);
        Assert.Equal(2, CopyMonActThinkCopyCore.ViewRangeShrink);
        Assert.Equal(10, CopyMonActThinkCopyCore.SpellSpeedBound);
        Assert.Equal(2013, CopyMonActThinkCopyCore.TodoYear);
        Assert.Equal(2799, CopyMonActThinkCopyCore.TargetNilLine);
        Assert.Equal(849, CopyMonActThinkCopyCore.ParentGuardLine);
        Assert.Equal(4, CopyMonActThinkCopyCore.SpeedFieldCount);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(CopyMonActThinkCopyCore.SpanMatches());
        Assert.Equal(12, CopyMonActThinkCopyCore.ActThinkEnd - CopyMonActThinkCopyCore.ActThinkStart + 1);
        Assert.Equal(48, CopyMonActThinkCopyCore.CopyEnd - CopyMonActThinkCopyCore.CopyStart + 1);
    }

    // ===================== 一、ActThink 的 nil 风险 =====================

    [Fact]
    public void NilGuardFacts()
    {
        Assert.True(CopyMonActThinkCopyCore.NoNilGuardInOriginal());
        Assert.True(CopyMonActThinkCopyCore.ParentHasNilGuard());
        Assert.True(CopyMonActThinkCopyCore.ReachableFromRun());
        Assert.True(CopyMonActThinkCopyCore.PreservedAsIs());
        Assert.True(CopyMonActThinkCopyCore.ReadsTwoTargetMembers());
        Assert.True(CopyMonActThinkCopyCore.NullTargetCrashes());
        Assert.True(CopyMonActThinkCopyCore.CallLinesExtracted());
    }

    [Fact]
    public void ThreeCallSites()
    {
        Assert.Equal(new[] { 2124, 2144, 2164 }, CopyMonActThinkCopyCore.ActThinkCallLines);
    }

    [Fact]
    public void SkipAttackRequiresAllFour()
    {
        Assert.True(CopyMonActThinkCopyCore.RequiresAllFour());

        // **全满足**
        Assert.True(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 0, 0, true));
        Assert.True(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 1, 1, true));
        // **逐个破坏**
        Assert.False(CopyMonActThinkCopyCore.ShouldSkipAttack(false, 0, 0, true));
        Assert.False(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 80, 0, true));
        Assert.False(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 0, 80, true));
        Assert.False(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 0, 0, false));
    }

    [Fact]
    public void OnlyTwoRacesAccepted()
    {
        Assert.True(CopyMonActThinkCopyCore.OnlyTwoRacesAccepted());
        Assert.True(CopyMonActThinkCopyCore.SameRacePredicateBothSides());

        // **怪物种族（80）不被接受**
        Assert.False(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 80, 0, true));
        // **NPC 种族（10）不被接受**
        Assert.False(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 10, 0, true));
        // **英雄种族（1）被接受**
        Assert.True(CopyMonActThinkCopyCore.ShouldSkipAttack(true, 1, 0, true));
    }

    [Fact]
    public void TodoComment()
    {
        Assert.True(CopyMonActThinkCopyCore.TodoHasOwnerAndCategory());
        Assert.True(CopyMonActThinkCopyCore.TodoDate2013());
        Assert.True(CopyMonActThinkCopyCore.FirstStructuredTodo());
    }

    [Fact]
    public void ActThinkReturnSemantics()
    {
        Assert.True(CopyMonActThinkCopyCore.ReturnsTrueOnHit());
        Assert.True(CopyMonActThinkCopyCore.NoSideEffectOnHit());
        Assert.True(CopyMonActThinkCopyCore.TrueMeansHandled());
        Assert.True(CopyMonActThinkCopyCore.HitShortCircuits());
        Assert.True(CopyMonActThinkCopyCore.MissDelegates());

        // **命中即真、无论继承结果**
        Assert.True(CopyMonActThinkCopyCore.ActThink(true, false));
        Assert.True(CopyMonActThinkCopyCore.ActThink(true, true));
        // **未命中取继承结果**
        Assert.False(CopyMonActThinkCopyCore.ActThink(false, false));
        Assert.True(CopyMonActThinkCopyCore.ActThink(false, true));
    }

    [Fact]
    public void InheritedStyle()
    {
        Assert.True(CopyMonActThinkCopyCore.InheritedOnMiss());
        Assert.True(CopyMonActThinkCopyCore.Unconditional());
        Assert.True(CopyMonActThinkCopyCore.ConsistentWithJ187Style());
    }

    [Fact]
    public void NoInstrumentationHere()
    {
        Assert.True(CopyMonActThinkCopyCore.NoInstrumentation());
        Assert.True(CopyMonActThinkCopyCore.NoExceptionHandler());
        Assert.True(CopyMonActThinkCopyCore.DiffersFromJ188());
    }

    // ===================== 二、Copy 的守卫与分组 =====================

    [Fact]
    public void CommentedGuards()
    {
        Assert.True(CopyMonActThinkCopyCore.CommentedNilGuard());
        Assert.True(CopyMonActThinkCopyCore.CommentedViewRange());
        Assert.True(CopyMonActThinkCopyCore.SourceDereferencedUnguarded());
        Assert.True(CopyMonActThinkCopyCore.CommentedTextsVerbatim());

        // **逐字保留的注释原文**
        Assert.Equal("// if Source = nil then Exit;", CopyMonActThinkCopyCore.CommentedGuardText);
        Assert.Equal(
            "// m_nViewRange := Max(Source.m_nViewRange - 2, 10);",
            CopyMonActThinkCopyCore.CommentedViewText);
    }

    [Fact]
    public void LostViewRangeRule()
    {
        Assert.True(CopyMonActThinkCopyCore.ViewRangeRuleLost());
        Assert.True(CopyMonActThinkCopyCore.MinTenFloor());
        Assert.True(CopyMonActThinkCopyCore.LostViewRangeValues());

        // **缩减两格**
        Assert.Equal(28, CopyMonActThinkCopyCore.LostViewRange(30));
        // **下限十**
        Assert.Equal(10, CopyMonActThinkCopyCore.LostViewRange(11));
        Assert.Equal(10, CopyMonActThinkCopyCore.LostViewRange(5));
    }

    [Fact]
    public void CopyGrouping()
    {
        Assert.True(CopyMonActThinkCopyCore.PureStateClone());
        Assert.True(CopyMonActThinkCopyCore.ThreeGroups());
        Assert.True(CopyMonActThinkCopyCore.AbilAssignedThenFull());
        Assert.True(CopyMonActThinkCopyCore.BothAbilAndWAbil());
        Assert.True(CopyMonActThinkCopyCore.NotInheritCurrentHp());
    }

    [Fact]
    public void CloneFillsAbility()
    {
        Assert.True(CopyMonActThinkCopyCore.CloneIgnoresSourceCurrentValues());

        // **满血满蓝、与源当前值无关**
        Assert.Equal((500, 300), CopyMonActThinkCopyCore.CloneAbility(500, 300, 1, 2));
        Assert.Equal((500, 300), CopyMonActThinkCopyCore.CloneAbility(500, 300, 499, 299));
    }

    [Fact]
    public void MoveSemantics()
    {
        Assert.True(CopyMonActThinkCopyCore.BytewiseMove());
        Assert.True(CopyMonActThinkCopyCore.NotElementWise());
        Assert.True(CopyMonActThinkCopyCore.TestsSourceTypeNotSelf());
        Assert.True(CopyMonActThinkCopyCore.TwoIndependentChecks());
    }

    [Fact]
    public void UseItemsChanged()
    {
        Assert.True(CopyMonActThinkCopyCore.UseItemsMoveNotAssign());
        Assert.True(CopyMonActThinkCopyCore.CommentedAssign());
        Assert.Equal(
            "// m_UseItems := TSmartObject(Source).m_UseItems;",
            CopyMonActThinkCopyCore.CommentedUseItemsText);
    }

    [Fact]
    public void SpeedFields()
    {
        Assert.True(CopyMonActThinkCopyCore.FourSpeedFields());
        Assert.True(CopyMonActThinkCopyCore.SpeedRangeComment());
    }

    [Fact]
    public void SemicolonTypo()
    {
        Assert.True(CopyMonActThinkCopyCore.DoubleSemicolon());
        Assert.True(CopyMonActThinkCopyCore.SemicolonTypoFamily());
        Assert.True(CopyMonActThinkCopyCore.ThirdInFamily());
    }

    // ===================== 三、技能克隆 =====================

    [Fact]
    public void DeepCopyFacts()
    {
        Assert.True(CopyMonActThinkCopyCore.DeepCopy());
        Assert.True(CopyMonActThinkCopyCore.NewThenAssignThenAdd());
        Assert.True(CopyMonActThinkCopyCore.JobFromClone());
        Assert.True(CopyMonActThinkCopyCore.JobZeroIsWarrior());
        Assert.True(CopyMonActThinkCopyCore.EquivalentToMaster());
    }

    [Fact]
    public void VkF1()
    {
        Assert.True(CopyMonActThinkCopyCore.VkF1Is112());
        Assert.True(CopyMonActThinkCopyCore.FromWindowsRtl());
        Assert.True(CopyMonActThinkCopyCore.MustBeExplicitInCsharp());
        Assert.True(CopyMonActThinkCopyCore.F1InFunctionKeyRange());

        Assert.Equal(112, CopyMonActThinkCopyCore.VK_F1);
    }

    [Fact]
    public void KeyRebinding()
    {
        Assert.True(CopyMonActThinkCopyCore.OnlyJobZeroRebound());
        Assert.True(CopyMonActThinkCopyCore.OthersKeepOriginal());
        Assert.True(CopyMonActThinkCopyCore.CloneKeyBindingValues());
        Assert.True(CopyMonActThinkCopyCore.OnlyWarriorRebound());

        // **战士技能改绑 F1**
        Assert.Equal(112, CopyMonActThinkCopyCore.CloneKeyBinding(0, 49));
        // **法师技能保持原键**
        Assert.Equal(49, CopyMonActThinkCopyCore.CloneKeyBinding(1, 49));
        Assert.Equal(51, CopyMonActThinkCopyCore.CloneKeyBinding(2, 51));
    }
}

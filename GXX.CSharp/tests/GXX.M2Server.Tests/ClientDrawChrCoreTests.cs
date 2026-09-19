using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J177：客户端角色绘制路径 1:1 测试。
/// **四个 DrawChr 覆盖链（只有雕像调 inherited）、
/// NPC 绘制顺序恰好是三个层的全部六种排列、
/// 绘制闸门的真值表与"标志语义是抑制而非允许"、
/// 以及方向取模的排除区间。**
/// </summary>
public sealed class ClientDrawChrCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(10000, ClientDrawChrCore.CustomNpcThreshold);
        Assert.Equal(3, ClientDrawChrCore.NpcDirModulo);
        Assert.Equal(246, ClientDrawChrCore.ExcludeLow);
        Assert.Equal(272, ClientDrawChrCore.ExcludeHigh);
        Assert.Equal(51, ClientDrawChrCore.SpecialSingle);
        Assert.Equal(-200, ClientDrawChrCore.StatuaryOffsetX);
        Assert.Equal(-237, ClientDrawChrCore.StatuaryOffsetY);
        Assert.Equal(20, ClientDrawChrCore.ScaleInflate);
        Assert.Equal(10, ClientDrawChrCore.FrameAdvanceEarly);
    }

    [Fact]
    public void SpecialRanges()
    {
        Assert.Equal((54, 58), ClientDrawChrCore.SpecialRangeA);
        Assert.Equal((94, 98), ClientDrawChrCore.SpecialRangeB);
        Assert.True(ClientDrawChrCore.BothRangesAreFiveWide());
        Assert.True(ClientDrawChrCore.RangeBoundaries());
        Assert.True(ClientDrawChrCore.RangesDisjoint());
        Assert.True(ClientDrawChrCore.SingleOutsideRanges());
    }

    // ===================== 一、覆盖链 =====================

    [Fact]
    public void Chain()
    {
        Assert.True(ClientDrawChrCore.FourOverrides());
        Assert.True(ClientDrawChrCore.HierarchyShape());
        Assert.True(ClientDrawChrCore.BaseIsVirtual());
        Assert.True(ClientDrawChrCore.FourChainEntries());
        Assert.Equal(4, ClientDrawChrCore.Chain.Length);
    }

    [Fact]
    public void InheritedOnlyInStatuary()
    {
        Assert.True(ClientDrawChrCore.OnlyStatuaryCallsInherited());
        Assert.True(ClientDrawChrCore.NpcAndHumFullyReplace());
        Assert.True(ClientDrawChrCore.StatuaryDrawsTwice());
        Assert.True(ClientDrawChrCore.ExactlyOneInherited());
    }

    [Fact]
    public void DirCheckPosition()
    {
        Assert.True(ClientDrawChrCore.BaseChecksDirFirst());
        Assert.True(ClientDrawChrCore.HumChecksDirLate());
        Assert.True(ClientDrawChrCore.SameCheckDifferentPosition());
        Assert.True(ClientDrawChrCore.IllegalDirStillDrawsSelfEffect());
    }

    [Fact]
    public void DirValidation()
    {
        Assert.True(ClientDrawChrCore.EightDirsValid());
        Assert.True(ClientDrawChrCore.DirIsValid(0));
        Assert.True(ClientDrawChrCore.DirIsValid(7));
        Assert.False(ClientDrawChrCore.DirIsValid(-1));
        Assert.False(ClientDrawChrCore.DirIsValid(8));
    }

    // ===================== 二、基类顺序与魔法帧 =====================

    [Fact]
    public void BaseOrder()
    {
        Assert.True(ClientDrawChrCore.BaseOrderIsBodyThenStateThenMagic());
        Assert.True(ClientDrawChrCore.PluginHooksWrapped());
    }

    [Fact]
    public void SpellFrameRange()
    {
        Assert.True(ClientDrawChrCore.FrameRangeInclusive());
        Assert.True(ClientDrawChrCore.SpellRangeBoundary());

        Assert.True(ClientDrawChrCore.InSpellRange(0, 5));
        Assert.True(ClientDrawChrCore.InSpellRange(4, 5));
        Assert.False(ClientDrawChrCore.InSpellRange(5, 5));
        Assert.False(ClientDrawChrCore.InSpellRange(-1, 5));
    }

    [Fact]
    public void IndexPlusFrame()
    {
        Assert.True(ClientDrawChrCore.IndexPlusFrame());
        Assert.True(ClientDrawChrCore.MagicIndexValues());
        Assert.Equal(103, ClientDrawChrCore.MagicIndex(100, 3));
    }

    [Fact]
    public void GraySameAsJ176()
    {
        Assert.True(ClientDrawChrCore.SameGrayCriterionAsJ176());
        Assert.True(ClientDrawChrCore.ChecksSelfNotActor());
        Assert.True(ClientDrawChrCore.GrayValues());
    }

    [Fact]
    public void GrayModel()
    {
        // **"自己非空且已死亡"才用灰度**
        Assert.False(ClientDrawChrCore.UseGray(true, true));
        Assert.False(ClientDrawChrCore.UseGray(true, false));
        Assert.False(ClientDrawChrCore.UseGray(false, false));
        Assert.True(ClientDrawChrCore.UseGray(false, true));
    }

    // ===================== 三、插件钩子 =====================

    [Fact]
    public void Hooks()
    {
        Assert.True(ClientDrawChrCore.ConditionalCompilation());
        Assert.True(ClientDrawChrCore.TwoHooksBothGuarded());
        Assert.True(ClientDrawChrCore.SwallowsException());
        Assert.True(ClientDrawChrCore.TwoMessagesPerHandler());
        Assert.True(ClientDrawChrCore.TwoHooks());
        Assert.True(ClientDrawChrCore.HooksNumberedConsecutively());
        Assert.Equal(2, ClientDrawChrCore.Hooks.Length);
    }

    // ===================== 四、绘制顺序六排列 =====================

    [Fact]
    public void SixOrders()
    {
        Assert.True(ClientDrawChrCore.SixDrawOrders());
        Assert.True(ClientDrawChrCore.SixOrderEntries());
        Assert.Equal(6, ClientDrawChrCore.Orders.Length);
    }

    [Fact]
    public void OrdersArePermutations()
    {
        // **恰好是三个层的全部六种排列**
        Assert.True(ClientDrawChrCore.ExactlySixPermutations());
        Assert.True(ClientDrawChrCore.NoDuplicates());
        Assert.True(ClientDrawChrCore.SetEquals());
        Assert.True(ClientDrawChrCore.AllPermutationsCovered());
        Assert.Equal(6, ClientDrawChrCore.Factorial(3));
    }

    [Fact]
    public void OrderTable()
    {
        Assert.True(ClientDrawChrCore.OrderTableMatchesEnum());

        Assert.Equal(new[] { ClientDrawChrCore.Layer.Keep, ClientDrawChrCore.Layer.Body, ClientDrawChrCore.Layer.Effect },
            ClientDrawChrCore.DrawSequence(ClientDrawChrCore.NpcDrawOrder.KeepChrEff));

        Assert.Equal(new[] { ClientDrawChrCore.Layer.Effect, ClientDrawChrCore.Layer.Body, ClientDrawChrCore.Layer.Keep },
            ClientDrawChrCore.DrawSequence(ClientDrawChrCore.NpcDrawOrder.EffChrKeep));
    }

    [Fact]
    public void CustomPath()
    {
        Assert.True(ClientDrawChrCore.CustomPathThreshold10000());
    }

    // ===================== 五、三个嵌套过程 =====================

    [Fact]
    public void NestedProcs()
    {
        Assert.True(ClientDrawChrCore.BodyChecksTwice());
        Assert.True(ClientDrawChrCore.EffectAndKeepOnce());
        Assert.True(ClientDrawChrCore.TwoBranchesIdenticalExceptCondition());
    }

    [Fact]
    public void DrawMode()
    {
        Assert.True(ClientDrawChrCore.ModeSelectsBlend());
        Assert.True(ClientDrawChrCore.BodyPassesConstantTrue());
        Assert.True(ClientDrawChrCore.ModeIsNotTheBlendArgument());
        Assert.True(ClientDrawChrCore.ModeValues());

        Assert.False(ClientDrawChrCore.UsesBlend(ClientDrawChrCore.DrawMode.Normal));
        Assert.True(ClientDrawChrCore.UsesBlend(ClientDrawChrCore.DrawMode.Blend));
    }

    [Fact]
    public void CoordTerms()
    {
        // **保留层四层加法、效果与身体三层**
        Assert.True(ClientDrawChrCore.KeepHasFourTerms());
        Assert.True(ClientDrawChrCore.EffectAndBodyHaveThree());
        Assert.True(ClientDrawChrCore.ConfigOffsetsOnlyForKeep());
        Assert.True(ClientDrawChrCore.CoordTermValues());
        Assert.Equal(4, ClientDrawChrCore.CoordTerms(true));
        Assert.Equal(3, ClientDrawChrCore.CoordTerms(false));
    }

    // ===================== 六、普通 NPC 路径 =====================

    [Fact]
    public void DirModulo()
    {
        Assert.True(ClientDrawChrCore.DirModuloThree());
        Assert.True(ClientDrawChrCore.Range246To272Excluded());
        Assert.True(ClientDrawChrCore.ExclusionIsInvertedNarrowRange());
        Assert.True(ClientDrawChrCore.ModuloBoundary());
    }

    [Fact]
    public void ModuloModel()
    {
        // **区间内不取模、区间外取模**
        Assert.True(ClientDrawChrCore.AppliesModulo(245));
        Assert.False(ClientDrawChrCore.AppliesModulo(246));
        Assert.False(ClientDrawChrCore.AppliesModulo(272));
        Assert.True(ClientDrawChrCore.AppliesModulo(273));

        Assert.True(ClientDrawChrCore.ModuloKeepsDirInRange());
        Assert.True(ClientDrawChrCore.ModuloReducesOutside());
        Assert.Equal(5, ClientDrawChrCore.NpcDir(5, 260));
        Assert.Equal(2, ClientDrawChrCore.NpcDir(5, 100));
    }

    [Fact]
    public void AppearanceSpecials()
    {
        Assert.True(ClientDrawChrCore.TwoRangesBlend());
        Assert.True(ClientDrawChrCore.SingleValue51Special());
        Assert.True(ClientDrawChrCore.ThreeAppearanceSpecialCases());
    }

    // ===================== 七、雕像类 =====================

    [Fact]
    public void StatuaryFlag()
    {
        Assert.True(ClientDrawChrCore.CallsInheritedFirst());
        Assert.True(ClientDrawChrCore.OneShotResetFlag());
        Assert.True(ClientDrawChrCore.ResetOnlyOnce());
        Assert.True(ClientDrawChrCore.CommentExplainsFullscreenBug());
        Assert.True(ClientDrawChrCore.FlagSetInFinalize());
        Assert.True(ClientDrawChrCore.ConsumedInDraw());
        Assert.True(ClientDrawChrCore.ProducerConsumerPair());
    }

    [Fact]
    public void ResetModel()
    {
        Assert.True(ClientDrawChrCore.ResetModel());
        Assert.True(ClientDrawChrCore.SecondCallDoesNotReset());

        var a = ClientDrawChrCore.ResetOnce(true);
        Assert.True(a.DidReset);
        Assert.False(a.Flag);

        var b = ClientDrawChrCore.ResetOnce(false);
        Assert.False(b.DidReset);
    }

    [Fact]
    public void StatuaryLayers()
    {
        Assert.True(ClientDrawChrCore.TwoScaledPaths());
        Assert.True(ClientDrawChrCore.SameLayerOrderBothPaths());
        Assert.True(ClientDrawChrCore.ThreeLayersEach());
        Assert.True(ClientDrawChrCore.EachLayerNullChecked());
        Assert.True(ClientDrawChrCore.ThreeStatuaryLayers());
        Assert.True(ClientDrawChrCore.BodyIsLast());
        Assert.Equal(3, ClientDrawChrCore.StatuaryLayers.Length);
    }

    [Fact]
    public void Scaling()
    {
        Assert.True(ClientDrawChrCore.InflateBy20());
        Assert.True(ClientDrawChrCore.SourceUnchanged());
        Assert.True(ClientDrawChrCore.ScaledGrowsBy40());
    }

    [Fact]
    public void Anchor()
    {
        Assert.True(ClientDrawChrCore.FixedOffset200And237());
        Assert.True(ClientDrawChrCore.HardcodedAnchor());
        Assert.True(ClientDrawChrCore.ScreenShakeAddedFirst());
        Assert.True(ClientDrawChrCore.AnchorValues());
    }

    [Fact]
    public void DestRectModel()
    {
        var normal = ClientDrawChrCore.ScaledDest(100, 200, 0, 0, 0, 0, false);
        var scaled = ClientDrawChrCore.ScaledDest(100, 200, 0, 0, 0, 0, true);

        Assert.Equal(-200, normal.Left);
        Assert.Equal(-237, normal.Top);
        Assert.Equal(-220, scaled.Left);
        Assert.Equal(-257, scaled.Top);
        Assert.Equal(140, scaled.W);
        Assert.Equal(240, scaled.H);
    }

    // ===================== 八、人物类闸门 =====================

    [Fact]
    public void GateShape()
    {
        Assert.True(ClientDrawChrCore.AsymmetricGate());
        Assert.True(ClientDrawChrCore.LocalPlayerExtraClause());
        Assert.True(ClientDrawChrCore.StealthRelaxesGate());
        Assert.True(ClientDrawChrCore.CommentExplainsStealth());
        Assert.True(ClientDrawChrCore.StealthIsOnlyRescue());
    }

    [Fact]
    public void OtherActorIgnoresFlag()
    {
        Assert.True(ClientDrawChrCore.OtherActorIgnoresFlag());
    }

    [Fact]
    public void SelfFlagSemantics()
    {
        // **标志为假时绘制、为真时不绘制（标志语义是抑制）**
        Assert.True(ClientDrawChrCore.SelfDependsOnFlag());

        Assert.False(ClientDrawChrCore.GateIsDraw(true, ClientDrawChrCore.SelfDrawOrder.PriorSelf, false, true, false));
        Assert.True(ClientDrawChrCore.GateIsDraw(true, ClientDrawChrCore.SelfDrawOrder.PriorSelf, false, false, false));
    }

    [Fact]
    public void StealthRescue()
    {
        // **标志为真时只有隐身位能救回**
        Assert.True(ClientDrawChrCore.StealthRescuesWhenFlagTrue());
        Assert.True(ClientDrawChrCore.StealthIrrelevantWhenFlagFalse());

        Assert.True(ClientDrawChrCore.GateIsDraw(true, ClientDrawChrCore.SelfDrawOrder.PriorSelf, false, true, true));
        Assert.False(ClientDrawChrCore.GateIsDraw(true, ClientDrawChrCore.SelfDrawOrder.PriorSelf, false, true, false));
    }

    [Fact]
    public void PriorMagicBranch()
    {
        Assert.True(ClientDrawChrCore.PriorMagicRequiresFlagOnlyForSelf());

        Assert.True(ClientDrawChrCore.GateIsDraw(true, ClientDrawChrCore.SelfDrawOrder.PriorMagic, true, true, false));
        Assert.False(ClientDrawChrCore.GateIsDraw(true, ClientDrawChrCore.SelfDrawOrder.PriorMagic, true, false, false));
        Assert.True(ClientDrawChrCore.GateIsDraw(false, ClientDrawChrCore.SelfDrawOrder.PriorMagic, true, false, false));
    }

    [Fact]
    public void BeforeDrawSemantics()
    {
        // **"先效果"要求已在绘制前；"先自身"要求不在绘制前**
        Assert.False(ClientDrawChrCore.GateIsDraw(false, ClientDrawChrCore.SelfDrawOrder.PriorMagic, false, true, false));
        Assert.False(ClientDrawChrCore.GateIsDraw(false, ClientDrawChrCore.SelfDrawOrder.PriorSelf, true, true, false));
        Assert.True(ClientDrawChrCore.GateIsDraw(false, ClientDrawChrCore.SelfDrawOrder.PriorSelf, false, true, false));
    }

    [Fact]
    public void CallSites()
    {
        Assert.True(ClientDrawChrCore.FourCallSites());
        Assert.True(ClientDrawChrCore.FourFlagCombinations());
        Assert.True(ClientDrawChrCore.AllCombinationsUsed());
        Assert.True(ClientDrawChrCore.FourCallFlagEntries());
        Assert.True(ClientDrawChrCore.CallFlagsAllDistinct());
        Assert.Equal(4, ClientDrawChrCore.CallFlags.Length);
    }

    // ---------- 强化等级 ----------

    [Fact]
    public void PlusLevel()
    {
        Assert.True(ClientDrawChrCore.PlusLevelMapping());
        Assert.True(ClientDrawChrCore.RangesAreThreeWide());
        Assert.True(ClientDrawChrCore.OverflowFallsToTop());
        Assert.True(ClientDrawChrCore.PlusLevelBoundaries());
    }

    [Fact]
    public void PlusLevelModel()
    {
        Assert.Equal("mplNone", ClientDrawChrCore.PlusLevel(0));
        Assert.Equal("mpl1_3", ClientDrawChrCore.PlusLevel(1));
        Assert.Equal("mpl1_3", ClientDrawChrCore.PlusLevel(3));
        Assert.Equal("mpl4_6", ClientDrawChrCore.PlusLevel(4));
        Assert.Equal("mpl4_6", ClientDrawChrCore.PlusLevel(6));
        Assert.Equal("mpl7_9", ClientDrawChrCore.PlusLevel(7));
        Assert.Equal("mpl7_9", ClientDrawChrCore.PlusLevel(9));
        Assert.Equal("mpl7_9", ClientDrawChrCore.PlusLevel(10));
    }

    // ---------- 帧推进 ----------

    [Fact]
    public void FrameAdvance()
    {
        Assert.True(ClientDrawChrCore.EarlyBy10ms());
        Assert.True(ClientDrawChrCore.MinusZeroIsNoop());
        Assert.True(ClientDrawChrCore.FrameClampIsPlainLessThan());
        Assert.True(ClientDrawChrCore.AdvanceBoundary());
        Assert.True(ClientDrawChrCore.FrameClampBoundary());
    }

    [Fact]
    public void FrameAdvanceModel()
    {
        // **提前十毫秒：播放时间一百时，九十即推进、八十九不推进**
        Assert.True(ClientDrawChrCore.ShouldAdvance(90, 0, 100));
        Assert.False(ClientDrawChrCore.ShouldAdvance(89, 0, 100));

        Assert.True(ClientDrawChrCore.CanAdvanceFrame(9, 10));
        Assert.False(ClientDrawChrCore.CanAdvanceFrame(10, 10));
    }

    // ---------- 重复与注释 ----------

    [Fact]
    public void Duplication()
    {
        Assert.True(ClientDrawChrCore.DuplicatedBlock());
        Assert.True(ClientDrawChrCore.IsWarrDrawRedundantInSecond());
        Assert.True(ClientDrawChrCore.TwoIdenticalBranches());
    }

    [Fact]
    public void Comments()
    {
        Assert.True(ClientDrawChrCore.CommentedStartIndexCheck());
        Assert.True(ClientDrawChrCore.TwoFieldNamesOneCommented());
        Assert.True(ClientDrawChrCore.TwoFrameFields());
        Assert.True(ClientDrawChrCore.ExactlyOneEffective());
        Assert.Equal(2, ClientDrawChrCore.FrameFields.Length);
    }

    [Fact]
    public void Shadowing()
    {
        Assert.True(ClientDrawChrCore.ShadowedParameterNames());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientDrawChrCore.FiveFragments());
        Assert.Equal(5, ClientDrawChrCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 64, 127, 42, 5, 9 }, ClientDrawChrCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(ClientDrawChrCore.NpcIsLongest());
        Assert.True(ClientDrawChrCore.FinalizeIsShortest());
        Assert.True(ClientDrawChrCore.NpcShareIs51());
        Assert.True(ClientDrawChrCore.NpcExceedsBaseBy63());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(ClientDrawChrCore.TotalLinesValues());
        Assert.Equal(247, ClientDrawChrCore.TotalLines());
    }
}

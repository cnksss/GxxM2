using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J193：`TCopyMon.ScatterBagItems` / `DropUseItems` / `RecalcLevelAbilitys` /
/// `AllowUseMagic` 1:1 测试（合计 30 行）。
/// **核心是"空覆写"形态**：全单元仅有的两处空体覆写都在 `TCopyMon`，
/// 且基类同名方法是带实体的大方法、父类同名方法也有实体 ——
/// 故"分身不掉落任何物品"是刻意设计，不是漏写。
/// </summary>
public sealed class CopyMonStubsMagicCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(3, CopyMonStubsMagicCore.ScatterLines);
        Assert.Equal(2187, CopyMonStubsMagicCore.ScatterStart);
        Assert.Equal(2189, CopyMonStubsMagicCore.ScatterEnd);
        Assert.Equal(3, CopyMonStubsMagicCore.DropLines);
        Assert.Equal(2191, CopyMonStubsMagicCore.DropStart);
        Assert.Equal(2193, CopyMonStubsMagicCore.DropEnd);
        Assert.Equal(4, CopyMonStubsMagicCore.RecalcLevelLines);
        Assert.Equal(2544, CopyMonStubsMagicCore.RecalcLevelStart);
        Assert.Equal(2547, CopyMonStubsMagicCore.RecalcLevelEnd);
        Assert.Equal(20, CopyMonStubsMagicCore.AllowMagicLines);
        Assert.Equal(1706, CopyMonStubsMagicCore.AllowMagicStart);
        Assert.Equal(1725, CopyMonStubsMagicCore.AllowMagicEnd);
        Assert.Equal(30, CopyMonStubsMagicCore.TotalLines);
        Assert.Equal(3540, CopyMonStubsMagicCore.UnitLines);
        Assert.Equal(2, CopyMonStubsMagicCore.EmptyOverrideCount);
        Assert.Equal(764, CopyMonStubsMagicCore.BaseScatterDeclLine);
        Assert.Equal(761, CopyMonStubsMagicCore.BaseDropDeclLine);
        Assert.Equal(34091, CopyMonStubsMagicCore.BaseScatterImplLine);
        Assert.Equal(34353, CopyMonStubsMagicCore.BaseDropImplLine);
        Assert.Equal(1442, CopyMonStubsMagicCore.ParentScatterLine);
        Assert.Equal(1561, CopyMonStubsMagicCore.ParentDropLine);
        Assert.Equal(579, CopyMonStubsMagicCore.ParentRecalcLevelLine);
        Assert.Equal(67, CopyMonStubsMagicCore.DeclScatterLine);
        Assert.Equal(68, CopyMonStubsMagicCore.DeclDropLine);
        Assert.Equal(2548, CopyMonStubsMagicCore.OrphanCommentLine2);
        Assert.Equal(2265, CopyMonStubsMagicCore.OrphanCommentLine1);
        Assert.Equal(4, CopyMonStubsMagicCore.NestedIfCount);
        Assert.Equal(1, CopyMonStubsMagicCore.AllowMagicLocals);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(CopyMonStubsMagicCore.SpanMatches());
        Assert.True(CopyMonStubsMagicCore.AllTiny());
        Assert.True(CopyMonStubsMagicCore.UnitLineCount());
        Assert.True(CopyMonStubsMagicCore.ContrastWithRun());
    }

    // ===================== 一、空覆写 =====================

    [Fact]
    public void EmptyOverrideFacts()
    {
        Assert.True(CopyMonStubsMagicCore.OnlyTwoEmptyOverrides());
        Assert.True(CopyMonStubsMagicCore.BothInCopyMon());
        Assert.True(CopyMonStubsMagicCore.ExhaustiveScanConfirmed());
        Assert.True(CopyMonStubsMagicCore.DeclaredBeforeImplemented());
        Assert.True(CopyMonStubsMagicCore.BaseLinesOrdered());
    }

    [Fact]
    public void EmptyOverrideLines()
    {
        Assert.Equal(new[] { 2187, 2191 }, CopyMonStubsMagicCore.EmptyOverrideLines);

        // **两个方法都是三行（头、begin、end;）**
        Assert.Equal(3, CopyMonStubsMagicCore.ScatterLines);
        Assert.Equal(3, CopyMonStubsMagicCore.DropLines);
    }

    [Fact]
    public void DeliberateSuppression()
    {
        Assert.True(CopyMonStubsMagicCore.BaseIsVirtual());
        Assert.True(CopyMonStubsMagicCore.BaseHasRealBody());
        Assert.True(CopyMonStubsMagicCore.DeliberateSuppression());
        Assert.True(CopyMonStubsMagicCore.ParentHasBody());
        Assert.True(CopyMonStubsMagicCore.OppositeDropPolicy());
        Assert.True(CopyMonStubsMagicCore.BothParentsSubstantive());
        Assert.True(CopyMonStubsMagicCore.PairedSuppression());
        Assert.True(CopyMonStubsMagicCore.SignaturesPreserved());
        Assert.True(CopyMonStubsMagicCore.PolymorphismStillDispatches());
    }

    [Fact]
    public void OverrideDropsNothing()
    {
        Assert.True(CopyMonStubsMagicCore.EmptyBodyHasNoEffect());
        Assert.True(CopyMonStubsMagicCore.BaseDiffersFromOverride());

        // **分身：任意候选数都掉零件**
        Assert.Equal(0, CopyMonStubsMagicCore.EmptyOverrideDropCount(0));
        Assert.Equal(0, CopyMonStubsMagicCore.EmptyOverrideDropCount(999));

        // **基类：掉三件**
        Assert.Equal(3, CopyMonStubsMagicCore.BaseScatterDropCount(3));
    }

    [Fact]
    public void DeclarationsAdjacent()
    {
        Assert.True(CopyMonStubsMagicCore.AdjacentDeclarations());
        Assert.True(CopyMonStubsMagicCore.DropDeclLinesExtracted());
        Assert.True(CopyMonStubsMagicCore.ParentLinesExtracted());

        Assert.Equal(new[] { 67, 68 }, CopyMonStubsMagicCore.DropDeclLines);
    }

    // ===================== 二、RecalcLevelAbilitys =====================

    [Fact]
    public void RecalcLevelIsPassThrough()
    {
        Assert.True(CopyMonStubsMagicCore.AnotherPassThrough());
        Assert.True(CopyMonStubsMagicCore.SecondShellInCopyMon());
        Assert.True(CopyMonStubsMagicCore.ParentRecalcHasBody());
        Assert.True(CopyMonStubsMagicCore.ParentRecalcLineExtracted());
    }

    [Fact]
    public void SecondOrphanComment()
    {
        Assert.True(CopyMonStubsMagicCore.OrphanCommentAgain());
        Assert.True(CopyMonStubsMagicCore.SecondInstance());
        Assert.True(CopyMonStubsMagicCore.DescribesMakeGhost());
        Assert.True(CopyMonStubsMagicCore.OrphanBetweenRecalcAndGhost());
        Assert.True(CopyMonStubsMagicCore.OrphanLinesExtracted());

        Assert.Equal(new[] { 2265, 2548 }, CopyMonStubsMagicCore.OrphanCommentLines);
    }

    [Fact]
    public void OrphanCommentVerbatim()
    {
        Assert.True(CopyMonStubsMagicCore.VerbatimKept());
        Assert.True(CopyMonStubsMagicCore.TyposPreserved());
        Assert.True(CopyMonStubsMagicCore.OrphanTextVerbatim());

        // **逐字保留（含"到时""要向"两处口语化措辞）**
        Assert.Equal("// 分身到时消灭要向英雄一样有特效",
            CopyMonStubsMagicCore.OrphanCommentText);
    }

    // ===================== 三、AllowUseMagic =====================

    [Fact]
    public void NestedIfStructure()
    {
        Assert.True(CopyMonStubsMagicCore.FourNestedIfs());
        Assert.True(CopyMonStubsMagicCore.NoElseAnywhere());
        Assert.True(CopyMonStubsMagicCore.AllMustHold());
        Assert.True(CopyMonStubsMagicCore.GatesExtracted());
        Assert.True(CopyMonStubsMagicCore.OneLocal());
    }

    [Fact]
    public void GatesInOrder()
    {
        Assert.Equal(5, CopyMonStubsMagicCore.AllowMagicGates.Length);
        Assert.Equal("wMagIdx < Length(m_UserMagics)", CopyMonStubsMagicCore.AllowMagicGates[0]);
        Assert.Equal("m_PEnvir.AllowMagics(wMagIdx)", CopyMonStubsMagicCore.AllowMagicGates[1]);
        Assert.Equal("UserMagic <> nil", CopyMonStubsMagicCore.AllowMagicGates[2]);
        Assert.Equal("UserMagic.btKey > 0", CopyMonStubsMagicCore.AllowMagicGates[3]);
        Assert.Equal("inherited AllowUseMagic", CopyMonStubsMagicCore.AllowMagicGates[4]);
    }

    [Fact]
    public void AllGatesRequired()
    {
        Assert.True(CopyMonStubsMagicCore.AllowMagicRequiresAll());

        // **全满足 → 真**
        Assert.True(CopyMonStubsMagicCore.AllowUseMagic(true, true, true, true, true));

        // **逐个破坏 → 假**
        Assert.False(CopyMonStubsMagicCore.AllowUseMagic(false, true, true, true, true));
        Assert.False(CopyMonStubsMagicCore.AllowUseMagic(true, false, true, true, true));
        Assert.False(CopyMonStubsMagicCore.AllowUseMagic(true, true, false, true, true));
        Assert.False(CopyMonStubsMagicCore.AllowUseMagic(true, true, true, false, true));
        Assert.False(CopyMonStubsMagicCore.AllowUseMagic(true, true, true, true, false));
    }

    [Fact]
    public void KeyGateBoundary()
    {
        Assert.True(CopyMonStubsMagicCore.KeyMustBePositive());
        Assert.True(CopyMonStubsMagicCore.ZeroKeyDisallowed());
        Assert.True(CopyMonStubsMagicCore.KeyGateIsDecisive());
        Assert.True(CopyMonStubsMagicCore.KeyGateBoundary());
        Assert.True(CopyMonStubsMagicCore.LinksToJ190Rebinding());
    }

    [Fact]
    public void IndexGateUpperBoundOnly()
    {
        Assert.True(CopyMonStubsMagicCore.UpperBoundOnly());
        Assert.True(CopyMonStubsMagicCore.WordIsUnsigned());
        Assert.True(CopyMonStubsMagicCore.NoLowerBoundNeeded());
        Assert.True(CopyMonStubsMagicCore.IndexGateStrict());
    }

    [Fact]
    public void CommentedAlternative()
    {
        Assert.True(CopyMonStubsMagicCore.CommentedAlternative());
        Assert.True(CopyMonStubsMagicCore.IndexVsFind());
        Assert.True(CopyMonStubsMagicCore.CommentedTextVerbatim());
        Assert.True(CopyMonStubsMagicCore.CommentedFindMagicVerbatim());

        Assert.Equal("// FindMagic(wMagIdx);", CopyMonStubsMagicCore.CommentedFindMagicText);
    }

    [Fact]
    public void ResultAssignmentAndDefaultParam()
    {
        Assert.True(CopyMonStubsMagicCore.ResultAssignedTwice());
        Assert.True(CopyMonStubsMagicCore.NoIntermediateAssign());
        Assert.True(CopyMonStubsMagicCore.DefaultInDeclaration());
        Assert.True(CopyMonStubsMagicCore.NotInImplementation());
        Assert.True(CopyMonStubsMagicCore.CsharpDefaultParam());
    }

    // ===================== 四、插桩 =====================

    [Fact]
    public void NoInstrumentation()
    {
        Assert.True(CopyMonStubsMagicCore.NoInstrumentation());
        Assert.True(CopyMonStubsMagicCore.InstrumentationOnlyInParent());
    }
}

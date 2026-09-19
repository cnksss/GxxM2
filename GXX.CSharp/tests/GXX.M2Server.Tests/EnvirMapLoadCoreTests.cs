using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J164：`TEnvirnoment` 地图文件装载族 1:1 测试。
/// **分支包含关系用枚举验证、极性相反用真值表对照、
/// 整数除法次序与格数清理尺寸错配用显式数值论证 ——
/// 所有计数均来自程序化统计。**
/// </summary>
public sealed class EnvirMapLoadCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void MagicConstants()
    {
        Assert.True(EnvirMapLoadCore.ConstantsPresent());
        Assert.Equal("Map 2010 Ver 1.0", EnvirMapLoadCore.NewMapTitle);
        Assert.Equal(0xAA38, EnvirMapLoadCore.XorWord);
        Assert.Equal(0x8000, EnvirMapLoadCore.BlockedBitBack);
        Assert.Equal(0x80, EnvirMapLoadCore.DoorBit);
        Assert.Equal(0x7F, EnvirMapLoadCore.DoorIndexMask);
        Assert.Equal(10, EnvirMapLoadCore.DoorMergeRadius);
        Assert.Equal(0x2000, EnvirMapLoadCore.BkImgNotSpecial);
        Assert.Equal(6, EnvirMapLoadCore.ReturnMapVersion);
        Assert.Equal(new byte[] { 13, 10 }, EnvirMapLoadCore.TitleTail);
    }

    // ===================== 一、DoLoadMapData =====================

    [Fact]
    public void FiveFieldZeroCheck()
    {
        Assert.True(EnvirMapLoadCore.FiveFieldZeroCheck());
        Assert.True(EnvirMapLoadCore.EquivalentToWholeBlock());
        Assert.True(EnvirMapLoadCore.AllDescZero(new[] { 0, 0, 0, 0, 0 }));
        Assert.False(EnvirMapLoadCore.AllDescZero(new[] { 0, 0, 1, 0, 0 }));
    }

    [Fact]
    public void DescShape()
    {
        Assert.True(EnvirMapLoadCore.DescHasFiveFields());
        Assert.Equal(5, EnvirMapLoadCore.DescFieldCount());
        Assert.True(EnvirMapLoadCore.DescIsTwentyBytes());
    }

    // ---------- 长度校验公式 ----------

    [Fact]
    public void TileInfoSize()
    {
        // **packed：1 字节文件号 + 2 字节瓦片号 = 3**
        Assert.Equal(3, EnvirMapLoadCore.TileInfoSize());
        Assert.True(EnvirMapLoadCore.TileInfoIsThreeBytes());
    }

    [Fact]
    public void MultiplyThenDivideOrder()
    {
        // **必须先乘后除**
        Assert.True(EnvirMapLoadCore.MultiplyThenDivideOrder());
        Assert.True(EnvirMapLoadCore.DivideBeforeMultiplyWouldDiffer());
        Assert.True(EnvirMapLoadCore.DivideFirstDiffersByOne());
    }

    [Fact]
    public void DivideOrderExactValues()
    {
        // w=1,h=2 → 先乘后除 = 1；先除后乘 = 0
        Assert.Equal(1, EnvirMapLoadCore.TileTableTermComputeFirst(1, 2));
        Assert.Equal(0, EnvirMapLoadCore.TileTableTermDivideFirst(1, 2));
        Assert.True(EnvirMapLoadCore.SameWhenDivisibleByFour());
    }

    [Fact]
    public void EIMapLengthFormula()
    {
        Assert.True(EnvirMapLoadCore.EIMapLengthFormula());
        Assert.True(EnvirMapLoadCore.ThreeLengthTerms());
        Assert.Equal(3, EnvirMapLoadCore.LengthFormulaTerms.Length);
    }

    [Fact]
    public void FormulaReusedInMapUnit()
    {
        // **同一公式出现五处**
        Assert.True(EnvirMapLoadCore.FormulaReusedInMapUnit());
        Assert.True(EnvirMapLoadCore.FiveFormulaSites());
        Assert.True(EnvirMapLoadCore.SeekMatchesCheckFormula());
        Assert.Equal(5, EnvirMapLoadCore.FormulaSites.Length);
    }

    // ---------- 句柄判断 ----------

    [Fact]
    public void HandleGreaterThanZero()
    {
        Assert.True(EnvirMapLoadCore.HandleGreaterThanZeroNotNonNegative());
        Assert.True(EnvirMapLoadCore.HandleZeroMistakenForFailure());
        Assert.True(EnvirMapLoadCore.HandleLooksValid(1));
        Assert.False(EnvirMapLoadCore.HandleLooksValid(0));
        Assert.False(EnvirMapLoadCore.HandleLooksValid(-1));
    }

    [Fact]
    public void FailureValueRejected()
    {
        Assert.True(EnvirMapLoadCore.FailureValueRejected());
    }

    [Fact]
    public void SentinelOverlapFamily()
    {
        Assert.True(EnvirMapLoadCore.SameFamilyAsSentinelOverlap());
        Assert.True(EnvirMapLoadCore.TwoSentinelOverlapInstances());
        Assert.Equal(2, EnvirMapLoadCore.SentinelOverlapInstances.Length);
    }

    // ---------- 位置复位 ----------

    [Fact]
    public void SeekSequence()
    {
        Assert.True(EnvirMapLoadCore.SeekToEndThenReset());
        Assert.True(EnvirMapLoadCore.TwoSeekCalls());
        Assert.True(EnvirMapLoadCore.BothBranchesReadFromZero());
        Assert.True(EnvirMapLoadCore.DoLoadIsSoleCaller());
        Assert.Equal(2, EnvirMapLoadCore.SeekCalls.Length);
    }

    // ===================== 二、LoadMapData 三分支 =====================

    [Fact]
    public void ThreeBranches()
    {
        Assert.True(EnvirMapLoadCore.ThreeBranches());
        Assert.True(EnvirMapLoadCore.ThreeBranchNames());
        Assert.Equal(3, EnvirMapLoadCore.BranchNames.Length);
    }

    [Fact]
    public void BranchPredicates()
    {
        Assert.True(EnvirMapLoadCore.IsENMap("Map 2010 Ver 1.0"));
        Assert.False(EnvirMapLoadCore.IsENMap("other"));
        Assert.True(EnvirMapLoadCore.IsReturnMap(13, 10, 6));
        Assert.True(EnvirMapLoadCore.IsNewMap(13, 10));
        Assert.False(EnvirMapLoadCore.IsNewMap(13, 11));
    }

    [Fact]
    public void BranchTwoImpliesBranchThree()
    {
        // **② ⊂ ③（枚举验证）**
        Assert.True(EnvirMapLoadCore.BranchTwoImpliesBranchThree());
        Assert.True(EnvirMapLoadCore.ElseIfOrderMatters());
    }

    [Fact]
    public void BranchThreeIsFallback()
    {
        // **③ 接收"像新地图但版本不是六"**
        Assert.True(EnvirMapLoadCore.BranchThreeIsTheFallback());
        Assert.Equal(2, EnvirMapLoadCore.DispatchBranch("x", 13, 10, 6));
        Assert.Equal(3, EnvirMapLoadCore.DispatchBranch("x", 13, 10, 7));
    }

    [Fact]
    public void VersionOnlySelectsTwoOrThree()
    {
        Assert.True(EnvirMapLoadCore.VersionOnlySelectsTwoOrThree());
        Assert.Equal(3, EnvirMapLoadCore.DispatchBranch("x", 13, 10, 5));
    }

    [Fact]
    public void NoBranchWhenNeither()
    {
        Assert.True(EnvirMapLoadCore.NoBranchWhenNeither());
        Assert.Equal(0, EnvirMapLoadCore.DispatchBranch("x", 0, 0, 6));
    }

    [Fact]
    public void ENTakesPrecedence()
    {
        Assert.True(EnvirMapLoadCore.ENTakesPrecedence());
        Assert.Equal(1, EnvirMapLoadCore.DispatchBranch("Map 2010 Ver 1.0", 13, 10, 6));
    }

    [Fact]
    public void TitleFieldWidths()
    {
        // **string[15] 与 string[16]**
        Assert.True(EnvirMapLoadCore.TitleFieldWidths());
        Assert.True(EnvirMapLoadCore.TailIndicesInRange());
        Assert.True(EnvirMapLoadCore.ENTitleIsWider());
        Assert.True(EnvirMapLoadCore.TwoDifferentTitleWidths());
        Assert.Equal(new[] { 15, 16 }, EnvirMapLoadCore.TitleWidths);
    }

    // ---------- XOR 解密 ----------

    [Fact]
    public void XorDecode()
    {
        Assert.True(EnvirMapLoadCore.ENWidthHeightXor());
        Assert.True(EnvirMapLoadCore.XorIsInvolution());
        Assert.Equal(1234, EnvirMapLoadCore.DecodeXorWidth(1234 ^ 0xAA38));
    }

    [Fact]
    public void XorFieldCounts()
    {
        // **三个字段异或（不是四个）**
        Assert.True(EnvirMapLoadCore.FourCellFieldsXor());
        Assert.True(EnvirMapLoadCore.ThreeXorCellFields());
        Assert.True(EnvirMapLoadCore.BkImgNotNotXored());
        Assert.Equal(3, EnvirMapLoadCore.XorCellFields.Length);
    }

    // ---------- BkImgNot 特殊位 ----------

    [Fact]
    public void BkImgNotSpecial()
    {
        Assert.True(EnvirMapLoadCore.BkImgNotSpecialSetsForceBit());
        Assert.True(EnvirMapLoadCore.BkImgNotSpecialHit());
        Assert.True(EnvirMapLoadCore.BkImgNotSpecialMiss());
    }

    [Fact]
    public void SpecialComparesAfterXor()
    {
        // **先异或再比较；用的是字面量不是常量名**
        Assert.True(EnvirMapLoadCore.SpecialComparesAfterXor());
        Assert.True(EnvirMapLoadCore.UsesLiteralNotConstantName());
        Assert.True(EnvirMapLoadCore.LiteralEqualsConstant());
    }

    // ---------- chFlag ----------

    [Fact]
    public void FlagValues()
    {
        Assert.Equal(1, EnvirMapLoadCore.FlagForBackBlocked());
        Assert.Equal(2, EnvirMapLoadCore.FlagForFrontBlocked());
        Assert.True(EnvirMapLoadCore.TwoDistinctBlockedFlags());
    }

    [Fact]
    public void FrontOverwritesBack()
    {
        // **前景后判、覆盖背景**
        Assert.True(EnvirMapLoadCore.FrontOverwritesBack());
        Assert.True(EnvirMapLoadCore.BothBlockedGivesTwo());
        Assert.Equal(2, EnvirMapLoadCore.ResolveFlag(true, true));
        Assert.Equal(1, EnvirMapLoadCore.ResolveFlag(true, false));
        Assert.Equal(2, EnvirMapLoadCore.ResolveFlag(false, true));
        Assert.Equal(0, EnvirMapLoadCore.ResolveFlag(false, false));
    }

    [Fact]
    public void Passability()
    {
        // **规则是"零可走"，所以 1 与 2 都阻挡**
        Assert.True(EnvirMapLoadCore.FlagOneIsAlsoBlocking());
        Assert.True(EnvirMapLoadCore.ThreeFlagStates());
        Assert.True(EnvirMapLoadCore.IsPassable(0));
        Assert.False(EnvirMapLoadCore.IsPassable(1));
        Assert.False(EnvirMapLoadCore.IsPassable(2));
    }

    [Fact]
    public void Sub4B5FC8MissesFlagOne()
    {
        // **sub_4B5FC8 只认 2、漏掉 1**
        Assert.True(EnvirMapLoadCore.TwoIsTheBlockedValueElsewhere());
        Assert.True(EnvirMapLoadCore.Sub4B5FC8MissesFlagOne());
        Assert.True(EnvirMapLoadCore.BlockedCheckMissesOne());
    }

    [Fact]
    public void FlagCounts()
    {
        // **程序化统计：读 16 与 1、写 5/5/2**
        Assert.Equal(16, EnvirMapLoadCore.ChFlagZeroReadCount());
        Assert.Equal(1, EnvirMapLoadCore.ChFlagTwoReadCount());
        Assert.True(EnvirMapLoadCore.SixteenToOneRatio());
        Assert.Equal(5, EnvirMapLoadCore.ChFlagOneWriteCount());
        Assert.Equal(5, EnvirMapLoadCore.ChFlagTwoWriteCount());
        Assert.Equal(2, EnvirMapLoadCore.ChFlagZeroWriteCount());
    }

    [Fact]
    public void FlagWriteTotals()
    {
        Assert.True(EnvirMapLoadCore.WritesOneEqualsWritesTwo());
        Assert.True(EnvirMapLoadCore.ZeroWritesAreTwo());
        Assert.True(EnvirMapLoadCore.TwelveTotalWrites());
        Assert.Equal(12, EnvirMapLoadCore.TotalFlagWrites());
    }

    // ---------- 门 ----------

    [Fact]
    public void DoorBitAndNumber()
    {
        Assert.True(EnvirMapLoadCore.HasDoorBit(0x80));
        Assert.False(EnvirMapLoadCore.HasDoorBit(0x7F));
        Assert.Equal(5, EnvirMapLoadCore.DoorNumber(0x85));
        Assert.Equal(127, EnvirMapLoadCore.DoorNumber(0xFF));
    }

    [Fact]
    public void ValidDoorNumber()
    {
        // **编号必须大于零**
        Assert.True(EnvirMapLoadCore.DoorNumberMustBePositive());
        Assert.True(EnvirMapLoadCore.DoorNumberZeroRejected());
        Assert.False(EnvirMapLoadCore.ValidDoorNumber(0x80));
        Assert.True(EnvirMapLoadCore.ValidDoorNumber(0x81));
        Assert.True(EnvirMapLoadCore.DoorNumberRange());
    }

    [Fact]
    public void MergeRadius()
    {
        // **半径十、切比雪夫、双向**
        Assert.True(EnvirMapLoadCore.MergeRadiusIsTen());
        Assert.True(EnvirMapLoadCore.MergeIsSymmetric());
        Assert.True(EnvirMapLoadCore.WithinMergeRadius(0, 0, 10, 10));
        Assert.False(EnvirMapLoadCore.WithinMergeRadius(0, 0, 11, 0));
        Assert.False(EnvirMapLoadCore.WithinMergeRadius(0, 0, 0, 11));
    }

    [Fact]
    public void NestedRadiusChecks()
    {
        Assert.True(EnvirMapLoadCore.NestedRadiusChecks());
        Assert.True(EnvirMapLoadCore.ThirdConditionIsDoorNumber());
    }

    [Fact]
    public void ReuseSemantics()
    {
        // **找到时复用状态并增加引用计数**
        Assert.True(EnvirMapLoadCore.ReuseIncrementsRefCount());
        Assert.True(EnvirMapLoadCore.ReuseDoesNotCreateStatus());

        var doors = new List<(int X, int Y, int N08)> { (5, 5, 3) };
        var (idx, rc) = EnvirMapLoadCore.Reuse(doors, 6, 6, 3, 4);

        Assert.Equal(0, idx);
        Assert.Equal(5, rc);
    }

    [Fact]
    public void ReuseMissCreatesStatus()
    {
        var doors = new List<(int X, int Y, int N08)> { (5, 5, 3) };
        var (idx, _) = EnvirMapLoadCore.Reuse(doors, 99, 99, 3, 4);

        Assert.Null(idx);
        Assert.True(EnvirMapLoadCore.NoMatchCreatesStatus());
    }

    [Fact]
    public void NewStatusInitialValues()
    {
        // **四个零 + 引用计数一**
        Assert.True(EnvirMapLoadCore.FourZerosAndOneRefCount());
        Assert.Equal(5, EnvirMapLoadCore.NewStatusInitialValues.Length);
        Assert.Equal(1, EnvirMapLoadCore.NewStatusInitialValues[4].Value);
    }

    [Fact]
    public void DoorAtCurrentCell()
    {
        Assert.True(EnvirMapLoadCore.DoorAtCurrentCell());
        Assert.True(EnvirMapLoadCore.N08IsDoorNumber());
        Assert.True(EnvirMapLoadCore.OffsetNamedFieldsSet());
    }

    // ---------- 四份重复门块 ----------

    [Fact]
    public void DoorBlockCopies()
    {
        // **共四份：三个分支 + EI**
        Assert.True(EnvirMapLoadCore.ThreeIdenticalDoorBlocks());
        Assert.True(EnvirMapLoadCore.ThreeDoorBlockCopies());
        Assert.True(EnvirMapLoadCore.FourDoorBlockCopies());
        Assert.Equal(4, EnvirMapLoadCore.DoorBlockCopyCount());
        Assert.Equal(3, EnvirMapLoadCore.DoorBlockSites.Length);
    }

    [Fact]
    public void DoorSourceFields()
    {
        // **四份只差"门索引取哪个字段"**
        Assert.True(EnvirMapLoadCore.CopiesDifferOnlyBySourceField());
        Assert.True(EnvirMapLoadCore.FourDoorSourceFields());
        Assert.True(EnvirMapLoadCore.FourthIsAbbreviated());
        Assert.True(EnvirMapLoadCore.IdxVersusIndexSpelling());
        Assert.Equal(4, EnvirMapLoadCore.DoorSourceFields.Length);
    }

    // ---------- 初始化与清理 ----------

    [Fact]
    public void Initialization()
    {
        Assert.True(EnvirMapLoadCore.InitializesBeforeBranching());
        Assert.True(EnvirMapLoadCore.EILoaderDoesNotInitialize());
    }

    [Fact]
    public void EISizeGuard()
    {
        // **宽高都必须大于一**
        Assert.True(EnvirMapLoadCore.EILoaderRequiresSizeAboveOne());
        Assert.True(EnvirMapLoadCore.EISizeGuardBoundary());
        Assert.True(EnvirMapLoadCore.EISizeGuardRejectsZeroAndNegative());
        Assert.False(EnvirMapLoadCore.EISizeGuard(1, 1));
        Assert.True(EnvirMapLoadCore.EISizeGuard(2, 2));
    }

    [Fact]
    public void GuardIsAboveOne()
    {
        // **是"大于一"不是"大于零"**
        Assert.True(EnvirMapLoadCore.GuardIsAboveOneNotAboveZero());
        Assert.True(EnvirMapLoadCore.OtherLoadersHaveNoSizeGuard());
        Assert.True(EnvirMapLoadCore.OnlyEIHasGuard());
        Assert.Equal(new[] { true, false, false }, EnvirMapLoadCore.SizeGuardPresence);
    }

    [Fact]
    public void CleanupSteps()
    {
        Assert.True(EnvirMapLoadCore.EILoaderFreesOldCellsFirst());
        Assert.True(EnvirMapLoadCore.ThreeCleanupSteps());
        Assert.True(EnvirMapLoadCore.CleanupOnlyWhenArrayNotNull());
        Assert.True(EnvirMapLoadCore.CleanupWalksAllCells());
        Assert.Equal(3, EnvirMapLoadCore.EICleanupSteps.Length);
    }

    [Fact]
    public void CleanupCellCount()
    {
        Assert.True(EnvirMapLoadCore.CleanupCellCountValues());
        Assert.Equal(9, EnvirMapLoadCore.CleanupCellCount(3, 3));
        Assert.Equal(0, EnvirMapLoadCore.CleanupCellCount(0, 5));
    }

    [Fact]
    public void CleanupUsesNewDimensionsBug()
    {
        // **清理用的是新宽高 —— 一处真实缺陷**
        Assert.True(EnvirMapLoadCore.CleanupUsesNewDimensionsBug());
        Assert.True(EnvirMapLoadCore.CleanupDimensionMismatch());
        Assert.True(EnvirMapLoadCore.TwoMismatchFailureModes());
        Assert.True(EnvirMapLoadCore.TwoMismatchModes());
        Assert.True(EnvirMapLoadCore.OrderCausesTheDefect());
        Assert.Equal(2, EnvirMapLoadCore.MismatchFailureModes.Length);
    }

    // ---------- EI 的相反极性 ----------

    [Fact]
    public void EIFlagLogicIsInverted()
    {
        // **EI：位零为零表示阻挡**
        Assert.True(EnvirMapLoadCore.EIFlagLogicIsInverted());
        Assert.True(EnvirMapLoadCore.EIZeroBitMeansBlocked());
        Assert.Equal(1, EnvirMapLoadCore.EIFlag(0));
        Assert.Equal(1, EnvirMapLoadCore.EIFlag(2));
        Assert.Equal(0, EnvirMapLoadCore.EIFlag(1));
        Assert.Equal(0, EnvirMapLoadCore.EIFlag(3));
    }

    [Fact]
    public void OppositePolarity()
    {
        // **两者极性相反**
        Assert.True(EnvirMapLoadCore.StandardIsBitSetMeansBlocked());
        Assert.True(EnvirMapLoadCore.OppositePolarity());
    }

    [Fact]
    public void EIZeroWrite()
    {
        // **EI 是唯一会写零的装载器**
        Assert.True(EnvirMapLoadCore.EIOnlyWriterOfZeroFlag());
        Assert.Equal(1, EnvirMapLoadCore.WritersOfZeroFlag());
        Assert.Equal(4, EnvirMapLoadCore.WritersOfOneOrTwo());
        Assert.True(EnvirMapLoadCore.OneVersusFour());
    }

    [Fact]
    public void EIZeroWriteIsRedundant()
    {
        // **AllocMem 已清零，那次写零冗余**
        Assert.True(EnvirMapLoadCore.EIZeroWriteIsExplicitPassable());
        Assert.True(EnvirMapLoadCore.EIZeroWriteIsRedundant());
        Assert.True(EnvirMapLoadCore.ZeroWriteRedundantAfterAllocMem());
        Assert.True(EnvirMapLoadCore.OtherLoadersRelyOnZeroInit());
    }

    // ---------- 地址注释 ----------

    [Fact]
    public void EIAddressComments()
    {
        Assert.True(EnvirMapLoadCore.EIBranchHasAddressComments());
        Assert.True(EnvirMapLoadCore.TwoEIAddressComments());
        Assert.True(EnvirMapLoadCore.SameFamilyAsSub4B5FC8());
        Assert.True(EnvirMapLoadCore.FourAddressInSourceItems());
        Assert.Equal(new[] { "// 004B5601", "// 004B562C" }, EnvirMapLoadCore.EIAddressComments);
    }

    [Fact]
    public void AddressInSourceFamily()
    {
        Assert.Equal(4, EnvirMapLoadCore.AddressInSourceFamily.Length);
    }

    // ---------- 读取返回值被忽略 ----------

    [Fact]
    public void UncheckedFileOperations()
    {
        // **程序化统计：9 次读、5 次移位**
        Assert.True(EnvirMapLoadCore.FileReadReturnIgnored());
        Assert.Equal(9, EnvirMapLoadCore.UncheckedReadCount());
        Assert.Equal(5, EnvirMapLoadCore.UncheckedSeekCount());
        Assert.True(EnvirMapLoadCore.NineUncheckedReads());
        Assert.True(EnvirMapLoadCore.FiveUncheckedSeeks());
        Assert.True(EnvirMapLoadCore.FourteenUncheckedOps());
    }

    [Fact]
    public void FailedReadLeavesZeros()
    {
        // **失败时缓冲区是清零内容；EN 地图会得到荒谬宽高**
        Assert.True(EnvirMapLoadCore.FailedReadLeavesZeros());
        Assert.True(EnvirMapLoadCore.FailedENReadStillWritesFlags());
        Assert.Equal(0xAA38, EnvirMapLoadCore.ZeroWidthAfterXor());
        Assert.True(EnvirMapLoadCore.ZeroXorsToNonZero());
        Assert.True(EnvirMapLoadCore.FailureProducesAbsurdDimensions());
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 35, 237, 136 }, EnvirMapLoadCore.MethodLineCounts);
        Assert.True(EnvirMapLoadCore.ThreeMethods());
    }

    [Fact]
    public void LineExtremes()
    {
        Assert.True(EnvirMapLoadCore.LoadMapDataIsLongest());
        Assert.True(EnvirMapLoadCore.DoLoadIsShortest());
        Assert.True(EnvirMapLoadCore.LoadMapDataExceedsRest());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirMapLoadCore.TotalLinesValues());
        Assert.Equal(408, EnvirMapLoadCore.TotalLines());
    }

    [Fact]
    public void DuplicationShare()
    {
        Assert.True(EnvirMapLoadCore.DuplicationShareIsLarge());
        Assert.Equal(105, EnvirMapLoadCore.ApproximatelyThreeDoorBlocks());
        Assert.True(EnvirMapLoadCore.DoorBlocksAreFortyFourPercent());
    }
}

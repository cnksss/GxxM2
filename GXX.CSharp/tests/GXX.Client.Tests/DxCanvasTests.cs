using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DxCanvas.pas 1-1246 的 1:1 测试。
/// 覆盖：
///   * 颜色查找表（g_Grays 的初始化、ColorTable_565 的悬空声明）
///   * CheckTextureAlpha（只判是否为 0）
///   * ConvertLine16 / ConvertBrightLine16 / ConvertGrayLine16（16bpp 目标）
///   * ConvertLine32 / _8 / _16 / _24 / _32（32bpp 目标，含 Alpha 覆盖高 8 位）
///   * ConvertBrightLine32（×1.3 + MIN(255)）/ ConvertGrayLine32（g_Grays[R+G+B]）
///   * Pack565 / DelphiRound（银行家舍入）
///   * ComputeCopyRegion / CopyTexture（含原文的 srcPitch * I 与 srcLeft 笔误）
///   * NewTexture / NewTextureGray / NewTextureBright / NewTextureFromGraphic 的行调度
///   * MeasureTextTexture / GetTextTexture 的几何
/// </summary>
public sealed class DxCanvasTests
{
    // ---------------------------------------------------------------
    // 测试替身
    // ---------------------------------------------------------------

    /// <summary>可编程的 TextureFactory（记下每次 Create 的请求并返回自建缓冲）。</summary>
    private sealed class FakeFactory : DxCanvas.IDxTextureFactory
    {
        public readonly List<(int w, int h)> Created = new();
        public readonly List<int> DdsFormats = new();
        public byte[] LastLoadData;
        public bool ReturnNullOnCreate;
        public bool ReturnNullOnLoad;
        public bool ReturnNullOnDds;
        /// <summary>DDS 尝试中第几次成功（1-based）；0 表示全部失败。</summary>
        public int DdsSucceedAtAttempt;

        public TDxTextureBuffer Create(int width, int height)
        {
            Created.Add((width, height));
            return ReturnNullOnCreate ? null : new TDxTextureBuffer(width, height);
        }

        public TDxTextureBuffer Load(byte[] fileData)
        {
            LastLoadData = fileData;
            return ReturnNullOnLoad ? null : new TDxTextureBuffer(1, 1);
        }

        public TDxTextureBuffer LoadDds(int d3dFmt, byte[] fileData)
        {
            DdsFormats.Add(d3dFmt);
            if (ReturnNullOnDds) return null;
            return DdsFormats.Count == DdsSucceedAtAttempt ? new TDxTextureBuffer(2, 2) : null;
        }
    }

    /// <summary>最小 IDxDib 实现。</summary>
    private sealed class FakeDib : DxCanvas.IDxDib
    {
        private readonly byte[][] _rows;
        public int Width { get; }
        public int Height { get; }
        public byte BitCount { get; }
        public byte[] ColorTable { get; } = new byte[256 * 4];

        public FakeDib(int width, int height, byte bitCount)
        {
            Width = width;
            Height = height;
            BitCount = bitCount;
            int bytesPerRow = bitCount switch
            {
                8 => width,
                16 => width * 2,
                24 => width * 3,
                32 => width * 4,
                _ => width,
            };
            _rows = new byte[height][];
            for (int y = 0; y < height; y++) _rows[y] = new byte[bytesPerRow];
        }

        public byte[] Row(int y) => _rows[y];
        public byte[] GetScanLine(int y) => _rows[y];
    }

    private static FakeFactory InstallFactory()
    {
        var f = new FakeFactory();
        DxCanvas.TextureFactory = f;
        return f;
    }

    private static byte[] Buf(int size) => new byte[size];

    // =====================================================================
    // 颜色查找表（45-64 / GameImages 1703-1716）
    // =====================================================================

    /// <summary>用例 1：g_Grays 的长度与首尾（0..767），由 InitGrays 的三连写生成。</summary>
    [Fact]
    public void GraysTable_Has768EntriesAndTripledPattern()
    {
        Assert.Equal(768, TDxCanvasColorTables.g_Grays.Length);
        Assert.Equal(0, TDxCanvasColorTables.g_Grays[0]);
        Assert.Equal(0, TDxCanvasColorTables.g_Grays[1]);
        Assert.Equal(0, TDxCanvasColorTables.g_Grays[2]);
        Assert.Equal(1, TDxCanvasColorTables.g_Grays[3]);
        Assert.Equal(255, TDxCanvasColorTables.g_Grays[765]);
        Assert.Equal(255, TDxCanvasColorTables.g_Grays[766]);
        Assert.Equal(255, TDxCanvasColorTables.g_Grays[767]);
    }

    /// <summary>用例 2（差异断言）：g_Grays[i] = i div 3 —— 用于 R+G+B 的灰度化。</summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 1)]
    [InlineData(300, 100)]
    [InlineData(765, 255)]
    public void GraysTable_EqualsIndexDividedByThree(int index, int expected)
    {
        Assert.Equal(expected, TDxCanvasColorTables.g_Grays[index]);
    }

    /// <summary>用例 3：ColorTable_565 是原文的**悬空声明**（256 项，全 0，从未被赋值或读取）。</summary>
    [Fact]
    public void ColorTable565_IsDanglingAllZero()
    {
        Assert.Equal(256, TDxCanvasColorTables.ColorTable_565.Length);
        Assert.All(TDxCanvasColorTables.ColorTable_565, v => Assert.Equal((ushort)0, v));
    }

    /// <summary>用例 4：各查找表的尺寸与原文声明一致。</summary>
    [Fact]
    public void LookupTableSizes_MatchDelphiDeclarations()
    {
        Assert.Equal(256, TDxCanvasColorTables.g_DefColorTable.GetLength(0));
        Assert.Equal(4, TDxCanvasColorTables.g_DefColorTable.GetLength(1));
        Assert.Equal(256, TDxCanvasColorTables.ColorTable_8_16Bit.Length);
        Assert.Equal(256, TDxCanvasColorTables.ColorTableBright_8_16Bit.Length);
        Assert.Equal(256, TDxCanvasColorTables.ColorTableGray_8_16Bit.Length);
        Assert.Equal(256, TDxCanvasColorTables.ColorTable_8_32Bit.Length);
        Assert.Equal(256, TDxCanvasColorTables.ColorTableBright_8_32Bit.Length);
        Assert.Equal(256, TDxCanvasColorTables.ColorTableGray_8_32Bit.Length);
        Assert.Equal(65536, TDxCanvasColorTables.ColorTable_16_32Bit.Length);
        Assert.Equal(65536, TDxCanvasColorTables.ColorTableBright_16_32Bit.Length);
        Assert.Equal(65536, TDxCanvasColorTables.ColorTableGray_16_32Bit.Length);
        Assert.Equal(65536, TDxCanvasColorTables.ColorTableBright_16.Length);
        Assert.Equal(65536, TDxCanvasColorTables.ColorTableGray_16.Length);
        Assert.Equal(32768, DxCanvas.MaxPixelCount);
    }

    /// <summary>用例 5：LoadDefColorTable 接缝把 256×4 调色板灌进 g_DefColorTable。</summary>
    [Fact]
    public void LoadDefColorTable_CopiesQuads()
    {
        TDxCanvasColorTables.Clear();
        var pal = new byte[256 * 4];
        pal[0] = 0x11; pal[1] = 0x22; pal[2] = 0x33; pal[3] = 0xFF;   // B,G,R,Reserved
        TDxCanvasColorTables.LoadDefColorTable(pal);

        Assert.Equal(0x11, TDxCanvasColorTables.g_DefColorTable[0, TDxCanvasColorTables.BlueOfs]);
        Assert.Equal(0x22, TDxCanvasColorTables.g_DefColorTable[0, TDxCanvasColorTables.GreenOfs]);
        Assert.Equal(0x33, TDxCanvasColorTables.g_DefColorTable[0, TDxCanvasColorTables.RedOfs]);
        Assert.Equal(0xFF, TDxCanvasColorTables.g_DefColorTable[0, TDxCanvasColorTables.ReservedOfs]);
    }

    // =====================================================================
    // Pack565 / DelphiRound（250/262/1190 等）
    // =====================================================================

    /// <summary>用例 6：Pack565 = `(R shl 8 and $F800) or (G shl 3 and $07E0) or (B shr 3 and $001F)`。</summary>
    [Theory]
    [InlineData(0, 0, 0, 0x0000)]
    [InlineData(255, 255, 255, 0xFFFF)]
    [InlineData(255, 0, 0, 0xF800)]
    [InlineData(0, 255, 0, 0x07E0)]
    [InlineData(0, 0, 255, 0x001F)]
    [InlineData(128, 0, 0, 0x8000)]      // 128 shl 8 = 0x8000，与 0xF800 相与 → 0x8000
    [InlineData(0, 4, 0, 0x0020)]        // 4 shl 3 = 0x20
    public void Pack565_Formula(byte r, byte g, byte b, int expected)
    {
        Assert.Equal((ushort)expected, DxCanvas.Pack565(r, g, b));
    }

    /// <summary>用例 7（差异断言）：Pack565 在 R shl 8 时按 16 位截断 —— R=0xFF 得 0xF800（低 3 位丢弃）。</summary>
    [Fact]
    public void Pack565_TruncatesLowRedBits()
    {
        Assert.Equal((ushort)0xF800, DxCanvas.Pack565(0xFF, 0, 0));
        Assert.Equal((ushort)0xF800, DxCanvas.Pack565(0xF8, 0, 0));
        Assert.Equal((ushort)0xF800, DxCanvas.Pack565(0xFF, 0, 0) == DxCanvas.Pack565(0xF8, 0, 0)
            ? (ushort)0xF800 : (ushort)0);
    }

    /// <summary>用例 8：DelphiRound 是银行家舍入（.5 → 偶数）。</summary>
    [Theory]
    [InlineData(0.5, 0)]
    [InlineData(1.5, 2)]
    [InlineData(2.5, 2)]
    [InlineData(3.5, 4)]
    [InlineData(-0.5, 0)]
    [InlineData(-1.5, -2)]
    [InlineData(-2.5, -2)]
    [InlineData(2.6, 3)]
    [InlineData(-100.6, -101)]
    public void DelphiRound_UsesBankersRounding(double value, int expected)
    {
        Assert.Equal(expected, TDxCanvasColorTables.DelphiRound(value));
    }

    // =====================================================================
    // CheckTextureAlpha（72-81）
    // =====================================================================

    /// <summary>用例 9：CheckTextureAlpha 只判「像素 != 0」。</summary>
    [Fact]
    public void CheckTextureAlpha_OnlyTestsNonZero()
    {
        var t = new TDxTextureBuffer(2, 2);
        t.SetPixel(0, 0, 0x00000000);
        t.SetPixel(1, 0, 0x00FFFFFF);   // alpha 为 0 但有颜色 → 仍算「有像素」
        t.SetPixel(0, 1, 0xFF000000);   // 只有 alpha
        t.SetPixel(1, 1, 0xFFFFFFFF);

        Assert.False(DxCanvas.CheckTextureAlpha(t, 0, 0));
        Assert.True(DxCanvas.CheckTextureAlpha(t, 1, 0));
        Assert.True(DxCanvas.CheckTextureAlpha(t, 0, 1));
        Assert.True(DxCanvas.CheckTextureAlpha(t, 1, 1));
    }

    /// <summary>用例 10（差异断言）：注释掉的 `(Pix &lt;&gt; $FF000000)` 条件**不参与**判定 —— 纯 alpha 像素算命中。</summary>
    [Fact]
    public void CheckTextureAlpha_IgnoresTheCommentedOutAlphaCondition()
    {
        var t = new TDxTextureBuffer(1, 1);
        t.SetPixel(0, 0, 0xFF000000);
        Assert.True(DxCanvas.CheckTextureAlpha(t, 0, 0));   // 若带上注释条件应为 False
    }

    /// <summary>用例 11：Source 为 nil → False（Assigned 守卫）。</summary>
    [Fact]
    public void CheckTextureAlpha_NullSourceIsFalse()
    {
        Assert.False(DxCanvas.CheckTextureAlpha(null, 0, 0));
    }

    // =====================================================================
    // ConvertLine16（136-173）
    // =====================================================================

    /// <summary>用例 12：8bpp → 查 ColorTable_8_16Bit 写 16 位。</summary>
    [Fact]
    public void ConvertLine16_8BitUsesLookup()
    {
        var des = Buf(16);

        DxCanvas.ConvertLine16(new byte[] { 0, 3, 1 }, 0, des, 2, 3, 8, null);

        // 3 号索引没设过表 → 0（静态表不做全局清空，测试之间不互相污染）
        Assert.Equal(0, des[2]); Assert.Equal(0, des[3]);
        Assert.Equal(0, des[4]); Assert.Equal(0, des[5]);
    }

    /// <summary>用例 12b：手工写好查找表后按索引取 16 位值（大小端为 Delphi 的 PWord 语义）。</summary>
    [Fact]
    public void ConvertLine16_8BitWritesLookupValueLittleEndian()
    {
        TDxCanvasColorTables.ColorTable_8_16Bit[3] = 0xBEEF;
        var des = Buf(4);

        DxCanvas.ConvertLine16(new byte[] { 3, 3 }, 0, des, 0, 2, 8, null);

        Assert.Equal(0xEF, des[0]);
        Assert.Equal(0xBE, des[1]);
        Assert.Equal(0xEF, des[2]);
        Assert.Equal(0xBE, des[3]);

        TDxCanvasColorTables.ColorTable_8_16Bit[3] = 0;   // 还原
    }

    /// <summary>用例 13：16bpp 走整段字节搬运（Move(SrcP^, DesP^, Width*2)）。</summary>
    [Fact]
    public void ConvertLine16_16BitIsRawCopy()
    {
        var src = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
        var des = Buf(8);

        DxCanvas.ConvertLine16(src, 0, des, 1, 3, 16, null);

        Assert.Equal(0, des[0]);
        Assert.Equal(0x11, des[1]); Assert.Equal(0x22, des[2]);
        Assert.Equal(0x33, des[3]); Assert.Equal(0x44, des[4]);
        Assert.Equal(0x55, des[5]); Assert.Equal(0x66, des[6]);
        Assert.Equal(0, des[7]);
    }

    /// <summary>用例 14（差异断言）：24bpp 与 32bpp 分支在原文里**整段注释掉** —— 目标缓冲原样不动。</summary>
    [Theory]
    [InlineData(24)]
    [InlineData(32)]
    public void ConvertLine16_24And32BitBranchesDoNothing(byte bitCount)
    {
        var src = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var des = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA };

        DxCanvas.ConvertLine16(src, 0, des, 0, 2, bitCount, null);

        Assert.All(des, b => Assert.Equal(0xAA, b));
    }

    /// <summary>用例 15：其它 BitCount（0/1/64）不进任何分支 → 同样不做任何事。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(64)]
    public void ConvertLine16_UnknownBitCountDoesNothing(byte bitCount)
    {
        var des = new byte[] { 0xAA, 0xAA };
        DxCanvas.ConvertLine16(new byte[] { 1, 2 }, 0, des, 0, 2, bitCount, null);
        Assert.All(des, b => Assert.Equal(0xAA, b));
    }

    // =====================================================================
    // ConvertGrayLine16（221-268）
    // =====================================================================

    /// <summary>用例 16：24bpp → 16bpp 灰度：源非黑时写 Pack565(gray,gray,gray)。</summary>
    [Fact]
    public void ConvertGrayLine16_24BitPacksGray565()
    {
        // 3 个像素：黑 / (10,0,0) / (0,0,3)
        var src = new byte[]
        {
            0, 0, 0,          // B,G,R = 黑
            0, 0, 10,         // R=10
            3, 0, 0,          // B=3
        };
        var des = Buf(6);

        DxCanvas.ConvertGrayLine16(src, 0, des, 0, 3, 24, null);

        // 第 1 个像素是黑的 → 原文不写目标、desP 不前进 → 第 2 个像素写在偏移 0
        int gray10 = TDxCanvasColorTables.g_Grays[10];
        int gray3 = TDxCanvasColorTables.g_Grays[3];
        Assert.Equal(DxCanvas.Pack565((byte)gray10, (byte)gray10, (byte)gray10),
            (ushort)(des[0] | (des[1] << 8)));
        Assert.Equal(DxCanvas.Pack565((byte)gray3, (byte)gray3, (byte)gray3),
            (ushort)(des[2] | (des[3] << 8)));
    }

    /// <summary>
    /// 用例 17（差异断言）：32bpp 灰度走 `g_Grays[R+G+B]`（注意是**三个通道之和**而不是平均值）——
    /// 全黑像素同样被跳过，故目标缓冲的写入位置会前移。
    /// </summary>
    [Fact]
    public void ConvertGrayLine16_32BitUsesChannelSum()
    {
        var src = new byte[]
        {
            0, 0, 0, 0,          // 黑（B,G,R,Res）
            0, 0, 30, 0,         // R=30（B=0,G=0）
        };
        var des = Buf(4);

        DxCanvas.ConvertGrayLine16(src, 0, des, 0, 2, 32, null);

        int expected = TDxCanvasColorTables.g_Grays[30];
        ushort packed = (ushort)(des[0] | (des[1] << 8));
        Assert.Equal(DxCanvas.Pack565((byte)expected, (byte)expected, (byte)expected), packed);
        Assert.Equal(0, des[2]);
        Assert.Equal(0, des[3]);
    }

    /// <summary>用例 18：8bpp 与 16bpp 灰度查表。</summary>
    [Fact]
    public void ConvertGrayLine16_8And16BitUseLookup()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTableGray_8_16Bit[5] = 0x1234;
        TDxCanvasColorTables.ColorTableGray_16[0x0201] = 0x5678;

        var des8 = Buf(2);
        DxCanvas.ConvertGrayLine16(new byte[] { 5 }, 0, des8, 0, 1, 8, null);
        Assert.Equal(0x1234, des8[0] | (des8[1] << 8));

        var des16 = Buf(2);
        DxCanvas.ConvertGrayLine16(new byte[] { 0x01, 0x02 }, 0, des16, 0, 1, 16, null);
        Assert.Equal(0x5678, des16[0] | (des16[1] << 8));
    }

    // =====================================================================
    // ConvertLine32_8 / _16 / _24 / _32（413-551）
    // =====================================================================

    /// <summary>用例 19：ConvertLine32_8 无 Alpha → 直接写查表值。</summary>
    [Fact]
    public void ConvertLine32_8_WithoutAlpha()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTable_8_32Bit[2] = 0xFF112233;
        var des = Buf(8);

        DxCanvas.ConvertLine32_8(new byte[] { 2, 2 }, 0, des, 0, 2, null, null, 0);

        Assert.Equal(0xFF112233u, ReadU32(des, 0));
        Assert.Equal(0xFF112233u, ReadU32(des, 4));
    }

    /// <summary>
    /// 用例 20（差异断言）：ConvertLine32_8 带 Alpha → **无条件**用 Alpha^ 覆盖高 8 位
    /// （哪怕查表值本身是 0）。
    /// </summary>
    [Fact]
    public void ConvertLine32_8_WithAlphaOverwritesHighByte()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTable_8_32Bit[0] = 0x00000000;
        TDxCanvasColorTables.ColorTable_8_32Bit[1] = 0xFF00FF00;
        var des = Buf(8);

        DxCanvas.ConvertLine32_8(new byte[] { 0, 1 }, 0, des, 0, 2, null, new byte[] { 0x40, 0x80 }, 0);

        Assert.Equal(0x40000000u, ReadU32(des, 0));   // 全 0 也被写上 alpha
        Assert.Equal(0x8000FF00u, ReadU32(des, 4));
    }

    /// <summary>用例 21：ConvertLine32_16 查 565 → 32bit 表。</summary>
    [Fact]
    public void ConvertLine32_16_Lookup()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTable_16_32Bit[0x001F] = 0xFF0000FF;
        var des = Buf(4);

        DxCanvas.ConvertLine32_16(new byte[] { 0x1F, 0x00 }, 0, des, 0, 1, null, null, 0);

        Assert.Equal(0xFF0000FFu, ReadU32(des, 0));
    }

    /// <summary>
    /// 用例 22（差异断言）：ConvertLine32_24 的「源像素全黑」**不写颜色**，
    /// 但带 Alpha 时仍会读回目标旧值并覆盖 alpha —— 即黑像素只剩 alpha。
    /// 字节布局与原文 TRGBQuad 一致：低字节 = B。
    /// </summary>
    [Fact]
    public void ConvertLine32_24_BlackPixelLeavesColorUntouchedButTakesAlpha()
    {
        var src = new byte[] { 1, 2, 3, 0, 0, 0 };      // 像素0 = (B1,G2,R3)，像素1 = 黑
        var des = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xAA, 0xBB, 0xCC, 0xDD };

        DxCanvas.ConvertLine32_24(src, 0, des, 0, 2, null, new byte[] { 0x11, 0x22 }, 0);

        // 像素0：Res=0xFF 被 alpha=0x11 覆盖 → 0xAABBGGRR = 0x11030201
        Assert.Equal(0x11030201u, ReadU32(des, 0));
        // 像素1：颜色保持原值 0xDDCCBBAA 的低 24 位，仅把 Res 换成 0x22
        Assert.Equal(0x22CCBBAAu, ReadU32(des, 4));
    }

    /// <summary>用例 22b：不带 Alpha 时全黑像素完全不写（目标保持原值）。</summary>
    [Fact]
    public void ConvertLine32_24_WithoutAlphaBlackPixelUntouched()
    {
        var src = new byte[] { 0, 0, 0 };
        var des = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };

        DxCanvas.ConvertLine32_24(src, 0, des, 0, 1, null, null, 0);

        Assert.Equal(0xDDCCBBAAu, ReadU32(des, 0));
    }

    /// <summary>用例 23：ConvertLine32_32 源为 0 时目标保持原值；源非 0 时 `or $FF000000`。</summary>
    [Fact]
    public void ConvertLine32_32_SourceZeroLeavesTarget()
    {
        var src = new byte[] { 0, 0, 0, 0, 0x11, 0x22, 0x33, 0x00 };
        var des = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };

        DxCanvas.ConvertLine32_32(src, 0, des, 0, 2, null, null, 0);

        Assert.Equal(0xAAAAAAAAu, ReadU32(des, 0));      // 源 0 → 不动
        Assert.Equal(0xFF332211u, ReadU32(des, 4));      // 源 & 0x00FFFFFF | 0xFF000000
    }

    /// <summary>用例 24：ConvertLine32 总入口按 BitCount 分派（Alpha=nil 分支）。</summary>
    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)16)]
    [InlineData((byte)24)]
    [InlineData((byte)32)]
    public void ConvertLine32_DispatchesOnBitCount(byte bitCount)
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTable_8_32Bit[7] = 0xFF070707;
        TDxCanvasColorTables.ColorTable_16_32Bit[7] = 0xFF161616;

        byte[] src = bitCount switch
        {
            8 => new byte[] { 7 },
            16 => new byte[] { 7, 0 },
            24 => new byte[] { 7, 7, 7 },
            _ => new byte[] { 7, 7, 7, 7 },
        };
        var des = Buf(4);

        DxCanvas.ConvertLine32(src, 0, des, 0, 1, bitCount, null, null, 0);

        Assert.NotEqual(0u, ReadU32(des, 0));
    }

    /// <summary>用例 25：ConvertLine32 的 Alpha 分支在 24bpp 下同样只覆盖高 8 位。</summary>
    [Fact]
    public void ConvertLine32_WithAlpha24Bit()
    {
        var src = new byte[] { 9, 8, 7 };
        var des = Buf(4);

        DxCanvas.ConvertLine32(src, 0, des, 0, 1, 24, null, new byte[] { 0x55 }, 0);

        Assert.Equal(0x55070809u, ReadU32(des, 0));
    }

    // =====================================================================
    // ConvertBrightLine32（553-697）
    // =====================================================================

    /// <summary>用例 26（差异断言）：亮化 = `MIN(255, Round(通道 * 1.3))` 且 rgbReserved := 255。</summary>
    [Theory]
    [InlineData(0, 0)]         // 全黑 → 不写（走「非黑才写」守卫）
    [InlineData(1, 1)]         // 1*1.3 = 1.3 → 1
    [InlineData(10, 13)]       // 13
    [InlineData(100, 130)]     // 130
    [InlineData(200, 255)]     // 260 → MIN(255) = 255
    [InlineData(255, 255)]
    public void ConvertBrightLine32_ScalesByOnePointThree(byte input, byte expected)
    {
        var src = new byte[] { input, input, input, 0 };
        var des = Buf(4);

        DxCanvas.ConvertBrightLine32(src, 0, des, 0, 1, 32, null, null, 0);

        if (input == 0)
        {
            Assert.Equal(0u, ReadU32(des, 0));       // 黑像素不写
        }
        else
        {
            Assert.Equal(expected, des[0]);
            Assert.Equal(expected, des[1]);
            Assert.Equal(expected, des[2]);
            Assert.Equal(255, des[3]);
        }
    }

    /// <summary>
    /// 用例 27（差异断言）：原文用 Round（银行家舍入）而非 Trunc ——
    /// 5*1.3 = 6.5 → 6（偶数），而不是 7。
    /// </summary>
    [Fact]
    public void ConvertBrightLine32_UsesBankersRoundingAtHalfValues()
    {
        var src = new byte[] { 5, 5, 5, 0 };
        var des = Buf(4);

        DxCanvas.ConvertBrightLine32(src, 0, des, 0, 1, 32, null, null, 0);

        Assert.Equal(6, des[0]);        // Round(6.5) = 6
        Assert.NotEqual(7, des[0]);
    }

    /// <summary>用例 28：亮化 8bpp 走 ColorTableBright_8_32Bit 查表 + Alpha 覆盖高 8 位。</summary>
    [Fact]
    public void ConvertBrightLine32_8BitLookupWithAlpha()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTableBright_8_32Bit[4] = 0xFF00FF00;
        var des = Buf(4);

        DxCanvas.ConvertBrightLine32(new byte[] { 4 }, 0, des, 0, 1, 8, null, new byte[] { 0x77 }, 0);

        Assert.Equal(0x7700FF00u, ReadU32(des, 0));
    }

    /// <summary>用例 29：亮化 16bpp 走 ColorTableBright_16_32Bit 查表。</summary>
    [Fact]
    public void ConvertBrightLine32_16BitLookup()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTableBright_16_32Bit[0x1234] = 0xFFAABBCC;
        var des = Buf(4);

        DxCanvas.ConvertBrightLine32(new byte[] { 0x34, 0x12 }, 0, des, 0, 1, 16, null, null, 0);

        Assert.Equal(0xFFAABBCCu, ReadU32(des, 0));
    }

    // =====================================================================
    // ConvertGrayLine32（699-843）
    // =====================================================================

    /// <summary>用例 30：灰度 32bpp = g_Grays[R+G+B] 三通道同值。</summary>
    [Fact]
    public void ConvertGrayLine32_32BitUsesChannelSum()
    {
        var src = new byte[] { 0, 0, 30, 0 };   // B=0,G=0,R=30
        var des = Buf(4);

        DxCanvas.ConvertGrayLine32(src, 0, des, 0, 1, 32, null, null, 0);

        byte g = (byte)TDxCanvasColorTables.g_Grays[30];
        Assert.Equal(g, des[0]);
        Assert.Equal(g, des[1]);
        Assert.Equal(g, des[2]);
        Assert.Equal(255, des[3]);
    }

    /// <summary>用例 31（差异断言）：灰度用的是**通道和**（g_Grays[R+G+B]），不是平均值 —— (10,10,10) 得 30 而非 10。</summary>
    [Fact]
    public void ConvertGrayLine32_SumNotAverage()
    {
        var src = new byte[] { 10, 10, 10, 0 };
        var des = Buf(4);

        DxCanvas.ConvertGrayLine32(src, 0, des, 0, 1, 32, null, null, 0);

        Assert.Equal((byte)TDxCanvasColorTables.g_Grays[30], des[0]);   // g_Grays[30] = 10
        Assert.Equal((byte)10, des[0]);
    }

    /// <summary>用例 32：灰度 8bpp / 16bpp 查表。</summary>
    [Fact]
    public void ConvertGrayLine32_8And16BitLookup()
    {
        TDxCanvasColorTables.Clear();
        TDxCanvasColorTables.ColorTableGray_8_32Bit[6] = 0xFF060606;
        TDxCanvasColorTables.ColorTableGray_16_32Bit[6] = 0xFF161616;

        var des8 = Buf(4);
        DxCanvas.ConvertGrayLine32(new byte[] { 6 }, 0, des8, 0, 1, 8, null, null, 0);
        Assert.Equal(0xFF060606u, ReadU32(des8, 0));

        var des16 = Buf(4);
        DxCanvas.ConvertGrayLine32(new byte[] { 6, 0 }, 0, des16, 0, 1, 16, null, null, 0);
        Assert.Equal(0xFF161616u, ReadU32(des16, 0));
    }

    // =====================================================================
    // ComputeCopyRegion（1068-1111）
    // =====================================================================

    /// <summary>用例 33：不裁剪的常规拷贝。</summary>
    [Fact]
    public void ComputeCopyRegion_NoClipping()
    {
        var r = DxCanvas.ComputeCopyRegion(10, 10, 100, 100, 5, 6);

        Assert.False(r.Abort);
        Assert.Equal(0, r.SrcLeft);
        Assert.Equal(10, r.SrcWidth);
        Assert.Equal(0, r.SrcTop);
        Assert.Equal(10, r.SrcBottom);
        Assert.Equal(5, r.X);
        Assert.Equal(6, r.Y);
    }

    /// <summary>用例 34：x/y 为负时源矩形左/上被削、目标坐标归零。</summary>
    [Fact]
    public void ComputeCopyRegion_NegativeOrigin()
    {
        var r = DxCanvas.ComputeCopyRegion(10, 10, 100, 100, -4, -6);

        Assert.False(r.Abort);
        Assert.Equal(4, r.SrcLeft);
        Assert.Equal(6, r.SrcWidth);        // 10 + (-4)（第 1285 行的截断条件此时不成立）
        Assert.Equal(0, r.X);
        Assert.Equal(6, r.SrcTop);
        Assert.Equal(10, r.SrcBottom);      // 6 + 10 + (-6) = 10（第 1291 行的截断不成立）
        Assert.Equal(0, r.Y);
    }

    /// <summary>用例 35（差异断言）：x &gt;= Target.Width 或 y &gt;= Target.Height 时直接 Abort（1079-1080）。</summary>
    [Theory]
    [InlineData(100, 0, true)]
    [InlineData(0, 100, true)]
    [InlineData(99, 99, false)]
    [InlineData(200, 200, true)]
    public void ComputeCopyRegion_AbortsWhenTargetOriginOutOfRange(int x, int y, bool abort)
    {
        var r = DxCanvas.ComputeCopyRegion(10, 10, 100, 100, x, y);
        Assert.Equal(abort, r.Abort);
    }

    /// <summary>用例 36：源图超出目标右下时宽度/高度被截断。</summary>
    [Fact]
    public void ComputeCopyRegion_ClipsToTargetEdges()
    {
        var r = DxCanvas.ComputeCopyRegion(50, 50, 100, 100, 80, 90);

        Assert.False(r.Abort);
        Assert.Equal(20, r.SrcWidth);       // 100 - 80
        Assert.Equal(10, r.SrcBottom);      // 100 - 90 + 0
    }

    /// <summary>用例 37：srcwidth 截到 0 或源左上越界时 Abort（1110-1111）。</summary>
    [Fact]
    public void ComputeCopyRegion_AbortsOnDegenerateResult()
    {
        // x = -50、源宽 10 → srcwidth = 10 + (-50) = -40 → Abort
        Assert.True(DxCanvas.ComputeCopyRegion(10, 10, 100, 100, -50, 0).Abort);
        // x = 100 恰好等于 Target.Width → Abort
        Assert.True(DxCanvas.ComputeCopyRegion(10, 10, 100, 100, 100, 0).Abort);
        // 源宽为 0 → srcwidth = 0 → Abort
        Assert.True(DxCanvas.ComputeCopyRegion(0, 10, 100, 100, 0, 0).Abort);
    }

    // =====================================================================
    // CopyTexture（1068-1149）
    // =====================================================================

    /// <summary>用例 38：不混合时 `if srcPoint^ &lt;&gt; 0 then targetPoint^ := srcPoint^`。</summary>
    [Fact]
    public void CopyTexture_NonBlendSkipsZeroSourcePixels()
    {
        var src = new TDxTextureBuffer(2, 1);
        src.SetPixel(0, 0, 0x00000000);
        src.SetPixel(1, 0, 0xFF112233);

        var dst = new TDxTextureBuffer(4, 1);
        dst.SetPixel(0, 0, 0xFFAAAAAA);
        dst.SetPixel(1, 0, 0xFFAAAAAA);

        DxCanvas.CopyTexture(src, dst, 1, 0, blend: false);

        Assert.Equal(0xFFAAAAAAu, dst.GetPixel(1, 0));   // 源 0 → 保持原值
        Assert.Equal(0xFF112233u, dst.GetPixel(2, 0));
    }

    /// <summary>
    /// 用例 39（差异断言）：混合时除拷贝外还按 `nGray := (30*B + 59*G + 11*R) div 100`
    /// 写入 rgbReserved（整数除法，向零截断）。
    /// </summary>
    [Fact]
    public void CopyTexture_BlendWritesGrayIntoReserved()
    {
        var src = new TDxTextureBuffer(1, 1);
        src.SetPixel(0, 0, 0x00123456);     // B=0x56, G=0x34, R=0x12
        var dst = new TDxTextureBuffer(1, 1);

        DxCanvas.CopyTexture(src, dst, 0, 0, blend: true);

        int expected = (30 * 0x56 + 59 * 0x34 + 11 * 0x12) / 100;
        Assert.Equal((byte)expected, dst.Bits[TDxCanvasColorTables.ReservedOfs]);
        Assert.Equal(0x56, dst.Bits[TDxCanvasColorTables.BlueOfs]);
        Assert.Equal(0x34, dst.Bits[TDxCanvasColorTables.GreenOfs]);
        Assert.Equal(0x12, dst.Bits[TDxCanvasColorTables.RedOfs]);
    }

    /// <summary>用例 40：Source 或 Target 为 nil → 直接返回（1077-1078）。</summary>
    [Fact]
    public void CopyTexture_NullArgumentsReturnQuietly()
    {
        var t = new TDxTextureBuffer(2, 2);
        DxCanvas.CopyTexture(null, t, 0, 0);
        DxCanvas.CopyTexture(t, null, 0, 0);
        // 不抛异常即通过
        Assert.Equal(0x00000000u, t.GetPixel(0, 0));
    }

    /// <summary>用例 41：越界（x &gt;= Target.Width）时整段不拷贝（1079）。</summary>
    [Fact]
    public void CopyTexture_OutOfRangeDoesNothing()
    {
        var src = new TDxTextureBuffer(2, 2);
        src.SetPixel(0, 0, 0xFFFFFFFF);
        var dst = new TDxTextureBuffer(2, 2);

        DxCanvas.CopyTexture(src, dst, 2, 0);
        Assert.Equal(0u, dst.GetPixel(0, 0));

        DxCanvas.CopyTexture(src, dst, 0, 2);
        Assert.Equal(0u, dst.GetPixel(0, 0));
    }

    /// <summary>
    /// 用例 42（原文缺陷，照抄并锁定）：内层列循环的上界写的是 `srcwidth - 1`，
    /// 但 j 的起点是 `srcleft`；当 srcleft = 0（x &gt;= 0）时二者恰好一致，
    /// 一旦 x &lt; 0（srcleft = -x &gt; 0）列循环就会**提前结束**，右端有 srcleft 个像素拷不到。
    /// 原文 DxCanvas.pas:1119/1129 `for j := srcLeft to srcwidth - 1`。
    /// </summary>
    [Fact]
    public void CopyTexture_ColumnLoopUpperBoundIsSrcWidthNotSrcLeftPlusWidth()
    {
        // 源 4×1，目标 4×1，x = -1 → srcleft = 1、srcwidth = 4
        var src = new TDxTextureBuffer(4, 1);
        for (int i = 0; i < 4; i++)
            src.SetPixel(i, 0, 0xFF000010u + (uint)i);

        var dst = new TDxTextureBuffer(4, 1);
        DxCanvas.CopyTexture(src, dst, -1, 0, blend: false);

        // j 从 1 跑到 srcwidth-1 = 3 → 共 3 次迭代，但目标列 = X + j - srcleft = j - 1 ∈ [0,2]
        // （目标列 2 落在目标宽 4 之内；实测只写出前两列，见下方差异断言）
        Assert.Equal(0xFF000011u, dst.GetPixel(0, 0));   // 源列 1
        Assert.Equal(0xFF000012u, dst.GetPixel(1, 0));   // 源列 2
        // 差异断言：源列 3（j=3）的值没能落到目标列 2 —— 原文上界 `srcwidth - 1` 的后果
        Assert.Equal(0u, dst.GetPixel(2, 0));
        Assert.Equal(0u, dst.GetPixel(3, 0));

        // 对照：源像素都还在，只是没被读走
        Assert.Equal(0xFF000013u, src.GetPixel(3, 0));
    }

    /// <summary>
    /// 用例 42b：x &gt;= 0 时（srcleft = 0）列循环上界恰好等于 srcleft+srcwidth-1，
    /// 故**逐列完整拷贝** —— 原文缺陷只在负起点时才显形。
    /// </summary>
    [Fact]
    public void CopyTexture_NonNegativeOriginCopiesEveryColumn()
    {
        var src = new TDxTextureBuffer(3, 1);
        src.SetPixel(0, 0, 0xFF000001);
        src.SetPixel(1, 0, 0xFF000002);
        src.SetPixel(2, 0, 0xFF000003);

        var dst = new TDxTextureBuffer(4, 1);
        DxCanvas.CopyTexture(src, dst, 1, 0, blend: false);

        Assert.Equal(0u, dst.GetPixel(0, 0));
        Assert.Equal(0xFF000001u, dst.GetPixel(1, 0));
        Assert.Equal(0xFF000002u, dst.GetPixel(2, 0));
        Assert.Equal(0xFF000003u, dst.GetPixel(3, 0));
    }

    /// <summary>
    /// 用例 42c：行内偏移 `srcPitch * i` 里的 i 是**行号**（原文大写 I 只是同一标识符的另一种写法，
    /// Delphi 大小写不敏感），故多行拷贝逐行正确对应。
    /// </summary>
    [Fact]
    public void CopyTexture_RowOffsetUsesRowIndex()
    {
        var src = new TDxTextureBuffer(1, 2);
        src.SetPixel(0, 0, 0xFF000001);
        src.SetPixel(0, 1, 0xFF000002);

        var dst = new TDxTextureBuffer(1, 2);
        DxCanvas.CopyTexture(src, dst, 0, 0, blend: false);

        Assert.Equal(0xFF000001u, dst.GetPixel(0, 0));
        Assert.Equal(0xFF000002u, dst.GetPixel(0, 1));
    }

    // =====================================================================
    // NewTexture 族（845-1066）
    // =====================================================================

    /// <summary>用例 43：NewTexture 按 Source.BitCount 分派，并对每一行调用转换。</summary>
    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)16)]
    [InlineData((byte)24)]
    [InlineData((byte)32)]
    public void NewTexture_DispatchesOnBitCount(byte bitCount)
    {
        var f = InstallFactory();
        var dib = new FakeDib(2, 3, bitCount);

        var tex = DxCanvas.NewTexture(dib);

        Assert.NotNull(tex);
        Assert.Single(f.Created);
        Assert.Equal((2, 3), f.Created[0]);
    }

    /// <summary>用例 44：NewTextureGray / NewTextureBright 同样按尺寸创建纹理并逐行转换。</summary>
    [Fact]
    public void NewTextureGrayAndBright_CreateMatchingTexture()
    {
        var f = InstallFactory();
        var dib = new FakeDib(4, 2, 32);
        dib.Row(0)[2] = 40;                  // R=40

        Assert.NotNull(DxCanvas.NewTextureGray(dib));
        Assert.NotNull(DxCanvas.NewTextureBright(dib));

        Assert.Equal(2, f.Created.Count);
        Assert.All(f.Created, c => Assert.Equal((4, 2), c));
    }

    /// <summary>用例 45：带 Alpha 时逐行取 Alpha.ScanLine[Y] 并覆盖高 8 位。</summary>
    [Fact]
    public void NewTexture_WithAlphaAppliesAlphaRow()
    {
        InstallFactory();
        var dib = new FakeDib(1, 1, 32);
        dib.Row(0)[2] = 0x40;                // R=0x40
        var alpha = new FakeDib(1, 1, 8);
        alpha.Row(0)[0] = 0x33;

        var tex = DxCanvas.NewTexture(dib, alpha);

        Assert.Equal(0x33, tex.Bits[TDxCanvasColorTables.ReservedOfs]);
    }

    /// <summary>用例 46：TextureFactory 为 nil 时返回 null（原文 Assigned(Result) 守卫）。</summary>
    [Fact]
    public void NewTexture_WithoutFactoryReturnsNull()
    {
        DxCanvas.TextureFactory = null;
        Assert.Null(DxCanvas.NewTexture(new FakeDib(2, 2, 32)));
        Assert.Null(DxCanvas.NewTextureGray(new FakeDib(2, 2, 32)));
        Assert.Null(DxCanvas.NewTextureBright(new FakeDib(2, 2, 32)));
        Assert.Null(DxCanvas.NullTexture());
    }

    /// <summary>用例 47：Create 返回 null 时不崩（原文 `if Assigned(Result)`）。</summary>
    [Fact]
    public void NewTexture_WhenCreateReturnsNullIsSafe()
    {
        var f = InstallFactory();
        f.ReturnNullOnCreate = true;

        Assert.Null(DxCanvas.NewTexture(new FakeDib(2, 2, 32)));
        Assert.Null(DxCanvas.NewTextureGray(new FakeDib(2, 2, 32)));
        Assert.Null(DxCanvas.NewTextureBright(new FakeDib(2, 2, 32)));
    }

    /// <summary>用例 48：NewTextureFromGraphic 对 null 返回 null（1008-1009）。</summary>
    [Fact]
    public void NewTextureFromGraphic_NullReturnsNull()
    {
        InstallFactory();
        Assert.Null(DxCanvas.NewTextureFromGraphic(null));
    }

    /// <summary>用例 49：NewTextureFromGraphic 对非 null 走 NewTexture。</summary>
    [Fact]
    public void NewTextureFromGraphic_DelegatesToNewTexture()
    {
        var f = InstallFactory();
        var tex = DxCanvas.NewTextureFromGraphic(new FakeDib(3, 2, 32));

        Assert.NotNull(tex);
        Assert.Single(f.Created);
        Assert.Equal((3, 2), f.Created[0]);
    }

    /// <summary>用例 50：NullTexture 请求 1×1（83-86）。</summary>
    [Fact]
    public void NullTexture_RequestsOneByOne()
    {
        var f = InstallFactory();
        DxCanvas.NullTexture();
        Assert.Equal((1, 1), f.Created[0]);
    }

    /// <summary>
    /// 用例 51（差异断言）：NewTexture(FileData...) 的 DDS 尝试顺序是
    /// DXT1 → DXT3 → DXT5 → DXT2 → DXT4（原文 1026-1034 的书写顺序，不可"排序"）。
    /// </summary>
    [Fact]
    public void NewTexture_DdsFormatOrderMatchesSource()
    {
        var f = InstallFactory();
        f.ReturnNullOnDds = true;

        DxCanvas.NewTexture(new byte[] { 1, 2, 3 }, 3, 0, 0, 0, useD3DFormat: true);

        Assert.Equal(
            new[] { DxCanvas.D3DFMT_DXT1, DxCanvas.D3DFMT_DXT3, DxCanvas.D3DFMT_DXT5,
                    DxCanvas.D3DFMT_DXT2, DxCanvas.D3DFMT_DXT4 },
            f.DdsFormats);
    }

    /// <summary>用例 52：某个 DDS 格式成功后**不再**尝试后续格式，也不再回落 Texture_Load。</summary>
    [Fact]
    public void NewTexture_StopsAtFirstSuccessfulDdsFormat()
    {
        var f = InstallFactory();
        f.DdsSucceedAtAttempt = 2;            // DXT3 成功

        var tex = DxCanvas.NewTexture(new byte[] { 1 }, 1, 0, 0, 0, useD3DFormat: true);

        Assert.NotNull(tex);
        Assert.Equal(new[] { DxCanvas.D3DFMT_DXT1, DxCanvas.D3DFMT_DXT3 }, f.DdsFormats);
        Assert.Null(f.LastLoadData);
    }

    /// <summary>
    /// 用例 53（差异断言）：全部 DDS 失败后回落到 Texture_Load（1036-1037 的 if **在 if 之外**，
    /// 故 useD3DFormat=False 也走 Load）。
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NewTexture_FallsBackToLoad(bool useD3DFormat)
    {
        var f = InstallFactory();
        f.ReturnNullOnDds = true;
        var data = new byte[] { 9, 9 };

        var tex = DxCanvas.NewTexture(data, data.Length, 0, 0, 0, useD3DFormat);

        Assert.NotNull(tex);
        Assert.Same(data, f.LastLoadData);
    }

    // =====================================================================
    // MeasureTextTexture / GetTextTexture（1151-1246）
    // =====================================================================

    /// <summary>用例 54：nW = 各行宽最大值；nLH = '|' 的高度；nH = nLH * 行数。</summary>
    [Fact]
    public void MeasureTextTexture_Geometry()
    {
        var lines = new List<string> { "A", "ABCDEF", "AB" };
        var layout = DxCanvas.MeasureTextTexture(new TDxFontEnv(), new object(), lines);

        Assert.Equal(36, layout.Width);          // "ABCDEF" = 6×6
        Assert.Equal(12, layout.LineHeight);     // '|' 的高（FFontHeight）
        Assert.Equal(36, layout.Height);         // 12 × 3
        Assert.Equal(3, layout.LineCount);
    }

    /// <summary>用例 55：每行按 DT_CENTER 于 Rect(0, I*nLH, nW, ...) 内居中，LineY = I*nLH。</summary>
    [Fact]
    public void MeasureTextTexture_CentersEachLine()
    {
        var lines = new List<string> { "A", "ABCDEF", "AB" };
        var layout = DxCanvas.MeasureTextTexture(new TDxFontEnv(), new object(), lines);

        Assert.Equal(new[] { 0, 12, 24 }, layout.LineY);
        Assert.Equal((36 - 6) / 2, layout.CenteredX[0]);      // 15
        Assert.Equal(0, layout.CenteredX[1]);                 // 最宽行不偏移
        Assert.Equal((36 - 12) / 2, layout.CenteredX[2]);     // 12
    }

    /// <summary>
    /// 用例 56（差异断言）：行数为 0 时 nW = 0 且 nH = 0（原文仍会 Texture_Create(0,0)）。
    /// </summary>
    [Fact]
    public void MeasureTextTexture_EmptyListGivesZeroSize()
    {
        var layout = DxCanvas.MeasureTextTexture(new TDxFontEnv(), new object(), new List<string>());

        Assert.Equal(0, layout.Width);
        Assert.Equal(0, layout.Height);
        Assert.Equal(12, layout.LineHeight);   // 行高仍会去量 '|'
        Assert.Equal(0, layout.LineCount);
    }

    /// <summary>用例 57：单行空串时 nW 保持 0、nH 为一行高（原文空行也算一行）。</summary>
    [Fact]
    public void MeasureTextTexture_SingleEmptyLine()
    {
        var layout = DxCanvas.MeasureTextTexture(new TDxFontEnv(), new object(), new List<string> { "" });

        Assert.Equal(0, layout.Width);
        Assert.Equal(12, layout.Height);
        Assert.Equal(1, layout.LineCount);
        Assert.Equal(0, layout.CenteredX[0]);
    }

    /// <summary>用例 58：GetTextTexture 请求 Create(nW, nH) 并按 `or $FF000000` 搬运。</summary>
    [Fact]
    public void GetTextTexture_CreatesTextureOfLayoutSize()
    {
        var f = InstallFactory();
        var layout = DxCanvas.MeasureTextTexture(new TDxFontEnv(), new object(), new List<string> { "AB" });
        DxCanvas._gdiTextBits = new byte[layout.Width * layout.Height * 4];

        var tex = DxCanvas.GetTextTexture(new TDxFontEnv(), new object(), new List<string> { "AB" });

        Assert.NotNull(tex);
        Assert.Equal((layout.Width, layout.Height), f.Created[0]);
    }

    /// <summary>用例 59（差异断言）：GDI 位图非 0 → `or $FF000000`；为 0 → 写 $00000000（覆盖目标）。</summary>
    [Fact]
    public void GetTextTexture_CopiesWithAlphaForced()
    {
        InstallFactory();
        var lines = new List<string> { "A" };                 // 6×12
        DxCanvas._gdiTextBits = new byte[6 * 12 * 4];
        // 第 0 个像素置成 0x00112233
        DxCanvas._gdiTextBits[0] = 0x33;
        DxCanvas._gdiTextBits[1] = 0x22;
        DxCanvas._gdiTextBits[2] = 0x11;
        DxCanvas._gdiTextBits[3] = 0x00;

        var tex = DxCanvas.GetTextTexture(new TDxFontEnv(), new object(), lines);

        Assert.Equal(0xFF112233u, tex.GetPixel(0, 0));        // 非 0 → 补 0xFF
        Assert.Equal(0x00000000u, tex.GetPixel(1, 0));        // 0 → 保持 0
    }

    /// <summary>用例 60：无工厂时 GetTextTexture 返回 null（不抛异常）。</summary>
    [Fact]
    public void GetTextTexture_WithoutFactoryReturnsNull()
    {
        DxCanvas.TextureFactory = null;
        DxCanvas._gdiTextBits = Array.Empty<byte>();

        Assert.Null(DxCanvas.GetTextTexture(new TDxFontEnv(), new object(), new List<string> { "A" }));
    }

    // ---------------------------------------------------------------

    private static uint ReadU32(byte[] buf, int ofs)
        => (uint)(buf[ofs] | (buf[ofs + 1] << 8) | (buf[ofs + 2] << 16) | (buf[ofs + 3] << 24));
}

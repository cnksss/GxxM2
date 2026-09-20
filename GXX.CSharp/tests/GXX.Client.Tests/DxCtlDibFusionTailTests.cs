using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GXX.Client.DxComponent;
using Xunit;

// =============================================================================================
// DIB.pas 1:1 移植 —— DXFusion 尾部（DIB.Fusion.cs，DIB.pas 7676-7928）。
//
// 覆盖：DoColorize（含内嵌 InvertBitmap）、FadeOut / FadeIn（内联 asm 逐字节 max/min）、
//      DoZoom（逐字节缩放）、DoBlur（十字 5 点均值）、FillDIB8（整块字节填充）。
//
// 为避开行尾填充字节，本文件的图宽一律取 4（WidthBytes = Width*3，无 padding）。
//
// 挂 dxctrl-serial：DibSeams / DibFusion* 是进程级静态状态。
// =============================================================================================

[Collection("dxctrl-serial")]
public class DxCtlDibFusionTailTests
{
    public DxCtlDibFusionTailTests()
    {
        DibSeams.Gdi = null;
        DibSeams.Canvas = null;
        DibSeams.GlobalMemory = null;
        DibFusionCanvas.Seam = null;
        DibFusionSpot.Seam = null;
        DibFusionColorize.Seam = null;
        DibFusionSupport.RandomFunc = null;
    }

    private static TDIB Mk24(int w, int h)
    {
        var d = new TDIB();
        d.SetSize(w, h, 24);
        return d;
    }

    private static int Col(int r, int g, int b) => (int)DibFusionSupport.Rgb((byte)r, (byte)g, (byte)b);

    private static void Fill(TDIB d, int r, int g, int b)
    {
        for (int y = 0; y < d.Height; y++)
            for (int x = 0; x < d.Width; x++)
                d.SetPixel(x, y, unchecked((uint)Col(r, g, b)));
    }

    /// <summary>整块缓冲的原始字节（含行尾填充）。</summary>
    private static byte[] Bytes(TDIB d) => d.SharedImage.SnapshotBits(d.Size);

    private static TDIB ByteUniform(int w, int h, byte v)
    {
        var d = Mk24(w, h);
        Fill(d, v, v, v);
        return d;
    }

    /// <summary>内存偏移（行 0 在最高地址）。</summary>
    private static int Off(TDIB d, int x, int y) => (d.Height - 1 - y) * d.WidthBytes + x * 3;

    /// <summary>构造"内存字节 i = 0x10 + i"的图（要求无行尾填充）。</summary>
    private static TDIB Pattern(int w, int h, out byte[] expected)
    {
        var d = Mk24(w, h);
        expected = new byte[d.Size];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int o = Off(d, x, y);
                d.SetPixel(x, y, unchecked((uint)Col(0x10 + o + 2, 0x10 + o + 1, 0x10 + o)));
            }
        for (int i = 0; i < expected.Length; i++) expected[i] = (byte)(0x10 + i);
        return d;
    }

    // =========================================================================================
    // DIB.pas 7905-7928 —— FillDIB8
    // =========================================================================================

    [Fact]
    public void FillDIB8_填满整块缓冲含行尾填充字节()
    {
        var d = Mk24(1, 1);   // WidthBytes = 4，但只有 3 字节是像素
        Fill(d, 1, 2, 3);
        d.FillDIB8(0x5A);
        var b = Bytes(d);
        Assert.Equal(4, b.Length);
        foreach (byte v in b) Assert.Equal(0x5A, v);
    }

    [Fact]
    public void FillDIB8_全0与全FF()
    {
        var d = Mk24(4, 2);
        Fill(d, 200, 100, 50);
        d.FillDIB8(0x00);
        foreach (byte v in Bytes(d)) Assert.Equal(0, v);

        d.FillDIB8(0xFF);
        foreach (byte v in Bytes(d)) Assert.Equal(0xFF, v);
    }

    [Fact]
    public void FillDIB8_尺寸不变且覆盖全部行列()
    {
        var d = Mk24(4, 4);
        d.FillDIB8(0x7E);
        Assert.Equal(4, d.Width);
        Assert.Equal(4, d.Height);
        Assert.Equal(48, d.Size);
        foreach (byte v in Bytes(d)) Assert.Equal(0x7E, v);
    }

    // =========================================================================================
    // DIB.pas 7780-7813 / 7870-7903 —— FadeOut / FadeIn
    // =========================================================================================

    [Fact]
    public void FadeOut_逐字节取max()
    {
        var src = ByteUniform(4, 2, 0x28);   // 40
        var dst = ByteUniform(4, 2, 0x00);
        src.FadeOut(dst, 100);
        foreach (byte v in Bytes(dst)) Assert.Equal(100, v);

        var src2 = ByteUniform(4, 2, 0xC8);  // 200
        var dst2 = ByteUniform(4, 2, 0x00);
        src2.FadeOut(dst2, 100);
        foreach (byte v in Bytes(dst2)) Assert.Equal(200, v);
    }

    [Fact]
    public void FadeOut_Step0是恒等_Step255全白()
    {
        var src = Pattern(4, 2, out var expect);
        var dst = ByteUniform(4, 2, 0x00);
        src.FadeOut(dst, 0);
        Assert.Equal(expect, Bytes(dst));

        var dst2 = ByteUniform(4, 2, 0x00);
        src.FadeOut(dst2, 255);
        foreach (byte v in Bytes(dst2)) Assert.Equal(255, v);
    }

    [Fact]
    public void FadeIn_逐字节取min()
    {
        var src = ByteUniform(4, 2, 0xC8);   // 200
        var dst = ByteUniform(4, 2, 0xFF);
        src.FadeIn(dst, 100);
        foreach (byte v in Bytes(dst)) Assert.Equal(100, v);

        var src2 = ByteUniform(4, 2, 0x28);  // 40
        var dst2 = ByteUniform(4, 2, 0xFF);
        src2.FadeIn(dst2, 100);
        foreach (byte v in Bytes(dst2)) Assert.Equal(40, v);
    }

    [Fact]
    public void FadeIn_Step0全黑_Step255是恒等()
    {
        var src = Pattern(4, 2, out var expect);
        var dst = ByteUniform(4, 2, 0xFF);
        src.FadeIn(dst, 0);
        foreach (byte v in Bytes(dst)) Assert.Equal(0, v);

        var dst2 = ByteUniform(4, 2, 0xFF);
        src.FadeIn(dst2, 255);
        Assert.Equal(expect, Bytes(dst2));
    }

    [Fact]
    public void FadeOut_目标图高于源图时ScanLine越界()
    {
        var src = Mk24(4, 2);
        var dst = Mk24(4, 4);
        Assert.Throws<EInvalidGraphicOperation>(() => src.FadeOut(dst, 10));
    }

    // =========================================================================================
    // DIB.pas 7815-7852 —— DoZoom
    // =========================================================================================

    [Fact]
    public void DoZoom_比例1是整体右下移1字节()
    {
        // 4x2：w = WidthBytes = 12、h = Height = 2、ZoomRatio = 1 ⇒ xstart = 0、yr = 0、step = 1
        // Y=1: p1 = ScanLine(0)；X=1..3 ⇒ xr=0,1,2 ⇒ P2[1..3] = p1[0..2]（行 0 在内存 +12）
        var src = Pattern(4, 2, out var sb);
        var dst = ByteUniform(4, 2, 0xAA);

        src.DoZoom(dst, 1.0);

        var db = Bytes(dst);
        Assert.Equal(sb[12], db[1]);
        Assert.Equal(sb[13], db[2]);
        Assert.Equal(sb[14], db[3]);
        Assert.Equal(0xAA, db[0]);
        for (int i = 4; i < 24; i++) Assert.Equal(0xAA, db[i]);
    }

    [Fact]
    public void DoZoom_比例0点5按xstart平移且步进保留小数()
    {
        // xstart = (12-6)/2 = 3；yr = (2-1)/2 = 0.5 ⇒ Trunc 0
        // X=1: xr=3 ⇒ p1[3]；X=2: xr=3.5 ⇒ p1[3]；X=3: xr=4 ⇒ p1[4]
        var src = Pattern(4, 2, out var sb);
        var dst = ByteUniform(4, 2, 0xAA);

        src.DoZoom(dst, 0.5);

        var db = Bytes(dst);
        Assert.Equal(sb[12 + 3], db[1]);
        Assert.Equal(sb[12 + 3], db[2]);
        Assert.Equal(sb[12 + 4], db[3]);
        Assert.Equal(0xAA, db[0]);
    }

    [Fact]
    public void DoZoom_比例0时步进为零故同一字节反复采样()
    {
        // xstart = (12-0)/2 = 6；yr = (2-0)/2 = 1 ⇒ p1 = ScanLine(1)（内存 +0）；xstep = 0 ⇒ xr 恒 6
        var src = Pattern(4, 2, out var sb);
        var dst = ByteUniform(4, 2, 0xAA);

        src.DoZoom(dst, 0.0);

        var db = Bytes(dst);
        Assert.Equal(sb[6], db[1]);
        Assert.Equal(sb[6], db[2]);
        Assert.Equal(sb[6], db[3]);
    }

    [Fact]
    public void DoZoom_负比例下yr超过h时整行清零()
    {
        // ZoomRatio = -2 ⇒ yr = (2 + 4)/2 = 3 > h = 2 ⇒ else 分支：P2[X] = 0（X = 1..Width-1 = 1..3）
        var src = ByteUniform(4, 2, 0x77);
        var dst = ByteUniform(4, 2, 0xAA);

        src.DoZoom(dst, -2.0);

        var db = Bytes(dst);
        Assert.Equal(0, db[1]);
        Assert.Equal(0, db[2]);
        Assert.Equal(0, db[3]);
        Assert.Equal(0xAA, db[0]);
        for (int i = 12; i < 24; i++) Assert.Equal(0xAA, db[i]);   // 行 0 完全不处理
    }

    // =========================================================================================
    // DIB.pas 7854-7868 —— DoBlur
    // =========================================================================================

    [Fact]
    public void DoBlur_十字五点均值div5()
    {
        // 4x4、WidthBytes = 12：整块 (100,100,100)，只有图像行 2 为 0。
        // Y=1 时 p1 = ScanLine(1)；p1[X+12] = 行 0、p1[X-12] = 行 2 ⇒ (100*4 + 0)/5 = 80
        var src = Mk24(4, 4);
        Fill(src, 100, 100, 100);
        for (int x = 0; x < 4; x++) src.SetPixel(x, 2, 0);

        var dst = ByteUniform(4, 4, 0xAA);
        src.DoBlur(dst);

        var db = Bytes(dst);
        // 4x4 24bpp：ScanLine(Y) 的内存偏移 = (3-Y)*12 ⇒ 图像行 1 在偏移 24
        Assert.Equal(80, db[24 + 1]);
        Assert.Equal(80, db[24 + 2]);
        Assert.Equal(80, db[24 + 3]);
        Assert.Equal(0xAA, db[24 + 0]);                 // X 从 1 起
        for (int i = 4; i < 12; i++) Assert.Equal(0xAA, db[24 + i]);
        for (int i = 36; i < 48; i++) Assert.Equal(0xAA, db[i]);  // 行 0（Y=0）不处理
    }

    [Fact]
    public void DoBlur_均匀字节图结果不变()
    {
        var src = ByteUniform(4, 4, 60);   // 三通道同值 ⇒ 任意十字均值仍为 60
        var dst = ByteUniform(4, 4, 0xAA);
        src.DoBlur(dst);
        var db = Bytes(dst);
        // 只写行 1 的字节 1..3（X = 1..Width-1）
        Assert.Equal(60, db[25]);
        Assert.Equal(60, db[26]);
        Assert.Equal(60, db[27]);
        Assert.Equal(0xAA, db[24]);
        for (int i = 28; i < 36; i++) Assert.Equal(0xAA, db[i]);
    }

    [Fact]
    public void DoBlur_高度1时不进入循环()
    {
        var src = ByteUniform(4, 1, 0x33);
        var dst = ByteUniform(4, 1, 0xAA);
        src.DoBlur(dst);
        foreach (byte v in Bytes(dst)) Assert.Equal(0xAA, v);
    }

    // =========================================================================================
    // DIB.pas 7676-7776 —— DoColorize
    // =========================================================================================

    private sealed class RecordingColorizeSeam : IDibFusionColorizeSeam
    {
        public readonly List<uint> CopyModes = new();
        public readonly List<(TDIB dib, uint value)> CopyModeOps = new();
        public readonly List<int> BrushColors = new();
        public readonly List<(int l, int t, int r, int b)> Fills = new();
        public readonly List<bool> CopyRectSelf = new();
        public int BrushStyles;
        public int Pixels;
        public int BrushBitmaps;
        public TDIB LastCopyRectDest;
        public TDIB LastCopyRectSrc;
        /// <summary>收到 cmSrcErase 的那张图在**当时**的共享图（= 最终赋给 Self 的图）。</summary>
        public TDIBSharedImage EraseSharedImage;

        public void SetBrushStyleSolid(TDIB Dib) => BrushStyles++;
        public void SetBrushColor(TDIB Dib, int Color) => BrushColors.Add(Color);
        public void FillRect(TDIB Dib, TDxRect Rect) => Fills.Add((Rect.Left, Rect.Top, Rect.Right, Rect.Bottom));

        public void SetCopyMode(TDIB Dib, uint Value)
        {
            CopyModes.Add(Value);
            CopyModeOps.Add((Dib, Value));
            if (Value == DibFusionRop.cmSrcErase) EraseSharedImage = Dib.SharedImage;
        }

        public void CopyRect(TDIB DestDib, TDxRect DestRect, TDIB SrcDib, TDxRect SrcRect)
        {
            CopyRectSelf.Add(ReferenceEquals(DestDib, SrcDib));
            LastCopyRectDest = DestDib;
            LastCopyRectSrc = SrcDib;
        }

        public void SetPixels(TDIB Dib, int X, int Y, int Color) => Pixels++;
        public void AssignBrushBitmap(TDIB Dib, TDIB Value) => BrushBitmaps++;
    }

    [Fact]
    public void DoColorize_接缝未装载时抛异常()
    {
        var d = Mk24(4, 2);
        Fill(d, 10, 20, 30);
        var ex = Assert.Throws<InvalidOperationException>(
            () => d.DoColorize(Col(255, 0, 0), Col(0, 0, 255)));
        Assert.Contains("IDibFusionColorizeSeam", ex.Message);
    }

    [Fact]
    public void DoColorize_按原文顺序发出光栅操作()
    {
        var seam = new RecordingColorizeSeam();
        DibFusionColorize.Seam = seam;
        var d = Mk24(4, 2);
        Fill(d, 10, 20, 30);
        int fore = Col(255, 0, 0);
        int back = Col(0, 0, 255);

        d.DoColorize(fore, back);

        // CopyMode 序列（含两次 InvertBitmap 内部的 cmDstInvert）—— DIB.pas:7711/7714/7717/7746/7749/7752/7755/7758
        Assert.Equal(new[]
        {
            DibFusionRop.cmSrcInvert,                                   // lTB  := BackColor
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(Src)
            DibFusionRop.cmSrcPaint,                                    // lTB  |= Src
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(lTB)
            DibFusionRop.cmSrcInvert,
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(Src)
            DibFusionRop.cmDstInvert,                                   // if not fForeDither → InvertBitmap(Src)
            DibFusionRop.cmPatPaint,                                    // lTB2 := 前色（非抖色分支）
            DibFusionRop.cmSrcInvert,
            DibFusionRop.cmSrcInvert,                                   // lTB  ^= lTB2
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(Src)
            DibFusionRop.cmSrcErase,                                    // lTB  &= ~Src
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(Src)
            DibFusionRop.cmSrcInvert,                                   // lTB  ^= lTB2
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(lTB)
            DibFusionRop.cmDstInvert,                                   // InvertBitmap(Src)
        }, seam.CopyModes);

        // 画刷颜色：BackColor（lTB 底）→ clBlack（lTB2 底）→ ForeColor（lTB2 前色）
        Assert.Equal(new[] { back, DibFusionSupport.clBlack, fore }, seam.BrushColors);
        Assert.Equal(3, seam.BrushStyles);
        Assert.Equal(2, seam.Fills.Count);
        Assert.Equal((0, 0, 4, 2), seam.Fills[0]);
        Assert.Equal((0, 0, 4, 2), seam.Fills[1]);

        // 16 次 CopyRect，其中 8 次是 InvertBitmap 的自拷贝
        Assert.Equal(16, seam.CopyRectSelf.Count);
        Assert.Equal(8, seam.CopyRectSelf.FindAll(x => x).Count);

        // fForeDither = false（原文未初始化；本片取 false）⇒ 抖色分支完全不执行
        Assert.Equal(0, seam.Pixels);
        Assert.Equal(0, seam.BrushBitmaps);

        // 最后一次 CopyRect 是自己对自己（InvertBitmap(Src)，原文顺序的收尾）
        Assert.True(seam.CopyRectSelf[seam.CopyRectSelf.Count - 1]);

        // `Dst.Assign(lTempBitmap)`：唯一一次 cmSrcErase 作用的对象（在 seam 调用当时捕获的共享图）
        // 就是最终的 Self 共享图（lTempBitmap 随后 Destroy ⇒ 事后看它的 SharedImage 已回到空图）
        Assert.NotNull(seam.EraseSharedImage);
        Assert.Same(seam.EraseSharedImage, d.SharedImage);
    }

    [Fact]
    public void DoColorize_结果Dst取自lTempBitmap()
    {
        // Dst := lTempBitmap —— 假接缝不产生像素，故 Dst 是"由 Src 尺寸建出的 24bpp 空图"
        var seam = new RecordingColorizeSeam();
        DibFusionColorize.Seam = seam;
        var d = Mk24(4, 2);
        Fill(d, 10, 20, 30);

        d.DoColorize(Col(255, 0, 0), Col(0, 0, 255));

        Assert.Equal(4, d.Width);
        Assert.Equal(2, d.Height);
        Assert.Equal(24, d.BitCount);
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 4; x++)
                Assert.Equal(0u, d.GetPixel(x, y));   // SetSize 清零，接缝未写像素
    }
}

using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

// =============================================================================================
// DIB.pas 1:1 移植 —— DXFusion 绘制分片测试（DIB.Fusion.cs，DIB.pas 4940-5846 + 5917-5934）。
//
// 覆盖：TCustomDXDIB / TCustomDXPaintBox（含 Paint 布局决策）、CreateDIBFromBitmap、
//      DrawOn / DrawTo、DrawTransparent / DrawShadow / DrawDarken / DrawQuickAlpha /
//      DrawAdditive / DrawTranslucent / DrawAlpha / DrawAlphaMask / DrawMorphed / DrawMono /
//      Draw3x3Matrix / DrawAntialias / FilterLine / FilterRect。
//
// 期望值全部由**回读原文**（DIB.pas）推导，并在注释里给出 `DIB.pas:<行>`。
// 本族"看起来一样实则不同"的分支极多，故大量使用**差异断言**（同名方法的两个分支给出不同值、
// 或两个语义相近的方法在同一输入下分道扬镳）：
//   * DrawTransparent / DrawTranslucent / DrawMorphed / DrawMono 的 `StartY := -DestStartY`
//     vs DrawAlpha / DrawAlphaMask 的 `StartY := DestStartY`（正负号 + 检查顺序都不同）；
//   * DrawShadow / DrawQuickAlpha 的 fmNormal 与 fmMix50 走**同一**分支；
//   * DrawTranslucent 用 **Integer** 加法，而 Drawing 族其它方法写回 Byte 会回绕；
//   * DrawAdditive 的 `(Alpha - p1^) * P2^ shr 8` 在 Alpha < dst 时**逻辑右移**放大后回绕；
//   * FilterLine 的 fmNormal 是"向 Color 靠拢"，fmMix25 是 "Color 权重 1/4"（与名字相反）；
//   * DrawTo 的 `Rect(X, Y, Width, Height)` 把 Width/Height 当 TRect.Right/Bottom。
//
// 挂 dxctrl-serial 串行集合：DibSeams / DibFusionCanvas / DibFusionSupport.RandomFunc 都是
// **进程级静态状态**，并行跑会互相污染（集合定义由另一车道提供，此处只使用不定义）。
// =============================================================================================

[Collection("dxctrl-serial")]
public class DxCtlDibFusionTests
{
    public DxCtlDibFusionTests()
    {
        // 进程级静态状态：每个用例都从干净状态开始
        DibSeams.Gdi = null;
        DibSeams.Canvas = null;
        DibSeams.GlobalMemory = null;
        DibFusionCanvas.Seam = null;
        DibFusionSupport.RandomFunc = null;
    }

    // -----------------------------------------------------------------------------------------
    // 假接缝（真实现属 §2.3 不移植项）
    // -----------------------------------------------------------------------------------------

    private sealed class FakeCanvas : DibSeams.IDibCanvasSeam
    {
        public int DrawCount;
        public int DrawGraphicCount;
        public object LastGraphic;
        public uint CopyMode => 0x00CC0020;
        public void Draw(IntPtr dc, int x, int y, TDIB source) { DrawCount++; }
        public void DrawGraphic(IntPtr dc, int x, int y, object graphic) { DrawGraphicCount++; LastGraphic = graphic; }
    }

    private sealed class FakeFusionCanvas : IDibFusionCanvasSeam
    {
        public int Count;
        public IntPtr DestDc, SrcDc;
        public int X, Y, Width, Height, XSrc, YSrc;
        public uint Rop;

        public void BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop)
        {
            Count++;
            DestDc = hdcDest; X = nXDest; Y = nYDest; Width = nWidth; Height = nHeight;
            SrcDc = hdcSrc; XSrc = nXSrc; YSrc = nYSrc; Rop = dwRop;
        }
    }

    private sealed class FakePaintSeam : IDibFusionPaintSeam
    {
        public bool Designing { get; set; }
        public bool ControlStyleReplicatable { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int ClientWidth { get; set; }
        public int ClientHeight { get; set; }

        public int InvalidateCount;
        public int PenDashCount;
        public int BrushClearCount;
        public readonly List<(int x1, int y1, int x2, int y2)> Rects = new();
        public int StretchDrawCount;
        public TDxRect LastStretchRect;
        public int DrawCount;
        public int LastDrawX = int.MinValue;
        public int LastDrawY = int.MinValue;
        public TDIB LastGraphic;

        public void Invalidate() => InvalidateCount++;
        public void SetPenStyleDash() => PenDashCount++;
        public void SetBrushStyleClear() => BrushClearCount++;
        public void Rectangle(int X1, int Y1, int X2, int Y2) => Rects.Add((X1, Y1, X2, Y2));

        public void StretchDraw(TDxRect DestRect, TDIB Graphic)
        {
            StretchDrawCount++;
            LastStretchRect = DestRect;
            LastGraphic = Graphic;
        }

        public void Draw(int X, int Y, TDIB Graphic)
        {
            DrawCount++;
            LastDrawX = X;
            LastDrawY = Y;
            LastGraphic = Graphic;
        }
    }

    private sealed class FakeBitmapSource : TDibBitmapSource
    {
        public FakeBitmapSource(int w, int h) { Width = w; Height = h; }
        public override int Width { get; }
        public override int Height { get; }
        public override IntPtr Handle => (IntPtr)0x1111;
        public override IntPtr Palette => IntPtr.Zero;
        public override void GetBitmapInfo(out int bitsPixel, out int width, out int height,
            out int field0, out int field1, out int field2, out bool isDibSection)
        {
            bitsPixel = 24; width = Width; height = Height;
            field0 = field1 = field2 = 0; isDibSection = false;
        }
    }

    // -----------------------------------------------------------------------------------------
    // 合成测试数据构造
    // -----------------------------------------------------------------------------------------

    /// <summary>建一个 24bpp 内存 DIB（PixelFormat 由 EmptyDIBImage 的 8:8:8 带出）。</summary>
    private static TDIB Mk24(int w, int h)
    {
        var d = new TDIB();
        d.SetSize(w, h, 24);
        return d;
    }

    /// <summary>Windows.RGB(r, g, b) —— 也就是 TDIB.GetPixel 的 DWord 打包（R 在低字节）。</summary>
    private static int Col(int r, int g, int b) => (int)DibFusionSupport.Rgb((byte)r, (byte)g, (byte)b);

    private static void Fill(TDIB d, int r, int g, int b)
    {
        for (int y = 0; y < d.Height; y++)
            for (int x = 0; x < d.Width; x++)
                d.SetPixel(x, y, unchecked((uint)Col(r, g, b)));
    }

    private static (int R, int G, int B) Px(TDIB d, int x, int y)
    {
        uint v = d.GetPixel(x, y);
        return (unchecked((int)(v & 0xFF)), unchecked((int)((v >> 8) & 0xFF)), unchecked((int)((v >> 16) & 0xFF)));
    }

    private static void AssertPx(TDIB d, int x, int y, int r, int g, int b, string why)
    {
        var (R, G, B) = Px(d, x, y);
        Assert.True(R == r && G == g && B == b,
            $"{why}: 期望 ({r},{g},{b}) 实得 ({R},{G},{B}) @({x},{y})");
    }

    // =========================================================================================
    // DibFusionSupport —— Delphi 语义助手（§24.3 移植陷阱 2/3/5 的落点）
    // =========================================================================================

    [Fact]
    public void Support_Shr_对负数_是逻辑右移_与算术右移不同()
    {
        // Delphi `shr` 对 Integer 逻辑右移（高位补 0）；C# 的 `>>` 对 int 是算术右移。
        Assert.Equal(16777215, DibFusionSupport.Shr(-3, 8));  // 0xFFFFFFFD >> 8 = 0x00FFFFFF
        Assert.Equal(-1, -3 >> 8);                            // 差异：算术右移
        Assert.NotEqual(-3 >> 8, DibFusionSupport.Shr(-3, 8));

        Assert.Equal(0, DibFusionSupport.Shr(0, 8));
        Assert.Equal(1, DibFusionSupport.Shr(256, 8));
        Assert.Equal(0x00FFFFFF, DibFusionSupport.Shr(unchecked((int)0xFFFFFFFF), 8));
    }

    [Fact]
    public void Support_Shl_与整数溢出回绕()
    {
        Assert.Equal(6, DibFusionSupport.Shl(3, 1));
        // 3 shl 1 + 3 = 9（DrawAdditive 的 Wid = 3*Width）
        Assert.Equal(9, DibFusionSupport.Shl(3, 1) + 3);
        // 负数左移：位级移位（丢弃高位）
        Assert.Equal(unchecked((int)0xFFFFFFFE), DibFusionSupport.Shl(-1, 1));
    }

    [Fact]
    public void Support_Round_银行家舍入()
    {
        // Delphi Round 是 round-half-to-even
        Assert.Equal(0, DibFusionSupport.Round(0.4));
        Assert.Equal(2, DibFusionSupport.Round(2.5));
        Assert.Equal(4, DibFusionSupport.Round(3.5));
        Assert.Equal(-2, DibFusionSupport.Round(-2.5));
        Assert.Equal(-4, DibFusionSupport.Round(-3.5));
        Assert.Equal(3, DibFusionSupport.Round(2.6));
    }

    [Fact]
    public void Support_Round_NaN或越界_不静默返回中性值()
    {
        // 有意偏离已登记：Delphi 抛 EInvalidOp；C# 的 (int)Math.Round(Inf) 会静默得 int.MinValue。
        // 依据 §25.2（接缝/落点不得把"语义错"伪装成"分支没命中"）。
        Assert.Throws<ArithmeticException>(() => DibFusionSupport.Round(double.PositiveInfinity));
        Assert.Throws<ArithmeticException>(() => DibFusionSupport.Round(double.NegativeInfinity));
        Assert.Throws<ArithmeticException>(() => DibFusionSupport.Round(double.NaN));
    }

    [Fact]
    public void Support_Trunc_向零截断()
    {
        Assert.Equal(2, DibFusionSupport.Trunc(2.9));
        Assert.Equal(-2, DibFusionSupport.Trunc(-2.9));
        Assert.Throws<ArithmeticException>(() => DibFusionSupport.Trunc(double.NaN));
    }

    [Fact]
    public void Support_Sqr_整数乘法不回绕到浮点()
    {
        Assert.Equal(9, DibFusionSupport.Sqr(3));
        Assert.Equal(9, DibFusionSupport.Sqr(-3));
        // 50000^2 超出 Int32 → 回绕（Delphi Sqr(Integer) 同）
        Assert.Equal(unchecked(50000 * 50000), DibFusionSupport.Sqr(50000));
    }

    [Fact]
    public void Support_颜色宏_TColor是BBGGRR()
    {
        int c = Col(0x12, 0x34, 0x56);
        Assert.Equal(0x563412, c);
        Assert.Equal(0x12, DibFusionSupport.GetRValue(c));
        Assert.Equal(0x34, DibFusionSupport.GetGValue(c));
        Assert.Equal(0x56, DibFusionSupport.GetBValue(c));
        // 高字节不参与（GetXValue 只取 $FF 掩码）
        Assert.Equal(0x56, DibFusionSupport.GetBValue(unchecked((int)0xFF563412)));
        Assert.Equal(0x12, DibFusionSupport.GetRValue(unchecked((int)0xFF563412)));
    }

    [Fact]
    public void Support_Rgb_形参是Byte_会截断()
    {
        Assert.Equal(Col(1, 2, 3), DibFusionSupport.Rgb(1, 2, 3));
        Assert.Equal(0x030201, DibFusionSupport.Rgb(1, 2, 3));
        Assert.Equal(Col(255, 0, 0), DibFusionSupport.Rgb(255, 0, 0));
    }

    [Fact]
    public void Support_Random_Range为0或负()
    {
        Assert.Equal(0, DibFusionSupport.Random(0));
        Assert.Equal(0, DibFusionSupport.Random(-5));
        DibFusionSupport.RandomFunc = r => r - 1;
        Assert.Equal(9, DibFusionSupport.Random(10));
    }

    // =========================================================================================
    // DIB.pas 5154-5159 / 5917-5934 —— DrawTo / DrawOn
    // =========================================================================================

    [Fact]
    public void DrawOn_SrcCanvas为null_直接退出_原文if_not_Assigned分支()
    {
        var seam = new FakeFusionCanvas();
        DibFusionCanvas.Seam = seam;
        var dst = Mk24(4, 4);
        dst.DrawOn(null, TDxRect.Rect(1, 2, 3, 4), new TDibCanvas((IntPtr)0x2, null), 0, 0);
        Assert.Equal(0, seam.Count);
    }

    [Fact]
    public void DrawOn_未装载BitBlt接缝_抛异常而不是静默返回()
    {
        var dst = Mk24(4, 4);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            dst.DrawOn(new TDibCanvas((IntPtr)0x1, null), TDxRect.Rect(1, 2, 3, 4),
                       new TDibCanvas((IntPtr)0x2, null), 0, 0));
        Assert.Contains("IDibFusionCanvasSeam", ex.Message);
    }

    [Fact]
    public void DrawOn_非负源坐标_Dest原样传入且宽高用的是RightBottom()
    {
        var seam = new FakeFusionCanvas();
        DibFusionCanvas.Seam = seam;
        var dst = Mk24(4, 4);
        dst.DrawOn(new TDibCanvas((IntPtr)0xAA, null), TDxRect.Rect(10, 20, 30, 40),
                   new TDibCanvas((IntPtr)0xBB, null), 1, 2);

        Assert.Equal(1, seam.Count);
        Assert.Equal((IntPtr)0xBB, seam.DestDc);
        Assert.Equal((IntPtr)0xAA, seam.SrcDc);
        Assert.Equal(10, seam.X);
        Assert.Equal(20, seam.Y);
        // 原文如此（DIB.pas:5933）：第 4/5 实参应是宽/高，实际传 Dest.Right / Dest.Bottom
        Assert.Equal(30, seam.Width);
        Assert.Equal(40, seam.Height);
        Assert.Equal(1, seam.XSrc);
        Assert.Equal(2, seam.YSrc);
        Assert.Equal(DibFusionCanvas.SRCCOPY, seam.Rop);
    }

    [Fact]
    public void DrawOn_源坐标为负_反向扩展Dest并把源坐标归零()
    {
        var seam = new FakeFusionCanvas();
        DibFusionCanvas.Seam = seam;
        var dst = Mk24(4, 4);
        dst.DrawOn(new TDibCanvas((IntPtr)0xAA, null), TDxRect.Rect(10, 20, 30, 40),
                   new TDibCanvas((IntPtr)0xBB, null), -3, -2);

        // Xsrc<0: Dec(Left, Xsrc)=10-(-3)=13；Inc(Right, Xsrc)=30+(-3)=27
        Assert.Equal(13, seam.X);
        // Ysrc<0: Dec(Top, Ysrc)=20-(-2)=22；Inc(Bottom, Ysrc)=40+(-2)=38
        Assert.Equal(22, seam.Y);
        Assert.Equal(27, seam.Width);
        Assert.Equal(38, seam.Height);
        Assert.Equal(0, seam.XSrc);
        Assert.Equal(0, seam.YSrc);
    }

    [Fact]
    public void DrawTo_把WidthHeight当Rect的RightBottom_原文如此()
    {
        var seam = new FakeFusionCanvas();
        DibFusionCanvas.Seam = seam;
        var dst = Mk24(8, 8);
        var src = Mk24(8, 8);

        dst.DrawTo(src, 5, 6, 7, 8, 1, 2);

        Assert.Equal(1, seam.Count);
        // 原文 `Rect(X, Y, Width, Height)` → Left=5, Top=6, Right=7, Bottom=8
        Assert.Equal(5, seam.X);
        Assert.Equal(6, seam.Y);
        Assert.Equal(7, seam.Width);
        Assert.Equal(8, seam.Height);
        Assert.Equal(1, seam.XSrc);
        Assert.Equal(2, seam.YSrc);
    }

    [Fact]
    public void DrawTo_源坐标为负时由DrawOn反向扩展Rect()
    {
        var seam = new FakeFusionCanvas();
        DibFusionCanvas.Seam = seam;
        var dst = Mk24(8, 8);
        var src = Mk24(8, 8);

        dst.DrawTo(src, 5, 6, 7, 8, -3, -2);

        // Rect(5,6,7,8) → Xsrc=-3 → Left=8、Right=4；Ysrc=-2 → Top=8、Bottom=6
        Assert.Equal(8, seam.X);
        Assert.Equal(8, seam.Y);
        Assert.Equal(4, seam.Width);
        Assert.Equal(6, seam.Height);
        Assert.Equal(0, seam.XSrc);
        Assert.Equal(0, seam.YSrc);
    }

    [Fact]
    public void DrawTo_未装载BitBlt接缝时抛异常()
    {
        var dst = Mk24(4, 4);
        var src = Mk24(4, 4);
        Assert.Throws<InvalidOperationException>(() => dst.DrawTo(src, 0, 0, 2, 2, 0, 0));
    }

    // =========================================================================================
    // DIB.pas 5147-5152 —— CreateDIBFromBitmap
    // =========================================================================================

    [Fact]
    public void CreateDIBFromBitmap_固定24bpp并走Canvas_DrawGraphic()
    {
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;
        var d = new TDIB();
        var src = new FakeBitmapSource(5, 3);

        d.CreateDIBFromBitmap(src);

        Assert.Equal(5, d.Width);
        Assert.Equal(3, d.Height);
        Assert.Equal(24, d.BitCount);            // always 24
        Assert.Equal(1, canvas.DrawGraphicCount);
        Assert.Same(src, canvas.LastGraphic);
    }

    [Fact]
    public void CreateDIBFromBitmap_零尺寸退化为空图且仍调用Draw()
    {
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;
        var d = new TDIB();
        d.CreateDIBFromBitmap(new FakeBitmapSource(0, 0));
        Assert.True(d.Empty);
        Assert.Equal(1, canvas.DrawGraphicCount);
    }

    [Fact]
    public void CreateDIBFromBitmap_负尺寸同样退化为空图()
    {
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;
        var d = new TDIB();
        // SetSize(AWidth<=0 || AHeight<=0) → Clear()（DIB.Core.cs SetSize）
        d.CreateDIBFromBitmap(new FakeBitmapSource(-4, -5));
        Assert.True(d.Empty);
        Assert.Equal(0, d.Width);
        Assert.Equal(1, canvas.DrawGraphicCount);
    }

    [Fact]
    public void CreateDIBFromBitmap_未装载Canvas接缝时不抛_只登记为空操作()
    {
        // 与 DrawOn 不同：原文 Canvas.Draw 在 TCanvas 上总会执行；托管侧 DibSeams.Canvas
        // 未装载时是"无接缝"，DIB.Core.cs 的既有约定为静默无操作（此处只记录该既有事实）。
        var d = new TDIB();
        d.CreateDIBFromBitmap(new FakeBitmapSource(2, 2));
        Assert.Equal(2, d.Width);
        Assert.Equal(24, d.BitCount);
    }

    // =========================================================================================
    // DIB.pas 5161-5217 —— DrawTransparent
    // =========================================================================================

    [Fact]
    public void DrawTransparent_色键像素跳过其余逐字节拷贝()
    {
        var dst = Mk24(3, 1);
        var src = Mk24(3, 1);
        Fill(dst, 10, 20, 30);
        src.SetPixel(0, 0, unchecked((uint)Col(1, 2, 3)));
        src.SetPixel(1, 0, unchecked((uint)Col(4, 5, 6)));
        src.SetPixel(2, 0, unchecked((uint)Col(7, 8, 9)));

        dst.DrawTransparent(src, 0, 0, 3, 1, 0, 0, Col(4, 5, 6));

        AssertPx(dst, 0, 0, 1, 2, 3, "DIB.pas:5208");
        AssertPx(dst, 1, 0, 10, 20, 30, "色键命中 → 不动（DIB.pas:5206）");
        AssertPx(dst, 2, 0, 7, 8, 9, "DIB.pas:5210");
    }

    [Fact]
    public void DrawTransparent_Height为0_整段不执行()
    {
        var dst = Mk24(3, 1);
        var src = Mk24(3, 1);
        Fill(dst, 10, 20, 30);
        Fill(src, 1, 2, 3);
        dst.DrawTransparent(src, 0, 0, 3, 0, 0, 0, 0);
        AssertPx(dst, 0, 0, 10, 20, 30, "EndY=StartY → for 不执行");
    }

    [Fact]
    public void DrawTransparent_X与SourceX只做偏移_不改写区域外像素()
    {
        var dst = Mk24(4, 1);
        var src = Mk24(4, 1);
        Fill(dst, 10, 20, 30);
        Fill(src, 1, 2, 3);
        // SourceX=1 → 源从像素 1 起；X=2 → 目标从像素 2 起；Width=2
        dst.DrawTransparent(src, 2, 0, 2, 1, 1, 0, -1);
        AssertPx(dst, 0, 0, 10, 20, 30, "目标像素 0 不在区间内");
        AssertPx(dst, 1, 0, 10, 20, 30, "目标像素 1 不在区间内");
        AssertPx(dst, 2, 0, 1, 2, 3, "目标像素 2 ← 源像素 1");
        AssertPx(dst, 3, 0, 1, 2, 3, "目标像素 3 ← 源像素 2");
    }

    [Fact]
    public void DrawTransparent_Y为负_行裁剪把StartY抬到负DestStartY()
    {
        // DestStartY = Y - SourceY = -1 - 1 = -2；StartY=1 → StartY+DestStartY=-1<0 → StartY=2
        var dst = Mk24(4, 4);
        var src = Mk24(4, 4);
        Fill(dst, 10, 20, 30);
        Fill(src, 1, 2, 3);
        src.SetPixel(0, 2, unchecked((uint)Col(9, 9, 9)));

        dst.DrawTransparent(src, 0, -1, 4, 2, 0, 1, -1);

        // j=2 → 目标行 j+DestStartY = 0 ← 源行 2
        AssertPx(dst, 0, 0, 9, 9, 9, "DIB.pas:5196-5197");
        AssertPx(dst, 0, 1, 10, 20, 30, "目标行 1 未被写到");
    }

    // =========================================================================================
    // DIB.pas 5219-5267 —— DrawShadow
    // =========================================================================================

    /// <summary>DrawShadow 的固定脚手架：4x3 图，只处理 src/dst 的第 1..2 行、第 1..3 列。</summary>
    private static (TDIB dst, TDIB src) ShadowFixture()
    {
        var dst = Mk24(4, 3);
        var src = Mk24(4, 3);
        Fill(dst, 100, 100, 100);
        Fill(src, 255, 255, 255);
        return (dst, src);
    }

    [Fact]
    public void DrawShadow_fmNormal_按B通道为0命中并各通道shr1()
    {
        var (dst, src) = ShadowFixture();
        src.SetPixel(1, 1, unchecked((uint)Col(255, 255, 0))); // B=0 → 命中，尽管 R/G 非 0
        src.SetPixel(2, 1, unchecked((uint)Col(0, 0, 255)));   // B=255 → 不命中
        src.SetPixel(3, 1, unchecked((uint)Col(0, 0, 0)));     // 命中

        dst.DrawShadow(src, 0, 0, 4, 3, 0, TFilterMode.fmNormal);

        AssertPx(dst, 0, 1, 100, 100, 100, "第 0 列不参与（p1 先 +3）");
        AssertPx(dst, 1, 1, 50, 50, 50, "命中 → 100 shr 1（DIB.pas:5239-5243）");
        AssertPx(dst, 2, 1, 100, 100, 100, "B=255 → 走 else Inc(p1,3)");
        AssertPx(dst, 3, 1, 50, 50, 50, "命中");
        AssertPx(dst, 1, 0, 100, 100, 100, "第 0 行不参与（for I := 1）");
        AssertPx(dst, 1, 2, 100, 100, 100, "源第 2 行全白 B=255 → 不命中");
    }

    [Fact]
    public void DrawShadow_fmMix50与fmNormal同分支_而fmMix25与fmMix75不同()
    {
        var (d1, s1) = ShadowFixture();
        var (d2, s2) = ShadowFixture();
        var (d3, s3) = ShadowFixture();
        var (d4, s4) = ShadowFixture();
        s1.SetPixel(1, 1, unchecked((uint)Col(0, 0, 0)));
        s2.SetPixel(1, 1, unchecked((uint)Col(0, 0, 0)));
        s3.SetPixel(1, 1, unchecked((uint)Col(0, 0, 0)));
        s4.SetPixel(1, 1, unchecked((uint)Col(0, 0, 0)));

        d1.DrawShadow(s1, 0, 0, 4, 3, 0, TFilterMode.fmNormal);
        d2.DrawShadow(s2, 0, 0, 4, 3, 0, TFilterMode.fmMix50);
        d3.DrawShadow(s3, 0, 0, 4, 3, 0, TFilterMode.fmMix25);
        d4.DrawShadow(s4, 0, 0, 4, 3, 0, TFilterMode.fmMix75);

        // fmNormal 与 fmMix50 走**同一** case 分支（DIB.pas:5238）
        Assert.Equal(Px(d1, 1, 1), Px(d2, 1, 1));
        AssertPx(d1, 1, 1, 50, 50, 50, "100 shr 1");
        // fmMix25: p - (p shr 2) = 100 - 25 = 75
        AssertPx(d3, 1, 1, 75, 75, 75, "DIB.pas:5247-5251");
        // fmMix75: p shr 2 = 25
        AssertPx(d4, 1, 1, 25, 25, 25, "DIB.pas:5255-5259");
    }

    [Fact]
    public void DrawShadow_Frame只影响源行偏移()
    {
        var (dst, src) = ShadowFixture();
        // 4px@24bpp 行宽 12 字节；FW=1*4=4 → Inc(P2, 3*(4+1)) = 15
        // I=1: SrcDIB.ScanLine(1)=块内 +12 → +15 = 行 0 的字节 3（行 0 像素 1）
        src.SetPixel(1, 0, unchecked((uint)Col(0, 0, 0)));       // Frame=1 会读到 → 命中
        src.SetPixel(1, 1, unchecked((uint)Col(255, 255, 255))); // Frame=0 会读到 → B=255 不命中

        dst.DrawShadow(src, 0, 0, 4, 3, 1, TFilterMode.fmNormal);

        AssertPx(dst, 1, 1, 50, 50, 50, "Frame=1 时源指针落到行 0（DIB.pas:5232）");
        AssertPx(dst, 2, 1, 100, 100, 100, "行 0 的像素 2 是白 → 不命中");

        // 对照组：Frame=0 读的是源行 1（白）→ 目标行 1 原样
        var (dst0, src0) = ShadowFixture();
        src0.SetPixel(1, 0, unchecked((uint)Col(0, 0, 0)));
        dst0.DrawShadow(src0, 0, 0, 4, 3, 0, TFilterMode.fmNormal);
        AssertPx(dst0, 1, 1, 100, 100, 100, "Frame=0 读源行 1（白）→ 不命中");
    }

    [Fact]
    public void DrawShadow_Height为1_整段不执行()
    {
        var dst = Mk24(4, 1);
        var src = Mk24(4, 1);
        Fill(dst, 100, 100, 100);
        Fill(src, 0, 0, 0);
        dst.DrawShadow(src, 0, 0, 4, 1, 0, TFilterMode.fmNormal);
        AssertPx(dst, 1, 0, 100, 100, 100, "for I := 1 to Height-1 空转");
    }

    // =========================================================================================
    // DIB.pas 5269-5297 —— DrawDarken
    // =========================================================================================

    [Fact]
    public void DrawDarken_逐通道相乘shr8_且只处理第1列起()
    {
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        Fill(dst, 200, 100, 50);
        Fill(src, 128, 64, 32);

        dst.DrawDarken(src, 0, 0, 3, 3, 0);

        // B: (32*50) shr 8 = 6；G: (64*100) shr 8 = 25；R: (128*200) shr 8 = 100
        AssertPx(dst, 1, 1, 100, 25, 6, "DIB.pas:5286-5292");
        AssertPx(dst, 2, 1, 100, 25, 6, "j 推进到像素 2");
        AssertPx(dst, 0, 1, 200, 100, 50, "第 0 列不参与");
        AssertPx(dst, 1, 0, 200, 100, 50, "第 0 行不参与");
    }

    [Fact]
    public void DrawDarken_Frame1读到上一行()
    {
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        Fill(dst, 200, 100, 50);
        Fill(src, 255, 255, 255);
        // 3px@24bpp 行宽 12 字节；frameoffset = 3*(1*3)+3 = 12 = 一整行
        // I=1: SrcDIB.ScanLine(1)=块内 +12 → +12 = 行 0 的字节 0（行 0 像素 0）
        src.SetPixel(0, 0, unchecked((uint)Col(128, 64, 32)));

        dst.DrawDarken(src, 0, 0, 3, 3, 1);

        AssertPx(dst, 1, 1, 100, 25, 6, "Frame=1 → 目标像素 1 ← 源行 0 像素 0（DIB.pas:5276）");
        AssertPx(dst, 2, 1, 199, 99, 49, "目标像素 2 ← 源行 0 像素 1（白色）");
    }

    [Fact]
    public void DrawDarken_零尺寸不崩且不改动()
    {
        var dst = Mk24(2, 2);
        var src = Mk24(2, 2);
        Fill(dst, 7, 8, 9);
        Fill(src, 1, 1, 1);
        dst.DrawDarken(src, 0, 0, 0, 0, 0);
        AssertPx(dst, 1, 1, 7, 8, 9, "Width=0/Height=0 → 空转");
    }

    // =========================================================================================
    // DIB.pas 5299-5375 —— DrawQuickAlpha
    // =========================================================================================

    private static TDIB QuickAlphaFixture(out TDIB src)
    {
        var dst = Mk24(4, 4);
        src = Mk24(4, 4);
        Fill(dst, 10, 10, 10);
        Fill(src, 200, 200, 200);
        return dst;
    }

    private static string Pattern(TDIB d, int y)
    {
        var s = new System.Text.StringBuilder();
        for (int x = 0; x < d.Width; x++)
        {
            var (R, _, _) = Px(d, x, y);
            s.Append(R == 200 ? 'C' : '.');
        }
        return s.ToString();
    }

    [Fact]
    public void DrawQuickAlpha_fmNormal_BitSwitch2跨行不重置()
    {
        var dst = QuickAlphaFixture(out var src);
        dst.DrawQuickAlpha(src, 0, 0, 4, 4, 0, 0, -1, TFilterMode.fmNormal);

        // BitSwitch1 由 Odd(Y)=false 起、每行翻转；BitSwitch2 由 Odd(X)=false 起、**跨行连续**翻转。
        // 若 BitSwitch2 每行重置，行 1 会与行 0 同型 —— 实得两者互补，即为"跨行不重置"的证据。
        Assert.Equal(".C.C", Pattern(dst, 0));
        Assert.Equal("C.C.", Pattern(dst, 1));
        Assert.Equal(".C.C", Pattern(dst, 2));
        Assert.Equal("C.C.", Pattern(dst, 3));
    }

    [Fact]
    public void DrawQuickAlpha_fmMix50与fmNormal同分支_fmMix25要求两个开关都为真()
    {
        var a = QuickAlphaFixture(out var sa);
        var b = QuickAlphaFixture(out var sb);
        var c = QuickAlphaFixture(out var sc);

        a.DrawQuickAlpha(sa, 0, 0, 4, 4, 0, 0, -1, TFilterMode.fmNormal);
        b.DrawQuickAlpha(sb, 0, 0, 4, 4, 0, 0, -1, TFilterMode.fmMix50);
        c.DrawQuickAlpha(sc, 0, 0, 4, 4, 0, 0, -1, TFilterMode.fmMix25);

        Assert.Equal(".C.C", Pattern(a, 0));
        Assert.Equal(".C.C", Pattern(b, 0));   // fmNormal 与 fmMix50 同一 case（DIB.pas:5351）
        // fmMix25 = BitSwitch1 and BitSwitch2 → 行 0 只有 (T,T) 命中
        Assert.Equal("C.C.", Pattern(c, 0));
        Assert.Equal("....", Pattern(c, 1));   // BitSwitch1=False → 整行不命中
    }

    [Fact]
    public void DrawQuickAlpha_fmMix75_或运算_第一行全命中()
    {
        var dst = QuickAlphaFixture(out var src);
        dst.DrawQuickAlpha(src, 0, 0, 4, 4, 0, 0, -1, TFilterMode.fmMix75);
        // 行 0: BitSwitch1=True → (True or x) 恒真
        Assert.Equal("CCCC", Pattern(dst, 0));
        // 行 1: BitSwitch1=False → 只由 BitSwitch2 决定
        Assert.Equal("C.C.", Pattern(dst, 1));
    }

    [Fact]
    public void DrawQuickAlpha_色键命中一律跳过()
    {
        var dst = QuickAlphaFixture(out var src);
        src.SetPixel(1, 0, unchecked((uint)Col(7, 7, 7)));
        dst.DrawQuickAlpha(src, 0, 0, 4, 4, 0, 0, Col(7, 7, 7), TFilterMode.fmMix75);
        AssertPx(dst, 1, 0, 10, 10, 10, "色键命中：即使 BitSwitch 为真也不写");
        AssertPx(dst, 0, 0, 200, 200, 200, "fmMix75 行 0 的其余像素照写");
    }

    // =========================================================================================
    // DIB.pas 5377-5400 —— DrawAdditive
    // =========================================================================================

    private static TDIB AdditiveFixture(out TDIB src)
    {
        var dst = Mk24(6, 3);
        src = Mk24(6, 3);
        Fill(dst, 100, 100, 100);
        Fill(src, 200, 200, 200);
        return dst;
    }

    [Fact]
    public void DrawAdditive_Alpha越界直接退出()
    {
        var dst = AdditiveFixture(out var src);
        dst.DrawAdditive(src, 0, 0, 6, 3, 0, 0);
        AssertPx(dst, 1, 1, 100, 100, 100, "Alpha=0 → Exit");
        dst.DrawAdditive(src, 0, 0, 6, 3, 257, 0);
        AssertPx(dst, 1, 1, 100, 100, 100, "Alpha=257 → Exit");
        dst.DrawAdditive(src, 0, 0, 6, 3, -1, 0);
        AssertPx(dst, 1, 1, 100, 100, 100, "Alpha=-1 → Exit");
    }

    [Fact]
    public void DrawAdditive_Alpha256_只处理每行中间Width像素_行列都从1起()
    {
        var dst = AdditiveFixture(out var src);
        dst.DrawAdditive(src, 0, 0, 6, 3, 256, 0);

        // delta = (256-100)*200 shr 8 = 31200 shr 8 = 121 → 100+121 = 221
        AssertPx(dst, 1, 1, 221, 221, 221, "DIB.pas:5395");
        AssertPx(dst, 4, 2, 221, 221, 221, "末列前一像素仍在区间内");
        AssertPx(dst, 0, 1, 100, 100, 100, "Inc(p1, 3X+3) 跳过列 0");
        AssertPx(dst, 5, 1, 100, 100, 100, "for j := 3 to Wid-4 不覆盖末列");
        AssertPx(dst, 1, 0, 100, 100, 100, "for I := 1 跳过第 0 行");
    }

    [Fact]
    public void DrawAdditive_Alpha小于目标值_逻辑右移导致巨大正增量再回绕()
    {
        var dst = AdditiveFixture(out var src);
        dst.DrawAdditive(src, 0, 0, 6, 3, 1, 0);

        // (1-100)*200 = -19800 → Delphi shr 8 逻辑右移 → 16777138 → 100+16777138 截断 Byte = 0x16 = 22
        // 若误用 C# 的算术 >> 会得 -78 → 100-78 = 22 —— 巧合相同？不：算术右移 -19800>>8 = -78，
        // 22 也一样；故本用例真正的判别点是"回绕成 22 而不是夹到 0"。
        AssertPx(dst, 1, 1, 22, 22, 22, "DIB.pas:5395 + §24.3 陷阱 5");
    }

    [Fact]
    public void DrawAdditive_越界行Break保护()
    {
        var dst = Mk24(6, 2);
        var src = Mk24(6, 4);
        Fill(dst, 100, 100, 100);
        Fill(src, 200, 200, 200);
        // Y=1, Height=4 → I=1 时 I+Y=2 > Height-1(=1) → Break，一行都不写
        dst.DrawAdditive(src, 0, 1, 6, 4, 256, 0);
        AssertPx(dst, 1, 1, 100, 100, 100, "Break 保护（DIB.pas:5388）");
    }

    // =========================================================================================
    // DIB.pas 5402-5457 —— DrawTranslucent
    // =========================================================================================

    [Fact]
    public void DrawTranslucent_整数加法不回绕()
    {
        var dst = Mk24(2, 1);
        var src = Mk24(2, 1);
        Fill(dst, 200, 200, 200);
        Fill(src, 200, 200, 200);

        dst.DrawTranslucent(src, 0, 0, 2, 1, 0, 0, -1);

        // (200+200) shr 1 = 200。若按 Byte 相加回绕会是 (144) shr 1 = 72。
        AssertPx(dst, 0, 0, 200, 200, 200, "Delphi 把 Byte 提升为 Integer（DIB.pas:5448）");
        AssertPx(dst, 1, 0, 200, 200, 200, "...");
    }

    [Fact]
    public void DrawTranslucent_色键跳过且保留目标原值()
    {
        var dst = Mk24(2, 1);
        var src = Mk24(2, 1);
        Fill(dst, 10, 20, 30);
        src.SetPixel(0, 0, unchecked((uint)Col(1, 2, 3)));
        src.SetPixel(1, 0, unchecked((uint)Col(4, 5, 6)));

        dst.DrawTranslucent(src, 0, 0, 2, 1, 0, 0, Col(1, 2, 3));

        AssertPx(dst, 0, 0, 10, 20, 30, "色键命中");
        AssertPx(dst, 1, 0, 7, 12, 18, "(10+4)>>1=7, (20+5)>>1=12, (30+6)>>1=18");
    }

    [Fact]
    public void DrawTranslucent_Height为0不执行()
    {
        var dst = Mk24(2, 2);
        var src = Mk24(2, 2);
        Fill(dst, 10, 20, 30);
        Fill(src, 200, 200, 200);
        dst.DrawTranslucent(src, 0, 0, 2, 0, 0, 0, -1);
        AssertPx(dst, 0, 0, 10, 20, 30, "EndY=StartY");
    }

    // =========================================================================================
    // DIB.pas 5459-5516 —— DrawAlpha
    // =========================================================================================

    [Fact]
    public void DrawAlpha_Alpha0保持目标_Alpha256取源()
    {
        var d0 = Mk24(2, 1);
        var d256 = Mk24(2, 1);
        var src = Mk24(2, 1);
        Fill(d0, 30, 60, 90);
        Fill(d256, 30, 60, 90);
        Fill(src, 200, 100, 50);

        d0.DrawAlpha(src, 0, 0, 2, 1, 0, 0, 0, -1);
        d256.DrawAlpha(src, 0, 0, 2, 1, 0, 0, 256, -1);

        AssertPx(d0, 0, 0, 30, 60, 90, "(dst*256) shr 8 = dst");
        AssertPx(d256, 0, 0, 200, 100, 50, "(src*256) shr 8 = src");
    }

    [Fact]
    public void DrawAlpha_混合公式_dst门256减Alpha加src乘Alpha再shr8()
    {
        var dst = Mk24(1, 1);
        var src = Mk24(1, 1);
        Fill(dst, 100, 100, 100);
        Fill(src, 200, 200, 200);

        dst.DrawAlpha(src, 0, 0, 1, 1, 0, 0, 64, -1);

        // (100*192 + 200*64) shr 8 = (19200 + 12800) shr 8 = 32000 shr 8 = 125
        AssertPx(dst, 0, 0, 125, 125, 125, "DIB.pas:5507-5509");
    }

    [Fact]
    public void DrawAlpha_色键命中跳过()
    {
        var dst = Mk24(1, 1);
        var src = Mk24(1, 1);
        Fill(dst, 100, 100, 100);
        Fill(src, 200, 200, 200);
        dst.DrawAlpha(src, 0, 0, 1, 1, 0, 0, 128, Col(200, 200, 200));
        AssertPx(dst, 0, 0, 100, 100, 100, "DIB.pas:5505");
    }

    [Fact]
    public void DrawAlpha_行裁剪差异_StartY取正号DestStartY_故Y为负时抛SScanline()
    {
        // 与 DrawTransparent 的同一输入对照：Transparent 用 -DestStartY（安全），Alpha 用 DestStartY（负）。
        var dstT = Mk24(4, 4);
        var srcT = Mk24(4, 4);
        Fill(dstT, 10, 20, 30);
        Fill(srcT, 1, 2, 3);
        srcT.SetPixel(0, 2, unchecked((uint)Col(9, 9, 9)));

        dstT.DrawTransparent(srcT, 0, -1, 4, 2, 0, 1, -1);
        AssertPx(dstT, 0, 0, 9, 9, 9, "DrawTransparent: StartY := -DestStartY = 2");

        var dstA = Mk24(4, 4);
        var srcA = Mk24(4, 4);
        Fill(dstA, 10, 20, 30);
        Fill(srcA, 1, 2, 3);
        // DrawAlpha: StartY := DestStartY = -2 → ScanLine(-2 + -2) 抛 SScanline
        var ex = Assert.Throws<EInvalidGraphicOperation>(() =>
            dstA.DrawAlpha(srcA, 0, -1, 4, 2, 0, 1, 128, -1));
        Assert.Contains("scanning line", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DrawAlpha_Y为负且EndY归零_实际一个像素都不画()
    {
        // Y=-1, SourceY=-2 → DestStartY=1；StartY=-2, EndY=-2+2=0
        // 顺序：EndY 未被夹（0+1=1 ≤ 4）→ StartY<0 → StartY=0 → 此时 EndY=0 ⇒ for 0 to -1 空转
        var dst = Mk24(4, 4);
        var src = Mk24(4, 4);
        Fill(dst, 10, 20, 30);
        Fill(src, 200, 100, 50);

        dst.DrawAlpha(src, 0, -1, 4, 2, 0, -2, 256, -1);

        AssertPx(dst, 0, 0, 10, 20, 30, "StartY 被抬到 0 而 EndY 已是 0（DIB.pas:5487-5493）");
        AssertPx(dst, 1, 0, 10, 20, 30, "...");
    }

    // =========================================================================================
    // DIB.pas 5518-5572 —— DrawAlphaMask
    // =========================================================================================

    [Fact]
    public void DrawAlphaMask_掩码下标从0起_与X和SourceX无关()
    {
        var dst = Mk24(4, 1);
        var src = Mk24(4, 1);
        var mask = Mk24(4, 1);
        Fill(dst, 10, 20, 30);
        Fill(src, 200, 100, 50);
        mask.SetPixel(0, 0, unchecked((uint)Col(10, 10, 10)));   // 掩码字节 0 = 10
        mask.SetPixel(1, 0, unchecked((uint)Col(200, 200, 200))); // 掩码字节 3 = 200
        mask.SetPixel(2, 0, unchecked((uint)Col(255, 255, 255)));
        mask.SetPixel(3, 0, unchecked((uint)Col(0, 0, 0)));

        // X=2, SourceX=2 → k1=6, k2=6；但 k3 从 0 起（DIB.pas:5559）
        dst.DrawAlphaMask(src, mask, 2, 0, 2, 1, 2, 0);

        // 目标像素 2 用 mask 字节 0 = 10：B=(30*246+50*10)>>8=30；G=(20*246+100*10)>>8=23；R=(10*246+200*10)>>8=17
        AssertPx(dst, 2, 0, 17, 23, 30, "掩码下标从 0 起（DIB.pas:5559）");
        // 目标像素 3 用 mask 字节 3 = 200：B=(30*56+50*200)>>8=45；G=(20*56+100*200)>>8=82；R=(10*56+200*200)>>8=158
        AssertPx(dst, 3, 0, 158, 82, 45, "...");
        AssertPx(dst, 0, 0, 10, 20, 30, "区间外不动");
        AssertPx(dst, 1, 0, 10, 20, 30, "区间外不动");
    }

    [Fact]
    public void DrawAlphaMask_没有色键参数_Alpha255近似取源()
    {
        var dst = Mk24(1, 1);
        var src = Mk24(1, 1);
        var mask = Mk24(1, 1);
        Fill(dst, 10, 20, 30);
        Fill(src, 200, 100, 50);
        Fill(mask, 255, 255, 255);

        dst.DrawAlphaMask(src, mask, 0, 0, 1, 1, 0, 0);

        // (10*1 + 200*255) shr 8 = 51010 shr 8 = 199（整数截断，不是 200）
        AssertPx(dst, 0, 0, 199, 99, 49, "DIB.pas:5563-5565");
    }

    [Fact]
    public void DrawAlphaMask_Width为0不执行()
    {
        var dst = Mk24(2, 1);
        var src = Mk24(2, 1);
        var mask = Mk24(2, 1);
        Fill(dst, 10, 20, 30);
        Fill(src, 200, 200, 200);
        Fill(mask, 255, 255, 255);
        dst.DrawAlphaMask(src, mask, 0, 0, 0, 1, 0, 0);
        AssertPx(dst, 0, 0, 10, 20, 30, "Width=0 → for I 空转");
    }

    // =========================================================================================
    // DIB.pas 5574-5642 —— DrawMorphed
    // =========================================================================================

    private static TDIB MorphedFixture(out TDIB src)
    {
        var dst = Mk24(3, 1);
        src = Mk24(3, 1);
        dst.SetPixel(0, 0, unchecked((uint)Col(1, 1, 1)));
        dst.SetPixel(1, 0, unchecked((uint)Col(2, 2, 2)));
        dst.SetPixel(2, 0, unchecked((uint)Col(3, 3, 3)));
        src.SetPixel(0, 0, unchecked((uint)Col(9, 9, 9)));
        src.SetPixel(1, 0, 0);                       // = Color(0) → 色键
        src.SetPixel(2, 0, unchecked((uint)Col(9, 9, 9)));
        return dst;
    }

    [Fact]
    public void DrawMorphed_RGB累加器在循环外声明_永不刷新时用初值0()
    {
        var dst = MorphedFixture(out var src);
        DibFusionSupport.RandomFunc = _ => 99;   // 99 < 50 为假 → 从不刷新
        dst.DrawMorphed(src, 0, 0, 3, 1, 0, 0, 0);

        AssertPx(dst, 0, 0, 0, 0, 0, "n!=Color → 写初值 (0,0,0)");
        AssertPx(dst, 1, 0, 2, 2, 2, "n==Color → 保留目标原值");
        AssertPx(dst, 2, 0, 0, 0, 0, "...");
    }

    [Fact]
    public void DrawMorphed_只刷新过一次时_后续命中色键沿用上次刷新值()
    {
        var dst = MorphedFixture(out var src);
        int n = 0;
        DibFusionSupport.RandomFunc = _ => (n++ == 0) ? 0 : 99;  // 第一次刷新，之后不刷
        dst.DrawMorphed(src, 0, 0, 3, 1, 0, 0, 0);

        // px0 刷新 → (1,1,1)，n!=Color → 写 (1,1,1)
        AssertPx(dst, 0, 0, 1, 1, 1, "DIB.pas:5624-5628");
        // px1 n==Color → 保留目标原值 (2,2,2)（没有被 R/G/B 覆盖）
        AssertPx(dst, 1, 0, 2, 2, 2, "DIB.pas:5631");
        // px2 n!=Color → 写**上次刷新**的 (1,1,1)，而不是它自己的 (3,3,3)
        AssertPx(dst, 2, 0, 1, 1, 1, "累加器不重置（DIB.pas:5608-5610）");
    }

    [Fact]
    public void DrawMorphed_始终刷新时等价于原样回写()
    {
        var dst = MorphedFixture(out var src);
        DibFusionSupport.RandomFunc = _ => 0;    // 0 < 50 → 每像素刷新
        dst.DrawMorphed(src, 0, 0, 3, 1, 0, 0, 0);

        AssertPx(dst, 0, 0, 1, 1, 1, "刷新自身值再回写");
        AssertPx(dst, 1, 0, 2, 2, 2, "色键命中保留");
        AssertPx(dst, 2, 0, 3, 3, 3, "刷新自身值再回写");
    }

    // =========================================================================================
    // DIB.pas 5644-5710 —— DrawMono
    // =========================================================================================

    [Fact]
    public void DrawMono_色键写BackColor其余写ForeColor_通道序为BGR()
    {
        var dst = Mk24(3, 1);
        var src = Mk24(3, 1);
        Fill(dst, 0, 0, 0);
        src.SetPixel(0, 0, unchecked((uint)Col(4, 5, 6)));
        src.SetPixel(1, 0, unchecked((uint)Col(7, 8, 9)));
        src.SetPixel(2, 0, unchecked((uint)Col(4, 5, 6)));

        dst.DrawMono(src, 0, 0, 3, 1, 0, 0, Col(4, 5, 6), Col(200, 100, 50), Col(1, 2, 3));

        AssertPx(dst, 0, 0, 1, 2, 3, "TransColor → BackColor（DIB.pas:5696-5698）");
        AssertPx(dst, 1, 0, 200, 100, 50, "其余 → ForeColor（DIB.pas:5701-5703）");
        AssertPx(dst, 2, 0, 1, 2, 3, "...");
    }

    [Fact]
    public void DrawMono_全命中时整块变BackColor()
    {
        var dst = Mk24(2, 2);
        var src = Mk24(2, 2);
        Fill(dst, 0, 0, 0);
        Fill(src, 9, 9, 9);
        dst.DrawMono(src, 0, 0, 2, 2, 0, 0, Col(9, 9, 9), Col(200, 100, 50), Col(1, 2, 3));
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(dst, x, y, 1, 2, 3, "全命中");
    }

    [Fact]
    public void DrawMono_Height为0不执行()
    {
        var dst = Mk24(2, 1);
        var src = Mk24(2, 1);
        Fill(dst, 11, 22, 33);
        Fill(src, 9, 9, 9);
        dst.DrawMono(src, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        AssertPx(dst, 0, 0, 11, 22, 33, "EndY=StartY");
    }

    // =========================================================================================
    // DIB.pas 5712-5733 —— Draw3x3Matrix
    // =========================================================================================

    [Fact]
    public void Draw3x3Matrix_单位核等价于逐字节拷贝()
    {
        var dst = Mk24(4, 4);
        var src = Mk24(4, 4);
        Fill(dst, 0, 0, 0);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                src.SetPixel(x, y, unchecked((uint)Col(x * 10, y * 10, 0)));

        int[] identity = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
        dst.Draw3x3Matrix(src, identity);

        // 只处理行 1..Height-2 = 1..2，字节列 3..3*Width-4 = 8
        AssertPx(dst, 1, 1, 10, 10, 0, "DIB.pas:5716-5731");
        AssertPx(dst, 2, 1, 20, 10, 0, "字节 3..8 = 像素 1 与像素 2 的完整 6 字节");
        AssertPx(dst, 2, 2, 20, 20, 0, "...");
        AssertPx(dst, 3, 1, 0, 0, 0, "像素 3 的字节 9..11 不在 3..8 区间内");
        AssertPx(dst, 0, 1, 0, 0, 0, "列 0 不参与");
        AssertPx(dst, 1, 0, 0, 0, 0, "行 0 不参与");
        AssertPx(dst, 1, 3, 0, 0, 0, "末行不参与");
    }

    [Fact]
    public void Draw3x3Matrix_结果夹到0到255()
    {
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        Fill(dst, 0, 0, 0);
        Fill(src, 200, 200, 200);

        int[] boost = { 0, 0, 0, 0, 3, 0, 0, 0, 0, 1 };  // k = 3*200 = 600 → 夹到 255
        dst.Draw3x3Matrix(src, boost);
        AssertPx(dst, 1, 1, 255, 255, 255, "k>255 → 255（DIB.pas:5729）");

        var dst2 = Mk24(3, 3);
        Fill(dst2, 0, 0, 0);
        int[] kill = { 0, -1, 0, 0, 0, 0, 0, 0, 0, 1 };  // k = -200 → 夹到 0
        dst2.Draw3x3Matrix(src, kill);
        AssertPx(dst2, 1, 1, 0, 0, 0, "k<0 → 0（DIB.pas:5728）");
    }

    [Fact]
    public void Draw3x3Matrix_除数为0_抛异常不静默()
    {
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        int[] zero = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
        Assert.Throws<DivideByZeroException>(() => dst.Draw3x3Matrix(src, zero));
    }

    // =========================================================================================
    // DIB.pas 5735-5754 —— DrawAntialias
    // =========================================================================================

    [Fact]
    public void DrawAntialias_2x2块平均_只写第1行列起()
    {
        var dst = Mk24(2, 2);
        var src = Mk24(4, 4);
        Fill(dst, 0, 0, 0);
        Fill(src, 0, 0, 0);
        src.SetPixel(2, 2, unchecked((uint)Col(100, 0, 0)));
        src.SetPixel(3, 2, unchecked((uint)Col(0, 100, 0)));
        src.SetPixel(2, 3, unchecked((uint)Col(0, 0, 100)));
        src.SetPixel(3, 3, unchecked((uint)Col(100, 100, 100)));

        dst.DrawAntialias(src);

        // R=(100+0+0+100)>>2=50；G=(0+100+0+100)>>2=50；B=(0+0+100+100)>>2=50
        AssertPx(dst, 1, 1, 50, 50, 50, "DIB.pas:5749-5751");
        AssertPx(dst, 0, 0, 0, 0, 0, "第 0 行/列不写");
        AssertPx(dst, 1, 0, 0, 0, 0, "第 0 行不写");
        AssertPx(dst, 0, 1, 0, 0, 0, "第 0 列不写");
    }

    [Fact]
    public void DrawAntialias_源图不足时抛SScanline()
    {
        var dst = Mk24(2, 3);
        var src = Mk24(2, 2);   // Self.Height=3 → k+1 = 5 越界
        Assert.Throws<EInvalidGraphicOperation>(() => dst.DrawAntialias(src));
    }

    [Fact]
    public void DrawAntialias_目标高1时不执行()
    {
        var dst = Mk24(4, 1);
        var src = Mk24(8, 8);
        Fill(dst, 11, 22, 33);
        dst.DrawAntialias(src);
        AssertPx(dst, 0, 0, 11, 22, 33, "for I := 1 to Height-1 = 0 → 空转");
        AssertPx(dst, 3, 0, 11, 22, 33, "...");
    }

    // =========================================================================================
    // DIB.pas 5756-5786 —— FilterLine
    // =========================================================================================

    [Fact]
    public void FilterLine_长度为0直接退出()
    {
        var d = Mk24(3, 3);
        Fill(d, 10, 20, 30);
        d.FilterLine(1, 1, 1, 1, Col(200, 100, 50), TFilterMode.fmNormal);
        AssertPx(d, 1, 1, 10, 20, 30, "j=Round(Sqrt(0))=0 < 1 → Exit（DIB.pas:5764）");
    }

    [Fact]
    public void FilterLine_含两端点共j加1个像素()
    {
        var d = Mk24(4, 1);
        Fill(d, 0, 0, 0);
        d.FilterLine(0, 0, 3, 0, Col(200, 100, 50), TFilterMode.fmMix50);
        // j=3 → I=0..3 共 4 个像素
        for (int x = 0; x < 4; x++)
            AssertPx(d, x, 0, 100, 50, 25, $"(200+0)>>1=100 于 x={x}（DIB.pas:5781）");
    }

    [Fact]
    public void FilterLine_fmNormal是向Color靠拢_fmMix50是两者平均()
    {
        var dn = Mk24(2, 1);
        var d50 = Mk24(2, 1);
        Fill(dn, 0, 0, 0);
        Fill(d50, 0, 0, 0);
        dn.SetPixel(1, 0, unchecked((uint)Col(255, 255, 255)));
        d50.SetPixel(1, 0, unchecked((uint)Col(255, 255, 255)));

        dn.FilterLine(0, 0, 1, 0, Col(200, 100, 50), TFilterMode.fmNormal);
        d50.FilterLine(0, 0, 1, 0, Col(200, 100, 50), TFilterMode.fmMix50);

        // fmNormal: r = 200 + ((256-200)*0 >> 8) = 200 → 目标是黑时直接得到 Color
        AssertPx(dn, 0, 0, 200, 100, 50, "DIB.pas:5777-5779");
        // fmNormal 对白底: r = 200 + (56*255>>8 = 55) = 255
        AssertPx(dn, 1, 0, 255, 255, 255, "...");
        // fmMix50: (200+0)>>1 = 100
        AssertPx(d50, 0, 0, 100, 50, 25, "DIB.pas:5781");
        AssertPx(d50, 1, 0, 227, 177, 152, "r=(200+255)>>1=227");
    }

    [Fact]
    public void FilterLine_fmMix25与fmMix75_Color权重分别是1和3_与名字相反()
    {
        var d25 = Mk24(2, 1);
        var d75 = Mk24(2, 1);
        Fill(d25, 0, 0, 0);
        Fill(d75, 0, 0, 0);
        for (int x = 0; x < 2; x++)
        {
            d25.SetPixel(x, 0, unchecked((uint)Col(100, 100, 100)));
            d75.SetPixel(x, 0, unchecked((uint)Col(100, 100, 100)));
        }

        d25.FilterLine(0, 0, 1, 0, Col(200, 100, 50), TFilterMode.fmMix25);
        d75.FilterLine(0, 0, 1, 0, Col(200, 100, 50), TFilterMode.fmMix75);

        // fmMix25: r=(200+100*3)>>2=125, g=(100+100*3)>>2=100, b=(50+100*3)>>2=87 → Color 权重 1/4
        AssertPx(d25, 0, 0, 125, 100, 87, "DIB.pas:5780");
        // fmMix75: r=(200*3+100)>>2=175, g=(100*3+100)>>2=100, b=(50*3+100)>>2=62 → Color 权重 3/4
        AssertPx(d75, 0, 0, 175, 100, 62, "DIB.pas:5782");
    }

    [Fact]
    public void FilterLine_对角线段_按div参数化取整()
    {
        var d = Mk24(6, 6);
        Fill(d, 0, 0, 0);
        d.FilterLine(0, 0, 3, 4, Col(255, 255, 255), TFilterMode.fmMix50);
        // j = Round(Sqrt(9+16)) = 5 → I=0..5 → 6 个采样点：(0,0)(0,0)(1,1)(1,2)(2,3)(3,4)
        // (0,0) 被采样两次：第二次读到的是第一次写入后的 127 → (255+127)>>1 = 191
        AssertPx(d, 0, 0, 191, 191, 191, "同一像素被重复采样（DIB.pas:5770-5784）");
        AssertPx(d, 1, 1, 127, 127, 127, "I=2: 3*2/5=1, 4*2/5=1");
        AssertPx(d, 1, 2, 127, 127, 127, "I=3: 3*3/5=1, 4*3/5=2");
        AssertPx(d, 2, 3, 127, 127, 127, "I=4: 12/5=2, 16/5=3");
        AssertPx(d, 3, 4, 127, 127, 127, "I=5: 15/5=3, 20/5=4");
        AssertPx(d, 5, 5, 0, 0, 0, "未采样");
    }

    // =========================================================================================
    // DIB.pas 5788-5846 —— FilterRect
    // =========================================================================================

    [Fact]
    public void FilterRect_fmMix50与fmMix25与fmMix75()
    {
        var a = Mk24(1, 1);
        var b = Mk24(1, 1);
        var c = Mk24(1, 1);
        Fill(a, 10, 20, 30);
        Fill(b, 10, 20, 30);
        Fill(c, 10, 20, 30);
        int col = Col(200, 100, 50);

        a.FilterRect(0, 0, 1, 1, col, TFilterMode.fmMix50);
        b.FilterRect(0, 0, 1, 1, col, TFilterMode.fmMix25);
        c.FilterRect(0, 0, 1, 1, col, TFilterMode.fmMix75);

        AssertPx(a, 0, 0, 105, 60, 40, "(10+200)>>1=105, (20+100)>>1=60, (30+50)>>1=40");
        AssertPx(b, 0, 0, 57, 40, 35, "(3*10+200)>>2=57, (3*20+100)>>2=40, (3*30+50)>>2=35");
        AssertPx(c, 0, 0, 152, 80, 45, "(10+3*200)>>2=152, (20+3*100)>>2=80, (30+3*50)>>2=45");
    }

    [Fact]
    public void FilterRect_fmNormal先取当前像素三通道均值再按Color三通道着色()
    {
        var d = Mk24(1, 1);
        Fill(d, 10, 20, 30);   // B=30, G=20, R=10 → C1 = 60/3 = 20
        d.FilterRect(0, 0, 1, 1, Col(200, 100, 50), TFilterMode.fmNormal);
        // B=(20*50)>>8=3；G=(20*100)>>8=7；R=(20*200)>>8=15
        AssertPx(d, 0, 0, 15, 7, 3, "DIB.pas:5810-5816");
    }

    [Fact]
    public void FilterRect_尺寸为0不执行()
    {
        var d = Mk24(2, 2);
        Fill(d, 10, 20, 30);
        d.FilterRect(0, 0, 0, 0, Col(200, 100, 50), TFilterMode.fmMix50);
        AssertPx(d, 0, 0, 10, 20, 30, "Width=0/Height=0 → 空转");
    }

    [Fact]
    public void FilterRect_全白与Color全255()
    {
        var d = Mk24(2, 2);
        Fill(d, 255, 255, 255);
        d.FilterRect(0, 0, 2, 2, Col(255, 255, 255), TFilterMode.fmMix50);
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(d, x, y, 255, 255, 255, "(255+255)>>1=255");
    }

    // =========================================================================================
    // DIB.pas 5848-5858 —— InitLight
    // =========================================================================================

    [Fact]
    public void InitLight_建立256x256距离LUT()
    {
        var d = Mk24(1, 1);
        d.InitLight(0, 1);

        Assert.Equal(0, d.LightCount);
        Assert.Equal(1, d.LightDetail);

        Assert.Equal(0, d.FLUTDistValue(0, 0));     // Round(Sqrt(0))
        Assert.Equal(10, d.FLUTDistValue(1, 0));    // Round(Sqrt(100))
        Assert.Equal(10, d.FLUTDistValue(0, 1));
        Assert.Equal(50, d.FLUTDistValue(3, 4));    // Round(Sqrt(900+1600)) = 50
        Assert.Equal(3606, d.FLUTDistValue(255, 255)); // Round(Sqrt(2*2550^2)) = Round(3606.24)
    }

    [Fact]
    public void InitLight_只写LUT不触碰像素()
    {
        var d = Mk24(2, 1);
        Fill(d, 11, 22, 33);
        d.InitLight(3, 2);
        AssertPx(d, 0, 0, 11, 22, 33, "InitLight 不动像素");
        Assert.Equal(3, d.LightCount);
        Assert.Equal(2, d.LightDetail);
    }

    [Fact]
    public void InitLight_重复调用覆盖计数()
    {
        var d = Mk24(1, 1);
        d.InitLight(5, 3);
        d.InitLight(1, 0);
        Assert.Equal(1, d.LightCount);
        Assert.Equal(0, d.LightDetail);
        // LUT 每次都重算（与计数无关）
        Assert.Equal(50, d.FLUTDistValue(3, 4));
    }

    // =========================================================================================
    // DIB.pas 5860-5915 —— DrawLights
    // =========================================================================================

    /// <summary>5x5 全 100 灰、无环境光、一盏点光源（X=4,Y=4,Size=1,红）。</summary>
    private static TDIB LightsFixture(out TLightSource[] lights)
    {
        var d = Mk24(5, 5);
        Fill(d, 100, 100, 100);
        lights = new[]
        {
            new TLightSource { X = 4, Y = 4, Size1 = 1, Size2 = 1, Color = Col(255, 0, 0) }
        };
        return d;
    }

    [Fact]
    public void DrawLights_逐格提亮且行列均从1起_第0行列不被直接写()
    {
        var d = LightsFixture(out var lights);
        d.InitLight(1, 1);
        d.DrawLights(lights, Col(0, 0, 0));

        // 5 div 2 = 2 → I=2,1；行号 = 2I-o → {4,3} 与 {2,1}；行 0 从不作为"行 Y"
        // 5 div 2 = 2 → j=2,1；列号 = j*2-q → {4,3} 与 {2,1}；列 0 从不被直写
        // I=2,j=2 → D1=0,D2=0 → FLUTDist=0 → m=255 → R=254 → (100*254)>>8 = 99
        AssertPx(d, 3, 4, 99, 0, 0, "I=2,j=2：q=1 → 像素 3");
        AssertPx(d, 4, 4, 99, 0, 0, "I=2,j=2：q=0 → 像素 4");
        // I=2,j=1 → D1=2,D2=0 → FLUTDist=20 → m=235 → R=234 → 91
        AssertPx(d, 1, 4, 91, 0, 0, "I=2,j=1");
        AssertPx(d, 2, 4, 91, 0, 0, "...");
        // I=1,j=2 → D1=0,D2=2 → FLUTDist=20 → m=235 → 91
        AssertPx(d, 3, 2, 91, 0, 0, "I=1,j=2");
        AssertPx(d, 4, 1, 91, 0, 0, "I=1 的第二行（o=1）是行 1");
        // I=1,j=1 → FLUTDist[2,2]=28 → m=227 → R=226 → (100*226)>>8 = 88
        AssertPx(d, 1, 2, 88, 0, 0, "I=1,j=1");
        AssertPx(d, 2, 1, 88, 0, 0, "...");
        // 行 0 与列 0 不被直接写
        AssertPx(d, 0, 4, 100, 100, 100, "列 0（n 从 3 起）");
        AssertPx(d, 3, 0, 100, 100, 100, "行 0（行号 2I-o ≥ 1）");
    }

    [Fact]
    public void DrawLights_无光源时只剩环境光系数()
    {
        var d = Mk24(5, 5);
        Fill(d, 100, 100, 100);
        d.InitLight(0, 1);
        d.DrawLights(Array.Empty<TLightSource>(), Col(128, 64, 32));

        // R=AR=128,G=64,B=32 → 字节序 (B,G,R) = ((100*32)>>8, (100*64)>>8, (100*128)>>8) = (12,25,50)
        AssertPx(d, 3, 4, 50, 25, 12, "环境光作为三通道乘系数");
        AssertPx(d, 1, 2, 50, 25, 12, "...");
        AssertPx(d, 0, 0, 100, 100, 100, "行列 0 保持原样");
    }

    [Fact]
    public void DrawLights_行号超出图像高度时抛SScanline()
    {
        // Height=4 是 (LG_DETAIL+1)=2 的整数倍 → I 的最大值 2 把行号顶到 4
        var d = Mk24(5, 4);
        Fill(d, 100, 100, 100);
        d.InitLight(1, 1);
        var lights = new[]
        {
            new TLightSource { X = 4, Y = 4, Size1 = 1, Size2 = 1, Color = Col(255, 0, 0) }
        };
        Assert.Throws<EInvalidGraphicOperation>(() => d.DrawLights(lights, 0));
    }

    [Fact]
    public void DrawLights_光源Size为0抛除零()
    {
        var d = LightsFixture(out var lights);
        lights[0].Size1 = 0;
        d.InitLight(1, 1);
        Assert.Throws<DivideByZeroException>(() => d.DrawLights(lights, 0));
    }

    [Fact]
    public void DrawLights_LG_COUNT大于光源数组长度时越界读()
    {
        var d = LightsFixture(out var lights);
        d.InitLight(5, 1);   // 只有 1 个光源
        Assert.Throws<IndexOutOfRangeException>(() => d.DrawLights(lights, 0));
    }

    [Fact]
    public void DrawLights_宽度为步长整数倍时行尾越界写入上一行()
    {
        // Width=4 是 (LG_DETAIL+1)=2 的整数倍 → j=2,q=0 → n = 3*4 = 12 = 行宽
        // 4px@24bpp 行宽 12 → 行 Y 的字节 12..14 正是**上一行**的像素 0
        var d = Mk24(4, 5);
        Fill(d, 100, 100, 100);
        d.InitLight(1, 1);
        var lights = new[]
        {
            new TLightSource { X = 4, Y = 4, Size1 = 1, Size2 = 1, Color = Col(255, 0, 0) }
        };

        d.DrawLights(lights, Col(0, 0, 0));   // 原文无列边界检查 → 不应抛

        AssertPx(d, 1, 4, 91, 0, 0, "j=1 → 像素 1");
        AssertPx(d, 3, 4, 99, 0, 0, "j=2,q=1 → 像素 3");
        AssertPx(d, 0, 4, 100, 100, 100, "行 4 的像素 0 不被直写（行 5 不存在）");
        // 行 1 的越界写落到行 0 的像素 0；I=1,j=2 时 R=234 → (100*234)>>8 = 91
        AssertPx(d, 0, 0, 91, 0, 0, "行尾越界写进上一行的像素 0（DIB.pas:5901-5907）");
    }

    // =========================================================================================
    // DIB.pas 5938-5966 —— Darkness
    // =========================================================================================

    [Fact]
    public void Darkness_非24bpp直接退出()
    {
        foreach (int bc in new[] { 1, 4, 8, 16, 32 })
        {
            var d = new TDIB();
            if (bc == 16) d.PixelFormat = DIB.MakeDIBPixelFormat(5, 6, 5);
            d.SetSize(2, 1, bc);
            d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
            uint before = d.GetPixel(0, 0);

            d.Darkness(255);

            // 位深校验通过且非 24bpp → 直接 Exit（DIB.pas:5954），像素与内容不变
            Assert.Equal(before, d.GetPixel(0, 0));
            Assert.Equal(bc, d.BitCount);
        }
    }

    [Fact]
    public void Darkness_公式为字节减字节乘Amount除255()
    {
        var d = Mk24(1, 1);
        Fill(d, 100, 100, 100);

        d.Darkness(51);
        // 100 - (100*51) div 255 = 100 - 20 = 80
        AssertPx(d, 0, 0, 80, 80, 80, "DIB.pas:5961-5963");

        var d2 = Mk24(1, 1);
        Fill(d2, 100, 100, 100);
        d2.Darkness(255);
        AssertPx(d2, 0, 0, 0, 0, 0, "255 → 全黑");

        var d3 = Mk24(1, 1);
        Fill(d3, 100, 100, 100);
        d3.Darkness(0);
        AssertPx(d3, 0, 0, 100, 100, 100, "0 → 不变");
    }

    [Fact]
    public void Darkness_三个通道各自独立()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 50, 10)));
        d.Darkness(51);
        // R: 200 - (200*51)div255 = 200-40 = 160
        // G:  50 - (50*51)div255  =  50-10 =  40
        // B:  10 - (10*51)div255  =  10- 2 =   8
        AssertPx(d, 0, 0, 160, 40, 8, "Amount 对三通道同式作用");
    }

    [Fact]
    public void Darkness_负数Amount反而提亮_超界被夹()
    {
        var d = Mk24(1, 1);
        Fill(d, 100, 100, 100);
        d.Darkness(-255);   // 100 - (100*-255)div255 = 100+100 = 200
        AssertPx(d, 0, 0, 200, 200, 200, "负 Amount 提亮（DIB.pas:5961）");

        var d2 = Mk24(1, 1);
        Fill(d2, 100, 100, 100);
        d2.Darkness(1000);  // 100 - 392 = -292 → IntToByte 夹到 0
        AssertPx(d2, 0, 0, 0, 0, 0, "IntToByte 下界夹取");
    }

    [Fact]
    public void Darkness_多像素逐行处理()
    {
        var d = Mk24(2, 2);
        Fill(d, 255, 0, 128);
        d.Darkness(51);
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(d, x, y, 204, 0, 103, "(255-51=204；128-(128*51 div 255)=128-25=103)");
    }

    // =========================================================================================
    // DIB.pas 5975-6036 —— DoSmoothRotate
    // =========================================================================================

    [Fact]
    public void DoSmoothRotate_角度0且同尺寸等价于恒等拷贝()
    {
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        Fill(dst, 0, 0, 0);
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                src.SetPixel(x, y, unchecked((uint)Col(x * 10, y * 20, 30)));

        dst.DoSmoothRotate(src, 0, 0, 0);

        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                AssertPx(dst, x, y, x * 10, y * 20, 30, $"恒等采样 ({x},{y})");
    }

    [Fact]
    public void DoSmoothRotate_角度0时cxcy不影响结果()
    {
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        Fill(dst, 1, 2, 3);
        Fill(src, 200, 100, 50);

        dst.DoSmoothRotate(src, 1, 1, 0);

        // fx = ((px*1 - py*0) - 1)/2 + cx - xDiff = (px-1)/2 + cx = (X-cx) + cx = X
        AssertPx(dst, 0, 0, 200, 100, 50, "cx/cy 在角度 0 时抵消");
        AssertPx(dst, 2, 2, 200, 100, 50, "...");
    }

    [Fact]
    public void DoSmoothRotate_尺寸不等时按xDiffyDiff平移且越界采样不写()
    {
        var dst = Mk24(4, 4);
        var src = Mk24(2, 2);
        Fill(dst, 7, 7, 7);
        src.SetPixel(0, 0, unchecked((uint)Col(1, 1, 1)));
        src.SetPixel(1, 0, unchecked((uint)Col(2, 2, 2)));
        src.SetPixel(0, 1, unchecked((uint)Col(3, 3, 3)));
        src.SetPixel(1, 1, unchecked((uint)Col(4, 4, 4)));

        dst.DoSmoothRotate(src, 0, 0, 0);

        AssertPx(dst, 1, 1, 1, 1, 1, "xDiff=yDiff=1（DIB.pas:5990-5991）");
        AssertPx(dst, 2, 1, 2, 2, 2, "...");
        AssertPx(dst, 1, 2, 3, 3, 3, "...");
        AssertPx(dst, 2, 2, 4, 4, 4, "...");
        AssertPx(dst, 0, 0, 7, 7, 7, "越界采样 → 整像素不写（DIB.pas:6002）");
        AssertPx(dst, 3, 3, 7, 7, 7, "fx=2 超出 Src.Width → 不写");
    }

    [Fact]
    public void DoSmoothRotate_90度把源前两行转置到目标前两列()
    {
        // Angle=-90° → sAngle=-1, cAngle≈0；cx=cy=1 ⇒ fx=Y, fy=1-X
        var dst = Mk24(3, 3);
        var src = Mk24(3, 3);
        Fill(dst, 7, 7, 7);
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                src.SetPixel(x, y, unchecked((uint)Col(x * 10, y * 20, 0)));

        dst.DoSmoothRotate(src, 1, 1, 90);

        AssertPx(dst, 0, 0, 0, 20, 0, "← Src(0,1)");
        AssertPx(dst, 0, 1, 10, 20, 0, "← Src(1,1)");
        AssertPx(dst, 0, 2, 20, 20, 0, "← Src(2,1)");
        AssertPx(dst, 1, 0, 0, 0, 0, "← Src(0,0)");
        AssertPx(dst, 1, 1, 10, 0, 0, "← Src(1,0)");
        AssertPx(dst, 1, 2, 20, 0, 0, "← Src(2,0)");
        AssertPx(dst, 2, 0, 7, 7, 7, "fy=-1 → 不写");
    }

    // =========================================================================================
    // DIB.pas 6042-6063 —— DoInvert
    // =========================================================================================

    [Fact]
    public void DoInvert_逐字节按位取反()
    {
        var d = Mk24(2, 2);
        Fill(d, 10, 20, 30);
        d.DoInvert();
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(d, x, y, 245, 235, 225, "not v = 255-v（DIB.pas:6055-6057）");
    }

    [Fact]
    public void DoInvert_两次还原()
    {
        var d = Mk24(2, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(1, 2, 3)));
        d.SetPixel(1, 0, unchecked((uint)Col(200, 150, 100)));
        d.DoInvert();
        d.DoInvert();
        AssertPx(d, 0, 0, 1, 2, 3, "对合");
        AssertPx(d, 1, 0, 200, 150, 100, "...");
    }

    [Fact]
    public void DoInvert_空图先被SetBitCount撑成1x1_24bpp()
    {
        var d = new TDIB();
        Assert.True(d.Empty);
        d.DoInvert();
        // w/h 在 SetBitCount **之前**取（都是 0）⇒ 循环空转；仅留下 SetBitCount(24) 的副作用
        Assert.Equal(1, d.Width);
        Assert.Equal(1, d.Height);
        Assert.Equal(24, d.BitCount);
    }

    // =========================================================================================
    // DIB.pas 6065-6124 —— DoAddColorNoise / DoAddMonoNoise
    // =========================================================================================

    [Fact]
    public void DoAddColorNoise_Amount0时零噪声()
    {
        var d = Mk24(2, 2);
        Fill(d, 100, 100, 100);
        d.DoAddColorNoise(0);
        // Random(0)=0、Amount shr 1=0
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(d, x, y, 100, 100, 100, "DIB.pas:6076-6081");
    }

    [Fact]
    public void DoAddColorNoise_噪声取值范围为负Amount除2到0()
    {
        var d = Mk24(16, 16);
        Fill(d, 100, 100, 100);
        d.DoAddColorNoise(2);   // Random(2) ∈ {0,1}，减去 (2 shr 1)=1 ⇒ 偏移 ∈ {-1,0}
        var seen = new HashSet<int>();
        for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
            {
                var (R, G, B) = Px(d, x, y);
                foreach (int v in new[] { R, G, B })
                {
                    Assert.InRange(v, 99, 100);
                    seen.Add(v);
                }
            }
        // 回归断言：修复 DibEffectsSupport.DibRandom 的 `(int)r * 0` 前，偏移恒为 -1（只见到 99）
        Assert.Contains(99, seen);
        Assert.Contains(100, seen);
    }

    [Fact]
    public void DoAddColorNoise_三通道独立取随机数()
    {
        var d = Mk24(16, 16);
        Fill(d, 100, 100, 100);
        d.DoAddColorNoise(2);
        bool sawMixed = false;
        for (int y = 0; y < 16 && !sawMixed; y++)
            for (int x = 0; x < 16; x++)
            {
                var (R, G, B) = Px(d, x, y);
                if (R != G || G != B) { sawMixed = true; break; }
            }
        Assert.True(sawMixed, "ColorNoise 每通道各调一次 Random（DIB.pas:6076-6078）");
    }

    [Fact]
    public void DoAddMonoNoise_三通道共用同一个偏移()
    {
        var d = Mk24(16, 16);
        Fill(d, 100, 100, 100);
        d.DoAddMonoNoise(2);
        for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
            {
                var (R, G, B) = Px(d, x, y);
                Assert.True(R == G && G == B, $"MonoNoise 同像素三通道偏移相同 @({x},{y}) 得 ({R},{G},{B})");
                Assert.InRange(R, 99, 100);
            }
    }

    [Fact]
    public void DoAddMonoNoise_Amount1时只可能是减0或减0_即不变()
    {
        var d = Mk24(4, 1);
        Fill(d, 50, 50, 50);
        d.DoAddMonoNoise(1);   // Random(1)=0（Range=1 ⇒ r*1 截断为 0）；(1 shr 1)=0 ⇒ 偏移 0
        for (int x = 0; x < 4; x++)
            AssertPx(d, x, 0, 50, 50, 50, "Random(1) 恒 0");
    }

    // =========================================================================================
    // DIB.pas 6126-6155 —— DoAntiAlias
    // =========================================================================================

    [Fact]
    public void DoAntiAlias_均匀图不变且只处理内区()
    {
        var d = Mk24(4, 4);
        Fill(d, 100, 100, 100);
        d.DoAntiAlias();
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                AssertPx(d, x, y, 100, 100, 100, "十字均值仍为 100");
    }

    [Fact]
    public void DoAntiAlias_3x3十字均值且X递增有顺序依赖()
    {
        var d = Mk24(4, 4);
        Fill(d, 0, 0, 0);
        d.SetPixel(2, 2, unchecked((uint)Col(255, 0, 0)));   // 只设 R 通道

        d.DoAntiAlias();

        // X ∈ [1, Width-2] = [1,2]、Y ∈ [1, Height-2] = [1,2]
        // 双向依赖（原文就地改写，必须照抄循环方向）：
        //   * Y 递增 ⇒ Y=2 的 p0 是**已被 Y=1 改写过的**行 1；
        //   * X 递增 ⇒ X=2 的 p1[(X-1)*3+2] 是 X=1 刚写入的值。
        // Y=1（p1=行1、P2=行2 含唯一非零 R=255）:
        //   X=1 → 行1 像素1 R = (0+0+0+0)/4 = 0
        //   X=2 → 行1 像素2 R = (row0[8]=0 + row2[8]=255 + row1[5]=0 + row1[11]=0)/4 = 63
        // Y=2（p0=行1(已改)、p1=行2、P2=行3）:
        //   X=1 → 行2 像素1 R = (row1[5]=0 + row3[5]=0 + row2[2]=0 + row2[8]=255)/4 = 63
        //   X=2 → 行2 像素2 R = (row1[8]=63 + row3[8]=0 + row2[5]=63 + row2[11]=0)/4 = 126/4 = 31
        AssertPx(d, 1, 2, 63, 0, 0, "DIB.pas:6146 + X 递增读到刚写入的 p1[(X-1)*3+2]");
        AssertPx(d, 2, 2, 31, 0, 0, "同时受 Y 方向依赖影响：p0 是已被改写的行 1");
        AssertPx(d, 2, 1, 63, 0, 0, "Y=1 的 X=2 从 P2（行2）读到 255");
        AssertPx(d, 1, 1, 0, 0, 0, "Y=1 的 X=1 全零");
        // 边界不被触碰
        AssertPx(d, 0, 0, 0, 0, 0, "XOrigin=Max(1,0)=1");
        AssertPx(d, 3, 3, 0, 0, 0, "XFinal=Min(Width-2,…)=2");
        AssertPx(d, 0, 2, 0, 0, 0, "第 0 列不处理");
        AssertPx(d, 2, 0, 0, 0, 0, "第 0 行不处理");
    }

    [Fact]
    public void DoAntiAlias_空图与1x1图不崩()
    {
        var e = new TDIB();
        e.DoAntiAlias();                 // SetBitCount(24) 撑成 1x1，循环空转
        Assert.Equal(24, e.BitCount);
        Assert.Equal(1, e.Width);

        var one = Mk24(1, 1);
        Fill(one, 5, 6, 7);
        one.DoAntiAlias();               // XFinal = Min(-1, 1) = -1 < XOrigin=1 → 空转
        AssertPx(one, 0, 0, 5, 6, 7, "尺寸过小不写");
    }

    // =========================================================================================
    // DIB.pas 6157-6191 —— DoContrast
    // =========================================================================================

    [Fact]
    public void DoContrast_以127为中心按Abs127减ch乘Amount除255拉开()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));   // 内存序 B=50,G=100,R=200
        d.DoContrast(255);
        // B=50  ≤127 → 50 - (77*255)/255 = 50-77 = -27 → 0
        // G=100 ≤127 → 100 - 27 = 73
        // R=200 > 127 → 200 + 73 = 273 → IntToByte → 255
        AssertPx(d, 0, 0, 255, 73, 0, "DIB.pas:6171-6179");
    }

    [Fact]
    public void DoContrast_恰好127走减分支且Amount0不变()
    {
        var d = Mk24(1, 1);
        Fill(d, 127, 127, 127);
        d.DoContrast(255);
        AssertPx(d, 0, 0, 127, 127, 127, "|127-127|=0 ⇒ 两分支都等价");

        var d2 = Mk24(1, 1);
        Fill(d2, 200, 100, 50);
        d2.DoContrast(0);
        AssertPx(d2, 0, 0, 200, 100, 50, "Amount=0 不变");
    }

    [Fact]
    public void DoContrast_负Amount反向压缩()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoContrast(-255);
        // B=50 → 50 - (77*-255)/255 = 50+77 = 127
        // G=100 → 100 + 27 = 127
        // R=200 → 200 + (73*-255)/255 = 200-73 = 127
        AssertPx(d, 0, 0, 127, 127, 127, "负 Amount 把所有值压向 127");
    }

    // =========================================================================================
    // DIB.pas 6193-6302 —— DoFishEye
    // =========================================================================================

    [Fact]
    public void DoFishEye_Amount0时全图采样中心一点()
    {
        var d = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                d.SetPixel(x, y, unchecked((uint)Col(x * 10, y * 10, 0)));
        // 中心 (xmid,ymid) = (2,2) → Col(20,20,0)

        d.DoFishEye(0);

        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                AssertPx(d, x, y, 20, 20, 0, "rmax=0 ⇒ r2=0 ⇒ 全部取 (xmid,ymid)（DIB.pas:6226）");
    }

    [Fact]
    public void DoFishEye_中心像素不被移动()
    {
        var d = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                d.SetPixel(x, y, unchecked((uint)Col(x * 10, y * 10, 0)));

        d.DoFishEye(1);

        // (2,2)：dx=dy=0、r1=0 → 直接取 (xmid,ymid) = (2,2)
        AssertPx(d, 2, 2, 20, 20, 0, "r1=0 特例（DIB.pas:6221-6223）");
    }

    [Fact]
    public void DoFishEye_尺寸为1且Amount非0时不崩()
    {
        var d = Mk24(1, 1);
        Fill(d, 9, 8, 7);
        d.DoFishEye(1);   // xmid=ymid=0.5 → ifx=0,ify=0
        AssertPx(d, 0, 0, 9, 8, 7, "1x1 采样自身");
    }

    // =========================================================================================
    // DIB.pas 6304-6328 —— DoGrayScale
    // =========================================================================================

    [Fact]
    public void DoGrayScale_按内存序B03G059R011加权()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));   // B=50,G=100,R=200
        d.DoGrayScale();
        // Round(50*0.3 + 100*0.59 + 200*0.11) = Round(15+59+22) = 96
        AssertPx(d, 0, 0, 96, 96, 96, "DIB.pas:6313");
    }

    [Fact]
    public void DoGrayScale_全白得255而非254()
    {
        var d = Mk24(1, 1);
        Fill(d, 255, 255, 255);
        d.DoGrayScale();
        AssertPx(d, 0, 0, 255, 255, 255, "权重和为 1，浮点误差被 Round 吞掉");
    }

    [Fact]
    public void DoGrayScale_全黑与多像素()
    {
        var d = Mk24(2, 2);
        Fill(d, 0, 0, 0);
        d.DoGrayScale();
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(d, x, y, 0, 0, 0, "全黑不变");
    }

    // =========================================================================================
    // DIB.pas 6330-6356 —— DoLightness
    // =========================================================================================

    [Fact]
    public void DoLightness_向白靠拢()
    {
        var d = Mk24(1, 1);
        Fill(d, 100, 100, 100);
        d.DoLightness(51);
        // 100 + (155*51)/255 = 100 + 31 = 131
        AssertPx(d, 0, 0, 131, 131, 131, "DIB.pas:6342-6344");
    }

    [Fact]
    public void DoLightness_Amount255全变白_Amount0不变()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoLightness(255);
        AssertPx(d, 0, 0, 255, 255, 255, "255 + 0");

        var d2 = Mk24(1, 1);
        d2.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d2.DoLightness(0);
        AssertPx(d2, 0, 0, 200, 100, 50, "Amount=0 不变");
    }

    [Fact]
    public void DoLightness_负Amount变暗到0()
    {
        var d = Mk24(1, 1);
        Fill(d, 100, 100, 100);
        d.DoLightness(-255);
        // 100 + (155*-255)/255 = 100-155 = -55 → IntToByte → 0
        AssertPx(d, 0, 0, 0, 0, 0, "IntToByte 下界夹取");
    }

    // =========================================================================================
    // DIB.pas 6358-6367 —— DoDarkness
    // =========================================================================================

    [Fact]
    public void DoDarkness_委托给Darkness()
    {
        var d = Mk24(1, 1);
        Fill(d, 100, 100, 100);
        d.DoDarkness(51);
        // 100 - (100*51)/255 = 80
        AssertPx(d, 0, 0, 80, 80, 80, "DIB.pas:6364 → 5961-5963");
    }

    [Fact]
    public void DoDarkness_非24bpp时Assign后位深回落_故不做任何事()
    {
        var d = new TDIB();
        d.SetSize(2, 1, 8);
        d.SetPixel(0, 0, 200);
        uint before = d.GetPixel(0, 0);
        d.DoDarkness(255);
        // BB.BitCount := 24 被随后的 BB.Assign(Self) 覆盖 → Darkness 直接 Exit
        Assert.Equal(8, d.BitCount);
        Assert.Equal(before, d.GetPixel(0, 0));
    }

    [Fact]
    public void DoDarkness_Amount0不变()
    {
        var d = Mk24(2, 1);
        Fill(d, 100, 100, 100);
        d.DoDarkness(0);
        AssertPx(d, 0, 0, 100, 100, 100, "Amount=0");
        AssertPx(d, 1, 0, 100, 100, 100, "...");
    }

    // =========================================================================================
    // DIB.pas 6369-6396 —— DoSaturation
    // =========================================================================================

    [Fact]
    public void DoSaturation_Amount255是恒等()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoSaturation(255);
        // Gray=116；每通道 116 + ((ch-116)*255)/255 = ch
        AssertPx(d, 0, 0, 200, 100, 50, "DIB.pas:6382-6384");
    }

    [Fact]
    public void DoSaturation_Amount0全部变灰()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoSaturation(0);
        // Gray = (50+100+200)/3 = 116
        AssertPx(d, 0, 0, 116, 116, 116, "Gray 是整数除");
    }

    [Fact]
    public void DoSaturation_Amount128_负差值走向零截断的div()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoSaturation(128);
        // B=50  ：116 + (-66*128)/255 = 116 + (-8448/255 = -33) = 83
        // G=100 ：116 + (-16*128)/255 = 116 + (-2048/255 =  -8) = 108
        // R=200 ：116 + ( 84*128)/255 = 116 + (10752/255 =  42) = 158
        AssertPx(d, 0, 0, 158, 108, 83, "div 向零截断（不是 shr）");
    }

    // =========================================================================================
    // DIB.pas 6398-6445 —— DoSplitBlur
    // =========================================================================================

    private static TDIB SplitBlurFixture()
    {
        var d = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                d.SetPixel(x, y, unchecked((uint)Col(x + y * 10, 0, 0)));
        return d;
    }

    [Fact]
    public void DoSplitBlur_Amount0不变()
    {
        var d = SplitBlurFixture();
        d.DoSplitBlur(0);
        AssertPx(d, 0, 0, 0, 0, 0, "Amount=0 → Exit（DIB.pas:6406）");
        AssertPx(d, 3, 3, 33, 0, 0, "...");
    }

    [Fact]
    public void DoSplitBlur_四角均值且越界反折与X递增依赖()
    {
        var d = SplitBlurFixture();
        d.DoSplitBlur(1);
        // Y=0: p1=行0（=p0 自身）、P2=行1；cx 左 = X-1 取**已改写**的左邻
        // X=0: (r0p0=0 + r1p0=10 + r0p1=1 + r1p1=11)/4 = 22/4 = 5
        // X=1: (r0p0(已改)=5 + r1p0=10 + r0p2=2 + r1p2=12)/4 = 29/4 = 7
        // X=2: (r0p1(已改)=7 + r1p1=11 + r0p3=3 + r1p3=13)/4 = 34/4 = 8
        // X=3: (r0p2(已改)=8 + r1p2=12 + 右越界 → cx = Width-X = 1 → r0p1(已改)=7 + r1p1=11)/4 = 38/4 = 9
        AssertPx(d, 0, 0, 5, 0, 0, "DIB.pas:6431-6433（R 通道）");
        AssertPx(d, 1, 0, 7, 0, 0, "...");
        AssertPx(d, 2, 0, 8, 0, 0, "...");
        AssertPx(d, 3, 0, 9, 0, 0, "右越界用 cx := clip.Width - X（DIB.pas:6424）");
        // Y=1: p1=行0、P2=行2、p0=行1
        // X=0: (r0p0=5 + r2p0=20 + r0p1=7 + r2p1=21)/4 = 53/4 = 13
        AssertPx(d, 0, 1, 13, 0, 0, "Y=1 读的是已被改写的行 0");
    }

    [Fact]
    public void DoSplitBlur_Amount不小于Height时Y0行号越界抛SScanline()
    {
        var d = SplitBlurFixture();
        // Y=0 → Y+Amount >= Height → P2 := ScanLine(Height - 0) = ScanLine(4) → 越界
        Assert.Throws<EInvalidGraphicOperation>(() => d.DoSplitBlur(4));
    }

    // =========================================================================================
    // DIB.pas 6447-6457 —— DoGaussianBlur
    // =========================================================================================

    [Fact]
    public void DoGaussianBlur_均匀图不变()
    {
        var d = Mk24(4, 4);
        Fill(d, 100, 100, 100);
        d.DoGaussianBlur(3);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                AssertPx(d, x, y, 100, 100, 100, "转调 GaussianBlur→SplitBlur（DIB.pas:4705-4706）");
    }

    [Fact]
    public void DoGaussianBlur_Amount0不变()
    {
        var d = Mk24(2, 1);
        Fill(d, 10, 20, 30);
        d.DoGaussianBlur(0);
        AssertPx(d, 0, 0, 10, 20, 30, "for I := 1 to 0 空转");
    }

    [Fact]
    public void DoGaussianBlur_非24bpp时SplitBlur直接退出()
    {
        var d = new TDIB();
        d.SetSize(4, 4, 8);
        d.SetPixel(0, 0, 123);
        uint before = d.GetPixel(0, 0);
        d.DoGaussianBlur(2);
        Assert.Equal(8, d.BitCount);   // BB.BitCount := 24 被 Assign 覆盖
        Assert.Equal(before, d.GetPixel(0, 0));
    }

    // =========================================================================================
    // DIB.pas 6459-6502 —— DoMosaic
    // =========================================================================================

    private static TDIB MosaicFixture()
    {
        var d = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                d.SetPixel(x, y, unchecked((uint)Col(x + y * 10, 0, 0)));
        return d;
    }

    [Fact]
    public void DoMosaic_按Size做水平游程填充且色取自游程起点()
    {
        var d = MosaicFixture();
        d.DoMosaic(2);
        // 关键：6476-6478 的 R/G/B 读在**最内层 repeat 之外** ⇒ 一次读、连续写 Size 个像素；
        // 被覆盖的位置**不会**先被读回 ⇒ 游程色恒为游程起点的颜色。
        // 行 0（p1 = 行 0）：run(0..1) 用 p0 → [0,0]；run(2..3) 用 p2 → [2,2]
        AssertPx(d, 0, 0, 0, 0, 0, "run 起点");
        AssertPx(d, 1, 0, 0, 0, 0, "被 run 起点颜色覆盖（原值 1）");
        AssertPx(d, 2, 0, 2, 0, 0, "下一 run 的起点");
        AssertPx(d, 3, 0, 2, 0, 0, "被覆盖（原值 3）");
        // j=2 的 P2 = 行 1，但 p1 仍是行 0 ⇒ 行 1 也变成行 0 的游程结果
        AssertPx(d, 0, 1, 0, 0, 0, "DIB.pas:6473 + 6489（Y 被内层推进）");
        AssertPx(d, 2, 1, 2, 0, 0, "...");
        // 最外层第二轮：p1 = 行 2（未被前面改过）
        AssertPx(d, 0, 2, 20, 0, 0, "第二轮 p1 = 行 2");
        AssertPx(d, 1, 2, 20, 0, 0, "...");
        AssertPx(d, 2, 2, 22, 0, 0, "...");
        AssertPx(d, 0, 3, 20, 0, 0, "行 3 ← 行 2 的游程结果（原值 30）");
        AssertPx(d, 3, 3, 22, 0, 0, "...");
    }

    [Fact]
    public void DoMosaic_Size1与Size0为恒等()
    {
        var d1 = MosaicFixture();
        d1.DoMosaic(1);
        var d0 = MosaicFixture();
        d0.DoMosaic(0);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
            {
                AssertPx(d1, x, y, x + y * 10, 0, 0, "Size=1 → 游程长度 1");
                AssertPx(d0, x, y, x + y * 10, 0, 0, "Size=0 → I > 0 立即成立，游程长度 1");
            }
    }

    [Fact]
    public void DoMosaic_单行图不崩()
    {
        var d = Mk24(3, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(1, 2, 3)));
        d.SetPixel(1, 0, unchecked((uint)Col(4, 5, 6)));
        d.SetPixel(2, 0, unchecked((uint)Col(7, 8, 9)));
        d.DoMosaic(2);
        AssertPx(d, 0, 0, 1, 2, 3, "游程 0..1 取 p0");
        AssertPx(d, 1, 0, 1, 2, 3, "被 p0 覆盖（原值 (4,5,6)）");
        AssertPx(d, 2, 0, 7, 8, 9, "第二个游程的起点");
    }

    // =========================================================================================
    // DIB.pas 6504-6642 —— DoTwist
    // =========================================================================================

    [Fact]
    public void DoTwist_均匀图在重采样后不变()
    {
        var d = Mk24(4, 4);
        Fill(d, 77, 88, 99);
        d.DoTwist(3);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                AssertPx(d, x, y, 77, 88, 99, "双线性权重和为 1 ⇒ 均匀图恒等");
    }

    [Fact]
    public void DoTwist_Amount0时R除0得Inf经Trunc抛异常()
    {
        var d = Mk24(4, 4);
        Fill(d, 10, 20, 30);
        // R / Amount = +Inf ⇒ Cos(Inf)=NaN ⇒ Trunc(NaN) ⇒ ArithmeticException（Delphi EInvalidOp）
        Assert.Throws<ArithmeticException>(() => d.DoTwist(0));
    }

    [Fact]
    public void DoTwist_小图时循环上界未被夹住会越界()
    {
        // 3x3：R = Sqrt(4+4) = 2.828 < Height=3 ⇒ ty2 不被夹到 2 ⇒ Round(2.828)=3 ⇒ ScanLine(3) 越界
        var d = Mk24(3, 3);
        Fill(d, 10, 20, 30);
        Assert.Throws<EInvalidGraphicOperation>(() => d.DoTwist(1));
    }

    // =========================================================================================
    // DIB.pas 6644-6790 —— DoTrace
    // =========================================================================================

    [Fact]
    public void DoTrace_未装载Canvas接缝时影子图恒0故原图不变()
    {
        var d = Mk24(4, 4);
        Fill(d, 10, 20, 30);
        d.DoTrace(2);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                AssertPx(d, x, y, 10, 20, 30, "8bpp 影子图未接线 ⇒ 全部比较为假（DIB.pas:6656）");
    }

    [Fact]
    public void DoTrace_装载Canvas接缝时取一次影子绘制且原图仍不变()
    {
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;
        var d = Mk24(4, 4);
        Fill(d, 10, 20, 30);

        d.DoTrace(2);

        Assert.Equal(1, canvas.DrawCount);
        AssertPx(d, 0, 0, 10, 20, 30, "影子图仍是全 0（假接缝不写像素）⇒ 无描边");
    }

    [Fact]
    public void DoTrace_intensity0时不进入循环()
    {
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;
        var d = Mk24(2, 2);
        Fill(d, 1, 2, 3);
        d.DoTrace(0);
        Assert.Equal(1, canvas.DrawCount);   // 影子图仍被建/绘
        AssertPx(d, 1, 1, 1, 2, 3, "for I := 1 to 0 空转");
    }

    // =========================================================================================
    // DIB.pas 6792-6825 —— DoSplitlight
    // =========================================================================================

    [Fact]
    public void DoSplitlight_0与255是曲线的不动点()
    {
        var d = Mk24(2, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(0, 255, 128)));
        d.SetPixel(1, 0, unchecked((uint)Col(255, 0, 0)));
        var (_, g0, _) = Px(d, 0, 0);
        d.DoSplitlight(1);
        // 内存序 (B,G,R)：px0 = (128,255,0) → 128 会被提亮，255/0 不动
        AssertPx(d, 1, 0, 255, 0, 0, "0 与 255 是 sin 曲线的端点（DIB.pas:6800）");
        Assert.InRange(Px(d, 0, 0).B, 140, 200);
        Assert.Equal(255, g0);
    }

    [Fact]
    public void DoSplitlight_Amount0不变()
    {
        var d = Mk24(2, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.SetPixel(1, 0, unchecked((uint)Col(10, 20, 30)));
        d.DoSplitlight(0);
        AssertPx(d, 0, 0, 200, 100, 50, "for I := 1 to 0 空转");
        AssertPx(d, 1, 0, 10, 20, 30, "...");
    }

    [Fact]
    public void DoSplitlight_多次施加单调提亮()
    {
        var d1 = Mk24(1, 1);
        Fill(d1, 128, 128, 128);
        d1.DoSplitlight(1);
        var one = Px(d1, 0, 0).R;

        var d2 = Mk24(1, 1);
        Fill(d2, 128, 128, 128);
        d2.DoSplitlight(2);
        var two = Px(d2, 0, 0).R;

        Assert.InRange(one, 150, 200);
        Assert.True(two > one, $"复合应更亮：{one} → {two}");
        Assert.True(two <= 255, "上界 255");
    }

    // =========================================================================================
    // DIB.pas 6827-6913 —— DoTile
    // =========================================================================================

    [Fact]
    public void DoTile_图太小直接早退_尺寸保持()
    {
        var d = Mk24(8, 8);
        Fill(d, 10, 20, 30);
        d.DoTile(2);   // 8 div 2 = 4 < 5 → Exit
        Assert.Equal(8, d.Width);
        Assert.Equal(8, d.Height);
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                AssertPx(d, x, y, 10, 20, 30, "早退条件 (w div Amount) < 5（DIB.pas:6888）");
    }

    [Fact]
    public void DoTile_Amount非正直接早退()
    {
        var d = Mk24(20, 20);
        Fill(d, 7, 8, 9);
        d.DoTile(0);
        AssertPx(d, 0, 0, 7, 8, 9, "Amount <= 0 → Exit");
        d.DoTile(-1);
        AssertPx(d, 0, 0, 7, 8, 9, "Amount < 0 → Exit");
    }

    [Fact]
    public void DoTile_满足条件时走SmoothResize但平铺靠接缝故像素不变()
    {
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;
        var d = Mk24(10, 10);
        for (int y = 0; y < 10; y++)
            for (int x = 0; x < 10; x++)
                d.SetPixel(x, y, unchecked((uint)Col(x * 20, y * 20, 0)));

        d.DoTile(2);   // 10 div 2 = 5 ≥ 5 → 进入 SmoothResize 与 4 次平铺 Draw

        Assert.Equal(10, d.Width);
        // 1 次 Tile 内的 Dst.Canvas.Draw(0,0,Src) + Amount^2 = 4 次平铺 Draw
        Assert.Equal(5, canvas.DrawCount);
        AssertPx(d, 0, 0, 0, 0, 0, "平铺写的是 Dst 的接缝（GDI），无头时不改像素");
    }

    // =========================================================================================
    // DIB.pas 6915-6958 —— DoSpotLight
    // =========================================================================================

    private sealed class FakeSpotSeam : IDibFusionSpotSeam
    {
        public readonly List<int> BrushColors = new();
        public readonly List<(int, int, int, int)> Rects = new();
        public readonly List<(int, int, int, int)> Ellipses = new();
        public readonly List<(TDIB bmp, bool value)> Transparency = new();
        public readonly List<(TDIB dib, uint value)> CopyModes = new();

        public void SetBrushColor(int Color) => BrushColors.Add(Color);
        public void FillRect(int Left, int Top, int Right, int Bottom) => Rects.Add((Left, Top, Right, Bottom));
        public void Ellipse(int X1, int Y1, int X2, int Y2) => Ellipses.Add((X1, Y1, X2, Y2));
        public void SetBitmapTransparent(TDIB Bmp, bool Value) => Transparency.Add((Bmp, Value));
        public void SetCanvasCopyMode(TDIB Dib, uint Value) => CopyModes.Add((Dib, Value));
    }

    [Fact]
    public void DoSpotLight_绘制接缝未装载时抛异常()
    {
        var d = Mk24(4, 4);
        Fill(d, 10, 20, 30);
        Assert.Throws<InvalidOperationException>(() => d.DoSpotLight(50, TDxRect.Rect(0, 0, 2, 2)));
    }

    [Fact]
    public void DoSpotLight_原文把结果画进临时z后即释放_故对自身是空操作()
    {
        var spot = new FakeSpotSeam();
        DibFusionSpot.Seam = spot;
        DibFusionCanvas.Seam = new FakeFusionCanvas();
        var canvas = new FakeCanvas();
        DibSeams.Canvas = canvas;

        var d = Mk24(4, 4);
        Fill(d, 100, 100, 100);

        d.DoSpotLight(51, TDxRect.Rect(1, 1, 3, 3));

        // 原文缺陷：SpotLight 内部只改临时 z，从不回写 Src ⇒ Self 像素不变
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                AssertPx(d, x, y, 100, 100, 100, "DIB.pas:6921-6944 未回写 Src");

        // 但接缝调用序列完整发生
        Assert.Equal(new[] { DibFusionSupport.clBlack, DibFusionSupport.clWhite }, spot.BrushColors);
        Assert.Single(spot.Rects);
        Assert.Equal((0, 0, 4, 4), spot.Rects[0]);
        Assert.Single(spot.Ellipses);
        Assert.Equal((1, 1, 3, 3), spot.Ellipses[0]);
        Assert.Single(spot.Transparency);
        Assert.True(spot.Transparency[0].value);
        Assert.Single(spot.CopyModes);
        Assert.Equal(DibFusionSupport.cmSrcAnd, spot.CopyModes[0].value);
        Assert.Equal(1, canvas.DrawCount);

        // 被设置 CopyMode 的那张图（= 临时 z）已被 Darkness 处理过：
        // z 的像素来自 BitBlt 接缝（假接缝不拷像素）⇒ 全 0 → Darkness 后仍全 0
        var z = spot.CopyModes[0].dib;
        Assert.NotSame(d, z);
        AssertPx(z, 0, 0, 0, 0, 0, "z 由 SetSize 清零且假 BitBlt 不拷像素");
    }

    // =========================================================================================
    // DIB.pas 6960-6988 —— DoEmboss
    // =========================================================================================

    [Fact]
    public void DoEmboss_当前行与下一行相隔3像素反相叠加()
    {
        var d = Mk24(4, 2);
        Fill(d, 100, 100, 100);
        for (int x = 0; x < 4; x++)
            d.SetPixel(x, 1, 0);          // 下一行全 0 → xor $FF = 255

        d.DoEmboss();

        // (100 + 255) shr 1 = 177，只作用于 Y=0 行、X=0（Width-4 = 0）
        AssertPx(d, 0, 0, 177, 177, 177, "DIB.pas:6970-6972");
        AssertPx(d, 1, 0, 100, 100, 100, "列范围 0..Width-4");
        AssertPx(d, 0, 1, 0, 0, 0, "行范围 0..Height-2，末行不改");
    }

    [Fact]
    public void DoEmboss_下一行为白时反相后为0()
    {
        var d = Mk24(4, 2);
        Fill(d, 100, 100, 100);
        for (int x = 0; x < 4; x++)
            d.SetPixel(x, 1, unchecked((uint)Col(255, 255, 255)));

        d.DoEmboss();
        AssertPx(d, 0, 0, 50, 50, 50, "(100 + (255 xor 255)) shr 1 = 50");
    }

    [Fact]
    public void DoEmboss_宽度小于4时不处理()
    {
        var d = Mk24(3, 2);
        Fill(d, 100, 100, 100);
        for (int x = 0; x < 3; x++)
            d.SetPixel(x, 1, 0);
        d.DoEmboss();
        AssertPx(d, 0, 0, 100, 100, 100, "for X := 0 to Width-4 = -1 空转");
    }

    // =========================================================================================
    // DIB.pas 6990-7031 —— DoSolorize
    // =========================================================================================

    [Fact]
    public void DoSolorize_均值大于阈值则整像素反相()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));   // 均值 = (50+100+200)/3 = 116
        d.DoSolorize(100);
        AssertPx(d, 0, 0, 55, 155, 205, "255 - 原值（DIB.pas:7007-7009）");
    }

    [Fact]
    public void DoSolorize_阈值等于均值时走拷贝分支()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoSolorize(116);
        AssertPx(d, 0, 0, 200, 100, 50, "C > Amount 为假 ⇒ 原样拷贝");
    }

    [Fact]
    public void DoSolorize_阈值更高与全黑像素()
    {
        var d = Mk24(2, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.SetPixel(1, 0, 0);
        d.DoSolorize(200);
        AssertPx(d, 0, 0, 200, 100, 50, "116 > 200 为假");
        AssertPx(d, 1, 0, 0, 0, 0, "C=0 > 200 为假");

        var d0 = Mk24(1, 1);
        d0.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d0.DoSolorize(0);
        AssertPx(d0, 0, 0, 55, 155, 205, "C=116 > 0 → 反相");
    }

    // =========================================================================================
    // DIB.pas 7033-7065 —— DoPosterize
    // =========================================================================================

    [Fact]
    public void DoPosterize_按Round浮点除量化()
    {
        var d = Mk24(2, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(100, 100, 100)));
        d.SetPixel(1, 0, unchecked((uint)Col(255, 255, 255)));
        d.DoPosterize(64);
        // 100/64 = 1.5625 → Round = 2 → 128
        AssertPx(d, 0, 0, 128, 128, 128, "DIB.pas:7047-7049");
        // 255/64 = 3.984 → Round = 4 → 256 → Byte 回绕 = 0
        AssertPx(d, 1, 0, 0, 0, 0, "量化结果可超过 255 ⇒ 写回 Byte 回绕");
    }

    [Fact]
    public void DoPosterize_Amount100时255回绕成44()
    {
        var d = Mk24(1, 1);
        Fill(d, 255, 255, 255);
        d.DoPosterize(100);
        // 255/100 = 2.55 → Round = 3 → 300 → 300 and $FF = 44
        AssertPx(d, 0, 0, 44, 44, 44, "原文无夹取（DIB.pas:7047）");
    }

    [Fact]
    public void DoPosterize_Amount0时除零抛异常()
    {
        var d = Mk24(1, 1);
        Fill(d, 100, 100, 100);
        Assert.Throws<ArithmeticException>(() => d.DoPosterize(0));
    }

    // =========================================================================================
    // DIB.pas 7067-7115 —— DoBrightness
    // =========================================================================================

    [Fact]
    public void DoBrightness_正数提亮并夹到255()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoBrightness(50);
        AssertPx(d, 0, 0, 250, 150, 100, "Min(255, v+Value)（DIB.pas:7091-7093）");

        var d2 = Mk24(1, 1);
        d2.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d2.DoBrightness(200);
        AssertPx(d2, 0, 0, 255, 255, 250, "R/G 溢出夹到 255");
    }

    [Fact]
    public void DoBrightness_负数变暗并夹到0()
    {
        var d = Mk24(1, 1);
        d.SetPixel(0, 0, unchecked((uint)Col(200, 100, 50)));
        d.DoBrightness(-50);
        AssertPx(d, 0, 0, 150, 50, 0, "Max(0, v+Value)（DIB.pas:7096-7098）");
    }

    [Fact]
    public void DoBrightness_Amount0保持不变()
    {
        var d = Mk24(2, 2);
        Fill(d, 10, 20, 30);
        d.DoBrightness(0);
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
                AssertPx(d, x, y, 10, 20, 30, "Value=0 → else 分支 Max(0, v)");
    }

    // =========================================================================================
    // DIB.pas 4940-4955 —— TCustomDXDIB
    // =========================================================================================

    [Fact]
    public void TCustomDXDIB_Create得到空DIB()
    {
        var c = new TCustomDXDIB();
        Assert.NotNull(c.DIB);
        Assert.True(c.DIB.Empty);
        Assert.Equal(0, c.DIB.Width);
    }

    [Fact]
    public void TCustomDXDIB_SetDIB走Assign_共享底层图()
    {
        var c = new TCustomDXDIB();
        var img = Mk24(3, 2);
        img.SetPixel(0, 0, unchecked((uint)Col(1, 2, 3)));

        c.DIB = img;

        Assert.Equal(3, c.DIB.Width);
        Assert.Equal(2, c.DIB.Height);
        AssertPx(c.DIB, 0, 0, 1, 2, 3, "Assign → SetImage 共享 TDIBSharedImage");
        Assert.NotSame(img, c.DIB);
    }

    [Fact]
    public void TCustomDXDIB_SetDIB为null等价Clear()
    {
        var c = new TCustomDXDIB();
        c.DIB = Mk24(2, 2);
        Assert.Equal(2, c.DIB.Width);
        c.DIB = null;
        Assert.True(c.DIB.Empty, "Assign(nil) → Clear()（DIB.Core.cs Assign）");
    }

    [Fact]
    public void TCustomDXDIB_Destroy后DIB为null()
    {
        var c = new TCustomDXDIB();
        c.Destroy();
        Assert.Null(c.DIB);
    }

    [Fact]
    public void TDXDIB_是TCustomDXDIB的空子类()
    {
        var c = new TDXDIB();
        Assert.IsAssignableFrom<TCustomDXDIB>(c);
        Assert.NotNull(c.DIB);
    }

    // =========================================================================================
    // DIB.pas 4959-5138 —— TCustomDXPaintBox
    // =========================================================================================

    [Fact]
    public void PaintBox_构造注入null接缝_抛异常而不是静默()
    {
        Assert.Throws<ArgumentNullException>(() => new TCustomDXPaintBox(null));
    }

    [Fact]
    public void PaintBox_构造写入ControlStyle与105尺寸()
    {
        var seam = new FakePaintSeam();
        var box = new TCustomDXPaintBox(seam);
        Assert.True(seam.ControlStyleReplicatable);
        Assert.Equal(105, seam.Height);
        Assert.Equal(105, seam.Width);
        Assert.NotNull(box.DIB);
    }

    [Fact]
    public void PaintBox_GetPalette转发FDIB的Palette()
    {
        var seam = new FakePaintSeam();
        var box = new TCustomDXPaintBox(seam);
        Assert.Equal(box.DIB.Palette, box.GetPalette());
    }

    [Fact]
    public void PaintBox_设计期画虚线框且仍会因Empty提前退出()
    {
        var seam = new FakePaintSeam { Designing = true, ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam);

        box.Paint();

        Assert.Equal(1, seam.PenDashCount);
        Assert.Equal(1, seam.BrushClearCount);
        // Rectangle 用的是**控件**的 Width/Height（=105），不是 DIB 的
        Assert.Single(seam.Rects);
        Assert.Equal((0, 0, 105, 105), seam.Rects[0]);
        Assert.Equal(0, seam.DrawCount);
        Assert.Equal(0, seam.StretchDrawCount);
    }

    [Fact]
    public void PaintBox_空图不绘制()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam);
        box.Paint();
        Assert.Equal(0, seam.DrawCount);
        Assert.Equal(0, seam.StretchDrawCount);
    }

    [Fact]
    public void PaintBox_尺寸相同走Draw_不拉伸()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2) };
        box.Paint();
        Assert.Equal(1, seam.DrawCount);
        Assert.Equal(0, seam.LastDrawX);
        Assert.Equal(0, seam.LastDrawY);
        Assert.Equal(0, seam.StretchDrawCount);
    }

    [Fact]
    public void PaintBox_Center为真时Draw偏移为负宽高超客户端宽高的一半()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), Center = true };
        box.Paint();
        // -(2-10) div 2 = 4（Delphi div 向零截断）
        Assert.Equal(4, seam.LastDrawX);
        Assert.Equal(4, seam.LastDrawY);
    }

    [Fact]
    public void PaintBox_Stretch不保宽高比时拉伸到整个客户端()
    {
        var seam = new FakePaintSeam { ClientWidth = 20, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), Stretch = true };
        box.Paint();
        Assert.Equal(1, seam.StretchDrawCount);
        Assert.Equal(TDxRect.Bounds(0, 0, 20, 10), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_Stretch保宽高比时按较小比例缩放()
    {
        var seam = new FakePaintSeam { ClientWidth = 20, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), Stretch = true, KeepAspect = true };
        box.Paint();
        // R = 20/2 = 10；r2 = 10/2 = 5；R > r2 → R = 5 → Round(10), Round(10)
        Assert.Equal(TDxRect.Bounds(0, 0, 10, 10), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_AutoStretch图比客户端小时按原尺寸Draw()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), AutoStretch = true };
        box.Paint();
        Assert.Equal(1, seam.DrawCount);
        Assert.Equal(0, seam.StretchDrawCount);
    }

    [Fact]
    public void PaintBox_AutoStretch图比客户端大时等比缩小()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(20, 10), AutoStretch = true };
        box.Paint();
        // R = 10/20 = 0.5；r2 = 10/10 = 1；R < r2 → R = 0.5 → Round(10), Round(5)
        Assert.Equal(TDxRect.Bounds(0, 0, 10, 5), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_AutoStretch且Center时偏移按缩放后尺寸计算()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(20, 10), AutoStretch = true, Center = true };
        box.Paint();
        // 目标 10x5 → StretchDraw(Bounds(-(10-10)/2=0, -(5-10)/2=2, 10, 5))
        Assert.Equal(TDxRect.Bounds(0, 2, 10, 5), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_ViewWidth优先于AutoStretch的非缩放分支()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), ViewWidth = 6 };
        box.Paint();
        // ViewWidth2=6；ViewHeight2=0 → FDIB.Height = 2；AutoStretch=false → Draw2(6,2)
        Assert.Equal(TDxRect.Bounds(0, 0, 6, 2), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_ViewWidth与AutoStretch且超出客户端时按比例缩小()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), ViewWidth = 20, AutoStretch = true };
        box.Paint();
        // ViewWidth2=20, ViewHeight2=2；10 < 20 → R = 20/10 = 2, r2 = 2/10 = 0.2 → R = 0.2
        // Draw2(Round(2), Round(2)) = (2,2) → 与 FDIB 同尺寸 → Draw
        Assert.Equal(1, seam.DrawCount);
        Assert.Equal(0, seam.StretchDrawCount);
    }

    [Fact]
    public void PaintBox_ViewWidth与AutoStretch客户端够大时按View尺寸拉伸()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(2, 2), ViewWidth = 6, ViewHeight = 4, AutoStretch = true };
        box.Paint();
        Assert.Equal(TDxRect.Bounds(0, 0, 6, 4), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_客户端为0时AutoStretch算出0尺寸()
    {
        var seam = new FakePaintSeam { ClientWidth = 0, ClientHeight = 0 };
        var box = new TCustomDXPaintBox(seam) { DIB = Mk24(20, 10), AutoStretch = true };
        box.Paint();
        // R = 0/20 = 0；r2 = 0/10 = 0 → R 保持 0 → Draw2(0,0)：0 != 20 → StretchDraw(Bounds(0,0,0,0))
        Assert.Equal(TDxRect.Bounds(0, 0, 0, 0), seam.LastStretchRect);
    }

    [Fact]
    public void PaintBox_Setter命中时Invalidate_未变化时不Invalidate()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam);

        box.AutoStretch = false;          // 未变化
        Assert.Equal(0, seam.InvalidateCount);
        box.AutoStretch = true;
        Assert.Equal(1, seam.InvalidateCount);

        box.Center = true;  Assert.Equal(2, seam.InvalidateCount);
        box.KeepAspect = true; Assert.Equal(3, seam.InvalidateCount);
        box.Stretch = true; Assert.Equal(4, seam.InvalidateCount);
        box.ViewWidth = 3;  Assert.Equal(5, seam.InvalidateCount);
        box.ViewHeight = 4; Assert.Equal(6, seam.InvalidateCount);
    }

    [Fact]
    public void PaintBox_SetViewWidth负数归一为0()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam);
        box.ViewWidth = -5;
        Assert.Equal(0, box.ViewWidth);
        Assert.Equal(0, seam.InvalidateCount);   // 归零后与默认值相同 → 不 Invalidate
        box.ViewHeight = -1;
        Assert.Equal(0, box.ViewHeight);
        Assert.Equal(0, seam.InvalidateCount);
    }

    [Fact]
    public void PaintBox_SetDIB是引用比较_同一实例不Invalidate()
    {
        var seam = new FakePaintSeam { ClientWidth = 10, ClientHeight = 10 };
        var box = new TCustomDXPaintBox(seam);
        var img = Mk24(2, 2);

        box.DIB = img;
        Assert.Equal(1, seam.InvalidateCount);
        Assert.Equal(2, box.DIB.Width);

        box.DIB = box.DIB;                // 同一引用 → 不进 if
        Assert.Equal(1, seam.InvalidateCount);

        box.DIB = Mk24(4, 4);             // 不同引用（即使内容相同也 Invalidate）
        Assert.Equal(2, seam.InvalidateCount);
        Assert.Equal(4, box.DIB.Width);
    }

    [Fact]
    public void PaintBox_Destroy后DIB为null()
    {
        var seam = new FakePaintSeam();
        var box = new TCustomDXPaintBox(seam);
        box.Destroy();
        Assert.Null(box.DIB);
    }

    [Fact]
    public void TDXPaintBox_是TCustomDXPaintBox的空子类()
    {
        var seam = new FakePaintSeam();
        var box = new TDXPaintBox(seam);
        Assert.IsAssignableFrom<TCustomDXPaintBox>(box);
        Assert.Equal(105, seam.Height);
    }
}

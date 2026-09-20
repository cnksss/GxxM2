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

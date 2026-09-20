using System;
using System.Runtime.InteropServices;
using GXX.Client.DxComponent;
using Xunit;

// =============================================================================================
// DIB.pas 1:1 移植 —— 重采样分片测试（DIB.Fusion.Filters.cs，DIB.pas 7117-7674）。
//
// 覆盖：8 个滤波器权重（Hermite/Box/Triangle/Bell/Spline/Lanczos3/SinC/Mitchell）、
//      Color2RGB/RGB2Color、4 个记录的内存布局、TDIB.DoResample 的两趟重采样。
//
// ★ 差异断言重点：
//   * BoxFilter 在 ±0.5 处**不对称**（-0.5 → 0、+0.5 → 1）；
//   * BellFilter 在 0.5 处走第二分支（`Value < 0.5` 为假）；
//   * DoResample 的 USE_SCANLINE 路径（DIB.pas:6 `{$DEFINE USE_SCANLINE}`）把源行按
//     TColorRGB(R,G,B) 解释，而 DIB 的 24bpp 内存是 B,G,R ⇒ **R/B 互换**：最终值出现在
//     `GetPixel` 的 **B 通道**。测试用 `Col(0,0,v)` 作为源色来锁死该颠倒。
//
// 挂 dxctrl-serial：DibSeams / DibFusionSupport 是进程级静态状态。
// =============================================================================================

[Collection("dxctrl-serial")]
public class DxCtlDibFusionFilterTests
{
    public DxCtlDibFusionFilterTests()
    {
        DibSeams.Gdi = null;
        DibSeams.Canvas = null;
        DibSeams.GlobalMemory = null;
        DibFusionCanvas.Seam = null;
        DibFusionSpot.Seam = null;
        DibFusionSupport.RandomFunc = null;
    }

    private static TDIB Mk24(int w, int h)
    {
        var d = new TDIB();
        d.SetSize(w, h, 24);
        return d;
    }

    private static int Col(int r, int g, int b) => (int)DibFusionSupport.Rgb((byte)r, (byte)g, (byte)b);

    private static (int R, int G, int B) Px(TDIB d, int x, int y)
    {
        uint v = d.GetPixel(x, y);
        return (unchecked((int)(v & 0xFF)), unchecked((int)((v >> 8) & 0xFF)), unchecked((int)((v >> 16) & 0xFF)));
    }

    private static void AssertSame(TDIB a, TDIB b, string why)
    {
        Assert.Equal(a.Width, b.Width);
        Assert.Equal(a.Height, b.Height);
        for (int y = 0; y < a.Height; y++)
            for (int x = 0; x < a.Width; x++)
                Assert.True(a.GetPixel(x, y) == b.GetPixel(x, y),
                    $"{why}: @({x},{y}) 期望 {b.GetPixel(x, y):X8} 实得 {a.GetPixel(x, y):X8}");
    }

    // =========================================================================================
    // DIB.pas 7126-7241 —— 8 个滤波器权重
    // =========================================================================================

    [Fact]
    public void HermiteFilter_三次曲线与绝对值()
    {
        Assert.Equal(1.0f, DibFusionFilters.HermiteFilter(0.0f));
        Assert.Equal(0.5f, DibFusionFilters.HermiteFilter(0.5f));
        Assert.Equal(0.5f, DibFusionFilters.HermiteFilter(-0.5f));   // |t|
        Assert.Equal(0.028f, DibFusionFilters.HermiteFilter(0.9f), 5);
        // Value < 1.0 为假 ⇒ 0
        Assert.Equal(0.0f, DibFusionFilters.HermiteFilter(1.0f));
        Assert.Equal(0.0f, DibFusionFilters.HermiteFilter(2.0f));
    }

    [Fact]
    public void BoxFilter_在正负0点5处不对称()
    {
        // 条件 (Value > -0.5) and (Value <= 0.5)
        Assert.Equal(0.0f, DibFusionFilters.BoxFilter(-0.5f));   // > 为假
        Assert.Equal(1.0f, DibFusionFilters.BoxFilter(-0.4999f));
        Assert.Equal(1.0f, DibFusionFilters.BoxFilter(0.0f));
        Assert.Equal(1.0f, DibFusionFilters.BoxFilter(0.5f));    // <= 为真
        Assert.Equal(0.0f, DibFusionFilters.BoxFilter(0.5001f));
        Assert.Equal(0.0f, DibFusionFilters.BoxFilter(-0.5001f));
    }

    [Fact]
    public void TriangleFilter_线性锥()
    {
        Assert.Equal(1.0f, DibFusionFilters.TriangleFilter(0.0f));
        Assert.Equal(0.75f, DibFusionFilters.TriangleFilter(0.25f));
        Assert.Equal(0.75f, DibFusionFilters.TriangleFilter(-0.25f));
        Assert.Equal(0.0f, DibFusionFilters.TriangleFilter(1.0f));    // < 1.0 为假
        Assert.Equal(0.0f, DibFusionFilters.TriangleFilter(-1.0f));
    }

    [Fact]
    public void BellFilter_三段且0点5走第二分支()
    {
        Assert.Equal(0.75f, DibFusionFilters.BellFilter(0.0f));
        // Value=0.5：`Value < 0.5` 为假 ⇒ 第二分支，Value := 0.5-1.5 = -1.0 → 0.5*1 = 0.5
        Assert.Equal(0.5f, DibFusionFilters.BellFilter(0.5f));
        Assert.Equal(0.125f, DibFusionFilters.BellFilter(1.0f));      // (1-1.5)^2*0.5
        Assert.Equal(0.125f, DibFusionFilters.BellFilter(-1.0f));
        Assert.Equal(0.0f, DibFusionFilters.BellFilter(1.5f));        // < 1.5 为假
        Assert.Equal(0.0f, DibFusionFilters.BellFilter(2.0f));
    }

    [Fact]
    public void SplineFilter_两段B样条()
    {
        Assert.Equal(2.0f / 3.0f, DibFusionFilters.SplineFilter(0.0f), 6);
        // 0.5: 0.5*0.25*0.5 - 0.25 + 2/3 = 0.4791667
        Assert.Equal(0.479167f, DibFusionFilters.SplineFilter(0.5f), 5);
        // 1.0 走第二段: (1/6)*(2-1)^2*(2-1) = 1/6
        Assert.Equal(1.0f / 6.0f, DibFusionFilters.SplineFilter(1.0f), 6);
        Assert.Equal(0.0f, DibFusionFilters.SplineFilter(2.0f));
        Assert.Equal(0.0f, DibFusionFilters.SplineFilter(3.0f));
    }

    [Fact]
    public void SinC_零点与对称()
    {
        Assert.Equal(1.0f, DibFusionFilters.SinC(0.0f));
        // sin(Pi)/Pi ≈ 3.9e-17
        Assert.Equal(0.0f, DibFusionFilters.SinC(1.0f), 6);
        Assert.Equal(0.636620f, DibFusionFilters.SinC(0.5f), 5);
        // sin(1.5Pi)/(1.5Pi) = -1/(1.5Pi) ≈ -0.212207
        Assert.Equal(-0.212207f, DibFusionFilters.SinC(1.5f), 5);
    }

    [Fact]
    public void Lanczos3Filter_窗口宽度3()
    {
        Assert.Equal(1.0f, DibFusionFilters.Lanczos3Filter(0.0f));
        Assert.Equal(0.0f, DibFusionFilters.Lanczos3Filter(1.0f), 5);
        Assert.Equal(0.0f, DibFusionFilters.Lanczos3Filter(-1.0f), 5);
        // SinC(1.5)*SinC(0.5) = -0.212207 * 0.636620 = -0.135094
        Assert.Equal(-0.135094f, DibFusionFilters.Lanczos3Filter(1.5f), 4);
        Assert.Equal(0.0f, DibFusionFilters.Lanczos3Filter(3.0f));    // < 3.0 为假
        Assert.Equal(0.0f, DibFusionFilters.Lanczos3Filter(4.0f));
    }

    [Fact]
    public void MitchellFilter_两段且BC均为三分之一()
    {
        // t=0: (6 - 2B)/6 = (6 - 0.6667)/6 = 0.888889
        Assert.Equal(0.888889f, DibFusionFilters.MitchellFilter(0.0f), 5);
        // t=0.5: (7*0.125 + (-12)*0.25 + 5.333333)/6 = 0.534722
        Assert.Equal(0.534722f, DibFusionFilters.MitchellFilter(0.5f), 5);
        // t=1: 第二段 = 0.333333/6 = 0.0555556
        Assert.Equal(0.055556f, DibFusionFilters.MitchellFilter(1.0f), 5);
        Assert.Equal(0.0f, DibFusionFilters.MitchellFilter(2.0f));
    }

    // =========================================================================================
    // DIB.pas 7305-7315 —— Color2RGB / RGB2Color
    // =========================================================================================

    [Fact]
    public void Color2RGB_低字节进R字段()
    {
        var c = DibFusionFilters.Color2RGB(0x00123456);
        Assert.Equal(0x56, c.R);
        Assert.Equal(0x34, c.G);
        Assert.Equal(0x12, c.B);
    }

    [Fact]
    public void RGB2Color_按R低字节打包()
    {
        var c = new DibFusionFilters.DibColorRGB { R = 0x56, G = 0x34, B = 0x12 };
        Assert.Equal(0x00123456, DibFusionFilters.RGB2Color(c));
    }

    [Fact]
    public void Color2RGB与RGB2Color_往返一致()
    {
        foreach (int v in new[] { 0x000000, 0xFFFFFF, 0x00FF00, 0x0000FF, 0xFF0000, 0x7F3E1D })
            Assert.Equal(v, DibFusionFilters.RGB2Color(DibFusionFilters.Color2RGB(v)));
    }

    // =========================================================================================
    // DIB.pas 7248-7279 —— 记录布局
    // =========================================================================================

    [Fact]
    public void 记录布局_与原文SizeOf一致()
    {
        // TContributor = record pixel: Integer; weight: Single; end; → 8
        Assert.Equal(8, Marshal.SizeOf<DibFusionFilters.DibContributor>());
        // TRGB = packed record R, G, B: Single; end; → 12
        Assert.Equal(12, Marshal.SizeOf<DibFusionFilters.DibRGB>());
        // TColorRGB = packed record R, G, B: Byte; end; → 3（§24.3 移植陷阱 1 的对照：纯字节不需要 Explicit）
        Assert.Equal(3, Marshal.SizeOf<DibFusionFilters.DibColorRGB>());
    }

    [Fact]
    public void 记录字段序_Contributor与ColorRGB()
    {
        var c = new DibFusionFilters.DibContributor { pixel = 0x01020304, weight = 1.5f };
        Assert.Equal(0x01020304, c.pixel);
        Assert.Equal(1.5f, c.weight);

        var col = new DibFusionFilters.DibColorRGB { R = 1, G = 2, B = 3 };
        Assert.Equal(1, col.R);
        Assert.Equal(2, col.G);
        Assert.Equal(3, col.B);
    }

    // =========================================================================================
    // DIB.pas 7117-7674 —— DoResample
    // =========================================================================================

    [Fact]
    public void DoResample_同尺寸Box与Triangle与Hermite是精确恒等()
    {
        foreach (var f in new[] { TFilterTypeResample.ftrBox, TFilterTypeResample.ftrTriangle,
                                  TFilterTypeResample.ftrHermite })
        {
            var src = Mk24(4, 4);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    src.SetPixel(x, y, unchecked((uint)Col(x * 20 + 1, y * 30 + 2, x * 7 + y * 5 + 3)));

            src.DoResample(4, 4, f);

            Assert.Equal(4, src.Width);
            Assert.Equal(4, src.Height);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    Assert.Equal((uint)Col(x * 20 + 1, y * 30 + 2, x * 7 + y * 5 + 3), src.GetPixel(x, y));
        }
    }

    [Fact]
    public void DoResample_四乘四到二乘二_Box_精确权重表且值落在B通道()
    {
        // 源值 v(x,y) = 3x + 12y（均为 3 的倍数 ⇒ 加权和恰为整数）；用 Col(0,0,v) 使内存为 [v,0,0]
        var src = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                src.SetPixel(x, y, unchecked((uint)Col(0, 0, 3 * x + 12 * y)));

        src.DoResample(2, 2, TFilterTypeResample.ftrBox);

        Assert.Equal(2, src.Width);
        Assert.Equal(2, src.Height);
        // 横向（xscale = 1/3 < 1，子采样分支）：
        //   I=0 → (2*v(1,k) + v(0,k))/3 = 2+12k；I=1 → (v(2,k) + 2*v(3,k))/3 = 8+12k
        // 纵向同理 → 行 0 = (2*Work[1] + Work[0])/3、行 1 = (Work[2] + 2*Work[3])/3
        // 行 0 = [(2*14+2)/3, (2*20+8)/3] = [10, 16]
        // 行 1 = [(26+2*38)/3, (32+2*44)/3] = [34, 40]
        // USE_SCANLINE 路径把源行按 (R,G,B) 解释而内存是 (B,G,R) ⇒ 结果落在 **B 通道**
        Assert.Equal((0, 0, 10), Px(src, 0, 0));
        Assert.Equal((0, 0, 16), Px(src, 1, 0));
        Assert.Equal((0, 0, 34), Px(src, 0, 1));
        Assert.Equal((0, 0, 40), Px(src, 1, 1));
    }

    [Fact]
    public void DoResample_一宽源走超采样分支_所有列取同一像素()
    {
        // SrcWidth = 1 ⇒ xscale := DstWidth / SrcWidth = 3（超采样分支，权重**不**除 fscale）
        var src = Mk24(1, 4);
        for (int y = 0; y < 4; y++)
            src.SetPixel(0, y, unchecked((uint)Col(0, 0, 10 + 10 * y)));

        src.DoResample(3, 4, TFilterTypeResample.ftrBox);

        Assert.Equal(3, src.Width);
        Assert.Equal(4, src.Height);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 3; x++)
                Assert.Equal((0, 0, 10 + 10 * y), Px(src, x, y));
    }

    [Fact]
    public void DoResample_均匀图在无越界反射的三个滤波器下保持灰度()
    {
        // 只用 ftrBox/ftrTriangle/ftrHermite（DefaultFilterRadius ≤ 1）：4x4→2x2 时其贡献者
        // 下标不会走到原文反射式的负值（见实现注释），故结果确定。
        foreach (var f in new[] { TFilterTypeResample.ftrBox, TFilterTypeResample.ftrTriangle,
                                  TFilterTypeResample.ftrHermite })
        {
            var src = Mk24(4, 4);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    src.SetPixel(x, y, unchecked((uint)Col(100, 100, 100)));

            src.DoResample(2, 2, f);

            Assert.Equal(2, src.Width);
            Assert.Equal(2, src.Height);
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                {
                    var (R, G, B) = Px(src, x, y);
                    Assert.True(R == G && G == B, $"{f} @({x},{y}) 应为灰度，实得 ({R},{G},{B})");
                    // 三个滤波器的离散权重和恰为 1.0（4x4→2x2 子采样分支）⇒ 100.0 → Round = 100
                    Assert.Equal((100, 100, 100), (R, G, B));
                }
        }
    }

    [Fact]
    public void DoResample_七种滤波器都不抛异常并给出合法尺寸()
    {
        // ftrBell/ftrBSpline/ftrLanczos3/ftrMitchell 在 DstWidth < SrcWidth 时会走原文的
        // 负下标反射（读到缓冲区之外），结果不可确定 —— 此处只断言"不抛、尺寸正确、值在 0..255"。
        foreach (TFilterTypeResample f in Enum.GetValues(typeof(TFilterTypeResample)))
        {
            var src = Mk24(8, 8);
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    src.SetPixel(x, y, unchecked((uint)Col(x * 20, y * 20, 128)));

            src.DoResample(4, 4, f);

            Assert.Equal(4, src.Width);
            Assert.Equal(4, src.Height);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                {
                    var (R, G, B) = Px(src, x, y);
                    Assert.InRange(R, 0, 255);
                    Assert.InRange(G, 0, 255);
                    Assert.InRange(B, 0, 255);
                }
        }
    }

    [Fact]
    public void DoResample_目标高度为1时yscale为零导致Trunc无穷抛异常()
    {
        // DstHeight = 1 ⇒ yscale := (1-1)/(4-1) = 0 ⇒ Width := FWidth/0 = +Inf ⇒ Trunc(+Inf) 抛
        // （Delphi EInvalidOp；托管侧 ArithmeticException）
        var src = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                src.SetPixel(x, y, unchecked((uint)Col(1, 2, 3)));
        Assert.Throws<ArithmeticException>(() => src.DoResample(2, 1, TFilterTypeResample.ftrBox));
    }

    [Fact]
    public void DoResample_源高为1时WorkScanLine1越界()
    {
        // SrcHeight = 1 ⇒ yscale := DstHeight/1 = 4（不等于 0）；Work 只有 1 行 ⇒
        // 纵向 Delta 计算里的 `Work.ScanLine(1)` 抛 SScanline
        var src = Mk24(4, 1);
        for (int x = 0; x < 4; x++)
            src.SetPixel(x, 0, unchecked((uint)Col(1, 2, 3)));
        Assert.Throws<EInvalidGraphicOperation>(() => src.DoResample(2, 4, TFilterTypeResample.ftrBox));
    }

    [Fact]
    public void DoResample_零尺寸目标抛SScanline()
    {
        var src = Mk24(4, 4);
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                src.SetPixel(x, y, unchecked((uint)Col(1, 2, 3)));
        // DstWidth = 0 ⇒ Work.Width := 0 → Clear；纵向写入时 Dst.ScanLine(0) 已越界
        Assert.Throws<EInvalidGraphicOperation>(() => src.DoResample(0, 2, TFilterTypeResample.ftrBox));
    }

    [Fact]
    public void DoResample_放大到更大尺寸不崩且尺寸正确()
    {
        var src = Mk24(2, 2);
        src.SetPixel(0, 0, unchecked((uint)Col(255, 0, 0)));
        src.SetPixel(1, 0, unchecked((uint)Col(0, 255, 0)));
        src.SetPixel(0, 1, unchecked((uint)Col(0, 0, 255)));
        src.SetPixel(1, 1, unchecked((uint)Col(255, 255, 255)));

        src.DoResample(6, 6, TFilterTypeResample.ftrBox);

        Assert.Equal(6, src.Width);
        Assert.Equal(6, src.Height);
    }

    [Fact]
    public void DoResample_空图源触发SourceBitmapTooSmall()
    {
        // BB1.BitCount := 24 先把空 BB1 撑成 1x1，但紧接着 BB1.Assign(Self) 又把它替换成
        // **Self 的空共享图**（0x0）⇒ SrcWidth = 0 ⇒ 命中原文守卫
        var empty = new TDIB();
        Assert.True(empty.Empty);
        var ex = Assert.Throws<Exception>(() => empty.DoResample(2, 2, TFilterTypeResample.ftrBox));
        Assert.Equal("Source bitmap too small", ex.Message);
    }
}

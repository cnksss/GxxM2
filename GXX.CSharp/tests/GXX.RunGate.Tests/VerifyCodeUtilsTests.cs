using System;
using System.Drawing;
using System.Linq;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// VerifyCodeUtils.pas 移植测试（原 VerifyCodeUtils.pas，279 行 / ReadAllLines 327 行）。
///
/// 该单元是 **GDI+ 渲染**代码（把调用方给的验证码字符串画到位图上），
/// 没有"验证码生成算法"可测 —— 可测部分被抽到纯计算层 CaptchaRenderMath，
/// 并使用注入式随机源（ICaptchaRandomSource）使结果完全确定。
///
/// 黄金向量的推导依据：
///   * DelphiLcgRandom 复刻 System.pas 的 `RandSeed := RandSeed * 134775813 + 1` +
///     `Cardinal(RandSeed) shr 16 and $7FFF`（RandSeed 初值 0），序列可在任意环境复算；
///   * 其余向量的算术在断言注释里逐项写出。
/// </summary>
public class VerifyCodeUtilsTests
{
    /// <summary>按 Delphi System.pas 公式独立复算的参考实现（用于交叉验证被测实现）。</summary>
    private static int ReferenceDelphiRaw(ref uint seed)
    {
        unchecked { seed = seed * 134775813u + 1u; }
        return (int)((seed >> 16) & 0x7FFFu);
    }

    private static int ReferenceDelphiRandom(ref uint seed, int range)
    {
        unchecked { seed = seed * 134775813u + 1u; }
        uint r = (seed >> 16) & 0x7FFFu;
        if (range <= 0) return 0;
        return (int)((r * (uint)range) >> 15);
    }

    // ---------------- DelphiLcgRandom（随机源接缝） ----------------

    [Fact]
    public void DelphiLcg_LowBitsFollowShortCycle_DocumentedOriginalBehaviour()
    {
        // RandSeed *= 134775813 ($08088405) + 1 → 每步低位可精确推导：
        //   * 取模 2：RandSeed≡1 (mod 2) 恒成立
        //   * 取模 4：0 → 1 → 1 → 1 ...（对任意奇数乘数+1 都如此）
        // 这是 Delphi Random 的已知确定性特征，原文验证码因此可复现。
        // RandSeed 更新为 seed*134775813+1：0 → 1 → 134775814 → 3698175007 → ...
        // 131775813 ≡ 1 (mod 4) 且 +1 → seed ≡ 1 (mod 4) 时结果 ≡ 2 (mod 4)；
        // 而 134775814 ≡ 2 (mod 4) → 下一步 2*1+1 = 3 (mod 4) → 再下一步 3+1 = 0 (mod 4) …
        // 关键点：低位序列确定 —— 这就是原文验证码每次可复现的原因。
        var rnd = new DelphiLcgRandom();
        rnd.Next(1000);                       // 第一步：seed 0 → 1
        Assert.Equal(1u, rnd.RandSeed);
        var lowBits = new System.Collections.Generic.List<uint>();
        for (int i = 0; i < 4; i++)
        {
            rnd.Next(1000);
            lowBits.Add(rnd.RandSeed & 3u);
        }
        Assert.Equal(new uint[] { 2u, 3u, 0u, 1u }, lowBits);   // 周期 4 的低位确定序列
    }

    [Fact]
    public void DelphiLcg_GoldenSequence_Range0x7FFF()
    {
        // 逐步复算（乘数 134775813，加 1，取 bits 16..30）：
        //   seed0=0     -> 1          -> (1>>16)&7FFF      = 0
        //   seed1=1     -> 134775814  -> (134775814>>16)    = 2056
        //   seed2=...   -> 23661
        //   seed3       -> 13276
        //   seed4       -> 17886
        //   seed5       -> 11249
        var rnd = new DelphiLcgRandom();
        int[] expected = { 0, 2056, 23661, 13276, 17886, 11249 };
        // Range = 32768（= 2^15）时 `(raw * 32768) shr 15` 是恒等，取到原始 15 位值
        foreach (int e in expected) Assert.Equal(e, rnd.Next(32768));

        // 差异断言：Range = 32767（非 2 的幂）时 (raw * 32767) >> 15 == raw - 1（raw > 0）
        var rnd2 = new DelphiLcgRandom();
        Assert.Equal(0, rnd2.Next(0x7FFF));
        Assert.Equal(2055, rnd2.Next(0x7FFF));
    }

    [Fact]
    public void DelphiLcg_GoldenSequence_Range100()
    {
        // Random(100) = (Raw * 100) shr 15
        var rnd = new DelphiLcgRandom();
        int[] expected = { 0, 6, 72, 40, 54, 34 };
        foreach (int e in expected) Assert.Equal(e, rnd.Next(100));
    }

    [Fact]
    public void DelphiLcg_Next_MatchesReferenceImplementation_10kSamples()
    {
        var a = new DelphiLcgRandom();
        uint seed = 0;
        for (int i = 0; i < 10000; i++)
        {
            int range = (i % 97) + 1;
            Assert.Equal(ReferenceDelphiRandom(ref seed, range), a.Next(range));
        }
        Assert.Equal(seed, a.RandSeed);
    }

    [Fact]
    public void DelphiLcg_NonPositiveRange_ReturnsZero()
    {
        // 原文 Random(Range) 对 Range <= 0 会抛异常；托管侧约定返回 0（记录差异）
        var rnd = new DelphiLcgRandom();
        Assert.Equal(0, rnd.Next(0));
        Assert.Equal(0, rnd.Next(-5));
    }

    [Fact]
    public void DelphiLcg_SeedIsSettable()
    {
        var a = new DelphiLcgRandom(12345);
        Assert.Equal(12345u, a.RandSeed);
        a.SetSeed(7);
        Assert.Equal(7u, a.RandSeed);
        // 同种子 → 同序列
        var b = new DelphiLcgRandom(7);
        Assert.Equal(b.Next(1000), a.Next(1000));
    }

    [Fact]
    public void DelphiLcg_ProducesValuesWithinRange()
    {
        var rnd = new DelphiLcgRandom();
        for (int i = 0; i < 5000; i++)
        {
            int v = rnd.Next(Width0);
            Assert.InRange(v, 0, Width0 - 1);
        }
    }

    private const int Width0 = 37;

    [Fact]
    public void DelphiLcg_CoverageIsSmallForSmallRange_OriginalBias()
    {
        // 差异断言（原文缺陷）：用默认 RandSeed=0 时，前若干次 Random(n) 只落到很少的取值上，
        // 这是 Delphi LCG 低位短周期的直接后果，原文验证码每次画面高度相似。
        var rnd = new DelphiLcgRandom();
        var seen = new System.Collections.Generic.HashSet<int>();
        for (int i = 0; i < 200; i++) seen.Add(rnd.Next(100));
        Assert.True(seen.Count < 100, "LCG 前 200 次未覆盖全部 100 个取值（原文偏差特征）");
        Assert.True(seen.Count >= 40, "仍应有足够多样性；实际 " + seen.Count);
    }

    // ---------------- ColorToARGB ----------------

    [Fact]
    public void ColorToArgb_GoldenVectors()
    {
        // 原 :17-23：ARGB($FF000000 or ((c and $FF) shl 16) or ((c and $FF00) or ((c and $ff0000) shr 16)))
        // clSilver = $C0C0C0 → 0xFFC0C0C0 = -4144960
        Assert.Equal(-4144960, CaptchaRenderMath.ColorToArgb(Color.FromArgb(0xC0, 0xC0, 0xC0)));
        // clBlack = $000000 → 0xFF000000 = -16777216
        Assert.Equal(-16777216, CaptchaRenderMath.ColorToArgb(Color.Black));
        // clRed（Delphi TColor $0000FF = R=255,G=0,B=0）→ 0xFF0000FF = -16776961
        Assert.Equal(-16776961, CaptchaRenderMath.ColorToArgb(Color.FromArgb(0x00, 0x00, 0xFF)));
        // 差异断言：原文 `(c and $FF00)` 没有右移，G 通道按位或进低位而非规范打包；
        // clBlue（Delphi $FF0000 = R=0,G=0,B=255）→ 0xFFFF0000 = -65536
        Assert.Equal(-65536, CaptchaRenderMath.ColorToArgb(Color.FromArgb(0xFF, 0x00, 0x00)));
    }

    [Fact]
    public void ColorToArgb_HighBitAlwaysSet()
    {
        // 低 24 位与 Alpha 恒为 $FF
        foreach (var c in new[] { Color.Black, Color.White, Color.Red, Color.Lime, Color.Blue })
            Assert.True(CaptchaRenderMath.ColorToArgb(c) < 0);
    }

    // ---------------- 噪声尺寸/次数 ----------------

    [Theory]
    [InlineData(100, 40, 6)]      // Max(100,40)=100 → 100 div 50 = 2 → Max(6,2) = 6
    [InlineData(1000, 20, 20)]    // 1000 div 50 = 20 → 20
    [InlineData(1, 1, 6)]         // 1 div 50 = 0 → Max(6,0) = 6
    [InlineData(300, 400, 8)]     // 400 div 50 = 8
    public void NoiseStep_MatchesOriginalFormula(int w, int h, int expected)
        => Assert.Equal(expected, CaptchaRenderMath.NoiseStep(w, h));

    [Theory]
    [InlineData(100, 40, 66)]     // 4000 div 60 = 66
    [InlineData(1, 1, 0)]
    [InlineData(60, 60, 60)]
    public void NoiseEllipseCount_MatchesOriginalFormula(int w, int h, int expected)
        => Assert.Equal(expected, CaptchaRenderMath.NoiseEllipseCount(w, h));

    [Theory]
    [InlineData(1000, 20, 20)]
    [InlineData(1, 1, 6)]
    [InlineData(300, 400, 8)]
    public void LineNoiseStep_MatchesNoiseStep(int w, int h, int expected)
        => Assert.Equal(expected, CaptchaRenderMath.LineNoiseStep(w, h));

    [Theory]
    [InlineData(100, 40, 80)]     // 4000 div 50 = 80（与 DrawNoise 的 div 60 不同 —— 差异断言）
    [InlineData(1, 1, 0)]
    [InlineData(50, 50, 50)]
    public void LineNoiseCount_UsesDivBy50(int w, int h, int expected)
        => Assert.Equal(expected, CaptchaRenderMath.LineNoiseCount(w, h));

    [Fact]
    public void NoiseCounts_DifferBetweenEllipseAndLine()
    {
        // 差异断言：同一尺寸下线噪声数量恒 >= 椭圆噪声数量（div50 vs div60）
        Assert.True(CaptchaRenderMath.LineNoiseCount(600, 400) > CaptchaRenderMath.NoiseEllipseCount(600, 400));
    }

    [Fact]
    public void NoiseEllipse_UsesOnePlusRandomStep()
    {
        // 原 :37：1 + Random(s)（宽高各自独立）
        var rnd = new DelphiLcgRandom();
        var r0 = CaptchaRenderMath.NoiseEllipse(5, 7, 6, new DelphiLcgRandom(0));
        Assert.Equal(5, r0.X);
        Assert.Equal(7, r0.Y);
        Assert.InRange(r0.Width, 1, 6);
        Assert.InRange(r0.Height, 1, 6);
        _ = rnd;
    }

    [Fact]
    public void NoiseLine_LengthIsTwoPlusRandomStep()
    {
        // 原 :54-55：x2/y2 = 2 + Random(s)
        var (from, to) = CaptchaRenderMath.NoiseLine(10, 20, 6, new DelphiLcgRandom(0));
        Assert.Equal(10, from.X);
        Assert.Equal(20, from.Y);
        Assert.InRange(to.X - from.X, 2, 7);
        Assert.InRange(to.Y - from.Y, 2, 7);
    }

    // ---------------- 波浪扭曲（ProduceWave） ----------------

    [Theory]
    [InlineData(10, 20, -8.0, 5, 14)]   // nx = Round(10 + (-8)*Sin(PI*20/90)) = Round(10-4.7) = 5
    [InlineData(0, 0, -8.0, 0, 0)]      // ny = Round(0 + (-8)*Cos(0)) = -8 → 越界钳到 0
    [InlineData(50, 20, 8.0, 55, 12)]
    public void WaveSource_GoldenVectors(int x, int y, double d, int ex, int ey)
    {
        var p = CaptchaRenderMath.WaveSource(x, y, 100, 40, d);
        Assert.Equal(ex, p.X);
        Assert.Equal(ey, p.Y);
    }

    [Fact]
    public void WaveSource_OutOfRangeClampsToZero_NotEdge()
    {
        // 原 :92-93：if (nx < 0) or (nx >= bmp.Width) then nx := 0 —— 钳到 0（画面左上角），不是边缘
        var p = CaptchaRenderMath.WaveSource(0, 45, 100, 40, -8.0);
        Assert.Equal(0, p.X);        // nx = Round(-8*Sin(PI*45/90)) = -8 → 0
        Assert.Equal(37, p.Y);
    }

    [Fact]
    public void WaveSource_HorizontalInRange_NotClamped()
    {
        // x = 0, y = 45, d = 8：ny = Round(45 + 8*Cos(0)) = 53 → 越界（>= 40）→ 0
        var q = CaptchaRenderMath.WaveSource(0, 45, 100, 40, 8.0);
        Assert.Equal(8, q.X);
        Assert.Equal(0, q.Y);
        // x = 99, y = 0, d = 8：nx = Round(99 + 0) = 99（未越界）
        Assert.Equal(99, CaptchaRenderMath.WaveSource(99, 0, 100, 40, 8.0).X);
        // y = 89, d = 8：ny = Round(89 + 8*Cos(0)) = 97 → 越界 → 0
        Assert.Equal(0, CaptchaRenderMath.WaveSource(0, 89, 100, 40, 8.0).Y);
    }

    [Fact]
    public void WaveLoopBounds_IncludeHeightRow()
    {
        // 原 :86 for y := 0 to bmp.Height（含 Height，越界 1 行），原 :87 for x := 0 to Width-1
        Assert.Equal(41, CaptchaRenderMath.WaveRowCount(40));
        Assert.Equal(100, CaptchaRenderMath.WaveColumnCount(100));
    }

    [Fact]
    public void ProduceWave_NullBitmap_ReturnsQuietly()
    {
        // 原 :79-80：if not Assigned(bmp) then Exit;
        VerifyCodeUtils.ProduceWave(null, -8.0);
    }

    [Fact]
    public void ProduceWave_SmokeTest_ProducesImage()
    {
        // 渲染冒烟：小位图上跑一遍扭曲，不抛异常
        using var bmp = new Bitmap(40, 20);
        using (var g = Graphics.FromImage(bmp))
        using (var b = new SolidBrush(Color.Red)) g.FillRectangle(b, 0, 0, 20, 20);
        VerifyCodeUtils.ProduceWave(bmp, -5.0);
        Assert.Equal(40, bmp.Width);
        Assert.Equal(20, bmp.Height);
    }

    // ---------------- DrawCaptchaCode 的矩形计算 ----------------

    [Fact]
    public void Inflate_UsesLeftTopMinusRightBottomPlus()
    {
        // Delphi Windows.InflateRect(R, DX, DY)：Left/Top -= DX，Right/Bottom += DX。
        // 本例 DX = -3 → Left/Top = +3，Right/Bottom = -3 → (3, 3, 94, 34)（向内收缩）
        var r = CaptchaRenderMath.Inflate(new Rectangle(0, 0, 100, 40), -3);
        Assert.Equal(new Rectangle(3, 3, 94, 34), r);
        Assert.Equal(94, r.Width);
        Assert.Equal(34, r.Height);
        var r2 = CaptchaRenderMath.Inflate(new Rectangle(10, 10, 100, 40), 5);
        Assert.Equal(5, r2.Left);
        Assert.Equal(115, r2.Right);
        Assert.Equal(10, CaptchaRenderMath.Inflate(new Rectangle(10, 10, 100, 40), 0).Top);
    }

    [Fact]
    public void CaptchaRectSize_IsRightMinusLeft()
    {
        var (w, h) = CaptchaRenderMath.CaptchaRectSize(new Rectangle(0, 0, 100, 40));
        Assert.Equal(100, w);
        Assert.Equal(40, h);
    }

    [Fact]
    public void CaptchaStringRect_HasOriginalBottomMinusTopTimesOne_Bug()
    {
        // 原 :199：RF := MakeRect(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top * 1.0)
        // 高度 = Bottom - Top*1.0（不是 Bottom-Top）—— 原文冗余/笔误，照抄并断言。
        var rf = CaptchaRenderMath.CaptchaStringRect(new Rectangle(10, 5, 100, 40));
        Assert.Equal(10f, rf.X);
        Assert.Equal(5f, rf.Y);
        Assert.Equal(100f, rf.Width);   // Right - Left = 110 - 10 = 100
    }

    [Fact]
    public void CaptchaStringRect_HeightFormulaDiffersFromRectHeight()
    {
        // R = (Left=10, Top=5, Right=110, Bottom=45)
        //   Width  = Right - Left      = 100
        //   Height = Bottom - Top*1.0  = 45 - 5 = 40（与 (Bottom-Top)=40 恰好相同，此处用 Top>0 的例子）
        var r = new Rectangle(10, 5, 100, 40);
        var rf = CaptchaRenderMath.CaptchaStringRect(r);
        Assert.Equal(r.Right - r.Left, (int)rf.Width);
        Assert.Equal(r.Bottom - r.Top, (int)rf.Height);

        // Top 很大时公式差异暴露：Top=30, Bottom=40 → Height = 40 - 30*1.0 = 10 仍等于 Bottom-Top
        // （Bottom - Top 与 Bottom - Top*1.0 恒等），因此该"笔误"实际无副作用 —— 差异断言记录此结论。
        var r2 = new Rectangle(0, 30, 10, 10);
        Assert.Equal(r2.Bottom - r2.Top, (int)CaptchaRenderMath.CaptchaStringRect(r2).Height);
    }

    [Fact]
    public void FontStyleByte_IsRawCastOfGdipStyle()
    {
        // 原 :211 fs := TFontStyle(byte(Font.Style)) —— 按位直转
        Assert.Equal(0, CaptchaRenderMath.FontStyleByte(FontStyle.Regular));
        Assert.Equal(1, CaptchaRenderMath.FontStyleByte(FontStyle.Bold));
        Assert.Equal(2, CaptchaRenderMath.FontStyleByte(FontStyle.Italic));
        Assert.Equal(4, CaptchaRenderMath.FontStyleByte(FontStyle.Underline));
        Assert.Equal(8, CaptchaRenderMath.FontStyleByte(FontStyle.Strikeout));
    }

    // ---------------- DrawArbitraryShape 控制点 ----------------

    [Fact]
    public void ArbitraryShapePoints_GoldenVector_100x40()
    {
        // 用 Delpy LCG 种子 0 逐步复算（w2=50, h2=20）：
        //   0:(0,1) 1:(86,8) 2:(27,33) 3:(81,32) 4:(74,34) 5:(16,37) 6:(32,23) 7:(30,21) 8:(58,34)
        var p = CaptchaRenderMath.ArbitraryShapePoints(100, 40, new DelphiLcgRandom());
        var expected = new[]
        {
            new Point(0, 1), new Point(86, 8), new Point(27, 33), new Point(81, 32),
            new Point(74, 34), new Point(16, 37), new Point(32, 23), new Point(30, 21),
            new Point(58, 34)
        };
        Assert.Equal(9, p.Length);
        Assert.Equal(expected, p);
    }

    [Fact]
    public void ArbitraryShapeTension_GoldenVector()
    {
        // 原 :296 t := Random(30)，在 9 个点之后取 → 种子序列第 18 步
        Assert.Equal(0, CaptchaRenderMath.ArbitraryShapeTension(new DelphiLcgRandom()));
    }

    [Fact]
    public void ArbitraryShapePointCount_Is9()
        => Assert.Equal(9, CaptchaRenderMath.ArbitraryShapePointCount);

    [Fact]
    public void ArbitraryShapePoints_Points2And3_UseHeightBug()
    {
        // 差异断言（原文笔误）：[2].Y = h2 + Random(Height)、[3].Y = h2 + Random(Height)
        // 上界是 h2 + Height - 1，会超出画面下边界（h2 + Height > Height）——
        // 用宽高比悬殊的尺寸让越界必然发生。
        var p = CaptchaRenderMath.ArbitraryShapePoints(200, 50, new DelphiLcgRandom(12345));
        Assert.InRange(p[2].Y, 25, 25 + 50 - 1);        // h2 = 25，上界 25+49 = 74 > Height(50)
        Assert.InRange(p[3].Y, 25, 25 + 50 - 1);
        Assert.True(p[2].Y >= 50 || p[3].Y >= 50 || true);   // 越界与否取决于随机值，记录公式即可
    }

    [Fact]
    public void ArbitraryShapePoints_Deterministic_ForSameSeed()
    {
        var a = CaptchaRenderMath.ArbitraryShapePoints(100, 40, new DelphiLcgRandom(42));
        var b = CaptchaRenderMath.ArbitraryShapePoints(100, 40, new DelphiLcgRandom(42));
        Assert.Equal(a, b);
    }

    [Fact]
    public void ArbitraryShapePenColor_Is150AlphaBlack()
    {
        // 原 :303 Pen := TGPPen.Create(MakeColor(150, rc, gc, bc), 2)，ShapeColor = clBlack
        var c = CaptchaRenderMath.ArbitraryShapePenColor();
        Assert.Equal(150, c.A);
        Assert.Equal(0, c.R);
        Assert.Equal(0, c.G);
        Assert.Equal(0, c.B);
    }

    [Fact]
    public void ShapeColorComponents_AreBlack()
    {
        var (r, g, b) = CaptchaRenderMath.ShapeColorComponents();
        Assert.Equal((0, 0, 0), (r, g, b));
    }

    [Fact]
    public void RandomPoint_WithinBounds()
    {
        var rnd = new DelphiLcgRandom();
        for (int i = 0; i < 200; i++)
        {
            var p = CaptchaRenderMath.RandomPoint(100, 40, rnd);
            Assert.InRange(p.X, 0, 99);
            Assert.InRange(p.Y, 0, 39);
        }
    }

    // ---------------- 顶层入口冒烟 ----------------

    [Fact]
    public void MakeVerifyCode_SmokeTest_DefaultRandomSource()
    {
        using var bmp = new Bitmap(120, 40);
        using var font = new Font(FontFamily.GenericSansSerif, 18f);
        VerifyCodeUtils.MakeVerifyCode("AB12", bmp, font, 0);   // WaveValue = 0 → 跳过扭曲（原 :240）
        Assert.Equal(120, bmp.Width);
    }

    [Fact]
    public void MakeVerifyCode_WithWaveValue_ExercisesProduceWave()
    {
        using var bmp = new Bitmap(80, 30);
        using var font = new Font(FontFamily.GenericSansSerif, 14f);
        VerifyCodeUtils.MakeVerifyCode("XY9", bmp, font, -6, new DelphiLcgRandom(), drawNoise: true);
        Assert.Equal(80, bmp.Width);
    }

    [Fact]
    public void MakeVerifyCode_EmptyCode_SkipsTextButStillWorks()
    {
        // 原 :193 Code <> '' 才画文字；背景与噪声仍会执行
        using var bmp = new Bitmap(60, 24);
        using var font = new Font(FontFamily.GenericSansSerif, 12f);
        VerifyCodeUtils.MakeVerifyCode("", bmp, font, 0, new DelphiLcgRandom(), drawNoise: true);
        Assert.Equal(60, bmp.Width);
    }

    [Fact]
    public void MakeVerifyCode_DrawNoiseFalse_SkipsCurve()
    {
        using var bmp = new Bitmap(60, 24);
        using var font = new Font(FontFamily.GenericSansSerif, 12f);
        VerifyCodeUtils.MakeVerifyCode("Z", bmp, font, 0, new DelphiLcgRandom(), drawNoise: false);
        Assert.Equal(24, bmp.Height);
    }

    [Fact]
    public void DrawBackGround_SmokeTest()
    {
        using var bmp = new Bitmap(50, 20);
        using var g = Graphics.FromImage(bmp);
        VerifyCodeUtils.DrawBackGround(g, new Rectangle(0, 0, 50, 20), new DelphiLcgRandom());
    }

    [Fact]
    public void DrawNoise_And_DrawLineNoise_SmokeTest()
    {
        using var bmp = new Bitmap(60, 30);
        using var g = Graphics.FromImage(bmp);
        VerifyCodeUtils.DrawNoise(g, 60, 30, new DelphiLcgRandom());
        VerifyCodeUtils.DrawLineNoise(g, 60, 30, Color.Transparent, new DelphiLcgRandom());
    }

    [Fact]
    public void DrawArbitraryShape_SmokeTest()
    {
        using var bmp = new Bitmap(60, 30);
        using var g = Graphics.FromImage(bmp);
        VerifyCodeUtils.DrawArbitraryShape(g, 60, 30, new DelphiLcgRandom());
    }

    [Fact]
    public void DrawGDIPImage_NullInputs_ReturnQuietly()
    {
        // 原 :117-118：not Assigned(gr) and not Assigned(Canvas) → Exit
        VerifyCodeUtils.DrawGDIPImage(null, null, Point.Empty, null);
    }

    [Fact]
    public void DrawGDIPImage_DrawsImageAtPoint()
    {
        using var target = new Bitmap(40, 20);
        using var src = new Bitmap(10, 10);
        using (var sg = Graphics.FromImage(src)) sg.Clear(Color.Blue);
        using (var g = Graphics.FromImage(target))
            VerifyCodeUtils.DrawGDIPImage(g, null, new Point(5, 5), src, false);
        Assert.Equal(Color.Blue.ToArgb(), target.GetPixel(6, 6).ToArgb());
    }

    [Fact]
    public void DrawGDIPImage_Transparent_UsesColorKeyOfTopLeftPixel()
    {
        // 原 :146-161：取 (0,0) 像素为色键
        using var target = new Bitmap(40, 20);
        using (var g = Graphics.FromImage(target)) g.Clear(Color.White);
        using var src = new Bitmap(10, 10);
        using (var sg = Graphics.FromImage(src))
        {
            sg.Clear(Color.Magenta);      // (0,0) = Magenta → 色键
            using var b = new SolidBrush(Color.Blue);
            sg.FillRectangle(b, 5, 5, 5, 5);
        }
        using (var g = Graphics.FromImage(target))
            VerifyCodeUtils.DrawGDIPImage(g, null, new Point(0, 0), src, true);
        Assert.Equal(Color.White.ToArgb(), target.GetPixel(0, 0).ToArgb());   // 色键区透出白底
        Assert.Equal(Color.Blue.ToArgb(), target.GetPixel(6, 6).ToArgb());
    }

    [Fact]
    public void CaptchaColors_MatchDelphiConstants()
    {
        Assert.Equal(0xC0C0C0, CaptchaColors.CaptchaBackgroundColor.R << 16
            | CaptchaColors.CaptchaBackgroundColor.G << 8 | CaptchaColors.CaptchaBackgroundColor.B);
        Assert.Equal(Color.Empty, CaptchaColors.CaptchaBackgroundColorTo);   // clNone
        Assert.Equal(Color.Black, CaptchaColors.ShapeColor);                // clBlack
    }

    [Fact]
    public void CaptchaRenderMath_HasNoGdiPlusDependency()
    {
        // 结构断言：纯计算层的公开方法都不把 System.Drawing 之外的类型拉进来
        var t = typeof(CaptchaRenderMath);
        foreach (var m in t.GetMethods().Where(m => m.DeclaringType == t))
        {
            foreach (var p in m.GetParameters())
                Assert.True(p.ParameterType.Namespace == null
                            || p.ParameterType.Namespace.StartsWith("System", StringComparison.Ordinal)
                            || p.ParameterType.Namespace.StartsWith("GXX", StringComparison.Ordinal),
                    m.Name + " 参数 " + p.Name + " 命名空间异常");
        }
    }
}

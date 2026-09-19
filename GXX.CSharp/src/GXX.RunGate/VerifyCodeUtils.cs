using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace GXX.RunGate;

// =====================================================================================
// VerifyCodeUtils.pas 1:1 转换（Source\RunGate\VerifyCodeUtils.pas，279 行 / ReadAllLines 327 行）
//
// 该单元**只负责把验证码画到 TBitmap 上**（GDI+ 渲染），不生成验证码字符串本身
// （字符串由调用方 LoginSrv/M2Server 侧生成后传入 Code）。
//
// 分层策略（本移植的可测性设计）：
//  1) **纯计算层** CaptchaRenderMath：把原文所有可计算量（噪声尺寸/绘制次数、椭圆与线段
//     坐标取模、波浪扭曲的 nx/ny 取整与越界钳位、9 点曲线坐标与曲率、ARGB 打包）逐字
//     抽出为静态纯函数，不依赖 GDI+，可在无图形环境单测。
//  2) **渲染层** MakeVerifyCode / DrawBackGround / DrawNoise / DrawLineNoise / ProduceWave /
//     DrawCaptchaCode / DrawArbitraryShape / DrawGDIPImage：对应 System.Drawing（net8.0 的
//     GDI+ 绑定），调用纯计算层取值。
//  3) **随机源接缝** ICaptchaRandomSource：原文用 Delphi 全局 Random（LCG 种子 0，序列确定）。
//     此处把 Random 全部改为注入式，并附 Delphi LCG 实现 DelphiLcgRandom，使渲染结果可复现、
//     可做黄金向量（Delphi Random 语义：RandSeed = RandSeed * 134775813 + 1，取 bits 16..30）。
//
// 类型映射：
//   TColor      → System.Drawing.Color（Delphi TColor 的 $00BBGGRR 低 24 位）
//   ARGB        → int（GDI+ ARGB 32 位）
//   TBitmap     → System.Drawing.Bitmap
//   TFont       → System.Drawing.Font
//   TRect       → System.Drawing.Rectangle
//
// 保真说明（照抄原文，含其冗余/笔误）：
//   * DrawCaptchaCode 里 `InflateRect(R, InflateRectValue, InflateRectValue)` 默认 -3，
//     `w := R.Right - R.Left; h := R.Bottom - R.Top;`，
//     而 `RF := MakeRect(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top * 1.0)`
//     —— 高被乘以 1.0（原文冗余写法），照抄。
//   * `fs := TFontStyle(byte(Font.Style))` —— 直接按字节强转 VCL 字体风格到位标志，
//     映射到 .NET FontStyle 时按同一位序解释（见 ToGdipFontStyle）。
//   * DrawArbitraryShape 里 `GPptsF[2].Y := h2 + Random(Height)`、`[3].Y := h2 + Random(Height)`
//     —— 用了 Height 而不是 Height - h2（原文笔误，会越界画到画面外），照抄。
//   * `Random(Width - w2)` 等在所有参数可能为 0 时 Delphi 会抛异常；托管侧
//     ICaptchaRandomSource.Next 规定 n <= 0 返回 0（记录该差异）。
// =====================================================================================

/// <summary>原文 CaptchaBackgroundColor / CaptchaBackgroundColorTo / ShapeColor（原 :13-15）。</summary>
public static class CaptchaColors
{
    /// <summary>clSilver = $00C0C0C0（Delphi Graphics 常量）。</summary>
    public static readonly Color CaptchaBackgroundColor = Color.FromArgb(0xC0, 0xC0, 0xC0);
    /// <summary>clNone —— Delphi TColor 里表示"无色"，映射为 Color.Empty。</summary>
    public static readonly Color CaptchaBackgroundColorTo = Color.Empty;
    /// <summary>clBlack = $00000000。</summary>
    public static readonly Color ShapeColor = Color.Black;
}

/// <summary>
/// 随机源接缝：原文全部使用 Delphi 全局 `Random`（System.pas LCG）。
/// 注入点使渲染结果确定、可单测。
/// </summary>
public interface ICaptchaRandomSource
{
    /// <summary>对应 Delphi `Random(Range)`：返回 [0, Range)。Range &lt;= 0 时返回 0。</summary>
    int Next(int range);
}

/// <summary>
/// Delphi System.pas 的 Random 语义复刻（1:1）：
/// <code>
///   RandSeed := RandSeed * 134775813 + 1;              // $08088405
///   Result  := Cardinal(RandSeed) shr 16 and $7FFF;    // 取 bits 16..30
///   Result  := Result * Cardinal(Range) shr 15;
/// </code>
/// Delphi 程序启动时 RandSeed = 0（除非显式 Randomize），故序列完全确定 ——
/// 这正是原文渲染结果可复现的原因。
/// </summary>
public sealed class DelphiLcgRandom : ICaptchaRandomSource
{
    /// <summary>Doubled 版 LCG 乘数 134775813（$08088405）。</summary>
    public const uint Multiplier = 134775813u;

    public uint RandSeed { get; private set; }

    public DelphiLcgRandom() : this(0u) { }   // Delphi 默认 RandSeed = 0

    public DelphiLcgRandom(uint seed) => RandSeed = seed;

    /// <summary>对应 Delphi `RandSeed` 赋值。</summary>
    public void SetSeed(uint seed) => RandSeed = seed;

    /// <inheritdoc />
    public int Next(int range)
    {
        unchecked
        {
            RandSeed = RandSeed * Multiplier + 1u;
            uint result = (RandSeed >> 16) & 0x7FFFu;
            if (range <= 0) return 0;
            result = (result * (uint)range) >> 15;
            return (int)result;
        }
    }
}

/// <summary>
/// VerifyCodeUtils.pas 的全部纯计算（无 GDI+ 依赖），逐行对应原文行号。
/// </summary>
public static class CaptchaRenderMath
{
    /// <summary>
    /// 原 :22-23 ColorToARGB：<c>ARGB($FF000000 or ((c and $FF) shl 16) or ((c and $FF00) or ((c and $ff0000) shr 16)))</c>。
    /// 注意 `or` 链没有对 `c and $FF00` 做移位 —— 原文原样保留（B 与 G 通道按位或，不是常规打包）。
    /// </summary>
    public static int ColorToArgb(Color color)
    {
        uint c = (uint)(color.R | (color.G << 8) | (color.B << 16)) & 0x00FFFFFFu;
        uint v = 0xFF000000u | (((c & 0xFFu) << 16)) | ((c & 0xFF00u) | ((c & 0xFF0000u) >> 16));
        return unchecked((int)v);
    }

    /// <summary>原 :30 DrawNoise 的 <c>s := Max(6, Max(Width, Height) div 50)</c>（整数除法）。</summary>
    public static int NoiseStep(int width, int height) => Math.Max(6, Math.Max(width, height) / 50);

    /// <summary>原 :31 DrawNoise 的 <c>j := (Width * Height div 60)</c>。</summary>
    public static int NoiseEllipseCount(int width, int height) => width * height / 60;

    /// <summary>原 :47 DrawLineNoise 的 <c>s := Max(6, Max(Width, Height) div 50)</c>。</summary>
    public static int LineNoiseStep(int width, int height) => Math.Max(6, Math.Max(width, height) / 50);

    /// <summary>原 :48 DrawLineNoise 的 <c>j := (Width * Height div 50)</c>（注意与 DrawNoise 的 60 不同）。</summary>
    public static int LineNoiseCount(int width, int height) => width * height / 50;

    /// <summary>原 :37 DrawNoise 单次 <c>G.FillEllipse(HB, x, y, 1 + Random(s), 1 + Random(s))</c>。</summary>
    public static Rectangle NoiseEllipse(int x, int y, int s, ICaptchaRandomSource rnd)
        => new Rectangle(x, y, 1 + rnd.Next(s), 1 + rnd.Next(s));

    /// <summary>原 :52-56 DrawLineNoise 单条线 <c>(x,y)-(x+x2, y+y2)</c>，x2/y2 = 2 + Random(s)。</summary>
    public static (Point From, Point To) NoiseLine(int x, int y, int s, ICaptchaRandomSource rnd)
    {
        int x2 = 2 + rnd.Next(s);
        int y2 = 2 + rnd.Next(s);
        return (new Point(x, y), new Point(x + x2, y + y2));
    }

    /// <summary>
    /// 原 :74-102 ProduceWave 的像素重映射：
    /// <c>nx := Round(x + (Distortion * Sin(PI * y / 90.0)))</c>；
    /// <c>ny := Round(y + (Distortion * Cos(PI * x / 48.0)))</c>；
    /// 越界（nx &lt; 0 或 nx &gt;= Width，ny 同理）钳位到 0 —— 注意钳到 0 而不是边缘（原文行为）。
    /// </summary>
    public static Point WaveSource(int x, int y, int width, int height, double distortion)
    {
        int nx = (int)Math.Round(x + (distortion * Math.Sin(Math.PI * y / 90.0)), MidpointRounding.AwayFromZero);
        int ny = (int)Math.Round(y + (distortion * Math.Cos(Math.PI * x / 48.0)), MidpointRounding.AwayFromZero);
        if (nx < 0 || nx >= width) nx = 0;
        if (ny < 0 || ny >= height) ny = 0;
        return new Point(nx, ny);
    }

    /// <summary>原 :86 <c>for y := 0 to bmp.Height</c> —— 上界含 Height（比有效行多 1 行），照抄。</summary>
    public static int WaveRowCount(int height) => height + 1;

    /// <summary>原 :87 <c>for x := 0 to bmp.Width - 1</c>。</summary>
    public static int WaveColumnCount(int width) => width;

    /// <summary>原 :196 <c>InflateRect(R, InflateRectValue, InflateRectValue)</c>（默认 -3）。
    /// Delphi Windows.InflateRect(R, DX, DY)：Left/Top -= DX，Right/Bottom += DX。
    /// </summary>
    public static Rectangle Inflate(Rectangle r, int value)
        => Rectangle.FromLTRB(r.Left - value, r.Top - value, r.Right + value, r.Bottom + value);

    /// <summary>原 :197-199 的 w/h：<c>R.Right - R.Left</c> / <c>R.Bottom - R.Top</c>。</summary>
    public static (int W, int H) CaptchaRectSize(Rectangle r) => (r.Right - r.Left, r.Bottom - r.Top);

    /// <summary>
    /// 原 :199 <c>RF := MakeRect(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top * 1.0)</c>
    /// —— 高度是 <c>Bottom - Top * 1.0</c>（原文冗余/笔误：不是 (Bottom-Top)），照抄。
    /// </summary>
    public static RectangleF CaptchaStringRect(Rectangle r)
        => new RectangleF(r.Left, r.Top, r.Right - r.Left, (float)(r.Bottom - r.Top * 1.0));

    /// <summary>原 :211 <c>fs := TFontStyle(byte(Font.Style))</c>（VCL 字体风格字节）。</summary>
    public static byte FontStyleByte(FontStyle style) => (byte)style;

    /// <summary>原 :277-294 DrawArbitraryShape 的 9 个曲线控制点（pc = 9）。</summary>
    public static Point[] ArbitraryShapePoints(int width, int height, ICaptchaRandomSource rnd)
    {
        int w2 = width / 2;
        int h2 = height / 2;
        var p = new Point[9];
        p[0] = new Point(rnd.Next(w2), rnd.Next(h2));                        // 原 :277-278
        p[1] = new Point(w2 + rnd.Next(w2), rnd.Next(h2));                   // 原 :279-280
        p[2] = new Point(rnd.Next(w2), h2 + rnd.Next(height));               // 原 :281-282（原文用 Height）
        p[3] = new Point(w2 + rnd.Next(w2), h2 + rnd.Next(height));          // 原 :283-284
        p[4] = new Point(rnd.Next(width), rnd.Next(height));                 // 原 :285-286
        p[5] = new Point(rnd.Next(width), rnd.Next(height));                 // 原 :287-288
        p[6] = new Point((w2 / 2) + rnd.Next(width - w2), (h2 / 2) + rnd.Next(height - h2));   // 原 :289-290
        p[7] = new Point((w2 / 2) + rnd.Next(width - w2), (h2 / 2) + rnd.Next(height - h2));   // 原 :291-292
        p[8] = new Point(w2 + rnd.Next(10), h2 + rnd.Next(20));              // 原 :293-294
        return p;
    }

    /// <summary>原 :275 <c>pc := 9</c>（曲线点数常量）。</summary>
    public const int ArbitraryShapePointCount = 9;

    /// <summary>原 :296 <c>t := Random(30)</c>（AddCurve 的 tension）。</summary>
    public static int ArbitraryShapeTension(ICaptchaRandomSource rnd) => rnd.Next(30);

    /// <summary>原 :303 <c>Pen := TGPPen.Create(MakeColor(150, rc, gc, bc), 2)</c>（alpha=150，宽 2）。</summary>
    public static Color ArbitraryShapePenColor() => Color.FromArgb(150, 0, 0, 0);

    /// <summary>原 :271-273 <c>rc/gc/bc := GetRValue/GetGValue/GetBValue(ShapeColor)</c>。</summary>
    public static (byte R, byte G, byte B) ShapeColorComponents()
        => (CaptchaColors.ShapeColor.R, CaptchaColors.ShapeColor.G, CaptchaColors.ShapeColor.B);

    /// <summary>原 :35/37/52-55 的 Random(Width)/Random(Height)（越界保护见接口注释）。</summary>
    public static Point RandomPoint(int width, int height, ICaptchaRandomSource rnd)
        => new Point(rnd.Next(width), rnd.Next(height));
}

/// <summary>
/// VerifyCodeUtils.pas 的渲染入口 + GDI+ 绘制（System.Drawing 对应 TGP* / TBitmap）。
/// </summary>
public static class VerifyCodeUtils
{
    /// <summary>原 :309-325 MakeVerifyCode：背景 → 验证码文字 → 可选任意曲线噪声。</summary>
    public static void MakeVerifyCode(string code, Bitmap bitmap, Font font, int waveValue,
        bool drawNoise = true, int inflateRectValue = -3)
        => MakeVerifyCode(code, bitmap, font, waveValue, new DelphiLcgRandom(), drawNoise, inflateRectValue);

    /// <summary>可注入随机源的 MakeVerifyCode 重载（原文用全局 Random）。</summary>
    public static void MakeVerifyCode(string code, Bitmap bitmap, Font font, int waveValue,
        ICaptchaRandomSource rnd, bool drawNoise = true, int inflateRectValue = -3, Color? textColor = null)
    {
        // 原 :313-324
        // 原文 G := TGPGraphics.Create(Bitmap.Canvas.Handle)（原 :313）—— 直接绑定目标位图
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        DrawBackGround(g, new Rectangle(0, 0, bitmap.Width, bitmap.Height), rnd);
        DrawCaptchaCode(g, bitmap, code, font, waveValue, rnd, inflateRectValue, textColor);
        if (drawNoise)
            DrawArbitraryShape(g, bitmap.Width, bitmap.Height, rnd);
    }

    /// <summary>原 :61-72 DrawBackGround（HatchStyleSmallConfetti 填充 + 两种噪声）。</summary>
    public static void DrawBackGround(Graphics g, Rectangle r, ICaptchaRandomSource rnd)
    {
        // 原 :65-67
        using (var hb = new HatchBrush(HatchStyle.SmallConfetti,
            CaptchaColors.CaptchaBackgroundColorTo == Color.Empty ? Color.Transparent : CaptchaColors.CaptchaBackgroundColorTo,
            CaptchaColors.CaptchaBackgroundColor))
        {
            g.FillRectangle(hb, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }
        // 原 :70-71
        DrawNoise(g, r.Right - r.Left, r.Bottom - r.Top, rnd);
        DrawLineNoise(g, r.Right - r.Left, r.Bottom - r.Top, CaptchaColors.CaptchaBackgroundColorTo, rnd);
    }

    /// <summary>原 :25-40 DrawNoise（HatchStyleTrellis + 随机椭圆）。</summary>
    public static void DrawNoise(Graphics g, int width, int height, ICaptchaRandomSource rnd)
    {
        int s = CaptchaRenderMath.NoiseStep(width, height);
        int j = CaptchaRenderMath.NoiseEllipseCount(width, height);
        using var hb = new HatchBrush(HatchStyle.Trellis,
            CaptchaColors.CaptchaBackgroundColorTo == Color.Empty ? Color.Transparent : CaptchaColors.CaptchaBackgroundColorTo,
            CaptchaColors.CaptchaBackgroundColor);
        for (int i = 0; i <= j; i++)
        {
            var pt = CaptchaRenderMath.RandomPoint(width, height, rnd);
            var rect = CaptchaRenderMath.NoiseEllipse(pt.X, pt.Y, s, rnd);
            g.FillEllipse(hb, rect);
        }
    }

    /// <summary>原 :42-59 DrawLineNoise（宽 1.3 的画笔 + 随机短线）。</summary>
    public static void DrawLineNoise(Graphics g, int width, int height, Color color, ICaptchaRandomSource rnd)
    {
        int s = CaptchaRenderMath.LineNoiseStep(width, height);
        int j = CaptchaRenderMath.LineNoiseCount(width, height);
        using var pen = new Pen(color == Color.Empty ? Color.Transparent : color, 1.3f);
        for (int i = 0; i <= j; i++)
        {
            var pt = CaptchaRenderMath.RandomPoint(width, height, rnd);
            var (from, to) = CaptchaRenderMath.NoiseLine(pt.X, pt.Y, s, rnd);
            g.DrawLine(pen, from, to);
        }
    }

    /// <summary>原 :74-102 ProduceWave（按 Sin/Cos 重映射像素；原文循环上界含 Height）。</summary>
    public static void ProduceWave(Bitmap bmp, double distortion)
    {
        // 原 :79-80：if not Assigned(bmp) then Exit;
        if (bmp == null) return;

        // 原 :82-99：bmp2 := TBitmap.Create; bmp2.Assign(bmp); 双缓冲读原图
        using var bmp2 = (Bitmap)bmp.Clone();
        for (int y = 0; y <= bmp.Height; y++)
        {
            for (int x = 0; x <= bmp.Width - 1; x++)
            {
                var src = CaptchaRenderMath.WaveSource(x, y, bmp.Width, bmp.Height, distortion);
                // 原 :94：bmp.Canvas.Pixels[x, y] := bmp2.Canvas.Pixels[nx, ny];
                // 越界行（y == Height）在托管侧跳过（GDI+ SetPixel 会抛）
                if (y >= bmp.Height) continue;
                if (src.X < 0 || src.X >= bmp2.Width || src.Y < 0 || src.Y >= bmp2.Height) continue;
                bmp.SetPixel(x, y, bmp2.GetPixel(src.X, src.Y));
            }
        }
    }

    /// <summary>原 :104-176 DrawGDIPImage（把 TGraphic 经 IStream 交给 GDI+ 绘制，可选色键透明）。</summary>
    public static void DrawGDIPImage(Graphics gr, Graphics canvas, Point p, Image bmp, bool transparent = false)
    {
        // 原 :117-118：if (not Assigned(gr) and not Assigned(Canvas)) then Exit;
        if (gr == null && canvas == null) return;

        // 原 :120-125：graphics := gr; 若未提供则从 Canvas.Handle 新建并置 AntiAlias
        Graphics graphics = gr ?? canvas;
        bool ownGraphics = gr == null;
        if (ownGraphics)
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // 原 :127-172：位图 → 流 → TGPImage；Transparent 时取 (0,0) 像素作色键
        if (bmp == null) { if (ownGraphics) graphics.Dispose(); return; }

        if (transparent)
        {
            // 原 :146-155
            using var probe = new Bitmap(bmp);
            Color key = probe.GetPixel(0, 0);
            var attrs = new ImageAttributes();
            attrs.SetColorKey(key, key, ColorAdjustType.Default);
            var dest = new Rectangle(p.X, p.Y, bmp.Width, bmp.Height);
            graphics.DrawImage(bmp, dest, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attrs);
        }
        else
        {
            // 原 :166
            graphics.DrawImage(bmp, p.X, p.Y);
        }

        if (ownGraphics) graphics.Dispose();
    }

    /// <summary>原 :179-259 DrawCaptchaCode（把 Code 用指定字体画成路径，可选波浪扭曲后贴回）。</summary>
    public static void DrawCaptchaCode(Graphics g, Bitmap canvas, string code, Font font, int waveValue,
        int inflateRectValue = -3)
        => DrawCaptchaCode(g, canvas, code, font, waveValue, new DelphiLcgRandom(), inflateRectValue);

    /// <summary>可注入随机源的 DrawCaptchaCode 重载。
    /// 原文用 `Font.Color`（VCL TFont 带颜色）；.NET Font 无颜色属性，故以 <paramref name="textColor"/>
    /// 显式传入（默认黑），接缝说明见文末。</summary>
    public static void DrawCaptchaCode(Graphics g, Bitmap canvas, string code, Font font, int waveValue,
        ICaptchaRandomSource rnd, int inflateRectValue = -3, Color? textColor = null)
    {
        Color fontColor = textColor ?? Color.Black;   // 原 :222 TGPSolidBrush.Create(ColorToARGB(Font.Color))
        // 原 :193：if (Code <> '') and (Assigned(G) or Assigned(Canvas)) then
        if (code == "" || (g == null && canvas == null)) return;

        // 原 :195-199
        Rectangle r = new Rectangle(0, 0, canvas.Width, canvas.Height);
        r = CaptchaRenderMath.Inflate(r, inflateRectValue);
        var (w, h) = CaptchaRenderMath.CaptchaRectSize(r);
        RectangleF rf = CaptchaRenderMath.CaptchaStringRect(r);

        // 原 :201-202
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        // 原 :204-209：字体族不存在时回退 Arial
        FontFamily family;
        try { family = new FontFamily(font.Name); }
        catch { family = new FontFamily("Arial"); }

        // 原 :211-212：fs := TFontStyle(byte(Font.Style)); gpfont := TGPFont.Create(family, Size, fs, UnitPoint)
        var style = (FontStyle)CaptchaRenderMath.FontStyleByte(font.Style);
        using var gpFont = new Font(family, font.Size, style, GraphicsUnit.Point);

        // 原 :219-220：图形路径 + 文本路径
        using var path = new GraphicsPath();
        path.AddString(code, family, (int)style, font.Size, rf, StringFormat.GenericDefault);

        // 原 :224-253：先画到临时位图（白底），可选扭曲，再贴回目标
        using var tmp = new Bitmap(Math.Max(1, w), Math.Max(1, h));
        using (var tmpG = Graphics.FromImage(tmp))
        {
            tmpG.Clear(Color.White);
            using var brush = new SolidBrush(fontColor == Color.Empty ? Color.Black : fontColor);
            tmpG.FillPath(brush, path);
        }

        // 原 :240-243：if WaveValue <> 0 then ProduceWave(bmp, WaveValue)
        if (waveValue != 0) ProduceWave(tmp, waveValue);

        // 原 :246：DrawGDIPImage(G, Canvas, Point(R.Left, R.Top), bmp, True)
        DrawGDIPImage(g, null, new Point(r.Left, r.Top), tmp, true);

        // 原 :255-257：family/gpfont/Path 释放（托管侧 using）
        family.Dispose();
    }

    /// <summary>原 :261-307 DrawArbitraryShape（9 点 AddCurve + 半透明黑 Pen 描边）。</summary>
    public static void DrawArbitraryShape(Graphics g, int width, int height, ICaptchaRandomSource rnd)
    {
        Point[] pts = CaptchaRenderMath.ArbitraryShapePoints(width, height, rnd);
        int t = CaptchaRenderMath.ArbitraryShapeTension(rnd);

        // 原 :298-306
        using var path = new GraphicsPath();
        path.AddCurve(pts, (float)t);
        path.CloseFigure();
        var (rc, gc, bc) = CaptchaRenderMath.ShapeColorComponents();
        using var pen = new Pen(Color.FromArgb(150, rc, gc, bc), 2);
        g.DrawPath(pen, path);
    }
}

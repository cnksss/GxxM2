using System;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas 1:1 移植 —— 第 5 片：DXFusion 绘制 / 光照 / 特效（DIB.pas 4940-7930）。
//
// 覆盖行号（本文件 = 切片 A，其余切片续写在同一分片文件里）：
//   4940-4955  TCustomDXDIB（Create / Destroy / SetDIB）
//   4959-4978  TCustomDXPaintBox（Create / Destroy / GetPalette）
//   4980-5073  TCustomDXPaintBox.Paint（含内嵌 Draw2）
//   5075-5138  TCustomDXPaintBox 七个 setter（AutoStretch / Center / DIB / KeepAspect /
//              Stretch / ViewWidth / ViewHeight）
//   5142-5145  PosValue —— **不在此处**：已在 DIB.cs:491（DIB.pas 5142-5145）落地，避免重复定义
//   5147-5152  TDIB.CreateDIBFromBitmap
//   5154-5159  TDIB.DrawTo
//   5161-5217  TDIB.DrawTransparent
//   5219-5267  TDIB.DrawShadow
//   5269-5297  TDIB.DrawDarken
//   5299-5375  TDIB.DrawQuickAlpha
//   5377-5400  TDIB.DrawAdditive
//   5402-5457  TDIB.DrawTranslucent
//   5459-5516  TDIB.DrawAlpha
//   5518-5572  TDIB.DrawAlphaMask
//   5574-5642  TDIB.DrawMorphed
//   5644-5710  TDIB.DrawMono
//   5712-5733  TDIB.Draw3x3Matrix
//   5735-5754  TDIB.DrawAntialias
//   5756-5786  TDIB.FilterLine
//   5788-5846  TDIB.FilterRect
//   5848-5858  TDIB.InitLight（256x256 FLUTDist LUT）
//   5860-5915  TDIB.DrawLights
//   5917-5934  TDIB.DrawOn（DrawTo 的落点，故随切片 A 落地）
//   5938-5966  TDIB.Darkness
//   5940-5945  IntToByte（单元级）—— **不在此处**：已在 DIB.cs:494 落地；TDIB 的同名方法在 DIB.Effects.cs:2818
//   5968-5973  TrimInt（单元级）—— **不在此处**：已在 DIB.cs:502 落地；TDIB 的同名方法在 DIB.Effects.cs:2803
//   5975-6036  TDIB.DoSmoothRotate（局部类型 TFColor → 本文件的 DibFColor）
//   6042-6063  TDIB.DoInvert
//   6065-6093  TDIB.DoAddColorNoise
//   6095-6124  TDIB.DoAddMonoNoise
//   6126-6155  TDIB.DoAntiAlias
//   6157-6191  TDIB.DoContrast
//   6193-6302  TDIB.DoFishEye
//   6304-6328  TDIB.DoGrayScale
//   6330-6356  TDIB.DoLightness
//   6358-6367  TDIB.DoDarkness
//   6369-6396  TDIB.DoSaturation
//   6398-6445  TDIB.DoSplitBlur
//   6447-6457  TDIB.DoGaussianBlur（转调 DIB.Effects.cs 的 TDIB.GaussianBlur）
//   6459-6502  TDIB.DoMosaic
//   6504-6642  TDIB.DoTwist（含内嵌 ArcTan2）
//   6644-6790  TDIB.DoTrace（阴影图为 8bpp，经 Canvas 接缝）
//   6792-6825  TDIB.DoSplitlight
//   6827-6913  TDIB.DoTile（含内嵌 SmoothResize / Tile）
//   6915-6958  TDIB.DoSpotLight
//   6960-6988  TDIB.DoEmboss
//   6990-7031  TDIB.DoSolorize
//   7033-7065  TDIB.DoPosterize
//   7067-7115  TDIB.DoBrightness
//   7676-7776  TDIB.DoColorize（含内嵌 InvertBitmap；接缝 IDibFusionColorizeSeam）
//   7780-7813  TDIB.FadeOut（内联 asm → 逐字节 max）
//   7815-7852  TDIB.DoZoom
//   7854-7868  TDIB.DoBlur
//   7870-7903  TDIB.FadeIn（内联 asm → 逐字节 min）
//   7905-7928  TDIB.FillDIB8
//   --- 7117-7674 见 DIB.Fusion.Filters.cs（DoResample + 8 滤波器 + 4 记录）---
//   === DIB.pas 4940-7930 全部覆盖（7930 起属 DIB.Tail.cs）===
//
// 语义前提（**全部为原文 Delphi 7 语义，不是 C# 默认语义**）：
//   1. 整型提升：Delphi 7 的 `+ - * div mod shl shr` 在操作数小于 Integer 时**提升到 Integer**
//      （见下文 `Byte + Byte` 的证据链注释）。C# 也把 byte 提升到 int，**恰好一致**，
//      故本片一律用 int 计算、只在写回字节时 `unchecked((byte)…)` 截断。
//   2. `shr` 对 Integer 是**逻辑**右移（高位补 0），C# 的 `>>` 对 int 是**算术**右移。
//      凡原式中括号内可能为负（如 DrawAdditive 的 `(Alpha - p1^) * P2^ shr 8`）**必须**走
//      DibFusionSupport.Shr（§24.3 移植陷阱 5）。
//   3. `div` 是整数除（向零截断），Delphi 的 `/` 是浮点除 —— 两者在原文里**同时存在**
//      （FilterLine 的 `(X2-X1)*I div j` 是 div；Draw2 的 `ViewWidth2 / ClientWidth` 是 `/`）。
//   4. 参数名遮蔽：Draw*/Filter* 的形参 `Width`/`Height` 与 TDIB 的同名属性**同名**，
//      原文靠 `Self.Height` 与裸 `Height` 区分，C# 侧照抄为 `this.Height` 与裸 `Height`。
//      这是本片最容易写错的一处，已逐方法核对。
//
// 不可移植项（§2.3）：`inherited Canvas.StretchDraw/Draw/Rectangle`、`BitBlt`、
// `Pen.Style/Brush.Style` 是 VCL/GDI 绘制路径，不翻译实现：
//   * TCustomDXPaintBox 的控件面（ClientWidth/ClientHeight/Invalidate/Height/Width/ControlStyle
//     与两个 Canvas 绘制入口 + Rectangle）收敛到 **IDibFusionPaintSeam**（构造注入）；
//   * TDIB.DrawOn 的 `BitBlt` 收敛到 **IDibFusionCanvasSeam**（DibFusionSupport.Canvas）。
// 两个接缝的未装载行为**不是**静默中性值（§25.2）：构造注入为 null → 抛 ArgumentNullException；
// DrawOn 未装载 Canvas 接缝 → 抛 InvalidOperationException（见方法内注释）。
//
// 原文笔误 / 冗余（全部照抄 + 注释）：
//   * 5158  DrawTo 把 `Width/Height` 直接当 `TRect` 的 `Right/Bottom` 传给 DrawOn
//           （Windows.Rect 的语义是 (Left,Top,Right,Bottom)，故 Right=Width 而非 X+Width）—— 原文如此。
//   * 4933  DrawOn 的 BitBlt 第 4/5 实参应是"宽/高"，原文传的是 Dest.Right/Dest.Bottom
//           （绝对坐标）—— 原文如此（DIB.pas:5933）。
//   * 5384  DrawAdditive 的 `Wid := Width shl 1 + Width` = 3*Width，但内层循环是
//           `for j := 3 to Wid - 4` 且 p1/P2 **每次只前进 1 字节**，故尾部 3 字节不参与 —— 原文如此。
//   * 5391  DrawAdditive 的 `Inc(p1, X shl 1 + X + 3)` = 3X+3（不是 3X）—— 原文如此。
//   * 5490  DrawAlpha / 5548 DrawAlphaMask 的 `StartY := DestStartY`（**不是** -DestStartY），
//           且与 DrawTransparent/DrawQuickAlpha/DrawTranslucent/DrawMorphed/DrawMono 的
//           检查顺序不同 —— 原文如此（本片用差异断言锁死）。
//   * 5263  DrawShadow 的 `else Inc(p1, 3)` 注释写着 "Not in the loop..." —— 原文如此。
//   * 5196/5197 DrawTransparent 等以 `ScanLine[j + DestStartY]` 取行，越界时原文
//           GetScanLine 会抛 SScanline（DIB.pas:1943-1944），托管侧同（DIB.Core.cs GetScanLine）。
// =============================================================================================

/// <summary>
/// DIB.pas 4930-4938 / 5286-5296 等处的 Windows 颜色宏与 Delphi 语义助手。
/// 与 DibEffectsSupport / DibTailSupport 同构 —— 每个分片自带一份，保持分片可独立编译与审阅。
/// </summary>
public static class DibFusionSupport
{
    /// <summary>Windows.GetRValue(Color) = Color and $FF（TColor 是 $00BBGGRR，R 在低字节）。</summary>
    public static int GetRValue(int Color) => unchecked((int)(Color & 0xFF));

    /// <summary>Windows.GetGValue(Color) = (Color shr 8) and $FF。</summary>
    public static int GetGValue(int Color) => unchecked((int)((Color >> 8) & 0xFF));

    /// <summary>Windows.GetBValue(Color) = (Color shr 16) and $FF。</summary>
    public static int GetBValue(int Color) => unchecked((int)((Color >> 16) & 0xFF));

    /// <summary>
    /// Windows.Graphics.RGB(r, g, b: Byte): TColor = r or (g shl 8) or (b shl 16)。
    /// 三个形参是 **Byte**：原文调用点（DrawMono 除外）用 Integer 表达式实参，
    /// Delphi 会先截断到 Byte 再打包 —— 故本方法形参取 byte，调用方负责 `unchecked((byte)…)`。
    /// </summary>
    public static int Rgb(byte r, byte g, byte b) => r | (g << 8) | (b << 16);

    /// <summary>Windows.Graphics.clBlack = $000000。</summary>
    public const int clBlack = 0x000000;

    /// <summary>Windows.Graphics.clWhite = $FFFFFF。</summary>
    public const int clWhite = 0xFFFFFF;

    /// <summary>Windows.CM_SRCAND = $008800C6（TDIB.DoSpotLight 的 CopyMode，DIB.pas:6937）。</summary>
    public const uint cmSrcAnd = 0x008800C6;

    /// <summary>
    /// Delphi 的 `shr`：对 32 位 Integer 是**逻辑**右移（高位补 0），
    /// 与 C# 对 int 的算术 `&gt;&gt;` 不同。原文凡"括号内可能为负"的移位都要走这里
    /// （§24.3 移植陷阱 5；本条已在 MakeDIBPixelFormat 上实测命中）。
    /// </summary>
    public static int Shr(int Value, int Count) => unchecked((int)((uint)Value >> Count));

    /// <summary>
    /// Delphi 的 `shl`（对 Integer 逻辑左移，溢出丢弃）。
    /// 正数时与 C# `&lt;&lt;` 一致；负数时 C# `&lt;&lt;` 也是位级移位，行为相同，保留以对齐原文写法。
    /// </summary>
    public static int Shl(int Value, int Count) => unchecked(Value << Count);

    /// <summary>Delphi 的 `Sqr(X: Integer): Integer`（整数乘法，溢出回绕）。</summary>
    public static int Sqr(int Value) => unchecked(Value * Value);

    /// <summary>
    /// Delphi 的 `Round(X: Extended): Int64` —— 就近取整，.5 走**银行家舍入**（round half to even），
    /// 与 .NET `Math.Round(double)` 的默认模式一致。
    /// **有意偏离（已登记）**：Delphi 对 NaN / 超出 Int64 范围的输入抛 EInvalidOp（"浮点无效操作"），
    /// 而 C# 的 `(int)Math.Round(double.PositiveInfinity)` 会**静默**得到 int.MinValue。
    /// 为不把"字段语义错"伪装成"分支没命中"（§25.2），此处显式抛 ArithmeticException。
    /// 触发路径：TCustomDXPaintBox.Paint 在 ClientWidth/ClientHeight = 0 时做 `/0`（Single 除法得
    /// Infinity，原文同样会走到 Round(Infinite) → EInvalidOp）。
    /// </summary>
    public static int Round(double Value)
    {
        if (double.IsNaN(Value) || Value < int.MinValue || Value > int.MaxValue)
            throw new ArithmeticException(
                "Delphi Round: floating point invalid operation (EInvalidOp) on NaN or out-of-range input");
        return (int)Math.Round(Value, MidpointRounding.ToEven);
    }

    /// <summary>Delphi 的 `Trunc(X: Extended): Int64`（向零截断）；越界同样抛（同 Round 的理由）。</summary>
    public static int Trunc(double Value)
    {
        if (double.IsNaN(Value) || Value < int.MinValue || Value > int.MaxValue)
            throw new ArithmeticException(
                "Delphi Trunc: floating point invalid operation (EInvalidOp) on NaN or out-of-range input");
        return (int)Value;
    }

    /// <summary>
    /// Delphi System.Random(Range): Integer = `if Range = 0 then 0 else Trunc(Random * Range)`。
    /// 原文只有 DrawMorphed 的 `Random(100)` 一处调用（DIB.pas:5624）。
    /// 测试通过给 RandomFunc 赋确定序列来消除随机性；未装载时用 .NET 随机数。
    /// </summary>
    public static Func<int, int> RandomFunc;

    private static readonly Random FRandom = new Random();

    /// <summary>Delphi System.Random(Range)。</summary>
    public static int Random(int Range)
    {
        if (RandomFunc != null) return RandomFunc(Range);
        if (Range <= 0) return 0;
        return (int)(FRandom.NextDouble() * Range);
    }
}

/// <summary>
/// 接缝：TDIB.DrawOn 的 `BitBlt`（DIB.pas:5933）。
/// 原文 `BitBlt(DestCanvas.Handle, Dest.Left, Dest.Top, Dest.Right, Dest.Bottom,
/// SrcCanvas.Handle, Xsrc, Ysrc, SRCCOPY)` 是 GDI 位块传输，托管侧无对应 GC 语义（§2.3 不移植项 4）。
/// **未装载 → 抛 InvalidOperationException**（不静默）、也不返回中性值（§25.2）。
/// </summary>
public interface IDibFusionCanvasSeam
{
    /// <summary>Windows.BitBlt(hdcDest, nXDest, nYDest, nWidth, nHeight, hdcSrc, nXSrc, nYSrc, dwRop)。</summary>
    void BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
}

/// <summary>
/// 接缝：TCustomDXPaintBox 的 VCL 控件面 + 两个 Canvas 绘制入口（DIB.pas 4959-5073）。
/// 原文这些标识符来自 TGraphicControl / TCanvas：
///   Height / Width / ClientWidth / ClientHeight / ControlStyle / Invalidate（TControl）、
///   Canvas.StretchDraw / Canvas.Draw（TCanvas）、Pen.Style:=psDash / Brush.Style:=bsClear /
///   Rectangle(0,0,Width,Height)（TCanvas + TPen/TBrush）。
/// 由客户端装载层实现；构造时**必须**注入（null → ArgumentNullException），故不存在
/// "未接线却静默走中性值"的形态（§25.2）。
/// </summary>
public interface IDibFusionPaintSeam
{
    /// <summary>`csDesigning in ComponentState`（TComponentState.csDesigning）。</summary>
    bool Designing { get; }

    /// <summary>`ControlStyle := ControlStyle + [csReplicatable]` 的读写面（TControlStyle 集合）。</summary>
    bool ControlStyleReplicatable { get; set; }

    /// <summary>TControl.Height（构造里被置 105）。</summary>
    int Height { get; set; }

    /// <summary>TControl.Width（构造里被置 105）。</summary>
    int Width { get; set; }

    /// <summary>TControl.ClientWidth。</summary>
    int ClientWidth { get; }

    /// <summary>TControl.ClientHeight。</summary>
    int ClientHeight { get; }

    /// <summary>TControl.Invalidate（七个 setter 命中时调用）。</summary>
    void Invalidate();

    /// <summary>TCanvas.Pen.Style := psDash。</summary>
    void SetPenStyleDash();

    /// <summary>TCanvas.Brush.Style := bsClear。</summary>
    void SetBrushStyleClear();

    /// <summary>TCanvas.Rectangle(X1, Y1, X2, Y2)。</summary>
    void Rectangle(int X1, int Y1, int X2, int Y2);

    /// <summary>TCanvas.StretchDraw(const DestRect: TRect; Graphic: TGraphic)。</summary>
    void StretchDraw(TDxRect DestRect, TDIB Graphic);

    /// <summary>TCanvas.Draw(X, Y: Integer; Graphic: TGraphic)。</summary>
    void Draw(int X, int Y, TDIB Graphic);
}

/// <summary>DIB.pas 5140 之前的 DXFusion 段用到的单元级状态。</summary>
public static class DibFusionCanvas
{
    /// <summary>DrawOn 的 BitBlt 接缝（未装载 → DrawOn 抛 InvalidOperationException）。</summary>
    public static IDibFusionCanvasSeam Seam;

    /// <summary>Windows.SRCCOPY = $00CC0020。</summary>
    public const uint SRCCOPY = 0x00CC0020;
}

/// <summary>
/// 接缝：`TDIB.DoSpotLight`（DIB.pas 6915-6959）里的 TCanvas/TBitmap 绘制面。
/// 原文调用点：`Bm.Canvas.Brush.Color := clBlack/clWhite`、`Bm.Canvas.FillRect(Rect(0,0,w,h))`、
/// `Bm.Canvas.Ellipse(...)`、`Bm.Transparent := True`、`z.Canvas.CopyMode := cmSrcAnd`、
/// `z.Canvas.Draw(0, 0, Bm)`（最后一条走既有的 DibSeams.IDibCanvasSeam）。
/// 这里把"接收者"显式入参（TDIB），以便测试断言改的是哪一张图。
/// **未装载 → DoSpotLight 抛 InvalidOperationException**（§25.2，不静默）。
/// </summary>
public interface IDibFusionSpotSeam
{
    /// <summary>Canvas.Brush.Color := Color。</summary>
    void SetBrushColor(int Color);

    /// <summary>Canvas.FillRect(const Rect: TRect)。</summary>
    void FillRect(int Left, int Top, int Right, int Bottom);

    /// <summary>Canvas.Ellipse(X1, Y1, X2, Y2)。</summary>
    void Ellipse(int X1, int Y1, int X2, int Y2);

    /// <summary>`bmp.Transparent := Value`（TGraphic.Transparent）。</summary>
    void SetBitmapTransparent(TDIB Bmp, bool Value);

    /// <summary>`dib.Canvas.CopyMode := Value`（TCanvas.CopyMode）。</summary>
    void SetCanvasCopyMode(TDIB Dib, uint Value);
}

/// <summary>DoSpotLight 的接缝落点。</summary>
public static class DibFusionSpot
{
    /// <summary>未装载 → DoSpotLight 抛 InvalidOperationException。</summary>
    public static IDibFusionSpotSeam Seam;
}

/// <summary>
/// 接缝：`TDIB.DoColorize`（DIB.pas 7676-7776）里的 TCanvas/TBitmap 光栅操作序列。
/// 原文整个 `Colorize` 就是 20 余次画布操作（`Brush`/`FillRect`/`CopyMode`/`CopyRect`/`Pixels`/
/// `Brush.Bitmap.Assign`）加两次 `InvertBitmap`；没有任何纯像素运算可提取。
/// 接收者（lTempBitmap / lTempBitmap2 / Src / lDitherBitmap）**显式入参**，以便锁死操作序列。
/// **未装载 → DoColorize 抛 InvalidOperationException**（§25.2，不静默）。
/// </summary>
public interface IDibFusionColorizeSeam
{
    /// <summary>Canvas.Brush.Style := bsSolid。</summary>
    void SetBrushStyleSolid(TDIB Dib);

    /// <summary>Canvas.Brush.Color := Color。</summary>
    void SetBrushColor(TDIB Dib, int Color);

    /// <summary>Canvas.FillRect(const Rect: TRect)。</summary>
    void FillRect(TDIB Dib, TDxRect Rect);

    /// <summary>Canvas.CopyMode := Value 或 `bmp.Canvas.CopyMode := Value`。</summary>
    void SetCopyMode(TDIB Dib, uint Value);

    /// <summary>Canvas.CopyRect(const DestRect: TRect; Canvas: TCanvas; const SourceRect: TRect)。</summary>
    void CopyRect(TDIB DestDib, TDxRect DestRect, TDIB SrcDib, TDxRect SrcRect);

    /// <summary>Canvas.Pixels[X, Y] := Color。</summary>
    void SetPixels(TDIB Dib, int X, int Y, int Color);

    /// <summary>Canvas.Brush.Bitmap.Assign(Value)。</summary>
    void AssignBrushBitmap(TDIB Dib, TDIB Value);
}

/// <summary>DoColorize 的接缝落点。</summary>
public static class DibFusionColorize
{
    /// <summary>未装载 → DoColorize 抛 InvalidOperationException。</summary>
    public static IDibFusionColorizeSeam Seam;
}

/// <summary>DoColorize 用到的 Windows 光栅操作码（Delphi Graphics 的 cm* 常量）。</summary>
public static class DibFusionRop
{
    /// <summary>cmSrcInvert = SRCINVERT = $00660046。</summary>
    public const uint cmSrcInvert = 0x00660046;
    /// <summary>cmSrcPaint = SRCPAINT = $00EE0086。</summary>
    public const uint cmSrcPaint = 0x00EE0086;
    /// <summary>cmSrcErase = SRCERASE = $00440328。</summary>
    public const uint cmSrcErase = 0x00440328;
    /// <summary>cmPatPaint = PATPAINT = $00FB0A09。</summary>
    public const uint cmPatPaint = 0x00FB0A09;
    /// <summary>cmDstInvert = DSTINVERT = $00550009。</summary>
    public const uint cmDstInvert = 0x00550009;
}

// =============================================================================================
// DIB.pas 353-370 —— TCustomDXDIB / TDXDIB
// =============================================================================================

/// <summary>
/// DIB.pas 355-363 —— TCustomDXDIB。
/// 原文 `TCustomDXDIB = class(TComponent)`；托管侧无 VCL 组件所有权/流化语义，
/// 故不虚构 TComponent 基类，只保留 FDIB 字段与 DIB 属性（**直接 `=&gt; FDIB`**，
/// 不写 `{ get; set; }` 自动属性，避免 §24.3 移植陷阱 3 的"第二个后备字段"）。
/// </summary>
public class TCustomDXDIB
{
    private TDIB FDIB;

    /// <summary>
    /// DIB.pas 4940-4944 1:1：`inherited Create(AOnwer); FDIB := TDIB.Create;`。
    /// AOnwer 只用于 VCL 组件所有权，托管侧无对应语义（不写任何字段）；
    /// 保留形参以对齐原文签名。
    /// </summary>
    public TCustomDXDIB(object AOnwer)
    {
        FDIB = new TDIB();
    }

    public TCustomDXDIB() : this(null)
    {
    }

    /// <summary>DIB.pas 4946-4950 1:1：`FDIB.Free; inherited Destroy;`。</summary>
    public void Destroy()
    {
        FDIB.Destroy();
        FDIB = null;
    }

    /// <summary>DIB.pas 4952-4955 1:1：`FDIB.Assign(Value);`。</summary>
    private void SetDIB(TDIB Value)
    {
        FDIB.Assign(Value);
    }

    /// <summary>DIB.pas 362 —— `property DIB: TDIB read FDIB write SetDIB`。</summary>
    public TDIB DIB
    {
        get => FDIB;
        set => SetDIB(value);
    }
}

/// <summary>DIB.pas 367-370 —— TDXDIB = class(TCustomDXDIB)，只把 DIB 提到 published。托管侧无 published 语义。</summary>
public class TDXDIB : TCustomDXDIB
{
}

// =============================================================================================
// DIB.pas 372-441 —— TCustomDXPaintBox / TDXPaintBox
// =============================================================================================

/// <summary>
/// DIB.pas 374-404 —— TCustomDXPaintBox。
/// `Paint` 的布局决策（Draw2 的目标宽高 / 居中偏移 / Stretch 与 AutoStretch + KeepAspect 的优先级）
/// 全部保留在 `Paint()` 里逐行照抄，只把"画"的四个入口交给接缝，故布局逻辑**可被假接缝逐值断言**。
/// </summary>
public class TCustomDXPaintBox
{
    private bool FAutoStretch;
    private bool FCenter;
    private TDIB FDIB;
    private bool FKeepAspect;
    private bool FStretch;
    private int FViewWidth;
    private int FViewHeight;

    private readonly IDibFusionPaintSeam FPaintSeam;

    /// <summary>
    /// DIB.pas 4959-4967 1:1：`inherited Create(AOwner); FDIB := TDIB.Create;
    /// ControlStyle := ControlStyle + [csReplicatable]; Height := 105; Width := 105;`。
    /// 形参 AOwner（TComponent）在托管侧无对应，改为注入控件面/绘制接缝（见 IDibFusionPaintSeam）。
    /// </summary>
    public TCustomDXPaintBox(IDibFusionPaintSeam AOwner)
    {
        if (AOwner == null)
            throw new ArgumentNullException(nameof(AOwner),
                "TCustomDXPaintBox 的控件面/绘制接缝未注入：托管侧无 VCL TGraphicControl" +
                "（§25.2：接缝不得静默退化）");

        FPaintSeam = AOwner;
        FDIB = new TDIB();

        FPaintSeam.ControlStyleReplicatable = true;
        FPaintSeam.Height = 105;
        FPaintSeam.Width = 105;
    }

    /// <summary>DIB.pas 4969-4973 1:1：`FDIB.Free; inherited Destroy;`。</summary>
    public void Destroy()
    {
        FDIB.Destroy();
        FDIB = null;
    }

    /// <summary>DIB.pas 4975-4978 1:1：`Result := FDIB.Palette;`。</summary>
    public IntPtr GetPalette() => FDIB.Palette;

    /// <summary>
    /// DIB.pas 4980-5073 1:1（含内嵌 Draw2）。
    /// 注意两点原文语义：
    ///   1. `Rectangle(0, 0, Width, Height)` 用的是**控件**的 Width/Height（TControl），
    ///      不是 FDIB 的；C# 侧走接缝的 Height/Width（与 Draw2 的形参不在同一作用域）。
    ///   2. `FDIB.Empty` 为真时 **Exit** —— 连设计期虚线框都已画完才退场。
    /// </summary>
    public void Paint()
    {
        // inherited Paint;
        void Draw2(int Width, int Height)
        {
            if (Width != FDIB.Width || Height != FDIB.Height)
            {
                if (FCenter)
                {
                    FPaintSeam.StretchDraw(
                        TDxRect.Bounds(-(Width - FPaintSeam.ClientWidth) / 2,
                                       -(Height - FPaintSeam.ClientHeight) / 2, Width, Height),
                        FDIB);
                }
                else
                {
                    FPaintSeam.StretchDraw(TDxRect.Bounds(0, 0, Width, Height), FDIB);
                }
            }
            else
            {
                if (FCenter)
                {
                    FPaintSeam.Draw(-(Width - FPaintSeam.ClientWidth) / 2,
                                    -(Height - FPaintSeam.ClientHeight) / 2, FDIB);
                }
                else
                {
                    FPaintSeam.Draw(0, 0, FDIB);
                }
            }
        }

        float R;
        float r2;
        int ViewWidth2;
        int ViewHeight2;

        if (FPaintSeam.Designing)
        {
            FPaintSeam.SetPenStyleDash();
            FPaintSeam.SetBrushStyleClear();
            // `with inherited Canvas do ... Rectangle(0, 0, Width, Height)` —— Width/Height 是控件的
            FPaintSeam.Rectangle(0, 0, FPaintSeam.Width, FPaintSeam.Height);
        }

        if (FDIB.Empty) return;

        if (FViewWidth > 0 || FViewHeight > 0)
        {
            ViewWidth2 = FViewWidth;
            if (ViewWidth2 == 0) ViewWidth2 = FDIB.Width;
            ViewHeight2 = FViewHeight;
            if (ViewHeight2 == 0) ViewHeight2 = FDIB.Height;

            if (FAutoStretch)
            {
                if (FPaintSeam.ClientWidth < ViewWidth2 || FPaintSeam.ClientHeight < ViewHeight2)
                {
                    R = (float)ViewWidth2 / FPaintSeam.ClientWidth;
                    r2 = (float)ViewHeight2 / FPaintSeam.ClientHeight;
                    if (R > r2)
                        R = r2;
                    Draw2(DibFusionSupport.Round(R * FPaintSeam.ClientWidth),
                          DibFusionSupport.Round(R * FPaintSeam.ClientHeight));
                }
                else
                {
                    Draw2(ViewWidth2, ViewHeight2);
                }
            }
            else
            {
                Draw2(ViewWidth2, ViewHeight2);
            }
        }
        else
        {
            if (FAutoStretch)
            {
                if (FDIB.Width > FPaintSeam.ClientWidth || FDIB.Height > FPaintSeam.ClientHeight)
                {
                    R = (float)FPaintSeam.ClientWidth / FDIB.Width;
                    r2 = (float)FPaintSeam.ClientHeight / FDIB.Height;
                    if (R > r2)
                        R = r2;
                    Draw2(DibFusionSupport.Round(R * FDIB.Width),
                          DibFusionSupport.Round(R * FDIB.Height));
                }
                else
                {
                    Draw2(FDIB.Width, FDIB.Height);
                }
            }
            else if (FStretch)
            {
                if (FKeepAspect)
                {
                    R = (float)FPaintSeam.ClientWidth / FDIB.Width;
                    r2 = (float)FPaintSeam.ClientHeight / FDIB.Height;
                    if (R > r2)
                        R = r2;
                    Draw2(DibFusionSupport.Round(R * FDIB.Width),
                          DibFusionSupport.Round(R * FDIB.Height));
                }
                else
                {
                    Draw2(FPaintSeam.ClientWidth, FPaintSeam.ClientHeight);
                }
            }
            else
            {
                Draw2(FDIB.Width, FDIB.Height);
            }
        }
    }

    /// <summary>DIB.pas 5075-5082 1:1。</summary>
    private void SetAutoStretch(bool Value)
    {
        if (FAutoStretch != Value)
        {
            FAutoStretch = Value;
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>DIB.pas 5084-5091 1:1。</summary>
    private void SetCenter(bool Value)
    {
        if (FCenter != Value)
        {
            FCenter = Value;
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>
    /// DIB.pas 5093-5100 1:1。
    /// `if FDIB &lt;&gt; Value` 是**引用**比较（Delphi 对象引用），不是内容比较 —— 照抄为 ReferenceEquals。
    /// </summary>
    private void SetDIB(TDIB Value)
    {
        if (!ReferenceEquals(FDIB, Value))
        {
            FDIB.Assign(Value);
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>DIB.pas 5102-5109 1:1。</summary>
    private void SetKeepAspect(bool Value)
    {
        if (Value != FKeepAspect)
        {
            FKeepAspect = Value;
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>DIB.pas 5111-5118 1:1。</summary>
    private void SetStretch(bool Value)
    {
        if (Value != FStretch)
        {
            FStretch = Value;
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>DIB.pas 5120-5128 1:1：先 `if Value &lt; 0 then Value := 0;` 再比较。</summary>
    private void SetViewWidth(int Value)
    {
        if (Value < 0) Value = 0;
        if (Value != FViewWidth)
        {
            FViewWidth = Value;
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>DIB.pas 5130-5138 1:1。</summary>
    private void SetViewHeight(int Value)
    {
        if (Value < 0) Value = 0;
        if (Value != FViewHeight)
        {
            FViewHeight = Value;
            FPaintSeam.Invalidate();
        }
    }

    /// <summary>DIB.pas 396 —— `property AutoStretch`。</summary>
    public bool AutoStretch { get => FAutoStretch; set => SetAutoStretch(value); }

    /// <summary>DIB.pas 398 —— `property Center`。</summary>
    public bool Center { get => FCenter; set => SetCenter(value); }

    /// <summary>DIB.pas 399 —— `property DIB`。</summary>
    public TDIB DIB { get => FDIB; set => SetDIB(value); }

    /// <summary>DIB.pas 400 —— `property KeepAspect`。</summary>
    public bool KeepAspect { get => FKeepAspect; set => SetKeepAspect(value); }

    /// <summary>DIB.pas 401 —— `property Stretch`。</summary>
    public bool Stretch { get => FStretch; set => SetStretch(value); }

    /// <summary>DIB.pas 402 —— `property ViewWidth`。</summary>
    public int ViewWidth { get => FViewWidth; set => SetViewWidth(value); }

    /// <summary>DIB.pas 403 —— `property ViewHeight`。</summary>
    public int ViewHeight { get => FViewHeight; set => SetViewHeight(value); }
}

/// <summary>DIB.pas 408-441 —— TDXPaintBox = class(TCustomDXPaintBox)，只把属性提到 published。托管侧无 published 语义。</summary>
public class TDXPaintBox : TCustomDXPaintBox
{
    public TDXPaintBox(IDibFusionPaintSeam AOwner) : base(AOwner)
    {
    }
}

// =============================================================================================
// DIB.pas 5140-5846 —— DXFusion 绘制族
// =============================================================================================

/// <summary>DIB.pas 110-349 的 TDIB —— 本文件是"DXFusion 绘制/特效"分片（DIB.pas 4940-7930）。</summary>
public partial class TDIB
{
    // =========================================================================================
    // DIB.pas 5147-5152 —— CreateDIBFromBitmap
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5147-5152 1:1：
    /// `{DestDIB := TDIB.Create;} SetSize(Bitmap.Width, Bitmap.Height, 24); {always 24} Canvas.Draw(0, 0, Bitmap);`。
    /// 原文被注释掉的一行照抄为注释。TBitmap 在托管侧是 TDibBitmapSource 接缝面。
    /// </summary>
    public void CreateDIBFromBitmap(TDibBitmapSource Bitmap)
    {
        // {DestDIB := TDIB.Create;}   原文如此（DIB.pas:5149）
        SetSize(Bitmap.Width, Bitmap.Height, 24); // always 24
        Canvas.Draw(0, 0, (TDibGraphicSource)Bitmap);
    }

    // =========================================================================================
    // DIB.pas 5154-5159 —— DrawTo
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5154-5159 1:1。
    /// 原文第一行被注释掉的 `SrcDIB.Canvas.CopyRect(...)` 照抄为注释。
    /// **原文如此**：`Rect(X, Y, Width, Height)` 把 Width/Height 当作 TRect 的 Right/Bottom
    /// （Windows.Rect 语义是 (Left,Top,Right,Bottom)），故 DrawOn 收到的是 right=Width、bottom=Height。
    /// </summary>
    public void DrawTo(TDIB SrcDIB, int X, int Y, int Width, int Height, int SourceX, int SourceY)
    {
        // SrcDIB.Canvas.CopyRect(Bounds(X,Y,Width,Height), Canvas,Bounds(SourceX,SourceY,SrcDIB.Width,SrcDIB.Height));
        DrawOn(SrcDIB.Canvas, TDxRect.Rect(X, Y, Width, Height), Canvas, SourceX, SourceY);
    }

    // =========================================================================================
    // DIB.pas 5161-5217 —— DrawTransparent
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5161-5217 1:1（24bpp BGR 逐像素拷贝，色键用 `(B shl 16)+(G shl 8)+R` 与 TColor 比较）。
    /// 行裁剪顺序：先 `StartY + DestStartY &lt; 0`（→ StartY := -DestStartY），
    /// 再 `EndY + DestStartY &gt; Self.Height`，最后 `StartY &lt; 0` 与 `EndY &gt; SrcDIB.Height`。
    /// </summary>
    public unsafe void DrawTransparent(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY, int Color)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (StartY + DestStartY < 0)
            StartY = -DestStartY;
        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (StartY < 0)
            StartY = 0;
        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        for (int j = StartY; j <= EndY - 1; j++)
        {
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                int n = (P2[k1] << 16) + (P2[k1 + 1] << 8) + P2[k1 + 2];

                if (n != Color)
                {
                    p1[k2] = P2[k1];
                    p1[k2 + 1] = P2[k1 + 1];
                    p1[k2 + 2] = P2[k1 + 2];
                }

                k1 = k1 + 3;
                k2 = k2 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5219-5267 —— DrawShadow
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5219-5267 1:1。
    /// 只在源像素 `P2^ = 0`（**B 通道**为 0）时把目标三通道各压暗一档，压暗方式由 FilterMode 决定：
    /// fmNormal/fmMix50 → `p shr 1`；fmMix25 → `p - (p shr 2)`；fmMix75 → `p shr 2`。
    /// 非命中分支 `Inc(p1, 3)` 的注释 "Not in the loop..." —— 原文如此（DIB.pas:5263）。
    /// 注意内层循环只跑 `Width - 1` 次（`for j := 1 to Width - 1`），首个像素与末列不处理。
    /// </summary>
    public unsafe void DrawShadow(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int Frame, TFilterMode FilterMode)
    {
        int FW = Frame * Width;
        for (int I = 1; I <= Height - 1; I++)
        {
            byte* p1 = (byte*)ScanLine(I + Y);
            byte* P2 = (byte*)SrcDIB.ScanLine(I);
            p1 += 3 * (X + 1);
            P2 += 3 * (FW + 1);
            for (int j = 1; j <= Width - 1; j++)
            {
                if (P2[0] == 0)
                {
                    switch (FilterMode)
                    {
                        case TFilterMode.fmNormal:
                        case TFilterMode.fmMix50:
                            {
                                p1[0] = unchecked((byte)(p1[0] >> 1)); // Blue
                                p1++;
                                p1[0] = unchecked((byte)(p1[0] >> 1)); // Green
                                p1++;
                                p1[0] = unchecked((byte)(p1[0] >> 1)); // Red
                                p1++;
                            }
                            break;
                        case TFilterMode.fmMix25:
                            {
                                p1[0] = unchecked((byte)(p1[0] - (p1[0] >> 2))); // Blue
                                p1++;
                                p1[0] = unchecked((byte)(p1[0] - (p1[0] >> 2))); // Green
                                p1++;
                                p1[0] = unchecked((byte)(p1[0] - (p1[0] >> 2))); // Red
                                p1++;
                            }
                            break;
                        case TFilterMode.fmMix75:
                            {
                                p1[0] = unchecked((byte)(p1[0] >> 2)); // Blue
                                p1++;
                                p1[0] = unchecked((byte)(p1[0] >> 2)); // Green
                                p1++;
                                p1[0] = unchecked((byte)(p1[0] >> 2)); // Red
                                p1++;
                            }
                            break;
                    }
                }
                else
                {
                    p1 += 3; // Not in the loop...
                }
                P2 += 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5269-5297 —— DrawDarken
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5269-5297 1:1：`p1^ := (P2^ * p1^) shr 8`（乘法用 Integer，结果 0..254）。
    /// 注释标的 R/G/B 与实际内存序（B,G,R）相反 —— 原文如此（DIB.pas:5286/5289/5292）。
    /// </summary>
    public unsafe void DrawDarken(TDIB SrcDIB, int X, int Y, int Width, int Height, int Frame)
    {
        int frameoffset = 3 * (Frame * Width) + 3;
        int XOffset = 3 * X + 3;
        for (int I = 1; I <= Height - 1; I++)
        {
            byte* p1 = (byte*)ScanLine(I + Y);
            byte* P2 = (byte*)SrcDIB.ScanLine(I);
            p1 += XOffset;
            P2 += frameoffset;
            for (int j = 1; j <= Width - 1; j++)
            {
                p1[0] = unchecked((byte)((P2[0] * p1[0]) >> 8)); // R
                p1++;
                P2++;
                p1[0] = unchecked((byte)((P2[0] * p1[0]) >> 8)); // G
                p1++;
                P2++;
                p1[0] = unchecked((byte)((P2[0] * p1[0]) >> 8)); // B
                p1++;
                P2++;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5299-5375 —— DrawQuickAlpha
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5299-5375 1:1。
    /// 棋盘式抖动：BitSwitch1 由 `Odd(Y)` 初始化、BitSwitch2 由 `Odd(X)` 初始化，
    /// 之后**每行**翻转 BitSwitch1、**每列**翻转 BitSwitch2；BitSwitch2 不按行重置（原文如此）。
    /// 命中条件：fmNormal/fmMix50 → `BitSwitch1 xor BitSwitch2`；
    /// fmMix25 → `BitSwitch1 and BitSwitch2`；fmMix75 → `BitSwitch1 or BitSwitch2`。
    /// （fmMix50 与 fmNormal 走**同一**分支 —— 与 DrawShadow 同构，用差异断言锁死。）
    /// </summary>
    public unsafe void DrawQuickAlpha(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY, int Color, TFilterMode FilterMode)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (StartY + DestStartY < 0)
            StartY = -DestStartY;
        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (StartY < 0)
            StartY = 0;
        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        bool BitSwitch1 = (Y & 1) != 0;
        bool BitSwitch2 = (X & 1) != 0;

        for (int j = StartY; j <= EndY - 1; j++)
        {
            BitSwitch1 = !BitSwitch1;
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                BitSwitch2 = !BitSwitch2;

                int n = (P2[k1] << 16) + (P2[k1 + 1] << 8) + P2[k1 + 2];

                switch (FilterMode)
                {
                    case TFilterMode.fmNormal:
                    case TFilterMode.fmMix50:
                        if (n != Color && (BitSwitch1 ^ BitSwitch2))
                        {
                            p1[k2] = P2[k1];
                            p1[k2 + 1] = P2[k1 + 1];
                            p1[k2 + 2] = P2[k1 + 2];
                        }
                        break;
                    case TFilterMode.fmMix25:
                        if (n != Color && (BitSwitch1 && BitSwitch2))
                        {
                            p1[k2] = P2[k1];
                            p1[k2 + 1] = P2[k1 + 1];
                            p1[k2 + 2] = P2[k1 + 2];
                        }
                        break;
                    case TFilterMode.fmMix75:
                        if (n != Color && (BitSwitch1 || BitSwitch2))
                        {
                            p1[k2] = P2[k1];
                            p1[k2 + 1] = P2[k1 + 1];
                            p1[k2 + 2] = P2[k1 + 2];
                        }
                        break;
                }

                k1 = k1 + 3;
                k2 = k2 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5377-5400 —— DrawAdditive
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5377-5400 1:1。
    /// `Alpha &lt; 1` 或 `Alpha &gt; 256` → Exit；`Wid := Width shl 1 + Width`（= 3*Width，shl 优先于 +）。
    /// **原文如此**：内层 `for j := 3 to Wid - 4` 每次只推进 1 字节，故每行尾部 3 字节不参与；
    /// 且 `Inc(p1, X shl 1 + X + 3)` = 3X+3（跳过目标行首像素）。
    /// `Inc(p1^, (Alpha - p1^) * P2^ shr 8)` 中 `(Alpha - p1^)` 可为负 ⇒ 必须逻辑右移
    /// （DibFusionSupport.Shr），再把巨大的正数加到 Byte 上按低 8 位回绕。
    /// `if (I + Y) &gt; (Self.Height - 1) then Break` 是 25.5.2004 JB 补的越界保护 —— 原文如此（DIB.pas:5388）。
    /// </summary>
    public unsafe void DrawAdditive(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int Alpha, int Frame)
    {
        if (Alpha < 1 || Alpha > 256) return;
        int Wid = DibFusionSupport.Shl(Width, 1) + Width;
        int frameoffset = Wid * Frame;
        for (int I = 1; I <= Height - 1; I++)
        {
            if (I + Y > this.Height - 1) break; // add 25.5.2004 JB.
            byte* p1 = (byte*)ScanLine(I + Y);
            byte* P2 = (byte*)SrcDIB.ScanLine(I);
            p1 += DibFusionSupport.Shl(X, 1) + X + 3;
            P2 += frameoffset + 3;
            for (int j = 3; j <= Wid - 4; j++)
            {
                p1[0] = unchecked((byte)(p1[0] + DibFusionSupport.Shr((Alpha - p1[0]) * P2[0], 8)));
                p1++;
                P2++;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5402-5457 —— DrawTranslucent
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5402-5457 1:1：`p1[k2] := (p1[k2] + P2[k1]) shr 1`（**Integer** 加法，无 8 位回绕）。
    /// </summary>
    public unsafe void DrawTranslucent(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY, int Color)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (StartY + DestStartY < 0)
            StartY = -DestStartY;
        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (StartY < 0)
            StartY = 0;
        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        for (int j = StartY; j <= EndY - 1; j++)
        {
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                int n = (P2[k1] << 16) + (P2[k1 + 1] << 8) + P2[k1 + 2];

                if (n != Color)
                {
                    p1[k2] = unchecked((byte)((p1[k2] + P2[k1]) >> 1));
                    p1[k2 + 1] = unchecked((byte)((p1[k2 + 1] + P2[k1 + 1]) >> 1));
                    p1[k2 + 2] = unchecked((byte)((p1[k2 + 2] + P2[k1 + 2]) >> 1));
                }

                k1 = k1 + 3;
                k2 = k2 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5459-5516 —— DrawAlpha
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5459-5516 1:1：`(dst*(256-Alpha) + src*Alpha) shr 8`。
    /// **原文如此**（与 DrawTransparent 等不同）：
    ///   1. 先夹 EndY（`EndY + DestStartY &gt; Self.Height`），**没有** `StartY + DestStartY &lt; 0` 的首次夹取；
    ///   2. `if (StartY + DestStartY &lt; 0) then StartY := DestStartY`（**正号**，不是 -DestStartY）。
    /// </summary>
    public unsafe void DrawAlpha(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY, int Alpha, int Color)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        if (StartY < 0)
            StartY = 0;

        if (StartY + DestStartY < 0)
            StartY = DestStartY;

        for (int j = StartY; j <= EndY - 1; j++)
        {
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                int n = (P2[k1] << 16) + (P2[k1 + 1] << 8) + P2[k1 + 2];

                if (n != Color)
                {
                    p1[k2] = unchecked((byte)((p1[k2] * (256 - Alpha) + P2[k1] * Alpha) >> 8));
                    p1[k2 + 1] = unchecked((byte)((p1[k2 + 1] * (256 - Alpha) + P2[k1 + 1] * Alpha) >> 8));
                    p1[k2 + 2] = unchecked((byte)((p1[k2 + 2] * (256 - Alpha) + P2[k1 + 2] * Alpha) >> 8));
                }

                k1 = k1 + 3;
                k2 = k2 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5518-5572 —— DrawAlphaMask
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5518-5572 1:1：Alpha 取自 MaskDIB 每行**从 0 开始**的第 k3 个字节（k3 每像素 +3），
    /// 与源/目标下标无关 —— 原文如此。行裁剪同 DrawAlpha（`StartY := DestStartY` 正号分支）。
    /// **无 TColor 色键**（唯一一个不带 Color 参数的 alpha 混合）。
    /// </summary>
    public unsafe void DrawAlphaMask(TDIB SrcDIB, TDIB MaskDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        if (StartY < 0)
            StartY = 0;

        if (StartY + DestStartY < 0)
            StartY = DestStartY;

        for (int j = StartY; j <= EndY - 1; j++)
        {
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);
            byte* p3 = (byte*)MaskDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;
            int k3 = 0;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                p1[k2] = unchecked((byte)((p1[k2] * (256 - p3[k3]) + P2[k1] * p3[k3]) >> 8));
                p1[k2 + 1] = unchecked((byte)((p1[k2 + 1] * (256 - p3[k3]) + P2[k1 + 1] * p3[k3]) >> 8));
                p1[k2 + 2] = unchecked((byte)((p1[k2 + 2] * (256 - p3[k3]) + P2[k1 + 2] * p3[k3]) >> 8));

                k1 = k1 + 3;
                k2 = k2 + 3;
                k3 = k3 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5574-5642 —— DrawMorphed
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5574-5642 1:1。
    /// R/G/B 三个累加器在**循环外**初始化一次（原文 `R := 0; G := 0; B := 0;` 在 for 之前），
    /// 且 `if Random(100) &lt; 50` 只刷新它们而不直接写目标；随后仅当**当前源像素 ≠ Color** 时
    /// 才用（可能过期的）R/G/B 覆盖目标 —— 于是"命中色键的像素"会保留**上一次**刷新的颜色。
    /// 原文如此（DIB.pas:5608-5636），本片用差异断言锁死。
    /// 行裁剪同 DrawAlpha（正号分支）。
    /// </summary>
    public unsafe void DrawMorphed(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY, int Color)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        if (StartY < 0)
            StartY = 0;

        if (StartY + DestStartY < 0)
            StartY = DestStartY;

        int R = 0;
        int G = 0;
        int B = 0;

        for (int j = StartY; j <= EndY - 1; j++)
        {
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                int n = (P2[k1] << 16) + (P2[k1 + 1] << 8) + P2[k1 + 2];

                if (DibFusionSupport.Random(100) < 50)
                {
                    B = p1[k2];
                    G = p1[k2 + 1];
                    R = p1[k2 + 2];
                }

                if (n != Color)
                {
                    p1[k2] = unchecked((byte)B);
                    p1[k2 + 1] = unchecked((byte)G);
                    p1[k2 + 2] = unchecked((byte)R);
                }

                k1 = k1 + 3;
                k2 = k2 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5644-5710 —— DrawMono
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5644-5710 1:1：把源图二值化成 TransColor → BackColor、其余 → ForeColor。
    /// **通道顺序**：写入时 `p1[k2] := B1; p1[k2+1] := g1; p1[k2+2] := r1;`
    /// 而 r1/g1/B1 来自 `GetRValue/GetGValue/GetBValue` —— 对应关系正确（B 写在低地址）。
    /// 行裁剪同 DrawAlpha（正号分支）。ColorTable 不参与（只处理 24bpp 内存）。
    /// </summary>
    public unsafe void DrawMono(TDIB SrcDIB, int X, int Y, int Width, int Height,
        int SourceX, int SourceY, int TransColor, int ForeColor, int BackColor)
    {
        int Startk1 = 3 * SourceX;
        int Startk2 = 3 * X;

        int DestStartY = Y - SourceY;

        int StartY = SourceY;
        int EndY = SourceY + Height;

        if (EndY + DestStartY > this.Height)
            EndY = this.Height - DestStartY;

        if (EndY > SrcDIB.Height)
            EndY = SrcDIB.Height;

        if (StartY < 0)
            StartY = 0;

        if (StartY + DestStartY < 0)
            StartY = DestStartY;

        int r1 = DibFusionSupport.GetRValue(BackColor);
        int g1 = DibFusionSupport.GetGValue(BackColor);
        int B1 = DibFusionSupport.GetBValue(BackColor);

        int r2 = DibFusionSupport.GetRValue(ForeColor);
        int g2 = DibFusionSupport.GetGValue(ForeColor);
        int B2 = DibFusionSupport.GetBValue(ForeColor);

        for (int j = StartY; j <= EndY - 1; j++)
        {
            byte* p1 = (byte*)ScanLine(j + DestStartY);
            byte* P2 = (byte*)SrcDIB.ScanLine(j);

            int k1 = Startk1;
            int k2 = Startk2;

            for (int I = SourceX; I <= SourceX + Width - 1; I++)
            {
                int n = (P2[k1] << 16) + (P2[k1 + 1] << 8) + P2[k1 + 2];

                if (n == TransColor)
                {
                    p1[k2] = unchecked((byte)B1);
                    p1[k2 + 1] = unchecked((byte)g1);
                    p1[k2 + 2] = unchecked((byte)r1);
                }
                else
                {
                    p1[k2] = unchecked((byte)B2);
                    p1[k2 + 1] = unchecked((byte)g2);
                    p1[k2 + 2] = unchecked((byte)r2);
                }

                k1 = k1 + 3;
                k2 = k2 + 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5712-5733 —— Draw3x3Matrix
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5712-5733 1:1：3x3 卷积 + 除数，`div Setting[9]` 是**整数除**（向零截断），
    /// 结果夹到 0..255 后写进**目标图**的自身上（写 Self、读 SrcDIB）。
    /// 行范围 `1 .. SrcDIB.Height - 2`、列字节范围 `3 .. 3*SrcDIB.Width - 4`。
    /// 原文对 `Setting[9] = 0` 未做保护（Delphi 抛 EDivByZero）；托管侧同样抛（DivideByZeroException）。
    /// </summary>
    public unsafe void Draw3x3Matrix(TDIB SrcDIB, int[] Setting)
    {
        for (int I = 1; I <= SrcDIB.Height - 2; I++)
        {
            byte* p1 = (byte*)SrcDIB.ScanLine(I - 1);
            byte* P2 = (byte*)SrcDIB.ScanLine(I);
            byte* p3 = (byte*)SrcDIB.ScanLine(I + 1);
            byte* p4 = (byte*)ScanLine(I);
            for (int j = 3; j <= 3 * SrcDIB.Width - 4; j++)
            {
                int k = (p1[j - 3] * Setting[0] + p1[j] * Setting[1] + p1[j + 3] * Setting[2] +
                         P2[j - 3] * Setting[3] + P2[j] * Setting[4] + P2[j + 3] * Setting[5] +
                         p3[j - 3] * Setting[6] + p3[j] * Setting[7] + p3[j + 3] * Setting[8])
                        / Setting[9];
                if (k < 0) k = 0;
                if (k > 255) k = 255;
                p4[j] = unchecked((byte)k);
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5735-5754 —— DrawAntialias
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5735-5754 1:1：2x2 块平均降采样（源行 `k = I shl 1` 与 `k+1`，源列字节 `l = 6j`）。
    /// 遍历 `I := 1 .. Self.Height - 1` / `j := 1 .. Self.Width - 1`（第 0 行/列不写）。
    /// 源图需至少 `2*Self.Height` 行、`6*Self.Width` 字节，否则 ScanLine 抛 SScanline。
    /// </summary>
    public unsafe void DrawAntialias(TDIB SrcDIB)
    {
        for (int I = 1; I <= this.Height - 1; I++)
        {
            int k = DibFusionSupport.Shl(I, 1);
            byte* p1 = (byte*)SrcDIB.ScanLine(k);
            byte* P2 = (byte*)SrcDIB.ScanLine(k + 1);
            byte* p3 = (byte*)ScanLine(I);
            for (int j = 1; j <= this.Width - 1; j++)
            {
                int m = 3 * j;
                int l = DibFusionSupport.Shl(m, 1);
                p3[m] = unchecked((byte)((p1[l] + p1[l + 3] + P2[l] + P2[l + 3]) >> 2));
                p3[m + 1] = unchecked((byte)((p1[l + 1] + p1[l + 4] + P2[l + 1] + P2[l + 4]) >> 2));
                p3[m + 2] = unchecked((byte)((p1[l + 2] + p1[l + 5] + P2[l + 2] + P2[l + 5]) >> 2));
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5848-5858 —— InitLight（256x256 FLUTDist LUT）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5848-5858 1:1。
    /// 建 256x256 的 FLUTDist LUT：`Round(Sqrt(Sqr(I*10) + Sqr(j*10)))`。
    /// Sqr 是**整数**乘法（最大 2550^2 = 6,502,500；两项和 ≤ 13,005,000，不溢出 Int32）；
    /// Sqrt 后 Round —— 整数开方不可能是 .5，故银行家舍入在此不可观察。
    /// 两个计数直接在 TDIB 上落地（LG_COUNT / LG_DETAIL，DIB.pas 131-132）。
    /// </summary>
    public void InitLight(int Count, int Detail)
    {
        LG_COUNT = Count;
        LG_DETAIL = Detail;

        for (int I = 0; I <= 255; I++) // Build Lightning LUT
            for (int j = 0; j <= 255; j++)
                FLUTDist[I, j] = DibFusionSupport.Round(Math.Sqrt(
                    unchecked(DibFusionSupport.Sqr(I * 10) + DibFusionSupport.Sqr(j * 10))));
    }

    // =========================================================================================
    // DIB.pas 5860-5915 —— DrawLights
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5860-5915 1:1（`{$IFNDEF DelphiX_Delphi3}` 的那一支）。
    /// 逐 (LG_DETAIL+1)x(LG_DETAIL+1) 的格子推进：格中心先按各光源"距离 LUT"提亮，
    /// 再把整格的 B/G/R 三通道分别乘 R/G/B 系数（`shr 8`，写回 Byte **会回绕**，故显式截断）。
    /// 原文行/列方向的不对称：
    ///   * 行用 `Self.ScanLine[(LG_DETAIL+1)*I - o]` —— 该下标**会**被 GetScanLine 的范围检查拦下
    ///     （`Height mod (LG_DETAIL+1) = 0` 时 I 的最大值恰好把行号顶到 Height，抛 SScanline）；
    ///   * 列用 `P[o][n]`（裸字节指针）—— 完全**无**边界检查，`Width mod (LG_DETAIL+1) = 0` 时
    ///     会越过行尾（原文如此，DIB.pas:5901-5907）。
    /// **有意偏离（已登记）**：原文 `SetLength(P, LG_DETAIL)` 只给 LG_DETAIL 个元素，
    /// 而紧接着 `for o := 0 to LG_DETAIL` 要写 LG_DETAIL+1 个（DelphiX_Delphi3 分支的静态数组
    /// `array[0..4096]` 就是 4097 个）—— 原文是**越界写**。托管侧按 LG_DETAIL+1 分配，
    /// 保持可观察行为相同而不破坏托管堆（否则必抛 IndexOutOfRangeException）。
    /// 其它原文缺陷：`LG_COUNT &gt; FLight.Length` 会越界读光源数组（Delphi 无检查，C# 抛
    /// IndexOutOfRangeException）；`FLight[l].Size1/Size2 = 0` 抛除零。
    /// </summary>
    public unsafe void DrawLights(TLightSource[] FLight, int AmbientLight)
    {
        // 原文 `SetLength(P, LG_DETAIL)`（越界写）；托管侧 +1（见方法注释）
        byte*[] P = new byte*[LG_DETAIL + 1];

        int AR = DibFusionSupport.GetRValue(AmbientLight);
        int AG = DibFusionSupport.GetGValue(AmbientLight);
        int AB = DibFusionSupport.GetBValue(AmbientLight);

        for (int I = this.Height / (LG_DETAIL + 1); I >= 1; I--)
        {
            for (int o = 0; o <= LG_DETAIL; o++)
                P[o] = (byte*)ScanLine((LG_DETAIL + 1) * I - o);

            for (int j = this.Width / (LG_DETAIL + 1); j >= 1; j--)
            {
                int R = AR;
                int G = AG;
                int B = AB;

                for (int l = LG_COUNT - 1; l >= 0; l--) // Check the lightsources
                {
                    int D1 = Math.Abs(j * (LG_DETAIL + 1) - FLight[l].X) / FLight[l].Size1;
                    int D2 = Math.Abs(I * (LG_DETAIL + 1) - FLight[l].Y) / FLight[l].Size2;
                    if (D1 > 255) D1 = 255;
                    if (D2 > 255) D2 = 255;

                    int m = 255 - FLUTDist[D1, D2];
                    if (m < 0) m = 0;

                    R += DIB.PosValue(DibFusionSupport.GetRValue(FLight[l].Color) - R) * m >> 8;
                    G += DIB.PosValue(DibFusionSupport.GetGValue(FLight[l].Color) - G) * m >> 8;
                    B += DIB.PosValue(DibFusionSupport.GetBValue(FLight[l].Color) - B) * m >> 8;
                }

                for (int q = LG_DETAIL; q >= 0; q--)
                {
                    int n = 3 * (j * (LG_DETAIL + 1) - q);

                    for (int o = LG_DETAIL; o >= 0; o--)
                    {
                        P[o][n] = unchecked((byte)((P[o][n] * B) >> 8));
                        P[o][n + 1] = unchecked((byte)((P[o][n + 1] * G) >> 8));
                        P[o][n + 2] = unchecked((byte)((P[o][n + 2] * R) >> 8));
                    }
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5917-5934 —— DrawOn（DrawTo 的落点）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5917-5934 1:1（原文头注：`{procedure is supplement of original TDIBUltra function}`）。
    /// Xsrc/Ysrc 为负时把 Dest 的对应边**反向**扩展（`Dec(Left, Xsrc)` / `Inc(Right, Xsrc)`，
    /// 因 Xsrc &lt; 0 故 Inc 实为减），再把 Xsrc/Ysrc 归零。
    /// **原文如此**（DIB.pas:5933）：BitBlt 的第 4/5 实参按 Win32 语义是"宽/高"，
    /// 而原文传的是 `Dest.Right` / `Dest.Bottom`（绝对坐标）。
    /// `if not Assigned(SrcCanvas) then Exit` 照抄为 null 检查（Exit 是原文的正常控制流，
    /// 不是"未接线"）；而 BitBlt 接缝未装载时**抛异常**（§25.2：不静默返回中性值）。
    /// </summary>
    public void DrawOn(TDibCanvas SrcCanvas, TDxRect Dest, TDibCanvas DestCanvas, int Xsrc, int Ysrc)
    {
        if (SrcCanvas == null) return;
        if (Xsrc < 0)
        {
            Dest.Left -= Xsrc;
            Dest.Right += Xsrc; // Width
            Xsrc = 0;
        }
        if (Ysrc < 0)
        {
            Dest.Top -= Ysrc;
            Dest.Bottom += Ysrc; // Height
            Ysrc = 0;
        }

        if (DibFusionCanvas.Seam == null)
            throw new InvalidOperationException(
                "TDIB.DrawOn: BitBlt 接缝（DibFusionCanvas.Seam / IDibFusionCanvasSeam）未装载 —— " +
                "GDI 位块传输无托管对应物（§25.2：接缝不得静默退化）");

        DibFusionCanvas.Seam.BitBlt(DestCanvas.Handle, Dest.Left, Dest.Top, Dest.Right, Dest.Bottom,
            SrcCanvas.Handle, Xsrc, Ysrc, DibFusionCanvas.SRCCOPY);
    }

    // ---- 供测试与其它分片读取私有状态（与 DIB.Core.cs 的 `SharedImage` 访问器同一做法） ----

    /// <summary>DIB.pas 130 —— `FLUTDist[I, j]`（InitLight 建立的 256x256 光照距离 LUT）。</summary>
    public int FLUTDistValue(int I, int j) => FLUTDist[I, j];

    /// <summary>DIB.pas 131 —— LG_COUNT。</summary>
    public int LightCount => LG_COUNT;

    /// <summary>DIB.pas 132 —— LG_DETAIL。</summary>
    public int LightDetail => LG_DETAIL;

    // =========================================================================================
    // DIB.pas 5938-5966 —— Darkness（"added effect for DIB"）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5949-5966 1:1（原文 `{standalone routine}`）。
    /// **只处理 24bpp**（其它位深直接 Exit）。
    /// 原文的 `R/G/B` 局部变量其实取的是内存序 B/G/R（`p0[X*3]` 是 B），
    /// 故三条赋值虽然写着 R/G/B，实际是"同一公式分别作用在三个字节上"—— 原文如此（DIB.pas:5958-5963）。
    /// 公式 `Byte - (Byte * Amount) div 255`（**整数除**），再经 IntToByte 夹到 0..255。
    /// 这里的裸 `IntToByte(...)` 在原文解析为 **TDIB 的同名方法**（DIB.pas:159 声明 / 4910-4918 实现），
    /// 而非单元级函数（DIB.pas:5940-5945）；两者实现等价（DIB.cs:494 / DIB.Effects.cs:2818）。
    /// 调用点：`TDIB.DoDarkness`（DIB.pas:6364）、`TDIB.DoSpotLight`（DIB.pas:6925）。
    /// </summary>
    public unsafe void Darkness(int Amount)
    {
        if (this.BitCount != 24) return;
        for (int Y = 0; Y <= this.Height - 1; Y++)
        {
            byte* p0 = (byte*)ScanLine(Y);
            for (int X = 0; X <= this.Width - 1; X++)
            {
                int R = p0[X * 3];
                int G = p0[X * 3 + 1];
                int B = p0[X * 3 + 2];
                p0[X * 3] = IntToByte(R - (R * Amount) / 255);
                p0[X * 3 + 1] = IntToByte(G - (G * Amount) / 255);
                p0[X * 3 + 2] = IntToByte(B - (B * Amount) / 255);
            }
        }
    }

    // =========================================================================================
    // DIB.pas 5975-6036 —— DoSmoothRotate
    // =========================================================================================

    /// <summary>DIB.pas 5978 —— 原文 `DoSmoothRotate` 内的**局部**类型 `TFColor = record B, G, R: Byte`。</summary>
    private struct DibFColor
    {
        public byte B;
        public byte G;
        public byte R;
    }

    /// <summary>
    /// DIB.pas 5975-6036 1:1（双线性采样的平滑旋转）。
    /// 原文冗余（照抄 + 注释）：
    ///   * 5986 `Angle := Angle;` —— **自赋值**，无效果；
    ///   * 5980 声明的 `Left` / `Right` / `wx` / `wy` **从未被使用**（托管侧不声明，避免 CS0168 噪声）。
    /// 字段名与内存序相反但自洽：`nw.R := p1[ifx*3]` 取的是**内存第 0 字节（B 通道）**，
    /// 而插值写回时 `p3[X*3] := …nw.R…` 也落在第 0 字节 —— 原文如此（DIB.pas:6009-6032）。
    /// 采样点越界（`ifx`/`ify` 不在 `[0, Src.W/H)`）时**整像素不写**（不是填零）。
    /// `fx`/`fy` 原文是 Extended（80 位），托管侧用 double（本工程既有约定）；
    /// `Round` 走 DibFusionSupport.Round（银行家舍入）。
    /// </summary>
    public unsafe void DoSmoothRotate(TDIB Src, int cx, int cy, double Angle)
    {
        double Top, Bottom, eww, nsw, fx, fy;
        double cAngle, sAngle;
        int xDiff, yDiff, ifx, ify, px, py, ix, iy, X, Y;
        DibFColor nw, ne, sw, se;

        Angle = Angle;                       // 原文如此（DIB.pas:5986）—— 自赋值
        Angle = -Angle * Math.PI / 180;
        sAngle = Math.Sin(Angle);
        cAngle = Math.Cos(Angle);
        xDiff = (this.Width - Src.Width) / 2;
        yDiff = (this.Height - Src.Height) / 2;
        for (Y = 0; Y <= this.Height - 1; Y++)
        {
            byte* p3 = (byte*)ScanLine(Y);
            py = 2 * (Y - cy) + 1;
            for (X = 0; X <= this.Width - 1; X++)
            {
                px = 2 * (X - cx) + 1;
                fx = (((px * cAngle - py * sAngle) - 1) / 2 + cx) - xDiff;
                fy = (((px * sAngle + py * cAngle) - 1) / 2 + cy) - yDiff;
                ifx = DibFusionSupport.Round(fx);
                ify = DibFusionSupport.Round(fy);

                if (ifx > -1 && ifx < Src.Width && ify > -1 && ify < Src.Height)
                {
                    eww = fx - ifx;
                    nsw = fy - ify;
                    iy = TrimInt(ify + 1, 0, Src.Height - 1);
                    ix = TrimInt(ifx + 1, 0, Src.Width - 1);
                    byte* p1 = (byte*)Src.ScanLine(ify);
                    byte* P2 = (byte*)Src.ScanLine(iy);
                    nw.R = p1[ifx * 3];
                    nw.G = p1[ifx * 3 + 1];
                    nw.B = p1[ifx * 3 + 2];
                    ne.R = p1[ix * 3];
                    ne.G = p1[ix * 3 + 1];
                    ne.B = p1[ix * 3 + 2];
                    sw.R = P2[ifx * 3];
                    sw.G = P2[ifx * 3 + 1];
                    sw.B = P2[ifx * 3 + 2];
                    se.R = P2[ix * 3];
                    se.G = P2[ix * 3 + 1];
                    se.B = P2[ix * 3 + 2];

                    Top = nw.B + eww * (ne.B - nw.B);
                    Bottom = sw.B + eww * (se.B - sw.B);
                    p3[X * 3 + 2] = IntToByte(DibFusionSupport.Round(Top + nsw * (Bottom - Top)));

                    Top = nw.G + eww * (ne.G - nw.G);
                    Bottom = sw.G + eww * (se.G - sw.G);
                    p3[X * 3 + 1] = IntToByte(DibFusionSupport.Round(Top + nsw * (Bottom - Top)));

                    Top = nw.R + eww * (ne.R - nw.R);
                    Bottom = sw.R + eww * (se.R - sw.R);
                    p3[X * 3] = IntToByte(DibFusionSupport.Round(Top + nsw * (Bottom - Top)));
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 6042-6063 —— DoInvert
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6042-6063 1:1。
    /// `PicInvert` 先取 `w`/`h` **再**执行 `Src.BitCount := 24`（SetBitCount → 空图时
    /// SetSize(max(W,1),max(H,1),24)，非空时 ConvertBitCount(24)）—— 于是循环边界用的是**旧**尺寸。
    /// 逐字节 `not`（Byte 语义 = 255 - v）。
    /// </summary>
    public unsafe void DoInvert()
    {
        void PicInvert(TDIB Src)
        {
            int w = Src.Width;
            int h = Src.Height;
            Src.SetBitCount(24);
            for (int Y = 0; Y <= h - 1; Y++)
            {
                byte* P = (byte*)Src.ScanLine(Y);
                for (int X = 0; X <= w - 1; X++)
                {
                    P[X * 3] = unchecked((byte)~P[X * 3]);
                    P[X * 3 + 1] = unchecked((byte)~P[X * 3 + 1]);
                    P[X * 3 + 2] = unchecked((byte)~P[X * 3 + 2]);
                }
            }
        }

        PicInvert(this);
    }

    // =========================================================================================
    // DIB.pas 6065-6093 —— DoAddColorNoise
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6065-6093 1:1（三通道**各自独立**取一次随机噪声）。
    /// `Amount shr 1` 走逻辑右移（§24.3 陷阱 5）。`Random(Amount)` 是 Delphi System.Random 的
    /// 全局 RandSeed LCG —— 本工程落在 DibEffectsSupport.DibRandom（与 DIB.Effects.cs 的
    /// Spray/AddMonoNoise 同一实现，保证跨方法共享同一 RandSeed 序列）。
    /// </summary>
    public unsafe void DoAddColorNoise(int Amount)
    {
        void AddColorNoise(TDIB clip, int Amount2)
        {
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int R = p0[X * 3] + (DibEffectsSupport.DibRandom(Amount2) - DibFusionSupport.Shr(Amount2, 1));
                    int G = p0[X * 3 + 1] + (DibEffectsSupport.DibRandom(Amount2) - DibFusionSupport.Shr(Amount2, 1));
                    int B = p0[X * 3 + 2] + (DibEffectsSupport.DibRandom(Amount2) - DibFusionSupport.Shr(Amount2, 1));
                    p0[X * 3] = IntToByte(R);
                    p0[X * 3 + 1] = IntToByte(G);
                    p0[X * 3 + 2] = IntToByte(B);
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        AddColorNoise(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6095-6124 —— DoAddMonoNoise
    // =========================================================================================

    /// <summary>DIB.pas 6095-6124 1:1（三个通道共用**同一个**随机噪声 `a`，与 DoAddColorNoise 相对照）。</summary>
    public unsafe void DoAddMonoNoise(int Amount)
    {
        void _AddMonoNoise(TDIB clip, int Amount2)
        {
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int a = DibEffectsSupport.DibRandom(Amount2) - DibFusionSupport.Shr(Amount2, 1);
                    int R = p0[X * 3] + a;
                    int G = p0[X * 3 + 1] + a;
                    int B = p0[X * 3 + 2] + a;
                    p0[X * 3] = IntToByte(R);
                    p0[X * 3 + 1] = IntToByte(G);
                    p0[X * 3 + 2] = IntToByte(B);
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        _AddMonoNoise(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6126-6155 —— DoAntiAlias
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6126-6155 1:1（3x3 十字均值 `div 4`）。
    /// 原文冗余/怪癖（照抄 + 注释）：
    ///   * 6132-6133 的 `Memo` 交换：入口恒定传 `(0, 0, Width, Height)`，故交换永不触发；
    ///   * 6134-6137 的夹取把范围收成 `X ∈ [1, Width-2]`、`Y ∈ [1, Height-2]`；
    ///   * `clip.BitCount := 24` 在夹取**之后**执行（会重排/换缓冲区）；
    ///   * 就地写 `p1[X*3]` 而 `p1[(X-1)*3]` 是**同一行上一像素**，X 递增 ⇒ 有顺序依赖，必须照抄顺序。
    /// 原文注释里的 `(* Inversion des valeurs *)` / `(* si diff俽ence n俫ative*)` 是 OEM 乱码，保留原样。
    /// </summary>
    public unsafe void DoAntiAlias()
    {
        void AntiAliasRect(TDIB clip, int XOrigin, int YOrigin, int XFinal, int YFinal)
        {
            int Memo;
            if (XFinal < XOrigin) { Memo = XOrigin; XOrigin = XFinal; XFinal = Memo; } // (* Inversion des valeurs   *)
            if (YFinal < YOrigin) { Memo = YOrigin; YOrigin = YFinal; YFinal = Memo; } // (* si diff俽ence n俫ative*)
            XOrigin = Math.Max(1, XOrigin);
            YOrigin = Math.Max(1, YOrigin);
            XFinal = Math.Min(clip.Width - 2, XFinal);
            YFinal = Math.Min(clip.Height - 2, YFinal);
            clip.SetBitCount(24);
            for (int Y = YOrigin; Y <= YFinal; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y - 1);
                byte* p1 = (byte*)clip.ScanLine(Y);
                byte* P2 = (byte*)clip.ScanLine(Y + 1);
                for (int X = XOrigin; X <= XFinal; X++)
                {
                    p1[X * 3] = unchecked((byte)((p0[X * 3] + P2[X * 3] + p1[(X - 1) * 3] + p1[(X + 1) * 3]) / 4));
                    p1[X * 3 + 1] = unchecked((byte)((p0[X * 3 + 1] + P2[X * 3 + 1] + p1[(X - 1) * 3 + 1] + p1[(X + 1) * 3 + 1]) / 4));
                    p1[X * 3 + 2] = unchecked((byte)((p0[X * 3 + 2] + P2[X * 3 + 2] + p1[(X - 1) * 3 + 2] + p1[(X + 1) * 3 + 2]) / 4));
                }
            }
        }

        void AntiAlias(TDIB clip)
        {
            AntiAliasRect(clip, 0, 0, clip.Width, clip.Height);
        }

        AntiAlias(this);
    }

    // =========================================================================================
    // DIB.pas 6157-6191 —— DoContrast
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6157-6191 1:1：以 127 为中心按 `(Abs(127-ch) * Amount) div 255` 拉开/压缩对比。
    /// 原文的 `R/G/B` 局部名取的仍是内存序 B/G/R（同式作用，结果不受影响）—— 原文如此（DIB.pas:6168-6170）。
    /// `> 127` 与 `else`（即 `&lt;= 127`）两分支方向相反 —— 注意 127 本身走**减**分支。
    /// </summary>
    public unsafe void DoContrast(int Amount)
    {
        void _Contrast(TDIB clip, int Amount2)
        {
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int R = p0[X * 3];
                    int G = p0[X * 3 + 1];
                    int B = p0[X * 3 + 2];
                    int rg = (Math.Abs(127 - R) * Amount2) / 255;
                    int gg = (Math.Abs(127 - G) * Amount2) / 255;
                    int bg = (Math.Abs(127 - B) * Amount2) / 255;
                    if (R > 127) R = R + rg; else R = R - rg;
                    if (G > 127) G = G + gg; else G = G - gg;
                    if (B > 127) B = B + bg; else B = B - bg;
                    p0[X * 3] = IntToByte(R);
                    p0[X * 3 + 1] = IntToByte(G);
                    p0[X * 3 + 2] = IntToByte(B);
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        _Contrast(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6193-6302 —— DoFishEye
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6193-6302 1:1。
    /// `Single` 声明照抄为 `float`（原文的表达式在 FPU 里按 Extended 求值，托管侧按 double/float；
    /// 差异只在末位，故本方法的测试用结构性断言而非逐位相等）。
    /// 原文缺陷/怪癖（照抄 + 注释）：
    ///   * 6226 `rmax / 2 * (1 / (1 - r1 / rmax) - 1)`：`r1 = rmax` 时 `1/0` 得 +Inf（Single 除法
    ///     不抛），随后 `Trunc(±Inf)` 在 Delphi 抛 EInvalidOp —— 托管侧由 DibFusionSupport.Trunc
    ///     显式抛 ArithmeticException（§25.2，不静默）；
    ///   * `Amount = 0` 时 `rmax = 0` ⇒ `r1/rmax = +Inf` ⇒ `r2` 得 `0 * (-1) = 0`
    ///     ⇒ 全图采样到 `(xmid, ymid)` 一点（本片用差异断言锁死）；
    ///   * 6265/6273 等回绕分支用 `Height - ify - iy` / `Width - ifx - ix`（**不是** `-1-…`），
    ///     下标可能为负 —— 原文如此；
    ///   * `slo[tx*3] := Round(total_red)` 直接截断到 Byte（**无** IntToByte 夹取）。
    /// </summary>
    public unsafe void DoFishEye(int Amount)
    {
        void _FishEye(TDIB bmp, TDIB Dst, double Amount2)
        {
            float xmid, ymid;
            float fx, fy;
            float r1, r2;
            int ifx, ify;
            float dx, dy;
            float rmax;
            int ty, tx;
            float[] weight_x = new float[2];
            float[] weight_y = new float[2];
            float weight;
            int new_red, new_green, new_blue;
            float total_red, total_green, total_blue;
            int ix, iy;

            xmid = (float)(bmp.Width / 2.0);
            ymid = (float)(bmp.Height / 2.0);
            rmax = (float)(Dst.Width * Amount2);

            for (ty = 0; ty <= Dst.Height - 1; ty++)
            {
                for (tx = 0; tx <= Dst.Width - 1; tx++)
                {
                    dx = tx - xmid;
                    dy = ty - ymid;
                    r1 = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (r1 == 0)
                    {
                        fx = xmid;
                        fy = ymid;
                    }
                    else
                    {
                        r2 = (float)((double)rmax / 2 * (1 / (1 - (double)r1 / rmax) - 1));
                        fx = (float)((double)dx * r2 / r1 + xmid);
                        fy = (float)((double)dy * r2 / r1 + ymid);
                    }
                    ify = DibFusionSupport.Trunc(fy);
                    ifx = DibFusionSupport.Trunc(fx);
                    // Calculate the weights.
                    if (fy >= 0)
                    {
                        weight_y[1] = fy - ify;
                        weight_y[0] = 1 - weight_y[1];
                    }
                    else
                    {
                        weight_y[0] = -(fy - ify);
                        weight_y[1] = 1 - weight_y[0];
                    }
                    if (fx >= 0)
                    {
                        weight_x[1] = fx - ifx;
                        weight_x[0] = 1 - weight_x[1];
                    }
                    else
                    {
                        weight_x[0] = -(fx - ifx);
                        weight_x[1] = 1 - weight_x[0];
                    }

                    if (ifx < 0)
                        ifx = bmp.Width - 1 - (-ifx % bmp.Width);
                    else if (ifx > bmp.Width - 1)
                        ifx = ifx % bmp.Width;
                    if (ify < 0)
                        ify = bmp.Height - 1 - (-ify % bmp.Height);
                    else if (ify > bmp.Height - 1)
                        ify = ify % bmp.Height;

                    total_red = 0.0f;
                    total_green = 0.0f;
                    total_blue = 0.0f;
                    for (ix = 0; ix <= 1; ix++)
                    {
                        for (iy = 0; iy <= 1; iy++)
                        {
                            byte* sli;
                            if (ify + iy < bmp.Height)
                                sli = (byte*)bmp.ScanLine(ify + iy);
                            else
                                sli = (byte*)bmp.ScanLine(bmp.Height - ify - iy);
                            if (ifx + ix < bmp.Width)
                            {
                                new_red = sli[(ifx + ix) * 3];
                                new_green = sli[(ifx + ix) * 3 + 1];
                                new_blue = sli[(ifx + ix) * 3 + 2];
                            }
                            else
                            {
                                new_red = sli[(bmp.Width - ifx - ix) * 3];
                                new_green = sli[(bmp.Width - ifx - ix) * 3 + 1];
                                new_blue = sli[(bmp.Width - ifx - ix) * 3 + 2];
                            }
                            weight = weight_x[ix] * weight_y[iy];
                            total_red = total_red + new_red * weight;
                            total_green = total_green + new_green * weight;
                            total_blue = total_blue + new_blue * weight;
                        }
                    }
                    byte* slo = (byte*)Dst.ScanLine(ty);
                    slo[tx * 3] = unchecked((byte)DibFusionSupport.Round(total_red));
                    slo[tx * 3 + 1] = unchecked((byte)DibFusionSupport.Round(total_green));
                    slo[tx * 3 + 2] = unchecked((byte)DibFusionSupport.Round(total_blue));
                }
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        _FishEye(BB1, BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6304-6328 —— DoGrayScale
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6304-6328 1:1：`Gray := Round(B*0.3 + G*0.59 + R*0.11)`，三通道同写该值。
    /// 写回是**直接截断**到 Byte（无 IntToByte 夹取）—— 原文如此（DIB.pas:6314-6316）。
    /// 权重按**内存序**（0=B、1=G、2=R）取，故 0.3 加在 B 上 —— 与常规亮度公式的
    /// "R 权重最大"相反，但三通道权重和仍为 1，结果不受影响。
    /// </summary>
    public unsafe void DoGrayScale()
    {
        void GrayScale(TDIB clip)
        {
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int Gray = DibFusionSupport.Round(
                        p0[X * 3] * 0.3 + p0[X * 3 + 1] * 0.59 + p0[X * 3 + 2] * 0.11);
                    p0[X * 3] = unchecked((byte)Gray);
                    p0[X * 3 + 1] = unchecked((byte)Gray);
                    p0[X * 3 + 2] = unchecked((byte)Gray);
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        GrayScale(BB);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6330-6356 —— DoLightness
    // =========================================================================================

    /// <summary>DIB.pas 6330-6356 1:1：`ch + ((255 - ch) * Amount) div 255`（向白靠拢）。</summary>
    public unsafe void DoLightness(int Amount)
    {
        void _Lightness(TDIB clip, int Amount2)
        {
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int R = p0[X * 3];
                    int G = p0[X * 3 + 1];
                    int B = p0[X * 3 + 2];
                    p0[X * 3] = IntToByte(R + ((255 - R) * Amount2) / 255);
                    p0[X * 3 + 1] = IntToByte(G + ((255 - G) * Amount2) / 255);
                    p0[X * 3 + 2] = IntToByte(B + ((255 - B) * Amount2) / 255);
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        _Lightness(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6358-6367 —— DoDarkness
    // =========================================================================================

    /// <summary>DIB.pas 6358-6367 1:1（转调 TDIB.Darkness，即 DIB.pas 5949-5966）。</summary>
    public void DoDarkness(int Amount)
    {
        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        BB.Darkness(Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6369-6396 —— DoSaturation
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6369-6396 1:1：`Gray := (R+G+B) div 3`，再 `Gray + ((ch - Gray) * Amount) div 255`。
    /// `(ch - Gray)` 可为负 ⇒ `* Amount) div 255` 是**向零截断**的整数除（C# 的 int `/` 同）——
    /// 注意与 `shr` 的区别：这里用的是 `div`，绝不能换成移位。
    /// </summary>
    public unsafe void DoSaturation(int Amount)
    {
        void _Saturation(TDIB clip, int Amount2)
        {
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int R = p0[X * 3];
                    int G = p0[X * 3 + 1];
                    int B = p0[X * 3 + 2];
                    int Gray = (R + G + B) / 3;
                    p0[X * 3] = IntToByte(Gray + (((R - Gray) * Amount2) / 255));
                    p0[X * 3 + 1] = IntToByte(Gray + (((G - Gray) * Amount2) / 255));
                    p0[X * 3 + 2] = IntToByte(Gray + (((B - Gray) * Amount2) / 255));
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        _Saturation(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6398-6445 —— DoSplitBlur
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6398-6445 1:1（原文头注 `{NOTE: For a gaussian blur is amount 3}`，照抄）。
    /// 四角采样均值 `shr 2`；源行/列的两侧"越界反折"写法是 `clip.Height - Y` / `clip.Width - X`
    /// （**不是** `Height-1-Y`）—— 原文如此（DIB.pas:6412/6424）。
    /// 后果：`Amount >= clip.Height` 时 Y=0 的 `ScanLine(clip.Height - 0)` 抛 SScanline；
    /// 列方向无范围检查（`p1[clip.Width*3]` 会越出行尾）。
    /// 又：`p1` 在 Y=0 时就是 `p0` 自身，而 `cx = X - Amount` 取的是**已改写过的**左邻（X 递增）——
    /// 原文如此，本片用逐像素表锁死。
    /// </summary>
    public unsafe void DoSplitBlur(int Amount)
    {
        void _SplitBlur(TDIB clip, int Amount2)
        {
            byte[] Buf = new byte[4 * 3]; // Buf: array[0..3, 0..2] of Byte（行主序）
            if (Amount2 == 0) return;
            for (int Y = 0; Y <= clip.Height - 1; Y++)
            {
                byte* p0 = (byte*)clip.ScanLine(Y);
                byte* p1 = (Y - Amount2 < 0) ? (byte*)clip.ScanLine(Y) : (byte*)clip.ScanLine(Y - Amount2);
                byte* P2 = (Y + Amount2 < clip.Height)
                    ? (byte*)clip.ScanLine(Y + Amount2)
                    : (byte*)clip.ScanLine(clip.Height - Y);

                for (int X = 0; X <= clip.Width - 1; X++)
                {
                    int cx = (X - Amount2 < 0) ? X : X - Amount2;
                    Buf[0] = p1[cx * 3];
                    Buf[1] = p1[cx * 3 + 1];
                    Buf[2] = p1[cx * 3 + 2];
                    Buf[3] = P2[cx * 3];
                    Buf[4] = P2[cx * 3 + 1];
                    Buf[5] = P2[cx * 3 + 2];
                    cx = (X + Amount2 < clip.Width) ? X + Amount2 : clip.Width - X;
                    Buf[6] = p1[cx * 3];
                    Buf[7] = p1[cx * 3 + 1];
                    Buf[8] = p1[cx * 3 + 2];
                    Buf[9] = P2[cx * 3];
                    Buf[10] = P2[cx * 3 + 1];
                    Buf[11] = P2[cx * 3 + 2];
                    p0[X * 3] = unchecked((byte)((Buf[0] + Buf[3] + Buf[6] + Buf[9]) >> 2));
                    p0[X * 3 + 1] = unchecked((byte)((Buf[1] + Buf[4] + Buf[7] + Buf[10]) >> 2));
                    p0[X * 3 + 2] = unchecked((byte)((Buf[2] + Buf[5] + Buf[8] + Buf[11]) >> 2));
                }
            }
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        _SplitBlur(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6447-6457 —— DoGaussianBlur
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6447-6457 1:1。
    /// **原文冗余照抄**：6451-6452 连写两次 `BB.BitCount := 24;`（第二行无任何效果）。
    /// 转调 `TDIB.GaussianBlur`（已由 DIB.Effects.cs:4701-4707 落地）。
    /// </summary>
    public void DoGaussianBlur(int Amount)
    {
        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.SetBitCount(24);   // 原文如此（DIB.pas:6452）—— 连写两次赋值
        BB.Assign(this);
        GaussianBlur(BB, Amount);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6459-6502 —— DoMosaic
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6459-6502 1:1（四层嵌套 repeat/until，全部照抄为 do/while）。
    /// **真实语义（实测，与"马赛克"名字不完全相符）**：
    ///   * 6476-6478 的 `R/G/B` 读位于**最内层 `repeat` 之外** ⇒ 每个游程只读一次起点颜色，
    ///     然后向 `Size` 个连续像素刷同一个颜色 —— 被覆盖的位置**不会**先被读回
    ///     （横向游程填充，不是块内取平均）；
    ///   * 6473-6474 每轮 `j` 都重新 `P2 := Bm.ScanLine[Y]` **并 `X := 0`**，而 `p1` 只在
    ///     最外层 `Y` 循环头取一次 ⇒ 当 `Y` 已被内层推进后 `P2` 与 `p1` 是**不同行**，
    ///     于是整块 `Size` 行都被刷成同一个横向游程结果（纵向块扩散）；
    ///   * 内层 `until (Y >= Bm.Height) or (X >= Bm.Width)` 中 `X` 此时恒为 `Width-1` 或 `Width`，
    ///     该条件几乎总为假 —— 循环靠 `j` 与最外层 `Y` 退出。
    /// 实测（4x4、`pixel(x,y).R = x + 10y`、Size=2）：行 0 = [0,0,2,2]、行 1 = [0,0,2,2]、
    /// 行 2 = [20,20,22,22]、行 3 = [20,20,22,22]。
    /// `Size &lt;= 1` 时每个游程长度为 1 ⇒ 恒等。
    /// </summary>
    public unsafe void DoMosaic(int Size)
    {
        void Mosaic(TDIB Bm, int Size2)
        {
            int X, Y, I, j;
            Y = 0;
            do
            {
                byte* p1 = (byte*)Bm.ScanLine(Y);
                X = 0;
                do
                {
                    j = 1;
                    do
                    {
                        byte* P2 = (byte*)Bm.ScanLine(Y);
                        X = 0;
                        do
                        {
                            byte R = p1[X * 3];
                            byte G = p1[X * 3 + 1];
                            byte B = p1[X * 3 + 2];
                            I = 1;
                            do
                            {
                                P2[X * 3] = R;
                                P2[X * 3 + 1] = G;
                                P2[X * 3 + 2] = B;
                                X++;
                                I++;
                            } while (!(X >= Bm.Width || I > Size2));
                        } while (!(X >= Bm.Width));
                        j++;
                        Y++;
                    } while (!(Y >= Bm.Height || j > Size2));
                } while (!(Y >= Bm.Height || X >= Bm.Width));
            } while (!(Y >= Bm.Height));
        }

        var BB = new TDIB();
        BB.SetBitCount(24);
        BB.Assign(this);
        Mosaic(BB, Size);
        Assign(BB);
        BB.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6504-6642 —— DoTwist
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6504-6642 1:1（内嵌 `ArcTan2` 一并照抄）。
    /// 原文缺陷/怪癖（照抄 + 注释）：
    ///   * 6526-6538 `ArcTan2(xt, yt)` 的形参序与常规 `atan2(y, x)` **相反**；
    ///     且 `xt &lt; 0` 时无条件 `Pi + ArcTan(yt/xt)`（未按 `yt` 符号选 `±Pi`），
    ///     并且把 `ArcTan(yt / xt)` 算了两遍；
    ///   * 6564 `R / Amount`：`Amount = 0` 时得 ±Inf ⇒ `Cos(Inf) = NaN` ⇒ `Trunc(NaN)`
    ///     抛 EInvalidOp（托管侧由 DibFusionSupport.Trunc 显式抛 ArithmeticException）；
    ///   * 6606/6613 回绕用 `Height - ify - iy` / `Width - ifx - ix`，下标可为负（原文无检查）；
    ///   * 6624-6626 写回直接截断到 Byte（**无** IntToByte 夹取）。
    /// `Single` 声明照抄为 `float`（表达式按 double 求值，末位与原文 Extended 可能有别）。
    /// </summary>
    public unsafe void DoTwist(int Amount)
    {
        void _Twist(TDIB bmp, TDIB Dst, int Amount2)
        {
            float fxmid, fymid, txmid, tymid;
            float fx, fy, tx2, ty2, R, Theta;
            int ifx, ify;
            float dx, dy;
            float OFFSET;
            int ty, tx;
            float[] weight_x = new float[2];
            float[] weight_y = new float[2];
            float weight;
            int new_red, new_green, new_blue;
            float total_red, total_green, total_blue;
            int ix, iy;

            float ArcTan2(float xt, float yt)
            {
                float Result;
                if (xt == 0)
                    if (yt > 0)
                        Result = (float)(Math.PI / 2);
                    else
                        Result = (float)(-(Math.PI / 2));
                else
                {
                    Result = (float)Math.Atan(yt / xt);
                    if (xt < 0)
                        Result = (float)(Math.PI + Math.Atan(yt / xt));
                }
                return Result;
            }

            OFFSET = (float)(-(Math.PI / 2));
            dx = bmp.Width - 1;
            dy = bmp.Height - 1;
            R = (float)Math.Sqrt(dx * dx + dy * dy);
            tx2 = R;
            ty2 = R;
            txmid = (bmp.Width - 1) / 2f;   // Adjust these to move center of rotation
            tymid = (bmp.Height - 1) / 2f;  // Adjust these to move ......
            fxmid = (bmp.Width - 1) / 2f;
            fymid = (bmp.Height - 1) / 2f;
            if (tx2 >= bmp.Width) tx2 = bmp.Width - 1;
            if (ty2 >= bmp.Height) ty2 = bmp.Height - 1;

            for (ty = 0; ty <= DibFusionSupport.Round(ty2); ty++)
            {
                for (tx = 0; tx <= DibFusionSupport.Round(tx2); tx++)
                {
                    dx = tx - txmid;
                    dy = ty - tymid;
                    R = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (R == 0)
                    {
                        fx = 0;
                        fy = 0;
                    }
                    else
                    {
                        Theta = ArcTan2(dx, dy) - R / Amount2 - OFFSET;
                        fx = (float)(R * Math.Cos(Theta));
                        fy = (float)(R * Math.Sin(Theta));
                    }
                    fx = fx + fxmid;
                    fy = fy + fymid;

                    ify = DibFusionSupport.Trunc(fy);
                    ifx = DibFusionSupport.Trunc(fx);
                    // Calculate the weights.
                    if (fy >= 0)
                    {
                        weight_y[1] = fy - ify;
                        weight_y[0] = 1 - weight_y[1];
                    }
                    else
                    {
                        weight_y[0] = -(fy - ify);
                        weight_y[1] = 1 - weight_y[0];
                    }
                    if (fx >= 0)
                    {
                        weight_x[1] = fx - ifx;
                        weight_x[0] = 1 - weight_x[1];
                    }
                    else
                    {
                        weight_x[0] = -(fx - ifx);
                        weight_x[1] = 1 - weight_x[0];
                    }

                    if (ifx < 0)
                        ifx = bmp.Width - 1 - (-ifx % bmp.Width);
                    else if (ifx > bmp.Width - 1)
                        ifx = ifx % bmp.Width;
                    if (ify < 0)
                        ify = bmp.Height - 1 - (-ify % bmp.Height);
                    else if (ify > bmp.Height - 1)
                        ify = ify % bmp.Height;

                    total_red = 0.0f;
                    total_green = 0.0f;
                    total_blue = 0.0f;
                    for (ix = 0; ix <= 1; ix++)
                    {
                        for (iy = 0; iy <= 1; iy++)
                        {
                            byte* sli;
                            if (ify + iy < bmp.Height)
                                sli = (byte*)bmp.ScanLine(ify + iy);
                            else
                                sli = (byte*)bmp.ScanLine(bmp.Height - ify - iy);
                            if (ifx + ix < bmp.Width)
                            {
                                new_red = sli[(ifx + ix) * 3];
                                new_green = sli[(ifx + ix) * 3 + 1];
                                new_blue = sli[(ifx + ix) * 3 + 2];
                            }
                            else
                            {
                                new_red = sli[(bmp.Width - ifx - ix) * 3];
                                new_green = sli[(bmp.Width - ifx - ix) * 3 + 1];
                                new_blue = sli[(bmp.Width - ifx - ix) * 3 + 2];
                            }
                            weight = weight_x[ix] * weight_y[iy];
                            total_red = total_red + new_red * weight;
                            total_green = total_green + new_green * weight;
                            total_blue = total_blue + new_blue * weight;
                        }
                    }
                    byte* slo = (byte*)Dst.ScanLine(ty);
                    slo[tx * 3] = unchecked((byte)DibFusionSupport.Round(total_red));
                    slo[tx * 3 + 1] = unchecked((byte)DibFusionSupport.Round(total_green));
                    slo[tx * 3 + 2] = unchecked((byte)DibFusionSupport.Round(total_blue));
                }
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        _Twist(BB1, BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6644-6790 —— DoTrace
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6644-6790 1:1。
    /// 内嵌 `Trace` 先造一张 **8bpp** 的影子图 `Bitmap`（`Width/Height` 各赋值一次，
    /// 中途 `Bitmap.BitCount := 8`），再 `Src.BitCount := 24`，然后按 8bpp 的单字节比较
    /// 在 24bpp 的 `Src` 上"描边"。
    /// 接缝依赖（§2.3）：`Bitmap.Canvas.Draw(0, 0, Src)` 是 GDI BitBlt（会把 24bpp 转成 8bpp），
    /// 托管侧走 `DibSeams.Canvas.DrawGraphic` 之外的 `IDibCanvasSeam.Draw`；
    /// **未装载接缝时影子图恒为 0** ⇒ 全部比较为假 ⇒ `Src` 除位深外不变（本片断言该退化行为）。
    /// 原文笔误照抄：6686-6688 连写两次 `p3[(X+1)*3+1] := TraceB;`（第二次覆盖第一次，R 通道漏写）。
    /// </summary>
    public unsafe void DoTrace(int Amount)
    {
        // 原文 `tb` 声明在 Trace 体内；托管侧提到此处以便局部函数捕获（Trace 只被调用一次，语义等价）
        byte tb = 0;

        void Trace(TDIB Src, int intensity)
        {
            var Bitmap = new TDIB();
            Bitmap.SetWidth(Src.Width);
            Bitmap.SetHeight(Src.Height);
            Bitmap.Canvas.Draw(0, 0, Src);
            Bitmap.SetBitCount(8);
            Src.SetBitCount(24);
            bool hasb = false;
            byte TraceB = 0x00;
            for (int I = 1; I <= intensity; I++)
            {
                for (int Y = 0; Y <= Bitmap.Height - 2; Y++)
                {
                    byte* p1 = (byte*)Bitmap.ScanLine(Y);
                    byte* P2 = (byte*)Bitmap.ScanLine(Y + 1);
                    byte* p3 = (byte*)Src.ScanLine(Y);
                    byte* p4 = (byte*)Src.ScanLine(Y + 1);
                    int X = 0;
                    do
                    {
                        if (p1[X] != p1[X + 1])
                        {
                            if (!hasb)
                            {
                                tb = p1[X + 1];
                                hasb = true;
                                p3[X * 3] = TraceB;
                                p3[X * 3 + 1] = TraceB;
                                p3[X * 3 + 2] = TraceB;
                            }
                            else
                            {
                                if (p1[X] != tb)
                                {
                                    p3[X * 3] = TraceB;
                                    p3[X * 3 + 1] = TraceB;
                                    p3[X * 3 + 2] = TraceB;
                                }
                                else
                                {
                                    p3[(X + 1) * 3] = TraceB;
                                    p3[(X + 1) * 3 + 1] = TraceB;
                                    p3[(X + 1) * 3 + 1] = TraceB; // 原文如此（DIB.pas:6688）—— 应为 +2
                                }
                            }
                        }
                        if (p1[X] != P2[X])
                        {
                            if (!hasb)
                            {
                                tb = P2[X];
                                hasb = true;
                                p3[X * 3] = TraceB;
                                p3[X * 3 + 1] = TraceB;
                                p3[X * 3 + 2] = TraceB;
                            }
                            else
                            {
                                if (p1[X] != tb)
                                {
                                    p3[X * 3] = TraceB;
                                    p3[X * 3 + 1] = TraceB;
                                    p3[X * 3 + 2] = TraceB;
                                }
                                else
                                {
                                    p4[X * 3] = TraceB;
                                    p4[X * 3 + 1] = TraceB;
                                    p4[X * 3 + 2] = TraceB;
                                }
                            }
                        }
                        X++;
                    } while (!(X >= Bitmap.Width - 2));
                }
                if (I > 1)
                {
                    for (int Y = Bitmap.Height - 1; Y >= 1; Y--)
                    {
                        byte* p1 = (byte*)Bitmap.ScanLine(Y);
                        byte* P2 = (byte*)Bitmap.ScanLine(Y - 1);
                        byte* p3 = (byte*)Src.ScanLine(Y);
                        byte* p4 = (byte*)Src.ScanLine(Y - 1);
                        int X = Bitmap.Width - 1;
                        do
                        {
                            if (p1[X] != p1[X - 1])
                            {
                                if (!hasb)
                                {
                                    tb = p1[X - 1];
                                    hasb = true;
                                    p3[X * 3] = TraceB;
                                    p3[X * 3 + 1] = TraceB;
                                    p3[X * 3 + 2] = TraceB;
                                }
                                else
                                {
                                    if (p1[X] != tb)
                                    {
                                        p3[X * 3] = TraceB;
                                        p3[X * 3 + 1] = TraceB;
                                        p3[X * 3 + 2] = TraceB;
                                    }
                                    else
                                    {
                                        p3[(X - 1) * 3] = TraceB;
                                        p3[(X - 1) * 3 + 1] = TraceB;
                                        p3[(X - 1) * 3 + 2] = TraceB;
                                    }
                                }
                            }
                            if (p1[X] != P2[X])
                            {
                                if (!hasb)
                                {
                                    tb = P2[X];
                                    hasb = true;
                                    p3[X * 3] = TraceB;
                                    p3[X * 3 + 1] = TraceB;
                                    p3[X * 3 + 2] = TraceB;
                                }
                                else
                                {
                                    if (p1[X] != tb)
                                    {
                                        p3[X * 3] = TraceB;
                                        p3[X * 3 + 1] = TraceB;
                                        p3[X * 3 + 2] = TraceB;
                                    }
                                    else
                                    {
                                        p4[X * 3] = TraceB;
                                        p4[X * 3 + 1] = TraceB;
                                        p4[X * 3 + 2] = TraceB;
                                    }
                                }
                            }
                            X--;
                        } while (!(X <= 1));
                    }
                }
            }
            Bitmap.Destroy();
        }

        var BB1 = new TDIB(); // 原文 `tb` 已在方法头声明（供局部函数捕获）
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        Trace(BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6792-6825 —— DoSplitlight
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6792-6825 1:1（对三通道反复施加 `sin(a/255*Pi/2)*255` 的提亮曲线）。
    /// 原文怪癖（照抄 + 注释）：
    ///   * 6798-6801 `sinpixs` 的结果写成 `variant(...)` —— Variant 转 Integer 走 `Round`
    ///     （银行家舍入），本片用 DibFusionSupport.Round；
    ///   * 6813/6818-6820/6824 的 `BB2` **整段被注释掉**（变量声明里的 `{,BB2}` 也是注释），
    ///     托管侧照抄为注释，不留死代码。
    /// 重复 `Amount` 次 ⇒ 曲线迭代复合。
    /// </summary>
    public unsafe void DoSplitlight(int Amount)
    {
        void Splitlight(TDIB clip, int Amount2)
        {
            int sinpixs(int a) => DibFusionSupport.Round(Math.Sin(a / 255.0 * Math.PI / 2) * 255);

            for (int I = 1; I <= Amount2; I++)
            {
                for (int Y = 0; Y <= clip.Height - 1; Y++)
                {
                    byte* p1 = (byte*)clip.ScanLine(Y);
                    for (int X = 0; X <= clip.Width - 1; X++)
                    {
                        p1[X * 3] = unchecked((byte)sinpixs(p1[X * 3]));
                        p1[X * 3 + 1] = unchecked((byte)sinpixs(p1[X * 3 + 1]));
                        p1[X * 3 + 2] = unchecked((byte)sinpixs(p1[X * 3 + 2]));
                    }
                }
            }
        }

        var BB1 = new TDIB(); // 原文 `var BB1 {,BB2}: TDIB;`（DIB.pas:6813）
        BB1.SetBitCount(24);
        BB1.Assign(this);
        // BB2 := TDIB.Create;     原文如此（DIB.pas:6818）—— 整段被注释掉
        // BB2.BitCount := 24;     原文如此（DIB.pas:6819）
        // BB2.Assign (BB1);       原文如此（DIB.pas:6820）
        Splitlight(BB1, Amount);
        Assign(BB1);
        BB1.Destroy();
        // BB2.Free;               原文如此（DIB.pas:6824）
    }

    // =========================================================================================
    // DIB.pas 6827-6913 —— DoTile（含 SmoothResize / Tile 两个内嵌过程）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6827-6913 1:1。
    /// `Tile` 只做三件事然后交给接缝：`Dst.Width/Height := Src 的`、`Dst.Canvas.Draw(0,0,Src)`、
    /// 以及早退条件 `(Amount &lt;= 0) or ((w div Amount) &lt; 5) or ((h div Amount) &lt; 5)`；
    /// 真正的平铺是 `for j/I` 里 `Dst.Canvas.Draw(I*w2, j*h2, Bm)`（GDI → 接缝，无头时不产生像素变化）。
    /// `SmoothResize` 是**纯逻辑**（16.15 定点双线性），本片逐像素实测。
    /// 原文笔误照抄：
    ///   * 6844 `yP shr 16 &lt; Src.Height - 1` 用的是 **shr 16**（同一函数其它处都是 shr 15）；
    ///   * 6871 **R 通道**那一行把 `Read2[(t+1)*3]` 当成源（其余两通道用 `Read[(t+1)*3+ch]`）
    ///     —— 导致 R 通道的"右上"取样取自下一行；
    ///   * 6864/6867/6870 写回直接截断到 Byte。
    /// </summary>
    public void DoTile(int Amount)
    {
        void SmoothResize(TDIB Src, TDIB Dst)
        {
            int xP2, yP2;
            xP2 = DibFusionSupport.Shl(Src.Width - 1, 15) / Dst.Width;
            yP2 = DibFusionSupport.Shl(Src.Height - 1, 15) / Dst.Height;
            int yP = 0;
            for (int Y = 0; Y <= Dst.Height - 1; Y++)
            {
                int xP = 0;
                unsafe
                {
                    byte* Read = (byte*)Src.ScanLine(yP >> 15);
                    byte* Read2 = ((yP >> 16) < Src.Height - 1)
                        ? (byte*)Src.ScanLine((yP >> 15) + 1)
                        : (byte*)Src.ScanLine(yP >> 15);
                    byte* pc = (byte*)Dst.ScanLine(Y);
                    int z2 = yP & 0x7FFF;
                    int iz2 = 0x8000 - z2;
                    for (int X = 0; X <= Dst.Width - 1; X++)
                    {
                        int t = xP >> 15;
                        byte Col1r = Read[t * 3];
                        byte col1g = Read[t * 3 + 1];
                        byte col1b = Read[t * 3 + 2];
                        byte Col2r = Read2[t * 3];
                        byte col2g = Read2[t * 3 + 1];
                        byte col2b = Read2[t * 3 + 2];
                        int z = xP & 0x7FFF;
                        int w2 = (z * iz2) >> 15;
                        int w1 = iz2 - w2;
                        int w4 = (z * z2) >> 15;
                        int w3 = z2 - w4;
                        pc[X * 3 + 2] = unchecked((byte)((col1b * w1 + Read[(t + 1) * 3 + 2] * w2 +
                            col2b * w3 + Read2[(t + 1) * 3 + 2] * w4) >> 15));
                        pc[X * 3 + 1] = unchecked((byte)((col1g * w1 + Read[(t + 1) * 3 + 1] * w2 +
                            col2g * w3 + Read2[(t + 1) * 3 + 1] * w4) >> 15));
                        // 原文如此（DIB.pas:6871）：R 通道这里写 Read2 而不是 Read
                        pc[X * 3] = unchecked((byte)((Col1r * w1 + Read2[(t + 1) * 3] * w2 +
                            Col2r * w3 + Read2[(t + 1) * 3] * w4) >> 15));
                        xP += xP2;
                    }
                }
                yP += yP2;
            }
        }

        void Tile(TDIB Src, TDIB Dst, int Amount2)
        {
            int w = Src.Width;
            int h = Src.Height;
            Dst.SetWidth(w);
            Dst.SetHeight(h);
            Dst.Canvas.Draw(0, 0, Src);
            if (Amount2 <= 0 || (w / Amount2) < 5 || (h / Amount2) < 5) return;
            int h2 = h / Amount2;
            int w2 = w / Amount2;
            var Bm = new TDIB();
            Bm.SetWidth(w2);
            Bm.SetHeight(h2);
            Bm.SetBitCount(24);
            SmoothResize(Src, Bm);
            for (int j = 0; j <= Amount2 - 1; j++)
                for (int I = 0; I <= Amount2 - 1; I++)
                    Dst.Canvas.Draw(I * w2, j * h2, Bm);
            Bm.Destroy();
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        Tile(BB1, BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6915-6958 —— DoSpotLight
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6915-6958 1:1。
    /// 流程：`z := 24bpp 的 Src 副本` → `z.DrawTo(Src, 0,0,W,H, 0,0)`（BitBlt 接缝）→
    /// `z.Darkness(Amount)`（纯逻辑）→ 造一张黑底白椭圆遮罩 `Bm` → `z.Canvas.Draw(0,0,Bm)`
    /// 配合 `CopyMode := cmSrcAnd` 只保留椭圆内的变暗结果。
    /// 接缝：`DibFusionCanvas.Seam`（BitBlt）+ `DibFusionSpot.Seam`（Brush/FillRect/Ellipse/
    /// Transparent/CopyMode）+ `DibSeams.Canvas`（`z.Canvas.Draw`）。前两者未装载 → 抛
    /// InvalidOperationException（§25.2）。**本方法的可见像素效果完全依赖这三个接缝。**
    /// 原文笔误照抄：6932/6934 `Bm.Canvas.Brush.Color := clBlack / clwhite`（clwhite 小写）。
    /// </summary>
    public void DoSpotLight(int Amount, TDxRect Spot)
    {
        void SpotLight(TDIB Src, int Amount2, TDxRect Spot2)
        {
            if (DibFusionSpot.Seam == null)
                throw new InvalidOperationException(
                    "TDIB.DoSpotLight: 绘制接缝（DibFusionSpot.Seam / IDibFusionSpotSeam）未装载 —— " +
                    "TCanvas/TBitmap 绘制面在托管侧无对应物（§25.2：接缝不得静默退化）");

            var z = new TDIB();
            try
            {
                z.SetSize(Src.Width, Src.Height, 24);
                z.DrawTo(Src, 0, 0, Src.Width, Src.Height, 0, 0);
                z.Darkness(Amount2);
                int w = z.Width;
                int h = z.Height;
                var Bm = new TDIB();
                try
                {
                    Bm.SetWidth(w);
                    Bm.SetHeight(h);
                    DibFusionSpot.Seam.SetBrushColor(DibFusionSupport.clBlack);
                    DibFusionSpot.Seam.FillRect(0, 0, w, h);
                    DibFusionSpot.Seam.SetBrushColor(DibFusionSupport.clWhite); // clwhite
                    DibFusionSpot.Seam.Ellipse(Spot2.Left, Spot2.Top, Spot2.Right, Spot2.Bottom);
                    DibFusionSpot.Seam.SetBitmapTransparent(Bm, true);
                    DibFusionSpot.Seam.SetCanvasCopyMode(z, DibFusionSupport.cmSrcAnd); // {as transparentcolor for white}
                    z.Canvas.Draw(0, 0, Bm);
                }
                finally
                {
                    Bm.Destroy();
                }
            }
            finally
            {
                z.Destroy();
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        SpotLight(BB2, Amount, Spot);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6960-6988 —— DoEmboss
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6960-6988 1:1：`p1[X*3+ch] := (p1[X*3+ch] + (P2[(X+3)*3+ch] xor $FF)) shr 1`
    /// （当前行与下一行相隔 3 像素叠加反相）。
    /// 行范围 `0..Height-2`、列范围 `0..Width-4`（`(X+3)*3+2` 恰好落在行内最后一字节）。
    /// `xor $FF` 在 Integer 上做（Byte 提升），写回直接截断（和 ≤ 510 ⇒ 结果 ≤ 255）。
    /// 同一行内先读后写、不跨迭代复用（P2 是下一行）—— 但 `p1` 与 `P2` 在
    /// `Y` 递增时角色互换（下一次迭代的 p1 已是上一轮的 P2 所在行的下一行）—— 原文如此。
    /// </summary>
    public unsafe void DoEmboss()
    {
        void Emboss(TDIB bmp)
        {
            for (int Y = 0; Y <= bmp.Height - 2; Y++)
            {
                byte* p1 = (byte*)bmp.ScanLine(Y);
                byte* P2 = (byte*)bmp.ScanLine(Y + 1);
                for (int X = 0; X <= bmp.Width - 4; X++)
                {
                    p1[X * 3] = unchecked((byte)((p1[X * 3] + (P2[(X + 3) * 3] ^ 0xFF)) >> 1));
                    p1[X * 3 + 1] = unchecked((byte)((p1[X * 3 + 1] + (P2[(X + 3) * 3 + 1] ^ 0xFF)) >> 1));
                    p1[X * 3 + 2] = unchecked((byte)((p1[X * 3 + 2] + (P2[(X + 3) * 3 + 2] ^ 0xFF)) >> 1));
                }
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        Emboss(BB2);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 6990-7031 —— DoSolorize（原文拼写如此）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 6990-7031 1:1：`C := (b0+b1+b2) div 3`，`C &gt; Amount` → 整像素反相，否则原样拷贝。
    /// 注意与 `TDIB.Negative`（DIB.Effects.cs:3165-3221，按 DWORD 粒度取反）**不是**同一实现。
    /// </summary>
    public unsafe void DoSolorize(int Amount)
    {
        void Solorize(TDIB Src, TDIB Dst, int Amount2)
        {
            int w = Src.Width;
            int h = Src.Height;
            Src.SetBitCount(24);
            Dst.SetBitCount(24);
            for (int Y = 0; Y <= h - 1; Y++)
            {
                byte* ps = (byte*)Src.ScanLine(Y);
                byte* pd = (byte*)Dst.ScanLine(Y);
                for (int X = 0; X <= w - 1; X++)
                {
                    int C = (ps[X * 3] + ps[X * 3 + 1] + ps[X * 3 + 2]) / 3;
                    if (C > Amount2)
                    {
                        pd[X * 3] = unchecked((byte)(255 - ps[X * 3]));
                        pd[X * 3 + 1] = unchecked((byte)(255 - ps[X * 3 + 1]));
                        pd[X * 3 + 2] = unchecked((byte)(255 - ps[X * 3 + 2]));
                    }
                    else
                    {
                        pd[X * 3] = ps[X * 3];
                        pd[X * 3 + 1] = ps[X * 3 + 1];
                        pd[X * 3 + 2] = ps[X * 3 + 2];
                    }
                }
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        Solorize(BB1, BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 7033-7065 —— DoPosterize
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7033-7065 1:1：`pd := Round(v / Amount) * Amount`，其中 `/` 是**浮点除法**。
    /// 两个原文陷阱（本片用差异断言锁死）：
    ///   * `Amount = 0` → `v / 0` 得 ±Inf（v=0 时得 NaN）⇒ `Round` 抛 EInvalidOp
    ///     （托管侧由 DibFusionSupport.Round 显式抛 ArithmeticException）；
    ///   * 量化结果**可能超过 255**（如 v=255、Amount=100 → Round(2.55)=3 → 300），
    ///     写回 Byte 时**回绕**成 44（原文没有夹取）。
    /// </summary>
    public unsafe void DoPosterize(int Amount)
    {
        void Posterize(TDIB Src, TDIB Dst, int Amount2)
        {
            int w = Src.Width;
            int h = Src.Height;
            Src.SetBitCount(24);
            Dst.SetBitCount(24);
            for (int Y = 0; Y <= h - 1; Y++)
            {
                byte* ps = (byte*)Src.ScanLine(Y);
                byte* pd = (byte*)Dst.ScanLine(Y);
                for (int X = 0; X <= w - 1; X++)
                {
                    pd[X * 3] = unchecked((byte)(DibFusionSupport.Round((double)ps[X * 3] / Amount2) * Amount2));
                    pd[X * 3 + 1] = unchecked((byte)(DibFusionSupport.Round((double)ps[X * 3 + 1] / Amount2) * Amount2));
                    pd[X * 3 + 2] = unchecked((byte)(DibFusionSupport.Round((double)ps[X * 3 + 2] / Amount2) * Amount2));
                }
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        Posterize(BB1, BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 7067-7115 —— DoBrightness
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7067-7115 1:1。
    /// 原文用 `pRGBArray = ^TRGBArray`（`TRGBArray = array[0..32767] of TRGBTriple`，
    /// `MaxPixelCount = 32768`），逐像素 `rgbtRed/Green/Blue`。
    /// 注意 Windows.TRGBTriple 的**字段序是 Blue,Green,Red**（内存偏移 0/1/2），
    /// 而原文按 Red→Green→Blue 的顺序赋值 —— 三条赋值互不依赖，结果不受影响（照抄该顺序）。
    /// 分支：`Value &gt; 0` 用 `Min(255, v+Value)`，否则（含 0 与负数）用 `Max(0, v+Value)`。
    /// </summary>
    public unsafe void DoBrightness(int Amount)
    {
        const int MaxPixelCount = 32768; // 原文 `MaxPixelCount = 32768`（DIB.pas:7070）

        void Brightness(TDIB Src, TDIB Dst, int level)
        {
            int Value = level;
            Src.SetBitCount(24);
            Dst.SetBitCount(24);
            for (int I = 0; I <= Src.Height - 1; I++)
            {
                byte* OrigRow = (byte*)Src.ScanLine(I);
                byte* DestRow = (byte*)Dst.ScanLine(I);
                for (int j = 0; j <= Src.Width - 1; j++)
                {
                    if (Value > 0)
                    {
                        DestRow[j * 3 + 2] = unchecked((byte)Math.Min(255, OrigRow[j * 3 + 2] + Value)); // rgbtRed
                        DestRow[j * 3 + 1] = unchecked((byte)Math.Min(255, OrigRow[j * 3 + 1] + Value)); // rgbtGreen
                        DestRow[j * 3] = unchecked((byte)Math.Min(255, OrigRow[j * 3] + Value));         // rgbtBlue
                    }
                    else
                    {
                        DestRow[j * 3 + 2] = unchecked((byte)Math.Max(0, OrigRow[j * 3 + 2] + Value));
                        DestRow[j * 3 + 1] = unchecked((byte)Math.Max(0, OrigRow[j * 3 + 1] + Value));
                        DestRow[j * 3] = unchecked((byte)Math.Max(0, OrigRow[j * 3] + Value));
                    }
                }
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetBitCount(24);
        BB2.Assign(BB1);
        Brightness(BB1, BB2, Amount);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 7676-7776 —— DoColorize
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7676-7776 1:1（`Colorize` + 内嵌 `InvertBitmap`）。
    /// 原文**没有**任何纯像素运算：整个函数是 20 余次 `TCanvas` 光栅操作
    /// （`Brush.Style/Color`、`FillRect`、`CopyMode` ∈ {cmSrcInvert, cmSrcPaint, cmSrcErase,
    /// cmPatPaint, cmDstInvert}、`CopyRect`、`Pixels[X,Y]`、`Brush.Bitmap.Assign`），
    /// 唯一"逻辑"是各操作的**顺序**与接收者。故收敛到 IDibFusionColorizeSeam，并把顺序照抄，
    /// 由测试逐条锁死（§2.3 不移植项 4）。
    /// 原文缺陷（照抄 + 注释）：
    ///   * 7697 `fColor := iBackColor; ;` —— **连写两个分号**，且 `fColor` 之后从未被读取；
    ///   * 7688 `fForeDither` 与 7689 `fBmpMade` **从未初始化**（栈上不定值），
    ///     且 `fBmpMade := True`（7764）之后从未被读取。托管侧 `fForeDither = false`
    ///     （有意偏离，已登记：Delphi 下为未定义值，本片取"抖色分支不生效"的实测常见取值）；
    ///   * 7713/7719 `InvertBitmap(Src)` 会**就地改写入参 Src**（它正是 BB1，与 Self 共享像素）。
    /// </summary>
    public void DoColorize(int ForeColor, int BackColor)
    {
        void InvertBitmap(TDIB bmp)
        {
            DibFusionColorize.Seam.SetCopyMode(bmp, DibFusionRop.cmDstInvert);
            DibFusionColorize.Seam.CopyRect(bmp,
                TDxRect.Rect(0, 0, bmp.Width, bmp.Height),
                bmp, TDxRect.Rect(0, 0, bmp.Width, bmp.Height));
        }

        void Colorize(TDIB Src, TDIB Dst, int iForeColor, int iBackColor)
        {
            if (DibFusionColorize.Seam == null)
                throw new InvalidOperationException(
                    "TDIB.DoColorize: 绘制接缝（DibFusionColorize.Seam / IDibFusionColorizeSeam）未装载 —— " +
                    "TCanvas/TBitmap 光栅操作在托管侧无对应物（§25.2：接缝不得静默退化）");

            // {--}   原文如此（DIB.pas:7696）
            int fColor = iBackColor; ; // 原文如此（DIB.pas:7697）—— 连写两个分号，且 fColor 从未被读取
            int fForeColor = iForeColor;
            bool fForeDither = false;  // 原文未初始化（DIB.pas:7688）—— 有意偏离，已登记
            bool fBmpMade = false;     // 原文未初始化（DIB.pas:7689）；7764 赋值后从未读取

            int w = Src.Width;
            int h = Src.Height;
            var lTempBitmap = new TDIB();
            lTempBitmap.SetSize(w, h, 24);
            var lTempBitmap2 = new TDIB();
            lTempBitmap2.SetSize(w, h, 24);
            TDxRect lCRect = TDxRect.Rect(0, 0, w, h);

            // with lTempBitmap.Canvas do begin
            DibFusionColorize.Seam.SetBrushStyleSolid(lTempBitmap);
            DibFusionColorize.Seam.SetBrushColor(lTempBitmap, iBackColor);
            DibFusionColorize.Seam.FillRect(lTempBitmap, lCRect);
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap, DibFusionRop.cmSrcInvert);
            DibFusionColorize.Seam.CopyRect(lTempBitmap, lCRect, Src, lCRect);
            InvertBitmap(Src);
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap, DibFusionRop.cmSrcPaint);
            DibFusionColorize.Seam.CopyRect(lTempBitmap, lCRect, Src, lCRect);
            InvertBitmap(lTempBitmap);
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap, DibFusionRop.cmSrcInvert);
            DibFusionColorize.Seam.CopyRect(lTempBitmap, lCRect, Src, lCRect);
            InvertBitmap(Src);
            // end

            // with lTempBitmap2.Canvas do begin
            DibFusionColorize.Seam.SetBrushStyleSolid(lTempBitmap2);
            DibFusionColorize.Seam.SetBrushColor(lTempBitmap2, DibFusionSupport.clBlack);
            DibFusionColorize.Seam.FillRect(lTempBitmap2, lCRect);
            TDIB lDitherBitmap = null;
            if (fForeDither)
            {
                InvertBitmap(Src);
                lDitherBitmap = new TDIB();
                lDitherBitmap.SetSize(8, 8, 24);
                // with lDitherBitmap.Canvas do
                for (int X = 0; X <= 7; X++)
                    for (int Y = 0; Y <= 7; Y++)
                        if ((X % 2 == 0 && Y % 2 > 0) || (X % 2 > 0 && Y % 2 == 0))
                            DibFusionColorize.Seam.SetPixels(lDitherBitmap, X, Y, fForeColor);
                        else
                            DibFusionColorize.Seam.SetPixels(lDitherBitmap, X, Y, iBackColor);
                DibFusionColorize.Seam.AssignBrushBitmap(lTempBitmap2, lDitherBitmap);
            }
            else
            {
                DibFusionColorize.Seam.SetBrushStyleSolid(lTempBitmap2);
                DibFusionColorize.Seam.SetBrushColor(lTempBitmap2, fForeColor);
            }
            if (!fForeDither)
                InvertBitmap(Src);
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap2, DibFusionRop.cmPatPaint);
            DibFusionColorize.Seam.CopyRect(lTempBitmap2, lCRect, Src, lCRect);
            if (fForeDither) lDitherBitmap.Destroy();
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap2, DibFusionRop.cmSrcInvert);
            DibFusionColorize.Seam.CopyRect(lTempBitmap2, lCRect, Src, lCRect);
            // end

            DibFusionColorize.Seam.SetCopyMode(lTempBitmap, DibFusionRop.cmSrcInvert);
            DibFusionColorize.Seam.CopyRect(lTempBitmap, lCRect, lTempBitmap2, lCRect);
            InvertBitmap(Src);
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap, DibFusionRop.cmSrcErase);
            DibFusionColorize.Seam.CopyRect(lTempBitmap, lCRect, Src, lCRect);
            InvertBitmap(Src);
            DibFusionColorize.Seam.SetCopyMode(lTempBitmap, DibFusionRop.cmSrcInvert);
            DibFusionColorize.Seam.CopyRect(lTempBitmap, lCRect, lTempBitmap2, lCRect);
            InvertBitmap(lTempBitmap);
            InvertBitmap(Src);
            Dst.Assign(lTempBitmap);
            lTempBitmap.Destroy();
            fBmpMade = true;
            _ = fColor; // 原文 fColor/fBmpMade 赋值后从未读取
            _ = fBmpMade;
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();   // 原文 7771-7772 未 SetSize
        Colorize(BB1, BB2, ForeColor, BackColor);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    // =========================================================================================
    // DIB.pas 7780-7813 —— FadeOut（内联 asm 逐字节 max）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7780-7813 1:1（`PUSH ESI/EDI` 的 32 位内联汇编逐字节循环）。
    /// 反汇编语义：`dst[i] = max(Step, src[i])`（`CMP AL,AH; JA @@2` 是**无符号**比较）。
    /// 计数 = `WidthBytes * Height`（`MOV EAX,H; IMUL EDX` 后取低 32 位）⇒ 覆盖**整块像素缓冲**。
    /// **原文陷阱**：源行取的是 `Self.ScanLine[DIB2.Height - 1]`（用 **DIB2** 的高度索引 Self 的行），
    /// 且写向 `DIB2.ScanLine[DIB2.Height - 1]` ⇒ 只有当两张图同尺寸时二者才都等于各自的 FPBits；
    /// `DIB2.Height > Self.Height` 时先抛 SScanline。原文如此（DIB.pas:7785-7786）。
    /// </summary>
    public unsafe void FadeOut(TDIB DIB2, byte Step)
    {
        byte* p1 = (byte*)ScanLine(DIB2.Height - 1);
        byte* P2 = (byte*)DIB2.ScanLine(DIB2.Height - 1);
        int w = WidthBytes;
        int h = Height;
        int count = unchecked(w * h);
        for (int i = 0; i < count; i++)
        {
            byte ah = p1[i];
            P2[i] = Step > ah ? Step : ah;   // dst = max(Step, src)
        }
    }

    // =========================================================================================
    // DIB.pas 7815-7852 —— DoZoom
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7815-7852 1:1（**逐字节**缩放，不是逐像素）。
    /// 原文陷阱（照抄 + 注释）：
    ///   * 外层用**像素**宽度 `Width` 做 `for X := 1 to Width - 1`，而 `P2[X]`/`p1[Trunc(xr)]`
    ///     是**字节**下标 ⇒ 列方向只缩放了每行前 `Width` 个字节；
    ///   * `xstep/ystep := ZoomRatio` 是**加性**步进（不是乘性采样）；
    ///   * 边界判定 `(xr >= 0) and (xr <= w)` / `(yr >= 0) and (yr <= h)` 允许取到 **w / h**
    ///     —— `xr = w` 时 `p1[w]` 越出行尾 1 字节（不抛）；`yr = h` 时 `ScanLine(h)` 抛 SScanline；
    ///   * `xr := xstart` 在 Y 循环**末尾**重置（每行重来），`yr` 累加；
    ///   * `ZoomRatio = 1.0` **不是**恒等：P2[X] ← p1[X-1] 且 P2[Y] ← p1[Y-1]（整体右下移 1）。
    /// </summary>
    public unsafe void DoZoom(TDIB DIB2, double ZoomRatio)
    {
        int w = WidthBytes;
        int h = Height;
        double xstart = (w - (w * ZoomRatio)) / 2;

        double xr = xstart;
        double yr = (h - (h * ZoomRatio)) / 2;
        double xstep = ZoomRatio;
        double ystep = ZoomRatio;

        for (int Y = 1; Y <= this.Height - 1; Y++)
        {
            byte* P2 = (byte*)DIB2.ScanLine(Y);
            if (yr >= 0 && yr <= h)
            {
                byte* p1 = (byte*)ScanLine(DibFusionSupport.Trunc(yr));
                for (int X = 1; X <= this.Width - 1; X++)
                {
                    if (xr >= 0 && xr <= w)
                    {
                        P2[X] = p1[DibFusionSupport.Trunc(xr)];
                    }
                    else
                    {
                        P2[X] = 0;
                    }
                    xr = xr + xstep;
                }
            }
            else
            {
                for (int X = 1; X <= this.Width - 1; X++)
                {
                    P2[X] = 0;
                }
            }
            xr = xstart;
            yr = yr + ystep;
        }
    }

    // =========================================================================================
    // DIB.pas 7854-7868 —— DoBlur
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7854-7868 1:1（十字 5 点均值 `div 5`，**逐字节**，读 Self 写 DIB2）。
    /// `w := WidthBytes`，故 `p1[X + w]` 是**内存上一行**（图像里的 Y-1 行，因 FNextLine 为负），
    /// `p1[X - w]` 是下一行（图像里的 Y+1 行）✓ 几何正确。
    /// 原文缺陷：`Y = Height - 1` 时 `p1[X - w]` 落在 `FPBits - WidthBytes` ⇒ **读出缓冲区之外**
    /// （原文无边界检查）。故本片只对 `Y &lt;= Height - 2` 的行做精确断言。
    /// 另：`p1[X + 1]` 在 `X = Width - 1` 时读到行尾填充字节（原文如此）。
    /// </summary>
    public unsafe void DoBlur(TDIB DIB2)
    {
        int w = WidthBytes;
        for (int Y = 1; Y <= this.Height - 1; Y++)
        {
            byte* p1 = (byte*)ScanLine(Y);
            byte* P2 = (byte*)DIB2.ScanLine(Y);
            for (int X = 1; X <= this.Width - 1; X++)
            {
                P2[X] = unchecked((byte)((p1[X] + p1[X - 1] + p1[X + 1] + p1[X + w] + p1[X - w]) / 5));
            }
        }
    }

    // =========================================================================================
    // DIB.pas 7870-7903 —— FadeIn（内联 asm 逐字节 min）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7870-7903 1:1。与 FadeOut 同构，只有跳转条件由 `JA` 变 `JB`
    /// ⇒ `dst[i] = min(Step, src[i])`（无符号）。
    /// </summary>
    public unsafe void FadeIn(TDIB DIB2, byte Step)
    {
        byte* p1 = (byte*)ScanLine(DIB2.Height - 1);
        byte* P2 = (byte*)DIB2.ScanLine(DIB2.Height - 1);
        int w = WidthBytes;
        int h = Height;
        int count = unchecked(w * h);
        for (int i = 0; i < count; i++)
        {
            byte ah = p1[i];
            P2[i] = Step < ah ? Step : ah;   // dst = min(Step, src)
        }
    }

    // =========================================================================================
    // DIB.pas 7905-7928 —— FillDIB8
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7905-7928 1:1（内联 asm 用 `Color` 填满 `WidthBytes * Height` 字节，
    /// 起点是 `Self.ScanLine[Height - 1]` = 缓冲区首地址）—— 与位深/调色板无关，纯字节填充。
    /// </summary>
    public unsafe void FillDIB8(byte Color)
    {
        byte* P = (byte*)ScanLine(this.Height - 1);
        int w = WidthBytes;
        int h = Height;
        int count = unchecked(w * h);
        for (int i = 0; i < count; i++)
            P[i] = Color;
    }

    // =========================================================================================
    // DIB.pas 5756-5786 —— FilterLine
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5756-5786 1:1：Bresenham 之外的最朴素直线——按 `I div j` 参数化取整，含两端点（`0..j`）。
    /// `j := Round(Sqrt(Sqr(Abs(X2-X1)) + Sqr(Abs(Y2-Y1))))`：Sqr 是**整数**乘法（溢出回绕），
    /// 再整体转 double 开方；`j &lt; 1` → Exit（零长度线不画）。
    /// 通道混合公式（注意 fmNormal 的 `r1 + (((256 - r1) * r2) shr 8)` 是"向 Color 靠拢"，
    /// 而 fmMix25 是 `(r1 + r2*3) shr 2` —— Color 权重 1/4，与名字相反）—— 原文如此（DIB.pas:5777-5782）。
    /// 读写都走 `Self.Pixels[x, y]`（GetPixel/SetPixel，带越界返回 0 语义）。
    /// </summary>
    public void FilterLine(int X1, int Y1, int X2, int Y2, int Color, TFilterMode FilterMode)
    {
        int j = DibFusionSupport.Round(Math.Sqrt(
            (double)unchecked(DibFusionSupport.Sqr(Math.Abs(X2 - X1)) +
                              DibFusionSupport.Sqr(Math.Abs(Y2 - Y1)))));
        if (j < 1) return;

        int r1 = DibFusionSupport.GetRValue(Color);
        int g1 = DibFusionSupport.GetGValue(Color);
        int B1 = DibFusionSupport.GetBValue(Color);

        for (int I = 0; I <= j; I++)
        {
            int px = X1 + ((X2 - X1) * I) / j;
            int py = Y1 + ((Y2 - Y1) * I) / j;
            int t = unchecked((int)this[px, py]);
            int r2 = DibFusionSupport.GetRValue(t);
            int g2 = DibFusionSupport.GetGValue(t);
            int B2 = DibFusionSupport.GetBValue(t);
            switch (FilterMode)
            {
                case TFilterMode.fmNormal:
                    t = DibFusionSupport.Rgb(
                        unchecked((byte)(r1 + (((256 - r1) * r2) >> 8))),
                        unchecked((byte)(g1 + (((256 - g1) * g2) >> 8))),
                        unchecked((byte)(B1 + (((256 - B1) * B2) >> 8))));
                    break;
                case TFilterMode.fmMix25:
                    t = DibFusionSupport.Rgb(
                        unchecked((byte)((r1 + r2 * 3) >> 2)),
                        unchecked((byte)((g1 + g2 * 3) >> 2)),
                        unchecked((byte)((B1 + B2 * 3) >> 2)));
                    break;
                case TFilterMode.fmMix50:
                    t = DibFusionSupport.Rgb(
                        unchecked((byte)((r1 + r2) >> 1)),
                        unchecked((byte)((g1 + g2) >> 1)),
                        unchecked((byte)((B1 + B2) >> 1)));
                    break;
                case TFilterMode.fmMix75:
                    t = DibFusionSupport.Rgb(
                        unchecked((byte)((r1 * 3 + r2) >> 2)),
                        unchecked((byte)((g1 * 3 + g2) >> 2)),
                        unchecked((byte)((B1 * 3 + B2) >> 2)));
                    break;
            }
            this[px, py] = unchecked((uint)t);
        }
    }

    // =========================================================================================
    // DIB.pas 5788-5846 —— FilterRect
    // =========================================================================================

    /// <summary>
    /// DIB.pas 5788-5846 1:1：整块滤镜。四个模式都**原地**改写 `Self` 的 24bpp 像素：
    ///   fmNormal → 先用当前像素的 B/G/R 均值为灰度，再分别乘 B/G/R 分量（彩色化）；
    ///   fmMix25 → `(3*p + C) shr 2`（**Color 权重 1/4**，与名字相反 —— 原文如此）；
    ///   fmMix50 → `(p + C) shr 1`；fmMix75 → `(p + 3*C) shr 2`。
    /// 列循环 `0 .. Width-1` 而每像素推进 3 字节 —— 原文如此（`Width` 是**像素**数）。
    /// fmNormal 分支里 P2/p3 是 p1 的 +1/+2 别名，**在本像素被改写前**取值 —— 原文如此。
    /// </summary>
    public unsafe void FilterRect(int X, int Y, int Width, int Height, int Color, TFilterMode FilterMode)
    {
        int R = DibFusionSupport.GetRValue(Color);
        int G = DibFusionSupport.GetGValue(Color);
        int B = DibFusionSupport.GetBValue(Color);

        for (int I = 0; I <= Height - 1; I++)
        {
            byte* p1 = (byte*)ScanLine(I + Y);
            p1 += 3 * X;
            for (int j = 0; j <= Width - 1; j++)
            {
                switch (FilterMode)
                {
                    case TFilterMode.fmNormal:
                        {
                            byte* P2 = p1;
                            P2++;
                            byte* p3 = P2;
                            p3++;
                            int C1 = (p1[0] + P2[0] + p3[0]) / 3;

                            p1[0] = unchecked((byte)((C1 * B) >> 8));
                            p1++;
                            p1[0] = unchecked((byte)((C1 * G) >> 8));
                            p1++;
                            p1[0] = unchecked((byte)((C1 * R) >> 8));
                            p1++;
                        }
                        break;
                    case TFilterMode.fmMix25:
                        {
                            p1[0] = unchecked((byte)((3 * p1[0] + B) >> 2));
                            p1++;
                            p1[0] = unchecked((byte)((3 * p1[0] + G) >> 2));
                            p1++;
                            p1[0] = unchecked((byte)((3 * p1[0] + R) >> 2));
                            p1++;
                        }
                        break;
                    case TFilterMode.fmMix50:
                        {
                            p1[0] = unchecked((byte)((p1[0] + B) >> 1));
                            p1++;
                            p1[0] = unchecked((byte)((p1[0] + G) >> 1));
                            p1++;
                            p1[0] = unchecked((byte)((p1[0] + R) >> 1));
                            p1++;
                        }
                        break;
                    case TFilterMode.fmMix75:
                        {
                            p1[0] = unchecked((byte)((p1[0] + 3 * B) >> 2));
                            p1++;
                            p1[0] = unchecked((byte)((p1[0] + 3 * G) >> 2));
                            p1++;
                            p1[0] = unchecked((byte)((p1[0] + 3 * R) >> 2));
                            p1++;
                        }
                        break;
                }
            }
        }
    }
}

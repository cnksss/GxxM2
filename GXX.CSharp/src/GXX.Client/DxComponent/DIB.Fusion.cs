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
//   5917-5934  TDIB.DrawOn（DrawTo 的落点，故随切片 A 落地）
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

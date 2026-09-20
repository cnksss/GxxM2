using System;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas（Source\Client-HGE\DxComponent\DIB.pas）1:1 移植 —— 第 6 片（尾部）。
//
// 覆盖行号：
//   7930-8006  TDIB.DoRotate
//   8007-8299  TDIB.Ink
//   8300-8383  TDIB.Distort
//   8384-8474  TDIB.AntialiasedLine
//   8475-8537  TDIB.GetColorBetween
//   8538-8736  TDIB.ColoredLine
//   8738-8746  initialization / finalization —— **归属登记，不在本文件实现**：
//              `TPicture.RegisterClipboardFormat(CF_DIB, TDIB)` / `TPicture.RegisterFileFormat('dib', …)`
//              已由 DIB.cs 的 DibSeams.RegisterPictureFormats() 登记（DIB.pas:8739-8740）；
//              `TPicture.UnRegisterGraphicClass(TDIB)` / `FEmptyDIBImage.Free` / `FPaletteManager.Free`
//              已由 DibSeams.FinalizeUnit() → DIB.ResetEmptyDIBImage() / DIB.ResetPaletteManager()
//              登记（DIB.pas:8742-8745）。本文件按 §10 不写静态构造器重复调用。
//
// 原文局部类型（各方法内重复声明，DIB.pas 7931-7936 / 8008-8013 / 8301-8306）：
//   T3Byte      = array[0..2] of Byte;                  → 3 字节步长
//   T3ByteArray = array[0..32767] of T3Byte;
//   TLongArray  = array[0..32767] of Longint;           → **有符号** 32 位（与 PArrayDWord 的 DWord 不同！）
//   PLines      = ^TLines;  TLines = array[0..0] of TBGR（DIB.pas 26-27，packed B,G,R）
//   PArrayByte(P)[X]  → ((byte*)P)[X]
//   PArrayWord(P)[X]  → ((ushort*)P)[X]
//   PArrayDWord(P)[X] → ((uint*)P)[X]
//   P3ByteArray(P)[X] → (byte*)P + X*3（再 [0]/[1]/[2] = B/G/R）
//
// 原文笔误/冗余（全部保留原样 + 注释）：
//   * DoRotate/Distort 8/16/24/32bpp 拷贝分支 `P[X] := P2[Width - X2]`：
//     X2=0 时下标 = Width，越过行末 1 个元素 —— 原文如此（DIB.pas:7954 / 7965 / 7976 / 7994 / 8331 …）。
//   * Distort 32bpp 退场分支的条件写成 `if P[X] > 2`（P 是 PByteArray，取的是**行内第 X 个字节**），
//     而赋值用的是 PLongArray(P)[X] —— 原文如此（DIB.pas:8373-8376）。
//   * AntialiasedLine 水平分支 `Inc(Y)` 之后**没有**重新 `P := ScanLine[Y]`，
//     第二次混合仍写回同一行 —— 原文如此（DIB.pas:8432-8435）。
//   * GetColorBetween 内嵌 asm 里的 `push ebp` / `pop ecx` 是寄存器手写冗余 —— 原文如此（DIB.pas:8509-8525）。
//   * ColoredLine 的 `tempColor` 声明后未初始化，pgPoint+csSolid 读到的是未初始化值 —— 原文如此（DIB.pas:8541）。
//   * ColoredLine 的 ColorToRGBTriple / RGBTripleToColor 两个局部函数从未被调用 —— 原文如此（DIB.pas:8650-8662）。
//
// 不可移植项（§2.3）：TCanvas 的 Pen/Brush/Pixels/MoveTo/LineTo/Ellipse/Rectangle 是 GDI 绘制路径，
// 不翻译实现，统一收敛到 DibTailSupport.Canvas（IDibTailCanvasSeam）。
// 注意：为保持 1:1，Canvas 接缝调用**不**触碰 TDIB.Canvas 属性（原文经 property getter → GetCanvas →
// AllocHandle 的 GDI 语义在托管侧无对应物，见回报"接缝"一节）。
// =============================================================================================

/// <summary>
/// 原文 Windows.TPoint（DIB.pas 8538 `const iStart, iEnd: TPoint` / 8667-8668 `iStart.X`）。
/// 字段名 X / Y 照抄。
/// </summary>
public struct TDibPoint
{
    public int X;
    public int Y;

    public TDibPoint(int x, int y) { X = x; Y = y; }
}

/// <summary>
/// 原文 Windows.TRGBTriple（DIB.pas 8650-8662 的局部函数用；字段顺序 rgbtBlue,rgbtGreen,rgbtRed）。
/// 带 Dib 前缀以避免与其它分片/单元可能引入的 TRGBTriple 重名。
/// </summary>
internal struct DibTRGBTriple
{
    public byte rgbtBlue;
    public byte rgbtGreen;
    public byte rgbtRed;
}

/// <summary>
/// 接缝：TDIB 尾部各方法里的 TCanvas 绘制面（§2.3 不移植项 4）。
/// 原文调用点：AntialiasedLine 的 Canvas.Pen/MoveTo/LineTo（8384-8403）；
/// ColoredLine 的 Canvas.Pen/Brush/Pixels/Ellipse/Rectangle（8546-8723）。
/// 由客户端装载层实现（真实 TCanvas/GDI+）；未装载时所有调用为无操作。
/// </summary>
public interface IDibTailCanvasSeam
{
    /// <summary>TCanvas.Pen.Color := Color。</summary>
    void SetPenColor(int Color);

    /// <summary>TCanvas.Brush.Color := Color。</summary>
    void SetBrushColor(int Color);

    /// <summary>TCanvas.Pixels[X, Y] := Color。</summary>
    void SetPixel(int X, int Y, int Color);

    /// <summary>TCanvas.MoveTo(X, Y)。</summary>
    void MoveTo(int X, int Y);

    /// <summary>TCanvas.LineTo(X, Y)。</summary>
    void LineTo(int X, int Y);

    /// <summary>TCanvas.Ellipse(X1, Y1, X2, Y2)。</summary>
    void Ellipse(int X1, int Y1, int X2, int Y2);

    /// <summary>TCanvas.Rectangle(X1, Y1, X2, Y2)。</summary>
    void Rectangle(int X1, int Y1, int X2, int Y2);
}

/// <summary>
/// 尾部切片（DIB.pas 7930-8746）用到的单元级辅助：Windows 的 TColor 宏、Delphi 的 shl/shr 语义、Random。
/// 名字带 Dib 前缀，避免与其它分片冲突。
/// </summary>
public static class DibTailSupport
{
    /// <summary>TCanvas 接缝（未装载时为 null，所有绘制调用退化为无操作）。</summary>
    public static IDibTailCanvasSeam Canvas;

    /// <summary>Windows.GetRValue(Color) = Color and $FF。</summary>
    public static byte GetRValue(int Color) => unchecked((byte)(Color & 0xFF));

    /// <summary>Windows.GetGValue(Color) = (Color shr 8) and $FF。</summary>
    public static byte GetGValue(int Color) => unchecked((byte)((Color >> 8) & 0xFF));

    /// <summary>Windows.GetBValue(Color) = (Color shr 16) and $FF。</summary>
    public static byte GetBValue(int Color) => unchecked((byte)((Color >> 16) & 0xFF));

    /// <summary>Windows.RGB(R, G, B) = R or (G shl 8) or (B shl 16)。参数按 Byte 语义截断。</summary>
    public static int Rgb(byte R, byte G, byte B) => R | (G << 8) | (B << 16);

    /// <summary>
    /// Delphi 的 `shr`：对 32 位 Integer 是**逻辑**右移（高位补 0），
    /// 与 C# 对 int 的算术 `&gt;&gt;`（高位补符号位）不同。
    /// 原文用 shr 的地方（8432/8455/8460/8515 等）一律走这里。
    /// </summary>
    public static int Shr(int Value, int Count) => unchecked((int)((uint)Value >> Count));

    /// <summary>Delphi 的 Round（银行家舍入）。</summary>
    public static int Round(double Value) => (int)Math.Round(Value, MidpointRounding.ToEven);

    /// <summary>
    /// Delphi System.Random(Range) 的托管等价（0 &lt;= X &lt; Range；Range = 0 → 0）。
    /// 原文只有 `Random(Width - 1)` / `Random(Height - 1)` 两处调用（DIB.pas:8039）。
    /// 测试通过给 RandomFunc 赋确定序列来消除随机性。Delphi 对 Range &lt; 0 未定义，此处按 0 处理。
    /// </summary>
    public static Func<int, int> RandomFunc;

    private static readonly System.Random FRandom = new System.Random();

    /// <summary>Delphi System.Random(Range)。</summary>
    public static int Random(int Range)
    {
        if (RandomFunc != null) return RandomFunc(Range);
        if (Range <= 0) return 0;
        return (int)(FRandom.NextDouble() * Range);
    }
}

/// <summary>DIB.pas 110-349 的 TDIB —— 本文件是"尾部"分片（DIB.pas 7930-8746）。</summary>
public partial class TDIB
{
    // =========================================================================================
    // DIB.pas 7930-8006 —— DoRotate
    // 注意：写入的是参数 DIB1，读取的是 Self（ScanLine / Width / Height / BitCount 均指 Self）。
    // =========================================================================================

    /// <summary>
    /// DIB.pas 7930-8006 1:1。
    /// Angle := 384 + a；cosy/siny 为 Real（= double，赋值时由 Single 拓宽）；
    /// X2/Y2 用 Trunc（向零截断）。四路 case 的退场衰减：8/16/32bpp 减 4、24bpp 逐通道减 4。
    /// </summary>
    public unsafe void DoRotate(TDIB DIB1, int xc, int yc, int a)
    {
        int X, Y, X2, Y2, Angle;
        double cosy, siny;

        Angle = 384 + a;
        for (Y = 0; Y <= Height - 1; Y++)
        {
            byte* P = (byte*)DIB1.ScanLine(Y);
            cosy = (Y - yc) * DIB.DCos(Angle & 0x1FF);
            siny = (Y - yc) * DIB.DSin(Angle & 0x1FF);
            for (X = 0; X <= Width - 1; X++)
            {
                X2 = (int)((X - xc) * DIB.DSin(Angle & 0x1FF) + cosy) + xc;
                Y2 = (int)((X - xc) * DIB.DCos(Angle & 0x1FF) - siny) + yc;
                switch (BitCount)
                {
                    case 8:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                // 原文如此（DIB.pas:7954）：X2 = 0 时下标 = Width，越行末 1 字节
                                P[X] = P2[Width - X2];
                            }
                            else
                            {
                                if (P[X] > 4)
                                    P[X] = (byte)(P[X] - 4);
                                else
                                    P[X] = 0;
                            }
                        }
                        break;
                    case 16:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                ((ushort*)P)[X] = ((ushort*)P2)[Width - X2];
                            }
                            else
                            {
                                if (((ushort*)P)[X] > 4)
                                    ((ushort*)P)[X] = (ushort)(((ushort*)P)[X] - 4);
                                else
                                    ((ushort*)P)[X] = 0;
                            }
                        }
                        break;
                    case 24:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                byte* Src = P2 + (Width - X2) * 3;
                                byte* Dst = P + X * 3;
                                Dst[0] = Src[0];   // B
                                Dst[1] = Src[1];   // G
                                Dst[2] = Src[2];   // R
                            }
                            else
                            {
                                byte* q = P + X * 3;
                                if (q[0] > 4)
                                    q[0] = (byte)(q[0] - 4);
                                else if (q[1] > 4)
                                    q[1] = (byte)(q[1] - 4);
                                else if (q[2] > 4)
                                    q[2] = (byte)(q[2] - 4);
                                else
                                {
                                    q[0] = 0;
                                    q[1] = 0;
                                    q[2] = 0;
                                }
                            }
                        }
                        break;
                    case 32:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                ((uint*)P)[X] = ((uint*)P2)[Width - X2];
                            }
                            else
                            {
                                // PLongArray = array of Longint → **有符号** 32 位比较
                                if (((int*)P)[X] > 4)
                                    ((int*)P)[X] = ((int*)P)[X] - 4;
                                else
                                    ((int*)P)[X] = 0;
                            }
                        }
                        break;
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 8007-8299 —— Ink
    // 参数 `DIB` 与单元级静态类 DIB 同名（原文如此），方法体内 DIB 指参数。
    // `case BitCount of` 用的是 **Self** 的位深，像素访问用参数 DIB 的行指针 —— 原文如此。
    // =========================================================================================

    /// <summary>
    /// DIB.pas 8007-8299 1:1。
    /// SprayInit 时 DIB.Assign(Self) 再随机撒 AmountSpray+1 个黑点（`for C := 0 to AmountSpray` 含右端）；
    /// 主循环对每个"暗"像素向四邻衰减：8/16/32bpp 自身 -8、上下 -4、左右 -2；24bpp 逐通道同规则。
    /// 返回 True 表示"全黑"（16bpp 及更低位深用无符号比较，32bpp 用 **有符号** Longint 比较）。
    /// </summary>
    public unsafe bool Ink(TDIB DIB, bool SprayInit, int AmountSpray)
    {
        int X, Y, C, z;
        byte* p0 = null;
        byte* P2 = null;

        DibTRGBTriple ColorToRGBTriple(int Color)
        {
            // 原文局部函数（DIB.pas:8014-8022）在 Ink 中未被调用
            DibTRGBTriple Result = default;
            Result.rgbtRed = DibTailSupport.GetRValue(Color);
            Result.rgbtGreen = DibTailSupport.GetGValue(Color);
            Result.rgbtBlue = DibTailSupport.GetBValue(Color);
            return Result;
        }

        bool TestQuad(byte* t, int Color)
        {
            // 原文 TestQuad(t: T3Byte; Color: Integer)（DIB.pas:8024-8029）：
            // t 是 3 字节记录（B,G,R），按值传入；此处用指针读取，语义相同（TestQuad 不写 t）
            return (t[0] > DibTailSupport.GetRValue(Color)) &&
                   (t[1] > DibTailSupport.GetGValue(Color)) &&
                   (t[2] > DibTailSupport.GetBValue(Color));
        }

        if (SprayInit)
        {
            DIB.Assign(this);
            // { Spray seeds }
            for (C = 0; C <= AmountSpray; C++)
            {
                DIB[DibTailSupport.Random(Width - 1), DibTailSupport.Random(Height - 1)] = 0;
            }
        }
        bool Result = true; // {all is black}
        for (Y = 0; Y <= DIB.Height - 1; Y++)
        {
            byte* P = (byte*)DIB.ScanLine(Y);
            for (X = 0; X <= DIB.Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 8:
                        {
                            if (P[X] < 16)
                            {
                                if (P[X] > 0) Result = false;
                                if (Y > 0)
                                {
                                    p0 = (byte*)DIB.ScanLine(Y - 1);
                                    if (p0[X] > 4)
                                        p0[X] = (byte)(p0[X] - 4);
                                    else
                                        p0[X] = 0;
                                    if (X > 0)
                                    {
                                        if (p0[X - 1] > 2)
                                            p0[X - 1] = (byte)(p0[X - 1] - 2);
                                        else
                                            p0[X - 1] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (p0[X + 1] > 2)
                                            p0[X + 1] = (byte)(p0[X + 1] - 2);
                                        else
                                            p0[X + 1] = 0;
                                    }
                                }
                                if (Y < DIB.Height - 1)
                                {
                                    P2 = (byte*)DIB.ScanLine(Y + 1);
                                    if (P2[X] > 4)
                                        P2[X] = (byte)(P2[X] - 4);
                                    else
                                        P2[X] = 0;
                                    if (X > 0)
                                    {
                                        if (P2[X - 1] > 2)
                                            P2[X - 1] = (byte)(P2[X - 1] - 2);
                                        else
                                            P2[X - 1] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (P2[X + 1] > 2)
                                            P2[X + 1] = (byte)(P2[X + 1] - 2);
                                        else
                                            P2[X + 1] = 0;
                                    }
                                }
                                if (P[X] > 8)
                                    P[X] = (byte)(P[X] - 8);
                                else
                                    P[X] = 0;
                                if (X > 0)
                                {
                                    if (P[X - 1] > 4)
                                        P[X - 1] = (byte)(P[X - 1] - 4);
                                    else
                                        P[X - 1] = 0;
                                }
                                if (X < DIB.Width - 1)
                                {
                                    if (P[X + 1] > 4)
                                        P[X + 1] = (byte)(P[X + 1] - 4);
                                    else
                                        P[X + 1] = 0;
                                }
                            }
                        }
                        break;
                    case 16:
                        {
                            if (((ushort*)P)[X] < 16)
                            {
                                if (((ushort*)P)[X] > 0) Result = false;
                                if (Y > 0)
                                {
                                    p0 = (byte*)DIB.ScanLine(Y - 1);
                                    if (((ushort*)p0)[X] > 4)
                                        ((ushort*)p0)[X] = (ushort)(((ushort*)p0)[X] - 4);
                                    else
                                        ((ushort*)p0)[X] = 0;
                                    if (X > 0)
                                    {
                                        if (((ushort*)p0)[X - 1] > 2)
                                            ((ushort*)p0)[X - 1] = (ushort)(((ushort*)p0)[X - 1] - 2);
                                        else
                                            ((ushort*)p0)[X - 1] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (((ushort*)p0)[X + 1] > 2)
                                            ((ushort*)p0)[X + 1] = (ushort)(((ushort*)p0)[X + 1] - 2);
                                        else
                                            ((ushort*)p0)[X + 1] = 0;
                                    }
                                }
                                if (Y < DIB.Height - 1)
                                {
                                    P2 = (byte*)DIB.ScanLine(Y + 1);
                                    if (((ushort*)P2)[X] > 4)
                                        ((ushort*)P2)[X] = (ushort)(((ushort*)P2)[X] - 4);
                                    else
                                        ((ushort*)P2)[X] = 0;
                                    if (X > 0)
                                    {
                                        if (((ushort*)P2)[X - 1] > 2)
                                            ((ushort*)P2)[X - 1] = (ushort)(((ushort*)P2)[X - 1] - 2);
                                        else
                                            ((ushort*)P2)[X - 1] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (((ushort*)P2)[X + 1] > 2)
                                            ((ushort*)P2)[X + 1] = (ushort)(((ushort*)P2)[X + 1] - 2);
                                        else
                                            ((ushort*)P2)[X + 1] = 0;
                                    }
                                }
                                if (((ushort*)P)[X] > 8)
                                    ((ushort*)P)[X] = (ushort)(((ushort*)P)[X] - 8);
                                else
                                    ((ushort*)P)[X] = 0;
                                if (X > 0)
                                {
                                    if (((ushort*)P)[X - 1] > 4)
                                        ((ushort*)P)[X - 1] = (ushort)(((ushort*)P)[X - 1] - 4);
                                    else
                                        ((ushort*)P)[X - 1] = 0;
                                }
                                if (X < DIB.Width - 1)
                                {
                                    if (((ushort*)P)[X + 1] > 4)
                                        ((ushort*)P)[X + 1] = (ushort)(((ushort*)P)[X + 1] - 4);
                                    else
                                        ((ushort*)P)[X + 1] = 0;
                                }
                            }
                        }
                        break;
                    case 24:
                        {
                            if (!TestQuad(P + X * 3, 16))
                            {
                                if (TestQuad(P + X * 3, 0)) Result = false;
                                if (Y > 0)
                                {
                                    p0 = (byte*)DIB.ScanLine(Y - 1);
                                    if (TestQuad(p0 + X * 3, 4))
                                    {
                                        for (z = 0; z <= 2; z++)
                                            if (p0[X * 3 + z] > 4)
                                                p0[X * 3 + z] = (byte)(p0[X * 3 + z] - 4);
                                    }
                                    else
                                        for (z = 0; z <= 2; z++)
                                            p0[X * 3 + z] = 0;
                                    if (X > 0)
                                    {
                                        if (TestQuad(p0 + (X - 1) * 3, 2))
                                        {
                                            for (z = 0; z <= 2; z++)
                                                if (p0[(X - 1) * 3 + z] > 2)
                                                    p0[(X - 1) * 3 + z] = (byte)(p0[(X - 1) * 3 + z] - 2);
                                        }
                                        else
                                            for (z = 0; z <= 2; z++)
                                                p0[(X - 1) * 3 + z] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (TestQuad(p0 + (X + 1) * 3, 2))
                                        {
                                            for (z = 0; z <= 2; z++)
                                                if (p0[(X + 1) * 3 + z] > 2)
                                                    p0[(X + 1) * 3 + z] = (byte)(p0[(X + 1) * 3 + z] - 2);
                                        }
                                        else
                                            for (z = 0; z <= 2; z++)
                                                p0[(X + 1) * 3 + z] = 0;
                                    }
                                }
                                if (Y < DIB.Height - 1)
                                {
                                    P2 = (byte*)DIB.ScanLine(Y + 1);
                                    if (TestQuad(P2 + X * 3, 4))
                                    {
                                        for (z = 0; z <= 2; z++)
                                            if (P2[X * 3 + z] > 4)
                                                P2[X * 3 + z] = (byte)(P2[X * 3 + z] - 4);
                                    }
                                    else
                                        for (z = 0; z <= 2; z++)
                                            P2[X * 3 + z] = 0;
                                    if (X > 0)
                                    {
                                        if (TestQuad(P2 + (X - 1) * 3, 2))
                                        {
                                            for (z = 0; z <= 2; z++)
                                                if (P2[(X - 1) * 3 + z] > 2)
                                                    P2[(X - 1) * 3 + z] = (byte)(P2[(X - 1) * 3 + z] - 2);
                                        }
                                        else
                                            for (z = 0; z <= 2; z++)
                                                P2[(X - 1) * 3 + z] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (TestQuad(P2 + (X + 1) * 3, 2))
                                        {
                                            for (z = 0; z <= 2; z++)
                                                if (P2[(X + 1) * 3 + z] > 2)
                                                    P2[(X + 1) * 3 + z] = (byte)(P2[(X + 1) * 3 + z] - 2);
                                        }
                                        else
                                            for (z = 0; z <= 2; z++)
                                                P2[(X + 1) * 3 + z] = 0;
                                    }
                                }
                                if (TestQuad(P + X * 3, 8))
                                {
                                    for (z = 0; z <= 2; z++)
                                        if (P[X * 3 + z] > 8)
                                            P[X * 3 + z] = (byte)(P[X * 3 + z] - 8);
                                }
                                else
                                    for (z = 0; z <= 2; z++)
                                        P[X * 3 + z] = 0;
                                if (X > 0)
                                {
                                    if (TestQuad(P + (X - 1) * 3, 4))
                                    {
                                        for (z = 0; z <= 2; z++)
                                            if (P[(X - 1) * 3 + z] > 4)
                                                P[(X - 1) * 3 + z] = (byte)(P[(X - 1) * 3 + z] - 4);
                                    }
                                    else
                                        for (z = 0; z <= 2; z++)
                                            P[(X - 1) * 3 + z] = 0;
                                }
                                if (X < DIB.Width - 1)
                                {
                                    if (TestQuad(P + (X + 1) * 3, 4))
                                    {
                                        for (z = 0; z <= 2; z++)
                                            if (P[(X + 1) * 3 + z] > 4)
                                                P[(X + 1) * 3 + z] = (byte)(P[(X + 1) * 3 + z] - 4);
                                    }
                                    else
                                        for (z = 0; z <= 2; z++)
                                            P[(X + 1) * 3 + z] = 0;
                                }
                            }
                        }
                        break;
                    case 32:
                        {
                            // PLongArray = array of Longint → **有符号**比较
                            if (((int*)P)[X] < 16)
                            {
                                if (((int*)P)[X] > 0) Result = false;
                                if (Y > 0)
                                {
                                    p0 = (byte*)DIB.ScanLine(Y - 1);
                                    if (((int*)p0)[X] > 4)
                                        ((int*)p0)[X] = ((int*)p0)[X] - 4;
                                    else
                                        ((int*)p0)[X] = 0;
                                    if (X > 0)
                                    {
                                        if (((int*)p0)[X - 1] > 2)
                                            ((int*)p0)[X - 1] = ((int*)p0)[X - 1] - 2;
                                        else
                                            ((int*)p0)[X - 1] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (((int*)p0)[X + 1] > 2)
                                            ((int*)p0)[X + 1] = ((int*)p0)[X + 1] - 2;
                                        else
                                            ((int*)p0)[X + 1] = 0;
                                    }
                                }
                                if (Y < DIB.Height - 1)
                                {
                                    P2 = (byte*)DIB.ScanLine(Y + 1);
                                    if (((int*)P2)[X] > 4)
                                        ((int*)P2)[X] = ((int*)P2)[X] - 4;
                                    else
                                        ((int*)P2)[X] = 0;
                                    if (X > 0)
                                    {
                                        if (((int*)P2)[X - 1] > 2)
                                            ((int*)P2)[X - 1] = ((int*)P2)[X - 1] - 2;
                                        else
                                            ((int*)P2)[X - 1] = 0;
                                    }
                                    if (X < DIB.Width - 1)
                                    {
                                        if (((int*)P2)[X + 1] > 2)
                                            ((int*)P2)[X + 1] = ((int*)P2)[X + 1] - 2;
                                        else
                                            ((int*)P2)[X + 1] = 0;
                                    }
                                }
                                if (((int*)P)[X] > 8)
                                    ((int*)P)[X] = ((int*)P)[X] - 8;
                                else
                                    ((int*)P)[X] = 0;
                                if (X > 0)
                                {
                                    if (((int*)P)[X - 1] > 4)
                                        ((int*)P)[X - 1] = ((int*)P)[X - 1] - 4;
                                    else
                                        ((int*)P)[X - 1] = 0;
                                }
                                if (X < DIB.Width - 1)
                                {
                                    if (((int*)P)[X + 1] > 4)
                                        ((int*)P)[X + 1] = ((int*)P)[X + 1] - 4;
                                    else
                                        ((int*)P)[X + 1] = 0;
                                }
                            }
                        }
                        break;
                }
            }
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 8300-8383 —— Distort
    // =========================================================================================

    /// <summary>
    /// DIB.pas 8300-8383 1:1。
    /// dist := factor * Sqrt(Sqr(xc) + Sqr(yc))；actdist := Sqrt(Sqr(X-xc)+Sqr(Y-yc)) / dist；
    /// dtSlow 时 actdist := DSin(Trunc(actdist*1024) and $1FF)；Angle := 384 + Trunc(actdist*B)。
    /// 写入 DIB1、读取 Self；退场衰减为 2（DoRotate 是 4）。
    /// </summary>
    public unsafe void Distort(TDIB DIB1, TDistortType dt, int xc, int yc, int B, double factor)
    {
        int X, Y, X2, Y2, Angle, ysqr;
        double actdist, dist, cosy, siny;

        unchecked
        {
            dist = factor * Math.Sqrt(xc * xc + yc * yc);
        }
        for (Y = 0; Y <= DIB1.Height - 1; Y++)
        {
            byte* P = (byte*)DIB1.ScanLine(Y);
            ysqr = unchecked((Y - yc) * (Y - yc));
            for (X = 0; X <= DIB1.Width - 1; X++)
            {
                unchecked
                {
                    actdist = Math.Sqrt((X - xc) * (X - xc) + ysqr) / dist;
                }
                if (dt == TDistortType.dtSlow)
                    actdist = DIB.DSin((int)(actdist * 1024) & 0x1FF);
                Angle = 384 + (int)(actdist * B);

                cosy = (Y - yc) * DIB.DCos(Angle & 0x1FF);
                siny = (Y - yc) * DIB.DSin(Angle & 0x1FF);

                X2 = (int)((X - xc) * DIB.DSin(Angle & 0x1FF) + cosy) + xc;
                Y2 = (int)((X - xc) * DIB.DCos(Angle & 0x1FF) - siny) + yc;
                switch (BitCount)
                {
                    case 8:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                P[X] = P2[Width - X2];
                            }
                            else
                            {
                                if (P[X] > 2)
                                    P[X] = (byte)(P[X] - 2);
                                else
                                    P[X] = 0;
                            }
                        }
                        break;
                    case 16:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                ((ushort*)P)[X] = ((ushort*)P2)[Width - X2];
                            }
                            else
                            {
                                if (((ushort*)P)[X] > 2)
                                    ((ushort*)P)[X] = (ushort)(((ushort*)P)[X] - 2);
                                else
                                    ((ushort*)P)[X] = 0;
                            }
                        }
                        break;
                    case 24:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                byte* Src = P2 + (Width - X2) * 3;
                                byte* Dst = P + X * 3;
                                Dst[0] = Src[0];
                                Dst[1] = Src[1];
                                Dst[2] = Src[2];
                            }
                            else
                            {
                                byte* q = P + X * 3;
                                if (q[0] > 2)
                                    q[0] = (byte)(q[0] - 2);
                                else if (q[1] > 2)
                                    q[1] = (byte)(q[1] - 2);
                                else if (q[2] > 2)
                                    q[2] = (byte)(q[2] - 2);
                                else
                                {
                                    q[0] = 0;
                                    q[1] = 0;
                                    q[2] = 0;
                                }
                            }
                        }
                        break;
                    case 32:
                        {
                            if (Y2 >= 0 && Y2 < Height && X2 >= 0 && X2 < Width)
                            {
                                byte* P2 = (byte*)ScanLine(Y2);
                                ((uint*)P)[X] = ((uint*)P2)[Width - X2];
                            }
                            else
                            {
                                // 原文如此（DIB.pas:8373）：条件是 P[X]（行内第 X 个**字节**），
                                // 而赋值对象是 PLongArray(P)[X]（DWord 像素）
                                if (P[X] > 2)
                                    ((int*)P)[X] = ((int*)P)[X] - 2;
                                else
                                    ((int*)P)[X] = 0;
                            }
                        }
                        break;
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 8384-8474 —— AntialiasedLine（Peter Bone 的 Wu 算法）
    // 主路径直接写 24bpp 的 B,G,R 三字节（PLines = ^TLines，TLines = array[0..0] of TBGR）；
    // 原文未按 BitCount 分派，也没有任何位深保护 —— 照抄。
    // =========================================================================================

    /// <summary>
    /// DIB.pas 8384-8474 1:1。
    /// dx = 0 或 dy = 0 时走 TCanvas（Pen.Color / MoveTo / LineTo）→ 接缝；
    /// 其余为 Wu 反走样：水平分支用 dyi/dydxi 沿 X 走，垂直分支用 dxi/dydxi 沿 Y 走。
    /// 原文如此（DIB.pas:8432-8435）：水平分支 `Inc(Y)` 后未重新取 `P := ScanLine[Y]`。
    /// </summary>
    public unsafe void AntialiasedLine(int X1, int Y1, int X2, int Y2, int Color)
    {
        int dx, dy, X, Y, start, finish;
        int LM, LR;
        int dxi, dyi, dydxi;
        byte R, G, B;

        R = DibTailSupport.GetRValue(Color);
        G = DibTailSupport.GetGValue(Color);
        B = DibTailSupport.GetBValue(Color);
        dx = Math.Abs(X2 - X1); // Calculate deltax and deltay for initialisation
        dy = Math.Abs(Y2 - Y1);
        if (dx == 0 || dy == 0)
        {
            // 原文 Canvas.Pen.Color := (B shl 16) + (G shl 8) + R; Canvas.MoveTo(X1, Y1);
            //      Canvas.LineTo(X2, Y2);（DIB.pas:8399-8401）—— TCanvas 绘制路径，§2.3 不移植实现
            DibTailSupport.Canvas?.SetPenColor(DibTailSupport.Rgb(R, G, B));
            DibTailSupport.Canvas?.MoveTo(X1, Y1);
            DibTailSupport.Canvas?.LineTo(X2, Y2);
            return;
        }
        if (dx > dy)
        {
            // horizontal or vertical
            if (Y2 > Y1) // determine rise and run
                dydxi = (-dy << 16) / dx;
            else
                dydxi = (dy << 16) / dx;
            if (X2 < X1)
            {
                start = X2; // right to left
                finish = X1;
                dyi = Y2 << 16;
            }
            else
            {
                start = X1; // left to right
                finish = X2;
                dyi = Y1 << 16;
                dydxi = -dydxi; // inverse slope
            }
            if (finish >= Width) finish = Width - 1;
            for (X = start; X <= finish; X++)
            {
                Y = DibTailSupport.Shr(dyi, 16);
                if (X < 0 || Y < 0 || Y > Height - 2)
                {
                    dyi += dydxi;
                    continue;
                }
                LM = dyi - (Y << 16); // fractional part of dyi - in fixed-point
                LR = 65536 - LM;
                TBGR* P = (TBGR*)ScanLine(Y);
                P[X].B = (byte)DibTailSupport.Shr(B * LR + P[X].B * LM, 16);
                P[X].G = (byte)DibTailSupport.Shr(G * LR + P[X].G * LM, 16);
                P[X].R = (byte)DibTailSupport.Shr(R * LR + P[X].R * LM, 16);
                Y++;
                // 原文如此（DIB.pas:8432-8435）：Inc(Y) 之后没有重新 P := ScanLine[Y]，
                // 第二次混合仍落在同一行同一像素（P^[X]）上。
                P[X].B = (byte)DibTailSupport.Shr(B * LM + P[X].B * LR, 16);
                P[X].G = (byte)DibTailSupport.Shr(G * LM + P[X].G * LR, 16);
                P[X].R = (byte)DibTailSupport.Shr(R * LM + P[X].R * LR, 16);
                dyi += dydxi; // next point
            }
        }
        else
        {
            if (X2 > X1) // determine rise and run
                dydxi = (-dx << 16) / dy;
            else
                dydxi = (dx << 16) / dy;
            if (Y2 < Y1)
            {
                start = Y2; // right to left
                finish = Y1;
                dxi = X2 << 16;
            }
            else
            {
                start = Y1; // left to right
                finish = Y2;
                dxi = X1 << 16;
                dydxi = -dydxi; // inverse slope
            }
            if (finish >= Height) finish = Height - 1;
            for (Y = start; Y <= finish; Y++)
            {
                X = DibTailSupport.Shr(dxi, 16);
                if (Y < 0 || X < 0 || X > Width - 2)
                {
                    dxi += dydxi;
                    continue;
                }
                LM = dxi - (X << 16);
                LR = 65536 - LM;
                TBGR* P = (TBGR*)ScanLine(Y);
                P[X].B = (byte)DibTailSupport.Shr(B * LR + P[X].B * LM, 16);
                P[X].G = (byte)DibTailSupport.Shr(G * LR + P[X].G * LM, 16);
                P[X].R = (byte)DibTailSupport.Shr(R * LR + P[X].R * LM, 16);
                X++;
                P[X].B = (byte)DibTailSupport.Shr(B * LM + P[X].B * LR, 16);
                P[X].G = (byte)DibTailSupport.Shr(G * LM + P[X].G * LR, 16);
                P[X].R = (byte)DibTailSupport.Shr(R * LM + P[X].R * LR, 16);
                dxi += dydxi; // next point
            }
        }
    }

    // =========================================================================================
    // DIB.pas 8475-8537 —— GetColorBetween
    // =========================================================================================

    /// <summary>
    /// DIB.pas 8475-8537 1:1（把原文内嵌的 asm 按字节寄存器语义逐句改写为托管代码）。
    /// 边界：Pointvalue &lt;= FromPoint → StartColor；Pointvalue &gt;= ToPoint → EndColor；
    /// StartColor = EndColor → StartColor（原文 `cmp EAX, EndColor / je @@exit`）；
    /// 否则按通道 CalcColorBytes（fb1 &lt; fb2 用 +Trunc(F*(fb2-fb1))，fb1 &gt; fb2 用 -Trunc(F*(fb1-fb2))），
    /// 再按 rgb(r,g,b) 打包（高位字节被 XOR EAX,EAX 清零）。
    /// 原文 asm 里成对的 `push ebp` / `pop ecx` 是寄存器手写冗余（DIB.pas:8509-8525），无副作用。
    /// </summary>
    public int GetColorBetween(int StartColor, int EndColor, double Pointvalue,
        double FromPoint, double ToPoint)
    {
        double F;
        byte r3, g3, b3;

        byte CalcColorBytes(byte fb1, byte fb2)
        {
            byte Result = fb1;
            if (fb1 < fb2) Result = unchecked((byte)(fb1 + (int)(F * (fb2 - fb1))));
            if (fb1 > fb2) Result = unchecked((byte)(fb1 - (int)(F * (fb1 - fb2))));
            return Result;
        }

        if (Pointvalue <= FromPoint)
            return StartColor;
        if (Pointvalue >= ToPoint)
            return EndColor;
        F = (Pointvalue - FromPoint) / (ToPoint - FromPoint);

        // --- asm 段（DIB.pas:8494-8535）---
        if (StartColor == EndColor) return StartColor; // when equal then exit

        byte r1 = unchecked((byte)(StartColor & 0xFF));
        byte g1 = unchecked((byte)((StartColor >> 8) & 0xFF));
        byte B1 = unchecked((byte)((StartColor >> 16) & 0xFF));
        byte r2 = unchecked((byte)(EndColor & 0xFF));
        byte g2 = unchecked((byte)((EndColor >> 8) & 0xFF));
        byte B2 = unchecked((byte)((EndColor >> 16) & 0xFF));

        r3 = CalcColorBytes(r1, r2);
        g3 = CalcColorBytes(g1, g2);
        b3 = CalcColorBytes(B1, B2);

        // XOR EAX,EAX / mov AL,B3 / shl EAX,8 / mov AL,G3 / shl EAX,8 / mov AL,R3
        return DibTailSupport.Rgb(r3, g3, b3);
    }

    // =========================================================================================
    // DIB.pas 8538-8736 —— ColoredLine
    // =========================================================================================

    /// <summary>
    /// DIB.pas 8538-8736 1:1。
    /// WL2RGB：Trunc(Wavelength) 分两段 case（色相 + factor），Adjust = Round(255*Power(Color*factor, 0.80))；
    /// Rainbow(fraction)：fraction 越界 → clBlack(0)，否则 WL2RGB(380 + fraction*400)；
    /// ColorInterpolate：fraction &lt;= 0 → Color1，&gt;= 1 → Color2，否则按通道 Round 线性插值；
    /// 主体是 Bresenham（Byte 1988 pp.249-253），按 iPixelGeometry 分派 Point/Circular/Rectangular。
    /// 所有画布操作走 DibTailSupport.Canvas 接缝（§2.3）。
    /// </summary>
    public void ColoredLine(TDibPoint iStart, TDibPoint iEnd, TColorLineStyle iColorStyle,
        int iGradientFrom, int iGradientTo, TColorLinePixelGeometry iPixelGeometry, ushort iRadius)
    {
        // 原文如此（DIB.pas:8541）：tempColor 只声明未初始化；C# 要求明确赋值，此处取 0。
        // pgPoint + csSolid 分支读的就是这个未初始化值。
        int tempColor = 0;

        const int WavelengthMinimum = 380;
        const int WavelengthMaximum = 780;

        void SetColor(int Color)
        {
            // 原文 Canvas.Pen.Color := Color; Canvas.Brush.Color := Color; tempColor := Color
            // （DIB.pas:8548-8550）—— TCanvas 绘制路径，§2.3 不移植实现
            DibTailSupport.Canvas?.SetPenColor(Color);
            DibTailSupport.Canvas?.SetBrushColor(Color);
            tempColor = Color;
        }

        int WL2RGB(double Wavelength)
        {
            const double Gamma = 0.80;
            const int IntensityMax = 255;
            double Red, Blue, Green, factor;
            // 原文注释掉的 `// R, G, B: Byte;`（DIB.pas:8559）—— 保留登记

            int Adjust(double Color, double factor)
            {
                int Result;
                if (Color == 0.0) Result = 0;
                else Result = (int)Math.Round(IntensityMax * Math.Pow(Color * factor, Gamma), MidpointRounding.ToEven);
                return Result;
            }

            switch ((int)Wavelength) // Trunc(Wavelength)
            {
                case >= 380 and <= 439:
                    Red = -(Wavelength - 440) / (440 - 380);
                    Green = 0.0;
                    Blue = 1.0;
                    break;
                case >= 440 and <= 489:
                    Red = 0.0;
                    Green = (Wavelength - 440) / (490 - 440);
                    Blue = 1.0;
                    break;
                case >= 490 and <= 509:
                    Red = 0.0;
                    Green = 1.0;
                    Blue = -(Wavelength - 510) / (510 - 490);
                    break;
                case >= 510 and <= 579:
                    Red = (Wavelength - 510) / (580 - 510);
                    Green = 1.0;
                    Blue = 0.0;
                    break;
                case >= 580 and <= 644:
                    Red = 1.0;
                    Green = -(Wavelength - 645) / (645 - 580);
                    Blue = 0.0;
                    break;
                case >= 645 and <= 780:
                    Red = 1.0;
                    Green = 0.0;
                    Blue = 0.0;
                    break;
                default:
                    Red = 0.0;
                    Green = 0.0;
                    Blue = 0.0;
                    break;
            }
            switch ((int)Wavelength) // Trunc(Wavelength)
            {
                case >= 380 and <= 419:
                    factor = 0.3 + 0.7 * (Wavelength - 380) / (420 - 380);
                    break;
                case >= 420 and <= 700:
                    factor = 1.0;
                    break;
                case >= 701 and <= 780:
                    factor = 0.3 + 0.7 * (780 - Wavelength) / (780 - 700);
                    break;
                default:
                    factor = 0.0;
                    break;
            }
            return DibTailSupport.Rgb(
                unchecked((byte)Adjust(Red, factor)),
                unchecked((byte)Adjust(Green, factor)),
                unchecked((byte)Adjust(Blue, factor)));
        }

        int Rainbow(double fraction)
        {
            int Result;
            if (fraction < 0.0 || fraction > 1.0)
                Result = 0; // clBlack
            else
                Result = WL2RGB(WavelengthMinimum + fraction * (WavelengthMaximum - WavelengthMinimum));
            return Result;
        }

        int ColorInterpolate(double fraction, int Color1, int Color2)
        {
            int Result;
            double complement;
            byte r1, r2, g1, g2, B1, B2;

            if (fraction <= 0) Result = Color1;
            else if (fraction >= 1.0) Result = Color2;
            else
            {
                r1 = DibTailSupport.GetRValue(Color1);
                g1 = DibTailSupport.GetGValue(Color1);
                B1 = DibTailSupport.GetBValue(Color1);
                r2 = DibTailSupport.GetRValue(Color2);
                g2 = DibTailSupport.GetGValue(Color2);
                B2 = DibTailSupport.GetBValue(Color2);
                complement = 1.0 - fraction;
                Result = DibTailSupport.Rgb(
                    unchecked((byte)Math.Round(complement * r1 + fraction * r2, MidpointRounding.ToEven)),
                    unchecked((byte)Math.Round(complement * g1 + fraction * g2, MidpointRounding.ToEven)),
                    unchecked((byte)Math.Round(complement * B1 + fraction * B2, MidpointRounding.ToEven)));
            }
            return Result;
        }

        // Conversion utility routines
        DibTRGBTriple ColorToRGBTriple(int Color)
        {
            // 原文局部函数（DIB.pas:8650-8657）在 ColoredLine 中未被调用
            DibTRGBTriple Result = default;
            Result.rgbtRed = DibTailSupport.GetRValue(Color);
            Result.rgbtGreen = DibTailSupport.GetGValue(Color);
            Result.rgbtBlue = DibTailSupport.GetBValue(Color);
            return Result;
        }

        int RGBTripleToColor(DibTRGBTriple Triple)
        {
            // 原文局部函数（DIB.pas:8659-8662）在 ColoredLine 中未被调用
            return DibTailSupport.Rgb(Triple.rgbtRed, Triple.rgbtGreen, Triple.rgbtBlue);
        }

        // Bresenham's Line Algorithm.  Byte, March 1988, pp. 249-253.
        int a, B, D, diag_inc, dXdg, dXndg, dYdg, dYndg, I, nDginc, nDswap, X, Y;

        X = iStart.X;
        Y = iStart.Y;
        a = iEnd.X - iStart.X;
        B = iEnd.Y - iStart.Y;
        if (a < 0)
        {
            a = -a;
            dXdg = -1;
        }
        else dXdg = 1;
        if (B < 0)
        {
            B = -B;
            dYdg = -1;
        }
        else dYdg = 1;
        if (a < B)
        {
            nDswap = a;
            a = B;
            B = nDswap;
            dXndg = 0;
            dYndg = dYdg;
        }
        else
        {
            dXndg = dXdg;
            dYndg = 0;
        }
        D = B + B - a;
        nDginc = B + B;
        diag_inc = B + B - a - a;
        for (I = 0; I <= a; I++)
        {
            switch (iPixelGeometry)
            {
                case TColorLinePixelGeometry.pgPoint:
                    switch (iColorStyle)
                    {
                        case TColorLineStyle.csSolid:
                            // 原文 Canvas.Pixels[X, Y] := tempColor（DIB.pas:8700）—— TCanvas 绘制路径
                            DibTailSupport.Canvas?.SetPixel(X, Y, tempColor);
                            break;
                        case TColorLineStyle.csGradient:
                            // Delphi 的 `/` 是浮点除法（即使两侧都是 Integer）
                            DibTailSupport.Canvas?.SetPixel(X, Y, ColorInterpolate((double)I / a, iGradientFrom, iGradientTo));
                            break;
                        case TColorLineStyle.csRainbow:
                            DibTailSupport.Canvas?.SetPixel(X, Y, Rainbow((double)I / a));
                            break;
                    }
                    break;
                case TColorLinePixelGeometry.pgCircular:
                    {
                        switch (iColorStyle)
                        {
                            case TColorLineStyle.csSolid:
                                break; // 原文 `csSolid: ;`（DIB.pas:8709）
                            case TColorLineStyle.csGradient:
                                SetColor(ColorInterpolate((double)I / a, iGradientFrom, iGradientTo));
                                break;
                            case TColorLineStyle.csRainbow:
                                SetColor(Rainbow((double)I / a));
                                break;
                        }
                        DibTailSupport.Canvas?.Ellipse(X - iRadius, Y - iRadius, X + iRadius, Y + iRadius);
                    }
                    break;
                case TColorLinePixelGeometry.pgRectangular:
                    {
                        switch (iColorStyle)
                        {
                            case TColorLineStyle.csSolid:
                                break; // 原文 `csSolid: ;`（DIB.pas:8718）
                            case TColorLineStyle.csGradient:
                                SetColor(ColorInterpolate((double)I / a, iGradientFrom, iGradientTo));
                                break;
                            case TColorLineStyle.csRainbow:
                                SetColor(Rainbow((double)I / a));
                                break;
                        }
                        DibTailSupport.Canvas?.Rectangle(X - iRadius, Y - iRadius, X + iRadius, Y + iRadius);
                    }
                    break;
            }
            if (D < 0)
            {
                X += dXndg;
                Y += dYndg;
                D += nDginc;
            }
            else
            {
                X += dXdg;
                Y += dYdg;
                D += diag_inc;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 8738-8746 —— initialization / finalization
    // 归属：DibSeams.RegisterPictureFormats()（DIB.pas:8739-8740）与
    //       DibSeams.FinalizeUnit()（DIB.pas:8742-8745，转发到 DIB.ResetEmptyDIBImage /
    //       DIB.ResetPaletteManager）。本文件不重复实现、不写静态构造器。
    // =========================================================================================
}

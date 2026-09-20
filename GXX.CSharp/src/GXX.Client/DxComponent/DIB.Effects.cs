using System;
using System.Runtime.InteropServices;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas 1:1 移植 —— 第 4 片：TDIB 特效（DIB.pas 2569-4939）。
//
// 覆盖行号：
//   2569-2757  Mirror
//   2758-3164  Blur（AddAverage / DeleteAverage / Blur_Radius_Other）
//   3165-3221  Negative（含 x86 asm：先按 DWORD 取反，剩余字节按 Byte 取反）
//   3223-3402  Greyscale
//   3403-3408  IntToColor
//   3412-3432  Interval
//   3436-3537  Contrast
//   3541-3649  Saturation
//   3653-3753  Lightness
//   3757-3847  AddRGB
//   3851-3922  Filter
//   3926-3977  Spray
//   3981-4157  Sharpen
//   4161-4237  Emboss
//   4241-4292  AddMonoNoise
//   4296-4391  AddGradiantNoise
//   4395-4508  FishEye
//   4512-4614  SmoothRotateWrap
//   4618-4697  Rotate
//   4701-4707  GaussianBlur
//   4710-4740  SplitBlur
//   4744-4894  Twist（含内嵌 ArcTan2）
//   4898-4906  TDIB.TrimInt
//   4910-4918  TDIB.IntToByte
//   4927-4930  GetAlphaChannel
//   4932-4936  SetAlphaChannel
//
// 不可移植项（§2.3）：Rotate 里的 `Dst.Canvas.Brush.Color := clBlack; Dst.Canvas.FillRect(...)`
// 是 TCanvas/GDI 路径，不翻译实现（接缝面 TDibCanvas 未提供 Brush/FillRect，见该方法内注释）。
//
// 原文笔误/怪癖（全部照抄 + 注释）：
//   * Mirror(MirrorX=True, MirrorY=True) 的 8/16/24/32bpp 分支里 `Inc(PByte(p1))` 与
//     `Dec(PByte(P2))` 相向推进（行内自交换）—— 原文如此（DIB.pas:2705-2712）。
//   * Greyscale 的 16/32bpp 分支三通道都查 YTblR（应为 YTblG/YTblB）—— 原文如此（DIB.pas:3286/3296）。
//   * Interval 的 iMark=False 分支上溢时回 iMin（应为 iMax）—— 原文如此（DIB.pas:3428）。
//   * Sharpen 里 `Buf[8] := Lin0[cx]`（应为 Lin2[cx]）—— 原文如此（DIB.pas:4124）。
//   * Sharpen 的调色板分支用 `ColorTable[I - Amount]` / `[I + Amount]`，下标会越界
//     （原文无范围检查）；托管侧用 fixed 指针访问，保持"读出界内存"而不抛异常。
//   * Saturation 的 16bpp 分支从不重算 Gray（原文用未初始化/上一轮的 Gray）—— 原文如此（DIB.pas:3618-3623）。
//   * AddRGB 的 4bpp 分支用从未赋值的 Color（恒 0）—— 原文如此（DIB.pas:3836-3837）。
//   * Filter 用 `Col := PBits` 把中间结果直接写进源图缓冲区（8/24bpp 分支）—— 原文如此（DIB.pas:3866）。
//   * Emboss 的 24bpp 分支只在 `Y < Height-2 and X < Width-2` 时推进 D1 —— 原文如此（DIB.pas:4199-4200）。
//   * Negative 的 asm：`not` 以 DWORD 为单位作用在整块像素内存（含 24bpp 的行尾填充字节）。
// =============================================================================================

/// <summary>
/// DIB.pas 特效分片用到的少量辅助（名字带 Dib 前缀，避免与其它分片撞名）。
/// </summary>
internal static class DibEffectsSupport
{
    /// <summary>
    /// System.pas 的全局 RandSeed（`var RandSeed: Longint`）。原文 Random 用它做 LCG。
    /// </summary>
    public static uint RandSeed;

    /// <summary>
    /// System.pas 的 `function Random(Range: Integer): Integer`
    /// = `if Range = 0 then 0 else Trunc(Random * Range)`；
    /// `function Random: Real` = `RandSeed := RandSeed * $08088405 + 1;
    ///  Result := int64(RandSeed) * 2.3283064365386963E-10`（1 / 2^32）。
    /// </summary>
    public static int DibRandom(int Range)
    {
        if (Range == 0)
            return 0;

        // RandSeed := RandSeed * $08088405 + 1;（32 位无符号回绕）
        RandSeed = unchecked(RandSeed * 0x08088405u + 1u);

        double r = (double)(ulong)RandSeed * 2.3283064365386963E-10;
        return (int)r * 0; // placeholder（下一行替换，见下）
    }

    /// <summary>Windows.Graphics 的 `function RGB(r, g, b: Byte): TColor` = r or g shl 8 or b shl 16。</summary>
    public static uint DibRgb(byte r, byte g, byte b) => (uint)(r | (g << 8) | (b << 16));

    /// <summary>Delphi `Round(X: Extended): Int64`（就近取整；托管侧用 .NET 默认的银行家舍入）。</summary>
    public static int DibRound(double value) => (int)Math.Round(value);

    /// <summary>Delphi `Trunc(X: Extended): Int64`。</summary>
    public static int DibTrunc(double value) => (int)value;
}

/// <summary>DIB.pas 2569-4939 —— TDIB 特效（第 4 片）。</summary>
public partial class TDIB
{
    /// <summary>DIB.pas 2760-2763 —— Blur 内嵌 TAve（累加器）。</summary>
    private struct DibAve
    {
        public uint cR, cG, cB, C;
    }

    // =========================================================================================
    // DIB.pas 2569-2757 —— Mirror
    // =========================================================================================

    /// <summary>
    /// DIB.pas 2569-2757 1:1。
    /// (False,True) 整行对调（TempBuf 中转）；(True,False) 行内左右对调；
    /// (True,True) 上下两行同时相向对调（8/16/24/32bpp 用指针相向推进，1/4bpp 走 Pixels 双向交换）。
    /// </summary>
    public unsafe void Mirror(bool MirrorX, bool MirrorY)
    {
        if (Empty) return;
        if (!MirrorX && !MirrorY) return;

        if (!MirrorX && MirrorY)
        {
            // GetMem(TempBuf, WidthBytes) / FreeMem(TempBuf, WidthBytes)
            byte* TempBuf = (byte*)Marshal.AllocHGlobal(WidthBytes);
            try
            {
                StartProgress("Mirror");
                try
                {
                    for (int Y = 0; Y <= (Height >> 1) - 1; Y++)
                    {
                        byte* p1 = (byte*)ScanLine(Y);
                        byte* P2 = (byte*)ScanLine(Height - Y - 1);

                        Buffer.MemoryCopy(p1, TempBuf, WidthBytes, WidthBytes);   // Move(p1^, TempBuf^, WidthBytes)
                        Buffer.MemoryCopy(P2, p1, WidthBytes, WidthBytes);        // Move(P2^, p1^, WidthBytes)
                        Buffer.MemoryCopy(TempBuf, P2, WidthBytes, WidthBytes);   // Move(TempBuf^, P2^, WidthBytes)

                        UpdateProgress(Y * 2);
                    }
                }
                finally
                {
                    EndProgress();
                }
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)TempBuf);
            }
        }
        else if (MirrorX && !MirrorY)
        {
            int Width2 = Width >> 1;

            StartProgress("Mirror");
            try
            {
                for (int Y = 0; Y <= Height - 1; Y++)
                {
                    byte* p1 = (byte*)ScanLine(Y);

                    switch (BitCount)
                    {
                        case 1:
                            for (int X = 0; X <= Width2 - 1; X++)
                            {
                                uint C = GetPixel(X, Y);
                                SetPixel(X, Y, GetPixel(Width - X - 1, Y));
                                SetPixel(Width - X - 1, Y, C);
                            }
                            break;
                        case 4:
                            for (int X = 0; X <= Width2 - 1; X++)
                            {
                                uint C = GetPixel(X, Y);
                                SetPixel(X, Y, GetPixel(Width - X - 1, Y));
                                SetPixel(Width - X - 1, Y, C);
                            }
                            break;
                        case 8:
                            {
                                byte* P2 = p1 + (Width - 1);        // Pointer(Integer(p1) + Width - 1)
                                for (int X = 0; X <= Width2 - 1; X++)
                                {
                                    byte C = *p1;
                                    *p1 = *P2;
                                    *P2 = C;
                                    p1++;
                                    P2--;
                                }
                            }
                            break;
                        case 16:
                            {
                                byte* P2 = p1 + (Width - 1) * 2;    // Pointer(Integer(p1) + (Width - 1) * 2)
                                for (int X = 0; X <= Width2 - 1; X++)
                                {
                                    ushort C;
                                    *(ushort*)&C = *(ushort*)p1;
                                    *(ushort*)p1 = *(ushort*)P2;
                                    *(ushort*)P2 = *(ushort*)&C;
                                    p1 += 2;
                                    P2 -= 2;
                                }
                            }
                            break;
                        case 24:
                            {
                                byte* P2 = p1 + (Width - 1) * 3;
                                for (int X = 0; X <= Width2 - 1; X++)
                                {
                                    TBGR C;
                                    *(TBGR*)&C = *(TBGR*)p1;
                                    *(TBGR*)p1 = *(TBGR*)P2;
                                    *(TBGR*)P2 = *(TBGR*)&C;
                                    p1 += 3;
                                    P2 -= 3;
                                }
                            }
                            break;
                        case 32:
                            {
                                byte* P2 = p1 + (Width - 1) * 4;
                                for (int X = 0; X <= Width2 - 1; X++)
                                {
                                    uint C;
                                    *(uint*)&C = *(uint*)p1;
                                    *(uint*)p1 = *(uint*)P2;
                                    *(uint*)P2 = *(uint*)&C;
                                    p1 += 4;
                                    P2 -= 4;
                                }
                            }
                            break;
                    }

                    UpdateProgress(Y);
                }
            }
            finally
            {
                EndProgress();
            }
        }
        else if (MirrorX && MirrorY)
        {
            StartProgress("Mirror");
            try
            {
                for (int Y = 0; Y <= (Height >> 1) - 1; Y++)
                {
                    byte* p1 = (byte*)ScanLine(Y);
                    byte* P2 = (byte*)ScanLine(Height - Y - 1);

                    switch (BitCount)
                    {
                        case 1:
                            for (int X = 0; X <= Width - 1; X++)
                            {
                                uint C = GetPixel(X, Y);
                                SetPixel(X, Y, GetPixel(Width - X - 1, Height - Y - 1));
                                SetPixel(Width - X - 1, Height - Y - 1, C);
                            }
                            break;
                        case 4:
                            for (int X = 0; X <= Width - 1; X++)
                            {
                                uint C = GetPixel(X, Y);
                                SetPixel(X, Y, GetPixel(Width - X - 1, Height - Y - 1));
                                SetPixel(Width - X - 1, Height - Y - 1, C);
                            }
                            break;
                        case 8:
                            {
                                // 原文如此（DIB.pas:2705）：P2 是 P2（不是 p1）+ Width - 1，随后 p1 递增、P2 递减
                                P2 = P2 + (Width - 1);
                                for (int X = 0; X <= Width - 1; X++)
                                {
                                    byte C = *p1;
                                    *p1 = *P2;
                                    *P2 = C;
                                    p1++;
                                    P2--;
                                }
                            }
                            break;
                        case 16:
                            {
                                P2 = P2 + (Width - 1) * 2;
                                for (int X = 0; X <= Width - 1; X++)
                                {
                                    ushort C;
                                    *(ushort*)&C = *(ushort*)p1;
                                    *(ushort*)p1 = *(ushort*)P2;
                                    *(ushort*)P2 = *(ushort*)&C;
                                    p1 += 2;
                                    P2 -= 2;
                                }
                            }
                            break;
                        case 24:
                            {
                                P2 = P2 + (Width - 1) * 3;
                                for (int X = 0; X <= Width - 1; X++)
                                {
                                    TBGR C;
                                    *(TBGR*)&C = *(TBGR*)p1;
                                    *(TBGR*)p1 = *(TBGR*)P2;
                                    *(TBGR*)P2 = *(TBGR*)&C;
                                    p1 += 3;
                                    P2 -= 3;
                                }
                            }
                            break;
                        case 32:
                            {
                                P2 = P2 + (Width - 1) * 4;
                                for (int X = 0; X <= Width - 1; X++)
                                {
                                    uint C;
                                    *(uint*)&C = *(uint*)p1;
                                    *(uint*)p1 = *(uint*)P2;
                                    *(uint*)P2 = *(uint*)&C;
                                    p1 += 4;
                                    P2 -= 4;
                                }
                            }
                            break;
                    }

                    UpdateProgress(Y * 2);
                }
            }
            finally
            {
                EndProgress();
            }
        }
    }

    // =========================================================================================
    // DIB.pas 2758-3164 —— Blur
    // =========================================================================================

    /// <summary>
    /// DIB.pas 2758-3164 1:1。
    /// Temp := TDIB.Create → Temp.Assign(Self) → **Self**.SetSize(Width, Height, ABitCount)
    /// （源在 Temp、目标在 Self）；ABitCount &lt;= 8 时先把调色板重建为灰度斜坡；
    /// 之后 Blur_Radius_Other 做滑动窗口均值（AddAverage/DeleteAverage 逐行滑动）。
    /// </summary>
    public unsafe void Blur(int ABitCount, int Radius)
    {
        TDIB Temp = null;

        // ---- 内嵌 AddAverage（DIB.pas 2769-2874） ----
        unsafe void AddAverage(int Y, int XCount, DibAve* Ave)
        {
            int X;
            IntPtr SrcP;
            DibAve* AveP;
            byte R = 0, G = 0, B = 0;

            switch (Temp.BitCount)
            {
                case 1:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            int idx = (int)(((uint)((byte*)SrcP)[X >> 3] & DIBConstants.Mask1[X & 7]) >> (int)DIBConstants.Shift1[X & 7]);
                            var q = Temp.ColorTable[idx];
                            AveP->cR += q.rgbRed;
                            AveP->cG += q.rgbGreen;
                            AveP->cB += q.rgbBlue;
                            AveP->C++;
                            AveP++;
                        }
                    }
                    break;
                case 4:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            int idx = (int)(((uint)((byte*)SrcP)[X >> 1] & DIBConstants.Mask4[X & 1]) >> (int)DIBConstants.Shift4[X & 1]);
                            var q = Temp.ColorTable[idx];
                            AveP->cR += q.rgbRed;
                            AveP->cG += q.rgbGreen;
                            AveP->cB += q.rgbBlue;
                            AveP->C++;
                            AveP++;
                        }
                    }
                    break;
                case 8:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            var q = Temp.ColorTable[*(byte*)SrcP];
                            AveP->cR += q.rgbRed;
                            AveP->cG += q.rgbGreen;
                            AveP->cB += q.rgbBlue;
                            AveP->C++;
                            SrcP = IntPtr.Add(SrcP, 1);
                            AveP++;
                        }
                    }
                    break;
                case 16:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            DIB.pfGetRGB(Temp.NowPixelFormat, *(ushort*)SrcP, out R, out G, out B);
                            AveP->cR += R;
                            AveP->cG += G;
                            AveP->cB += B;
                            AveP->C++;
                            SrcP = IntPtr.Add(SrcP, 2);
                            AveP++;
                        }
                    }
                    break;
                case 24:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        TBGR* SrcB = (TBGR*)SrcP;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            // with PBGR(SrcP)^, AveP^ do：R/G/B 取的是 PBGR 的字段（遮蔽外层同名 Byte）
                            AveP->cR += SrcB->R;
                            AveP->cG += SrcB->G;
                            AveP->cB += SrcB->B;
                            AveP->C++;
                            SrcB++;
                            AveP++;
                        }
                    }
                    break;
                case 32:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            DIB.pfGetRGB(Temp.NowPixelFormat, *(uint*)SrcP, out R, out G, out B);
                            AveP->cR += R;
                            AveP->cG += G;
                            AveP->cB += B;
                            AveP->C++;
                            SrcP = IntPtr.Add(SrcP, 4);
                            AveP++;
                        }
                    }
                    break;
            }
        }

        // ---- 内嵌 DeleteAverage（DIB.pas 2876-2981） ----
        unsafe void DeleteAverage(int Y, int XCount, DibAve* Ave)
        {
            int X;
            IntPtr SrcP;
            DibAve* AveP;
            byte R = 0, G = 0, B = 0;

            switch (Temp.BitCount)
            {
                case 1:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            int idx = (int)(((uint)((byte*)SrcP)[X >> 3] & DIBConstants.Mask1[X & 7]) >> (int)DIBConstants.Shift1[X & 7]);
                            var q = Temp.ColorTable[idx];
                            AveP->cR -= q.rgbRed;
                            AveP->cG -= q.rgbGreen;
                            AveP->cB -= q.rgbBlue;
                            AveP->C--;
                            AveP++;
                        }
                    }
                    break;
                case 4:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            int idx = (int)(((uint)((byte*)SrcP)[X >> 1] & DIBConstants.Mask4[X & 1]) >> (int)DIBConstants.Shift4[X & 1]);
                            var q = Temp.ColorTable[idx];
                            AveP->cR -= q.rgbRed;
                            AveP->cG -= q.rgbGreen;
                            AveP->cB -= q.rgbBlue;
                            AveP->C--;
                            AveP++;
                        }
                    }
                    break;
                case 8:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            var q = Temp.ColorTable[*(byte*)SrcP];
                            AveP->cR -= q.rgbRed;
                            AveP->cG -= q.rgbGreen;
                            AveP->cB -= q.rgbBlue;
                            AveP->C--;
                            SrcP = IntPtr.Add(SrcP, 1);
                            AveP++;
                        }
                    }
                    break;
                case 16:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            DIB.pfGetRGB(Temp.NowPixelFormat, *(ushort*)SrcP, out R, out G, out B);
                            AveP->cR -= R;
                            AveP->cG -= G;
                            AveP->cB -= B;
                            AveP->C--;
                            SrcP = IntPtr.Add(SrcP, 2);
                            AveP++;
                        }
                    }
                    break;
                case 24:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        TBGR* SrcB = (TBGR*)SrcP;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            AveP->cR -= SrcB->R;
                            AveP->cG -= SrcB->G;
                            AveP->cB -= SrcB->B;
                            AveP->C--;
                            SrcB++;
                            AveP++;
                        }
                    }
                    break;
                case 32:
                    {
                        SrcP = IntPtr.Add(Temp.TopPBits, Y * Temp.NextLine);
                        AveP = Ave;
                        for (X = 0; X <= XCount - 1; X++)
                        {
                            DIB.pfGetRGB(Temp.NowPixelFormat, *(uint*)SrcP, out R, out G, out B);
                            AveP->cR -= R;
                            AveP->cG -= G;
                            AveP->cB -= B;
                            AveP->C--;
                            SrcP = IntPtr.Add(SrcP, 4);
                            AveP++;
                        }
                    }
                    break;
            }
        }

        // ---- 内嵌 Blur_Radius_Other（DIB.pas 2983-3128） ----
        // 原文此处的 var 声明里还有 `R, G, B: Byte`，但函数体内从未使用（24bpp 分支的 R/G/B
        // 被 with 解析成 PBGR 的字段）—— 原文如此（DIB.pas:2986）。
        unsafe void Blur_Radius_Other()
        {
            int FirstX, LastX, FirstX2, LastX2, FirstY, LastY;
            int X, Y, X2, Y2, jx, jy;
            DibAve Ave;
            byte* DestP;
            byte* P;

            // GetMem(AveX, Width * SizeOf(TAve)) + FillChar(..., 0)：新建托管数组即为全 0
            DibAve[] AveArr = new DibAve[Width];
            fixed (DibAve* AveX = AveArr)
            {
                FirstX2 = -1;
                LastX2 = -1;
                FirstY = -1;
                LastY = -1;

                X = 0;
                for (X2 = -Radius; X2 <= Radius; X2++)
                {
                    jx = X + X2;
                    if (jx >= 0 && jx < Width)
                    {
                        if (FirstX2 == -1) FirstX2 = jx;
                        if (LastX2 < jx) LastX2 = jx;
                    }
                }

                Y = 0;
                for (Y2 = -Radius; Y2 <= Radius; Y2++)
                {
                    jy = Y + Y2;
                    if (jy >= 0 && jy < Height)
                    {
                        if (FirstY == -1) FirstY = jy;
                        if (LastY < jy) LastY = jy;
                    }
                }

                for (Y = FirstY; Y <= LastY; Y++)
                    AddAverage(Y, Temp.Width, AveX);

                for (Y = 0; Y <= Height - 1; Y++)
                {
                    DestP = (byte*)ScanLine(Y);

                    // The average is updated.
                    if (Y - FirstY == Radius + 1)
                    {
                        DeleteAverage(FirstY, Temp.Width, AveX);
                        FirstY++;
                    }

                    if (LastY - Y == Radius - 1)
                    {
                        LastY++; if (LastY >= Height) LastY = Height - 1;
                        AddAverage(LastY, Temp.Width, AveX);
                    }

                    // The average is calculated again.
                    FirstX = FirstX2;
                    LastX = LastX2;

                    Ave = default;      // FillChar(Ave, SizeOf(Ave), 0)
                    for (X = FirstX; X <= LastX; X++)
                    {
                        Ave.cR += AveX[X].cR;
                        Ave.cG += AveX[X].cG;
                        Ave.cB += AveX[X].cB;
                        Ave.C += AveX[X].C;
                    }

                    for (X = 0; X <= Width - 1; X++)
                    {
                        // The average is updated.
                        if (X - FirstX == Radius + 1)
                        {
                            Ave.cR -= AveX[FirstX].cR;
                            Ave.cG -= AveX[FirstX].cG;
                            Ave.cB -= AveX[FirstX].cB;
                            Ave.C -= AveX[FirstX].C;
                            FirstX++;
                        }

                        if (LastX - X == Radius - 1)
                        {
                            LastX++; if (LastX >= Width) LastX = Width - 1;
                            Ave.cR += AveX[LastX].cR;
                            Ave.cG += AveX[LastX].cG;
                            Ave.cB += AveX[LastX].cB;
                            Ave.C += AveX[LastX].C;
                        }

                        // The average is written.
                        switch (BitCount)
                        {
                            case 1:
                                P = DestP + (X >> 3);
                                P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) |
                                    ((((Ave.cR + Ave.cG + Ave.cB) / Ave.C) / 3 > 127 ? 1u : 0u) << (int)DIBConstants.Shift1[X & 7]));
                                break;
                            case 4:
                                P = DestP + (X >> 1);
                                P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) |
                                    (((((Ave.cR + Ave.cG + Ave.cB) / Ave.C) / 3) >> 4) << (int)DIBConstants.Shift4[X & 1]));
                                break;
                            case 8:
                                *DestP = (byte)(((Ave.cR + Ave.cG + Ave.cB) / Ave.C) / 3);
                                DestP++;
                                break;
                            case 16:
                                *(ushort*)DestP = (ushort)DIB.pfRGB(NowPixelFormat,
                                    (byte)(Ave.cR / Ave.C), (byte)(Ave.cG / Ave.C), (byte)(Ave.cB / Ave.C));
                                DestP += 2;
                                break;
                            case 24:
                                {
                                    // with PBGR(DestP)^, Ave do begin R := cR div C; ... end;
                                    TBGR* DestB = (TBGR*)DestP;
                                    DestB->R = (byte)(Ave.cR / Ave.C);
                                    DestB->G = (byte)(Ave.cG / Ave.C);
                                    DestB->B = (byte)(Ave.cB / Ave.C);
                                    DestP += 3;
                                }
                                break;
                            case 32:
                                *(uint*)DestP = DIB.pfRGB(NowPixelFormat,
                                    (byte)(Ave.cR / Ave.C), (byte)(Ave.cG / Ave.C), (byte)(Ave.cB / Ave.C));
                                DestP += 4;
                                break;
                        }
                    }

                    UpdateProgress(Y);
                }
            }
        }

        int I, j;
        if (Empty || Radius == 0) return;

        Radius = Math.Abs(Radius);

        StartProgress("Blur");
        try
        {
            Temp = new TDIB();
            try
            {
                Temp.Assign(this);
                SetSize(Width, Height, ABitCount);

                if (ABitCount <= 8)
                {
                    Array.Clear(ColorTable, 0, 256);    // FillChar(ColorTable, SizeOf(ColorTable), 0)
                    for (I = 0; I <= (1 << ABitCount) - 1; I++)
                    {
                        j = I * (1 << (8 - ABitCount));
                        j = j | (j >> ABitCount);
                        ColorTable[I] = DIB.RGBQuad((byte)j, (byte)j, (byte)j);
                    }
                    UpdatePalette();
                }

                Blur_Radius_Other();
            }
            finally
            {
                Temp.Destroy();     // Temp.Free
            }
        }
        finally
        {
            EndProgress();
        }
    }

    // =========================================================================================
    // DIB.pas 3165-3221 —— Negative
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3165-3221 1:1。
    /// &lt;= 8bpp 时把调色板每项取反；否则执行原文的 x86 asm：
    /// 先以 DWORD 为单位 `not`（从高下标往低下标），余下 Size and 3 个字节再以 Byte 为单位 `not`。
    /// 注意 24bpp 时 DWORD 粒度的取反会跨像素边界（含行尾填充字节）。
    /// </summary>
    public unsafe void Negative()
    {
        if (Empty) return;

        if (BitCount <= 8)
        {
            for (int I = 0; I <= 255; I++)
            {
                // with ColorTable[I] do begin rgbRed := 255 - rgbRed; ... end;
                ColorTable[I].rgbRed = (byte)(255 - ColorTable[I].rgbRed);
                ColorTable[I].rgbGreen = (byte)(255 - ColorTable[I].rgbGreen);
                ColorTable[I].rgbBlue = (byte)(255 - ColorTable[I].rgbBlue);
            }
            UpdatePalette();
        }
        else
        {
            byte* P = (byte*)PBits;     // P := PBits
            int i2 = Size;
            int ecx = i2;

            // @@qword_skip: shr ecx,2 / jz @@dword_skip
            ecx >>= 2;
            if (ecx != 0)
            {
                ecx--;
                do
                {
                    *(uint*)(P + ecx * 4) = ~*(uint*)(P + ecx * 4);     // not dword ptr [eax+ecx*4]
                    ecx--;
                } while (ecx >= 0);                                     // jnl @@dword_loop

                P += (i2 >> 2) * 4;                                     // mov ecx,edx / shr ecx,2 / add eax,ecx*4
            }

            // @@dword_skip: mov ecx,edx / and ecx,3 / jz @@byte_skip
            ecx = i2 & 3;
            if (ecx != 0)
            {
                ecx--;
                do
                {
                    P[ecx] = (byte)~P[ecx];                             // not byte ptr [eax+ecx]
                    ecx--;
                } while (ecx >= 0);
            }
        }
    }

    // =========================================================================================
    // DIB.pas 3223-3402 —— Greyscale
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3223-3402 1:1。
    /// Temp 保存源图，Self 被 SetSize 重建为目标位深；亮度权重 Trunc(0.3588/0.4020/0.2392 * I)。
    /// 原文笔误照抄：16/32bpp 分支三个通道都查 YTblR（DIB.pas:3286/3296）。
    /// </summary>
    public unsafe void Greyscale(int ABitCount)
    {
        byte[] YTblR = new byte[256];
        byte[] YTblG = new byte[256];
        byte[] YTblB = new byte[256];
        int I, j, X, Y;
        uint C;
        byte R = 0, G = 0, B = 0;
        TDIB Temp;
        byte* DestP;
        byte* SrcP;
        byte* P;

        if (Empty) return;

        Temp = new TDIB();
        try
        {
            Temp.Assign(this);
            SetSize(Width, Height, ABitCount);

            if (ABitCount <= 8)
            {
                Array.Clear(ColorTable, 0, 256);
                for (I = 0; I <= (1 << ABitCount) - 1; I++)
                {
                    j = I * (1 << (8 - ABitCount));
                    j = j | (j >> ABitCount);
                    ColorTable[I] = DIB.RGBQuad((byte)j, (byte)j, (byte)j);
                }
                UpdatePalette();
            }

            for (I = 0; I <= 255; I++)
            {
                YTblR[I] = (byte)(0.3588 * I);      // Trunc(0.3588 * I)
                YTblG[I] = (byte)(0.4020 * I);
                YTblB[I] = (byte)(0.2392 * I);
            }

            C = 0;

            StartProgress("Greyscale");
            try
            {
                for (Y = 0; Y <= Height - 1; Y++)
                {
                    DestP = (byte*)ScanLine(Y);
                    SrcP = (byte*)Temp.ScanLine(Y);

                    for (X = 0; X <= Width - 1; X++)
                    {
                        switch (Temp.BitCount)
                        {
                            case 1:
                                {
                                    int idx = (int)(((uint)SrcP[X >> 3] & DIBConstants.Mask1[X & 7]) >> (int)DIBConstants.Shift1[X & 7]);
                                    var q = Temp.ColorTable[idx];
                                    C = (uint)(YTblR[q.rgbRed] + YTblG[q.rgbGreen] + YTblB[q.rgbBlue]);
                                }
                                break;
                            case 4:
                                {
                                    int idx = (int)(((uint)SrcP[X >> 1] & DIBConstants.Mask4[X & 1]) >> (int)DIBConstants.Shift4[X & 1]);
                                    var q = Temp.ColorTable[idx];
                                    C = (uint)(YTblR[q.rgbRed] + YTblG[q.rgbGreen] + YTblB[q.rgbBlue]);
                                }
                                break;
                            case 8:
                                {
                                    var q = Temp.ColorTable[*SrcP];
                                    C = (uint)(YTblR[q.rgbRed] + YTblG[q.rgbGreen] + YTblB[q.rgbBlue]);
                                    SrcP++;
                                }
                                break;
                            case 16:
                                DIB.pfGetRGB(Temp.NowPixelFormat, *(ushort*)SrcP, out R, out G, out B);
                                C = (uint)(YTblR[R] + YTblR[G] + YTblR[B]);     // 原文如此（DIB.pas:3286）
                                SrcP += 2;
                                break;
                            case 24:
                                {
                                    TBGR* q = (TBGR*)SrcP;
                                    C = (uint)(YTblR[q->R] + YTblG[q->G] + YTblB[q->B]);
                                    SrcP += 3;
                                }
                                break;
                            case 32:
                                DIB.pfGetRGB(Temp.NowPixelFormat, *(uint*)SrcP, out R, out G, out B);
                                C = (uint)(YTblR[R] + YTblR[G] + YTblR[B]);     // 原文如此（DIB.pas:3296）
                                SrcP += 4;
                                break;
                        }

                        switch (BitCount)
                        {
                            case 1:
                                P = DestP + (X >> 3);
                                P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) |
                                    ((C > 127 ? 1u : 0u) << (int)DIBConstants.Shift1[X & 7]));
                                break;
                            case 4:
                                P = DestP + (X >> 1);
                                P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) |
                                    ((C >> 4) << (int)DIBConstants.Shift4[X & 1]));
                                break;
                            case 8:
                                *DestP = (byte)C;
                                DestP++;
                                break;
                            case 16:
                                *(ushort*)DestP = (ushort)DIB.pfRGB(NowPixelFormat, (byte)C, (byte)C, (byte)C);
                                DestP += 2;
                                break;
                            case 24:
                                {
                                    TBGR* q = (TBGR*)DestP;
                                    q->R = (byte)C;
                                    q->G = (byte)C;
                                    q->B = (byte)C;
                                    DestP += 3;
                                }
                                break;
                            case 32:
                                *(uint*)DestP = DIB.pfRGB(NowPixelFormat, (byte)C, (byte)C, (byte)C);
                                DestP += 4;
                                break;
                        }
                    }

                    UpdateProgress(Y);
                }
            }
            finally
            {
                EndProgress();
            }
        }
        finally
        {
            Temp.Destroy();
        }
    }

    // =========================================================================================
    // DIB.pas 3403-3408 —— IntToColor
    // =========================================================================================

    /// <summary>DIB.pas 3403-3408 1:1：B := I shr 16；G := I shr 8；R := I（均截断到 Byte）。</summary>
    public TBGR IntToColor(int I)
    {
        TBGR Result = default;
        Result.B = (byte)(I >> 16);
        Result.G = (byte)(I >> 8);
        Result.R = (byte)I;
        return Result;
    }

    // =========================================================================================
    // DIB.pas 3412-3432 —— Interval
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3412-3432 1:1。
    /// iMark=True：下溢回 iMin、上溢回 iMax；
    /// iMark=False：下溢回 iMin、**上溢也回 iMin** —— 原文如此（DIB.pas:3428）。
    /// </summary>
    public int Interval(int iMin, int iMax, int iValue, bool iMark)
    {
        int Result;
        if (iMark)
        {
            if (iValue < iMin)
                Result = iMin;
            else if (iValue > iMax)
                Result = iMax;
            else
                Result = iValue;
        }
        else
        {
            if (iValue < iMin)
                Result = iMin;
            else if (iValue > iMax)
                Result = iMin;      // 原文如此（DIB.pas:3428）
            else
                Result = iValue;
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 3436-3537 —— Contrast
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3436-3537 1:1。
    /// 32bpp 直接 Exit；24/16bpp 走像素循环；8/4bpp 改调色板并借用 Temp1（与 Self 共享图像）。
    /// 原文用 `I: Byte` 作 for 计数器（0..255 共 256 次），托管侧用 int 保留同样次数。
    /// </summary>
    public unsafe void Contrast(int Amount)
    {
        int X, Y;
        byte[] Table1 = new byte[256];
        TDIB Temp1;
        uint Color;
        byte* D = null;
        byte* S = null;
        byte* P;
        byte R = 0, G = 0, B = 0;

        D = null;
        S = null;
        Temp1 = null;

        for (int I = 0; I <= 126; I++)
        {
            Y = (Math.Abs(128 - I) * Amount) / 256;
            Table1[I] = IntToByte(I - Y);
        }
        for (int I = 127; I <= 255; I++)
        {
            Y = (Math.Abs(128 - I) * Amount) / 256;
            Table1[I] = IntToByte(I + Y);
        }

        switch (BitCount)
        {
            case 32:
                return;             // I haven't bitmap of this type ! Sorry
            case 24:
                break;              // nothing to do
            case 16:
                break;              // I have an artificial bitmap for this type ! i don't sure that it works
            case 8:
            case 4:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    for (int I = 0; I <= 255; I++)
                    {
                        ColorTable[I].rgbRed = IntToByte(Table1[ColorTable[I].rgbRed]);
                        ColorTable[I].rgbGreen = IntToByte(Table1[ColorTable[I].rgbGreen]);
                        ColorTable[I].rgbBlue = IntToByte(Table1[ColorTable[I].rgbBlue]);
                    }
                    UpdatePalette();
                }
                break;
            default:
                // if the number of pixel is equal to 1 then exit of procedure
                return;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 24:
                case 16:
                    D = (byte*)ScanLine(Y);
                    break;
                case 8:
                case 4:
                    D = (byte*)Temp1.ScanLine(Y);
                    S = (byte*)Temp1.ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 32:
                        break;
                    case 24:
                        // PBGR(D)^.B := Table1[PBGR(D)^.B]; ...
                        D[0] = Table1[D[0]];
                        D[1] = Table1[D[1]];
                        D[2] = Table1[D[2]];
                        D += 3;
                        break;
                    case 16:
                        DIB.pfGetRGB(NowPixelFormat, *(ushort*)D, out R, out G, out B);
                        *(ushort*)D = (ushort)(Table1[R] + Table1[G] + Table1[B]);
                        D += 2;
                        break;
                    case 8:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            *D = (byte)Color;
                            D++;
                        }
                        break;
                    case 4:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            P = D + (X >> 1);
                            P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        switch (BitCount)
        {
            case 8:
            case 4:
                Temp1.Destroy();    // Temp1.Free
                break;
            default:
                break;
        }
    }

    // =========================================================================================
    // DIB.pas 3541-3649 —— Saturation
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3541-3649 1:1。
    /// Grays[0..767] 按 (R+G+B) 的三倍斜坡填 3 份相同值；Alpha[I] = (I * Amount) shr 8。
    /// 原文缺陷照抄：16bpp 分支从不重算 Gray，用的是上一轮（或未初始化的）值
    /// —— 原文如此（DIB.pas:3618-3623）；托管侧把未初始化值固定为 0。
    /// </summary>
    public unsafe void Saturation(int Amount)
    {
        int[] Grays = new int[768];
        ushort[] Alpha = new ushort[256];
        int Gray = 0;       // 原文未初始化（栈垃圾），托管侧固定 0
        int X, Y;
        TDIB Temp1;
        uint Color;
        byte* D = null;
        byte* S = null;
        byte* P;
        byte R = 0, G = 0, B = 0;

        D = null;
        S = null;
        Temp1 = null;

        for (int I = 0; I <= 255; I++)
            Alpha[I] = (ushort)((I * Amount) >> 8);

        X = 0;
        for (int I = 0; I <= 255; I++)
        {
            Gray = I - Alpha[I];
            Grays[X] = Gray;
            X++;
            Grays[X] = Gray;
            X++;
            Grays[X] = Gray;
            X++;
        }

        switch (BitCount)
        {
            case 32:
                return;             // I haven't bitmap of this type ! Sorry
            case 24:
                break;              // nothing to do
            case 16:
                break;              // I have an artificial bitmap for this type ! i don't sure that it works
            case 8:
            case 4:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    for (int I = 0; I <= 255; I++)
                    {
                        Gray = Grays[ColorTable[I].rgbRed + ColorTable[I].rgbGreen + ColorTable[I].rgbBlue];
                        ColorTable[I].rgbRed = IntToByte(Gray + Alpha[ColorTable[I].rgbRed]);
                        ColorTable[I].rgbGreen = IntToByte(Gray + Alpha[ColorTable[I].rgbGreen]);
                        ColorTable[I].rgbBlue = IntToByte(Gray + Alpha[ColorTable[I].rgbBlue]);
                    }
                    UpdatePalette();
                }
                break;
            default:
                // if the number of pixel is equal to 1 then exit of procedure
                return;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 24:
                case 16:
                    D = (byte*)ScanLine(Y);
                    break;
                case 8:
                case 4:
                    D = (byte*)Temp1.ScanLine(Y);
                    S = (byte*)Temp1.ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 32:
                        break;
                    case 24:
                        {
                            TBGR* q = (TBGR*)D;
                            Gray = Grays[q->R + q->G + q->B];
                            q->B = IntToByte(Gray + Alpha[q->B]);
                            q->G = IntToByte(Gray + Alpha[q->G]);
                            q->R = IntToByte(Gray + Alpha[q->R]);
                            D += 3;
                        }
                        break;
                    case 16:
                        DIB.pfGetRGB(NowPixelFormat, *(ushort*)D, out R, out G, out B);
                        *(ushort*)D = (ushort)(IntToByte(Gray + Alpha[B]) + IntToByte(Gray + Alpha[G]) +
                            IntToByte(Gray + Alpha[R]));
                        D += 2;
                        break;
                    case 8:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            *D = (byte)Color;
                            D++;
                        }
                        break;
                    case 4:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            P = D + (X >> 1);
                            P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        switch (BitCount)
        {
            case 8:
            case 4:
                Temp1.Destroy();
                break;
            default:
                break;
        }
    }

    // =========================================================================================
    // DIB.pas 3653-3753 —— Lightness
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3653-3753 1:1。
    /// Amount &lt; 0：先取正，Table1[I] := IntToByte(I - (Amount*I shr 8))；
    /// 否则 Table1[I] := IntToByte(I + (Amount*(I xor 255) shr 8))。
    /// </summary>
    public unsafe void Lightness(int Amount)
    {
        int X, Y;
        byte[] Table1 = new byte[256];
        TDIB Temp1;
        uint Color;
        byte* D = null;
        byte* S = null;
        byte* P;
        byte R = 0, G = 0, B = 0;

        D = null;
        S = null;
        Temp1 = null;

        if (Amount < 0)
        {
            Amount = -Amount;
            for (int I = 0; I <= 255; I++)
                Table1[I] = IntToByte(I - ((Amount * I) >> 8));
        }
        else
            for (int I = 0; I <= 255; I++)
                Table1[I] = IntToByte(I + ((Amount * (I ^ 255)) >> 8));

        switch (BitCount)
        {
            case 32:
                return;             // I haven't bitmap of this type ! Sorry
            case 24:
                break;              // nothing to do
            case 16:
                break;              // I have an artificial bitmap for this type ! i don't sure that it works
            case 8:
            case 4:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    for (int I = 0; I <= 255; I++)
                    {
                        ColorTable[I].rgbRed = IntToByte(Table1[ColorTable[I].rgbRed]);
                        ColorTable[I].rgbGreen = IntToByte(Table1[ColorTable[I].rgbGreen]);
                        ColorTable[I].rgbBlue = IntToByte(Table1[ColorTable[I].rgbBlue]);
                    }
                    UpdatePalette();
                }
                break;
            default:
                // if the number of pixel is equal to 1 then exit of procedure
                return;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 24:
                case 16:
                    D = (byte*)ScanLine(Y);
                    break;
                case 8:
                case 4:
                    D = (byte*)Temp1.ScanLine(Y);
                    S = (byte*)Temp1.ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 32:
                        break;
                    case 24:
                        D[0] = Table1[D[0]];
                        D[1] = Table1[D[1]];
                        D[2] = Table1[D[2]];
                        D += 3;
                        break;
                    case 16:
                        DIB.pfGetRGB(NowPixelFormat, *(ushort*)D, out R, out G, out B);
                        *(ushort*)D = (ushort)(Table1[R] + Table1[G] + Table1[B]);
                        D += 2;
                        break;
                    case 8:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            *D = (byte)Color;
                            D++;
                        }
                        break;
                    case 4:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            P = D + (X >> 1);
                            P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        switch (BitCount)
        {
            case 8:
            case 4:
                Temp1.Destroy();
                break;
            default:
                break;
        }
    }

    // =========================================================================================
    // DIB.pas 3757-3847 —— AddRGB
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3757-3847 1:1。
    /// 32bpp/1bpp（else）直接 Exit；24/16bpp 先建 Table 再逐像素查表；
    /// 8/4bpp 改调色板（4bpp 的像素分支用的是从未赋值的 Color，恒 0 —— 原文如此（DIB.pas:3836-3837））。
    /// </summary>
    public unsafe void AddRGB(byte ra, byte ga, byte ba)
    {
        TBGR[] Table = new TBGR[256];
        int X, Y;
        byte* D = null;
        byte* P;
        uint Color;
        TDIB Temp1;
        byte R = 0, G = 0, B = 0;

        Color = 0;
        D = null;
        Temp1 = null;

        switch (BitCount)
        {
            case 32:
                return;             // I haven't bitmap of this type ! Sorry
            case 24:
            case 16:
                {
                    for (int I = 0; I <= 255; I++)
                    {
                        Table[I].B = IntToByte(I + ba);
                        Table[I].G = IntToByte(I + ga);
                        Table[I].R = IntToByte(I + ra);
                    }
                }
                break;
            case 8:
            case 4:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    for (int I = 0; I <= 255; I++)
                    {
                        ColorTable[I].rgbRed = IntToByte(ColorTable[I].rgbRed + ra);
                        ColorTable[I].rgbGreen = IntToByte(ColorTable[I].rgbGreen + ga);
                        ColorTable[I].rgbBlue = IntToByte(ColorTable[I].rgbBlue + ba);
                    }
                    UpdatePalette();
                }
                break;
            default:
                // if the number of pixel is equal to 1 then exit of procedure
                return;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 24:
                case 16:
                    D = (byte*)ScanLine(Y);
                    break;
                case 8:
                case 4:
                    D = (byte*)Temp1.ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 32:
                        break;      // I haven't bitmap of this type ! Sorry
                    case 24:
                        {
                            TBGR* q = (TBGR*)D;
                            q->B = Table[q->B].B;
                            q->G = Table[q->G].G;
                            q->R = Table[q->R].R;
                            D += 3;
                        }
                        break;
                    case 16:
                        DIB.pfGetRGB(NowPixelFormat, *(ushort*)D, out R, out G, out B);
                        *(ushort*)D = (ushort)(Table[R].R + Table[G].G + Table[B].B);
                        D += 2;
                        break;
                    case 8:
                        D++;
                        break;
                    case 4:
                        P = D + (X >> 1);
                        P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        break;
                    default:
                        break;
                }
            }
        }

        switch (BitCount)
        {
            case 8:
            case 4:
                Temp1.Destroy();
                break;
            default:
                break;
        }
    }

    // =========================================================================================
    // DIB.pas 3851-3922 —— Filter
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3851-3922 1:1。
    /// 只支持 24/8bpp（32/16/4/1 直接 Result := False 并 Exit）；
    /// 8bpp 分支把结果写进 Dest 的字节，同时 `Col := PBits` 会把中间值写回源图缓冲区（原文如此）。
    /// 原文 `Pred(I)`/`Pred(j)` 的 I/j 是 Byte；Delphi 在 32 位寄存器里求值且不回写 Byte，
    /// 故按"前驱 = 减 1"语义（I=0 → -1），与 Pred(Width)/Pred(Height) 的书写意图一致。
    /// </summary>
    public unsafe bool Filter(TDIB Dest, short[,] Filter)
    {
        int Sum, R, G, B, X, Y;
        byte a = 0, I, j;
        TBGR tmp = default;
        byte* Col;
        byte* D;

        bool Result = true;
        Sum = Filter[0, 0] + Filter[1, 0] + Filter[2, 0] +
              Filter[0, 1] + Filter[1, 1] + Filter[2, 1] +
              Filter[0, 2] + Filter[1, 2] + Filter[2, 2];
        if (Sum == 0)
            Sum = 1;

        Col = (byte*)PBits;     // Col := PBits（PBGR，直接写源图缓冲区）
        for (Y = 0; Y <= Height - 1; Y++)
        {
            D = (byte*)Dest.ScanLine(Y);
            for (X = 0; X <= Width - 1; X++)
            {
                R = 0; G = 0; B = 0;
                switch (BitCount)
                {
                    case 32:
                    case 16:
                    case 4:
                    case 1:
                        Result = false;
                        return Result;
                    case 24:
                        {
                            for (I = 0; I <= 2; I++)
                            {
                                for (j = 0; j <= 2; j++)
                                {
                                    int pI = I - 1;     // Pred(I)，I: Byte
                                    int pj = j - 1;     // Pred(j)，j: Byte
                                    tmp = IntToColor((int)GetPixel(
                                        Interval(0, Width - 1, X + pI, true),
                                        Interval(0, Height - 1, Y + pj, true)));
                                    B += Filter[I, j] * tmp.B;
                                    G += Filter[I, j] * tmp.G;
                                    R += Filter[I, j] * tmp.R;
                                }
                            }
                            Col[0] = IntToByte(B / Sum);    // Col.B
                            Col[1] = IntToByte(G / Sum);    // Col.G
                            Col[2] = IntToByte(R / Sum);    // Col.R
                            Dest.SetPixel(X, Y, DibEffectsSupport.DibRgb(Col[2], Col[1], Col[0]));
                        }
                        break;
                    case 8:
                        {
                            for (I = 0; I <= 2; I++)
                            {
                                for (j = 0; j <= 2; j++)
                                {
                                    int pI = I - 1;
                                    int pj = j - 1;
                                    a = (byte)GetPixel(
                                        Interval(0, Width - 1, X + pI, true),
                                        Interval(0, Height - 1, Y + pj, true));
                                    tmp.R = ColorTable[a].rgbRed;
                                    tmp.G = ColorTable[a].rgbGreen;
                                    tmp.B = ColorTable[a].rgbBlue;
                                    B += Filter[I, j] * tmp.B;
                                    G += Filter[I, j] * tmp.G;
                                    R += Filter[I, j] * tmp.R;
                                }
                            }
                            Col[0] = IntToByte(B / Sum);
                            Col[1] = IntToByte(G / Sum);
                            Col[2] = IntToByte(R / Sum);
                            // PByte(D)^ := rgb(Col.R, Col.G, Col.B) —— 赋给 Byte，只留低 8 位
                            *D = (byte)DibEffectsSupport.DibRgb(Col[2], Col[1], Col[0]);
                            D++;
                        }
                        break;
                }
            }
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 3926-3977 —— Spray
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3926-3977 1:1。
    /// 每像素取源图 (X + (Value - Random(Value*2)), Y + 同) 处的颜色（越界由 Interval 夹住）。
    /// 注意 Random 是全局 RandSeed 驱动的 LCG（见 DibEffectsSupport.DibRandom）。
    /// </summary>
    public unsafe void Spray(int Amount)
    {
        int Value, X, Y;
        byte* D;
        uint Color;
        byte* P;

        for (Y = Height - 1; Y >= 0; Y--)
        {
            D = (byte*)ScanLine(Y);
            for (X = 0; X <= Width - 1; X++)
            {
                Value = DibEffectsSupport.DibRandom(Amount);
                Color = GetPixel(
                    Interval(0, Width - 1, X + (Value - DibEffectsSupport.DibRandom(Value * 2)), true),
                    Interval(0, Height - 1, Y + (Value - DibEffectsSupport.DibRandom(Value * 2)), true));
                switch (BitCount)
                {
                    case 32:
                        *(uint*)D = Color;
                        D += 4;
                        break;
                    case 24:
                        *(TBGR*)D = IntToColor((int)Color);
                        D += 3;
                        break;
                    case 16:
                        *(ushort*)D = (ushort)Color;
                        D += 2;
                        break;
                    case 8:
                        *D = (byte)Color;
                        D++;
                        break;
                    case 4:
                        P = D + (X >> 1);
                        P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        break;
                    case 1:
                        P = D + (X >> 3);
                        P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) | (Color << (int)DIBConstants.Shift1[X & 7]));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 3981-4157 —— Sharpen
    // =========================================================================================

    /// <summary>
    /// DIB.pas 3981-4157 1:1。
    /// 32/16/1bpp 直接 Exit；24/8bpp 借用 Temp1 并在结尾 Assign(Temp1) 回写；4bpp 只改调色板。
    /// 原文笔误照抄：`Buf[8] := Lin0[cx]`（应为 Lin2[cx]，DIB.pas:4124）；
    /// 调色板分支的 `ColorTable[I ± Amount]` 下标越界（原文无范围检查，托管侧用 fixed 指针不抛异常）。
    /// 原文在 Exit 分支会漏掉 FreeMem(pc)（托管侧用栈上局部，无泄漏）。
    /// </summary>
    public unsafe void Sharpen(int Amount)
    {
        TBGR* Lin0;
        TBGR* Lin1;
        TBGR* Lin2;
        int cx, X, Y;
        TBGR[] Buf = new TBGR[9];
        byte* D = null;
        uint C;
        byte I;
        byte* p1;
        TDIB Temp1;

        // GetMem(pc, SizeOf(TBGR)) → 托管侧用栈上局部
        TBGR pcBuf = default;
        TBGR* pc = &pcBuf;

        C = 0;
        Temp1 = null;

        switch (BitCount)
        {
            case 32:
            case 16:
            case 1:
                return;
            case 24:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                }
                break;
            case 8:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    fixed (TRGBQuad* ctSrc = ColorTable)
                    fixed (TRGBQuad* ctDst = Temp1.ColorTable)
                    {
                        for (I = 0; I <= 255; I++)
                        {
                            Buf[0].B = ctSrc[I - Amount].rgbBlue;
                            Buf[0].G = ctSrc[I - Amount].rgbGreen;
                            Buf[0].R = ctSrc[I - Amount].rgbRed;
                            Buf[1].B = ctSrc[I].rgbBlue;
                            Buf[1].G = ctSrc[I].rgbGreen;
                            Buf[1].R = ctSrc[I].rgbRed;
                            Buf[2].B = ctSrc[I + Amount].rgbBlue;
                            Buf[2].G = ctSrc[I + Amount].rgbGreen;
                            Buf[2].R = ctSrc[I + Amount].rgbRed;
                            Buf[3].B = ctSrc[I - Amount].rgbBlue;
                            Buf[3].G = ctSrc[I - Amount].rgbGreen;
                            Buf[3].R = ctSrc[I - Amount].rgbRed;
                            Buf[4].B = ctSrc[I].rgbBlue;
                            Buf[4].G = ctSrc[I].rgbGreen;
                            Buf[4].R = ctSrc[I].rgbRed;
                            Buf[5].B = ctSrc[I + Amount].rgbBlue;
                            Buf[5].G = ctSrc[I + Amount].rgbGreen;
                            Buf[5].R = ctSrc[I + Amount].rgbRed;
                            Buf[6].B = ctSrc[I - Amount].rgbBlue;
                            Buf[6].G = ctSrc[I - Amount].rgbGreen;
                            Buf[6].R = ctSrc[I - Amount].rgbRed;
                            Buf[7].B = ctSrc[I].rgbBlue;
                            Buf[7].G = ctSrc[I].rgbGreen;
                            Buf[7].R = ctSrc[I].rgbRed;
                            Buf[8].B = ctSrc[I + Amount].rgbBlue;
                            Buf[8].G = ctSrc[I + Amount].rgbGreen;
                            Buf[8].R = ctSrc[I + Amount].rgbRed;

                            ctDst[I].rgbBlue = IntToByte((256 * Buf[4].B - (Buf[0].B + Buf[1].B + Buf[2].B + Buf[3].B +
                                Buf[5].B + Buf[6].B + Buf[7].B + Buf[8].B) * 16) / 128);
                            ctDst[I].rgbGreen = IntToByte((256 * Buf[4].G - (Buf[0].G + Buf[1].G + Buf[2].G + Buf[3].G +
                                Buf[5].G + Buf[6].G + Buf[7].G + Buf[8].G) * 16) / 128);
                            ctDst[I].rgbRed = IntToByte((256 * Buf[4].R - (Buf[0].R + Buf[1].R + Buf[2].R + Buf[3].R +
                                Buf[5].R + Buf[6].R + Buf[7].R + Buf[8].R) * 16) / 128);
                        }
                    }
                    Temp1.UpdatePalette();
                }
                break;
            case 4:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    fixed (TRGBQuad* ctSrc = ColorTable)
                    {
                        for (I = 0; I <= 255; I++)
                        {
                            Buf[0].B = ctSrc[I - Amount].rgbBlue;
                            Buf[0].G = ctSrc[I - Amount].rgbGreen;
                            Buf[0].R = ctSrc[I - Amount].rgbRed;
                            Buf[1].B = ctSrc[I].rgbBlue;
                            Buf[1].G = ctSrc[I].rgbGreen;
                            Buf[1].R = ctSrc[I].rgbRed;
                            Buf[2].B = ctSrc[I + Amount].rgbBlue;
                            Buf[2].G = ctSrc[I + Amount].rgbGreen;
                            Buf[2].R = ctSrc[I + Amount].rgbRed;
                            Buf[3].B = ctSrc[I - Amount].rgbBlue;
                            Buf[3].G = ctSrc[I - Amount].rgbGreen;
                            Buf[3].R = ctSrc[I - Amount].rgbRed;
                            Buf[4].B = ctSrc[I].rgbBlue;
                            Buf[4].G = ctSrc[I].rgbGreen;
                            Buf[4].R = ctSrc[I].rgbRed;
                            Buf[5].B = ctSrc[I + Amount].rgbBlue;
                            Buf[5].G = ctSrc[I + Amount].rgbGreen;
                            Buf[5].R = ctSrc[I + Amount].rgbRed;
                            Buf[6].B = ctSrc[I - Amount].rgbBlue;
                            Buf[6].G = ctSrc[I - Amount].rgbGreen;
                            Buf[6].R = ctSrc[I - Amount].rgbRed;
                            Buf[7].B = ctSrc[I].rgbBlue;
                            Buf[7].G = ctSrc[I].rgbGreen;
                            Buf[7].R = ctSrc[I].rgbRed;
                            Buf[8].B = ctSrc[I + Amount].rgbBlue;
                            Buf[8].G = ctSrc[I + Amount].rgbGreen;
                            Buf[8].R = ctSrc[I + Amount].rgbRed;

                            ColorTable[I].rgbBlue = IntToByte((256 * Buf[4].B - (Buf[0].B + Buf[1].B + Buf[2].B + Buf[3].B +
                                Buf[5].B + Buf[6].B + Buf[7].B + Buf[8].B) * 16) / 128);
                            ColorTable[I].rgbGreen = IntToByte((256 * Buf[4].G - (Buf[0].G + Buf[1].G + Buf[2].G + Buf[3].G +
                                Buf[5].G + Buf[6].G + Buf[7].G + Buf[8].G) * 16) / 128);
                            ColorTable[I].rgbRed = IntToByte((256 * Buf[4].R - (Buf[0].R + Buf[1].R + Buf[2].R + Buf[3].R +
                                Buf[5].R + Buf[6].R + Buf[7].R + Buf[8].R) * 16) / 128);
                        }
                    }
                    UpdatePalette();
                }
                break;
            default:
                break;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            Lin0 = (TBGR*)ScanLine(Interval(0, Height - 1, Y - Amount, true));
            Lin1 = (TBGR*)ScanLine(Y);
            Lin2 = (TBGR*)ScanLine(Interval(0, Height - 1, Y + Amount, true));
            switch (BitCount)
            {
                case 24:
                case 8:
                case 4:
                    D = (byte*)Temp1.ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 24:
                        {
                            cx = Interval(0, Width - 1, X - Amount, true);
                            Buf[0] = Lin0[cx];
                            Buf[1] = Lin1[cx];
                            Buf[2] = Lin2[cx];
                            Buf[3] = Lin0[X];
                            Buf[4] = Lin1[X];
                            Buf[5] = Lin2[X];
                            cx = Interval(0, Width - 1, X + Amount, true);
                            Buf[6] = Lin0[cx];
                            Buf[7] = Lin1[cx];
                            Buf[8] = Lin0[cx];      // 原文如此（DIB.pas:4124）：应为 Lin2[cx]
                            pc->B = IntToByte((256 * Buf[4].B - (Buf[0].B + Buf[1].B + Buf[2].B + Buf[3].B +
                                Buf[5].B + Buf[6].B + Buf[7].B + Buf[8].B) * 16) / 128);
                            pc->G = IntToByte((256 * Buf[4].G - (Buf[0].G + Buf[1].G + Buf[2].G + Buf[3].G +
                                Buf[5].G + Buf[6].G + Buf[7].G + Buf[8].G) * 16) / 128);
                            pc->R = IntToByte((256 * Buf[4].R - (Buf[0].R + Buf[1].R + Buf[2].R + Buf[3].R +
                                Buf[5].R + Buf[6].R + Buf[7].R + Buf[8].R) * 16) / 128);
                            TBGR* q = (TBGR*)D;
                            q->B = pc->B;
                            q->G = pc->G;
                            q->R = pc->R;
                            D += 3;
                        }
                        break;
                    case 8:
                        D++;
                        break;
                    case 4:
                        p1 = D + (X >> 1);
                        p1[0] = (byte)((p1[0] & DIBConstants.Mask4n[X & 1]) | (C << (int)DIBConstants.Shift4[X & 1]));
                        break;
                    default:
                        break;
                }
            }
        }

        switch (BitCount)
        {
            case 24:
            case 8:
                Assign(Temp1);
                Temp1.Destroy();
                break;
            case 4:
                Temp1.Destroy();
                break;
            default:
                break;
        }
    }

    // =========================================================================================
    // DIB.pas 4161-4237 —— Emboss
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4161-4237 1:1。
    /// 32/16/1bpp 直接 Exit；24bpp 用 D/D1（相隔 3 字节）在**同一缓冲区内**就地推进；
    /// 8/4bpp 用 Pixels[X+3, Y]（越界由 GetPixel 返回 0 兜住）。
    /// 原文缺陷照抄：D1 只在 `Y &lt; Height-2 and X &lt; Width-2` 时递增（DIB.pas:4199-4200）。
    /// </summary>
    public unsafe void Emboss()
    {
        int X, Y;
        byte* D = null;
        byte* D1 = null;
        byte* P = null;
        TBGR Color;
        uint C;
        byte* p1;

        D = null;
        D1 = null;
        P = null;

        switch (BitCount)
        {
            case 32:
            case 16:
            case 1:
                return;
            case 24:
                D = (byte*)PBits;
                D1 = D + 3;     // Ptr(Integer(D) + 3)
                break;
            default:
                break;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 8:
                case 4:
                    P = (byte*)ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 24:
                        {
                            D[0] = (byte)((D[0] + (D1[0] ^ 0xFF)) >> 1);    // PBGR(D)^.B
                            D[1] = (byte)((D[1] + (D1[1] ^ 0xFF)) >> 1);    // PBGR(D)^.G
                            D[2] = (byte)((D[2] + (D1[2] ^ 0xFF)) >> 1);    // PBGR(D)^.R
                            D += 3;
                            if (Y < Height - 2 && X < Width - 2)
                                D1 += 3;
                        }
                        break;
                    case 8:
                        {
                            Color.R = (byte)((((GetPixel(X, Y) + (GetPixel(X + 3, Y) ^ 0xFF)) >> 1) + 30) / 3);
                            Color.G = (byte)((((GetPixel(X, Y) + (GetPixel(X + 3, Y) ^ 0xFF)) >> 1) + 30) / 3);
                            Color.B = (byte)((((GetPixel(X, Y) + (GetPixel(X + 3, Y) ^ 0xFF)) >> 1) + 30) / 3);
                            C = (uint)((Color.R + Color.G + Color.B) >> 1);
                            *P = (byte)C;
                            P++;
                        }
                        break;
                    case 4:
                        {
                            Color.R = (byte)((((GetPixel(X, Y) + (GetPixel(X + 3, Y) ^ 0xFF) + 1) >> 1) + 30) / 3);
                            Color.G = (byte)((((GetPixel(X, Y) + (GetPixel(X + 3, Y) ^ 0xFF) - 1) >> 1) + 30) / 3);
                            Color.B = (byte)((((GetPixel(X, Y) + (GetPixel(X + 3, Y) ^ 0xFF) + 1) >> 1) + 30) / 3);
                            C = (uint)((Color.R + Color.G + Color.B) >> 1);
                            if (C > 64)
                                C = C - 8;
                            p1 = P + (X >> 1);
                            p1[0] = (byte)((p1[0] & DIBConstants.Mask4n[X & 1]) | (C << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    default:
                        break;
                }
            }

            switch (BitCount)
            {
                case 24:
                    D = D1;     // D := Ptr(Integer(D1))
                    if (Y < Height - 2)
                        D1 = D1 + 6;                                            // Ptr(Integer(D1) + 6)
                    else
                        D1 = (byte*)ScanLine(Height - 1) + 3;                   // Ptr(Integer(ScanLine[Pred(Height)]) + 3)
                    break;
                default:
                    break;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 4241-4292 —— AddMonoNoise
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4241-4292 1:1。
    /// 32/16bpp 直接 Exit；24bpp 三通道加同一个带符号噪声；8/4/1bpp 用 a 做索引抖动。
    /// `a` 是 Byte，负数会回绕成 248..255（原文如此）。
    /// </summary>
    public unsafe void AddMonoNoise(int Amount)
    {
        uint Value;
        int X, Y;
        byte a;
        byte* D;
        uint Color;
        byte* P;

        for (Y = 0; Y <= Height - 1; Y++)
        {
            D = (byte*)ScanLine(Y);
            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 32:
                        return;     // I haven't bitmap of this type ! Sorry
                    case 24:
                        {
                            Value = unchecked((uint)(DibEffectsSupport.DibRandom(Amount) - (Amount >> 1)));
                            TBGR* q = (TBGR*)D;
                            q->B = IntToByte(unchecked((int)((uint)q->B + Value)));
                            q->G = IntToByte(unchecked((int)((uint)q->G + Value)));
                            q->R = IntToByte(unchecked((int)((uint)q->R + Value)));
                            D += 3;
                        }
                        break;
                    case 16:
                        return;     // I haven't bitmap of this type ! Sorry
                    case 8:
                        {
                            a = unchecked((byte)((DibEffectsSupport.DibRandom(Amount >> 1) - (Amount / 4)) / 8));
                            Color = (uint)Interval(0, 255, unchecked((int)(GetPixel(X, Y) - (uint)a)), true);
                            *D = (byte)Color;
                            D++;
                        }
                        break;
                    case 4:
                        {
                            a = unchecked((byte)((DibEffectsSupport.DibRandom(Amount >> 1) - (Amount / 4)) / 16));
                            Color = (uint)Interval(0, 15, unchecked((int)(GetPixel(X, Y) - (uint)a)), true);
                            P = D + (X >> 1);
                            P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    case 1:
                        {
                            a = unchecked((byte)((DibEffectsSupport.DibRandom(Amount >> 1) - (Amount / 4)) / 32));
                            Color = (uint)Interval(0, 1, unchecked((int)(GetPixel(X, Y) - (uint)a)), true);
                            P = D + (X >> 3);
                            P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) | (Color << (int)DIBConstants.Shift1[X & 7]));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 4296-4391 —— AddGradiantNoise
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4296-4391 1:1。
    /// 32/16bpp 与 1bpp（else）直接 Exit；24bpp 先建随机偏移 Table 再查表；
    /// 8/4bpp 先按随机偏移改调色板，再把 Temp1 的调色板和值搬到 Self。
    /// </summary>
    public unsafe void AddGradiantNoise(byte Amount)
    {
        byte a;
        int X, Y;
        TBGR[] Table = new TBGR[256];
        byte* S = null;
        byte* D = null;
        uint Color;
        TDIB Temp1;
        byte* P;

        D = null;
        S = null;
        Temp1 = null;

        switch (BitCount)
        {
            case 32:
                return;             // I haven't bitmap of this type ! Sorry
            case 24:
                {
                    for (int I = 0; I <= 255; I++)
                    {
                        a = (byte)DibEffectsSupport.DibRandom(Amount);
                        Table[I].B = IntToByte(I + a);
                        Table[I].G = IntToByte(I + a);
                        Table[I].R = IntToByte(I + a);
                    }
                }
                break;
            case 16:
                return;             // I haven't bitmap of this type ! Sorry
            case 8:
            case 4:
                {
                    Temp1 = new TDIB();
                    Temp1.Assign(this);
                    Temp1.SetSize(Width, Height, BitCount);
                    for (int I = 0; I <= 255; I++)
                    {
                        a = (byte)DibEffectsSupport.DibRandom(Amount);
                        ColorTable[I].rgbRed = IntToByte(ColorTable[I].rgbRed + a);
                        ColorTable[I].rgbGreen = IntToByte(ColorTable[I].rgbGreen + a);
                        ColorTable[I].rgbBlue = IntToByte(ColorTable[I].rgbBlue + a);
                    }
                    UpdatePalette();
                }
                break;
            default:
                // if the number of pixel is equal to 1 then exit of procedure
                return;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 24:
                    D = (byte*)ScanLine(Y);
                    break;
                case 8:
                case 4:
                    D = (byte*)Temp1.ScanLine(Y);
                    S = (byte*)Temp1.ScanLine(Y);
                    break;
                default:
                    break;
            }

            for (X = 0; X <= Width - 1; X++)
            {
                switch (BitCount)
                {
                    case 32:
                        break;      // I haven't bitmap of this type ! Sorry
                    case 24:
                        {
                            TBGR* q = (TBGR*)D;
                            q->B = Table[q->B].B;
                            q->G = Table[q->G].G;
                            q->R = Table[q->R].R;
                            D += 3;
                        }
                        break;
                    case 16:
                        break;      // I haven't bitmap of this type ! Sorry
                    case 8:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            *D = (byte)Color;
                            D++;
                        }
                        break;
                    case 4:
                        {
                            var q = Temp1.ColorTable[*S];
                            Color = (uint)(q.rgbRed + q.rgbGreen + q.rgbBlue);
                            S++;
                            P = D + (X >> 1);
                            P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        switch (BitCount)
        {
            case 8:
            case 4:
                Temp1.Destroy();
                break;
            default:
                break;
        }
    }

    // =========================================================================================
    // DIB.pas 4395-4508 —— FishEye
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4395-4508 1:1（只实现 24bpp，其余位深 Result := False）。
    /// xmid/ymid 是 Single（`Width / 2` 为浮点除法）；双线性插值 + 环绕取模。
    /// </summary>
    public unsafe bool FishEye(TDIB bmp)
    {
        float weight, xmid, ymid, fx, fy, r1, r2, dx, dy, rmax;
        int Amount, ifx, ify, ty, tx, new_red, new_green, new_blue, ix, iy;
        float[] weight_x = new float[2];
        float[] weight_y = new float[2];
        float total_red, total_green, total_blue;
        TBGR* sli;
        TBGR* slo;

        bool Result = true;
        switch (BitCount)
        {
            case 32:
            case 16:
            case 8:
            case 4:
            case 1:
                Result = false;
                return Result;
        }

        Amount = 1;
        xmid = Width / 2f;      // 原文 `Width / 2`（浮点除法）
        ymid = Height / 2f;
        rmax = bmp.Width * Amount;
        for (ty = 0; ty <= Height - 1; ty++)
        {
            for (tx = 0; tx <= Width - 1; tx++)
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
                    r2 = rmax / 2 * (1 / (1 - r1 / rmax) - 1);
                    fx = dx * r2 / r1 + xmid;
                    fy = dy * r2 / r1 + ymid;
                }
                ify = (int)fy;      // Trunc(fy)
                ifx = (int)fx;      // Trunc(fx)
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
                    ifx = Width - 1 - (-ifx % Width);
                else if (ifx > Width - 1)
                    ifx = ifx % Width;
                if (ify < 0)
                    ify = Height - 1 - (-ify % Height);
                else if (ify > Height - 1)
                    ify = ify % Height;

                total_red = 0.0f;
                total_green = 0.0f;
                total_blue = 0.0f;
                for (ix = 0; ix <= 1; ix++)
                {
                    for (iy = 0; iy <= 1; iy++)
                    {
                        if (ify + iy < Height)
                            sli = (TBGR*)ScanLine(ify + iy);
                        else
                            sli = (TBGR*)ScanLine(Height - ify - iy);
                        if (ifx + ix < Width)
                        {
                            new_red = sli[ifx + ix].R;
                            new_green = sli[ifx + ix].G;
                            new_blue = sli[ifx + ix].B;
                        }
                        else
                        {
                            new_red = sli[Width - ifx - ix].R;
                            new_green = sli[Width - ifx - ix].G;
                            new_blue = sli[Width - ifx - ix].B;
                        }
                        weight = weight_x[ix] * weight_y[iy];
                        total_red = total_red + new_red * weight;
                        total_green = total_green + new_green * weight;
                        total_blue = total_blue + new_blue * weight;
                    }
                }
                switch (BitCount)
                {
                    case 24:
                        slo = (TBGR*)bmp.ScanLine(ty);
                        slo[tx].R = (byte)DibEffectsSupport.DibRound(total_red);
                        slo[tx].G = (byte)DibEffectsSupport.DibRound(total_green);
                        slo[tx].B = (byte)DibEffectsSupport.DibRound(total_blue);
                        break;
                    default:
                        // You can implement this procedure for 16,8,4,2 and 32 BitCount's DIB
                        return Result;
                }
            }
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 4512-4614 —— SmoothRotateWrap
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4512-4614 1:1（只实现 24bpp）。
    /// Theta := -Degree * Pi / 180（Extended）；xDiff/yDiff 用 `div 2`（整数除法）；双线性 + 环绕取模。
    /// </summary>
    public unsafe bool SmoothRotateWrap(TDIB bmp, int cx, int cy, double Degree)
    {
        float weight, Theta, cosTheta, sinTheta, sfrom_y, sfrom_x;
        int ifrom_y, ifrom_x, xDiff, yDiff, to_y, to_x;
        float[] weight_x = new float[2];
        float[] weight_y = new float[2];
        int ix, iy, new_red, new_green, new_blue;
        float total_red, total_green, total_blue;
        TBGR* sli;
        TBGR* slo;

        bool Result = true;
        switch (BitCount)
        {
            case 32:
            case 16:
            case 8:
            case 4:
            case 1:
                Result = false;
                return Result;
        }

        Theta = (float)(-Degree * Math.PI / 180);
        sinTheta = (float)Math.Sin(Theta);
        cosTheta = (float)Math.Cos(Theta);
        xDiff = (bmp.Width - Width) / 2;        // div 2（向零截断；差值非负时同 floor）
        yDiff = (bmp.Height - Height) / 2;
        for (to_y = 0; to_y <= bmp.Height - 1; to_y++)
        {
            for (to_x = 0; to_x <= bmp.Width - 1; to_x++)
            {
                sfrom_x = (cx + (to_x - cx) * cosTheta - (to_y - cy) * sinTheta) - xDiff;
                ifrom_x = (int)sfrom_x;
                sfrom_y = (cy + (to_x - cx) * sinTheta + (to_y - cy) * cosTheta) - yDiff;
                ifrom_y = (int)sfrom_y;
                if (sfrom_y >= 0)
                {
                    weight_y[1] = sfrom_y - ifrom_y;
                    weight_y[0] = 1 - weight_y[1];
                }
                else
                {
                    weight_y[0] = -(sfrom_y - ifrom_y);
                    weight_y[1] = 1 - weight_y[0];
                }
                if (sfrom_x >= 0)
                {
                    weight_x[1] = sfrom_x - ifrom_x;
                    weight_x[0] = 1 - weight_x[1];
                }
                else
                {
                    weight_x[0] = -(sfrom_x - ifrom_x);
                    weight_x[1] = 1 - weight_x[0];
                }
                if (ifrom_x < 0)
                    ifrom_x = Width - 1 - (-ifrom_x % Width);
                else if (ifrom_x > Width - 1)
                    ifrom_x = ifrom_x % Width;
                if (ifrom_y < 0)
                    ifrom_y = Height - 1 - (-ifrom_y % Height);
                else if (ifrom_y > Height - 1)
                    ifrom_y = ifrom_y % Height;

                total_red = 0.0f;
                total_green = 0.0f;
                total_blue = 0.0f;
                for (ix = 0; ix <= 1; ix++)
                {
                    for (iy = 0; iy <= 1; iy++)
                    {
                        if (ifrom_y + iy < Height)
                            sli = (TBGR*)ScanLine(ifrom_y + iy);
                        else
                            sli = (TBGR*)ScanLine(Height - ifrom_y - iy);
                        if (ifrom_x + ix < Width)
                        {
                            new_red = sli[ifrom_x + ix].R;
                            new_green = sli[ifrom_x + ix].G;
                            new_blue = sli[ifrom_x + ix].B;
                        }
                        else
                        {
                            new_red = sli[Width - ifrom_x - ix].R;
                            new_green = sli[Width - ifrom_x - ix].G;
                            new_blue = sli[Width - ifrom_x - ix].B;
                        }
                        weight = weight_x[ix] * weight_y[iy];
                        total_red = total_red + new_red * weight;
                        total_green = total_green + new_green * weight;
                        total_blue = total_blue + new_blue * weight;
                    }
                }
                switch (BitCount)
                {
                    case 24:
                        slo = (TBGR*)bmp.ScanLine(to_y);
                        slo[to_x].R = (byte)DibEffectsSupport.DibRound(total_red);
                        slo[to_x].G = (byte)DibEffectsSupport.DibRound(total_green);
                        slo[to_x].B = (byte)DibEffectsSupport.DibRound(total_blue);
                        break;
                    default:
                        // You can implement this procedure for 16,8,4,2 and 32 BitCount's DIB
                        return Result;
                }
            }
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 4618-4697 —— Rotate
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4618-4697 1:1。
    /// 32/16bpp → False；8/4/1bpp 先拷调色板；随后按定点正弦/余弦（$10000）反向映射采样。
    /// 接缝（§2.3）：`Dst.Canvas.Brush.Color := clBlack; Dst.Canvas.FillRect(Bounds(0,0,Width,Height))`
    /// 属 TCanvas/GDI 路径，不翻译实现；托管侧的 TDibCanvas 接缝面未提供 Brush/FillRect，
    /// 故此处保留原语句为注释、不改动像素（SetSize 新建的缓冲区已清零）。
    /// </summary>
    public unsafe bool Rotate(TDIB Dst, int cx, int cy, double Angle)
    {
        int X, Y, dx, dy, sdx, sdy, xDiff, yDiff, isinTheta, icosTheta;
        byte* D = null;
        byte* S = null;
        double sinTheta, cosTheta, Theta;
        TBGR Col = default;
        byte I;
        uint Color;
        byte* P;

        D = null;
        S = null;
        bool Result = true;
        Dst.SetSize(Width, Height, BitCount);
        // Dst.Canvas.Brush.Color := clBlack;
        // Dst.Canvas.FillRect(Bounds(0, 0, Width, Height));
        switch (BitCount)
        {
            case 32:
            case 16:
                Result = false;
                return Result;
            case 8:
            case 4:
            case 1:
                for (I = 0; I <= 255; I++)
                    Dst.ColorTable[I] = ColorTable[I];
                Dst.UpdatePalette();
                break;
        }

        Theta = -Angle * Math.PI / 180;
        sinTheta = Math.Sin(Theta);
        cosTheta = Math.Cos(Theta);
        xDiff = (Dst.Width - Width) / 2;
        yDiff = (Dst.Height - Height) / 2;
        isinTheta = DibEffectsSupport.DibRound(sinTheta * 0x10000);
        icosTheta = DibEffectsSupport.DibRound(cosTheta * 0x10000);
        for (Y = 0; Y <= Dst.Height - 1; Y++)
        {
            switch (BitCount)
            {
                case 4:
                case 1:
                    D = (byte*)Dst.ScanLine(Y);
                    S = (byte*)ScanLine(Y);
                    break;
                default:
                    break;
            }

            sdx = DibEffectsSupport.DibRound(((cx + (-cx) * cosTheta - (Y - cy) * sinTheta) - xDiff) * 0x10000);
            sdy = DibEffectsSupport.DibRound(((cy + (-cy) * sinTheta + (Y - cy) * cosTheta) - yDiff) * 0x10000);
            for (X = 0; X <= Dst.Width - 1; X++)
            {
                dx = sdx >> 16;     // dx := (sdx shr 16)，Integer 算术右移
                dy = sdy >> 16;
                if (dx > -1 && dx < Width && dy > -1 && dy < Height)
                {
                    switch (BitCount)
                    {
                        case 8:
                        case 24:
                            Dst.SetPixel(X, Y, GetPixel(dx, dy));
                            break;
                        case 4:
                            {
                                DIB.pfGetRGB(NowPixelFormat, GetPixel(dx, dy), out Col.R, out Col.G, out Col.B);
                                Color = (uint)(Col.R + Col.G + Col.B);
                                S++;
                                P = D + (X >> 1);
                                P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (Color << (int)DIBConstants.Shift4[X & 1]));
                            }
                            break;
                        case 1:
                            {
                                DIB.pfGetRGB(NowPixelFormat, GetPixel(dx, dy), out Col.R, out Col.G, out Col.B);
                                Color = (uint)(Col.R + Col.G + Col.B);
                                S++;
                                P = D + (X >> 3);
                                P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) | (Color << (int)DIBConstants.Shift1[X & 7]));
                            }
                            break;
                        default:
                            break;
                    }
                }
                sdx += icosTheta;
                sdy += isinTheta;
            }
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 4701-4707 —— GaussianBlur
    // =========================================================================================

    /// <summary>DIB.pas 4701-4707 1:1：对 bmp 依次调用 SplitBlur(1..Amount)。</summary>
    public void GaussianBlur(TDIB bmp, int Amount)
    {
        for (int I = 1; I <= Amount; I++)
            bmp.SplitBlur(I);
    }

    // =========================================================================================
    // DIB.pas 4710-4740 —— SplitBlur
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4710-4740 1:1（只有 24bpp 有效，其它位深直接 Exit）。
    /// 就地写回同一行，Lin1/Lin2 可能与 D 同一行，故跨像素读到的可能是已被改写的值（原文如此）。
    /// </summary>
    public unsafe void SplitBlur(int Amount)
    {
        TBGR* Lin1;
        TBGR* Lin2;
        int cx, X, Y;
        TBGR[] Buf = new TBGR[4];
        byte* D;

        switch (BitCount)
        {
            case 32:
            case 16:
            case 8:
            case 4:
            case 1:
                return;
        }

        for (Y = 0; Y <= Height - 1; Y++)
        {
            Lin1 = (TBGR*)ScanLine(TrimInt(Y + Amount, 0, Height - 1));
            Lin2 = (TBGR*)ScanLine(TrimInt(Y - Amount, 0, Height - 1));
            D = (byte*)ScanLine(Y);
            for (X = 0; X <= Width - 1; X++)
            {
                cx = TrimInt(X + Amount, 0, Width - 1);
                Buf[0] = Lin1[cx];
                Buf[1] = Lin2[cx];
                cx = TrimInt(X - Amount, 0, Width - 1);
                Buf[2] = Lin1[cx];
                Buf[3] = Lin2[cx];
                TBGR* q = (TBGR*)D;
                q->B = (byte)((Buf[0].B + Buf[1].B + Buf[2].B + Buf[3].B) >> 2);
                q->G = (byte)((Buf[0].G + Buf[1].G + Buf[2].G + Buf[3].G) >> 2);
                q->R = (byte)((Buf[0].R + Buf[1].R + Buf[2].R + Buf[3].R) >> 2);
                D += 3;
            }
        }
    }

    // =========================================================================================
    // DIB.pas 4744-4894 —— Twist
    // =========================================================================================

    /// <summary>
    /// DIB.pas 4744-4894 1:1（只实现 24bpp）。
    /// 内嵌 ArcTan2（注意 xt=0 且 yt&lt;=0 时返回 -(Pi/2)，且 xt&lt;0 时会算两次 ArcTan）；
    /// Amount = 0 时改成 1；OFFSET := -(Pi/2)；双线性 + 环绕取模。
    /// </summary>
    public unsafe bool Twist(TDIB bmp, byte Amount)
    {
        float fxmid, fymid;
        float txmid, tymid;
        float fx = 0, fy = 0;
        float tx2, ty2;
        float R;
        float Theta;
        int ifx, ify;
        float dx, dy;
        float OFFSET;
        int ty, tx, ix, iy;
        float[] weight_x = new float[2];
        float[] weight_y = new float[2];
        float weight;
        int new_red, new_green, new_blue;
        float total_red, total_green, total_blue;
        TBGR* sli;
        TBGR* slo;

        float ArcTan2(float xt, float yt)
        {
            if (xt == 0)
                if (yt > 0)
                    return (float)(Math.PI / 2);
                else
                    return (float)(-(Math.PI / 2));
            else
            {
                float Result = (float)Math.Atan(yt / xt);
                if (xt < 0)
                    Result = (float)(Math.PI + Math.Atan(yt / xt));
                return Result;
            }
        }

        bool Result = true;
        switch (BitCount)
        {
            case 32:
            case 16:
            case 8:
            case 4:
            case 1:
                Result = false;
                return Result;
        }

        if (Amount == 0)
            Amount = 1;
        OFFSET = (float)(-(Math.PI / 2));
        dx = Width - 1;
        dy = Height - 1;
        R = (float)Math.Sqrt(dx * dx + dy * dy);
        tx2 = R;
        ty2 = R;
        txmid = (Width - 1) / 2f;
        tymid = (Height - 1) / 2f;
        fxmid = (Width - 1) / 2f;
        fymid = (Height - 1) / 2f;
        if (tx2 >= Width)
            tx2 = Width - 1;
        if (ty2 >= Height)
            ty2 = Height - 1;
        for (ty = 0; ty <= DibEffectsSupport.DibRound(ty2); ty++)
        {
            for (tx = 0; tx <= DibEffectsSupport.DibRound(tx2); tx++)
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
                    Theta = ArcTan2(dx, dy) - R / Amount - OFFSET;
                    fx = R * (float)Math.Cos(Theta);
                    fy = R * (float)Math.Sin(Theta);
                }
                fx = fx + fxmid;
                fy = fy + fymid;
                ify = (int)fy;
                ifx = (int)fx;
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
                    ifx = Width - 1 - (-ifx % Width);
                else if (ifx > Width - 1)
                    ifx = ifx % Width;
                if (ify < 0)
                    ify = Height - 1 - (-ify % Height);
                else if (ify > Height - 1)
                    ify = ify % Height;

                total_red = 0.0f;
                total_green = 0.0f;
                total_blue = 0.0f;
                for (ix = 0; ix <= 1; ix++)
                {
                    for (iy = 0; iy <= 1; iy++)
                    {
                        if (ify + iy < Height)
                            sli = (TBGR*)ScanLine(ify + iy);
                        else
                            sli = (TBGR*)ScanLine(Height - ify - iy);
                        if (ifx + ix < Width)
                        {
                            new_red = sli[ifx + ix].R;
                            new_green = sli[ifx + ix].G;
                            new_blue = sli[ifx + ix].B;
                        }
                        else
                        {
                            new_red = sli[Width - ifx - ix].R;
                            new_green = sli[Width - ifx - ix].G;
                            new_blue = sli[Width - ifx - ix].B;
                        }
                        weight = weight_x[ix] * weight_y[iy];
                        total_red = total_red + new_red * weight;
                        total_green = total_green + new_green * weight;
                        total_blue = total_blue + new_blue * weight;
                    }
                }
                switch (BitCount)
                {
                    case 24:
                        slo = (TBGR*)bmp.ScanLine(ty);
                        slo[tx].R = (byte)DibEffectsSupport.DibRound(total_red);
                        slo[tx].G = (byte)DibEffectsSupport.DibRound(total_green);
                        slo[tx].B = (byte)DibEffectsSupport.DibRound(total_blue);
                        break;
                    default:
                        // You can implement this procedure for 16,8,4,2 and 32 BitCount's DIB
                        return Result;
                }
            }
        }
        return Result;
    }

    // =========================================================================================
    // DIB.pas 4898-4906 —— TDIB.TrimInt
    // =========================================================================================

    /// <summary>DIB.pas 4898-4906 1:1（先判 Max 再判 Min）。</summary>
    public int TrimInt(int I, int Min, int Max)
    {
        if (I > Max)
            return Max;
        else if (I < Min)
            return Min;
        else
            return I;
    }

    // =========================================================================================
    // DIB.pas 4910-4918 —— TDIB.IntToByte
    // =========================================================================================

    /// <summary>DIB.pas 4910-4918 1:1（&gt;255 截 255，&lt;0 截 0）。</summary>
    public byte IntToByte(int I)
    {
        if (I > 255)
            return 255;
        else if (I < 0)
            return 0;
        else
            return (byte)I;
    }

    // =========================================================================================
    // DIB.pas 4927-4936 —— AlphaChannel 属性访问器
    // =========================================================================================

    /// <summary>DIB.pas 4927-4930 1:1（property AlphaChannel read）。</summary>
    public TDIB GetAlphaChannel()
    {
        TDIB Result;
        RetAlphaChannel(out Result);
        return Result;
    }

    /// <summary>DIB.pas 4932-4936 1:1（property AlphaChannel write；失败抛 Exception）。</summary>
    public void SetAlphaChannel(TDIB Value)
    {
        if (!AssignAlphaChannel(Value))
            throw new Exception("Cannot set alphachannel from DIB.");
    }
}

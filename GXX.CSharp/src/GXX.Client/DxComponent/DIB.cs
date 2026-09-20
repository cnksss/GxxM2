using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas（Source\Client-HGE\DxComponent\DIB.pas，8,747 物理行 / 8,087 统计行）1:1 移植 —— 第 1 片：
// 单元级类型/常量/函数 + 接缝 + 调色板管理器（DIB.pas 1-800）。
//
// 分片清单（全部落在 DxComponent/Dib*.cs 独占区）：
//   DIB.cs             ← 本文件：类型、单元级函数、接缝、TPaletteManager      （DIB.pas 1-800）
//   DIB.SharedImage.cs ← TDIBSharedImage（NewImage/BMP 读写/RLE/调色板）        （DIB.pas 802-1555）
//   DIB.Core.cs        ← TDIB 核心（属性/ScanLine/Pixels/位深转换/进度）         （DIB.pas 1557-2540）
//   DIB.Effects.cs     ← TDIB 特效（Mirror/Blur/Negative/…/Twist）              （DIB.pas 2526-4940）
//   DIB.Fusion.cs      ← TDIB 绘制与 DXFusion 特效（Draw*/Do*）                 （DIB.pas 4940-7930）
//   DIB.Tail.cs        ← TDIB 尾部（DoRotate/Ink/Distort/ColoredLine 等）       （DIB.pas 7930-8747）
//
// 像素内存表示：原文用 GlobalAlloc/GetMem 拿非托管块，再做 `PArrayDWord(Integer(FTopPBits)+Y*FNextLine)[X]`
// 这类指针运算。托管侧改用 Marshal.AllocHGlobal 持有 IntPtr，保留完全一样的指针算术
// （`*(uint*)((byte*)TopPBits + Y*FNextLine + X*4)`），使逐行搬运可逐字对照。
//
// 不可移植项（§2.3）：GDI 路径（CreateDIBSection / CreateCompatibleDC / SelectObject / SetDIBColorTable /
// CreatePalette / StretchDIBits / StretchBlt / TCanvas.Draw / TPicture.RegisterFileFormat）不翻译实现，
// 收敛到 IDibGdiSeam / IDibCanvasSeam，原方法签名全部保留。
// =============================================================================================

/// <summary>DIB.pas 12 —— TColorLineStyle（ColoredLine 用）。</summary>
public enum TColorLineStyle { csSolid, csGradient, csRainbow }

/// <summary>DIB.pas 13 —— TColorLinePixelGeometry。</summary>
public enum TColorLinePixelGeometry { pgPoint, pgCircular, pgRectangular }

/// <summary>DIB.pas 93-94 —— TFilterTypeResample。</summary>
public enum TFilterTypeResample { ftrBox, ftrTriangle, ftrHermite, ftrBell, ftrBSpline, ftrLanczos3, ftrMitchell }

/// <summary>DIB.pas 96 —— TDistortType。</summary>
public enum TDistortType { dtFast, dtSlow }

/// <summary>DIB.pas 98 —— TFilterMode（DXFusion effect type）。</summary>
public enum TFilterMode { fmNormal, fmMix50, fmMix25, fmMix75 }

/// <summary>DIB.pas 20-22 —— PBGR/TBGR 打包记录（B,G,R 三字节）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TBGR
{
    public byte B;
    public byte G;
    public byte R;
}

/// <summary>
/// DIB.pas 48-53 —— TDIBPixelFormat。
/// 12 个 DWord 字段，SizeOf = 48（测试锁定）。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TDIBPixelFormat
{
    public uint RBitMask;
    public uint GBitMask;
    public uint BBitMask;
    public uint RBitCount;
    public uint GBitCount;
    public uint BBitCount;
    public uint RShift;
    public uint GShift;
    public uint BShift;
    public uint RBitCount2;
    public uint GBitCount2;
    public uint BBitCount2;

    /// <summary>Delphi CompareMem(@A, @B, SizeOf(TDIBPixelFormat)) 的托管等价。</summary>
    public static bool MemEquals(in TDIBPixelFormat a, in TDIBPixelFormat b)
    {
        return a.RBitMask == b.RBitMask && a.GBitMask == b.GBitMask && a.BBitMask == b.BBitMask &&
               a.RBitCount == b.RBitCount && a.GBitCount == b.GBitCount && a.BBitCount == b.BBitCount &&
               a.RShift == b.RShift && a.GShift == b.GShift && a.BShift == b.BShift &&
               a.RBitCount2 == b.RBitCount2 && a.GBitCount2 == b.GBitCount2 && a.BBitCount2 == b.BBitCount2;
    }

    /// <summary>只比较三个掩码（TDIB.SetSize 的判等条件只用这三个）。</summary>
    public static bool MaskEquals(in TDIBPixelFormat a, in TDIBPixelFormat b)
        => a.RBitMask == b.RBitMask && a.GBitMask == b.GBitMask && a.BBitMask == b.BBitMask;
}

/// <summary>
/// DIB.pas 15 / Windows.TRGBQuad —— 注意字段顺序 rgbBlue,rgbGreen,rgbRed,rgbReserved。
/// 32bpp 像素在内存里就是 4 个这样的字节（BGRA），因此 GetPixel 的 DWord = R|G&lt;&lt;8|B&lt;&lt;16|A&lt;&lt;24。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TRGBQuad
{
    public byte rgbBlue;
    public byte rgbGreen;
    public byte rgbRed;
    public byte rgbReserved;

    public static TRGBQuad Of(byte r, byte g, byte b) => new TRGBQuad { rgbRed = r, rgbGreen = g, rgbBlue = b, rgbReserved = 0 };
}

/// <summary>Windows.TPaletteEntry —— peRed,peGreen,peBlue,peFlags。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TPaletteEntry
{
    public byte peRed;
    public byte peGreen;
    public byte peBlue;
    public byte peFlags;
}

/// <summary>DIB.pas 100-104 —— TLightSource。</summary>
public struct TLightSource
{
    public int X;
    public int Y;
    public int Size1;
    public int Size2;
    public int Color; // TColor
}

/// <summary>DIB.pas 108 —— TMatrixSetting = array[0..9] of Integer。</summary>
public static class TMatrixSettingUtil
{
    public const int Length = 10;
}

/// <summary>
/// DIB.pas 443-486 —— 单元级常量。
/// 原文这些是编译期常量数组；C# 无常量数组，改 static readonly（值逐项照抄）。
/// </summary>
public static class DIBConstants
{
    /// <summary>DIB.pas 444 —— DefaultFilterRadius: array[TFilterTypeResample] of Single（按枚举序）。</summary>
    public static readonly float[] DefaultFilterRadius = { 0.5f, 1f, 1f, 1.5f, 2f, 3f, 2f };

    /// <summary>DIB.pas 471 —— EdgeFilter: TFilter = ((-1,-1,-1),(-1,8,-1),(-1,-1,-1))。</summary>
    public static readonly short[,] EdgeFilter = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };

    /// <summary>DIB.pas 472 —— StrongOutlineFilter。</summary>
    public static readonly short[,] StrongOutlineFilter = { { -100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 100 } };

    /// <summary>DIB.pas 473 —— Enhance3DFilter。</summary>
    public static readonly short[,] Enhance3DFilter = { { -100, 5, 5 }, { 5, 5, 5 }, { 5, 5, 100 } };

    /// <summary>DIB.pas 474 —— LinearFilter。</summary>
    public static readonly short[,] LinearFilter = { { -40, -40, -40 }, { -40, 255, -40 }, { -40, -40, -40 } };

    /// <summary>DIB.pas 475 —— GranularFilter。</summary>
    public static readonly short[,] GranularFilter = { { -20, 5, 20 }, { 5, -10, 5 }, { 100, 5, -100 } };

    /// <summary>DIB.pas 476 —— SharpFilter。</summary>
    public static readonly short[,] SharpFilter = { { -2, -2, -2 }, { -2, 20, -2 }, { -2, -2, -2 } };

    /// <summary>DIB.pas 482 —— msEmboss: TMatrixSetting = (-1,-1,0,-1,6,1,0,1,1,6)。</summary>
    public static readonly int[] msEmboss = { -1, -1, 0, -1, 6, 1, 0, 1, 1, 6 };

    /// <summary>DIB.pas 483 —— msHardEmboss。</summary>
    public static readonly int[] msHardEmboss = { -4, -2, -1, -2, 10, 2, -1, 2, 4, 8 };

    /// <summary>DIB.pas 484 —— msBlur。</summary>
    public static readonly int[] msBlur = { 1, 2, 1, 2, 4, 2, 1, 2, 1, 16 };

    /// <summary>DIB.pas 485 —— msSharpen。</summary>
    public static readonly int[] msSharpen = { -1, -1, -1, -1, 15, -1, -1, -1, -1, 7 };

    /// <summary>DIB.pas 486 —— msEdgeDetect。</summary>
    public static readonly int[] msEdgeDetect = { -1, -1, -1, -1, 8, -1, -1, -1, -1, 1 };

    /// <summary>DIB.pas 1983 —— Mask1（1bpp 位掩码）。</summary>
    public static readonly uint[] Mask1 = { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

    /// <summary>DIB.pas 1984 —— Mask1n（原位清位用）。</summary>
    public static readonly uint[] Mask1n = { 0xFFFFFF7F, 0xFFFFFFBF, 0xFFFFFFDF, 0xFFFFFFEF, 0xFFFFFFF7, 0xFFFFFFFB, 0xFFFFFFFD, 0xFFFFFFFE };

    /// <summary>DIB.pas 1986 —— Mask4。</summary>
    public static readonly uint[] Mask4 = { 0xF0, 0x0F };

    /// <summary>DIB.pas 1987 —— Mask4n。</summary>
    public static readonly uint[] Mask4n = { 0xFFFFFF0F, 0xFFFFFFF0 };

    /// <summary>DIB.pas 1989 —— Shift1。</summary>
    public static readonly uint[] Shift1 = { 7, 6, 5, 4, 3, 2, 1, 0 };

    /// <summary>DIB.pas 1990 —— Shift4。</summary>
    public static readonly uint[] Shift4 = { 4, 0 };
}

/// <summary>
/// DIB.pas 接缝集合（§2.3：GDI / 直接显存路径不翻译实现，只保留签名与理由）。
/// </summary>
public static class DibSeams
{
    /// <summary>
    /// 接缝：Windows GDI 面。
    /// 原文 DIB.pas 通过 CreateCompatibleDC / CreateDIBSection / SelectObject / SetDIBColorTable /
    /// CreatePalette / StretchDIBits / StretchBlt / GetPaletteEntries / GdiFlush 直接操作 GDI 句柄。
    /// 托管侧不翻译这些实现（§2.3 不移植项 4：句柄级 hack → 语义等效的标准实现），
    /// 全部收敛到本接口；未装载时 TDIBSharedImage 一律走 MemoryImage 分支。
    /// </summary>
    public interface IDibGdiSeam
    {
        IntPtr CreateCompatibleDC();
        IntPtr CreateDIBSection(IntPtr dc, IntPtr bitmapInfo, uint usage, out IntPtr bits, IntPtr section, uint offset);
        IntPtr SelectObject(IntPtr dc, IntPtr obj);
        bool DeleteObject(IntPtr obj);
        bool DeleteDC(IntPtr dc);
        bool SetDIBColorTable(IntPtr dc, uint start, uint count, byte[] table);
        IntPtr CreatePalette(byte[] logPalette);
        void GdiFlush();
        bool GetPaletteEntries(IntPtr palette, uint start, uint count, byte[] entries);
    }

    /// <summary>
    /// 接缝：TCanvas.Draw / TCanvas.Handle / StretchDIBits。
    /// 原文 `Canvas.Draw(0, 0, Temp)` 走的是 GDI BitBlt（会同时做位深转换与调色板映射）；
    /// 托管侧改由 IDibCanvasSeam 提供（未装载时退化为"仅同格式逐行搬运"，
    /// 并把退化写进 TDIB.LastCanvasDrawDegraded 供测试断言）。
    /// </summary>
    public interface IDibCanvasSeam
    {
        void Draw(IntPtr dc, int x, int y, TDIB source);
        void DrawGraphic(IntPtr dc, int x, int y, object graphic);
        uint CopyMode { get; }
    }

    /// <summary>
    /// 接缝：TPicture.RegisterClipboardFormat / RegisterFileFormat（DIB.pas 8739-8742 的 initialization/finalization）。
    /// 托管侧无 TPicture 注册表；保留签名以便客户端装载层接入。
    /// </summary>
    public static Action<string> RegisterClipboardFormatHook;
    public static Action<string, string> RegisterFileFormatHook;

    /// <summary>
    /// 接缝：GlobalLock / GlobalUnlock / GlobalSize / GlobalAlloc（DIB.pas 2057-2069 的 TGlobalMemoryStream、
    /// 2119-2141 的 SaveToClipboardFormat、2071-2082 的 LoadFromClipboardFormat 使用）。
    /// 剪贴板全局内存句柄在托管侧无对应语义（§2.3 不移植项 4），收敛到此接缝。
    /// </summary>
    public interface IDibGlobalMemorySeam
    {
        IntPtr GlobalAlloc(uint flags, int size);
        IntPtr GlobalLock(IntPtr handle);
        bool GlobalUnlock(IntPtr handle);
        int GlobalSize(IntPtr handle);
    }

    public static IDibGdiSeam Gdi;
    public static IDibCanvasSeam Canvas;
    public static IDibGlobalMemorySeam GlobalMemory;

    /// <summary>GHND = GMEM_MOVEABLE or GMEM_ZEROINIT（Windows 常量）。</summary>
    public const uint GHND = 0x0042;
    public const uint GMEM_FIXED = 0x0000;

    /// <summary>DIB.pas 8738-8742 initialization/finalization 的托管等价入口。</summary>
    public static void RegisterPictureFormats()
    {
        RegisterClipboardFormatHook?.Invoke("CF_DIB");
        RegisterFileFormatHook?.Invoke("dib", "Device Independent Bitmap");
    }

    /// <summary>DIB.pas 8744-8745 finalization：释放单元级全局对象。</summary>
    public static void FinalizeUnit()
    {
        DIB.ResetEmptyDIBImage();
        DIB.ResetPaletteManager();
    }
}

/// <summary>
/// DIB.pas 单元级函数（interface 段 446-467 + implementation 段 505-662）。
/// 原文这些是 unit-level 函数；C# 收进静态类，函数名/参数名/分支顺序照抄。
/// </summary>
public static class DIB
{
    /// <summary>DIB.pas 466 —— TOC = 0..511。</summary>
    public const int TOCMin = 0;
    public const int TOCMax = 511;

    /// <summary>DIB.pas 2084-2085 —— BitmapFileType = Ord('B') + Ord('M') * $100 = $4D42。</summary>
    public const int BitmapFileType = 'B' + ('M' * 0x100);

    // Windows 常量（原文直接引用 Windows 单元）。
    public const uint BI_RGB = 0;
    public const uint BI_RLE8 = 1;
    public const uint BI_RLE4 = 2;
    public const uint BI_BITFIELDS = 3;
    public const uint DIB_RGB_COLORS = 0;
    public const int SizeOfBitmapFileHeader = 14;
    public const int SizeOfBitmapInfoHeader = 40;
    public const int SizeOfBitmapCoreHeader = 12;
    public const int SizeOfTRGBQuad = 4;
    public const int SizeOfTRGBTriple = 3;
    public const int CF_DIB = 8;

    // =========================================================================================
    // DIB.pas 505-513 —— DSin / DCos
    // =========================================================================================

    /// <summary>
    /// DIB.pas 505-508 1:1：Result := Sin(((C * 360) / 511) * Pi / 180)。
    /// 注意 Delphi 的 `/` 是**浮点除法**（即使两侧都是 Integer），故此处不是整数除法。
    /// </summary>
    public static float DSin(int C)
        => (float)Math.Sin(((C * 360) / 511.0) * Math.PI / 180.0);

    /// <summary>DIB.pas 510-513 1:1：Cos(((C * 360) / 511) * Pi / 180)。</summary>
    public static float DCos(int C)
        => (float)Math.Cos(((C * 360) / 511.0) * Math.PI / 180.0);

    // =========================================================================================
    // DIB.pas 515-551 —— 像素格式
    // =========================================================================================

    /// <summary>
    /// DIB.pas 515-529 1:1。
    /// 原文笔误照抄：RShift := (GBitCount + BBitCount) - (8 - RBitCount)（一般应为 8-RBitCount-(8-RBitCount)=…，
    /// 但对 5:6:5 / 5:5:5 / 8:8:8 三种实际用到的格式结果与 pfGetRGB 的读取方式自洽）。
    /// </summary>
    public static TDIBPixelFormat MakeDIBPixelFormat(int RBitCount, int GBitCount, int BBitCount)
    {
        TDIBPixelFormat Result = default;
        unchecked
        {
            Result.RBitMask = (uint)(((1 << RBitCount) - 1) << (GBitCount + BBitCount));
            Result.GBitMask = (uint)(((1 << GBitCount) - 1) << BBitCount);
            Result.BBitMask = (uint)((1 << BBitCount) - 1);
            Result.RBitCount = (uint)RBitCount;
            Result.GBitCount = (uint)GBitCount;
            Result.BBitCount = (uint)BBitCount;
            Result.RBitCount2 = (uint)(8 - RBitCount);
            Result.GBitCount2 = (uint)(8 - GBitCount);
            Result.BBitCount2 = (uint)(8 - BBitCount);
            Result.RShift = (uint)((GBitCount + BBitCount) - (8 - RBitCount));
            Result.GShift = (uint)(BBitCount - (8 - GBitCount));
            Result.BShift = (uint)(8 - BBitCount);
        }
        return Result;
    }

    /// <summary>
    /// DIB.pas 531-551 1:1（含内嵌 GetBitCount）。
    /// GetBitCount：先跳过低位 0（I&lt;31 保护），再从 I 起数连续 1 的个数。
    /// </summary>
    public static TDIBPixelFormat MakeDIBPixelFormatMask(int RBitMask, int GBitMask, int BBitMask)
    {
        int GetBitCount(int B)
        {
            int I = 0;
            while (I < 31 && ((1 << I) & B) == 0) I++;

            int Result = 0;
            while (((1 << I) & B) != 0)
            {
                I++;
                Result++;
            }
            return Result;
        }

        return MakeDIBPixelFormat(GetBitCount(RBitMask), GetBitCount(GBitMask), GetBitCount(BBitMask));
    }

    /// <summary>DIB.pas 553-557 1:1。</summary>
    public static uint pfRGB(in TDIBPixelFormat PixelFormat, byte R, byte G, byte B)
    {
        unchecked
        {
            return ((uint)(R << (int)PixelFormat.RShift) & PixelFormat.RBitMask)
                 | ((uint)(G << (int)PixelFormat.GShift) & PixelFormat.GBitMask)
                 | ((uint)(B >> (int)PixelFormat.BShift) & PixelFormat.BBitMask);
        }
    }

    /// <summary>DIB.pas 559-570 1:1（三个通道都用 "or (x shr NBitCount2)" 做低位复制）。</summary>
    public static void pfGetRGB(in TDIBPixelFormat PixelFormat, uint Color, out byte R, out byte G, out byte B)
    {
        unchecked
        {
            R = (byte)((Color & PixelFormat.RBitMask) >> (int)PixelFormat.RShift);
            R = (byte)(R | (R >> (int)PixelFormat.RBitCount2));
            G = (byte)((Color & PixelFormat.GBitMask) >> (int)PixelFormat.GShift);
            G = (byte)(G | (G >> (int)PixelFormat.GBitCount2));
            B = (byte)((Color & PixelFormat.BBitMask) << (int)PixelFormat.BShift);
            B = (byte)(B | (B >> (int)PixelFormat.BBitCount2));
        }
    }

    /// <summary>DIB.pas 572-579 1:1。</summary>
    public static byte pfGetRValue(in TDIBPixelFormat PixelFormat, uint Color)
    {
        unchecked
        {
            byte Result = (byte)((Color & PixelFormat.RBitMask) >> (int)PixelFormat.RShift);
            Result = (byte)(Result | (Result >> (int)PixelFormat.RBitCount2));
            return Result;
        }
    }

    /// <summary>DIB.pas 581-588 1:1。</summary>
    public static byte pfGetGValue(in TDIBPixelFormat PixelFormat, uint Color)
    {
        unchecked
        {
            byte Result = (byte)((Color & PixelFormat.GBitMask) >> (int)PixelFormat.GShift);
            Result = (byte)(Result | (Result >> (int)PixelFormat.GBitCount2));
            return Result;
        }
    }

    /// <summary>DIB.pas 590-597 1:1。</summary>
    public static byte pfGetBValue(in TDIBPixelFormat PixelFormat, uint Color)
    {
        unchecked
        {
            byte Result = (byte)((Color & PixelFormat.BBitMask) << (int)PixelFormat.BShift);
            Result = (byte)(Result | (Result >> (int)PixelFormat.BBitCount2));
            return Result;
        }
    }

    // =========================================================================================
    // DIB.pas 599-662 —— 调色板辅助
    // =========================================================================================

    /// <summary>DIB.pas 599-611 1:1：rgbRed=rgbGreen=rgbBlue=I, rgbReserved=0。</summary>
    public static TRGBQuad[] GreyscaleColorTable()
    {
        var Result = new TRGBQuad[256];
        for (int I = 0; I <= 255; I++)
        {
            Result[I].rgbRed = (byte)I;
            Result[I].rgbGreen = (byte)I;
            Result[I].rgbBlue = (byte)I;
            Result[I].rgbReserved = 0;
        }
        return Result;
    }

    /// <summary>DIB.pas 613-622 1:1。</summary>
    public static TRGBQuad RGBQuad(byte R, byte G, byte B)
    {
        TRGBQuad Result = default;
        Result.rgbRed = R;
        Result.rgbGreen = G;
        Result.rgbBlue = B;
        Result.rgbReserved = 0;
        return Result;
    }

    /// <summary>DIB.pas 624-634 1:1。</summary>
    public static TRGBQuad PaletteEntryToRGBQuad(in TPaletteEntry Entry)
    {
        TRGBQuad Result = default;
        Result.rgbRed = Entry.peRed;
        Result.rgbGreen = Entry.peGreen;
        Result.rgbBlue = Entry.peBlue;
        Result.rgbReserved = 0;
        return Result;
    }

    /// <summary>DIB.pas 636-642 1:1。</summary>
    public static TRGBQuad[] PaletteEntriesToRGBQuads(TPaletteEntry[] Entries)
    {
        var Result = new TRGBQuad[256];
        for (int I = 0; I <= 255; I++)
            Result[I] = PaletteEntryToRGBQuad(Entries[I]);
        return Result;
    }

    /// <summary>DIB.pas 644-654 1:1。</summary>
    public static TPaletteEntry RGBQuadToPaletteEntry(in TRGBQuad quad)
    {
        TPaletteEntry Result = default;
        Result.peRed = quad.rgbRed;
        Result.peGreen = quad.rgbGreen;
        Result.peBlue = quad.rgbBlue;
        Result.peFlags = 0;
        return Result;
    }

    /// <summary>DIB.pas 656-662 1:1。</summary>
    public static TPaletteEntry[] RGBQuadsToPaletteEntries(TRGBQuad[] quads)
    {
        var Result = new TPaletteEntry[256];
        for (int I = 0; I <= 255; I++)
            Result[I] = RGBQuadToPaletteEntry(quads[I]);
        return Result;
    }

    /// <summary>DIB.pas 5142-5145 1:1（负值归零）。</summary>
    public static int PosValue(int Value) => Value < 0 ? 0 : Value;

    /// <summary>DIB.pas 5940-5945 1:1（&gt;255 截 255，&lt;0 截 0）。</summary>
    public static byte IntToByte(int I)
    {
        if (I > 255) return 255;
        if (I < 0) return 0;
        return (byte)I;
    }

    /// <summary>DIB.pas 5968-5973 1:1（先判 Max 再判 Min）。</summary>
    public static int TrimInt(int I, int Min, int Max)
    {
        if (I > Max) return Max;
        if (I < Min) return Min;
        return I;
    }

    // =========================================================================================
    // 调色板 ↔ 原始字节（原文靠 Move/CompareMem 在 TRGBQuads 上直接操作；托管侧补两个助手）
    // =========================================================================================

    /// <summary>把 TRGBQuads 摊平成 256*4 字节（B,G,R,Reserved 顺序，与 Windows 一致）。</summary>
    public static byte[] ColorTableToBytes(TRGBQuad[] table, int count)
    {
        var bytes = new byte[Math.Max(count, 0) * 4];
        for (int i = 0; i < count; i++)
        {
            bytes[i * 4 + 0] = table[i].rgbBlue;
            bytes[i * 4 + 1] = table[i].rgbGreen;
            bytes[i * 4 + 2] = table[i].rgbRed;
            bytes[i * 4 + 3] = table[i].rgbReserved;
        }
        return bytes;
    }

    /// <summary>从 256*4 字节还原 TRGBQuads（不足 256 项时其余保持 0）。</summary>
    public static TRGBQuad[] BytesToColorTable(byte[] bytes, int count)
    {
        var table = new TRGBQuad[256];
        if (bytes == null) return table;
        for (int i = 0; i < count && i * 4 + 3 < bytes.Length; i++)
        {
            table[i].rgbBlue = bytes[i * 4 + 0];
            table[i].rgbGreen = bytes[i * 4 + 1];
            table[i].rgbRed = bytes[i * 4 + 2];
            table[i].rgbReserved = bytes[i * 4 + 3];
        }
        return table;
    }

    // =========================================================================================
    // DIB.pas 666-800 —— TPaletteManager / PaletteManager / FPaletteManager
    // =========================================================================================

    /// <summary>
    /// DIB.pas 672-682 —— TPaletteItem。
    /// 原文的 Palette:HPalette 是 GDI 句柄（CreatePalette 产物）；托管侧保留同名字段，
    /// 值由 DibSeams.Gdi.CreatePalette 提供（未装载接缝时为 0，且不参与比较失败）。
    /// DIB.pas 693-708 的 Destroy/AddRef/Release 1:1。
    /// </summary>
    public sealed class TPaletteItem
    {
        public int ID;
        public IntPtr Palette;
        public int RefCount;
        public TRGBQuad[] ColorTable = new TRGBQuad[256];
        public int ColorTableCount;

        /// <summary>DIB.pas 693-697 —— destructor：DeleteObject(Palette) + inherited Destroy。</summary>
        public void Destroy()
        {
            if (Palette != IntPtr.Zero) DibSeams.Gdi?.DeleteObject(Palette);
            Palette = IntPtr.Zero;
        }

        /// <summary>DIB.pas 699-702。</summary>
        public void AddRef() => RefCount++;

        /// <summary>DIB.pas 704-708 —— RefCount &lt;= 0 时 Free（托管侧由 manager 从列表移除）。</summary>
        public void Release()
        {
            RefCount--;
            if (RefCount <= 0) Destroy();
        }
    }

    /// <summary>DIB.pas 684-691 + 710-790 —— TPaletteManager（调色板去重缓存）。</summary>
    public sealed class TPaletteManager
    {
        /// <summary>DIB.pas 686 —— FList: TCollection（保序）。</summary>
        public readonly List<TPaletteItem> FList = new();

        /// <summary>DIB.pas 710-714。</summary>
        public TPaletteManager() { }

        /// <summary>DIB.pas 716-720 —— FList.Free。</summary>
        public void Destroy()
        {
            foreach (var item in FList) item.Destroy();
            FList.Clear();
        }

        /// <summary>
        /// DIB.pas 722-771 1:1。
        /// ID 哈希 = ColorTableCount + Σ(rgbRed+rgbGreen+rgbBlue)（逐项累加，顺序照抄）。
        /// 命中条件：ID 相同 且 ColorTableCount 相同 且 CompareMem(ColorTable, 前 ColorTableCount*4 字节)。
        /// 新建时构造 TMyLogPalette（palVersion=$300, palNumEntries=Count, palPalEntry=RGBQuadsToPaletteEntries）
        /// 后交给 GDI 接缝。
        /// </summary>
        public IntPtr CreatePalette(TRGBQuad[] ColorTable, int ColorTableCount)
        {
            int I, ID;
            TPaletteItem Item;

            // Hash key making
            ID = ColorTableCount;
            for (I = 0; I <= ColorTableCount - 1; I++)
            {
                ID += ColorTable[I].rgbRed;
                ID += ColorTable[I].rgbGreen;
                ID += ColorTable[I].rgbBlue;
            }

            // Does the same palette already exist?
            for (I = 0; I <= FList.Count - 1; I++)
            {
                Item = FList[I];
                if (Item.ID == ID && Item.ColorTableCount == ColorTableCount &&
                    ColorTablePrefixEquals(Item.ColorTable, ColorTable, ColorTableCount))
                {
                    Item.AddRef();
                    return Item.Palette;
                }
            }

            // New palette making
            Item = new TPaletteItem { ID = ID };
            Array.Copy(ColorTable, Item.ColorTable, Math.Min(ColorTableCount, 256));
            Item.ColorTableCount = ColorTableCount;
            FList.Add(Item);

            // with LogPalette do begin palVersion := $300; palNumEntries := ColorTableCount;
            //   palPalEntry := RGBQuadsToPaletteEntries(ColorTable); end;
            var entryBytes = new byte[256 * 4];
            var entries = RGBQuadsToPaletteEntries(ColorTable);
            for (int e = 0; e < 256; e++)
            {
                entryBytes[e * 4 + 0] = entries[e].peRed;
                entryBytes[e * 4 + 1] = entries[e].peGreen;
                entryBytes[e * 4 + 2] = entries[e].peBlue;
                entryBytes[e * 4 + 3] = entries[e].peFlags;
            }
            var logPalette = new byte[4 + 256 * 4];
            logPalette[0] = 0x00; logPalette[1] = 0x03; // palVersion = $300 (LE)
            logPalette[2] = (byte)(ColorTableCount & 0xFF);
            logPalette[3] = (byte)((ColorTableCount >> 8) & 0xFF);
            Array.Copy(entryBytes, 0, logPalette, 4, entryBytes.Length);

            Item.Palette = DibSeams.Gdi?.CreatePalette(logPalette) ?? IntPtr.Zero;
            Item.AddRef();
            return Item.Palette;
        }

        /// <summary>DIB.pas 773-790 1:1：Palette=0 直接退出；找到则 Palette:=0 并 Release。</summary>
        public void DeletePalette(ref IntPtr Palette)
        {
            if (Palette == IntPtr.Zero) return;

            for (int I = 0; I <= FList.Count - 1; I++)
            {
                var Item = FList[I];
                if (Item.Palette == Palette)
                {
                    Palette = IntPtr.Zero;
                    var old = Item.RefCount;
                    Item.Release();
                    if (old - 1 <= 0) FList.Remove(Item);
                    return;
                }
            }
        }

        private static bool ColorTablePrefixEquals(TRGBQuad[] a, TRGBQuad[] b, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (a[i].rgbBlue != b[i].rgbBlue || a[i].rgbGreen != b[i].rgbGreen ||
                    a[i].rgbRed != b[i].rgbRed || a[i].rgbReserved != b[i].rgbReserved) return false;
            }
            return true;
        }
    }

    /// <summary>DIB.pas 792-793 —— var FPaletteManager: TPaletteManager。</summary>
    private static TPaletteManager FPaletteManager;

    /// <summary>DIB.pas 795-800 1:1（惰性创建）。</summary>
    public static TPaletteManager PaletteManager()
    {
        if (FPaletteManager == null)
            FPaletteManager = new TPaletteManager();
        return FPaletteManager;
    }

    /// <summary>DIB.pas 8745 finalization —— FPaletteManager.Free。测试隔离亦用。</summary>
    public static void ResetPaletteManager()
    {
        FPaletteManager?.Destroy();
        FPaletteManager = null;
    }

    // =========================================================================================
    // DIB.pas 1544-1555 —— FEmptyDIBImage / EmptyDIBImage
    // =========================================================================================

    private static TDIBSharedImage FEmptyDIBImage;

    /// <summary>DIB.pas 1547-1555 1:1（首次创建时 Reference 一次）。</summary>
    public static TDIBSharedImage EmptyDIBImage()
    {
        if (FEmptyDIBImage == null)
        {
            FEmptyDIBImage = new TDIBSharedImage();   // = TDIBSharedImage.Create
            FEmptyDIBImage.Reference();
        }
        return FEmptyDIBImage;
    }

    /// <summary>DIB.pas 8744 finalization —— FEmptyDIBImage.Free。</summary>
    public static void ResetEmptyDIBImage()
    {
        FEmptyDIBImage = null;
    }
}

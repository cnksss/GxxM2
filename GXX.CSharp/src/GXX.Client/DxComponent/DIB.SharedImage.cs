using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas 1:1 移植 —— 第 2 片：TDIBSharedImage（DIB.pas 55-91 声明 + 666-706 辅助类 + 802-1555）。
//
// 覆盖行号：
//   802-809   Create
//   811-934   NewImage（像素格式校验、BITMAPINFO 布局、DIB 分配）
//   936-956   Duplicate
//   958-1214  Compress（EncodeRLE4 960-1083 / EncodeRLE8 1085-1199）
//   1216-1350 Decompress（DecodeRLE4 1218-1292 / DecodeRLE8 1294-1335）
//   1352-1489 ReadData（BMP 头/OS2 核头/BITFIELDS/调色板/像素数据）
//   1491-1509 Destroy
//   1511-1513 FreeHandle
//   1515-1528 GetPalette
//   1530-1540 SetColorTable
//
// GDI 不可移植项（§2.3）：CreateCompatibleDC / CreateDIBSection / SelectObject / DeleteObject /
// DeleteDC / SetDIBColorTable / GlobalAlloc(GMEM_FIXED) 之外的句柄路径全部收敛到 DibSeams.IDibGdiSeam。
// =============================================================================================

/// <summary>VCL EInvalidGraphic 的托管等价（DIB.pas 1406/1434/1487/2096/2100 抛此异常）。</summary>
public class EInvalidGraphic : Exception
{
    public EInvalidGraphic(string message) : base(message) { }
}

/// <summary>VCL EInvalidGraphicOperation 的托管等价（DIB.pas 822/824/.../841/1944/1954 抛此异常）。</summary>
public class EInvalidGraphicOperation : Exception
{
    public EInvalidGraphicOperation(string message) : base(message) { }
}

/// <summary>VCL EOutOfResources 的托管等价（DIB.pas 927 抛此异常）。</summary>
public class EOutOfResources : Exception
{
    public EOutOfResources(string message) : base(message) { }
}

/// <summary>VCL EOutOfMemory 的托管等价（DIB.pas 920 OutOfMemoryError / 2133）。</summary>
public class EOutOfMemory : Exception
{
    public EOutOfMemory(string message) : base(message) { }
}

/// <summary>
/// DIB.pas 55-91 —— TDIBSharedImage。
///
/// 基类 TSharedImage 属于 DelphiX（DXClass.pas），**源码树里只有预编译 .dcu，没有 .pas**
/// （全仓 grep `TSharedImage` 只在 DIB.pas / JvGIF.pas 里被引用）。其 RefCount/Reference/Release/FreeHandle
/// 语义由 DIB.pas 的用法反推重建：Create 后 RefCount=0；Reference 自增；Release 自减，
/// 减到 &lt;=0 时 Free（对本类即 Destroy）；FreeHandle 是空实现（DIB.pas 1511-1513）。
/// </summary>
public sealed class TDIBSharedImage
{
    // ---- TSharedImage 基类语义（重建） ----
    public int RefCount;

    // ---- DIB.pas 57-77 的私有字段（C# 用 internal 让同程序集的 TDIB / TDIBSharedImage 互访） ----
    internal int FBitCount;
    /// <summary>FBitmapInfo: PBitmapInfo —— 非托管块，布局 = BITMAPINFOHEADER(40) [+3 DWORD 掩码] + TRGBQuads。</summary>
    internal IntPtr FBitmapInfo;
    internal int FBitmapInfoSize;
    internal bool FChangePalette;
    internal TRGBQuad[] FColorTable = new TRGBQuad[256];
    internal int FColorTablePos;
    internal bool FCompressed;
    internal IntPtr FDC;
    internal IntPtr FHandle;
    internal int FHeight;
    internal bool FMemoryImage;
    internal int FNextLine;
    internal IntPtr FOldHandle;
    internal IntPtr FPalette;
    internal int FPaletteCount;
    internal IntPtr FPBits;
    internal TDIBPixelFormat FPixelFormat;
    internal int FSize;
    internal IntPtr FTopPBits;
    internal int FWidth;
    internal int FWidthBytes;

    /// <summary>托管侧簿记：FPBits 当前的分配容量（原文靠 ReAllocMem 隐式保存，无对应字段）。</summary>
    private int FBitsCapacity;

    // =========================================================================================
    // 指针助手（原文用 PArrayXxx(Integer(P)^[I]) 做指针算术，这里保留同样的算术）
    // =========================================================================================

    internal unsafe byte* Bits => (byte*)FPBits;
    internal unsafe byte* TopBits => (byte*)FTopPBits;
    internal unsafe byte* Line(int y) => (byte*)FTopPBits + (long)y * FNextLine;

    // =========================================================================================
    // BITMAPINFO 头字段访问（原文直接写 FBitmapInfo^.bmiHeader.xxx）
    // =========================================================================================

    internal unsafe uint BiSize { get => *(uint*)FBitmapInfo; set => *(uint*)FBitmapInfo = value; }
    internal unsafe int BiWidth { get => *(int*)((byte*)FBitmapInfo + 4); set => *(int*)((byte*)FBitmapInfo + 4) = value; }
    internal unsafe int BiHeight { get => *(int*)((byte*)FBitmapInfo + 8); set => *(int*)((byte*)FBitmapInfo + 8) = value; }
    internal unsafe ushort BiPlanes { get => *(ushort*)((byte*)FBitmapInfo + 12); set => *(ushort*)((byte*)FBitmapInfo + 12) = value; }
    internal unsafe ushort BiBitCount { get => *(ushort*)((byte*)FBitmapInfo + 14); set => *(ushort*)((byte*)FBitmapInfo + 14) = value; }
    internal unsafe uint BiCompression { get => *(uint*)((byte*)FBitmapInfo + 16); set => *(uint*)((byte*)FBitmapInfo + 16) = value; }
    internal unsafe uint BiSizeImage { get => *(uint*)((byte*)FBitmapInfo + 20); set => *(uint*)((byte*)FBitmapInfo + 20) = value; }
    internal unsafe int BiXPelsPerMeter { get => *(int*)((byte*)FBitmapInfo + 24); set => *(int*)((byte*)FBitmapInfo + 24) = value; }
    internal unsafe int BiYPelsPerMeter { get => *(int*)((byte*)FBitmapInfo + 28); set => *(int*)((byte*)FBitmapInfo + 28) = value; }
    internal unsafe uint BiClrUsed { get => *(uint*)((byte*)FBitmapInfo + 32); set => *(uint*)((byte*)FBitmapInfo + 32) = value; }
    internal unsafe uint BiClrImportant { get => *(uint*)((byte*)FBitmapInfo + 36); set => *(uint*)((byte*)FBitmapInfo + 36) = value; }

    /// <summary>测试/诊断用：按偏移直读 BITMAPINFO 原始字节。</summary>
    public byte ReadBitmapInfoByte(int offset)
    {
        if (FBitmapInfo == IntPtr.Zero || offset < 0 || offset >= FBitmapInfoSize) return 0;
        return Marshal.ReadByte(FBitmapInfo, offset);
    }

    /// <summary>测试/诊断用：BITMAPINFO 原始字节快照。</summary>
    public byte[] BitmapInfoBytes()
    {
        if (FBitmapInfo == IntPtr.Zero || FBitmapInfoSize <= 0) return Array.Empty<byte>();
        var b = new byte[FBitmapInfoSize];
        Marshal.Copy(FBitmapInfo, b, 0, FBitmapInfoSize);
        return b;
    }

    // =========================================================================================
    // TSharedImage：Reference / Release
    // =========================================================================================

    /// <summary>DIB.pas 1552 / 1640 / 2224 处的 `.Reference`。</summary>
    public void Reference() => RefCount++;

    /// <summary>DIB.pas 2222 处的 `.Release`（TSharedImage 语义：减到 &lt;=0 即 Free）。</summary>
    public void Release()
    {
        RefCount--;
        if (RefCount <= 0) Destroy();
    }

    // =========================================================================================
    // DIB.pas 802-809 —— constructor TDIBSharedImage.Create
    // =========================================================================================

    public TDIBSharedImage() => Create();

    /// <summary>
    /// DIB.pas 802-809。
    /// 原文 `NewImage` 的第一句就是 `Create;`（在已构造实例上重跑构造函数体），
    /// 这里用同名实例方法复现；原文重跑会泄漏 FPBits/FBitmapInfo，托管侧显式释放（唯一偏差，行为不可观察）。
    /// </summary>
    public void Create()
    {
        FreeBuffersOnRecreate();
        FMemoryImage = true;
        // SetColorTable(GreyscaleColorTable);
        SetColorTable(DIB.GreyscaleColorTable());
        FColorTable = DIB.GreyscaleColorTable();
        FPixelFormat = DIB.MakeDIBPixelFormat(8, 8, 8);
    }

    /// <summary>原文重跑 Create 不释放旧块（泄漏）；托管侧必须显式释放，否则 AllocHGlobal 泄漏。</summary>
    private void FreeBuffersOnRecreate()
    {
        if (FPBits != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(FPBits);
            FPBits = IntPtr.Zero;
            FTopPBits = IntPtr.Zero;
            FBitsCapacity = 0;
        }
        if (FBitmapInfo != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(FBitmapInfo);
            FBitmapInfo = IntPtr.Zero;
            FBitmapInfoSize = 0;
        }
    }

    // =========================================================================================
    // DIB.pas 811-934 —— NewImage
    // =========================================================================================

    /// <summary>
    /// DIB.pas 811-934 1:1。
    /// 位深校验（1/4/8 必须 $FF0000/$00FF00/$0000FF；16 允许 5:5:5 或 5:6:5；24/32 必须 8:8:8），
    /// 否则抛 EInvalidGraphicOperation(SInvalidDIBPixelFormat)；未知位深抛 SInvalidDIBBitCount。
    /// FWidthBytes = (((AWidth*ABitCount)+31) shr 5) * 4（4 字节行对齐）；FNextLine = -FWidthBytes；
    /// FSize = FWidthBytes*FHeight；UsePixelFormat = ABitCount in [16,32]；
    /// FPaletteCount = 1 shl FBitCount（仅 &lt;=8）；FBitmapInfoSize = 40 + (UsePixelFormat?12:0) + 4*FPaletteCount；
    /// FColorTablePos = 40 + (UsePixelFormat?12:0)。
    /// </summary>
    public void NewImage(int AWidth, int AHeight, int ABitCount, in TDIBPixelFormat PixelFormat,
        TRGBQuad[] ColorTable, bool MemoryImage, bool Compressed)
    {
        int InfoOfs;
        bool UsePixelFormat;

        Create();

        // Pixel format check
        switch (ABitCount)
        {
            case 1:
                if (!(PixelFormat.RBitMask == 0xFF0000 && PixelFormat.GBitMask == 0x00FF00 && PixelFormat.BBitMask == 0x0000FF))
                    throw new EInvalidGraphicOperation(DXConsts.SInvalidDIBPixelFormat);
                break;
            case 4:
                if (!(PixelFormat.RBitMask == 0xFF0000 && PixelFormat.GBitMask == 0x00FF00 && PixelFormat.BBitMask == 0x0000FF))
                    throw new EInvalidGraphicOperation(DXConsts.SInvalidDIBPixelFormat);
                break;
            case 8:
                if (!(PixelFormat.RBitMask == 0xFF0000 && PixelFormat.GBitMask == 0x00FF00 && PixelFormat.BBitMask == 0x0000FF))
                    throw new EInvalidGraphicOperation(DXConsts.SInvalidDIBPixelFormat);
                break;
            case 16:
                if (!((PixelFormat.RBitMask == 0x7C00 && PixelFormat.GBitMask == 0x03E0 && PixelFormat.BBitMask == 0x001F) ||
                      (PixelFormat.RBitMask == 0xF800 && PixelFormat.GBitMask == 0x07E0 && PixelFormat.BBitMask == 0x001F)))
                    throw new EInvalidGraphicOperation(DXConsts.SInvalidDIBPixelFormat);
                break;
            case 24:
                if (!(PixelFormat.RBitMask == 0xFF0000 && PixelFormat.GBitMask == 0x00FF00 && PixelFormat.BBitMask == 0x0000FF))
                    throw new EInvalidGraphicOperation(DXConsts.SInvalidDIBPixelFormat);
                break;
            case 32:
                if (!(PixelFormat.RBitMask == 0xFF0000 && PixelFormat.GBitMask == 0x00FF00 && PixelFormat.BBitMask == 0x0000FF))
                    throw new EInvalidGraphicOperation(DXConsts.SInvalidDIBPixelFormat);
                break;
            default:
                throw new EInvalidGraphicOperation(string.Format(DXConsts.SInvalidDIBBitCount, ABitCount));
        }

        FBitCount = ABitCount;
        FHeight = AHeight;
        FWidth = AWidth;
        FWidthBytes = (((AWidth * ABitCount) + 31) >> 5) * 4;
        FNextLine = -FWidthBytes;
        FSize = FWidthBytes * FHeight;
        UsePixelFormat = ABitCount == 16 || ABitCount == 32;

        FPixelFormat = PixelFormat;

        FPaletteCount = 0;
        if (FBitCount <= 8)
            FPaletteCount = 1 << FBitCount;

        FBitmapInfoSize = DIB.SizeOfBitmapInfoHeader;
        if (UsePixelFormat)
            FBitmapInfoSize += 12;   // SizeOf(TLocalDIBPixelFormat) = 3 * DWORD
        FBitmapInfoSize += DIB.SizeOfTRGBQuad * FPaletteCount;

        FBitmapInfo = Marshal.AllocHGlobal(FBitmapInfoSize);
        for (int i = 0; i < FBitmapInfoSize; i++) Marshal.WriteByte(FBitmapInfo, i, 0); // FillChar(..., 0)

        // BitmapInfo setting.
        BiSize = DIB.SizeOfBitmapInfoHeader;
        BiWidth = FWidth;
        BiHeight = FHeight;
        BiPlanes = 1;
        BiBitCount = (ushort)FBitCount;
        if (UsePixelFormat)
            BiCompression = DIB.BI_BITFIELDS;
        else
        {
            if (FBitCount == 4 && Compressed)
                BiCompression = DIB.BI_RLE4;
            else if (FBitCount == 8 && Compressed)
                BiCompression = DIB.BI_RLE8;
            else
                BiCompression = DIB.BI_RGB;
        }
        BiSizeImage = (uint)FSize;
        BiXPelsPerMeter = 0;
        BiYPelsPerMeter = 0;
        BiClrUsed = 0;
        BiClrImportant = 0;

        InfoOfs = DIB.SizeOfBitmapInfoHeader;

        if (UsePixelFormat)
        {
            Marshal.WriteInt32(FBitmapInfo, InfoOfs + 0, unchecked((int)PixelFormat.RBitMask));
            Marshal.WriteInt32(FBitmapInfo, InfoOfs + 4, unchecked((int)PixelFormat.GBitMask));
            Marshal.WriteInt32(FBitmapInfo, InfoOfs + 8, unchecked((int)PixelFormat.BBitMask));

            InfoOfs += 12;
        }

        FColorTablePos = InfoOfs;

        FColorTable = ColorTable;
        WriteColorTableToBitmapInfo();

        FCompressed = BiCompression == DIB.BI_RLE4 || BiCompression == DIB.BI_RLE8;
        FMemoryImage = MemoryImage || FCompressed;

        // DIB making.
        if (!Compressed)
        {
            if (MemoryImage)
            {
                // FPBits := Pointer(GlobalAlloc(GMEM_FIXED, FSize));
                FPBits = FSize > 0 ? Marshal.AllocHGlobal(FSize) : IntPtr.Zero;
                FBitsCapacity = FSize;
                if (FPBits == IntPtr.Zero) throw new EOutOfMemory("Out of memory");
                ZeroBits(FSize);
            }
            else
            {
                // 接缝：GDI 路径（CreateCompatibleDC → CreateDIBSection → SelectObject）不翻译实现（§2.3）。
                // 未装载 DibSeams.Gdi 时退化为内存位图，并保持 FMemoryImage 语义（=False）以便调用方
                // 仍走 AllocHandle() → Duplicate(FImage, False) 的"要 GDI 句柄"分支。
                if (DibSeams.Gdi != null)
                {
                    FDC = DibSeams.Gdi.CreateCompatibleDC();
                    FHandle = DibSeams.Gdi.CreateDIBSection(FDC, FBitmapInfo, DIB.DIB_RGB_COLORS, out FPBits, IntPtr.Zero, 0);
                    if (FHandle == IntPtr.Zero)
                        throw new EOutOfResources(string.Format(DXConsts.SCannotMade, "DIB"));
                    FOldHandle = DibSeams.Gdi.SelectObject(FDC, FHandle);
                }
                else
                {
                    FDC = IntPtr.Zero;
                    FHandle = IntPtr.Zero;
                    FOldHandle = IntPtr.Zero;
                    FPBits = FSize > 0 ? Marshal.AllocHGlobal(FSize) : IntPtr.Zero;
                    FBitsCapacity = FSize;
                    if (FPBits == IntPtr.Zero) throw new EOutOfMemory("Out of memory");
                    ZeroBits(FSize);
                }
            }
        }

        FTopPBits = IntPtr.Add(FPBits, (FHeight - 1) * FWidthBytes);
    }

    private void ZeroBits(int size)
    {
        if (FPBits != IntPtr.Zero && size > 0)
            unsafe { new Span<byte>((void*)FPBits, size).Clear(); }
    }

    /// <summary>DIB.pas 908 —— Move(FColorTable, FBitmapInfo + FColorTablePos, 4*FPaletteCount)。</summary>
    private void WriteColorTableToBitmapInfo()
    {
        if (FBitmapInfo == IntPtr.Zero || FPaletteCount <= 0) return;
        var bytes = DIB.ColorTableToBytes(FColorTable, FPaletteCount);
        Marshal.Copy(bytes, 0, IntPtr.Add(FBitmapInfo, FColorTablePos), bytes.Length);
    }

    // =========================================================================================
    // DIB.pas 936-956 —— Duplicate
    // =========================================================================================

    /// <summary>
    /// DIB.pas 936-956 1:1。
    /// 注意原文不一致：FCompressed 分支虽然把 biSizeImage 改成 Source 的值，
    /// 却**没有**同步 FSize（FSize 仍是 NewImage 算出的未压缩尺寸）—— 原文如此（DIB.pas:946-950）。
    /// </summary>
    public void Duplicate(TDIBSharedImage Source, bool MemoryImage)
    {
        if (Source.FSize == 0)
        {
            Create();
            FMemoryImage = MemoryImage;
        }
        else
        {
            NewImage(Source.FWidth, Source.FHeight, Source.FBitCount,
                Source.FPixelFormat, Source.FColorTable, MemoryImage, Source.FCompressed);
            if (FCompressed)
            {
                BiSizeImage = Source.BiSizeImage;
                AllocBitsExact((int)BiSizeImage);
                CopyBits(Source.FPBits, FPBits, (int)BiSizeImage);
            }
            else
            {
                CopyBits(Source.FPBits, FPBits, (int)BiSizeImage);
            }
        }
    }

    private static void CopyBits(IntPtr src, IntPtr dst, int count)
    {
        if (count <= 0 || src == IntPtr.Zero || dst == IntPtr.Zero) return;
        unsafe { Buffer.MemoryCopy((void*)src, (void*)dst, count, count); }
    }

    /// <summary>GetMem(FPBits, size)：原文不复用旧块（泄漏），此处先释放（偏差不可观察）。</summary>
    private void AllocBitsExact(int size)
    {
        if (FPBits != IntPtr.Zero) Marshal.FreeHGlobal(FPBits);
        FPBits = size > 0 ? Marshal.AllocHGlobal(size) : IntPtr.Zero;
        FBitsCapacity = size;
        ZeroBits(size);
    }

    // =========================================================================================
    // DIB.pas 958-1214 —— Compress（RLE4 / RLE8 编码）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 958-1214 1:1。
    /// Source 已压缩 → 直接 Duplicate；否则按 NewImage(..., MemoryImage:=True, Compressed:=True) 建目标，
    /// 再按 biCompression 选 EncodeRLE4 / EncodeRLE8；两者都不匹配 → Duplicate。
    /// </summary>
    public void Compress(TDIBSharedImage Source)
    {
        if (Source.FCompressed)
            Duplicate(Source, Source.FMemoryImage);
        else
        {
            NewImage(Source.FWidth, Source.FHeight, Source.FBitCount,
                Source.FPixelFormat, Source.FColorTable, true, true);
            switch (BiCompression)
            {
                case DIB.BI_RLE4: EncodeRLE4(Source); break;
                case DIB.BI_RLE8: EncodeRLE8(Source); break;
                default: Duplicate(Source, Source.FMemoryImage); break;
            }
        }
    }

    /// <summary>x86 非托管分配的 32 位地址（原文 Longint(P) 截断语义）。</summary>
    private static unsafe int Addr(IntPtr p) => unchecked((int)(long)p);

    /// <summary>原文 AllocByte 的 ReAllocMem 扩容语义（Size mod 4096 = 0 时增容到 Size+4095）。</summary>
    private unsafe void GrowBits(int newCap)
    {
        IntPtr np = Marshal.AllocHGlobal(newCap);
        if (FPBits != IntPtr.Zero && FBitsCapacity > 0)
            Buffer.MemoryCopy((void*)FPBits, (void*)np, newCap, Math.Min(FBitsCapacity, newCap));
        if (FPBits != IntPtr.Zero) Marshal.FreeHGlobal(FPBits);
        FPBits = np;
        FBitsCapacity = newCap;
    }

    /// <summary>
    /// DIB.pas 960-1083 —— 内嵌 EncodeRLE4。
    /// 原文笔误照抄1：行首指针用 `Y * FWidthBytes`（**Self** 的 FWidthBytes，不是 Source 的），
    /// 这里利用 NewImage 已按 Source 尺寸建立 Self，故两者相等（DIB.pas:992）。
    /// 原文笔误照抄2：绝对模式下 `GetPixel(X+2)/GetPixel(X+3)` 在 X 越界时仍被读取（无守卫）。
    /// </summary>
    private unsafe void EncodeRLE4(TDIBSharedImage Source)
    {
        int Size = 0;

        byte* AllocByte()
        {
            if (Size % 4096 == 0) GrowBits(Size + 4095);
            byte* r = (byte*)FPBits + Size;
            Size++;
            return r;
        }

        byte* Src = null;
        int GetPixel(int X)
        {
            // DIB.pas 978-983：偶数位取高半字节，奇数位取低半字节
            if ((X & 1) == 0)
                return Src[X >> 1] >> 4;
            else
                return Src[X >> 1] & 0x0F;
        }

        byte B1, B2, C;
        int PB1, PB2;
        int X, Y;

        for (Y = 0; Y <= Source.FHeight - 1; Y++)
        {
            X = 0;
            Src = (byte*)Source.FPBits + Y * FWidthBytes;   // 原文如此（用 Self 的 FWidthBytes）
            while (X < Source.FWidth)
            {
                if ((Source.FWidth - X > 3) && (GetPixel(X) == GetPixel(X + 2)))
                {
                    // Encoding mode
                    B1 = 2;
                    B2 = (byte)((GetPixel(X) << 4) | GetPixel(X + 1));

                    X += 2;

                    C = B2;

                    while ((X < Source.FWidth) && ((C & 0xF) == GetPixel(X)) && (B1 < 255))
                    {
                        B1++;
                        X++;
                        C = (byte)((C >> 4) | (C << 4));
                    }

                    *AllocByte() = B1;
                    *AllocByte() = B2;
                }
                else if ((Source.FWidth - X > 5) && ((GetPixel(X) != GetPixel(X + 2)) || (GetPixel(X + 1) != GetPixel(X + 3)))
                    && ((GetPixel(X + 2) == GetPixel(X + 4)) && (GetPixel(X + 3) == GetPixel(X + 5))))
                {
                    // Encoding mode
                    *AllocByte() = 2;
                    *AllocByte() = (byte)((GetPixel(X) << 4) | GetPixel(X + 1));
                    X += 2;
                }
                else
                {
                    if (Source.FWidth - X < 4)
                    {
                        // Encoding mode
                        while (Source.FWidth - X >= 2)
                        {
                            *AllocByte() = 2;
                            *AllocByte() = (byte)((GetPixel(X) << 4) | GetPixel(X + 1));
                            X += 2;
                        }

                        if (Source.FWidth - X == 1)
                        {
                            *AllocByte() = 1;
                            *AllocByte() = (byte)(GetPixel(X) << 4);
                            X++;
                        }
                    }
                    else
                    {
                        // Absolute mode
                        PB1 = Size; AllocByte();
                        PB2 = Size; AllocByte();

                        B1 = 0;
                        B2 = 4;

                        *AllocByte() = (byte)((GetPixel(X) << 4) | GetPixel(X + 1));
                        *AllocByte() = (byte)((GetPixel(X + 2) << 4) | GetPixel(X + 3));

                        X += 4;

                        while ((X + 1 < Source.FWidth) && (B2 < 254))
                        {
                            if ((Source.FWidth - X > 3) && (GetPixel(X) == GetPixel(X + 2)) && (GetPixel(X + 1) == GetPixel(X + 3)))
                                break;

                            *AllocByte() = (byte)((GetPixel(X) << 4) | GetPixel(X + 1));
                            B2 += 2;
                            X += 2;
                        }

                        *((byte*)FPBits + PB1) = B1;
                        *((byte*)FPBits + PB2) = B2;
                    }
                }

                if ((Size & 1) == 1) AllocByte();
            }

            // End of line
            *AllocByte() = 0;
            *AllocByte() = 0;
        }

        // End of bitmap
        *AllocByte() = 0;
        *AllocByte() = 1;

        BiSizeImage = (uint)Size;
        FSize = Size;
    }

    /// <summary>
    /// DIB.pas 1085-1199 —— 内嵌 EncodeRLE8。
    /// 与 EncodeRLE4 同构；注意 `if Size and 1 = 1 then AllocByte` 的字节对齐补位位置在**每段之后**（原文如此）。
    /// </summary>
    private unsafe void EncodeRLE8(TDIBSharedImage Source)
    {
        int Size = 0;

        byte* AllocByte()
        {
            if (Size % 4096 == 0) GrowBits(Size + 4095);
            byte* r = (byte*)FPBits + Size;
            Size++;
            return r;
        }

        byte B1, B2;
        int PB1, PB2;
        byte* Src;
        int X, Y;

        for (Y = 0; Y <= Source.FHeight - 1; Y++)
        {
            X = 0;
            Src = (byte*)Source.FPBits + Y * FWidthBytes;   // 原文如此（用 Self 的 FWidthBytes）
            while (X < Source.FWidth)
            {
                if ((Source.FWidth - X > 2) && (Src[0] == Src[1]))
                {
                    // Encoding mode
                    B1 = 2;
                    B2 = Src[0];

                    X += 2;
                    Src += 2;

                    while ((X < Source.FWidth) && (Src[0] == B2) && (B1 < 255))
                    {
                        B1++;
                        X++;
                        Src++;
                    }

                    *AllocByte() = B1;
                    *AllocByte() = B2;
                }
                else if ((Source.FWidth - X > 2) && (Src[0] != Src[1]) && (Src[1] == Src[2]))
                {
                    // Encoding mode
                    *AllocByte() = 1;
                    *AllocByte() = Src[0]; Src++;
                    X++;
                }
                else
                {
                    if (Source.FWidth - X < 4)
                    {
                        // Encoding mode
                        if (Source.FWidth - X == 2)
                        {
                            *AllocByte() = 1;
                            *AllocByte() = Src[0]; Src++;

                            *AllocByte() = 1;
                            *AllocByte() = Src[0]; Src++;
                            X += 2;
                        }
                        else
                        {
                            *AllocByte() = 1;
                            *AllocByte() = Src[0]; Src++;
                            X++;
                        }
                    }
                    else
                    {
                        // Absolute mode
                        PB1 = Size; AllocByte();
                        PB2 = Size; AllocByte();

                        B1 = 0;
                        B2 = 3;

                        X += 3;

                        *AllocByte() = Src[0]; Src++;
                        *AllocByte() = Src[0]; Src++;
                        *AllocByte() = Src[0]; Src++;

                        while ((X < Source.FWidth) && (B2 < 255))
                        {
                            if ((Source.FWidth - X > 3) && (Src[0] == Src[1]) && (Src[0] == Src[2]) && (Src[0] == Src[3]))
                                break;

                            *AllocByte() = Src[0]; Src++;
                            B2++;
                            X++;
                        }

                        *((byte*)FPBits + PB1) = B1;
                        *((byte*)FPBits + PB2) = B2;
                    }
                }

                if ((Size & 1) == 1) AllocByte();
            }

            // End of line
            *AllocByte() = 0;
            *AllocByte() = 0;
        }

        // End of bitmap
        *AllocByte() = 0;
        *AllocByte() = 1;

        BiSizeImage = (uint)Size;
        FSize = Size;
    }

    // =========================================================================================
    // DIB.pas 1216-1350 —— Decompress（RLE4 / RLE8 解码）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1216-1350 1:1。
    /// 未压缩 → Duplicate；否则 NewImage(..., Compressed:=False) 后按 Source 的 biCompression 解码。
    /// </summary>
    public void Decompress(TDIBSharedImage Source, bool MemoryImage)
    {
        if (!Source.FCompressed)
            Duplicate(Source, MemoryImage);
        else
        {
            NewImage(Source.FWidth, Source.FHeight, Source.FBitCount,
                Source.FPixelFormat, Source.FColorTable, MemoryImage, false);
            switch (Source.BiCompression)
            {
                case DIB.BI_RLE4: DecodeRLE4(Source); break;
                case DIB.BI_RLE8: DecodeRLE8(Source); break;
                default: Duplicate(Source, MemoryImage); break;
            }
        }
    }

    /// <summary>
    /// DIB.pas 1218-1292 —— 内嵌 DecodeRLE4。
    /// 原文笔误照抄：`2: begin Inc(X, B1); Inc(Y, B2); Inc(Src, 2); end`
    /// —— 此时 B1=0、B2=2，所以实际是 Inc(X,0) / Inc(Y,2)，**不是**按后续两字节增量移动
    /// （正确写法应再读两个字节）。原文如此（DIB.pas:1241-1244）。
    /// </summary>
    private unsafe void DecodeRLE4(TDIBSharedImage Source)
    {
        byte B1, B2, C;
        byte* Dest, Src, P;
        int X, Y, I;

        Src = (byte*)Source.FPBits;
        X = 0;
        Y = 0;

        while (true)
        {
            B1 = Src[0]; Src++;
            B2 = Src[0]; Src++;

            if (B1 == 0)
            {
                if (B2 == 0)
                {
                    // End of line
                    X = 0;
                    Y++;
                }
                else if (B2 == 1)
                {
                    break; // End of bitmap
                }
                else if (B2 == 2)
                {
                    // Difference of coordinates —— 原文如此（B1/B2 此时是 0 与 2）
                    X += B1;
                    Y += B2; Src += 2;
                }
                else
                {
                    // Absolute mode
                    Dest = (byte*)FPBits + (long)Y * FWidthBytes;

                    C = 0;
                    for (I = 0; I <= B2 - 1; I++)
                    {
                        if ((I & 1) == 0)
                        {
                            C = Src[0]; Src++;
                        }
                        else
                        {
                            C = (byte)(C << 4);
                        }

                        P = Dest + (X >> 1);
                        if ((X & 1) == 0)
                            P[0] = (byte)((P[0] & 0x0F) | (C & 0xF0));
                        else
                            P[0] = (byte)((P[0] & 0xF0) | ((C & 0xF0) >> 4));

                        X++;
                    }
                }
            }
            else
            {
                // Encoding mode
                Dest = (byte*)FPBits + (long)Y * FWidthBytes;

                for (I = 0; I <= B1 - 1; I++)
                {
                    P = Dest + (X >> 1);
                    if ((X & 1) == 0)
                        P[0] = (byte)((P[0] & 0x0F) | (B2 & 0xF0));
                    else
                        P[0] = (byte)((P[0] & 0xF0) | ((B2 & 0xF0) >> 4));

                    X++;

                    // Swap nibble
                    B2 = (byte)((B2 >> 4) | (B2 << 4));
                }
            }

            // Word arrangement —— 原文按**绝对地址**奇偶对齐（DIB.pas:1290）
            Src += Addr((IntPtr)Src) & 1;
        }
    }

    /// <summary>
    /// DIB.pas 1294-1335 —— 内嵌 DecodeRLE8。
    /// 同样照抄 `2:` 分支的 B1/B2 误用（DIB.pas:1318-1321）。
    /// </summary>
    private unsafe void DecodeRLE8(TDIBSharedImage Source)
    {
        byte B1, B2;
        byte* Dest, Src;
        int X, Y;

        Dest = (byte*)FPBits;
        Src = (byte*)Source.FPBits;
        X = 0;
        Y = 0;

        while (true)
        {
            B1 = Src[0]; Src++;
            B2 = Src[0]; Src++;

            if (B1 == 0)
            {
                if (B2 == 0)
                {
                    // End of line
                    X = 0; Y++;
                    Dest = (byte*)FPBits + (long)Y * FWidthBytes + X;
                }
                else if (B2 == 1)
                {
                    break; // End of bitmap
                }
                else if (B2 == 2)
                {
                    // Difference of coordinates —— 原文如此
                    X += B1; Y += B2; Src += 2;
                    Dest = (byte*)FPBits + (long)Y * FWidthBytes + X;
                }
                else
                {
                    // Absolute mode
                    for (int i = 0; i < B2; i++) Dest[i] = Src[i];
                    Dest += B2; Src += B2;
                }
            }
            else
            {
                // Encoding mode
                for (int i = 0; i < B1; i++) Dest[i] = B2;
                Dest += B1;
            }

            // Word arrangement
            Src += Addr((IntPtr)Src) & 1;
        }
    }

    // =========================================================================================
    // DIB.pas 1352-1489 —— ReadData（BMP/DIB 字节流解析）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1352-1489 1:1。
    /// 首 4 字节 = biSize：0 → 空图；12 → OS/2 核头；40 → Windows 头；其它 → EInvalidGraphic(SInvalidDIB)。
    /// biCompression = BI_BITFIELDS(3) → 再读 12 字节三掩码并 MakeDIBPixelFormatMask；
    /// 否则 16bpp → 5:5:5，32bpp → 8:8:8，其余 → 8:8:8（原文的三段 else-if）。
    /// 调色板块数：biClrUsed；为 0 且位深 &lt;=8 时取 1 shl 位深；上限 256。
    /// OS/2 调色板是 TRGBTriple（3 字节）→ RGBQuad(rgbtRed, rgbtGreen, rgbtBlue)。
    /// 像素读取：BI_RGB/BI_BITFIELDS → LoadRGB；BI_RLE4/BI_RLE8 → 读 biSizeImage 字节。
    /// </summary>
    public void ReadData(Stream Stream, bool MemoryImage)
    {
        byte[] BI = new byte[DIB.SizeOfBitmapInfoHeader];
        byte[] BC = new byte[DIB.SizeOfBitmapCoreHeader];
        TRGBQuad[] AColorTable = new TRGBQuad[256];   // FillChar(AColorTable, 0) 等价于 new（全 0）

        int I, PalCount;
        bool OS2;
        TDIBPixelFormat APixelFormat = default;

        // Header size reading
        I = Stream.Read(BI, 0, 4);

        if (I == 0)
        {
            Create();
            return;
        }
        if (I != 4)
            throw new EInvalidGraphic(DXConsts.SInvalidDIB);

        // Kind check of DIB
        OS2 = false;
        uint biSize = BitConverter.ToUInt32(BI, 0);

        if (biSize == DIB.SizeOfBitmapCoreHeader)
        {
            // OS/2 type
            ReadBufferAt(Stream, BC, 4, DIB.SizeOfBitmapCoreHeader - 4);

            // with BI do begin biClrUsed := 0; biCompression := BI_RGB; biBitCount := BC.bcBitCount;
            //   biHeight := BC.bcHeight; biWidth := BC.bcWidth; end;
            // 注意：其余字段（biSizeImage 等）在原文里是**未初始化的栈内容**，此处保持 0（不可观察差异，
            // 因为 OS/2 头必然走 BI_RGB 分支，LoadRGB 用的是 FSize 而不是 biSizeImage）。
            ushort bcWidth = BitConverter.ToUInt16(BC, 4);
            ushort bcHeight = BitConverter.ToUInt16(BC, 6);
            ushort bcBitCount = BitConverter.ToUInt16(BC, 10);

            WriteI32(BI, 32, 0);                       // biClrUsed := 0
            WriteI32(BI, 16, (int)DIB.BI_RGB);         // biCompression := BI_RGB
            WriteU16(BI, 14, bcBitCount);              // biBitCount
            WriteI32(BI, 8, bcHeight);                 // biHeight
            WriteI32(BI, 4, bcWidth);                  // biWidth

            OS2 = true;
        }
        else if (biSize == DIB.SizeOfBitmapInfoHeader)
        {
            // Windows type
            ReadBufferAt(Stream, BI, 4, DIB.SizeOfBitmapInfoHeader - 4);
        }
        else
            throw new EInvalidGraphic(DXConsts.SInvalidDIB);

        uint biCompression = (uint)ReadI32(BI, 16);
        ushort biBitCount = (ushort)ReadU16(BI, 14);
        int biWidth = ReadI32(BI, 4);
        int biHeight = ReadI32(BI, 8);
        uint biClrUsed = (uint)ReadI32(BI, 32);
        uint biSizeImage = (uint)ReadI32(BI, 20);

        // Bit mask reading.
        if (biCompression == DIB.BI_BITFIELDS)
        {
            var localpf = new byte[12];
            ReadBufferAt(Stream, localpf, 0, 12);
            APixelFormat = DIB.MakeDIBPixelFormatMask(
                BitConverter.ToInt32(localpf, 0),
                BitConverter.ToInt32(localpf, 4),
                BitConverter.ToInt32(localpf, 8));
        }
        else
        {
            if (biBitCount == 16)
                APixelFormat = DIB.MakeDIBPixelFormat(5, 5, 5);
            else if (biBitCount == 32)
                APixelFormat = DIB.MakeDIBPixelFormat(8, 8, 8);
            else
                APixelFormat = DIB.MakeDIBPixelFormat(8, 8, 8);
        }

        // Palette reading
        PalCount = (int)biClrUsed;
        if (PalCount == 0 && biBitCount <= 8)
            PalCount = 1 << biBitCount;
        if (PalCount > 256) PalCount = 256;

        if (OS2)
        {
            // OS/2 type：TRGBTriple 序列
            var bc = new byte[DIB.SizeOfTRGBTriple * PalCount];
            ReadBufferAt(Stream, bc, 0, bc.Length);
            for (I = 0; I <= PalCount - 1; I++)
            {
                byte rgbtBlue = bc[I * 3 + 0];
                byte rgbtGreen = bc[I * 3 + 1];
                byte rgbtRed = bc[I * 3 + 2];
                AColorTable[I] = DIB.RGBQuad(rgbtRed, rgbtGreen, rgbtBlue);
            }
        }
        else
        {
            // Windows type：TRGBQuad 序列
            var rb = new byte[DIB.SizeOfTRGBQuad * PalCount];
            ReadBufferAt(Stream, rb, 0, rb.Length);
            for (I = 0; I < PalCount; I++)
            {
                AColorTable[I].rgbBlue = rb[I * 4 + 0];
                AColorTable[I].rgbGreen = rb[I * 4 + 1];
                AColorTable[I].rgbRed = rb[I * 4 + 2];
                AColorTable[I].rgbReserved = rb[I * 4 + 3];
            }
        }

        // DIB compilation
        NewImage(biWidth, Math.Abs(biHeight), biBitCount, APixelFormat, AColorTable,
            MemoryImage, biCompression == DIB.BI_RLE4 || biCompression == DIB.BI_RLE8);

        // Pixel data reading
        switch (biCompression)
        {
            case DIB.BI_RGB:
                LoadRGB(Stream, biHeight, biSizeImage);
                break;
            case DIB.BI_RLE4:
                LoadRLE(Stream, biSizeImage);
                break;
            case DIB.BI_RLE8:
                LoadRLE(Stream, biSizeImage);
                break;
            case DIB.BI_BITFIELDS:
                LoadRGB(Stream, biHeight, biSizeImage);
                break;
            default:
                throw new EInvalidGraphic(DXConsts.SInvalidDIB);
        }
    }

    /// <summary>DIB.pas 1376-1388 —— LoadRGB。biHeight &lt; 0（top-down）逐行读到 TopPBits + Y*FNextLine，否则整块读。</summary>
    private void LoadRGB(Stream Stream, int biHeight, uint biSizeImage)
    {
        if (biHeight < 0)
        {
            for (int Y = 0; Y <= Math.Abs(biHeight) - 1; Y++)
            {
                ReadBufferAt(Stream, FTopPBits, Y * FNextLine, FWidthBytes);
            }
        }
        else
        {
            ReadBufferAt(Stream, FPBits, 0, FSize);
        }
    }

    /// <summary>DIB.pas 1358-1374 —— LoadRLE4 / LoadRLE8（两者代码完全一致）。</summary>
    private void LoadRLE(Stream Stream, uint biSizeImage)
    {
        FSize = (int)biSizeImage;
        // FPBits := GlobalAllocPtr(GMEM_FIXED, FSize);
        AllocBitsExact(FSize);
        BiSizeImage = (uint)FSize;
        ReadBufferAt(Stream, FPBits, 0, FSize);
    }

    // ---- 流读助手（Delphi TStream.Read / ReadBuffer 语义） ----

    private static void ReadBufferAt(Stream s, byte[] dst, int offset, int count)
    {
        if (count <= 0) return;
        int read = 0;
        while (read < count)
        {
            int n = s.Read(dst, offset + read, count - read);
            if (n <= 0) throw new EInvalidGraphic(DXConsts.SInvalidDIB); // ReadBuffer 短读即抛
            read += n;
        }
    }

    private static void ReadBufferAt(Stream s, IntPtr dst, int offset, int count)
    {
        if (count <= 0) return;
        var tmp = new byte[count];
        ReadBufferAt(s, tmp, 0, count);
        Marshal.Copy(tmp, 0, IntPtr.Add(dst, offset), count);
    }

    private static int ReadI32(byte[] b, int ofs) => BitConverter.ToInt32(b, ofs);
    private static ushort ReadU16(byte[] b, int ofs) => BitConverter.ToUInt16(b, ofs);
    private static void WriteI32(byte[] b, int ofs, int v) => BitConverter.GetBytes(v).CopyTo(b, ofs);
    private static void WriteU16(byte[] b, int ofs, ushort v) => BitConverter.GetBytes(v).CopyTo(b, ofs);

    // =========================================================================================
    // DIB.pas 1491-1540 —— Destroy / FreeHandle / GetPalette / SetColorTable
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1491-1509 1:1（外加托管侧必须释放的非托管块；重复调用安全）。
    /// </summary>
    public void Destroy()
    {
        if (FHandle != IntPtr.Zero)
        {
            if (FOldHandle != IntPtr.Zero) DibSeams.Gdi?.SelectObject(FDC, FOldHandle);
            DibSeams.Gdi?.DeleteObject(FHandle);
        }
        else
        {
            if (FPBits != IntPtr.Zero)
            {
                // GlobalFreePtr(FPBits)
                Marshal.FreeHGlobal(FPBits);
            }
        }

        FPBits = IntPtr.Zero;
        FTopPBits = IntPtr.Zero;
        FBitsCapacity = 0;
        FHandle = IntPtr.Zero;
        FOldHandle = IntPtr.Zero;

        DIB.PaletteManager().DeletePalette(ref FPalette);
        if (FDC != IntPtr.Zero) DibSeams.Gdi?.DeleteDC(FDC);
        FDC = IntPtr.Zero;

        if (FBitmapInfo != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(FBitmapInfo);
            FBitmapInfo = IntPtr.Zero;
        }
        FBitmapInfoSize = 0;
    }

    /// <summary>DIB.pas 1511-1513 —— 空实现（原文如此）。</summary>
    public void FreeHandle() { }

    /// <summary>
    /// DIB.pas 1515-1528 1:1。
    /// FPaletteCount &gt; 0 时：若 FChangePalette 为真则清标志、先删旧调色板再重建；
    /// 否则直接返回 0。
    /// </summary>
    public IntPtr GetPalette()
    {
        if (FPaletteCount > 0)
        {
            if (FChangePalette)
            {
                FChangePalette = false;
                DIB.PaletteManager().DeletePalette(ref FPalette);
                FPalette = DIB.PaletteManager().CreatePalette(FColorTable, FPaletteCount);
            }
            return FPalette;
        }
        return IntPtr.Zero;
    }

    /// <summary>
    /// DIB.pas 1530-1540 1:1。
    /// 无条件设 FColorTable 与 FChangePalette；仅当 FSize&gt;0 且 FPaletteCount&gt;0 时
    /// 才 SetDIBColorTable（GDI 接缝）+ Move 进 BITMAPINFO。
    /// </summary>
    public void SetColorTable(TRGBQuad[] Value)
    {
        FColorTable = Value;
        FChangePalette = true;

        if (FSize > 0 && FPaletteCount > 0)
        {
            if (FDC != IntPtr.Zero)
                DibSeams.Gdi?.SetDIBColorTable(FDC, 0, 256, DIB.ColorTableToBytes(FColorTable, 256));
            WriteColorTableToBitmapInfo();
        }
    }

    // =========================================================================================
    // 测试/诊断助手（不改变语义）
    // =========================================================================================

    // ---- 字段的只读公开投影（测试/其它分片需要读，字段本身是 internal 无法跨程序集访问） ----

    public int ImgBitCount => FBitCount;
    public int ImgWidth => FWidth;
    public int ImgHeight => FHeight;
    public int ImgWidthBytes => FWidthBytes;
    public int ImgSize => FSize;
    public int ImgNextLine => FNextLine;
    public int ImgPaletteCount => FPaletteCount;
    public bool Compressed => FCompressed;
    public bool MemoryImage => FMemoryImage;
    public TDIBPixelFormat ImgPixelFormat => FPixelFormat;
    public int ColorTablePos => FColorTablePos;
    public int BitmapInfoSize => FBitmapInfoSize;
    public TRGBQuad[] ImgColorTable => FColorTable;
    public bool ChangePalette => FChangePalette;
    public IntPtr DC => FDC;
    public IntPtr GdiHandle => FHandle;
    public IntPtr PaletteHandle => FPalette;
    public IntPtr BitsPtr => FPBits;
    public IntPtr TopBitsPtr => FTopPBits;
    public uint ImgBiSizeImage => FBitmapInfo == IntPtr.Zero ? 0u : BiSizeImage;
    public uint ImgBiCompression => FBitmapInfo == IntPtr.Zero ? 0u : BiCompression;
    public ushort ImgBiBitCount => FBitmapInfo == IntPtr.Zero ? (ushort)0 : BiBitCount;
    public int ImgBiWidth => FBitmapInfo == IntPtr.Zero ? 0 : BiWidth;
    public int ImgBiHeight => FBitmapInfo == IntPtr.Zero ? 0 : BiHeight;
    public ushort ImgBiPlanes => FBitmapInfo == IntPtr.Zero ? (ushort)0 : BiPlanes;
    public uint ImgBiClrUsed => FBitmapInfo == IntPtr.Zero ? 0u : BiClrUsed;

    /// <summary>FPBits 的托管快照（前 count 字节）。</summary>
    public byte[] SnapshotBits(int count)
    {
        if (FPBits == IntPtr.Zero || count <= 0) return Array.Empty<byte>();
        var b = new byte[count];
        Marshal.Copy(FPBits, b, 0, count);
        return b;
    }

    /// <summary>把托管字节写回 FPBits（测试构造合成图像用）。</summary>
    public void LoadBits(byte[] data)
    {
        if (FPBits == IntPtr.Zero || data == null) return;
        Marshal.Copy(data, 0, FPBits, Math.Min(data.Length, FBitsCapacity));
    }
}

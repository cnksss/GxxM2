using System;
using System.IO;
using System.Runtime.InteropServices;
using GXX.Core.Rtl;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas 1:1 移植 —— 第 3 片：TDIB 核心（声明 110-349 + 实现 1557-2540、2041-2313）。
//
// 覆盖行号：
//   1557-1568  Create / Destroy
//   1570-1651  Assign（AssignBitmap 1572-1618 / AssignGraphic 1620-1630）
//   1653-1688  Draw（GDI → 接缝）
//   1690-1715  Clear / CanvasChanging / Changing
//   1717-1783  AllocHandle / Compress / Decompress / FreeHandle
//   1785-1873  HasAlphaChannel / AssignAlphaChannel / RetAlphaChannel
//   1875-1975  GetBitmapInfo[Size] / GetCanvas / GetEmpty / GetHandle / GetHeight /
//              GetPalette / GetPaletteCount / GetPBits[ReadOnly] / GetScanLine[ReadOnly] /
//              GetTopPBits[ReadOnly] / GetWidth
//   1982-2039  Mask1/Mask1n/Mask4/Mask4n/Shift1/Shift4 + GetPixel / SetPixel
//   2041-2069  DefineProperties / TGlobalMemoryStream
//   2071-2171  LoadFromClipboardFormat / LoadFromStream / ReadData / SaveToClipboardFormat /
//              SaveToStream / WriteData
//   2173-2313  SetBitCount / SetHeight / SetWidth / SetImage / SetNowPixelFormat / SetPalette /
//              SetSize / UpdatePalette
//   2315-2524  ConvertBitCount（CreateHalftonePalette / PaletteToPalette_Inc /
//              PaletteToRGB_or_RGBtoRGB）
//   2528-2567  StartProgress / EndProgress / UpdateProgress
//
// 原文笔误（照抄 + 注释）：
//   * SetPixel 4bpp 分支：字节下标用 `X shr 3`，但掩码/位移用 `X and 1` —— 原文如此（DIB.pas:2025-2026）。
//   * ConvertBitCount.PaletteToPalette_Inc 1bpp 目标分支：`I shl Shift1[X shr 3]` 而非 `Shift1[X and 7]`
//     —— 原文如此（DIB.pas:2364）。
//   * ConvertBitCount.PaletteToPalette_Inc 4bpp 源分支：`PArrayByte(SrcP)[X and 1]` 而非 `[X shr 1]`
//     —— 原文如此（DIB.pas:2353）。
// =============================================================================================

/// <summary>VCL TProgressStage。</summary>
public enum TProgressStage { psStarting, psRunning, psEnding }

/// <summary>TGraphic.OnProgress 的托管等价。</summary>
public delegate void TDibProgressEvent(TDIB Sender, TProgressStage Stage, int Percent, bool Redraw, TDxRect Rect, string Name);

/// <summary>
/// DIB.pas 2048-2069 —— TGlobalMemoryStream（把全局内存句柄包装成 TMemoryStream）。
/// 依赖 GlobalLock/GlobalSize（见 DibSeams.IDibGlobalMemorySeam）。
/// </summary>
public sealed class TGlobalMemoryStream : MemoryStream
{
    private IntPtr FHandle;

    /// <summary>DIB.pas 2057-2062：SetPointer(GlobalLock(AHandle), GlobalSize(AHandle))。</summary>
    public TGlobalMemoryStream(IntPtr AHandle)
    {
        FHandle = AHandle;
        if (DibSeams.GlobalMemory != null && AHandle != IntPtr.Zero)
        {
            IntPtr p = DibSeams.GlobalMemory.GlobalLock(AHandle);
            int size = DibSeams.GlobalMemory.GlobalSize(AHandle);
            if (p != IntPtr.Zero && size > 0)
            {
                var buf = new byte[size];
                Marshal.Copy(p, buf, 0, size);
                Write(buf, 0, size);
                Position = 0;
            }
        }
    }

    /// <summary>DIB.pas 2064-2069。</summary>
    public void Destroy()
    {
        if (FHandle != IntPtr.Zero) DibSeams.GlobalMemory?.GlobalUnlock(FHandle);
        FHandle = IntPtr.Zero;
    }
}

/// <summary>VCL TFiler 的最小面（DIB.pas 2041-2046 只用 DefineBinaryProperty）。</summary>
public interface IDibFiler
{
    void DefineBinaryProperty(string name, Action<Stream> read, Action<Stream> write, bool hasWrite);
}

/// <summary>
/// VCL TPersistent/TGraphic/TBitmap 的托管接缝基类（DIB.pas 1572-1630 的 AssignBitmap / AssignGraphic）。
/// 具体图形对象由客户端装载层提供（§2.3：VCL 图形对象不翻译，改为接口面）。
/// </summary>
public abstract class TDibGraphicSource
{
    public abstract int Width { get; }
    public abstract int Height { get; }
}

/// <summary>VCL TBitmap 的接缝面（AssignBitmap 需要 Handle + Palette）。</summary>
public abstract class TDibBitmapSource : TDibGraphicSource
{
    public abstract IntPtr Handle { get; }
    public abstract IntPtr Palette { get; }

    /// <summary>GetObject(Handle, SizeOf(TBitmap)/SizeOf(TDIBSection), @Data) 的托管等价：
    /// 返回 (bitsPixel, width, height, 三个位域掩码)。不是 DIB Section 时掩码为 0。</summary>
    public abstract void GetBitmapInfo(out int bitsPixel, out int width, out int height,
        out int field0, out int field1, out int field2, out bool isDibSection);
}

/// <summary>
/// DIB.pas 110-349 —— TDIB。
/// 本文件只含"核心"；特效见 DIB.Effects.cs，绘制/DXFusion 见 DIB.Fusion.cs，尾部见 DIB.Tail.cs。
/// </summary>
public partial class TDIB
{
    // ---- DIB.pas 112-132 私有字段 ----
    private TDibCanvas FCanvas;
    internal TDIBSharedImage FImage;

    private string FProgressName;
    private uint FProgressOldY;
    private uint FProgressOldTime;
    private uint FProgressOld;
    private uint FProgressY;

    // For speed-up
    internal int FBitCount;
    internal int FHeight;
    internal int FNextLine;
    internal TDIBPixelFormat FNowPixelFormat;
    internal IntPtr FPBits;
    internal int FSize;
    internal IntPtr FTopPBits;
    internal int FWidth;
    internal int FWidthBytes;
    internal int[,] FLUTDist = new int[256, 256];
    internal int LG_COUNT;
    internal int LG_DETAIL;

    // ---- DIB.pas 179-180 公开字段 ----
    /// <summary>DIB.pas 179 —— ColorTable: TRGBQuads。</summary>
    public TRGBQuad[] ColorTable = new TRGBQuad[256];

    /// <summary>DIB.pas 180 —— PixelFormat: TDIBPixelFormat。</summary>
    public TDIBPixelFormat PixelFormat;

    /// <summary>TGraphic.PaletteModified（DIB.pas 2298/2312 设置）。</summary>
    public bool PaletteModified;

    /// <summary>TGraphic.OnProgress 的托管等价。</summary>
    public event TDibProgressEvent OnProgress;

    // =========================================================================================
    // 指针助手（与原文 Integer(FPBits)/FTopPBits 的指针算术一一对应）
    // =========================================================================================

    internal unsafe byte* Bits => (byte*)FPBits;
    internal unsafe byte* TopBits => (byte*)FTopPBits;
    /// <summary>原文 `Integer(FTopPBits) + Y * FNextLine`。</summary>
    internal unsafe byte* Line(int Y) => (byte*)FTopPBits + (long)Y * FNextLine;
    internal static unsafe int Addr(IntPtr p) => unchecked((int)(long)p);

    // =========================================================================================
    // DIB.pas 1557-1568 —— Create / Destroy
    // =========================================================================================

    /// <summary>DIB.pas 1557-1561 1:1：SetImage(EmptyDIBImage)。</summary>
    public TDIB()
    {
        SetImage(DIB.EmptyDIBImage());
    }

    /// <summary>DIB.pas 1563-1568 1:1：SetImage(EmptyDIBImage) → FCanvas.Free。</summary>
    public void Destroy()
    {
        SetImage(DIB.EmptyDIBImage());
        FCanvas = null;
    }

    // =========================================================================================
    // DIB.pas 1570-1651 —— Assign
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1570-1651 1:1（VCL 类型判定改由托管接缝类型判定）。
    /// nil → Clear；TDIB → SetImage(Source.FImage)（自身则不动）；TDibGraphicSource → AssignGraphic；
    /// 其它 → inherited Assign（托管侧无父类语义，登记为无操作）。
    /// </summary>
    public void Assign(object Source)
    {
        if (Source == null)
        {
            Clear();
        }
        else if (Source is TDIB)
        {
            if (!ReferenceEquals(Source, this))
                SetImage(((TDIB)Source).FImage);
        }
        else if (Source is TDibGraphicSource)
        {
            AssignGraphic((TDibGraphicSource)Source);
        }
        else
        {
            // inherited Assign(Source)
        }
    }

    /// <summary>DIB.pas 1572-1618 —— AssignBitmap（GetObject 结果分派 + Canvas.Draw）。</summary>
    private void AssignBitmap(TDibBitmapSource Source)
    {
        var EntryBytes = new byte[256 * 4];
        if (Source.Palette != IntPtr.Zero && DibSeams.Gdi != null)
            DibSeams.Gdi.GetPaletteEntries(Source.Palette, 0, 256, EntryBytes);

        var entries = new TPaletteEntry[256];
        for (int e = 0; e < 256; e++)
        {
            entries[e].peRed = EntryBytes[e * 4 + 0];
            entries[e].peGreen = EntryBytes[e * 4 + 1];
            entries[e].peBlue = EntryBytes[e * 4 + 2];
            entries[e].peFlags = EntryBytes[e * 4 + 3];
        }
        ColorTable = DIB.PaletteEntriesToRGBQuads(entries);
        UpdatePalette();

        Source.GetBitmapInfo(out int bitsPixel, out int bmWidth, out int bmHeight,
            out int f0, out int f1, out int f2, out bool isDibSection);

        if (!isDibSection)
        {
            // SizeOf(Windows.TBitmap) 分支
            if (bitsPixel == 16)
                PixelFormat = DIB.MakeDIBPixelFormat(5, 5, 5);
            else
                PixelFormat = DIB.MakeDIBPixelFormat(8, 8, 8);
            SetSize(bmWidth, bmHeight, bitsPixel);
        }
        else
        {
            // SizeOf(TDIBSection) 分支
            if (bitsPixel >= 24)
            {
                PixelFormat = DIB.MakeDIBPixelFormat(8, 8, 8);
            }
            else if (bitsPixel > 8)
            {
                PixelFormat = DIB.MakeDIBPixelFormatMask(f0, f1, f2); // correct I.Ceneff, thanks
            }
            else
            {
                PixelFormat = DIB.MakeDIBPixelFormat(8, 8, 8);
            }
            SetSize(bmWidth, bmHeight, bitsPixel);
        }

        FillCharBits(0, FSize);
        Canvas.Draw(0, 0, Source);
    }

    /// <summary>DIB.pas 1620-1630 —— AssignGraphic。</summary>
    private void AssignGraphic(TDibGraphicSource Source)
    {
        if (Source is TDibBitmapSource bmp)
        {
            AssignBitmap(bmp);
        }
        else
        {
            SetSize(Source.Width, Source.Height, 24);
            FillCharBits(0, FSize);
            Canvas.Draw(0, 0, Source);
        }
    }

    /// <summary>FillChar(PBits^, Size, 0)。</summary>
    private void FillCharBits(int offset, int count)
    {
        if (FPBits == IntPtr.Zero || count <= 0) return;
        for (int i = 0; i < count; i++) Marshal.WriteByte(FPBits, offset + i, 0);
    }

    // =========================================================================================
    // DIB.pas 1653-1688 —— Draw（GDI → 接缝）
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1653-1688 1:1（GDI StretchDIBits / StretchBlt 收敛到 DibSeams.Canvas）。
    /// 原文：Size &gt; 0 才画；PaletteCount &gt; 0 时 SelectPalette + RealizePalette；
    /// SetStretchBltMode(COLORONCOLOR)；FImage.FMemoryImage → StretchDIBits(FPBits)，否则 StretchBlt(FDC)。
    /// </summary>
    public void Draw(TDibCanvas ACanvas, TDxRect Rect)
    {
        if (Size > 0)
        {
            if (DibSeams.Canvas == null) return;
            DibSeams.Canvas.Draw(ACanvas.Handle, Rect.Left, Rect.Top, this);
        }
    }

    // =========================================================================================
    // DIB.pas 1690-1715 —— Clear / CanvasChanging / Changing
    // =========================================================================================

    /// <summary>DIB.pas 1690-1693。</summary>
    public void Clear() => SetImage(DIB.EmptyDIBImage());

    /// <summary>DIB.pas 1695-1698。</summary>
    private void CanvasChanging(object Sender) => Changing(false);

    /// <summary>
    /// DIB.pas 1700-1715 1:1。
    /// 触发条件：FImage.RefCount &gt; 1 或 FImage.FCompressed 或 (not MemoryImage 且 FImage.FMemoryImage)；
    /// 命中则新建 TempImage 并 Decompress(FImage, FImage.FMemoryImage and MemoryImage) 后 SetImage。
    /// </summary>
    public void Changing(bool MemoryImage)
    {
        if (FImage.RefCount > 1 || FImage.FCompressed || (!MemoryImage && FImage.FMemoryImage))
        {
            var TempImage = new TDIBSharedImage();
            try
            {
                TempImage.Decompress(FImage, FImage.FMemoryImage && MemoryImage);
            }
            catch
            {
                TempImage.Destroy();
                throw;
            }
            SetImage(TempImage);
        }
    }

    // =========================================================================================
    // DIB.pas 1717-1783 —— AllocHandle / Compress / Decompress / FreeHandle
    // =========================================================================================

    /// <summary>DIB.pas 1717-1732：FImage.FMemoryImage → Decompress(FImage, False)（要 GDI 句柄）。</summary>
    public void AllocHandle()
    {
        if (FImage.FMemoryImage)
        {
            var TempImage = new TDIBSharedImage();
            try
            {
                TempImage.Decompress(FImage, false);
            }
            catch
            {
                TempImage.Destroy();
                throw;
            }
            SetImage(TempImage);
        }
    }

    /// <summary>DIB.pas 1734-1749：仅当未压缩且位深 in [4,8] 时压缩。</summary>
    public void Compress()
    {
        if (!FImage.FCompressed && (BitCount == 4 || BitCount == 8))
        {
            var TempImage = new TDIBSharedImage();
            try
            {
                TempImage.Compress(FImage);
            }
            catch
            {
                TempImage.Destroy();
                throw;
            }
            SetImage(TempImage);
        }
    }

    /// <summary>DIB.pas 1751-1766：FImage.FCompressed 时解压。</summary>
    public void Decompress()
    {
        if (FImage.FCompressed)
        {
            var TempImage = new TDIBSharedImage();
            try
            {
                TempImage.Decompress(FImage, FImage.FMemoryImage);
            }
            catch
            {
                TempImage.Destroy();
                throw;
            }
            SetImage(TempImage);
        }
    }

    /// <summary>DIB.pas 1768-1783：非内存图时 Duplicate(FImage, True) 去掉 GDI 句柄。</summary>
    public void FreeHandle()
    {
        if (!FImage.FMemoryImage)
        {
            var TempImage = new TDIBSharedImage();
            try
            {
                TempImage.Duplicate(FImage, true);
            }
            catch
            {
                TempImage.Destroy();
                throw;
            }
            SetImage(TempImage);
        }
    }

    // =========================================================================================
    // DIB.pas 1785-1873 —— Alpha 通道
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1789-1804 1:1。
    /// 原文逻辑：Result := True；**仅当 BitCount = 32 时**才扫描；扫描中一旦遇到
    /// rgbReserved &lt;&gt; 0 就 **Exit（返回 True）**；扫完（或无扫描）后 Result := False。
    /// 即：32bpp 且任一位为 0 → 整图判为无 alpha；非 32bpp 恒为 False。
    /// </summary>
    public bool HasAlphaChannel()
    {
        bool Result = true;
        if (BitCount == 32)
        {
            for (int Y = 0; Y <= Height - 1; Y++)
            {
                IntPtr P = ScanLine(Y);
                for (int X = 0; X <= Width - 1; X++)
                {
                    byte reserved = Marshal.ReadByte(P, X * 4 + 3);
                    if (reserved != 0) return true;
                }
            }
        }
        Result = false;
        return Result;
    }

    /// <summary>
    /// DIB.pas 1806-1853 1:1。
    /// 空图直接 False；位深非 32 时先通过临时 DIB + Canvas.Draw 转成 32bpp；
    /// 尺寸不同则 False；源位深 32 → 逐像素拷 rgbReserved；源位深 8 → 逐像素拷字节；其余 False。
    /// </summary>
    public bool AssignAlphaChannel(TDIB DIB)
    {
        bool Result = false;
        if (Empty) return false;

        // Alphachannel can be copy into 32bit DIB only!
        if (BitCount != 32)
        {
            var tmpDIB = new TDIB();
            try
            {
                tmpDIB.Assign(this);
                Clear();
                SetSize(tmpDIB.Width, tmpDIB.Height, 32); // 1.07f
                Canvas.Draw(0, 0, tmpDIB);
            }
            finally
            {
                tmpDIB.Destroy();
            }
        }

        // Must be the same size!
        if (!(Width == DIB.Width && Height == DIB.Height)) return false;

        switch (DIB.BitCount)
        {
            case 32:
                for (int Y = 0; Y <= Height - 1; Y++)
                {
                    IntPtr p0 = ScanLine(Y);
                    IntPtr p1 = DIB.ScanLine(Y);
                    for (int X = 0; X <= Width - 1; X++)
                    {
                        byte src = Marshal.ReadByte(p1, X * 4 + 3);
                        Marshal.WriteByte(p0, X * 4 + 3, src);
                    }
                }
                break;
            case 8:
                for (int Y = 0; Y <= Height - 1; Y++)
                {
                    IntPtr p0 = ScanLine(Y);
                    IntPtr pB = DIB.ScanLine(Y);
                    for (int X = 0; X <= Width - 1; X++)
                    {
                        byte src = Marshal.ReadByte(pB, X);
                        Marshal.WriteByte(p0, X * 4 + 3, src);
                    }
                }
                break;
            default:
                return false;
        }
        Result = true;
        return Result;
    }

    /// <summary>DIB.pas 1855-1873 1:1：无 alpha 通道时 DIB := nil，否则导出 8bpp 灰度 alpha 图。</summary>
    public void RetAlphaChannel(out TDIB DIB)
    {
        DIB = null;
        if (!HasAlphaChannel()) return;
        DIB = new TDIB();
        DIB.SetSize(Width, Height, 8);
        for (int Y = 0; Y <= Height - 1; Y++)
        {
            IntPtr p0 = ScanLine(Y);
            IntPtr pB = DIB.ScanLine(Y);
            for (int X = 0; X <= Width - 1; X++)
            {
                byte src = Marshal.ReadByte(p0, X * 4 + 3);
                Marshal.WriteByte(pB, X, src);
            }
        }
    }

    // =========================================================================================
    // DIB.pas 1875-1980 —— 属性读取
    // =========================================================================================

    /// <summary>DIB.pas 1875-1878。</summary>
    public IntPtr GetBitmapInfo() => FImage.FBitmapInfo;

    /// <summary>DIB.pas 1880-1883。</summary>
    public int GetBitmapInfoSize() => FImage.FBitmapInfoSize;

    /// <summary>DIB.pas 1885-1896：Canvas 为空或 Handle=0 时 AllocHandle 并新建。</summary>
    public TDibCanvas GetCanvas()
    {
        if (FCanvas == null || FCanvas.Handle == IntPtr.Zero)
        {
            AllocHandle();
            FCanvas = new TDibCanvas(FImage.FDC, CanvasChanging);
        }
        return FCanvas;
    }

    /// <summary>DIB.pas 1898-1901。</summary>
    public bool GetEmpty() => Size == 0;

    /// <summary>DIB.pas 1903-1907：Changing(True) 后返回 FImage.FHandle。</summary>
    public IntPtr GetHandle()
    {
        Changing(true);
        return FImage.FHandle;
    }

    /// <summary>DIB.pas 1909-1912。</summary>
    public int GetHeight() => FHeight;

    /// <summary>DIB.pas 1914-1917。</summary>
    public IntPtr GetPalette() => FImage.GetPalette();

    /// <summary>DIB.pas 1919-1922。</summary>
    public int GetPaletteCount() => FImage.FPaletteCount;

    /// <summary>DIB.pas 1924-1931：Changing(True)；非内存图先 GdiFlush。</summary>
    public IntPtr GetPBits()
    {
        Changing(true);
        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();
        return FPBits;
    }

    /// <summary>DIB.pas 1933-1938。</summary>
    public IntPtr GetPBitsReadOnly()
    {
        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();
        return FPBits;
    }

    /// <summary>
    /// DIB.pas 1940-1949 1:1：Changing(True)；Y 越界抛 EInvalidGraphicOperation(SScanline)；
    /// 返回 FTopPBits + Y*FNextLine。
    /// </summary>
    public IntPtr GetScanLine(int Y)
    {
        Changing(true);
        if (Y < 0 || Y >= FHeight)
            // 原文（DIB.pas:1943-1944）`CreateFmt(SScanline, [Y])`；SScanline 是 **Delphi** 格式串
            // 'Index of the scanning line exceeded the range. (%d)'，必须走 DelphiFormat.Format。
            // 上一轮误用 `string.Format`（`%d` 无法替换），属实现缺陷，已修正。
            throw new EInvalidGraphicOperation(DelphiFormat.Format(DXConsts.SScanline, Y));

        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();
        return IntPtr.Add(FTopPBits, Y * FNextLine);
    }

    /// <summary>DIB.pas 1951-1959（不触发 Changing）。</summary>
    public IntPtr GetScanLineReadOnly(int Y)
    {
        if (Y < 0 || Y >= FHeight)
            // 同上（DIB.pas:1953-1954）：SScanline 走 DelphiFormat.Format。
            throw new EInvalidGraphicOperation(DelphiFormat.Format(DXConsts.SScanline, Y));

        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();
        return IntPtr.Add(FTopPBits, Y * FNextLine);
    }

    /// <summary>DIB.pas 1961-1968。</summary>
    public IntPtr GetTopPBits()
    {
        Changing(true);
        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();
        return FTopPBits;
    }

    /// <summary>DIB.pas 1970-1975。</summary>
    public IntPtr GetTopPBitsReadOnly()
    {
        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();
        return FTopPBits;
    }

    /// <summary>DIB.pas 1977-1980。</summary>
    public int GetWidth() => FWidth;

    // ---- C# 便利访问器（对应原文的 property；名字不同以避免 C# 索引器签名冲突） ----

    public int BitCount => FBitCount;
    public int Width => FWidth;
    public int Height => FHeight;
    public int NextLine => FNextLine;
    public int Size => FSize;
    public int WidthBytes => FWidthBytes;
    public int PaletteCount => GetPaletteCount();
    public bool Empty => GetEmpty();
    public TDIBPixelFormat NowPixelFormat => FNowPixelFormat;
    public IntPtr PBits => GetPBits();
    public IntPtr PBitsReadOnly => GetPBitsReadOnly();
    public IntPtr TopPBits => GetTopPBits();
    public IntPtr TopPBitsReadOnly => GetTopPBitsReadOnly();
    /// <summary>原文 property ScanLine[Y]: Pointer（C# 不能用两个同名索引器，改为方法）。</summary>
    public IntPtr ScanLine(int Y) => GetScanLine(Y);
    /// <summary>原文 property ScanLineReadOnly[Y]: Pointer。</summary>
    public IntPtr ScanLineReadOnly(int Y) => GetScanLineReadOnly(Y);
    /// <summary>原文 property Canvas: TCanvas。</summary>
    public TDibCanvas Canvas => GetCanvas();
    /// <summary>原文 property Handle: THandle。</summary>
    public IntPtr Handle => GetHandle();
    /// <summary>原文 property Palette: HPalette。</summary>
    public IntPtr Palette => GetPalette();
    /// <summary>原文 property BitmapInfo: PBitmapInfo。</summary>
    public IntPtr BitmapInfo => GetBitmapInfo();
    /// <summary>原文 property BitmapInfoSize: Integer。</summary>
    public int BitmapInfoSize => GetBitmapInfoSize();
    /// <summary>原文 property Pixels[X, Y]: DWord。</summary>
    public uint this[int X, int Y]
    {
        get => GetPixel(X, Y);
        set => SetPixel(X, Y, value);
    }

    // =========================================================================================
    // DIB.pas 1992-2039 —— GetPixel / SetPixel
    // =========================================================================================

    /// <summary>
    /// DIB.pas 1992-2009 1:1。
    /// 先 Decompress；越界返回 0（不抛）。1bpp 用 Mask1/Shift1[X and 7]；4bpp 用 Mask4/Shift4[X and 1]；
    /// 8bpp 直读；16bpp 读 Word；24bpp 读 BGR 并合成 R|G&lt;&lt;8|B&lt;&lt;16；32bpp 读 DWord。
    /// </summary>
    public uint GetPixel(int X, int Y)
    {
        Decompress();

        uint Result = 0;
        if (X >= 0 && X < FWidth && Y >= 0 && Y < FHeight)
        {
            unsafe
            {
                byte* row = Line(Y);
                switch (FBitCount)
                {
                    case 1:
                        Result = ((uint)row[X >> 3] & DIBConstants.Mask1[X & 7]) >> (int)DIBConstants.Shift1[X & 7];
                        break;
                    case 4:
                        Result = ((uint)row[X >> 1] & DIBConstants.Mask4[X & 1]) >> (int)DIBConstants.Shift4[X & 1];
                        break;
                    case 8:
                        Result = row[X];
                        break;
                    case 16:
                        Result = *(ushort*)(row + X * 2);
                        break;
                    case 24:
                        {
                            byte* p = row + X * 3;
                            Result = (uint)(p[2] | (p[1] << 8) | (p[0] << 16));
                        }
                        break;
                    case 32:
                        Result = *(uint*)(row + X * 4);
                        break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// DIB.pas 2011-2039 1:1。
    /// 原文 4bpp 分支的字节下标是 `X shr 3`（而掩码/位移用 `X and 1`）—— 原文笔误，照抄（DIB.pas:2025-2026）。
    /// </summary>
    public void SetPixel(int X, int Y, uint Value)
    {
        Changing(true);

        if (X >= 0 && X < FWidth && Y >= 0 && Y < FHeight)
        {
            unsafe
            {
                byte* row = Line(Y);
                switch (FBitCount)
                {
                    case 1:
                        {
                            byte* P = row + (X >> 3);
                            P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) | ((Value & 1) << (int)DIBConstants.Shift1[X & 7]));
                        }
                        break;
                    case 4:
                        {
                            // 原文如此（DIB.pas:2025）：字节下标用 X shr 3
                            byte* P = row + (X >> 3);
                            P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | ((Value & 15) << (int)DIBConstants.Shift4[X & 1]));
                        }
                        break;
                    case 8:
                        row[X] = (byte)Value;
                        break;
                    case 16:
                        *(ushort*)(row + X * 2) = (ushort)Value;
                        break;
                    case 24:
                        {
                            byte* p = row + X * 3;
                            p[0] = (byte)(Value >> 16); // B
                            p[1] = (byte)(Value >> 8);  // G
                            p[2] = (byte)Value;         // R
                        }
                        break;
                    case 32:
                        *(uint*)(row + X * 4) = Value;
                        break;
                }
            }
        }
    }

    // =========================================================================================
    // DIB.pas 2041-2046 —— DefineProperties
    // =========================================================================================

    /// <summary>DIB.pas 2041-2046 1:1（VCL TFiler → IDibFiler 接缝）。</summary>
    public void DefineProperties(IDibFiler Filer)
    {
        // inherited DefineProperties(Filer);
        // For interchangeability with an old version.
        Filer?.DefineBinaryProperty("DIB", LoadFromStream, null, false);
    }

    // =========================================================================================
    // DIB.pas 2071-2171 —— 剪贴板 / 流读写
    // =========================================================================================

    /// <summary>DIB.pas 2071-2082 1:1（GlobalLock 走接缝）。</summary>
    public void LoadFromClipboardFormat(ushort AFormat, IntPtr AData, IntPtr APalette)
    {
        var Stream = new TGlobalMemoryStream(AData);
        try
        {
            ReadData(Stream);
        }
        finally
        {
            Stream.Destroy();
        }
    }

    /// <summary>
    /// DIB.pas 2087-2103 1:1。
    /// 读 14 字节 TBitmapFileHeader：I=0 → 直接 Exit（**保留原图**，不清空）；I&lt;&gt;14 → EInvalidGraphic；
    /// bfType 必须 = 'BM'（$4D42）否则 EInvalidGraphic；随后 ReadData。
    /// </summary>
    public void LoadFromStream(Stream Stream)
    {
        var BF = new byte[DIB.SizeOfBitmapFileHeader];
        int I = Stream.Read(BF, 0, DIB.SizeOfBitmapFileHeader);
        if (I == 0) return;
        if (I != DIB.SizeOfBitmapFileHeader)
            throw new EInvalidGraphic(DXConsts.SInvalidDIB);

        ushort bfType = BitConverter.ToUInt16(BF, 0);
        if (bfType != DIB.BitmapFileType)
            throw new EInvalidGraphic(DXConsts.SInvalidDIB);

        ReadData(Stream);
    }

    /// <summary>DIB.pas 2105-2117 1:1：新建 TempImage → TempImage.ReadData(Stream, FImage.FMemoryImage) → SetImage。</summary>
    public void ReadData(Stream Stream)
    {
        var TempImage = new TDIBSharedImage();
        try
        {
            TempImage.ReadData(Stream, FImage.FMemoryImage);
        }
        catch
        {
            TempImage.Destroy();
            throw;
        }
        SetImage(TempImage);
    }

    /// <summary>DIB.pas 2119-2141 1:1（GlobalAlloc/GHND 走接缝）。</summary>
    public void SaveToClipboardFormat(out ushort AFormat, out IntPtr AData, out IntPtr APalette)
    {
        AFormat = DIB.CF_DIB;
        APalette = IntPtr.Zero;

        using var Stream = new MemoryStream();
        WriteData(Stream);

        AData = DibSeams.GlobalMemory != null
            ? DibSeams.GlobalMemory.GlobalAlloc(DibSeams.GHND, (int)Stream.Length)
            : IntPtr.Zero;
        if (AData == IntPtr.Zero) throw new EOutOfMemory("Out of memory");

        IntPtr P = DibSeams.GlobalMemory.GlobalLock(AData);
        if (P != IntPtr.Zero)
        {
            var buf = Stream.ToArray();
            Marshal.Copy(buf, 0, P, buf.Length);
        }
        DibSeams.GlobalMemory.GlobalUnlock(AData);
    }

    /// <summary>
    /// DIB.pas 2143-2160 1:1。
    /// bfType=$4D42；bfOffBits = 14 + BitmapInfoSize；bfSize = bfOffBits + biSizeImage；
    /// bfReserved1/2 = 0；写 14 字节头后 WriteData。
    /// </summary>
    public void SaveToStream(Stream Stream)
    {
        if (Empty) return;

        var BF = new byte[DIB.SizeOfBitmapFileHeader];
        BitConverter.GetBytes((ushort)DIB.BitmapFileType).CopyTo(BF, 0);
        uint bfOffBits = (uint)(DIB.SizeOfBitmapFileHeader + BitmapInfoSize);
        BitConverter.GetBytes(bfOffBits).CopyTo(BF, 10);
        BitConverter.GetBytes(bfOffBits + FImage.BiSizeImage).CopyTo(BF, 2);
        BitConverter.GetBytes((ushort)0).CopyTo(BF, 6);
        BitConverter.GetBytes((ushort)0).CopyTo(BF, 8);
        Stream.Write(BF, 0, DIB.SizeOfBitmapFileHeader);

        WriteData(Stream);
    }

    /// <summary>
    /// DIB.pas 2162-2171 1:1。
    /// 空图直接 Exit；非内存图先 GdiFlush；写 BitmapInfoSize 字节的 BITMAPINFO，再写 biSizeImage 字节像素。
    /// </summary>
    public void WriteData(Stream Stream)
    {
        if (Empty) return;

        if (!FImage.FMemoryImage) DibSeams.Gdi?.GdiFlush();

        var info = FImage.BitmapInfoBytes();
        Stream.Write(info, 0, info.Length);

        int pixelBytes = (int)FImage.BiSizeImage;
        if (pixelBytes > 0 && FImage.FPBits != IntPtr.Zero)
        {
            var px = new byte[pixelBytes];
            Marshal.Copy(FImage.FPBits, px, 0, pixelBytes);
            Stream.Write(px, 0, px.Length);
        }
    }

    // =========================================================================================
    // DIB.pas 2173-2313 —— Set* / UpdatePalette
    // =========================================================================================

    /// <summary>DIB.pas 2173-2187 1:1：Value &lt;= 0 → Clear；空图 → SetSize(Max(Width,1), Max(Height,1), Value)；否则 ConvertBitCount。</summary>
    public void SetBitCount(int Value)
    {
        if (Value <= 0)
            Clear();
        else
        {
            if (Empty)
                SetSize(Math.Max(Width, 1), Math.Max(Height, 1), Value);
            else
                ConvertBitCount(Value);
        }
    }

    /// <summary>DIB.pas 2189-2200 1:1。</summary>
    public void SetHeight(int Value)
    {
        if (Value <= 0)
            Clear();
        else
        {
            if (Empty)
                SetSize(Math.Max(Width, 1), Value, 8);
            else
                SetSize(Width, Value, BitCount);
        }
    }

    /// <summary>DIB.pas 2202-2213 1:1。</summary>
    public void SetWidth(int Value)
    {
        if (Value <= 0)
            Clear();
        else
        {
            if (Empty)
                SetSize(Value, Math.Max(Height, 1), 8);
            else
                SetSize(Value, Height, BitCount);
        }
    }

    /// <summary>
    /// DIB.pas 2215-2242 1:1。
    /// FImage 不同才动作：清 FCanvas.Handle → 旧图 Release、新图 Reference →
    /// 把 FImage 的 FBitCount/FHeight/FNextLine/FPixelFormat/FPBits/FSize/FTopPBits/FWidth/FWidthBytes
    /// 逐一镜像到 TDIB 自己的同名字段（"For speed-up"），并同步 ColorTable / PixelFormat。
    /// </summary>
    public void SetImage(TDIBSharedImage Value)
    {
        if (!ReferenceEquals(FImage, Value))
        {
            if (FCanvas != null)
                FCanvas.Handle = IntPtr.Zero;

            FImage?.Release();
            FImage = Value;
            FImage.Reference();

            if (FCanvas != null)
                FCanvas.Handle = FImage.FDC;

            ColorTable = FImage.FColorTable;
            PixelFormat = FImage.FPixelFormat;

            FBitCount = FImage.FBitCount;
            FHeight = FImage.FHeight;
            FNextLine = FImage.FNextLine;
            FNowPixelFormat = FImage.FPixelFormat;
            FPBits = FImage.FPBits;
            FSize = FImage.FSize;
            FTopPBits = FImage.FTopPBits;
            FWidth = FImage.FWidth;
            FWidthBytes = FImage.FWidthBytes;
        }
    }

    /// <summary>
    /// DIB.pas 2244-2260 1:1。
    /// CompareMem(Value, FImage.FPixelFormat) 相同则 Exit；否则 PixelFormat := Value；
    /// 临时 DIB Assign(Self) → SetSize(Width, Height, BitCount) → Canvas.Draw（重新解释像素）。
    /// </summary>
    public void SetNowPixelFormat(in TDIBPixelFormat Value)
    {
        if (TDIBPixelFormat.MemEquals(Value, FImage.FPixelFormat)) return;

        PixelFormat = Value;

        var Temp = new TDIB();
        try
        {
            Temp.Assign(this);
            SetSize(Width, Height, BitCount);
            Canvas.Draw(0, 0, Temp);
        }
        finally
        {
            Temp.Destroy();
        }
    }

    /// <summary>DIB.pas 2262-2271：GetPaletteEntries(Value,0,256) → DeleteObject(Value) → ColorTable → UpdatePalette。</summary>
    public void SetPalette(IntPtr Value)
    {
        var EntryBytes = new byte[256 * 4];
        if (Value != IntPtr.Zero && DibSeams.Gdi != null)
            DibSeams.Gdi.GetPaletteEntries(Value, 0, 256, EntryBytes);
        if (Value != IntPtr.Zero) DibSeams.Gdi?.DeleteObject(Value);

        var entries = new TPaletteEntry[256];
        for (int e = 0; e < 256; e++)
        {
            entries[e].peRed = EntryBytes[e * 4 + 0];
            entries[e].peGreen = EntryBytes[e * 4 + 1];
            entries[e].peBlue = EntryBytes[e * 4 + 2];
            entries[e].peFlags = EntryBytes[e * 4 + 3];
        }
        ColorTable = DIB.PaletteEntriesToRGBQuads(entries);
        UpdatePalette();
    }

    /// <summary>
    /// DIB.pas 2273-2299 1:1。
    /// 尺寸+位深+三个掩码全同 → Exit；AWidth/AHeight &lt;= 0 → Clear；否则 NewImage(..., FImage.FMemoryImage, False)
    /// 后 SetImage 并置 PaletteModified := True。
    /// </summary>
    public void SetSize(int AWidth, int AHeight, int ABitCount)
    {
        if (AWidth == Width && AHeight == Height && ABitCount == BitCount &&
            NowPixelFormat.RBitMask == PixelFormat.RBitMask &&
            NowPixelFormat.GBitMask == PixelFormat.GBitMask &&
            NowPixelFormat.BBitMask == PixelFormat.BBitMask) return;

        if (AWidth <= 0 || AHeight <= 0)
        {
            Clear();
            return;
        }

        var TempImage = new TDIBSharedImage();
        try
        {
            TempImage.NewImage(AWidth, AHeight, ABitCount,
                PixelFormat, ColorTable, FImage.FMemoryImage, false);
        }
        catch
        {
            TempImage.Destroy();
            throw;
        }
        SetImage(TempImage);

        PaletteModified = true;
    }

    /// <summary>
    /// DIB.pas 2301-2313 1:1。
    /// CompareMem(ColorTable, FImage.FColorTable) 相同 → Exit；否则 Changing(True)、
    /// ColorTable := Col、FImage.SetColorTable(ColorTable)、PaletteModified := True。
    /// </summary>
    public void UpdatePalette()
    {
        if (ColorTableEquals(ColorTable, FImage.FColorTable)) return;

        var Col = ColorTable;
        Changing(true);
        ColorTable = Col;
        FImage.SetColorTable(ColorTable);

        PaletteModified = true;
    }

    private static bool ColorTableEquals(TRGBQuad[] a, TRGBQuad[] b)
    {
        for (int i = 0; i < 256; i++)
        {
            if (a[i].rgbBlue != b[i].rgbBlue || a[i].rgbGreen != b[i].rgbGreen ||
                a[i].rgbRed != b[i].rgbRed || a[i].rgbReserved != b[i].rgbReserved) return false;
        }
        return true;
    }

    // =========================================================================================
    // DIB.pas 2315-2524 —— ConvertBitCount
    // =========================================================================================

    /// <summary>
    /// DIB.pas 2319-2330 —— CreateHalftonePalette（半色调调色板）。
    /// 注意原文表达式 `1 shl R - 1` 在 Delphi 里是 `1 shl (R-1)`（shl 优先级低于 -），
    /// 即 (1&lt;&lt;(R-1))；`(I shr (G+B-1)) and (1 shl R - 1)` 同理。这里照抄该优先级。
    /// </summary>
    private void CreateHalftonePalette(int R, int G, int B)
    {
        for (int I = 0; I <= 255; I++)
        {
            ColorTable[I].rgbRed = (byte)(((I >> (G + B - 1)) & ((1 << R) - 1)) * 255 / ((1 << R) - 1));
            ColorTable[I].rgbGreen = (byte)(((I >> (B - 1)) & ((1 << G) - 1)) * 255 / ((1 << G) - 1));
            ColorTable[I].rgbBlue = (byte)(((I >> 0) & ((1 << B) - 1)) * 255 / ((1 << B) - 1));
        }
    }

    /// <summary>
    /// DIB.pas 2332-2377 —— PaletteToPalette_Inc（低位深 → 更高位深调色板搬运）。
    /// 原文笔误照抄：1bpp 目标分支用 `Shift1[X shr 3]`（应为 X and 7）；4bpp 源分支用 `[X and 1]`（应为 X shr 1）。
    /// 8bpp 源/目标用指针自增（逐像素前进）。
    /// </summary>
    private void PaletteToPalette_Inc(TDIB Temp)
    {
        uint I = 0;

        for (int Y = 0; Y <= Height - 1; Y++)
        {
            IntPtr SrcP = Temp.ScanLine(Y);
            IntPtr DestP = ScanLine(Y);

            unsafe
            {
                byte* src = (byte*)SrcP;
                byte* dst = (byte*)DestP;

                for (int X = 0; X <= Width - 1; X++)
                {
                    switch (Temp.BitCount)
                    {
                        case 1:
                            I = ((uint)src[X >> 3] & DIBConstants.Mask1[X & 7]) >> (int)DIBConstants.Shift1[X & 7];
                            break;
                        case 4:
                            // 原文如此（DIB.pas:2353）：下标用 X and 1
                            I = ((uint)src[X & 1] & DIBConstants.Mask4[X & 1]) >> (int)DIBConstants.Shift4[X & 1];
                            break;
                        case 8:
                            I = src[0];
                            src++;
                            break;
                    }

                    switch (BitCount)
                    {
                        case 1:
                            {
                                // 原文如此（DIB.pas:2364）：Shift1[X shr 3]
                                byte* P = dst + (X >> 3);
                                P[0] = (byte)((P[0] & DIBConstants.Mask1n[X & 7]) | (I << (int)DIBConstants.Shift1[X >> 3]));
                            }
                            break;
                        case 4:
                            {
                                byte* P = dst + (X >> 1);
                                P[0] = (byte)((P[0] & DIBConstants.Mask4n[X & 1]) | (I << (int)DIBConstants.Shift4[X & 1]));
                            }
                            break;
                        case 8:
                            dst[0] = (byte)I;
                            dst++;
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// DIB.pas 2379-2463 —— PaletteToRGB_or_RGBToRGB（任意 → 16/24/32 或 16/24/32 → 16/24/32）。
    /// 源分支 1/4/8（查 Temp.ColorTable）、16/24/32（pfGetRGB / BGR）；
    /// 目标分支只处理 16/24/32（pfRGB / BGR 写入）；源与目标都是 &lt;=8 时不会调用本过程。
    /// </summary>
    private void PaletteToRGB_or_RGBToRGB(TDIB Temp)
    {
        byte cR = 0, cG = 0, cB = 0;

        for (int Y = 0; Y <= Height - 1; Y++)
        {
            IntPtr SrcP = Temp.ScanLine(Y);
            IntPtr DestP = ScanLine(Y);

            unsafe
            {
                byte* src = (byte*)SrcP;
                byte* dst = (byte*)DestP;

                for (int X = 0; X <= Width - 1; X++)
                {
                    switch (Temp.BitCount)
                    {
                        case 1:
                            {
                                var q = Temp.ColorTable[((uint)src[X >> 3] & DIBConstants.Mask1[X & 7]) >> (int)DIBConstants.Shift1[X & 7]];
                                cR = q.rgbRed; cG = q.rgbGreen; cB = q.rgbBlue;
                            }
                            break;
                        case 4:
                            {
                                var q = Temp.ColorTable[((uint)src[X >> 1] & DIBConstants.Mask4[X & 1]) >> (int)DIBConstants.Shift4[X & 1]];
                                cR = q.rgbRed; cG = q.rgbGreen; cB = q.rgbBlue;
                            }
                            break;
                        case 8:
                            {
                                var q = Temp.ColorTable[src[0]];
                                cR = q.rgbRed; cG = q.rgbGreen; cB = q.rgbBlue;
                                src++;
                            }
                            break;
                        case 16:
                            DIB.pfGetRGB(Temp.NowPixelFormat, *(ushort*)src, out cR, out cG, out cB);
                            src += 2;
                            break;
                        case 24:
                            cR = src[2]; cG = src[1]; cB = src[0];
                            src += 3;
                            break;
                        case 32:
                            DIB.pfGetRGB(Temp.NowPixelFormat, *(uint*)src, out cR, out cG, out cB);
                            src += 4;
                            break;
                    }

                    switch (BitCount)
                    {
                        case 16:
                            *(ushort*)dst = (ushort)DIB.pfRGB(NowPixelFormat, cR, cG, cB);
                            dst += 2;
                            break;
                        case 24:
                            dst[0] = cB; dst[1] = cG; dst[2] = cR;
                            dst += 3;
                            break;
                        case 32:
                            *(uint*)dst = DIB.pfRGB(NowPixelFormat, cR, cG, cB);
                            dst += 4;
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// DIB.pas 2315-2524 1:1。
    /// Size=0 → Exit；Temp.Assign(Self)；SetSize(Temp.Width, Temp.Height, ABitCount)；
    /// 若 SetSize 后 FImage 竟与 Temp.FImage 相同则 Exit（原文的防御）；
    /// 四象限：(≤8,≤8) → 升位深用 PaletteToPalette_Inc / 降位深用半色调调色板 + Canvas.Draw；
    /// (≤8,&gt;8) 与 (&gt;8,&gt;8) → PaletteToRGB_or_RGBtoRGB；(&gt;8,≤8) → 半色调 + Canvas.Draw。
    /// </summary>
    public void ConvertBitCount(int ABitCount)
    {
        if (Size == 0) return;

        var Temp = new TDIB();
        try
        {
            Temp.Assign(this);
            SetSize(Temp.Width, Temp.Height, ABitCount);

            if (ReferenceEquals(FImage, Temp.FImage)) return;

            if (Temp.BitCount <= 8 && BitCount <= 8)
            {
                // The image is converted from the palette color image into the palette color image.
                if (Temp.BitCount <= BitCount)
                {
                    PaletteToPalette_Inc(Temp);
                }
                else
                {
                    switch (BitCount)
                    {
                        case 1:
                            ColorTable[0] = DIB.RGBQuad(0, 0, 0);
                            ColorTable[1] = DIB.RGBQuad(255, 255, 255);
                            break;
                        case 4:
                            CreateHalftonePalette(1, 2, 1);
                            break;
                        case 8:
                            CreateHalftonePalette(3, 3, 2);
                            break;
                    }
                    UpdatePalette();

                    Canvas.Draw(0, 0, Temp);
                }
            }
            else if (Temp.BitCount <= 8 && BitCount > 8)
            {
                // The image is converted from the palette color image into the rgb color image.
                PaletteToRGB_or_RGBToRGB(Temp);
            }
            else if (Temp.BitCount > 8 && BitCount <= 8)
            {
                // The image is converted from the rgb color image into the palette color image.
                switch (BitCount)
                {
                    case 1:
                        ColorTable[0] = DIB.RGBQuad(0, 0, 0);
                        ColorTable[1] = DIB.RGBQuad(255, 255, 255);
                        break;
                    case 4:
                        CreateHalftonePalette(1, 2, 1);
                        break;
                    case 8:
                        CreateHalftonePalette(3, 3, 2);
                        break;
                }
                UpdatePalette();

                Canvas.Draw(0, 0, Temp);
            }
            else if (Temp.BitCount > 8 && BitCount > 8)
            {
                // The image is converted from the rgb color image into the rgb color image.
                PaletteToRGB_or_RGBToRGB(Temp);
            }
        }
        finally
        {
            Temp.Destroy();
        }
    }

    // =========================================================================================
    // DIB.pas 2528-2567 —— 进度
    // =========================================================================================

    /// <summary>DIB.pas 2528-2536 1:1（TGraphic.Progress → OnProgress 事件）。</summary>
    public void StartProgress(string Name)
    {
        FProgressName = Name;
        FProgressOld = 0;
        FProgressOldTime = unchecked((uint)Environment.TickCount);
        FProgressY = 0;
        FProgressOldY = 0;
        Progress(TProgressStage.psStarting, 0, false, TDxRect.Rect(0, 0, Width, Height), FProgressName);
    }

    /// <summary>DIB.pas 2538-2541 1:1。</summary>
    public void EndProgress()
    {
        Progress(TProgressStage.psEnding, 100, true, TDxRect.Rect(0, (int)FProgressOldY, Width, Height), FProgressName);
    }

    /// <summary>
    /// DIB.pas 2543-2567 1:1（DWord 环绕算术照抄）。
    /// Redraw 条件 = (GetTickCount - FProgressOldTime &gt; 200) and (FProgressY - FProgressOldY &gt; 32) and
    /// (((Height div 3 &gt; FProgressY) and (FProgressOldY = 0)) or (FProgressOldY &lt;&gt; 0))；
    /// Percent = PercentY * 100 div Height；只有 Percent 变化或 Redraw 时才发进度并最后 Inc(FProgressY)。
    /// </summary>
    public void UpdateProgress(int PercentY)
    {
        uint now = unchecked((uint)Environment.TickCount);
        bool Redraw = (now - FProgressOldTime > 200) && (FProgressY - FProgressOldY > 32) &&
                      (((Height / 3 > (int)FProgressY) && (FProgressOldY == 0)) || (FProgressOldY != 0));

        uint Percent = unchecked((uint)(PercentY * 100 / Height));

        if (Percent != FProgressOld || Redraw)
        {
            Progress(TProgressStage.psRunning, (int)Percent, Redraw,
                TDxRect.Rect(0, (int)FProgressOldY, Width, (int)FProgressY), FProgressName);
            if (Redraw)
            {
                FProgressOldY = FProgressY;
                FProgressOldTime = now;
            }

            FProgressOld = Percent;
        }

        FProgressY++;
    }

    /// <summary>TGraphic.Progress 的托管等价。</summary>
    private void Progress(TProgressStage Stage, int Percent, bool Redraw, TDxRect Rect, string Name)
    {
        OnProgress?.Invoke(this, Stage, Percent, Redraw, Rect, Name);
        // 原文未对 OnProgress 为 nil 做处理（TGraphic.Progress 内部判 nil）
    }

    // ---- 内部：供其它分片使用的私有状态访问（TDIB 字段本身就是 internal） ----

    /// <summary>DIB.pas 1704/2217 等处的 FImage 访问器（测试与分片实现用）。</summary>
    public TDIBSharedImage SharedImage => FImage;
}

/// <summary>
/// VCL TCanvas 的托管接缝（DIB.pas 里的 `Canvas` 属性 + `Canvas.Draw(0,0,X)`）。
/// Handle 对应 FImage.FDC；OnChanging 对应 TCanvas.OnChanging（DIB.pas 1893）。
/// </summary>
public sealed class TDibCanvas
{
    private IntPtr _handle;
    private readonly Action<object> _onChanging;

    public TDibCanvas(IntPtr handle, Action<object> onChanging)
    {
        _handle = handle;
        _onChanging = onChanging;
    }

    /// <summary>TCanvas.Handle（可写；DIB.pas 1892/2220/2227 会赋值）。</summary>
    public IntPtr Handle
    {
        get => _handle;
        set => _handle = value;
    }

    /// <summary>TCanvas.CopyMode（DIB.pas 1674/1679 传给 StretchDIBits/StretchBlt）。</summary>
    public uint CopyMode => DibSeams.Canvas?.CopyMode ?? 0x00CC0020; // SRCCOPY

    /// <summary>TCanvas.Draw(X, Y, Graphic)。</summary>
    public void Draw(int x, int y, TDIB source)
    {
        _onChanging?.Invoke(this);
        DibSeams.Canvas?.Draw(Handle, x, y, source);
    }

    /// <summary>TCanvas.Draw(X, Y, Graphic)——非 TDIB 图形源。</summary>
    public void Draw(int x, int y, TDibGraphicSource source)
    {
        _onChanging?.Invoke(this);
        DibSeams.Canvas?.DrawGraphic(Handle, x, y, source);
    }
}

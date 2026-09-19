using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GXX.Core.Rtl;

namespace GXX.Client.ReadResources;

// =====================================================================================
// GameImages.pas / DIB.pas / DxCanvas.pas 的**最小接缝**层。
//
// 本车道（ReadResources：Wzl/Wis/Uib）只移植资源格式解析；下列类型是解析结果落地所必需的
// 最小承载物，全部 1:1 对应原文的语义，但**不**属于本车道范围的完整移植：
//   - 接缝：TDib        ← DIB.pas TDIB（此处只保留 8bit 扫描线 + 调色板，够 Wzl/Wis 用）
//   - 接缝：TTexture*   ← DxCanvas.pas NewTexture/NewTextureGray/NewTextureBright/NullTexture
//   - 接缝：DebugTextOut← GameImages.pas DebugTextOut / TGameImages.OutMessage
// 待 DIB.pas / DxCanvas.pas / HGE.pas 移植后接入（见 GameImagesSeams）。
// =====================================================================================

/// <summary>
/// HGE.pas TTexture（纹理句柄）+ NULLTexture() 哨兵的三态表示。
/// 原文有三种状态且语义不同，必须在类型上区分：
///   - nil         → 尚未尝试装载
///   - NULLTexture → NullTexture() 返回的 1x1 占位纹理（“装载过，但该图不可用”）
///   - 其它         → 真实纹理
/// 对应原文 DxCanvas.pas 83-86：
/// <code>function NullTexture():TTexture; begin Result := GameCanvas.HGE.Texture_Create(1, 1); end;</code>
/// </summary>
public readonly struct TTextureRef : IEquatable<TTextureRef>
{
    /// <summary>纹理句柄（IntPtr.Zero = 无）。</summary>
    public readonly IntPtr Handle;

    /// <summary>是否已赋过值（含赋为 NULLTexture）。</summary>
    public readonly bool Assigned;

    /// <summary>是否为 NullTexture() 哨兵。</summary>
    public readonly bool IsNullTexture;

    private TTextureRef(IntPtr handle, bool assigned, bool isNullTexture)
    {
        Handle = handle;
        Assigned = assigned;
        IsNullTexture = isNullTexture;
    }

    /// <summary>Delphi nil。</summary>
    public static readonly TTextureRef Nil = default;

    /// <summary>DELPHI NULLTexture（哨兵）。</summary>
    public static TTextureRef NullTextureSentinel => new(IntPtr.Zero, true, true);

    /// <summary>真实纹理句柄。</summary>
    public static TTextureRef FromHandle(IntPtr handle)
        => handle == IntPtr.Zero ? Nil : new TTextureRef(handle, true, false);

    /// <summary>对应原文 <c>X := nil</c>。</summary>
    public static implicit operator TTextureRef(NilMarker _) => Nil;

    public bool Equals(TTextureRef other)
        => Handle == other.Handle && Assigned == other.Assigned && IsNullTexture == other.IsNullTexture;

    public override bool Equals(object? obj) => obj is TTextureRef t && Equals(t);

    public override int GetHashCode() => HashCode.Combine(Handle, Assigned, IsNullTexture);

    public static bool operator ==(TTextureRef a, TTextureRef b) => a.Equals(b);

    public static bool operator !=(TTextureRef a, TTextureRef b) => !a.Equals(b);

    public override string ToString()
        => !Assigned ? "nil" : IsNullTexture ? "NULLTexture" : $"Texture(0x{Handle.ToInt64():X})";
}

/// <summary>用于书写 <c>Surface = NilTextureMarker.Value</c> 的空值标记（见 TTextureRef 隐式转换）。</summary>
public sealed class NilMarker
{
    private NilMarker() { }
    public static readonly NilMarker Value = new();
}

/// <summary>
/// DIB.pas TDIB 的最小可用子集（1:1 部分）：
/// 8bit 调色板位图（WidthBytes = ((W*8)+31) shr 5 * 4，ScanLine[Y] 自上而下第 Y 行，
/// SetSize 后内容为 0 —— 对应 <c>Canvas.Brush.Color := clBlack; Canvas.FillRect(ClipRect)</c> 的初始黑底）。
/// </summary>
public sealed class TDib
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int BitCount { get; private set; }

    /// <summary>DIB.pas 847：<c>FWidthBytes := (((AWidth * ABitCount) + 31) shr 5) * 4;</c></summary>
    public int WidthBytes { get; private set; }

    /// <summary>DIB.pas 849：<c>FSize := FWidthBytes * FHeight;</c></summary>
    public int Size => WidthBytes * Height;

    private byte[] _bits = Array.Empty<byte>();

    public static TDib Create() => new();

    /// <summary>DIB.pas TDIB.SetSize（仅 8bit 路径）。</summary>
    public void SetSize(int aWidth, int aHeight, int aBitCount)
    {
        Width = aWidth;
        Height = aHeight;
        BitCount = aBitCount;
        WidthBytes = aBitCount == 8 ? (((aWidth * 8) + 31) >> 5) * 4 : (((aWidth * aBitCount) + 31) >> 5) * 4;
        _bits = new byte[Math.Max(Size, 0)];
    }

    /// <summary>DIB.pas TDIB.GetScanLine[Y]（自上而下第 Y 行首地址的托管镜像）。</summary>
    public int ScanLineOffset(int y) => y * WidthBytes;

    /// <summary>把 <paramref name="count"/> 个字节写到 ScanLine[<paramref name="y"/>] 起始处。</summary>
    public void WriteScanLine(int y, byte[] src, int srcOffset, int count)
    {
        if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
        Array.Copy(src, srcOffset, _bits, ScanLineOffset(y), count);
    }

    /// <summary>读 ScanLine[<paramref name="y"/>] 起 <paramref name="count"/> 字节（越界补 0，对齐原文越界读的容错意图）。</summary>
    public byte[] ReadScanLine(int y, int count)
    {
        var dst = new byte[count];
        if (y < 0 || y >= Height) return dst;
        int off = ScanLineOffset(y);
        int avail = Math.Min(count, _bits.Length - off);
        if (avail > 0) Array.Copy(_bits, off, dst, 0, avail);
        return dst;
    }

    /// <summary>整块位数据（长度 = Size）。</summary>
    public byte[] Bits => _bits;

    /// <summary>取 ScanLine[Y] 的第 X 字节（测试/断言用）。</summary>
    public byte Pixel(int x, int y) => _bits[ScanLineOffset(y) + x];
}

/// <summary>
/// Wzl.pas/Wis.pas 装载流程的纹理生成接缝（对应 DxCanvas.pas NewTexture 系列 + GameImages.MakeDibByPixelFormat）。
/// <para>原文调用序列：<c>Source := MakeDibByPixelFormat(pf, w, h)</c> → 逐行/解压填 PBits →
/// <c>NewTexture(Source)</c> / <c>NewTextureGray(Source)</c> / <c>NewTextureBright(Source)</c>；
/// 另有 <c>NewTexture(FileData, FileSize, w, h, TransparentColor, D3DFormat)</c> 的 D3D 路径。</para>
/// 接缝：待 DIB.pas / DxCanvas.pas / HGE.pas 移植后接入（此处默认实现走 <see cref="IHgeTextureBackend"/> 风格的回调）。
/// </summary>
public static class TextureSeams
{
    /// <summary>NewTexture(Source:TDIB; Alpha:TDIB = nil)。</summary>
    public static Func<TDib, TDib?, IntPtr> NewTextureFn = DefaultNewTexture;

    /// <summary>NewTextureGray(Source:TDIB; Alpha:TDIB = nil)。</summary>
    public static Func<TDib, TDib?, IntPtr> NewTextureGrayFn = DefaultNewTexture;

    /// <summary>NewTextureBright(Source:TDIB; Alpha:TDIB = nil)。</summary>
    public static Func<TDib, TDib?, IntPtr> NewTextureBrightFn = DefaultNewTexture;

    /// <summary>Texture_Free（HGE.pas）。</summary>
    public static Action<IntPtr> TextureFreeFn = _ => { };

    /// <summary>已生成纹理的托管镜像（句柄 → 像素），供 headless 测试断言；渲染层接入后可为空。</summary>
    private static readonly Dictionary<IntPtr, Bitmap> _images = new();
    private static int _nextHandle = 0x1000;

    public static Bitmap? GetImage(IntPtr handle)
        => _images.TryGetValue(handle, out var bmp) ? bmp : null;

    /// <summary>
    /// GDIPlusHge 默认实现：把 8bit 索引 + g_DefColorTable 调色板展开为 32bpp ARGB 位图并登记句柄。
    /// Alpha（对应原文 ResetWZLAlpha 解析出的每像素 alpha 平面）非 nil 时按乘性 alpha 写入。
    /// </summary>
    public static IntPtr DefaultNewTexture(TDib source, TDib? alpha)
    {
        if (source == null || source.Width <= 0 || source.Height <= 0) return IntPtr.Zero;

        var bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        var rect = new Rectangle(0, 0, source.Width, source.Height);
        var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            var buf = new byte[data.Stride * source.Height];
            for (int y = 0; y < source.Height; y++)
            {
                int rowOff = y * data.Stride;
                int lineOff = source.ScanLineOffset(y);
                for (int x = 0; x < source.Width; x++)
                {
                    int argb = GDefColorTable.GetARGB32(source.Bits[lineOff + x]); // Cardinal(pal1) or $FF000000
                    if (alpha != null)
                        argb = (argb & 0x00FFFFFF) | (alpha.Pixel(x, y) << 24);
                    buf[rowOff + (x * 4) + 0] = (byte)(argb & 0xFF);
                    buf[rowOff + (x * 4) + 1] = (byte)((argb >> 8) & 0xFF);
                    buf[rowOff + (x * 4) + 2] = (byte)((argb >> 16) & 0xFF);
                    buf[rowOff + (x * 4) + 3] = (byte)((argb >> 24) & 0xFF);
                }
            }

            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
        }
        finally
        {
            bmp.UnlockBits(data);
        }

        IntPtr handle = new IntPtr(_nextHandle++);
        _images[handle] = bmp;
        return handle;
    }

    /// <summary>测试用：清空句柄镜像并发复位。</summary>
    public static void ResetForTest()
    {
        _images.Clear();
        _nextHandle = 0x1000;
        NewTextureFn = DefaultNewTexture;
        NewTextureGrayFn = DefaultNewTexture;
        NewTextureBrightFn = DefaultNewTexture;
        TextureFreeFn = _ => { };
    }
}

/// <summary>
/// GameImages.pas <c>ColorArray[0..1023]</c>（190-255 行）的 1:1 抽取：
/// 256 × TRGBQuad（顺序 B,G,R,Reserved），运行时由
/// <c>initialization Move(ColorArray, g_DefColorTable, SizeOf(g_DefColorTable))</c>（1828 行）灌入 g_DefColorTable。
/// <para>本表由脚本从原文抽取生成（见 ResourceWzlTests 的校验和断言），非手工转录。</para>
/// </summary>
public static class GDefColorTable
{
    /// <summary>g_DefColorTable 的原始字节视图（1024 字节，B,G,R,0 × 256）。</summary>
    public static readonly byte[] ColorArray = new byte[1024]
    {
        0x00,0x00,0x00,0x00, 0x00,0x00,0x80,0x00, 0x00,0x80,0x00,0x00, 0x00,0x80,0x80,0x00,
        0x80,0x00,0x00,0x00, 0x80,0x00,0x80,0x00, 0x80,0x80,0x00,0x00, 0xC0,0xC0,0xC0,0x00,
        0x97,0x80,0x55,0x00, 0xC8,0xB9,0x9D,0x00, 0x73,0x73,0x7B,0x00, 0x29,0x29,0x2D,0x00,
        0x52,0x52,0x5A,0x00, 0x5A,0x5A,0x63,0x00, 0x39,0x39,0x42,0x00, 0x18,0x18,0x1D,0x00,
        0x10,0x10,0x18,0x00, 0x18,0x18,0x29,0x00, 0x08,0x08,0x10,0x00, 0x71,0x79,0xF2,0x00,
        0x5F,0x67,0xE1,0x00, 0x5A,0x5A,0xFF,0x00, 0x31,0x31,0xFF,0x00, 0x52,0x5A,0xD6,0x00,
        0x00,0x10,0x94,0x00, 0x18,0x29,0x94,0x00, 0x00,0x08,0x39,0x00, 0x00,0x10,0x73,0x00,
        0x00,0x18,0xB5,0x00, 0x52,0x63,0xBD,0x00, 0x10,0x18,0x42,0x00, 0x99,0xAA,0xFF,0x00,
        0x00,0x10,0x5A,0x00, 0x29,0x39,0x73,0x00, 0x31,0x4A,0xA5,0x00, 0x73,0x7B,0x94,0x00,
        0x31,0x52,0xBD,0x00, 0x10,0x21,0x52,0x00, 0x18,0x31,0x7B,0x00, 0x10,0x18,0x2D,0x00,
        0x31,0x4A,0x8C,0x00, 0x00,0x29,0x94,0x00, 0x00,0x31,0xBD,0x00, 0x52,0x73,0xC6,0x00,
        0x18,0x31,0x6B,0x00, 0x42,0x6B,0xC6,0x00, 0x00,0x4A,0xCE,0x00, 0x39,0x63,0xA5,0x00,
        0x18,0x31,0x5A,0x00, 0x00,0x10,0x2A,0x00, 0x00,0x08,0x15,0x00, 0x00,0x18,0x3A,0x00,
        0x00,0x00,0x08,0x00, 0x00,0x00,0x29,0x00, 0x00,0x00,0x4A,0x00, 0x00,0x00,0x9D,0x00,
        0x00,0x00,0xDC,0x00, 0x00,0x00,0xDE,0x00, 0x00,0x00,0xFB,0x00, 0x52,0x73,0x9C,0x00,
        0x4A,0x6B,0x94,0x00, 0x29,0x4A,0x73,0x00, 0x18,0x31,0x52,0x00, 0x18,0x4A,0x8C,0x00,
        0x11,0x44,0x88,0x00, 0x00,0x21,0x4A,0x00, 0x10,0x18,0x21,0x00, 0x5A,0x94,0xD6,0x00,
        0x21,0x6B,0xC6,0x00, 0x00,0x6B,0xEF,0x00, 0x00,0x77,0xFF,0x00, 0x84,0x94,0xA5,0x00,
        0x21,0x31,0x42,0x00, 0x08,0x10,0x18,0x00, 0x08,0x18,0x29,0x00, 0x00,0x10,0x21,0x00,
        0x18,0x29,0x39,0x00, 0x39,0x63,0x8C,0x00, 0x10,0x29,0x42,0x00, 0x18,0x42,0x6B,0x00,
        0x18,0x4A,0x7B,0x00, 0x00,0x4A,0x94,0x00, 0x7B,0x84,0x8C,0x00, 0x5A,0x63,0x6B,0x00,
        0x39,0x42,0x4A,0x00, 0x18,0x21,0x29,0x00, 0x29,0x39,0x46,0x00, 0x94,0xA5,0xB5,0x00,
        0x5A,0x6B,0x7B,0x00, 0x94,0xB1,0xCE,0x00, 0x73,0x8C,0xA5,0x00, 0x5A,0x73,0x8C,0x00,
        0x73,0x94,0xB5,0x00, 0x73,0xA5,0xD6,0x00, 0x4A,0xA5,0xEF,0x00, 0x8C,0xC6,0xEF,0x00,
        0x42,0x63,0x7B,0x00, 0x39,0x56,0x6B,0x00, 0x5A,0x94,0xBD,0x00, 0x00,0x39,0x63,0x00,
        0xAD,0xC6,0xD6,0x00, 0x29,0x42,0x52,0x00, 0x18,0x63,0x94,0x00, 0xAD,0xD6,0xEF,0x00,
        0x63,0x8C,0xA5,0x00, 0x4A,0x5A,0x63,0x00, 0x7B,0xA5,0xBD,0x00, 0x18,0x42,0x5A,0x00,
        0x31,0x8C,0xBD,0x00, 0x29,0x31,0x35,0x00, 0x63,0x84,0x94,0x00, 0x4A,0x6B,0x7B,0x00,
        0x5A,0x8C,0xA5,0x00, 0x29,0x4A,0x5A,0x00, 0x39,0x7B,0x9C,0x00, 0x10,0x31,0x42,0x00,
        0x21,0xAD,0xEF,0x00, 0x00,0x10,0x18,0x00, 0x00,0x21,0x29,0x00, 0x00,0x6B,0x9C,0x00,
        0x5A,0x84,0x94,0x00, 0x18,0x42,0x52,0x00, 0x29,0x5A,0x6B,0x00, 0x21,0x63,0x7B,0x00,
        0x21,0x7B,0x9C,0x00, 0x00,0xA5,0xDE,0x00, 0x39,0x52,0x5A,0x00, 0x10,0x29,0x31,0x00,
        0x7B,0xBD,0xCE,0x00, 0x39,0x5A,0x63,0x00, 0x4A,0x84,0x94,0x00, 0x29,0xA5,0xC6,0x00,
        0x18,0x9C,0x10,0x00, 0x4A,0x8C,0x42,0x00, 0x42,0x8C,0x31,0x00, 0x29,0x94,0x10,0x00,
        0x10,0x18,0x08,0x00, 0x18,0x18,0x08,0x00, 0x10,0x29,0x08,0x00, 0x29,0x42,0x18,0x00,
        0xAD,0xB5,0xA5,0x00, 0x73,0x73,0x6B,0x00, 0x29,0x29,0x18,0x00, 0x4A,0x42,0x18,0x00,
        0x4A,0x42,0x31,0x00, 0xDE,0xC6,0x63,0x00, 0xFF,0xDD,0x44,0x00, 0xEF,0xD6,0x8C,0x00,
        0x39,0x6B,0x73,0x00, 0x39,0xDE,0xF7,0x00, 0x8C,0xEF,0xF7,0x00, 0x00,0xE7,0xF7,0x00,
        0x5A,0x6B,0x6B,0x00, 0xA5,0x8C,0x5A,0x00, 0xEF,0xB5,0x39,0x00, 0xCE,0x9C,0x4A,0x00,
        0xB5,0x84,0x31,0x00, 0x6B,0x52,0x31,0x00, 0xD6,0xDE,0xDE,0x00, 0xB5,0xBD,0xBD,0x00,
        0x84,0x8C,0x8C,0x00, 0xDE,0xF7,0xF7,0x00, 0x18,0x08,0x00,0x00, 0x39,0x18,0x08,0x00,
        0x29,0x10,0x08,0x00, 0x00,0x18,0x08,0x00, 0x00,0x29,0x08,0x00, 0xA5,0x52,0x00,0x00,
        0xDE,0x7B,0x00,0x00, 0x4A,0x29,0x10,0x00, 0x6B,0x39,0x10,0x00, 0x8C,0x52,0x10,0x00,
        0xA5,0x5A,0x21,0x00, 0x5A,0x31,0x10,0x00, 0x84,0x42,0x10,0x00, 0x84,0x52,0x31,0x00,
        0x31,0x21,0x18,0x00, 0x7B,0x5A,0x4A,0x00, 0xA5,0x6B,0x52,0x00, 0x63,0x39,0x29,0x00,
        0xDE,0x4A,0x10,0x00, 0x21,0x29,0x29,0x00, 0x39,0x4A,0x4A,0x00, 0x18,0x29,0x29,0x00,
        0x29,0x4A,0x4A,0x00, 0x42,0x7B,0x7B,0x00, 0x4A,0x9C,0x9C,0x00, 0x29,0x5A,0x5A,0x00,
        0x14,0x42,0x42,0x00, 0x00,0x39,0x39,0x00, 0x00,0x59,0x59,0x00, 0x2C,0x35,0xCA,0x00,
        0x21,0x73,0x6B,0x00, 0x00,0x31,0x29,0x00, 0x10,0x39,0x31,0x00, 0x18,0x39,0x31,0x00,
        0x00,0x4A,0x42,0x00, 0x18,0x63,0x52,0x00, 0x29,0x73,0x5A,0x00, 0x18,0x4A,0x31,0x00,
        0x00,0x21,0x18,0x00, 0x00,0x31,0x18,0x00, 0x10,0x39,0x18,0x00, 0x4A,0x84,0x63,0x00,
        0x4A,0xBD,0x6B,0x00, 0x4A,0xB5,0x63,0x00, 0x4A,0xBD,0x63,0x00, 0x4A,0x9C,0x5A,0x00,
        0x39,0x8C,0x4A,0x00, 0x4A,0xC6,0x63,0x00, 0x4A,0xD6,0x63,0x00, 0x4A,0x84,0x52,0x00,
        0x29,0x73,0x31,0x00, 0x5A,0xC6,0x63,0x00, 0x4A,0xBD,0x52,0x00, 0x00,0xFF,0x10,0x00,
        0x18,0x29,0x18,0x00, 0x4A,0x88,0x4A,0x00, 0x4A,0xE7,0x4A,0x00, 0x00,0x5A,0x00,0x00,
        0x00,0x88,0x00,0x00, 0x00,0x94,0x00,0x00, 0x00,0xDE,0x00,0x00, 0x00,0xEE,0x00,0x00,
        0x00,0xFB,0x00,0x00, 0x94,0x5A,0x4A,0x00, 0xB5,0x73,0x63,0x00, 0xD6,0x8C,0x7B,0x00,
        0xD6,0x7B,0x6B,0x00, 0xFF,0x88,0x77,0x00, 0xCE,0xC6,0xC6,0x00, 0x9C,0x94,0x94,0x00,
        0xC6,0x94,0x9C,0x00, 0x39,0x31,0x31,0x00, 0x84,0x18,0x29,0x00, 0x84,0x00,0x18,0x00,
        0x52,0x42,0x4A,0x00, 0x7B,0x42,0x52,0x00, 0x73,0x5A,0x63,0x00, 0xF7,0xB5,0xCE,0x00,
        0x9C,0x7B,0x8C,0x00, 0xCC,0x22,0x77,0x00, 0xFF,0xAA,0xDD,0x00, 0x2A,0xB4,0xF0,0x00,
        0x9F,0x00,0xDF,0x00, 0xB3,0x17,0xE3,0x00, 0xF0,0xFB,0xFF,0x00, 0xA4,0xA0,0xA0,0x00,
        0x80,0x80,0x80,0x00, 0x00,0x00,0xFF,0x00, 0x00,0xFF,0x00,0x00, 0x00,0xFF,0xFF,0x00,
        0xFF,0x00,0x00,0x00, 0xFF,0x00,0xFF,0x00, 0xFF,0xFF,0x00,0x00, 0xFF,0xFF,0xFF,0x00,
    };

    /// <summary>GameImages.pas 1795：<c>ColorTable_8_32Bit[I] := Cardinal(pal1) or $FF000000;</c></summary>
    public static int GetARGB32(int index)
    {
        int i = index * 4;
        return (0xFF << 24) | (ColorArray[i + 2] << 16) | (ColorArray[i + 1] << 8) | ColorArray[i];
    }

    /// <summary>GameImages.pas 1793：<c>pal1 := g_DefColorTable[I]; if Integer(pal1) &lt;&gt; 0 then ...</c> 的判定。</summary>
    public static bool IsNonZero(int index)
    {
        int i = index * 4;
        return (ColorArray[i] | ColorArray[i + 1] | ColorArray[i + 2] | ColorArray[i + 3]) != 0;
    }
}

/// <summary>
/// GameImages.pas 常量（12-20 行）。
/// </summary>
public static class GameImagesConsts
{
    public const int LOADIMAGEMODE = 0;

    /// <summary>GameImages.pas 14：<c>USEMAPSTREAM = 0;</c>（Wzl.pas/Wis.pas 的 $IF 分支据此确定）。</summary>
    public const int USEMAPSTREAM = 0;

    /// <summary>
    /// GameImages.pas 17：<c>MAX_IMAGE_SIZE = 8 shl 10 shl 10; // 单张图片大小不能超过4M</c>
    /// <para>注意：Delphi 7 中常量表达式按 Integer 运算，8 shl 10 shl 10 = $800000 = 8388608（即注释「4M」有误）。</para>
    /// </summary>
    public const int MAX_IMAGE_SIZE = 8 << 10 << 10;

    /// <summary>GameImages.pas 19。</summary>
    public const int MAX_IMAGE_WIDTH = 3200;

    /// <summary>GameImages.pas 20。</summary>
    public const int MAX_IMAGE_HEIGHT = 3200;
}

/// <summary>
/// DxCanvas.pas 45：<c>g_DefColorTable:TRGBQuads;</c>（由 GameImages.pas 1828 行 initialization 灌入）。
/// </summary>
public static class DxCanvasGlobal
{
    public static readonly byte[] g_DefColorTable = GDefColorTable.ColorArray;
}

/// <summary>
/// GameImages.pas TDXImage（25-45 行）1:1。
/// 原文 nWidth/nHeight 为 Word，nPx/nPy 为 SmallInt；Surface/Gray/Bright 为 TTexture；
/// dwLatest*Time 为 LongWord，boUpdateStart/boUpdateStop 为 Boolean（Delphi 中 Boolean = 1 字节）。
/// </summary>
public sealed class TDxImage
{
    public ushort nWidth;
    public ushort nHeight;

    public short nPx;
    public short nPy;

    public TTextureRef Bitmap;
    public TTextureRef Surface;
    public TTextureRef Gray;
    public TTextureRef Bright;

    public uint dwLatestTime;
    public uint dwLatestGrayTime;
    public uint dwLatestBrightTime;

    public bool boUpdateStart; // 更新状态
    public uint dwUpdateStartTick;
    public bool boUpdateStop;  // 是否不需要更新
}

/// <summary>
/// GameImages.pas TGameImages（51-158 行）中本车道所需的最小 1:1 子集：
/// m_IndexList / IndexList / GrayIndexList / BrightIndexList / m_ImgArr / ImageCount /
/// FileName / IndexFileName / Initialized / Lock/UnLock / FreeOldMemorys_Ex / OutMessage。
/// </summary>
public abstract class TGameImages
{
    // ---- GameImages.pas public 字段（52-70 行）----
    public uint m_dwFreeMemCheckTick;
    public uint m_dwUseCheckTick;
    public uint m_dwMemCheckTick;
    public uint m_dwMemCheckGrayTick;
    public uint m_dwMemCheckBrightTick;
    public int m_nProcIdx;
    public int m_nProcGrayIdx;
    public int m_nProcBrightIdx;
    public bool m_boUpdateIndex;      // 是否需要更新 索引文件
    public bool m_boUpdateIndexing;   // 正在更新 索引文件
    public uint m_dwUpdateIndexingTick;
    public bool m_boNeedUpdate;       // 是否需要更新
    public List<int> m_IndexList = new();
    public uint m_dwMemChecktTick;

    // ---- GameImages.pas private 字段（71-85 行）----
    private string _fileName = "";
    private string _indexFileName = "";
    private int _imageCount;
    private ushort _appr;
    private byte _bitCount;
    private bool _initialized;
    private int _libType;             // TLibType
    private uint _transparentColor;
    private bool _d3DFormat;
    private bool _resetWZLAlpha;
    private readonly object _lock = new();

    public ushort Appr { get => _appr; set => _appr = value; }
    public byte BitCount { get => _bitCount; set => _bitCount = value; }

    public bool Initialized { get => _initialized; set => _initialized = value; }
    public int LibType { get => _libType; set => _libType = value; }
    public uint TransparentColor { get => _transparentColor; set => _transparentColor = value; }
    public bool D3DFormat { get => _d3DFormat; set => _d3DFormat = value; }
    public bool ResetWZLAlpha { get => _resetWZLAlpha; set => _resetWZLAlpha = value; }

    /// <summary>GameImages.pas 138：property FileName（写入时联动 IndexFileName，见 SetFileName 1109-1121）。</summary>
    public string FileName
    {
        get => _fileName;
        set => SetFileName(value);
    }

    /// <summary>GameImages.pas 139：property IndexFileName。</summary>
    public string IndexFileName { get => _indexFileName; set => _indexFileName = value; }

    /// <summary>GameImages.pas 140：property ImageCount。</summary>
    public int ImageCount { get => _imageCount; set => _imageCount = value; }

    public List<int> IndexList = new();
    public List<int> GrayIndexList = new();
    public List<int> BrightIndexList = new();

    /// <summary>GameImages.pas 104：<c>m_ImgArr:PTDxImageArr;</c>（nil 表示未分配）。</summary>
    public TDxImage[]? m_ImgArr;

    // ---- 全局接缝 ----

    /// <summary>GameImages.pas 176：<c>g_boD3DFormat:Boolean = False;</c></summary>
    public static bool g_boD3DFormat;

    /// <summary>GameImages.pas 178：<c>g_NullImage:TTexture = nil;</c>（GetCached* 失败时的回退值）。</summary>
    public static TTextureRef g_NullImage = default;

    /// <summary>MShare.pas 的全局开关（Wzl/Wis/Uib 的自动更新分支使用；原为 g_boAutoUpdate / g_boDeviceInitializeOK）。</summary>
    public static bool g_boAutoUpdate;
    public static bool g_boDeviceInitializeOK;

    /// <summary>GameImages.pas 181：<c>g_DebugTextOut:TDebugTextOut = nil;</c></summary>
    public static Action<string, bool>? g_DebugTextOut;

    /// <summary>MyGetTickCount（HUtil32/Windows GetTickCount 语义，uint 回绕）。</summary>
    public static Func<uint> MyGetTickCountFn = () => DelphiRTL.GetTickCount();

    public static uint MyGetTickCount() => MyGetTickCountFn();

    /// <summary>UpdateEngine 接缝：g_UpdateEngine.Add(FileName, udt*, Index, Self, fn) 返回是否受理。</summary>
    public static Func<string, int, int, object?, object?, bool>? UpdateEngineAddFn;

    /// <summary>g_UpdateRetryTime（MShare.pas 全局）。</summary>
    public static uint g_UpdateRetryTime = 30000;

    // ---- 方法 ----

    /// <summary>
    /// GameImages.pas 1109-1121 SetFileName 1:1：
    /// TWMImages → .wix；TWzlImages → .wzx；其它 → 与 FileName 相同。
    /// <para>原文以 <c>Self is TWMImages</c> / <c>Self is TWzlImages</c> 判别，此处以 <see cref="IndexFileExtension"/> 表达同一判别。</para>
    /// </summary>
    protected virtual string IndexFileExtension => "";

    public void SetFileName(string value)
    {
        _fileName = value;
        string ext = IndexFileExtension;
        _indexFileName = ext.Length == 0 ? _fileName : ExtractFilePath(_fileName) + ExtractFileNameOnly(_fileName) + ext;
    }

    /// <summary>GameImages.pas 257-263 ExtractFilePath：<c>Copy(FileName, 1, LastDelimiter(PathDelim + DriveDelim, FileName))</c>。</summary>
    public static string ExtractFilePath(string fileName)
    {
        int i = LastDelimiter("\\:", fileName);
        return DelphiRTL.Copy(fileName, 1, i);
    }

    /// <summary>GameImages.pas 265-278 ExtractFileNameOnly。</summary>
    public static string ExtractFileNameOnly(string fname)
    {
        string ext = ExtractFileExt(fname);
        string fn = ExtractFileName(fname);
        if (ext != "")
            return DelphiRTL.Copy(fn, 1, DelphiRTL.Pos(ext, fn) - 1);
        return fn;
    }

    private static string ExtractFileExt(string fileName)
    {
        int i = fileName.LastIndexOf('.');
        return i < 0 ? "" : fileName.Substring(i);
    }

    private static string ExtractFileName(string fileName)
    {
        int i = LastDelimiter("\\:", fileName);
        return i <= 0 ? fileName : fileName.Substring(i);
    }

    /// <summary>SysUtils.LastDelimiter（1-based，未找到返回 0）。</summary>
    private static int LastDelimiter(string delims, string s)
    {
        int result = 0;
        for (int i = 0; i < s.Length; i++)
            if (delims.IndexOf(s[i]) >= 0) result = i + 1;
        return result;
    }

    /// <summary>GameImages.pas 112-114 TryLock / 1140-1142 Lock（原文用 g_LoadCriticalSection）。</summary>
    public void Lock() => System.Threading.Monitor.Enter(_lock);

    /// <summary>GameImages.pas 1089-1092 UnLock。</summary>
    public void UnLock() => System.Threading.Monitor.Exit(_lock);

    /// <summary>GameImages.pas 107：<c>procedure OutMessage(const Msg:string; boWriteDate:Boolean = True);</c></summary>
    public void OutMessage(string msg, bool boWriteDate = true)
    {
        g_DebugTextOut?.Invoke(msg, boWriteDate);
    }

    /// <summary>GameImages.pas 280-284 DebugTextOut。</summary>
    public static void DebugTextOut(string msg, bool boWriteDate = true)
    {
        g_DebugTextOut?.Invoke(msg, boWriteDate);
    }

    /// <summary>
    /// GameImages.pas 1561-… FreeOldMemorys_Ex：此处只做本车道所需的「释放并置 nil」；
    /// 原文的 LRU 回收（m_dwMemCheckTick 计时、m_nProcIdx 游标）依赖 g_* 回收阈值全局，
    /// 接缝：待 GameImages.pas 全量移植后接入（回收策略不影响资源格式解析）。
    /// </summary>
    public void FreeOldMemorys_Ex()
    {
    }

    /// <summary>释放 m_ImgArr 中全部纹理（GameImages.pas 各 Finalize 的公共部分）。</summary>
    protected void FreeImageArray()
    {
        if (m_ImgArr == null) return;
        for (int i = 0; i < ImageCount && i < m_ImgArr.Length; i++)
        {
            FreeSlot(ref m_ImgArr[i].Surface);
            FreeSlot(ref m_ImgArr[i].Gray);
            FreeSlot(ref m_ImgArr[i].Bright);
            FreeSlot(ref m_ImgArr[i].Bitmap);
        }
        m_ImgArr = null;
    }

    private static void FreeSlot(ref TTextureRef slot)
    {
        if (slot.Assigned && !slot.IsNullTexture && slot.Handle != IntPtr.Zero)
        {
            try { TextureSeams.TextureFreeFn(slot.Handle); }
            catch { /* 原文 FreeAndNil 包在 try/except 中并吞掉异常 */ }
        }
        slot = default;
    }

    /// <summary>GameImages.pas 85-… 各子类 Initialize（子类覆写）。</summary>
    public abstract void Initialize();

    /// <summary>GameImages.pas 各子类 Finalize（子类覆写）。</summary>
    public abstract void Finalize_();
}

/// <summary>
/// Delphi <c>ShortString</c>（string[N]）的内存布局助手：
/// 第 0 字节为长度，随后 N 字节为 GBK 字符数据（Delphi 7 AnsiString 语义）。
/// </summary>
public static class ShortStringLayout
{
    /// <summary>按 Delphi ShortString 规则写入：长度截断到 N，剩余字节补 0（原文为未初始化的栈残留，此处取 0）。</summary>
    public static void SetBytes(byte[] buffer, int offset, int maxLen, string value)
    {
        byte[] bytes = GXX.Core.EncodingInit.GBK.GetBytes(value ?? "");
        int len = Math.Min(bytes.Length, maxLen);
        buffer[offset] = (byte)len;
        for (int i = 0; i < maxLen; i++)
            buffer[offset + 1 + i] = i < len ? bytes[i] : (byte)0;
    }

    /// <summary>按长度字节取字符串。</summary>
    public static string GetString(byte[] buffer, int offset, int maxLen)
    {
        int len = buffer[offset];
        if (len > maxLen) len = maxLen;
        return len == 0 ? "" : GXX.Core.EncodingInit.GBK.GetString(buffer, offset + 1, len);
    }
}

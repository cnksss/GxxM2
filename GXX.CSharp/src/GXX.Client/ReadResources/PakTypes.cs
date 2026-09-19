using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Pak.pas 1:1 移植 —— 记录/结构布局层（原文 interface 段 21-111 行）。
//
// 源单元：Source\Client-HGE\ReadResources\Pak.pas
// 全部记录在原文中均标注 **packed record**（唯一例外见 TPakFileHeader 注释），
// 因此逐字段无填充；下面每个类型都给出 Delphi 7 的 SizeOf 推导与 FromBytes/ToBytes 对拍。
//
// 关键点（逐字节 1:1 的依据）：
//   * Delphi 7 AnsiChar = 1 字节，ShortString(N) = 1 + N 字节（第 0 字节是长度）。
//   * Delphi 7 TPixelFormat / Boolean 的 SizeOf 均为 **1**。
//   * Delphi 7 TDateTime = Double（8 字节，OLE 自动化日期：天数为整数部分）。
// =====================================================================================

/// <summary>
/// Pak.pas 22-27：<c>TPakPassword = packed record KeyData:array[0..31] of LongWord;
/// Chain:array[0..19] of Byte; KeyDataLz:array[0..31] of LongWord; ChainLz:array[0..19] of Byte; end;</c>
/// <para>SizeOf = 128 + 20 + 128 + 20 = <b>296</b>。</para>
/// </summary>
public struct TPakPassword
{
    /// <summary>偏移 0：<c>KeyData:array[0..32-1] of LongWord;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public uint[] KeyData;

    /// <summary>偏移 128：<c>Chain:array[0..20 - 1] of Byte;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] Chain;

    /// <summary>偏移 148：<c>KeyDataLz:array[0..32-1] of LongWord;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public uint[] KeyDataLz;

    /// <summary>偏移 276：<c>ChainLz:array[0..20 - 1] of Byte;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] ChainLz;

    /// <summary>Delphi <c>SizeOf(TPakPassword)</c> = 296。</summary>
    public const int SizeOf = 296;

    /// <summary>Delphi 默认（零初始化）构造。</summary>
    public static TPakPassword CreateEmpty() => new()
    {
        KeyData = new uint[32],
        Chain = new byte[20],
        KeyDataLz = new uint[32],
        ChainLz = new byte[20],
    };

    /// <summary>按 packed 布局（小端）反序列化。</summary>
    public static TPakPassword FromBytes(byte[] buf, int offset)
    {
        var p = CreateEmpty();
        for (int i = 0; i < 32; i++) p.KeyData[i] = ReadU32(buf, offset + i * 4);
        Buffer.BlockCopy(buf, offset + 128, p.Chain, 0, 20);
        for (int i = 0; i < 32; i++) p.KeyDataLz[i] = ReadU32(buf, offset + 148 + i * 4);
        Buffer.BlockCopy(buf, offset + 276, p.ChainLz, 0, 20);
        return p;
    }

    /// <summary>按 packed 布局（小端）序列化。</summary>
    public readonly void ToBytes(byte[] buf, int offset)
    {
        for (int i = 0; i < 32; i++) WriteU32(buf, offset + i * 4, KeyData[i]);
        Buffer.BlockCopy(Chain, 0, buf, offset + 128, 20);
        for (int i = 0; i < 32; i++) WriteU32(buf, offset + 148 + i * 4, KeyDataLz[i]);
        Buffer.BlockCopy(ChainLz, 0, buf, offset + 276, 20);
    }

    private static uint ReadU32(byte[] b, int o)
        => (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));

    private static void WriteU32(byte[] b, int o, uint v)
    {
        b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
    }
}

/// <summary>
/// Pak.pas 30-33：<c>TPakIndexHeader = packed record OffSet:Integer; Length:Integer; end;</c>
/// <para>SizeOf = <b>8</b>。原文在 interface 段声明但 implementation 段从未使用（仅注释里出现），
/// 本移植保留类型以维持 1:1 清单完整性。</para>
/// </summary>
public struct TPakIndexHeader
{
    public int OffSet;
    public int Length;

    public const int SizeOf = 8;

    public static TPakIndexHeader FromBytes(byte[] buf, int offset) => new()
    {
        OffSet = BitConverter.ToInt32(buf, offset),
        Length = BitConverter.ToInt32(buf, offset + 4),
    };

    public readonly void ToBytes(byte[] buf, int offset)
    {
        BitConverter.GetBytes(OffSet).CopyTo(buf, offset);
        BitConverter.GetBytes(Length).CopyTo(buf, offset + 4);
    }
}

/// <summary>
/// Pak.pas 38-48：<c>TPakImageInfo = packed record btEncr0,btEncr1,bt2,bt3:Byte;
/// wW,wH,wPx,wPy:Smallint; end;</c>
/// <para>SizeOf = 4 + 8 = <b>12</b>。LzPak V0 的图片头。</para>
/// </summary>
public struct TPakImageInfo
{
    public byte btEncr0;  // 0x00
    public byte btEncr1;  // 0x01
    public byte bt2;      // 0x02
    public byte bt3;      // 0x03
    public short wW;      // 0x04
    public short wH;      // 0x06
    public short wPx;     // 0x08
    public short wPy;     // 0x0A

    public const int SizeOf = 12;

    public static TPakImageInfo FromBytes(byte[] buf, int offset)
    {
        var v = default(TPakImageInfo);
        v.btEncr0 = buf[offset + 0];
        v.btEncr1 = buf[offset + 1];
        v.bt2 = buf[offset + 2];
        v.bt3 = buf[offset + 3];
        v.wW = BitConverter.ToInt16(buf, offset + 4);
        v.wH = BitConverter.ToInt16(buf, offset + 6);
        v.wPx = BitConverter.ToInt16(buf, offset + 8);
        v.wPy = BitConverter.ToInt16(buf, offset + 10);
        return v;
    }

    public readonly void ToBytes(byte[] buf, int offset)
    {
        buf[offset + 0] = btEncr0; buf[offset + 1] = btEncr1; buf[offset + 2] = bt2; buf[offset + 3] = bt3;
        BitConverter.GetBytes(wW).CopyTo(buf, offset + 4);
        BitConverter.GetBytes(wH).CopyTo(buf, offset + 6);
        BitConverter.GetBytes(wPx).CopyTo(buf, offset + 8);
        BitConverter.GetBytes(wPy).CopyTo(buf, offset + 10);
    }
}

/// <summary>
/// Pak.pas 50-61：<c>TNewPakImageInfo = packed record PixelFormat:TPixelFormat; bt2,bt3:Byte;
/// boAlpha:Boolean; nWidth,nHeight,px,py:SmallInt; Length:Integer; end;</c>
/// <para>SizeOf = 1 + 1 + 1 + 1 + 2 + 2 + 2 + 2 + 4 = <b>16</b>（TPixelFormat / Boolean 各 1 字节）。</para>
/// </summary>
public struct TNewPakImageInfo
{
    public byte PixelFormat; // TPixelFormat（SizeOf = 1）
    public byte bt2;
    public byte bt3;
    public bool boAlpha;     // 是否有通道数据（Boolean，1 字节：0=False，非 0=True）
    public short nWidth;
    public short nHeight;
    public short px;
    public short py;
    public int Length;

    public const int SizeOf = 16;

    public static TNewPakImageInfo FromBytes(byte[] buf, int offset)
    {
        var v = default(TNewPakImageInfo);
        v.PixelFormat = buf[offset + 0];
        v.bt2 = buf[offset + 1];
        v.bt3 = buf[offset + 2];
        // Delphi Boolean：$00 = False，其余（惯例 $01）为 True。
        v.boAlpha = buf[offset + 3] != 0;
        v.nWidth = BitConverter.ToInt16(buf, offset + 4);
        v.nHeight = BitConverter.ToInt16(buf, offset + 6);
        v.px = BitConverter.ToInt16(buf, offset + 8);
        v.py = BitConverter.ToInt16(buf, offset + 10);
        v.Length = BitConverter.ToInt32(buf, offset + 12);
        return v;
    }

    public readonly void ToBytes(byte[] buf, int offset)
    {
        buf[offset + 0] = PixelFormat;
        buf[offset + 1] = bt2;
        buf[offset + 2] = bt3;
        buf[offset + 3] = boAlpha ? (byte)1 : (byte)0;
        BitConverter.GetBytes(nWidth).CopyTo(buf, offset + 4);
        BitConverter.GetBytes(nHeight).CopyTo(buf, offset + 6);
        BitConverter.GetBytes(px).CopyTo(buf, offset + 8);
        BitConverter.GetBytes(py).CopyTo(buf, offset + 10);
        BitConverter.GetBytes(Length).CopyTo(buf, offset + 12);
    }
}

/// <summary>
/// Pak.pas 63-65：<c>TFileHeaderInfo = packed record FileType:string[9]; end;</c>
/// <para>SizeOf = 1 + 9 = <b>10</b>。原文以 <c>CompareLStr(HeaderInfo.FileType, 'GEEM2', Length('GEEM2'))</c> 比较前 5 字节，
/// 但 <c>SizeOf(TFileHeaderInfo)</c> 走满 10 字节 —— 这就是磁盘上 GEEM2/GEEPAK2/GEEPAK3 头的真实宽度。</para>
/// </summary>
public struct TFileHeaderInfo
{
    /// <summary>偏移 0：长度字节 + 9 字节 GBK 数据。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] FileType;

    public const int SizeOf = 10;

    /// <summary>原文 <c>HeaderInfo.FileType := 'GEEM2'</c>（ShortString 赋值，截断到 9 字节）。</summary>
    public string FileTypeText
    {
        readonly get => ShortStringLayout.GetString(FileType, 0, 9);
        set => ShortStringLayout.SetBytes(FileType, 0, 9, value);
    }

    public static TFileHeaderInfo CreateEmpty() => new() { FileType = new byte[10] };

    public static TFileHeaderInfo FromBytes(byte[] buf, int offset)
    {
        var h = CreateEmpty();
        Buffer.BlockCopy(buf, offset, h.FileType, 0, 10);
        return h;
    }

    public readonly void ToBytes(byte[] buf, int offset) => Buffer.BlockCopy(FileType, 0, buf, offset, 10);
}

/// <summary>
/// Pak.pas 67-69：<c>TLzFileHeaderInfo = packed record FileType : array[0..5-1] of AnsiChar; end;</c>
/// <para>SizeOf = <b>5</b>（**不是** ShortString，无长度字节；原文 603-607 逐字节赋 'H','X','M','2','.'）。</para>
/// </summary>
public struct TLzFileHeaderInfo
{
    /// <summary>5 字节 AnsiChar（LZ PAK 的魔数 'HXM2.'）。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] FileType;

    public const int SizeOf = 5;

    /// <summary>按 GBK 解出的魔数文本（原文 836/970 行 <c>string(LzHeaderInfo.FileType)</c>）。</summary>
    public readonly string FileTypeText => GXX.Core.EncodingInit.GBK.GetString(FileType, 0, 5);

    public static TLzFileHeaderInfo CreateEmpty() => new() { FileType = new byte[5] };

    public static TLzFileHeaderInfo FromBytes(byte[] buf, int offset)
    {
        var h = CreateEmpty();
        Buffer.BlockCopy(buf, offset, h.FileType, 0, 5);
        return h;
    }

    public readonly void ToBytes(byte[] buf, int offset) => Buffer.BlockCopy(FileType, 0, buf, offset, 5);

    /// <summary>原文 603-607 的 'HXM2.' 逐字节填充。</summary>
    public static TLzFileHeaderInfo Hzm2Dot() => new() { FileType = new byte[] { (byte)'H', (byte)'X', (byte)'M', (byte)'2', (byte)'.' } };
}

/// <summary>
/// Pak.pas 71-90：<c>TPakFileHeader = packed record</c>（原文注释「新定义的Pak文件头」）。
/// <para>SizeOf 推导（逐字段无填充）：
/// bt1 1 + Title(ShortString 40) 41 + Size 4 + ImageCount 4 + bfType/bfReserved1/2/3 4
/// + IndexOffSet 4 + BitCount 2 + CreateDate(TDateTime=Double) 8 + CheckCode(ShortString 12) 13
/// + bfReserveds[0..2] 3 + Reserve[0..5] of Integer 24 + KeyData[0..31] of DWord 128
/// + Chain[0..19] 20 = <b>256</b>。</para>
/// <para><c>IndexOffSet</c> 与 <c>Size</c> 在原文 588-589 都写成
/// <c>SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader) = 10 + 256 = 266</c>（普通 PAK）；
/// LZ PAK 走 585-586 的 <c>SizeOf(TLzFileHeaderInfo) + SizeOf(TPakFileHeader) + 1 = 262</c>。</para>
/// </summary>
public struct TPakFileHeader
{
    public byte bt1;                 // 偏移 0

    /// <summary>偏移 1：<c>Title:string[40];</c>（41 字节）</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
    public byte[] Title;

    public uint Size;                // 偏移 42
    public uint ImageCount;          // 偏移 46
    public byte bfType;              // 偏移 50
    public byte bfReserved1;         // 偏移 51
    public byte bfReserved2;         // 偏移 52
    public byte bfReserved3;         // 偏移 53
    public uint IndexOffSet;         // 偏移 54
    public ushort BitCount;          // 偏移 58

    /// <summary>偏移 60：<c>CreateDate:TDateTime;</c>（Double，OLE 自动化日期）</summary>
    public double CreateDate;

    /// <summary>偏移 68：<c>CheckCode:string[12];</c>（13 字节）</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
    public byte[] CheckCode;

    /// <summary>偏移 81：<c>bfReserveds:array[0..2] of Byte;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] bfReserveds;

    /// <summary>偏移 84：<c>Reserve:array[0..5] of Integer;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public int[] Reserve;

    /// <summary>偏移 108：<c>KeyData:array[0..31] of DWord;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public uint[] KeyData;

    /// <summary>偏移 236：<c>Chain:array[0..20 - 1] of Byte;</c></summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] Chain;

    /// <summary>Delphi <c>SizeOf(TPakFileHeader)</c> = 256。</summary>
    public const int SizeOf = 256;

    /// <summary>Delphi <c>SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader)</c> = 266（原文 588-589）。</summary>
    public const int PlainIndexOffset = TFileHeaderInfo.SizeOf + SizeOf; // 266

    public string TitleText
    {
        readonly get => ShortStringLayout.GetString(Title, 0, 40);
        set => ShortStringLayout.SetBytes(Title, 0, 40, value);
    }

    public string CheckCodeText
    {
        readonly get => ShortStringLayout.GetString(CheckCode, 0, 12);
        set => ShortStringLayout.SetBytes(CheckCode, 0, 12, value);
    }

    /// <summary>
    /// Delphi 默认（零初始化）构造，对应原文 <c>FillChar(FileHeader, SizeOf(TPakFileHeader), 0)</c>
    /// 与 <c>FillChar(FileHeader, SizeOf(TPakFileHeader), #0)</c>。
    /// </summary>
    public static TPakFileHeader CreateEmpty() => new()
    {
        Title = new byte[41],
        CheckCode = new byte[13],
        bfReserveds = new byte[3],
        Reserve = new int[6],
        KeyData = new uint[32],
        Chain = new byte[20],
    };

    /// <summary>按 packed 布局（小端）反序列化 —— 与 <see cref="ToBytes"/> 对拍。</summary>
    public static TPakFileHeader FromBytes(byte[] buf, int offset)
    {
        var h = CreateEmpty();
        h.bt1 = buf[offset + 0];
        Buffer.BlockCopy(buf, offset + 1, h.Title, 0, 41);
        h.Size = ReadU32(buf, offset + 42);
        h.ImageCount = ReadU32(buf, offset + 46);
        h.bfType = buf[offset + 50];
        h.bfReserved1 = buf[offset + 51];
        h.bfReserved2 = buf[offset + 52];
        h.bfReserved3 = buf[offset + 53];
        h.IndexOffSet = ReadU32(buf, offset + 54);
        h.BitCount = (ushort)(buf[offset + 58] | (buf[offset + 59] << 8));
        h.CreateDate = BitConverter.ToDouble(buf, offset + 60);
        Buffer.BlockCopy(buf, offset + 68, h.CheckCode, 0, 13);
        Buffer.BlockCopy(buf, offset + 81, h.bfReserveds, 0, 3);
        for (int i = 0; i < 6; i++) h.Reserve[i] = BitConverter.ToInt32(buf, offset + 84 + i * 4);
        for (int i = 0; i < 32; i++) h.KeyData[i] = ReadU32(buf, offset + 108 + i * 4);
        Buffer.BlockCopy(buf, offset + 236, h.Chain, 0, 20);
        return h;
    }

    /// <summary>按 packed 布局（小端）序列化。</summary>
    public readonly void ToBytes(byte[] buf, int offset)
    {
        buf[offset + 0] = bt1;
        Buffer.BlockCopy(Title, 0, buf, offset + 1, 41);
        WriteU32(buf, offset + 42, Size);
        WriteU32(buf, offset + 46, ImageCount);
        buf[offset + 50] = bfType;
        buf[offset + 51] = bfReserved1;
        buf[offset + 52] = bfReserved2;
        buf[offset + 53] = bfReserved3;
        WriteU32(buf, offset + 54, IndexOffSet);
        buf[offset + 58] = (byte)BitCount;
        buf[offset + 59] = (byte)(BitCount >> 8);
        BitConverter.GetBytes(CreateDate).CopyTo(buf, offset + 60);
        Buffer.BlockCopy(CheckCode, 0, buf, offset + 68, 13);
        Buffer.BlockCopy(bfReserveds, 0, buf, offset + 81, 3);
        for (int i = 0; i < 6; i++) BitConverter.GetBytes(Reserve[i]).CopyTo(buf, offset + 84 + i * 4);
        for (int i = 0; i < 32; i++) WriteU32(buf, offset + 108 + i * 4, KeyData[i]);
        Buffer.BlockCopy(Chain, 0, buf, offset + 236, 20);
    }

    private static uint ReadU32(byte[] b, int o)
        => (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));

    private static void WriteU32(byte[] b, int o, uint v)
    {
        b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
    }
}

/// <summary>
/// Pak.pas 93-100：<c>TPakKey = packed record ImageCount:Integer; PakType:Word;
/// KeyData:array[0..31] of LongWord; Chain:array[0..20 - 1] of Byte; Reserve:array[0..5] of Integer; end;</c>
/// <para>SizeOf = 4 + 2 + 128 + 20 + 24 = <b>178</b>。原文注释（96）：
/// 「PakTypeWord 的高位记录 LzPakV0 的 BitCount, 其他类型保持不变」——
/// 见原文 509-510：<c>nLzV0BitCount := (PakType shr 8) and $FF; wPakType := PakType and $00FF;</c>。</para>
/// </summary>
public struct TPakKey
{
    public int ImageCount;
    public ushort PakType;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public uint[] KeyData;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] Chain;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public int[] Reserve;

    public const int SizeOf = 178;

    public static TPakKey CreateEmpty() => new()
    {
        KeyData = new uint[32],
        Chain = new byte[20],
        Reserve = new int[6],
    };

    public static TPakKey FromBytes(byte[] buf, int offset)
    {
        var k = CreateEmpty();
        k.ImageCount = BitConverter.ToInt32(buf, offset);
        k.PakType = (ushort)(buf[offset + 4] | (buf[offset + 5] << 8));
        for (int i = 0; i < 32; i++)
            k.KeyData[i] = (uint)(buf[offset + 6 + i * 4] | (buf[offset + 7 + i * 4] << 8)
                | (buf[offset + 8 + i * 4] << 16) | (buf[offset + 9 + i * 4] << 24));
        Buffer.BlockCopy(buf, offset + 134, k.Chain, 0, 20);
        for (int i = 0; i < 6; i++) k.Reserve[i] = BitConverter.ToInt32(buf, offset + 154 + i * 4);
        return k;
    }

    public readonly void ToBytes(byte[] buf, int offset)
    {
        BitConverter.GetBytes(ImageCount).CopyTo(buf, offset);
        buf[offset + 4] = (byte)PakType;
        buf[offset + 5] = (byte)(PakType >> 8);
        for (int i = 0; i < 32; i++)
        {
            uint v = KeyData[i];
            buf[offset + 6 + i * 4] = (byte)v;
            buf[offset + 7 + i * 4] = (byte)(v >> 8);
            buf[offset + 8 + i * 4] = (byte)(v >> 16);
            buf[offset + 9 + i * 4] = (byte)(v >> 24);
        }
        Buffer.BlockCopy(Chain, 0, buf, offset + 134, 20);
        for (int i = 0; i < 6; i++) BitConverter.GetBytes(Reserve[i]).CopyTo(buf, offset + 154 + i * 4);
    }

    /// <summary>原文 509：<c>nLzV0BitCount := (PakType shr 8) and $FF;</c></summary>
    public readonly int LzV0BitCount => (PakType >> 8) & 0xFF;

    /// <summary>原文 510：<c>wPakType := (PakType and $00FF);</c></summary>
    public readonly int PakTypeLow => PakType & 0x00FF;
}

/// <summary>
/// Pak.pas 102-106：<c>TLzPakIndexVer0 = packed record nImageOffSet:Integer; nDataSize:Integer; end;</c>
/// <para>SizeOf = <b>8</b>。LzPakV0 的索引项（每项 8 字节，普通 PAK 每项 4 字节）。</para>
/// </summary>
public struct TLzPakIndexVer0
{
    public int nImageOffSet;
    public int nDataSize;

    public const int SizeOf = 8;

    public static TLzPakIndexVer0 FromBytes(byte[] buf, int offset) => new()
    {
        nImageOffSet = BitConverter.ToInt32(buf, offset),
        nDataSize = BitConverter.ToInt32(buf, offset + 4),
    };

    public readonly void ToBytes(byte[] buf, int offset)
    {
        BitConverter.GetBytes(nImageOffSet).CopyTo(buf, offset);
        BitConverter.GetBytes(nDataSize).CopyTo(buf, offset + 4);
    }
}

/// <summary>
/// Delphi 的 <c>CompareLStr(S1, S2, Len)</c>（SysUtils.AnsiCompareStr 前缀版）：
/// 只比较前 <paramref name="len"/> 个字节，**大小写敏感**，长度不足时按「短者小」。
/// <para>Pak.pas 825/828/831/836/957/961/965/970 用它识别文件头魔数
/// （'GEEM2' / 'GEEPAK2' / 'GEEPAK3' / 'HXM2'）。</para>
/// <para><b>原文易错点</b>：<c>HeaderInfo.FileType</c> 是 ShortString(9)，
/// 而 <c>CompareLStr(HeaderInfo.FileType, 'GEEM2', 5)</c> 只比 5 字节 ——
/// 所以 'GEEM2' 之后的第 6..9 字节（未初始化栈残留）**不参与**判定；
/// 但 'GEEPAK2' 比 6 字节、'GEEPAK3' 比 6 字节。三者长度不同，顺序必须是
/// GEEM2 → GEEPAK2 → GEEPAK3（原文 825-833 即此顺序，前 5 字节的 'GEEPA' 不会误判 GEEM2）。</para>
/// </summary>
public static class PakStringCompare
{
    /// <summary>按 GBK 字节做 <paramref name="len"/> 字节前缀比较（大小写敏感）。</summary>
    public static bool CompareLStr(string s1, string s2, int len)
    {
        byte[] b1 = GXX.Core.EncodingInit.GBK.GetBytes(s1 ?? "");
        byte[] b2 = GXX.Core.EncodingInit.GBK.GetBytes(s2 ?? "");
        for (int i = 0; i < len; i++)
        {
            byte c1 = i < b1.Length ? b1[i] : (byte)0;
            byte c2 = i < b2.Length ? b2[i] : (byte)0;
            if (c1 != c2) return false;
        }
        return true;
    }

    /// <summary>ShortString(N) 版本：直接读长度字节 + 数据（用于 TFileHeaderInfo.FileType）。</summary>
    public static bool CompareLStr(byte[] shortString, string s2, int len)
    {
        byte[] b2 = GXX.Core.EncodingInit.GBK.GetBytes(s2 ?? "");
        for (int i = 0; i < len; i++)
        {
            // Delphi ShortString 的字符串数据从下标 1 开始 → 字节缓冲偏移 +1。
            byte c1 = (i + 1) < shortString.Length ? shortString[i + 1] : (byte)0;
            byte c2 = i < b2.Length ? b2[i] : (byte)0;
            if (c1 != c2) return false;
        }
        return true;
    }
}

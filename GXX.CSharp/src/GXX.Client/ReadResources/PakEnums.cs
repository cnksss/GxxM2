using System;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Pak.pas 1:1 移植 —— 枚举层（原文 interface 段 108-111 行 + implemented uses 段常量 235-242 行）。
//
// 源单元：Source\Client-HGE\ReadResources\Pak.pas（3199 行）
// 命名空间与本目录既有产物（Wil.cs / Wzl.cs / GameImagesBase.cs）一致。
//
// 原文条件编译常量（GameImages.pas 12-20 + Pak.pas 230-242）：
//   LOADIMAGEMODE = 0；USEMAPSTREAM = 0；CLIENTEXE = 1；PAK3_ENCODE = 1；PRIVATE_CLIENT = 0
//   故 Pak.pas 中所有 {$IF CLIENTEXE = 1} 分支**成立**、{$IF PRIVATE_CLIENT = 0} 分支**成立**。
// 本移植按成立分支逐行对应。
// =====================================================================================

/// <summary>
/// Pak.pas 108：<c>TDxTextureStyle = (dtsNormal, dtsGray, dtsBright);</c>
/// <para>Delphi 无显式序号 → 0/1/2（SizeOf = 1）。</para>
/// </summary>
public enum TDxTextureStyle : byte
{
    dtsNormal = 0,
    dtsGray = 1,
    dtsBright = 2,
}

/// <summary>
/// Pak.pas 110-111：<c>TPakFileType = (pftPak1, pftPak2, pftPak3, pftLzPakV0, pftLzPakV1, pftLzPakV0orV1);</c>
/// <para>原文注释：HZQ 20230711 Gee(string[9]) 和 Gom(string[10]) 记录了类型，龙族的 PAK 需要解密后才能知道类型。</para>
/// </summary>
public enum TPakFileType : byte
{
    pftPak1 = 0,
    pftPak2 = 1,
    pftPak3 = 2,
    pftLzPakV0 = 3,
    pftLzPakV1 = 4,
    pftLzPakV0orV1 = 5,
}

/// <summary>
/// Pak.pas 236-241（implementation 段 const + 条件常量）。
/// </summary>
public static class PakConsts
{
    /// <summary>Pak.pas 236：<c>PAK3_ENCODE = 1; // PAK3加密</c></summary>
    public const int PAK3_ENCODE = 1;

    /// <summary>
    /// Pak.pas 237：<c>LZ_PAK_FILE_HEADER_SIZE = 262;</c>
    /// <para>= SizeOf(TLzFileHeaderInfo) 5 + SizeOf(TPakFileHeader) 256 + 1 填充字节（见原文 585-586）。</para>
    /// </summary>
    public const int LZ_PAK_FILE_HEADER_SIZE = 262;

    /// <summary>Pak.pas 240：<c>g_UpdateRetryTime = 3;</c>（仅 CLIENTEXE &lt;&gt; 1 时定义；移植保留常量语义）。</summary>
    public const int g_UpdateRetryTime = 3;

    /// <summary>Pak.pas 241：<c>g_boAutoUpdate = False;</c>（仅 CLIENTEXE &lt;&gt; 1 时定义）。</summary>
    public const bool g_boAutoUpdate = false;

    /// <summary>
    /// Pak.pas 246：<c>BytesPerPixels:array[TPixelFormat] of Byte = (1, 1, 1, 1, 2, 2, 3, 4, 4);</c>
    /// <para>按 TPixelFormat 序号排列：pfDevice..pf32bit。注意 pfDevice/pf1bit/pf4bit 全为 1（原文如此）。</para>
    /// </summary>
    public static readonly byte[] BytesPerPixels = { 1, 1, 1, 1, 2, 2, 3, 4, 4 };

    /// <summary>Pak.pas 246 的查表（越界返回 0，对应 Delphi 变体数组越界为未定义行为；此处取 0）。</summary>
    public static byte BytesPerPixel(TPixelFormat pf)
    {
        int i = (int)pf;
        return i >= 0 && i < BytesPerPixels.Length ? BytesPerPixels[i] : (byte)0;
    }

    /// <summary>
    /// GameImages.pas 315-318：<c>WidthBytes(BitCount:Byte; Width:Integer):Integer;
    /// Result := (((Width * BitCount) + 31) div 32) * 4;</c>
    /// <para>Pak.pas 2273/2356-2359/2381/2390/2423/2696-2700 均调用该函数。</para>
    /// </summary>
    public static int WidthBytes(int bitCount, int width)
        => (((width * bitCount) + 31) / 32) * 4;

    /// <summary>Pak.pas 2382-2384 的向上取整行字节：<c>nAlphaLineSize := nImgWidth div 2; if (nImgWidth and $01) &lt;&gt; 0 then Inc(..)</c>。</summary>
    public static int AlphaLineSize(int imgWidth)
    {
        int size = imgWidth / 2;
        if ((imgWidth & 0x01) != 0) size++;
        return size;
    }

    /// <summary>Pak.pas 2390：<c>nAlphaWidthBytes := (((nImgWidth * 8) + 31) div 32) * 4;</c></summary>
    public static int AlphaWidthBytes(int imgWidth) => (((imgWidth * 8) + 31) / 32) * 4;
}

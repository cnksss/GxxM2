using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

/// <summary>
/// DxCanvas.pas 全局颜色查找表（44-64 的 unit var）。
///
/// 原文这些表**声明**在 DxCanvas 的 interface 段、但**初始化**写在 GameImages.pas 的
/// initialization（GameImages.pas 1826-1830：Move(ColorArray, g_DefColorTable) / InitGrays() /
/// BuildColorLevels()）。即 GameImages 与 DxCanvas 靠 unit 级全局变量耦合。
/// 本批只移植 DxCanvas，故这里保留同名同型的静态表 + 显式装载接缝，
/// **不**顺手把 GameImages 的初始化搬过来（那是另一车道的单元）。
/// </summary>
public static class TDxCanvasColorTables
{
    /// <summary>
    /// DxCanvas.pas 45 —— g_DefColorTable:TRGBQuads（256 项，每项 4 字节 B,G,R,Reserved）。
    /// 内容由 GameImages.pas 的初始化（ColorArray）填充。
    /// 接缝：待 GameImages 移植后接入。
    /// </summary>
    public static readonly byte[,] g_DefColorTable = new byte[256, 4];

    /// <summary>
    /// DxCanvas.pas 47 —— g_Grays:array[0..767] of Integer。
    /// GameImages.pas 1703-1716 InitGrays：对 I in 0..255 连续写三次 g_Grays[X]:=I（X 递增）。
    /// 该表**只依赖自身**，故这里 1:1 直接初始化（不视为接缝）。
    /// </summary>
    public static readonly int[] g_Grays = BuildGrays();

    private static int[] BuildGrays()
    {
        var a = new int[768];
        int x = 0;
        for (int i = 0; i <= 255; i++)
        {
            a[x] = i; x++;
            a[x] = i; x++;
            a[x] = i; x++;
        }
        return a;
    }

    /// <summary>DxCanvas.pas 49 —— ColorTable_565:array[0..255] of Word。原文中**从未被赋值或读取**（悬空声明）。</summary>
    public static readonly ushort[] ColorTable_565 = new ushort[256];

    /// <summary>DxCanvas.pas 51 —— ColorTable_8_16Bit（8bit 调色板 → 565）。</summary>
    public static readonly ushort[] ColorTable_8_16Bit = new ushort[256];

    /// <summary>DxCanvas.pas 52 —— ColorTableBright_8_16Bit。</summary>
    public static readonly ushort[] ColorTableBright_8_16Bit = new ushort[256];

    /// <summary>DxCanvas.pas 53 —— ColorTableGray_8_16Bit。</summary>
    public static readonly ushort[] ColorTableGray_8_16Bit = new ushort[256];

    /// <summary>DxCanvas.pas 55 —— ColorTable_8_32Bit。</summary>
    public static readonly uint[] ColorTable_8_32Bit = new uint[256];

    /// <summary>DxCanvas.pas 56 —— ColorTableBright_8_32Bit。</summary>
    public static readonly uint[] ColorTableBright_8_32Bit = new uint[256];

    /// <summary>DxCanvas.pas 57 —— ColorTableGray_8_32Bit。</summary>
    public static readonly uint[] ColorTableGray_8_32Bit = new uint[256];

    /// <summary>DxCanvas.pas 59 —— ColorTable_16_32Bit（565 → 32bit）。</summary>
    public static readonly uint[] ColorTable_16_32Bit = new uint[65536];

    /// <summary>DxCanvas.pas 60 —— ColorTableBright_16_32Bit。</summary>
    public static readonly uint[] ColorTableBright_16_32Bit = new uint[65536];

    /// <summary>DxCanvas.pas 61 —— ColorTableGray_16_32Bit。</summary>
    public static readonly uint[] ColorTableGray_16_32Bit = new uint[65536];

    /// <summary>DxCanvas.pas 63 —— ColorTableBright_16（16bit → 16bit 亮化）。</summary>
    public static readonly ushort[] ColorTableBright_16 = new ushort[65536];

    /// <summary>DxCanvas.pas 64 —— ColorTableGray_16（16bit → 16bit 灰度）。</summary>
    public static readonly ushort[] ColorTableGray_16 = new ushort[65536];

    /// <summary>
    /// 接缝：把 256 项 Delphi TRGBQuads 调色板灌进 g_DefColorTable
    /// （原文由 GameImages.pas 1828 的 Move(ColorArray, g_DefColorTable, SizeOf) 完成）。
    /// </summary>
    public static void LoadDefColorTable(byte[] rgbQuads)
    {
        if (rgbQuads == null) return;
        int n = Math.Min(rgbQuads.Length, 256 * 4);
        for (int i = 0; i < n; i++)
            g_DefColorTable[i / 4, i % 4] = rgbQuads[i];
    }

    /// <summary>清空所有表（测试隔离用）。</summary>
    public static void Clear()
    {
        Array.Clear(g_DefColorTable, 0, g_DefColorTable.Length);
        Array.Clear(ColorTable_8_16Bit, 0, ColorTable_8_16Bit.Length);
        Array.Clear(ColorTableBright_8_16Bit, 0, ColorTableBright_8_16Bit.Length);
        Array.Clear(ColorTableGray_8_16Bit, 0, ColorTableGray_8_16Bit.Length);
        Array.Clear(ColorTable_8_32Bit, 0, ColorTable_8_32Bit.Length);
        Array.Clear(ColorTableBright_8_32Bit, 0, ColorTableBright_8_32Bit.Length);
        Array.Clear(ColorTableGray_8_32Bit, 0, ColorTableGray_8_32Bit.Length);
    }

    /// <summary>TRGBQuad 字段偏移 1:1（Delphi Windows.TRGBQuad: rgbBlue, rgbGreen, rgbRed, rgbReserved）。</summary>
    public const int QuadSize = 4;
    public const int BlueOfs = 0;
    public const int GreenOfs = 1;
    public const int RedOfs = 2;
    public const int ReservedOfs = 3;

    /// <summary>TRGBTriple 字段偏移 1:1（rgbtBlue, rgbtGreen, rgbtRed）。</summary>
    public const int TripleSize = 3;
    public const int TripleBlueOfs = 0;
    public const int TripleGreenOfs = 1;
    public const int TripleRedOfs = 2;

    /// <summary>Delphi Round：银行家舍入（与 GXX.Client.Scenes.TActorCore.DelphiRound 同语义）。</summary>
    public static int DelphiRound(double value) => (int)Math.Round(value, MidpointRounding.ToEven);
}

/// <summary>
/// 一张 32bpp 目标纹理的内存镜像（对应原文 Result.Lock(Bits, Pitch) 拿到的可写缓冲）。
/// 行距 Pitch 与原文一致：由调用方指定，允许 &gt; Width*4。
/// </summary>
public sealed class TDxTextureBuffer : IDxTexture
{
    public int Width { get; }
    public int Height { get; }
    public int Pitch { get; }
    public byte[] Bits { get; }

    public TDxTextureBuffer(int width, int height, int pitch = 0)
    {
        Width = width;
        Height = height;
        Pitch = pitch > 0 ? pitch : width * 4;
        Bits = new byte[Math.Max(Pitch * height, 0)];
    }

    public TDxRect ClientRect => TDxRect.Rect(0, 0, Width, Height);

    /// <summary>原文 PCardinal(Bits + Y*Pitch + X*4)。</summary>
    public uint GetPixel(int x, int y)
    {
        int o = y * Pitch + x * 4;
        return (uint)(Bits[o] | (Bits[o + 1] << 8) | (Bits[o + 2] << 16) | (Bits[o + 3] << 24));
    }

    public void SetPixel(int x, int y, uint value)
    {
        int o = y * Pitch + x * 4;
        Bits[o] = (byte)(value & 0xFF);
        Bits[o + 1] = (byte)((value >> 8) & 0xFF);
        Bits[o + 2] = (byte)((value >> 16) & 0xFF);
        Bits[o + 3] = (byte)((value >> 24) & 0xFF);
    }
}

/// <summary>
/// GetTextTexture 的排版产物（DxCanvas.pas 1151-1246 的几何部分）。
/// 与 GXX.Client.Scenes.TextTextureLayout（批次J77 的 headless 镜像）同语义，
/// 此处按 DxCanvas 原位置重列，两者由测试交叉锁定（DxCanvasTests.与Scenes镜像一致）。
/// </summary>
public sealed class TDxTextTextureLayout
{
    /// <summary>纹理宽（各行绘制宽度最大值）。</summary>
    public int Width;

    /// <summary>纹理高（行高 × 行数）。</summary>
    public int Height;

    /// <summary>单行行高（原文取 '|' 的 GetTextExtentPoint32 高度）。</summary>
    public int LineHeight;

    /// <summary>行数（SL.Count）。</summary>
    public int LineCount;

    /// <summary>每行在 Rect(0, I*nLH, nW, I*nLH+nLH) 内按 DT_CENTER 居中后的 x 起点。</summary>
    public readonly List<int> CenteredX = new();

    /// <summary>每行绘制 y 起点（I * nLH）。</summary>
    public readonly List<int> LineY = new();
}

/// <summary>
/// DxCanvas.pas 1:1 逐字移植（1-1246）。
///
/// 已 1:1 落地的部分：
///   * CheckTextureAlpha（72-81）
///   * ConvertLine16 / ConvertBrightLine16 / ConvertGrayLine16（136-268）
///   * ConvertLine32（272-411）/ ConvertLine32_8/_16/_24/_32（413-551）
///   * ConvertBrightLine32 / ConvertGrayLine32（553-843）
///   * NewTextureGray / NewTextureBright / NewTexture / NewTextureFromGraphic 的行调度（845-1018）
///   * CopyTexture 的坐标裁剪与逐像素拷贝（1068-1149）
///   * GetTextTexture 的度量与排版（1151-1246 的几何部分）
///
/// 接缝（未在本车道实现真实绘制）：
///   * GameCanvas.HGE.Texture_Create / Texture_Load / Texture_LoadDDS / Lock / Unlock → IDxTextureFactory
///   * TDIB（DIB.pas，251KB，未移植）→ IDxDib 最小面
///   * g_DefColorTable 等查找表的装载来自 GameImages（见 TDxCanvasColorTables 注释）
///
/// 原文缺陷已照抄并标注：见 CopyTexture 的 `srcLeft`（大小写不匹配）与
/// `srcPitch * I`（应为 i）——均为原文如此（DxCanvas.pas:1119-1121/1129-1131）。
/// </summary>
public static class DxCanvas
{
    /// <summary>DxCanvas.pas 128 MaxPixelCount。</summary>
    public const int MaxPixelCount = 32768;

    // =========================================================================
    // 接缝
    // =========================================================================

    /// <summary>
    /// 接缝：GameCanvas.HGE 的纹理面（Texture_Create / Texture_Load / Texture_LoadDDS）。
    /// 待 HGE 层（HGE.pas / HGECanvas.pas）移植后接入真实实现。
    /// </summary>
    public static IDxTextureFactory TextureFactory;

    /// <summary>接缝：TDIB（DIB.pas）最小面。</summary>
    public interface IDxDib
    {
        int Width { get; }
        int Height { get; }
        byte BitCount { get; }
        /// <summary>DIB.ColorTable:TRGBQuads（256×4 字节）。</summary>
        byte[] ColorTable { get; }
        /// <summary>DIB.ScanLine[Y] —— 返回自该行首字节起的可读缓冲。</summary>
        byte[] GetScanLine(int y);
    }

    /// <summary>
    /// 接缝：纹理工厂。Lock 出来的缓冲由 TDxTextureBuffer 表达；
    /// 原文 `Result.Lock(Bits, Pitch, False) and (Bits &lt;&gt; nil) and (Pitch &gt; 0)` 的三重守卫
    /// 在这里由 Create 返回 null 或 Pitch &lt;= 0 表达。
    /// </summary>
    public interface IDxTextureFactory
    {
        /// <summary>GameCanvas.HGE.Texture_Create(Width, Height)。</summary>
        TDxTextureBuffer Create(int width, int height);

        /// <summary>GameCanvas.HGE.Texture_Load(FileData, FileSize, False)。</summary>
        TDxTextureBuffer Load(byte[] fileData);

        /// <summary>GameCanvas.HGE.Texture_LoadDDS(Fmt, FileData, FileSize, False)。</summary>
        TDxTextureBuffer LoadDds(int d3dFmt, byte[] fileData);
    }

    /// <summary>D3DFMT_DXT1/DXT2/DXT3/DXT4/DXT5（原文 1026-1034 的尝试顺序）。</summary>
    public const int D3DFMT_DXT1 = 0x31545844;
    public const int D3DFMT_DXT2 = 0x32545844;
    public const int D3DFMT_DXT3 = 0x33545844;
    public const int D3DFMT_DXT4 = 0x34545844;
    public const int D3DFMT_DXT5 = 0x35545844;

    /// <summary>DxCanvas.pas 1026-1034 的 DDS 格式尝试顺序（原文顺序，不可改）。</summary>
    public static readonly int[] DdsFormatOrder =
    {
        D3DFMT_DXT1, D3DFMT_DXT3, D3DFMT_DXT5, D3DFMT_DXT2, D3DFMT_DXT4,
    };

    // =========================================================================
    // CheckTextureAlpha（72-81）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 72-81 1:1。原文：
    /// `Result := False; if Assigned(Source) then begin Pix := Source.Pixels[X, Y];
    ///  Result := {(Pix &lt;&gt; $FF000000) and}(Pix &lt;&gt; $00000000); end;`
    /// 即 **仅** 判「是否等于 0」，注释掉的那半个条件不参与运算（原文如此）。
    /// </summary>
    public static bool CheckTextureAlpha(TDxTextureBuffer source, int x, int y)
    {
        bool result = false;
        if (source != null)
        {
            uint pix = source.GetPixel(x, y);
            result = pix != 0x00000000;
        }
        return result;
    }

    // =========================================================================
    // NullTexture（83-86）
    // =========================================================================

    /// <summary>DxCanvas.pas 83-86 1:1：GameCanvas.HGE.Texture_Create(1, 1)。</summary>
    public static TDxTextureBuffer NullTexture() => TextureFactory?.Create(1, 1);

    // =========================================================================
    // ConvertLine16（136-173）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 136-173 1:1（16bpp 目标）。
    /// 8 → ColorTable_8_16Bit[SrcP^]；16 → Move(SrcP^, DesP^, Width*2)（整段字节搬运）；
    /// 24 与 32 两个分支原文整体注释掉 → **不做任何事**（目标缓冲保持原样）。
    /// </summary>
    public static void ConvertLine16(byte[] src, int srcOfs, byte[] des, int desOfs, int width, byte bitCount,
        byte[] colorTable)
    {
        switch (bitCount)
        {
            case 8:
                for (int x = 0; x < width; x++)
                {
                    WriteUInt16(des, desOfs + x * 2, TDxCanvasColorTables.ColorTable_8_16Bit[src[srcOfs]]);
                    srcOfs++;
                }
                break;
            case 16:
                Buffer.BlockCopy(src, srcOfs, des, desOfs, width * 2);
                break;
            case 24:
                // 原文如此（DxCanvas.pas 151-162）：整段为注释，无实际动作。
                break;
            case 32:
                // 原文如此（DxCanvas.pas 163-171）：整段为注释，无实际动作。
                break;
        }
    }

    /// <summary>
    /// DxCanvas.pas 175-219 1:1（原文此过程**未在 interface 段导出**，但被实现段使用；
    /// 8 与 16 两个分支有效，24/32 分支整段注释）。
    /// </summary>
    public static void ConvertBrightLine16(byte[] src, int srcOfs, byte[] des, int desOfs, int width, byte bitCount,
        byte[] colorTable)
    {
        switch (bitCount)
        {
            case 8:
                for (int x = 0; x < width; x++)
                {
                    WriteUInt16(des, desOfs + x * 2, TDxCanvasColorTables.ColorTableBright_8_16Bit[src[srcOfs]]);
                    srcOfs++;
                }
                break;
            case 16:
                for (int x = 0; x < width; x++)
                {
                    ushort v = TDxCanvasColorTables.ColorTableBright_16[ReadUInt16(src, srcOfs)];
                    WriteUInt16(des, desOfs + x * 2, v);
                    srcOfs += 2;
                }
                break;
            case 24:
                // 原文如此（DxCanvas.pas 194-205）：整段注释。
                break;
            case 32:
                // 原文如此（DxCanvas.pas 206-217）：整段注释。
                break;
        }
    }

    /// <summary>
    /// DxCanvas.pas 221-268 1:1（16bpp 目标）。
    /// 8/16 用灰度表；24/32 分支：源像素非黑时取 g_Grays[R+G+B] 作三通道，
    /// 再打包成 565（`(R shl 8 and $F800) or (G shl 3 and $07E0) or (B shr 3 and $001F)`）；
    /// **源像素为全黑时原文不写目标**（即保留目标原值，且 DesP 不前进 —— 原文如此）。
    /// </summary>
    public static void ConvertGrayLine16(byte[] src, int srcOfs, byte[] des, int desOfs, int width, byte bitCount,
        byte[] colorTable)
    {
        switch (bitCount)
        {
            case 8:
                for (int x = 0; x < width; x++)
                {
                    WriteUInt16(des, desOfs + x * 2, TDxCanvasColorTables.ColorTableGray_8_16Bit[src[srcOfs]]);
                    srcOfs++;
                }
                break;
            case 16:
                for (int x = 0; x < width; x++)
                {
                    ushort v = TDxCanvasColorTables.ColorTableGray_16[ReadUInt16(src, srcOfs)];
                    WriteUInt16(des, desOfs + x * 2, v);
                    srcOfs += 2;
                }
                break;
            case 24:
                {
                    int desP = desOfs;
                    for (int x = 0; x < width; x++)
                    {
                        int o = srcOfs + x * TDxCanvasColorTables.TripleSize;
                        byte r = src[o + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[o + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[o + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            byte gray = (byte)TDxCanvasColorTables.g_Grays[r + g + b];
                            WriteUInt16(des, desP, Pack565(gray, gray, gray));
                            desP += 2;
                        }
                    }
                    break;
                }
            case 32:
                {
                    int desP = desOfs;
                    for (int x = 0; x < width; x++)
                    {
                        int o = srcOfs + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[o + TDxCanvasColorTables.RedOfs];
                        byte g = src[o + TDxCanvasColorTables.GreenOfs];
                        byte b = src[o + TDxCanvasColorTables.BlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            byte gray = (byte)TDxCanvasColorTables.g_Grays[r + g + b];
                            WriteUInt16(des, desP, Pack565(gray, gray, gray));
                            desP += 2;
                        }
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// DxCanvas.pas 250/262 的 565 打包式 1:1：
    /// `(R shl 8 and $F800) or (G shl 3 and $07E0) or (B shr 3 and $001F)`（Byte 运算后按 Word 截断）。
    /// </summary>
    public static ushort Pack565(byte red, byte green, byte blue)
    {
        int v = ((red << 8) & 0xF800) | ((green << 3) & 0x07E0) | ((blue >> 3) & 0x001F);
        return (ushort)(v & 0xFFFF);
    }

    // =========================================================================
    // ConvertLine32（272-411）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 272-411 1:1（32bpp 目标）。
    /// Alpha &lt;&gt; nil：写完颜色后**无条件**用 Alpha^ 覆盖高 8 位（每像素 Alpha 前进 1）；
    /// Alpha = nil：仅写颜色（原文把 alpha 段落注释掉了）。
    /// 各分支细节：
    ///   8  → ColorTable_8_32Bit[SrcP^]，SrcP 前进 1
    ///   16 → ColorTable_16_32Bit[PWord(SrcP)^]，SrcP 前进 2，DesP 前进 4
    ///   24 → 源像素非黑时写 r/g/b + rgbReserved=255（**黑色时完全不写**，但 Alpha 仍前进）
    ///   32 → 源非 0 时 `PCardinal(DesP)^ := SrcP^ or $FF000000`（源为 0 则目标保持原值）
    /// </summary>
    public static void ConvertLine32(byte[] src, int srcOfs, byte[] des, int desOfs, int width, byte bitCount,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        if (alpha != null)
        {
            int srcP = srcOfs;
            int desP = desOfs;
            int alphaP = alphaOfs;
            switch (bitCount)
            {
                case 8:
                    for (int x = 0; x < width; x++)
                    {
                        uint c = TDxCanvasColorTables.ColorTable_8_32Bit[src[srcP]];
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);
                        alphaP++;
                        srcP += 1;
                        desP += 4;
                    }
                    break;
                case 16:
                    for (int x = 0; x < width; x++)
                    {
                        uint c = TDxCanvasColorTables.ColorTable_16_32Bit[ReadUInt16(src, srcP)];
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);
                        alphaP++;
                        srcP += 2;
                        desP += 4;
                    }
                    break;
                case 24:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.TripleSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            des[dobj + TDxCanvasColorTables.RedOfs] = r;
                            des[dobj + TDxCanvasColorTables.GreenOfs] = g;
                            des[dobj + TDxCanvasColorTables.BlueOfs] = b;
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                        uint c = ReadUInt32(des, dobj);
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaOfs + x] << 24);
                        WriteUInt32(des, dobj, c);
                    }
                    break;
                case 32:
                    for (int x = 0; x < width; x++)
                    {
                        uint s = ReadUInt32(src, srcP);
                        if (s != 0)
                            WriteUInt32(des, desP, s | 0xFF000000u);

                        uint c = ReadUInt32(des, desP);
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);

                        alphaP++;
                        srcP += 4;
                        desP += 4;
                    }
                    break;
            }
        }
        else
        {
            int srcP = srcOfs;
            int desP = desOfs;
            switch (bitCount)
            {
                case 8:
                    for (int x = 0; x < width; x++)
                    {
                        WriteUInt32(des, desP, TDxCanvasColorTables.ColorTable_8_32Bit[src[srcP]]);
                        srcP += 1;
                        desP += 4;
                    }
                    break;
                case 16:
                    for (int x = 0; x < width; x++)
                    {
                        WriteUInt32(des, desP, TDxCanvasColorTables.ColorTable_16_32Bit[ReadUInt16(src, srcP)]);
                        srcP += 2;
                        desP += 4;
                    }
                    break;
                case 24:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.TripleSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            des[dobj + TDxCanvasColorTables.RedOfs] = r;
                            des[dobj + TDxCanvasColorTables.GreenOfs] = g;
                            des[dobj + TDxCanvasColorTables.BlueOfs] = b;
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                    }
                    break;
                case 32:
                    for (int x = 0; x < width; x++)
                    {
                        uint s = ReadUInt32(src, srcP);
                        if (s != 0)
                            WriteUInt32(des, desP, s | 0xFF000000u);
                        srcP += 4;
                        desP += 4;
                    }
                    break;
            }
        }
    }

    // =========================================================================
    // ConvertLine32_8 / _16 / _24 / _32（413-551）
    // =========================================================================

    /// <summary>DxCanvas.pas 413-445 1:1。Alpha≠nil 时无条件覆盖 alpha；Alpha=nil 时只写颜色。</summary>
    public static void ConvertLine32_8(byte[] src, int srcOfs, byte[] des, int desOfs, int width,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        int srcP = srcOfs;
        int desP = desOfs;
        if (alpha != null)
        {
            int alphaP = alphaOfs;
            for (int x = 0; x < width; x++)
            {
                uint c = TDxCanvasColorTables.ColorTable_8_32Bit[src[srcP]];
                c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                WriteUInt32(des, desP, c);
                alphaP++;
                srcP += 1;
                desP += 4;
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                WriteUInt32(des, desP, TDxCanvasColorTables.ColorTable_8_32Bit[src[srcP]]);
                srcP += 1;
                desP += 4;
            }
        }
    }

    /// <summary>
    /// DxCanvas.pas 447-469 1:1。注意原文用局部 P := Alpha 而不是参数 Alpha，
    /// 两分支判断等价（P := Alpha 后立即 `if P &lt;&gt; nil`）——照抄。
    /// </summary>
    public static void ConvertLine32_16(byte[] src, int srcOfs, byte[] des, int desOfs, int width,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        int p = alphaOfs;
        int srcP = srcOfs;
        int desP = desOfs;
        if (alpha != null)
        {
            for (int x = 0; x < width; x++)
            {
                uint c = TDxCanvasColorTables.ColorTable_16_32Bit[ReadUInt16(src, srcP)];
                c = (c & 0x00FFFFFFu) | ((uint)alpha[p] << 24);
                WriteUInt32(des, desP, c);
                p++;
                srcP += 2;
                desP += 4;
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                WriteUInt32(des, desP, TDxCanvasColorTables.ColorTable_16_32Bit[ReadUInt16(src, srcP)]);
                srcP += 2;
                desP += 4;
            }
        }
    }

    /// <summary>DxCanvas.pas 471-513 1:1。</summary>
    public static void ConvertLine32_24(byte[] src, int srcOfs, byte[] des, int desOfs, int width,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        if (alpha != null)
        {
            int alphaP = alphaOfs;
            for (int x = 0; x < width; x++)
            {
                int so = srcOfs + x * TDxCanvasColorTables.TripleSize;
                int dobj = desOfs + x * TDxCanvasColorTables.QuadSize;
                byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                if (r != 0 || g != 0 || b != 0)
                {
                    des[dobj + TDxCanvasColorTables.RedOfs] = r;
                    des[dobj + TDxCanvasColorTables.GreenOfs] = g;
                    des[dobj + TDxCanvasColorTables.BlueOfs] = b;
                    des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                }
                uint c = ReadUInt32(des, dobj);
                c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                WriteUInt32(des, dobj, c);
                alphaP++;
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                int so = srcOfs + x * TDxCanvasColorTables.TripleSize;
                int dobj = desOfs + x * TDxCanvasColorTables.QuadSize;
                byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                if (r != 0 || g != 0 || b != 0)
                {
                    des[dobj + TDxCanvasColorTables.RedOfs] = r;
                    des[dobj + TDxCanvasColorTables.GreenOfs] = g;
                    des[dobj + TDxCanvasColorTables.BlueOfs] = b;
                    des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                }
            }
        }
    }

    /// <summary>DxCanvas.pas 515-551 1:1。</summary>
    public static void ConvertLine32_32(byte[] src, int srcOfs, byte[] des, int desOfs, int width,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        int srcP = srcOfs;
        int desP = desOfs;
        if (alpha != null)
        {
            int alphaP = alphaOfs;
            for (int x = 0; x < width; x++)
            {
                uint s = ReadUInt32(src, srcP);
                if (s != 0)
                    WriteUInt32(des, desP, s | 0xFF000000u);

                uint c = ReadUInt32(des, desP);
                c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                WriteUInt32(des, desP, c);

                alphaP++;
                srcP += 4;
                desP += 4;
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                uint s = ReadUInt32(src, srcP);
                if (s != 0)
                    WriteUInt32(des, desP, s | 0xFF000000u);
                srcP += 4;
                desP += 4;
            }
        }
    }

    // =========================================================================
    // ConvertBrightLine32（553-697）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 553-697 1:1（32bpp 目标，亮化 1.3 倍）。
    ///   8  → ColorTableBright_8_32Bit[SrcP^]
    ///   16 → ColorTableBright_16_32Bit[PWord(SrcP)^]
    ///   24/32 → 源非黑时 `MIN(255, Round(通道 * 1.3))`，且 rgbReserved := 255
    /// （MIN 是 Delphi Math.MIN；Round 是银行家舍入）。Alpha≠nil 时随后无条件覆盖高 8 位。
    /// </summary>
    public static void ConvertBrightLine32(byte[] src, int srcOfs, byte[] des, int desOfs, int width, byte bitCount,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        if (alpha != null)
        {
            int srcP = srcOfs;
            int desP = desOfs;
            int alphaP = alphaOfs;
            switch (bitCount)
            {
                case 8:
                    for (int x = 0; x < width; x++)
                    {
                        uint c = TDxCanvasColorTables.ColorTableBright_8_32Bit[src[srcP]];
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);
                        alphaP++;
                        srcP += 1;
                        desP += 4;
                    }
                    break;
                case 16:
                    for (int x = 0; x < width; x++)
                    {
                        uint c = TDxCanvasColorTables.ColorTableBright_16_32Bit[ReadUInt16(src, srcP)];
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);
                        alphaP++;
                        srcP += 2;
                        desP += 4;
                    }
                    break;
                case 24:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.TripleSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            des[dobj + TDxCanvasColorTables.RedOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(r * 1.3));
                            des[dobj + TDxCanvasColorTables.GreenOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(g * 1.3));
                            des[dobj + TDxCanvasColorTables.BlueOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(b * 1.3));
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                        uint c = ReadUInt32(des, dobj);
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, dobj, c);
                        alphaP++;
                    }
                    break;
                case 32:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.QuadSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.RedOfs];
                        byte g = src[so + TDxCanvasColorTables.GreenOfs];
                        byte b = src[so + TDxCanvasColorTables.BlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            des[dobj + TDxCanvasColorTables.RedOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(r * 1.3));
                            des[dobj + TDxCanvasColorTables.GreenOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(g * 1.3));
                            des[dobj + TDxCanvasColorTables.BlueOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(b * 1.3));
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                        uint c = ReadUInt32(des, dobj);
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, dobj, c);
                        alphaP++;
                    }
                    break;
            }
        }
        else
        {
            int srcP = srcOfs;
            int desP = desOfs;
            switch (bitCount)
            {
                case 8:
                    for (int x = 0; x < width; x++)
                    {
                        WriteUInt32(des, desP, TDxCanvasColorTables.ColorTableBright_8_32Bit[src[srcP]]);
                        srcP += 1;
                        desP += 4;
                    }
                    break;
                case 16:
                    for (int x = 0; x < width; x++)
                    {
                        WriteUInt32(des, desP, TDxCanvasColorTables.ColorTableBright_16_32Bit[ReadUInt16(src, srcP)]);
                        srcP += 2;
                        desP += 4;
                    }
                    break;
                case 24:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.TripleSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            des[dobj + TDxCanvasColorTables.RedOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(r * 1.3));
                            des[dobj + TDxCanvasColorTables.GreenOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(g * 1.3));
                            des[dobj + TDxCanvasColorTables.BlueOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(b * 1.3));
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                    }
                    break;
                case 32:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.QuadSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.RedOfs];
                        byte g = src[so + TDxCanvasColorTables.GreenOfs];
                        byte b = src[so + TDxCanvasColorTables.BlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            des[dobj + TDxCanvasColorTables.RedOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(r * 1.3));
                            des[dobj + TDxCanvasColorTables.GreenOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(g * 1.3));
                            des[dobj + TDxCanvasColorTables.BlueOfs] = (byte)Math.Min(255, TDxCanvasColorTables.DelphiRound(b * 1.3));
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                    }
                    break;
            }
        }
    }

    // =========================================================================
    // ConvertGrayLine32（699-843）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 699-843 1:1（32bpp 目标，灰度化）。
    ///   8  → ColorTableGray_8_32Bit[SrcP^]
    ///   16 → ColorTableGray_16_32Bit[PWord(SrcP)^]
    ///   24/32 → 源非黑时 R=G=B=g_Grays[R+G+B]、rgbReserved := 255（源全黑时不写）
    /// Alpha≠nil 时随后无条件覆盖高 8 位。
    /// </summary>
    public static void ConvertGrayLine32(byte[] src, int srcOfs, byte[] des, int desOfs, int width, byte bitCount,
        byte[] colorTable, byte[] alpha, int alphaOfs)
    {
        if (alpha != null)
        {
            int srcP = srcOfs;
            int desP = desOfs;
            int alphaP = alphaOfs;
            switch (bitCount)
            {
                case 8:
                    for (int x = 0; x < width; x++)
                    {
                        uint c = TDxCanvasColorTables.ColorTableGray_8_32Bit[src[srcP]];
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);
                        alphaP++;
                        srcP += 1;
                        desP += 4;
                    }
                    break;
                case 16:
                    for (int x = 0; x < width; x++)
                    {
                        uint c = TDxCanvasColorTables.ColorTableGray_16_32Bit[ReadUInt16(src, srcP)];
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, desP, c);
                        alphaP++;
                        srcP += 2;
                        desP += 4;
                    }
                    break;
                case 24:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.TripleSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            byte gray = (byte)TDxCanvasColorTables.g_Grays[r + g + b];
                            des[dobj + TDxCanvasColorTables.RedOfs] = gray;
                            des[dobj + TDxCanvasColorTables.GreenOfs] = gray;
                            des[dobj + TDxCanvasColorTables.BlueOfs] = gray;
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                        uint c = ReadUInt32(des, dobj);
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, dobj, c);
                        alphaP++;
                    }
                    break;
                case 32:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.QuadSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.RedOfs];
                        byte g = src[so + TDxCanvasColorTables.GreenOfs];
                        byte b = src[so + TDxCanvasColorTables.BlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            byte gray = (byte)TDxCanvasColorTables.g_Grays[r + g + b];
                            des[dobj + TDxCanvasColorTables.RedOfs] = gray;
                            des[dobj + TDxCanvasColorTables.GreenOfs] = gray;
                            des[dobj + TDxCanvasColorTables.BlueOfs] = gray;
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                        uint c = ReadUInt32(des, dobj);
                        c = (c & 0x00FFFFFFu) | ((uint)alpha[alphaP] << 24);
                        WriteUInt32(des, dobj, c);
                        alphaP++;
                    }
                    break;
            }
        }
        else
        {
            int srcP = srcOfs;
            int desP = desOfs;
            switch (bitCount)
            {
                case 8:
                    for (int x = 0; x < width; x++)
                    {
                        WriteUInt32(des, desP, TDxCanvasColorTables.ColorTableGray_8_32Bit[src[srcP]]);
                        srcP += 1;
                        desP += 4;
                    }
                    break;
                case 16:
                    for (int x = 0; x < width; x++)
                    {
                        WriteUInt32(des, desP, TDxCanvasColorTables.ColorTableGray_16_32Bit[ReadUInt16(src, srcP)]);
                        srcP += 2;
                        desP += 4;
                    }
                    break;
                case 24:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.TripleSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.TripleRedOfs];
                        byte g = src[so + TDxCanvasColorTables.TripleGreenOfs];
                        byte b = src[so + TDxCanvasColorTables.TripleBlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            byte gray = (byte)TDxCanvasColorTables.g_Grays[r + g + b];
                            des[dobj + TDxCanvasColorTables.RedOfs] = gray;
                            des[dobj + TDxCanvasColorTables.GreenOfs] = gray;
                            des[dobj + TDxCanvasColorTables.BlueOfs] = gray;
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                    }
                    break;
                case 32:
                    for (int x = 0; x < width; x++)
                    {
                        int so = srcP + x * TDxCanvasColorTables.QuadSize;
                        int dobj = desP + x * TDxCanvasColorTables.QuadSize;
                        byte r = src[so + TDxCanvasColorTables.RedOfs];
                        byte g = src[so + TDxCanvasColorTables.GreenOfs];
                        byte b = src[so + TDxCanvasColorTables.BlueOfs];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            byte gray = (byte)TDxCanvasColorTables.g_Grays[r + g + b];
                            des[dobj + TDxCanvasColorTables.RedOfs] = gray;
                            des[dobj + TDxCanvasColorTables.GreenOfs] = gray;
                            des[dobj + TDxCanvasColorTables.BlueOfs] = gray;
                            des[dobj + TDxCanvasColorTables.ReservedOfs] = 255;
                        }
                    }
                    break;
            }
        }
    }

    // =========================================================================
    // NewTextureGray（845-879）/ NewTextureBright（881-914）/ NewTexture（916-1002）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 845-879 1:1：逐行取 Source.ScanLine[Y] 与 Alpha.ScanLine[Y]，
    /// 调 ConvertGrayLine32（含 Alpha 版本选择）。
    /// </summary>
    public static TDxTextureBuffer NewTextureGray(IDxDib source, IDxDib alpha = null)
    {
        var result = TextureFactory?.Create(source.Width, source.Height);
        if (result != null)
        {
            if (result.Pitch > 0)
            {
                if (alpha != null)
                {
                    for (int y = 0; y <= source.Height - 1; y++)
                    {
                        var srcP = source.GetScanLine(y);
                        var alphaP = alpha.GetScanLine(y);
                        ConvertGrayLine32(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                            source.BitCount, source.ColorTable, alphaP, 0);
                    }
                }
                else
                {
                    for (int y = 0; y <= source.Height - 1; y++)
                    {
                        var srcP = source.GetScanLine(y);
                        ConvertGrayLine32(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                            source.BitCount, source.ColorTable, null, 0);
                    }
                }
            }
        }
        return result;
    }

    /// <summary>DxCanvas.pas 881-914 1:1。</summary>
    public static TDxTextureBuffer NewTextureBright(IDxDib source, IDxDib alpha = null)
    {
        var result = TextureFactory?.Create(source.Width, source.Height);
        if (result != null)
        {
            if (result.Pitch > 0)
            {
                if (alpha != null)
                {
                    for (int y = 0; y <= source.Height - 1; y++)
                    {
                        var srcP = source.GetScanLine(y);
                        var alphaP = alpha.GetScanLine(y);
                        ConvertBrightLine32(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                            source.BitCount, source.ColorTable, alphaP, 0);
                    }
                }
                else
                {
                    for (int y = 0; y <= source.Height - 1; y++)
                    {
                        var srcP = source.GetScanLine(y);
                        ConvertBrightLine32(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                            source.BitCount, source.ColorTable, null, 0);
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// DxCanvas.pas 916-1002 1:1：按 Source.BitCount 选择 ConvertLine32_8/_16/_24/_32，
    /// 外层再按 Alpha 是否为 nil 分两套完全平行的循环（原文如此，未合并）。
    /// </summary>
    public static TDxTextureBuffer NewTexture(IDxDib source, IDxDib alpha = null)
    {
        var result = TextureFactory?.Create(source.Width, source.Height);
        if (result != null)
        {
            if (result.Pitch > 0)
            {
                if (alpha != null)
                {
                    switch (source.BitCount)
                    {
                        case 8:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                var alphaP = alpha.GetScanLine(y);
                                ConvertLine32_8(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, alphaP, 0);
                            }
                            break;
                        case 16:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                var alphaP = alpha.GetScanLine(y);
                                ConvertLine32_16(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, alphaP, 0);
                            }
                            break;
                        case 24:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                var alphaP = alpha.GetScanLine(y);
                                ConvertLine32_24(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, alphaP, 0);
                            }
                            break;
                        case 32:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                var alphaP = alpha.GetScanLine(y);
                                ConvertLine32_32(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, alphaP, 0);
                            }
                            break;
                    }
                }
                else
                {
                    switch (source.BitCount)
                    {
                        case 8:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                ConvertLine32_8(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, null, 0);
                            }
                            break;
                        case 16:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                ConvertLine32_16(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, null, 0);
                            }
                            break;
                        case 24:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                ConvertLine32_24(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, null, 0);
                            }
                            break;
                        case 32:
                            for (int y = 0; y <= source.Height - 1; y++)
                            {
                                var srcP = source.GetScanLine(y);
                                ConvertLine32_32(srcP, 0, result.Bits, y * result.Pitch, source.Width,
                                    source.ColorTable, null, 0);
                            }
                            break;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 接缝：TGraphic（VCL Graphics.TGraphic）→ 本移植以 IDxDib 表达（原文 TDIB.Assign(srcPic)）。
    /// 待 DIB.pas 移植后接入；srcPic 为 null 时返回 null（原文 1008-1009 的 Assigned 守卫）。
    /// </summary>
    public static TDxTextureBuffer NewTextureFromGraphic(IDxDib srcPic)
    {
        TDxTextureBuffer result = null;
        if (srcPic != null)
        {
            var objDib = srcPic;                 // 原文：objDib := TDIB.Create; objDib.Assign(srcPic);
            result = NewTexture(objDib);
            // 原文 finally FreeAndNil(objDib) —— 托管侧由调用方持有生命周期。
        }
        return result;
    }

    /// <summary>
    /// DxCanvas.pas 1020-1066 1:1。
    /// UseD3DFormat=True 时按 D3DFMT_DXT1 → DXT3 → DXT5 → DXT2 → DXT4 顺序尝试，
    /// 全部失败再回落到 Texture_Load（原文 1036-1037 **不在 if 内**，故 UseD3DFormat=False 也走 Load）。
    /// Alpha 参数在原文里整段被注释（1040-1065），故此处保留形参但不参与运算。
    /// </summary>
    public static TDxTextureBuffer NewTexture(byte[] fileData, int fileSize, int aWidth, int aHeight,
        uint transparentColor, bool useD3DFormat, IDxDib alpha = null)
    {
        TDxTextureBuffer texture = null;
        if (useD3DFormat)
        {
            foreach (var fmt in DdsFormatOrder)
            {
                texture = TextureFactory?.LoadDds(fmt, fileData);
                if (texture != null) break;
            }
        }
        if (texture == null)
            texture = TextureFactory?.Load(fileData);
        return texture;
    }

    // =========================================================================
    // CopyTexture（1068-1149）
    // =========================================================================

    /// <summary>CopyTexture 的裁剪结果（原文 1070-1111）。</summary>
    public struct CopyRegion
    {
        public int SrcLeft;
        public int SrcWidth;
        public int SrcTop;
        public int SrcBottom;
        public int X;
        public int Y;
        /// <summary>原文 1110-1111 的提前退出条件命中 → 整段不画。</summary>
        public bool Abort;
    }

    /// <summary>
    /// DxCanvas.pas 1068-1111 1:1 的坐标裁剪（不触碰像素，可独立单测）。
    /// 逐条顺序不可换：
    ///   1) Source=nil → exit；Target=nil → exit
    ///   2) x &gt;= Target.Width → exit；y &gt;= Target.Height → exit
    ///   3) x &lt; 0 → srcleft=-x, srcwidth=Source.Width+x, x=0；否则 srcleft=0, srcwidth=Source.Width
    ///   4) y &lt; 0 → srctop=-y, srcbottom=srctop+Source.Height+y, y=0；否则 srctop=0, srcbottom=Source.Height
    ///   5) `if (srcleft + srcwidth) &gt; Source.Width`（恒不成立，因 srcwidth = Source.Width+x 或 Source.Width）
    ///   6) srcbottom &gt; Source.Height → 截断
    ///   7) (x + srcwidth) &gt; Target.Width → srcwidth 截断
    ///   8) (y + srcbottom - srctop) &gt; Target.Height → srcbottom 截断
    ///   9) Abort = (srcwidth &lt;= 0) or (srcbottom &lt;= 0) or (srcleft &gt;= Source.Width) or (srctop &gt;= Source.Height)
    /// </summary>
    public static CopyRegion ComputeCopyRegion(int sourceWidth, int sourceHeight,
        int targetWidth, int targetHeight, int x, int y)
    {
        var r = new CopyRegion { X = x, Y = y };
        if (x >= targetWidth) { r.Abort = true; return r; }
        if (y >= targetHeight) { r.Abort = true; return r; }

        if (x < 0)
        {
            r.SrcLeft = -x;
            r.SrcWidth = sourceWidth + x;
            r.X = 0;
        }
        else
        {
            r.SrcLeft = 0;
            r.SrcWidth = sourceWidth;
        }

        if (y < 0)
        {
            r.SrcTop = -y;
            r.SrcBottom = r.SrcTop + sourceHeight + y;
            r.Y = 0;
        }
        else
        {
            r.SrcTop = 0;
            r.SrcBottom = r.SrcTop + sourceHeight;
        }

        if (r.SrcLeft + r.SrcWidth > sourceWidth)
            r.SrcWidth = sourceWidth - r.SrcLeft;
        if (r.SrcBottom > sourceHeight)
            r.SrcBottom = sourceHeight;
        if (r.X + r.SrcWidth > targetWidth)
            r.SrcWidth = targetWidth - r.X;
        if (r.Y + r.SrcBottom - r.SrcTop > targetHeight)
            r.SrcBottom = targetHeight - r.Y + r.SrcTop;

        if (r.SrcWidth <= 0 || r.SrcBottom <= 0 || r.SrcLeft >= sourceWidth || r.SrcTop >= sourceHeight)
            r.Abort = true;

        return r;
    }

    /// <summary>
    /// DxCanvas.pas 1068-1149 1:1（逐像素搬运）。
    ///
    /// **原文缺陷照抄并标注**：内层两个循环用的是 `srcLeft`（小写 l，原文并未声明该名字，
    /// 实际是 `srcleft` 的笔误；Delphi 大小写不敏感，故编译通过、语义等于 srcleft），
    /// 以及 `srcPitch * I`（大写 I —— 是外层的行号变量，**不是**内层的 i；原文如此）。
    /// 本移植用 `I`（外层行号）与 `j`（内层列号）如实表达，见 DxCanvas.pas:1119-1121 / 1129-1131。
    ///
    /// Blend=False：`if srcPoint^ &lt;&gt; 0 then targetPoint^ := srcPoint^`
    /// Blend=True ：同上，另按 `nGray := (30*B + 59*G + 11*R) div 100` 写 rgbReserved。
    /// </summary>
    public static void CopyTexture(TDxTextureBuffer source, TDxTextureBuffer target, int x, int y, bool blend = false)
    {
        if (source == null) return;
        if (target == null) return;

        var r = ComputeCopyRegion(source.Width, source.Height, target.Width, target.Height, x, y);
        if (r.Abort) return;

        // 原文 Source.Lock(srcBits, srcPitch, True) / Target.Lock(destBits, destPitch)
        int srcPitch = source.Pitch;
        int destPitch = target.Pitch;

        if (!blend)
        {
            for (int i = r.SrcTop; i <= r.SrcBottom - 1; i++)
            {
                for (int j = r.SrcLeft; j <= r.SrcWidth - 1; j++)
                {
                    int srcOfs = srcPitch * i + j * 4;                                  // 原文 srcPitch * I
                    int dstOfs = destPitch * (r.Y + i - r.SrcTop) + (r.X + j - r.SrcLeft) * 4;
                    uint s = ReadUInt32(source.Bits, srcOfs);
                    if (s != 0)
                        WriteUInt32(target.Bits, dstOfs, s);
                }
            }
        }
        else
        {
            for (int i = r.SrcTop; i <= r.SrcBottom - 1; i++)
            {
                for (int j = r.SrcLeft; j <= r.SrcWidth - 1; j++)
                {
                    int srcOfs = srcPitch * i + j * 4;                                  // 原文 srcPitch * I
                    int dstOfs = destPitch * (r.Y + i - r.SrcTop) + (r.X + j - r.SrcLeft) * 4;
                    uint s = ReadUInt32(source.Bits, srcOfs);
                    if (s != 0)
                    {
                        WriteUInt32(target.Bits, dstOfs, s);
                        // RGBQuad := PRGBQuad(targetPoint); nGray := (30*B + 59*G + 11*R) div 100
                        byte b = target.Bits[dstOfs + TDxCanvasColorTables.BlueOfs];
                        byte g = target.Bits[dstOfs + TDxCanvasColorTables.GreenOfs];
                        byte red = target.Bits[dstOfs + TDxCanvasColorTables.RedOfs];
                        int nGray = (30 * b + 59 * g + 11 * red) / 100;
                        target.Bits[dstOfs + TDxCanvasColorTables.ReservedOfs] = (byte)nGray;
                    }
                }
            }
        }
    }

    // =========================================================================
    // GetTextTexture（1151-1246）
    // =========================================================================

    /// <summary>
    /// DxCanvas.pas 1151-1246 的**几何部分** 1:1（不创建纹理、不逐像素搬运）。
    ///   1) nW := 0；逐行 GetTextExtentPoint32 → 取最大值
    ///   2) 以 '|' 度量得 nLH（单行行高）
    ///   3) `nW := nW;`（原文的自赋值，无副作用，仅留痕）
    ///   4) nH := nLH * SL.Count
    ///   5) 每行 DrawText 于 Rect(0, I*nLH, nW, I*nLH+nLH)，标志 DT_CENTER or DT_SINGLELINE
    /// 注意：**SL.Count = 0 时 nW=0、nH=0**（原文仍会 Texture_Create(0,0)）。
    /// </summary>
    public static TDxTextTextureLayout MeasureTextTexture(TDxFontEnv env, object hgeFont, IReadOnlyList<string> sl)
    {
        var layout = new TDxTextTextureLayout();
        if (sl == null) return layout;

        int nW = 0;
        for (int i = 0; i <= sl.Count - 1; i++)
        {
            string s = sl[i];
            int cx = env != null && env.TextWidth != null ? env.TextWidth(hgeFont, s) : TDxFontEnv.MeasureTextWidth(s);
            if (cx > nW) nW = cx;
        }

        int nLH = env != null && env.TextHeight != null
            ? env.TextHeight(hgeFont, "|")
            : TDxFontEnv.MeasureTextHeight("|");

        // 原文 1186：nW := nW;（自赋值，无副作用）
        int nH = nLH * sl.Count;

        layout.Width = nW;
        layout.LineHeight = nLH;
        layout.Height = nH;
        layout.LineCount = sl.Count;

        for (int i = 0; i <= sl.Count - 1; i++)
        {
            string s = sl[i];
            int cx = env != null && env.TextWidth != null ? env.TextWidth(hgeFont, s) : TDxFontEnv.MeasureTextWidth(s);
            layout.LineY.Add(i * nLH);
            // DT_CENTER 于 Rect(0, I*nLH, nW, I*nLH+nLH)：x = (nW - 本行宽) / 2
            layout.CenteredX.Add((nW - cx) / 2);
        }

        return layout;
    }

    /// <summary>
    /// DxCanvas.pas 1151-1246：度量 + 创建纹理 + 逐像素搬运（`if SrcP^ &lt;&gt; 0 then or $FF000000 else $00000000`）。
    /// 纹理创建与搬运走 TextureFactory 接缝；无工厂时返回 null（不抛异常）。
    /// </summary>
    public static TDxTextureBuffer GetTextTexture(TDxFontEnv env, object hgeFont, IReadOnlyList<string> sl)
    {
        var layout = MeasureTextTexture(env, hgeFont, sl);
        var result = TextureFactory?.Create(layout.Width, layout.Height);
        if (result == null || result.Pitch <= 0) return result;

        // 原文 1218-1237：Result.Lock(R, Bits, Pitch, False) 后逐像素搬运。
        // DIB 位图内容由 GDI 文本绘制产生（接缝），此处只表达搬运规则：
        // SrcP^ <> 0 → DesP^ := SrcP^ or $FF000000；否则 DesP^ := $00000000。
        for (int y = 0; y <= layout.Height - 1; y++)
        {
            for (int x = 0; x <= layout.Width - 1; x++)
            {
                int so = y * layout.Width * 4 + x * 4;
                int dobj = y * result.Pitch + x * 4;
                uint srcPix = so + 4 <= _gdiTextBits.Length ? ReadUInt32(_gdiTextBits, so) : 0u;
                WriteUInt32(result.Bits, dobj, srcPix != 0 ? (srcPix | 0xFF000000u) : 0x00000000u);
            }
        }
        return result;
    }

    /// <summary>
    /// 接缝：GetTextTexture 里 CreateDIBSection 出来的 32bpp 位图内容
    /// （DxCanvas.pas 1202-1214 由 HHDC + DrawText DT_CENTER 生成）。
    /// 待 HGE/GDI 文本层移植后由真实光栅化填充；缺省全 0。
    /// </summary>
    public static byte[] _gdiTextBits = Array.Empty<byte>();

    // =========================================================================
    // 字节读写辅助（原文 PCardinal/PWord 解引用）
    // =========================================================================

    internal static uint ReadUInt32(byte[] buf, int ofs)
        => (uint)(buf[ofs] | (buf[ofs + 1] << 8) | (buf[ofs + 2] << 16) | (buf[ofs + 3] << 24));

    internal static void WriteUInt32(byte[] buf, int ofs, uint value)
    {
        buf[ofs] = (byte)(value & 0xFF);
        buf[ofs + 1] = (byte)((value >> 8) & 0xFF);
        buf[ofs + 2] = (byte)((value >> 16) & 0xFF);
        buf[ofs + 3] = (byte)((value >> 24) & 0xFF);
    }

    internal static ushort ReadUInt16(byte[] buf, int ofs)
        => (ushort)(buf[ofs] | (buf[ofs + 1] << 8));

    internal static void WriteUInt16(byte[] buf, int ofs, ushort value)
    {
        buf[ofs] = (byte)(value & 0xFF);
        buf[ofs + 1] = (byte)((value >> 8) & 0xFF);
    }
}

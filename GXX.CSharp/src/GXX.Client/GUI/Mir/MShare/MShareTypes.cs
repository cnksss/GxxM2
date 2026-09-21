using System.Runtime.InteropServices;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【P17 切片1】MShare.pas 侧用到的 Windows GDI 基础结构（原文经 `Windows` 单元引用）。
// 字段名保留 Windows SDK 原名，使 `g_DefColorTable[c].rgbRed` 这类原文表达式可逐字对照。
// 计数对账（本文件）：真实体 1 / NotPorted 0 / 原文如此 0 = 1。
// ============================================================================================

/// <summary>
/// Windows `wingdi.h` 的 `RGBTRIPLE`（3 字节，**无对齐填充**）。
/// HGE.pas `T256ColorTable` / `g_DefColorTable:array[0..255] of TRGBQuad` 的元素类型；
/// MShare.pas:4113 只读它的 `rgbRed` / `rgbGreen` / `rgbBlue`（与 `RGBTRIPLE` 的字段拼写一致，
/// 原文 typedef `TRGBQuad = RGBTRIPLE`）。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TRGBQuad
{
    /// <summary>Windows RGBTRIPLE.rgbtRed（别名 rgbRed）。</summary>
    public byte rgbRed;

    /// <summary>Windows RGBTRIPLE.rgbtGreen（别名 rgbGreen）。</summary>
    public byte rgbGreen;

    /// <summary>Windows RGBTRIPLE.rgbtBlue（别名 rgbBlue）。</summary>
    public byte rgbBlue;
}

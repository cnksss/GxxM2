using System;

namespace GXX.Core.Compress;

/// <summary>
/// CompressUnit.pas 1:1 转换：按 CompType 分派的图像缓冲压缩/解压。
///
/// 覆盖审计（证据见 docs/并行报告-p2c-common-crypto.md）：
///  - CompType = 1 → RLEUnit.EncodeRLE/DecodeRLE。既有 Compress/ZlibEx.cs 里只有一个简化版
///    <c>DecodeRLE(byte[], int, int)</c>，**与原文 RLEUnit 的 (InData, OutData, Width, Height,
///    BytesPerPixel) 签名和语义都不同**（原文按 32/24/16/8 位色分别走不同分支）→
///    本文件按原文签名与 CompType 分派重建，并保留原文的"未实现分支"行为；
///  - CompType = 2 → CompressBufZ/DecompressBufZ（raw deflate，既有 ZlibEx 已实现，转调）；
///  - 其它 CompType → 原文直接 `Move(InData^, OutData^, W*H*BPP)`（原样拷贝）。
///
/// 原文照抄点（CompressUnit.pas:10-31）：
///  1. CompressBuffer 的返回值：默认 `Width*Height*BytesPerPixel`，仅 CompType=1/2 时被覆盖；
///  2. DecompressBuffer 无返回值；CompType=1 时忽略 OutBytes；
///  3. **原文 RLEUnit 的 EncodeRLE/DecodeRLE 本任务未移植**（RLEUnit.pas 6,648 字节，
///     属 Client-HGE 资源解码族）——此处按原文的调用签名保留接缝并抛 NotSupportedException，
///     以免"看起来能用但结果是错的"。CompressUnit.pas 在源码树中**零调用方**
///     （唯一提及是 Pak.pas:224 被注释掉的 `//CompressUnit,`），故该接缝不影响任何现有路径。
/// </summary>
public static class CompressUnit
{
    /// <summary>CompType = 1（RLE）。</summary>
    public const int CT_RLE = 1;

    /// <summary>CompType = 2（raw deflate）。</summary>
    public const int CT_DEFLATE = 2;

    /// <summary>
    /// 对应原文 CompressBuffer(CompType, InData, OutData, Width, Height, BytesPerPixel): Integer。
    /// 返回写入 OutData 的字节数。
    /// </summary>
    public static int CompressBuffer(int compType, byte[] inData, byte[] outData, int width, int height, int bytesPerPixel)
    {
        int rawSize = width * height * bytesPerPixel;
        switch (compType)
        {
            case CT_RLE:
                // 原文：Result := EncodeRLE(InData, OutData, Width, Height, BytesPerPixel);
                throw new NotSupportedException(
                    "CompressUnit.pas CompType=1 走 RLEUnit.EncodeRLE；RLEUnit.pas 本波次未移植（接缝）");
            case CT_DEFLATE:
                {
                    // 原文：CompressBufZ(InData, Width*Height*BytesPerPixel, OutData, Result)
                    // 原文：CompressBufZ(InData, Width*Height*BytesPerPixel, OutData, Result);
                    // 原文 OutBytes 由 CompressBufZ 带回"压缩后长度"；C# 无出参指针，
                    // 调用方需按返回的 z.Length 自行截断（见便捷重载的实现说明）。
                    byte[] z = ZlibEx.CompressBufZ(inData, rawSize);
                    if (z == null || z.Length == 0)
                        return 0;
                    Array.Copy(z, outData, Math.Min(z.Length, outData.Length));
                    return z.Length;
                }
            default:
                // 原文：Move(InData^, OutData^, Width*Height*BytesPerPixel)
                Array.Copy(inData, outData, Math.Min(rawSize, Math.Min(inData.Length, outData.Length)));
                return rawSize;
        }
    }

    /// <summary>
    /// 对应原文 DecompressBuffer(CompType, InData, OutData, Width, Height, BytesPerPixel)（无返回值）。
    /// </summary>
    public static void DecompressBuffer(int compType, byte[] inData, byte[] outData, int width, int height, int bytesPerPixel)
    {
        int rawSize = width * height * bytesPerPixel;
        switch (compType)
        {
            case CT_RLE:
                // 原文：DecodeRLE(InData, OutData, Width, Height, BytesPerPixel);
                throw new NotSupportedException(
                    "CompressUnit.pas CompType=1 走 RLEUnit.DecodeRLE；RLEUnit.pas 本波次未移植（接缝）");
            case CT_DEFLATE:
                {
                    // 原文：DecompressBufZ(InData, Width*Height*BytesPerPixel, 0, OutData, OutBytes);
                    // 注意：原文 InData 是裸指针、**没有输入长度**（由 deflate 流自身终止），
                    // 故这里必须传整个 inData.Length，而不是 rawSize —— 否则输入会被提前截断
                    // （.NET 的 raw-deflate 流对整数序列可能带 sync-flush，比原始数据更长）。
                    byte[] plain = ZlibEx.DecompressBufZ(inData, inData.Length);
                    if (plain != null && plain.Length > 0)
                        Array.Copy(plain, outData, Math.Min(plain.Length, outData.Length));
                    return;
                }
            default:
                Array.Copy(inData, outData, Math.Min(rawSize, Math.Min(inData.Length, outData.Length)));
                return;
        }
    }

    /// <summary>
    /// 便捷重载（**仅适用于 CompType = 2 raw deflate**）。
    /// 原文的 OutBytes 是**出参**、返回值对 CompType=2 而言是"原始数据长度"，
    /// 压缩后长度由 CompressBufZ 的 OutBytes 带出；因此便捷版必须直接返回
    /// CompressBufZ 的结果，而不能用"返回值 == 原始长度"去截断（会切掉压缩流末尾）。
    /// </summary>
    public static byte[] CompressBuffer(int compType, byte[] inData, int width, int height, int bytesPerPixel)
    {
        if (compType != CT_DEFLATE)
            throw new ArgumentException("便捷重载只支持 CompType=2；CompType=" + compType + " 需由调用方提供 OutBuf");

        int rawSize = width * height * bytesPerPixel;
        byte[] z = ZlibEx.CompressBufZ(inData, rawSize);
        return z ?? Array.Empty<byte>();
    }

    /// <summary>便捷重载：解压并返回输出缓冲。</summary>
    public static byte[] DecompressBuffer(int compType, byte[] inData, int width, int height, int bytesPerPixel)
    {
        var outData = new byte[width * height * bytesPerPixel];
        DecompressBuffer(compType, inData, outData, width, height, bytesPerPixel);
        return outData;
    }
}

using System;
using System.IO;

namespace GXX.Core.Compress;

/// <summary>
/// ZipUnit.pas 1:1 转换：ZlibEx 的四个薄包装（缓冲区/流版压缩解压）。
///
/// 覆盖审计：既有 <see cref="ZlibEx"/>（Compress/ZlibEx.cs）已实现
/// CompressBuf/DecompressBuf/CompressBufZ/DecompressBufZ（zlib 标准流），
/// 本文件只是把 ZipUnit.pas 的 4 个函数逐一对应到它上面 —— 纯转调，不重复算法。
///
/// 原文（ZipUnit.pas:42-52）：
///   ZipCompressBuffer  → ZCompress(InBuf, InBytes, OutBuf, OutBytes, Level)
///   ZipDecompressBuffer→ ZDecompress(InBuf, InBytes, OutBuf, OutBytes, 0)
/// 其中 TCompressionLevel（clNone/clFastest/clDefault/clMax）在 C# 侧映射到
/// CompressionLevel；原 TZCompressionLevel（zcNone/zcFastest/zcDefault/zcMax）同理。
/// </summary>
public static class ZipUnit
{
    /// <summary>原文 ZLibEx.TCompressionLevel（ZipUnit.pas:7 的 Level 参数）。</summary>
    public enum TCompressionLevel
    {
        clNone = 0,
        clFastest = 1,
        clDefault = 2,
        clMax = 3,
    }

    /// <summary>原文 ZLibEx.TZCompressionLevel（ZipUnit.pas:12 的 Level 参数）。</summary>
    public enum TZCompressionLevel
    {
        zcNone = 0,
        zcFastest = 1,
        zcDefault = 2,
        zcMax = 3,
    }

    /// <summary>
    /// 对应原文 ZipCompressBuffer（out OutBuf/OutBytes 的指针语义 →
    /// C# 返回压缩结果字节数组，null 表示失败）。
    /// </summary>
    public static byte[] ZipCompressBuffer(byte[] inBuf, int inBytes, TCompressionLevel level = TCompressionLevel.clDefault)
    {
        // ZlibEx.CompressBuf 走 zlib 默认级别；Level 参数在 .NET ZLibStream 下只有
        // Optimal/Fastest/NoCompression 三档，此处按原文四档做等价映射。
        return level switch
        {
            TCompressionLevel.clNone => CompressWith(inBuf, inBytes, System.IO.Compression.CompressionLevel.NoCompression),
            TCompressionLevel.clFastest => CompressWith(inBuf, inBytes, System.IO.Compression.CompressionLevel.Fastest),
            TCompressionLevel.clMax => CompressWith(inBuf, inBytes, System.IO.Compression.CompressionLevel.Optimal),
            _ => ZlibEx.CompressBuf(inBuf, inBytes),
        };
    }

    /// <summary>对应原文 ZipDecompressBuffer。</summary>
    public static byte[] ZipDecompressBuffer(byte[] inBuf, int inBytes)
        => ZlibEx.DecompressBuf(inBuf, inBytes);

    /// <summary>对应原文 ZipCompressStream（OnProgress 在 C# 侧无对应，忽略）。</summary>
    public static void ZipCompressStream(Stream inStream, Stream outStream, TZCompressionLevel level = TZCompressionLevel.zcDefault)
    {
        var cl = level switch
        {
            TZCompressionLevel.zcNone => System.IO.Compression.CompressionLevel.NoCompression,
            TZCompressionLevel.zcFastest => System.IO.Compression.CompressionLevel.Fastest,
            _ => System.IO.Compression.CompressionLevel.Optimal,
        };
        using var zs = new System.IO.Compression.ZLibStream(outStream, cl, leaveOpen: true);
        inStream.CopyTo(zs);
    }

    /// <summary>对应原文 ZipDecompressStream（OutStream.CopyFrom(zStream, 0) 语义）。</summary>
    public static void ZipDecompressStream(Stream inStream, Stream outStream)
    {
        using var zs = new System.IO.Compression.ZLibStream(inStream, System.IO.Compression.CompressionMode.Decompress);
        zs.CopyTo(outStream);
    }

    private static byte[] CompressWith(byte[] inBuf, int inBytes, System.IO.Compression.CompressionLevel level)
    {
        try
        {
            using var ms = new MemoryStream();
            using (var zs = new System.IO.Compression.ZLibStream(ms, level, leaveOpen: true))
                zs.Write(inBuf, 0, inBytes);
            return ms.ToArray();
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }

    /// <summary>把 ZipCompressBuffer 的 out 参数语义补齐（对应原文指针出参）。</summary>
    public static void ZipCompressBuffer(byte[] inBuf, int inBytes, out byte[] outBuf, out int outBytes, TCompressionLevel level = TCompressionLevel.clDefault)
    {
        outBuf = ZipCompressBuffer(inBuf, inBytes, level);
        outBytes = outBuf?.Length ?? 0;
    }

    /// <summary>把 ZipDecompressBuffer 的 out 参数语义补齐（对应原文指针出参）。</summary>
    public static void ZipDecompressBuffer(byte[] inBuf, int inBytes, out byte[] outBuf, out int outBytes)
    {
        outBuf = ZipDecompressBuffer(inBuf, inBytes);
        outBytes = outBuf?.Length ?? 0;
    }
}

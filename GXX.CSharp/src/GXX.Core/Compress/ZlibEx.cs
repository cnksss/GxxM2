using System;
using System.IO;
using System.IO.Compression;

namespace GXX.Core.Compress;

/// <summary>
/// ZLibEx.pas（base2 ZLibEx, Delphi）1:1 语义转换。
/// 原库 CompressBuf/DecompressBuf 使用 zlib 标准 deflate 流（deflateInit_，windowBits=15），
/// 与 .NET System.IO.Compression.ZLibStream（RFC1950 zlib 流）字节兼容。
/// </summary>
public static class ZlibEx
{
    /// <summary>对应 CompressBuf：失败返回 null（原实现异常时 zLibBufSize=0）。</summary>
    public static byte[] CompressBuf(byte[] inBuf, int inBytes)
    {
        try
        {
            using var msOut = new MemoryStream();
            using (var zs = new ZLibStream(msOut, CompressionLevel.Optimal, leaveOpen: true))
            {
                zs.Write(inBuf, 0, inBytes);
            }
            return msOut.ToArray();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>对应 DecompressBuf：OutEstimate 仅作容量提示；失败返回 null。</summary>
    public static byte[] DecompressBuf(byte[] inBuf, int inBytes, int outEstimate = 0)
    {
        try
        {
            using var msIn = new MemoryStream(inBuf, 0, inBytes, writable: false);
            using var zs = new ZLibStream(msIn, CompressionMode.Decompress);
            using var msOut = outEstimate > 0 ? new MemoryStream(outEstimate) : new MemoryStream();
            zs.CopyTo(msOut);
            return msOut.ToArray();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>对应 CompressBufZ（原始 deflate，无 zlib 头）——个别单元使用。</summary>
    public static byte[] CompressBufZ(byte[] inBuf, int inBytes)
    {
        try
        {
            using var msOut = new MemoryStream();
            using (var ds = new DeflateStream(msOut, CompressionLevel.Optimal, leaveOpen: true))
            {
                ds.Write(inBuf, 0, inBytes);
            }
            return msOut.ToArray();
        }
        catch
        {
            return null;
        }
    }

    public static byte[] DecompressBufZ(byte[] inBuf, int inBytes, int outEstimate = 0)
    {
        try
        {
            using var msIn = new MemoryStream(inBuf, 0, inBytes, writable: false);
            using var ds = new DeflateStream(msIn, CompressionMode.Decompress);
            using var msOut = new MemoryStream();
            ds.CopyTo(msOut);
            return msOut.ToArray();
        }
        catch
        {
            return null;
        }
    }

    // ---- RLE（RLEUnit.pas：客户端 WIL 图片解码使用）----

    /// <summary>RLE 解码（RLEUnit.pas DecodeRLE 语义）。</summary>
    public static byte[] DecodeRLE(byte[] src, int srcLen, int destSize)
    {
        var dest = new byte[destSize];
        int sp = 0, dp = 0;
        while (sp < srcLen && dp < destSize)
        {
            byte b = src[sp++];
            if ((b & 0xC0) == 0xC0)   // 0xC0 前缀 = 重复计数
            {
                int count = b & 0x3F;
                byte v = src[sp++];
                for (int i = 0; i < count && dp < destSize; i++) dest[dp++] = v;
            }
            else
            {
                dest[dp++] = b;
            }
        }
        return dest;
    }
}

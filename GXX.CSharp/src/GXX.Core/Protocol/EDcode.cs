using System;
using GXX.Core.Compress;
using GXX.Core.Rtl;

namespace GXX.Core.Protocol;

/// <summary>
/// EDcode.pas 1:1 转换：6-Bit 编解码（N/C/S 三套变体）+ 默认消息/字符串/缓冲编码 + zLib 压缩系列。
/// EDCODEBEIJING = 0：Encode/Decode6BitBuf 默认走 _N 路径。
/// </summary>
public static unsafe partial class EDcode
{
    public const int EDCODEBEIJING = 0;

    public static int GetEncodeSize(int inSize) => (inSize * 4 + 2) / 3;

    public static int GetDecodeSize(int inSize) => (int)(inSize * 3 / 4);

    // =========================================================================
    // _N 变体（标准 6-Bit）
    // =========================================================================

    public static int Encode6BitBuf_N(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
    {
        int resultLen = GetEncodeSize(srcLen);
        if (resultLen > destLen)
            srcLen = GetDecodeSize(destLen);

        byte surplusValue = 0;
        int surplusCount = 0;
        int pbSrc = srcOffset;
        int pbDest = destOffset;

        for (int i = 0; i < srcLen; i++)
        {
            byte c = src[pbSrc];
            byte currentBit6Value = (byte)((surplusValue | (c >> (2 + surplusCount))) & 0x3F);
            surplusValue = (byte)(((c << (8 - (2 + surplusCount))) >> 2) & 0x3F);
            surplusCount += 2;

            dest[pbDest] = (byte)(currentBit6Value + 0x3C);
            pbDest++;

            if (surplusCount == 6)
            {
                dest[pbDest] = (byte)(surplusValue + 0x3C);
                pbDest++;
                surplusValue = 0;
                surplusCount = 0;
            }
            pbSrc++;
        }

        if (surplusCount > 0)
            dest[pbDest] = (byte)(surplusValue + 0x3C);

        return GetEncodeSize(srcLen);
    }

    public static int Decode6BitBuf_N(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
    {
        int result = 0;
        int resultLen = GetDecodeSize(srcLen);
        if (resultLen > destLen)
            srcLen = GetEncodeSize(destLen);

        byte surplusValue = 0;
        int surplusCount = 0;
        int pbSrc = srcOffset;
        int pbDest = destOffset;

        for (int i = 0; i < srcLen; i++)
        {
            byte b = src[pbSrc];
            if (b < 0x3C || b >= 0x3C + 64)
            {
                Array.Clear(dest, destOffset, destLen);
                return 0;
            }

            byte bSrc = (byte)(b - 0x3C);
            if (surplusCount > 0)
            {
                byte currentBit8Value = (byte)(surplusValue | (bSrc >> (surplusCount - 2)));
                dest[pbDest] = currentBit8Value;
                pbDest++;
                surplusValue = (byte)(bSrc << (8 - (surplusCount - 2)));
                surplusCount -= 2;
            }
            else
            {
                surplusCount = 6;
                surplusValue = (byte)((bSrc << 2) & 0xFC);
            }
            pbSrc++;
        }

        return GetDecodeSize(srcLen);
    }

    // =========================================================================
    // _C 变体（客户端混淆：mask/mask2 与 reverse3/reverse4 S 盒原样移植）
    // =========================================================================

    public static int Encode6BitBuf_C(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
    {
        int resultLen = GetEncodeSize(srcLen);
        if (resultLen > destLen)
            srcLen = GetDecodeSize(destLen);

        byte surplusValue = 0;
        int surplusCount = 0;
        int pbSrc = srcOffset;
        int pbDest = destOffset;

        byte encodeLength = (byte)((srcLen + (srcLen + 2) / 3) % 256 ^ 0x90);

        for (int i = 0; i < srcLen; i++)
        {
            byte c = src[pbSrc];
            c = (byte)(Mask2_C[c ^ 0xCC] ^ encodeLength);

            byte currentBit6Value = (byte)((surplusValue | (c >> (2 + surplusCount))) & 0x3F);
            surplusValue = (byte)(((c << (8 - (2 + surplusCount))) >> 2) & 0x3F);
            surplusCount += 2;

            dest[pbDest] = (byte)(currentBit6Value + 0x3C);
            pbDest++;

            if (surplusCount == 6)
            {
                dest[pbDest] = (byte)(surplusValue + 0x3C);
                pbDest++;
                surplusValue = 0;
                surplusCount = 0;
            }
            pbSrc++;
        }

        if (surplusCount > 0)
            dest[pbDest] = (byte)(surplusValue + 0x3C);

        return GetEncodeSize(srcLen);
    }

    public static int Decode6BitBuf_C(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
    {
        int resultLen = GetDecodeSize(srcLen);
        if (resultLen > destLen)
            srcLen = GetEncodeSize(destLen);

        byte surplusValue = 0;
        int surplusCount = 0;
        int pbSrc = srcOffset;
        int pbDest = destOffset;

        byte encodeLength = (byte)((srcLen % 256) ^ 0xBA);

        for (int i = 0; i < srcLen; i++)
        {
            byte b = src[pbSrc];
            if (b < 0x3C || b >= 0x3C + 64)
            {
                Array.Clear(dest, destOffset, destLen);
                return 0;
            }

            byte bSrc = (byte)(b - 0x3C);
            if (surplusCount > 0)
            {
                byte currentBit8Value = (byte)(surplusValue | (bSrc >> (surplusCount - 2)));
                currentBit8Value = (byte)(Reverse4_C[Reverse3_C[currentBit8Value ^ encodeLength]] ^ 0xA8);
                dest[pbDest] = currentBit8Value;
                pbDest++;
                surplusValue = (byte)(bSrc << (8 - (surplusCount - 2)));
                surplusCount -= 2;
            }
            else
            {
                surplusCount = 6;
                surplusValue = (byte)((bSrc << 2) & 0xFC);
            }
            pbSrc++;
        }

        return GetDecodeSize(srcLen);
    }

    // =========================================================================
    // _S 变体（服务端混淆）
    // =========================================================================

    public static int Encode6BitBuf_S(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
    {
        int resultLen = GetEncodeSize(srcLen);
        if (resultLen > destLen)
            srcLen = GetDecodeSize(destLen);

        byte surplusValue = 0;
        int surplusCount = 0;
        int pbSrc = srcOffset;
        int pbDest = destOffset;

        byte encodeLength = (byte)(((srcLen + (srcLen + 2) / 3) % 256) ^ 0xBA);

        for (int i = 0; i < srcLen; i++)
        {
            byte c = src[pbSrc];
            c = (byte)(Mask3_S[Mask4_S[c ^ 0xA8]] ^ encodeLength);

            byte currentBit6Value = (byte)((surplusValue | (c >> (2 + surplusCount))) & 0x3F);
            surplusValue = (byte)(((c << (8 - (2 + surplusCount))) >> 2) & 0x3F);
            surplusCount += 2;

            dest[pbDest] = (byte)(currentBit6Value + 0x3C);
            pbDest++;

            if (surplusCount == 6)
            {
                dest[pbDest] = (byte)(surplusValue + 0x3C);
                pbDest++;
                surplusValue = 0;
                surplusCount = 0;
            }
            pbSrc++;
        }

        if (surplusCount > 0)
            dest[pbDest] = (byte)(surplusValue + 0x3C);

        return GetEncodeSize(srcLen);
    }

    public static int Decode6BitBuf_S(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
    {
        int resultLen = GetDecodeSize(srcLen);
        if (resultLen > destLen)
            srcLen = GetEncodeSize(destLen);

        byte surplusValue = 0;
        int surplusCount = 0;
        int pbSrc = srcOffset;
        int pbDest = destOffset;

        byte encodeLength = (byte)(Mask_S[(srcLen % 256) ^ 0x90]);

        for (int i = 0; i < srcLen; i++)
        {
            byte b = src[pbSrc];
            if (b < 0x3C || b >= 0x3C + 64)
            {
                Array.Clear(dest, destOffset, destLen);
                return 0;
            }

            byte bSrc = (byte)(b - 0x3C);
            if (surplusCount > 0)
            {
                byte currentBit8Value = (byte)(surplusValue | (bSrc >> (surplusCount - 2)));
                currentBit8Value = (byte)(Reverse2_S[currentBit8Value ^ encodeLength] ^ 0xCC);
                dest[pbDest] = currentBit8Value;
                pbDest++;
                surplusValue = (byte)(bSrc << (8 - (surplusCount - 2)));
                surplusCount -= 2;
            }
            else
            {
                surplusCount = 6;
                surplusValue = (byte)((bSrc << 2) & 0xFC);
            }
            pbSrc++;
        }

        return GetDecodeSize(srcLen);
    }

    // =========================================================================
    // 默认分派（EDCODEBEIJING=0 → _N）
    // =========================================================================

    public static int Encode6BitBuf(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
        => EDCODEBEIJING == 0
            ? Encode6BitBuf_N(src, srcOffset, dest, destOffset, srcLen, destLen)
            : Encode6BitBuf_C(src, srcOffset, dest, destOffset, srcLen, destLen);

    public static int Decode6BitBuf(byte[] src, int srcOffset, byte[] dest, int destOffset, int srcLen, int destLen)
        => EDCODEBEIJING == 0
            ? Decode6BitBuf_N(src, srcOffset, dest, destOffset, srcLen, destLen)
            : Decode6BitBuf_S(src, srcOffset, dest, destOffset, srcLen, destLen);

    // =========================================================================
    // 消息 / 字符串 / 缓冲
    // =========================================================================

    public static byte[] EncodeMessage(in TDefaultMessage msg)
    {
        byte[] raw = StructBytes.BytesOf(msg);
        byte[] result = new byte[GetEncodeSize(raw.Length)];
        Encode6BitBuf(raw, 0, result, 0, raw.Length, result.Length);
        return result;
    }

    public static int EncodeMessage(in TDefaultMessage msg, byte[] dest, int destLen)
    {
        byte[] raw = StructBytes.BytesOf(msg);
        return Encode6BitBuf(raw, 0, dest, 0, raw.Length, destLen);
    }

    public static TDefaultMessage DecodeMessage(byte[] s)
    {
        if (s == null || s.Length == 0)
            return default;
        byte[] raw = new byte[TDefaultMessage.SizeOf];
        Decode6BitBuf(s, 0, raw, 0, s.Length, raw.Length);
        return StructBytes.FromBytes<TDefaultMessage>(raw);
    }

    public static TDefaultMessage DecodeMessage(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return default;
        byte[] raw = new byte[TDefaultMessage.SizeOf];
        Decode6BitBuf(src, 0, raw, 0, srcLen, raw.Length);
        return StructBytes.FromBytes<TDefaultMessage>(raw);
    }

    public static byte[] EncodeString(string s)
    {
        byte[] data = DelphiRTL.AnsiBytes(s);
        if (data.Length == 0)
            return Array.Empty<byte>();
        byte[] result = new byte[GetEncodeSize(data.Length)];
        Encode6BitBuf(data, 0, result, 0, data.Length, result.Length);
        return result;
    }

    public static int EncodeString(string s, byte[] dest, int destLen)
    {
        byte[] data = DelphiRTL.AnsiBytes(s);
        if (data.Length == 0)
            return 0;
        return Encode6BitBuf(data, 0, dest, 0, data.Length, destLen);
    }

    public static byte[] EncodeBuffer(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return Array.Empty<byte>();
        byte[] result = new byte[GetEncodeSize(srcLen)];
        Encode6BitBuf(src, 0, result, 0, srcLen, result.Length);
        return result;
    }

    public static int EncodeBuffer(byte[] src, int srcLen, byte[] dest, int destLen)
    {
        if (srcLen == 0)
            return 0;
        return Encode6BitBuf(src, 0, dest, 0, srcLen, destLen);
    }

    public static byte[] DecodeString(byte[] s)
    {
        if (s == null || s.Length == 0)
            return Array.Empty<byte>();
        byte[] result = new byte[GetDecodeSize(s.Length)];
        Decode6BitBuf(s, 0, result, 0, s.Length, result.Length);
        return result;
    }

    public static int DecodeString(byte[] s, byte[] dest, int destLen)
    {
        if (s == null || s.Length == 0)
            return 0;
        return Decode6BitBuf(s, 0, dest, 0, s.Length, destLen);
    }

    public static byte[] DecodeBuffer(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return Array.Empty<byte>();
        byte[] result = new byte[GetDecodeSize(srcLen)];
        Decode6BitBuf(src, 0, result, 0, srcLen, result.Length);
        return result;
    }

    public static int DecodeBuffer(byte[] src, int srcLen, byte[] dest, int destLen)
    {
        if (srcLen == 0)
            return 0;
        return Decode6BitBuf(src, 0, dest, 0, srcLen, destLen);
    }

    // ---- string 便捷重载（GBK 文本）----

    public static string EncodeStringText(string s) => DelphiRTL.AnsiString(EncodeString(s));
    public static string DecodeStringText(byte[] s) => DelphiRTL.AnsiString(DecodeString(s));

    // ---- K / z 前缀（等价转发）----

    public static byte[] EncodeStringK(string s) => EncodeString(s);
    public static byte[] DecodeStringK(byte[] s) => DecodeString(s);
    public static byte[] zEncodeString(string s) => EncodeString(s);
    public static byte[] zEncodeString(byte[] src, int srcLen) => EncodeBuffer(src, srcLen);
    public static byte[] zDecodeString(byte[] s) => DecodeString(s);
    public static byte[] zDecodeString(byte[] src, int srcLen) => DecodeBuffer(src, srcLen);
    public static byte[] zEncodeBuffer(byte[] src, int srcLen) => EncodeBuffer(src, srcLen);
    public static int zDecodeBuffer(byte[] s, byte[] dest, int destLen) => DecodeString(s, dest, destLen);

    // =========================================================================
    // zLib 系列：压缩后 6-Bit 编码 / 解码后解压
    // =========================================================================

    public static byte[] zLibEncodeString(string s)
    {
        byte[] data = DelphiRTL.AnsiBytes(s);
        if (data.Length == 0)
            return Array.Empty<byte>();
        byte[] zLibBuf = ZlibEx.CompressBuf(data, data.Length);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return Array.Empty<byte>();
        byte[] result = new byte[GetEncodeSize(zLibBuf.Length)];
        Encode6BitBuf(zLibBuf, 0, result, 0, zLibBuf.Length, result.Length);
        return result;
    }

    public static int zLibEncodeString(string s, byte[] dest, int destLen)
    {
        byte[] data = DelphiRTL.AnsiBytes(s);
        if (data.Length == 0)
            return 0;
        byte[] zLibBuf = ZlibEx.CompressBuf(data, data.Length);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        return Encode6BitBuf(zLibBuf, 0, dest, 0, zLibBuf.Length, destLen);
    }

    public static byte[] zLibEncodeBuffer(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return Array.Empty<byte>();
        byte[] zLibBuf = ZlibEx.CompressBuf(src, srcLen);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return Array.Empty<byte>();
        byte[] result = new byte[GetEncodeSize(zLibBuf.Length)];
        Encode6BitBuf(zLibBuf, 0, result, 0, zLibBuf.Length, result.Length);
        return result;
    }

    public static int zLibEncodeBuffer(byte[] src, int srcLen, byte[] dest, int destLen)
    {
        if (srcLen == 0)
            return 0;
        byte[] zLibBuf = ZlibEx.CompressBuf(src, srcLen);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        return Encode6BitBuf(zLibBuf, 0, dest, 0, zLibBuf.Length, destLen);
    }

    public static byte[] zLibDecodeString(byte[] s)
    {
        if (s == null || s.Length == 0)
            return Array.Empty<byte>();
        int decodeBufSize = GetDecodeSize(s.Length);
        byte[] decodeBuf = new byte[decodeBufSize];
        Decode6BitBuf(s, 0, decodeBuf, 0, s.Length, decodeBufSize);
        return ZlibEx.DecompressBuf(decodeBuf, decodeBufSize) ?? Array.Empty<byte>();
    }

    public static int zLibDecodeString(byte[] s, byte[] dest, int destLen)
    {
        if (s == null || s.Length == 0)
            return 0;
        int decodeBufSize = GetDecodeSize(s.Length);
        byte[] decodeBuf = new byte[decodeBufSize];
        Decode6BitBuf(s, 0, decodeBuf, 0, s.Length, decodeBufSize);
        byte[] zLibBuf = ZlibEx.DecompressBuf(decodeBuf, decodeBufSize);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        int n = Math.Min(zLibBuf.Length, destLen);
        Array.Copy(zLibBuf, 0, dest, 0, n);
        return zLibBuf.Length;
    }

    public static byte[] zLibDecodeBuffer(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return Array.Empty<byte>();
        int decodeBufSize = GetDecodeSize(srcLen);
        byte[] decodeBuf = new byte[decodeBufSize];
        Decode6BitBuf(src, 0, decodeBuf, 0, srcLen, decodeBufSize);
        return ZlibEx.DecompressBuf(decodeBuf, decodeBufSize) ?? Array.Empty<byte>();
    }

    public static int zLibDecodeBuffer(byte[] src, int srcLen, byte[] dest, int destLen)
    {
        if (srcLen == 0)
            return 0;
        int decodeBufSize = GetDecodeSize(srcLen);
        byte[] decodeBuf = new byte[decodeBufSize];
        Decode6BitBuf(src, 0, decodeBuf, 0, srcLen, decodeBufSize);
        byte[] zLibBuf = ZlibEx.DecompressBuf(decodeBuf, decodeBufSize);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        int n = Math.Min(zLibBuf.Length, destLen);
        Array.Copy(zLibBuf, 0, dest, 0, n);
        return zLibBuf.Length;
    }

    // ---- 纯压缩（无 6-Bit）----

    public static byte[] zLibCompressString(string s)
    {
        byte[] data = DelphiRTL.AnsiBytes(s);
        if (data.Length == 0)
            return Array.Empty<byte>();
        return ZlibEx.CompressBuf(data, data.Length) ?? Array.Empty<byte>();
    }

    public static int zLibCompressString(string s, byte[] dest, int destLen)
    {
        byte[] data = DelphiRTL.AnsiBytes(s);
        if (data.Length == 0)
            return 0;
        byte[] zLibBuf = ZlibEx.CompressBuf(data, data.Length);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        if (destLen >= zLibBuf.Length)
        {
            Array.Copy(zLibBuf, 0, dest, 0, zLibBuf.Length);
            return zLibBuf.Length;
        }
        return 0;
    }

    public static byte[] zLibCompressBuffer(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return Array.Empty<byte>();
        return ZlibEx.CompressBuf(src, srcLen) ?? Array.Empty<byte>();
    }

    public static int zLibCompressBuffer(byte[] src, int srcLen, byte[] dest, int destLen)
    {
        if (srcLen == 0)
            return 0;
        byte[] zLibBuf = ZlibEx.CompressBuf(src, srcLen);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        if (destLen >= zLibBuf.Length)
        {
            Array.Copy(zLibBuf, 0, dest, 0, zLibBuf.Length);
            return zLibBuf.Length;
        }
        return 0;
    }

    public static byte[] zLibDecompressString(byte[] s)
    {
        if (s == null || s.Length == 0)
            return Array.Empty<byte>();
        return ZlibEx.DecompressBuf(s, s.Length) ?? Array.Empty<byte>();
    }

    public static int zLibDecompressString(byte[] s, byte[] dest, int destLen)
    {
        if (s == null || s.Length == 0)
            return 0;
        byte[] zLibBuf = ZlibEx.DecompressBuf(s, s.Length);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        if (destLen >= zLibBuf.Length)
        {
            Array.Copy(zLibBuf, 0, dest, 0, zLibBuf.Length);
            return zLibBuf.Length;
        }
        return 0;
    }

    public static byte[] zLibDecompressBuffer(byte[] src, int srcLen)
    {
        if (srcLen == 0)
            return Array.Empty<byte>();
        return ZlibEx.DecompressBuf(src, srcLen) ?? Array.Empty<byte>();
    }

    public static int zLibDecompressBuffer(byte[] src, int srcLen, byte[] dest, int destLen)
    {
        if (srcLen == 0)
            return 0;
        byte[] zLibBuf = ZlibEx.DecompressBuf(src, srcLen);
        if (zLibBuf == null || zLibBuf.Length == 0)
            return 0;
        if (destLen >= zLibBuf.Length)
        {
            Array.Copy(zLibBuf, 0, dest, 0, zLibBuf.Length);
            return zLibBuf.Length;
        }
        return 0;
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace GXX.Core.Crypto;

/// <summary>Base64.pas 1:1（DCPcrypt 标准字母表，等价于 RFC4648）。</summary>
public static class Base64Util
{
    public static string Base64EncodeStr(byte[] value)
    {
        if (value == null || value.Length == 0) return "";
        return Convert.ToBase64String(value);
    }

    public static byte[] Base64DecodeStr(string value)
    {
        if (string.IsNullOrEmpty(value)) return Array.Empty<byte>();
        try
        {
            return Convert.FromBase64String(value);
        }
        catch
        {
            // 容忍非法长度（原实现在长度非 4 倍数时的宽松行为）
            try
            {
                string padded = value;
                int rem = padded.Length % 4;
                if (rem == 1) padded = padded.Substring(0, padded.Length - 1);
                else if (rem > 1) padded = padded.PadRight(padded.Length + (4 - rem), '=');
                return Convert.FromBase64String(padded);
            }
            catch { return Array.Empty<byte>(); }
        }
    }

    // ---- 别名（对应 Base64EncryBuffer/Base64DecryBuffer）----

    public static string Base64EncryBuffer(byte[] buf, int bufsize)
        => Base64EncodeStr(buf.AsSpan(0, bufsize).ToArray());
}

/// <summary>MD5Util.pas（标准 MD5，System.Security.Cryptography 实现）。</summary>
public static class MD5Util
{
    public static byte[] MD5Buffer(byte[] data, int offset, int count)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(data, offset, count);
    }

    public static string MD5BufHex(byte[] data, int offset, int count)
        => BitConverter.ToString(MD5Buffer(data, offset, count)).Replace("-", "").ToLowerInvariant();

    public static string MD5Print(byte[] digest)
        => BitConverter.ToString(digest).Replace("-", "").ToLowerInvariant();

    public static string MD5String(string s)
    {
        byte[] data = EncodingInit.GBK.GetBytes(s ?? "");
        return MD5BufHex(data, 0, data.Length);
    }
}

/// <summary>
/// CheckCrc.pas 的 CRC-32。
/// ⚠ 注意初值不是标准的 $FFFFFFFF，而是 <c>$DBC66688</c>：
/// <code>
///   nCrc := $DBC66688;              // CheckCrc.pas:119
///   nCrc := nCrc xor $FFFFFFFF;     // CheckCrc.pas:123
///   ... 逐字节查表 ...
///   Result := nCrc xor $FFFFFFFF;   // CheckCrc.pas:154
/// </code>
/// 校验表与 zlib 相同（反射，多项式 $EDB88320），但初值不同 ⇒ 结果与标准 CRC-32 不同。
/// Client.dat / ClientData.dat 尾记录的 nCrc 必须用本函数计算（已对真机文件验证通过）。
/// </summary>
public static class CheckCrc
{
    private static readonly uint[] CrcTable = UnitDes.BuildCrcTablePublic();

    /// <summary>Delphi CheckCrc.Crc32：初值 $DBC66688。</summary>
    public static uint Crc32(byte[] buf, int len) => Crc32(buf, 0, len);

    public static uint Crc32(byte[] buf, int offset, int len)
    {
        if (buf == null || len <= 0) return 0;
        uint nCrc = 0xDBC66688u ^ 0xFFFFFFFFu;   // = $24399977
        for (int i = 0; i < len; i++)
            nCrc = CrcTable[(nCrc ^ buf[offset + i]) & 0xFF] ^ (nCrc >> 8);
        return nCrc ^ 0xFFFFFFFFu;
    }

    public static uint BufferCRC(byte[] buf, int len) => Crc32(buf, len);

    /// <summary>标准 CRC-32（初值 $FFFFFFFF）。用于 PAK 等其它场景。</summary>
    public static uint Crc32Standard(byte[] buf, int len) => UnitDes.CalcCrc32(buf, 0, len);
}

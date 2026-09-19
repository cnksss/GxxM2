using System;
using System.Security.Cryptography;
using System.Text;

namespace GXX.Core.Crypto;

/// <summary>
/// UnitDes.pas 1:1 转换：OpenSSL(libdes) 风格 DES + 自定义 CBC（BS=20 字节块）+ SHA-1 派生密钥。
/// 表数据见 UnitDes.Tables.g.cs（从原 .pas 逐字节提取）。
/// </summary>
public static unsafe partial class UnitDes
{
    public const int BS = 20;   // 内部块大小（字节）— 注意不是 8！

    // ---- UnitHash.pas：标准 SHA-1 ----

    public static void Hash(string key, byte[] digest)
    {
        byte[] data = EncodingInit.GBK.GetBytes(key ?? "");
        using var sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(data);
        Array.Copy(hash, digest, 20);
    }

    public static void XorBlock(byte[] a, int aOffset, byte[] b, int bOffset, int size)
    {
        for (int i = 0; i < size; i++)
            a[aOffset + i] ^= b[bOffset + i];
    }

    // ---- 位操作 ----

    private static void PermOp(ref uint a, ref uint b, int n, uint m)
    {
        uint t = ((a >> n) ^ b) & m;
        b ^= t;
        a ^= t << n;
    }

    private static void HPermOp(ref uint a, int n, uint m)
    {
        uint t = ((a << (16 - n)) ^ a) & m;
        a ^= t ^ (t >> (16 - n));
    }

    public static void DoInit(byte[] keyB, uint[] keyData)
    {
        uint c = (uint)(keyB[0] | (keyB[1] << 8) | (keyB[2] << 16) | (keyB[3] << 24));
        uint d = (uint)(keyB[4] | (keyB[5] << 8) | (keyB[6] << 16) | (keyB[7] << 24));
        PermOp(ref d, ref c, 4, 0x0F0F0F0F);
        HPermOp(ref c, -2, 0xCCCC0000);
        HPermOp(ref d, -2, 0xCCCC0000);
        PermOp(ref d, ref c, 1, 0x55555555);
        PermOp(ref c, ref d, 8, 0x00FF00FF);
        PermOp(ref d, ref c, 1, 0x55555555);
        d = ((d & 0xFF) << 16) | (d & 0xFF00) | ((d & 0xFF0000) >> 16) | ((c & 0xF0000000) >> 4);
        c &= 0xFFFFFFF;   // 原代码 c := c and $FFFFFFF
        for (int i = 0; i < 16; i++)
        {
            if (Shifts2[i] != 0)
            {
                c = (c >> 2) | (c << 26);
                d = (d >> 2) | (d << 26);
            }
            else
            {
                c = (c >> 1) | (c << 27);
                d = (d >> 1) | (d << 27);
            }
            c &= 0xFFFFFFF;
            d &= 0xFFFFFFF;
            uint s = des_skb[0][c & 0x3F] |
                     des_skb[1][((c >> 6) & 0x03) | ((c >> 7) & 0x3C)] |
                     des_skb[2][((c >> 13) & 0x0F) | ((c >> 14) & 0x30)] |
                     des_skb[3][((c >> 20) & 0x01) | ((c >> 21) & 0x06) | ((c >> 22) & 0x38)];
            uint t = des_skb[4][d & 0x3F] |
                     des_skb[5][((d >> 7) & 0x03) | ((d >> 8) & 0x3C)] |
                     des_skb[6][(d >> 15) & 0x3F] |
                     des_skb[7][((d >> 21) & 0x0F) | ((d >> 22) & 0x30)];
            uint t2 = (t << 16) | (s & 0xFFFF);
            keyData[(i << 1)] = (t2 << 2) | (t2 >> 30);
            t2 = (s >> 16) | (t & 0xFFFF0000);
            keyData[(i << 1) + 1] = (t2 << 6) | (t2 >> 26);
        }
    }

    private static readonly byte[] Shifts2 = { 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0 };

    private static uint ComputeL(uint r, uint l, int i, uint[] keyData)
    {
        uint u = r ^ keyData[i];
        uint t = r ^ keyData[i + 1];
        t = (t >> 4) | (t << 28);
        return l ^ des_sptrans[0][(u >> 2) & 0x3F] ^
                    des_sptrans[2][(u >> 10) & 0x3F] ^
                    des_sptrans[4][(u >> 18) & 0x3F] ^
                    des_sptrans[6][(u >> 26) & 0x3F] ^
                    des_sptrans[1][(t >> 2) & 0x3F] ^
                    des_sptrans[3][(t >> 10) & 0x3F] ^
                    des_sptrans[5][(t >> 18) & 0x3F] ^
                    des_sptrans[7][(t >> 26) & 0x3F];
    }

    public static void EncryptBlock(byte[] inData, int inOffset, byte[] outData, int outOffset, uint[] keyData)
    {
        uint r = BitConverter.ToUInt32(inData, inOffset);
        uint l = BitConverter.ToUInt32(inData, inOffset + 4);
        uint t = ((l >> 4) ^ r) & 0x0F0F0F0F;
        r ^= t;
        l ^= t << 4;
        t = ((r >> 16) ^ l) & 0x0000FFFF;
        l ^= t;
        r ^= t << 16;
        t = ((l >> 2) ^ r) & 0x33333333;
        r ^= t;
        l ^= t << 2;
        t = ((r >> 8) ^ l) & 0x00FF00FF;
        l ^= t;
        r ^= t << 8;
        t = ((l >> 1) ^ r) & 0x55555555;
        r ^= t;
        l ^= t << 1;
        r = (r >> 29) | (r << 3);
        l = (l >> 29) | (l << 3);
        int i = 0;
        while (i < 32)
        {
            l = ComputeL(r, l, i, keyData);
            r = ComputeL(l, r, i + 2, keyData);
            l = ComputeL(r, l, i + 4, keyData);
            r = ComputeL(l, r, i + 6, keyData);
            i += 8;
        }
        r = (r >> 3) | (r << 29);
        l = (l >> 3) | (l << 29);
        t = ((r >> 1) ^ l) & 0x55555555;
        l ^= t;
        r ^= t << 1;
        t = ((l >> 8) ^ r) & 0x00FF00FF;
        r ^= t;
        l ^= t << 8;
        t = ((r >> 2) ^ l) & 0x33333333;
        l ^= t;
        r ^= t << 2;
        t = ((l >> 16) ^ r) & 0x0000FFFF;
        r ^= t;
        l ^= t << 16;
        t = ((r >> 4) ^ l) & 0x0F0F0F0F;
        l ^= t;
        r ^= t << 4;
        WriteU32(outData, outOffset, l);
        WriteU32(outData, outOffset + 4, r);
    }

    private static uint ComputeLD(uint r, uint l, int i, uint[] keyData)
    {
        // 解密轮：与加密轮相同的 S 盒运算（封装复用）
        return ComputeL(r, l, i, keyData);
    }

    public static void DecryptBlock(byte[] inData, int inOffset, byte[] outData, int outOffset, uint[] keyData)
    {
        uint r = BitConverter.ToUInt32(inData, inOffset);
        uint l = BitConverter.ToUInt32(inData, inOffset + 4);
        uint t = ((l >> 4) ^ r) & 0x0F0F0F0F;
        r ^= t;
        l ^= t << 4;
        t = ((r >> 16) ^ l) & 0x0000FFFF;
        l ^= t;
        r ^= t << 16;
        t = ((l >> 2) ^ r) & 0x33333333;
        r ^= t;
        l ^= t << 2;
        t = ((r >> 8) ^ l) & 0x00FF00FF;
        l ^= t;
        r ^= t << 8;
        t = ((l >> 1) ^ r) & 0x55555555;
        r ^= t;
        l ^= t << 1;
        r = (r >> 29) | (r << 3);
        l = (l >> 29) | (l << 3);
        int i = 30;
        while (i > 0)
        {
            l = ComputeLD(r, l, i, keyData);
            r = ComputeLD(l, r, i - 2, keyData);
            l = ComputeLD(r, l, i - 4, keyData);
            r = ComputeLD(l, r, i - 6, keyData);
            i -= 8;
        }
        r = (r >> 3) | (r << 29);
        l = (l >> 3) | (l << 29);
        t = ((r >> 1) ^ l) & 0x55555555;
        l ^= t;
        r ^= t << 1;
        t = ((l >> 8) ^ r) & 0x00FF00FF;
        r ^= t;
        l ^= t << 8;
        t = ((r >> 2) ^ l) & 0x33333333;
        l ^= t;
        r ^= t << 2;
        t = ((l >> 16) ^ r) & 0x0000FFFF;
        r ^= t;
        l ^= t << 16;
        t = ((r >> 4) ^ l) & 0x0F0F0F0F;
        l ^= t;
        r ^= t << 4;
        WriteU32(outData, outOffset, l);
        WriteU32(outData, outOffset + 4, r);
    }

    /// <summary>EncryptBlock3（KeyData 为 3×32 DWord）。</summary>
    public static void EncryptBlock3Core(byte[] data, int off, uint[][] kd3)
    {
        byte[] tmp = new byte[8];
        EncryptBlock(data, off, tmp, 0, kd3[0]);
        DecryptBlock(tmp, 0, tmp, 0, kd3[1]);
        EncryptBlock(tmp, 0, data, off, kd3[2]);
    }

    public static void DecryptBlock3Core(byte[] data, int off, uint[][] kd3)
    {
        byte[] tmp = new byte[8];
        DecryptBlock(data, off, tmp, 0, kd3[2]);
        EncryptBlock(tmp, 0, tmp, 0, kd3[1]);
        DecryptBlock(tmp, 0, data, off, kd3[0]);
    }

    // ---- 密钥派生 ----

    public static void GetKeyData(string key, byte[] chain, uint[] keyData)
    {
        byte[] iv = new byte[BS];
        byte[] digest = new byte[20];
        Hash(key, digest);
        byte[] keyB = new byte[8];
        Array.Copy(digest, keyB, 8);
        DoInit(keyB, keyData);
        Array.Fill(iv, (byte)0x8F);
        EncryptBlock(iv, 0, iv, 0, keyData);
        Array.Copy(iv, chain, BS);
    }

    public static void GetKeyDataPak(string key, byte[] chain, uint[] keyData)
    {
        byte[] iv = new byte[BS];
        byte[] digest = new byte[20];
        Hash(key, digest);
        byte[] keyB = new byte[8];
        Array.Copy(digest, keyB, 8);
        DoInit(keyB, keyData);
        Array.Fill(iv, (byte)0x60);
        EncryptBlock(iv, 0, iv, 0, keyData);
        Array.Copy(iv, chain, BS);
    }

    public static void GetKeyDataPak2(string key, byte[] chain, uint[] keyData)
    {
        byte[] iv = new byte[BS];
        byte[] digest = new byte[20];
        Hash(key, digest);
        for (int i = 1; i < digest.Length; i += 2)
            digest[i] ^= digest[i - 1];
        byte[] keyB = new byte[8];
        Array.Copy(digest, keyB, 8);
        DoInit(keyB, keyData);
        Array.Fill(iv, (byte)0x4F);
        EncryptBlock(iv, 0, iv, 0, keyData);
        Array.Copy(iv, chain, BS);
    }

    // ---- CBC 模式（BS=20）----

    public static void EncryptCBC(byte[] inData, byte[] outData, int size, byte[] chain, uint[] keyData)
    {
        int blocks = size / BS;
        for (int i = 0; i < blocks; i++)
        {
            int off = i * BS;
            Array.Copy(inData, off, outData, off, BS);
            XorBlock(outData, off, chain, 0, BS);
            EncryptBlock(outData, off, outData, off, keyData);
            Array.Copy(outData, off, chain, 0, BS);
        }
        int rem = size % BS;
        if (rem != 0)
        {
            EncryptBlock(chain, 0, chain, 0, keyData);
            int off = blocks * BS;
            Array.Copy(inData, off, outData, off, rem);
            XorBlock(outData, off, chain, 0, rem);
        }
    }

    public static void DecryptCBC(byte[] inData, byte[] outData, int size, byte[] chain, uint[] keyData)
    {
        byte[] temp = new byte[BS];
        int blocks = size / BS;
        for (int i = 0; i < blocks; i++)
        {
            int off = i * BS;
            Array.Copy(inData, off, outData, off, BS);
            Array.Copy(inData, off, temp, 0, BS);
            DecryptBlock(outData, off, outData, off, keyData);
            XorBlock(outData, off, chain, 0, BS);
            Array.Copy(temp, chain, BS);
        }
        int rem = size % BS;
        if (rem != 0)
        {
            EncryptBlock(chain, 0, chain, 0, keyData);
            int off = blocks * BS;
            Array.Copy(inData, off, outData, off, rem);
            XorBlock(outData, off, chain, 0, rem);
        }
    }

    // ---- 对外接口（对应原 EncryptDes/DecryptDes/StrDes 系列）----

    public static void EncryptDes(byte[] inData, byte[] outData, int size, string key)
    {
        var keyData = new uint[32];
        var chain = new byte[BS];
        GetKeyData(key, chain, keyData);
        EncryptCBC(inData, outData, size, chain, keyData);
    }

    public static void DecryptDes(byte[] inData, byte[] outData, int size, string key)
    {
        var keyData = new uint[32];
        var chain = new byte[BS];
        GetKeyData(key, chain, keyData);
        DecryptCBC(inData, outData, size, chain, keyData);
    }

    public static byte[] EncryptStrDes(string source, string key)
    {
        byte[] data = EncodingInit.GBK.GetBytes(source ?? "");
        byte[] result = new byte[data.Length];
        EncryptDes(data, result, data.Length, key);
        return result;
    }

    public static byte[] EncryptStrDesBytes(byte[] source, string key)
    {
        byte[] result = new byte[source.Length];
        EncryptDes(source, result, source.Length, key);
        return result;
    }

    public static byte[] DecryptStrDesBytes(byte[] source, string key)
    {
        byte[] result = new byte[source.Length];
        DecryptDes(source, result, source.Length, key);
        return result;
    }

    public static byte[] DecryptStrDes(byte[] source, string key)
    {
        byte[] result = new byte[source.Length];
        DecryptDes(source, result, source.Length, key);
        return result;
    }

    public static string EncryptStrDesText(string source, string key)
    {
        // 密文是任意字节，使用 Latin-1 (ISO-8859-1) 字节透明映射承载为 string（对应 Delphi AnsiString 语义）
        byte[] plain = EncodingInit.GBK.GetBytes(source ?? "");
        byte[] cipher = EncryptStrDesBytes(plain, key);
        return System.Text.Encoding.GetEncoding(28591).GetString(cipher);
    }

    public static string DecryptStrDesText(string source, string key)
    {
        byte[] cipher = System.Text.Encoding.GetEncoding(28591).GetBytes(source);
        byte[] plain = DecryptStrDesBytes(cipher, key);
        return EncodingInit.GBK.GetString(plain);
    }

    private static void WriteU32(byte[] buf, int offset, uint value)
    {
        buf[offset] = (byte)value;
        buf[offset + 1] = (byte)(value >> 8);
        buf[offset + 2] = (byte)(value >> 16);
        buf[offset + 3] = (byte)(value >> 24);
    }

    // ---- CRC32（crc_table 与 zlib 标准表一致，算法生成）----

    private static readonly uint[] CrcTable = BuildCrcTable();

    private static uint[] BuildCrcTable()
    {
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint c = i;
            for (int k = 0; k < 8; k++)
                c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            table[i] = c;
        }
        return table;
    }

    public static uint CalcCrc32(byte[] data, int offset, int count)
    {
        uint crc = 0xFFFFFFFF;
        for (int i = offset; i < offset + count; i++)
            crc = CrcTable[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
        return crc ^ 0xFFFFFFFF;
    }

    /// <summary>暴露标准 zlib CRC-32 表（make_crc_table 结果）。</summary>
    public static uint[] BuildCrcTablePublic() => BuildCrcTable();

    /// <summary>公开标准 DES Key Schedule（供 DesNew2 等派生实现复用）。</summary>
    public static void DoInitPublic(byte[] keyB, uint[] keyData) => DoInit(keyB, keyData);

    /// <summary>公开标准 DES 加密块（供 DesNew2 尾块处理复用）。</summary>
    public static void EncryptBlockPublic(byte[] data, int off, uint[] keyData)
        => EncryptBlock(data, off, data, off, keyData);
}

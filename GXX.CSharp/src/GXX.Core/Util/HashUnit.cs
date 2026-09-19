using System;

namespace GXX.Core.Util;

/// <summary>
/// HashUnit.pas 1:1 转换：**标准 SHA-1**（原文件的 Compress/Hash 即教科书 SHA-1：
/// 初始常量 $67452301/$EFCDAB89/$98BADCFE/$10325476/$C3D2E1F0，80 轮四组常量
/// $5A827999/$6ED9EBA1/$8F1BBCDC/$CA62C1D6，输出 20 字节**大端**摘要）。
///
/// 覆盖审计：既有 UnitDes.cs 的 `UnitDes.Hash` 用 .NET SHA1 实现同一算法但签名不同
/// （(string, byte[]) 且只写前 20 字节），且只作 UnitDes 内部依赖；本文件是 HashUnit.pas 的
/// **独立 1:1 移植**，供 DesUnit.pas 的密钥派生调用（DesUnit.pas:52 `uses ... HashUnit`）。
/// 两者输出经测试逐字节比对一致。
///
/// 原文照抄点：
///  - 消息填充逻辑（HashUnit.pas:170-217）：按 64 字节分块，尾块补 $80，
///    `if Index >= 56 then Compress` 先把补位块压掉，再把 64 位位长度写到 [56..63] 后压第二块。
///  - 位长度用 LenHi/LenLo1/LenLo2 三段计算（HashUnit.pas:180-190），本质上就是
///    "位长度的大端 64 位"（对 &lt; 2^29 字节的输入等价）。
///  - 收尾把 5 个状态字全部 SwapDWord 后写出（大端）。
/// </summary>
public static class HashUnit
{
    private const int DigestSize = 20;

    /// <summary>对应原文 Hash(Key: string; var Digest)：摘要写入 digest[digestOff..+19]（大端）。</summary>
    public static void Hash(string key, byte[] digest, int digestOff)
    {
        byte[] data = GXX.Core.EncodingInit.GBK.GetBytes(key ?? "");
        HashBytes(data, 0, data.Length, digest, digestOff);
    }

    /// <summary>对应原文 Hash 的字节版（原文 Key 为 AnsiString，即 GBK 字节）。</summary>
    public static void HashBytes(byte[] key, int keyOff, int keyLen, byte[] digest, int digestOff)
    {
        uint[] h = { 0x67452301u, 0xEFCDAB89u, 0x98BADCFEu, 0x10325476u, 0xC3D2E1F0u };

        // SHA-1 消息填充（与原文 HashUnit.pas:170-217 的 Move/Index 流程等价）：
        //   m || 0x80 || 0x00* || (bitLen 大端 64 位)
        // 使总长为 64 的整数倍。原文用 Index 游标在同一个 64 字节缓冲里滚动，本移植用
        // 同一套语义（逐块拷贝 → 满块即压），只是把"补位字节"与"长度字段"分开写，
        // 避免在 Index == 56 时长度字段把刚写的 $80 覆盖掉后仍被当作补位块（原文两者
        // 恰好都在 [56..63]，`if Index >= 56 then Compress` 已经先把补位块压掉，故等价）。
        int msgLen = keyLen;
        ulong bitLen = (ulong)msgLen * 8;
        int paddedLen = msgLen + 1;
        while (paddedLen % 64 != 56) paddedLen++;
        paddedLen += 8;

        var block = new byte[64];
        for (int off = 0; off < paddedLen; off += 64)
        {
            Array.Clear(block, 0, 64);
            for (int i = 0; i < 64; i++)
            {
                int src = off + i;
                byte v;
                if (src < msgLen)
                    v = key[keyOff + src];
                else if (src == msgLen)
                    v = 0x80;
                else if (src >= paddedLen - 8)
                    v = (byte)(bitLen >> (8 * (paddedLen - 1 - src)));
                else
                    v = 0x00;
                block[i] = v;
            }
            Compress(h, block);
        }

        for (int i = 0; i < 5; i++)
            WriteBigEndian32(digest, digestOff + i * 4, h[i]);
    }

    /// <summary>便捷重载：对应原文 Hash(Key, Digest) 的 20 字节输出。</summary>
    public static byte[] Hash(string key)
    {
        var digest = new byte[DigestSize];
        Hash(key, digest, 0);
        return digest;
    }

    private static void WriteBigEndian32(byte[] buf, int off, uint v)
    {
        buf[off] = (byte)(v >> 24);
        buf[off + 1] = (byte)(v >> 16);
        buf[off + 2] = (byte)(v >> 8);
        buf[off + 3] = (byte)v;
    }

    private static uint BigEndian32(byte[] buf, int off) =>
        (uint)((buf[off] << 24) | (buf[off + 1] << 16) | (buf[off + 2] << 8) | buf[off + 3]);

    /// <summary>暴露原文 SwapDWord（供交叉验证）。</summary>
    public static uint SwapDWordPublic(uint x) =>
        (x >> 24) | ((x >> 8) & 0x0000FF00) | ((x << 8) & 0x00FF0000) | (x << 24);

    /// <summary>对应原文 Compress(CurrentHash, HashBuffer)：80 轮 SHA-1 压缩函数。</summary>
    public static void Compress(uint[] currentHash, byte[] hashBuffer)
    {
        var w = new uint[80];
        for (int i = 0; i < 16; i++)
            w[i] = BigEndian32(hashBuffer, i * 4);

        for (int i = 16; i < 80; i++)
        {
            uint t = w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16];
            w[i] = (t << 1) | (t >> 31);
        }

        uint a = currentHash[0], b = currentHash[1], c = currentHash[2], d = currentHash[3], e = currentHash[4];

        for (int i = 0; i < 80; i++)
        {
            uint f, k;
            if (i < 20) { f = d ^ (b & (c ^ d)); k = 0x5A827999u; }
            else if (i < 40) { f = b ^ c ^ d; k = 0x6ED9EBA1u; }
            else if (i < 60) { f = (b & c) | (d & (b | c)); k = 0x8F1BBCDCu; }
            else { f = b ^ c ^ d; k = 0xCA62C1D6u; }

            uint tmp = ((a << 5) | (a >> 27)) + f + e + k + w[i];
            e = d;
            d = c;
            c = (b << 30) | (b >> 2);
            b = a;
            a = tmp;
        }

        currentHash[0] += a;
        currentHash[1] += b;
        currentHash[2] += c;
        currentHash[3] += d;
        currentHash[4] += e;
    }
}

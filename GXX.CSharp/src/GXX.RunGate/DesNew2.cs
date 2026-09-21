using System;
using GXX.Core.Crypto;

namespace GXX.RunGate;

/// <summary>
/// DesNew2.pas 1:1 核心转换：RunGate 专用 DES-CBC(BS=20) 解密器。
/// 密钥派生与 UnitDes 不同：CRC32(Key) 得 KeyA 初值，再经 Jenkins 风格
/// 三字混合（a=$AD2832E3, b=$50FF46DE, c=$F07BB613，含三处非标准移位）得 KeyB；
/// Key64 = [KeyA, KeyB]，DoInit 标准调度；Chain 填 $8F 后加密一次。
/// 解密末轮与尾块沿用原实现的 $33333331 掩码变体（保真复刻）。
/// </summary>
public static unsafe class DesNew2
{
    private static readonly uint[] CrcTable = UnitDes.BuildCrcTablePublic();

    public static void DecryptDes_New2(byte[] inData, byte[] outData, int size, string key)
    {
        // ---- 密钥派生：CRC32 表滚动 + Jenkins 混合（DesNew2.pas 专属）----
        uint keyA = CrcOfKey(key);
        uint keyB = JenkinsMix(key, keyA);
        keyA ^= keyB;

        var keyData = new uint[32];
        var keyB8 = new byte[8];
        BitConverter.GetBytes(keyB).CopyTo(keyB8, 0);
        DoInit(keyB8, keyData);

        // ---- Chain：$8F×20 后加密一次 ----
        var chain = new byte[20];
        Array.Fill(chain, (byte)0x8F);
        EncryptBlockCore(chain, 0, keyData);

        // ---- CBC 解密 ----
        var temp = new byte[20];
        int blocks = size / 20;
        for (int i = 0; i < blocks; i++)
        {
            int off = i * 20;
            Array.Copy(inData, off, outData, off, 20);
            Array.Copy(inData, off, temp, 0, 20);
            DecryptBlockNew2(outData, off, keyData);
            XorBlock(outData, off, chain, 0, 20);
            Array.Copy(temp, chain, 20);
        }
        int rem = size % 20;
        if (rem != 0)
        {
            // 尾块：EncryptBlock(Chain)（原实现注释掉 Decrypt 直接加密 Chain）
            int off = blocks * 20;
            EncryptBlockCore(chain, 0, keyData);
            Array.Copy(inData, off, outData, off, rem);
            XorBlock(outData, off, chain, 0, rem);
        }
    }

    private static uint CrcOfKey(string key)
    {
        byte[] data = GXX.Core.EncodingInit.GBK.GetBytes(key);
        uint keyA = 0xFFFFFFFF;
        foreach (byte b in data)
            keyA = CrcTable[(keyA ^ b) & 0xFF] ^ (keyA >> 8);
        return keyA ^ 0xFFFFFFFF;
    }

    /// <summary>Jenkins 风格三字混合（a/b/c 初值与非常规移位 1:1 复刻）。</summary>
    private static uint JenkinsMix(string key, uint seed)
    {
        byte[] data = GXX.Core.EncodingInit.GBK.GetBytes(key);
        uint a = 0xAD2832E3u, b = 0x50FF46DEu, c = 0xF07BB613u;
        int len = data.Length;
        int k = 0;

        while (len >= 12)
        {
            a += (uint)(data[k + 0] + (data[k + 1] << 8) + (data[k + 2] << 16) + (data[k + 3] << 24));
            b += (uint)(data[k + 5] + (data[k + 4] << 8) + (data[k + 6] << 16) + (data[k + 7] << 24));
            c += (uint)(data[k + 8] + (data[k + 9] << 8) + (data[k + 10] << 16) + (data[k + 11] << 24));

            Mix12(ref a, ref b, ref c);
            k += 12;
            len -= 12;
        }

        c += (uint)data.Length;
        if (len >= 11) c += (uint)data[k + 10] << 24;
        if (len >= 10) c += (uint)data[k + 9] << 16;
        if (len >= 9) c += (uint)data[k + 8] << 8;
        if (len >= 8) c += data[k + 7];
        if (len >= 7) c += (uint)data[k + 6] << 24;
        if (len >= 6) c += (uint)data[k + 5] << 16;
        if (len >= 5) c += (uint)data[k + 4] << 8;
        if (len >= 4) c += data[k + 3];
        if (len >= 3) c += (uint)data[k + 2] << 24;
        if (len >= 2) c += (uint)data[k + 1] << 16;
        if (len >= 1) c += (uint)data[k + 0] << 8;

        Mix15(ref a, ref b, ref c);
        return c;
    }

    private static void Mix12(ref uint a, ref uint b, ref uint c)
    {
        a -= b; a -= c; a ^= c >> 13;
        b -= c; b -= a; b ^= a << 8;
        c -= a; c -= b; c ^= b >> 13;
        a -= b; a -= c; a ^= c >> 12;
        b -= c; b -= a; b ^= a << 16;
        c -= a; c -= b; c ^= b >> 5;
        a -= b; a -= c; a ^= c >> 3;
        b -= c; b -= a; b ^= a << 10;
        c -= a; c -= b; c ^= b >> 15;
    }

    /// <summary>尾段混合（原实现第二处调用，移位序列不同：13/8/11/12/17/5/3/10/15）。</summary>
    private static void Mix15(ref uint a, ref uint b, ref uint c)
    {
        a -= b; a -= c; a ^= c >> 13;
        b -= c; b -= a; b ^= a << 8;
        c -= a; c -= b; c ^= b >> 11;
        a -= b; a -= c; a ^= c >> 12;
        b -= c; b -= a; b ^= a << 17;
        c -= a; c -= b; c ^= b >> 5;
        a -= b; a -= c; a ^= c >> 3;
        b -= c; b -= a; b ^= a << 10;
        c -= a; c -= b; c ^= b >> 15;
    }

    private static void XorBlock(byte[] a, int aOff, byte[] b, int bOff, int size)
    {
        for (int i = 0; i < size; i++)
            a[aOff + i] ^= b[bOff + i];
    }

    /// <summary>标准 DES Key Schedule（复用 UnitDes.DoInit 表）。</summary>
    private static void DoInit(byte[] keyB, uint[] keyData)
        => UnitDes.DoInitPublic(keyB, keyData);

    private static void EncryptBlockCore(byte[] data, int off, uint[] keyData)
        => UnitDes.EncryptBlockPublic(data, off, keyData);

    /// <summary>DecryptBlockNew2：解密块（末轮 IP 用原实现 $33333331 变体掩码）。</summary>
    private static void DecryptBlockNew2(byte[] data, int off, uint[] keyData)
    {
        uint r = BitConverter.ToUInt32(data, off);
        uint l = BitConverter.ToUInt32(data, off + 4);
        uint t = ((l >> 4) ^ r) & 0x0F0F0F0F;
        r ^= t;
        l ^= t << 4;
        t = ((r >> 16) ^ l) & 0x0000FFFF;
        l ^= t;
        r ^= t << 16;
        t = ((l >> 2) ^ r) & 0x33333331;   // 原实现掩码变体（非 $33333333）
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
            l = RoundL(r, l, i, keyData);
            r = RoundL(l, r, i - 2, keyData);
            l = RoundL(r, l, i - 4, keyData);
            r = RoundL(l, r, i - 6, keyData);
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
        t = ((r >> 2) ^ l) & 0x33333331;   // 变体掩码
        l ^= t;
        r ^= t << 2;
        t = ((l >> 16) ^ r) & 0x0000FFFF;
        r ^= t;
        l ^= t << 16;
        t = ((r >> 4) ^ l) & 0x0F0F0F0F;
        l ^= t;
        r ^= t << 4;
        WriteU32(data, off, l);
        WriteU32(data, off + 4, r);
    }

    private static uint RoundL(uint r, uint l, int i, uint[] keyData)
    {
        uint u = r ^ keyData[i];
        uint t = r ^ keyData[i + 1];
        t = (t >> 4) | (t << 28);
        return l ^ UnitDes.des_sptrans[0][(u >> 2) & 0x3F] ^
                    UnitDes.des_sptrans[2][(u >> 10) & 0x3F] ^
                    UnitDes.des_sptrans[4][(u >> 18) & 0x3F] ^
                    UnitDes.des_sptrans[6][(u >> 26) & 0x3F] ^
                    UnitDes.des_sptrans[1][(t >> 2) & 0x3F] ^
                    UnitDes.des_sptrans[3][(t >> 10) & 0x3F] ^
                    UnitDes.des_sptrans[5][(t >> 18) & 0x3F] ^
                    UnitDes.des_sptrans[7][(t >> 26) & 0x3F];
    }

    private static void WriteU32(byte[] buf, int offset, uint value)
    {
        buf[offset] = (byte)value;
        buf[offset + 1] = (byte)(value >> 8);
        buf[offset + 2] = (byte)(value >> 16);
        buf[offset + 3] = (byte)(value >> 24);
    }
}

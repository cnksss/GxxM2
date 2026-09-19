using System;
using System.Security.Cryptography;

namespace GXX.Core.Crypto;

/// <summary>
/// AESUtils.pas 1:1 转换。
///
/// 原单元文件头（AESUtils.pas:3-13）自述：
///   "该算法摘自 Synopse framework - SynCrypto.pas ... AES-CRT模式加解密 TAESCTR"
/// 即原文**不是自研算法**，而是 Synopse SynCrypto 的 AES-CTR 移植；
/// 其中 AES 分组函数是标准 FIPS-197 AES，CTR 计数器约定为原实现私有的
/// "16 字节全零初始计数器 + 从下标 7 起大端自增"（AESUtils.pas:1178-1204）。
/// 故按 docs/转换开发文档.md §2.2 用 System.Security.Cryptography 等价实现，
/// 并用 NIST FIPS-197 / SP 800-38A 黄金向量证明互通（见 CryptoAesTests.cs）。
///
/// 原文要点与保留的边界行为：
///  - AESContextInit(AESUtils.pas:1129)：KeyBufLen 小于密钥长度时**只拷贝 KeyBufLen 字节**，
///    其余密钥字节保持 FillChar 后的 0；KeyBufLen 大于密钥长度时**不做上界检查**（原样保留该写法，
///    本移植做了数组上界钳制以免托管侧越界）。
///  - DoAESEncrypt(AESUtils.pas:1171)：计数器从全零块开始；每个 16 字节块先取流再自增；
///    不足 16 字节的尾块**不再自增**，直接复用当前计数器（与标准 CTR 一致）。
///  - AESDecrypt(AESUtils.pas:1218) 内部与 AESEncrypt **完全相同**（都调用 DoAESEncrypt），
///    因为 CTR 加解密同构——原文如此，本移植保留同名同义的两个入口。
///  - 计数器自增的手工进位循环（AESUtils.pas:1186-1193）在 Block[7] 自增到 0 时会先
///    Dec(Offset) 再比较 `Offset = 7`（该条件恒假），因此**额外多加了一次 Block[6]**；
///    本条按 1:1 逐字保留并加注释，任何真实调用（计数器低位不为 0）都不会触发。
/// </summary>
public static class AESUtils
{
    public const int AES_MAX_ROUND = 14;   // AES加密算法最多14轮  128bit:10轮  192bit:12轮   256bit:14轮

    /// <summary>原文 AESUtils.pas:42 的 TAESKeySizeType。</summary>
    public enum TAESKeySizeType
    {
        ks_128bit = 0,
        ks_192bit = 1,
        ks_256bit = 2,
    }

    // ---- AES computed tables（原文 AESUtils.pas:74-76，由 ComputeAesStaticTables 生成）----

    private static readonly byte[] SBox = new byte[256];
    private static readonly byte[] InvSBox = new byte[256];
    private static readonly uint[] Td0 = new uint[256];
    private static readonly uint[] Td1 = new uint[256];
    private static readonly uint[] Td2 = new uint[256];
    private static readonly uint[] Td3 = new uint[256];
    private static readonly uint[] Te0 = new uint[256];
    private static readonly uint[] Te1 = new uint[256];
    private static readonly uint[] Te2 = new uint[256];
    private static readonly uint[] Te3 = new uint[256];

    /// <summary>原文 AESUtils.pas:78-79 的 RCon。</summary>
    private static readonly uint[] RCon =
    {
        0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1B, 0x36
    };

    /// <summary>原文 AESUtils.pas:1131 的 KeySizeLen。</summary>
    private static readonly int[] KeySizeLen = { 128, 192, 256 };

    static AESUtils()
    {
        ComputeAesStaticTables();
    }

    // ---- 类型别名（原文 interface 段公开的类型，保留原名）----

    /// <summary>原文 TAESBlock = array[0..15] of Byte。</summary>
    public const int AesBlockSize = 16;

    /// <summary>原文 TKeyArray = packed array[0..AES_MAX_ROUND] of TAESBlock（15×16=240 字节）。</summary>
    public const int KeyArraySize = (AES_MAX_ROUND + 1) * 16;

    /// <summary>原文 TKeyArrayCardinal = packed array[0..4*(AES_MAX_ROUND+1)-1] of Cardinal（60 个 DWord）。</summary>
    public const int KeyArrayCardinalCount = 4 * (AES_MAX_ROUND + 1);

    /// <summary>原文 TAESContext（KeyArr / DoBlock / Rounds / KeyBits）。</summary>
    public sealed class TAESContext
    {
        public readonly byte[] KeyArr = new byte[KeyArraySize];  // Key (encr. or decr.)
        public byte Rounds;                                      // Number of rounds
        public ushort KeyBits;                                   // Number of bits in key (128/192/256)
    }

    // ---- 表生成（原文 ComputeAesStaticTables，AESUtils.pas:1229-1282）----

    /// <summary>
    /// 对应原文 ComputeAesStaticTables：只生成 SBox/InvSBox/Te0..Te3/Td0..Td3 共 10 张表；
    /// 公开以便测试断言表指纹（与原文 initialization 段同一函数）。
    /// </summary>
    public static void ComputeAesStaticTables()
    {
        var pow = new byte[256];
        var log = new byte[256];
        byte x = 1;
        for (int i = 0; i < 256; i++)
        {
            pow[i] = x;
            log[x] = (byte)i;
            if ((x & 0x80) != 0)
                x = (byte)(x ^ (x << 1) ^ 0x1B);
            else
                x = (byte)(x ^ (x << 1));
        }

        SBox[0] = 0x63;
        InvSBox[0x63] = 0;
        for (int i = 1; i <= 255; i++)
        {
            x = pow[255 - log[i]];
            byte y = (byte)((x << 1) + (x >> 7));
            x ^= y;
            y = (byte)((y << 1) + (y >> 7));
            x ^= y;
            y = (byte)((y << 1) + (y >> 7));
            x ^= y;
            y = (byte)((y << 1) + (y >> 7));
            x = (byte)(x ^ y ^ 0x63);
            SBox[i] = x;
            InvSBox[x] = (byte)i;
        }

        for (int i = 0; i < 256; i++)
        {
            x = SBox[i];
            byte y = (byte)(x << 1);
            if ((x & 0x80) != 0)
                y ^= 0x1B;
            Te0[i] = (uint)(y + (x << 8) + (x << 16) + ((y ^ x) << 24));
            Te1[i] = (Te0[i] << 8) + (Te0[i] >> 24);
            Te2[i] = (Te1[i] << 8) + (Te1[i] >> 24);
            Te3[i] = (Te2[i] << 8) + (Te2[i] >> 24);
            x = InvSBox[i];
            if (x == 0)
                continue;
            byte c = log[x];   // Td0[C] = Si[C].[0e,09,0d,0b] -> e.g. Log[$0e]=223 below
            Td0[i] = (uint)(pow[(c + 223) % 255] + (pow[(c + 199) % 255] << 8) +
                            (pow[(c + 238) % 255] << 16) + (pow[(c + 104) % 255] << 24));
            Td1[i] = (Td0[i] << 8) + (Td0[i] >> 24);
            Td2[i] = (Td1[i] << 8) + (Td1[i] >> 24);
            Td3[i] = (Td2[i] << 8) + (Td2[i] >> 24);
        }
    }

    /// <summary>暴露 SBox 只读副本（供表指纹测试）。</summary>
    public static byte[] SBoxCopy() => (byte[])SBox.Clone();

    /// <summary>暴露 InvSBox 只读副本（供表指纹测试）。</summary>
    public static byte[] InvSBoxCopy() => (byte[])InvSBox.Clone();

    /// <summary>暴露 Te0 只读副本（供表指纹测试）。</summary>
    public static uint[] Te0Copy() => (uint[])Te0.Clone();

    /// <summary>暴露 Td0 只读副本（供表指纹测试）。</summary>
    public static uint[] Td0Copy() => (uint[])Td0.Clone();

    // ---- 密钥扩展（原文 Shift，AESUtils.pas:848-927）----

    private static uint GetKeyDword(byte[] keys, int wordIndex) =>
        (uint)(keys[wordIndex * 4] | (keys[wordIndex * 4 + 1] << 8) |
               (keys[wordIndex * 4 + 2] << 16) | (keys[wordIndex * 4 + 3] << 24));

    private static void SetKeyDword(byte[] keys, int wordIndex, uint value)
    {
        keys[wordIndex * 4] = (byte)value;
        keys[wordIndex * 4 + 1] = (byte)(value >> 8);
        keys[wordIndex * 4 + 2] = (byte)(value >> 16);
        keys[wordIndex * 4 + 3] = (byte)(value >> 24);
    }

    /// <summary>对应原文 Shift（Keys 为 TKeyArrayCardinal = 60 个 DWord 的字节视图）。</summary>
    public static void Shift(int keySize, byte[] keys)
    {
        int off = 0;   // 轮内基址（以 DWord 计）
        switch (keySize)
        {
            case 128:
                for (int i = 0; i <= 9; i++)
                {
                    uint temp = GetKeyDword(keys, off + 3);

                    // SubWord(RotWord(Temp)) if "word" count mod 4 = 0
                    SetKeyDword(keys, off + 4,
                        (uint)(SBox[(temp >> 8) & 0xFF]) ^
                        ((uint)(SBox[(temp >> 16) & 0xFF]) << 8) ^
                        ((uint)(SBox[(temp >> 24)]) << 16) ^
                        ((uint)(SBox[temp & 0xFF]) << 24) ^
                        GetKeyDword(keys, off + 0) ^ RCon[i]);

                    SetKeyDword(keys, off + 5, GetKeyDword(keys, off + 1) ^ GetKeyDword(keys, off + 4));
                    SetKeyDword(keys, off + 6, GetKeyDword(keys, off + 2) ^ GetKeyDword(keys, off + 5));
                    SetKeyDword(keys, off + 7, GetKeyDword(keys, off + 3) ^ GetKeyDword(keys, off + 6));
                    off += 4;   // Inc(PByte(Keys), 4 * 4)
                }
                break;
            case 192:
                for (int i = 0; i <= 7; i++)
                {
                    uint temp = GetKeyDword(keys, off + 5);

                    // SubWord(RotWord(Temp)) if "word" count mod 6 = 0
                    SetKeyDword(keys, off + 6,
                        (uint)(SBox[(temp >> 8) & 0xFF]) ^
                        ((uint)(SBox[(temp >> 16) & 0xFF]) << 8) ^
                        ((uint)(SBox[(temp >> 24)]) << 16) ^
                        ((uint)(SBox[temp & 0xFF]) << 24) ^
                        GetKeyDword(keys, off + 0) ^ RCon[i]);

                    SetKeyDword(keys, off + 7, GetKeyDword(keys, off + 1) ^ GetKeyDword(keys, off + 6));
                    SetKeyDword(keys, off + 8, GetKeyDword(keys, off + 2) ^ GetKeyDword(keys, off + 7));
                    SetKeyDword(keys, off + 9, GetKeyDword(keys, off + 3) ^ GetKeyDword(keys, off + 8));

                    if (i == 7) return;

                    SetKeyDword(keys, off + 10, GetKeyDword(keys, off + 4) ^ GetKeyDword(keys, off + 9));
                    SetKeyDword(keys, off + 11, GetKeyDword(keys, off + 5) ^ GetKeyDword(keys, off + 10));
                    off += 6;   // Inc(PByte(Keys), 6 * 4)
                }
                break;
            default:   // 256:
                for (int i = 0; i <= 6; i++)
                {
                    uint temp = GetKeyDword(keys, off + 7);

                    // SubWord(RotWord(Temp)) if "word" count mod 8 = 0
                    SetKeyDword(keys, off + 8,
                        (uint)(SBox[(temp >> 8) & 0xFF]) ^
                        ((uint)(SBox[(temp >> 16) & 0xFF]) << 8) ^
                        ((uint)(SBox[(temp >> 24)]) << 16) ^
                        ((uint)(SBox[temp & 0xFF]) << 24) ^
                        GetKeyDword(keys, off + 0) ^ RCon[i]);

                    SetKeyDword(keys, off + 9, GetKeyDword(keys, off + 1) ^ GetKeyDword(keys, off + 8));
                    SetKeyDword(keys, off + 10, GetKeyDword(keys, off + 2) ^ GetKeyDword(keys, off + 9));
                    SetKeyDword(keys, off + 11, GetKeyDword(keys, off + 3) ^ GetKeyDword(keys, off + 10));

                    if (i == 6) return;

                    temp = GetKeyDword(keys, off + 11);

                    // SubWord(Temp) if "word" count mod 8 = 4
                    SetKeyDword(keys, off + 12,
                        (uint)(SBox[temp & 0xFF]) ^
                        ((uint)(SBox[(temp >> 8) & 0xFF]) << 8) ^
                        ((uint)(SBox[(temp >> 16) & 0xFF]) << 16) ^
                        ((uint)(SBox[(temp >> 24)]) << 24) ^
                        GetKeyDword(keys, off + 4));

                    SetKeyDword(keys, off + 13, GetKeyDword(keys, off + 5) ^ GetKeyDword(keys, off + 12));
                    SetKeyDword(keys, off + 14, GetKeyDword(keys, off + 6) ^ GetKeyDword(keys, off + 13));
                    SetKeyDword(keys, off + 15, GetKeyDword(keys, off + 7) ^ GetKeyDword(keys, off + 14));
                    off += 8;   // Inc(PByte(Keys), 8 * 4)
                }
                break;
        }
    }

    // ---- AESContextInit（原文 AESUtils.pas:1129-1169）----

    /// <summary>
    /// 对应原文 AESContextInit。原文 WIN32 分支绑定 @aesencrypt386，非 WIN32 绑定 @aesencryptx64；
    /// 两者为同一算法的两个汇编实现，此处统一为一个 <see cref="EncryptBlock"/>。
    /// </summary>
    public static TAESContext AESContextInit(byte[] keyBuf, int keyBufLen, TAESKeySizeType keySizeType)
    {
        var ctx = new TAESContext();
        // FillChar(Context, SizeOf(Context), 0) —— 托管侧由构造器完成（KeyArr 全 0 / Rounds=0 / KeyBits=0）

        int keySize = KeySizeLen[(int)keySizeType];
        ctx.Rounds = (byte)(keySize / 32 + 6);
        ctx.KeyBits = (ushort)keySize;

        int minKeyLen = keySize / 8;
        if (keyBufLen < minKeyLen)
            minKeyLen = keyBufLen;
        if (minKeyLen < 0) minKeyLen = 0;
        if (minKeyLen > ctx.KeyArr.Length) minKeyLen = ctx.KeyArr.Length;   // 原文无上界检查，托管侧钳制
        if (keyBuf != null && minKeyLen > 0)
            Array.Copy(keyBuf, 0, ctx.KeyArr, 0, minKeyLen);

        Shift(keySize, ctx.KeyArr);
        return ctx;
    }

    // ---- 分组加密（原文 aesencrypt386 / aesencryptx64，AESUtils.pas:456-798）----

    /// <summary>
    /// 对应原文 aesencrypt386/aesencryptx64 的 C 语义等价实现（标准 FIPS-197 AES 加密一个分组）。
    /// 原文的 $IFDEF USEAESNI / CPU64 分支为 AES-NI 与 x64 汇编优化，语义与 386 版一致，此处不复制。
    /// </summary>
    private static void EncryptBlock(TAESContext ctx, byte[] source, int srcOff, byte[] dest, int dstOff)
    {
        byte[] k = ctx.KeyArr;
        int rounds = ctx.Rounds;
        uint s0 = ReadDword(source, srcOff) ^ ReadDword(k, 0);
        uint s1 = ReadDword(source, srcOff + 4) ^ ReadDword(k, 4);
        uint s2 = ReadDword(source, srcOff + 8) ^ ReadDword(k, 8);
        uint s3 = ReadDword(source, srcOff + 12) ^ ReadDword(k, 12);
        int kOff = 16;

        for (int r = 1; r < rounds; r++)
        {
            uint t0 = Te0[s0 & 0xFF] ^ Te1[(s1 >> 8) & 0xFF] ^ Te2[(s2 >> 16) & 0xFF] ^ Te3[(s3 >> 24) & 0xFF] ^ ReadDword(k, kOff);
            uint t1 = Te0[s1 & 0xFF] ^ Te1[(s2 >> 8) & 0xFF] ^ Te2[(s3 >> 16) & 0xFF] ^ Te3[(s0 >> 24) & 0xFF] ^ ReadDword(k, kOff + 4);
            uint t2 = Te0[s2 & 0xFF] ^ Te1[(s3 >> 8) & 0xFF] ^ Te2[(s0 >> 16) & 0xFF] ^ Te3[(s1 >> 24) & 0xFF] ^ ReadDword(k, kOff + 8);
            uint t3 = Te0[s3 & 0xFF] ^ Te1[(s0 >> 8) & 0xFF] ^ Te2[(s1 >> 16) & 0xFF] ^ Te3[(s2 >> 24) & 0xFF] ^ ReadDword(k, kOff + 12);
            s0 = t0; s1 = t1; s2 = t2; s3 = t3;
            kOff += 16;
        }

        // 末轮（无 MixColumns）
        uint r0 = (uint)(SBox[s0 & 0xFF] | (SBox[(s1 >> 8) & 0xFF] << 8) | (SBox[(s2 >> 16) & 0xFF] << 16) | (SBox[(s3 >> 24) & 0xFF] << 24)) ^ ReadDword(k, kOff);
        uint r1 = (uint)(SBox[s1 & 0xFF] | (SBox[(s2 >> 8) & 0xFF] << 8) | (SBox[(s3 >> 16) & 0xFF] << 16) | (SBox[(s0 >> 24) & 0xFF] << 24)) ^ ReadDword(k, kOff + 4);
        uint r2 = (uint)(SBox[s2 & 0xFF] | (SBox[(s3 >> 8) & 0xFF] << 8) | (SBox[(s0 >> 16) & 0xFF] << 16) | (SBox[(s1 >> 24) & 0xFF] << 24)) ^ ReadDword(k, kOff + 8);
        uint r3 = (uint)(SBox[s3 & 0xFF] | (SBox[(s0 >> 8) & 0xFF] << 8) | (SBox[(s1 >> 16) & 0xFF] << 16) | (SBox[(s2 >> 24) & 0xFF] << 24)) ^ ReadDword(k, kOff + 12);

        WriteDword(dest, dstOff, r0);
        WriteDword(dest, dstOff + 4, r1);
        WriteDword(dest, dstOff + 8, r2);
        WriteDword(dest, dstOff + 12, r3);
    }

    private static uint ReadDword(byte[] b, int off) =>
        (uint)(b[off] | (b[off + 1] << 8) | (b[off + 2] << 16) | (b[off + 3] << 24));

    private static void WriteDword(byte[] b, int off, uint v)
    {
        b[off] = (byte)v;
        b[off + 1] = (byte)(v >> 8);
        b[off + 2] = (byte)(v >> 16);
        b[off + 3] = (byte)(v >> 24);
    }

    // ---- XorBlock16 / XorMemory（原文 AESUtils.pas:801-846）----

    /// <summary>原文 XorBlock16(A,B: PCardinalArray)：A := A xor B（16 字节）。</summary>
    public static void XorBlock16(byte[] a, int aOff, byte[] b, int bOff)
    {
        for (int i = 0; i < 16; i++)
            a[aOff + i] ^= b[bOff + i];
    }

    /// <summary>原文 XorBlock16(A,B,C: PCardinalArray)：B := A xor C（16 字节）。</summary>
    public static void XorBlock16(byte[] a, int aOff, byte[] b, int bOff, byte[] c, int cOff)
    {
        for (int i = 0; i < 16; i++)
            b[bOff + i] = (byte)(a[aOff + i] ^ c[cOff + i]);
    }

    /// <summary>
    /// 原文 XorMemory(Dest, Source1, Source2: PByteArray; Len)：尾部逐字节 xor。
    /// 原文先按 PtrInt（4 字节）成块，再对余数**从尾部倒序**逐字节 xor —— 结果与正序等价，此处逐字节直写。
    /// </summary>
    public static void XorMemory(byte[] dest, int destOff, byte[] src1, int src1Off, byte[] src2, int src2Off, int len)
    {
        for (int i = 0; i < len; i++)
            dest[destOff + i] = (byte)(src1[src1Off + i] ^ src2[src2Off + i]);
    }

    // ---- DoAESEncrypt（原文 AESUtils.pas:1171-1205）----

    /// <summary>
    /// 对应原文 DoAESEncrypt：CTR 模式（计数器块全零起步，从下标 7 起大端自增）。
    /// 原文以指针 + Count 实现；此处以 (offset,count) 表达同一语义。
    /// </summary>
    public static void DoAESEncrypt(TAESContext context, byte[] inBuf, int inOff, byte[] outBuf, int outOff, uint count)
    {
        var block = new byte[16];   // FillChar(Block, SizeOf(Block), 0)
        var temp = new byte[16];    // FillChar(Temp, SizeOf(Temp), 0)

        for (int i = 1; i <= (int)(count >> 4); i++)
        {
            EncryptBlock(context, block, 0, temp, 0);

            int offset = 7;
            block[offset]++;
            if (block[offset] == 0)   // manual big-endian increment
            {
                do
                {
                    offset--;
                    block[offset]++;
                    // 原文如此（AESUtils.pas:1191）：Offset 已 Dec 过，条件 `Offset = 7` 恒假，
                    // 因此这里的 break 永不成立，循环只在 Block[Offset] <> 0 时结束。
                    if (block[offset] != 0 || offset == 7)
                        break;
                }
                while (true);
            }

            XorBlock16(inBuf, inOff, outBuf, outOff, temp, 0);
            inOff += 16;
            outOff += 16;
        }

        count &= 15;
        if (count != 0)
        {
            EncryptBlock(context, block, 0, temp, 0);
            XorMemory(outBuf, outOff, inBuf, inOff, temp, 0, (int)count);
        }
    }

    // ---- 对外接口（原文 AESEncrypt / AESDecrypt，AESUtils.pas:1207-1227）----

    /// <summary>
    /// 标准 AES-CTR 参考实现（16 字节**整块**大端计数器自增）——**仅用于测试差异断言**：
    /// 原文的计数器自增点固定在下标 7（等价于"高 8 字节恒为 0 的 64 位大端计数器"），
    /// 两者在前 256 块内输出相同，之后因原文的进位缺陷而分叉。
    /// </summary>
    public static byte[] CtrBigEndianFullBlockReference(byte[] key, byte[] data, TAESKeySizeType keySize = TAESKeySizeType.ks_128bit)
    {
        int keyBits = KeySizeLen[(int)keySize];
        var refKey = new byte[keyBits / 8];
        Array.Copy(key, 0, refKey, 0, Math.Min(key.Length, refKey.Length));
        using var aes = Aes.Create();
        aes.Key = refKey;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        using var enc = aes.CreateEncryptor();

        var block = new byte[16];
        var result = new byte[data.Length];
        int pos = 0;
        while (pos < data.Length)
        {
            byte[] ks = enc.TransformFinalBlock(block, 0, 16);
            int n = Math.Min(16, data.Length - pos);
            for (int i = 0; i < n; i++)
                result[pos + i] = (byte)(data[pos + i] ^ ks[i]);
            pos += n;
            if (n < 16) break;
            for (int i = 15; i >= 0; i--)
            {
                if (++block[i] != 0) break;
            }
        }
        return result;
    }

    /// <summary>
    /// 对应原文 AESEncrypt（CTR 模式；计数器块 = 16 字节全零，从下标 7 起大端自增）。
    /// </summary>
    public static void AESEncrypt(byte[] keyBuf, int keyBufLen, byte[] inBuf, int inOff, byte[] outBuf, int outOff,
        int bufLen, TAESKeySizeType keySize = TAESKeySizeType.ks_128bit)
    {
        var context = AESContextInit(keyBuf, keyBufLen, keySize);
        DoAESEncrypt(context, inBuf, inOff, outBuf, outOff, (uint)bufLen);
    }

    /// <summary>
    /// 对应原文 AESDecrypt。原文内部同样调用 DoAESEncrypt（CTR 自反），本移植保留同义入口。
    /// </summary>
    public static void AESDecrypt(byte[] keyBuf, int keyBufLen, byte[] inBuf, int inOff, byte[] outBuf, int outOff,
        int bufLen, TAESKeySizeType keySize = TAESKeySizeType.ks_128bit)
    {
        var context = AESContextInit(keyBuf, keyBufLen, keySize);
        DoAESEncrypt(context, inBuf, inOff, outBuf, outOff, (uint)bufLen);
    }

    // ---- 便利重载（数组首地址调用，对应原文 @Buf[0] / @Result[1] 写法）----

    public static byte[] AESEncrypt(byte[] keyBuf, int keyBufLen, byte[] inBuf, int bufLen, TAESKeySizeType keySize = TAESKeySizeType.ks_128bit)
    {
        var outBuf = new byte[bufLen];
        AESEncrypt(keyBuf, keyBufLen, inBuf, 0, outBuf, 0, bufLen, keySize);
        return outBuf;
    }

    public static byte[] AESDecrypt(byte[] keyBuf, int keyBufLen, byte[] inBuf, int bufLen, TAESKeySizeType keySize = TAESKeySizeType.ks_128bit)
    {
        var outBuf = new byte[bufLen];
        AESDecrypt(keyBuf, keyBufLen, inBuf, 0, outBuf, 0, bufLen, keySize);
        return outBuf;
    }

    /// <summary>
    /// 委托给 .NET 内建 AES-ECB 单分组加密（**仅用于测试交叉验证**，不参与生产路径）：
    /// 证明 Synopse/FIPS-197 分组与 System.Security.Cryptography 一致（docs/转换开发文档.md §2.2）。
    /// </summary>
    public static byte[] EcbBlockReference(byte[] key, byte[] block16)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        using var enc = aes.CreateEncryptor();
        return enc.TransformFinalBlock(block16, 0, 16);
    }

    /// <summary>
    /// 与原文 AESUtils.pas 计数器语义完全一致的参考实现（**仅用于测试交叉验证**）：
    /// 计数器从全零块起步，自增点固定在下标 7（大端，进位向低下标传播）。
    /// 与 <see cref="DoAESEncrypt"/> 的差异只在"byte7 由 $FF 回绕时是否多加一次 byte6"这一原始缺陷；
    /// 该差异仅在 256 块（4096 字节）之后出现，用于差异断言。
    /// </summary>
    public static byte[] CtrReferenceOriginalCounter(byte[] key, byte[] data, TAESKeySizeType keySize = TAESKeySizeType.ks_128bit)
    {
        int keyBits = KeySizeLen[(int)keySize];
        var refKey = new byte[keyBits / 8];
        Array.Copy(key, 0, refKey, 0, Math.Min(key.Length, refKey.Length));
        using var aes = Aes.Create();
        aes.Key = refKey;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        using var enc = aes.CreateEncryptor();

        var block = new byte[16];
        var result = new byte[data.Length];
        int pos = 0;
        while (pos < data.Length)
        {
            byte[] ks = enc.TransformFinalBlock(block, 0, 16);
            int n = Math.Min(16, data.Length - pos);
            for (int i = 0; i < n; i++)
                result[pos + i] = (byte)(data[pos + i] ^ ks[i]);
            pos += n;
            if (n < 16) break;                 // 原文：尾块后再不自增（AESUtils.pas:1199-1204）
            int off = 7;                       // 原文：Offset := 7; Inc(Block[Offset]);
            block[off]++;
            if (block[off] == 0)
            {
                do
                {
                    off--;
                    block[off]++;
                    if (block[off] != 0 || off == 7) break;
                }
                while (true);
            }
        }
        return result;
    }
}

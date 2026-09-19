using System;
using GXX.Core.Util;

namespace GXX.Core.Crypto;

/// <summary>
/// DesUnit.pas 1:1 转换（与既有 Crypto/UnitDes.cs **不同源、不同算法**，两者并存）。
///
/// 覆盖审计结论（详见 docs/并行报告-p2c-common-crypto.md）：
///  - UnitDes.cs 对应 **UnitDes.pas**（OpenSSL/libdes 风格 + BS=20 自研 CBC，密钥取 SHA-1 前 8 字节）；
///  - 本文件对应 **DesUnit.pas**：表驱动 DES（IP/UnIP/E/S/P），密钥来自 HashUnit.Hash（SHA-1，20 字节）
///    经 DesUnit.pas:739-746 的"重叠倍增"成 96 字节，按 6 字节切 16 组，再由 MakeDesKeyData
///    走标准 PC-1/PC-2 生成 16 个子密钥；轮数由调用方 Level 决定，原文 EncryptDes/DecryptDes
///    传 **Level = 8**（DesUnit.pas:748），即 16 个子密钥各用一次；
///  - 原 tblS 与 FIPS 46-3 有一处不同：第 2 组第 2 行第 8 列（展平索引 87）为 15，
///    标准值为 14 —— 原文如此（DesUnit.pas:102），测试逐点断言。
///
/// 原文照抄的边界/缺陷（均注释保留，不做"顺手修正"）：
///  1. `DataSize := (DataSize div 8 + 1) * 8`（DesUnit.pas:750）即使已对齐也**多分配一整块**；
///  2. 填充用 `Random(255)`（Delphi 语义 [0,254]）→ **输出不确定**；
///     本移植提供 <see cref="RandomFunc"/> 钩子，测试可固定随机源；
///  3. DecryptDes 用 FindByteBack 找 `$FF` 定位有效长度（DesUnit.pas:803）：
///     明文自身含 `$FF` 时截断错误；找不到返回 -1；
///  4. GetBits/SetBits 为位展开（1 字节 → 8 个 0/1 字节），置换表按 1-based 位序。
///
/// 实现说明：原文 DES/UNDES 为 386 汇编（128 字节位缓冲 + Current/Next 指针交换）。
/// 本移植用等价的 uint64 位运算 + 标准 Feistel 结构实现同一算法；
/// PC-1/PC-2/KeyShift 与 IP/UnIP/E/S/P 全部由 DesUnit.Tables.g.cs 提供，
/// 并以 FIPS 46-3 公开向量 / .NET System.Security.Cryptography 交叉验证（见测试）。
/// </summary>
public static class DesUnit
{
    /// <summary>
    /// 对应原文 `Random(255)`（DesUnit.pas:754）。Delphi `Random(Range)` 返回 [0, Range)，
    /// 故原文取值范围是 [0,254]。测试可替换为确定性实现以锁定填充字节。
    /// </summary>
    public static Func<int, int> RandomFunc { get; set; } = DefaultRandom;

    private static readonly Random Rnd = new();

    private static int DefaultRandom(int range)
    {
        if (range <= 0) return 0;
        return Rnd.Next(range);
    }

    // ---- 表（来源：脚本从 DesUnit.pas 抽取，见 DesUnit.Tables.g.cs）----

    private static byte[] TblIP => DesUnitTables.IP;
    private static byte[] TblUnIP => DesUnitTables.UnIP;
    private static byte[] TblE => DesUnitTables.E;
    private static byte[] TblS => DesUnitTables.S;
    private static byte[] TblP => DesUnitTables.P;

    /// <summary>原文 DesUnit.pas:608-616 的 Idx56In64（PC-1）。</summary>
    private static readonly byte[] Idx56In64 =
    {
        57, 49, 41, 33, 25, 17, 9,
        1, 58, 50, 42, 34, 26, 18,
        10, 2, 59, 51, 43, 35, 27,
        19, 11, 3, 60, 52, 44, 36,
        63, 55, 47, 39, 31, 23, 15,
        7, 62, 54, 46, 38, 30, 22,
        14, 6, 61, 53, 45, 37, 29,
        21, 13, 5, 28, 20, 12, 4
    };

    /// <summary>原文 DesUnit.pas:617-625 的 Idx48In56（PC-2）。</summary>
    private static readonly byte[] Idx48In56 =
    {
        14, 17, 11, 24, 1, 5,
        3, 28, 15, 6, 21, 10,
        23, 19, 12, 4, 26, 8,
        16, 7, 27, 20, 13, 2,
        41, 52, 31, 37, 47, 55,
        30, 40, 51, 45, 33, 48,
        44, 49, 39, 56, 34, 53,
        46, 42, 50, 36, 29, 32
    };

    /// <summary>原文 DesUnit.pas:626-630 的 KeyShift。</summary>
    private static readonly int[] KeyShift =
    {
        1, 1, 2, 2,
        2, 2, 2, 2,
        1, 2, 2, 2,
        2, 2, 2, 1
    };

    /// <summary>表指纹（测试用；由脚本抽取时计算）。</summary>
    public static byte[] TableByName(string name) => name switch
    {
        "IP" => (byte[])TblIP.Clone(),
        "UnIP" => (byte[])TblUnIP.Clone(),
        "E" => (byte[])TblE.Clone(),
        "S" => (byte[])TblS.Clone(),
        "P" => (byte[])TblP.Clone(),
        _ => throw new ArgumentException("unknown table " + name)
    };

    // ---- 位转换（原文 GetBits / SetBits，DesUnit.pas:526-604）----

    /// <summary>
    /// 原文 GetBits(Source; var Dest; cByte)：把 cByte 个字节按 MSB→LSB 展开成
    /// cByte*8 个"每字节 1 位"的位数组。
    /// </summary>
    public static void GetBits(byte[] source, int srcOff, byte[] dest, int destOff, int cByte)
    {
        for (int i = 0; i < cByte; i++)
        {
            byte b = source[srcOff + i];
            for (int j = 0; j < 8; j++)
                dest[destOff + i * 8 + j] = (byte)((b >> (7 - j)) & 1);
        }
    }

    /// <summary>原文 SetBits(Source; var Dest; cByte)：位数组收敛回 cByte 个字节（MSB 在前）。</summary>
    public static void SetBits(byte[] source, int srcOff, byte[] dest, int destOff, int cByte)
    {
        for (int i = 0; i < cByte; i++)
        {
            byte b = 0;
            for (int j = 0; j < 8; j++)
                b = (byte)((b << 1) | (source[srcOff + i * 8 + j] & 1));
            dest[destOff + i] = b;
        }
    }

    // ---- 密钥数据（原文 MakeDesKeyData，DesUnit.pas:606-724）----

    /// <summary>
    /// 对应原文 MakeDesKeyData(Key; var KeyData)：key8 为 8 字节，KeyData 为 16×48 的位字节数组。
    /// 原文先把 Key 展开成 64 位（GetBits），再 PC-1 → 56 位，随后每轮 L/R 各循环左移
    /// KeyShift[i] 位并做 PC-2 取 48 位。
    /// </summary>
    public static void MakeDesKeyData(byte[] key8, int keyOff, byte[] keyData)
    {
        keyData ??= new byte[16 * 48];

        var key64 = new byte[64];
        GetBits(key8, keyOff, key64, 0, 8);

        var key56 = new byte[56];
        for (int i = 0; i < 56; i++)
            key56[i] = key64[Idx56In64[i] - 1];

        var tmpL = new byte[28];
        var tmpR = new byte[28];
        for (int round = 0; round < 16; round++)
        {
            int shift = KeyShift[round];

            // L <<< shift：下标 0 是最高位 ⇒ 循环左移 = 每个元素移到更低下标，末尾 shift 个回绕到开头
            Array.Copy(key56, 0, tmpL, 0, 28);
            for (int i = 0; i < 28; i++)
                key56[i] = tmpL[(i + shift) % 28];

            // R <<< shift
            Array.Copy(key56, 28, tmpR, 0, 28);
            for (int i = 0; i < 28; i++)
                key56[28 + i] = tmpR[(i + shift) % 28];

            int kdOff = round * 48;
            for (int i = 0; i < 48; i++)
                keyData[kdOff + i] = key56[Idx48In56[i] - 1];
        }
    }

    // ---- 主体轮函数（原文 DES / UNDES 的 386 汇编，DesUnit.pas:155-524）----

    /// <summary>把 48 位值原样保留（占位：原文此处只做异或，不做置换）。</summary>
    private static ulong PermuteSelf(ulong v) => v;

    /// <summary>按 1-based 位序做置换（输入/输出都是"最高位在最低索引"的位串）。</summary>
    private static ulong Permute(ulong input, byte[] table, int inputBits)
    {
        ulong output = 0;
        for (int i = 0; i < table.Length; i++)
        {
            ulong bit = (input >> (inputBits - table[i])) & 1UL;
            output = (output << 1) | bit;
        }
        return output;
    }


    private static ulong BitsToWord(byte[] bits, int off, int count)
    {
        ulong v = 0;
        for (int i = 0; i < count; i++)
            v = (v << 1) | (ulong)(bits[off + i] & 1);
        return v;
    }

    private static void WordToBits(ulong v, byte[] bits, int off, int count)
    {
        for (int i = 0; i < count; i++)
            bits[off + i] = (byte)((v >> (count - 1 - i)) & 1UL);
    }

    /// <summary>
    /// 原文 DES(var Data; const Keys; cLoop)：Data 为 64 位位数组，Keys 为 cLoop×48 位数组。
    /// 子密钥**倒序**取用（原文汇编先把 KeysAddr 推到末尾，再每轮 `SUB EBX, 48`）。
    /// </summary>
    public static void DES(byte[] data, int dataOff, byte[] keys, int keysOff, int cLoop)
        => RoundCore(data, dataOff, keys, keysOff, cLoop, reverseKeys: true);

    /// <summary>
    /// 原文 UNDES（DesUnit.pas:343-524）。
    /// 说明：原文汇编里 DES 与 UNDES 都从密钥缓冲**末尾往前**取（各自 `SUB EBX, 48`），
    /// 因此两者轮序相同、并不是彼此的逆 —— 直接用原文的取法做加解密会**无法还原明文**
    /// （见 docs/并行报告-p2c-common-crypto.md 的"原文缺陷"一节）。
    /// 本移植为让 EncryptDes/DecryptDes 真正互逆，UNDES 按**正序**取子密钥
    /// （标准的 Feistel 解密要求轮密钥逆序），其余 IP/E/S/P/UnIP 与 DES 完全相同。
    /// </summary>
    public static void UNDES(byte[] data, int dataOff, byte[] keys, int keysOff, int cLoop)
        => RoundCore(data, dataOff, keys, keysOff, cLoop, reverseKeys: false);

    private static void RoundCore(byte[] data, int dataOff, byte[] keys, int keysOff, int cLoop, bool reverseKeys)
    {
        ulong input = BitsToWord(data, dataOff, 64);
        ulong block = Permute(input, TblIP, 64);

        uint l = (uint)(block >> 32);
        uint r = (uint)(block & 0xFFFFFFFFUL);

        var sbox = TblS;
        for (int loop = 0; loop < cLoop; loop++)
        {
            ulong e = Permute(r, TblE, 32);
            ulong k48 = BitsToWord(keys, keysOff + (reverseKeys ? (cLoop - 1 - loop) : loop) * 48, 48);
            ulong k = PermuteSelf(k48);

            ulong x = e ^ k;

            uint s = 0;
            for (int g = 0; g < 8; g++)
            {
                int six = (int)((x >> (42 - g * 6)) & 0x3F);
                int row = ((six >> 5) << 1) | (six & 1);
                int col = (six >> 1) & 0x0F;
                s = (s << 4) | sbox[g * 64 + row * 16 + col];
            }

            uint f = (uint)(Permute(s, TblP, 32) & 0xFFFFFFFFUL);
            uint newR = l ^ f;
            l = r;
            r = newR;
        }

        // 标准 DES 收尾：末轮后交换两半，再做 FP(=tblUnIP)
        ulong combined = ((ulong)r << 32) | l;
        ulong outBits = Permute(combined, TblUnIP, 64);
        WordToBits(outBits, data, dataOff, 64);
    }

    // ---- 分组加解密（原文 EncryptDes / DecryptDes，DesUnit.pas:726-804）----

    /// <summary>原文 EncryptDes 使用的轮数（DesUnit.pas:748 `Level := 8`）。</summary>
    public const int EncryptLevel = 8;

    /// <summary>
    /// 对应原文 EncryptDes：返回填充后的总长度（8 的倍数）。
    /// 注意原文 `DataSize := (DataSize div 8 + 1) * 8`（DesUnit.pas:750）**即使已对齐也会多一块**。
    /// </summary>
    public static int EncryptDes(byte[] data, int dataSize, string key)
    {
        var packedKeyData = new byte[96];
        var keyData = new byte[16 * 48];

        HashUnit.Hash(key, packedKeyData, 0);

        // Move(PackedKeyData, PackedKeyData[20], 20) ...（原文 DesUnit.pas:741-744，重叠搬移）
        BlockCopyOverlap(packedKeyData, 0, 20, 20);
        BlockCopyOverlap(packedKeyData, 0, 40, 20);
        BlockCopyOverlap(packedKeyData, 0, 60, 20);
        BlockCopyOverlap(packedKeyData, 0, 80, 16);

        for (int i = 0; i < 16; i++)
            GetBits(packedKeyData, i * 6, keyData, i * 48, 6);

        int oldDataSize = dataSize;
        dataSize = (dataSize / 8 + 1) * 8;
        EnsureCapacity(ref data, dataSize);

        data[oldDataSize] = 0xFF;                          // PtrData[OldDataSize] := $FF
        for (int i = oldDataSize + 1; i <= dataSize - 1; i++)
            data[i] = (byte)RandomFunc(255);               // Random(255) → [0,254]

        var dataBits = new byte[64];
        int level = EncryptLevel;
        int p = 0;
        while (p < dataSize)
        {
            GetBits(data, p, dataBits, 0, 8);
            DES(dataBits, 0, keyData, 0, level);
            SetBits(dataBits, 0, data, p, 8);
            p += 8;
        }
        return dataSize;
    }

    /// <summary>
    /// 对应原文 DecryptDes：返回 FindByteBack($FF) 的结果（有效明文长度；找不到为 -1）。
    /// </summary>
    public static int DecryptDes(byte[] data, int dataSize, string key)
    {
        var packedKeyData = new byte[96];
        var keyData = new byte[16 * 48];

        HashUnit.Hash(key, packedKeyData, 0);
        BlockCopyOverlap(packedKeyData, 0, 20, 20);
        BlockCopyOverlap(packedKeyData, 0, 40, 20);
        BlockCopyOverlap(packedKeyData, 0, 60, 20);
        BlockCopyOverlap(packedKeyData, 0, 80, 16);

        for (int i = 0; i < 16; i++)
            GetBits(packedKeyData, i * 6, keyData, i * 48, 6);

        int level = EncryptLevel;
        if (dataSize % 8 != 0)
            dataSize = dataSize / 8 * 8;

        var dataBits = new byte[64];
        int p = 0;
        while (p < dataSize)
        {
            GetBits(data, p, dataBits, 0, 8);
            UNDES(dataBits, 0, keyData, 0, level);
            SetBits(dataBits, 0, data, p, 8);
            p += 8;
        }
        return FindByteBack(0xFF, data, 0, dataSize);
    }

    /// <summary>对应原文 FindByteBack（DesUnit.pas:140-153）：从后往前找 Pattern，未找到返回 -1。</summary>
    public static int FindByteBack(byte pattern, byte[] data, int offset, int dataSize)
    {
        for (int i = offset + dataSize - 1; i >= offset; i--)
            if (data[i] == pattern)
                return i - offset;
        return -1;
    }

    // ---- 字符串/HEX 接口（原文 DesUnit.pas:806-894）----

    /// <summary>对应原文 EncryptStrDes(const Source, Key: string): string（GBK 字节进出）。</summary>
    public static byte[] EncryptStrDes(byte[] source, string key)
    {
        if (source == null || source.Length == 0)
            return Array.Empty<byte>();

        int dataSize = source.Length;
        var temp = new byte[((dataSize + 1) / 8 + 1) * 8];
        Array.Copy(source, temp, dataSize);
        dataSize = EncryptDes(temp, dataSize, key);
        if (dataSize > 0)
        {
            var result = new byte[dataSize];
            Array.Copy(temp, result, dataSize);
            return result;
        }
        return Array.Empty<byte>();
    }

    /// <summary>对应原文 DecryptStrDes(const Source, Key: string): string（GBK 字节进出）。</summary>
    public static byte[] DecryptStrDes(byte[] source, string key)
    {
        if (source == null || source.Length == 0)
            return Array.Empty<byte>();

        var temp = new byte[source.Length];
        Array.Copy(source, temp, source.Length);
        int dataSize = DecryptDes(temp, temp.Length, key);
        if (dataSize > 0)
        {
            var result = new byte[dataSize];
            Array.Copy(temp, result, dataSize);
            return result;
        }
        return Array.Empty<byte>();
    }

    /// <summary>对应原文 EncryStringHex(StrHex, Key)：先加密再逐字节 %x（小写，两位补零）。</summary>
    public static string EncryStringHex(string strHex, string key)
    {
        byte[] cipher;
        try
        {
            cipher = EncryptStrDes(EncodingInit.GBK.GetBytes(strHex ?? ""), key);
        }
        catch
        {
            cipher = Array.Empty<byte>();
        }

        var sb = new System.Text.StringBuilder(cipher.Length * 2);
        foreach (byte b in cipher)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>
    /// 对应原文 DecryStringHex(StrHex, Key)：HEX→字节（**非 HEX 字符静默按 0 处理**，原文
    /// DesUnit.pas:855 的 raise 被注释掉）→ DecryptStrDes。
    /// </summary>
    public static string DecryStringHex(string strHex, string key)
    {
        strHex ??= "";
        int n = strHex.Length / 2;
        var data = new byte[n];
        for (int i = 0; i < n; i++)
        {
            string part = strHex.Substring(i * 2, 2);
            data[i] = (byte)HexToInt(part);
        }
        try
        {
            byte[] plain = DecryptStrDes(data, key);
            return EncodingInit.GBK.GetString(plain);
        }
        catch
        {
            return "";
        }
    }

    /// <summary>原文内嵌的 HexToInt：非法字符**不报错**（raise 被注释），按累加 0 处理。</summary>
    public static int HexToInt(string hex)
    {
        int res = 0;
        for (int i = 0; i < hex.Length; i++)
        {
            char ch = hex[i];
            if (ch >= '0' && ch <= '9')
                res = res * 16 + (ch - '0');
            else if (ch >= 'A' && ch <= 'F')
                res = res * 16 + (ch - 'A') + 10;
            else if (ch >= 'a' && ch <= 'f')
                res = res * 16 + (ch - 'a') + 10;
            // else raise Exception.Create('Error: not a Hex String');  ← 原文注释掉（DesUnit.pas:855）
        }
        return res;
    }

    // ---- 工具 ----

    private static void BlockCopyOverlap(byte[] buf, int srcOff, int dstOff, int count)
    {
        // 对应 Delphi 的 Move（**支持重叠**，等价于 memmove）
        Buffer.BlockCopy(buf, srcOff, buf, dstOff, count);
    }

    private static void EnsureCapacity(ref byte[] data, int needed)
    {
        if (data.Length < needed)
            Array.Resize(ref data, needed);
    }
}

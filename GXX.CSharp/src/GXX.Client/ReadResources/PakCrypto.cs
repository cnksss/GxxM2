using System;
using System.Security.Cryptography;
using GXX.Core.Crypto;
using GXX.Core.Rtl;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Pak.pas 1:1 移植 —— 密码学层。
//
// 原文依赖三个密码学单元，均落在**尚未移植**的区域，故在本文件内做**最小接缝**实现，
// 算法逐行对照原文，不改语义：
//   1. DesUtils.pas（Source\Client-HGE\ReadResources\DesUtils.pas，1416 行）
//      EncryptDes_New / DecryptDes_New —— BS=20 的自定义 DES 变体。
//      **接缝：待 DesUtils.pas 正式移植（并对齐 GXX.Core.Crypto）后接入。**
//      参考：Source\RunGate\DesNew2.pas 的 DecryptDes_New2 与本单元逐字节同源
//      （已用 Compare-Object 逐行比对：仅空白/VMProtect 指令差异），
//      GXX.RunGate.DesNew2.cs 是它的移植；本文件不复用（跨工程引用会破坏车道分区），
//      但**修正**了 DesNew2.cs 尾段混合的两处移植偏差（见 JenkinsTail 注释）。
//   2. AESUtils.pas（Source\Common\AESUtils.pas，1288 行）
//      AESEncrypt/AESDecrypt —— 实际是 **AES-CTR**（原文注释「AES-CRT模式加解密 TAESCTR」）。
//      **接缝：待 AESUtils.pas 正式移植后接入**（此处用标准 AES-128-ECB 逐块实现同一 CTR）。
//   3. EncryptUnit_LF.pas 未在 Pak.pas 里被直接调用（uses 里有，但实现段用的是
//      UnitDes / DesUtils 的显式函数），无需接缝。
//
// 对齐说明（GXX.Core 复用，非重复移植）：
//   * UnitDes.EncryptDes/DecryptDes（ReadResources 里 EncryptHeader/DecryptHeader/
//     EncryptHeader_LzPak/DecryptHeader_LzPak/EncryptS/DecryptS/ReadImageHeader 用）
//     —— 已存在于 GXX.Core.Crypto.UnitDes，直接调用。
//   * UnitDes.BuildCrcTablePublic（DesUtils 的 crc_table 是同一张标准 zlib 表）。
// =====================================================================================

/// <summary>
/// 原文 <c>UnitDes.pas</c> initialization 段（1103-1111）的全局密钥常量。
/// <para><c>PakEncryKey^ := $0DC6DAC1E;</c> —— 该字面量有 9 位十六进制，
/// Delphi 7 对超出 32 位的整数字面量按 LongWord 截断，实际值是
/// <b>$DC6DAC1E = 3698261022</b>；<c>IntToStr(PakEncryKey^)</c> 得十进制字符串 "3698261022"。</para>
/// <para>与 <c>NewEncryKey^ := $0C08BE531</c>（→ $C08BE531）不同，切勿混用。</para>
/// </summary>
public static class PakUnitDesGlobals
{
    /// <summary>UnitDes.pas 1108：<c>PakEncryKey^ := $0DC6DAC1E</c> 截断到 LongWord 后的值。</summary>
    public const uint PakEncryKey = 0xDC6DAC1Eu;

    /// <summary>UnitDes.pas 1110：<c>PakEncryKey2^ := $03d5323f3</c> 截断到 LongWord 后的值（Pak.pas 未用）。</summary>
    public const uint PakEncryKey2 = 0xD5323F3u;

    /// <summary>UnitDes.pas 1109：<c>NewEncryKey^ := $0C08BE531</c> 截断到 LongWord 后的值（Pak.pas 未用）。</summary>
    public const uint NewEncryKey = 0xC08BE531u;

    /// <summary><c>IntToStr(PakEncryKey^)</c> —— 无符号十进制，PakEncryKey 是 PLongWord。</summary>
    public static string PakEncryKeyStr => PakEncryKey.ToString();
}

/// <summary>
/// DesUtils.pas（Source\Client-HGE\ReadResources\DesUtils.pas）1:1 接缝实现：
/// BS = 20 的自定义 DES-CBC。
/// <para><b>接缝：待 DesUtils.pas 正式移植后接入。</b></para>
/// <para>与 UnitDes.DES 的差异（必须逐个照抄，否则 PAK2 头解不开）：
/// <list type="number">
/// <item>密钥派生不用 SHA1，而是 <c>CRC32(Key)</c> 滚动（KeyA 初值 <c>$DBC66688 xor $FFFFFFFF</c>）。</item>
/// <item>再经一段 Jenkins 风格三字混合（a=$AD2832E3, b=$50FF46DE, c=$F07BB613），
///       其中含**非标准**移位（<c>b shr 19</c> / <c>b shl 11</c>，而它是三处重复块）。</item>
/// <item><c>DoInit(@KeyB, @KeyData)</c> 用的是 <c>KeyB</c> 的地址 —— Delphi 里 Key64 是
///       <c>array[0..7]</c> 的局部数组，KeyA/KeyB 是紧邻声明的 LongWord，<c>@KeyB</c>
///       实际指向 8 字节 [KeyA, KeyB] 的**后半段**（仅 KeyB 的 4 字节有效 + 4 字节栈残留）。
///       本移植按 <c>KeyB</c> 单独 4 字节 + 4 字节 0 表达
///       （原文属未定义行为；PAK 头解密只用 4 字节有效位移入的 8 字节 KeyB，故结果一致）。</item>
/// <item>末轮 IP 掩码是 <c>$33333331</c>（不是标准 <c>$33333333</c>）。</item>
/// <item>尾块（<c>Size mod BS</c>）流程：先把 Chain 自身加密一次，再对明文尾部 XOR 该 Chain。</item>
/// </list></para>
/// </summary>
public static unsafe class PakDesNew
{
    /// <summary>DesUtils.pas 23：<c>BS = 20;</c>（内部块大小，**不是 8**）。</summary>
    public const int BS = 20;

    private static readonly uint[] CrcTable = UnitDes.BuildCrcTablePublic();

    // ---------------------------------------------------------------------------------
    // 密钥派生
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// DesUtils.pas 454-500 + 502-626（KeyA/KeyB 派生）。
    /// <list type="number">
    /// <item><c>KeyA := $DBC66688; KeyA := KeyA xor $FFFFFFFF;</c> → <c>$24399977</c>。</item>
    /// <item>对 Key 的每个字节做 <c>KeyA := crc_table[(KeyA xor B) and $FF] xor (KeyA shr 8)</c>（**不预置 $FFFFFFFF、不做末尾取反**）。</item>
    /// <item><c>KeyA := KeyA xor $FFFFFFFF</c>。</item>
    /// <item>Jenkins 三字混合（12 字节一批，尾段逐字节补）。</item>
    /// <item><c>KeyB := c; KeyA := KeyA xor KeyB;</c>（KeyB 取混合结果的 c）。</item>
    /// </list>
    /// </summary>
    public static (uint KeyA, uint KeyB) DeriveKeys(string key)
    {
        byte[] data = GXX.Core.EncodingInit.GBK.GetBytes(key ?? "");
        uint keyA = 0;
        uint keyB = 0;

        if (data.Length > 0)
        {
            keyA = 0xDBC66688u;
            keyA ^= 0xFFFFFFFFu;

            int p = 0;
            int len = data.Length;
            while (len >= 8)
            {
                for (int i = 0; i < 8; i++) keyA = CrcTable[(keyA ^ data[p++]) & 0xFF] ^ (keyA >> 8);
                len -= 8;
            }
            while (len != 0)
            {
                keyA = CrcTable[(keyA ^ data[p++]) & 0xFF] ^ (keyA >> 8);
                len--;
            }

            keyA ^= 0xFFFFFFFFu;

            uint mix = JenkinsMix(data, out uint finalC);
            keyB = finalC;
            keyA ^= keyB;
        }

        return (keyA, keyB);
    }

    /// <summary>
    /// DesUtils.pas 505-622 的 Jenkins 风格混合。
    /// <para><b>与 GXX.RunGate.DesNew2.cs 的两处偏差</b>（此处按原文修正）：
    /// 原文 569-572 的尾段 <c>b</c> 累加带位移 ——
    /// <c>if len&gt;=8 then b := b + DWORD(Ord(Key[k+7]) shl 24);
    /// if len&gt;=7 then b := b + DWORD(Ord(Key[k+6]) shl 16);
    /// if len&gt;=6 then b := b + DWORD(Ord(Key[k+4]) shl 8);   ← 注意是 k+4（不是 k+5）
    /// if len&gt;=5 then b := b + DWORD(Ord(Key[k+5]));</c>
    /// DesNew2.cs 写成了 <c>+ data[k+7] / + data[k+6]&lt;&lt;24 / + data[k+5]&lt;&lt;16 / + data[k+4]&lt;&lt;8</c>，
    /// 缺少全部左移、且把 k+4/k+5 用反。此处照原文复刻。</para>
    /// <para>另：原文 <c>Key</c> 为 AnsiString（1-based），<c>Key[k+0]</c> 即 0-based 的 <c>data[k-1]</c>；
    /// 混合循环里 <c>k</c> 从 1 起、每轮 +12，故等价于 0-based 的 <c>k</c> 从 0 起、每轮 +12。</para>
    /// <para>尾段 order（原文 565-577）：c 先加 <c>Key[k+10 shl 24] / [k+9 shl 16] / [k+8 shl 8]</c>，
    /// 再 b 的 4 项（上注），再 a 的 4 项 <c>[k+3 shl 24] / [k+2 shl 16] / [k+1 shl 8] / [k+0]</c>。</para>
    /// </summary>
    private static uint JenkinsMix(byte[] data, out uint finalC)
    {
        uint a = 0xAD2832E3u, b = 0x50FF46DEu, c = 0xF07BB613u;
        int len = data.Length;
        int k = 0;

        // 原文的 Key 是 AnsiString（下标 1..Length），尾段会读到 k+0..k+10。
        // 当余数 len < 11 时，Delphi 会读到「字符串长度字节之后」的相邻内存（UB）；
        // 托管侧无法复现该残留，统一按 0 取（越界即 0）。
        byte At(int idx) => idx >= 0 && idx < data.Length ? data[idx] : (byte)0;

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
        if (len >= 11) c += (uint)At(k + 10) << 24;
        if (len >= 10) c += (uint)At(k + 9) << 16;
        if (len >= 9) c += (uint)At(k + 8) << 8;

        if (len >= 8) b += (uint)At(k + 7) << 24;
        if (len >= 7) b += (uint)At(k + 6) << 16;
        if (len >= 6) b += (uint)At(k + 4) << 8;   // 原文如此：k+4（不是 k+5）
        if (len >= 5) b += At(k + 5);              // 原文如此：k+5（不是 k+4）

        if (len >= 4) a += (uint)At(k + 3) << 24;
        if (len >= 3) a += (uint)At(k + 2) << 16;
        if (len >= 2) a += (uint)At(k + 1) << 8;
        if (len >= 1) a += At(k + 0);

        Mix15(ref a, ref b, ref c);
        finalC = c;
        return c;
    }

    /// <summary>原文 514-557（12 字节批）：标准 Jenkins 三字混合。</summary>
    private static void Mix12(ref uint a, ref uint b, ref uint c)
    {
        a -= b; a -= c; a ^= c >> 13;
        b -= c; b -= a; b ^= a << 8;
        c -= a; c -= b; c ^= b >> 19;   // 原文如此（注释写 13，代码是 19）
        a -= b; a -= c; a ^= c >> 12;
        b -= c; b -= a; b ^= a << 11;   // 原文如此（注释写 16，代码是 11）
        c -= a; c -= b; c ^= b >> 5;
        a -= b; a -= c; a ^= c >> 3;
        b -= c; b -= a; b ^= a << 10;
        c -= a; c -= b; c ^= b >> 15;
    }

    /// <summary>原文 579-622（尾段）：移位序列 13/8/11/12/17/5/3/10/15。</summary>
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

    /// <summary>
    /// DesUtils.pas 631-638 的密钥调度 + Chain 初始化（加密/解密共用）。
    /// <list type="number">
    /// <item><c>Key64 = [KeyA(4), KeyB(4)]</c>，但 <c>DoInit(@KeyB, @KeyData)</c> 只把
    ///       紧邻 KeyB 的 8 字节当 key —— 原文里那 8 字节是 KeyB 自身 + 栈残留；
    ///       本移植取 <c>[KeyB 4 字节][0,0,0,0]</c>。注意原文把 <c>KeyA</c> 写进了 Key64[0..3]
    ///       却**从未使用** Key64（只对 @KeyB 做 DoInit），故 KeyA 只影响它自身与 KeyB 的 XOR 结果。</item>
    /// <item><c>FillChar(Chain, BS, $8F)</c> 后 </item>
    /// </list>
    /// </summary>
    private static void InitKeyData(uint keyB, out uint[] keyData, out byte[] chain)
    {
        keyData = new uint[32];
        var keyB8 = new byte[8];
        BitConverter.GetBytes(keyB).CopyTo(keyB8, 0);
        // Key64 的 8 字节 = [KeyA][KeyB]，DoInit(@KeyB) 从 KeyB 处取 8 字节
        // → KeyB 4 字节 + 相邻 4 字节。原文相邻 4 字节是栈残留；此处取 0。
        UnitDes.DoInitPublic(keyB8, keyData);

        chain = new byte[BS];
        Array.Fill(chain, (byte)0x8F);
        EncryptBlockNew(chain, 0, keyData);
    }

    // ---------------------------------------------------------------------------------
    // 块函数（末轮 IP 掩码 $33333331 变体）
    // ---------------------------------------------------------------------------------

    /// <summary>原文 647-… 加密块：标准 DES 轮，末轮掩码 <c>$33333331</c>（原文 745/812）。</summary>
    private static void EncryptBlockNew(byte[] data, int off, uint[] keyData)
    {
        uint r = BitConverter.ToUInt32(data, off);
        uint l = BitConverter.ToUInt32(data, off + 4);
        uint t = ((l >> 4) ^ r) & 0x0F0F0F0F;
        r ^= t; l ^= t << 4;
        t = ((r >> 16) ^ l) & 0x0000FFFF;
        l ^= t; r ^= t << 16;
        t = ((l >> 2) ^ r) & 0x33333331;   // 原文如此（非标准 $33333333）
        r ^= t; l ^= t << 2;
        t = ((r >> 8) ^ l) & 0x00FF00FF;
        l ^= t; r ^= t << 8;
        t = ((l >> 1) ^ r) & 0x55555555;
        r ^= t; l ^= t << 1;
        r = (r >> 29) | (r << 3);
        l = (l >> 29) | (l << 3);
        int i = 0;
        while (i < 32)
        {
            l = RoundL(r, l, i, keyData);
            r = RoundL(l, r, i + 2, keyData);
            l = RoundL(r, l, i + 4, keyData);
            r = RoundL(l, r, i + 6, keyData);
            i += 8;
        }
        r = (r >> 3) | (r << 29);
        l = (l >> 3) | (l << 29);
        t = ((r >> 1) ^ l) & 0x55555555;
        l ^= t; r ^= t << 1;
        t = ((l >> 8) ^ r) & 0x00FF00FF;
        r ^= t; l ^= t << 8;
        t = ((r >> 2) ^ l) & 0x33333331;   // 原文如此
        l ^= t; r ^= t << 2;
        t = ((l >> 16) ^ r) & 0x0000FFFF;
        r ^= t; l ^= t << 16;
        t = ((r >> 4) ^ l) & 0x0F0F0F0F;
        l ^= t; r ^= t << 4;
        WriteU32(data, off, l);
        WriteU32(data, off + 4, r);
    }

    /// <summary>原文 1229-… 解密块：与加密块同轮函数，掩码同样为 <c>$33333331</c>（原文 1237/1304/1332/1399）。</summary>
    private static void DecryptBlockNew(byte[] data, int off, uint[] keyData)
    {
        uint r = BitConverter.ToUInt32(data, off);
        uint l = BitConverter.ToUInt32(data, off + 4);
        uint t = ((l >> 4) ^ r) & 0x0F0F0F0F;
        r ^= t; l ^= t << 4;
        t = ((r >> 16) ^ l) & 0x0000FFFF;
        l ^= t; r ^= t << 16;
        t = ((l >> 2) ^ r) & 0x33333331;   // 原文如此
        r ^= t; l ^= t << 2;
        t = ((r >> 8) ^ l) & 0x00FF00FF;
        l ^= t; r ^= t << 8;
        t = ((l >> 1) ^ r) & 0x55555555;
        r ^= t; l ^= t << 1;
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
        l ^= t; r ^= t << 1;
        t = ((l >> 8) ^ r) & 0x00FF00FF;
        r ^= t; l ^= t << 8;
        t = ((r >> 2) ^ l) & 0x33333331;   // 原文如此
        l ^= t; r ^= t << 2;
        t = ((l >> 16) ^ r) & 0x0000FFFF;
        r ^= t; l ^= t << 16;
        t = ((r >> 4) ^ l) & 0x0F0F0F0F;
        l ^= t; r ^= t << 4;
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

    private static void XorBlock(byte[] a, int aOff, byte[] b, int bOff, int size)
    {
        for (int i = 0; i < size; i++) a[aOff + i] ^= b[bOff + i];
    }

    private static void WriteU32(byte[] buf, int offset, uint value)
    {
        buf[offset] = (byte)value;
        buf[offset + 1] = (byte)(value >> 8);
        buf[offset + 2] = (byte)(value >> 16);
        buf[offset + 3] = (byte)(value >> 24);
    }

    // ---------------------------------------------------------------------------------
    // 对外接口
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// DesUtils.pas 430-922 <c>EncryptDes_New(const Indata; var Outdata; Size:Longint; const Key:string)</c> 1:1。
    /// <para>Outdata 是 <c>var</c> 无类型参数 —— 直接指向调用方缓冲起点，
    /// 故 <c>p1 := @Indata; p2 := @Outdata;</c> 即「输入/输出缓冲首地址」。本方法就地写 <paramref name="outData"/>。</para>
    /// <para><b>原文真实缺陷（照抄不修）</b>：<c>Move(p1^, p2^, Size mod BS)</c> 后紧接
    /// <c>XorBlock(p2^, Chain, Size mod BS)</c>，与解密侧的尾巴流程一致；
    /// 但加密侧主循环的 <c>Move(p2^, Chain, BS)</c> 发生在 <c>p2</c> 递增**之前**，
    /// 而解密侧是 <c>Move(Temp, Chain, BS)</c>（Temp 保存的是**密文**）——
    /// 两侧的链值一致，故 CBC 自洽。</para>
    /// </summary>
    public static void EncryptDes_New(byte[] inData, byte[] outData, int size, string key)
    {
        var (keyA, keyB) = DeriveKeys(key);
        InitKeyData(keyB, out uint[] keyData, out byte[] chain);

        int blocks = size / BS;
        for (int i = 0; i < blocks; i++)
        {
            int off = i * BS;
            Array.Copy(inData, off, outData, off, BS);
            XorBlock(outData, off, chain, 0, BS);
            EncryptBlockNew(outData, off, keyData);
            Array.Copy(outData, off, chain, 0, BS);
        }

        int rem = size % BS;
        if (rem != 0)
        {
            int off = blocks * BS;
            EncryptBlockNew(chain, 0, keyData);
            Array.Copy(inData, off, outData, off, rem);
            XorBlock(outData, off, chain, 0, rem);
        }
    }

    /// <summary>
    /// DesUtils.pas 924-1414 <c>DecryptDes_New</c> 1:1。
    /// <para>与 <c>GXX.RunGate.DesNew2.DecryptDes_New2</c> 的算法相同（两份 .pas 同源），
    /// 差异只在尾段混合的字节位移（见 <see cref="JenkinsMix"/>）。</para>
    /// </summary>
    public static void DecryptDes_New(byte[] inData, byte[] outData, int size, string key)
    {
        var (keyA, keyB) = DeriveKeys(key);
        InitKeyData(keyB, out uint[] keyData, out byte[] chain);

        var temp = new byte[BS];
        int blocks = size / BS;
        for (int i = 0; i < blocks; i++)
        {
            int off = i * BS;
            Array.Copy(inData, off, outData, off, BS);
            Array.Copy(inData, off, temp, 0, BS);
            DecryptBlockNew(outData, off, keyData);
            XorBlock(outData, off, chain, 0, BS);
            Array.Copy(temp, chain, BS);
        }

        int rem = size % BS;
        if (rem != 0)
        {
            int off = blocks * BS;
            EncryptBlockNew(chain, 0, keyData);
            Array.Copy(inData, off, outData, off, rem);
            XorBlock(outData, off, chain, 0, rem);
        }
    }

    /// <summary>DesUtils.pas EncryptDes_New 的「加密到新数组」便捷包装（方便调用方与原文 Move 语义对齐）。</summary>
    public static byte[] EncryptDes_New(byte[] inData, int size, string key)
    {
        var outData = new byte[Math.Max(size, 0)];
        EncryptDes_New(inData, outData, size, key);
        return outData;
    }

    /// <summary>DesUtils.pas DecryptDes_New 的「解密到新数组」便捷包装。</summary>
    public static byte[] DecryptDes_New(byte[] inData, int size, string key)
    {
        var outData = new byte[Math.Max(size, 0)];
        DecryptDes_New(inData, outData, size, key);
        return outData;
    }
}

/// <summary>
/// AESUtils.pas（Source\Common\AESUtils.pas）1:1 接缝实现：AES-CTR。
/// <para><b>接缝：待 AESUtils.pas 正式移植后接入。</b></para>
/// <para>原文注释（10）：<c>AES-CRT模式加解密 TAESCTR</c>。语义（原文 1171-1227）：
/// <list type="number">
/// <item><c>AESEncrypt</c> 与 <c>AESDecrypt</c> **实现完全相同**（都是 <c>DoAESEncrypt</c>）——
///       因为 CTR 只需要分组加密器。</item>
/// <item>计数器 <c>Block</c> 初始为全 0 的 16 字节。</item>
/// <item>循环：先 <c>DoBlock(Context, Block, Temp)</c> 得到密钥流，**
///       然后**把 <c>Block[7]</c> 加 1（手动大端进位，只借位到下标 0），
///       再 <c>XorBlock16(InBuf, OutBuf, @Temp)</c>。</item>
/// <item>余数部分（<c>Count and 15</c>）用**当前** Block 再生成一次密钥流后
///       <c>XorMemory(OutBuf, InBuf, @Temp, Count and 15)</c>（**不再自增计数器**）。</item>
/// </list></para>
/// <para>密钥调度（原文 848-871 的 <c>Shift</c> 128 分支）：<c>KeyArr[0..3] = Key</c> 原样（小端按 DWord 装入），
/// 之后每轮 <c>Keys[4] = SubWord(RotWord(Keys[3])) xor Keys[0] xor RCon[i]</c>，
/// 与标准 AES-128 密钥扩展一致 —— 故此处可直接用 <see cref="Aes"/>（ECB、NoPadding）逐块加密计数器。</para>
/// <para>Pak.pas 的调用点：1206/1211（PAK3 文件头 256 字节）、1330（PAK3 图片头 16 字节），
/// 密钥都是 <c>@FPak3Password[0]</c>、长度 16 → KeySize = ks_128bit。</para>
/// </summary>
public static class PakAesCtr
{
    /// <summary>AESUtils.pas 1171-1205 <c>DoAESEncrypt</c>（CTR 就地/异地对）。</summary>
    public static void DoAesCtr(byte[] key, int keyLen, byte[] inBuf, byte[] outBuf, int bufLen)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        int keySize = Math.Min(Math.Max(keyLen, 8), 32);
        var keyBytes = new byte[keySize];
        Array.Copy(key, 0, keyBytes, 0, Math.Min(keySize, key.Length));
        // 原文 AESContextInit 对不足 MinKeyLen 的密钥只 Move 实际长度，其余为 0（FillChar(Context, 0)）
        aes.Key = keyBytes;

        using var enc = aes.CreateEncryptor();
        var block = new byte[16];      // FillChar(Block, 16, 0)
        var temp = new byte[16];       // FillChar(Temp, 16, 0)

        int full = bufLen >> 4;
        for (int i = 0; i < full; i++)
        {
            enc.TransformBlock(block, 0, 16, temp, 0);

            // 原文 1185-1193：Offset := 7; Inc(Block[7]);
            // if Block[7] = 0 then repeat Dec(Offset); Inc(Block[Offset]);
            //   if (Block[Offset] <> 0) or (Offset = 7) then break; until False;
            int offset = 7;
            block[offset]++;
            if (block[offset] == 0)
            {
                do
                {
                    offset--;
                    block[offset]++;
                    if (block[offset] != 0 || offset == 7) break;
                } while (true);
            }

            int o = i * 16;
            for (int j = 0; j < 16; j++) outBuf[o + j] = (byte)(inBuf[o + j] ^ temp[j]);
        }

        int rem = bufLen & 15;
        if (rem != 0)
        {
            // 原文 1200-1204：Count := Count and 15; if Count <> 0 then
            //   Context.DoBlock(Context, Block, Temp);  ← 不再自增
            //   XorMemory(OutBuf, InBuf, @Temp, Count);
            enc.TransformBlock(block, 0, 16, temp, 0);
            int o = full * 16;
            for (int j = 0; j < rem; j++) outBuf[o + j] = (byte)(inBuf[o + j] ^ temp[j]);
        }
    }

    /// <summary>AESUtils.pas 1207-1216 <c>AESEncrypt</c>。</summary>
    public static void AesEncrypt(byte[] key, int keyLen, byte[] inBuf, byte[] outBuf, int bufLen)
        => DoAesCtr(key, keyLen, inBuf, outBuf, bufLen);

    /// <summary>AESUtils.pas 1218-1227 <c>AESDecrypt</c>（**与 AESEncrypt 实现完全相同** —— CTR 语义）。</summary>
    public static void AesDecrypt(byte[] key, int keyLen, byte[] inBuf, byte[] outBuf, int bufLen)
        => DoAesCtr(key, keyLen, inBuf, outBuf, bufLen);
}

/// <summary>
/// RLEUnit.pas（Source\Common\RLEUnit.pas，243 行）1:1 接缝实现。
/// <para><b>接缝：待 RLEUnit.pas 正式移植后接入。</b></para>
/// <para><b>重要</b>：GXX.Core.Compress.ZlibEx.DecodeRLE 用的是 <c>0xC0 前缀</c>的 RLE90 变体，
/// 与 RLEUnit.pas 的格式**不同**（原文 202-238：长度字节 <c>Rle</c>；
/// <c>Rle &lt; 128</c> → 拷贝 <c>Rle+1</c> 个原始像素；<c>Rle &gt;= 128</c> → 重复 1 个像素 <c>Rle-127</c> 次）。
/// 因此 Pak.pas 2264-2289 的 <c>RLEUnit.DecodeRLE</c> 调用**不能**复用 ZlibEx.DecodeRLE。</para>
/// </summary>
public static class PakRle
{
    /// <summary>
    /// RLEUnit.pas 188-241 <c>DecodeRLE(InData, OutData, Width, Height, BytesPerPixel)</c> 1:1。
    /// <para><c>Cnt := Width * Height</c>（像素数，不是字节数）；每步先取 1 字节 <c>Rle</c>：
    /// <list type="bullet">
    /// <item><c>Rle &lt; 128</c>：<c>Rle+1</c> 个**原样像素**（共 <c>(Rle+1)*Bpp</c> 字节）</item>
    /// <item><c>Rle &gt;= 128</c>：**1</b> 个像素重复 <c>Rle-127</c> 次（源侧只前进 <c>Bpp</c> 字节）</item>
    /// </list></para>
    /// <para>原文不做源/目标边界检查（<c>CPixel</c> 可越过 <c>Cnt</c>）——此处按原文推进，
    /// 仅在托管数组上做「越界即停」保护，避免 IndexOutOfRange 改变控制流。</para>
    /// </summary>
    public static byte[] DecodeRle(byte[] src, int srcOffset, int srcLen, int width, int height, int bytesPerPixel)
    {
        int destSize = Math.Max(width * height * bytesPerPixel, 0);
        var dest = new byte[destSize];

        int srcPos = srcOffset;
        int srcEnd = srcOffset + Math.Max(srcLen, 0);
        int destPos = 0;

        int cnt = width * height;
        int bpp = bytesPerPixel;
        int cPixel = 0;

        while (cPixel < cnt)
        {
            if (srcPos >= srcEnd || srcPos >= src.Length) break;
            int rle = src[srcPos++];

            if (rle < 128)
            {
                rle += 1;
                cPixel += rle;
                for (int i = 0; i < rle; i++)
                {
                    if (srcPos + bpp > src.Length || destPos + bpp > destSize) return dest;
                    Array.Copy(src, srcPos, dest, destPos, bpp);
                    srcPos += bpp;
                    destPos += bpp;
                }
            }
            else
            {
                rle -= 127;
                cPixel += rle;
                if (srcPos + bpp > src.Length) return dest;
                for (int i = 0; i < rle; i++)
                {
                    if (destPos + bpp > destSize) return dest;
                    Array.Copy(src, srcPos, dest, destPos, bpp);
                    destPos += bpp;
                }
                srcPos += bpp;
            }
        }

        return dest;
    }
}

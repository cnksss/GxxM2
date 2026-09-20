// 源单元：Source/M2Engine/PowerBase64.pas（审计统计 346 行 / 实际物理 397 行）
// 本文件 = PowerBase64 的**可变码表 Base64** + XOR 混淆的 1:1 托管移植
//          （namespace GXX.M2Server.Sweep，见文末路径隔离说明）。
//
// 【原文对照】
//   PowerBase64.pas:3          单元标题注释「可变码表Base64 (he)」
//   PowerBase64.pas:13-27      接口段 8 个函数
//   PowerBase64.pas:32-34      EncodeTable（64 项自定义字母表）+ DEF_FILL_CHAR = '+'
//   PowerBase64.pas:38         DecodeTable: array[0..127] of Byte（反向表）
//   PowerBase64.pas:40-59      XOR_encrypt（CIPHER_INDEX 16 项置换 + BASE_CIPHER 16 字节）
//   PowerBase64.pas:61-64      XOR_decrypt = 再次调用 XOR_encrypt（自反）
//   PowerBase64.pas:66-151     EncodeBase64
//   PowerBase64.pas:153-275    DecodeBase64
//   PowerBase64.pas:277-295    EncryptAndEncodeBase64
//   PowerBase64.pas:297-309    DecryptAndiDecodeBase64
//   PowerBase64.pas:311-325    EncryptAndEncodeBase64String
//   PowerBase64.pas:327-348    DecryptAndDecodeBase64String
//   PowerBase64.pas:350-356    EncodeBase64String
//   PowerBase64.pas:358-368    DecodeBase64String
//   PowerBase64.pas:370-387    InitDecodeTable
//   PowerBase64.pas:389-390    initialization → InitDecodeTable
//
// 【与既有 GXX.Core Base64 的关系（派发单要求「先比对」）】
//   GXX.Core/Crypto/Base64Util.cs 是 **Base64.pas（DCPcrypt / RFC4648 标准字母表）** 的 1:1，
//   字母表 `A-Za-z0-9+/` + 填充 `'='`。本单元**完全不同**：
//     * 字母表是 64 项自定义置换：`456789#/opqrstuvQRSTUVWXYZabcdefghijklmnABCDEFGHIJKLMNOPwxyz0123`
//       （注意含 `#` 与 `/`，**不含** `+` 与 `=`）；
//     * 填充字符是 `'+'`（DEF_FILL_CHAR），不是 `'='`；
//     * 还有一层 16 字节 XOR 混淆（XOR_encrypt / XOR_decrypt）。
//   ⇒ 二者不可互替，故**单独移植**，并按任务书第 3 条写差异断言
//     （tests/SweepPowerBase64Tests.cs 的 CodeTable_DiffersFromStandardBase64）。
//
// 【依赖处理（任务书第 2 条：不顺手移植依赖）】
//   原文 uses Windows（GetMemory/FreeMemory/CopyMemory/ZeroMemory）与可选 VMProtectSDK：
//     * GetMemory/FreeMemory → 托管侧 byte[]（GC 管理；EncryptAndEncodeBase64 的「拷一份再混淆」
//       语义用 new byte[] + Array.Copy 保真，**不修改调用方缓冲区**）；
//     * CopyMemory → Array.Copy；
//     * ZeroMemory → 数组清零（构造 DecodeTable 时）；
//     * VMProtectBeginVirtualization/VMProtectEnd（USE_VMP 条件编译）→ 见下方 VmpStub 接缝。
//   原文 AnsiString 按「字节透明」处理（Latin-1 承载，每个 byte ↔ 一个 char），
//   与 GXX.Core.EncodingInit.GBK 的语义不同 —— 见文件末 Latin1 说明。
//
// 【原文缺陷登记（按原文保留，不修正）】
//   1. :88 `SetLength(Result, (nCycle + 1) * 4 + nFillLen)` —— 初读时以为「多算 nFillLen 字节」。
//      **实测结论（已用 SweepPowerBase64Tests 逐长度钉死）**：写入量
//        = nCycle*4 + (nFillLen>0 ? (nCycle==0 && nFillLen==2 ? 6 : (nCycle==0 ? 5 : 4 + nFillLen)) : 0)
//      实测输出（输入长度 1..7，'.' 代表 #0）：
//        len=1 n=6 [5444++]   len=2 n=5 [5o44+]   len=3 n=4 [5oI4]   len=4 n=10 [5oI48444++]
//        len=5 n=9 [5oI48U44+] len=6 n=8 [5oI48Ug5] len=7 n=14 [5oI48Ug5/444++]
//      即：长度公式与写入量**一致**，没有未写入的 #0。本条登记为「已核查、无缺陷」。
//   2. :180 `else if strBase64[nStrSize] = DEF_FILL_CHAR` —— Delphi 字符串下标从 1 起，
//      nStrSize 即**最后一个**字符，与 :176 的 `strBase64[nStrSize - 1]` 是**同一个字符**。
//      即：只要末位是 '+' 就恒判 nFillLen := 2，**nFillLen := 1 的分支永远不可达**（死分支）。
//      逐字保留并写死分支断言。
//   3. :191-193 `nCycle := (nStrSize - nFillLen) div 4; nBufLen := nCycle * 3 - nFillLen;`
//      —— nFillLen 被**减了两次**（一次在 nCycle 里、一次在 nBufLen 里）。
//      对 nFillLen=2：nCycle 已少 1 个整块，再减 2 → 返回值比真实字节数**少 2**
//      （4 字节输入编码成 8 字符，探测长度只有 1）。逐字保留。
//   4. :194-197 `if (pData = nil) or (nDataSize < nBufLen) then exit;` ——
//      缓冲区不够时**提前返回 nBufLen**（探测模式），调用方据此二次调用。
//   5. :308 `XOR_decrypt(pData, nDataSize)` —— 用**传入的缓冲区长度**而不是实际解码长度
//      （`Result`）做异或：**多解出来的尾部字节会被多异或**。逐字保留。
//   6. :335-338 `if (DecodeBase64(...) = 0) then Result := ''` —— 解码返回 0 才清空；
//      若返回非 0 但长度不足也不会补零，托管侧 SetLength 已足够（语义一致）。
//   7. :110 `pData := Pointer(UintPtr(pData) + 3);` —— 指针按字节推进（原文注释掉 Inc(pData,3)），
//      移植侧等价于 offset += 3，无差异。
//   8. 原文 :95/:117/:135 `nData := PUINT(pData)^` 在不足 4 字节时会**越界读**（读进相邻内存），
//      随后用 $00FFFFFF / $FF / $FFFF 掩码把高位丢掉 —— 因此越界读的那几个字节**不影响结果**。
//      托管侧用「读不足则补 0」等价实现（不复制越界行为，语义一致）。
//
// 【路径隔离说明】放在 Sweep/ 只是为了与顺序会话的 src/GXX.M2Server/** 常驻区物理隔离；
//   实现复用 GXX.Core.Rtl（DelphiRTL）与 BCL，不复制第二份。

using System;
using System.Text;
using GXX.Core.Rtl;

namespace GXX.M2Server.Sweep;

/// <summary>
/// 原文 <c>PowerBase64.pas:8-10</c> 的 <c>{$IFDEF USE_VMP} VMProtectSDK {$ENDIF}</c>。
/// 接缝：待 VMProtect（或等效壳）接入后替换为真实实现（当前为无操作，不改变任何数据语义）。
/// </summary>
public static class VmpStub
{
    /// <summary>原文 <c>VMProtectBeginVirtualization(Marker)</c>。</summary>
    public static void VMProtectBeginVirtualization(string marker)
    {
        // 原文如此：仅作为壳标记，无数据语义。M2Server.dpr 未定义 USE_VMP 时该调用**根本不编译**。
    }

    /// <summary>原文 <c>VMProtectEnd()</c>。</summary>
    public static void VMProtectEnd()
    {
    }
}

/// <summary>
/// 原文 <c>PowerBase64.pas</c> 整单元：可变码表 Base64 + XOR 混淆。
/// <para>字段可见性说明：原文 <c>DecodeTable</c> 是 implementation 段单元级 var（:38），
/// 托管侧收进静态类私有字段（等价物），仅额外暴露 <see cref="DecodeTableSnapshot"/> 供单测观测。</para>
/// </summary>
public static class PowerBase64
{
    /// <summary>
    /// 原文 <c>PowerBase64.pas:32-34</c> <c>EncodeTable: array[0..63] of AnsiChar</c>。
    /// 脚本从原文抽取 + 回读比对（见 tools 记录），非手工转录。
    /// </summary>
    public const string EncodeTable = "456789#/opqrstuvQRSTUVWXYZabcdefghijklmnABCDEFGHIJKLMNOPwxyz0123";

    /// <summary>原文 <c>PowerBase64.pas:35</c> <c>DEF_FILL_CHAR = '+'</c>。</summary>
    public const char DEF_FILL_CHAR = '+';

    /// <summary>
    /// 原文 <c>PowerBase64.pas:44-45</c>：<c>CIPHER_INDEX</c> / <c>BASE_CIPHER</c>（各 16 项）。
    /// </summary>
    private static readonly byte[] CIPHER_INDEX =
        { 4, 12, 9, 15, 13, 8, 11, 5, 14, 6, 0, 7, 2, 10, 1, 3 };

    private static readonly byte[] BASE_CIPHER =
        { 0xA5, 0x5A, 0x96, 0x69, 0xAF, 0xFA, 0x5F, 0xF5, 0x9F, 0xF9, 0x6F, 0xF6, 0xAA, 0x55, 0x66, 0x99 };

    /// <summary>原文 <c>PowerBase64.pas:38</c> <c>DecodeTable: array[0..128 - 1] of Byte</c>。</summary>
    private static readonly byte[] DecodeTable = new byte[128];

    /// <summary>
    /// 原文 <c>PowerBase64.pas:370-387 InitDecodeTable</c> + <c>:389-390 initialization</c>。
    /// 静态构造函数即托管侧的 initialization 段（首次访问类型时执行一次，顺序等价）。
    /// </summary>
    static PowerBase64()
    {
        InitDecodeTable();
    }

    private static void InitDecodeTable()
    {
        VmpStub.VMProtectBeginVirtualization("PowerBase64.InitDecodeTable");

        Array.Clear(DecodeTable, 0, DecodeTable.Length);   // 原文 ZeroMemory(@DecodeTable, SizeOf)
        for (int i = 0; i <= 64 - 1; i++)
        {
            DecodeTable[(byte)EncodeTable[i]] = (byte)i;
        }

        VmpStub.VMProtectEnd();
    }

    /// <summary>仅测试用：解码表快照（原文 DecodeTable 是 implementation 段私有 var）。</summary>
    public static byte[] DecodeTableSnapshot() => (byte[])DecodeTable.Clone();

    // ------------------------------------------------------------------ XOR 混淆

    /// <summary>
    /// 原文 <c>PowerBase64.pas:40-59 XOR_encrypt(pData: PByte; dwLen: DWORD)</c>。
    /// <para>托管侧要求 <paramref name="offset"/> 之后的 <paramref name="dwLen"/> 个字节可写；
    /// <paramref name="dwLen"/> 保持原文的 **DWORD**（uint）语义 —— 原文 <c>for i := 0 to dwLen - 1</c>
    /// 在 dwLen=0 时会下溢成 $FFFFFFFF（死循环），调用方从不传 0 以外的边界值。</para>
    /// </summary>
    public static void XorEncrypt(byte[] pData, int offset, uint dwLen)
    {
        VmpStub.VMProtectBeginVirtualization("PowerBase64.XorEncrypt");

        // 原文 `for i := 0 to dwLen - 1`：DWORD 下 dwLen=0 会下溢成 $FFFFFFFF（Delphi 侧是**死循环**）。
        // 托管侧必须显式挡掉 0，否则 uint 下溢会让循环执行一次以上并越界。
        // 可观测语义等价：原文在 dwLen=0 时不可能正常返回，调用方（本单元 4 处）全部先判 nDataSize>0。
        if (dwLen == 0) { VmpStub.VMProtectEnd(); return; }

        for (uint i = 0; i <= dwLen - 1; i++)
        {
            // 原文 pData[i] := pData[i] xor BASE_CIPHER[CIPHER_INDEX[i and $F]];
            int k = offset + (int)i;
            pData[k] = (byte)(pData[k] ^ BASE_CIPHER[CIPHER_INDEX[i & 0xF]]);
        }

        VmpStub.VMProtectEnd();
    }

    /// <summary>原文 <c>PowerBase64.pas:61-64 XOR_decrypt</c>：直接转调 <c>XOR_encrypt</c>（自反）。</summary>
    public static void XorDecrypt(byte[] pData, int offset, uint dwLen) => XorEncrypt(pData, offset, dwLen);

    // ------------------------------------------------------------------ EncodeBase64

    /// <summary>
    /// 原文 <c>PowerBase64.pas:66-151 EncodeBase64(pData: Pointer; nDataSize: UINT): AnsiString</c>。
    /// <para><b>返回值长度严格照抄原文</b>：长度为 <c>(nCycle + 1) * 4 + nFillLen</c> 当 <c>nFillLen &gt; 0</c>
    ///（尾部 nFillLen 个字节是未写入的 #0，见文件头缺陷登记第 1 条），
    /// 否则 <c>nCycle * 4</c>；<c>nDataSize = 0</c> 时返回 <c>""</c>。</para>
    /// </summary>
    /// <param name="pData">源字节。</param>
    /// <param name="offset">源起始下标（原文的 pData 指针）。</param>
    /// <param name="nDataSize">源长度（UINT）。</param>
    public static string EncodeBase64(byte[] pData, int offset, int nDataSize)
    {
        string Result = "";
        uint nFillLen;
        uint nCycle;
        uint i;
        uint nData;
        byte d0, d1, d2, d3;
        int pStr = 0;                       // 原文 pStr: PAnsiChar

        if (nDataSize == 0)
            return Result;                  // 原文 exit（Result 未 SetLength → ''）

        uint nRem = (uint)nDataSize % 3;
        if (nRem != 0)
            nFillLen = 3 - nRem;
        else
            nFillLen = 0;
        nCycle = (uint)nDataSize / 3;

        if (nFillLen == 0)
        {
            // 原文 SetLength(Result, nCycle * 4)
            Result = new string('\0', (int)(nCycle * 4));
        }
        else
        {
            // 原文如此（PowerBase64.pas:88）：多算 nFillLen 个字节（尾部保持 #0）
            Result = new string('\0', (int)((nCycle + 1) * 4 + nFillLen));
        }

        var sb = new char[Result.Length];
        sb.AsSpan().Fill('\0');

        i = 0;
        while (i < nCycle)
        {
            nData = ReadUInt32Le(pData, offset);     // 原文 PUINT(pData)^（小端）
            nData &= 0x00FFFFFF;
            d0 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d1 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d2 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d3 = (byte)(nData & 0x0000003F);

            sb[pStr + 0] = EncodeTable[d0];
            sb[pStr + 1] = EncodeTable[d1];
            sb[pStr + 2] = EncodeTable[d2];
            sb[pStr + 3] = EncodeTable[d3];

            offset += 3;                            // 原文 pData := Pointer(UintPtr(pData) + 3)
            pStr += 4;
            i++;
        }

        if (nFillLen == 2)
        {
            nData = ReadUInt32Le(pData, offset);
            nData &= 0x000000FF;
            d0 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d1 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d2 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d3 = (byte)(nData & 0x0000003F);
            sb[pStr + 0] = EncodeTable[d0];
            sb[pStr + 1] = EncodeTable[d1];
            sb[pStr + 2] = EncodeTable[d2];
            sb[pStr + 3] = EncodeTable[d3];
            sb[pStr + 4] = DEF_FILL_CHAR;
            sb[pStr + 5] = DEF_FILL_CHAR;
        }
        else if (nFillLen == 1)
        {
            nData = ReadUInt32Le(pData, offset);
            nData &= 0x0000FFFF;
            d0 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d1 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d2 = (byte)(nData & 0x0000003F);
            nData >>= 6;
            d3 = (byte)(nData & 0x0000003F);
            sb[pStr + 0] = EncodeTable[d0];
            sb[pStr + 1] = EncodeTable[d1];
            sb[pStr + 2] = EncodeTable[d2];
            sb[pStr + 3] = EncodeTable[d3];
            sb[pStr + 4] = DEF_FILL_CHAR;
        }

        return new string(sb);
    }

    /// <summary>原文 <c>EncodeBase64(pData, nDataSize)</c>（offset = 0 的重载）。</summary>
    public static string EncodeBase64(byte[] pData, int nDataSize) => EncodeBase64(pData, 0, nDataSize);

    // ------------------------------------------------------------------ DecodeBase64

    /// <summary>
    /// 原文 <c>PowerBase64.pas:153-275 DecodeBase64(strBase64; pData: Pointer; nDataSize: UINT): UINT</c>。
    /// <para>返回值为原文语义：**缓冲区够则返回写入字节数；不够（或 pData=nil）则返回
    /// <c>nCycle * 3 - nFillLen</c> 这个「探测长度」**（见文件头缺陷登记第 3 条，它比真实字节数少 2）。
    /// 另：<c>nStrSize &lt; 4</c>、长度非 4 倍数、字符 &gt; $7F 三种情况返回 0。</para>
    /// </summary>
    public static uint DecodeBase64(string strBase64, byte[]? pData, int pDataOffset, int nDataSize)
    {
        uint Result;
        uint nFillLen;
        uint nCycle;
        uint i;
        byte d0, d1, d2, d3;
        byte i0, i1, i2, i3;
        uint nData;
        int pbData = pDataOffset;           // 原文 pbData: PByte
        int pch = 0;                        // 原文 pch: PAnsiChar

        VmpStub.VMProtectBeginVirtualization("PowerBase64.DecodeBase64");

        // 原文 AnsiString 按 1-based 下标；托管侧内部一律用 0-based，故所有下标 -1。
        uint nStrSize = (uint)(strBase64?.Length ?? 0);
        if (nStrSize < 4)
        {
            Result = 0;
            return Result;                  // 原文 exit
        }

        nFillLen = 0;
        if (strBase64![(int)(nStrSize - 1)] == DEF_FILL_CHAR)
        {
            nFillLen = 2;
        }
        // 原文 :180 `else if strBase64[nStrSize] = DEF_FILL_CHAR then nFillLen := 1;`
        // —— Delphi 1-based 下标下 nStrSize 与上面 :176 的 nStrSize-1 是**同一个字符**，
        //    所以这一支**永远不可达**（死分支，见文件头缺陷登记第 2 条）。
        //    托管侧保留为注释：写成可达形式会改变行为（会把长度 ≡1 mod 4 的串判成合法）。

        if (((nStrSize - nFillLen) < 4) || (((nStrSize - nFillLen) % 4) != 0))
        {
            Result = 0;
            return Result;                  // 原文 Exit
        }

        nCycle = (nStrSize - nFillLen) / 4; // 上面的判断已经确保 nCycle - nFillLen > 0（原文注释）
        uint nBufLen = nCycle * 3 - nFillLen;
        Result = nBufLen;
        if ((pData == null) || (nDataSize < nBufLen))
        {
            return Result;                  // 缓冲区长度不够，返回真实长度（原文注释）
        }

        if (nFillLen != 0)
            nCycle -= 1;                    // 如果有填充，则最后一个循环需要特殊处理

        if (nCycle > 0)
        {
            for (i = 0; i <= nCycle - 1; i++)
            {
                i0 = (byte)strBase64[pch + 0];
                i1 = (byte)strBase64[pch + 1];
                i2 = (byte)strBase64[pch + 2];
                i3 = (byte)strBase64[pch + 3];
                if ((i0 > 0x7F) || (i1 > 0x7F) || (i2 > 0x7F) || (i3 > 0x7F))
                {
                    Result = 0;
                    return Result;          // 原文 exit
                }
                d0 = DecodeTable[i0];
                d1 = DecodeTable[i1];
                d2 = DecodeTable[i2];
                d3 = DecodeTable[i3];
                nData = d3;
                nData <<= 6;
                nData |= d2;
                nData <<= 6;
                nData |= d1;
                nData <<= 6;
                nData |= d0;

                // 兼容大小端模式（原文注释）
                pData![pbData + 0] = (byte)(nData & 0xFF);
                pData[pbData + 1] = (byte)((nData >> 8) & 0xFF);
                pData[pbData + 2] = (byte)((nData >> 16) & 0xFF);
                pch += 4;
                pbData += 3;
            }
        }

        if (nFillLen != 0)
        {
            i0 = (byte)strBase64[pch + 0];
            i1 = (byte)strBase64[pch + 1];
            i2 = (byte)strBase64[pch + 2];
            i3 = (byte)strBase64[pch + 3];
            if ((i0 > 0x7F) || (i1 > 0x7F) || (i2 > 0x7F) || (i3 > 0x7F))
            {
                Result = 0;
                return Result;              // 原文 exit
            }
            d0 = DecodeTable[i0];
            d1 = DecodeTable[i1];
            d2 = DecodeTable[i2];
            d3 = DecodeTable[i3];
            nData = d3;
            nData <<= 6;
            nData |= d2;
            nData <<= 6;
            nData |= d1;
            nData <<= 6;
            nData |= d0;

            if (nFillLen == 1)
            {
                pData![pbData + 0] = (byte)(nData & 0xFF);
                pData[pbData + 1] = (byte)((nData >> 8) & 0xFF);
            }
            else if (nFillLen == 2)
            {
                pData![pbData + 0] = (byte)(nData & 0xFF);
            }
        }

        VmpStub.VMProtectEnd();
        return Result;
    }

    /// <summary>
    /// 原文 <c>DecodeBase64(strBase64, nil, 0)</c>（探测模式：只求长度/合法性，不写缓冲区）。
    /// 返回 0 表示格式非法。注意返回值对『有填充』的串比真实字节数少（原文缺陷，见文件头第 3 条）。
    /// </summary>
    public static uint DecodeBase64Probe(string strBase64)
        => DecodeBase64(strBase64, null, 0, 0);

    /// <summary>
    /// 原文 <c>DecodeBase64(strBase64, pData, nDataSize)</c> 的托管等价物（缓冲区足够时返回写入长度）。
    /// </summary>
    public static uint DecodeBase64(string strBase64, byte[] pData, int pDataOffset, int nDataSize,
        out bool bufferTooSmall)
    {
        uint probe = DecodeBase64(strBase64, null, 0, 0);
        bufferTooSmall = (pData == null) || (nDataSize < probe);
        return DecodeBase64(strBase64, pData, pDataOffset, nDataSize);
    }

    // ------------------------------------------------------------------ 组合函数（原文 :277-368）

    /// <summary>
    /// 原文 <c>PowerBase64.pas:277-295 EncryptAndEncodeBase64</c>：
    /// 先 GetMemory 拷一份，再 XOR_encrypt，再 EncodeBase64（**不改调用方缓冲区**）。
    /// </summary>
    public static string EncryptAndEncodeBase64(byte[] pData, int offset, int nDataSize)
    {
        if (nDataSize == 0)
            return "";                      // 原文 Result := ''; exit

        byte[] pLocalData = new byte[nDataSize];   // 原文 GetMemory(nDataSize)
        Array.Copy(pData, offset, pLocalData, 0, nDataSize); // 原文 CopyMemory
        XorEncrypt(pLocalData, 0, (uint)nDataSize);
        return EncodeBase64(pLocalData, 0, nDataSize);       // 原文 finally FreeMemory
    }

    /// <summary>原文 <c>EncryptAndEncodeBase64(pData, nDataSize)</c>（offset = 0 的重载）。</summary>
    public static string EncryptAndEncodeBase64(byte[] pData, int nDataSize)
        => EncryptAndEncodeBase64(pData, 0, nDataSize);

    /// <summary>
    /// 原文 <c>PowerBase64.pas:297-309 DecryptAndiDecodeBase64</c>。
    /// <para>⚠ 原文 :308 用 <paramref name="nDataSize"/>（传入缓冲区长度）而非实际解码长度做 XOR —— 逐字保留。</para>
    /// </summary>
    public static uint DecryptAndiDecodeBase64(string strBase64, byte[]? pData, int pDataOffset, int nDataSize)
    {
        uint nRealDataSize = DecodeBase64(strBase64, null, 0, 0);
        if ((pData == null) || (nDataSize < nRealDataSize))
        {
            return nRealDataSize;           // 原文 Result := nRealDataSize; Exit
        }

        uint Result = DecodeBase64(strBase64, pData, pDataOffset, nDataSize);
        XorDecrypt(pData, pDataOffset, (uint)nDataSize);   // 原文如此（:308，用 nDataSize 而非解码长度）
        return Result;
    }

    /// <summary>
    /// 原文 <c>PowerBase64.pas:311-325 EncryptAndEncodeBase64String</c>。
    /// 输入按 AnsiString 字节透明处理（Latin-1），输出同。
    /// </summary>
    public static string EncryptAndEncodeBase64String(string strSrc)
    {
        string Result;
        uint nStrSize = (uint)(strSrc?.Length ?? 0);
        if (nStrSize > 0)
        {
            byte[] buf = Latin1Bytes(strSrc!);       // 原文 Pointer(@strSrc[1])
            XorEncrypt(buf, 0, nStrSize);
            Result = EncodeBase64(buf, 0, (int)nStrSize);
        }
        else
        {
            Result = "";
        }
        return Result;
    }

    /// <summary>
    /// 原文 <c>PowerBase64.pas:327-348 DecryptAndDecodeBase64String</c>。
    /// </summary>
    public static string DecryptAndDecodeBase64String(string strBase64)
    {
        string Result;
        uint nDataSize = DecodeBase64(strBase64, null, 0, 0);
        if (nDataSize > 0)
        {
            // 原文 SetLength(Result, nDataSize)（AnsiString，内容未初始化）
            var buf = new byte[nDataSize];
            if (DecodeBase64(strBase64, buf, 0, (int)nDataSize) == 0)
            {
                Result = "";
            }
            else
            {
                XorDecrypt(buf, 0, nDataSize);  // 原文 Pointer(PAnsiChar(Result))
                Result = Latin1String(buf);
            }
        }
        else
        {
            Result = "";
        }
        return Result;
    }

    /// <summary>
    /// 原文 <c>PowerBase64.pas:350-356 EncodeBase64String</c>。
    /// ⚠ 注意 <c>nStrSize = 0</c> 时原文仍调用 <c>EncodeBase64</c>，而后者在 0 长度时 exit → 返回 ''（一致）。
    /// </summary>
    public static string EncodeBase64String(string strSrc)
    {
        uint nStrSize = (uint)(strSrc?.Length ?? 0);
        return EncodeBase64(Latin1Bytes(strSrc ?? ""), 0, (int)nStrSize);
    }

    /// <summary>
    /// 原文 <c>PowerBase64.pas:358-368 DecodeBase64String</c>。
    /// <para>注意原文 <c>SetLength(Result, nDataSize)</c> 用的是**探测长度**（有填充时比真实字节少 2），
    /// 随后 <c>DecodeBase64(..., nDataSize)</c> 也会因为 <c>nDataSize &lt; nBufLen</c> 走
    /// 「缓冲区不够 → 直接返回长度」的提前退出分支（**不写入任何字节**）→ 返回非 0 → 结果是全 #0 串。
    /// 逐字保留，测试钉死。</para>
    /// </summary>
    public static string DecodeBase64String(string strBase64)
    {
        uint nDataSize = DecodeBase64(strBase64, null, 0, 0);
        var buf = new byte[nDataSize];               // 原文 SetLength(Result, nDataSize)
        if (DecodeBase64(strBase64, buf, 0, (int)nDataSize) == 0)
        {
            return "";
        }
        return Latin1String(buf);
    }

    // ------------------------------------------------------------------ 内部工具

    /// <summary>
    /// 原文 <c>PUINT(pData)^</c>：小端读 4 字节；不足 4 字节时按原文「越界读 + 掩码丢弃高位」的
    /// **等价结果**补 0（见文件头缺陷登记第 8 条）。
    /// </summary>
    private static uint ReadUInt32Le(byte[] pData, int offset)
    {
        uint v = 0;
        for (int k = 0; k < 4; k++)
        {
            int idx = offset + k;
            if (idx < 0 || idx >= pData.Length) break;   // 越界部分补 0（原文本就靠掩码丢弃）
            v |= (uint)pData[idx] << (8 * k);
        }
        return v;
    }

    /// <summary>原文 AnsiString 的「逐字节」承载：byte[] → string（Latin-1，每字节 1 个 char）。</summary>
    private static string Latin1String(byte[] buf)
    {
        var chars = new char[buf.Length];
        for (int i = 0; i < buf.Length; i++) chars[i] = (char)buf[i];
        return new string(chars);
    }

    /// <summary>原文 AnsiString 的「逐字节」承载：string → byte[]（Latin-1）。</summary>
    private static byte[] Latin1Bytes(string s)
    {
        var buf = new byte[s.Length];
        for (int i = 0; i < s.Length; i++) buf[i] = (byte)s[i];
        return buf;
    }
}

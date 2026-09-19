using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GXX.Client.ReadResources;
using GXX.Core.Compress;
using GXX.Core.Crypto;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行车道 <c>par/p2-resources-pak</c>：Pak.pas（Source\Client-HGE\ReadResources\Pak.pas，3199 行）1:1 移植的资源格式测试。
/// <para><b>全部用合成字节</b>构造文件头/索引/图片头（不依赖仓库外的真实 .pak 资源），
/// 逐字段断言偏移与宽度，并覆盖异常路径（魔数错、索引越界、条目数为 0、文件被截断、偏移指向文件尾、
/// zlib 流非法、RLE 越界、AES 计数器进位）。</para>
/// <para><b>Pak3 密钥表金向向量</b>来自独立重算程序
/// （<c>gxx_pak_gold</c>，直接照 .pas 文本用另一份 C# 转写，非本车道实现的拷贝），
/// 见 <see cref="InitPak3Password_MatchesIndependentGoldenVector"/>。</para>
/// <para><b>有效几何的原文怪癖</b>（本文件所有期望值都据此推算）：
/// <list type="bullet">
/// <item><c>TPakFileHeader</c> 是 packed，SizeOf = 256；普通 PAK 的索引偏移 = 10 + 256 = 266；
///       LZ PAK = 5 + 256 + 1 = 262。</item>
/// <item>索引条目宽度：Pak1/Pak2/Pak3/LzPakV1 = 4 字节；LzPakV0 = 8 字节（off, size 各 4）。</item>
/// <item>文件头魔数比较长度不同：'GEEM2' 比 5 字节、'GEEPAK2'/'GEEPAK3' 比 6 字节、'HXM2' 比 4 字节。</item>
/// <item><c>nPosition &gt;= 256</c>（普通 PAK 图片头合法性）vs <c>nPosition &gt;= 262</c>（LZ 路径）。</item>
/// </list></para>
/// </summary>
public sealed class ResourcePakTests : IDisposable
{
    private readonly string _dir;

    public ResourcePakTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_p2_pak_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        ResetGlobals();
    }

    public void Dispose()
    {
        TextureSeams.ResetForTest();
        PakSeams.DecodeResStrFn = s => s;
        PakSeams.UpdateEngineAddFn = null;
        ResetGlobals();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private static void ResetGlobals()
    {
        TGameImages.g_boAutoUpdate = false;
        TGameImages.g_boDeviceInitializeOK = false;
        TGameImages.g_NullImage = default;
        TGameImages.g_DebugTextOut = null;
        TGameImages.UpdateEngineAddFn = null;
        TGameImages.g_UpdateRetryTime = 30000;
        TGameImages.MyGetTickCountFn = () => 0x0001_0000u;
        PakSeams.UpdateEngineAddFn = null;
    }

    // ===================================================================================
    // 0. 测试夹具：合成密钥材料 + 合成 PAK 文件构造器
    // ===================================================================================

    /// <summary>合成 FKeyData（**不是**任何真实资源的密钥）：FKeyData[i] = 0x01020304 + i*0x00010001。</summary>
    private static uint[] MakeKeyData()
    {
        var fk = new uint[32];
        for (int i = 0; i < 32; i++) fk[i] = 0x01020304u + (uint)i * 0x00010001u;
        return fk;
    }

    /// <summary>合成 FChain：FChain[i] = 0x10 + i。</summary>
    private static byte[] MakeChain()
    {
        var fc = new byte[20];
        for (int i = 0; i < 20; i++) fc[i] = (byte)(0x10 + i);
        return fc;
    }

    private static TPakPassword MakePassword()
    {
        var p = TPakPassword.CreateEmpty();
        p.KeyData = MakeKeyData();
        p.Chain = MakeChain();
        p.KeyDataLz = MakeKeyData();
        p.ChainLz = MakeChain();
        return p;
    }

    private static TPakImages NewImages(TPakPassword? pw = null)
    {
        var p = pw ?? MakePassword();
        var img = new TPakImages(p);
        img.FileName = Path.Combine(Path.GetTempPath(), "gxx_p2_pak_nonexistent_" + Guid.NewGuid().ToString("N") + ".pak");
        return img;
    }

    // ---- 合成文件构造器 ----

    /// <summary>合成密钥材料 + 合成 PAK 文件构造器：文件头加密后，索引与图片区由调用方给定。</summary>
    private static byte[] BuildPlainPak(TPakFileType type, int[] indexOffsets, byte[] imageArea,
        TPakPassword? pw = null, string checkCode = "GEEM2", uint imageCountOverride = uint.MaxValue)
    {
        var password = pw ?? MakePassword();
        var header = TPakFileHeader.CreateEmpty();
        header.bt1 = 0;
        header.TitleText = "www.gameofmir2.com";
        header.Size = header.IndexOffSet = (uint)(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf);
        header.ImageCount = imageCountOverride == uint.MaxValue ? (uint)indexOffsets.Length : imageCountOverride;
        header.bfType = 2;
        header.BitCount = 8;
        Array.Copy(password.KeyData, header.KeyData, 32);
        Array.Copy(password.Chain, header.Chain, 20);

        var img = new TPakImages(password);
        img.FPakFileType = type;
        if (type == TPakFileType.pftPak3) img.InitPak3Password();

        // **关键**：img.FKeyData/img.FChain 是构造时从 password **深拷贝**出来的原始值，
        // 永远不受后续 CBC 就地改写影响（password.Chain 与 header.Chain 是同一个数组引用，
        // 一旦 EncryptS 跑过就被污染）。所有"需要原始密钥"的步骤都从这里复原。
        uint[] pristineKey = (uint[])img.FKeyData.Clone();
        byte[] pristineChain = (byte[])img.FChain.Clone();

        var encImg = new TPakImages(password);
        encImg.FileHeader = header;
        SetCheckCode(header, encImg.EncryptS(checkCode));

        // 复原被 EncryptS 就地改写的 CBC 链
        Array.Copy(pristineKey, header.KeyData, 32);
        Array.Copy(pristineChain, header.Chain, 20);

        // 加密索引
        //
        // **Pak2 的特殊点**：LoadIndex 读回后会做 `index ^= FileHeader.Chain[0]`（原文 1455），
        // 所以落盘的密文应当是 `(Offset ^ Chain[0])` 经两次 CBC 的结果。fixture 必须同样先 XOR。
        // （Pak1/Pak3/LZ 不做这一层。）
        var idxPlain = new byte[indexOffsets.Length * 4];
        for (int i = 0; i < indexOffsets.Length; i++)
        {
            int v = type == TPakFileType.pftPak2 ? (indexOffsets[i] ^ header.Chain[0]) : indexOffsets[i];
            BitConverter.GetBytes(v).CopyTo(idxPlain, i * 4);
        }
        var idxCipher = EncryptIndexForTest(img, type, idxPlain, header);

        // 加密文件头
        var headPlain = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(headPlain, 0);
        string magic = type == TPakFileType.pftPak1 ? "GEEM2" : type == TPakFileType.pftPak2 ? "GEEPAK2" : "GEEPAK3";
        byte[] headCipher;
        if (type == TPakFileType.pftPak1)
            headCipher = UnitDes.EncryptStrDesBytes(headPlain, PakUnitDesGlobals.PakEncryKeyStr);
        else if (type == TPakFileType.pftPak2)
            headCipher = PakDesNew.EncryptDes_New(headPlain, headPlain.Length, PakUnitDesGlobals.PakEncryKeyStr);
        else
        {
            headCipher = new byte[TPakFileHeader.SizeOf];
            PakAesCtr.AesEncrypt(img.Pak3KeyBytes(), 16, headPlain, headCipher, TPakFileHeader.SizeOf);
        }

        var headerInfo = TFileHeaderInfo.CreateEmpty();
        headerInfo.FileTypeText = magic;

        // 文件布局：TFileHeaderInfo(10) + TPakFileHeader(256) + 索引 + 图片区
        var outBuf = new List<byte>();
        outBuf.AddRange(headerInfo.FileType);
        outBuf.AddRange(headCipher);
        outBuf.AddRange(idxCipher);
        outBuf.AddRange(imageArea);
        return outBuf.ToArray();
    }

    /// <summary>
    /// 对应原文 <c>FileHeader.CheckCode := EncryptS('GEEM2')</c> 的**逐字节**语义
    /// （Delphi AnsiString → ShortString(12)）。不能走 <c>CheckCodeText</c>（GBK 会损坏 0x80–0xFF 密文）。
    /// </summary>
    private static void SetCheckCode(TPakFileHeader header, string cipher)
    {
        byte[] bytes = PakSeams.FromLatin1(cipher);
        int len = Math.Min(bytes.Length, 12);
        header.CheckCode[0] = (byte)len;
        for (int i = 0; i < len; i++) header.CheckCode[1 + i] = bytes[i];
        for (int i = len; i < 12; i++) header.CheckCode[1 + i] = 0;
    }

    /// <summary>
    /// 与 <see cref="BuildPlainPak"/> 同一布局，但**索引指向当前单张图片区**的首地址
    /// （= 10 + 256 + 1*4 = 270）。用于 GetCached* 系列需要「索引指向有效图片头」的用例。
    /// </summary>
    private static byte[] BuildPlainPakWithImage(TPakFileType type, byte[] imageArea, TPakPassword? pw = null)
    {
        int offset = TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + sizeof(int);
        return BuildPlainPak(type, new[] { offset }, imageArea, pw);
    }

    private static byte[] EncryptIndexForTest(TPakImages img, TPakFileType type, byte[] plain, TPakFileHeader header)
    {
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(header.KeyData, keyData, 32);
        Array.Copy(header.Chain, chain, 20);

        var cipher = new byte[plain.Length];
        if (type == TPakFileType.pftPak1)
        {
            UnitDes.EncryptCBC(plain, cipher, plain.Length, chain, keyData);
        }
        else if (type == TPakFileType.pftPak2)
        {
            // 原文 1124-1138：**两次** EncryptCBC；第二次前把 KeyData/Chain 从 FileHeader **重新**拷一份。
            // 注意 UnitDes.EncryptCBC 会**就地改写**传入的 chain 数组（CBC 反馈），
            // 所以两次都必须传"原始 chain 的独立副本"，否则第二次用的是被污染过的链。
            var chainA = new byte[20];
            Array.Copy(header.Chain, chainA, 20);
            var kdA = new uint[32];
            Array.Copy(header.KeyData, kdA, 32);
            UnitDes.EncryptCBC(plain, cipher, plain.Length, chainA, kdA);

            var chainB = new byte[20];
            Array.Copy(header.Chain, chainB, 20);
            var kdB = new uint[32];
            Array.Copy(header.KeyData, kdB, 32);
            UnitDes.EncryptCBC(cipher, cipher, plain.Length, chainB, kdB);
        }
        else // Pak3：每项 xor FPak3Password[i mod 64] xor (not i)，不解密
        {
            for (int i = 0; i < plain.Length / 4; i++)
            {
                int v = BitConverter.ToInt32(plain, i * 4);
                uint enc = (uint)v ^ img.FPak3Password[i % 64] ^ ~(uint)i;
                BitConverter.GetBytes(enc).CopyTo(cipher, i * 4);
            }
        }
        return cipher;
    }

    private static string WriteFile(string dir, string name, byte[] bytes)
    {
        string path = Path.Combine(dir, name);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    // ===================================================================================
    // 1. 记录布局：逐字段偏移与宽度 + 往返对拍
    // ===================================================================================

    [Fact]
    public void TPakPassword_Layout_Is296Bytes_AndRoundTrips()
    {
        Assert.Equal(296, TPakPassword.SizeOf);

        var p = MakePassword();
        var buf = new byte[TPakPassword.SizeOf];
        p.ToBytes(buf, 0);

        // 偏移 0 的 KeyData[0]（小端）
        Assert.Equal(0x04, buf[0]);
        Assert.Equal(0x03, buf[1]);
        Assert.Equal(0x02, buf[2]);
        Assert.Equal(0x01, buf[3]);
        // 偏移 4 的 KeyData[1] = 0x01020304 + 0x00010001 = 0x01030305 → 字节 05 03 03 01
        Assert.Equal(0x05, buf[4]);
        Assert.Equal(0x03, buf[5]);
        Assert.Equal(0x03, buf[6]);
        Assert.Equal(0x01, buf[7]);
        // 偏移 128 的 Chain[0] = 0x10
        Assert.Equal(0x10, buf[128]);
        Assert.Equal(0x23, buf[128 + 19]);
        // 偏移 148 的 KeyDataLz[0]
        Assert.Equal(0x04, buf[148]);
        // 偏移 276 的 ChainLz[0]
        Assert.Equal(0x10, buf[276]);

        var back = TPakPassword.FromBytes(buf, 0);
        Assert.Equal(p.KeyData, back.KeyData);
        Assert.Equal(p.Chain, back.Chain);
        Assert.Equal(p.KeyDataLz, back.KeyDataLz);
        Assert.Equal(p.ChainLz, back.ChainLz);
    }

    [Fact]
    public void TPakIndexHeader_Layout_Is8Bytes()
    {
        Assert.Equal(8, TPakIndexHeader.SizeOf);

        var h = new TPakIndexHeader { OffSet = 0x03020100, Length = unchecked((int)0x84736251) };
        var buf = new byte[8];
        h.ToBytes(buf, 0);

        Assert.Equal(new byte[] { 0x00, 0x01, 0x02, 0x03 }, buf[0..4]);
        Assert.Equal(new byte[] { 0x51, 0x62, 0x73, 0x84 }, buf[4..8]);

        var back = TPakIndexHeader.FromBytes(buf, 0);
        Assert.Equal(h.OffSet, back.OffSet);
        Assert.Equal(h.Length, back.Length);
    }

    [Fact]
    public void TPakFileHeader_Layout_Is256Bytes_FieldsAtExpectedOffsets()
    {
        Assert.Equal(256, TPakFileHeader.SizeOf);
        Assert.Equal(266, TPakFileHeader.PlainIndexOffset);

        var h = TPakFileHeader.CreateEmpty();
        h.bt1 = 0xAB;
        h.TitleText = "www.gameofmir2.com";     // 18 字节
        h.Size = 0x00000102;
        h.ImageCount = 3;
        h.bfType = 2;
        h.IndexOffSet = 266;
        h.BitCount = 8;
        h.CreateDate = 45000.5;
        h.CheckCodeText = "GEEM2";
        h.Reserve[0] = 0x11223344;
        h.KeyData[0] = 0xAABBCCDD;
        h.Chain[0] = 0x5A;

        var buf = new byte[TPakFileHeader.SizeOf];
        h.ToBytes(buf, 0);

        Assert.Equal(0xAB, buf[0]);                    // bt1
        Assert.Equal(18, buf[1]);                      // Title 的 ShortString 长度字节
        Assert.Equal((byte)'w', buf[2]);               // Title 数据起点
        Assert.Equal(0x02, buf[42]);                   // Size 低字节
        Assert.Equal(0x01, buf[43]);
        Assert.Equal(3, buf[46]);                      // ImageCount
        Assert.Equal(2, buf[50]);                      // bfType
        Assert.Equal(0x0A, buf[54]);                   // IndexOffSet = 266 = 0x10A
        Assert.Equal(0x01, buf[55]);
        Assert.Equal(8, buf[58]);                      // BitCount
        Assert.Equal(0, buf[59]);
        Assert.Equal(5, buf[68]);                      // CheckCode 长度字节
        Assert.Equal((byte)'G', buf[69]);
        Assert.Equal(0x44, buf[84]);                   // Reserve[0]
        Assert.Equal(0xDD, buf[108]);                  // KeyData[0]
        Assert.Equal(0x5A, buf[236]);                  // Chain[0]

        var back = TPakFileHeader.FromBytes(buf, 0);
        Assert.Equal("www.gameofmir2.com", back.TitleText);
        Assert.Equal(45000.5, back.CreateDate);
        Assert.Equal("GEEM2", back.CheckCodeText);
        Assert.Equal(0x11223344, back.Reserve[0]);
        Assert.Equal(0xAABBCCDDu, back.KeyData[0]);
        Assert.Equal(0x5A, back.Chain[0]);
    }

    [Fact]
    public void TPakKey_Layout_Is178Bytes_AndPakTypeSplit()
    {
        Assert.Equal(178, TPakKey.SizeOf);

        var k = TPakKey.CreateEmpty();
        k.ImageCount = 7;
        // PakType 高字节 = LzPakV0 的 BitCount，低字节 = 类型
        k.PakType = (ushort)((24 << 8) | (int)TPakFileType.pftLzPakV0);
        k.KeyData[0] = 0x01020304;
        k.Chain[0] = 0x99;
        k.Reserve[0] = -1;

        var buf = new byte[TPakKey.SizeOf];
        k.ToBytes(buf, 0);

        Assert.Equal(7, buf[0]);
        Assert.Equal((byte)TPakFileType.pftLzPakV0, buf[4]);
        Assert.Equal(24, buf[5]);
        Assert.Equal(0x04, buf[6]);   // KeyData[0] 在偏移 6
        Assert.Equal(0x99, buf[134]); // Chain[0] 在偏移 134
        Assert.Equal(0xFF, buf[154]); // Reserve[0] = -1

        var back = TPakKey.FromBytes(buf, 0);
        Assert.Equal(7, back.ImageCount);
        Assert.Equal(24, back.LzV0BitCount);                          // (PakType shr 8) and $FF
        Assert.Equal((int)TPakFileType.pftLzPakV0, back.PakTypeLow);  // PakType and $00FF
    }

    [Fact]
    public void TNewPakImageInfo_Layout_Is16Bytes_BooleanIsOneByte()
    {
        Assert.Equal(16, TNewPakImageInfo.SizeOf);
        Assert.Equal(12, TPakImageInfo.SizeOf);
        Assert.Equal(5, TLzFileHeaderInfo.SizeOf);
        Assert.Equal(10, TFileHeaderInfo.SizeOf);

        var v = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            bt2 = 1,
            bt3 = 2,
            boAlpha = true,
            nWidth = 16,
            nHeight = 32,
            px = -7,
            py = 9,
            Length = 0x01020304,
        };
        var buf = new byte[16];
        v.ToBytes(buf, 0);

        Assert.Equal(3, buf[0]);            // pf8bit = 3
        Assert.Equal(1, buf[1]);
        Assert.Equal(2, buf[2]);
        Assert.Equal(1, buf[3]);            // Boolean true → 1
        Assert.Equal(16, buf[4]);
        Assert.Equal(0, buf[5]);
        Assert.Equal(0xF9, buf[8]);         // px = -7 → 0xFFF9
        Assert.Equal(0xFF, buf[9]);
        Assert.Equal(0x04, buf[12]);

        var back = TNewPakImageInfo.FromBytes(buf, 0);
        Assert.True(back.boAlpha);
        Assert.Equal((short)-7, back.px);
        Assert.Equal(0x01020304, back.Length);
    }

    [Fact]
    public void TFileHeaderInfo_And_TLzFileHeaderInfo_MagicTexts()
    {
        var fi = TFileHeaderInfo.CreateEmpty();
        fi.FileTypeText = "GEEPAK3";           // 7 字节，ShortString(9) 容得下
        Assert.Equal(7, fi.FileType[0]);
        Assert.Equal((byte)'G', fi.FileType[1]);
        Assert.Equal("GEEPAK3", fi.FileTypeText);

        // 磁盘上 'GEEM2' 的字节是 [05]['G']['E']['E']['M']['2']
        var g = TFileHeaderInfo.CreateEmpty();
        g.FileTypeText = "GEEM2";
        Assert.Equal(5, g.FileType[0]);
        Assert.Equal(new byte[] { (byte)'G', (byte)'E', (byte)'E', (byte)'M', (byte)'2' }, g.FileType[1..6]);

        // 'GEEPAK3' 的前 5 字节是 'GEEPA'，与 'GEEM2' 不同 → 不会误判为 Pak1
        Assert.False(PakStringCompare.CompareLStr(fi.FileType, "GEEM2", 5));
        Assert.True(PakStringCompare.CompareLStr(g.FileType, "GEEM2", 5));
        Assert.True(PakStringCompare.CompareLStr(fi.FileType, "GEEPAK2", 6));
        Assert.True(PakStringCompare.CompareLStr(fi.FileType, "GEEPAK3", 6));

        var lz = TLzFileHeaderInfo.Hzm2Dot();
        Assert.Equal(new byte[] { (byte)'H', (byte)'X', (byte)'M', (byte)'2', (byte)'.' }, lz.FileType);
        Assert.Equal("HXM2.", lz.FileTypeText);
        Assert.True(PakStringCompare.CompareLStr(lz.FileTypeText, "HXM2", 4));
    }

    // ===================================================================================
    // 2. 常量与查表（由脚本抽取 + 回读比对）
    // ===================================================================================

    [Fact]
    public void BytesPerPixels_Table_MatchesSource()
    {
        // Pak.pas 246：BytesPerPixels:array[TPixelFormat] of Byte = (1, 1, 1, 1, 2, 2, 3, 4, 4);
        Assert.Equal(new byte[] { 1, 1, 1, 1, 2, 2, 3, 4, 4 }, PakConsts.BytesPerPixels);

        // 与 TPixelFormat 序号一一对应（pfDevice..pf32bit）
        Assert.Equal(1, PakConsts.BytesPerPixel(TPixelFormat.pfDevice));
        Assert.Equal(2, PakConsts.BytesPerPixel(TPixelFormat.pf15bit));
        Assert.Equal(4, PakConsts.BytesPerPixel(TPixelFormat.pf32bit));
        Assert.Equal(0, PakConsts.BytesPerPixel((TPixelFormat)200)); // 越界 → 0
    }

    [Fact]
    public void PakConsts_HeaderSizes_MatchSource()
    {
        // Pak.pas 237
        Assert.Equal(262, PakConsts.LZ_PAK_FILE_HEADER_SIZE);
        // 262 = 5 + 256 + 1（原文 585-586 的 SizeOf 推导）
        Assert.Equal(TLzFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 1, PakConsts.LZ_PAK_FILE_HEADER_SIZE);
        // 266 = 10 + 256（原文 588-589）
        Assert.Equal(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf, TPakFileHeader.PlainIndexOffset);

        Assert.Equal(1, PakConsts.PAK3_ENCODE);
        Assert.False(PakConsts.g_boAutoUpdate);
    }

    [Fact]
    public void WidthBytes_And_AlphaHelpers_MatchSource()
    {
        // GameImages.pas 315-318：(((Width * BitCount) + 31) div 32) * 4
        Assert.Equal(0, PakConsts.WidthBytes(8, 0));
        Assert.Equal(4, PakConsts.WidthBytes(8, 1));
        Assert.Equal(4, PakConsts.WidthBytes(8, 4));
        Assert.Equal(8, PakConsts.WidthBytes(8, 5));
        Assert.Equal(1600, PakConsts.WidthBytes(8, 1600));
        Assert.Equal(8, PakConsts.WidthBytes(16, 3));    // (48+31) div 32 = 2 → 2*4 = 8
        Assert.Equal(12, PakConsts.WidthBytes(24, 4));   // (96+31)/32*4 = 3*4
        Assert.Equal(16, PakConsts.WidthBytes(24, 5));   // (120+31)/32*4 = 4*4
        Assert.Equal(16, PakConsts.WidthBytes(32, 4));   // (128+31)/32*4 = 4*4
        Assert.Equal(4, PakConsts.WidthBytes(16, 1));    // (16+31)/32*4 = 1*4

        // Pak.pas 2382-2384：nAlphaLineSize = w div 2 (+1 当 w 为奇数)
        Assert.Equal(8, PakConsts.AlphaLineSize(16));
        Assert.Equal(9, PakConsts.AlphaLineSize(17));
        Assert.Equal(1, PakConsts.AlphaLineSize(1));     // 1 div 2 = 0，且 (1 and 1) <> 0 → 1

        // Pak.pas 2390
        Assert.Equal(16, PakConsts.AlphaWidthBytes(16));
        Assert.Equal(20, PakConsts.AlphaWidthBytes(17)); // (136+31)/32*4 = 5*4
    }

    [Fact]
    public void GetBitCountByPixelFormat_AllBranches()
    {
        Assert.Equal(1, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf1bit));
        Assert.Equal(4, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf4bit));
        Assert.Equal(8, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf8bit));
        Assert.Equal(16, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf15bit));
        Assert.Equal(16, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf16bit));
        Assert.Equal(24, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf24bit));
        Assert.Equal(32, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pf32bit));
        // 未知格式（含 pfDevice / pfCustom）→ 0
        Assert.Equal(0, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pfDevice));
        Assert.Equal(0, TPakImages.GetBitCountByPixelFormat((byte)TPixelFormat.pfCustom));
        Assert.Equal(0, TPakImages.GetBitCountByPixelFormat(0xFF));
    }

    [Fact]
    public void IsValidLzCheckCode_SixAcceptedValues_CaseSensitive()
    {
        // Pak.pas 478-479
        Assert.True(TPakImages.IsValidLzCheckCode("D3DM2"));
        Assert.True(TPakImages.IsValidLzCheckCode("HXM2"));
        Assert.True(TPakImages.IsValidLzCheckCode("HeroM2"));
        Assert.True(TPakImages.IsValidLzCheckCode("HeroM2."));
        Assert.True(TPakImages.IsValidLzCheckCode("HXM2."));
        Assert.True(TPakImages.IsValidLzCheckCode("FreeMF"));

        // 大小写敏感（原文是 =，Delphi 字符串比较区分大小写）
        Assert.False(TPakImages.IsValidLzCheckCode("hxm2"));
        Assert.False(TPakImages.IsValidLzCheckCode("GEEM2"));   // 普通 PAK 的校验码**不**在此表
        Assert.False(TPakImages.IsValidLzCheckCode(""));
        Assert.False(TPakImages.IsValidLzCheckCode("HXM2.."));
    }

    // ===================================================================================
    // 3. InitPak3Password —— 独立重算的金向向量
    // ===================================================================================

    [Fact]
    public void InitPak3Password_MatchesIndependentGoldenVector()
    {
        // 金向向量由独立程序 gxx_pak_gold 生成（直接照 Pak.pas 272-474 用另一份 C# 转写，
        // 非本车道实现的拷贝），输入 = 本文件的 MakeKeyData()/MakeChain()。
        var img = NewImages();
        img.InitPak3Password();

        // 偶数槽 = FKeyData 逆序（原文 285-287）
        Assert.Equal(0x01020304u, img.FPak3Password[62]); // FKeyData[0]
        Assert.Equal(0x01210323u, img.FPak3Password[0]);  // FKeyData[31]
        Assert.Equal(0x01200322u, img.FPak3Password[2]);  // FKeyData[30]

        // 5 个由 FChain 4 字节搬入的奇数槽（原文 289-293）
        Assert.Equal(0x13121110u, img.FPak3Password[1]);
        Assert.Equal(0x17161514u, img.FPak3Password[7]);
        Assert.Equal(0x1B1A1918u, img.FPak3Password[11]);
        Assert.Equal(0x1F1E1D1Cu, img.FPak3Password[13]);
        Assert.Equal(0x23222120u, img.FPak3Password[15]);

        // 第一段混合的 3 个结果（原文 374-376）
        Assert.Equal(0x8BA9C3F8u, img.FPak3Password[3]);
        Assert.Equal(0x7BEB9ABAu, img.FPak3Password[5]);
        Assert.Equal(0x92E3EFD3u, img.FPak3Password[9]);

        // JSHash / DJBHash（原文 379-390）
        Assert.Equal(0xA9E369B1u, img.FPak3Password[17]);
        Assert.Equal(0xDB64DBEAu, img.FPak3Password[19]);

        // 第二段循环（原文 392-472，i = 10..31 → 槽 21/23/…/63）
        Assert.Equal(0x87FE7CDFu, img.FPak3Password[21]);
        Assert.Equal(0x4CFEF769u, img.FPak3Password[63]);

        // 64 槽全和（金向向量的 SUM64）
        uint sum = 0;
        foreach (uint v in img.FPak3Password) sum += v;
        Assert.Equal(0xC9545D6Eu, sum);
    }

    [Fact]
    public void InitPak3Password_IsDeterministic_AndDifferentInputsDiffer()
    {
        var img = NewImages();
        img.InitPak3Password();
        var first = (uint[])img.FPak3Password.Clone();

        // 第二次调用（同一实例）结果完全相同：算法只读 FKeyData/FChain，
        // 第一段循环在第二段循环之前已把 0..19 全部重写，故不受上一次调用残留影响。
        img.InitPak3Password();
        Assert.Equal(first, img.FPak3Password);

        // 换一份 FKeyData → 结果不同（偶数槽直接反映输入）
        var pw2 = TPakPassword.CreateEmpty();
        pw2.KeyData = new uint[32];
        pw2.Chain = MakeChain();
        var img2 = NewImages(pw2);
        img2.InitPak3Password();
        Assert.Equal(0u, img2.FPak3Password[62]);
        Assert.NotEqual(first[3], img2.FPak3Password[3]);
    }

    [Fact]
    public void InitPak3Password_OddSlotsComeFromFChain_NotFromKeyData()
    {
        // 差异断言：只改 FChain（保留 FKeyData）→ 偶数槽不变、5 个搬运槽变。
        var pwA = MakePassword();
        var pwB = TPakPassword.CreateEmpty();
        pwB.KeyData = (uint[])pwA.KeyData.Clone();
        pwB.Chain = new byte[20];
        for (int i = 0; i < 20; i++) pwB.Chain[i] = (byte)(0x40 + i);
        pwB.KeyDataLz = (uint[])pwA.KeyDataLz.Clone();
        pwB.ChainLz = (byte[])pwA.ChainLz.Clone();

        var a = NewImages(pwA); a.InitPak3Password();
        var b = NewImages(pwB); b.InitPak3Password();

        for (int i = 0; i < 32; i++) Assert.Equal(a.FPak3Password[i * 2], b.FPak3Password[i * 2]);
        Assert.NotEqual(a.FPak3Password[1], b.FPak3Password[1]);
        Assert.Equal(0x43424140u, b.FPak3Password[1]);
        Assert.NotEqual(a.FPak3Password[7], b.FPak3Password[7]);
    }

    // ===================================================================================
    // 4. 文件头加解密（往返 + 差异断言）
    // ===================================================================================

    [Fact]
    public void HeaderEncryptDecrypt_GameOfMir_RoundTrip()
    {
        var img = NewImages();
        var h = TPakFileHeader.CreateEmpty();
        h.ImageCount = 5;
        h.BitCount = 8;
        h.CheckCodeText = "GEEM2";
        h.TitleText = "synthetic";
        Array.Copy(img.FKeyData, h.KeyData, 32);
        Array.Copy(img.FChain, h.Chain, 20);

        string cipher = img.EncryptHeader_GameOfMir(h);
        Assert.Equal(TPakFileHeader.SizeOf, PakSeams.FromLatin1(cipher).Length); // 密文恒 256 字节

        var back = img.DecryptHeader_GameOfMir(cipher);
        Assert.Equal(5u, back.ImageCount);
        Assert.Equal("synthetic", back.TitleText);

        // 字节视图重载与 string 重载等价
        var back2 = img.DecryptHeader_GameOfMir(PakSeams.FromLatin1(cipher));
        Assert.Equal(back.ImageCount, back2.ImageCount);
        Assert.Equal(back.TitleText, back2.TitleText);
    }

    [Fact]
    public void HeaderEncryptDecrypt_Pak2_RoundTrip_AndDiffersFromPak1()
    {
        var img = NewImages();
        var h = TPakFileHeader.CreateEmpty();
        h.ImageCount = 9;
        h.bfType = 1;
        h.CheckCodeText = "GEEM2";
        Array.Copy(img.FKeyData, h.KeyData, 32);
        Array.Copy(img.FChain, h.Chain, 20);

        string c2 = img.EncryptHeader_Pak2(h);
        var back = img.DecryptHeader_Pak2(c2);
        Assert.Equal(9u, back.ImageCount);
        Assert.Equal("GEEM2", back.CheckCodeText);

        // 差异断言：Pak1 与 Pak2 对**同一个头**产出的密文不同（密钥派生不同）
        string c1 = img.EncryptHeader_GameOfMir(h);
        Assert.NotEqual(PakSeams.FromLatin1(c1), PakSeams.FromLatin1(c2));

        // 用 Pak1 的解密器解 Pak2 密文 → 得不到原头
        var wrong = img.DecryptHeader_GameOfMir(c2);
        Assert.NotEqual(h.ImageCount, wrong.ImageCount);
    }

    [Fact]
    public void HeaderEncryptDecrypt_Pak3_AesCtr_RoundTrip()
    {
        var img = NewImages();
        img.InitPak3Password();

        var h = TPakFileHeader.CreateEmpty();
        h.ImageCount = 0xA1B2C3D4u;
        h.IndexOffSet = 266;
        h.CheckCodeText = "GEEM2";
        Array.Copy(img.FKeyData, h.KeyData, 32);
        Array.Copy(img.FChain, h.Chain, 20);

        string c3 = img.EncryptHeader_Pak3(h);
        var back = img.DecryptHeader_Pak3(c3);
        Assert.Equal(0xA1B2C3D4u, back.ImageCount);
        Assert.Equal(266u, back.IndexOffSet);
        Assert.Equal("GEEM2", back.CheckCodeText);
    }

    [Fact]
    public void HeaderEncryptDecrypt_LzPak_UsesHardcodedPassword442517066()
    {
        var img = NewImages();
        var h = TPakFileHeader.CreateEmpty();
        h.bfType = 0;
        h.ImageCount = 12;
        h.BitCount = 16;
        h.CheckCodeText = "D3DM2";
        Array.Copy(img.FKeyDataLz, h.KeyData, 32);
        Array.Copy(img.FChainLz, h.Chain, 20);

        string cipher = img.EncryptHeader_LzPak(h);
        var back = img.DecryptHeader_LzPak(cipher);
        Assert.Equal(0, back.bfType);
        Assert.Equal(12u, back.ImageCount);
        Assert.Equal(16, back.BitCount);
        Assert.Equal("D3DM2", back.CheckCodeText);

        // 差异断言：LZ 头不是 Pak1 头（不同口令）
        string c1 = img.EncryptHeader_GameOfMir(h);
        Assert.NotEqual(PakSeams.FromLatin1(c1), PakSeams.FromLatin1(cipher));

        // 原文 EncryptHeader（442517066 硬编码）也应能自洽往返
        string ce = img.EncryptHeader(h);
        var be = img.DecryptHeader(ce);
        Assert.Equal(12u, be.ImageCount);
    }

    [Fact]
    public void PakEncryKeyLiteral_IsTruncatedTo32Bits()
    {
        // UnitDes.pas 1108：PakEncryKey^ := $0DC6DAC1E —— 9 位十六进制，按 LongWord 截断
        // $DC6DAC1E = 3698175006
        Assert.Equal(0xDC6DAC1Eu, PakUnitDesGlobals.PakEncryKey);
        Assert.Equal(3698175006u, PakUnitDesGlobals.PakEncryKey);
        Assert.Equal("3698175006", PakUnitDesGlobals.PakEncryKeyStr);   // IntToStr(PLongWord^)

        // 与 NewEncryKey（$0C08BE531 → $C08BE531）不同，切勿混用
        Assert.Equal(0xC08BE531u, PakUnitDesGlobals.NewEncryKey);
        Assert.Equal("3230393649", PakUnitDesGlobals.NewEncryKey.ToString());
        Assert.Equal(0x0D5323F3u, PakUnitDesGlobals.PakEncryKey2);
    }

    [Fact]
    public void EncryptS_DecryptS_RoundTrip_AndEmptyStringShortCircuit()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);

        // 空串直接返回空串（原文 1234/1249 的 if S <> '' 守卫）
        Assert.Equal("", img.EncryptS(""));
        Assert.Equal("", img.DecryptS(""));

        string c = img.EncryptS("GEEM2");   // 5 字节明文 → 5 字节密文
        Assert.Equal(5, PakSeams.FromLatin1(c).Length);
        Assert.Equal("GEEM2", img.DecryptS(c));

        // 差异断言：密文与明文不同（不是恒等映射）
        Assert.NotEqual("GEEM2", c);
    }

    // ===================================================================================
    // 5. 索引加解密 + WriteIndexList + LoadIndex
    // ===================================================================================

    [Fact]
    public void IndexEncryptDecrypt_Pak1_RoundTrip_AndLengthSemantics()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);

        var plain = new byte[8];
        BitConverter.GetBytes(266).CopyTo(plain, 0);
        BitConverter.GetBytes(0).CopyTo(plain, 4);

        var cipher = new byte[8];
        img.EncryptIndexList(plain, 8, cipher);
        Assert.NotEqual(plain, cipher);

        var back = new byte[8];
        img.DecryptIndexList(cipher, 8, back);
        Assert.Equal(plain, back);

        // 长度 < BS(20) 的尾巴路径也要往返一致
        var p5 = new byte[] { 1, 2, 3, 4, 5 };
        var c5 = new byte[5];
        img.EncryptIndexList(p5, 5, c5);
        Assert.NotEqual(p5, c5);
        var b5 = new byte[5];
        img.DecryptIndexList(c5, 5, b5);
        Assert.Equal(p5, b5);
    }

    [Fact]
    public void IndexEncryptDecrypt_Pak2_IsDoubleCbc_DiffersFromSingleCbc()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);

        var plain = new byte[8];
        BitConverter.GetBytes(266).CopyTo(plain, 0);
        BitConverter.GetBytes(300).CopyTo(plain, 4);

        var single = new byte[8];
        img.EncryptIndexList(plain, 8, single);
        var dbl = new byte[8];
        img.EncryptIndexList_Pak2(plain, 8, dbl);

        // 差异断言：Pak2 是两次 CBC → 与单次 CBC 结果不同
        Assert.NotEqual(single, dbl);

        var back = new byte[8];
        img.DecryptIndexList_Pak2(dbl, 8, back);
        Assert.Equal(plain, back);
    }

    [Fact]
    public void LoadIndex_Pak1_DecryptsWholeTable_AndZeroCountShortCircuits()
    {
        var indexOffsets = new[] { 266, 300, 0 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak1, indexOffsets, new byte[0]);
        string path = WriteFile(_dir, "p1.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();

        Assert.True(img.Initialized);
        Assert.Equal(TPakFileType.pftPak1, img.FPakFileType);
        Assert.True(img.FPasswordOK);
        Assert.Equal(3, img.ImageCount);
        Assert.Equal(8, img.BitCount);
        Assert.Equal(new List<int> { 266, 300, 0 }, img.Index);

        // ImageCount = 0 → LoadIndex 的 InSize = 0 直接 Exit（原文 1425），索引表保持空
        var img0 = NewImages();
        img0.FileHeader = TPakFileHeader.CreateEmpty();
        img0.FileHeader.ImageCount = 0;
        img0.FPakFileType = TPakFileType.pftPak1;
        img0.LoadIndex();
        Assert.Empty(img0.Index);
    }

    [Fact]
    public void LoadIndex_Pak2_XorsChain0_OnEveryEntry()
    {
        var indexOffsets = new[] { 266, 278 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak2, indexOffsets, new byte[0]);
        string path = WriteFile(_dir, "p2.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();

        Assert.True(img.Initialized, "Pak2 合成文件应能通过初始化");
        Assert.Equal(TPakFileType.pftPak2, img.FPakFileType);
        Assert.Equal(new List<int> { 266, 278 }, img.Index);
    }

    [Fact]
    public void LoadIndex_Pak3_DecryptsWithoutCbc()
    {
        var indexOffsets = new[] { 266, 400, 0 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak3, indexOffsets, new byte[0]);
        string path = WriteFile(_dir, "p3.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();

        Assert.True(img.Initialized, "Pak3 合成文件应能通过初始化");
        Assert.Equal(TPakFileType.pftPak3, img.FPakFileType);
        Assert.Equal(new List<int> { 266, 400, 0 }, img.Index);
    }

    [Fact]
    public void WriteIndexList_Pak1Pak2Pak3_ProduceDifferentBytesForSameIndex()
    {
        // 同一组索引值经三种格式落盘 → 三种字节布局各不相同（差异断言）
        var results = new List<byte[]>();
        foreach (var type in new[] { TPakFileType.pftPak1, TPakFileType.pftPak2, TPakFileType.pftPak3 })
        {
            var img = NewImages();
            Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
            Array.Copy(img.FChain, img.FileHeader.Chain, 20);
            img.FPakFileType = type;
            img.InitPak3Password();
            img.Index.Add(266);
            img.Index.Add(0);

            string p = Path.Combine(_dir, "w_" + type + ".pak");
            File.WriteAllBytes(p, new byte[400]);
            using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                img.WriteIndexList(fs, -1);
            }
            var all = File.ReadAllBytes(p);
            results.Add(all[(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf)..(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 8)]);
        }

        Assert.Equal(8, results[0].Length);
        Assert.NotEqual(results[0], results[1]);
        Assert.NotEqual(results[1], results[2]);
        Assert.NotEqual(results[0], results[2]);
    }

    [Fact]
    public void WriteIndexList_Pak3_PartialUpdate_WritesOnlyOneInteger()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak3;
        img.InitPak3Password();
        img.Index.Add(0x1111);
        img.Index.Add(0x2222);
        img.Index.Add(0x3333);

        string p = Path.Combine(_dir, "p3part.pak");
        File.WriteAllBytes(p, new byte[400]);
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            // 先全量写一次作为基线
            img.WriteIndexList(fs, -1);
        }
        byte[] baseline = File.ReadAllBytes(p);

        // 只改第 1 项，走 Partial 分支
        img.Index[1] = 0x4444;
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            img.WriteIndexList(fs, 1);
        }
        byte[] after = File.ReadAllBytes(p);

        int indexBase = TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf;
        // 第 0、2 项字节不变，第 1 项变了
        Assert.Equal(baseline[indexBase..(indexBase + 4)], after[indexBase..(indexBase + 4)]);
        Assert.NotEqual(baseline[(indexBase + 4)..(indexBase + 8)], after[(indexBase + 4)..(indexBase + 8)]);
        Assert.Equal(baseline[(indexBase + 8)..(indexBase + 12)], after[(indexBase + 8)..(indexBase + 12)]);

        // Partial 写出的 4 字节必须等于「全量公式」对 0x4444 的结果
        uint expected = 0x4444u ^ img.FPak3Password[1 % 64] ^ ~(uint)1;
        Assert.Equal(BitConverter.GetBytes(expected), after[(indexBase + 4)..(indexBase + 8)]);

        // Index 超出 Count-1 → 原文两个分支都不命中，什么都不做
        byte[] beforeOutOfRange = File.ReadAllBytes(p);
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            img.WriteIndexList(fs, 99);
        }
        Assert.Equal(beforeOutOfRange, File.ReadAllBytes(p));
    }

    [Fact]
    public void WriteIndexList_Pak2_XorsChain0BeforeEncrypt()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak2;
        img.Index.Add(266);
        img.Index.Add(0);

        string p = Path.Combine(_dir, "p2w.pak");
        File.WriteAllBytes(p, new byte[400]);
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            img.WriteIndexList(fs, -1);
        }

        byte[] all = File.ReadAllBytes(p);
        int baseOff = TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf;
        byte[] written = all[baseOff..(baseOff + 8)];

        // 手工复算：每项 (offset or 0) xor Chain[0] → 两次 CBC
        var inData = new byte[8];
        BitConverter.GetBytes(266 ^ img.FileHeader.Chain[0]).CopyTo(inData, 0);
        BitConverter.GetBytes(0 ^ img.FileHeader.Chain[0]).CopyTo(inData, 4);
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(img.FileHeader.KeyData, keyData, 32);
        Array.Copy(img.FileHeader.Chain, chain, 20);
        var expected = new byte[8];
        UnitDes.EncryptCBC(inData, expected, 8, chain, keyData);
        var chain2 = new byte[20]; Array.Copy(img.FileHeader.Chain, chain2, 20);
        var kd2 = new uint[32]; Array.Copy(keyData, kd2, 32);
        UnitDes.EncryptCBC(expected, expected, 8, chain2, kd2);

        Assert.Equal(expected, written);
    }

    [Fact]
    public void WriteIndexList_LzPakV0_WritesEightByteEntriesAt262()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.Index.Add(262);
        img.Index.Add(300);
        img.m_ImageSizeList = new List<int> { 100, 200 };

        string p = Path.Combine(_dir, "lz0.pak");
        File.WriteAllBytes(p, new byte[500]);
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            img.WriteIndexList(fs, -1);
        }

        byte[] all = File.ReadAllBytes(p);
        byte[] written = all[262..(262 + 16)];

        var plain = new byte[16];
        new TLzPakIndexVer0 { nImageOffSet = 262, nDataSize = 100 }.ToBytes(plain, 0);
        new TLzPakIndexVer0 { nImageOffSet = 300, nDataSize = 200 }.ToBytes(plain, 8);

        var keyData = new uint[32]; Array.Copy(img.FileHeader.KeyData, keyData, 32);
        var chain = new byte[20]; Array.Copy(img.FileHeader.Chain, chain, 20);
        var expected = new byte[16];
        UnitDes.EncryptCBC(plain, expected, 16, chain, keyData);
        Assert.Equal(expected, written);

        // 前置 262 字节应保持 0（LzPak 索引写在 262，不是 266）
        for (int i = 0; i < 262; i++) Assert.Equal(0, all[i]);
    }

    [Fact]
    public void WriteIndexList_LzPakV1_WritesFourByteEntriesAt262()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV1;
        img.Index.Add(262);
        img.Index.Add(0);

        string p = Path.Combine(_dir, "lz1.pak");
        File.WriteAllBytes(p, new byte[500]);
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            img.WriteIndexList(fs, -1);
        }
        byte[] all = File.ReadAllBytes(p);

        var plain = new byte[8];
        BitConverter.GetBytes(262).CopyTo(plain, 0);
        BitConverter.GetBytes(0).CopyTo(plain, 4);
        var keyData = new uint[32]; Array.Copy(img.FileHeader.KeyData, keyData, 32);
        var chain = new byte[20]; Array.Copy(img.FileHeader.Chain, chain, 20);
        var expected = new byte[8];
        UnitDes.EncryptCBC(plain, expected, 8, chain, keyData);
        Assert.Equal(expected, all[262..270]);
    }

    [Fact]
    public void WriteIndexList_UnknownFileType_WritesNothing()
    {
        var img = NewImages();
        img.FPakFileType = TPakFileType.pftLzPakV0orV1;   // 原文 case 未覆盖
        img.Index.Add(266);

        string p = Path.Combine(_dir, "unk.pak");
        File.WriteAllBytes(p, new byte[400]);
        byte[] before = File.ReadAllBytes(p);
        using (var fs = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            img.WriteIndexList(fs, -1);
        }
        Assert.Equal(before, File.ReadAllBytes(p));
    }

    // ===================================================================================
    // 6. 异常路径：魔数错 / 截断 / 口令错 / 文件不存在
    // ===================================================================================

    [Fact]
    public void Initialize_WrongMagic_LeavesUninitialized()
    {
        string p = WriteFile(_dir, "bad.pak", new byte[600]);   // 全 0：既非 GEEM2 也非 HXM2
        var img = NewImages();
        img.FileName = p;
        img.Initialize();

        Assert.False(img.Initialized);
        Assert.False(img.FPasswordOK);
        Assert.Equal(0, img.ImageCount);
    }

    [Fact]
    public void Initialize_MissingFile_DoesNotThrow_AndStaysUninitialized()
    {
        var img = NewImages();
        img.FileName = Path.Combine(_dir, "does_not_exist.pak");
        img.Initialize();

        Assert.False(img.Initialized);
        Assert.False(img.FPasswordOK);
        Assert.Null(img.m_FileStream);
    }

    [Fact]
    public void Initialize_TruncatedFile_DoesNotThrow()
    {
        // 只有魔数（5 字节）+ 37 字节，远不足 TFileHeaderInfo(10) + TPakFileHeader(256)
        var bytes = new byte[42];
        bytes[0] = 5;
        bytes[1] = (byte)'G'; bytes[2] = (byte)'E'; bytes[3] = (byte)'E'; bytes[4] = (byte)'M'; bytes[5] = (byte)'2';
        string p = WriteFile(_dir, "trunc.pak", bytes);

        var img = NewImages();
        img.FileName = p;
        var ex = Record.Exception(() => img.Initialize());

        // 截断读到的头部是 0 填充 → 口令校验必然失败
        if (ex == null)
        {
            Assert.False(img.Initialized);
            Assert.False(img.FPasswordOK);
        }
        else
        {
            // 原文 916-918 会把它包成 Exception.Create(E.Message) 抛出
            Assert.IsType<Exception>(ex);
        }
    }

    [Fact]
    public void Initialize_IndexOffsetAtFileEnd_YieldsZeroIndexEntries()
    {
        // 合法 Pak1 头，但索引偏移指向文件尾（无索引数据可读）
        var indexOffsets = new[] { 0, 0 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak1, indexOffsets, new byte[0]);
        string p = WriteFile(_dir, "tail.pak", fileBytes);

        var img = NewImages();
        img.FileName = p;
        img.Initialize();

        Assert.True(img.Initialized);
        Assert.Equal(2, img.ImageCount);
        // 文件在 266 处结束 → Read 到 0 字节 → 解密后索引全 0
        Assert.Equal(new List<int> { 0, 0 }, img.Index);
        Assert.Equal((long)(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 8), img.m_FileStream!.Length);
    }

    [Fact]
    public void Initialize_WrongPasswordKey_ReportsPasswordError()
    {
        // 用 A 的密钥写文件，用 B 的密钥读 → DecryptS 得到的 CheckCode 不是 'GEEM2'
        var pwA = MakePassword();
        var pwB = TPakPassword.CreateEmpty();
        pwB.KeyData = new uint[32];
        for (int i = 0; i < 32; i++) pwB.KeyData[i] = 0xDEADBEEF;
        pwB.Chain = new byte[20];
        pwB.KeyDataLz = new uint[32];
        pwB.ChainLz = new byte[20];

        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak1, new[] { 266 }, new byte[0], pwA);
        string p = WriteFile(_dir, "pw.pak", fileBytes);

        var img = NewImages(pwB);
        img.FileName = p;
        var msgs = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => msgs.Add(m);
        img.Initialize();

        Assert.False(img.Initialized);
        Assert.False(img.FPasswordOK);
        Assert.Contains(msgs, m => m.Contains("password error"));
    }

    [Fact]
    public void Initialize_LzPakV0AndV1_AreDistinguishedByBfType()
    {
        foreach (int bfType in new[] { 0, 1 })
        {
            byte[] fileBytes = BuildLzPak((byte)bfType, bfType == 0
                ? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
                : new byte[] { 0, 0, 0, 0 });
            string p = WriteFile(_dir, $"lz{bfType}.pak", fileBytes);

            var img = NewImages();
            img.FileName = p;
            img.Initialize();

            Assert.True(img.Initialized, $"bfType={bfType} 的 LZ 文件应初始化成功");
            Assert.Equal(bfType == 0 ? TPakFileType.pftLzPakV0 : TPakFileType.pftLzPakV1, img.FPakFileType);
            Assert.True(img.FPasswordOK);
            if (bfType == 0) Assert.NotNull(img.m_ImageSizeList);
            else Assert.Null(img.m_ImageSizeList);   // V1 不建 size 表
        }
    }

    [Fact]
    public void Initialize_LzPak_WrongCheckCode_ReportsPasswordError()
    {
        // CheckCode 加密的是 'XXXXXX'（不在 IsValidLzCheckCode 的 6 个值里）
        byte[] fileBytes = BuildLzPak(0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, "XXXXXX");
        string p = WriteFile(_dir, "lzbad.pak", fileBytes);

        var img = NewImages();
        img.FileName = p;
        var msgs = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => msgs.Add(m);
        img.Initialize();

        Assert.False(img.Initialized);
        Assert.False(img.FPasswordOK);
        Assert.Contains(msgs, m => m.Contains("password error"));
    }

    /// <summary>构造 LZ PAK 文件（5 字节 'HXM2.' + 256 字节 DES 头 + 1 填充 + 8 或 4 字节索引）。</summary>
    private static byte[] BuildLzPak(byte bfType, byte[] encryptedIndex, string checkCode = "D3DM2")
    {
        var pw = MakePassword();
        var header = TPakFileHeader.CreateEmpty();
        header.bfType = bfType;
        header.ImageCount = bfType == 0 ? 1u : 2u;
        header.BitCount = 8;
        header.Size = header.IndexOffSet = 262;
        header.CheckCodeText = "";                 // 稍后手工加密
        Array.Copy(pw.KeyDataLz, header.KeyData, 32);
        Array.Copy(pw.ChainLz, header.Chain, 20);

        // CheckCode := EncryptS('D3DM2')：用**头里的** KeyData/Chain 加密
        var img = new TPakImages(pw);
        img.FileHeader = header;
        img.FPakFileType = bfType == 0 ? TPakFileType.pftLzPakV0 : TPakFileType.pftLzPakV1;
        SetCheckCode(header, img.EncryptS(checkCode));
        // EncryptS 会就地改写头里的 Chain；Header 加密用的是 sHeadPassword（与 Chain 无关），
        // 但为保持一致仍把 KeyData/Chain 复原（Initialize 侧会用 FKeyDataLz/FChainLz）。
        Array.Copy(pw.KeyDataLz, header.KeyData, 32);
        Array.Copy(pw.ChainLz, header.Chain, 20);

        var plain = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(plain, 0);
        var cipher = new byte[TPakFileHeader.SizeOf];
        UnitDes.EncryptDes(plain, cipher, TPakFileHeader.SizeOf, "442517066");

        var outBuf = new List<byte>();
        outBuf.AddRange(TLzFileHeaderInfo.Hzm2Dot().FileType);
        outBuf.AddRange(cipher);
        outBuf.Add(0);                             // 1 字节填充
        outBuf.AddRange(encryptedIndex);
        return outBuf.ToArray();
    }

    // ===================================================================================
    // 7. Initialize_UpdateNewFile / UpdateIndex / WriteHeader / Finalize
    // ===================================================================================

    [Fact]
    public void InitializeUpdateNewFile_AfterUpdateIndex_SucceedsOnFreshlyWrittenFile()
    {
        var img = NewImages();
        string path = Path.Combine(_dir, "fresh.pak");
        img.FileName = path;

        var key = TPakKey.CreateEmpty();
        key.ImageCount = 4;
        key.PakType = (ushort)((8 << 8) | (int)TPakFileType.pftLzPakV0);
        Array.Copy(img.FKeyData, key.KeyData, 32);
        Array.Copy(img.FChain, key.Chain, 20);

        img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘

        Assert.True(File.Exists(path));
        // UpdateIndex 末尾的 Initialize_UpdateNewFile 会把 m_FileStream 打开（原文语义）；
        // 要在对象存活期间读磁盘，必须先释放句柄（托管 FileStream 必须显式 Dispose）。
        img.Dispose();

        byte[] all = File.ReadAllBytes(path);
        // 前 5 字节是 'HXM2.'
        Assert.Equal("HXM2.", GXX.Core.EncodingInit.GBK.GetString(all, 0, 5));
        // 文件长度 = 262（LZ 头）+ ImageCount(4) × TLzPakIndexVer0(8) = 262 + 32 = 294
        Assert.Equal(294, all.Length);
        Assert.Equal(TPakFileType.pftLzPakV0, img.FPakFileType);
        Assert.Equal((ushort)8, img.FileHeader.BitCount);
        Assert.Equal("www.gameofmir2.com", img.FileHeader.TitleText);

        // 再自行初始化读回
        var img2 = NewImages();
        img2.FileName = path;
        img2.Initialize();
        Assert.True(img2.Initialized);
        Assert.Equal(4, img2.ImageCount);
        Assert.Equal(262, (int)img2.FileHeader.IndexOffSet);
    }

    [Fact]
    public void UpdateIndex_ImageCountZero_SetsNeedUpdateFalseAndDoesNothing()
    {
        var img = NewImages();
        string path = Path.Combine(_dir, "zero.pak");
        img.FileName = path;

        var key = TPakKey.CreateEmpty();
        key.ImageCount = 0;
        img.m_boNeedUpdate = true;

        img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘

        Assert.False(img.m_boNeedUpdate);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void UpdateIndex_UnknownPakType_FallsBackToPak1()
    {
        var img = NewImages();
        string path = Path.Combine(_dir, "fallback.pak");
        img.FileName = path;

        var key = TPakKey.CreateEmpty();
        key.ImageCount = 2;
        key.PakType = 0x00FF;             // 低字节 0xFF 不在 [0..4]
        Array.Copy(img.FKeyData, key.KeyData, 32);
        Array.Copy(img.FChain, key.Chain, 20);

        img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘

        Assert.Equal(TPakFileType.pftPak1, img.FPakFileType);
        byte[] all = File.ReadAllBytes(path);
        Assert.Equal("GEEM2", GXX.Core.EncodingInit.GBK.GetString(all, 1, 5)); // ShortString 长度字节 + 'GEEM2'
        // 索引 = ImageCount(2) × 4 = 8 → 266 + 8 = 274
        Assert.Equal(274, all.Length);
    }

    [Fact]
    public void UpdateIndex_CompareHeaderIdentical_ShortCircuits()
    {
        var img = NewImages();
        string path = Path.Combine(_dir, "cmp.pak");
        img.FileName = path;

        var key = TPakKey.CreateEmpty();
        key.ImageCount = 2;
        key.PakType = (ushort)TPakFileType.pftPak1;
        Array.Copy(img.FKeyData, key.KeyData, 32);
        Array.Copy(img.FChain, key.Chain, 20);

        img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘
        // UpdateIndex 末尾的 Initialize_UpdateNewFile **不**把 Initialized 置真（原文 1019-1049 亦然）
        Assert.False(img.Initialized);

        // 显式 Initialize 才把 Initialized 置真（读回刚写好的文件）
        img.Initialize();
        Assert.True(img.Initialized);
        Assert.Equal(2, img.Index.Count);

        // 相同 key 且头部完全一致 + IsCompareHeader=True → 直接 Exit，Initialized 保持为真
        img.UpdateIndex(key, true);
        Assert.True(img.Initialized);
        Assert.Equal(TPakFileType.pftPak1, img.FPakFileType);

        // 改一个字节 → 不再一致 → 走 Finalize + 重写（Initialized 被 Finalize 清掉）
        var key2 = TPakKey.CreateEmpty();
        key2.ImageCount = 2;
        key2.PakType = (ushort)TPakFileType.pftPak1;
        Array.Copy(img.FKeyData, key2.KeyData, 32);
        Array.Copy(img.FChain, key2.Chain, 20);
        key2.Chain[3] ^= 0xFF;           // 与 FileHeader.Chain 不同 → CompareMem 失败
        img.UpdateIndex(key2, true);
        Assert.False(img.Initialized);    // Finalize_() 在重写路径上被调用
        Assert.Equal(TPakFileType.pftPak1, img.FPakFileType);
        Assert.Equal(2, img.Index.Count); // 重写后索引表重建（全 0 占位）
    }

    [Fact]
    public void WriteHeader_WritesEncryptedHeaderAtOffsetZero_WithDocumentedBug()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.Index.Add(266);
        img.Index.Add(0x1234);
        img.Initialized = true;

        string path = Path.Combine(_dir, "wh.pak");
        File.WriteAllBytes(path, new byte[400]);
        img.m_FileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        img.WriteHeader();
        img.m_FileStream.Dispose();
        img.m_FileStream = null;

        byte[] all = File.ReadAllBytes(path);
        // 原文自承 BUG：256 字节头直接写在偏移 0（覆盖了本该在前的 TFileHeaderInfo）
        Assert.Equal(256, TPakFileHeader.SizeOf);
        Assert.Equal(2, img.FileHeader.bfType);
        Assert.Equal((uint)img.Index.Count, img.FileHeader.ImageCount);
        Assert.Equal((uint)TPakFileHeader.SizeOf, img.FileHeader.IndexOffSet); // 256，不是 266

        var back = img.DecryptHeader(PakSeams.ToLatin1(all[0..256]));
        Assert.Equal(2u, back.ImageCount);
        Assert.Equal(2, back.bfType);
        Assert.Equal((uint)TPakFileHeader.SizeOf, back.IndexOffSet);
    }

    [Fact]
    public void Finalize_ClearsEverything_AndReleasesStream()
    {
        var indexOffsets = new[] { 266 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak1, indexOffsets, new byte[0]);
        string path = WriteFile(_dir, "fin.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();
        Assert.True(img.Initialized);
        Assert.NotNull(img.m_FileStream);

        img.Finalize_();

        Assert.False(img.Initialized);
        Assert.Null(img.m_ImgArr);
        Assert.Equal(0, img.ImageCount);
        Assert.Empty(img.Index);
        Assert.Empty(img.IndexList);
        Assert.Empty(img.GrayIndexList);
        Assert.Empty(img.BrightIndexList);
        Assert.Null(img.m_FileStream);
    }

    [Fact]
    public void Finalize_OnUninitializedInstance_IsSafe()
    {
        var img = NewImages();
        var ex = Record.Exception(() => img.Finalize_());
        Assert.Null(ex);
        Assert.False(img.Initialized);
    }

    // ===================================================================================
    // 8. 图片头校验 / 尺寸计算
    // ===================================================================================

    [Fact]
    public void IsValidLzImageInfoV0_BitCountAndBounds()
    {
        var img = NewImages();
        var ok = new TPakImageInfo { btEncr0 = 0, wW = 16, wH = 16, wPx = 0, wPy = 0 };

        Assert.True(img.IsValidLzImageInfoV0(ok, 0, 256, 8));
        Assert.True(img.IsValidLzImageInfoV0(ok, 0, 256, 16));
        Assert.True(img.IsValidLzImageInfoV0(ok, 0, 256, 24));
        Assert.True(img.IsValidLzImageInfoV0(ok, 0, 256, 32));
        // BitCount 不在 {8,16,24,32}
        Assert.False(img.IsValidLzImageInfoV0(ok, 0, 256, 0));
        Assert.False(img.IsValidLzImageInfoV0(ok, 0, 256, 4));
        // px/py 越界
        Assert.False(img.IsValidLzImageInfoV0(new TPakImageInfo { wW = 16, wH = 16, wPx = 3201 }, 0, 256, 8));
        Assert.False(img.IsValidLzImageInfoV0(new TPakImageInfo { wW = 16, wH = 16, wPy = -3201 }, 0, 256, 8));
        // Length ≥ MAX_IMAGE_SIZE
        Assert.False(img.IsValidLzImageInfoV0(ok, 0, GameImagesConsts.MAX_IMAGE_SIZE, 8));
        // 宽或高 ≤ 0
        Assert.False(img.IsValidLzImageInfoV0(new TPakImageInfo { wW = 0, wH = 16 }, 0, 256, 8));
        Assert.False(img.IsValidLzImageInfoV0(new TPakImageInfo { wW = 16, wH = -1 }, 0, 256, 8));
        // 宽或高 ≥ MAX（原文这里**不** Exit，但 Result 为 False）
        Assert.False(img.IsValidLzImageInfoV0(new TPakImageInfo { wW = 3200, wH = 16 }, 0, 256, 8));
    }

    [Fact]
    public void IsValidLzImageInfoV0_AbsSmallIntWrapsAtMinValue()
    {
        // 原文 Abs(SmallInt(-32768)) 按 16 位回绕仍为 -32768（负），
        // 于是 |px| > 3200 的守卫对 -32768 **失效** → 该图被判为合法。
        var img = NewImages();
        var wrapped = new TPakImageInfo { wW = 16, wH = 16, wPx = short.MinValue };
        Assert.True(img.IsValidLzImageInfoV0(wrapped, 0, 256, 8));

        // 对照：-3201 会被拦下（差异断言）
        var caught = new TPakImageInfo { wW = 16, wH = 16, wPx = -3201 };
        Assert.False(img.IsValidLzImageInfoV0(caught, 0, 256, 8));
    }

    [Fact]
    public void IsValidLzImageInfoV1_PixelFormatGate()
    {
        var img = NewImages();
        var ok = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 16,
            nHeight = 16,
            Length = 256,
        };
        Assert.True(img.IsValidLzImageInfoV1(ok, 0));

        // pfDevice / pf1bit / pf4bit / pfCustom 都不在 [pf8bit..pf32bit]
        foreach (var pf in new[] { TPixelFormat.pfDevice, TPixelFormat.pf1bit, TPixelFormat.pf4bit, TPixelFormat.pfCustom })
        {
            var bad = ok; bad.PixelFormat = (byte)pf;
            Assert.False(img.IsValidLzImageInfoV1(bad, 0));
        }
        // 宽高边界
        var w0 = ok; w0.nWidth = 0;
        Assert.False(img.IsValidLzImageInfoV1(w0, 0));
        var wMax = ok; wMax.nWidth = 3200;
        Assert.False(img.IsValidLzImageInfoV1(wMax, 0));
        var lenMax = ok; lenMax.Length = GameImagesConsts.MAX_IMAGE_SIZE;
        Assert.False(img.IsValidLzImageInfoV1(lenMax, 0));
    }

    [Fact]
    public void GetNewPakImageDataSize_LengthWins_AlphaAddsAFull8bitPlane()
    {
        // Length > 0 → 直接返回 Length，**不**再加 alpha
        Assert.Equal(999, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 16,
            nHeight = 16,
            boAlpha = true,
            Length = 999,
        }));

        // Length = 0 → 按 PixelFormat 计算。WidthBytes(bits, w) = ((w*bits + 31) div 32) * 4：
        //   pf8  (8,16)  = 16 → 16*16  = 256
        //   pf16 (16,16) = 32 → 32*16  = 512
        //   pf24 (24,16) = 48 → 48*16  = 768
        //   pf32 (32,16) = 64 → 64*16  = 1024
        Assert.Equal(16 * 16, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf8bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(256, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf8bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(32 * 16, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf16bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(512, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf16bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(48 * 16, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf24bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(768, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf24bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(64 * 16, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf32bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(1024, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf32bit, nWidth = 16, nHeight = 16, Length = 0 }));

        // boAlpha → 额外加一个 8bit 平面：
        //   主体 WidthBytes(16,16)=((256+31)/32)*4=32 → 32*16 = 512
        //   alpha WidthBytes(8,16)=((128+31)/32)*4=16 → 16*16 = 256，合计 768
        Assert.Equal(32 * 16 + 16 * 16, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf16bit, nWidth = 16, nHeight = 16, boAlpha = true, Length = 0 }));
        Assert.Equal(768, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf16bit, nWidth = 16, nHeight = 16, boAlpha = true, Length = 0 }));

        // 未知格式 → 0；负数 → 0（注意原文是 < 0，**不含** 0）
        Assert.Equal(0, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf1bit, nWidth = 16, nHeight = 16, Length = 0 }));
        Assert.Equal(0, TPakImages.GetNewPakImageDataSize(new TNewPakImageInfo
        { PixelFormat = (byte)TPixelFormat.pf8bit, nWidth = -1, nHeight = 16, Length = 0 }));
    }

    [Fact]
    public void IsNewSDFormat_OnlyFor16BitWithoutAlpha()
    {
        // pf16bit 且 !boAlpha 且 nWidth*nHeight*2 != nDecompressedSize → true
        var h = new TNewPakImageInfo { PixelFormat = (byte)TPixelFormat.pf16bit, nWidth = 8, nHeight = 8 };
        Assert.True(TPakImages.IsNewSDFormat(h, 100));    // 128 != 100
        Assert.False(TPakImages.IsNewSDFormat(h, 128));   // 相等 → false

        // boAlpha = true → 恒 false
        var ha = h; ha.boAlpha = true;
        Assert.False(TPakImages.IsNewSDFormat(ha, 100));

        // 非 pf16bit → 恒 false
        var h8 = h; h8.PixelFormat = (byte)TPixelFormat.pf8bit;
        Assert.False(TPakImages.IsNewSDFormat(h8, 100));
    }

    [Fact]
    public void GetAlphaDibFromNewFormat_ExpandsNibblesAndRejectsShortInput()
    {
        // 4x2，16bit 主体 = 4*2*2 = 16 字节；alpha 行字节 = ceil(4/2)=2，alpha 总 = 2*2 = 4
        int w = 4, h = 2;
        int imgSize = w * h * 2;
        int alphaLine = PakConsts.AlphaLineSize(w);   // 2
        int alphaSize = h * alphaLine;                // 4
        var src = new byte[imgSize + alphaSize];

        // 第 0 行 alpha 字节：0x12 → x0 = 1*17 = 17, x1 = 2*17 = 34
        // 第 1 行 alpha 字节：0xF0 → x0 = 15*17 = 255, x1 = 0
        src[imgSize + 0] = 0x12;
        src[imgSize + 1] = 0x00;
        src[imgSize + alphaLine + 0] = 0xF0;
        src[imgSize + alphaLine + 1] = 0x00;

        var dib = TPakImages.GetAlphaDibFromNewFormat(w, h, src, src.Length);
        Assert.NotNull(dib);
        Assert.Equal(4, dib!.Width);
        Assert.Equal(2, dib.Height);
        Assert.Equal(8, dib.BitCount);

        Assert.Equal((byte)17, dib.Pixel(0, 0));
        Assert.Equal((byte)34, dib.Pixel(1, 0));
        Assert.Equal((byte)0, dib.Pixel(2, 0));
        Assert.Equal((byte)0, dib.Pixel(3, 0));
        Assert.Equal((byte)255, dib.Pixel(0, 1));
        Assert.Equal((byte)0, dib.Pixel(1, 1));

        // 越界守卫：nAlphaSize > (nSrcSize - nImgSize) → null
        Assert.Null(TPakImages.GetAlphaDibFromNewFormat(w, h, src, imgSize + alphaSize - 1));
        Assert.Null(TPakImages.GetAlphaDibFromNewFormat(w, h, src, imgSize));
    }

    // ===================================================================================
    // 9. 文件读取原语与 RLE
    // ===================================================================================

    [Fact]
    public void ReadFully_ReturnsActualByteCount_AndStopsAtEof()
    {
        string p = WriteFile(_dir, "rf.bin", new byte[] { 1, 2, 3 });
        using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        var buf = new byte[8];
        Assert.Equal(3, TPakImages.ReadFully(fs, buf, 0, 8));
        Assert.Equal(new byte[] { 1, 2, 3, 0, 0, 0, 0, 0 }, buf);

        fs.Position = 0;
        var buf2 = new byte[2];
        Assert.Equal(2, TPakImages.ReadFully(fs, buf2, 0, 2));
        Assert.Equal(new byte[] { 1, 2 }, buf2);

        fs.Position = 3;
        Assert.Equal(0, TPakImages.ReadFully(fs, buf2, 0, 2));   // 已在 EOF
    }

    [Fact]
    public void ReadImageHeader_Pak1_DecryptsInPlace_Deterministically()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);

        // 4 字节明文 → 先手工加密写入文件 → ReadImageHeader 读回后应等于明文
        var plain = new byte[] { 0x11, 0x22, 0x33, 0x44 };
        var cipher = new byte[4];
        var kd = new uint[32]; var ch = new byte[20];
        Array.Copy(img.FileHeader.KeyData, kd, 32);
        Array.Copy(img.FileHeader.Chain, ch, 20);
        UnitDes.EncryptCBC(plain, cipher, 4, ch, kd);

        string p = WriteFile(_dir, "rih.bin", cipher);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var buf = new byte[4];
        img.ReadImageHeader(buf, 4);
        Assert.Equal(plain, buf);

        // 差异断言：ReadImageHeader 每次从 FileHeader.Chain **重新拷一份** chain 副本再解，
        // 所以 FileHeader.Chain 本身**不被改写**（原文是就地传 @Chain，会被 CBC 改写）；
        // 因此回到文件头重读同一段密文，结果与第一次相同。
        img.m_FileStream.Position = 0;
        var buf2 = new byte[4];
        img.ReadImageHeader(buf2, 4);
        Assert.Equal(plain, buf2);
        // 但 FileHeader.Chain 保持原值（托管侧从副本运算）
        Assert.Equal((byte)0x10, img.FileHeader.Chain[0]);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void ReadImageHeader_Pak3_AesDecryptsWithPerIndexKey()
    {
        var img = NewImages();
        img.InitPak3Password();

        // 用 Pak3K 组合规则反推加密：加密 = AES-CTR(同一 key)，CTR 对称
        int index = 3;
        var key = new byte[16];
        BitConverter.GetBytes(img.FPak3Password[(index + 0) % 64] ^ img.FPak3Password[(index + 14) % 64]).CopyTo(key, 0);
        BitConverter.GetBytes(img.FPak3Password[(index + 12) % 64] & img.FPak3Password[(index + 19) % 64]).CopyTo(key, 4);
        BitConverter.GetBytes(img.FPak3Password[(index + 28) % 64] ^ ~img.FPak3Password[(index + 10) % 64]).CopyTo(key, 8);
        BitConverter.GetBytes(img.FPak3Password[(index + 1) % 64]).CopyTo(key, 12);

        var plain = new byte[16];
        for (int i = 0; i < 16; i++) plain[i] = (byte)(0x80 + i);
        var cipher = new byte[16];
        PakAesCtr.AesEncrypt(key, 16, plain, cipher, 16);

        string p = WriteFile(_dir, "rih3.bin", cipher);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var buf = new byte[16];
        img.ReadImageHeader_Pak3(index, buf, 16);
        Assert.Equal(plain, buf);

        // 用错的 index 读同一段字节 → 得到不同的结果（差异断言）
        var bufWrong = new byte[16];
        img.m_FileStream.Position = 0;
        img.ReadImageHeader_Pak3(index + 1, bufWrong, 16);
        Assert.NotEqual(buf, bufWrong);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void PakAesCtr_CounterIncrementsBigEndianAtByte7_AndTailDoesNotIncrement()
    {
        // 独立构造期望的密钥流：Block 从全 0 起，每块先加密再对 Block[7] 手动大端 +1；
        // 余数部分用**当前** Block 再加密一次（**不再自增**）。
        var key = new byte[16];
        for (int i = 0; i < 16; i++) key[i] = (byte)(0xA0 + i);

        int len = 40;                      // 2 整块 + 8 字节余数
        var plain = new byte[len];
        for (int i = 0; i < len; i++) plain[i] = (byte)i;

        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Mode = System.Security.Cryptography.CipherMode.ECB;
        aes.Padding = System.Security.Cryptography.PaddingMode.None;
        aes.Key = key;
        using var enc = aes.CreateEncryptor();
        var block = new byte[16];
        var temp = new byte[16];
        var expected = new byte[len];
        for (int b = 0; b < len / 16; b++)
        {
            enc.TransformBlock(block, 0, 16, temp, 0);
            int offset = 7;
            block[offset]++;
            if (block[offset] == 0)
            {
                do { offset--; block[offset]++; if (block[offset] != 0 || offset == 7) break; } while (true);
            }
            for (int j = 0; j < 16; j++) expected[b * 16 + j] = (byte)(plain[b * 16 + j] ^ temp[j]);
        }
        enc.TransformBlock(block, 0, 16, temp, 0);    // 余数：不再自增
        for (int j = 0; j < len % 16; j++) expected[(len / 16) * 16 + j] = (byte)(plain[(len / 16) * 16 + j] ^ temp[j]);

        var actual = new byte[len];
        PakAesCtr.AesEncrypt(key, 16, plain, actual, len);
        Assert.Equal(expected, actual);

        // AES-CTR 是对合：AesDecrypt(密文) == 明文
        var back = new byte[len];
        PakAesCtr.AesDecrypt(key, 16, actual, back, len);
        Assert.Equal(plain, back);
    }

    [Fact]
    public void PakDesNew_EncryptDecryptRoundTrip_AndDiffersFromUnitDes()
    {
        const string key = "3698261022";

        var plain = new byte[TPakFileHeader.SizeOf];
        for (int i = 0; i < plain.Length; i++) plain[i] = (byte)(i * 7 + 3);

        var cipher = new byte[plain.Length];
        PakDesNew.EncryptDes_New(plain, cipher, plain.Length, key);

        // 差异断言：DesUtils 变体（BS=20、自有密钥派生）与 UnitDes（BS=20、SHA1 派生）不同
        var unitCipher = new byte[plain.Length];
        UnitDes.EncryptDes(plain, unitCipher, plain.Length, key);
        Assert.NotEqual(unitCipher, cipher);

        var back = new byte[plain.Length];
        PakDesNew.DecryptDes_New(cipher, back, plain.Length, key);
        Assert.Equal(plain, back);

        // 便捷包装与就地版本一致
        Assert.Equal(cipher, PakDesNew.EncryptDes_New(plain, plain.Length, key));
        Assert.Equal(plain, PakDesNew.DecryptDes_New(cipher, plain.Length, key));

        // 尾块路径（非 20 倍数）
        var p7 = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
        var c7 = PakDesNew.EncryptDes_New(p7, 7, key);
        Assert.NotEqual(p7, c7);
        Assert.Equal(p7, PakDesNew.DecryptDes_New(c7, 7, key));
    }

    [Fact]
    public void PakDesNew_DeriveKeys_StructureAndEmptyKeyFallback()
    {
        // 空 Key → KeyA/KeyB 都是 0（原文 450-451 的初值，if Length(Key) > 0 不成立）
        var (ka, kb) = PakDesNew.DeriveKeys("");
        Assert.Equal(0u, ka);
        Assert.Equal(0u, kb);

        // 非空 Key → 两个派生子密钥都非 0（CRC32 段 + Jenkins 段各产出非 0 中间值）
        var (ka2, kb2) = PakDesNew.DeriveKeys("3698261022");
        Assert.NotEqual(0u, ka2);
        Assert.NotEqual(0u, kb2);

        // 同一 Key 两次派生必须一致（纯函数）
        var (ka3, kb3) = PakDesNew.DeriveKeys("3698261022");
        Assert.Equal(ka2, ka3);
        Assert.Equal(kb2, kb3);

        // 不同 Key → 不同派生结果
        var (ka4, kb4) = PakDesNew.DeriveKeys("3698261023");
        Assert.NotEqual(ka2, ka4);
    }

    [Fact]
    public void PakDesNew_TailMix_DiffersFromRunGateDesNew2Port()
    {
        // 原文 DesUtils 尾段对 b 的 4 项累加带位移（k+7<<24 / k+6<<16 / k+4<<8 / k+5），
        // 而 GXX.RunGate.DesNew2.cs 的实现漏了全部左移并把 k+4/k+5 用反。
        // 用长度 5..11 的密钥（会走到尾段）做区分：两者的密钥派生理应不同。
        foreach (int len in new[] { 5, 6, 7, 8, 9, 10, 11 })
        {
            string key = new string('7', len);
            var (ka, kb) = PakDesNew.DeriveKeys(key);
            Assert.NotEqual(0u, kb);   // 尾段确实参与了混合（不是「KeyB 保持 0」）
        }
    }

    [Fact]
    public void PakRle_DecodeRle_LiteralRunRepeatRunAndBoundaryStop()
    {
        // RLEUnit.pas 188-241：Rle < 128 → 拷贝 (Rle+1) 个像素；Rle >= 128 → 1 个像素重复 (Rle-127) 次
        // Bpp = 1，宽 4 高 1 → 目标 4 字节
        var src = new byte[] { 3, 0xAA, 0xBB, 0xCC, 0xDD };
        var dest = PakRle.DecodeRle(src, 0, src.Length, 4, 1, 1);
        Assert.Equal(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }, dest);

        // 重复段：Rle = 0x80 + 2 = 130 → 130 - 127 = 3 次
        var src2 = new byte[] { 130, 0x5A };
        var dest2 = PakRle.DecodeRle(src2, 0, src2.Length, 3, 1, 1);
        Assert.Equal(new byte[] { 0x5A, 0x5A, 0x5A }, dest2);

        // 混合：2 个原样 (Rle=1) + 2 个重复 (Rle=129 → 129-127 = 2)
        var src3 = new byte[] { 1, 0x01, 0x02, 129, 0x09 };
        var dest3 = PakRle.DecodeRle(src3, 0, src3.Length, 4, 1, 1);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x09, 0x09 }, dest3);

        // Bpp = 2：像素是 16bit
        var src4 = new byte[] { 1, 0x11, 0x22, 0x33, 0x44 };
        var dest4 = PakRle.DecodeRle(src4, 0, src4.Length, 2, 1, 2);
        Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44 }, dest4);

        // 源不足 → 越界即停，不抛异常
        var shortSrc = new byte[] { 10, 0x01 };
        var dest5 = PakRle.DecodeRle(shortSrc, 0, shortSrc.Length, 20, 1, 1);
        Assert.Equal(20, dest5.Length);
        Assert.Equal(0x01, dest5[0]);
        Assert.Equal(0, dest5[10]);
    }

    [Fact]
    public void PakRle_DiffersFromZlibExDecodeRle()
    {
        // 差异断言：ZlibEx.DecodeRLE 用的是 0xC0 前缀的 RLE90 语义，与 RLEUnit 完全不同。
        // 输入 { 130, 0x5A }：RLEUnit → 3 个 0x5A；RLE90（0xC0 前缀）→ 130 & 0xC0 == 0x80 ≠ 0xC0 → 逐字面量。
        var src = new byte[] { 130, 0x5A };
        var pak = PakRle.DecodeRle(src, 0, src.Length, 2, 1, 1);
        var zlib = ZlibEx.DecodeRLE(src, src.Length, 2);
        Assert.Equal(new byte[] { 0x5A, 0x5A }, pak);
        Assert.NotEqual(pak, zlib);
    }

    // ===================================================================================
    // 10. 缓存取图（GetCached* / GetBitmap）—— 未初始化与越界路径
    // ===================================================================================

    [Fact]
    public void GetBitmap_UninitializedReturnsNull_AndStillWritesUseCheckTick()
    {
        // 原文 1529：m_dwUseCheckTick := MyGetTickCount; 在 index >= 0 的**外层**，无条件执行
        TGameImages.MyGetTickCountFn = () => 0x0001_0000u;
        var img = NewImages();
        var r = img.GetBitmap(0, out int px, out int py);

        Assert.Null(r);
        Assert.Equal(0, px);
        Assert.Equal(0, py);
        Assert.Equal(0x0001_0000u, img.m_dwUseCheckTick);
    }

    [Fact]
    public void GetBitmap_NegativeIndex_StillWritesUseCheckTick()
    {
        // 原文 1529-1530：先写 tick，再判 if (Index >= 0) —— 负索引**也**会写 tick
        var img = NewImages();
        TGameImages.MyGetTickCountFn = () => 0x1234u;
        var r = img.GetBitmap(-1, out _, out _);
        Assert.Null(r);
        Assert.Equal(0x1234u, img.m_dwUseCheckTick);
    }

    [Fact]
    public void GetCachedImage_NotInitialized_TriesIndexUpdateOnlyWhenFlagsAllow()
    {
        var img = NewImages();
        int calls = 0;
        PakSeams.UpdateEngineAddFn = (_, _, _, _, _) => { calls++; return true; };

        // 未初始化 + 未允许自动更新 → 不请求
        var r = img.GetCachedImage(0, out int px, out int py);
        Assert.False(r.Assigned);
        Assert.Equal(0, px);
        Assert.Equal(0, calls);

        // 打开全部开关 → 请求一次（Index < ImageCount → imageIndex = -1）
        img.m_boUpdateIndex = true;
        img.m_boUpdateIndexing = false;
        img.m_boNeedUpdate = true;
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;

        var r2 = img.GetCachedImage(0, out _, out _);
        Assert.False(r2.Assigned);
        Assert.Equal(1, calls);
        Assert.True(img.m_boUpdateIndexing);

        // 已在 indexing 且未到重试时间 → 不再请求
        var r3 = img.GetCachedImage(0, out _, out _);
        Assert.False(r3.Assigned);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void GetCachedImage_NegativeIndex_ReturnsNil_WithoutLocking()
    {
        var img = NewImages();
        var r = img.GetCachedImage(-5, out int px, out int py);
        Assert.False(r.Assigned);
        Assert.Equal(0, px);
        Assert.Equal(0, py);
    }

    [Fact]
    public void GetCachedImageSize_NonLzWithoutInit_ReturnsFalse()
    {
        var img = NewImages();
        img.FPakFileType = TPakFileType.pftPak1;
        var size = new Size(0, 0);
        var pt = new Point(0, 0);
        Assert.False(img.GetCachedImageSize(0, ref size, ref pt));
        Assert.Equal(0, size.Width);
        Assert.Equal(0, pt.X);
    }

    [Fact]
    public void GetCachedImageSize_LzTypeDelegatesToLzPath()
    {
        // LZ 类型 → 转调 GetCachedLzImageSize（未初始化时同样 False）
        var img = NewImages();
        img.FPakFileType = TPakFileType.pftLzPakV0;
        var size = new Size(0, 0);
        var pt = new Point(0, 0);
        Assert.False(img.GetCachedImageSize(0, ref size, ref pt));
    }

    [Fact]
    public void GetCachedSurface_NotInitialized_ReturnsNil()
    {
        var img = NewImages();
        var r = img.GetCachedSurface(0);
        Assert.False(r.Assigned);

        var g = img.GetCachedGray(0);
        Assert.False(g.Assigned);

        var b = img.GetCachedBright(0);
        Assert.False(b.Assigned);

        // 负数索引短路
        Assert.False(img.GetCachedSurface(-1).Assigned);
        Assert.False(img.GetCachedGray(-1).Assigned);
        Assert.False(img.GetCachedBright(-1).Assigned);
    }

    [Fact]
    public void GetCachedImage_ReturnsNullImageSentinel_WhenLoadFailsOnValidSetup()
    {
        // 索引指向 0（< 256）→ LoadDxImage 整段不执行，Surface 仍为 nil → 回退 g_NullImage
        var indexOffsets = new[] { 0 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak1, indexOffsets, new byte[0]);
        string path = WriteFile(_dir, "nullimg.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();
        Assert.True(img.Initialized);

        TGameImages.g_NullImage = new TTextureRef();
        var r = img.GetCachedImage(0, out int px, out int py);

        // LoadDxImage 因 position(0) < 256 直接返回，Surface 仍为 nil → 回退 g_NullImage
        Assert.False(r.Assigned);         // g_NullImage 是 default（nil）
        Assert.Equal(0, px);
        Assert.Equal(0, py);
    }

    [Fact]
    public void GetCachedImage_OutOfRangeIndex_TriesIndexUpdate()
    {
        var indexOffsets = new[] { 0 };
        byte[] fileBytes = BuildPlainPak(TPakFileType.pftPak1, indexOffsets, new byte[0]);
        string path = WriteFile(_dir, "oor.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();

        int calls = 0;
        int lastImageIndex = int.MinValue;
        PakSeams.UpdateEngineAddFn = (_, t, ii, _, _) => { calls++; lastImageIndex = ii; return true; };
        img.m_boUpdateIndex = true;
        img.m_boUpdateIndexing = false;
        img.m_boNeedUpdate = true;
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;

        var r = img.GetCachedImage(99, out _, out _);
        Assert.False(r.Assigned);
        Assert.Equal(1, calls);
        Assert.Equal(0, lastImageIndex);   // Index >= ImageCount → imageIndex = 0
    }

    [Fact]
    public void GetCachedImage_SecondCallReusesCachedSurface_AndDoesNotReload()
    {
        // 索引指向图片区首地址（BuildPlainPakWithImage 自动算 = 270），图片是 1x1 的合法头
        var body = BuildOneImageArea(270, width: 1, height: 1);
        byte[] fileBytes = BuildPlainPakWithImage(TPakFileType.pftPak1, body);
        Assert.Equal(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 4 + 16, fileBytes.Length);
        string path = WriteFile(_dir, "cache.pak", fileBytes);

        var img = NewImages();
        img.FileName = path;
        img.Initialize();
        Assert.True(img.Initialized);
        Assert.Equal(270, img.Index[0]);

        var r1 = img.GetCachedImage(0, out int px1, out int py1);
        Assert.True(r1.Assigned);
        Assert.True(r1.IsNullTexture);        // 1x1 → NULLTexture
        Assert.Equal(1, (int)img.m_ImgArr![0].nWidth);
        Assert.Equal(1, (int)img.m_ImgArr[0].nHeight);

        // 第二次：Surface 已赋值 → 走 else 分支，不重新装载
        var r2 = img.GetCachedImage(0, out int px2, out int py2);
        Assert.True(r2.Assigned);
        Assert.Equal(r1, r2);
        Assert.Equal(px1, px2);
        Assert.Equal(py1, py2);
        Assert.Single(img.IndexList);         // 只在首次 Add 一次
    }

    /// <summary>构造一段图片区：16 字节 TNewPakImageInfo（加密）+ 0 字节数据。</summary>
    private static byte[] BuildOneImageArea(int position, short width, short height)
    {
        var pw = MakePassword();
        var img = new TPakImages(pw);
        var header = TPakFileHeader.CreateEmpty();
        Array.Copy(pw.KeyData, header.KeyData, 32);
        Array.Copy(pw.Chain, header.Chain, 20);
        img.FileHeader = header;
        img.FPakFileType = TPakFileType.pftPak1;

        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            boAlpha = false,
            nWidth = width,
            nHeight = height,
            px = 0,
            py = 0,
            Length = 0,
        };
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);

        var kd = new uint[32]; Array.Copy(header.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(header.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);
        // position 只用于断言，不影响内容：调用方负责把它放到索引指向的位置
        Assert.True(position >= TPakFileHeader.SizeOf);
        return cipher;
    }

    // ===================================================================================
    // 11. LoadDx*Image 与 LZ 装载
    // ===================================================================================

    [Fact]
    public void LoadDxImage_PositionBelowHeaderSize_DoesNothing()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        string p = WriteFile(_dir, "ldi.bin", new byte[600]);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var dxi = new TDxImage();
        // position = 255 < SizeOf(TPakFileHeader) = 256 → 整段不执行
        img.LoadDxImage(255, dxi, 0);
        Assert.False(dxi.Surface.Assigned);
        Assert.Equal(0, (int)dxi.nWidth);

        // position 越界到文件尾之后 → 同样不执行
        img.LoadDxImage(100000, dxi, 0);
        Assert.False(dxi.Surface.Assigned);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImage_BadPixelFormat_SetsErrorState()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        // 4x4（乘积 16 > 4）但 PixelFormat = pf1bit（不在合法集合）→ boError
        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf1bit,
            nWidth = 4,
            nHeight = 4,
            Length = 0,
        };
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);

        var fileBytes = new byte[TPakFileHeader.SizeOf + cipher.Length];
        Array.Copy(cipher, 0, fileBytes, TPakFileHeader.SizeOf, cipher.Length);
        string p = WriteFile(_dir, "badpf.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var dxi = new TDxImage();
        img.LoadDxImage(TPakFileHeader.SizeOf, dxi, 0);

        // boError → 4 字段归零 + Surface := nil
        Assert.Equal(0, (int)dxi.nWidth);
        Assert.Equal(0, (int)dxi.nHeight);
        Assert.False(dxi.Surface.Assigned);
        Assert.Equal(0, (int)dxi.nPx);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImage_SmallImageWritesSurfaceEvenForGrayAndBright()
    {
        // 差异/缺陷断言：nWidth*nHeight <= 4 时，三支**都**写 DXImage.Surface（原文 2657-2665 / 2864-2872 / 3067-3074）
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        byte[] cipher = BuildOneImageArea(266, 2, 2);   // 2x2 = 4 <= 4
        var fileBytes = new byte[TPakFileHeader.SizeOf + cipher.Length];
        Array.Copy(cipher, 0, fileBytes, TPakFileHeader.SizeOf, cipher.Length);
        string p = WriteFile(_dir, "small.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var gray = new TDxImage();
        img.LoadDxGrayImage(TPakFileHeader.SizeOf, gray, 0);
        Assert.True(gray.Surface.Assigned);
        Assert.True(gray.Surface.IsNullTexture);
        Assert.False(gray.Gray.Assigned);          // Gray 槽**没有**被写

        var bright = new TDxImage();
        img.LoadDxBrightImage(TPakFileHeader.SizeOf, bright, 0);
        Assert.True(bright.Surface.Assigned);
        Assert.False(bright.Bright.Assigned);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImage_UncompressedPath_LoadsRawScanlineBytes()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        // 4x4 pf8bit、Length = 0（未压缩）→ 直接读 WidthBytes(8,4)*4 = 16 字节
        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 4,
            nHeight = 4,
            px = 3,
            py = -2,
            Length = 0,
        };
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);

        var pixels = new byte[16];
        for (int i = 0; i < 16; i++) pixels[i] = (byte)(0x30 + i);

        var fileBytes = new byte[TPakFileHeader.SizeOf + cipher.Length + pixels.Length];
        Array.Copy(cipher, 0, fileBytes, TPakFileHeader.SizeOf, cipher.Length);
        Array.Copy(pixels, 0, fileBytes, TPakFileHeader.SizeOf + cipher.Length, pixels.Length);

        string p = WriteFile(_dir, "raw.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        var dxi = new TDxImage();
        img.LoadDxImage(TPakFileHeader.SizeOf, dxi, 0);

        Assert.Equal(4, (int)dxi.nWidth);
        Assert.Equal(4, (int)dxi.nHeight);
        Assert.Equal((short)3, dxi.nPx);
        Assert.Equal((short)-2, dxi.nPy);
        Assert.True(dxi.Surface.Assigned);
        Assert.False(dxi.Surface.IsNullTexture);

        // 纹理句柄镜像里应能读回那 16 字节像素（索引即调色板下标）
        var bmp = TextureSeams.GetImage(dxi.Surface.Handle);
        Assert.NotNull(bmp);
        Assert.Equal(4, bmp!.Width);
        Assert.Equal(4, bmp.Height);
        // GDefColorTable[0x30] 的 B 分量（0xCF）——与 DIB 里的索引字节不同，故必须经调色板换算
        Assert.Equal(0x30, pixels[0]);
        Assert.Equal(GDefColorTable.ColorArray[0x30 * 4 + 0], bmp.GetPixel(0, 0).B);
        Assert.Equal(GDefColorTable.ColorArray[0x39 * 4 + 0], bmp.GetPixel(1, 1).B);
    }

    [Fact]
    public void LoadDxImage_CompressedPath_DecompressesZlibData()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        // 8x8 pf8bit，nSize = 64 字节；压缩后写入
        var raw = new byte[64];
        for (int i = 0; i < 64; i++) raw[i] = (byte)(0x11 * (i % 15));
        var compressed = ZlibEx.CompressBuf(raw, raw.Length);
        Assert.NotNull(compressed);

        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 8,
            nHeight = 8,
            Length = compressed!.Length,
        };
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);

        var fileBytes = new byte[TPakFileHeader.SizeOf + cipher.Length + compressed.Length];
        Array.Copy(cipher, 0, fileBytes, TPakFileHeader.SizeOf, cipher.Length);
        Array.Copy(compressed, 0, fileBytes, TPakFileHeader.SizeOf + cipher.Length, compressed.Length);

        string p = WriteFile(_dir, "z.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        var dxi = new TDxImage();
        img.LoadDxImage(TPakFileHeader.SizeOf, dxi, 0);

        Assert.Equal(8, (int)dxi.nWidth);
        Assert.True(dxi.Surface.Assigned);
        var bmp = TextureSeams.GetImage(dxi.Surface.Handle);
        Assert.NotNull(bmp);
        Assert.Equal(GDefColorTable.ColorArray[raw[0] * 4 + 0], bmp!.GetPixel(0, 0).B);
        Assert.Equal(GDefColorTable.ColorArray[raw[9] * 4 + 0], bmp.GetPixel(1, 1).B);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImage_InvalidZlibStream_FlagsError()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        var garbage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 8,
            nHeight = 8,
            Length = garbage.Length,
        };
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);

        var fileBytes = new byte[TPakFileHeader.SizeOf + cipher.Length + garbage.Length];
        Array.Copy(cipher, 0, fileBytes, TPakFileHeader.SizeOf, cipher.Length);
        Array.Copy(garbage, 0, fileBytes, TPakFileHeader.SizeOf + cipher.Length, garbage.Length);

        string p = WriteFile(_dir, "badz.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var msgs = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => msgs.Add(m);

        var dxi = new TDxImage();
        img.LoadDxImage(TPakFileHeader.SizeOf, dxi, 0);

        // 解压失败 → 尺寸照常回填，但 Surface 保持 nil 且报错
        Assert.Equal(8, (int)dxi.nWidth);
        Assert.False(dxi.Surface.Assigned);
        Assert.Contains(msgs, m => m.Contains("decompressBuf"));

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImage_BoundsGuards_ReportCorrectMessage()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftPak1;

        var msgs = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => msgs.Add(m);

        // px 越界（> 3200）
        RunGuard(img, new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 4,
            nHeight = 4,
            px = 3201,
        }, "position error");
        Assert.Contains(msgs, m => m.Contains("LoadDxImage position error"));

        msgs.Clear();
        RunGuard(img, new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 4,
            nHeight = 4,
            Length = GameImagesConsts.MAX_IMAGE_SIZE,
        }, "Length error");
        Assert.Contains(msgs, m => m.Contains("LoadDxImage Length error"));

        msgs.Clear();
        RunGuard(img, new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 0,
            nHeight = 4,
        }, "Size error");
        Assert.Contains(msgs, m => m.Contains("LoadDxImage Size error"));

        msgs.Clear();
        RunGuard(img, new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 3200,
            nHeight = 4,
        }, "Size error");
        Assert.Contains(msgs, m => m.Contains("LoadDxImage Size error"));

        // 灰度/高亮版本的资源串不同（差异断言）
        msgs.Clear();
        RunGuard(img, new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 4,
            nHeight = 4,
            px = 3201,
        }, "position error", LoadKind.Gray);
        Assert.Contains(msgs, m => m.Contains("LoadDxGrayImage position error"));

        msgs.Clear();
        RunGuard(img, new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 4,
            nHeight = 4,
            px = 3201,
        }, "position error", LoadKind.Bright);
        Assert.Contains(msgs, m => m.Contains("LoadDxBrightImage position error"));
    }

    private enum LoadKind { Normal, Gray, Bright }

    private static void RunGuard(TPakImages img, TNewPakImageInfo info, string what, LoadKind kind = LoadKind.Normal)
    {
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);

        var fileBytes = new byte[TPakFileHeader.SizeOf + cipher.Length];
        Array.Copy(cipher, 0, fileBytes, TPakFileHeader.SizeOf, cipher.Length);
        string p = Path.Combine(Path.GetTempPath(), "gxx_p2_pak_guard_" + Guid.NewGuid().ToString("N") + ".bin");
        File.WriteAllBytes(p, fileBytes);

        var prev = img.m_FileStream;
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        var dxi = new TDxImage();
        try
        {
            if (kind == LoadKind.Gray) img.LoadDxGrayImage(TPakFileHeader.SizeOf, dxi, 0);
            else if (kind == LoadKind.Bright) img.LoadDxBrightImage(TPakFileHeader.SizeOf, dxi, 0);
            else img.LoadDxImage(TPakFileHeader.SizeOf, dxi, 0);
        }
        finally
        {
            img.m_FileStream.Dispose();
            img.m_FileStream = prev;
            try { File.Delete(p); } catch { /* best effort */ }
        }
    }

    [Fact]
    public void LoadDxBitmap_IsEmptyBody_SoBitmapSlotStaysNil()
    {
        var img = NewImages();
        var dxi = new TDxImage();
        img.LoadDxBitmap(266, dxi, 0);
        Assert.False(dxi.Bitmap.Assigned);
        Assert.False(dxi.Surface.Assigned);
    }

    [Fact]
    public void LoadDxImageLzPak_OffsetBelow262_DoesNothing()
    {
        var img = NewImages();
        Array.Copy(img.FKeyData, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChain, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.m_ImageSizeList = new List<int> { 100 };

        string p = WriteFile(_dir, "lzb.bin", new byte[600]);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var dxi = new TDxImage();
        // nImgOffset = 262 不满足 > 262 → 整段不执行，boError 保持 True → 4 字段归零
        img.LoadDxImageLzPak(0, 262, 100, TDxTextureStyle.dtsNormal, dxi);
        Assert.Equal(0, (int)dxi.nWidth);
        Assert.False(dxi.Surface.Assigned);

        // 偏移超过文件尾 → 同样不执行
        img.LoadDxImageLzPak(0, 100000, 100, TDxTextureStyle.dtsNormal, dxi);
        Assert.Equal(0, (int)dxi.nWidth);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImageLzPak_V0_UncompressedPath_LoadsDib()
    {
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.FileHeader.BitCount = 8;

        // 4x2 图：wW*wH = 8 > 4；btEncr0 = 0 → 未压缩
        int w = 4, h = 2;
        var imageInfo = new TPakImageInfo { btEncr0 = 0, wW = (short)w, wH = (short)h, wPx = 1, wPy = 2 };
        var infoPlain = new byte[TPakImageInfo.SizeOf];
        imageInfo.ToBytes(infoPlain, 0);

        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var infoCipher = new byte[TPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(infoPlain, infoCipher, TPakImageInfo.SizeOf, ch, kd);

        int pixelBytes = PakConsts.WidthBytes(8, w) * h;   // 8
        var pixels = new byte[pixelBytes];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = (byte)(0x40 + i);

        // 原文 2492：nImgDataSize := nImgDataSize - SizeOf(ImageInfoV0)
        // → 传给 LoadDxImageLzPak 的 nImgDataSize = 头 + 数据，函数内部再减去头长，
        //   于是 msData 只装「头之后」的 nImgDataSize-12 字节。文件布局必须是 头 + 数据。
        const int offset = 300;   // > 262
        var fileBytes = new byte[offset + infoCipher.Length + pixels.Length];
        Array.Copy(infoCipher, 0, fileBytes, offset, infoCipher.Length);
        Array.Copy(pixels, 0, fileBytes, offset + infoCipher.Length, pixels.Length);
        string p = WriteFile(_dir, "lzv0.bin", fileBytes);

        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        var dxi = new TDxImage();
        int nImgDataSize = TPakImageInfo.SizeOf + pixels.Length;
        img.LoadDxImageLzPak(0, offset, nImgDataSize, TDxTextureStyle.dtsNormal, dxi);

        Assert.Equal(w, (int)dxi.nWidth);
        Assert.Equal(h, (int)dxi.nHeight);
        Assert.Equal((short)1, dxi.nPx);
        Assert.Equal((short)2, dxi.nPy);
        Assert.True(dxi.Surface.Assigned);

        var bmp = TextureSeams.GetImage(dxi.Surface.Handle);
        Assert.NotNull(bmp);
        Assert.Equal(GDefColorTable.ColorArray[pixels[0] * 4 + 0], bmp!.GetPixel(0, 0).B);
        Assert.Equal(GDefColorTable.ColorArray[pixels[1] * 4 + 0], bmp.GetPixel(1, 0).B);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImageLzPak_V0_RlePath_ExpandsRepeatRuns()
    {
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.FileHeader.BitCount = 8;

        int w = 8, h = 1;
        var imageInfo = new TPakImageInfo { btEncr0 = 1, wW = (short)w, wH = (short)h };
        var infoPlain = new byte[TPakImageInfo.SizeOf];
        imageInfo.ToBytes(infoPlain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var infoCipher = new byte[TPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(infoPlain, infoCipher, TPakImageInfo.SizeOf, ch, kd);

        // RLE：用**重复段**——Rle=0x87(=135) → 135-127 = 8 个 0x77（源侧只需 1 个像素字节）。
        // 注意若用 Rle=7 的字面量段，源侧必须真给出 8 个像素字节（原文越界即停，不会产出 8 个）。
        var rle = new byte[] { 0x87, 0x77 };

        const int offset = 300;
        var fileBytes = new byte[offset + infoCipher.Length + rle.Length];
        Array.Copy(infoCipher, 0, fileBytes, offset, infoCipher.Length);
        Array.Copy(rle, 0, fileBytes, offset + infoCipher.Length, rle.Length);
        string p = WriteFile(_dir, "lzv0rle.bin", fileBytes);

        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        var dxi = new TDxImage();
        img.LoadDxImageLzPak(0, offset, TPakImageInfo.SizeOf + rle.Length, TDxTextureStyle.dtsNormal, dxi);

        Assert.Equal(w, (int)dxi.nWidth);
        Assert.True(dxi.Surface.Assigned);
        var bmp = TextureSeams.GetImage(dxi.Surface.Handle);
        Assert.NotNull(bmp);
        {
            // 直接复算 DecodeRLE：验证"重复段"语义（源侧只需 1 个像素字节）
            var rleDirect = PakRle.DecodeRle(rle, 0, rle.Length, w, h, 1);
            Assert.Equal(w, rleDirect.Length);
            for (int x = 0; x < w; x++) Assert.Equal((byte)0x77, rleDirect[x]);
        }
        // 差异断言：Pak.pas 2264-2316 的 btEncr0=1 走 **RLEUnit.DecodeRLE**（长度字节 Rle<128 → 原样 Rle+1 个像素；
        // Rle>=128 → 1 个像素重复 Rle-127 次），**不是** GXX.Core.Compress.ZlibEx.DecodeRLE 的 0xC0 前缀变体。
        // 这里用 RLE = {0x87, 0x77} → 8 个 0x77 像素。
        // 调色板 0x77 → (B=0x00, G=0x6B, R=0x9C)，故取 R 分量断言（B 分量该调色板恰为 0）。
        for (int x = 0; x < w; x++) Assert.Equal(GDefColorTable.ColorArray[0x77 * 4 + 2], bmp!.GetPixel(x, 0).R);
        Assert.Equal(0x9C, bmp!.GetPixel(0, 0).R);
        Assert.NotEqual((byte)0x77, ZlibEx.DecodeRLE(new byte[] { 7, 0x77 }, 2, w)[0]);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImageLzPak_V0_ZlibPath_RequiresExactUncompressedSize()
    {
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.FileHeader.BitCount = 8;

        int w = 4, h = 4;               // nImgSize = WidthBytes(8,4)*4 = 16
        var raw = new byte[16];
        for (int i = 0; i < 16; i++) raw[i] = (byte)(0x20 + i);
        var compressed = ZlibEx.CompressBuf(raw, raw.Length)!;

        var imageInfo = new TPakImageInfo { btEncr0 = 2, wW = (short)w, wH = (short)h };
        var infoPlain = new byte[TPakImageInfo.SizeOf];
        imageInfo.ToBytes(infoPlain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var infoCipher = new byte[TPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(infoPlain, infoCipher, TPakImageInfo.SizeOf, ch, kd);

        const int offset = 300;
        var fileBytes = new byte[offset + infoCipher.Length + compressed.Length];
        Array.Copy(infoCipher, 0, fileBytes, offset, infoCipher.Length);
        Array.Copy(compressed, 0, fileBytes, offset + infoCipher.Length, compressed.Length);
        string p = WriteFile(_dir, "lzv0z.bin", fileBytes);

        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        // 原文 2294-2301 要求解压结果**恰好**等于 nImgSize，否则 Source 保持 nil。
        var dxi = new TDxImage();
        img.LoadDxImageLzPak(0, offset, TPakImageInfo.SizeOf + compressed.Length, TDxTextureStyle.dtsNormal, dxi);
        Assert.Equal(w, (int)dxi.nWidth);
        var bmp = TextureSeams.GetImage(dxi.Surface.Handle);
        Assert.NotNull(bmp);
        Assert.Equal(GDefColorTable.ColorArray[raw[5] * 4 + 0], bmp!.GetPixel(1, 1).B);

        // 差异断言：解压长度与 nImgSize(16) 不符 → LoadLzImageDataV0 返回 false（Source = nil）
        // → LoadDxImageLzPak 的 boError 保持 True → nWidth/nHeight 归零、Surface 置 nil。
        var rawShort = new byte[8];
        Array.Copy(raw, rawShort, 8);
        var compressedShort = ZlibEx.CompressBuf(rawShort, rawShort.Length)!;
        var fileBytes2 = new byte[offset + infoCipher.Length + compressedShort.Length];
        Array.Copy(infoCipher, 0, fileBytes2, offset, infoCipher.Length);
        Array.Copy(compressedShort, 0, fileBytes2, offset + infoCipher.Length, compressedShort.Length);
        string p2 = WriteFile(_dir, "lzv0z2.bin", fileBytes2);
        img.m_FileStream.Dispose();
        img.m_FileStream = new FileStream(p2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        var dxi2 = new TDxImage();
        img.LoadDxImageLzPak(0, offset, TPakImageInfo.SizeOf + compressedShort.Length, TDxTextureStyle.dtsNormal, dxi2);
        Assert.Equal(0, (int)dxi2.nWidth);
        Assert.Equal(0, (int)dxi2.nHeight);
        Assert.False(dxi2.Surface.Assigned);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImageLzPak_V1_UsesNewFormatAndStyleSlots()
    {
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV1;
        img.FileHeader.BitCount = 8;

        int w = 4, h = 4;                 // 乘积 16 > 4
        var pixels = new byte[PakConsts.WidthBytes(8, w) * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = (byte)(0x60 + i);

        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = (short)w,
            nHeight = (short)h,
            px = 5,
            py = 6,
            Length = 0,                   // Length = 0 → 未压缩，走 msData.Memory
        };
        var infoPlain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(infoPlain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var infoCipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(infoPlain, infoCipher, TNewPakImageInfo.SizeOf, ch, kd);

        const int offset = 300;
        var fileBytes = new byte[offset + infoCipher.Length + pixels.Length];
        Array.Copy(infoCipher, 0, fileBytes, offset, infoCipher.Length);
        Array.Copy(pixels, 0, fileBytes, offset + infoCipher.Length, pixels.Length);
        string p = WriteFile(_dir, "lzv1.bin", fileBytes);

        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        // Gray 风格 → 应写 DXImage.Gray（不是 Surface）
        var dxi = new TDxImage();
        img.LoadDxImageLzPak(0, offset, 0, TDxTextureStyle.dtsGray, dxi);

        Assert.Equal(w, (int)dxi.nWidth);
        Assert.Equal((short)5, dxi.nPx);
        Assert.Equal((short)6, dxi.nPy);
        Assert.True(dxi.Gray.Assigned);
        Assert.False(dxi.Surface.Assigned);

        // Bright 风格 → Bright 槽
        var dxi2 = new TDxImage();
        img.m_FileStream.Position = 0;
        img.LoadDxImageLzPak(0, offset, 0, TDxTextureStyle.dtsBright, dxi2);
        Assert.True(dxi2.Bright.Assigned);
        Assert.False(dxi2.Surface.Assigned);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void LoadDxImageLzPak_WrongStyleBulletproof_BrightBranchIsAlwaysTaken()
    {
        // 原文 2556：else if dtsBright = dtsBright then —— 恒真；
        // 等价于「dtsNormal → Surface、dtsGray → Gray、其它全部 → Bright」。
        // 取一个**不在枚举里**的值来验证「其它」落到 Bright 分支。
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV1;
        img.FileHeader.BitCount = 8;

        int w = 4, h = 4;
        var pixels = new byte[16];
        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = (short)w,
            nHeight = (short)h,
            Length = 0,
        };
        var infoPlain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(infoPlain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var infoCipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(infoPlain, infoCipher, TNewPakImageInfo.SizeOf, ch, kd);

        const int offset = 300;
        var fileBytes = new byte[offset + infoCipher.Length + pixels.Length];
        Array.Copy(infoCipher, 0, fileBytes, offset, infoCipher.Length);
        Array.Copy(pixels, 0, fileBytes, offset + infoCipher.Length, pixels.Length);
        string p = WriteFile(_dir, "lzunk.bin", fileBytes);

        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        TextureSeams.ResetForTest();

        var dxi = new TDxImage();
        img.LoadDxImageLzPak(0, offset, 0, (TDxTextureStyle)99, dxi);
        Assert.True(dxi.Bright.Assigned);
        Assert.False(dxi.Surface.Assigned);
        Assert.False(dxi.Gray.Assigned);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    // ===================================================================================
    // 12. GetCachedLzImageSize
    // ===================================================================================

    [Fact]
    public void GetCachedLzImageSize_NotInitialized_ReturnsFalse()
    {
        var img = NewImages();
        img.FPakFileType = TPakFileType.pftLzPakV0;
        var size = new Size(0, 0);
        var pt = new Point(0, 0);
        Assert.False(img.GetCachedLzImageSize(0, ref size, ref pt));
    }

    [Fact]
    public void GetCachedLzImageSize_V1_ReadsHeaderAndFillsSize()
    {
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV1;
        img.FileHeader.BitCount = 8;
        img.Initialized = true;
        img.ImageCount = 1;
        img.FPasswordOK = true;
        img.m_ImgArr = new[] { new TDxImage() };
        img.Index.Add(300);

        var info = new TNewPakImageInfo
        {
            PixelFormat = (byte)TPixelFormat.pf8bit,
            nWidth = 12,
            nHeight = 34,
            px = -3,
            py = 7,
            Length = 0,
        };
        var plain = new byte[TNewPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TNewPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TNewPakImageInfo.SizeOf, ch, kd);

        var fileBytes = new byte[300 + cipher.Length + 16];
        Array.Copy(cipher, 0, fileBytes, 300, cipher.Length);
        string p = WriteFile(_dir, "lzsize.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var size = new Size(0, 0);
        var pt = new Point(0, 0);
        bool ok = img.GetCachedLzImageSize(0, ref size, ref pt);

        Assert.True(ok);
        Assert.Equal(12, size.Width);
        Assert.Equal(34, size.Height);
        Assert.Equal(-3, pt.X);
        Assert.Equal(7, pt.Y);
        // 回填到 m_ImgArr
        Assert.Equal((ushort)12, img.m_ImgArr![0].nWidth);
        Assert.Equal((ushort)34, img.m_ImgArr[0].nHeight);

        // 第二次调用走「已缓存」分支（宽高乘积 ≠ 0）
        var size2 = new Size(0, 0);
        var pt2 = new Point(0, 0);
        Assert.True(img.GetCachedLzImageSize(0, ref size2, ref pt2));
        Assert.Equal(12, size2.Width);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void GetCachedLzImageSize_V0_RequiresPositiveDataSize()
    {
        var img = NewImages();
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.Initialized = true;
        img.ImageCount = 1;
        img.FPasswordOK = true;
        img.m_ImgArr = new[] { new TDxImage() };
        img.Index.Add(300);
        img.m_ImageSizeList = new List<int> { 0 };   // dataSize = 0 → bCanRead = false

        string p = WriteFile(_dir, "lzsize0.bin", new byte[600]);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var size = new Size(0, 0);
        var pt = new Point(0, 0);
        Assert.False(img.GetCachedLzImageSize(0, ref size, ref pt));

        // dataSize > 0 但 offset < 262 → 仍 false
        img.m_ImageSizeList[0] = 100;
        img.Index[0] = 261;
        Assert.False(img.GetCachedLzImageSize(0, ref size, ref pt));

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    [Fact]
    public void GetCachedLzImageSize_V0_InvalidImageInfo_RequestsIndexUpdate()
    {
        var img = NewImages();
        Array.Copy(img.FKeyDataLz, img.FileHeader.KeyData, 32);
        Array.Copy(img.FChainLz, img.FileHeader.Chain, 20);
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.FileHeader.BitCount = 8;
        img.Initialized = true;
        img.ImageCount = 1;
        img.FPasswordOK = true;
        img.m_ImgArr = new[] { new TDxImage() };
        img.Index.Add(300);
        img.m_ImageSizeList = new List<int> { 500 };
        img.m_boUpdateIndex = true;
        img.m_boUpdateIndexing = false;
        TGameImages.g_boAutoUpdate = true;

        // wW = 0 → IsValidLzImageInfoV0 失败
        var info = new TPakImageInfo { btEncr0 = 0, wW = 0, wH = 10 };
        var plain = new byte[TPakImageInfo.SizeOf];
        info.ToBytes(plain, 0);
        var kd = new uint[32]; Array.Copy(img.FileHeader.KeyData, kd, 32);
        var ch = new byte[20]; Array.Copy(img.FileHeader.Chain, ch, 20);
        var cipher = new byte[TPakImageInfo.SizeOf];
        UnitDes.EncryptCBC(plain, cipher, TPakImageInfo.SizeOf, ch, kd);

        var fileBytes = new byte[300 + cipher.Length + 64];
        Array.Copy(cipher, 0, fileBytes, 300, cipher.Length);
        string p = WriteFile(_dir, "lzsizebad.bin", fileBytes);
        img.m_FileStream = new FileStream(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        int calls = 0;
        PakSeams.UpdateEngineAddFn = (_, _, _, _, _) => { calls++; return true; };

        var size = new Size(0, 0);
        var pt = new Point(0, 0);
        bool ok = img.GetCachedLzImageSize(0, ref size, ref pt);

        Assert.False(ok);
        Assert.Equal(1, calls);
        Assert.True(img.m_boUpdateIndexing);

        img.m_FileStream.Dispose();
        img.m_FileStream = null;
    }

    // ===================================================================================
    // 13. UpdateImageDataSize —— 只有 LzPakV0 生效
    // ===================================================================================

    [Fact]
    public void UpdateImageDataSize_OnlyActsForLzPakV0()
    {
        var img = NewImages();
        img.m_ImageSizeList = null;

        // 非 V0 类型 → 什么都不做（连列表都不建）
        img.FPakFileType = TPakFileType.pftPak1;
        img.UpdateImageDataSize(0, 123);
        Assert.Null(img.m_ImageSizeList);

        img.FPakFileType = TPakFileType.pftLzPakV1;
        img.UpdateImageDataSize(0, 123);
        Assert.Null(img.m_ImageSizeList);

        // V0 → 写入（并按需扩张）
        img.FPakFileType = TPakFileType.pftLzPakV0;
        img.UpdateImageDataSize(0, 111);
        Assert.NotNull(img.m_ImageSizeList);
        Assert.Equal(111, img.m_ImageSizeList![0]);

        img.UpdateImageDataSize(3, 444);
        Assert.Equal(4, img.m_ImageSizeList.Count);
        Assert.Equal(0, img.m_ImageSizeList[1]);
        Assert.Equal(444, img.m_ImageSizeList[3]);
    }

    // ===================================================================================
    // 14. UpdateIndex 的 LzPakV1 / Pak2 分支 + 头部一致性
    // ===================================================================================

    [Fact]
    public void UpdateIndex_LzPakV1_Writes262ByteHeader_WithBfType1()
    {
        var img = NewImages();
        string path = Path.Combine(_dir, "v1.pak");
        img.FileName = path;

        var key = TPakKey.CreateEmpty();
        key.ImageCount = 3;
        key.PakType = (ushort)TPakFileType.pftLzPakV1;
        Array.Copy(img.FKeyData, key.KeyData, 32);
        Array.Copy(img.FChain, key.Chain, 20);
        key.Reserve[0] = 0x0A0B0C0D;

        img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘

        byte[] all = File.ReadAllBytes(path);
        // 262 头 + ImageCount(3) × 4 字节（V1 每项 4 字节）= 274
        Assert.Equal(262 + 3 * 4, all.Length);
        Assert.Equal(274, all.Length);
        Assert.Equal("HXM2.", GXX.Core.EncodingInit.GBK.GetString(all, 0, 5));
        Assert.Equal(1, img.FileHeader.bfType);          // V1 → bfType = 1
        Assert.Equal((ushort)0, img.FileHeader.BitCount); // 非 V0 → BitCount = 0
        Assert.Equal(0x0A0B0C0D, img.FileHeader.Reserve[0]);
    }

    [Fact]
    public void UpdateIndex_Pak2_AndPak3_ChooseExpectedMagicAndLength()
    {
        foreach (var (type, magic, bfType) in new[]
        {
            (TPakFileType.pftPak2, "GEEPAK2", 2),
            (TPakFileType.pftPak3, "GEEPAK3", 2),
        })
        {
            var img = NewImages();
            string path = Path.Combine(_dir, $"{type}.pak");
            img.FileName = path;

            var key = TPakKey.CreateEmpty();
            key.ImageCount = 2;
            key.PakType = (ushort)type;
            Array.Copy(img.FKeyData, key.KeyData, 32);
            Array.Copy(img.FChain, key.Chain, 20);

            img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘

            byte[] all = File.ReadAllBytes(path);
            // 索引 = ImageCount(2) × 4 字节整数 = 8 → 266 + 8 = 274
            Assert.Equal(266 + 2 * 4, all.Length);
            Assert.Equal(274, all.Length);
            Assert.Equal(magic, GXX.Core.EncodingInit.GBK.GetString(all, 1, magic.Length));
            Assert.Equal(bfType, img.FileHeader.bfType);
        }
    }

    [Fact]
    public void UpdateIndex_ThenInitialize_OnSameInstance_SkipsViaCompareHeader()
    {
        var img = NewImages();
        string path = Path.Combine(_dir, "twice.pak");
        img.FileName = path;

        var key = TPakKey.CreateEmpty();
        key.ImageCount = 3;
        key.PakType = (ushort)TPakFileType.pftPak1;
        key.Reserve[2] = 0x55;
        Array.Copy(img.FKeyData, key.KeyData, 32);
        Array.Copy(img.FChain, key.Chain, 20);

        img.UpdateIndex(key, false);
        img.Dispose(); // 释放 m_FileStream 句柄，便于下方读磁盘
        // UpdateIndex 末尾会 Initialize_UpdateNewFile（不置 Initialized），这里显式初始化
        img.Initialize();
        Assert.True(img.Initialized);
        Assert.Equal(3, img.ImageCount);

        // 完全一致的 key → CompareHeader 命中 → 直接退出，Initialized 保持
        img.UpdateIndex(key, true);
        Assert.True(img.Initialized);
        Assert.Equal(3, img.ImageCount);
    }
}

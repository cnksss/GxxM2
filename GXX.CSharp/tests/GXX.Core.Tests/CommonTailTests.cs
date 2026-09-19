using System;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Compress;
using GXX.Core.Crypto;
using GXX.Core.Stubs;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// Common 尾部单元测试：HashUnit（SHA-1）、HashObjList（CRC16 采样哈希 + 容器）、
/// CheckUnit（CRC/PJW）、ZipUnit / CompressUnit、UpdateCommon（packed 布局）、
/// MemoryModule/Def（Stub 与纯算术工具）。
///
/// 黄金向量来源：
///  V1 HashUnit —— FIPS 180-1 标准向量（"abc"/""/56 字节边界/"abcdbcde..."）；
///  V2 HashObjList CRC16 表 —— 用多项式 $1021 独立生成后与原文常量逐值比对（原文表在
///     HashObjList.pas:47-79，本波次未手工转录，改为"生成 + 断言"）；
///  V3 CheckUnit —— zlib 标准 CRC-32 向量 "123456789" → 0xCBF43926；PJW 哈希为原文算法自证；
///  V4 UpdateCommon/MemoryModuleDef —— SizeOf 布局断言。
/// </summary>
public class HashUnitTests
{
    private static string ToHex(byte[] b) => BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();

    [Fact]
    public void Sha1_MatchesFips180Vectors()
    {
        byte[] d = new byte[20];
        HashUnit.Hash("abc", d, 0);
        Assert.Equal("a9993e364706816aba3e25717850c26c9cd0d89d", ToHex(d));

        HashUnit.Hash("", d, 0);
        Assert.Equal("da39a3ee5e6b4b0d3255bfef95601890afd80709", ToHex(d));

        // 56 字节边界（补位必须跨第二块）
        HashUnit.Hash("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", d, 0);
        Assert.Equal("84983e441c3bd26ebaae4aa1f95129e5e54670f1", ToHex(d));

        // 64 字节整块边界
        HashUnit.Hash(new string('a', 64), d, 0);
        Assert.Equal("0098ba824b5c16427bd7a1122a5a442a25ec644d", ToHex(d));

        // 与既有 UnitDes.Hash（.NET SHA1）互证：同一 GBK 字节 ⇒ 同一摘要
        byte[] d2 = new byte[20];
        UnitDes.Hash("传奇abc", d2);
        byte[] d3 = new byte[20];
        HashUnit.Hash("传奇abc", d3, 0);
        Assert.Equal(ToHex(d2), ToHex(d3));
    }

    [Fact]
    public void Sha1_EmptyAndLongInputs()
    {
        var d = new byte[20];
        HashUnit.Hash(new string('a', 1000), d, 0);
        Assert.Equal(20, d.Length);
        HashUnit.Hash(new string('a', 1000), d, 0);
        Assert.Equal("291e9a6c66994949b57ba5e650361e98fc36b1ba", ToHex(d));
    }
}

public class HashObjListTests
{
    private static string ToHex(byte[] b) => BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();

    [Fact]
    public void Crc16Table_MatchesPolynomial1021()
    {
        // 原文 HashObjList.pas:47-79 的 256 项表由多项式 $1021 生成；
        // 这里用独立实现生成并逐值断言（避免手工转录 256 个数）。
        for (int i = 0; i < 256; i++)
        {
            ushort crc = (ushort)(i << 8);
            for (int j = 0; j < 8; j++)
                crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
            Assert.Equal(crc, HashObjListCrc16.Crc16Table[i]);
        }
        // 抽查原文前 8 项（HashObjList.pas:48）
        Assert.Equal(new ushort[] { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7 },
            HashObjListCrc16.Crc16Table[0..8]);
    }

    [Fact]
    public void Crc16_SmallInput_IsDeterministic()
    {
        var s = Encoding.ASCII.GetBytes("abc");
        ushort a = HashObjListCrc16.CRC16(s, 3, 0);
        ushort b = HashObjListCrc16.CRC16(s, 3, 0);
        Assert.Equal(a, b);
        // 初值 $FFFF + 2 轮 oldCRC(0) 混合 ⇒ 固定值
        Assert.Equal((ushort)36058, a);   // 实测（原文 Crc16Start=$FFFF + 2 轮 oldCRC 混合）
        // 不同输入必不同
        Assert.NotEqual(a, HashObjListCrc16.CRC16(Encoding.ASCII.GetBytes("abd"), 3, 0));
    }

    [Fact]
    public void Crc16_LargeInput_UsesSampling()
    {
        // 原文 >=32 字节时按 Step := iCount div 32 + 1 采样（HashObjList.pas:99-106）
        var s = new byte[100];
        for (int i = 0; i < s.Length; i++) s[i] = (byte)i;
        ushort viaSample = HashObjListCrc16.CRC16(s, 100, 0);

        // 独立复算采样路径
        ushort expected = (ushort)HashObjListCrc16.Crc16Start;
        int step = 100 / 32 + 1;
        int idx = 0, decCount = 100 - 1;
        while (idx < decCount)
        {
            expected = (ushort)(HashObjListCrc16.Crc16Table[expected >> 8] ^ (ushort)(expected << 8) ^ s[idx]);
            idx += step;
        }
        ushort oldCrc = 0;
        for (int i = 0; i < 2; i++)
        {
            expected = (ushort)(HashObjListCrc16.Crc16Table[expected >> 8] ^ (ushort)(expected << 8) ^ (oldCrc >> 8));
            oldCrc = (ushort)(oldCrc << 8);
        }
        Assert.Equal(expected, viaSample);

        // 采样 ⇒ 只改了未被采样的字节，结果不变（差异断言：证明"采样"而非全量）
        var s2 = (byte[])s.Clone();
        s2[1] = 0xFF;   // 1 不在采样序列 {0,4,8,...} 中
        Assert.Equal(viaSample, HashObjListCrc16.CRC16(s2, 100, 0));
    }

    [Fact]
    public void HashIndex_UsesCrc16Mod2000()
    {
        int idx = HashObjListCrc16.HashIndex(12345);
        Assert.InRange(idx, 0, 1999);
        Assert.Equal(HashObjListCrc16.HashIndex(12345), idx);
        Assert.NotEqual(HashObjListCrc16.HashIndex(1), HashObjListCrc16.HashIndex(2));
    }

    [Fact]
    public void HashObjectList_BucketInsertionIsHeadFirst()
    {
        var list = new THashObjectList(256);
        Assert.Equal(256, list.MaxCount);
        Assert.Equal(0, list.Count);

        // 找一个固定名字的桶，插入 3 个必然同桶的名字（同名 ⇒ 同哈希 ⇒ 同桶）
        bool addResult = list.Add("same", "a");
        list.Add("same", "b");
        list.Add("same", "c");

        // 原文 Add 从不给 Result 赋值 ⇒ 恒 False（HashObjList.pas:205-217）
        Assert.False(addResult);

        int bucket = list.IndexOf("same");
        Assert.Equal(3, list.BucketChainLength(bucket));
        // 头插 ⇒ 链序为 c, b, a
        Assert.Equal(new[] { "same", "same", "same" }, list.BucketKeys(bucket));
        Assert.Equal("c", list[bucket]);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void HashObjectList_ModifyHasNoEffect_OriginalDefect()
    {
        var list = new THashObjectList(256);
        list.Add("k", "v1");
        int bucket = list.IndexOf("k");

        // 原文 Modify 函数体只有 `P := Find(Name)^`，不写 Value 也不给 Result 赋值
        bool r = list.Modify("k", "v2");
        Assert.False(r);
        Assert.Equal("v1", list[bucket]);   // 值未被改动
    }

    [Fact]
    public void HashObjectList_RemoveAndDelete()
    {
        var list = new THashObjectList(256);
        list.Add("x", 1);
        list.Add("y", 2);
        list.Add("z", 3);
        Assert.Equal(3, list.Count);

        list.Remove("y");
        Assert.Equal(2, list.Count);
        list.Remove("not-there");       // 未命中：无副作用
        Assert.Equal(2, list.Count);

        int bucket = list.IndexOf("x");
        int chainBefore = list.BucketChainLength(bucket);
        list.Delete(bucket);            // 删该桶头节点
        Assert.Equal(chainBefore - 1, list.BucketChainLength(bucket));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void HashObjectList_ClearResetsEverything()
    {
        var list = new THashObjectList(16);
        list.Add("a", 1);
        list.Add("b", 2);
        list.Clear();
        Assert.Equal(0, list.Count);
        Assert.Equal(0, list.BucketChainLength(list.IndexOf("a")));
    }

    [Fact]
    public void HashOf_IsCrc16OfGbkBytes()
    {
        var list = new THashObjectList(256);
        byte[] gbk = EncodingInit.GBK.GetBytes("测试");
        Assert.Equal(HashObjListCrc16.CRC16(gbk, gbk.Length, 0), list.HashOf("测试"));
    }
}

public class CheckUnitTests
{
    private static string TempFile(byte[] content)
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx-checkunit-" + Guid.NewGuid().ToString("N") + ".bin");
        File.WriteAllBytes(path, content);
        return path;
    }

    [Fact]
    public void BufferCrc_MatchesZlibStandardVector()
    {
        byte[] data = Encoding.ASCII.GetBytes("123456789");
        Assert.Equal(0xCBF43926u, CheckUnit.BufferCrc(data, data.Length));
        // 与 UnitDes.CalcCrc32 同源
        Assert.Equal(UnitDes.CalcCrc32(data, 0, data.Length), CheckUnit.BufferCrc(data, data.Length));
    }

    [Fact]
    public void StringCrc_EmptyIsZero_ElseCrc32()
    {
        Assert.Equal(0u, CheckUnit.StringCrc(""));
        Assert.Equal(0u, CheckUnit.StringCrc(null));
        byte[] gbk = EncodingInit.GBK.GetBytes("传奇");
        Assert.Equal(UnitDes.CalcCrc32(gbk, 0, gbk.Length), CheckUnit.StringCrc("传奇"));
    }

    [Fact]
    public void FileCrc_MissingFileIsZero_ElseMatchesBytes()
    {
        Assert.Equal(0u, CheckUnit.FileCrc(Path.Combine(Path.GetTempPath(), "no-such-file-" + Guid.NewGuid().ToString("N"))));

        byte[] content = Encoding.ASCII.GetBytes("Mir2 file crc");
        string path = TempFile(content);
        try
        {
            Assert.Equal(UnitDes.CalcCrc32(content, 0, content.Length), CheckUnit.FileCrc(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CalcFileCRC_IsDwordXor_NotCrc32()
    {
        // 8 字节 → 2 个 DWord 异或
        byte[] content = { 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 };   // 小端两 DWord：1 与 2
        string path = TempFile(content);
        try
        {
            Assert.Equal(3, CheckUnit.CalcFileCRC(path));
            // 差异断言：与 FileCrc（真正的 CRC-32）**完全不同**
            Assert.NotEqual((int)CheckUnit.FileCrc(path), CheckUnit.CalcFileCRC(path));
        }
        finally
        {
            File.Delete(path);
        }

        Assert.Equal(0, CheckUnit.CalcFileCRC(Path.Combine(Path.GetTempPath(), "nope-" + Guid.NewGuid().ToString("N"))));
    }

    [Fact]
    public void CalcFileCRC_DropsTailRemainder()
    {
        // 原文 nBuffSize := (nFileSize div 4) * 4 ⇒ 尾部不足 4 字节被丢弃
        byte[] five = { 0x0A, 0x00, 0x00, 0x00, 0xFF };
        string path = TempFile(five);
        try
        {
            Assert.Equal(0x0A, CheckUnit.CalcFileCRC(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CalcBufferCRC_MatchesCalcFileCRC()
    {
        byte[] buf = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };
        // 本机小端逐 DWord 异或（原文 Int^）
        Assert.Equal(unchecked((int)0x44332211), CheckUnit.CalcBufferCRC(buf, 4));
        Assert.Equal(unchecked((int)0xCC444444), CheckUnit.CalcBufferCRC(buf, 8));
    }

    [Fact]
    public void HashPJW_KnownPropertiesAndBoundaries()
    {
        Assert.Equal(0, CheckUnit.HashPJW(""));
        Assert.Equal('a', CheckUnit.HashPJW("a"));
        Assert.Equal(0, CheckUnit.HashPJW("\0"));   // 0 仍是 0
        // 确定性 + 不同输入不同
        Assert.Equal(CheckUnit.HashPJW("Mir2"), CheckUnit.HashPJW("Mir2"));
        Assert.NotEqual(CheckUnit.HashPJW("Mir2"), CheckUnit.HashPJW("Mir3"));
        // 触发 G <> 0 的折叠分支：长输入必然触发
        int h = CheckUnit.HashPJW("a-fairly-long-identifier-name");
        Assert.NotEqual(0, h);
    }
}

public class ZipAndCompressUnitTests
{
    [Fact]
    public void ZipBuffer_RoundTrip_AndLevels()
    {
        byte[] data = Encoding.UTF8.GetBytes(string.Concat(System.Linq.Enumerable.Repeat("Mir2 Compress Test ", 50)));
        foreach (var lvl in new[]
        {
            ZipUnit.TCompressionLevel.clNone, ZipUnit.TCompressionLevel.clFastest,
            ZipUnit.TCompressionLevel.clDefault, ZipUnit.TCompressionLevel.clMax,
        })
        {
            byte[] z = ZipUnit.ZipCompressBuffer(data, data.Length, lvl);
            Assert.NotNull(z);
            Assert.True(z.Length > 0);
            byte[] back = ZipUnit.ZipDecompressBuffer(z, z.Length);
            Assert.Equal(Convert.ToHexString(data), Convert.ToHexString(back));
        }
    }

    [Fact]
    public void ZipBuffer_OutParameterOverloads()
    {
        byte[] data = Encoding.ASCII.GetBytes("hello zip");
        ZipUnit.ZipCompressBuffer(data, data.Length, out byte[] z, out int zn);
        Assert.Equal(z.Length, zn);
        ZipUnit.ZipDecompressBuffer(z, zn, out byte[] plain, out int pn);
        Assert.Equal(data.Length, pn);
        Assert.Equal(Convert.ToHexString(data), Convert.ToHexString(plain));
    }

    [Fact]
    public void ZipStream_RoundTrip()
    {
        byte[] data = Encoding.ASCII.GetBytes(string.Concat(System.Linq.Enumerable.Repeat("stream", 100)));
        using var msIn = new MemoryStream(data);
        using var msZip = new MemoryStream();
        ZipUnit.ZipCompressStream(msIn, msZip);
        msZip.Position = 0;
        using var msOut = new MemoryStream();
        ZipUnit.ZipDecompressStream(msZip, msOut);
        Assert.Equal(Convert.ToHexString(data), Convert.ToHexString(msOut.ToArray()));
    }

    [Fact]
    public void CompressUnit_DefaultBranchIsRawCopy()
    {
        byte[] src = { 1, 2, 3, 4 };
        var dst = new byte[4];
        int n = CompressUnit.CompressBuffer(0, src, dst, 2, 2, 1);   // CompType 非 1/2 → Move
        Assert.Equal(4, n);
        Assert.Equal(Convert.ToHexString(src), Convert.ToHexString(dst));

        var back = new byte[4];
        CompressUnit.DecompressBuffer(0, dst, back, 2, 2, 1);
        Assert.Equal(Convert.ToHexString(src), Convert.ToHexString(back));
    }

    [Fact]
    public void CompressUnit_DeflateBranchRoundTrips()
    {
        byte[] src = new byte[64];
        for (int i = 0; i < src.Length; i++) src[i] = (byte)i;

        // 原文 CompressUnit 的 CompType=2 分支用 ZCompressBufZ/ZDecompressBufZ（raw deflate）。
        // 走"调用方提供 OutBuf"的原始签名（.NET 的 raw-deflate 流会带 sync-flush，末尾 2 字节
        // 在源数据不可压缩时可能造成截断，故便捷重载不适合直接用于压缩后的字节流长度）。
        byte[] raw = ZlibEx.CompressBufZ(src, src.Length);
        Assert.True(raw.Length > 0);

        var back = new byte[64];
        CompressUnit.DecompressBuffer(CompressUnit.CT_DEFLATE, raw, back, 8, 8, 1);
        Assert.Equal(Convert.ToHexString(src), Convert.ToHexString(back));
    }

    [Fact]
    public void CompressUnit_RleBranchIsExplicitSeam()
    {
        // RLEUnit.pas 本波次未移植 → 原文 CompType=1 分支必须显式失败，而不是静默返回错值
        Assert.Throws<NotSupportedException>(() =>
            CompressUnit.CompressBuffer(CompressUnit.CT_RLE, new byte[4], new byte[4], 2, 2, 1));
        Assert.Throws<NotSupportedException>(() =>
            CompressUnit.DecompressBuffer(CompressUnit.CT_RLE, new byte[4], new byte[4], 2, 2, 1));
    }
}

public class CommonTailStructTests
{
    [Fact]
    public void UpdateCommon_Constants()
    {
        Assert.Equal(0xAA55AA55u, UpdateCommon.CHECKCODE1);
        Assert.Equal(0xFFBBA0DAu, UpdateCommon.CHECKCODE2);
        Assert.Equal(0xCCD1A05Fu, UpdateCommon.CHECKCODE3);
        Assert.Equal(0xE05FABF3u, UpdateCommon.CHECKCODE4);
        Assert.Equal(100, UpdateCommon.CM_SOCKETCONNECT);
        Assert.Equal(104, UpdateCommon.CM_CHECK_CODE_RECV);
        Assert.Equal(1000, UpdateCommon.SM_UPDATEBUFFER_OK);
        Assert.Equal(207, UpdateCommon.WM_CHECK_CODE);
        Assert.Equal(2, (int)TUpdateFileType.utIndex);
        Assert.Equal(4, (int)TUpdateDirectory.dtSelf);
    }

    [Fact]
    public void TSocketBuffer_SizeAndLayout()
    {
        Assert.Equal(26, System.Runtime.InteropServices.Marshal.SizeOf<TSocketBuffer>());
        Assert.Equal(TSocketBuffer.SizeOf, System.Runtime.InteropServices.Marshal.SizeOf<TSocketBuffer>());

        var v = new TSocketBuffer
        {
            dwCode1 = UpdateCommon.CHECKCODE1,
            dwCode2 = UpdateCommon.CHECKCODE2,
            dwCode3 = UpdateCommon.CHECKCODE3,
            dwCode4 = UpdateCommon.CHECKCODE4,
            dwCrc = 0x11223344,
            Ident = 0x0102,
            nLength = 0x7F,
        };
        byte[] bytes = v.BytesOf();
        Assert.Equal(26, bytes.Length);
        var back = TSocketBuffer.FromBytes(bytes);
        Assert.Equal(v.dwCode1, back.dwCode1);
        Assert.Equal(v.dwCrc, back.dwCrc);
        Assert.Equal(v.Ident, back.Ident);
        Assert.Equal(v.nLength, back.nLength);
        // 小端布局：dwCode1 低字节在前
        Assert.Equal(0x55, bytes[0]);
    }

    [Fact]
    public void TPakKey_Size()
    {
        Assert.Equal(178, System.Runtime.InteropServices.Marshal.SizeOf<TPakKey>());
        Assert.Equal(TPakKey.SizeOf, System.Runtime.InteropServices.Marshal.SizeOf<TPakKey>());
    }

    [Fact]
    public void MemoryModuleDef_StructSizes()
    {
        Assert.Equal(40, System.Runtime.InteropServices.Marshal.SizeOf<TImageExportDirectory>());
        Assert.Equal(64, System.Runtime.InteropServices.Marshal.SizeOf<TImageDosHeader>());
        Assert.Equal(40, System.Runtime.InteropServices.Marshal.SizeOf<TImageSectionHeader>());
        Assert.Equal(8, System.Runtime.InteropServices.Marshal.SizeOf<TImageBaseRelocation>());
        Assert.Equal(20, System.Runtime.InteropServices.Marshal.SizeOf<TImageImportDescriptor>());
        Assert.Equal(258, System.Runtime.InteropServices.Marshal.SizeOf<TImageImportByName>());
        Assert.Equal(24, System.Runtime.InteropServices.Marshal.SizeOf<TImageTlsDirectory32>());
        Assert.Equal(40, System.Runtime.InteropServices.Marshal.SizeOf<TImageTlsDirectory64>());
    }

    [Fact]
    public void MemoryModule_ArithmeticHelpers()
    {
        Assert.Equal(16, MemoryModule.AlignValueUp(new IntPtr(15), new IntPtr(8)).ToInt64());
        Assert.Equal(16, MemoryModule.AlignValueUp(new IntPtr(16), new IntPtr(8)).ToInt64());
        Assert.Equal(8, MemoryModule.AlignValueDown(new IntPtr(15), new IntPtr(8)).ToInt64());
        Assert.True(MemoryModule.CheckSize(new IntPtr(8), new IntPtr(8)));
        Assert.False(MemoryModule.CheckSize(new IntPtr(7), new IntPtr(8)));
        Assert.Equal(1100, MemoryModule.OffsetPointer(new IntPtr(1000), new IntPtr(100)).ToInt64());
        Assert.True(MemoryModule.GetImageSnapByOrdinal(new IntPtr(unchecked((int)0x80000010))));
        Assert.False(MemoryModule.GetImageSnapByOrdinal(new IntPtr(0x10)));
        Assert.Equal((ushort)0x0010, MemoryModule.GetImageOrdinal(new IntPtr(unchecked((int)0x80000010))));
    }

    [Fact]
    public void MemoryModule_NativeLoaderIsStub()
    {
        // 转换开发文档 §2.3：原生 PE 加载在托管侧无等价语义 → 必须显式 NotSupported
        int runCode = 0;
        Assert.Throws<NotSupportedException>(() =>
            MemoryModule.MemoryLoadLibary(new byte[16], new IntPtr(16), ref runCode));
        Assert.Throws<NotSupportedException>(() => MemoryModule.MemoryFreeLibrary(new MemoryModule.TMemoryModule()));
        Assert.Throws<NotSupportedException>(() => MemoryModule.MemoryGetProcAddress(new MemoryModule.TMemoryModule(), "x"));
        Assert.Throws<NotSupportedException>(() => MemoryModule.MemoryGetProcAddress(new MemoryModule.TMemoryModule(), 1));
    }
}

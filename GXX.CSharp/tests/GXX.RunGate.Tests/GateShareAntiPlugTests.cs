// 测试：Source\RunGate\GateShare.pas（实测 LF 3595）的**客户端反外挂模块加载**族
//   → src/GXX.RunGate/GateShareAntiPlug.cs
//
// 覆盖策略：RSA（LbRSA，本仓库缺失）与 DES 两步走**注入接缝**，其余流程（FileFlag 校验 / 文件布局 /
// DllLen 比对 / RandKey XOR / CRC / zLib / 分块数）全部真跑。原文缺陷写成差异断言。
using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

[Collection("RunGateFormLane")]      // 共享 GateShareGlobals 静态量 + GateSharePaths，必须串行
public sealed class GateShareAntiPlugTests : IDisposable
{
    private readonly string _dir;

    public GateShareAntiPlugTests()
    {
        GateShareGlobals.ResetForTest();
        FormGlobals.ResetForTest();
        _dir = Path.Combine(Path.GetTempPath(), "p2rg-antiplug-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        GateSharePaths.SetExeDirForTest(_dir);
    }

    public void Dispose()
    {
        GateSharePaths.ResetForTest();
        GateShareGlobals.ResetForTest();
        FormGlobals.ResetForTest();
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { }
    }

    private sealed class FakeCipher : IClientAntiPlugCipher
    {
        public byte[] RsaOut;
        public byte[] LastKey;
        public int LastLen;
        public int RsaCalls;
        public int LastOffset;
        public int LastEncLen;

        public byte[] RsaDecryptBuffer(byte[] src, int offset, int len)
        {
            RsaCalls++; LastOffset = offset; LastEncLen = len;
            return RsaOut;
        }

        public void DecryptDesInPlace(byte[] data, int len, byte[] key)
        {
            LastKey = (byte[])key.Clone();
            LastLen = len;
            // 恒等（测试里不需要真 DES；CRC 直接对明文算）
        }
    }

    /// <summary>构造 <c>client_module.dat</c> 的字节布局：FileFlag(4) + EncLen(4) + Enc(EncLen) + dll(remain)。</summary>
    private static byte[] BuildModuleFile(uint fileFlag, byte[] enc, byte[] dllCipher)
    {
        var ms = new MemoryStream();
        var w = new BinaryWriter(ms);
        w.Write(fileFlag);
        w.Write(enc.Length);
        w.Write(enc);
        w.Write(dllCipher);
        w.Flush();
        return ms.ToArray();
    }

    private static byte[] BuildAddData(uint rungateType, uint dllCrc, uint dllLen, byte[] randKey)
    {
        var b = new byte[GateShareAntiPlug.ANTI_PLUG_ADD_DATA_SIZE];
        BitConverter.GetBytes(rungateType).CopyTo(b, 0);
        BitConverter.GetBytes(dllCrc).CopyTo(b, 4);
        BitConverter.GetBytes(dllLen).CopyTo(b, 8);
        Array.Copy(randKey, 0, b, 12, 8);
        return b;
    }

    // ================= 常量表（脚本抽取 + 回读） =================

    [Fact]
    public void FileFlags表6项且当前生效IOCP6()
    {
        Assert.Equal(6, GateShareAntiPlug.FileFlags.Length);
        Assert.Equal(0x5E462548u, GateShareAntiPlug.FileFlags[0]);   // 原 :2078
        Assert.Equal(0x4FB74474u, GateShareAntiPlug.FileFlags[1]);   // 原 :2081
        Assert.Equal(0xA1DFE8F0u, GateShareAntiPlug.FileFlags[2]);   // 原 :2084
        Assert.Equal(0x22CE2669u, GateShareAntiPlug.FileFlags[3]);   // 原 :2087
        Assert.Equal(0x9EA5AC55u, GateShareAntiPlug.FileFlags[4]);   // 原 :2090
        Assert.Equal(0x0533BF07u, GateShareAntiPlug.FileFlags[5]);   // 原 :2093
        Assert.Equal(6, GateShareAntiPlug.RungateLEG_IOCP);          // Grobal2_Ex.pas:32
        Assert.Equal(0x0533BF07u, GateShareAntiPlug.ActiveFileFlag);
    }

    [Fact]
    public void KeyTables6组且当前生效IOCP6()
    {
        Assert.Equal(6, GateShareAntiPlug.KeyTables.Length);
        Assert.All(GateShareAntiPlug.KeyTables, k => Assert.Equal(8, k.Length));
        Assert.Equal(new byte[] { 0xA7, 0x45, 0x32, 0xBB, 0x3D, 0x6A, 0x7F, 0x90 }, GateShareAntiPlug.ActiveKey);
        Assert.Equal(new byte[] { 0x3D, 0xEF, 0x8E, 0x6F, 0x51, 0x4F, 0x1D, 0x9F }, GateShareAntiPlug.KeyTables[0]);
        Assert.Equal(new byte[] { 0xE7, 0x67, 0x37, 0x30, 0x33, 0x50, 0x4A, 0xA4 }, GateShareAntiPlug.KeyTables[4]);
    }

    [Fact]
    public void ActiveKey返回副本_不被调用方污染()
    {
        var k = GateShareAntiPlug.ActiveKey;
        k[0] = 0x00;
        Assert.Equal(0xA7, GateShareAntiPlug.ActiveKey[0]);
    }

    [Fact]
    public void RSA常量登记()
    {
        // 脚本抽取：IOCP1..6 的 aks1024 模数长度均为 256 hex，指数依次 E31A/A114/B318/CB14/D31F/010001
        Assert.Equal(256, GateShareAntiPlug.RsaPrivateModulus_1024.Length);
        Assert.Equal("010001", GateShareAntiPlug.RsaPrivateExponent);
        Assert.StartsWith("63073E95891EAD15", GateShareAntiPlug.RsaPrivateModulus_1024);
        Assert.EndsWith("31E35C8D6", GateShareAntiPlug.RsaPrivateModulus_1024);

        // aks128 公钥（RebuildProcessBlacklist 用，与 IOCP 变体无关）
        Assert.Equal("597A185BA5F22A014F50B453E647C0C5", GateShareAntiPlug.RsaPublicModulus_128);
        Assert.Equal("CF2C34C6204626E70E493F5B37930363", GateShareAntiPlug.RsaPublicExponent_128);
    }

    [Fact]
    public void IsKeyOK恒为真_原文缺陷S()
    {
        // 原文 :2063/:2327 `IsKeyOK := True;` 替换了被整段注释掉的 WinLicense 校验
        Assert.True(GateShareAntiPlug.IsKeyOK);
        Assert.Equal(20, GateShareAntiPlug.ANTI_PLUG_ADD_DATA_SIZE);   // 4+4+4+8，无 packed
        Assert.Equal("client_module.dat", GateShareAntiPlug.ClientModuleFileName);
    }

    [Fact]
    public void ParseAddData_小端逐字段()
    {
        var rand = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var buf = BuildAddData(0xAABBCCDD, 0x11223344, 0x55667788, rand);
        var a = GateShareAntiPlug.ParseAddData(buf);

        Assert.Equal(0xAABBCCDDu, a.RungateType);
        Assert.Equal(0x11223344u, a.DllCRC);
        Assert.Equal(0x55667788u, a.DllLen);
        Assert.Equal(rand, a.RandKey);
    }

    [Fact]
    public void ParseAddData_短缓冲不抛异常()
    {
        Assert.NotNull(GateShareAntiPlug.ParseAddData(null));
        Assert.Equal(0u, GateShareAntiPlug.ParseAddData(new byte[3]).DllCRC);
        Assert.Equal(0u, GateShareAntiPlug.ParseAddData(Array.Empty<byte>()).DllLen);
    }

    // ================= CheckClientAntiPlugDllChanged（原 :2019-2165） =================

    [Fact]
    public void Check_文件不存在返回False()
    {
        Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
        Assert.Equal(0, GateShareAntiPlug.DefaultCipherRsaCallCount);
    }

    [Fact]
    public void Check_文件过短返回False()
    {
        File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"), new byte[] { 1, 2, 3 });
        Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
        Assert.Equal(0, GateShareAntiPlug.DefaultCipherRsaCallCount);
    }

    [Fact]
    public void Check_FileFlag不匹配返回False且不调解密()
    {
        File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
            BuildModuleFile(0x12345678, new byte[20], Array.Empty<byte>()));
        Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
        Assert.Equal(0, GateShareAntiPlug.DefaultCipherRsaCallCount);
    }

    [Fact]
    public void Check_FileFlag正确但RSA未接线返回False()
    {
        // 默认接缝返回 null（LbRSA 未移植）→ OutSize 判定失败 → False（安全失败）
        int before = GateShareAntiPlug.DefaultCipherRsaCallCount;
        File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
            BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], Array.Empty<byte>()));

        Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
        Assert.Equal(before + 1, GateShareAntiPlug.DefaultCipherRsaCallCount);      // 确实走到了 RSA 步
        Assert.Equal((8, 20), GateShareAntiPlug.DefaultCipherLastArgs);             // offset = 4+4，len = EncLen
    }

    [Fact]
    public void Check_版本一致返回True()
    {
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, 0xDEADBEEF, 4, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugVersion = 0xDEADBEEF;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[24], Array.Empty<byte>()));

            Assert.True(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
            Assert.Equal(1, cipher.RsaCalls);
            Assert.Equal(8, cipher.LastOffset);
            Assert.Equal(24, cipher.LastEncLen);
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Check_版本不一致返回False()
    {
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, 0xDEADBEEF, 4, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugVersion = 0x00000001;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[24], Array.Empty<byte>()));
            Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Check_解密结果长度不对返回False_原文缺陷O()
    {
        // ★ 缺陷 O：原文判据是 `OutSize = SizeOf(AddData)`；这里模拟"只返回 16 字节"（aks128 的模数宽度）
        var cipher = new FakeCipher { RsaOut = new byte[16] };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugVersion = 0xDEADBEEF;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[16], Array.Empty<byte>()));
            Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
            Assert.Equal(1, cipher.RsaCalls);          // 接了但长度不匹配
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Check_EncLen为0或超出文件长度时不调解密()
    {
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, 1, 1, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;

            // EncLen = 0（原文 `EncLen > 0` 为假）
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, Array.Empty<byte>(), Array.Empty<byte>()));
            Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
            Assert.Equal(0, cipher.RsaCalls);

            // EncLen >= 文件长度（原文 `EncLen < MS.Size` 为假）：手工拼一个 EncLen = 100 的头
            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.Write(GateShareAntiPlug.ActiveFileFlag);
            w.Write(100);
            w.Write(new byte[4]);
            w.Flush();
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"), ms.ToArray());
            Assert.False(GateShareAntiPlug.CheckClientAntiPlugDllChanged());
            Assert.Equal(0, cipher.RsaCalls);
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    // ================= LoadClientAntiPlugDll（原 :2168-2519） =================

    [Fact]
    public void Load_文件不存在时清空全局并返回False()
    {
        GateShareGlobals.g_ClientAntiPlugDllString = "OLD";
        GateShareGlobals.g_ClientAntiPlugDllStringCRC = 123;
        GateShareGlobals.g_ClientAntiPlugDllSize = 456;

        Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());
        Assert.Equal("", GateShareGlobals.g_ClientAntiPlugDllString);
        Assert.Equal(0u, GateShareGlobals.g_ClientAntiPlugDllStringCRC);
        Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllSize);      // 原 :2387-2389
    }

    [Fact]
    public void Load_FileFlag不匹配返回False()
    {
        File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
            BuildModuleFile(0x00000000, new byte[20], Array.Empty<byte>()));
        Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());
    }

    [Fact]
    public void Load_文件过短返回False()
    {
        File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"), new byte[] { 1, 2 });
        Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());
    }

    [Fact]
    public void Load_RSA未接线返回False()
    {
        int before = GateShareAntiPlug.DefaultCipherRsaCallCount;
        File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
            BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], new byte[8]));
        Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());
        Assert.Equal(before + 1, GateShareAntiPlug.DefaultCipherRsaCallCount);
        Assert.Empty(GateShareAntiPlug.ClientAntiPlugDllBytes);
    }

    [Fact]
    public void Load_DllLen与剩余长度不符返回False()
    {
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, 0, 99, new byte[8]) };   // 声称 99 字节
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], new byte[10]));
            Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());   // 原 :2480
            Assert.Null(cipher.LastKey);                               // 没走到 DES
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Load_CRC不匹配返回False()
    {
        var plain = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, 0x11111111, 8, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], plain));
            Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());   // 原 :2496
            Assert.Equal(8, cipher.LastLen);
            Assert.Empty(GateShareAntiPlug.ClientAntiPlugDllBytes);
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Load_成功路径_校验全流程()
    {
        var randKey = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80 };
        var plain = new byte[32];
        for (int i = 0; i < plain.Length; i++) plain[i] = (byte)(i + 1);
        uint crc = GXX.Core.Crypto.CheckCrc.BufferCRC(plain, plain.Length);

        var cipher = new FakeCipher { RsaOut = BuildAddData(1, crc, (uint)plain.Length, randKey) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugDllBlockSize = 8;

            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], plain));

            Assert.True(GateShareAntiPlug.LoadClientAntiPlugDll());     // 整条链路走通

            // DES 收到的 key = RandKey XOR Key[0..7]（原 :2485-2488）
            var expected = new byte[8];
            var active = GateShareAntiPlug.ActiveKey;
            for (int i = 0; i < 8; i++) expected[i] = (byte)(randKey[i] ^ active[i]);
            Assert.Equal(expected, cipher.LastKey);
            Assert.Equal(plain.Length, cipher.LastLen);

            // 全局量（原 :2499-2505）。注意：`g_ClientAntiPlugDllString` 是 **zLib 压缩后**的负载，
            // 不是明文；`g_ClientAntiPlugDllSize` 才是明文长度（原文 `MSDll.Size`）。
            Assert.NotEmpty(GateShareAntiPlug.ClientAntiPlugDllBytes);
            Assert.Equal(GateShareAntiPlug.ClientAntiPlugDllBytes.Length,
                         GateShareGlobals.g_ClientAntiPlugDllString.Length);      // 逐字节保真
            Assert.Equal(crc, GateShareGlobals.g_ClientAntiPlugVersion);          // DllCRC
            Assert.Equal(plain.Length, GateShareGlobals.g_ClientAntiPlugDllSize); // MSDll.Size = 明文长度
            Assert.Equal(GXX.Core.Crypto.CheckCrc.BufferCRC(GateShareAntiPlug.ClientAntiPlugDllBytes,
                                                            GateShareAntiPlug.ClientAntiPlugDllBytes.Length),
                         GateShareGlobals.g_ClientAntiPlugDllStringCRC);
            Assert.Equal((GateShareAntiPlug.ClientAntiPlugDllBytes.Length + 8 - 1) / 8,
                         GateShareGlobals.g_ClientAntiPlugDllBlockCount);
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Load_BlockSize为0时除零_原文缺陷P()
    {
        // ★ 缺陷 P：`g_ClientAntiPlugDllBlockSize` 初值 0（:597）且本函数不校验 → EDivByZero
        var plain = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        uint crc = GXX.Core.Crypto.CheckCrc.BufferCRC(plain, plain.Length);
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, crc, 8, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugDllBlockSize = 0;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], plain));

            Assert.Throws<DivideByZeroException>(() => GateShareAntiPlug.LoadClientAntiPlugDll());
        }
        finally
        {
            GateShareAntiPlug.Cipher = original;
            GateShareGlobals.g_ClientAntiPlugDllBlockSize = 0;
        }
    }

    [Fact]
    public void Load_DllLen为0时CRC对空缓冲()
    {
        // 空明文的 CRC 由实现决定（原文 CheckCrc.pas 的初值是 $DBC66688 而非标准 $FFFFFFFF），
        // 所以这里先算出来再塞进 AddData，避免把 CRC 常量抄错。
        uint emptyCrc = GXX.Core.Crypto.CheckCrc.BufferCRC(Array.Empty<byte>(), 0);
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, emptyCrc, 0u, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugDllBlockSize = 4;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], Array.Empty<byte>()));

            Assert.True(GateShareAntiPlug.LoadClientAntiPlugDll());
            Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllSize);
            Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllBlockCount);
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Load_可注入zLib接缝()
    {
        var plain = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };
        uint crc = GXX.Core.Crypto.CheckCrc.BufferCRC(plain, plain.Length);
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, crc, 4, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        var originalZlib = GateShareAntiPlug.ZlibCompressBuffer;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareAntiPlug.ZlibCompressBuffer = (src, len) => new byte[] { 0x5A, 0xA5 };
            GateShareGlobals.g_ClientAntiPlugDllBlockSize = 1;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], plain));

            Assert.True(GateShareAntiPlug.LoadClientAntiPlugDll());
            Assert.Equal(new byte[] { 0x5A, 0xA5 }, GateShareAntiPlug.ClientAntiPlugDllBytes);
            Assert.Equal(2, GateShareGlobals.g_ClientAntiPlugDllBlockCount);   // (2 + 1 - 1) / 1
        }
        finally
        {
            GateShareAntiPlug.Cipher = original;
            GateShareAntiPlug.ZlibCompressBuffer = originalZlib;
        }
    }

    [Fact]
    public void Load_失败时保留清空后的状态()
    {
        // 先制造一次成功，再制造一次失败 → 失败路径在入口已清空，故结果为空
        var plain = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        uint crc = GXX.Core.Crypto.CheckCrc.BufferCRC(plain, plain.Length);
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, crc, 8, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            GateShareGlobals.g_ClientAntiPlugDllBlockSize = 8;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, new byte[20], plain));
            Assert.True(GateShareAntiPlug.LoadClientAntiPlugDll());
            Assert.NotEmpty(GateShareAntiPlug.ClientAntiPlugDllBytes);

            // 换成 FileFlag 不对的文件 → 入口已清空
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(0x11111111, new byte[20], plain));
            Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());
            Assert.Empty(GateShareAntiPlug.ClientAntiPlugDllBytes);
            Assert.Equal("", GateShareGlobals.g_ClientAntiPlugDllString);
            Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllSize);
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }

    [Fact]
    public void Load_EncLen越界时不调解密()
    {
        var cipher = new FakeCipher { RsaOut = BuildAddData(1, 0, 0, new byte[8]) };
        var original = GateShareAntiPlug.Cipher;
        try
        {
            GateShareAntiPlug.Cipher = cipher;
            File.WriteAllBytes(Path.Combine(_dir, "client_module.dat"),
                BuildModuleFile(GateShareAntiPlug.ActiveFileFlag, Array.Empty<byte>(), new byte[10]));
            Assert.False(GateShareAntiPlug.LoadClientAntiPlugDll());
            Assert.Equal(0, cipher.RsaCalls);       // EncLen = 0 → 不调
        }
        finally { GateShareAntiPlug.Cipher = original; }
    }
}

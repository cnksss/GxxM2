using System.Text;
using GXX.Core.Crypto;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.Core.Launcher;

/// <summary>
/// 登录器 → 客户端 的启动参数（Delphi <c>TClientParam</c>）。
///
/// <para>依据 <c>Source\Client-HGE\Client.dpr:570-584</c>（发布分支 <c>{$IF TESTMODE = 0}</c>）：</para>
/// <code>
///   Client.exe &lt;arg1&gt; &lt;arg2&gt;          // 标准模式
///   Client.exe "0" &lt;arg1&gt; &lt;arg2&gt;       // 三方登录器模式（ParamStr(1)='0'）
///
///   arg1 = EncryString_LF(IntToStr(pKey))                          // 会话随机码
///   arg2 = zEncryBufferK(TClientParam 字节, SizeOf, GetKeyValue(pKey))   // 设置参数
/// </code>
/// <para>客户端侧解密（<c>Client.dpr:575-584</c>）：</para>
/// <code>
///   g_PKey^ := StrToIntDef(DecryString_LF(ParamStr(2)), 0);
///   zDecryBufferK(ParamStr(3), @g_ClientParam, SizeOf(TClientParam), EncryptUnit.GetKeyValue(g_PKey^));
/// </code>
///
/// <para>⚠ <b>关于 <see cref="SizeOf"/>：</b>
/// 记录中含 <c>sDomainName: array[0..999] of Char</c>。Delphi 7 / 非 Unicode 下 <c>Char</c> = 1 字节
/// （SizeOf = <b>2333</b>）；Delphi 2009+ Unicode 下 <c>Char</c> = 2 字节（SizeOf = <b>3333</b>）。
/// 本仓库其它实测证据（枚举 1 字节、ShortString 为 AnsiChar 语义）指向非 Unicode，
/// 但**尚无线上 <c>Client.exe</c> 可做二进制验证**，因此 <see cref="CharWidth"/> 保留为显式参数。
/// 若客户端收不到参数，把它改成 2 再试即可（差异仅在 <c>sDomainName</c> 一处）。</para>
/// </summary>
public sealed class LauncherParam
{
    /// <summary>1 = 非 Unicode（Delphi 7，SizeOf=2333）；2 = Unicode（Delphi 2009+，SizeOf=3333）。</summary>
    public int CharWidth = 1;

    public uint Handle;
    public string GameLoginFileName = "";     // string[255] 登录器文件名
    public string ServerCaption = "";         // string[100] 服务器标题
    public string ServerAddr = "";            // string[100] 网关地址
    public int ServerPort;                    // Integer
    public string UpdateAddr = "";            // string[100] 微端更新地址
    public int UpdatePort;                    // Integer
    public string UpdatePassWord = "";        // string[100] 微端更新密码
    public uint ClientCrc;                    // Cardinal
    public uint LoginCrc;                     // Cardinal
    public string DomainName = "";            // array[0..999] of Char（绑定域名）
    public string HomePage = "";              // string[255]
    public byte MaxClientCount = 2;           // Byte 多开上限
    public ushort ScreenWidth = 800;          // Word
    public ushort ScreenHeight = 600;         // Word
    public byte BitCount = 32;                // Byte 16/32
    public bool WindowMode = true;            // Boolean
    public bool VSync = true;                 // Boolean
    public bool Hardware = true;              // Boolean
    public byte ClientVersion;                // TClientVersion 枚举（1 字节）
    public string SemaphoreName = "";         // string[40]
    public string MachineID = "";             // string[32] 硬件ID
    public string ClientDataFile = "";        // string[255] 内核附加数据（ClientData.dat 路径）
    public byte[] ConfigUrlMD5 = new byte[16];// MD5Digest 服务器列表 URL 的 MD5
    public string PromotionFlag = "";         // string[40] 推广ID

    /// <summary>SizeOf(TClientParam)。</summary>
    public int SizeOf => 4 + 256 + 101 + 101 + 4 + 101 + 4 + 101 + 4 + 4
                       + (1000 * CharWidth) + 256 + 1 + 2 + 2 + 1 + 1 + 1 + 1 + 1
                       + 41 + 33 + 256 + 16 + 41;

    /// <summary>按声明顺序序列化为 packed record 字节（测试模式分支 TClientParam）。</summary>
    public byte[] Serialize()
    {
        var b = new byte[SizeOf];
        int p = 0;
        void I32(int v) { BitConverter.TryWriteBytes(b.AsSpan(p, 4), v); p += 4; }
        void U32(uint v) { BitConverter.TryWriteBytes(b.AsSpan(p, 4), v); p += 4; }
        void U16(ushort v) { BitConverter.TryWriteBytes(b.AsSpan(p, 2), v); p += 2; }
        void B8(byte v) { b[p++] = v; }
        void BB(bool v) { b[p++] = v ? (byte)1 : (byte)0; }
        void S(string s, int cap) { ShortStr.Set(b, p, cap, s); p += cap + 1; }

        U32(Handle);
        S(GameLoginFileName, 255);
        S(ServerCaption, 100);
        S(ServerAddr, 100);
        I32(ServerPort);
        S(UpdateAddr, 100);
        I32(UpdatePort);
        S(UpdatePassWord, 100);
        U32(ClientCrc);
        U32(LoginCrc);
        // array[0..999] of Char：非 Unicode 下等价于 1000 字节的定长 AnsiChar 缓冲
        {
            int cap = 1000 * CharWidth;
            var raw = EncodingInit.GBK.GetBytes(DomainName ?? "");
            if (raw.Length > cap) raw = raw.AsSpan(0, cap).ToArray();
            Array.Copy(raw, 0, b, p, raw.Length);
            p += cap;
        }
        S(HomePage, 255);
        B8(MaxClientCount);
        U16(ScreenWidth);
        U16(ScreenHeight);
        B8(BitCount);
        BB(WindowMode);
        BB(VSync);
        BB(Hardware);
        B8(ClientVersion);
        S(SemaphoreName, 40);
        S(MachineID, 32);
        S(ClientDataFile, 255);
        Array.Copy(ConfigUrlMD5 ?? new byte[16], 0, b, p, 16); p += 16;
        S(PromotionFlag, 40);

        if (p != b.Length)
            throw new InvalidOperationException($"序列化长度 {p} ≠ SizeOf {b.Length}（内部错误）");
        return b;
    }

    /// <summary>从字节反序列化（用于自检）。</summary>
    public static LauncherParam Deserialize(byte[] b, int charWidth = 1)
    {
        var r = new LauncherParam { CharWidth = charWidth };
        int p = 0;
        int I32() { var v = BitConverter.ToInt32(b, p); p += 4; return v; }
        uint U32() { var v = BitConverter.ToUInt32(b, p); p += 4; return v; }
        ushort U16() { var v = BitConverter.ToUInt16(b, p); p += 2; return v; }
        byte B8() => b[p++];
        bool BB() => b[p++] != 0;
        string S(int cap) { var v = ShortStr.Get(b, p, cap); p += cap + 1; return v; }

        r.Handle = U32();
        r.GameLoginFileName = S(255);
        r.ServerCaption = S(100);
        r.ServerAddr = S(100);
        r.ServerPort = I32();
        r.UpdateAddr = S(100);
        r.UpdatePort = I32();
        r.UpdatePassWord = S(100);
        r.ClientCrc = U32();
        r.LoginCrc = U32();
        {
            int cap = 1000 * charWidth;
            r.DomainName = EncodingInit.GBK.GetString(b, p, cap).TrimEnd('\0');
            p += cap;
        }
        r.HomePage = S(255);
        r.MaxClientCount = B8();
        r.ScreenWidth = U16();
        r.ScreenHeight = U16();
        r.BitCount = B8();
        r.WindowMode = BB();
        r.VSync = BB();
        r.Hardware = BB();
        r.ClientVersion = B8();
        r.SemaphoreName = S(40);
        r.MachineID = S(32);
        r.ClientDataFile = S(255);
        r.ConfigUrlMD5 = b.AsSpan(p, 16).ToArray(); p += 16;
        r.PromotionFlag = S(40);
        return r;
    }

    /// <summary>
    /// 生成启动命令行参数。
    /// </summary>
    /// <param name="pKey">会话随机码（一般用 <see cref="EncryptUnit.GetKeyValue()"/> 生成）</param>
    /// <param name="thirdPartyMode">必须为 <c>true</c>。三方登录器模式（<c>Client.exe "0" arg1 arg2</c>）
    /// 只需 <c>EncryptUnit</c> 的 z 系列；标准模式要用 <c>EncryptDes_New</c>，
    /// 而该单元未包含在本仓库中 —— 客户端作者正是为此才留出三方模式
    /// （<c>Client.dpr:574</c>「用于三方登录器，因为不想提供 EncryptDes_New 的代码」）。</param>
    public (string Arg1, string Arg2, int PKey, string KeyString) Encode(int pKey, bool thirdPartyMode = true)
    {
        if (!thirdPartyMode)
            throw new NotSupportedException(
                "标准模式需要 Common\\DesUtils.pas 的 EncryptDes_New，本仓库未包含该实现。" +
                "请使用三方登录器模式（Client.exe \"0\" arg1 arg2）—— 这也是客户端官方预留的接入方式。");

        string keyStr = EncryptUnit.GetKeyValue(pKey);
        byte[] blob = Serialize();
        string arg2 = LauncherCrypto.ZEncryBufferK(blob, blob.Length, keyStr);
        string arg1 = LauncherCrypto.EncryStringLF(DelphiRTL.IntToStr(pKey));
        return (arg1, arg2, pKey, keyStr);
    }

    /// <summary>解析启动参数（镜像客户端逻辑，用于自检）。</summary>
    public static LauncherParam Decode(string arg1, string arg2, int charWidth = 1)
    {
        int pKey = DelphiRTL.StrToIntDef(LauncherCrypto.DecryStringLF(arg1), 0);
        string keyStr = EncryptUnit.GetKeyValue(pKey);
        byte[] plain = LauncherCrypto.ZDecryBufferK(arg2, keyStr);
        int size = new LauncherParam { CharWidth = charWidth }.SizeOf;
        var buf = new byte[size];
        Array.Copy(plain, buf, Math.Min(plain.Length, size));
        return Deserialize(buf, charWidth);
    }

    /// <summary>组装完整命令行（不含 exe 路径）。</summary>
    public static string BuildCommandLine(string arg1, string arg2, bool thirdPartyMode)
        => thirdPartyMode ? $"0 {arg1} {arg2}" : $"{arg1} {arg2}";
}

/// <summary>
/// <c>Common\EncryptUnit_LF.pas</c> 中的 LF 系列（密钥 = <c>IntToStr(NewEncryKey^) = "3230393649"</c>）。
/// GXX.Core 现有 <see cref="EncryptUnit"/> 对应的是默认 <c>EncryKey = 20120101</c>，
/// 而登录器 → 客户端 通道用的是 LF 系列，故在此补齐。
/// </summary>
public static class LauncherCrypto
{
    /// <summary><c>IntToStr(NewEncryKey^)</c>，<c>NewEncryKey^ = $0C08BE531</c>（UnitDes.pas:1109）。</summary>
    public const string NewEncryKeyString = ClientDataFile.DesKey;

    /// <summary><c>EncryString_LF</c>：DES(LF密钥) → Base64 → 6bit EDcode。</summary>
    public static string EncryStringLF(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        byte[] raw = EncodingInit.GBK.GetBytes(s);
        byte[] enc = UnitDes.EncryptStrDesBytes(raw, NewEncryKeyString);
        return EncodingInit.GBK.GetString(EDcode.EncodeString(Base64Util.Base64EncodeStr(enc)));
    }

    /// <summary><c>DecryString_LF</c>：EDcode → Base64 → DES(LF密钥)。</summary>
    public static string DecryStringLF(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        byte[] decoded = EDcode.DecodeString(EncodingInit.GBK.GetBytes(s));
        byte[] b64 = Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded));
        byte[] plain = UnitDes.DecryptStrDes(b64, NewEncryKeyString);
        return EncodingInit.GBK.GetString(plain);
    }

    /// <summary><c>DecryString_LF2</c>（标准模式）—— 需要 <c>DesUtils.pas</c> 的 <c>DecryptDes_New</c>，本仓库未包含。</summary>
    public static string DecryStringLF2(string s)
        => throw new NotSupportedException("标准模式需要 Common\\DesUtils.pas 的 DecryptDes_New，本仓库未包含该实现。");

    /// <summary><c>EncryString_LF2</c>（标准模式）—— 需要 <c>DesUtils.pas</c> 的 <c>EncryptDes_New</c>，本仓库未包含。</summary>
    public static string EncryStringLF2(string s)
        => throw new NotSupportedException("标准模式需要 Common\\DesUtils.pas 的 EncryptDes_New，本仓库未包含该实现。");

    // ================================================================
    // 字节安全的 z 系列（zEncryBufferK / zDecryBufferK）
    //
    // ⚠ 为什么不能用 EncryptUnit.zEncryBufferK：
    //   EDcode(6bit) 的输出是**二进制**，可含非 ASCII 字节。
    //   现有实现把它经 GBK 做 string 往返 —— 非法 GBK 序列会被替换，**有损**，
    //   导致参数传到客户端后解不出来（小样本 ASCII 负载下看不出来）。
    //   Delphi 的 AnsiString 此处只是字节容器，正确映射是 Latin-1（逐字节保真）。
    // ================================================================

    static byte[] ToBytes(string raw) => Encoding.Latin1.GetBytes(raw ?? "");
    static string ToRawString(byte[] bytes) => Encoding.Latin1.GetString(bytes);

    /// <summary><c>zEncryBufferK</c> 的字节安全实现：zlib → DES → Base64 → EDcode。</summary>
    public static string ZEncryBufferK(byte[] buf, int bufsize, string key)
    {
        if (bufsize <= 0) return "";
        byte[] compressed = Compress.ZlibEx.CompressBuf(buf, bufsize);
        if (compressed == null || compressed.Length == 0) return "";
        byte[] cipher = new byte[compressed.Length];
        UnitDes.EncryptDes(compressed, cipher, compressed.Length, key);
        string b64 = Base64Util.Base64EncodeStr(cipher);
        return ToRawString(EDcode.EncodeString(b64));
    }

    /// <summary><c>zDecryBufferK</c> 的字节安全实现。</summary>
    public static byte[] ZDecryBufferK(string src, string key)
    {
        if (string.IsNullOrEmpty(src)) return Array.Empty<byte>();
        byte[] decoded = EDcode.DecodeString(ToBytes(src));
        byte[] cipher = Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded));
        byte[] plain = new byte[cipher.Length];
        UnitDes.DecryptDes(cipher, plain, cipher.Length, key);
        byte[] raw = Compress.ZlibEx.DecompressBuf(plain, plain.Length);
        return raw ?? Array.Empty<byte>();
    }
}

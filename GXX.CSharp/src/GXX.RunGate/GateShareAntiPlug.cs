// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（Delphi 7，GBK）—— **客户端反外挂模块加载 / 变更检测（暗桩侧）**
//   实测 LF = 3595 行。本文件覆盖：
//     :341-348   `{$IF CLIENT_ANTIPLUG = 1}` 的 `TAntiPlugAddData`（**未声明 packed**）
//     :535-623   `{$IF CLIENT_ANTIPLUG = 1}` 的插件类型/全局量（局部需要的部分见 GateShareGlobals.cs）
//     :2016-2165 `CheckClientAntiPlugDllChanged`
//     :2167-2519 `LoadClientAntiPlugDll`
//
// 生效组合：`CLIENT_ANTIPLUG = 1`（Grobal2_Ex.pas:10）→ 这两个函数**是活代码**；
//   `RungateLEG_IOCP = 6`（Grobal2_Ex.pas:32）→ FileFlag = `$0533BF07`、Key[0..7] = `A7 45 32 BB 3D 6A 7F 90`、
//   aks1024 私钥 = 下面的 `RsaPrivateModulus_1024`。其余 5 组是死分支（本文件只登记常量，不转录 5 份 256 位私钥；
//   若切换开关，用文件末尾的抽取脚本重新生成）。
//
// ── 为什么这里必须是**接缝** ────────────────────────────────────────────────────────
//   原文这两步依赖 `LbRSA.pas` 的 `TLbRSA`（`PublicKey/PrivateKey.ModulusAsString` + `EncryptBuffer/DecryptBuffer`）
//   与 `VMProtect` 的 `$I VMProtectBeginMutation.inc` / `VMProtectEnd.inc` 包裹。
//   **`LbRSA.pas` 在本仓库整树不存在**（`Get-ChildItem -Recurse -Filter LbRSA.pas` 零命中），
//   `VMProtect*.inc` 也不存在 → 无法 1:1 移植，只能把"RSA 解密"与"DES 解密"两步做成可注入接缝。
//   默认实现 `NotWiredClientAntiPlugCipher.RsaDecryptBuffer` 返回 **null**（= OutSize 不匹配 → 返回 False），
//   即"未接线时安全失败"，与原文在缺少私钥/模块时的失败方向一致。**不伪造加密结果。**
//
// ── 原文缺陷 / 易错点登记（照抄语义 + 差异断言）─────────────────────────────────────
//   O. [:2476 vs :342-348] `if OutSize = SizeOf(AddData) then`：`TAntiPlugAddData` 是
//      `RungateType/DllCRC/DllLen: LongWord` + `RandKey: array[0..7] of Byte` → 无 packed、无 `{$A}` 指令
//      （已 grep 全树：GateShare.pas 与 iocp.inc 里没有 `{$A}`/`{$PACKRECORDS}`）→ 默认对齐下 `SizeOf = 20`；
//      而同一函数的 `RSA.KeySize := aks128` 配的模数是 **32 hex 字符 = 16 字节 = 128 位**（:1940），
//      `aks1024` 配的模数是 256 hex = 128 字节（:2105-2145）。若 `DecryptBuffer` 返回"与模数同宽的字节数"，
//      则 `OutSize` 为 16(ak128)/128(ak1024)，**永远不等于 20** → 两个函数恒返回 False。
//      ★ 该结论依赖 LbRSA 的语义，本环境无法实跑 Delphi 复核 → 标 **UNVERIFIED**。
//      托管侧按 `ANTI_PLUG_ADD_DATA_SIZE = 20` 实现（忠实于"记录尺寸 20"这一侧），
//      并用注入的接缝返回 20 字节来驱动后续流程（这样 DllLen 校验/XOR/DES/CRC/zLib 全部可真跑）。
//   P. [:2502] `g_ClientAntiPlugDllBlockCount := (Length(...) + g_ClientAntiPlugDllBlockSize - 1) div g_ClientAntiPlugDllBlockSize;`
//      —— `g_ClientAntiPlugDllBlockSize` 初值 **0**（:597）且本函数**不校验** → 未配置时是 `EDivByZero`。
//      托管侧保留除零（`DivideByZeroException`），对应 Delphi 的 `EDivByZero`。
//   Q. [:2400 / :2422] 用 `MS.Read(...)` 顺序读文件头，而 `P := MS.Memory`（:2398）在 `Read` **之前**取得，
//      随后又在 :2473 用 `PChar(Integer(MS.Memory) + 8)` **重新**取指针 —— 两处 `MS.Memory` 是同一个基址，
//      但前者（:2398/:2073 的 `P`）**从未被使用**（死赋值）。
//   R. [:2065 / :2385] `Result := False;` 在 `$I VMProtectBeginMutation.inc` **之后**、
//      `if FileExists(...) and IsKeyOK then` **之前** —— 所有 `Exit` 都保留 False。
//   S. [:2063 / :2327] `IsKeyOK := True;` 替换了整段 WinLicense 注册校验（原文 `{$IF NEED_REGISTER = 1}`
//      的 WL* 代码被 `//` 整段注释掉）→ 在当前源码里 `IsKeyOK` **恒为 True**，是常量条件。
//   T. [:2030 / :2179] `AddData: TAntiPlugAddData;` 与 `OutBuf: array[0..10240-1] of Byte;`
//      —— `OutBuf` 是**栈上 10KB** 缓冲；`Move(OutBuf, AddData, SizeOf(AddData))` 只取前 20 字节。
// =====================================================================================

using System;
using System.IO;
using GXX.Core.Crypto;
using GXX.Core.Protocol;

namespace GXX.RunGate;

/// <summary>原文 :341-348 `{$IF CLIENT_ANTIPLUG = 1} TAntiPlugAddData = record
/// RungateType, DllCRC, DllLen: LongWord; RandKey: array[0..7] of Byte; end; {$IFEND}`
/// （**未声明 packed**；托管侧尺寸常量见 <see cref="GateShareAntiPlug.ANTI_PLUG_ADD_DATA_SIZE"/>）。</summary>
public class TAntiPlugAddData
{
    public uint RungateType;
    public uint DllCRC;
    public uint DllLen;
    public byte[] RandKey = new byte[8];
}

/// <summary>`CheckClientAntiPlugDllChanged` / `LoadClientAntiPlugDll` 的 RSA + DES 两步接缝。</summary>
public interface IClientAntiPlugCipher
{
    /// <summary>原文 `RSA.DecryptBuffer(P^, EncLen, OutBuf)`（LbRSA；本仓库缺失）。
    /// 返回 **null** 表示"未接线/失败" → 调用方按 `OutSize` 不匹配处理（返回 False）。</summary>
    byte[] RsaDecryptBuffer(byte[] src, int offset, int len);

    /// <summary>原文 `DecryptDes(P^, P^, AddData.DllLen, sKey)` —— **就地**解密；
    /// `key` 是 `RandKey XOR Key[0..7]` 得到的 **8 个原始字节**（不是十六进制串）。</summary>
    void DecryptDesInPlace(byte[] data, int len, byte[] key);
}

/// <summary>GateShare.pas 的客户端反外挂模块加载（1:1，RSA/DES 两步走接缝）。</summary>
public static class GateShareAntiPlug
{
    /// <summary>`client_module.dat` 的文件名（原文两处都是 `ExtractFilePath(ParamStr(0)) + 'client_module.dat'`，:2067/:2391）。</summary>
    public const string ClientModuleFileName = "client_module.dat";

    /// <summary>`True` = 原文 :2063/:2327 的 `IsKeyOK := True;`（缺陷 S：WinLicense 校验被整段注释掉，恒为真）。</summary>
    public const bool IsKeyOK = true;

    /// <summary>推断的 `SizeOf(TAntiPlugAddData)` = 4 + 4 + 4 + 8（无 packed、无 `{$A}` 指令）→ 20。见缺陷 O。</summary>
    public const int ANTI_PLUG_ADD_DATA_SIZE = 20;

    /// <summary>原文 :2078-2093 / :2403-2418 的 `FileFlag` 表（两组完全相同，脚本回读校验）。
    /// 当前生效的是索引 6（`RungateLEG_IOCP = 6`）。</summary>
    public static readonly uint[] FileFlags =
    {
        0x5E462548,   // RungateLEG_IOCP = 1   原 :2078 / :2403
        0x4FB74474,   //                      2  原 :2081 / :2406
        0xA1DFE8F0,   //                      3  原 :2084 / :2409
        0x22CE2669,   //                      4  原 :2087 / :2412
        0x9EA5AC55,   //                      5  原 :2090 / :2415
        0x0533BF07,   //                      6  原 :2093 / :2418  ★ 生效
    };

    /// <summary>当前生效的 IOCP 变体号（`RungateLEG_IOCP`，Grobal2_Ex.pas:32 = 6）。</summary>
    public const int RungateLEG_IOCP = 6;

    /// <summary>当前生效的 `FileFlag`。</summary>
    public static uint ActiveFileFlag => FileFlags[RungateLEG_IOCP - 1];

    /// <summary>原文 :2329-2381 的 `Key[0..7]` 表（6 组；当前生效第 6 组 = `A7 45 32 BB 3D 6A 7F 90`）。</summary>
    public static readonly byte[][] KeyTables =
    {
        new byte[] { 0x3D, 0xEF, 0x8E, 0x6F, 0x51, 0x4F, 0x1D, 0x9F },   // IOCP 1  原 :2329-2336
        new byte[] { 0xAC, 0x28, 0xD0, 0xD7, 0x81, 0x35, 0x4C, 0xC7 },   // IOCP 2  原 :2338-2345
        new byte[] { 0x7C, 0x17, 0xCD, 0xBA, 0x2D, 0x9E, 0x43, 0xCF },   // IOCP 3  原 :2347-2354
        new byte[] { 0xE5, 0x23, 0x90, 0xCF, 0xE2, 0x4E, 0xAF, 0x8C },   // IOCP 4  原 :2356-2363
        new byte[] { 0xE7, 0x67, 0x37, 0x30, 0x33, 0x50, 0x4A, 0xA4 },   // IOCP 5  原 :2365-2372
        new byte[] { 0xA7, 0x45, 0x32, 0xBB, 0x3D, 0x6A, 0x7F, 0x90 },   // IOCP 6  原 :2374-2381  ★ 生效
    };

    /// <summary>当前生效的 `Key[0..7]`。</summary>
    public static byte[] ActiveKey => (byte[])KeyTables[RungateLEG_IOCP - 1].Clone();

    /// <summary>原文 :2140-2144 `RSA.PrivateKey.ModulusAsString`（aks1024，IOCP 6；256 hex 脚本抽取 + 回读）。
    /// 其余 5 组是死分支，未转录（抽取脚本见文件末尾注释）。</summary>
    public const string RsaPrivateModulus_1024 =
        "63073E95891EAD15EA768ED83F87D27ABAB0EF7CA5BD48C6ABA32A311841FE465161B" +
        "A489F3A6443DF0F757B5D8E74C84385253D66BE2436518CC85E90BF58F4D5F9A1E1CF" +
        "AE80A82DBE7A7ED1FE4BAED0FFEEC9DE73D17C99855045ED1B79B25AA65CCE17D765E" +
        "7DBDE3599DF71474B9E2278191B7CE5FE71F8E8B31E35C8D6";

    /// <summary>原文 :2145 `RSA.PrivateKey.ExponentAsString := '010001';`（IOCP 6）。</summary>
    public const string RsaPrivateExponent = "010001";

    /// <summary>`RebuildProcessBlacklist` 用的 aks128 公钥（原文 :1940-1941，与 IOCP 变体无关）。</summary>
    public const string RsaPublicModulus_128 = "597A185BA5F22A014F50B453E647C0C5";
    public const string RsaPublicExponent_128 = "CF2C34C6204626E70E493F5B37930363";

    private sealed class NotWiredClientAntiPlugCipher : IClientAntiPlugCipher
    {
        public int RsaDecryptCallCount;
        public int LastOffset;
        public int LastLen;

        /// <summary>未接线：返回 null → `OutSize` 判定失败 → 两个函数都返回 False（安全失败）。</summary>
        public byte[] RsaDecryptBuffer(byte[] src, int offset, int len)
        {
            RsaDecryptCallCount++;
            LastOffset = offset;
            LastLen = len;
            return null;
        }

        /// <summary>默认 DES 实现走 `GXX.Core.Crypto.UnitDes.DecryptDes`。
        /// ★ 已知差异：`UnitDes.DecryptDes(…, key: string)` 内部按 **GBK** 把 key 编码成字节
        ///   （`UnitDes.GetKeyData` → `EncodingInit.GBK.GetBytes(key)`），而原文的 `sKey` 是
        ///   `Move(Key[0], sKey[1], 8)` 得到的 **8 个原始字节**。对 0x81-0xFE 的 key 字节，GBK 往返不等价。
        ///   这里用 Latin-1（逐字节保真）字符串传入；**生产接线时必须改成给 `GXX.Core` 加一个
        ///   `byte[] key` 重载**（属其它分区，本车道无权改，已登记为集成事项）。</summary>
        public void DecryptDesInPlace(byte[] data, int len, byte[] key)
        {
            var chars = new char[8];
            for (int i = 0; i < 8; i++) chars[i] = (char)key[i];
            UnitDes.DecryptDes(data, data, len, new string(chars));
        }
    }

    private static readonly NotWiredClientAntiPlugCipher DefaultCipher = new NotWiredClientAntiPlugCipher();

    /// <summary>可注入的 RSA/DES 接缝（默认未接线实现；生产接线由集成者替换）。</summary>
    public static IClientAntiPlugCipher Cipher = DefaultCipher;

    /// <summary>默认实现的 RSA 调用次数（测试探针）。</summary>
    public static int DefaultCipherRsaCallCount => DefaultCipher.RsaDecryptCallCount;

    /// <summary>默认实现最后一次收到的 `(offset, len)`（测试探针）。</summary>
    public static (int Offset, int Len) DefaultCipherLastArgs => (DefaultCipher.LastOffset, DefaultCipher.LastLen);

    /// <summary>zLib 压缩接缝（默认走 `EDcode.zLibCompressBuffer`）。</summary>
    public static Func<byte[], int, byte[]> ZlibCompressBuffer = (src, len) => EDcode.zLibCompressBuffer(src, len);

    /// <summary>`LoadClientAntiPlugDll` 产出的压缩负载（托管侧权威表示；对应原文的二进制 `g_ClientAntiPlugDllString`）。</summary>
    public static byte[] ClientAntiPlugDllBytes { get; private set; } = Array.Empty<byte>();

    /// <summary>原文 :2019-2165 `function CheckClientAntiPlugDllChanged: Boolean;`
    /// 只做"版本比对"：读 `client_module.dat` → FileFlag 校验 → RSA 解密 20 字节 AddData →
    /// `Result := g_ClientAntiPlugVersion = AddData.DllCRC`。</summary>
    public static bool CheckClientAntiPlugDllChanged()
    {
        // 原 :2064-2065 `{$I VMProtectBeginMutation.inc}` / `Result := False;`
        bool Result = false;

        string FileName = GateSharePaths.ExeDir + ClientModuleFileName;    // 原 :2067
        if (!File.Exists(FileName) || !IsKeyOK) return Result;             // 原 :2068（IsKeyOK 恒真，缺陷 S）

        byte[] fileBytes = File.ReadAllBytes(FileName);                    // 原 :2070-2072 `MS.LoadFromFile`
        if (fileBytes.Length < 8) return Result;                           // 托管侧护栏（原文靠 TMemoryStream 的读越界静默返回 0）

        // 原 :2075 `MS.Read(FileFlag, SizeOf(FileFlag));`
        uint FileFlag = BitConverter.ToUInt32(fileBytes, 0);
        if (FileFlag != ActiveFileFlag) return Result;                     // 原 :2077-2095

        // 原 :2097 `MS.Read(EncLen, SizeOf(EncLen));`
        int EncLen = BitConverter.ToInt32(fileBytes, 4);
        // 原 :2098 `if (EncLen > 0) and (EncLen < MS.Size) then`
        if ((EncLen > 0) && (EncLen < fileBytes.Length))
        {
            // 原 :2100-2146 `RSA.KeySize := aks1024; ModulusAsString := …; ExponentAsString := '010001';`
            // 原 :2148 `P := PChar(Integer(MS.Memory) + SizeOf(FileFlag) + SizeOf(EncLen));`
            // 原 :2149 `OutSize := RSA.DecryptBuffer(P^, EncLen, OutBuf);`
            byte[] OutBuf = Cipher.RsaDecryptBuffer(fileBytes, 4 + 4, EncLen);

            // 原 :2151 `if OutSize = SizeOf(AddData) then`
            if (OutBuf != null && OutBuf.Length == ANTI_PLUG_ADD_DATA_SIZE)
            {
                var AddData = ParseAddData(OutBuf);                        // 原 :2153 `Move(OutBuf, AddData, SizeOf(AddData));`
                // 原 :2154 `Result := g_ClientAntiPlugVersion = AddData.DllCRC;`
                Result = GateShareGlobals.g_ClientAntiPlugVersion == AddData.DllCRC;
            }
        }
        return Result;
        // 原 :2164 `{$I VMProtectEnd.inc}`
    }

    /// <summary>原文 :2168-2519 `function LoadClientAntiPlugDll: Boolean;`
    /// 完整流程：清掉旧的 dll 串/CRC/尺寸 → FileFlag 校验 → RSA(aks128) 解密 AddData →
    /// `DllLen` 与文件剩余长度比对 → 取 dll 密文 → `RandKey XOR Key[0..7]` →
    /// DES 就地解密 → `BufferCRC` 校验 → zLib 压缩 + CRC + 分块数（**可能除零**，缺陷 P）。</summary>
    public static bool LoadClientAntiPlugDll()
    {
        byte[] Key = ActiveKey;                                            // 原 :2328-2382

        // 原 :2385-2389
        bool Result = false;
        GateShareGlobals.g_ClientAntiPlugDllString = "";
        GateShareGlobals.g_ClientAntiPlugDllStringCRC = 0;
        GateShareGlobals.g_ClientAntiPlugDllSize = 0;
        ClientAntiPlugDllBytes = Array.Empty<byte>();

        string FileName = GateSharePaths.ExeDir + ClientModuleFileName;     // 原 :2391
        if (!File.Exists(FileName) || !IsKeyOK) return Result;              // 原 :2392

        byte[] MS = File.ReadAllBytes(FileName);                            // 原 :2394-2397
        if (MS.Length < 8) return Result;                                   // 托管侧护栏

        // 原 :2400 `MS.Read(FileFlag, SizeOf(FileFlag));`
        uint FileFlag = BitConverter.ToUInt32(MS, 0);
        if (FileFlag != ActiveFileFlag) return Result;                      // 原 :2402-2420

        // 原 :2422 `MS.Read(EncLen, SizeOf(EncLen));`
        int EncLen = BitConverter.ToInt32(MS, 4);
        if ((EncLen > 0) && (EncLen < MS.Length))                           // 原 :2423
        {
            // 原 :2425-2471 `RSA.KeySize := aks128; PublicKey.Modulus/Exponent := …`
            // 原 :2473-2474
            byte[] OutBuf = Cipher.RsaDecryptBuffer(MS, 4 + 4, EncLen);

            if (OutBuf != null && OutBuf.Length == ANTI_PLUG_ADD_DATA_SIZE)  // 原 :2476
            {
                var AddData = ParseAddData(OutBuf);                         // 原 :2478

                // 原 :2480 `if MS.Size - SizeOf(FileFlag) - SizeOf(EncLen) - EncLen = AddData.DllLen then`
                int remain = MS.Length - 4 - 4 - EncLen;
                if (remain == unchecked((int)AddData.DllLen))
                {
                    // 原 :2482-2483 `MS.Position := 8 + EncLen; MSDll.CopyFrom(MS, remain);`
                    var MSDll = new byte[remain];
                    Array.Copy(MS, 4 + 4 + EncLen, MSDll, 0, remain);

                    // 原 :2485-2488 `for I := 0 to Length(AddData.RandKey) - 1 do Key[I] := AddData.RandKey[I] xor Key[I];`
                    for (int I = 0; I < AddData.RandKey.Length; I++)
                        Key[I] = (byte)(AddData.RandKey[I] ^ Key[I]);

                    // 原 :2490-2491 `SetLength(sKey, Length(Key)); Move(Key[0], sKey[1], Length(Key));`
                    // 原 :2493-2494 `P := PChar(MSDll.Memory); DecryptDes(P^, P^, AddData.DllLen, sKey);`
                    Cipher.DecryptDesInPlace(MSDll, unchecked((int)AddData.DllLen), Key);

                    // 原 :2496 `Result := AddData.DllCRC = BufferCRC(P, AddData.DllLen);`
                    Result = AddData.DllCRC == CheckCrc.BufferCRC(MSDll, unchecked((int)AddData.DllLen));

                    if (Result)                                         // 原 :2497
                    {
                        // 原 :2499 `g_ClientAntiPlugDllString := zLibCompressBuffer(MSDll.Memory, MSDll.Size);`
                        ClientAntiPlugDllBytes = ZlibCompressBuffer(MSDll, MSDll.Length);
                        GateShareGlobals.g_ClientAntiPlugDllString = BytesToBinaryString(ClientAntiPlugDllBytes);

                        // 原 :2500 `g_ClientAntiPlugDllStringCRC := BufferCrc(PChar(g_ClientAntiPlugDllString), Length(...));`
                        GateShareGlobals.g_ClientAntiPlugDllStringCRC =
                            CheckCrc.BufferCRC(ClientAntiPlugDllBytes, ClientAntiPlugDllBytes.Length);

                        // 原 :2502（★ 缺陷 P：`g_ClientAntiPlugDllBlockSize` 为 0 时除零）
                        GateShareGlobals.g_ClientAntiPlugDllBlockCount =
                            (ClientAntiPlugDllBytes.Length + GateShareGlobals.g_ClientAntiPlugDllBlockSize - 1)
                            / GateShareGlobals.g_ClientAntiPlugDllBlockSize;

                        GateShareGlobals.g_ClientAntiPlugDllSize = MSDll.Length;       // 原 :2503
                        GateShareGlobals.g_ClientAntiPlugVersion = AddData.DllCRC;     // 原 :2505
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 :2153 / :2478 的 `Move(OutBuf, AddData, SizeOf(AddData))`（小端逐字段拷）。</summary>
    public static TAntiPlugAddData ParseAddData(byte[] buf)
    {
        var a = new TAntiPlugAddData();                                    // 原 :2030/:2179 的 AddData 变量
        if (buf == null || buf.Length < ANTI_PLUG_ADD_DATA_SIZE) return a;
        a.RungateType = BitConverter.ToUInt32(buf, 0);
        a.DllCRC = BitConverter.ToUInt32(buf, 4);
        a.DllLen = BitConverter.ToUInt32(buf, 8);
        Array.Copy(buf, 12, a.RandKey, 0, 8);
        return a;
    }

    /// <summary>原文 `IntToStr`/`Move` 之外的二进制串表示：byte[] → 逐字节保真字符串。</summary>
    private static string BytesToBinaryString(byte[] b)
    {
        var chars = new char[b.Length];
        for (int i = 0; i < b.Length; i++) chars[i] = (char)b[i];
        return new string(chars);
    }
}

// ------------------------------------------------------------------------------------
// 死分支常量抽取脚本（RungateLEG_IOCP 若切到 1..5，用它重新生成 5 份 256 位私钥）：
//   $l=[IO.File]::ReadAllLines('…\GateShare.pas.txt')        # GBK 归一化后的 UTF-8 副本
//   $r=[regex]::Matches(($l[2103..2146] -join "`n"), 'RungateLEG_IOCP = (\d)\}([\s\S]*?)(?=\{\$ELSEIF|\{\$IFEND)')
//   foreach($m in $r){ $s=[regex]::Matches($m.Groups[2].Value,"'([0-9A-F]+)'")|%{$_.Groups[1].Value};
//                       "IOCP$($m.Groups[1].Value) exp=$($s[-1]) mod=" + (($s[0..($s.Count-2)]) -join '') }
//   回读结论：IOCP1..6 的 modLen 均为 256，exp 依次为 E31A / A114 / B318 / CB14 / D31F / 010001。
// ------------------------------------------------------------------------------------

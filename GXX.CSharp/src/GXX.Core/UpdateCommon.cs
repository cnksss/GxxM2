using System;
using System.Runtime.InteropServices;

namespace GXX.Core;

/// <summary>
/// UpdateCommon.pas 1:1 转换：更新协议常量 + 三个 packed record。
///
/// 覆盖审计（证据见 docs/并行报告-p2c-common-crypto.md）：
///  既有 src/GXX.Core/** 无同名/同语义产物（UpdateCommon 在源码树中**零调用方**），
///  故本文件按原文逐条新建。
///
/// 原文要点（UpdateCommon.pas:5-67）：
///  - CHECKCODE1..4 与 CM_*/SM_*/WM_* 常量全部 1:1；
///  - TSocketBuffer / TPakKey 是 **packed record**，本移植用
///    [StructLayout(LayoutKind.Sequential, Pack = 1)] + SizeOf 断言锁定布局；
///  - TUpdateInfo 是**非 packed** record，布局由字段自然对齐决定（此处也按 Sequential 默认对齐）。
/// </summary>
public static class UpdateCommon
{
    public const uint CHECKCODE1 = 0xAA55AA55;
    public const uint CHECKCODE2 = 0xFFBBA0DA;
    public const uint CHECKCODE3 = 0xCCD1A05F;
    public const uint CHECKCODE4 = 0xE05FABF3;

    public const int CM_SOCKETCONNECT = 100;      // 连接
    public const int CM_UPDATEBUFFER = 101;       // 请求更新
    public const int CM_STARTUPDATE = 102;        // 开始更新
    public const int CM_GATECHECK = 103;          // 网关检测信号 piaoyun 2013-11-22
    public const int CM_CHECK_CODE_RECV = 104;

    public const int SM_UPDATEBUFFER_OK = 1000;
    public const int SM_UPDATEBUFFER_FAIL = 1001;
    public const int SM_GATECHECK = 1003;

    // WM_DATA = 202; // 202 - 208
    public const int WM_DATA = 202;
    public const int WM_COMPDATA = 203;
    public const int WM_UPDATE_OK = 204;
    public const int WM_UPDATE_FAIL = 205;
    public const int WM_UPDATE_STOP = 206;
    public const int WM_CHECK_CODE = 207;
}

/// <summary>原文 TUpdateFileType = (utFiles, utImage, utIndex)。</summary>
public enum TUpdateFileType
{
    utFiles = 0,
    utImage = 1,
    utIndex = 2,
}

/// <summary>原文 TUpdateDirectory = (dtData, dtMap, dtWav, dtMusic, dtSelf)。</summary>
public enum TUpdateDirectory
{
    dtData = 0,
    dtMap = 1,
    dtWav = 2,
    dtMusic = 3,
    dtSelf = 4,
}

/// <summary>
/// 原文 TSocketBuffer（packed record，UpdateCommon.pas:34-44）。
/// 布局：dwCode1..4(4×4) + dwCrc(4) + Ident(2) + nLength(4) = 26 字节。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TSocketBuffer
{
    public uint dwCode1;
    public uint dwCode2;
    public uint dwCode3;
    public uint dwCode4;

    public uint dwCrc;
    public ushort Ident;
    public int nLength;   // 原 nRecogId 已被注释掉（UpdateCommon.pas:42）

    /// <summary>Delphi SizeOf(TSocketBuffer) = 26（测试断言）。</summary>
    public static int SizeOf => 26;

    public readonly byte[] BytesOf()
    {
        var buf = new byte[SizeOf];
        BitConverter.TryWriteBytes(buf.AsSpan(0, 4), dwCode1);
        BitConverter.TryWriteBytes(buf.AsSpan(4, 4), dwCode2);
        BitConverter.TryWriteBytes(buf.AsSpan(8, 4), dwCode3);
        BitConverter.TryWriteBytes(buf.AsSpan(12, 4), dwCode4);
        BitConverter.TryWriteBytes(buf.AsSpan(16, 4), dwCrc);
        BitConverter.TryWriteBytes(buf.AsSpan(20, 2), Ident);
        BitConverter.TryWriteBytes(buf.AsSpan(22, 4), nLength);
        return buf;
    }

    public static TSocketBuffer FromBytes(byte[] buf, int offset = 0)
    {
        return new TSocketBuffer
        {
            dwCode1 = BitConverter.ToUInt32(buf, offset + 0),
            dwCode2 = BitConverter.ToUInt32(buf, offset + 4),
            dwCode3 = BitConverter.ToUInt32(buf, offset + 8),
            dwCode4 = BitConverter.ToUInt32(buf, offset + 12),
            dwCrc = BitConverter.ToUInt32(buf, offset + 16),
            Ident = BitConverter.ToUInt16(buf, offset + 20),
            nLength = BitConverter.ToInt32(buf, offset + 22),
        };
    }
}

/// <summary>
/// 原文 TUpdateInfo（**非 packed** record，UpdateCommon.pas:47-53）。
/// Handle: THandle → IntPtr；Result: Boolean → bool。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TUpdateInfo
{
    public IntPtr Handle;
    public TUpdateFileType DataType;
    public TUpdateDirectory Directory;
    public int Index;
    public int Position;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Result;
}

/// <summary>
/// 原文 TPakKey（packed record，UpdateCommon.pas:56-62）。
/// 布局：ImageCount(4) + PakType(2) + KeyData(32×4=128) + Chain(20) + Reserve(6×4=24) = 178 字节。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TPakKey
{
    public int ImageCount;
    public ushort PakType;

    public fixed uint KeyData[32];
    public fixed byte Chain[20];
    public fixed int Reserve[6];

    /// <summary>Delphi SizeOf(TPakKey) = 178（测试断言）。</summary>
    public static int SizeOf => 178;
}

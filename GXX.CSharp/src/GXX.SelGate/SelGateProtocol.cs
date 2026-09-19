using System.Runtime.InteropServices;

namespace GXX.SelGate;

/// <summary>
/// SelGate Protocol.pas → SelGateProtocol.cs
/// 1:1 逐字移植：常量（Protocol.pas:9-37）、枚举（:83-84）、记录（:43-103）。
/// 与 LoginGate\Protocol.pas 完全同构（TBlockIPMethod 名称一致，见两文件 :83）；
/// RunGate 使用不同枚举名（bmDisconnect/bmTempBlock/bmBlockList，GateShare.pas:217），故不得合并。
/// </summary>
public static class SelGateProtocol
{
    // ---------------- 常量 Protocol.pas:10-23 ----------------
    public const string _STR_GRID_INDEX = "网关";              // :10
    public const string _STR_GRID_IP = "网关地址";              // :11
    public const string _STR_GRID_PORT = "端口";                // :12
    public const string _STR_GRID_CONNECT_STATUS = "连接状态";   // :13
    public const string _STR_GRID_ONLINE_USER = "通讯";          // :14

    public const string _STR_NOW_START = "正在启动角色网关...";   // :16
    public const string _STR_STARTED = "角色网关启动完成...";     // :17
    public const string _STR_NOW_STOP = "在线";                  // :18（原文如此：名为 NOW_STOP 值却是"在线"）

    public const string _STR_CONFIG_FILE = @".\Config.ini";              // :20
    public const string _STR_BLOCK_FILE = @".\BlockIPList.txt";          // :21
    public const string _STR_BLOCK_AREA_FILE = @".\BlockIPAreaList.txt"; // :22
    public const string _STR_USER_NAME_FILTER_FILE = @".\NewChrNameFilter.txt"; // :23

    // ---------------- 定时器 ID Protocol.pas:25-29（WM_USER = $0400 = 1024）----------------
    public const int WM_USER = 0x0400;
    public const int _IDM_SERVERSOCK_MSG = WM_USER + 1000;              // :25 = 2024
    public const int _IDM_TIMER_STARTSERVICE = _IDM_SERVERSOCK_MSG + 1; // :26 = 2025
    public const int _IDM_TIMER_STOPSERVICE = _IDM_SERVERSOCK_MSG + 2;  // :27 = 2026
    public const int _IDM_TIMER_KEEP_ALIVE = _IDM_SERVERSOCK_MSG + 3;   // :28 = 2027
    public const int _IDM_TIMER_THREAD_INFO = _IDM_SERVERSOCK_MSG + 4;  // :29 = 2028

    /// <summary>Protocol.pas:32 —— 原文八进制字面量 0080（原文如此），Delphi 八进制 80 = 十进制 64。</summary>
    public const int FIRST_PAKCET_MAX_LEN = 64; // 0080(oct) = 64(dec)

    public const int MAX_FUNC_COUNT = 1024;             // :35
    public const int MAX_SERVER_FUNC_SIZE = 16 * 1024;  // :36
    public const int MAX_CLIENT_FUNC_SIZE = 16 * 1024;  // :37

    // ---------------- 消息结构布局 Protocol.pas:43-81（与 Common\Grobal2.pas 一致）----------------

    /// <summary>Protocol.pas:43-49 TCmdPack = packed record（16 字节；TDefaultMessage 为其别名 :52）。</summary>
    public const int SizeOfTCmdPack = 16;

    /// <summary>Protocol.pas:55-62 TSvrCmdPack = packed record（20 字节）。</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TSvrCmdPack
    {
        public uint Flag;    // :56
        public uint SockID;  // :57
        public ushort Seq;   // :58
        public ushort Cmd;   // :59
        public int GGSock;   // :60 TClientThread
        public int DataLen;  // :61
    }

    public const int SizeOfTSvrCmdPack = 20; // 4+4+2+2+4+4

    /// <summary>Protocol.pas:65-73 _tagCmdHeader = packed record（12 字节）。</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TCmdHeader
    {
        public uint Header;  // :68
        public ushort Cmd;   // :69
        public ushort Cmd1;  // :70
        public uint Tail;    // :71
    }

    public const int SizeOfTCmdHeader = 12;

    /// <summary>Protocol.pas:75-81 TEnDeInfo = packed record（12 字节，字段与 _tagCmdHeader 相同）。</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TEnDeInfo
    {
        public uint Head;   // :76
        public ushort Cmd;  // :77
        public ushort Cmd1; // :78
        public uint Tail;   // :79
    }

    public const int SizeOfTEnDeInfo = 12;

    // ---------------- IP 过滤相关记录（非 packed，Delphi 32 位自然对齐）----------------
}

/// <summary>
/// Protocol.pas:86-90 TPerIPAddr = record；非 packed，Delphi 32 位下 8 字节（两个 4 字节成员）。
/// C# 托管投影统一用 LayoutKind.Sequential（不保证 Marshal.SizeOf 与 Delphi SizeOf 相等，
/// 与既有 GatewayKit/GateService.cs 的 TPerIPAddr 处理方式一致）。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SelPerIPAddr
{
    public int IPaddr; // :87 LongInt
    public int Count;  // :88 Integer
}

/// <summary>Protocol.pas:92-97 TNewIDAddr = record；12 字节（LongInt+Integer+LongWord）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TNewIDAddr
{
    public int IPaddr;          // :93
    public int Count;           // :94
    public uint dwIDCountTick;  // :95
}

/// <summary>Protocol.pas:99-103 TIPArea = record；8 字节（两个 DWORD）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TIPArea
{
    public uint Low;   // :100
    public uint High;  // :101
}

/// <summary>Protocol.pas:83 TBlockIPMethod = (mDisconnect, mBlock, mBlockList)。</summary>
public enum TBlockIPMethod
{
    mDisconnect = 0, // :83 第 1 项
    mBlock = 1,      // :83 第 2 项 ← SelGate 的"动态/临时"过滤
    mBlockList = 2   // :83 第 3 项 ← SelGate 的"永久"过滤
}

/// <summary>Protocol.pas:84 TSockThreadStutas = (stConnecting, stConnected, stTimeOut)。</summary>
public enum TSockThreadStutas
{
    stConnecting = 0, // :84
    stConnected = 1,  // :84
    stTimeOut = 2     // :84
}

/// <summary>
/// Protocol.pas:105-110 全局变量（g_hMainWnd/g_hGameCenterHandle/g_boNetComGate/
/// g_fCanClose/g_fServiceStarted）。线程可见性用 volatile 表达 Delphi 的普通全局读。
/// </summary>
public static class SelGateGlobals
{
    public static IntPtr g_hMainWnd;                  // :106
    public static IntPtr g_hGameCenterHandle;         // :107
    public static volatile bool g_boNetComGate;       // :108
    public static volatile bool g_fCanClose = false;  // :109
    public static volatile bool g_fServiceStarted = false; // :110
}

/// <summary>Protocol.pas:39-41 动态代码回调类型（LPDYNCODE/LPGETDYNCODE）——仅登记，SelGate 未使用。</summary>
public delegate byte LPDYNCODE(byte[] pszBuffer, uint len);
public delegate LPDYNCODE? LPGETDYNCODE(int id);

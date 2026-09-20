// ============================================================================
// 源单元：Source\Common\HardInfo.pas（GBK，898 行）
// 本文件：原文的类型面（interface :11-40 的 TRegisters/TCPUID）+ 该单元**接缝契约**
//         所需的 Windows.pas / Nb30.pas 结构（原文通过 uses 引用，其声明不在 HardInfo.pas 内）。
//
// 逐段对照：
//   HardInfo.pas:13-18   TRegisters = record EAX/EBX/ECX/EDX: DWORD end
//   HardInfo.pas:19      TCPUID = array [1 .. 4] of Longint
//   HardInfo.pas:50-59   TIDERegs（GetIdeSerialNumber 内嵌）
//   HardInfo.pas:60-70   TSendCmdInParams（GetIdeSerialNumber 内嵌）
//   HardInfo.pas:71-103  TIdSector / PIdSector（GetIdeSerialNumber 内嵌）
//   HardInfo.pas:104-119 TDriverStatus / TSendCmdOutParams（GetIdeSerialNumber 内嵌）
//   HardInfo.pas:400-468 GetIdeDiskSerialNumber 内嵌的第二套同构声明（TSrbIoControl 为新增）
//   HardInfo.pas:670-700 TNcb / TLanaEnum / TAdapterStatus（uses Nb30）
//   HardInfo.pas:209/217/289/389 TDeviceMode / TDisplayDevice / TMemoryStatusEx / TSYSTEMINFO（uses Windows）
//   HardInfo.pas:329     SYSTEM_LOGICAL_PROCESSOR_INFORMATION（uses Windows）
//
// ★ 布局纪律：本文件所有结构按 **Delphi 7 / Win32 (x86)** 的布局声明 —— 原文即 32 位编译产物。
//   指针成员用 `nint`（.NET 自然语义），故 **TNCB / TSystemInfo 在 x64 下与 Win32 布局不同**
//   （TNCB x86=64 / x64=88，且 Win64 原生 NCB 的 ncb_reserve 是 18 字节；TSystemInfo x86=36 / x64=44）。
//   不含指针的结构布局与平台无关，已由 HardwareLayoutTests 用 Marshal.SizeOf/OffsetOf 锁死。
// ============================================================================
using System.Runtime.InteropServices;

namespace GXX.Core.Hardware;

/// <summary>
/// 原文 <c>HardInfo.pas:13-18</c>：
/// <code>
/// TRegisters = record
///   EAX: DWORD;  EBX: DWORD;  ECX: DWORD;  EDX: DWORD;
/// end;
/// </code>
/// 被 <c>GetCPUID</c>（:253）写出、被 <c>GetCpuName</c>（:307）作为 3 个叶的容器使用。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TRegisters
{
    public uint EAX;
    public uint EBX;
    public uint ECX;
    public uint EDX;
}

/// <summary>
/// 原文 <c>HardInfo.pas:19</c>：<c>TCPUID = array [1 .. 4] of Longint;</c> —— **1-based**。
/// <para>
/// ★ 原文**全单元从未使用**该类型，全部调用点（MShare.pas / M2Share.pas）也从未引用（死类型）。
/// 此处按 1-based 语义建模以保留边界行为：下标合法域 1..4，越界抛 <see cref="IndexOutOfRangeException"/>
/// （Delphi 开 <c>{$R+}</c> 时为 ERangeError；原文工程未开范围检查 ⇒ 原文越界是**越权读写**）。
/// </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TCPUID
{
    private int _e1;
    private int _e2;
    private int _e3;
    private int _e4;

    public int this[int index]
    {
        get => index switch
        {
            1 => _e1,
            2 => _e2,
            3 => _e3,
            4 => _e4,
            _ => throw new IndexOutOfRangeException(
                "TCPUID 下标 " + index + " 越界（原文 TCPUID = array [1 .. 4] of Longint，HardInfo.pas:19）"),
        };
        set
        {
            switch (index)
            {
                case 1: _e1 = value; break;
                case 2: _e2 = value; break;
                case 3: _e3 = value; break;
                case 4: _e4 = value; break;
                default:
                    throw new IndexOutOfRangeException(
                        "TCPUID 下标 " + index + " 越界（原文 TCPUID = array [1 .. 4] of Longint，HardInfo.pas:19）");
            }
        }
    }
}

// ---------------------------------------------------------------------------
// IDE / S.M.A.R.T. 结构（原文 :50-119 与 :400-468 的两套同构声明）
// ---------------------------------------------------------------------------

/// <summary>原文 <c>HardInfo.pas:50-59</c>（GetIdeSerialNumber 内嵌）与 <c>:412-421</c>（GetIdeDiskSerialNumber 内嵌）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TIDERegs
{
    public byte bFeaturesReg;      // Used for specifying SMART "commands".
    public byte bSectorCountReg;   // IDE sector count register
    public byte bSectorNumberReg;  // IDE sector number register
    public byte bCylLowReg;        // IDE low order cylinder value
    public byte bCylHighReg;       // IDE high order cylinder value
    public byte bDriveHeadReg;     // IDE drive/head register
    public byte bCommandReg;       // Actual IDE command.
    public byte bReserved;         // reserved for future use.  Must be zero.
}

/// <summary>原文 <c>HardInfo.pas:60-70</c> / <c>:425-434</c>（<c>packed record</c>，SizeOf = 33）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSendCmdInParams
{
    public uint cBufferSize;                  // 0   Buffer size in bytes
    public TIDERegs irDriveRegs;              // 4   Structure with drive register values.
    public byte bDriveNumber;                 // 12  Physical drive number (0,1,2,3).
    public fixed byte bReserved[3];           // 13
    public fixed uint dwReserved[4];          // 16
    public fixed byte bBuffer[1];             // 32  Input buffer.
}

/// <summary>原文 <c>HardInfo.pas:104-111</c>（<c>packed record</c>，SizeOf = 12）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDriverStatus
{
    public byte bDriverError;        // 驱动器返回的错误代码，无错则返回0
    public byte bIDEStatus;          // IDE出错寄存器的内容，只有当 bDriverError 为 SMART_IDE_ERROR 时有效
    public fixed byte bReserved[2];
    public fixed uint dwReserved[2];
}

/// <summary>
/// 原文 <c>HardInfo.pas:112-119</c>（<c>packed record</c>，SizeOf = 17）。
/// 原文 <c>aIdOutCmd</c> 就是该结构的 backing buffer（<c>:125-126</c> 的 <c>absolute</c> 技巧）。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSendCmdOutParams
{
    public uint cBufferSize;              // 0   bBuffer 的大小
    public TDriverStatus DriverStatus;    // 4   驱动器状态
    public fixed byte bBuffer[1];         // 16  从驱动器读出的数据（IDENTIFY 扇区起点）
}

/// <summary>
/// 原文 <c>HardInfo.pas:71-103</c> / <c>:436-468</c>（<c>packed record</c>，SizeOf = 256，
/// 逐字段偏移见 <see cref="HardInfoLayout"/>）。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TIdSector
{
    public ushort wGenConfig;                     //   0
    public ushort wNumCyls;                       //   2
    public ushort wReserved;                      //   4
    public ushort wNumHeads;                      //   6
    public ushort wBytesPerTrack;                 //   8
    public ushort wBytesPerSector;                //  10
    public ushort wSectorsPerTrack;               //  12
    public fixed ushort wVendorUnique[3];         //  14
    public fixed byte sSerialNumber[20];          //  20
    public ushort wBufferType;                    //  40
    public ushort wBufferSize;                    //  42
    public ushort wECCSize;                       //  44
    public fixed byte sFirmwareRev[8];            //  46
    public fixed byte sModelNumber[40];           //  54
    public ushort wMoreVendorUnique;              //  94
    public ushort wDoubleWordIO;                  //  96
    public ushort wCapabilities;                  //  98
    public ushort wReserved1;                     // 100
    public ushort wPIOTiming;                     // 102
    public ushort wDMATiming;                     // 104
    public ushort wBS;                            // 106
    public ushort wNumCurrentCyls;                // 108
    public ushort wNumCurrentHeads;               // 110
    public ushort wNumCurrentSectorsPerTrack;     // 112
    public uint ulCurrentSectorCapacity;          // 114
    public ushort wMultSectorStuff;               // 118
    public uint ulTotalAddressableSectors;        // 120
    public ushort wSingleWordDMA;                 // 124
    public ushort wMultiWordDMA;                  // 126
    public fixed byte bReserved[128];             // 128  → 合计 256
}

/// <summary>原文 <c>HardInfo.pas:400-410</c>（GetIdeDiskSerialNumber 内嵌，<c>packed record</c>，SizeOf = 28）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSrbIoControl
{
    public uint HeaderLength;          //  0
    public fixed byte Signature[8];    //  4  System.Move('SCSIDISK', ..., 8)
    public uint Timeout;               // 12
    public uint ControlCode;           // 16
    public uint ReturnCode;            // 20
    public uint Length;                // 24
}

// ---------------------------------------------------------------------------
// Nb30.pas（uses HardInfo.pas:9 `Nb30 {Net bios 30}`）—— 本仓库无该单元源码，
// 按 Win32 NB30.H 与 Delphi 7 Nb30.pas 的重合定义重建（逐字段与 NB30.H 一致）。
// ---------------------------------------------------------------------------

/// <summary>原文 <c>HardInfo.pas:667</c> <c>Ncb: TNcb</c>。NB30.H 的 <c>NCB</c>。
/// <para>★ 含指针 ⇒ x86 SizeOf = 64（= Win32 NCB），x64 SizeOf = 88（Win64 原生 NCB 为 96，ncb_reserve 18 字节）
/// —— 接缝的**真实实现若跑 x64 需另行声明原生布局**（见 docs/并行报告-p6-core-hardinfo.md）。</para></summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TNCB
{
    public byte ncb_command;
    public byte ncb_retcode;
    public byte ncb_lsn;
    public byte ncb_num;
    public nint ncb_buffer;
    public ushort ncb_length;
    public fixed byte ncb_callname[16];   // NCBNAMSZ = 16
    public fixed byte ncb_name[16];       // NCBNAMSZ = 16
    public byte ncb_rto;
    public byte ncb_sto;
    public nint ncb_post;
    public byte ncb_lana_num;
    public byte ncb_cmd_cplt;
    public fixed byte ncb_reserve[10];    // Win32 为 10，Win64 为 18
    public nint ncb_event;
}

/// <summary>原文 <c>HardInfo.pas:669</c> <c>Lanaenum: TLanaenum</c>。NB30.H 的 <c>LANA_ENUM</c>（SizeOf = 256）。</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TLanaEnum
{
    public byte length;                 // 0  Number of valid entries in lana[]
    public fixed byte lana[255];        // 1  lana: array [0..MAX_LANA] of UCHAR，MAX_LANA = 254

    /// <summary>SizeOf(TLanaEnum) = 1 + 255 = 256（= `Ncb.ncb_length := SizeOf(Lanaenum)`，原文 :680）。</summary>
    public const int Size = 256;

    /// <summary>字段 <c>lana</c> 相对本结构的偏移（= 1）。</summary>
    public const int LanaOffset = 1;

    /// <summary>
    /// 原文 <c>Lanaenum.lana[aNo]</c>（:685/:691）的等价读取。
    /// <para>★ 原文缺陷：<paramref name="index"/> 未做 0..254 边界检查 ⇒
    /// <c>index = -1</c> 会读到相邻的 <c>length</c> 字节，<c>index &lt; -1</c> 或 <c>&gt; 254</c> 越权读写相邻内存；
    /// 托管侧一律抛 <see cref="IndexOutOfRangeException"/>（差异登记）。</para>
    /// </summary>
    public static byte GetLana(byte[] buffer, int index)
    {
        if (index < 0 || index > 254)
            throw new IndexOutOfRangeException(
                "Lanaenum.lana[" + index + "] 越界（原文 array [0..MAX_LANA]，MAX_LANA = 254；原文 HardInfo.pas:685/:691 无边界检查）");
        return buffer[LanaOffset + index];
    }
}

/// <summary>原文 <c>HardInfo.pas:668</c> <c>Adapter: TAdapterStatus</c>。NB30.H 的 <c>ADAPTER_STATUS</c>（SizeOf = 60）。</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TAdapterStatus
{
    public fixed byte adapter_address[6];   // 0  ★ 原文 :700 只使用这 6 字节
    public byte rev_major;                  // 6
    public byte reserved0;                  // 7
    public byte adapter_type;               // 8
    public byte rev_minor;                  // 9
    public ushort duration;                 // 10
    public ushort frmr_recv;                // 12
    public ushort frmr_xmit;                // 14
    public ushort iframe_recv_err;          // 16
    public ushort xmit_aborts;              // 18
    public uint xmit_success;               // 20
    public uint recv_success;               // 24
    public ushort iframe_xmit_err;          // 28
    public ushort recv_buff_unavail;        // 30
    public ushort t1_timeouts;              // 32
    public ushort ti_timeouts;              // 34
    public uint reserved1;                  // 36
    public ushort free_ncbs;                // 40
    public ushort max_cfg_ncbs;             // 42
    public ushort max_ncbs;                 // 44
    public ushort xmit_buf_unavail;         // 46
    public ushort max_dgram_size;           // 48
    public ushort pending_sess;             // 50
    public ushort max_cfg_sess;             // 52
    public ushort max_sess;                 // 54
    public ushort max_sess_pkt_size;        // 56
    public ushort name_count;               // 58  → 合计 60

    /// <summary>SizeOf(TAdapterStatus) = 60（= 原文 :694 <c>Ncb.ncb_length := SizeOf(Adapter)</c>）。</summary>
    public const int Size = 60;
}

// ---------------------------------------------------------------------------
// Windows.pas 结构（原文 uses Windows.pas:5）
// ---------------------------------------------------------------------------

/// <summary>原文 <c>HardInfo.pas:600</c> <c>VersionInfo: TOSVersionInfo</c>（HGE 版为 TOSVersionInfoA，SizeOf = 148）。</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TOSVersionInfo
{
    public uint dwOSVersionInfoSize;   //   0
    public uint dwMajorVersion;        //   4
    public uint dwMinorVersion;        //   8
    public uint dwBuildNumber;         //  12
    public uint dwPlatformId;          //  16  ★ 原文拼作 `dwPlatformid`（:610），Delphi 大小写不敏感
    public fixed byte szCSDVersion[128]; // 20 → 合计 148

    /// <summary>SizeOf(TOSVersionInfo) = 148（= 原文 :604 <c>VersionInfo.dwOSVersionInfoSize := SizeOf(VersionInfo)</c>）。</summary>
    public const int Size = 148;

    /// <summary>原文 <c>VersionInfo.szCSDVersion[k]</c> 的等价读取（0-based，与 Delphi 一致）。</summary>
    public byte CSDVersionAt(int index) => szCSDVersion[index];

    /// <summary>
    /// 原文 <c>VersionInfo.szCSDVersion &lt;&gt; ''</c>（:650）的等价判断：
    /// <c>array[0..127] of AnsiChar</c> 与字符串比较 ⇒ 按 **NUL 结尾的 C 串**比较，即"首字节非 0"。
    /// </summary>
    public string CSDVersionString()
    {
        int n = 0;
        while (n < 128 && szCSDVersion[n] != 0) n++;
        return GXX.Core.Rtl.DelphiRTL.AnsiString(ToBytes(n), 0, n);
    }

    /// <summary>测试/接缝辅助（非原文成员）：写入 szCSDVersion（不足补 0，超长截断到 127）。</summary>
    public void SetCSDVersion(string value)
    {
        var bytes = GXX.Core.Rtl.DelphiRTL.AnsiBytes(value ?? "");
        for (int i = 0; i < 128; i++) szCSDVersion[i] = i < bytes.Length ? bytes[i] : (byte)0;
    }

    private byte[] ToBytes(int count)
    {
        var b = new byte[count];
        for (int i = 0; i < count; i++) b[i] = szCSDVersion[i];
        return b;
    }
}

/// <summary>
/// 原文 <c>HardInfo.pas:209</c> <c>DeviceMode: TDeviceMode</c>。
/// <para>★ 只声明到 <c>dmDisplayFrequency</c>（原文 :212 只读该字段）；完整 DEVMODEA 为 156 字节，
/// 本声明为 124 字节（余下 dmPanningWidth/dmPanningHeight/dmDisplayOrientation 等未声明）。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TDeviceMode
{
    public fixed byte dmDeviceName[32];   //   0  CCHDEVICENAME = 32
    public ushort dmSpecVersion;          //  32
    public ushort dmDriverVersion;        //  34
    public ushort dmSize;                 //  36
    public ushort dmDriverExtra;          //  38
    public uint dmFields;                 //  40
    public short dmOrientation;           //  44
    public short dmPaperSize;             //  46
    public short dmPaperLength;           //  48
    public short dmPaperWidth;            //  50
    public short dmScale;                 //  52
    public short dmCopies;                //  54
    public short dmDefaultSource;         //  56
    public short dmPrintQuality;          //  58
    public short dmColor;                 //  60
    public short dmDuplex;                //  62
    public short dmYResolution;           //  64
    public short dmTTOption;              //  66
    public short dmCollate;               //  68
    public fixed byte dmFormName[32];     //  70  CCHFORMNAME = 32
    public ushort dmLogPixels;            // 102
    public uint dmBitsPerPel;             // 104
    public uint dmPelsWidth;              // 108
    public uint dmPelsHeight;             // 112
    public uint dmDisplayFlags;           // 116  原文 Windows.pas：与 dmNup 共用同一 DWORD
    public uint dmDisplayFrequency;       // 120  ★ 原文 :212 读取
}

/// <summary>原文 <c>HardInfo.pas:217</c> <c>lpDisplayDevice: TDisplayDevice</c>（DISPLAY_DEVICEA，SizeOf = 424）。</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TDisplayDevice
{
    public uint cb;                       //   0  ★ 原文 :221 只设置一次（循环外）
    public fixed byte DeviceName[32];     //   4  CCHDEVICENAME = 32
    public fixed byte DeviceString[128];  //  36
    public uint StateFlags;               // 164
    public fixed byte DeviceID[128];      // 168
    public fixed byte DeviceKey[128];     // 296  → 合计 424

    /// <summary>SizeOf(TDisplayDevice) = 424。</summary>
    public const int Size = 424;

    /// <summary>原文 <c>lpDisplayDevice.DeviceName = '\\.\Display1'</c>（:227）的等价读取（NUL 结尾 C 串）。</summary>
    public string DeviceNameString()
    {
        fixed (byte* p = DeviceName) return FixedAnsiToString(p, 32);
    }

    /// <summary>原文 <c>lpDisplayDevice.DeviceString</c>（:228）的等价读取（NUL 结尾 C 串）。</summary>
    public string DeviceStringString()
    {
        fixed (byte* p = DeviceString) return FixedAnsiToString(p, 128);
    }

    /// <summary>测试/接缝辅助（非原文成员）。</summary>
    public void SetDeviceName(string value)
    {
        fixed (byte* p = DeviceName) SetFixedAnsi(p, 32, value);
    }

    /// <summary>测试/接缝辅助（非原文成员）。</summary>
    public void SetDeviceString(string value)
    {
        fixed (byte* p = DeviceString) SetFixedAnsi(p, 128, value);
    }

    private static string FixedAnsiToString(byte* p, int len)
    {
        int n = 0;
        while (n < len && p[n] != 0) n++;
        var b = new byte[n];
        for (int i = 0; i < n; i++) b[i] = p[i];
        return GXX.Core.Rtl.DelphiRTL.AnsiString(b, 0, n);
    }

    private static void SetFixedAnsi(byte* p, int len, string value)
    {
        var bytes = GXX.Core.Rtl.DelphiRTL.AnsiBytes(value ?? "");
        for (int i = 0; i < len; i++) p[i] = i < bytes.Length ? bytes[i] : (byte)0;
    }
}

/// <summary>原文 <c>HardInfo.pas:289</c> <c>oMemoinfo: TMemoryStatusEx</c>（MEMORYSTATUSEX，SizeOf = 64）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TMemoryStatusEx
{
    public uint dwLength;
    public uint dwMemoryLoad;
    public ulong ullTotalPhys;
    public ulong ullAvailPhys;
    public ulong ullTotalPageFile;
    public ulong ullAvailPageFile;
    public ulong ullTotalVirtual;
    public ulong ullAvailVirtual;
    public ulong ullAvailExtendedVirtual;

    /// <summary>SizeOf(TMemoryStatusEx) = 64。</summary>
    public const int Size = 64;
}

/// <summary>
/// 原文 <c>HardInfo.pas:389</c> <c>SysInfo: TSYSTEMINFO</c>（原文拼写，即 Windows.pas 的 <c>TSystemInfo</c>）。
/// <para>★ 含指针：x86 SizeOf = 36，x64 SizeOf = 44。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TSystemInfo
{
    public uint dwOemId;                        //  0
    public uint dwPageSize;                     //  4
    public nint lpMinimumApplicationAddress;    //  8
    public nint lpMaximumApplicationAddress;    // 12 (x86)
    public uint dwActiveProcessorMask;
    public uint dwNumberOfProcessors;           //  ★ 原文 :392
    public uint dwProcessorType;                //  ★ 原文 :393
    public uint dwAllocationGranularity;
    public ushort wProcessorLevel;
    public ushort wProcessorRevision;           //  ★ 原文 :393
}

/// <summary>
/// 原文 <c>HardInfo.pas:363</c> <c>case Buffer[III].Relationship of</c>（Windows.pas 的
/// <c>TLogicalProcessorRelationship</c>；Delphi 7 只声明这 4 个值）。
/// <para>★ 现代 Windows 会返回 4(RelationGroup)/5(RelationProcessorDie)/6/7 等**枚举外**取值，
/// 而原文 <c>case</c> **无 else** ⇒ 一律静默忽略（差异断言见 HardwareCpuCountTests）。</para>
/// </summary>
public enum TLogicalProcessorRelationship
{
    RelationProcessorCore = 0,
    RelationNumaNode = 1,
    RelationCache = 2,
    RelationProcessorPackage = 3,
}

/// <summary>原文 <c>HardInfo.pas:363-377</c> 的 <c>RelationCache</c> 联合成员（CACHE_DESCRIPTOR，12 字节）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TCacheDescriptor
{
    public byte Level;
    public byte Associativity;
    public ushort LineSize;
    public uint CacheSize;
    public int Type;
}

/// <summary>
/// SYSTEM_LOGICAL_PROCESSOR_INFORMATION 的匿名联合。
/// 现实布局为 <c>ULONGLONG Reserved[2]</c>（16 字节）⇒ 联合大小 16。
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct TLogicalProcessorInformationUnion
{
    [FieldOffset(0)] public byte ProcessorCoreFlags;
    [FieldOffset(0)] public uint NumaNodeNumber;
    [FieldOffset(0)] public TCacheDescriptor Cache;
    [FieldOffset(0)] public ulong Reserved0;
    [FieldOffset(8)] public ulong Reserved1;
}

/// <summary>
/// 原文 <c>HardInfo.pas:329</c> <c>Buffer: array of SYSTEM_LOGICAL_PROCESSOR_INFORMATION</c>。
/// <para>★ <c>ProcessorMask</c> 原文为 <c>ULONG_PTR</c>；按 Delphi 7（x86）取 4 字节，
/// 使 <c>Marshal.SizeOf</c> 恒为 24 = 原文 <c>sizeof(...)</c>（x64 原生为 32）。
/// <c>HardInfoLayout.SizeOfSystemLogicalProcessorInformation = 24</c> 与之配套。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
{
    public uint ProcessorMask;                              // 0  ULONG_PTR（x86）
    public TLogicalProcessorRelationship Relationship;      // 4
    public TLogicalProcessorInformationUnion Union;         // 8（16 字节）→ 合计 24
}

/// <summary>
/// 原文 <c>packed record</c> / <c>array[0..N] of T</c> 的**关键偏移与大小**（:50-119、:400-468）。
/// <para>每一个常量都由 HardwareLayoutTests 用 <c>Marshal.SizeOf</c>/<c>Marshal.OffsetOf</c> 回读比对，
/// 并与原文逐字段累加值对照（原文缺陷/易错点见 docs/并行报告-p6-core-hardinfo.md）。</para>
/// </summary>
public static class HardInfoLayout
{
    /// <summary><c>sizeof(TIdSector)</c>（原文 :71-103 逐字段累加）。</summary>
    public const int IdSectorSize = 256;

    /// <summary><c>@sSerialNumber</c> 相对 TIdSector 的偏移；原文 :180 对其做 20 字节字节序变换。</summary>
    public const int IdSector_sSerialNumber = 20;

    /// <summary><c>sSerialNumber</c> 长度（<c>array[0..19] of CHAR</c>）。</summary>
    public const int IdSector_sSerialNumberLength = 20;

    /// <summary><c>@wBufferType</c>：原文 :181 的 <c>(PChar(@sSerialNumber) + 20)^ := #0</c> **越界写**到这里。</summary>
    public const int IdSector_wBufferType = 40;

    /// <summary><c>@sFirmwareRev</c>；原文 :583 对其做 8 字节变换。</summary>
    public const int IdSector_sFirmwareRev = 46;

    /// <summary><c>sFirmwareRev</c> 长度（<c>array[0..7] of Char</c>）。</summary>
    public const int IdSector_sFirmwareRevLength = 8;

    /// <summary><c>@sModelNumber</c>；原文 :580 对其做 40 字节变换。</summary>
    public const int IdSector_sModelNumber = 54;

    /// <summary><c>sModelNumber</c> 长度（<c>array[0..39] of Char</c>）。</summary>
    public const int IdSector_sModelNumberLength = 40;

    /// <summary><c>@wNumCurrentSectorsPerTrack</c>；原文 :592 对其做 2 字节变换。</summary>
    public const int IdSector_wNumCurrentSectorsPerTrack = 112;

    /// <summary><c>@ulCurrentSectorCapacity</c>；原文 :589 对其做 4 字节变换。</summary>
    public const int IdSector_ulCurrentSectorCapacity = 114;

    /// <summary><c>@ulTotalAddressableSectors</c>；原文 :586 对其做 4 字节变换。</summary>
    public const int IdSector_ulTotalAddressableSectors = 120;

    /// <summary><c>sizeof(TSendCmdOutParams)</c> = 17；原文 :125 用 <c>cBufferSize..bBuffer</c> 的 absolute 技巧。</summary>
    public const int SendCmdOutParamsSize = 17;

    /// <summary><c>@bBuffer</c> 相对 TSendCmdOutParams 的偏移 = 16（原文 :178 <c>PIdSector(@IdOutCmd.bBuffer)^</c>）。</summary>
    public const int SendCmdOutParams_bBuffer = 16;

    /// <summary><c>sizeof(TSendCmdInParams)</c> = 33（原文 :155/:173 用 <c>SizeOf(...)-1</c> = 32）。</summary>
    public const int SendCmdInParamsSize = 33;

    /// <summary><c>@bBuffer</c> 相对 TSendCmdInParams 的偏移 = 32（原文 :552 <c>pOutData := @pInData^.bBuffer</c>）。</summary>
    public const int SendCmdInParams_bBuffer = 32;

    /// <summary><c>sizeof(TSrbIoControl)</c> = 28（原文 :515/:521 的 +sizeof(SRB_IO_CONTROL)）。</summary>
    public const int SrbIoControlSize = 28;

    /// <summary>
    /// 原文 <c>sizeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION)</c>：Delphi 7 生成 x86 代码 ⇒ 24。
    /// <para>原文 :346/:360 用它做 <c>ReturnLength div sizeof</c> 换算；接缝实现必须用**同一常量**填 ReturnLength。</para>
    /// </summary>
    public const int SizeOfSystemLogicalProcessorInformation = 24;

    /// <summary>
    /// GetIdeSerialNumber 从 <c>aIdOutCmd</c> 定位 IDENTIFY 扇区的偏移：
    /// <c>sizeof(TSendCmdOutParams 头部) + 16</c> —— 原文 :178 是 <c>bBuffer</c>（16），
    /// 而 GetIdeDiskSerialNumber 的 NT 分支是 <c>sizeof(SRB_IO_CONTROL) + 16</c>（:520-522、:575）。
    /// </summary>
    public const int IdeSerialNumberIdentifyOffset = SendCmdOutParams_bBuffer;

    /// <summary>
    /// GetIdeDiskSerialNumber NT 分支定位 IDENTIFY 扇区的偏移：
    /// <c>sizeof(SRB_IO_CONTROL)(28) + 16</c> = 44。
    /// </summary>
    public const int IdeDiskSerialNumberScsiIdentifyOffset = SrbIoControlSize + 16;

    /// <summary>
    /// GetIdeDiskSerialNumber Win9x 分支定位 IDENTIFY 扇区的偏移：
    /// <c>@pInData^.bBuffer(32) + 16</c> = 48 —— ★ 与 NT 分支的 44 **不同**（原文缺陷，见报告）。
    /// </summary>
    public const int IdeDiskSerialNumberW9xIdentifyOffset = SendCmdInParams_bBuffer + 16;
}

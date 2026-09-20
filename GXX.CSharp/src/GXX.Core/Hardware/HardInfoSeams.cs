// ============================================================================
// 源单元：Source\Common\HardInfo.pas（GBK，898 行）
// 本文件：**接缝层**（不是移植单元本体）。HardInfo.pas 的每一个 OS 调用都在这里落到接口上。
//
// 为什么必须这样切：原单元的行为是"采集真实硬件信息"，其结果在无头 CI 里不可断言。
// 因此把"取原始数据"（OS 调用 → 本文件）与"怎么处理数据"（纯函数 → HardInfo.cs）分开，
// 后者用断言锁死，前者由集成方注入真实实现。
//
// ★ 本文件与默认实现 **不含任何 P/Invoke**，与 GXX.Core 全项目现状一致
//   （`git grep DllImport -- 'GXX.CSharp/src/GXX.Core'` 零命中）。
//   默认实现一律返回"失败/空"，绝不做真实 OS 调用。
//
// 逐调用点对照（行号 = HardInfo.pas）：
//   :145/:506  SysUtils.Win32Platform                     → IDiskIdentifyApi.Win32Platform
//   :211       EnumDisplaySettings                        → IDisplayApi.EnumDisplaySettings
//   :224       EnumDisplayDevices                         → IDisplayApi.EnumDisplayDevices
//   :253-284   CPUID（内联 asm）                           → ICpuIdApi.GetCPUID
//   :294       GlobalMemoryStatusEx                       → IMemoryApi.GlobalMemoryStatusEx
//   :293/:342/:348 GetLogicalProcessorInformation          → IProcessorInformationApi
//   :344       GetLastError                               → IProcessorInformationApi.GetLastError
//   :391       GetSystemInfo                              → IProcessorInformationApi.GetSystemInfo
//   :148/:152/:173 DeviceIoControl（PhysicalDrive0/SMARTVSD）→ IDiskIdentifyApi
//   :509-540   CreateFile('\\.\Scsi0:')/DeviceIoControl（IOCTL_SCSI_MINIPORT）→ IDiskIdentifyApi
//   :606       GetVersionEx                               → IVersionApi.GetVersionEx
//   :678-695   NetBios（Nb30）                             → INetBiosApi.NetBios
//   :719-745   RegOpenKeyEx/RegEnumKeyEx/RegEnumValue/RegCloseKey → IRegistryApi
//   :760-767   TRegistry（Create/RootKey/OpenKey/ReadString/CloseKey/Free）→ IRegistryApi
//   :792-799   CreateFile('\\.\'+Name)/DeviceIoControl（IOCTL_NDIS_QUERY_GLOBAL_STATS）→ INdisQueryApi
// ============================================================================

namespace GXX.Core.Hardware;

/// <summary>
/// 原文 <c>HardInfo.pas:606</c> <c>GetVersionEx(VersionInfo)</c>（SysUtils/Windows 的 <c>GetVersionExA</c>）。
/// <para>接缝：待真实 Windows 版本探测（Windows.pas / WMI）移植后接入。</para>
/// <para>注意：<c>GetVersionEx</c> 自 Win8.1 起对未声明 supportedOS 的进程返回"假"版本号 ⇒ 真实实现
/// 应改用 <c>RtlGetVersion</c> 或 WMI（见 Checklist.md:269"HardInfo 机器码需 WMI 深化"）。</para>
/// </summary>
public interface IVersionApi
{
    /// <summary>原文 <c>GetVersionEx(VersionInfo)</c>（返回 <c>Boolean</c>，原文 :606 **忽略返回值**）。</summary>
    bool GetVersionEx(ref TOSVersionInfo versionInfo);
}

/// <summary>
/// 原文 <c>HardInfo.pas:211</c> <c>EnumDisplaySettings(nil, Cardinal(-1), DeviceMode)</c> 与
/// <c>:224</c> <c>EnumDisplayDevices(nil, cc, lpDisplayDevice, dwFlags)</c>。
/// <para>接缝：待显示设备枚举（Windows.pas 的 user32 调用）移植后接入。</para>
/// </summary>
public interface IDisplayApi
{
    /// <summary>
    /// 原文 <c>EnumDisplaySettings(nil, Cardinal(-1), DeviceMode)</c>（:211）。
    /// <para><paramref name="iModeNum"/> = <c>Cardinal(-1)</c> = $FFFFFFFF = <c>ENUM_CURRENT_SETTINGS</c>。</para>
    /// </summary>
    bool EnumDisplaySettings(uint iModeNum, ref TDeviceMode lpDevMode);

    /// <summary>
    /// 原文 <c>EnumDisplayDevices(nil, cc, lpDisplayDevice, dwFlags)</c>（:224）。
    /// <para><paramref name="dwDevNum"/> = 原文 <c>cc</c>（0,1,2,…）；返回 false 表示枚举结束。</para>
    /// <para>★ 接缝实现若**恒返回 true** 会让原文 <c>while</c>（:224）死循环 —— 与本移植同样是缺陷。</para>
    /// </summary>
    bool EnumDisplayDevices(uint dwDevNum, ref TDisplayDevice lpDisplayDevice, uint dwFlags);
}

/// <summary>
/// 原文 <c>HardInfo.pas:253-284</c> <c>procedure GetCPUID(Param: Cardinal; var Registers: TRegisters);</c>
/// （<c>asm ... DB $0F, $A2 ...</c> 内联机器码，x86/x64 两套分支）。
/// <para>接缝：待 x86/x64 CPUID 指令层（或 <c>System.Runtime.Intrinsics.X86.X86Base.CpuId</c>）接入后实现。</para>
/// <para>★ 原文缺陷：该 asm **从未把 <paramref name="Param"/> 装入 EAX**（:254-284 全程没有 <c>MOV EAX, Param</c>），
/// 且先 <c>XOR EBX/ECX/EDX</c> ⇒ 叶号实际是**调用时 EAX 的残留值**，ECX 恒为 0。
/// 托管接缝保留 <paramref name="Param"/> 形参（签名 1:1）；真实实现须自行裁定是否照抄该缺陷。</para>
/// </summary>
public interface ICpuIdApi
{
    /// <summary>原文 <c>GetCPUID</c>（<c>procedure</c>，无返回值；结果由 <paramref name="Registers"/> 带出）。</summary>
    void GetCPUID(uint Param, out TRegisters Registers);
}

/// <summary>原文 <c>HardInfo.pas:293</c> <c>GlobalMemoryStatusEx(oMemoinfo)</c>。</summary>
public interface IMemoryApi
{
    /// <summary>原文 <c>GlobalMemoryStatusEx(oMemoinfo)</c>（原文 :293 **忽略返回值**）。</summary>
    bool GlobalMemoryStatusEx(ref TMemoryStatusEx buffer);
}

/// <summary>
/// 原文 <c>HardInfo.pas:344/:391</c> 的 <c>GetLastError</c> / <c>GetSystemInfo</c> 与
/// <c>:342/:348</c> 的 <c>GetLogicalProcessorInformation</c>。
/// <para>接缝：待 kernel32 处理器信息层移植后接入。</para>
/// </summary>
public interface IProcessorInformationApi
{
    /// <summary>
    /// 原文 <c>GetLogicalProcessorInformation(@Buffer[0], ReturnLength)</c>（:342 与 :348 两次调用）。
    /// <para><paramref name="returnLength"/> 为 <c>var</c>：原文初值 <c>sizeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION)</c>（24），
    /// 失败时由 API 写入所需字节数。</para>
    /// <para>★ 接缝实现必须用 <see cref="HardInfoLayout.SizeOfSystemLogicalProcessorInformation"/>（24）
    /// 做"元素数 ↔ 字节数"换算，否则原文 :346/:360 的 <c>div sizeof</c> 会算错。</para>
    /// </summary>
    bool GetLogicalProcessorInformation(SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] buffer, ref uint returnLength);

    /// <summary>原文 <c>GetLastError</c>（:344）。</summary>
    uint GetLastError();

    /// <summary>原文 <c>GetSystemInfo(SysInfo)</c>（:391）。</summary>
    void GetSystemInfo(out TSystemInfo lpSystemInfo);
}

/// <summary>
/// 原文 <c>HardInfo.pas:145-152 / 173-174 / 506-540</c> 的 CreateFile / DeviceIoControl / CloseHandle，
/// 以及 <c>SysUtils.Win32Platform</c>（:145、:506）。
/// <para>
/// ★ 接缝粒度：**请求结构（SCIP / SRB_IO_CONTROL / Buffer）的构造留在移植单元内**（属于原文逻辑，可测），
/// 接缝只承担三个 CreateFile+DeviceIoControl+CloseHandle 调用本身。
/// 原文用**同一条 S.M.A.R.T. IOCTL**但从三个设备读 IDENTIFY 数据，缓冲起点与 IDENTIFY 扇区偏移各不相同：
/// <list type="bullet">
/// <item><c>GetIdeSerialNumber</c> NT 分支：<c>'\\.\PhysicalDrive0'</c> + <c>DFP_RECEIVE_DRIVE_DATA($0007C088)</c>，
///       输出缓冲 = <c>aIdOutCmd</c>（527 字节），IDENTIFY 在 +16。</item>
/// <item><c>GetIdeSerialNumber</c> Win9x 分支：<c>'\\.\SMARTVSD'</c> + 同一 IOCTL，同样 +16。</item>
/// <item><c>GetIdeDiskSerialNumber</c> NT 分支：<c>'\\.\Scsi0:'</c> + <c>IOCTL_SCSI_MINIPORT($0004D008)</c>
///       且 <c>ControlCode := IOCTL_SCSI_MINIPORT_IDENTIFY($001B0501)</c>，输出缓冲 = <c>Buffer</c>（572 字节），IDENTIFY 在 +44。</item>
/// <item><c>GetIdeDiskSerialNumber</c> Win9x 分支：同 <c>SmartVsd</c>，但 IDENTIFY 在 +48（原文缺陷，见报告）。</item>
/// </list>
/// </para>
/// <para>接缝：待真实 S.M.A.R.T./DeviceIoControl 层移植后接入。</para>
/// </summary>
public interface IDiskIdentifyApi
{
    /// <summary>
    /// 原文 <c>SysUtils.Win32Platform</c>（:145、:506 判据 <c>VER_PLATFORM_WIN32_NT</c> = 2）。
    /// <para>默认实现返回 <c>VER_PLATFORM_WIN32_NT</c>（现代 Windows 恒为真，与原文运行期一致）。</para>
    /// </summary>
    uint Win32Platform { get; }

    /// <summary>
    /// 原文 <c>HardInfo.pas:148-149 + 173-174</c>：
    /// <c>CreateFile('\\.\PhysicalDrive0', GENERIC_READ|GENERIC_WRITE, FILE_SHARE_READ|FILE_SHARE_WRITE, nil, OPEN_EXISTING, 0, 0)</c>
    /// 后 <c>DeviceIoControl(hDevice, $0007C088, @SCIP, SizeOf(TSendCmdInParams)-1, @aIdOutCmd + outOffset, outSize, cbBytesReturned, nil)</c>。
    /// <para>原文取值：<c>sendCmdInSize = 32</c>、<c>outOffset = 0</c>、<c>outSize = 527</c>。
    /// 设备名固定为 <c>'\\.\PhysicalDrive0'</c>（原文 :148）。</para>
    /// </summary>
    /// <returns>false 复刻原文 <c>hDevice = INVALID_HANDLE_VALUE</c>（:153）或 <c>DeviceIoControl</c> 失败（:174）的 <c>Exit</c>。</returns>
    bool PhysicalDriveIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] idOutCmd, int outOffset, int outSize, out uint bytesReturned);

    /// <summary>
    /// 原文 <c>HardInfo.pas:509-512 + 538-540</c>：<c>CreateFile('\\.\Scsi0:', ...)</c> +
    /// <c>DeviceIoControl(hDevice, IOCTL_SCSI_MINIPORT, @Buffer, BufferSize, @Buffer, BufferSize, cbBytesReturned, nil)</c>。
    /// <para><paramref name="buffer"/> 已由调用方按原文 <c>:505-537</c> 填好（<c>TSrbIoControl</c> 头
    /// <c>Signature := 'SCSIDISK'</c>、<c>Timeout := 2</c>、<c>Length := 544</c>、<c>ControlCode := $001B0501</c>，
    /// 以及位于 <c>+28</c> 的 <c>TSendCmdInParams</c>）；<paramref name="bufferSize"/> = 572。</para>
    /// </summary>
    bool ScsiMiniportIoControl(byte[] buffer, int bufferSize, out uint bytesReturned);

    /// <summary>
    /// 原文 <c>'\\.\SMARTVSD'</c> + <c>DFP_RECEIVE_DRIVE_DATA</c> 的 Win9x 路径
    /// （<c>GetIdeSerialNumber :152 + :173-174</c>、<c>GetIdeDiskSerialNumber :547-548 + :568-570</c>）。
    /// <para>原文取值：<c>sendCmdInSize = 32</c>；
    /// <c>GetIdeSerialNumber</c> 用 <c>outOffset = 0 / outSize = 527</c>，
    /// <c>GetIdeDiskSerialNumber</c> 用 <c>outOffset = 32（= @pInData^.bBuffer）/ outSize = 528（W9xBufferSize）</c>。</para>
    /// </summary>
    bool SmartVsdIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] buffer, int outOffset, int outSize, out uint bytesReturned);
}

/// <summary>
/// 原文 <c>HardInfo.pas:678/:681/:686/:695</c> <c>NetBios(@Ncb)</c>（uses <c>Nb30</c>，<c>function NetBios(pncb: PNCB): Char; stdcall;</c>）。
/// <para>
/// ★ 托管差异（必读）：原文把数据缓冲地址写在 NCB 里（<c>Ncb.ncb_buffer := @Lanaenum</c> :679、
/// <c>:= @Adapter</c> :693）。托管侧无法把托管数组地址写进结构并保证语义 ⇒ 接缝**额外**传
/// <paramref name="ncbBuffer"/>（即 <c>ncb_buffer</c> 所指的数组，可能为 null）。
/// 本移植仍会把真实地址写进 <c>ncb_buffer</c>（用 <c>fixed</c> 钉住数组），故两个通道都可用：
/// 托管实现读 <paramref name="ncbBuffer"/>，原生实现读 <c>ncb.ncb_buffer</c>。
/// </para>
/// <para>接缝：待真实 NetBIOS（Nb30.pas / netapi32）移植后接入。</para>
/// </summary>
public interface INetBiosApi
{
    /// <summary>原文 <c>NetBios(pncb: PNCB): Char</c>；返回 0 = NRC_GOODRET（成功）。</summary>
    byte NetBios(ref TNCB ncb, byte[] ncbBuffer);
}

/// <summary>
/// 原文 <c>HardInfo.pas:792-799</c>：<c>CreateFile('\\.\' + NetCardName, GENERIC_READ|GENERIC_WRITE,
/// FILE_SHARE_READ|FILE_SHARE_WRITE, nil, OPEN_EXISTING, 0, 0)</c> +
/// <c>DeviceIoControl(hDevice, IOCTL_NDIS_QUERY_GLOBAL_STATS($00170002), @inBuf, 4, @outBuf, 256, BytesReturned, nil)</c>。
/// <para><paramref name="oid"/> = 原文 <c>inBuf</c> 的 4 字节输入内容（<c>OID_802_3_PERMANENT_ADDRESS = $01010101</c>，小端 4 字节）。</para>
/// <para>接缝：待真实 NDIS 查询层移植后接入。</para>
/// </summary>
public interface INdisQueryApi
{
    /// <summary><paramref name="outBuffer"/> 长度恒为 256（原文 <c>outBuf: array[1..256] of Byte</c>）。</summary>
    bool DeviceIoControlNdis(string deviceName, int ioControlCode, int oid, byte[] outBuffer, out uint bytesReturned);
}

/// <summary>
/// 原文 <c>HardInfo.pas:719-745</c>（裸 Win32 注册表 API）与 <c>:752-772</c>（<c>Registry.TRegistry</c>）。
/// <para>接缝：待真实注册表层（Registry.pas / Microsoft.Win32.Registry）移植后接入。</para>
/// </summary>
public interface IRegistryApi
{
    /// <summary>原文 <c>RegOpenKeyEx(RootKey, PChar(Name), 0, KEY_READ, hTemp) = ERROR_SUCCESS</c>（:719）。</summary>
    bool RegOpenKeyEx(nint rootKey, string name, uint samDesired, out nint hKey);

    /// <summary>
    /// 原文 <c>RegEnumKeyEx(hTemp, i, Buf, BufSize, nil, nil, nil, nil)</c>（:729）。
    /// <para><paramref name="nameLength"/> 为 <c>var</c>：入参 = 缓冲字节容量（原文恒 1024），
    /// 出参 = 名称长度（**不含**结尾 NUL）。</para>
    /// </summary>
    uint RegEnumKeyEx(nint hKey, int index, byte[] nameBuffer, ref uint nameLength);

    /// <summary>原文 <c>RegEnumValue(hTemp, i, Buf, BufSize, nil, nil, nil, nil)</c>（:731）。语义同上。</summary>
    uint RegEnumValue(nint hKey, int index, byte[] nameBuffer, ref uint nameLength);

    /// <summary>原文 <c>RegCloseKey(hTemp)</c>（:745）。</summary>
    void RegCloseKey(nint hKey);

    /// <summary>原文 <c>Reg := TRegistry.Create;</c>（:760，Delphi 的无参虚构造 ⇒ 默认 <c>FAccess = KEY_READ</c>）。</summary>
    nint RegistryCreate();

    /// <summary>原文 <c>Reg.RootKey := RootKey;</c>（:761，<c>RootKey = HKEY_LOCAL_MACHINE</c>）。</summary>
    void RegistrySetRootKey(nint registry, nint rootKey);

    /// <summary>原文 <c>Reg.OpenKey(NetKey+netList.Strings[i], False)</c>（:764）。</summary>
    bool RegistryOpenKey(nint registry, string key, bool canCreate);

    /// <summary>原文 <c>Reg.ReadString('ServiceName')</c>（:766）。</summary>
    string RegistryReadString(nint registry, string name);

    /// <summary>原文 <c>Reg.CloseKey;</c>（:767）。</summary>
    void RegistryCloseKey(nint registry);

    /// <summary>原文 <c>Reg.Free;</c>（:771）。</summary>
    void RegistryFree(nint registry);
}

/// <summary>
/// 本移植的全部外部依赖打包（对应原文散落的 <c>uses</c> 单元级 OS 调用）。
/// <para>与 <c>GXX.Core.Async.TAsyncCallRuntime</c> 同一模式：构造注入 + <see cref="Default"/> 空实现。</para>
/// </summary>
public sealed class THardInfoRuntime
{
    public THardInfoRuntime(
        IVersionApi version,
        IDisplayApi display,
        ICpuIdApi cpuId,
        IMemoryApi memory,
        IProcessorInformationApi processor,
        IDiskIdentifyApi disk,
        INetBiosApi netBios,
        INdisQueryApi ndis,
        IRegistryApi registry)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Display = display ?? throw new ArgumentNullException(nameof(display));
        CpuId = cpuId ?? throw new ArgumentNullException(nameof(cpuId));
        Memory = memory ?? throw new ArgumentNullException(nameof(memory));
        Processor = processor ?? throw new ArgumentNullException(nameof(processor));
        Disk = disk ?? throw new ArgumentNullException(nameof(disk));
        NetBios = netBios ?? throw new ArgumentNullException(nameof(netBios));
        Ndis = ndis ?? throw new ArgumentNullException(nameof(ndis));
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public IVersionApi Version { get; }
    public IDisplayApi Display { get; }
    public ICpuIdApi CpuId { get; }
    public IMemoryApi Memory { get; }
    public IProcessorInformationApi Processor { get; }
    public IDiskIdentifyApi Disk { get; }
    public INetBiosApi NetBios { get; }
    public INdisQueryApi Ndis { get; }
    public IRegistryApi Registry { get; }

    /// <summary>
    /// 默认运行时：全部接缝都是"失败/空"的空实现（<see cref="THardInfoNullOs"/>），
    /// **不做任何真实 OS 调用**（GXX.Core 不含 P/Invoke）。
    /// <para>因此默认情况下：<c>GetIdeSerialNumber()=""</c>、<c>GetIdeDiskSerialNumber(...)=false</c>（且不改写 ref 形参）、
    /// <c>GetDisplayFrequency()=0</c>、<c>GetDisplayDevice()=""</c>、<c>GetMemorySize()="0Bytes"</c>、
    /// <c>GetCpuName()=""</c>、<c>GetCpuIDstringEx()="0000"</c>、<c>GetCPUCount()=""</c>、
    /// <c>GetAdapterMac(0)=""</c>、<c>GetNetCardName()=""</c>、<c>GetNetCardMac(...)=""</c>、
    /// <c>GetWindowsVersion()="Windows 3.11, Build: 0"</c>（见各方法注释）。</para>
    /// </summary>
    public static THardInfoRuntime Default { get; } = new THardInfoRuntime(
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs(),
        new THardInfoNullOs());
}

/// <summary>
/// 全部接缝的"空/失败"默认实现。
/// <para>接缝：待各 OS 层（Windows.pas / Nb30.pas / Registry.pas / DeviceIoControl）移植后，
/// 由集成方按 docs/并行报告-p6-core-hardinfo.md 的接缝清单实现并注入。</para>
/// </summary>
public sealed class THardInfoNullOs :
    IVersionApi,
    IDisplayApi,
    ICpuIdApi,
    IMemoryApi,
    IProcessorInformationApi,
    IDiskIdentifyApi,
    INetBiosApi,
    INdisQueryApi,
    IRegistryApi
{
    // ---------------- IVersionApi ----------------
    public bool GetVersionEx(ref TOSVersionInfo versionInfo) => false;

    // ---------------- IDisplayApi ----------------
    public bool EnumDisplaySettings(uint iModeNum, ref TDeviceMode lpDevMode) => false;

    public bool EnumDisplayDevices(uint dwDevNum, ref TDisplayDevice lpDisplayDevice, uint dwFlags) => false;

    // ---------------- ICpuIdApi ----------------
    /// <summary>默认不执行 CPUID：四个寄存器归零（原文未执行的语义近似）。</summary>
    public void GetCPUID(uint Param, out TRegisters Registers) => Registers = default;

    // ---------------- IMemoryApi ----------------
    public bool GlobalMemoryStatusEx(ref TMemoryStatusEx buffer) => false;

    // ---------------- IProcessorInformationApi ----------------
    /// <summary>
    /// 默认复刻"缓冲不足"的失败：<c>ReturnLength</c> 保持调用方给的 24 不变，返回 false 且
    /// <see cref="GetLastError"/> = <c>ERROR_INSUFFICIENT_BUFFER</c>(122) ⇒ 原文 :346 会扩容到 2 个元素后再调一次，
    /// 再次失败 ⇒ :350 <c>Exit</c> ⇒ <c>GetCPUCount()</c> 返回 <c>""</c>。
    /// </summary>
    public bool GetLogicalProcessorInformation(SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] buffer, ref uint returnLength) => false;

    public uint GetLastError() => HardInfo.ERROR_INSUFFICIENT_BUFFER;

    public void GetSystemInfo(out TSystemInfo lpSystemInfo) => lpSystemInfo = default;

    // ---------------- IDiskIdentifyApi ----------------
    /// <summary>默认与原文运行期一致：现代 Windows 恒为 <c>VER_PLATFORM_WIN32_NT</c>(2)。</summary>
    public uint Win32Platform => HardInfo.VER_PLATFORM_WIN32_NT;

    public bool PhysicalDriveIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] idOutCmd, int outOffset, int outSize, out uint bytesReturned)
    {
        bytesReturned = 0;
        return false;
    }

    public bool ScsiMiniportIoControl(byte[] buffer, int bufferSize, out uint bytesReturned)
    {
        bytesReturned = 0;
        return false;
    }

    public bool SmartVsdIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] buffer, int outOffset, int outSize, out uint bytesReturned)
    {
        bytesReturned = 0;
        return false;
    }

    // ---------------- INetBiosApi ----------------
    /// <summary>默认返回非 0（NRC 失败码 0x01 = NRC_BUFLEN），复刻"没有网卡/枚举失败"。</summary>
    public byte NetBios(ref TNCB ncb, byte[] ncbBuffer) => 0x01;

    // ---------------- INdisQueryApi ----------------
    public bool DeviceIoControlNdis(string deviceName, int ioControlCode, int oid, byte[] outBuffer, out uint bytesReturned)
    {
        bytesReturned = 0;
        return false;
    }

    // ---------------- IRegistryApi ----------------
    public bool RegOpenKeyEx(nint rootKey, string name, uint samDesired, out nint hKey)
    {
        hKey = 0;
        return false;
    }

    public uint RegEnumKeyEx(nint hKey, int index, byte[] nameBuffer, ref uint nameLength) => HardInfo.ERROR_NO_MORE_ITEMS;

    public uint RegEnumValue(nint hKey, int index, byte[] nameBuffer, ref uint nameLength) => HardInfo.ERROR_NO_MORE_ITEMS;

    public void RegCloseKey(nint hKey) { }

    public nint RegistryCreate() => 0;

    public void RegistrySetRootKey(nint registry, nint rootKey) { }

    public bool RegistryOpenKey(nint registry, string key, bool canCreate) => false;

    public string RegistryReadString(nint registry, string name) => "";

    public void RegistryCloseKey(nint registry) { }

    public void RegistryFree(nint registry) { }
}

// ============================================================================
// 源单元：Source\Common\HardInfo.pas（GBK，898 行，Delphi 7 / Win32）
// 本文件：HardInfo.pas 的 1:1 移植（interface :11-40 的 12 个函数 + implementation 全部例程）。
//
// 行号对照（左 = 原文，右 = 本文件成员）：
//   :13-19     TRegisters / TCPUID            → HardInfoTypes.cs
//   :46-184    GetIdeSerialNumber             → GetIdeSerialNumber / ChangeByteOrder
//   :128-142   内嵌 ChangeByteOrder（第 1 份） → ChangeByteOrder
//   :207-213   GetDisplayFrequency            → GetDisplayFrequency
//   :215-231   GetDisplayDevice               → GetDisplayDevice / MatchesPrimaryDisplayName
//   :233-251   CountSetBits                   → CountSetBits
//   :253-284   GetCPUID（内联 asm）            → GetCPUID（接缝）
//   :287-303   GetMemorySize                  → GetMemorySize / FormatMemorySize
//   :305-324   GetCpuName                     → GetCpuName / AssembleCpuName
//   :327-382   GetCPUCount                    → GetCPUCount / FormatCPUCount
//   :387-394   GetCpuIDstringEx               → GetCpuIDstringEx / FormatCpuIDstringEx
//   :396-595   GetIdeDiskSerialNumber         → GetIdeDiskSerialNumber / DecodeIdSector
//   :487-501   内嵌 ChangeByteOrder（第 2 份） → ChangeByteOrder
//   :597-660   GetWindowsVersion              → GetWindowsVersion / FormatWindowsVersion
//   :664-705   GetAdapterMac                  → GetAdapterMac / FormatAdapterAddress6
//   :707-773   GetNetCardName（含内嵌 RegEnum）→ GetNetCardName / RegEnum / SplitTextStr
//   :775-813   GetNetCardMac                  → GetNetCardMac / FormatNdisAddressHex
//   :815-831   FormatMac                      → FormatMac
//   :833-897   GetHwid2 + initialization/finalization → **整段被原文注释**，见文件末保留块
//
// ★ 接缝：全部 OS 调用走 GXX.Core.Hardware 的接缝接口（HardInfoSeams.cs），
//   默认实现返回"失败/空"；真实实现由集成方注入（精确签名见 docs/并行报告-p6-core-hardinfo.md）。
// ★ Delphi 语义：格式化一律走 GXX.Core.Rtl.DelphiRTL / DelphiFormat，**禁用 string.Format**。
// ============================================================================
using System.Runtime.InteropServices;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.Core.Hardware;

/// <summary>
/// <c>HardInfo.pas</c> 的托管对应（静态类，对应 Delphi 单元级函数）。
/// <para>
/// ★ 原文并**不存在** <c>THardInfo</c> 类型 —— 该单元只有 <c>TRegisters</c>/<c>TCPUID</c> 两个记录与
/// 12 个单元级函数（派发提示词里的 "THardInfo" 是误称，已核实 `grep -E "THardInfo" Source/**/*.pas` 零命中）。
/// </para>
/// <para>每个方法的末位可选参数 <see cref="THardInfoRuntime"/> 是接缝注入口（原文无对应形参）。</para>
/// </summary>
public static unsafe class HardInfo
{
    // ---------------- 原文用到的常量（来自 Windows.pas / Nb30.pas / 本地 const 段） ----------------

    /// <summary>Windows.pas <c>VER_PLATFORM_WIN32_NT</c>（原文 :145、:506 的判据）。</summary>
    public const uint VER_PLATFORM_WIN32_NT = 2;

    /// <summary>Windows.pas <c>ERROR_SUCCESS</c>（原文 :719、:732 的判据）。</summary>
    public const uint ERROR_SUCCESS = 0;

    /// <summary>Windows.pas <c>ERROR_INSUFFICIENT_BUFFER</c>（原文 :344 的判据）。</summary>
    public const uint ERROR_INSUFFICIENT_BUFFER = 122;

    /// <summary>Windows.pas <c>ERROR_NO_MORE_ITEMS</c>（原文 :742 <c>until iRes &lt;&gt; ERROR_SUCCESS</c> 的正常终止码）。</summary>
    public const uint ERROR_NO_MORE_ITEMS = 259;

    /// <summary>Windows.pas <c>HKEY_LOCAL_MACHINE</c>（原文 :749 <c>RootKey = HKEY_LOCAL_MACHINE</c>）。</summary>
    public static readonly nint HKEY_LOCAL_MACHINE = unchecked((nint)0x80000002);

    /// <summary>Windows.pas <c>KEY_READ</c>（原文 :719）。</summary>
    public const uint KEY_READ = 0x20019;

    /// <summary>Nb30.pas <c>NCBENUM</c>（原文 :677 拼作 <c>NCbenum</c>）。</summary>
    public const byte NCBENUM = 0x37;

    /// <summary>Nb30.pas <c>NCBRESET</c>（原文 :684 拼作 <c>NcbReset</c>）。</summary>
    public const byte NCBRESET = 0x32;

    /// <summary>Nb30.pas <c>NCBASTAT</c>（原文 :690 拼作 <c>NcbAstat</c>）。</summary>
    public const byte NCBASTAT = 0x33;

    /// <summary>原文 :48 <c>IDENTIFY_BUFFER_SIZE = 512;</c>（GetIdeSerialNumber 内嵌 const）。</summary>
    public const int IDENTIFY_BUFFER_SIZE = 512;

    /// <summary>原文 :471 <c>IDE_ID_FUNCTION = $EC;</c></summary>
    public const byte IDE_ID_FUNCTION = 0xEC;

    /// <summary>原文 :473 <c>DFP_RECEIVE_DRIVE_DATA = $0007C088;</c></summary>
    public const uint DFP_RECEIVE_DRIVE_DATA = 0x0007C088;

    /// <summary>原文 :474 <c>IOCTL_SCSI_MINIPORT = $0004D008;</c></summary>
    public const uint IOCTL_SCSI_MINIPORT = 0x0004D008;

    /// <summary>原文 :475 <c>IOCTL_SCSI_MINIPORT_IDENTIFY = $001B0501;</c></summary>
    public const uint IOCTL_SCSI_MINIPORT_IDENTIFY = 0x001B0501;

    /// <summary>原文 :478 <c>W9xBufferSize = IDENTIFY_BUFFER_SIZE + 16;</c>（= 528）。</summary>
    public const int W9xBufferSize = IDENTIFY_BUFFER_SIZE + 16;

    /// <summary>原文 :476 <c>DataSize = sizeof(TSendCmdInParams) - 1 + IDENTIFY_BUFFER_SIZE;</c>（= 544）。</summary>
    public const int SCSI_DataSize = HardInfoLayout.SendCmdInParamsSize - 1 + IDENTIFY_BUFFER_SIZE;

    /// <summary>原文 :477 <c>BufferSize = sizeof(SRB_IO_CONTROL) + DataSize;</c>（= 572）。</summary>
    public const int SCSI_BufferSize = HardInfoLayout.SrbIoControlSize + SCSI_DataSize;

    /// <summary>原文 :125 <c>aIdOutCmd</c> 的元素个数（<c>array[0..(SizeOf(TSendCmdOutParams)+512-1)-1]</c> = 527）。</summary>
    public const int IDEOUTCMD_SIZE = (HardInfoLayout.SendCmdOutParamsSize + IDENTIFY_BUFFER_SIZE - 1) - 1 + 1;

    // =========================================================================================
    // GetIdeSerialNumber —— 原文 :46-184
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:46-184</c>：<c>function GetIdeSerialNumber: pchar;</c> —— 取第一个 IDE 硬盘的序列号。
    /// <para>
    /// ★ 返回值差异：原文返回 <c>PChar</c>（指向**单元级全局</b> <c>aIdOutCmd</c> 内的 <c>sSerialNumber</c>）；
    /// 全部调用点（MShare.pas:3554 的 <c>GetIdeSerialNumber + GetWindowsVersion</c>）触发 PChar→string 隐式转换
    /// ⇒ 语义 = "取到第一个 #0 为止"。托管侧直接返回该 string。
    /// </para>
    /// <para>
    /// ★ 原文 :181 的 <c>(PChar(@sSerialNumber) + SizeOf(sSerialNumber))^ := #0</c> 是**越界写**：
    /// <c>sSerialNumber</c> 只有 20 字节（:80），写出界正好落在 <c>wBufferType</c> 的低字节上。
    /// 本移植按字节等价照做（同一原始缓冲内的 +40 处），并登记为原文缺陷。
    /// </para>
    /// <para>★ 原文 <c>:120-126</c> 的 <c>hDevice/cbBytesReturned/SCIP/aIdOutCmd/IdOutCmd</c> 是**单元级 var**
    /// （静态存储、跨调用保留）；每次调用都被重新赋值/FillChar（唯一例外 <c>SCIP.bBuffer[0]</c> 那 1 字节，
    /// 因 :155 只清 <c>SizeOf(...)-1</c> = 32 字节），故托管侧用局部变量等价。</para>
    /// </summary>
    public static string GetIdeSerialNumber(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;

        string Result = ""; // 如果出错则返回空串 —— 原文 :144

        // var SCIP: TSendCmdInParams;  （原文 :124，单元级）
        var SCIP = new TSendCmdInParams();
        // FillChar(SCIP, SizeOf(TSendCmdInParams) - 1, #0);  —— 原文 :155
        //   ★ 只清 32 字节（SizeOf = 33），bBuffer[0] 未清 —— 原文如此（HardInfo.pas:155）
        //   托管侧 SCIP 为 default 局部结构（全 0），与原文"全局静态初值 0 + 每次只清 32 字节"结果一致。
        // var aIdOutCmd: array[0..(SizeOf(TSendCmdOutParams) + IDENTIFY_BUFFER_SIZE - 1) - 1] of Byte;  —— :125
        var aIdOutCmd = new byte[IDEOUTCMD_SIZE];
        // FillChar(aIdOutCmd, SizeOf(aIdOutCmd), #0);  —— 原文 :156
        uint cbBytesReturned = 0; // 原文 :157

        // Set up data structures for IDENTIFY command.  —— 原文 :159-172
        SCIP.cBufferSize = IDENTIFY_BUFFER_SIZE;
        // bDriveNumber := 0;  —— 原文 :162 该行被注释掉；bDriveNumber 由 FillChar 保持 0
        SCIP.irDriveRegs.bSectorCountReg = 1;
        SCIP.irDriveRegs.bSectorNumberReg = 1;
        // 原文 :167-168 两行被注释掉：
        //   // if Win32Platform=VER_PLATFORM_WIN32_NT then bDriveHeadReg := $A0
        //   // else bDriveHeadReg := $A0 or ((bDriveNum and 1) shl 4);
        SCIP.irDriveRegs.bDriveHeadReg = 0xA0;
        SCIP.irDriveRegs.bCommandReg = 0xEC;

        bool ioOk;
        if (runtime.Disk.Win32Platform == VER_PLATFORM_WIN32_NT)
        {
            // Windows NT, Windows 2000 —— 原文 :145-150 + :173-174
            // 提示! 改变名称可适用于其它驱动器，如第二个驱动器： '\\.\PhysicalDrive1\'
            ioOk = runtime.Disk.PhysicalDriveIoControl(
                SCIP, HardInfoLayout.SendCmdInParamsSize - 1, aIdOutCmd, 0, IDEOUTCMD_SIZE, out cbBytesReturned);
        }
        else
        {
            // Version Windows 95 OSR2, Windows 98 —— 原文 :151-152 + :173-174
            ioOk = runtime.Disk.SmartVsdIoControl(
                SCIP, HardInfoLayout.SendCmdInParamsSize - 1, aIdOutCmd, 0, IDEOUTCMD_SIZE, out cbBytesReturned);
        }

        if (!ioOk) return Result; // 原文 :153 `if hDevice = INVALID_HANDLE_VALUE then Exit;` / :174 `then Exit`

        // with PIdSector(@IdOutCmd.bBuffer)^ do   —— 原文 :178
        int b = HardInfoLayout.IdeSerialNumberIdentifyOffset;
        EnsureIdentifyBuffer(aIdOutCmd, b, nameof(GetIdeSerialNumber));
        ChangeByteOrder(aIdOutCmd, b + HardInfoLayout.IdSector_sSerialNumber, HardInfoLayout.IdSector_sSerialNumberLength); // :180
        aIdOutCmd[b + HardInfoLayout.IdSector_wBufferType] = 0;                                                              // :181（越界写！）
        return DelphiRTL.StrPas(aIdOutCmd, b + HardInfoLayout.IdSector_sSerialNumber);                                       // :182
    }

    // =========================================================================================
    // ChangeByteOrder —— 原文 :128-142（GetIdeSerialNumber 内嵌）与 :487-501（GetIdeDiskSerialNumber 内嵌）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:128-142</c> 与 <c>:487-501</c> 的**两份完全相同**的内嵌例程：
    /// <code>
    /// procedure ChangeByteOrder(var Data; Size: Integer);
    /// begin
    ///   ptr := @Data;
    ///   for i := 0 to (Size shr 1) - 1 do
    ///   begin c := ptr^; ptr^ := (ptr + 1)^; (ptr + 1)^ := c; Inc(ptr, 2); end;
    /// end;
    /// </code>
    /// <para>逐字节两两交换（ATA IDENTIFY 字段的字序 → 主机字节序）。</para>
    /// <para>★ 原文如此：<c>var Data</c> 是无类型形参、<c>Data</c> 为 0-based，故托管签名加 <paramref name="offset"/>；
    /// 无 <c>offset</c> 的重载等价于 <c>offset = 0</c>（对应 :180 那类"从字段起点开始"的调用）。</para>
    /// <para>★ 边界：<c>Size</c> 为奇数时**最后一字节不参与交换**（<c>Size shr 1</c> 向下取整）；
    /// <c>Size</c> 为负时 Delphi 的 <c>shr</c> 是**逻辑右移** ⇒ 迭代次数 = 2^31-1，原文会越权读写到 AV；
    /// 托管侧在数组边界处抛 <see cref="IndexOutOfRangeException"/>（差异登记，见 <see cref="ChangeByteOrderLoopCount"/>）。</para>
    /// </summary>
    public static void ChangeByteOrder(byte[] Data, int offset, int Size)
    {
        long loops = ChangeByteOrderLoopCount(Size); // for i := 0 to (Size shr 1) - 1
        for (long i = 0; i < loops; i++)
        {
            long p = (long)offset + i * 2; // Inc(ptr, 2)
            byte c = Data[p];              // c := ptr^;
            Data[p] = Data[p + 1];         // ptr^ := (ptr + 1)^;
            Data[p + 1] = c;               // (ptr + 1)^ := c;
        }
    }

    /// <summary>原文 <c>ChangeByteOrder(Data, Size)</c>（<c>Data</c> 起点的重载，等价 <c>offset = 0</c>）。</summary>
    public static void ChangeByteOrder(byte[] Data, int Size) => ChangeByteOrder(Data, 0, Size);

    /// <summary>
    /// 原文 <c>for i := 0 to (Size shr 1) - 1</c> 的迭代次数（Delphi <c>shr</c> = **逻辑**右移）。
    /// <para>Size = 0 → 0；1 → 0；2 → 1；3 → 1；4 → 2；-1 → 2147483647；int.MinValue → 1073741824。</para>
    /// </summary>
    public static long ChangeByteOrderLoopCount(int Size) => (uint)Size >> 1;

    // =========================================================================================
    // GetDisplayFrequency —— 原文 :207-213
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:207-213</c>：<c>EnumDisplaySettings(nil, Cardinal(-1), DeviceMode); Result := DeviceMode.dmDisplayFrequency;</c>
    /// <para>★ 原文缺陷：<c>EnumDisplaySettings</c> 的 **Boolean 返回值被丢弃**（:211），
    /// 且 <c>DeviceMode</c> 是**未初始化**的局部变量 ⇒ 调用失败时返回的是栈上垃圾。
    /// 托管侧 <see cref="TDeviceMode"/> 为值类型（default 全 0），失败时返回 0（差异登记）。</para>
    /// <para>原文注释（:205-206）："这个函数返回的显示刷新率是以Hz为单位的"。</para>
    /// </summary>
    public static int GetDisplayFrequency(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        var DeviceMode = new TDeviceMode();
        runtime.Display.EnumDisplaySettings(uint.MaxValue, ref DeviceMode); // Cardinal(-1) = $FFFFFFFF = ENUM_CURRENT_SETTINGS
        return (int)DeviceMode.dmDisplayFrequency;
    }

    // =========================================================================================
    // GetDisplayDevice —— 原文 :215-231
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:215-231</c>：枚举显示设备，取名为 <c>'\\.\Display1'</c> 或 <c>'\\.\DISPLAY1'</c> 的
    /// <c>DeviceString</c>。
    /// <para>★ <c>lpDisplayDevice.cb</c> 只在**循环外**设置一次（:221）；<c>cc</c> 在名称判定**之前**自增（:226）；
    /// 未匹配到任何设备时 <c>Result</c> 保持 Delphi 字符串函数结果的初值 <c>''</c>。</para>
    /// <para>★ 原文缺陷：接缝/API 若恒返回 true ⇒ <c>while</c> 死循环（原文与托管侧同）。</para>
    /// </summary>
    public static string GetDisplayDevice(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        var lpDisplayDevice = new TDisplayDevice();
        lpDisplayDevice.cb = (uint)TDisplayDevice.Size; // sizeof(lpDisplayDevice) —— 原文 :221
        uint dwFlags = 0;                                // 原文 :222
        uint cc = 0;                                     // 原文 :223
        string Result = null;                            // Delphi 字符串函数结果初值 ''
        while (runtime.Display.EnumDisplayDevices(cc, ref lpDisplayDevice, dwFlags)) // 原文 :224
        {
            cc++; // 原文 :226
            if (MatchesPrimaryDisplayName(lpDisplayDevice.DeviceNameString()))       // 原文 :227
                Result = lpDisplayDevice.DeviceStringString();                        // 原文 :228
            // ListBox1.Items.Add(lpDisplayDevice.DeviceString); {there is also additional information in lpDisplayDevice}
            //   —— 原文 :229，被注释掉的 UI 语句
        }
        return Result ?? "";
    }

    /// <summary>
    /// 原文 <c>:227</c> 的判据：<c>(DeviceName = '\\.\Display1') or (DeviceName = '\\.\DISPLAY1')</c>。
    /// <para>★ Delphi 的字符串 <c>=</c> 是**逐字节大小写敏感**比较（不是 AnsiCompareText）⇒
    /// <c>'\\.\display1'</c>、<c>'\\.\DISPLAY2'</c>、<c>'\\.\Display1 '</c> 均**不匹配**。</para>
    /// </summary>
    public static bool MatchesPrimaryDisplayName(string DeviceName)
        => DeviceName == "\\\\.\\Display1" || DeviceName == "\\\\.\\DISPLAY1";

    // =========================================================================================
    // CountSetBits —— 原文 :233-251
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:233-251</c>：
    /// <code>
    /// LSHIFT := sizeof(Cardinal) * 8 - 1;   // = 31
    /// bitSetCount := 0;  bitTest := 1 shl LSHIFT;
    /// for I := 0 to LSHIFT - 1 do
    /// begin
    ///   bitSetCount := Ifthen((bitMask and bitTest) = 0, 1, 0);   // ★ 赋值，不是累加！
    ///   bitTest := bitTest div 2;
    /// end;
    /// Result := bitSetCount;
    /// </code>
    /// <para>
    /// ★★ 原文缺陷（不是 popcount）：<c>bitSetCount :=</c> 每轮**覆盖**（Delphi 的 <c>Ifthen</c> 返回 1/0），
    /// 循环只跑 <c>I = 0..30</c>（<c>LSHIFT - 1</c> 是**上界**而非长度），因此结果 = 最后一轮的判定值。
    /// 逐轮推导（<c>bitTest</c> 为 <c>Uint64</c>，<c>1 shl 31</c> 以 32 位求值 = <c>Integer($80000000)</c>，
    /// 赋给 64 位时按 0xFFFFFFFF80000000 扩展；无论该扩展是有符号还是无符号，
    /// 与 32 位 <c>bitMask</c> 相与的结果只取决于低 32 位 = <c>$FFFFFFFF shl (31-k)</c>）：
    /// 第 k 轮（k = 0..30）的掩码 = bit[31-k .. 31]，末轮 k = 30 ⇒ 掩码 = <c>$FFFFFFFE</c>。
    /// </para>
    /// <para>⇒ <b>CountSetBits(x) = 1 当且仅当 x ∈ {0, 1}，否则 0</b>（与"数出置位数"毫无关系；见差异断言）。</para>
    /// </summary>
    public static uint CountSetBits(uint bitMask)
    {
        uint LSHIFT = sizeof(uint) * 8 - 1;                       // 31
        uint bitSetCount = 0;
        // `bitTest: Uint64` ← `1 shl LSHIFT`：Delphi 以 32 位求值 1 shl 31 = Integer($80000000)，再扩到 64 位
        ulong bitTest = unchecked((ulong)(long)(int)(1u << 31));  // = 0xFFFFFFFF80000000

        for (uint I = 0; I <= LSHIFT - 1; I++)                    // 原文 `for I := 0 to LSHIFT - 1` ⇒ 31 轮（0..30）
        {
            // 原文 `bitSetCount := Ifthen((bitMask and bitTest) = 0, 1, 0);`（Math.IfThen(Integer,Integer)）
            bitSetCount = (bitMask & bitTest) == 0 ? 1u : 0u;
            bitTest = bitTest / 2;                                // 原文 `bitTest := bitTest div 2;`
        }

        return bitSetCount;
    }

    // =========================================================================================
    // GetCPUID —— 原文 :253-284（{$IFDEF M2SERVER} 包裹）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:253-284</c> <c>procedure GetCPUID(Param: Cardinal; var Registers: TRegisters);</c>
    /// （<c>asm ... DB $0F, $A2 ...</c>）。
    /// <para>★ 原文缺陷：该 asm **从未把 <paramref name="Param"/> 装入 EAX**（:256-268 / :270-282 全程没有
    /// <c>MOV EAX, Param</c>），且先 <c>XOR EBX/ECX/EDX</c> ⇒ 实参叶号被完全忽略、ECX 恒 0，
    /// 实际叶号是调用时 EAX 的残留值。<b>GetCpuName 因此永远不可能稳定工作。</b></para>
    /// <para>托管侧保留 <paramref name="Param"/> 形参（签名 1:1），由接缝决定是否照抄该缺陷。</para>
    /// <para>原文用 <c>{$IFDEF M2SERVER}</c> 包裹；托管侧无条件提供（差异登记）。</para>
    /// </summary>
    public static void GetCPUID(uint Param, out TRegisters Registers, THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        runtime.CpuId.GetCPUID(Param, out Registers);
    }

    // =========================================================================================
    // GetMemorySize —— 原文 :287-303（{$IFDEF M2SERVER} 包裹）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:287-303</c>：<c>GlobalMemoryStatusEx</c> 后按 1GB/1MB/1KB 分档。
    /// <para>★ 原文缺陷：<c>GlobalMemoryStatusEx</c> 的返回值被丢弃（:293），<c>oMemoinfo</c> 是未初始化局部
    /// （只设了 <c>dwLength</c>）⇒ 调用失败时读到栈上垃圾。托管侧 default 结构为 0 ⇒ 返回 <c>'0Bytes'</c>。</para>
    /// <para>★ <c>cSize := oMemoinfo.ullTotalPhys</c> 是 <c>UInt64 → Int64</c> 的**位保留**赋值（无范围检查）。</para>
    /// </summary>
    public static string GetMemorySize(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        var oMemoinfo = new TMemoryStatusEx();
        oMemoinfo.dwLength = (uint)TMemoryStatusEx.Size; // SizeOf(TMemoryStatusEx) —— 原文 :292
        runtime.Memory.GlobalMemoryStatusEx(ref oMemoinfo);
        long cSize = unchecked((long)oMemoinfo.ullTotalPhys); // 原文 :294（UInt64 → Int64）
        return FormatMemorySize(cSize);
    }

    /// <summary>
    /// 原文 <c>:295-302</c> 的数值→可读字符串分支（提取的纯逻辑，原文无独立名称）。
    /// <para>★ 判据是**严格大于**：恰好 1GB → 走 MB 档得 <c>'1024MB'</c>；恰好 1MB → <c>'1024KB'</c>；
    /// 恰好 1KB → <c>'1024Bytes'</c>。</para>
    /// <para>★ <c>FloatToStr(Round(cSize / (1024*1024*1024)))</c>：Delphi <c>Round</c> 是**四舍六入五取偶**
    /// （banker's rounding）⇒ 1.5GB → <c>'2GB'</c>、2.5GB → <c>'2GB'</c>（半值上取会得 3GB）。
    /// <c>FloatToStr(整数)</c> 等价于十进制整数串（值域 ≤ 2^63/2^30 ≈ 8.6e9，远小于 FloatToStr 的科学计数门槛）。</para>
    /// </summary>
    public static string FormatMemorySize(long cSize)
    {
        if (cSize > 1024 * 1024 * 1024)
            return DelphiRTL.IntToStr(DelphiRound(cSize / (1024.0 * 1024 * 1024))) + "GB";
        if (cSize > 1024 * 1024)
            return DelphiRTL.IntToStr(DelphiRound(cSize / (1024.0 * 1024))) + "MB";
        if (cSize > 1024)
            return DelphiRTL.IntToStr(DelphiRound(cSize / 1024.0)) + "KB";
        return DelphiRTL.IntToStr(cSize) + "Bytes";
    }

    /// <summary>Delphi <c>Round(Extended): Int64</c> —— 半值取偶（banker's rounding），与 <see cref="Math.Round(double)"/> 一致。</summary>
    private static long DelphiRound(double value) => (long)Math.Round(value, MidpointRounding.ToEven);

    // =========================================================================================
    // GetCpuName —— 原文 :305-324（{$IFDEF M2SERVER} 包裹）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:305-324</c>：对叶 <c>$80000002/$80000003/$80000004</c> 取 CPU 品牌串。
    /// <para>★ 原文 :314 <c>TTT := 1 shl 31 + III;</c> —— Delphi 的 <c>shl</c> 属**乘性**优先级组，
    /// **高于** 加性 <c>+</c> ⇒ 等价于 <c>(1 shl 31) + III</c> = <c>$80000002/$80000003/$80000004</c>
    /// （若误读为 <c>1 shl (31 + III)</c> 会得到完全不同的叶号）。</para>
    /// <para>★ 原文缺陷：<c>GetCPUID</c> 忽略叶号（见 <see cref="GetCPUID"/>）⇒ 本函数在原文里拿不到品牌串。</para>
    /// </summary>
    public static string GetCpuName(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        var regs = new TRegisters[3]; // 原文 `regs: TRegisters` 在循环内复用
        for (int III = 2; III <= 4; III++)
        {
            uint TTT = unchecked((uint)((int)(1u << 31) + III)); // (1 shl 31) + III
            runtime.CpuId.GetCPUID(TTT, out regs[III - 2]);
        }
        return AssembleCpuName(regs);
    }

    /// <summary>
    /// 原文 <c>:317-323</c> 的寄存器→字符串拼装（提取的纯逻辑，原文无独立名称）。
    /// <para><c>Move(regs.EAX, processor_name[(III-2)*16 + 00], 4)</c> 是**小端原样**拷贝，**不做字节序变换**
    /// —— 与 <see cref="ChangeByteOrder"/> 的语义相反（差异断言锁定）。</para>
    /// <para>叶序：III=2 → 字节 0..15；III=3 → 16..31；III=4 → 32..47；字节 48 恒置 <c>#0</c>（:322）。</para>
    /// <para>★ <c>string(AnsiString(processor_name))</c> 按 **NUL 结尾 C 串**转换 ⇒ 中途出现 0 字节即截断。</para>
    /// </summary>
    public static string AssembleCpuName(TRegisters[] regs)
    {
        if (regs == null || regs.Length < 3)
            throw new ArgumentException(
                "AssembleCpuName 需要 3 组 CPUID 结果（叶 $80000002/$80000003/$80000004）", nameof(regs));
        var processor_name = new byte[49]; // array [0 .. 48] of AnsiChar —— 原文 :308
        for (int III = 2; III <= 4; III++)
        {
            var r = regs[III - 2];
            WriteLittleEndian(processor_name, (III - 2) * 16 + 00, r.EAX);
            WriteLittleEndian(processor_name, (III - 2) * 16 + 04, r.EBX);
            WriteLittleEndian(processor_name, (III - 2) * 16 + 08, r.ECX);
            WriteLittleEndian(processor_name, (III - 2) * 16 + 12, r.EDX);
        }
        processor_name[48] = 0;                    // 原文 :322
        return DelphiRTL.StrPas(processor_name);   // string(AnsiString(processor_name)) —— 原文 :323
    }

    private static void WriteLittleEndian(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value & 0xFF);
        buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
        buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
    }

    // =========================================================================================
    // GetCPUCount —— 原文 :327-382（{$IFDEF M2SERVER} 包裹）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:327-382</c>：两次 <c>GetLogicalProcessorInformation</c> 取缓冲，再统计各类关系。
    /// <para>★ 原文缺陷 1（:342-353）：第一次调用用 1 元素缓冲；**只有**失败且 <c>GetLastError = ERROR_INSUFFICIENT_BUFFER</c>
    /// 时才扩容重试；若第一次"恰好成功"，<c>ReturnLength</c> 不会被改写为真实大小，<c>Count</c> 就恒等于 1（只统计 1 项）。</para>
    /// <para>★ 原文缺陷 2（:348-351）：第二次调用失败时 <c>Exit</c> ⇒ 返回空串（Delphi 字符串结果初值）。</para>
    /// <para>★ 原文缺陷 3（:360-361）：<c>Count := ReturnLength div sizeof(...)</c> 未与 <c>Length(Buffer)</c> 复核
    /// ⇒ 若接缝给出的 <c>ReturnLength</c> 超过已分配元素数，原文越权读相邻内存，托管侧抛
    /// <see cref="IndexOutOfRangeException"/>（差异登记）。</para>
    /// </summary>
    public static string GetCPUCount(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;

        var Buffer = new SYSTEM_LOGICAL_PROCESSOR_INFORMATION[1];          // SetLength(Buffer, 1) —— 原文 :338
        uint ReturnLength = HardInfoLayout.SizeOfSystemLogicalProcessorInformation; // 原文 :339

        // 第一次调用获取缓冲区大小 —— 原文 :341-353
        if (!runtime.Processor.GetLogicalProcessorInformation(Buffer, ref ReturnLength))
        {
            if (runtime.Processor.GetLastError() == ERROR_INSUFFICIENT_BUFFER)
            {
                Buffer = new SYSTEM_LOGICAL_PROCESSOR_INFORMATION[
                    (int)(ReturnLength / HardInfoLayout.SizeOfSystemLogicalProcessorInformation + 1)]; // 原文 :346
                // 第二次调用，返回结果 —— 原文 :348
                if (!runtime.Processor.GetLogicalProcessorInformation(Buffer, ref ReturnLength))
                {
                    return ""; // 原文 :350 `Exit;`（Result 为 ''）
                }
            }
        }

        int Count = (int)(ReturnLength / HardInfoLayout.SizeOfSystemLogicalProcessorInformation); // 原文 :360
        return FormatCPUCount(Buffer, Count);
    }

    /// <summary>
    /// 原文 <c>:355-381</c> 的统计与格式化（提取的纯逻辑，原文无独立名称）。
    /// <para>★ 原文缺陷：<c>case ... of</c>（:363-378）**没有 else** ⇒ 现代 Windows 返回的
    /// <c>RelationGroup(4)</c>/<c>RelationProcessorDie(5)</c>/<c>RelationNumaNodeEx(6)</c>/<c>RelationProcessorModule(7)</c>
    /// 一律被**静默丢弃**，不计入任何计数。</para>
    /// <para>★ <c>RelationCache</c> 分支用 <c>CountSetBits</c> 判"是否单逻辑处理器"，而
    /// <see cref="CountSetBits"/> 实际只在 mask ∈ {0,1} 时返回 1 ⇒ 判据被完全改变（见其注释）。</para>
    /// <para><c>Format('%d ...')</c> 的参数序是 <c>numaNodeCount, processorPackageCount, processorCoreCount, logicalProcessorCount</c>
    /// —— 注意标签顺序（NumaNodes / PhysicalProcessorPackages / ProcessorCores / LogicalProcessors）与
    /// 计数变量声明顺序（core / numa / logical / package）**不一致**，容易看错。</para>
    /// </summary>
    public static string FormatCPUCount(SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] Buffer, int Count)
    {
        int processorCoreCount = 0;    // 原文 :355
        int numaNodeCount = 0;         // 原文 :356
        int logicalProcessorCount = 0; // 原文 :357
        int processorPackageCount = 0; // 原文 :358

        for (int III = 0; III <= Count - 1; III++)
        {
            switch ((int)Buffer[III].Relationship)
            {
                case (int)TLogicalProcessorRelationship.RelationProcessorCore:    // 原文 :364
                    processorCoreCount++;
                    break;
                case (int)TLogicalProcessorRelationship.RelationNumaNode:         // 原文 :366
                    numaNodeCount++;
                    break;
                case (int)TLogicalProcessorRelationship.RelationProcessorPackage: // 原文 :368
                    processorPackageCount++;
                    break;
                case (int)TLogicalProcessorRelationship.RelationCache:            // 原文 :370
                {
                    int JJJ = (int)CountSetBits(Buffer[III].ProcessorMask);       // 原文 :372
                    if (JJJ == 1)
                    {
                        logicalProcessorCount++;                                  // 原文 :375
                    }
                    break;
                }
                // 原文如此：case 无 else（:363-378）—— 其余 Relationship 值被静默忽略
            }
        }

        return DelphiFormat.Format(
            "NumaNodes=%d PhysicalProcessorPackages=%d ProcessorCores=%d LogicalProcessors=%d",
            numaNodeCount, processorPackageCount, processorCoreCount, logicalProcessorCount); // 原文 :380-381
    }

    // =========================================================================================
    // GetCpuIDstringEx —— 原文 :387-394
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:387-394</c>：<c>GetSystemInfo</c> 后把 4 个数值**直接拼接**（无分隔符）。
    /// </summary>
    public static string GetCpuIDstringEx(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        runtime.Processor.GetSystemInfo(out TSystemInfo SysInfo);
        return FormatCpuIDstringEx(in SysInfo);
    }

    /// <summary>
    /// 原文 <c>:392-393</c> 的 4 段拼接（提取的纯逻辑，原文无独立名称）。
    /// <para>★ 原文缺陷：4 个无符号数**无分隔符**直接相连 ⇒ 结果**不可逆**
    /// （如 (1,2,3,4) 与 (12,3,4) 无法区分；甚至 (1,23,4,5) 与 (12,3,4,5) 都是 "12345"）。</para>
    /// <para>★ 存疑项：Delphi 7 的 <c>IntToStr</c> 有 <c>Integer</c> 与 <c>Int64</c> 两个重载，
    /// <c>DWORD</c> 实参落到哪个重载本仓库无法取证（决定 &gt;= $80000000 时打印无符号还是有符号）。
    /// 本移植沿 <c>GXX.Core.Rtl.DelphiRTL.IntToStr(uint)</c>（无符号）；四个字段的真实取值域
    /// （OEM ID ≤ 9、处理器数 ≤ 128、类型 ≤ 8664、Revision ≤ 65535）都不触及该分歧。</para>
    /// </summary>
    public static string FormatCpuIDstringEx(in TSystemInfo SysInfo)
        => DelphiRTL.IntToStr(SysInfo.dwOemId)
         + DelphiRTL.IntToStr(SysInfo.dwNumberOfProcessors)
         + DelphiRTL.IntToStr(SysInfo.dwProcessorType)
         + DelphiRTL.IntToStr(SysInfo.wProcessorRevision);

    // =========================================================================================
    // GetIdeDiskSerialNumber —— 原文 :396-595
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:396-595</c>：取硬盘物理号（序列号/型号/固件版本/容量参数）。
    /// <para>
    /// ★ 参数语义：原文是 <c>var</c> 形参，**失败路径（:513/:540/:549/:570）不改写它们** ⇒
    /// 托管侧用 <c>ref</c>（不是 <c>out</c>）以保留"失败时调用方原值不变"的语义（差异断言锁定）。
    /// </para>
    /// <para>
    /// ★ 原文缺陷：Win9x 分支把 <c>pOutData</c> 设为 <c>@pInData^.bBuffer</c>（:552，= 缓冲 +32），
    /// 于是 IDENTIFY 扇区定位在 <b>+48</b>；而 NT 分支是 <c>sizeof(SRB_IO_CONTROL) + 16</c> = <b>+44</b>。
    /// 两条分支对**同一 IOCTL 的同一输出**取了不同偏移 —— 至少一侧必然错位。
    /// </para>
    /// <para>★ <c>SetString(S, Buf, Len)</c>（:578/:581/:584，定长重载）**不在 #0 处截断**，
    /// 与 <see cref="GetIdeSerialNumber"/> 的 PChar→string（截断于 #0）语义不同（差异断言锁定）。</para>
    /// </summary>
    public static bool GetIdeDiskSerialNumber(
        ref string SerialNumber,
        ref string ModelNumber,
        ref string FirmwareRev,
        ref uint TotalAddressableSectors,
        ref uint SectorCapacity,
        ref ushort SectorsPerTrack,
        THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;

        bool Result = false; // 原文 :504
        int idSectorOffset;
        byte[] Buffer;
        bool ioOk;
        uint cbBytesReturned = 0;

        if (runtime.Disk.Win32Platform == VER_PLATFORM_WIN32_NT)
        {
            // Windows NT, Windows 2000 —— 原文 :506-544
            // Get SCSI port handle —— 原文 :508（CreateFile('\\.\Scsi0:') 在接缝内）
            Buffer = new byte[SCSI_BufferSize];
            // FillChar(Buffer, BufferSize, #0);  —— 原文 :505（已在分支前，此处等价）
            BuildScsiMiniportIdentifyRequest(Buffer); // 原文 :515-537
            ioOk = runtime.Disk.ScsiMiniportIoControl(Buffer, SCSI_BufferSize, out cbBytesReturned); // 原文 :538-540
            idSectorOffset = HardInfoLayout.IdeDiskSerialNumberScsiIdentifyOffset;                   // 28 + 16 = 44
        }
        else
        {
            // Windows 95 OSR2, Windows 98 —— 原文 :545-574
            Buffer = new byte[SCSI_BufferSize];
            var pInData = new TSendCmdInParams();
            // FillChar(Buffer, BufferSize, #0) —— 原文 :505
            BuildScsiIdentifySendCmdIn(ref pInData, identifyCommand: IDE_ID_FUNCTION); // 原文 :553-567
            // pInData := PSendCmdInParams(@Buffer); —— 原文 :551（Win9x 分支的 SCIP 就写在缓冲首址）
            WriteStruct(pInData, Buffer, 0);
            ioOk = runtime.Disk.SmartVsdIoControl(
                pInData, HardInfoLayout.SendCmdInParamsSize - 1, Buffer,
                HardInfoLayout.SendCmdInParams_bBuffer, W9xBufferSize, out cbBytesReturned); // 原文 :568-570
            idSectorOffset = HardInfoLayout.IdeDiskSerialNumberW9xIdentifyOffset; // 32 + 16 = 48
        }

        if (!ioOk) return Result; // 原文 :513/:540/:549/:570 `Exit`

        EnsureIdentifyBuffer(Buffer, idSectorOffset, nameof(GetIdeDiskSerialNumber));
        DecodeIdSector(Buffer, idSectorOffset, ref SerialNumber, ref ModelNumber, ref FirmwareRev,
            ref TotalAddressableSectors, ref SectorCapacity, ref SectorsPerTrack);
        return true; // 原文 :585 `Result := True;`（原文在 ulTotalAddressableSectors 之前）
    }

    /// <summary>
    /// 原文 <c>:457-467</c> 的 <c>with PIdSector(PChar(pOutData) + 16)^ do</c> 段（提取的纯逻辑）。
    /// <para>执行顺序照抄原文：serial（:577-578）→ model（:580-581）→ firmware（:583-584）→
    /// 【<c>Result := True</c>，:585】→ total（:586-587）→ capacity（:589-590）→ sectorsPerTrack（:592-593）。</para>
    /// <para>四个数值字段都是先 <see cref="ChangeByteOrder"/> 再读（小端 32/16 位读取）。</para>
    /// </summary>
    public static void DecodeIdSector(
        byte[] Buffer, int idSectorOffset,
        ref string SerialNumber, ref string ModelNumber, ref string FirmwareRev,
        ref uint TotalAddressableSectors, ref uint SectorCapacity, ref ushort SectorsPerTrack)
    {
        int b = idSectorOffset;

        ChangeByteOrder(Buffer, b + HardInfoLayout.IdSector_sSerialNumber, HardInfoLayout.IdSector_sSerialNumberLength); // :577
        SerialNumber = DelphiRTL.AnsiString(Buffer, b + HardInfoLayout.IdSector_sSerialNumber,
            HardInfoLayout.IdSector_sSerialNumberLength);                                                                 // :578 硬盘生产序号

        ChangeByteOrder(Buffer, b + HardInfoLayout.IdSector_sModelNumber, HardInfoLayout.IdSector_sModelNumberLength);   // :580
        ModelNumber = DelphiRTL.AnsiString(Buffer, b + HardInfoLayout.IdSector_sModelNumber,
            HardInfoLayout.IdSector_sModelNumberLength);                                                                  // :581 硬盘型号

        ChangeByteOrder(Buffer, b + HardInfoLayout.IdSector_sFirmwareRev, HardInfoLayout.IdSector_sFirmwareRevLength);   // :583
        FirmwareRev = DelphiRTL.AnsiString(Buffer, b + HardInfoLayout.IdSector_sFirmwareRev,
            HardInfoLayout.IdSector_sFirmwareRevLength);                                                                  // :584 硬盘硬件版本

        // Result := True;  —— 原文 :585（由调用方 <see cref="GetIdeDiskSerialNumber"/> 负责）

        ChangeByteOrder(Buffer, b + HardInfoLayout.IdSector_ulTotalAddressableSectors, sizeof(uint));                     // :586
        TotalAddressableSectors = ReadUInt32LE(Buffer, b + HardInfoLayout.IdSector_ulTotalAddressableSectors);            // :587

        ChangeByteOrder(Buffer, b + HardInfoLayout.IdSector_ulCurrentSectorCapacity, sizeof(uint));                       // :589
        SectorCapacity = ReadUInt32LE(Buffer, b + HardInfoLayout.IdSector_ulCurrentSectorCapacity);                       // :590

        ChangeByteOrder(Buffer, b + HardInfoLayout.IdSector_wNumCurrentSectorsPerTrack, sizeof(ushort));                  // :592
        SectorsPerTrack = ReadUInt16LE(Buffer, b + HardInfoLayout.IdSector_wNumCurrentSectorsPerTrack);                   // :593
    }

    private static uint ReadUInt32LE(byte[] b, int o) => (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));

    private static ushort ReadUInt16LE(byte[] b, int o) => (ushort)(b[o] | (b[o + 1] << 8));

    /// <summary>原文 <c>:515-537</c>：构造 SCSI miniport IDENTIFY 请求（SRB 头 + SENDCMDINPARAMS）。</summary>
    private static void BuildScsiMiniportIdentifyRequest(byte[] Buffer)
    {
        var srbControl = new TSrbIoControl();
        srbControl.HeaderLength = HardInfoLayout.SrbIoControlSize; // sizeof(SRB_IO_CONTROL) —— 原文 :515
        var sig = DelphiRTL.AnsiBytes("SCSIDISK");                 // System.Move('SCSIDISK', srbControl.Signature, 8) —— :516
        for (int i = 0; i < 8; i++) srbControl.Signature[i] = i < sig.Length ? sig[i] : (byte)0;
        srbControl.Timeout = 2;                                    // 原文 :517
        srbControl.Length = (uint)SCSI_DataSize;                   // 原文 :518
        srbControl.ControlCode = IOCTL_SCSI_MINIPORT_IDENTIFY;     // 原文 :519

        var pInData = new TSendCmdInParams();
        // pInData := PSendCmdInParams(PChar(@Buffer) + sizeof(SRB_IO_CONTROL));  —— 原文 :520
        BuildScsiIdentifySendCmdIn(ref pInData, identifyCommand: IDE_ID_FUNCTION); // 原文 :523-537

        WriteStruct(srbControl, Buffer, 0);
        WriteStruct(pInData, Buffer, HardInfoLayout.SrbIoControlSize);
    }

    /// <summary>原文 <c>:523-537</c>（NT 分支）与 <c>:553-567</c>（Win9x 分支）的 SCIP 字段赋值。</summary>
    private static void BuildScsiIdentifySendCmdIn(ref TSendCmdInParams pInData, byte identifyCommand)
    {
        pInData.cBufferSize = IDENTIFY_BUFFER_SIZE; // 原文 :525 / :555
        pInData.bDriveNumber = 0;                   // 原文 :526 / :556
        pInData.irDriveRegs.bFeaturesReg = 0;       // 原文 :529 / :559
        pInData.irDriveRegs.bSectorCountReg = 1;    // 原文 :530 / :560
        pInData.irDriveRegs.bSectorNumberReg = 1;   // 原文 :531 / :561
        pInData.irDriveRegs.bCylLowReg = 0;         // 原文 :532 / :562
        pInData.irDriveRegs.bCylHighReg = 0;        // 原文 :533 / :563
        pInData.irDriveRegs.bDriveHeadReg = 0xA0;   // 原文 :534 / :564
        pInData.irDriveRegs.bCommandReg = identifyCommand; // 原文 :535 / :565（IDE_ID_FUNCTION = $EC）
    }

    /// <summary>把托管接缝结构按其在 Delphi 中的字节布局写入原始缓冲（等价原文的指针别名写法）。</summary>
    private static void WriteStruct<T>(in T value, byte[] buffer, int offset) where T : unmanaged
    {
        var span = MemoryMarshal.CreateReadOnlySpan(ref System.Runtime.CompilerServices.Unsafe.AsRef(in value), 1);
        MemoryMarshal.AsBytes(span).CopyTo(buffer.AsSpan(offset));
    }

    /// <summary>
    /// 托管侧守卫：接缝返回的原始缓冲必须容得下 <c>offset + sizeof(TIdSector)</c>（= 256）。
    /// <para>★ 差异登记：原文无此检查，缓冲过短时会越权读写（原文缺陷方向是"读垃圾"而非崩溃）。</para>
    /// </summary>
    private static void EnsureIdentifyBuffer(byte[] raw, int identifyOffset, string caller)
    {
        if (raw == null || raw.Length < identifyOffset + HardInfoLayout.IdSectorSize)
            throw new InvalidOperationException(
                caller + "：接缝返回的 IDENTIFY 原始缓冲过短（需 >= " + (identifyOffset + HardInfoLayout.IdSectorSize) +
                " 字节，实得 " + (raw?.Length ?? 0) + " 字节）—— 接缝实现违约（原文 HardInfo.pas 无此检查，会越权读内存）。");
    }

    // =========================================================================================
    // GetWindowsVersion —— 原文 :597-660
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:597-660</c>：<c>GetVersionEx</c> 后按平台/版本映射成中文可读串。
    /// <para>★ 原文缺陷：<c>GetVersionEx</c> 的返回值被丢弃（:606）；自 Win8.1 起该 API 对未声明
    /// supportedOS 的进程返回假版本（真实实现应改 RtlGetVersion/WMI）。</para>
    /// <para>★ <c>FillChar(VersionInfo, SizeOf(VersionInfo), 0)</c>（:603）在托管侧等价于
    /// <c>new TOSVersionInfo()</c>（值类型全 0，含 <c>szCSDVersion</c> 的 128 字节）。</para>
    /// </summary>
    public static string GetWindowsVersion(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        var VersionInfo = new TOSVersionInfo();                   // FillChar(..., 0) —— 原文 :603
        VersionInfo.dwOSVersionInfoSize = (uint)TOSVersionInfo.Size; // SizeOf(VersionInfo) —— 原文 :604
        runtime.Version.GetVersionEx(ref VersionInfo);            // 原文 :606（返回值被忽略）
        return FormatWindowsVersion(in VersionInfo);
    }

    /// <summary>
    /// 原文 <c>:610-658</c> 的平台/版本分支映射（提取的纯逻辑，原文无独立名称）。
    /// <para>
    /// ★★ 三个必须照抄的原文缺陷/易错点：
    /// <list type="number">
    /// <item><b>:622</b> 判据是 <c>szCSDVersion[1] = 'A'</c> —— 即**第 2 个字符**（Windows 98 SE 的
    ///       CSDVersion 为 <c>" A "</c>，A 在下标 1）⇒ <c>"A..."</c>（A 在下标 0）**不会**被判为 98 SE。</item>
    /// <item><b>:641-644</b> 内层 <c>case dwMinorVersion of 0/1</c> **没有 else** ⇒ major=5 且 minor∉{0,1}
    ///       （即 Windows XP = 5.1 之外的 5.2/5.3）时 <c>result</c> 保持函数结果初值 <c>''</c>，
    ///       最终输出形如 <c>", Build: 2600"</c>（**XP/2003 不被识别**）。</item>
    /// <item><b>:658</b> <c>IntToStr(Loword(dwBuildNumber))</c> ⇒ 只取低 16 位，build ≥ 65536 被截断
    ///       （如 $00010005 → "5"）。</item>
    /// </list>
    /// </para>
    /// <para>★ 最后一行（:657-658）对**所有**分支都追加 <c>", Build: N"</c>（连 'Unknown Platform' 也不例外）。</para>
    /// <para>★ <c>if VersionInfo.szCSDVersion &lt;&gt; ''</c>（:650）只对 platform = 2 分支生效。</para>
    /// </summary>
    public static string FormatWindowsVersion(in TOSVersionInfo VersionInfo)
    {
        string result;

        switch (VersionInfo.dwPlatformId) // 原文 :610
        {
            case 0: // 原文 :611
                result = "Windows 3.11"; // :613
                break;

            case 1: // 原文 :616
                switch (VersionInfo.dwMinorVersion) // :618
                {
                    case 0:
                        result = "Windows 95"; // :619
                        break;
                    case 10: // 原文 :620
                        if (VersionInfo.CSDVersionAt(1) == (byte)'A') // ★ szCSDVersion[1]，不是 [0] —— 原文 :622
                            result = "Windows 98 SE";                 // :623
                        else
                            result = "Windows 98"; // :625
                        break;
                    case 90:
                        result = "Windows Millenium"; // :627
                        break;
                    default:
                        result = "Unknown Version"; // :629
                        break;
                }
                break; // end case —— 原文 :630

            case 2: // 原文 :633
                switch (VersionInfo.dwMajorVersion) // :635
                {
                    case 3:
                    case 4:
                        result = "Windows NT " +
                            DelphiRTL.IntToStr(VersionInfo.dwMajorVersion) + "." +
                            DelphiRTL.IntToStr(VersionInfo.dwMinorVersion); // 原文 :636-638
                        break;
                    case 5: // 原文 :639
                        switch (VersionInfo.dwMinorVersion) // :641
                        {
                            case 0:
                                result = "Windows 2000"; // :642
                                break;
                            case 1:
                                result = "Windows Whistler"; // :643
                                break;
                            default:
                                // 原文如此（HardInfo.pas:641-644）：内层 case **无 else** ⇒
                                // result 保持 Delphi 字符串函数结果的初值 ''（如 Windows XP = 5.1 之外的 5.2/5.3）
                                result = "";
                                break;
                        }
                        break; // end 5 —— 原文 :645
                    default:
                        result = "Unknown Version"; // :647
                        break;
                }
                // service packs apply to the NT/2000 platform —— 原文 :649-651
                if (VersionInfo.CSDVersionString() != "")
                    result = result + " Service pack: " + VersionInfo.CSDVersionString();
                break; // end 2 —— 原文 :652

            default:
                result = "Unknown Platform"; // 原文 :654
                break;
        }

        // add build info.  —— 原文 :656-658
        result = result + ", Build: " + DelphiRTL.IntToStr((int)DelphiRTL.LoWord((int)VersionInfo.dwBuildNumber));
        return result;
    }

    // =========================================================================================
    // GetAdapterMac —— 原文 :664-705
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:664-705</c>：用 NetBIOS 枚举 → Reset → AdapterStatus 取网卡 MAC（12 位大写十六进制，无分隔）。
    /// <para>★ 原文缺陷 1（:677-681）：第一次 <c>NetBios</c>（NCBENUM）在 <c>ncb_buffer/ncb_length</c> **尚未设置**时发出
    /// （<c>ZeroMemory</c> 后 buffer=nil、length=0）—— 这是一次注定失败的多余调用；真正的枚举在 :681，其返回码才被检查。</para>
    /// <para>★ 原文缺陷 2（:695）：最后一次 <c>NetBios</c>（NCBASTAT）的**返回码被丢弃**；
    /// 若它失败，<c>Adapter</c> 保持 <c>ZeroMemory</c> 的全 0 ⇒ 返回 <c>'000000000000'</c>（无法与真实全 0 MAC 区分）。</para>
    /// <para>★ 原文缺陷 3（:685/:691）：<c>Lanaenum.lana[aNo]</c> **不检查 <paramref name="ANo"/> 的 0..254 边界**。</para>
    /// <para>★ <c>Result := ''</c>（:674）在 <c>try</c> 之前；<c>try..finally</c> 的 finally 体是**空的**（:702-704）——原文冗余，照抄保留。</para>
    /// <para>接缝：<see cref="INetBiosApi"/> —— 托管侧额外把缓冲数组传给接缝（见该接口注释）。</para>
    /// </summary>
    public static string GetAdapterMac(int ANo, THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;
        string Result = ""; // 原文 :674
        try
        {
            // ZeroMemory(@Ncb, SizeOf(Ncb)); —— 原文 :676
            var Ncb = new TNCB();
            Ncb.ncb_command = NCBENUM;                       // Ncb.ncb_command := Chr(NCbenum); —— 原文 :677
            runtime.NetBios.NetBios(ref Ncb, null);          // NetBios(@NCb);  —— 原文 :678（返回码被丢弃，见缺陷 1）

            var lanaEnumBytes = new byte[TLanaEnum.Size];    // 原文 `Lanaenum: TLanaenum`（未初始化局部，由 NCBENUM 填充）
            Ncb.ncb_length = TLanaEnum.Size;                 // Ncb.ncb_length := SizeOf(Lanaenum); —— 原文 :680
            // Ncb.ncb_buffer := @Lanaenum; —— 原文 :679（托管侧用 fixed 写入真实地址）
            byte cRc;
            fixed (byte* pLana = lanaEnumBytes)
            {
                Ncb.ncb_buffer = (nint)pLana;
                cRc = runtime.NetBios.NetBios(ref Ncb, lanaEnumBytes); // cRc := NetBios(@Ncb); —— 原文 :681
            }
            if (cRc != 0) return Result;                     // if Ord(cRc) <> 0 then exit; —— 原文 :682

            byte lanaNum = TLanaEnum.GetLana(lanaEnumBytes, ANo); // Lanaenum.lana[aNo] —— 原文 :685/:691

            // ZeroMemory(@Ncb, SizeOf(Ncb)); —— 适配器清零 —— 原文 :683
            Ncb = new TNCB();
            Ncb.ncb_command = NCBRESET;                      // 原文 :684
            Ncb.ncb_lana_num = lanaNum;                      // 原文 :685
            cRc = runtime.NetBios.NetBios(ref Ncb, null);    // 原文 :686
            if (cRc != 0) return Result;                     // 原文 :687

            // 得到适配器状态 —— 原文 :688-695
            Ncb = new TNCB();
            Ncb.ncb_command = NCBASTAT;                      // 原文 :690
            Ncb.ncb_lana_num = lanaNum;                      // 原文 :691
            SetCallName(ref Ncb, "*");                       // StrPcopy(Ncb.ncb_callname, '*'); —— 原文 :692
            var adapterBytes = new byte[TAdapterStatus.Size]; // 原文 `Adapter: TAdapterStatus`（由 NCBASTAT 填充）
            Ncb.ncb_length = TAdapterStatus.Size;            // Ncb.ncb_length := SizeOf(Adapter); —— 原文 :694
            fixed (byte* pAdapter = adapterBytes)
            {
                Ncb.ncb_buffer = (nint)pAdapter;             // Ncb.ncb_buffer := @Adapter; —— 原文 :693
                runtime.NetBios.NetBios(ref Ncb, adapterBytes); // NetBios(@Ncb); —— 原文 :695（返回码被丢弃）
            }

            // 将mac地址转换成字符串输出 —— 原文 :696-701
            Result = FormatAdapterAddress6(adapterBytes);    // 原文 :697-701
            return Result;
        }
        finally
        {
            // 原文如此（HardInfo.pas:702-704）：`finally` 块是**空的**（无资源释放）
        }
    }

    /// <summary>
    /// 原文 <c>:699-700</c>：<c>for IntIdx := 0 to 5 do StrTemp := StrTemp + IntToHex(Integer(Adapter.adapter_address[IntIdx]), 2);</c>
    /// <para>★ 恰好读 6 字节（与 <c>MAX</c> 无关）；<c>IntToHex(..., 2)</c> 为**大写、最小宽度 2**。</para>
    /// <para>★ 空数组/短数组在原文是越权读（无边界检查），托管侧抛 <see cref="IndexOutOfRangeException"/>。</para>
    /// </summary>
    public static string FormatAdapterAddress6(byte[] adapterAddress)
    {
        string StrTemp = ""; // 原文 :697
        for (int IntIdx = 0; IntIdx <= 5; IntIdx++)
            StrTemp = StrTemp + ((int)adapterAddress[IntIdx]).ToString("X2", System.Globalization.CultureInfo.InvariantCulture);
        return StrTemp;
    }

    /// <summary>原文 <c>:692</c> <c>StrPcopy(Ncb.ncb_callname, '*')</c> —— 写 <c>'*'</c> + <c>#0</c>，其余保持（原文 已 ZeroMemory）。</summary>
    private static void SetCallName(ref TNCB Ncb, string value)
    {
        var tmp = Ncb; // 值拷贝；fixed 缓冲只在 unsafe 上下文内可索引，局部变量最直接
        var bytes = DelphiRTL.AnsiBytes(value ?? "");
        for (int i = 0; i < 16; i++) tmp.ncb_callname[i] = i < bytes.Length ? bytes[i] : (byte)0;
        Ncb = tmp;
    }

    // =========================================================================================
    // GetNetCardName —— 原文 :707-773（implementation 段内，原文未导出）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:707-773</c>：从注册表 <c>...\NetworkCards\</c> 枚举网卡并读第一个可打开项的 <c>ServiceName</c>。
    /// <para>★ 可达性：原文只被 <c>:881</c>（在**被整段注释掉的</b> GetHwid2 内）引用 ⇒ 实际是**死代码**，
    /// 但仍参与编译。托管侧为可测性改为 <c>public</c>（差异登记：原文位于 implementation 段）。</para>
    /// <para>★ 原文缺陷 1（:757）：<c>RegEnum</c> 的 Boolean 返回值被丢弃。</para>
    /// <para>★ 原文缺陷 2（:760-772）：<c>Reg.Free</c>/<c>netList.Free</c> **不在 try..finally** 中 ⇒ 异常时泄漏；
    /// 且 <c>Reg := TRegistry.Create</c>（无参虚构造 ⇒ <c>FAccess = KEY_READ</c>）后才设 <c>RootKey</c>。</para>
    /// <para>★ 原文缺陷 3（:762-770）：无任何 <c>ServiceName</c> 存在性判断 —— <c>OpenKey</c> 成功即 <c>Break</c>，读不到就返回空串。</para>
    /// <para>★ 原文缺陷 4（:726-742）：<c>repeat</c> 的条件是 <c>iRes &lt;&gt; ERROR_SUCCESS</c>，而 <c>iRes</c> 只在
    /// <c>ERROR_SUCCESS</c> 时自增 <c>i</c> ⇒ 逻辑正确但**任何非 0 且非 ERROR_NO_MORE_ITEMS 的错误也被当成正常结束**。</para>
    /// </summary>
    public static string GetNetCardName(THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;

        const string NetKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\NetworkCards\\"; // 原文 :750

        string netListText = "";
        RegEnum(runtime, HKEY_LOCAL_MACHINE, NetKey, ref netListText, true); // 原文 :757（返回值被忽略；RootKey = HKEY_LOCAL_MACHINE，:749）

        var netList = new TStringList();
        foreach (var line in SplitTextStr(netListText))           // netList.Text := netListText; —— 原文 :759
            netList.Add(line);

        nint Reg = runtime.Registry.RegistryCreate();             // Reg := TRegistry.Create; —— 原文 :760
        runtime.Registry.RegistrySetRootKey(Reg, HKEY_LOCAL_MACHINE); // Reg.RootKey := RootKey; —— 原文 :761

        string Result = null;                                     // Delphi 字符串函数结果初值 ''
        for (int i = 0; i <= netList.Count - 1; i++)              // 原文 :762
        {
            if (runtime.Registry.RegistryOpenKey(Reg, NetKey + netList[i], false)) // 原文 :764
            {
                Result = runtime.Registry.RegistryReadString(Reg, "ServiceName");  // 原文 :766
                runtime.Registry.RegistryCloseKey(Reg);                            // 原文 :767
                break;                                                             // 原文 :768
            }
        }

        runtime.Registry.RegistryFree(Reg);                       // Reg.Free; —— 原文 :771
        // netList.Free; —— 原文 :772（托管侧由 GC 负责）
        return Result ?? "";
    }

    /// <summary>
    /// 原文 <c>HardInfo.pas:708-747</c> 的内嵌 <c>function RegEnum(RootKey: HKEY; Name: string; var ResultList: string;
    /// const DoKeys: Boolean): Boolean;</c>。
    /// <para>★ <c>Move(Buf^, s[1], BufSize)</c>（:735）按 <c>SetString</c> 定长语义复制（不在 #0 处截断）；
    /// <c>s</c> 是 Delphi 7 的 <c>AnsiString</c> ⇒ GBK 解码。</para>
    /// <para>★ 连接符是 <c>#13#10</c>（CRLF），恰好与 <c>TStrings.Text</c> 的分行符一致（Round-trip 成立）。</para>
    /// </summary>
    private static bool RegEnum(THardInfoRuntime runtime, nint RootKey, string Name, ref string ResultList, bool DoKeys)
    {
        bool Result = false;   // 原文 :717
        ResultList = "";       // 原文 :718
        if (runtime.Registry.RegOpenKeyEx(RootKey, Name, KEY_READ, out nint hTemp)) // 原文 :719
        {
            Result = true;                             // 原文 :721
            uint BufSize = 1024;                       // BufSize := 1024; —— 原文 :722
            var Buf = new byte[BufSize];               // GetMem(Buf, BufSize); —— 原文 :723
            int i = 0;                                 // 原文 :724
            uint iRes;
            do
            {
                BufSize = 1024;                        // 原文 :727
                iRes = DoKeys
                    ? runtime.Registry.RegEnumKeyEx(hTemp, i, Buf, ref BufSize)   // 原文 :729
                    : runtime.Registry.RegEnumValue(hTemp, i, Buf, ref BufSize);  // 原文 :731
                if (iRes == ERROR_SUCCESS)             // 原文 :732
                {
                    string s = DelphiRTL.AnsiString(Buf, 0, (int)BufSize); // SetLength(s, BufSize); Move(Buf^, s[1], BufSize); —— :734-735
                    if (ResultList == "")              // 原文 :736
                        ResultList = s;                // 原文 :737
                    else
                        ResultList = string.Concat(ResultList, "\r\n", s); // Concat(ResultList, #13#10, s) —— 原文 :739
                    i++;                               // 原文 :740
                }
            } while (iRes == ERROR_SUCCESS);           // until iRes <> ERROR_SUCCESS; —— 原文 :742

            runtime.Registry.RegCloseKey(hTemp);       // RegCloseKey(hTemp); —— 原文 :745
        }
        return Result;
    }

    /// <summary>
    /// <c>Classes.TStrings.SetTextStr</c> 的等价实现（原文 <c>netList.Text := netListText</c>，:759）。
    /// <para>
    /// ★ 接缝/依赖缺口登记：<c>GXX.Core.Util.TStringList</c> **没有 <c>Text</c> 属性**
    /// （`grep "public string Text" Util/TStringList.cs` 零命中），故此处按 Delphi <c>SetTextStr</c> 语义内联实现：
    /// 以 <c>#13</c>/<c>#10</c> 分行（<c>#13#10</c> 视作一次换行），**结尾换行不产生额外空行**，空串 → 0 行。
    /// </para>
    /// </summary>
    public static string[] SplitTextStr(string value)
    {
        if (string.IsNullOrEmpty(value)) return Array.Empty<string>();
        var lines = new List<string>();
        int p = 0;
        while (p < value.Length)
        {
            int start = p;
            while (p < value.Length && value[p] != '\n' && value[p] != '\r') p++;
            lines.Add(value.Substring(start, p - start));
            if (p < value.Length && value[p] == '\r') p++;
            if (p < value.Length && value[p] == '\n') p++;
        }
        return lines.ToArray();
    }

    // =========================================================================================
    // GetNetCardMac —— 原文 :775-813（implementation 段内，原文未导出）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:775-813</c>：对 <c>'\\.\' + NetCardName</c> 发
    /// <c>IOCTL_NDIS_QUERY_GLOBAL_STATS</c>（OID = <c>OID_802_3_PERMANENT_ADDRESS</c>）取 MAC。
    /// <para>★ 原文如此：<c>OID_802_3_CURRENT_ADDRESS</c>（:778）与 <c>IOCTL_NDIS_QUERY_GLOBAL_STATS</c>（:779）
    /// 都在 const 段声明，但前者**从未使用**。</para>
    /// <para>★ 原文缺陷：<c>BytesReturned</c> 未与 <c>256</c> 比较（:802）⇒ 驱动返回 &gt; 256 时越权读栈；
    /// 托管侧抛 <see cref="IndexOutOfRangeException"/>（差异登记）。</para>
    /// <para>★ 原文用 <c>try..finally</c> 关句柄，但 <c>hDevice := INVALID_HANDLE_VALUE</c> 初值（:790）
    /// 使 finally 的判据成立性依赖该初值 —— 语义正确，照抄。</para>
    /// </summary>
    public static string GetNetCardMac(string NetCardName, THardInfoRuntime runtime = null)
    {
        runtime ??= THardInfoRuntime.Default;

        const int OID_802_3_PERMANENT_ADDRESS = 0x01010101;      // 原文 :777
        const int OID_802_3_CURRENT_ADDRESS = 0x01010102;        // 原文 :778（★ 声明但从未使用）
        const int IOCTL_NDIS_QUERY_GLOBAL_STATS = 0x00170002;    // 原文 :779

        var outBuf = new byte[256];                              // outBuf: array[1..256] of Byte —— 原文 :783
        string Result = "";                                      // 原文 :789
        if (!runtime.Ndis.DeviceIoControlNdis(
                "\\\\.\\" + NetCardName, IOCTL_NDIS_QUERY_GLOBAL_STATS, OID_802_3_PERMANENT_ADDRESS,
                outBuf, out uint BytesReturned))             // 原文 :792-799
            return Result;
        return FormatNdisAddressHex(outBuf, BytesReturned);      // 原文 :800-806
    }

    /// <summary>
    /// 原文 <c>:802-805</c>：
    /// <c>for i := 1 to BytesReturned do MacAddr := MacAddr + IntToHex(outBuf[i], 2);</c>
    /// <para>★ <c>outBuf</c> 在原文是 <c>array[1..256]</c>（**1-based**）⇒ <c>outBuf[i]</c> 对应托管 <c>outBuf[i-1]</c>。</para>
    /// <para>★ 与 <see cref="FormatAdapterAddress6"/> 的区别：这里**把 6 硬编码换成了 <paramref name="BytesReturned"/>**
    /// （长度由驱动决定，可能是 6，也可能不是）。</para>
    /// </summary>
    public static string FormatNdisAddressHex(byte[] outBuf, uint BytesReturned)
    {
        string MacAddr = ""; // 原文 :801
        for (int i = 1; i <= BytesReturned; i++)
            MacAddr = MacAddr + ((int)outBuf[i - 1]).ToString("X2", System.Globalization.CultureInfo.InvariantCulture);
        return MacAddr;
    }

    // =========================================================================================
    // FormatMac —— 原文 :815-831（implementation 段内，原文未导出）
    // =========================================================================================

    /// <summary>
    /// 原文 <c>HardInfo.pas:815-831</c>：
    /// <code>
    /// Result := ''; nMac := AMac;
    /// for i := 1 to 6 do
    /// begin
    ///   v := LeftStr(nMac, 2);
    ///   nMac := RightStr(nMac, Length(nMac) - 2);
    ///   if i = 1 then Result := v else Result := Result + '-' + v;
    /// end;
    /// </code>
    /// <para>把 12 位 MAC 串切成 <c>AA-BB-CC-DD-EE-FF</c>。</para>
    /// <para>★ 边界：循环**恒 6 轮**，不因输入变短而提前退出 ⇒ 输入不足 12 位时输出仍带足 5 个分隔符
    /// （<c>''</c> → <c>'-----'</c>；<c>'A1'</c> → <c>'A1-----'</c>）；超过 12 位则被丢弃（差异断言锁定）。</para>
    /// <para>★ 可达性：原文只在被注释掉的 GetHwid2（:881）里被引用 ⇒ 死代码，但仍参与编译。</para>
    /// </summary>
    public static string FormatMac(string AMac)
    {
        string Result = "";        // 原文 :820
        string nMac = AMac ?? "";  // 原文 :821（Delphi 的空 AnsiString 即 ''）
        for (int i = 1; i <= 6; i++) // 原文 :822
        {
            string v = LeftStr(nMac, 2);                            // 原文 :824
            nMac = RightStr(nMac, nMac.Length - 2);                 // 原文 :825（Delphi 的 Length 不含长度前缀）
            if (i == 1)
                Result = v;                                         // 原文 :827
            else
                Result = Result + "-" + v;                          // 原文 :829
        }
        return Result;
    }

    /// <summary>StrUtils.LeftStr(S, ACount) = <c>Copy(S, 1, ACount)</c>（原文 uses StrUtils，:7）。</summary>
    private static string LeftStr(string s, int aCount) => DelphiRTL.Copy(s ?? "", 1, aCount);

    /// <summary>
    /// StrUtils.RightStr(S, ACount) = <c>Copy(S, Length(S) - ACount + 1, ACount)</c>。
    /// <para>★ ACount 为负时 <c>Copy</c> 的 Index 会 &gt; Length(S) ⇒ 返回空串（Delphi <c>Copy</c> 的既有语义，
    /// 由 <c>GXX.Core.Rtl.DelphiRTL.Copy</c> 覆盖）。</para>
    /// </summary>
    private static string RightStr(string s, int aCount)
    {
        s ??= "";
        return DelphiRTL.Copy(s, s.Length - aCount + 1, aCount);
    }

    // =========================================================================================
    // 原文 :833-897 —— **整段被注释掉的代码**（照抄保留，便于将来 1:1 复核）
    // =========================================================================================
    /*
{
function GetHwid2: string;
var
  DiskInfo: TDiskDriveInfo;
  BiosInfo: TBiosInfo;
  CpuInfo: TProcessorInfo;
  CSysInfo: TComputerSystemInfo;
  S: string;
begin
  DiskInfo := TDiskDriveInfo.Create(nil);
  BiosInfo := TBiosInfo.Create(nil);
  CpuInfo  := TProcessorInfo.Create(nil);
  CSysInfo := TComputerSystemInfo.Create(nil);
  try
    DiskInfo.Active := True;
    BiosInfo.Active := True;
    CpuInfo.Active := True;
    CSysInfo.Active := True;

    S := // BIOS信息
         BiosInfo.BiosProperties.Manufacturer +                     // 制造商
         BiosInfo.BiosProperties.SoftwareElementID +                // 软件ID
         IntToStr(Trunc(BiosInfo.BiosProperties.ReleaseDate)) +     // 发行日期
         BiosInfo.BiosProperties.SMBIOSBIOSVersion +                // F1（CMOS进入键）
         IntToStr(BiosInfo.BiosProperties.SMBIOSMajorVersion) +     // 主版本
         IntToStr(BiosInfo.BiosProperties.SMBIOSMinorVersion) +     // 次版本

         // 主板信息
         CSysInfo.ComputerSystemProperties.Manufacturer +           // 主板制造商
         CSysInfo.ComputerSystemProperties.Model +                  // 主板型号

         // 硬盘信息
         DiskInfo.DiskDriveProperties.Caption +                     // 硬盘类型
         IntToStr(DiskInfo.DiskDriveProperties.BytesPerSector) +    // 每扇区多少字节
         IntToStr(DiskInfo.DiskDriveProperties.SectorsPerTrack) +   // 每磁道多个扇区
         IntToStr(DiskInfo.DiskDriveProperties.TotalTracks) +       // 共有多少磁道\
         DiskInfo.DiskDriveProperties.SerialNumber +

         // CPU信息
         CpuInfo.ProcessorProperties.Manufacturer +                 // CPU制造商
         CpuInfo.ProcessorProperties.Caption +                      // CPU类型
         CpuInfo.ProcessorProperties.SocketDesignation +            // CPU接口类型
         CpuInfo.ProcessorProperties.ProcessorId +                  // CPU ID
         IntToStr(CpuInfo.ProcessorProperties.Revision) +           // CPU版本号
         IntToStr(CpuInfo.ProcessorProperties.L2CacheSize) +        // 二级缓存大小
         IntToStr(CpuInfo.ProcessorProperties.NumberOfCores);       // 内核数量

         // MAC地址
         // FormatMac(GetNetCardMac(GetNetCardName));
    Result := RivestStr(S);
  finally
    BiosInfo.Free;
    DiskInfo.Free;
    CpuInfo.Free;
    CSysInfo.Free;
  end;
end;

initialization
  CoInitialize(nil);

finalization
  CoUninitialize();

}
    */
    // ★ 移植判定：该块在原文中**被 `{ }` 整段注释**（HardInfo.pas:833 的 `{` 与 :897 的 `}`）
    //   ⇒ 不属于编译单元，**不移植**。其依赖全部未移植：
    //     · WMI 器件类 TDiskDriveInfo/TBiosInfo/TProcessorInfo/TComputerSystemInfo（WbemScripting 导入单元）
    //     · MD5Util.RivestStr（原文 uses MD5Util，:7）—— GXX.Core 尚未移植
    //     · ActiveX.CoInitialize/CoUninitialize（initialization/finalization 段）
    //   因此 HardInfo.pas **没有 initialization 段**（整段随注释消失）。
    //   接缝：待 WMI 器件层与 MD5Util.pas 移植后接入（Checklist.md:269"HardInfo 机器码需 WMI 深化"）。
    //   调用点状态：MShare.pas:13383 对该函数的引用同样被注释掉（:13383 行首 `//`），
    //   故当前活代码里唯一的"机器码"路径是 MShare.pas:3554 与 M2Share.pas:8450。
}

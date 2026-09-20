// 布局锁定：HardInfo.pas 引用的 packed record / Windows.pas / Nb30.pas 结构的
// Marshal.SizeOf / Marshal.OffsetOf 必须与原文的 Delphi SizeOf / 字段偏移一致。
// 期望值全部按原文逐字段累加手工推导（见每条断言的注释）。
using System;
using System.Runtime.InteropServices;
using GXX.Core.Hardware;
using Xunit;

namespace GXX.Core.Tests;

public class HardwareLayoutTests
{
    // ---------------- HardInfo.pas 内嵌的 IDE / SMART 结构 ----------------

    [Fact]
    public void TIDERegs_Is8Bytes()
    {
        // bFeaturesReg..bReserved = 8 × Byte
        Assert.Equal(8, Marshal.SizeOf<TIDERegs>());
        Assert.Equal(0, (int)Marshal.OffsetOf<TIDERegs>(nameof(TIDERegs.bFeaturesReg)));
        Assert.Equal(6, (int)Marshal.OffsetOf<TIDERegs>(nameof(TIDERegs.bCommandReg)));
    }

    [Fact]
    public void TSendCmdInParams_Is33Bytes_AndBufferAt32()
    {
        // cBufferSize(4) + irDriveRegs(8) + bDriveNumber(1) + bReserved[3] + dwReserved[4×4=16] + bBuffer[1]
        // = 4 + 8 + 1 + 3 + 16 + 1 = 33
        Assert.Equal(33, Marshal.SizeOf<TSendCmdInParams>());
        Assert.Equal(0, (int)Marshal.OffsetOf<TSendCmdInParams>(nameof(TSendCmdInParams.cBufferSize)));
        Assert.Equal(4, (int)Marshal.OffsetOf<TSendCmdInParams>(nameof(TSendCmdInParams.irDriveRegs)));
        Assert.Equal(12, (int)Marshal.OffsetOf<TSendCmdInParams>(nameof(TSendCmdInParams.bDriveNumber)));
        Assert.Equal(32, (int)Marshal.OffsetOf<TSendCmdInParams>(nameof(TSendCmdInParams.bBuffer)));
        Assert.Equal(32, HardInfoLayout.SendCmdInParams_bBuffer);
        Assert.Equal(33, HardInfoLayout.SendCmdInParamsSize);
    }

    [Fact]
    public void TDriverStatus_Is12Bytes()
    {
        // bDriverError(1) + bIDEStatus(1) + bReserved[2] + dwReserved[2×4=8] = 12
        Assert.Equal(12, Marshal.SizeOf<TDriverStatus>());
        Assert.Equal(4, (int)Marshal.OffsetOf<TDriverStatus>(nameof(TDriverStatus.dwReserved)));
    }

    [Fact]
    public void TSendCmdOutParams_Is17Bytes_AndBufferAt16()
    {
        // cBufferSize(4) + DriverStatus(12) + bBuffer[1] = 17
        Assert.Equal(17, Marshal.SizeOf<TSendCmdOutParams>());
        Assert.Equal(16, (int)Marshal.OffsetOf<TSendCmdOutParams>(nameof(TSendCmdOutParams.bBuffer)));
        Assert.Equal(16, HardInfoLayout.SendCmdOutParams_bBuffer);
        Assert.Equal(17, HardInfoLayout.SendCmdOutParamsSize);
    }

    [Fact]
    public void TSrbIoControl_Is28Bytes()
    {
        // HeaderLength(4) + Signature[8] + Timeout(4) + ControlCode(4) + ReturnCode(4) + Length(4) = 28
        Assert.Equal(28, Marshal.SizeOf<TSrbIoControl>());
        Assert.Equal(4, (int)Marshal.OffsetOf<TSrbIoControl>(nameof(TSrbIoControl.Signature)));
        Assert.Equal(12, (int)Marshal.OffsetOf<TSrbIoControl>(nameof(TSrbIoControl.Timeout)));
        Assert.Equal(16, (int)Marshal.OffsetOf<TSrbIoControl>(nameof(TSrbIoControl.ControlCode)));
        Assert.Equal(28, HardInfoLayout.SrbIoControlSize);
    }

    [Fact]
    public void TIdSector_Is256Bytes_WithDocumentedFieldOffsets()
    {
        // 逐字段累加（原文 :436-467 的 packed record）：
        //  0 wGenConfig(2) 2 wNumCyls(2) 4 wReserved(2) 6 wNumHeads(2) 8 wBytesPerTrack(2)
        // 10 wBytesPerSector(2) 12 wSectorsPerTrack(2) 14 wVendorUnique[3](6) 20 sSerialNumber[20]
        // 40 wBufferType(2) 42 wBufferSize(2) 44 wECCSize(2) 46 sFirmwareRev[8] 54 sModelNumber[40]
        // 94 wMoreVendorUnique(2) 96 wDoubleWordIO(2) 98 wCapabilities(2) 100 wReserved1(2)
        // 102 wPIOTiming(2) 104 wDMATiming(2) 106 wBS(2) 108 wNumCurrentCyls(2)
        // 110 wNumCurrentHeads(2) 112 wNumCurrentSectorsPerTrack(2) 114 ulCurrentSectorCapacity(4)
        // 118 wMultSectorStuff(2) 120 ulTotalAddressableSectors(4) 124 wSingleWordDMA(2)
        // 126 wMultiWordDMA(2) 128 bReserved[128] → 256
        Assert.Equal(256, Marshal.SizeOf<TIdSector>());
        Assert.Equal(256, HardInfoLayout.IdSectorSize);

        Assert.Equal(20, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.sSerialNumber)));
        Assert.Equal(40, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.wBufferType)));
        Assert.Equal(46, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.sFirmwareRev)));
        Assert.Equal(54, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.sModelNumber)));
        Assert.Equal(112, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.wNumCurrentSectorsPerTrack)));
        Assert.Equal(114, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.ulCurrentSectorCapacity)));
        Assert.Equal(120, (int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.ulTotalAddressableSectors)));

        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.sSerialNumber)), HardInfoLayout.IdSector_sSerialNumber);
        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.wBufferType)), HardInfoLayout.IdSector_wBufferType);
        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.sFirmwareRev)), HardInfoLayout.IdSector_sFirmwareRev);
        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.sModelNumber)), HardInfoLayout.IdSector_sModelNumber);
        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.wNumCurrentSectorsPerTrack)), HardInfoLayout.IdSector_wNumCurrentSectorsPerTrack);
        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.ulCurrentSectorCapacity)), HardInfoLayout.IdSector_ulCurrentSectorCapacity);
        Assert.Equal((int)Marshal.OffsetOf<TIdSector>(nameof(TIdSector.ulTotalAddressableSectors)), HardInfoLayout.IdSector_ulTotalAddressableSectors);
    }

    [Fact]
    public void HardInfo_BufferSizes_MatchOriginalConstSection()
    {
        // 原文 :476-478：DataSize = sizeof(TSendCmdInParams) - 1 + 512 = 544；BufferSize = 28 + 544 = 572
        Assert.Equal(544, HardInfo.SCSI_DataSize);
        Assert.Equal(572, HardInfo.SCSI_BufferSize);
        Assert.Equal(528, HardInfo.W9xBufferSize);

        // 原文 :125：array[0..(SizeOf(TSendCmdOutParams) + IDENTIFY_BUFFER_SIZE - 1) - 1]
        //   = array[0..(17+512-1)-1] = array[0..527] ⇒ **528** 个元素
        Assert.Equal(528, HardInfo.IDEOUTCMD_SIZE);

        // 原文 :178 的 bBuffer 偏移 = 16；:520-522 + :575 的 SRB 头 28 + 16 = 44；:552 + :575 的 32 + 16 = 48
        Assert.Equal(16, HardInfoLayout.IdeSerialNumberIdentifyOffset);
        Assert.Equal(44, HardInfoLayout.IdeDiskSerialNumberScsiIdentifyOffset);
        Assert.Equal(48, HardInfoLayout.IdeDiskSerialNumberW9xIdentifyOffset);
    }

    // ---------------- Nb30.pas 结构 ----------------

    [Fact]
    public void TLanaEnum_Is256Bytes_LanaAt1()
    {
        // length: UCHAR(1) + lana: array[0..254](255) = 256（MAX_LANA = 254）
        Assert.Equal(256, Marshal.SizeOf<TLanaEnum>());
        Assert.Equal(1, (int)Marshal.OffsetOf<TLanaEnum>(nameof(TLanaEnum.lana)));
        Assert.Equal(1, TLanaEnum.LanaOffset);
        Assert.Equal(256, TLanaEnum.Size);
    }

    [Fact]
    public void TAdapterStatus_Is60Bytes_AdapterAddressAt0()
    {
        // adapter_address[6] + rev_major/reserved0/adapter_type/rev_minor(4) + duration..xmit_aborts(5×2=10)
        // + xmit_success/recv_success(8) + iframe_xmit_err..ti_timeouts(4×2=8) + reserved1(4)
        // + free_ncbs..name_count(10×2=20) = 6+4+10+8+8+4+20 = 60
        Assert.Equal(60, Marshal.SizeOf<TAdapterStatus>());
        Assert.Equal(0, (int)Marshal.OffsetOf<TAdapterStatus>(nameof(TAdapterStatus.adapter_address)));
        Assert.Equal(60, TAdapterStatus.Size);
    }

    [Fact]
    public void TNCB_LeadingFieldsArePlatformIndependent()
    {
        // 原文 :667 的 Ncb（NB30.H 的 NCB）。含指针 ⇒ SizeOf 依 ABI 而变，此处只锁前 4 个字节字段
        // 与 Win32 下 ncb_length 的位置（x86 = 8）。
        Assert.Equal(0, (int)Marshal.OffsetOf<TNCB>(nameof(TNCB.ncb_command)));
        Assert.Equal(1, (int)Marshal.OffsetOf<TNCB>(nameof(TNCB.ncb_retcode)));
        Assert.Equal(2, (int)Marshal.OffsetOf<TNCB>(nameof(TNCB.ncb_lsn)));
        Assert.Equal(3, (int)Marshal.OffsetOf<TNCB>(nameof(TNCB.ncb_num)));
        int expectedLengthOffset = IntPtr.Size == 4 ? 8 : 16;
        Assert.Equal(expectedLengthOffset, (int)Marshal.OffsetOf<TNCB>(nameof(TNCB.ncb_length)));
    }

    // ---------------- Windows.pas 结构 ----------------

    [Fact]
    public void TOSVersionInfo_Is148Bytes()
    {
        // dwOSVersionInfoSize..dwPlatformId = 5 × DWORD = 20；szCSDVersion[128] ⇒ 148
        Assert.Equal(148, Marshal.SizeOf<TOSVersionInfo>());
        Assert.Equal(148, TOSVersionInfo.Size);
        Assert.Equal(20, (int)Marshal.OffsetOf<TOSVersionInfo>(nameof(TOSVersionInfo.szCSDVersion)));
        Assert.Equal(16, (int)Marshal.OffsetOf<TOSVersionInfo>(nameof(TOSVersionInfo.dwPlatformId)));
    }

    [Fact]
    public void TMemoryStatusEx_Is64Bytes()
    {
        // dwLength/dwMemoryLoad(8) + 7 × ULONGLONG(56) = 64
        Assert.Equal(64, Marshal.SizeOf<TMemoryStatusEx>());
        Assert.Equal(64, TMemoryStatusEx.Size);
        Assert.Equal(8, (int)Marshal.OffsetOf<TMemoryStatusEx>(nameof(TMemoryStatusEx.ullTotalPhys)));
    }

    [Fact]
    public void TDisplayDevice_Is424Bytes()
    {
        // cb(4) + DeviceName[32] + DeviceString[128] + StateFlags(4) + DeviceID[128] + DeviceKey[128] = 424
        Assert.Equal(424, Marshal.SizeOf<TDisplayDevice>());
        Assert.Equal(424, TDisplayDevice.Size);
        Assert.Equal(4, (int)Marshal.OffsetOf<TDisplayDevice>(nameof(TDisplayDevice.DeviceName)));
        Assert.Equal(36, (int)Marshal.OffsetOf<TDisplayDevice>(nameof(TDisplayDevice.DeviceString)));
    }

    [Fact]
    public void TDeviceMode_DmDisplayFrequencyAt120()
    {
        // dmDeviceName[32] + 4×WORD(8) + dmFields(4) + 8×SmallInt(16) + 5×SmallInt(10) + dmFormName[32]
        // + dmLogPixels(2) + dmBitsPerPel(4) + dmPelsWidth(4) + dmPelsHeight(4) + dmDisplayFlags(4)
        // → dmDisplayFrequency 在 120（完整 DEVMODEA 为 156，本声明到 dmDisplayFrequency 为止 = 124）
        Assert.Equal(120, (int)Marshal.OffsetOf<TDeviceMode>(nameof(TDeviceMode.dmDisplayFrequency)));
        Assert.Equal(124, Marshal.SizeOf<TDeviceMode>());
    }

    [Fact]
    public void SystemLogicalProcessorInformation_Is24Bytes_OnThisDeclaration()
    {
        // ProcessorMask(4，Delphi 7 x86 的 ULONG_PTR) + Relationship(4) + 联合(16，ULONGLONG[2] 对齐 8) = 24
        // ⇒ 与原文 :346/:360 的 sizeof(...) 一致（x64 原生为 32）。
        Assert.Equal(24, Marshal.SizeOf<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>());
        Assert.Equal(24, HardInfoLayout.SizeOfSystemLogicalProcessorInformation);
        Assert.Equal(0, (int)Marshal.OffsetOf<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>(nameof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION.ProcessorMask)));
        Assert.Equal(4, (int)Marshal.OffsetOf<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>(nameof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION.Relationship)));
        Assert.Equal(8, (int)Marshal.OffsetOf<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>(nameof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION.Union)));
        Assert.Equal(12, Marshal.SizeOf<TCacheDescriptor>());
        Assert.Equal(16, Marshal.SizeOf<TLogicalProcessorInformationUnion>());
    }

    [Fact]
    public void TRegisters_Is16Bytes_FieldOrder()
    {
        Assert.Equal(16, Marshal.SizeOf<TRegisters>());
        Assert.Equal(0, (int)Marshal.OffsetOf<TRegisters>(nameof(TRegisters.EAX)));
        Assert.Equal(4, (int)Marshal.OffsetOf<TRegisters>(nameof(TRegisters.EBX)));
        Assert.Equal(8, (int)Marshal.OffsetOf<TRegisters>(nameof(TRegisters.ECX)));
        Assert.Equal(12, (int)Marshal.OffsetOf<TRegisters>(nameof(TRegisters.EDX)));
    }

    [Fact]
    public void TSystemInfo_X86FieldOrder()
    {
        Assert.Equal(0, (int)Marshal.OffsetOf<TSystemInfo>(nameof(TSystemInfo.dwOemId)));
        Assert.Equal(4, (int)Marshal.OffsetOf<TSystemInfo>(nameof(TSystemInfo.dwPageSize)));
        // x86 = 36（原文的 SizeOf）；x64 = 44 → 因 nint 对齐 8 补到 48。
        // ★ 该声明只在 x86 下与 Win32 SYSTEM_INFO 逐字节一致；接缝的真实实现若跑 x64 需另声明原生布局。
        Assert.Equal(IntPtr.Size == 4 ? 36 : 48, Marshal.SizeOf<TSystemInfo>());
    }

    // ---------------- 常量 ----------------

    [Fact]
    public void Constants_MatchOriginalHexLiterals()
    {
        Assert.Equal(0xEC, HardInfo.IDE_ID_FUNCTION);                    // 原文 :471
        Assert.Equal(0x0007C088u, HardInfo.DFP_RECEIVE_DRIVE_DATA);      // 原文 :473
        Assert.Equal(0x0004D008u, HardInfo.IOCTL_SCSI_MINIPORT);         // 原文 :474
        Assert.Equal(0x001B0501u, HardInfo.IOCTL_SCSI_MINIPORT_IDENTIFY); // 原文 :475
        Assert.Equal(512, HardInfo.IDENTIFY_BUFFER_SIZE);                // 原文 :48/:472
        Assert.Equal(0x37, HardInfo.NCBENUM);                            // NB30 NCBENUM
        Assert.Equal(0x32, HardInfo.NCBRESET);                           // NB30 NCBRESET
        Assert.Equal(0x33, HardInfo.NCBASTAT);                           // NB30 NCBASTAT
        Assert.Equal(2u, HardInfo.VER_PLATFORM_WIN32_NT);                // Windows.pas
        Assert.Equal(122u, HardInfo.ERROR_INSUFFICIENT_BUFFER);
        Assert.Equal(unchecked((long)0x80000002), (long)HardInfo.HKEY_LOCAL_MACHINE);
    }

    [Fact]
    public unsafe void CSDVersionHelpers_AreNulTerminatedAndRespectSecondByteTrap()
    {
        var info = new TOSVersionInfo();
        info.SetCSDVersion(" A ");
        Assert.Equal(" A ", info.CSDVersionString());
        Assert.Equal((byte)' ', info.CSDVersionAt(0));
        Assert.Equal((byte)'A', info.CSDVersionAt(1));

        // 原文 :622 的判据是 szCSDVersion[1]；把 'A' 放到下标 0 时第 2 字节是 0
        info.SetCSDVersion("A");
        Assert.Equal((byte)'A', info.CSDVersionAt(0));
        Assert.Equal((byte)0, info.CSDVersionAt(1));

        info.SetCSDVersion("");
        Assert.Equal("", info.CSDVersionString());
        Assert.Equal((byte)0, info.CSDVersionAt(0));
    }

    [Fact]
    public void DisplayDevice_StringHelpers()
    {
        var d = new TDisplayDevice();
        Assert.Equal("", d.DeviceNameString());
        Assert.Equal("", d.DeviceStringString());
        d.SetDeviceName("\\\\.\\DISPLAY1");
        d.SetDeviceString("NVIDIA GeForce");
        Assert.Equal("\\\\.\\DISPLAY1", d.DeviceNameString());
        Assert.Equal("NVIDIA GeForce", d.DeviceStringString());
    }
}

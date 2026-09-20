// HardInfo.pas 的**接缝行为**断言：用可编程替身驱动原文的控制流（请求结构、分支顺序、缺陷照抄）。
// 接缝的真实 OS 语义（真的去读盘/读注册表）无法在无头 CI 里验证 —— 见 docs/并行报告-p6-core-hardinfo.md。
using System;
using GXX.Core.Hardware;
using Xunit;

namespace GXX.Core.Tests;

public class HardwareSeamTests
{
    private static THardInfoRuntime Rt(THardInfoStubOs os) => THardInfoStub.Runtime(os);

    // =========================================================================================
    // GetIdeSerialNumber —— 原文 :46-184
    // =========================================================================================

    [Fact]
    public void GetIdeSerialNumber_DefaultRuntime_ReturnsEmpty()
    {
        Assert.Equal("", HardInfo.GetIdeSerialNumber());
        Assert.Equal("", HardInfo.GetIdeSerialNumber(Rt(new THardInfoStubOs())));
    }

    [Fact]
    public void GetIdeSerialNumber_BuildsIdentifyRequestAndSwapsSerial()
    {
        var os = new THardInfoStubOs();
        var payload = new byte[HardInfo.IDEOUTCMD_SIZE];
        var serial = System.Text.Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJ");
        Array.Copy(serial, 0, payload, HardInfoLayout.IdeSerialNumberIdentifyOffset + HardInfoLayout.IdSector_sSerialNumber, 20);
        os.OnPhysicalDriveIoControl = (outOffset, outSize, len) => (true, payload);

        var s = HardInfo.GetIdeSerialNumber(Rt(os));

        Assert.Equal("1032547698BADCFEHGJI", s);

        var call = Assert.Single(os.PhysicalDriveCalls);
        Assert.Equal(0, call.outOffset);
        Assert.Equal(HardInfo.IDEOUTCMD_SIZE, call.outSize);
        Assert.Equal(HardInfo.IDEOUTCMD_SIZE, call.bufferLength);
        Assert.Equal(HardInfoLayout.SendCmdInParamsSize - 1, call.inSize); // 原文 `SizeOf(TSendCmdInParams) - 1` = 32
        Assert.Equal(512u, call.scip.cBufferSize);
        Assert.Equal(1, call.scip.irDriveRegs.bSectorCountReg);
        Assert.Equal(1, call.scip.irDriveRegs.bSectorNumberReg);
        Assert.Equal(0xA0, call.scip.irDriveRegs.bDriveHeadReg);
        Assert.Equal(0xEC, call.scip.irDriveRegs.bCommandReg);
        Assert.Equal(0, call.scip.irDriveRegs.bFeaturesReg);
        Assert.Equal(0, call.scip.bDriveNumber);
    }

    [Fact]
    public void GetIdeSerialNumber_NulTerminatorOverrunsIntoWBufferType()
    {
        // ★ 原文缺陷（:181）锁死：NUL 写在 sSerialNumber + 20 = wBufferType 的低字节（缓冲内偏移 16+40）
        var os = new THardInfoStubOs();
        var payload = new byte[HardInfo.IDEOUTCMD_SIZE];
        payload[HardInfoLayout.IdeSerialNumberIdentifyOffset + HardInfoLayout.IdSector_wBufferType] = 0xAB; // wBufferType 低字节
        os.OnPhysicalDriveIoControl = (o, s, l) => (true, payload);

        HardInfo.GetIdeSerialNumber(Rt(os));

        Assert.Equal(0, os.PhysicalDriveBufferRef[HardInfoLayout.IdeSerialNumberIdentifyOffset + HardInfoLayout.IdSector_wBufferType]);
        // wBufferType 的高字节未被触碰
        Assert.Equal(payload[HardInfoLayout.IdeSerialNumberIdentifyOffset + HardInfoLayout.IdSector_wBufferType + 1],
            os.PhysicalDriveBufferRef[HardInfoLayout.IdeSerialNumberIdentifyOffset + HardInfoLayout.IdSector_wBufferType + 1]);
    }

    [Fact]
    public void GetIdeSerialNumber_StopsAtEmbeddedNul_UnlikeSetString()
    {
        // ★ 差异断言：PChar → string 截断于 #0（对比 GetIdeDiskSerialNumber 的 SetString 定长语义）
        var os = new THardInfoStubOs();
        var payload = new byte[HardInfo.IDEOUTCMD_SIZE];
        int serialAt = HardInfoLayout.IdeSerialNumberIdentifyOffset + HardInfoLayout.IdSector_sSerialNumber;
        payload[serialAt + 0] = (byte)'A';
        payload[serialAt + 1] = (byte)'B';
        payload[serialAt + 2] = 0;      // [3] 也是 0
        os.OnPhysicalDriveIoControl = (o, s, l) => (true, payload);

        // 交换后：[0]='B' [1]='A' [2]=0 ⇒ C 串 = "BA"
        Assert.Equal("BA", HardInfo.GetIdeSerialNumber(Rt(os)));
    }

    [Fact]
    public void GetIdeSerialNumber_IoFailureReturnsEmpty()
    {
        var os = new THardInfoStubOs { OnPhysicalDriveIoControl = (o, s, l) => (false, null) };
        Assert.Equal("", HardInfo.GetIdeSerialNumber(Rt(os)));
    }

    [Fact]
    public void GetIdeSerialNumber_Win9xBranchUsesSmartVsd()
    {
        var os = new THardInfoStubOs { Win32PlatformValue = 1 }; // VER_PLATFORM_WIN32_WINDOWS
        var payload = new byte[HardInfo.IDEOUTCMD_SIZE];
        os.OnSmartVsdIoControl = (outOffset, outSize, len) => (true, payload);

        // 全 0 缓冲 ⇒ 首字节即 #0 ⇒ PChar→string 为空
        Assert.Equal("", HardInfo.GetIdeSerialNumber(Rt(os)));

        Assert.Empty(os.PhysicalDriveCalls);
        var call = Assert.Single(os.SmartVsdCalls);
        Assert.Equal(0, call.outOffset);
        Assert.Equal(HardInfo.IDEOUTCMD_SIZE, call.outSize);
        Assert.Equal(HardInfoLayout.SendCmdInParamsSize - 1, call.inSize);
    }

    // =========================================================================================
    // GetIdeDiskSerialNumber —— 原文 :396-595
    // =========================================================================================

    /// <summary>构造 572 字节的 SCSI miniport 原始缓冲，IDENTIFY 扇区在 <paramref name="identifyOffset"/>。</summary>
    private static byte[] ScsiBuffer(int identifyOffset)
    {
        int b = identifyOffset;
        var buf = new byte[HardInfo.SCSI_BufferSize];
        var serial = System.Text.Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJ");
        Array.Copy(serial, 0, buf, b + HardInfoLayout.IdSector_sSerialNumber, 20);
        var model = System.Text.Encoding.ASCII.GetBytes("0123456789012345678901234567890123456789");
        Assert.Equal(40, model.Length);
        Array.Copy(model, 0, buf, b + HardInfoLayout.IdSector_sModelNumber, 40);
        var fw = System.Text.Encoding.ASCII.GetBytes("FW123456");
        Array.Copy(fw, 0, buf, b + HardInfoLayout.IdSector_sFirmwareRev, 8);
        // wNumCurrentSectorsPerTrack @ +112 = 01 02 ⇒ 交换相邻对得 02 01 ⇒ 小端读 = 0x0102
        buf[b + HardInfoLayout.IdSector_wNumCurrentSectorsPerTrack + 0] = 0x01;
        buf[b + HardInfoLayout.IdSector_wNumCurrentSectorsPerTrack + 1] = 0x02;
        // ulCurrentSectorCapacity @ +114 = AA BB CC DD ⇒ 交换相邻对得 BB AA DD CC ⇒ 小端读 = 0xCCDDAABB
        buf[b + HardInfoLayout.IdSector_ulCurrentSectorCapacity + 0] = 0xAA;
        buf[b + HardInfoLayout.IdSector_ulCurrentSectorCapacity + 1] = 0xBB;
        buf[b + HardInfoLayout.IdSector_ulCurrentSectorCapacity + 2] = 0xCC;
        buf[b + HardInfoLayout.IdSector_ulCurrentSectorCapacity + 3] = 0xDD;
        // ulTotalAddressableSectors @ +120 = 11 22 33 44 ⇒ 交换相邻对得 22 11 44 33 ⇒ 小端读 = 0x33441122
        buf[b + HardInfoLayout.IdSector_ulTotalAddressableSectors + 0] = 0x11;
        buf[b + HardInfoLayout.IdSector_ulTotalAddressableSectors + 1] = 0x22;
        buf[b + HardInfoLayout.IdSector_ulTotalAddressableSectors + 2] = 0x33;
        buf[b + HardInfoLayout.IdSector_ulTotalAddressableSectors + 3] = 0x44;
        return buf;
    }

    [Fact]
    public void GetIdeDiskSerialNumber_NtBranch_DecodesAllSixFields()
    {
        var os = new THardInfoStubOs();
        var buf = ScsiBuffer(HardInfoLayout.IdeDiskSerialNumberScsiIdentifyOffset);
        os.OnScsiMiniportIoControl = bufferSize => (true, buf);

        string serial = "原值序列号", model = "原值型号", fw = "原值固件";
        uint total = 111, capacity = 222;
        ushort spt = 333;

        Assert.True(HardInfo.GetIdeDiskSerialNumber(ref serial, ref model, ref fw, ref total, ref capacity, ref spt, Rt(os)));

        Assert.Equal("1032547698BADCFEHGJI", serial);
        Assert.Equal("1032547698103254769810325476981032547698", model);
        Assert.Equal("WF214365", fw);
        // ★ ChangeByteOrder 只交换**相邻字节对**（不是整段反转）⇒ 读小端得到：
        //   输入 [11,22,33,44] → [22,11,44,33] → LE = 0x33441122
        //   输入 [AA,BB,CC,DD] → [BB,AA,DD,CC] → LE = 0xCCDDAABB
        //   输入 [01,02]       → [02,01]       → LE = 0x0102
        Assert.Equal(0x33441122u, total);
        Assert.Equal(0xCCDDAABBu, capacity);
        Assert.Equal((ushort)0x0102, spt);

        var call = Assert.Single(os.ScsiMiniportCalls);
        Assert.Equal(HardInfo.SCSI_BufferSize, call.bufferSize);
        // 端口构造的 SRB 头（克隆取自调用时）
        Assert.Equal(28u, ReadU32(call.bufferAtCall, 0));                       // HeaderLength = sizeof(SRB_IO_CONTROL)
        Assert.Equal("SCSIDISK", System.Text.Encoding.ASCII.GetString(call.bufferAtCall, 4, 8));
        Assert.Equal(2u, ReadU32(call.bufferAtCall, 12));                       // Timeout
        Assert.Equal(HardInfo.IOCTL_SCSI_MINIPORT_IDENTIFY, ReadU32(call.bufferAtCall, 16));
        Assert.Equal((uint)HardInfo.SCSI_DataSize, ReadU32(call.bufferAtCall, 24));
        // 位于 +28 的 SENDCMDINPARAMS
        Assert.Equal(512u, ReadU32(call.bufferAtCall, 28 + 0));                 // cBufferSize
        Assert.Equal(1, call.bufferAtCall[28 + 4 + 1]);                         // bSectorCountReg
        Assert.Equal(0xA0, call.bufferAtCall[28 + 4 + 5]);                      // bDriveHeadReg
        Assert.Equal(0xEC, call.bufferAtCall[28 + 4 + 6]);                      // bCommandReg
    }

    [Fact]
    public void GetIdeDiskSerialNumber_Win9xBranch_UsesOffset48()
    {
        var os = new THardInfoStubOs { Win32PlatformValue = 1 };
        var buf = ScsiBuffer(HardInfoLayout.IdeDiskSerialNumberW9xIdentifyOffset);
        os.OnSmartVsdIoControl = (outOffset, outSize, len) => (true, buf);

        string serial = "", model = "", fw = "";
        uint total = 0, capacity = 0;
        ushort spt = 0;
        Assert.True(HardInfo.GetIdeDiskSerialNumber(ref serial, ref model, ref fw, ref total, ref capacity, ref spt, Rt(os)));

        Assert.Equal("1032547698BADCFEHGJI", serial);
        Assert.Equal((ushort)0x0102, spt);

        Assert.Empty(os.ScsiMiniportCalls);
        var call = Assert.Single(os.SmartVsdCalls);
        // ★ 原文缺陷：Win9x 分支的 outOffset 是 @pInData^.bBuffer = 32（不是 0），IDENTIFY 因而是 +48 而不是 +44
        Assert.Equal(HardInfoLayout.SendCmdInParams_bBuffer, call.outOffset);
        Assert.Equal(HardInfo.W9xBufferSize, call.outSize);
    }

    [Fact]
    public void GetIdeDiskSerialNumber_FailureLeavesRefParametersUntouched()
    {
        // ★ 差异断言：原文是 `var` 形参 ⇒ 失败路径不書き換；故托管用 `ref` 而不是 `out`
        var os = new THardInfoStubOs { OnScsiMiniportIoControl = _ => (false, null) };
        string serial = "KEEP-S", model = "KEEP-M", fw = "KEEP-F";
        uint total = 7, capacity = 8;
        ushort spt = 9;

        Assert.False(HardInfo.GetIdeDiskSerialNumber(ref serial, ref model, ref fw, ref total, ref capacity, ref spt, Rt(os)));
        Assert.Equal("KEEP-S", serial);
        Assert.Equal("KEEP-M", model);
        Assert.Equal("KEEP-F", fw);
        Assert.Equal(7u, total);
        Assert.Equal(8u, capacity);
        Assert.Equal((ushort)9, spt);
    }

    [Fact]
    public void GetIdeDiskSerialNumber_DefaultRuntimeReturnsFalse()
    {
        string serial = "S", model = "M", fw = "F";
        uint total = 1, capacity = 2;
        ushort spt = 3;
        Assert.False(HardInfo.GetIdeDiskSerialNumber(ref serial, ref model, ref fw, ref total, ref capacity, ref spt));
        Assert.Equal("S", serial);
        Assert.Equal(1u, total);
        Assert.Equal((ushort)3, spt);
    }

    [Fact]
    public void GetIdeDiskSerialNumber_SetStringDoesNotTruncateAtNul()
    {
        // ★ 差异断言：SetString(S, Buf, 20) 复制**恰好 20 字节**，不在 #0 处截断（与 GetIdeSerialNumber 相反）
        var os = new THardInfoStubOs();
        var buf = new byte[HardInfo.SCSI_BufferSize];
        int b = HardInfoLayout.IdeDiskSerialNumberScsiIdentifyOffset;
        // 输入 [0..3] = 'A', 0, 'B', 0 ⇒ 交换后 = 0, 'A', 0, 'B'
        buf[b + HardInfoLayout.IdSector_sSerialNumber + 0] = (byte)'A';
        buf[b + HardInfoLayout.IdSector_sSerialNumber + 1] = 0;
        buf[b + HardInfoLayout.IdSector_sSerialNumber + 2] = (byte)'B';
        buf[b + HardInfoLayout.IdSector_sSerialNumber + 3] = 0;
        os.OnScsiMiniportIoControl = _ => (true, buf);

        string serial = "", model = "", fw = "";
        uint total = 0, capacity = 0;
        ushort spt = 0;
        Assert.True(HardInfo.GetIdeDiskSerialNumber(ref serial, ref model, ref fw, ref total, ref capacity, ref spt, Rt(os)));

        Assert.Equal(20, serial.Length);
        Assert.Equal('\0', serial[0]);
        Assert.Equal('A', serial[1]);
        Assert.Equal('\0', serial[2]);
        Assert.Equal('B', serial[3]);
    }

    // 注：HardInfo.EnsureIdentifyBuffer（接缝返回缓冲过短的守卫）**无测试覆盖** ——
    //     原始缓冲由本移植按原文固定尺寸自行分配（528 / 572），守卫在现有尺寸下不可达；
    //     保留它只为在接缝实现违约时给出明确错误而非静默越界读。已登记在报告中。

    private static uint ReadU32(byte[] b, int o) => (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));

    // =========================================================================================
    // GetDisplayFrequency / GetDisplayDevice —— 原文 :207-231
    // =========================================================================================

    [Fact]
    public void GetDisplayFrequency_DefaultIsZero_AndModeNumIsMinus1()
    {
        var os = new THardInfoStubOs();
        Assert.Equal(0, HardInfo.GetDisplayFrequency(Rt(os)));
        Assert.Equal(uint.MaxValue, Assert.Single(os.EnumDisplaySettingsCalls).modeNum);
    }

    [Fact]
    public void GetDisplayFrequency_ReturnsDmDisplayFrequency()
    {
        var os = new THardInfoStubOs();
        TDeviceMode mode = default;
        mode.dmDisplayFrequency = 144;
        os.OnEnumDisplaySettings = _ => (true, mode);
        Assert.Equal(144, HardInfo.GetDisplayFrequency(Rt(os)));
    }

    [Fact]
    public void GetDisplayFrequency_IgnoresEnumFailure()
    {
        // ★ 差异断言：原文 :211 丢弃 EnumDisplaySettings 的 Boolean ⇒ 失败时依旧返回结构里的值
        var os = new THardInfoStubOs();
        TDeviceMode mode = default;
        mode.dmDisplayFrequency = 75;
        os.OnEnumDisplaySettings = _ => (false, mode);
        Assert.Equal(75, HardInfo.GetDisplayFrequency(Rt(os)));
    }

    [Fact]
    public void GetDisplayDevice_DefaultIsEmpty()
    {
        Assert.Equal("", HardInfo.GetDisplayDevice());
        Assert.Equal("", HardInfo.GetDisplayDevice(Rt(new THardInfoStubOs())));
    }

    [Fact]
    public void GetDisplayDevice_PicksLastMatchingDeviceAndSetsCbOnce()
    {
        var os = new THardInfoStubOs();
        os.OnEnumDisplayDevices = devNum => devNum switch
        {
            0 => (true, Device("\\\\.\\DISPLAY2", "Intel HD")),
            1 => (true, Device("\\\\.\\DISPLAY1", "NVIDIA GeForce")),
            _ => (false, default),
        };

        Assert.Equal("NVIDIA GeForce", HardInfo.GetDisplayDevice(Rt(os)));

        Assert.Equal(3, os.EnumDisplayDevicesCalls.Count);
        Assert.Equal(new uint[] { 0, 1, 2 }, os.EnumDisplayDevicesCalls.ConvertAll(c => c.devNum).ToArray());
        Assert.All(os.EnumDisplayDevicesCalls, c => Assert.Equal(424u, c.cb));   // cb := sizeof(lpDisplayDevice)
        Assert.All(os.EnumDisplayDevicesCalls, c => Assert.Equal(0u, c.flags));  // dwFlags := 0
    }

    [Fact]
    public void GetDisplayDevice_IsCaseSensitive()
    {
        var os = new THardInfoStubOs();
        os.OnEnumDisplayDevices = devNum => devNum == 0
            ? (true, Device("\\\\.\\display1", "Should Not Match"))
            : (false, default);
        Assert.Equal("", HardInfo.GetDisplayDevice(Rt(os)));
    }

    [Fact]
    public void GetDisplayDevice_NoMatchReturnsEmpty()
    {
        var os = new THardInfoStubOs();
        os.OnEnumDisplayDevices = devNum => devNum == 0
            ? (true, Device("\\\\.\\DISPLAY9", "X"))
            : (false, default);
        Assert.Equal("", HardInfo.GetDisplayDevice(Rt(os)));
    }

    private static TDisplayDevice Device(string name, string text)
    {
        var d = new TDisplayDevice();
        d.SetDeviceName(name);
        d.SetDeviceString(text);
        return d;
    }

    // =========================================================================================
    // GetMemorySize —— 原文 :287-303
    // =========================================================================================

    [Fact]
    public void GetMemorySize_DefaultIsZeroBytes()
    {
        Assert.Equal("0Bytes", HardInfo.GetMemorySize());
        var os = new THardInfoStubOs();
        Assert.Equal("0Bytes", HardInfo.GetMemorySize(Rt(os)));
        Assert.Equal(64u, Assert.Single(os.MemoryCalls)); // dwLength := SizeOf(TMemoryStatusEx)
    }

    [Theory]
    [InlineData(2UL * 1024 * 1024 * 1024, "2GB")]
    [InlineData(1UL * 1024 * 1024 * 1024, "1024MB")]      // ★ 恰好 1GiB 走 MB 档
    [InlineData(16UL * 1024 * 1024 * 1024, "16GB")]
    [InlineData(512UL * 1024 * 1024, "512MB")]
    public void GetMemorySize_FromSeamValue(ulong totalPhys, string expected)
    {
        var os = new THardInfoStubOs();
        os.OnGlobalMemoryStatusEx = () =>
        {
            var m = new TMemoryStatusEx();
            m.ullTotalPhys = totalPhys;
            return (true, m);
        };
        Assert.Equal(expected, HardInfo.GetMemorySize(Rt(os)));
    }

    // =========================================================================================
    // GetCpuName / GetCPUID —— 原文 :253-324
    // =========================================================================================

    [Fact]
    public void GetCpuName_RequestsThreeExtendedLeaves()
    {
        var os = new THardInfoStubOs();
        HardInfo.GetCpuName(Rt(os));
        Assert.Equal(3, os.CpuIdCalls.Count);
        Assert.Equal(0x80000002u, os.CpuIdCalls[0].param);
        Assert.Equal(0x80000003u, os.CpuIdCalls[1].param);
        Assert.Equal(0x80000004u, os.CpuIdCalls[2].param);
    }

    [Fact]
    public void GetCpuName_AssemblesRegistersFromSeam()
    {
        var os = new THardInfoStubOs();
        os.OnCpuId = param => param switch
        {
            0x80000002u => new TRegisters { EAX = Le("Genu"), EBX = Le("ineI"), ECX = Le("ntel"), EDX = 0 },
            _ => default,
        };
        Assert.Equal("GenuineIntel", HardInfo.GetCpuName(Rt(os)));
    }

    [Fact]
    public void GetCpuName_DefaultRuntimeReturnsEmpty()
    {
        Assert.Equal("", HardInfo.GetCpuName());
        Assert.Equal("", HardInfo.GetCpuName(Rt(new THardInfoStubOs())));
    }

    [Fact]
    public void GetCPUID_PassesParamThroughToSeam()
    {
        var os = new THardInfoStubOs();
        os.OnCpuId = param => new TRegisters { EAX = param };
        HardInfo.GetCPUID(0x80000002u, out var regs, Rt(os));
        Assert.Equal(0x80000002u, regs.EAX);
        Assert.Equal(0x80000002u, Assert.Single(os.CpuIdCalls).param);
    }

    [Fact]
    public void GetCPUID_DefaultIsZeroedRegisters()
    {
        HardInfo.GetCPUID(0x80000002u, out var regs);
        Assert.Equal(0u, regs.EAX);
        Assert.Equal(0u, regs.EBX);
        Assert.Equal(0u, regs.ECX);
        Assert.Equal(0u, regs.EDX);
    }

    private static uint Le(string four)
    {
        var b = System.Text.Encoding.ASCII.GetBytes(four);
        return (uint)(b[0] | (b[1] << 8) | (b[2] << 16) | (b[3] << 24));
    }

    // =========================================================================================
    // GetCpuIDstringEx —— 原文 :387-394
    // =========================================================================================

    [Fact]
    public void GetCpuIDstringEx_FromSeam()
    {
        var os = new THardInfoStubOs();
        os.OnGetSystemInfo = () => new TSystemInfo
        {
            dwOemId = 0,
            dwNumberOfProcessors = 8,
            dwProcessorType = 586,
            wProcessorRevision = 9728,
        };
        Assert.Equal("085869728", HardInfo.GetCpuIDstringEx(Rt(os)));
    }

    [Fact]
    public void GetCpuIDstringEx_DefaultIsFourZeros()
    {
        Assert.Equal("0000", HardInfo.GetCpuIDstringEx());
    }

    // =========================================================================================
    // GetCPUCount —— 原文 :327-382
    // =========================================================================================

    private static SYSTEM_LOGICAL_PROCESSOR_INFORMATION E(TLogicalProcessorRelationship rel, uint mask = 0)
        => new() { Relationship = rel, ProcessorMask = mask };

    [Fact]
    public void GetCPUCount_DefaultRuntimeReturnsEmptyString()
    {
        // 默认接缝：第一次调用失败 + ERROR_INSUFFICIENT_BUFFER ⇒ 扩容到 2 ⇒ 第二次仍失败 ⇒ 原文 :350 `Exit`
        Assert.Equal("", HardInfo.GetCPUCount());
        var os = new THardInfoStubOs();
        Assert.Equal("", HardInfo.GetCPUCount(Rt(os)));
        Assert.Equal(new[] { 1, 2 }, os.GLPIBufferLengths.ToArray());
        Assert.Equal(new uint[] { 24, 24 }, os.GLPICalls.ToArray());
    }

    [Fact]
    public void GetCPUCount_RetriesWithResizedBuffer()
    {
        var os = new THardInfoStubOs();
        int call = 0;
        var entries = new[]
        {
            E(TLogicalProcessorRelationship.RelationProcessorCore),
            E(TLogicalProcessorRelationship.RelationProcessorCore),
        };
        os.OnGLPI = (len, rl) =>
        {
            call++;
            if (call == 1) return (false, 48u, null);   // API 写入所需字节数
            return (true, 48u, entries);
        };
        os.OnGetLastError = () => HardInfo.ERROR_INSUFFICIENT_BUFFER;

        Assert.Equal("NumaNodes=0 PhysicalProcessorPackages=0 ProcessorCores=2 LogicalProcessors=0",
            HardInfo.GetCPUCount(Rt(os)));
        Assert.Equal(new[] { 1, 3 }, os.GLPIBufferLengths.ToArray()); // 48 / 24 + 1 = 3
        Assert.Equal(new uint[] { 24, 48 }, os.GLPICalls.ToArray());
    }

    [Fact]
    public void GetCPUCount_FirstCallSucceeds_OnlyOneEntryExamined()
    {
        // ★ 原文缺陷：第一次调用成功时 ReturnLength 不会被改写为真实大小 ⇒ Count 恒 = 1
        var os = new THardInfoStubOs();
        os.OnGLPI = (len, rl) => (true, rl, new[]
        {
            E(TLogicalProcessorRelationship.RelationNumaNode),
            E(TLogicalProcessorRelationship.RelationNumaNode),
            E(TLogicalProcessorRelationship.RelationNumaNode),
        });
        Assert.Equal("NumaNodes=1 PhysicalProcessorPackages=0 ProcessorCores=0 LogicalProcessors=0",
            HardInfo.GetCPUCount(Rt(os)));
        Assert.Equal(new[] { 1 }, os.GLPIBufferLengths.ToArray());
    }

    [Fact]
    public void GetCPUCount_OtherErrorSkipsResize()
    {
        var os = new THardInfoStubOs();
        os.OnGLPI = (len, rl) => (false, rl, null);
        os.OnGetLastError = () => 5; // ERROR_ACCESS_DENIED —— 不是 ERROR_INSUFFICIENT_BUFFER
        // 不扩容 ⇒ Count = 24 / 24 = 1，读 Buffer[0]（default ⇒ RelationProcessorCore）
        Assert.Equal("NumaNodes=0 PhysicalProcessorPackages=0 ProcessorCores=1 LogicalProcessors=0",
            HardInfo.GetCPUCount(Rt(os)));
        Assert.Equal(new[] { 1 }, os.GLPIBufferLengths.ToArray());
    }

    [Fact]
    public void GetCPUCount_CountExceedingBufferLengthThrows()
    {
        // ★ 原文缺陷：Count := ReturnLength div sizeof 未与 Length(Buffer) 复核 ⇒ 原文越权读；
        //   托管侧抛 IndexOutOfRangeException
        var os = new THardInfoStubOs();
        os.OnGLPI = (len, rl) => (true, 72u, new[] { E(TLogicalProcessorRelationship.RelationProcessorCore) });
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.GetCPUCount(Rt(os)));
    }

    // =========================================================================================
    // GetWindowsVersion —— 原文 :597-660
    // =========================================================================================

    [Fact]
    public void GetWindowsVersion_PassesStructSizeAndMapsVersion()
    {
        var os = new THardInfoStubOs();
        os.OnGetVersionEx = _ =>
        {
            var v = new TOSVersionInfo();
            v.dwPlatformId = 2;
            v.dwMajorVersion = 5;
            v.dwMinorVersion = 0;
            v.dwBuildNumber = 2195;
            v.SetCSDVersion("Service Pack 4");
            return (true, v);
        };

        Assert.Equal("Windows 2000 Service pack: Service Pack 4, Build: 2195", HardInfo.GetWindowsVersion(Rt(os)));
        Assert.Equal(148u, Assert.Single(os.VersionCalls).dwOSVersionInfoSize); // SizeOf(VersionInfo) —— 原文 :604
    }

    [Fact]
    public void GetWindowsVersion_DefaultRuntimeYieldsPlatformZeroBranch()
    {
        // 默认接缝不改动结构 ⇒ dwPlatformId = 0 ⇒ 'Windows 3.11'
        Assert.Equal("Windows 3.11, Build: 0", HardInfo.GetWindowsVersion());
    }

    [Fact]
    public void GetWindowsVersion_IgnoresGetVersionExReturnValue()
    {
        // ★ 差异断言：原文 :606 丢弃返回值 ⇒ 失败也照常映射结构内容
        var os = new THardInfoStubOs();
        os.OnGetVersionEx = _ =>
        {
            var v = new TOSVersionInfo();
            v.dwPlatformId = 3;
            v.dwBuildNumber = 42;
            return (false, v);
        };
        Assert.Equal("Unknown Platform, Build: 42", HardInfo.GetWindowsVersion(Rt(os)));
    }

    // =========================================================================================
    // GetAdapterMac —— 原文 :664-705
    // =========================================================================================

    [Fact]
    public void GetAdapterMac_DefaultRuntimeReturnsEmpty()
    {
        Assert.Equal("", HardInfo.GetAdapterMac(0));
        Assert.Equal("", HardInfo.GetAdapterMac(0, Rt(new THardInfoStubOs())));
    }

    [Fact]
    public void GetAdapterMac_FullCommandSequenceAndMac()
    {
        var os = new THardInfoStubOs();
        os.OnNetBios = (cmd, len, buf) =>
        {
            switch (cmd)
            {
                case HardInfo.NCBENUM:
                    if (buf != null) { buf[0] = 2; buf[1] = 0x00; buf[2] = 0x07; }
                    return 0;
                case HardInfo.NCBRESET:
                    return 0;
                case HardInfo.NCBASTAT:
                    if (buf != null) { buf[0] = 0xDE; buf[1] = 0xAD; buf[2] = 0xBE; buf[3] = 0xEF; buf[4] = 0x00; buf[5] = 0x11; }
                    return 0;
                default:
                    return 1;
            }
        };

        Assert.Equal("DEADBEEF0011", HardInfo.GetAdapterMac(1, Rt(os)));

        Assert.Equal(4, os.NetBiosCalls.Count);
        // ★ 原文缺陷 1 锁死：第一次 NCBENUM 在 ncb_buffer/ncb_length 尚未设置时发出（注定失败的多余调用）
        Assert.Equal(HardInfo.NCBENUM, os.NetBiosCalls[0].command);
        Assert.Equal(0, os.NetBiosCalls[0].length);
        Assert.Equal(0, (long)os.NetBiosCalls[0].buffer);
        Assert.Null(os.NetBiosCalls[0].payload);
        // 第二次才是真正的枚举
        Assert.Equal(HardInfo.NCBENUM, os.NetBiosCalls[1].command);
        Assert.Equal(256, os.NetBiosCalls[1].length);
        Assert.NotEqual(0, (long)os.NetBiosCalls[1].buffer);
        Assert.NotNull(os.NetBiosCalls[1].payload);
        // Reset / Astat 用的是 lana[1] = 0x07
        Assert.Equal(HardInfo.NCBRESET, os.NetBiosCalls[2].command);
        Assert.Equal(0x07, os.NetBiosCalls[2].lana);
        Assert.Equal(HardInfo.NCBASTAT, os.NetBiosCalls[3].command);
        Assert.Equal(0x07, os.NetBiosCalls[3].lana);
        Assert.Equal(60, os.NetBiosCalls[3].length);              // SizeOf(Adapter) = 60
        Assert.NotEqual(0, (long)os.NetBiosCalls[3].buffer);
        // StrPcopy(Ncb.ncb_callname, '*')
        Assert.Equal("*", os.NetBiosCallNames[3]);
        Assert.Equal("", os.NetBiosCallNames[0]);
    }

    [Fact]
    public void GetAdapterMac_EnumFailureReturnsEmpty()
    {
        var os = new THardInfoStubOs { OnNetBios = (c, l, b) => 1 };
        Assert.Equal("", HardInfo.GetAdapterMac(0, Rt(os)));
        // ★ 原文 :677-681 恒发两次 NCBENUM（第一次的返回码被丢弃，第二次才检查）
        Assert.Equal(2, os.NetBiosCalls.Count);
        Assert.Equal(HardInfo.NCBENUM, os.NetBiosCalls[0].command);
        Assert.Equal(HardInfo.NCBENUM, os.NetBiosCalls[1].command);
    }

    [Fact]
    public void GetAdapterMac_ResetFailureReturnsEmpty()
    {
        var os = new THardInfoStubOs();
        os.OnNetBios = (cmd, len, buf) => cmd == HardInfo.NCBENUM ? (byte)0 : (byte)0x03;
        Assert.Equal("", HardInfo.GetAdapterMac(0, Rt(os)));
        Assert.Equal(3, os.NetBiosCalls.Count);
    }

    [Fact]
    public void GetAdapterMac_AstatFailureIsSilentlyIgnored()
    {
        // ★ 原文缺陷 2 锁死：NCBASTAT（:695）的返回码被丢弃 ⇒ 缓冲区保持全 0 ⇒ '000000000000'
        var os = new THardInfoStubOs();
        os.OnNetBios = (cmd, len, buf) => cmd == HardInfo.NCBASTAT ? (byte)0x17 : (byte)0;
        Assert.Equal("000000000000", HardInfo.GetAdapterMac(0, Rt(os)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(255)]
    [InlineData(int.MaxValue)]
    public void GetAdapterMac_LanaIndexOutOfRangeThrows(int aNo)
    {
        // ★ 原文缺陷 3：Lanaenum.lana[aNo] 无边界检查；托管侧抛异常
        var os = new THardInfoStubOs();
        os.OnNetBios = (cmd, len, buf) =>
        {
            if (cmd == HardInfo.NCBENUM && buf != null) { buf[0] = 1; buf[1] = 0x00; }
            return 0;
        };
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.GetAdapterMac(aNo, Rt(os)));
    }

    // =========================================================================================
    // GetNetCardName —— 原文 :707-773
    // =========================================================================================

    private const string NetKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\NetworkCards\\";

    [Fact]
    public void GetNetCardName_DefaultRuntimeReturnsEmpty()
    {
        Assert.Equal("", HardInfo.GetNetCardName());
        Assert.Equal("", HardInfo.GetNetCardName(Rt(new THardInfoStubOs())));
    }

    [Fact]
    public void GetNetCardName_ReadsFirstOpenableServiceName()
    {
        var os = new THardInfoStubOs();
        os.OnRegOpenKeyEx = (root, name) => (name == NetKey, 0x1234);
        os.OnRegEnum = (h, i, isKey) => i switch
        {
            0 => (HardInfo.ERROR_SUCCESS, "0000"),
            1 => (HardInfo.ERROR_SUCCESS, "0001"),
            _ => (HardInfo.ERROR_NO_MORE_ITEMS, (string)null),
        };
        os.OnRegistryOpenKey = (reg, key, canCreate) => key == NetKey + "0001";
        os.OnRegistryReadString = (reg, name) => name == "ServiceName" ? "E1000" : "";

        Assert.Equal("E1000", HardInfo.GetNetCardName(Rt(os)));
        var open = Assert.Single(os.RegOpenKeyExCalls);
        Assert.Equal(NetKey, open.name);
        Assert.Equal(HardInfo.KEY_READ, open.sam);                 // 原文 :719 的 KEY_READ
        Assert.Equal((long)HardInfo.HKEY_LOCAL_MACHINE, (long)open.root);
        Assert.All(os.RegEnumCalls, c => Assert.True(c.isKey));    // RegEnum(..., DoKeys := True)
        Assert.Equal(new[] { NetKey + "0000", NetKey + "0001" },
            os.RegistryOpenKeyCalls.ConvertAll(c => c.key).ToArray());
        Assert.All(os.RegistryOpenKeyCalls, c => Assert.False(c.canCreate)); // OpenKey(..., False)
        Assert.Equal(new[] { "ServiceName" }, os.RegistryReadStringCalls.ToArray());
    }

    [Fact]
    public void GetNetCardName_NoOpenableKeyReturnsEmpty()
    {
        var os = new THardInfoStubOs();
        os.OnRegOpenKeyEx = (root, name) => (true, 0x1234);
        os.OnRegEnum = (h, i, isKey) => i == 0
            ? (HardInfo.ERROR_SUCCESS, "0000")
            : (HardInfo.ERROR_NO_MORE_ITEMS, (string)null);
        os.OnRegistryOpenKey = (reg, key, canCreate) => false;

        Assert.Equal("", HardInfo.GetNetCardName(Rt(os)));
        Assert.Empty(os.RegistryReadStringCalls);
    }

    [Fact]
    public void GetNetCardName_EmptyEnumerationReturnsEmpty()
    {
        var os = new THardInfoStubOs();
        os.OnRegOpenKeyEx = (root, name) => (true, 0x1234);
        os.OnRegEnum = (h, i, isKey) => (HardInfo.ERROR_NO_MORE_ITEMS, (string)null);
        Assert.Equal("", HardInfo.GetNetCardName(Rt(os)));
        Assert.Empty(os.RegistryOpenKeyCalls);
    }

    // =========================================================================================
    // GetNetCardMac —— 原文 :775-813
    // =========================================================================================

    [Fact]
    public void GetNetCardMac_DefaultRuntimeReturnsEmpty()
    {
        Assert.Equal("", HardInfo.GetNetCardMac("Nic"));
        Assert.Equal("", HardInfo.GetNetCardMac("Nic", Rt(new THardInfoStubOs())));
    }

    [Fact]
    public void GetNetCardMac_QueriesPermanentAddress()
    {
        var os = new THardInfoStubOs();
        var mac = new byte[256];
        mac[0] = 0xDE; mac[1] = 0xAD; mac[2] = 0xBE; mac[3] = 0xEF; mac[4] = 0x00; mac[5] = 0x11;
        os.OnNdis = oid => (true, mac, 6u);

        Assert.Equal("DEADBEEF0011", HardInfo.GetNetCardMac("MyNic", Rt(os)));

        var call = Assert.Single(os.NdisCalls);
        Assert.Equal("\\\\.\\MyNic", call.device);                 // CreateFile('\\.\' + NetCardName)
        Assert.Equal(0x00170002, call.ioctl);                      // IOCTL_NDIS_QUERY_GLOBAL_STATS
        Assert.Equal(0x01010101, call.oid);                        // OID_802_3_PERMANENT_ADDRESS
        Assert.Equal(256, call.outLength);                         // outBuf: array[1..256]
    }

    [Fact]
    public void GetNetCardMac_ZeroBytesReturnedIsEmpty()
    {
        var os = new THardInfoStubOs { OnNdis = oid => (true, new byte[256], 0u) };
        Assert.Equal("", HardInfo.GetNetCardMac("MyNic", Rt(os)));
    }

    [Fact]
    public void GetNetCardMac_OverrunThrows()
    {
        var os = new THardInfoStubOs { OnNdis = oid => (true, new byte[256], 257u) };
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.GetNetCardMac("MyNic", Rt(os)));
    }

    [Fact]
    public void GetNetCardMac_FailureReturnsEmpty()
    {
        var os = new THardInfoStubOs { OnNdis = oid => (false, null, 0u) };
        Assert.Equal("", HardInfo.GetNetCardMac("MyNic", Rt(os)));
    }
}

// 测试替身：HardInfo.pas 全部接缝的可编程实现（记录调用参数 + 按脚本返回）。
// 与被测单元 src/GXX.Core/Hardware/HardInfoSeams.cs 一一对应。
using System;
using System.Collections.Generic;
using GXX.Core.Hardware;

namespace GXX.Core.Tests;

/// <summary>全部 HardInfo 接缝的可编程替身：默认"失败/空"，逐方法可替换为脚本。</summary>
public sealed unsafe class THardInfoStubOs :
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
    // ---------------- 记录 ----------------
    public readonly List<TOSVersionInfo> VersionCalls = new();
    public readonly List<(uint modeNum, TDeviceMode mode)> EnumDisplaySettingsCalls = new();
    public readonly List<(uint devNum, uint flags, uint cb)> EnumDisplayDevicesCalls = new();
    public readonly List<(uint param, TRegisters regs)> CpuIdCalls = new();
    public readonly List<ulong> MemoryCalls = new();
    public readonly List<uint> GLPICalls = new();
    public readonly List<int> GLPIBufferLengths = new();
    public readonly List<(TSendCmdInParams scip, int inSize, int outOffset, int outSize, int bufferLength)> PhysicalDriveCalls = new();
    public readonly List<(int bufferSize, byte[] bufferAtCall)> ScsiMiniportCalls = new();
    public readonly List<(TSendCmdInParams scip, int inSize, int outOffset, int outSize, int bufferLength)> SmartVsdCalls = new();
    public readonly List<(byte command, byte lana, ushort length, nint buffer, byte[] payload)> NetBiosCalls = new();
    public readonly List<string> NetBiosCallNames = new();

    /// <summary>最近一次调用时**调用方**缓冲区快照（调用前克隆，故不受脚本写入影响）。</summary>
    public byte[] LastPhysicalDriveBuffer;
    public byte[] LastScsiMiniportBuffer;
    public byte[] LastSmartVsdBuffer;

    /// <summary>调用方缓冲区的**活引用**（调用后仍可观察被测单元的就地改写）。</summary>
    public byte[] PhysicalDriveBufferRef;
    public byte[] ScsiMiniportBufferRef;
    public byte[] SmartVsdBufferRef;
    public readonly List<(string device, int ioctl, int oid, int outLength)> NdisCalls = new();
    public readonly List<(nint root, string name, uint sam)> RegOpenKeyExCalls = new();
    public readonly List<(nint hkey, int index, bool isKey, uint inLength)> RegEnumCalls = new();
    public readonly List<(nint registry, string key, bool canCreate)> RegistryOpenKeyCalls = new();
    public readonly List<string> RegistryReadStringCalls = new();

    // ---------------- 脚本 ----------------

    /// <summary>GetVersionEx 脚本；默认不改动结构并返回 false。</summary>
    public Func<TOSVersionInfo, (bool ok, TOSVersionInfo value)> OnGetVersionEx;

    /// <summary>EnumDisplaySettings 脚本；默认返回 false 且不改动。</summary>
    public Func<uint, (bool ok, TDeviceMode mode)> OnEnumDisplaySettings;

    /// <summary>EnumDisplayDevices 脚本；默认返回 false（枚举立即结束）。</summary>
    public Func<uint, (bool ok, TDisplayDevice device)> OnEnumDisplayDevices;

    /// <summary>CPUID 脚本；默认四寄存器归零。</summary>
    public Func<uint, TRegisters> OnCpuId;

    /// <summary>GlobalMemoryStatusEx 脚本；默认返回 false。</summary>
    public Func<(bool ok, TMemoryStatusEx value)> OnGlobalMemoryStatusEx;

    /// <summary>GetSystemInfo 脚本；默认全 0。</summary>
    public Func<TSystemInfo> OnGetSystemInfo;

    /// <summary>GetLogicalProcessorInformation 脚本；默认返回 false 且不动 returnLength。</summary>
    public Func<int, uint, (bool ok, uint returnLength, SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] entries)> OnGLPI;

    /// <summary>GetLastError 脚本；默认 ERROR_INSUFFICIENT_BUFFER。</summary>
    public Func<uint> OnGetLastError = () => HardInfo.ERROR_INSUFFICIENT_BUFFER;

    /// <summary>Win32Platform 值；默认 VER_PLATFORM_WIN32_NT。</summary>
    public uint Win32PlatformValue = HardInfo.VER_PLATFORM_WIN32_NT;

    /// <summary>PhysicalDriveIoControl 脚本；默认 false。</summary>
    public Func<int, int, int, (bool ok, byte[] buffer)> OnPhysicalDriveIoControl;

    /// <summary>ScsiMiniportIoControl 脚本；默认 false。</summary>
    public Func<int, (bool ok, byte[] buffer)> OnScsiMiniportIoControl;

    /// <summary>SmartVsdIoControl 脚本；默认 false。</summary>
    public Func<int, int, int, (bool ok, byte[] buffer)> OnSmartVsdIoControl;

    /// <summary>NetBios 脚本；默认返回 0x01（NRC_BUFLEN，失败）。第三参数为 <c>ncb_buffer</c> 所指的数组（可能为 null）。</summary>
    public Func<byte, ushort, byte[], byte> OnNetBios = (cmd, len, buf) => 0x01;

    /// <summary>NDIS 查询脚本；默认 false。</summary>
    public Func<int, (bool ok, byte[] outBuffer, uint bytesReturned)> OnNdis;

    /// <summary>RegOpenKeyEx 脚本；默认 false。</summary>
    public Func<nint, string, (bool ok, nint hKey)> OnRegOpenKeyEx;

    /// <summary>RegEnumKeyEx / RegEnumValue 脚本；默认 ERROR_NO_MORE_ITEMS。</summary>
    public Func<nint, int, bool, (uint rc, string name)> OnRegEnum;

    /// <summary>RegistryOpenKey 脚本；默认 false。</summary>
    public Func<nint, string, bool, bool> OnRegistryOpenKey;

    /// <summary>RegistryReadString 脚本；默认空串。</summary>
    public Func<nint, string, string> OnRegistryReadString = (_, __) => "";

    // ---------------- IVersionApi ----------------
    public bool GetVersionEx(ref TOSVersionInfo versionInfo)
    {
        VersionCalls.Add(versionInfo);
        if (OnGetVersionEx == null) return false;
        var (ok, value) = OnGetVersionEx(versionInfo);
        versionInfo = value;
        return ok;
    }

    // ---------------- IDisplayApi ----------------
    public bool EnumDisplaySettings(uint iModeNum, ref TDeviceMode lpDevMode)
    {
        EnumDisplaySettingsCalls.Add((iModeNum, lpDevMode));
        if (OnEnumDisplaySettings == null) return false;
        var (ok, mode) = OnEnumDisplaySettings(iModeNum);
        lpDevMode = mode;
        return ok;
    }

    public bool EnumDisplayDevices(uint dwDevNum, ref TDisplayDevice lpDisplayDevice, uint dwFlags)
    {
        EnumDisplayDevicesCalls.Add((dwDevNum, dwFlags, lpDisplayDevice.cb));
        if (OnEnumDisplayDevices == null) return false;
        var (ok, device) = OnEnumDisplayDevices(dwDevNum);
        if (ok)
        {
            // 真实 EnumDisplayDevices 会把 cb 写回自身大小；替身保持"调用方设置的值"，
            // 以便观察原文 :221 只在循环外设一次 cb。
            uint cb = lpDisplayDevice.cb;
            lpDisplayDevice = device;
            lpDisplayDevice.cb = cb;
        }
        return ok;
    }

    // ---------------- ICpuIdApi ----------------
    public void GetCPUID(uint Param, out TRegisters Registers)
    {
        Registers = OnCpuId?.Invoke(Param) ?? default;
        CpuIdCalls.Add((Param, Registers));
    }

    // ---------------- IMemoryApi ----------------
    public bool GlobalMemoryStatusEx(ref TMemoryStatusEx buffer)
    {
        MemoryCalls.Add(buffer.dwLength);
        if (OnGlobalMemoryStatusEx == null) return false;
        var (ok, value) = OnGlobalMemoryStatusEx();
        buffer = value;
        return ok;
    }

    // ---------------- IProcessorInformationApi ----------------
    public bool GetLogicalProcessorInformation(SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] buffer, ref uint returnLength)
    {
        GLPICalls.Add(returnLength);
        GLPIBufferLengths.Add(buffer.Length);
        if (OnGLPI == null) return false;
        var (ok, rl, entries) = OnGLPI(buffer.Length, returnLength);
        if (ok && entries != null)
            for (int i = 0; i < buffer.Length && i < entries.Length; i++) buffer[i] = entries[i];
        returnLength = rl;
        return ok;
    }

    public uint GetLastError() => OnGetLastError();

    public void GetSystemInfo(out TSystemInfo lpSystemInfo) => lpSystemInfo = OnGetSystemInfo?.Invoke() ?? default;

    // ---------------- IDiskIdentifyApi ----------------
    public uint Win32Platform => Win32PlatformValue;

    public bool PhysicalDriveIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] idOutCmd, int outOffset, int outSize, out uint bytesReturned)
    {
        PhysicalDriveCalls.Add((sendCmdIn, sendCmdInSize, outOffset, outSize, idOutCmd?.Length ?? -1));
        LastPhysicalDriveBuffer = idOutCmd == null ? null : (byte[])idOutCmd.Clone();
        PhysicalDriveBufferRef = idOutCmd;
        bytesReturned = 0;
        if (OnPhysicalDriveIoControl == null) return false;
        var (ok, buf) = OnPhysicalDriveIoControl(outOffset, outSize, idOutCmd?.Length ?? 0);
        if (ok && buf != null)
            for (int i = 0; i < idOutCmd.Length && i < buf.Length; i++) idOutCmd[i] = buf[i];
        return ok;
    }

    public bool ScsiMiniportIoControl(byte[] buffer, int bufferSize, out uint bytesReturned)
    {
        ScsiMiniportCalls.Add((bufferSize, buffer == null ? null : (byte[])buffer.Clone()));
        LastScsiMiniportBuffer = buffer == null ? null : (byte[])buffer.Clone();
        ScsiMiniportBufferRef = buffer;
        bytesReturned = 0;
        if (OnScsiMiniportIoControl == null) return false;
        var (ok, buf) = OnScsiMiniportIoControl(bufferSize);
        if (ok && buf != null)
            for (int i = 0; i < buffer.Length && i < buf.Length; i++) buffer[i] = buf[i];
        return ok;
    }

    public bool SmartVsdIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] buffer, int outOffset, int outSize, out uint bytesReturned)
    {
        SmartVsdCalls.Add((sendCmdIn, sendCmdInSize, outOffset, outSize, buffer?.Length ?? -1));
        LastSmartVsdBuffer = buffer == null ? null : (byte[])buffer.Clone();
        SmartVsdBufferRef = buffer;
        bytesReturned = 0;
        if (OnSmartVsdIoControl == null) return false;
        var (ok, buf) = OnSmartVsdIoControl(outOffset, outSize, buffer?.Length ?? 0);
        if (ok && buf != null)
            for (int i = 0; i < buffer.Length && i < buf.Length; i++) buffer[i] = buf[i];
        return ok;
    }

    // ---------------- INetBiosApi ----------------
    public byte NetBios(ref TNCB ncb, byte[] ncbBuffer)
    {
        NetBiosCalls.Add((ncb.ncb_command, ncb.ncb_lana_num, ncb.ncb_length, ncb.ncb_buffer, ncbBuffer));
        var name = new System.Text.StringBuilder();
        for (int i = 0; i < 16; i++)
        {
            byte b = ncb.ncb_callname[i];
            if (b == 0) break;
            name.Append((char)b);
        }
        NetBiosCallNames.Add(name.ToString());
        return OnNetBios(ncb.ncb_command, ncb.ncb_length, ncbBuffer);
    }

    // ---------------- INdisQueryApi ----------------
    public bool DeviceIoControlNdis(string deviceName, int ioControlCode, int oid, byte[] outBuffer, out uint bytesReturned)
    {
        NdisCalls.Add((deviceName, ioControlCode, oid, outBuffer.Length));
        bytesReturned = 0;
        if (OnNdis == null) return false;
        var (ok, buf, br) = OnNdis(oid);
        if (ok && buf != null)
            for (int i = 0; i < outBuffer.Length && i < buf.Length; i++) outBuffer[i] = buf[i];
        bytesReturned = br;
        return ok;
    }

    // ---------------- IRegistryApi ----------------
    public bool RegOpenKeyEx(nint rootKey, string name, uint samDesired, out nint hKey)
    {
        RegOpenKeyExCalls.Add((rootKey, name, samDesired));
        hKey = 0;
        if (OnRegOpenKeyEx == null) return false;
        var (ok, h) = OnRegOpenKeyEx(rootKey, name);
        hKey = h;
        return ok;
    }

    public uint RegEnumKeyEx(nint hKey, int index, byte[] nameBuffer, ref uint nameLength)
        => RegEnum(hKey, index, nameBuffer, ref nameLength, isKey: true);

    public uint RegEnumValue(nint hKey, int index, byte[] nameBuffer, ref uint nameLength)
        => RegEnum(hKey, index, nameBuffer, ref nameLength, isKey: false);

    private uint RegEnum(nint hKey, int index, byte[] nameBuffer, ref uint nameLength, bool isKey)
    {
        RegEnumCalls.Add((hKey, index, isKey, nameLength));
        if (OnRegEnum == null) return HardInfo.ERROR_NO_MORE_ITEMS;
        var (rc, name) = OnRegEnum(hKey, index, isKey);        if (rc == HardInfo.ERROR_SUCCESS)
        {
            var bytes = GXX.Core.Rtl.DelphiRTL.AnsiBytes(name);
            for (int i = 0; i < nameBuffer.Length; i++) nameBuffer[i] = i < bytes.Length ? bytes[i] : (byte)0;
            nameLength = (uint)bytes.Length;
        }
        return rc;
    }

    public void RegCloseKey(nint hKey) { }

    public nint RegistryCreate() => 0x4321;

    public void RegistrySetRootKey(nint registry, nint rootKey) { }

    public bool RegistryOpenKey(nint registry, string key, bool canCreate)
    {
        RegistryOpenKeyCalls.Add((registry, key, canCreate));
        return OnRegistryOpenKey?.Invoke(registry, key, canCreate) ?? false;
    }

    public string RegistryReadString(nint registry, string name)
    {
        RegistryReadStringCalls.Add(name);
        return OnRegistryReadString(registry, name);
    }

    public void RegistryCloseKey(nint registry) { }

    public void RegistryFree(nint registry) { }
}

/// <summary>把替身装配成 <see cref="THardInfoRuntime"/>。</summary>
public static class THardInfoStub
{
    public static THardInfoRuntime Runtime(THardInfoStubOs os) => new THardInfoRuntime(
        os, os, os, os, os, os, os, os, os);
}

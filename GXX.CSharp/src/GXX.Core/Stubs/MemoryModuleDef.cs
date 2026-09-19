using System;
using System.Runtime.InteropServices;

namespace GXX.Core.Stubs;

/// <summary>
/// MemoryModuleDef.pas 1:1（**数据/常量/结构定义层**）。
///
/// 原文为原生 PE 内存加载器（MemoryModule.c 的 Delphi 移植）所需的 Windows 结构别名与常量。
/// 按 docs/转换开发文档.md §2.3，**原生 PE 加载在托管运行时无等价语义**（CLR 不允许把 PE
/// 映像手工映射进本进程再直接执行导出函数），故本文件只保留
/// **常量 / 枚举 / packed 结构定义**，不含任何加载逻辑（逻辑见 MemoryModule.cs 的 Stub）。
///
/// 结构体全部用 [StructLayout(LayoutKind.Sequential, Pack = 1)] 保持与 Windows SDK /
/// 原文 packed record 一致的字节布局，并给出 SizeOf 常量供布局断言。
///
/// 原文（MemoryModuleDef.pas:8-197）定义的内容逐条对应如下。
/// </summary>
public static class MemoryModuleDef
{
    public const uint IMAGE_FILE_MACHINE_I386 = 0x014C;
    public const uint IMAGE_FILE_MACHINE_AMD64 = 0x8664;

    /// <summary>原文 {$IFDEF WIN64} HOST_MACHINE = AMD64 {$ELSE} I386 {$ENDIF}；托管侧固定 64 位宿主。</summary>
    public const uint HOST_MACHINE = IMAGE_FILE_MACHINE_AMD64;

    // ---- 重定位类型（MemoryModuleDef.pas:18-29）----
    public const int IMAGE_REL_BASED_ABSOLUTE = 0;
    public const int IMAGE_REL_BASED_HIGH = 1;
    public const int IMAGE_REL_BASED_LOW = 2;
    public const int IMAGE_REL_BASED_HIGHLOW = 3;
    public const int IMAGE_REL_BASED_HIGHADJ = 4;
    public const int IMAGE_REL_BASED_MIPS_JMPADDR = 5;
    public const int IMAGE_REL_BASED_SECTION = 6;
    public const int IMAGE_REL_BASED_REL32 = 7;
    public const int IMAGE_REL_BASED_MIPS_JMPADDR16 = 9;
    public const int IMAGE_REL_BASED_IA64_IMM64 = 9;
    public const int IMAGE_REL_BASED_DIR64 = 10;
    public const int IMAGE_REL_BASED_HIGH3ADJ = 11;

    // ---- 目录项 / 段属性（MemoryModuleDef.pas:32-45）----
    public const uint IMAGE_ORDINAL_FLAG32 = 0x80000000;
    public const int IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
    public const int IMAGE_DIRECTORY_ENTRY_BASERELOC = 5;
    public const int IMAGE_DIRECTORY_ENTRY_EXPORT = 0;

    public const uint IMAGE_SCN_LNK_NRELOC_CVFL = 0x01000000;
    public const uint IMAGE_SCN_MEM_DISCARDABLE = 0x02000000;
    public const uint IMAGE_SCN_MEM_NOT_CACHED = 0x04000000;
    public const uint IMAGE_SCN_MEM_NOT_PAGED = 0x08000000;
    public const uint IMAGE_SCN_MEM_NOT_SHARED = 0x10000000;
    public const uint IMAGE_SCN_MEM_EXECUTE = 0x20000000;
    public const uint IMAGE_SCN_MEM_READ = 0x40000000;
    public const uint IMAGE_SCN_MEM_WRITE = 0x80000000;
    public const uint IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040;
    public const uint IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080;

    /// <summary>Windows SDK IMAGE_SIZEOF_SHORT_NAME = 8（原文 MemoryModuleDef.pas:117 使用）。</summary>
    public const int IMAGE_SIZEOF_SHORT_NAME = 8;

    public const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;      // "MZ"
    public const uint IMAGE_NT_SIGNATURE = 0x00004550;     // "PE\0\0"
    public const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x010B;
    public const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x020B;
}

/// <summary>原文 _IMAGE_EXPORT_DIRECTORY（packed，MemoryModuleDef.pas:74-86）。SizeOf = 40。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TImageExportDirectory
{
    public uint Characteristics;
    public uint TimeDateStamp;
    public ushort MajorVersion;
    public ushort MinorVersion;
    public uint Name;
    public uint Base;
    public uint NumberOfFunctions;
    public uint NumberOfNames;
    public uint AddressOfFunctions;      // RVA from base of image
    public uint AddressOfNames;          // RVA from base of image
    public uint AddressOfNameOrdinals;   // RVA from base of image

    public static int SizeOf => 40;
}

/// <summary>原文 _IMAGE_DOS_HEADER（packed，MemoryModuleDef.pas:91-111）。SizeOf = 64。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TImageDosHeader
{
    public ushort e_magic;
    public ushort e_cblp;
    public ushort e_cp;
    public ushort e_crlc;
    public ushort e_cparhdr;
    public ushort e_minalloc;
    public ushort e_maxalloc;
    public ushort e_ss;
    public ushort e_sp;
    public ushort e_csum;
    public ushort e_ip;
    public ushort e_cs;
    public ushort e_lfarlc;
    public ushort e_ovno;
    public fixed ushort e_res[4];
    public ushort e_oemid;
    public ushort e_oeninfo;
    public fixed ushort e_res2[10];
    public int _lfanew;

    public static int SizeOf => 64;
}

/// <summary>
/// 原文 _IMAGE_SECTION_HEADER（packed，MemoryModuleDef.pas:116-127）。SizeOf = 40。
/// Misc 原文是 TISHMisc（union of PhysicalAddress/VirtualSize），此处用 VirtualSize 单值表达。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TImageSectionHeader
{
    public fixed byte Name[8];   // IMAGE_SIZEOF_SHORT_NAME
    public uint Misc;            // union: PhysicalAddress / VirtualSize
    public uint VirtualAddress;
    public uint SizeOfRawData;
    public uint PointerToRawData;
    public uint PointerToRelocations;
    public uint PointerToLinenumbers;
    public ushort NumberOfRelocations;
    public ushort NuberOfLinenumbers;   // 原文如此拼写（MemoryModuleDef.pas:125）
    public uint Characteristics;

    public static int SizeOf => 40;
}

/// <summary>原文 _IMAGE_BASE_RELOCATION（packed，MemoryModuleDef.pas:132-135）。SizeOf = 8。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TImageBaseRelocation
{
    public uint VirtualAddress;
    public uint SizeOfBlock;

    public static int SizeOf => 8;
}

/// <summary>原文 _IMAGE_IMPORT_DESCRIPTOR（packed，MemoryModuleDef.pas:140-146）。SizeOf = 20。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TImageImportDescriptor
{
    public uint OriginalFirstThunk;
    public uint TimeDateStamp;
    public uint ForwarderChain;
    public uint Name;
    public uint FirstThunk;

    public static int SizeOf => 20;
}

/// <summary>
/// 原文 _IMAGE_IMPORT_BY_NAME（packed，MemoryModuleDef.pas:151-154）。SizeOf = 258。
/// 原文注释：`Name: array [0..255] of Byte; // original: "Name: array [0..0] of Byte;"`
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TImageImportByName
{
    public ushort Hint;
    public fixed byte Name[256];

    public static int SizeOf => 258;
}

/// <summary>原文 _SECTION_FINALIZE_DATA（packed，MemoryModuleDef.pas:159-165）。SizeOf = 32（x64）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TSectionFinalizeData
{
    public IntPtr Address;
    public IntPtr AlignedAddress;
    public IntPtr Size;             // SIZE_T
    public uint Characteristics;
    public int IsLast;              // BOOL

    public static int SizeOf => IntPtr.Size == 8 ? 32 : 20;
}

/// <summary>原文 _IMAGE_TLS_DIRECTORY32（MemoryModuleDef.pas:170-177）。SizeOf = 24。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TImageTlsDirectory32
{
    public uint StartAddressOfRawData;
    public uint EndAddressOfRawData;
    public uint AddressOfIndex;
    public uint AddressOfCallBacks;
    public uint SizeOfZeroFill;
    public uint Characteristics;

    public static int SizeOf => 24;
}

/// <summary>原文 _IMAGE_TLS_DIRECTORY64（MemoryModuleDef.pas:182-189）。SizeOf = 40。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TImageTlsDirectory64
{
    public ulong StartAddressOfRawData;
    public ulong EndAddressOfRawData;
    public ulong AddressOfIndex;
    public ulong AddressOfCallBacks;
    public uint SizeOfZeroFill;
    public uint Characteristics;

    public static int SizeOf => 40;
}

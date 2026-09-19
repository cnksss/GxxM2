using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GXX.Core.Stubs;

/// <summary>
/// MemoryModule.pas 的**托管侧 Stub**（docs/转换开发文档.md §2.3 不移植项）。
///
/// 原文（MemoryModule.pas:10-21 / 430-833）是 MemoryModule.c 的 Delphi 移植：
/// 把 PE 映像手工映射进本进程地址空间（CopySections → PerformBaseRelocation →
/// BuildImportTable → FinalizeSections → ExecuteTLS → 调 DllMain），返回可调用导出函数的
/// PMemoryModule。**托管运行时不存在对应语义**：
///  - CLR 不允许把任意原生 PE 直接映射进进程后执行其导出函数；
///  - 段保护/重定位/导入表/异常目录都由 OS PE 加载器与 CLR 自己的加载器负责；
///  - .NET 的等价能力是 AssemblyLoadContext（见 M2Engine/PluginManager 侧）。
/// 因此本文件只保留**数据/枚举/结构定义与全部原方法签名**，方法体抛
/// <see cref="NotSupportedException"/> 或返回失败值（false / IntPtr.Zero），
/// 使调用方在编译期即可拿到完整接口面，运行期显式失败而不是静默错误。
///
/// 保留的**纯算术工具函数**（AlignValueUp/Down、CheckSize、OffsetPointer、
/// GetImageSnapByOrdinal、GetImageOrdinal）不含任何原生语义，按原文 1:1 实现并测试。
/// </summary>
public static class MemoryModule
{
    /// <summary>原文 MemoryModuleDef.pas:65-71 的 TMemoryModule（packed record）。
    /// 托管侧以 class 表达（ModuleList 为对象列表而非原生指针链）。</summary>
    public sealed class TMemoryModule
    {
        public IntPtr Headers;                          // PImageNtHeaders
        public IntPtr CodeBase;
        public List<object> ModuleList = new();         // 原文 TList
        public IntPtr PageSize;                         // SIZE_T
        public int IsInitialized;                       // BOOL

        public static int SizeOf => IntPtr.Size == 8 ? 40 : 24;   // 4(→8)+8+8+8+4+padding
    }

    /// <summary>原文 TDllEntryProc（MemoryModuleDef.pas:197）。</summary>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int DllEntryProc(IntPtr hInstDLL, uint dwReason, IntPtr lpReserved);

    // ================= 纯算术工具（原文实现可直接移植，已 1:1 保留）=================

    /// <summary>原文 AlignValueUp（MemoryModule.pas:32-35）：`(Value + Alignment - 1) div Alignment * Alignment`。</summary>
    public static IntPtr AlignValueUp(IntPtr value, IntPtr alignment)
    {
        long v = value.ToInt64();
        long a = alignment.ToInt64();
        return new IntPtr((v + a - 1) / a * a);
    }

    /// <summary>原文 AlignValueDown（MemoryModule.pas:37-40）：`Value div Alignment * Alignment`。</summary>
    public static IntPtr AlignValueDown(IntPtr value, IntPtr alignment)
    {
        long v = value.ToInt64();
        long a = alignment.ToInt64();
        return new IntPtr(v / a * a);
    }

    /// <summary>原文 CheckSize（MemoryModule.pas:42-45）：`ASize >= ExpectedSize`。</summary>
    public static bool CheckSize(IntPtr aSize, IntPtr expectedSize)
        => aSize.ToInt64() >= expectedSize.ToInt64();

    /// <summary>原文 OffsetPointer（MemoryModule.pas:52-55）：`Pointer(IntPtr(Data) + Offset)`。</summary>
    public static IntPtr OffsetPointer(IntPtr data, IntPtr offset)
        => new IntPtr(data.ToInt64() + offset.ToInt64());

    /// <summary>原文 GetImageSnapByOrdinal（MemoryModule.pas:71-74）：
    /// `(Ordinal and IMAGE_ORDINAL_FLAG32) &lt;&gt; 0`。</summary>
    public static bool GetImageSnapByOrdinal(IntPtr ordinal)
        => ((ulong)ordinal.ToInt64() & MemoryModuleDef.IMAGE_ORDINAL_FLAG32) != 0;

    /// <summary>原文 GetImageOrdinal（MemoryModule.pas:76-79）：`Ordinal and $FFFF`。</summary>
    public static ushort GetImageOrdinal(IntPtr ordinal)
        => (ushort)((ulong)ordinal.ToInt64() & 0xFFFF);

    // ================= 原生 PE 加载接口（Stub：托管无等价语义）=================

    private const string StubReason =
        "原文为原生 PE 内存加载（MemoryModule.pas:430-833，CopySections/PerformBaseRelocation/" +
        "BuildImportTable/FinalizeSections/ExecuteTLS/调 DllMain），托管侧无等价语义" +
        "（转换开发文档 §2.3）；请改用 AssemblyLoadContext（见 M2Engine/PluginManager）。";

    /// <summary>对应原文 MemoryLoadLibary(MemData, MemSize, var RunCode, IsProcessAttach): PMemoryModule。</summary>
    public static TMemoryModule MemoryLoadLibary(byte[] memData, IntPtr memSize, ref int runCode, bool isProcessAttach = true)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 MemoryFreeLibrary(Module, IsProcessDetach)。</summary>
    public static void MemoryFreeLibrary(TMemoryModule module, bool isProcessDetach = true)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 MemoryGetProcAddress(Module, const Name: PAnsiChar): Pointer。</summary>
    public static IntPtr MemoryGetProcAddress(TMemoryModule module, string name)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 MemoryGetProcAddress(Module, const Index: Integer): Pointer（按序号）。</summary>
    public static IntPtr MemoryGetProcAddress(TMemoryModule module, int index)
        => throw new NotSupportedException(StubReason);

    // 以下为原文的私有/辅助函数，同样保留签名（托管不可用）

    /// <summary>对应原文 GetFieldOffset（MemoryModule.pas:47-50）：`IntPtr(@Field) - IntPtr(@Struc)`。</summary>
    public static uint GetFieldOffset(IntPtr struc, IntPtr field)
        => (uint)(field.ToInt64() - struc.ToInt64());

    /// <summary>对应原文 GetImageFirstSection（MemoryModule.pas:57-64）。</summary>
    public static IntPtr GetImageFirstSection(IntPtr ntHeaders)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 GetHeaderDictionary（MemoryModule.pas:66-69）。</summary>
    public static IntPtr GetHeaderDictionary(TMemoryModule module, int index)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 GetRealSectionSize（MemoryModule.pas:81-91）。</summary>
    public static IntPtr GetRealSectionSize(TMemoryModule module, IntPtr section)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 GetAllSectionSize（MemoryModule.pas:93-117）。</summary>
    public static IntPtr GetAllSectionSize(IntPtr ntHeaders)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 CopySections（MemoryModule.pas:119-156）。</summary>
    public static void CopySections(byte[] memData, IntPtr ntHeaders, TMemoryModule module)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 PerformBaseRelocation（MemoryModule.pas:158-217）。</summary>
    public static void PerformBaseRelocation(TMemoryModule module, uint relocationOffset)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 BuildImportTable（MemoryModule.pas:229-303）。</summary>
    public static bool BuildImportTable(TMemoryModule module)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 FinalizeSection（MemoryModule.pas:305-358）。</summary>
    public static bool FinalizeSection(TMemoryModule module, TSectionFinalizeData sectionData)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 FinalizeSections（MemoryModule.pas:360-391）。</summary>
    public static bool FinalizeSections(TMemoryModule module)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 ExecuteTLS（MemoryModule.pas:393-428）。</summary>
    public static void ExecuteTLS(TMemoryModule module)
        => throw new NotSupportedException(StubReason);

    /// <summary>对应原文 PAnsiCharToPChar（MemoryModule.pas:219-227）。</summary>
    public static IntPtr PAnsiCharToPChar(IntPtr addr)
        => throw new NotSupportedException(StubReason);
}

// 测试：Source/M2Engine/MemoryModuleEx.pas → GXX.M2Server.Sweep.MemoryModuleEx（Stub，按 §2.3）
//
// Stub 的测试目标与常规移植不同：**不是**验证业务行为，而是钉死三件事 ——
//   1. 签名/结构保留（原文接口段三个 stdcall 函数 + TMemoryModule 类型别名都在）；
//   2. **任何调用都显式失败**（NotSupportedException），且异常消息里带得上 §2.3 的理由与替代路径
//      —— 绝不能静默返回 null/IntPtr.Zero（那会让调用方误判为「加载成功但取不到符号」）；
//   3. 能力探测位 IsSupported 恒 false。

using System;
using GXX.M2Server.Sweep;
using Xunit;

namespace GXX.M2Server.Tests;

public class SweepMemoryModuleExTests
{
    // ------------------------------------------------------------------ 签名/结构保留

    [Fact]
    public void Surface_KeepsOriginalInterfaceSection()
    {
        // 原文接口段（MemoryModuleEx.pas:60-72）只有：TMemoryModule + 三个函数
        var t = typeof(MemoryModuleEx);
        Assert.NotNull(t.GetMethod(nameof(MemoryModuleEx.MemoryLoadLibrary), new[] { typeof(byte[]) }));
        Assert.NotNull(t.GetMethod(nameof(MemoryModuleEx.MemoryLoadLibrary), new[] { typeof(byte[]), typeof(int) }));
        Assert.NotNull(t.GetMethod(nameof(MemoryModuleEx.MemoryGetProcAddress),
            new[] { typeof(TMemoryModule), typeof(string) }));
        Assert.NotNull(t.GetMethod(nameof(MemoryModuleEx.MemoryGetProcAddress),
            new[] { typeof(TMemoryModule), typeof(byte[]) }));
        Assert.NotNull(t.GetMethod(nameof(MemoryModuleEx.MemoryFreeLibrary), new[] { typeof(TMemoryModule) }));

        // 返回类型保留（原文返回 TMemoryModule / Pointer / 无返回）
        Assert.Equal(typeof(TMemoryModule),
            t.GetMethod(nameof(MemoryModuleEx.MemoryLoadLibrary), new[] { typeof(byte[]) })!.ReturnType);
        Assert.Equal(typeof(IntPtr),
            t.GetMethod(nameof(MemoryModuleEx.MemoryGetProcAddress),
                new[] { typeof(TMemoryModule), typeof(string) })!.ReturnType);
        Assert.Equal(typeof(void),
            t.GetMethod(nameof(MemoryModuleEx.MemoryFreeLibrary), new[] { typeof(TMemoryModule) })!.ReturnType);

        // 静态类（原文是单元级函数，非对象方法）
        Assert.True(t.IsAbstract && t.IsSealed, "MemoryModuleEx 应为静态类");
    }

    [Fact]
    public void Surface_IsSupportedIsFalse()
    {
        Assert.False(MemoryModuleEx.IsSupported);   // 按 §2.3 恒不可用
    }

    // ------------------------------------------------------------------ 显式失败（不静默）

    [Fact]
    public void MemoryLoadLibrary_Throws_NotSupported()
    {
        var ex = Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryLoadLibrary(new byte[16]));
        Assert.Contains("2.3", ex.Message);                          // 带上 §2.3 依据
        Assert.Contains("MemoryModuleEx.pas", ex.Message);           // 带上源单元名（便于定位）
        Assert.Contains("Assembly.Load", ex.Message);                // 带上托管替代路径
    }

    [Fact]
    public void MemoryLoadLibrary_OffsetOverload_Throws_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryLoadLibrary(new byte[16], 4));
    }

    [Fact]
    public void MemoryLoadLibrary_EmptyOrNullInput_StillThrows_NotReturnsNull()
    {
        // 关键差异断言：**空/非法输入也必须抛**，不能因为「输入不合法」就返回 null 装作是加载失败。
        // 原文对非法 PE 的处理是返回 nil；托管 Stub 若也返回 null，就与「假成功」无法区分了。
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryLoadLibrary(Array.Empty<byte>()));
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryLoadLibrary(new byte[0], 0));
    }

    [Fact]
    public void MemoryGetProcAddress_Throws_NotSupported_ForBothOverloads()
    {
        var module = MakeModuleHandle();
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryGetProcAddress(module, "DllMain"));
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryGetProcAddress(module, Array.Empty<byte>()));
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryGetProcAddress(module, ""));
    }

    [Fact]
    public void MemoryFreeLibrary_Throws_NotSupported()
    {
        var module = MakeModuleHandle();
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryFreeLibrary(module));
        Assert.Throws<NotSupportedException>(() => MemoryModuleEx.MemoryFreeLibrary(null!));
    }

    // ------------------------------------------------------------------ 句柄类型

    [Fact]
    public void TMemoryModule_IsOpaqueHandle_WithReadableToString()
    {
        var module = MakeModuleHandle();
        Assert.Equal(new IntPtr(0x1234), module.Handle);
        Assert.Contains("1234", module.ToString());
        Assert.StartsWith("TMemoryModule(", module.ToString());
    }

    /// <summary>构造一个句柄对象（原文是 THandle 整数；托管侧构造器为 internal，测试用反射取）。</summary>
    private static TMemoryModule MakeModuleHandle()
    {
        var ctor = typeof(TMemoryModule).GetConstructor(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null, new[] { typeof(IntPtr) }, null);
        Assert.NotNull(ctor);
        return (TMemoryModule)ctor!.Invoke(new object[] { new IntPtr(0x1234) });
    }
}

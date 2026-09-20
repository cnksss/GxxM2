using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// 把 uBuffer 的全部测试类放进**同一个 xUnit collection**（默认不并行执行同一 collection 内的类）。
/// <para>
/// 原因：uBuffer.pas 的六个全局内存池与 <c>TotalMemBytes</c> 是**进程级共享**的
/// （原文 unit-level `var` + `initialization`），而本工程的测试默认按类并行；
/// 其它类一旦调用 <c>MemoryPoolGlobal.FreeObjPool()</c> 就会在并行测试中间把池整体释放掉，
/// 使"刚 Free 完必然 AllPoolsFreed""全局账目精确增量"这类断言偶发失败
/// （实测正是这一原因，见 uBuffer 交接报告）。
/// </para>
/// <para>
/// 本 collection 只序列化 uBuffer 自己的 5 个测试类，不影响其它测试类的并行度。
/// </para>
/// </summary>
[CollectionDefinition(BufferTestCollection.Name, DisableParallelization = true)]
public sealed class BufferTestCollection
{
    /// <summary>collection 名称。</summary>
    public const string Name = "uBuffer";
}

// ============================================================================
// 车道 p6-test-isolation：程序集级自定义 xUnit TestFramework
//
// 为什么用「程序集级 TestFramework」而不是 BeforeAfterTestAttribute：
//   * BeforeAfterTestAttribute 必须逐类/逐方法标注，无法程序集级生效 —— 未来新写的测试
//     只要忘了标注就重新漏水；而本缺陷的根因正是「没人记得还原」。
//   * TestFramework + 执行器包装是 xUnit 2 官方公开扩展点
//     （XunitTestFramework.CreateExecutor / TestFrameworkExecutor<T>.RunTestCases 都是
//      protected virtual），只包一层 IXunitTestCase，不改任何既有测试文件。
//
// 生效方式：TestConfig.cs 里的
//     [assembly: TestFramework("GXX.M2Server.Tests.M2ConfigIsolationFramework", "GXX.M2Server.Tests")]
// 执行语义：**每个测试用例**（含类构造、BeforeAfter 特性、用例体、IDisposable 释放）
// 之前抓一份静态全局快照，之后（finally）逐成员复位。
//
// 语义选择说明：还原目标是「本用例开始前的状态」而不是「进程首次抓取的原始默认值」。
// 这样既治好了跨用例污染，又不会误伤 xUnit 允许的、在用例之外建立的上下文
// （例如将来引入 IClassFixture/ICollectionFixture 时在 fixture 构造器里设的全局量）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GXX.M2Server.Tests;

/// <summary>程序集级测试框架：为每个测试用例套上静态全局快照/还原。</summary>
public sealed class M2ConfigIsolationFramework : XunitTestFramework
{
    public M2ConfigIsolationFramework(IMessageSink messageSink)
        : base(messageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        => new M2ConfigIsolationFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
}

/// <summary>执行器：把每个 <see cref="IXunitTestCase"/> 包一层隔离壳。</summary>
public sealed class M2ConfigIsolationFrameworkExecutor : XunitTestFrameworkExecutor
{
    public M2ConfigIsolationFrameworkExecutor(AssemblyName assemblyName,
                                              ISourceInformationProvider sourceInformationProvider,
                                              IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
    }

    /// <summary>
    /// RunAll 与 RunTests 两条路径都汇到这里（<see cref="TestFrameworkExecutor{TTestCase}.RunAll"/> 内部也调用
    /// <c>RunTestCases</c>），因此只覆盖这一个点即可保证「一个测试用例都逃不掉」。
    /// </summary>
    protected override void RunTestCases(IEnumerable<IXunitTestCase> testCases,
                                         IMessageSink executionMessageSink,
                                         ITestFrameworkExecutionOptions executionOptions)
        => base.RunTestCases(testCases.Select(tc => (IXunitTestCase)new M2ConfigIsolationTestCase(tc)).ToArray(),
                             executionMessageSink,
                             executionOptions);
}

/// <summary>
/// 单个测试用例的隔离壳：完全委托给内层用例，只在 <see cref="RunAsync"/> 前后做静态全局快照/还原。
/// </summary>
/// <remarks>
/// 必须派生自 <see cref="LongLivedMarshalByRefObject"/> 并提供无参构造器（xUnit 分析器 xUnit3000/xUnit3001
/// 对 IXunitTestCase 实现类的硬性要求）。隔离壳只在**执行期**包裹用例，从不参与序列化
/// （序列化发生在发现期，那时还没有本壳），所以无参构造器只在被误序列化时才可能用到。
/// </remarks>
public sealed class M2ConfigIsolationTestCase : LongLivedMarshalByRefObject, IXunitTestCase
{
    private readonly IXunitTestCase _inner;

    /// <summary>仅供 xUnit 反序列化需求占位，正常路径不会用到。</summary>
    public M2ConfigIsolationTestCase()
        : this(null!)
    {
    }

    public M2ConfigIsolationTestCase(IXunitTestCase inner)
        => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    /// <summary>本壳已包裹的用例数（覆盖度自检用）。</summary>
    public static int WrappedCaseCount;

    public string DisplayName => _inner.DisplayName;

    public string SkipReason => _inner.SkipReason;

    public ISourceInformation SourceInformation
    {
        get => _inner.SourceInformation;
        set => _inner.SourceInformation = value;
    }

    public ITestMethod TestMethod => _inner.TestMethod;

    public object[] TestMethodArguments => _inner.TestMethodArguments;

    public Dictionary<string, List<string>> Traits => _inner.Traits;

    public string UniqueID => _inner.UniqueID;

    public Exception InitializationException => _inner.InitializationException;

    public IMethodInfo Method => _inner.Method;

    public int Timeout => _inner.Timeout;

    public void Deserialize(IXunitSerializationInfo info) => _inner.Deserialize(info);

    public void Serialize(IXunitSerializationInfo info) => _inner.Serialize(info);

    public async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                          IMessageBus messageBus,
                                          object[] constructorArguments,
                                          ExceptionAggregator aggregator,
                                          System.Threading.CancellationTokenSource cancellationTokenSource)
    {
        System.Threading.Interlocked.Increment(ref WrappedCaseCount);

        // 快照必须在内层用例启动之前抓（此时类构造器/BeforeAfter 特性都还没跑）。
        var snapshot = M2ConfigIsolationState.Capture();
        try
        {
            // ConfigureAwait(false) 是刻意的：本壳只是「前后包一层」，不需要把自己的续体
            // 再排回 xUnit 的 MaxConcurrencySyncContext（那会为一个 7,300 例的套件多出
            // 7,300 次线程跳转）。内层用例自己的 await 仍然照旧走 xUnit 的同步上下文。
            return await _inner.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource)
                               .ConfigureAwait(false);
        }
        finally
        {
            // 用例抛异常 / 被取消也必须还原。
            M2ConfigIsolationState.Restore(snapshot);
        }
    }
}

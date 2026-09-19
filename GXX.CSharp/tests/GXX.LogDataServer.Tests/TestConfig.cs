using System;
using System.Threading;
using Xunit;

// LogDataShare / TSearchManagerHost 为静态全局（对应 Delphi g_ControlIPList / g_SearchManager），
// 多测试类并行会互相污染，故禁用并行。
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GXX.LogDataServer.Tests;

/// <summary>xUnit STA 线程运行的 Fact（WinForms 控件操作）。</summary>
public sealed class StaFactAttribute : FactAttribute
{
    public StaFactAttribute() { }
}

/// <summary>xUnit 参数化。</summary>
public sealed class StaTheoryAttribute : TheoryAttribute
{
    public StaTheoryAttribute() { }
}

/// <summary>STA 线程执行器（约定照抄 tests/GXX.M2Server.Tests/FormGeneralConfigTests.cs）。</summary>
public static class StaRunner
{
    public static T New<T>(Func<T> factory) where T : IDisposable
    {
        T? result = default;
        Action action = () => result = factory();
        New(action);
        return result!;
    }

    /// <summary>STA 线程执行并回抛异常。</summary>
    public static void New(Action action)
    {
        Exception? caught = null;
        var t = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }
}

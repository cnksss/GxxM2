using System;
using System.Threading;
using Xunit;

// LoginSrvShare.g_Config / g_AccountDB / g_RoleDB 为静态全局（对应 Delphi g_Config 等），
// 多测试类并行会互相污染，故禁用并行。
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GXX.LoginSrv.Tests;

/// <summary>xUnit STA 线程运行的 Fact（WinForms 控件操作）。</summary>
public sealed class StaFactAttribute : FactAttribute
{
    public StaFactAttribute() { }
}

/// <summary>xUnit 参数化（控件文本驱动，无需真 STA 句柄，与 StaFact 同语义命名）。</summary>
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

    /// <summary>STA 线程执行并回抛异常（异常逃逸线程入口会崩 testhost 使运行挂起）。</summary>
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

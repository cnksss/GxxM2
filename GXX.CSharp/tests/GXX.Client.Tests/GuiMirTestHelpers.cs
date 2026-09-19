using System;
using System.Threading;

namespace GXX.Client.Tests;

/// <summary>
/// 本车道（并行批次P1 · GUI/Mir 装备窗口族）测试公共夹具。
///
/// 测试约定说明（替代做法）：
/// 本车道 5 个窗口都是 HGE 自绘窗口 —— 原 Delphi 里 TSerialWindows 继承 FState.pas 的 TFrmDlg
/// （TObject，非 TForm），控件是 DxComponent 的逻辑控件（TDxControl 继承 TComponent，见
/// DxControls.pas:152），GUI 布局由 Mir.GUI 资源流在运行期载入，**没有 .dfm**。因此这里不存在
/// WinForms 句柄/消息泵需要阻断的对象，控件属性可直接读写（免句柄）。为与 FormJ5x 约定保持
/// 一致（STA 线程执行 + 异常回抛，避免测试宿主挂起），仍统一经 <see cref="GuiSta.Run"/> 执行。
/// </summary>
public static class GuiSta
{
    public static void Run(Action action)
    {
        Exception caught = null;
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

    public static T Run<T>(Func<T> factory)
    {
        T result = default;
        Run(() => { result = factory(); });
        return result;
    }
}

/// <summary>把 GXX.Client.GUI.Mir 的 MShare 全局复位到单元初始化默认值。</summary>
public static class GuiReset
{
    public static void All()
    {
        GXX.Client.GUI.Mir.MShareGlobalsReset.ResetForTests();
    }
}

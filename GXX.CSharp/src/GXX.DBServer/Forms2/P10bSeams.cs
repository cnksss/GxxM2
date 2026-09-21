using System;
using System.Windows.Forms;

// 接缝层（p10-m2-misc 车道）：uFrmHumanExport.pas / CreateChr.pas 需要的
// Delphi `Dialogs.TSaveDialog` 与 `SysUtils.ExtractFileExt/ChangeFileExt` 托管等价物。
// 本文件**不**移植任何产品单元，只提供可注入的 UI/路径接缝（登记 D-P10-07/08）。

namespace GXX.DBServer.Forms2;

/// <summary>
/// Delphi `Dialogs.TSaveDialog` 的托管替身。
///
/// <para>
/// 原文用法（`uFrmHumanExport.pas:77-100` / `:121-143`）：
/// <c>SaveDialog := TSaveDialog.Create(nil); SaveDialog.Title/Filter/FileName := …; if SaveDialog.Execute then …</c>。
/// 托管侧把"用户是否确认 + 落到哪个路径"收成一个可覆写的方法，测试注入桩即可断言
/// "弹没弹、标题/过滤/默认文件名是什么、最终写到哪个路径"，且**不会**挂死 testhost。
/// </para>
/// </summary>
public class TSaveDialogSeam
{
    /// <summary>Delphi `TSaveDialog.Title`。</summary>
    public string Title = "";

    /// <summary>Delphi `TSaveDialog.Filter`（WinForms 同用 <c>|</c> 分隔的 '描述|模式' 语法）。</summary>
    public string Filter = "";

    /// <summary>Delphi `TSaveDialog.FileName`（Execute 成功后为用户的最终选择）。</summary>
    public string FileName = "";

    /// <summary>Delphi `TSaveDialog.Execute`；默认弹真实 <see cref="SaveFileDialog"/>。</summary>
    public virtual bool Execute()
    {
        using var dlg = new SaveFileDialog
        {
            Title = Title,
            Filter = Filter,
            FileName = FileName,
        };
        if (dlg.ShowDialog() != DialogResult.OK) return false;
        FileName = dlg.FileName;
        return true;
    }
}

/// <summary>
/// 本车道两个 DBServer 窗体所需的 UI 闸门（全局注入点；测试用毕请调 <see cref="ResetForTests"/>）。
/// 与 `RouteEditUnit.ShowFrmRouteEdit` / `UiSeam.MessageBox` 同一形态。
/// </summary>
public static class DBServerForms2Ui
{
    /// <summary>
    /// Delphi `Form.ShowModal` 的替身（默认弹真实模态框）。
    /// 测试注入即可返回 <see cref="DialogResult.OK"/> / <see cref="DialogResult.Cancel"/> 而不建窗口。
    /// </summary>
    public static Func<Form, DialogResult> ShowModal = f => f.ShowDialog();

    /// <summary>Delphi `TSaveDialog.Create(nil)` 的替身工厂。</summary>
    public static Func<TSaveDialogSeam> CreateSaveDialog = () => new TSaveDialogSeam();

    /// <summary>恢复默认（测试隔离用）。</summary>
    public static void ResetForTests()
    {
        ShowModal = f => f.ShowDialog();
        CreateSaveDialog = () => new TSaveDialogSeam();
    }
}

/// <summary>
/// Delphi `SysUtils.ExtractFileExt` / `ChangeFileExt` 的托管等价物。
///
/// <para>
/// `ChangeFileExt` 直接复用既有实现 <see cref="GXX.Core.Paradox.PxFileUtils.ChangeFileExt"/>
/// （已按原文处理"无扩展名分隔符时直接追加"与"不越过路径分隔符找 '.'"两条边界，**不**新建第二份）。
/// `ExtractFileExt` 按同一口径补齐（GXX.Core 侧暂无公开同名函数）。
/// </para>
/// </summary>
public static class P10bFileUtils
{
    /// <summary>
    /// SysUtils.ExtractFileExt：返回含 '.' 的扩展名；无扩展名（或 '.' 出现在最后一个路径分隔符之前）返回 ''。
    /// </summary>
    public static string ExtractFileExt(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return "";
        int p = fileName.LastIndexOf('.');
        int sep = fileName.LastIndexOfAny(new[] { '\\', '/', ':' });
        if (p <= sep) return "";
        return fileName.Substring(p);
    }

    /// <summary>SysUtils.ChangeFileExt（转调既有实现，避免第二份设施，§14.2）。</summary>
    public static string ChangeFileExt(string fileName, string newExt)
        => GXX.Core.Paradox.PxFileUtils.ChangeFileExt(fileName, newExt);
}

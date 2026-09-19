using System;

namespace GXX.DBServer;

/// <summary>Delphi Dialogs.TMsgDlgType。</summary>
public enum TMsgDlgType
{
    mtWarning = 0,
    mtError = 1,
    mtInformation = 2,
    mtConfirmation = 3,
    mtCustom = 4
}

/// <summary>Delphi Dialogs.TMsgDlgBtn（set of → [Flags]）。</summary>
[Flags]
public enum TMsgDlgButtons
{
    mbYes = 0x01,
    mbNo = 0x02,
    mbOK = 0x04,
    mbCancel = 0x08,
    mbAbort = 0x10,
    mbRetry = 0x20,
    mbIgnore = 0x40,
    mbAll = 0x80,
    mbNoToAll = 0x100,
    mbYesToAll = 0x200,
    mbHelp = 0x400
}

/// <summary>
/// 消息框接缝（Delphi Windows.MessageBox / Dialogs.MessageDlg / Dialogs.ShowMessage）。
/// 窗体校验分支全部走这里，单测注入记录器即可断言"弹了什么、弹了几次"，
/// 无需真实 UI，也不会阻塞。
/// </summary>
public static class UiSeam
{
    /// <summary>对应 Windows.MessageBox(Handle, Text, Caption, uType)。返回 IDOK/IDYES... 。</summary>
    public static Func<string, string, uint, int> MessageBox = DefaultMessageBox;

    /// <summary>对应 Dialogs.MessageDlg(Msg, DlgType, Buttons, HelpCtx)。返回 mrYes/mrNo... 。</summary>
    public static Func<string, TMsgDlgType, TMsgDlgButtons, int, int> MessageDlg = DefaultMessageDlg;

    /// <summary>对应 Dialogs.ShowMessage(Msg)。</summary>
    public static Action<string> ShowMessage = DefaultShowMessage;

    private static int DefaultMessageBox(string text, string caption, uint flags)
        => (int)System.Windows.Forms.MessageBox.Show(text, caption,
               System.Windows.Forms.MessageBoxButtons.OK,
               ((flags & TMsgBox.MB_ICONERROR) != 0) ? System.Windows.Forms.MessageBoxIcon.Error
             : ((flags & TMsgBox.MB_ICONINFORMATION) != 0) ? System.Windows.Forms.MessageBoxIcon.Information
             : System.Windows.Forms.MessageBoxIcon.None);

    private static int DefaultMessageDlg(string msg, TMsgDlgType dlgType, TMsgDlgButtons buttons, int helpCtx)
    {
        var btns = System.Windows.Forms.MessageBoxButtons.OK;
        if ((buttons & TMsgDlgButtons.mbYes) != 0 && (buttons & TMsgDlgButtons.mbNo) != 0)
            btns = System.Windows.Forms.MessageBoxButtons.YesNo;
        System.Windows.Forms.DialogResult r = System.Windows.Forms.MessageBox.Show(msg, "",
            btns,
            dlgType == TMsgDlgType.mtConfirmation ? System.Windows.Forms.MessageBoxIcon.Question : System.Windows.Forms.MessageBoxIcon.Information);
        return r == System.Windows.Forms.DialogResult.Yes ? TModalResult.mrYes : TModalResult.mrNo;
    }

    private static void DefaultShowMessage(string msg)
        => System.Windows.Forms.MessageBox.Show(msg);
}

namespace GXX.LogDataServer;

/// <summary>
/// Delphi Application.MessageBox / ShowMessage 1:1 等效层（与 GXX.M2Server.Forms.M2Forms、
/// GXX.LoginSrv.LoginSrvForms 同约定）：运行时弹 WinForms MessageBox，
/// 测试可注入捕获（LastMessage/LastCaption/NextAnswer）。
/// </summary>
public static class LogDataForms
{
    public const int MB_OK = 0x00;
    public const int MB_YESNO = 0x04;
    public const int MB_ICONERROR = 0x10;
    public const int MB_ICONQUESTION = 0x20;
    public const int MB_ICONINFORMATION = 0x40;
    public const int MB_ICONWARNING = 0x30;

    public const int IDOK = 1;
    public const int IDYES = 6;
    public const int IDNO = 7;
    public const int IDCANCEL = 2;

    public const int mrNone = 0;
    public const int mrOk = 1;
    public const int mrCancel = 2;

    public static System.Func<string, string, int, int>? MessageBoxHandler;

    public static string? LastMessage;
    public static string? LastCaption;

    public static int? NextAnswer;

    public static int MessageBox(string text, string caption, int flags)
    {
        LastMessage = text;
        LastCaption = caption;
        if (NextAnswer.HasValue)
        {
            int answer = NextAnswer.Value;
            NextAnswer = null;
            return answer;
        }
        if (MessageBoxHandler != null)
            return MessageBoxHandler(text, caption, flags);

        var buttons = (flags & MB_YESNO) != 0
            ? System.Windows.Forms.MessageBoxButtons.YesNo
            : System.Windows.Forms.MessageBoxButtons.OK;
        var icon = (flags & MB_ICONQUESTION) != 0 ? System.Windows.Forms.MessageBoxIcon.Question
                 : (flags & MB_ICONERROR) != 0 ? System.Windows.Forms.MessageBoxIcon.Error
                 : (flags & MB_ICONWARNING) != 0 ? System.Windows.Forms.MessageBoxIcon.Warning
                 : (flags & MB_ICONINFORMATION) != 0 ? System.Windows.Forms.MessageBoxIcon.Information
                 : System.Windows.Forms.MessageBoxIcon.None;
        return (int)System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon);
    }

    public static void ShowMessage(string text) => MessageBox(text, "", MB_OK);
}

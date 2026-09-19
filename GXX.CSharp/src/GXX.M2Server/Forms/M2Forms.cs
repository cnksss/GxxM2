namespace GXX.M2Server.Forms;

/// <summary>
/// Delphi Application.MessageBox 1:1 等效层：运行时弹 WinForms MessageBox，
/// 测试可注入捕获（LastMessage/LastCaption/NextAnswer），保证窗体校验分支可脱 UI 验证。
/// </summary>
public static class M2Forms
{
    public const int MB_OK = 0x00;
    public const int MB_YESNO = 0x04;
    public const int MB_ICONERROR = 0x10;
    public const int MB_ICONQUESTION = 0x20;
    public const int MB_ICONINFORMATION = 0x40;

    public const int IDOK = 1;
    public const int IDYES = 6;
    public const int IDNO = 7;
    public const int IDCANCEL = 2;

    /// <summary>Application.HintColor 等效（RefDlgConf/ColorBoxHint 使用）。</summary>
    public static System.Drawing.Color HintColor = System.Drawing.SystemColors.Info;

    /// <summary>测试注入：(text, caption, flags) → 返回值。</summary>
    public static Func<string, string, int, int>? MessageBoxHandler;

    /// <summary>测试捕获：最近一次弹窗文本/标题。</summary>
    public static string? LastMessage;
    public static string? LastCaption;

    /// <summary>测试注入：下一次 MessageBox 返回值（如 IDYES=6 / IDNO=7），弹一次后清除。</summary>
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

        var buttons = (flags & MB_YESNO) != 0 ? System.Windows.Forms.MessageBoxButtons.YesNo : System.Windows.Forms.MessageBoxButtons.OK;
        var icon = (flags & MB_ICONQUESTION) != 0 ? System.Windows.Forms.MessageBoxIcon.Question
                 : (flags & MB_ICONERROR) != 0 ? System.Windows.Forms.MessageBoxIcon.Error
                 : System.Windows.Forms.MessageBoxIcon.None;
        return (int)System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon);
    }

    public static void ErrorBox(string text, string caption = "错误信息")
        => MessageBox(text, caption, MB_OK | MB_ICONERROR);
}

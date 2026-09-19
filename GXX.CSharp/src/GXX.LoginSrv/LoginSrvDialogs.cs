using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Util;

namespace GXX.LoginSrv;

/// <summary>Delphi TStrings.Text / SetTextStr 语义（GXX.Core.TStringList 未提供 Text 属性）。</summary>
public static class DelphiStrings
{
    /// <summary>TStrings.GetTextStr：每行后追加 sLineBreak(#13#10)（含最后一行）。</summary>
    public static string GetText(TStringList list)
    {
        var sb = new StringBuilder();
        foreach (string s in list.AsEnumerable())
            sb.Append(s).Append("\r\n");
        return sb.ToString();
    }

    /// <summary>TStrings.SetTextStr：按 #13 / #10 / #13#10 切行，末尾换行不产生空行。</summary>
    public static void SetText(TStringList list, string text)
    {
        list.Clear();
        if (string.IsNullOrEmpty(text)) return;
        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '\r' || c == '\n')
            {
                list.Add(text.Substring(start, i - start));
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                start = i + 1;
            }
        }
        if (start < text.Length) list.Add(text.Substring(start));
    }

    /// <summary>Delphi TStrings.IndexOf（对应 GXX.Core.TStringList.IndexOf 的 Ordinal 语义）。</summary>
    public static int IndexOf(TStringList list, string s) => list.IndexOf(s);
}

/// <summary>
/// BasicSet.pas:241 GetAveCharSize + :252 InputQueryEx 1:1（uFrmRemoteQuerySetting.pas:96/107 同名同实现）。
/// 接缝：GetTextExtentPoint 的 Canvas 度量由 WinForms TextRenderer 等效实现（返回字符平均宽度，与原文 div 52 同）。
/// </summary>
public static class LoginSrvInputQuery
{
    /// <summary>BasicSet.pas:254 FORM_WIDTH = 280。</summary>
    public const int FORM_WIDTH = 280;

    /// <summary>测试注入：(ACaption, APrompt, AHint, Value) → true 表示确定并返回新值。</summary>
    public static Func<string, string, string, string, (bool ok, string value)>? Handler;

    /// <summary>BasicSet.pas:241 GetAveCharSize（Result.X := Result.X div 52）。</summary>
    public static System.Drawing.Point GetAveCharSize(System.Drawing.Graphics canvas, System.Drawing.Font font)
    {
        var buf = new char[52];
        for (int I = 0; I <= 25; I++) buf[I] = (char)('A' + I);
        for (int I = 0; I <= 25; I++) buf[I + 26] = (char)('a' + I);
        System.Drawing.SizeF size = canvas.MeasureString(new string(buf), font);
        int x = (int)size.Width / 52;
        return new System.Drawing.Point(x, (int)size.Height);
    }

    /// <summary>Delphi MulDiv（四舍五入）。</summary>
    private static int MulDiv(int number, int numerator, int denominator)
        => (int)Math.Round((double)number * numerator / denominator, MidpointRounding.AwayFromZero);

    /// <summary>
    /// BasicSet.pas:252 InputQueryEx。
    /// showModal=false 时通过 Handler 注入结果（测试/宿主），否则弹真实输入窗体。
    /// </summary>
    public static bool InputQueryEx(string ACaption, string APrompt, string AHint, ref string Value, bool showModal = true)
    {
        if (!showModal || Handler != null)
        {
            var (ok, value) = Handler != null ? Handler(ACaption, APrompt, AHint, Value) : (false, Value);
            if (ok) Value = value;
            return ok;
        }

        bool Result = false;
        using var Form = new System.Windows.Forms.Form();
        Form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        Form.Text = ACaption;
        Form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Form.MinimizeBox = false;
        Form.MaximizeBox = false;
        Form.ShowInTaskbar = false;

        using var g = Form.CreateGraphics();
        System.Drawing.Point DialogUnits = GetAveCharSize(g, Form.Font);
        Form.ClientSize = new System.Drawing.Size(MulDiv(FORM_WIDTH, DialogUnits.X, 4), 120);

        var Prompt = new System.Windows.Forms.Label
        {
            Text = APrompt,
            Left = MulDiv(8, DialogUnits.X, 4),
            Top = MulDiv(8, DialogUnits.Y, 8),
            MaximumSize = new System.Drawing.Size(MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4), 0),
            AutoSize = true,
        };
        Form.Controls.Add(Prompt);

        var Edit = new System.Windows.Forms.TextBox
        {
            Left = Prompt.Left,
            Top = Prompt.Top + Prompt.Height + 5,
            Width = MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4),
            MaxLength = 255,
            Text = Value,
        };
        Edit.SelectAll();
        Form.Controls.Add(Edit);

        int ButtonTop = Edit.Top + Edit.Height + 8;
        int ButtonWidth = MulDiv(50, DialogUnits.X, 4);
        int ButtonHeight = MulDiv(14, DialogUnits.Y, 8);

        var OkBtn = new System.Windows.Forms.Button
        {
            Text = "确定",
            DialogResult = System.Windows.Forms.DialogResult.OK,
            Left = Edit.Left + Edit.Width - ButtonWidth * 2 - 6,
            Top = ButtonTop,
            Width = ButtonWidth,
            Height = ButtonHeight,
        };
        Form.Controls.Add(OkBtn);
        Form.AcceptButton = OkBtn;

        var CancelBtn = new System.Windows.Forms.Button
        {
            Text = "取消",
            DialogResult = System.Windows.Forms.DialogResult.Cancel,
            Left = Edit.Left + Edit.Width - ButtonWidth,
            Top = ButtonTop,
            Width = ButtonWidth,
            Height = ButtonHeight,
        };
        Form.Controls.Add(CancelBtn);
        Form.CancelButton = CancelBtn;
        Form.ClientSize = new System.Drawing.Size(Form.ClientSize.Width, CancelBtn.Top + CancelBtn.Height + 10);

        var Hint = new System.Windows.Forms.Label
        {
            Text = AHint,
            ForeColor = System.Drawing.Color.Blue,
            Left = Edit.Left,
            Top = ButtonTop + (ButtonHeight - 12) / 2,
            MaximumSize = new System.Drawing.Size(MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4), 0),
            AutoSize = true,
        };
        Form.Controls.Add(Hint);

        if (Form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            Value = Edit.Text;
            Result = true;
        }
        return Result;
    }
}

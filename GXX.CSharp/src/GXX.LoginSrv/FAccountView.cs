namespace GXX.LoginSrv;

/// <summary>
/// FAccountView.pas TFrmAccountView 1:1（充值记录查看：在 ListBox 中按 Enter 定位匹配项）。
/// 源码：FAccountView.pas:29-52；DFM：FAccountView.dfm（Caption='充值记录', ClientWidth=402, ClientHeight=456, bsSingle）。
/// </summary>
public sealed class TFrmAccountView : System.Windows.Forms.Form
{
    // DFM: EdFindID TEdit Left=8 Top=408 Width=193 Height=20 TabOrder=0
    public System.Windows.Forms.TextBox EdFindID = null!;
    // DFM: EdFindIP TEdit Left=208 Top=408 Width=185 Height=20 TabOrder=1
    public System.Windows.Forms.TextBox EdFindIP = null!;
    // DFM: ListBox1 TListBox Left=8 Top=8 Width=193 Height=393 ItemHeight=12 TabOrder=2
    public System.Windows.Forms.ListBox ListBox1 = null!;
    // DFM: ListBox2 TListBox Left=208 Top=8 Width=193 Height=393 ItemHeight=12 TabOrder=3
    public System.Windows.Forms.ListBox ListBox2 = null!;

    public TFrmAccountView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // DFM: Caption = '充值记录'
        Text = "充值记录";
        // DFM: BorderStyle = bsSingle / BorderIcons = [biSystemMenu, biMinimize]
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;
        // DFM: ClientWidth = 402  ClientHeight = 456
        ClientSize = new System.Drawing.Size(402, 456);

        EdFindID = new System.Windows.Forms.TextBox { Left = 8, Top = 408, Width = 193, Height = 20, TabIndex = 0 };
        EdFindIP = new System.Windows.Forms.TextBox { Left = 208, Top = 408, Width = 185, Height = 20, TabIndex = 1 };
        ListBox1 = new System.Windows.Forms.ListBox { Left = 8, Top = 8, Width = 193, Height = 393, TabIndex = 2, IntegralHeight = false };
        ListBox2 = new System.Windows.Forms.ListBox { Left = 208, Top = 8, Width = 193, Height = 393, TabIndex = 3, IntegralHeight = false };

        EdFindID.KeyPress += (s, e) => { char k = e.KeyChar; EdFindIDKeyPress(s, ref k); e.KeyChar = k; };
        EdFindIP.KeyPress += (s, e) => { char k = e.KeyChar; EdFindIPKeyPress(s, ref k); e.KeyChar = k; };

        Controls.Add(EdFindID);
        Controls.Add(EdFindIP);
        Controls.Add(ListBox1);
        Controls.Add(ListBox2);
    }

    /// <summary>
    /// FAccountView.pas:29 EdFindIDKeyPress。
    /// ★ 原文循环**不 break**：同名多项时 ItemIndex 最终停在**最后一个**匹配项。
    /// </summary>
    public void EdFindIDKeyPress(object? Sender, ref char Key)
    {
        if (Key != '\x0D') return;
        for (int I = 0; I <= ListBox1.Items.Count - 1; I++)
        {
            if (string.Equals(EdFindID.Text, ListBox1.Items[I] as string ?? ""))
                ListBox1.SelectedIndex = I;
        }
    }

    /// <summary>FAccountView.pas:42 EdFindIPKeyPress（同样不 break）。</summary>
    public void EdFindIPKeyPress(object? Sender, ref char Key)
    {
        if (Key != '\x0D') return;
        for (int I = 0; I <= ListBox2.Items.Count - 1; I++)
        {
            if (string.Equals(EdFindIP.Text, ListBox2.Items[I] as string ?? ""))
                ListBox2.SelectedIndex = I;
        }
    }
}

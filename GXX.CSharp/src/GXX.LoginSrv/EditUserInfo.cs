using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.LoginSrv;

/// <summary>
/// EditUserInfo.pas TFrmUserInfoEdit 1:1（账号信息录入/编辑）。
/// 源码：EditUserInfo.pas:62-164；DFM：EditUserInfo.dfm（Caption='编辑帐号信息', ClientWidth=305, ClientHeight=387, bsSingle）。
/// </summary>
public sealed class TFrmUserInfoEdit : System.Windows.Forms.Form
{
    // DFM: 各 TLabel 的 Caption 1:1
    public System.Windows.Forms.Label Label1 = null!;   // '帐号:'
    public System.Windows.Forms.Label Label2 = null!;   // '密码:'
    public System.Windows.Forms.Label Label3 = null!;   // '用户名称:'
    public System.Windows.Forms.Label Label4 = null!;   // '生日:'
    public System.Windows.Forms.Label Label5 = null!;   // '电话:'
    public System.Windows.Forms.Label Label6 = null!;   // '问题一:'
    public System.Windows.Forms.Label Label7 = null!;   // '答案一:'
    public System.Windows.Forms.Label Label8 = null!;   // '电子邮箱:'
    public System.Windows.Forms.Label Label9 = null!;   // '身份证号:'
    public System.Windows.Forms.Label Label10 = null!;  // '问题二:'
    public System.Windows.Forms.Label Label11 = null!;  // '答案二:'
    public System.Windows.Forms.Label Label12 = null!;  // '移动电话:'
    public System.Windows.Forms.Label Label13 = null!;  // '备注信息:'
    public System.Windows.Forms.Label Label14 = null!;  // '二级密码:'

    public System.Windows.Forms.TextBox edtAccountName = null!;
    public System.Windows.Forms.TextBox edtPassword = null!;
    public System.Windows.Forms.TextBox edtUserName = null!;
    public System.Windows.Forms.TextBox edtIDCard = null!;
    public System.Windows.Forms.TextBox edtBirthday = null!;
    public System.Windows.Forms.TextBox edtQuestions1 = null!;
    public System.Windows.Forms.TextBox edtAnswers1 = null!;
    public System.Windows.Forms.TextBox edtQuestions2 = null!;
    public System.Windows.Forms.TextBox edtAnswers2 = null!;
    public System.Windows.Forms.TextBox edtPhone = null!;
    public System.Windows.Forms.TextBox edtMobilePhone = null!;
    public System.Windows.Forms.TextBox edtMemo = null!;
    public System.Windows.Forms.TextBox edtL2Password = null!;
    public System.Windows.Forms.TextBox edtMail = null!;

    public System.Windows.Forms.Button Button1 = null!;   // '确定(&O)'
    public System.Windows.Forms.Button Button2 = null!;   // '取消(&C)' ModalResult=2
    public System.Windows.Forms.Button Button3 = null!;   // '解锁(&U)' ModalResult=2
    public System.Windows.Forms.CheckBox chkEditAccount = null!;   // '修改数据'

    /// <summary>
    /// 测试注入：showModal=false 时用于替代 `ShowModal = mrOK` 的判定（默认 mrCancel=2，即不提交）。
    /// </summary>
    public int SimulatedModalResult = LoginSrvForms.mrCancel;

    public TFrmUserInfoEdit()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // DFM: Caption = '编辑帐号信息' / bsSingle / [biSystemMenu, biMinimize]
        Text = "编辑帐号信息";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;
        // DFM: ClientWidth = 305  ClientHeight = 387
        ClientSize = new System.Drawing.Size(305, 387);

        static System.Windows.Forms.Label Lb(string cap, int left, int top, int width)
            => new() { Text = cap, Left = left, Top = top, Width = width, Height = 12, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleRight };

        // DFM 标签：Label1(43,20) Label2(43,44) Label3(19,68) Label9(19,92) Label4(43,116) Label6(31,140)
        //          Label7(31,164) Label10(31,188) Label11(31,212) Label5(43,236) Label12(19,260)
        //          Label13(19,284) Label8(19,308) Label14(19,332)
        Label1 = Lb("帐号:", 43, 20, 30);
        Label2 = Lb("密码:", 43, 44, 30);
        Label3 = Lb("用户名称:", 19, 68, 54);
        Label9 = Lb("身份证号:", 19, 92, 54);
        Label4 = Lb("生日:", 43, 116, 30);
        Label6 = Lb("问题一:", 31, 140, 42);
        Label7 = Lb("答案一:", 31, 164, 42);
        Label10 = Lb("问题二:", 31, 188, 42);
        Label11 = Lb("答案二:", 31, 212, 42);
        Label5 = Lb("电话:", 43, 236, 30);
        Label12 = Lb("移动电话:", 19, 260, 54);
        Label13 = Lb("备注信息:", 19, 284, 54);
        Label8 = Lb("电子邮箱:", 19, 308, 54);
        Label14 = Lb("二级密码:", 19, 332, 54);

        static System.Windows.Forms.TextBox Ed(int left, int top, int width, int maxLen, int tab)
            => new() { Left = left, Top = top, Width = width, Height = 20, MaxLength = maxLen, TabIndex = tab };

        // DFM: edtAccountName(78,16,121,MaxLength=10,Enabled=False,TabOrder=0)
        edtAccountName = Ed(78, 16, 121, 10, 0); edtAccountName.Enabled = false;
        // DFM: edtPassword(78,40,121,MaxLength=10,TabOrder=1)
        edtPassword = Ed(78, 40, 121, 10, 1);
        // DFM: edtUserName(78,64,121,MaxLength=20,TabOrder=2)
        edtUserName = Ed(78, 64, 121, 20, 2);
        // DFM: edtIDCard(78,88,121,MaxLength=14,TabOrder=3)
        edtIDCard = Ed(78, 88, 121, 14, 3);
        // DFM: edtBirthday(78,112,121,MaxLength=10,TabOrder=4)
        edtBirthday = Ed(78, 112, 121, 10, 4);
        // DFM: edtQuestions1(78,136,209,MaxLength=20,TabOrder=5)
        edtQuestions1 = Ed(78, 136, 209, 20, 5);
        // DFM: edtAnswers1(78,160,209,MaxLength=12,TabOrder=6)
        edtAnswers1 = Ed(78, 160, 209, 12, 6);
        // DFM: edtQuestions2(78,184,209,MaxLength=20,TabOrder=7)
        edtQuestions2 = Ed(78, 184, 209, 20, 7);
        // DFM: edtAnswers2(78,208,209,MaxLength=12,TabOrder=8)
        edtAnswers2 = Ed(78, 208, 209, 12, 8);
        // DFM: edtPhone(78,232,121,MaxLength=14,TabOrder=9)
        edtPhone = Ed(78, 232, 121, 14, 9);
        // DFM: edtMobilePhone(78,256,121,MaxLength=13,TabOrder=10)
        edtMobilePhone = Ed(78, 256, 121, 13, 10);
        // DFM: edtMemo(78,280,209,MaxLength=20,TabOrder=11)
        edtMemo = Ed(78, 280, 209, 20, 11);
        // DFM: edtMail(78,304,209,MaxLength=40,TabOrder=12)
        edtMail = Ed(78, 304, 209, 40, 12);
        // DFM: edtL2Password(78,328,209,MaxLength=20,TabOrder=13)
        edtL2Password = Ed(78, 328, 209, 20, 13);

        // DFM: Button1(147,356,66,25) Caption='确定(&O)' OnClick=Button1Click TabOrder=14
        Button1 = new System.Windows.Forms.Button { Text = "确定(&O)", Left = 147, Top = 356, Width = 66, Height = 25, TabIndex = 14 };
        Button1.Click += (s, e) => Button1Click(s);
        // DFM: Button2(221,356,66,25) Caption='取消(&C)' ModalResult=2 TabOrder=15
        Button2 = new System.Windows.Forms.Button { Text = "取消(&C)", Left = 221, Top = 356, Width = 66, Height = 25, TabIndex = 15, DialogResult = System.Windows.Forms.DialogResult.Cancel };
        // DFM: chkEditAccount(218,16,69,17) Caption='修改数据' OnClick=chkEditAccountClick TabOrder=16
        chkEditAccount = new System.Windows.Forms.CheckBox { Text = "修改数据", Left = 218, Top = 16, Width = 69, Height = 17, TabIndex = 16, AutoSize = false };
        chkEditAccount.Click += (s, e) => chkEditAccountClick(s);
        // DFM: Button3(218,107,69,25) Caption='解锁(&U)' ModalResult=2 OnClick=Button3Click TabOrder=17
        Button3 = new System.Windows.Forms.Button { Text = "解锁(&U)", Left = 218, Top = 107, Width = 69, Height = 25, TabIndex = 17, DialogResult = System.Windows.Forms.DialogResult.Cancel };
        Button3.Click += (s, e) => Button3Click(s);

        Controls.Add(Label1); Controls.Add(Label2); Controls.Add(Label3); Controls.Add(Label4);
        Controls.Add(Label5); Controls.Add(Label6); Controls.Add(Label7); Controls.Add(Label8);
        Controls.Add(Label9); Controls.Add(Label10); Controls.Add(Label11); Controls.Add(Label12);
        Controls.Add(Label13); Controls.Add(Label14);
        Controls.Add(edtAccountName); Controls.Add(edtPassword); Controls.Add(edtUserName); Controls.Add(edtIDCard);
        Controls.Add(edtBirthday); Controls.Add(edtQuestions1); Controls.Add(edtAnswers1); Controls.Add(edtQuestions2);
        Controls.Add(edtAnswers2); Controls.Add(edtPhone); Controls.Add(edtMobilePhone); Controls.Add(edtMemo);
        Controls.Add(edtMail); Controls.Add(edtL2Password);
        Controls.Add(Button1); Controls.Add(Button2); Controls.Add(Button3); Controls.Add(chkEditAccount);
    }

    /// <summary>EditUserInfo.pas:62 chkEditAccountClick（12 个编辑框随勾选启停；不含帐号/密码）。</summary>
    public void chkEditAccountClick(object? Sender)
    {
        bool IsEdit = chkEditAccount.Checked;
        edtUserName.Enabled = IsEdit;
        edtIDCard.Enabled = IsEdit;
        edtBirthday.Enabled = IsEdit;
        edtQuestions1.Enabled = IsEdit;
        edtAnswers1.Enabled = IsEdit;
        edtQuestions2.Enabled = IsEdit;
        edtAnswers2.Enabled = IsEdit;
        edtPhone.Enabled = IsEdit;
        edtMobilePhone.Enabled = IsEdit;
        edtMemo.Enabled = IsEdit;
        edtL2Password.Enabled = IsEdit;
        edtMail.Enabled = IsEdit;
    }

    /// <summary>
    /// EditUserInfo.pas:83 InputAccountInfo。
    /// showModal=false 供测试/宿主复用：以 SimulatedModalResult 替代 `ShowModal = mrOK` 判定。
    /// </summary>
    public bool InputAccountInfo(bool boNew, ref TAccountInfo AccountInfo, bool showModal = true)
    {
        bool Result = false;
        if (!boNew)
        {
            chkEditAccount.Enabled = true;
            chkEditAccount.Checked = false;
            chkEditAccountClick(this);
            edtAccountName.Enabled = false;
        }
        else
        {
            chkEditAccount.Enabled = false;
            chkEditAccount.Checked = true;
            chkEditAccountClick(this);
            edtAccountName.Enabled = true;
        }

        if (Label14.Visible)
        {
            Label14.Text = "二级密码:";
        }

        edtAccountName.Text = AccountInfo.AccountNameStr;
        edtPassword.Text = AccountInfo.PasswordStr;
        edtUserName.Text = AccountInfo.UserNameStr;
        edtIDCard.Text = AccountInfo.IDCardStr;
        edtBirthday.Text = AccountInfo.BirthDayStr;
        edtQuestions1.Text = AccountInfo.Questions1Str;
        edtAnswers1.Text = AccountInfo.Answers1Str;
        edtQuestions2.Text = AccountInfo.Questions2Str;
        edtAnswers2.Text = AccountInfo.Answers2Str;
        edtPhone.Text = AccountInfo.PhoneStr;
        edtMobilePhone.Text = AccountInfo.MobilePhoneStr;
        edtMemo.Text = AccountInfo.MemoStr;
        edtL2Password.Text = AccountInfo.L2PasswordStr;
        edtMail.Text = AccountInfo.MailStr;

        int mr = showModal ? (int)ShowDialog() : SimulatedModalResult;
        if (mr != LoginSrvForms.mrOk) return false;
        if (boNew)
        {
            AccountInfo.AccountNameStr = DelphiRTL.Trim(edtAccountName.Text);
        }
        AccountInfo.PasswordStr = DelphiRTL.Trim(edtPassword.Text);
        AccountInfo.UserNameStr = DelphiRTL.Trim(edtUserName.Text);
        AccountInfo.IDCardStr = DelphiRTL.Trim(edtIDCard.Text);
        AccountInfo.BirthDayStr = DelphiRTL.Trim(edtBirthday.Text);
        AccountInfo.Questions1Str = DelphiRTL.Trim(edtQuestions1.Text);
        AccountInfo.Answers1Str = DelphiRTL.Trim(edtAnswers1.Text);
        AccountInfo.Questions2Str = DelphiRTL.Trim(edtQuestions2.Text);
        AccountInfo.Answers2Str = DelphiRTL.Trim(edtAnswers2.Text);
        AccountInfo.PhoneStr = DelphiRTL.Trim(edtPhone.Text);
        AccountInfo.MobilePhoneStr = DelphiRTL.Trim(edtMobilePhone.Text);
        AccountInfo.MemoStr = DelphiRTL.Trim(edtMemo.Text);
        AccountInfo.L2PasswordStr = DelphiRTL.Trim(edtL2Password.Text);
        AccountInfo.MailStr = DelphiRTL.Trim(edtMail.Text);
        Result = true;
        return Result;
    }

    /// <summary>EditUserInfo.pas:141 Button3Click（解锁账号；成功才提示）。</summary>
    public void Button3Click(object? Sender)
    {
        if (LoginSrvShare.g_AccountDB != null && LoginSrvShare.g_AccountDB.UnLockAccount(edtAccountName.Text))
        {
            LoginSrvForms.ShowMessage("解锁成功");
        }
    }

    /// <summary>
    /// EditUserInfo.pas:152 Button1Click。
    /// ★ 校验用的是 `Length(Trim(...)) &lt;= MIN_ACCOUNT_LEN`（≤4，而非 &lt;4），原文如此。
    /// </summary>
    public void Button1Click(object? Sender)
    {
        if (edtAccountName.Enabled)
        {
            if (DelphiRTL.Trim(edtAccountName.Text).Length <= Grobal2Const.MIN_ACCOUNT_LEN)
            {
                LoginSrvForms.ShowMessage("帐户名称长度最低4个字符或2汉字");
                return;
            }
        }

        DialogResult = System.Windows.Forms.DialogResult.OK;
    }
}

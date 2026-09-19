using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.LoginSrv;
using Xunit;

namespace GXX.LoginSrv.Tests;

/// <summary>EditUserInfo.pas TFrmUserInfoEdit 1:1 测试。</summary>
public sealed class EditUserInfoTests : IDisposable
{
    public EditUserInfoTests()
    {
        LoginSrvShare.ResetForTests();
        LoginSrvForms.MessageBoxHandler = (_, _, _) => LoginSrvForms.IDOK;
        LoginSrvForms.NextAnswer = null;
        LoginSrvForms.LastMessage = null;
        LoginSrvForms.LastCaption = null;
    }

    public void Dispose()
    {
        LoginSrvForms.MessageBoxHandler = null;
        LoginSrvForms.NextAnswer = null;
        LoginSrvShare.ResetForTests();
    }

    private static TFrmUserInfoEdit NewForm() => StaRunner.New(() => new TFrmUserInfoEdit());

    [StaFact]
    public void FormCreate_LabelsAndMaxLengthsMatchDfm()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            Assert.Equal("编辑帐号信息", f.Text);
            Assert.Equal("帐号:", f.Label1.Text);
            Assert.Equal("密码:", f.Label2.Text);
            Assert.Equal("用户名称:", f.Label3.Text);
            Assert.Equal("生日:", f.Label4.Text);
            Assert.Equal("电话:", f.Label5.Text);
            Assert.Equal("问题一:", f.Label6.Text);
            Assert.Equal("答案一:", f.Label7.Text);
            Assert.Equal("电子邮箱:", f.Label8.Text);
            Assert.Equal("身份证号:", f.Label9.Text);
            Assert.Equal("问题二:", f.Label10.Text);
            Assert.Equal("答案二:", f.Label11.Text);
            Assert.Equal("移动电话:", f.Label12.Text);
            Assert.Equal("备注信息:", f.Label13.Text);
            Assert.Equal("二级密码:", f.Label14.Text);

            Assert.Equal("确定(&O)", f.Button1.Text);
            Assert.Equal("取消(&C)", f.Button2.Text);
            Assert.Equal("解锁(&U)", f.Button3.Text);
            Assert.Equal("修改数据", f.chkEditAccount.Text);

            Assert.Equal(10, f.edtAccountName.MaxLength);
            Assert.Equal(10, f.edtPassword.MaxLength);
            Assert.Equal(20, f.edtUserName.MaxLength);
            Assert.Equal(14, f.edtIDCard.MaxLength);      // DFM 如此（AccountInfo.IDCard 实为 string[18]）
            Assert.Equal(13, f.edtMobilePhone.MaxLength);
            Assert.Equal(40, f.edtMail.MaxLength);
            Assert.False(f.edtAccountName.Enabled);       // DFM: Enabled = False
        });
    }

    [StaFact]
    public void chkEditAccountClick_TogglesTwelveEdits_NotAccountNorPassword()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            f.chkEditAccount.Checked = true;
            f.chkEditAccountClick(f);

            Assert.True(f.edtUserName.Enabled);
            Assert.True(f.edtIDCard.Enabled);
            Assert.True(f.edtBirthday.Enabled);
            Assert.True(f.edtQuestions1.Enabled);
            Assert.True(f.edtAnswers1.Enabled);
            Assert.True(f.edtQuestions2.Enabled);
            Assert.True(f.edtAnswers2.Enabled);
            Assert.True(f.edtPhone.Enabled);
            Assert.True(f.edtMobilePhone.Enabled);
            Assert.True(f.edtMemo.Enabled);
            Assert.True(f.edtL2Password.Enabled);
            Assert.True(f.edtMail.Enabled);
            // 帐号/密码不在启停列表内（原文如此）；edtPassword 的 DFM 默认 Enabled=True，全程不动
            Assert.False(f.edtAccountName.Enabled);
            Assert.True(f.edtPassword.Enabled);

            f.chkEditAccount.Checked = false;
            f.chkEditAccountClick(f);
            Assert.False(f.edtUserName.Enabled);
            Assert.False(f.edtMail.Enabled);
            Assert.True(f.edtPassword.Enabled);   // 不受影响
        });
    }

    [StaFact]
    public void InputAccountInfo_NewMode_UnchecksEnabledAndEnablesAccountEdit()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            TAccountInfo info = default;
            f.SimulatedModalResult = LoginSrvForms.mrCancel;

            Assert.False(f.InputAccountInfo(true, ref info, showModal: false));

            Assert.False(f.chkEditAccount.Enabled);
            Assert.True(f.chkEditAccount.Checked);
            Assert.True(f.edtAccountName.Enabled);
            Assert.True(f.edtUserName.Enabled);
        });
    }

    [StaFact]
    public void InputAccountInfo_EditMode_ChecksEnabledAndDisablesAccountEdit()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            TAccountInfo info = default;
            f.SimulatedModalResult = LoginSrvForms.mrCancel;

            Assert.False(f.InputAccountInfo(false, ref info, showModal: false));

            Assert.True(f.chkEditAccount.Enabled);
            Assert.False(f.chkEditAccount.Checked);
            Assert.False(f.edtAccountName.Enabled);
            Assert.False(f.edtUserName.Enabled);
        });
    }

    [StaFact]
    public void InputAccountInfo_LoadsAllFieldsIntoControls()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            TAccountInfo info = default;
            info.AccountNameStr = "alice";
            info.PasswordStr = "pwd";
            info.UserNameStr = "u";
            info.IDCardStr = "id";
            info.BirthDayStr = "bd";
            info.Questions1Str = "q1";
            info.Answers1Str = "a1";
            info.Questions2Str = "q2";
            info.Answers2Str = "a2";
            info.PhoneStr = "ph";
            info.MobilePhoneStr = "mp";
            info.MemoStr = "memo";
            info.L2PasswordStr = "l2";
            info.MailStr = "mail";

            f.InputAccountInfo(false, ref info, showModal: false);

            Assert.Equal("alice", f.edtAccountName.Text);
            Assert.Equal("pwd", f.edtPassword.Text);
            Assert.Equal("u", f.edtUserName.Text);
            Assert.Equal("id", f.edtIDCard.Text);
            Assert.Equal("bd", f.edtBirthday.Text);
            Assert.Equal("q1", f.edtQuestions1.Text);
            Assert.Equal("a1", f.edtAnswers1.Text);
            Assert.Equal("q2", f.edtQuestions2.Text);
            Assert.Equal("a2", f.edtAnswers2.Text);
            Assert.Equal("ph", f.edtPhone.Text);
            Assert.Equal("mp", f.edtMobilePhone.Text);
            Assert.Equal("memo", f.edtMemo.Text);
            Assert.Equal("l2", f.edtL2Password.Text);
            Assert.Equal("mail", f.edtMail.Text);
        });
    }

    [StaFact]
    public void InputAccountInfo_Cancel_ReturnsFalse_AndDoesNotWriteBack()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            TAccountInfo info = default;
            info.AccountNameStr = "alice";
            info.PasswordStr = "orig";
            f.SimulatedModalResult = LoginSrvForms.mrCancel;
            f.OnBeforeModalCheck = form => form.edtPassword.Text = "changed";

            Assert.False(f.InputAccountInfo(false, ref info, showModal: false));
            Assert.Equal("orig", info.PasswordStr);   // 未提交
        });
    }

    [StaFact]
    public void InputAccountInfo_Ok_TrimsAndWritesAllFields()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            TAccountInfo info = default;
            info.AccountNameStr = "alice";
            f.SimulatedModalResult = LoginSrvForms.mrOk;
            f.OnBeforeModalCheck = form =>
            {
                form.edtPassword.Text = "  pwd  ";
                form.edtUserName.Text = " u ";
                form.edtIDCard.Text = " i ";
                form.edtBirthday.Text = " b ";
                form.edtQuestions1.Text = " q1 ";
                form.edtAnswers1.Text = " a1 ";
                form.edtQuestions2.Text = " q2 ";
                form.edtAnswers2.Text = " a2 ";
                form.edtPhone.Text = " ph ";
                form.edtMobilePhone.Text = " mp ";
                form.edtMemo.Text = " memo ";
                form.edtL2Password.Text = " l2 ";
                form.edtMail.Text = " mail ";
                form.edtAccountName.Text = "  NEWNAME  ";   // boNew=false → 不写回
            };

            Assert.True(f.InputAccountInfo(false, ref info, showModal: false));

            Assert.Equal("alice", info.AccountNameStr);   // false 分支不写 AccountName
            Assert.Equal("pwd", info.PasswordStr);
            Assert.Equal("u", info.UserNameStr);
            Assert.Equal("i", info.IDCardStr);
            Assert.Equal("b", info.BirthDayStr);
            Assert.Equal("q1", info.Questions1Str);
            Assert.Equal("a1", info.Answers1Str);
            Assert.Equal("q2", info.Questions2Str);
            Assert.Equal("a2", info.Answers2Str);
            Assert.Equal("ph", info.PhoneStr);
            Assert.Equal("mp", info.MobilePhoneStr);
            Assert.Equal("memo", info.MemoStr);
            Assert.Equal("l2", info.L2PasswordStr);
            Assert.Equal("mail", info.MailStr);
        });
    }

    [StaFact]
    public void InputAccountInfo_NewMode_Ok_WritesAccountName()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            TAccountInfo info = default;
            f.SimulatedModalResult = LoginSrvForms.mrOk;
            f.OnBeforeModalCheck = form => form.edtAccountName.Text = "  newbie  ";

            Assert.True(f.InputAccountInfo(true, ref info, showModal: false));
            Assert.Equal("newbie", info.AccountNameStr);
        });
    }

    /// <summary>Label14.Visible 时 Caption 复位为 '二级密码:'（原文 101-104 行）。</summary>
    [StaFact]
    public void InputAccountInfo_ResetsLabel14CaptionWhenVisible()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            f.Label14.Text = "other";
            TAccountInfo info = default;
            f.InputAccountInfo(false, ref info, showModal: false);
            Assert.Equal("二级密码:", f.Label14.Text);

            f.Label14_Visible = false;
            f.Label14.Text = "other";
            f.InputAccountInfo(false, ref info, showModal: false);
            Assert.Equal("other", f.Label14.Text);   // Visible=False → 不重置
        });
    }

    /// <summary>★ 校验用 `Length(Trim(...)) &lt;= MIN_ACCOUNT_LEN(4)`：4 个字符即被拒。</summary>
    [StaTheory]
    [InlineData("abcd", true)]
    [InlineData("abc", true)]
    [InlineData("", true)]
    [InlineData("abcde", false)]
    [InlineData("  abcd  ", true)]
    [InlineData("  abcde  ", false)]
    public void Button1Click_AccountNameLengthRule(string name, bool rejected)
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            f.edtAccountName.Enabled = true;   // 触发校验分支
            f.edtAccountName.Text = name;

            f.Button1Click(f);

            if (rejected)
            {
                Assert.Equal("帐户名称长度最低4个字符或2汉字", LoginSrvForms.LastMessage);
                Assert.NotEqual(System.Windows.Forms.DialogResult.OK, f.DialogResult);
            }
            else
            {
                Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
            }
        });
    }

    [StaFact]
    public void Button1Click_AccountDisabled_SkipsValidation()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmUserInfoEdit();
            f.edtAccountName.Enabled = false;
            f.edtAccountName.Text = "";

            f.Button1Click(f);

            Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void Button3Click_Success_ShowsUnlockMessage()
    {
        StaRunner.New(() =>
        {
            var db = new InMemoryAccountDb();
            TAccountInfo info = default;
            info.AccountNameStr = "alice";
            db.Store["alice"] = info;
            LoginSrvShare.g_AccountDB = db;

            using var f = new TFrmUserInfoEdit();
            f.edtAccountName.Text = "alice";
            f.Button3Click(f);

            Assert.Equal("解锁成功", LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void Button3Click_Failure_NoMessage()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_AccountDB = new InMemoryAccountDb();
            using var f = new TFrmUserInfoEdit();
            f.edtAccountName.Text = "ghost";
            f.Button3Click(f);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void Button3Click_NoAccountDb_NoMessage()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_AccountDB = null;
            using var f = new TFrmUserInfoEdit();
            f.Button3Click(f);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }
}

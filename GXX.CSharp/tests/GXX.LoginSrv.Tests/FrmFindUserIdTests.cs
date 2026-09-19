using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.LoginSrv;
using Xunit;

namespace GXX.LoginSrv.Tests;

/// <summary>FrmFindId.pas TFrmFindUserId 1:1 测试。</summary>
public sealed class FrmFindUserIdTests : IDisposable
{
    private readonly InMemoryAccountDb _db = new();

    public FrmFindUserIdTests()
    {
        LoginSrvShare.ResetForTests();
        LoginSrvShare.g_AccountDB = _db;
        LoginSrvForms.MessageBoxHandler = (_, _, _) => LoginSrvForms.IDOK;
        LoginSrvForms.LastMessage = null;
        LoginSrvForms.LastCaption = null;
        TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit();
    }

    public void Dispose()
    {
        LoginSrvForms.MessageBoxHandler = null;
        LoginSrvForms.LastMessage = null;
        TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit();
        LoginSrvShare.ResetForTests();
    }

    private static TAccountInfo Account(string name, string pwd = "p")
    {
        TAccountInfo i = default;
        i.AccountNameStr = name;
        i.PasswordStr = pwd;
        return i;
    }

    [StaFact]
    public void FormCreate_WritesFifteenHeaderCells()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmFindUserId();
            Assert.Equal(2, f.IdGrid.RowCount);
            Assert.Equal("帐号", f.Cells(0, 0));
            Assert.Equal("密码", f.Cells(1, 0));
            Assert.Equal("用户名称", f.Cells(2, 0));
            Assert.Equal("身份证号", f.Cells(3, 0));
            Assert.Equal("生日", f.Cells(4, 0));
            Assert.Equal("问题一", f.Cells(5, 0));
            Assert.Equal("答案一", f.Cells(6, 0));
            Assert.Equal("问题二", f.Cells(7, 0));
            Assert.Equal("答案二", f.Cells(8, 0));
            Assert.Equal("电话", f.Cells(9, 0));
            Assert.Equal("移动电话", f.Cells(10, 0));
            Assert.Equal("备注信息", f.Cells(11, 0));
            // 位置 12 原文注释掉的是 '备注二'，实际写的是 '创建时间'
            Assert.Equal("创建时间", f.Cells(12, 0));
            Assert.Equal("最后登录时间", f.Cells(13, 0));
            Assert.Equal("电子邮箱", f.Cells(14, 0));
            Assert.Equal(15, f.IdGrid.Columns.Count);
        });
    }

    [StaTheory]
    [InlineData('x')]
    [InlineData('\t')]
    public void edtFindAccountKeyPress_NonEnter_DoesNothing(char key)
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice");
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "alice";
            char k = key;
            f.edtFindAccountKeyPress(f, ref k);
            Assert.Equal(2, f.IdGrid.RowCount);   // 未重置为 1
        });
    }

    [StaFact]
    public void edtFindAccountKeyPress_Enter_ResetsToHeaderOnly_ThenAppendsMatch()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice", "pw1");
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "  alice  ";
            char k = '\r';
            f.edtFindAccountKeyPress(f, ref k);

            Assert.Equal(2, f.IdGrid.RowCount);
            Assert.Equal("alice", f.Cells(0, 1));
            Assert.Equal("pw1", f.Cells(1, 1));
        });
    }

    [StaFact]
    public void edtFindAccountKeyPress_Enter_NotFound_LeavesOnlyHeaderRow()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice");
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "bob";
            char k = '\r';
            f.edtFindAccountKeyPress(f, ref k);
            Assert.Equal(1, f.IdGrid.RowCount);
        });
    }

    [StaFact]
    public void btnFindAccountClick_EmptyAccount_IsNoOp()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice");
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "   ";
            f.btnFindAccountClick(f);
            Assert.Equal(2, f.IdGrid.RowCount);   // 未改动
        });
    }

    [StaFact]
    public void btnFindAccountClick_PrefixMatches_AppendsAllRows()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice");
            _db.Store["alan"] = Account("alan");
            _db.Store["bob"] = Account("bob");
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "al";
            f.btnFindAccountClick(f);

            Assert.Equal(3, f.IdGrid.RowCount);   // 表头 + 2
            Assert.Equal("alice", f.Cells(0, 1));
            Assert.Equal("alan", f.Cells(0, 2));
        });
    }

    [StaFact]
    public void btnFindAccountClick_Exception_LogsMainOutMessage()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_AccountDB = new ThrowingAccountDb();
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "al";
            f.btnFindAccountClick(f);

            // TAccountDB.FindAccount 内部吞异常并 MainOutMessage(E.Message)
            Assert.Contains(LoginSrvShare.g_MainMsgList.AsEnumerable(), s => s.Contains("boom", StringComparison.Ordinal));
        });
    }

    private sealed class ThrowingAccountDb : TAccountDB
    {
        public ThrowingAccountDb() : base("") { }
        protected override void DoInit() { }
        protected override void DoFinal() { }
        protected override bool DoGetAccountByQuick(string UID, string ID, ref TAccountInfo i) => false;
        protected override bool DoGetAccountByPhone(string Phone, ref TAccountInfo i) => false;
        protected override bool DoGetAccount(string AccountName, ref TAccountInfo i) => false;
        protected override int DoFindAccount(string AccountName, TAccountList AccountList) => throw new InvalidOperationException("boom");
        protected override bool DoUpdateAccount(TAccountInfo i, TAccountUpdateField f) => false;
        protected override void DoGetAllAccount(GXX.Core.Util.TStringList l) { }
        protected override bool DoEnabledAccounts(GXX.Core.Util.TStringList l, bool e) => false;
        protected override bool DoCheckAccountExists(string AccountName) => false;
        protected override bool DoAddAccount(TAccountInfo i) => false;
        protected override bool DoUnLockAccount(string AccountName) => false;
    }

    [StaFact]
    public void Button1Click_DelegatesToMasSocLoadServerAddr()
    {
        StaRunner.New(() =>
        {
            int calls = 0;
            LoginSrvShare.FrmMasSoc.LoadServerAddr = () => calls++;
            using var f = new TFrmFindUserId();
            f.Button1Click(f);
            Assert.Equal(1, calls);
        });
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public void BtnEditClick_RowNotPositive_IsNoOp(int rowIndex)
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice");
            using var f = new TFrmFindUserId();
            f.SetCells(0, 0, "alice");
            if (rowIndex >= 0) f.IdGrid.CurrentCell = f.IdGrid[0, rowIndex];
            else f.IdGrid.CurrentCell = null;

            f.BtnEditClick(f);

            Assert.Equal("p", _db.Store["alice"].PasswordStr);
        });
    }

    [StaFact]
    public void BtnEditClick_UpdatesAllFieldsAndWritesLog()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice", "old");
            var logged = new System.Collections.Generic.List<string>();
            LoginSrvLog.WriteLogMsg = (ident, info) => logged.Add(ident + ":" + info.PasswordStr);

            TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit
            {
                SimulatedModalResult = LoginSrvForms.mrOk,
                OnBeforeModalCheck = form => form.edtPassword.Text = "new",
            };

            using var f = new TFrmFindUserId();
            f.IdGrid.CurrentCell = f.IdGrid[0, 1];
            f.SetCells(0, 1, "alice");
            f.BtnEditClick(f);

            Assert.Equal("new", _db.Store["alice"].PasswordStr);
            Assert.Equal(new[] { "ch2:new" }, logged);
            LoginSrvLog.WriteLogMsg = (_, _) => { };
        });
    }

    [StaFact]
    public void BtnEditClick_Cancelled_NoUpdate()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice", "old");
            TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit { SimulatedModalResult = LoginSrvForms.mrCancel };

            using var f = new TFrmFindUserId();
            f.IdGrid.CurrentCell = f.IdGrid[0, 1];
            f.SetCells(0, 1, "alice");
            f.BtnEditClick(f);

            Assert.Equal("old", _db.Store["alice"].PasswordStr);
        });
    }

    [StaFact]
    public void Button2Click_NewAccount_AddsAndLogs()
    {
        StaRunner.New(() =>
        {
            var logged = new System.Collections.Generic.List<string>();
            LoginSrvLog.WriteLogMsg = (ident, info) => logged.Add(ident + ":" + info.AccountNameStr);
            TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit
            {
                SimulatedModalResult = LoginSrvForms.mrOk,
                OnBeforeModalCheck = form => form.edtAccountName.Text = "newbie",
            };

            using var f = new TFrmFindUserId();
            f.Button2Click(f);

            Assert.True(_db.Store.ContainsKey("newbie"));
            Assert.Equal(new[] { "ch2:newbie" }, logged);
            Assert.Contains(LoginSrvShare.g_MainMsgList.AsEnumerable(), s => s.Contains("创建帐号成功: newbie", StringComparison.Ordinal));
            LoginSrvLog.WriteLogMsg = (_, _) => { };
        });
    }

    [StaFact]
    public void Button2Click_DuplicateAccount_ShowsMessage()
    {
        StaRunner.New(() =>
        {
            _db.Store["newbie"] = Account("newbie");
            TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit
            {
                SimulatedModalResult = LoginSrvForms.mrOk,
                OnBeforeModalCheck = form => form.edtAccountName.Text = "newbie",
            };

            using var f = new TFrmFindUserId();
            f.Button2Click(f);

            Assert.Equal("帐号名重复", LoginSrvForms.LastMessage);
        });
    }

    /// <summary>InputAccountInfo 返回的名称太短（&lt; MIN_ACCOUNT_LEN）→ 什么都不做。</summary>
    [StaFact]
    public void Button2Click_TooShortName_IsNoOp()
    {
        StaRunner.New(() =>
        {
            TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit
            {
                SimulatedModalResult = LoginSrvForms.mrOk,
                OnBeforeModalCheck = form => form.edtAccountName.Text = "abc",
            };

            using var f = new TFrmFindUserId();
            f.Button2Click(f);

            Assert.Empty(_db.Store);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }

    /// <summary>InputAccountInfo 取消 → 不新增、不提示。</summary>
    [StaFact]
    public void Button2Click_Cancelled_IsNoOp()
    {
        StaRunner.New(() =>
        {
            TFrmFindUserId.CreateUserInfoEdit = () => new TFrmUserInfoEdit { SimulatedModalResult = LoginSrvForms.mrCancel };
            using var f = new TFrmFindUserId();
            f.Button2Click(f);
            Assert.Empty(_db.Store);
        });
    }

    /// <summary>RefChrGrid(0, ...) 也走"追加行"分支（原文 `if nIndex &lt;= 0`）。</summary>
    [StaFact]
    public void RefChrGrid_IndexZero_AppendsRow()
    {
        StaRunner.New(() =>
        {
            _db.Store["alice"] = Account("alice");
            using var f = new TFrmFindUserId();
            char k = '\r';
            _db.Store["alice"] = Account("alice", "pw");
            f.edtFindAccount.Text = "alice";
            f.edtFindAccountKeyPress(f, ref k);
            Assert.Equal(2, f.IdGrid.RowCount);
            Assert.Equal("pw", f.Cells(1, 1));
        });
    }

    [StaFact]
    public void EdtFindAccountKeyPress_NoAccountDb_DoesNotThrow()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_AccountDB = null;
            using var f = new TFrmFindUserId();
            f.edtFindAccount.Text = "alice";
            char k = '\r';
            f.edtFindAccountKeyPress(f, ref k);
            Assert.Equal(1, f.IdGrid.RowCount);
        });
    }
}

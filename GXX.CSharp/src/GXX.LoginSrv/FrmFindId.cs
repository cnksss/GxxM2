using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.LoginSrv;

/// <summary>
/// FrmFindId.pas TFrmFindUserId 1:1（登录帐号管理：查询/编辑/新建）。
/// 源码：FrmFindId.pas:44-202；DFM：FrmFindId.dfm（Caption='登录帐号管理', ClientWidth=756, ClientHeight=255, bsSingle）。
/// TStringGrid → DataGridView（行 0 为表头行，与 Delphi FixedRows=1 对齐）。
/// </summary>
public sealed class TFrmFindUserId : System.Windows.Forms.Form
{
    // DFM: IdGrid TStringGrid Left=0 Top=0 Width=756 Height=217 Align=alClient ColCount=15
    //      DefaultRowHeight=20 FixedCols=0 ColWidths=(64 64 71 91 89 150 141 169 170 83 90 72 76 182 159)
    public System.Windows.Forms.DataGridView IdGrid = null!;
    // DFM: Panel1 TPanel(0,217,756,38) Align=alBottom BevelOuter=bvNone Caption=' '
    public System.Windows.Forms.Panel Panel1 = null!;
    public System.Windows.Forms.Label Label1 = null!;      // '帐号:'
    public System.Windows.Forms.TextBox edtFindAccount = null!;
    public System.Windows.Forms.Button btnFindAccount = null!;   // '搜索(&S)'
    public System.Windows.Forms.Button Button1 = null!;          // '重新加载授权IP列表'
    public System.Windows.Forms.Button BtnEdit = null!;          // '编辑(&E)'
    public System.Windows.Forms.Button Button2 = null!;          // '新建(&N)'

    /// <summary>接缝：Delphi 全局 FrmUserInfoEdit（EditUserInfo.pas）。</summary>
    public static Func<TFrmUserInfoEdit> CreateUserInfoEdit = () => new TFrmUserInfoEdit();

    // FrmFindId.pas:121 resourcestring
    private const string sEditAccount = "ch2";
    // FrmFindId.pas:146 resourcestring
    private const string sAddAccount = "ch2";
    private const string sMakingIDSuccess = "创建帐号成功: %s";

    public TFrmFindUserId()
    {
        InitializeComponent();
        FormCreate(this);
    }

    private void InitializeComponent()
    {
        // DFM: Caption = '登录帐号管理' / bsSingle / [biSystemMenu, biMinimize]
        Text = "登录帐号管理";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;
        // DFM: ClientWidth = 756  ClientHeight = 255
        ClientSize = new System.Drawing.Size(756, 255);

        IdGrid = new System.Windows.Forms.DataGridView
        {
            Left = 0,
            Top = 0,
            Width = 756,
            Height = 217,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            RowTemplate = { Height = 20 },            // DFM: DefaultRowHeight = 20
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None,
        };
        int[] colWidths = { 64, 64, 71, 91, 89, 150, 141, 169, 170, 83, 90, 72, 76, 182, 159 };
        for (int i = 0; i < colWidths.Length; i++)                    // DFM: ColCount = 15
        {
            IdGrid.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Width = colWidths[i],
                SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable,
            });
        }
        // DFM: OnDblClick = BtnEditClick
        IdGrid.CellDoubleClick += (s, e) => BtnEditClick(s);

        Panel1 = new System.Windows.Forms.Panel { Left = 0, Top = 217, Width = 756, Height = 38, TabIndex = 1 };
        // DFM: Caption = ' '（Panel1 上无实际标题栏；BevelOuter = bvNone）
        Label1 = new System.Windows.Forms.Label { Text = "帐号:", Left = 16, Top = 12, Width = 30, Height = 12, AutoSize = false };
        edtFindAccount = new System.Windows.Forms.TextBox { Left = 56, Top = 8, Width = 121, Height = 20, TabIndex = 0 };
        edtFindAccount.KeyPress += (s, e) => { char k = e.KeyChar; edtFindAccountKeyPress(s, ref k); e.KeyChar = k; };
        btnFindAccount = new System.Windows.Forms.Button { Text = "搜索(&S)", Left = 192, Top = 8, Width = 57, Height = 21, TabIndex = 1 };
        btnFindAccount.Click += (s, e) => btnFindAccountClick(s);
        Button1 = new System.Windows.Forms.Button { Text = "重新加载授权IP列表", Left = 576, Top = 10, Width = 145, Height = 21, TabIndex = 4 };
        Button1.Click += (s, e) => Button1Click(s);
        BtnEdit = new System.Windows.Forms.Button { Text = "编辑(&E)", Left = 288, Top = 8, Width = 75, Height = 21, TabIndex = 2 };
        BtnEdit.Click += (s, e) => BtnEditClick(s);
        Button2 = new System.Windows.Forms.Button { Text = "新建(&N)", Left = 376, Top = 8, Width = 75, Height = 21, TabIndex = 3 };
        Button2.Click += (s, e) => Button2Click(s);

        Panel1.Controls.Add(Label1);
        Panel1.Controls.Add(edtFindAccount);
        Panel1.Controls.Add(btnFindAccount);
        Panel1.Controls.Add(Button1);
        Panel1.Controls.Add(BtnEdit);
        Panel1.Controls.Add(Button2);

        Controls.Add(IdGrid);
        Controls.Add(Panel1);
    }

    /// <summary>IdGrid.Cells[c, r] 读取等效。</summary>
    public string Cells(int col, int row) => IdGrid[col, row].Value as string ?? "";

    /// <summary>IdGrid.Cells[c, r] := v 写入等效。</summary>
    public void SetCells(int col, int row, string value) => IdGrid[col, row].Value = value;

    /// <summary>IdGrid.Row 等效（无 CurrentCell 时为 0，与 Delphi TStringGrid 默认 Row=0 一致）。</summary>
    public int GridRow => IdGrid.CurrentCell?.RowIndex ?? 0;

    /// <summary>FrmFindId.pas:60 FormCreate（RowCount := 2 + 15 列表头）。</summary>
    public void FormCreate(object? Sender)
    {
        IdGrid.RowCount = 2;
        SetCells(0, 0, "帐号");
        SetCells(1, 0, "密码");
        SetCells(2, 0, "用户名称");
        SetCells(3, 0, "身份证号");
        SetCells(4, 0, "生日");
        SetCells(5, 0, "问题一");
        SetCells(6, 0, "答案一");
        SetCells(7, 0, "问题二");
        SetCells(8, 0, "答案二");
        SetCells(9, 0, "电话");
        SetCells(10, 0, "移动电话");
        SetCells(11, 0, "备注信息");
        //IdGrid.Cells[12, 0] := '备注二';      // 原文如此（FrmFindId.pas:75 注释）
        SetCells(12, 0, "创建时间");
        SetCells(13, 0, "最后登录时间");
        SetCells(14, 0, "电子邮箱");
    }

    /// <summary>FrmFindId.pas:44 edtFindAccountKeyPress。</summary>
    public void edtFindAccountKeyPress(object? Sender, ref char Key)
    {
        if (Key != '\x0D') return;
        string sAccount = DelphiRTL.Trim(edtFindAccount.Text);
        IdGrid.RowCount = 1;

        TAccountInfo AccountInfo = default;
        if (LoginSrvShare.g_AccountDB != null && LoginSrvShare.g_AccountDB.GetAccount(sAccount, ref AccountInfo))
        {
            RefChrGrid(-1, AccountInfo);
        }
    }

    /// <summary>FrmFindId.pas:81 btnFindAccountClick（异常分支仅记录 MainOutMessage）。</summary>
    public void btnFindAccountClick(object? Sender)
    {
        string sAccount = DelphiRTL.Trim(edtFindAccount.Text);
        if (string.Equals(sAccount, "")) return;

        try
        {
            IdGrid.RowCount = 1;
            TAccountList AccountList = new();
            if (LoginSrvShare.g_AccountDB != null && LoginSrvShare.g_AccountDB.FindAccount(sAccount, AccountList) > 0)
            {
                for (int I = 0; I <= AccountList.Count - 1; I++)
                {
                    TAccountInfo AccountInfo = AccountList[I];
                    RefChrGrid(-1, AccountInfo);
                }
            }
        }
        catch (Exception)
        {
            LoginSrvShare.MainOutMessage("TFrmFindUserId.BtnFindAllClick");
        }
    }

    /// <summary>FrmFindId.pas:111 Button1Click（FrmMasSoc.LoadServerAddr）。</summary>
    public void Button1Click(object? Sender)
    {
        LoginSrvShare.FrmMasSoc.LoadServerAddr();
    }

    /// <summary>FrmFindId.pas:116 BtnEditClick（编辑选中帐号 → UpdateAccount(ufAllField) → WriteLogMsg）。</summary>
    public void BtnEditClick(object? Sender)
    {
        int nRow = GridRow;
        if (nRow <= 0) return;
        string sAccount = Cells(0, nRow);
        if (string.Equals(sAccount, "")) return;

        TAccountInfo AccountInfo = default;
        if (LoginSrvShare.g_AccountDB != null && LoginSrvShare.g_AccountDB.GetAccount(sAccount, ref AccountInfo))
        {
            TFrmUserInfoEdit FrmUserInfoEdit = CreateUserInfoEdit();
            if (FrmUserInfoEdit.InputAccountInfo(false, ref AccountInfo, showModal: false))
            {
                if (LoginSrvShare.g_AccountDB.UpdateAccount(AccountInfo, TAccountUpdateField.ufAllField))
                {
                    LoginSrvLog.WriteLogMsg(sEditAccount, AccountInfo);
                }
            }
        }
    }

    /// <summary>FrmFindId.pas:141 Button2Click（新建帐号）。</summary>
    public void Button2Click(object? Sender)
    {
        TAccountInfo AccountInfo = default;   // 原文 FillChar(AccountInfo, SizeOf, #0)
        string sAccount;
        TFrmUserInfoEdit FrmUserInfoEdit = CreateUserInfoEdit();
        if (FrmUserInfoEdit.InputAccountInfo(true, ref AccountInfo, showModal: false)
            && AccountInfo.AccountNameStr.Length >= Grobal2Const.MIN_ACCOUNT_LEN)
        {
            if (LoginSrvShare.g_AccountDB != null && !LoginSrvShare.g_AccountDB.CheckAccountExists(AccountInfo.AccountNameStr))
            {
                sAccount = AccountInfo.AccountNameStr;
                if (LoginSrvShare.g_AccountDB.AddAccount(AccountInfo))
                {
                    LoginSrvShare.MainOutMessage(DelphiRTL.Format(sMakingIDSuccess, sAccount));
                    LoginSrvLog.WriteLogMsg(sAddAccount, AccountInfo);
                }
            }
            else
            {
                LoginSrvForms.ShowMessage("帐号名重复");
            }
        }
    }

    /// <summary>
    /// FrmFindId.pas:168 RefChrGrid。
    /// ★ 原文 `if nIndex &lt;= 0` 视为"追加行"（含 nIndex = 0）；且整体被 try/except 吞掉异常。
    /// </summary>
    private void RefChrGrid(int nIndex, TAccountInfo AccountInfo)
    {
        try
        {
            int nRow;
            if (nIndex <= 0)
            {
                IdGrid.RowCount = IdGrid.RowCount + 1;
                // IdGrid.FixedRows := 1 —— Delphi 固定行恒为第 0 行；DataGridView 侧 0 行即表头行，无需额外设置
                nRow = IdGrid.RowCount - 1;
            }
            else
            {
                nRow = nIndex;
            }
            SetCells(0, nRow, AccountInfo.AccountNameStr);
            SetCells(1, nRow, AccountInfo.PasswordStr);
            SetCells(2, nRow, AccountInfo.UserNameStr);
            SetCells(3, nRow, AccountInfo.IDCardStr);
            SetCells(4, nRow, AccountInfo.BirthDayStr);
            SetCells(5, nRow, AccountInfo.Questions1Str);
            SetCells(6, nRow, AccountInfo.Answers1Str);
            SetCells(7, nRow, AccountInfo.Questions2Str);
            SetCells(8, nRow, AccountInfo.Answers2Str);
            SetCells(9, nRow, AccountInfo.PhoneStr);
            SetCells(10, nRow, AccountInfo.MobilePhoneStr);
            SetCells(11, nRow, AccountInfo.MemoStr);
            //IdGrid.Cells[12, nRow] := DBRecord.UserEntryAdd.sMemo2;   // 原文如此（FrmFindId.pas:195 注释）
            SetCells(12, nRow, DelphiRTL.IntToStr(AccountInfo.CreateDate));
            SetCells(13, nRow, DelphiRTL.IntToStr(AccountInfo.LoginDate));
            SetCells(14, nRow, AccountInfo.MailStr);
        }
        catch
        {
            // 原文 for 无语句的 except（FrmFindId.pas:199-201）——静默吞掉
        }
    }
}

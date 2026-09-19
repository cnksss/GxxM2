using System;
using GXX.Core.Rtl;

namespace GXX.LoginSrv;

/// <summary>
/// uFrmDataManager.pas TFrmDataManager 1:1（角色数据管理，DBServer 数据管理窗体在 LoginSrv 侧副本）。
/// 源码：uFrmDataManager.pas:79-464；DFM（二进制）：uFrmDataManager.dfm 已解码 → Caption='角色数据管理',
/// ClientWidth=576, ClientHeight=362, bsSingle, Position=poDesktopCenter。
/// TVirtualStringTree → TVirtualStringTreeModel 接缝。
/// </summary>
public sealed class TFrmDataManager : System.Windows.Forms.Form
{
    private TSerarchRoleList FSerarchRoleList = null!;

    // DFM: lblRole TLabel(309,15) Caption='角色：'
    public System.Windows.Forms.Label lblRole = null!;
    // DFM: LabelCount TLabel(73,136)（无 Caption）
    public System.Windows.Forms.Label LabelCount = null!;
    // DFM: lblAccount TLabel(9,15) Caption='帐户：'
    public System.Windows.Forms.Label lblAccount = null!;
    // DFM: BtnCreateChr TButton(9,328,81,25) Caption='创建角色(&C)' Enabled=False
    public System.Windows.Forms.Button BtnCreateChr = null!;
    // DFM: btnDeleteRole TButton(282,328,81,25) Caption='删除角色(&D)' Enabled=False Visible=False
    public System.Windows.Forms.Button btnDeleteRole = null!;
    // DFM: btnSearchRole TButton(495,6,70,25) Caption='按角色名'
    public System.Windows.Forms.Button btnSearchRole = null!;
    // DFM: btnDisableHuman TButton(100,328,81,25) Caption='禁用人物(&D)' Enabled=False
    public System.Windows.Forms.Button btnDisableHuman = null!;
    // DFM: btnEnableHuman TButton(191,328,81,25) Caption='启用人物(&U)' Enabled=False
    public System.Windows.Forms.Button btnEnableHuman = null!;
    // DFM: btnEditData TButton(470,328,97,25) Caption='编辑数据(&E)' Enabled=False
    public System.Windows.Forms.Button btnEditData = null!;
    // DFM: btnSearchAccount TButton(195,6,70,25) Caption='按帐户名'
    public System.Windows.Forms.Button btnSearchAccount = null!;
    // DFM: edtRole TEdit(341,11,100,20)
    public System.Windows.Forms.TextBox edtRole = null!;
    // DFM: vstRole TVirtualStringTree(9,40,558,283) DefaultNodeHeight=22
    //      Columns: 启用(40) 登录帐户(120) 角色名称(98) 是否英雄(60) 人物删除(60) 性别 职业 等级(80)
    public TVirtualStringTreeModel vstRole = null!;
    // DFM: chkRoleFuzzy TCheckBox(448,12,47,17) Caption='模糊'
    public System.Windows.Forms.CheckBox chkRoleFuzzy = null!;
    // DFM: edtAccount TEdit(41,11,100,20)
    public System.Windows.Forms.TextBox edtAccount = null!;
    // DFM: chkAccountFuzzy TCheckBox(148,12,47,17) Caption='模糊'
    public System.Windows.Forms.CheckBox chkAccountFuzzy = null!;

    /// <summary>DFM: vstRole 列宽（Positions 0..7）。</summary>
    public static readonly string[] RoleColumnTitles = { "启用", "登录帐户", "角色名称", "是否英雄", "人物删除", "性别", "职业", "等级" };
    public static readonly int[] RoleColumnWidths = { 40, 120, 98, 60, 60, 0, 0, 80 };

    public TFrmDataManager()
    {
        InitializeComponent();
        FormCreate(this);
    }

    private void InitializeComponent()
    {
        // DFM: Caption = '角色数据管理' / bsSingle / [biSystemMenu, biMinimize] / Position = poDesktopCenter
        Text = "角色数据管理";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        // DFM: ClientWidth = 576  ClientHeight = 362
        ClientSize = new System.Drawing.Size(576, 362);

        lblRole = new System.Windows.Forms.Label { Text = "角色：", Left = 309, Top = 15, Width = 36, Height = 12, AutoSize = false };
        LabelCount = new System.Windows.Forms.Label { Text = "", Left = 73, Top = 136, Width = 6, Height = 12, AutoSize = false };
        lblAccount = new System.Windows.Forms.Label { Text = "帐户：", Left = 9, Top = 15, Width = 36, Height = 12, AutoSize = false };

        BtnCreateChr = new System.Windows.Forms.Button { Text = "创建角色(&C)", Left = 9, Top = 328, Width = 81, Height = 25, TabIndex = 7, Enabled = false };
        BtnCreateChr.Click += (s, e) => BtnCreateChrClick(s);
        btnDeleteRole = new System.Windows.Forms.Button { Text = "删除角色(&D)", Left = 282, Top = 328, Width = 81, Height = 25, TabIndex = 8, Enabled = false, Visible = false };
        btnDeleteRole.Click += (s, e) => btnDeleteRoleClick(s);
        btnSearchRole = new System.Windows.Forms.Button { Text = "按角色名", Left = 495, Top = 6, Width = 70, Height = 25, TabIndex = 5 };
        btnSearchRole.Click += (s, e) => btnSearchRoleClick(s);
        btnDisableHuman = new System.Windows.Forms.Button { Text = "禁用人物(&D)", Left = 100, Top = 328, Width = 81, Height = 25, TabIndex = 9, Enabled = false };
        btnDisableHuman.Click += (s, e) => btnDisableHumanClick(s);
        btnEnableHuman = new System.Windows.Forms.Button { Text = "启用人物(&U)", Left = 191, Top = 328, Width = 81, Height = 25, TabIndex = 10, Enabled = false };
        btnEnableHuman.Click += (s, e) => btnEnableHumanClick(s);
        btnEditData = new System.Windows.Forms.Button { Text = "编辑数据(&E)", Left = 470, Top = 328, Width = 97, Height = 25, TabIndex = 11, Enabled = false };
        btnEditData.Click += (s, e) => btnEditDataClick(s);
        btnSearchAccount = new System.Windows.Forms.Button { Text = "按帐户名", Left = 195, Top = 6, Width = 70, Height = 25, TabIndex = 2 };
        btnSearchAccount.Click += (s, e) => btnSearchAccountClick(s);

        edtRole = new System.Windows.Forms.TextBox { Left = 341, Top = 11, Width = 100, Height = 20, TabIndex = 3 };
        edtRole.KeyPress += (s, e) => { char k = e.KeyChar; edtRoleKeyPress(s, ref k); e.KeyChar = k; };
        edtAccount = new System.Windows.Forms.TextBox { Left = 41, Top = 11, Width = 100, Height = 20, TabIndex = 0 };
        edtAccount.KeyPress += (s, e) => { char k = e.KeyChar; edtAccountKeyPress(s, ref k); e.KeyChar = k; };

        vstRole = new TVirtualStringTreeModel { NodeDataSize = 0 };
        chkRoleFuzzy = new System.Windows.Forms.CheckBox { Text = "模糊", Left = 448, Top = 12, Width = 47, Height = 17, TabIndex = 4, AutoSize = false };
        chkAccountFuzzy = new System.Windows.Forms.CheckBox { Text = "模糊", Left = 148, Top = 12, Width = 47, Height = 17, TabIndex = 1, AutoSize = false };

        Controls.Add(lblRole);
        Controls.Add(LabelCount);
        Controls.Add(lblAccount);
        Controls.Add(BtnCreateChr);
        Controls.Add(btnDeleteRole);
        Controls.Add(btnSearchRole);
        Controls.Add(btnDisableHuman);
        Controls.Add(btnEnableHuman);
        Controls.Add(btnEditData);
        Controls.Add(btnSearchAccount);
        Controls.Add(edtRole);
        Controls.Add(edtAccount);
        Controls.Add(chkRoleFuzzy);
        Controls.Add(chkAccountFuzzy);
    }

    /// <summary>uFrmDataManager.pas:67 ShowFrmDataManager（全局过程）。</summary>
    public static void ShowFrmDataManager(bool showModal = true)
    {
        var FrmDataManager = new TFrmDataManager();
        try
        {
            if (showModal) FrmDataManager.ShowDialog();
        }
        finally
        {
            FrmDataManager.Dispose();
        }
    }

    /// <summary>uFrmDataManager.pas:79 FormCreate。</summary>
    public void FormCreate(object? Sender)
    {
        FSerarchRoleList = new TSerarchRoleList();
    }

    /// <summary>uFrmDataManager.pas:84 FormDestroy。</summary>
    public void FormDestroy(object? Sender)
    {
        FSerarchRoleList = null!;
    }

    /// <summary>uFrmDataManager.pas:89 ShowAskMsg（= mrYes 才为 True）。</summary>
    public bool ShowAskMsg(string S)
    {
        return LoginSrvForms.MessageBox(S, "询问", LoginSrvForms.MB_YESNO | LoginSrvForms.MB_ICONQUESTION) == LoginSrvForms.mrYes;
    }

    /// <summary>uFrmDataManager.pas:94 ShowInfoMsg。</summary>
    public void ShowInfoMsg(string S)
    {
        LoginSrvForms.MessageBox(S, "提示", LoginSrvForms.MB_OK | LoginSrvForms.MB_ICONINFORMATION);
    }

    //  { uFrmDataManager.pas:99-104 ShowErrorMsg —— 原文整体注释，保留注释 }

    /// <summary>uFrmDataManager.pas:106 edtAccountKeyPress（★ Key 先置 #0 再转交按钮）。</summary>
    public void edtAccountKeyPress(object? Sender, ref char Key)
    {
        if (Key == '\x0D')
        {
            Key = '\x00';
            btnSearchAccountClick(btnSearchAccount);
        }
    }

    /// <summary>uFrmDataManager.pas:128 edtRoleKeyPress。</summary>
    public void edtRoleKeyPress(object? Sender, ref char Key)
    {
        if (Key == '\x0D')
        {
            Key = '\x00';
            btnSearchRoleClick(btnSearchRole);
        }
    }

    /// <summary>uFrmDataManager.pas:115 RefreshRoleList。</summary>
    public void RefreshRoleList()
    {
        vstRole.Clear();
        for (int I = 0; I <= FSerarchRoleList.Count - 1; I++)
        {
            TSerarchRoleData RoleData = FSerarchRoleList.Items[I];
            vstRole.AddChild(null).Data = RoleData;
        }
    }

    /// <summary>uFrmDataManager.pas:137 BtnCreateChrClick（★ 原文方法体整体被 (* *) 注释 → 空实现）。</summary>
    public void BtnCreateChrClick(object? Sender)
    {
        // (* ... *) 原文如此（uFrmDataManager.pas:139-176）：方法体整段被注释
    }

    /// <summary>uFrmDataManager.pas:179 btnEditDataClick。</summary>
    public void btnEditDataClick(object? Sender)
    {
        if (vstRole.FocusedNode == null) return;
        TSerarchRoleData? RoleData = vstRole.FocusedNode.Data as TSerarchRoleData;
        if (RoleData == null) return;
        if (!RoleData.IsHero)
        {
            THumData? HumData = new THumData();
            int HumanID;
            if (LoginSrvRoleDb.g_RoleDB != null
                && LoginSrvRoleDb.g_RoleDB.HumanDB.Get(RoleData.Account, RoleData.RoleName, ref HumData, out HumanID))
            {
                LoginSrvRoleDb.ShowFrmRoleDataEdit(HumanID, HumData, null);
            }
        }
        else
        {
            THeroData? HeroData = new THeroData();
            int HeroID;
            if (LoginSrvRoleDb.g_RoleDB != null
                && LoginSrvRoleDb.g_RoleDB.HeroDB.Get(RoleData.RoleName, ref HeroData, out HeroID))
            {
                LoginSrvRoleDb.ShowFrmRoleDataEdit(HeroID, null, HeroData);
            }
        }
    }

    /// <summary>uFrmDataManager.pas:211 btnSearchAccountClick。</summary>
    public void btnSearchAccountClick(object? Sender)
    {
        string Account = edtAccount.Text;
        if (Account.Length > 0)
        {
            FSerarchRoleList.Clear();

            btnDisableHuman.Enabled = false;
            btnEnableHuman.Enabled = false;
            btnEditData.Enabled = false;

            TSerarchRoleList TempRoleList = new();
            if (chkAccountFuzzy.Checked)
            {
                LoginSrvRoleDb.g_RoleDB?.HumanDB.SearchByAccount(Account, TSearchMatchType.smtFuzzy, FSerarchRoleList);
                LoginSrvRoleDb.g_RoleDB?.HeroDB.SearchByAccount(Account, TSearchMatchType.smtFuzzy, TempRoleList);
            }
            else
            {
                LoginSrvRoleDb.g_RoleDB?.HumanDB.SearchByAccount(Account, TSearchMatchType.smtComplete, FSerarchRoleList);
                LoginSrvRoleDb.g_RoleDB?.HeroDB.SearchByAccount(Account, TSearchMatchType.smtComplete, TempRoleList);
            }

            for (int I = 0; I <= TempRoleList.Count - 1; I++)
            {
                TSerarchRoleData RoleData = TempRoleList.Items[I];
                FSerarchRoleList.Add(RoleData);
            }

            RefreshRoleList();
        }
    }

    /// <summary>uFrmDataManager.pas:253 btnSearchRoleClick。</summary>
    public void btnSearchRoleClick(object? Sender)
    {
        string Role = edtRole.Text;
        if (Role.Length > 0)
        {
            FSerarchRoleList.Clear();

            TSerarchRoleList TempRoleList = new();
            if (chkRoleFuzzy.Checked)
            {
                LoginSrvRoleDb.g_RoleDB?.HumanDB.SearchByName(Role, TSearchMatchType.smtFuzzy, FSerarchRoleList);
                LoginSrvRoleDb.g_RoleDB?.HeroDB.SearchByName(Role, TSearchMatchType.smtFuzzy, TempRoleList);
            }
            else
            {
                LoginSrvRoleDb.g_RoleDB?.HumanDB.SearchByName(Role, TSearchMatchType.smtComplete, FSerarchRoleList);
                LoginSrvRoleDb.g_RoleDB?.HeroDB.SearchByName(Role, TSearchMatchType.smtComplete, TempRoleList);
            }

            for (int I = 0; I <= TempRoleList.Count - 1; I++)
            {
                TSerarchRoleData RoleData = TempRoleList.Items[I];
                FSerarchRoleList.Add(RoleData);
            }

            RefreshRoleList();
        }
    }

    /// <summary>uFrmDataManager.pas:291 GetJobName（0/1/2 → 战士/法师/道士，否则 '-'）。</summary>
    public static string GetJobName(int nJob)
    {
        string Result;
        switch (nJob)
        {
            case 0: Result = "战士"; break;
            case 1: Result = "法师"; break;
            case 2: Result = "道士"; break;
            default: Result = "-"; break;
        }
        return Result;
    }

    /// <summary>uFrmDataManager.pas:302 vstRoleGetText。</summary>
    public void vstRoleGetText(TVirtualStringTreeModel Sender, TVirtualNode Node, int Column, ref string CellText)
    {
        // SEX_NAME: array[Boolean] of string = ('男', '女')
        string[] SEX_NAME = { "男", "女" };
        // BOOL_NAME: array[Boolean] of string = ('', '√')
        string[] BOOL_NAME = { "", "√" };

        TSerarchRoleData? RoleData = Sender is null ? null : (Node?.Data as TSerarchRoleData);
        if (RoleData == null) return;
        switch (Column)
        {
            case 0: CellText = BOOL_NAME[(RoleData.IsDelete & 2) == 0 ? 1 : 0]; break;
            case 1: CellText = RoleData.Account; break;
            case 2: CellText = RoleData.RoleName; break;
            case 3: CellText = BOOL_NAME[RoleData.IsHero ? 1 : 0]; break;
            case 4: CellText = BOOL_NAME[(RoleData.IsDelete & 1) != 0 ? 1 : 0]; break;
            case 5: CellText = SEX_NAME[RoleData.Sex == 1 ? 1 : 0]; break;
            case 6: CellText = GetJobName(RoleData.Job); break;
            case 7: CellText = DelphiRTL.IntToStr(RoleData.Level); break;
        }
    }

    /// <summary>uFrmDataManager.pas:327 vstRoleFocusChanged。</summary>
    public void vstRoleFocusChanged(TVirtualStringTreeModel Sender, TVirtualNode? Node, int Column)
    {
        if (Node == null)
        {
            btnDisableHuman.Enabled = false;
            btnEnableHuman.Enabled = false;
            btnEditData.Enabled = false;
            return;
        }

        TSerarchRoleData? RoleData = Node.Data as TSerarchRoleData;
        if (RoleData == null) return;
        btnDisableHuman.Enabled = (!RoleData.IsHero) && ((RoleData.IsDelete & 2) == 0);
        btnEnableHuman.Enabled = (!RoleData.IsHero) && ((RoleData.IsDelete & 2) != 0);
        btnEditData.Enabled = true;
    }

    /// <summary>uFrmDataManager.pas:349 btnDisableHumanClick。</summary>
    public void btnDisableHumanClick(object? Sender)
    {
        if (vstRole.FocusedNode == null) return;

        TSerarchRoleData? RoleData = vstRole.FocusedNode.Data as TSerarchRoleData;
        if (RoleData == null) return;

        if ((RoleData.IsHero) || ((RoleData.IsDelete & 2) != 0)) return;

        if (ShowAskMsg("被禁用人物在登录器中无法恢复" + "\r\n" + "\r\n" + "是否禁用人物 \"" + RoleData.RoleName + "\" ?"))
        {
            if (LoginSrvRoleDb.g_RoleDB != null
                && LoginSrvRoleDb.g_RoleDB.HumanDB.SetEnabled(RoleData.Account, RoleData.RoleName, RoleData.IsDelete | 2))
            {
                RoleData.IsDelete = RoleData.IsDelete | 2;
                vstRole.InvalidateNode(vstRole.FocusedNode);
                btnDisableHuman.Enabled = false;
                btnEnableHuman.Enabled = true;
            }
        }
    }

    /// <summary>uFrmDataManager.pas:374 btnEnableHumanClick。</summary>
    public void btnEnableHumanClick(object? Sender)
    {
        if (vstRole.FocusedNode == null) return;

        TSerarchRoleData? RoleData = vstRole.FocusedNode.Data as TSerarchRoleData;
        if (RoleData == null) return;

        if ((RoleData.IsHero) || ((RoleData.IsDelete & 2) == 0)) return;

        if (ShowAskMsg("是否启用人物 \"" + RoleData.RoleName + "\" ?"))
        {
            if (LoginSrvRoleDb.g_RoleDB != null
                && LoginSrvRoleDb.g_RoleDB.HumanDB.SetEnabled(RoleData.Account, RoleData.RoleName, RoleData.IsDelete & ~2))
            {
                RoleData.IsDelete = RoleData.IsDelete & ~2;
                vstRole.InvalidateNode(vstRole.FocusedNode);
                btnDisableHuman.Enabled = true;
                btnEnableHuman.Enabled = false;
            }
        }
    }

    /// <summary>uFrmDataManager.pas:399 btnDeleteRoleClick。</summary>
    public void btnDeleteRoleClick(object? Sender)
    {
        // RoleTypeNames: array[Boolean] of string = ('人物', '英雄')
        string[] RoleTypeNames = { "人物", "英雄" };

        if (vstRole.FocusedNode == null) return;

        TSerarchRoleData? RoleData = vstRole.FocusedNode.Data as TSerarchRoleData;
        if (RoleData == null) return;

        if (ShowAskMsg("执行删除操作将会删除角色对应的数据并且不可恢复！" + "\r\n" + "\r\n" +
            "你确定要删除" + RoleTypeNames[RoleData.IsHero ? 1 : 0] + " \"" + RoleData.RoleName + "\" 吗?"))
        {
            bool IsOK;
            if (!RoleData.IsHero)
            {
                IsOK = LoginSrvRoleDb.g_RoleDB != null && LoginSrvRoleDb.g_RoleDB.HumanDB.Erase(RoleData.Account, RoleData.RoleName);
            }
            else
            {
                IsOK = LoginSrvRoleDb.g_RoleDB != null && LoginSrvRoleDb.g_RoleDB.HeroDB.Erase(RoleData.RoleName);
            }

            if (IsOK)
            {
                vstRole.DeleteNode(vstRole.FocusedNode);
            }
        }
    }

    /// <summary>uFrmDataManager.pas:433 vstRoleNodeDblClick（与 btnEditDataClick 同体）。</summary>
    public void vstRoleNodeDblClick(TVirtualStringTreeModel Sender, TVirtualNode? HitNode)
    {
        if (HitNode == null) return;
        TSerarchRoleData? RoleData = HitNode.Data as TSerarchRoleData;
        if (RoleData == null) return;
        if (!RoleData.IsHero)
        {
            THumData? HumData = new THumData();
            int HumanID;
            if (LoginSrvRoleDb.g_RoleDB != null
                && LoginSrvRoleDb.g_RoleDB.HumanDB.Get(RoleData.Account, RoleData.RoleName, ref HumData, out HumanID))
            {
                LoginSrvRoleDb.ShowFrmRoleDataEdit(HumanID, HumData, null);
            }
        }
        else
        {
            THeroData? HeroData = new THeroData();
            int HeroID;
            if (LoginSrvRoleDb.g_RoleDB != null
                && LoginSrvRoleDb.g_RoleDB.HeroDB.Get(RoleData.RoleName, ref HeroData, out HeroID))
            {
                LoginSrvRoleDb.ShowFrmRoleDataEdit(HeroID, null, HeroData);
            }
        }
    }
}

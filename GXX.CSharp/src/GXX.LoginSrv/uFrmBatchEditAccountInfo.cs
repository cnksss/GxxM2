using System;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.LoginSrv;

/// <summary>uFrmBatchEditAccountInfo.pas TNodeData（原文为 string[14]，赋值即截断）。</summary>
public sealed class TBatchEditNodeData
{
    private string _accountName = "";

    /// <summary>AccountName: string[14] —— 短字符串赋值截断（Delphi 语义）。</summary>
    public string AccountName
    {
        get => _accountName;
        set => _accountName = value != null && value.Length > 14 ? value.Substring(0, 14) : (value ?? "");
    }
}

/// <summary>
/// uFrmBatchEditAccountInfo.pas TFrmBatchEditAccountInfo 1:1（帐号批量启用/禁用）。
/// 源码：uFrmBatchEditAccountInfo.pas:56-327；DFM：uFrmBatchEditAccountInfo.dfm。
/// TVirtualStringTree → TVirtualStringTreeModel 接缝；TOpenDialog → OpenFileDialog + 注入提供者。
/// </summary>
public sealed class TFrmBatchEditAccountInfo : System.Windows.Forms.Form
{
    public System.Windows.Forms.Panel pnlLeft = null!;
    public TVirtualStringTreeModel vstEnableAccount = null!;
    public System.Windows.Forms.Panel Panel1 = null!;
    public System.Windows.Forms.Panel pnlRightTitle = null!;
    public System.Windows.Forms.Button btnSetEnabled = null!;
    public System.Windows.Forms.TextBox edtDisableAccount = null!;
    public System.Windows.Forms.Panel pnlLeftTitle = null!;
    public System.Windows.Forms.Button btnSetDisable = null!;
    public System.Windows.Forms.Button btn2 = null!;
    public System.Windows.Forms.TextBox edtEnabledAccount = null!;
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.Label Label1 = null!;
    public TVirtualStringTreeModel vstDisableAccount = null!;
    public System.Windows.Forms.OpenFileDialog dlgOpen = null!;

    /// <summary>测试注入：非 null 时替代 dlgOpen.Execute；返回 null 表示取消。</summary>
    public static Func<string?>? OpenFileProvider;

    public TFrmBatchEditAccountInfo()
    {
        InitializeComponent();
        FormCreate(this);
    }

    private void InitializeComponent()
    {
        // DFM: Caption = '批量编辑帐户信息'（uFrmBatchEditAccountInfo.dfm）
        Text = "批量编辑帐户信息";
        ClientSize = new System.Drawing.Size(640, 420);

        pnlLeft = new System.Windows.Forms.Panel { Left = 0, Top = 0, Width = 320, Height = 420, TabIndex = 0 };
        pnlLeftTitle = new System.Windows.Forms.Panel { Left = 0, Top = 0, Width = 320, Height = 24, TabIndex = 0 };
        lbl1 = new System.Windows.Forms.Label { Text = "可用帐户", Left = 6, Top = 5, Width = 80, Height = 12, AutoSize = false };
        pnlLeftTitle.Controls.Add(lbl1);
        edtEnabledAccount = new System.Windows.Forms.TextBox { Left = 90, Top = 2, Width = 150, Height = 20, TabIndex = 1 };
        edtEnabledAccount.TextChanged += (s, e) => edtEnabledAccountChange(s);
        pnlLeftTitle.Controls.Add(edtEnabledAccount);
        btnSetDisable = new System.Windows.Forms.Button { Text = "禁用选中(&D)", Left = 246, Top = 0, Width = 74, Height = 22, TabIndex = 2 };
        btnSetDisable.Click += (s, e) => btnSetDisableClick(s);
        pnlLeftTitle.Controls.Add(btnSetDisable);
        vstEnableAccount = new TVirtualStringTreeModel();
        btn2 = new System.Windows.Forms.Button { Text = "导入列表(&I)", Left = 0, Top = 0, Width = 90, Height = 22, TabIndex = 3 };
        btn2.Click += (s, e) => btn2Click(s);

        Panel1 = new System.Windows.Forms.Panel { Left = 320, Top = 0, Width = 320, Height = 420, TabIndex = 1 };
        pnlRightTitle = new System.Windows.Forms.Panel { Left = 0, Top = 0, Width = 320, Height = 24, TabIndex = 0 };
        Label1 = new System.Windows.Forms.Label { Text = "已禁用帐户", Left = 6, Top = 5, Width = 80, Height = 12, AutoSize = false };
        pnlRightTitle.Controls.Add(Label1);
        edtDisableAccount = new System.Windows.Forms.TextBox { Left = 90, Top = 2, Width = 150, Height = 20, TabIndex = 1 };
        edtDisableAccount.TextChanged += (s, e) => edtDisableAccountChange(s);
        pnlRightTitle.Controls.Add(edtDisableAccount);
        btnSetEnabled = new System.Windows.Forms.Button { Text = "解禁选中(&E)", Left = 246, Top = 0, Width = 74, Height = 22, TabIndex = 2 };
        btnSetEnabled.Click += (s, e) => btnSetEnabledClick(s);
        pnlRightTitle.Controls.Add(btnSetEnabled);
        vstDisableAccount = new TVirtualStringTreeModel();

        dlgOpen = new System.Windows.Forms.OpenFileDialog { Filter = "文本文件(*.txt)|*.txt", Title = "导入帐号列表" };

        Controls.Add(pnlLeft);
        Controls.Add(Panel1);
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:56 ShowFrmBatchEditAccountInfo（全局过程）。</summary>
    public static void ShowFrmBatchEditAccountInfo(bool showModal = true)
    {
        var FrmBatchEditAccountInfo = new TFrmBatchEditAccountInfo();
        try
        {
            if (showModal) FrmBatchEditAccountInfo.ShowDialog();
        }
        finally
        {
            FrmBatchEditAccountInfo.Dispose();
        }
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:68 FormCreate（g_AccountDB.GetAllAccount 分流两棵树）。</summary>
    public void FormCreate(object? Sender)
    {
        vstEnableAccount.NodeDataSize = 1;    // 原文 SizeOf(TNodeData)（string[14] 短串 = 15 字节）
        vstDisableAccount.NodeDataSize = 1;

        TStringList AccountList = new();
        if (LoginSrvShare.g_AccountDB != null)
            LoginSrvShare.g_AccountDB.GetAllAccount(AccountList);

        for (int I = 0; I <= AccountList.Count - 1; I++)
        {
            // 原文 Integer(AccountList.Objects[I]) = 0 → 启用列表，否则禁用列表
            if (Convert.ToInt32(AccountList.GetObject(I) ?? 0) == 0)
            {
                TVirtualNode Node = vstEnableAccount.AddChild(null);
                Node.CheckType = TCheckType.ctCheckBox;
                Node.CheckState = TCheckState.csUncheckedNormal;
                ((TBatchEditNodeData)EnsureData(Node)).AccountName = AccountList[I];
            }
            else
            {
                TVirtualNode Node = vstDisableAccount.AddChild(null);
                Node.CheckType = TCheckType.ctCheckBox;
                Node.CheckState = TCheckState.csUncheckedNormal;
                ((TBatchEditNodeData)EnsureData(Node)).AccountName = AccountList[I];
            }
        }
    }

    private static object EnsureData(TVirtualNode node) => node.Data ??= new TBatchEditNodeData();

    /// <summary>uFrmBatchEditAccountInfo.pas:108 vstEnableAccountGetText。</summary>
    public void vstEnableAccountGetText(TVirtualStringTreeModel Sender, TVirtualNode Node, int Column, ref string CellText)
    {
        TBatchEditNodeData? NodeData = Node?.Data as TBatchEditNodeData;
        if (NodeData == null) return;
        CellText = NodeData.AccountName;
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:118 vstEnableAccountBeforeItemErase（奇数行斑马色 $00FFFBF7）。</summary>
    public void vstEnableAccountBeforeItemErase(TVirtualStringTreeModel Sender, TVirtualNode Node,
        ref int ItemColor, ref TItemEraseAction EraseAction)
    {
        if (Node.Index % 2 != 0)
        {
            ItemColor = 0x00FFFBF7;
            EraseAction = TItemEraseAction.eaColor;
        }
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:130 btnSetDisableClick。</summary>
    public void btnSetDisableClick(object? Sender)
    {
        TStringList SL = new();
        TVirtualNode? Node = vstEnableAccount.GetFirstChecked();
        while (Node != null)
        {
            TBatchEditNodeData? NodeData = Node.Data as TBatchEditNodeData;
            SL.AddObject(NodeData?.AccountName ?? "", Node);
            Node = vstEnableAccount.GetNextChecked(Node);
        }

        if (SL.Count > 0)
        {
            if (LoginSrvShare.g_AccountDB != null && LoginSrvShare.g_AccountDB.EnabledAccounts(SL, false))
            {
                for (int I = 0; I <= SL.Count - 1; I++)
                {
                    TVirtualNode NewNode = vstDisableAccount.AddChild(null);
                    NewNode.CheckType = TCheckType.ctCheckBox;
                    NewNode.CheckState = TCheckState.csUncheckedNormal;
                    ((TBatchEditNodeData)EnsureData(NewNode)).AccountName = SL[I];

                    vstEnableAccount.DeleteNode(SL.GetObject(I) as TVirtualNode);
                }

                LoginSrvForms.ShowMessage("已成功禁用" + DelphiRTL.IntToStr(SL.Count) + "个帐户");
            }
        }
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:171 btnSetEnabledClick。</summary>
    public void btnSetEnabledClick(object? Sender)
    {
        TStringList SL = new();
        TVirtualNode? Node = vstDisableAccount.GetFirstChecked();
        while (Node != null)
        {
            TBatchEditNodeData? NodeData = Node.Data as TBatchEditNodeData;
            SL.AddObject(NodeData?.AccountName ?? "", Node);
            Node = vstDisableAccount.GetNextChecked(Node);
        }

        if (SL.Count > 0)
        {
            if (LoginSrvShare.g_AccountDB != null && LoginSrvShare.g_AccountDB.EnabledAccounts(SL, true))
            {
                for (int I = 0; I <= SL.Count - 1; I++)
                {
                    TVirtualNode NewNode = vstEnableAccount.AddChild(null);
                    NewNode.CheckType = TCheckType.ctCheckBox;
                    NewNode.CheckState = TCheckState.csUncheckedNormal;
                    ((TBatchEditNodeData)EnsureData(NewNode)).AccountName = SL[I];

                    vstDisableAccount.DeleteNode(SL.GetObject(I) as TVirtualNode);
                }

                LoginSrvForms.ShowMessage("已成功解禁" + DelphiRTL.IntToStr(SL.Count) + "个帐户");
            }
        }
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:212 edtEnabledAccountChange（空串→全可见）。</summary>
    public void edtEnabledAccountChange(object? Sender)
    {
        if (edtEnabledAccount.Text.Length == 0)
        {
            vstEnableAccount.BeginUpdate();
            try
            {
                TVirtualNode? Node = vstEnableAccount.GetFirst();
                while (Node != null)
                {
                    vstEnableAccount.SetVisible(Node, true);
                    Node = vstEnableAccount.GetNext(Node);
                }
            }
            finally
            {
                vstEnableAccount.EndUpdate();
            }
        }
        else
        {
            vstEnableAccount.BeginUpdate();
            try
            {
                TVirtualNode? Node = vstEnableAccount.GetFirst();
                while (Node != null)
                {
                    TBatchEditNodeData? NodeData = Node.Data as TBatchEditNodeData;
                    vstEnableAccount.SetVisible(Node, DelphiRTL.Pos(edtEnabledAccount.Text, NodeData?.AccountName ?? "") > 0);
                    Node = vstEnableAccount.GetNext(Node);
                }
            }
            finally
            {
                vstEnableAccount.EndUpdate();
            }
        }
    }

    /// <summary>uFrmBatchEditAccountInfo.pas:249 edtDisableAccountChange。</summary>
    public void edtDisableAccountChange(object? Sender)
    {
        if (edtDisableAccount.Text.Length == 0)
        {
            vstDisableAccount.BeginUpdate();
            try
            {
                TVirtualNode? Node = vstDisableAccount.GetFirst();
                while (Node != null)
                {
                    vstDisableAccount.SetVisible(Node, true);
                    Node = vstDisableAccount.GetNext(Node);
                }
            }
            finally
            {
                vstDisableAccount.EndUpdate();
            }
        }
        else
        {
            vstDisableAccount.BeginUpdate();
            try
            {
                TVirtualNode? Node = vstDisableAccount.GetFirst();
                while (Node != null)
                {
                    TBatchEditNodeData? NodeData = Node.Data as TBatchEditNodeData;
                    vstDisableAccount.SetVisible(Node, DelphiRTL.Pos(edtDisableAccount.Text, NodeData?.AccountName ?? "") > 0);
                    Node = vstDisableAccount.GetNext(Node);
                }
            }
            finally
            {
                vstDisableAccount.EndUpdate();
            }
        }
    }

    /// <summary>
    /// uFrmBatchEditAccountInfo.pas:286 btn2Click。
    /// ★ 两处原文怪癖保留：(a) 循环里写的是 `vstDisableAccount.IsVisible[Node]`，Node 却来自 vstEnableAccount；
    /// (b) `SL.IndexOf(...) > 0`（首行永不被选中，应为 &gt;= 0）。
    /// </summary>
    public void btn2Click(object? Sender)
    {
        string? FileName = OpenFileProvider != null
            ? OpenFileProvider()
            : (dlgOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dlgOpen.FileName : null);
        if (FileName != null)
        {
            TStringList SL = new();
            SL.LoadFromFile(FileName);

            int Count = 0;

            TVirtualNode? Node = vstEnableAccount.GetFirst();
            while (Node != null)
            {
                TBatchEditNodeData? NodeData = Node.Data as TBatchEditNodeData;

                vstDisableAccount.SetVisible(Node, true);   // 原文如此：写的是 vstDisableAccount

                if (SL.IndexOf(NodeData?.AccountName ?? "") > 0)
                {
                    Node.CheckState = TCheckState.csCheckedNormal;
                    Count++;
                }
                else
                {
                    Node.CheckState = TCheckState.csUnCheckedNormal;   // 原文如此（UnChecked 拼写）
                }

                Node = vstEnableAccount.GetNext(Node);
            }

            vstEnableAccount.Invalidate();
            LoginSrvForms.ShowMessage("已成功导入并选中" + DelphiRTL.IntToStr(Count) + "个帐户");
        }
    }
}

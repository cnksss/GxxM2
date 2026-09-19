using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewSession.pas TfrmViewSession 1:1 转换（登录会话查看：帐号/地址/会话ID/充值）。
/// TStringGrid → DataGridView。
/// </summary>
public sealed class ViewSessionForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.DataGridView GridSession = null!;
    public System.Windows.Forms.Button ButtonRefGrid = null!;
    public System.Windows.Forms.Panel PanelStatus = null!;

    public ViewSessionForm()
    {
        InitializeComponent();
        FormCreate();
    }

    private void InitializeComponent()
    {
        Text = "查看会话";
        Width = 560;
        Height = 420;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        GridSession = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 8,
            Width = 530,
            Height = 320,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        };
        GridSession.Columns.Add("c0", "序号");
        GridSession.Columns.Add("c1", "登录帐号");
        GridSession.Columns.Add("c2", "登录地址");
        GridSession.Columns.Add("c3", "会话ID号");
        GridSession.Columns.Add("c4", "充值");
        GridSession.Columns.Add("c5", "充值模式");
        Controls.Add(GridSession);

        ButtonRefGrid = new System.Windows.Forms.Button { Text = "刷新(&R)", Left = 8, Top = 336, Width = 90, Height = 26 };
        ButtonRefGrid.Click += (s, e) => ButtonRefGridClick(s);
        Controls.Add(ButtonRefGrid);

        PanelStatus = new System.Windows.Forms.Panel { Left = 108, Top = 336, Width = 430, Height = 26, BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D };
        Controls.Add(PanelStatus);
    }

    /// <summary>Delphi FormCreate（表头行 Cells[x,0]）。</summary>
    public void FormCreate()
    {
        GridSession.Rows.Add(1);
        GridSession[0, 0].Value = "序号";
        GridSession[1, 0].Value = "登录帐号";
        GridSession[2, 0].Value = "登录地址";
        GridSession[3, 0].Value = "会话ID号";
        GridSession[4, 0].Value = "充值";
        GridSession[5, 0].Value = "充值模式";
    }

    /// <summary>Delphi Open()；showModal=false 供测试/宿主复用。</summary>
    public void Open(bool showModal = true)
    {
        RefGridSession();
        if (showModal)
            ShowDialog();
    }

    public void RefGridSession()
    {
        PanelStatus.Text = "正在取得数据...";
        GridSession.Visible = false;
        ClearRow(1);

        IdSocState.FrmIDSoc.m_SessionList.Lock();
        try
        {
            if (IdSocState.FrmIDSoc.m_SessionList.Count <= 0)
            {
                ResetGridRows(2);
            }
            else
            {
                ResetGridRows(IdSocState.FrmIDSoc.m_SessionList.Count + 1);
            }
            for (int i = 0; i < IdSocState.FrmIDSoc.m_SessionList.Count; i++)
            {
                var sessInfo = IdSocState.FrmIDSoc.m_SessionList[i];
                if (!sessInfo.boClose)
                {
                    GridSession[0, i + 1].Value = i.ToString();
                    GridSession[1, i + 1].Value = sessInfo.sAccount;
                    GridSession[2, i + 1].Value = sessInfo.sIPaddr;
                    GridSession[3, i + 1].Value = sessInfo.nSessionID.ToString();
                    GridSession[4, i + 1].Value = sessInfo.nPayMent.ToString();
                    GridSession[5, i + 1].Value = sessInfo.nPayMode.ToString();
                }
            }
        }
        finally
        {
            IdSocState.FrmIDSoc.m_SessionList.UnLock();
        }
        GridSession.Visible = true;
    }

    /// <summary>Delphi Cells[x,1]:='' 清空首数据行（1:1 顺序）。</summary>
    private void ClearRow(int row)
    {
        if (row >= GridSession.RowCount) return;
        for (int c = 0; c < 6; c++)
            GridSession[c, row].Value = "";
    }

    private void ResetGridRows(int count)
    {
        GridSession.Rows.Clear();
        GridSession.Rows.Add(count);
        for (int c = 0; c < 6; c++)
            GridSession[c, 0].Value = Headers[c];
    }

    private static readonly string[] Headers = { "序号", "登录帐号", "登录地址", "会话ID号", "充值", "充值模式" };

    public void ButtonRefGridClick(object? sender)
    {
        RefGridSession();
    }
}

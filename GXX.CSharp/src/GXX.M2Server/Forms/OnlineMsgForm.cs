using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// OnlineMsg.pas TfrmOnlineMsg 1:1 转换（在线消息/全服广播/功能开关控制）。
/// StringGrid → DataGridView 单列；StrList → TStringList 等效；StrListFile 缺省 '.\MsgList.txt'，
/// 构造可注入工作目录（测试/宿主复用，缺省保持 Delphi 相对路径语义）。
/// </summary>
public sealed class OnlineMsgForm : System.Windows.Forms.Form
{
    private readonly GXX.Core.Util.TStringList StrList = new();
    private string StrListFile = ".\\MsgList.txt";
    private string FSendMsg = "";
    private readonly TUserEngine _engine;

    public System.Windows.Forms.DataGridView Grid = null!;
    public System.Windows.Forms.Button btnSend = null!;
    public System.Windows.Forms.Button ButtonAdd = null!;
    public System.Windows.Forms.Button ButtonDelete = null!;
    public System.Windows.Forms.TextBox MemoMsg = null!;
    public System.Windows.Forms.ComboBox ComboBoxMsg = null!;
    public System.Windows.Forms.CheckBox chkAutoRun = null!;
    public System.Windows.Forms.NumericUpDown seAutRunInterval = null!;
    public System.Windows.Forms.Timer tmrRun = null!;
    public System.Windows.Forms.CheckBox chkDisableTrading = null!;
    public System.Windows.Forms.CheckBox chkDisableRepair = null!;
    public System.Windows.Forms.CheckBox chkDisableSaveToStorage = null!;
    public System.Windows.Forms.CheckBox chkDisableGetFromStorage = null!;
    public System.Windows.Forms.CheckBox chkDisableBuy = null!;
    public System.Windows.Forms.CheckBox chkDisableSell = null!;
    public System.Windows.Forms.CheckBox chkDisableDropItem = null!;
    public System.Windows.Forms.CheckBox chkDisableUseNpc = null!;
    public System.Windows.Forms.CheckBox chkDisableChallenge = null!;
    public System.Windows.Forms.CheckBox chkDisableShop = null!;

    public OnlineMsgForm() : this(new TUserEngine(), null) { }

    public OnlineMsgForm(TUserEngine engine, string? workDir)
    {
        _engine = engine;
        InitializeComponent();
        if (workDir != null)
            StrListFile = Path.Combine(workDir, "MsgList.txt");
        FormCreate();
    }

    private System.Windows.Forms.CheckBox MakeChk(string text, int top)
    {
        var chk = new System.Windows.Forms.CheckBox { Text = text, Left = 14, Top = top, AutoSize = true };
        return chk;
    }

    private void InitializeComponent()
    {
        Text = "在线消息";
        Width = 640;
        Height = 560;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        var lbl1 = new System.Windows.Forms.Label { Text = "消息内容:", Left = 8, Top = 8, AutoSize = true };
        ComboBoxMsg = new System.Windows.Forms.ComboBox { Left = 90, Top = 4, Width = 380 };
        ComboBoxMsg.TextChanged += (s, e) => ComboBoxMsgChange(s);
        ComboBoxMsg.KeyPress += (s, e) => ComboBoxMsgKeyPress(s, e);
        Controls.Add(lbl1);
        Controls.Add(ComboBoxMsg);

        btnSend = new System.Windows.Forms.Button { Text = "发送(&S)", Left = 490, Top = 2, Width = 90, Height = 26 };
        btnSend.Click += (s, e) => btnSendClick(s);
        Controls.Add(btnSend);

        var grp1 = new System.Windows.Forms.GroupBox { Text = "功能开关", Left = 8, Top = 34, Width = 190, Height = 260 };
        chkDisableTrading = MakeChk("禁止交易", 18);
        chkDisableRepair = MakeChk("禁止修理", 42);
        chkDisableBuy = MakeChk("禁止购买", 66);
        chkDisableSell = MakeChk("禁止出售", 90);
        chkDisableSaveToStorage = MakeChk("禁止存入仓库", 114);
        chkDisableGetFromStorage = MakeChk("禁止取出仓库", 138);
        chkDisableDropItem = MakeChk("禁止丢弃物品", 162);
        chkDisableUseNpc = MakeChk("禁止使用NPC", 186);
        chkDisableChallenge = MakeChk("禁止挑战", 210);
        chkDisableShop = MakeChk("禁止商店", 234);
        grp1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            chkDisableTrading, chkDisableRepair, chkDisableBuy, chkDisableSell,
            chkDisableSaveToStorage, chkDisableGetFromStorage, chkDisableDropItem,
            chkDisableUseNpc, chkDisableChallenge, chkDisableShop
        });
        Controls.Add(grp1);

        var grp2 = new System.Windows.Forms.GroupBox { Text = "自动循环", Left = 8, Top = 300, Width = 190, Height = 70 };
        chkAutoRun = new System.Windows.Forms.CheckBox { Text = "自动发送", Left = 14, Top = 18, AutoSize = true };
        var lbl2 = new System.Windows.Forms.Label { Text = "间隔(秒):", Left = 14, Top = 42, AutoSize = true };
        seAutRunInterval = new System.Windows.Forms.NumericUpDown { Left = 90, Top = 38, Width = 80, Maximum = 3600 };
        grp2.Controls.Add(chkAutoRun);
        grp2.Controls.Add(lbl2);
        grp2.Controls.Add(seAutRunInterval);
        Controls.Add(grp2);

        Grid = new System.Windows.Forms.DataGridView
        {
            Left = 208,
            Top = 34,
            Width = 200,
            Height = 336,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        Grid.Columns.Add("msg", "消息列表");
        Grid.Columns[0].Width = 194;
        Grid.CellClick += (s, e) => StringGridClick(s);
        Grid.CellDoubleClick += (s, e) => StringGridDblClick(s);
        Controls.Add(Grid);

        ButtonAdd = new System.Windows.Forms.Button { Text = "增加(&A)", Left = 416, Top = 34, Width = 80, Height = 26, Enabled = false };
        ButtonAdd.Click += (s, e) => ButtonAddClick(s);
        ButtonDelete = new System.Windows.Forms.Button { Text = "删除(&D)", Left = 416, Top = 66, Width = 80, Height = 26, Enabled = false };
        ButtonDelete.Click += (s, e) => ButtonDeleteClick(s);
        Controls.Add(ButtonAdd);
        Controls.Add(ButtonDelete);

        var lbl3 = new System.Windows.Forms.Label { Text = "发送记录:", Left = 208, Top = 376, AutoSize = true };
        MemoMsg = new System.Windows.Forms.TextBox
        {
            Left = 208,
            Top = 394,
            Width = 372,
            Height = 110,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        };
        MemoMsg.TextChanged += (s, e) => MemoMsgChange(s);
        Controls.Add(lbl3);
        Controls.Add(MemoMsg);

        tmrRun = new System.Windows.Forms.Timer { Interval = 1000 };
        tmrRun.Tick += (s, e) => tmrRunTimer(s);
    }

    // ================= Delphi 1:1 =================

    /// <summary>Delphi ShowFrmOnlineMsg 入口（ShowModal + finally 复位 boDisable*）。</summary>
    public void Open(bool showModal = true)
    {
        tmrRun.Enabled = false;
        if (showModal)
            ShowDialog();
        finallyResetFlags();
    }

    /// <summary>ShowFrmOnlineMsg finally 段 1:1（活动开关全部复位）。</summary>
    private void finallyResetFlags()
    {
        var o = OnlineMsgControl.g_OnlineMsgControl;
        o.boDisableTrading = false;
        o.boDisableRepair = false;
        o.boDisableBuy = false;
        o.boDisableSell = false;
        o.boDisableSaveToStorage = false;
        o.boDisableGetFromStorage = false;
        o.boDisableDropItem = false;
        o.boDisableUseNpc = false;
        o.boDisableChallenge = false;
    }

    public void FormCreate()
    {
        if (File.Exists(StrListFile))
        {
            StrList.LoadFromFile(StrListFile);
            Grid.Rows.Add(StrList.Count);
            for (int i = 0; i < StrList.Count; i++)
                Grid[0, i].Value = StrList[i];
        }
        else
        {
            StrList.SaveToFile(StrListFile);
        }
        MemoMsg.Clear();

        chkDisableTrading.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableTrading;
        chkDisableRepair.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableRepair;
        chkDisableBuy.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableBuy;
        chkDisableSell.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableSell;
        chkDisableSaveToStorage.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableSaveToStorage;
        chkDisableGetFromStorage.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableGetFromStorage;
        chkDisableDropItem.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableDropItem;
        chkDisableUseNpc.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableUseNpc;
        chkDisableChallenge.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableChallenge;
        chkDisableShop.Checked = OnlineMsgControl.g_OnlineMsgControl.boSaveDisableShop;
    }

    public void ComboBoxMsgKeyPress(object? sender, System.Windows.Forms.KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)13)
        {
            string msg = ComboBoxMsg.Text;
            if (msg.Trim() != "")
            {
                if (ComboBoxMsg.Items.Count == 0)
                    ComboBoxMsg.Items.Add(msg);
                ComboBoxMsg.Items.Insert(1, msg);
                _engine.SendBroadCastMsgExt(msg, TMsgType.t_System);
                MemoMsg.AppendText(M2Config.sSysMsgPreFix + msg + "\r\n");
            }
            ComboBoxMsg.SelectedIndex = ComboBoxMsg.Items.Count > 0 ? 0 : -1;
            ComboBoxMsg.Text = "";
            ButtonAdd.Enabled = false;
        }
    }

    public void ComboBoxMsgChange(object? sender)
    {
        if (ComboBoxMsg.Items.Count > 20)
            ComboBoxMsg.Items.RemoveAt(19);
        ButtonAdd.Enabled = ComboBoxMsg.Text.Trim() != "";
    }

    public void StringGridClick(object? sender)
    {
        if (Grid.CurrentCell != null)
            ButtonDelete.Enabled = true;
    }

    public void ButtonAddClick(object? sender)
    {
        string msg = ComboBoxMsg.Text.Trim();
        if (msg != "")
        {
            StrList.Add(msg);
        }
        Grid.Rows.Clear();
        Grid.Rows.Add(StrList.Count);
        for (int i = 0; i < StrList.Count; i++)
            Grid[0, i].Value = StrList[i];
        ButtonAdd.Enabled = false;
        StrList.SaveToFile(StrListFile);
    }

    public void StringGridDblClick(object? sender)
    {
        if (Grid.CurrentRow != null)
            ComboBoxMsg.Text = StrList[Grid.CurrentRow.Index];
        ComboBoxMsg.Focus();
    }

    public void ButtonDeleteClick(object? sender)
    {
        if (Grid.RowCount == 1)
        {
            ButtonDelete.Enabled = false;
            return;
        }
        int row = Grid.CurrentRow?.Index ?? 0;
        StrList.Delete(row);
        Grid.Rows.Clear();
        Grid.Rows.Add(StrList.Count);
        for (int i = 0; i < StrList.Count; i++)
            Grid[0, i].Value = StrList[i];
        StrList.SaveToFile(StrListFile);
    }

    public void MemoMsgChange(object? sender)
    {
        // Delphi MemoMsg.Lines.Count > 80 → Clear（无句柄环境按 Text 行数等效判定）
        if (MemoMsg.Text.Split('\n').Length > 80)
        {
            MemoMsg.Clear();
        }
    }

    public int GridRowCount => Grid.RowCount;

    public void btnSendClick(object? sender)
    {
        var o = OnlineMsgControl.g_OnlineMsgControl;
        o.boSaveDisableTrading = chkDisableTrading.Checked;
        o.boSaveDisableRepair = chkDisableRepair.Checked;
        o.boSaveDisableBuy = chkDisableBuy.Checked;
        o.boSaveDisableSell = chkDisableSell.Checked;
        o.boSaveDisableSaveToStorage = chkDisableSaveToStorage.Checked;
        o.boSaveDisableGetFromStorage = chkDisableGetFromStorage.Checked;
        o.boSaveDisableDropItem = chkDisableDropItem.Checked;
        o.boSaveDisableUseNpc = chkDisableUseNpc.Checked;
        o.boSaveDisableChallenge = chkDisableChallenge.Checked;
        o.boSaveDisableShop = chkDisableShop.Checked;

        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("OnlineMsgControl", "DisableTrading", o.boSaveDisableTrading);
        Config.WriteBool("OnlineMsgControl", "DisableRepair", o.boSaveDisableRepair);
        Config.WriteBool("OnlineMsgControl", "DisableBuy", o.boSaveDisableBuy);
        Config.WriteBool("OnlineMsgControl", "DisableSell", o.boSaveDisableSell);
        Config.WriteBool("OnlineMsgControl", "DisableSaveToStorage", o.boSaveDisableSaveToStorage);
        Config.WriteBool("OnlineMsgControl", "DisableGetFromStorage", o.boSaveDisableGetFromStorage);
        Config.WriteBool("OnlineMsgControl", "DisableDropItem", o.boSaveDisableDropItem);
        Config.WriteBool("OnlineMsgControl", "DisableUseNpc", o.boSaveDisableUseNpc);
        Config.WriteBool("OnlineMsgControl", "DisableChallenge", o.boSaveDisableChallenge);
        Config.WriteBool("OnlineMsgControl", "DisableShop", o.boSaveDisableShop);
        Config.UpdateFile();

        string msg = ComboBoxMsg.Text;
        if (msg.Trim().Length == 0)
            return;

        if (ComboBoxMsg.Items.Count == 0)
            ComboBoxMsg.Items.Add(msg);
        else if (ComboBoxMsg.Items.IndexOf(msg) < 0)
            ComboBoxMsg.Items.Insert(1, msg);
        else
            ComboBoxMsg.SelectedIndex = ComboBoxMsg.Items.IndexOf(msg);

        o.boDisableTrading = chkDisableTrading.Checked;
        o.boDisableRepair = chkDisableRepair.Checked;
        o.boDisableBuy = chkDisableBuy.Checked;
        o.boDisableSell = chkDisableSell.Checked;
        o.boDisableSaveToStorage = chkDisableSaveToStorage.Checked;
        o.boDisableGetFromStorage = chkDisableGetFromStorage.Checked;
        o.boDisableDropItem = chkDisableDropItem.Checked;
        o.boDisableUseNpc = chkDisableUseNpc.Checked;
        o.boDisableChallenge = chkDisableChallenge.Checked;

        FSendMsg = msg;
        _engine.SendBroadCastMsgExt(FSendMsg, TMsgType.t_System);
        MemoMsg.AppendText(M2Config.sSysMsgPreFix + FSendMsg + "\r\n");

        if (chkAutoRun.Checked && seAutRunInterval.Value > 0)
        {
            tmrRun.Interval = (int)seAutRunInterval.Value * 1000;
            tmrRun.Enabled = true;
        }
        else
        {
            tmrRun.Enabled = false;
        }
    }

    public void tmrRunTimer(object? sender)
    {
        _engine.SendBroadCastMsgExt(FSendMsg, TMsgType.t_System);
        MemoMsg.AppendText(M2Config.sSysMsgPreFix + FSendMsg + "\r\n");
    }
}

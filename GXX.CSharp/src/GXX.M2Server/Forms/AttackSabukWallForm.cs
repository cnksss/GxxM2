using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// AttackSabukWallConfig.pas TFrmAttackSabukWall 1:1 转换（增加/编辑攻城行会申请对话框）。
/// </summary>
public sealed class AttackSabukWallForm : System.Windows.Forms.Form
{
    private bool FAddAttackGuild;

    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    public System.Windows.Forms.Label Label1 = null!;
    public System.Windows.Forms.Label Label2 = null!;
    public System.Windows.Forms.TextBox EditGuildName = null!;
    public System.Windows.Forms.DateTimePicker RzDateTimeEditAttackDate = null!;
    public System.Windows.Forms.Button ButtonOK = null!;
    public System.Windows.Forms.ListBox ListBoxGuild = null!;
    public System.Windows.Forms.CheckBox CheckBoxAll = null!;
    public System.Windows.Forms.Button ButtonCancel = null!;

    /// <summary>Delphi 全局 FrmAttackSabukWall（单元级变量 1:1）。</summary>
    public static AttackSabukWallForm? FrmAttackSabukWall;

    /// <summary>Delphi 全局 frmCastleManage（ButtonOK 后刷新攻城列表）。</summary>
    public static CastleManageForm? frmCastleManage;

    public AttackSabukWallForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "攻城设置";
        Width = 380;
        Height = 320;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        GroupBox1 = new System.Windows.Forms.GroupBox { Text = "攻城行会", Left = 8, Top = 8, Width = 350, Height = 270 };
        Controls.Add(GroupBox1);

        Label1 = new System.Windows.Forms.Label { Text = "行会名称:", Left = 14, Top = 24, AutoSize = true };
        EditGuildName = new System.Windows.Forms.TextBox { Left = 90, Top = 20, Width = 150 };
        EditGuildName.TextChanged += (s, e) => EditGuildNameChange(s);
        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(EditGuildName);

        Label2 = new System.Windows.Forms.Label { Text = "攻城日期:", Left = 14, Top = 52, AutoSize = true };
        RzDateTimeEditAttackDate = new System.Windows.Forms.DateTimePicker { Left = 90, Top = 48, Width = 150, Format = System.Windows.Forms.DateTimePickerFormat.Short };
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(RzDateTimeEditAttackDate);

        CheckBoxAll = new System.Windows.Forms.CheckBox { Text = "全部行会", Left = 14, Top = 80, AutoSize = true };
        CheckBoxAll.Click += (s, e) => CheckBoxAllClick(s);
        GroupBox1.Controls.Add(CheckBoxAll);

        ListBoxGuild = new System.Windows.Forms.ListBox { Left = 14, Top = 104, Width = 226, Height = 130 };
        ListBoxGuild.Click += (s, e) => ListBoxGuildClick(s);
        GroupBox1.Controls.Add(ListBoxGuild);

        ButtonOK = new System.Windows.Forms.Button { Text = "确定(&O)", Left = 250, Top = 104, Width = 88, Height = 26 };
        ButtonOK.Click += (s, e) => ButtonOKClick(s);
        GroupBox1.Controls.Add(ButtonOK);

        ButtonCancel = new System.Windows.Forms.Button { Text = "取消(&C)", Left = 250, Top = 138, Width = 88, Height = 26 };
        ButtonCancel.Click += (s, e) => ButtonCancelClick(s);
        GroupBox1.Controls.Add(ButtonCancel);
    }

    // ================= Delphi 1:1 =================

    private void LoadGuildList()
    {
        ListBoxGuild.Items.Clear();
        foreach (var guild in CastleState.g_GuildManager.GuildList)
            ListBoxGuild.Items.Add(guild.sGuildName);
    }

    /// <summary>Delphi Open(boAdd)；showModal=false 供测试/宿主复用。</summary>
    public void Open(bool boAdd, bool showModal = true)
    {
        FAddAttackGuild = boAdd;

        if (FAddAttackGuild)
        {
            Text = "增加攻城行会";
            EditGuildName.Text = "";
            RzDateTimeEditAttackDate.Value = DateTime.Today;
        }
        else
        {
            if (CastleManageForm.SelAttackGuildInfo != null)
            {
                Text = "编辑攻城行会 " + CastleManageForm.SelAttackGuildInfo.sGuildName;
                EditGuildName.Text = CastleManageForm.SelAttackGuildInfo.sGuildName;
                RzDateTimeEditAttackDate.Value = CastleManageForm.SelAttackGuildInfo.AttackDate;
            }
            else
            {
                Text = "编辑攻城行会";
                EditGuildName.Text = "";
                RzDateTimeEditAttackDate.Value = DateTime.Now;
            }
        }
        LoadGuildList();
        if (showModal)
            ShowDialog();
    }

    public (bool ok, string? msg) ButtonOKClick(object? sender)
    {
        if (CastleManageForm.CurCastle != null)
        {
            ButtonOK.Enabled = false;
            string sGuildName = EditGuildName.Text.Trim();
            DateTime attackDate = RzDateTimeEditAttackDate.Value.Date;
            var CurCastle = CastleManageForm.CurCastle;
            if (FAddAttackGuild)
            {
                if (CheckBoxAll.Checked)
                {
                    foreach (var guild in CastleState.g_GuildManager.GuildList)
                        CurCastle.AddAttackerInfo(guild, attackDate);
                    CurCastle.Save();
                }
                else
                {
                    var guild = CastleState.g_GuildManager.FindGuild(sGuildName);
                    if (guild != null)
                    {
                        CurCastle.AddAttackerInfo(guild, attackDate);
                        CurCastle.Save();
                    }
                    else
                    {
                        M2Forms.MessageBox("输入的行会不存在！", "提示信息", M2Forms.MB_ICONQUESTION);
                        return (false, "输入的行会不存在！");
                    }
                }
            }
            else
            {
                if (CheckBoxAll.Checked)
                {
                    foreach (var attackerInfo in CurCastle.m_AttackWarList)
                        attackerInfo.AttackDate = attackDate;
                    CurCastle.Save();
                }
                else
                {
                    if (CastleManageForm.SelAttackGuildInfo != null)
                    {
                        CastleManageForm.SelAttackGuildInfo.AttackDate = attackDate;
                        CurCastle.Save();
                    }
                }
            }
            frmCastleManage?.RefCastleAttackSabukWall();
        }
        Close();
        return (true, null);
    }

    public void ListBoxGuildClick(object? sender)
    {
        if (ListBoxGuild.SelectedIndex >= 0 && ListBoxGuild.SelectedIndex < ListBoxGuild.Items.Count)
            EditGuildName.Text = ListBoxGuild.Items[ListBoxGuild.SelectedIndex]?.ToString() ?? "";
        else
            EditGuildName.Text = "";
    }

    public void CheckBoxAllClick(object? sender)
    {
        EditGuildName.Enabled = !CheckBoxAll.Checked;
    }

    public void EditGuildNameChange(object? sender)
    {
        // Delphi EditGuildName 无 OnChange 处理器（占位保持结构对齐）
    }

    public void ButtonCancelClick(object? sender)
    {
        Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (FrmAttackSabukWall == this) FrmAttackSabukWall = null;
        base.Dispose(disposing);
    }
}

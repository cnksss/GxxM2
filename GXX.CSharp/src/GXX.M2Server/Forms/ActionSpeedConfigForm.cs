using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ActionSpeedConfig.pas TfrmActionSpeed 1:1 转换（动作速度控制：组合/跑刺杀/跑攻击/走攻击/跑魔法间隔）。
/// SpinEditEx → NumericUpDown；事件处理器与 g_Config 写入时机（boOpened 门控）按 Delphi 1:1。
/// </summary>
public sealed class ActionSpeedConfigForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;

    public System.Windows.Forms.NumericUpDown EditRunLongHitIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditActionIntervalTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxControlActionInterval = null!;
    public System.Windows.Forms.CheckBox CheckBoxControlRunLongHit = null!;
    public System.Windows.Forms.NumericUpDown EditRunHitIntervalTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxControlRunHit = null!;
    public System.Windows.Forms.NumericUpDown EditWalkHitIntervalTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxControlWalkHit = null!;
    public System.Windows.Forms.Button ButtonSave = null!;
    public System.Windows.Forms.Button ButtonDefault = null!;
    public System.Windows.Forms.Button ButtonClose = null!;
    public System.Windows.Forms.CheckBox CheckBoxIncremeng = null!;
    public System.Windows.Forms.NumericUpDown EditRunMagicIntervalTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxControlRunMagic = null!;

    public ActionSpeedConfigForm()
    {
        InitializeComponent();
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, out System.Windows.Forms.Label lb)
    {
        lb = new System.Windows.Forms.Label { Text = caption, Left = 14, Top = top + 4, AutoSize = true };
        var edit = new System.Windows.Forms.NumericUpDown
        {
            Left = 150,
            Top = top,
            Width = 100,
            Maximum = 60000,
            Increment = 10
        };
        return edit;
    }

    private void InitializeComponent()
    {
        Text = "动作速度设置";
        Width = 420;
        Height = 430;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        System.Windows.Forms.Label lb;
        var groupBox1 = new System.Windows.Forms.GroupBox { Text = "动作速度", Left = 8, Top = 8, Width = 390, Height = 330 };
        Controls.Add(groupBox1);

        lb = new System.Windows.Forms.Label { Text = "组合操作间隔:", Left = 14, Top = 28, AutoSize = true };
        EditActionIntervalTime = new System.Windows.Forms.NumericUpDown { Left = 150, Top = 24, Width = 100, Maximum = 60000, Increment = 10 };
        EditActionIntervalTime.ValueChanged += (s, e) => EditActionIntervalTimeChange(s);
        groupBox1.Controls.Add(lb);
        groupBox1.Controls.Add(EditActionIntervalTime);
        CheckBoxControlActionInterval = new System.Windows.Forms.CheckBox { Text = "控制组合操作", Left = 262, Top = 24, AutoSize = true };
        CheckBoxControlActionInterval.Click += (s, e) => CheckBoxControlActionIntervalClick(s);
        groupBox1.Controls.Add(CheckBoxControlActionInterval);

        lb = new System.Windows.Forms.Label { Text = "跑位刺杀间隔:", Left = 14, Top = 60, AutoSize = true };
        EditRunLongHitIntervalTime = new System.Windows.Forms.NumericUpDown { Left = 150, Top = 56, Width = 100, Maximum = 60000, Increment = 10 };
        EditRunLongHitIntervalTime.ValueChanged += (s, e) => EditRunLongHitIntervalTimeChange(s);
        groupBox1.Controls.Add(lb);
        groupBox1.Controls.Add(EditRunLongHitIntervalTime);
        CheckBoxControlRunLongHit = new System.Windows.Forms.CheckBox { Text = "控制跑位刺杀", Left = 262, Top = 56, AutoSize = true };
        CheckBoxControlRunLongHit.Click += (s, e) => CheckBoxControlRunLongHitClick(s);
        groupBox1.Controls.Add(CheckBoxControlRunLongHit);

        lb = new System.Windows.Forms.Label { Text = "跑位攻击间隔:", Left = 14, Top = 92, AutoSize = true };
        EditRunHitIntervalTime = new System.Windows.Forms.NumericUpDown { Left = 150, Top = 88, Width = 100, Maximum = 60000, Increment = 10 };
        EditRunHitIntervalTime.ValueChanged += (s, e) => EditRunHitIntervalTimeChange(s);
        groupBox1.Controls.Add(lb);
        groupBox1.Controls.Add(EditRunHitIntervalTime);
        CheckBoxControlRunHit = new System.Windows.Forms.CheckBox { Text = "控制跑位攻击", Left = 262, Top = 88, AutoSize = true };
        CheckBoxControlRunHit.Click += (s, e) => CheckBoxControlRunHitClick(s);
        groupBox1.Controls.Add(CheckBoxControlRunHit);

        lb = new System.Windows.Forms.Label { Text = "走位攻击间隔:", Left = 14, Top = 124, AutoSize = true };
        EditWalkHitIntervalTime = new System.Windows.Forms.NumericUpDown { Left = 150, Top = 120, Width = 100, Maximum = 60000, Increment = 10 };
        EditWalkHitIntervalTime.ValueChanged += (s, e) => EditWalkHitIntervalTimeChange(s);
        groupBox1.Controls.Add(lb);
        groupBox1.Controls.Add(EditWalkHitIntervalTime);
        CheckBoxControlWalkHit = new System.Windows.Forms.CheckBox { Text = "控制走位攻击", Left = 262, Top = 120, AutoSize = true };
        CheckBoxControlWalkHit.Click += (s, e) => CheckBoxControlWalkHitClick(s);
        groupBox1.Controls.Add(CheckBoxControlWalkHit);

        lb = new System.Windows.Forms.Label { Text = "跑位魔法间隔:", Left = 14, Top = 156, AutoSize = true };
        EditRunMagicIntervalTime = new System.Windows.Forms.NumericUpDown { Left = 150, Top = 152, Width = 100, Maximum = 60000, Increment = 10 };
        EditRunMagicIntervalTime.ValueChanged += (s, e) => EditRunMagicIntervalTimeChange(s);
        groupBox1.Controls.Add(lb);
        groupBox1.Controls.Add(EditRunMagicIntervalTime);
        CheckBoxControlRunMagic = new System.Windows.Forms.CheckBox { Text = "控制跑位魔法", Left = 262, Top = 152, AutoSize = true };
        CheckBoxControlRunMagic.Click += (s, e) => CheckBoxControlRunMagicClick(s);
        groupBox1.Controls.Add(CheckBoxControlRunMagic);

        CheckBoxIncremeng = new System.Windows.Forms.CheckBox { Text = "微调步长(1)", Left = 14, Top = 188, AutoSize = true };
        CheckBoxIncremeng.Click += (s, e) => CheckBoxIncremengClick(s);
        groupBox1.Controls.Add(CheckBoxIncremeng);

        ButtonSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 14, Top = 348, Width = 80, Height = 26, Enabled = false };
        ButtonSave.Click += (s, e) => ButtonSaveClick(s);
        ButtonDefault = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 110, Top = 348, Width = 80, Height = 26 };
        ButtonDefault.Click += (s, e) => ButtonDefaultClick(s);
        ButtonClose = new System.Windows.Forms.Button { Text = "关闭(&C)", Left = 206, Top = 348, Width = 80, Height = 26 };
        ButtonClose.Click += (s, e) => ButtonCloseClick(s);
        Controls.Add(ButtonSave);
        Controls.Add(ButtonDefault);
        Controls.Add(ButtonClose);
    }

    // ================= Delphi 1:1 =================

    public bool IsModValued => boModValued;

    private void ModValue()
    {
        boModValued = true;
        ButtonSave.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        ButtonSave.Enabled = false;
    }

    /// <summary>Delphi Open()；showModal=false 供测试/宿主复用。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        uModValue();
        RefSpeedConfig();
        boOpened = true;
        if (showModal)
            ShowDialog();
    }

    public void ButtonCloseClick(object? sender)
    {
        const string sExitMsg = "设置已被修改是否不保存设置退出？";
        const string sExitMsgTitle = "确认信息";
        if (!boModValued)
        {
            Close();
            return;
        }
        if (M2Forms.MessageBox(sExitMsg, sExitMsgTitle, M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
        {
            Close();
        }
    }

    public void RefSpeedConfig()
    {
        EditActionIntervalTime.Value = M2Config.dwActionIntervalTime;
        EditRunLongHitIntervalTime.Value = M2Config.dwRunLongHitIntervalTime;
        EditRunHitIntervalTime.Value = M2Config.dwRunHitIntervalTime;
        EditWalkHitIntervalTime.Value = M2Config.dwWalkHitIntervalTime;
        EditRunMagicIntervalTime.Value = M2Config.dwRunMagicIntervalTime;
        CheckBoxControlActionInterval.Checked = M2Config.boControlActionInterval;
        CheckBoxControlRunLongHit.Checked = M2Config.boControlRunLongHit;
        CheckBoxControlRunHit.Checked = M2Config.boControlRunHit;
        CheckBoxControlWalkHit.Checked = M2Config.boControlWalkHit;
        CheckBoxControlRunMagic.Checked = M2Config.boControlRunMagic;
    }

    public void ButtonDefaultClick(object? sender)
    {
        const string sExitMsg = "是否确认恢复默认设置？";
        const string sExitMsgTitle = "确认信息";
        if (M2Forms.MessageBox(sExitMsg, sExitMsgTitle, M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        boOpened = false;
        ModValue();
        M2Config.dwActionIntervalTime = 400;
        M2Config.dwRunLongHitIntervalTime = 800;
        M2Config.dwRunHitIntervalTime = 800;
        M2Config.dwWalkHitIntervalTime = 800;
        M2Config.dwRunMagicIntervalTime = 900;
        M2Config.boControlActionInterval = true;
        M2Config.boControlRunLongHit = true;
        M2Config.boControlRunHit = true;
        M2Config.boControlWalkHit = true;
        M2Config.boControlRunMagic = true;

        RefSpeedConfig();
        boOpened = true;
    }

    public void SaveConfig()
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "ControlActionInterval", M2Config.boControlActionInterval);
        Config.WriteBool("Setup", "ControlWalkHit", M2Config.boControlWalkHit);
        Config.WriteBool("Setup", "ControlRunLongHit", M2Config.boControlRunLongHit);
        Config.WriteBool("Setup", "ControlRunHit", M2Config.boControlRunHit);
        Config.WriteBool("Setup", "ControlRunMagic", M2Config.boControlRunMagic);

        Config.WriteInteger("Setup", "ActionIntervalTime", (int)M2Config.dwActionIntervalTime);
        Config.WriteInteger("Setup", "RunLongHitIntervalTime", (int)M2Config.dwRunLongHitIntervalTime);
        Config.WriteInteger("Setup", "RunHitIntervalTime", (int)M2Config.dwRunHitIntervalTime);
        Config.WriteInteger("Setup", "WalkHitIntervalTime", (int)M2Config.dwWalkHitIntervalTime);
        Config.WriteInteger("Setup", "RunMagicIntervalTime", (int)M2Config.dwRunMagicIntervalTime);
        Config.UpdateFile();
    }

    public void ButtonSaveClick(object? sender)
    {
        SaveConfig();
        uModValue();
    }

    public void CheckBoxIncremengClick(object? sender)
    {
        int nIncrement = CheckBoxIncremeng.Checked ? 1 : 10;

        EditActionIntervalTime.Increment = nIncrement;
        EditRunLongHitIntervalTime.Increment = nIncrement;
        EditRunHitIntervalTime.Increment = nIncrement;
        EditWalkHitIntervalTime.Increment = nIncrement;
    }

    public void CheckBoxControlActionIntervalClick(object? sender)
    {
        bool boStatus = CheckBoxControlActionInterval.Checked;
        EditActionIntervalTime.Enabled = boStatus;
        CheckBoxControlRunLongHit.Enabled = boStatus;
        CheckBoxControlRunHit.Enabled = boStatus;
        CheckBoxControlWalkHit.Enabled = boStatus;
        CheckBoxControlRunMagic.Enabled = boStatus;

        CheckBoxControlRunLongHitClick(sender);
        CheckBoxControlRunHitClick(sender);
        CheckBoxControlWalkHitClick(sender);
        CheckBoxControlRunMagicClick(sender);

        if (!boOpened)
            return;
        M2Config.boControlActionInterval = boStatus;
        ModValue();
    }

    public void EditActionIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwActionIntervalTime = (uint)EditActionIntervalTime.Value;
        ModValue();
    }

    public void CheckBoxControlRunLongHitClick(object? sender)
    {
        bool boStatus = CheckBoxControlRunLongHit.Checked && CheckBoxControlRunLongHit.Enabled;

        EditRunLongHitIntervalTime.Enabled = boStatus;

        if (!boOpened)
            return;
        M2Config.boControlRunLongHit = boStatus;
        ModValue();
    }

    public void EditRunLongHitIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwRunLongHitIntervalTime = (uint)EditRunLongHitIntervalTime.Value;
        ModValue();
    }

    public void CheckBoxControlRunHitClick(object? sender)
    {
        bool boStatus = CheckBoxControlRunHit.Checked && CheckBoxControlRunHit.Enabled;
        EditRunHitIntervalTime.Enabled = boStatus;

        if (!boOpened)
            return;
        M2Config.boControlRunHit = boStatus;
        ModValue();
    }

    public void EditRunHitIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwRunHitIntervalTime = (uint)EditRunHitIntervalTime.Value;
        ModValue();
    }

    public void CheckBoxControlWalkHitClick(object? sender)
    {
        bool boStatus = CheckBoxControlWalkHit.Checked && CheckBoxControlWalkHit.Enabled;
        EditWalkHitIntervalTime.Enabled = boStatus;

        if (!boOpened)
            return;
        M2Config.boControlWalkHit = boStatus;
        ModValue();
    }

    public void EditWalkHitIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwWalkHitIntervalTime = (uint)EditWalkHitIntervalTime.Value;
        ModValue();
    }

    public void CheckBoxControlRunMagicClick(object? sender)
    {
        bool boStatus = CheckBoxControlRunMagic.Checked && CheckBoxControlRunMagic.Enabled;
        EditRunMagicIntervalTime.Enabled = boStatus;

        if (!boOpened)
            return;
        M2Config.boControlRunMagic = boStatus;
        ModValue();
    }

    public void EditRunMagicIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwRunMagicIntervalTime = (uint)EditRunMagicIntervalTime.Value;
        ModValue();
    }
}

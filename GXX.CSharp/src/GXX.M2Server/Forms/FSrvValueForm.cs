using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// FSrvValue.pas TfrmServerValue 1:1 转换（引擎资源限额与怪物处理参数设置）。
/// SpinEditEx → NumericUpDown；Min/Max 限幅、boOpened 门控、INI 键名与 'FLASE' 原文拼写 1:1。
/// </summary>
public sealed class FSrvValueForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;

    public System.Windows.Forms.NumericUpDown ESendBlock = null!;
    public System.Windows.Forms.NumericUpDown ECheckBlock = null!;
    public System.Windows.Forms.NumericUpDown EAvailableBlock = null!;
    public System.Windows.Forms.NumericUpDown EGateLoad = null!;
    public System.Windows.Forms.NumericUpDown EHum = null!;
    public System.Windows.Forms.NumericUpDown EMon = null!;
    public System.Windows.Forms.NumericUpDown EZen = null!;
    public System.Windows.Forms.NumericUpDown ESoc = null!;
    public System.Windows.Forms.NumericUpDown EDec = null!;
    public System.Windows.Forms.NumericUpDown ENpc = null!;
    public System.Windows.Forms.CheckBox CbViewHack = null!;
    public System.Windows.Forms.CheckBox CkViewAdmfail = null!;
    public System.Windows.Forms.NumericUpDown EditZenMonRate = null!;
    public System.Windows.Forms.NumericUpDown EditZenMonTime = null!;
    public System.Windows.Forms.NumericUpDown EditProcessTime = null!;
    public System.Windows.Forms.NumericUpDown EditProcessMonsterInterval = null!;
    public System.Windows.Forms.CheckBox CheckBoxSendCompressDataToRunGate = null!;
    public System.Windows.Forms.Button BitBtn1 = null!;
    public System.Windows.Forms.Button ButtonDefault = null!;

    public FSrvValueForm()
    {
        InitializeComponent();
        FormCreate();
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, string hint, out System.Windows.Forms.Label lb)
    {
        lb = new System.Windows.Forms.Label { Text = caption, Left = 14, Top = top + 4, AutoSize = true };
        var edit = new System.Windows.Forms.NumericUpDown { Left = 170, Top = top, Width = 110, Maximum = 1000000 };
        return edit;
    }

    private void InitializeComponent()
    {
        Text = "引擎参数设置";
        Width = 420;
        Height = 560;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        var items = new (string caption, string hint, System.Windows.Forms.NumericUpDown[] edits)[0];
        System.Windows.Forms.Label lb;
        int y = 8;

        lb = null!;
        ESendBlock = MakeSpin("发送数据块:", y, "与网关之间一次传输数据块大小(字节)。", out lb); Wire(ESendBlock, ESendBlockChange); AddRow(lb, ESendBlock, ref y);
        ECheckBlock = MakeSpin("自检数据块:", y, "与网关之间传输指定大小数据后，进行一次自检。", out lb); Wire(ECheckBlock, ECheckBlockChange); AddRow(lb, ECheckBlock, ref y);
        EAvailableBlock = MakeSpin("可用数据块:", y, "", out lb); Wire(EAvailableBlock, EAvailableBlockChange); AddRow(lb, EAvailableBlock, ref y);
        EGateLoad = MakeSpin("网关负载:", y, "设置与网关传输负载测试数据量大小。", out lb); Wire(EGateLoad, EGateLoadChange); AddRow(lb, EGateLoad, ref y);
        EHum = MakeSpin("人物处理数:", y, "处理人物数据分配时间。", out lb); Wire(EHum, EHumChange); AddRow(lb, EHum, ref y);
        EMon = MakeSpin("怪物处理数:", y, "处理怪物数据分配时间。", out lb); Wire(EMon, EMonChange); AddRow(lb, EMon, ref y);
        EZen = MakeSpin("刷新怪物数:", y, "刷新怪物数据分配时间。", out lb); Wire(EZen, EZenChange); AddRow(lb, EZen, ref y);
        ESoc = MakeSpin("网关处理数:", y, "处理网关数据分配时间。", out lb); Wire(ESoc, ESocChange); AddRow(lb, ESoc, ref y);
        EDec = MakeSpin("数据解密数:", y, "", out lb); Wire(EDec, EDecChange); AddRow(lb, EDec, ref y);
        ENpc = MakeSpin("NPC处理数:", y, "处理NPC数据分配时间。", out lb); Wire(ENpc, ENpcChange); AddRow(lb, ENpc, ref y);

        CbViewHack = new System.Windows.Forms.CheckBox { Text = "显示外挂信息", Left = 14, Top = y + 2, AutoSize = true };
        CbViewHack.Click += (s, e) => CbViewHackClick(s);
        Controls.Add(CbViewHack); y += 26;
        CkViewAdmfail = new System.Windows.Forms.CheckBox { Text = "显示进入失败", Left = 14, Top = y + 2, AutoSize = true };
        CkViewAdmfail.Click += (s, e) => CkViewAdmfailClick(s);
        Controls.Add(CkViewAdmfail); y += 30;

        EditZenMonRate = MakeSpin("刷怪倍率:", y, "刷怪倍率，倍率除以10为实际倍率(设置为10则为1:1)，此倍率以刷怪文件设置为准，数字越大，刷怪数量越小。", out lb); Wire(EditZenMonRate, EditZenMonRateChange); AddRow(lb, EditZenMonRate, ref y);
        EditZenMonTime = MakeSpin("刷怪间隔:", y, "刷怪间隔控制，数字越大，刷怪速度越慢。", out lb); Wire(EditZenMonTime, EditZenMonTimeChange); AddRow(lb, EditZenMonTime, ref y);
        EditProcessTime = MakeSpin("怪物处理间隔:", y, "处理怪物的时间间隔，此设置数字越大，怪物行动越慢。数字越小，怪物行动越灵活，CPU占用越高。默认值250", out lb); Wire(EditProcessTime, EditProcessTimeChange); AddRow(lb, EditProcessTime, ref y);
        EditProcessMonsterInterval = MakeSpin("空闲检测次数:", y, "空闲时处理怪物检测次数，数字越大怪物运行越迟钝。默认=3", out lb); Wire(EditProcessMonsterInterval, EditProcessMonsterIntervalChange); AddRow(lb, EditProcessMonsterInterval, ref y);
        CheckBoxSendCompressDataToRunGate = new System.Windows.Forms.CheckBox { Text = "发送压缩数据到网关", Left = 14, Top = y + 2, AutoSize = true };
        CheckBoxSendCompressDataToRunGate.Click += (s, e) => CheckBoxSendCompressDataToRunGateClick(s);
        Controls.Add(CheckBoxSendCompressDataToRunGate); y += 34;

        BitBtn1 = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 14, Top = y, Width = 90, Height = 26, Enabled = false };
        BitBtn1.Click += (s, e) => BitBtn1Click(s);
        ButtonDefault = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 120, Top = y, Width = 90, Height = 26 };
        ButtonDefault.Click += (s, e) => ButtonDefaultClick(s);
        Controls.Add(BitBtn1);
        Controls.Add(ButtonDefault);
        ClientSize = new System.Drawing.Size(400, y + 40);
    }

    private void Wire(System.Windows.Forms.NumericUpDown edit, Action<object?> handler)
        => edit.ValueChanged += (s, e) => handler(s);

    private void AddRow(System.Windows.Forms.Label lb, System.Windows.Forms.Control edit, ref int y)
    {
        Controls.Add(lb);
        Controls.Add(edit);
        y += 30;
    }

    /// <summary>Delphi FormCreate（Hint 文案 1:1，WinForms ToolTip 等效）。</summary>
    public void FormCreate()
    {
        var tips = new System.Windows.Forms.ToolTip();
        tips.SetToolTip(ESendBlock, "与网关之间一次传输数据块大小(字节)。");
        tips.SetToolTip(ECheckBlock, "与网关之间传输指定大小数据后，进行一次自检。");
        tips.SetToolTip(EGateLoad, "设置与网关传输负载测试数据量大小。");
        tips.SetToolTip(EHum, "处理人物数据分配时间。");
        tips.SetToolTip(EMon, "处理怪物数据分配时间。");
        tips.SetToolTip(EZen, "刷新怪物数据分配时间。");
        tips.SetToolTip(ESoc, "处理网关数据分配时间。");
        tips.SetToolTip(ENpc, "处理NPC数据分配时间。");
        tips.SetToolTip(EditZenMonRate, "刷怪倍率，倍率除以10为实际倍率(设置为10则为1:1)，此倍率以刷怪文件设置为准，数字越大，刷怪数量越小。");
        tips.SetToolTip(EditZenMonTime, "刷怪间隔控制，数字越大，刷怪速度越慢。");
        tips.SetToolTip(EditProcessTime, "处理怪物的时间间隔，此设置数字越大，怪物行动越慢。数字越小，怪物行动越灵活，CPU占用越高。默认值250");
        tips.SetToolTip(EditProcessMonsterInterval, "空闲时处理怪物检测次数，数字越大怪物运行越迟钝。默认=3");
    }

    // ================= Delphi 1:1 =================

    private void ModValue()
    {
        boModValued = true;
        BitBtn1.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        BitBtn1.Enabled = false;
    }

    /// <summary>Delphi AdjuestServerConfig；showModal=false 供测试/宿主复用。</summary>
    public bool AdjuestServerConfig(bool showModal = true)
    {
        boOpened = false;
        RefShow();
        uModValue();
        boOpened = true;
        if (showModal)
            ShowDialog();
        return true;
    }

    public void RefShow()
    {
        EHum.Value = M2ShareLimits.g_dwHumLimit;
        EMon.Value = M2ShareLimits.g_dwMonLimit;
        EZen.Value = M2ShareLimits.g_dwZenLimit;
        ESoc.Value = M2ShareLimits.g_dwSocLimit;
        EDec.Value = M2ShareLimits.nDecLimit;
        ENpc.Value = M2ShareLimits.g_dwNpcLimit;
        ESendBlock.Value = M2Config.nSendBlock;
        ECheckBlock.Value = M2Config.nCheckBlock;
        EAvailableBlock.Value = M2Config.nAvailableBlock;
        EGateLoad.Value = M2Config.nGateLoad;
        CbViewHack.Checked = M2Config.boViewHackMessage;
        CkViewAdmfail.Checked = M2Config.boViewAdmissionFailure;

        EditZenMonRate.Value = M2Config.nMonGenRate;
        EditZenMonTime.Value = M2Config.dwRegenMonstersTime;
        EditProcessTime.Value = M2Config.dwProcessMonstersTime;
        EditProcessMonsterInterval.Value = M2Config.nProcessMonsterInterval;
        CheckBoxSendCompressDataToRunGate.Checked = M2Config.boSendCompressDataToRunGate;
    }

    public void BitBtn1Click(object? sender)
    {
        string tBool;
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Server", "HumLimit", (int)M2ShareLimits.g_dwHumLimit);
        Config.WriteInteger("Server", "MonLimit", (int)M2ShareLimits.g_dwMonLimit);
        Config.WriteInteger("Server", "ZenLimit", (int)M2ShareLimits.g_dwZenLimit);
        Config.WriteInteger("Server", "SocLimit", (int)M2ShareLimits.g_dwSocLimit);
        Config.WriteInteger("Server", "DecLimit", M2ShareLimits.nDecLimit);
        Config.WriteInteger("Server", "NpcLimit", (int)M2ShareLimits.g_dwNpcLimit);
        Config.WriteInteger("Server", "SendBlock", (int)M2Config.nSendBlock);
        Config.WriteInteger("Server", "CheckBlock", (int)M2Config.nCheckBlock);
        Config.WriteInteger("Server", "GateLoad", (int)M2Config.nGateLoad);
        tBool = M2Config.boViewHackMessage ? "TRUE" : "FLASE";
        Config.WriteString("Server", "ViewHackMessage", tBool);
        tBool = M2Config.boViewAdmissionFailure ? "TRUE" : "FLASE";
        Config.WriteString("Server", "ViewAdmissionFailure", tBool);

        Config.WriteInteger("Setup", "GenMonRate", (int)M2Config.nMonGenRate);
        Config.WriteInteger("Server", "ProcessMonstersTime", (int)M2Config.dwProcessMonstersTime);
        Config.WriteInteger("Server", "RegenMonstersTime", (int)M2Config.dwRegenMonstersTime);
        Config.WriteInteger("Setup", "ProcessMonsterInterval", (int)M2Config.nProcessMonsterInterval);
        Config.WriteBool("Setup", "SendCompressDataToRunGate2", M2Config.boSendCompressDataToRunGate);
        Config.UpdateFile();

        uModValue();
    }

    public void EditZenMonRateChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.nMonGenRate = (uint)EditZenMonRate.Value;
        ModValue();
    }

    public void EditZenMonTimeChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.dwRegenMonstersTime = (uint)EditZenMonTime.Value;
        ModValue();
    }

    public void EditProcessTimeChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.dwProcessMonstersTime = (uint)EditProcessTime.Value;
        ModValue();
    }

    public void ESendBlockChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.nSendBlock = (uint)Math.Max(10, (int)ESendBlock.Value);
        ModValue();
    }

    public void ECheckBlockChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.nCheckBlock = (uint)ECheckBlock.Value;
        ModValue();
    }

    public void EGateLoadChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.nGateLoad = (uint)EGateLoad.Value;
        ModValue();
    }

    public void EHumChange(object? sender)
    {
        if (!boOpened) return;
        M2ShareLimits.g_dwHumLimit = (uint)Math.Min(150, (int)EHum.Value);
        ModValue();
    }

    public void EMonChange(object? sender)
    {
        if (!boOpened) return;
        M2ShareLimits.g_dwMonLimit = (uint)Math.Min(150, (int)EMon.Value);
        ModValue();
    }

    public void EZenChange(object? sender)
    {
        if (!boOpened) return;
        M2ShareLimits.g_dwZenLimit = (uint)Math.Min(150, (int)EZen.Value);
        ModValue();
    }

    public void ESocChange(object? sender)
    {
        if (!boOpened) return;
        M2ShareLimits.g_dwSocLimit = (uint)Math.Min(150, (int)ESoc.Value);
        ModValue();
    }

    public void ENpcChange(object? sender)
    {
        if (!boOpened) return;
        M2ShareLimits.g_dwNpcLimit = (uint)Math.Min(150, (int)ENpc.Value);
        ModValue();
    }

    public void EDecChange(object? sender)
    {
        // Delphi 无 EDecChange 处理器（EDec 仅展示），占位保持结构对齐
    }

    public void EAvailableBlockChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.nAvailableBlock = (uint)Math.Max(10, (int)EAvailableBlock.Value);
        ModValue();
    }

    public void CbViewHackClick(object? sender)
    {
        if (!boOpened) return;
        M2Config.boViewHackMessage = CbViewHack.Checked;
        ModValue();
    }

    public void CkViewAdmfailClick(object? sender)
    {
        if (!boOpened) return;
        M2Config.boViewAdmissionFailure = CkViewAdmfail.Checked;
        ModValue();
    }

    public void ButtonDefaultClick(object? sender)
    {
        if (M2Forms.MessageBox("是否确认恢复默认设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        M2ShareLimits.g_dwHumLimit = 30;
        M2ShareLimits.g_dwMonLimit = 10;
        M2ShareLimits.g_dwZenLimit = 5;
        M2ShareLimits.g_dwSocLimit = 10;
        M2ShareLimits.nDecLimit = 20;
        M2ShareLimits.g_dwNpcLimit = 5;
        M2Config.nSendBlock = 4000;
        M2Config.nCheckBlock = 10000;
        M2Config.nAvailableBlock = 8000;
        M2Config.nGateLoad = 0;
        M2Config.boViewHackMessage = false;
        M2Config.boViewAdmissionFailure = false;
        M2Config.boSendCompressDataToRunGate = false;
        M2Config.boIsOldClient = true;
        M2Config.nMonGenRate = 10;
        M2Config.dwRegenMonstersTime = 200;
        M2Config.dwProcessMonstersTime = 250;
        M2Config.nProcessMonsterInterval = 3;
        RefShow();
    }

    public void EditProcessMonsterIntervalChange(object? sender)
    {
        if (!boOpened) return;
        M2Config.nProcessMonsterInterval = (uint)EditProcessMonsterInterval.Value;
        ModValue();
    }

    public void CheckBoxSendCompressDataToRunGateClick(object? sender)
    {
        if (!boOpened) return;
        M2Config.boSendCompressDataToRunGate = CheckBoxSendCompressDataToRunGate.Checked;
        ModValue();
    }
}

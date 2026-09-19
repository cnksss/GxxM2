using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>uFrmCombatPowerAddVar.pas TFrmCombatPowerAddVar 1:1（战力变量添加对话框）。</summary>
public sealed class CombatPowerAddVarForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.ComboBox cbbVarName = null!;
    public System.Windows.Forms.NumericUpDown seVarIndex = null!;
    public System.Windows.Forms.NumericUpDown seVarCount = null!;
    public System.Windows.Forms.CheckBox chkBatch = null!;
    public System.Windows.Forms.Button btnOK = null!;

    public CombatPowerPowerAddVarInit init = default;

    /// <summary>初始化参数接缝（Delphi ShowFrmCombatPowerAddVar 出参承载）。</summary>
    public record struct CombatPowerPowerAddVarInit(string VarName, bool IsBatch, int VarIndex, int VarCount);

    public CombatPowerAddVarForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "添加战力变量";
        Width = 360;
        Height = 280;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var lbVar = new System.Windows.Forms.Label { Text = "变量名:", Left = 14, Top = 14, AutoSize = true };
        cbbVarName = new System.Windows.Forms.ComboBox { Left = 90, Top = 10, Width = 200 };
        Controls.Add(lbVar);
        Controls.Add(cbbVarName);

        var lbIdx = new System.Windows.Forms.Label { Text = "起始序号:", Left = 14, Top = 44, AutoSize = true };
        seVarIndex = new System.Windows.Forms.NumericUpDown { Left = 90, Top = 40, Width = 90, Maximum = 9999 };
        Controls.Add(lbIdx);
        Controls.Add(seVarIndex);

        var lbCount = new System.Windows.Forms.Label { Text = "数量:", Left = 14, Top = 74, AutoSize = true };
        seVarCount = new System.Windows.Forms.NumericUpDown { Left = 90, Top = 70, Width = 90, Maximum = 9999 };
        Controls.Add(lbCount);
        Controls.Add(seVarCount);

        chkBatch = new System.Windows.Forms.CheckBox { Text = "批量添加", Left = 14, Top = 104, AutoSize = true };
        chkBatch.Click += (s, e) => chkBatchClick(s);
        Controls.Add(chkBatch);

        btnOK = new System.Windows.Forms.Button { Text = "确定(&O)", Left = 14, Top = 140, Width = 80, Height = 26 };
        btnOK.Click += (s, e) => btnOKClick(s);
        Controls.Add(btnOK);
    }

    // ================= Delphi 1:1 =================

    public void chkBatchClick(object? sender)
    {
        seVarIndex.Enabled = chkBatch.Checked;
        seVarCount.Enabled = chkBatch.Checked;
    }

    public bool btnOKClick(object? sender)
    {
        var varName = cbbVarName.Text.Trim();
        if (varName == "")
        {
            M2Forms.MessageBox("变量名不能为空", "提示", M2Forms.MB_OK);
            cbbVarName.Focus();
            return false;
        }

        if (!CombatPowerAddVar.CheckCombatPowerVarSupport(varName))
        {
            M2Forms.MessageBox("不支持的变量", "提示", M2Forms.MB_OK);
            cbbVarName.Focus();
            return false;
        }

        DialogResult = System.Windows.Forms.DialogResult.OK; // ModalResult := mrOk
        return true;
    }

    /// <summary>ShowFrmCombatPowerAddVar 1:1（出参经 Result 属性承载；showModal=false 供测试）。</summary>
    public bool ShowModal2(bool showModal = true)
    {
        if (showModal)
            ShowDialog();
        return DialogResult == System.Windows.Forms.DialogResult.OK;
    }

    public string OutVarName => cbbVarName.Text.Trim();
    public bool OutIsBatch => chkBatch.Checked;
    public int OutVarIndex => (int)seVarIndex.Value;
    public int OutVarCount => (int)seVarCount.Value;
}

/// <summary>ConfigMissionNpcPage.pas TFrmMissionNpcPageEditDlg 1:1（任务页面名单编辑：增删/上移/下移/保存/下发）。</summary>
public sealed class MissionNpcPageEditForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.ListBox ListBoxMissionPageCaptionList = null!;
    public System.Windows.Forms.TextBox EditMissionPage = null!;
    public System.Windows.Forms.Button ButtonEnablePickUpDelete = null!;
    public System.Windows.Forms.Button ButtonEnablePickUpSave = null!;
    public System.Windows.Forms.Button ButtonMissionNpcAdd = null!;
    public System.Windows.Forms.Button ButtonUp = null!;
    public System.Windows.Forms.Button ButtonDown = null!;
    public System.Windows.Forms.Button ButtonSendMissionNpc = null!;

    /// <summary>UserEngine.SendMissionNpc 接缝。</summary>
    public Action? SendMissionNpcHandler;

    public MissionNpcPageEditForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "任务页面编辑";
        Width = 460;
        Height = 480;

        ListBoxMissionPageCaptionList = new System.Windows.Forms.ListBox { Left = 8, Top = 8, Width = 260, Height = 360 };
        ListBoxMissionPageCaptionList.SelectedIndexChanged += (s, e) => ListBoxMissionPageCaptionListClick(s);
        Controls.Add(ListBoxMissionPageCaptionList);

        EditMissionPage = new System.Windows.Forms.TextBox { Left = 280, Top = 8, Width = 160 };
        Controls.Add(EditMissionPage);

        ButtonMissionNpcAdd = new System.Windows.Forms.Button { Text = "增加(&A)", Left = 280, Top = 40, Width = 80, Height = 26 };
        ButtonMissionNpcAdd.Click += (s, e) => ButtonMissionNpcAddClick(s);
        Controls.Add(ButtonMissionNpcAdd);

        ButtonEnablePickUpDelete = new System.Windows.Forms.Button { Text = "删除(&D)", Left = 280, Top = 74, Width = 80, Height = 26, Enabled = false };
        ButtonEnablePickUpDelete.Click += (s, e) => ButtonEnablePickUpDeleteClick(s);
        Controls.Add(ButtonEnablePickUpDelete);

        ButtonUp = new System.Windows.Forms.Button { Text = "上移(&U)", Left = 280, Top = 108, Width = 80, Height = 26 };
        ButtonUp.Click += (s, e) => ButtonUpClick(s);
        Controls.Add(ButtonUp);

        ButtonDown = new System.Windows.Forms.Button { Text = "下移(&N)", Left = 280, Top = 142, Width = 80, Height = 26 };
        ButtonDown.Click += (s, e) => ButtonDownClick(s);
        Controls.Add(ButtonDown);

        ButtonEnablePickUpSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 280, Top = 176, Width = 80, Height = 26, Enabled = false };
        ButtonEnablePickUpSave.Click += (s, e) => ButtonEnablePickUpSaveClick(s);
        Controls.Add(ButtonEnablePickUpSave);

        ButtonSendMissionNpc = new System.Windows.Forms.Button { Text = "下发(&T)", Left = 280, Top = 210, Width = 80, Height = 26 };
        ButtonSendMissionNpc.Click += (s, e) => ButtonSendMissionNpcClick(s);
        Controls.Add(ButtonSendMissionNpc);
    }

    // ================= Delphi 1:1 =================

    public void Open(bool showModal = true)
    {
        ListBoxMissionPageCaptionList.Items.AddRange(MissionPageState.g_MissionPageCaptionList.AsEnumerable().ToArray());
        if (showModal)
            ShowDialog();
    }

    public void ListBoxMissionPageCaptionListClick(object? sender)
    {
        if (ListBoxMissionPageCaptionList.SelectedIndex >= 0)
        {
            EditMissionPage.Text = ListBoxMissionPageCaptionList.Items[ListBoxMissionPageCaptionList.SelectedIndex]?.ToString() ?? "";
            ButtonEnablePickUpDelete.Enabled = true;
        }
        else
            ButtonEnablePickUpDelete.Enabled = false;
    }

    public void ButtonEnablePickUpDeleteClick(object? sender)
    {
        if (ListBoxMissionPageCaptionList.SelectedIndex >= 0)
        {
            ListBoxMissionPageCaptionList.Items.RemoveAt(ListBoxMissionPageCaptionList.SelectedIndex);
            ButtonEnablePickUpDelete.Enabled = false;
            ButtonEnablePickUpSave.Enabled = true;
        }
    }

    public (bool ok, string? msg) ButtonMissionNpcAddClick(object? sender)
    {
        var sCaption = EditMissionPage.Text.Trim();
        if (sCaption == "")
        {
            M2Forms.ErrorBox("请输入页面名称！");
            return (false, "请输入页面名称！");
        }
        if (MissionPageState.GetMissionPageCaption(sCaption))
        {
            M2Forms.ErrorBox("此页面名称已经在列表中了！");
            return (false, "此页面名称已经在列表中了！");
        }
        ListBoxMissionPageCaptionList.Items.Add(sCaption);
        ButtonEnablePickUpSave.Enabled = true;
        return (true, null);
    }

    public void ButtonEnablePickUpSaveClick(object? sender)
    {
        MissionPageState.g_MissionPageCaptionList.Clear();
        foreach (var item in ListBoxMissionPageCaptionList.Items)
            MissionPageState.g_MissionPageCaptionList.Add(item?.ToString() ?? "");
        MissionPageState.SaveMissionPageCaptionList();
        ButtonEnablePickUpSave.Enabled = false;
    }

    public void ButtonUpClick(object? sender)
    {
        var itemIndex = ListBoxMissionPageCaptionList.SelectedIndex;
        if (itemIndex > 0)
        {
            var sCaption = ListBoxMissionPageCaptionList.Items[itemIndex]?.ToString() ?? "";
            ListBoxMissionPageCaptionList.Items.RemoveAt(itemIndex);
            ListBoxMissionPageCaptionList.Items.Insert(itemIndex - 1, sCaption);
            ListBoxMissionPageCaptionList.SelectedIndex = itemIndex - 1;
            ButtonEnablePickUpSave.Enabled = true;
        }
    }

    public void ButtonDownClick(object? sender)
    {
        var itemIndex = ListBoxMissionPageCaptionList.SelectedIndex;
        if (itemIndex >= 0 && itemIndex < ListBoxMissionPageCaptionList.Items.Count - 1)
        {
            var sCaption = ListBoxMissionPageCaptionList.Items[itemIndex]?.ToString() ?? "";
            ListBoxMissionPageCaptionList.Items.RemoveAt(itemIndex);
            ListBoxMissionPageCaptionList.Items.Insert(itemIndex + 1, sCaption);
            ListBoxMissionPageCaptionList.SelectedIndex = itemIndex + 1;
            ButtonEnablePickUpSave.Enabled = true;
        }
    }

    public void ButtonSendMissionNpcClick(object? sender)
    {
        SendMissionNpcHandler?.Invoke(); // UserEngine.SendMissionNpc
    }
}

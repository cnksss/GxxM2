using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ClientModules.pas TftmClientModules 1:1 转换（客户端模块白/黑名单管理）。
/// ListView 对象挂载用 Tag 等效 Delphi SubItems.Objects[0]。
/// </summary>
public sealed class ClientModulesForm : System.Windows.Forms.Form
{
    private bool boOpened;

    public TModuleInfo? SelModuleInfo;

    public System.Windows.Forms.DataGridView ListViewClientModule = null!;
    public System.Windows.Forms.DataGridView ListViewClientBlackModule = null!;
    public System.Windows.Forms.TextBox EditMD5 = null!;
    public System.Windows.Forms.TextBox EditFileName = null!;
    public System.Windows.Forms.RadioButton RadioButtonWhiteModule = null!;
    public System.Windows.Forms.RadioButton RadioButtonBlackModule = null!;
    public System.Windows.Forms.CheckBox CheckBoxGetCheckModule = null!;
    public System.Windows.Forms.CheckBox CheckBoxClientAddModule = null!;
    public System.Windows.Forms.Button ButtonModuleAdd = null!;
    public System.Windows.Forms.Button ButtonModuleSave = null!;
    public System.Windows.Forms.Button ButtonModuleDel = null!;
    public System.Windows.Forms.Button ButtonLoad = null!;

    /// <summary>Delphi UserEngine.SendClientModules 接缝（引擎批次接入）。</summary>
    public Action? SendClientModulesHandler;

    public ClientModulesForm()
    {
        InitializeComponent();
    }

    private static System.Windows.Forms.DataGridView MakeGrid(params (string text, int width)[] cols)
    {
        var lv = new System.Windows.Forms.DataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        foreach (var (text, width) in cols)
            lv.Columns.Add("c" + lv.Columns.Count, text);
        return lv;
    }

    private void InitializeComponent()
    {
        Text = "客户端模块";
        Width = 620;
        Height = 480;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        ListViewClientModule = MakeGrid(("序号", 40), ("模式", 80), ("MD5", 220), ("文件名", 180));
        ListViewClientModule.Bounds = new System.Drawing.Rectangle(8, 8, 440, 160);
        ListViewClientModule.Click += (s, e) => ListViewClientModuleClick(s);
        Controls.Add(ListViewClientModule);

        ListViewClientBlackModule = MakeGrid(("序号", 40), ("模式", 80), ("MD5", 220), ("文件名", 180));
        ListViewClientBlackModule.Bounds = new System.Drawing.Rectangle(8, 174, 440, 160);
        ListViewClientBlackModule.Click += (s, e) => ListViewClientBlackModuleClick(s);
        Controls.Add(ListViewClientBlackModule);

        var lbMD5 = new System.Windows.Forms.Label { Text = "MD5:", Left = 8, Top = 342, AutoSize = true };
        EditMD5 = new System.Windows.Forms.TextBox { Left = 60, Top = 338, Width = 260 };
        var lbFile = new System.Windows.Forms.Label { Text = "文件名:", Left = 8, Top = 368, AutoSize = true };
        EditFileName = new System.Windows.Forms.TextBox { Left = 60, Top = 364, Width = 260 };
        Controls.Add(lbMD5);
        Controls.Add(EditMD5);
        Controls.Add(lbFile);
        Controls.Add(EditFileName);

        RadioButtonWhiteModule = new System.Windows.Forms.RadioButton { Text = "白名单", Left = 460, Top = 340, AutoSize = true, Checked = true };
        RadioButtonBlackModule = new System.Windows.Forms.RadioButton { Text = "黑名单", Left = 460, Top = 362, AutoSize = true };
        Controls.Add(RadioButtonWhiteModule);
        Controls.Add(RadioButtonBlackModule);

        CheckBoxGetCheckModule = new System.Windows.Forms.CheckBox { Text = "客户端校验模块", Left = 460, Top = 8, AutoSize = true };
        CheckBoxGetCheckModule.Click += (s, e) => CheckBoxGetCheckModuleClick(s);
        CheckBoxClientAddModule = new System.Windows.Forms.CheckBox { Text = "客户端添加模块", Left = 460, Top = 30, AutoSize = true };
        CheckBoxClientAddModule.Click += (s, e) => CheckBoxClientAddModuleClick(s);
        Controls.Add(CheckBoxGetCheckModule);
        Controls.Add(CheckBoxClientAddModule);

        ButtonModuleAdd = new System.Windows.Forms.Button { Text = "增加(&A)", Left = 460, Top = 390, Width = 70, Height = 26 };
        ButtonModuleAdd.Click += (s, e) => ButtonModuleAddClick(s);
        ButtonModuleSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 536, Top = 390, Width = 70, Height = 26, Enabled = false };
        ButtonModuleSave.Click += (s, e) => ButtonModuleSaveClick(s);
        ButtonModuleDel = new System.Windows.Forms.Button { Text = "删除(&D)", Left = 460, Top = 422, Width = 70, Height = 26, Enabled = false };
        ButtonModuleDel.Click += (s, e) => ButtonModuleDelClick(s);
        ButtonLoad = new System.Windows.Forms.Button { Text = "加载(&L)", Left = 536, Top = 422, Width = 70, Height = 26 };
        ButtonLoad.Click += (s, e) => ButtonLoadClick(s);
        Controls.Add(ButtonModuleAdd);
        Controls.Add(ButtonModuleSave);
        Controls.Add(ButtonModuleDel);
        Controls.Add(ButtonLoad);
    }

    // ================= Delphi 1:1 =================

    public void RefModuleList()
    {
        ListViewClientModule.Rows.Clear();
        for (int i = 0; i < ClientModuleState.g_ModuleList.Count; i++)
        {
            var moduleInfo = ClientModuleState.g_ModuleList[i];
            var item = new System.Windows.Forms.DataGridViewRow();
            item.CreateCells(ListViewClientModule);
            item.SetValues(i.ToString(), moduleInfo.boMode ? "远程添加" : "本地添加", moduleInfo.sMD5, moduleInfo.sFileName);
            item.Tag = moduleInfo;
            ListViewClientModule.Rows.Add(item);
        }

        ListViewClientBlackModule.Rows.Clear();
        for (int i = 0; i < ClientModuleState.g_BlackModuleList.Count; i++)
        {
            var moduleInfo = ClientModuleState.g_BlackModuleList[i];
            var item = new System.Windows.Forms.DataGridViewRow();
            item.CreateCells(ListViewClientBlackModule);
            item.SetValues(i.ToString(), moduleInfo.boMode ? "远程添加" : "本地添加", moduleInfo.sMD5, moduleInfo.sFileName);
            item.Tag = moduleInfo;
            ListViewClientBlackModule.Rows.Add(item);
        }
    }

    public void Open(bool showModal = true)
    {
        boOpened = false;
        CheckBoxGetCheckModule.Checked = M2Config.boClientCheckModule;
        CheckBoxClientAddModule.Checked = M2Config.boClientAddModule;
        RadioGroupModuleIndex = M2Config.boAddModuleList ? 1 : 0;

        ButtonModuleSave.Enabled = false;
        ButtonModuleDel.Enabled = false;
        SelModuleInfo = null;
        RefModuleList();
        boOpened = true;
        if (showModal)
            ShowDialog();
    }

    /// <summary>Delphi RadioGroupModule.ItemIndex（0=不添加 1=添加名单）。</summary>
    public int RadioGroupModuleIndex { get; set; }

    public (bool ok, string? msg) ButtonModuleAddClick(object? sender)
    {
        string sMD5 = EditMD5.Text.Trim().ToLower();
        string sFileName = EditFileName.Text.Trim();
        if (sMD5.Length != 32)
        {
            M2Forms.ErrorBox("请输入正确的MD5值！");
            return (false, "请输入正确的MD5值！");
        }

        if (RadioButtonWhiteModule.Checked)
        {
            foreach (var moduleInfo in ClientModuleState.g_ModuleList)
            {
                if (moduleInfo.sMD5 == sMD5)
                {
                    M2Forms.ErrorBox("你输入的MD5值已经在白名单！");
                    return (false, "你输入的MD5值已经在白名单！");
                }
            }
            for (int i = ClientModuleState.g_BlackModuleList.Count - 1; i >= 0; i--)
            {
                if (ClientModuleState.g_BlackModuleList[i].sMD5 == sMD5)
                    ClientModuleState.g_BlackModuleList.RemoveAt(i); // 白名单添加时从黑名单移除
            }
        }
        else
        {
            foreach (var moduleInfo in ClientModuleState.g_BlackModuleList)
            {
                if (moduleInfo.sMD5 == sMD5)
                {
                    M2Forms.ErrorBox("你输入的MD5值已经在黑名单！");
                    return (false, "你输入的MD5值已经在黑名单！");
                }
            }
            for (int i = ClientModuleState.g_ModuleList.Count - 1; i >= 0; i--)
            {
                if (ClientModuleState.g_ModuleList[i].sMD5 == sMD5)
                    ClientModuleState.g_ModuleList.RemoveAt(i);
            }
        }

        var info = new TModuleInfo { sMD5 = sMD5, sFileName = sFileName };
        if (RadioButtonWhiteModule.Checked)
            ClientModuleState.g_ModuleList.Add(info);
        else
            ClientModuleState.g_BlackModuleList.Add(info);

        RefModuleList();
        ButtonModuleSave.Enabled = true;
        return (true, null);
    }

    public void ButtonModuleSaveClick(object? sender)
    {
        ClientModuleState.SaveClientModules();
        ClientModuleState.SaveClientBlackModules();
        ButtonModuleSave.Enabled = false;
    }

    public void ListViewClientModuleClick(object? sender)
    {
        var row = ListViewClientModule.CurrentRow;
        if (row == null)
        {
            ButtonModuleDel.Enabled = false;
            SelModuleInfo = null;
            return;
        }
        SelModuleInfo = row.Tag as TModuleInfo;
        if (SelModuleInfo != null)
        {
            EditFileName.Text = SelModuleInfo.sFileName;
            EditMD5.Text = SelModuleInfo.sMD5;
        }
        ButtonModuleDel.Enabled = SelModuleInfo != null;
        RadioButtonWhiteModule.Checked = true;
    }

    public void ListViewClientBlackModuleClick(object? sender)
    {
        var row = ListViewClientBlackModule.CurrentRow;
        if (row == null)
        {
            ButtonModuleDel.Enabled = false;
            SelModuleInfo = null;
            return;
        }
        SelModuleInfo = row.Tag as TModuleInfo;
        if (SelModuleInfo != null)
        {
            EditFileName.Text = SelModuleInfo.sFileName;
            EditMD5.Text = SelModuleInfo.sMD5;
        }
        ButtonModuleDel.Enabled = SelModuleInfo != null;
        RadioButtonBlackModule.Checked = true;
    }

    public void ButtonModuleDelClick(object? sender)
    {
        bool boDelete = false;
        if (SelModuleInfo != null)
        {
            if (ClientModuleState.g_ModuleList.Remove(SelModuleInfo))
            {
                SelModuleInfo = null;
                boDelete = true;
            }
            else if (ClientModuleState.g_BlackModuleList.Remove(SelModuleInfo))
            {
                SelModuleInfo = null;
                boDelete = true;
            }
        }
        if (boDelete)
        {
            RefModuleList();
            ButtonModuleSave.Enabled = true;
        }
        ButtonModuleDel.Enabled = false;
    }

    public void ButtonLoadClick(object? sender)
    {
        ClientModuleState.LoadClientModules();
        ClientModuleState.LoadClientBlackModules();
        ButtonModuleSave.Enabled = false;
        ButtonModuleDel.Enabled = false;
        SelModuleInfo = null;
        RefModuleList();
    }

    public void CheckBoxGetCheckModuleClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boClientCheckModule = CheckBoxGetCheckModule.Checked;
        M2ShareState.ConfigIni.WriteBool("Setup", "ClientCheckModule", M2Config.boClientCheckModule);
        M2ShareState.ConfigIni.UpdateFile();
        SendClientModulesHandler?.Invoke(); // UserEngine.SendClientModules
    }

    public void CheckBoxClientAddModuleClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boClientAddModule = CheckBoxClientAddModule.Checked;
        M2ShareState.ConfigIni.WriteBool("Setup", "ClientAddModule", M2Config.boClientAddModule);
        M2ShareState.ConfigIni.UpdateFile();
        SendClientModulesHandler?.Invoke();
    }

    public void RadioGroupModuleClick(object? sender, int itemIndex)
    {
        if (!boOpened)
            return;
        RadioGroupModuleIndex = itemIndex;
        M2Config.boAddModuleList = itemIndex == 1;
        M2ShareState.ConfigIni.WriteBool("Setup", "AddModuleList", M2Config.boAddModuleList);
        M2ShareState.ConfigIni.UpdateFile();
    }
}

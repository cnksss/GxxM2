using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J12：ClientModules.pas + fTxtEditor.pas 核心转换测试。</summary>
public sealed class ClientModulesTests : IDisposable
{
    private readonly string _dir;
    private readonly ClientModulesForm _form;

    public ClientModulesTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j12_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        ClientModuleState.ResetForTests(_dir + "\\");
        M2ShareState.ResetForTests(_dir);
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() => new ClientModulesForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        ClientModuleState.ResetForTests(null);
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private static string Md5Of(int n) => (n.ToString("D32") + "0000000000000000000000000000")[..32];

    [StaFact]
    public void ModuleAdd_Md5LengthGuard()
    {
        _form.Open(showModal: false);
        _form.EditMD5.Text = "abc";
        var r = _form.ButtonModuleAddClick(_form);
        Assert.False(r.ok);
        Assert.Equal("请输入正确的MD5值！", r.msg);
    }

    [StaFact]
    public void ModuleAdd_WhiteList_DuplicateGuard_AndBlackRemoval()
    {
        _form.Open(showModal: false);
        // 先加黑名单
        _form.RadioButtonBlackModule.Checked = true;
        _form.EditMD5.Text = Md5Of(1).ToLower();
        Assert.True(_form.ButtonModuleAddClick(_form).ok);
        Assert.Single(ClientModuleState.g_BlackModuleList);

        // 切白名单加同一 MD5 → 从黑名单移除并进白名单
        _form.RadioButtonWhiteModule.Checked = true;
        Assert.True(_form.ButtonModuleAddClick(_form).ok);
        Assert.Empty(ClientModuleState.g_BlackModuleList);
        Assert.Single(ClientModuleState.g_ModuleList);

        // 重复加白名单 → 拒绝
        var r = _form.ButtonModuleAddClick(_form);
        Assert.False(r.ok);
        Assert.Equal("你输入的MD5值已经在白名单！", r.msg);
    }

    [StaFact]
    public void ModuleAdd_BlackList_DuplicateGuard()
    {
        _form.Open(showModal: false);
        _form.RadioButtonBlackModule.Checked = true;
        _form.EditMD5.Text = Md5Of(2).ToLower();
        Assert.True(_form.ButtonModuleAddClick(_form).ok);
        var r = _form.ButtonModuleAddClick(_form);
        Assert.False(r.ok);
        Assert.Equal("你输入的MD5值已经在黑名单！", r.msg);
    }

    [StaFact]
    public void SaveAndLoad_ModuleListTxt()
    {
        _form.Open(showModal: false);
        _form.EditMD5.Text = Md5Of(3).ToLower();
        _form.EditFileName.Text = "Module.dll";
        _form.RadioButtonWhiteModule.Checked = true;
        _form.ButtonModuleAddClick(_form);
        _form.ButtonModuleSaveClick(_form);

        Assert.True(File.Exists(Path.Combine(_dir, "ModuleList.txt")));
        Assert.True(ClientModuleState.g_ModuleListTextLen > 0);
        Assert.NotEmpty(ClientModuleState.g_ModuleListText);
        Assert.NotEqual(0u, ClientModuleState.g_ModuleListTextCRC);

        ClientModuleState.g_ModuleList.Clear();
        ClientModuleState.LoadClientModules();
        Assert.Single(ClientModuleState.g_ModuleList);
        Assert.Equal(Md5Of(3).ToLower(), ClientModuleState.g_ModuleList[0].sMD5);
        Assert.Equal("Module.dll", ClientModuleState.g_ModuleList[0].sFileName);
    }

    [StaFact]
    public void CheckModuleToggle_WritesIni_AndInvokesSend()
    {
        int sendCount = 0;
        _form.SendClientModulesHandler = () => sendCount++;
        _form.Open(showModal: false);
        _form.CheckBoxGetCheckModule.Checked = true;
        _form.CheckBoxGetCheckModuleClick(_form);
        Assert.True(M2Config.boClientCheckModule);
        Assert.Equal(1, sendCount);

        _form.CheckBoxClientAddModule.Checked = true;
        _form.CheckBoxClientAddModuleClick(_form);
        Assert.True(M2Config.boClientAddModule);
        Assert.Equal(2, sendCount);
    }
}

/// <summary>批次J12：fTxtEditor.pas TfrmTXTEditor 核心转换测试。</summary>
public sealed class TxtEditorTests : IDisposable
{
    private readonly string _dir;

    public TxtEditorTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j12b_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        Directory.CreateDirectory(Path.Combine(_dir, "Envir", "Market_Def"));
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private TxtEditorForm NewForm()
    {
        var f = StaRunner.New(() => new TxtEditorForm { AppDir = _dir });
        return f;
    }

    [StaTheory]
    [InlineData("QManage", "MapQuest_def\\")]
    [InlineData("RobotManage", "Robot_def\\")]
    [InlineData("MerchantA", "Market_Def\\")]
    public void GetMerchantScriptFile_RoutesByCharName(string charName, string expected)
    {
        Assert.Equal("D:\\\\Envir\\" + expected, TxtEditorForm.GetMerchantScriptFile(charName, "D:\\\\Envir\\"));
    }

    [StaFact]
    public void LoadScriptTxt_MissingFile_ShowsStatus()
    {
        var form = NewForm();
        try
        {
            Assert.False(form.LoadScriptTxt(Path.Combine(_dir, "no.txt")));
            Assert.Contains("[文件不存在]", form.statPanel.Text);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void LoadScriptTxt_LoadsAndAddsHistory()
    {
        var script = Path.Combine(_dir, "Envir", "Market_Def", "MerchantA-3.txt");
        File.WriteAllText(script, "[test]#SAY你好", System.Text.Encoding.UTF8);
        var form = NewForm();
        try
        {
            Assert.True(form.LoadScriptTxt(script));
            Assert.Equal("[test]#SAY你好", form.Editor.Text);
            Assert.Equal(1, form.lstHistory.Items.Count);
            Assert.Equal(" 脚本路径：" + script, form.statPanel.Text);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void LoadScriptTxt_ModifiedCancelPreventsLoad()
    {
        var first = Path.Combine(_dir, "a.txt");
        var second = Path.Combine(_dir, "b.txt");
        File.WriteAllText(first, "AAA", System.Text.Encoding.UTF8);
        File.WriteAllText(second, "BBB", System.Text.Encoding.UTF8);
        var form = NewForm();
        try
        {
            form.LoadScriptTxt(first);
            form.SetModified(true);
            form.LoadScriptTxt(second, yesNoCancel: M2Forms.IDCANCEL);
            Assert.Equal("", form.Editor.Text); // 取消 → Delphi Clear 先于弹窗（1:1）
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void AddHistory_DedupesAndMovesToTop()
    {
        var f1 = Path.Combine(_dir, "1.txt");
        var f2 = Path.Combine(_dir, "2.txt");
        File.WriteAllText(f1, "1", System.Text.Encoding.UTF8);
        File.WriteAllText(f2, "2", System.Text.Encoding.UTF8);
        var form = NewForm();
        try
        {
            form.LoadScriptTxt(f1);
            form.LoadScriptTxt(f2);
            form.LoadScriptTxt(f1); // 再次打开 → 置顶去重
            Assert.Equal(2, form.lstHistory.Items.Count);
            Assert.Equal("1.txt", ((THistory)form.lstHistory.Items[0]!).FileName);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void SaveAndLoad_HistoryPersists()
    {
        var f1 = Path.Combine(_dir, "x.txt");
        File.WriteAllText(f1, "X", System.Text.Encoding.UTF8);
        var form = NewForm();
        try
        {
            form.LoadScriptTxt(f1);
            form.SaveHistory();
        }
        finally { StaRunner.New(() => form.Dispose()); }

        var form2 = NewForm();
        try
        {
            form2.LoadHistory();
            Assert.Equal(1, form2.lstHistory.Items.Count);
        }
        finally { StaRunner.New(() => form2.Dispose()); }
    }

    [StaFact]
    public void FormCloseQuery_ModifiedYesSaves()
    {
        var f1 = Path.Combine(_dir, "y.txt");
        File.WriteAllText(f1, "OLD", System.Text.Encoding.UTF8);
        var form = NewForm();
        try
        {
            form.LoadScriptTxt(f1);
            form.Editor.Text = "NEW";
            form.SetModified(true);
            Assert.True(form.FormCloseQuery(M2Forms.IDYES));
            Assert.Equal("NEW", File.ReadAllText(f1, System.Text.Encoding.UTF8));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void FormCloseQuery_ModifiedCancelBlocks()
    {
        var f1 = Path.Combine(_dir, "z.txt");
        File.WriteAllText(f1, "OLD", System.Text.Encoding.UTF8);
        var form = NewForm();
        try
        {
            form.LoadScriptTxt(f1);
            form.SetModified(true);
            Assert.False(form.FormCloseQuery(M2Forms.IDCANCEL));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

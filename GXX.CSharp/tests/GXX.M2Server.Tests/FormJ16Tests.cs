using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J16：uFrmCombatPowerAddVar + ConfigMissionNpcPage 移植测试。</summary>
public sealed class CombatPowerVarTests
{
    [Theory]
    [InlineData("D9", true)]
    [InlineData("M1", true)]
    [InlineData("N2", true)]
    [InlineData("U3", true)]
    [InlineData("J4", true)]
    [InlineData("X5", false)]
    [InlineData("", false)]
    [InlineData("N$VAR", true)]  // 前缀 N$ 且长度 > 2
    [InlineData("N$", true)] // 首字符 N 已命中    // 长度不大于 2
    [InlineData("dv", true)]     // 小写转大写
    public void CheckCombatPowerVarSupport_AllBranches(string varName, bool expected)
    {
        Assert.Equal(expected, CombatPowerAddVar.CheckCombatPowerVarSupport(varName));
    }

    [Fact]
    public void BtnOK_EmptyName_Rejected()
    {
        var form = StaRunner.New(() => new CombatPowerAddVarForm());
        try
        {
            form.cbbVarName.Text = "";
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
            Assert.False(form.btnOKClick(form));
            Assert.Equal("变量名不能为空", M2Forms.LastMessage);
        }
        finally
        {
            M2Forms.MessageBoxHandler = null;
            StaRunner.New(() => form.Dispose());
        }
    }

    [Fact]
    public void BtnOK_UnsupportedVar_Rejected()
    {
        var form = StaRunner.New(() => new CombatPowerAddVarForm());
        try
        {
            form.cbbVarName.Text = "X1";
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
            Assert.False(form.btnOKClick(form));
            Assert.Equal("不支持的变量", M2Forms.LastMessage);
        }
        finally
        {
            M2Forms.MessageBoxHandler = null;
            StaRunner.New(() => form.Dispose());
        }
    }

    [Fact]
    public void BtnOK_SupportedVar_AcceptedAndOutParams()
    {
        var form = StaRunner.New(() => new CombatPowerAddVarForm());
        try
        {
            form.cbbVarName.Text = " D9 ";
            form.chkBatch.Checked = true;
            form.chkBatchClick(form);
            form.seVarIndex.Value = 3;
            form.seVarCount.Value = 10;
            Assert.True(form.btnOKClick(form));
            Assert.True(form.ShowModal2(showModal: false)); // ModalResult = mrOk
            Assert.Equal("D9", form.OutVarName);            // Trim 出参
            Assert.True(form.OutIsBatch);
            Assert.Equal(3, form.OutVarIndex);
            Assert.Equal(10, form.OutVarCount);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void ChkBatch_TogglesVarEdits()
    {
        var form = StaRunner.New(() => new CombatPowerAddVarForm());
        try
        {
            form.chkBatch.Checked = true;
            form.chkBatchClick(form);
            Assert.True(form.seVarIndex.Enabled);
            Assert.True(form.seVarCount.Enabled);
            form.chkBatch.Checked = false;
            form.chkBatchClick(form);
            Assert.False(form.seVarIndex.Enabled);
            Assert.False(form.seVarCount.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

/// <summary>批次J16：ConfigMissionNpcPage.pas 移植测试（任务页面名单编辑全处理器）。</summary>
public sealed class MissionNpcPageTests : IDisposable
{
    private readonly string _dir;

    public MissionNpcPageTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j16_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        MissionPageState.ResetForTests(_dir + "\\");
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        MissionPageState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [StaFact]
    public void Open_LoadsFromGlobalList()
    {
        MissionPageState.g_MissionPageCaptionList.Add("主页面");
        MissionPageState.g_MissionPageCaptionList.Add("支线页面");
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.Open(showModal: false);
            Assert.Equal(2, form.ListBoxMissionPageCaptionList.Items.Count);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void Add_EmptyCaption_Rejected()
    {
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.EditMissionPage.Text = "";
            var (ok, msg) = form.ButtonMissionNpcAddClick(form);
            Assert.False(ok);
            Assert.Equal("请输入页面名称！", msg);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void Add_DuplicateCaption_Rejected()
    {
        MissionPageState.g_MissionPageCaptionList.Add("主页面");
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.EditMissionPage.Text = "主页 面".Replace(" ", ""); // 主页面
            var (ok, msg) = form.ButtonMissionNpcAddClick(form);
            Assert.False(ok);
            Assert.Equal("此页面名称已经在列表中了！", msg);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void Add_Valid_CaseInsensitiveDuplicate()
    {
        MissionPageState.g_MissionPageCaptionList.Add("Main");
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.EditMissionPage.Text = "MAIN"; // CompareText 忽略大小写
            var (ok, msg) = form.ButtonMissionNpcAddClick(form);
            Assert.False(ok);
            Assert.Equal("此页面名称已经在列表中了！", msg);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void AddAndSave_PersistsToFile()
    {
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.EditMissionPage.Text = "新页面";
            Assert.True(form.ButtonMissionNpcAddClick(form).ok);
            form.ButtonEnablePickUpSaveClick(form);
            Assert.True(MissionPageState.g_sSendPageCaptionText.Contains("新页面"));
            Assert.True(File.Exists(MissionPageState.ListPath(_dir + "\\")));
            Assert.True(MissionPageState.LoadMissionPageCaptionList());
            Assert.Contains("新页面", MissionPageState.g_MissionPageCaptionList.AsEnumerable());
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void UpDownMove_Reorders()
    {
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.ListBoxMissionPageCaptionList.Items.AddRange(new object[] { "A", "B", "C" });
            form.ListBoxMissionPageCaptionList.SelectedIndex = 2;
            form.ButtonUpClick(form);
            Assert.Equal("C", form.ListBoxMissionPageCaptionList.Items[1]);
            form.ButtonDownClick(form);
            Assert.Equal("C", form.ListBoxMissionPageCaptionList.Items[2]);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void Delete_DisablesAndEnablesSave()
    {
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            form.ListBoxMissionPageCaptionList.Items.Add("唯一");
            form.ListBoxMissionPageCaptionList.SelectedIndex = 0;
            form.ListBoxMissionPageCaptionListClick(form);
            Assert.True(form.ButtonEnablePickUpDelete.Enabled);
            form.ButtonEnablePickUpDeleteClick(form);
            Assert.Equal(0, form.ListBoxMissionPageCaptionList.Items.Count);
            Assert.False(form.ButtonEnablePickUpDelete.Enabled);
            Assert.True(form.ButtonEnablePickUpSave.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void SendMissionNpc_HandlerInvoked()
    {
        var form = StaRunner.New(() => new MissionNpcPageEditForm());
        try
        {
            int called = 0;
            form.SendMissionNpcHandler = () => called++;
            form.ButtonSendMissionNpcClick(form);
            Assert.Equal(1, called);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

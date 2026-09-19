using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J32：GameCommand.pas TfrmGameCmd 命令配置窗体（用户命令页）1:1 测试。</summary>
public sealed class GameCommandFormTests : IDisposable
{
    private readonly string _dir;
    private readonly GameCommandForm _form;

    public GameCommandFormTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j32_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        GameCommandState.ResetForTests();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() => new GameCommandForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void Open_BuildsUserCommandList_WithGroupsAndDefaults()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);

            Assert.Equal(6, _form.UserCommandRows.Count(r => r.IsGroup));
            Assert.Equal("聊天及信息", _form.UserCommandRows[0].GroupText); // 首行组标题
            Assert.Equal(45, _form.UserCommandRows.Count(r => !r.IsGroup)); // 45 条命令
            Assert.Equal(1, _form.UserCommandRows[1].Index); // 命令行序号从 1 递增
            Assert.Equal("禁止指定人物发的私聊信息", _form.UserCommandRows[1].Desc);
            Assert.Equal("人物名称", _form.UserCommandRows[1].Param);
            Assert.Equal("PrvMsg", _form.UserCommandRows[1].Cmd!.sCmd);
            // 列表框含组标题行与命令行
            Assert.Contains("聊天及信息", _form.ListBoxUserCmd.Items.Cast<string>());
            Assert.Contains(_form.UserCommandRows[1].Cmd!.sCmd + "(PrvMsg) 人物名称 禁止指定人物发的私聊信息",
                _form.ListBoxUserCmd.Items.Cast<string>());
            Assert.False(_form.ButtonUserCmdOK.Enabled);
            Assert.False(_form.ButtonUserCmdSave.Enabled);
        });
    }

    [Fact]
    public void RegistryDefaults_MatchTypedConstants()
    {
        StaRunner.New(() =>
        {
            Assert.Equal("Date", GameCommandState.g_GameCommand.Data.sCmd);
            Assert.Equal("PrvMsg", GameCommandState.g_GameCommand.PRVMSG.sCmd);
            Assert.Equal("联盟", GameCommandState.g_GameCommand.AUTH.sCmd);
            Assert.Equal("取消联盟", GameCommandState.g_GameCommand.AUTHCANCEL.sCmd);
            Assert.Equal("骑马", GameCommandState.g_GameCommand.TAKEONHORSE.sCmd);
            Assert.Equal("下马", GameCommandState.g_GameCommand.TAKEOFHORSE.sCmd);
            Assert.Equal("禁止邀请上马", GameCommandState.g_GameCommand.DISABLEHORSEINVITE.sCmd);
            Assert.Equal("角色交易", GameCommandState.g_GameCommand.OpenSellPlayer.sCmd);
            Assert.Equal("加入国家", GameCommandState.g_GameCommand.LETNATION.sCmd);
            Assert.Equal("允许国家聊天", GameCommandState.g_GameCommand.BANNATIONCHAT.sCmd);
            Assert.Equal(0, GameCommandState.g_GameCommand.PRVMSG.nPermissionMin);
            Assert.Equal(10, GameCommandState.g_GameCommand.PRVMSG.nPermissionMax);
        });
    }

    [Fact]
    public void UserCmdOK_EmptyName_Rejected()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ListBoxUserCmd.SelectedIndex = 1; // 第一条命令（PRVMSG）
            _form.edtUserCmdName.Text = "";
            M2Forms.NextAnswer = M2Forms.IDOK;
            _form.btnUserCmdOKClick(_form);
            Assert.Equal("命令名称不能为空！", M2Forms.LastMessage);
            Assert.Equal("PrvMsg", GameCommandState.g_GameCommand.PRVMSG.sCmd); // 未写入
        });
    }

    [Fact]
    public void UserCmdOK_WritesCommandAndPermission()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ListBoxUserCmd.SelectedIndex = 1;
            _form.edtUserCmdName.Text = "禁止私聊";
            _form.seUserCmdPermission.Value = 3;
            _form.btnUserCmdOKClick(_form);
            Assert.Equal("禁止私聊", GameCommandState.g_GameCommand.PRVMSG.sCmd);
            Assert.Equal(3, GameCommandState.g_GameCommand.PRVMSG.nPermissionMin);
        });
    }

    [Fact]
    public void UserCmdSave_WritesCommandAndPermissionSections()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            GameCommandState.g_GameCommand.Data.sCmd = "ServDate";
            GameCommandState.g_GameCommand.Data.nPermissionMin = 2;
            _form.ButtonUserCmdSaveClick(_form);
            Assert.Equal("ServDate", M2ShareState.CommandConfIni.ReadString("Command", "Date", ""));
            Assert.Equal("PrvMsg", M2ShareState.CommandConfIni.ReadString("Command", "PrvMsg", ""));
            Assert.Equal("PublicChat", M2ShareState.CommandConfIni.ReadString("Command", "PublicChat", ""));
            Assert.Equal("角色交易", M2ShareState.CommandConfIni.ReadString("Command", "OpenSellPlayer", ""));
            Assert.Equal(2, M2ShareState.CommandConfIni.ReadInteger("Permission", "Date", -1));
            Assert.Equal(0, M2ShareState.CommandConfIni.ReadInteger("Permission", "PrvMsg", -1));
            Assert.Equal(0, M2ShareState.CommandConfIni.ReadInteger("Permission", "AllowMsg", -1));
            Assert.False(_form.ButtonUserCmdSave.Enabled);
        });
    }
}

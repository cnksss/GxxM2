using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J33：GameCommand.pas 管理员/调试命令两表与保存 1:1 测试（巨片收尾）。</summary>
public sealed class GameCommandAdminDebugTests : IDisposable
{
    private readonly string _dir;
    private readonly GameCommandForm _form;

    public GameCommandAdminDebugTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j33_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        GameCommandState.ResetForTests();
        GameCommandState.ResetAdminDebugForTests();
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
    public void Open_BuildsAdminAndDebugLists()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Assert.Equal(105, _form.AdminCommandRows.Count(r => !r.IsGroup)); // 管理员命令 105 条
            Assert.True(_form.AdminCommandRows[0].IsPermission);             // GAMEMASTER 权限列 √
            Assert.Equal("进入/退出管理员模式(进入模式后不会受到任何角色攻击)", _form.AdminCommandRows[0].Desc);
            Assert.Equal(40, _form.DebugCommandRows.Count(r => !r.IsGroup)); // 调试命令 40 条
            Assert.Equal("重新加载管理员列表", _form.DebugCommandRows[0].Desc);
            Assert.False(_form.ButtonAdminCmdOK.Enabled);
        });
    }

    [Fact]
    public void RegistryAdminDebugDefaults()
    {
        StaRunner.New(() =>
        {
            var admin = GameCommandState.AdminCmdByName;
            var debug = GameCommandState.DebugCmdByName;
            Assert.Equal("GameMaster", admin["GAMEMASTER"].sCmd);
            Assert.Equal(10, admin["GAMEMASTER"].nPermissionMin);
            Assert.Equal("Observer", admin["OBSERVER"].sCmd);
            Assert.Equal("Superman", admin["SUEPRMAN"].sCmd);
            Assert.Equal("Kick", admin["KICK"].sCmd);
            Assert.Equal(10, admin["KICK"].nPermissionMin);
            Assert.Equal("Ting", admin["TING"].sCmd);
            Assert.Equal("Recall", admin["RECALL"].sCmd);
            Assert.Equal("ReGoto", admin["REGOTO"].sCmd);
            Assert.Equal("Level", admin["Level"].sCmd);
            Assert.Equal("HumanLocal", admin["HUMANLOCAL"].sCmd);
            Assert.Equal(3, admin["HUMANLOCAL"].nPermissionMin);
            Assert.Equal("Move", admin["Move"].sCmd);
            Assert.Equal(3, admin["Move"].nPermissionMin);
            Assert.Equal(6, admin["Move"].nPermissionMax);
            Assert.Equal("PositionMove", admin["POSITIONMOVE"].sCmd);
            Assert.Equal(3, admin["POSITIONMOVE"].nPermissionMin);
            Assert.Equal(6, admin["POSITIONMOVE"].nPermissionMax);
            Assert.Equal("Who", admin["WHO"].sCmd);
            Assert.Equal("Total", admin["TOTAL"].sCmd);
            Assert.Equal("Make", admin["MAKE"].sCmd);
            Assert.Equal("showflag", debug["SHOWFLAG"].sCmd); // 原文小写
            Assert.Equal("showopen", debug["SHOWOPEN"].sCmd);
            Assert.Equal("showunit", debug["SHOWUNIT"].sCmd);
            Assert.Equal("MobNpc", debug["MOBNPC"].sCmd);
            Assert.Equal("DelNpc", debug["DELNPC"].sCmd);
            Assert.Equal("ClrPassword", admin["CLRPASSWORD"].sCmd);
            Assert.Equal(10, admin["CLRPASSWORD"].nPermissionMin);
        });
    }

    [Fact]
    public void AdminCmdOK_WritesRegistry()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ListBoxAdminCmd.SelectedIndex = 0; // GAMEMASTER
            _form.edtAdminCmdName.Text = "管理员模式";
            _form.seAdminCmdPermission.Value = 8;
            _form.btnAdminCmdOKClick(_form);
            Assert.Equal("管理员模式", GameCommandState.AdminCmdByName["GAMEMASTER"].sCmd);
            Assert.Equal(8, GameCommandState.AdminCmdByName["GAMEMASTER"].nPermissionMin);
        });
    }

    [Fact]
    public void AdminCmdSave_WritesCommandAndPermissionSections()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            GameCommandState.AdminCmdByName["KICK"].nPermissionMin = 9;
            _form.ButtonAdminCmdSaveClick(_form);
            Assert.Equal("Observer", M2ShareState.CommandConfIni.ReadString("Command", "ObServer", ""));
            Assert.Equal("GameMaster", M2ShareState.CommandConfIni.ReadString("Command", "GameMaster", ""));
            Assert.Equal("Superman", M2ShareState.ConfigIni.ReadString("Setup", "__none", "__none") == "__none" ? M2ShareState.CommandConfIni.ReadString("Command", "SuperMan", "") : "");
            Assert.Equal("ClrPassword", M2ShareState.CommandConfIni.ReadString("Command", "StorageClearPassword", ""));
            Assert.Equal("Make", M2ShareState.CommandConfIni.ReadString("Command", "Make", ""));
            Assert.Equal("Spirit", M2ShareState.CommandConfIni.ReadString("Command", "SpiritStart", ""));
            Assert.Equal("DelSellPlayer", M2ShareState.CommandConfIni.ReadString("Command", "DelSellPlayer", ""));
            Assert.Equal(10, M2ShareState.CommandConfIni.ReadInteger("Permission", "GameMaster", -1));
            Assert.Equal(9, M2ShareState.CommandConfIni.ReadInteger("Permission", "Kick", -1));
            // Make 权限双键原文（Min/Max 分写）
            Assert.Equal(0, M2ShareState.CommandConfIni.ReadInteger("Permission", "MakeMin", -1));
            Assert.Equal(10, M2ShareState.CommandConfIni.ReadInteger("Permission", "MakeMax", -1));
            Assert.Equal(3, M2ShareState.CommandConfIni.ReadInteger("Permission", "PositionMoveMin", -1));
            Assert.Equal(6, M2ShareState.CommandConfIni.ReadInteger("Permission", "PositionMoveMax", -1));
            Assert.False(_form.ButtonAdminCmdSave.Enabled);
        });
    }

    [Fact]
    public void DebugCmdSave_WritesKeys()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            GameCommandState.DebugCmdByName["RELOADADMIN"].sCmd = "ReloadAdminList";
            _form.ButtonDebugCmdSaveClick(_form);
            Assert.Equal("showflag", M2ShareState.CommandConfIni.ReadString("Command", "SHOWFLAG", ""));
            Assert.Equal("ReloadAdminList", M2ShareState.CommandConfIni.ReadString("Command", "RELOADADMIN", ""));
            Assert.Equal("ReLoadNpc", M2ShareState.CommandConfIni.ReadString("Command", "ReLoadNpc", ""));
            Assert.Equal("Reloadabuse", M2ShareState.CommandConfIni.ReadString("Command", "RELOADABUSE", ""));
            Assert.False(_form.ButtonDebugCmdSave.Enabled);
        });
    }
}

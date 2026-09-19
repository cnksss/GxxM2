using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;
using static GXX.M2Server.Engine.M2ShareState;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J1：GeneralConfig.pas（TfrmGeneralConfig）1:1 转换测试。
/// 覆盖三组保存按钮的全部校验分支（Delphi 顺序 + 原文消息）、g_Config 赋值与 INI 持久化、
/// 测试模式联动、BDE/Sqlite 单选联动、修改状态标记（ModValue/uModValue）。
/// </summary>
public sealed class FormGeneralConfigTests : IDisposable
{
    private readonly string _dir;
    private readonly GeneralConfigForm _form;

    public FormGeneralConfigTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j1_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        ResetForTests(_dir);
        M2Config.ResetGeneralDefaults();
        nServerIndex = 7;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK; // 阻断真实弹窗（测试进程无交互面）
        _form = StaRunner.New(() => new GeneralConfigForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private string IniPath => Path.Combine(_dir, "!Setup.txt");

    // ---------- 网络设置（ButtonNetWorkSaveClick） ----------

    [StaFact]
    public void Network_InvalidGateAddr_ShowsExactMessage()
    {
        _form.FillNetworkTabValid();
        _form.EditGateAddr.Text = "not_an_ip";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditGateAddr", r.focus);
        Assert.Equal("网关地址设置错误！", M2Forms.LastMessage);
        Assert.Equal("错误信息", M2Forms.LastCaption);
    }

    [StaFact]
    public void Network_InvalidGatePort_OutOfRange()
    {
        _form.FillNetworkTabValid();
        _form.EditGatePort.Text = "70000";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditGatePort", r.focus);
        Assert.Equal("网关端口设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidGatePort_NonNumberParsesAsMinusOne()
    {
        _form.FillNetworkTabValid();
        _form.EditGatePort.Text = "abc";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("网关端口设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidIDSAddr()
    {
        _form.FillNetworkTabValid();
        _form.EditIDSAddr.Text = "1.2.3";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditIDSAddr", r.focus);
        Assert.Equal("管理服务器地址设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidIDSPort()
    {
        _form.FillNetworkTabValid();
        _form.EditIDSPort.Text = "-1";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditIDSPort", r.focus);
        Assert.Equal("管理服务器端口设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidDBAddr()
    {
        _form.FillNetworkTabValid();
        _form.EditDBAddr.Text = "bad";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditDBAddr", r.focus);
        Assert.Equal("数据库服务器地址设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidDBPort()
    {
        _form.FillNetworkTabValid();
        _form.EditDBPort.Text = "99999";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditDBPort", r.focus);
        Assert.Equal("数据库服务器端口设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidLogServerAddr()
    {
        _form.FillNetworkTabValid();
        _form.EditLogServerAddr.Text = "x";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditLogServerAddr", r.focus);
        Assert.Equal("日志服务器地址设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidLogServerPort()
    {
        _form.FillNetworkTabValid();
        _form.EditLogServerPort.Text = "65536";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditLogServerPort", r.focus);
        Assert.Equal("日志服务器端口设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidMsgSrvAddr()
    {
        _form.FillNetworkTabValid();
        _form.EditMsgSrvAddr.Text = "1..1";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditMsgSrvAddr", r.focus);
        Assert.Equal("游戏主服务器地址设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_InvalidMsgSrvPort()
    {
        _form.FillNetworkTabValid();
        _form.EditMsgSrvPort.Text = "-2";
        var r = _form.ButtonNetWorkSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditMsgSrvPort", r.focus);
        Assert.Equal("游戏主服务器端口设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Network_ValidSave_UpdatesConfigAndIni()
    {
        _form.FillNetworkTabValid(); // 192.168.1.10:7100 等合法值
        var r = _form.ButtonNetWorkSaveClick();
        Assert.True(r.ok);
        Assert.Equal("192.168.1.10", M2Config.sGateAddr);
        Assert.Equal(7100, M2Config.nGatePort);
        Assert.Equal("192.168.1.11", M2Config.sIDSAddr);
        Assert.Equal(7200, M2Config.nIDSPort);
        Assert.Equal("192.168.1.12", M2Config.sDBAddr);
        Assert.Equal(7300, M2Config.nDBPort);
        Assert.Equal("192.168.1.13", M2Config.sLogServerAddr);
        Assert.Equal(7400, M2Config.nLogServerPort);
        Assert.Equal("192.168.1.14", M2Config.sMsgSrvAddr);
        Assert.Equal(7500, M2Config.nMsgSrvPort);

        var ini = new TFastIniFile(IniPath);
        Assert.Equal("192.168.1.10", ini.ReadString("Server", "GateAddr", ""));
        Assert.Equal(7100, ini.ReadInteger("Server", "GatePort", 0));
        Assert.Equal("192.168.1.14", ini.ReadString("Server", "MsgSrvAddr", ""));
        Assert.Equal(7500, ini.ReadInteger("Server", "MsgSrvPort", 0));
    }

    // ---------- 游戏设置（ButtonServerInfoSaveClick） ----------

    [StaFact]
    public void ServerInfo_EmptyGameName()
    {
        _form.FillServerInfoTabValid();
        _form.EditGameName.Text = "  ";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditGameName", r.focus);
        Assert.Equal("游戏名称设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_ServerIndexRange_Low()
    {
        _form.FillServerInfoTabValid();
        _form.EditServerIndex.Text = "-1";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditServerIndex", r.focus);
        Assert.Equal("服务器号设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_ServerIndexRange_High()
    {
        _form.FillServerInfoTabValid();
        _form.EditServerIndex.Text = "256";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("服务器号设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_ServerNumberRange()
    {
        _form.FillServerInfoTabValid();
        _form.EditServerNumber.Text = "300";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditServerNumber", r.focus);
        Assert.Equal("服务器数设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_TestLevelRange()
    {
        _form.FillServerInfoTabValid();
        _form.EditTestLevel.Text = "65536";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditTestLevel", r.focus);
        Assert.Equal("开始等级设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_TestGoldRange_HighIntegerDiv2()
    {
        _form.FillServerInfoTabValid();
        _form.EditTestGold.Text = "1073741824"; // High(Integer) div 2 = 1073741823
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditTestGold", r.focus);
        Assert.Equal("开始金币设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_TestUserLimitRange()
    {
        _form.FillServerInfoTabValid();
        _form.EditTestUserLimit.Text = "10001";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditTestUserLimit", r.focus);
        Assert.Equal("测试人数设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_UserFullRange()
    {
        _form.FillServerInfoTabValid();
        _form.EditUserFull.Text = "-5";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditUserFull", r.focus);
        Assert.Equal("上限人数设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_BdeRequiresDbName()
    {
        _form.FillServerInfoTabValid();
        _form.rbBDE.Checked = true;
        _form.rbBDEClick(_form);
        _form.EditDBName.Text = "";
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditDBName", r.focus);
        Assert.Equal("数据库名称设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_SqliteRequiresExistingFile()
    {
        _form.FillServerInfoTabValid();
        _form.rbSqlite.Checked = true;
        _form.rbSqliteClick(_form);
        _form.edtSqlite.Text = Path.Combine(_dir, "no_such.db");
        var r = _form.ButtonServerInfoSaveClick();
        Assert.False(r.ok);
        Assert.Equal("edtSqlite", r.focus);
        Assert.Equal("数据库文件设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void ServerInfo_ValidSave_UpdatesGlobalsConfigAndIni()
    {
        _form.FillServerInfoTabValid();
        _form.CheckBoxTestServer.Checked = true;
        _form.CheckBoxServiceMode.Checked = true;
        _form.chkShowBlockIPLog.Checked = true;
        _form.rbBDE.Checked = true;
        var r = _form.ButtonServerInfoSaveClick();
        Assert.True(r.ok);

        Assert.Equal("测试服", M2Config.sServerName);
        Assert.Equal(1, M2Config.nServerNumber);
        Assert.True(M2Config.boServiceMode);
        Assert.True(M2Config.boTestServer);
        Assert.Equal(60, M2Config.nTestLevel);
        Assert.Equal(100000, M2Config.nTestGold);
        Assert.Equal(500, M2Config.nTestUserLimit);
        Assert.Equal(2000, M2Config.nUserFull);
        Assert.False(g_boUseSqliteDB);
        Assert.Equal("HeroDB", g_sDBName);
        Assert.True(g_boShowBlockIPLog);

        var ini = new TFastIniFile(IniPath);
        Assert.Equal("测试服", ini.ReadString("Server", "ServerName", ""));
        Assert.Equal(7, ini.ReadInteger("Server", "ServerIndex", -1)); // 写入全局 nServerIndex
        Assert.Equal("-1", ini.ReadString("Server", "TestServer", "")); // Delphi BoolToStr 默认 '-1'
        Assert.Equal(60, ini.ReadInteger("Server", "TestLevel", 0));
        Assert.Equal(100000, ini.ReadInteger("Server", "TestGold", 0));
        Assert.Equal(500, ini.ReadInteger("Server", "TestServerUserLimit", 0));
        Assert.Equal(2000, ini.ReadInteger("Server", "UserFull", 0));
        Assert.True(ini.ReadBool("Server", "ShowBlockIPLog", false));
        Assert.False(ini.ReadBool("Setup", "UseSqliteDB", true));
        Assert.Equal("HeroDB", ini.ReadString("Server", "DBName", ""));
    }

    [StaFact]
    public void ServerInfo_TestServerWriteAsMinusOneString()
    {
        _form.FillServerInfoTabValid();
        _form.CheckBoxTestServer.Checked = true;
        Assert.True(_form.ButtonServerInfoSaveClick().ok);
        var ini = new TFastIniFile(IniPath);
        // Delphi BoolToStr 默认返回 '-1'/'0'
        Assert.Equal("-1", ini.ReadString("Server", "TestServer", ""));
    }

    // ---------- 目录设置（ButtonShareDirSaveClick） ----------

    [StaFact]
    public void Share_MissingGuildDir()
    {
        _form.FillShareTabValid(_dir);
        _form.EditGuildDir.Text = Path.Combine(_dir, "nope") + "\\";
        var r = _form.ButtonShareDirSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditGuildDir", r.focus);
        Assert.Equal("行会目录设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Share_GuildDirWithoutTrailingBackslash()
    {
        _form.FillShareTabValid(_dir);
        _form.EditGuildDir.Text = Path.Combine(_dir, "GuildDir"); // 存在但无结尾反斜杠
        var r = _form.ButtonShareDirSaveClick();
        Assert.False(r.ok);
        Assert.Equal("行会目录设置错误！", M2Forms.LastMessage);
    }

    [StaFact]
    public void Share_MissingGuildFile()
    {
        _form.FillShareTabValid(_dir);
        _form.EditGuildFile.Text = Path.Combine(_dir, "none.txt");
        var r = _form.ButtonShareDirSaveClick();
        Assert.False(r.ok);
        Assert.Equal("EditGuildFile", r.focus);
        Assert.Equal("行会文件设置错误！", M2Forms.LastMessage);
    }

    [StaTheory]
    [InlineData("EditVentureDir", "Venture目录设置错误！")]
    [InlineData("EditConLogDir", "登录日志目录设置错误！")]
    [InlineData("EditCastleDir", "城堡目录设置错误！")]
    [InlineData("EditEnvirDir", "配置目录设置错误！")]
    [InlineData("EditMapDir", "地图目录设置错误！")]
    [InlineData("EditNoticeDir", "公告目录设置错误！")]
    [InlineData("EditPlugDir", "插件目录设置错误！")]
    [InlineData("edtBoxsDir", "宝箱目录设置错误！")]
    public void Share_MissingDirs(string edit, string msg)
    {
        _form.FillShareTabValid(_dir);
        switch (edit)
        {
            case "EditVentureDir": _form.EditVentureDir.Text = Path.Combine(_dir, "missing1") + "\\"; break;
            case "EditConLogDir": _form.EditConLogDir.Text = Path.Combine(_dir, "missing2") + "\\"; break;
            case "EditCastleDir": _form.EditCastleDir.Text = Path.Combine(_dir, "missing3") + "\\"; break;
            case "EditEnvirDir": _form.EditEnvirDir.Text = Path.Combine(_dir, "missing4") + "\\"; break;
            case "EditMapDir": _form.EditMapDir.Text = Path.Combine(_dir, "missing5") + "\\"; break;
            case "EditNoticeDir": _form.EditNoticeDir.Text = Path.Combine(_dir, "missing6") + "\\"; break;
            case "EditPlugDir": _form.EditPlugDir.Text = Path.Combine(_dir, "missing7") + "\\"; break;
            case "edtBoxsDir": _form.edtBoxsDir.Text = Path.Combine(_dir, "missing8") + "\\"; break;
        }
        var r = _form.ButtonShareDirSaveClick();
        Assert.False(r.ok);
        Assert.Equal(edit, r.focus);
        Assert.Equal(msg, M2Forms.LastMessage);
    }

    [StaFact]
    public void Share_ValidSave_UpdatesConfigAndIni()
    {
        _form.FillShareTabValid(_dir);
        var r = _form.ButtonShareDirSaveClick();
        Assert.True(r.ok);
        Assert.Equal(_dir + "\\GuildDir\\List\\", M2Config.sGuildDir);
        Assert.Equal(_dir + "\\GuildDir\\List.txt", M2Config.sGuildFile);
        Assert.Equal(_dir + "\\Envir\\Boxs\\", M2Config.sBoxsDir);

        var ini = new TFastIniFile(IniPath);
        Assert.Equal(_dir + "\\GuildDir\\List\\", ini.ReadString("Share", "GuildDir", ""));
        Assert.Equal(_dir + "\\GuildDir\\List.txt", ini.ReadString("Share", "GuildFile", ""));
        Assert.Equal(_dir + "\\Envir\\Boxs\\", ini.ReadString("Share", "BoxsDir", ""));
        Assert.Equal(_dir + "\\Envir\\", ini.ReadString("Share", "EnvirDir", ""));
    }

    // ---------- 状态联动 ----------

    [StaFact]
    public void TestServerCheckbox_TogglesTestEditEnabled()
    {
        _form.CheckBoxTestServer.Checked = true;
        _form.CheckBoxTestServerClick(_form);
        Assert.True(_form.EditTestLevel.Enabled);
        Assert.True(_form.EditTestGold.Enabled);
        Assert.True(_form.EditTestUserLimit.Enabled);

        _form.CheckBoxTestServer.Checked = false;
        _form.CheckBoxTestServerClick(_form);
        Assert.False(_form.EditTestLevel.Enabled);
        Assert.False(_form.EditTestGold.Enabled);
        Assert.False(_form.EditTestUserLimit.Enabled);
    }

    [StaFact]
    public void BdeSqliteRadio_TogglesDbEdits()
    {
        _form.rbBDEClick(_form);
        Assert.True(_form.EditDBName.Enabled);
        Assert.False(_form.edtSqlite.Enabled);
        _form.rbSqlite.Checked = true; // 用户点击 Sqlite 单选 → 容器自动互斥取消 BDE
        Assert.False(_form.rbBDE.Checked);
        _form.rbSqliteClick(_form);
        Assert.False(_form.EditDBName.Enabled);
        Assert.True(_form.edtSqlite.Enabled);
    }

    [StaFact]
    public void ModValue_EnablesSaveButtons_AndUmodDisables()
    {
        Assert.False(_form.ButtonNetWorkSave.Enabled);
        Assert.False(_form.ButtonServerInfoSave.Enabled);
        Assert.False(_form.ButtonShareDirSave.Enabled);
        _form.SimulateEdit(); // EditValueChange 路径
        Assert.True(_form.ButtonNetWorkSave.Enabled);
        Assert.True(_form.ButtonServerInfoSave.Enabled);
        Assert.True(_form.ButtonShareDirSave.Enabled);
        Assert.True(_form.IsModValued);
        _form.FillNetworkTabValid();
        _form.ButtonNetWorkSaveClick();
        Assert.False(_form.IsModValued);
        Assert.False(_form.ButtonNetWorkSave.Enabled);
    }

    [StaFact]
    public void Open_LoadsAllControlsFromConfig()
    {
        M2Config.sGateAddr = "10.0.0.1";
        M2Config.nGatePort = 5000;
        M2Config.sServerName = "加载测试";
        M2Config.nServerNumber = 9;
        M2Config.boTestServer = true;
        M2Config.nTestLevel = 55;
        M2Config.nUserFull = 3000;
        M2Config.sEnvirDir = "D:\\Mir\\Envir\\";
        g_sDBName = "MyDb";
        g_sSqliteDBName = "sqlite.db";
        g_boUseSqliteDB = false;
        _form.Open(showModal: false);

        Assert.Equal("10.0.0.1", _form.EditGateAddr.Text);
        Assert.Equal("5000", _form.EditGatePort.Text);
        Assert.Equal("加载测试", _form.EditGameName.Text);
        Assert.Equal("9", _form.EditServerNumber.Text);
        Assert.Equal("7", _form.EditServerIndex.Text); // 显示全局 nServerIndex（测试构造器置 7）
        Assert.True(_form.CheckBoxTestServer.Checked);
        Assert.Equal("55", _form.EditTestLevel.Text);
        Assert.Equal("3000", _form.EditUserFull.Text);
        Assert.Equal("MyDb", _form.EditDBName.Text);
        Assert.Equal("sqlite.db", _form.edtSqlite.Text);
        Assert.True(_form.rbBDE.Checked);
        Assert.False(_form.rbSqlite.Checked);
        Assert.True(_form.EditDBName.Enabled); // UseSqliteDB=false → EditDBName.Enabled := not UseSqliteDB
    }

    [StaFact]
    public void PageControlChanging_ConfirmDiscard_ResetsModifiedFlag()
    {
        _form.Open(showModal: false);
        _form.SimulateEdit();
        M2Forms.NextAnswer = 6; // IDYES
        Assert.True(_form.PageControlChangingConfirm());
        Assert.False(_form.IsModValued);
    }

    [StaFact]
    public void PageControlChanging_Refuse_CancelsChange()
    {
        _form.Open(showModal: false);
        _form.SimulateEdit();
        M2Forms.NextAnswer = 7; // IDNO
        Assert.False(_form.PageControlChangingConfirm());
        Assert.True(_form.IsModValued);
    }
}

/// <summary>GeneralConfigForm 测试填充辅助（合法值场景）。</summary>
internal static class GeneralConfigFormTestExt
{
    public static void FillNetworkTabValid(this GeneralConfigForm f)
    {
        f.EditGateAddr.Text = "192.168.1.10"; f.EditGatePort.Text = "7100";
        f.EditIDSAddr.Text = "192.168.1.11"; f.EditIDSPort.Text = "7200";
        f.EditDBAddr.Text = "192.168.1.12"; f.EditDBPort.Text = "7300";
        f.EditLogServerAddr.Text = "192.168.1.13"; f.EditLogServerPort.Text = "7400";
        f.EditMsgSrvAddr.Text = "192.168.1.14"; f.EditMsgSrvPort.Text = "7500";
    }

    public static void FillServerInfoTabValid(this GeneralConfigForm f)
    {
        f.EditGameName.Text = "测试服";
        f.EditServerIndex.Text = "0";
        f.EditServerNumber.Text = "1";
        f.EditTestLevel.Text = "60";
        f.EditTestGold.Text = "100000";
        f.EditTestUserLimit.Text = "500";
        f.EditUserFull.Text = "2000";
        f.EditDBName.Text = "HeroDB";
        f.rbBDE.Checked = true;
    }

    public static void FillShareTabValid(this GeneralConfigForm f, string dir)
    {
        Func<string, string> mk = rel =>
        {
            string p = Path.Combine(dir, rel.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(p);
            return p;
        };

        f.EditGuildDir.Text = mk("GuildDir\\List") + "\\";
        File.WriteAllText(Path.Combine(dir, "GuildDir", "List.txt"), "[]");
        f.EditGuildFile.Text = Path.Combine(dir, "GuildDir", "List.txt");
        f.EditVentureDir.Text = mk("VentureDir") + "\\";
        f.EditConLogDir.Text = mk("ConLogDir") + "\\";
        f.EditCastleDir.Text = mk("CastleDir") + "\\";
        f.EditEnvirDir.Text = mk("Envir") + "\\";
        f.EditMapDir.Text = mk("Map") + "\\";
        f.EditNoticeDir.Text = mk("Notice") + "\\";
        f.EditPlugDir.Text = mk("PlugDir") + "\\";
        f.edtBoxsDir.Text = mk("Envir\\Boxs") + "\\";
    }
}

/// <summary>xUnit STA 线程运行的 Fact（WinForms 控件操作）。</summary>
public sealed class StaFactAttribute : FactAttribute
{
    public StaFactAttribute() { }
}

/// <summary>xUnit 参数化（控件文本驱动，无需真 STA 句柄，与 StaFact 同语义命名）。</summary>
public sealed class StaTheoryAttribute : TheoryAttribute
{
    public StaTheoryAttribute() { }
}

public static class StaRunner
{
    public static T New<T>(Func<T> factory) where T : IDisposable
    {
        T? result = default;
        Action action = () => result = factory();
        New(action);
        return result!;
    }

    /// <summary>STA 线程执行并回抛异常（异常逃逸线程入口会崩 testhost 使运行挂起）。</summary>
    public static void New(Action action)
    {
        Exception? caught = null;
        var t = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }
}

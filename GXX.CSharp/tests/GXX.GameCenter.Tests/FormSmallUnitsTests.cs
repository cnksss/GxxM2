using System;
using GXX.GameCenter;
using Xunit;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GCertServerSet.pas / GLoginServer.pas / GLoginServerRouteSet.pas 小单元（131/35/26 行）测试。
/// 窗体操作在 STA 线程执行，弹窗经 <see cref="GameCenterDialogs"/> 接缝阻断。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class FormSmallUnitsTests : GameCenterTestBase
{
    public FormSmallUnitsTests()
    {
        GCertServerSetGlobals.ResetForTests();
    }

    private static void Sta(Action action)
    {
        Exception? caught = null;
        var t = new System.Threading.Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        t.SetApartmentState(System.Threading.ApartmentState.STA);
        t.Start();
        t.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }

    // ==================================================================
    // GCertServerSet.pas
    // ==================================================================

    [Fact]
    public void FormCreate_DisablesOkButton()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            Assert.False(f.ButtonOK.Enabled);
            Assert.False(f.IsOpened);
            Assert.False(f.IsModValued);
        });
    }

    [Fact]
    public void RefInfo_LoadsSixGlobalsAndMarksOpened()
    {
        Sta(() =>
        {
            GCertServerSetGlobals.g_sRunGate_Config_RegServerAddr = "1.1.1.1";
            GCertServerSetGlobals.g_nRunGate_Config_RegServerPort = 100;
            GCertServerSetGlobals.g_sDBServer_Config_RegServerAddr = "2.2.2.2";
            GCertServerSetGlobals.g_nDBServer_Config_RegServerPort = 200;
            GCertServerSetGlobals.g_sM2Server_Config_RegServerAddr = "3.3.3.3";
            GCertServerSetGlobals.g_nM2Server_Config_RegServerPort = 300;

            using var f = new CertServerSetForm();
            f.RefInfo();

            Assert.Equal("1.1.1.1", f.EditRunGate_Config_RegServerAddr.Text);
            Assert.Equal("100", f.EditRunGate_Config_RegServerPort.Text);
            Assert.Equal("2.2.2.2", f.EditDBServer_Config_RegServerAddr.Text);
            Assert.Equal("200", f.EditDBServer_Config_RegServerPort.Text);
            Assert.Equal("3.3.3.3", f.EditM2Server_Config_RegServerAddr.Text);
            Assert.Equal("300", f.EditM2Server_Config_RegServerPort.Text);
            Assert.True(f.IsOpened);
            Assert.False(f.ButtonOK.Enabled);
        });
    }

    [Fact]
    public void EditChange_BeforeOpen_DoesNotModValue()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditChange(f);
            Assert.False(f.IsModValued);
            Assert.False(f.ButtonOK.Enabled);
        });
    }

    [Fact]
    public void EditChange_AfterRefInfo_MarksModifiedAndEnablesOk()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.RefInfo();
            f.EditChange(f);
            Assert.True(f.IsModValued);
            Assert.True(f.ButtonOK.Enabled);
        });
    }

    [Fact]
    public void ModValueAndUModValue_ToggleBothFlags()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.ModValue();
            Assert.True(f.IsModValued);
            Assert.True(f.ButtonOK.Enabled);
            f.uModValue();
            Assert.False(f.IsModValued);
            Assert.False(f.ButtonOK.Enabled);
        });
    }

    [Fact]
    public void ButtonOKClick_InvalidRunGateAddr_ShowsExactMessageAndFocus()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "not_an_ip";
            f.EditRunGate_Config_RegServerPort.Text = "100";

            Assert.False(f.ButtonOKClick(f));
            Assert.Equal("游戏网关验证服务器地址设置错误！！！", GameCenterDialogs.LastMessage);
            Assert.Equal("错误信息", GameCenterDialogs.LastCaption);
            Assert.Equal("EditRunGate_Config_RegServerAddr", f.LastFocus);
        });
    }

    [Fact]
    public void ButtonOKClick_RunGatePortAbove65535_IsRejected()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "1.2.3.4";
            f.EditRunGate_Config_RegServerPort.Text = "65536";
            Assert.False(f.ButtonOKClick(f));
            Assert.Equal("游戏网关验证服务器端口设置错误！！！", GameCenterDialogs.LastMessage);
        });
    }

    [Fact]
    public void ButtonOKClick_RunGatePortNegative_IsRejected()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "1.2.3.4";
            f.EditRunGate_Config_RegServerPort.Text = "-1";
            Assert.False(f.ButtonOKClick(f));
            Assert.Equal("游戏网关验证服务器端口设置错误！！！", GameCenterDialogs.LastMessage);
        });
    }

    [Fact]
    public void ButtonOKClick_NonNumericRunGatePort_ParsesAsMinusOne()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "1.2.3.4";
            f.EditRunGate_Config_RegServerPort.Text = "abc";
            Assert.False(f.ButtonOKClick(f));
            Assert.Equal("游戏网关验证服务器端口设置错误！！！", GameCenterDialogs.LastMessage);
        });
    }

    [Fact]
    public void ButtonOKClick_InvalidDbAddr_IsRejected()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "1.2.3.4";
            f.EditRunGate_Config_RegServerPort.Text = "100";
            f.EditDBServer_Config_RegServerAddr.Text = "1.2.3";
            f.EditDBServer_Config_RegServerPort.Text = "200";

            Assert.False(f.ButtonOKClick(f));
            Assert.Equal("数据库验证服务器地址设置错误！！！", GameCenterDialogs.LastMessage);
            Assert.Equal("EditDBServer_Config_RegServerAddr", f.LastFocus);
        });
    }

    [Fact]
    public void ButtonOKClick_InvalidDbPort_IsRejected()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "1.2.3.4";
            f.EditRunGate_Config_RegServerPort.Text = "100";
            f.EditDBServer_Config_RegServerAddr.Text = "2.2.2.2";
            f.EditDBServer_Config_RegServerPort.Text = "70000";

            Assert.False(f.ButtonOKClick(f));
            Assert.Equal("数据库验证服务器端口设置错误！！！", GameCenterDialogs.LastMessage);
            Assert.Equal("EditDBServer_Config_RegServerPort", f.LastFocus);
        });
    }

    [Fact]
    public void ButtonOKClick_InvalidM2Addr_IsNotValidated()
    {
        // 差异断言（原文缺陷）：第三段重复校验 RunGate，M2Server 的非法地址被直接接受。
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = "1.2.3.4";
            f.EditRunGate_Config_RegServerPort.Text = "100";
            f.EditDBServer_Config_RegServerAddr.Text = "2.2.2.2";
            f.EditDBServer_Config_RegServerPort.Text = "200";
            f.EditM2Server_Config_RegServerAddr.Text = "not_an_ip_at_all";
            f.EditM2Server_Config_RegServerPort.Text = "999999";

            Assert.True(f.ButtonOKClick(f));
            Assert.Equal("not_an_ip_at_all", GCertServerSetGlobals.g_sM2Server_Config_RegServerAddr);
            Assert.Equal(999999, GCertServerSetGlobals.g_nM2Server_Config_RegServerPort);
            // 三段校验均未报错
            Assert.Null(GameCenterDialogs.LastMessage);
        });
    }

    [Fact]
    public void ButtonOKClick_ValidInput_WritesSixGlobalsAndClearsModified()
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = " 1.2.3.4 ";
            f.EditRunGate_Config_RegServerPort.Text = "100";
            f.EditDBServer_Config_RegServerAddr.Text = "2.2.2.2";
            f.EditDBServer_Config_RegServerPort.Text = "200";
            f.EditM2Server_Config_RegServerAddr.Text = "3.3.3.3";
            f.EditM2Server_Config_RegServerPort.Text = "300";

            Assert.True(f.ButtonOKClick(f));

            // Trim 生效
            Assert.Equal("1.2.3.4", GCertServerSetGlobals.g_sRunGate_Config_RegServerAddr);
            Assert.Equal(100, GCertServerSetGlobals.g_nRunGate_Config_RegServerPort);
            Assert.Equal("2.2.2.2", GCertServerSetGlobals.g_sDBServer_Config_RegServerAddr);
            Assert.Equal(200, GCertServerSetGlobals.g_nDBServer_Config_RegServerPort);
            Assert.Equal("3.3.3.3", GCertServerSetGlobals.g_sM2Server_Config_RegServerAddr);
            Assert.Equal(300, GCertServerSetGlobals.g_nM2Server_Config_RegServerPort);
            Assert.False(f.ButtonOK.Enabled);
            Assert.False(f.IsModValued);
        });
    }

    [Theory]
    [InlineData("0.0.0.0", true)]
    [InlineData("255.255.255.255", true)]
    [InlineData("1.2.3", false)]
    [InlineData("", false)]
    public void ButtonOKClick_BoundaryRunGateAddr(string addr, bool accepted)
    {
        Sta(() =>
        {
            using var f = new CertServerSetForm();
            f.EditRunGate_Config_RegServerAddr.Text = addr;
            f.EditRunGate_Config_RegServerPort.Text = "0";
            f.EditDBServer_Config_RegServerAddr.Text = "1.1.1.1";
            f.EditDBServer_Config_RegServerPort.Text = "1";
            f.EditM2Server_Config_RegServerAddr.Text = "1.1.1.1";
            f.EditM2Server_Config_RegServerPort.Text = "1";
            Assert.Equal(accepted, f.ButtonOKClick(f));
        });
    }

    [Fact]
    public void Open_CallsRefInfoThenShowModal()
    {
        Sta(() =>
        {
            bool shown = false;
            CertServerSetForm.ShowModalHandler = _ => { shown = true; return true; };
            try
            {
                GCertServerSetGlobals.g_sRunGate_Config_RegServerAddr = "9.9.9.9";
                using var f = new CertServerSetForm();
                f.Open();
                Assert.True(shown);
                Assert.Equal("9.9.9.9", f.EditRunGate_Config_RegServerAddr.Text);
            }
            finally { CertServerSetForm.ShowModalHandler = null; }
        });
    }

    // ==================================================================
    // GLoginServer.pas
    // ==================================================================

    [Fact]
    public void LoginServerConfigForm_FormCreateWritesSixColumnHeaders()
    {
        Sta(() =>
        {
            using var f = new LoginServerConfigForm();
            Assert.Equal(
                new[] { "服务器名称", "路由标识", "登录网关内IP", "登录网关外IP", "角色网关", "端口" },
                f.HeaderRow());
        });
    }

    [Fact]
    public void LoginServerConfigForm_OpenOnlyShowsModal()
    {
        Sta(() =>
        {
            bool shown = false;
            LoginServerConfigForm.ShowModalHandler = _ => { shown = true; return false; };
            try
            {
                using var f = new LoginServerConfigForm();
                f.Open();
                Assert.True(shown);
            }
            finally { LoginServerConfigForm.ShowModalHandler = null; }
        });
    }

    [Fact]
    public void LoginServerConfigForm_HasSixGridColumns()
    {
        Sta(() =>
        {
            using var f = new LoginServerConfigForm();
            Assert.Equal(6, f.GridGateRoute.Columns.Count);
        });
    }

    // ==================================================================
    // GLoginServerRouteSet.pas
    // ==================================================================

    [Fact]
    public void LoginServerRouteSetForm_OpenStoresNewRouteModeAndShowsModal()
    {
        Sta(() =>
        {
            bool shown = false;
            LoginServerRouteSetForm.ShowModalHandler = _ => { shown = true; return true; };
            try
            {
                using var f = new LoginServerRouteSetForm();
                f.Open(true);
                Assert.True(f.NewRouteMode);
                Assert.True(shown);
            }
            finally { LoginServerRouteSetForm.ShowModalHandler = null; }
        });
    }

    [Fact]
    public void LoginServerRouteSetForm_OpenWithFalse_ClearsFlag()
    {
        Sta(() =>
        {
            using var f = new LoginServerRouteSetForm();
            f.Open(true);
            f.Open(false);
            Assert.False(f.NewRouteMode);
        });
    }
}

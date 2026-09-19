using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>TestSelGate.pas:1-56（网关路由自检窗体）逻辑测试。</summary>
public class TestSelGateTests : TempDirTest
{
    private static void AddRoute(int idx, string selGate, params (string ip, int port)[] gates)
    {
        var r = DBShareSeam.g_RouteInfo[idx];
        r.sSelGateIP = selGate;
        r.nGateCount = gates.Length;
        for (int i = 0; i < gates.Length; i++)
        {
            r.sGameGateIP[i] = gates[i].ip;
            r.nGameGatePort[i] = gates[i].port;
        }
    }

    [Fact]
    public void GetGameGateText_命中时返回IP冒号端口()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        DelphiRandom.Next = _ => 0;

        string text = TestSelGateLogic.GetGameGateText("127.0.0.1", out int port);

        Assert.Equal("10.0.0.1:7200", text);
        Assert.Equal(7200, port);
    }

    [Fact]
    public void GetGameGateText_随机取第二条()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        DelphiRandom.Next = _ => 1;

        Assert.Equal("10.0.0.2:7201", TestSelGateLogic.GetGameGateText("127.0.0.1", out _));
    }

    [Fact]
    public void GetGameGateText_未命中返回无此网关设置()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));

        string text = TestSelGateLogic.GetGameGateText("192.168.0.1", out int port);

        Assert.Equal("无此网关设置", text);
        Assert.Equal(0, port);
    }

    [Fact]
    public void GetGameGateText_对输入做Trim_原文如此()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));

        Assert.Equal("10.0.0.1:7200", TestSelGateLogic.GetGameGateText("  127.0.0.1  ", out _));
    }

    [Fact]
    public void GetGameGateText_空输入不匹配任何路由()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));

        Assert.Equal("无此网关设置", TestSelGateLogic.GetGameGateText("   ", out _));
        Assert.Equal("无此网关设置", TestSelGateLogic.GetGameGateText("", out _));
    }

    [Fact]
    public void ButtonTestClick_把结果写进EditGameGate()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));
        using var form = new FrmTestSelGate();
        form.EditSelGate.Text = "127.0.0.1";

        form.ButtonTestClick(null, EventArgs.Empty);
        Assert.Equal("10.0.0.1:7200", form.EditGameGate.Text);

        form.EditSelGate.Text = "1.2.3.4";
        form.ButtonTestClick(null, EventArgs.Empty);
        Assert.Equal("无此网关设置", form.EditGameGate.Text);
    }

    [Fact]
    public void Button1Click_调用路由管理窗体打开接缝()
    {
        int opened = 0;
        Action saved = FrmTestSelGate.OpenRouteManage;
        try
        {
            FrmTestSelGate.OpenRouteManage = () => opened++;
            using var form = new FrmTestSelGate();
            form.Button1Click(null, EventArgs.Empty);
            Assert.Equal(1, opened);
        }
        finally
        {
            FrmTestSelGate.OpenRouteManage = saved;
        }
    }

    [Fact]
    public void DFM_控件几何与文案对齐()
    {
        using var form = new FrmTestSelGate();
        Assert.Equal("测试选择网关", form.Text);
        Assert.Equal(new System.Drawing.Size(209, 120), form.ClientSize);
        Assert.Equal("127.0.0.1", form.EditSelGate.Text);
        Assert.Equal("", form.EditGameGate.Text);
        Assert.Equal("测试(&T)", form.ButtonTest.Text);
        Assert.Equal("配置(&C)", form.Button1.Text);
        Assert.Equal(new System.Drawing.Point(64, 16), form.EditSelGate.Location);
        Assert.Equal(new System.Drawing.Point(64, 40), form.EditGameGate.Location);
        Assert.Equal(new System.Drawing.Point(16, 72), form.ButtonTest.Location);
        Assert.Equal(new System.Drawing.Point(112, 72), form.Button1.Location);
    }
}

/// <summary>CreateId.pas:1-35：原文只有窗体骨架，没有任何 ID 生成规则。</summary>
public class CreateIdTests : TempDirTest
{
    [Fact]
    public void DFM_窗体与控件属性对齐()
    {
        using var form = new FrmCreateId();
        Assert.Equal("新建帐号", form.Text);
        Assert.Equal(new System.Drawing.Size(237, 135), form.ClientSize);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedSingle, form.FormBorderStyle);
        Assert.False(form.MaximizeBox);         // BorderIcons = [biSystemMenu, biMinimize]
        Assert.Equal("帐号:", form.Label1.Text);
        Assert.Equal("密码:", form.Label2.Text);
        Assert.Equal(new System.Drawing.Point(60, 22), form.EdId.Location);
        Assert.Equal(new System.Drawing.Point(60, 51), form.EdPasswd.Location);
        Assert.Equal("确定(&O)", form.BitBtn1.Text);
        Assert.Equal(System.Windows.Forms.DialogResult.OK, form.BitBtn1.DialogResult);
        Assert.Equal(System.Windows.Forms.DialogResult.Cancel, form.BitBtn2.DialogResult);
        Assert.Equal(0, form.EdId.TabIndex);
        Assert.Equal(1, form.EdPasswd.TabIndex);
    }

    [Fact]
    public void FormShow_不抛异常且不改动输入框()
    {
        using var form = new FrmCreateId();
        form.EdId.Text = "abc";
        Exception ex = Record.Exception(() => form.FormShow(null, EventArgs.Empty));
        Assert.Null(ex);
        Assert.Equal("abc", form.EdId.Text);
    }

    [Fact]
    public void 原文如此_该单元只声明DFM中的控件与FormShow_没有ID生成API()
    {
        // 防止后续"顺手补全"：CreateId.pas 没有任何生成/校验 ID 的方法，
        // 真正的角色 ID 分配在 RoleDB/SqliteRoleDB（DoGetID/自增 ID）一侧。
        string[] expectedFields =
        {
            "EdId", "EdPasswd", "Label1", "Label2", "BitBtn1", "BitBtn2"
        };
        string[] actualFields = typeof(FrmCreateId)
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f => f.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(expectedFields.OrderBy(n => n, StringComparer.Ordinal).ToArray(), actualFields);

        string[] declaredMethods = typeof(FrmCreateId)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(new[] { "FormShow" }, declaredMethods);
    }
}

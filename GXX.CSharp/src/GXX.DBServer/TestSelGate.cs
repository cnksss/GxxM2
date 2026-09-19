using System;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.DBServer;

// TestSelGate.pas (1-56) → TestSelGate.cs
// 网关路由自检小窗体：输入角色选择网关地址 → 走 DBShare.GateRouteIP 得到随机主游戏网关 → 显示 "IP:Port"。

/// <summary>TestSelGate.pas:35-49 的纯逻辑（可单测）。</summary>
public static class TestSelGateLogic
{
    public const string NoSuchGateText = "无此网关设置";

    /// <summary>
    /// TestSelGate.pas:35-49 `ButtonTestClick`：
    ///   sSelGateIPaddr := Trim(EditSelGate.Text)
    ///   sGameGateIPaddr := GateRouteIP(sSelGateIPaddr, nGameGatePort)
    ///   '' → '无此网关设置'；否则 format('%s:%d', [IP, Port])
    /// </summary>
    public static string GetGameGateText(string editSelGateText, out int nGameGatePort)
    {
        string sSelGateIPaddr = DelphiRTL.Trim(editSelGateText);
        string sGameGateIPaddr = DBShareSeam.GateRouteIP(sSelGateIPaddr, out nGameGatePort);
        if (sGameGateIPaddr == "")
        {
            return NoSuchGateText;
        }
        return DelphiRTL.Format("%s:%d", sGameGateIPaddr, nGameGatePort);
    }
}

/// <summary>TestSelGate.pas:27 `var frmTestSelGate: TfrmTestSelGate;`。</summary>
public static class TestSelGateGlobal
{
    public static FrmTestSelGate frmTestSelGate;
}

/// <summary>TestSelGate.pas:10-24 `TfrmTestSelGate`（DFM: TestSelGate.dfm）。</summary>
public class FrmTestSelGate : Form
{
    // DFM: frmTestSelGate Left=1479 Top=473 BorderIcons=[biSystemMenu] BorderStyle=bsSingle
    //      Caption='测试选择网关' ClientHeight=120 ClientWidth=209 Position=poMainFormCenter PixelsPerInch=96
    public GroupBox GroupBox1;
    public Label Label1;
    public TextBox EditSelGate;
    public Label Label2;
    public TextBox EditGameGate;
    public Button ButtonTest;
    public Button Button1;

    /// <summary>TestSelGate.pas:52-54 `Button1Click` 打开的路由管理窗体（接缝：由宿主注入）。</summary>
    public static Action OpenRouteManage = () => RouteManageGlobal.frmRouteManage?.Open();

    public FrmTestSelGate()
    {
        // DFM: frmTestSelGate Left=1479 Top=473 BorderIcons=[biSystemMenu] BorderStyle=bsSingle
        //      Caption='测试选择网关' ClientHeight=120 ClientWidth=209
        Text = "测试选择网关";
        StartPosition = FormStartPosition.CenterParent;   // Position=poMainFormCenter
        Location = new System.Drawing.Point(1479, 473);
        ClientSize = new System.Drawing.Size(209, 120);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: GroupBox1 Left=8 Top=8 Width=193 Height=105 Caption='' TabOrder=0
        GroupBox1 = new GroupBox { Left = 8, Top = 8, Width = 193, Height = 105, Text = "" };
        // DFM: Label1 Left=8 Top=20 Width=54 Height=12 Caption='角色网关:'
        Label1 = new Label { Left = 8, Top = 20, Width = 54, Height = 12, Text = "角色网关:" };
        // DFM: Label2 Left=8 Top=44 Width=54 Height=12 Caption='游戏网关:'
        Label2 = new Label { Left = 8, Top = 44, Width = 54, Height = 12, Text = "游戏网关:" };
        // DFM: EditSelGate Left=64 Top=16 Width=113 Height=20 TabOrder=0 Text='127.0.0.1'
        EditSelGate = new TextBox { Left = 64, Top = 16, Width = 113, Height = 20, TabIndex = 0, Text = "127.0.0.1" };
        // DFM: EditGameGate Left=64 Top=40 Width=113 Height=20 TabOrder=1
        EditGameGate = new TextBox { Left = 64, Top = 40, Width = 113, Height = 20, TabIndex = 1 };
        // DFM: ButtonTest Left=16 Top=72 Width=65 Height=25 Caption='测试(&T)' TabOrder=2 OnClick=ButtonTestClick
        ButtonTest = new Button { Left = 16, Top = 72, Width = 65, Height = 25, Text = "测试(&T)", TabIndex = 2 };
        // DFM: Button1 Left=112 Top=72 Width=65 Height=25 Caption='配置(&C)' TabOrder=3 OnClick=Button1Click
        Button1 = new Button { Left = 112, Top = 72, Width = 65, Height = 25, Text = "配置(&C)", TabIndex = 3 };

        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(EditSelGate);
        GroupBox1.Controls.Add(EditGameGate);
        GroupBox1.Controls.Add(ButtonTest);
        GroupBox1.Controls.Add(Button1);
        Controls.Add(GroupBox1);

        ButtonTest.Click += (s, e) => ButtonTestClick(s, e);
        Button1.Click += (s, e) => Button1Click(s, e);
    }

    /// <summary>TestSelGate.pas:35-49 `ButtonTestClick`。</summary>
    public void ButtonTestClick(object Sender, EventArgs e)
    {
        int nGameGatePort;
        EditGameGate.Text = TestSelGateLogic.GetGameGateText(EditSelGate.Text, out nGameGatePort);
    }

    /// <summary>TestSelGate.pas:51-54 `Button1Click`：frmRouteManage.Open。</summary>
    public void Button1Click(object Sender, EventArgs e)
    {
        OpenRouteManage();
    }
}

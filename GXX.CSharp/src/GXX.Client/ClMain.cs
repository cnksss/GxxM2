using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Client.HGE;
using GXX.GatewayKit;

namespace GXX.Client;

/// <summary>
/// ClMain.pas TfrmMain 1:1 核心转换：HGE 渲染主窗体 + 网关连接 + 场景调度。
/// 场景：登录（Login）→ 选角（Select）→ 游戏（Play），对应 DrawScrn/IntroScn/PlayScn。
/// </summary>
public class FrmMain : Form
{
    private readonly IHge _hge = new GdiPlusHge();
    private TcpLink? _gateLink;

    public int ScreenWidth { get; set; } = 1024;
    public int ScreenHeight { get; set; } = 768;
    public string ServerAddr { get; set; } = "127.0.0.1";
    public int ServerPort { get; set; } = 7000;

    public enum GameScene { Intro, Login, SelectChr, Play }
    public GameScene Scene { get; set; } = GameScene.Login;

    public string AccountName = "";
    public string AccountPass = "";
    public string ChrName = "";
    public bool LoggedIn;
    public bool ChrSelected;

    public FrmMain()
    {
        Text = "传奇客户端 (C#)";
        ClientSize = new Size(ScreenWidth, ScreenHeight);
        StartPosition = FormStartPosition.CenterScreen;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

        _gateLink = new TcpLink(ServerAddr, ServerPort);
        _gateLink.OnReceive += OnGateData;
    }

    private void OnGateData(byte[] buf, int off, int len)
    {
        _gateLink?.Accumulate(buf, off, len);
        // 逐包解析 6-Bit TDefaultMessage 帧（每帧 GetEncodeSize(16)=22 字节起）
        var link = _gateLink!;
        while (link.AccumLength >= 22)
        {
            TDefaultMessage msg = EDcode.DecodeMessage(link.AccumBuffer.AsSpan(0, 22).ToArray());
            switch (msg.Ident)
            {
                case 529: // SM_PASSOK_SELECTSERVER
                    LoggedIn = true;
                    Scene = GameScene.SelectChr;
                    break;
                case 503: // SM_PASSWD_FAIL
                    LoggedIn = false;
                    break;
                case 520: // SM_QUERYCHR
                    Scene = GameScene.SelectChr;
                    break;
                case 525: // SM_STARTPLAY
                    ChrSelected = true;
                    Scene = GameScene.Play;
                    break;
            }
            link.ConsumeAccum(22);
        }
    }

    public bool ConnectToGate()
    {
        _gateLink ??= new TcpLink(ServerAddr, ServerPort);
        _gateLink.OnReceive -= OnGateData;
        _gateLink.OnReceive += OnGateData;
        return _gateLink.Connect();
    }

    public void SendLogin(string account, string password)
    {
        AccountName = account;
        AccountPass = password;
        // CM_IDPASSWORD = 2001，body: account\tpassword
        var msg = TDefaultMessage.Make(2001, 0, 0, 0, 0);
        byte[] head = EDcode.EncodeMessage(msg);
        byte[] body = EDcode.EncodeString(account + "\t" + password);
        byte[] packet = new byte[head.Length + body.Length];
        Array.Copy(head, packet, head.Length);
        Array.Copy(body, 0, packet, head.Length, body.Length);
        _gateLink?.Send(packet);
    }

    public void SendQueryChr()
    {
        var msg = TDefaultMessage.Make(100 /* CM_QUERYCHR */, 0, 0, 0, 0);
        byte[] head = EDcode.EncodeMessage(msg);
        byte[] body = EDcode.EncodeString(AccountName);
        byte[] packet = new byte[head.Length + body.Length];
        Array.Copy(head, packet, head.Length);
        Array.Copy(body, 0, packet, head.Length, body.Length);
        _gateLink?.Send(packet);
    }

    public void SendWalk(byte dir)
    {
        // CM_WALK = 3011：Recog=方向，Param=X，Tag=Y
        var msg = TDefaultMessage.Make(3011, dir, 0, 0, 0);
        _gateLink?.Send(EDcode.EncodeMessage(msg));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _hge.Gfx_BeginScene();
        _hge.Gfx_Clear(HgeColor.ARGB(255, 0, 0, 0));
        // 简单场景渲染指示（完整场景层 PlayScn/IntroScn 在 P6 深化）
        string text = Scene switch
        {
            GameScene.Login => "登录场景 LoginScene - 按回车发送测试登录",
            GameScene.SelectChr => "选角场景 SelectChrScene",
            GameScene.Play => "游戏场景 PlayScene",
            _ => "Intro"
        };
        e.Graphics.DrawString(text, SystemFonts.DefaultFont, Brushes.White, 20, 20);
        _hge.Gfx_EndScene();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Enter && Scene == GameScene.Login && !LoggedIn)
        {
            if (ConnectToGate())
                SendLogin("testacct", "testpass");
        }
        else if (Scene == GameScene.Play)
        {
            byte dir = e.KeyCode switch
            {
                Keys.Up => Grobal2Const.DR_UP,
                Keys.Right => Grobal2Const.DR_RIGHT,
                Keys.Down => Grobal2Const.DR_DOWN,
                Keys.Left => Grobal2Const.DR_LEFT,
                _ => (byte)255
            };
            if (dir != 255)
                SendWalk(dir);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _gateLink?.Close();
        _hge.Dispose();
        base.OnFormClosed(e);
    }
}

using System;
using System.Collections.Generic;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// `IDSocCli`（464 行）的测试替身：JSocket/TTimer/模块表/在线数 -> 可观测的内存实现。
/// **绝不建真 socket、绝不起真定时器**。
/// </summary>
public sealed class FakeIDSocSocketEndPoint : IIDSocSocketEndPoint
{
    public bool Connected { get; set; } = true;
    public string ReceiveText { get; set; } = "";
    public string RemoteAddress { get; set; } = "10.1.1.1";
    public int LocalPort { get; set; } = 5601;
    public int RemotePort { get; set; } = 5600;

    public readonly List<string> Sent = new();
    public int CloseCount;

    public void SendText(string sText) => Sent.Add(sText);
    public void Close() => CloseCount++;
}

/// <summary>接缝：`JSocket.pas` 的 TClientSocket（记录每一次属性写入的顺序）。</summary>
public sealed class FakeIDSocClientSocket : IIDSocClientSocket
{
    private bool _active;

    public bool Active
    {
        get => _active;
        set { _active = value; Ops.Add("Active=" + (value ? "True" : "False")); }
    }

    private string _address = "";
    public string Address
    {
        get => _address;
        set { _address = value; Ops.Add("Address=" + value); }
    }

    private int _port;
    public int Port
    {
        get => _port;
        set { _port = value; Ops.Add("Port=" + value); }
    }

    public FakeIDSocSocketEndPoint EndPoint { get; } = new();
    public IIDSocSocketEndPoint Socket => EndPoint;

    /// <summary>属性写入/读取顺序（用于锁定 `OpenConnect` 的"先设地址端口、再激活"）。</summary>
    public readonly List<string> Ops = new();

    public void ClearOps() => Ops.Clear();
}

/// <summary>`IDSocCli` 测试基类：装上 4 组宿主面接缝（全部可观测），并复位本车道其它接缝。</summary>
public abstract class IDSocCliTestBase : TempDirTest
{
    protected readonly FakeIDSocClientSocket Sock = new();
    protected readonly List<string> Logs = new();
    protected readonly List<bool> Timer1 = new();
    protected readonly List<bool> KeepAliveTimer = new();
    protected readonly List<(object Module, string Name, string Address, string Buffer)> ModuleAdded = new();
    protected readonly List<object> ModuleRemoved = new();
    protected readonly List<(IntPtr Handle, string Buffer)> ModuleBuffers = new();
    protected int SelectCharCount = 7;

    protected IDSocCliTestBase()
    {
        IDSocCliSeam.Reset();
        SelectClientModuleSeam.Reset();
        RoleDbSeam.MainOutMessage = s => Logs.Add(s);

        IDSocCliSeam.IDSocket = Sock;
        IDSocCliSeam.Timer1Enabled = v => Timer1.Add(v);
        IDSocCliSeam.KeepAliveTimerEnabled = v => KeepAliveTimer.Add(v);
        IDSocCliSeam.GetSelectCharCount = () => SelectCharCount;
        SelectClientModuleSeam.AddModule = (m, n, a, b) =>
        {
            ModuleAdded.Add((m, n, a, b));
            return new IntPtr(ModuleAdded.Count);
        };
        SelectClientModuleSeam.RemoveModule = m => ModuleRemoved.Add(m);
        SelectClientModuleSeam.UpdateModuleBuffer = (h, b) => ModuleBuffers.Add((h, b));
    }

    /// <summary>建一个窗体（**测试务必 `using`**：`TFrmIDSoc` 是 WinForms Form）。</summary>
    protected TFrmIDSoc NewFrm() => new TFrmIDSoc();

    /// <summary>模拟"socket 收到一段文本" → 原文 `IDSocketRead`（:100-108）。</summary>
    protected void Read(TFrmIDSoc frm, string text)
    {
        Sock.EndPoint.ReceiveText = text;
        frm.IDSocketRead();
    }

    /// <summary>塞一条会话，等价于 LoginSrv 推了一帧 `(1000/账号/会话号/0/x/IP)`。</summary>
    protected static void PushSession(TFrmIDSoc frm, string account, int sessionId, string ip = "1.1.1.1")
        => frm.ProcessAddSession(account + "/" + sessionId + "/0/x/" + ip);
}

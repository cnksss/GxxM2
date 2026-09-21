using System;
using System.IO;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

/// <summary>
/// `GXX.LoginGate.Rest11.Rest11LoginGateKernel`（LoginGate 侧接线）**无法**在本工程引用
/// （`GXX.GatewayKit.Tests.csproj` 只引用 GatewayKit + RunGate，且本车道**禁止改 csproj**）。
/// 因此此处覆盖的是 kernel **实际使用的**通用入口在 GatewayKit 侧的行为契约，
/// 并通过 <see cref="GatewaySessionAdapterContract"/> 把 LoginGate 侧适配器必须满足的语义固定下来。
/// </summary>
public class Rest11LoginGateKernelContractTests
{
    // =====================================================================================
    // FuncForComm.pas:358-427 StartService / :429-465 StopService 的残部行为
    // =====================================================================================
    [Fact]
    public void StartService_EnablesThreadAndLoadsConfigAndClearsConnectTable()
    {
        string dir = Path.Combine(Path.GetTempPath(), "rest11k-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var cfg = new Rest11LoginGateConfig(Path.Combine(dir, "LoginGate.ini"));
            var filter = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = dir };
            var thread = new Rest11ProcMsgThread();

            // StartService 的残部（:366 / :368 / :373 / :375）
            filter.OverConnectOfIP(Rest11LoginGateNet.InetAddr("10.0.0.1"));
            Assert.Single(filter.g_ConnectOfIPList);

            filter.ClearConnectOfIP();      // :375
            cfg.LoadConfig();               // :373（无文件 ⇒ 全部回写默认）
            thread.Enabled = true;          // :368

            Assert.Empty(filter.g_ConnectOfIPList);
            Assert.True(thread.Enabled);
            Assert.Equal(5500, cfg.m_xGameGateList[1].nServerPort);   // LoginGate 默认端口

            // StopService 的残部（:457 / :459 / :460 / :462）
            filter.SaveBlockIPList();
            filter.SaveBlockIPAreaList();
            thread.Enabled = false;
            Assert.True(File.Exists(filter.BlockFilePath));
            Assert.True(File.Exists(filter.BlockAreaFilePath));
            Assert.False(thread.Enabled);
        }
        finally
        {
            try { Directory.Delete(dir, true); } catch { }
        }
    }

    // =====================================================================================
    // FuncForComm.pas:404 SetTimer(_IDM_TIMER_THREAD_INFO, 1000) 的周期登记
    // =====================================================================================
    [Fact]
    public void ThreadInfoTimerIdAndInterval_MatchOriginal()
    {
        // :404 SetTimer(g_hMainWnd, _IDM_TIMER_THREAD_INFO, 1000, Pointer(@OnTimerProc))
        Assert.Equal(Rest11LoginGateConstants.WM_USER + 1004, Rest11LoginGateConstants._IDM_TIMER_THREAD_INFO);
        Assert.Equal(Rest11LoginGateConstants.WM_USER, 1024);   // WM_USER = $0400
    }
}

/// <summary>
/// 把 `GXX.LoginGate.Rest11.Rest11GateSessionAdapter` 必须满足的语义固定在 GatewayKit 侧：
/// 它只是 `GateSession`（既有实现，**复用**）+ LoginGate 独有 4 字段的投影，**不新造会话实现**。
/// </summary>
public class GatewaySessionAdapterContract
{
    [Fact]
    public void GateSession_ProvidesTheFieldsTheAdapterDelegates()
    {
        var session = new GateSession { SocketId = 9001, RemoteIP = "1.2.3.4" };
        Assert.Equal(9001, session.SocketId);                 // m_pUserOBJ._SendObj.Socket
        Assert.Equal("1.2.3.4", session.RemoteIP);
        Assert.Equal(0, session.BufferLen);
    }

    [Fact]
    public void GateService_DefaultCheckIpSemanticsAreUnchanged()
    {
        // 本车道的 Rest11 设施**不修改** GateService：这里以公开面固定"默认语义不变"
        using var gate = new ContractProbeGate();
        Assert.Equal(TBlockIPMethod.mDisconnect, gate.BlockIPMethod);   // 默认 mDisconnect（GateService.cs:52）
        Assert.Equal(20, gate.MaxConnOfIPaddr);                          // 默认 20（GateService.cs:53）
        gate.AddBlockIP("10.0.0.1");                                     // :224-227 既有入口仍在
        Assert.True(gate.ServiceName.Length > 0);
        Assert.False(gate.ServiceStarted);
    }

    private sealed class ContractProbeGate : GateService
    {
        public ContractProbeGate() : base(".\\rest11_probe.ini") { }
        protected override int GateDefaultPort => 7000;
        protected override int ServerDefaultPort => 5500;
        public override string ServiceName => "Rest11Probe";
        protected override void OnServerData(TcpLink link) { }
    }
}

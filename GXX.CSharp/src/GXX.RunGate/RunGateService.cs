using System;
using System.Collections.Concurrent;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.GatewayKit;

namespace GXX.RunGate;

/// <summary>
/// RunGate.pas（uFrmMain/MirClientContext/GateShare）→ RunGateService.cs
/// 游戏数据网关：客户端 ↔ 本网关 ↔ M2Server。
/// 与 LoginGate/SelGate 同构（三网关共用 IOCP 骨架），附加 RunGate 特有：
/// 通信节流规则（PacketRule）、Grobal2_Ex 扩展命令、GM_RUN_GATE_VER 上报。
/// </summary>
public class RunGateService : GateService
{
    private readonly ConcurrentDictionary<int, GateSession> _sessions = new();

    // ---- GateShare.pas 配置 ----
    public string ServerName { get; set; } = "GXX";
    public int CheckServerTick { get; set; } = 5000;

    // 节流（PacketRuleConfig：超过阈值断开）
    public int MaxClientPacketCount { get; set; } = 1200;      // 时间窗内最大包数
    public uint ClientPacketSpeedInterval { get; set; } = 1000; // 时间窗（毫秒）

    public int SessionCount => _sessions.Count;

    public RunGateService() : base(@".\Config.ini")
    {
    }

    protected override int GateDefaultPort => 7200;   // 原默认游戏网关端口
    protected override int ServerDefaultPort => 5600; // M2Server 网关接受端口（RunSock）
    public override string ServiceName => "游戏网关";

    protected override void OnClientAccept(GateSession session)
    {
        base.OnClientAccept(session);
        session.PacketCount = 0;
        session.PacketSpeedTick = DelphiRTL.GetTickCount();
        _sessions[session.SocketId] = session;
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_OPEN, 0, null, 0));
    }

    protected override void OnClientDisconnect(GateSession session)
    {
        base.OnClientDisconnect(session);
        if (_sessions.TryRemove(session.SocketId, out _))
            SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_CLOSE, 0, null, 0));
    }

    protected override void OnClientReceive(GateSession session, byte[] buf, int offset, int len)
    {
        // 节流检测（对应 PacketRuleConfig/uFrmGameSpeed）
        uint now = DelphiRTL.GetTickCount();
        if (now - session.PacketSpeedTick >= ClientPacketSpeedInterval)
        {
            session.PacketSpeedTick = now;
            session.PacketCount = 0;
        }
        session.PacketCount++;
        if (MaxClientPacketCount > 0 && session.PacketCount > MaxClientPacketCount)
        {
            SendLog($"客户端 #{session.SocketId} 超速，强制断开", 2);
            ClientIocp.CloseSession(session);
            return;
        }

        base.OnClientReceive(session, buf, offset, len);
        var data = new byte[len];
        Array.Copy(buf, offset, data, 0, len);
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_DATA, 0, data, len));
    }

    protected override void SendServerOpenGate()
    {
        // 对应 GateShare：网关版本上报 + 心跳注册
        SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_RUN_GATE_VER, 0, null, 0));
        SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
    }

    protected override void OnServerData(TcpLink link)
    {
        while (link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
        {
            var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(link.AccumBuffer, 0);
            if (header.Flag != GatewayProtocol.RUNGATECODE)
            {
                SendLog("M2 帧校验失败，重置链路缓冲", 1);
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            int dataLen = header.DataLen;
            if (dataLen < 0 || dataLen > 256 * 1024)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            if (link.AccumLength < GatewayProtocol.SizeOfTSvrCmdPack + dataLen)
                return;

            if (dataLen > 0)
            {
                byte[] data = new byte[dataLen];
                Array.Copy(link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, data, 0, dataLen);
                if (_sessions.TryGetValue((int)header.SockID, out var session))
                    ClientIocp.Send(session, data);
            }
            else if (header.Cmd == GatewayProtocol.GM_KICK || header.Cmd == GatewayProtocol.GM_CLOSE)
            {
                if (_sessions.TryGetValue((int)header.SockID, out var sClose))
                    ClientIocp.CloseSession(sClose);
            }
            link.ConsumeAccum(GatewayProtocol.SizeOfTSvrCmdPack + dataLen);
        }
    }

    public void SendKeepAlive()
    {
        if (_serverConnected)
            SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
    }
}

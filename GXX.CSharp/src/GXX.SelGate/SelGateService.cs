using System;
using System.Collections.Concurrent;
using GXX.Core.Protocol;
using GXX.GatewayKit;

namespace GXX.SelGate;

/// <summary>
/// SelGate.pas → SelGateService.cs
/// 角色选择网关：客户端 ↔ 本网关 ↔ DBServer。
/// 与 LoginGate 同构（AppMain/ClientSession/ClientThread 三网关共用骨架），上游为 DBServer。
/// </summary>
public class SelGateService : GateService
{
    private readonly ConcurrentDictionary<int, GateSession> _sessions = new();

    public int SessionCount => _sessions.Count;

    public SelGateService() : base(@".\Config.ini")
    {
    }

    protected override int GateDefaultPort => 7100;   // 原默认角色网关端口
    protected override int ServerDefaultPort => 5100; // DBServer 网关接受端口
    public override string ServiceName => "角色网关";

    protected override void OnClientAccept(GateSession session)
    {
        base.OnClientAccept(session);
        _sessions[session.SocketId] = session;
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_OPEN, 0, null, 0));
        SendLog($"新客户端连接 #{session.SocketId} ({session.RemoteIP})");
    }

    protected override void OnClientDisconnect(GateSession session)
    {
        base.OnClientDisconnect(session);
        if (_sessions.TryRemove(session.SocketId, out _))
        {
            SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_CLOSE, 0, null, 0));
            SendLog($"客户端断开 #{session.SocketId}");
        }
    }

    protected override void OnClientReceive(GateSession session, byte[] buf, int offset, int len)
    {
        base.OnClientReceive(session, buf, offset, len);
        var data = new byte[len];
        Array.Copy(buf, offset, data, 0, len);
        // 透传 CM_QUERYCHR/CM_NEWCHR/CM_DELCHR/CM_SELCHR 等 → DBServer
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_DATA, 0, data, len));
    }

    protected override void SendServerOpenGate()
        => SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));

    protected override void OnServerData(TcpLink link)
    {
        while (link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
        {
            var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(link.AccumBuffer, 0);
            if (header.Flag != GatewayProtocol.RUNGATECODE)
            {
                SendLog("上游帧 Flag 校验失败，重置链路缓冲", 1);
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            int dataLen = header.DataLen;
            if (dataLen < 0 || dataLen > 64 * 1024)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            if (link.AccumLength < GatewayProtocol.SizeOfTSvrCmdPack + dataLen)
                return;

            if (dataLen > 0 && _sessions.TryGetValue((int)header.SockID, out var session))
            {
                byte[] data = new byte[dataLen];
                Array.Copy(link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, data, 0, dataLen);
                ClientIocp.Send(session, data);
            }
            else if (header.Cmd == GatewayProtocol.GM_CLOSE && _sessions.TryGetValue((int)header.SockID, out var sClose))
            {
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.GatewayKit;

namespace GXX.LoginGate;

/// <summary>
/// LoginGate.pas → LoginGateService.cs
/// 登录网关服务：客户端 ↔ 本网关 ↔ LoginSrv。
/// - 客户端方向：TCmdPack(TDefaultMessage) 6-Bit 编码帧（CM_* 消息）。
/// - 服务器方向：TSvrCmdPack 帧（RUNGATECODE + GM_* 命令），含会话打开/关闭/转发。
/// 对应原 AppMain.pas + ClientSession.pas + ClientThread.pas + FuncForComm.pas。
/// </summary>
public class LoginGateService : GateService
{
    // 会话表：SockID → 会话（对应 ClientSession.pas 的 TSessionInfo 列表）
    private readonly ConcurrentDictionary<int, GateSession> _sessions = new();
    private long _seq;

    public int SessionCount => _sessions.Count;

    public LoginGateService() : base(@".\Config.ini")
    {
    }

    protected override int GateDefaultPort => 7000;   // 原默认登录网关端口
    protected override int ServerDefaultPort => 5600; // LoginSrv 网关接受端口
    public override string ServiceName => "登录网关";

    // ---------------- 客户端方向 ----------------

    protected override void OnClientAccept(GateSession session)
    {
        base.OnClientAccept(session);
        _sessions[session.SocketId] = session;
        // 通知 LoginSrv 打开会话（SS_OPENSESSION）
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_OPEN, 0, null, 0));
        SendLog($"新客户端连接 #{session.SocketId} ({session.RemoteIP})");
    }

    protected override void OnClientDisconnect(GateSession session)
    {
        base.OnClientDisconnect(session);
        if (_sessions.TryRemove(session.SocketId, out _))
        {
            // 通知 LoginSrv 关闭会话（SS_CLOSESESSION）
            SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_CLOSE, 0, null, 0));
            SendLog($"客户端断开 #{session.SocketId}");
        }
    }

    protected override void OnClientReceive(GateSession session, byte[] buf, int offset, int len)
    {
        base.OnClientReceive(session, buf, offset, len);
        // 透传客户端数据 → LoginSrv（GM_DATA 帧）
        var data = new byte[len];
        Array.Copy(buf, offset, data, 0, len);
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_DATA, 0, data, len));
    }

    // ---------------- 服务器方向 ----------------

    protected override void SendServerOpenGate()
    {
        // 对应 ClientThread.pas 连接成功后发送的网关注册信息（GM_CHECKSERVER 心跳由计时器周期发送）
        SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
    }

    protected override void OnServerData(TcpLink link)
    {
        // 帧格式：TSvrCmdPack(20B) + Data
        while (link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
        {
            var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(link.AccumBuffer, 0);
            if (header.Flag != GatewayProtocol.RUNGATECODE)
            {
                // 流失步：丢弃缓冲（对应原网关 Flag 校验失败断链）
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
                return; // 等待更多数据

            if (dataLen > 0)
            {
                byte[] data = new byte[dataLen];
                Array.Copy(link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, data, 0, dataLen);
                DeliverToClient((int)header.SockID, header.Cmd, data);
            }
            else
            {
                DeliverToClient((int)header.SockID, header.Cmd, null);
            }
            link.ConsumeAccum(GatewayProtocol.SizeOfTSvrCmdPack + dataLen);
        }
    }

    private void DeliverToClient(int sockId, ushort cmd, byte[]? data)
    {
        switch (cmd)
        {
            case GatewayProtocol.GM_CLOSE:
                // 服务器要求断开该客户端
                if (_sessions.TryGetValue(sockId, out var sClose))
                    ClientIocp.CloseSession(sClose);
                break;
            case GatewayProtocol.GM_KICK:
                if (_sessions.TryGetValue(sockId, out var sKick))
                    ClientIocp.CloseSession(sKick);
                break;
            case GatewayProtocol.GM_DATA:
            default:
                if (data != null && data.Length > 0 && _sessions.TryGetValue(sockId, out var sData))
                    ClientIocp.Send(sData, data);
                break;
        }
    }

    /// <summary>GM_CHECKSERVER 心跳（原 _IDM_TIMER_KEEP_ALIVE）。</summary>
    public void SendKeepAlive()
    {
        if (_serverConnected)
            SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
    }
}

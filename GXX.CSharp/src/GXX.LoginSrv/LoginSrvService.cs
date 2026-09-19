using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.GatewayKit;

namespace GXX.LoginSrv;

/// <summary>
/// LoginSrv.pas（LMain/MasSock/Parse）→ LoginSrvService.cs
/// 登录服务器：接受 LoginGate 连接（帧协议），处理 CM_* 账号消息（2000 系列），返回 SM_* 结果。
/// 对应 Parse.pas 的账号命令解析 + AccountDB 查询 + 会话管理（SS_*）。
/// </summary>
public class LoginSrvService : IDisposable
{
    private readonly AccountDatabase _db;
    private readonly ConcurrentDictionary<int, TcpLink> _gateLinks = new();
    private Socket? _listenSocket;
    private Thread? _acceptThread;
    private volatile bool _running;

    public int GatePort { get; set; } = 5600;
    public bool ServiceStarted => _running;
    public int GateCount => _gateLinks.Count;

    public event Action<string, int>? OnLogMsg;

    // 会话（对应 LSShare.pas 的 TSessionInfo 列表：网关侧用户会话）
    public class TSessionInfo
    {
        public int GateIdx;
        public uint SockId;
        public string Account = "";
        public string IPaddr = "";
        public uint SessionId;
        public bool boStartPlay;
        public uint dwTick;
    }

    private readonly ConcurrentDictionary<uint, TSessionInfo> _sessions = new();
    private int _nextSessionId = 1;

    public LoginSrvService(string dbFile)
    {
        _db = new AccountDatabase(dbFile);
    }

    public bool StartService()
    {
        if (_running) return true;
        try
        {
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, GatePort));
            _listenSocket.Listen(16);
            _running = true;
            _acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "LoginSrv-Accept" };
            _acceptThread.Start();
            SendLog($"登录服务器启动，监听网关端口 {GatePort}");
            return true;
        }
        catch (Exception ex)
        {
            SendLog("启动失败: " + ex.Message, 1);
            return false;
        }
    }

    public void StopService()
    {
        _running = false;
        try { _listenSocket?.Close(); } catch { }
        foreach (var kv in _gateLinks)
            kv.Value.Close();
        _gateLinks.Clear();
        _sessions.Clear();
    }

    private void AcceptLoop()
    {
        while (_running)
        {
            try
            {
                var client = _listenSocket!.Accept();
                var link = new TcpLink(client);
                int idx = _gateLinks.Count + 1;
                link.OnReceive += (buf, off, len) =>
                {
                    link.Accumulate(buf, off, len);
                    ProcessGateData(link, idx);
                };
                link.OnDisconnected += () =>
                {
                    _gateLinks.TryRemove(idx, out _);
                    SendLog($"网关 #{idx} 断开");
                };
                _gateLinks[idx] = link;
                SendLog($"网关 #{idx} 已连接 ({((IPEndPoint)client.RemoteEndPoint!).Address})");
            }
            catch (SocketException) when (!_running) { return; }
            catch (ThreadInterruptedException) { return; }
            catch { }
        }
    }

    // ---------------- 帧解析与分发（对应 MasSock.pas + Parse.pas）----------------

    private void ProcessGateData(TcpLink link, int gateIdx)
    {
        while (link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
        {
            var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(link.AccumBuffer, 0);
            if (header.Flag != GatewayProtocol.RUNGATECODE)
            {
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

            byte[]? data = null;
            if (dataLen > 0)
            {
                data = new byte[dataLen];
                Array.Copy(link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, data, 0, dataLen);
            }
            link.ConsumeAccum(GatewayProtocol.SizeOfTSvrCmdPack + dataLen);

            switch (header.Cmd)
            {
                case GatewayProtocol.GM_OPEN:
                    SendLog($"会话打开 #{header.SockID}");
                    break;
                case GatewayProtocol.GM_CLOSE:
                    if (_sessions.TryGetValue(header.SockID, out var s))
                        _sessions.TryRemove(header.SockID, out _);
                    break;
                case GatewayProtocol.GM_CHECKSERVER:
                    // 心跳回应
                    SendToGate(link, GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
                    break;
                case GatewayProtocol.GM_DATA:
                    if (data != null)
                        ProcessClientPacket(link, header.SockID, data);
                    break;
            }
        }
    }

    /// <summary>解析客户端 6-Bit 帧（TDefaultMessage + 可选文本体）→ CM_* 处理。</summary>
    private void ProcessClientPacket(TcpLink link, uint sockId, byte[] data)
    {
        if (data.Length < EDcode.GetEncodeSize(TDefaultMessage.SizeOf)) return;
        TDefaultMessage msg = EDcode.DecodeMessage(data);
        // 剩余为文本体（GetEncodeSize(DefMsg) 之后的 DefStr 部分）
        int msgLen = EDcode.GetEncodeSize(TDefaultMessage.SizeOf);
        string body = "";
        if (data.Length > msgLen)
        {
            byte[] rest = new byte[data.Length - msgLen];
            Array.Copy(data, msgLen, rest, 0, rest.Length);
            body = EncodingInit.GBK.GetString(EDcode.DecodeBuffer(rest, rest.Length));
        }

        byte[] reply;
        switch (msg.Ident)
        {
            case 2000: // CM_PROTOCOL: 客户端版本协商
                reply = MakeReply(sockId, msg, 529 /* SM_PASSOK_SELECTSERVER */, "1");
                break;

            case 2001: // CM_IDPASSWORD 登录
            {
                string[] parts = body.Split(new[] { '\t', '/', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string account = parts.Length > 0 ? parts[0] : "";
                string password = parts.Length > 1 ? parts[1] : "";
                reply = HandleLogin(sockId, msg, account, password);
                break;
            }

            case 2002: // CM_ADDNEWUSER 注册
            {
                // body: account/password/username ... (Parse.pas GetDefMsg 结构)
                var parts = body.Split(new[] { '\r', '\n', '/', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]))
                {
                    var info = new TAccountInfo();
                    info.AccountNameStr = parts[0].Length > 14 ? parts[0][..14] : parts[0];
                    info.PasswordStr = parts[1].Length > 10 ? parts[1][..10] : parts[1];
                    if (_db.Add(info))
                        reply = MakeReply(sockId, msg, 504 /* SM_NEWID_SUCCESS */, "");
                    else
                        reply = MakeReply(sockId, msg, 505 /* SM_NEWID_FAIL */, "");
                }
                else
                    reply = MakeReply(sockId, msg, 505, "");
                break;
            }

            case 2003: // CM_CHANGEPASSWORD
            {
                var parts = body.Split(new[] { '\r', '\n', '/', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int ret = parts.Length >= 3 ? _db.ChangePassword(parts[0], parts[1], parts[2]) : 0;
                reply = MakeReply(sockId, msg, (ushort)(ret == 1 ? 506 /* SM_CHGPASSWD_SUCCESS */ : 507 /* SM_CHGPASSWD_FAIL */), "");
                break;
            }

            case 100: // CM_QUERYCHR：查询角色（登录成功后进入选择服流程）
                reply = MakeReply(sockId, msg, 529 /* SM_PASSOK_SELECTSERVER */, "");
                break;

            default:
                // 未知命令回应（对应 UNKNOWMSG）
                reply = MakeReply(sockId, msg, (ushort)CommonConst.UNKNOWMSG, "");
                break;
        }

        SendToGate(link, GatewayProtocol.BuildServerPacket(sockId, GatewayProtocol.GM_DATA, 0, reply, reply.Length));
    }

    private byte[] HandleLogin(uint sockId, in TDefaultMessage msg, string account, string password)
    {
        if (string.IsNullOrEmpty(account))
            return MakeReply(sockId, msg, 503 /* SM_PASSWD_FAIL */, "");

        TAccountInfo? found = _db.Find(account);
        if (found == null)
            return MakeReply(sockId, msg, 503, ""); // 账号不存在

        TAccountInfo info = found.Value;
        if (info.IsDisable != 0)
            return MakeReply(sockId, msg, 503, "");

        if (!string.Equals(info.PasswordStr, password, StringComparison.Ordinal))
        {
            info.ErrorCount++;
            _db.Update(info);
            return MakeReply(sockId, msg, 503, "");
        }

        // 登录成功：建立会话（对应 GrobalSession.pas 的 TGlobaSessionInfo）
        info.LoginDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        info.LastActionTick = DelphiRTL.GetTickCount();
        info.ErrorCount = 0;
        _db.Update(info);

        var session = new TSessionInfo
        {
            GateIdx = 0,
            SockId = sockId,
            Account = account,
            SessionId = (uint)Interlocked.Increment(ref _nextSessionId),
            dwTick = DelphiRTL.GetTickCount()
        };
        _sessions[sockId] = session;

        // SM_PASSOK_SELECTSERVER：密码正确，开始选服
        return MakeReply(sockId, msg, 529, session.SessionId.ToString());
    }

    private static byte[] MakeReply(uint sockId, in TDefaultMessage req, ushort smIdent, string bodyText)
    {
        var replyMsg = TDefaultMessage.Make(smIdent, req.Recog, req.Param, req.Tag, req.Series);
        byte[] head = EDcode.EncodeMessage(replyMsg);
        if (bodyText.Length == 0)
            return head;
        byte[] body = EDcode.EncodeString(bodyText);
        byte[] buf = new byte[head.Length + body.Length];
        Array.Copy(head, buf, head.Length);
        Array.Copy(body, 0, buf, head.Length, body.Length);
        return buf;
    }

    private void SendToGate(TcpLink link, byte[] data) => link.Send(data);

    private void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    public void Dispose()
    {
        StopService();
        _db.Dispose();
    }
}

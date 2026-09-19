using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.GatewayKit;
using GXX.GatewayKit;

namespace GXX.DBServer;

/// <summary>
/// DBServer.pas（uFrmMain/SelectClient/IDSocCli/RoleDB）→ DBServerService.cs
/// 数据库服务器：
/// - 网关端口：接受 SelGate 链接，处理 CM_QUERYCHR/CM_NEWCHR/CM_DELCHR/CM_SELCHR（6-Bit TDefaultMessage 帧）。
/// - 数据端口：接受 M2Server 链接，处理 DB_LOADHUMANRCD / DB_SAVEHUMANRCD（TDBMsgHeader 帧）。
/// </summary>
public class DBServerService : IGateUiService
{
    private readonly RoleDatabase _roles;
    private Socket? _gateListener;
    private Socket? _m2Listener;
    private Thread? _gateThread;
    private Thread? _m2Thread;
    private volatile bool _running;

    private readonly ConcurrentDictionary<int, TcpLink> _gateLinks = new();
    private readonly ConcurrentDictionary<int, TcpLink> _m2Links = new();

    public int GatePort { get; set; } = 5100;
    public int M2Port { get; set; } = 6000;
    public bool ServiceStarted => _running;
    public int OnlineCount => _gateLinks.Count + _m2Links.Count;
    public string ServiceName => "数据库服务器";
    public string GateAddr => "0.0.0.0";
    public string ServerAddr => "-";

    public event Action<string, int>? OnLogMsg;

    public DBServerService(string dbFile)
    {
        _roles = new RoleDatabase(dbFile);
    }

    public RoleDatabase Roles => _roles;

    public int ServerPort => M2Port;

    public bool StartService()
    {
        if (_running) return true;
        try
        {
            _gateListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _gateListener.Bind(new IPEndPoint(IPAddress.Any, GatePort));
            _gateListener.Listen(16);

            _m2Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _m2Listener.Bind(new IPEndPoint(IPAddress.Any, M2Port));
            _m2Listener.Listen(16);

            _running = true;
            _gateThread = new Thread(GateAcceptLoop) { IsBackground = true, Name = "DBServer-Gate" };
            _gateThread.Start();
            _m2Thread = new Thread(M2AcceptLoop) { IsBackground = true, Name = "DBServer-M2" };
            _m2Thread.Start();

            SendLog($"数据库服务器启动（SelGate 端口 {GatePort}，M2 数据端口 {M2Port}）");
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
        try { _gateListener?.Close(); } catch { }
        try { _m2Listener?.Close(); } catch { }
        foreach (var kv in _gateLinks) kv.Value.Close();
        foreach (var kv in _m2Links) kv.Value.Close();
        _gateLinks.Clear();
        _m2Links.Clear();
    }

    // ---------------- SelGate 端 ----------------

    private void GateAcceptLoop()
    {
        while (_running)
        {
            try
            {
                var client = _gateListener!.Accept();
                var link = new TcpLink(client);
                int idx = _gateLinks.Count + 1;
                link.OnReceive += (buf, off, len) =>
                {
                    link.Accumulate(buf, off, len);
                    ProcessGateData(link);
                };
                link.OnDisconnected += () => _gateLinks.TryRemove(idx, out _);
                _gateLinks[idx] = link;
            }
            catch { if (!_running) return; }
        }
    }

    private void ProcessGateData(TcpLink link)
    {
        while (link.AccumLength >= 22)
        {
            TDefaultMessage msg = EDcode.DecodeMessage(link.AccumBuffer, TDefaultMessage.SizeOf);
            // 解析 body（若帧长足够）
            int headEnc = EDcode.GetEncodeSize(TDefaultMessage.SizeOf);
            string body = "";
            // 单帧长度未知，按 SelGate 逐包转发特性：一收即一帧
            byte[] whole = new byte[link.AccumLength];
            Array.Copy(link.AccumBuffer, whole, link.AccumLength);
            link.ConsumeAccum(link.AccumLength);

            TDefaultMessage reply;
            switch (msg.Ident)
            {
                case 100: // CM_QUERYCHR
                {
                    string account = ExtractBodyText(whole, headEnc);
                    byte[] list = new byte[4096];
                    int n = _roles.QueryChr(account, list);
                    reply = TDefaultMessage.Make(520 /* SM_QUERYCHR */, msg.Recog, (ushort)(n > 0 ? 1 : 0), 0, (ushort)Math.Min(n, 16));
                    SendReply(link, reply, list.AsSpan(0, n).ToArray());
                    break;
                }
                case 101: // CM_NEWCHR
                {
                    string bodyText = ExtractBodyText(whole, headEnc);
                    string[] parts = bodyText.Split('/');
                    bool ok = parts.Length >= 3 && _roles.NewChr(parts[0], parts[1], ToByte(parts[2]), 0);
                    reply = TDefaultMessage.Make(ok ? (ushort)521 /* SM_NEWCHR_SUCCESS */ : (ushort)522 /* SM_NEWCHR_FAIL */, msg.Recog, 0, 0, 0);
                    SendReply(link, reply, null);
                    break;
                }
                case 102: // CM_DELCHR
                {
                    string bodyText = ExtractBodyText(whole, headEnc);
                    string[] parts = bodyText.Split('/');
                    bool ok = parts.Length >= 2 && _roles.DelChr(parts[0], parts[1]);
                    reply = TDefaultMessage.Make(ok ? (ushort)523 /* SM_DELCHR_SUCCESS */ : (ushort)524 /* SM_DELCHR_FAIL */, msg.Recog, 0, 0, 0);
                    SendReply(link, reply, null);
                    break;
                }
                case 103: // CM_SELCHR：选择角色 → 返回角色数据
                {
                    string bodyText = ExtractBodyText(whole, headEnc);
                    string[] parts = bodyText.Split('/');
                    var data = parts.Length >= 2 ? _roles.LoadHum(parts[0], parts[1]) : null;
                    if (data != null)
                    {
                        reply = TDefaultMessage.Make(525 /* SM_STARTPLAY */, msg.Recog, 0, 0, 0);
                        SendReply(link, reply, StructBytes.BytesOf(data.Value));
                    }
                    else
                    {
                        reply = TDefaultMessage.Make(526 /* SM_STARTFAIL */, msg.Recog, 0, 0, 0);
                        SendReply(link, reply, null);
                    }
                    break;
                }
                default:
                    // 未知命令（对应原 DBServer 的 UNKNOWMSG 处理）
                    reply = TDefaultMessage.Make((ushort)CommonConst.UNKNOWMSG, msg.Recog, 0, 0, 0);
                    SendReply(link, reply, null);
                    break;
            }
        }
    }

    private static byte ToByte(string s) => (byte)GXX.Core.Rtl.DelphiRTL.StrToIntDef(s, 0);

    private static string ExtractBodyText(byte[] whole, int headLen)
    {
        if (whole.Length <= headLen) return "";
        byte[] rest = new byte[whole.Length - headLen];
        Array.Copy(whole, headLen, rest, 0, rest.Length);
        return EncodingInit.GBK.GetString(EDcode.DecodeBuffer(rest, rest.Length));
    }

    private void SendReply(TcpLink link, in TDefaultMessage msg, byte[]? body)
    {
        byte[] head = EDcode.EncodeMessage(msg);
        if (body == null || body.Length == 0)
        {
            link.Send(head);
            return;
        }
        byte[] encBody = EDcode.EncodeBuffer(body, body.Length);
        byte[] buf = new byte[head.Length + encBody.Length];
        Array.Copy(head, buf, head.Length);
        Array.Copy(encBody, 0, buf, head.Length, encBody.Length);
        link.Send(buf);
    }

    // ---------------- M2Server 数据端 ----------------

    private void M2AcceptLoop()
    {
        while (_running)
        {
            try
            {
                var client = _m2Listener!.Accept();
                var link = new TcpLink(client);
                int idx = _m2Links.Count + 1;
                link.OnReceive += (buf, off, len) =>
                {
                    link.Accumulate(buf, off, len);
                    ProcessM2Data(link);
                };
                link.OnDisconnected += () => _m2Links.TryRemove(idx, out _);
                _m2Links[idx] = link;
                SendLog("M2Server 已连接数据端口");
            }
            catch { if (!_running) return; }
        }
    }

    private void ProcessM2Data(TcpLink link)
    {
        // M2 数据协议：TDBMsgHeader(dwCode=$AA55AA00+idx, dwCrc, DefMsg: TDefaultMessage, nLength) + Data
        int headSize = StructBytes.SizeOf<TDBMsgHeader>();
        while (link.AccumLength >= headSize)
        {
            var header = StructBytes.FromBytes<TDBMsgHeader>(link.AccumBuffer, 0);
            if (header.nLength < 0 || header.nLength > 4 * 1024 * 1024)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            if (link.AccumLength < headSize + header.nLength)
                return;

            byte[]? data = null;
            if (header.nLength > 0)
            {
                data = new byte[header.nLength];
                Array.Copy(link.AccumBuffer, headSize, data, 0, header.nLength);
            }
            link.ConsumeAccum(headSize + header.nLength);

            HandleM2Request(link, header, data);
        }
    }

    private void HandleM2Request(TcpLink link, TDBMsgHeader header, byte[]? data)
    {
        switch (header.DefMsg.Ident)
        {
            case CommonConst.DB_CHECKCONNECT:
            {
                SendM2Reply(link, header, CommonConst.DB_CHECKCONNECT, ((long)1).ToBytes());
                break;
            }
            case CommonConst.DB_LOADHUMANRCD:
            {
                if (data == null || data.Length < StructBytes.SizeOf<TDBLoadHuman>()) return;
                var req = StructBytes.FromBytes<TDBLoadHuman>(data);
                var hum = _roles.LoadHum(req.Account, req.HumanName);
                byte[] payload = hum != null ? StructBytes.BytesOf(hum.Value) : Array.Empty<byte>();
                SendM2Reply(link, header, CommonConst.DB_LOADHUMANRCD, payload);
                break;
            }
            case CommonConst.DB_SAVEHUMANRCD:
            {
                if (data == null || data.Length < 8 + StructBytes.SizeOf<THumData>()) return;
                // TDBSaveHuman: nSessionID(4) + PlayObject(8 语义上 TObject→Int64 但紧凑写法 4+4) — 兼容：按 M2 侧发送格式解析
                // 本实现采用 [nSessionID(4)][PlayObject(8)][THumData]
                int sid = BitConverter.ToInt32(data, 0);
                var hum = StructBytes.FromBytes<THumData>(data, 12);
                bool ok = _roles.SaveHum(hum.Account, hum.ChrName, hum);
                SendM2Reply(link, header, CommonConst.DB_SAVEHUMANRCD, ok ? new byte[] { 1 } : new byte[] { 0 });
                break;
            }
            case CommonConst.DB_LOADHERORCD:
            {
                if (data == null || data.Length < StructBytes.SizeOf<TDBLoadHero>()) return;
                var req = StructBytes.FromBytes<TDBLoadHero>(data);
                var hero = _roles.LoadHero(req.Account, req.HumanName);
                byte[] payload = hero != null ? StructBytes.BytesOf(hero.Value) : Array.Empty<byte>();
                SendM2Reply(link, header, CommonConst.DB_LOADHERORCD, payload);
                break;
            }
            case CommonConst.DB_SAVEHERORCD:
            {
                if (data == null || data.Length < 12 + StructBytes.SizeOf<THeroData>()) return;
                var hero = StructBytes.FromBytes<THeroData>(data, 12);
                bool ok = _roles.SaveHero(hero.Account, hero.ChrName, hero);
                SendM2Reply(link, header, CommonConst.DB_SAVEHERORCD, ok ? new byte[] { 1 } : new byte[] { 0 });
                break;
            }
        }
    }

    private void SendM2Reply(TcpLink link, TDBMsgHeader req, ushort ident, byte[] data)
    {
        var reply = new TDBMsgHeader
        {
            dwCode = req.dwCode,
            dwCrc = req.dwCrc,
            DefMsg = TDefaultMessage.Make(ident, req.DefMsg.Recog, 0, 0, 0),
            nLength = data.Length
        };
        byte[] head = StructBytes.BytesOf(reply);
        byte[] buf = new byte[head.Length + data.Length];
        Array.Copy(head, buf, head.Length);
        Array.Copy(data, 0, buf, head.Length, data.Length);
        link.Send(buf);
    }

    private void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    public void OnKeepAliveTimer() { }

    public void Dispose()
    {
        StopService();
        _roles.Dispose();
    }
}

internal static class BytesExt
{
    public static byte[] ToBytes(this long v) => BitConverter.GetBytes(v);
}

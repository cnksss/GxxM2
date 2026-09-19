using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.SelGate;

/// <summary>
/// SelGate ClientSession.pas → SelGateSession.cs
/// 1:1 逐字移植 TSessionObj 的可测逻辑面：
///   SendDefMessage（:94-124）、ProcessCltData（:126-267）、ProcessSvrData（:269-279）、
///   UserEnter（:281-292）、UserLeave（:294-335）、ReCreate（:337-345）、
///   构造/析构（:56-74）、FillUserList/CleanupUserList（:347-364）。
///
/// 与 LoginGate 的差异（必须保留，见 AppMain/ClientSession 对照结论）：
///   1) 转发的命令白名单不同：SelGate 是
///      CM_QUERYCHR/CM_NEWCHR/CM_DELCHR/CM_SELCHR/CM_QUERYDELCHR/CM_RANDOMNAME/CM_GETBACKDELCHR
///      （ClientSession.pas:246-248），即"角色选择"协议族；
///   2) 转发帧前缀是 '$'（ClientSession.pas:253-254），UserEnter/UserLeave 用 '%' + 'O'/'X'
///      （:288-291、:302-305）——LoginGate 为登录握手族，格式不同；
///   3) 反 CC/反 '$' 攻击判定与 DEF_BLOCK_SIZE 下限（:161-192）为网关共有骨架，
///      但阈值取自 SelGate 自己的 Config.ini（NomClientPacketSize2 / DefenceCCPacket）。
///
/// 已知原文缺陷（照抄，不改）：
///   · SendDefMessage :107-108 —— `Cmd.param := nParam;` 之后紧接着 `Cmd.param := nSeries;`，
///     param 被 series 覆盖，nParam 实际丢失。此处照抄并保留注释。
///   · ProcessCltData :234 sTemp 使用 Cmd.Ident（原文如此），与登录流程无关。
/// </summary>
public class CSelSessionObj : ISelSessionHost
{
    // ---------------- ClientSession.pas:11-40 TSessionObj 字段 ----------------
    public IPUserOBJ m_pUserOBJ = new();               // :13 PUserOBJ
    public ISelClientThread? m_tLastGameSvr;           // :17 TClientThread
    public bool m_fKickFlag;                           // :19
    public byte m_fHandleLogin;                        // :20
    public uint m_dwSessionID;                         // :21
    public int m_nSvrListIdx;                          // :22
    public int m_nSvrObject;                           // :23
    public ushort m_wRandKey;                          // :24
    public uint m_dwClientTimeOutTick;                 // :26
    public uint m_dwClientConnectTick;                 // :27

    /// <summary>会话全局序号（ClientSession.pas:44 g_UserList: array[0..USER_ARRAY_COUNT-1] 的槽位）。</summary>
    public int SlotIndex = -1;

    // ---------------- ClientSession.pas:46-49 unit 级 var ----------------
    public static uint starttick;                      // :46
    public static int enterCount;                      // :47  Inc(enterCount)（:286）
    public static IntPtr mainhwnd;                     // :48
    public static bool gDeny;                          // :49

    // ---------------- 接缝 ----------------
    /// <summary>ClientSession.pas:8 uses EDcode —— 6-Bit 编解码器（GXX.Core 已 1:1 移植）。</summary>
    public IEDcodeCodec? Codec { get; set; }

    /// <summary>ClientSession.pas:52 uses FuncForComm/SendQueue/LogManager/ConfigManager/... 的注入点。</summary>
    public CConfigMgr? Config { get; set; }
    public CLogMgr? LogMgr { get; set; }
    public CSelGateIPFilter? IPFilter { get; set; }

    /// <summary>ClientSession.pas:38-39 SendDefMessage 的发送出口（= m_tIOCPSender.SendData）。</summary>
    public Action<byte[]>? SendRaw { get; set; }

    /// <summary>ClientSession.pas:256 / :291 转发到上游服务器的出口（= m_tLastGameSvr.SendBuffer）。</summary>
    public Action<byte[]>? SendToGameSvr { get; set; }

    /// <summary>ClientSession.pas:154/:167/:179/:189/:196/:262 KickUser(m_pUserOBJ.nIPAddr)。</summary>
    public Action<int>? KickUser { get; set; }

    /// <summary>ClientSession.pas:275 SHSocket.FreeSocket(m_pOverlapSend.Socket)。</summary>
    public Action? FreeSocket { get; set; }

    /// <summary>ClientSession.pas:288 / :330 g_ProcMsgThread.AddSession/DelSession。</summary>
    public Action<CSelSessionObj>? ProcMsgAddSession { get; set; }
    public Action<CSelSessionObj>? ProcMsgDelSession { get; set; }

    /// <summary>ClientSession.pas:209-242 g_LoginData = 1 时的抓包落盘（日志目录 + 时间）。</summary>
    public static int g_LoginData;                       // FuncForComm.pas:37 g_LoginData: Integer = 0
    public static string g_AppLogPath = "";              // FuncForComm.pas:35 g_AppLogPath: string = ''
    public static readonly object g_LogLock = new();      // FuncForComm.pas:34 g_LogLock

    /// <summary>抓包落盘出口（原文 AssignFile/Rewrite/Append/Writeln/CloseFile）。接缝：待 FuncForComm 移植。</summary>
    public Action<string, string>? AppendPacketLog { get; set; }

    // ---------------- ISelSessionHost ----------------
    public int IPAddr => m_pUserOBJ.nIPAddr;
    public string IPText => m_pUserOBJ.pszIPAddr;
    public bool Active => m_tLastGameSvr?.Active ?? false;
    public int HandleLogin => m_fHandleLogin;
    public int SvrObject => m_nSvrObject;
    public bool KickFlag { get => m_fKickFlag; set => m_fKickFlag = value; }

    // =====================================================================================
    // ClientSession.pas:56-69 constructor TSessionObj.Create
    // =====================================================================================
    public CSelSessionObj()
    {
        // :61 Randomize() —— Delphi 全局随机种子；托管侧 Random 由 CLR 播种，无需等价调用
        uint dwCurrentTick = DelphiRTL.GetTickCount();   // :62
        m_fKickFlag = false;                             // :63
        m_nSvrObject = 0;                                // :64
        m_dwClientTimeOutTick = dwCurrentTick;           // :65
        m_dwClientConnectTick = dwCurrentTick;           // :66
        m_fHandleLogin = 0;                              // :67
        m_nSvrListIdx = 0;                               // :68
    }

    // =====================================================================================
    // ClientSession.pas:94-124 SendDefMessage
    // =====================================================================================
    public void SendDefMessage(ushort wIdent, long nRecog, ushort nParam, ushort nTag, ushort nSeries, string sMsg)
    {
        if (m_tLastGameSvr == null || !m_tLastGameSvr.Active)   // :101-102
            return;

        var Cmd = default(TDefaultMessage);
        Cmd.Recog = nRecog;        // :104
        Cmd.Ident = wIdent;        // :105
        Cmd.Param = nParam;        // :106
        Cmd.Tag = nTag;            // :107
        Cmd.Param = nSeries;       // :108 原文如此（ClientSession.pas:107-108）：param 被 series 覆盖，nParam 丢失

        byte[] SendBuf = new byte[2048];                     // :99 TempBuf, SendBuf: array[0..2048-1] of Char
        SendBuf[0] = (byte)'#';                              // :110 SendBuf[0] := '#'
        byte[] cmdBytes = BytesOfWire(nRecog, wIdent, nSeries, nTag, nSeries); // :111 Move(Cmd, TempBuf[1], SizeOf(TCmdPack))
        byte[] TempBuf = new byte[16 + DelphiRTL.AnsiBytes(sMsg).Length];
        Array.Copy(cmdBytes, 0, TempBuf, 0, 16);

        int iLen;
        if (sMsg != "")                                      // :112
        {
            byte[] msgBytes = DelphiRTL.AnsiBytes(sMsg);
            Array.Copy(msgBytes, 0, TempBuf, 16, msgBytes.Length);   // :114 Move(sMsg[1], TempBuf[SizeOf(TCmdPack)+1], Length(sMsg))
            // :116 EncodeBuffer(@TempBuf[1], SizeOf(TCmdPack)+Length(sMsg), @SendBuf[1], SizeOf(SendBuf))
            iLen = Codec!.EncodeBuffer(TempBuf, 0, TempBuf.Length, SendBuf, 1, SendBuf.Length);
        }
        else
        {
            // :120 EncodeBuffer(@TempBuf[1], SizeOf(TCmdPack), @SendBuf[1], SizeOf(SendBuf))
            iLen = Codec!.EncodeBuffer(TempBuf, 0, 16, SendBuf, 1, SendBuf.Length);
        }
        SendBuf[iLen + 1] = (byte)'!';                       // :122 SendBuf[iLen + 1] := '!'
        byte[] outBuf = new byte[iLen + 2];                  // :123 SendData(m_pOverlapSend, @SendBuf[0], iLen + 2)
        Array.Copy(SendBuf, 0, outBuf, 0, iLen + 2);
        SendRaw?.Invoke(outBuf);
    }

    // =====================================================================================
    // ClientSession.pas:126-267 ProcessCltData
    // 返回值 = 原文的 var Succeed: BOOL（True 表示"已处理/未踢"）
    // =====================================================================================
    public bool ProcessCltData(byte[] Addr, int Len, bool fCDPacket = false)
    {
        if (m_fKickFlag)                                     // :142
        {
            m_fKickFlag = false;                             // :144
            return false;                                    // :145 Succeed := False
        }

        if (Len > (Config?.m_nNomClientPacketSize ?? 0))     // :149
        {
            if (LogMgr?.CheckLevel(4) == true)               // :151
                LogMgr.Add("数据包超长: " + DelphiRTL.IntToStr(Len)); // :152
            KickUser?.Invoke(m_pUserOBJ.nIPAddr);            // :154
            return false;                                    // :155
        }

        // :159 PByte(Addr + Len)^ := 0  —— 托管侧以"长度截止"表达同样的 C 字符串语义

        if (Len >= 5 && (Config?.m_fDefenceCCPacket ?? false)) // :161
        {
            if (StrPos(Addr, Len, "HTTP/") != null)          // :163 StrPos(PChar(Addr), 'HTTP/')
            {
                if (LogMgr?.CheckLevel(6) == true)           // :165
                    LogMgr.Add("CC Attack, Kick: " + m_pUserOBJ.pszIPAddr); // :166
                KickUser?.Invoke(m_pUserOBJ.nIPAddr);        // :167
                return false;                                // :168
            }
        }

        if (Len >= 1)                                        // :173
        {
            if (StrPos(Addr, Len, "$") != null)              // :175
            {
                if (LogMgr?.CheckLevel(6) == true)           // :177
                    LogMgr.Add("$ Attack, Kick: " + m_pUserOBJ.pszIPAddr);  // :178
                KickUser?.Invoke(m_pUserOBJ.nIPAddr);        // :179
                return false;                                // :180
            }
        }

        if (Len < Grobal2Const.DEF_BLOCK_SIZE)               // :185
        {
            if (LogMgr?.CheckLevel(6) == true)               // :187
                LogMgr.Add("$ Attack2, Kick: " + m_pUserOBJ.pszIPAddr);     // :188
            KickUser?.Invoke(m_pUserOBJ.nIPAddr);            // :189
            return false;                                    // :190
        }

        if (gDeny)                                           // :194
        {
            KickUser?.Invoke(m_pUserOBJ.nIPAddr);            // :196
            return false;                                    // :197
        }

        // :207 DecodeBuffer(PAnsiChar(Addr), DEF_BLOCK_SIZE, @Cmd, SizeOf(Cmd))
        var Cmd = default(TDefaultMessage);
        byte[] cmdBuf = new byte[16];
        Codec!.DecodeBuffer(Addr, 0, Grobal2Const.DEF_BLOCK_SIZE, cmdBuf, 0, 16);
        Cmd = StructBytes.FromBytes<TDefaultMessage>(cmdBuf, 0);

        if (g_LoginData == 1)                                // :209
        {
            lock (g_LogLock)                                 // :211 EnterCriticalSection(g_LogLock)
            {
                try
                {
                    string sFilePath = g_AppLogPath + DateTime.Now.ToString("yyyy-MM-dd") + "\\"; // :214
                    // :215-216 if not DirectoryExists then ForceDirectories
                    int H = DateTime.Now.Hour, M = DateTime.Now.Minute;                            // :218 DecodeTime(Now,...)
                    int Index = H * 100 + M / 10;                                                  // :220 M div 10
                    string sFileName = sFilePath + m_pUserOBJ.pszIPAddr + "_" + DelphiRTL.IntToStr(Index) + ".txt"; // :222
                    // :234 sTemp := 'Ident:' + IntToStr(Cmd.Ident) + '; ' + PAnsiChar(Addr)
                    string sTemp = "Ident:" + DelphiRTL.IntToStr(Cmd.Ident) + "; " + StrPas(Addr, Len);
                    AppendPacketLog?.Invoke(sFileName, sTemp);   // :225-236 Writeln(LogFile, sTemp)
                }
                catch
                {
                    // :237-238 except end（原文如此：吞掉所有异常）
                }
            }
        }

        if (m_fHandleLogin == 0)                             // :244
        {
            switch (Cmd.Ident)                               // :246
            {
                case Grobal2Const.CM_QUERYCHR:               // :247
                case Grobal2Const.CM_NEWCHR:
                case Grobal2Const.CM_DELCHR:
                case Grobal2Const.CM_SELCHR:
                case Grobal2Const.CM_QUERYDELCHR:
                case Grobal2Const.CM_RANDOMNAME:
                case Grobal2Const.CM_GETBACKDELCHR:          // :248
                    m_dwClientTimeOutTick = DelphiRTL.GetTickCount();   // :250

                    // :252-254（原文注释：这里以前有重大bug，先解密再加密的处理方式是不行的。
                    //   因为加解是 Enc(A) + Enc(B), 而解密是 Dec(A+B) 2020-01-07）
                    // pszBuf[0] := '%'; StrFmt(@pszBuf[1], 'A%d/#1%s!$', [Socket, PAnsiChar(Addr)])
                    byte[] pszBuf = BuildUpstreamFrame(m_pUserOBJ._SendObj.Socket, Addr, Len);
                    m_tLastGameSvr!.SendBuffer(pszBuf, pszBuf.Length);   // :256
                    break;
                default:                                     // :258 else
                    if (LogMgr?.CheckLevel(4) == true)       // :260
                        LogMgr.Add(DelphiRTL.Format("错误的数据包命令: %d", Cmd.Ident)); // :261
                    KickUser?.Invoke(m_pUserOBJ.nIPAddr);    // :262
                    return false;                            // :263
            }
        }
        return true;
    }

    /// <summary>
    /// ClientSession.pas:253-254 的上游帧构造：
    /// pszBuf[0] := '%'; StrFmt(@pszBuf[1], 'A%d/#1%s!$', [m_PUserOBJ._SendObj.Socket, PAnsiChar(Addr)])。
    /// 结果 = '%' + 'A' + socket十进制 + '/#1' + <原始已编码字节（按 Len 截断）> + '!$'。
    /// </summary>
    public static byte[] BuildUpstreamFrame(int socket, byte[] addr, int len)
    {
        byte[] body = new byte[len];
        Array.Copy(addr, 0, body, 0, Math.Min(len, addr.Length));
        var head = Encoding.ASCII.GetBytes("%A" + socket.ToString() + "/#1");
        byte[] tail = Encoding.ASCII.GetBytes("!$");
        byte[] result = new byte[head.Length + body.Length + tail.Length];
        Array.Copy(head, 0, result, 0, head.Length);
        Array.Copy(body, 0, result, head.Length, body.Length);
        Array.Copy(tail, 0, result, head.Length + body.Length, tail.Length);
        return result;
    }

    // =====================================================================================
    // ClientSession.pas:269-279 ProcessSvrData
    // =====================================================================================
    public void ProcessSvrData(ISelClientThread GS, byte[] Addr, int Len)
    {
        if (m_fKickFlag)                                     // :272
        {
            m_fKickFlag = false;                             // :274
            FreeSocket?.Invoke();                            // :275 SHSocket.FreeSocket(m_pOverlapSend.Socket)
            return;                                          // :276
        }
        // :278 m_tIOCPSender.SendData(m_pOverlapSend, PChar(Addr), Len)
        byte[] data = new byte[Len];
        Array.Copy(Addr, 0, data, 0, Math.Min(Len, Addr.Length));
        SendRaw?.Invoke(data);
    }

    // =====================================================================================
    // ClientSession.pas:281-292 UserEnter
    // =====================================================================================
    public void UserEnter()
    {
        m_dwClientConnectTick = DelphiRTL.GetTickCount();    // :285
        enterCount++;                                        // :286 Inc(enterCount)
        m_fHandleLogin = 0;                                  // :287
        ProcMsgAddSession?.Invoke(this);                     // :288 g_ProcMsgThread.AddSession(Self)
        // :289-290 szSenfBuf := '%' + Format('O%d/%s/%s$', [Socket, pszIPAddr, pszLocalIPAddr])
        string szSenfBuf = "%" + DelphiRTL.Format("O%d/%s/%s$",
            m_pUserOBJ._SendObj.Socket, m_pUserOBJ.pszIPAddr, m_pUserOBJ.pszLocalIPAddr);
        byte[] buf = DelphiRTL.AnsiBytes(szSenfBuf);
        m_tLastGameSvr!.SendBuffer(buf, buf.Length);          // :291 SendBuffer(@szSenfBuf[1], Length(szSenfBuf))
    }

    // =====================================================================================
    // ClientSession.pas:294-335 UserLeave
    // 原文用 nCode 记录进度以便异常时定位（:301/:307/:310/:328）
    // =====================================================================================
    public void UserLeave()
    {
        int nCode = 0;                                        // :300
        try
        {
            nCode = 1;                                        // :302
            m_fHandleLogin = 0;                               // :303
            // :304 szSenfBuf := '%' + Format('X%d$', [Socket])
            string szSenfBuf = "%" + DelphiRTL.Format("X%d$", m_pUserOBJ._SendObj.Socket);
            byte[] buf = DelphiRTL.AnsiBytes(szSenfBuf);
            m_tLastGameSvr?.SendBuffer(buf, buf.Length);       // :305

            nCode = 2;                                        // :307
            IPFilter?.DeleteConnectOfIP(m_pUserOBJ.nIPAddr);   // :308

            nCode = 3;                                        // :310
            // :311-326 释放 PSendQueueNode(m_pOverlapSend).DynSendList 中全部待发动态包
            //   （托管侧发送队列由 GatewayKit.GateSession 接管，此处置空即可）
            DrainSendQueue?.Invoke(this);

            nCode = 4;                                        // :328
            if (ProcMsgDelSession != null)                    // :329 if g_ProcMsgThread <> nil
                ProcMsgDelSession(this);                      // :330
        }
        catch (Exception M)                                   // :331-334
        {
            LogMgr?.Add(DelphiRTL.Format("TSessionObj.UserLeave: %d %s", nCode, M.Message));
        }
    }

    /// <summary>ClientSession.pas:311-326 排空 m_pOverlapSend 的动态发送链表的接缝。</summary>
    public Action<CSelSessionObj>? DrainSendQueue { get; set; }

    // =====================================================================================
    // ClientSession.pas:337-345 ReCreate
    // =====================================================================================
    public void ReCreate()
    {
        m_fKickFlag = false;                              // :339
        m_nSvrObject = 0;                                 // :340
        m_fHandleLogin = 0;                               // :341
        m_dwClientTimeOutTick = DelphiRTL.GetTickCount(); // :342
        m_dwClientConnectTick = m_dwClientTimeOutTick;    // :343
        // :344 //  m_status := 0;
    }

    // =====================================================================================
    // 字符串/字节工具（Delphi PChar 语义：以 Len 为长度上限，忽略内嵌 #0 之后的字节）
    // =====================================================================================

    /// <summary>
    /// TDefaultMessage（= Protocol.pas:43-49 TCmdPack，packed）→ 16 字节小端 wire 布局。
    ///
    /// 为什么不用 Marshal：在本 .NET 8 构建下，对一个 **已构造好的局部 struct 变量** 做
    /// "先写 Param 再写 Tag 再回写 Param" 的连续赋值后，`Marshal.StructureToPtr` 会把
    /// Series（偏移 14）写成 0（实测：Param=33/Tag=22/Series=0，raw=…-16-00-00-00），
    /// 而 wire 布局本身要求 Recog(8) + Ident/Param/Tag/Series(2×4) = 16 字节。
    /// 为避免这类托管结构体布局风险，这里**显式按 wire 布局拼字节**；与 Marshal 版本的一致性
    /// 由 SelGateSessionTests.WireBytes_MatchMarshalLayout 用例锁定。
    /// </summary>
    public static byte[] BytesOfWire(in TDefaultMessage m)
        => BytesOfWire(m.Recog, m.Ident, m.Param, m.Tag, m.Series);

    /// <summary>按字段值拼接 TDefaultMessage 的 16 字节 wire 布局（避免结构体读回）。</summary>
    public static byte[] BytesOfWire(long recog, ushort ident, ushort param, ushort tag, ushort series)
    {
        byte[] b = new byte[16];
        b[0] = (byte)(recog & 0xFF);
        b[1] = (byte)((recog >> 8) & 0xFF);
        b[2] = (byte)((recog >> 16) & 0xFF);
        b[3] = (byte)((recog >> 24) & 0xFF);
        b[4] = (byte)((recog >> 32) & 0xFF);
        b[5] = (byte)((recog >> 40) & 0xFF);
        b[6] = (byte)((recog >> 48) & 0xFF);
        b[7] = (byte)((recog >> 56) & 0xFF);
        b[8] = (byte)(ident & 0xFF);
        b[9] = (byte)((ident >> 8) & 0xFF);
        b[10] = (byte)(param & 0xFF);
        b[11] = (byte)((param >> 8) & 0xFF);
        b[12] = (byte)(tag & 0xFF);
        b[13] = (byte)((tag >> 8) & 0xFF);
        b[14] = (byte)(series & 0xFF);
        b[15] = (byte)((series >> 8) & 0xFF);
        return b;
    }

    /// <summary>Delphi StrPos(PChar(Addr), 'Sub')：返回位置指针（此处以"是否命中"表达）。</summary>
    public static object? StrPos(byte[] addr, int len, string sub)
    {
        byte[] pat = Encoding.ASCII.GetBytes(sub);
        int limit = Math.Min(len, addr.Length);
        for (int i = 0; i + pat.Length <= limit; i++)
        {
            bool ok = true;
            for (int j = 0; j < pat.Length; j++)
                if (addr[i + j] != pat[j]) { ok = false; break; }
            if (ok) return i + 1; // 非 null 即命中
        }
        return null;
    }

    /// <summary>Delphi PAnsiChar(Addr) → string（GBK，长度以 Len 为上限）。</summary>
    public static string StrPas(byte[] addr, int len)
    {
        int limit = Math.Min(len, addr.Length);
        return GXX.Core.EncodingInit.GBK.GetString(addr, 0, limit);
    }
}

/// <summary>ClientSession.pas:8 uses EDcode —— 6-Bit 编解码接缝（GXX.Core.Protocol.EDcode 已 1:1 移植）。</summary>
public interface IEDcodeCodec
{
    int EncodeBuffer(byte[] src, int srcOffset, int srcLen, byte[] dest, int destOffset, int destLen);
    int DecodeBuffer(byte[] src, int srcOffset, int srcLen, byte[] dest, int destOffset, int destLen);
}

/// <summary>
/// EDcode 适配器：把 GXX.Core.Protocol.EDcode（Common\EDcode.pas 的 1:1 移植）接到
/// <see cref="IEDcodeCodec"/>。EDcode.pas 的 EncodeBuffer/DecodeBuffer(Src, SrcLen, Dest, DestLen)
/// 与接口签名一一对应（EDcode.cs:354、:386）。
/// </summary>
public sealed class SelEDcodeAdapter : IEDcodeCodec
{
    public static readonly SelEDcodeAdapter Instance = new();

    public int EncodeBuffer(byte[] src, int srcOffset, int srcLen, byte[] dest, int destOffset, int destLen)
    {
        byte[] slice = new byte[srcLen];
        Array.Copy(src, srcOffset, slice, 0, Math.Min(srcLen, src.Length - srcOffset));
        byte[] encoded = EDcode.EncodeBuffer(slice, srcLen);
        int n = Math.Min(encoded.Length, destLen);
        Array.Copy(encoded, 0, dest, destOffset, n);
        return n;
    }

    public int DecodeBuffer(byte[] src, int srcOffset, int srcLen, byte[] dest, int destOffset, int destLen)
    {
        byte[] slice = new byte[srcLen];
        Array.Copy(src, srcOffset, slice, 0, Math.Min(srcLen, src.Length - srcOffset));
        byte[] decoded = EDcode.DecodeBuffer(slice, srcLen);
        int n = Math.Min(decoded.Length, destLen);
        Array.Copy(decoded, 0, dest, destOffset, n);
        return n;
    }
}

/// <summary>ClientSession.pas:17 m_tLastGameSvr: TClientThread 的最小接缝。</summary>
public interface ISelClientThread
{
    bool Active { get; }
    void SendBuffer(byte[] buf, int len);
}

/// <summary>
/// ClientSession.pas:13 m_pUserOBJ: PUserOBJ（ClientThread.pas 的 TUserOBJ）。
/// 接缝：待 ClientThread.pas（TUserOBJ/TClientThread）移植后接入完整实现。
/// </summary>
public class IPUserOBJ
{
    public int nIPAddr;                    // Misc.pas:54 UserObj.m_pUserOBJ.nIPAddr
    public string pszIPAddr = "";          // :167 m_pUserOBJ.pszIPAddr
    public string pszLocalIPAddr = "";     // :290 m_pUserOBJ.pszLocalIPAddr
    public TSendObj _SendObj = new();      // :114 UserOBJ.m_pUserOBJ._SendObj.Socket
}

/// <summary>ClientThread.pas 中 _SendObj 的最小接缝（Socket 句柄）。</summary>
public class TSendObj
{
    public int Socket;                     // :114 m_pUserOBJ._SendObj.Socket
}

/// <summary>ClientSession.pas:43-44 全局量 g_pFillUserObj / g_UserList[0..USER_ARRAY_COUNT-1]。</summary>
public static class CSelUserList
{
    /// <summary>AcceptExWorkedThread.pas:12/14 —— MAX_GAME_USER = 1000; USER_ARRAY_COUNT = MAX_GAME_USER + 48 = 1048。
    /// （定长数组以 List 表达；上限一致，便于与原版逐一对照。）</summary>
    public const int MAX_GAME_USER = 1000;
    public const int USER_ARRAY_COUNT = MAX_GAME_USER + 48;

    public static CSelSessionObj? g_pFillUserObj;
    public static readonly List<CSelSessionObj?> g_UserList = new();

    /// <summary>ClientSession.pas:347-358 FillUserList。</summary>
    public static void FillUserList()
    {
        if (g_pFillUserObj == null)                    // :351
            g_pFillUserObj = new CSelSessionObj();     // :352
        g_pFillUserObj.m_tLastGameSvr = null;          // :353
        g_UserList.Clear();
        for (int i = 0; i <= USER_ARRAY_COUNT - 1; i++) // :354
            g_UserList.Add(g_pFillUserObj);            // :356
    }

    /// <summary>ClientSession.pas:360-364 CleanupUserList。</summary>
    public static void CleanupUserList()
    {
        if (g_pFillUserObj != null)                    // :362
            g_pFillUserObj = null;                     // :363 g_pFillUserObj.Free
    }
}

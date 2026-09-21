using System;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// `Source/LoginGate/ClientSession.pas` 的 **LoginGate 独有残部** → `Rest11LoginGateSession.cs`
///
/// <para>
/// 逐字移植的成员（复核报告 §4.1 的 `ClientSession` 残留缺口清单）：
/// <list type="number">
///   <item>`m_dwProtocolPassword`（:27）+ 构造/`ReCreate` 的置零（:74、:95）+ `UserEnter` 的随机化（:716）；</item>
///   <item>`m_IsCanSetL2Password` / `m_IsCanCheckL2Password`（:28-29）+ `ProcessCltData` 的门控（:489-520）；</item>
///   <item>`m_IsDelayClose` / `m_dwDelayCloseTick`（:31-32）+ `DelayClose`（:785-789）；</item>
///   <item>`RotateBits(C: Char; Bits: Integer): Char`（:103-121）；</item>
///   <item>`ReCreate`（:87-101）、`UserEnter`（:706-737）、`UserLeave`（:739-783）、
///         `ProcessSvrData`（:690-704）。</item>
/// </list>
/// </para>
///
/// <para>
/// ⚠ 这三项**LoginGate 副本独有**（SelGate 副本没有）⇒ `SelGateSession.cs` **无法复用**，
/// 也**不得**被"统一"进 `GateSession`（`GatewayProtocol.cs:84-130`）。本类与 `GateSession`
/// **并存**：`GateSession` 继续服务既有 LoginGate/SelGate 运行路径，本类只在
/// `Rest11LoginGateOptions.EnableSessionResidual` 打开时被宿主使用。
/// </para>
///
/// <para>
/// 原文缺陷（照抄，未顺手修）：
/// <list type="number">
///   <item>★ `TSessionObj.Create`（:62-80）与 `ReCreate`（:87-101）在 `m_dwDelayCloseTick` 处写的是
///         **`GetTickCount`（无括号）**（:79、:100），而同过程的 `m_dwClientTimeOutTick` 写的是
///         `GetTickCount()`（:68、:93）⇒ 原文是**不一致写法**；Delphi 下 `GetTickCount` 无参调用
///         仍是函数调用（取当前 tick），故**行为等价**。托管侧统一取 `DelphiRTL.GetTickCount()`，
///         并在报告 §4 登记为"原文如此 / 语义等价"。</item>
///   <item>`ProcessCltData`（:123-155）的 `SendDefMessage` 把 `nSeries` 写进了 `Cmd.param`
///         （:136 `Cmd.param := nSeries;`）而 `nParam` 已被 :134 用过 ⇒ **nParam 被 nSeries 覆盖**。
///         照抄。</item>
///   <item>`ProcessSvrData`（:690-704）在 `m_fKickFlag` 为真时把标志**反转回 False**（:694）
///         然后关连接；照抄。</item>
///   <item>`UserEnter`（:706-737）声明 `dwTemp: array[0..3]` 并把
///         `m_dwProtocolPassword xor dwTemp[0..2]` 放进 `dwTemp[3]`（:729）；
///         `m_dwProtocolPassword` 由 `1 + Random(High(Integer) - 1)` 生成（:716）。
///         托管用注入的 <see cref="Rand"/>（默认 `Random.Shared`）以保持可测。</item>
///   <item>`UserLeave`（:739-783）清空发送队列时依赖 `PSendQueueNode(m_pOverlapSend).DynSendList`
///         （`SendQueue.pas`，托管侧已由 `GateSession._sendQueue` 取代，报告 §4.2）
///         ⇒ 该段以 <see cref="DrainSendQueue"/> 接缝表达（默认空实现）。</item>
/// </list>
/// </para>
/// </summary>
public class Rest11LoginGateSession : IRest11SessionObj
{
    /// <summary>`WinSock2.INVALID_SOCKET`（= -1）。</summary>
    public const int INVALID_SOCKET = -1;

    // ---- ClientSession.pas:12-32 字段（LoginGate 副本全集）----
    public int SocketId;                       // m_pUserOBJ._SendObj.Socket 的会话侧标识
    public bool m_fConnectedToGameSvr;         // `m_tLastGameSvr <> nil` 的连接状态侧
    public bool m_fGameSvrActive;              // `m_tLastGameSvr.Active`

    public bool m_fKickFlag;                   // :18
    public byte m_fHandleLogin;                // :19
    public uint m_dwSessionID;                 // :20
    public int m_nSvrListIdx;                  // :21
    public int m_nSvrObject;                   // :22
    public ushort m_wRandKey;                  // :23

    public uint m_dwClientTimeOutTick;         // :25

    public uint m_dwProtocolPassword;          // :27 ★ LoginGate 独有（通讯协议密码）
    public bool m_IsCanSetL2Password;          // :28 ★ LoginGate 独有
    public bool m_IsCanCheckL2Password;        // :29 ★ LoginGate 独有

    public bool m_IsDelayClose;                // :31 ★ LoginGate 独有
    public uint m_dwDelayCloseTick;            // :32 ★ LoginGate 独有

    /// <summary>`ClientSession.pas:51 enterCount`（模块级全局，`UserEnter` 自增）。</summary>
    public static int g_enterCount;

    /// <summary>`ClientSession.pas:53 gDeny`（模块级全局；`ProcessCltData` :231 会踢线）。</summary>
    public static volatile bool gDeny;

    /// <summary>随机源（`Randomize()` + `Random(...)`）；测试可替换为确定性实现。</summary>
    public Func<int, int> Rand { get; set; } = max => Random.Shared.Next(max);

    /// <summary>`m_pUserOBJ._SendObj.Socket`。</summary>
    public int Socket { get; set; } = INVALID_SOCKET;

    /// <summary>`m_pUserOBJ.pszIPAddr`。</summary>
    public string IPText { get; set; } = "";

    /// <summary>`m_pUserOBJ.nIPAddr`。</summary>
    public int IPAddr { get; set; }

    /// <summary>`m_nSvrObject`（`IRest11SessionObj.SvrObject` 的显式实现）。</summary>
    int IRest11SessionObj.SvrObject { get => m_nSvrObject; set => m_nSvrObject = value; }

    /// <summary>`m_IsCanSetL2Password`（`IRest11SessionObj` 的显式实现；底层字段同名）。</summary>
    bool IRest11SessionObj.m_IsCanSetL2Password { get => m_IsCanSetL2Password; set => m_IsCanSetL2Password = value; }

    /// <summary>`m_IsCanCheckL2Password`（`IRest11SessionObj` 的显式实现；底层字段同名）。</summary>
    bool IRest11SessionObj.m_IsCanCheckL2Password { get => m_IsCanCheckL2Password; set => m_IsCanCheckL2Password = value; }

    // ---- IRest11SessionObj 接缝实现 ----
    bool IRest11SessionObj.KickFlag { get => m_fKickFlag; set => m_fKickFlag = value; }
    byte IRest11SessionObj.HandleLogin { get => m_fHandleLogin; set => m_fHandleLogin = value; }
    bool IRest11SessionObj.LastGameSvrActive => m_fConnectedToGameSvr && m_fGameSvrActive;
    bool IRest11SessionObj.IsDelayClose { get => m_IsDelayClose; set => m_IsDelayClose = value; }
    uint IRest11SessionObj.dwDelayCloseTick { get => m_dwDelayCloseTick; set => m_dwDelayCloseTick = value; }
    uint IRest11SessionObj.dwClientTimeOutTick { get => m_dwClientTimeOutTick; set => m_dwClientTimeOutTick = value; }

    /// <summary>`ClientSession.pas:62-80` `constructor TSessionObj.Create`。</summary>
    public Rest11LoginGateSession()
    {
        // :65 Randomize() —— 托管 Random.Shared 自带种子
        m_fKickFlag = false;                                         // :66
        m_nSvrObject = 0;                                            // :67
        m_dwClientTimeOutTick = DelphiRTL.GetTickCount();            // :68
        m_fHandleLogin = 0;                                          // :69
        m_nSvrListIdx = 0;                                           // :70

        m_wRandKey = 0;                                              // :72

        m_dwProtocolPassword = 0;                                    // :74
        m_IsCanSetL2Password = false;                                // :75
        m_IsCanCheckL2Password = false;                              // :76

        m_IsDelayClose = false;                                      // :78
        m_dwDelayCloseTick = DelphiRTL.GetTickCount();               // :79 原文如此：`GetTickCount`（无括号），语义等价
    }

    /// <summary>`ClientSession.pas:82-85` `destructor Destroy`（原文只调 inherited）。</summary>
    public void Destroy() { }

    /// <summary>`ClientSession.pas:87-101` `ReCreate`。</summary>
    public void ReCreate()
    {
        m_fKickFlag = false;                                         // :89

        m_nSvrObject = 0;                                            // :91
        m_fHandleLogin = 0;                                          // :92
        m_dwClientTimeOutTick = DelphiRTL.GetTickCount();            // :93

        m_dwProtocolPassword = 0;                                    // :95
        m_IsCanSetL2Password = false;                                // :96
        m_IsCanCheckL2Password = false;                              // :97

        m_IsDelayClose = false;                                      // :99
        m_dwDelayCloseTick = DelphiRTL.GetTickCount();               // :100 原文如此：`GetTickCount`（无括号）
    }

    /// <summary>
    /// `ClientSession.pas:785-789` `DelayClose(DelayTick: LongWord)`：
    /// `m_dwDelayCloseTick := GetTickCount + DelayTick; m_IsDelayClose := True;`
    /// （到期判定在 `FuncForComm.TProcMsgThread.Run` :213）。
    /// </summary>
    public void DelayClose(uint DelayTick)
    {
        m_dwDelayCloseTick = DelphiRTL.GetTickCount() + DelayTick;    // :787
        m_IsDelayClose = true;                                        // :788
    }

    /// <summary>
    /// `ClientSession.pas:706-737` `UserEnter`。原文对 `m_pUserOBJ.pszLocalIPAddr` 与
    /// `m_tLastGameSvr.SendBuffer` 的依赖以注入回调表达（接缝）。
    /// </summary>
    public void UserEnter(Rest11ProcMsgThread g_ProcMsgThread,
                          Action<int, string, string> sendToGameSvr,
                          Action<byte[]> sendToClient)
    {
        g_enterCount++;                                               // :711 Inc(enterCount)
        m_fHandleLogin = 0;                                           // :712
        g_ProcMsgThread.AddSession(this);                             // :713

        // :715 Randomize
        m_dwProtocolPassword = (uint)(1 + Rand(int.MaxValue - 1));    // :716 1 + Random(High(Integer) - 1)

        m_IsCanSetL2Password = false;                                 // :718
        m_IsCanCheckL2Password = false;                               // :719

        // :721 szSendBuf := '%' + Format('O%d/%s/%s$', [Socket, pszIPAddr, pszLocalIPAddr])
        string szSendBuf = "%" + string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "O{0}/{1}/{2}$", Socket, IPText, LocalIPText);
        sendToGameSvr(Socket, szSendBuf, IPText);                     // :722 m_tLastGameSvr.SendBuffer(...)

        // :725-736
        var dwTemp = new uint[4];
        dwTemp[0] = (uint)Rand(int.MaxValue - 1);                     // :727 Random(High(Integer) - 1)
        dwTemp[1] = (uint)Rand(int.MaxValue - 1);                     // :727
        dwTemp[2] = (uint)Rand(int.MaxValue - 1);                     // :728
        dwTemp[3] = m_dwProtocolPassword ^ dwTemp[0] ^ dwTemp[1] ^ dwTemp[2]; // :729

        byte[] src = new byte[16];
        Buffer.BlockCopy(dwTemp, 0, src, 0, 16);
        byte[] encoded = EDcode.EncodeBuffer(src, src.Length);        // :731 EncodeBuffer(@dwTemp[0], SizeOf(dwTemp))
        sendToClient(encoded);                                        // :732 m_tIOCPSender.SendData(...)
    }

    /// <summary>`m_pUserOBJ.pszLocalIPAddr`（`UserEnter` :721 的第二个占位）。</summary>
    public string LocalIPText { get; set; } = "";

    /// <summary>
    /// `ClientSession.pas:690-704` `ProcessSvrData(GS: TClientThread; Addr, Len)`。
    /// 返回 false 表示原文执行了 `InterlockedExchange(m_pOverlapRecv.Socket, INVALID_SOCKET)`。
    /// </summary>
    public bool ProcessSvrData(byte[] addr, int len, Action<byte[], int> sendToClient)
    {
        if (m_fKickFlag)                                              // :692
        {
            m_fKickFlag = false;                                      // :694
            Socket = INVALID_SOCKET;                                  // :695 InterlockedExchange(m_pOverlapRecv.Socket, INVALID_SOCKET)
            return false;                                             // 关闭动作由宿主负责（:696 SHSocket.FreeSocket）
        }

        bool ok = sendToClientSafe(sendToClient, addr, len);          // :700 m_tIOCPSender.SendData(m_pOverlapSend, PChar(Addr), Len)
        if (!ok)                                                      // :702
        {
            Socket = INVALID_SOCKET;                                  // :703 InterlockedExchange(m_pOverlapRecv.Socket, INVALID_SOCKET)
        }
        return ok;
    }

    private static bool sendToClientSafe(Action<byte[], int> sendToClient, byte[] addr, int len)
    {
        try
        {
            sendToClient(addr, len);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// `ClientSession.pas:739-783` `UserLeave`。
    /// 第 3 步（:757-774）原依赖 `PSendQueueNode(m_pOverlapSend).DynSendList` + `GetDynPacket`，
    /// 托管由 <see cref="DrainSendQueue"/> 接缝表达（默认无操作，与 `GateSession._sendQueue` 并存）。
    /// 第 4 步 `g_ProcMsgThread.DelSession(Self)`（:777-778）。
    /// </summary>
    public void UserLeave(Rest11ProcMsgThread? g_ProcMsgThread,
                          Action<int, string> sendToGameSvr, Action? drainSendQueue = null)
    {
        int nCode = 0;
        try
        {
            nCode = 1;                                                // :747
            m_fHandleLogin = 0;                                       // :748
            string szSenfBuf = "%" + string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "X{0}$", Socket);                                     // :749
            sendToGameSvr(Socket, szSenfBuf);                         // :750 m_tLastGameSvr.SendBuffer(...)

            nCode = 2;                                                // :752
            DeleteConnectOfIPCallback?.Invoke(IPAddr);                // :753 DeleteConnectOfIP(Self.m_pUserOBJ.nIPAddr)

            nCode = 3;                                                // :755
            (drainSendQueue ?? DrainSendQueue)?.Invoke();             // :757-774（DynSendList 清空）

            nCode = 4;                                                // :776
            if (g_ProcMsgThread != null)                              // :777
                g_ProcMsgThread.DelSession(this);                     // :778
        }
        catch (Exception M)                                           // :779 except on M: Exception do
        {
            _ = nCode;                                                // :781 g_pLogMgr.Add(Format('TSessionObj.UserLeave: %d %s', ...))
            _ = M;
        }
    }

    /// <summary>第 3 步的清队列接缝（宿主可挂 `GateSession` 的发送队列清理）。</summary>
    public Action? DrainSendQueue { get; set; }

    /// <summary>`UserLeave` 第 2 步 `DeleteConnectOfIP(m_pUserOBJ.nIPAddr)`（:753）的注入点。</summary>
    public Action<int>? DeleteConnectOfIPCallback { get; set; }

    /// <summary>
    /// `ClientSession.pas:103-121` `RotateBits(C: Char; Bits: Integer): Char`。
    /// ★ **LoginGate 副本独有**（SelGate 副本没有此函数）；
    /// 原文唯一的活跃调用点在 `ClientSession.pas:600/:602`，而那整块（`CM_IDPASSWORD` 分支）
    /// 已被 `{$I ... VMPE.inc}` 前的大段注释包住（:582-678）⇒ **当前不参与编译**。
    /// 本实现仍 1:1 落地，供将来解注释时使用。
    /// </summary>
    public static char RotateBits(char C, int Bits)
    {
        Bits = Bits % 8;                                              // :107
        ushort SI;
        int shift = Math.Abs(Bits);
        // 复用 `GXX.Core.Rtl.DelphiRTL.MakeWord(int lo, int hi)`——该重载与 Delphi/Windows
        // `MakeWord(Lo, Hi) = (Hi shl 8) or Lo` **一致**（实测 `MakeWord(0, 0x41)` = 0x4100）。
        // 用**具名实参**调用，避免与 `MakeWord(byte, byte)`（实参顺序相反的旧重载）撞上。
        if (Bits < 0)                                                 // :108
        {
            SI = (ushort)DelphiRTL.MakeWord(lo: (int)(byte)C, hi: 0);  // :110 MakeWord(Byte(C), 0)
            // :111 `SI := SI shl Abs(Bits)`：Delphi 的 shl 在 Word 上**截断到 16 位**；
            //      C# 的 ushort 左移先提升为 int ⇒ 必须显式 & 0xFFFF，否则高位幸存。
            SI = (ushort)((SI << shift) & 0xFFFF);
        }
        else
        {
            SI = (ushort)DelphiRTL.MakeWord(lo: 0, hi: (int)(byte)C);  // :115 MakeWord(0, Byte(C))
            SI = (ushort)(SI >> shift);                               // :116 SI := SI shr Abs(Bits)
        }
        SI = SwapWord(SI);                                            // :118 SI := Swap(SI)
        SI = (ushort)(LoByte(SI) | HiByte(SI));                       // :119 SI := Lo(SI) or Hi(SI)
        return (char)SI;                                              // :120 chr(SI)
    }

    /// <summary>
    /// `System.Swap`（16 位字节交换）：`Swap($2080) = $8020`，即 `(w &gt;&gt; 8) | (w &lt;&lt; 8)` 截断到 16 位。
    /// ★ 必须显式 `&amp; 0xFFFF`：C# 的 `ushort` 参与 `&lt;&lt;` 前会**提升为 int**
    ///   ⇒ `(ushort)((w &gt;&gt; 8) | (w &lt;&lt; 8))` 的高位会幸存（实测 `Swap($2080)` 得 `$208000`）。
    /// </summary>
    private static ushort SwapWord(ushort w)
        => (ushort)(((w >> 8) | ((w << 8) & 0xFFFF)) & 0xFFFF);

    /// <summary>`System.Lo(Word)` = 低字节。</summary>
    private static ushort LoByte(ushort w) => (ushort)(w & 0x00FF);

    /// <summary>`System.Hi(Word)` = 高字节。</summary>
    private static ushort HiByte(ushort w) => (ushort)((w >> 8) & 0x00FF);

    /// <summary>
    /// `ClientSession.pas:123-155` `SendDefMessage` 的帧构造部分（`#` + 编码体 + `!`）。
    /// 原文把 `nSeries` 覆盖进 `Cmd.param`（:136）—— 照抄（见类头注释缺陷 2）。
    /// </summary>
    public static byte[] BuildDefMessageFrame(ushort wIdent, long nRecog, ushort nParam, ushort nTag,
                                             ushort nSeries, string sMsg)
    {
        // ★ 必须**按原文顺序逐条赋值**（不能用对象初始化器的"一次性"语义）：
        //   :134 `Cmd.param := nParam;` 之后 :136 又 `Cmd.param := nSeries;`
        //   ⇒ **param 最终等于 nSeries**（原文缺陷，照抄；原文 :136 写的就是 param）。
        var cmd = new TDefaultMessage
        {
            Recog = nRecog,   // :132
            Ident = wIdent,   // :133
            Param = nParam,   // :134
            Tag = nTag        // :135
        };
        cmd.Param = nSeries;  // :136 ★ 原文如此：`Cmd.param := nSeries`（覆盖 :134 的 nParam）

        byte[] cmdBytes = StructBytes.BytesOf(cmd);                   // :139 Move(Cmd, TempBuf[1], SizeOf(TCmdPack))

        byte[] temp = new byte[cmdBytes.Length + (sMsg.Length > 0 ? sMsg.Length : 0)];
        Buffer.BlockCopy(cmdBytes, 0, temp, 0, cmdBytes.Length);
        if (sMsg.Length > 0)
        {
            // :142 Move(sMsg[1], TempBuf[SizeOf(TCmdPack)+1], Length(sMsg)) —— Delphi 1-based string 的字节拷贝
            byte[] msg = EncodingInit.GBK.GetBytes(sMsg);
            var grown = new byte[cmdBytes.Length + msg.Length];
            Buffer.BlockCopy(temp, 0, grown, 0, cmdBytes.Length);
            Buffer.BlockCopy(msg, 0, grown, cmdBytes.Length, msg.Length);
            temp = grown;
        }

        byte[] sendBufBody = EDcode.EncodeBuffer(temp, temp.Length);  // :144/:148 EncodeBuffer(@TempBuf[1], ..., @SendBuf[1], ...)
        var frame = new byte[sendBufBody.Length + 2];
        frame[0] = (byte)'#';                                         // :138 SendBuf[0] := '#'
        Buffer.BlockCopy(sendBufBody, 0, frame, 1, sendBufBody.Length);
        frame[^1] = (byte)'!';                                        // :150 SendBuf[iLen + 1] := '!'
        return frame;
    }

    /// <summary>
    /// `ClientSession.pas:791-800` `FillUserList` / `802-806` `CleanupUserList`：
    /// 原文把 **同一个** `g_pFillUserObj` 填满整个 `g_UserList`（长度为
    /// `USER_ARRAY_COUNT = MAX_GAME_USER + 48 = 1048`，`AcceptExWorkedThread.pas:11-13`），
    /// 且 `m_tLastGameSvr := nil`。照抄（含"全是同一对象"这一原文特征）。
    /// </summary>
    public static IRest11SessionObj?[] FillUserList(int userArrayCount = 1000 + 48,
                                                   Rest11LoginGateSession? fillUserObj = null)
    {
        fillUserObj ??= new Rest11LoginGateSession();                 // :795-796 if g_pFillUserObj = nil then Create
        fillUserObj.m_fConnectedToGameSvr = false;                    // :797 g_pFillUserObj.m_tLastGameSvr := nil
        var list = new IRest11SessionObj?[userArrayCount];            // :47 g_UserList: array[0..USER_ARRAY_COUNT-1]
        for (int i = 0; i <= userArrayCount - 1; i++)                  // :798
            list[i] = fillUserObj;                                    // :799
        return list;
    }
}

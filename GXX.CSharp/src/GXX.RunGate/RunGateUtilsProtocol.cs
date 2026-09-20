using System;
using System.Runtime.InteropServices;
using GXX.Core;
using GXX.Core.Protocol;

// 源单元：Source/RunGate/Grobal2_Ex.pas（帧头/结构/常量） + Source/RunGate/GateShare.pas（EncodeRunGateMsg）
// 本文件只放「协议字节布局」相关的移植，不含任何网络/线程代码。
// 编译期开关（Grobal2_Ex.pas:10-32 / Common/IocpCommon.pas:14）：实际生效组合为
//   CLIENT_ANTIPLUG = 1, NEED_REGISTER = 1, REGISTER_TEST = 0, MultiThreadRunContext = 1, UseIocpClient = 1
// 因此 {$IF NeedIocpClient=0} / {$IF MultiThreadRunContext=0} 分支是**死代码**（见 RunGateUtils.pas:1538-1786 / 4331-4476）。

namespace GXX.RunGate;

/// <summary>
/// RunGate 侧协议常量（Grobal2_Ex.pas:667-694）。
/// <para>
/// 易错点（原文 Grobal2_Ex.pas:677-686 "这几个值用变量搞"）：当 <c>NEED_REGISTER = 1</c> 时，
/// <c>GM_DATA/GM_COMPDATA/GM_KICK/GM_DATA_CACHE/GM_NO_CERTIFICATION/GM_FULL_SERVICE_MSG/
/// GM_RANDOM_DATA/GM_RUN_GATE_VER/GM_RUN_GATE_MAGICS</c> 在原文里被声明为 <c>var</c>（可运行时改写），
/// 但注释里残留的条件值 <c>{$IF NEED_REGISTER = 1} 0 {$ELSE} ...</c> 是**假的**（已被注释掉），
/// 实际初值就是 5/9/10/11/12/13/14/15/16。
/// 全源码树里对它们唯一的赋值点都在 <c>{...}</c> 注释块内（RunGateUtils.pas:4184/4219/4321-4323/4779-4784/4866、
/// MirClientContext.pas:10224、AppMain.pas:1306），因此**运行期取值恒等于声明初值** → 可安全当常量。
/// </para>
/// </summary>
public static class RunGateUtilsConst
{
    /// <summary>Grobal2_Ex.pas:693 — RUNGATECODE = $AA55AA55（NEED_REGISTER=1 下是 var，但无运行期赋值）。</summary>
    public const uint RUNGATECODE = 0xAA55AA55u;

    /// <summary>Grobal2_Ex.pas:694 — RUNGATECODEX = $AA9AAA9A（GM_COMPDATA 帧 nSocket 的校验值）。</summary>
    public const uint RUNGATECODEX = 0xAA9AAA9Au;

    /// <summary>Grobal2_Ex.pas:37 — RUN_GATE_MSG_CODE = $AABBCCDD（TRungateMsgHeader.Code）。</summary>
    public const uint RUN_GATE_MSG_CODE = 0xAABBCCDDu;

    // ---- GM_* 服务器↔网关命令（Grobal2_Ex.pas:667-686，等价 CommonConst.*）----
    public const int GM_OPEN = CommonConst.GM_OPEN;                                   // 1
    public const int GM_CLOSE = CommonConst.GM_CLOSE;                                 // 2
    public const int GM_CHECKSERVER = CommonConst.GM_CHECKSERVER;                     // 3
    public const int GM_CHECKCLIENT = CommonConst.GM_CHECKCLIENT;                     // 4
    public const int GM_DATA = CommonConst.GM_DATA;                                   // 5
    public const int GM_SERVERUSERINDEX = CommonConst.GM_SERVERUSERINDEX;             // 6
    public const int GM_RECEIVE_OK = CommonConst.GM_RECEIVE_OK;                       // 7
    public const int GM_CLOSECONNECT = CommonConst.GM_CLOSECONNECT;                   // 8
    public const int GM_COMPDATA = CommonConst.GM_COMPDATA;                           // 9
    public const int GM_KICK = CommonConst.GM_KICK;                                   // 10
    public const int GM_DATA_CACHE = CommonConst.GM_DATA_CACHE;                       // 11
    public const int GM_NO_CERTIFICATION = CommonConst.GM_NO_CERTIFICATION;           // 12
    public const int GM_FULL_SERVICE_MSG = CommonConst.GM_FULL_SERVICE_MSG;           // 13
    public const int GM_RANDOM_DATA = CommonConst.GM_RANDOM_DATA;                     // 14
    public const int GM_RUN_GATE_VER = CommonConst.GM_RUN_GATE_VER;                   // 15
    public const int GM_RUN_GATE_MAGICS = CommonConst.GM_RUN_GATE_MAGICS;             // 16
    public const int GM_DELAY_CLOSE = CommonConst.GM_DELAY_CLOSE;                     // 20

    // ---- 帧长（Delphi SizeOf，32 位自然对齐）----
    /// <summary>TM2MsgHeader = 20：LongWord+Integer+Word+Word+LongWord+Integer。</summary>
    public const int SizeOfTM2MsgHeader = 20;

    /// <summary>TDefaultMessage = 16：Int64 + Word×4（与 GXX.Core.Protocol.TDefaultMessage.SizeOf 一致）。</summary>
    public const int SizeOfTDefaultMessage = 16;

    /// <summary>TRungateMsgHeader = 24：LongWord + LongWord + TDefaultMessage（8 对齐 → 偏移 8）。</summary>
    public const int SizeOfTRungateMsgHeader = 24;

    /// <summary>TRungateVerifyHeader = 12：LongWord×3。</summary>
    public const int SizeOfTRungateVerifyHeader = 12;

    /// <summary>TRungateVerifyData = 51（packed）：4 + 15 + 32。</summary>
    public const int SizeOfTRungateVerifyData = 51;

    /// <summary>TRungateVerifyData_New = 59（packed）：51 + 4 + 4。</summary>
    public const int SizeOfTRungateVerifyData_New = 59;

    /// <summary>GateShare.pas:3479 — EncodeRunGateMsg 固定用 RUN_GATE_MSG_CODE 作 TRungateMsgHeader.Code。</summary>
    public const uint EncodeRunGateMsgCode = RUN_GATE_MSG_CODE;
}

/// <summary>TRungateVerifyHeader（Grobal2_Ex.pas:265-269，非 packed，自然对齐 = 12 字节）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TRungateVerifyHeader
{
    public uint dwCode;
    public uint dwCmd;
    public uint nLength;
}

/// <summary>TRungateVerifyData（Grobal2_Ex.pas:271-275，packed = 1+14+1 字节 IP + 8×DWORD HWID）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TRungateVerifyData
{
    public uint Key;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public byte[] IP;          // array[0..14] of Char
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public uint[] HWID;        // array[0..7] of LongWord
}

/// <summary>TRungateVerifyData_New（Grobal2_Ex.pas:278-284，packed，尾部多 UpdateDate/Version）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TRungateVerifyData_New
{
    public uint Key;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public byte[] IP;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public uint[] HWID;
    public uint UpdateDate;
    public uint Version;
}

/// <summary>
/// 帧构造助手（对应 RunGateUtils.pas 里手写的 TM2MsgHeader 填充 + PostSendBuffer/SendBuf）。
/// 纯函数，无 socket。
/// </summary>
public static class RunGateFrameBuilder
{
    /// <summary>
    /// RunGateUtils.pas:402-425 / 1743-1770 — 组装「TM2MsgHeader + 数据 + 结尾 #0」。
    /// 原文在补数据时把缓冲长度算作 <c>BufferLen + 20 + 1</c>（多留一个字节并把最后一位置 0），
    /// 但 <c>GateMsg.nLength</c> 仍写的是**真实** BufferLen，不含这个 #0。
    /// </summary>
    public static byte[] BuildServerMsg(uint code, int nSocket, ushort wSocketIdx, int nIdent,
                                        uint wUserListIndex, byte[] data, int bufferLen)
    {
        int payload = data == null ? 0 : bufferLen;
        int total = RunGateUtilsConst.SizeOfTM2MsgHeader + payload + (data == null ? 0 : 1);
        var buf = new byte[total];
        WriteHeader(buf, 0, code, nSocket, wSocketIdx, nIdent, wUserListIndex, bufferLen);
        if (data != null && bufferLen > 0)
            Array.Copy(data, 0, buf, RunGateUtilsConst.SizeOfTM2MsgHeader, payload);
        if (data != null)
            buf[total - 1] = 0;                     // 原 430：PChar(... + nLen - 1)^ := #0
        return buf;
    }

    /// <summary>RunGateUtils.pas:402-407 — 只写 20 字节帧头（Buffer = nil 分支）。</summary>
    public static byte[] BuildServerMsgHeaderOnly(uint code, int nSocket, ushort wSocketIdx, int nIdent,
                                                  uint wUserListIndex, int bufferLen)
        => BuildServerMsg(code, nSocket, wSocketIdx, nIdent, wUserListIndex, null, bufferLen);

    public static void WriteHeader(byte[] dst, int offset, uint code, int nSocket, ushort wSocketIdx,
                                   int nIdent, uint wUserListIndex, int nLength)
    {
        var h = default(TM2MsgHeader);
        h.dwCode = code;
        h.nSocket = nSocket;
        h.wGSocketIdx = wSocketIdx;
        h.wIdent = (ushort)nIdent;
        h.wUserListIndex = wUserListIndex;
        h.nLength = nLength;
        var b = StructBytes.BytesOf(h);
        Array.Copy(b, 0, dst, offset, b.Length);
    }

    /// <summary>
    /// GateShare.pas:3475-3494 — EncodeRunGateMsg：TRungateMsgHeader(Code=RUN_GATE_MSG_CODE,
    /// DataLen, Msg) + 附加数据。DataAdd 为 nil 或长度 &lt;= 0 时 DataLen = 0（不代表“长度为负时照抄”）。
    /// </summary>
    public static byte[] EncodeRunGateMsg(in TDefaultMessage defMsg, byte[] dataAdd, int dataAddLen)
    {
        int len = (dataAdd != null && dataAddLen > 0) ? dataAddLen : 0;
        var hdr = default(TRungateMsgHeader);
        hdr.Code = RunGateUtilsConst.EncodeRunGateMsgCode;
        hdr.Msg = defMsg;
        hdr.DataLen = (uint)len;
        var head = StructBytes.BytesOf(hdr);
        if (head.Length != RunGateUtilsConst.SizeOfTRungateMsgHeader)
            throw new InvalidOperationException("TRungateMsgHeader 布局与 Delphi SizeOf 不符: " + head.Length);
        var result = new byte[head.Length + len];
        Array.Copy(head, 0, result, 0, head.Length);
        if (len > 0)
            Array.Copy(dataAdd, 0, result, head.Length, len);
        return result;
    }

    /// <summary>
    /// RunGateUtils.pas:314-357（TMirRemoteContext.DoConnect）— 版本上报帧。
    /// IntVer = Y*10000 + M*100 + D（来自 g_sUpdateTime），wGSocketIdx 固定 5，nLength = 0。
    /// 原文只在 <c>not g_boSendVersionToM2</c> 时发；而该标志初始为 False 且赋值行也被注释掉
    /// （RunGateUtils.pas:325/342），所以**每次连接都会发**。
    /// </summary>
    public static int MakeVersionNumber(int year, int month, int day) => year * 10000 + month * 100 + day;

    /// <summary>RunGateUtils.pas:333-339 — 版本上报帧（GM_RUN_GATE_VER）。</summary>
    public static byte[] BuildRunGateVersionMsg(int intVer)
        => BuildServerMsgHeaderOnly(RunGateUtilsConst.RUNGATECODE, intVer, 5,
                                    RunGateUtilsConst.GM_RUN_GATE_VER, 0, 0);

    /// <summary>RunGateUtils.pas:349-355 — 请求技能列表帧（GM_RUN_GATE_MAGICS）。</summary>
    public static byte[] BuildRequestMagicListMsg()
        => BuildServerMsgHeaderOnly(RunGateUtilsConst.RUNGATECODE, 0, 0,
                                    RunGateUtilsConst.GM_RUN_GATE_MAGICS, 0, 0);
}

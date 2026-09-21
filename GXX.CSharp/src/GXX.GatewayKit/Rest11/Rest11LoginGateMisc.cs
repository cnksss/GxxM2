using System;
using System.Collections.Generic;
using GXX.Core.Rtl;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// `Source/LoginGate/Misc.pas` → `Rest11LoginGateMisc.cs`
/// 1:1 逐字移植：常量/枚举（:8-131）、`CloseIPConnect`（:155-179）、
/// `KickUser`×2（:181-224）、`BlockUser`（:226-242）、`ReverseIP`（:244-250）、
/// `AnsiStrToVal`（:252-273）、`SendGameCenterMsg`（:275-286）、`CheckAccountName`（:288-318）。
///
/// <para>
/// ⚠ 与既有设施的关系（**并存**，不合并）：
///   · `GXX.GatewayKit.GateService` 只有 `AddBlockIP(ip)`（GateService.cs:224-227）与简化 `CheckIP`；
///     原版 `KickUser`/`BlockUser` 是**两条不同的封禁路径**且都受 `m_fKickOverPacketSize` 总开关约束
///     （Misc.pas:184、:209、:228），并按 `m_tBlockIPMethod` 三分支走
///     `AddToTempBlockIPList` / `AddToBlockIPList`。本类照抄原版。
///   · `GXX.SelGate.CSelGateMisc`（SelGateMisc.cs:11-16 自陈"与 GatewayKit 的差异必须保留"）
///     是同一份 `.pas` 的 **SelGate 副本**投影，它多出 `{$DEFINE SIGN3D}` / `g_boNetComGate` /
///     `tSelGate` 分支、`SendGameCenterMsg` 走 `g_boNetComGate → tSelGate1`。本类照抄 **LoginGate 副本**：
///     `nParam := MakeLong(Word(tLoginGate), wIdent)`（:280）**无分支**。
/// </para>
///
/// <para>
/// 原文缺陷（照抄，未顺手修）：
/// <list type="number">
///   <item>`KickUser(nRemoteIP: Integer): Boolean`（:181-205）：`mDisconnect` 分支**也不做任何封禁**
///         只返回 False（:186-189）；`mBlock`/`mBlockList` 分支返回 False 且执行封禁；
///         开关 `m_fKickOverPacketSize=False` 时返回 **True**（:183）。照抄。</item>
///   <item>`KickUser(const UserObj)`（:207-224）：**先** `SHSocket.FreeSocket` **再**置 `m_fKickFlag`
///         （:211-212），与 `BlockUser` 的顺序相反；且 `case` **只列 mBlock/mBlockList**，
///         `mDisconnect` 既不封禁也不做别的。照抄。</item>
///   <item>`AnsiStrToVal`（:252-273）在"非数字开头"时返回 0 且 nPos=0；原版是 `PChar` 指针逐字节推进，
///         托管改为 `string` + 下标，边界以 `idx &lt; Length` 表达（原文靠 `#0` 终止符，等价）。</item>
///   <item>`SendGameCenterMsg`（:275-286）`cbData := Length(sSendMsg) + 1` 且 `StrCopy` 写入结尾 `#0`；
///         托管把"含结尾 NUL 的字节数"语义交给 <see cref="IRest11GameCenterChannel"/> 实现方
///         （与 `SelGateMisc.cs:155` 同一约定）。</item>
///   <item>`CheckAccountName`（:288-318）字符范围 `(sName[I] &lt; '0') or (sName[I] &gt; 'z')` **极宽松**：
///         大写字母、`':;&lt;=&gt;?@'` 等 ASCII 全被放行；只有越界字节才进入 GBK 双字节回退（:306-312）。
///         且 `Length(sName)` 在 Delphi 下是**字节**长度而托管是 **UTF-16 字符**数 ⇒ 对纯 ASCII 等价，
///         对非 ASCII 有偏差（登记见报告 D-P11-xx，未修）。照抄。</item>
/// </list>
/// </para>
/// </summary>
public static class Rest11LoginGateMisc
{
    // ---- Misc.pas:120-131 服务器 ↔ GameCenter 消息标识 ----
    public const int SG_FORMHANDLE = 1000; // :121 服务器HANLD
    public const int SG_STARTNOW = 1001;   // :122 正在启动服务器...
    public const int SG_STARTOK = 1002;    // :123 服务器启动完成...
    public const int SG_ACTIVE = 1003;     // :124
    public const int GS_QUIT = 2000;       // :126 关闭

    /// <summary>`Misc.pas:129-131` `TProgamType`（`tLoginGate = 4` 被 `SendGameCenterMsg` 用到）。</summary>
    public enum TProgamType
    {
        tDBServer = 0, tLoginSrv = 1, tLogServer = 2, tM2Server = 3, tLoginGate = 4,
        tLoginGate1 = 5, tSelGate = 6, tSelGate1 = 7, tRunGate = 8, tRunGate1 = 9, tRunGate2 = 10,
        tRunGate3 = 11, tRunGate4 = 12, tRunGate5 = 13, tRunGate6 = 14, tRunGate7 = 15
    }

    // ---- Misc.pas:8-10 / :117 VER_TYPE 相关（LoginGate 副本特有）----

    /// <summary>`Misc.pas:9 VER_TYPE = 0`（0: 通用版；1: 专用版）。LoginGate 副本为 **0**。</summary>
    public const int VER_TYPE = 0;

    /// <summary>`Misc.pas:10 VER_VERSION = '2025/04/28'`。</summary>
    public const string VER_VERSION = "2025/04/28";

    /// <summary>`Misc.pas:13 PROGRAM_NAME = '登陆网关'`（`{$IF VER_TYPE = 0}` 分支）。</summary>
    public const string PROGRAM_NAME = "登陆网关";

    /// <summary>`Misc.pas:14 VER_TYPE1_TEST = 1`（VER_TYPE=0 分支的值）。</summary>
    public const int VER_TYPE1_TEST = 1;

    /// <summary>
    /// `Misc.pas:116` `g_nProtocolKey3: LongWord = 0`（**在 `{$IF VER_TYPE = 1}` 块内**，
    /// `VER_TYPE=0` 时该变量**不参与编译**；托管侧保留为常量 0 并在报告中登记"未被引用"）。
    /// </summary>
    public const uint g_nProtocolKey3 = 0;

    /// <summary>
    /// `Misc.pas:146` `g_nProtocolKey2: DWORD = 0`（在 `implementation` 段、`VER_TYPE` 之外）。
    /// </summary>
    public static uint g_nProtocolKey2 = 0;

    // =====================================================================================
    // Misc.pas:155-179 CloseIPConnect(const nRemoteIP: Integer)
    // 遍历 g_UserList（USER_ARRAY_COUNT 定长数组），命中条件：
    //   UserObj <> nil and UserObj.m_tLastGameSvr <> nil and UserObj.m_tLastGameSvr.Active
    //   and UserObj.m_pUserOBJ.nIPAddr = nRemoteIP
    // 命中后按 m_fHandleLogin >= 2 分派：发 SM_OUTOFCONNECTION + 置 KickFlag，否则 FreeSocket
    // =====================================================================================
    public static void CloseIPConnect(int nRemoteIP, IReadOnlyList<IRest11SessionObj?> g_UserList,
                                      IRest11EnforcementChannel channel)
    {
        if (!channel.ServiceStarted)                                       // :160 if not g_fServiceStarted then Exit
            return;
        for (int n = 0; n <= g_UserList.Count - 1; n++)                    // :162 for n := 0 to USER_ARRAY_COUNT - 1
        {
            IRest11SessionObj? UserObj = g_UserList[n];                    // :164
            if (UserObj != null                                             // :165 (UserObj <> nil)
                && UserObj.LastGameSvrActive                                // :166-167 (m_tLastGameSvr <> nil) and .Active
                && UserObj.IPAddr == nRemoteIP)                             // :168 (m_pUserOBJ.nIPAddr = nRemoteIP)
            {
                if (UserObj.HandleLogin >= 2)                               // :170
                {
                    channel.SendOutOfConnection(UserObj, UserObj.SvrObject);// :172 SendDefMessage(SM_OUTOFCONNECTION, m_nSvrObject, 0,0,0,'')
                    UserObj.KickFlag = true;                                // :173
                }
                else
                {
                    channel.FreeSocket(UserObj.Socket);                     // :176 SHSocket.FreeSocket(m_pUserOBJ._SendObj.Socket)
                }
            }
        }
    }

    // =====================================================================================
    // Misc.pas:181-205 KickUser(const nRemoteIP: Integer): Boolean
    // 原文如此：开关关闭时返回 True；mDisconnect 分支**只**返回 False（不封禁）
    // =====================================================================================
    public static bool KickUser(int nRemoteIP, Rest11LoginGateConfig g_pConfig,
                                IReadOnlyList<IRest11SessionObj?> g_UserList,
                                IRest11EnforcementChannel channel)
    {
        bool Result = true;                                                // :183
        if (g_pConfig.m_fKickOverPacketSize)                               // :184
        {
            switch ((Rest11TBlockIPMethod)g_pConfig.m_tBlockIPMethod)      // :186 case g_pConfig.m_tBlockIPMethod of
            {
                case Rest11TBlockIPMethod.mDisconnect:                     // :187
                    Result = false;                                        // :189
                    break;
                case Rest11TBlockIPMethod.mBlock:                          // :191
                    channel.AddToTempBlockIPList(nRemoteIP);               // :193 AddToTempBlockIPList(nRemoteIP)
                    CloseIPConnect(nRemoteIP, g_UserList, channel);        // :194 CloseIPConnect(nRemoteIP)
                    Result = false;                                        // :195
                    break;
                case Rest11TBlockIPMethod.mBlockList:                      // :197
                    channel.AddToBlockIPList(nRemoteIP);                   // :199 AddToBlockIPList(nRemoteIP)
                    CloseIPConnect(nRemoteIP, g_UserList, channel);        // :200 CloseIPConnect(nRemoteIP)
                    Result = false;                                        // :201
                    break;
            }
        }
        return Result;
    }

    // =====================================================================================
    // Misc.pas:207-224 KickUser(const UserObj: TSessionObj)
    // 注意顺序：先 FreeSocket（:211）再置 m_fKickFlag（:212），与 BlockUser 相反
    // =====================================================================================
    public static void KickUser(IRest11SessionObj UserObj, Rest11LoginGateConfig g_pConfig,
                                IRest11EnforcementChannel channel)
    {
        if (g_pConfig.m_fKickOverPacketSize)                               // :209
        {
            channel.FreeSocket(UserObj.Socket);                            // :211 SHSocket.FreeSocket(UserObj.m_pUserOBJ._SendObj.Socket)
            UserObj.KickFlag = true;                                       // :212
            switch ((Rest11TBlockIPMethod)g_pConfig.m_tBlockIPMethod)      // :213
            {
                case Rest11TBlockIPMethod.mBlock:                          // :214
                    channel.AddToTempBlockIPList(UserObj.IPAddr);          // :216 AddToTempBlockIPList(UserObj.m_pUserOBJ.nIPAddr)
                    break;
                case Rest11TBlockIPMethod.mBlockList:                      // :218
                    channel.AddToBlockIPList(UserObj.IPAddr);              // :220 AddToBlockIPList(UserObj.m_pUserOBJ.nIPAddr)
                    break;
                // :222 mDisconnect 分支原文未列出（原文如此：:213-222 只处理 mBlock/mBlockList）
            }
        }
    }

    // =====================================================================================
    // Misc.pas:226-242 BlockUser(const UserObj: TSessionObj) —— 不 FreeSocket，只置标志 + 封禁
    // =====================================================================================
    public static void BlockUser(IRest11SessionObj UserObj, Rest11LoginGateConfig g_pConfig,
                                 IRest11EnforcementChannel channel)
    {
        if (g_pConfig.m_fKickOverPacketSize)                               // :228
        {
            UserObj.KickFlag = true;                                       // :230
            switch ((Rest11TBlockIPMethod)g_pConfig.m_tBlockIPMethod)      // :231
            {
                case Rest11TBlockIPMethod.mBlock:                          // :232
                    channel.AddToTempBlockIPList(UserObj.IPAddr);          // :234
                    break;
                case Rest11TBlockIPMethod.mBlockList:                      // :236
                    channel.AddToBlockIPList(UserObj.IPAddr);              // :238
                    break;
                // :240 mDisconnect 分支原文未列出（原文如此）
            }
        }
    }

    // =====================================================================================
    // Misc.pas:244-250 ReverseIP —— 4 字节反转
    // =====================================================================================
    public static uint ReverseIP(uint dwIP) => Rest11LoginGateNet.ReverseIP(dwIP);

    // =====================================================================================
    // Misc.pas:252-273 AnsiStrToVal —— 从字符串起解析十进制整数，nPos 返回消耗的字符数
    // 原文 `while ((c >= '0') and (c <= '9'))` 靠 PChar 的 #0 终止（:265-271）
    // =====================================================================================
    public static int AnsiStrToVal(string? nPtr, out int nPos)
    {
        nPos = 0;                                                          // :258
        int Result = 0;                                                    // :259
        if (nPtr == null)                                                  // :260
            return Result;
        int idx = 0;                                                       // tPtr := nPtr (:262)
        int total = 0;                                                     // :264
        while (idx < nPtr.Length && nPtr[idx] >= '0' && nPtr[idx] <= '9')  // :265 while ((c >= '0') and (c <= '9'))
        {
            total = 10 * total + ((byte)nPtr[idx] - (byte)'0');            // :267
            idx++;                                                         // :268 inc(tPtr)
            nPos++;                                                        // :269 inc(nPos)
        }
        Result = total;                                                    // :272
        return Result;
    }

    // =====================================================================================
    // Misc.pas:275-286 SendGameCenterMsg
    //   nParam := MakeLong(Word(tLoginGate), wIdent);            // :280
    //   SendData.cbData := Length(sSendMsg) + 1;                 // :281（含结尾 #0）
    //   SendMessage(g_hGameCenterHandle, WM_COPYDATA, nParam, ...) // :284
    // LoginGate 副本**没有** SelGate 的 g_boNetComGate → tSelGate1 分支（差异必须保留）
    // =====================================================================================
    public static void SendGameCenterMsg(ushort wIdent, string sSendMsg, IRest11GameCenterChannel channel)
    {
        int nParam = DelphiRTL.MakeLong((int)TProgamType.tLoginGate, wIdent);   // :280
        channel.SendCopyData(Rest11LoginGateNet.g_hGameCenterHandle, nParam, sSendMsg); // :284
    }

    // =====================================================================================
    // Misc.pas:288-318 CheckAccountName —— 字符范围 '0'..'z' 的白名单 + GBK 双字节特例
    // Delphi 1-based 索引 → C# 0-based 等值换算
    // =====================================================================================
    public static bool CheckAccountName(string sName)
    {
        bool Result = false;                                              // :293
        if (sName == "")                                                  // :294
            return Result;
        Result = true;                                                    // :296
        int nLen = sName.Length;                                          // :297
        int I = 1;                                                        // :298 Delphi 1-based
        while (true)                                                      // :299
        {
            if (I > nLen)                                                 // :301
                break;                                                    // :302
            char ch = sName[I - 1];
            if (ch < '0' || ch > 'z')                                     // :303
            {
                Result = false;                                           // :305
                if (ch >= '\u00B0' && ch <= '\u00C8')                     // :306  #$B0..#$C8
                {
                    I++;                                                  // :308
                    if (I <= nLen)                                        // :309
                    {
                        char ch2 = sName[I - 1];
                        if (ch2 >= '\u00A1' && ch2 <= '\u00FE')           // :310  #$A1..#$FE
                            Result = true;                                // :311
                    }
                }
                if (!Result)                                              // :313
                    break;                                                // :314
            }
            I++;                                                          // :316
        }
        return Result;
    }
}

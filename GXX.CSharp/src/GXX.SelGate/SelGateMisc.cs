using System;
using System.Collections.Generic;
using GXX.Core.Rtl;

namespace GXX.SelGate;

/// <summary>
/// SelGate Misc.pas → SelGateMisc.cs
/// 1:1 逐字移植（:10-33 常量/枚举/函数声明、:41-137、:139-160、:162-177、:179-209）。
///
/// 与 GatewayKit 的差异（必须保留）：
///   · GatewayKit.GateService 只有 AddBlockIP(ip)（GateService.cs:224-227）与简化 CheckIP；
///     原版 KickUser/BlockUser 是**两条不同的封禁路径**且都受 m_fKickOverPacketSize 总开关约束
///     （Misc.pas:71、:96、:115），并按 m_tBlockIPMethod 三分支走 AddToTempBlockIPList / AddToBlockIPList。
///   · KickUser(Integer) 在 mBlock/mBlockList 分支**总是返回 False**，只有 mDisconnect 分支返回 False
///     且 **不做任何封禁**（原文如此：:74-77）；开关关闭时返回 True。
/// </summary>
public static class CSelGateMisc
{
    /// <summary>Misc.pas:11-14 服务器↔GameCenter 消息标识。</summary>
    public const int SG_FORMHANDLE = 1000; // :11 服务器HANLD
    public const int SG_STARTNOW = 1001;   // :12 正在启动服务器...
    public const int SG_STARTOK = 1002;    // :13 服务器启动完成...
    public const int SG_ACTIVE = 1003;     // :14

    /// <summary>Misc.pas:16 GS_QUIT = 2000（关闭）。</summary>
    public const int GS_QUIT = 2000;

    /// <summary>Misc.pas:19-21 TProgamType。</summary>
    public enum TProgamType
    {
        tDBServer = 0, tLoginSrv = 1, tLogServer = 2, tM2Server = 3, tLoginGate = 4,
        tLoginGate1 = 5, tSelGate = 6, tSelGate1 = 7, tRunGate = 8, tRunGate1 = 9, tRunGate2 = 10,
        tRunGate3 = 11, tRunGate4 = 12, tRunGate5 = 13, tRunGate6 = 14, tRunGate7 = 15
    }

    // =====================================================================================
    // Misc.pas:41-66 CloseIPConnect —— 遍历用户表关连接（过滤逻辑见 CSelGateIPFilter.CloseIPConnect）
    // =====================================================================================
    public static void CloseIPConnect(int nRemoteIP, IReadOnlyList<ISelSessionHost?> g_UserList,
                                      Action<ISelSessionHost, int> sendOutOfConnection,
                                      Action<ISelSessionHost> freeSocket)
        => CSelGateIPFilter.CloseIPConnect(nRemoteIP, g_UserList, sendOutOfConnection, freeSocket);

    // =====================================================================================
    // Misc.pas:68-92 KickUser(nRemoteIP): Boolean
    // =====================================================================================
    public static bool KickUser(int nRemoteIP, CConfigMgr g_pConfig, CSelGateIPFilter ipFilter,
                                IReadOnlyList<ISelSessionHost?> g_UserList,
                                Action<ISelSessionHost, int> sendOutOfConnection,
                                Action<ISelSessionHost> freeSocket)
    {
        bool Result = true;                                            // :70
        if (g_pConfig.m_fKickOverPacketSize)                           // :71
        {
            switch ((TBlockIPMethod)g_pConfig.m_tBlockIPMethod)        // :73
            {
                case TBlockIPMethod.mDisconnect:                       // :74
                    Result = false;                                    // :76
                    break;
                case TBlockIPMethod.mBlock:                            // :78
                    ipFilter.AddToTempBlockIPList(nRemoteIP);          // :80
                    CloseIPConnect(nRemoteIP, g_UserList, sendOutOfConnection, freeSocket); // :81
                    Result = false;                                    // :82
                    break;
                case TBlockIPMethod.mBlockList:                        // :84
                    ipFilter.AddToBlockIPList(nRemoteIP);              // :86
                    CloseIPConnect(nRemoteIP, g_UserList, sendOutOfConnection, freeSocket); // :87
                    Result = false;                                    // :88
                    break;
            }
        }
        return Result;
    }

    // =====================================================================================
    // Misc.pas:94-111 KickUser(const UserObj: TSessionObj)
    // 注意：先 FreeSocket 再置 m_fKickFlag（:98-99），与 KickUser(Integer) 的顺序相反
    // =====================================================================================
    public static void KickUser(ISelSessionHost UserObj, CConfigMgr g_pConfig, CSelGateIPFilter ipFilter,
                                Action<ISelSessionHost> freeSocket)
    {
        if (g_pConfig.m_fKickOverPacketSize)                           // :96
        {
            freeSocket(UserObj);                                      // :98 SHSocket.FreeSocket(...)
            UserObj.KickFlag = true;                                  // :99
            switch ((TBlockIPMethod)g_pConfig.m_tBlockIPMethod)        // :100
            {
                case TBlockIPMethod.mBlock:                            // :101
                    ipFilter.AddToTempBlockIPList(UserObj.IPAddr);     // :103
                    break;
                case TBlockIPMethod.mBlockList:                        // :105
                    ipFilter.AddToBlockIPList(UserObj.IPAddr);         // :107
                    break;
                // mDisconnect 分支原文未列出（原文如此：:100-109 只处理 mBlock/mBlockList）
            }
        }
    }

    // =====================================================================================
    // Misc.pas:113-129 BlockUser(const UserObj: TSessionObj) —— 不 FreeSocket，只置标志 + 封禁
    // =====================================================================================
    public static void BlockUser(ISelSessionHost UserObj, CConfigMgr g_pConfig, CSelGateIPFilter ipFilter)
    {
        if (g_pConfig.m_fKickOverPacketSize)                           // :115
        {
            UserObj.KickFlag = true;                                  // :117
            switch ((TBlockIPMethod)g_pConfig.m_tBlockIPMethod)        // :118
            {
                case TBlockIPMethod.mBlock:                            // :119
                    ipFilter.AddToTempBlockIPList(UserObj.IPAddr);     // :121
                    break;
                case TBlockIPMethod.mBlockList:                        // :123
                    ipFilter.AddToBlockIPList(UserObj.IPAddr);         // :125
                    break;
            }
        }
    }

    // =====================================================================================
    // Misc.pas:139-160 AnsiStrToVal —— 从 PChar 起解析十进制整数，nPos 返回消耗的字符数
    // 原文 `while ((c >= '0') and (c <= '9'))` 对空串会读到 #0 直接停（:152）
    // =====================================================================================
    public static int AnsiStrToVal(string? nPtr, out int nPos)
    {
        nPos = 0;                                                 // :145
        int Result = 0;                                           // :146
        if (nPtr == null)                                         // :147
            return Result;
        int idx = 0;                                              // tPtr := nPtr (:149)
        int total = 0;                                            // :151
        while (idx < nPtr.Length && nPtr[idx] >= '0' && nPtr[idx] <= '9') // :152
        {
            total = 10 * total + (byte)nPtr[idx] - (byte)'0';     // :154
            idx++;                                                // :155 inc(tPtr)
            nPos++;                                               // :156 inc(nPos)
        }
        Result = total;                                           // :159
        return Result;
    }

    // =====================================================================================
    // Misc.pas:162-177 SendGameCenterMsg —— WM_COPYDATA 到 g_hGameCenterHandle
    // 托管侧以 <see cref="ISelGameCenterChannel"/> 表达 SendMessage(WM_COPYDATA, ...) 语义。
    // 接缝：待 AppMain.pas（TFrmMain）移植后接入真实窗体句柄。
    // =====================================================================================
    public static void SendGameCenterMsg(ushort wIdent, string sSendMsg, ISelGameCenterChannel channel)
    {
        int nParam;
        if (!SelGateGlobals.g_boNetComGate)                                                 // :167
            nParam = DelphiRTL.MakeLong((int)TProgamType.tSelGate, wIdent);                 // :168
        else
            nParam = DelphiRTL.MakeLong((int)TProgamType.tSelGate1, wIdent);                // :170

        // :172-174 SendData.cbData := Length(sSendMsg) + 1; lpData := StrCopy(...)（含结尾 #0）
        channel.SendCopyData(SelGateGlobals.g_hGameCenterHandle, nParam, sSendMsg);          // :175
    }

    // =====================================================================================
    // Misc.pas:179-209 CheckAccountName —— 字符范围 '0'..'z' 的白名单 + GBK 双字节特例
    // 原文如此：范围检查 (sName[I] < '0') or (sName[I] > 'z') 极宽松（:194），
    // 因此大写字母、':;<=>?@' 等都被放行；只有越界字节才进入 GBK 双字节回退判定（:197-203）。
    // Delphi 1-based 索引 → C# 0-based 等值换算。
    // =====================================================================================
    public static bool CheckAccountName(string sName)
    {
        bool Result = false;                                      // :184
        if (sName == "")                                          // :185
            return Result;
        Result = true;                                            // :187
        int nLen = sName.Length;                                  // :188
        int I = 1;                                                // :189 Delphi 1-based
        while (true)                                              // :190
        {
            if (I > nLen)                                         // :192
                break;                                            // :193
            char ch = sName[I - 1];
            if (ch < '0' || ch > 'z')                             // :194
            {
                Result = false;                                   // :196
                if (ch >= '\u00B0' && ch <= '\u00C8')             // :197  #$B0..#$C8
                {
                    I++;                                          // :199
                    if (I <= nLen)                                // :200
                    {
                        char ch2 = sName[I - 1];
                        if (ch2 >= '\u00A1' && ch2 <= '\u00FE')   // :201  #$A1..#$FE
                            Result = true;                        // :202
                    }
                }
                if (!Result)                                      // :204
                    break;                                        // :205
            }
            I++;                                                  // :207
        }
        return Result;
    }
}

/// <summary>
/// Misc.pas:175 SendMessage(g_hGameCenterHandle, WM_COPYDATA, nParam, @SendData) 的托管接缝。
/// 接缝：待 AppMain.pas（TFrmMain）/ SDK.pas 消息泵移植后接入。
/// </summary>
public interface ISelGameCenterChannel
{
    /// <summary>对应 WM_COPYDATA：nParam = MakeLong(程序类型, wIdent)，文本为 sSendMsg + '\0'。</summary>
    void SendCopyData(IntPtr hWnd, int nParam, string sSendMsg);
}

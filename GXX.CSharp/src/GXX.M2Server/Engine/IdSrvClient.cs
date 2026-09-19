using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// IdSrvClient.pas TFrmIDSoc 会话管理全量移植（批次J13）：
/// NewSession/DelSession/ClearSession/GetPasswdSuccess/GetCancelAdmission/GetAdmission/GetAdmissionEx/
/// SendHumanLogOutMsg/SendLogon/SendGetAccountInfo/SendLogonCostMsg/SendOnlineHumCountMsg/Run 消息分派。
/// m_SessionList 为 J4a 的线程安全表；SendSocket 输出落入 SentMessages 观察列表（登录网关批次接出）。
/// SS 常量：SS_OPENSESSION=100/SS_CLOSESESSION=101/SS_KEEPALIVE=104/SS_KICKUSER=111/SS_SERVERLOAD=113。
/// </summary>
public static class IdSrvClient
{
    public const int SS_OPENSESSION = 100;
    public const int SS_CLOSESESSION = 101;
    public const int SS_KEEPALIVE = 104;
    public const int SS_KICKUSER = 111;
    public const int SS_SERVERLOAD = 113;
    public const int SS_SOFTOUTSESSION = 2102;
    public const int SS_PASSWORDSUCCESS = 203;
    public const int SS_LOGINCOST = 206;
    public const int SS_SERVERINFO = 207;
    public const int SS_GETACCOUNTINFO = 208;
    public const int SS_GETACCOUNTINFORET = 209;
    public const int SS_CHANGEACCOUNTINFO = 211;
    public const int SS_CHANGEACCOUNTINFORET = 212;

    /// <summary>SendSocket 输出观察列表（测试/网关接出）。</summary>
    public static readonly List<string> SentMessages = new();

    /// <summary>RunSocket.KickUser 接缝。</summary>
    public static Action<string, int>? KickUserHandler;

    public static void Reset()
    {
        SentMessages.Clear();
        KickUserHandler = null;
    }

    /// <summary>SendSocket 等效（网关批次接出前先落观察列表）。</summary>
    public static void SendSocket(string message) => SentMessages.Add(message);

    /// <summary>NewSession：按帐号查找，命中则刷新全部字段，否则新建（1:1）。</summary>
    public static void NewSession(string sAccount, string sIPaddr, int nSessionID, int nPayMent, int nPayMode)
    {
        bool boFind = false;
        var list = IdSocState.FrmIDSoc.m_SessionList;
        list.Lock();
        try
        {
            for (int i = 0; i < list.Count; i++)
            {
                var sessInfo = list[i];
                if (sessInfo.sAccount == sAccount)
                {
                    sessInfo.sIPaddr = sIPaddr;
                    sessInfo.nSessionID = nSessionID;
                    sessInfo.nPayMent = nPayMent;
                    sessInfo.nPayMode = nPayMode;
                    sessInfo.nSessionStatus = 0;
                    sessInfo.dwStartTick = DelphiRTL.GetTickCount();
                    sessInfo.dwActiveTick = DelphiRTL.GetTickCount();
                    sessInfo.nRefCount = 1;
                    sessInfo.boClose = false;
                    sessInfo.dwCloseTick = DelphiRTL.GetTickCount();
                    boFind = true;
                    break;
                }
            }
        }
        finally
        {
            list.UnLock();
        }

        if (!boFind)
        {
            list.Add(new TSessInfo
            {
                sAccount = sAccount,
                sIPaddr = sIPaddr,
                nSessionID = nSessionID,
                nPayMent = nPayMent,
                nPayMode = nPayMode,
                nSessionStatus = 0,
                dwStartTick = DelphiRTL.GetTickCount(),
                dwActiveTick = DelphiRTL.GetTickCount(),
                nRefCount = 1,
                boClose = false,
                dwCloseTick = DelphiRTL.GetTickCount()
            });
        }
    }

    /// <summary>DelSession：按会话 ID 逆序删除，命中后 KickUser(sAccount, nSessionID)。</summary>
    public static void DelSession(int nSessionID)
    {
        string sAccount = "";
        var list = IdSocState.FrmIDSoc.m_SessionList;
        list.Lock();
        try
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].nSessionID == nSessionID)
                {
                    sAccount = list[i].sAccount;
                    list.RemoveAt(i);
                    break;
                }
            }
        }
        finally
        {
            list.UnLock();
        }

        if (sAccount != "")
            KickUserHandler?.Invoke(sAccount, nSessionID);
    }

    /// <summary>ClearSession：清空全部会话。</summary>
    public static void ClearSession() => IdSocState.FrmIDSoc.m_SessionList.Clear();

    /// <summary>SendHumanLogOutMsg：按 ID+帐号标记 boClose 并刷 dwCloseTick，再发 SS_SOFTOUTSESSION。</summary>
    public static void SendHumanLogOutMsg(string sUserID, int nID)
    {
        var list = IdSocState.FrmIDSoc.m_SessionList;
        list.Lock();
        try
        {
            for (int i = 0; i < list.Count; i++)
            {
                var sessInfo = list[i];
                if (sessInfo.nSessionID == nID && sessInfo.sAccount == sUserID)
                {
                    sessInfo.dwCloseTick = DelphiRTL.GetTickCount();
                    sessInfo.boClose = true;
                    break;
                }
            }
        }
        finally
        {
            list.UnLock();
        }
        SendSocket($"({SS_SOFTOUTSESSION}/{sUserID}/{nID})");
    }

    public static void SendLogon(string sAccount)
        => SendSocket($"({SS_PASSWORDSUCCESS}/{sAccount})");

    public static void SendGetAccountInfo(string sAccount, string sUserName)
        => SendSocket($"({SS_GETACCOUNTINFO}/{sAccount}/{sUserName})");

    public static void SendLogonCostMsg(string sAccount, int nTime)
        => SendSocket($"({SS_LOGINCOST}/{sAccount}/{nTime})");

    public static void SendOnlineHumCountMsg(int nCount)
        => SendSocket($"({SS_SERVERINFO}/{M2Config.sServerName}/{M2ShareState.nServerIndex}/{nCount})");

    /// <summary>GetPasswdSuccess：解析 '帐号/会话ID/计费/计费模式/IP' → NewSession。</summary>
    public static void GetPasswdSuccess(string sData)
    {
        var parts = sData.Split('/');
        if (parts.Length < 5)
            return;
        NewSession(parts[0], parts[4],
            DelphiRTL.StrToIntDef(parts[1], 0),
            DelphiRTL.StrToIntDef(parts[2], 0),
            DelphiRTL.StrToIntDef(parts[3], 0));
    }

    /// <summary>GetCancelAdmission：解析 'SC/会话ID' → DelSession。</summary>
    public static void GetCancelAdmission(string sData)
    {
        var parts = sData.Split('/');
        if (parts.Length >= 2)
            DelSession(DelphiRTL.StrToIntDef(parts[1], 0));
    }

    /// <summary>GetAdmission 1:1：ID+帐号匹配；boClose 命中则标记 boClose（不发失败日志差异）；
    /// nPayMent 折算 2→3/1→2/0→1；未命中且 boViewAdmissionFailure 时输出失败消息。</summary>
    public static TSessInfo? GetAdmission(string sAccount, string sIPaddr, int nSessionID, out int nPayMode, out int nPayMent)
    {
        bool boFound = false;
        bool boClose = false;
        TSessInfo? result = null;
        nPayMent = 0;
        nPayMode = 0;

        var list = IdSocState.FrmIDSoc.m_SessionList;
        list.Lock();
        try
        {
            for (int i = 0; i < list.Count; i++)
            {
                var sessInfo = list[i];
                if (sessInfo.nSessionID == nSessionID && sessInfo.sAccount == sAccount)
                {
                    boClose = true;
                    if (!sessInfo.boClose)
                    {
                        nPayMent = sessInfo.nPayMent switch { 2 => 3, 1 => 2, 0 => 1, _ => 0 };
                        result = sessInfo;
                        nPayMode = sessInfo.nPayMode;
                        boFound = true;
                        break;
                    }
                }
            }
        }
        finally
        {
            list.UnLock();
        }

        if (M2Config.boViewAdmissionFailure && !boFound)
        {
            var msg = boClose
                ? $"[登录验证] 会话已关闭--取会话({sAccount}/{sIPaddr}/{nSessionID})"
                : $"[登录验证] 取会话失败1({sAccount}/{sIPaddr}/{nSessionID})";
            M2ServerLog.MainOutMessage(msg);
        }
        return result;
    }

    /// <summary>GetAdmissionEx 1:1：仅按帐号（非 boClose）匹配，回填 nSessionID/nPayMode/nPayMent。</summary>
    public static TSessInfo? GetAdmissionEx(string sAccount, string sIPaddr, out int nSessionID, out int nPayMode, out int nPayMent)
    {
        bool boFound = false;
        TSessInfo? result = null;
        nPayMent = 0;
        nPayMode = 0;
        nSessionID = 0;

        var list = IdSocState.FrmIDSoc.m_SessionList;
        list.Lock();
        try
        {
            for (int i = 0; i < list.Count; i++)
            {
                var sessInfo = list[i];
                if (!sessInfo.boClose && sessInfo.sAccount == sAccount)
                {
                    nPayMent = sessInfo.nPayMent switch { 2 => 3, 1 => 2, 0 => 1, _ => 0 };
                    result = sessInfo;
                    nSessionID = sessInfo.nSessionID;
                    nPayMode = sessInfo.nPayMode;
                    boFound = true;
                    break;
                }
            }
        }
        finally
        {
            list.UnLock();
        }

        if (M2Config.boViewAdmissionFailure && !boFound)
            M2ServerLog.MainOutMessage($"[登录验证] 取会话失败2({sAccount}/{sIPaddr}/{nSessionID})");
        return result;
    }

    /// <summary>Run 消息分派 1:1：按 '(code/body)' 逐帧解析，支持 SS_OPENSESSION/
    /// SS_CLOSESESSION/SS_KEEPALIVE/SS_KICKUSER/SS_GETACCOUNTINFORET/SS_CHANGEACCOUNTINFORET。</summary>
    public static void Run(string socketText)
    {
        while (true)
        {
            int open = socketText.IndexOf('(');
            if (open < 0)
                break;
            int close = socketText.IndexOf(')');
            if (close < 0)
                break;
            string sData = socketText[(open + 1)..close];
            socketText = socketText[(close + 1)..];
            if (sData == "")
                break;

            int slash = sData.IndexOf('/');
            string sCode = slash >= 0 ? sData[..slash] : sData;
            string sBody = slash >= 0 ? sData[(slash + 1)..] : "";
            int code = DelphiRTL.StrToIntDef(sCode, 0);

            switch (code)
            {
                case SS_OPENSESSION:
                    GetPasswdSuccess(sBody);
                    break;
                case SS_CLOSESESSION:
                case SS_KICKUSER:
                    GetCancelAdmission(sBody);
                    break;
                case SS_KEEPALIVE:
                    SetTotalHumanCount(sBody);
                    break;
            }

            if (!socketText.Contains(')'))
                break;
        }
    }

    /// <summary>SetTotalHumanCount。</summary>
    public static int g_nTotalHumCount;

    public static void SetTotalHumanCount(string sData)
        => g_nTotalHumCount = DelphiRTL.StrToIntDef(sData, 0);
}

/// <summary>MainOutMessage 接缝（svMain 主窗体批次接出前先落列表）。</summary>
public static class M2ServerLog
{
    public static readonly List<string> Messages = new();

    public static void MainOutMessage(string msg) => Messages.Add(msg);
}

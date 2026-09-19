using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Protocol;
using GXX.Core.Util;

namespace GXX.LoginSrv;

/// <summary>
/// LSShare.pas 最小接缝层（本批次 BasicSet/MonSoc/窗体族所依赖的片段，1:1 对齐原文）。
/// 接缝：LSShare.pas 其余部分（THttpThread / TUserInfo / TGateInfo / SessionList / GateRoute …）
/// 待 LSShare 整单元移植后接入，本文件**不**代为移植。
/// </summary>
public static class LoginSrvShare
{
    // ---------------- LSShare.pas resourcestring / const ----------------
    public const string g_sUpDateTime = "更新日期: 2025/04/28";
    public const string g_sProductName = "程序名称: GxxM2登录服务器 V1.0";
    public const string g_sProgram = "程序制作: GxxM2";
    public const string g_sWebSite = "程序网站: http://www.gxxm2.com";

    public const uint ControlMsgHeaderIdent = 0xEA59C795;

    public const int SMSG_CHECK_PASSWORD_OK = 1;
    public const int SMSG_CHECK_PASSWORD_FAIL = 2;
    public const int SMSG_RESPONSE_ACCOUNT_LIST = 3;
    public const int SMSG_EDIT_ACCOUNT_INFO_OK = 4;
    public const int SMSG_EDIT_ACCOUNT_INFO_FAIL = 5;
    public const int SMSG_HEARTBEAT = 6;

    public const int CMSG_CHECK_PASSWORD = 1;
    public const int CMSG_QUERY_ACCOUNT_LIST = 2;
    public const int CMSG_EDIT_ACCOUNT_INFO = 3;
    public const int CMSG_HEARTBEAT = 6;

    // ---------------- LSShare.pas var ----------------
    /// <summary>g_Config: TConfig（typed constant 初值 1:1，见 LSShare.pas:273-337）。</summary>
    public static TConfig g_Config = TConfig.CreateDefault();

    public static string g_ControlIPFile = "";
    public static TSafeStringList g_ControlIPList = new();
    public static string g_DisablePasswordFile = "";
    public static TStringList g_DisablePasswordList = new();

    /// <summary>g_MainMsgList（LSShare.pas:343）+ g_OutMessageCS 的等效。</summary>
    public static readonly TStringList g_MainMsgList = new();
    private static readonly object _outMessageLock = new();

    /// <summary>接缝：AccountDB.pas 的 g_AccountDB（对象生命周期由宿主/TMain 装配）。</summary>
    public static TAccountDB? g_AccountDB;

    /// <summary>接缝：MasSock.pas 的全局 FrmMasSoc。</summary>
    public static TMasSocSeam FrmMasSoc = new();

    /// <summary>LSShare.pas MainOutMessage：加时间戳写入主消息队列（1:1）。</summary>
    public static void MainOutMessage(string sMsg)
    {
        lock (_outMessageLock)
        {
            g_MainMsgList.Add("[" + DateTime.Now.ToString() + "] " + sMsg);
        }
    }

    // ---------------- LSShare.pas 函数（本批次依赖） ----------------

    /// <summary>LSShare.pas:432 Date2MyDate（Y*10000 + M*100 + D）。</summary>
    public static int Date2MyDate(DateTime Dt)
    {
        int Y = Dt.Year, M = Dt.Month, D = Dt.Day;
        return Y * 10000 + M * 100 + D;
    }

    /// <summary>LSShare.pas:440 MyDate2Date（dt &lt;= 10000000 时返回 0）。</summary>
    public static DateTime MyDate2Date(int dt)
    {
        if (dt > 10000000)
        {
            int Y = dt / 10000;
            int M = (dt - Y * 10000) / 100;
            int D = dt % 100;
            try { return new DateTime(Y, M, D); } catch { return default; }
        }
        return default;
    }

    /// <summary>LSShare.pas:455 GetCodeMsgSize。</summary>
    public static int GetCodeMsgSize(double X)
        => Math.Truncate(X) < X ? (int)Math.Truncate(X) + 1 : (int)Math.Truncate(X);

    /// <summary>LSShare.pas:589 tick_diff（uint 回绕）。</summary>
    public static uint tick_diff(uint tick_start, uint tick_end)
        => tick_end >= tick_start ? tick_end - tick_start : uint.MaxValue - tick_start + tick_end;

    /// <summary>LSShare.pas:554 GenSpaceString（注意 nSpaceCOunt &lt; length 时仍追加一个空格）。</summary>
    public static string GenSpaceString(string sStr, int nSpaceCOunt)
    {
        string Result = sStr + " ";
        for (int I = 1; I <= nSpaceCOunt - sStr.Length; I++)
            Result += " ";
        return Result;
    }

    /// <summary>测试/宿主复位（对应 Delphi 单元 initialization 的全局初值）。</summary>
    public static void ResetForTests()
    {
        g_Config = TConfig.CreateDefault();
        g_ControlIPFile = "";
        g_ControlIPList = new TSafeStringList();
        g_DisablePasswordFile = "";
        g_DisablePasswordList = new TStringList();
        g_MainMsgList.Clear();
        g_AccountDB = null;
        FrmMasSoc = new TMasSocSeam();
    }
}

/// <summary>
/// LSShare.pas TSafeHashStringList（THashedStringList + 临界区）。
/// 接缝说明：MudUtil.THashedStringList 已由 GXX.Core 提供 TStringList，
/// 此处沿用其存储语义，仅补 Lock/UnLock（对应 EnterCriticalSection/LeaveCriticalSection）。
/// </summary>
public class TSafeStringList : TStringList
{
    private readonly object _cs = new();
    public void Lock() { System.Threading.Monitor.Enter(_cs); }
    public void UnLock() { System.Threading.Monitor.Exit(_cs); }
}

/// <summary>
/// LSShare.pas TConfig（仅本批次依赖字段；其余字段待 LSShare 整单元移植）。
/// typed constant 默认值 1:1 取自 LSShare.pas:273-337。
/// </summary>
public class TConfig
{
    public TFastIniFile? IniConf;

    public bool boRemoteClose;
    public string sDBServer = "";
    public int nDBSPort;
    public string sFeeServer = "";
    public int nFeePort;
    public string sLogServer = "";
    public int nLogPort;
    public string sGateAddr = "";
    public int nGatePort;
    public string sServerAddr = "";
    public string sServerName = "";
    public int nServerPort;
    public string sMonAddr = "";
    public int nMonPort;
    public int nControlPort;
    public string sControlPassword = "";

    public bool boShowBlockIPLog;

    public string sGateIPaddr = "";
    public string sIdDir = "";
    public string sWebLogDir = "";
    public string sFeedIDList = "";
    public string sFeedIPList = "";
    public string sCountLogDir = "";
    public string sChrLogDir = "";
    public bool boTestServer;
    public bool boEnableMakingID;
    public bool boDynamicIPMode;
    public bool boEnableGetbackPassword;
    public bool boGetbackPasswordCheckAll;
    public bool boDisableIDSamePassword;
    public bool boDisableQuizSameAnswer;
    public bool boDisableIDSameL2Password;
    public bool boDisableL2SamePassword;
    public bool boDisablePwdSameChr;
    public bool boDisablePwdAllNum;
    public bool boDisablePwdAllLetter;

    public bool boAutoClearID;
    public bool boUnLockAccount;
    public int nReadyServers;

    public int dwAutoClearTime;
    public int dwUnLockAccountTime;

    public bool boShowDetailMsg;

    /// <summary>boRandomCode: array[TRandCodeType] of Boolean（初值 False,False,False,False）。</summary>
    public bool[] boRandomCode = new bool[4];

    public byte btLoginWaveValue;
    public byte btOtherWaveValue;

    public int nRandomCodeErrorMaxCount;
    public int nRandomCodeRefreshMaxCount;
    public bool boEnabledL2Password;
    public bool boChangedMACCheckL2;
    public bool boChangedIPCheckL2;
    public bool boAlwaysCheckL2;

    public uint dwProcessGateTick;
    public uint dwProcessGateTime;
    public int nRouteCount;

    public int nDataSaveDBType;
    public string sDataSaveDBServer = "";
    public ushort wDataSaveDBPort;
    public string sDataSaveDBUser = "";
    public string sDataSaveDBPassword = "";
    public string sDataSaveDataBase = "";

    public bool boNewLoginDlg;
    public bool boNewLoginInto;
    public bool boNewLoginPhone;
    public bool boNewLoginMustHasPhone;

    /// <summary>LSShare.pas:273 g_Config 的 typed constant 初值，逐字段 1:1。</summary>
    public static TConfig CreateDefault() => new()
    {
        boRemoteClose = false,
        sDBServer = "127.0.0.1",
        nDBSPort = 16300,
        sFeeServer = "127.0.0.1",
        nFeePort = 16301,
        sLogServer = "127.0.0.1",
        nLogPort = 16301,
        sGateAddr = "0.0.0.0",
        nGatePort = 5500,
        sServerAddr = "0.0.0.0",
        nServerPort = 5600,
        sMonAddr = "0.0.0.0",
        nMonPort = 3000,
        nControlPort = 0,
        sControlPassword = "bmm2",
        boShowBlockIPLog = false,
        sIdDir = @".\DB\",
        sWebLogDir = @".\Share\",
        sFeedIDList = @".\FeedIDList.txt",
        sFeedIPList = @".\FeedIPList.txt",
        sCountLogDir = @".\CountLog\",
        sChrLogDir = @".\ChrLog\",
        boTestServer = true,
        boEnableMakingID = true,
        boDynamicIPMode = false,
        boEnableGetbackPassword = true,
        boGetbackPasswordCheckAll = false,
        boDisableIDSamePassword = false,
        boDisableQuizSameAnswer = false,
        boDisableIDSameL2Password = false,
        boDisableL2SamePassword = false,
        boDisablePwdSameChr = false,
        boDisablePwdAllNum = false,
        boDisablePwdAllLetter = false,
        boAutoClearID = false,
        boUnLockAccount = true,
        nReadyServers = 0,
        boShowDetailMsg = false,
        boRandomCode = new bool[4],           // (False, False, False, False)
        btLoginWaveValue = 8,
        btOtherWaveValue = 4,
        nRandomCodeErrorMaxCount = 3,
        nRandomCodeRefreshMaxCount = 5,
        boEnabledL2Password = false,
        boChangedMACCheckL2 = false,
        boChangedIPCheckL2 = false,
        boAlwaysCheckL2 = false,
        nDataSaveDBType = 0,
        sDataSaveDBServer = "",
        wDataSaveDBPort = 3306,
        sDataSaveDBUser = "",
        sDataSaveDBPassword = "",
        sDataSaveDataBase = "",
        boNewLoginDlg = false,
        boNewLoginInto = true,
        boNewLoginPhone = true,
        boNewLoginMustHasPhone = true,
    };
}

/// <summary>MasSock.pas TFrmMasSoc 的最小接缝（m_ServerList + LoadServerAddr）。</summary>
public sealed class TMasSocSeam
{
    public readonly List<TMsgServerInfo> m_ServerList = new();

    /// <summary>接缝：FrmMasSoc.LoadServerAddr（重载 !addrtable.txt，待 MasSock 移植后接入）。</summary>
    public Action LoadServerAddr = () => { };
}

/// <summary>MasSock.pas TMsgServerInfo（字段 1:1）。</summary>
public sealed class TMsgServerInfo
{
    public string sServerName = "";
    public int nServerIndex;
    public int nOnlineCount;
    public uint dwKeepAliveTick;
}

using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>IdSrvClient.pas TGlobaSessionInfo（全局会话列表条目，GlobaSession 窗体展示）。</summary>
public class TGlobaSessionInfo
{
    public string sAccount = "";
    public string sIPaddr = "";
    public int nSessionID;
    public DateTime dAddDate;
}

/// <summary>
/// 批次J14：三个中片窗体的引擎层状态/逻辑移植：
/// ① GlobaSessionList（IdSrvClient.pas FrmIDSoc.GlobaSessionList：全局会话展示表）；
/// ② InterServerMsg.pas TFrmSrvMsg（10 槽 SrvArray 消息服务器：连接/断开/读缓冲/SendSocketMsg 广播，DecodeSocStr/MsgGetUserServerChange 在 Delphi 中即为空实现）；
/// ③ InterMsgClient.pas TFrmMsgClient（连接消息服务器 + 20 秒重连 + '(code/body)' 帧解析，SS_200..214 分支在 Delphi 中均为空 case）；
/// ④ uFrmGlobalVarEdit 的 G/A 全局变量存储（M2Share.pas GlobalVal/GlobalAVal array[0..999]）。
/// </summary>
public static class InterServerState
{
    /// <summary>FrmIDSoc.GlobaSessionList。</summary>
    public static readonly List<TGlobaSessionInfo> GlobaSessionList = new();

    /// <summary>TFrmSrvMsg.SrvArray：0..9 槽（Socket 以对象引用承载，连接时登记）。</summary>
    public sealed class TServerMsgInfo
    {
        public object? Socket;
        public string s2E0 = "";
    }

    public static readonly TServerMsgInfo[] SrvArray = MakeSrvArray();

    private static TServerMsgInfo[] MakeSrvArray()
    {
        var arr = new TServerMsgInfo[10];
        for (int i = 0; i < arr.Length; i++)
            arr[i] = new TServerMsgInfo();
        return arr;
    }

    /// <summary>TFrmSrvMsg.SendSocketMsg：向全部已连接槽广播 '(msg)'。</summary>
    public static List<string> SentMessages = new();

    public static void SendSocketMsg(string sMsg)
    {
        foreach (var slot in SrvArray)
        {
            if (slot.Socket != null)
                SentMessages.Add("(" + sMsg + ")");
        }
    }

    /// <summary>MsgServerClientConnect：占第一个空槽（Delphi 1:1，注意原文无 break，会填满所有空槽）。</summary>
    public static void ClientConnect(object socket)
    {
        foreach (var slot in SrvArray)
        {
            if (slot.Socket == null)
            {
                slot.Socket = socket;
                slot.s2E0 = "";
            }
        }
    }

    /// <summary>MsgServerClientDisconnect：清除匹配槽。</summary>
    public static void ClientDisconnect(object socket)
    {
        foreach (var slot in SrvArray)
        {
            if (slot.Socket == socket)
            {
                slot.Socket = null;
                slot.s2E0 = "";
            }
        }
    }

    /// <summary>MsgServerClientRead：按槽缓冲接收文本。</summary>
    public static void ClientRead(object socket, string text)
    {
        foreach (var slot in SrvArray)
        {
            if (slot.Socket == socket)
            {
                slot.s2E0 += text;
                return;
            }
        }
    }

    // ---- TFrmMsgClient（InterMsgClient.pas） ----

    public static string sRecvMsg = "";
    public static uint dw2D4Tick;
    public static bool Connected;
    public static bool Active;

    /// <summary>TFrmMsgClient.Run 1:1：连接则解析，否则 20 秒超时重连。</summary>
    public static void MsgClientRun()
    {
        if (Connected)
        {
            DecodeSocStr();
        }
        else if (DelphiRTL.GetTickCount() - dw2D4Tick > 20 * 1000)
        {
            dw2D4Tick = DelphiRTL.GetTickCount();
            Active = true; // MsgClient.Active := True
            Connected = true;
        }
    }

    /// <summary>TFrmMsgClient.DecodeSocStr 1:1：'(code/body)' 帧解析；SS_200..SS_214 分支在 Delphi 中均为空 case（1:1 保留为 no-op）。</summary>
    public static List<int> DispatchedCodes = new();

    public static void DecodeSocStr()
    {
        if (!sRecvMsg.Contains(')'))
            return;
        var sData = sRecvMsg;
        sRecvMsg = "";
        while (true)
        {
            int open = sData.IndexOf('(');
            if (open < 0)
                break;
            int close = sData.IndexOf(')');
            if (close < 0)
                break;
            var sc = sData[(open + 1)..close];
            sData = sData[(close + 1)..];
            if (sc == "")
                break;
            int slash = sc.IndexOf('/');
            var s10 = slash >= 0 ? sc[..slash] : sc;
            int n1C = DelphiRTL.StrToIntDef(s10, 0);
            // n20 = Str_ToInt(DeCodeString(s18), -1)：SS_200..SS_214 分支为空 case（1:1 no-op）
            DispatchedCodes.Add(n1C);
            if (!sData.Contains(')'))
                break;
        }
    }

    // ---- uFrmGlobalVarEdit：G/A 全局变量存储 ----

    public static readonly int[] GlobalVal = new int[1000];
    public static readonly string[] GlobalAVal = new string[1000];

    public static void ResetGlobalVars()
    {
        Array.Clear(GlobalVal);
        for (int i = 0; i < GlobalAVal.Length; i++)
            GlobalAVal[i] = "";
    }
}

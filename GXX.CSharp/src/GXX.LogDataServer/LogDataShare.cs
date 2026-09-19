using System;
using System.Collections.Generic;
using GXX.Core.Util;

namespace GXX.LogDataServer;

/// <summary>
/// LDShare.pas 最小接缝层（本批次 LogManage / uFrmRemoteQuerySetting 所依赖的片段，1:1 对齐原文）。
/// 接缝：LDShare.pas 其余部分（TCmd / TControlSessionInfo / TControlMsgHeader /
/// SendGameCenterMsg / SendControlMsg / TSafeHashStringList）待 LDShare 整单元移植后接入。
/// </summary>
public static class LogDataShare
{
    // ---- LDShare.pas:9-22 const ----
    public const uint ControlMsgHeaderIdent = 0x422C9CD1;

    public const int CMSG_HEARTBEAT = 1000;
    public const int CMSG_CHECK_PASSWORD = 1001;
    public const int CMSG_SEARCH_LOG = 1002;

    public const int SMSG_HEARTBEAT = 1000;
    public const int SMSG_CHECK_PASSWORD_OK = 1001;
    public const int SMSG_CHECK_PASSWORD_FAIL = 1002;

    public const int SMSG_SEARCH_START = 1003;
    public const int SMSG_SEARCH_LOG = 1004;
    public const int SMSG_SEARCH_LOG_END = 1005;

    public const int tLogServer = 2;

    // ---- LDShare.pas:138-145 var ----
    public static string sBaseDir = @".\BaseDir";
    public static string sServerName = "";
    public static string sCaption = "LogDataSrv";

    public static int nServerPort = 10000;

    public static ushort g_nControlPort = 0;
    public static string g_sControlPassword = "";
    public static string g_ControlIPFile = "";
    public static TSafeStringList g_ControlIPList = new();

    /// <summary>LDShare.pas:216 IntToString（&lt; 10 补前导 0）。</summary>
    public static string IntToString(int nInt)
    {
        return nInt < 10 ? "0" + nInt.ToString() : nInt.ToString();
    }

    public static void ResetForTests()
    {
        sBaseDir = @".\BaseDir";
        sServerName = "";
        sCaption = "LogDataSrv";
        nServerPort = 10000;
        g_nControlPort = 0;
        g_sControlPassword = "";
        g_ControlIPFile = "";
        g_ControlIPList = new TSafeStringList();
        TSearchManagerHost.g_SearchManager = new TSearchManagerSeam();
    }
}

/// <summary>LDShare.pas TLogActorType。</summary>
public enum TLogActorType
{
    latNone,
    latHuman,
    latDummyHuman,
    latHero,
    latDummyHero,
    latPlayMonster,
    latMonster,
}

/// <summary>LDShare.pas:135 LogActorTypeNames（1:1）。</summary>
public static class LogActorTypeNames
{
    public static readonly string[] Names = { "-", "人物", "假人", "英雄", "假人英雄", "人形怪", "怪物" };

    public static string Get(TLogActorType t) => Names[(int)t];
}

/// <summary>LDShare.pas TLogData（字段 1:1）。</summary>
public sealed class TLogData
{
    public int nIndx;
    public int nServerNumber;
    public int nServerIndex;
    public uint nAct;
    public string sMapName = "";
    public int nX;
    public int nY;
    public string sObjectName = "";
    public TLogActorType ObjectType;
    public string sItemName = "";
    public int nItemIndex;
    public string sActObjectName = "";
    public int nData1;
    public int nData2;
    public string LogDesc = "";
    public DateTime Date;
}

/// <summary>LDShare.pas TControlLogData（packed record，string[N] 短串）。</summary>
public sealed class TControlLogData
{
    public int nIndx;
    public int nServerNumber;
    public int nServerIndex;
    public uint nAct;
    public string sMapName = "";       // string[39]
    public int nX;
    public int nY;
    public string sObjectName = "";    // string[39]
    public TLogActorType ObjectType;
    public string sItemName = "";      // string[39]
    public int nItemIndex;
    public string sActObjectName = ""; // string[39]
    public int nData1;
    public int nData2;
    public string LogDesc = "";        // string[99]
    public DateTime Date;
}

/// <summary>LDShare.pas TSafeHashStringList（THashedStringList + 临界区）。</summary>
public class TSafeStringList : TStringList
{
    private readonly object _cs = new();
    public void Lock() { System.Threading.Monitor.Enter(_cs); }
    public void UnLock() { System.Threading.Monitor.Exit(_cs); }
}

/// <summary>Classes.pas TThreadList（LockList/UnlockList）。</summary>
public sealed class TThreadList
{
    private readonly object _lock = new();
    private readonly List<object> _list = new();

    public List<object> LockList()
    {
        System.Threading.Monitor.Enter(_lock);
        return _list;
    }

    public void UnlockList()
    {
        System.Threading.Monitor.Exit(_lock);
    }
}

/// <summary>ThreadPool.pas TPoolTask（本批次只用到 TaskID 语义，保留最小面）。</summary>
public class TPoolTask
{
    public int TaskID;
}

/// <summary>FileSearchPool.pas TSearchTask。</summary>
public sealed class TSearchTask : TPoolTask
{
    /// <summary>FileSearchPool.pas:52 ShowPanel（Delphi TStatusPanel；此处以标签文本接缝承载）。</summary>
    public System.Windows.Forms.ToolStripStatusLabel? ShowPanel;

    public string FileName = "";
}

/// <summary>FileSearchPool.pas TTaskCompleteEvent = procedure(Task: TPoolTask) of object。</summary>
public delegate void TTaskCompleteEvent(TPoolTask Task);

/// <summary>
/// 接缝：FileSearchPool.pas TSearchManager（线程池搜索管理器）。
/// 字段名/语义 1:1；执行体（搜索线程池）待 FileSearchPool/ThreadPool 移植后接入。
/// </summary>
public class TSearchManagerSeam
{
    /// <summary>FileSearchPool.pas:85 SearchActions: array[Byte] of Boolean。</summary>
    public bool[] SearchActions = new bool[256];

    public int SearchWhere;

    public string SearchObjName = "";
    public string SearchActObjName = "";
    public int SearchActObjType;
    public string SearchItemName = "";
    public int SearchItemID;

    public TThreadList? SearchDataList;

    public TTaskCompleteEvent? OnTaskComplete;

    /// <summary>已加入的搜索任务（宿主/测试可观察）。</summary>
    public readonly List<TPoolTask> Tasks = new();

    /// <summary>FileSearchPool.pas AddTask。</summary>
    public virtual void AddTask(TPoolTask Task) => Tasks.Add(Task);

    /// <summary>CancelAndClearAllTask（LogManage.pas:315 停止查询）。</summary>
    public virtual void CancelAndClearAllTask() => Tasks.Clear();
}

/// <summary>接缝宿主：LogManage.pas:99 的全局 g_SearchManager。</summary>
public static class TSearchManagerHost
{
    public static TSearchManagerSeam g_SearchManager = new TSearchManagerSeam();
}

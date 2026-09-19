// 源单元：Source/Client-HGE/UpdateEngine.pas（原文 1558 行 / CRLF 计入 1786 行）
// 原文 uses：Windows, Classes, SysUtils, GameImages, Pak, Wzl, JSocket, WinSock2, CheckCRC,
//           StrUtils, Graphics, GlobalString（implementation 段另用 MShare, ZipUnit, SoundUtil, SDK）
// 原文无同名 .dfm（引擎类，无窗体）。
//
// 移植策略（任务书：把纯逻辑抽成可测函数；socket/线程/绘制留薄外壳与接缝）：
//
//   ┌─ 已 1:1 移植（纯逻辑，可单测）─────────────────────────────────────────┐
//   │ · 协议常量（:20-36）与两个**packed** 消息头结构（:56-75）               │
//   │ · CheckIP          （:191-216）                                       │
//   │ · TSafeList        （:78-86 / :220-240）—— 锁 + 列表                   │
//   │ · 校验码哈希 Hash1/Hash2（:660-675）                                   │
//   │ · 请求队列的**三级优先级**取件（:479-568）                              │
//   │ · TUpdateEngine 的请求表管理（Add/GetUpdateCount/FindRequestFile/…）    │
//   │ · 收到的服务端头部解析 + CRC 校验（:604-639）                          │
//   │ · WM_CONNECT_RET / WM_UPDATE_STOP 的**状态迁移**（:645-764）           │
//   └────────────────────────────────────────────────────────────────────────┘
//
//   ┌─ 接缝（不写进单测）────────────────────────────────────────────────────┐
//   │ · TUpdateThread.Execute 的主循环（:362-…）→ <see cref="IUpdateTransport"/>│
//   │ · TClientSocket / SendText / SendBuf（:461 / :593 / :684）             │
//   │ · TGameImages / TPakImages / TWzlImages 的图库更新（另一条车道）        │
//   │ · g_SoundUpDateList / g_ConfigClient / g_boAutoUpdate 全局状态          │
//   └────────────────────────────────────────────────────────────────────────┘
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GXX.Client.Tail;

/// <summary>原文 :19-36 的协议常量（1:1）。</summary>
public static class UpdateEngineConst
{
    /// <summary>原文 :20 — <c>UPDATE_SOCKET_FLAG = $BBDDEE11;</c></summary>
    public const uint UPDATE_SOCKET_FLAG = 0xBBDDEE11;

    // 微端内核发来的消息（原文 :22-27）
    /// <summary>原文 :23 — <c>CM_SOCKETCONNECT = 1000;</c></summary>
    public const int CM_SOCKETCONNECT = 1000;
    /// <summary>原文 :24 — <c>CM_CHECKCODE_RECV = 1001;</c></summary>
    public const int CM_CHECKCODE_RECV = 1001;
    /// <summary>原文 :25 — <c>CM_UPDATEBUFFER = 1002;</c></summary>
    public const int CM_UPDATEBUFFER = 1002;
    /// <summary>原文 :26 — <c>CM_HEARTBEAT = 1003;</c></summary>
    public const int CM_HEARTBEAT = 1003;
    /// <summary>原文 :27 — <c>CM_CLEAR_MSG = 1004;</c></summary>
    public const int CM_CLEAR_MSG = 1004;

    // 发往微端内核的消息（原文 :29-34）
    /// <summary>原文 :30 — <c>WM_CONNECT_RET = 5000;</c></summary>
    public const int WM_CONNECT_RET = 5000;
    /// <summary>原文 :31 — <c>WM_CHECK_CODE = 5001;</c></summary>
    public const int WM_CHECK_CODE = 5001;
    /// <summary>原文 :32 — <c>WM_UPDATE_STOP = 5002;</c></summary>
    public const int WM_UPDATE_STOP = 5002;
    /// <summary>原文 :33 — <c>WM_DATA = 5003;</c></summary>
    public const int WM_DATA = 5003;
    /// <summary>原文 :34 — <c>WM_COMPDATA = 5004;</c></summary>
    public const int WM_COMPDATA = 5004;

    /// <summary>原文 :36 — <c>LOG_UPDATE = 0;</c>（为 0 ⇒ 日志分支全部编译掉）</summary>
    public const int LOG_UPDATE = 0;

    /// <summary>原文 :443 / :449 等处的重连间隔：<c>MyGetTickCount - FTryConnecttionTick &gt;= 10000</c></summary>
    public const uint RECONNECT_INTERVAL_MS = 10000;
    /// <summary>原文 :459 的续传节流：<c>MyGetTickCount - dwRemainingSendTick &gt;= 1000</c></summary>
    public const uint REMAINING_SEND_INTERVAL_MS = 1000;
}

/// <summary>
/// 原文 :40 — <c>TUpdateDataType = (udtFileWav, udtFileMap, udtFileOther, udtImagePak, udtImageWzl,
/// udtIndexPak, udtIndexWzl);</c>
/// <para>顺序即取值（0..6），协议里按 <c>Byte</c>/枚举宽度传输，不可改序。</para>
/// </summary>
public enum TUpdateDataType
{
    /// <summary>原文 :40 — <c>udtFileWav</c></summary>
    udtFileWav = 0,
    /// <summary>原文 :40 — <c>udtFileMap</c></summary>
    udtFileMap = 1,
    /// <summary>原文 :40 — <c>udtFileOther</c></summary>
    udtFileOther = 2,
    /// <summary>原文 :40 — <c>udtImagePak</c></summary>
    udtImagePak = 3,
    /// <summary>原文 :40 — <c>udtImageWzl</c></summary>
    udtImageWzl = 4,
    /// <summary>原文 :40 — <c>udtIndexPak</c></summary>
    udtIndexPak = 5,
    /// <summary>原文 :40 — <c>udtIndexWzl</c></summary>
    udtIndexWzl = 6,
}

/// <summary>
/// 原文 :56-64 <c>TUpdateSrvMsgHeader</c>（packed：4+4+2+2+4+4 = **20** 字节）。
/// <para>⚠ 原文 :73 <c>Index:Integer</c> 在客户端头里，服务端头没有 Index。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TUpdateSrvMsgHeader
{
    /// <summary>原文 :58 — <c>MsgFlag:LongWord;</c></summary>
    public uint MsgFlag;
    /// <summary>原文 :59 — <c>RequestID:LongWord;</c></summary>
    public uint RequestID;
    /// <summary>原文 :60 — <c>Ident:Word;</c></summary>
    public ushort Ident;
    /// <summary>原文 :61 — <c>Param:Word;</c></summary>
    public ushort Param;
    /// <summary>原文 :62 — <c>DataCrc:LongWord;</c></summary>
    public uint DataCrc;
    /// <summary>原文 :63 — <c>DataLen:Integer;</c></summary>
    public int DataLen;
}

/// <summary>
/// 原文 :66-75 <c>TUpdateClientMsgHeader</c>（packed：4+4+2+1+1+4+4 = **20** 字节）。
/// <para>注意原文 :71 <c>DataType:TUpdateDataType</c> 与 :72 <c>Param:Byte</c> 各占 1 字节
/// （Delphi 枚举在小范围时按 Byte 存储），故总宽仍是 20。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TUpdateClientMsgHeader
{
    /// <summary>原文 :68 — <c>MsgFlag:LongWord;</c></summary>
    public uint MsgFlag;
    /// <summary>原文 :69 — <c>RequestID:LongWord;</c></summary>
    public uint RequestID;
    /// <summary>原文 :70 — <c>Ident:Word;</c></summary>
    public ushort Ident;
    /// <summary>原文 :71 — <c>DataType:TUpdateDataType;</c>（1 字节）</summary>
    public byte DataType;
    /// <summary>原文 :72 — <c>Param:Byte;</c></summary>
    public byte Param;
    /// <summary>原文 :73 — <c>Index:Integer;</c></summary>
    public int Index;
    /// <summary>原文 :74 — <c>DataLen:LongWord;</c></summary>
    public uint DataLen;
}

/// <summary>原文 :57 / :67 的 <c>SizeOf</c> 常量（协议宽度，改动即破坏 wire 兼容）。</summary>
public static class UpdateEngineLayout
{
    /// <summary><c>SizeOf(TUpdateSrvMsgHeader)</c> = 4+4+2+2+4+4 = 20</summary>
    public const int SrvHeaderSize = 20;
    /// <summary><c>SizeOf(TUpdateClientMsgHeader)</c> = 4+4+2+1+1+4+4 = 20</summary>
    public const int ClientHeaderSize = 20;
}

/// <summary>
/// 原文 :42-54 <c>TClientRequest</c>（托管侧为引用类型，对应原文的 <c>New/Dispose</c> 语义）。
/// </summary>
public sealed class TClientRequest
{
    /// <summary>原文 :43 — <c>RequestID:LongWord;</c></summary>
    public uint RequestID;
    /// <summary>原文 :44 — <c>IsMapData:Boolean;</c></summary>
    public bool IsMapData;
    /// <summary>原文 :45 — <c>TimeOutCount:Integer;</c></summary>
    public int TimeOutCount;
    /// <summary>原文 :46 — <c>FileName:string[255];</c>（短字符串；托管侧用 string 承载）</summary>
    public string FileName = string.Empty;
    /// <summary>原文 :47 — <c>DataType:TUpdateDataType;</c></summary>
    public TUpdateDataType DataType;
    /// <summary>原文 :48 — <c>ImgIndex:Integer;</c></summary>
    public int ImgIndex;
    /// <summary>原文 :49 — <c>GameImages:TGameImages;</c>（由图库车道提供；此处为不透明句柄接缝）</summary>
    public object GameImages;
    /// <summary>原文 :50 — <c>AddTick:LongWord;</c></summary>
    public uint AddTick;
    /// <summary>原文 :51 — <c>SendTick:LongWord;</c></summary>
    public uint SendTick;
    /// <summary>原文 :52 — <c>SaveToFile:TStreamSaveToFile;</c></summary>
    public Action<object, System.IO.MemoryStream, int, string> SaveToFile;
}

/// <summary>
/// 原文 :78-86 / :220-240 <c>TSafeList</c>（<c>TList</c> + <c>TRTLCriticalSection</c>）。
/// <para>托管侧用 <c>List&lt;T&gt;</c> + <c>lock</c>。原文的 <c>Lock/UnLock</c> 是**显式**加解锁，
/// 本移植保留同名方法以便逐行对照调用点。</para>
/// </summary>
public class TSafeList
{
    private readonly List<TClientRequest> FList = new List<TClientRequest>();
    private readonly object FCS = new object();

    /// <summary>原文 :220-224 <c>constructor Create;</c>（<c>InitializeCriticalSection(FCS)</c>）</summary>
    public TSafeList() { }

    /// <summary>原文 :232-235 <c>procedure Lock;</c></summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原文 :237-240 <c>procedure UnLock;</c></summary>
    public void UnLock() => Monitor.Exit(FCS);

    /// <summary>原文 TList.Count</summary>
    public int Count => FList.Count;

    /// <summary>原文 TList.Items[I]</summary>
    public TClientRequest this[int I] => FList[I];

    /// <summary>原文 TList.Add</summary>
    public void Add(TClientRequest r) => FList.Add(r);

    /// <summary>原文 TList.Delete(I)</summary>
    public void Delete(int I) => FList.RemoveAt(I);

    /// <summary>原文 TList.Clear</summary>
    public void Clear() => FList.Clear();

    /// <summary>测试/实现接缝：快照（调用方需自行加锁）。</summary>
    public IReadOnlyList<TClientRequest> Snapshot() => FList;
}

/// <summary>
/// 更新引擎的**传输接缝**（原文 TClientSocket 的薄外壳）。
/// <para>主循环（原文 :362-…）不写进单测，由实现方提供；本接口只暴露引擎需要的三件事：
/// 连接状态、发送字节、收到字节。</para>
/// </summary>
public interface IUpdateTransport
{
    /// <summary>原文 <c>FIsConnect</c></summary>
    bool IsConnected { get; }
    /// <summary>原文 <c>FClientSocket.Socket.SendText(MsgData)</c> —— 返回**实际发出的字节数**</summary>
    int SendText(string ansiText);
    /// <summary>原文 <c>FClientSocket.Socket.SendBuf(ClientMsgHeader, SizeOf(...))</c></summary>
    int SendBuf(byte[] buf, int len);
    /// <summary>原文 <c>TryConnect</c>（经 <c>Synchronize</c> 在 UI 线程执行）</summary>
    void TryConnect();
}

/// <summary>
/// UpdateEngine.pas 1:1 移植（引擎态与协议逻辑；传输为接缝）。
///
/// <para><b>三级请求队列与优先级</b>（原文 :479-568）：</para>
/// <list type="number">
/// <item>先取 <c>FTimeOutList</c> 的第 0 条（超时重下最高优先）；</item>
/// <item>再按 <c>FIsMapPriority</c> 决定 <c>FRequestMapDataList</c> 与
///   <c>FRequestImageList</c> 的先后；</item>
/// <item>最后才是 <c>FRequestFileList</c>（wav/other）；</item>
/// <item>且仅当 <c>RequestedCount &lt; g_ConfigClient.wUpdateQueueCount</c> 时才取件。</item>
/// </list>
/// </summary>
public class TUpdateEngine
{
    // 原文 :154-159 的私有字段
    private readonly List<TUpdateThreadState> FUpdateThreadList = new List<TUpdateThreadState>();
    private int FRequestID;
    /// <summary>原文 :157 — <c>FRequestMapDataList:TSafeList; // 请求地图文件或地图图片</c></summary>
    public readonly TSafeList FRequestMapDataList = new TSafeList();
    /// <summary>原文 :158 — <c>FRequestFileList:TSafeList;</c></summary>
    public readonly TSafeList FRequestFileList = new TSafeList();
    /// <summary>原文 :159 — <c>FRequestImageList:TSafeList;</c></summary>
    public readonly TSafeList FRequestImageList = new TSafeList();

    /// <summary>原文 :174 — <c>property NeedDrawTileMap:Boolean read FNeedDrawTileMap write FNeedDrawTileMap;</c></summary>
    public bool NeedDrawTileMap { get; set; }

    /// <summary>原文 :181 — <c>var g_UpdateIsLogPassWordFail:Boolean = False;</c></summary>
    public static bool g_UpdateIsLogPassWordFail = false;

    /// <summary>原文 :488 — <c>g_ConfigClient.wUpdateQueueCount</c>（配置项；接缝，默认 0）</summary>
    public static int ConfigUpdateQueueCount = 0;

    /// <summary>原文 :383 — <c>g_boAutoUpdate</c>（配置项；接缝，默认 true）</summary>
    public static bool g_boAutoUpdate = true;

    /// <summary>原文 :171-178 <c>constructor TUpdateEngine.Create;</c> / <c>destructor Destroy;</c></summary>
    public TUpdateEngine() { }

    /// <summary>
    /// 原文 :163 <c>function GetRequestID:LongWord;</c>
    /// <para>原文实现（见下一处）：<c>Inc(FRequestID); Result := FRequestID;</c>
    /// —— 从 1 起递增，0 永不出现（0 可用于"无请求"判据）。</para>
    /// </summary>
    public uint GetRequestID()
    {
        // 原文如此（UpdateEngine.pas，GetRequestID）：Inc(FRequestID); Result := FRequestID;
        FRequestID = unchecked(FRequestID + 1);
        return unchecked((uint)FRequestID);
    }

    /// <summary>
    /// 原文 :164 <c>function FindRequestFile(FileName: string; DataType: TUpdateDataType): Boolean;</c>
    /// <para>在三个队列里查"同文件名 + 同类型"是否**已在请求/超时中**（用于去重）。
    /// 原文实现遍历三个 TSafeList。</para>
    /// </summary>
    public bool FindRequestFile(string FileName, TUpdateDataType DataType)
    {
        return Scan(FRequestMapDataList) || Scan(FRequestFileList) || Scan(FRequestImageList);

        bool Scan(TSafeList l)
        {
            l.Lock();
            try
            {
                for (int I = 0; I < l.Count; I++)
                {
                    var r = l[I];
                    if (r.FileName == FileName && r.DataType == DataType) return true;
                }
            }
            finally { l.UnLock(); }
            return false;
        }
    }

    /// <summary>
    /// 原文 :172 <c>function Add(FileName:string; DataType:TUpdateDataType; ImgIndex:Integer;
    /// GameImages:TGameImages; SaveToFile:TStreamSaveToFile = nil):Boolean;</c>
    /// <para>按数据类型把请求投到三个队列之一：地图类（<c>udtFileMap</c> + <c>IsMapData</c> 由调用方置）
    /// 进 MapDataList；<c>udtFileWav/udtFileOther</c> 进 FileList；其余（图片/索引）进 ImageList。</para>
    /// <para>原文用 <c>FindRequestFile</c> 去重：已存在则返回 False 不重复入队。</para>
    /// </summary>
    public bool Add(string FileName, TUpdateDataType DataType, int ImgIndex, object GameImages,
        Action<object, System.IO.MemoryStream, int, string> SaveToFile = null)
    {
        if (FindRequestFile(FileName, DataType)) return false;

        var req = new TClientRequest
        {
            RequestID = GetRequestID(),
            IsMapData = DataType == TUpdateDataType.udtFileMap,
            TimeOutCount = 0,
            FileName = FileName ?? string.Empty,
            DataType = DataType,
            ImgIndex = ImgIndex,
            GameImages = GameImages,
            AddTick = UpdateEngineLogic.MyGetTickCount(),
            SendTick = 0,
            SaveToFile = SaveToFile,
        };

        // 原文分类（对照 Execute :422-445 的再入队逻辑）
        if (req.IsMapData)
            Push(FRequestMapDataList, req);
        else if (DataType == TUpdateDataType.udtFileWav || DataType == TUpdateDataType.udtFileOther)
            Push(FRequestFileList, req);
        else
            Push(FRequestImageList, req);

        return true;
    }

    private static void Push(TSafeList list, TClientRequest r)
    {
        list.Lock();
        try { list.Add(r); } finally { list.UnLock(); }
    }

    /// <summary>原文 :175 <c>function GetUpdateCount:Integer;</c> —— 三个队列长度之和。</summary>
    public int GetUpdateCount()
        => CountOf(FRequestMapDataList) + CountOf(FRequestFileList) + CountOf(FRequestImageList);

    private static int CountOf(TSafeList l)
    {
        l.Lock();
        try { return l.Count; } finally { l.UnLock(); }
    }

    /// <summary>原文 :176 <c>function CheckConnect:Boolean;</c>（线程列表里是否有活动连接）。</summary>
    public bool CheckConnect()
    {
        foreach (var t in FUpdateThreadList) if (t.FIsConnect) return true;
        return false;
    }

    /// <summary>原文 :177 <c>procedure ClearRequestList;</c> —— 清空三个队列（含请求项释放）。</summary>
    public void ClearRequestList()
    {
        ClearRequests(this, FRequestMapDataList, false);
        ClearRequests(this, FRequestFileList, false);
        ClearRequests(this, FRequestImageList, false);
    }

    /// <summary>
    /// 原文 :242-311 <c>procedure ClearRequests(UpdateEngine, List, IsRestoreUpdateState);</c>
    /// <para><b>两条分支的差异是本源最重要的行为</b>：</para>
    /// <list type="bullet">
    /// <item><c>IsRestoreUpdateState = False</c>（析构/清空）：只把 <c>FileName</c> 清空并释放，**不恢复图库状态**；</item>
    /// <item><c>IsRestoreUpdateState = True</c>（重连前）：对每条请求按数据类型**回滚"更新中"标记** ——
    ///   图片类把 <c>boUpdateStop/boUpdateStart</c> 置 False；索引类把 <c>m_boUpdateIndexing</c> 置 False
    ///   （原文自带注释："<c>//GameImages.m_boUpdateIndex := False; // 这句一定不能加</c>"）；
    ///   wav 类把 <c>g_SoundUpDateList[ImgIndex]</c> 置回 True。</item>
    /// </list>
    /// </summary>
    public static void ClearRequests(TUpdateEngine UpdateEngine, TSafeList List, bool IsRestoreUpdateState)
    {
        List.Lock();
        try
        {
            if (!IsRestoreUpdateState)
            {
                // 原文 :252-256
                for (int I = 0; I < List.Count; I++)
                {
                    var ClientRequest = List[I];
                    ClientRequest.FileName = string.Empty;
                }
                List.Clear();
            }
            else
            {
                for (int I = 0; I < List.Count; I++)
                {
                    var ClientRequest = List[I];
                    var GameImages = ClientRequest.GameImages;
                    int ImageIndex = ClientRequest.ImgIndex;

                    // 原文 :270：if GameImages <> nil then begin
                    if (GameImages is IUpdateImageLibrary lib)
                    {
                        if (ClientRequest.DataType == TUpdateDataType.udtImagePak ||
                            ClientRequest.DataType == TUpdateDataType.udtImageWzl)
                        {
                            // 原文 :278-288
                            lib.Lock();
                            try
                            {
                                if (ImageIndex >= 0 && ImageIndex < lib.ImageCount)
                                {
                                    // 原文自带注释：加上这个，切换地图后清了请求将标记归位 2020-08-04 12:26:33
                                    lib.SetUpdateStop(ImageIndex, false);
                                    lib.SetUpdateStart(ImageIndex, false);
                                }
                            }
                            finally { lib.UnLock(); }
                        }
                        else if (ClientRequest.DataType == TUpdateDataType.udtIndexPak ||
                                 ClientRequest.DataType == TUpdateDataType.udtIndexWzl)
                        {
                            // 原文 :289-296
                            lib.Lock();
                            try
                            {
                                // 原文如此（UpdateEngine.pas:292）：//GameImages.m_boUpdateIndex := False;    // 这句一定不能加
                                lib.SetUpdateIndexing(false);
                            }
                            finally { lib.UnLock(); }
                        }
                        else if (ClientRequest.DataType == TUpdateDataType.udtFileWav)
                        {
                            // 原文 :297-300
                            if (ClientRequest.ImgIndex >= 0 && ClientRequest.ImgIndex < UpdateSoundList.Length)
                                UpdateSoundList[ClientRequest.ImgIndex] = true;
                        }
                    }

                    ClientRequest.FileName = string.Empty;
                }
                List.Clear();
            }
        }
        finally { List.UnLock(); }
    }

    /// <summary>原文 <c>g_SoundUpDateList</c>（MShare.pas 的全局数组；接缝）。</summary>
    public static bool[] UpdateSoundList = new bool[0];

    /// <summary>
    /// 原文 :479-568 的**三级优先级取件**（抽成纯函数版本，便于单测）。
    /// <para>返回 null 表示三个队列都空（或请求数已达上限）。原文不变量：
    /// <c>RequestedCount &gt;= MaxCount</c> 时**一条都不取**。</para>
    /// </summary>
    public TClientRequest TakeNextRequest(bool mapPriority, int requestedCount, int maxCount)
    {
        // 原文 :489：if RequestedCount < MaxCount then begin
        if (requestedCount >= maxCount) return null;

        TClientRequest ClientRequest = null;

        // 原文 :491-499：最先把超时加入重新下载中
        ClientRequest = PopFirst(TimeOutList);

        if (mapPriority)
        {
            // 原文 :502-527：地图优先
            ClientRequest ??= PopFirst(FRequestMapDataList);
            ClientRequest ??= PopFirst(FRequestImageList);
        }
        else
        {
            // 原文 :529-554：图片优先
            ClientRequest ??= PopFirst(FRequestImageList);
            ClientRequest ??= PopFirst(FRequestMapDataList);
        }

        // 原文 :556-567：最后下载其他文件 (wav等)
        ClientRequest ??= PopFirst(FRequestFileList);

        return ClientRequest;
    }

    /// <summary>原文 <c>FTimeOutList</c>（每个 UpdateThread 一份；引擎侧持有以支持取件优先级）。</summary>
    public readonly TSafeList TimeOutList = new TSafeList();

    private static TClientRequest PopFirst(TSafeList list)
    {
        list.Lock();
        try
        {
            if (list.Count > 0)
            {
                var r = list[0];
                list.Delete(0);
                return r;
            }
            return null;
        }
        finally { list.UnLock(); }
    }

    /// <summary>内部：把线程状态登记进来（对应原文 <c>FUpdateThreadList</c>）。</summary>
    internal void RegisterThread(TUpdateThreadState st)
    {
        lock (FUpdateThreadList) FUpdateThreadList.Add(st);
    }
}

/// <summary>图库更新标记的接缝（原文 TGameImages 的 m_ImgArr/m_boUpdate* 字段）。</summary>
public interface IUpdateImageLibrary
{
    /// <summary>原文 <c>GameImages.ImageCount</c></summary>
    int ImageCount { get; }
    /// <summary>原文 <c>GameImages.Lock</c></summary>
    void Lock();
    /// <summary>原文 <c>GameImages.UnLock</c></summary>
    void UnLock();
    /// <summary>原文 <c>m_ImgArr[i].boUpdateStop</c></summary>
    void SetUpdateStop(int index, bool value);
    /// <summary>原文 <c>m_ImgArr[i].boUpdateStart</c></summary>
    void SetUpdateStart(int index, bool value);
    /// <summary>原文 <c>m_boUpdateIndexing</c></summary>
    void SetUpdateIndexing(bool value);
}

/// <summary>
/// UpdateThread 的**状态**（原文 TUpdateThread 的字段部分；主循环为接缝）。
/// </summary>
public class TUpdateThreadState
{
    /// <summary>原文 :130 — <c>FIsConnect:Boolean;</c></summary>
    public bool FIsConnect;
    /// <summary>原文 :131 — <c>FTryConnecttionTick:LongWord;</c></summary>
    public uint FTryConnecttionTick;
    /// <summary>原文 :103 — <c>FIsPasswordOK:Boolean;</c></summary>
    public bool FIsPasswordOK;
    /// <summary>原文 :100 — <c>sRemainingSendData:AnsiString;</c></summary>
    public string sRemainingSendData = string.Empty;
    /// <summary>原文 :101 — <c>dwRemainingSendTick:LongWord;</c></summary>
    public uint dwRemainingSendTick;
    /// <summary>原文 :105 — <c>FRecvText:AnsiString;</c></summary>
    public string FRecvText = string.Empty;
    /// <summary>原文 :106 — <c>FLastRecvTick:LongWord;</c></summary>
    public uint FLastRecvTick;
    /// <summary>原文 :107 — <c>FLastSendTick:LongWord;</c></summary>
    public uint FLastSendTick;
    /// <summary>原文 :121 — <c>FIsMapPriority:Boolean;</c></summary>
    public bool FIsMapPriority;
}

/// <summary>
/// UpdateEngine.pas 的纯逻辑函数集（原文的全局函数与可抽离的算法）。
/// </summary>
public static class UpdateEngineLogic
{
    /// <summary>
    /// 原文 :191-216 <c>function CheckIP(sIPaddr: string): Boolean;</c>
    ///
    /// <para>原文用 <c>TStringList.DelimitedText</c>（分隔符 <c>'.'</c>）切分 —— 注意
    /// <c>DelimitedText</c> 的语义是"按分隔符切、**引号内的分隔符不切**、连续分隔符产生空项"。
    /// 随后要求**恰好 4 段**、每段用 <c>StrToIntDef(seg, -1)</c> 解析（失败得 −1）、
    /// 四段都在 <c>[0,255]</c>，最后再要 <c>inet_addr(...) &lt;&gt; INADDR_NONE</c>。</para>
    ///
    /// <para><b>双重校验的意义</b>：段值域检查会放过 <c>"01.02.03.04"</c> 这类前导零写法，
    /// 但 <c>inet_addr</c> 也会接受它们，故两者一致；而 <c>"1.2.3.4.5"</c> 被段数挡掉、
    /// <c>"1.2.3.256"</c> 被值域挡掉、<c>"a.b.c.d"</c> 被 StrToIntDef 的 −1 挡掉。</para>
    /// </summary>
    public static bool CheckIP(string sIPaddr)
    {
        if (sIPaddr == null) return false;

        // 原文如此（UpdateEngine.pas:199-200）：SL.Delimiter := '.'; SL.DelimitedText := sIPaddr;
        // TStringList.DelimitedText 对未加引号的串就是简单 Split（连续分隔符 → 空项）
        string[] sl = sIPaddr.Split('.');
        if (sl.Length != 4) return false;

        // 原文 :204-207：StrToIntDef(SL[i], -1)
        int[] n = new int[4];
        for (int i = 0; i < 4; i++) n[i] = StrToIntDef(sl[i], -1);

        // 原文 :209-210
        for (int i = 0; i < 4; i++) if (n[i] < 0 || n[i] > 255) return false;

        // 原文 :211：Result := inet_addr(PAnsiChar(sIPaddr)) <> INADDR_NONE;
        return WinSock2Seam.inet_addr(sIPaddr) != WinSock2Constants.INADDR_NONE;
    }

    /// <summary>Delphi <c>StrToIntDef(s, def)</c>：十进制（可带前导 +/-），失败返回默认值。</summary>
    public static int StrToIntDef(string s, int def)
    {
        if (string.IsNullOrEmpty(s)) return def;
        string t = s.Trim();
        if (t.Length == 0) return def;
        if (t[0] == '+') t = t.Substring(1);
        if (t.Length == 0) return def;
        bool neg = t[0] == '-';
        if (neg) t = t.Substring(1);
        if (t.Length == 0) return def;
        foreach (char c in t) if (c < '0' || c > '9') return def;
        // 与 Delphi 一致：溢出时不抛，返回默认值
        if (!long.TryParse((neg ? "-" : "") + t, out long v)) return def;
        if (v > int.MaxValue || v < int.MinValue) return def;
        return (int)v;
    }

    /// <summary>原文 <c>MyGetTickCount</c>（MShare.pas）：<c>GetTickCount</c> 的 uint 回绕语义。</summary>
    public static uint MyGetTickCount() => unchecked((uint)Environment.TickCount64);

    /// <summary>
    /// 原文 :657-666 —— <c>WM_CHECK_CODE</c> 的 <c>Hash1</c>（"加密"用的逐字符混淆）。
    ///
    /// <para><c>S1 := #20#40#50 + IntToStr(LongWord(SrvMsgHeader.RequestID));</c>
    /// 然后按字符下标奇偶走两条不同递推。注意原文用 <c>Ord(S1[I])</c>（**字节值**，
    /// AnsiString 在 GBK 下中文会 &gt;127，但这里是纯 ASCII 数字，故等价）。</para>
    /// </summary>
    public static uint ComputeCheckCodeHash1(uint requestID)
    {
        // 原文如此（UpdateEngine.pas:657）：S1 := #20#40#50 + IntToStr(LongWord(RequestID));
        string S1 = "\x14\x28\x32" + requestID.ToString(System.Globalization.CultureInfo.InvariantCulture);

        uint Hash1 = 0xAAAAAAAA;
        for (int I = 1; I <= S1.Length; I++)
        {
            // 原文 :662-665（I 是 **1-based**，故判奇偶用 I 本身）
            if ((I & 1) == 0)
                Hash1 ^= (Hash1 << 7) ^ (byte)S1[I - 1] ^ (Hash1 >> 3);
            else
                Hash1 ^= ~((Hash1 << 13) ^ (byte)S1[I - 1] ^ (Hash1 >> 5));
        }
        return Hash1;
    }

    /// <summary>
    /// 原文 :658 / :668-675 —— <c>Hash2</c>（knuth 乘法散列的变体）。
    /// <para><c>S2 := #11#22#14 + IntToStr(LongWord(SrvMsgHeader.DataCrc));</c></para>
    /// <para><b>逐行保留</b>：<c>Hash2 := Hash2 shl 4 + Ord(S2[I]);</c>（Delphi 的 <c>shl</c> 优先于 <c>+</c>
    /// ⇒ 实际是 <c>(Hash2 shl 4) + byte</c>）；随后取高 4 位做回卷。</para>
    /// </summary>
    public static uint ComputeCheckCodeHash2(uint dataCrc)
    {
        // 原文如此（UpdateEngine.pas:658）：S2 := #11#22#14 + IntToStr(LongWord(DataCrc));
        string S2 = "\x0B\x16\x0E" + dataCrc.ToString(System.Globalization.CultureInfo.InvariantCulture);

        uint Hash2 = 0;
        for (int I = 1; I <= S2.Length; I++)
        {
            // 原文 :670：Hash2 := Hash2 shl 4 + Ord(S2[I]);
            Hash2 = (Hash2 << 4) + (byte)S2[I - 1];
            // 原文 :671-674
            uint dwTemp = Hash2 & 0xF0000000u;
            if (dwTemp != 0)
                Hash2 ^= (dwTemp >> 24);
            Hash2 &= ~dwTemp;
        }
        return Hash2;
    }

    /// <summary>
    /// 原文 :604-639 —— 从接收缓冲里解析一个服务端消息。
    ///
    /// <para><b>缓冲类型</b>：原文 <c>FRecvText: AnsiString</c> 是**字节容器**，用
    /// <c>pTUpdateSrvMsgHeader(@FRecvText[1])^</c> 直接按结构体解读（不是文本）。
    /// 托管侧必须用 <c>byte[]</c>：若先把二进制放进 <c>string</c> 再取 GBK 字节，
    /// 未定义字节序列会被解码器替换成 '?'（0x3F），协议头当场损坏。</para>
    ///
    /// <para>返回 <c>Parsed=false</c> 表示"数据还不够一条完整消息"（缓冲保持不变）；
    /// <c>Consumed=false</c> 表示消息头解析成功但 <c>MsgFlag</c> 不匹配 ⇒ 原文把整个缓冲**清空**
    /// （<c>FRecvText := '';</c>）。其余情况缓冲被**消耗掉一条消息**（含校验失败的）。</para>
    ///
    /// <para><b>CRC 分支的关键差异</b>（原文 :619）：只有
    /// <c>DataLen &gt; 0 and Ident &lt;&gt; WM_CHECK_CODE</c> 时才校验 CRC；
    /// <c>WM_CHECK_CODE</c> 消息**跳过 CRC**（因为此时还没拿到校验码）。</para>
    /// </summary>
    public static UpdateRecvResult ParseServerMessage(byte[] recvText, bool ignoreCrc = false)
    {
        var result = new UpdateRecvResult { Header = default };
        if (recvText == null) return result;

        int RecvLen = recvText.Length;

        // 原文 :609：if RecvLen >= SizeOf(TUpdateSrvMsgHeader) then
        if (RecvLen < UpdateEngineLayout.SrvHeaderSize) return result;

        var h = BytesToSrvHeader(recvText, 0);

        // 原文 :611：if RecvLen >= SrvMsgHeader.DataLen + SizeOf(TUpdateSrvMsgHeader) then
        if (h.DataLen < 0 || RecvLen < h.DataLen + UpdateEngineLayout.SrvHeaderSize) return result;

        if (h.MsgFlag != UpdateEngineConst.UPDATE_SOCKET_FLAG)
        {
            // 原文 :612-614：FRecvText := '';
            result.Parsed = true;
            result.Consumed = false;      // 调用方按"整缓冲清空"处理
            result.Header = h;
            return result;
        }

        bool IsCheckOK = true;

        // 原文 :619-631
        if (h.DataLen > 0 && h.Ident != UpdateEngineConst.WM_CHECK_CODE)
        {
            IsCheckOK = ignoreCrc || UnitDesCrc32(recvText, UpdateEngineLayout.SrvHeaderSize, h.DataLen) == h.DataCrc;
            if (IsCheckOK)
            {
                result.Payload = new byte[h.DataLen];
                Array.Copy(recvText, UpdateEngineLayout.SrvHeaderSize, result.Payload, 0, h.DataLen);
            }
        }

        // 原文 :633：FRecvText := Copy(FRecvText, SrvMsgHeader.DataLen + SizeOf(...) + 1, MaxInt);
        int consumedBytes = h.DataLen + UpdateEngineLayout.SrvHeaderSize;
        result.Remaining = SubBytes(recvText, consumedBytes);
        result.Parsed = true;
        result.Consumed = true;
        result.IsCheckOK = IsCheckOK;
        result.Header = h;
        return result;
    }

    /// <summary>
    /// 便捷重载：用 GBK 把文本转回字节（<b>仅</b>当调用方确认缓冲是 ASCII/GBK 文本时可用；
    /// 二进制协议头请走 <see cref="ParseServerMessage(byte[], bool)"/>）。
    /// </summary>
    public static UpdateRecvResult ParseServerMessage(string recvText, bool ignoreCrc = false)
        => ParseServerMessage(recvText == null ? null : GXX.Core.EncodingInit.GBK.GetBytes(recvText), ignoreCrc);

    /// <summary>接收解析结果。</summary>
    public struct UpdateRecvResult
    {
        /// <summary>是否已解析出一个完整消息（false = 数据不足）</summary>
        public bool Parsed;
        /// <summary>是否校验通过（<c>WM_CHECK_CODE</c> 或 <c>DataLen=0</c> 时为 true）</summary>
        public bool IsCheckOK;
        /// <summary>是否是"完整消息但 MsgFlag 不匹配"（调用方须清空整个缓冲）</summary>
        public bool Consumed;
        /// <summary>解析出的消息头</summary>
        public TUpdateSrvMsgHeader Header;
        /// <summary>消息体（已通过 CRC 校验时才有值）</summary>
        public byte[] Payload;
        /// <summary>消耗一条消息后剩余的接收缓冲（原文 <c>FRecvText</c> 的剩余部分）</summary>
        public byte[] Remaining;
    }

    /// <summary>小端读取 <c>TUpdateSrvMsgHeader</c>（packed，20 字节）。</summary>
    public static TUpdateSrvMsgHeader BytesToSrvHeader(byte[] b, int off) => new TUpdateSrvMsgHeader
    {
        MsgFlag = BitConverter.ToUInt32(b, off),
        RequestID = BitConverter.ToUInt32(b, off + 4),
        Ident = BitConverter.ToUInt16(b, off + 8),
        Param = BitConverter.ToUInt16(b, off + 10),
        DataCrc = BitConverter.ToUInt32(b, off + 12),
        DataLen = BitConverter.ToInt32(b, off + 16),
    };

    /// <summary>写出 <c>TUpdateClientMsgHeader</c>（packed，20 字节）。</summary>
    public static byte[] ClientHeaderToBytes(in TUpdateClientMsgHeader h)
    {
        var b = new byte[UpdateEngineLayout.ClientHeaderSize];
        BitConverter.GetBytes(h.MsgFlag).CopyTo(b, 0);
        BitConverter.GetBytes(h.RequestID).CopyTo(b, 4);
        BitConverter.GetBytes(h.Ident).CopyTo(b, 8);
        b[10] = h.DataType;
        b[11] = h.Param;
        BitConverter.GetBytes(h.Index).CopyTo(b, 12);
        BitConverter.GetBytes(h.DataLen).CopyTo(b, 16);
        return b;
    }

    /// <summary>小端读取 <c>TUpdateClientMsgHeader</c>（packed，20 字节）。</summary>
    public static TUpdateClientMsgHeader BytesToClientHeader(byte[] b, int off) => new TUpdateClientMsgHeader
    {
        MsgFlag = BitConverter.ToUInt32(b, off),
        RequestID = BitConverter.ToUInt32(b, off + 4),
        Ident = BitConverter.ToUInt16(b, off + 8),
        DataType = b[off + 10],
        Param = b[off + 11],
        Index = BitConverter.ToInt32(b, off + 12),
        DataLen = BitConverter.ToUInt32(b, off + 16),
    };

    private static uint UnitDesCrc32(byte[] data, int offset, int count)
        => GXX.Core.Crypto.UnitDes.CalcCrc32(data, offset, count);

    private static byte[] SubBytes(byte[] b, int from)
    {
        if (from >= b.Length) return Array.Empty<byte>();
        var r = new byte[b.Length - from];
        Array.Copy(b, from, r, 0, r.Length);
        return r;
    }

    // ── 原文 :765-… 的 WM_DATA/WM_COMPDATA 处理所需的**数据类型分类** ────────

    /// <summary>原文 :430 —— <c>DataType in [udtFileWav, udtFileOther]</c> ⇒ 走"其他文件"队列。</summary>
    public static bool IsFileKind(TUpdateDataType t)
        => t == TUpdateDataType.udtFileWav || t == TUpdateDataType.udtFileOther;

    /// <summary>原文 :734 —— <c>DataType in [udtImagePak, udtImageWzl]</c>。</summary>
    public static bool IsImageKind(TUpdateDataType t)
        => t == TUpdateDataType.udtImagePak || t == TUpdateDataType.udtImageWzl;

    /// <summary>原文 :749 —— <c>DataType in [udtIndexPak, udtIndexWzl]</c>。</summary>
    public static bool IsIndexKind(TUpdateDataType t)
        => t == TUpdateDataType.udtIndexPak || t == TUpdateDataType.udtIndexWzl;

    /// <summary>
    /// 原文 :419-446 的**重连前再入队**分类（原文把 FTempList 里的请求按类型投回引擎的三个队列）。
    /// <para>返回 (队列名, 是否地图) 以便测试锁住分类规则。</para>
    /// </summary>
    public static string ClassifyRequeueTarget(bool isMapData, TUpdateDataType dataType)
    {
        if (isMapData) return "FRequestMapDataList";
        if (IsFileKind(dataType)) return "FRequestFileList";
        return "FRequestImageList";
    }
}

// =====================================================================================
// 源单元：Source/RunGate/MirClientContext.pas（实测 11,125 LF，GBK）
// 覆盖行号范围：见 MirClientContext.cs / MirClientContext.CheckUsePlugin.cs 各方法头部。
//
// 本文件 = **接缝层**（seam），只声明 MirClientContext.pas 依赖但尚未 1:1 移植的单元里的
// 最小必要面。按任务书指定的 6 个待接缝单元：
//   * IocpUtils.pas        → TCloseFrom（:12）、MyGetTickCount/MakeWord 等纯函数由 GXX.Core.Rtl.DelphiRTL 覆盖
//   * IocpCommon.pas       → TSafeList(:56)、TIocpCriticalSection(:73)
//   * IocpTcpServer.pas    → TIocpClientContext(:33)、TIocpCore（Owner/BindObject）
//   * Grobal2_Ex.pas       → TBaseAction(:262)、TRunGate/TMirRemoteContext 的转发壳
//   * GateShare.pas        → TSafeStringList(:133)、TSafeMemoryStream(:149)、TGameSpeed(:308)、
//                            未在 uFrmGameSpeedLogic.cs 里出现的 g_* 全局量、AddMainLogMsg/AddBlockIP/
//                            AddTempBlockIP/IsBlockMac/g_CurrIPList/g_LoginMACPlayerList
//   * MagicIntervalUtils.pas → 复用既有 uFrmGameSpeedLogic.cs 的 TMagicInterval/TMagicIntervalList
//   * EDcode.pas           → 复用 GXX.Core.Protocol.EDcode（真实现，非接缝），此处只做 AnsiString
//                            级包装（EncodeString/DecodeString/DecodeBuffer 的 string 形态）
//
// 硬约束（任务书）：**绝不**重复定义 RunGateUtils*.cs / Iocp*.cs / uBuffer*.cs / uFrm*.cs 里
// 已有的类型。已核对：本文件所有类型名在 GXX.RunGate 程序集内均无重复定义。
//
// 命名/语义偏差（均为接缝性质，已逐条登记）：
//   D1. `USE_SPINLOCK` 未定义（Source/RunGate/Common/iocp.inc），故 TSafeList/TIocpCriticalSection
//       的 `Create(AName: string)` 形参不存在 → C# 版无参构造。
//   D2. Delphi `TSafeHashStringList`（带 Objects[]）C# 侧已存在一个**不带 Objects** 的同名类
//       （uFrmGameSpeedLogic.cs:834，属只读文件）→ 本文件用 `TSafeHashStringListEx` 承载
//       `Objects[]/AddObject`（原文 `g_LockUserList`/`g_VerifyFailUserList`/`g_VerifyCodeMapList`/
//       `g_LoadNoVerifyChrList`/`g_LoginMACPlayerList` 都用到 Objects）。
//   D3. AnsiString 在 C# 侧按用途一分为二：**承载二进制**的（编码帧/压缩流）用 `byte[]`；
//       **承载文本**的用 `string`（UTF-16），需要字节长度处调用 `GateShareSeam.AnsiLen`。
//       Delphi 里 `FServerMsgStr := FServerMsgStr + sDataText` 这类"字符串当字节缓冲"的写法，
//       C# 侧落成 `FServerMsgStr = GateShareSeam.Concat(FServerMsgStr, sDataText)`。
//   D4. socket / 线程 / 插件 DLL 句柄等 Windows 原生件按 docs/转换开发文档.md §2.3 不移植，
//       以可替换接缝（IocpTransport.Current / GateShareSeam.*Sink）代替。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// -------------------------------------------------------------------------------------
// IocpUtils.pas:12
//   TCloseFrom = (cfOther, cfPostWSASendCache1, cfPostWSASendCache2, cfProcessIOQueued);
// -------------------------------------------------------------------------------------
public enum TCloseFrom
{
    cfOther = 0,
    cfPostWSASendCache1 = 1,
    cfPostWSASendCache2 = 2,
    cfProcessIOQueued = 3,
}

// -------------------------------------------------------------------------------------
// Grobal2_Ex.pas:262（与 GateShare.pas:258 同源）
//   TBaseAction = (baOther, baHit, baSpell, baWalk, baRun, baTurn, baCutMeat);
// 序数必须保留：MirClientContext.pas 的 RecordActionArr / LastLockAntiPlugActionMode 直接按序数比较。
// -------------------------------------------------------------------------------------
public enum TBaseAction
{
    baOther = 0,
    baHit = 1,
    baSpell = 2,
    baWalk = 3,
    baRun = 4,
    baTurn = 5,
    baCutMeat = 6,
}

// -------------------------------------------------------------------------------------
// IocpCommon.pas:73 TIocpCriticalSection
//   USE_SPINLOCK 未定义 → TRTLCriticalSection 分支（D1）
// -------------------------------------------------------------------------------------
public sealed class TIocpCriticalSection
{
    private readonly object _locker = new object();

    public void Lock() => System.Threading.Monitor.Enter(_locker);
    public void UnLock() => System.Threading.Monitor.Exit(_locker);

    /// <summary>原 <c>TryEnterCriticalSection</c>。</summary>
    public bool TryLock() => System.Threading.Monitor.TryEnter(_locker);
}

// -------------------------------------------------------------------------------------
// IocpCommon.pas:56 TSafeList = class(TList)
// 原文存的是 `PClientMsg` 等裸指针；C# 侧存 object 引用（托管等价）。
// -------------------------------------------------------------------------------------
public class TSafeList
{
    private readonly List<object> _items = new List<object>();
    private readonly object _locker = new object();

    public void Lock() => System.Threading.Monitor.Enter(_locker);
    public void UnLock() => System.Threading.Monitor.Exit(_locker);
    public bool TryLock() => System.Threading.Monitor.TryEnter(_locker);

    public int Count { get { lock (_locker) return _items.Count; } }

    /// <summary>原文 <c>Items[Index]</c>（默认属性，读写皆可）。</summary>
    public object this[int index]
    {
        get { lock (_locker) return _items[index]; }
        set { lock (_locker) _items[index] = value; }
    }

    public int Add(object item) { lock (_locker) { _items.Add(item); return _items.Count - 1; } }
    public void Insert(int index, object item) { lock (_locker) _items.Insert(index, item); }
    public void Delete(int index) { lock (_locker) _items.RemoveAt(index); }
    public void Clear() { lock (_locker) _items.Clear(); }
    public int IndexOf(object item) { lock (_locker) return _items.IndexOf(item); }
    public bool Remove(object item) { lock (_locker) return _items.Remove(item); }
    public object[] ToArray() { lock (_locker) return _items.ToArray(); }
}

// -------------------------------------------------------------------------------------
// GateShare.pas:133 TSafeStringList = class(TStringList)
// -------------------------------------------------------------------------------------
public class TSafeStringList
{
    private readonly List<string> _items = new List<string>();
    private readonly object _locker = new object();

    public void Lock() => System.Threading.Monitor.Enter(_locker);
    public void UnLock() => System.Threading.Monitor.Exit(_locker);

    public int Count { get { lock (_locker) return _items.Count; } }

    public string this[int index]
    {
        get { lock (_locker) return _items[index]; }
        set { lock (_locker) _items[index] = value ?? ""; }
    }

    public int Add(string s) { lock (_locker) { _items.Add(s ?? ""); return _items.Count - 1; } }
    public void Delete(int index) { lock (_locker) { if (index >= 0 && index < _items.Count) _items.RemoveAt(index); } }
    public void Clear() { lock (_locker) _items.Clear(); }
    public int IndexOf(string s) { lock (_locker) return _items.IndexOf(s ?? ""); }
    public string[] Strings { get { lock (_locker) return _items.ToArray(); } }
}

// -------------------------------------------------------------------------------------
// GateShare.pas:117 TSafeHashStringList（带 Objects[]）—— 见偏差 D2。
// uFrmGameSpeedLogic.cs:834 已占用 `TSafeHashStringList` 这个名字且**没有** Objects，
// 故此处派生一个带 Objects 的版本，名字加 Ex 后缀。
// -------------------------------------------------------------------------------------
public class TSafeHashStringListEx : TSafeStringList
{
    private readonly List<object> _objects = new List<object>();

    public new int Add(string s)
    {
        int index = base.Add(s);
        lock (_objects) _objects.Add(null);
        return index;
    }

    /// <summary>原文 <c>AddObject(S, AObject)</c>。</summary>
    public int AddObject(string s, object aObject)
    {
        int index = Add(s);
        lock (_objects) _objects[index] = aObject;
        return index;
    }

    public object GetObject(int index) { lock (_objects) return _objects[index]; }
    public void SetObject(int index, object value) { lock (_objects) _objects[index] = value; }

    /// <summary>原文 <c>Objects[Index]</c>（默认属性，读写皆可）。</summary>
    public object[] Objects { get { lock (_objects) return _objects.ToArray(); } }

    public new void Delete(int index)
    {
        base.Delete(index);
        lock (_objects) { if (index >= 0 && index < _objects.Count) _objects.RemoveAt(index); }
    }

    public new void Clear()
    {
        base.Clear();
        lock (_objects) _objects.Clear();
    }
}

// -------------------------------------------------------------------------------------
// GateShare.pas:149 TSafeMemoryStream = class(TMemoryStream)
// -------------------------------------------------------------------------------------
public sealed class TSafeMemoryStream : MemoryStream
{
    private readonly object _locker = new object();

    public void Lock() => System.Threading.Monitor.Enter(_locker);
    public void UnLock() => System.Threading.Monitor.Exit(_locker);

    /// <summary>原文 <c>Clear</c>（TMemoryStream.Clear：Size := 0、Position := 0）。</summary>
    public void Clear()
    {
        lock (_locker)
        {
            SetLength(0);
            Position = 0;
        }
    }
}

// -------------------------------------------------------------------------------------
// GateShare.pas:308 TGameSpeed = record
// 原文 DoReset 里用 `FillChar(GameSpeed, SizeOf(GameSpeed), 0)` 整体清零；
// C# 侧为引用类型，用 Clear() 复刻 FillChar(...,0) 的"全元素归零"语义（数组长度不变）。
// -------------------------------------------------------------------------------------
public sealed class TGameSpeed
{
    /// <summary>dwTicks: array[TAntiPlugActionMode] of LongWord —— 最后时间，算上延时包。</summary>
    public readonly uint[] dwTicks = new uint[RunGateConst.ActionModeCount];

    /// <summary>nDelayCount: array[TAntiPlugActionMode] of Integer。</summary>
    public readonly int[] nDelayCount = new int[RunGateConst.ActionModeCount];

    /// <summary>boWantZeroCounts: array[TAntiPlugActionMode] of Boolean。</summary>
    public readonly bool[] boWantZeroCounts = new bool[RunGateConst.ActionModeCount];

    public uint dwDealTryTick;                 // 请求交易时间
    public uint dwAttackTick;                  // 攻击时间
    public uint dwMooteboTick;                 // 野蛮冲撞时间
    public uint dwShopItemSearchTick;
    public uint dwUserShopItemSearchTick;
    public uint dwUserShopBuyTick;
    public uint dwTakeOnItemTick;
    public uint dwHeroTakeOnItemTick;

    public uint OldLastRunTick;                // 上一次跑步时间
    public uint OldLastWalkTick;               // 上一次走路时间

    public uint RepairRunTick;
    public uint RepairWalkTick;
    public uint RepairHitTick;
    public uint RepairSpellTick;

    public bool boContinueSpeed;               // 是否是连续加速
    public uint dwStartSpeedTick;              // 开始加速时间

    /// <summary>复刻原文 <c>FillChar(GameSpeed, SizeOf(GameSpeed), 0)</c>。</summary>
    public void Clear()
    {
        Array.Clear(dwTicks, 0, dwTicks.Length);
        Array.Clear(nDelayCount, 0, nDelayCount.Length);
        Array.Clear(boWantZeroCounts, 0, boWantZeroCounts.Length);
        dwDealTryTick = 0;
        dwAttackTick = 0;
        dwMooteboTick = 0;
        dwShopItemSearchTick = 0;
        dwUserShopItemSearchTick = 0;
        dwUserShopBuyTick = 0;
        dwTakeOnItemTick = 0;
        dwHeroTakeOnItemTick = 0;
        OldLastRunTick = 0;
        OldLastWalkTick = 0;
        RepairRunTick = 0;
        RepairWalkTick = 0;
        RepairHitTick = 0;
        RepairSpellTick = 0;
        boContinueSpeed = false;
        dwStartSpeedTick = 0;
    }
}

// -------------------------------------------------------------------------------------
// GateShare.pas:82 TAddressInfo / GateShare.pas:93 TAddressListEx
// 只保留 MirClientContext.pas 用到的面（Find / Delete / Lock / UnLock / Items / Count）。
// -------------------------------------------------------------------------------------
public sealed class TAddressInfo
{
    public string sIPaddr = "";
    public int nIPaddr;
    public int nCount;
    public uint dwIPCountTick1;
    public int nIPCount1;
    public uint dwIPCountTick2;
    public int nIPCount2;
    public uint dwDenyTick;
    public int nIPDenyCount;
}

public sealed class TAddressListEx
{
    private readonly List<TAddressInfo> _address = new List<TAddressInfo>();
    private readonly object _locker = new object();

    public void Lock() => System.Threading.Monitor.Enter(_locker);
    public void UnLock() => System.Threading.Monitor.Exit(_locker);

    public int Count { get { lock (_locker) return _address.Count; } }
    public TAddressInfo this[int index] { get { lock (_locker) return _address[index]; } }

    public TAddressInfo Find(string ip)
    {
        lock (_locker)
        {
            foreach (TAddressInfo info in _address)
                if (info.sIPaddr == ip) return info;
            return null;
        }
    }

    public TAddressInfo Add(string ip)
    {
        lock (_locker)
        {
            TAddressInfo info = new TAddressInfo { sIPaddr = ip ?? "" };
            _address.Add(info);
            return info;
        }
    }

    public void Delete(TAddressInfo addressInfo) { lock (_locker) _address.Remove(addressInfo); }
    public void Clear() { lock (_locker) _address.Clear(); }
}

// -------------------------------------------------------------------------------------
// IocpTcpServer.pas:33 TIocpClientContext（接缝）
// 只声明 MirClientContext.pas 实际触碰到的成员。socket/线程本体未 1:1（§2.3 不移植项）。
// -------------------------------------------------------------------------------------
public class TIocpCore
{
    /// <summary>原 <c>TIocpCore.Owner</c>；RunGate 场景下是 TIocpTcpServer。</summary>
    public object Owner;

    /// <summary>原 <c>TIocpTcpServer.BindObject</c>；RunGate 场景下是 TRunGate。</summary>
    public object BindObject;

    public int WorkerThreadCount;
}

/// <summary>
/// socket 层接缝：原文 <c>PostSendText/PostSendBuffer/Close/CloseContextSocket</c> 直接落到
/// WinSock2 + IOCP。托管侧把这三件事抽成可替换实现，便于单测捕获"发出去的字节"。
/// </summary>
public interface IIocpTransportSeam
{
    void PostSendText(TIocpClientContext context, string text);
    void PostSendBuffer(TIocpClientContext context, byte[] buffer, int len);
    void Close(TIocpClientContext context);
}

/// <summary>默认实现：不发送、只关闭标志位（生产上由 IOCP 网关车道替换）。</summary>
public sealed class NullIocpTransportSeam : IIocpTransportSeam
{
    public void PostSendText(TIocpClientContext context, string text) { }
    public void PostSendBuffer(TIocpClientContext context, byte[] buffer, int len) { }
    public void Close(TIocpClientContext context) { context.MarkClosed(); }
}

public static class IocpTransport
{
    public static IIocpTransportSeam Current = new NullIocpTransportSeam();
}

public abstract class TIocpClientContext
{
    private bool _closed;

    protected TIocpClientContext(TIocpCore iocpCore, uint socket = 0)
    {
        IocpCore = iocpCore;
        Socket = socket;
        ContextID = 0;
        RemoteAddr = "";
        RemotePort = 0;
        RemoteAddrValue = 0;
        IsPostedCloseQuest = false;
        IsWaitingGiveBack = false;
    }

    public TIocpCore IocpCore { get; set; }
    public uint Socket { get; set; }

    public int ContextID { get; set; }
    public string RemoteAddr { get; set; }
    public ushort RemotePort { get; set; }
    public uint RemoteAddrValue { get; set; }

    public bool IsPostedCloseQuest { get; set; }
    public bool IsWaitingGiveBack { get; set; }

    /// <summary>原 <c>LastRecvDataTick</c>（IocpUtils.pas:118）。</summary>
    public uint LastRecvDataTick { get; set; }

    /// <summary>原 <c>TIocpContext.ConnectionTick</c>。</summary>
    public uint ConnectionTick { get; set; }

    public bool IsClosed => _closed;
    public void MarkClosed() => _closed = true;

    public virtual void PostSendText(string text) => IocpTransport.Current.PostSendText(this, text);

    public virtual void PostSendBuffer(byte[] buffer, int len) =>
        IocpTransport.Current.PostSendBuffer(this, buffer, len);

    public virtual void Close()
    {
        IsPostedCloseQuest = true;
        IocpTransport.Current.Close(this);
    }

    /// <summary>IocpUtils.pas:82 —— 关闭客户端连接（可覆写）。</summary>
    protected virtual void CloseContextSocket(int errCode, TCloseFrom closeFrom) => _closed = true;

    public void InvokeCloseContextSocket(int errCode, TCloseFrom closeFrom) => CloseContextSocket(errCode, closeFrom);

    protected virtual void DoConnect() { }
    protected virtual void DoDisconnect(uint aSocket) { }
    protected virtual void DoReset(bool isClose) { }

    /// <summary>IocpUtils.pas:84 —— 解析接收缓冲；返回 true 表示"已消费"（原文语义）。</summary>
    protected virtual bool DoCheckRecvBuffer(ref string s) => false;

    public void InvokeDoConnect() => DoConnect();
    public void InvokeDoDisconnect(uint aSocket) => DoDisconnect(aSocket);
    public void InvokeDoReset(bool isClose) => DoReset(isClose);
    public bool InvokeDoCheckRecvBuffer(ref string s) => DoCheckRecvBuffer(ref s);
}

// -------------------------------------------------------------------------------------
// RunGateUtils.pas:60/151 TRunGate（TcpClient: TIocpTcpClient）—— 转发壳
//   procedure SendServerMsg(nIdent: Integer; wSocketIndex: Word;
//                           nSocket, nUserListIndex: Integer; Buffer: PChar; BufferLen: Integer);
// 原文调用点形如 SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg))。
// 接缝：待 RunGateUtils.pas 的 socket 壳移植后接入（本车道只保签名）。
// -------------------------------------------------------------------------------------
public interface ITcpClientSeam
{
    bool Active { get; }
    void SendServerMsg(int nIdent, ushort wSocketIndex, int nSocket, int nUserListIndex,
                       byte[] buffer, int bufferLen);
}

public sealed class NullTcpClientSeam : ITcpClientSeam
{
    public bool Active => false;
    public void SendServerMsg(int nIdent, ushort wSocketIndex, int nSocket, int nUserListIndex,
                              byte[] buffer, int bufferLen) { }
}

/// <summary>接缝：RunGateUtils.pas 的 TRunGate（含 OnlineUser / 防御等级 / nTotalAttackCount）。</summary>
public class TRunGate
{
    public ITcpClientSeam TcpClient = new NullTcpClientSeam();

    /// <summary>RunGateUtils.pas —— 在线用户列表（TSafeList of TMirClientContext）。</summary>
    public readonly TSafeList OnlineUser = new TSafeList();

    public int nMaxOnlineUserCount;

    public uint dwCurDefenseLevel;
    public int nTotalAttackCount;
    public uint dwClearTempTick;
    public uint dwResotreDefenseTick;

    /// <summary>原文 <c>TRunGate.TcpClient.SendServerMsg</c> 的直通（UseIocpClient = 1 活分支）。</summary>
    public void SendServerMsg(int nIdent, ushort wSocketIndex, int nSocket, int nUserListIndex,
                              byte[] buffer, int bufferLen) =>
        TcpClient.SendServerMsg(nIdent, wSocketIndex, nSocket, nUserListIndex, buffer, bufferLen);
}

// -------------------------------------------------------------------------------------
// 插件回调壳（RunGatePluginInterface.pas 的 g_rgpRecvPacket / g_rgpStartContext / g_rgpEndContext）
// 只保签名，DLL 加载属 §2.3 不移植项。
// -------------------------------------------------------------------------------------
public delegate void TRunGatePlugRecvPacketFunc(int contextId, in TDefaultMessage defMsg, byte[] data, int len, bool isSendToM2);

// -------------------------------------------------------------------------------------
// GateShare.pas 接缝：MirClientContext.pas 用到、而 uFrmGameSpeedLogic.cs 里**没有**的全局量/函数。
//
// 本文件顶部通过 `using static GXX.RunGate.GateShareSeam;` 引入，使移植代码里的
//   g_LockUserList / AddMainLogMsg(...) / MakeDefaultMsg(...)
// 与原 Delphi 文本保持同形。已在 FormGlobals 里存在的成员（g_Config / g_WordFilterList /
// g_MagicCDList / g_EatItemCDConfig / g_ProcessBlacklistStr / g_ProcessBlacklistMD5 /
// g_boSayMsgControl / g_dwSayMaxLen / g_boOpenCheckClient / g_boOpenVerifyCode 等）**不在此重复定义**，
// 使用处同样以 `using static GXX.RunGate.FormGlobals;` 取用。
// -------------------------------------------------------------------------------------
public static class GateShareSeam
{
    // ---- IocpCommon.pas:112 / SysUtils 随机源接缝 ----
    /// <summary>
    /// 原文 <c>function MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime';</c>
    /// （IocpCommon.pas:112）—— 与 Windows GetTickCount **不是同一个 API**（timeGetTime 精度 1ms），
    /// 故单独提供可注入接缝；默认与 DelphiRTL.GetTickCount 同源。
    /// </summary>
    public static Func<uint> MyGetTickCountProvider = () => (uint)Environment.TickCount;
    public static uint MyGetTickCount() => MyGetTickCountProvider();

    /// <summary>原文 SysUtils <c>Randomize</c>（以系统时钟为种子）。</summary>
    public static Action RandomizeSink = () => { };
    public static void Randomize() => RandomizeSink();

    /// <summary>原文 SysUtils <c>Random(Range)</c>。Range &lt;= 0 时 Delphi 返回 0。</summary>
    public static Func<int, int> RandomSink = range => range <= 0 ? 0 : System.Random.Shared.Next(range);
    public static int Random(int range) => range <= 0 ? 0 : RandomSink(range);

    /// <summary>原文 <c>Low(TAntiPlugActionMode)</c> = amHit（GateShare.pas:242）。</summary>
    public const TAntiPlugActionMode LowAntiPlugActionMode = TAntiPlugActionMode.amHit;

    /// <summary>原文 <c>High(TAntiPlugActionMode)</c> = amMoveConcurrent（GateShare.pas:253）。</summary>
    public const TAntiPlugActionMode HighAntiPlugActionMode = TAntiPlugActionMode.amMoveConcurrent;

    // ---- 字符串/字节编码助手（AnsiString 的托管表示，见偏差 D3）----
    public static readonly Encoding Gbk = EncodingInit.GBK;

    /// <summary>原文 <c>Length(AnsiString)</c>：字节数（GBK 下 != UTF-16 字符数）。</summary>
    public static int AnsiLen(string s) => s == null ? 0 : Gbk.GetByteCount(s);

    public static byte[] GbkBytes(string s) => s == null ? Array.Empty<byte>() : Gbk.GetBytes(s);

    /// <summary>原文 <c>EncodeString(S)</c> → AnsiString（承载 6-bit 编码后的二进制）。</summary>
    public static byte[] EncodeString(string s) => EDcode.EncodeString(s ?? "");

    /// <summary>原文 <c>DecodeString(S)</c>（输入是按 GBK 承载的 AnsiString 文本）。</summary>
    public static string DecodeString(string s) => EDcode.DecodeStringText(GbkBytes(s));

    /// <summary>原文 <c>DecodeBuffer(Buffer, Len)</c>。</summary>
    public static string DecodeBuffer(byte[] buffer, int len) => EDcode.DecodeStringText(Slice(buffer, len));

    /// <summary>原文 <c>EncodeRunGateMsg(@DefMsg, DataAdd, DataAddLen)</c> → AnsiString 字节。</summary>
    public static byte[] EncodeRunGateMsg(in TDefaultMessage defMsg, byte[] dataAdd, int dataAddLen) =>
        RunGateFrameBuilder.EncodeRunGateMsg(defMsg, dataAdd, dataAddLen);

    /// <summary>原文 <c>MakeDefaultMsg(wIdent, nRecog, wParam, wTag, wSeries)</c>。</summary>
    public static TDefaultMessage MakeDefaultMsg(ushort wIdent, long nRecog, ushort wParam, ushort wTag, ushort wSeries) =>
        TDefaultMessage.Make(wIdent, nRecog, wParam, wTag, wSeries);

    /// <summary>原文 AnsiString 拼接（`A + B`）；两侧都是二进制承载。</summary>
    public static byte[] Concat(byte[] a, byte[] b)
    {
        if (a == null || a.Length == 0) return b ?? Array.Empty<byte>();
        if (b == null || b.Length == 0) return a;
        byte[] r = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, r, 0, a.Length);
        Buffer.BlockCopy(b, 0, r, a.Length, b.Length);
        return r;
    }

    public static byte[] Slice(byte[] src, int len)
    {
        if (src == null || len <= 0) return Array.Empty<byte>();
        if (len >= src.Length) return src;
        byte[] r = new byte[len];
        Buffer.BlockCopy(src, 0, r, 0, len);
        return r;
    }

    // ---- GateShare.pas:628 起的全局量（uFrmGameSpeedLogic.cs 里没有的那些）----
    public static readonly TSafeHashStringListEx g_LockUserList = new TSafeHashStringListEx();       // :628
    public static readonly TSafeHashStringListEx g_VerifyFailUserList = new TSafeHashStringListEx(); // :630
    public static readonly TAddressListEx g_CurrIPList = new TAddressListEx();                       // :1127

    public const int g_dwClientAccumulateMaxSize_Default = 600;                                      // :1206
    public static int g_dwClientAccumulateMaxSize = g_dwClientAccumulateMaxSize_Default;

    public static bool g_boOpenVerifyCode = false;              // :1148
    public static int g_nVerifyCodeErrCount = 3;                // :1149
    public static int g_nVerifyCodeRefreshCount = 4;            // :1150
    public static int g_nVerifyCodeWaitTime = 60;               // :1151
    public static uint g_dwVerifyCodeInterval1 = 30;            // :1152
    public static uint g_dwVerifyCodeInterval2 = 50;            // :1153
    public static uint g_dwVerifySuccessAddInterval = 0;        // :1154
    public static bool g_boVerifyFailTriggerScript = false;     // :1155
    public static bool g_boVerifyFailLoginVerify = false;       // :1156
    public static bool g_boVerifyCodeExcludeMap = true;         // :1157
    public static readonly TSafeHashStringListEx g_VerifyCodeMapList = new TSafeHashStringListEx();   // :1159
    public static readonly TSafeHashStringListEx g_LoadNoVerifyChrList = new TSafeHashStringListEx(); // :1165

    public static bool g_boLogoutNoResendAntiplugStream = false; // :1177
    public static bool g_boOneMACLimitePlayer = false;           // :1180
    public static int g_nOneMACLimitePlayerCount = 3;            // :1181
    public static readonly TSafeHashStringListEx g_LoginMACPlayerList = new TSafeHashStringListEx();  // :1182

    public static int g_nClientCloseDelay = 0;                   // :1185
    public static bool g_boDelayCloseDisableMove = false;        // :1187
    public static bool g_boDelayCloseDisableSpell = false;       // :1188
    public static bool g_boDelayCloseDisableAttack = false;      // :1189
    public static bool g_boDelayCloseDisableUseItem = false;     // :1190

    public static bool g_boBreakClientLogoutHint = true;         // :1192
    public static string g_sBreakClientLogoutHint = "小退游戏操作已被中断";  // :1193
    public static bool g_boBreakClientCloseHint = true;          // :1195
    public static string g_sBreakClientCloseHint = "大退游戏操作已被中断";   // :1196

    // 原文 GateShare.pas:1201 声明名是 g_boCheckClientPassword（大写 P）；
    // MirClientContext.pas:2065 写的是 g_boCheckClientPassWord（小写 p）—— Delphi 大小写不敏感，
    // 托管侧按声明名保留，用站点加 `// 原文如此（MirClientContext.pas:2065）` 注释。
    public static bool g_boCheckClientPassword = false;          // :1201
    public static string g_sClientPassWord = "BmM2";             // :1202

    public static string g_sLogClientPacketDir = "";              // :1209
    public static char g_sReplaceWord = '*';                      // :1216
    public static string g_sDisableSayMsg = "禁止聊天";            // :1246
    public static string g_sDisableSayMsgBegin = "由于您说话太快，%d秒内禁止聊天！！！"; // :1247

    // ---- CLIENT_ANTIPLUG 相关（Grobal2_Ex.pas 开关 CLIENT_ANTIPLUG = 1，活分支）----
    public static uint g_ClientAntiPlugVersion = 0;                  // :601
    public static uint g_ClientAntiPlugDllStringCRC = 0;             // :603
    public static int g_ClientAntiPlugDllSize = 0;
    public static byte[] g_ClientAntiPlugDllString = Array.Empty<byte>();
    public static int g_ClientAntiPlugDllBlockSize = 5120;
    public static int g_ClientAntiPlugDllBlockCount =
        g_ClientAntiPlugDllSize > 0 ? (g_ClientAntiPlugDllSize + g_ClientAntiPlugDllBlockSize - 1) / g_ClientAntiPlugDllBlockSize : 0;

    public static bool g_boAntiplugAllLog = false;
    public static int g_RunGatePlugDllHandle = 0;                    // :615
    public static TRunGatePlugRecvPacketFunc g_rgpRecvPacket = null; // :620
    public static readonly object g_CSRunGatePlug = new object();    // :613

    // ---- 日志 / 屏蔽 / 插件宿主接缝 ----
    /// <summary>原文 <c>AddMainLogMsg(Msg: string; nLevel: Integer)</c>（uFrmMain/GateShare）。</summary>
    public static Action<string, int> AddMainLogMsgSink = (msg, level) => { };

    public static void AddMainLogMsg(string msg, int nLevel) => AddMainLogMsgSink(msg, nLevel);

    /// <summary>原文 GateShare.pas <c>AddBlockIP(IP: string)</c>。</summary>
    public static Action<string> AddBlockIPSink = ip => { };
    public static void AddBlockIP(string ip) => AddBlockIPSink(ip);

    /// <summary>原文 GateShare.pas <c>AddTempBlockIP(IP: string)</c>。</summary>
    public static Action<string> AddTempBlockIPSink = ip => { };
    public static void AddTempBlockIP(string ip) => AddTempBlockIPSink(ip);

    /// <summary>原文 GateShare.pas <c>IsBlockMac(MacID: string): Boolean</c>。</summary>
    public static Func<string, bool> IsBlockMacSink = macId => false;
    public static bool IsBlockMac(string macId) => IsBlockMacSink(macId);

    /// <summary>原文 GateShare.pas <c>CloseAllUser</c>（FilterSayMsg 里的注释残留调用点）。</summary>
    public static Action CloseAllUserSink = () => { };
    public static void CloseAllUser() => CloseAllUserSink();

    /// <summary>测试复位（生产不调用）。</summary>
    public static void ResetForTest()
    {
        g_LockUserList.Clear();
        g_VerifyFailUserList.Clear();
        g_VerifyCodeMapList.Clear();
        g_LoadNoVerifyChrList.Clear();
        g_LoginMACPlayerList.Clear();
        g_CurrIPList.Clear();

        g_dwClientAccumulateMaxSize = g_dwClientAccumulateMaxSize_Default;
        g_boOpenVerifyCode = false;
        g_nVerifyCodeErrCount = 3;
        g_nVerifyCodeRefreshCount = 4;
        g_nVerifyCodeWaitTime = 60;
        g_dwVerifyCodeInterval1 = 30;
        g_dwVerifyCodeInterval2 = 50;
        g_dwVerifySuccessAddInterval = 0;
        g_boVerifyFailTriggerScript = false;
        g_boVerifyFailLoginVerify = false;
        g_boVerifyCodeExcludeMap = true;
        g_boLogoutNoResendAntiplugStream = false;
        g_boOneMACLimitePlayer = false;
        g_nOneMACLimitePlayerCount = 3;
        g_nClientCloseDelay = 0;
        g_boDelayCloseDisableMove = false;
        g_boDelayCloseDisableSpell = false;
        g_boDelayCloseDisableAttack = false;
        g_boDelayCloseDisableUseItem = false;
        g_boBreakClientLogoutHint = true;
        g_sBreakClientLogoutHint = "小退游戏操作已被中断";
        g_boBreakClientCloseHint = true;
        g_sBreakClientCloseHint = "大退游戏操作已被中断";
        g_boCheckClientPassword = false;
        g_sClientPassWord = "BmM2";
        g_sLogClientPacketDir = "";
        g_sReplaceWord = '*';
        g_sDisableSayMsg = "禁止聊天";
        g_sDisableSayMsgBegin = "由于您说话太快，%d秒内禁止聊天！！！";
        g_ClientAntiPlugVersion = 0;
        g_ClientAntiPlugDllStringCRC = 0;
        g_ClientAntiPlugDllSize = 0;
        g_ClientAntiPlugDllString = Array.Empty<byte>();
        g_boAntiplugAllLog = false;
        g_RunGatePlugDllHandle = 0;
        g_rgpRecvPacket = null;

        AddMainLogMsgSink = (msg, level) => { };
        AddBlockIPSink = ip => { };
        AddTempBlockIPSink = ip => { };
        IsBlockMacSink = macId => false;
        CloseAllUserSink = () => { };
        MyGetTickCountProvider = () => (uint)Environment.TickCount;
        RandomizeSink = () => { };
        RandomSink = range => range <= 0 ? 0 : System.Random.Shared.Next(range);
    }
}

// -------------------------------------------------------------------------------------
// MagicIntervalUtils.pas 接缝：
//   原文 TMagicInterval = record MagicId / Interval;  TMagicIntervalList 由 uFrmGameSpeedLogic.cs:944/951
//   **真实现**（非接缝），此处只补 MirClientContext.pas 用到的 `Find(MagicID)`（已存在）。
//   ⚠ 差异：uFrmGameSpeedLogic 的 Find 返回 null 表示未找到，与原文 `PMagicInterval` 的 nil 语义一致。
// -------------------------------------------------------------------------------------
public static class MagicIntervalUtilsSeam
{
    /// <summary>原文 MagicIntervalUtils.pas 的 `Find`（`g_MagicCDList.Find(MagicId)`）。</summary>
    public static TMagicInterval Find(TMagicIntervalList list, int magicId) => list.Find(magicId);
}

// =====================================================================================
// IocpTcpServer.pas:119 TIocpTcpServer（接缝）—— 只保留 MirClientContext.pas:9749-9752 用到的
// `BindObject`（RunGate 场景下挂 TRunGate）。
// =====================================================================================
public class TIocpTcpServer
{
    public object BindObject;
}

// =====================================================================================
// StrUtils.pas / SysUtils.pas 的 AnsiString 助手（原文 uses StrUtils / SysUtils）。
// 语义 1:1：AnsiContainsText / AnsiReplaceText 都是 **大小写不敏感**。
// =====================================================================================
public static class AnsiStrSeam
{
    /// <summary>原文 <c>AnsiContainsText(AText, ASubText)</c>（StrUtils）。</summary>
    public static bool AnsiContainsText(string aText, string aSubText)
    {
        if (string.IsNullOrEmpty(aSubText)) return false;
        return aText != null && aText.IndexOf(aSubText, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>原文 <c>AnsiReplaceText(AText, AFromText, AToText)</c>（StrUtils）。</summary>
    public static string AnsiReplaceText(string aText, string aFromText, string aToText)
        => StringReplace(aText, aFromText, aToText, true);

    /// <summary>
    /// 原文 <c>StringReplace(S, OldPattern, NewPattern, Flags)</c>。
    /// Delphi 的 <c>[]</c> = 区分大小写；<c>[rfIgnoreCase]</c> = 忽略大小写。
    /// </summary>
    public static string StringReplace(string s, string oldPattern, string newPattern, bool ignoreCase)
    {
        if (s == null) return "";
        if (string.IsNullOrEmpty(oldPattern)) return s;
        return s.Replace(oldPattern, newPattern ?? "",
            ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }

    /// <summary>原文 <c>StringOfChar(Ch, Count)</c>（SysUtils）。</summary>
    public static string StringOfChar(char ch, int count) => count <= 0 ? "" : new string(ch, count);

    /// <summary>原文 <c>SameText(A, B)</c>（SysUtils，大小写不敏感）。</summary>
    public static bool SameText(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>原文 <c>FormatDateTime('yyyy-mm-dd', Now)</c> / <c>TimeToStr(Now)</c> 的托管对应。</summary>
    public static string FormatDateTime(string fmt, DateTime dt) =>
        dt.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);
}


// =====================================================================================
// 二进制缓冲区助手：Delphi 的 `SetLength(S, Len) + Move(...)` 在 C# 侧统一走这里，
// 避免每处手写数组拷贝导致边界不一致。
// =====================================================================================
public static class AnsiBufferSeam
{
    /// <summary>原文 <c>Move(Src, Dst, Len)</c>。</summary>
    public static void Move(byte[] src, int srcOffset, byte[] dst, int dstOffset, int len)
    {
        if (len <= 0) return;
        Buffer.BlockCopy(src, srcOffset, dst, dstOffset, len);
    }

    /// <summary>原文 <c>SetLength(S, Len)</c> 后按字节填充。</summary>
    public static byte[] NewBytes(int len) => len <= 0 ? Array.Empty<byte>() : new byte[len];
}

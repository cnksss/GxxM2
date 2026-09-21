// ============================================================================
// FileSearchPool.pas（558 行）→ Pool/FileSearchPool.cs
// 单元：LogDataServer/FileSearchPool.pas（全仓仅此一份，实测 sha256 唯一）
//
// 命名空间同 ThreadPool.cs：接缝 `GXX.LogDataServer/LogDataShare.cs` 已声明最小面的
// `TSearchTask` / `TSearchManagerSeam`，本单元是真实现 ⇒ 落在子命名空间
// `GXX.LogDataServer.Pool`（D-P10-01）。`TLogData` / `TLogActorType` / `TThreadList`
// 按「能复用就复用」直接复用 `GXX.LogDataServer` 里的既有声明（不造第三份）。
// ============================================================================

using System;
using System.IO;
using GXX.Core;

namespace GXX.LogDataServer.Pool;

/// <summary>接缝：Classes.pas `EStreamError`（`SMemoryStreamError` = 'Stream error'）。</summary>
public class EStreamError : Exception
{
    public EStreamError(string message) : base(message) { }
}

/// <summary>
/// FileSearchPool.pas:10-22 `TCustomMemoryStreamEx`（原文继承 `TStream`）。
/// 托管侧不继承 <see cref="Stream"/>（其契约与本类相反：短读返回 0 而不抛），
/// 改为独立类并保留原文全部成员语义（D-P10-07）。
/// </summary>
public class TCustomMemoryStreamEx
{
    // ---- FileSearchPool.pas:11-13 ----
    private byte[]? FMemory;
    private int FSize;
    private int FPosition;

    /// <summary>FileSearchPool.pas:15 `procedure SetPointer(Ptr: Pointer; Size: Longint);`</summary>
    protected void SetPointer(byte[]? Ptr, int Size)
    {
        FMemory = Ptr;
        FSize = Size;
    }

    /// <summary>
    /// 供同单元派生类写原文 private 的 `FSize`
    /// （原文 `FSize`/`FPosition` 虽是 private，但 FileSearchPool.pas 是**单文件单单元**，
    /// `TMemoryStreamEx` 可直接读写；托管侧以 protected 访问器复刻同一可见性）。
    /// </summary>
    protected void SetSizeInternal(int Size) => FSize = Size;

    /// <summary>FileSearchPool.pas:21 `property Memory: Pointer read FMemory;`</summary>
    public byte[]? Memory => FMemory;

    /// <summary>TStream.Size（原文由 SetPointer/SetSize 维护）。</summary>
    public int Size => FSize;

    /// <summary>TStream.Position。</summary>
    public int Position
    {
        get => FPosition;
        set => FPosition = value;
    }

    // ---- TStream.Seek 的 Origin 常量（Classes.pas） ----
    public const ushort soFromBeginning = 0;
    public const ushort soFromCurrent = 1;
    public const ushort soFromEnd = 2;

    /// <summary>
    /// FileSearchPool.pas:114-128 `function Read(var Buffer; Count: Longint): Longint;`
    /// 原文如此：`(FPosition >= 0) and (Count >= 0)` 守卫；短读返回 0 而**不抛**。
    /// </summary>
    public int Read(byte[] Buffer, int Offset, int Count)
    {
        if ((FPosition >= 0) && (Count >= 0))
        {
            int Result = FSize - FPosition;
            if (Result > 0)
            {
                if (Result > Count) Result = Count;
                Array.Copy(FMemory!, FPosition, Buffer, Offset, Result);
                FPosition += Result;
                return Result;      // 原文 Exit
            }
        }
        return 0;
    }

    /// <summary>原文 `Read(var Buffer; Count)`（Buffer 偏移 0）。</summary>
    public int Read(byte[] Buffer, int Count) => Read(Buffer, 0, Count);

    /// <summary>
    /// FileSearchPool.pas:130-138 `function Seek(Offset: Longint; Origin: Word): Longint;`
    /// 原文如此：`case` 无 `else` ⇒ 未知 Origin 时 FPosition **不变**。
    /// </summary>
    public int Seek(int Offset, ushort Origin)
    {
        switch (Origin)
        {
            case soFromBeginning:
                FPosition = Offset;
                break;
            case soFromCurrent:
                FPosition += Offset;
                break;
            case soFromEnd:
                FPosition = FSize + Offset;
                break;
        }
        return FPosition;
    }

    /// <summary>FileSearchPool.pas:140-143 `procedure SaveToStream(Stream: TStream);`</summary>
    public void SaveToStream(Stream Stream)
    {
        if (FSize != 0) Stream.Write(FMemory!, 0, FSize);
    }

    /// <summary>FileSearchPool.pas:145-155 `procedure SaveToFile(const FileName: string);`</summary>
    public void SaveToFile(string FileName)
    {
        // 原文 Stream := TFileStream.Create(FileName, fmCreate);
        Stream Stream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
        try
        {
            SaveToStream(Stream);
        }
        finally
        {
            Stream.Dispose();
        }
    }
}

/// <summary>
/// FileSearchPool.pas:24-38 `TMemoryStreamEx`（原文 = Delphi Classes.TMemoryStream 的私有副本）。
/// </summary>
public class TMemoryStreamEx : TCustomMemoryStreamEx
{
    /// <summary>FileSearchPool.pas:160-161 `MemoryDelta = $2000; { Must be a power of 2 }`</summary>
    private const int MemoryDelta = 0x2000;

    private int FCapacity;

    /// <summary>FileSearchPool.pas:30 `property Capacity: Longint read FCapacity write SetCapacity;`</summary>
    public int Capacity
    {
        get => FCapacity;
        set => SetCapacity(value);
    }

    /// <summary>FileSearchPool.pas:163-167 `destructor TMemoryStreamEx.Destroy;`</summary>
    public virtual void Dispose()
    {
        Clear();
    }

    /// <summary>FileSearchPool.pas:169-174 `procedure Clear;`</summary>
    public void Clear()
    {
        SetCapacity(0);
        SetSizeInternal(0);     // 原文 FSize := 0
        Position = 0;           // 原文 FPosition := 0
    }

    /// <summary>FileSearchPool.pas:176-184 `procedure LoadFromStream(Stream: TStream);`</summary>
    public void LoadFromStream(Stream Stream)
    {
        Stream.Position = 0;                        // 原文 Stream.Position := 0;
        int Count = (int)Stream.Length;             // 原文 Count := Stream.Size;
        SetSize(Count);
        if (Count != 0) ReadBufferExactly(Stream, Memory!, Count);
    }

    /// <summary>FileSearchPool.pas:186-196 `procedure LoadFromFile(const FileName: string);`</summary>
    public void LoadFromFile(string FileName)
    {
        // 原文 TFileStream.Create(FileName, fmOpenRead or fmShareDenyRead)
        //   fmShareDenyRead = 只拒绝"其他读者" ⇒ 托管 FileShare.Write
        Stream Stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Write);
        try
        {
            LoadFromStream(Stream);
        }
        finally
        {
            Stream.Dispose();
        }
    }

    private static void ReadBufferExactly(Stream Stream, byte[] Buffer, int Count)
    {
        int done = 0;
        while (done < Count)
        {
            int n = Stream.Read(Buffer, done, Count - done);
            if (n <= 0) throw new EndOfStreamException();
            done += n;
        }
    }

    /// <summary>FileSearchPool.pas:198-202 `procedure SetCapacity(NewCapacity: Longint);`</summary>
    private void SetCapacity(int NewCapacity)
    {
        SetPointer(Realloc(ref NewCapacity), Size);
        FCapacity = NewCapacity;
    }

    /// <summary>FileSearchPool.pas:204-212 `procedure SetSize(NewSize: Longint);`</summary>
    public virtual void SetSize(int NewSize)
    {
        int OldPosition = Position;
        SetCapacity(NewSize);
        SetSizeInternal(NewSize);
        if (OldPosition > NewSize) Seek(0, soFromEnd);
    }

    /// <summary>
    /// FileSearchPool.pas:214-245 `function Realloc(var NewCapacity: Longint): Pointer; virtual;`
    /// <para>
    /// 原文用 `GlobalAllocPtr/GlobalReallocPtr`（保留内容）；托管侧用 `byte[]` 重分配 + 拷贝，
    /// **容量取整规则逐字保留**（`(NewCapacity + 8191) and not 8191`，且 `NewCapacity &lt;&gt; FSize` 才取整）。
    /// </para>
    /// </summary>
    protected virtual byte[]? Realloc(ref int NewCapacity)
    {
        // 原文如此：只有 NewCapacity > 0 且 <> FSize 才向上取整到 MemoryDelta 倍数
        if ((NewCapacity > 0) && (NewCapacity != Size))
            NewCapacity = (NewCapacity + (MemoryDelta - 1)) & ~(MemoryDelta - 1);

        byte[]? Result = Memory;
        if (NewCapacity != FCapacity)
        {
            if (NewCapacity == 0)
            {
                Result = null;      // 原文 GlobalFreePtr(Memory)
            }
            else
            {
                byte[] block = new byte[NewCapacity];
                if (Memory != null)
                {
                    // GlobalReallocPtr / ReallocMem：保留旧内容（长度取二者较小）
                    int keep = Math.Min(Memory.Length, NewCapacity);
                    Array.Copy(Memory, block, keep);
                }
                Result = block;
                // 原文 `if Result = nil then raise EStreamError.CreateRes(@SMemoryStreamError);`
                if (Result == null) throw new EStreamError("Stream error");
            }
        }
        return Result;
    }

    /// <summary>FileSearchPool.pas:247-269 `function Write(const Buffer; Count: Longint): Longint;`</summary>
    public int Write(byte[] Buffer, int Offset, int Count)
    {
        if ((Position >= 0) && (Count >= 0))
        {
            int Pos = Position + Count;
            if (Pos > 0)
            {
                if (Pos > Size)
                {
                    if (Pos > FCapacity)
                        SetCapacity(Pos);
                    SetSizeInternal(Pos);
                }
                Array.Copy(Buffer, Offset, Memory!, Position, Count);
                Position = Pos;
                return Count;       // 原文 Exit
            }
        }
        return 0;
    }

    /// <summary>原文 `Write(const Buffer; Count)`（Buffer 偏移 0）。</summary>
    public int Write(byte[] Buffer, int Count) => Write(Buffer, 0, Count);
}

/// <summary>
/// FileSearchPool.pas:40-56 `TSearchTask`。
/// </summary>
public class TSearchTask : TPoolTask
{
    // ---- FileSearchPool.pas:41-44 ----
    private int FTaskID;
    private string FFileName = "";
    private System.Windows.Forms.ToolStripStatusLabel? FPanel;

    /// <summary>FileSearchPool.pas:273-276 `constructor TSearchTask.Create;`（原文空体，未调 inherited）。</summary>
    public TSearchTask()
    {
    }

    /// <summary>FileSearchPool.pas:278-282 `destructor TSearchTask.Destroy;`（原文空体 + inherited）。</summary>
    public override void Dispose()
    {
    }

    /// <summary>
    /// FileSearchPool.pas:284-293 `procedure AssignTo(Dest: TPoolTask); override;`
    /// ★ 原文如此：**只搬 FPanel**（TaskID / FileName 不搬），且 `Dest` 不是 TSearchTask 时才 `inherited`。
    /// </summary>
    protected override void AssignTo(TPoolTask Dest)
    {
        if (Dest is TSearchTask d)
        {
            d.FPanel = FPanel;
        }
        else
        {
            base.AssignTo(Dest);
        }
    }

    /// <summary>
    /// FileSearchPool.pas:295-300 `function CompareTask(Task: TPoolTask): Boolean; override;`
    /// 原文如此：函数体只有 `Result := False`（局部变量声明被注释掉，比较逻辑从未写）。
    /// </summary>
    public override bool CompareTask(TPoolTask Task) => false;

    /// <summary>FileSearchPool.pas:51 `property TaskID: Integer read FTaskID write FTaskID;`</summary>
    public int TaskID
    {
        get => FTaskID;
        set => FTaskID = value;
    }

    /// <summary>
    /// FileSearchPool.pas:52 `property ShowPanel: TStatusPanel read FPanel write FPanel;`
    /// 托管偏离：ComCtrls `TStatusPanel` → `ToolStripStatusLabel`（沿用既有接缝
    /// `GXX.LogDataServer.LogDataShare` 的同款选择，D-P10-08）。
    /// </summary>
    public System.Windows.Forms.ToolStripStatusLabel? ShowPanel
    {
        get => FPanel;
        set => FPanel = value;
    }

    /// <summary>FileSearchPool.pas:53 `property FileName: string read FFileName write FFileName;`</summary>
    public string FileName
    {
        get => FFileName;
        set => FFileName = value;
    }
}

/// <summary>
/// FileSearchPool.pas:58-73 `TSearchThread`。
/// </summary>
public class TSearchThread : TPoolThread
{
    // ---- FileSearchPool.pas:59-61 ----
    private string FFileName = "";
    private readonly TMemoryStreamEx FMemoryStream;

    private readonly byte[] _scratch = new byte[8];

    /// <summary>FileSearchPool.pas:304-309 `constructor TSearchThread.Create(AOwner; CreateSuspended);`</summary>
    public TSearchThread(TPoolManager AOwner, bool CreateSuspended)
        : base(AOwner, CreateSuspended)
    {
        FMemoryStream = new TMemoryStreamEx();
    }

    /// <summary>FileSearchPool.pas:311-315 `destructor TSearchThread.Destroy;`</summary>
    public override void Dispose()
    {
        FMemoryStream.Dispose();
        base.Dispose();
    }

    /// <summary>FileSearchPool.pas:317-320 `procedure DoTaskComplete;`（原文 private）</summary>
    private void DoTaskComplete()
    {
        // 原文 (Owner as TSearchManager).DoTaskComplete(FContextTask)
        ((TSearchManager)Owner).DoTaskComplete(FContextTask!);
    }

    /// <summary>
    /// FileSearchPool.pas:322-354 `procedure DoExecuteLoop; override;`
    /// </summary>
    protected override void DoExecuteLoop()
    {
        if (FContextTask != null)
        {
            FContextTask.Dispose();     // 原文 FreeAndNil(FContextTask)
            FContextTask = null;
        }

        var Tasks = Owner.LockTaskList();
        try
        {
            // 原文 `if Tasks.Count = 0 then Exit;` —— Exit 在 try/finally 内，finally 仍会 UnlockTaskList
            if (Tasks.Count == 0) return;
            FContextTask = Tasks[0];
            Tasks.RemoveAt(0);          // 原文 Tasks.Delete(0)
        }
        finally
        {
            Owner.UnlockTaskList();
        }

        if (FContextTask == null) return;

        if (FContextTask is TSearchTask)
        {
            TSearchTask Task = (TSearchTask)FContextTask;

            FFileName = Task.FileName;
            Synchronize(RefreshLabel);

            DoSearch(Task);

            Synchronize(DoTaskComplete);

            TriggerEvent();
        }
    }

    /// <summary>FileSearchPool.pas:356-362 `procedure RefreshLabel;`（原文 private）</summary>
    private void RefreshLabel()
    {
        // 原文如此：`Task := FContextTask as TSearchTask; Task.FPanel.Text := FFileName;`
        //   FPanel 为 nil 时原文 AV ⇒ 托管对应 NullReferenceException（不额外加守卫）
        TSearchTask Task = (TSearchTask)FContextTask!;
        Task.ShowPanel!.Text = FFileName;
    }

    // ---- 逐字段读取助手（原文 `FMemoryStream.Read(X, SizeOf(X)) <> SizeOf(X)` 的等价物） ----

    private bool ReadBytes(int Count, out byte[] Buf)
    {
        Buf = new byte[Count];
        return FMemoryStream.Read(Buf, 0, Count) == Count;
    }

    private bool ReadInt32(out int Value)
    {
        Value = 0;
        if (FMemoryStream.Read(_scratch, 0, 4) != 4) return false;
        Value = BitConverter.ToInt32(_scratch, 0);
        return true;
    }

    private bool ReadUInt32(out uint Value)
    {
        Value = 0;
        if (FMemoryStream.Read(_scratch, 0, 4) != 4) return false;
        Value = BitConverter.ToUInt32(_scratch, 0);
        return true;
    }

    private bool ReadByte(out byte Value)
    {
        Value = 0;
        if (FMemoryStream.Read(_scratch, 0, 1) != 1) return false;
        Value = _scratch[0];
        return true;
    }

    /// <summary>原文 `Read(Dt, SizeOf(Dt))`，TDateTime = Double（8 字节）。</summary>
    private bool ReadDateTime(out DateTime Value)
    {
        Value = default;
        if (FMemoryStream.Read(_scratch, 0, 8) != 8) return false;
        Value = DateTime.FromOADate(BitConverter.ToDouble(_scratch, 0));
        return true;
    }

    /// <summary>原文 `SetLength(s, nLen); Read(s[1], nLen)`（AnsiString，GBK 字节）。</summary>
    private bool ReadAnsiString(int nLen, out string Value)
    {
        Value = "";
        if (nLen <= 0) return true;
        if (!ReadBytes(nLen, out byte[] buf)) return false;
        Value = EncodingInit.GBK.GetString(buf);
        return true;
    }

    /// <summary>
    /// FileSearchPool.pas:391-523 `procedure DoSearch(Task: TSearchTask);`
    /// <para>
    /// ★ 原文如此（缺陷，照抄）：① 整个函数体包在 `try ... except end` 里 ⇒ **静默吞掉一切异常**
    /// （含 `LoadFromFile` 打不开文件）；② 读出的 `nServerNumber` / `nServerIndex`
    /// **从未写进 `LogData`**（局部变量读了就丢）。
    /// </para>
    /// </summary>
    private void DoSearch(TSearchTask Task)
    {
        try
        {
            FMemoryStream.LoadFromFile(Task.FileName);
            int nIndex = 0;

            FMemoryStream.Seek(0, TCustomMemoryStreamEx.soFromBeginning);
            while (FMemoryStream.Position < FMemoryStream.Size)
            {
                if (!ReadInt32(out int nServerNumber)) break;

                if (!ReadInt32(out int nServerIndex)) break;

                if (!ReadUInt32(out uint nAction)) break;

                string sMapName = "";
                if (!ReadInt32(out int nLen)) break;
                if (nLen > 0)
                {
                    if (!ReadAnsiString(nLen, out sMapName)) break;
                }

                if (!ReadInt32(out int nX)) break;

                if (!ReadInt32(out int nY)) break;

                string sObjectName = "";
                if (!ReadInt32(out nLen)) break;
                if (nLen > 0)
                {
                    if (!ReadAnsiString(nLen, out sObjectName)) break;
                }

                if (!ReadByte(out byte btActorType)) break;
                if ((btActorType < (int)TLogActorType.latNone) || (btActorType > (int)TLogActorType.latMonster))
                {
                    btActorType = (byte)TLogActorType.latNone;
                }

                string sItemName = "";
                if (!ReadInt32(out nLen)) break;
                if (nLen > 0)
                {
                    if (!ReadAnsiString(nLen, out sItemName)) break;
                }

                if (!ReadInt32(out int nItemIndex)) break;

                string sActObjectName = "";
                if (!ReadInt32(out nLen)) break;
                if (nLen > 0)
                {
                    if (!ReadAnsiString(nLen, out sActObjectName)) break;
                }

                if (!ReadInt32(out int nData1)) break;
                if (!ReadInt32(out int nData2)) break;

                string sLogDesc = "";
                if (!ReadInt32(out nLen)) break;
                if (nLen > 0)
                {
                    if (!ReadAnsiString(nLen, out sLogDesc)) break;
                }

                if (!ReadDateTime(out DateTime Dt)) break;

                nIndex++;

                if (Task.Canceled) break;

                // 原文把字段逐个写进**局部记录** LogData；随后的 `New(PLogData); PLogData^ := LogData;`
                // 是"堆上复制一份快照"。托管侧以"每轮新建一个 TLogData 实例"表达同一语义
                // （原记录里 nServerNumber / nServerIndex 从未被赋值 ⇒ 无跨轮残留语义，D-P10-09）。
                var LogData = new TLogData();
                LogData.nAct = nAction;
                LogData.sMapName = sMapName;
                LogData.nX = nX;
                LogData.nY = nY;
                LogData.sObjectName = sObjectName;
                LogData.ObjectType = (TLogActorType)btActorType;
                LogData.sItemName = sItemName;
                LogData.nItemIndex = nItemIndex;
                LogData.sActObjectName = sActObjectName;
                LogData.nData1 = nData1;
                LogData.nData2 = nData2;
                LogData.LogDesc = sLogDesc;
                LogData.Date = Dt;
                // ★ 原文如此：nServerNumber / nServerIndex 读出来后**没有**赋给 LogData

                TSearchManager SM = (TSearchManager)Owner;

                if (SM.GetActionChecked(nAction) &&
                    FileSearchPool.GetSearch(SM.SearchWhere, SM.SearchObjName, SM.SearchActObjName,
                        SM.SearchItemName, SM.SearchActObjType, SM.SearchItemID, LogData))
                {
                    LogData.nIndx = Task.TaskID * 1000000 + nIndex;

                    var List = SM.SearchDataList!.LockList();
                    try
                    {
                        List.Add(LogData);
                    }
                    finally
                    {
                        SM.SearchDataList!.UnlockList();
                    }
                }
            }
        }
        catch
        {
            // 原文如此：`except end;` —— 吞掉全部异常（无日志、无重抛）
        }
    }

    /// <summary>
    /// TThread.Synchronize 接缝（D-P10-10）。
    /// 原文是"阻塞式投递到主线程"；托管侧默认**就地调用**（无头/测试环境没有 VCL 消息泵），
    /// 宿主可通过 <see cref="SynchronizeHandler"/> 注入真正的 UI 线程投递。
    /// </summary>
    public static Action<Action>? SynchronizeHandler;

    /// <summary>原文 `Synchronize(Method)`（TThread.Synchronize）。</summary>
    protected void Synchronize(Action Method)
    {
        Action<Action>? handler = SynchronizeHandler;
        if (handler != null) handler(Method);
        else Method();
    }
}

/// <summary>
/// FileSearchPool.pas:75 `TTaskCompleteEvent = procedure(Task: TPoolTask) of object;`
/// </summary>
public delegate void TTaskCompleteEvent(TPoolTask Task);

/// <summary>
/// FileSearchPool.pas:76-98 `TSearchManager`。
/// </summary>
public class TSearchManager : TPoolManager
{
    // ---- FileSearchPool.pas:78 ----
    private TTaskCompleteEvent? FOnTaskComplete;

    /// <summary>FileSearchPool.pas:80 `function GetActionChecked(nAction: LongWord): Boolean;`（原文 private；同单元被 TSearchThread 访问 ⇒ internal）</summary>
    internal bool GetActionChecked(uint nAction)
    {
        // 日志类型暂时只用了Word
        ushort W1 = LoWord(nAction);
        byte B1 = LoByte(W1);
        byte B2 = HiByte(W1);

        return SearchActions[B1] || SearchActions[B2];
    }

    private static ushort LoWord(uint v) => (ushort)(v & 0xFFFF);
    private static byte LoByte(ushort v) => (byte)(v & 0xFF);
    private static byte HiByte(ushort v) => (byte)(v >> 8);

    /// <summary>FileSearchPool.pas:552-555 `function GetPoolThreadClass: TPoolThreadClass; override;`</summary>
    protected override Type GetPoolThreadClass() => typeof(TSearchThread);

    /// <summary>FileSearchPool.pas:533-537 `procedure DoTaskComplete(Task: TPoolTask); virtual;`（原文 protected；同单元被 TSearchThread 访问 ⇒ internal）</summary>
    internal virtual void DoTaskComplete(TPoolTask Task)
    {
        if (FOnTaskComplete != null)
            FOnTaskComplete(Task);
    }

    /// <summary>继承 `TPoolManager.Create(ThreadCount)`（原文 TSearchManager 无自定义构造）。</summary>
    public TSearchManager(int ThreadCount) : base(ThreadCount)
    {
    }

    // ---- FileSearchPool.pas:85-94 public 字段 ----

    /// <summary>FileSearchPool.pas:85 `SearchActions: array[Byte] of Boolean;`</summary>
    public bool[] SearchActions = new bool[256];

    /// <summary>FileSearchPool.pas:87 `SearchWhere: Integer;`</summary>
    public int SearchWhere;

    /// <summary>FileSearchPool.pas:88 `SearchObjName: string;`</summary>
    public string SearchObjName = "";

    /// <summary>FileSearchPool.pas:89 `SearchActObjName: string;`</summary>
    public string SearchActObjName = "";

    /// <summary>FileSearchPool.pas:90 `SearchActObjType: Integer;`</summary>
    public int SearchActObjType;

    /// <summary>FileSearchPool.pas:91 `SearchItemName: string;`</summary>
    public string SearchItemName = "";

    /// <summary>FileSearchPool.pas:92 `SearchItemID: Integer;`</summary>
    public int SearchItemID;

    /// <summary>FileSearchPool.pas:94 `SearchDataList: TThreadList;`（原文由 LogManage.pas 赋值，本单元不初始化）</summary>
    public TThreadList? SearchDataList;

    /// <summary>FileSearchPool.pas:527-531 `procedure AddTask(Task: TPoolTask); override;`</summary>
    public override void AddTask(TPoolTask Task)
    {
        if (Task is not TSearchTask) return;    // 原文 `if not (Task is TSearchTask) then Exit;`
        base.AddTask(Task);                     // 原文 inherited
    }

    /// <summary>FileSearchPool.pas:97 `property OnTaskComplete: TTaskCompleteEvent read FOnTaskComplete write FOnTaskComplete;`</summary>
    public TTaskCompleteEvent? OnTaskComplete
    {
        get => FOnTaskComplete;
        set => FOnTaskComplete = value;
    }
}

/// <summary>
/// 单元级自由函数（FileSearchPool.pas:364-389 `GetSearch`）。
/// 命名沿用同项目既有约定（`LogDataShare` 静态类对应 LDShare.pas）。
/// </summary>
public static class FileSearchPool
{
    /// <summary>
    /// FileSearchPool.pas:364 `function GetSearch(nWhere: Integer; sObjName, sActObjName,
    /// sItemName: string; nActObjType: Integer; nItemID: Integer; LogData: pTLogData): Boolean;`
    /// <para>
    /// ★ 原文如此：`if nWhere &lt;= 0 then Exit;` 时 `Result` 已被置 **True** ⇒ "不筛选"= 全部命中；
    /// 每个 `if` 前都有 `if not Result then Exit;` ⇒ 逐位"与"短路。
    /// </para>
    /// </summary>
    public static bool GetSearch(int nWhere, string sObjName, string sActObjName, string sItemName,
        int nActObjType, int nItemID, TLogData LogData)
    {
        bool Result = true;
        if (nWhere <= 0) return Result;

        if ((nWhere & 1) == 1)
            Result = AnsiContainsText(LogData.sObjectName, sObjName);
        if (!Result) return Result;

        if ((nWhere & 2) == 2)
            Result = (int)LogData.ObjectType == nActObjType;
        if (!Result) return Result;

        if ((nWhere & 4) == 4)
            Result = AnsiContainsText(LogData.sActObjectName, sActObjName);
        if (!Result) return Result;

        if ((nWhere & 8) == 8)
            Result = AnsiContainsText(LogData.sItemName, sItemName);
        if (!Result) return Result;

        if ((nWhere & 16) == 16)
            Result = LogData.nItemIndex == nItemID;

        return Result;
    }

    /// <summary>
    /// StrUtils.AnsiContainsText：`Pos(AnsiLowerCase(SubText), AnsiLowerCase(Text)) &gt; 0`。
    /// 空 SubText ⇒ Pos('') = 1 &gt; 0 ⇒ **True**（原文如此）。
    /// </summary>
    public static bool AnsiContainsText(string Text, string SubText)
        => Text.IndexOf(SubText ?? "", StringComparison.OrdinalIgnoreCase) >= 0;
}

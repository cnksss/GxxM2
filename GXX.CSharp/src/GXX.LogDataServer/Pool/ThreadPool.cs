// ============================================================================
// ThreadPool.pas（445 行）→ Pool/ThreadPool.cs
// 单元：LogDataServer/ThreadPool.pas（唯一一份真移植；LoginGate/SelGate 的
//       ThreadPool.pas 是 Windows APC 线程池封装，已登记为「被 GatewayKit 取代、不移植」）
//
// 命名空间说明（D-P10-01）：既有接缝 `GXX.LogDataServer/LogDataShare.cs` 里已经声明了
// 一个"最小面"的 `TPoolTask` / `TSearchTask`（其注释明写"待 FileSearchPool/ThreadPool
// 移植后接入"）。本单元是**真实现**，若放在同一命名空间会与接缝**同命名空间重名（CS0101）**；
// 而 `LogDataShare.cs` 在本车道分区之外（禁止修改）⇒ 本单元落在子命名空间
// `GXX.LogDataServer.Pool`。接缝退役（把 `TSearchManagerHost.g_SearchManager` 换成
// `TSearchManager`）列为跨区事项 B-P10-01，本区不擅改。
//
// 1:1 要点：
//   · 逐方法对照，保留原文早退顺序/边界/字段名（F 前缀成员按原文名保留）。
//   · 虚分派保留：`DoExecuteLoop` / `GetTerminated` / `AssignTo` / `CompareTask` /
//     `GetPoolThreadClass` / `DoTaskFinished` / `AddTask` / `FindTask` 全部为 virtual/override。
//   · 原文缺陷照抄并加 `// 原文如此`（见 §原文缺陷清单）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace GXX.LogDataServer.Pool;

/// <summary>
/// 接缝：SysUtils.EConvertError（RTLConsts.SAssignError）。
/// 托管侧 `GXX.Core.Rtl` 内**无**此异常类型（实测 0 命中），故本单元按原文所需最小面声明；
/// 归位到 `GXX.Core` 属跨区事项（B-P10-02）。
/// </summary>
public class EConvertError : Exception
{
    /// <summary>RTLConsts.SAssignError = 'Cannot assign a %s to a %s'。</summary>
    public const string SAssignError = "Cannot assign a %s to a %s";

    public EConvertError(string message) : base(message) { }

    /// <summary>EConvertError.CreateResFmt(@SAssignError, [SourceName, ClassName])。</summary>
    public static EConvertError CreateResFmt(string resFmt, params string[] args)
    {
        var sb = new System.Text.StringBuilder(resFmt.Length + 32);
        int a = 0;
        for (int i = 0; i < resFmt.Length; i++)
        {
            if (resFmt[i] == '%' && i + 1 < resFmt.Length && resFmt[i + 1] == 's' && a < args.Length)
            {
                sb.Append(args[a++]);
                i++;
            }
            else
            {
                sb.Append(resFmt[i]);
            }
        }
        return new EConvertError(sb.ToString());
    }
}

/// <summary>
/// 接缝：SysUtils.EAbstractError。
/// 原文 `TPoolThread.DoExecuteLoop` 是 `virtual; abstract`，且 `TPoolManager` 默认
/// `GetPoolThreadClass` 返回 **非抽象** 的 `TPoolThread` ⇒ Delphi 只在运行时调到时抛
/// `EAbstractError`。C# 的 `abstract` 成员要求类也是 `abstract`，而抽象类无法被
/// `Activator.CreateInstance` 实例化 ⇒ 无法 1:1 表达"可实例化但方法抽象"（D-P10-03）。
/// 处置：方法保持 `virtual`，方法体抛 `EAbstractError`，运行时行为与 Delphi 一致。
/// </summary>
public class EAbstractError : Exception
{
    public EAbstractError() : base("Abstract Error") { }
    public EAbstractError(string message) : base(message) { }
}

/// <summary>
/// ThreadPool.pas:74 `TTaskFinishedEvent = procedure(Manager: TPoolManager;
/// Thread: TPoolThread; Task: TPoolTask) of object;`
/// </summary>
public delegate void TTaskFinishedEvent(TPoolManager Manager, TPoolThread Thread, TPoolTask Task);

/// <summary>
/// ThreadPool.pas:23-37 `TPoolTask`。
/// </summary>
public class TPoolTask : IDisposable
{
    // ---- ThreadPool.pas:24-26 private 字段 ----
    private bool FCanceled;
    private bool FFinished;

    /// <summary>ThreadPool.pas:27 `procedure AssignError(Source: TPoolTask);`（原文 private）。</summary>
    private void AssignError(TPoolTask? Source)
    {
        // 原文如此：SourceName := Source.ClassName 或 'nil'（Source.ClassName 是类名字符串）
        string SourceName = Source != null ? Source.GetType().Name : "nil";
        throw EConvertError.CreateResFmt(EConvertError.SAssignError, SourceName, GetType().Name);
    }

    /// <summary>ThreadPool.pas:29 `procedure AssignTo(Dest: TPoolTask); virtual;`</summary>
    protected virtual void AssignTo(TPoolTask Dest)
    {
        Dest.AssignError(this);
    }

    /// <summary>ThreadPool.pas:31 `constructor Create; virtual;`（体：FCanceled := False）。</summary>
    public TPoolTask()
    {
        FCanceled = false;   // 原文如此；C# 默认即 false，逐字保留语义
    }

    /// <summary>ThreadPool.pas:32 `procedure Cancel;`</summary>
    public virtual void Cancel()
    {
        FCanceled = true;
    }

    /// <summary>ThreadPool.pas:33 `property Canceled: Boolean read FCanceled;`（只读）。</summary>
    public bool Canceled => FCanceled;

    /// <summary>ThreadPool.pas:34 `property Finished: Boolean read FFinished write FFinished;`</summary>
    public bool Finished
    {
        get => FFinished;
        set => FFinished = value;
    }

    /// <summary>ThreadPool.pas:35 `function CompareTask(Task: TPoolTask): Boolean; virtual; abstract;`</summary>
    public virtual bool CompareTask(TPoolTask Task) => throw new EAbstractError();

    /// <summary>
    /// ThreadPool.pas:36 `procedure Assign(Source: TPoolTask); virtual;`
    /// 原文：`if Source &lt;&gt; nil then Source.AssignTo(Self) else AssignError(nil);`
    /// </summary>
    public virtual void Assign(TPoolTask? Source)
    {
        if (Source != null) Source.AssignTo(this);
        else AssignError(null);
    }

    /// <summary>TObject.Free / TObject.Destroy（原文 TPoolTask 无显式析构 ⇒ 空体）。</summary>
    public virtual void Dispose()
    {
    }

    /// <summary>Delphi `Free` 的托管入口（等价 `FreeAndNil` 的前半）。</summary>
    public void Free() => Dispose();
}

/// <summary>
/// ThreadPool.pas:39-71 `TPoolThread`。
/// 原文基类是 `TThread`；托管侧以"自带 <see cref="Thread"/> 的包装类"表达，
/// 以保留 `Execute` / `Terminate` / `Suspended` / `Sleeping` 的可覆盖面（D-P10-04）。
/// </summary>
public class TPoolThread : IDisposable
{
    // ---- ThreadPool.pas:40-43 ----
    private readonly TPoolManager FOwner;
    private readonly AutoResetEvent FEvent;      // 原文 TEvent.Create(nil, False, False, '') → 自动重置、初始未置位
    private bool FInExecuteLoop;

    /// <summary>ThreadPool.pas:46 `FContextTask: TPoolTask;`（protected）。</summary>
    protected TPoolTask? FContextTask;

    // ---- 托管侧承载 TThread 的运行时 ----
    private readonly Thread _thread;
    private bool _started;
    private bool _suspended;
    private bool _terminated;
    private bool _finished;
    private bool _disposed;

    /// <summary>
    /// 线程体抛出且未被处理的异常（D-P10-12）。
    /// <para>
    /// 原文（Delphi 7 `TThread`）里线程体异常**不会终止进程**：`ThreadProc` 交顶层异常处理，
    /// 线程结束、进程存活。托管侧 .NET 的默认行为是"任一线程未处理异常 ⇒ 进程立即终止"
    /// —— 对服务端是不可接受的语义放大（测试宿主会直接崩，实测已复现）。
    /// 故此处捕获并存入 <see cref="ExecException"/>（不静默吞：可从外部读取与断言）。
    /// </para>
    /// </summary>
    public Exception? ExecException { get; private set; }

    /// <summary>
    /// ThreadPool.pas:153-166 `constructor TPoolThread.Create(AOwner: TPoolManager; CreateSuspended: Boolean);`
    /// </summary>
    public TPoolThread(TPoolManager AOwner, bool CreateSuspended)
    {
        FContextTask = null;
        FOwner = AOwner;                    // 原文 `FOwner := AOwner;`
        _suspended = CreateSuspended;
        _terminated = false;
        _finished = false;

        // 原文 `FEvent := TEvent.Create(nil, False, False, '')`：
        //   参数2 = False ⇒ 事件对象控制一次后立即重置（自动重置）
        //   参数3 = False ⇒ 对象建立后为暂停状态（未置位）
        FEvent = new AutoResetEvent(false);

        // 原文 `inherited Create(CreateSuspended)` + `FreeOnTerminate := False`
        // ⚠ 托管偏离：`IsBackground = true`。Delphi 线程默认前台；若测试宿主里
        //   线程未退出会阻止进程结束（xunit 挂死）。管理器的 Destroy 一定会 Terminate+Join，
        //   故 IsBackground 只影响"异常路径不挂死"。见 D-P10-04。
        _thread = new Thread(RunThread) { IsBackground = true, Name = GetType().Name };
    }

    // ---- TThread 兼容面 ----

    /// <summary>TThread.Suspended（原文 GetSleeping 读它）。</summary>
    public bool Suspended => _suspended;

    /// <summary>TThread.Resume（原文 TPoolManager.Create 末尾逐个 Resume）。</summary>
    public void Resume()
    {
        _suspended = false;
        if (!_started)
        {
            _started = true;
            _thread.Start();
        }
    }

    /// <summary>TThread.Terminated（原文 GetTerminated 读它）。</summary>
    public bool Terminated => _terminated;

    /// <summary>TThread.WaitFor（原文由 TThread.Destroy 内部调用）。</summary>
    public void WaitFor()
    {
        if (_started && !_finished)
        {
            try { _thread.Join(); }
            catch (ThreadStateException) { /* 未启动 */ }
        }
    }

    // ---- ThreadPool.pas:39-71 ----

    /// <summary>ThreadPool.pas:44 `function GetSleeping: Boolean;`（原文 private；同单元内被 TPoolManager 访问 ⇒ internal）。</summary>
    internal bool GetSleeping()
    {
        // 原文如此：`Result := (not FInExecuteLoop) and (not Suspended);`
        return (!FInExecuteLoop) && (!_suspended);
    }

    /// <summary>ThreadPool.pas:48 `procedure Execute; override;`</summary>
    protected virtual void Execute()
    {
        DoExecInitialize();
        try
        {
            while (!GetTerminated())
            {
                FEvent.WaitOne();            // 原文 FEvent.WaitFor(INFINITE)，返回值被忽略
                FInExecuteLoop = true;
                try
                {
                    DoExecuteLoop();
                }
                finally
                {
                    FInExecuteLoop = false;
                }
            }
        }
        finally
        {
            DoExecFinalize();
        }
    }

    private void RunThread()
    {
        try
        {
            Execute();
        }
        catch (Exception ex)
        {
            // 见 ExecException 说明（D-P10-12）：原文线程体异常不会终止进程
            ExecException = ex;
        }
        finally
        {
            _finished = true;
        }
    }

    /// <summary>ThreadPool.pas:49 `procedure TriggerEvent;`（原文 protected；同单元内被 TPoolManager/自身访问 ⇒ internal）。</summary>
    internal void TriggerEvent()
    {
        FEvent.Set();
    }

    /// <summary>ThreadPool.pas:51 `function GetTerminated: Boolean; virtual;`</summary>
    protected virtual bool GetTerminated()
    {
        return _terminated;   // 原文 `Result := Terminated;`
    }

    /// <summary>ThreadPool.pas:54 `procedure DoExecInitialize; virtual;`（原文空体）。</summary>
    protected virtual void DoExecInitialize()
    {
    }

    /// <summary>ThreadPool.pas:57 `procedure DoExecFinalize; virtual;`（原文空体）。</summary>
    protected virtual void DoExecFinalize()
    {
    }

    /// <summary>
    /// ThreadPool.pas:60 `procedure DoExecuteLoop; virtual; abstract;`
    /// 见 <see cref="EAbstractError"/> 与 D-P10-03。
    /// </summary>
    protected virtual void DoExecuteLoop()
    {
        throw new EAbstractError();
    }

    /// <summary>ThreadPool.pas:62 `procedure DoTaskFinished;`</summary>
    protected void DoTaskFinished()
    {
        FOwner.DoTaskFinished(this, FContextTask!);
    }

    /// <summary>ThreadPool.pas:64 `constructor Create(AOwner: TPoolManager; CreateSuspended: Boolean); virtual;`</summary>
    /// <remarks>已由上面的构造函数承载；Delphi 的"虚构造"由 <c>GetPoolThreadClass + Activator</c> 还原（D-P10-04）。</remarks>
    public static TPoolThread Create(TPoolManager AOwner, bool CreateSuspended)
        => new(AOwner, CreateSuspended);

    /// <summary>ThreadPool.pas:66 `procedure Terminate; reintroduce; virtual;`</summary>
    public virtual void Terminate()
    {
        if (FContextTask != null) FContextTask.Cancel();
        // inherited Terminate（TThread.Terminate 置 Terminated := True）
        _terminated = true;
        TriggerEvent();
    }

    /// <summary>ThreadPool.pas:68 `property Sleeping: Boolean read GetSleeping;`</summary>
    public bool Sleeping => GetSleeping();

    /// <summary>ThreadPool.pas:69 `property Owner: TPoolManager read FOwner;`</summary>
    public TPoolManager Owner => FOwner;

    /// <summary>ThreadPool.pas:70 `property ContextTask: TPoolTask read FContextTask;`</summary>
    public TPoolTask? ContextTask => FContextTask;

    /// <summary>
    /// ThreadPool.pas:168-174 `destructor TPoolThread.Destroy;`
    /// <para>
    /// ⚠ 顺序偏离（D-P10-05）：原文先 `FEvent.Free` 再 `inherited`（TThread.Destroy 内部
    /// Terminate+WaitFor）。托管侧若照抄会在线程仍等事件时释放 <see cref="AutoResetEvent"/>
    /// ⇒ `ObjectDisposedException`。故把"等待线程退出"提到释放事件之前，**净语义相同**
    /// （原文也必然等待线程退出，只是等待发生在 `inherited` 里）。
    /// </para>
    /// </summary>
    public virtual void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // 原文 inherited（TThread.Destroy）：
        //   if (FThreadID <> 0) and not FFinished then begin Terminate; if FCreateSuspended then Resume; WaitFor; end;
        if (_started && !_finished)
        {
            if (_suspended) Resume();
            Terminate();
            WaitFor();
        }

        FEvent.Dispose();
        if (FContextTask != null)
        {
            FContextTask.Dispose();       // 原文 FreeAndNil(FContextTask)
            FContextTask = null;
        }
    }

    /// <summary>Delphi `Free` 的托管入口。</summary>
    public void Free() => Dispose();
}

/// <summary>
/// ThreadPool.pas:75-114 `TPoolManager`。
/// </summary>
public class TPoolManager : IDisposable
{
    // ---- ThreadPool.pas:76-82 private 字段 ----
    private bool FIsWantDestroy;
    private readonly List<TPoolThread> FThreads;
    private readonly List<TPoolTask> FTasks;
    private readonly object FSectionTask = new();     // 原文 TCriticalSection（可重入）
    private TTaskFinishedEvent? FOnTaskFinished;

    // ---- ThreadPool.pas:84-88 accessors ----

    private TPoolThread? GetSleepingThread()
    {
        for (int I = 0; I <= FThreads.Count - 1; I++)
        {
            TPoolThread Thread = FThreads[I];
            if (Thread.GetSleeping())
            {
                return Thread;     // 原文 Result := Thread; Break;
            }
        }
        return null;
    }

    /// <summary>ThreadPool.pas:421-426 `function GetTaskCount: Integer;`（Enter/Leave 包住）。</summary>
    private int GetTaskCount()
    {
        FSectionTaskEnter();
        try { return FTasks.Count; }
        finally { FSectionTaskLeave(); }
    }

    private int GetThreadCount() => FThreads.Count;

    private TPoolTask GetTasks(int Index)
    {
        FSectionTaskEnter();
        try { return FTasks[Index]; }
        finally { FSectionTaskLeave(); }
    }

    private TPoolThread GetThreads(int Index) => FThreads[Index];

    private void FSectionTaskEnter() => Monitor.Enter(FSectionTask);
    private void FSectionTaskLeave() => Monitor.Exit(FSectionTask);

    // ---- ThreadPool.pas:89-91 protected ----

    /// <summary>
    /// ThreadPool.pas:90 `function GetPoolThreadClass: TPoolThreadClass; virtual;`
    /// 原文 `TPoolThreadClass = class of TPoolThread`（第 73 行）→ 托管以 <see cref="Type"/> 表达，
    /// 实例化走 <see cref="Activator"/>（D-P10-06）。
    /// </summary>
    protected virtual Type GetPoolThreadClass() => typeof(TPoolThread);

    /// <summary>ThreadPool.pas:91 `procedure DoTaskFinished(Thread: TPoolThread; Task: TPoolTask); virtual;`</summary>
    internal virtual void DoTaskFinished(TPoolThread Thread, TPoolTask Task)
    {
        if (FOnTaskFinished != null)
            FOnTaskFinished(this, Thread, Task);
    }

    // ---- ThreadPool.pas:93-94 ----

    /// <summary>
    /// ThreadPool.pas:234-255 `constructor TPoolManager.Create(ThreadCount: Integer);`
    /// </summary>
    public TPoolManager(int ThreadCount)
    {
        FIsWantDestroy = false;
        FThreads = new List<TPoolThread>();
        FTasks = new List<TPoolTask>();

        for (int I = 0; I <= ThreadCount - 1; I++)
        {
            // 原文 Thread := GetPoolThreadClass.Create(Self, True);
            TPoolThread Thread = (TPoolThread)Activator.CreateInstance(
                GetPoolThreadClass(), new object[] { this, true })!;
            FThreads.Add(Thread);
        }

        for (int I = 0; I <= FThreads.Count - 1; I++)
        {
            TPoolThread Thread = FThreads[I];
            Thread.Resume();
        }
    }

    /// <summary>
    /// ThreadPool.pas:257-289 `destructor TPoolManager.Destroy;`
    /// </summary>
    public virtual void Dispose()
    {
        FIsWantDestroy = true;
        CancelAndClearAllTask();

        for (int I = 0; I <= FThreads.Count - 1; I++)
        {
            TPoolThread Thread = FThreads[I];
            // 原文如此：忙等"线程空闲"（Sleep(1) + Application.ProcessMessages）
            while (!Thread.Sleeping)
            {
                ThreadSleep1();
                ApplicationProcessMessages();
            }
            Thread.Terminate();
        }

        for (int I = 0; I <= FThreads.Count - 1; I++)
        {
            TPoolThread Thread = FThreads[I];
            //Thread.WaitFor;      // 原文如此：源码里这行被注释掉（等待实际发生在 TThread.Destroy 内部）
            Thread.Free();
        }
        FThreads.Clear();          // 原文 FThreads.Free

        ClearTasks();
        FTasks.Clear();            // 原文 FTasks.Free

        // 原文 FSectionTask.Free（托管侧无可释放资源）
    }

    /// <summary>Delphi `Free` 的托管入口。</summary>
    public void Free() => Dispose();

    private static void ThreadSleep1() => Thread.Sleep(1);

    /// <summary>原文 `Application.ProcessMessages`（Forms 单元）。</summary>
    private static void ApplicationProcessMessages() => System.Windows.Forms.Application.DoEvents();

    /// <summary>ThreadPool.pas:291-295 `procedure DoTaskFinished(Thread, Task);`（原文 protected virtual；见上）。</summary>

    /// <summary>
    /// ThreadPool.pas:297-330 `function FindTask(Task: TPoolTask): TPoolTask; virtual;`
    /// </summary>
    public virtual TPoolTask? FindTask(TPoolTask Task)
    {
        TPoolTask? Result = null;

        for (int I = 0; I <= FThreads.Count - 1; I++)
        {
            TPoolThread Thread = FThreads[I];
            TPoolTask? ContextTask = Thread.ContextTask;
            if ((ContextTask != null) && ContextTask.CompareTask(Task))
            {
                Result = ContextTask;
                return Result;      // 原文 Exit
            }
        }

        FSectionTaskEnter();
        try
        {
            for (int I = 0; I <= FTasks.Count - 1; I++)
            {
                TPoolTask ContextTask = FTasks[I];
                if (ContextTask.CompareTask(Task))
                {
                    Result = ContextTask;
                    break;          // 原文 Break
                }
            }
        }
        finally
        {
            FSectionTaskLeave();
        }

        return Result;
    }

    /// <summary>
    /// ThreadPool.pas:332-351 `procedure AddTask(Task: TPoolTask); virtual;`
    /// <para>
    /// ★ 原文如此（缺陷，照抄）：① 任务先进队列，**只唤醒 1 个**空闲线程；
    /// ② `Sleep(5)` 那行被注释掉 ⇒ 空闲线程可能尚未把 `FInExecuteLoop` 置位，
    ///    紧接着的 `GetSleepingThread` 会把同一个线程再算一次空闲。
    /// </para>
    /// </summary>
    public virtual void AddTask(TPoolTask Task)
    {
        FSectionTaskEnter();
        try
        {
            FTasks.Add(Task);
        }
        finally
        {
            FSectionTaskLeave();
        }

        // 如果有空闲线程，把任务加进去，并让空闲线程执行
        TPoolThread? SleepThread = GetSleepingThread();
        if (SleepThread != null)
        {
            SleepThread.TriggerEvent();
            // 小停片刻，让空闲线程先跑起来，不然 AddTask 速度太快，而空闲线程还未置为非空闲
            //Sleep(5);
        }
    }

    /// <summary>ThreadPool.pas:353-368 `procedure CancelAndClearAllTask;`</summary>
    public void CancelAndClearAllTask()
    {
        ClearTasks();

        for (int I = 0; I <= FThreads.Count - 1; I++)
        {
            TPoolThread Thread = FThreads[I];
            if ((!Thread.GetSleeping()) && Thread.ContextTask != null)
            {
                Thread.ContextTask.Cancel();
            }
        }
    }

    /// <summary>ThreadPool.pas:370-386 `procedure ClearTasks;`</summary>
    public void ClearTasks()
    {
        FSectionTaskEnter();
        try
        {
            for (int I = 0; I <= FTasks.Count - 1; I++)
            {
                TPoolTask Task = FTasks[I];
                Task.Free();
            }
            FTasks.Clear();
        }
        finally
        {
            FSectionTaskLeave();
        }
    }

    // ---- ThreadPool.pas:102-113 属性 ----

    /// <summary>ThreadPool.pas:102 `property TaskCount: Integer read GetTaskCount;`</summary>
    public int TaskCount => GetTaskCount();

    /// <summary>ThreadPool.pas:103 `property Tasks[Index: Integer]: TPoolTask read GetTasks;`</summary>
    public TPoolTask GetTaskAt(int Index) => GetTasks(Index);

    /// <summary>ThreadPool.pas:105 `property ThreadCount: Integer read GetThreadCount;`</summary>
    public int ThreadCount => GetThreadCount();

    /// <summary>ThreadPool.pas:106 `property Threads[Index: Integer]: TPoolThread read GetThreads;`</summary>
    public TPoolThread GetThreadAt(int Index) => GetThreads(Index);

    /// <summary>ThreadPool.pas:108 `property OnTaskFinished: TTaskFinishedEvent read FOnTaskFinished write FOnTaskFinished;`</summary>
    public TTaskFinishedEvent? OnTaskFinished
    {
        get => FOnTaskFinished;
        set => FOnTaskFinished = value;
    }

    /// <summary>ThreadPool.pas:410-414 `function LockTaskList: TList;`</summary>
    public List<TPoolTask> LockTaskList()
    {
        FSectionTaskEnter();
        return FTasks;
    }

    /// <summary>ThreadPool.pas:416-419 `procedure UnlockTaskList;`</summary>
    public void UnlockTaskList()
    {
        FSectionTaskLeave();
    }

    /// <summary>ThreadPool.pas:113 `property IsWantDestroy: Boolean read FIsWantDestroy;`</summary>
    public bool IsWantDestroy => FIsWantDestroy;
}

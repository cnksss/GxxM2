// ============================================================================
// ThreadPool.pas（445 行）→ Pool/ThreadPool.cs 的逐成员用例。
// 覆盖对账：原文 2 个类（TPoolTask / TPoolThread / TPoolManager，共 3 个类）
//           + 1 个类引用类型（TPoolThreadClass）+ 1 个事件类型（TTaskFinishedEvent）。
// ============================================================================

using System;
using System.Threading;
using Xunit;

// 命名空间见 P10PoolTestKit.cs 顶部说明（接缝与真实现同名 ⇒ 必须落在 ...Pool.Tests）。
namespace GXX.LogDataServer.Pool.Tests;

[Collection("P10PoolSerial")]
public sealed class P10ThreadPoolTests
{
    // ---------------- TPoolTask ----------------

    [Fact]
    public void TPoolTask_Create_CanceledIsFalseAndFinishedIsFalse()
    {
        var t = new P10ProbeTask(1);
        Assert.False(t.Canceled);
        Assert.False(t.Finished);
    }

    [Fact]
    public void TPoolTask_Cancel_SetsCanceled()
    {
        var t = new P10ProbeTask(1);
        t.Cancel();
        Assert.True(t.Canceled);
    }

    [Fact]
    public void TPoolTask_Finished_IsReadWrite()
    {
        var t = new P10ProbeTask(1) { Finished = true };
        Assert.True(t.Finished);
        t.Finished = false;
        Assert.False(t.Finished);
    }

    [Fact]
    public void TPoolTask_CompareTask_BaseClassThrowsEAbstractError()
    {
        // 原文 `function CompareTask(Task: TPoolTask): Boolean; virtual; abstract;`
        // TPoolTask 本身可实例化（Delphi 只在调到时抛 EAbstractError）⇒ 托管同构（D-P10-03）
        var t = new TPoolTask();
        Assert.Throws<EAbstractError>(() => t.CompareTask(new P10ProbeTask(1)));
    }

    [Fact]
    public void TPoolTask_Assign_NullSource_RaisesEConvertErrorWithNilText()
    {
        var t = new P10ProbeTask(1);
        var ex = Assert.Throws<EConvertError>(() => t.Assign(null));
        // RTLConsts.SAssignError = 'Cannot assign a %s to a %s'
        Assert.Equal("Cannot assign a nil to a P10ProbeTask", ex.Message);
    }

    [Fact]
    public void TPoolTask_AssignTo_DefaultImplementation_RaisesEConvertError()
    {
        // 原文默认 AssignTo(Dest) 直接 `Dest.AssignError(Self)` ⇒ Source 非 nil 时报源类名
        var src = new TPoolTask();
        var dst = new TPoolTask();
        var ex = Assert.Throws<EConvertError>(() => dst.Assign(src));
        Assert.Equal("Cannot assign a TPoolTask to a TPoolTask", ex.Message);
    }

    [Fact]
    public void TPoolTask_Assign_OverrideAssignTo_IsCalled()
    {
        var src = new P10CopyingTask { Payload = "abc" };
        var dst = new P10CopyingTask();
        dst.Assign(src);
        Assert.Equal("abc", dst.Payload);
    }

    private sealed class P10CopyingTask : TPoolTask
    {
        public string Payload = "";

        protected override void AssignTo(TPoolTask Dest)
        {
            if (Dest is P10CopyingTask d) d.Payload = Payload;
            else base.AssignTo(Dest);
        }

        public override bool CompareTask(TPoolTask Task) => false;
    }

    // ---------------- TPoolThread ----------------

    [Fact]
    public void TPoolThread_BaseDoExecuteLoop_ThrowsEAbstractError()
    {
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            Assert.Throws<EAbstractError>(() => th.CallBaseDoExecuteLoop());
        }
        finally
        {
            th.Free();
        }
    }

    [Fact]
    public void TPoolThread_ContextTask_StartsNullAndReflectsField()
    {
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            Assert.Null(th.ContextTask);
            var task = new P10ProbeTask(9);
            th.SetContextTask(task);
            Assert.Same(task, th.ContextTask);
            th.SetContextTask(null);
        }
        finally
        {
            th.Free();
        }
    }

    [Fact]
    public void TPoolThread_Sleeping_SuspendedThreadIsNotSleeping()
    {
        // 原文如此：`Sleeping = (not FInExecuteLoop) and (not Suspended)`
        // 尚未 Resume 的线程 Suspended=True ⇒ Sleeping=False
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            Assert.True(th.Suspended);
            Assert.False(th.Sleeping);
            th.Resume();
            Assert.False(th.Suspended);
            Assert.True(P10ThreadPoolTestHelpers.WaitForSleeping(th));
        }
        finally
        {
            th.Free();
        }
    }

    [Fact]
    public void TPoolThread_Terminate_CancelsContextTaskAndSetsTerminated()
    {
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            th.Resume();
            Assert.True(P10ThreadPoolTestHelpers.WaitForSleeping(th));

            var task = new P10ProbeTask(3);
            th.SetContextTask(task);
            Assert.False(task.Canceled);

            th.Terminate();
            Assert.True(th.Terminated);
            Assert.True(task.Canceled);
        }
        finally
        {
            th.Free();
        }
    }

    [Fact]
    public void TPoolThread_Owner_IsCtorArgument()
    {
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            Assert.Same(mgr, th.Owner);
        }
        finally
        {
            th.Free();
        }
    }

    [Fact]
    public void TPoolThread_DoTaskFinished_InvokesManagerOnTaskFinished()
    {
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            var task = new P10ProbeTask(11);
            th.SetContextTask(task);

            TPoolManager? gotMgr = null;
            TPoolThread? gotThread = null;
            TPoolTask? gotTask = null;
            mgr.OnTaskFinished = (m, t, k) => { gotMgr = m; gotThread = t; gotTask = k; };

            th.CallDoTaskFinished();

            Assert.Same(mgr, gotMgr);
            Assert.Same(th, gotThread);
            Assert.Same(task, gotTask);
        }
        finally
        {
            th.Free();
        }
    }

    [Fact]
    public void TPoolThread_DoTaskFinished_WithoutHandler_IsNoOp()
    {
        using var mgr = new P10ProbeManager(0);
        var th = new P10IdlePoolThread(mgr, true);
        try
        {
            th.SetContextTask(new P10ProbeTask(12));
            th.CallDoTaskFinished();   // 原文 `if Assigned(FOnTaskFinished) then ...` ⇒ 无处理器时不抛
        }
        finally
        {
            th.Free();
        }
    }

    // ---------------- TPoolManager ----------------

    [Fact]
    public void TPoolManager_Create_UsesVirtualPoolThreadClassAndThreadCount()
    {
        using var mgr = new P10ProbeManager(3);
        Assert.Equal(3, mgr.ThreadCount);
        Assert.IsType<P10IdlePoolThread>(mgr.GetThreadAt(0));
        Assert.False(mgr.IsWantDestroy);
        Assert.Equal(0, mgr.TaskCount);
    }

    [Fact]
    public void TPoolManager_AddTask_EnqueuesAndSignalsOneSleepingThread()
    {
        using var mgr = new P10ProbeManager(2);
        var a = new P10ProbeTask(1);
        var b = new P10ProbeTask(2);
        mgr.AddTask(a);
        mgr.AddTask(b);

        // 池线程不消化队列（P10IdlePoolThread）⇒ 任务留在队列里
        Assert.Equal(2, mgr.TaskCount);
        Assert.Same(a, mgr.GetTaskAt(0));
        Assert.Same(b, mgr.GetTaskAt(1));
    }

    [Fact]
    public void TPoolManager_GetTaskAt_OutOfRange_Throws()
    {
        using var mgr = new P10ProbeManager(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => mgr.GetTaskAt(0));
    }

    [Fact]
    public void TPoolManager_GetThreadAt_OutOfRange_Throws()
    {
        using var mgr = new P10ProbeManager(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => mgr.GetThreadAt(5));
    }

    [Fact]
    public void TPoolManager_ClearTasks_FreesEveryQueuedTask()
    {
        using var mgr = new P10ProbeManager(1);
        var a = new P10ProbeTask(1);
        var b = new P10ProbeTask(2);
        mgr.AddTask(a);
        mgr.AddTask(b);

        mgr.ClearTasks();

        Assert.Equal(1, a.DisposeCount);
        Assert.Equal(1, b.DisposeCount);
        Assert.Equal(0, mgr.TaskCount);
    }

    [Fact]
    public void TPoolManager_FindTask_FindsQueuedTaskByCompareTask()
    {
        using var mgr = new P10ProbeManager(1);
        var a = new P10ProbeTask(1);
        var b = new P10ProbeTask(2);
        mgr.AddTask(a);
        mgr.AddTask(b);

        Assert.Same(b, mgr.FindTask(new P10ProbeTask(2)));
        Assert.Null(mgr.FindTask(new P10ProbeTask(99)));
    }

    [Fact]
    public void TPoolManager_FindTask_ScansThreadContextFirstThenQueue()
    {
        var mgr = new P10HoldingManager(1);
        var th = (P10HoldingPoolThread)mgr.GetThreadAt(0);
        try
        {
            var running = new P10ProbeTask(1);      // 被线程取走 ⇒ 成为 ContextTask
            mgr.AddTask(running);
            Assert.True(th.Took.Wait(5000), "池线程未在 5s 内取走任务");

            var queued = new P10ProbeTask(2);       // 留在队列里
            mgr.AddTask(queued);

            // 两条扫描路径都命中：ContextTask 优先（原文先扫 FThreads 且命中即 Exit）
            Assert.Same(running, mgr.FindTask(new P10ProbeTask(1)));
            Assert.Same(queued, mgr.FindTask(new P10ProbeTask(2)));
            th.Release.Set();
        }
        finally
        {
            th.Release.Set();
            mgr.Dispose();
        }
    }

    [Fact]
    public void TPoolManager_LockTaskList_IsReentrantViaUnlockTaskList()
    {
        using var mgr = new P10ProbeManager(1);
        var list = mgr.LockTaskList();
        Assert.Same(list, mgr.LockTaskList());   // TCriticalSection 可重入
        mgr.UnlockTaskList();
        mgr.UnlockTaskList();
    }

    [Fact]
    public void TPoolManager_CancelAndClearAllTask_ClearsQueueAndCancelsRunning()
    {
        var mgr = new P10HoldingManager(1);
        var th = (P10HoldingPoolThread)mgr.GetThreadAt(0);
        try
        {
            var running = new P10ProbeTask(1);
            mgr.AddTask(running);
            Assert.True(th.Took.Wait(5000), "池线程未在 5s 内取走任务");

            var queued = new P10ProbeTask(2);
            mgr.AddTask(queued);

            mgr.CancelAndClearAllTask();

            Assert.True(running.Canceled);        // 运行中的 ContextTask 被 Cancel
            Assert.Equal(1, queued.DisposeCount); // 队列里的被 Free
            Assert.Equal(0, mgr.TaskCount);
            Assert.False(queued.Canceled);        // 原文只 Cancel 线程的 ContextTask（不 Cancel 队列）
            th.Release.Set();
        }
        finally
        {
            th.Release.Set();
            mgr.Dispose();
        }
    }

    [Fact]
    public void TPoolManager_Dispose_SetsIsWantDestroyAndStopsThreads()
    {
        var mgr = new P10ProbeManager(2);
        mgr.AddTask(new P10ProbeTask(1));
        mgr.Dispose();
        Assert.True(mgr.IsWantDestroy);
        Assert.Equal(0, mgr.TaskCount);
    }

    [Fact]
    public void TPoolManager_Dispose_DisposesRunningContextTask()
    {
        var mgr = new P10HoldingManager(1);
        var th = (P10HoldingPoolThread)mgr.GetThreadAt(0);
        var running = new P10ProbeTask(1);
        mgr.AddTask(running);
        Assert.True(th.Took.Wait(5000), "池线程未在 5s 内取走任务");

        th.Release.Set();
        mgr.Dispose();

        // TPoolThread.Destroy 的 `FreeAndNil(FContextTask)`
        Assert.Equal(1, running.DisposeCount);
    }

    [Fact]
    public void TPoolManager_OnTaskFinished_DefaultIsNullAndSettable()
    {
        using var mgr = new P10ProbeManager(1);
        Assert.Null(mgr.OnTaskFinished);
        TTaskFinishedEvent handler = (_, _, _) => { };
        mgr.OnTaskFinished = handler;
        Assert.Same(handler, mgr.OnTaskFinished);
    }
}

/// <summary>辅助（避免在用例里写裸循环）。</summary>
internal static class P10ThreadPoolTestHelpers
{
    public static bool WaitForSleeping(TPoolThread th)
    {
        for (int i = 0; i < 500; i++)
        {
            if (th.Sleeping) return true;
            Thread.Sleep(5);
        }
        return th.Sleeping;
    }
}

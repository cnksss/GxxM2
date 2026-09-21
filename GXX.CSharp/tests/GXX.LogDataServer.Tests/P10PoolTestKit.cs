// ============================================================================
// 车道 p10-db-login-forms —— LogDataServer 线程池/搜索池测试基础设施。
//
//  · `P10PoolSerial`：本组用例会起真实线程并读 5 秒级等待，串行执行。
//  · 探针线程/任务：把原文 protected 面（`DoExecuteLoop` / `FContextTask` /
//    `DoTaskFinished`）在用例里暴露出来。
//  · `P10LogFile`：按 FileSearchPool.DoSearch 的读序构造日志文件（临时目录，不碰真实数据目录）。
// ============================================================================

using System;
using System.IO;
using System.Threading;
using GXX.Core;
using Xunit;

// ⚠ 命名空间刻意落在 `GXX.LogDataServer.Pool.Tests`（而不是工程根命名空间
//   `GXX.LogDataServer.Tests`）：接缝 `GXX.LogDataServer/LogDataShare.cs` 里已有一个
//   "最小面"的 `TPoolTask` / `TSearchTask`，与本车道的真实现**同名**。
//   若本文件位于 `GXX.LogDataServer.Tests`，按 C# 名字查找规则（先外层命名空间成员、
//   后 compilation unit 的 using）简单名 `TPoolTask` 会解析到**接缝**那份（实测 CS0115）。
//   落在 `GXX.LogDataServer.Pool.Tests` 后，外层命名空间 `...Pool` 的成员先生效 ⇒ 无歧义。
namespace GXX.LogDataServer.Pool.Tests;

/// <summary>本车道线程族测试串行集合（真实线程 + 静态接缝）。</summary>
[CollectionDefinition("P10PoolSerial", DisableParallelization = true)]
public sealed class P10PoolSerialCollection
{
}

/// <summary>探针任务：`CompareTask` 按 Id 相等（用于 FindTask 取证）。</summary>
public sealed class P10ProbeTask : TPoolTask
{
    public readonly int Id;

    /// <summary>`Dispose`（= Delphi `Free`）被调用次数。</summary>
    public int DisposeCount;

    public P10ProbeTask(int id) => Id = id;

    public override bool CompareTask(TPoolTask Task) => Task is P10ProbeTask p && p.Id == Id;

    public override void Dispose() => DisposeCount++;
}

/// <summary>探针线程：`DoExecuteLoop` **不**消化任务（任务留在队列里，便于计数）。</summary>
public class P10IdlePoolThread : TPoolThread
{
    public int LoopCount;

    public P10IdlePoolThread(TPoolManager AOwner, bool CreateSuspended)
        : base(AOwner, CreateSuspended)
    {
    }

    protected override void DoExecuteLoop() => Interlocked.Increment(ref LoopCount);

    /// <summary>暴露 protected 的 `FContextTask`。</summary>
    public void SetContextTask(TPoolTask? t) => FContextTask = t;

    /// <summary>暴露 protected 的 `DoTaskFinished`。</summary>
    public void CallDoTaskFinished() => DoTaskFinished();

    /// <summary>暴露基类 `TPoolThread.DoExecuteLoop`（应抛 EAbstractError）。</summary>
    public void CallBaseDoExecuteLoop() => base.DoExecuteLoop();
}

/// <summary>探针管理器：池线程用 <see cref="P10IdlePoolThread"/>。</summary>
public sealed class P10ProbeManager : TPoolManager
{
    public P10ProbeManager(int ThreadCount) : base(ThreadCount) { }

    protected override Type GetPoolThreadClass() => typeof(P10IdlePoolThread);
}

/// <summary>持有型线程：把队列头的任务搬进 `FContextTask` 并阻塞，直到用例放行。</summary>
public sealed class P10HoldingPoolThread : TPoolThread
{
    public readonly ManualResetEventSlim Took = new(false);
    public readonly ManualResetEventSlim Release = new(false);

    public P10HoldingPoolThread(TPoolManager AOwner, bool CreateSuspended)
        : base(AOwner, CreateSuspended)
    {
    }

    protected override void DoExecuteLoop()
    {
        var tasks = Owner.LockTaskList();
        try
        {
            if (tasks.Count > 0)
            {
                FContextTask = tasks[0];
                tasks.RemoveAt(0);
            }
        }
        finally
        {
            Owner.UnlockTaskList();
        }

        if (FContextTask != null)
        {
            Took.Set();
            Release.Wait(5000);
        }
    }
}

/// <summary>持有型管理器。</summary>
public sealed class P10HoldingManager : TPoolManager
{
    public P10HoldingManager(int ThreadCount) : base(ThreadCount) { }

    protected override Type GetPoolThreadClass() => typeof(P10HoldingPoolThread);
}

/// <summary>
/// 按 `FileSearchPool.DoSearch` 的**读序**构造一条日志记录（临时目录，不碰真实数据）。
/// 布局：nServerNumber(4) nServerIndex(4) nAction(4) [nLen(4)+mapName] nX(4) nY(4)
///       [nLen(4)+objectName] actorType(1) [nLen(4)+itemName] itemIndex(4)
///       [nLen(4)+actObjectName] data1(4) data2(4) [nLen(4)+logDesc] date(8)
/// </summary>
public static class P10LogFile
{
    public static void WriteRecord(BinaryWriter w, uint nAction, string mapName, string objectName,
        byte actorType, string itemName, int itemIndex, string actObjectName, string logDesc,
        int nServerNumber = 1, int nServerIndex = 2, int nX = 100, int nY = 200,
        int nData1 = 7, int nData2 = 8)
    {
        w.Write(nServerNumber);
        w.Write(nServerIndex);
        w.Write(nAction);
        WriteAnsi(w, mapName);
        w.Write(nX);
        w.Write(nY);
        WriteAnsi(w, objectName);
        w.Write(actorType);
        WriteAnsi(w, itemName);
        w.Write(itemIndex);
        WriteAnsi(w, actObjectName);
        w.Write(nData1);
        w.Write(nData2);
        WriteAnsi(w, logDesc);
        w.Write(DateTime.Now.ToOADate());
    }

    private static void WriteAnsi(BinaryWriter w, string s)
    {
        byte[] b = EncodingInit.GBK.GetBytes(s ?? "");
        w.Write(b.Length);
        w.Write(b);
    }

    /// <summary>写一份单记录日志文件到临时目录，返回完整路径。</summary>
    public static string WriteSingle(string dir, uint nAction = 5, string objectName = "英雄")
    {
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "p10log.dat");
        using var w = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
        WriteRecord(w, nAction, "0", objectName, 1, "itemA", 42, "monster", "desc");
        return path;
    }

    /// <summary>在临时目录里造一份全部 256 种动作都为 false 的筛选表。</summary>
    public static bool[] NoActions() => new bool[256];
}

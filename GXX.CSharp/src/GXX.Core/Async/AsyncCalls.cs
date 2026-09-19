using System;

namespace GXX.Core.Async;

/// <summary>
/// AsyncCalls.pas 的单元级全局量：<c>ThreadPool</c> 与模块初始化/收尾。
/// <para>
/// 原文 <c>initialization</c>（3417-3425）在单元装载时创建全局线程池，
/// <c>finalization</c>（3427-3436）销毁它。托管侧改为惰性创建：
/// 只有真正用到全局池的调用点才会构造（避免测试/无异步需求的进程产生 OS 资源）。
/// 这是本移植与原文唯一的初始化时机差异，已在报告登记。
/// </para>
/// </summary>
public static class AsyncCallsGlobals
{
    private static readonly object Sync = new object();
    private static TThreadPool _threadPool;

    /// <summary>原文 1100：<c>var ThreadPool: TThreadPool;</c></summary>
    public static TThreadPool ThreadPool
    {
        get
        {
            TThreadPool pool = _threadPool;
            if (pool != null)
                return pool;
            lock (Sync)
            {
                return _threadPool ??= new TThreadPool(AsyncCalls.Runtime);
            }
        }
        set
        {
            lock (Sync)
            {
                _threadPool = value;
            }
        }
    }

    /// <summary>对应 <c>finalization</c>（3427-3429）：销毁并清空全局池。</summary>
    public static void FinalizeUnit()
    {
        lock (Sync)
        {
            _threadPool?.Free();
            _threadPool = null;
        }
    }
}

/// <summary>
/// AsyncCalls.pas 的对外函数面（1:1）：<c>SetMaxAsyncCallThreads</c> /
/// <c>AsyncCall</c> 全家族 / <c>AsyncCallEx</c> / <c>AsyncExec</c> / 多对象等待包装。
/// </summary>
/// <remarks>
/// <b>可测性注入钩子</b>：原文各函数直接引用单元级全局 <c>ThreadPool</c>；
/// 本移植为每个入口追加两个<b>可选</b>参数 <c>pool</c> / <c>runtime</c>
/// （省略即取全局，行为与原文一致），使单测无需改动全局状态。
/// </remarks>
public static class AsyncCalls
{
    /// <summary>注入的运行时；宿主可替换（对应原文散落的 Win32 直接调用）。</summary>
    public static TAsyncCallRuntime Runtime { get; set; } = TAsyncCallRuntime.Default;

    /// <summary>原文 1100 的单元级 <c>ThreadPool</c> 全局量。</summary>
    public static TThreadPool ThreadPool
    {
        get => AsyncCallsGlobals.ThreadPool;
        set => AsyncCallsGlobals.ThreadPool = value;
    }

    private static TThreadPool P(TThreadPool pool) => pool ?? ThreadPool;

    private static TAsyncCallRuntime R(TAsyncCallRuntime runtime) => runtime ?? Runtime;

    // ------------------------------------------------------------------
    // 原文 1102-1113：线程池规模
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 1102-1108：
    /// <code>
    /// if MaxThreads >= Length(ThreadPool.FThreads) then MaxThreads := Length(ThreadPool.FThreads);
    /// if MaxThreads >= 0 then ThreadPool.FMaxThreads := MaxThreads;
    /// </code>
    /// <b>边界</b>：<c>&gt;= 256</c> 钳制为 256；<b>负数被忽略</b>（不改变当前值）。
    /// </summary>
    public static void SetMaxAsyncCallThreads(int MaxThreads, TThreadPool pool = null)
    {
        TThreadPool p = P(pool);
        if (MaxThreads >= AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_LENGTH)
            MaxThreads = AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_LENGTH;
        if (MaxThreads >= 0)
            p.SetMaxThreads(MaxThreads);
    }

    /// <summary>原文 1110-1113：<c>Result := ThreadPool.FMaxThreads</c></summary>
    public static int GetMaxAsyncCallThreads(TThreadPool pool = null)
    {
        return P(pool).MaxThreads;
    }

    // ------------------------------------------------------------------
    // 原文 1117-1235：AsyncCall（函数 / 方法）重载
    // ------------------------------------------------------------------

    /// <summary>原文 1117-1124</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgObjectProc Proc, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        // Execute the function synchron if no thread pool exists
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgObject(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>
    /// 原文 1126-1129：
    /// <code>
    /// Result := AsyncCall(TAsyncCallArgObjectProc(Proc), TObject(Arg));
    /// </code>
    /// <para>
    /// <b>差异断言点</b>：Integer 重载<b>自己不做</b> <c>MaxThreads = 0</c> 判定，
    /// 而是把 Integer 塞进 TObject 槽转调 <b>Object 重载</b>，由后者判定；
    /// 因此最终构造的是 <c>TAsyncCallArgObject</c>（原文<b>没有</b>
    /// <c>TAsyncCallArgInteger</c> 这个类）。
    /// </para>
    /// </summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgIntegerProc Proc, int Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall(
            (TAsyncCallArgObjectProc)(arg => Proc((int)arg)),
            (object)Arg,
            pool,
            runtime);
    }

    /// <summary>原文 1131-1138</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgStringProc Proc, string Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgString(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1140-1147</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgWideStringProc Proc, string Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgWideString(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1149-1156</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgInterfaceProc Proc, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgInterface(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1158-1165</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgExtendedProc Proc, double Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgExtended(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1167-1174</summary>
    public static IAsyncCall AsyncCallVar(TAsyncCallArgVariantProc Proc, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgVariant(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1178-1185</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgObjectMethod Method, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgObject(Method, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1187-1190：与方法版 Object 重载的关系同 <see cref="AsyncCall(TAsyncCallArgIntegerProc,int,TThreadPool,TAsyncCallRuntime)"/></summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgIntegerMethod Method, int Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall(
            (TAsyncCallArgObjectMethod)(arg => Method((int)arg)),
            (object)Arg,
            pool,
            runtime);
    }

    /// <summary>原文 1192-1199</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgStringMethod Method, string Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgString(Method, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1201-1208</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgWideStringMethod Method, string Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgWideString(Method, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1210-1217</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgInterfaceMethod Method, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgInterface(Method, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1219-1226</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgExtendedMethod Method, double Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgExtended(Method, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1228-1235</summary>
    public static IAsyncCall AsyncCallVar(TAsyncCallArgVariantMethod Method, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgVariant(Method, Arg, r).ExecuteAsync(p);
    }

    // ------------------------------------------------------------------
    // 原文 1239-1272：事件（procedure of object）重载 —— 全部转调对应的“方法”重载。
    // ------------------------------------------------------------------

    /// <summary>原文 1239-1242</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgObjectEvent Method, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgObjectMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    /// <summary>原文 1244-1247</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgIntegerEvent Method, int Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgIntegerMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    /// <summary>原文 1249-1252</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgStringEvent Method, string Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgStringMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    /// <summary>原文 1254-1257</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgWideStringEvent Method, string Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgWideStringMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    /// <summary>原文 1259-1262</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgInterfaceEvent Method, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgInterfaceMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    /// <summary>原文 1264-1267</summary>
    public static IAsyncCall AsyncCall(TAsyncCallArgExtendedEvent Method, double Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgExtendedMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    /// <summary>原文 1269-1272</summary>
    public static IAsyncCall AsyncCallVar(TAsyncCallArgVariantEvent Method, object Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCallVar((TAsyncCallArgVariantMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    // ------------------------------------------------------------------
    // 原文 1274-1283：IAsyncRunnable
    // ------------------------------------------------------------------

    /// <summary>原文 1274-1278：<c>IAsyncRunnable(Arg).AsyncRun; Result := 0;</c></summary>
    public static int AsyncCallRunnable(object Arg)
    {
        ((IAsyncRunnable)Arg).AsyncRun();
        return 0;
    }

    /// <summary>原文 1280-1283：<c>Result := AsyncCall(AsyncCallRunnable, IInterface(Runnable));</c></summary>
    public static IAsyncCall AsyncCall(IAsyncRunnable Runnable,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCall((TAsyncCallArgInterfaceProc)(a => AsyncCallRunnable(a)), (object)Runnable, pool, runtime);
    }

    // ------------------------------------------------------------------
    // 原文 1287-1299：AsyncExec
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 1287-1299：<c>AsyncExec</c> —— 在等待期间反复调用 <paramref name="IdleMsgMethod"/>。
    /// <para>
    /// <c>while MsgAsyncMultiSync([Handle], False, INFINITE, QS_ALLINPUT or QS_ALLPOSTMESSAGE) = 1 do</c>
    /// —— 常量 <c>1</c> 即“单个等待对象的列表被消息唤醒”时的返回值
    /// （<c>WAIT_OBJECT_0 + Count</c>，Count=1）。原文如此，照抄。
    /// </para>
    /// </summary>
    public static void AsyncExec(TAsyncCallArgObjectEvent Method, object Arg, TAsyncIdleMsgMethod IdleMsgMethod,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        IAsyncCall Handle = AsyncCall(Method, Arg, p, r);
        if (IdleMsgMethod != null)
        {
            Handle.ForceDifferentThread();
            IdleMsgMethod();
            while (TAsyncCallMultiSync.MsgAsyncMultiSync(
                       r, p, new[] { Handle }, false, AsyncCallsConst.INFINITE,
                       AsyncCallsConst.QS_ALLINPUT | AsyncCallsConst.QS_ALLPOSTMESSAGE) == 1)
                IdleMsgMethod();
        }
    }

    // ------------------------------------------------------------------
    // 原文 1354-1375：AsyncCallEx（记录型参数）
    // ------------------------------------------------------------------

    /// <summary>原文 1354-1361</summary>
    public static IAsyncCall AsyncCallEx(TAsyncCallArgRecordProc Proc, IntPtr Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Proc(Arg));
        return new TAsyncCallArgRecord(Proc, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1363-1370</summary>
    public static IAsyncCall AsyncCallEx(TAsyncCallArgRecordMethod Method, IntPtr Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        if (p.MaxThreads == 0)
            return new TSyncCall(Method(Arg));
        return new TAsyncCallMethodArgRecord(Method, Arg, r).ExecuteAsync(p);
    }

    /// <summary>原文 1372-1375</summary>
    public static IAsyncCall AsyncCallEx(TAsyncCallArgRecordEvent Method, IntPtr Arg,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        return AsyncCallEx((TAsyncCallArgRecordMethod)(a => { Method(a); return 0; }), Arg, pool, runtime);
    }

    // ------------------------------------------------------------------
    // 原文 1380-1406：array of const 变参重载（SUPPORT_LOCAL_FUNCTIONS，D7 活跃）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 1380-1392：<c>AsyncCall(Proc: TCdeclFunc; const Args: array of const)</c>。
    /// <b>注意</b>：<c>MaxThreads = 0</c> 时走的是 <c>InternExecuteSyncCall</c> +
    /// <c>TAsyncCall.Create(Call)</c>，<b>不是</b> <see cref="TSyncCall"/>。
    /// </summary>
    public static IAsyncCall AsyncCall(nint Proc, TVarRec[] Args,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        var Call = new TAsyncCallArrayOfConst(Proc, Args, r);
        if (p.MaxThreads == 0)
        {
            Call.Pool = p;
            Call.InternExecuteSyncCall();
            return new TAsyncCall(Call, r);
        }
        return Call.ExecuteAsync(p);
    }

    /// <summary>原文 1394-1406：<c>AsyncCall(Proc: TCdeclMethod; const Args: array of const)</c></summary>
    public static IAsyncCall AsyncCall(nint Proc, object MethodData, TVarRec[] Args,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = P(pool);
        TAsyncCallRuntime r = R(runtime);
        var Call = new TAsyncCallArrayOfConst(Proc, MethodData, Args, r);
        if (p.MaxThreads == 0)
        {
            Call.Pool = p;
            Call.InternExecuteSyncCall();
            return new TAsyncCall(Call, r);
        }
        return Call.ExecuteAsync(p);
    }

    // ------------------------------------------------------------------
    // 原文 1641-1663 的对外包装（使用全局池 + 注入运行时）
    // ------------------------------------------------------------------

    public static uint AsyncMultiSync(System.Collections.Generic.IReadOnlyList<IAsyncCall> List,
        bool WaitAll = true, uint Milliseconds = AsyncCallsConst.INFINITE,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
        => TAsyncCallMultiSync.AsyncMultiSync(R(runtime), P(pool), List, WaitAll, Milliseconds);

    public static uint AsyncMultiSyncEx(System.Collections.Generic.IReadOnlyList<IAsyncCall> List,
        IAsyncWaitObject[] Handles, bool WaitAll = true, uint Milliseconds = AsyncCallsConst.INFINITE,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
        => TAsyncCallMultiSync.AsyncMultiSyncEx(R(runtime), P(pool), List, Handles, WaitAll, Milliseconds);

    public static uint MsgAsyncMultiSync(System.Collections.Generic.IReadOnlyList<IAsyncCall> List,
        bool WaitAll, uint Milliseconds, uint dwWakeMask,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
        => TAsyncCallMultiSync.MsgAsyncMultiSync(R(runtime), P(pool), List, WaitAll, Milliseconds, dwWakeMask);

    public static uint MsgAsyncMultiSyncEx(System.Collections.Generic.IReadOnlyList<IAsyncCall> List,
        IAsyncWaitObject[] Handles, bool WaitAll, uint Milliseconds, uint dwWakeMask,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
        => TAsyncCallMultiSync.MsgAsyncMultiSyncEx(R(runtime), P(pool), List, Handles, WaitAll, Milliseconds, dwWakeMask);
}

/// <summary>
/// 原文 608-716：<c>TAsyncCalls</c> 类。
/// <para>
/// <b>条件编译登记</b>：整个类体处于 <c>{$IFDEF DELPHI2009_UP}</c>（576-717、3049-3326），
/// <b>Delphi 7 下不参与编译</b>。此处只移植其中与编译器版本无关的<b>决策语义</b>
/// （<c>MaxThreads = 0 ⇒ 就地同步执行</c>、<c>VCLInvoke</c> 的主线程短路、<c>MsgExec</c> 的空闲循环），
/// 泛型 <c>Invoke&lt;T...&gt;</c> 家族与 <c>TMultiArgProcCall&lt;...&gt;</c> 未移植（见报告“未完成”节）。
/// </para>
/// </summary>
public static class TAsyncCallsFacade
{
    /// <summary>
    /// 原文 3310-3324：<c>TAsyncCalls.MsgExec</c>（D2009+ 分支）。
    /// <para>与单元级 <see cref="AsyncCalls.AsyncExec"/> 的循环形状相同，仅入口条件不同。</para>
    /// </summary>
    public static void MsgExec(IAsyncCall AsyncCall, TAsyncIdleMsgMethod IdleMsgMethod,
        TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = pool ?? AsyncCalls.ThreadPool;
        TAsyncCallRuntime r = runtime ?? AsyncCalls.Runtime;

        if (r.WaitService.IsMainThread)
        {
            if (IdleMsgMethod != null)
            {
                AsyncCall.ForceDifferentThread();
                IdleMsgMethod();
                while (TAsyncCallMultiSync.MsgAsyncMultiSync(
                           r, p, new[] { AsyncCall }, false, AsyncCallsConst.INFINITE,
                           AsyncCallsConst.QS_ALLINPUT | AsyncCallsConst.QS_ALLPOSTMESSAGE) == 1)
                    IdleMsgMethod();
            }
        }
        else
            AsyncCall.Sync();
    }

    /// <summary>
    /// 原文 3292-3308：<c>TAsyncCalls.VCLInvoke</c>（D2009+ 分支）。
    /// <para>主线程内<b>就地执行</b>并返回已完成的 <see cref="TSyncCall"/>；否则投递到主线程消息泵。</para>
    /// </summary>
    public static IAsyncCall VCLInvoke(Action Proc, TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = pool ?? AsyncCalls.ThreadPool;
        TAsyncCallRuntime r = runtime ?? AsyncCalls.Runtime;

        if (r.WaitService.IsMainThread)
        {
            Proc();
            return new TSyncCall(0);
        }

        p.CheckDestroying();
        var Call = new TAsyncVclCallAnonymProc(Proc, r);
        p.SendVclSync(Call);
        return new TAsyncCall(Call, r);
    }

    /// <summary>
    /// 原文 3271-3290：<c>TAsyncCalls.VCLSync</c>（D2009+ 分支）。
    /// <para>
    /// 主线程内直接调用；否则经 <c>StaticSynchronize</c> 阻塞到主线程执行。
    /// 接缝：<c>StaticSynchronize</c> 由 <see cref="TAsyncCallRuntime.MainThread"/> 承接。
    /// </para>
    /// </summary>
    public static void VCLSync(Action Proc, TThreadPool pool = null, TAsyncCallRuntime runtime = null)
    {
        TThreadPool p = pool ?? AsyncCalls.ThreadPool;
        TAsyncCallRuntime r = runtime ?? AsyncCalls.Runtime;

        if (r.WaitService.IsMainThread)
        {
            Proc();
            return;
        }

        // 原文 3285-3288：
        //   ThreadPool.CheckDestroying;
        //   M.Code := @Exec; M.Data := @Proc; StaticSynchronize(TThreadMethod(M));
        // 接缝：StaticSynchronize（RTL 的 TThread.Synchronize）待 Classes.pas 移植后接入。
        p.CheckDestroying();
        throw new NotSupportedException(
            "接缝：StaticSynchronize（Classes.TThread.Synchronize）尚未移植（AsyncCalls.pas:3288）。");
    }

    /// <summary>
    /// 原文 3079-3091 + 326-3326：<c>TAsyncCalls.TAsyncVclCallAnonymProc</c> 的匿名过程派发。
    /// </summary>
    private sealed class TAsyncVclCallAnonymProc : TInternalAsyncCall
    {
        private readonly Action FProc;

        public TAsyncVclCallAnonymProc(Action AProc, TAsyncCallRuntime runtime) : base(runtime)
        {
            FProc = AProc;
            CreateEvent();
        }

        /// <summary>原文 3087-3091：<c>FProc(); Result := 0;</c></summary>
        protected override int ExecuteAsyncCall()
        {
            FProc();
            return 0;
        }
    }
}

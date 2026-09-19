using System;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

// ---------------------------------------------------------------------------
// 原文 903-1096：所有 TInternalAsyncCall 的具体派生类（按参数类型划分）。
// 每个类的 ExecuteAsyncCall 都是“转发给被捕获的委托”，语义 1:1。
// 构造函数对应原文的 `inherited Create`（后者建立手动重置事件）。
// ---------------------------------------------------------------------------

/// <summary>原文 903-911：<c>TAsyncCallArgObject</c>（Arg: TObject）</summary>
public sealed class TAsyncCallArgObject : TInternalAsyncCall
{
    private readonly TAsyncCallArgObjectProc FProc;
    private readonly object FArg;

    public TAsyncCallArgObject(TAsyncCallArgObjectProc AProc, object AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2452-2457：<c>Result := FProc(FArg)</c></summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 913-921：<c>TAsyncCallArgString</c></summary>
public sealed class TAsyncCallArgString : TInternalAsyncCall
{
    private readonly TAsyncCallArgStringProc FProc;
    private readonly string FArg;

    public TAsyncCallArgString(TAsyncCallArgStringProc AProc, string AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2483-2488</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 923-931：<c>TAsyncCallArgWideString</c></summary>
public sealed class TAsyncCallArgWideString : TInternalAsyncCall
{
    private readonly TAsyncCallArgWideStringProc FProc;
    private readonly string FArg;

    public TAsyncCallArgWideString(TAsyncCallArgWideStringProc AProc, string AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2513-2518</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 933-941：<c>TAsyncCallArgInterface</c></summary>
public sealed class TAsyncCallArgInterface : TInternalAsyncCall
{
    private readonly TAsyncCallArgInterfaceProc FProc;
    private readonly object FArg;

    public TAsyncCallArgInterface(TAsyncCallArgInterfaceProc AProc, object AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2543-2548</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>
/// 原文 943-951：<c>TAsyncCallArgExtended</c>。
/// <para>Delphi <c>Extended</c> 为 80 位（10 字节）；C# 无对应类型，取 <c>double</c>（64 位）—— 见报告差异登记。</para>
/// </summary>
public sealed class TAsyncCallArgExtended : TInternalAsyncCall
{
    private readonly TAsyncCallArgExtendedProc FProc;
    private readonly double FArg;

    public TAsyncCallArgExtended(TAsyncCallArgExtendedProc AProc, double AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2573-2578</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 953-961：<c>TAsyncCallArgVariant</c></summary>
public sealed class TAsyncCallArgVariant : TInternalAsyncCall
{
    private readonly TAsyncCallArgVariantProc FProc;
    private readonly object FArg;

    public TAsyncCallArgVariant(TAsyncCallArgVariantProc AProc, object AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2603-2608</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1001-1009：<c>TAsyncCallMethodArgObject</c></summary>
public sealed class TAsyncCallMethodArgObject : TInternalAsyncCall
{
    private readonly TAsyncCallArgObjectMethod FProc;
    private readonly object FArg;

    public TAsyncCallMethodArgObject(TAsyncCallArgObjectMethod AProc, object AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2467-2472</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1011-1019：<c>TAsyncCallMethodArgString</c></summary>
public sealed class TAsyncCallMethodArgString : TInternalAsyncCall
{
    private readonly TAsyncCallArgStringMethod FProc;
    private readonly string FArg;

    public TAsyncCallMethodArgString(TAsyncCallArgStringMethod AProc, string AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2498-2503</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1021-1029：<c>TAsyncCallMethodArgWideString</c></summary>
public sealed class TAsyncCallMethodArgWideString : TInternalAsyncCall
{
    private readonly TAsyncCallArgWideStringMethod FProc;
    private readonly string FArg;

    public TAsyncCallMethodArgWideString(TAsyncCallArgWideStringMethod AProc, string AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2528-2533</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1031-1039：<c>TAsyncCallMethodArgInterface</c></summary>
public sealed class TAsyncCallMethodArgInterface : TInternalAsyncCall
{
    private readonly TAsyncCallArgInterfaceMethod FProc;
    private readonly object FArg;

    public TAsyncCallMethodArgInterface(TAsyncCallArgInterfaceMethod AProc, object AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2558-2563</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1041-1049：<c>TAsyncCallMethodArgExtended</c></summary>
public sealed class TAsyncCallMethodArgExtended : TInternalAsyncCall
{
    private readonly TAsyncCallArgExtendedMethod FProc;
    private readonly double FArg;

    public TAsyncCallMethodArgExtended(TAsyncCallArgExtendedMethod AProc, double AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2588-2593</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1051-1059：<c>TAsyncCallMethodArgVariant</c></summary>
public sealed class TAsyncCallMethodArgVariant : TInternalAsyncCall
{
    private readonly TAsyncCallArgVariantMethod FProc;
    private readonly object FArg;

    public TAsyncCallMethodArgVariant(TAsyncCallArgVariantMethod AProc, object AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2618-2623</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>
/// 原文 1063-1071：<c>TAsyncCallArgRecord</c>（<c>var Arg</c> 传记录指针）。
/// <para>托管侧记录以 <see cref="IntPtr"/> 承载（对应原文的 <c>Pointer</c>）；真实语义由调用方保证。</para>
/// </summary>
public sealed class TAsyncCallArgRecord : TInternalAsyncCall
{
    private readonly TAsyncCallArgRecordProc FProc;
    private readonly IntPtr FArg;

    public TAsyncCallArgRecord(TAsyncCallArgRecordProc AProc, IntPtr AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2422-2427</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

/// <summary>原文 1073-1081：<c>TAsyncCallMethodArgRecord</c></summary>
public sealed class TAsyncCallMethodArgRecord : TInternalAsyncCall
{
    private readonly TAsyncCallArgRecordMethod FProc;
    private readonly IntPtr FArg;

    public TAsyncCallMethodArgRecord(TAsyncCallArgRecordMethod AProc, IntPtr AArg, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FArg = AArg;
        CreateEvent();
    }

    /// <summary>原文 2437-2442</summary>
    protected override int ExecuteAsyncCall() => FProc(FArg);
}

// ---------------------------------------------------------------------------
// 原文 965-997：局部函数（SUPPORT_LOCAL_FUNCTIONS）三兄弟 —— Delphi 7 下活跃。
// ---------------------------------------------------------------------------

/// <summary>
/// 原文 966-974：<c>TAsyncCallLocalProc</c>。
/// <para>
/// 原文的 <c>FBasePointer</c> 是 Delphi 局部函数的栈帧（EBP）复刻手段
/// （asm 见 2635-2643：<c>push edx; call ecx; pop ecx</c>，EAX=0 作无参）。
/// 托管侧闭包自动捕获环境，故 <see cref="ExecuteAsyncCall"/> 即 <c>FProc()</c>；
/// <c>ABasePointer</c> 仅保留形参以对照原文调用点（登记为差异）。
/// </para>
/// </summary>
public sealed class TAsyncCallLocalProc : TInternalAsyncCall
{
    private readonly Func<int> FProc;
    private readonly IntPtr FBasePointer;

    public TAsyncCallLocalProc(Func<int> AProc, IntPtr ABasePointer, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FBasePointer = ABasePointer;
        CreateEvent();
    }

    /// <summary>原文的栈帧基址（仅为对照保留）。</summary>
    public IntPtr BasePointer => FBasePointer;

    /// <summary>原文 2635-2643</summary>
    protected override int ExecuteAsyncCall() => FProc();
}

/// <summary>原文 976-985：<c>TAsyncCallLocalProcEx</c>（额外带一个 INT_PTR 参数）</summary>
public sealed class TAsyncCallLocalProcEx : TInternalAsyncCall
{
    private readonly Func<IntPtr, IntPtr> FProc;
    private readonly IntPtr FBasePointer;
    private readonly IntPtr FParam;

    public TAsyncCallLocalProcEx(Func<IntPtr, IntPtr> AProc, IntPtr AParam, IntPtr ABasePointer, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FBasePointer = ABasePointer;
        FParam = AParam;
        CreateEvent();
    }

    /// <summary>原文 2655-2663</summary>
    protected override int ExecuteAsyncCall() => (int)FProc(FParam);
}

/// <summary>
/// 原文 987-996：<c>TAsyncVclCallLocalProc</c> —— 必须回到主线程执行的局部函数。
/// <para>由 <c>ThreadPool.SendVclSync</c> 派发（原文 2748），因此 <see cref="ExecuteAsyncCall"/> 自身只负责调用。</para>
/// </summary>
public sealed class TAsyncVclCallLocalProc : TInternalAsyncCall
{
    private readonly Func<IntPtr, IntPtr> FProc;
    private readonly IntPtr FBasePointer;
    private readonly IntPtr FParam;

    public TAsyncVclCallLocalProc(Func<IntPtr, IntPtr> AProc, IntPtr AParam, IntPtr ABasePointer, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FBasePointer = ABasePointer;
        FParam = AParam;
        CreateEvent();
    }

    /// <summary>原文 2677-2685</summary>
    protected override int ExecuteAsyncCall() => (int)FProc(FParam);
}

/// <summary>
/// 原文 1084-1095 + 2231-2409：<c>TAsyncCallArrayOfConst</c> ——
/// <c>array of const</c> 变参异步调用。
/// <para>
/// 可移植的判别式逻辑在 <see cref="TVarRecMarshal"/>（已单测）；
/// 真正的 cdecl 调用经 <see cref="IVarRecInvoker"/> 接缝。
/// </para>
/// </summary>
public sealed class TAsyncCallArrayOfConst : TInternalAsyncCall
{
    private readonly nint FProc;
    private readonly object FMethodData;
    private readonly TVarRec[] FArgs;

    /// <summary>原文 2231-2240：<c>Create(AProc: Pointer; const AArgs: array of const)</c></summary>
    public TAsyncCallArrayOfConst(nint AProc, TVarRec[] AArgs, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FMethodData = null;
        FArgs = new TVarRec[AArgs.Length];
        for (int I = 0; I < AArgs.Length; I++)
            TVarRecMarshal.CopyVarRec(in AArgs[I], out FArgs[I]);
        CreateEvent();
    }

    /// <summary>
    /// 原文 2242-2256：<c>Create(AProc: Pointer; MethodData: TObject; const AArgs: array of const)</c>
    /// —— 在实参最前面插入 <c>Self</c>（<c>vtObject</c>）。
    /// </summary>
    public TAsyncCallArrayOfConst(nint AProc, object MethodData, TVarRec[] AArgs, TAsyncCallRuntime runtime = null)
        : base(runtime)
    {
        FProc = AProc;
        FMethodData = MethodData;
        FArgs = new TVarRec[1 + AArgs.Length];

        // insert "Self"
        FArgs[0].VType = TVarRecType.vtObject;
        FArgs[0].VObject = MethodData;

        for (int I = 0; I < AArgs.Length; I++)
            TVarRecMarshal.CopyVarRec(in AArgs[I], out FArgs[I + 1]);
        CreateEvent();
    }

    /// <summary>实参副本（原文 <c>FArgs</c>；含可能的 Self 槽）。</summary>
    public TVarRec[] Args => FArgs;

    /// <summary>原文 2258-2282：<c>destructor Destroy</c> —— 逐个释放负载。</summary>
    public override void Destroy()
    {
        for (int I = 0; I < FArgs.Length; I++)
            TVarRecMarshal.ReleaseVarRec(ref FArgs[I]);
        base.Destroy();
    }

    /// <summary>
    /// 原文 2315-2409：<c>ExecuteAsyncCall</c>。
    /// <para>
    /// 原文先构造零填充栈缓冲（<c>Length(FArgs) * 4 + $40</c>），再自右向左压栈，
    /// 最后 <c>Result := FProc</c> 直接跳入目标函数，返回后 <c>add esp, [ByteCount]</c>。
    /// 托管侧仅保留<b>可测的分类与字节记账</b>（<see cref="TVarRecMarshal.BuildPushSequence"/> /
    /// <see cref="TVarRecMarshal.ComputeStackByteCount"/>），调用本体交由
    /// <see cref="IVarRecInvoker"/>。
    /// </para>
    /// </summary>
    protected override int ExecuteAsyncCall()
    {
        // 先按原文顺序校验/分类（未支持类型在此抛 UnknownVarRecType，与原文等位）
        _ = TVarRecMarshal.BuildPushSequence(FArgs);
        _ = TVarRecMarshal.ComputeStackByteCount(FArgs);
        return Runtime.VarRecInvoker.Invoke(FProc, FMethodData, FArgs);
    }
}

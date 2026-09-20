using System;
using System.Threading;

namespace GXX.Client.DxComponent;

// =====================================================================================
// AsphyreTimer.pas（335 行，源：Source\Client-HGE\DxComponent\AsphyreTimer.pas）1:1 移植。
//
// 源单元行号范围（本文件逐条对应）：
//   * 1-43      单元头 / Asphyre 版权与变更日志（原文注释，摘要保留在下方）
//   * 44-49     interface uses（Windows/Classes/Forms/Math/MMSystem）
//   * 51-52     TPerformancePrecision = (ppLow, ppHigh)
//   * 55-113    TAsphyreTimer 声明（私有字段 57-80 / 私有方法 81-88 / public 89-100 /
//               published 101-112）
//   * 116-117   单元级 var Timer: TAsphyreTimer = nil
//   * 123-125   const FixedHigh = $100000; DeltaLimit = 32 * FixedHigh
//   * 129-156   constructor Create
//   * 160-165   SetSpeed
//   * 169-174   SetMaxFPS
//   * 178-188   GetDelta / GetLatency
//   * 192-208   RetreiveLatency（**原文拼写错误 Retreive，逐字保留**）
//   * 212-262   DoTimerEvent（7 步）
//   * 264-271   Start
//   * 273-280   Stop
//   * 282-290   OnIdle
//   * 292-296   AppIdle
//   * 300-314   Process
//   * 318-323   Reset
//   * 326-331   initialization / finalization（Timer := TAsphyreTimer.Create / Timer.Free）
// =====================================================================================
//
// -------------------------------------------------------------------------------------
// 托管侧接缝与偏差（逐条登记）：
//
//   1. 原文 `Real`（Delphi）= Double，故 `FSpeed`/`Delta`/`Latency` 均为 `double`。
//
//   2. 原文 `Integer` 为 32 位有符号，且工程默认 `$Q-`（溢出检查关）→ 托管侧凡
//      「Integer 域内可能溢出的整数运算」一律显式 `unchecked(...)` 以复现截断语义：
//        * `RetreiveLatency` 低精度分支 `(CurTime - PrevTime) * FixedHigh`
//          —— `CurTime - PrevTime` 是 **Cardinal**（mod 2^32 回绕），
//             再 `Cardinal * Integer` 在 Delphi 里提升为 **Int64**，最后截断回 Integer。
//        * `SampleLatency := SampleLatency + LatencyFP + (WaitAmount * FixedHigh)`
//        * `Inc(FixedDelta, DeltaFP)`
//
//   3. 原文 `Round(Float)` = 银行家舍入（就近偶数）；C# `Math.Round(double)` 默认
//      `MidpointRounding.ToEven` —— 一致，故直接使用（用于 SpeedLatcy / MinLatency）。
//
//   4. `GetTickCount`（Windows）→ `GetTickCountFn`，默认转发到本树既有的
//      `DxTickCount.MyGetTickCount()`（DxControls.cs），保持单一全局注入点。
//
//   5. `QueryPerformanceFrequency` / `QueryPerformanceCounter` → `QueryPerformanceFrequencyFn`
//      / `QueryPerformanceCounterFn`，默认走 `System.Diagnostics.Stopwatch`
//      （`Stopwatch.IsHighResolution` 为假时第一者返回 false → 退化为 ppLow，与原文
//      `if QueryPerformanceFrequency(HighFreq) then ppHigh` 同构）。
//
//   6. `SleepEx(ms, True)` → `SleepExFn`，默认 `Thread.Sleep(ms)`
//      （alertable 等待且无 APC 队列时与 Sleep 等价）。测试注入记录器以免真睡。
//
//   7. `timeBeginPeriod(1)`（MMSystem）→ `TimeBeginPeriodFn`，**默认 no-op**。
//      .NET 无对应公开 API；该调用只影响系统定时器分辨率，不影响本单元任何计算。
//      （原文 148 行；此处是唯一的"默认不执行原文副作用"的偏差。）
//
//   8. `Application.OnIdle := AppIdle`（原文 145 / 267 / 278）→ `SetApplicationOnIdleFn`，
//      **默认 no-op**（无头 testhost 安全：绝不接线、绝不会被消息循环回调而挂死）。
//      真实客户端宿主需自行调用 `AttachWinFormsIdle()`（内部用 `Application.Idle` 静态事件）。
//      注意语义损失：VCL 的 `OnIdle(Sender; var Done)` 可写回 `Done`，WinForms 的
//      `Application.Idle` 无此形参 —— 托管侧每次回调新建 `BoolRef(true)` 占位，
//      `FIdleDone` 的写回语义在 WinForms 侧不可表达（已登记）。
//
//   9. 原文 `initialization Timer := TAsphyreTimer.Create();` / `finalization Timer.Free();`
//      → `AsphyreTimerUnit.Timer`（惰性 getter 承载 initialization）/ `FinalizeUnit()`。
//      偏差：托管侧无"单元卸载"，`Timer` 在 `FinalizeUnit()` 之后再次访问会重新初始化。
//
//  10. 可见性：原文 `SetSpeed`/`SetMaxFPS`/`GetDelta`/`GetLatency`/`RetreiveLatency`/
//      `DoTimerEvent`/`AppIdle` 均为 private。托管侧**未改可见性**：
//      前者经属性 setter 可达，`RetreiveLatency`/`DoTimerEvent` 经 public 的
//      `OnIdle()` / `Reset()` 可达，`AppIdle` 经接缝 `SetApplicationOnIdleFn` 取得委托后可达。
//
//  11. 原文怪癖（逐字保留）：
//      * **`Start` 把 `Application.OnIdle` 置 nil，`Stop` 反而装上 `AppIdle`**
//        （原文 264-280）—— 与直觉相反，逐字保留并加注释。
//      * `Process` 里 `FixedDelta := FixedDelta and (FixedHigh - 1)` 在
//        `if Assigned(FOnProcess)` **之外**（原文 309-313）→ 无论有无回调都会取低 20 位。
//      * `DoTimerEvent` 第 (3) 步 `if WaitAmount < 0 then WaitAmount := 10;`
//        在 `LatencyFP < MinLatency` 分支内，而该分支下 `MinLatency - LatencyFP > 0`
//        故 `WaitAmount >= 0` —— 该保底分支**只在 MinLatency=0 且 LatencyFP<0 时**可达。
//      * **`Reset` 丢弃 `RetreiveLatency` 的返回值**（原文 322 只把函数当过程调用）：
//        `LatencyFP` 在 `Reset` 之后**不变**（仍是上一次的值，构造后则为 0），
//        被"消费"的只是 `PrevTime`/`PrevTime64` 这个时间基准。逐字保留并加注释。
//      * `DeltaFP` 有 `DeltaLimit = 32 * FixedHigh` 上限（原文 238）：60fps 下延迟超过
//        约 533ms 就被夹到 32 帧 —— 写测试时不可忽略这一步。
// =====================================================================================

// -------------------------------------------------------------------------------------
// AsphyreTimer.pas 51-52
// -------------------------------------------------------------------------------------

/// <summary>AsphyreTimer.pas 52 <c>TPerformancePrecision = (ppLow, ppHigh)</c>。</summary>
public enum TPerformancePrecision { ppLow, ppHigh }

// -------------------------------------------------------------------------------------
// 接缝委托（原文 Windows / MMSystem / Forms 的等价物）
// -------------------------------------------------------------------------------------

/// <summary>`QueryPerformanceFrequency(var Frequency: Int64): BOOL`。</summary>
public delegate bool TQueryPerformanceFrequency(out long frequency);

/// <summary>`QueryPerformanceCounter(var Counter: Int64): BOOL`（原文忽略返回值）。</summary>
public delegate bool TQueryPerformanceCounter(out long counter);

/// <summary>`TApplicationEvents.OnIdle(Sender: TObject; var Done: Boolean)`（原文 Forms 单元）。</summary>
public delegate void TIdleEvent(object sender, BoolRef done);

// -------------------------------------------------------------------------------------
// AsphyreTimer.pas 55-113：TAsphyreTimer
// -------------------------------------------------------------------------------------

/// <summary>
/// AsphyreTimer.pas 55-113 / 129-323 <c>TAsphyreTimer</c>
/// （Asphyre 3.1 单核 Idle 定时器，12:20 定点：`FixedHigh = $100000`）。
/// </summary>
public class TAsphyreTimer
{
    // ---- 原文 123-125 const ----
    /// <summary>原文 124 `FixedHigh = $100000`（12:20 定点的 1.0）。</summary>
    public const int FixedHigh = 0x100000;

    /// <summary>原文 125 `DeltaLimit = 32 * FixedHigh`。</summary>
    public const int DeltaLimit = 32 * FixedHigh;

    // ===============================================================================
    // 脚本化接缝（原文 Windows / MMSystem / Forms 全局函数的托管落点）
    // ===============================================================================

    /// <summary>`QueryPerformanceFrequency`（原文 137）。</summary>
    public static TQueryPerformanceFrequency QueryPerformanceFrequencyFn = (out long f) =>
    {
        if (!System.Diagnostics.Stopwatch.IsHighResolution) { f = 0; return false; }
        f = System.Diagnostics.Stopwatch.Frequency;
        return true;
    };

    /// <summary>`QueryPerformanceCounter`（原文 140 / 199）。</summary>
    public static TQueryPerformanceCounter QueryPerformanceCounterFn = (out long c) =>
    {
        c = System.Diagnostics.Stopwatch.GetTimestamp();
        return true;
    };

    /// <summary>`GetTickCount`（原文 140 / 204）。默认转发到既有 `DxTickCount`。</summary>
    public static Func<uint> GetTickCountFn = () => DxTickCount.MyGetTickCount();

    /// <summary>`SleepEx(Milliseconds, True)`（原文 223 / 232）。</summary>
    public static Action<int> SleepExFn = ms => Thread.Sleep(ms);

    /// <summary>`timeBeginPeriod(1)`（原文 148）。默认 no-op，见文件头第 7 条。</summary>
    public static Action<int> TimeBeginPeriodFn = _ => { };

    /// <summary>
    /// `Application.OnIdle := Handler`（原文 145 / 267 / 278）。默认 no-op，
    /// 见文件头第 8 条；宿主可调用 <see cref="AttachWinFormsIdle"/> 接真实消息循环。
    /// </summary>
    public static Action<TIdleEvent> SetApplicationOnIdleFn = _ => { };

    // ===============================================================================
    // 原文 57-80 私有字段
    // ===============================================================================

    private int FMaxFPS;                    // 57
    private double FSpeed;                  // 58
    private bool FEnabled;                  // 59
    private TNotifyEvent FOnTimer;          // 60
    private int FFrameRate;                 // 62
    private TPerformancePrecision FPrecision;   // 64
    private uint PrevTime;                  // 65
    private long PrevTime64;                // 66
    private TNotifyEvent FOnProcess;        // 67
    private bool Processed;                 // 68
    private int LatencyFP;                  // 70
    private int DeltaFP;                    // 71
    private long HighFreq;                  // 72
    private int MinLatency;                 // 73
    private int SpeedLatcy;                 // 74
    private int FixedDelta;                 // 75
    private int SampleLatency;              // 77
    private int SampleIndex;                // 78
    private bool FIdleDone;                 // 80

    // ===============================================================================
    // 原文 89-100 public / 101-112 published
    // ===============================================================================

    /// <summary>原文 90 `property Delta: Real read GetDelta`。</summary>
    public double Delta => GetDelta();

    /// <summary>原文 91 `property Latency: Real read GetLatency`。</summary>
    public double Latency => GetLatency();

    /// <summary>原文 92 `property FrameRate: Integer read FFrameRate`。</summary>
    public int FrameRate => FFrameRate;

    /// <summary>原文 103 `property Speed: Real read FSpeed write SetSpeed`。</summary>
    public double Speed
    {
        get => FSpeed;
        set => SetSpeed(value);
    }

    /// <summary>原文 105 `property MaxFPS: Integer read FMaxFPS write SetMaxFPS`。</summary>
    public int MaxFPS
    {
        get => FMaxFPS;
        set => SetMaxFPS(value);
    }

    /// <summary>原文 107 `property Enabled: Boolean read FEnabled write FEnabled`（裸字段读写）。</summary>
    public bool Enabled
    {
        get => FEnabled;
        set => FEnabled = value;
    }

    /// <summary>原文 109 `property Precision: TPerformancePrecision read FPrecision`（只读）。</summary>
    public TPerformancePrecision Precision => FPrecision;

    /// <summary>原文 110 `property IdleDone: Boolean read FIdleDone write FIdleDone`。</summary>
    public bool IdleDone
    {
        get => FIdleDone;
        set => FIdleDone = value;
    }

    /// <summary>原文 111 `property OnTimer: TNotifyEvent read FOnTimer write FOnTimer`。</summary>
    public TNotifyEvent OnTimer
    {
        get => FOnTimer;
        set => FOnTimer = value;
    }

    /// <summary>原文 112 `property OnProcess: TNotifyEvent read FOnProcess write FOnProcess`。</summary>
    public TNotifyEvent OnProcess
    {
        get => FOnProcess;
        set => FOnProcess = value;
    }

    // ===============================================================================
    // 原文 129-156 constructor
    // ===============================================================================

    /// <summary>原文 129-156 <c>TAsphyreTimer.Create()</c> 逐项。</summary>
    public TAsphyreTimer()
    {
        // 原文 133-134：走属性 setter（会算出 SpeedLatcy / MinLatency）
        Speed = 60.0;
        MaxFPS = 100;

        // 原文 136-137
        FPrecision = TPerformancePrecision.ppLow;
        if (QueryPerformanceFrequencyFn(out long highFreq))
        {
            HighFreq = highFreq;
            FPrecision = TPerformancePrecision.ppHigh;
        }

        // 原文 139-140
        if (FPrecision == TPerformancePrecision.ppHigh)
        {
            QueryPerformanceCounterFn(out PrevTime64);
        }
        else
        {
            PrevTime = GetTickCountFn();
        }

        // 原文 143-145：Application.OnIdle := AppIdle
        SetApplicationOnIdleFn(AppIdle);

        // 原文 148
        TimeBeginPeriodFn(1);

        // 原文 150-155
        FixedDelta = 0;
        FFrameRate = 0;
        SampleLatency = 0;
        SampleIndex = 0;
        Processed = false;
        FIdleDone = true;

        // 注意：原文未给 FEnabled 赋值 → 构造后 Enabled = False（原文 59 的 Boolean 默认 False）。
    }

    // ===============================================================================
    // 原文 160-174 setter
    // ===============================================================================

    /// <summary>
    /// 原文 160-165 `SetSpeed`：先写入，再 `if FSpeed &lt; 1.0 then FSpeed := 1.0`
    /// （**写的是字段**，故 Speed 的读回值也被夹到 1.0），最后
    /// `SpeedLatcy := Round(FixedHigh * 1000.0 / FSpeed)`。
    /// </summary>
    private void SetSpeed(double value)
    {
        FSpeed = value;
        if (FSpeed < 1.0) FSpeed = 1.0;
        SpeedLatcy = (int)Math.Round(FixedHigh * 1000.0 / FSpeed);
    }

    /// <summary>原文 169-174 `SetMaxFPS`：同样先夹 `&lt; 1 → 1`，再 `MinLatency := Round(FixedHigh * 1000.0 / FMaxFPS)`。</summary>
    private void SetMaxFPS(int value)
    {
        FMaxFPS = value;
        if (FMaxFPS < 1) FMaxFPS = 1;
        MinLatency = (int)Math.Round(FixedHigh * 1000.0 / FMaxFPS);
    }

    // ===============================================================================
    // 原文 178-188 getter
    // ===============================================================================

    /// <summary>原文 178-181 `GetDelta`：`DeltaFP / FixedHigh`（**浮点除法**）。</summary>
    private double GetDelta() => (double)DeltaFP / FixedHigh;

    /// <summary>原文 183-188 `GetLatency`：`LatencyFP / FixedHigh`（**浮点除法**）。</summary>
    private double GetLatency() => (double)LatencyFP / FixedHigh;

    // ===============================================================================
    // 原文 192-208 RetreiveLatency（原文拼写如此，逐字保留）
    // ===============================================================================

    /// <summary>
    /// 原文 192-208 <c>RetreiveLatency</c>（**原文拼写错误：Retreive**，逐字保留）。
    ///
    /// 高精度：`((CurTime64 - PrevTime64) * FixedHigh * 1000) div HighFreq`（Int64 全程）；
    /// 低精度：`(CurTime - PrevTime) * FixedHigh` —— `CurTime - PrevTime` 是 **Cardinal**
    /// （uint32 回绕），再与 Integer 常量相乘时 Delphi 提升为 Int64，最后截断回 Integer。
    /// 两条分支都会把 `PrevTime*` 推进到当前值。
    /// </summary>
    private int RetreiveLatency()
    {
        if (FPrecision == TPerformancePrecision.ppHigh)
        {
            QueryPerformanceCounterFn(out long curTime64);
            long result = ((curTime64 - PrevTime64) * FixedHigh * 1000) / HighFreq;
            PrevTime64 = curTime64;
            return unchecked((int)result);
        }
        else
        {
            uint curTime = GetTickCountFn();
            // (uint)(curTime - PrevTime) 复现 Cardinal 回绕；提升 Int64 后再截断回 Integer
            long result = (long)(uint)(curTime - PrevTime) * FixedHigh;
            PrevTime = curTime;
            return unchecked((int)result);
        }
    }

    // ===============================================================================
    // 原文 212-262 DoTimerEvent（7 步，顺序不可换）
    // ===============================================================================

    /// <summary>
    /// 原文 212-262 <c>DoTimerEvent</c>：
    /// ① 取延迟；② `not Enabled` → `SleepEx(5, True)` 并 Exit；
    /// ③ `LatencyFP &lt; MinLatency` → 等待 `(MinLatency - LatencyFP) div FixedHigh` 毫秒
    ///    （`&lt; 0` 时保底 10）；④ `DeltaFP := Int64(LatencyFP) * FixedHigh div SpeedLatcy`，
    ///    并夹到 `DeltaLimit`；⑤ 每 `SampleMax` 次算一次 FrameRate；
    /// ⑥ `Processed` 时 `Inc(FixedDelta, DeltaFP)` 并清 `Processed`；⑦ `OnTimer`。
    /// </summary>
    private void DoTimerEvent()
    {
        int waitAmount;

        // (1) 原文 218
        LatencyFP = RetreiveLatency();

        // (2) 原文 221-225：禁用时小睡 5ms 并直接返回（**不推进 DeltaFP / 不回调**）
        if (!FEnabled)
        {
            SleepExFn(5);
            return;
        }

        // (3) 原文 228-233
        if (LatencyFP < MinLatency)
        {
            waitAmount = (MinLatency - LatencyFP) / FixedHigh;
            if (waitAmount < 0) waitAmount = 10;
            SleepExFn(waitAmount);
        }
        else
        {
            waitAmount = 0;
        }

        // (4) 原文 236-238
        DeltaFP = unchecked((int)(((long)LatencyFP * FixedHigh) / SpeedLatcy));
        if (DeltaFP > DeltaLimit) DeltaFP = DeltaLimit;

        // (5) 原文 241-251
        SampleLatency = unchecked(SampleLatency + LatencyFP + waitAmount * FixedHigh);
        int sampleMax = LatencyFP <= 0 ? 4 : unchecked((int)((long)FixedHigh * 1000 / LatencyFP));

        SampleIndex = unchecked(SampleIndex + 1);
        if (SampleIndex >= sampleMax)
        {
            FFrameRate = unchecked((int)((long)SampleIndex * FixedHigh * 1000 / SampleLatency));
            SampleLatency = 0;
            SampleIndex = 0;
        }

        // (6) 原文 254-258
        if (Processed)
        {
            FixedDelta = unchecked(FixedDelta + DeltaFP);
            Processed = false;
        }

        // (7) 原文 261
        FOnTimer?.Invoke(this);
    }

    // ===============================================================================
    // 原文 264-296 Start / Stop / OnIdle / AppIdle
    // ===============================================================================

    /// <summary>
    /// 原文 264-271 <c>Start</c>：`Application.OnIdle := nil;`
    /// **原文如此 —— Start 卸掉 Idle 钩子（与直觉相反），Stop 反而装上 AppIdle**（见文件头第 11 条）。
    /// </summary>
    public void Start() => SetApplicationOnIdleFn(null);

    /// <summary>原文 273-280 <c>Stop</c>：`Application.OnIdle := AppIdle;`（见 <see cref="Start"/> 的怪癖说明）。</summary>
    public void Stop() => SetApplicationOnIdleFn(AppIdle);

    /// <summary>原文 282-290 <c>OnIdle</c>：仅 `DoTimerEvent`（原文残留的 try/finally 注释照抄留档）。</summary>
    public void OnIdle()
    {
        DoTimerEvent();
        // 原文 285-288（注释掉的 try/finally）照抄留档：
        //   {try
        //
        //   finally
        //   end;  }
    }

    /// <summary>
    /// 原文 292-296 私有 <c>AppIdle</c>：`Done := FIdleDone; DoTimerEvent;`
    /// （先写 Done 再跑事件 —— 顺序不可换）。托管侧保持 private，经
    /// <see cref="SetApplicationOnIdleFn"/> 取得的委托可达（见文件头第 10 条）。
    /// </summary>
    private void AppIdle(object sender, BoolRef done)
    {
        done.Value = FIdleDone;
        DoTimerEvent();
    }

    // ===============================================================================
    // 原文 300-314 Process
    // ===============================================================================

    /// <summary>
    /// 原文 300-314 <c>Process</c>：
    /// `Processed := True`；`Amount := FixedDelta div FixedHigh`；`Amount &lt; 1` → Exit；
    /// 有 `OnProcess` 时调用 `Amount` 次；**随后无条件** `FixedDelta := FixedDelta and (FixedHigh - 1)`
    /// （原文 313 在 `if Assigned` 之外 —— 见文件头第 11 条）。
    /// </summary>
    public void Process()
    {
        Processed = true;

        int amount = FixedDelta / FixedHigh;
        if (amount < 1) return;

        if (FOnProcess != null)
        {
            for (int i = 1; i <= amount; i++)
                FOnProcess(this);
        }

        FixedDelta = FixedDelta & (FixedHigh - 1);
    }

    // ===============================================================================
    // 原文 318-323 Reset
    // ===============================================================================

    /// <summary>
    /// 原文 318-323 <c>Reset</c>：`FixedDelta := 0; DeltaFP := 0; RetreiveLatency();`
    /// —— **返回值被丢弃**（原文把函数当过程调用），故 `Latency`/`LatencyFP` 在 `Reset`
    /// 之后**不变**；被消费的只是时间基准 `PrevTime`/`PrevTime64`（见文件头第 11 条）。
    /// </summary>
    public void Reset()
    {
        FixedDelta = 0;
        DeltaFP = 0;
        RetreiveLatency();
    }

    // ===============================================================================
    // 原文 145 / 267 / 278 的 Application.OnIdle 真实接线（可选）
    // ===============================================================================

    /// <summary>当前经 <see cref="AttachWinFormsIdle"/> 接上的桥（null 表示未接线）。</summary>
    private static EventHandler _winFormsIdleBridge;
    private static TIdleEvent _winFormsIdleHandler;

    /// <summary>
    /// 把 <see cref="SetApplicationOnIdleFn"/> 接到 WinForms 的静态事件 `Application.Idle`。
    /// **默认不接**（无头 testhost 安全，见文件头第 8 条）；真实客户端宿主在启动时调用一次。
    /// 语义损失：`Application.Idle(sender, EventArgs)` 没有 `var Done` 形参，故每次回调
    /// 传入新建的 `BoolRef(true)` 占位。
    /// </summary>
    public static void AttachWinFormsIdle()
    {
        SetApplicationOnIdleFn = handler =>
        {
            if (_winFormsIdleBridge != null)
            {
                System.Windows.Forms.Application.Idle -= _winFormsIdleBridge;
                _winFormsIdleBridge = null;
            }
            _winFormsIdleHandler = handler;
            if (handler == null) return;
            _winFormsIdleBridge = (sender, e) => _winFormsIdleHandler(sender, new BoolRef(true));
            System.Windows.Forms.Application.Idle += _winFormsIdleBridge;
        };
    }

    /// <summary>撤掉 <see cref="AttachWinFormsIdle"/> 的接线并恢复默认 no-op。</summary>
    public static void DetachWinFormsIdle()
    {
        SetApplicationOnIdleFn(null);
        SetApplicationOnIdleFn = _ => { };
    }
}

// -------------------------------------------------------------------------------------
// AsphyreTimer.pas 116-117 / 326-331：单元级 var Timer + initialization / finalization
// -------------------------------------------------------------------------------------

/// <summary>
/// AsphyreTimer.pas 116-117 的单元级全局 <c>Timer: TAsphyreTimer</c>
/// 与 326-331 的 initialization / finalization 段托管落点。
///
/// 托管侧无"单元加载/卸载"概念，故：
///   * initialization 段（327 `Timer := TAsphyreTimer.Create();`）→ <see cref="Timer"/> 的惰性 getter；
///   * finalization 段（331 `Timer.Free();`）→ <see cref="FinalizeUnit"/> 丢弃实例。
/// 再次访问 <see cref="Timer"/> 会重新初始化（**托管侧偏差**，原文的 initialization 只跑一次）。
/// 这样 <see cref="FinalizeUnit"/> 可重复调用且不污染后续使用者，无需依赖调用顺序。
/// </summary>
public static class AsphyreTimerUnit
{
    private static TAsphyreTimer _timer;

    /// <summary>
    /// 原文 117 `var Timer: TAsphyreTimer = nil;` + 327 `Timer := TAsphyreTimer.Create();`
    /// （首次访问即"单元加载"）。
    /// </summary>
    public static TAsphyreTimer Timer => _timer ??= new TAsphyreTimer();

    /// <summary>原文 331 `Timer.Free();`（finalization 段）：丢弃当前实例。可重复调用。</summary>
    public static void FinalizeUnit() => _timer = null;
}

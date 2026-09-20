using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// AsphyreTimer.pas（335 行）的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/AsphyreTimer.cs）：
//   * 构造默认值与两条精度路径（ppLow / ppHigh）
//   * Speed / MaxFPS 的夹取（< 1 → 1）与 SpeedLatcy / MinLatency 的银行家舍入
//   * RetreiveLatency 两条分支（含 Cardinal 回绕 + 提升 Int64 后截断回 Integer 的怪癖）
//   * DoTimerEvent 七步：禁用小睡 5ms / 等待量 / DeltaFP 与 DeltaLimit / FrameRate 采样
//     / Processed 累积 / OnTimer
//   * Process：Amount = FixedDelta div FixedHigh、调用次数、**无条件**取低 20 位
//   * Start/Stop 的**反直觉**原文接线（Start 卸钩、Stop 装钩）
//   * AppIdle 先写 Done 再跑事件的顺序
//   * AsphyreTimerUnit 的单元级 Timer（initialization / finalization）
//
// 所有 `原文 NNN` 行号指 Source\Client-HGE\DxComponent\AsphyreTimer.pas。
// 全部时间/UI 副作用经静态接缝注入，**不真睡、不接消息循环**（无头安全）。
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlAsphyreTimerTests
{
    // ===============================================================================
    // 接缝夹具：可控时钟 + 副作用记录器 + 静态字段的保存/恢复
    // ===============================================================================

    private sealed class Clock
    {
        public bool HighPrecision;
        public long PerfFreq = 1_000_000;
        public long PerfCounter;
        public uint Tick;

        public readonly List<int> Sleeps = new();
        public readonly List<int> TimePeriods = new();
        public readonly List<TIdleEvent> IdleSets = new();

        /// <summary>最近一次 `Application.OnIdle := X` 传进来的 X（nil 时为 null）。</summary>
        public TIdleEvent IdleHandler;
    }

    private sealed class Hook : IDisposable
    {
        private readonly TQueryPerformanceFrequency _qpf = TAsphyreTimer.QueryPerformanceFrequencyFn;
        private readonly TQueryPerformanceCounter _qpc = TAsphyreTimer.QueryPerformanceCounterFn;
        private readonly Func<uint> _tick = TAsphyreTimer.GetTickCountFn;
        private readonly Action<int> _sleep = TAsphyreTimer.SleepExFn;
        private readonly Action<int> _period = TAsphyreTimer.TimeBeginPeriodFn;
        private readonly Action<TIdleEvent> _idle = TAsphyreTimer.SetApplicationOnIdleFn;

        public Hook(Clock c)
        {
            TAsphyreTimer.QueryPerformanceFrequencyFn = (out long f) =>
            {
                f = c.HighPrecision ? c.PerfFreq : 0;
                return c.HighPrecision;
            };
            TAsphyreTimer.QueryPerformanceCounterFn = (out long v) => { v = c.PerfCounter; return true; };
            TAsphyreTimer.GetTickCountFn = () => c.Tick;
            TAsphyreTimer.SleepExFn = ms => c.Sleeps.Add(ms);
            TAsphyreTimer.TimeBeginPeriodFn = ms => c.TimePeriods.Add(ms);
            TAsphyreTimer.SetApplicationOnIdleFn = h => { c.IdleHandler = h; c.IdleSets.Add(h); };
        }

        public void Dispose()
        {
            TAsphyreTimer.QueryPerformanceFrequencyFn = _qpf;
            TAsphyreTimer.QueryPerformanceCounterFn = _qpc;
            TAsphyreTimer.GetTickCountFn = _tick;
            TAsphyreTimer.SleepExFn = _sleep;
            TAsphyreTimer.TimeBeginPeriodFn = _period;
            TAsphyreTimer.SetApplicationOnIdleFn = _idle;
        }
    }

    /// <summary>默认 SpeedLatcy = Round(1048576 * 1000 / 60)（原文 164）。</summary>
    private const int SpeedLatcy60 = 17476267;

    /// <summary>默认 MinLatency = Round(1048576 * 1000 / 100)（原文 173）。</summary>
    private const int MinLatency100 = 10485760;

    private static TAsphyreTimer NewTimer(Clock c, out Hook hook, bool enabled = true)
    {
        hook = new Hook(c);
        var t = new TAsphyreTimer();
        t.Enabled = enabled;
        return t;
    }

    // ===============================================================================
    // 一、构造（原文 129-156）
    // ===============================================================================

    [Fact]
    public void Constructor_LowPrecision_DefaultsMatchOriginal()
    {
        var c = new Clock { HighPrecision = false };
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        Assert.Equal(TPerformancePrecision.ppLow, t.Precision);   // 原文 136-137
        Assert.Equal(60.0, t.Speed);                              // 原文 133
        Assert.Equal(100, t.MaxFPS);                              // 原文 134
        Assert.False(t.Enabled);                                  // 原文 59 FEnabled 未赋值 → False
        Assert.True(t.IdleDone);                                  // 原文 155
        Assert.Equal(0, t.FrameRate);                             // 原文 151
        Assert.Equal(0.0, t.Delta);                               // 原文 150 DeltaFP := 0
        Assert.Equal(0.0, t.Latency);                             // LatencyFP 未初始化 → 0
        Assert.Null(t.OnTimer);                                   // 原文 60
        Assert.Null(t.OnProcess);                                 // 原文 67
    }

    [Fact]
    public void Constructor_HighPrecision_SetsPrecisionAndSamplesCounter()
    {
        var c = new Clock { HighPrecision = true, PerfFreq = 3_579_545, PerfCounter = 12345 };
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        Assert.Equal(TPerformancePrecision.ppHigh, t.Precision);   // 原文 137
        // 原文 140：高精度时采样 QPC 作为 PrevTime64。用一次 OnIdle 验证基准点确实是 12345。
        c.PerfCounter = 12345 + 3_579_545;                          // Δ=1 秒
        t.OnIdle();
        Assert.Equal(1000.0, t.Latency, 6);                         // ΔQPC * FixedHigh * 1000 / HighFreq
    }

    [Fact]
    public void Constructor_CallsTimeBeginPeriod1()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        new TAsphyreTimer();

        Assert.Equal(new List<int> { 1 }, c.TimePeriods);           // 原文 148
    }

    [Fact]
    public void Constructor_WiresIdleHandler()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        new TAsphyreTimer();

        Assert.Single(c.IdleSets);                                  // 原文 145
        Assert.NotNull(c.IdleHandler);
    }

    [Fact]
    public void Constructor_LowPrecision_DoesNotTouchPerfCounter()
    {
        var c = new Clock { HighPrecision = false, Tick = 77 };
        long qpcCalls = 0;
        using var _ = new Hook(c);
        TAsphyreTimer.QueryPerformanceCounterFn = (out long v) => { qpcCalls++; v = 0; return true; };

        new TAsphyreTimer();

        Assert.Equal(0, qpcCalls);                                  // 原文 139-140 走 else 分支
    }

    // ===============================================================================
    // 二、Speed / MaxFPS（原文 160-174）
    // ===============================================================================

    [Fact]
    public void Speed_Zero_IsClampedToOne()
    {
        var c = new Clock();
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Speed = 0.0;
            Assert.Equal(1.0, t.Speed);                             // 原文 163 写字段
        }
    }

    [Fact]
    public void Speed_Negative_IsClampedToOne()
    {
        var c = new Clock();
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Speed = -12.5;
            Assert.Equal(1.0, t.Speed);
        }
    }

    [Fact]
    public void Speed_ExactlyOne_IsKept()
    {
        var c = new Clock();
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Speed = 1.0;
            Assert.Equal(1.0, t.Speed);
        }
    }

    [Fact]
    public void Speed_Fractional_74_2_SetsSpeedLatcyByBankersRounding()
    {
        // SpeedLatcy = Round(1048576 * 1000 / 74.2) = Round(14131752.02...) = 14131752
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Speed = 74.2;

            c.Tick = 10;                                            // Δ=10ms → DeltaFP 远低于 DeltaLimit
            t.OnIdle();

            int expected = (int)Math.Round(TAsphyreTimer.FixedHigh * 1000.0 / 74.2);
            Assert.Equal(14131752, expected);                       // 先钉住常数本身
            // DeltaFP = 10*FixedHigh * FixedHigh div 14131752 = 778043（未被 DeltaLimit 夹取）
            int deltaFp = (int)((10L * TAsphyreTimer.FixedHigh * TAsphyreTimer.FixedHigh) / expected);
            Assert.Equal(778043, deltaFp);
            Assert.Equal(deltaFp / (double)TAsphyreTimer.FixedHigh, t.Delta, 9);
        }
    }

    [Fact]
    public void Speed_ValueRoundTrips()
    {
        var c = new Clock();
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Speed = 30.0;
            Assert.Equal(30.0, t.Speed);
        }
    }

    [Fact]
    public void MaxFPS_Zero_IsClampedToOne()
    {
        var c = new Clock();
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.MaxFPS = 0;
            Assert.Equal(1, t.MaxFPS);                              // 原文 172
        }
    }

    [Fact]
    public void MaxFPS_Negative_IsClampedToOne()
    {
        var c = new Clock();
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.MaxFPS = -100;
            Assert.Equal(1, t.MaxFPS);
        }
    }

    [Fact]
    public void MaxFPS_Default100_YieldsSleepOfTenMilliseconds()
    {
        // LatencyFP = 0（Tick 不变）< MinLatency=10485760 → WaitAmount = 10485760 div 1048576 = 10
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.Tick = 0;
            t.OnIdle();

            Assert.Equal(new List<int> { MinLatency100 / TAsphyreTimer.FixedHigh }, c.Sleeps);
            Assert.Equal(10, c.Sleeps[0]);
        }
    }

    [Fact]
    public void MaxFPS_MaxValue_MinLatencyRoundsToZero_AndThenDividesByZero()
    {
        // MinLatency = Round(1048576 * 1000 / 2147483647) = Round(0.488) = 0
        // → LatencyFP=0 不再触发等待，SampleLatency 恒为 0 → 第 4 次采样 div 0（Delphi 亦 EDivByZero）
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.MaxFPS = int.MaxValue;
            Assert.Empty(c.Sleeps);

            t.OnIdle();
            t.OnIdle();
            t.OnIdle();
            Assert.Equal(0, t.FrameRate);

            Assert.Throws<DivideByZeroException>(() => t.OnIdle());  // 原文 248 div SampleLatency
        }
    }

    // ===============================================================================
    // 三、Enabled / IdleDone / OnTimer / OnProcess 读写（原文 107-112）
    // ===============================================================================

    [Fact]
    public void Enabled_RoundTrips()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        Assert.False(t.Enabled);
        t.Enabled = true;
        Assert.True(t.Enabled);
        t.Enabled = false;
        Assert.False(t.Enabled);
    }

    [Fact]
    public void IdleDone_RoundTrips()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        Assert.True(t.IdleDone);
        t.IdleDone = false;
        Assert.False(t.IdleDone);
        t.IdleDone = true;
        Assert.True(t.IdleDone);
    }

    [Fact]
    public void OnTimer_And_OnProcess_RoundTrip()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        TNotifyEvent a = _ => { };
        TNotifyEvent b = _ => { };
        t.OnTimer = a;
        t.OnProcess = b;
        Assert.Same(a, t.OnTimer);
        Assert.Same(b, t.OnProcess);
        t.OnTimer = null;
        Assert.Null(t.OnTimer);
    }

    // ===============================================================================
    // 四、RetreiveLatency 低精度分支（原文 202-207）
    // ===============================================================================

    [Fact]
    public void OnIdle_LowPrecision_LatencyIsTickDeltaTimesFixedHigh()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.Tick = 1000;
            t.OnIdle();

            Assert.Equal(1000.0, t.Latency, 12);                    // 1000 * FixedHigh / FixedHigh
            Assert.Equal(1000.0 * TAsphyreTimer.FixedHigh, (double)t.Latency * TAsphyreTimer.FixedHigh, 6);
        }
    }

    [Fact]
    public void OnIdle_LowPrecision_ZeroTickDelta_StillWaitsFullMinLatency()
    {
        var c = new Clock { Tick = 500 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.OnIdle();                                             // Tick 未推进 → LatencyFP = 0
            Assert.Equal(0.0, t.Latency);
            Assert.Equal(10, c.Sleeps[0]);
            Assert.Equal(0.0, t.Delta);
        }
    }

    [Fact]
    public void OnIdle_LowPrecision_CardinalWrapAround_YieldsNegativeLatency()
    {
        // 原文 204-205：`(CurTime - PrevTime)` 是 **Cardinal**（mod 2^32），
        // 时钟倒退 100ms → 差值 4294967196 → * FixedHigh 后截断回 Integer = -104857600
        // → Latency = -100.0，且 WaitAmount = (MinLatency + 104857600) div FixedHigh = 110
        var c = new Clock { Tick = 1000 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.Tick = 900;                                           // 倒退
            t.OnIdle();

            Assert.Equal(-100.0, t.Latency, 12);
            Assert.Equal(110, c.Sleeps[0]);
        }
    }

    [Fact]
    public void OnIdle_LowPrecision_Int32TruncationOfLatency()
    {
        // Δ=2048 → 2048 * 1048576 = 2^31 → unchecked((int)) = int.MinValue → Latency = -2048.0
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.Tick = 2048;
            t.OnIdle();

            Assert.Equal(-2048.0, t.Latency, 12);                   // 若不建模截断会得到 +2048
            // WaitAmount = (MinLatency - int.MinValue) div FixedHigh 溢出为负 → 保底 10（原文 231）
            Assert.Equal(10, c.Sleeps[0]);
        }
    }

    // ===============================================================================
    // 五、RetreiveLatency 高精度分支（原文 197-201）
    // ===============================================================================

    [Fact]
    public void OnIdle_HighPrecision_LatencyFromPerfCounter()
    {
        // PerfFreq=100MHz、Δ=2,000,000 → 20ms；LatencyFP = 20*FixedHigh（未溢出 Int32）
        // DeltaFP = 20*FixedHigh*FixedHigh div SpeedLatcy = 1258291（未夹取）
        var c = new Clock { HighPrecision = true, PerfFreq = 100_000_000, PerfCounter = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.PerfCounter = 2_000_000;
            t.OnIdle();

            Assert.Equal(20.0, t.Latency, 12);
            Assert.Equal(1.2, t.Delta, 6);                          // ≈ 20ms * 60fps
        }
    }

    [Fact]
    public void OnIdle_HighPrecision_LongStallIsClampedToDeltaLimit()
    {
        // PerfFreq=1MHz、Δ=2s → LatencyFP=2097152000 → DeltaFP≈125829117 > DeltaLimit
        // → 夹到 32*FixedHigh → Delta = 32.0（原文 238）
        var c = new Clock { HighPrecision = true, PerfFreq = 1_000_000, PerfCounter = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.PerfCounter = 2_000_000;
            t.OnIdle();

            Assert.Equal(2000.0, t.Latency, 6);
            Assert.Equal(32.0, t.Delta, 12);
        }
    }

    [Fact]
    public void OnIdle_HighPrecision_TruncatesFrameRateToZeroWhenSampleMaxIsZero()
    {
        // LatencyFP = 2097152000 > FixedHigh*1000=1048576000
        // → SampleMax = 1048576000 div 2097152000 = 0（整数除法）→ 每次都重算
        // → FrameRate = (1 * FixedHigh * 1000) div SampleLatency = 1048576000 div 2097152000 = 0
        var c = new Clock { HighPrecision = true, PerfFreq = 1_000_000, PerfCounter = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.PerfCounter = 2_000_000;
            t.OnIdle();

            Assert.Equal(0, t.FrameRate);
        }
    }

    [Fact]
    public void OnIdle_HighPrecision_ZeroCounterDelta_KeepsLatencyZero()
    {
        var c = new Clock { HighPrecision = true, PerfFreq = 1_000_000, PerfCounter = 4242 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.OnIdle();                                             // 计数器未推进
            Assert.Equal(0.0, t.Latency);
            Assert.Equal(0.0, t.Delta);
        }
    }

    // ===============================================================================
    // 六、DoTimerEvent 其余步骤（原文 212-262）
    // ===============================================================================

    [Fact]
    public void OnIdle_Disabled_SleepsFiveMillisecondsAndSkipsEverything()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook, enabled: false);
        using (hook)
        {
            int fired = 0;
            t.OnTimer = _ => fired++;
            c.Tick = 2000;                                          // 2000 * FixedHigh 未溢出 Int32

            t.OnIdle();

            Assert.Equal(new List<int> { 5 }, c.Sleeps);             // 原文 223
            Assert.Equal(0, fired);                                 // 原文 224 Exit，未到第 (7) 步
            Assert.Equal(0.0, t.Delta);                             // DeltaFP 未更新
            Assert.Equal(2000.0, t.Latency, 12);                    // LatencyFP 在早退前已更新（原文 218）
        }
    }

    [Fact]
    public void OnIdle_Enabled_FiresOnTimerOncePerCall()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            int fired = 0;
            t.OnTimer = _ => fired++;

            c.Tick = 100;
            t.OnIdle();
            c.Tick = 200;
            t.OnIdle();

            Assert.Equal(2, fired);
        }
    }

    [Fact]
    public void OnIdle_DeltaIsClampedToDeltaLimit()
    {
        // 极长延迟：LatencyFP 巨大会让 DeltaFP 超过 DeltaLimit=32*FixedHigh
        var c = new Clock { HighPrecision = true, PerfFreq = 1_000_000, PerfCounter = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.PerfCounter = 1_000_000_000L;                         // 1000 秒
            t.OnIdle();

            Assert.Equal(TAsphyreTimer.DeltaLimit / (double)TAsphyreTimer.FixedHigh, t.Delta, 12);
            Assert.Equal(32.0, t.Delta, 12);                        // 原文 238
        }
    }

    [Fact]
    public void OnIdle_FrameRateIsOneWhenOneFramePerSecond()
    {
        // SampleMax = 1048576000 div LatencyFP；LatencyFP = 1000*FixedHigh → SampleMax = 1
        // FrameRate = (1 * FixedHigh * 1000) div SampleLatency(=1000*FixedHigh) = 1（原文 248）
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.Tick = 1000;
            t.OnIdle();

            Assert.Equal(1, t.FrameRate);
        }
    }

    [Fact]
    public void OnIdle_FrameRateStaysZeroUntilSampleMaxReached()
    {
        // LatencyFP = 10 * FixedHigh → SampleMax = 1048576000 div 10485760 = 100 → 前 99 次不算
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            for (int i = 1; i <= 99; i++)
            {
                c.Tick = (uint)(i * 10);
                t.OnIdle();
            }
            Assert.Equal(0, t.FrameRate);

            c.Tick = 1000;
            t.OnIdle();
            Assert.NotEqual(0, t.FrameRate);
        }
    }

    [Fact]
    public void OnIdle_NoOnTimer_DoesNotThrow()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.OnTimer = null;
            c.Tick = 1000;
            t.OnIdle();                                             // 不抛
            Assert.Equal(1, t.FrameRate);
        }
    }

    // ===============================================================================
    // 七、Process（原文 300-314）
    // ===============================================================================

    [Fact]
    public void Process_WithoutFixedDelta_ReturnsImmediately()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            int fired = 0;
            t.OnProcess = _ => fired++;

            t.Process();

            Assert.Equal(0, fired);                                 // Amount = 0 div FixedHigh = 0
        }
    }

    [Fact]
    public void Process_InvokesOnProcessAmountTimes()
    {
        // 60fps、Δ=100ms → DeltaFP = 100*FixedHigh*FixedHigh div SpeedLatcy = 6291455（未夹取）
        // Amount = 6291455 div FixedHigh = 5，余数 1048575
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            int fired = 0;
            t.OnProcess = _ => fired++;

            t.Process();                                            // Processed := True
            c.Tick = 100;
            t.OnIdle();                                             // FixedDelta = 6291455
            t.Process();

            Assert.Equal(5, fired);
            t.Process();                                            // 余数 1048575 < FixedHigh
            Assert.Equal(5, fired);
        }
    }

    [Fact]
    public void Process_MasksFixedDeltaEvenWhenOnProcessIsNull()
    {
        // 原文 309-313：`FixedDelta := FixedDelta and (FixedHigh-1)` 在 `if Assigned` **之外**
        // → OnProcess 为 nil 时也取低 20 位（6291455 → 1048575）。
        // **差异断言**：若把取位误放进 `if Assigned` 内，第 3 次 Process 会看到完整的
        // 6291455 → 5 次回调，而不是 0 次。
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Process();
            c.Tick = 100;
            t.OnIdle();

            int fired = 0;
            t.Process();                                            // OnProcess 为 nil：无回调，但仍取低 20 位
            t.OnProcess = _ => fired++;
            t.Process();                                            // 1048575 < FixedHigh → 0 次

            Assert.Equal(0, fired);
        }
    }

    [Fact]
    public void Process_MaskedRemainderAccumulatesIntoNextFrame()
    {
        // 余数 1048575 只差 1 就到一帧；下一轮 Δ=1ms 的 DeltaFP(62914) 便凑出 1 帧
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Process();
            c.Tick = 100;
            t.OnIdle();
            t.Process();                                            // 取低 20 位 → 1048575

            int fired = 0;
            t.OnProcess = _ => fired++;
            t.Process();                                            // Processed := True
            c.Tick = 101;                                           // Δ=1ms
            t.OnIdle();                                             // 1048575 + 62914 = 1111489
            t.Process();

            Assert.Equal(1, fired);
            Assert.Equal(9, c.Sleeps[0]);                           // Δ1ms < MinLatency → WaitAmount=9
        }
    }

    [Fact]
    public void Process_AccumulatesOnlyWhenProcessedFlagWasSet()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            // 不调用 Process → Processed 为 False → OnIdle 不累积 FixedDelta
            c.Tick = 1000;
            t.OnIdle();

            int fired = 0;
            t.OnProcess = _ => fired++;
            t.Process();
            Assert.Equal(0, fired);                                 // FixedDelta 仍为 0
        }
    }

    [Fact]
    public void Process_ClearsProcessedFlag()
    {
        // Processed 被第 (6) 步消费 → 同一次延迟不会二次累积
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Process();
            c.Tick = 100;
            t.OnIdle();                                             // 累积 6291455，Processed → False

            int total = 0;
            t.OnProcess = _ => total++;

            t.Process();                                            // 5 次
            int afterFirst = total;
            t.Process();                                            // 余数 1048575 → 0 次

            Assert.Equal(5, afterFirst);
            Assert.Equal(afterFirst, total);
        }
    }

    // ===============================================================================
    // 八、Reset（原文 318-323）
    // ===============================================================================

    [Fact]
    public void Reset_ClearsFixedDeltaAndDeltaFp()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Process();
            c.Tick = 1000;
            t.OnIdle();
            Assert.NotEqual(0.0, t.Delta);

            t.Reset();
            Assert.Equal(0.0, t.Delta);                             // 原文 321
        }
    }

    [Fact]
    public void Reset_ConsumesTimeBaseButLeavesLatencyUntouched()
    {
        // 原文 322：`RetreiveLatency();` 的返回值被**丢弃**（当过程用）
        // → LatencyFP 不变（构造后为 0），被消费的是 PrevTime 这个时间基准。
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            c.Tick = 2000;                                          // 2000 * FixedHigh 未溢出 Int32
            t.Reset();

            Assert.Equal(0.0, t.Latency);                           // LatencyFP 未被赋值（原文怪癖）

            t.OnIdle();                                             // 基准已被 Reset 推到 2000
            Assert.Equal(0.0, t.Latency);                           // 故本次 ΔTick = 0
            Assert.Equal(10, c.Sleeps[0]);                          // WaitAmount = MinLatency div FixedHigh
        }
    }

    [Fact]
    public void Reset_ThenProcessDoesNothing()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Process();
            c.Tick = 1000;
            t.OnIdle();

            t.Reset();                                              // FixedDelta := 0
            int fired = 0;
            t.OnProcess = _ => fired++;
            t.Process();

            Assert.Equal(0, fired);
        }
    }

    [Fact]
    public void Reset_KeepsSpeedAndMaxFps()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.Speed = 30.0;
            t.MaxFPS = 50;
            t.Reset();

            Assert.Equal(30.0, t.Speed);
            Assert.Equal(50, t.MaxFPS);
        }
    }

    // ===============================================================================
    // 九、Start / Stop（原文 264-280）—— 原文接线与直觉相反
    // ===============================================================================

    [Fact]
    public void Start_UnhooksIdleHandler()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();
        Assert.NotNull(c.IdleHandler);

        t.Start();

        Assert.Null(c.IdleHandler);                                 // 原文 267：Start 置 nil
    }

    [Fact]
    public void Stop_HooksIdleHandlerBack()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        t.Start();
        Assert.Null(c.IdleHandler);
        t.Stop();

        Assert.NotNull(c.IdleHandler);                              // 原文 278：Stop 装 AppIdle
    }

    [Fact]
    public void Stop_ThenStart_IsIdempotentOnHandlerPresence()
    {
        var c = new Clock();
        using var _ = new Hook(c);
        var t = new TAsphyreTimer();

        int before = c.IdleSets.Count;
        t.Stop();
        t.Start();
        t.Stop();

        Assert.Equal(before + 3, c.IdleSets.Count);
        Assert.NotNull(c.IdleHandler);
    }

    // ===============================================================================
    // 十、AppIdle 顺序（原文 292-296）
    // ===============================================================================

    [Fact]
    public void AppIdle_WritesIdleDoneIntoDoneBeforeTimerEvent()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.IdleDone = false;
            var done = new BoolRef(true);

            int fired = 0;
            t.OnTimer = _ => { Assert.Equal(false, done.Value); fired++; };   // 事件时 Done 已写回

            c.IdleHandler(null, done);

            Assert.False(done.Value);                               // 原文 294 Done := FIdleDone
            Assert.Equal(1, fired);
        }
    }

    [Fact]
    public void AppIdle_RunsTimerEventEvenWhenDisabled()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook, enabled: false);
        using (hook)
        {
            int fired = 0;
            t.OnTimer = _ => fired++;

            var done = new BoolRef(false);
            c.IdleHandler(null, done);

            Assert.True(done.Value);                                // IdleDone 默认 True
            Assert.Equal(0, fired);                                 // 仍走 disabled 早退
            Assert.Equal(5, c.Sleeps[0]);
        }
    }

    [Fact]
    public void AppIdle_IdleDoneTrueIsPreserved()
    {
        var c = new Clock { Tick = 0 };
        var t = NewTimer(c, out var hook);
        using (hook)
        {
            t.IdleDone = true;
            var done = new BoolRef(false);
            c.IdleHandler(null, done);
            Assert.True(done.Value);
        }
    }

    // ===============================================================================
    // 十一、接缝默认值（无头安全）
    // ===============================================================================

    [Fact]
    public void SeamDefaults_DoNotTouchWinFormsOrSleep()
    {
        // 撤掉夹具后恢复系统默认值；断言"默认 SetApplicationOnIdleFn / TimeBeginPeriodFn
        // 都是 no-op"（无头环境不接线、不久睡、不碰 WinForms）。
        var c = new Clock();
        var hook = new Hook(c);
        hook.Dispose();                                             // 恢复系统默认

        TAsphyreTimer.SetApplicationOnIdleFn(null);                 // 不得抛、不得碰 WinForms
        TAsphyreTimer.TimeBeginPeriodFn(1);                         // 不得抛
        Assert.Empty(c.TimePeriods);                                // 夹具已撤 → 默认实现未记录
        Assert.Empty(c.Sleeps);
    }

    [Fact]
    public void GetTickCountFn_DefaultsToDxTickCount()
    {
        // 默认实现必须接在本树既有的 DxTickCount 上（单一全局注入点）
        var c = new Clock();
        var hook = new Hook(c);
        hook.Dispose();

        var dflt = TAsphyreTimer.GetTickCountFn;
        Assert.NotNull(dflt);
        Assert.Equal(DxTickCount.MyGetTickCount(), dflt());
    }

    [Fact]
    public void QueryPerformanceFrequencyFn_DefaultReportsSystemFrequency()
    {
        var c = new Clock();
        var hook = new Hook(c);
        hook.Dispose();

        bool ok = TAsphyreTimer.QueryPerformanceFrequencyFn(out long freq);
        if (System.Diagnostics.Stopwatch.IsHighResolution)
        {
            Assert.True(ok);
            Assert.Equal(System.Diagnostics.Stopwatch.Frequency, freq);
        }
        else
        {
            Assert.False(ok);
        }
    }

    [Fact]
    public void DetachWinFormsIdle_RestoresNoOpSeam()
    {
        TAsphyreTimer.AttachWinFormsIdle();
        TAsphyreTimer.DetachWinFormsIdle();

        TAsphyreTimer.SetApplicationOnIdleFn(null);                 // 不得抛、不得接 WinForms
    }

    // ===============================================================================
    // 十二、枚举与常量（原文 52 / 124-125）
    // ===============================================================================

    [Fact]
    public void TPerformancePrecision_OrderMatchesOriginal()
    {
        Assert.Equal(0, (int)TPerformancePrecision.ppLow);
        Assert.Equal(1, (int)TPerformancePrecision.ppHigh);
    }

    [Fact]
    public void Constants_MatchOriginal()
    {
        Assert.Equal(0x100000, TAsphyreTimer.FixedHigh);            // 原文 124
        Assert.Equal(32 * 0x100000, TAsphyreTimer.DeltaLimit);      // 原文 125
        Assert.Equal(33554432, TAsphyreTimer.DeltaLimit);
    }

    // ===============================================================================
    // 十三、单元级 Timer（原文 116-117 / 326-331）
    // ===============================================================================

    [Fact]
    public void AsphyreTimerUnit_TimerExistsWithConstructorDefaults()
    {
        // 断言与注入的时钟无关（单元初始化可能在任何一次触碰时执行）
        var timer = AsphyreTimerUnit.Timer;

        Assert.NotNull(timer);
        Assert.Equal(60.0, timer.Speed);                            // 原文 133
        Assert.Equal(100, timer.MaxFPS);                            // 原文 134
        Assert.True(timer.IdleDone);                                // 原文 155
        Assert.False(timer.Enabled);                                // 原文 59
    }

    [Fact]
    public void AsphyreTimerUnit_TimerIsSingletonUntilFinalized()
    {
        var a = AsphyreTimerUnit.Timer;
        Assert.Same(a, AsphyreTimerUnit.Timer);
    }

    [Fact]
    public void AsphyreTimerUnit_FinalizeUnit_DiscardsInstance()
    {
        // 原文 331 `Timer.Free();` —— 托管侧对应"丢弃实例"（再次访问会重新初始化，见类注释）
        var before = AsphyreTimerUnit.Timer;

        AsphyreTimerUnit.FinalizeUnit();

        Assert.NotNull(AsphyreTimerUnit.Timer);
        Assert.NotSame(before, AsphyreTimerUnit.Timer);
    }

    [Fact]
    public void AsphyreTimerUnit_FinalizeUnit_IsIdempotent()
    {
        AsphyreTimerUnit.FinalizeUnit();
        AsphyreTimerUnit.FinalizeUnit();                            // 第二次不得抛

        var t = AsphyreTimerUnit.Timer;                             // 重新初始化后仍可用
        t.Enabled = true;
        Assert.True(t.Enabled);
    }

    [Fact]
    public void AsphyreTimerUnit_TimerIsUsableAfterFinalize()
    {
        AsphyreTimerUnit.FinalizeUnit();

        var c = new Clock { Tick = 0 };
        using var _ = new Hook(c);                                  // 先装接缝，再让单元初始化

        var t = AsphyreTimerUnit.Timer;
        int fired = 0;
        t.OnTimer = _ => fired++;
        t.Enabled = true;
        c.Tick = 2000;
        t.OnIdle();

        Assert.Equal(1, fired);
    }
}

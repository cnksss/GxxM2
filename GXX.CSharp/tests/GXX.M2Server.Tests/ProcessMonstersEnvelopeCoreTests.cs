using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J107：ProcessMonsters 外层骨架（UsrEngn.pas 3974-4199）+ 统计量衰减
/// （svMain.pas 767-774）+ GetGenMonCount(4201-4218) 1:1 测试。
/// </summary>
public sealed class ProcessMonstersEnvelopeCoreTests
{
    private static ProcessMonstersEnvelopeCore.ProcTimeStats Stats(
        int monProcTimeMin = 0, int monProcTimeMax = 0, int monTimeMin = 0, int monTimeMax = 0)
        => new()
        {
            MonProcTimeMin = monProcTimeMin,
            MonProcTimeMax = monProcTimeMax,
            MonTimeMin = monTimeMin,
            MonTimeMax = monTimeMax,
        };

    // ===================== 异常消息（3987/4178-4186） =====================

    [Fact]
    public void ExceptionMsgFormat()
    {
        Assert.Equal("[Exception] TUserEngine.ProcessMonsters 14; boom",
            ProcessMonstersEnvelopeCore.ExceptionMsg(14, "boom"));
    }

    [Fact]
    public void NilPlaceholderIsLiteralNil()
    {
        Assert.Equal("nil", ProcessMonstersEnvelopeCore.NilPlaceholder);
    }

    [Fact]
    public void ExceptionDetailUsesMessageWhenMonsterPresent()
    {
        Assert.Equal("boom", ProcessMonstersEnvelopeCore.SelectExceptionDetail(false, "boom"));
    }

    [Fact]
    public void ExceptionDetailUsesNilWhenMonsterAbsent()
    {
        // 4185：Monster = nil → 第二参退化为字面量 'nil'，**不**用真实消息
        Assert.Equal("nil", ProcessMonstersEnvelopeCore.SelectExceptionDetail(true, "boom"));
    }

    [Fact]
    public void IsErrorSetOnlyWhenMonsterPresent()
    {
        // 4181：仅 Monster <> nil 时置 IsError
        Assert.True(ProcessMonstersEnvelopeCore.ShouldSetIsError(false));
        Assert.False(ProcessMonstersEnvelopeCore.ShouldSetIsError(true));
    }

    [Fact]
    public void EarlyExceptionDoesNotSetIsError()
    {
        // **行为后果**：Monster 仍为 nil 时抛异常（如 3992-4000 准备段）→ 不置 IsError
        // → 4189 的诊断段也不执行，故只打印一条消息
        bool isError = ProcessMonstersEnvelopeCore.ShouldSetIsError(monsterIsNull: true);
        Assert.False(isError);
        Assert.False(ProcessMonstersEnvelopeCore.ShouldPrintAbnormalObject(isError, true));
    }

    [Fact]
    public void AbnormalObjectDiagnosisNeedsBoth()
    {
        Assert.True(ProcessMonstersEnvelopeCore.ShouldPrintAbnormalObject(true, false));
        Assert.False(ProcessMonstersEnvelopeCore.ShouldPrintAbnormalObject(true, true));
        Assert.False(ProcessMonstersEnvelopeCore.ShouldPrintAbnormalObject(false, false));
    }

    [Fact]
    public void AbnormalObjectMsgDistinguishesPet()
    {
        // 4193/4195
        Assert.Equal("异常怪物: 骷髅", ProcessMonstersEnvelopeCore.AbnormalObjectMsg(false, "骷髅"));
        Assert.Equal("异常宠物: 小狗", ProcessMonstersEnvelopeCore.AbnormalObjectMsg(true, "小狗"));
    }

    // ===================== 统计段（4165-4174） =====================

    [Fact]
    public void MonProcTimeIsNowMinusStartTick()
    {
        var s = Stats();
        var (monProcTime, _) = ProcessMonstersEnvelopeCore.UpdateTimingStats(s, now: 1500, monProcTick: 1000, runTick: 1000);
        Assert.Equal(500, monProcTime);
    }

    [Fact]
    public void BothTimingsEqualWhenTicksEqual()
    {
        // dwMonProcTick(3998) 与 dwRunTick(3992) 都在本轮开始处取值，
        // 故无异常时二者必然相等
        var s = Stats();
        var (monProcTime, monTime) = ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 1500, 1000, 1000);
        Assert.Equal(monTime, monProcTime);
    }

    [Fact]
    public void MonProcTimeMinTakesLargerValue()
    {
        // **4167-4168：名为 Min 却取更大值**——语义是衰减峰值，非最小值
        var s = Stats(monProcTimeMin: 10);
        ProcessMonstersEnvelopeCore.UpdateTimingStats(s, now: 1000, monProcTick: 0, runTick: 0);

        Assert.Equal(1000, s.MonProcTimeMin);
        Assert.NotEqual(10, s.MonProcTimeMin);   // 未取小
    }

    [Fact]
    public void MonProcTimeMinDoesNotShrink()
    {
        // 更小的值**不覆盖**（这正是"峰值"特征，与 Min 之名相反）
        var s = Stats(monProcTimeMin: 5000);
        ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 100, 0, 0);
        Assert.Equal(5000, s.MonProcTimeMin);
    }

    [Fact]
    public void MonProcTimeMaxAlsoTakesLarger()
    {
        var s = Stats(monProcTimeMax: 10);
        ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 1000, 0, 0);
        Assert.Equal(1000, s.MonProcTimeMax);
    }

    [Fact]
    public void MonProcTimeMinAndMaxTrackIdentically()
    {
        // 4167-4168 与 4169-4170 条件形式**完全相同**（都是 value > 原值），
        // 故两者恒等——它们是同一个量的两份冗余拷贝
        var s = Stats();
        ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 777, 0, 0);

        Assert.Equal(s.MonProcTimeMax, s.MonProcTimeMin);
    }

    [Fact]
    public void MinAndMaxStayEqualAcrossRounds()
    {
        var s = Stats();

        foreach (uint now in new[] { 100u, 5000u, 200u, 9000u, 50u })
        {
            ProcessMonstersEnvelopeCore.UpdateTimingStats(s, now, 0, 0);
            Assert.Equal(s.MonProcTimeMax, s.MonProcTimeMin);
        }
    }

    [Fact]
    public void MonTimeMaxUsesReversedComparison()
    {
        // 4173：`if g_nMonTimeMax < g_nMonTimeMin` —— 形式与 4167/4169 **相反**，
        // 但效果同样是"取更大值"
        var s = Stats(monTimeMax: 10);
        ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 1000, 0, 0);
        Assert.Equal(1000, s.MonTimeMax);
    }

    [Fact]
    public void MonTimeMaxDoesNotShrink()
    {
        var s = Stats(monTimeMax: 5000);
        ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 100, 0, 0);
        Assert.Equal(5000, s.MonTimeMax);
    }

    [Fact]
    public void TimingUpdateIsUnconditional()
    {
        // 4166/4172：两个当前值**无条件**覆盖，故表示"最近一轮"
        var s = Stats();
        var (a, _) = ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 1000, 0, 0);
        var (b, _) = ProcessMonstersEnvelopeCore.UpdateTimingStats(s, 50, 0, 0);

        Assert.Equal(1000, a);
        Assert.Equal(50, b);    // 变小了，因为是无条件赋值
    }

    // ===================== 衰减（svMain.pas 767-774） =====================

    [Fact]
    public void MonTimeMaxDecaysByOne()
    {
        // 767-768
        var s = Stats(monTimeMax: 10);
        ProcessMonstersEnvelopeCore.DecayStats(s);
        Assert.Equal(9, s.MonTimeMax);
    }

    [Fact]
    public void MonProcTimeMinDecaysByTwo()
    {
        // 773-774
        var s = Stats(monProcTimeMin: 10);
        ProcessMonstersEnvelopeCore.DecayStats(s);
        Assert.Equal(8, s.MonProcTimeMin);
    }

    [Fact]
    public void MonTimeMaxFloorIsZero()
    {
        // 767：if > 0 then Dec
        var s = Stats(monTimeMax: 0);
        ProcessMonstersEnvelopeCore.DecayStats(s);
        Assert.Equal(0, s.MonTimeMax);

        var s1 = Stats(monTimeMax: 1);
        ProcessMonstersEnvelopeCore.DecayStats(s1);
        Assert.Equal(0, s1.MonTimeMax);
    }

    [Fact]
    public void MonProcTimeMinFloorIsOne()
    {
        // 773：if > 1 then Dec 2 —— **下限是 1 不是 0**
        var s = Stats(monProcTimeMin: 1);
        ProcessMonstersEnvelopeCore.DecayStats(s);
        Assert.Equal(1, s.MonProcTimeMin);

        var s0 = Stats(monProcTimeMin: 0);
        ProcessMonstersEnvelopeCore.DecayStats(s0);
        Assert.Equal(0, s0.MonProcTimeMin);

        // 2 → 0（因为 2 > 1 成立）
        var s2 = Stats(monProcTimeMin: 2);
        ProcessMonstersEnvelopeCore.DecayStats(s2);
        Assert.Equal(0, s2.MonProcTimeMin);
    }

    [Fact]
    public void TwoStatsDecayDifferently()
    {
        // **差异保护**：两者步长与下限都不同，不可统一
        var s = Stats(monTimeMax: 10, monProcTimeMin: 10);
        ProcessMonstersEnvelopeCore.DecayStats(s);

        Assert.NotEqual(s.MonTimeMax, s.MonProcTimeMin);   // 9 vs 8
    }

    [Fact]
    public void DecayNeverTouchesOtherFields()
    {
        // 767-776 只衰减 Max/Min 若干项，**不动** MonTimeMin / MonProcTimeMax
        var s = Stats(monProcTimeMin: 100, monProcTimeMax: 100, monTimeMin: 100, monTimeMax: 100);
        ProcessMonstersEnvelopeCore.DecayStats(s);

        Assert.Equal(100, s.MonProcTimeMax);
        Assert.Equal(100, s.MonTimeMin);
    }

    [Fact]
    public void PeakDecaysOverManyFrames()
    {
        // 峰值从 100 每帧 -2（Min）与 -1（Max），故 Max 衰减更慢
        var s = Stats(monTimeMax: 100, monProcTimeMin: 100);

        for (int i = 0; i < 10; i++)
            ProcessMonstersEnvelopeCore.DecayStats(s);

        Assert.Equal(90, s.MonTimeMax);      // 100 - 10*1
        Assert.Equal(80, s.MonProcTimeMin);  // 100 - 10*2
    }

    // ===================== 显示串（svMain.pas 699） =====================

    [Fact]
    public void DisplayFormatMatchesSource()
    {
        Assert.Equal(" MonP:5/3/9", ProcessMonstersEnvelopeCore.FormatMonProcDisplay(5, 3, 9));
    }

    [Fact]
    public void DisplayHasLeadingSpace()
    {
        // 699 原文为 `' MonP:'`，前导空格是拼接产物的一部分
        Assert.StartsWith(" ", ProcessMonstersEnvelopeCore.FormatMonProcDisplay(1, 2, 3));
    }

    // ===================== GetGenMonCount（4201-4218） =====================

    [Fact]
    public void GenMonCountCountsAliveNonGhost()
    {
        var certs = new (bool, bool, bool)[]
        {
            (false, false, false),   // 活 → 计
            (false, true, false),    // 死 → 不计
            (false, false, true),    // 幽灵 → 不计
        };

        Assert.Equal(1, ProcessMonstersEnvelopeCore.GetGenMonCount(certs));
    }

    [Fact]
    public void GenMonCountSkipsNil()
    {
        // 4211-4212：nil 直接跳过
        var certs = new (bool, bool, bool)[]
        {
            (true, false, false),    // nil → 跳过（即使两个标志都是 false）
            (false, false, false),
        };

        Assert.Equal(1, ProcessMonstersEnvelopeCore.GetGenMonCount(certs));
    }

    [Fact]
    public void GenMonCountEmptyIsZero()
    {
        Assert.Equal(0, ProcessMonstersEnvelopeCore.GetGenMonCount(ReadOnlySpan<(bool, bool, bool)>.Empty));
    }

    [Fact]
    public void GenMonCountRequiresBothNegations()
    {
        // 4213：not Death **and** not Ghost
        var deadAndGhost = new (bool, bool, bool)[] { (false, true, true) };
        Assert.Equal(0, ProcessMonstersEnvelopeCore.GetGenMonCount(deadAndGhost));

        var aliveOnly = new (bool, bool, bool)[] { (false, false, false) };
        Assert.Equal(1, ProcessMonstersEnvelopeCore.GetGenMonCount(aliveOnly));
    }

    [Fact]
    public void GenMonCountAllAlive()
    {
        var certs = new (bool, bool, bool)[5];
        for (int i = 0; i < 5; i++)
            certs[i] = (false, false, false);

        Assert.Equal(5, ProcessMonstersEnvelopeCore.GetGenMonCount(certs));
    }

    [Fact]
    public void GenMonCountConsistentWithGhostRules()
    {
        // **一致性**：J106 中幽灵不进候选表，此处幽灵也不计入"存活怪物数"——
        // 两处对 m_boGhost 的处理方向一致
        var ghost = new (bool, bool, bool)[] { (false, false, true) };

        Assert.Equal(0, ProcessMonstersEnvelopeCore.GetGenMonCount(ghost));
        Assert.Equal(MonsterListBuildCore.CertAction.GhostRetained,
            MonsterListBuildCore.DecideCertAction(false, true, 0, 1000, 0, 0, 0));
    }
}

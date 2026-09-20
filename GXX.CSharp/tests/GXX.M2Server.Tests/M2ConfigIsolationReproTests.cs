// ============================================================================
// 车道 p6-test-isolation 的复现证据（步骤 1：先证明污染真实存在）
//
// 事实（本轮真实发生）：FormGeneralConfigTests.Open_LoadsAllControlsFromConfig（:488）
// 把静态全局 M2Config.sEnvirDir 设成 "D:\Mir\Envir\" 且不还原；同类写法散落在
// CombatPowerTests / CustomHeroMagicTests / CustomNpcFormTests / CastleFormTests /
// CombatPowerSettingTests / FormJ*Tests 等处。TestConfig.cs 用
// [assembly: CollectionBehavior(DisableTestParallelization = true)] 关了并行，
// 测试按类串行执行，但静态状态在类之间延续 → 后跑的类按残留值拼路径必然错位。
//
// 复现方式：两类各「一污染 + 一受害」，顺序由类级 TestCaseOrderer 显式钉死
// （xunit 默认的同类内方法顺序是 UniqueID 哈希序，实测不是声明顺序，不能依赖）：
//   顺序 A：M2ConfigIsolationPolluterThenVictimTests  —— 污染先跑 → 修复前必红
//   顺序 B：M2ConfigIsolationVictimThenPolluterTests  —— 受害先跑 → 修复前应绿
// 修复后两种顺序都必须全绿（逐测还原 → 顺序无关）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.M2Server.Engine;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GXX.M2Server.Tests;

/// <summary>按方法名序（Ordinal）排序，把复现用例的执行顺序钉死，不依赖 xunit 默认哈希序。</summary>
public sealed class M2ConfigIsolationNameOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
        => testCases.OrderBy(tc => tc.TestMethod.Method.Name, StringComparer.Ordinal);
}

/// <summary>顺序 A：污染先跑（模拟 FormGeneralConfigTests 先跑、后续测试类后跑）。</summary>
[TestCaseOrderer("GXX.M2Server.Tests.M2ConfigIsolationNameOrderer", "GXX.M2Server.Tests")]
public sealed class M2ConfigIsolationPolluterThenVictimTests
{
    private const string PollutedEnvirDir = "D:\\Mir\\Envir\\";

    /// <summary>模拟 FormGeneralConfigTests:488：赋值后不还原。</summary>
    [Fact]
    public void Step01_Polluter_AssignsEnvirDirWithoutRestore()
    {
        M2Config.sEnvirDir = PollutedEnvirDir;

        Assert.Equal(PollutedEnvirDir, M2Config.sEnvirDir);
    }

    /// <summary>
    /// 模拟 p3-m2-sweep 的 SweepNationsTests：实现侧照抄 Nation.pas 用
    /// g_Config.sEnvirDir + '\Nations\...' 拼路径；本类从未设过 sEnvirDir，看到的必须是默认值。
    /// </summary>
    [Fact]
    public void Step02_Victim_SeesPristineEnvirDirAndDerivesPath()
    {
        Assert.Equal(".\\Envir\\", M2Config.sEnvirDir);
        Assert.Equal(".\\Envir\\Nations\\Nations.txt", M2Config.sEnvirDir + "Nations\\Nations.txt");
    }
}

/// <summary>顺序 B：受害先跑（顺序无关性验证的另一半）。</summary>
[TestCaseOrderer("GXX.M2Server.Tests.M2ConfigIsolationNameOrderer", "GXX.M2Server.Tests")]
public sealed class M2ConfigIsolationVictimThenPolluterTests
{
    private const string PollutedEnvirDir = "D:\\Mir\\Envir\\";

    /// <summary>受害类先跑：修复前后都应绿（只用于证明「两种顺序都跑过」）。</summary>
    [Fact]
    public void Step01_Victim_SeesPristineEnvirDirAndDerivesPath()
    {
        Assert.Equal(".\\Envir\\", M2Config.sEnvirDir);
        Assert.Equal(".\\Envir\\Nations\\Nations.txt", M2Config.sEnvirDir + "Nations\\Nations.txt");
    }

    /// <summary>污染类后跑：修复前把残留留给后续测试类，修复后必须被逐测还原吃掉。</summary>
    [Fact]
    public void Step02_Polluter_AssignsEnvirDirWithoutRestore()
    {
        M2Config.sEnvirDir = PollutedEnvirDir;

        Assert.Equal(PollutedEnvirDir, M2Config.sEnvirDir);
    }
}

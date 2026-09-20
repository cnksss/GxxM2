// ============================================================================
// 车道 p6-test-isolation：隔离机制的自检/回归守卫
//
// 这些用例不验证业务逻辑，只验证「静态全局隔离」本身没坏、且覆盖度没漏：
//   * 覆盖度：受管类型的每个**可赋值**静态成员都必须有快照槽（防手写清单漂移）；
//   * 装配：程序集级 TestFramework 特性必须在（防被误删）；
//   * 语义：逐项复位（含 null / 数组元素 / 静态自动属性）；无用例改动时 Restore 是 no-op；
//   * 健康：构建期与运行期诊断都必须为空（任何读/写失败的成员都会在这里现形）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GXX.M2Server.DbLayer;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;
using Xunit.Sdk;

namespace GXX.M2Server.Tests;

public sealed class M2ConfigIsolationEngineTests
{
    /// <summary>程序集级 TestFramework 特性必须在（删掉它就等于隔离失效）。</summary>
    [Fact]
    public void Framework_IsInstalledAtAssemblyLevel()
    {
        var data = typeof(M2ConfigIsolationEngineTests).Assembly
            .GetCustomAttributesData()
            .SingleOrDefault(d => d.AttributeType == typeof(TestFrameworkAttribute));

        Assert.NotNull(data);
        Assert.Equal("GXX.M2Server.Tests.M2ConfigIsolationFramework", (string)data!.ConstructorArguments[0].Value);
        Assert.Equal("GXX.M2Server.Tests", (string)data.ConstructorArguments[1].Value);

        // 本用例本身已经被隔离壳包过（RunAsync 里自增）。
        Assert.True(M2ConfigIsolationTestCase.WrappedCaseCount > 0);
    }

    /// <summary>覆盖度守卫：每个可赋值静态成员（字段/自动属性）都必须被反射纳入，堵死「将来加字段就失效」。</summary>
    [Fact]
    public void Coverage_IncludesEveryAssignableStaticMember()
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic
                                 | BindingFlags.Static | BindingFlags.DeclaredOnly;

        var keys = new HashSet<string>(M2ConfigIsolationState.SlotKeys, StringComparer.Ordinal);
        var missing = new List<string>();

        foreach (var type in M2ConfigIsolationState.CoveredTypes)
        {
            foreach (var field in type.GetFields(Flags))
            {
                if (field.IsLiteral) continue;
                if (field.IsInitOnly && !M2ConfigIsolationState.IsSnapshotArray(field.FieldType)) continue;
                if (field.Name.EndsWith("k__BackingField", StringComparison.Ordinal)) continue;
                if (!keys.Contains(type.Name + "." + field.Name)) missing.Add($"{type.Name}.{field.Name} (field)");
            }

            foreach (var property in type.GetProperties(Flags))
            {
                if (property.GetMethod == null || property.SetMethod == null) continue;
                if (property.GetIndexParameters().Length != 0) continue;
                if (!property.GetMethod.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                    continue;   // 手写 getter 的属性按设计不接管（可能有副作用）
                if (!keys.Contains(type.Name + "." + property.Name)) missing.Add($"{type.Name}.{property.Name} (property)");
            }
        }

        Assert.Empty(missing);
    }

    /// <summary>覆盖规模守卫：M2Config 与总槽位数不应异常缩水（例如反射口径被改坏）。</summary>
    [Fact]
    public void Coverage_HasExpectedScale()
    {
        var keys = M2ConfigIsolationState.SlotKeys;
        var m2ConfigSlots = keys.Count(k => k.StartsWith("M2Config.", StringComparison.Ordinal));
        var total = keys.Count;
        var types = M2ConfigIsolationState.CoveredTypes.Count;
        var assignable = M2ConfigIsolationState.AssignableSlotCount;
        var report = $"types={types} total={total} assignable={assignable} M2Config={m2ConfigSlots}";

        Assert.True(types >= 20, report);
        Assert.True(m2ConfigSlots >= 900, report);
        Assert.True(total >= 1000, report);
        Assert.True(assignable >= 900, report);
    }

    /// <summary>无改动时 Restore 必须一个成员都不写（幂等 + 不误伤）。</summary>
    [Fact]
    public void Restore_IsNoOp_WhenNothingChanged()
    {
        var snapshot = M2ConfigIsolationState.Capture();

        Assert.Equal(0, M2ConfigIsolationState.Restore(snapshot));
        Assert.Equal(0, M2ConfigIsolationState.Restore(snapshot));
        Assert.Equal(0, M2ConfigIsolationState.Restore(M2ConfigIsolationState.Capture()));
    }

    /// <summary>逐项复位：标量 / 字符串 / 可空 / 委托引用 / 静态自动属性 / null 双向。</summary>
    [Fact]
    public void Restore_ResetsScalarsStringsNullsAndProperties()
    {
        var snapshot = M2ConfigIsolationState.Capture();

        var envirDir = M2Config.sEnvirDir;
        var boxsFile = M2Config.sBoxsFile;
        var serverNumber = M2Config.nServerNumber;
        var testServer = M2Config.boTestServer;
        var nextAnswer = M2Forms.NextAnswer;
        var handler = M2Forms.MessageBoxHandler;
        var dbName = M2ShareState.g_sDBName;
        var tickCount = DbLayerGlobals.GetTickCount;

        try
        {
            M2Config.sEnvirDir = "D:\\Mir\\Envir\\";
            M2Config.sBoxsFile = null;
            M2Config.nServerNumber = 4242;
            M2Config.boTestServer = !testServer;
            M2Forms.NextAnswer = 6;
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDNO;
            M2ShareState.g_sDBName = "PollutedDb";
            DbLayerGlobals.GetTickCount = () => 12345u;
        }
        finally
        {
            Assert.True(M2ConfigIsolationState.Restore(snapshot) >= 8);
        }

        Assert.Equal(envirDir, M2Config.sEnvirDir);
        Assert.Equal(boxsFile, M2Config.sBoxsFile);
        Assert.Equal(serverNumber, M2Config.nServerNumber);
        Assert.Equal(testServer, M2Config.boTestServer);
        Assert.Equal(nextAnswer, M2Forms.NextAnswer);
        Assert.Same(handler, M2Forms.MessageBoxHandler);
        Assert.Equal(dbName, M2ShareState.g_sDBName);
        Assert.Same(tickCount, DbLayerGlobals.GetTickCount);
    }

    /// <summary>null → 非 null 的反向复位（任务书要求「含 null 与默认值」）。</summary>
    [Fact]
    public void Restore_PutsBackCapturedNull()
    {
        var original = M2Config.sItemDropLogDir;
        try
        {
            M2Config.sItemDropLogDir = null;
            var snapshot = M2ConfigIsolationState.Capture();   // 快照里是 null

            M2Config.sItemDropLogDir = "polluted";

            Assert.Equal(1, M2ConfigIsolationState.Restore(snapshot));
            Assert.Null(M2Config.sItemDropLogDir);
        }
        finally
        {
            M2Config.sItemDropLogDir = original;
        }
    }

    /// <summary>数组元素级复位：只读数组原地回填（保持引用恒等）+ 可写数组换实例后整实例复位。</summary>
    [Fact]
    public void Restore_ResetsArrayElementsAndReferences()
    {
        var snapshot = M2ConfigIsolationState.Capture();

        var clientConfigs = M2Config.ClientConfigs;                    // static readonly bool[]
        var clientConfig0 = M2Config.ClientConfigs[0];
        var dropRates = M2Config.DieDropUseItemRates;                  // static int[]（可写）
        var dropRate0 = M2Config.DieDropUseItemRates[0];
        var levelExpRate0 = M2Config.LevelExpRates[3];                 // static uint[1001]

        try
        {
            M2Config.ClientConfigs[0] = !clientConfig0;
            M2Config.DieDropUseItemRates[0] = dropRate0 + 7;
            M2Config.LevelExpRates[3] = levelExpRate0 + 5;
            M2Config.DieDropUseItemRates = new[] { 999, 998, 997 };
        }
        finally
        {
            Assert.True(M2ConfigIsolationState.Restore(snapshot) >= 3);
        }

        Assert.Same(clientConfigs, M2Config.ClientConfigs);
        Assert.Equal(clientConfig0, M2Config.ClientConfigs[0]);
        Assert.Same(dropRates, M2Config.DieDropUseItemRates);
        Assert.Equal(dropRate0, M2Config.DieDropUseItemRates[0]);
        Assert.Equal(levelExpRate0, M2Config.LevelExpRates[3]);
    }

    /// <summary>构建期不该有任何成员被静默丢弃；运行期不该有任何读写异常。</summary>
    [Fact]
    public void Diagnostics_AreEmpty()
    {
        // 构建期诊断里只允许「明确排除」的两类（const / static readonly 非数组）。
        var unexpected = M2ConfigIsolationState.CoverageDiagnostics
            .Where(d => !d.Contains("skipped (const/literal)", StringComparison.Ordinal)
                     && !d.Contains("skipped (static readonly, non-array)", StringComparison.Ordinal))
            .ToArray();
        Assert.Empty(unexpected);

        Assert.Empty(M2ConfigIsolationState.RuntimeDiagnostics);
    }
}

using Xunit;

// M2Config 为静态全局配置（对应 Delphi g_Config），多测试类并行会互相污染，故禁用并行
[assembly: CollectionBehavior(DisableTestParallelization = true)]

// 程序集级静态全局隔离：每个测试用例前后快照/还原 M2Config 等静态全局（车道 p6-test-isolation）。
// 实现见 M2ConfigIsolationFramework.cs / M2ConfigIsolationState.cs / M2ConfigIsolationCoverage.cs。
[assembly: TestFramework("GXX.M2Server.Tests.M2ConfigIsolationFramework", "GXX.M2Server.Tests")]

using Xunit;

namespace GXX.Integration.Tests;

/// <summary>集成测试共用串行集合：两套链路测试都使用临时端口，禁止并行执行以消除端口抢占竞态。</summary>
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class SequentialCollection
{
}

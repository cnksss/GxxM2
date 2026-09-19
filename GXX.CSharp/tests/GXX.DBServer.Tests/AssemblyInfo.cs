using Xunit;

// DBShareSeam / FdbExploreSeam / UiSeam 都是 Delphi unit 级全局量的对应物（静态），
// 测试之间会互相污染 —— 关闭 xunit 的并行执行，并统一在 TestReset 里复位。
[assembly: CollectionBehavior(DisableTestParallelization = true)]

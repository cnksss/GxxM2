using Xunit;

// M2Config 为静态全局配置（对应 Delphi g_Config），多测试类并行会互相污染，故禁用并行
[assembly: CollectionBehavior(DisableTestParallelization = true)]

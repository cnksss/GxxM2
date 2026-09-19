using Xunit;

// Client 场景层测试共享 MagicEffEnv/SceneTime 等静态接缝（对应 Delphi 全局 g_MySelf/场景单例），
// 类间并行会互相污染静态状态 → 全程序集禁用并行（与 Delphi 单实例语义一致）。
[assembly: CollectionBehavior(DisableTestParallelization = true)]

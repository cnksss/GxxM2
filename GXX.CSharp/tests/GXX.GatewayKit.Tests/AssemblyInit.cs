using System.Runtime.CompilerServices;

// ============================================================================
// 集成方新增（台账 §52.1）：测试程序集的**编码初始化**
//
// 为什么需要它：产品代码里有 37 处直接 `Encoding.GetEncoding(936)`（GBK），
// 它们依赖 `GXX.Core.EncodingInit` 的 `[ModuleInitializer]` **恰好已经跑过** ——
// 那是**加载顺序**依赖，不是保证。实测后果：本程序集的 `RunGateDetailTests.DesNew2_*`
// 会因为 `GXX.RunGate.DesNew2.CrcOfKey` 直取 936 而抛 `No data is available for encoding 936`。
// （该处**产品代码**已同时改为 `EncodingInit.GBK`；本文件是程序集级的双保险，
// 让本工程任何用例都不再受"谁先被触碰"影响。）
// ============================================================================
namespace GXX.GatewayKit.Tests;

internal static class AssemblyInit
{
    [ModuleInitializer]
    internal static void Init() => GXX.Core.EncodingInit.Ensure();
}

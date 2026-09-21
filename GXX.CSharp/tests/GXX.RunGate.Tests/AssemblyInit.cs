using System.Runtime.CompilerServices;

// ============================================================================
// 集成方新增（台账 §52.1）：测试程序集的**编码初始化**
//
// 为什么需要它：产品代码里有 37 处直接 `Encoding.GetEncoding(936)`（GBK），
// 它们依赖 `GXX.Core.EncodingInit` 的 `[ModuleInitializer]` **恰好已经跑过** ——
// 那是**加载顺序**依赖，不是保证。实测后果：只跑某个测试类时（没有任何先触碰 GXX.Core 的用例）
// 直接抛 `No data is available for encoding 936`，而整轮跑又可能"恰好绿" ⇒ **次序性假绿/假红**。
//
// 本文件把注册提前到**本测试程序集加载时**，因此与用例顺序、与其它程序集是否被触碰都无关。
// 产品侧的同源问题（35 处直取 936）已登记为待办，见台账 §52.1。
// ============================================================================
namespace GXX.RunGate.Tests;

internal static class AssemblyInit
{
    [ModuleInitializer]
    internal static void Init() => GXX.Core.EncodingInit.Ensure();
}

// 源单元：Source/M2Engine/MemoryModuleEx.pas（审计统计 712 行 / 实际物理 815 行）
// 本文件 = MemoryModuleEx 的 **Stub（按《转换开发文档》§2.3「不移植项」处理）**。
//
// 【为什么只做 Stub —— 理由（§2.3 + 本单元特性）】
//   该单元是 Joachim Bauch 的 **MemoryModule**（C 版 0.0.4 → Delphi 移植 Fr0sT-Brutal，
//   见原文 :12-53 的 MPL-2.0 版权头）—— 一个**纯 Windows 原生 PE 加载器**：
//   把 DLL/EXE 映像从**内存**直接映射并完成重定位、导入表解析、TLS 回调、异常目录注册，
//   最终用 `DllMain`/`LoadLibrary` 语义在**进程地址空间里伪造一个已加载模块**。
//   它的实现 100% 是原生语义，托管运行时**不存在对应物**，逐行移植既不可能也无意义：
//     1. `VirtualAlloc`/`VirtualProtect`/`VirtualFree` 直接操作页属性（RWX/RO/RW）；
//     2. 手工解析 `IMAGE_DOS_HEADER` / `IMAGE_NT_HEADERS` / `IMAGE_SECTION_HEADER`
//        / `IMAGE_BASE_RELOCATION` / `IMAGE_IMPORT_DESCRIPTOR` / `IMAGE_TLS_DIRECTORY`
//        / `IMAGE_LOAD_CONFIG_DIRECTORY`，并**按 RVA 原地改写内存**；
//     3. 用 `Pointer(UintPtr(Base) + RVA)` 做基址重定位（原文大量 `PBYTE`/`PDWORD` 算术）；
//     4. `MemoryGetProcAddress` 返回的是**可被 `stdcall` 直接调用**的原生函数指针
//        —— 托管侧无法产出这种东西（.NET 不允许构造任意可执行原生入口）；
//     5. 原文自身也标注了未实现部分（:51「Resource loading and exe loading, custom functions,
//        user data not implemented yet」）。
//   ⇒ 按 §2.3「与业务逻辑无关、托管运行时不存在对应语义」→ **保留签名与结构、实现抛
//      NotSupportedException**，并在下方逐项登记「若将来真需要，正确的托管替代路径是什么」。
//
// 【原文对照（接口段，全部保留为签名）】
//   MemoryModuleEx.pas:61      TMemoryModule = THandle
//   MemoryModuleEx.pas:68      MemoryModuleLoadLibrary  → MemoryLoadLibrary(Data: Pointer): TMemoryModule; stdcall
//   MemoryModuleEx.pas:70      MemoryGetProcAddress(Module: TMemoryModule; const Name: PAnsiChar): Pointer; stdcall
//   MemoryModuleEx.pas:72      MemoryFreeLibrary(Module: TMemoryModule); stdcall
//   （接口段只有这三函数 + 一个类型别名；其余 815 行全在 implementation 段的原生实现里）
//
// 【托管替代路径（若将来确有「插件需从内存加载」的需求）】
//   * 从字节数组加载托管程序集 → `System.Reflection.Assembly.Load(byte[])`（真正的托管等价物）；
//   * 需要隔离/卸载 → `AssemblyLoadContext`（.NET Core 起支持可卸载 ALC）；
//   * 原生 DLL 只能从文件加载 → `NativeLibrary.Load(path)`（.NET 5+），**不支持纯内存映像**。
//   * 本项目既有的插件加载走的是 `PluginsAssemblyLoader`（见 src/GXX.M2Server/Plugins/**），
//     与本单元无关，不要误接。
//
// 【路径隔离说明】放在 Sweep/ 只是为了与顺序会话的 src/GXX.M2Server/** 常驻区物理隔离。

using System;

namespace GXX.M2Server.Sweep;

/// <summary>
/// 原文 <c>MemoryModuleEx.pas:61</c> <c>TMemoryModule = THandle</c>。
/// 托管侧用密封类表示「已加载的内存模块」句柄（原文是不透明整数句柄）。
/// </summary>
public sealed class TMemoryModule
{
    /// <summary>原文句柄值（原文即 THandle；Stub 侧仅用于标识，不承载任何原生语义）。</summary>
    public IntPtr Handle { get; }

    internal TMemoryModule(IntPtr handle) => Handle = handle;

    /// <summary>原文 <c>MemoryFreeLibrary</c> 会释放该句柄；此处仅作标识与展示。</summary>
    public override string ToString() => "TMemoryModule(0x" + Handle.ToInt64().ToString("X") + ")";
}

/// <summary>
/// 原文 <c>MemoryModuleEx.pas</c> 的三个 <c>stdcall</c> 导出函数
/// （<c>implementation</c> 段是 815 行原生 PE 加载代码，按 §2.3 不移植）。
///
/// <para><b>全部成员抛 <see cref="NotSupportedException"/></b>：签名与结构保留，便于审计与
/// 后续按需替换；任何调用都会**立即显式失败**，绝不静默返回伪值
/// （静默返回 <c>IntPtr.Zero</c> 会让调用方误判为「加载成功但取不到符号」，掩盖真实不兼容）。</para>
/// </summary>
public static class MemoryModuleEx
{
    /// <summary>不适用的统一说明（异常消息里带上，便于运行期一眼定位）。</summary>
    private const string Reason =
        "MemoryModuleEx.pas 是 Windows 原生 PE 内存加载器（MPL-2.0，Joachim Bauch / Fr0sT-Brutal），" +
        "按《转换开发文档》§2.3 列为不移植项：托管运行时不存在等价语义" +
        "（无法 VirtualAlloc/VirtualProtect 伪造已加载模块，也无法返回可被 stdcall 调用的原生函数指针）。" +
        "托管替代：Assembly.Load(byte[]) / AssemblyLoadContext / NativeLibrary.Load(path)。";

    /// <summary>
    /// 原文 <c>MemoryModuleEx.pas:68</c>
    /// <c>function MemoryLoadLibrary(Data: Pointer): TMemoryModule; stdcall;</c>
    /// <para>原文语义：把 <paramref name="data"/> 指向的内存在进程内映射为一个可用的 DLL 模块；
    /// 失败返回 <c>nil</c>。</para>
    /// </summary>
    /// <param name="data">原始 PE 映像字节（原文是裸指针）。</param>
    public static TMemoryModule? MemoryLoadLibrary(byte[] data) => throw new NotSupportedException(Reason);

    /// <summary>原文指针重载（<c>Data: Pointer</c>）。</summary>
    public static TMemoryModule? MemoryLoadLibrary(byte[] data, int offset) => throw new NotSupportedException(Reason);

    /// <summary>
    /// 原文 <c>MemoryModuleEx.pas:70</c>
    /// <c>function MemoryGetProcAddress(Module: TMemoryModule; const Name: PAnsiChar): Pointer; stdcall;</c>
    /// <para>原文语义：在内存模块的导出表里查名字，返回**可直接调用的原生函数指针**。</para>
    /// </summary>
    public static IntPtr MemoryGetProcAddress(TMemoryModule module, string name)
        => throw new NotSupportedException(Reason);

    /// <summary>原文 <c>const Name: PAnsiChar</c> 的字节重载。</summary>
    public static IntPtr MemoryGetProcAddress(TMemoryModule module, byte[] name)
        => throw new NotSupportedException(Reason);

    /// <summary>
    /// 原文 <c>MemoryModuleEx.pas:72</c>
    /// <c>procedure MemoryFreeLibrary(Module: TMemoryModule); stdcall;</c>
    /// <para>原文语义：卸载模块（解除映射、释放虚拟内存）。</para>
    /// </summary>
    public static void MemoryFreeLibrary(TMemoryModule module) => throw new NotSupportedException(Reason);

    /// <summary>
    /// 原文「是否可用」在托管侧恒为 <c>false</c>（供调用方做能力探测，避免靠捕获异常分流）。
    /// </summary>
    public static bool IsSupported => false;
}

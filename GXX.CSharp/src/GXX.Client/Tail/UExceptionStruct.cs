// 源单元：Source/Client-HGE/uExceptionStruct.pas（原文 134 行，CRLF 计入 153 行）
// 原文 uses：Windows
// 原文无同名 .dfm。
//
// 覆盖事实：uExceptionStruct **确实被 Client.dpr 引用**（Client.dpr:104），
//   并被 ClMain.pas:88 / ClMain20230516.pas:16 的 uses 列表引用；但两个 ClMain 都**只**
//   引用该单元，源码树里没有任何一处使用它的类型或常量（已全树 grep 核对：
//   `TExcDesc` / `TExcFrame` / `EXCEPTION_DISPOSITION` / `EH_NONCONTINUABLE` 等
//   在 uExceptionStruct.pas 之外零出现）。它是一份「引而不入」的声明集合。
//
// ── 转换开发文档 §2.3「不移植项」登记 ──────────────────────────────────────
// 该单元通篇是 **Win32 SEH / VEH 的 32 位异常展开框架结构体**：
//   `TExcDesc`（Delphi 异常表描述）+ 跳转指令 `TJmpInstruction{opCode; distance}`
//   + `TExcFrame`（SEH 链帧）+ `_ExceptionHandler` / `_ExceptionRegistration`
//   （链表注册结构）+ `EXCEPTION_DISPOSITION` 与 `EXCEPTION_EXECUTE_HANDLER` 等常量。
// 其 `PExcFrame` 的 `hEBP` 字段、`case Integer of ... ConstructedObject/SelfOfMethod`
// 变体记录，都是 **x86 栈帧/寄存器级**布局，只对原生 SEH 展开器有意义。
// 托管运行时：① CLR 有自己的两阶段异常展开（Windows 上走 SEH 但结构体由 CLR 私有持有）；
//   ② 托管代码无法注册原生 SEH 处理器（`TSEHExceptionHandler` 的 cdecl 回调没有
//   可安全调用的托管对应）；③ x86 专用的 `hEBP` 在 AnyCPU/x64 下没有意义。
// 故按 §2.3 第 2 条做 **Stub**：保留全部类型名与字段名（布局按原文 packed 声明保留，
// 便于日后做结构体语义对照），保留全部常量与其精确取值，**不提供**任何注册/派发函数。
// ──────────────────────────────────────────────────────────────────────────
using System;
using System.Runtime.InteropServices;

namespace GXX.Client.Tail;

/// <summary>
/// uExceptionStruct.pas 1:1 类型声明移植（Stub 形态，见文件头 §2.3 登记）。
/// <para>命名空间与本车道其它单元一致（<c>GXX.Client.Tail</c>）。
/// 类型名前缀 <c>T</c> 与常量名原样保留，便于与 Delphi 源逐条对照。</para>
/// </summary>
public static class UExceptionStruct
{
    // ── 常量（uExceptionStruct.pas:138-149）────────────────────────────────

    /// <summary>原文 :140 — <c>EXCEPTION_EXECUTE_HANDLER = 1;</c>（表示我已经处理了异常，可以优雅地结束了）</summary>
    public const int EXCEPTION_EXECUTE_HANDLER = 1;

    /// <summary>原文 :141 — <c>EXCEPTION_CONTINUE_SEARCH = 0;</c>（表示我不处理，其他人来吧）</summary>
    public const int EXCEPTION_CONTINUE_SEARCH = 0;

    /// <summary>
    /// 原文 :142 — <c>EXCEPTION_CONTINUE_EXECUTION = -1;</c>（表示忽略此异常，请从异常发生处继续执行）
    /// <para>⚠ 注意这是**唯一取负值**的成员，与上面两个常量同属「非 SEH 异常返回值」一组。
    /// 用 <c>!= -1</c> 之类哨兵值判成功会踩坑，故此处保留精确的 <c>int</c> 类型。</para>
    /// </summary>
    public const int EXCEPTION_CONTINUE_EXECUTION = -1;

    /// <summary>原文 :144 — <c>EH_NONE = $0;</c></summary>
    public const uint EH_NONE = 0x0;

    /// <summary>原文 :145 — <c>EH_NONCONTINUABLE = $1;</c></summary>
    public const uint EH_NONCONTINUABLE = 0x1;

    /// <summary>原文 :146 — <c>EH_UNWINDING = $2;</c></summary>
    public const uint EH_UNWINDING = 0x2;

    /// <summary>原文 :147 — <c>EH_EXIT_UNWIND = $4;</c></summary>
    public const uint EH_EXIT_UNWIND = 0x4;

    /// <summary>原文 :148 — <c>EH_STACK_INVALID = $8;</c></summary>
    public const uint EH_STACK_INVALID = 0x8;

    /// <summary>原文 :149 — <c>EH_NESTED_CALL = $10;</c></summary>
    public const uint EH_NESTED_CALL = 0x10;

    // ── 常量表的可测快照 ──────────────────────────────────────────────────

    /// <summary>原文 :138-149 全部常量的 (名, 值) 快照，供指纹测试锁死。</summary>
    public static readonly (string Name, long Value)[] ConstantFingerprint = new[]
    {
        ("EXCEPTION_EXECUTE_HANDLER", (long)EXCEPTION_EXECUTE_HANDLER),
        ("EXCEPTION_CONTINUE_SEARCH", (long)EXCEPTION_CONTINUE_SEARCH),
        ("EXCEPTION_CONTINUE_EXECUTION", (long)EXCEPTION_CONTINUE_EXECUTION),
        ("EH_NONE", (long)EH_NONE),
        ("EH_NONCONTINUABLE", (long)EH_NONCONTINUABLE),
        ("EH_UNWINDING", (long)EH_UNWINDING),
        ("EH_EXIT_UNWIND", (long)EH_EXIT_UNWIND),
        ("EH_STACK_INVALID", (long)EH_STACK_INVALID),
        ("EH_NESTED_CALL", (long)EH_NESTED_CALL),
    };
}

/// <summary>原文 :124-129 — <c>EXCEPTION_DISPOSITION</c> 枚举（顺序与取值原样保留）。</summary>
public enum EXCEPTION_DISPOSITION
{
    /// <summary>原文注释：恢复寄存器，继续执行</summary>
    ExceptionContinueExecution = 0,
    /// <summary>原文注释：调用处理链表中下一个处理函数</summary>
    ExceptionContinueSearch = 1,
    /// <summary>原文注释：函数中出发了新的异常</summary>
    ExceptionNestedException = 2,
    /// <summary>原文注释：发生了嵌套展开操作</summary>
    ExceptionCollidedUnwind = 3,
}

/// <summary>原文 :21-24 — <c>TJmpInstruction = packed record opCode:Byte; distance:Longint; end;</c>（5 字节）</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TJmpInstruction
{
    /// <summary>原文 :22 — <c>opCode:Byte;</c></summary>
    public byte opCode;
    /// <summary>原文 :23 — <c>distance:Longint;</c></summary>
    public int distance;
}

/// <summary>原文 :27-30 — <c>TExcDescEntry = record vTable:Pointer; handler:Pointer; end;</c></summary>
[StructLayout(LayoutKind.Sequential)]
public struct TExcDescEntry
{
    /// <summary>原文 :28 — <c>vTable:Pointer;</c></summary>
    public IntPtr vTable;
    /// <summary>原文 :29 — <c>handler:Pointer;</c></summary>
    public IntPtr handler;
}

/// <summary>
/// 原文 :32-37 — <c>TExcDesc = packed record jmp:TJmpInstruction;
/// case Integer of 0:(instructions:array[0..0] of Byte); 1:(cnt:Integer; excTab:array[0..0] of TExcDescEntry); end;</c>
/// <para>变体记录的 <c>array[0..0]</c> 是「柔性数组」惯用法（实际长度由 <c>cnt</c> 决定）。
/// 托管侧的对应表示是「结构头 + 调用方按 <c>cnt</c> 走的偏移」，故这里只保留头部的
/// <c>jmp</c> 字段并给出偏移/尺寸常量。</para>
/// </summary>
public static class TExcDescLayout
{
    /// <summary><c>jmp</c> 的偏移（0）与尺寸（5）。</summary>
    public const int JmpOffset = 0;
    /// <summary><c>jmp</c> 尺寸 = <c>SizeOf(TJmpInstruction)</c> = 1 + 4 = 5。</summary>
    public const int JmpSize = 5;
    /// <summary>变体 1 的 <c>cnt:Integer</c> 偏移（紧跟 <c>jmp</c>）。</summary>
    public const int CntOffset = JmpOffset + JmpSize;
    /// <summary>变体 1 的 <c>excTab</c> 偏移。</summary>
    public const int ExcTabOffset = CntOffset + 4;
    /// <summary>变体 0 的 <c>instructions</c> 偏移（与 <c>cnt</c> 同址，原文如此）。</summary>
    public const int InstructionsOffset = CntOffset;
}

/// <summary>原文 :39-48 — <c>TExcFrame</c>（SEH 链帧；<c>hEBP</c> 是 x86 专有字段）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TExcFrame
{
    /// <summary>原文 :41 — <c>next:PExcFrame;</c></summary>
    public IntPtr next;
    /// <summary>原文 :42 — <c>desc:PExcDesc;</c></summary>
    public IntPtr desc;
    /// <summary>原文 :43 — <c>hEBP:Pointer;</c>（x86 栈帧基址；托管侧无对应含义）</summary>
    public IntPtr hEBP;
    /// <summary>原文 :44-48 的变体记录 —— 三个分支同址（偏移 24 之后的 union 起点）。
    /// 托管侧以单字段 + 三种「视图」属性表示。</summary>
    public IntPtr Union;
}

/// <summary>
/// 原文 :109-115 — <c>_ExceptionHandler</c> / <c>Exception_Handler</c>
/// （原始命名就带前导下划线，本移植保留）。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct _ExceptionHandler
{
    /// <summary>原文 :110 — <c>ExceptionRecord:PExceptionRecord;</c></summary>
    public IntPtr ExceptionRecord;
    /// <summary>原文 :111 — <c>SEH:PException_Registration;</c></summary>
    public IntPtr SEH;
    /// <summary>原文 :112 — <c>Context:PContext;</c></summary>
    public IntPtr Context;
    /// <summary>原文 :113 — <c>DispatcherContext:Pointer;</c></summary>
    public IntPtr DispatcherContext;
}

/// <summary>原文 :117-120 — <c>_ExceptionRegistration</c>（SEH 链表注册结构）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct _ExceptionRegistration
{
    /// <summary>原文 :118 — <c>Prev:PException_Registration;</c></summary>
    public IntPtr Prev;
    /// <summary>原文 :119 — <c>Handler:PExecption_Handler;</c>（原文拼写即 <c>PExecption</c>，非 <c>PException</c>）</summary>
    public IntPtr Handler;
}

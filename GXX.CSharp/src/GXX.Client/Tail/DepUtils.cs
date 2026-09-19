// 源单元：Source/Client-HGE/DepUtils.pas（原文 84 行，CRLF 计入 101 行）
// 原文 uses：Windows
// 原文无同名 .dfm。
//
// ⚠ 覆盖事实（已核）：DepUtils 单元**不在 Client.dpr 的 uses 列表中**
//   （Source/Client-HGE/Client.dpr:15-113 逐行核对，仅 uAntiPlug / uExceptionStruct /
//    IECache / ClientBuff / uWeatherEffectDef / CheckProcessModules / uDropItemEffectList /
//    uFrmNGItemEdit / NPCFormDeBug 等被引用）。它是一份孤儿单元，整棵源码树无引用点。
//
// ── 转换开发文档 §2.3「不移植项」登记 ──────────────────────────────────────
// 原文 `SetCurrentProcessDEP` 直接改写本进程的 DEP/NX 执行策略：
//   先试 kernel32!SetProcessDEPPolicy，失败再退到 ntdll!NtSetInformationProcess(
//   ProcessExecuteFlags) 写 MEM_EXECUTE_OPTION_* 标志；单元的 initialization 段
//   无条件调用 `SetCurrentProcessDEP(DEP_DISABLED)` —— 即**主动关闭本进程的 DEP**。
//   Google Chrome 的沙箱代码里这是「为兼容老 ATL7 thunk 而按需关闭 DEP」，
//   Delphi 侧被照搬过来在启动时全局关掉 DEP。
// 托管运行时下该能力：① 无 .NET 对应 API；② 关闭进程 DEP 会削弱所有托管代码的
//   安全基线（.NET 依赖 DEP 做 JIT 页保护）；③ 原文调用点在 initialization，
//   .NET 的 ModuleInitializer 时机与之并不等价。故按 §2.3 做 **Stub**：
//   保留 `SetCurrentProcessDEP` 的方法签名与 `DepEnforcement` 枚举（含原值顺序），
//   保留全部常量，函数恒返回原文的「未生效」取值 False。
// ──────────────────────────────────────────────────────────────────────────
using System;

namespace GXX.Client.Tail;

/// <summary>
/// DepUtils.pas 1:1 移植（Stub 形态，见文件头 §2.3 登记）。
/// <para>原文 <c>DepEnforcement</c> 取值顺序必须原样保留（DEP_DISABLED=0 / DEP_ENABLED=1 /
/// DEP_ENABLED_ATL7_COMPAT=2），因为后续 <c>dep_flags</c> 的 case 分支与之一一对应。</para>
/// </summary>
public enum DepEnforcement
{
    /// <summary>原文注释：DEP is completely disabled.</summary>
    DEP_DISABLED = 0,
    /// <summary>原文注释：DEP is permanently enforced.</summary>
    DEP_ENABLED = 1,
    /// <summary>原文注释：DEP with support for ATL7 thunking is permanently enforced.</summary>
    DEP_ENABLED_ATL7_COMPAT = 2,
}

/// <summary>DepUtils.pas 的常量与（Stub 的）入口函数。</summary>
public static class DepUtils
{
    // ── 原文 implementation 段常量（DepUtils.pas:26-31）──────────────────
    /// <summary>原文 DepUtils.pas:26 — <c>PROCESS_DEP_ENABLE:DWORD = $00000001;</c></summary>
    public const uint PROCESS_DEP_ENABLE = 0x00000001;

    /// <summary>原文 DepUtils.pas:27 — <c>PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION:DWORD = $00000002;</c></summary>
    public const uint PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION = 0x00000002;

    /// <summary>原文 DepUtils.pas:28 — <c>MEM_EXECUTE_OPTION_ENABLE:DWORD = 1;</c></summary>
    public const uint MEM_EXECUTE_OPTION_ENABLE = 1;

    /// <summary>原文 DepUtils.pas:29 — <c>MEM_EXECUTE_OPTION_DISABLE:DWORD = 2;</c></summary>
    public const uint MEM_EXECUTE_OPTION_DISABLE = 2;

    /// <summary>原文 DepUtils.pas:30 — <c>MEM_EXECUTE_OPTION_ATL7_THUNK_EMULATION:DWORD = 4;</c></summary>
    public const uint MEM_EXECUTE_OPTION_ATL7_THUNK_EMULATION = 4;

    /// <summary>原文 DepUtils.pas:31 — <c>MEM_EXECUTE_OPTION_PERMANENT:DWORD = 8;</c></summary>
    public const uint MEM_EXECUTE_OPTION_PERMANENT = 8;

    /// <summary>原文 DepUtils.pas:34 — <c>PROCESS_INFORMATION_CLASS = (ProcessExecuteFlags = $22);</c></summary>
    public const int ProcessExecuteFlags = 0x22;

    /// <summary>
    /// 原文 DepUtils.pas:44-96 <c>function SetCurrentProcessDEP(enforcement: DepEnforcement): Boolean;</c>
    ///
    /// <para><b>原文的 dep_flags 取值表（Stub 不执行，但语义等价性由测试锁住）</b>：</para>
    /// <list type="table">
    /// <item><term>DEP_DISABLED</term><description>SetProcessDEPPolicy 分支 <c>0</c>；
    ///   NtSetInformationProcess 分支 <c>MEM_EXECUTE_OPTION_DISABLE</c>=2</description></item>
    /// <item><term>DEP_ENABLED</term><description><c>PROCESS_DEP_ENABLE | PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION</c>=3；
    ///   ntdll 分支 <c>MEM_EXECUTE_OPTION_PERMANENT | MEM_EXECUTE_OPTION_ENABLE</c>=9</description></item>
    /// <item><term>DEP_ENABLED_ATL7_COMPAT</term><description><c>PROCESS_DEP_ENABLE</c>=1；
    ///   ntdll 分支 <c>9 | MEM_EXECUTE_OPTION_ATL7_THUNK_EMULATION</c>=13</description></item>
    /// </list>
    /// <para>原文 <c>else Exit;</c> 分支在 Delphi 里会以 <c>Result</c> 当时的取值（函数入口赋的
    /// <c>False</c>）直接返回 —— 枚举越界即"未生效"。</para>
    ///
    /// <para><b>Stub 取值</b>：恒返回 <c>false</c>（原文未设置成功时的取值）。
    /// 注意原文 initialization 段（DepUtils.pas:98-99）会调用
    /// <c>SetCurrentProcessDEP(DEP_DISABLED)</c>；托管侧**刻意不执行**
    /// （见文件头 §2.3 理由 ②）。</para>
    /// </summary>
    /// <param name="enforcement">目标 DEP 策略（枚举顺序原样保留）。</param>
    /// <returns>原文「未生效」取值 <c>False</c>。</returns>
    public static bool SetCurrentProcessDEP(DepEnforcement enforcement)
    {
        // 原文如此（DepUtils.pas:52）：Result := False;
        // 接缝：待 DepUtils 的 DEP 写入路径（SetProcessDEPPolicy / NtSetInformationProcess）
        //       以 P/Invoke 落地后接入；本 Stub 不修改进程 DEP 策略。
        _ = enforcement;
        return false;
    }

    /// <summary>
    /// 原文 DepUtils.pas:57-63 / 75-85 的 <c>dep_flags</c> 计算（**纯逻辑**，已抽成可测函数）。
    /// 原文两处 case 落在两个不同 API 分支上，故此方法以 <paramref name="useNtdllFallback"/>
    /// 选择分支；枚举越界时返回 <c>null</c>（对应原文的 <c>else Exit;</c>）。
    /// </summary>
    /// <param name="enforcement">策略。</param>
    /// <param name="useNtdllFallback">false = SetProcessDEPPolicy 分支；true = NtSetInformationProcess 分支。</param>
    public static uint? ComputeDepFlags(DepEnforcement enforcement, bool useNtdllFallback)
    {
        if (!useNtdllFallback)
        {
            // 原文 DepUtils.pas:57-63
            switch (enforcement)
            {
                case DepEnforcement.DEP_DISABLED: return 0;
                case DepEnforcement.DEP_ENABLED:
                    return PROCESS_DEP_ENABLE | PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION;
                case DepEnforcement.DEP_ENABLED_ATL7_COMPAT: return PROCESS_DEP_ENABLE;
                default: return null; // 原文 `else Exit;`
            }
        }

        // 原文 DepUtils.pas:75-85
        switch (enforcement)
        {
            case DepEnforcement.DEP_DISABLED: return MEM_EXECUTE_OPTION_DISABLE;
            case DepEnforcement.DEP_ENABLED:
                return MEM_EXECUTE_OPTION_PERMANENT | MEM_EXECUTE_OPTION_ENABLE;
            case DepEnforcement.DEP_ENABLED_ATL7_COMPAT:
                // 原文如此（DepUtils.pas:80-82）：三项按顺序 or 起来，分行书写。
                return MEM_EXECUTE_OPTION_PERMANENT
                     | MEM_EXECUTE_OPTION_ENABLE
                     | MEM_EXECUTE_OPTION_ATL7_THUNK_EMULATION;
            default: return null; // 原文 `else Exit;`
        }
    }
}

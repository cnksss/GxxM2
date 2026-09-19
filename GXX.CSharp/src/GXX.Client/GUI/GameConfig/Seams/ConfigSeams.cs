using System;

namespace GXX.Client.GUI.GameConfig.Seams;

/// <summary>
/// GameConfig 配置对话框族的**外部依赖接缝**。
///
/// 这些符号在 Delphi 侧来自 DxComponents.pas / Grobal2.pas / MShare.pas，
/// 在 C# 侧尚未移植。按工程规程（"未移植的依赖绝不要顺手移植：定义最小接缝并注释"）：
/// 本文件只声明配置对话框族真正用到的那一小部分，**不做任何推测性移植**；
/// 待对应单元移植完成后，把这里的接缝替换为真实类型即可。
///
/// 所有接缝都集中在本文件，便于审计"哪些是真实逻辑、哪些是桩"。
/// </summary>
public static class ConfigSeams
{
    /// <summary>
    /// 接缝：MShare.pas:2979/11565 的 <c>ProcessFileNameSpecialChar</c>。
    /// 原文把文件名非法字符替换为等价可见字符（'/'-&gt;'{'、'\'-&gt;'}'、':'-&gt;';'、
    /// '*'-&gt;'@'、'?'-&gt;'!'、'"'-&gt;'~'、'&lt;'-&gt;'('、'&gt;'-&gt;')'、'|'-&gt;'-'）。
    /// 因为它是**已在原文中实现的纯函数**、且 FilterItems 的落盘文件名完全依赖它，
    /// 这里按原文逐字复刻（不是桩）。
    /// </summary>
    public static string ProcessFileNameSpecialChar(string S)
    {
        if (string.IsNullOrEmpty(S)) return S ?? "";
        var sb = new System.Text.StringBuilder(S.Length);
        foreach (char ch in S)
        {
            switch (ch)
            {
                case '/': sb.Append('{'); break;
                case '\\': sb.Append('}'); break;
                case ':': sb.Append(';'); break;
                case '*': sb.Append('@'); break;
                case '?': sb.Append('!'); break;
                case '"': sb.Append('~'); break;
                case '<': sb.Append('('); break;
                case '>': sb.Append(')'); break;
                case '|': sb.Append('-'); break;
                default: sb.Append(ch); break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 接缝：MShare.pas 的 <c>function MyGetTickCount:DWORD; stdcall; external mmsyst name 'timeGetTime';</c>。
    /// 与 <c>Windows.GetTickCount</c> 同为"系统启动后毫秒数"，语义一致；
    /// 统一走 GXX.Core.Rtl.DelphiRTL.GetTickCount（uint 回绕）。
    /// </summary>
    public static uint MyGetTickCount() => GXX.Core.Rtl.DelphiRTL.GetTickCount();
}

/// <summary>
/// 接缝：DxComponents.pas:27 的 <c>TClientVersion</c>。
/// 原文枚举为
/// <c>(cv176, cv185, cvHero, cvSerial, cvMirSequel, {cvMirs, cvMirReturn,} {cvMirReturn2} cvMirNewUI205)</c>，
/// 但条件编译存在把 cvMirs/cvMirReturn/cvMirReturn2 打开的分支（MirsConfigDlg.pas:553 用到 cvMirs）。
/// 这里按"打开全部成员"的序号给出，**序号与原文顺序一致**。
/// </summary>
public enum TClientVersion
{
    cv176 = 0,
    cv185 = 1,
    cvHero = 2,
    cvSerial = 3,
    cvMirSequel = 4,
    cvMirs = 5,       // 原文此成员被 {} 注释，但 MirsConfigDlg.pas:553 使用 → 条件编译下存在
    cvMirReturn = 6,  // 同上
    cvMirReturn2 = 7, // 同上
    cvMirNewUI205 = 8,
}

/// <summary>
/// Delphi <c>TShiftState = set of (ssShift, ssAlt, ssCtrl, ssLeft, ssRight, ssMiddle, ssDouble)</c>
/// 的逐位映射。**位序即 Delphi 枚举序**（ssShift=bit0 … ssDouble=bit6），
/// 因为 ConfigShare.GetKeyDownStr 用 <c>in</c> 测试且判定顺序即原文 if-else 链顺序。
/// </summary>
[Flags]
public enum DelphiShiftState
{
    None = 0,
    ssShift = 1 << 0,
    ssAlt = 1 << 1,
    ssCtrl = 1 << 2,
    ssLeft = 1 << 3,
    ssRight = 1 << 4,
    ssMiddle = 1 << 5,
    ssDouble = 1 << 6,
}

/// <summary>
/// 接缝：Grobal2.pas:4787/4810 的 <c>TClientConfig</c>（服务端发给客户端的配置）。
/// **仅保留配置对话框族用到的字段**（GameConfigDlgs.pas:134 读 <c>btConfigDlgType</c>）；
/// 其余字段待 Grobal2.pas 完整移植后并入。
/// </summary>
public sealed class TClientConfig
{
    /// <summary>原文 TClientConfig.btConfigDlgType:Byte；0 表示走 JSY 之外的配置对话框。</summary>
    public byte btConfigDlgType;

    // 接缝：待 Grobal2.pas 的 TClientConfig 完整移植后接入其余字段
    // （boParalyCanRun/boParalyCanWalk/nMoveSpeed/nAttackSpeed/nSpellSpeed/… 以及
    //  GameConfig 侧的 ClientConfigs 数组）。
}

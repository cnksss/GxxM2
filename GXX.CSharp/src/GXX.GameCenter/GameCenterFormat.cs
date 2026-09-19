using System;
using GXX.Core.Rtl;

namespace GXX.GameCenter;

/// <summary>
/// GMain.pas 用到的 <c>System.SysUtils.Format</c> 补充语义。
/// <para>
/// Delphi 的 <c>Format('%d', [True])</c> 返回 <c>'-1'</c>（布尔按 -1/0 渲染），
/// 而 <c>GXX.Core.Rtl.DelphiFormat</c> 会按 <c>Convert.ToInt64</c> 渲染成 1/0。
/// GMain.pas:2068 <c>SaveList.Add(Format('%s %s %d', [g_sGameName, g_sGameName, g_nLimitOnlineUser]))</c>
/// 的第三个实参实际是 <c>Integer</c>，不受影响；但为与 Delphi 语义完全一致，
/// 本车道统一经由本包装调用，凡布尔实参按 -1/0 归一后再格式化。
/// </para>
/// <para>接缝说明：<c>GXX.Core.Rtl.DelphiFormat</c> 属 GXX.Core（他车道/主会话的文件），
/// 本车道不修改它，改以本包装承载差异（见交付报告"留作接缝"）。</para>
/// </summary>
public static class GameCenterFormat
{
    /// <summary>Delphi <c>Format</c>（布尔实参按 -1/0 渲染）。</summary>
    public static string Format(string fmt, params object[] args)
    {
        if (args != null)
        {
            for (int i = 0; i < args.Length; i++)
                if (args[i] is bool b) args[i] = b ? -1 : 0;
        }
        return DelphiFormat.Format(fmt, args);
    }
}

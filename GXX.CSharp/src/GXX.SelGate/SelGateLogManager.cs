using System;
using GXX.Core.Rtl;

namespace GXX.SelGate;

/// <summary>
/// SelGate LogManager.pas → SelGateLogManager.cs
/// 1:1 逐字移植 TLogMgr（:9-16、:26-53）。
/// 原文 Add 通过 WM_GETTEXTLENGTH / EM_SETSEL / EM_REPLACESEL / WM_VSCROLL 把文本追加到
/// 主窗体 RichEdit（:49-52）；托管侧以 <see cref="OnAppend"/> 事件表达同样的"向宿主追加"语义。
/// </summary>
public class CLogMgr
{
    /// <summary>LogManager.pas:10 m_hWnd: HWND（宿主窗体句柄）。</summary>
    public IntPtr m_hWnd;

    /// <summary>LogManager.pas:49-52 的 4 条 SendMessage 等价出口（Append + 滚动到底）。</summary>
    public event Action<string>? OnAppend;

    public CLogMgr(IntPtr nWnd)   // :26-29 constructor TLogMgr.Create
    {
        m_hWnd = nWnd;
    }

    /// <summary>LogManager.pas:36-39 CheckLevel：g_pConfig.m_nShowLogLevel &gt;= nShowLv。</summary>
    public bool CheckLevel(int nShowLv)
    {
        return Config!.m_nShowLogLevel >= nShowLv;
    }

    /// <summary>日志级别来源（Delphi 全局 g_pConfig；LogManager.pas:24 uses ConfigManager）。</summary>
    public CConfigMgr? Config { get; set; }

    /// <summary>LogManager.pas:41-53 Add：文本按 FormatStr = '[%s] %s'#13#10 排版（:46），TimeToStr(Now)。</summary>
    public void Add(string szMsg)
    {
        string szTempMsg = DelphiRTL.Format(FormatStr, TimeToStr(DelphiRTL.Now()), szMsg); // :48
        OnAppend?.Invoke(szTempMsg);                                                        // :49-52
    }

    /// <summary>LogManager.pas:45-46 const FormatStr = '[%s] %s'#13#10。</summary>
    public const string FormatStr = "[%s] %s\r\n";

    /// <summary>Delphi SysUtils.TimeToStr：'hh:nn:ss'（含 AM/PM 由 FormatSettings 决定，默认 24 小时制）。</summary>
    public static string TimeToStr(DateTime t) => t.ToString("HH:mm:ss");
}

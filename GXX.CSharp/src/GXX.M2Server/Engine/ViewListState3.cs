using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J38：ViewList.pas 余下 12 组名单全局表（TStringList 全局组，键名与 Delphi M2Share 一致）。
/// </summary>
public static partial class ViewListState
{
    public static readonly TStringList g_NoClearMonList = new();
    public static readonly TStringList g_MonList = new();
    public static readonly TStringList g_AdminList = new();
    public static readonly TStringList g_PreviewItemMonList = new();
}

public static class ViewListState3
{
    /// <summary>测试隔离：清空余下 4 组名单。</summary>
    public static void ResetDefaults()
    {
        ViewListState.g_NoClearMonList.Clear();
        ViewListState.g_MonList.Clear();
        ViewListState.g_AdminList.Clear();
        ViewListState.g_PreviewItemMonList.Clear();
    }
}

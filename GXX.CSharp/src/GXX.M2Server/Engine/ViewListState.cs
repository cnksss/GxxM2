using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J18/J21：ViewList.pas 名单全局表（TStringList 全局组，Lock/UnLock/Sorted 语义 1:1）。
/// 每个名单对应窗体上一组 增/全增/删/全删/保存 按钮；全局表名与 Delphi M2Share 完全一致。
/// </summary>
public static partial class ViewListState
{
    public static readonly object LockObj = new();

    public static readonly TStringList g_EnableMakeItemList = new();
    public static readonly TStringList g_DisableMakeItemList = new();

    // ---- 批次J21：其余名单组全局表（ViewList.pas Open/save 1:1 引用名） ----
    public static readonly TStringList g_GameLogItemNameList = new();
    public static readonly TStringList g_DisableTakeOffList = new();
    public static readonly TStringList g_DisableShowItemFromList = new();
    public static readonly TStringList g_DisableMoveMapList = new();
    public static readonly TStringList g_EnablePickUpItemList = new();
    public static readonly TStringList g_MoveGuardPickItemList = new();
    public static readonly TStringList g_PriorityPickUpItemList = new();
    public static readonly TStringList g_DisableRangePickItemList = new();
    public static readonly TStringList g_DisableDropToBagItemList = new();
    public static readonly TStringList g_NameFilterList = new();

    public static void ResetForTests()
    {
        foreach (var list in new[] { g_EnableMakeItemList, g_DisableMakeItemList, g_GameLogItemNameList,
                     g_DisableTakeOffList, g_DisableShowItemFromList, g_DisableMoveMapList,
                     g_EnablePickUpItemList, g_MoveGuardPickItemList, g_PriorityPickUpItemList,
                     g_DisableRangePickItemList, g_DisableDropToBagItemList, g_NameFilterList })
        {
            list.Clear();
            list.Sorted = false;
        }
    }
}

using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// 批次J19/J20：ViewList.pas 其余名单组处理器（1:1）：
/// ① GameLog 组（ListBoxitemList2 → ListBoxGameLogList → g_GameLogItemNameList Sorted + 保存时询问重载物品库）；
/// ② DisableTakeOff 组（ListBoxitemList3 → '序号  名称' AddObject 形式 → g_DisableTakeOffList 按 Idx 排序）；
/// ③ DisableShowItemFrom 组（同 DisableTakeOff 结构）；
/// ④ DisableMoveMap 组（ListBoxMapList → ListBoxDisableMoveMap）；
/// ⑤ EnablePickUp / PriorityPickUp / MoveGuardPick / DisableRangePick / DisableDropToBag / NameFilter 组
///    （均为同名 增/全增/删/全删/保存 五钮结构，映射对应全局 TStringList）。
/// </summary>
public static class ViewListGroups
{
    /// <summary>名单组共用锁定对象。</summary>
    public static readonly object GroupLockObj = new();

    /// <summary>GameLog 保存询问重载物品库的用户应答（测试注入；运行时弹窗）。</summary>
    public static bool? ReloadItemsDBAnswer;
    public static int ReloadItemsDBCalls;
    public static int LoadItemsDBCalls;

    /// <summary>通用增（选中项去重追加）——适用于 GameLog/DisableMoveMap/EnablePickUp/
    /// PriorityPickUp/MoveGuardPick/DisableRangePickItem/DisableDropToBagItem/NameFilter 等同名组。</summary>
    public static void AddSelected(TStringList source, TStringList target, int[] selected)
    {
        foreach (var i in selected)
        {
            var sItemName = source[i];
            if (target.IndexOf(sItemName) < 0)
                target.Add(sItemName);
        }
    }

    /// <summary>通用全增。</summary>
    public static void AddAll(TStringList source, TStringList target)
    {
        target.Clear();
        for (int i = 0; i < source.Count; i++)
            target.Add(source[i]);
    }

    /// <summary>通用删（按索引）。</summary>
    public static void DeleteAt(TStringList target, int index)
    {
        if (index >= 0 && index < target.Count)
            target.Delete(index);
    }

    /// <summary>通用全删。</summary>
    public static void DeleteAll(TStringList target) => target.Clear();

    /// <summary>通用保存（写回全局 TStringList 并 Sorted；调用方持锁语义由 Delphi Lock/UnLock 对应）。</summary>
    public static void Save(TStringList listBox, TStringList globalList)
    {
        lock (globalList)
        {
            try
            {
                globalList.Clear();
                for (int i = 0; i < listBox.Count; i++)
                    globalList.Add(listBox[i]);
                globalList.Sorted = true;
            }
            finally { }
        }
    }

    /// <summary>btnAddLogItemClick 1:1（选中项去重追加）。</summary>
    public static void AddLogItemClick(TStringList source, TStringList target)
        => AddSelected(source, target, Enumerable.Range(0, source.Count).ToArray());

    /// <summary>btnSaveLogItemClick 1:1（写回 g_GameLogItemNameList Sorted；询问重载物品库）。</summary>
    public static void SaveLogItemClick(TStringList listBox, TStringList globalList)
    {
        Save(listBox, globalList);

        ReloadItemsDBCalls++;
        if (ReloadItemsDBAnswer == true)
            LoadItemsDBCalls++;
    }

    /// <summary>ButtonDisableTakeOffAddClick 1:1：'序号  名称' 形式追加（AddObject 原 Index）。</summary>
    public static void DisableTakeOffAdd(TStringList source, TStringList target, int[] selected)
    {
        foreach (var i in selected)
        {
            var sItemName = i + "  " + source[i];
            if (target.IndexOf(sItemName) < 0)
                target.AddObject(sItemName, i);
        }
    }

    /// <summary>ButtonDisableTakeOffAddAllClick 1:1。</summary>
    public static void DisableTakeOffAddAll(TStringList source, TStringList target)
    {
        target.Clear();
        for (int i = 0; i < source.Count; i++)
            target.AddObject(i + "  " + source[i], i);
    }
}

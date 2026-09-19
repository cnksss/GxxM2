using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J20：ViewListGroups 通用名单组处理器测试（增去重/全增/删/全删/保存 Sorted）。</summary>
public sealed class ViewListGroupsTests
{
    private static TStringList MakeList(params string[] items)
    {
        var list = new TStringList();
        foreach (var i in items)
            list.Add(i);
        return list;
    }

    [Fact]
    public void AddSelected_Dedupes()
    {
        var source = MakeList("A", "B", "C");
        var target = MakeList("A");
        ViewListGroups.AddSelected(source, target, new[] { 0, 1, 2 });
        Assert.Equal(3, target.Count); // A 去重，B/C 追加
        Assert.Equal("A", target[0]);
        Assert.Equal("B", target[1]);
        Assert.Equal("C", target[2]);
    }

    [Fact]
    public void DeleteAt_Bounds()
    {
        var target = MakeList("A", "B");
        ViewListGroups.DeleteAt(target, 5);  // 越界无操作
        Assert.Equal(2, target.Count);
        ViewListGroups.DeleteAt(target, 0);
        Assert.Equal("B", target[0]);
    }

    [Fact]
    public void Save_WritesBackSorted()
    {
        var global = MakeList("C", "A");
        var listBox = MakeList("B", "A");
        ViewListGroups.Save(listBox, global);
        Assert.True(global.Sorted);
        Assert.Equal(2, global.Count);
        Assert.Equal("A", global[0]);
        Assert.Equal("B", global[1]);
    }

    [Fact]
    public void DisableTakeOff_Add_FormatsIndexAndName()
    {
        var source = MakeList("木剑", "金创药");
        var target = new TStringList();
        ViewListGroups.DisableTakeOffAdd(source, target, new[] { 0, 1 });
        Assert.Equal("0  木剑", target[0]);
        Assert.Equal("1  金创药", target[1]);
    }

    [Fact]
    public void DisableTakeOff_AddAll_Resets()
    {
        var source = MakeList("木剑", "金创药");
        var target = MakeList("旧值");
        ViewListGroups.DisableTakeOffAddAll(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("0  木剑", target[0]);
    }

    [Fact]
    public void SaveLogItem_IncrementsReloadCalls()
    {
        var listBox = MakeList("A");
        var global = new TStringList();
        var before = ViewListGroups.LoadItemsDBCalls;
        ViewListGroups.ReloadItemsDBAnswer = true; // Delphi：询问重载物品库，答“是”走 FrmDB.LoadItemsDB
        ViewListGroups.SaveLogItemClick(listBox, global);
        Assert.Equal(before + 1, ViewListGroups.LoadItemsDBCalls);
        ViewListGroups.ReloadItemsDBAnswer = null;
    }
}

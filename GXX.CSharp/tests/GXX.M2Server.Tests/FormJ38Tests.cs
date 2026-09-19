using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J38：ViewList.pas 余片（剩余 12 组列表框映射与同步保存）1:1 测试。</summary>
public sealed class ViewListGroups2Tests : IDisposable
{
    private readonly ViewListForm _form;

    public ViewListGroups2Tests()
    {
        M2ShareState.ResetForTests(Path.GetTempPath());
            M2Config.ResetItemSetSliceDefaults();
            ViewListState.ResetForTests();
            ViewListState3.ResetDefaults();
            _form = StaRunner.New(() =>
            {
                var f = new ViewListForm();
                f.StdItemNamesHandler = () => new List<string> { "木剑", "金创药" };
                f.MapNamesHandler = () => new List<string> { "0", "3", "盟重省" };
                f.MonsterNamesHandler = () => new List<string> { "鸡", "稻草人" };
                return f;
            });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2ShareState.ResetForTests(null);
    }

    [Fact]
    public void Open_BuildsAllTwelveGroups()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);

            Assert.Equal(12, _form.Groups2.Count);
            Assert.All(_form.Groups2, g => Assert.True(g.SourceListBox.Items.Count > 0));
            Assert.Equal("禁止取下", _form.Groups2[0].Title);
        });
    }

    [Fact]
    public void EnablePickUp_AddAndSortedSave()
    {
        StaRunner.New(() =>
        {
            ViewListState.g_EnablePickUpItemList.Add("木剑");
            ViewListState.g_EnablePickUpItemList.Add("金创药");
            _form.Open(showModal: false);
            var g = _form.Groups2.First(x => x.Title == "允许拾取");

            Assert.Equal(2, g.TargetListBox.Items.Count); // Open：全局表载入列表框
            g.AddSelected("魔法药");
            Assert.Equal(3, g.TargetListBox.Items.Count);

            g.Save();
            Assert.True(ViewListState.g_EnablePickUpItemList.Sorted);
            Assert.Equal(3, ViewListState.g_EnablePickUpItemList.Count);
        });
    }

    [Fact]
    public void DisableTakeOff_IntList_SortedSave()
    {
        StaRunner.New(() =>
        {
            ViewListState.g_DisableTakeOffList.Add("3");
            ViewListState.g_DisableTakeOffList.Add("1");
            _form.Open(showModal: false);
            var g = _form.Groups2.First(x => x.Title == "禁止取下");

            Assert.Equal(2, g.TargetListBox.Items.Count);
            g.AddSelectedInt(2, "金创药");
            g.Save();
            Assert.Equal(3, ViewListState.g_DisableTakeOffList.Count);
            Assert.True(ViewListState.g_DisableTakeOffList.Sorted);
        });
    }

    [Fact]
    public void DisableMoveMap_AddFromSource_Save()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            var g = _form.Groups2.First(x => x.Title == "禁止传送地图");
            Assert.True(g.SourceListBox.Items.Count > 0);
            g.SourceListBox.SelectedIndex = 0;
            g.AddSelectedFromSource();
            Assert.Equal(1, g.TargetListBox.Items.Count); // 已加入目标列表框
            g.Save();
            Assert.Equal(1, ViewListState.g_DisableMoveMapList.Count);
            Assert.Equal("0", ViewListState.g_DisableMoveMapList[0]);
            g.ClearTarget();
            g.Save();
            Assert.Equal(0, ViewListState.g_DisableMoveMapList.Count);
        });
    }

    [Fact]
    public void MonsterGroups_LoadMonsters()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            var mon = _form.Groups2.First(x => x.Title == "不清理怪物");
            Assert.True(mon.SourceListBox.Items.Count > 0);
            var preview = _form.Groups2.First(x => x.Title == "预览物品怪物");
            Assert.True(preview.SourceListBox.Items.Count > 0);
        });
    }
}

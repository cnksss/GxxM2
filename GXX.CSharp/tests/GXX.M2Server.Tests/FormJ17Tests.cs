using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J17：uFrmStorageItemsView.pas 仓库物品查看测试。</summary>
public sealed class StorageItemsViewTests
{
    private static List<StorageItemsViewForm.StorageItemRow> MakeStorage(int count)
    {
        var list = new List<StorageItemsViewForm.StorageItemRow>();
        for (int i = 0; i < count; i++)
            list.Add(new StorageItemsViewForm.StorageItemRow
            {
                No = i + 1,
                Name = "物品" + i,
                MakeIndex = 100 + i,
                WIndex = 200 + i,
                Dura = 300,
                DuraMax = 500
            });
        return list;
    }

    [Fact]
    public void Open_LoadsHumans_AndInitialButtonStates()
    {
        StaRunner.New(() =>
        {
            using var form = new StorageItemsViewForm();
            var humans = new List<string> { "甲", "乙" };
            form.Open(humans, showModal: false);
            Assert.Equal(2, form.lstUsers.Items.Count);
            Assert.False(form.btnDel.Enabled);
            Assert.True(form.btnDelAll.Enabled == false); // 未选择用户，仓库为空
        });
    }

    [Fact]
    public void LstUsersClick_LoadsStorageItems()
    {
        StaRunner.New(() =>
        {
            using var form = new StorageItemsViewForm();
            var storage = MakeStorage(2);
            form.LoadStorageItemsHandler = _ => storage;
            form.GetStdItemNameHandler = _ => "木剑";
            form.lstUsers.Items.Add("甲");
            form.lstUsers.SelectedIndex = 0;
            form.lstUsersClick(form, 0);

            Assert.Equal(2, form.lvItems.Rows.Count);
            Assert.Equal("1", form.lvItems[0, 0].Value);
            Assert.Equal("木剑", form.lvItems[1, 0].Value);
            Assert.Equal("100", form.lvItems[2, 0].Value);
            Assert.Equal("201", form.lvItems[3, 0].Value);
            // Delphi：加载后 lvItems 未点选 → btnDel 禁用，btnDelAll 启用
            Assert.False(form.btnDel.Enabled);
            Assert.True(form.btnDelAll.Enabled);
        });
    }

    [Fact]
    public void BtnDel_RemovesByMakeIndex_AndCallsDB()
    {
        StaRunner.New(() =>
        {
            using var form = new StorageItemsViewForm();
            var storage = MakeStorage(3);
            form.LoadStorageItemsHandler = _ => storage;
            form.GetStdItemNameHandler = _ => "物品";
            int deleteCalls = 0;
            form.DeleteStorageItemHandler = (_, _, _, _) => deleteCalls++;

            form.lstUsers.Items.Add("甲");
            form.lstUsers.SelectedIndex = 0;
            form.lstUsersClick(form, 0);
            form.SelectedItemIndex = 1; // MakeIndex=101
            form.btnDelClick(form);

            Assert.Equal(2, form.lvItems.Rows.Count);
            Assert.Equal(1, deleteCalls);
            Assert.Equal(2, storage.Count);
            Assert.False(storage.Any(s => s.MakeIndex == 101));
            Assert.Equal(2, form.lvItems.Rows.Count);
        });
    }

    [Fact]
    public void BtnDelAll_ClearsAllAndDB()
    {
        StaRunner.New(() =>
        {
            using var form = new StorageItemsViewForm();
            var storage = MakeStorage(3);
            form.LoadStorageItemsHandler = _ => storage;
            form.GetStdItemNameHandler = _ => "物品";
            form.lstUsers.Items.Add("甲");
            form.lstUsers.SelectedIndex = 0;
            form.lstUsersClick(form, 0);
            form.btnDelAllClick(form, confirmed: true);

            Assert.Equal(0, form.lvItems.Rows.Count);
            Assert.Empty(storage);
            Assert.False(form.btnDel.Enabled);
            Assert.False(form.btnDelAll.Enabled);
        });
    }

    [Fact]
    public void BtnSearch_SameText_SelectsUser()
    {
        StaRunner.New(() =>
        {
            using var form = new StorageItemsViewForm();
            var storage = MakeStorage(1);
            form.LoadStorageItemsHandler = _ => storage;
            form.lstUsers.Items.Add("甲");
            form.lstUsers.Items.Add("乙");
            Assert.NotNull(form.edtUser);
            form.edtUser.Text = "乙";
            form.btnSearchClick(form);
            Assert.Equal("乙", form.CurrentUserName);
        });
    }
}

/// <summary>批次J17：uFrmPlugManager.pas 插件管理测试。</summary>
public sealed class PlugManagerTests
{
    [Fact]
    public void FormCreate_FillsPlugList()
    {
        StaRunner.New(() =>
        {
            using var form = new PlugManagerForm();
            form.PlugList.Add(("插件甲", null));
            form.PlugList.Add(("插件乙", new PlugManagerForm.PluginInfo { IsSysDef = true }));
            form.FormCreate();
            Assert.Equal(2, form.vstPlug.Rows.Count);
            Assert.Equal("插件甲", form.vstPlug[1, 0].Value);
        });
    }

    [Fact]
    public void LoadPlug_NullPlugin_CanLoad()
    {
        StaRunner.New(() =>
        {
            using var form = new PlugManagerForm();
            var info = new PlugManagerForm.PluginInfo { sFileName = "test.dll", sVersion = "1.0" };
            form.PlugList.Add(("插件甲", null));
            form.LoadPluginHandler = _ => info;
            form.SetFocusedIndex(0);
            form.vstPlugFocusChanged(form);
            Assert.True(form.btnLoadPlug.Enabled);

            form.mniLoadPlugClick(form);
            Assert.Equal("test.dll", form.PlugList[0].Plugin!.sFileName);
            Assert.False(form.btnLoadPlug.Enabled);
            Assert.True(form.btnUnloadPlug.Enabled);
            Assert.Contains("test.dll", form.mmoPlugInfo.Text);
        });
    }

    [Fact]
    public void UnloadPlug_SysDef_Blocked()
    {
        StaRunner.New(() =>
        {
            using var form = new PlugManagerForm();
            var sys = new PlugManagerForm.PluginInfo { IsSysDef = true, sFileName = "sys.dll" };
            form.PlugList.Add(("系统插件", sys));
            form.SetFocusedIndex(0);
            form.vstPlugFocusChanged(form);
            Assert.False(form.btnLoadPlug.Enabled);
            Assert.False(form.btnUnloadPlug.Enabled);

            form.mniUnloadPlugClick(form);
            Assert.NotNull(form.PlugList[0].Plugin); // 系统插件不可卸载
        });
    }

    [Fact]
    public void PopupMenu_States()
    {
        StaRunner.New(() =>
        {
            using var form = new PlugManagerForm();
            form.PlugList.Add(("未加载", null));
            form.PlugList.Add(("已加载", new PlugManagerForm.PluginInfo()));
            form.SetFocusedIndex(0);
            var (load0, unload0) = form.pmPlugListPopup();
            Assert.True(load0);
            Assert.False(unload0);
            form.SetFocusedIndex(1);
            var (load1, unload1) = form.pmPlugListPopup();
            Assert.False(load1);
            Assert.True(unload1);
        });
    }
}

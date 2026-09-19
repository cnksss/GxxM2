using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J18：ViewList.pas TfrmViewList 核心组（允许/禁止制造名单）移植测试。</summary>
public sealed class ViewListTests : IDisposable
{
    private readonly string[] _itemNames = { "木剑", "金创药", "魔法药", "祖玛头像" };

    public ViewListTests()
    {
        ViewListState.ResetForTests();
    }

    public void Dispose()
    {
        ViewListState.ResetForTests();
    }

    private ViewListForm NewForm()
    {
        return StaRunner.New(() =>
        {
            var f = new ViewListForm();
            f.StdItemNamesHandler = () => _itemNames.ToList();
            return f;
        });
    }

    [StaFact]
    public void Open_FillsItemLists_AndEnableMakeListGetsAll()
    {
        var form = NewForm();
        try
        {
            form.Open(showModal: false);
            Assert.Equal(4, form.ListBoxItemList.Items.Count);
            // Delphi 1:1：Open 时把全部物品加入允许制造表
            Assert.Equal(4, form.ListBoxEnableMakeList.Items.Count);
            Assert.False(form.btnSaveEnableMakeItem.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void AddEnableMake_Dedupes_AndSetsModValue()
    {
        var form = NewForm();
        try
        {
            form.Open(showModal: false);
            form.btnDelAllEnableMakeItemClick(form); // 先清空（Open 时 Delphi 已全量加入）
            form.btnAddEnableMakeItemClick(form, new[] { 0, 0, 1 }); // 重复项 0 去重
            Assert.Equal(2, form.ListBoxEnableMakeList.Items.Count);
            Assert.Equal("木剑", form.ListBoxEnableMakeList.Items[0]);
            Assert.Equal("金创药", form.ListBoxEnableMakeList.Items[1]);
            Assert.True(form.IsModValued);
            Assert.True(form.btnSaveEnableMakeItem.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void AddAllEnableMake_CopiesAllItems()
    {
        var form = NewForm();
        try
        {
            form.Open(showModal: false);
            form.btnAddAllEnableMakeItemClick(form);
            Assert.Equal(4, form.ListBoxEnableMakeList.Items.Count);
            Assert.True(form.IsModValued);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void DelAndDelAll_EnableMake()
    {
        var form = NewForm();
        try
        {
            form.Open(showModal: false);
            form.btnDelEnableMakeItemClick(form, 1); // 删除第 2 项（金创药）
            Assert.Equal("魔法药", form.ListBoxEnableMakeList.Items[1]); // 魔法药 上移至 index 1
            Assert.False(form.btnDelEnableMakeItem.Enabled); // 删除后 SelectedIndex < 0 → 禁用

            form.btnDelAllEnableMakeItemClick(form);
            Assert.Equal(0, form.ListBoxEnableMakeList.Items.Count);
            Assert.False(form.btnDelEnableMakeItem.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void SaveEnableMake_WritesGlobalSorted_AndDisablesSave()
    {
        var saved = 0;
        var form = NewForm();
        try
        {
            form.SaveEnableMakeItemHandler = () => saved++;
            form.Open(showModal: false);
            form.btnDelEnableMakeItemClick(form, 0);
            form.btnSaveEnableMakeItemClick(form);

            Assert.Equal(3, ViewListState.g_EnableMakeItemList.Count);
            Assert.True(ViewListState.g_EnableMakeItemList.Sorted);
            Assert.Equal(1, saved);
            Assert.False(form.btnSaveEnableMakeItem.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void DisableMake_AddDelSave()
    {
        var form = NewForm();
        try
        {
            form.Open(showModal: false);
            form.btnAddDisableMakeItemClick(form, new[] { 2 });
            Assert.Equal("魔法药", form.ListBoxDisableMakeList.Items[0]);
            form.btnDelDisableMakeItemClick(form, 0);
            Assert.Equal(0, form.ListBoxDisableMakeList.Items.Count);
            Assert.False(form.btnDelDisableMakeItem.Enabled);

            form.btnAddDisableMakeItemClick(form, new[] { 0, 3 });
            form.btnSaveDisableMakeItemClick(form);
            Assert.Equal(2, ViewListState.g_DisableMakeItemList.Count);
            Assert.True(ViewListState.g_DisableMakeItemList.Sorted);
            Assert.False(form.btnSaveDisableMakeItem.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void GlobalLists_PersistAcrossForms()
    {
        ViewListState.g_EnableMakeItemList.Add("祖玛头像");
        var form = NewForm();
        try
        {
            form.Open(showModal: false);
            Assert.Contains("祖玛头像", ViewListState.g_EnableMakeItemList.AsEnumerable());
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

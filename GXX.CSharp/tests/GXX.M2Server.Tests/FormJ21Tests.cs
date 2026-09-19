using GXX.M2Server.Common;
using GXX.M2Server.Engine;
using GXX.M2Server.GameCenter;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J21：ViewList 剩余名单组接入窗体 + GameCenter/Common 收尾引擎层测试。</summary>
public sealed class ViewListGroupWiringTests : IDisposable
{
    private readonly string[] _itemNames = { "木剑", "金创药", "魔法药" };

    private readonly ViewListForm _form;

    public ViewListGroupWiringTests()
    {
        _form = StaRunner.New(() =>
        {
            var f = new ViewListForm();
            f.StdItemNamesHandler = () => _itemNames.ToList();
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
    }

    [Fact]
    public void GroupAddSelected_DedupesAcrossAllGroups()
    {
        StaRunner.New(() =>
        {
            ViewListState.ResetForTests();
            _form.Open(showModal: false);
            foreach (var g in _form.Groups)
            {
                Assert.Equal(3, g.Source.Count); // 物品总表已同步到各组 Source
                ViewListGroups.AddSelected(g.Source, g.Target, new[] { 0, 1 });
                ViewListGroups.AddSelected(g.Source, g.Target, new[] { 0, 1 }); // 重复添加被去重
                Assert.Equal(2, g.Target.Count);
                g.SaveHandler?.Invoke(); // 窗体已预接全局表（ctor WireGroupSaveHandlers）
            }
            Assert.Equal(2, ViewListState.g_DisableMoveMapList.Count);
            Assert.Equal(2, ViewListState.g_EnablePickUpItemList.Count);
        });
    }

    [Fact]
    public void GroupSave_PersistsToGlobal()
    {
        StaRunner.New(() =>
        {
            ViewListState.ResetForTests();
            _form.Open(showModal: false);
            var g = _form.Group("DisableMoveMap");
            ViewListGroups.AddSelected(g.Source, g.Target, new[] { 0 });
            g.SaveHandler!.Invoke(); // 1:1 btnSaveDisabelMoveMapClick：写回 g_DisableMoveMapList 并 Sorted
            Assert.Equal(1, ViewListState.g_DisableMoveMapList.Count);
            Assert.Equal("木剑", ViewListState.g_DisableMoveMapList[0]);
        });
    }

    [Fact]
    public void GroupOpen_LoadsGlobalsIntoTargets()
    {
        StaRunner.New(() =>
        {
            ViewListState.ResetForTests();
            ViewListState.g_NameFilterList.Add("半兽勇士");
            _form.Open(showModal: false);
            Assert.Equal("半兽勇士", _form.Group("NameFilter").Target[0]); // Open 把全局表载入组 Target
        });
    }

    [Fact]
    public void GroupDeleteAll_Empties()
    {
        StaRunner.New(() =>
        {
            ViewListState.ResetForTests();
            _form.Open(showModal: false);
            var g = _form.Group("EnablePickUp");
            ViewListGroups.AddSelected(g.Source, g.Target, new[] { 0, 1, 2 });
            Assert.Equal(3, g.Target.Count);
            ViewListGroups.DeleteAll(g.Target);
            Assert.Equal(0, g.Target.Count);
        });
    }
}

/// <summary>GameCenter GBDEtoSqlite/GHeroDBConfig 与 Common Rsa/UnitDes3 引擎层测试。</summary>
public sealed class GameCenterCommonTests
{
    [Fact]
    public void GBDEtoSqlite_FieldNameTables_Complete()
    {
        Assert.Equal(34, GBDEtoSqlite.GomFieldNameStd.Length);
        Assert.Equal(34, GBDEtoSqlite.GeeFieldNameStd.Length);
        Assert.Equal(2, GBDEtoSqlite.GomFieldNameMag.Length);
        Assert.Equal(2, GBDEtoSqlite.GeeFieldNameMon.Length);
        Assert.Equal("Value1", GBDEtoSqlite.GomFieldNameStd[5]);
        Assert.Equal("element", GBDEtoSqlite.GeeFieldNameStd[5]);
    }

    [Fact]
    public void GBDEtoSqlite_MapFieldName_RoundTrip()
    {
        Assert.Equal("element", GBDEtoSqlite.MapFieldName("STD", "Value1"));
        Assert.Equal("Expand1", GBDEtoSqlite.MapFieldName("STD", "Expand1")); // 同名同位直映
        Assert.Equal("Light", GBDEtoSqlite.MapFieldName("STD", "Job"));       // Job→Light
        Assert.Equal("ExploreItem", GBDEtoSqlite.MapFieldName("MON", "ExploreItem"));
        Assert.Equal("MaxUpgradeLv", GBDEtoSqlite.MapFieldName("MAG", "MaxUpgradeLv"));
        Assert.Null(GBDEtoSqlite.MapFieldName("STD", "NoSuchField"));
        Assert.Null(GBDEtoSqlite.MapFieldName("BAD", "Any"));
    }

    [Fact]
    public void GHeroDBConfig_Defaults_AndReset()
    {
        GHeroDBConfig.Reset();
        Assert.True(GHeroDBConfig.UseSqlite);
        Assert.Equal(@".\FDB\", GHeroDBConfig.BdeDir);
        Assert.Equal(@".\GHeroDB.db", GHeroDBConfig.SqliteFile);
        GHeroDBConfig.UseSqlite = false;
        GHeroDBConfig.Reset();
        Assert.True(GHeroDBConfig.UseSqlite);
    }

    [Fact]
    public void CommonCrypto_RsaRoundTrip()
    {
        var (pub, priv) = CommonCrypto.GenerateRsaKeys(512);
        var data = System.Text.Encoding.UTF8.GetBytes("复刻测试");
        var cipher = CommonCrypto.RsaEncrypt(data, pub);
        var plain = CommonCrypto.RsaDecrypt(cipher, priv);
        Assert.Equal(data, plain);
    }
}

/// <summary>UnitDes3 变体占位测试（源码接入前为恒等占位）。</summary>
public sealed class UnitDes3PlaceholderTests
{
    [Fact]
    public void Placeholder_Identity()
    {
        var data = new byte[] { 1, 2, 3 };
        Assert.Equal(data, Common.UnitDes3.Encrypt3(data, new byte[Common.UnitDes3.KeySizeBytes]));
        Assert.Equal(data, Common.UnitDes3.Decrypt3(data, new byte[Common.UnitDes3.KeySizeBytes]));
    }
}

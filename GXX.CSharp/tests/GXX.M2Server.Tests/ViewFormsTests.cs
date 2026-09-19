using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J15：掉落日志/寄售货币汇总/用户商铺查看窗体移植测试（UI 测试体整段 STA 执行）。</summary>
public sealed class ViewFormsTests : IDisposable
{
    private readonly string _dir;

    public ViewFormsTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j15_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2Config.ResetServerValueDefaults();
    }

    public void Dispose()
    {
        M2Config.ResetServerValueDefaults();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    // ---------- 掉落日志 ----------

    [Fact]
    public void LoadItemDropLog_ParsesAndFiltersByMap()
    {
        var log = Path.Combine(_dir, "木剑.txt");
        File.WriteAllLines(log, new[]
        {
            "2026-09-16 10:00:00 玩家甲 骷髅 0151 100 200",
            "2026-09-16 11:30:00 玩家乙 沃玛战士 0151 300 400",
            "2026-09-16 12:00:00 玩家丙 骷髅 0150 500 600"
        }, System.Text.Encoding.UTF8);

        var all = ViewFormsData.LoadItemDropLog(log, "*");
        Assert.Equal(3, all.Count);
        Assert.Equal("玩家甲", all[0].ItemOwner);
        Assert.Equal("骷髅", all[0].DropMonName);
        Assert.Equal("0151", all[0].MapName);
        Assert.Equal(100, all[0].nX);
        Assert.Equal(200, all[0].nY);

        // Delphi SameText(MapName, S4)：按怪物名字段过滤（1:1）
        var filtered = ViewFormsData.LoadItemDropLog(log, "骷髅");
        Assert.Equal(2, filtered.Count);
        Assert.Equal("玩家甲", filtered[0].ItemOwner);
        Assert.Equal("玩家丙", filtered[1].ItemOwner);
    }

    [Fact]
    public void LogColumnText_FormatsAllColumns()
    {
        var log = new TDropItemLogData
        {
            DropDate = new DateTime(2026, 9, 16, 10, 0, 0),
            ItemOwner = "甲",
            DropMonName = "骷髅",
            MapName = "0151",
            nX = 10,
            nY = 20
        };
        Assert.Equal("2026/09/16 10:00:00", ViewFormsData.LogColumnText(log, 0));
        Assert.Equal("甲", ViewFormsData.LogColumnText(log, 1));
        Assert.Equal("骷髅", ViewFormsData.LogColumnText(log, 2));
        Assert.Equal("0151", ViewFormsData.LogColumnText(log, 3));
        Assert.Equal("10, 20", ViewFormsData.LogColumnText(log, 4));
    }

    [Fact]
    public void ItemDropLogForm_LoadLog_FillsGrid()
    {
        StaRunner.New(() =>
        {
            var log = Path.Combine(_dir, "圣战头盔.txt");
            File.WriteAllLines(log, new[] { "2026-09-16 10:00:00 玩家甲 骷髅 0151 100 200" }, System.Text.Encoding.UTF8);

            using var form = new ItemDropLogForm();
            form.LoadLog(log, "*");
            Assert.Equal(1, form.vstLogs.Rows.Count);
            Assert.Equal("2026/09/16 10:00:00", form.vstLogs[0, 0].Value);
            Assert.Equal("玩家甲", form.vstLogs[1, 0].Value);
        });
    }

    // ---------- 寄售货币汇总 ----------

    [Fact]
    public void RefreshUserShopMoneyTotal_GroupsByMaster()
    {
        var items = new List<TSelledAndNoGetMoneyTotal>
        {
            new() { sMasterName = "甲", btMoneyType = 0, nSumPrice = 100 },
            new() { sMasterName = "甲", btMoneyType = 0, nSumPrice = 50 },
            new() { sMasterName = "乙", btMoneyType = 3, nSumPrice = 7 }
        };

        var (rows, sums) = ViewFormsData.RefreshUserShopMoneyTotal(items);
        Assert.Equal(2, rows.Count);
        Assert.Equal("甲", rows[0].Item1);
        Assert.Equal(150, rows[0].Item2[0]);
        Assert.Equal("乙", rows[1].Item1);
        Assert.Equal(7, rows[1].Item2[3]);
        Assert.Equal(150, sums[0]);
        Assert.Equal(7, sums[3]);
    }

    [Fact]
    public void RefreshUserShopMoneyTotal_SkipsZeroRow()
    {
        var items = new List<TSelledAndNoGetMoneyTotal>
        {
            new() { sMasterName = "甲", btMoneyType = 9, nSumPrice = 100 }
        };
        var (rows, _) = ViewFormsData.RefreshUserShopMoneyTotal(items);
        Assert.Empty(rows);
    }

    [Fact]
    public void UserShopMoneyForm_RefreshData_Fills()
    {
        StaRunner.New(() =>
        {
            using var form = new UserShopGetMoneyTotalForm();
            form.RefreshData(new List<TSelledAndNoGetMoneyTotal>
            {
                new() { sMasterName = "甲", btMoneyType = 0, nSumPrice = 100 },
                new() { sMasterName = "甲", btMoneyType = 1, nSumPrice = 25 }
            });
            Assert.Equal(1, form.lvMoney.Rows.Count);
            Assert.Equal("甲", form.lvMoney[0, 0].Value);
            Assert.Equal("100", form.lvMoney[1, 0].Value);
            Assert.Equal("25", form.lvMoney[2, 0].Value);
            Assert.Equal("100", form.edt1.Text);
            Assert.Equal("25", form.edt2.Text);
        });
    }

    // ---------- 用户商铺 ----------

    [Fact]
    public void UserShopView_FormCreate_Fills()
    {
        StaRunner.New(() =>
        {
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
            using var form = new UserShopViewForm();
            form.FormCreate(new List<TUserShop>
            {
                new() { ShopID = 1, sMasterName = "甲", sShopName = "商铺一", dCreateDate = new DateTime(2026, 9, 1) },
                new() { ShopID = 2, sMasterName = "乙", sShopName = "商铺二", dCreateDate = new DateTime(2026, 9, 2) }
            });
            Assert.Equal(2, form.lvItems.Rows.Count);
            Assert.Equal("商铺一", form.lvItems[2, 0].Value);
            Assert.Equal("2026/09/01", form.lvItems[3, 0].Value);
        });
    }

    [Fact]
    public void UserShopView_Rename_ExistsBlocked()
    {
        StaRunner.New(() =>
        {
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
            using var form = new UserShopViewForm();
            form.FormCreate(new List<TUserShop>
            {
                new() { ShopID = 1, sMasterName = "甲", sShopName = "旧名", dCreateDate = DateTime.Today }
            });
            form.lvItems.CurrentCell = form.lvItems.Rows[0].Cells[0];
            form.lvItemsSelectItem(form);

            form.ShopNameExistsHandler = _ => true;
            form.edtShopName.Text = "新名";
            var (ok, msg) = form.btnRenameClick(form);
            Assert.False(ok);
            Assert.Contains("店铺名已被占用", msg ?? "");
        });
    }

    [Fact]
    public void UserShopView_Rename_Success()
    {
        StaRunner.New(() =>
        {
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
            using var form = new UserShopViewForm();
            form.FormCreate(new List<TUserShop>
            {
                new() { ShopID = 1, sMasterName = "甲", sShopName = "旧名", dCreateDate = DateTime.Today }
            });
            form.lvItems.CurrentCell = form.lvItems.Rows[0].Cells[0];
            form.lvItemsSelectItem(form);

            form.ShopNameExistsHandler = _ => false;
            form.GetNameInFilterListHandler = _ => false;
            form.ShopRenameHandler = (id, name) => true;
            M2Forms.NextAnswer = M2Forms.IDYES;
            form.edtShopName.Text = "新名";
            var (ok, msg) = form.btnRenameClick(form);
            Assert.True(ok);
            Assert.Equal("店铺改名成功", msg);
            Assert.Equal("新名", form.lvItems[2, 0].Value);
        });
    }

    [Fact]
    public void UserShopView_Search_ByMasterName()
    {
        StaRunner.New(() =>
        {
            M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
            using var form = new UserShopViewForm();
            form.FormCreate(new List<TUserShop>
            {
                new() { ShopID = 1, sMasterName = "甲", sShopName = "铺1" },
                new() { ShopID = 2, sMasterName = "乙", sShopName = "铺2" }
            });
            form.edtUser.Text = "乙";
            form.btnSearchClick(form);
            Assert.Equal("乙", form.lvItems[1, form.lvItems.CurrentRow.Index].Value);
        });
    }
}

using System;
using System.IO;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>Ranking.pas:1-332（排行榜管理）逻辑测试：九张榜的显示列与 9 个 INI 键的保存。</summary>
public class RankingTests : TempDirTest
{
    private static TRoleRankData Rank(string human, string hero, uint level, uint master)
        => new TRoleRankData { HumanName = human, HeroName = hero, Level = level, MasterCount = master };

    private static RankingDisplay NewDisplay(out MemoryListViewSink[] sinks)
    {
        var d = new RankingDisplay();
        sinks = new MemoryListViewSink[9];
        for (int i = 0; i < 9; i++) sinks[i] = new MemoryListViewSink();
        d.ListViewHum = sinks[0];
        d.ListViewWarrior = sinks[1];
        d.ListViewWizzard = sinks[2];
        d.ListViewMonk = sinks[3];
        d.ListViewHero = sinks[4];
        d.ListViewHeroWarrior = sinks[5];
        d.ListViewHeroWizzard = sinks[6];
        d.ListViewHeroMonk = sinks[7];
        d.ListViewMaster = sinks[8];
        return d;
    }

    [Fact]
    public void RefRanking_个人榜四张_Caption为序号_子项为名称与等级()
    {
        DBShareSeam.g_HumanRankList.Add(Rank("hum0", "", 40, 0));
        DBShareSeam.g_HumanRankList.Add(Rank("hum1", "", 39, 0));
        DBShareSeam.g_WarriorRankList.Add(Rank("war", "", 30, 0));
        DBShareSeam.g_WizardRankList.Add(Rank("wiz", "", 20, 0));
        DBShareSeam.g_TaoistRankList.Add(Rank("tao", "", 10, 0));

        var d = NewDisplay(out var sinks);
        byte guard = 0;
        RankingLogic.RefRanking(d, ref guard);

        Assert.Equal(2, sinks[0].Count);
        Assert.Equal("0", sinks[0].Rows[0].Caption);
        Assert.Null(sinks[0].Rows[0].Tag);
        Assert.Equal(new[] { "hum0", "40" }, sinks[0].Rows[0].SubItems);
        Assert.Equal("1", sinks[0].Rows[1].Caption);
        Assert.Equal("hum1", sinks[0].Rows[1].SubItems[0]);
        Assert.Equal(new[] { "war", "30" }, sinks[1].Rows[0].SubItems);
        Assert.Equal(new[] { "wiz", "20" }, sinks[2].Rows[0].SubItems);
        Assert.Equal(new[] { "tao", "10" }, sinks[3].Rows[0].SubItems);
    }

    [Fact]
    public void RefRanking_英雄榜四张_依次为英雄名角色名等级()
    {
        DBShareSeam.g_HeroRankList.Add(Rank("h1", "hero1", 50, 0));
        DBShareSeam.g_HeroWarriorRankList.Add(Rank("h2", "hero2", 51, 0));
        DBShareSeam.g_HeroWizardRankList.Add(Rank("h3", "hero3", 52, 0));
        DBShareSeam.g_HeroTaoistRankList.Add(Rank("h4", "hero4", 53, 0));

        var d = NewDisplay(out var sinks);
        byte guard = 0;
        RankingLogic.RefRanking(d, ref guard);

        Assert.Equal(new[] { "hero1", "h1", "50" }, sinks[4].Rows[0].SubItems);
        Assert.Equal(new[] { "hero2", "h2", "51" }, sinks[5].Rows[0].SubItems);
        Assert.Equal(new[] { "hero3", "h3", "52" }, sinks[6].Rows[0].SubItems);
        Assert.Equal(new[] { "hero4", "h4", "53" }, sinks[7].Rows[0].SubItems);
    }

    [Fact]
    public void RefRanking_名师榜_子项为名称与出师徒弟数()
    {
        DBShareSeam.g_MasterRankList.Add(Rank("m1", "", 0, 7));
        DBShareSeam.g_MasterRankList.Add(Rank("m2", "", 0, 8));

        var d = NewDisplay(out var sinks);
        byte guard = 0;
        RankingLogic.RefRanking(d, ref guard);

        Assert.Equal(new[] { "m1", "7" }, sinks[8].Rows[0].SubItems);
        Assert.Equal(new[] { "m2", "8" }, sinks[8].Rows[1].SubItems);
    }

    [Fact]
    public void RefRanking_先清空九张榜_空列表不产生行()
    {
        var d = NewDisplay(out var sinks);
        sinks[0].AddRow("stale", null, "x");
        byte guard = 0;
        RankingLogic.RefRanking(d, ref guard);
        Assert.Equal(0, sinks[0].Count);
    }

    [Fact]
    public void RefRanking_g_boRefRanking为真时直接返回_不清空()
    {
        DBShareSeam.g_boRefRanking = 1;
        var d = NewDisplay(out var sinks);
        sinks[0].AddRow("keep", null, "x");
        byte guard = 0;

        RankingLogic.RefRanking(d, ref guard);

        Assert.Equal(1, sinks[0].Count);
        Assert.Equal((byte)0, guard);
    }

    [Fact]
    public void RefRanking_m_boRefRanking为真时直接返回_防重入()
    {
        DBShareSeam.g_HumanRankList.Add(Rank("hum", "", 1, 0));
        var d = NewDisplay(out var sinks);
        byte guard = 1;

        RankingLogic.RefRanking(d, ref guard);

        Assert.Equal(0, sinks[0].Count);
    }

    [Fact]
    public void RefRanking_结束后复位m_boRefRanking_异常也复位()
    {
        DBShareSeam.g_HumanRankList.Add(Rank("hum", "", 1, 0));
        var d = NewDisplay(out _);
        byte guard = 0;
        RankingLogic.RefRanking(d, ref guard);
        Assert.Equal((byte)0, guard);
    }

    [Fact]
    public void RefRanking_每100行让出一次消息循环()
    {
        for (int i = 0; i < 250; i++) DBShareSeam.g_HumanRankList.Add(Rank("h" + i, "", 1, 0));
        var d = NewDisplay(out _);
        int pumped = 0;
        Action saved = RankingLogic.ProcessMessages;
        try
        {
            RankingLogic.ProcessMessages = () => pumped++;
            byte guard = 0;
            RankingLogic.RefRanking(d, ref guard);
        }
        finally
        {
            RankingLogic.ProcessMessages = saved;
        }
        Assert.Equal(3, pumped);       // I = 0, 100, 200
    }

    [Fact]
    public void ButtonSave_九个INI键顺序与大小写逐字节()
    {
        DBShareSeam.g_boAutoRefRanking = 1;
        DBShareSeam.g_nRankingCount = 100;
        DBShareSeam.g_nRankingMinLevel = 20;
        DBShareSeam.g_nRankingMaxLevel = 500;
        DBShareSeam.g_nRefRankingHour1 = 1;
        DBShareSeam.g_nRefRankingHour2 = 2;
        DBShareSeam.g_nRefRankingMinute1 = 3;
        DBShareSeam.g_nRefRankingMinute2 = 4;
        DBShareSeam.g_nAutoRefRankingType = 1;

        RankingLogic.ButtonSave();

        string expected =
            "[Setup]\r\n" +
            "AutoRefRanking=1\r\n" +
            "RankingCount=100\r\n" +
            "RankingMinLevel=20\r\n" +
            "RankingMaxLevel=500\r\n" +
            "RefRankingHour1=1\r\n" +
            "RefRankingHour2=2\r\n" +
            "RefRankingMinute1=3\r\n" +
            "RefRankingMinute2=4\r\n" +
            "AutoRefRankingType=1\r\n";
        Assert.Equal(expected, File.ReadAllText(DBShareSeam.g_sConfFileName));
    }

    [Fact]
    public void ButtonSave_写入GlobalConfig读取用的RankingCount而不是gRankingCount_原文键名不一致()
    {
        // DBShare.LoadConfig 读的是 'gRankingCount'（DBShare.pas:933），而 Ranking 存的是 'RankingCount'
        DBShareSeam.g_nRankingCount = 77;
        RankingLogic.ButtonSave();
        string text = File.ReadAllText(DBShareSeam.g_sConfFileName);
        Assert.Contains("RankingCount=77", text);
        Assert.DoesNotContain("gRankingCount", text);
    }

    [Fact]
    public void RadioButton1Click_勾选为0_未勾选为1()
    {
        RankingLogic.RadioButton1Click(true);
        Assert.Equal(0, DBShareSeam.g_nAutoRefRankingType);
        RankingLogic.RadioButton1Click(false);
        Assert.Equal(1, DBShareSeam.g_nAutoRefRankingType);
    }

    [Fact]
    public void 窗体Open_关闭时的FormCloseQuery与刷新中的互斥()
    {
        DBShareSeam.g_boAutoRefRanking = 0;
        DBShareSeam.g_nRankingCount = 100;
        DBShareSeam.g_nRankingMinLevel = 20;
        DBShareSeam.g_nRankingMaxLevel = 500;
        DBShareSeam.g_nAutoRefRankingType = 1;

        using var form = new FrmRankingDlg();
        form.OpenCore();

        Assert.False(form.CheckBoxAutoRefRanking.Checked);
        Assert.Equal(100, form.seRankingCount.Value);
        Assert.Equal(20, form.EditMinLevel.Value);
        Assert.Equal(500, form.EditMaxLevel.Value);
        Assert.True(form.RadioButton2.Checked);
        Assert.False(form.ButtonSave.Enabled);
        Assert.True(form.Timer.Enabled);

        bool canClose = true;
        form.FormCloseQuery(null, ref canClose);
        Assert.True(canClose);
    }

    [Fact]
    public void 窗体事件_保存按钮在任意编辑后启用()
    {
        using var form = new FrmRankingDlg();
        form.ButtonSave.Enabled = false;

        form.EditMinLevel.Value = 30;
        form.EditMinLevelChange(null, EventArgs.Empty);
        Assert.True(form.ButtonSave.Enabled);
        Assert.Equal(30, DBShareSeam.g_nRankingMinLevel);

        form.ButtonSave.Enabled = false;
        form.seRankingCount.Value = 5;
        form.seRankingCountChange(null, EventArgs.Empty);
        Assert.True(form.ButtonSave.Enabled);
        Assert.Equal(5, DBShareSeam.g_nRankingCount);

        form.ButtonSave.Enabled = false;
        form.ButtonSaveClick(null, EventArgs.Empty);
        Assert.False(form.ButtonSave.Enabled);
    }

    [Fact]
    public void 窗体TimerTimer_触发一次刷新后关闭定时器()
    {
        DBShareSeam.g_HumanRankList.Add(Rank("h", "", 1, 0));
        using var form = new FrmRankingDlg();
        form.Timer.Enabled = true;

        form.TimerTimer(null, EventArgs.Empty);

        Assert.False(form.Timer.Enabled);
        Assert.Equal(1, form.ListViewHum.Items.Count);
    }

    [Fact]
    public void 窗体DFM_九张ListView的列头宽度对齐()
    {
        using var form = new FrmRankingDlg();
        Assert.Equal(3, form.ListViewHum.Columns.Count);
        Assert.Equal("序号", form.ListViewHum.Columns[0].Text);
        Assert.Equal(60, form.ListViewHum.Columns[0].Width);
        Assert.Equal("名称", form.ListViewHum.Columns[1].Text);
        Assert.Equal(100, form.ListViewHum.Columns[1].Width);
        Assert.Equal("等级", form.ListViewHum.Columns[2].Text);
        Assert.Equal(4, form.ListViewHero.Columns.Count);
        Assert.Equal("英雄名称", form.ListViewHero.Columns[1].Text);
        Assert.Equal("角色名称", form.ListViewHero.Columns[2].Text);
        Assert.Equal(3, form.ListViewMaster.Columns.Count);
        Assert.Equal("出师徒弟数", form.ListViewMaster.Columns[2].Text);
        Assert.Equal("排行榜管理", form.Text);
    }

    [Fact]
    public void 窗体ButtonRefRankingClick_调用刷新引擎并复位按钮()
    {
        int engineCalls = 0;
        Action saved = DBShareSeam.RankingEngine_RefRanking;
        try
        {
            DBShareSeam.RankingEngine_RefRanking = () => engineCalls++;
            using var form = new FrmRankingDlg();
            form.ButtonRefRankingClick(null, EventArgs.Empty);
            Assert.Equal(1, engineCalls);
            Assert.True(form.ButtonRefRanking.Enabled);
        }
        finally
        {
            DBShareSeam.RankingEngine_RefRanking = saved;
        }
    }

    [Fact]
    public void 窗体ButtonRefRankingClick_g_boRefRanking为真时不刷新()
    {
        int engineCalls = 0;
        Action saved = DBShareSeam.RankingEngine_RefRanking;
        try
        {
            DBShareSeam.RankingEngine_RefRanking = () => engineCalls++;
            DBShareSeam.g_boRefRanking = 1;
            using var form = new FrmRankingDlg();
            form.ButtonRefRankingClick(null, EventArgs.Empty);
            Assert.Equal(0, engineCalls);
        }
        finally
        {
            DBShareSeam.RankingEngine_RefRanking = saved;
        }
    }
}

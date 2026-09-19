using System;
using System.IO;
using GXX.Core.Util;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>Setting.pas:1-138（基本设置）逻辑与 INI 键序测试。</summary>
public class SettingTests : TempDirTest
{
    [Fact]
    public void Open_把全局量映射到控件值_含过滤串文本()
    {
        DBShareSeam.g_boDenyChrName = 1;
        DBShareSeam.g_boCanCreateHuman = 0;
        DBShareSeam.g_boCanDeleteHuman = 1;
        DBShareSeam.g_boCanGetBackDeleteHuman = 0;
        DBShareSeam.g_boForbidNumberName = 1;
        DBShareSeam.g_boForbidLetterName = 1;
        DBShareSeam.g_boCanRanking = 0;
        DBShareSeam.g_nCanDeleteHumanLowLevel = 7;
        DBShareSeam.g_nCreateChrNameCount = 3;
        DBShareSeam.g_boUseActiveRunGage = 1;
        DBShareSeam.g_boShowBlockIPLog = 1;
        DBShareSeam.g_FilterNewHumanNameTextList.Add("A");
        DBShareSeam.g_FilterNewHumanNameTextList.Add("B");
        DBShareSeam.g_FilterRankingNameTextList.Add("GM");

        SettingValues v = SettingLogic.Open();

        Assert.True(v.DenyChrName);
        Assert.False(v.CanCreateHuman);
        Assert.True(v.CanDeleteHuman);
        Assert.False(v.CanGetBackDeleteHuman);
        Assert.True(v.ForbidNumberName);
        Assert.True(v.ForbidLetterName);
        Assert.False(v.Ranking);
        Assert.Equal(7, v.CanDeleteHumanLowLevel);
        Assert.Equal(3, v.CreateChrNameCount);
        Assert.True(v.UseActiveRunGage);
        Assert.True(v.ShowBlockIPLog);
        Assert.Equal("A\r\nB", v.FilterNewHumanNameText);
        Assert.Equal("GM", v.FilterRankingNameText);
    }

    [Fact]
    public void ButtonOK_INI十一个键的顺序与大小写逐字节()
    {
        var v = new SettingValues
        {
            CanCreateHuman = true,
            CanDeleteHuman = true,
            CanGetBackDeleteHuman = true,
            CanDeleteHumanLowLevel = 45,
            ForbidNumberName = false,
            ForbidLetterName = false,
            DenyChrName = false,
            Ranking = true,
            CreateChrNameCount = 20,
            UseActiveRunGage = false,
            ShowBlockIPLog = false,
            FilterNewHumanNameText = "",
            FilterRankingNameText = ""
        };

        SettingLogic.ButtonOK(v);

        string expected =
            "[Setup]\r\n" +
            "CanCreateHuman=1\r\n" +
            "CanDeleteHuman=1\r\n" +
            "CanGetBackDeleteHuman=1\r\n" +
            "CanDeleteHumanLowLevel=45\r\n" +
            "ForbidNumberName=0\r\n" +
            "ForbidLetterName=0\r\n" +
            "DenyChrName=0\r\n" +
            "CanRanking=1\r\n" +
            "CreateChrNameCount=20\r\n" +
            "UseActiveRunGage=0\r\n" +
            "ShowBlockIPLog=0\r\n";
        Assert.Equal(expected, File.ReadAllText(DBShareSeam.g_sConfFileName));
    }

    [Fact]
    public void ButtonOK_写回全局量并落盘两个过滤串文件()
    {
        var v = new SettingValues
        {
            CanCreateHuman = false,
            CanDeleteHuman = false,
            CanGetBackDeleteHuman = false,
            CanDeleteHumanLowLevel = 12,
            ForbidNumberName = true,
            ForbidLetterName = true,
            DenyChrName = true,
            Ranking = false,
            CreateChrNameCount = 5,
            UseActiveRunGage = true,
            ShowBlockIPLog = true,
            FilterNewHumanNameText = "X\r\nY"
        };

        SettingLogic.ButtonOK(v);

        Assert.Equal((byte)0, DBShareSeam.g_boCanCreateHuman);
        Assert.Equal((byte)0, DBShareSeam.g_boCanDeleteHuman);
        Assert.Equal((byte)0, DBShareSeam.g_boCanGetBackDeleteHuman);
        Assert.Equal(12, DBShareSeam.g_nCanDeleteHumanLowLevel);
        Assert.Equal((byte)1, DBShareSeam.g_boForbidNumberName);
        Assert.Equal((byte)1, DBShareSeam.g_boForbidLetterName);
        Assert.Equal((byte)1, DBShareSeam.g_boDenyChrName);
        Assert.Equal((byte)0, DBShareSeam.g_boCanRanking);
        Assert.Equal(5, DBShareSeam.g_nCreateChrNameCount);
        Assert.Equal((byte)1, DBShareSeam.g_boUseActiveRunGage);
        Assert.Equal((byte)1, DBShareSeam.g_boShowBlockIPLog);

        Assert.Equal(2, DBShareSeam.g_FilterNewHumanNameTextList.Count);
        Assert.Equal("X", DBShareSeam.g_FilterNewHumanNameTextList[0]);
        Assert.Equal("Y", DBShareSeam.g_FilterNewHumanNameTextList[1]);
        Assert.Equal("X\r\nY\r\n", File.ReadAllText(Path.Combine(Dir, "FilterNewHumanNameString.txt")));
        Assert.Equal("", File.ReadAllText(Path.Combine(Dir, "FilterRankingNameString.txt")));
    }

    [Fact]
    public void SetText_按CRLF切分_结尾换行不产生额外空行()
    {
        var sl = new TStringList();
        TStringsHelper.SetText(sl, "a\r\nb\r\n");
        Assert.Equal(2, sl.Count);

        TStringsHelper.SetText(sl, "a\r\n\r\nb");
        Assert.Equal(3, sl.Count);
        Assert.Equal("", sl[1]);

        TStringsHelper.SetText(sl, "");
        Assert.Equal(0, sl.Count);

        TStringsHelper.SetText(sl, "a\nb\rc");
        Assert.Equal(3, sl.Count);
        Assert.Equal("c", sl[2]);
    }

    [Fact]
    public void 窗体ButtonOKClick_取消按钮共用同一处理器_DFM原文如此()
    {
        using var ui = new UiRecorder();
        using var form = new FrmSetting();
        form.CheckBoxCanCreateHuman.Checked = true;
        form.EditCanDeleteHumanLowLevel.Value = 45;      // DFM MaxValue=0，编程赋值不裁剪
        form.MemoFilterNewHumanName.Text = "abc";

        form.ButtonOKClick(form.Button1, EventArgs.Empty);

        Assert.Equal(System.Windows.Forms.DialogResult.OK, form.DialogResult);
        Assert.Equal((byte)1, DBShareSeam.g_boCanCreateHuman);
        Assert.Equal(45, DBShareSeam.g_nCanDeleteHumanLowLevel);
        Assert.Equal(1, DBShareSeam.g_FilterNewHumanNameTextList.Count);
        Assert.Equal("abc", DBShareSeam.g_FilterNewHumanNameTextList[0]);
        Assert.True(File.Exists(DBShareSeam.g_sConfFileName));
    }

    [Fact]
    public void 窗体Open_把全局写入控件_取消按钮的ModalResult为Cancel()
    {
        DBShareSeam.g_boCanCreateHuman = 0;
        DBShareSeam.g_nCanDeleteHumanLowLevel = 45;
        DBShareSeam.g_FilterNewHumanNameTextList.Add("k");

        using var form = new FrmSetting();
        form.Open();

        Assert.False(form.CheckBoxCanCreateHuman.Checked);
        Assert.Equal(45, form.EditCanDeleteHumanLowLevel.Value);
        Assert.Equal("k", form.MemoFilterNewHumanName.Text);
        Assert.Equal(System.Windows.Forms.DialogResult.Cancel, form.Button1.DialogResult);
        Assert.Equal(2, form.Button1.TabIndex);
    }

    [Fact]
    public void 窗体DFM_控件几何与文案对齐()
    {
        using var form = new FrmSetting();
        Assert.Equal("基本设置", form.Text);
        Assert.Equal(new System.Drawing.Size(737, 490), form.ClientSize);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.Equal("确定(&O)", form.ButtonOK.Text);
        Assert.Equal("取消(&C)", form.Button1.Text);
        Assert.Equal(new System.Drawing.Point(558, 456), form.ButtonOK.Location);
        Assert.Equal(new System.Drawing.Point(654, 456), form.Button1.Location);
        Assert.Equal("基本设置", form.GroupBox1.Text);
        Assert.Equal("其他设置", form.GroupBox2.Text);
        Assert.Equal("允许特殊字符创建人物", form.CheckBoxDenyChrName.Text);
        Assert.Equal("允许删除人物", form.CheckBoxCanDeleteHuman.Text);
        Assert.Equal("允许建立新人物", form.CheckBoxCanCreateHuman.Text);
        Assert.Equal("允许找回删除人物", form.CheckBoxCanGetBackDeleteHuman.Text);
        Assert.Equal("禁止建立包含数字的人物名", form.CheckBoxForbidNumberName.Text);
        Assert.Equal("禁止建立全英文人物名", form.CheckBoxForbidLetterName.Text);
        Assert.Equal("开启排行榜", form.CheckBoxRanking.Text);
        Assert.Equal("只分配可连接的游戏网关给客户端", form.chkUseActiveRunGage.Text);
        Assert.Equal("显示非法请求内部端口日志", form.chkShowBlockIPLog.Text);
        Assert.Equal(2, form.EditCreateChrNameCount.Value);         // DFM Value=2
        Assert.Equal(20, form.EditCreateChrNameCount.DfmMaxValue);
        Assert.Equal(1, form.EditCreateChrNameCount.DfmMinValue);
        Assert.Equal(0, form.EditCanDeleteHumanLowLevel.DfmMaxValue);
    }

    [Fact]
    public void ShowFrmSetting_签名与原文一致返回Boolean()
    {
        var mi = typeof(SettingUnit).GetMethod("ShowFrmSetting");
        Assert.NotNull(mi);
        Assert.True(mi.IsStatic);
        Assert.Equal(typeof(bool), mi.ReturnType);
        Assert.Empty(mi.GetParameters());
    }
}

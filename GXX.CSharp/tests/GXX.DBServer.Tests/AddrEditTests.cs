using System;
using System.Collections.Generic;
using System.IO;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>AddrEdit.pas:1-135（!ServerInfo.txt 的可视化编辑）逻辑测试。</summary>
public class AddrEditTests : TempDirTest
{
    private static TStringGridModel NewGrid() => new TStringGridModel(AddrEditLogic.ColCount, AddrEditLogic.InitialRowCount);

    [Fact]
    public void FormCreate_表头九列与DFM一致()
    {
        var g = NewGrid();
        AddrEditLogic.FormCreate(g);

        Assert.Equal(AddrEditLogic.ColCount, g.ColCount);
        Assert.Equal(AddrEditLogic.InitialRowCount, g.RowCount);
        Assert.Equal("角色选择网关地址", g[0, 0]);
        for (int c = 1; c < 9; c++)
            Assert.Equal(c % 2 == 1 ? "游戏网关" : "端口", g[c, 0]);
    }

    [Fact]
    public void BtnApplyRowClick_行数小于1时钳到1_再置RowCount为值加1()
    {
        var g = NewGrid();

        int v = 8;
        AddrEditLogic.BtnApplyRowClick(g, ref v);
        Assert.Equal(8, v);
        Assert.Equal(9, g.RowCount);

        v = 0;
        AddrEditLogic.BtnApplyRowClick(g, ref v);
        Assert.Equal(1, v);
        Assert.Equal(2, g.RowCount);

        v = -5;
        AddrEditLogic.BtnApplyRowClick(g, ref v);
        Assert.Equal(1, v);
        Assert.Equal(2, g.RowCount);
    }

    [Fact]
    public void BuildSaveLines_首列非空时逐列Trim并补空格_尾部也有空格()
    {
        var g = NewGrid();
        g[0, 1] = "1.1.1.1";
        g[1, 1] = " 2.2.2.2 ";
        g[2, 1] = "100";
        g[3, 1] = "3.3.3.3";
        g[4, 1] = "200";
        g[5, 1] = "4.4.4.4";
        g[6, 1] = "300";
        g[7, 1] = "5.5.5.5";
        g[8, 1] = "400";

        List<string> lines = AddrEditLogic.BuildSaveLines(g);

        Assert.Equal(8, lines.Count);   // RowCount=9 → 数据行 1..8
        Assert.Equal("1.1.1.1 2.2.2.2 100 3.3.3.3 200 4.4.4.4 300 5.5.5.5 400 ", lines[0]);
        Assert.Equal("", lines[1]);
    }

    [Fact]
    public void BuildSaveLines_首列为空则整行是空串_原文如此不跳过()
    {
        var g = NewGrid();
        g[1, 3] = "1.1.1.1";            // 只在没有首列的格子里写东西

        List<string> lines = AddrEditLogic.BuildSaveLines(g);

        Assert.Equal("", lines[2]);
    }

    [Fact]
    public void SaveToFile_落盘文本为逐行CRLF_包含空行()
    {
        var g = NewGrid();
        g.RowCount = 3;
        g[0, 1] = "1.1.1.1";
        g[1, 1] = "2.2.2.2";
        g[2, 1] = "100";

        string file = Path2("!ServerInfo.txt");
        AddrEditLogic.SaveToFile(g, file);

        // 第 1 行：'1.1.1.1 2.2.2.2 100 ' + 第 3..8 列（空）各补一个空格 = 行尾共 7 个空格
        // 第 2 行：首列为空 → 空串（原文如此，写出一行空行）
        string expected = "1.1.1.1 2.2.2.2 100 " + new string(' ', 6) + "\r\n\r\n";
        Assert.Equal(expected, File.ReadAllText(file));
    }

    [Fact]
    public void SaveToFile_写盘失败时ShowMessage保存异常()
    {
        using var ui = new UiRecorder();
        var g = NewGrid();
        string bad = Path.Combine(Dir, "no-such-dir", "x.txt");

        AddrEditLogic.SaveToFile(g, bad);

        Assert.Single(ui.ShowMessages);
        Assert.Equal(bad + " 保存异常！！！", ui.ShowMessages[0]);
    }

    [Fact]
    public void OpenInto_按空格拆分到1加2乘4列()
    {
        var g = NewGrid();
        string file = Path2("!ServerInfo.txt");
        File.WriteAllText(file, "1.1.1.1 2.2.2.2 100 3.3.3.3 200\r\n");

        AddrEditLogic.OpenInto(g, file);

        Assert.Equal("1.1.1.1", g[0, 1]);
        Assert.Equal("2.2.2.2", g[1, 1]);
        Assert.Equal("100", g[2, 1]);
        Assert.Equal("3.3.3.3", g[3, 1]);
        Assert.Equal("200", g[4, 1]);
        Assert.Equal("", g[5, 1]);
        Assert.Equal("", g[8, 1]);
    }

    [Fact]
    public void OpenInto_四组网关正好写满九列()
    {
        var g = NewGrid();
        string file = Path2("!ServerInfo.txt");
        File.WriteAllText(file, "1.1.1.1 2.2.2.2 100 3.3.3.3 200 4.4.4.4 300 5.5.5.5 400\r\n");

        AddrEditLogic.OpenInto(g, file);

        Assert.Equal("5.5.5.5", g[7, 1]);
        Assert.Equal("400", g[8, 1]);
    }

    [Fact]
    public void OpenInto_超过四组网关第五组越界抛异常_原文EInvalidGridIndex()
    {
        var g = NewGrid();
        string file = Path2("!ServerInfo.txt");
        File.WriteAllText(file, "1.1.1.1 2.2.2.2 100 3.3.3.3 200 4.4.4.4 300 5.5.5.5 400 6.6.6.6 500\r\n");

        Assert.Throws<IndexOutOfRangeException>(() => AddrEditLogic.OpenInto(g, file));
    }

    [Fact]
    public void OpenInto_跳过分号行且多行时自动扩RowCount()
    {
        var g = NewGrid();
        string file = Path2("!ServerInfo.txt");
        var sb = new System.Text.StringBuilder();
        sb.Append(";注释\r\n");
        for (int i = 1; i <= 12; i++) sb.Append($"1.1.1.{i} 2.2.2.2 100\r\n");
        File.WriteAllText(file, sb.ToString());

        AddrEditLogic.OpenInto(g, file);

        Assert.True(g.RowCount > AddrEditLogic.InitialRowCount);
        Assert.Equal("1.1.1.12", g[0, 12]);
    }

    [Fact]
    public void OpenInto_文件不存在时ShowMessage读取异常()
    {
        using var ui = new UiRecorder();
        var g = NewGrid();
        string missing = Path2("nope.txt");

        AddrEditLogic.OpenInto(g, missing);

        Assert.Single(ui.ShowMessages);
        Assert.Equal(missing + " 读取异常！！！", ui.ShowMessages[0]);
    }

    [Fact]
    public void OpenInto_先清空数据区但保留表头()
    {
        var g = NewGrid();
        AddrEditLogic.FormCreate(g);
        g[0, 1] = "stale";
        g[1, 1] = "stale";
        string file = Path2("empty.txt");
        File.WriteAllText(file, "");

        AddrEditLogic.OpenInto(g, file);

        Assert.Equal("", g[0, 1]);
        Assert.Equal("", g[1, 1]);
        Assert.Equal("角色选择网关地址", g[0, 0]);
    }

    [Fact]
    public void ClearDataCells_只清第1行起的全部列()
    {
        var g = NewGrid();
        AddrEditLogic.FormCreate(g);
        g[0, 0] = "表头";
        g[0, 1] = "x";
        g[8, 8] = "y";

        AddrEditLogic.ClearDataCells(g);

        Assert.Equal("表头", g[0, 0]);
        Assert.Equal("", g[0, 1]);
        Assert.Equal("", g[8, 8]);
    }

    [Fact]
    public void 往返_写出的Tab分隔文件能被OpenInto正确读回()
    {
        var g1 = NewGrid();
        g1.RowCount = 2;
        g1[0, 1] = "1.1.1.1";
        g1[1, 1] = "2.2.2.2";
        g1[2, 1] = "100";
        string file = Path2("round.txt");
        AddrEditLogic.SaveToFile(g1, file);

        var g2 = NewGrid();
        AddrEditLogic.OpenInto(g2, file);

        Assert.Equal("1.1.1.1", g2[0, 1]);
        Assert.Equal("2.2.2.2", g2[1, 1]);
        Assert.Equal("100", g2[2, 1]);
    }

    [Fact]
    public void DataGridView与模型双向同步()
    {
        using var form = new FrmEditAddr();
        form.FormCreate(null, EventArgs.Empty);
        form.Grid.SetCell(0, 1, "9.9.9.9");

        form.SyncViewFromModel();
        Assert.Equal("9.9.9.9", Convert.ToString(form.AddrGrid.Rows[1].Cells[0].Value));

        form.AddrGrid.Rows[1].Cells[1].Value = "8.8.8.8";
        form.SyncModelFromView();
        Assert.Equal("8.8.8.8", form.Grid.GetCell(1, 1));
    }
}

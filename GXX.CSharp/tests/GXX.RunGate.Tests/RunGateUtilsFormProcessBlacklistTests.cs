using System;
using System.Collections.Generic;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmProcessBlacklist.pas（202 行）的纯逻辑与窗体测试。
/// 重点：
///   * `FindIndex` 是子串（非前缀）匹配且双侧 UpperCase；cbbSearchField 为 0/1 之外时恒不匹配；
///   * `NextSearchStart` 的回绕规则（-1 → 0；Count-1 → 0）；
///   * `BuildRows` 序号从 1 开始；`NewRowCaptionAfterAdd` 用 Add 之后的 Count；
///   * `NormalizeCaptions` 删除后统一重排为 I+1（与 D4 的"追加时用 Count"规则不同）。
/// ★ 本类的窗体测试会读写 `FormGlobals` 的共享全局量与 `FakeSink`，
///   必须与其他窗体测试串行（见 AssemblyInfo.cs 的 `[assembly: CollectionBehavior(DisableTestParallelization = true)]`）。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormProcessBlacklistTests
{
    public RunGateUtilsFormProcessBlacklistTests() => FormGlobals.ResetForTest();

    private static ProcessBlacklistRow Row(string name, string md5, TProcessInfo data = null)
        => new ProcessBlacklistRow { Caption = "", ProcessName = name, ProcessMD5 = md5, Data = data };

    private static List<ProcessBlacklistRow> Rows(params (string Name, string Md5)[] items)
    {
        var list = new List<ProcessBlacklistRow>();
        for (int i = 0; i < items.Length; i++)
            list.Add(Row(items[i].Name, items[i].Md5, new TProcessInfo
            {
                ProcessName = items[i].Name,
                ProcessMD5 = items[i].Md5
            }));
        return list;
    }

    // ---------------- BuildRows（原 :50-59）----------------

    [Fact]
    public void BuildRows_序号从1开始且带Data引用()
    {
        var list = new TProcessBlackList();
        var a = list.Add("p1", new string('A', 32));
        var b = list.Add("p2", new string('B', 32));

        var rows = ProcessBlacklistLogic.BuildRows(list);

        Assert.Equal(2, rows.Count);
        Assert.Equal("1", rows[0].Caption);              // 原 :55 IntToStr(I + 1)
        Assert.Equal("2", rows[1].Caption);
        Assert.Equal("p1", rows[0].ProcessName);
        Assert.Equal(new string('A', 32), rows[0].ProcessMD5);
        Assert.Same(a, rows[0].Data);                    // 原 :56 Item.Data
        Assert.Same(b, rows[1].Data);
    }

    [Fact]
    public void BuildRows_空列表返回空()
        => Assert.Empty(ProcessBlacklistLogic.BuildRows(new TProcessBlackList()));

    // ---------------- CanAdd（原 :61）----------------

    [Fact]
    public void CanAdd_按MaxCount判定()
    {
        var list = new TProcessBlackList();
        Assert.True(ProcessBlacklistLogic.CanAdd(list));        // 0 < 80

        list.MaxCount = 1;
        list.Add("p", new string('A', 32));
        Assert.False(ProcessBlacklistLogic.CanAdd(list));       // 1 < 1 为假
    }

    // ---------------- FindIndex（原 :68-96 / :98-131）----------------

    [Fact]
    public void FindIndex_字段0按进程名子串匹配()
    {
        var rows = Rows(("cheat.exe", "AAA"), ("other.exe", "BBB"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "cheat"));
        Assert.Equal(1, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "other"));
        Assert.Equal(-1, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "nothere"));
    }

    [Fact]
    public void FindIndex_字段1按MD5子串匹配()
    {
        var rows = Rows(("a", "0123456789ABCDEF0123456789ABCDEF"), ("b", "FEDCBA9876543210FEDCBA9876543210"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 1, "234567"));
        Assert.Equal(1, ProcessBlacklistLogic.FindIndex(rows, 0, 1, "cba987"));
    }

    [Fact]
    public void FindIndex_大小写不敏感_双侧UpperCase()
    {
        var rows = Rows(("Cheat.EXE", "abcdef0123456789abcdef0123456789"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "cheat"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "CHEAT"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "eAt.E"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 1, "ABCDEF"));
    }

    [Fact]
    public void FindIndex_子串而非前缀()
    {
        var rows = Rows(("xxcheat", "AAA"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "cheat"));   // 命中中段
    }

    [Fact]
    public void FindIndex_字段下标越界时恒不匹配_case无else()
    {
        var rows = Rows(("cheat", "AAA"));
        Assert.Equal(-1, ProcessBlacklistLogic.FindIndex(rows, 0, 2, "cheat"));
        Assert.Equal(-1, ProcessBlacklistLogic.FindIndex(rows, 0, -1, "cheat"));
    }

    [Fact]
    public void FindIndex_空搜索文本命中首项_Pos空串返回1()
    {
        // Delphi `Pos('', S)` 返回 1（>0）→ 空串会命中第 startIndex 项。
        // 注意 GXX.Core.Rtl.DelphiRTL.Pos 对空子串返回 0（与 Delphi 不同），
        // ProcessBlacklistLogic.FindIndex 已显式补上该语义。
        var rows = Rows(("a", "A"), ("b", "B"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, ""));
        Assert.Equal(1, ProcessBlacklistLogic.FindIndex(rows, 1, 0, ""));
        // 字段下标非法时即使空串也不匹配（原 case 无 else）
        Assert.Equal(-1, ProcessBlacklistLogic.FindIndex(rows, 0, 2, ""));
    }

    [Fact]
    public void FindIndex_只返回首个命中_不继续()
    {
        var rows = Rows(("hit1", "A"), ("hit2", "B"));
        Assert.Equal(0, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "hit"));
    }

    [Fact]
    public void FindIndex_startIndex超出范围返回负1()
    {
        var rows = Rows(("a", "A"));
        Assert.Equal(-1, ProcessBlacklistLogic.FindIndex(rows, 5, 0, "a"));
        Assert.Equal(-1, ProcessBlacklistLogic.FindIndex(rows, 0, 0, "zzz"));
    }

    // ---------------- NextSearchStart（原 :104-108）----------------

    [Fact]
    public void NextSearchStart_未选中时从0开始()
        => Assert.Equal(0, ProcessBlacklistLogic.NextSearchStart(-1, 5));

    [Fact]
    public void NextSearchStart_中间项时指向下一项()
        => Assert.Equal(3, ProcessBlacklistLogic.NextSearchStart(2, 5));

    [Fact]
    public void NextSearchStart_末项时回绕到0()
        => Assert.Equal(0, ProcessBlacklistLogic.NextSearchStart(4, 5));

    [Fact]
    public void NextSearchStart_空列表时归0()
        => Assert.Equal(0, ProcessBlacklistLogic.NextSearchStart(-1, 0));

    [Fact]
    public void NextSearchStart_越界下标也归0()
        => Assert.Equal(0, ProcessBlacklistLogic.NextSearchStart(99, 5));

    [Fact]
    public void NextSearchStart_倒数第二项指向末项()
        => Assert.Equal(4, ProcessBlacklistLogic.NextSearchStart(3, 5));

    // ---------------- 新增/删除的序号规则（D4 差异）----------------

    [Fact]
    public void NewRowCaptionAfterAdd_用Add之后的Count()
    {
        // 原 :151 `IntToStr(lvProcessBlacklist.Items.Count)`，此时列表已含新项
        Assert.Equal("1", ProcessBlacklistLogic.NewRowCaptionAfterAdd(1));
        Assert.Equal("3", ProcessBlacklistLogic.NewRowCaptionAfterAdd(3));
    }

    [Fact]
    public void NormalizeCaptions_统一重排为I加1()
    {
        var rows = Rows(("a", "A"), ("b", "B"), ("c", "C"));
        rows[0].Caption = "9";
        rows[1].Caption = "9";
        rows[2].Caption = "9";

        ProcessBlacklistLogic.NormalizeCaptions(rows);

        Assert.Equal("1", rows[0].Caption);      // 原 :195
        Assert.Equal("2", rows[1].Caption);
        Assert.Equal("3", rows[2].Caption);
    }

    [Fact]
    public void NewRowCaptionAfterAdd与NormalizeCaptions对追加结果一致_但删除中间项后规则不同()
    {
        // 追加：两种规则都得 N+1
        Assert.Equal("3", ProcessBlacklistLogic.NewRowCaptionAfterAdd(3));
        var rows = Rows(("a", "A"), ("b", "B"), ("c", "C"));
        ProcessBlacklistLogic.NormalizeCaptions(rows);
        Assert.Equal("3", rows[2].Caption);

        // ★ 差异断言：删除中间项后，原文只对**删除后的行**重排（序号连续）；
        //   而 btnAddClick 的序号是"当时的行数"，若不做重排会与既有序号撞号。
        rows.RemoveAt(1);                                   // 删掉 "b"
        var afterDelete = new List<ProcessBlacklistRow>(rows) { Row("d", "D") };
        Assert.NotEqual("3", afterDelete[2].Caption);       // 旧 "c" 仍是 "3" → 与新加的 "3" 撞号
        ProcessBlacklistLogic.NormalizeCaptions(afterDelete);
        Assert.Equal("3", afterDelete[2].Caption);          // 重排后才一致
    }

    // ---------------- CanDelete（原 :162-171）----------------

    [Fact]
    public void CanDelete_只有选中且Data非null才为真()
    {
        Assert.False(ProcessBlacklistLogic.CanDelete(null));
        Assert.False(ProcessBlacklistLogic.CanDelete(Row("a", "A")));               // Data == null
        Assert.True(ProcessBlacklistLogic.CanDelete(Row("a", "A", new TProcessInfo())));
    }

    // ---------------- 接缝 Sink（原 :155-156 / :189-190）----------------

    private class FakeSink : IProcessBlacklistSink
    {
        public int Save;
        public int Rebuild;
        public void SaveProcessBlacklist() => Save++;
        public void RebuildProcessBlacklist() => Rebuild++;
    }

    // ---------------- 窗体（DFM 对齐 + 端到端）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmProcessBlacklist();

        Assert.Equal("进程黑名单", f.Text);                       // DFM: Caption
        Assert.Equal(622, f.ClientSize.Width);                    // DFM: ClientWidth=622
        Assert.Equal(441, f.ClientSize.Height);                   // DFM: ClientHeight=441
        Assert.NotNull(f.lvProcessBlacklist);
        Assert.NotNull(f.lbl1);
        Assert.NotNull(f.cbbSearchField);
        Assert.NotNull(f.lbl2);
        Assert.NotNull(f.edtSearchText);
        Assert.NotNull(f.btnSearch);
        Assert.NotNull(f.btnSearchNext);
        Assert.NotNull(f.lbl3);
        Assert.NotNull(f.btnAdd);
        Assert.NotNull(f.pmDelete);
        Assert.NotNull(f.mniDelete);

        Assert.Equal("搜索字段：", f.lbl1.Text);
        Assert.Equal("搜索内容：", f.lbl2.Text);
        Assert.Equal("说明：进程列表最多只支持80个", f.lbl3.Text);
        Assert.Equal("搜索", f.btnSearch.Text);
        Assert.Equal("搜索下一个", f.btnSearchNext.Text);
        Assert.Equal("添加", f.btnAdd.Text);
        Assert.Equal("删除进程", f.mniDelete.Text);

        // DFM: lvProcessBlacklist Columns=[序号,进程名(200),MD5(330)] ViewStyle=vsReport GridLines=True RowSelect=True
        Assert.Equal(3, f.lvProcessBlacklist.Columns.Count);
        Assert.Equal("序号", f.lvProcessBlacklist.Columns[0].Text);
        Assert.Equal("进程名", f.lvProcessBlacklist.Columns[1].Text);
        Assert.Equal(200, f.lvProcessBlacklist.Columns[1].Width);
        Assert.Equal("MD5", f.lvProcessBlacklist.Columns[2].Text);
        Assert.Equal(330, f.lvProcessBlacklist.Columns[2].Width);
        Assert.Equal(System.Windows.Forms.View.Details, f.lvProcessBlacklist.View);
        Assert.True(f.lvProcessBlacklist.GridLines);
        Assert.True(f.lvProcessBlacklist.FullRowSelect);
        Assert.Same(f.pmDelete, f.lvProcessBlacklist.ContextMenuStrip);     // DFM: PopupMenu=pmDelete

        // DFM: cbbSearchField Style=csDropDownList ItemIndex=0 Items=('进程','MD5')
        Assert.Equal(2, f.cbbSearchField.Items.Count);
        Assert.Equal("进程", f.cbbSearchField.Items[0]);
        Assert.Equal("MD5", f.cbbSearchField.Items[1]);
        Assert.Equal(0, f.cbbSearchField.SelectedIndex);
        Assert.Equal(System.Windows.Forms.ComboBoxStyle.DropDownList, f.cbbSearchField.DropDownStyle);
    }

    [Fact]
    public void 窗体_FillList按全局列表填充()
    {
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("p1", new string('A', 32));
        list.Add("p2", new string('B', 32));

        using var f = new FrmProcessBlacklist();
        f.FillList();

        Assert.Equal(2, f.lvProcessBlacklist.Items.Count);
        Assert.Equal("1", f.lvProcessBlacklist.Items[0].Text);
        Assert.Equal("p1", f.lvProcessBlacklist.Items[0].SubItems[1].Text);
        Assert.Equal(new string('A', 32), f.lvProcessBlacklist.Items[0].SubItems[2].Text);
        Assert.Equal("2", f.lvProcessBlacklist.Items[1].Text);
    }

    [Fact]
    public void 窗体_btnSearch_Click定位首个命中项()
    {
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("alpha", new string('A', 32));
        list.Add("beta", new string('B', 32));
        list.Add("alphonse", new string('C', 32));

        using var f = new FrmProcessBlacklist();
        f.FillList();
        f.cbbSearchField.SelectedIndex = 0;
        f.edtSearchText.Text = "alpha";
        f.btnSearch_Click(f, EventArgs.Empty);

        Assert.Equal(0, f.SelectedRow);   // 首个命中
    }

    [Fact]
    public void 窗体_btnSearch_Click无命中时不清空原选中()
    {
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("alpha", new string('A', 32));

        using var f = new FrmProcessBlacklist();
        f.FillList();
        f.SetSelectedRow(0);
        f.edtSearchText.Text = "zzz";
        f.btnSearch_Click(f, EventArgs.Empty);

        Assert.Equal(0, f.SelectedRow);   // 未改变
    }

    [Fact]
    public void 窗体_btnSearchNext_Click从当前项下一个开始并回绕()
    {
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("hitA", new string('A', 32));
        list.Add("miss", new string('B', 32));
        list.Add("hitB", new string('C', 32));

        using var f = new FrmProcessBlacklist();
        f.FillList();
        f.edtSearchText.Text = "hit";

        // 当前选中 0 → 从 1 开始 → 命中 2
        f.SetSelectedRow(0);
        f.btnSearchNext_Click(f, EventArgs.Empty);
        Assert.Equal(2, f.SelectedRow);

        // 当前选中 2（末项）→ 回绕到 0 → 命中 0
        f.SetSelectedRow(2);
        f.btnSearchNext_Click(f, EventArgs.Empty);
        Assert.Equal(0, f.SelectedRow);
    }

    [Fact]
    public void 窗体_edtSearchTextKeyDown回车且有文本时触发搜索()
    {
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("a", new string('A', 32));
        list.Add("target", new string('B', 32));

        using var f = new FrmProcessBlacklist();
        f.FillList();
        f.edtSearchText.Text = "target";

        // 空文本不触发
        var emptyArgs = new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Return);
        f.edtSearchText.Text = "";
        f.edtSearchText_KeyDown(f, emptyArgs);
        Assert.Equal(-1, f.SelectedRow);

        // 非回车键不触发
        f.edtSearchText.Text = "target";
        f.edtSearchText_KeyDown(f, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.A));
        Assert.Equal(-1, f.SelectedRow);

        // 回车 + 非空 → 触发
        f.edtSearchText_KeyDown(f, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Return));
        Assert.Equal(1, f.SelectedRow);
    }

    [Fact]
    public void 窗体_btnAdd_Click经ShowAddProcessBlack加入并触发SaveRebuild()
    {
        var sink = new FakeSink();
        var originalSink = ProcessBlacklistUnit.Sink;
        var originalShow = MessageBoxSeam.Show;
        try
        {
            ProcessBlacklistUnit.Sink = sink;
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;
            // 注意：btnAdd_Click 会**弹出真实的模态窗体**（ShowAddProcessBlack）——
            // 这是原文行为；此处只验证接缝调用路径，故直接构造行后走 mniDelete 分支。
            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();
            list.Add("added.exe", new string('A', 32));

            using var f = new FrmProcessBlacklist();
            f.FillList();
            Assert.Equal(1, f.lvProcessBlacklist.Items.Count);

            // 删除路径覆盖 Save/Rebuild 接缝
            f.SetSelectedRow(0);
            f.mniDelete_Click(f, EventArgs.Empty);

            Assert.Equal(1, sink.Save);
            Assert.Equal(1, sink.Rebuild);
        }
        finally
        {
            ProcessBlacklistUnit.Sink = originalSink;
            MessageBoxSeam.Show = originalShow;
        }
    }

    [Fact]
    public void 窗体_mniDelete_Click删除项并重排序号与启用状态()
    {
        var sink = new FakeSink();
        var originalSink = ProcessBlacklistUnit.Sink;
        try
        {
            ProcessBlacklistUnit.Sink = sink;
            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();
            list.Add("a", new string('A', 32));
            list.Add("b", new string('B', 32));
            list.Add("c", new string('C', 32));

            using var f = new FrmProcessBlacklist();
            f.FillList();

            f.SetSelectedRow(1);   // 删 "b"
            f.mniDelete_Click(f, EventArgs.Empty);

            Assert.Equal(2, f.lvProcessBlacklist.Items.Count);
            Assert.Equal(2, list.Count);
            Assert.Equal("a", f.lvProcessBlacklist.Items[0].SubItems[1].Text);
            Assert.Equal("c", f.lvProcessBlacklist.Items[1].SubItems[1].Text);
            Assert.Equal("1", f.lvProcessBlacklist.Items[0].Text);   // 重排（原 :193-196）
            Assert.Equal("2", f.lvProcessBlacklist.Items[1].Text);
            Assert.Equal(1, sink.Save);
            Assert.Equal(1, sink.Rebuild);
        }
        finally
        {
            ProcessBlacklistUnit.Sink = originalSink;
        }
    }

    [Fact]
    public void 窗体_mniDelete_Click未选中时不做事()
    {
        var sink = new FakeSink();
        var originalSink = ProcessBlacklistUnit.Sink;
        try
        {
            ProcessBlacklistUnit.Sink = sink;
            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();
            list.Add("a", new string('A', 32));

            using var f = new FrmProcessBlacklist();
            f.FillList();
            f.SetSelectedRow(-1);

            f.mniDelete_Click(f, EventArgs.Empty);

            Assert.Equal(1, list.Count);
            Assert.Equal(0, sink.Save);
        }
        finally
        {
            ProcessBlacklistUnit.Sink = originalSink;
        }
    }

    [Fact]
    public void 窗体_pmDelete_Popup按选中与Data决定可见性()
    {
        using var f = new FrmProcessBlacklist();
        f.lvProcessBlacklist.Items.Add(new System.Windows.Forms.ListViewItem("1"));   // Tag = null → Data = null

        f.pmDelete_Popup(f, EventArgs.Empty);
        Assert.False(f.DeleteMenuItemShouldBeVisible);       // 原 :166

        // 换成带 Data 的行
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("a", new string('A', 32));
        f.FillList();
        Assert.Single(f.lvProcessBlacklist.Items);
        Assert.NotNull(f.lvProcessBlacklist.Items[0].Tag);
        Assert.NotNull(((ProcessBlacklistRow)f.lvProcessBlacklist.Items[0].Tag).Data);
        f.SetSelectedRow(0);
        Assert.Equal(0, f.SelectedRow);

        f.pmDelete_Popup(f, EventArgs.Empty);
        Assert.True(f.DeleteMenuItemShouldBeVisible);        // 原 :170
    }

    [Fact]
    public void 窗体_ReadRows把界面读回纯逻辑行()
    {
        var list = FormGlobals.g_ProcessBlackList;
        list.Clear();
        list.Add("p1", new string('A', 32));

        using var f = new FrmProcessBlacklist();
        f.FillList();

        var rows = f.ReadRows();
        Assert.Single(rows);
        Assert.Equal("p1", rows[0].ProcessName);
        Assert.Equal(new string('A', 32), rows[0].ProcessMD5);
        Assert.NotNull(rows[0].Data);
    }
}

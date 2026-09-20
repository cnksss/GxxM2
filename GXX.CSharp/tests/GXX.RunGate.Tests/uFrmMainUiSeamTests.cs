// 测试：uFrmMain.pas 的 UI 接缝垫片（uFrmMainUiSeams.cs）
//   核心断言：Delphi 的 `ItemIndex := 越界值` **静默置 -1**，WinForms 原生会抛 —— 垫片必须复刻前者。
using System;
using System.Windows.Forms;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

[Collection("RunGateFormLane")]      // 触碰 WinForms 控件
public sealed class uFrmMainUiSeamTests
{
    // ================= DelphiItemIndex（纯函数） =================

    [Fact]
    public void DelphiItemIndex_合法值原样返回()
    {
        Assert.Equal(0, RunGateUiSeams.DelphiItemIndex(0, 11));
        Assert.Equal(10, RunGateUiSeams.DelphiItemIndex(10, 11));
        Assert.Equal(0, RunGateUiSeams.DelphiItemIndex(0, 1));
    }

    [Fact]
    public void DelphiItemIndex_越界与负数静默置负一()
    {
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(11, 11));      // ★ C1 的命中点：钳位目标是 Items.Count=11
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(99, 11));
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(-1, 11));      // 无选中
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(-5, 11));
    }

    [Fact]
    public void DelphiItemIndex_空列表任何值都是负一()
    {
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(0, 0));
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(-1, 0));
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(int.MaxValue, 0));
    }

    [Fact]
    public void DelphiItemIndex_极大值与极小值()
    {
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(int.MaxValue, 11));
        Assert.Equal(-1, RunGateUiSeams.DelphiItemIndex(int.MinValue, 11));
    }

    // ================= ComboBox 垫片（真实控件） =================

    /// <summary>造一个与 `uFrmMain.dfm:715-735` 同形的 `cbbShowLogLevel`：11 项。</summary>
    private static ComboBox ShowLogLevelCombo()
    {
        var cb = new ComboBox();
        for (int i = 0; i <= 10; i++) cb.Items.Add(i + "级");
        return cb;
    }

    [Fact]
    public void SetComboItemIndex_WinForms原生会抛而垫片不抛()
    {
        using var cb = ShowLogLevelCombo();

        // 先证明原生行为确实会抛（否则这条垫片就没意义）
        Assert.Throws<ArgumentOutOfRangeException>(() => cb.SelectedIndex = 11);

        // 垫片：越界 → -1，不抛
        var ex = Record.Exception(() => RunGateUiSeams.SetComboItemIndex(cb, 11));
        Assert.Null(ex);
        Assert.Equal(-1, cb.SelectedIndex);
    }

    [Fact]
    public void SetComboItemIndex_合法值正常选中()
    {
        using var cb = ShowLogLevelCombo();
        RunGateUiSeams.SetComboItemIndex(cb, 5);
        Assert.Equal(5, cb.SelectedIndex);
        Assert.Equal("5级", cb.SelectedItem?.ToString());
    }

    [Fact]
    public void SetComboItemIndex_ShowLogLevel钳位到11时不崩_原文缺陷C1()
    {
        // ★ 完整复刻 C1 的路径：`g_btShowLogLevel` 被钳到 Items.Count(11) → 赋给 ItemIndex
        using var cb = ShowLogLevelCombo();
        int clampedShowLogLevel = 11;                        // 原 :1112-1113 的钳位结果
        RunGateUiSeams.SetComboItemIndex(cb, clampedShowLogLevel);
        Assert.Equal(-1, cb.SelectedIndex);                  // Delphi 也是这样（静默 -1）
    }

    [Fact]
    public void SetComboItemIndex_null与空列表不抛()
    {
        Assert.Null(Record.Exception(() => RunGateUiSeams.SetComboItemIndex(null, 3)));
        using var empty = new ComboBox();
        Assert.Null(Record.Exception(() => RunGateUiSeams.SetComboItemIndex(empty, 0)));
        Assert.Equal(-1, empty.SelectedIndex);
    }

    // ================= ListBox 垫片 =================

    [Fact]
    public void SetListItemIndex_越界静默置负一()
    {
        using var lb = new ListBox();
        lb.Items.Add("a");
        lb.Items.Add("b");

        Assert.Throws<ArgumentOutOfRangeException>(() => lb.SelectedIndex = 5);
        Assert.Null(Record.Exception(() => RunGateUiSeams.SetListItemIndex(lb, 5)));
        Assert.Equal(-1, lb.SelectedIndex);
    }

    [Fact]
    public void SetListItemIndex_合法值与null()
    {
        using var lb = new ListBox();
        lb.Items.AddRange(new object[] { "a", "b", "c" });
        RunGateUiSeams.SetListItemIndex(lb, 2);
        Assert.Equal(2, lb.SelectedIndex);
        Assert.Null(Record.Exception(() => RunGateUiSeams.SetListItemIndex(null, 0)));
    }

    [Fact]
    public void SetListItemIndex_负数与空列表()
    {
        using var empty = new ListBox();
        RunGateUiSeams.SetListItemIndex(empty, -3);
        Assert.Equal(-1, empty.SelectedIndex);
    }

    // ================= 文本读取 =================

    [Fact]
    public void ComboTextForItemIndex_无选中返回空串()
    {
        using var cb = ShowLogLevelCombo();
        Assert.Equal("", RunGateUiSeams.ComboTextForItemIndex(cb));      // 未选中
        RunGateUiSeams.SetComboItemIndex(cb, 3);
        Assert.Equal("3级", RunGateUiSeams.ComboTextForItemIndex(cb));
        RunGateUiSeams.SetComboItemIndex(cb, 99);                        // 越界 → -1
        Assert.Equal("", RunGateUiSeams.ComboTextForItemIndex(cb));
    }

    [Fact]
    public void ComboTextForItemIndex_null返回空串()
        => Assert.Equal("", RunGateUiSeams.ComboTextForItemIndex(null));

    [Fact]
    public void ItemTextOr_越界返回默认值()
    {
        using var cb = ShowLogLevelCombo();
        Assert.Equal("0级", RunGateUiSeams.ItemTextOr(cb, 0));
        Assert.Equal("10级", RunGateUiSeams.ItemTextOr(cb, 10));
        Assert.Equal("", RunGateUiSeams.ItemTextOr(cb, 11));
        Assert.Equal("none", RunGateUiSeams.ItemTextOr(cb, -1, "none"));
        Assert.Equal("none", RunGateUiSeams.ItemTextOr(null, 0, "none"));
    }
}

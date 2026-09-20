using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmHitInterval.pas（85 行）的纯逻辑与窗体测试。
/// 重点：与 uFrmInterval 的**校验差异**（本窗体判 `Value = 0`，uFrmInterval 判 `Value &lt;= 0`）、
///       INI 键名 Speed0..SpeedN（从 0 开始，无负号）、表格填充规则。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormHitIntervalTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormHitIntervalTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p2rg_form_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        FormGlobals.ResetForTest();
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string IniPath => Path.Combine(_dir, "HitIntervals.ini");
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";

    // ---------------- BuildGrid（原 :35-43）----------------

    [Fact]
    public void BuildGrid_表头与行文本规则()
    {
        var rows = HitIntervalLogic.BuildGrid(new uint[] { 100, 200, 300 });

        Assert.Equal(4, rows.Count);                                  // Length + 1
        Assert.Equal("攻击加速", rows[0].Key);
        Assert.Equal("间隔检测", rows[0].Value);
        Assert.Equal("攻击加速+0", rows[1].Key);
        Assert.Equal("100", rows[1].Value);
        Assert.Equal("攻击加速+2", rows[3].Key);
        Assert.Equal("300", rows[3].Value);
    }

    [Fact]
    public void BuildGrid_空数组只有表头()
    {
        var rows = HitIntervalLogic.BuildGrid(Array.Empty<uint>());
        Assert.Single(rows);
        Assert.Equal("攻击加速", rows[0].Key);
    }

    [Fact]
    public void BuildGrid_null不抛异常_只有表头()
    {
        var rows = HitIntervalLogic.BuildGrid(null);
        Assert.Single(rows);
    }

    [Fact]
    public void BuildGrid_LongWord值按无符号显示()
    {
        // g_dwHitIntervals 是 LongWord；0xFFFFFFFF 应显示为 4294967295（不是 -1）
        var rows = HitIntervalLogic.BuildGrid(new uint[] { 0xFFFFFFFF });
        Assert.Equal("4294967295", rows[1].Value);
    }

    // ---------------- ValidateAll（原 :58-68）----------------

    [Fact]
    public void ValidateAll_全为正数_通过()
    {
        int bad = HitIntervalLogic.ValidateAll(new[] { "1", "100", "999" }, out string msg);
        Assert.Equal(-1, bad);
        Assert.Equal("", msg);
    }

    [Fact]
    public void ValidateAll_0被拒绝_返回该行下标与提示文本()
    {
        int bad = HitIntervalLogic.ValidateAll(new[] { "5", "0", "7" }, out string msg);
        Assert.Equal(1, bad);
        Assert.Equal("输入的数据必须大于0", msg);        // 原 :63
    }

    [Fact]
    public void ValidateAll_非数字按StrToIntDef退化为0_被拒绝()
    {
        int bad = HitIntervalLogic.ValidateAll(new[] { "abc" }, out string msg);
        Assert.Equal(0, bad);
        Assert.Equal("输入的数据必须大于0", msg);
    }

    [Fact]
    public void ValidateAll_负数被接受_与uFrmInterval不同()
    {
        // ★ 差异断言：原 :61 是 `if Value = 0`（判等），不是 `<= 0`。
        //   uFrmInterval.pas:206 用的是 `if Value <= 0` → 会拒绝负数。两者**不同**。
        int badHere = HitIntervalLogic.ValidateAll(new[] { "-5" }, out string msgHere);
        Assert.Equal(-1, badHere);
        Assert.Equal("", msgHere);

        // 对照：IntervalLogic 会拒绝同一个 "-5"
        int badThere = IntervalLogic.ValidateAll(new[] { "-5" }, out string msgThere);
        Assert.Equal(0, badThere);
        Assert.Equal("输入的数据必须大于0", msgThere);
    }

    [Fact]
    public void ValidateAll_首个非法行优先返回()
    {
        int bad = HitIntervalLogic.ValidateAll(new[] { "1", "0", "0" }, out _);
        Assert.Equal(1, bad);
    }

    [Fact]
    public void ValidateAll_空集合通过()
    {
        Assert.Equal(-1, HitIntervalLogic.ValidateAll(Array.Empty<string>(), out _));
    }

    // ---------------- ApplyValues（原 :70）----------------

    [Fact]
    public void ApplyValues_写回全局数组_负数按32位补码截断()
    {
        var arr = new uint[3];
        HitIntervalLogic.ApplyValues(arr, new[] { "10", "-5", "0x1F" });
        Assert.Equal(10u, arr[0]);
        Assert.Equal(unchecked((uint)(-5)), arr[1]);       // LongWord 语义
        Assert.Equal(0u, arr[2]);                          // DelphiRTL.StrToIntDef 不认 0x 前缀 → 0
    }

    // ---------------- Save（原 :73-79）----------------

    [Fact]
    public void Save_INI节名Intervals与键名Speed0起()
    {
        HitIntervalLogic.Save(IniPath, new uint[] { 111, 222 });

        // ★ 与 uFrmInterval 的差异：这里是 'Speed0'..'SpeedN'（从 0 开始且无负号）；
        //   uFrmInterval 写的是 'Speed' + (I - 200) → Speed-200..Speed200。
        Assert.Contains("[Intervals]\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("Speed0=111\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("Speed1=222\r\n", IniText, StringComparison.Ordinal);
        Assert.DoesNotContain("Speed-", IniText, StringComparison.Ordinal);
        Assert.DoesNotContain("Speed2=", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void Save_空文件名不抛异常()
    {
        var ex = Record.Exception(() => HitIntervalLogic.Save("", new uint[] { 1 }));
        Assert.Null(ex);
    }

    // ---------------- RunButtonClick（原 :52-82）----------------

    [Fact]
    public void RunButtonClick_校验失败时弹窗且不落盘()
    {
        var arr = new uint[] { 1, 2 };
        var popups = new List<(string Text, string Caption)>();

        bool ok = HitIntervalLogic.RunButtonClick(arr, new[] { "1", "0" }, IniPath,
            (t, c) => popups.Add((t, c)));

        Assert.False(ok);                                        // 原 :67 Exit → 不置 mrOK
        Assert.Single(popups);
        Assert.Equal("输入的数据必须大于0", popups[0].Text);
        Assert.Equal("错误", popups[0].Caption);                 // 原 :63 的标题
        Assert.Equal(new uint[] { 1, 2 }, arr);                  // 未写入
        Assert.False(File.Exists(IniPath));                      // 未落盘
    }

    [Fact]
    public void RunButtonClick_成功时写全局并落盘()
    {
        var arr = new uint[] { 1, 2 };
        bool ok = HitIntervalLogic.RunButtonClick(arr, new[] { "7", "8" }, IniPath, (t, c) => { });

        Assert.True(ok);                                         // 原 :81 ModalResult := mrOK
        Assert.Equal(new uint[] { 7, 8 }, arr);
        Assert.Contains("Speed0=7", IniText, StringComparison.Ordinal);
        Assert.Contains("Speed1=8", IniText, StringComparison.Ordinal);
    }

    // ---------------- 窗体（DFM 对齐）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmHitInterval();

        Assert.Equal("攻击间隔设置", f.Text);                      // DFM: Caption
        Assert.Equal(195, f.ClientSize.Width);                     // DFM: ClientWidth=195
        Assert.Equal(518, f.ClientSize.Height);                    // DFM: ClientHeight=518
        Assert.Equal(new System.Windows.Forms.Padding(5), f.Padding);       // DFM: BorderWidth=5
        Assert.NotNull(f.grpSetting);
        Assert.NotNull(f.grdHitInterval);
        Assert.NotNull(f.pnlBottom);
        Assert.NotNull(f.btn1);
        Assert.Equal("攻击间隔设置", f.grpSetting.Text);            // DFM Caption
        Assert.Equal("确定", f.btn1.Text);                          // DFM Caption='确定'
        Assert.Equal(2, f.grdHitInterval.ColumnCount);              // DFM: ColCount=2
        Assert.Equal(100, f.grdHitInterval.Rows.Count);             // DFM: RowCount=100
    }

    [Fact]
    public void 窗体_FillGrid按全局数组填表()
    {
        FormGlobals.g_dwHitIntervals = new uint[] { 11, 22 };

        using var f = new FrmHitInterval();
        f.FillGrid();

        Assert.Equal("攻击加速", f.grdHitInterval.Rows[0].Cells[0].Value);
        Assert.Equal("间隔检测", f.grdHitInterval.Rows[0].Cells[1].Value);
        Assert.Equal("攻击加速+0", f.grdHitInterval.Rows[1].Cells[0].Value);
        Assert.Equal("11", f.grdHitInterval.Rows[1].Cells[1].Value);
        Assert.Equal("攻击加速+1", f.grdHitInterval.Rows[2].Cells[0].Value);
        Assert.Equal("22", f.grdHitInterval.Rows[2].Cells[1].Value);

        // 读取助手的行数 = 数组长度（不含表头）
        Assert.Equal(2, f.ReadValueColumnTexts().Count);
    }

    [Fact]
    public void 窗体_btn1_Click写入并置DialogResult_非法输入则弹窗不置()
    {
        FormGlobals.g_dwHitIntervals = new uint[] { 1, 1 };
        FormGlobals.g_sHitIntervalsFileName = IniPath;

        var captured = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { captured.Add(t); return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmHitInterval();
            f.FillGrid();

            // 非法：第 1 个数据行为空 → StrToIntDef → 0 → 弹窗
            f.grdHitInterval.Rows[0 + 1].Cells[1].Value = "0";
            f.btn1_Click(f, EventArgs.Empty);
            Assert.Single(captured);
            Assert.Equal("输入的数据必须大于0", captured[0]);
            Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);

            // 合法
            captured.Clear();
            f.grdHitInterval.Rows[1].Cells[1].Value = "9";
            f.grdHitInterval.Rows[2].Cells[1].Value = "8";
            f.btn1_Click(f, EventArgs.Empty);
            Assert.Empty(captured);
            Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
            Assert.Equal(new uint[] { 9, 8 }, FormGlobals.g_dwHitIntervals);
            Assert.Contains("Speed0=9", IniText, StringComparison.Ordinal);
            Assert.Contains("Speed1=8", IniText, StringComparison.Ordinal);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }
}
